using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ButceHazirlikv2;
using Istemci.ButceKod;
using Istemci.Organizasyon;
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
using TNS.YAT;

namespace Istemci.ButceHazirlik
{
    public class YollukSatiri
    {
        public string Dairesi { get; set; }
        public string Konusu { get; set; }
        public bool MuhtelifMi { get; set; }
        public int Suresi { get; set; }
        public int KisiSayisi { get; set; }
        public string YapilacagiAy { get; set; }
        public string Yeri { get; set; }
        public double Maliyet { get; set; }
    }

    public class YollukKayitIstegi
    {
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        public string Tertip { get; set; }
        public List<YollukSatiri> Satirlar { get; set; }
    }

    public class YollukDegerIstegi
    {
        public int Yil { get; set; }
        public double YiYolculuk { get; set; }
        public double YiGundelik { get; set; }
        public double YdYolculuk { get; set; }
        public double YdGundelik { get; set; }
    }

    public class YollukOnayIstegi
    {
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        /// <summary>Birinci düzey faaliyet kodu; boşsa bütün birimler (eski Hepsini Onayla / Hepsini Aç).</summary>
        public string Kod { get; set; }
        /// <summary>5: onayla (kilitle), 1: onayı kaldır (aç).</summary>
        public int Durum { get; set; }
    }

    /// <summary>
    /// Yolluk Planlama Formu sayfasının (eski ButceHazirlikv2/YollukPlanlamaForm.aspx) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/yolluk/tanim, /kodlar?yil=&amp;ust=, /degerler?yil=, /faaliyet?yil=&amp;senaryo=&amp;peb=
    ///   GET  /api/yolluk/liste?yil=&amp;senaryo=&amp;tertip=, /onaylar?yil=&amp;senaryo=
    ///   POST /api/yolluk/kaydet, /deger-kaydet, /onay
    ///   GET  /api/yolluk/rapor?yil=&amp;senaryo=&amp;peb= (Excel; eski kodla, YollukPlanlamaRaporu)
    /// Seçilen faaliyetin (PEB) yolluk tertiplerine (ekonomik kod 06.9.2.01/03, 03.3.1/3) eğitim / toplantı planları girilir;
    /// maliyet toplamı tertibin ödeneğine eşit olmalıdır (iş kurallarında denetlenir). Yolluk değerleri (yurt içi / dışı yolculuk
    /// ve gündelik) eskisi gibi "stratek" kullanıcısının YP_YIY/YIG/YDY/YDG+yıl değişkenlerindedir. Değerleri saklama ve birimlerin
    /// onay durumunu değiştirme yalnızca sistem yöneticisinindir; onaylanmış birimin planı değiştirilemez.
    /// </summary>
    public static class YollukPlanlamaUclari
    {
        private const string SayfaAdresi = "../ButceHazirlikv2/YollukPlanlamaForm.aspx";
        private const string DegerKullanici = "stratek"; // eski sayfa yolluk değerlerini bu kullanıcının değişkenlerinde saklıyordu
        // Onay durumu: 5 onaylı, 0 / 1 açık, -1 faaliyet seçilmemiş.
        private const int Onayli = 5;

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/yolluk/tanim", Tanim);
            app.MapGet("/api/yolluk/kodlar", Kodlar);
            app.MapGet("/api/yolluk/degerler", Degerler);
            app.MapGet("/api/yolluk/faaliyet", Faaliyet);
            app.MapGet("/api/yolluk/liste", Liste);
            app.MapGet("/api/yolluk/onaylar", Onaylar);
            app.MapPost("/api/yolluk/kaydet", Kaydet);
            app.MapPost("/api/yolluk/deger-kaydet", DegerKaydet);
            app.MapPost("/api/yolluk/onay", Onay);
            app.MapGet("/api/yolluk/rapor", Rapor);
        }

        /// <summary>Kullanıcı bu sayfayı kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), SayfaAdresi);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
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

        /// <summary>Faaliyet kodu tam mı (eski TamMi: noktasız 8 hane).</summary>
        private static bool TamMi(string pebKod)
        {
            if (string.IsNullOrWhiteSpace(pebKod)) return false;
            return pebKod.Trim().Replace(".", "").Length == 8;
        }

        /// <summary>Faaliyetin birinci düzeyinin onay durumu (eski OnayDurumBul; -1: faaliyet seçilmemiş, 0: kayıt yok).</summary>
        private static int OnayDurumBul(Kullanici kullanan, int yil, string senaryo, string pebKod)
        {
            pebKod = (pebKod ?? "").Trim();
            if (pebKod.Length < 2) return -1;
            TavanGenel tg = new TavanGenel();
            tg.kod1 = pebKod.Substring(0, 2);
            tg = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayOku(kullanan, yil, senaryo ?? "", ENUMTavanTur.yollukPlanlamaForm, tg);
            if (tg == null) return 0;
            return tg.durum;
        }

