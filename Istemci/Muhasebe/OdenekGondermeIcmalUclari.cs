using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Istemci.ButceHazirlik;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    public class OgiKaydetIstegi
    {
        public int Yil { get; set; }
        public int BelgeTur { get; set; }
        public int IcmalNo { get; set; }
        public string KurKod { get; set; }
        /// <summary>yyyy-MM-dd</summary>
        public string KayitTarih { get; set; }
        public List<string> Kullanicilar { get; set; }
    }

    public class OgiDurumIstegi
    {
        public int Yil { get; set; }
        public int BelgeTur { get; set; }
        /// <summary>5 onayla, 1 yeni yap</summary>
        public int Durum { get; set; }
        public List<int> IcmalNolar { get; set; }
    }

    /// <summary>
    /// Ödenek Gönderme / Tenkis İcmali (eski ButceMuhasebe/OdenekGondermeIcmalFrame.aspx ve iki alt sayfası:
    /// OdenekGondermeIcmalForm.aspx = Belge Düzenleme, OdenekGondermeIcmalSorgu.aspx = Belge Arama). İş kuralları
    /// TNS.MUH (MUHOdenekGondermeIcmal) servisinden çağrılır; yetki menüdeki çerçeve adresinden gelir.
    ///   Form : GET tanim, kurkodlar, oku, yazdir, kisi-ara   POST kaydet
    ///   Liste: GET liste, kodlar (kurumsal kod ağacı)        POST durum (onayla / yeni yap)
    /// </summary>
    public static class OdenekGondermeIcmalUclari
    {
        public const string FrameAdres = "../ButceMuhasebe/OdenekGondermeIcmalFrame.aspx";
        public const string FormAdres = "../ButceMuhasebe/OdenekGondermeIcmalForm.aspx";
        public const string SorguAdres = "../ButceMuhasebe/OdenekGondermeIcmalSorgu.aspx";
        private const string Uc = "/api/odenekgondermeicmal/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "kurkodlar", KurKodlar);
            app.MapGet(Uc + "oku", Oku);
            app.MapPost(Uc + "kaydet", Kaydet);
            app.MapGet(Uc + "yazdir", Yazdir);
            app.MapGet(Uc + "kisi-ara", KisiAra);
            app.MapGet(Uc + "kodlar", Kodlar);
            app.MapGet(Uc + "liste", Liste);
            app.MapPost(Uc + "durum", Durum);
        }

        // ------------------------------------------------------------------ yetki ve ortak

        /// <summary>Çerçeve menüdeki yetkiyle, alt sayfalar çerçevenin yetkisiyle açılır.</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru, string sayfaAdresi)
        {
            if (sayfaAdresi == FrameAdres) return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, FrameAdres);
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, sayfaAdresi, FrameAdres);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!SayfaYetki.Yetkili(kullanan, ortam.ContentRootPath, FrameAdres)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static IResult Basari(string bilgi) { return Results.Json(new { basarili = true, bilgi = bilgi ?? "" }); }

        private static IResult Basarisiz(string hata) { return Results.Json(new { basarili = false, hata = hata ?? "" }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = Temiz(hata);
            return hata == "" ? varsayilan : hata;
        }

        private static bool BelgeTuruGecerli(int belgeTur)
        {
            return belgeTur == (int)ENUMBelgeTuru.ODENEKGONDERME || belgeTur == (int)ENUMBelgeTuru.TENKIS;
        }

        /// <summary>yyyy-MM-dd -> TNSDateTime (boşsa boş tarih).</summary>
        private static TNSDateTime Tarih(string s)
        {
            s = Temiz(s);
            if (s != "" && DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                return new TNSDateTime(t);
            return new TNSDateTime(DateTime.MinValue);
        }

        private static string TarihYaz(TNSDateTime t)
        {
            return t == null || TNSDateTime.IsNull(t) ? "" : t.Oku().ToString("yyyy-MM-dd");
        }

        /// <summary>Kurumsal kodlar (eski KurkodDoldur: 4. düzey, yalnızca detay).</summary>
        private static List<object> KurKodListesi(Kullanici kullanan, int yil)
        {
            List<object> l = new List<object>();
            ABSKodlarVAN v = TNS.BKD.Arac.Tanimla().KodListeleSeviye(yil, "1", "", 4, true);
            if (v == null || !v.sonuc.islemSonuc || v.absKodlar == null) return l;
            foreach (ABSKod ak in v.absKodlar)
                l.Add(new { kod = ak.kod, kodAd = ak.kod + " - " + ak.Ad(kullanan.aktifKultur) });
            return l;
        }

        // ------------------------------------------------------------------ form

        /// <summary>Açılış (eski Page_Load iki alt sayfa için): yıllar, belge türleri, durumlar, kurumsal kodlar.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int yil = DateTime.Now.Year;
                List<int> yillar = new List<int>();
                for (int i = yil + 1; i >= 2006; i--) yillar.Add(i);
                return Results.Json(new
                {
                    yil = yil,
                    yillar = yillar,
                    bugun = DateTime.Today.ToString("yyyy-MM-dd"),
                    belgeTurleri = new object[]
                    {
                        new { kod = (int)ENUMBelgeTuru.ODENEKGONDERME, ad = "Ödenek Gönderme" },
                        new { kod = (int)ENUMBelgeTuru.TENKIS, ad = "Tenkis" }
                    },
                    durumlar = new object[]
                    {
                        new { kod = 1, ad = "Yeniler" },
                        new { kod = 5, ad = "Onaylılar" },
                        new { kod = 0, ad = "Hepsi" }
                    },
                    kurKodlar = KurKodListesi(kullanan, yil)
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Sayfa bilgileri okunamadı."));
            }
        }

        private static IResult KurKodlar(HttpContext baglam, IWebHostEnvironment ortam, int? yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((yil ?? 0) <= 0) return Hata("Yıl seçiniz.");
            try
            {
                return Results.Json(new { kurKodlar = KurKodListesi(kullanan, yil.Value) });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kurumsal kodlar okunamadı."));
            }
        }

        /// <summary>Oku (eski btnOku_Click).</summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? belgeTur, int? icmalNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((yil ?? 0) <= 0 || (icmalNo ?? 0) <= 0 || !BelgeTuruGecerli(belgeTur ?? 0)) return Hata("Anahtar alanları girmelisiniz.");
            try
            {
                GondermeTenkisIcmal form = new GondermeTenkisIcmal(yil.Value, (ENUMBelgeTuru)belgeTur.Value, icmalNo.Value);
                ObjectArray formlar = TNS.MUH.Arac.Tanimla().ListeleOdenekGondermeIcmal(kullanan, form);
                if (formlar == null || !formlar.sonuc.islemSonuc || formlar.ObjeSayisi != 1)
                    return Hata("Verilen kriterlere uygun kayıt bulunamadı.");
                form = (GondermeTenkisIcmal)formlar.objeler[0];

                List<object> kullanicilar = new List<object>();
                if (form.kullanicilar != null)
                    foreach (GondermeTenkisIcmalKullanici k in form.kullanicilar)
                    {
                        if (k == null || string.IsNullOrWhiteSpace(k.kulKod)) continue;
                        kullanicilar.Add(new { kulKod = k.kulKod, adSoyad = AdSoyad(kullanan, k.kulKod) });
                    }

                return Results.Json(new
                {
                    yil = form.yil,
                    belgeTur = (int)form.belgeTur,
                    icmalNo = form.icmalNo,
                    kurKod = form.kurKod,
                    kayitTarih = TarihYaz(form.kayitTarih),
                    formDurum = form.formDurum,
                    durum = form.durum,
                    kullanicilar = kullanicilar
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Verilen kriterlere uygun kayıt bulunamadı."));
            }
        }

        private static string AdSoyad(Kullanici kullanan, string kulKod)
        {
            try
            {
                KullaniciVAN kul = TNS.KYM.Arac.Tanimla().KullaniciOku(kullanan, kulKod.Trim());
                if (kul == null || !kul.sonuc.islemSonuc || kul.kullanici == null) return "";
                return ((kul.kullanici.ad ?? "") + " " + (kul.kullanici.soyad ?? "")).Trim();
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>Kaydet (eski btnKaydet_Click). Alan denetimleri iş kurallarında (MUHOdenekGondermeIcmal.AlanlarGecerliMi).</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OgiKaydetIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new OgiKaydetIstegi();
            if (istek.Yil <= 0 || istek.IcmalNo <= 0 || !BelgeTuruGecerli(istek.BelgeTur)) return Basarisiz("Anahtar alanları girmelisiniz.");
            try
            {
                GondermeTenkisIcmal form = new GondermeTenkisIcmal(istek.Yil, (ENUMBelgeTuru)istek.BelgeTur, istek.IcmalNo);
                form.kurKod = Temiz(istek.KurKod);
                form.kayitTarih = Tarih(istek.KayitTarih);
                form.kullanicilar = new TNSCollection();
                HashSet<string> eklenen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (string kod in istek.Kullanicilar ?? new List<string>())
                {
                    string k = Temiz(kod);
                    if (k == "" || !eklenen.Add(k)) continue;
                    form.kullanicilar.Add(new GondermeTenkisIcmalKullanici(k));
                }
                Sonuc sonuc = TNS.MUH.Arac.Tanimla().KaydetOdenekGondermeIcmal(kullanan, form);
                if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "Kayıt işlemi başarısız."));
                return Basari("Kayıt işlemi başarıyla gerçekleşti.");
            }
            catch (Exception ex)
            {
                return Basarisiz(HataMetni(ex.Message, "Kayıt işlemi başarısız."));
            }
        }

        /// <summary>Yazdır (eski btnYazdir_Click; Ödenek Gönderme sayfasındaki icmal çıktısıyla aynı rapor).</summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? belgeTur, int? icmalNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((icmalNo ?? 0) <= 0) return Hata("Yazdırılacak İcmal numarası belirtilmemiş.");
            if ((yil ?? 0) <= 0 || !BelgeTuruGecerli(belgeTur ?? 0)) return Hata("Anahtar alanları girmelisiniz.");
            try
            {
                AsposeLisans.Yukle();
                string kurum = GenelIslemlerIstemci.VarsayilanKurumBul() ?? "";
                string hata = OdenekGondermeYazdirma.IcmalBelgesiYazdir(kullanan, yil.Value, (ENUMBelgeTuru)belgeTur.Value, kurum, icmalNo.Value, out string dosya);
                if (dosya == null) return Hata(HataMetni(hata, "İcmal yazdırılamadı."));
                return DosyaIslem.DosyaGonder(dosya, "IcmalListesi_" + icmalNo.Value, true, GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İcmal yazdırılamadı."));
            }
        }

        /// <summary>
        /// Kullanıcı Seç penceresi (eski KYM/ListeKullaniciSec.aspx): kullanıcılar arasında arama (en az bir ölçüt; aktif kullanıcılar,
        /// en çok 300).
        /// </summary>
        private static IResult KisiAra(HttpContext baglam, IWebHostEnvironment ortam, string ad, string soyad, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            Kullanici kriter = new Kullanici();
            kriter.ad = Temiz(ad);
            kriter.soyad = Temiz(soyad);
            kriter.kullaniciKodu = Temiz(kod);
            if (kriter.ad == "" && kriter.soyad == "" && kriter.kullaniciKodu == "")
                return Hata("Ad, soyad ya da kullanıcı kodundan en az birini yazınız.");
            kriter.aktifMi = ENUMAktiflik.AKTIF;
            try
            {
                ObjectArray<Kullanici> sonuc = TNS.KYM.Arac.Tanimla().KullaniciAra(kriter, 1, 0);
                if (sonuc == null || !sonuc.sonuc.islemSonuc) return Hata("Kullanıcılar okunamadı.");

                List<object> satirlar = new List<object>();
                bool fazla = false;
                foreach (Kullanici k in sonuc.objeler)
                {
                    if (string.IsNullOrWhiteSpace(k.kullaniciKodu)) continue;
                    if (satirlar.Count >= 300) { fazla = true; break; }
                    satirlar.Add(new { kullaniciKodu = k.kullaniciKodu, ad = k.ad, soyad = k.soyad, birim = k.calistigiBirim });
                }
                return Results.Json(new { satirlar = satirlar, fazla = fazla });
            }
            catch (Exception)
            {
                return Hata("Kullanıcılar okunamadı.");
            }
        }

        // ------------------------------------------------------------------ liste

        /// <summary>Listele (eski OdenekGondermeIcmalSorgu btnListe_Click).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? belgeTur, int? durum,
            string icmalBas, string icmalBit, string kurKod, string islemYapan, string basTarih, string bitTarih)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((yil ?? 0) <= 0 || !BelgeTuruGecerli(belgeTur ?? 0)) return Hata("Yıl ve belge türü seçiniz.");
            try
            {
                GondermeTenkisIcmal f = new GondermeTenkisIcmal();
                f.yil = yil.Value;
                f.belgeTur = (ENUMBelgeTuru)belgeTur.Value;
                f.kurKod = Temiz(kurKod);
                f.formDurum = durum ?? 0;
                f.icmalBas = OrtakFonksiyonlar.ConvertToInt(Temiz(icmalBas), 0);
                f.icmalBit = OrtakFonksiyonlar.ConvertToInt(Temiz(icmalBit), 0);
                f.araBasTarih = Tarih(basTarih);
                f.araBitTarih = Tarih(bitTarih);
                string kul = Temiz(islemYapan);
                if (kul != "")
                {
                    f.kullanicilar = new TNSCollection();
                    f.kullanicilar.Add(new GondermeTenkisIcmalKullanici(kul));
                }

                ObjectArray fler = TNS.MUH.Arac.Tanimla().ListeleOdenekGondermeIcmal(kullanan, f);
                if (fler == null) return Hata("Liste okunamadı.");
                if (!fler.sonuc.islemSonuc) return Hata(HataMetni(fler.sonuc.hataStr, "Liste okunamadı."));

                List<object> satirlar = new List<object>();
                foreach (GondermeTenkisIcmal ff in fler.objeler)
                {
                    satirlar.Add(new
                    {
                        icmalNo = ff.icmalNo,
                        belgeTur = (int)ff.belgeTur,
                        belgeTurAd = ff.belgeTurAd,
                        kisiler = ff.kisiler,
                        kurKod = ff.kurKod,
                        formDurum = ff.formDurum,
                        durum = ff.durum,
                        dtKayitTarih = TarihYaz(ff.kayitTarih)
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Liste okunamadı."));
            }
        }

        /// <summary>Kurumsal kod ağacının bir düzeyi (eski ddKurKod ağacı: nodeLoadPar('1', ...)).</summary>
        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return GorusmeCariUclari.KodDuzeyi(kullanan, yil, "1", ust, false);
        }

        /// <summary>Onayla (5) / Yeni Yap (1) (eski BelgeDurumDegisikligi).</summary>
        private static async Task<IResult> Durum(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, OgiDurumIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new OgiDurumIstegi();
            if (istek.Durum != 1 && istek.Durum != 5) return Basarisiz("Geçersiz işlem.");
            if (istek.Yil <= 0 || !BelgeTuruGecerli(istek.BelgeTur)) return Basarisiz("Yıl ve belge türü seçiniz.");

            TNSCollection fler = new TNSCollection();
            List<string> nolar = new List<string>();
            foreach (int no in istek.IcmalNolar ?? new List<int>())
            {
                if (no <= 0) continue;
                GondermeTenkisIcmal f = new GondermeTenkisIcmal(istek.Yil, (ENUMBelgeTuru)istek.BelgeTur, no);
                f.formDurum = istek.Durum;
                fler.Add(f);
                nolar.Add(no.ToString());
            }
            if (fler.Count == 0) return Basarisiz("Listeden herhangi bir belge seçilmediği için işlem yapılamadı.");

            string ad = istek.Durum == 5 ? "Onaylama" : "Yeni yapma";
            try
            {
                Sonuc sonuc = TNS.MUH.Arac.Tanimla().DurumAtaOdenekGondermeIcmal(kullanan, fler);
                if (sonuc == null || !sonuc.islemSonuc)
                    return Basarisiz(ad + " başarısız: " + HataMetni(sonuc?.hataStr, "") + " " + string.Join(",", nolar));
                return Basari(ad + " başarılı");
            }
            catch (Exception ex)
            {
                return Basarisiz(ad + " başarısız: " + ex.Message);
            }
        }
    }
}
