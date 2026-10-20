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
using TNS.HRC;
using TNS.KYM;

namespace Istemci.HarcamaSurecleri
{
    /// <summary>Tablo değer satırı: alan adları BordroTabloDetay sınıfındaki gibi.</summary>
    public class BordroTabloSatiri
    {
        public int derece { get; set; }
        public decimal deger { get; set; }
        public decimal carpan { get; set; }
    }

    /// <summary>Kaydet isteği (eski btnKaydet_Click: seçili tablo, yıl, ünvan ve tablodaki satırlar).</summary>
    public class BordroTabloKaydetIstegi
    {
        public int kod { get; set; }
        public int yil { get; set; }
        public int unvanKod { get; set; }
        public List<BordroTabloSatiri> satirlar { get; set; }
    }

    /// <summary>Tablo bilgi düzenleme / giriş penceresi (eski wndTabloBilgi). tur: 1 Ünvan/Derece, 2 Derece/Kademe.</summary>
    public class BordroTabloTanimIstegi
    {
        public int kod { get; set; }
        public string ad { get; set; }
        public int tur { get; set; }
    }

    /// <summary>Artış Yap (eski btnArtis_Click).</summary>
    public class BordroTabloArtisIstegi
    {
        public int kod { get; set; }
        public int yil { get; set; }
        public double katsayi { get; set; }
    }

    /// <summary>Kopyala (eski btnTabloKopyala_Click).</summary>
    public class BordroTabloKopyalaIstegi
    {
        public int kod { get; set; }
        public int yil { get; set; }
        public int kaynakKod { get; set; }
        public int unvanKod { get; set; }
        public bool ustuneYaz { get; set; }
    }

    /// <summary>Maaş Hesapla (eski btnHesaplaYazdir_Click). unvanKod 0: bütün aktif ünvanlar.</summary>
    public class BordroTabloMaasIstegi
    {
        public int yil { get; set; }
        public int unvanKod { get; set; }
    }

