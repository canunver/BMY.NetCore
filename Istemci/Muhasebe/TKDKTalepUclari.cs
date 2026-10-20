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
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>Talep formu satırı (eski DetayStore1 / DetayStore2: HESAPKOD, HESAPADI, TUTAR11..14 nakit, TUTAR21/22 aktarma).</summary>
    public class TKDKTalepSatirIstegi
    {
        public string HESAPKOD { get; set; }
        public string HESAPADI { get; set; }
        public double? TUTAR11 { get; set; }
        public double? TUTAR12 { get; set; }
        public double? TUTAR13 { get; set; }
        public double? TUTAR14 { get; set; }
        public double? TUTAR21 { get; set; }
        public double? TUTAR22 { get; set; }
    }

    /// <summary>Talep formu (eski TKDKTalepGiris.aspx btnKaydet_Click / btnSil_Click).</summary>
    public class TKDKTalepFormIstegi
    {
        public string kod { get; set; }
        public string muhasebe { get; set; }
        public string birim { get; set; }
        public int tur { get; set; }
        public int yil { get; set; }
        public int ay { get; set; }
        public double? mevcutBakiye { get; set; }
        public string gerekce { get; set; }
        public List<TKDKTalepSatirIstegi> satirlar { get; set; }
    }

    /// <summary>Seçili taleplerin durumu (eski btnOnayla_Click / btnReddet_Click: SATIRLAR).</summary>
    public class TKDKTalepDurumIstegi
    {
        public List<string> kodlar { get; set; }
    }

    /// <summary>İmza bilgileri (eski wndImzaBilgileri).</summary>
    public class TKDKTalepImzaIstegi
    {
        public string duzenleyenAdi { get; set; }
        public string duzenleyenGorev { get; set; }
        public string onaylayanAdi { get; set; }
        public string onaylayanGorev { get; set; }
    }

    public class TKDKTalepTertipSatir
    {
        public string KOD { get; set; }
        public string TERTIP { get; set; }
    }

    /// <summary>Ödeme emri oluşturma (eski btnOdemeEmriNakit_Click: hafta; btnOdemeEmriAktarma_Click: grdTertipler).</summary>
    public class TKDKTalepOdemeEmriIstegi
    {
        public int yil { get; set; }
        public int ay { get; set; }
        public int hafta { get; set; }
        public List<TKDKTalepTertipSatir> tertipler { get; set; }
    }

    /// <summary>
    /// İl Koordinatörlüğü Ödenek Talep İşlemleri (eski ButceMuhasebe/TKDKTalepListesi.aspx) ve içinden pencere olarak açılan
    /// Talep Giriş (TKDKTalepGiris.aspx) uçları. Eski sayfaların arka plan kodu buraya taşındı; iş kuralları (IMUHServis.TkdkTalep*,
    /// TalepTanimListele, OdemeEmriMIFKaydet) değişmeden çağrılır.
    ///   Liste : GET  /api/tkdktalep/tanim, liste, liste/yazdir (MUH/TKDKTalepListesi.xltx), rapor (MUH-TKDKTalepFormRapolarOlurEki.xltx),
    ///                banka-listesi?hafta (MUH-TKDKTalepZiraat.xlsm), imza, tertip
    ///           POST /api/tkdktalep/onayla, reddet, imza, odeme-emri/nakit, odeme-emri/aktarma
    ///   Form  : GET  /api/tkdktalep/form/tanim?kod, form/tanim-satir?tur&amp;yil&amp;ay, form/yazdir?kod (MUH-TKDKTalepFormNormal.xltx)
    ///           POST /api/tkdktalep/form/kaydet, form/sil
    /// Yetki: menüdeki Talep Listesi + ödeme emri görme yetkisi (eski Page_Load). Talep Giriş menüde yok, yetkisi Talep Listesi'nden.
    /// Onayla / Reddet / Rapor ve Olur Eki eski sayfada yalnız sistem yöneticisine görünürdü; uçlar da yalnız ona açıktır.
    /// </summary>
    public static class TKDKTalepUclari
    {
        public const string SayfaAdresi = "../ButceMuhasebe/TKDKTalepListesi.aspx";
        public const string FormAdresi = "../ButceMuhasebe/TKDKTalepGiris.aspx";
        private const string Uc = "/api/tkdktalep/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "liste", Liste);
            app.MapGet(Uc + "liste/yazdir", ListeYazdir);
            app.MapGet(Uc + "rapor", Rapor);
            app.MapGet(Uc + "banka-listesi", BankaListesi);
            app.MapGet(Uc + "imza", ImzaOku);
            app.MapPost(Uc + "imza", ImzaKaydet);
            app.MapGet(Uc + "tertip", TertipListe);
            app.MapPost(Uc + "onayla", Onayla);
            app.MapPost(Uc + "reddet", Reddet);
            app.MapPost(Uc + "odeme-emri/nakit", OdemeEmriNakit);
            app.MapPost(Uc + "odeme-emri/aktarma", OdemeEmriAktarma);

            app.MapGet(Uc + "form/tanim", FormTanim);
            app.MapGet(Uc + "form/tanim-satir", FormTanimSatir);
            app.MapPost(Uc + "form/kaydet", FormKaydet);
            app.MapPost(Uc + "form/sil", FormSil);
            app.MapGet(Uc + "form/yazdir", FormYazdir);
        }

        // ------------------------------------------------------------------ yetki ve ortak

        /// <summary>Eski Page_Load: ödeme emri görme yetkisi olmayan giremez; ayrıca menü yetkisi (alt sayfa için üst sayfanınki).</summary>
        public static bool YetkiVar(Kullanici kullanan, string klasor, string altSayfa = null)
        {
            if (kullanan == null) return false;
            bool oeYetki;
            try { oeYetki = TNS.MUH.Yetki.YetkisiVarMi(kullanan, (int)TNS.MUH.ENUMOGY.Gorme, (int)ENUMBelgeTuru.ODEMEEMRI); }
            catch (Exception) { oeYetki = false; }
            if (!oeYetki) return false;
            return altSayfa == null ? SayfaYetki.Yetkili(kullanan, klasor, SayfaAdresi)
                                    : SayfaYetki.Yetkili(kullanan, klasor, altSayfa, SayfaAdresi);
        }

        /// <summary>Eski sayfada Onayla, Reddet, Rapor ve Olur Eki yalnız sistem yöneticisine görünürdü.</summary>
        public static bool Yonetici(Kullanici kullanan)
        {
            try { return kullanan != null && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI); }
            catch (Exception) { return false; }
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan, bool yonetici = false)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (yonetici && !Yonetici(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static async Task<IResult> DenetleYaz(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, bool yonetici, Func<Kullanici, IResult> is_)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan, yonetici);
            if (ret != null) return ret;
            try { return is_(kullanan); }
            catch (Exception ex) { return Hata(HataMetni(ex.Message, "İşlem yapılamadı.")); }
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        private static IResult Hata(string hata) { return Results.Json(new { hata = Duz(hata) }); }

        private static string Duz(string s)
        {
            return (s ?? "").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n")
                .Replace("<ul>", "\n").Replace("</ul>", "").Replace("<li>", "\n- ").Replace("</li>", "").Trim();
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("   at ")) return varsayilan;
            return Duz(hata);
        }

        private static string Q(HttpContext baglam, string ad)
        {
            return ((string)baglam.Request.Query[ad] ?? "").Trim();
        }

        private static int QSayi(HttpContext baglam, string ad)
        {
            return OrtakFonksiyonlar.ConvertToInt(Q(baglam, ad), 0);
        }

        private static string Degisken(Kullanici kullanan, string ad)
        {
            try { return GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, ad) ?? ""; }
            catch (Exception) { return ""; }
        }

        private static string TalepTurAd(int tur)
        {
            return tur == 1 ? "Nakit" : tur == 2 ? "Ödenek Aktarma" : tur == 3 ? "Ek Ödenek" : "";
        }

        private static string DurumAd(int durum)
        {
            try { return ButceMuhasebeResI.ResourceManager.GetString("TKDKTALEPDURUM_" + durum.ToString("d2")) ?? ""; }
            catch (Exception) { return ""; }
        }

        private static string SablonYol(string sablonAd)
        {
            string kurum = Path.Combine(Ayar.Oku("RaporSablonYol") ?? "", "MUH", sablonAd);
            if (File.Exists(kurum)) return kurum;
            foreach (string kok in new string[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
            {
                string uygulama = Path.Combine(kok, "RaporSablon", "MUH", sablonAd);
                if (File.Exists(uygulama)) return uygulama;
            }
            return kurum;
        }

        /// <summary>Şablonu açar; bulunamazsa null.</summary>
        private static Tablo SablonAc(string sablonAd, string sonucDosyaAd)
        {
            string sablon = SablonYol(sablonAd);
            if (!File.Exists(sablon)) return null;
            AsposeLisans.Yukle();
            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablon, sonucDosyaAd);
            return XLS;
        }

        // ------------------------------------------------------------------ liste açılışı (eski Page_Load)

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            bool yonetici = Yonetici(kullanan);
            // Eski Listele(sayfaAcilis): yönetici olmayan kullanıcının kıstasına kendi muhasebe / birimi yazılır.
            return Results.Json(new
            {
                yil = DateTime.Now.Year,
                ay = DateTime.Now.Month,
                yonetici,
                muhasebe = yonetici ? "" : Degisken(kullanan, "MUHASEBE"),
                birim = yonetici ? "" : Degisken(kullanan, "MUHASEBEBIRIM")
            });
        }

        // ------------------------------------------------------------------ listele (eski Listele)

        private static TkdkTalepForm Kriter(HttpContext baglam)
        {
            TkdkTalepForm kriter = new TkdkTalepForm();
            kriter.muhasebeKod = QSayi(baglam, "muhasebe");
            kriter.birimKod = Q(baglam, "birim");
            kriter.yil = QSayi(baglam, "yil");
            kriter.ay = QSayi(baglam, "ay");
            kriter.tur = QSayi(baglam, "talepTur");
            kriter.durum = QSayi(baglam, "durum");
            kriter.detayGoster = true;
            return kriter;
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray bilgiler = Servis().TkdkTalepListele(kullanan, Kriter(baglam));
                if (bilgiler == null || !bilgiler.sonuc.islemSonuc) return Hata(HataMetni(bilgiler?.sonuc.hataStr, "Liste okunamadı."));

                List<object> liste = new List<object>();
                foreach (TkdkTalepForm item in bilgiler.objeler)
                {
                    double toplamTutar = 0.0;
                    double tutar1 = 0.0;
                    double tutar2 = 0.0;
                    double tutar3 = 0.0;
                    double tutar4 = 0.0;

                    foreach (TkdkTalepDetay ditem in item.satirlar.objeler)
                    {
                        if (item.tur == 1)
                        {
                            toplamTutar += ditem.tutar + ditem.tutar1 + ditem.tutar2 + ditem.tutar3;
                            tutar1 += ditem.tutar;
                            tutar2 += ditem.tutar1;
                            tutar3 += ditem.tutar2;
                            tutar4 += ditem.tutar3;
                        }
                        else
                            toplamTutar += ditem.tutar;
                    }

                    DateTime islemTarihi = TNSDateTime.IsNull(item.kayitTarihi) ? DateTime.MinValue : item.kayitTarihi.Oku();
                    liste.Add(new
                    {
                        KOD = item.kod,
                        YIL = item.yil,
                        AY = OrtakFonksiyonlar.AyAdiGetir(item.ay),
                        MUHASEBEKOD = item.muhasebeKod,
                        BIRIMKOD = item.birimKod,
                        TUR = TalepTurAd(item.tur),
                        TUTAR = toplamTutar,
                        DURUM = DurumAd(item.durum),
                        ISLEMYAPAN = item.kayitEden,
                        ISLEMTARIHI = islemTarihi == DateTime.MinValue ? "" : islemTarihi.ToString("yyyy-MM-dd"),
                        ODEMEEMRINO1 = item.odemeEmriNo1,
                        ODEMEEMRINO2 = item.odemeEmriNo2,
                        ODEMEEMRINO3 = item.odemeEmriNo3,
                        ODEMEEMRINO4 = item.odemeEmriNo4,
                        TUTAR1 = tutar1,
                        TUTAR2 = tutar2,
                        TUTAR3 = tutar3,
                        TUTAR4 = tutar4
                    });
                }
                return Results.Json(new { satirlar = liste });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Liste okunamadı."));
            }
        }

        // ------------------------------------------------------------------ onayla / reddet (eski btnOnayla_Click / btnReddet_Click)

        private static IResult DurumDegistir(Kullanici kullanan, TKDKTalepDurumIstegi istek, ENUMTKDKTalepDurum durum)
        {
            ObjectArray bilgiler = new ObjectArray();
            foreach (string kod in istek?.kodlar ?? new List<string>())
            {
                TkdkTalepForm bilgi = new TkdkTalepForm();
                bilgi.kod = kod ?? "";
                bilgi.durum = (int)durum;
                bilgiler.Ekle(bilgi);
            }

            Sonuc sonuc = Servis().TkdkTalepGuncelleDurum(kullanan, bilgiler);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc.hataStr, "Durum güncellenemedi."));
            return Results.Json(new { basarili = true, bilgi = Duz(sonuc.bilgiStr) });
        }

        private static Task<IResult> Onayla(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TKDKTalepDurumIstegi istek)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, true, kullanan => DurumDegistir(kullanan, istek, ENUMTKDKTalepDurum.TAMAMLANDI));
        }

        private static Task<IResult> Reddet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TKDKTalepDurumIstegi istek)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, true, kullanan => DurumDegistir(kullanan, istek, ENUMTKDKTalepDurum.REDDEDILDI));
        }

        // ------------------------------------------------------------------ imza bilgileri (eski btnImzaBilgileri_Getir / btnImzaKaydet_Click)

        private static IResult ImzaOku(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return Results.Json(new
            {
                duzenleyenAdi = Degisken(kullanan, "TKDKTALEPDUZENLEYENADSOYAD"),
                duzenleyenGorev = Degisken(kullanan, "TKDKTALEPDUZENLEYENAGOREV"),
                onaylayanAdi = Degisken(kullanan, "TKDKTALEPONAYLAYANADSOYAD"),
                onaylayanGorev = Degisken(kullanan, "TKDKTALEPONAYLAYANADGOREV")
            });
        }

        private static Task<IResult> ImzaKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TKDKTalepImzaIstegi istek)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, false, kullanan =>
            {
                if (istek == null) return Results.BadRequest();
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "TKDKTALEPDUZENLEYENADSOYAD", istek.duzenleyenAdi ?? "");
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "TKDKTALEPDUZENLEYENAGOREV", istek.duzenleyenGorev ?? "");
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "TKDKTALEPONAYLAYANADSOYAD", istek.onaylayanAdi ?? "");
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "TKDKTALEPONAYLAYANADGOREV", istek.onaylayanGorev ?? "");
                return Results.Json(new { basarili = true, bilgi = "İmza bilgileri kaydedildi." });
            });
        }

        // ------------------------------------------------------------------ listeyi yazdır (eski btnListeYazdir_Click)

        private static IResult ListeYazdir(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray bilgiler = Servis().TkdkTalepListele(kullanan, Kriter(baglam));
                if (bilgiler == null || !bilgiler.sonuc.islemSonuc) return Hata(HataMetni(bilgiler?.sonuc.hataStr, "Liste okunamadı."));

                string sonucDosyaAd = DosyaIslem.DosyaAdUret("TalepListesi", "") + ".xlsx";
                Tablo XLS = SablonAc("TKDKTalepListesi.xltx", sonucDosyaAd);
                if (XLS == null) return Hata("Şablon (MUH/TKDKTalepListesi.xltx) bulunamadı.");

                int yazSatir = 0;
                int sutun = 0;
                XLS.HucreAdAdresCoz("baslaSatir", ref yazSatir, ref sutun);

                foreach (TkdkTalepForm item in bilgiler.objeler)
                {
                    string durumTipiAd = DurumAd(item.durum);
                    string talepTur = "Nakit";
                    if (item.tur == 2)
                        talepTur = "Ödenek Aktarma";
                    if (item.tur == 3)
                        talepTur = "Ek Ödenek";

                    double tutar = 0.0;
                    double tutar1 = 0.0;
                    double tutar2 = 0.0;
                    double tutar3 = 0.0;
                    double tutar4 = 0.0;

                    foreach (TkdkTalepDetay ditem in item.satirlar.objeler)
                    {
                        if (item.tur == 1)
                        {
                            tutar += ditem.tutar + ditem.tutar1 + ditem.tutar2 + ditem.tutar3;
                            tutar1 += ditem.tutar;
                            tutar2 += ditem.tutar1;
                            tutar3 += ditem.tutar2;
                            tutar4 += ditem.tutar3;
                        }
                        else
                            tutar += ditem.tutar;
                    }

                    XLS.HucreKopyala(0, sutun, 0, sutun + 16, yazSatir, sutun);
                    XLS.HucreDegerYaz(yazSatir, 0, item.yil.ToString());
                    XLS.HucreDegerYaz(yazSatir, 1, OrtakFonksiyonlar.AyAdiGetir(item.ay));
                    XLS.HucreDegerYaz(yazSatir, 2, item.muhasebeKod);
                    XLS.HucreDegerYaz(yazSatir, 3, item.birimKod);
                    XLS.HucreDegerYaz(yazSatir, 4, talepTur);
                    XLS.HucreDegerYaz(yazSatir, 5, tutar);
                    XLS.HucreDegerYaz(yazSatir, 6, durumTipiAd);
                    XLS.HucreDegerYaz(yazSatir, 7, item.kayitEdenAdi);
                    XLS.HucreDegerYaz(yazSatir, 8, TNSDateTime.IsNull(item.kayitTarihi) ? "" : item.kayitTarihi.Oku().ToShortDateString());
                    XLS.HucreDegerYaz(yazSatir, 9, item.odemeEmriNo1);
                    XLS.HucreDegerYaz(yazSatir, 10, item.odemeEmriNo2);
                    XLS.HucreDegerYaz(yazSatir, 11, item.odemeEmriNo3);
                    XLS.HucreDegerYaz(yazSatir, 12, item.odemeEmriNo4);
                    XLS.HucreDegerYaz(yazSatir, 13, tutar1);
                    XLS.HucreDegerYaz(yazSatir, 14, tutar2);
                    XLS.HucreDegerYaz(yazSatir, 15, tutar3);
                    XLS.HucreDegerYaz(yazSatir, 16, tutar4);

                    yazSatir++;
                }

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "TalepListesi", true, "xlsx");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        // ------------------------------------------------------------------ nakit talep tablosu (eski SutunBilgi / SatirBilgi / ListeHazirla)

        private class SutunBilgi
        {
            public string sutunAdi = "";
            public string sutunKodu = "";
            public double tutar = 0.0;
            public int kolonNo = 0;
        }

        private class SatirBilgi
        {
            public string birimKodu = "";
            public string birimAdi = "";
            public string IBAN = "";
            public string vergiNo = "";
            public string vergiAdi = "";
            public string odemeEmriNo1 = "";
            public string odemeEmriNo2 = "";
            public string odemeEmriNo3 = "";
            public string odemeEmriNo4 = "";
            public double mevcutBakiye = 0.0;
            public double[] tutar = new double[100];
            public double[] tutar1 = new double[100];
            public double[] tutar2 = new double[100];
            public double[] tutar3 = new double[100];
            public double[] tutar4 = new double[100];
        }

        private static void ListeHazirla(Kullanici kullanan, int yil, int ay, List<SatirBilgi> satirListe, List<SutunBilgi> sutunListe, ref int kolonSutun)
        {
            IMUHServis servisMUH = Servis();

            // Tanım tablosundan bilgileri al ve sütun nesnesini doldur, sütun başlıklarını oluştur
            TalepTanimForm tanimKriter = new TalepTanimForm();
            tanimKriter.talepTur = 1;//Nakit
            tanimKriter.yil = yil;
            tanimKriter.ay = ay;
            tanimKriter.detayGoster = true;
            ObjectArray tanimListe = servisMUH.TalepTanimListele(kullanan, tanimKriter);
            foreach (TalepTanimForm item in tanimListe.objeler)
            {
                foreach (TalepTanimFormDetay ditem in item.satirlar.objeler)
                {
                    sutunListe.Add(new SutunBilgi { sutunAdi = ditem.ekoAdi, sutunKodu = ditem.ekoKod });
                }
            }
            sutunListe.Add(new SutunBilgi { sutunAdi = "Toplam", sutunKodu = "TOPLAM" });
            sutunListe.Add(new SutunBilgi { sutunAdi = "Mevcut Bakiye", sutunKodu = "MEVCUT" });
            sutunListe.Add(new SutunBilgi { sutunAdi = "Gönderilecek Tutar", sutunKodu = "GONDERILECEK" });

            int toplamKolon = 0;
            int mevcutKolon = 0;
            int gonderilecekKolon = 0;
            foreach (SutunBilgi item in sutunListe)
            {
                item.kolonNo = kolonSutun;

                if (item.sutunKodu == "TOPLAM") toplamKolon = kolonSutun;
                else if (item.sutunKodu == "MEVCUT") mevcutKolon = kolonSutun;
                else if (item.sutunKodu == "GONDERILECEK") gonderilecekKolon = kolonSutun;

                kolonSutun++;
            }

            // Talep tablosundan bilgileri al ve satır nesnesini doldur
            TkdkTalepForm kriter = new TkdkTalepForm();
            kriter.yil = yil;
            kriter.ay = ay;
            kriter.tur = 1;//Nakit
            kriter.durum = (int)ENUMTKDKTalepDurum.TAMAMLANDI;
            kriter.detayGoster = true;
            ObjectArray bilgiler = servisMUH.TkdkTalepListele(kullanan, kriter);
            foreach (TkdkTalepForm item in bilgiler.objeler)
            {
                SatirBilgi sab = new SatirBilgi();
                sab.birimAdi = item.birimAdi;
                sab.IBAN = item.birimIban;
                sab.vergiNo = item.birimVergiNo;
                sab.vergiAdi = item.birimVergiAdi;
                sab.odemeEmriNo1 = item.odemeEmriNo1;
                sab.odemeEmriNo2 = item.odemeEmriNo2;
                sab.odemeEmriNo3 = item.odemeEmriNo3;
                sab.odemeEmriNo4 = item.odemeEmriNo4;
                sab.mevcutBakiye = item.mevcutBakiye;
                sab.birimKodu = item.birimKod;

                foreach (TkdkTalepDetay ditem in item.satirlar.objeler)
                {
                    foreach (SutunBilgi sitem in sutunListe)
                    {
                        if (sitem.sutunKodu == ditem.ekoKod)
                        {
                            sab.tutar[sitem.kolonNo] = ditem.tutar + ditem.tutar1 + ditem.tutar2 + ditem.tutar3;
                            sab.tutar1[sitem.kolonNo] = ditem.tutar;
                            sab.tutar2[sitem.kolonNo] = ditem.tutar1;
                            sab.tutar3[sitem.kolonNo] = ditem.tutar2;
                            sab.tutar4[sitem.kolonNo] = ditem.tutar3;
                        }
                    }
                }

                satirListe.Add(sab);
            }
            satirListe.Add(new SatirBilgi { birimAdi = "TOPLAM" });

            // Toplamları hesapla
            SatirBilgi toplamSatiri = satirListe.Find(x => x.birimAdi.Contains("TOPLAM"));

            foreach (SatirBilgi item in satirListe)
            {
                if (item.birimAdi == "TOPLAM") continue;//Toplam satırı ise işlem yapma

                double satirToplam = 0;
                double satirToplam1 = 0;
                double satirToplam2 = 0;
                double satirToplam3 = 0;
                double satirToplam4 = 0;
                //Yan toplamı hesapla
                foreach (SutunBilgi sitem in sutunListe)
                {
                    if (!(sitem.sutunKodu == "TOPLAM" ||
                        sitem.sutunKodu == "MEVCUT" ||
                        sitem.sutunKodu == "GONDERILECEK"))
                    {
                        satirToplam += item.tutar[sitem.kolonNo];
                        satirToplam1 += item.tutar1[sitem.kolonNo];
                        satirToplam2 += item.tutar2[sitem.kolonNo];
                        satirToplam3 += item.tutar3[sitem.kolonNo];
                        satirToplam4 += item.tutar4[sitem.kolonNo];
                    }
                }

                item.tutar[toplamKolon] = satirToplam;
                item.tutar1[toplamKolon] = satirToplam1;
                item.tutar2[toplamKolon] = satirToplam2;
                item.tutar3[toplamKolon] = satirToplam3;
                item.tutar4[toplamKolon] = satirToplam4;
                item.tutar[mevcutKolon] = item.mevcutBakiye;//Banka tutarı buraya gelecek
                item.tutar[gonderilecekKolon] = item.tutar[toplamKolon];// - item.tutar[mevcutKolon];
                if (item.tutar[gonderilecekKolon] < 0) item.tutar[gonderilecekKolon] = 0;

                //Alt toplamı hesapla
                foreach (SutunBilgi sitem in sutunListe)
                {
                    toplamSatiri.tutar[sitem.kolonNo] += item.tutar[sitem.kolonNo];
                    toplamSatiri.tutar1[sitem.kolonNo] += item.tutar1[sitem.kolonNo];
                    toplamSatiri.tutar2[sitem.kolonNo] += item.tutar2[sitem.kolonNo];
                    toplamSatiri.tutar3[sitem.kolonNo] += item.tutar3[sitem.kolonNo];
                    toplamSatiri.tutar4[sitem.kolonNo] += item.tutar4[sitem.kolonNo];
                }
            }
        }

        /// <summary>Birimin o haftadaki toplam tutarı (eski btnOdemeEmriNakit_Click / BankaListesi'ndeki döngü).</summary>
        private static double HaftaTutar(SatirBilgi item, List<SutunBilgi> sutunListe, int hafta)
        {
            double birimTutar = 0;
            foreach (SutunBilgi sitem in sutunListe)
            {
                if (sitem.sutunKodu == "TOPLAM")
                {
                    if (hafta == 1)
                        birimTutar += item.tutar1[sitem.kolonNo];
                    else if (hafta == 2)
                        birimTutar += item.tutar2[sitem.kolonNo];
                    else if (hafta == 3)
                        birimTutar += item.tutar3[sitem.kolonNo];
                    else if (hafta == 4)
                        birimTutar += item.tutar4[sitem.kolonNo];
                }
            }
            return birimTutar;
        }

        // ------------------------------------------------------------------ Rapor ve Olur Eki (eski btnYazdir_Click)

        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan, true);
            if (ret != null) return ret;
            int yil = QSayi(baglam, "yil");
            int ay = QSayi(baglam, "ay");
            if (yil == 0 || ay == 0)
                return Hata("Yıl veya ay bilgisi boş bırakılamaz. Lütfen ilgili yıl ve ay bilgisini seçiniz");
            try
            {
                string sonucDosyaAd = DosyaIslem.DosyaAdUret("TalepRapolarOlurEki", "") + ".xlsx";
                Tablo XLS = SablonAc("MUH-TKDKTalepFormRapolarOlurEki.xltx", sonucDosyaAd);
                if (XLS == null) return Hata("Şablon (MUH/MUH-TKDKTalepFormRapolarOlurEki.xltx) bulunamadı.");

                int kaynakSatir = 0;
                int yazSatir = 0;
                int sutun = 0;
                int kolonSatir = 0;
                int kolonSutun = 0;

                XLS.HucreAdAdresCoz("baslaSatir", ref kaynakSatir, ref sutun);
                XLS.HucreAdAdresCoz("sutunBaslaSatir", ref kolonSatir, ref kolonSutun);

                yazSatir = kaynakSatir;

                List<SatirBilgi> satirListe = new List<SatirBilgi>();
                List<SutunBilgi> sutunListe = new List<SutunBilgi>();

                ListeHazirla(kullanan, yil, ay, satirListe, sutunListe, ref kolonSutun);

                // Kolon başlıklarını oluştur
                foreach (SutunBilgi item in sutunListe)
                {
                    XLS.HucreKopyala(kolonSatir, 0, kolonSatir, 0, kolonSatir, item.kolonNo);//kolonSatir ın ilk sutundakini kopyala
                    XLS.HucreDegerYaz(kolonSatir, item.kolonNo, item.sutunAdi);
                }
                XLS.HucreBirlestir(0, 0, 0, kolonSutun - 1);//Başlık hucresini birleştir
                XLS.SutunGenislikAyarla(1, kolonSutun - 1, 16);

                // Ayrıntılı listeyi yazdır
                foreach (SatirBilgi item in satirListe)
                {
                    XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 0, yazSatir, 0);
                    XLS.HucreDegerYaz(yazSatir, 0, item.birimAdi);

                    foreach (SutunBilgi sitem in sutunListe)
                    {
                        XLS.HucreKopyala(kaynakSatir, 1, kaynakSatir, 1, yazSatir, sitem.kolonNo);
                        XLS.HucreDegerYaz(yazSatir, sitem.kolonNo, item.tutar[sitem.kolonNo]);
                    }

                    yazSatir++;
                }
                XLS.KoyuYap(yazSatir - 1, 0, yazSatir - 1, kolonSutun - 1, true);
                XLS.ArkaPlanRenk(yazSatir - 1, 0, yazSatir - 1, kolonSutun - 1, System.Drawing.Color.FromArgb(242, 242, 242));
                XLS.SatirYukseklikAyarla(yazSatir - 1, yazSatir - 1, 19);

                // Sade listeyi yazdır
                XLS.AktifSheetDegistir(1);
                XLS.HucreAdAdresCoz("baslaSatir2", ref kaynakSatir, ref sutun);
                yazSatir = kaynakSatir;

                foreach (SatirBilgi item in satirListe)
                {
                    XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 3, yazSatir, 0);
                    XLS.HucreDegerYaz(yazSatir, 0, item.birimAdi);

                    foreach (SutunBilgi sitem in sutunListe)
                    {
                        if (sitem.sutunKodu == "TOPLAM")
                            XLS.HucreDegerYaz(yazSatir, 1, item.tutar[sitem.kolonNo]);
                        if (sitem.sutunKodu == "MEVCUT")
                            XLS.HucreDegerYaz(yazSatir, 2, item.tutar[sitem.kolonNo]);
                        if (sitem.sutunKodu == "GONDERILECEK")
                            XLS.HucreDegerYaz(yazSatir, 3, item.tutar[sitem.kolonNo]);
                    }

                    yazSatir++;
                }
                XLS.KoyuYap(yazSatir - 1, 0, yazSatir - 1, 3, true);
                XLS.ArkaPlanRenk(yazSatir - 1, 0, yazSatir - 1, 3, System.Drawing.Color.FromArgb(242, 242, 242));
                XLS.SatirYukseklikAyarla(yazSatir - 1, yazSatir - 1, 19);

                // Olur Ekini yazdır
                XLS.AktifSheetDegistir(2);
                XLS.HucreAdAdresCoz("baslaSatir3", ref kaynakSatir, ref sutun);
                yazSatir = kaynakSatir;
                int siraNo = 1;
                foreach (SatirBilgi item in satirListe)
                {
                    XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 3, yazSatir, 0);
                    XLS.HucreDegerYaz(yazSatir, 0, siraNo);
                    XLS.HucreDegerYaz(yazSatir, 1, item.birimAdi);
                    XLS.HucreDegerYaz(yazSatir, 2, item.IBAN);

                    foreach (SutunBilgi sitem in sutunListe)
                    {
                        if (sitem.sutunKodu == "GONDERILECEK")
                            XLS.HucreDegerYaz(yazSatir, 3, OrtakFonksiyonlar.YuvarlaYukari(item.tutar[sitem.kolonNo], 0));
                    }
                    siraNo++;
                    yazSatir++;
                }
                XLS.HucreDegerYaz(yazSatir - 1, 0, "");//Toplam satırındaki sırano bilgisini sil
                XLS.KoyuYap(yazSatir - 1, 0, yazSatir - 1, 3, true);
                XLS.ArkaPlanRenk(yazSatir - 1, 0, yazSatir - 1, 3, System.Drawing.Color.FromArgb(242, 242, 242));
                XLS.SatirYukseklikAyarla(yazSatir - 1, yazSatir - 1, 19);

                //Başlık için yıl ay yaz
                XLS.HucreAdBulYaz("ayYil", yil + " " + OrtakFonksiyonlar.AyAdiGetir(ay).ToUpper());

                TkdkTalepForm kriter = new TkdkTalepForm();
                kriter.yil = yil;
                kriter.ay = ay;
                kriter.durum = (int)ENUMTKDKTalepDurum.TAMAMLANDI;
                kriter.detayGoster = true;
                ObjectArray bilgiler = Servis().TkdkTalepListele(kullanan, kriter);

                // Ödenek Aktarma yazdır
                XLS.AktifSheetDegistir(3);
                XLS.HucreAdBulYaz("baslikOdenekAktarma", kriter.yil + " " + OrtakFonksiyonlar.AyAdiGetir(kriter.ay).ToUpper() + " AYI ÖDENEK AKTARMA RAPORU");

                XLS.HucreAdAdresCoz("baslaSatir4", ref kaynakSatir, ref sutun);
                yazSatir = kaynakSatir;

                siraNo = 1;

                double dusulecekToplam = 0.0;
                double artirilacakToplam = 0.0;

                foreach (TkdkTalepForm item in bilgiler.objeler)
                {
                    if (item.tur == 1 || item.tur == 3) continue;

                    foreach (TkdkTalepDetay ditem in item.satirlar.objeler)
                    {
                        if (ditem.tutar + ditem.tutar1 == 0) continue;

                        XLS.SatirAc(yazSatir, 1);
                        XLS.HucreKopyala(0, sutun, 0, sutun + 5, yazSatir, sutun);

                        XLS.HucreDegerYaz(yazSatir, 0, siraNo);
                        XLS.HucreDegerYaz(yazSatir, 1, item.birimAdi);
                        XLS.HucreDegerYaz(yazSatir, 2, ditem.ekoKod);
                        XLS.HucreDegerYaz(yazSatir, 3, ditem.ekoAdi);
                        XLS.HucreDegerYaz(yazSatir, 4, ditem.tutar);
                        XLS.HucreDegerYaz(yazSatir, 5, ditem.tutar1);

                        dusulecekToplam += ditem.tutar;
                        artirilacakToplam += ditem.tutar1;

                        yazSatir++;
                        siraNo++;
                    }
                }

                XLS.HucreAdBulYaz("odenegiDusurulecekTutar", dusulecekToplam);
                XLS.HucreAdBulYaz("odenegiArtirilacakTutar", artirilacakToplam);

                // Ek Ödenek yazdır
                XLS.AktifSheetDegistir(4);
                XLS.HucreAdBulYaz("baslikEkOdenek", kriter.yil + " " + OrtakFonksiyonlar.AyAdiGetir(kriter.ay).ToUpper() + " AYI EK ÖDENEK RAPORU");

                XLS.HucreAdAdresCoz("baslaSatir5", ref kaynakSatir, ref sutun);
                yazSatir = kaynakSatir;

                siraNo = 1;

                foreach (TkdkTalepForm item in bilgiler.objeler)
                {
                    if (item.tur == 1 || item.tur == 2) continue;

                    XLS.SatirAc(yazSatir, 1);
                    XLS.HucreKopyala(0, sutun, 0, sutun + 3, yazSatir, sutun);

                    XLS.HucreDegerYaz(yazSatir, 0, siraNo);
                    XLS.HucreDegerYaz(yazSatir, 1, item.birimAdi);
                    XLS.HucreDegerYaz(yazSatir, 2, item.gerekce);

                    yazSatir++;
                    siraNo++;
                }

                // Haftalık yazdır
                XLS.AktifSheetDegistir(5);
                XLS.HucreAdAdresCoz("baslaSatir6", ref kaynakSatir, ref sutun);
                yazSatir = kaynakSatir;
                siraNo = 1;
                double hafta1AltToplam = 0;
                double hafta2AltToplam = 0;
                double hafta3AltToplam = 0;
                double hafta4AltToplam = 0;

                foreach (SatirBilgi item in satirListe)
                {
                    XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 7, yazSatir, 0);
                    XLS.HucreDegerYaz(yazSatir, 0, siraNo);
                    XLS.HucreDegerYaz(yazSatir, 1, item.birimAdi);

                    foreach (SutunBilgi sitem in sutunListe)
                    {
                        if (item.birimAdi == "TOPLAM") continue;

                        if (sitem.sutunKodu == "MEVCUT")
                            XLS.HucreDegerYaz(yazSatir, 2, item.tutar[sitem.kolonNo]);

                        if (sitem.sutunKodu == "TOPLAM")
                        {
                            if (item.tutar1[sitem.kolonNo] > 0)
                            {
                                double yaztutar = item.tutar1[sitem.kolonNo];
                                XLS.HucreDegerYaz(yazSatir, 3, yaztutar);
                                hafta1AltToplam += yaztutar;
                            }
                            else
                                XLS.HucreDegerYaz(yazSatir, 3, 0);

                            if (item.tutar2[sitem.kolonNo] > 0)
                            {
                                double yaztutar = item.tutar2[sitem.kolonNo];
                                XLS.HucreDegerYaz(yazSatir, 4, yaztutar);
                                hafta2AltToplam += yaztutar;
                            }
                            else
                                XLS.HucreDegerYaz(yazSatir, 4, 0);

                            if (item.tutar3[sitem.kolonNo] > 0)
                            {
                                double yaztutar = item.tutar3[sitem.kolonNo];
                                XLS.HucreDegerYaz(yazSatir, 5, yaztutar);
                                hafta3AltToplam += yaztutar;
                            }
                            else
                                XLS.HucreDegerYaz(yazSatir, 5, 0);

                            if (item.tutar4[sitem.kolonNo] > 0)
                            {
                                double yaztutar = item.tutar4[sitem.kolonNo];
                                XLS.HucreDegerYaz(yazSatir, 6, yaztutar);
                                hafta4AltToplam += yaztutar;
                            }
                            else
                                XLS.HucreDegerYaz(yazSatir, 6, 0);
                        }
                    }
                    siraNo++;
                    yazSatir++;
                }
                XLS.HucreDegerYaz(yazSatir - 1, 0, "");//Toplam satırındaki sırano bilgisini sil
                XLS.HucreDegerYaz(yazSatir - 1, 3, hafta1AltToplam);//Alt toplam hafta1
                XLS.HucreDegerYaz(yazSatir - 1, 4, hafta2AltToplam);//Alt toplam hafta2
                XLS.HucreDegerYaz(yazSatir - 1, 5, hafta3AltToplam);//Alt toplam hafta3
                XLS.HucreDegerYaz(yazSatir - 1, 6, hafta4AltToplam);//Alt toplam hafta4
                XLS.HucreDegerYaz(yazSatir - 1, 7, hafta1AltToplam + hafta2AltToplam + hafta3AltToplam + hafta4AltToplam);
                XLS.KoyuYap(yazSatir - 1, 0, yazSatir - 1, 7, true);
                XLS.ArkaPlanRenk(yazSatir - 1, 0, yazSatir - 1, 7, System.Drawing.Color.FromArgb(242, 242, 242));
                XLS.SatirYukseklikAyarla(yazSatir - 1, yazSatir - 1, 19);

                XLS.AktifSheetDegistir(0);
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "TalepRapolarOlurEki", true, "xlsx");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        // ------------------------------------------------------------------ banka listesi (eski BankaListesi DirectMethod)

        private static IResult BankaListesi(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int yil = QSayi(baglam, "yil");
            int ay = QSayi(baglam, "ay");
            int hafta = QSayi(baglam, "hafta");
            if (yil == 0 || ay == 0 || hafta == 0)
                return Hata("Yıl, ay veya hafta bilgisi boş bırakılamaz. Lütfen ilgili yıl ve ay bilgisini seçiniz");
            try
            {
                string sonucDosyaAd = DosyaIslem.DosyaAdUret("BankaListesi", "") + ".xlsm";
                Tablo XLS = SablonAc("MUH-TKDKTalepZiraat.xlsm", sonucDosyaAd);
                if (XLS == null) return Hata("Şablon (MUH/MUH-TKDKTalepZiraat.xlsm) bulunamadı.");

                int kaynakSatir = 0;
                int sutun = 0;
                XLS.HucreAdAdresCoz("baslaSatir", ref kaynakSatir, ref sutun);

                int satir = kaynakSatir;

                List<SatirBilgi> satirListe = new List<SatirBilgi>();
                List<SutunBilgi> sutunListe = new List<SutunBilgi>();
                int kolonSutun = 1;
                ListeHazirla(kullanan, yil, ay, satirListe, sutunListe, ref kolonSutun);

                int siraNo = 1;
                foreach (SatirBilgi item in satirListe)
                {
                    if (item.birimAdi == "TOPLAM") continue;

                    double birimTutar = HaftaTutar(item, sutunListe, hafta);
                    if (birimTutar <= 0) continue;

                    string adiParca1 = "";
                    string adiParca2 = "";
                    int uzunluk = 30;

                    char[] chBirim = (item.vergiAdi ?? "").Trim().ToCharArray();

                    for (int i = 0; i < chBirim.Length; i++)
                    {
                        adiParca1 += chBirim[i];
                        if (i == uzunluk - 1) break;
                    }
                    for (int i = uzunluk; i < chBirim.Length; i++)
                    {
                        adiParca2 += chBirim[i];
                        if (i == (uzunluk * 2) - 1) break;
                    }

                    XLS.HucreDegerYaz(satir, 0, siraNo);
                    XLS.HucreDegerYaz(satir, 1, "M01_HesabaÖdeme");
                    XLS.HucreDegerYaz(satir, 2, "99_Diğer");
                    XLS.HucreDegerYaz(satir, 3, item.IBAN);
                    XLS.HucreDegerYaz(satir, 4, adiParca1);
                    XLS.HucreDegerYaz(satir, 5, adiParca2);
                    XLS.HucreDegerYaz(satir, 6, item.vergiNo);
                    XLS.HucreDegerYaz(satir, 7, birimTutar);
                    XLS.HucreDegerYaz(satir, 8, item.birimAdi.Trim() + "ne Merkezden gönderilen tutar");
                    satir++;
                    siraNo++;
                }

                XLS.DosyaSaklamaFormatAta("xlsm");
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "BankaListesi", true, "xlsm");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Banka listesi hazırlanamadı."));
            }
        }

        // ------------------------------------------------------------------ ödeme emri (nakit) (eski btnOdemeEmriNakit_Click)

        private static Task<IResult> OdemeEmriNakit(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TKDKTalepOdemeEmriIstegi istek)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, false, kullanan =>
            {
                if (istek == null) return Results.BadRequest();
                int yil = istek.yil;
                int ay = istek.ay;
                int hafta = istek.hafta;
                if (yil == 0 || ay == 0 || hafta == 0)
                    return Hata("Yıl, ay veya hafta bilgisi boş bırakılamaz. Lütfen ilgili yıl ve ay bilgisini seçiniz");

                IMUHServis servisMUH = Servis();

                string ilgiliVNo = "8240299719";
                string detayBirim = "904.50.30.04.04";
                string hesapKodBorc = "510.03.01";
                string hesapKodAlacak = "103.02.01.01";

                int muhasebeKod = OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIMUHASEBEKOD"), 0);
                TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
                TNS.HRC.FirmaBilgisi f = servisHRC.FirmaBilgisiListele(kullanan, muhasebeKod, ilgiliVNo);
                if (f == null) return Hata(ilgiliVNo + " vergi numaralı firma bilgisi bulunamadı.");

                //Ödeme emri başlık oluştur
                TNS.MUH.OdemeEmriMIFForm oe = new TNS.MUH.OdemeEmriMIFForm();
                oe.yil = yil;
                oe.kurum = TNS.MUH.Arac.VarsayilanKurumKod();
                oe.belgeNo = "";
                oe.muhasebe = muhasebeKod;
                oe.birim = "000";
                oe.islemTarih = new TNS.TNSDateTime(DateTime.Now);
                oe.ilgiliAd = f.ad;
                oe.ilgiliVD = f.vergiDairesi;
                oe.ilgiliNo = f.vergiNo;
                oe.ilgiliBankaAd = f.banka;
                oe.ilgiliBankaNo = f.hesapNo;
                oe.aciklama = OrtakFonksiyonlar.AyAdiGetir(ay) + " ayı " + hafta + ".hafta ödemeler için merkezden gönderilen tutarlar.";

                List<SatirBilgi> satirListe = new List<SatirBilgi>();
                List<SutunBilgi> sutunListe = new List<SutunBilgi>();
                int kolonSutun = 1;
                ListeHazirla(kullanan, yil, ay, satirListe, sutunListe, ref kolonSutun);

                TNS.MUH.OdemeEmriMIFDetay od;
                double genelToplam = 0;

                foreach (SatirBilgi item in satirListe)
                {
                    if (item.birimAdi == "TOPLAM") continue;

                    //Önceden oluşturulmuş ödeme emri var ise onu kullan
                    if (hafta == 1 && !string.IsNullOrEmpty(item.odemeEmriNo1)) oe.belgeNo = item.odemeEmriNo1;
                    else if (hafta == 2 && !string.IsNullOrEmpty(item.odemeEmriNo2)) oe.belgeNo = item.odemeEmriNo2;
                    else if (hafta == 3 && !string.IsNullOrEmpty(item.odemeEmriNo3)) oe.belgeNo = item.odemeEmriNo3;
                    else if (hafta == 4 && !string.IsNullOrEmpty(item.odemeEmriNo4)) oe.belgeNo = item.odemeEmriNo4;

                    double birimTutar = HaftaTutar(item, sutunListe, hafta);
                    if (birimTutar < 0)
                        birimTutar = 0;

                    od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                    od.hesapKod = hesapKodBorc;
                    od.detayBirim = detayBirim;
                    od.borc = birimTutar;
                    od.alacak = 0.0;
                    od.altHesapKod = item.vergiNo;
                    TNS.MUH.Arac.TertipAta(ref od, "");
                    if (od.borc > 0)
                        oe.detay.objeler.Add(od);

                    genelToplam += birimTutar;
                }

                od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                od.hesapKod = hesapKodAlacak;
                od.detayBirim = detayBirim;
                od.borc = 0.0;
                od.alacak = genelToplam;
                od.altHesapKod = "";
                TNS.MUH.Arac.TertipAta(ref od, "");
                oe.detay.objeler.Add(od);

                Sonuc sonuc = servisMUH.OdemeEmriMIFKaydet(kullanan, oe);
                if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc.hataStr, "Ödeme emri oluşturulamadı."));

                TkdkTalepForm oeBilgi = new TkdkTalepForm();
                oeBilgi.yil = yil;
                oeBilgi.ay = ay;
                oeBilgi.tur = 1;//Nakit
                if (hafta == 1) oeBilgi.odemeEmriNo1 = sonuc.anahtar;
                else if (hafta == 2) oeBilgi.odemeEmriNo2 = sonuc.anahtar;
                else if (hafta == 3) oeBilgi.odemeEmriNo3 = sonuc.anahtar;
                else if (hafta == 4) oeBilgi.odemeEmriNo4 = sonuc.anahtar;
                servisMUH.TkdkTalepOdemeEmriGuncelle(kullanan, oeBilgi);

                return Results.Json(new
                {
                    basarili = true,
                    bilgi = "Ödeme Emri/MİF kaydı yapıldı. Belge Numarası : " + sonuc.anahtar,
                    yil,
                    belgeNo = sonuc.anahtar
                });
            });
        }

        // ------------------------------------------------------------------ ödeme emri (aktarma) (eski btnTertipEkraniHazirla_Click / btnOdemeEmriAktarma_Click)

        /// <summary>Tertip penceresi satırları: onaylanmış ödenek aktarma taleplerinin tutarlı satırları.</summary>
        private static IResult TertipListe(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                TkdkTalepForm kriter = Kriter(baglam);
                kriter.durum = (int)ENUMTKDKTalepDurum.TAMAMLANDI;

                ObjectArray bilgiler = Servis().TkdkTalepListele(kullanan, kriter);
                if (bilgiler == null || !bilgiler.sonuc.islemSonuc) return Hata(HataMetni(bilgiler?.sonuc.hataStr, "Liste okunamadı."));

                List<object> liste = new List<object>();
                foreach (TkdkTalepForm item in bilgiler.objeler)
                {
                    if (item.tur == 1 || item.tur == 3) continue;

                    foreach (TkdkTalepDetay ditem in item.satirlar.objeler)
                    {
                        if (ditem.tutar + ditem.tutar1 == 0) continue;

                        liste.Add(new
                        {
                            KOD = ditem.kod,
                            BIRIMKOD = item.birimKod,
                            KURKOD = item.birimKurumsalkod,
                            EKOKOD = ditem.ekoKod,
                            DUSULEN = ditem.tutar,
                            EKLENEN = ditem.tutar1,
                            TERTIP = ditem.tertip ?? ""
                        });
                    }
                }
                return Results.Json(new { satirlar = liste });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Liste okunamadı."));
            }
        }

        private static Task<IResult> OdemeEmriAktarma(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TKDKTalepOdemeEmriIstegi istek)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, false, kullanan =>
            {
                if (istek == null) return Results.BadRequest();
                IMUHServis servisMUH = Servis();
                string oHata = "";

                if (istek.tertipler != null)
                {
                    ObjectArray tListe = new ObjectArray();
                    foreach (TKDKTalepTertipSatir item in istek.tertipler)
                    {
                        TkdkTalepDetay t = new TkdkTalepDetay();
                        t.kod = item.KOD ?? "";
                        t.tertip = item.TERTIP ?? "";
                        tListe.objeler.Add(t);
                    }

                    servisMUH.TkdkTalepTertipKaydet(kullanan, tListe);
                }

                int yil = istek.yil;
                int ay = istek.ay;

                if (yil == 0 || ay == 0)
                    return Hata("Yıl veya ay bilgisi boş bırakılamaz. Lütfen ilgili yıl ve ay bilgisini seçiniz");

                string hesapKodDusen = "900";
                string hesapKodArtan = "901";

                int muhasebeKod = OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIMUHASEBEKOD"), 0);

                //Ödeme emri başlık oluştur
                TNS.MUH.OdemeEmriMIFForm oe = new TNS.MUH.OdemeEmriMIFForm();
                oe.yil = yil;
                oe.kurum = TNS.MUH.Arac.VarsayilanKurumKod();
                oe.belgeNo = "";
                oe.muhasebe = muhasebeKod;
                oe.birim = "000";
                oe.islemTarih = new TNS.TNSDateTime(DateTime.Now);
                oe.aciklama = OrtakFonksiyonlar.AyAdiGetir(ay) + " ayı ödemeleri için yapılan bütçe aktarma tutarları.";

                TkdkTalepForm kriter = new TkdkTalepForm();
                kriter.yil = yil;
                kriter.ay = ay;
                kriter.tur = 2;//Aktarma
                kriter.durum = (int)ENUMTKDKTalepDurum.TAMAMLANDI;
                kriter.detayGoster = true;
                ObjectArray bilgiler = servisMUH.TkdkTalepListele(kullanan, kriter);

                TNS.MUH.OdemeEmriMIFDetay od;
                List<object> belgeler = new List<object>();

                foreach (TkdkTalepForm item in bilgiler.objeler)
                {
                    //Önceden oluşturulmuş ödeme emri var ise onu kullan
                    if (!string.IsNullOrEmpty(item.odemeEmriNo1)) oe.belgeNo = item.odemeEmriNo1;

                    oe.birim = item.birimKod;
                    oe.ilgiliAd = item.birimAdi;
                    oe.ilgiliNo = item.birimVergiNo;

                    foreach (TkdkTalepDetay ditem in item.satirlar.objeler)
                    {
                        if (!(ditem.tutar > 0 || ditem.tutar1 > 0))
                            continue;

                        //Düşen,900.03.04 kodu 10.000 alacak yazılaraK düşülür = karşılığında 901.12.03.04.kodu 10.000 borç yazılır. (Düşülen hesapta 901.12 kodu)
                        //Artan,900.03.07.kodu 10.000 borç yazılarak arttırılır = karşılığında 901.11.03.07 kodu 10.000 alacak yazılır. (Arttırılan hesapta ise 901.11 kodu kullanılır)

                        string karsiHesap = "";
                        od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                        od.hesapKod = hesapKodDusen + "." + ditem.ekoKod;
                        od.borc = 0;
                        od.alacak = 0;

                        if (ditem.tutar > 0)//Düşen
                        {
                            od.alacak = ditem.tutar;
                            karsiHesap = "12";
                        }
                        else//Artan
                        {
                            od.borc = ditem.tutar1;
                            karsiHesap = "11";
                        }

                        TNS.MUH.Arac.TertipAta(ref od, ditem.tertip);
                        oe.detay.objeler.Add(od);

                        od = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                        od.hesapKod = hesapKodArtan + "." + karsiHesap + "." + ditem.ekoKod;
                        if (karsiHesap == "12")
                            od.borc = ditem.tutar;
                        else
                            od.alacak = ditem.tutar1;

                        TNS.MUH.Arac.TertipAta(ref od, ditem.tertip);
                        oe.detay.objeler.Add(od);
                    }

                    Sonuc sonuc = servisMUH.OdemeEmriMIFKaydet(kullanan, oe);
                    if (sonuc.islemSonuc)
                    {
                        TkdkTalepForm oeBilgi = new TkdkTalepForm();
                        oeBilgi.yil = yil;
                        oeBilgi.ay = ay;
                        oeBilgi.odemeEmriNo1 = sonuc.anahtar;
                        oeBilgi.kod = item.kod;
                        oeBilgi.tur = 2;//Aktarma
                        servisMUH.TkdkTalepOdemeEmriGuncelle(kullanan, oeBilgi);
                        belgeler.Add(new { yil, belgeNo = sonuc.anahtar, birim = item.birimKod });
                    }
                    else
                        oHata += "<li>" + sonuc.hataStr + "</li>";

                    oe.belgeNo = "";
                    oe.detay = new ObjectArray();
                }

                if (oHata != "")
                    oHata = "<ul>Aktarım sırasında bazı hatalar oldu" + oHata + "</ul>";

                return Results.Json(new { basarili = true, bilgi = Duz("Bütçe aktarım fişi aktarıldı" + oHata), hataVar = oHata != "", belgeler });
            });
        }

        // ================================================================== Talep Giriş (eski TKDKTalepGiris.aspx)

        private static IResult DenetleForm(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath, FormAdresi)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static async Task<IResult> DenetleFormYaz(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, Func<Kullanici, IResult> is_)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = DenetleForm(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try { return is_(kullanan); }
            catch (Exception ex) { return Hata(HataMetni(ex.Message, "İşlem yapılamadı.")); }
        }

        private static TkdkTalepForm TalepOku(Kullanici kullanan, string kod)
        {
            TkdkTalepForm kriter = new TkdkTalepForm();
            kriter.kod = (kod ?? "").Trim();
            kriter.detayGoster = true;
            if (kriter.kod == "") return null;
            ObjectArray bilgiler = Servis().TkdkTalepListele(kullanan, kriter);
            if (bilgiler == null || bilgiler.objeler.Count == 0) return null;
            return (TkdkTalepForm)bilgiler.objeler[0];
        }

        /// <summary>Eski Page_Load + Listele: yeni talepte kullanıcının muhasebe / birimi, kayıtlı talepte bilgileri ve satırları.</summary>
        private static IResult FormTanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = DenetleForm(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                string kod = Q(baglam, "kod");
                TkdkTalepForm item = kod == "" ? null : TalepOku(kullanan, kod);
                if (kod != "" && item == null) return Hata("Talep kaydı bulunamadı.");

                if (item == null)
                {
                    return Results.Json(new
                    {
                        kod = "",
                        muhasebe = Degisken(kullanan, "MUHASEBE"),
                        birim = Degisken(kullanan, "MUHASEBEBIRIM"),
                        tur = 0,
                        yil = DateTime.Now.Year,
                        ay = DateTime.Now.Month,
                        mevcutBakiye = 0.0,
                        gerekce = "",
                        durum = 0,
                        durumAd = "",
                        satirlar = new List<object>()
                    });
                }

                List<object> liste = new List<object>();
                foreach (TkdkTalepDetay ditem in item.satirlar.objeler)
                {
                    liste.Add(new
                    {
                        KOD = ditem.kod,
                        HESAPKOD = ditem.ekoKod,
                        HESAPADI = ditem.ekoAdi,
                        TUTAR11 = ditem.tutar,
                        TUTAR12 = ditem.tutar1,
                        TUTAR13 = ditem.tutar2,
                        TUTAR14 = ditem.tutar3,
                        TUTAR21 = ditem.tutar,
                        TUTAR22 = ditem.tutar1
                    });
                }

                return Results.Json(new
                {
                    kod = item.kod,
                    muhasebe = item.muhasebeKod.ToString(),
                    birim = item.birimKod,
                    tur = item.tur,
                    yil = item.yil,
                    ay = item.ay,
                    mevcutBakiye = item.mevcutBakiye,
                    gerekce = item.gerekce,
                    durum = item.durum,
                    durumAd = DurumAd(item.durum),
                    satirlar = liste
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Talep okunamadı."));
            }
        }

        /// <summary>Eski TalepTanimListele_Click: talep türü / yıl / ay değişince tanım formundaki ekonomik kodlar tutarsız satır olarak gelir.</summary>
        private static IResult FormTanimSatir(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = DenetleForm(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                TalepTanimForm kriter = new TalepTanimForm();
                kriter.talepTur = QSayi(baglam, "tur");
                kriter.yil = QSayi(baglam, "yil");
                kriter.ay = QSayi(baglam, "ay");
                kriter.detayGoster = true;
                if (kriter.talepTur == 0 && kriter.yil == 0 && kriter.ay == 0) return Results.Json(new { satirlar = new List<object>() });

                ObjectArray bilgiler = Servis().TalepTanimListele(kullanan, kriter);
                if (bilgiler == null || !bilgiler.sonuc.islemSonuc) return Hata(HataMetni(bilgiler?.sonuc.hataStr, "Tanımlar okunamadı."));

                List<object> liste = new List<object>();
                foreach (TalepTanimForm item in bilgiler.objeler)
                {
                    foreach (TalepTanimFormDetay ditem in item.satirlar.objeler)
                    {
                        liste.Add(new
                        {
                            KOD = "",
                            HESAPKOD = ditem.ekoKod,
                            HESAPADI = ditem.ekoAdi,
                            TUTAR11 = 0,
                            TUTAR12 = 0,
                            TUTAR13 = 0,
                            TUTAR14 = 0,
                            TUTAR21 = 0,
                            TUTAR22 = 0
                        });
                    }
                }
                return Results.Json(new { satirlar = liste });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Tanımlar okunamadı."));
            }
        }

        /// <summary>Eski DetayDoldur: satırlar; ödenek aktarmada düşülecek ve artırılacak toplamı eşit değilse sonuç başarısız.</summary>
        private static ObjectArray DetayDoldur(List<TKDKTalepSatirIstegi> satirlar, int talepTur, string gerekce)
        {
            ObjectArray bilgiler = new ObjectArray();

            double dusulecekToplamTutar = 0.0;
            double artirilacakToplamTutar = 0.0;

            int siraNo = 0;
            foreach (TKDKTalepSatirIstegi row in satirlar)
            {
                double tutar1 = 0.0;
                double tutar2 = 0.0;
                double tutar3 = 0.0;
                double tutar4 = 0.0;
                if (talepTur == 1)
                {
                    tutar1 = row.TUTAR11 ?? 0;
                    tutar2 = row.TUTAR12 ?? 0;
                    tutar3 = row.TUTAR13 ?? 0;
                    tutar4 = row.TUTAR14 ?? 0;
                }
                else if (talepTur == 2)
                {
                    tutar1 = row.TUTAR21 ?? 0;
                    tutar2 = row.TUTAR22 ?? 0;
                    dusulecekToplamTutar += tutar1;
                    artirilacakToplamTutar += tutar2;
                }

                siraNo++;

                TkdkTalepDetay d = new TkdkTalepDetay();
                string hesapKod = row.HESAPKOD ?? "";
                string hesapAdi = row.HESAPADI ?? "";

                string[] kod = hesapKod.Split('-');

                d.siraNo = siraNo;
                d.ekoKod = kod[0].Replace(".", "").Replace("830", "");
                d.tutar = tutar1;
                d.tutar1 = tutar2;
                d.tutar2 = tutar3;
                d.tutar3 = tutar4;
                d.gerekce = gerekce;
                d.ekoAdi = hesapAdi;

                bilgiler.Ekle(d);
            }

            if (dusulecekToplamTutar != artirilacakToplamTutar)
                bilgiler.sonuc.islemSonuc = false;

            return bilgiler;
        }

        /// <summary>Eski btnKaydet_Click.</summary>
        private static Task<IResult> FormKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TKDKTalepFormIstegi istek)
        {
            return DenetleFormYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                if (istek == null) return Results.BadRequest();

                TkdkTalepForm bilgi = new TkdkTalepForm();
                bilgi.kod = (istek.kod ?? "").Trim();
                bilgi.muhasebeKod = OrtakFonksiyonlar.ConvertToInt(istek.muhasebe, 0);
                bilgi.birimKod = (istek.birim ?? "").Trim();
                bilgi.tur = istek.tur;
                bilgi.yil = istek.yil;
                bilgi.ay = istek.ay;
                bilgi.gerekce = istek.gerekce ?? "";
                bilgi.durum = (int)ENUMTKDKTalepDurum.YENI;
                bilgi.mevcutBakiye = Math.Round(istek.mevcutBakiye ?? 0, 0);
                if (bilgi.tur != 1)//Nakit talep dışındakiler için
                    bilgi.mevcutBakiye = 0;

                List<TKDKTalepSatirIstegi> satirlar = (bilgi.tur == 1 || bilgi.tur == 2) ? istek.satirlar : null;

                if (bilgi.tur != 3 && satirlar != null && satirlar.Count > 0)
                    bilgi.satirlar = DetayDoldur(satirlar, bilgi.tur, bilgi.gerekce);

                if (!bilgi.satirlar.sonuc.islemSonuc)//DUSULECEK VE ARTIRILACAK TUTARLARI ESIT DEGILSE KAYIT ISLEMI YAPTIRMA
                    return Hata("Düşürülecek tertip toplam tutarı ile artırılacak tertip toplam tutarı eşit olmak zorundadır.");

                Sonuc sonuc = Servis().TkdkTalepKaydet(kullanan, bilgi);
                if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc.hataStr, "Kaydedilemedi."));

                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBE", istek.muhasebe ?? "");
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBEBIRIM", istek.birim ?? "");

                return Results.Json(new { basarili = true, bilgi = Duz(sonuc.bilgiStr), kod = sonuc.anahtar });
            });
        }

        /// <summary>Eski btnSil_Click.</summary>
        private static Task<IResult> FormSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TKDKTalepFormIstegi istek)
        {
            return DenetleFormYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                if (istek == null) return Results.BadRequest();
                TkdkTalepForm bilgi = new TkdkTalepForm();
                bilgi.kod = (istek.kod ?? "").Trim();
                bilgi.muhasebeKod = OrtakFonksiyonlar.ConvertToInt(istek.muhasebe, 0);
                bilgi.birimKod = (istek.birim ?? "").Trim();

                Sonuc sonuc = Servis().TkdkTalepSil(kullanan, bilgi);
                if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc.hataStr, "Silinemedi."));
                return Results.Json(new { basarili = true, bilgi = Duz(sonuc.bilgiStr) });
            });
        }

        /// <summary>Eski btnYazdir_Click: aynı yıl / ay / muhasebe / birimin talepleri MUH-TKDKTalepFormNormal.xltx (Nakit ve Aktarma sayfaları).</summary>
        private static IResult FormYazdir(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = DenetleForm(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                TkdkTalepForm kriter = TalepOku(kullanan, Q(baglam, "kod"));
                if (kriter == null) return Hata("Yazdırmak için önce talebi kaydediniz.");

                TkdkTalepForm kriter1 = new TkdkTalepForm();
                kriter1.yil = kriter.yil;
                kriter1.ay = kriter.ay;
                kriter1.muhasebeKod = kriter.muhasebeKod;
                kriter1.birimKod = kriter.birimKod;
                kriter1.detayGoster = true;

                ObjectArray ekListe = Servis().TkdkTalepListele(kullanan, kriter1);

                string sonucDosyaAd = DosyaIslem.DosyaAdUret("TalepFormu", "") + ".xlsx";
                Tablo XLS = SablonAc("MUH-TKDKTalepFormNormal.xltx", sonucDosyaAd);
                if (XLS == null) return Hata("Şablon (MUH/MUH-TKDKTalepFormNormal.xltx) bulunamadı.");

                YazdirNakit(kullanan, XLS, ekListe);
                YazdirOdenekAktarma(XLS, ekListe);

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "TalepFormu", true, "xlsx");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        private static void YazdirNakit(Kullanici kullanan, Tablo XLS, ObjectArray bilgiler)
        {
            int yazSatir = 0;
            int sutun = 0;

            XLS.HucreAdAdresCoz("baslaSatir1", ref yazSatir, ref sutun);

            XLS.HucreAdBulYaz("duzenleyen", Degisken(kullanan, "TKDKTALEPDUZENLEYENADSOYAD"));
            XLS.HucreAdBulYaz("duzenleyenGorev", Degisken(kullanan, "TKDKTALEPDUZENLEYENAGOREV"));
            XLS.HucreAdBulYaz("onaylayan", Degisken(kullanan, "TKDKTALEPONAYLAYANADSOYAD"));
            XLS.HucreAdBulYaz("onaylayanGorev", Degisken(kullanan, "TKDKTALEPONAYLAYANADGOREV"));

            foreach (TkdkTalepForm item in bilgiler.objeler)
            {
                if (item.tur != 1) continue;

                XLS.HucreAdBulYaz("yilToplamGelenNakit", item.yil + " YILI TOPLAM GELEN NAKİT");
                XLS.HucreAdBulYaz("birim", (item.birimAdi ?? "").ToUpper());
                XLS.HucreAdBulYaz("ayYil", OrtakFonksiyonlar.AyAdiGetir(item.ay).ToUpper() + " " + item.yil);
                XLS.HucreAdBulYaz("IBANNo", item.birimIban);
                XLS.HucreAdBulYaz("mevcutBakiye", item.mevcutBakiye);
                XLS.HucreAdBulYaz("gelenNakit", item.gelenNakit);

                double talepEdilenToplamTutar = 0.0;
                double birOncekiAyToplamTutar = 0.0;

                int siraNo = 0;
                int listeyeBaslamaSatiri = yazSatir;
                foreach (TkdkTalepDetay ditem in item.satirlar.objeler)
                {
                    talepEdilenToplamTutar += ditem.tutar + ditem.tutar1 + ditem.tutar2 + ditem.tutar3;

                    siraNo++;

                    XLS.SatirAc(yazSatir, 1);
                    XLS.HucreKopyala(0, sutun, 0, sutun + 3, yazSatir, sutun);

                    XLS.HucreDegerYaz(yazSatir, 0, siraNo);
                    XLS.HucreDegerYaz(yazSatir, 1, ditem.ekoAdi);
                    XLS.HucreDegerYaz(yazSatir, 3, ditem.tutar + ditem.tutar1 + ditem.tutar2 + ditem.tutar3);

                    yazSatir++;
                }

                XLS.HucreAdBulYaz("talepEdilenToplamTutar", talepEdilenToplamTutar);
                XLS.HucreAdBulYaz("birOncekiAyToplamTutar", birOncekiAyToplamTutar);

                XLS.SatirYukseklikAyarla(listeyeBaslamaSatiri, yazSatir - 1, 20);
            }

            XLS.AktifSheetDegistir(1);
        }

        private static void YazdirOdenekAktarma(Tablo XLS, ObjectArray bilgiler)
        {
            int yazSatir1 = 0;
            int sutun = 0;

            double dusulecekToplam = 0.0;
            double artirilacakToplam = 0.0;

            foreach (TkdkTalepForm item in bilgiler.objeler)
            {
                if (item.tur == 1) continue;

                XLS.HucreAdBulYaz("yilToplamGelenNakit", item.yil + " YILI TOPLAM GELEN NAKİT");
                XLS.HucreAdBulYaz("birim", (item.birimAdi ?? "").ToUpper());
                XLS.HucreAdBulYaz("ayYil", OrtakFonksiyonlar.AyAdiGetir(item.ay).ToUpper() + " " + item.yil);

                XLS.HucreAdAdresCoz("baslaSatir4", ref yazSatir1, ref sutun);
                int listeyeBaslamaSatiri = yazSatir1;
                foreach (TkdkTalepDetay ditem in item.satirlar.objeler)
                {
                    XLS.SatirAc(yazSatir1, 1);
                    XLS.HucreKopyala(0, sutun, 0, sutun + 6, yazSatir1, sutun);

                    XLS.HucreDegerYaz(yazSatir1, 0, ditem.ekoKod);
                    XLS.HucreDegerYaz(yazSatir1, 1, ditem.ekoAdi);
                    XLS.HucreDegerYaz(yazSatir1, 2, "");//BASLANGIC ODENEGI
                    XLS.HucreDegerYaz(yazSatir1, 3, "");//TATIL ODENEK
                    XLS.HucreDegerYaz(yazSatir1, 4, "");//KULLANILAN ODENEK
                    XLS.HucreDegerYaz(yazSatir1, 5, ditem.tutar);
                    XLS.HucreDegerYaz(yazSatir1, 6, ditem.tutar1);

                    dusulecekToplam += ditem.tutar;
                    artirilacakToplam += ditem.tutar1;

                    yazSatir1++;
                }
                XLS.SatirYukseklikAyarla(listeyeBaslamaSatiri, yazSatir1 - 1, 18);

                int ekAciklamaSatir = 0;
                int ekAciklamaSutun = 0;
                if (item.tur == 2)
                {
                    XLS.HucreAdBulYaz("gerekce", item.gerekce);
                    XLS.HucreAdAdresCoz("gerekce", ref ekAciklamaSatir, ref ekAciklamaSutun);
                    XLS.SatirYukseklikAyarla(ekAciklamaSatir, ekAciklamaSatir, 129);
                }
                else if (item.tur == 3)
                {
                    XLS.HucreAdBulYaz("ekOdenekGerekce", item.gerekce);
                    XLS.HucreAdAdresCoz("ekOdenekGerekce", ref ekAciklamaSatir, ref ekAciklamaSutun);
                    XLS.SatirYukseklikAyarla(ekAciklamaSatir, ekAciklamaSatir, 129);
                }
            }

            XLS.HucreAdBulYaz("dusurulecekToplam", dusulecekToplam);
            XLS.HucreAdBulYaz("artirilacakToplam", artirilacakToplam);

            XLS.AktifSheetDegistir(0);
        }
    }
}
