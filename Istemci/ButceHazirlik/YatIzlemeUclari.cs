using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using ButceHazirlikv2;
using Istemci.OrtakSayfa;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;
using TNS.YAT;

namespace Istemci.ButceHazirlik
{
    /// <summary>Sayfanın proje / alt proje ve aşama bilgisi (bütün POST isteklerinde ortak).</summary>
    public class YatIzlemeIstegi
    {
        public int Yil { get; set; }
        public int PKod { get; set; }
        public int ApKod { get; set; }
        public int Asama { get; set; }
    }

    /// <summary>Pr. ve Gerç. Durumu (Gerekce) ya da Tamamlama Raporu alanları.</summary>
    public class YatIzlemeKayitIstegi : YatIzlemeIstegi
    {
        public string Gerekce { get; set; }
        public string Amac1 { get; set; }
        public string Fiziki1 { get; set; }
        public string Amac2 { get; set; }
        public string Fiziki2 { get; set; }
        public string Cikti { get; set; }
        public string Fayda { get; set; }
        public string Eksen { get; set; }
        public string Sorunlar { get; set; }
        public string Dersler { get; set; }
    }

    public class YatIzlemeOnayIstegi : YatIzlemeIstegi
    {
        /// <summary>true: Onay Kaldır, false: Onayla.</summary>
        public bool Kaldir { get; set; }
    }

    /// <summary>Genel Değerlendirme penceresinin sekmeleri (HTML).</summary>
    public class YatIzlemeGenelIstegi
    {
        public int Yil { get; set; }
        public int Asama { get; set; }
        public string FinansKaynak { get; set; }
        public string HizmetKapasitesi { get; set; }
        public string KapasiteArtisi { get; set; }
        public string YatirimUygulamalari { get; set; }
        public string Notlar { get; set; }
        public string SorunCozum { get; set; }
    }

    /// <summary>
    /// Yatırım Programı İzleme ve Değerlendirme sayfasının (eski ButceHazirlikv2/YATIzlemeDegerlendirme.aspx; ProjeGiris
    /// islemKod=1'de proje / alt proje seçilince açılır) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/yatizleme/oku?yil=&amp;pKod=&amp;apKod=&amp;asama=   (asama yoksa kullanıcının sakladığı aşama; eski AsamaGetir)
    ///   GET  /api/yatizleme/tuslar?yil=&amp;pKod=&amp;apKod=&amp;asama= (eski cmbAsama_Changed → TuslariAyarla)
    ///   POST /api/yatizleme/asama, /gerekce, /tamamlama, /onay
    ///   GET  /api/yatizleme/genel?yil=&amp;asama=, POST /api/yatizleme/genel (Genel Değerlendirme; yalnızca sistem yöneticisi)
    ///   GET  /api/yatizleme/rapor?n=&amp;yil=&amp;pKod=&amp;apKod=&amp;asama= (Rapor.xml 2004-14, 2004-16; 2004-15, 2004-17 genel)
    /// Metinler KYM büyük metin tablosunda "YAT" modülünde, eski anahtarlarla saklanır (YID{yil}_{p}_{ap}_{aşama}_{alan};
    /// genel değerlendirme YID{yil}_G_{aşama}_{alan}). Onay, birim onay tablosunda YATIZLEMEDEGERLENDIRME türüyle tutulur.
    /// Senaryo eskisi gibi her zaman "06.00"dır (adresteki senaryoKod eski sayfada da kullanılmıyordu).
    /// </summary>
    public static class YatIzlemeUclari
    {
        private const string SayfaAd = "YATIzlemeDegerlendirme";
        private const string Senaryo = "06.00";
        private const string AsamaDegisken = "YAT_IzlDeg";
        private const int Onayli = 5;