        private static Sonuc DurumDegistir(Kullanici kullanan, int yil, string senaryo, string kod, int durum)
        {
            TavanGenel tg = new TavanGenel();
            tg.kod1 = kod;
            tg.durum = durum;
            return TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayKaydet(kullanan, yil, senaryo, ENUMTavanTur.yollukPlanlamaForm, tg);
        }

        /// <summary>Faaliyet kodunun adı (eski AdBul(58, ...): ABSPEBKOD uzayı, koşul "yıl-kod").</summary>
        private static string PebAdBul(int yil, string pebKod)
        {
            if (string.IsNullOrWhiteSpace(pebKod)) return "";
            string ad = TNS.UZY.Arac.Tanimla().UzayDegeriStr(new Kullanici(), "ABSPEBKOD", yil + "-" + pebKod.Trim(), true);
            return (ad ?? "").Replace(",", " ");
        }

        /// <summary>
        /// Faaliyetin yolluk tertipleri ve ödenekleri (eski TertipleriYenile): varsayılan kurum, yolluk ekonomik kodları
        /// (eko kod yapısına göre 06.9.2.01, 06.9.2.03, 03.3.1, 03.3.3 ya da uzun biçimleri) ve seçilen faaliyet.
        /// </summary>
        private static List<object> TertipleriOku(Kullanici kullanan, int yil, string senaryo, string pebKod)
        {
            List<object> liste = new List<object>();
            if (yil <= 0 || string.IsNullOrWhiteSpace(senaryo) || !TamMi(pebKod)) return liste;

            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            kistasTertip.kodlar[0] = TNS.DEG.Arac.VarsayilanKurumBul();
            if (kistasTertip.KodUzunlukBul(3, 4, true) == 6)
                kistasTertip.kodlar[3] = "06.9.2.01,06.9.2.03,03.3.1,03.3.3";
            else
                kistasTertip.kodlar[3] = "06.09.20.01,06.09.20.03,03.03.10,03.03.30";
            kistasTertip.kodlar[4] = pebKod.Trim();

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(1, kistasTertip.KodNoKirilimSay(0)));
            kodBilgileri.Add(new UcgenKodBilgi(2, 4));
            kodBilgileri.Add(new UcgenKodBilgi(3, 1));
            kodBilgileri.Add(new UcgenKodBilgi(4, 4));
            kodBilgileri.Add(new UcgenKodBilgi(14, 4));
            EkDegerParam[] ekBilgiler = new EkDegerParam[1];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryo.Trim());
            OdenekBilgi[] obler = TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, false, true);
            foreach (OdenekBilgi ob in obler ?? new OdenekBilgi[0])
            {
                string tertip = ob.tertip.TertipStrOl();
                liste.Add(new { kod = tertip, ad = tertip + " (" + ob.EkRakam(0).ToString("#,###") + " TL)" });
            }
            return liste;
        }

        private static double DegerOku(string ad, int yil)
        {
            return OrtakFonksiyonlar.ConvertToDouble(GenelIslemler.KullaniciDegiskenGetir(DegerKullanici, ad + yil), 0);
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Açılış (eski Page_Load): bütçe hazırlık yılı, aşamalar (ilki seçili) ve sistem yöneticisi mi.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                List<object> senaryolar = new List<object>();
                foreach (ButceSenaryo bs in TNS.BHMv2.Arac.Tanimla().ButceSenaryoListele(kullanan, null) ?? new ButceSenaryo[0])
                    senaryolar.Add(new { kod = bs.kod, ad = bs.kod + "- " + bs.ad });
                return Results.Json(new
                {
                    yil = KodTanimUclari.YilBul(null),
                    senaryolar = senaryolar,
                    sistemYoneticisi = GenelGorevUclari.YetkiVar(kullanan)
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>Faaliyet (14) kod ağacının bir düzeyi (eski KodAgac.aspx?kodTur=14), bütçe hazırlık yetki koduyla.</summary>
        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return ButceRakamUclari.KodDuzeyi(kullanan, yil, "14", ust);
        }

        /// <summary>Yılın yolluk değerleri (eski RakamlariOku).</summary>
        private static IResult Degerler(HttpContext baglam, IWebHostEnvironment ortam, int yil)
        {
            IResult ret = Denetle(baglam, ortam, out _);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                return Results.Json(new
                {
                    yiYolculuk = DegerOku("YP_YIY", yil),
                    yiGundelik = DegerOku("YP_YIG", yil),
                    ydYolculuk = DegerOku("YP_YDY", yil),
                    ydGundelik = DegerOku("YP_YDG", yil)
                });
            }
            catch (Exception)
            {
                return Hata("Yolluk değerleri okunamadı.");
            }
        }

        /// <summary>Faaliyet değişince (eski pebDegisti): faaliyetin adı, yolluk tertipleri ve onay durumu.</summary>
        private static IResult Faaliyet(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryo, string peb)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                peb = (peb ?? "").Trim();
                return Results.Json(new
                {
                    ad = PebAdBul(yil, peb),
                    tertipler = TertipleriOku(kullanan, yil, senaryo, peb),
                    durum = OnayDurumBul(kullanan, yil, senaryo, peb)
                });
            }
            catch (Exception)
            {
                return Hata("Faaliyet bilgileri okunamadı.");
            }
        }

        /// <summary>Yolluk planı (eski Oku): seçilen tertibin satırları. Tertip boşsa eskisi gibi tertip süzmeden okunur.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryo, string tertip)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            if (string.IsNullOrWhiteSpace(senaryo)) return Hata("Lütfen aşama seçiniz!");
            try
            {
                Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                t.TertipAta((tertip ?? "").Trim());
                ObjectArray oa = TNS.YAT.Arac.Tanimla().ListeleYollukPlanlama(kullanan, yil, senaryo.Trim(), t, null);
                if (oa == null) return Hata("Liste okunamadı.");
                if (!oa.sonuc.islemSonuc) return Hata(HataMetni(oa.sonuc.hataStr, "Liste okunamadı."));

                List<object> satirlar = new List<object>();
                for (int i = 0; i < oa.ObjeSayisi; i++)
                {
                    YollukPlan yp = (YollukPlan)oa.objeler[i];
                    satirlar.Add(new
                    {
                        siraNo = yp.sIRANO, dairesi = yp.dairesi, konusu = yp.konusu, muhtelifMi = yp.muhtelifMi,
                        suresi = yp.suresi, kisiSayisi = yp.kisiSayisi, yapilacagiAy = yp.yapilacagiAy, yeri = yp.yeri, maliyet = yp.mALIYET
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Liste okunamadı.");
            }
        }

        /// <summary>Birimlerin onay durumu (eski wndOnay_Load): birinci düzey faaliyet kodları. Yalnızca sistem yöneticisi.</summary>
        private static IResult Onaylar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!GenelGorevUclari.YetkiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                List<object> satirlar = new List<object>();
                ABSKodlarVAN akv = TNS.BKD.Arac.Tanimla().KodListele(yil, "14", "");
                if (akv != null && akv.absKodlar != null)
                    foreach (ABSKod item in akv.absKodlar)
                    {
                        int durum = OnayDurumBul(kullanan, yil, senaryo, item.kod);
                        satirlar.Add(new { kod = item.kod, ad = item.ad, onayli = durum == Onayli });
                    }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Onay durumları okunamadı.");
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>
        /// Kaydeder (eski Kaydet): tertibin var olan planı silinip satırlar sırayla yazılır. Alan denetimleri ve maliyet toplamının
        /// tertip ödeneğine eşitliği iş kurallarındadır. Onaylanmış birimin planı değiştirilemez (eski sayfa yalnızca tuşu kapatıyordu).
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, YollukKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });
            if (string.IsNullOrWhiteSpace(istek.Senaryo)) return Results.Json(new { basarili = false, hata = "Lütfen aşama seçiniz!" });
            if (string.IsNullOrWhiteSpace(istek.Tertip)) return Results.Json(new { basarili = false, hata = "Lütfen tertip seçiniz!" });

            try
            {
                string senaryo = istek.Senaryo.Trim();
                Tertip t = new Tertip(istek.Yil, ENUMTabloKod.TABPEBOdenek);
                t.TertipAta(istek.Tertip.Trim());
                if (OnayDurumBul(kullanan, istek.Yil, senaryo, OrtakFonksiyonlar.DiziElemanAl(t.kodlar, 4, "")) == Onayli)
                    return Results.Json(new { basarili = false, hata = "Birimin yolluk planı onaylanmış, değiştirilemez." });

                TNSCollection formlar = new TNSCollection();
                foreach (YollukSatiri s in istek.Satirlar ?? new List<YollukSatiri>())
                {
                    if (s == null) continue;
                    YollukPlan yp = new YollukPlan();
                    yp.dairesi = (s.Dairesi ?? "").Trim();
                    yp.konusu = (s.Konusu ?? "").Trim();
                    yp.muhtelifMi = s.MuhtelifMi;
                    yp.suresi = s.Suresi;
                    yp.kisiSayisi = s.KisiSayisi;
                    yp.yapilacagiAy = (s.YapilacagiAy ?? "").Trim();
                    yp.yeri = (s.Yeri ?? "").Trim();
                    yp.mALIYET = s.Maliyet;
                    formlar.Add(yp);
                }

                Sonuc sonuc = TNS.YAT.Arac.Tanimla().KaydetYollukPlanlama(kullanan, istek.Yil, senaryo, t, formlar);
                if (sonuc == null || !sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc?.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarı ile tamamlandı." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
        }

        /// <summary>Yolluk değerlerini saklar (eski btnDegerKaydet_Click): yalnızca sistem yöneticisi.</summary>
        private static async Task<IResult> DegerKaydet(HttpContext baglam, IWebHostEnvironment ortam, YollukDegerIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!GenelGorevUclari.YetkiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });

            try
            {
                Kullanici k = new Kullanici();
                k.kullaniciKodu = DegerKullanici;
                string yil = istek.Yil.ToString();
                Sonuc s = GenelIslemler.KullaniciDegiskenSakla(k, "YP_YIY" + yil, istek.YiYolculuk.ToString());
                if (s != null && s.islemSonuc) s = GenelIslemler.KullaniciDegiskenSakla(k, "YP_YIG" + yil, istek.YiGundelik.ToString());
                if (s != null && s.islemSonuc) s = GenelIslemler.KullaniciDegiskenSakla(k, "YP_YDY" + yil, istek.YdYolculuk.ToString());
                if (s != null && s.islemSonuc) s = GenelIslemler.KullaniciDegiskenSakla(k, "YP_YDG" + yil, istek.YdGundelik.ToString());
                if (s != null && !s.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(s.hataStr, "Değerler saklanamadı.") });
                return Results.Json(new { basarili = true, bilgi = "Yolluk değerleri saklandı." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Değerler saklanamadı." });
            }
        }

        /// <summary>
        /// Birimin onay durumunu değiştirir (eski DurumDegistirBirim; kod boşsa btnHepsiniOnayla_Click gibi bütün birinci düzey
        /// faaliyet kodları, ilk hatada durur). Yalnızca sistem yöneticisi.
        /// </summary>
        private static async Task<IResult> Onay(HttpContext baglam, IWebHostEnvironment ortam, YollukOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!GenelGorevUclari.YetkiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });
            if (string.IsNullOrWhiteSpace(istek.Senaryo)) return Results.Json(new { basarili = false, hata = "Lütfen aşama seçiniz!" });
            int durum = istek.Durum == Onayli ? Onayli : 1;

            try
            {
                string senaryo = istek.Senaryo.Trim();
                Sonuc s = new Sonuc(true, "", "");
                if (!string.IsNullOrWhiteSpace(istek.Kod))
                    s = DurumDegistir(kullanan, istek.Yil, senaryo, istek.Kod.Trim(), durum);
                else
                {
                    ABSKodlarVAN akv = TNS.BKD.Arac.Tanimla().KodListele(istek.Yil, "14", "");
                    if (akv != null && akv.absKodlar != null)
                        foreach (ABSKod item in akv.absKodlar)
                        {
                            s = DurumDegistir(kullanan, istek.Yil, senaryo, item.kod, durum);
                            if (s == null || !s.islemSonuc) break;
                        }
                }
                if (s == null || !s.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(s?.hataStr, "İşlem başarısız.") });
                return Results.Json(new { basarili = true, bilgi = durum == Onayli ? "Onaylandı." : "Onay kaldırıldı." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "İşlem başarısız." });
            }
        }

        // ------------------------------------------------------------------ rapor (eski kodla)

        /// <summary>
        /// Yazdır (eski btnYazdir_Click → TekRaporAl(2003, 14), YatirimRaporlari "YP"): faaliyetin yolluk planı
        /// (şablon YAT\YATYollukPlanlama.xltx).
        /// </summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryo, string peb)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            if (string.IsNullOrWhiteSpace(senaryo)) return Hata("Lütfen aşama seçiniz!");
            try
            {
                AsposeLisans.Yukle();
                string dosya = YollukPlanlamaRaporu.YollukPlani(kullanan, yil, senaryo.Trim(), (peb ?? "").Trim(), Ayar.Oku("RaporSablonYol"));
                return DosyaIslem.DosyaGonder(dosya, "YollukPlanlama_" + yil, true, "xlsx");
            }
            catch (RaporHatasi ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }
    }
}
