using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Aspose.Words;
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
    /// <summary>Fatura satırı (eski FaturaDetayStore: ACIKLAMA, HESAPKOD, MIKTAR, KDVORAN, BIRIMFIYAT, TUTAR).</summary>
    public class FaturaDetayIstegi
    {
        public int FaturaKod { get; set; }
        public string Aciklama { get; set; }
        public string HesapKod { get; set; }
        public double? Miktar { get; set; }
        public double? KdvOran { get; set; }
        public double? BirimFiyat { get; set; }
        public double? Tutar { get; set; }
    }

    /// <summary>Fatura Giriş formunun Kaydet isteği (eski btnKaydet_Click alanları). Tarihler yyyy-MM-dd.</summary>
    public class FaturaKaydetIstegi
    {
        public int Kod { get; set; }
        public string Muhasebe { get; set; }
        public string Birim { get; set; }
        public string Mudurluk { get; set; }
        public string MusteriKod { get; set; }
        public string MusteriAdi { get; set; }
        public string MusteriAdiEk { get; set; }
        public string FaturaNo { get; set; }
        public string FaturaTarihi { get; set; }
        public string IrsaliyeNo { get; set; }
        public string IrsaliyeTarihi { get; set; }
        public string Aciklama { get; set; }
        public int Tevkifat { get; set; }
        public string MuhBelgeAciklama { get; set; }
        public string TahakkukBelgeNo { get; set; }
        public string OdemeBelgeNo { get; set; }
        public string HesaplamaBirimFiyat { get; set; }
        public string HesaplamaYuzde { get; set; }
        public List<FaturaDetayIstegi> Satirlar { get; set; }
    }

    public class FaturaKodIstegi
    {
        public int Kod { get; set; }
    }

    /// <summary>
    /// Tahakkuk / Ödeme MİF oluşturma penceresi (eski wndTahakkuk). BelgeTur 1: tahakkuk, 2: ödeme.
    /// Damga: 1 kesilmiş, 2 kesilmemiş, 3 yok (eski optDamgaUret1..3). Tarih yyyy-MM-dd.
    /// Fatura Giriş'te Kod (tek fatura), Toplu Tahakkuk'ta Faturalar (seçili satırlar) dolu gelir.
    /// </summary>
    public class FaturaBelgeIstegi
    {
        public int Kod { get; set; }
        public string Muhasebe { get; set; }
        public int BelgeTur { get; set; }
        public string Tarih { get; set; }
        public string TahakkukBelgeNo { get; set; }
        public string OdemeBelgeNo { get; set; }
        public string MuhBelgeAciklama { get; set; }
        public string TahakkukHesapKod { get; set; }
        public string OdemeHesapKod { get; set; }
        public int Damga { get; set; }
        public bool TahakkukTahsilat { get; set; }
        public bool TahakkukAyniFis { get; set; }
        public bool SadeceDamga { get; set; }
        public List<FaturaSeciliSatir> Faturalar { get; set; }
    }

    /// <summary>Toplu Tahakkuk listesinde seçilen satır (eski SATIRLAR: KOD, FATURANO, MUSTERIKOD).</summary>
    public class FaturaSeciliSatir
    {
        public int KOD { get; set; }
        public string FATURANO { get; set; }
        public string MUSTERIKOD { get; set; }
    }

    public class FaturaAciklamaIstegi
    {
        public int Kod { get; set; }
        public int Tur { get; set; }
        public string Aciklama { get; set; }
    }

    /// <summary>
    /// Fatura Hazırlama (eski ButceMuhasebe/FaturaListesi.aspx), içinden açılan Fatura Giriş (FaturaGiris.aspx) ve Toplu Tahakkuk
    /// (FaturaTahakkukIslemleri.aspx) sayfalarının uçları. Eski sayfaların arka plan kodu buraya taşındı; iş kuralları (TNS.MUH)
    /// değişmeden çağrılır.
    ///   Liste : GET  /api/fatura/tanim, liste, liste/yazdir (MUH/FaturaListe.xltx), mudurluk
    ///   Form  : GET  /api/fatura/form/tanim?kod=, form/yazdir?kod= (MUH/FaturaForm[kurum].dotx, Word)
    ///           POST /api/fatura/form/kaydet, form/iptal, form/belge (Tahakkuk / Ödeme MİF oluştur)
    ///   Açıklama: GET /api/fatura/aciklama/liste?tur=   POST aciklama/kaydet, aciklama/sil
    ///   Toplu : GET  /api/fatura/toplu/liste?...      POST toplu/belge
    ///   Hesap : GET  /api/fatura/hesap/agac, hesap/ara, hesap/ad (Sgb.hesapKodu, Sgb.hesapPlaniSec)
    /// Yetki: menüdeki Fatura Hazırlama; Fatura Giriş ve Toplu Tahakkuk menüde yok, yetkileri Fatura Hazırlama'dan gelir.
    /// </summary>
    public static class FaturaUclari
    {
        public const string SayfaAdresi = "../ButceMuhasebe/FaturaListesi.aspx";
        public const string FormAdresi = "../ButceMuhasebe/FaturaGiris.aspx";
        public const string TopluAdresi = "../ButceMuhasebe/FaturaTahakkukIslemleri.aspx";
        private const string Uc = "/api/fatura/";

        private static readonly CultureInfo Tr = new CultureInfo("tr-TR");

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "liste", Liste);
            app.MapGet(Uc + "liste/yazdir", ListeYazdir);
            app.MapGet(Uc + "mudurluk", MudurlukListe);

            app.MapGet(Uc + "form/tanim", FormTanim);
            app.MapPost(Uc + "form/kaydet", FormKaydet);
            app.MapPost(Uc + "form/iptal", FormIptal);
            app.MapGet(Uc + "form/yazdir", FormYazdir);
            app.MapPost(Uc + "form/belge", FormBelge);

            app.MapGet(Uc + "aciklama/liste", AciklamaListe);
            app.MapPost(Uc + "aciklama/kaydet", AciklamaKaydet);
            app.MapPost(Uc + "aciklama/sil", AciklamaSil);

            app.MapGet(Uc + "toplu/liste", TopluListe);
            app.MapPost(Uc + "toplu/belge", TopluBelge);

            app.MapGet(Uc + "hesap/agac", HesapAgac);
            app.MapGet(Uc + "hesap/ara", HesapAra);
            app.MapGet(Uc + "hesap/ad", HesapAd);
        }

        // ------------------------------------------------------------------ yetki ve ortak

        /// <summary>Fatura Hazırlama menü yetkisi; alt sayfalar (form, toplu tahakkuk) üst sayfa Fatura Hazırlama'dan.</summary>
        public static bool YetkiVar(Kullanici kullanan, string klasor, string altSayfa = null)
        {
            if (kullanan == null) return false;
            return altSayfa == null ? SayfaYetki.Yetkili(kullanan, klasor, SayfaAdresi)
                                    : SayfaYetki.Yetkili(kullanan, klasor, altSayfa, SayfaAdresi);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!SayfaYetki.Yetkili(kullanan, ortam.ContentRootPath, null, SayfaAdresi)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static async Task<IResult> DenetleYaz(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, Func<Kullanici, IResult> is_)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return is_(kullanan);
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        private static IResult Hata(string hata) { return Results.Json(new { hata = Duz(hata) }); }

        private static IResult Basarisiz(string hata) { return Results.Json(new { basarili = false, hata = Duz(hata) }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string Duz(string s)
        {
            return (s ?? "").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n").Trim();
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return Duz(hata);
        }

        private static string Degisken(Kullanici kullanan, string ad)
        {
            try { return GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, ad) ?? ""; }
            catch (Exception) { return ""; }
        }

        private static TNSDateTime Tarih(string iso) { return OdemeEmriMIFUclari.Tarih(iso); }

        private static string TarihYaz(TNSDateTime t)
        {
            try
            {
                if (t == null || t.isNull) return "";
                DateTime d = t.Oku();
                return d <= DateTime.MinValue.AddDays(1) ? "" : d.ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static string Tutar(double d) { return d.ToString("#,###.00", Tr); }

        /// <summary>Eski ButceMuhasebeResI "DURUMTIPI_" + durum.ToString("d3") metinleri.</summary>
        public static string DurumAd(int durum)
        {
            switch (durum)
            {
                case 1: return "Yeni";
                case 3: return "Tahakkuk";
                case 5: return "Ödendi";
                case 9: return "İptal";
                default: return "";
            }
        }

        /// <summary>Şablon: önce kurumun RaporSablonYol/MUH klasörü, yoksa uygulamanın RaporSablon/MUH klasörü (bulunamazsa null).</summary>
        private static string SablonBul(string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol") ?? "";
            if (kurumYol != "")
            {
                string kurum = Path.Combine(kurumYol, "MUH", sablonAd);
                if (File.Exists(kurum)) return kurum;
            }
            foreach (string kok in new string[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
            {
                string uygulama = Path.Combine(kok, "RaporSablon", "MUH", sablonAd);
                if (File.Exists(uygulama)) return uygulama;
            }
            return null;
        }

        private static bool Ayar1(string degisken)
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DegiskenDegerBul(0, degisken), 0) > 0;
        }

        // ------------------------------------------------------------------ Fatura Hazırlama (eski FaturaListesi.aspx)

        private static object[] Durumlar()
        {
            return new object[]
            {
                new { kod = (int)ENUMFaturaDurum.BUTUNFATURALAR, ad = "Bütün Faturalar" },
                new { kod = (int)ENUMFaturaDurum.YENI, ad = "Yeni" },
                new { kod = (int)ENUMFaturaDurum.TAHAKKUK, ad = "Tahakkuk" },
                new { kod = (int)ENUMFaturaDurum.ODENDI, ad = "Ödendi" },
                new { kod = (int)ENUMFaturaDurum.IPTAL, ad = "İptal" }
            };
        }

        /// <summary>Açılış (eski Page_Load + Filtre(sayfaAcilis)): yıl, durum Yeni, kullanıcının muhasebe / birim / müdürlüğü.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                string muhasebe = Degisken(kullanan, "MUHASEBE");
                string birim = Degisken(kullanan, "MUHASEBEBIRIM");
                string mudurluk = Degisken(kullanan, "FATURAMUDURLUK");
                string kurum = "";
                string birimAd = "";
                try { birimAd = MuhasebeBirimUclari.BirimAdBul(OrtakFonksiyonlar.ConvertToInt(muhasebe, 0), birim, out kurum); }
                catch (Exception) { birimAd = ""; }
                return Results.Json(new
                {
                    yil = DateTime.Now.Year,
                    durum = (int)ENUMFaturaDurum.YENI,
                    durumlar = Durumlar(),
                    muhasebe = muhasebe, birim = birim, birimAd = birimAd,
                    kurumsal = MuhasebeBirimUclari.KurumsalYaz(DateTime.Now.Year, kurum),
                    mudurluk = mudurluk
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>
        /// Eski Filtre(): bitiş tarihi boşsa eski kodla aynı biçimde hesaplanır (eski kod boş bir tarihin üzerine bir yıl
        /// ekliyordu; davranış değiştirilmedi).
        /// </summary>
        private static Fatura Filtre(int? muhasebe, string birim, string mudurluk, string faturaNo, string musteri, int? durum, string islemYapan,
            int? yil, string tarih1, string tarih2)
        {
            Fatura filtre = new Fatura();
            TNSDateTime sorguBasTarih = new TNSDateTime();

            filtre.muhasebeKod = muhasebe ?? 0;
            filtre.birimKod = Temiz(birim);
            filtre.mudurlukKod = Temiz(mudurluk);
            filtre.faturaNo = Temiz(faturaNo);
            filtre.musteriKod = Temiz(musteri);
            filtre.durum = durum ?? 0;
            filtre.islemYapan = Temiz(islemYapan);

            string y = (yil ?? 0) > 0 ? yil.Value.ToString() : DateTime.Now.Year.ToString();

            TNSDateTime t1 = Tarih(tarih1);
            TNSDateTime t2 = Tarih(tarih2);
            if (!t1.isNull) filtre.sorguBasTarih = t1;
            if (!t2.isNull) filtre.sorguBitTarih = t2;

            if (filtre.sorguBasTarih.isNull)
                filtre.sorguBasTarih = new TNSDateTime("01.01." + y);
            if (filtre.sorguBitTarih.isNull)
            {
                filtre.sorguBitTarih = new TNSDateTime(sorguBasTarih.ToString());
                filtre.sorguBitTarih.AddYears(1);
            }
            return filtre;
        }

        private static object ListeSatir(Fatura tt)
        {
            double kalanTutar = tt.genelToplam - tt.odenenToplam;
            string ek = tt.musteriAdiEk != "" ? " (" + tt.musteriAdiEk + ")" : "";
            return new
            {
                KOD = tt.kod,
                MUSTERIKOD = tt.musteriKod,
                FATURANO = tt.faturaNo,
                FATURATARIHI = TarihYaz(tt.faturaTarihi),
                IRSALIYENO = tt.irsaliyeNo,
                TOPLAM = tt.toplam,
                KDVTOPLAM = tt.kdvToplam,
                GENELTOPLAM = tt.genelToplam,
                ODEMETARIHI = TarihYaz(tt.odemeTarihi),
                MUSTERIADI = tt.musteriAdi + ek,
                GENELTOPLAMPARABIRIMI = Tutar(tt.genelToplam) + " " + tt.faturaParaBirimi,
                KALANTUTAR = Tutar(kalanTutar) + " " + tt.faturaParaBirimi,
                DURUM = DurumAd(tt.durum),
                DURUMKOD = tt.durum,
                MUHASEBEKOD = tt.muhasebeKod,
                BIRIMKOD = tt.birimKod,
                MUDURLUKKOD = tt.mudurlukKod,
                TAHAKKUKBELGENO = tt.tahakkukBelgeNo,
                ODEMEBELGENO = tt.odemeBelgeNo,
                ISLEMYAPAN = tt.islemYapan,
                ISLEMTARIHI = TarihYaz(tt.islemTarihi)
            };
        }

        /// <summary>Listele (eski btnListele_Click / Listele).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? muhasebe, string birim, string mudurluk, string faturaNo,
            string musteri, int? durum, string islemYapan, int? yil, string tarih1, string tarih2)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                Fatura filtre = Filtre(muhasebe, birim, mudurluk, faturaNo, musteri, durum, islemYapan, yil, tarih1, tarih2);
                ObjectArray bilgiler = Servis().FaturaListele(kullanan, filtre);
                if (bilgiler == null || !bilgiler.sonuc.islemSonuc) return Hata(HataMetni(bilgiler?.sonuc.hataStr, "Listeleme yapılamadı."));
                List<object> satirlar = new List<object>();
                foreach (Fatura tt in bilgiler.objeler) satirlar.Add(ListeSatir(tt));
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Listeleme yapılamadı.");
            }
        }

        /// <summary>Listeyi Yazdır (eski btnListeYazdir_Click): MUH/FaturaListe.xltx.</summary>
        private static IResult ListeYazdir(HttpContext baglam, IWebHostEnvironment ortam, int? muhasebe, string birim, string mudurluk, string faturaNo,
            string musteri, int? durum, string islemYapan, int? yil, string tarih1, string tarih2)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            string sablonAd = "FaturaListe.xltx";
            string sablon = SablonBul(sablonAd);
            if (sablon == null) return Hata("Rapor şablonu bulunamadı (MUH\\" + sablonAd + ").");
            try
            {
                Fatura filtre = Filtre(muhasebe, birim, mudurluk, faturaNo, musteri, durum, islemYapan, yil, tarih1, tarih2);
                ObjectArray bilgiler = Servis().FaturaListele(kullanan, filtre);
                if (bilgiler == null || !bilgiler.sonuc.islemSonuc) return Hata(HataMetni(bilgiler?.sonuc.hataStr, "Rapor hazırlanamadı."));

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                int kaynakSatir = 0;
                int sutun = 0;
                string sablonDosyaAc = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();

                XLS.DosyaAc(sablon, sablonDosyaAc);
                XLS.HucreAdAdresCoz("baslamaSatiri", ref kaynakSatir, ref sutun);

                foreach (Fatura item in bilgiler.objeler)
                {
                    double kalanTutar = item.genelToplam - item.odenenToplam;
                    string durumTipiAd = DurumAd(item.durum);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 10, kaynakSatir, sutun);
                    XLS.HucreDegerYaz(kaynakSatir, 0, item.faturaTarihi.ToString());
                    XLS.HucreDegerYaz(kaynakSatir, 1, item.faturaNo.ToString());
                    XLS.HucreDegerYaz(kaynakSatir, 2, item.musteriAdi);
                    XLS.HucreDegerYaz(kaynakSatir, 3, item.genelToplam.ToString("#,###.00"));
                    XLS.HucreDegerYaz(kaynakSatir, 4, item.odenenToplam.ToString("#,###.00"));
                    XLS.HucreDegerYaz(kaynakSatir, 5, kalanTutar.ToString("#,###.00"));
                    XLS.HucreDegerYaz(kaynakSatir, 6, durumTipiAd);

                    kaynakSatir++;
                    XLS.SatirAc(kaynakSatir, 1);
                }

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sablonDosyaAc, "FaturaListe", true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }

        /// <summary>Müdürlük seçimi (eski ListeMudurluk.aspx): aktif müdürlükler.</summary>
        private static IResult MudurlukListe(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray yler = Servis().MudurlukListele(kullanan, new Mudurluk { aktif = 1 }) /* yalnız aktif müdürlükler */;
                if (yler == null || !yler.sonuc.islemSonuc) return Hata(HataMetni(yler?.sonuc.hataStr, "Müdürlükler okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (Mudurluk d in yler.objeler)
                    satirlar.Add(new { kod = d.kod, ad = d.ad });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Müdürlükler okunamadı.");
            }
        }

        // ------------------------------------------------------------------ Fatura Giriş (eski FaturaGiris.aspx)

        private static object FormFatura(Fatura tt)
        {
            List<object> satirlar = new List<object>();
            foreach (FaturaDetay fd in tt.detay.objeler)
            {
                satirlar.Add(new
                {
                    FATURAKOD = fd.faturaKod,
                    ACIKLAMA = fd.aciklama,
                    MIKTAR = fd.miktar,
                    KDVORAN = fd.kdvOran,
                    BIRIMFIYAT = fd.birimFiyat,
                    TUTAR = fd.tutar,
                    HESAPKOD = fd.hesapKod
                });
            }
            return new
            {
                kod = tt.kod,
                musteriKod = tt.musteriKod,
                musteriAdi = tt.musteriAdi,
                musteriAdiEk = tt.musteriAdiEk,
                muhasebe = tt.muhasebeKod,
                birim = tt.birimKod,
                mudurluk = tt.mudurlukKod,
                aciklama = tt.aciklama,
                faturaNo = tt.faturaNo,
                faturaTarihi = TarihYaz(tt.faturaTarihi),
                faturaYil = tt.faturaTarihi.isNull ? 0 : tt.faturaTarihi.Yil,
                irsaliyeNo = tt.irsaliyeNo,
                irsaliyeTarihi = TarihYaz(tt.irsaliyeTarihi),
                kdv = Tutar(tt.kdvToplam),
                toplam = Tutar(tt.toplam),
                genelToplam = Tutar(tt.genelToplam),
                muhBelgeAciklama = tt.muhBelgeAciklama,
                tevkifat = tt.tevkifatOran,
                tahakkukBelgeNo = tt.tahakkukBelgeNo,
                odemeBelgeNo = tt.odemeBelgeNo,
                odemeYil = tt.odemeTarihi.isNull ? 0 : tt.odemeTarihi.Yil,
                // eski: tahakkuk var, ödeme yoksa "Tahakkuku aynı fişte üret" kapalı gelir
                tahakkukAyniFis = !(tt.tahakkukBelgeNo != "" && tt.odemeBelgeNo == ""),
                tahakkukHesapKod = tt.tahakkukHesapKod,
                odemeHesapKod = tt.odemeHesapKod,
                odemeTarih = !tt.odemeTarihi.isNull ? TarihYaz(tt.odemeTarihi) : DateTime.Today.ToString("yyyy-MM-dd"),
                durum = tt.durum,
                durumAd = DurumAd(tt.durum),
                satirlar = satirlar
            };
        }

        /// <summary>Açılış (eski Page_Load + Listele): kullanıcı değişkenleri, otomatik numara / tutar girişi ayarları, kayıtlı fatura.</summary>
        private static IResult FormTanim(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                string muhasebe = Degisken(kullanan, "MUHASEBE");
                string birim = Degisken(kullanan, "MUHASEBEBIRIM");
                string kurum = "";
                string birimAd = "";
                try { birimAd = MuhasebeBirimUclari.BirimAdBul(OrtakFonksiyonlar.ConvertToInt(muhasebe, 0), birim, out kurum); }
                catch (Exception) { birimAd = ""; }

                object fatura = null;
                int k = kod ?? 0;
                if (k != 0)
                {
                    Fatura t = new Fatura();
                    t.kod = k;
                    ObjectArray bilgiler = Servis().FaturaListele(kullanan, t);
                    if (bilgiler == null || !bilgiler.sonuc.islemSonuc) return Hata(HataMetni(bilgiler?.sonuc.hataStr, "Fatura okunamadı."));
                    foreach (Fatura tt in bilgiler.objeler) fatura = FormFatura(tt);
                }

                return Results.Json(new
                {
                    bugun = DateTime.Today.ToString("yyyy-MM-dd"),
                    yil = DateTime.Now.Year,
                    muhasebe = muhasebe, birim = birim, birimAd = birimAd,
                    kurumsal = MuhasebeBirimUclari.KurumsalYaz(DateTime.Now.Year, kurum),
                    mudurluk = Degisken(kullanan, "FATURAMUDURLUK"),
                    otoNumara = Ayar1("/MUHASEBE/MUHASEBEFATURAOTONUMARA"),
                    tutarGirisi = Ayar1("/MUHASEBE/MUHASEBEFATURATUTARGIRISI"),
                    hesaplamaBirimFiyat = Degisken(kullanan, "FATURAHESAPBIRIMFIYAT"),
                    hesaplamaYuzde = Degisken(kullanan, "FATURAHESAPYUZDE"),
                    fatura = fatura
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        private static bool MudurlukKontrol(Kullanici kullanan, string mudurluk)
        {
            Mudurluk kriter = new Mudurluk();
            kriter.kod = mudurluk;
            kriter.aktif = 1; // yalnız aktif müdürlük kabul edilir
            if (kriter.kod.Trim() == "") return false;
            ObjectArray yler = Servis().MudurlukListele(kullanan, kriter);
            return yler.objeler.Count != 0;
        }

        /// <summary>Kaydet (eski btnKaydet_Click): satır tutarları / birim fiyatları eski formüllerle hesaplanır.</summary>
        private static Task<IResult> FormKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, FaturaKaydetIstegi istek)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                if (istek == null) return Basarisiz("İstek okunamadı.");
                try
                {
                    string mudurluk = Temiz(istek.Mudurluk);
                    if (mudurluk == "" || !MudurlukKontrol(kullanan, mudurluk))
                        return Basarisiz("Müdürlük kod bilgisi hatalı girilmiş");

                    GenelIslemler.KullaniciDegiskenSakla(kullanan, "FATURAHESAPBIRIMFIYAT", istek.HesaplamaBirimFiyat ?? "");
                    GenelIslemler.KullaniciDegiskenSakla(kullanan, "FATURAHESAPYUZDE", istek.HesaplamaYuzde ?? "");

                    Fatura t = new Fatura();
                    t.kod = istek.Kod;
                    t.muhasebeKod = OrtakFonksiyonlar.ConvertToInt(istek.Muhasebe, 0);
                    t.birimKod = istek.Birim ?? "";
                    t.mudurlukKod = istek.Mudurluk ?? "";
                    t.musteriKod = istek.MusteriKod ?? "";
                    t.musteriAdi = istek.MusteriAdi ?? "";
                    t.musteriAdiEk = istek.MusteriAdiEk ?? "";
                    t.faturaNo = istek.FaturaNo ?? "";
                    t.faturaTarihi = Tarih(istek.FaturaTarihi);
                    t.irsaliyeNo = istek.IrsaliyeNo ?? "";
                    t.irsaliyeTarihi = Tarih(istek.IrsaliyeTarihi);
                    t.aciklama = Temiz(istek.Aciklama);
                    t.durum = (int)ENUMFaturaDurum.YENI;
                    t.tevkifatOran = istek.Tevkifat;
                    t.muhBelgeAciklama = Temiz(istek.MuhBelgeAciklama);
                    t.tahakkukBelgeNo = istek.TahakkukBelgeNo ?? "";
                    t.odemeBelgeNo = istek.OdemeBelgeNo ?? "";

                    bool tutarGirisi = Ayar1("/MUHASEBE/MUHASEBEFATURATUTARGIRISI");

                    double toplamKDV = 0.0;
                    double toplamTutar = 0.0;
                    ObjectArray fd = new ObjectArray();
                    foreach (FaturaDetayIstegi item in istek.Satirlar ?? new List<FaturaDetayIstegi>())
                    {
                        if (item == null) continue;
                        string hesapKod = item.HesapKod ?? "";

                        FaturaDetay fDetay = new FaturaDetay();
                        fDetay.kod = 0;
                        fDetay.faturaKod = item.FaturaKod;
                        fDetay.aciklama = (item.Aciklama ?? "").Trim();
                        // eski: ConvertToInt(KDVORAN)
                        fDetay.kdvOran = OrtakFonksiyonlar.ConvertToInt(item.KdvOran ?? 0, 0);
                        fDetay.miktar = item.Miktar ?? 0;
                        fDetay.birimFiyat = item.BirimFiyat ?? 0;
                        fDetay.tutar = item.Tutar ?? 0;
                        fDetay.hesapKod = hesapKod;

                        if (fDetay.hesapKod != "")
                            fDetay.hesapKod = fDetay.hesapKod.Split('-')[0].Trim();

                        double miktar = fDetay.miktar;
                        if (miktar == 0) miktar = 1;

                        if (tutarGirisi)
                        {
                            if (fDetay.tutar > 0)
                            {
                                fDetay.birimFiyat = fDetay.tutar / miktar;
                                fDetay.birimFiyat = fDetay.birimFiyat - (fDetay.birimFiyat * fDetay.kdvOran / (100 + fDetay.kdvOran));
                                fDetay.birimFiyat = Math.Round(fDetay.birimFiyat, 8);
                            }
                        }
                        else
                        {
                            if (fDetay.birimFiyat > 0)
                            {
                                fDetay.tutar = Math.Round((miktar * fDetay.birimFiyat) * ((fDetay.kdvOran / 100) + 1), 2);
                            }
                        }

                        fDetay.kdvTutar = Math.Round(fDetay.tutar - (miktar * fDetay.birimFiyat), 8);

                        toplamKDV += fDetay.kdvTutar;
                        toplamTutar += fDetay.tutar;

                        fd.objeler.Add(fDetay);
                    }
                    t.detay = fd;

                    t.toplam = OrtakFonksiyonlar.Yuvarla(toplamTutar, 2) - OrtakFonksiyonlar.Yuvarla(toplamKDV, 2);
                    t.kdvToplam = OrtakFonksiyonlar.Yuvarla(toplamKDV, 2);
                    t.genelToplam = OrtakFonksiyonlar.Yuvarla(toplamTutar, 2);
                    t.tevkifatTutar = t.kdvToplam * (OrtakFonksiyonlar.ConvertToDouble(t.tevkifatOran) / 10);

                    Sonuc sonuc = Servis().FaturaKaydet(kullanan, t);
                    if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "Kayıt yapılamadı."));

                    GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBE", istek.Muhasebe ?? "");
                    GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBEBIRIM", istek.Birim ?? "");
                    GenelIslemler.KullaniciDegiskenSakla(kullanan, "FATURAMUDURLUK", istek.Mudurluk ?? "");

                    return Results.Json(new { basarili = true, bilgi = "İstenilen bilgi başarıyla kayıt edildi.", kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0) });
                }
                catch (Exception)
                {
                    return Basarisiz("Kayıt yapılamadı.");
                }
            });
        }

        /// <summary>İptal (eski btnIptal_Click): FaturaDurumGuncelle (iş kuralı faturayı iptal eder).</summary>
        private static Task<IResult> FormIptal(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, FaturaKodIstegi istek)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                Fatura t = new Fatura();
                t.kod = istek?.Kod ?? 0;
                if (t.kod == 0) return Basarisiz("İlk önce kayıtlı fatura seçiniz.");
                try
                {
                    Sonuc sonuc = Servis().FaturaDurumGuncelle(kullanan, t);
                    if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "İptal işlemi yapılamadı."));
                    return Results.Json(new { basarili = true, bilgi = "İstenilen bilgi başarıyla iptal edildi." });
                }
                catch (Exception)
                {
                    return Basarisiz("İptal işlemi yapılamadı.");
                }
            });
        }

        private class SiralaKDV : IComparer
        {
            public int Compare(object o1, object o2)
            {
                if (o1 == o2) return 0;
                FaturaDetay i_o1 = (FaturaDetay)o1;
                FaturaDetay i_o2 = (FaturaDetay)o2;
                return (i_o1.kdvOran).CompareTo((i_o2.kdvOran));
            }
        }

        private static string TevkifatAdBul(int kod)
        {
            return OrtakFonksiyonlar.ConvertToInt(kod, 0) + "/10";
        }

        /// <summary>Yazdır (eski btnYazdir_Click): MUH/FaturaForm[kurum].dotx, yoksa FaturaForm.dotx; Word (docx).</summary>
        private static IResult FormYazdir(HttpContext baglam, IWebHostEnvironment ortam, int? kod, string muhasebe)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            Fatura f = new Fatura();
            f.kod = kod ?? 0;
            if (f.kod == 0) return Hata("İlk önce kayıtlı fatura seçiniz.");
            try
            {
                IMUHServis servisMUH = Servis();
                ObjectArray faturalar = servisMUH.FaturaListele(kullanan, f);
                if (faturalar.ObjeSayisi == 0) return Hata("Fatura bulunamadı.");
                f = (Fatura)faturalar.objeler[0];

                string kurum = GenelIslemlerIstemci.VarsayilanKurumBul().Replace(".", "");
                string sablonAd = "FaturaForm" + kurum + ".dotx";
                string dosyaAdYol = SablonBul(sablonAd);
                if (dosyaAdYol == null)
                {
                    sablonAd = "FaturaForm.dotx";
                    dosyaAdYol = SablonBul(sablonAd);
                }
                if (dosyaAdYol == null) return Hata("Rapor şablonu bulunamadı (MUH\\" + sablonAd + ").");

                TNS.HRC.FirmaBilgisi b = new TNS.HRC.FirmaBilgisi();
                b.muhasebe = OrtakFonksiyonlar.ConvertToInt(muhasebe, 0);
                b.vergiNo = f.musteriKod;
                b.aktifDurum = 1;
                ObjectArray listeFirma = TNS.HRC.Arac.Tanimla().FirmaBilgisiListele(kullanan, b);
                if (listeFirma.ObjeSayisi == 0) return Hata("Müşteri (kişi / firma) bilgisi bulunamadı.");
                TNS.HRC.FirmaBilgisi firma = (TNS.HRC.FirmaBilgisi)listeFirma.objeler[0];

                AsposeLisans.Yukle();
                string gonderilecekDosyaTmp = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
                Document doc = new Document(dosyaAdYol);
                DocumentBuilder builder = new DocumentBuilder(doc);

                if (f.musteriAdiEk != "") f.musteriAdiEk = " (" + f.musteriAdiEk + ")";
                firma.ad = firma.ad + f.musteriAdiEk;

                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ISIM", firma.ad);
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ADRES", firma.adres);
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "VERGIDAIRESI", firma.vergiDairesi);
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "VERGINO", firma.vergiNo);
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "FATURATARIHI", f.faturaTarihi.ToString());
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "FATURANO", f.faturaNo);
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "SONODEMETARIHI", "");

                string tutarParaBirimi = "";
                string yaziyla = OrtakFonksiyonlar.Yaziyla(f.genelToplam - f.tevkifatTutar);

                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "YAZIYLA", yaziyla);
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ACIKLAMA", f.aciklama);

                DataTable tabDetay = new DataTable();
                tabDetay.TableName = "Detay";
                tabDetay.Columns.Add("SIRANO");
                tabDetay.Columns.Add("CINSI");
                tabDetay.Columns.Add("MIKTARI");
                tabDetay.Columns.Add("KDV");
                tabDetay.Columns.Add("BIRIMFIYAT");
                tabDetay.Columns.Add("SATIRTOPLAM");

                ObjectArray kdvler = new ObjectArray();
                int siraNo = 1;
                int toplamSatir = 10;

                foreach (FaturaDetay fd in f.detay.objeler)
                {
                    DataRow row = tabDetay.NewRow();
                    row["SIRANO"] = siraNo.ToString();
                    row["CINSI"] = fd.aciklama;
                    row["MIKTARI"] = fd.miktar > 0 ? fd.miktar.ToString("#,###.##") : "";
                    row["KDV"] = fd.kdvOran > 0 ? fd.kdvOran.ToString() : "MUAF";
                    row["BIRIMFIYAT"] = fd.birimFiyat > 0 && fd.miktar > 0 ? fd.birimFiyat.ToString("#,###.00######") : "";
                    row["SATIRTOPLAM"] = fd.tutar > 0 ? Math.Round(fd.tutar - fd.kdvTutar, 2).ToString("#,###.00") : "";

                    siraNo++;

                    bool bulundu = false;
                    foreach (FaturaDetay kdv in kdvler.objeler)
                    {
                        if (kdv.kdvOran == fd.kdvOran)
                        {
                            kdv.kdvTutar += fd.kdvTutar;
                            bulundu = true;
                        }
                    }
                    if (!bulundu && fd.kdvOran > 0)
                    {
                        FaturaDetay yeniKDV = new FaturaDetay();
                        yeniKDV.kdvOran = fd.kdvOran;
                        yeniKDV.kdvTutar = fd.kdvTutar;
                        kdvler.objeler.Add(yeniKDV);
                    }

                    tabDetay.Rows.Add(row);
                }

                for (int i = siraNo; i < toplamSatir; i++)
                {
                    DataRow row = tabDetay.NewRow();
                    row["SIRANO"] = i.ToString();
                    tabDetay.Rows.Add(row);
                }

                kdvler.objeler.Sort(new SiralaKDV());

                DataTable tabKDV = new DataTable();
                tabKDV.TableName = "DetayKDV";
                tabKDV.Columns.Add("KDVORAN");
                tabKDV.Columns.Add("KDVTUTAR");
                foreach (FaturaDetay fd in kdvler.objeler)
                {
                    DataRow row = tabKDV.NewRow();
                    row["KDVORAN"] = fd.kdvOran;
                    row["KDVTUTAR"] = fd.kdvTutar.ToString("#,###.00");
                    tabKDV.Rows.Add(row);
                }
                if (tabKDV.Rows.Count == 0)
                {
                    DataRow row = tabKDV.NewRow();
                    row["KDVORAN"] = 0;
                    row["KDVTUTAR"] = "-";
                    tabKDV.Rows.Add(row);
                }

                doc.MailMerge.ExecuteWithRegions(tabDetay);
                doc.MailMerge.ExecuteWithRegions(tabKDV);

                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ALTTOPLAM", f.toplam.ToString("#,###.00") + " " + tutarParaBirimi);
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "GENELTOPLAM", (f.genelToplam - f.tevkifatTutar).ToString("#,###.00") + " " + tutarParaBirimi);
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ODENECEKTUTAR", (f.genelToplam - f.tevkifatTutar).ToString("#,###.00") + " " + tutarParaBirimi);
                string tevkifatAlan;
                if (f.tevkifatOran > 0)
                {
                    tevkifatAlan = string.Format("Toplam\t{0}\r", f.toplam.ToString("#,###.00"));
                    foreach (FaturaDetay fd in kdvler.objeler)
                        tevkifatAlan += string.Format("Tevkifat öncesi KDV (" + fd.kdvOran + "%)\t{0}\r", fd.kdvTutar.ToString("#,###.00"));

                    if (kdvler.objeler.Count > 1)
                        tevkifatAlan += string.Format("Tevkifat öncesi KDV (Toplam)\t{0}\r", f.kdvToplam.ToString("#,###.00"));

                    tevkifatAlan += string.Format("KDV Dahil Tutar\t{0}\r", f.genelToplam.ToString("#,###.00"));
                    tevkifatAlan += string.Format("KDV Tevkifatı ({0})\t{1}\r", TevkifatAdBul(f.tevkifatOran), f.tevkifatTutar.ToString("#,###.00"));

                    tevkifatAlan += string.Format("    -----------------------------------------------\r");
                    tevkifatAlan += string.Format("Toplam\t{0}\r", f.toplam.ToString("#,###.00"));
                    tevkifatAlan += string.Format("KDV Tutarı\t{0}\r", (f.kdvToplam - f.tevkifatTutar).ToString("#,###.00"));
                    tevkifatAlan += string.Format("Genel Toplam\t{0}\r", (f.genelToplam - f.tevkifatTutar).ToString("#,###.00"));
                }
                else
                {
                    tevkifatAlan = string.Format("Toplam\t{0}\r", f.toplam.ToString("#,###.00"));
                    foreach (FaturaDetay fd in kdvler.objeler)
                        tevkifatAlan += string.Format("KDV Tutarı (" + fd.kdvOran + "%)\t{0}\r", fd.kdvTutar.ToString("#,###.00"));

                    if (kdvler.objeler.Count > 1)
                        tevkifatAlan += string.Format("KDV Tutarı (Toplam)\t{0}\r", f.kdvToplam.ToString("#,###.00"));

                    tevkifatAlan += string.Format("Genel Toplam\t{0}\r", f.genelToplam.ToString("#,###.00"));
                }
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TEVKIFAT", tevkifatAlan);

                doc.Save(gonderilecekDosyaTmp, SaveFormat.Docx);
                return DosyaIslem.DosyaGonder(gonderilecekDosyaTmp, "Fatura", true, "docx");
            }
            catch (Exception)
            {
                return Hata("Fatura yazdırılamadı.");
            }
        }

        /// <summary>Faturanın birimine ait kurumsal kod (eski KullaniciMuhasebeBirimleri döngüsü).</summary>
        private static string KurumsalKodBul(IMUHServis servisMUH, Kullanici kullanan, Fatura tt)
        {
            string kurumsalKod = "";
            ObjectArray mv = servisMUH.KullaniciMuhasebeBirimleri(kullanan, tt.muhasebeKod, TNS.MUH.Arac.VarsayilanKurumKod());
            foreach (MuhasebeBirim m in mv.objeler)
            {
                if (m.birimKod == tt.birimKod)
                    kurumsalKod = m.kurum.Replace(".", "");
            }
            return kurumsalKod;
        }

        private static OdemeEmriMIFDetay YeniSatir(int yil)
        {
            return new OdemeEmriMIFDetay(yil);
        }

        /// <summary>Eski BelgeyiGoster / OdemeEmriMIFKaydet sonucu: belge numarası bağlantısı JS'te çizilir.</summary>
        private static IResult BelgeSonucu(int yil, string belgeNo, int belgeTur)
        {
            return Results.Json(new
            {
                basarili = true,
                bilgi = "Ödeme Emri/MİF kaydı yapıldı. Belge Numarası :",
                yil = yil,
                belgeNo = belgeNo,
                belgeTur = belgeTur
            });
        }

        /// <summary>
        /// Tahakkuk / Ödeme MİF Oluştur (eski FaturaGiris btnOlustur_Click). Eski kodun hesap satırları ve 800/830/805/835 satırları
        /// aynen kurulur. Tek fark: 600'lü satır son satırsa eski kod sonraki satıra bakarken hata veriyordu, burada sınır denetlenir.
        /// </summary>
        private static Task<IResult> FormBelge(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, FaturaBelgeIstegi istek)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                if (istek == null) return Basarisiz("İstek okunamadı.");
                try
                {
                    IMUHServis servisMUH = Servis();
                    TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();

                    Fatura aciklamaMuh = new Fatura();
                    aciklamaMuh.kod = istek.Kod;
                    aciklamaMuh.muhBelgeAciklama = istek.MuhBelgeAciklama ?? "";
                    if (aciklamaMuh.kod == 0) return Basarisiz("İlk önce kayıtlı fatura seçiniz.");
                    servisMUH.FaturaMuhasebeAciklamaKaydet(kullanan, aciklamaMuh);

                    string hesapKodTahakkuk = istek.TahakkukHesapKod ?? "";
                    string hesapKodOdeme = istek.OdemeHesapKod ?? "";
                    int belgeTur = istek.BelgeTur;
                    int damga = istek.Damga <= 0 ? 1 : istek.Damga;
                    bool chkTahakkuk = istek.TahakkukAyniFis;
                    bool chkDamgaVergisi = istek.SadeceDamga;
                    string belgeNo = belgeTur == 1 ? (istek.TahakkukBelgeNo ?? "") : (istek.OdemeBelgeNo ?? "");
                    TNSDateTime belgeTarih = Tarih(istek.Tarih);

                    if (!chkDamgaVergisi)
                    {
                        if (string.IsNullOrEmpty(hesapKodTahakkuk.Trim()) && (chkTahakkuk || belgeTur == 1))
                            return Basarisiz("Tahakkuk Hesap kodunu boş bırakamazsınız");
                        if (string.IsNullOrEmpty(hesapKodOdeme.Trim()) && belgeTur == 2)
                            return Basarisiz("Ödeme Hesap kodunu boş bırakamazsınız");
                    }

                    if (belgeTarih.isNull) return Basarisiz("Ödeme tarihini boş bırakamazsınız");

                    int faturaKod = 0;
                    string vergiNo = "";
                    int muhasebKod = 0;
                    string birimKod = "";
                    double genelToplam = 0.0;
                    double genelToplamKDVHaric = 0.0;
                    string faturaNo = "";
                    string aciklama = "";
                    string faturaTarih = "";
                    string kurumsalKod = "";
                    double tevkifatOrani = 0;
                    List<FaturaDetay> detay = new List<FaturaDetay>();

                    Fatura t = new Fatura();
                    t.kod = istek.Kod;
                    ObjectArray bilgiler = servisMUH.FaturaListele(kullanan, t);
                    foreach (Fatura tt in bilgiler.objeler)
                    {
                        kurumsalKod = KurumsalKodBul(servisMUH, kullanan, tt);

                        faturaKod = tt.kod;
                        vergiNo = tt.musteriKod;
                        muhasebKod = tt.muhasebeKod;
                        birimKod = tt.birimKod;
                        faturaNo = tt.faturaNo;
                        faturaTarih = tt.faturaTarihi.ToString();
                        genelToplam = tt.genelToplam - tt.tevkifatTutar;
                        aciklama = tt.muhBelgeAciklama + "\n" + "Fatura Tarihi:" + tt.faturaTarihi.ToString() + ", Fatura No:" + faturaNo;
                        tevkifatOrani = OrtakFonksiyonlar.ConvertToDbl(tt.tevkifatOran);

                        if (tt.musteriAdiEk != "") tt.musteriAdiEk = " (" + tt.musteriAdiEk + ")";
                        aciklama += "\n" + tt.musteriAdiEk;

                        foreach (FaturaDetay fd in tt.detay.objeler)
                        {
                            bool bulundu = false;
                            foreach (FaturaDetay fdd in detay)
                            {
                                if (fdd.hesapKod == fd.hesapKod && fdd.kdvOran == fd.kdvOran)
                                {
                                    bulundu = true;
                                    fdd.tutar += fd.tutar;
                                    fdd.kdvTutar += fd.kdvTutar;
                                }
                            }
                            if (!bulundu)
                                detay.Add(fd);
                        }
                    }

                    OdemeEmriMIFForm o = new OdemeEmriMIFForm();
                    TNS.HRC.FirmaBilgisi f = servisHRC.FirmaBilgisiListele(kullanan, OrtakFonksiyonlar.ConvertToInt(istek.Muhasebe, 0), vergiNo);
                    if (f != null)
                    {
                        o.ilgiliAd = f.ad;
                        o.ilgiliBankaAd = f.banka;
                        o.ilgiliBankaNo = f.hesapNo;
                        o.ilgiliVD = f.vergiDairesi;
                        o.ilgiliNo = f.vergiNo;
                    }

                    o.yil = DateTime.Now.Year;
                    o.birim = birimKod;
                    o.kurKod = kurumsalKod;
                    o.muhasebe = muhasebKod;
                    o.belgeNo = belgeNo;
                    o.islemTarih = belgeTarih;
                    o.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
                    o.formDurumTarih = new TNSDateTime(DateTime.Now);
                    o.islemYapan = kullanan.kullaniciKodu;
                    o.islemYapanAd = kullanan.ad + " " + kullanan.soyad;
                    o.aciklama = aciklama;

                    foreach (FaturaDetay fdd in detay)
                        genelToplamKDVHaric += Math.Round(fdd.tutar - fdd.kdvTutar, 2);

                    string satirAciklama = "Fat.Tar:" + faturaTarih + " Fat.No:" + faturaNo;
                    OdemeEmriMIFDetay td;

                    if (belgeTur == 1 || chkTahakkuk)
                    {
                        td = YeniSatir(o.yil);
                        td.hesapKod = hesapKodTahakkuk.Split('-')[0].Trim();
                        td.borc = Math.Round(genelToplam, 2);
                        td.alacak = 0;
                        td.detayBirim = birimKod + "." + kurumsalKod;
                        td.kur = kurumsalKod;
                        td.satirAciklama = satirAciklama;
                        o.detay.objeler.Add(td);

                        if (belgeTur == 1 && istek.TahakkukTahsilat)
                        {
                            td = YeniSatir(o.yil);
                            td.hesapKod = hesapKodTahakkuk.Split('-')[0].Trim();
                            td.borc = 0;
                            td.alacak = Math.Round(genelToplam, 2);
                            td.detayBirim = birimKod + "." + kurumsalKod;
                            td.kur = kurumsalKod;
                            td.satirAciklama = satirAciklama;
                            o.detay.objeler.Add(td);
                        }
                        else
                        {
                            foreach (FaturaDetay fdd in detay)
                            {
                                td = YeniSatir(o.yil);
                                td.hesapKod = fdd.hesapKod;
                                td.borc = 0;
                                td.alacak = Math.Round(fdd.tutar - fdd.kdvTutar, 2);
                                td.detayBirim = birimKod + "." + kurumsalKod;
                                td.kur = kurumsalKod;
                                td.satirAciklama = satirAciklama;
                                o.detay.objeler.Add(td);

                                if (fdd.kdvOran > 0)
                                {
                                    string kdvHesapKod = "";
                                    if (fdd.kdvOran == 18) kdvHesapKod = "391.01";
                                    if (fdd.kdvOran == 8) kdvHesapKod = "391.02";
                                    if (fdd.kdvOran == 1) kdvHesapKod = "391.03";

                                    td = YeniSatir(o.yil);
                                    td.hesapKod = kdvHesapKod;
                                    td.borc = 0;
                                    td.alacak = Math.Round(fdd.kdvTutar, 2);
                                    td.detayBirim = birimKod + "." + kurumsalKod;
                                    td.kur = kurumsalKod;
                                    td.satirAciklama = satirAciklama;
                                    o.detay.objeler.Add(td);

                                    if (tevkifatOrani > 0)
                                    {
                                        td = YeniSatir(o.yil);
                                        td.hesapKod = TevkifatHesapKoduBul(OrtakFonksiyonlar.ConvertToInt(tevkifatOrani, 0));
                                        td.borc = Math.Round(fdd.kdvTutar * (tevkifatOrani / 10), 2);
                                        td.alacak = 0;
                                        td.detayBirim = birimKod + "." + kurumsalKod;
                                        td.kur = kurumsalKod;
                                        td.satirAciklama = satirAciklama;
                                        o.detay.objeler.Add(td);
                                    }
                                }
                            }
                        }
                    }

                    double dgVergiOran = 0.00948;
                    double dgVergi = 0.0;
                    double dgVergiKesinti = 0.0;
                    if (belgeTur == 2 && damga != 3)
                    {
                        dgVergi = Math.Round(genelToplamKDVHaric * dgVergiOran, 2);
                        dgVergiKesinti = dgVergi;
                    }
                    if (damga == 2)
                        dgVergiKesinti = 0.0;

                    if (belgeTur == 2 && !chkDamgaVergisi)
                    {
                        td = YeniSatir(o.yil);
                        td.hesapKod = hesapKodTahakkuk.Split('-')[0].Trim();
                        td.borc = 0;
                        td.alacak = Math.Round(genelToplam, 2);
                        td.detayBirim = birimKod + "." + kurumsalKod;
                        td.kur = kurumsalKod;
                        td.satirAciklama = satirAciklama;
                        o.detay.objeler.Add(td);

                        td = YeniSatir(o.yil);
                        td.hesapKod = hesapKodOdeme.Split('-')[0].Trim();
                        td.borc = Math.Round(genelToplam, 2) - dgVergiKesinti;
                        td.alacak = 0;
                        td.detayBirim = birimKod + "." + kurumsalKod;
                        td.kur = kurumsalKod;
                        td.satirAciklama = satirAciklama;
                        o.detay.objeler.Add(td);
                    }

                    if (belgeTur == 2)
                    {
                        if (damga == 2) // Damga vergisi kesilmemiş
                        {
                            td = YeniSatir(o.yil);
                            td.hesapKod = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/FATURADAMGAVERGISIHESAPKOD");
                            td.borc = 0;
                            td.alacak = dgVergi;
                            td.detayBirim = birimKod + "." + kurumsalKod;
                            td.kur = kurumsalKod;
                            td.satirAciklama = satirAciklama;
                            td.altHesapKod = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/FATURADAMGAVERGISIALTHESAP");
                            o.detay.objeler.Add(td);
                        }

                        if (damga != 3) // Damga vergisi yok işaretli değilse
                        {
                            td = YeniSatir(o.yil);
                            td.hesapKod = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/FATURADAMGAVERGISIGIDERHESAPKOD");
                            td.TertipAta(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/FATURADAMGAVERGISITERTIP"));
                            td.detayBirim = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/FATURADAMGAVERGISIBIRIM") + "." + td.kur;
                            if (td.detayBirim != "")
                            {
                                o.birim = "000";
                                o.kurKod = "";
                            }
                            td.borc = dgVergi;
                            td.alacak = 0;
                            td.satirAciklama = satirAciklama;
                            o.detay.objeler.Add(td);
                        }
                    }

                    double toplam800 = 0.0;
                    double toplam830 = 0.0;
                    string birim800 = "";
                    string birim830 = "";
                    string kur800 = "";
                    string kur830 = "";

                    TNSCollection detayListe = new TNSCollection(); // o.detay'a yeni satır ekleneceği için kopyası üzerinde dönülür
                    foreach (OdemeEmriMIFDetay ddd in o.detay.objeler)
                        detayListe.Add(ddd);

                    if (belgeTur == 2)
                    {
                        for (int i = 0; i < detayListe.Count; i++)
                        {
                            OdemeEmriMIFDetay ddd = (OdemeEmriMIFDetay)detayListe[i];

                            if (ddd.hesapKod.StartsWith("600") && !chkDamgaVergisi)
                            {
                                double kdv = 0.0;
                                double tevkifat = 0.0;
                                if (detayListe.Count > i + 1 && ((OdemeEmriMIFDetay)detayListe[i + 1]).hesapKod.StartsWith("391"))
                                {
                                    kdv = ((OdemeEmriMIFDetay)detayListe[i + 1]).alacak;
                                    // Tevkifat varsa KDV'den sonraki satır
                                    if (detayListe.Count > i + 2 && ((OdemeEmriMIFDetay)detayListe[i + 2]).hesapKod.StartsWith("360.04.02"))
                                        tevkifat = ((OdemeEmriMIFDetay)detayListe[i + 2]).borc;
                                }

                                td = YeniSatir(o.yil);
                                td.hesapKod = ddd.hesapKod.Replace("600", "800");
                                td.borc = 0;
                                td.alacak = ddd.alacak + (kdv - tevkifat);
                                td.detayBirim = ddd.detayBirim;
                                td.kur = ddd.kur;
                                o.detay.objeler.Add(td);

                                birim800 = ddd.detayBirim;
                                kur800 = ddd.kur;
                                toplam800 += Math.Round(ddd.alacak + (kdv - tevkifat), 2);
                            }
                            if (ddd.hesapKod.StartsWith("630"))
                            {
                                td = YeniSatir(o.yil);
                                td.hesapKod = ddd.hesapKod.Replace("630", "830");
                                td.TertipAta(ddd.TertipStr());
                                td.borc = Math.Round(ddd.borc, 2);
                                td.alacak = 0;
                                td.detayBirim = ddd.detayBirim;
                                td.kur = ddd.kur;
                                o.detay.objeler.Add(td);

                                birim830 = ddd.detayBirim;
                                kur830 = ddd.kur;
                                toplam830 += ddd.borc;
                            }
                        }
                    }

                    if (toplam800 > 0)
                    {
                        td = YeniSatir(o.yil);
                        td.hesapKod = "805";
                        td.borc = Math.Round(toplam800, 2);
                        td.alacak = 0;
                        td.detayBirim = birim800;
                        td.kur = kur800;
                        o.detay.objeler.Add(td);
                    }

                    if (toplam830 > 0)
                    {
                        td = YeniSatir(o.yil);
                        td.hesapKod = "835";
                        td.borc = 0;
                        td.alacak = Math.Round(toplam830, 2);
                        td.detayBirim = birim830;
                        td.kur = kur830;
                        o.detay.objeler.Add(td);
                    }

                    Sonuc sonuc = servisMUH.OdemeEmriMIFKaydet(kullanan, o);
                    if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "Belge oluşturulamadı."));

                    Fatura fGuncelle = new Fatura();
                    fGuncelle.kod = faturaKod;
                    if (belgeTur == 1)
                    {
                        fGuncelle.tahakkukBelgeNo = sonuc.anahtar;
                        fGuncelle.tahakkukHesapKod = hesapKodTahakkuk;
                    }
                    else
                    {
                        fGuncelle.odemeBelgeNo = sonuc.anahtar;
                        fGuncelle.odemeTarihi = belgeTarih;
                        fGuncelle.tahakkukHesapKod = hesapKodTahakkuk;
                        fGuncelle.odemeHesapKod = hesapKodOdeme;
                    }
                    servisMUH.FaturaMIFBelgeNoGuncelle(kullanan, fGuncelle);

                    return BelgeSonucu(o.yil, sonuc.anahtar, belgeTur);
                }
                catch (Exception ex)
                {
                    return Basarisiz(HataMetni(ex.Message, "Belge oluşturulamadı."));
                }
            });
        }

        /// <summary>Eski FaturaTahakkukIslemleri.TevkifatHesapKoduBul (yalnız 460604 kurumu için hesap kodu verir).</summary>
        public static string TevkifatHesapKoduBul(int tevkifatOran)
        {
            string hesapKod = "";
            string kurumKod = TNS.MUH.Arac.VarsayilanKurumKod().Replace(".", "");

            if (kurumKod == "460604")
            {
                if (tevkifatOran == 2)
                    hesapKod = "360.04.02.07";
                else if (tevkifatOran == 3)
                    hesapKod = "360.04.02.11";
                else if (tevkifatOran == 5)
                    hesapKod = "360.04.02.08";
                else if (tevkifatOran == 7)
                    hesapKod = "360.04.02.09";
                else if (tevkifatOran == 9)
                    hesapKod = "360.04.02.06";
                else if (tevkifatOran == 10)
                    hesapKod = "360.04.02.10";
            }

            return hesapKod;
        }

        // ------------------------------------------------------------------ fatura açıklamaları (eski wndFaturaAciklama)

        private static IResult AciklamaListe(HttpContext baglam, IWebHostEnvironment ortam, int? tur)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                FaturaAciklama t = new FaturaAciklama();
                t.tur = tur ?? 0;
                ObjectArray bilgiler = Servis().FaturaAciklamaListele(kullanan, t);
                List<object> satirlar = new List<object>();
                if (bilgiler != null && bilgiler.sonuc.islemSonuc)
                    foreach (FaturaAciklama tt in bilgiler.objeler)
                        satirlar.Add(new { KOD = tt.kod, ACIKLAMA = tt.aciklama });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Açıklamalar okunamadı.");
            }
        }

        private static Task<IResult> AciklamaKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, FaturaAciklamaIstegi istek)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                if (istek == null) return Basarisiz("İstek okunamadı.");
                try
                {
                    FaturaAciklama t = new FaturaAciklama();
                    t.kod = istek.Kod;
                    t.aciklama = istek.Aciklama ?? "";
                    t.tur = istek.Tur;
                    Sonuc sonuc = Servis().FaturaAciklamaKaydet(kullanan, t);
                    if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "Kayıt yapılamadı."));
                    return Results.Json(new { basarili = true, bilgi = "İstenilen bilgi başarıyla kayıt edildi." });
                }
                catch (Exception)
                {
                    return Basarisiz("Kayıt yapılamadı.");
                }
            });
        }

        private static Task<IResult> AciklamaSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, FaturaAciklamaIstegi istek)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                if (istek == null || istek.Kod == 0) return Basarisiz("Silinecek açıklamayı seçiniz.");
                try
                {
                    FaturaAciklama t = new FaturaAciklama();
                    t.kod = istek.Kod;
                    Sonuc sonuc = Servis().FaturaAciklamaSil(kullanan, t);
                    if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "Silme yapılamadı."));
                    return Results.Json(new { basarili = true, bilgi = "İstenilen bilgi başarıyla silindi." });
                }
                catch (Exception)
                {
                    return Basarisiz("Silme yapılamadı.");
                }
            });
        }

        // ------------------------------------------------------------------ Toplu Tahakkuk (eski FaturaTahakkukIslemleri.aspx)

        /// <summary>Eski Listele: Fatura Hazırlama'nın kriterleriyle (adres satırından) faturalar; yıl en son fatura tarihinin yılı.</summary>
        private static IResult TopluListe(HttpContext baglam, IWebHostEnvironment ortam, int? muhasebe, string birim, string mudurluk, string faturaNo,
            string musteri, int? durum, int? yil, string tarih1, string tarih2)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                Fatura filtre = Filtre(muhasebe, birim, mudurluk, faturaNo, musteri, durum, "", yil, tarih1, tarih2);
                ObjectArray bilgiler = Servis().FaturaListele(kullanan, filtre);
                if (bilgiler == null || !bilgiler.sonuc.islemSonuc) return Hata(HataMetni(bilgiler?.sonuc.hataStr, "Listeleme yapılamadı."));
                List<object> satirlar = new List<object>();
                foreach (Fatura tt in bilgiler.objeler)
                {
                    satirlar.Add(new
                    {
                        KOD = tt.kod,
                        MUSTERIKOD = tt.musteriKod,
                        FATURANO = tt.faturaNo,
                        FATURATARIHI = TarihYaz(tt.faturaTarihi),
                        MUSTERIADI = tt.musteriAdi,
                        GENELTOPLAMPARABIRIMI = Tutar(tt.genelToplam) + " " + tt.faturaParaBirimi,
                        DURUM = DurumAd(tt.durum),
                        MUHASEBEKOD = tt.muhasebeKod,
                        BIRIMKOD = tt.birimKod
                    });
                    if (tt.faturaTarihi.Oku() > filtre.faturaTarihi.Oku())
                        filtre.faturaTarihi = tt.faturaTarihi;
                }
                return Results.Json(new
                {
                    satirlar = satirlar,
                    yil = filtre.faturaTarihi.isNull ? 0 : filtre.faturaTarihi.Yil,
                    bugun = DateTime.Today.ToString("yyyy-MM-dd"),
                    hesapYil = DateTime.Now.Year
                });
            }
            catch (Exception)
            {
                return Hata("Listeleme yapılamadı.");
            }
        }

        /// <summary>Belge Oluştur (eski FaturaTahakkukIslemleri btnOlustur_Click): seçilen faturalardan tek tahakkuk / ödeme MİF'i.</summary>
        private static Task<IResult> TopluBelge(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, FaturaBelgeIstegi istek)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                if (istek == null || istek.Faturalar == null || istek.Faturalar.Count == 0)
                    return Basarisiz("En az bir fatura seçilmiş olmalı");
                try
                {
                    IMUHServis servisMUH = Servis();
                    TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();

                    List<int> listeFaturaKod = new List<int>();
                    string hesapKodTahakkuk = istek.TahakkukHesapKod ?? "";
                    string hesapKodOdeme = istek.OdemeHesapKod ?? "";
                    int belgeTur = istek.BelgeTur;
                    int damga = istek.Damga <= 0 ? 1 : istek.Damga;
                    bool chkTahakkuk = istek.TahakkukAyniFis;
                    bool chkDamgaVergisi = istek.SadeceDamga;

                    string muhtelif = "";
                    string musteriKod = "";
                    string faturaStr = "";
                    foreach (FaturaSeciliSatir jc in istek.Faturalar)
                    {
                        if (jc == null) continue;
                        listeFaturaKod.Add(jc.KOD);

                        if (faturaStr != "") faturaStr += ", ";
                        faturaStr += jc.FATURANO ?? "";

                        string m = jc.MUSTERIKOD ?? "";
                        if (m == "" || (m != "" && musteriKod != "" && m != musteriKod))
                            muhtelif = "Muhtelif Kişi/Firma";
                        musteriKod = m;
                    }

                    string belgeNo = belgeTur == 1 ? (istek.TahakkukBelgeNo ?? "") : (istek.OdemeBelgeNo ?? "");
                    TNSDateTime belgeTarih = Tarih(istek.Tarih);

                    if (!chkDamgaVergisi)
                    {
                        if (string.IsNullOrEmpty(hesapKodTahakkuk.Trim()) && (chkTahakkuk || belgeTur == 1))
                            return Basarisiz("Tahakkuk Hesap kodunu boş bırakamazsınız");
                        if (string.IsNullOrEmpty(hesapKodOdeme.Trim()) && belgeTur == 2)
                            return Basarisiz("Ödeme Hesap kodunu boş bırakamazsınız");
                    }

                    if (belgeTarih.isNull) return Basarisiz("Tarihini boş bırakamazsınız");

                    string vergiNo = "";
                    int muhasebKod = 0;
                    string birimKod = "";
                    double genelToplam = 0.0;
                    double genelToplamKDVHaric = 0.0;
                    string aciklama = "";
                    string kurumsalKod = "";

                    Fatura t = new Fatura();
                    t.soruguKod = listeFaturaKod.ToArray();
                    if (t.soruguKod.Length == 0) return Basarisiz("En az bir fatura seçilmiş olmalı");
                    ObjectArray bilgiler = servisMUH.FaturaListele(kullanan, t);
                    foreach (Fatura tt in bilgiler.objeler)
                    {
                        kurumsalKod = KurumsalKodBul(servisMUH, kullanan, tt);

                        vergiNo = tt.musteriKod;
                        muhasebKod = tt.muhasebeKod;
                        birimKod = tt.birimKod;
                        aciklama = "Fatura No:" + faturaStr;

                        // Aynı hesabı içeren satırlar tek satıra indirilir
                        ObjectArray detay = new ObjectArray();
                        foreach (FaturaDetay fd in tt.detay.objeler)
                        {
                            bool bulundu = false;
                            foreach (FaturaDetay fdd in detay.objeler)
                            {
                                if (fdd.hesapKod == fd.hesapKod && fdd.kdvOran == fd.kdvOran)
                                {
                                    bulundu = true;
                                    fdd.tutar += fd.tutar;
                                    fdd.kdvTutar += fd.kdvTutar;
                                }
                            }
                            if (!bulundu)
                                detay.objeler.Add(fd);
                        }
                        tt.detay = detay;
                    }

                    OdemeEmriMIFForm o = new OdemeEmriMIFForm();
                    if (muhtelif == "")
                    {
                        TNS.HRC.FirmaBilgisi f = servisHRC.FirmaBilgisiListele(kullanan, muhasebKod, vergiNo);
                        if (f != null)
                        {
                            o.ilgiliAd = f.ad;
                            o.ilgiliBankaAd = f.banka;
                            o.ilgiliBankaNo = f.hesapNo;
                            o.ilgiliVD = f.vergiDairesi;
                            o.ilgiliNo = f.vergiNo;
                        }
                    }
                    else
                    {
                        o.ilgiliAd = muhtelif;
                    }

                    o.yil = DateTime.Now.Year;
                    o.birim = birimKod;
                    o.kurKod = kurumsalKod;
                    o.muhasebe = muhasebKod;
                    o.belgeNo = belgeNo;
                    o.islemTarih = belgeTarih;
                    o.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
                    o.formDurumTarih = new TNSDateTime(DateTime.Now);
                    o.islemYapan = kullanan.kullaniciKodu;
                    o.islemYapanAd = kullanan.ad + " " + kullanan.soyad;
                    o.aciklama = aciklama;

                    OdemeEmriMIFDetay td;
                    foreach (Fatura tt in bilgiler.objeler)
                    {
                        tt.genelToplam = tt.genelToplam - tt.tevkifatTutar;
                        string satirAciklama = "Fat.Tar:" + tt.faturaTarihi.ToString() + " Fat.No:" + tt.faturaNo;

                        if (belgeTur == 1 || chkTahakkuk)
                        {
                            td = YeniSatir(o.yil);
                            td.hesapKod = hesapKodTahakkuk.Split('-')[0].Trim();
                            td.borc = Math.Round(tt.genelToplam, 2);
                            td.alacak = 0;
                            td.detayBirim = birimKod + "." + kurumsalKod;
                            td.kur = kurumsalKod;
                            td.altHesapKod = tt.musteriKod;
                            td.satirAciklama = satirAciklama;
                            o.detay.objeler.Add(td);

                            if (belgeTur == 1 && istek.TahakkukTahsilat)
                            {
                                td = YeniSatir(o.yil);
                                td.hesapKod = hesapKodTahakkuk.Split('-')[0].Trim();
                                td.borc = 0;
                                td.alacak = Math.Round(tt.genelToplam, 2);
                                td.detayBirim = birimKod + "." + kurumsalKod;
                                td.kur = kurumsalKod;
                                td.altHesapKod = tt.musteriKod;
                                td.satirAciklama = satirAciklama;
                                o.detay.objeler.Add(td);
                            }
                            else
                            {
                                foreach (FaturaDetay fdd in tt.detay.objeler)
                                {
                                    td = YeniSatir(o.yil);
                                    td.hesapKod = fdd.hesapKod;
                                    td.borc = 0;
                                    td.alacak = Math.Round(fdd.tutar - fdd.kdvTutar, 2);
                                    td.detayBirim = birimKod + "." + kurumsalKod;
                                    td.kur = kurumsalKod;
                                    td.altHesapKod = tt.musteriKod;
                                    td.satirAciklama = satirAciklama;
                                    o.detay.objeler.Add(td);

                                    if (fdd.kdvOran > 0)
                                    {
                                        string kdvHesapKod = "";
                                        if (fdd.kdvOran == 18) kdvHesapKod = "391.01";
                                        if (fdd.kdvOran == 8) kdvHesapKod = "391.02";
                                        if (fdd.kdvOran == 1) kdvHesapKod = "391.03";

                                        td = YeniSatir(o.yil);
                                        td.hesapKod = kdvHesapKod;
                                        td.borc = 0;
                                        td.alacak = Math.Round(fdd.kdvTutar, 2);
                                        td.detayBirim = birimKod + "." + kurumsalKod;
                                        td.kur = kurumsalKod;
                                        td.satirAciklama = satirAciklama;
                                        o.detay.objeler.Add(td);

                                        if (tt.tevkifatOran > 0)
                                        {
                                            td = YeniSatir(o.yil);
                                            td.hesapKod = TevkifatHesapKoduBul(tt.tevkifatOran);
                                            td.borc = Math.Round(fdd.kdvTutar * ((tt.tevkifatOran * 1.00) / 10), 2);
                                            td.alacak = 0;
                                            td.detayBirim = birimKod + "." + kurumsalKod;
                                            td.kur = kurumsalKod;
                                            td.satirAciklama = satirAciklama;
                                            o.detay.objeler.Add(td);
                                        }
                                    }
                                }
                            }
                        }

                        genelToplam = tt.genelToplam; // yukarıda tevkifat düşüldü
                        genelToplamKDVHaric = 0;
                        foreach (FaturaDetay fd in tt.detay.objeler)
                            genelToplamKDVHaric += Math.Round(fd.tutar - fd.kdvTutar, 2);

                        double dgVergiOran = 0.00948;
                        double dgVergi = 0.0;
                        double dgVergiKesinti = 0.0;
                        if (belgeTur == 2 && damga != 3)
                        {
                            dgVergi = Math.Round(genelToplamKDVHaric * dgVergiOran, 2);
                            dgVergiKesinti = dgVergi;
                        }
                        if (damga == 2)
                            dgVergiKesinti = 0.0;

                        if (belgeTur == 2 && !chkDamgaVergisi)
                        {
                            td = YeniSatir(o.yil);
                            td.hesapKod = hesapKodTahakkuk.Split('-')[0].Trim();
                            td.borc = 0;
                            td.alacak = Math.Round(genelToplam, 2);
                            td.detayBirim = birimKod + "." + kurumsalKod;
                            td.kur = kurumsalKod;
                            td.altHesapKod = tt.musteriKod;
                            td.satirAciklama = satirAciklama;
                            o.detay.objeler.Add(td);

                            td = YeniSatir(o.yil);
                            td.hesapKod = hesapKodOdeme.Split('-')[0].Trim();
                            td.borc = Math.Round(genelToplam, 2) - dgVergiKesinti;
                            td.alacak = 0;
                            td.detayBirim = birimKod + "." + kurumsalKod;
                            td.kur = kurumsalKod;
                            td.altHesapKod = tt.musteriKod;
                            td.satirAciklama = satirAciklama;
                            o.detay.objeler.Add(td);
                        }

                        if (belgeTur == 2)
                        {
                            if (damga == 2) // Damga vergisi kesilmemiş
                            {
                                td = YeniSatir(o.yil);
                                td.hesapKod = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/FATURADAMGAVERGISIHESAPKOD");
                                td.borc = 0;
                                td.alacak = dgVergi;
                                td.detayBirim = birimKod + "." + kurumsalKod;
                                td.kur = kurumsalKod;
                                td.satirAciklama = satirAciklama;
                                td.altHesapKod = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/FATURADAMGAVERGISIALTHESAP");
                                o.detay.objeler.Add(td);
                            }

                            if (damga != 3) // Damga vergisi yok işaretli değilse
                            {
                                td = YeniSatir(o.yil);
                                td.hesapKod = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/FATURADAMGAVERGISIGIDERHESAPKOD");
                                td.TertipAta(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/FATURADAMGAVERGISITERTIP"));
                                td.detayBirim = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/FATURADAMGAVERGISIBIRIM") + "." + td.kur;
                                if (td.detayBirim != "")
                                {
                                    o.birim = "000";
                                    o.kurKod = "";
                                }
                                td.borc = dgVergi;
                                td.alacak = 0;
                                td.altHesapKod = tt.musteriKod;
                                td.satirAciklama = satirAciklama;
                                o.detay.objeler.Add(td);
                            }
                        }
                    }

                    double toplam800 = 0.0;
                    double toplam830 = 0.0;
                    string birim800 = "";
                    string birim830 = "";
                    string kur800 = "";
                    string kur830 = "";

                    TNSCollection detayListe = new TNSCollection();
                    foreach (OdemeEmriMIFDetay ddd in o.detay.objeler)
                        detayListe.Add(ddd);

                    if (belgeTur == 2)
                    {
                        for (int i = 0; i < detayListe.Count; i++)
                        {
                            OdemeEmriMIFDetay ddd = (OdemeEmriMIFDetay)detayListe[i];

                            if (ddd.hesapKod.StartsWith("600") && !chkDamgaVergisi)
                            {
                                double kdv = 0.0;
                                double tevkifat = 0.0;
                                if (detayListe.Count > i + 1 && ((OdemeEmriMIFDetay)detayListe[i + 1]).hesapKod.StartsWith("391"))
                                {
                                    kdv = ((OdemeEmriMIFDetay)detayListe[i + 1]).alacak;
                                    if (detayListe.Count > i + 2 && ((OdemeEmriMIFDetay)detayListe[i + 2]).hesapKod.StartsWith("360.04.02"))
                                        tevkifat = ((OdemeEmriMIFDetay)detayListe[i + 2]).borc;
                                }

                                td = YeniSatir(o.yil);
                                bool bulundu = false;
                                foreach (OdemeEmriMIFDetay tmp in o.detay.objeler)
                                {
                                    if (tmp.hesapKod == ddd.hesapKod.Replace("600", "800") && tmp.detayBirim == ddd.detayBirim)
                                    {
                                        td = tmp;
                                        bulundu = true;
                                    }
                                }

                                td.hesapKod = ddd.hesapKod.Replace("600", "800");
                                td.borc = 0;
                                td.alacak += ddd.alacak + (kdv - tevkifat);
                                td.detayBirim = ddd.detayBirim;
                                td.kur = ddd.kur;
                                if (!bulundu)
                                    o.detay.objeler.Add(td);

                                birim800 = ddd.detayBirim;
                                kur800 = ddd.kur;
                                toplam800 += Math.Round(ddd.alacak + (kdv - tevkifat), 2);
                            }
                            if (ddd.hesapKod.StartsWith("630"))
                            {
                                td = YeniSatir(o.yil);
                                bool bulundu = false;
                                foreach (OdemeEmriMIFDetay tmp in o.detay.objeler)
                                {
                                    if (tmp.hesapKod == ddd.hesapKod.Replace("630", "830") && tmp.detayBirim == ddd.detayBirim)
                                    {
                                        td = tmp;
                                        bulundu = true;
                                    }
                                }

                                td.hesapKod = ddd.hesapKod.Replace("630", "830");
                                td.TertipAta(ddd.TertipStr());
                                td.borc += Math.Round(ddd.borc, 2);
                                td.alacak = 0;
                                td.detayBirim = ddd.detayBirim;
                                td.kur = ddd.kur;
                                if (!bulundu)
                                    o.detay.objeler.Add(td);

                                birim830 = ddd.detayBirim;
                                kur830 = ddd.kur;
                                toplam830 += ddd.borc;
                            }
                        }
                    }

                    if (toplam800 > 0)
                    {
                        td = YeniSatir(o.yil);
                        td.hesapKod = "805";
                        td.borc = Math.Round(toplam800, 2);
                        td.alacak = 0;
                        td.detayBirim = birim800;
                        td.kur = kur800;
                        o.detay.objeler.Add(td);
                    }

                    if (toplam830 > 0)
                    {
                        td = YeniSatir(o.yil);
                        td.hesapKod = "835";
                        td.borc = 0;
                        td.alacak = Math.Round(toplam830, 2);
                        td.detayBirim = birim830;
                        td.kur = kur830;
                        o.detay.objeler.Add(td);
                    }

                    Sonuc sonuc = servisMUH.OdemeEmriMIFKaydet(kullanan, o);
                    if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "Belge oluşturulamadı."));

                    foreach (int gFaturaKod in listeFaturaKod)
                    {
                        Fatura fGuncelle = new Fatura();
                        fGuncelle.kod = gFaturaKod;
                        if (belgeTur == 1)
                        {
                            fGuncelle.tahakkukBelgeNo = sonuc.anahtar;
                            fGuncelle.tahakkukHesapKod = hesapKodTahakkuk;
                        }
                        else
                        {
                            fGuncelle.odemeBelgeNo = sonuc.anahtar;
                            fGuncelle.odemeTarihi = belgeTarih;
                            fGuncelle.tahakkukHesapKod = hesapKodTahakkuk;
                            fGuncelle.odemeHesapKod = hesapKodOdeme;
                        }
                        servisMUH.FaturaMIFBelgeNoGuncelle(kullanan, fGuncelle);
                    }

                    return BelgeSonucu(o.yil, sonuc.anahtar, belgeTur);
                }
                catch (Exception ex)
                {
                    return Basarisiz(HataMetni(ex.Message, "Belge oluşturulamadı."));
                }
            });
        }

        // ------------------------------------------------------------------ hesap kodu (eski HesapKod.ascx ve satırdaki hesap combosu)
        // Mahsup İşlemleri'nin (KapatmaUclari) hesap uçlarının aynısı; yetki Fatura Hazırlama'dan.

        private static int YilVeyaBuYil(int? yil)
        {
            int y = yil ?? 0;
            return y <= 2000 ? DateTime.Now.Year : y;
        }

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
                int y = YilVeyaBuYil(yil);
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

                string[] hesapKirilimlar = ust.Split('.');
                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.yil = y;
                hs.hesapKod = ust;
                if (ust.Length < 3) hs.seviye = 1;
                else hs.seviye = hesapKirilimlar.Length + 1;

                IMUHServis servisMUH = Servis();
                ObjectArray hv = new ObjectArray();
                for (int i = 0; i < 10; i++)
                {
                    hv = servisMUH.HesapPlaniListele(new Kullanici(), hs);
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
            metin = Temiz(metin);
            if (metin.Length < 2) return Hata("En az iki karakter yazınız.");
            try
            {
                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = YilVeyaBuYil(yil);
                h.hesapKodHesapAd = metin;
                h.detay = sonKirilim != "0";
                ObjectArray hesap = Servis().HesapPlaniListele(kullanan, h);
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
            kod = Temiz(kod);
            if (kod == "") return Results.Json(new { ad = "", detay = false });
            try
            {
                int y = YilVeyaBuYil(yil);
                string hesYapi = TNS.MUH.Arac.DegiskenDegerBul(y, "/KODYAPI/KOD8/KODYAPI");
                if (!string.IsNullOrEmpty(hesYapi)) kod = OrtakFonksiyonlar.KodNoktaKoy(hesYapi, kod.Replace(".", ""));
                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = y;
                h.hesapKod = kod;
                ObjectArray hv = Servis().HesapPlaniListele(kullanan, h);
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
    }
}
