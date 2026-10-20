using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;
using TNS.ORG;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Kullanıcı İşlemleri isteklerinin ortak alanları.</summary>
    public class PersonelKullaniciIstegi
    {
        public string tcKimlikNo { get; set; }
        /// <summary>Yeni Kullanıcı Oluştur: önerilen ya da yazılan kullanıcı kodu.</summary>
        public string kullaniciKodu { get; set; }
        public string eskiParola { get; set; }
        public string yeniParola { get; set; }
        public string yeniParolaTekrar { get; set; }
        /// <summary>Görev Ekle: görev anahtarı (birimKod_siraNo) ve virgülle ayrılmış birim kodları.</summary>
        public string gorevKod { get; set; }
        public string birimler { get; set; }
        /// <summary>Rol Sil: seçili rol satırları.</summary>
        public List<PersonelKullaniciRolSatiri> satirlar { get; set; }
    }

    public class PersonelKullaniciRolSatiri
    {
        public int BIRIMKOD { get; set; }
        public int ROLKOD { get; set; }
        public int ROLSIRANO { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri "Kullanıcı İşlemleri" sekmesi (eski InsanKaynaklari/PersonelKullaniciIslemleri.aspx; yeni sayfa
    /// InsanKaynaklari/PersonelKullaniciIslemleri) uçları. Kişi adres satırındaki tcKimlikNo ile belirlenir (eski sayfa gibi).
    ///   GET  /api/personelbilgi/kullanici/durum?tcKimlikNo=     Kullanıcı var mı, roller / öneriler, görev listesi (eski Page_Load, KullaniciKontrol, GorevleriDoldur)
    ///   GET  /api/personelbilgi/kullanici/birimler?ust=         Birim ağacı (eski trpBirim, nodeLoadOrgBirimParam)
    ///   POST /api/personelbilgi/kullanici/olustur               Yeni Kullanıcı Oluştur (eski btnYeniKullaniciOlustur_Click)
    ///   POST /api/personelbilgi/kullanici/parola-degistir       Değiştir (eski btnDegistir_Click)
    ///   POST /api/personelbilgi/kullanici/yeni-parola           Yeni Parola Ata (eski btnYeniParolaAta_Click)
    ///   POST /api/personelbilgi/kullanici/rol-sil               Rolleri Kaldır (eski btnRolSil_Click)
    ///   POST /api/personelbilgi/kullanici/gorev-ekle            Görev Ekle (eski btnGorevEkle_Click)
    /// </summary>
    public static class PersonelKullaniciIslemleriUclari
    {
        private const string SayfaAd = "PersonelKullaniciIslemleri";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/kullanici/durum", Durum);
            app.MapGet("/api/personelbilgi/kullanici/birimler", Birimler);
            app.MapPost("/api/personelbilgi/kullanici/olustur", Olustur);
            app.MapPost("/api/personelbilgi/kullanici/parola-degistir", ParolaDegistir);
            app.MapPost("/api/personelbilgi/kullanici/yeni-parola", YeniParola);
            app.MapPost("/api/personelbilgi/kullanici/rol-sil", RolSil);
            app.MapPost("/api/personelbilgi/kullanici/gorev-ekle", GorevEkle);
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static string Tc(string tcKimlikNo)
        {
            return (tcKimlikNo ?? "").Trim();
        }

        // ---------------------------------------------------------------- durum (eski KullaniciKontrol)

        /// <summary>Eski GorevleriDoldur: kullananın "Personel Kullanıcı Görev Listesi" yetki tipinde olan genel görevler.</summary>
        private static List<object> Gorevler(Kullanici kullanan)
        {
            List<object> l = new List<object>();
            BirimGenelGorev[] gorevler = TNS.ORG.Arac.Tanimla().GenelGorevListele(kullanan, null, true);
            if (gorevler != null)
                for (int i = 0; i < gorevler.Length; i++)
                {
                    BirimGenelGorev bgg = gorevler[i];
                    if (kullanan.YetkiTipiDegeriIceriyorMu("Personel Kullanıcı Görev Listesi", bgg.siraNo.ToString(), false))
                        l.Add(new { bgg.Anahtar, bgg.ad });
                }
            return l;
        }

        /// <summary>Eski KullaniciYetkiListele.</summary>
        private static List<object> Roller(Kullanici kullanan, string tc)
        {
            List<object> yetkiler = new List<object>();
            if (tc == "") return yetkiler;
            KisiGorev kg = new KisiGorev();
            kg.tcKimlikNo = tc;
            KisiGorev[] kgList = TNS.ORG.Arac.Tanimla().KisiGenelGorevleriListele(kullanan, kg);
            if (kgList != null)
                foreach (KisiGorev item in kgList)
                    yetkiler.Add(new { BIRIMKOD = item.orgKod, BIRIMAD = item.orgAd, ROLKOD = item.gorevOrgKod, ROLSIRANO = item.gorevSiraNo, ROLAD = item.gorevAd });
            return yetkiler;
        }

        /// <summary>Eski OneriListele: personelin adı / soyadına göre önerilen kullanıcı kodları.</summary>
        private static List<object> Oneriler(Kullanici kullanan, string tc)
        {
            List<object> listOneri = new List<object>();
            TNS.IKY.Personel per = new TNS.IKY.Personel();
            per.tcKimlikNo = tc;
            ObjectArray perList = TNS.IKY.Arac.Tanimla().PersonelListele(kullanan, per, false, true);
            if (perList.sonuc.islemSonuc && perList.objeler.Count == 1)
            {
                per = (TNS.IKY.Personel)perList.objeler[0];
                Kullanici kul = new Kullanici();
                kul.ad = per.ad;
                kul.soyad = per.soyad;
                ObjectArray<Kullanici> oneriler = TNS.KYM.Arac.Tanimla().KullaniciAdOner(kul);
                if (oneriler != null && oneriler.objeler != null)
                    foreach (Kullanici item in oneriler.objeler)
                        listOneri.Add(item.kullaniciKodu);
            }
            return listOneri;
        }

        private static IResult Durum(HttpContext baglam, IWebHostEnvironment ortam, string tcKimlikNo)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            string tc = Tc(tcKimlikNo);
            try
            {
                List<object> gorevler = Gorevler(kullanan);
                KullaniciVAN kul = TNS.KYM.Arac.Tanimla().KullaniciOku(kullanan, tc);
                bool kullaniciYok = kul == null || kul.kullanici == null || string.IsNullOrWhiteSpace(kul.kullanici.kullaniciKodu);
                if (kul != null && kul.sonuc != null && kul.sonuc.islemSonuc && kul.kullanici != null)
                {
                    return Results.Json(new
                    {
                        kullaniciVar = !kullaniciYok,
                        kullaniciKodu = kul.kullanici.kullaniciKodu ?? "",
                        roller = Roller(kullanan, tc),
                        oneriler = new List<object>(),
                        gorevler = gorevler
                    });
                }
                return Results.Json(new
                {
                    kullaniciVar = !kullaniciYok,
                    kullaniciKodu = "",
                    roller = new List<object>(),
                    oneriler = Oneriler(kullanan, tc),
                    gorevler = gorevler
                });
            }
            catch (Exception ex)
            {
                return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(ex.Message, "Kullanıcı bilgileri okunamadı.") });
            }
        }

        // ---------------------------------------------------------------- birim ağacı

        /// <summary>
        /// Birim ağacı (eski trpBirim: ORGBirim.aspx NodeLoad, çoklu seçim). Kökte sistem yöneticisi bütün birimleri, diğerleri
        /// yetkili olduğu birimleri görür (Core'daki Organizasyon/OrgBirimUclari.Agac ile aynı kural).
        /// </summary>
        private static IResult Birimler(HttpContext baglam, IWebHostEnvironment ortam, int? ust)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                IORGServis servis = TNS.ORG.Arac.Tanimla();
                OrganizasyonBirim kriter = new OrganizasyonBirim();
                kriter.anaBirimKod = Math.Max(0, ust ?? 0);
                if (kriter.anaBirimKod == 0) kriter.kopruKod = "999";
                ObjectArray<OrganizasyonBirim> sonuc = kriter.anaBirimKod == 0 && !PersonelBilgiOrtak.SistemYoneticisi(kullanan)
                    ? servis.OrganizasyonBirimListeleKullanici(kullanan, false, false)
                    : servis.OrganizasyonBirimListele(kriter, false, false);
                if (!sonuc.sonuc.islemSonuc) return Results.Json(new { hata = "Birimler okunamadı." });
                List<object> satirlar = new List<object>();
                foreach (OrganizasyonBirim b in sonuc.objeler)
                    satirlar.Add(new { kod = b.kod, ad = b.adi, altVar = b.altDalVar });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Birimler okunamadı." });
            }
        }

        // ---------------------------------------------------------------- yeni kullanıcı

        /// <summary>Eski KullaniciOl: kullanıcı bilgileri personel kaydından doldurulur.</summary>
        private static Kullanici KullaniciOl(Kullanici kullanan, string tc, string kullaniciKodu, out string hata)
        {
            hata = "";
            Kullanici yeniKullanici = new Kullanici();
            yeniKullanici.kullaniciKodu = kullaniciKodu;
            yeniKullanici.mernis = tc;

            TNS.IKY.Personel per = new TNS.IKY.Personel();
            per.tcKimlikNo = tc;
            ObjectArray perList = TNS.IKY.Arac.Tanimla().PersonelListele(kullanan, per, false, true);
            if (perList.sonuc.islemSonuc)
            {
                if (perList.objeler.Count == 1)
                {
                    per = (TNS.IKY.Personel)perList.objeler[0];
                    yeniKullanici.ad = per.ad;
                    yeniKullanici.soyad = per.soyad;
                    yeniKullanici.unvan = per.unvanAd;
                    yeniKullanici.aktifMi = ENUMAktiflik.AKTIF;
                    yeniKullanici.anaAdi = per.nufus.anaAd;
                    yeniKullanici.babaAdi = per.nufus.babaAd;
                    yeniKullanici.calistigiBirim = per.kadro.gorevBirimAd;
                    yeniKullanici.calistigiKurum = per.kadro.muhasebeKodu.ToString();
                    yeniKullanici.dogumTarihi = per.dogumTarihi;
                    yeniKullanici.kullaniciTipi = new ENUMKullaniciTipi[1];
                    yeniKullanici.kullaniciTipi[0] = ENUMKullaniciTipi.MISAFIR;

                    if (per.cinsiyet == TNS.IKY.ENUMCINSIYET.ERKEK)
                        yeniKullanici.cinsiyet = "E";
                    else if (per.cinsiyet == TNS.IKY.ENUMCINSIYET.KADIN)
                        yeniKullanici.cinsiyet = "K";
                    else
                        yeniKullanici.cinsiyet = "";
                    yeniKullanici.emailAdresi = per.EPOSTA;
                    yeniKullanici.telefon = per.isTelefon;
                    if (string.IsNullOrEmpty(yeniKullanici.telefon)) yeniKullanici.telefon = "Girilmemiş";
                }
            }
            else
                hata = PersonelBilgiOrtak.HataMetni(perList.sonuc.hataStr, "Personel bilgileri okunamadı.");
            return yeniKullanici;
        }

        private static async Task<IResult> Olustur(HttpContext baglam, IWebHostEnvironment ortam, PersonelKullaniciIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string kullaniciKodu = istek.kullaniciKodu ?? "";
            if (kullaniciKodu.Trim() == string.Empty) return Hata("Kullanıcı Kodu Boş Olmamalıdır.");

            Kullanici yeniKullanici = KullaniciOl(kullanan, Tc(istek.tcKimlikNo), kullaniciKodu, out string personelHata);
            Sonuc sonuc = TNS.KYM.Arac.Tanimla().KullaniciKaydet(kullanan, yeniKullanici, true);
            if (!sonuc.islemSonuc)
                return Hata(PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Kullanıcı oluşturulamadı."));
            return Results.Json(new { basarili = true, bilgi = personelHata == "" ? "Kullanıcı oluşturuldu." : personelHata });
        }

        // ---------------------------------------------------------------- parola

        private static async Task<IResult> ParolaDegistir(HttpContext baglam, IWebHostEnvironment ortam, PersonelKullaniciIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string eskiparola = istek.eskiParola ?? "";
            string parola = istek.yeniParola ?? "";
            string parolatekrar = istek.yeniParolaTekrar ?? "";
            if (parola != parolatekrar) return Hata("Parola ve Parola Tekrarı Aynı Olmalıdır.");

            // Eski: salt okunur Kullanıcı Kodu alanındaki kod (KullaniciKontrol'de kişinin kullanıcı kaydından gelir); burada
            // aynı kod sunucuda kişinin kullanıcı kaydından okunur.
            KullaniciVAN kv = TNS.KYM.Arac.Tanimla().KullaniciOku(kullanan, Tc(istek.tcKimlikNo));
            string kullaniciKodu = kv != null && kv.kullanici != null ? (kv.kullanici.kullaniciKodu ?? "").Trim() : "";
            if (kullaniciKodu == string.Empty) return Hata("Kullanıcı Kodu Boş Olmamalıdır.");

            Kullanici kul = new Kullanici();
            kul.kullaniciKodu = kullaniciKodu;
            Sonuc sonuc = TNS.KYM.Arac.Tanimla().ParolaDegistir(kul, eskiparola, parola, parolatekrar);
            if (!sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Parola değiştirilemedi."));
            return Results.Json(new { basarili = true, bilgi = (sonuc.bilgiStr ?? "").Trim() == string.Empty ? "İşlem Başarıyla Gerçekleştirildi." : PersonelBilgiOrtak.Duz(sonuc.bilgiStr) });
        }

        private static async Task<IResult> YeniParola(HttpContext baglam, IWebHostEnvironment ortam, PersonelKullaniciIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            Sonuc sonuc = TNS.KYM.Arac.Tanimla().KullaniciyaYeniParolaAta(Tc(istek.tcKimlikNo));
            if (!sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Yeni parola atanamadı."));
            return Results.Json(new { basarili = true, bilgi = (sonuc.bilgiStr ?? "").Trim() == string.Empty ? "İşlem Başarıyla Gerçekleştirildi." : PersonelBilgiOrtak.Duz(sonuc.bilgiStr) });
        }

        // ---------------------------------------------------------------- roller / görevler

        private static async Task<IResult> RolSil(HttpContext baglam, IWebHostEnvironment ortam, PersonelKullaniciIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string tc = Tc(istek.tcKimlikNo);
            if (istek.satirlar == null || istek.satirlar.Count == 0 || tc == string.Empty) return Hata("Lütfen kaldırılacak rolleri seçiniz.");

            List<KisiGorev> kisigorevleri = new List<KisiGorev>();
            foreach (PersonelKullaniciRolSatiri item in istek.satirlar)
            {
                KisiGorev kg = new KisiGorev();
                kg.orgKod = item.BIRIMKOD;
                kg.gorevOrgKod = item.ROLKOD;
                kg.gorevSiraNo = item.ROLSIRANO;
                kg.tcKimlikNo = tc;
                kisigorevleri.Add(kg);
            }
            Sonuc sonuc = TNS.ORG.Arac.Tanimla().KisiGenelGorevleriSil(kullanan, kisigorevleri.ToArray());
            if (!sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Roller kaldırılamadı."));
            return Results.Json(new { basarili = true, bilgi = PersonelBilgiOrtak.Duz(sonuc.bilgiStr), roller = Roller(kullanan, tc) });
        }

        private static async Task<IResult> GorevEkle(HttpContext baglam, IWebHostEnvironment ortam, PersonelKullaniciIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string gorevKod = istek.gorevKod ?? "";
            if (string.IsNullOrWhiteSpace(gorevKod)) return Hata("Lütfen görev seçiniz!");
            string secBirimler = istek.birimler ?? "";
            if (string.IsNullOrWhiteSpace(secBirimler)) return Hata("Lütfen birim seçiniz!");

            string tc = Tc(istek.tcKimlikNo);
            IORGServis servisORG = TNS.ORG.Arac.Tanimla();
            string[] birimler = secBirimler.Split(',');
            TNS.ORG.Personel[] personeller = new TNS.ORG.Personel[1];
            string hata = "";
            int eklenen = 0;
            foreach (string birimStr in birimler)
            {
                OrganizasyonBirim organizasyonBirim = new OrganizasyonBirim();
                organizasyonBirim.kod = OrtakFonksiyonlar.ConvertToInt(birimStr.Trim(), 0);
                personeller[0] = new TNS.ORG.Personel();
                personeller[0].tcKimlikNo = tc;
                Sonuc s = servisORG.GenelGorevKisileriEkle(kullanan, organizasyonBirim, BirimGenelGorev.GorevKodBicimindenGenelGoreve(gorevKod), personeller);
                if (!s.islemSonuc)
                {
                    if (hata != "") hata += ", ";
                    hata += PersonelBilgiOrtak.Duz(s.hataStr);
                }
                else
                    eklenen++;
            }
            // Eski: hata varsa gösterilir; en az biri eklendiyse sayfa (KullaniciKontrol) yenilenir.
            return Results.Json(new { basarili = hata == "", hata = hata, eklenen = eklenen });
        }
    }
}
