using System;
using System.Collections;
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
using TNS.BKD;
using TNS.KYM;
using TNS.MUH;
using TNS.ORG;
using TNS.PER;

namespace Istemci.Organizasyon
{
    public class OrgHarcamaKodIstegi
    {
        public string KurKod { get; set; }
        public string KurAd { get; set; }
        public string MuhasebeBirimKod { get; set; }
        public string MuhasebeBirimAd { get; set; }
        public string FonKod { get; set; }
        public string FonAd { get; set; }
    }

    public class OrgBirimIstegi
    {
        /// <summary>Birimin kodu; yeni birimde 0.</summary>
        public int Kod { get; set; }
        public int AnaBirimKod { get; set; }
        public bool Sablon { get; set; }

        public string Ad { get; set; }
        public string EntegreKod { get; set; }
        public string KisaAd { get; set; }
        public string KurumKod { get; set; }
        public string TurKod { get; set; }
        public int MerkezKod { get; set; }

        // Birim özellikleri
        public bool Yonetici { get; set; }
        public bool Kurum { get; set; }
        public bool Harcama { get; set; }
        public bool Butce { get; set; }
        public bool Tasinir { get; set; }
        public bool Ambar { get; set; }
        public bool Personel { get; set; }
        public bool Evrak { get; set; }
        public bool Iletisim { get; set; }
        /// <summary>Coğrafi birim türü: "" | ulke | bolge | il | ilce.</summary>
        public string Cografya { get; set; }
        public bool CizimCikisNoktasi { get; set; }
        public bool CizimdeGorunme { get; set; }

        public int YoneticiKod { get; set; }
        public string YoneticiTc { get; set; }
        public int UstYoneticiKod1 { get; set; }
        public string UstYoneticiTc1 { get; set; }
        public int UstYoneticiKod2 { get; set; }
        public string UstYoneticiTc2 { get; set; }

        public string Muhasebe { get; set; }
        public OrgHarcamaKodIstegi[] HarcamaKodlari { get; set; }
        public string VergiNo { get; set; }
        public string VergiDairesi { get; set; }

        public string MerkezKurKod { get; set; }
        public string TasraKurKod { get; set; }
        public string YurtDisiKurKod { get; set; }
        public string FaaliyetKodlari { get; set; }

        public string AmbarKurKod { get; set; }
        public string AmbarBirimKod { get; set; }
        public string AmbarKod { get; set; }

        public string EvrakBirim { get; set; }
        public string Antet { get; set; }

        public string Ulke { get; set; }
        public string Bolge { get; set; }
        public string Il { get; set; }
        public string Ilce { get; set; }

        public string Adres { get; set; }
        public string Telefon { get; set; }
        public string Faks { get; set; }
        public string Eposta { get; set; }
        public string Url { get; set; }
    }

    public class OrgBirimKodIstegi
    {
        public int Kod { get; set; }
        public bool Sablon { get; set; }
    }