        // Eski KontrolBul sırası: anahtarın sonu → JSON alan adı.
        private static readonly string[] ProjeAlanlari = { "Gerekce", "Amac1", "Fiziki1", "Amac2", "Fiziki2", "Cikti", "Fayda", "Eksen", "Sorunlar", "Dersler" };
        private static readonly string[] GenelAlanlar = { "FinansKaynak", "HizmetKapasitesi", "KapasiteArtisi", "YatirimUygulamalari", "Notlar", "SorunCozum" };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/yatizleme/oku", Oku);
            app.MapGet("/api/yatizleme/tuslar", Tuslar);
            app.MapPost("/api/yatizleme/asama", AsamaSakla);
            app.MapPost("/api/yatizleme/gerekce", GerekceKaydet);
            app.MapPost("/api/yatizleme/tamamlama", TamamlamaKaydet);
            app.MapPost("/api/yatizleme/onay", Onay);
            app.MapGet("/api/yatizleme/genel", GenelOku);
            app.MapPost("/api/yatizleme/genel", GenelKaydet);
            app.MapGet("/api/yatizleme/rapor", Rapor);
        }

        // ---------------------------------------------------------------- eski yardımcılar

        /// <summary>Eski AsamaGetir: 1..8 dışındaki değerler 1 sayılır.</summary>
        private static int AsamaDuzelt(int asama)
        {
            return asama >= 1 && asama <= 8 ? asama : 1;
        }

        /// <summary>Eski AyOl: aşamanın ödenek / harcama ayı.</summary>
        private static int AyOl(int asama)
        {
            if (asama == 3 || asama == 4) return 3;
            if (asama == 5 || asama == 6) return 6;
            if (asama == 7 || asama == 8) return 9;
            return 12;
        }

        private static string AnahtarBul(int yil, int pKod, int apKod, int asama, string sonEk)
        {
            return "YID" + yil.ToString() + "_" + pKod.ToString() + "_" + apKod.ToString() + "_" + asama.ToString() + "_" + sonEk;
        }

        private static string AnahtarBulGenel(int yil, int asama, string sonEk)
        {
            return "YID" + yil.ToString() + "_" + "G" + "_" + asama.ToString() + "_" + sonEk;
        }

        private static string AnahtarBulOnay(int pKod, int apKod, int asama)
        {
            return "YID" + pKod.ToString() + "_" + apKod.ToString() + "_" + asama.ToString();
        }

        private static string IlkKucuk(string ad)
        {
            return char.ToLowerInvariant(ad[0]) + ad.Substring(1);
        }

        private static bool ProjeVar(int yil, int pKod, int apKod)
        {
            return yil >= 2000 && (pKod > 0 || apKod > 0);
        }

        /// <summary>Eski ProjeAltProjeYazmaGormeHakkiVarMi denetimi (eski KaydetListe).</summary>
        private static Sonuc GormeHakki(Kullanici kullanan, int yil, int pKod, int apKod)
        {
            Sonuc s = TNS.YAT.Arac.Tanimla().ProjeAltProjeYazmaGormeHakkiVarMi(kullanan, pKod, apKod, yil, Senaryo, TNS.BHMv2.ENUMOGY.Gorme, false);
            return s ?? new Sonuc();
        }

        /// <summary>Eski TuslariAyarla: onaylıysa Kaydet ve Onayla gizli, Onay Kaldır yalnızca yöneticiye görünür.</summary>
        private static object TusDurumu(Kullanici kullanan, int yil, int pKod, int apKod, int asama)
        {
            int tut = TNS.BHMv2.Arac.Tanimla().OnayKodOkuTasra(kullanan, yil, 0, "000", AnahtarBulOnay(pKod, apKod, asama), ENUMBirimOnayTur.YATIZLEMEDEGERLENDIRME);
            bool onayli = tut == Onayli;
            return new
            {
                onayli = onayli,
                kaydet = !onayli,
                onayla = !onayli,
                onayKaldir = onayli && YatProjeOrtak.SistemYoneticisi(kullanan)
            };
        }

        /// <summary>
        /// Eski OdenekTabloOl: projenin / alt projenin bağlı faaliyetlerinin program ödeneği (K.B.Ö.), revize ödenek
        /// (K.B.Ö. + ekleme / düşme), aşamanın ayına kadarki harcama ve harcama oranı. Sayılar eski biçimle (#,##0) yazılır;
        /// tablo bulunamazsa hepsi "-".
        /// </summary>
        private static object OdenekBul(Kullanici kullanan, int yil, int ay, int pKod, int apKod)
        {
            IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
            OdenekBilgi[] odnler = null;
            string pebKodlar = YollukPlanlamaRaporu.PebKodlariBul(kullanan, servisBHMv2, yil, Senaryo, pKod, apKod);
            if (!string.IsNullOrWhiteSpace(pebKodlar))
            {
                Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", "", pebKodlar);
                UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
                kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(0), 1));

