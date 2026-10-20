using System;
using System.Collections.Generic;
using System.Data;
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
using TNS.BKD;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>Bir banka listesinin anahtarı (eski hdnSelSay, hdnSelBir, hdnGun, hdnSiraNo). gun: yyyy-MM-dd.</summary>
    public class BankaListesiAnahtar
    {
        public int saymanlik { get; set; }
        public string birim { get; set; }
        public string gun { get; set; }
        public int siraNo { get; set; }
    }

    /// <summary>Banka listesindeki (ya da eklenecek ödeme emirleri listesindeki) bir satır (eski stoOE / stoOdenecekOE alanları).</summary>
    public class BankaListesiSatir
    {
        public int sira { get; set; }
        public int mifYil { get; set; }
        public string birimKod { get; set; }
        public string mifBelgeNo { get; set; }
        public int mifSira { get; set; }
        public string muhHesapKodu { get; set; }
        public double tutar { get; set; }
        public string doviz { get; set; }
        public string ilgili { get; set; }
        public string tc_Vno { get; set; }
        public string ilgBanka { get; set; }
        public string ilgIBAN { get; set; }
        public string aciklama { get; set; }
        public string dekontNo { get; set; }
        public string odemeEmriNo { get; set; }
        public int mifYevmiyeNo { get; set; }
        public string cekNo { get; set; }
    }

    /// <summary>Yeni banka listesi (eski wndYeni: btnYeni_Click).</summary>
    public class BankaListesiEkleIstegi
    {
        public int muhasebe { get; set; }
        public string birim { get; set; }
        public string odemeTarih { get; set; }
        public string aciklama { get; set; }
    }

    /// <summary>Başlık bilgileri (eski btnGuncelle_Click).</summary>
    public class BankaListesiBaslikIstegi : BankaListesiAnahtar
    {
        public string aciklama { get; set; }
        public string muhYetkili { get; set; }
        public string muhYetUnvan { get; set; }
        public string hazirlayan { get; set; }
        public string onaylayan { get; set; }
        public string onayUnvan { get; set; }
        public string odemeTarih { get; set; }
    }

    /// <summary>Seçili satırlarla yapılan işlemler (ödeme emri ekle, listeden çıkart, toplu kapat, çek no ata).</summary>
    public class BankaListesiSatirIstegi : BankaListesiAnahtar
    {
        public List<BankaListesiSatir> satirlar { get; set; }
        /// <summary>Toplu kapatmada kapatma tarihi (yyyy-MM-dd).</summary>
        public string tarih { get; set; }
        /// <summary>Çek no atamada elle verilen numara (boşsa otomatik).</summary>
        public string cekNo { get; set; }
    }

    /// <summary>Onayla / onay kaldır (eski btnOnayla_Click, onayKod 5 / 1).</summary>
    public class BankaListesiOnayIstegi : BankaListesiAnahtar
    {
        public int onayKod { get; set; }
        public int donemNo { get; set; }
    }

    /// <summary>Satır ödendi (eski wndOde: btnOde_Click).</summary>
    public class BankaListesiOdeIstegi : BankaListesiAnahtar
    {
        public int sira { get; set; }
        public string dekontTarih { get; set; }
        public string dekontNo { get; set; }
    }

    /// <summary>Çek numarasından ödeme emri getir (eski btnCekNoOEGetir_Click).</summary>
    public class BankaListesiCekIstegi : BankaListesiAnahtar
    {
        public string cekEmriNo { get; set; }
    }

    /// <summary>
    /// Banka Listesi İşlemleri (eski ButceMuhasebe/BankaListesiAna.aspx) uçları. Eski sayfanın arka plan kodunun işleri burada yapılır,
    /// iş kuralları servisleri (IMUHServis.BankaListesi*) değişmeden çağrılır.
    ///   GET  /api/bankalistesi/tanim                       açılış: kullanıcının muhasebe birimi, tarihler, yönetici / çekli mi
    ///   GET  /api/bankalistesi/liste?muhasebe&amp;birim&amp;kurumsal&amp;basla&amp;bitis&amp;ara   eski btnListele_Click (BankaListesiListele)
    ///   POST /api/bankalistesi/ekle                        eski btnYeni_Click (BankaListesiEkle)
    ///   POST /api/bankalistesi/guncelle                    eski btnGuncelle_Click (BankaListesiGuncelle)
    ///   POST /api/bankalistesi/sil                         eski btnSilKayit_Click (BankaListesiSil)
    ///   GET  /api/bankalistesi/detay?anahtar               eski stoOE_RefreshData (BankaListesiOku)
    ///   GET  /api/bankalistesi/odenecek?yil&amp;saymanlik&amp;birim&amp;basla&amp;bitis   eski grdOdenecekOEGuncelle (BankaListesiOdenecekEmirListele)
    ///   POST /api/bankalistesi/odenecek-ekle               eski OdenecekEkle (BankaListesineOEEkle, satır satır)
    ///   POST /api/bankalistesi/listeden-cikar              eski OEListedenCikart (BankaListesindenOESil)
    ///   POST /api/bankalistesi/onayla                      eski btnOnayla_Click (BankaListesiOnayla)
    ///   POST /api/bankalistesi/ode                         eski btnOde_Click (BankaListesiDetayOdendi)
    ///   POST /api/bankalistesi/toplu-kapat                 eski btnSeciliKapat_Click (BankaListesiDetayOdendiToplu)
    ///   POST /api/bankalistesi/cek-no                      eski BasilanCekNoAta (BankaListesiBasilanCekNoAta)
    ///   POST /api/bankalistesi/cek-oe                      eski btnCekNoOEGetir_Click (BankaListesiCekNoOEEkle)
    ///   GET  /api/bankalistesi/yazdir?anahtar&amp;tur&amp;grup&amp;pdf   eski Yazdır menüsü (Word: MUH/BankaListesi*.dotx, Excel: MUH/BankaListesi*.xlt*)
    ///   POST /api/bankalistesi/ebys                        eski btnEvrakaGonder_Click
    ///   POST /api/bankalistesi/bankaya-gonder              eski btnYazdirBanka_Click (banka dosyası, FTP)
    ///   POST /api/bankalistesi/bankadan-al                 eski btnOkuBanka_Click (BankaAkibetDosyasiIsle)
    ///   GET  /api/bankalistesi/dosya?ad                    eski DosyaIndir (bankaya gönderilen dosya)
    /// Yetki: menüdeki sayfa adresi.
    /// </summary>
    public static class BankaListesiUclari
    {
        public const string SayfaAdres = "../ButceMuhasebe/BankaListesiAna.aspx";
        private const string Uc = "/api/bankalistesi/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "liste", Liste);
            app.MapPost(Uc + "ekle", YeniEkle);
            app.MapPost(Uc + "guncelle", Guncelle);
            app.MapPost(Uc + "sil", Sil);
            app.MapGet(Uc + "detay", Detay);
            app.MapGet(Uc + "odenecek", Odenecek);
            app.MapPost(Uc + "odenecek-ekle", OdenecekEkle);
            app.MapPost(Uc + "listeden-cikar", ListedenCikar);
            app.MapPost(Uc + "onayla", Onayla);
            app.MapPost(Uc + "ode", Ode);
            app.MapPost(Uc + "toplu-kapat", TopluKapat);
            app.MapPost(Uc + "cek-no", CekNo);
            app.MapPost(Uc + "cek-oe", CekOE);
            app.MapGet(Uc + "yazdir", Yazdir);
            app.MapPost(Uc + "ebys", Ebys);
            app.MapPost(Uc + "bankaya-gonder", BankayaGonder);
            app.MapPost(Uc + "bankadan-al", BankadanAl);
            app.MapGet(Uc + "dosya", DosyaIndir);
        }

        public static bool SayfaYetkili(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdres);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!SayfaYetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static async Task<IResult> DenetlePost(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, Action<Kullanici> kullaniciAta)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            kullaniciAta(kullanan);
            return null;
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        private static IResult Hata(string hata) { return Results.Json(new { hata = Duz(hata) }); }

        private static IResult Bilgi(string bilgi) { return Results.Json(new { basarili = true, bilgi = Duz(bilgi) }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string Duz(string s)
        {
            return (s ?? "").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n").Trim();
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

        private static TNSDateTime Tarih(string iso) { return OdemeEmriMIFUclari.Tarih(iso); }

        private static string TarihYaz(TNSDateTime t) { return OdemeEmriMIFUclari.TarihYaz(t); }

        /// <summary>Eski DurumToStr.</summary>
        private static string DurumToStr(ENUMBankaListesiDurum durum)
        {
            if (durum == ENUMBankaListesiDurum.Degismis)
                return "Değiştirildi";
            else if (durum == ENUMBankaListesiDurum.Iptal)
                return "İptal";
            else if (durum == ENUMBankaListesiDurum.Onayli)
                return "Onaylı";
            else if (durum == ENUMBankaListesiDurum.Yeni)
                return "Yeni";
            else
                return "Tanımsız";
        }

        /// <summary>Eski SaymanlikBirimYaz2 + gun + siraNo.</summary>
        private static MUHBankaOdeme BilgiOl(BankaListesiAnahtar a)
        {
            MUHBankaOdeme bilgi = new MUHBankaOdeme();
            bilgi.saymanlik = a?.saymanlik ?? 0;
            bilgi.birim = Temiz(a?.birim);
            bilgi.gun = Tarih(a?.gun);
            bilgi.siraNo = a?.siraNo ?? 0;
            return bilgi;
        }

        private static BankaListesiAnahtar AnahtarOku(HttpContext baglam)
        {
            return new BankaListesiAnahtar
            {
                saymanlik = OrtakFonksiyonlar.ConvertToInt(Q(baglam, "saymanlik"), 0),
                birim = Q(baglam, "birim"),
                gun = Q(baglam, "gun"),
                siraNo = OrtakFonksiyonlar.ConvertToInt(Q(baglam, "siraNo"), 0)
            };
        }

        private static bool AnahtarEksik(BankaListesiAnahtar a)
        {
            return a == null || Temiz(a.gun) == "" || a.siraNo <= 0;
        }

        private const string ListeSecilmedi = "Soldaki listeden bir banka listesi seçiniz.";

        /// <summary>Sayfanın gönderdiği satır (eski JSON.Deserialize&lt;MUHBankaOdemeDetay&gt;(rec)).</summary>
        private static MUHBankaOdemeDetay DetayOl(BankaListesiSatir s)
        {
            MUHBankaOdemeDetay d = new MUHBankaOdemeDetay();
            if (s == null) return d;
            d.sira = s.sira;
            d.mifYil = s.mifYil;
            d.birimKod = s.birimKod;
            d.mifBelgeNo = s.mifBelgeNo;
            d.mifSira = s.mifSira;
            d.muhHesapKodu = s.muhHesapKodu;
            d.tutar = s.tutar;
            d.doviz = s.doviz;
            d.ilgili = s.ilgili;
            d.tc_Vno = s.tc_Vno;
            d.ilgBanka = s.ilgBanka;
            d.ilgIBAN = s.ilgIBAN;
            d.aciklama = s.aciklama;
            d.dekontNo = s.dekontNo;
            d.odemeEmriNo = s.odemeEmriNo;
            d.mifYevmiyeNo = s.mifYevmiyeNo;
            d.cekNo = s.cekNo;
            return d;
        }

        private static object SatirYaz(MUHBankaOdemeDetay d)
        {
            return new
            {
                sira = d.sira,
                mifYil = d.mifYil,
                birimKod = d.birimKod ?? "",
                mifBelgeNo = d.mifBelgeNo ?? "",
                mifSira = d.mifSira,
                muhHesapKodu = d.muhHesapKodu ?? "",
                tutar = d.tutar,
                doviz = d.doviz ?? "",
                ilgili = d.ilgili ?? "",
                tc_Vno = d.tc_Vno ?? "",
                ilgBanka = d.ilgBanka ?? "",
                ilgIBAN = d.ilgIBAN ?? "",
                aciklama = d.aciklama ?? "",
                dekontNo = d.dekontNo ?? "",
                odemeEmriNo = d.odemeEmriNo ?? "",
                mifYevmiyeNo = d.mifYevmiyeNo,
                cekNo = d.cekNo ?? ""
            };
        }

        // ------------------------------------------------------------------ açılış (eski Page_Load)

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int muhasebe = OrtakFonksiyonlar.ConvertToInt(GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBE"), 0);
            string birim = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEBIRIM") ?? "";
            string kurumsal = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEKURUMSAL") ?? "";
            string birimAd = "";
            if (muhasebe > 0 && birim != "")
            {
                try { birimAd = MuhasebeBirimUclari.BirimAdBul(muhasebe, birim, out _) ?? ""; } catch (Exception) { birimAd = ""; }
            }
            bool sy = kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI, (int)ENUMKullaniciTipi.MUHASEBEYONETICISI);
            bool cekli = TNS.MUH.Arac.DegiskenDegerBul(0, "/CEKLI") == "1";
            DateTime bugun = DateTime.Now.Date;
            return Results.Json(new
            {
                muhasebe = muhasebe,
                birim = birim,
                kurumsal = kurumsal,
                birimAd = birimAd,
                sy = sy,
                cekli = cekli,
                basla = bugun.AddDays(-1).ToString("yyyy-MM-dd"),
                bitis = bugun.ToString("yyyy-MM-dd"),
                bugun = bugun.ToString("yyyy-MM-dd"),
                yevBasla = bugun.AddDays(-7).ToString("yyyy-MM-dd")
            });
        }

        // ------------------------------------------------------------------ listele (eski btnListele_Click)

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int muhasebe = OrtakFonksiyonlar.ConvertToInt(Q(baglam, "muhasebe"), 0);
                string birim = Q(baglam, "birim");
                // Eski DegiskenleriKaydet: seçilen muhasebe birimi kullanıcıya saklanır.
                try
                {
                    GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBE", muhasebe > 0 ? muhasebe.ToString() : "");
                    GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBEBIRIM", birim);
                    GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBEKURUMSAL", Q(baglam, "kurumsal"));
                }
                catch (Exception) { }

                ObjectArray oa = Servis().BankaListesiListele(kullanan, muhasebe, birim, Tarih(Q(baglam, "basla")), Tarih(Q(baglam, "bitis")), Q(baglam, "ara"));
                if (!oa.sonuc.islemSonuc) return Hata(HataMetni(oa.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (MUHBankaOdeme item in oa.objeler)
                {
                    satirlar.Add(new
                    {
                        saymanlik = item.saymanlik,
                        birim = item.birim ?? "",
                        gun = TarihYaz(item.gun),
                        siraNo = item.siraNo,
                        aciklama = item.aciklama ?? "",
                        kullaniciKod = item.kullaniciKod ?? "",
                        hazirlayan = item.hazirlayan ?? "",
                        odemeTarih = TarihYaz(item.odemeTarih),
                        muhYetkili = item.muhYetkili ?? "",
                        muhYetUnvan = item.muhYetUnvan ?? "",
                        onaylayan = item.onaylayan ?? "",
                        onayUnvan = item.onayUnvan ?? "",
                        cekEmriNo = item.cekEmriNo ?? "",
                        donemNo = item.donemNo,
                        tur = (int)item.tur,
                        onayli = item.durum == ENUMBankaListesiDurum.Onayli,
                        durum = DurumToStr(item.durum),
                        dosyaAd = item.bankaDosyaAd ?? ""
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Liste okunamadı."));
            }
        }

        // ------------------------------------------------------------------ yeni liste (eski btnYeni_Click)

        private static async Task<IResult> YeniEkle(HttpContext baglam, IWebHostEnvironment ortam, BankaListesiEkleIstegi istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, k => kullanan = k);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            try
            {
                TNSDateTime dt = Tarih(istek.odemeTarih);
                if (dt.isNull) return Hata("Ödeme tarihini giriniz.");
                string k = kullanan.kullaniciKodu;
                string muhYetkili = GenelIslemler.KullaniciDegiskenGetir(k, "MuhYetkili");
                string muhYetUnvan = GenelIslemler.KullaniciDegiskenGetir(k, "MuhYetUnvan");
                string onaylayan = GenelIslemler.KullaniciDegiskenGetir(k, "Onaylayan");
                string onayUnvan = GenelIslemler.KullaniciDegiskenGetir(k, "OnayUnvan");
                string aciklama = Temiz(istek.aciklama);
                Sonuc s = Servis().BankaListesiEkle(kullanan, istek.muhasebe, Temiz(istek.birim), dt, aciklama, muhYetkili, muhYetUnvan, onaylayan, onayUnvan);
                if (!s.islemSonuc) return Hata(HataMetni(s.hataStr, "Liste eklenemedi."));
                // Eski SonaEkle: yeni satır soldaki listenin sonuna eklenir.
                return Results.Json(new
                {
                    basarili = true,
                    satir = new
                    {
                        saymanlik = istek.muhasebe,
                        birim = Temiz(istek.birim),
                        gun = TarihYaz(dt),
                        siraNo = OrtakFonksiyonlar.ConvertToInt(s.anahtar, 0),
                        aciklama = aciklama,
                        kullaniciKod = k,
                        hazirlayan = kullanan.ad + " " + kullanan.soyad,
                        odemeTarih = TarihYaz(dt),
                        muhYetkili = muhYetkili,
                        muhYetUnvan = muhYetUnvan,
                        onaylayan = onaylayan,
                        onayUnvan = onayUnvan,
                        cekEmriNo = "",
                        donemNo = 0,
                        tur = (int)ENUMBankaListesiTur.OESecim,
                        onayli = false,
                        durum = DurumToStr(ENUMBankaListesiDurum.Yeni),
                        dosyaAd = ""
                    }
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Liste eklenemedi."));
            }
        }

        // ------------------------------------------------------------------ başlık bilgileri (eski btnGuncelle_Click)

        private static async Task<IResult> Guncelle(HttpContext baglam, IWebHostEnvironment ortam, BankaListesiBaslikIstegi istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, k => kullanan = k);
            if (ret != null) return ret;
            if (AnahtarEksik(istek)) return Hata(ListeSecilmedi);
            try
            {
                MUHBankaOdeme bilgi = BilgiOl(istek);
                bilgi.aciklama = istek.aciklama ?? "";
                bilgi.muhYetkili = istek.muhYetkili ?? "";
                bilgi.muhYetUnvan = istek.muhYetUnvan ?? "";
                bilgi.hazirlayan = istek.hazirlayan ?? "";
                bilgi.onaylayan = istek.onaylayan ?? "";
                bilgi.onayUnvan = istek.onayUnvan ?? "";
                TNSDateTime odemeTarih = Tarih(istek.odemeTarih);
                bilgi.odemeTarih = odemeTarih.isNull ? null : odemeTarih;

                if (!string.IsNullOrWhiteSpace(bilgi.muhYetkili))
                    GenelIslemler.KullaniciDegiskenSakla(kullanan, "MuhYetkili", bilgi.muhYetkili);
                if (!string.IsNullOrWhiteSpace(bilgi.muhYetUnvan))
                    GenelIslemler.KullaniciDegiskenSakla(kullanan, "MuhYetUnvan", bilgi.muhYetUnvan);
                if (!string.IsNullOrWhiteSpace(bilgi.onaylayan))
                    GenelIslemler.KullaniciDegiskenSakla(kullanan, "Onaylayan", bilgi.onaylayan);
                if (!string.IsNullOrWhiteSpace(bilgi.onayUnvan))
                    GenelIslemler.KullaniciDegiskenSakla(kullanan, "OnayUnvan", bilgi.onayUnvan);

                Sonuc s = Servis().BankaListesiGuncelle(kullanan, bilgi);
                if (!s.islemSonuc) return Hata("Bilgiler değiştirilemedi! " + HataMetni(s.hataStr, ""));
                return Bilgi("Bilgiler güncellendi!");
            }
            catch (Exception ex)
            {
                return Hata("Bilgiler değiştirilemedi! " + HataMetni(ex.Message, ""));
            }
        }

        // ------------------------------------------------------------------ sil (eski btnSilKayit_Click)

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, BankaListesiAnahtar istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, k => kullanan = k);
            if (ret != null) return ret;
            if (AnahtarEksik(istek)) return Hata(ListeSecilmedi);
            try
            {
                Sonuc s = Servis().BankaListesiSil(kullanan, BilgiOl(istek));
                if (!s.islemSonuc) return Hata("Kayıt silinemedi! " + HataMetni(s.hataStr, ""));
                return Bilgi("Kayıt silindi.");
            }
            catch (Exception ex)
            {
                return Hata("Kayıt silinemedi! " + HataMetni(ex.Message, ""));
            }
        }

        // ------------------------------------------------------------------ listedeki ödemeler (eski stoOE_RefreshData)

        private static IResult Detay(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            BankaListesiAnahtar a = AnahtarOku(baglam);
            if (AnahtarEksik(a)) return Hata(ListeSecilmedi);
            try
            {
                MUHBankaOdeme mbO = Servis().BankaListesiOku(kullanan, BilgiOl(a));
                if (mbO == null) return Hata("Liste okunamadı.");
                List<object> satirlar = new List<object>();
                double toplam = 0;
                if (mbO.muhBankaOdemeDetay != null)
                    foreach (MUHBankaOdemeDetay d in mbO.muhBankaOdemeDetay)
                    {
                        satirlar.Add(SatirYaz(d));
                        toplam += d.tutar;
                    }
                return Results.Json(new { satirlar = satirlar, toplam = toplam });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Liste okunamadı."));
            }
        }

        // ------------------------------------------------------------------ eklenebilecek ödeme emirleri (eski grdOdenecekOEGuncelle)

        private static IResult Odenecek(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray oa = Servis().BankaListesiOdenecekEmirListele(kullanan, OrtakFonksiyonlar.ConvertToInt(Q(baglam, "yil"), 0),
                    OrtakFonksiyonlar.ConvertToInt(Q(baglam, "saymanlik"), 0), Q(baglam, "birim"), Tarih(Q(baglam, "basla")), Tarih(Q(baglam, "bitis")));
                if (oa.sonuc != null && !oa.sonuc.islemSonuc) return Hata(HataMetni(oa.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                for (int i = 0; i < oa.ObjeSayisi; i++)
                    satirlar.Add(SatirYaz((MUHBankaOdemeDetay)oa.objeler[i]));
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Liste okunamadı."));
            }
        }

        /// <summary>Eski OdenecekEkle / TekSatirEkle: satırlar tek tek eklenir; eklenemeyenlerin hatası döner.</summary>
        private static async Task<IResult> OdenecekEkle(HttpContext baglam, IWebHostEnvironment ortam, BankaListesiSatirIstegi istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, k => kullanan = k);
            if (ret != null) return ret;
            if (AnahtarEksik(istek)) return Hata(ListeSecilmedi);
            if (istek.satirlar == null || istek.satirlar.Count == 0) return Hata("Eklenecek ödeme emri satırı seçilmedi.");
            IMUHServis servis = Servis();
            MUHBankaOdeme bilgi = BilgiOl(istek);
            List<object> eklenen = new List<object>();
            List<string> hatalar = new List<string>();
            foreach (BankaListesiSatir satir in istek.satirlar)
            {
                try
                {
                    Sonuc s = servis.BankaListesineOEEkle(kullanan, bilgi, DetayOl(satir));
                    if (s.islemSonuc)
                        eklenen.Add(new { satir.mifYil, satir.mifBelgeNo, satir.mifSira });
                    else
                        hatalar.Add(satir.mifBelgeNo + "/" + satir.mifSira + ": " + HataMetni(s.hataStr, "eklenemedi"));
                }
                catch (Exception ex)
                {
                    hatalar.Add(satir.mifBelgeNo + "/" + satir.mifSira + ": " + HataMetni(ex.Message, "eklenemedi"));
                }
            }
            return Results.Json(new { basarili = hatalar.Count == 0, eklenen = eklenen, hata = hatalar.Count > 0 ? string.Join("\n", hatalar) : null });
        }

        // ------------------------------------------------------------------ listeden çıkart (eski OEListedenCikart)

        private static async Task<IResult> ListedenCikar(HttpContext baglam, IWebHostEnvironment ortam, BankaListesiSatirIstegi istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, k => kullanan = k);
            if (ret != null) return ret;
            if (AnahtarEksik(istek)) return Hata(ListeSecilmedi);
            if (istek.satirlar == null || istek.satirlar.Count == 0) return Hata("Listeden çıkartılacak satırları seçiniz.");
            try
            {
                MUHBankaOdeme bilgi = BilgiOl(istek);
                bilgi.muhBankaOdemeDetay = istek.satirlar.ConvertAll(DetayOl).ToArray();
                MUHBankaOdemeDetay d = null;
                Sonuc s = Servis().BankaListesindenOESil(kullanan, bilgi, d);
                if (!s.islemSonuc) return Hata(HataMetni(s.hataStr, "Satırlar listeden çıkartılamadı."));
                return Bilgi("Seçilen satırlar listeden çıkartıldı.");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Satırlar listeden çıkartılamadı."));
            }
        }

        // ------------------------------------------------------------------ onayla / onay kaldır (eski btnOnayla_Click)

        private static async Task<IResult> Onayla(HttpContext baglam, IWebHostEnvironment ortam, BankaListesiOnayIstegi istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, k => kullanan = k);
            if (ret != null) return ret;
            if (AnahtarEksik(istek)) return Hata(ListeSecilmedi);
            try
            {
                ENUMBankaListesiDurum onayKod = istek.onayKod == 1 ? ENUMBankaListesiDurum.Degismis : ENUMBankaListesiDurum.Onayli;
                Sonuc s = Servis().BankaListesiOnayla(kullanan, istek.saymanlik, Temiz(istek.birim), Tarih(istek.gun), istek.siraNo, onayKod, istek.donemNo);
                if (!s.islemSonuc) return Hata(HataMetni(s.hataStr, "İşlem yapılamadı."));
                return Results.Json(new
                {
                    basarili = true,
                    bilgi = onayKod == ENUMBankaListesiDurum.Onayli ? "Form Onaylandı!" : "Onay kaldırıldı.",
                    onayli = onayKod == ENUMBankaListesiDurum.Onayli,
                    durum = DurumToStr(onayKod),
                    donemNo = OrtakFonksiyonlar.ConvertToInt(s.anahtar, istek.donemNo)
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İşlem yapılamadı."));
            }
        }

        // ------------------------------------------------------------------ ödendi (eski btnOde_Click)

        private static async Task<IResult> Ode(HttpContext baglam, IWebHostEnvironment ortam, BankaListesiOdeIstegi istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, k => kullanan = k);
            if (ret != null) return ret;
            if (AnahtarEksik(istek)) return Hata(ListeSecilmedi);
            try
            {
                // Eski kod dekont tarihi ve numarasını "gg.aa.yyyy/no" biçiminde birleştirirdi.
                TNSDateTime dekontTarih = Tarih(istek.dekontTarih);
                string tarihStr = dekontTarih.isNull ? "" : dekontTarih.Oku().ToString("dd.MM.yyyy");
                string no = Temiz(istek.dekontNo);
                string dekontTarihveNo;
                if (tarihStr != "" && no != "") dekontTarihveNo = tarihStr + "/" + no;
                else if (tarihStr != "") dekontTarihveNo = tarihStr;
                else if (no != "") dekontTarihveNo = "/" + no;
                else dekontTarihveNo = "";

                Sonuc s = Servis().BankaListesiDetayOdendi(kullanan, istek.saymanlik, Temiz(istek.birim), Tarih(istek.gun), istek.siraNo, istek.sira, dekontTarihveNo);
                if (!s.islemSonuc) return Hata(HataMetni(s.hataStr, "İşlem yapılamadı."));
                return Results.Json(new { basarili = true, odemeEmriNo = s.anahtar ?? "", dekontNo = dekontTarihveNo });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İşlem yapılamadı."));
            }
        }

        // ------------------------------------------------------------------ toplu kapat (eski btnSeciliKapat_Click)

        private static async Task<IResult> TopluKapat(HttpContext baglam, IWebHostEnvironment ortam, BankaListesiSatirIstegi istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, k => kullanan = k);
            if (ret != null) return ret;
            if (AnahtarEksik(istek)) return Hata(ListeSecilmedi);
            if (istek.satirlar == null || istek.satirlar.Count == 0) return Hata("Kapatılacak satırları seçiniz.");
            try
            {
                MUHBankaOdeme bilgi = BilgiOl(istek);
                bilgi.kapatmaTarihi = Tarih(istek.tarih);
                Sonuc s = Servis().BankaListesiDetayOdendiToplu(kullanan, bilgi, istek.satirlar.ConvertAll(DetayOl).ToArray());
                if (!s.islemSonuc) return Hata(HataMetni(s.hataStr, "İşlem yapılamadı."));
                return Bilgi("Seçili kayıtlar kapatıldı.");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İşlem yapılamadı."));
            }
        }

        // ------------------------------------------------------------------ basılan çek no (eski BasilanCekNoAta)

        private static async Task<IResult> CekNo(HttpContext baglam, IWebHostEnvironment ortam, BankaListesiSatirIstegi istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, k => kullanan = k);
            if (ret != null) return ret;
            if (AnahtarEksik(istek)) return Hata(ListeSecilmedi);
            if (istek.satirlar == null || istek.satirlar.Count == 0) return Hata("Satır seçilmedi.");
            try
            {
                MUHBankaOdemeDetay d = DetayOl(istek.satirlar[0]);
                d.cekNo = istek.cekNo ?? "";
                Sonuc s = Servis().BankaListesiBasilanCekNoAta(kullanan, BilgiOl(istek), d);
                if (!s.islemSonuc) return Hata(HataMetni(s.hataStr, "Çek no atanamadı!"));
                if (string.IsNullOrEmpty(s.anahtar)) return Hata("Çek no atanamadı!");
                return Results.Json(new { basarili = true, cekNo = s.anahtar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Çek no atanamadı!"));
            }
        }

        // ------------------------------------------------------------------ çek numarasından ödeme emri (eski btnCekNoOEGetir_Click)

        private static async Task<IResult> CekOE(HttpContext baglam, IWebHostEnvironment ortam, BankaListesiCekIstegi istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, k => kullanan = k);
            if (ret != null) return ret;
            if (AnahtarEksik(istek)) return Hata(ListeSecilmedi);
            try
            {
                string cekEmriNo = Temiz(istek.cekEmriNo);
                Sonuc s = Servis().BankaListesiCekNoOEEkle(kullanan, BilgiOl(istek), cekEmriNo);
                if (!s.islemSonuc) return Hata(HataMetni(s.hataStr, "İşlem yapılamadı."));
                return Results.Json(new { basarili = true, cekEmriNo = cekEmriNo });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İşlem yapılamadı."));
            }
        }

        // ------------------------------------------------------------------ yazdır (eski Yazdır menüsü)

        /// <summary>
        /// tur: word (eski btnYazdirWord_Click), wordtek (Word Tek Ödeme), wordozet (Word Tek Özet), xls (Liste; grup=1 Liste Grup, pdf=1 pdf),
        /// detay (Liste Detaylı).
        /// </summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            BankaListesiAnahtar a = AnahtarOku(baglam);
            if (AnahtarEksik(a)) return Hata(ListeSecilmedi);
            string tur = Q(baglam, "tur");
            string grup = Q(baglam, "grup") == "1" ? "1" : null;
            bool pdf = Q(baglam, "pdf") == "1";
            try
            {
                AsposeLisans.Yukle();
                if (tur == "xls") return YazdirExcel(kullanan, a, grup, pdf);
                if (tur == "detay") return YazdirDetay(kullanan, a, grup);
                bool tekBelge = tur == "wordtek" || tur == "wordozet";
                bool ozet = tur == "wordozet";
                string sonucDosyaAd = DosyaUret(kullanan, a, tekBelge, ozet, false, out string hata);
                if (sonucDosyaAd == null) return Hata(hata);
                return DosyaIslem.DosyaGonder(sonucDosyaAd, Path.GetFileName(sonucDosyaAd), true, "docx");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        /// <summary>
        /// Eski DosyaIslem.SablonAdOl: imzalı istenirse önce "eimza" önekli şablon; önce kurumun RaporSablonYol/MUH klasörü,
        /// yoksa uygulamanın RaporSablon/MUH klasörü.
        /// </summary>
        private static string SablonYol(string sablonAd, bool imza = false)
        {
            if (imza)
            {
                string imzali = SablonBul("eimza" + sablonAd);
                if (File.Exists(imzali)) return imzali;
            }
            return SablonBul(sablonAd);
        }

        private static string SablonBul(string sablonAd)
        {
            string kurum = Path.Combine(Ayar.Oku("RaporSablonYol") ?? "", "MUH", sablonAd);
            if (File.Exists(kurum)) return kurum;
            // Geliştirmede şablonlar çıktı klasörüne kopyalanmaz (yalnız yayında); önce uygulama klasörüne (SGB.Net), sonra exe klasörüne bakılır.
            foreach (string kok in new string[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
            {
                string uygulama = Path.Combine(kok, "RaporSablon", "MUH", sablonAd);
                if (File.Exists(uygulama)) return uygulama;
            }
            return kurum;
        }

        /// <summary>Eski ButceMuhasebeGenel.DondurParaBirimi.</summary>
        private static ParaBirimi DondurParaBirimi(int paraBirimiKod)
        {
            TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
            ObjectArray paralar = servisHRC.ParaBirimiListele(new Kullanici(), new ParaBirimi());
            foreach (ParaBirimi bilgi in paralar.objeler)
            {
                if (bilgi.kod == paraBirimiKod)
                    return bilgi;
            }
            return new ParaBirimi();
        }

        /// <summary>Eski DosyaUret: Word banka listesi (BankaListesi.dotx; Ulusal Ajans'ta BankaListesiUA.dotx; tek belgede BankaListesiTek.dotx).</summary>
        private static string DosyaUret(Kullanici kullanan, BankaListesiAnahtar a, bool tekBelge, bool ozet, bool imza, out string hata)
        {
            hata = null;
            IMUHServis servisMUH = Servis();
            TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
            string kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = GenelIslemlerIstemci.VarsayilanKurumBulAd();

            string sablon = "BankaListesi.dotx";
            bool bosluklariAt = true;
            bool guvenli = false;
            if (TNS.HRC.OrtakServis.UlusalAjansMi())
            {
                sablon = "BankaListesiUA.dotx";
                guvenli = true;
            }
            if (tekBelge)
            {
                sablon = "BankaListesiTek.dotx";
                bosluklariAt = false;
            }
            string sablonYol = SablonYol(sablon, imza);
            if (!File.Exists(sablonYol))
            {
                hata = "Banka listesi şablonu (MUH/" + sablon + ") bulunamadı.";
                return null;
            }

            MUHBankaOdeme mbO = servisMUH.BankaListesiOku(kullanan, BilgiOl(a));
            if (mbO == null || mbO.muhBankaOdemeDetay == null || mbO.muhBankaOdemeDetay.Length == 0)
            {
                hata = "Listede ödeme yok.";
                return null;
            }
            double toplamTutar = 0;

            DataTable dtListe = new DataTable();
            dtListe.TableName = "Liste";
            dtListe.Columns.Add("SiraNo");
            dtListe.Columns.Add("BelgeNo");
            dtListe.Columns.Add("KurumIBAN");
            dtListe.Columns.Add("HesapSahibi");
            dtListe.Columns.Add("VergiNo");
            dtListe.Columns.Add("BankaAdi");
            dtListe.Columns.Add("IBAN");
            dtListe.Columns.Add("Tutar");
            dtListe.Columns.Add("TutarBirim");
            dtListe.Columns.Add("Aciklama");
            dtListe.Columns.Add("Yaziyla");
            dtListe.Columns.Add("FaturaNo");
            dtListe.Columns.Add("BankaHesapNo");
            dtListe.Columns.Add("tg");
            dtListe.Columns.Add("ta");
            dtListe.Columns.Add("ty");

            string bankaAd = "";
            string subeAd = "";
            OdemeEmriMIFForm oeForm = null;
            TNSDateTime odemeTarih = mbO.odemeTarih ?? new TNSDateTime();

            for (int i = 0; i < mbO.muhBankaOdemeDetay.Length; i++)
            {
                MUHBankaOdemeDetay d = mbO.muhBankaOdemeDetay[i];
                toplamTutar += d.tutar;
                if (d.ilgIBAN == "0") d.ilgIBAN = "";
                string faturaNo = "";
                string bankaHesapNo = "";

                if (oeForm == null || oeForm.yil != d.mifYil || oeForm.belgeNo != d.mifBelgeNo)
                {
                    oeForm = new OdemeEmriMIFForm();
                    oeForm.yil = d.mifYil;
                    oeForm.belgeNo = d.mifBelgeNo;
                    ObjectArray oeBelgeleri = servisMUH.OdemeEmriMIFTekBelgeListele(kullanan, oeForm);
                    foreach (OdemeEmriMIFForm oeBelge in oeBelgeleri.objeler)
                    {
                        string odemeBelgeTuru = oeBelge.OEBTur;
                        faturaNo = oeBelge.OEBNo;
                        if (odemeBelgeTuru != "" && faturaNo != "") faturaNo = faturaNo + " nolu " + odemeBelgeTuru + " karşılığı";

                        if (oeBelge.ilgiliBankaNo != null && oeBelge.ilgiliBankaNo.Length > 2 &&
                            OrtakFonksiyonlar.ConvertToInt(oeBelge.ilgiliBankaNo.Substring(0, 2), 0) != 0)
                            bankaHesapNo = oeBelge.ilgiliBankaNo;
                    }
                }

                if (!ozet)
                {
                    BankaBilgi bb = TNS.MUH.Arac.HesapIBANBul(d.muhHesapKodu);

                    if (bankaAd.IndexOf(bb.BankaAd) < 0)
                    {
                        if (!string.IsNullOrWhiteSpace(bankaAd)) bankaAd += ", ";
                        bankaAd += bb.BankaAd;
                    }
                    if (subeAd.IndexOf(bb.SubeAd) < 0)
                    {
                        if (!string.IsNullOrWhiteSpace(subeAd)) subeAd += ", ";
                        subeAd += bb.SubeAd;
                    }

                    RakamiYaziveYaziylaYap(servisMUH, servisHRC, kullanan, d.mifYil, d.muhHesapKodu, d.tutar, out string tutar, out string paraBirimiStr, out string yaziyla, guvenli);

                    string vNo;
                    string ibanOn = "";
                    if (tekBelge)
                    {
                        vNo = "Vergi No.: " + d.tc_Vno;
                        if (!string.IsNullOrWhiteSpace(bb.IBAN))
                            ibanOn = "IBAN: ";
                        if (!string.IsNullOrWhiteSpace(bankaHesapNo))
                            bankaHesapNo = "(" + bankaHesapNo + " Nolu Hesaba)";
                    }
                    else
                        vNo = d.tc_Vno;
                    dtListe.Rows.Add(i + 1, d.mifBelgeNo, ibanOn + TNS.MUH.Arac.IBANBoslukla(bb.IBAN), d.ilgili, vNo, d.ilgBanka, TNS.MUH.Arac.IBANBoslukla(d.ilgIBAN), tutar, paraBirimiStr, d.aciklama, yaziyla, faturaNo, bankaHesapNo,
                        odemeTarih.isNull ? "" : odemeTarih.Gun.ToString("00"), odemeTarih.isNull ? "" : odemeTarih.Ay.ToString("00"), odemeTarih.isNull ? "" : (odemeTarih.Yil % 100).ToString("00"));
                }
            }

            MUHBankaOdemeDetay dTop = mbO.muhBankaOdemeDetay[0];
            RakamiYaziveYaziylaYap(servisMUH, servisHRC, kullanan, dTop.mifYil, dTop.muhHesapKodu, toplamTutar, out string tutarT, out string paraBirimiStrT, out string yaziylaT, guvenli);
            if (ozet && toplamTutar > 0)
            {
                dtListe.Rows.Add(1, "", "", "(EKLİ LİSTEDEKİ KİŞİLERE/FİRMALARA KARŞILARINDA GÖSTERİLEN TUTARLARIN", "BANKA HESAPLARINA AKTARILMAK ÜZERE)", "", "", tutarT, paraBirimiStrT, "", yaziylaT, "", "", "", "", "");
            }

            Aspose.Words.Document doc = new Aspose.Words.Document(sablonYol);
            if (bosluklariAt)
                doc.MailMerge.CleanupOptions = Aspose.Words.Reporting.MailMergeCleanupOptions.RemoveEmptyParagraphs;

            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            string odemeTarihStr = odemeTarih.isNull ? "" : odemeTarih.ToString();

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Hazirlayan", mbO.hazirlayan);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HazUnvan", kullanan.unvan);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Tarih", odemeTarihStr);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "MYUnvan", mbO.muhYetUnvan);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "MYAd", mbO.muhYetkili);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "OnayUnvan", mbO.onayUnvan);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "OnayAd", mbO.onaylayan);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KurumKodu", kurum);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KurumAdi", kurumAd);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Banka", bankaAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Sube", subeAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "OdemeTarih", odemeTarihStr);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil", odemeTarih.isNull ? "" : odemeTarih.Yil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Toplam", tutarT);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ToplamYaziyla", yaziylaT);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "DonemNo", mbO.donemNo.ToString());
            doc.MailMerge.ExecuteWithRegions(dtListe);

            string sonucDosyaad;
            Aspose.Words.SaveFormat sf;
            if (imza)
            {
                sonucDosyaad = DosyaIslem.DosyaAdUret("BankaListesi", "") + ".html";
                sf = Aspose.Words.SaveFormat.Html;
            }
            else
            {
                sonucDosyaad = DosyaIslem.DosyaAdUret("BankaListesi", "") + ".docx";
                sf = Aspose.Words.SaveFormat.Docx;
            }
            string parola = TNS.DEG.Arac.DegiskenDegerBul(0, "/MUHASEBE/CIKTIPAROLA");
            if (!string.IsNullOrWhiteSpace(parola) && !imza)
                doc.Protect(Aspose.Words.ProtectionType.ReadOnly, parola);
            doc.Save(sonucDosyaad, sf);
            return sonucDosyaad;
        }

        /// <summary>Eski RakamiYaziveYaziylaYap: tutar, para birimi ve yazıyla (hesap kodunun döviz cinsine göre).</summary>
        private static void RakamiYaziveYaziylaYap(IMUHServis servisMUH, TNS.HRC.IHRCServis servisHRC, Kullanici kullanan, int hesapYil, string hesapKodu, double d_tutar, out string tutar, out string birimKod, out string yaziyla, bool guvenli)
        {
            HesapPlaniSatir hps = new HesapPlaniSatir();
            hps.hesapKod = hesapKodu;
            hps.yil = hesapYil;
            ObjectArray oa = servisMUH.HesapPlaniListele(kullanan, hps);
            ParaBirimi kriter = new ParaBirimi();

            if (oa.ObjeSayisi == 1)
            {
                hps = (HesapPlaniSatir)oa.objeler[0];
                if (!string.IsNullOrWhiteSpace(hps.dovizKurKod))
                    kriter.kisaltma = hps.dovizKurKod;
                else
                    kriter.kod = 1;
            }
            else
                kriter.kod = 1;

            ObjectArray oaPB = servisHRC.ParaBirimiListele(kullanan, kriter);
            if (oaPB.sonuc.islemSonuc && oaPB.ObjeSayisi == 1)
                kriter = (ParaBirimi)oaPB.objeler[0];
            else
                kriter = null;

            if (kriter == null)
                birimKod = DondurParaBirimi(1).kisaltma;
            else
                birimKod = kriter.ParaBirimiKod;
            tutar = d_tutar.ToString("#,###.00");
            ParaIsmi pi;
            if (kriter == null)
                pi = new ParaIsmi("Lira", true, "Kuruş", true);
            else
                pi = kriter.ParaIsmiBul;

            yaziyla = OrtakFonksiyonlar.Yaziyla(d_tutar, pi);

            yaziyla = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(yaziyla.ToLower());
            if (guvenli)
                yaziyla = "(" + yaziyla.Replace(" ", "") + ")";
        }

        public static string Entersiz(string s)
        {
            return (s ?? "").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\"", " ").Replace("\'", " ");
        }

        /// <summary>Eski btnYazdirExcel_Click: Liste / Liste Grup (xls ya da pdf). Şablon MUH/BankaListesi.xltx (Ulusal Ajans'ta BankaListesiUA.xlt).</summary>
        private static IResult YazdirExcel(Kullanici kullanan, BankaListesiAnahtar a, string grup, bool pdf)
        {
            MUHBankaOdeme bilgi = BilgiOl(a);
            bilgi.grupOku = grup;
            MUHBankaOdeme mbO = Servis().BankaListesiOku(kullanan, bilgi);
            if (mbO == null || mbO.muhBankaOdemeDetay == null) return Hata("Liste okunamadı.");
            double toplamTutar = 0;

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            string sablonAd = "BankaListesi.xltx";
            int sablonNo = 1;
            if (TNS.HRC.OrtakServis.UlusalAjansMi())
            {
                sablonAd = "BankaListesiUA.xlt";
                sablonNo = 2;
            }
            string sablonYol = SablonYol(sablonAd);
            if (!File.Exists(sablonYol)) return Hata("Şablon (MUH/" + sablonAd + ") bulunamadı.");
            string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
            TNSDateTime odemeTarih = mbO.odemeTarih ?? new TNSDateTime();

            XLS.DosyaAc(sablonYol, sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            XLS.HucreAdBulYaz("KurumAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());
            XLS.HucreAdBulYaz("Tarih", odemeTarih.isNull ? "" : odemeTarih.ToString());
            XLS.HucreAdBulYaz("CekNo", mbO.cekEmriNo ?? "");
            XLS.HucreAdBulYaz("donemNo", mbO.donemNo);
            int sheetNo = -1;
            int formatSat1 = -1;
            int formatSut1 = -1;
            int formatSat2 = -1;
            int formatSut2 = -1;
            XLS.HucreAdAdresCoz("KaynakFormat", ref sheetNo, ref formatSat1, ref formatSut1, ref formatSat2, ref formatSut2);

            int ilkSatir = satir;
            int yazSatir = satir;
            for (int i = 0; i < mbO.muhBankaOdemeDetay.Length; i++)
            {
                MUHBankaOdemeDetay baslik = mbO.muhBankaOdemeDetay[i];
                toplamTutar += baslik.tutar;

                BankaBilgi bankaBilgi = TNS.MUH.Arac.HesapIBANBul(baslik.muhHesapKodu);
                string kurumIban = TNS.MUH.Arac.IBANBoslukla(bankaBilgi.IBAN);
                string personelIban = TNS.MUH.Arac.IBANBoslukla(baslik.ilgIBAN);

                XLS.SatirAc(yazSatir, 1);
                XLS.HucreKopyala(formatSat1, formatSut1, formatSat2, formatSut2, yazSatir, sutun);
                if (sablonNo == 2)
                {
                    XLS.HucreDegerYaz(yazSatir, sutun, odemeTarih.isNull ? "" : odemeTarih.ToString());
                    XLS.HucreDegerYaz(yazSatir, sutun + 1, bankaBilgi.musteriNo);
                    XLS.HucreDegerYaz(yazSatir, sutun + 2, kurumIban);

                    XLS.HucreDegerYaz(yazSatir, sutun + 3, TNS.MUH.Arac.IBANBoslukla(personelIban));
                    XLS.HucreDegerYaz(yazSatir, sutun + 4, baslik.tutar);
                    XLS.HucreDegerYaz(yazSatir, sutun + 5, baslik.tc_Vno ?? "");
                    XLS.HucreDegerYaz(yazSatir, sutun + 6, baslik.ilgili ?? "");
                }
                else
                {
                    XLS.HucreDegerYaz(yazSatir, sutun, i + 1);
                    XLS.HucreDegerYaz(yazSatir, sutun + 1, baslik.ilgBanka ?? "");
                    XLS.HucreDegerYaz(yazSatir, sutun + 2, baslik.ilgili ?? "");
                    XLS.HucreDegerYaz(yazSatir, sutun + 3, baslik.tc_Vno ?? "");
                    if (string.IsNullOrWhiteSpace(bilgi.grupOku))
                    {
                        XLS.HucreDegerYaz(yazSatir, sutun + 4, Entersiz(baslik.aciklama));
                        XLS.HucreDegerYaz(yazSatir, sutun + 5, baslik.mifBelgeNo + "/" + baslik.yevmiyeNoStr);
                    }
                    XLS.HucreDegerYaz(yazSatir, sutun + 6, baslik.tutar);
                    XLS.HucreDegerYaz(yazSatir, sutun + 7, TNS.MUH.Arac.IBANBoslukla(personelIban));
                }
                yazSatir++;
            }

            XLS.SatirYukseklikAyarla(ilkSatir, yazSatir, -1);

            if (sablonNo == 2)
            {
                XLS.HucreKopyala(formatSat1, formatSut1, formatSat2, formatSut2, yazSatir + 1, sutun);
                XLS.HucreDegerYaz(yazSatir + 1, sutun + 3, "TOPLAM");
                XLS.HucreDegerYaz(yazSatir + 1, sutun + 4, toplamTutar);
            }
            else
            {
                XLS.HucreAdBulYaz("Toplam", toplamTutar);
                ParaIsmi pi = new ParaIsmi("Lira", true, "Kuruş", true);
                string yaziyla = OrtakFonksiyonlar.Yaziyla(toplamTutar, pi);
                XLS.HucreAdBulYaz("Yaziyla", yaziyla);
            }

            string uzanti = GenelIslemler.ExcelTur();
            if (pdf)
            {
                uzanti = "pdf";
                XLS.CalculateFormula();
            }
            XLS.DosyaSaklamaFormatAta(uzanti);
            XLS.DosyaSaklaTamYol();
            return DosyaIslem.DosyaGonder(sonucDosyaAd, "BankaListesi_" + DosyaIslem.DosyaAdUretSade(), true, uzanti);
        }

        /// <summary>Eski btnYazdirDetay_Click: Liste (Detaylı), MUH/BankaListesiDetay.xltx.</summary>
        private static IResult YazdirDetay(Kullanici kullanan, BankaListesiAnahtar a, string grup)
        {
            MUHBankaOdeme bilgi = BilgiOl(a);
            bilgi.grupOku = grup;
            MUHBankaOdeme mbO = Servis().BankaListesiOku(kullanan, bilgi);
            if (mbO == null || mbO.muhBankaOdemeDetay == null) return Hata("Liste okunamadı.");
            double toplamTutar = 0;

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            string sablonYol = SablonYol("BankaListesiDetay.xltx");
            if (!File.Exists(sablonYol)) return Hata("Şablon (MUH/BankaListesiDetay.xltx) bulunamadı.");
            string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();

            XLS.DosyaAc(sablonYol, sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            XLS.HucreAdBulYaz("KurumAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());
            XLS.HucreAdBulYaz("Tarih", mbO.odemeTarih == null || mbO.odemeTarih.isNull ? "" : mbO.odemeTarih.ToString());
            XLS.HucreAdBulYaz("CekNo", mbO.cekEmriNo ?? "");
            XLS.HucreAdBulYaz("donemNo", mbO.donemNo);
            int sheetNo = -1;
            int formatSat1 = -1;
            int formatSut1 = -1;
            int formatSat2 = -1;
            int formatSut2 = -1;
            XLS.HucreAdAdresCoz("KaynakFormat", ref sheetNo, ref formatSat1, ref formatSut1, ref formatSat2, ref formatSut2);

            int ilkSatir = satir;
            int yazSatir = satir;
            for (int i = 0; i < mbO.muhBankaOdemeDetay.Length; i++)
            {
                MUHBankaOdemeDetay baslik = mbO.muhBankaOdemeDetay[i];
                toplamTutar += baslik.tutar;

                XLS.HucreKopyala(formatSat1, formatSut1, formatSat2, formatSut2, yazSatir, sutun);

                int sut = sutun;
                XLS.HucreDegerYaz(yazSatir, sut++, i + 1);
                XLS.HucreDegerYaz(yazSatir, sut++, baslik.muhHesapKodu ?? "");
                XLS.HucreDegerYaz(yazSatir, sut++, baslik.cekNo ?? "");
                XLS.HucreDegerYaz(yazSatir, sut++, bilgi.gun.Oku());
                XLS.HucreDegerYaz(yazSatir, sut++, baslik.tc_Vno ?? "");
                XLS.HucreDegerYaz(yazSatir, sut++, baslik.ilgili ?? "");
                XLS.HucreDegerYaz(yazSatir, sut++, baslik.mifBelgeNo ?? "");
                XLS.HucreDegerYaz(yazSatir, sut++, baslik.tutar);
                if (!TNSDateTime.IsNull(baslik.odemeEmriIslemTarih))
                    XLS.HucreDegerYaz(yazSatir, sut++, baslik.odemeEmriIslemTarih.Oku());
                else
                    sut++;
                XLS.HucreDegerYaz(yazSatir, sut++, baslik.odemeEmriNo ?? "");
                XLS.HucreDegerYaz(yazSatir, sut++, DurumToStr(mbO.durum));

                yazSatir++;
            }

            yazSatir++;
            XLS.HucreKopyala(formatSat1 + 2, formatSut1, formatSat2 + 2, formatSut2, yazSatir, sutun);
            XLS.HucreDegerYaz(yazSatir, sutun + 7, toplamTutar);

            XLS.SatirYukseklikAyarla(ilkSatir, yazSatir, -1);

            XLS.FormulleriSil();
            XLS.SatirSil(0, 7);
            XLS.SutunSil(0, 1);
            XLS.DosyaSaklaTamYol();
            return DosyaIslem.DosyaGonder(sonucDosyaAd, "BankaListesi_" + DosyaIslem.DosyaAdUretSade(), true, "xlsx");
        }

        // ------------------------------------------------------------------ EBYS'ye gönder (eski btnEvrakaGonder_Click)

        /// <summary>
        /// Eski kod listeyi eimzaBankaListesi*.dotx ile HTML'e çevirip UAEvrakAktar eklentisiyle (TNS.EvrakAktar) evrak sistemine gönderirdi.
        /// Bu eklenti eski .NET Framework uygulamasına bağlı bir kitaplıktır ve Core'da yüklenemez (Ödeme Emri / MİF'teki EvrakaGonderUA gibi);
        /// belge yine üretilir, eklenti bulunamadığı için eski kodun verdiği hata döner.
        /// </summary>
        private static async Task<IResult> Ebys(HttpContext baglam, IWebHostEnvironment ortam, BankaListesiAnahtar istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, k => kullanan = k);
            if (ret != null) return ret;
            if (AnahtarEksik(istek)) return Hata(ListeSecilmedi);
            try
            {
                AsposeLisans.Yukle();
                string sonucDosyaAd = DosyaUret(kullanan, istek, false, false, true, out string hata);
                if (sonucDosyaAd == null) return Hata(hata);
                try { File.Delete(sonucDosyaAd); } catch { }
                return Hata("Aktarım işlemi başarısız:UAEvrakAktar dosyası bulunamadı!");
            }
            catch (Exception ex)
            {
                return Hata("Aktarım işlemi başarısız:" + HataMetni(ex.Message, ""));
            }
        }

        // ------------------------------------------------------------------ bankaya gönder (eski btnYazdirBanka_Click)

        private static async Task<IResult> BankayaGonder(HttpContext baglam, IWebHostEnvironment ortam, BankaListesiAnahtar istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, k => kullanan = k);
            if (ret != null) return ret;
            if (AnahtarEksik(istek)) return Hata(ListeSecilmedi);
            try
            {
                IMUHServis servisMUH = Servis();
                MUHBankaOdeme mbO = servisMUH.BankaListesiOku(kullanan, BilgiOl(istek));
                if (mbO == null || mbO.muhBankaOdemeDetay == null || mbO.muhBankaOdemeDetay.Length == 0)
                    throw new Exception("Ödeme listesi boş olduğundan dosya oluşturulamadı!");
                MUHBankaOdemeDetay ilkBaslik = mbO.muhBankaOdemeDetay[0];
                BankaBilgi bankaBilgi = TNS.MUH.Arac.HesapIBANBul(ilkBaslik.muhHesapKodu);
                FTPIslem di = new FTPIslem(bankaBilgi.ftpAd); // "halkbankftp"

                if (mbO.odemeTarih == null || mbO.odemeTarih.isNull)
                    throw new Exception("Ödeme tarihi doldurulmamış!");

                if (mbO.odemeTarih.Oku() < DateTime.Now.Date && di.tur != FTPIslem.DI_TUR.FTPYOK)
                    throw new Exception("Ödeme tarihi eski olan bir dosya gönderilemez!");

                BankaAktarim ba = null;
                if (bankaBilgi.ftpAd.StartsWith("halk"))
                    ba = new BankaAktarimUAHalk(bankaBilgi, mbO);
                else if (bankaBilgi.ftpAd.StartsWith("vakif"))
                    ba = new BankaAktarimUAVakif(bankaBilgi, mbO);
                else if (bankaBilgi.ftpAd.StartsWith("ziraat"))
                    ba = new BankaAktarimTKDKZiraat(bankaBilgi, mbO);
                if (ba == null)
                    throw new Exception("Tanınmayan aktarım ftp adresi!");

                double toplamTutar = 0;
                string anaYol = OrtakFonksiyonlar.WebConfigOku("MuhasebeDosyaYol", "");
                string sonucDosyaAd = ba.SonucDosyaAd(servisMUH, kullanan, anaYol);

                using (StreamWriter sw = new StreamWriter(anaYol + "/" + sonucDosyaAd + ba.Uzanti(), false, TurkceAnsi()))
                {
                    ba.BaslikYaz(sw);
                    for (int i = 0; i < mbO.muhBankaOdemeDetay.Length; i++)
                    {
                        MUHBankaOdemeDetay baslik = mbO.muhBankaOdemeDetay[i];
                        ba.SatirYaz(sw, baslik);
                        toplamTutar += baslik.tutar;
                    }
                    ba.ToplamSatirYaz(sw, toplamTutar);
                }

                di.DosyadanFTPye(anaYol + "/" + sonucDosyaAd + ba.Uzanti(), sonucDosyaAd + ba.Uzanti(), false);

                Sonuc s = servisMUH.BankaListesiBankayaGonderildi(kullanan, mbO, sonucDosyaAd);
                if (s.islemSonuc)
                    return Results.Json(new { basarili = true, bilgi = string.Format("{0} isimli dosya aktarıldı.", sonucDosyaAd), dosyaAd = sonucDosyaAd });
                return Results.Json(new { basarili = true, bilgi = string.Format("{0} isimli dosya aktarıldı, fakat veri tabanına aktarıldı diye işlenemedi.", sonucDosyaAd) });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Banka dosyası gönderilemedi."));
            }
        }

        /// <summary>Eski StreamWriter(..., Encoding.Default): Türkçe Windows'ta 1254 kod sayfası.</summary>
        private static System.Text.Encoding TurkceAnsi()
        {
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                return System.Text.Encoding.GetEncoding(1254);
            }
            catch (Exception)
            {
                return System.Text.Encoding.Default;
            }
        }

        // ------------------------------------------------------------------ bankadan al (eski btnOkuBanka_Click)

        private static async Task<IResult> BankadanAl(HttpContext baglam, IWebHostEnvironment ortam, BankaListesiAnahtar istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, k => kullanan = k);
            if (ret != null) return ret;
            if (istek == null || Temiz(istek.gun) == "") return Hata(ListeSecilmedi);
            try
            {
                Sonuc s = Servis().BankaAkibetDosyasiIsle(kullanan, Tarih(istek.gun));
                if (s.islemSonuc) return Bilgi("İşlem tamamlandı");
                return Hata(HataMetni(s.hataStr, "İşlem yapılamadı."));
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İşlem yapılamadı."));
            }
        }

        // ------------------------------------------------------------------ gönderilen banka dosyası (eski DosyaIndir)

        private static IResult DosyaIndir(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            // Yalnızca dosya adı kabul edilir (klasör dışına çıkılamaz).
            string ad = Path.GetFileName(Q(baglam, "ad"));
            if (ad == "") return Hata("Dosya adı yok.");
            string anaYol = OrtakFonksiyonlar.WebConfigOku("MuhasebeDosyaYol", "");
            string dosya = Path.Combine(anaYol, ad + ".txt");
            if (!File.Exists(dosya))
            {
                // Halkbank dosyalarının uzantısı yoktur.
                string uzantisiz = Path.Combine(anaYol, ad);
                if (!File.Exists(uzantisiz)) return Hata(ad + " dosyası bulunamadı.");
                dosya = uzantisiz;
            }
            return Results.File(File.ReadAllBytes(dosya), "text/plain", ad + ".txt");
        }
    }

    // ====================================================================== banka dosyaları (eski IBankaAktarim ve türevleri)

    /// <summary>Eski IBankaAktarim: bankaya gönderilen ödeme dosyasının biçimi.</summary>
    public class BankaAktarim
    {
        public string tarihStr;
        public BankaBilgi bankaBilgi;
        public MUHBankaOdeme mbO;
        public int satirSay;

        public virtual string BankaDosyaOnAdOl(bool gonder)
        {
            return "";
        }

        public string SonucDosyaAd(IMUHServis servisMUH, Kullanici kullanan, string anaYol)
        {
            string onAd = BankaDosyaOnAdOl(true);
            int ekSiraNo = servisMUH.BankaDosyaSiraNoBul(kullanan, onAd);
            int dosyaSay = 0;
            string sonucDosyaAd = "";
            try
            {
                while (true)
                {
                    sonucDosyaAd = onAd + SiraNoEkle(ekSiraNo);
                    if (File.Exists(anaYol + "/" + sonucDosyaAd + Uzanti()))
                    {
                        ekSiraNo++;
                        dosyaSay++;
                        if (dosyaSay > 25) break;
                    }
                    else
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Dosya önceden gönderilmiş mi kontrolü yapılamadı!: " + ex.Message);
            }
            return sonucDosyaAd;
        }

        public virtual string SiraNoEkle(int ekSiraNo)
        {
            return OrtakFonksiyonlar.MetinYap(ekSiraNo, 2);
        }

        public virtual string Uzanti()
        {
            return "";
        }

        public virtual void BaslikYaz(StreamWriter sw) { }

        public virtual void SatirYaz(StreamWriter sw, MUHBankaOdemeDetay baslik) { }

        public virtual void ToplamSatirYaz(StreamWriter sw, double toplamTutar) { }
    }

    public class BankaAktarimUAHalk : BankaAktarim
    {
        int tur = 2;
        public BankaAktarimUAHalk(BankaBilgi bankaBilgi, MUHBankaOdeme mbO)
        {
            this.mbO = mbO;
            this.bankaBilgi = bankaBilgi;
        }

        public override string BankaDosyaOnAdOl(bool gonder)
        {
            return "M" + OrtakFonksiyonlar.CharEkle(bankaBilgi.musteriNo, '0', 8, OrtakFonksiyonlar.ENUMMetinEkleYon.SOL, true) + "." + (gonder ? "OG" : "OD") + OrtakFonksiyonlar.MetinYap(mbO.odemeTarih.Ay, 2) + OrtakFonksiyonlar.MetinYap(mbO.odemeTarih.Gun, 2);
        }

        public override void BaslikYaz(StreamWriter sw)
        {
            tarihStr = OrtakFonksiyonlar.TarihYYYYMMDD(mbO.odemeTarih);
            sw.WriteLine("B000" + tarihStr + "0000" + OrtakFonksiyonlar.CharEkle(bankaBilgi.musteriNo, '0', 12, OrtakFonksiyonlar.ENUMMetinEkleYon.SOL, true));
        }

        public override void SatirYaz(StreamWriter sw, MUHBankaOdemeDetay baslik)
        {
            string satir;

            if (tur == 1)
                satir = "D"
                    + tarihStr
                    + OrtakFonksiyonlar.CharEkle(bankaBilgi.MBSubeNo, '0', 5, OrtakFonksiyonlar.ENUMMetinEkleYon.SOL, true)
                    + OrtakFonksiyonlar.CharEkle(bankaBilgi.IBAN.Replace(" ", ""), ' ', 26, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                    + OrtakFonksiyonlar.MetinYap(0, 3)
                    + OrtakFonksiyonlar.MetinYap(0, 4)
                    + OrtakFonksiyonlar.MetinYap(0, 5)
                    + OrtakFonksiyonlar.CharEkle(baslik.ilgIBAN.Replace(" ", ""), ' ', 26, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                    + OrtakFonksiyonlar.MetinYap(baslik.tutar, 12, 2)
                    + OrtakFonksiyonlar.CharEkle(BankaListesiUclari.Entersiz(baslik.aciklama), ' ', 30, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                    + OrtakFonksiyonlar.CharEkle(baslik.mifBelgeNo + "-" + baslik.mifSira + "-" + baslik.sira.ToString(), ' ', 36, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                    + OrtakFonksiyonlar.CharEkle("", ' ', 14, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                    + OrtakFonksiyonlar.CharEkle(baslik.ilgili, ' ', 30, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                    + OrtakFonksiyonlar.CharEkle("", ' ', 60, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true);
            else if (tur == 2)
            {
                satir = "D"
                    + tarihStr
                    + OrtakFonksiyonlar.CharEkle(bankaBilgi.MBSubeNo, '0', 5, OrtakFonksiyonlar.ENUMMetinEkleYon.SOL, true)
                    + OrtakFonksiyonlar.CharEkle(bankaBilgi.IBAN.Replace(" ", ""), ' ', 26, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                    + OrtakFonksiyonlar.MetinYap(0, 3)
                    + OrtakFonksiyonlar.MetinYap(0, 4)
                    + OrtakFonksiyonlar.MetinYap(0, 5)
                    + OrtakFonksiyonlar.CharEkle(baslik.ilgIBAN.Replace(" ", ""), ' ', 26, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                    + OrtakFonksiyonlar.MetinYap(baslik.tutar, 12, 2)
                    + OrtakFonksiyonlar.CharEkle(BankaListesiUclari.Entersiz(baslik.aciklama), ' ', 750, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                    + OrtakFonksiyonlar.CharEkle(baslik.mifBelgeNo + "-" + baslik.mifSira + "-" + baslik.sira.ToString(), ' ', 36, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                    + OrtakFonksiyonlar.CharEkle("", ' ', 14, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                    + OrtakFonksiyonlar.CharEkle(baslik.ilgili, ' ', 30, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                    + OrtakFonksiyonlar.CharEkle("", ' ', 60, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true);
            }
            else
                satir = "";
            sw.WriteLine(satir);
        }

        public override void ToplamSatirYaz(StreamWriter sw, double toplamTutar)
        {
            if (tur == 1 || tur == 2)
                sw.WriteLine("S000" + OrtakFonksiyonlar.MetinYap(mbO.muhBankaOdemeDetay.Length, 6) + OrtakFonksiyonlar.MetinYap(toplamTutar, 12, 2));
        }
    }

    public class BankaAktarimUAVakif : BankaAktarim
    {
        public BankaAktarimUAVakif(BankaBilgi bankaBilgi, MUHBankaOdeme mbO)
        {
            this.mbO = mbO;
            this.bankaBilgi = bankaBilgi;
        }

        public override string Uzanti()
        {
            return ".txt";
        }

        public override string BankaDosyaOnAdOl(bool gonder)
        {
            return "UA" + OrtakFonksiyonlar.MetinYap(mbO.odemeTarih.Yil, 4) + OrtakFonksiyonlar.MetinYap(mbO.odemeTarih.Ay, 2) + OrtakFonksiyonlar.MetinYap(mbO.odemeTarih.Gun, 2) + "_";
        }

        public override void BaslikYaz(StreamWriter sw)
        {
            TNSDateTime bugun = new TNSDateTime(DateTime.Now);
            tarihStr = OrtakFonksiyonlar.TarihYYYYMMDD(mbO.odemeTarih);
            string bugunStr = OrtakFonksiyonlar.TarihYYYYMMDD(bugun);
            string iban = OrtakFonksiyonlar.CharEkle(bankaBilgi.IBAN.Replace(" ", ""), '0', 26, OrtakFonksiyonlar.ENUMMetinEkleYon.SOL, true);
            sw.WriteLine("H" + bugunStr + "001500081" + OrtakFonksiyonlar.CharEkle(iban.Substring(9, 17), ' ', 26, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true) + OrtakFonksiyonlar.CharEkle(bankaBilgi.MBSubeNo, '0', 5, OrtakFonksiyonlar.ENUMMetinEkleYon.SOL, true));
        }

        public override void SatirYaz(StreamWriter sw, MUHBankaOdemeDetay baslik)
        {
            string satir;
            satir = "D"
                + tarihStr
                + "    "
                + "     "
                + OrtakFonksiyonlar.CharEkle(baslik.ilgIBAN.Replace(" ", ""), ' ', 26, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                + OrtakFonksiyonlar.MetinYap(baslik.tutar, 18, 2)
                + "TL "
                + OrtakFonksiyonlar.CharEkle(BankaListesiUclari.Entersiz(baslik.aciklama), ' ', 100, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                + OrtakFonksiyonlar.CharEkle(baslik.ilgili, ' ', 40, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                + OrtakFonksiyonlar.CharEkle("", ' ', 70, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                + OrtakFonksiyonlar.CharEkle(baslik.tc_Vno, ' ', 11, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                + OrtakFonksiyonlar.CharEkle("", ' ', 95, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                + OrtakFonksiyonlar.CharEkle("", ' ', 56, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                + "00"
                + OrtakFonksiyonlar.CharEkle("", ' ', 45, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                + OrtakFonksiyonlar.CharEkle("", ' ', 32, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true);
            sw.WriteLine(satir);
        }

        public override void ToplamSatirYaz(StreamWriter sw, double toplamTutar)
        {
            sw.WriteLine("F0015" + OrtakFonksiyonlar.MetinYap(mbO.muhBankaOdemeDetay.Length, 6) + OrtakFonksiyonlar.MetinYap(toplamTutar, 18, 2));
        }
    }

    public class BankaAktarimTKDKZiraat : BankaAktarim
    {
        public BankaAktarimTKDKZiraat(BankaBilgi bankaBilgi, MUHBankaOdeme mbO)
        {
            satirSay = 0;
            this.mbO = mbO;
            this.bankaBilgi = bankaBilgi;
        }

        public override string Uzanti()
        {
            return ".txt";
        }

        public override string BankaDosyaOnAdOl(bool gonder)
        {
            return OrtakFonksiyonlar.MetinYap(mbO.odemeTarih.Yil, 4) + OrtakFonksiyonlar.MetinYap(mbO.odemeTarih.Ay, 2) + OrtakFonksiyonlar.MetinYap(mbO.odemeTarih.Gun, 2) + "_" + bankaBilgi.musteriNo;
        }

        public override string SiraNoEkle(int ekSiraNo)
        {
            if (ekSiraNo <= 1)
                return "";
            else
                return "_" + OrtakFonksiyonlar.MetinYap(ekSiraNo, 2);
        }

        public override void BaslikYaz(StreamWriter sw)
        {
            TNSDateTime bugun = new TNSDateTime(DateTime.Now);
            tarihStr = OrtakFonksiyonlar.TarihYYYYMMDD(mbO.odemeTarih);
            sw.WriteLine("H" + OrtakFonksiyonlar.CharEkle(bankaBilgi.musteriNo, '0', 8, OrtakFonksiyonlar.ENUMMetinEkleYon.SOL, true) +
                OrtakFonksiyonlar.TarihYYMMDD(bugun) +
                OrtakFonksiyonlar.TarihHHMM(bugun) +
                tarihStr +
                OrtakFonksiyonlar.CharEkle(mbO.aciklama, ' ', 50, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true));
        }

        public override void SatirYaz(StreamWriter sw, MUHBankaOdemeDetay baslik)
        {
            satirSay++;
            string satir;
            satir = "D"
                + OrtakFonksiyonlar.MetinYap(satirSay, 7)
                + "M0399"
                + tarihStr
                + OrtakFonksiyonlar.CharEkle("", '0', 9, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                + OrtakFonksiyonlar.CharEkle(baslik.ilgIBAN.Replace(" ", ""), ' ', 26, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                + OrtakFonksiyonlar.CharEkle("", ' ', 16, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                + OrtakFonksiyonlar.MetinYap(baslik.tutar, 13, 2)
                + IlgiliOl(baslik.ilgili, baslik.tc_Vno)
                + OrtakFonksiyonlar.CharEkle("", ' ', 30, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                + OrtakFonksiyonlar.CharEkle(baslik.tc_Vno, ' ', 11, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true)
                + OrtakFonksiyonlar.CharEkle(BankaListesiUclari.Entersiz(baslik.aciklama), ' ', 100, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true);
            sw.WriteLine(satir);
        }

        private string IlgiliOl(string ilgili, string vergiNo)
        {
            string ad = "";
            string soyad = "";
            ilgili = (ilgili ?? "").Trim();
            if ((vergiNo ?? "").Length == 11)
            {
                int boslukYer = ilgili.LastIndexOf(' ');
                if (boslukYer > 0)
                {
                    ad = ilgili.Substring(0, boslukYer);
                    soyad = ilgili.Substring(boslukYer + 1);
                }
            }

            if (!string.IsNullOrEmpty(ad) && !string.IsNullOrEmpty(soyad))
                return OrtakFonksiyonlar.CharEkle(ad, ' ', 30, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true) + OrtakFonksiyonlar.CharEkle(soyad, ' ', 30, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true);
            else
                return OrtakFonksiyonlar.CharEkle(ilgili, ' ', 60, OrtakFonksiyonlar.ENUMMetinEkleYon.SAG, true);
        }

        public override void ToplamSatirYaz(StreamWriter sw, double toplamTutar)
        {
            sw.WriteLine("T" + OrtakFonksiyonlar.MetinYap(mbO.muhBankaOdemeDetay.Length, 7) + OrtakFonksiyonlar.MetinYap(toplamTutar, 13, 2));
        }
    }
}