    /// <summary>
    /// Organizasyon Şeması sayfasının (eski Organizasyon/ORGAna.aspx + ORGAgac.aspx + ORGBirim.aspx) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/orgbirim/rapor/birimler?kod= (Yazdır), /rapor/gorevler (Görev Yaz): Excel dosyaları
    ///   GET  /api/orgbirim/tanimlar, /agac?ust=, /oku?kod=, /cografya?ust=&amp;seviye=, /kodlar?tur=&amp;q=, /muhasebe?kod=, /muhasebe-birim?muhasebe=&amp;kurum=&amp;q=
    ///   POST /api/orgbirim/kaydet, /sil, /ac, /kapat
    /// Sayfayı menüde "Organizasyon Şeması" seçeneğini görebilenler açar; sistem yöneticisi olmayan yalnızca yetkili olduğu
    /// birimleri görür ve değiştiremez. /OrgSemaReadOnly değişkeni 0'dan büyükse şema herkes için salt okunurdur.
    /// </summary>
    public static class OrgBirimUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/orgbirim/tanimlar", Tanimlar);
            app.MapGet("/api/orgbirim/agac", Agac);
            app.MapGet("/api/orgbirim/oku", Oku);
            app.MapGet("/api/orgbirim/cografya", Cografya);
            app.MapGet("/api/orgbirim/kodlar", Kodlar);
            app.MapGet("/api/orgbirim/muhasebe", MuhasebeAd);
            app.MapGet("/api/orgbirim/muhasebe-birim", MuhasebeBirimleri);
            app.MapGet("/api/orgbirim/rapor/birimler", BirimRaporu);
            app.MapGet("/api/orgbirim/rapor/gorevler", GorevRaporu);
            app.MapPost("/api/orgbirim/kaydet", Kaydet);
            app.MapPost("/api/orgbirim/sil", Sil);
            app.MapPost("/api/orgbirim/ac", BirimAc);
            app.MapPost("/api/orgbirim/kapat", BirimKapat);
        }

        // ------------------------------------------------------------------ yetki

        /// <summary>Kullanıcı Organizasyon Şeması sayfasını açabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), "../Organizasyon/ORGAna.aspx?tur=1");
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Şemayı değiştirebilir mi? (eski ORGAna.OrgSemaReadOnly'nin tersi)</summary>
        public static bool Yazabilir(Kullanici kullanan)
        {
            if (!GenelGorevUclari.YetkiVar(kullanan)) return false;
            return OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "/OrgSemaReadOnly"), 0) <= 0;
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, bool yazma, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath) || (yazma && !Yazabilir(kullanan)))
                return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static string Kultur(Kullanici kullanan)
        {
            return string.IsNullOrEmpty(kullanan.aktifKultur) ? "tr-TR" : kullanan.aktifKultur;
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Formdaki seçim listeleri: birim türleri, yönetici unvanları, ülkeler.</summary>
        private static IResult Tanimlar(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, false, out Kullanici kullanan);
            if (ret != null) return ret;

            string kultur = Kultur(kullanan);
            List<string> uyarilar = new List<string>();

            List<object> birimTurleri = new List<object>();
            try
            {
                ABSKodlarVAN turler = TNS.BKD.Arac.Tanimla().KodListeleSeviye(DateTime.Now.Year, "21", "", -1);
                if (turler.sonuc.islemSonuc)
                    foreach (ABSKod k in turler.absKodlar)
                        birimTurleri.Add(new { kod = k.kod, ad = k.kod + " - " + k.AdKultur(kultur) });
            }
            catch (Exception)
            {
                uyarilar.Add("Birim türleri okunamadı.");
            }

            List<object> unvanlar = new List<object>();
            try
            {
                ObjectArray liste = TNS.PER.Arac.Tanimla().UnvanListele(new Unvan { yonetici = 1 });
                if (liste.sonuc.islemSonuc)
                    foreach (Unvan u in liste.objeler)
                        unvanlar.Add(new { kod = u.kod, ad = u.ad });
            }
            catch (Exception)
            {
                uyarilar.Add("Yönetici unvanları okunamadı (Personel modülü); kayıtlı unvanlar korunur.");
            }

            return Results.Json(new
            {
                yazabilir = Yazabilir(kullanan),
                // Göreve kişi atama (Görevler sekmesi) yalnızca sistem yöneticisine açıktır.
                gorevAtayabilir = GenelGorevUclari.YetkiVar(kullanan),
                birimTurleri = birimTurleri,
                unvanlar = unvanlar,
                ulkeler = CografyaListesi("", 1, kultur),
                uyari = uyarilar.Count > 0 ? string.Join(" ", uyarilar) : null
            });
        }

        /// <summary>Birimin ağaçtaki ikonu ve açıklaması (eski ORGAgac: birimHarcama.png, birimButce.png ...).</summary>
        private static string BirimTipi(OrganizasyonBirim d)
        {
            if (d.harcamaMi > 0) return "harcama";
            if (d.butceMi > 0) return "butce";
            if (d.tasinirMi > 0) return "tasinir";
            if (d.personelMi > 0) return "personel";
            if (d.evrakMi > 0) return "evrak";
            if (d.ulkeMi > 0 || d.bolgeMi > 0 || d.ilMi > 0 || d.ilceMi > 0) return "cografya";
            return "grup";
        }

        /// <summary>
        /// Ağacın bir düzeyi (eski ORGAgac NodeLoad): kökte sistem yöneticisi bütün şemayı, diğerleri yetkili oldukları
        /// birimleri görür. kapaliDahil=1 ise kapatılmış birimler de gelir (Birim Aç penceresi).
        /// </summary>
        private static IResult Agac(HttpContext baglam, IWebHostEnvironment ortam, int? ust, string sablon, string kapaliDahil)
        {
            IResult ret = Denetle(baglam, ortam, false, out Kullanici kullanan);
            if (ret != null) return ret;

            IORGServis servis = TNS.ORG.Arac.Tanimla();
            OrganizasyonBirim kriter = new OrganizasyonBirim();
            kriter.anaBirimKod = Math.Max(0, ust ?? 0);
            if (kriter.anaBirimKod == 0)
            {
                kriter.kopruKod = "999";
                kriter.sablonMu = sablon == "1";
            }
            if (kapaliDahil == "1")
            {
                kriter.baslangicTarih = null;
                kriter.bitisTarih = null;
            }

            ObjectArray<OrganizasyonBirim> sonuc = kriter.anaBirimKod == 0 && !GenelGorevUclari.YetkiVar(kullanan)
                ? servis.OrganizasyonBirimListeleKullanici(kullanan, false, false)
                : servis.OrganizasyonBirimListele(kriter, false, false);
            if (!sonuc.sonuc.islemSonuc) return Results.Json(new { hata = "Birimler okunamadı." });

            List<object> satirlar = new List<object>();
            foreach (OrganizasyonBirim b in sonuc.objeler)
                satirlar.Add(new { kod = b.kod, ad = b.adi, altVar = b.altDalVar, tip = BirimTipi(b), ust = b.anaBirimKod });
            return Results.Json(new { satirlar = satirlar });
        }

        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int kod, string sablon)
        {
            IResult ret = Denetle(baglam, ortam, false, out Kullanici kullanan);
            if (ret != null) return ret;
            if (kod <= 0) return Results.BadRequest();

            OrganizasyonBirim kriter = new OrganizasyonBirim();
            kriter.kod = kod;
            kriter.sablonMu = sablon == "1";
            ObjectArray<OrganizasyonBirim> birimler = TNS.ORG.Arac.Tanimla().OrganizasyonBirimListele(kriter, true, true);
            if (!birimler.sonuc.islemSonuc || birimler.objeler.Count == 0) return Results.Json(new { hata = "Birim okunamadı." });
            OrganizasyonBirim b = (OrganizasyonBirim)birimler.objeler[0];

            string merkezKur = "", tasraKur = "", yurtDisiKur = "";
            if (b.orgButceKod != null)
                foreach (OrgButceKod o in b.orgButceKod)
                {
                    if (o == null) continue;
                    if (o.tur == (int)ENUMORGButceKodTur.MERKEZ) merkezKur = o.kurKod ?? "";
                    else if (o.tur == (int)ENUMORGButceKodTur.TASRA) tasraKur = o.kurKod ?? "";
                    else if (o.tur == (int)ENUMORGButceKodTur.YURTDISI) yurtDisiKur = o.kurKod ?? "";
                }

            List<object> harcamaKodlari = new List<object>();
            if (b.orgHarcamaKod != null)
                foreach (OrgHarcamaKod o in b.orgHarcamaKod)
                    if (o != null)
                        harcamaKodlari.Add(new { kurKod = o.kurKod ?? "", kurAd = o.kurAd ?? "", muhasebeBirimKod = o.muhasebeBirimKod ?? "", muhasebeBirimAd = o.muhasebeBirimAd ?? "", fonKod = o.fonKod ?? "", fonAd = o.fonAd ?? "" });

            string cografya = b.ulkeMi > 0 ? "ulke" : b.bolgeMi > 0 ? "bolge" : b.ilMi > 0 ? "il" : b.ilceMi > 0 ? "ilce" : "";
            bool ambar = b.ambarMi > 0;

            return Results.Json(new
            {
                kod = b.kod,
                anaBirimKod = b.anaBirimKod,
                ad = b.adi,
                entegreKod = b.entegreKod,
                kisaAd = b.kisaadi,
                kurumKod = b.kurumKod,
                turKod = b.turKod,
                turAd = b.turAd,
                merkezKod = b.merkezKod,
                merkezAd = b.merkezAd,

                yonetici = b.yoneticiKod > 0,
                kurum = b.kurumMu > 0,
                harcama = b.harcamaMi > 0,
                butce = b.butceMi > 0,
                tasinir = b.tasinirMi > 0,
                ambar = ambar,
                personel = b.personelMi > 0,
                evrak = b.evrakMi > 0,
                iletisim = b.iletisimMi > 0,
                cografya = cografya,
                cizimCikisNoktasi = b.cizimCikisNoktasi,
                cizimdeGorunme = b.cizimdeGorunme,

                yoneticiKod = b.yoneticiKod,
                yoneticiAd = b.yoneticiAd,
                yoneticiTc = b.yoneticiTCKimlik,
                yoneticiTcAd = b.yoneticiTCKimlikAd,
                ustYoneticiKod1 = b.ustYoneticiKod1,
                ustYoneticiAd1 = b.ustYoneticiAd1,
                ustYoneticiTc1 = b.ustYoneticiTCKimlik1,
                ustYoneticiTcAd1 = b.ustYoneticiTCKimlikAd1,
                ustYoneticiKod2 = b.ustYoneticiKod2,
                ustYoneticiAd2 = b.ustYoneticiAd2,
                ustYoneticiTc2 = b.ustYoneticiTCKimlik2,
                ustYoneticiTcAd2 = b.ustYoneticiTCKimlikAd2,

                muhasebe = b.muhasebekodu,
                muhasebeAd = MuhasebeAdBul(b.muhasebekodu),
                harcamaKodlari = harcamaKodlari,
                vergiNo = b.vergino,
                vergiDairesi = b.vergidairesi,

                merkezKurKod = merkezKur,
                tasraKurKod = tasraKur,
                yurtDisiKurKod = yurtDisiKur,
                faaliyetKodlari = b.altFaaliyetKodlar,

                ambarKurKod = ambar ? b.kurumsalkod : "",
                ambarBirimKod = ambar ? b.muhasebebirim : "",
                ambarKod = ambar ? b.ambarKod : "",

                evrakBirim = b.evrakbirim,
                antet = b.antet,

                ulke = b.ulke,
                bolge = b.bolge,
                il = b.il,
                ilce = b.ilce,

                adres = b.adres,
                telefon = b.telefon,
                faks = b.faks,
                eposta = b.eposta,
                url = b.url
            });
        }

        private static List<object> CografyaListesi(string ust, int seviye, string kultur)
        {
            List<object> satirlar = new List<object>();
            ABSKodlarVAN kodlar = TNS.BKD.Arac.Tanimla().KodListeleSeviye(0, "20", ust ?? "", seviye, true, true, "", "", kultur);
            if (kodlar.sonuc.islemSonuc)
                foreach (ABSKod k in kodlar.absKodlar)
                    satirlar.Add(new { kod = k.kod, ad = k.AdKultur(kultur) });
            return satirlar;
        }

        /// <summary>Ülke / bölge / il / ilçe listesi: ust kodunun altındaki, verilen seviyedeki (1-4) coğrafya kodları.</summary>
        private static IResult Cografya(HttpContext baglam, IWebHostEnvironment ortam, string ust, int seviye)
        {
            IResult ret = Denetle(baglam, ortam, false, out Kullanici kullanan);
            if (ret != null) return ret;
            if (seviye < 1 || seviye > 4) return Results.BadRequest();
            return Results.Json(new { satirlar = CografyaListesi((ust ?? "").Trim(), seviye, Kultur(kullanan)) });
        }

        /// <summary>
        /// Kurumsal (tur=1) ya da fonksiyonel (tur=2) kod arama: q ile başlayan dördüncü seviye kodlar (eski ABSKodDoldur).
        /// q boşsa kurumsal kodlarda varsayılan kurumun kodları gelir.
        /// </summary>
        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int tur, string q)
        {
            IResult ret = Denetle(baglam, ortam, false, out Kullanici kullanan);
            if (ret != null) return ret;
            if (tur != 1 && tur != 2) return Results.BadRequest();

            string filtre = (q ?? "").Trim();
            if (filtre == "" || filtre == "*") filtre = tur == 1 ? TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMKOD") ?? "" : "";

            List<object> satirlar = new List<object>();
            try
            {
                string kultur = Kultur(kullanan);
                ABSKodlarVAN kodlar = TNS.BKD.Arac.Tanimla().KodListeleSeviye(DateTime.Now.Year, tur.ToString(), filtre, 4);
                if (kodlar.sonuc.islemSonuc)
                    foreach (ABSKod k in kodlar.absKodlar)
                    {
                        satirlar.Add(new { kod = k.kod, ad = k.AdKultur(kultur) });
                        if (satirlar.Count >= 200) break;
                    }
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Kodlar okunamadı." });
            }
            return Results.Json(new { satirlar = satirlar });
        }

        private static string MuhasebeAdBul(string kod)
        {
            kod = (kod ?? "").Trim();
            if (OrtakFonksiyonlar.ConvertToInt(kod, 0) <= 0) return "";
            try
            {
                MuhasebeBilgisi kriter = new MuhasebeBilgisi();
                kriter.kistasKodAd = kod;
                ObjectArray sonuc = TNS.MUH.Arac.Tanimla().MuhasebeListele(kriter);
                if (sonuc.sonuc.islemSonuc)
                    foreach (MuhasebeBilgisi m in sonuc.objeler)
                        if (Convert.ToString(m.kod) == kod || OrtakFonksiyonlar.ConvertToInt(Convert.ToString(m.kod), -1) == OrtakFonksiyonlar.ConvertToInt(kod, 0))
                            return m.ad ?? "";
            }
            catch (Exception)
            {
                // Ad bulunamazsa boş kalır; kod yine de kaydedilebilir.
            }
            return "";
        }

        private static IResult MuhasebeAd(HttpContext baglam, IWebHostEnvironment ortam, string kod)
        {
            IResult ret = Denetle(baglam, ortam, false, out Kullanici kullanan);
            if (ret != null) return ret;
            return Results.Json(new { ad = MuhasebeAdBul(kod) });
        }

        /// <summary>Muhasebenin harcama birimleri: ada ya da koda göre arama (eski MuhasebeBirimDoldur).</summary>
        private static IResult MuhasebeBirimleri(HttpContext baglam, IWebHostEnvironment ortam, string muhasebe, string kurum, string q)
        {
            IResult ret = Denetle(baglam, ortam, false, out Kullanici kullanan);
            if (ret != null) return ret;

            int muhasebeKod = OrtakFonksiyonlar.ConvertToInt((muhasebe ?? "").Trim(), 0);
            if (muhasebeKod <= 0) return Results.Json(new { hata = "Önce muhasebe kodunu yazınız." });
            string filtre = (q ?? "").Trim();
            if (filtre == "*") filtre = "";

            List<object> satirlar = new List<object>();
            HashSet<string> eklenen = new HashSet<string>();
            try
            {
                IMUHServis servis = TNS.MUH.Arac.Tanimla();
                // Önce ada, sonra koda göre aranır.
                for (int tur = 0; tur < (filtre == "" ? 1 : 2); tur++)
                {
                    MuhasebeBirim kriter = new MuhasebeBirim();
                    kriter.muhasebe = muhasebeKod;
                    kriter.kurum = (kurum ?? "").Trim();
                    if (filtre != "")
                    {
                        if (tur == 0) kriter.birimAd = filtre;
                        else kriter.birimKod = filtre;
                    }
                    ObjectArray liste = servis.MuhasebeBirimListele(kriter);
                    if (!liste.sonuc.islemSonuc) continue;
                    foreach (MuhasebeBirim mb in liste.objeler)
                    {
                        if (!eklenen.Add(mb.birimKod ?? "")) continue;
                        satirlar.Add(new { kod = mb.birimKod, ad = mb.birimAd });
                        if (satirlar.Count >= 200) break;
                    }
                }
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Harcama birimleri okunamadı." });
            }
            return Results.Json(new { satirlar = satirlar });
        }

        // ------------------------------------------------------------------ raporlar
        // Eski ORGAna.aspx.cs'teki kodun aynısıdır (OrganizasyonBirimYazdir, btnGorev_Click): Excel dosyaları OrtakClass.Tablo
        // (TabloAspose) ile RaporSablon\KYM altındaki şablonlardan üretilir. Tek fark dosyanın tarayıcıya gönderilme biçimidir.

        private static string RaporSablonYol()
        {
            return Ayar.Oku("RaporSablonYol");
        }

        /// <summary>Yazdır: verilen birim ve altındaki birimlerin listesi (kod verilmezse bütün şema). Şablon: KYM\OrgBirimSablon.xlt.</summary>
        private static IResult BirimRaporu(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            IResult ret = Denetle(baglam, ortam, false, out Kullanici kullanan);
            if (ret != null) return ret;
            return BirimRaporuUret(kullanan, kod);
        }

        private static IResult BirimRaporuUret(Kullanici kullanan, int? kod)
        {
            try
            {
                int anaBirimKod = Math.Max(0, kod ?? 0);
                IORGServis servisORG = TNS.ORG.Arac.Tanimla();

                OrganizasyonBirim ob = new OrganizasyonBirim();
                ob.kopruKod = TNS.UZY.Arac.Tanimla().UzayDegeriStr(kullanan, "ORGBIRIM", anaBirimKod.ToString(), true, "KOPRUKOD");
                ObjectArray<OrganizasyonBirim> obler = servisORG.OrganizasyonBirimListele(ob, false, true);
                if (!obler.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(obler.sonuc, "Birimler okunamadı.") });
                if (obler.objeler.Count <= 0) return Results.Json(new { hata = "Yazdırılacak birim bulunamadı." });

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;
                int sutun = 0;
                int kaynakSatir = 0;

                string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
                string sablonAd = "OrgBirimSablon.xlt";
                XLS.DosyaAc(Path.Combine(RaporSablonYol(), "KYM", sablonAd), sonucDosyaAd);

                XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

                satir = kaynakSatir;

                for (int i = 0; i < obler.objeler.Count; i++)
                {
                    ob = (OrganizasyonBirim)obler.objeler[i];

                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 31, satir, sutun);

                    XLS.HucreDegerYaz(satir, sutun, ob.kod);
                    XLS.HucreDegerYaz(satir, sutun + 1, ob.adi);
                    XLS.HucreDegerYaz(satir, sutun + 2, ob.kisaadi);
                    XLS.HucreDegerYaz(satir, sutun + 3, ob.anaBirimKod);
                    XLS.HucreDegerYaz(satir, sutun + 4, ob.merkezKod);
                    XLS.HucreDegerYaz(satir, sutun + 5, ob.yoneticiAd);
                    XLS.HucreDegerYaz(satir, sutun + 6, ob.ustYoneticiAd1);
                    XLS.HucreDegerYaz(satir, sutun + 7, ob.ustYoneticiAd2);
                    XLS.HucreDegerYaz(satir, sutun + 8, ob.kurumsalkod);
                    XLS.HucreDegerYaz(satir, sutun + 9, ob.fonKod);
                    XLS.HucreDegerYaz(satir, sutun + 10, ob.muhasebekodu);
                    XLS.HucreDegerYaz(satir, sutun + 11, ob.muhasebebirim);
                    XLS.HucreDegerYaz(satir, sutun + 12, ob.evrakbirim);
                    XLS.HucreDegerYaz(satir, sutun + 13, ob.ulkeAd);
                    XLS.HucreDegerYaz(satir, sutun + 14, ob.bolgeAd);
                    XLS.HucreDegerYaz(satir, sutun + 15, ob.ilAd);
                    XLS.HucreDegerYaz(satir, sutun + 16, ob.ilceAd);
                    XLS.HucreDegerYaz(satir, sutun + 17, ob.vergino);
                    XLS.HucreDegerYaz(satir, sutun + 18, ob.vergidairesi);
                    XLS.HucreDegerYaz(satir, sutun + 19, ob.adres);
                    XLS.HucreDegerYaz(satir, sutun + 20, ob.telefon);
                    XLS.HucreDegerYaz(satir, sutun + 21, ob.faks);
                    XLS.HucreDegerYaz(satir, sutun + 22, ob.eposta);
                    XLS.HucreDegerYaz(satir, sutun + 23, ob.url);
                    XLS.HucreDegerYaz(satir, sutun + 24, ob.yerMi);
                    XLS.HucreDegerYaz(satir, sutun + 25, ob.harcamaMi);
                    XLS.HucreDegerYaz(satir, sutun + 26, ob.butceMi);
                    XLS.HucreDegerYaz(satir, sutun + 27, ob.tasinirMi);
                    XLS.HucreDegerYaz(satir, sutun + 28, ob.personelMi);
                    XLS.HucreDegerYaz(satir, sutun + 29, ob.evrakMi);
                    XLS.HucreDegerYaz(satir, sutun + 30, ob.iletisimMi);
                    XLS.HucreDegerYaz(satir, sutun + 31, ob.turAd);
                }

                XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), "Organizasyon Şeması", true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }

        private static int GorevYerBul(List<BirimGenelGorev> l, int gorevOrgKod, int gorevSiraNo)
        {
            int yer = 0;
            foreach (BirimGenelGorev g in l)
            {
                if (g.OrgKod == gorevOrgKod && g.siraNo == gorevSiraNo)
                    return yer;
                yer++;
            }
            return -1;
        }

        /// <summary>Görev Yaz: kullanıcıların birimlere göre görevleri (yalnızca sistem yöneticisi). Şablon: KYM\KullaniciGorevGruplu.xltx.</summary>
        private static IResult GorevRaporu(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, false, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!GenelGorevUclari.YetkiVar(kullanan)) return Results.Json(new { hata = "Bu raporu sistem yöneticileri alabilir!" });
            return GorevRaporuUret();
        }

        private static IResult GorevRaporuUret()
        {
            try
            {
                AsposeLisans.Yukle();
                string sonucDosya = DosyaIslem.DosyaAdUret("KullaniciGorev", OrtakFonksiyonlar.TarihYYYYMMDD(new TNSDateTime(DateTime.Now))) + "." + GenelIslemler.ExcelTur();
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaAc(Path.Combine(RaporSablonYol(), "KYM", "KullaniciGorevGruplu.xltx"), sonucDosya);

                int fSat = 0, fSut = 0, baslaSatir = 0, sutun = 0, baslikSatir = 0;
                XLS.HucreAdAdresCoz("FormatSatir", ref fSat, ref fSut);
                XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
                XLS.HucreAdAdresCoz("Baslik", ref baslikSatir, ref sutun);

                TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
                string[] sql = new string[1];
                string[] baglanti = new string[1];
                baglanti[0] = "BaglantiSatiriKYM";

                sql[0] = @"SELECT GP.GOREVORGKOD, GP.GOREVSIRANO, G.AD, COUNT(*) SAY
                              FROM GRVGENELGOREV G
                              INNER JOIN GRVGENELGOREVPERSONEL GP on GP.GOREVORGKOD=G.ORGKOD AND GP.GOREVSIRANO=G.GOREVSIRANO
                              group by GP.GOREVORGKOD, GP.GOREVSIRANO, G.AD
                              having count(*)>0
                              order by 3";

                SortedList sl = uzyServis.SorguBirlestir(sql, baglanti, new int[] { 0, 1 });
                List<BirimGenelGorev> l = new List<BirimGenelGorev>();
                int yer = 0;
                foreach (DictionaryEntry de in sl)
                {
                    TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                    TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                    BirimGenelGorev g = new BirimGenelGorev();
                    g.OrgKod = alAnahtar.IntAl(0);
                    g.siraNo = alAnahtar.IntAl(1);
                    g.ad = alVeri.StrAl(0);
                    l.Add(g);
                    if (yer > 0)
                    {
                        XLS.HucreKopyala(fSat, fSut + 5, baslaSatir - 1, fSut + 5, fSat, fSut + 5 + yer);
                    }
                    XLS.HucreDegerYaz(baslaSatir - 1, fSut + 5 + yer, g.ad);
                    yer++;
                }
                XLS.HucreBirlestir(baslikSatir, sutun, baslikSatir, sutun + 5 + yer);
                XLS.HucreDegerYaz(baslikSatir, sutun, TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMAD") + " SGB.Net Kullanıcıları");
                sql[0] = @"SELECT case when UBP.ADI is null then '-' else UBP.ADI end, case when BP.ADI is null then '-' else BP.ADI end, LTRIM(RTRIM(GP.TCKIMLIKNO)), GP.ORGKOD, GP.GOREVORGKOD, GP.GOREVSIRANO,
                            K.AD, K.SOYAD, UB.KOPRUKOD, B.KOPRUKOD, UB.ADI, B.ADI, G.AD
                            FROM GRVGENELGOREVPERSONEL GP
                            LEFT OUTER JOIN GRVGENELGOREV G on G.ORGKOD = GP.GOREVORGKOD AND G.GOREVSIRANO = GP.GOREVSIRANO
                            LEFT OUTER JOIN ORGBIRIM B on B.KOD = GP.ORGKOD
                            LEFT OUTER JOIN ORGBIRIM UB on UB.KOD = B.ANABIRIMKOD
                            LEFT OUTER JOIN KULLANICI K on K.MERNIS = LTRIM(RTRIM(GP.TCKIMLIKNO))

                            LEFT OUTER JOIN Personel.dbo.PERPERSONEL PER  on PER.TCKIMLIKNO = LTRIM(RTRIM(GP.TCKIMLIKNO))
                            LEFT OUTER JOIN ORGBIRIM BP on BP.KOD = PER.ORGANIZASYONKOD
                            LEFT OUTER JOIN ORGBIRIM UBP on UBP.KOD = BP.ANABIRIMKOD
                            order by 1,2,3,4,5,7";

                sl = uzyServis.SorguBirlestir(sql, baglanti, new int[] { 0, 1, 2, 3, 4, 5 });

                int satir = baslaSatir - 1;
                string oncTC = "";
                int oncOrgKod = -1;
                foreach (DictionaryEntry de in sl)
                {
                    TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                    TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                    string bulunguBirimAd = alAnahtar.StrAl(1);
                    string tc = alAnahtar.StrAl(2);
                    int orgKod = alAnahtar.IntAl(3);
                    int gorevOrgKod = alAnahtar.IntAl(4);
                    int gorevSiraNo = alAnahtar.IntAl(5);

                    string ad = alVeri.StrAl(0);
                    string soyad = alVeri.StrAl(1);
                    string birimAdi = alVeri.StrAl(5);

                    if (oncTC != tc || oncOrgKod != orgKod)
                    {
                        oncOrgKod = orgKod;
                        oncTC = tc;
                        satir++;
                        XLS.HucreKopyala(fSat, fSut, fSat, fSut + 5 + yer, satir, fSut);

                        XLS.HucreDegerYaz(satir, 0, tc);
                        XLS.HucreDegerYaz(satir, 1, ad);
                        XLS.HucreDegerYaz(satir, 2, soyad);
                        XLS.HucreDegerYaz(satir, 3, bulunguBirimAd);
                        XLS.HucreDegerYaz(satir, 4, birimAdi);
                    }
                    int gorevYer = GorevYerBul(l, gorevOrgKod, gorevSiraNo);
                    if (gorevYer >= 0)
                    {
                        XLS.HucreDegerYaz(satir, 5 + gorevYer, 1);
                    }
                }

                XLS.SatirYukseklikAyarla(baslaSatir, satir, -1, 5, 15);
                XLS.SatirSil(0, 1);
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosya, Path.GetFileName(sonucDosya), true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }

        // ------------------------------------------------------------------ yazma

        private static string Kirp(string deger)
        {
            return (deger ?? "").Trim();
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, OrgBirimIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, true, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            if (Kirp(istek.Ad) == "") return Hata("Birimin adını yazınız.");
            string cografya = Kirp(istek.Cografya);
            bool ulke = cografya == "ulke", bolge = cografya == "bolge", il = cografya == "il", ilce = cografya == "ilce";
            if (!istek.Sablon && il && Kirp(istek.Il) == "") return Hata("İl bilgisi boş bırakılamaz.");
            // Ambar ile harcama / taşınır birimi birlikte seçilemez (eski sayfadaki gibi).
            if (istek.Ambar && (istek.Harcama || istek.Tasinir)) return Hata("Ambar, harcama ya da taşınır birimiyle birlikte seçilemez.");

            OrganizasyonBirim birim = new OrganizasyonBirim();
            birim.kod = Math.Max(0, istek.Kod);
            birim.anaBirimKod = Math.Max(0, istek.AnaBirimKod);
            birim.merkezKod = Math.Max(0, istek.MerkezKod);
            birim.adi = Kirp(istek.Ad);
            birim.entegreKod = Kirp(istek.EntegreKod);
            birim.kisaadi = Kirp(istek.KisaAd);
            birim.sablonMu = istek.Sablon;
            birim.turKod = Kirp(istek.TurKod);

            if (istek.Yonetici)
            {
                birim.yoneticiKod = Math.Max(0, istek.YoneticiKod);
                // Üst yöneticiler yalnızca bir üst birimi olan birimlerde tanımlanır.
                if (birim.anaBirimKod > 0)
                {
                    birim.ustYoneticiKod1 = Math.Max(0, istek.UstYoneticiKod1);
                    birim.ustYoneticiKod2 = Math.Max(0, istek.UstYoneticiKod2);
                }
                if (birim.yoneticiKod > 0) birim.yoneticiTCKimlik = Kirp(istek.YoneticiTc);
                if (birim.ustYoneticiKod1 > 0) birim.ustYoneticiTCKimlik1 = Kirp(istek.UstYoneticiTc1);
                if (birim.ustYoneticiKod2 > 0) birim.ustYoneticiTCKimlik2 = Kirp(istek.UstYoneticiTc2);
            }
            if (istek.Harcama || istek.Tasinir || istek.Ambar)
                birim.muhasebekodu = Kirp(istek.Muhasebe);
            if (istek.Harcama || istek.Tasinir)
            {
                List<OrgHarcamaKod> kodlar = new List<OrgHarcamaKod>();
                foreach (OrgHarcamaKodIstegi h in istek.HarcamaKodlari ?? new OrgHarcamaKodIstegi[0])
                {
                    if (h == null) continue;
                    OrgHarcamaKod o = new OrgHarcamaKod();
                    o.kurKod = Kirp(h.KurKod);
                    o.kurAd = Kirp(h.KurAd);
                    o.muhasebeBirimKod = Kirp(h.MuhasebeBirimKod);
                    o.muhasebeBirimAd = Kirp(h.MuhasebeBirimAd);
                    o.fonKod = Kirp(h.FonKod);
                    o.fonAd = Kirp(h.FonAd);
                    if (o.kurKod == "" && o.muhasebeBirimKod == "" && o.fonKod == "") continue;
                    kodlar.Add(o);
                }
                birim.orgHarcamaKod = kodlar.ToArray();
                if (kodlar.Count > 0) birim.muhasebebirim = kodlar[0].muhasebeBirimKod;
            }
            if (istek.Harcama)
            {
                birim.vergino = Kirp(istek.VergiNo);
                birim.vergidairesi = Kirp(istek.VergiDairesi);
            }
            if (istek.Butce)
            {
                birim.orgButceKod = new[]
                {
                    new OrgButceKod { tur = (int)ENUMORGButceKodTur.MERKEZ, kurKod = Kirp(istek.MerkezKurKod) },
                    new OrgButceKod { tur = (int)ENUMORGButceKodTur.TASRA, kurKod = Kirp(istek.TasraKurKod) },
                    new OrgButceKod { tur = (int)ENUMORGButceKodTur.YURTDISI, kurKod = Kirp(istek.YurtDisiKurKod) }
                };
                birim.altFaaliyetKodlar = Kirp(istek.FaaliyetKodlari);
            }
            if (istek.Evrak)
            {
                birim.evrakbirim = Kirp(istek.EvrakBirim);
                birim.antet = Kirp(istek.Antet);
            }
            if (istek.Iletisim)
            {
                birim.adres = Kirp(istek.Adres);
                birim.telefon = Kirp(istek.Telefon);
                birim.faks = Kirp(istek.Faks);
                birim.eposta = Kirp(istek.Eposta);
                birim.url = Kirp(istek.Url);
            }
            if (istek.Kurum) birim.kurumKod = Kirp(istek.KurumKod);
            if (istek.Ambar)
            {
                birim.kurumsalkod = Kirp(istek.AmbarKurKod);
                birim.muhasebebirim = Kirp(istek.AmbarBirimKod);
                birim.ambarKod = Kirp(istek.AmbarKod);
            }

            if (ulke || bolge || il || ilce) birim.ulke = Kirp(istek.Ulke);
            if (bolge || il || ilce) birim.bolge = Kirp(istek.Bolge);
            if (il || ilce) birim.il = Kirp(istek.Il);
            if (ilce)
            {
                birim.ilce = Kirp(istek.Ilce);
                if (birim.ilce.Length > 8) birim.il = birim.ilce.Substring(0, 7);
            }

            // "Grup" (yalnızca yer tutan birim): başka hiçbir özelliği olmayan birim.
            bool ozellikVar = istek.Harcama || istek.Butce || istek.Tasinir || istek.Personel || istek.Evrak || ulke || bolge || il || ilce
                              || istek.Iletisim || istek.Kurum || istek.Ambar;
            birim.yerMi = ozellikVar ? 0 : 1;
            birim.harcamaMi = istek.Harcama ? 1 : 0;
            birim.butceMi = istek.Butce ? 1 : 0;
            birim.tasinirMi = istek.Tasinir ? 1 : 0;
            birim.personelMi = istek.Personel ? 1 : 0;
            birim.evrakMi = istek.Evrak ? 1 : 0;
            birim.ulkeMi = ulke ? 1 : 0;
            birim.bolgeMi = bolge ? 1 : 0;
            birim.ilMi = il ? 1 : 0;
            birim.ilceMi = ilce ? 1 : 0;
            birim.iletisimMi = istek.Iletisim ? 1 : 0;
            birim.cizimCikisNoktasi = istek.CizimCikisNoktasi;
            birim.cizimdeGorunme = istek.CizimdeGorunme;
            birim.kurumMu = istek.Kurum ? 1 : 0;
            birim.ambarMi = istek.Ambar ? 1 : 0;

            Sonuc sonuc = TNS.ORG.Arac.Tanimla().OrganizasyonBirimKaydet(kullanan, birim);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Birim kaydedilemedi."));

            int kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, birim.kod);
            return Results.Json(new { basarili = true, bilgi = DuzBilgi(sonuc, "Birim kaydedildi."), kod = kod, anaBirimKod = birim.anaBirimKod });
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, OrgBirimKodIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, true, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Kod <= 0) return Results.BadRequest();

            IORGServis servis = TNS.ORG.Arac.Tanimla();
            // Alt birimi olan birim silinemez.
            if (servis.OrganizasyonBirimListele(new OrganizasyonBirim { anaBirimKod = istek.Kod }, false, false).objeler.Count > 0)
                return Hata("Alt birimi olan birim silinemez. Önce alt birimleri siliniz.");

            Sonuc sonuc = servis.OrganizasyonBirimSil(kullanan, new OrganizasyonBirim { kod = istek.Kod, sablonMu = istek.Sablon });
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Birim silinemedi."));
            return Results.Json(new { basarili = true, bilgi = "Birim silindi." });
        }

        private static async Task<IResult> BirimAc(HttpContext baglam, IWebHostEnvironment ortam, OrgBirimKodIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, true, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Kod <= 0) return Results.BadRequest();

            Sonuc sonuc = TNS.ORG.Arac.Tanimla().OrganizasyonBirimAc(kullanan, istek.Kod);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Birim açılamadı."));
            return Results.Json(new { basarili = true, bilgi = DuzBilgi(sonuc, "Birim açıldı.") });
        }

        private static async Task<IResult> BirimKapat(HttpContext baglam, IWebHostEnvironment ortam, OrgBirimKodIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, true, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Kod <= 0) return Results.BadRequest();

            Sonuc sonuc = TNS.ORG.Arac.Tanimla().OrganizasyonBirimKapat(kullanan, istek.Kod);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Birim kapatılamadı."));
            return Results.Json(new { basarili = true, bilgi = DuzBilgi(sonuc, "Birim kapatıldı.") });
        }

        // ------------------------------------------------------------------ yardımcılar

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static string HataMetni(Sonuc sonuc, string varsayilan)
        {
            string hata = sonuc.hataStr ?? "";
            return hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at ") ? varsayilan : hata;
        }

        private static string DuzBilgi(Sonuc sonuc, string varsayilan)
        {
            string bilgi = (sonuc.bilgiStr ?? "").Trim();
            return bilgi == "" || bilgi.Contains("<") ? varsayilan : bilgi;
        }
    }
}
