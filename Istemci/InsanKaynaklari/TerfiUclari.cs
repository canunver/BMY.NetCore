using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Terfi Onayına Gönder isteği: seçili satırların kadro kodları (eski grdListe seçili satırları).</summary>
    public class TerfiGonderIstegi
    {
        public int yil { get; set; }
        public int ay { get; set; }
        public int tip { get; set; }
        public int sebepKod { get; set; }
        public List<int> pKadroKodlar { get; set; }
    }

    /// <summary>Terfiyi Onayla / Reddet isteği: seçili terfi kayıtlarının kodları.</summary>
    public class TerfiOnayIstegi
    {
        public bool onay { get; set; }
        public List<int> kodlar { get; set; }
    }

    /// <summary>
    /// Personel terfi ekranlarının uçları:
    ///   Terfiye Hak Kazananlar (eski InsanKaynaklari/TerfiyeHakKazananlar.aspx; yeni sayfa InsanKaynaklari/TerfiyeHakKazananlar)
    ///     GET  /api/terfi/secenekler                         yıl/ay, tip, durum ve hizmet sebebi (TERFI) seçenekleri
    ///     GET  /api/terfi/hakkazanan/liste?yil=&amp;ay=&amp;tip=      (eski TerfiListele)
    ///     POST /api/terfi/hakkazanan/gonder                  (eski TerfiOnayinaGonder)
    ///     GET  /api/terfi/hakkazanan/yazdir?yil=&amp;ay=&amp;tip=    (eski TerfiListesiYazdir, IKY\TerfiEdeceklerListesi.xltx)
    ///   Personel Terfi Onaylama (eski InsanKaynaklari/PersonelTerfiOnaylama.aspx; yeni sayfa InsanKaynaklari/PersonelTerfiOnaylama)
    ///     GET  /api/terfi/onay/liste?durum=&amp;tip=              (eski TerfiListele)
    ///     POST /api/terfi/onay/islem                         (eski TerfiOnayla: Terfiyi Onayla / Terfiyi Reddet)
    ///     GET  /api/terfi/onay/yazdir?durum=&amp;tip=             (eski TerfiListesiYazdir, IKY\TerfiListesi.xltx)
    /// İş kuralları eski sayfalardaki gibi IIKYServis.TerfisiGelenler / PersonelTerfiKaydet / PersonelTerfiListele / EKEKHATerfiEt.
    /// Seçili satırların bilgileri istemciden alınmaz: kodlar sunucudaki listeyle eşlenir, kişi / kadro / tip / sebep oradan okunur.
    /// </summary>
    public static class TerfiUclari
    {
        public const string HakKazananSayfa = "../InsanKaynaklari/TerfiyeHakKazananlar.aspx";
        public const string OnaySayfa = "../InsanKaynaklari/PersonelTerfiOnaylama.aspx";

        private static readonly CultureInfo TR = new CultureInfo("tr-TR");

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/terfi/secenekler", Secenekler);
            app.MapGet("/api/terfi/hakkazanan/liste", HakKazananListe);
            app.MapPost("/api/terfi/hakkazanan/gonder", HakKazananGonder);
            app.MapGet("/api/terfi/hakkazanan/yazdir", HakKazananYazdir);
            app.MapGet("/api/terfi/onay/liste", OnayListe);
            app.MapPost("/api/terfi/onay/islem", OnayIslem);
            app.MapGet("/api/terfi/onay/yazdir", OnayYazdir);
        }

        /// <summary>Eski Page_Load: GenelIslemler.SayfayaGirisYapabilirMi (menü yetkisi).</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru, string sayfa)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, sayfa);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan, params string[] sayfalar)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            foreach (string s in sayfalar)
                if (YetkiVar(kullanan, ortam.ContentRootPath, s)) return null;
            return Results.StatusCode(StatusCodes.Status403Forbidden);
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        // ---------------------------------------------------------------- seçenekler (eski Genel.AyDoldur, TerfiTipiDoldur, TerfiDurumuDoldur, HizmetSebebiDoldur)

        private static IResult Secenekler(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan, HakKazananSayfa, OnaySayfa);
            if (ret != null) return ret;

            List<object> aylar = new List<object>();
            for (int i = 1; i < 13; i++)
                aylar.Add(new { kod = i, ad = TR.DateTimeFormat.GetMonthName(i) });

            List<object> tipler = new List<object>
            {
                new { kod = (int)ENUMTerfiTipi.KHA, ad = "Sadece K.H.A." },
                new { kod = (int)ENUMTerfiTipi.EKEA, ad = "Sadece E.K.E.A." },
                new { kod = (int)ENUMTerfiTipi.KHAEKEA, ad = "K.H.A. ve E.K.E.A." }
            };
            List<object> durumlar = new List<object>
            {
                new { kod = (int)ENUMTerfiDurumu.Beklemede, ad = "Beklemede" },
                new { kod = (int)ENUMTerfiDurumu.Onay, ad = "Onaylandı" },
                new { kod = (int)ENUMTerfiDurumu.Red, ad = "Reddedildi" }
            };

            List<object> sebepler = new List<object>();
            ObjectArray oa = TNS.IKY.Arac.Tanimla().HizmetSebebiListele(ENUMHizmetSebebiNevi.TERFI);
            if (oa.sonuc.islemSonuc)
                foreach (HizmetSebebi h in oa.objeler)
                    sebepler.Add(new { kod = h.kod, ad = h.sebepAdi });

            return Results.Json(new { basarili = true, yil = DateTime.Now.Year, ay = DateTime.Now.Month, aylar, tipler, durumlar, sebepler });
        }

        // ---------------------------------------------------------------- Terfiye Hak Kazananlar

        /// <summary>Eski Listele'deki yıl / ay denetimi; yilAy eski sayfadaki gibi yyyyMM15.</summary>
        private static string YilAyDenetle(int yil, int ay, int tip, out string yilAy)
        {
            yilAy = "";
            string hata = "";
            if (yil <= 0) hata = "Yıl bilgisini seçmelisiniz.";
            if (ay <= 0 || ay > 12) hata += " Ay bilgisini seçmelisiniz.";
            // İş kuralı (IKYPersonelTerfi.TerfisiGelenler) tip 1-3 dışında boş liste döndürür; eski sayfada boş tip boş liste veriyordu.
            if (tip < 1 || tip > 3) hata += " Tip bilgisini seçmelisiniz.";
            if (hata != "") return hata.Trim();
            yilAy = yil.ToString("0000") + ay.ToString("00") + "15";
            return null;
        }

        private static IResult HakKazananListe(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? ay, int? tip)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan, HakKazananSayfa);
            if (ret != null) return ret;

            string hata = YilAyDenetle(yil ?? 0, ay ?? 0, tip ?? 0, out string yilAy);
            if (hata != null) return Hata(hata);

            ObjectArray oa = TNS.IKY.Arac.Tanimla().TerfisiGelenler(kullanan, yilAy, tip.Value);
            if (!oa.sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(oa.sonuc.hataStr, "Terfiye hak kazananlar okunamadı."));

            List<object> satirlar = new List<object>();
            foreach (Terfi p in oa.objeler)
                satirlar.Add(Satir(p, false));
            return Results.Json(new { basarili = true, satirlar });
        }

        private static async Task<IResult> HakKazananGonder(HttpContext baglam, IWebHostEnvironment ortam, TerfiGonderIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan, HakKazananSayfa);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            if (istek.pKadroKodlar == null || istek.pKadroKodlar.Count == 0)
                return Hata("Listeden herhangi bir başvuru seçilmediği için işlem yapılamadı.");
            if (istek.sebepKod <= 0)
                return Hata("Atama gerekçesi (hizmet sebebi) seçiniz!");

            string hata = YilAyDenetle(istek.yil, istek.ay, istek.tip, out string yilAy);
            if (hata != null) return Hata(hata);

            IIKYServis servis = TNS.IKY.Arac.Tanimla();
            ObjectArray oa = servis.TerfisiGelenler(kullanan, yilAy, istek.tip);
            if (!oa.sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(oa.sonuc.hataStr, "Terfiye hak kazananlar okunamadı."));

            Dictionary<int, Terfi> liste = new Dictionary<int, Terfi>();
            foreach (Terfi t in oa.objeler)
                liste[t.pKadroKod] = t;

            List<string> hatalar = new List<string>();
            int basarili = 0;
            foreach (int pKadroKod in istek.pKadroKodlar)
            {
                if (!liste.TryGetValue(pKadroKod, out Terfi s))
                {
                    hatalar.Add("Kadro " + pKadroKod + ": seçilen dönemin terfi listesinde bulunamadı.");
                    continue;
                }
                Terfi terfi = new Terfi();
                terfi.pKod = s.pKod;
                terfi.pKadroKod = s.pKadroKod;
                terfi.durum = (int)ENUMTerfiDurumu.Beklemede;
                terfi.hizmetSebebi = istek.sebepKod;
                terfi.tip = s.tip;
                terfi.yil = istek.yil;
                terfi.ay = istek.ay;
                Sonuc sonuc = servis.PersonelTerfiKaydet(kullanan, terfi);
                if (sonuc.islemSonuc) basarili++;
                else hatalar.Add(s.ad + ": " + PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Terfi onayına gönderilemedi."));
            }

            if (hatalar.Count == 0)
                return Results.Json(new { basarili = true, bilgi = "Seçili hizmetler terfiye başarıyla gönderilmiştir." });
            return Results.Json(new
            {
                basarili = basarili > 0,
                hata = string.Join("\n", hatalar) + (basarili > 0 ? "\nDiğer seçili hizmetler (" + basarili + ") terfiye başarıyla gönderilmiştir." : "")
            });
        }

        private static IResult HakKazananYazdir(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? ay, int? tip)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan, HakKazananSayfa);
            if (ret != null) return ret;

            string hata = YilAyDenetle(yil ?? 0, ay ?? 0, tip ?? 0, out string yilAy);
            if (hata != null) return Hata(hata);

            string sablonAd = PersonelOrtakRaporlar.SablonBul(ortam, "IKY", "TerfiEdeceklerListesi.xltx");
            if (sablonAd == null) return Hata("Rapor şablonu bulunamadı (IKY\\TerfiEdeceklerListesi.xltx).");

            int t = tip.Value;
            // Eski TerfiListesiYazdir yyyyMM ile çağırıyordu; iş kuralı yalnızca ay kısmını (4,2) kullanır.
            ObjectArray oa = TNS.IKY.Arac.Tanimla().TerfisiGelenler(kullanan, yilAy.Substring(0, 6), t);
            if (!oa.sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(oa.sonuc.hataStr, "Terfiye hak kazananlar okunamadı."));

            try
            {
                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                string sonucDosyaAd = DosyaIslem.DosyaAdUret("", ".xlsx");
                XLS.DosyaAc(sablonAd, sonucDosyaAd);

                int kaynakSatir = 0, sutun = 0;
                XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
                XLS.HucreAdBulYaz("YilAy", yil.Value + " YILI " + ay.Value + ". AY " + TipAd(t) + " ' NA GÖRE TERFİ EDECEKLER LİSTESİ");
                int satir = kaynakSatir;

                int sayac = 1;
                foreach (Terfi terfi in oa.objeler)
                {
                    satir++;
                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 9, satir, sutun);
                    XLS.HucreDegerYaz(satir, 0, sayac);
                    XLS.HucreDegerYaz(satir, 1, terfi.ad);
                    XLS.HucreDegerYaz(satir, 2, terfi.unvan);
                    XLS.HucreDegerYaz(satir, 3, terfi.ogrenim);
                    XLS.HucreDegerYaz(satir, 4, terfi.kadroDerece);
                    XLS.HucreDegerYaz(satir, 5, terfi.kazanilmisHakDerece);
                    XLS.HucreDegerYaz(satir, 6, terfi.kazanilmisHakKademe);
                    XLS.HucreDegerYaz(satir, 7, terfi.emekliyeEsasDerece);
                    XLS.HucreDegerYaz(satir, 8, terfi.emekliyeEsasKademe);
                    XLS.HucreDegerYaz(satir, 9, terfi.yukselecegiDerece);
                    sayac++;

                    if (t == (int)ENUMTerfiTipi.KHA)
                    {
                        XLS.SutunGizle(7, 8, true);
                        if (terfi.kadroDerece == terfi.kazanilmisHakDerece)
                        {
                            XLS.KoyuYap(satir, 5, satir, 6, true);
                            XLS.YaziRenk(satir, 5, satir, 6, TabloRenk.ORANGE);
                        }
                    }
                    else if (t == (int)ENUMTerfiTipi.EKEA)
                    {
                        XLS.SutunGizle(5, 6, true);
                        if (terfi.kadroDerece == terfi.emekliyeEsasDerece)
                        {
                            XLS.KoyuYap(satir, 7, satir, 8, true);
                            XLS.YaziRenk(satir, 7, satir, 8, TabloRenk.ORANGE);
                        }
                    }
                    else
                    {
                        if (terfi.kadroDerece == terfi.kazanilmisHakDerece || terfi.kadroDerece == terfi.emekliyeEsasDerece)
                        {
                            XLS.KoyuYap(satir, 5, satir, 8, true);
                            XLS.YaziRenk(satir, 5, satir, 8, TabloRenk.ORANGE);
                        }
                    }
                }

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "TerfiEdeceklerListesi_" + DateTime.Now.Ticks.ToString(), true, "xlsx");
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        // ---------------------------------------------------------------- Personel Terfi Onaylama

        private static IResult OnayListe(HttpContext baglam, IWebHostEnvironment ortam, int? durum, int? tip)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan, OnaySayfa);
            if (ret != null) return ret;

            Terfi terfi = new Terfi();
            terfi.durum = durum ?? 0;
            terfi.tip = tip ?? 0;
            ObjectArray oa = TNS.IKY.Arac.Tanimla().PersonelTerfiListele(kullanan, terfi);
            if (!oa.sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(oa.sonuc.hataStr, "Terfi kayıtları okunamadı."));

            List<object> satirlar = new List<object>();
            foreach (Terfi p in oa.objeler)
                satirlar.Add(Satir(p, true));
            return Results.Json(new { basarili = true, satirlar });
        }

        private static async Task<IResult> OnayIslem(HttpContext baglam, IWebHostEnvironment ortam, TerfiOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan, OnaySayfa);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            if (istek.kodlar == null || istek.kodlar.Count == 0)
                return Hata("Listeden herhangi bir başvuru seçilmediği için işlem yapılamadı.");

            IIKYServis servis = TNS.IKY.Arac.Tanimla();
            ObjectArray oa = servis.PersonelTerfiListele(kullanan, new Terfi());
            if (!oa.sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(oa.sonuc.hataStr, "Terfi kayıtları okunamadı."));

            Dictionary<int, Terfi> liste = new Dictionary<int, Terfi>();
            foreach (Terfi t in oa.objeler)
                liste[t.kod] = t;

            int durum = istek.onay ? (int)ENUMTerfiDurumu.Onay : (int)ENUMTerfiDurumu.Red;
            List<string> hatalar = new List<string>();
            int basarili = 0;
            foreach (int kod in istek.kodlar)
            {
                if (!liste.TryGetValue(kod, out Terfi s))
                {
                    hatalar.Add("Terfi kaydı " + kod + " bulunamadı.");
                    continue;
                }
                if (s.durum != (int)ENUMTerfiDurumu.Beklemede)
                {
                    hatalar.Add(s.ad + ": Onay/red işlemi yapabilmeniz için seçtiğiniz hizmetin durumu 'beklemede' olmalıdır.");
                    continue;
                }
                Terfi terfi = new Terfi();
                terfi.kod = s.kod;
                terfi.pKod = s.pKod;
                terfi.pKadroKod = s.pKadroKod;
                terfi.durum = durum;
                terfi.tip = s.tip;
                terfi.hizmetSebebi = s.hizmetSebebi;

                Sonuc sonuc = servis.EKEKHATerfiEt(kullanan, terfi);
                if (sonuc.islemSonuc) basarili++;
                else hatalar.Add(s.ad + ": " + PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Onay/red işlemi yapılamadı."));
            }

            if (hatalar.Count == 0)
                return Results.Json(new { basarili = true, bilgi = "Seçili hizmetlerin onay/red işlemi başarıyla gerçekleştirilmiştir." });
            return Results.Json(new
            {
                basarili = basarili > 0,
                hata = string.Join("\n", hatalar) + (basarili > 0 ? "\nDiğer seçili hizmetlerin (" + basarili + ") onay/red işlemi başarıyla gerçekleştirilmiştir." : "")
            });
        }

        private static IResult OnayYazdir(HttpContext baglam, IWebHostEnvironment ortam, int? durum, int? tip)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan, OnaySayfa);
            if (ret != null) return ret;

            Terfi terfi = new Terfi();
            terfi.durum = durum ?? 0;
            terfi.tip = tip ?? 0;
            if (terfi.durum == 0) return Hata("Durum bilgisini seçmelisiniz.");

            string durumAd = "";
            if (terfi.durum == (int)ENUMTerfiDurumu.Beklemede) durumAd = "ONAY BEKLEYENLER";
            if (terfi.durum == (int)ENUMTerfiDurumu.Red) durumAd = "REDDEDİLENLER";
            if (terfi.durum == (int)ENUMTerfiDurumu.Onay) durumAd = "TERFİ EDENLER";

            string sablonAd = PersonelOrtakRaporlar.SablonBul(ortam, "IKY", "TerfiListesi.xltx");
            if (sablonAd == null) return Hata("Rapor şablonu bulunamadı (IKY\\TerfiListesi.xltx).");

            ObjectArray oa = TNS.IKY.Arac.Tanimla().PersonelTerfiListele(kullanan, terfi);
            if (!oa.sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(oa.sonuc.hataStr, "Terfi kayıtları okunamadı."));

            try
            {
                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                string sonucDosyaAd = DosyaIslem.DosyaAdUret("", ".xlsx");
                XLS.DosyaAc(sablonAd, sonucDosyaAd);

                int kaynakSatir = 0, sutun = 0;
                XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
                XLS.HucreAdBulYaz("YilAy", TipAd(terfi.tip) + "' NA GÖRE " + durumAd + " LİSTESİ");
                int satir = kaynakSatir;

                int sayac = 1;
                foreach (Terfi t in oa.objeler)
                {
                    satir++;
                    XLS.SatirAc(satir, 1);
                    // Eski kodda her satırda sutun++ yapılıyordu; biçim kopyası her satırda bir sütun kayıyordu. Sütun sabit tutuldu.
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
                    XLS.HucreDegerYaz(satir, 0, sayac);
                    XLS.HucreDegerYaz(satir, 1, t.ad);
                    XLS.HucreDegerYaz(satir, 2, t.statu);
                    XLS.HucreDegerYaz(satir, 3, t.kazanilmisHakDerece);
                    XLS.HucreDegerYaz(satir, 4, t.kazanilmisHakKademe);
                    XLS.HucreDegerYaz(satir, 5, t.kazanilmisHakEkGosterge);
                    XLS.HucreDegerYaz(satir, 6, t.YeniKazanilmisHakDerece);
                    XLS.HucreDegerYaz(satir, 7, t.YeniKazanilmisHakKademe);
                    XLS.HucreDegerYaz(satir, 8, t.YeniKazanilmisHakEkGosterge);
                    XLS.HucreDegerYaz(satir, 9, t.emekliyeEsasDerece);
                    XLS.HucreDegerYaz(satir, 10, t.emekliyeEsasKademe);
                    XLS.HucreDegerYaz(satir, 11, t.emekliyeEsasEkGosterge);
                    XLS.HucreDegerYaz(satir, 12, t.YeniEmekliyeEsasDerece);
                    XLS.HucreDegerYaz(satir, 13, t.YeniEmekliyeEsasKademe);
                    XLS.HucreDegerYaz(satir, 14, t.YeniEmekliyeEsasEkGosterge);
                    sayac++;
                }

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "TerfiEdenlerinListesi_" + DateTime.Now.Ticks.ToString(), true, "xlsx");
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        // ---------------------------------------------------------------- ortak

        /// <summary>Eski Genel.TerfiTipAdiDondur.</summary>
        public static string TipAd(int tip)
        {
            if (tip == (int)ENUMTerfiTipi.EKEA) return "E.K.E.A.";
            if (tip == (int)ENUMTerfiTipi.KHA) return "K.H.A.";
            if (tip == (int)ENUMTerfiTipi.KHAEKEA) return "K.H.A. ve E.K.E.A.";
            return "K.H.A. veya E.K.E.A.";
        }

        /// <summary>Eski Genel.TerfiDurumAdiDondur.</summary>
        public static string DurumAd(int durum)
        {
            if (durum == (int)ENUMTerfiDurumu.Beklemede) return "Beklemede";
            if (durum == (int)ENUMTerfiDurumu.Onay) return "Onaylandı";
            if (durum == (int)ENUMTerfiDurumu.Red) return "Reddedildi";
            return "";
        }

        private static string TarihYaz(TNSDateTime t)
        {
            return t == null ? "" : t.ToString();
        }

        /// <summary>Tablo satırı (eski ListeStore alanları). renkAta: kadro derecesine ulaşılmış (eski RENKATA, yeşil zemin).</summary>
        private static object Satir(Terfi p, bool onay)
        {
            return new
            {
                p.kod,
                p.pKadroKod,
                p.pKod,
                p.kimlikNo,
                p.ad,
                p.sicilNo,
                p.tip,
                tipAd = TipAd(p.tip),
                p.birim,
                p.unvan,
                p.ogrenim,
                p.kadroDerece,
                kadroTarih = TarihYaz(p.kadroTarih),
                p.kazanilmisHakDerece,
                p.kazanilmisHakKademe,
                kazanilmisHakTarih = TarihYaz(p.kazanilmisHakTarih),
                p.odemeyeEsasDerece,
                p.odemeyeEsasKademe,
                odemeyeEsasTarih = TarihYaz(p.odemeyeEsasTarih),
                p.emekliyeEsasDerece,
                p.emekliyeEsasKademe,
                emekliyeEsasTarih = TarihYaz(p.emekliyeEsasTarih),
                p.yukselecegiDerece,
                renkAta = !onay && (p.kadroDerece == p.kazanilmisHakDerece || p.kadroDerece == p.emekliyeEsasDerece),
                yilAy = onay ? p.yil.ToString() + p.ay.ToString().PadLeft(2, '0') : "",
                p.durum,
                durumAd = onay ? DurumAd(p.durum) : "",
                p.hizmetSebebi,
                p.sebepAd
            };
        }
    }
}
