using System;
using System.Collections.Generic;
using System.Globalization;
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
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Onayla / İptal Et / Yeni Yap / Sil isteği (eski btnOnayla_Click: seçili satırlar + onayKod).</summary>
    public class KadroOnayIstegi
    {
        public int personelKod { get; set; }
        public List<int> kodlar { get; set; }
        public int onayKod { get; set; }
    }

    /// <summary>Belge silme isteği (eski BelgeSil).</summary>
    public class KadroBelgeSilIstegi
    {
        public int kod { get; set; }
    }

    /// <summary>HİTAP'tan Al / Veri Al isteği (eski PersonelHITAP.aspx btnVeriAl_Click).</summary>
    public class KadroHitapIstegi
    {
        public int personelKod { get; set; }
        public int tur { get; set; }
        public string kullanici { get; set; }
        public string parola { get; set; }
        public int personelTip { get; set; }
        public int organizasyonKodu { get; set; }
    }

    /// <summary>Hizmet Ayarları / Kaydet isteği (eski PersonelHizmetAyari.aspx btnKaydet_Click).</summary>
    public class KadroHizmetAyariIstegi
    {
        public int personelKod { get; set; }
        public int kTerfiAyi { get; set; }
        public int eTerfiAyi { get; set; }
        public string kAcilisTarihi { get; set; }
        public string eAcilisTarihi { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri / Hizmet Belgesi sekmesi (eski InsanKaynaklari/PersonelKadro.aspx) ve açtığı pencerelerin uçları:
    ///   GET  /api/personelbilgi/kadro/liste?personelKod=            hizmet kayıtları (eski Listele) + belgeler (Manas)
    ///   POST /api/personelbilgi/kadro/onay                           Onayla (5) / İptal Et (9) / Yeni Yap (1) / Sil (10) (eski btnOnayla_Click)
    ///   GET  /api/personelbilgi/kadro/hizmetbelgesi?personelKod=     Hizmet Belgesi Yazdır (PDF; eski HizmetBelgesiYazdir)
    ///   GET  /api/personelbilgi/kadro/belgeler?personelKod=          ekli belgeler (eski BelgeListesiDoldur; yalnız Manas)
    ///   POST /api/personelbilgi/kadro/belgekaydet                    (form: personelKod, dosya) (eski BelgeKaydet)
    ///   GET  /api/personelbilgi/kadro/belgegoster?kod=               (eski BelgeGoster)
    ///   POST /api/personelbilgi/kadro/belgesil                       (eski BelgeSil)
    ///   GET  /api/personelbilgi/kadro/hitap/bilgi                    HİTAP penceresi: kayıtlı kullanıcı / parola, personel türleri
    ///   GET  /api/personelbilgi/kadro/hitap/birimler?ust=            HİTAP penceresi birim ağacı (eski ORGBirim NodeLoad, personelMi=1)
    ///   POST /api/personelbilgi/kadro/hitap/verial                   (eski PersonelHITAP btnVeriAl_Click)
    ///   GET  /api/personelbilgi/kadro/hizmetayari?personelKod=       (eski PersonelHizmetAyari Listele + AyDoldur)
    ///   POST /api/personelbilgi/kadro/hizmetayari/kaydet             (eski PersonelHizmetAyari btnKaydet_Click)
    ///   GET  /api/personelbilgi/kadro/hizmetsureleri?personelKod=    (eski PersonelHizmetSureleri HizmetSureleriniHesapla)
    /// Kişi Borcu düğmesi ortak /api/personelbilgi/rapor/kisiborcu ucunu kullanır (burada yazılmadı).
    /// </summary>
    public static class PersonelKadroUclari
    {
        private const string Sayfa = "PersonelKadro";
        private const string SayfaHitap = "PersonelHITAP";
        private const string SayfaHizmetAyari = "PersonelHizmetAyari";
        private const string SayfaHizmetSureleri = "PersonelHizmetSureleri";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/kadro/liste", Liste);
            app.MapPost("/api/personelbilgi/kadro/onay", Onay);
            app.MapGet("/api/personelbilgi/kadro/hizmetbelgesi", HizmetBelgesi);
            app.MapGet("/api/personelbilgi/kadro/belgeler", Belgeler);
            app.MapPost("/api/personelbilgi/kadro/belgekaydet", BelgeKaydet);
            app.MapGet("/api/personelbilgi/kadro/belgegoster", BelgeGoster);
            app.MapPost("/api/personelbilgi/kadro/belgesil", BelgeSil);
            app.MapGet("/api/personelbilgi/kadro/hitap/bilgi", HitapBilgi);
            app.MapGet("/api/personelbilgi/kadro/hitap/birimler", HitapBirimler);
            app.MapPost("/api/personelbilgi/kadro/hitap/verial", HitapVeriAl);
            app.MapGet("/api/personelbilgi/kadro/hizmetayari", HizmetAyariOku);
            app.MapPost("/api/personelbilgi/kadro/hizmetayari/kaydet", HizmetAyariKaydet);
            app.MapGet("/api/personelbilgi/kadro/hizmetsureleri", HizmetSureleri);
        }

        private static IIKYServis Servis() { return TNS.IKY.Arac.Tanimla(); }

        private static bool ManasMi()
        {
            try { return TNS.DEG.Arac.ManasKullaniyor(); }
            catch (Exception) { return false; }
        }

        // ---------------------------------------------------------------- Hizmet listesi

        /// <summary>Eski Listele: PersonelKadroListele(personelKod, tarihArtan=false); Manas kullanılıyorsa belge listesi de.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, Sayfa, out Kullanici kullanan);
            if (ret != null) return ret;

            bool manas = ManasMi();
            int pKod = personelKod ?? 0;
            if (pKod == 0) return Results.Json(new { satirlar = new object[0], belgeVar = manas, belgeler = new object[0] });

            ObjectArray pler = Servis().PersonelKadroListele(kullanan, new TNS.IKY.PersonelKadro { personelKod = pKod }, false);
            if (!pler.sonuc.islemSonuc)
                return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(pler.sonuc.hataStr, "Hizmet bilgileri okunamadı."), belgeVar = manas });

            List<object> satirlar = new List<object>();
            foreach (TNS.IKY.PersonelKadro k in pler.objeler)
            {
                satirlar.Add(new
                {
                    k.kod,
                    k.organizasyonKodu,
                    k.birimGorevAd,
                    k.unvan,
                    k.unvanAd,
                    k.sinifi,
                    k.evrakNo,
                    k.yasalDayanak,
                    k.aciklama,
                    k.kadroDerece,
                    k.kazanilmisHakDerece,
                    k.kazanilmisHakKademe,
                    k.kazanilmisGosterge,
                    k.kazanilmisEkGosterge,
                    k.odemeyeEsasDerece,
                    k.odemeyeEsasKademe,
                    k.odemeyeEsasGosterge,
                    k.odemeyeEsasEkGosterge,
                    k.emekliyeEsasDerece,
                    k.emekliyeEsasKademe,
                    k.emekliyeEsasGosterge,
                    k.emekliyeEsasEkGosterge,
                    kadroDurum = (int)k.kadroDurum,
                    k.kadroDurumAd,
                    k.ayrilmaNedeni,
                    k.gorevYeri,
                    k.hizmetSebebi,
                    k.hizmetSebebiKod,
                    k.belgeCinsi,
                    k.belgeNo,
                    k.eskiHizmetTipi,
                    atamaTarihi = PersonelBilgiOrtak.TarihYaz(k.atamaTarihi),
                    ayrilmaTarihi = PersonelBilgiOrtak.TarihYaz(k.ayrilmaTarihi),
                    onayTarih = PersonelBilgiOrtak.TarihYaz(k.onayTarih),
                    k.personelTipAd,
                    k.hizmetSebebiAd,
                    k.ayrilmaGerekcesiAd
                });
            }

            List<object> belgeler = manas ? BelgeListesi(kullanan, pKod) : new List<object>();
            return Results.Json(new { satirlar = satirlar, belgeVar = manas, belgeler = belgeler });
        }

        // ---------------------------------------------------------------- Onay işlemleri

        /// <summary>Eski btnOnayla_Click: seçili satırlar AtamaOnayBilgileriniGuncelle'ye onayKod (5 onay, 9 red, 1 yeni yap, 10 sil) ile gider.</summary>
        private static async Task<IResult> Onay(HttpContext baglam, IWebHostEnvironment ortam, KadroOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, Sayfa, out Kullanici kullanan);
            if (ret != null) return ret;

            int onayKod = istek.onayKod;
            if (onayKod != 1 && onayKod != 5 && onayKod != 9 && onayKod != 10) onayKod = 5; // eski ConvertToInt(onayKod, 5)

            TNSCollection tc = new TNSCollection();
            foreach (int kod in istek.kodlar ?? new List<int>())
            {
                TNS.IKY.PersonelKadro kadro = new TNS.IKY.PersonelKadro();
                kadro.personelKod = istek.personelKod;
                kadro.kod = kod;
                tc.Add(kadro);
            }

            Sonuc sonuc = Servis().AtamaOnayBilgileriniGuncelle(kullanan, tc, onayKod);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Seçili işlemlerin durumu değiştirilemedi.") });
            string goster = string.IsNullOrWhiteSpace(sonuc.bilgiStr) ? "Seçili işlemlerin durumu başarı ile değiştirildi." : PersonelBilgiOrtak.Duz(sonuc.bilgiStr);
            return Results.Json(new { basarili = true, bilgi = goster });
        }

        // ---------------------------------------------------------------- Hizmet Belgesi

        /// <summary>Eski HizmetBelgesiYazdir: IKY\HizmetBelgesi.docx şablonundan PDF.</summary>
        private static IResult HizmetBelgesi(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, Sayfa, out Kullanici kullanan);
            if (ret != null) return ret;
            int pKod = personelKod ?? 0;
            if (pKod == 0) return Results.Json(new { hata = "İlk önce Personel seçiniz" });

            string sablon = PersonelKadroRaporlari.SablonBul(ortam.ContentRootPath, PersonelKadroRaporlari.HizmetBelgesiSablon);
            if (sablon == null) return Results.Json(new { hata = "Rapor şablonu (IKY\\" + PersonelKadroRaporlari.HizmetBelgesiSablon + ") bulunamadı." });
            try
            {
                string dosya = PersonelKadroRaporlari.HizmetBelgesiYazdir(kullanan, Servis(), pKod, sablon, out string hata);
                if (dosya == null) return Results.Json(new { hata = hata ?? "Rapor hazırlanamadı." });
                return DosyaIslem.DosyaGonder(dosya, "HizmetBelgesi.pdf", true, "pdf");
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }

        // ---------------------------------------------------------------- Belgeler (yalnız Manas)

        private static List<object> BelgeListesi(Kullanici kullanan, int pKod)
        {
            PersonelBelgeler belge = new PersonelBelgeler();
            belge.pKod = pKod;
            belge.tip = (int)ENUMBELGEFORMTIP.PERSONELKADRO;
            belge.tipKayitKod = pKod;

            ObjectArray belgeler = Servis().PersonelBelgeListele(kullanan, belge);
            List<object> liste = new List<object>();
            if (belgeler == null || belgeler.objeler == null) return liste;
            foreach (PersonelBelgeler detay in belgeler.objeler)
            {
                liste.Add(new
                {
                    KOD = detay.kod,
                    PKOD = detay.pKod,
                    TIPKAYITKOD = detay.tipKayitKod,
                    TIP = detay.tip,
                    BELGEADI = detay.belgeAdi
                });
            }
            return liste;
        }

        private static IResult Belgeler(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, Sayfa, out Kullanici kullanan);
            if (ret != null) return ret;
            int pKod = personelKod ?? 0;
            if (pKod == 0 || !ManasMi()) return Results.Json(new { belgeler = new object[0] });
            return Results.Json(new { belgeler = BelgeListesi(kullanan, pKod) });
        }

        /// <summary>Eski BelgeKaydet (Belge Ekle düğmesi).</summary>
        private static async Task<IResult> BelgeKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, Sayfa, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!baglam.Request.HasFormContentType) return Results.BadRequest();

            IFormCollection form = await baglam.Request.ReadFormAsync();
            string pKodStr = (form["personelKod"].ToString() ?? "").Trim();
            if (string.IsNullOrEmpty(pKodStr))
                return Results.Json(new { basarili = false, hata = "Belgenin kim için kaydedileceğini seçin." });
            int pKod = OrtakFonksiyonlar.ConvertToInt(pKodStr, 0);
            if (pKod == 0) return Results.Json(new { basarili = false, hata = "Belgenin kim için kaydedileceğini seçin." });

            IFormFile dosya = form.Files["dosya"];
            int belgeBoyutu = dosya == null ? 0 : (int)Math.Min(dosya.Length, int.MaxValue);
            if (belgeBoyutu <= 0) return Results.Json(new { basarili = false, hata = "Yüklemek istediğiniz dosyanın içi boş." });

            PersonelBelgeler yeniBelge = new PersonelBelgeler(pKod, pKod, (int)ENUMBELGEFORMTIP.PERSONELKADRO, dosya.FileName ?? "belge", belgeBoyutu);
            using (MemoryStream ms = new MemoryStream())
            {
                await dosya.CopyToAsync(ms);
                yeniBelge.belge = ms.ToArray();
            }

            Sonuc sonuc = Servis().PersonelBelgeKaydet(kullanan, yeniBelge);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Belge kaydedilemedi."), belgeler = BelgeListesi(kullanan, pKod) });
            return Results.Json(new { basarili = true, bilgi = PersonelBilgiOrtak.Duz(sonuc.bilgiStr), belgeler = BelgeListesi(kullanan, pKod) });
        }

        /// <summary>Eski BelgeGoster: iş kuralı dosyanın yolunu, adını ve uzantısını "\n" ile ayırarak verir.</summary>
        private static IResult BelgeGoster(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, Sayfa, out Kullanici kullanan);
            if (ret != null) return ret;
            Sonuc dosyaKonumuAdiUzantisi = Servis().PersonelBelgeListesiGetir(kullanan, new PersonelBelgeler { kod = kod ?? 0 });
            if (dosyaKonumuAdiUzantisi == null || !dosyaKonumuAdiUzantisi.islemSonuc)
                return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(dosyaKonumuAdiUzantisi?.hataStr, "Belge okunamadı.") });
            string[] parametreler = (dosyaKonumuAdiUzantisi.anahtar ?? "").Split('\n');
            if (parametreler.Length != 3 || !File.Exists(parametreler[0])) return Results.Json(new { hata = "Belge okunamadı." });
            return DosyaIslem.DosyaGonder(parametreler[0], parametreler[1], true, parametreler[2]);
        }

        /// <summary>Eski BelgeSil.</summary>
        private static async Task<IResult> BelgeSil(HttpContext baglam, IWebHostEnvironment ortam, KadroBelgeSilIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, Sayfa, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek.kod == 0) return Results.Json(new { basarili = false, hata = "Belge seçiniz." });
            Sonuc sonuc = Servis().PersonelBelgeSil(kullanan, new PersonelBelgeler { kod = istek.kod });
            if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Belge silinemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Belge silindi." });
        }

        // ---------------------------------------------------------------- HİTAP penceresi

        /// <summary>Eski PersonelHITAP Page_Load: kullanıcı değişkenlerinden HİTAP kullanıcı / parolası, Genel.PersonelTurDoldur.</summary>
        private static IResult HitapBilgi(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaHitap, out Kullanici kullanan);
            if (ret != null) return ret;

            List<object> tipler = new List<object>();
            tipler.Add(new { kod = 0, ad = "" });
            TNSCollection liste = TNS.IKY.Arac.PersonelTipListe;
            if (liste != null)
                foreach (PersonelTipi t in liste)
                    tipler.Add(new { t.kod, t.ad });

            return Results.Json(new
            {
                kullanici = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "HITAPKUL"),
                parola = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "HITAPPAR"),
                personelTipleri = tipler
            });
        }

        /// <summary>
        /// HİTAP penceresinin birim ağacı (eski OrgBirimDoldur -> ORGBirim NodeLoad, { personelMi: 1, yetkiliOlduguMuhasebeBirim }):
        /// kökte kullanıcının "Personel Saymanlık-Birim Yetkisi" varsa o birimler, yoksa bütün kök birimler.
        /// </summary>
        private static IResult HitapBirimler(HttpContext baglam, IWebHostEnvironment ortam, int? ust)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaHitap, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                TNS.ORG.OrganizasyonBirim kriter = new TNS.ORG.OrganizasyonBirim();
                kriter.anaBirimKod = Math.Max(0, ust ?? 0);
                kriter.sablonMu = false;
                kriter.personelMi = 1;
                if (kriter.anaBirimKod == 0)
                {
                    string yetkili = kullanan.YetkiTipiDegeriBul("Personel Saymanlık-Birim Yetkisi") ?? "";
                    bool birimYetkisiVar = false;
                    foreach (string y in yetkili.Split(';'))
                    {
                        string[] m = y.Split('-');
                        if (m.Length == 2 && (OrtakFonksiyonlar.ConvertToInt(m[0], 0) > 0 || OrtakFonksiyonlar.ConvertToInt(m[1], 0) > 0))
                            birimYetkisiVar = true;
                    }
                    if (birimYetkisiVar) kriter.yetkiliOlduguMuhasebeBirim = yetkili;
                    else kriter.kopruKod = "999";
                }

                ObjectArray<TNS.ORG.OrganizasyonBirim> sonuc = TNS.ORG.Arac.Tanimla().OrganizasyonBirimListele(kriter, false, false);
                if (sonuc == null || !sonuc.sonuc.islemSonuc) return Results.Json(new { hata = "Birimler okunamadı." });
                List<object> satirlar = new List<object>();
                foreach (TNS.ORG.OrganizasyonBirim b in sonuc.objeler)
                    satirlar.Add(new { kod = b.kod, ad = b.adi ?? "", altVar = b.altDalVar });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Birimler okunamadı." });
            }
        }

        /// <summary>
        /// Eski btnVeriAl_Click: HİTAP kullanıcı / parolası kullanıcı değişkenine saklanır, aktarım türü 1 ise
        /// Hitap.HitapHizmetleriAl ile SGK HİTAP servisinden hizmetler alınır. HİTAP SOAP istemcisi (WebServisler.SGKHitapWS)
        /// Core'da olmadığından aktarım adımı yapılamaz; kullanıcıya bilgi verilir (NOTLAR.md).
        /// </summary>
        private static async Task<IResult> HitapVeriAl(HttpContext baglam, IWebHostEnvironment ortam, KadroHitapIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaHitap, out Kullanici kullanan);
            if (ret != null) return ret;

            try
            {
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "HITAPKUL", istek.kullanici ?? "");
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "HITAPPAR", istek.parola ?? "");
                int aktarTur = istek.tur == 0 ? 1 : istek.tur; // eski RequestBul(this, "tur", 1)
                if (aktarTur != 1) return Results.Json(new { basarili = false, hata = "Aktarım türü tanımsız" });
                if (istek.personelKod <= 0) return Results.Json(new { basarili = false, hata = "Lütfen personel seçiniz!" });

                return Results.Json(new
                {
                    basarili = false,
                    hata = "HİTAP bağlantısı bu sürümde yok: SGK HİTAP web servis istemcisi henüz taşınmadı. Kullanıcı kodu ve parola kaydedildi."
                });
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(ex.Message, "Aktarım yapılamadı.") });
            }
        }

        // ---------------------------------------------------------------- Hizmet Ayarları penceresi

        /// <summary>Eski PersonelHizmetAyari Page_Load: Genel.AyDoldur (ilk seçenek boş) + Listele (PersonelKadroAyarListele).</summary>
        private static IResult HizmetAyariOku(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaHizmetAyari, out Kullanici kullanan);
            if (ret != null) return ret;

            DateTimeFormatInfo tr = new CultureInfo("tr-TR").DateTimeFormat;
            List<object> aylar = new List<object>();
            aylar.Add(new { KOD = 0, AD = string.Empty });
            for (int i = 1; i < 13; i++)
                aylar.Add(new { KOD = i, AD = tr.GetMonthName(i) });

            ObjectArray arr = Servis().PersonelKadroAyarListele(kullanan, new KadroAyar { pKod = personelKod ?? 0 });
            if (!arr.sonuc.islemSonuc)
                return Results.Json(new { aylar = aylar, hata = PersonelBilgiOrtak.HataMetni(arr.sonuc.hataStr, "Hizmet ayarları okunamadı.") });

            object ayar = null;
            if (arr.objeler.Count > 0)
            {
                KadroAyar a = (KadroAyar)arr.objeler[0];
                ayar = new
                {
                    a.kTerfiAyi,
                    a.eTerfiAyi,
                    kAcilisTarihi = PersonelBilgiOrtak.TarihYaz(a.kAcilisTarihi),
                    eAcilisTarihi = PersonelBilgiOrtak.TarihYaz(a.eAcilisTarihi)
                };
            }
            return Results.Json(new { aylar = aylar, ayar = ayar });
        }

        /// <summary>Eski PersonelHizmetAyari btnKaydet_Click.</summary>
        private static async Task<IResult> HizmetAyariKaydet(HttpContext baglam, IWebHostEnvironment ortam, KadroHizmetAyariIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaHizmetAyari, out Kullanici kullanan);
            if (ret != null) return ret;

            KadroAyar ayar = new KadroAyar();
            ayar.pKod = istek.personelKod;
            ayar.kTerfiAyi = istek.kTerfiAyi;
            ayar.eTerfiAyi = istek.eTerfiAyi;
            ayar.kAcilisTarihi = PersonelBilgiOrtak.TnsTarih(istek.kAcilisTarihi);
            ayar.eAcilisTarihi = PersonelBilgiOrtak.TnsTarih(istek.eAcilisTarihi);

            Sonuc sonuc = Servis().PersonelKadroAyarKaydet(kullanan, ayar);
            if (sonuc.islemSonuc) return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarılı." });
            return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Hizmet ayarları kaydedilemedi.") });
        }

        // ---------------------------------------------------------------- Hizmet Süreleri penceresi

        /// <summary>Eski PersonelHizmetSureleri HizmetSureleriniHesapla.</summary>
        private static IResult HizmetSureleri(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaHizmetSureleri, out Kullanici kullanan);
            if (ret != null) return ret;

            ObjectArray arr = Servis().HizmetleriGetir(kullanan, "", (personelKod ?? 0).ToString(), "", new TNSDateTime(), 0, "", -1);
            if (arr.sonuc.islemSonuc)
            {
                if (arr.objeler.Count > 0)
                {
                    Hizmet h = (Hizmet)arr.objeler[0];
                    return Results.Json(new
                    {
                        toplamHizmet = h.hizmetYilAyGun,
                        ekea = h.EKEAYilAyGun,
                        kah = h.kidemYilAyGun,
                        kha = h.KHAYilAyGun,
                        kurum = Hizmet.YilAyGunDondur(h.kurumdakiSure),
                        izin = Hizmet.YilAyGunDondur(h.izinHizmetGunu)
                    });
                }
                return Results.Json(new { hata = "Aranan personel bulunamadı!" });
            }
            return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(arr.sonuc.hataStr, "Hizmet süreleri hesaplanamadı.") });
        }
    }
}
