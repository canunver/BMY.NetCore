using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;

namespace Istemci.ButceHazirlik
{
    public class BbhSatiri
    {
        public string tertip { get; set; }
        public double talepEdilen { get; set; }
        public string aciklama { get; set; }
    }

    public class BbhKayitIstegi
    {
        public int Yil { get; set; }
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        /// <summary>true: "Diğer Proje/Faaliyetler" sekmesi.</summary>
        public bool Yeni { get; set; }
        /// <summary>"Diğer Proje/Faaliyetlerden Bütçe Teklifi Yoktur" işaretli mi?</summary>
        public bool YeniYok { get; set; }
        /// <summary>Yalnızca değişen detay (eko 4) satırları.</summary>
        public List<BbhSatiri> Satirlar { get; set; }
    }

    public class BbhOnayIstegi
    {
        public int Yil { get; set; }
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        public bool Yeni { get; set; }
        public bool YeniYok { get; set; }
        /// <summary>1: İle Gönder, 2: İlçeye İade Et, 3: Gönder (merkeze), 4: İle İade Et (ENUMTasraHazirlikOnayDurum).</summary>
        public int Tur { get; set; }
    }

    public class BbhSeciliBirim
    {
        public int MUHASEBEKOD { get; set; }
        public string BIRIMKOD { get; set; }
    }

    public class BbhSeciliOnayIstegi
    {
        public int Yil { get; set; }
        /// <summary>5: Onayla (kurum onayı), 1: Onay Kaldır (il onayı kaldırılır).</summary>
        public int OnayKod { get; set; }
        public List<BbhSeciliBirim> Birimler { get; set; }
    }

    /// <summary>
    /// Birim Bütçe Hazırlığı (eski ButceHazirlikv2/BirimButceHazirlikFormAna.aspx, iki sekme) ve sekmelerde açılan Birim Bütçe Teklifi
    /// (eski BirimButceHazirlikForm.aspx; menüde "Ödenek Planlaması" olarak da tek başına açılır) sayfalarının uçları:
    ///   GET  /api/birimbutcehazirlik/tanim?yeni=
    ///   GET  /api/birimbutcehazirlik/kodlar?yil=&amp;tur=&amp;ust=   (Ekonomik: tur 4; Bakanlığın Tüm Projeleri: tur 14)
    ///   GET  /api/birimbutcehazirlik/faaliyetler?yil=&amp;muhasebe=&amp;birim=   (Birim Projeleri listesi, eski FillddlFaaliyetKod)
    ///   GET  /api/birimbutcehazirlik/durum?yil=&amp;muhasebe=&amp;birim=&amp;yeni=&amp;yeniYok=   (onay durumu, tarihçe, tuşlar)
    ///   GET  /api/birimbutcehazirlik/oku?yil=&amp;muhasebe=&amp;birim=&amp;yeni=&amp;yeniYok=&amp;eko=&amp;faaliyet=   (eski btnGet_Click)
    ///   POST /api/birimbutcehazirlik/kaydet, /onay (İle Gönder, İlçeye İade, Gönder, İle İade)
    ///   GET  /api/birimbutcehazirlik/rapor?grup=&amp;yil=&amp;muhasebe=&amp;birim=   (0 Yazdır, 1 Rapor Yazdır, 2 Özet Yazdır; eski Excel kodu)
    ///   Yalnızca sistem yöneticisi: GET /onaydurum?yil=, POST /secili-onayla (eski Durum penceresi)
    /// Tablo: taşra ödeneği, aşama 00.00. Birim görme / yazma yetkisi iş kurallarında denetlenir. Yetki menüdeki iki sayfadan gelir.
    /// </summary>
    public static class BirimButceHazirlikUclari
    {
        public const string SayfaAdresi = "../ButceHazirlikv2/BirimButceHazirlikForm.aspx";
        public const string AnaSayfaAdresi = "../ButceHazirlikv2/BirimButceHazirlikFormAna.aspx";
        private const string Asama = "00.00";
        private const string ToplamTertip = "T o p l a m";
        public const string YeniYokMetni = "Diğer Proje/Faaliyetlerden Bütçe Teklifi Yoktur";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/birimbutcehazirlik/tanim", Tanim);
            app.MapGet("/api/birimbutcehazirlik/kodlar", Kodlar);
            app.MapGet("/api/birimbutcehazirlik/faaliyetler", Faaliyetler);
            app.MapGet("/api/birimbutcehazirlik/durum", Durum);
            app.MapGet("/api/birimbutcehazirlik/oku", Oku);
            app.MapPost("/api/birimbutcehazirlik/kaydet", Kaydet);
            app.MapPost("/api/birimbutcehazirlik/onay", Onay);
            app.MapGet("/api/birimbutcehazirlik/rapor", Rapor);
            app.MapGet("/api/birimbutcehazirlik/onaydurum", OnayDurum);
            app.MapPost("/api/birimbutcehazirlik/secili-onayla", SeciliOnayla);
        }

