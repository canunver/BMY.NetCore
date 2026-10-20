using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>
    /// Muhasebe Raporları (eski ButceMuhasebe/MizanBilanco.aspx) uçları. Solda rapor listesi, sağda seçilen raporun kuralları;
    /// Yazdır raporu eski rapor sınıflarıyla (Muhasebe/MizanBilancoRaporlari.cs, isim alanı ButceMuhasebe) ve eski şablonlarla
    /// (RaporSablon/MUH) üretir. Bu sınıf eski code-behind'ın yerini alır.
    ///   GET /api/muhrapor/tanim   : rapor listesi ve her raporda görünen alanlar, fiş türleri, döviz, kullanıcı değişkenleri (eski Page_Load)
    ///   GET /api/muhrapor/yazdir  : raporu üretir ve indirir (eski btnListele_Click)
    ///   GET /api/muhrapor/hesap/agac, hesap/ara, hesap/ad : hesap kodu alanı (Sgb.hesapKodu)
    ///   GET /api/muhrapor/firma/ara : alt hesap (kişi / firma) seçimi (Sgb.firmaSec)
    /// Rapor parametre adları eskisiyle aynıdır (Yil, AyNo, Saymanlik, Birim, BasTarih, BitTarih, BasHesap, BitHesap, ...).
    /// </summary>
    public static class MizanBilancoUclari
    {
        public const string SayfaAdres = "../ButceMuhasebe/MizanBilanco.aspx";
        private const string Uc = "/api/muhrapor/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "yazdir", Yazdir);
            app.MapGet(Uc + "hesap/agac", HesapAgac);
            app.MapGet(Uc + "hesap/ara", HesapAra);
            app.MapGet(Uc + "hesap/ad", HesapAd);
            app.MapGet(Uc + "firma/ara", FirmaAra);
        }

        // ------------------------------------------------------------------ rapor tanımları (eski RaporBilgi / tumRaporlar)

        /// <summary>
        /// Eski MizanBilanco.RaporBilgi. Gizli alanların sırası eski kurucudakiyle aynıdır. Eski kurucu ddlYil_Hidden'ı atamadığı
        /// için Yıl her raporda görünürdü; burada da öyle (YilGizli hiç kullanılmaz).
        /// </summary>
        public class RaporBilgi
        {
            public Type raporTip;
            public string sablonAd;
            public string RAPORADI;
            public string RAPORKODU;
            public Dictionary<string, bool> gizli = new Dictionary<string, bool>();

            public static readonly string[] Alanlar =
            {
                "ddlYil", "ddlAy", "txtBasTarih", "txtBitTarih", "cmpMuhasebe", "cmpBirim", "cmpBaslangicHesapKodu", "cmpBitisHesapKodu",
                "ddlSeviye", "ddlTur", "ddlFisTur", "ddlListeTur", "chkHesapAdiniSec", "txtKurKod", "txtFonKod", "ddlRaporSira",
                "cftertipGosterme", "ddlSayistay", "chkBakiyeSifir", "cmpAltHesap", "ddlFirmaRaporTur", "cfImza", "pnlAciklama",
                "chkProgramButceTertip"
            };

            public RaporBilgi(Type raporTip, string sablonAd, string RAPORADI, string RAPORKODU, params bool[] gizliler)
            {
                this.raporTip = raporTip;
                this.sablonAd = sablonAd;
                this.RAPORADI = RAPORADI;
                this.RAPORKODU = RAPORKODU;
                for (int i = 0; i < Alanlar.Length; i++)
                    gizli[Alanlar[i]] = gizliler[i];
                gizli["ddlYil"] = false; // eski kurucu ddlYil_Hidden'ı atamıyordu
            }
        }

        /// <summary>Eski tumRaporlar (sıra, şablon ve gizli alan değerleri aynen; Resources.ButceMuhasebe FRMMIB metinleri yerine yazıldı).</summary>
        public static readonly RaporBilgi[] tumRaporlar =
        {
            new RaporBilgi(typeof(ButceMuhasebe.Mizan), "MUH-Mizan.xlt", "Mizan",  "1", false, true, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, true, false, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.MizanBirim), "MUH-MizanBirim.xlt", "Mizan (Birim bazında)",  "2", false, true, false, false, false, false, false, false, false, false, false, false, true, false, false, true, true, true, false, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.MizanTertip), "MUH-MizanTertip.xlt",  "Mizan (Tertip bazında)",  "8", false, true, false, false, false, false, false, false, false, false, false, false, true, true, true, true, false, true, false, true, true, true, true, false),

            new RaporBilgi(typeof(ButceMuhasebe.Mizan), "MUH-Mizan.xlt", "Mizan (Geçici)", "44", false, true, true, true, false, false, true, true, false, false, false, false, true, true, true, true, true, true, false, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.Mizan), "MUH-Mizan.xlt", "Mizan (Kesin)", "45", false, true, true, true, false, false, true, true, false, false, false, false, true, true, true, true, true, true, false, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.Mizan), "MUH-Mizan.xlt", "Mizan (Alt Hesap)", "47", false, true, false, false, false, false, false, false, true, false, false, false, true, true, true, true, true, true, false, false, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.Mizan), "MUH-MizanDevir.xlt", "Mizan (Devreden)",  "64", false, true, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, true, false, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.BuyukDefter), "MUH-BuyukDefter.xltx", "Muavin Defteri (Ana hesap bazında)", "3", false, true, false, false, false, false, false, false, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),

            new RaporBilgi(typeof(ButceMuhasebe.BuyukDefter), "MUH-BuyukDefter.xltx", "Muavin Defteri (Detay hesap bazında)", "4", false, true, false, false, false, false, false, true, true, false, false, false, false, false, false, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.BuyukDefter), "MUH-BuyukDefterListe.xlt", "Muavin Defteri (Liste Bazında)", "49", false, true, false, false, false, false, false, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),

            new RaporBilgi(typeof(ButceMuhasebe.BuyukDefterAltHesap), "MUH-BuyukDefter.xltx", "Muavin Defteri (Alt Hesap Bazında)", "46", false, true, false, false, false, false, false, false, true, false, false, false, true, true, true, true, true, true, true, false, true, true, true,true),

            new RaporBilgi(typeof(ButceMuhasebe.BuyukDefter), "MUH-BuyukDefter.xltx",  "Defter-i Kebir", "10",   false, true, false, false, false, false, false, false, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.BuyukDefter), "MUH-BuyukDefter.xltx",  "Defter-i Kebir (Resmi Format)", "10.1", false, true, false, false, false, false, false, false, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),

            new RaporBilgi(typeof(ButceMuhasebe.Yevmiye), "MUH-Yevmiye.xlt", "Yevmiye Defteri", "5", false, true, false, false, false, false, true, true, true, false, false, false, true, true, true, false, true, true, true, true, true, true, true,true),

            new RaporBilgi(typeof(ButceMuhasebe.Yevmiye), "MUH-Yevmiye.xlt", "Yevmiye Defteri (Resmi Format)", "5.1", false, true, false, false, false, false, true, true, true, false, false, false, true, true, true, false, true, false, true, true, true, true, true,true),

            new RaporBilgi(typeof(ButceMuhasebe.Kontrol830905), "MUH-Kontrol830905.xlt", "830-905 Kontrol Raporu", "7", false, true, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),

            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapButce), "MUH-Bilanco.XLT", "Bilanço", "33", false, true, false, false, false, false, true, true, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapButce), "MUH-KesinHesapFaaliyet.XLT", "Faaliyet Sonuçları Tablosu", "31", false, true, false, false, false, false, true, true, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),

            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapGider), "MUH-KesinHesapGider.xlt", "Bütçe Gider Kesin Hesap Cetveli", "11", false, true, false, false, false, false, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapGelir), "MUH-KesinHesapGelir.xlt", "Bütçe Gelir Kesin Hesap Cetveli", "12", false, true, false, false, false, false, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapButce), "MUH-KesinHesapButce.XLT", "Giderlerin Kurumsal Sınıflandırması", "13", false, true, false, false, false, false, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapButce), "MUH-KesinHesapButce.XLT", "Giderlerin Fonksiyonel Sınıflandırması", "14", false, true, false, false, false, false, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapButce), "MUH-KesinHesapButce.XLT", "Giderlerin Ekonomik Sınıflandırması", "15", false, true, false, false, false, false, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapButce), "MUH-KesinHesapButce.XLT", "Bütçe Giderlerinin Kurumsal Sınıflandırması", "16", false, true, false, false, false, false, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapButce), "MUH-KesinHesapButce.XLT", "Bütçe Giderlerinin Fonksiyonel Sınıflandırması", "17", false, true, false, false, false, false, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapButce), "MUH-KesinHesapButce.XLT", "Bütçe Giderlerinin Ekonomik Sınıflandırması", "18", false, true, false, false, false, false, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapButce), "MUH-KesinHesapButce.XLT", "Bütçe Giderlerinin Finansal Sınıflandırması", "19", false, true, false, false, false, false, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapButce), "MUH-KesinHesapButce.XLT", "Gelirlerin Ekonomik Sınıflandırması", "20", false, true, false, false, false, false, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapButce), "MUH-KesinHesapButce.XLT", "Bütçe Gelirlerin Ekonomik Sınıflandırması", "21", false, true, false, false, false, false, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),

            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapButce), "MUH-KesinHesapFaaliyet.XLT", "Bütçe Uygulama Sonuçları Tablosu", "32", false, true, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),

            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapButce), "MUH-KesinHesapDegisim.XLT", "İç Borç Değişim Tablosu", "34", false, true, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapButce), "MUH-KesinHesapDegisim.XLT", "Dış Borç Değişim Tablosu", "35", false, true, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapButce), "MUH-KesinHesapMali.XLT", "Mali Varlık ve Yükümlülükler Değişim Tablosu", "36", false, true, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.KesinHesapOdenek), "MUH-KesinHesapOdenek.XLT", "Bütçe Giderleri ve Ödenekler Tablosu", "37", false, true, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.ButceGiderleriOdenekPrg), "MUH-ButceGiderOdenekPrg.xltx", "Bütçe Giderleri ve Ödenekler Tablosu (Prg)", "37.1", false, true, true, true, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.ButceGiderleriOdenekPrg), "MUH-ButceGiderFaaliyet.xltx", "Bütçe Giderlerinin Faaliyet Sınıflandırması Tablosu", "37.2", false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.UstHesapKodKullanilanlar), "MUH-UstHesap.xlt", "Üst Hesap Kodu Kullanılan Belgeler", "9", false, true, false, false, false, false, false, false, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),

            new RaporBilgi(typeof(ButceMuhasebe.DamgaVergisiBeyannamesi), "MUH-KesinHesapOdenek.XLT", "Damga vergisi beyannamesi", "40", false, true, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.YevmiyeAktar), "", "Birleştirilmiş Veriler Defteri", "41", false, true, false, false, false, false, true, true, true, false, false, false, true, true, true, false, true, true, true, true, true, true, true,true),

            new RaporBilgi(typeof(ButceMuhasebe.DevredenHesaplar), "MUH-DevredenHesaplar.XLT", "Devreden Hesaplar", "42", false, true, false, false, false, false, false, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.MuhasebatButceGideriFonEkoListe), "MUH-MuhasebatGiderFonEkoListe.XLT", "KBS Aktarım Dosyası", "43", false, true, false, false, false, false, true, true, true, false, true, true, true, false, false, true, true, true, true, true, true, true, true,true),

            new RaporBilgi(typeof(ButceMuhasebe.AylikBazdaListe), "MUH-AylikBazdaListe.XLT", "Aylık Bazda Dağılım", "48", false, true, true, true, false, false, false, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.YillikBazdaListe), "MUH-YillikBazdaListe.XLT", "Yıllık Bazda Dağılım (5 Yıllık)", "50", false, true, true, true, false, false, false, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.YillikAylikBazdaListe), "MUH-YillikAylikBazdaListe.XLT", "Yıllık/Aylık Bazda Karşılaştırma (3 Yıllık)", "52", false, true, true, true, false, false, false, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.YillikAylikBazdaListe2Yillik), "MUH-YillikAylikBazdaListe2Yillik.XLT", "Yıllık/Aylık Bazda Karşılaştırma (2 Yıllık)", "63", false, true, true, true, false, false, false, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.YillikAylikTarihselBazdaListe), "MUH-YillikAylikTarihselBazdaListe.XLT", "Yıllık/Aylık Bazda Karşılaştırma (Tarihsel)", "58", false, true, false, false, false, false, false, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true,true),

            new RaporBilgi(typeof(ButceMuhasebe.BirimBazdaListe), "MUH-BirimBazdaListe.XLT", "Birim Bazda Karşılaştırma", "53", false, true, false, false, false, false, false, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.BirimGelirGider), "MUH-BirimGelirGider.XLT", "Birim Gelir/Gider Tablosu", "59", false, true, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.GelirGiderListe), "MUH-GelirGiderListe.XLT", "Gelir/Gider Listesi", "60", false, true, false, false, false, false, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true,true),

            new RaporBilgi(typeof(ButceMuhasebe.EmanetlerDefteri), "EmanetlerDefteri.xltx", "Emanetler Defteri", "51", false, true, true, true, false, false, false, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.TasinirRaporu), "MUH-TasinirRaporu.xlt", "Taşınır Raporu", "54", false, true, true, true, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.GelirStopaj), "MUH-Stopaj.xltx", "Gelir Stopajları", "55", false, true, false, false, false, false, false, false, true, false, false, false, true, true, true, true, true, true, true, false, true, true, true,true),
            new RaporBilgi(typeof(ButceMuhasebe.GelirStopajAktar), "", "Gelir Stopajları (Aktarım)", "61", false, true, false, false, false, false, false, false, true, false, false, false, true, true, true, true, true, true, true, false, true, true, true,true),

            new RaporBilgi(typeof(ButceMuhasebe.FirmalardanAlinan), "MUH-FirmaKesinti.xltx", "Firmadan Yapılan Kesintiler", "56", true, true, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, false,true),
            new RaporBilgi(typeof(ButceMuhasebe.AcikAvanslar), "MUH-AcikAvans.xltx", "Açık Avanslar", "57", true, true, true, true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false, true, true, true,true),

            new RaporBilgi(typeof(ButceMuhasebe.ManasGunlukTakip), "MUH-ManasGunlukTakip.xltx", "Günlük Takip Raporu", "62", true, true, false, true, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true,true),
        };

        private static RaporBilgi RaporBul(string raporTur)
        {
            foreach (RaporBilgi rb in tumRaporlar)
                if (rb.RAPORKODU == raporTur) return rb;
            return null;
        }

        // ------------------------------------------------------------------ yetki ve ortak

        /// <summary>Menü yetkisi (eski GenelIslemler.SayfayaGirisYapabilirMi).</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdres);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = Temiz(hata);
            return hata == "" ? varsayilan : hata;
        }

        private static int HesapYil(int? yil)
        {
            int y = yil ?? 0;
            return y <= 2000 ? DateTime.Now.Year : y;
        }

        /// <summary>yyyy-MM-dd (input type=date) -> gg.aa.yyyy (eski Ext DateField metni); boşsa boş.</summary>
        private static string TarihMetni(string s)
        {
            s = Temiz(s);
            if (s == "") return "";
            if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                return t.ToString("dd.MM.yyyy");
            return s;
        }

        /// <summary>"kod - ad" -> kod (eski Value.Split('-')[0]).</summary>
        private static string KodKismi(string s)
        {
            return Temiz((s ?? "").Split('-')[0]);
        }

        // ------------------------------------------------------------------ açılış (eski Page_Load, DdlleriDoldur, Temizle)

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                List<object> raporlar = new List<object>();
                foreach (RaporBilgi rb in tumRaporlar)
                    raporlar.Add(new { RAPORKODU = rb.RAPORKODU, RAPORADI = rb.RAPORADI, gizli = rb.gizli });

                // Eski ButceMuhasebeGenel.FisTurDoldur
                List<object> fisTurleri = new List<object>();
                foreach (ENUMFISTUR f in new[] { ENUMFISTUR.DONEMICI, ENUMFISTUR.ACILIS, ENUMFISTUR.KAPANIS, ENUMFISTUR.AMORTISMANKAPANISI, ENUMFISTUR.SONFIS })
                    fisTurleri.Add(new { kod = (int)f, ad = TNS.MUH.Arac.FisTurAdBul(f) });

                // Eski ParaBirimiDoldur
                string paraBirimi = ButceMuhasebe.ButceMuhasebeGenel.DondurParaBirimi(1).kisaltma ?? "";

                int seviye = OrtakFonksiyonlar.ConvertToInt(GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBERAPORSEVIYE"), 1);
                string tertipGosterme = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBERAPORTERTIPGOSTERME");
                if (string.IsNullOrEmpty(tertipGosterme)) tertipGosterme = "150;253;254;255";

                string muhasebe = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIMUHASEBEKOD") + "";

                return Results.Json(new
                {
                    raporlar = raporlar,
                    fisTurleri = fisTurleri,
                    paraBirimi = paraBirimi,
                    dovizVar = TNS.MUH.Arac.MuhasebeDovizVarMi(),
                    yilIlk = 2005,
                    yilSon = DateTime.Now.Year + 1,
                    muhasebe = muhasebe.Trim(),
                    seviye = seviye,
                    tertipGosterme = tertipGosterme,
                    imzaAdi = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "RAPORIMZAYETKILIADI"),
                    imzaBirim = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "RAPORIMZAYETKILIBIRIM"),
                    imzaUnvan = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "RAPORIMZAYETKILIUNVAN")
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Sayfa bilgileri okunamadı."));
            }
        }

        // ------------------------------------------------------------------ Yazdır (eski btnListele_Click)

        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            IQueryCollection q = baglam.Request.Query;
            string Al(string ad) { return Temiz(q[ad]); }
            bool Isaretli(string ad) { string v = Al(ad).ToLowerInvariant(); return v == "1" || v == "true"; }

            string raporTur = Al("raporTur");
            RaporBilgi rb = RaporBul(raporTur);
            if (rb == null) return Hata("Raporu seçiniz.");

            string hesap1 = Al("basHesap");
            string hesap2 = Al("bitHesap");
            string altHesap = Al("altHesap");
            string hataStr = "";

            if (raporTur != "5" && raporTur != "5.1")
            {
                if (raporTur == "48" || raporTur == "50" || raporTur == "52" || raporTur == "58" || raporTur == "63")
                {
                }
                else
                {
                    if (hesap1 == "")
                        hataStr = "Başlangıç hesap Kodu alanını boş bırakamazsınız.";
                    else if (hesap1.Length < 3)
                        hataStr = "Başlangıç hesap kodunu kontrol ediniz.";
                }
            }
            if (raporTur != "40")
            {
                if (hesap2 == "")
                    hataStr = "Bitiş hesap Kodu alanını boş bırakamazsınız.";
                else if (hesap2.Length < 3)
                    hataStr = "Bitiş hesap kodunu kontrol ediniz.";
            }

            if (raporTur == "56")
            {
                if (KodKismi(altHesap) == "")
                    hataStr = "Alt hesap bilgisini boş bırakamazsınız";
            }

            if (hataStr != "") return Hata(hataStr);

            try
            {
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBERAPORSEVIYE", Al("seviye"));
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBERAPORTERTIPGOSTERME", Al("tertipGostermeHesaplar"));
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "RAPORIMZAYETKILIADI", Al("imzaAdi"));
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "RAPORIMZAYETKILIBIRIM", Al("imzaBirim"));
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "RAPORIMZAYETKILIUNVAN", Al("imzaUnvan"));
            }
            catch (Exception)
            {
                // eski kodda da saklama hatası raporu durdurmuyordu
            }

            string basHesap = KodKismi(hesap1);
            string bitHesap = KodKismi(hesap2);
            string altHesapKod = "";
            string muhasebe = Al("muhasebe");
            string birim = Al("birim");

            Hashtable raporParam = new Hashtable();
            raporParam.Add("Yil", Al("yil"));
            raporParam.Add("AyNo", Al("ay"));
            raporParam.Add("Saymanlik", muhasebe);
            raporParam.Add("Birim", birim);
            raporParam.Add("SaymanlikAd", GenelIslemler.KodAd(11, muhasebe, true));
            raporParam.Add("BirimAd", GenelIslemler.KodAd(12, birim + "-" + muhasebe, false));
            raporParam.Add("KurKod", Al("kurKod"));
            raporParam.Add("FonKod", Al("fonKod"));
            raporParam.Add("BasTarih", TarihMetni(Al("basTarih")));
            raporParam.Add("BitTarih", TarihMetni(Al("bitTarih")));
            raporParam.Add("BasHesap", basHesap);
            raporParam.Add("BitHesap", bitHesap);
            raporParam.Add("Tur", Al("tur"));
            raporParam.Add("RaporTur", raporTur);
            raporParam.Add("Seviye", Al("seviye"));
            raporParam.Add("FisTur", Al("fisTur"));
            raporParam.Add("ListeTur", Al("listeTur"));
            raporParam.Add("Sayistay", Al("sayistay"));
            raporParam.Add("SayfaSatirSay", "65");
            raporParam.Add("HesapAdiAyniOlanlariListele", Isaretli("hesapAdiAyni"));
            raporParam.Add("RaporSira", Al("raporSira"));
            raporParam.Add("TasinirHesapTertipGosterme", Isaretli("tertipGosterme"));
            raporParam.Add("TasinirHesapTertipGostermeHesaplar", Al("tertipGostermeHesaplar"));
            raporParam.Add("BakiyeSifirGosterme", Isaretli("bakiyeSifir"));
            raporParam.Add("Aciklama", (string)q["aciklama"] ?? "");
            raporParam.Add("KesintiRaporTur", Al("kesintiRaporTur"));
            raporParam.Add("ProgramButceTertipGoster", Isaretli("programButceTertip"));

            if (raporTur == "46" || raporTur == "47" || raporTur == "56")
            {
                altHesapKod = KodKismi(altHesap);
                if (altHesapKod == "") altHesapKod = "-999-";
            }
            raporParam.Add("AltHesap", altHesapKod);
            double.TryParse(Al("stopajOran").Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out double stopajOran);
            raporParam.Add("StopajOran", stopajOran);

            string sablonAd = rb.sablonAd;
            string sablon;
            if (string.IsNullOrEmpty(sablonAd))
            {
                // Eski: şablonsuz raporlara RaporSablonYol + "/MUH/" verilirdi.
                string kurumYol = Ayar.Oku("RaporSablonYol") ?? "";
                sablon = kurumYol != "" ? kurumYol + "/MUH/" : Path.Combine(ortam.ContentRootPath, "RaporSablon", "MUH") + Path.DirectorySeparatorChar;
            }
            else
            {
                sablon = MuavinDefteriIslem.SablonBul(ortam.ContentRootPath, sablonAd);
                if (sablon == null) return Hata("Rapor şablonu bulunamadı: MUH/" + sablonAd);
            }

            string ciktiYeri = GenelIslemler.ExcelTur();
            if (raporTur == "5.1" || raporTur == "10.1")
                ciktiYeri = "TXT";
            else if (raporTur == "41")
                ciktiYeri = "CSV";

            string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
            try
            {
                AsposeLisans.Yukle();
                object ir = Activator.CreateInstance(rb.raporTip);
                MethodInfo raporAl = rb.raporTip.GetMethod("RaporAl", new[] { typeof(object), typeof(object), typeof(string), typeof(string), typeof(string), typeof(Hashtable), typeof(string) });
                if (raporAl == null) return Hata("Rapor sınıfında RaporAl bulunamadı: " + rb.raporTip.Name);

                string sonucDosyaAdCevap;
                try
                {
                    sonucDosyaAdCevap = (string)raporAl.Invoke(ir, new object[] { kullanan, TNS.MUH.Arac.Tanimla(), sablon, sonucDosyaAd, ciktiYeri, raporParam, "" });
                }
                catch (TargetInvocationException tie) when (tie.InnerException != null)
                {
                    return Hata(HataMetni(tie.InnerException.Message, "Rapor üretilemedi"));
                }

                try { File.Delete(sonucDosyaAd); } catch { }

                if (string.IsNullOrEmpty(sonucDosyaAdCevap) || !File.Exists(sonucDosyaAdCevap))
                    return Hata("Rapor üretilemedi");

                string ad = string.IsNullOrEmpty(sablonAd) ? rb.RAPORADI : Path.GetFileNameWithoutExtension(sablonAd);
                string ciktiDosyaAdi = ad + "_" + DateTime.Now.ToString().Replace("-", "").Replace(":", "").Replace(".", "").Replace("/", "").Replace(" ", "");
                string uzanti = Path.GetExtension(sonucDosyaAdCevap).TrimStart('.');
                if (uzanti == "") uzanti = ciktiYeri;
                return DosyaIslem.DosyaGonder(sonucDosyaAdCevap, ciktiDosyaAdi, true, uzanti.ToLowerInvariant());
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor üretilemedi"));
            }
        }

        // ------------------------------------------------------------------ hesap kodu (eski HesapKod.ascx / ListeHesapPlani.aspx)
        // Sgb.hesapKodu (js/hesapkodu.js) uçları; Çapraz Tablo uçlarının aynısı, bu sayfanın yetkisiyle.

        private static object HesapDugum(HesapPlaniSatir h)
        {
            return new { kod = h.hesapKod, ad = h.hesapKod + " - " + h.aciklama, hesapAd = h.aciklama ?? "", altVar = !h.detay, detay = h.detay };
        }

        private static IResult HesapAgac(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            try
            {
                int y = HesapYil(yil);
                ust = Temiz(ust);
                List<object> dugumler = new List<object>();
                if (ust == "")
                {
                    TNS.UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
                    for (int i = 1; i < 10; i++)
                    {
                        string ad = servisUZY.UzayDegeriStr(new Kullanici(), "MUHHESAPPLANI", y.ToString() + "-" + i.ToString(), true);
                        dugumler.Add(new { kod = i.ToString(), ad = i.ToString() + "-" + ad, hesapAd = ad ?? "", altVar = true, detay = false });
                    }
                    return Results.Json(new { dugumler = dugumler });
                }

                string[] kirilimlar = ust.Split('.');
                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.yil = y;
                hs.hesapKod = ust;
                hs.seviye = ust.Length < 3 ? 1 : kirilimlar.Length + 1;

                IMUHServis servis = TNS.MUH.Arac.Tanimla();
                ObjectArray hv = new ObjectArray();
                for (int i = 0; i < 10; i++)
                {
                    hv = servis.HesapPlaniListele(new Kullanici(), hs);
                    if (hv.objeler.Count == 0) hs.seviye++;
                    else break;
                }
                foreach (HesapPlaniSatir h in hv.objeler) dugumler.Add(HesapDugum(h));
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Hata("Hesap planı okunamadı.");
            }
        }

        private static IResult HesapAra(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string metin, string sonKirilim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                metin = Temiz(metin);
                if (metin.Length < 2) return Hata("En az iki karakter yazınız.");
                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = HesapYil(yil);
                h.hesapKodHesapAd = metin;
                h.detay = sonKirilim == "1";
                ObjectArray hesap = TNS.MUH.Arac.Tanimla().HesapPlaniListele(kullanan, h);
                if (hesap == null || !hesap.sonuc.islemSonuc) return Hata(HataMetni(hesap?.sonuc.hataStr, "Arama yapılamadı."));
                List<object> satirlar = new List<object>();
                foreach (HesapPlaniSatir d in hesap.objeler)
                {
                    satirlar.Add(HesapDugum(d));
                    if (satirlar.Count >= 500) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Arama yapılamadı.");
            }
        }

        private static IResult HesapAd(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                kod = Temiz(kod);
                if (kod == "") return Results.Json(new { kod = "", ad = "", detay = false });
                int y = HesapYil(yil);
                string hesYapi = TNS.MUH.Arac.DegiskenDegerBul(y, "/KODYAPI/KOD8/KODYAPI");
                if (!string.IsNullOrEmpty(hesYapi)) kod = OrtakFonksiyonlar.KodNoktaKoy(hesYapi, kod.Replace(".", ""));
                ObjectArray hv = TNS.MUH.Arac.Tanimla().HesapPlaniListele(kullanan, new HesapPlaniSatir { yil = y, hesapKod = kod });
                if (hv != null && hv.objeler != null)
                    foreach (HesapPlaniSatir d in hv.objeler)
                        if (d.hesapKod == kod) return Results.Json(new { kod = d.hesapKod, ad = d.aciklama ?? "", detay = d.detay });
                return Results.Json(new { kod = kod, ad = "", detay = false });
            }
            catch (Exception)
            {
                return Hata("Hesap kodu okunamadı.");
            }
        }

        // ------------------------------------------------------------------ alt hesap (eski AltHesapKod.ascx + ListeFirmaKisi.aspx)

        private static IResult FirmaAra(HttpContext baglam, IWebHostEnvironment ortam, int? muhasebe, string ad, string vergiNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            ad = Temiz(ad);
            vergiNo = Temiz(vergiNo);
            if (ad == "" && vergiNo == "") return Hata("Aramak için ad ya da kimlik / vergi no yazınız.");
            try
            {
                TNS.HRC.FirmaBilgisi h = new TNS.HRC.FirmaBilgisi();
                h.muhasebe = muhasebe ?? 0;
                h.ad = ad;
                h.vergiNo = vergiNo;
                h.aktifDurum = 1;
                ObjectArray bilgi = TNS.HRC.Arac.Tanimla().FirmaBilgisiListele(kullanan, h);
                if (bilgi == null || !bilgi.sonuc.islemSonuc) return Hata(HataMetni(bilgi?.sonuc.hataStr, "Arama yapılamadı."));
                List<object> satirlar = new List<object>();
                foreach (TNS.HRC.FirmaBilgisi f in bilgi.objeler)
                {
                    satirlar.Add(new { kod = f.kod, vergiNo = f.vergiNo ?? "", ad = f.ad ?? "", vergiDairesi = f.vergiDairesi ?? "", adres = f.adres ?? "" });
                    if (satirlar.Count >= 500) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Arama yapılamadı.");
            }
        }
    }
}