    /// <summary>
    /// Bordro Tabloları (eski HarcamaSurecleri/BordroTabloForm.aspx; yeni sayfa HarcamaSurecleri/BordroTabloForm) uçları:
    ///   GET  /api/bordrotablo/baslangic     tablolar + aktif ünvanlar (eski UnvanDoldur)
    ///   GET  /api/bordrotablo/tablolar      tablolar
    ///   GET  /api/bordrotablo/oku           seçili tablo, ünvan ve yılın değerleri (eski Listele)
    ///   POST /api/bordrotablo/kaydet        (eski Kaydet)
    ///   POST /api/bordrotablo/tanimkaydet   tablo kodu / adı / türü (eski wndTabloBilgi Kaydet)
    ///   GET  /api/bordrotablo/yazdir        seçili tablonun bütün ünvanları .xlsx (eski Yazdır)
    ///   POST /api/bordrotablo/yukle         Excel'den değer yükleme (eski Dosya Yükle)
    ///   POST /api/bordrotablo/artis         (eski Artış Yap)
    ///   POST /api/bordrotablo/kopyala       (eski Kopyala)
    ///   POST /api/bordrotablo/maashesapla   TKDKMaasHesap.xlsx (eski Maaş Hesapla)
    /// İş kuralları eski sayfadaki gibi IHRCServis (ListeleBordroTabloAna, ListeleBordroTablo, KaydetBordroTablo, KaydetBordroTabloTanim,
    /// BordroTabloArttir, KopyalaBordroTablo, PSLMaasTemelBilgisiListele, PSLGelirVergisiListele) ve IIKYServis.UnvanListele.
    /// Maaş hesabı eski sayfanın kendi (istemci katmanındaki) kodudur; aynen alındı.
    /// </summary>
    public static class BordroTabloUclari
    {
        public const string SayfaAdresi = "../HarcamaSurecleri/BordroTabloForm.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/bordrotablo/baslangic", Baslangic);
            app.MapGet("/api/bordrotablo/tablolar", Tablolar);
            app.MapGet("/api/bordrotablo/oku", Oku);
            app.MapPost("/api/bordrotablo/kaydet", Kaydet);
            app.MapPost("/api/bordrotablo/tanimkaydet", TanimKaydet);
            app.MapGet("/api/bordrotablo/yazdir", Yazdir);
            app.MapPost("/api/bordrotablo/yukle", Yukle).DisableAntiforgery();
            app.MapPost("/api/bordrotablo/artis", Artis);
            app.MapPost("/api/bordrotablo/kopyala", Kopyala);
            app.MapPost("/api/bordrotablo/maashesapla", MaasHesaplaYazdir);
        }

        /// <summary>Eski Page_Load: GenelIslemler.SayfayaGirisYapabilirMi (menü yetkisi).</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static string HataMetni(Sonuc sonuc, string varsayilan)
        {
            string hata = (sonuc.hataStr ?? "").Replace("<br>", " ").Replace("<br/>", " ");
            return hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at ") ? varsayilan : hata.Trim();
        }

        // ------------------------------------------------------------------ başlangıç (eski UnvanDoldur)

        private static List<object> TabloListesi(Kullanici kullanan, IHRCServis servis, out string hata)
        {
            hata = null;
            List<object> l = new List<object>();
            ObjectArray oa = servis.ListeleBordroTabloAna(kullanan);
            if (!oa.sonuc.islemSonuc) { hata = HataMetni(oa.sonuc, "Bordro tabloları okunamadı."); return l; }
            foreach (BordroTabloAna a in oa.objeler)
                l.Add(new { a.kod, a.ad, tur = (int)a.tur });
            return l;
        }

        private static IResult Baslangic(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            List<object> tablolar = TabloListesi(kullanan, TNS.HRC.Arac.Tanimla(), out string hata);

            List<object> unvanlar = new List<object>();
            ObjectArray dler = TNS.IKY.Arac.Tanimla().UnvanListele(new TNS.IKY.Unvan { aktif = true });
            foreach (TNS.IKY.Unvan u in dler.objeler)
                unvanlar.Add(new { u.UnvanKod, u.UnvanAd });

            return Results.Json(new { hata, tablolar, unvanlar });
        }

        private static IResult Tablolar(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            List<object> tablolar = TabloListesi(kullanan, TNS.HRC.Arac.Tanimla(), out string hata);
            return Results.Json(new { hata, tablolar });
        }

        // ------------------------------------------------------------------ oku / kaydet

        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int? kod, int? unvanKod, int? yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            BordroTabloDetay form = new BordroTabloDetay();
            form.kod = kod ?? 0;
            form.unvanKod = unvanKod ?? 0;
            form.yil = yil ?? 0;
            if (form.kod <= 0 || form.unvanKod <= 0) return Hata("Tablo kodu ve unvan kodu seçiniz!");

            ObjectArray oa = TNS.HRC.Arac.Tanimla().ListeleBordroTablo(kullanan, form);
            if (!oa.sonuc.islemSonuc) return Hata(HataMetni(oa.sonuc, "Tablo değerleri okunamadı."));

            List<object> satirlar = new List<object>();
            foreach (BordroTabloDetay d in oa.objeler)
                satirlar.Add(new { d.derece, d.deger, d.carpan });
            return Results.Json(new { basarili = true, satirlar });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, BordroTabloKaydetIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            TNSCollection bordroTabloDetay = new TNSCollection();
            if (istek.satirlar != null)
                foreach (BordroTabloSatiri s in istek.satirlar)
                {
                    if (s == null) continue;
                    BordroTabloDetay d = new BordroTabloDetay();
                    d.derece = s.derece;
                    d.deger = s.deger;
                    d.carpan = s.carpan;
                    bordroTabloDetay.Add(d);
                }

            Sonuc sonuc = TNS.HRC.Arac.Tanimla().KaydetBordroTablo(kullanan, istek.kod, istek.yil, istek.unvanKod, bordroTabloDetay);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kayıt yapılamadı."));
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
        }

        private static async Task<IResult> TanimKaydet(HttpContext baglam, IWebHostEnvironment ortam, BordroTabloTanimIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            BordroTabloAna ana = new BordroTabloAna();
            ana.kod = istek.kod;
            ana.ad = istek.ad ?? "";
            ana.tur = istek.tur == 2 ? BordroTabloAna.ENUMBordroTabloTur.DereceKademe : BordroTabloAna.ENUMBordroTabloTur.UnvanDerece;
            Sonuc sonuc = TNS.HRC.Arac.Tanimla().KaydetBordroTabloTanim(kullanan, ana);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kayıt yapılamadı."));
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti.", kod = ana.kod });
        }

        // ------------------------------------------------------------------ yazdır (eski btnYazdir_Click)

        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int? kod, int? yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            try
            {
                AsposeLisans.Yukle();
                string dosyaAdxls = DosyaIslem.DosyaAdUret("AracTip", "") + "." + GenelIslemler.ExcelTur();
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.BosDosyaAc(dosyaAdxls);

                BordroTabloDetay f = new BordroTabloDetay();
                f.kod = kod ?? 0;
                f.yil = yil ?? 0;

                ObjectArray oa = TNS.HRC.Arac.Tanimla().ListeleBordroTablo(kullanan, f);
                int eskiUnvanKod = -10;
                int sat = 0;
                foreach (BordroTabloDetay item in oa.objeler)
                {
                    if (eskiUnvanKod != item.unvanKod)
                    {
                        if (sat == 0)
                        {
                            XLS.HucreDegerYaz(sat, 0, item.ad);
                        }
                        sat++;
                        eskiUnvanKod = item.unvanKod;
                        XLS.HucreDegerYaz(sat, 0, item.unvanKod);
                        XLS.HucreDegerYaz(sat, 1, item.unvanAd);
                    }
                    int kol = 2 + (item.derece - 1) * 2;
                    XLS.HucreDegerYaz(sat, kol, item.deger);
                    XLS.HucreDegerYaz(sat, kol + 1, item.carpan);
                }
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(dosyaAdxls, "Tablo" + f.kod.ToString() + "_" + f.yil.ToString() + ".xlsx", true, "xlsx");
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        // ------------------------------------------------------------------ dosya yükle (eski btnYukle_FileSelected)

        private static async Task<IResult> Yukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            IFormCollection form = await baglam.Request.ReadFormAsync();
            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length <= 0) return Hata("Lütfen Dosya Seçiniz");

            int kod = OrtakFonksiyonlar.ConvertToInt((string)form["kod"], 0);
            int yil = OrtakFonksiyonlar.ConvertToInt((string)form["yil"], 0);
            IHRCServis servis = TNS.HRC.Arac.Tanimla();
            try
            {
                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                using (MemoryStream akis = new MemoryStream())
                {
                    await dosya.CopyToAsync(akis);
                    akis.Position = 0;
                    XLS.DosyaOkuAc(akis);
                }
                int sat = 0;
                int oncUnvanKod = -100;
                TNSCollection bordroTabloDetay = new TNSCollection();
                while (true)
                {
                    string unvanKodStr = XLS.HucreDegerAl(sat, 0);
                    if (string.IsNullOrWhiteSpace(unvanKodStr)) break;
                    int unvanKod = OrtakFonksiyonlar.ConvertToInt(unvanKodStr, -1);
                    if (oncUnvanKod != unvanKod)
                    {
                        if (bordroTabloDetay.Count > 0)
                        {
                            Sonuc s = servis.KaydetBordroTablo(kullanan, kod, yil, oncUnvanKod, bordroTabloDetay);
                            if (!s.islemSonuc) return Hata("Dosya işlenemedi:" + s.hataStr);
                        }
                        bordroTabloDetay.Clear();
                        oncUnvanKod = unvanKod;
                    }
                    BordroTabloDetay btd = new BordroTabloDetay();
                    btd.derece = OrtakFonksiyonlar.ConvertToInt(XLS.HucreDegerAl(sat, 1), -1);
                    btd.deger = Convert.ToDecimal(XLS.HucreDegerAlDbl(sat, 2));
                    btd.carpan = Convert.ToDecimal(XLS.HucreDegerAlDbl(sat, 3));
                    bordroTabloDetay.Add(btd);
                    sat++;
                }
                XLS.DosyaKapat();
                if (bordroTabloDetay.Count > 0)
                {
                    Sonuc s = servis.KaydetBordroTablo(kullanan, kod, yil, oncUnvanKod, bordroTabloDetay);
                    if (!s.islemSonuc) return Hata("Dosya işlenemedi:" + s.hataStr);
                }
                return Results.Json(new { basarili = true, bilgi = "Dosya yüklendi." });
            }
            catch
            {
                return Hata("Dosya işlenemedi");
            }
        }

        // ------------------------------------------------------------------ artış / kopyala

        private static async Task<IResult> Artis(HttpContext baglam, IWebHostEnvironment ortam, BordroTabloArtisIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            if (istek.katsayi <= 0) return Hata("Artış katsayısı giriniz!");
            Sonuc s = TNS.HRC.Arac.Tanimla().BordroTabloArttir(kullanan, istek.kod, istek.yil, istek.katsayi);
            if (!s.islemSonuc) return Hata(HataMetni(s, "Artış yapılamadı."));
            return Results.Json(new { basarili = true, bilgi = "Aktarıldı." });
        }

        private static async Task<IResult> Kopyala(HttpContext baglam, IWebHostEnvironment ortam, BordroTabloKopyalaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            Sonuc sonuc = TNS.HRC.Arac.Tanimla().KopyalaBordroTablo(kullanan, istek.yil, istek.kod, istek.kaynakKod, istek.unvanKod, istek.ustuneYaz);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kopyalama yapılamadı."));
            return Results.Json(new { basarili = true, bilgi = "Kopyalama tamamlandı!" });
        }

        // ------------------------------------------------------------------ maaş hesapla (eski btnHesaplaYazdir_Click)

        /// <summary>Şablon önce kurumun RaporSablonYol\HRC klasöründe, yoksa uygulamayla gelen RaporSablon\HRC klasöründe aranır.</summary>
        private static string SablonBul(string uygulamaKlasoru, string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "HRC", sablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "HRC", sablonAd);
            return File.Exists(u) ? u : null;
        }

        private static async Task<IResult> MaasHesaplaYazdir(HttpContext baglam, IWebHostEnvironment ortam, BordroTabloMaasIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            string sablon = SablonBul(ortam.ContentRootPath, "TKDKMaasHesap.xltx");
            if (sablon == null) return Hata("Rapor şablonu bulunamadı (HRC\\TKDKMaasHesap.xltx).");

            try
            {
                return MaasHesaplaYaz(kullanan, sablon, istek.yil, istek.unvanKod);
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        private static IResult MaasHesaplaYaz(Kullanici kullanan, string sablon, int YIL, int seciliUnvanKod)
        {
            IHRCServis servis = TNS.HRC.Arac.Tanimla();
            AsposeLisans.Yukle();
            TNS.HRC.Arac.TabloDegerSifirla();
            Tablo XLS = GenelIslemler.NewTablo();
            string sonucDosya = DosyaIslem.DosyaAdUret("", DateTime.Now.Ticks.ToString()) + ".xlsx";
            XLS.DosyaAc(sablon, sonucDosya);
            int shNo = 0, sat1 = 0, sut1 = 0, sat2 = 0, sut2 = 0, bSat = 0, bSut = 0, unvanSat = 0, unvanSut = 0;
            int lshNo = 0, lsat1 = 0, lsut1 = 0, lsat2 = 0, lsut2 = 0, lbSat = 0, lbSut = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref shNo, ref sat1, ref sut1, ref sat2, ref sut2);
            XLS.HucreAdAdresCoz("baslaSatir", ref bSat, ref bSut);

            XLS.HucreAdAdresCoz("FormatSatir2", ref lshNo, ref lsat1, ref lsut1, ref lsat2, ref lsut2);
            XLS.HucreAdAdresCoz("BaslaSatir2", ref lbSat, ref lbSut);
            XLS.HucreAdAdresCoz("unvanAd", ref unvanSat, ref unvanSut);
            XLS.AktifSheetDegistir(lshNo);
            int satYuk = XLS.SatirYukseklikAl(lbSat);

            TNS.IKY.Unvan kriterUvan = new TNS.IKY.Unvan { aktif = true };
            kriterUvan.kod = seciliUnvanKod;

            ObjectArray dler = TNS.IKY.Arac.Tanimla().UnvanListele(kriterUvan);
            int satirListe = lbSat;

            foreach (TNS.IKY.Unvan unvan in dler.objeler)
            {
                for (int kidemYil = 0; kidemYil <= 25; kidemYil++)
                {
                    ObjectArray oa2 = MaasHesapla(servis, kullanan, YIL, kidemYil, unvan);

                    MaasHesapSonucAlanlar ilkSayfa = null;
                    XLS.AktifSheetDegistir(lshNo);
                    int sutunListe = lbSut;
                    foreach (TNSCollection unvanDegerlerL in oa2.objeler)
                    {
                        if (unvanDegerlerL.Count == 0) continue;
                        int tur = 1;
                        ilkSayfa = (MaasHesapSonucAlanlar)unvanDegerlerL[0];

                        XLS.HucreKopyala(lsat1, lsut1, lsat2, lsut2, satirListe, sutunListe);
                        XLS.HucreDegerYaz(satirListe, sutunListe, ilkSayfa.unvanAd);
                        XLS.HucreDegerYaz(satirListe, sutunListe + 1, ilkSayfa.kidem);
                        sutunListe = sutunListe + 2;
                        do
                        {
                            TNSCollection degerler = new TNSCollection();
                            foreach (MaasHesapSonucAlanlar mhsa in unvanDegerlerL)
                            {
                                if (mhsa.ay == 0) continue;
                                if (mhsa.tur != tur) continue;
                                BordroTabloDetay td = new BordroTabloDetay();
                                td.derece = mhsa.ay;
                                td.deger = Convert.ToDecimal(mhsa.netMaas);
                                td.carpan = 1;
                                degerler.Add(td);
                                XLS.HucreDegerYaz(satirListe, sutunListe, mhsa.netMaas);
                                sutunListe++;
                            }
                            servis.KaydetBordroTablo(kullanan, tur * 100 + kidemYil, YIL, unvan.kod, degerler);
                            tur++;
                        } while (tur <= 2);
                        XLS.SatirYukseklikAyarla(satirListe, satirListe, satYuk);
                        satirListe++;
                    }

                    XLS.AktifSheetDegistir(shNo);
                    int sutGen = XLS.SutunGenislikAl(bSut - 1);
                    int yilBaslaSutun = -1;
                    foreach (TNSCollection unvanDegerler in oa2.objeler)
                    {
                        if (unvanDegerler.Count == 0) continue;
                        ilkSayfa = (MaasHesapSonucAlanlar)unvanDegerler[0];

                        int satir = bSat;
                        int sutun = bSut;
                        int yeniSheetNo = XLS.YeniSheetEkle(shNo);
                        XLS.SheetAdiVer(XLS.AktifSheet(), ilkSayfa.unvanAd.Substring(0, ilkSayfa.unvanAd.Length > 25 ? 25 : ilkSayfa.unvanAd.Length) + $" ({kidemYil})");
                        XLS.HucreDegerYaz(unvanSat, unvanSut, ilkSayfa.unvanAd);
                        bool ilk = true;

                        foreach (MaasHesapSonucAlanlar mhsa in unvanDegerler)
                        {
                            XLS.HucreKopyala(sat1, sut1, sat2, sut2, satir, sutun);

                            if (mhsa.ay == 0)
                                XLS.HucreDegerYaz(satir, sutun, mhsa.yil - 1);
                            else if (ilk)
                            {
                                XLS.HucreDegerYaz(satir, sutun, mhsa.yil);
                                ilk = false;
                                yilBaslaSutun = sutun;
                            }
                            XLS.HucreDegerYaz(satir + 1, sutun, OrtakFonksiyonlar.AyAdiGetir(mhsa.ay == 0 ? 12 : mhsa.ay) + TurAd(mhsa.tur));
                            XLS.HucreDegerYaz(satir + 2, sutun, mhsa.gostergeMaas);
                            XLS.HucreDegerYaz(satir + 3, sutun, mhsa.ekGostergeMaas);
                            XLS.HucreDegerYaz(satir + 4, sutun, mhsa.tabanAyligi);
                            XLS.HucreDegerYaz(satir + 5, sutun, mhsa.kidemMaas);
                            XLS.HucreDegerYaz(satir + 6, sutun, mhsa.ozelHizmetTazminati);
                            XLS.HucreDegerYaz(satir + 7, sutun, mhsa.yanOdemeMaasi);
                            XLS.HucreDegerYaz(satir + 8, sutun, mhsa.ekOdemeMaasi);
                            XLS.HucreDegerYaz(satir + 9, sutun, mhsa.brutSozlesmeUcreti);
                            XLS.HucreDegerYaz(satir + 10, sutun, mhsa.ozelHizmetTazminati2);
                            if (mhsa.ay == 1 && mhsa.tur == 1)
                            {
                                XLS.HucreDegerYaz(satir + 2, bSut - 1, mhsa.gosterge);
                                XLS.HucreDegerYaz(satir + 3, bSut - 1, mhsa.ekGosterge);
                                XLS.HucreDegerYaz(satir + 4, bSut - 1, mhsa.tabanAylikKatsayisi);
                                XLS.HucreDegerYaz(satir + 5, bSut - 1, mhsa.kidem);
                                XLS.HucreDegerYaz(satir + 6, bSut - 1, mhsa.ozelHizmetTazminatiOrani);
                                XLS.HucreDegerYaz(satir + 7, bSut - 1, mhsa.yanOdeme);
                                XLS.HucreDegerYaz(satir + 8, bSut - 1, mhsa.ekOdeme);
                                XLS.HucreDegerYaz(satir + 9, bSut - 1, mhsa.brutSozlesmeGostergesi);
                                XLS.HucreDegerYaz(satir + 10, bSut - 1, mhsa.ozelHizmetTazminatiGostergesi);
                            }

                            XLS.HucreDegerYaz(satir + 12, sutun, mhsa.gelirToplami);

                            if (mhsa.ay > 0)
                            {
                                XLS.HucreDegerYaz(satir + 14, sutun, mhsa.emekliKesenegiSahis);
                                XLS.HucreDegerYaz(satir + 15, sutun, mhsa.emekliKesenegiKurum);
                                XLS.HucreDegerYaz(satir + 16, sutun, mhsa.gelirVergisiMatrahi);
                                XLS.HucreDegerYaz(satir + 17, sutun, mhsa.gelirVergisi);
                                XLS.HucreDegerYaz(satir + 18, sutun, mhsa.damgaVergisi);
                                XLS.HucreDegerYaz(satir + 20, sutun, mhsa.kesintilerToplami);
                                XLS.HucreDegerYaz(satir + 22, sutun, mhsa.netMaas);
                            }
                            sutun++;
                        }
                        if (yilBaslaSutun > -1)
                        {
                            XLS.HucreBirlestir(satir, yilBaslaSutun, satir, sutun - 1);
                            XLS.HucreBirlestir(bSat - 2, bSut - 2, bSat - 2, sutun - 1);
                        }
                        XLS.SutunGenislikAyarla(bSut, sutun - 1, -1, 3, sutGen);
                    }
                }
            }
            XLS.SheetSil(shNo);
            XLS.AktifSheetDegistir(lshNo);
            XLS.DosyaSaklaTamYol();
            return DosyaIslem.DosyaGonder(sonucDosya, "TKDKMaasHesap.xlsx", true, "xlsx");
        }

        private static string TurAd(int tur)
        {
            if (tur == 2) return " FARK";
            return "";
        }

        private static ObjectArray MaasHesapla(IHRCServis servis, Kullanici kullanan, int yil, int kidemYil, TNS.IKY.Unvan unvan)
        {
            PSLMaasTemelBilgisi maasTemelBilgisi = new PSLMaasTemelBilgisi();
            maasTemelBilgisi.yil = yil;
            maasTemelBilgisi.ay = 0;
            ObjectArray temelBilgiler = servis.PSLMaasTemelBilgisiListele(null, maasTemelBilgisi);
            maasTemelBilgisi.yil = yil - 1;
            maasTemelBilgisi.ay = 12;
            ObjectArray temelBilgiler2 = servis.PSLMaasTemelBilgisiListele(null, maasTemelBilgisi);
            if (temelBilgiler2.ObjeSayisi == 1)
            {
                TNS.HRC.PSLMaasTemelBilgisi f = (TNS.HRC.PSLMaasTemelBilgisi)temelBilgiler2.objeler[0];
                f.yil = yil;
                f.ay = 0;
                temelBilgiler.objeler.Add(f);
            }

            TNS.HRC.PSLGelirVergisi kriter = new PSLGelirVergisi();
            kriter.yil = yil;
            ObjectArray gelirVergisiOranlari = servis.PSLGelirVergisiListele(null, kriter);
            ObjectArray donusDeger = new ObjectArray();
            {
                TNSCollection unvanDeger = new TNSCollection();
                MaasHesapSonuc mhsOnc = null;
                try
                {
                    mhsOnc = new MaasHesapSonuc(servis, kullanan, temelBilgiler, yil, 0, unvan.kod, unvan.ad, kidemYil);
                }
                catch { return donusDeger; }
                MaasHesapSonucAlanlar mhsSonuc = new MaasHesapSonucAlanlar(mhsOnc, 0, gelirVergisiOranlari);
                unvanDeger.Add(mhsSonuc);
                double oncMatrah = 0;
                for (int ay = 1; ay <= 12; ay++)
                {
                    MaasHesapSonuc mhs = new MaasHesapSonuc(servis, kullanan, temelBilgiler, yil, ay, unvan.kod, unvan.ad, kidemYil);
                    if (!OrtakFonksiyonlar.EsitMi(mhsOnc.gostergeMaas, mhs.gostergeMaas))
                    {
                        mhsSonuc = new MaasHesapSonucAlanlar(mhs, mhsOnc, oncMatrah, gelirVergisiOranlari);
                        unvanDeger.Add(mhsSonuc);
                        oncMatrah += mhsSonuc.gelirVergisiMatrahi;
                    }
                    mhsSonuc = new MaasHesapSonucAlanlar(mhs, oncMatrah, gelirVergisiOranlari);
                    unvanDeger.Add(mhsSonuc);
                    oncMatrah += mhsSonuc.gelirVergisiMatrahi;
                    mhsOnc = mhs;
                }
                donusDeger.Ekle(unvanDeger);
            }
            return donusDeger;
        }

        /// <summary>Eski BordroTabloForm.MaasHesapSonucAlanlar: bir ayın (ya da fark satırının) maaş kalemleri.</summary>
        public class MaasHesapSonucAlanlar
        {
            public int unvanKod;
            public string unvanAd;
            public int yil;
            public int ay;
            public int tur;

            public double gelirVergisiMuaf = 0;
            public double damgaVergisiMatrahMuaf = 0;
            public double maasKatsayisi;
            public double tabanAylikKatsayisi;
            public double yanOdemeKatsayisi;
            public double brutSozlesmeGostergesi = 0;
            public double ozelHizmetTazminatiGostergesi = 0;
            public double ozelHizmetTazminatiOrani = 0;
            public int gosterge = 0;
            public int ekGosterge = 0;
            public int kidem = 0;
            public int ekOdeme = 0;
            public int yanOdeme = 0;
            public int ozelHizmetTazminatOraniEmekli = 0;

            public double gostergeMaas;
            public double ekGostergeMaas;
            public double tabanAyligi;
            public double kidemMaas;
            public double ozelHizmetTazminati;
            public double ozelHizmetTazminatiEmekli;
            public double ozelHizmetTazminati2;
            public double yanOdemeMaasi;
            public double ekOdemeMaasi;
            public double brutSozlesmeUcreti;
            private double ustLimit;

            private double GelirVergisiMuafHaric(double gelirVergisi)
            {
                double tut = gelirVergisi - gelirVergisiMuaf;
                if (tut > 0) return tut;
                return 0;
            }
            private double DamgaVergisiMatrahiMuafHaric(double damgaVergisiMatrahi)
            {
                double tut = damgaVergisiMatrahi - damgaVergisiMatrahMuaf;
                if (tut > 0) return tut;
                return 0;
            }

            public double gelirToplami { get { if (brutSozlesmeGostergesi > 0) return brutSozlesmeUcreti + ozelHizmetTazminati2; return gostergeMaas + ekGostergeMaas + tabanAyligi + kidemMaas + ozelHizmetTazminati + yanOdemeMaasi + ekOdemeMaasi; } }

            public double emekliKesenegiMatrahi { get { if (tur == 2) return 0; return gostergeMaas + ekGostergeMaas + tabanAyligi + kidemMaas + ozelHizmetTazminatiEmekli; } }
            public double emekliKesenegiSahis { get { return OrtakFonksiyonlar.KurusYap(emekliKesenegiMatrahi * 0.16); } }
            public double emekliKesenegiKurum { get { return OrtakFonksiyonlar.KurusYap(emekliKesenegiMatrahi * 0.2); } }
            public double gelirVergisiMatrahi { get { if (brutSozlesmeGostergesi > 0) return brutSozlesmeUcreti - emekliKesenegiSahis; return gostergeMaas + ekGostergeMaas + tabanAyligi + kidemMaas + yanOdemeMaasi - emekliKesenegiSahis; } }
            public double damgaVergisiMatrahi { get { return DamgaVergisiMatrahiMuafHaric(gelirToplami); } }

            public double gelirVergisi { get { return GelirVergisiMuafHaric(TNS.HRC.Arac.GelirVergisiHesapla(oncGelirVergisiMatrahi, gelirVergisiMatrahi, gelirVergisiOranlari)); } }

            public double damgaVergisi { get { return OrtakFonksiyonlar.KurusYap(damgaVergisiMatrahi * 0.00759); } }
            public double kesintilerToplami { get { return emekliKesenegiSahis + gelirVergisi + damgaVergisi; } }
            public double netMaas
            {
                get
                {
                    double nm = gelirToplami - kesintilerToplami;
                    if (ustLimit > -1 && nm > ustLimit) nm = ustLimit;
                    return nm;
                }
            }

            double oncGelirVergisiMatrahi;
            ObjectArray gelirVergisiOranlari;
            public MaasHesapSonucAlanlar(MaasHesapSonuc mhs, double oncGelirVergisiMatrahi, ObjectArray gelirVergisiOranlari)
            {
                this.oncGelirVergisiMatrahi = oncGelirVergisiMatrahi;
                this.gelirVergisiOranlari = gelirVergisiOranlari;
                this.unvanKod = mhs.unvanKod;
                this.unvanAd = mhs.unvanAd;
                this.yil = mhs.yil;
                this.ay = mhs.ay;
                this.tur = 1;
                this.gelirVergisiMuaf = mhs.gelirVergisiMuaf;
                this.damgaVergisiMatrahMuaf = mhs.damgaVergisiMatrahMuaf;

                this.maasKatsayisi = mhs.maasKatsayisi;
                this.tabanAylikKatsayisi = mhs.tabanAylikKatsayisi;
                this.yanOdemeKatsayisi = mhs.yanOdemeKatsayisi;
                this.brutSozlesmeGostergesi = mhs.brutSozlesmeGostergesi;
                this.ozelHizmetTazminatiGostergesi = mhs.ozelHizmetTazminatiGostergesi;
                this.ozelHizmetTazminatiOrani = mhs.ozelHizmetTazminatOrani;
                this.gosterge = mhs.gosterge;
                this.ekGosterge = mhs.ekGosterge;
                this.kidem = mhs.kidem;
                this.ekOdeme = mhs.ekOdeme;
                this.yanOdeme = mhs.yanOdeme;
                this.ozelHizmetTazminatOraniEmekli = mhs.ozelHizmetTazminatOraniEmekli;
                this.gostergeMaas = mhs.gostergeMaas;
                this.ekGostergeMaas = mhs.ekGostergeMaas;
                this.tabanAyligi = mhs.tabanAyligi;
                this.kidemMaas = mhs.kidemMaas;
                this.ozelHizmetTazminati = mhs.ozelHizmetTazminati;
                this.ozelHizmetTazminatiEmekli = mhs.ozelHizmetTazminatiEmekli;
                this.ozelHizmetTazminati2 = mhs.ozelHizmetTazminati2;
                this.yanOdemeMaasi = mhs.yanOdemeMaasi;
                this.ekOdemeMaasi = mhs.ekOdemeMaasi;
                this.brutSozlesmeUcreti = mhs.brutSozlesmeUcreti;
                this.ustLimit = mhs.ustLimit;
            }

            public MaasHesapSonucAlanlar(MaasHesapSonuc mhs, MaasHesapSonuc oncMhs, double oncGelirVergisiMatrahi, ObjectArray gelirVergisiOranlari)
            {
                this.oncGelirVergisiMatrahi = oncGelirVergisiMatrahi;
                this.gelirVergisiOranlari = gelirVergisiOranlari;
                this.unvanKod = mhs.unvanKod;
                this.unvanAd = mhs.unvanAd;
                this.yil = mhs.yil;
                this.ay = mhs.ay;
                this.tur = 2;
                this.gelirVergisiMuaf = 0;
                this.damgaVergisiMatrahMuaf = 0;

                this.maasKatsayisi = mhs.maasKatsayisi;
                this.tabanAylikKatsayisi = mhs.tabanAylikKatsayisi;
                this.yanOdemeKatsayisi = mhs.yanOdemeKatsayisi;
                this.brutSozlesmeGostergesi = mhs.brutSozlesmeGostergesi;
                this.ozelHizmetTazminatiOrani = mhs.ozelHizmetTazminatOrani;
                this.ozelHizmetTazminatiGostergesi = mhs.ozelHizmetTazminatiGostergesi;
                this.gosterge = mhs.gosterge;
                this.ekGosterge = mhs.ekGosterge;
                this.kidem = mhs.kidem;
                this.ekOdeme = mhs.ekOdeme;
                this.yanOdeme = mhs.yanOdeme;
                this.ozelHizmetTazminatOraniEmekli = mhs.ozelHizmetTazminatOraniEmekli;

                this.gostergeMaas = Fark(mhs.gostergeMaas, oncMhs.gostergeMaas);
                this.ekGostergeMaas = Fark(mhs.ekGostergeMaas, oncMhs.ekGostergeMaas);
                this.tabanAyligi = Fark(mhs.tabanAyligi, oncMhs.tabanAyligi);
                this.kidemMaas = Fark(mhs.kidemMaas, oncMhs.kidemMaas);
                this.ozelHizmetTazminati = Fark(mhs.ozelHizmetTazminati, oncMhs.ozelHizmetTazminati);
                this.ozelHizmetTazminatiEmekli = Fark(mhs.ozelHizmetTazminatiEmekli, oncMhs.ozelHizmetTazminatiEmekli);
                this.ozelHizmetTazminati2 = Fark(mhs.ozelHizmetTazminati2, oncMhs.ozelHizmetTazminati2);
                this.yanOdemeMaasi = Fark(mhs.yanOdemeMaasi, oncMhs.yanOdemeMaasi);
                this.ekOdemeMaasi = Fark(mhs.ekOdemeMaasi, oncMhs.ekOdemeMaasi);
                this.brutSozlesmeUcreti = Fark(mhs.brutSozlesmeUcreti, oncMhs.brutSozlesmeUcreti);
                this.ustLimit = -1;
            }

            private double Fark(double deger1, double deger2)
            {
                return OrtakFonksiyonlar.KurusYap(((deger1 - deger2) / (ay == 1 ? 31 : 30)) * 14);
            }
        }

        /// <summary>Eski BordroTabloForm.MaasHesapSonuc: ünvan, yıl, ay ve kıdeme göre gösterge / katsayılar (bordro tablolarından).</summary>
        public class MaasHesapSonuc
        {
            ObjectArray temelBilgiler;
            public int yil;
            public int ay;

            public MaasHesapSonuc(IHRCServis servis, Kullanici kullanan, ObjectArray temelBilgiler, int yil, int ay, int unvanKod, string unvanAd, int kidem)
            {
                this.yil = yil;
                this.ay = ay;
                this.unvanKod = unvanKod;
                this.unvanAd = unvanAd;
                this.temelBilgiler = temelBilgiler;
                this.kidem = kidem;

                int basDerece = Convert.ToInt32(servis_OkuBordroTablo(servis, kullanan, 16, unvanKod, 1, 0));
                int basKademe = Convert.ToInt32(servis_OkuBordroTablo(servis, kullanan, 16, unvanKod, 2, 0));
                int kistUnvan = Convert.ToInt32(servis_OkuBordroTablo(servis, kullanan, 16, unvanKod, 3, 0));
                this.ustLimit = -1;
                if (kistUnvan > 0)
                {
                    this.ustLimit = UstLimitBul(servis, kullanan, kistUnvan, yil, ay);
                }

                if (basDerece <= 0 || basKademe <= 0)
                {
                    basDerece = 15;
                    basKademe = 1;
                }

                int basYil = DereceKademedenYil(basDerece, basKademe);
                YildanDereceKademe(basYil + kidem, out derece, out kademe);

                PSLMaasTemelBilgisi mtb = null;
                foreach (PSLMaasTemelBilgisi item in temelBilgiler.objeler)
                {
                    if (item.ay == ay) mtb = item;
                }

                if (mtb != null)
                {
                    maasKatsayisi = Convert.ToDouble(mtb.aylikKatsayi);
                    tabanAylikKatsayisi = Convert.ToDouble(mtb.tabanAylikKatsayi);
                    yanOdemeKatsayisi = Convert.ToDouble(mtb.yanOdemeKatsayi);
                    if (yil >= 2022)
                    {
                        if (yil != 2022 || ay != 0)
                        {
                            gelirVergisiMuaf = OrtakFonksiyonlar.KurusYap(Convert.ToDouble(mtb.netAsgariUcret) * 0.15);
                            damgaVergisiMatrahMuaf = Convert.ToDouble(mtb.brutAsgariUcret);
                        }
                    }
                }
                else
                {
                    maasKatsayisi = 0;
                    tabanAylikKatsayisi = 0;
                    yanOdemeKatsayisi = 0;
                    damgaVergisiMatrahMuaf = 0;
                    gelirVergisiMuaf = 0;
                }

                this.gosterge = TNS.IKY.Arac.GostergeBul(derece, kademe);
                this.ekGosterge = Convert.ToUInt16(servis_OkuBordroTablo(servis, kullanan, 15, unvanKod, derece, 0));

                this.ekOdeme = Convert.ToUInt16(servis_OkuBordroTablo(servis, kullanan, 17, unvanKod, derece, 0));
                this.yanOdeme = Convert.ToUInt16(servis_OkuBordroTablo(servis, kullanan, 18, unvanKod, derece, 0));
                this.ozelHizmetTazminatOrani = Convert.ToUInt16(servis_OkuBordroTablo(servis, kullanan, 19, unvanKod, derece, 0));
                this.ozelHizmetTazminatOraniEmekli = Convert.ToUInt16(servis_OkuBordroTablo(servis, kullanan, 20, unvanKod, derece, 0));

                brutSozlesmeGostergesi = servis_OkuBordroTablo(servis, kullanan, 13, unvanKod, derece, yil);
                ozelHizmetTazminatiGostergesi = servis_OkuBordroTablo(servis, kullanan, 14, unvanKod, derece, yil);
            }

            private double UstLimitBul(IHRCServis servis, Kullanici kullanan, int kistUnvan, int yil, int ay)
            {
                double minVal = -1;
                for (int i = 0; i <= 25; i++)
                {
                    double simdVal = servis_OkuBordroTablo(servis, kullanan, 100 + i, kistUnvan, ay, yil);
                    if (minVal == -1 || minVal > simdVal) minVal = simdVal;
                }
                return minVal;
            }

            private double servis_OkuBordroTablo(IHRCServis servis, Kullanici kullanan, int tabloKod, int unvanKod, int derece, int yil)
            {
                return TNS.HRC.Arac.TabloDegerYil(servis, kullanan, tabloKod, unvanKod, derece, yil);
            }

            private void YildanDereceKademe(int yil, out int derece, out int kademe)
            {
                if (yil > 44)
                {
                    derece = 1;
                    kademe = 4;
                }
                else if (yil < 0)
                {
                    derece = 15;
                    kademe = 1;
                }
                else
                {
                    int m = yil % 3;
                    derece = 15 - ((yil - m) / 3);
                    kademe = m + 1;
                }
            }

            private int DereceKademedenYil(int basDerece, int basKademe)
            {
                return (15 - basDerece) * 3 + (basKademe - 1);
            }

            public int unvanKod;
            public string unvanAd;
            public double maasKatsayisi;
            public double tabanAylikKatsayisi;
            public double yanOdemeKatsayisi;
            public int kidem;
            public int derece;
            public int kademe;
            public double brutSozlesmeGostergesi = 0;
            public double ozelHizmetTazminatiGostergesi = 0;

            public int gosterge;
            public int ekGosterge;
            public int ekOdeme;
            public int yanOdeme;
            public int ozelHizmetTazminatOraniEmekli;
            public int ozelHizmetTazminatOrani;
            internal double damgaVergisiMatrahMuaf;
            internal double gelirVergisiMuaf;
            public double ustLimit;

            public double gostergeMaas { get { return OrtakFonksiyonlar.KurusYap(maasKatsayisi * gosterge); } }
            public double ekGostergeMaas { get { return OrtakFonksiyonlar.KurusYap(maasKatsayisi * ekGosterge); } }
            public double tabanAyligi { get { return OrtakFonksiyonlar.KurusYap(tabanAylikKatsayisi * 1000); } }
            public double kidemMaas { get { return OrtakFonksiyonlar.KurusYap(maasKatsayisi * kidem * 20); } }
            public double ozelHizmetTazminati { get { return OrtakFonksiyonlar.KurusYap(maasKatsayisi * ozelHizmetTazminatOrani * 95); } }
            public double ozelHizmetTazminatiEmekli { get { return OrtakFonksiyonlar.KurusYap(maasKatsayisi * ozelHizmetTazminatOraniEmekli * 95); } }
            public double ozelHizmetTazminati2 { get { return OrtakFonksiyonlar.KurusYap(maasKatsayisi * ozelHizmetTazminatiGostergesi); } }
            public double yanOdemeMaasi { get { return OrtakFonksiyonlar.KurusYap(yanOdemeKatsayisi * yanOdeme); } }
            public double ekOdemeMaasi { get { return OrtakFonksiyonlar.KurusYap(maasKatsayisi * ekOdeme * 95); } }
            public double brutSozlesmeUcreti { get { return OrtakFonksiyonlar.KurusYap(maasKatsayisi * brutSozlesmeGostergesi); } }
        }
    }
}