        /// <summary>Kullanıcı bu sayfaları kullanabilir mi? Menüdeki iki seçenekten (çerçeve ya da tek sayfa) birini görebilmeli.</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi, AnaSayfaAdresi)
                || SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, AnaSayfaAdresi, SayfaAdresi);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static bool SistemYoneticisi(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        private static IResult Hata(string hata)
        {
            return Results.Json(new { hata = hata });
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static string Kod(string deger)
        {
            return TNS.BHMv2.Arac.TiredenKodBul((deger ?? "").Trim()) ?? "";
        }

        /// <summary>Eski GenelIslemlerIstemci.HazirlikYiliBul: /BUTCEHAZIRLIK değişkeni, yoksa gelecek yıl.</summary>
        private static int HazirlikYili()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBulCache(0, "/BUTCEHAZIRLIK"), DateTime.Now.Year + 1);
        }

        /// <summary>Eski BirimAdBul: saymanlık ve birim adı (UZY); ikisi de bulunursa "saymanlık / birim" adı.</summary>
        internal static string BirimAdBul(int saymanlik, string birim, out string saymanlikAd, out string birimAd)
        {
            TNS.UZY.IUZYServis uzy = TNS.UZY.Arac.Tanimla();
            Kullanici k = new Kullanici();
            saymanlikAd = saymanlik > 0 ? uzy.UzayDegeriStr(k, "MUHMUHASEBE", saymanlik.ToString(), true) ?? "" : "";
            birimAd = saymanlik > 0 && !string.IsNullOrWhiteSpace(birim) ? uzy.UzayDegeriStr(k, "MUHMUHASEBEBIRIM", birim.Trim() + "-" + saymanlik, true) ?? "" : "";
            return TNS.BHMv2.Arac.BirimAdOl(saymanlikAd, birimAd);
        }

        // ------------------------------------------------------------------ eski yardımcılar

        /// <summary>Eski FaaliyetleriGetir: birimin taşra tertiplerindeki faaliyet / projeler (kod 14, 4. düzey) ve ek rakamları.</summary>
        internal static OdenekBilgi[] FaaliyetleriGetir(Kullanici kullanan, int yil, int saymanlik, string birim, bool talepDahil, bool bosEkle)
        {
            Tertip t = new Tertip(yil, ENUMTabloKod.TABTasraOdenek, GenelIslemlerIstemci.VarsayilanKurumBul(), "", "", "", "", "@" + saymanlik.ToString() + "." + birim);
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(14, 4));
            EkDegerParam[] ekBilgiler = new EkDegerParam[talepDahil ? 3 : 2];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABMasrafPlaniBirim, -1, 0, "5");
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABOdemeEmri, -2, 0, "");
            if (talepDahil)
                ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABTasraOdenek, 0, 0, Asama);

            OdenekBilgi[] ob = TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, t, kodBilgileri, ekBilgiler, true, false, true) ?? new OdenekBilgi[0];
            if (bosEkle)
            {
                OdenekBilgi[] ob2 = new OdenekBilgi[ob.Length + 1];
                Array.Copy(ob, 0, ob2, 1, ob.Length);
                ob2[0] = new OdenekBilgi(yil, ENUMTabloKod.TABTasraOdenek);
                return ob2;
            }
            return ob;
        }

        /// <summary>Eski FaaliyetGorunsunMu: önceki yıllarda harcaması / masraf planı olan faaliyetler "Mevcut" sekmesinde, olmayanlar "Diğer" sekmesinde.</summary>
        private static bool FaaliyetGorunsunMu(string simdFaal, OdenekBilgi[] faaliyetler, bool yeni)
        {
            for (int i = 0; i < faaliyetler.Length; i++)
            {
                if (faaliyetler[i].kod4 == simdFaal)
                {
                    if (faaliyetler[i].EkRakam(0) > 0 || faaliyetler[i].EkRakam(1) > 0)
                        return !yeni;
                    return yeni;
                }
            }
            return yeni;
        }

        /// <summary>Eski EskiFaaliyetYeniFaaliyetBul: eski faaliyetlerden ve yeni faaliyetlerden teklif var mı?</summary>
        private static void EskiFaaliyetYeniFaaliyetBul(OdenekBilgi[] faaliyetler, out bool eskiVar, out bool yeniVar)
        {
            eskiVar = false;
            yeniVar = false;
            int eskiSay = 0;
            for (int i = 0; i < faaliyetler.Length; i++)
            {
                bool eski = faaliyetler[i].EkRakam(0) > 0 || faaliyetler[i].EkRakam(1) > 0;
                if (eski) eskiSay++;
                if (faaliyetler[i].EkRakam(2) > 0)
                {
                    if (eski) eskiVar = true;
                    else yeniVar = true;
                }
            }
            if (eskiSay == 0) eskiVar = true;
        }

        private static bool IlceyiYonetiyorMu(Kullanici kullanan, int saymanlik)
        {
            return SistemYoneticisi(kullanan) || TNS.BHMv2.Arac.Tanimla().IlKullanicisiMi(kullanan, saymanlik / 1000);
        }

        /// <summary>
        /// Onay durumu, tarihçe ve tuşlar (eski OnayKodListeleTasra + BirimTuslariAyarla). Tuş adları eski sayfadaki gibi:
        /// kaydet, yazdir (Yazdır / Rapor Yazdır), ileGonder, merkezeGonder (+ Pasif: görünür ama basılamaz), ileIade, ilceyeIade,
        /// durum (sistem yöneticisi), yeniYok ("teklif yoktur" kutusu görünür mü).
        /// </summary>
        private static object DurumBilgisi(Kullanici kullanan, int yil, int saymanlik, string birim, bool yeni, bool yeniYok)
        {
            birim = (birim ?? "").Trim();
            bool yonetici = SistemYoneticisi(kullanan);
            int onayDurum = -1;
            List<object> tarihce = new List<object>();
            if (saymanlik > 0 && birim != "")
            {
                ObjectArray oa = TNS.BHMv2.Arac.Tanimla().OnayKodListeleTasra(kullanan, yil, saymanlik, birim, Asama, ENUMBirimOnayTur.BIRIMBUTCEHAZIRLIK);
                if (oa != null)
                {
                    onayDurum = OrtakFonksiyonlar.ConvertToInt(oa.sonuc.anahtar, -1);
                    for (int i = 0; i < oa.ObjeSayisi; i++)
                    {
                        IslemTarihce it = (IslemTarihce)oa.objeler[i];
                        tarihce.Add(new { it.aciklama, it.ISLEMIYAPAN, it.ISLEMTARIHSTR });
                    }
                }
            }
            int durum = onayDurum == -1 ? 0 : onayDurum;
            string durumAd = saymanlik > 0 && birim != "" ? (onayDurum == 5 ? "Onaylı" : "Onaysız") : "";

            BirimAdBul(saymanlik, birim, out _, out string birimAd);
            ENUMBirimTur tur = string.IsNullOrWhiteSpace(birimAd) ? ENUMBirimTur.Bilinmiyor : TNS.BHMv2.Arac.BirimTurBul(saymanlik, birim);
            bool yazdir = tur != ENUMBirimTur.Bilinmiyor;
            if (!yazdir) onayDurum = -1;

            bool kaydet = false, ileGonder = false, merkezeGonder = false, ileIade = false, ilceyeIade = false, yeniYokGoster = false;
            bool ileGonderPasif = false, merkezePasif = false;
            if (onayDurum == 5)
            {
                if (tur == ENUMBirimTur.Tasra_Il || tur == ENUMBirimTur.Tasra_MerkezeBagli || tur == ENUMBirimTur.Merkez)
                    ileIade = yonetici;
                if (tur == ENUMBirimTur.Tasra_Ilce)
                    ilceyeIade = IlceyiYonetiyorMu(kullanan, saymanlik);
            }
            else if (onayDurum != -1)
            {
                kaydet = true;
                ileGonder = tur == ENUMBirimTur.Tasra_Ilce;
                merkezeGonder = tur == ENUMBirimTur.Merkez || tur == ENUMBirimTur.Tasra_Il || tur == ENUMBirimTur.Tasra_MerkezeBagli;
                if (!yonetici)
                {
                    EskiFaaliyetYeniFaaliyetBul(FaaliyetleriGetir(kullanan, yil, saymanlik, birim, true, false), out bool eskiVar, out bool yeniVar);
                    bool acik = eskiVar && (yeniVar || yeniYok);
                    ileGonderPasif = !acik;
                    merkezePasif = !acik;
                    yeniYokGoster = yeni && !yeniVar && (ileGonder || merkezeGonder);
                    ileGonder = true; // eski sayfada yönetici olmayan kullanıcıya İle Gönder her durumda görünürdü
                }
            }

            return new
            {
                durum = durum, durumAd = durumAd, tarihce = tarihce,
                tuslar = new
                {
                    kaydet = kaydet, yazdir = yazdir, ileGonder = ileGonder, ileGonderPasif = ileGonderPasif,
                    merkezeGonder = merkezeGonder, merkezePasif = merkezePasif, ileIade = ileIade, ilceyeIade = ilceyeIade,
                    durum = yonetici, yeniYok = yeniYokGoster
                }
            };
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Açılış (eski Page_Load): hazırlık yılı, kullanıcının saymanlık / birimi (kullanıcı değişkenleri) ve sekme açıklaması.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam, int? yeni)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int yil = HazirlikYili();
                string muhasebe = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBE");
                string birim = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEBIRIM");
                string birimAd = MuhasebeBirimUclari.BirimAdBul(OrtakFonksiyonlar.ConvertToInt(muhasebe, 0), birim, out string kurum);
                string bilgi = yeni != 0
                    ? "Bu ekranda biriminizde uygulanmayan ancak uygulanmasını teklif edeceğiniz faaliyet/projeler için giriş yapılacaktır."
                    : "Bu ekranda önceki yıllarda yürütülen ve devam edecek faaliyet/projeler için bütçe teklifi girişi yapılacaktır. Burada yer almayan teklifleriniz var ise 'Diğer Proje/Faaliyetler' ekranından yapınız.";
                return Results.Json(new
                {
                    yil = yil, muhasebe = muhasebe, birim = birim, birimAd = birimAd, kurumsal = MuhasebeBirimUclari.KurumsalYaz(yil, kurum),
                    bilgi = bilgi, yeniYokMetni = YeniYokMetni, sistemYoneticisi = SistemYoneticisi(kullanan)
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>Ekonomik (4) ve Bakanlığın Tüm Projeleri (14) kod ağaçlarının bir düzeyi (eski nodeLoadPar / nodeLoadPar3).</summary>
        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string tur, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            tur = (tur ?? "").Trim();
            if (tur != "4" && tur != "14") return Hata("Kod türü hatalı!");
            return ButceRakamUclari.KodDuzeyi(kullanan, yil, tur, ust);
        }

        /// <summary>Birim Projeleri listesi (eski FillddlFaaliyetKod): ilk satır boştur (bütün faaliyetler).</summary>
        private static IResult Faaliyetler(HttpContext baglam, IWebHostEnvironment ortam, int yil, int? muhasebe, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            List<object> liste = new List<object>();
            birim = (birim ?? "").Trim();
            if ((muhasebe ?? 0) <= 0 || birim == "") return Results.Json(new { satirlar = liste });
            try
            {
                foreach (OdenekBilgi ob in FaaliyetleriGetir(kullanan, yil, muhasebe.Value, birim, false, true))
                    liste.Add(new { kod4 = ob.kod4 ?? "", kodad4 = ob.kodad4 ?? "" });
                return Results.Json(new { satirlar = liste });
            }
            catch (Exception)
            {
                return Hata("Birim projeleri okunamadı.");
            }
        }

        private static IResult Durum(HttpContext baglam, IWebHostEnvironment ortam, int yil, int? muhasebe, string birim, int? yeni, bool? yeniYok)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                return Results.Json(DurumBilgisi(kullanan, yil, muhasebe ?? 0, birim, yeni != 0, yeniYok == true));
            }
            catch (Exception)
            {
                return Hata("Onay durumu okunamadı.");
            }
        }

        /// <summary>
        /// Listele (eski Get): birimin taşra tertipleri, eko 2 toplamları (ana) ve eko 4 detayları; sekmeye göre önceki yıllarda yürütülen
        /// ya da yürütülmeyen faaliyetler. İlk satır genel toplamdır. Saymanlık ve birim kullanıcı değişkeni olarak saklanır.
        /// </summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int yil, int? muhasebe, string birim, int? yeni, bool? yeniYok, string eko, string faaliyet)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int saymanlik = muhasebe ?? 0;
            birim = (birim ?? "").Trim();
            bool yeniMi = yeni != 0;
            if (yil < 2000) return Hata("Yıl hatalı!");
            if (saymanlik <= 0 || birim == "") return Hata("Lütfen saymanlık ve birim kodunu giriniz!");
            try
            {
                BirimAdBul(saymanlik, birim, out string saymanlikAd, out string birimAd);
                if (string.IsNullOrWhiteSpace(saymanlikAd)) return Hata("Girilen saymanlık bulunamadı!");
                if (string.IsNullOrWhiteSpace(birimAd)) return Hata("Girilen birim bulunamadı!");
                string kurKod = TNS.MUH.Arac.Tanimla().SaymanlikBirimKurkodBul(saymanlik, birim);
                if (string.IsNullOrWhiteSpace(kurKod)) return Hata("Girilen birimin kurumsal kodu bulunamadı!");

                Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kurKod, "", "", Kod(eko), Kod(faaliyet));
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                ObjectArray ob = servis.ListeleBirimButceHazirlik(kullanan, t, saymanlik, birim, Asama, false, false);
                if (ob == null || !ob.sonuc.islemSonuc) return Hata("Listeleme işlemi yapılamadı:" + HataMetni(ob?.sonuc.hataStr, ""));
                int ilkSayi = ob.ObjeSayisi;

                OdenekBilgi[] faaliyetler = FaaliyetleriGetir(kullanan, yil, saymanlik, birim, true, false);
                for (int i = 0; i < ob.ObjeSayisi;)
                {
                    string simdFaal = ((BirimButceHazirlikKarsilanan)ob[i]).FaaliyetKodBul();
                    bool goster = FaaliyetGorunsunMu(simdFaal, faaliyetler, yeniMi);
                    // Aynı faaliyetin ardışık satırları birlikte atılır ya da tutulur.
                    while (i < ob.ObjeSayisi && ((BirimButceHazirlikKarsilanan)ob[i]).FaaliyetKodBul() == simdFaal)
                    {
                        if (goster) i++;
                        else ob.objeler.RemoveAt(i);
                    }
                }

                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBE", saymanlik.ToString());
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBEBIRIM", birim);

                List<object> satirlar = new List<object>();
                double harcama = 0, planlanan = 0, talep = 0;
                for (int i = 0; i < ob.ObjeSayisi; i++)
                {
                    BirimButceHazirlikKarsilanan g = (BirimButceHazirlikKarsilanan)ob[i];
                    harcama += g.harcama;
                    planlanan += g.planlanan;
                    talep += g.talepEdilen;
                    satirlar.Add(new
                    {
                        detayMi = g.detayMi, tertip = g.tertip ?? "", pebad = g.pebad ?? "", ekoad = g.ekoad ?? "",
                        harcama = g.harcama, planlanan = g.planlanan, talepEdilen = g.talepEdilen, aciklama = g.aciklama ?? ""
                    });
                }
                // Eski gobAna: harcama ve talep hem ana hem detay satırlarında olduğundan yarıya bölünür (masraf planı yalnızca ana satırda).
                object toplam = new { detayMi = false, tertip = ToplamTertip, pebad = "", ekoad = "", harcama = harcama / 2, planlanan = planlanan, talepEdilen = talep / 2, aciklama = "" };

                string uyari = null;
                if (yeniMi && satirlar.Count == 0)
                {
                    uyari = ilkSayi == 0
                        ? "Girmiş olduğunuz kriterlere uyan tertip bulunanamıştır. Yeni tertip açtırabilir veya kriterleri değiştirebilirsiniz!"
                        : "Seçmiş olduğunuz faaliyet/projeyi daha önce kullanmış olduğunuzdan, bilgileri yeni faaliyet/proje sayfasında gösterilmemiştir. Lütfen Mevcut Faaliyet/Proje sayfasını kullanarak giriş yapınız!";
                }
                return Results.Json(new { toplam = toplam, satirlar = satirlar, uyari = uyari, durumBilgi = DurumBilgisi(kullanan, yil, saymanlik, birim, yeniMi, yeniYok == true) });
            }
            catch (Exception)
            {
                return Hata("Listeleme işlemi yapılamadı.");
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>Kaydet (eski Save): değişen detay satırlarının talebi ve gerekçesi (tertip adı alanında) aşama 00.00'a yazılır.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, BbhKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });
            if (istek.Satirlar == null || istek.Satirlar.Count == 0)
                return Results.Json(new { basarili = false, hata = "Bütçe teklifi girişi yapmadan kaydedemezsiniz!" });
            if (istek.Yeni && istek.YeniYok)
                return Results.Json(new { basarili = false, hata = "Diğer faaliyet/projelerden teklifim yoktur işaretli olmasına rağmen, kaydetmeye çalışıyorsunuz!" });

            try
            {
                List<TertipOdenek> l = new List<TertipOdenek>();
                foreach (BbhSatiri s in istek.Satirlar)
                {
                    if (s == null || string.IsNullOrWhiteSpace(s.tertip) || s.tertip == ToplamTertip) continue;
                    TertipOdenek odenek = new TertipOdenek(1);
                    odenek.tertip = new Tertip(istek.Yil, ENUMTabloKod.TABTasraOdenek);
                    odenek.tertip.TertipAta(s.tertip.Trim());
                    odenek.odenek[0] = s.talepEdilen;
                    odenek.tertip.tertipAdi = s.aciklama ?? "";
                    l.Add(odenek);
                }
                if (l.Count == 0) return Results.Json(new { basarili = false, hata = "Bütçe teklifi girişi yapmadan kaydedemezsiniz!" });

                string birim = (istek.Birim ?? "").Trim();
                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().SaklaBirimButceHazirlik(kullanan, istek.Yil, istek.Muhasebe, birim, Asama, l.ToArray());
                if (sonuc == null || !sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc?.hataStr, "Kayıt işlemi başarısız.") });
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBE", istek.Muhasebe.ToString());
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBEBIRIM", birim);
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarılı.", durumBilgi = DurumBilgisi(kullanan, istek.Yil, istek.Muhasebe, birim, istek.Yeni, istek.YeniYok) });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
        }

        /// <summary>
        /// İle Gönder (1), İlçeye İade Et (2), Gönder (3), İle İade Et (4) (eski btnOnay_Click). Gönderirken yönetici olmayan kullanıcı
        /// eski faaliyetlerden en az birine ve diğer faaliyetlere teklif vermiş (ya da "teklif yoktur" işaretlemiş) olmalıdır.
        /// Kaydedilmemiş değişiklik denetimi sayfada yapılır.
        /// </summary>
        private static async Task<IResult> Onay(HttpContext baglam, IWebHostEnvironment ortam, BbhOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000 || istek.Tur < 1 || istek.Tur > 4) return Results.Json(new { basarili = false, hata = "İşlem yapılamadı." });

            ENUMTasraHazirlikOnayDurum onayDurum = (ENUMTasraHazirlikOnayDurum)istek.Tur;
            string birim = (istek.Birim ?? "").Trim();
            bool gonder = onayDurum == ENUMTasraHazirlikOnayDurum.IlceOnayla || onayDurum == ENUMTasraHazirlikOnayDurum.IlOnayla;
            try
            {
                if (gonder && !SistemYoneticisi(kullanan))
                {
                    EskiFaaliyetYeniFaaliyetBul(FaaliyetleriGetir(kullanan, istek.Yil, istek.Muhasebe, birim, true, false), out bool eskiVar, out bool yeniVar);
                    if (!eskiVar)
                        return Results.Json(new { basarili = false, hata = "Geçen senelerde yürütmüş olduğunuz faaliyet/projelerden en az birinden bütçe talebi yapmalısınız!" });
                    if (!yeniVar && !istek.YeniYok)
                        return Results.Json(new
                        {
                            basarili = false,
                            hata = "Yaptığınız teklifte diğer proje/faaliyet sayfasından bilgi bulunmamaktadır. Eğer yapacağınız teklifte diğer proje/faaliyet bulunmayacaksa, ilgili sayfayı açıp, listeleme işlemi yapınız. Sonra '" + YeniYokMetni + "' seçimini işaretleyerek gönderme işlemini yapınız!"
                        });
                }

                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().OnayKodAtaTasra(kullanan, istek.Yil, istek.Muhasebe, birim, Asama, ENUMBirimOnayTur.BIRIMBUTCEHAZIRLIK, onayDurum);
                if (sonuc == null || !sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc?.hataStr, "İşlem yapılamadı.") });
                return Results.Json(new
                {
                    basarili = true, bilgi = (gonder ? "Onaylama" : "Onay kaldırma") + " işlemi başarılı.",
                    durumBilgi = DurumBilgisi(kullanan, istek.Yil, istek.Muhasebe, birim, istek.Yeni, istek.YeniYok)
                });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "İşlem yapılamadı." });
            }
        }

        // ------------------------------------------------------------------ raporlar (eski kodla)

        /// <summary>Yazdır (grup 0), Rapor Yazdır (1), Özet Yazdır (2) (eski btnYazdir_Click; şablonlar BHMv2\BirimButceHazirlik(Grup).xltx).</summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int grup, int yil, int? muhasebe, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                AsposeLisans.Yukle();
                BirimButceHazirlikRaporu r = new BirimButceHazirlikRaporu(kullanan, ortam.ContentRootPath, yil, muhasebe ?? 0, (birim ?? "").Trim());
                string dosya;
                if (grup == 1) dosya = r.YazdirGrup();
                else if (grup == 2) dosya = r.YazdirOzet();
                else dosya = r.YazdirTek();
                if (dosya == null) return Hata(r.hata ?? "Rapor hazırlanamadı.");
                return DosyaIslem.DosyaGonder(dosya, Path.GetFileName(dosya), true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }

        // ------------------------------------------------------------------ Durum penceresi (sistem yöneticisi)

        /// <summary>Bütün birimlerin girdiği toplam ve onay durumu (eski DurumYukle).</summary>
        private static IResult OnayDurum(HttpContext baglam, IWebHostEnvironment ortam, int yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            try
            {
                ObjectArray oa = TNS.BHMv2.Arac.Tanimla().BirimButceHazirlikOnayDurum(kullanan, yil, "0000", 0, null);
                List<object> liste = new List<object>();
                if (oa != null)
                    for (int i = 0; i < oa.ObjeSayisi; i++)
                    {
                        IslemTarihce it = (IslemTarihce)oa.objeler[i];
                        liste.Add(new { it.MUHASEBEKOD, it.BIRIMKOD, it.MUHASEBEBIRIMAD, it.GIRILEN, it.DURUMAD });
                    }
                return Results.Json(new { satirlar = liste });
            }
            catch (Exception)
            {
                return Hata("Onay durumları okunamadı.");
            }
        }

        /// <summary>Seçili birimleri onaylar (kurum onayı) ya da il onayını kaldırır (eski btnSeciliOnayla_Click).</summary>
        private static async Task<IResult> SeciliOnayla(HttpContext baglam, IWebHostEnvironment ortam, BbhSeciliOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || istek.Birimler == null || istek.Birimler.Count == 0)
                return Results.Json(new { basarili = false, hata = "Seçim yapmanız gerekmektedir!" });

            ENUMTasraHazirlikOnayDurum onayDurum = istek.OnayKod == 1 ? ENUMTasraHazirlikOnayDurum.IlOnayKaldir : ENUMTasraHazirlikOnayDurum.KurumOnayla;
            IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
            int hatali = 0, basarili = 0;
            string hatalar = "";
            foreach (BbhSeciliBirim item in istek.Birimler)
            {
                if (item == null) continue;
                try
                {
                    Sonuc sonuc = servis.OnayKodAtaTasra(kullanan, istek.Yil, item.MUHASEBEKOD, item.BIRIMKOD, Asama, ENUMBirimOnayTur.BIRIMBUTCEHAZIRLIK, onayDurum);
                    if (sonuc == null || !sonuc.islemSonuc)
                    {
                        hatalar += string.Format(" {0}-{1} birimi onaylanırken hata oluştu: {2}", item.MUHASEBEKOD, item.BIRIMKOD, HataMetni(sonuc?.hataStr, "işlem yapılamadı"));
                        hatali++;
                    }
                    else basarili++;
                }
                catch (Exception)
                {
                    hatalar += string.Format(" {0}-{1} birimi onaylanırken hata oluştu.", item.MUHASEBEKOD, item.BIRIMKOD);
                    hatali++;
                }
            }
            if (hatali == 0 && basarili == 0) return Results.Json(new { basarili = false, hata = "Seçim yapmanız gerekmektedir!" });
            return Results.Json(new { basarili = hatali == 0, bilgi = $"{basarili} adet birim onaylandı ve {hatali} adet birim onaylanamadı!" + hatalar });
        }
    }
}