                EkDegerParam[] ekBilgiler = new EkDegerParam[3];
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, Senaryo);
                ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABMasrafPlaniForm, 0, 0, ";0/" + (ay + 1).ToString() + ";" + TNS.BHMv2.Arac.EklemeDusmeIslemler() + ";;");
                ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPEBHarcama, 0, 0, ay.ToString());

                odnler = servisBHMv2.ListeTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, false);
            }
            if (odnler != null && odnler.Length == 1)
            {
                CultureInfo tr = CultureInfo.GetCultureInfo("tr-TR");
                double ys = odnler[0].EkRakam(0) + odnler[0].EkRakam(1);
                return new
                {
                    program = odnler[0].EkRakam(0).ToString("#,##0", tr),
                    revize = ys.ToString("#,##0", tr),
                    harcanan = odnler[0].EkRakam(2).ToString("#,##0", tr),
                    oran = OrtakFonksiyonlar.YuzdeBul(odnler[0].EkRakam(2), ys, 2).ToString("#,##0", tr)
                };
            }
            return new { program = "-", revize = "-", harcanan = "-", oran = "-" };
        }

        // ---------------------------------------------------------------- uçlar

        /// <summary>
        /// Eski btnOku_Click (ve açılış): aşamanın metinleri, ödenek tablosu ve tuş durumları. asama verilmezse kullanıcının
        /// sakladığı aşama (eski AsamaGetir) kullanılır. Eskiden okunmayan alanlar önceki değerini koruyordu; burada boş gelir.
        /// </summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int yil, int pKod, int apKod, int? asama)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!ProjeVar(yil, pKod, apKod)) return YatProjeOrtak.Hata("Proje ya da alt proje seçilmemiş.");
            try
            {
                int a = asama.HasValue ? AsamaDuzelt(asama.Value) : AsamaDuzelt(OrtakFonksiyonlar.ConvertToInt(GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, AsamaDegisken), 1));

                Sonuc s = GormeHakki(kullanan, yil, pKod, apKod);
                if (!s.islemSonuc) return YatProjeOrtak.Hata(YatProjeOrtak.HataMetni(s.hataStr, "Bu projeyi görme yetkiniz yok."));

                ObjectArray<StrKodStrAd> oa = TNS.KYM.Arac.Tanimla().BuyukMetinListele(kullanan, "YAT", AnahtarBul(yil, pKod, apKod, a, ""), "tr");
                if (oa == null || oa.sonuc == null || !oa.sonuc.islemSonuc)
                    return YatProjeOrtak.Hata(YatProjeOrtak.HataMetni(oa?.sonuc?.hataStr, "Bilgiler okunamadı."));

                Dictionary<string, string> metinler = new Dictionary<string, string>();
                foreach (string alan in ProjeAlanlari) metinler[IlkKucuk(alan)] = "";
                foreach (StrKodStrAd item in oa.objeler)
                {
                    string kod = item.kod ?? "";
                    foreach (string alan in ProjeAlanlari)
                    {
                        if (kod.EndsWith(alan)) { metinler[IlkKucuk(alan)] = item.ad ?? ""; break; }
                    }
                }

                return Results.Json(new
                {
                    asama = a,
                    yonetici = YatProjeOrtak.SistemYoneticisi(kullanan),
                    metinler = metinler,
                    odenek = OdenekBul(kullanan, yil, AyOl(a), pKod, apKod),
                    tuslar = TusDurumu(kullanan, yil, pKod, apKod, a)
                });
            }
            catch (Exception)
            {
                return YatProjeOrtak.Hata("Bilgiler okunamadı.");
            }
        }

        /// <summary>Eski cmbAsama_Changed: yalnızca tuşlar yeni aşamanın onay durumuna göre ayarlanır.</summary>
        private static IResult Tuslar(HttpContext baglam, IWebHostEnvironment ortam, int yil, int pKod, int apKod, int asama)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!ProjeVar(yil, pKod, apKod)) return YatProjeOrtak.Hata("Proje ya da alt proje seçilmemiş.");
            try
            {
                return Results.Json(new { tuslar = TusDurumu(kullanan, yil, pKod, apKod, AsamaDuzelt(asama)) });
            }
            catch (Exception)
            {
                return YatProjeOrtak.Hata("Onay durumu okunamadı.");
            }
        }

        /// <summary>Eski AsamaSakla (Oku'ya basılınca): seçilen aşama kullanıcı değişkeninde saklanır.</summary>
        private static async Task<IResult> AsamaSakla(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YatIzlemeIstegi istek)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAd);
            if (ret != null) return ret;
            if (istek == null) return YatProjeOrtak.Hata("İstek hatalı.");
            try
            {
                GenelIslemler.KullaniciDegiskenSakla(kullanan, AsamaDegisken, AsamaDuzelt(istek.Asama).ToString());
                return Results.Json(new { basarili = true });
            }
            catch (Exception)
            {
                return YatProjeOrtak.Hata("Aşama saklanamadı.");
            }
        }

        /// <summary>Eski btnKaydet_Click: Pr. ve Gerç. Durumu metni.</summary>
        private static async Task<IResult> GerekceKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YatIzlemeKayitIstegi istek)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAd);
            if (ret != null) return ret;
            if (istek == null || !ProjeVar(istek.Yil, istek.PKod, istek.ApKod)) return YatProjeOrtak.Hata("Proje ya da alt proje seçilmemiş.");
            int asama = AsamaDuzelt(istek.Asama);
            try
            {
                // Eskiden Kaydet tuşu onaylıyken gizlenirdi; burada sunucuda da denetlenir.
                int tut = TNS.BHMv2.Arac.Tanimla().OnayKodOkuTasra(kullanan, istek.Yil, 0, "000", AnahtarBulOnay(istek.PKod, istek.ApKod, asama), ENUMBirimOnayTur.YATIZLEMEDEGERLENDIRME);
                if (tut == Onayli) return YatProjeOrtak.Hata("Bu aşama onaylanmış; değiştirilemez.");
                TNSCollection liste = new TNSCollection();
                liste.Add(new StrKodStrAd(AnahtarBul(istek.Yil, istek.PKod, istek.ApKod, asama, "Gerekce"), istek.Gerekce ?? ""));
                return KaydetListe(kullanan, istek.Yil, istek.PKod, istek.ApKod, asama, liste);
            }
            catch (Exception)
            {
                return YatProjeOrtak.Hata("Kayıt yapılamadı.");
            }
        }

        /// <summary>Eski btnKaydetTamam_Click: Tamamlama Raporu alanları.</summary>
        private static async Task<IResult> TamamlamaKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YatIzlemeKayitIstegi istek)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAd);
            if (ret != null) return ret;
            if (istek == null || !ProjeVar(istek.Yil, istek.PKod, istek.ApKod)) return YatProjeOrtak.Hata("Proje ya da alt proje seçilmemiş.");
            int asama = AsamaDuzelt(istek.Asama);
            try
            {
                TNSCollection liste = new TNSCollection();
                void ekle(string alan, string deger) => liste.Add(new StrKodStrAd(AnahtarBul(istek.Yil, istek.PKod, istek.ApKod, asama, alan), deger ?? ""));
                ekle("Amac1", istek.Amac1);
                ekle("Fiziki1", istek.Fiziki1);
                ekle("Amac2", istek.Amac2);
                ekle("Fiziki2", istek.Fiziki2);
                ekle("Cikti", istek.Cikti);
                ekle("Fayda", istek.Fayda);
                ekle("Eksen", istek.Eksen);
                ekle("Sorunlar", istek.Sorunlar);
                ekle("Dersler", istek.Dersler);
                return KaydetListe(kullanan, istek.Yil, istek.PKod, istek.ApKod, asama, liste);
            }
            catch (Exception)
            {
                return YatProjeOrtak.Hata("Kayıt yapılamadı.");
            }
        }

        /// <summary>Eski KaydetListe: proje görme hakkı; "Kabul Edilen (Yıl Sonu)" aşamasına yalnızca sistem yöneticisi yazar.</summary>
        private static IResult KaydetListe(Kullanici kullanan, int yil, int pKod, int apKod, int asama, TNSCollection liste)
        {
            Sonuc s = GormeHakki(kullanan, yil, pKod, apKod);
            if (s.islemSonuc && asama == 2 && !YatProjeOrtak.SistemYoneticisi(kullanan))
                return YatProjeOrtak.Hata("Bu aşamaya kaydetme yetkiniz bulunmamaktadır!");
            if (s.islemSonuc)
                s = TNS.KYM.Arac.Tanimla().BuyukMetinListeSakla(kullanan, "YAT", "tr", liste);
            if (s == null || !s.islemSonuc)
                return YatProjeOrtak.Hata(YatProjeOrtak.HataMetni(s?.hataStr, "Kayıt yapılamadı."));
            return Results.Json(new { basarili = true, bilgi = "Kayıt tamamlandı" });
        }

        /// <summary>Eski btnOnayla_Click / btnOnayKaldir_Click; yeni tuş durumlarını döndürür.</summary>
        private static async Task<IResult> Onay(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YatIzlemeOnayIstegi istek)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAd);
            if (ret != null) return ret;
            if (istek == null || !ProjeVar(istek.Yil, istek.PKod, istek.ApKod)) return YatProjeOrtak.Hata("Proje ya da alt proje seçilmemiş.");
            // Eskiden Onay Kaldır tuşu yalnızca sistem yöneticisine görünürdü.
            if (istek.Kaldir && !YatProjeOrtak.SistemYoneticisi(kullanan)) return YatProjeOrtak.Hata("Onayı kaldırma yetkiniz yok.");
            int asama = AsamaDuzelt(istek.Asama);
            try
            {
                Sonuc s = GormeHakki(kullanan, istek.Yil, istek.PKod, istek.ApKod);
                if (!s.islemSonuc) return YatProjeOrtak.Hata(YatProjeOrtak.HataMetni(s.hataStr, "Bu projeyi görme yetkiniz yok."));
                s = TNS.BHMv2.Arac.Tanimla().OnayKodAtaTasra(kullanan, istek.Yil, 0, "000", AnahtarBulOnay(istek.PKod, istek.ApKod, asama), ENUMBirimOnayTur.YATIZLEMEDEGERLENDIRME,
                    istek.Kaldir ? ENUMTasraHazirlikOnayDurum.KurumOnayKaldir : ENUMTasraHazirlikOnayDurum.KurumOnayla);
                if (s == null || !s.islemSonuc)
                    return YatProjeOrtak.Hata(YatProjeOrtak.HataMetni(s?.hataStr, "İşlem yapılamadı."));
                return Results.Json(new
                {
                    basarili = true,
                    bilgi = istek.Kaldir ? "Onay kaldırıldı." : "Onaylandı.",
                    tuslar = TusDurumu(kullanan, istek.Yil, istek.PKod, istek.ApKod, asama)
                });
            }
            catch (Exception)
            {
                return YatProjeOrtak.Hata("İşlem yapılamadı.");
            }
        }

        /// <summary>Eski btnGenelDegerlendirme_Show: pencere her açılışta aşamanın genel değerlendirme metinlerini okur.</summary>
        private static IResult GenelOku(HttpContext baglam, IWebHostEnvironment ortam, int yil, int asama)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!YatProjeOrtak.SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (yil < 2000) return YatProjeOrtak.Hata("Yıl hatalı!");
            int a = AsamaDuzelt(asama);
            try
            {
                ObjectArray<StrKodStrAd> oa = TNS.KYM.Arac.Tanimla().BuyukMetinListele(kullanan, "YAT", AnahtarBulGenel(yil, a, ""), "tr");
                if (oa == null || oa.sonuc == null || !oa.sonuc.islemSonuc)
                    return YatProjeOrtak.Hata(YatProjeOrtak.HataMetni(oa?.sonuc?.hataStr, "Bilgiler okunamadı."));
                // Eski ProjeDosyasiRaporlari.MetinBul: anahtarı tam eşleşen kayıt.
                Dictionary<string, string> metinler = new Dictionary<string, string>();
                foreach (string alan in GenelAlanlar)
                {
                    string anahtar = AnahtarBulGenel(yil, a, alan);
                    string deger = "";
                    foreach (StrKodStrAd item in oa.objeler)
                        if (item.kod == anahtar) { deger = item.ad ?? ""; break; }
                    metinler[IlkKucuk(alan)] = deger;
                }
                return Results.Json(new { metinler = metinler });
            }
            catch (Exception)
            {
                return YatProjeOrtak.Hata("Bilgiler okunamadı.");
            }
        }

        /// <summary>Eski btnGenelDegerlendirmeKaydet_Click.</summary>
        private static async Task<IResult> GenelKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YatIzlemeGenelIstegi istek)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAd);
            if (ret != null) return ret;
            if (!YatProjeOrtak.SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || istek.Yil < 2000) return YatProjeOrtak.Hata("Yıl hatalı!");
            int a = AsamaDuzelt(istek.Asama);
            try
            {
                TNSCollection liste = new TNSCollection();
                liste.Add(new StrKodStrAd(AnahtarBulGenel(istek.Yil, a, "FinansKaynak"), istek.FinansKaynak ?? ""));
                liste.Add(new StrKodStrAd(AnahtarBulGenel(istek.Yil, a, "HizmetKapasitesi"), istek.HizmetKapasitesi ?? ""));
                liste.Add(new StrKodStrAd(AnahtarBulGenel(istek.Yil, a, "KapasiteArtisi"), istek.KapasiteArtisi ?? ""));
                liste.Add(new StrKodStrAd(AnahtarBulGenel(istek.Yil, a, "YatirimUygulamalari"), istek.YatirimUygulamalari ?? ""));
                liste.Add(new StrKodStrAd(AnahtarBulGenel(istek.Yil, a, "Notlar"), istek.Notlar ?? ""));
                liste.Add(new StrKodStrAd(AnahtarBulGenel(istek.Yil, a, "SorunCozum"), istek.SorunCozum ?? ""));
                Sonuc s = TNS.KYM.Arac.Tanimla().BuyukMetinListeSakla(kullanan, "YAT", "tr", liste);
                if (s == null || !s.islemSonuc)
                    return YatProjeOrtak.Hata(YatProjeOrtak.HataMetni(s?.hataStr, "Kayıt yapılamadı."));
                return Results.Json(new { basarili = true, bilgi = "Kayıt tamamlandı" });
            }
            catch (Exception)
            {
                return YatProjeOrtak.Hata("Kayıt yapılamadı.");
            }
        }

        /// <summary>
        /// Raporlar (Rapor.xml 2004-n): 14 Prj. Yazdır, 16 Birim Yazdır (seçili proje / alt proje); 15 Genel Değerlendirme,
        /// 17 Sektörlere Göre (Genel Değerlendirme penceresi; proje 0, yalnızca sistem yöneticisi). Parametreler eskisiyle aynı.
        /// </summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int n, int yil, int? pKod, int? apKod, int asama)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            bool genel = n == 15 || n == 17;
            if (!genel && n != 14 && n != 16) return YatProjeOrtak.Hata("Rapor bulunamadı.");
            if (genel && !YatProjeOrtak.SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (yil < 2000) return YatProjeOrtak.Hata("Yıl hatalı!");
            int p = genel ? 0 : (pKod ?? 0);
            int ap = genel ? 0 : (apKod ?? 0);
            if (!genel && !ProjeVar(yil, p, ap)) return YatProjeOrtak.Hata("Proje ya da alt proje seçilmemiş.");
            try
            {
                Hashtable raporParam = new Hashtable();
                raporParam["ButceYil"] = yil.ToString();
                raporParam["Senaryo"] = Senaryo;
                raporParam["ProjeNo"] = p.ToString();
                raporParam["AltProjeNo"] = ap.ToString();
                raporParam["YIDAsama"] = AsamaDuzelt(asama).ToString();

                AsposeLisans.Yukle();
                string dosya = YatProjeRaporlari.Rapor2004(kullanan, n, raporParam, Ayar.Oku("RaporSablonYol"));
                string ad = n == 14 ? "ProjeIzlemeDegerlendirme" : n == 16 ? "BirimIzlemeDegerlendirme" : n == 15 ? "GenelDegerlendirme" : "GenelDegerlendirmeSektor";
                return DosyaIslem.DosyaGonder(dosya, ad + "_" + yil, true, Path.GetExtension(dosya));
            }
            catch (RaporHatasi ex)
            {
                return YatProjeOrtak.Hata(YatProjeOrtak.HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
            catch (Exception)
            {
                return YatProjeOrtak.Hata("Rapor hazırlanamadı.");
            }
        }
    }
}
