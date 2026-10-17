using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.ATM;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.AlacakTakip
{
    public class OdemeDosyaIstegi
    {
        public int yil { get; set; }
        public int dosyaNo { get; set; }
    }

    public class OdemePlanIstegi
    {
        public int yil { get; set; }
        public int dosyaNo { get; set; }
        public int taksitSayisi { get; set; }
        public int periyod { get; set; }
        /// <summary>yyyy-MM-dd</summary>
        public string ilkOdemeTarihi { get; set; }
    }

    /// <summary>Taksit tablosunun bir satırı (eski grdListe.getRowsValues()); alan adları eski kayıt alanlarıyla aynıdır.</summary>
    public class OdemeTaksitSatiri
    {
        public int SIRANO { get; set; }
        public double? TUTAR { get; set; }
        public double? TAKSITFAIZI { get; set; }
        public double? FAIZTUTAR { get; set; }
        /// <summary>Son ödeme tarihi, yyyy-MM-dd (boş olabilir).</summary>
        public string ODEMETARIH { get; set; }
        /// <summary>Ödemenin yapıldığı tarih, yyyy-MM-dd (boş olabilir).</summary>
        public string GERCEKODEMETARIH { get; set; }
        public string MAKBUZNO { get; set; }
    }

    public class OdemeKaydetIstegi
    {
        public int yil { get; set; }
        public int dosyaNo { get; set; }
        public List<OdemeTaksitSatiri> taksitler { get; set; }
    }

    public class OdemeHesaplaIstegi
    {
        public int yil { get; set; }
        public int dosyaNo { get; set; }
        public double odemeTutari { get; set; }
        /// <summary>yyyy-MM-dd</summary>
        public string odemeTarihi { get; set; }
    }

    public class OdemeTahakkukIstegi
    {
        public int yil { get; set; }
        public int dosyaNo { get; set; }
        /// <summary>Taksit tahakkukunda taksitin sıra numarası (açılış tahakkukunda kullanılmaz).</summary>
        public int siraNo { get; set; }
        public string borcHesap { get; set; }
        public string alacakHesap { get; set; }
        /// <summary>Yalnızca taksit (faiz / ödeme) tahakkukunda.</summary>
        public string odemeHesap { get; set; }
    }

    /// <summary>
    /// Ödeme Bilgileri (eski AlacakTakip/Odeme.aspx) uçları. Sayfa Takip İşlemleri (AlacakTakipListe) ekranından
    /// Sgb.pencere('../AlacakTakip/Odeme?yil=&amp;dosyaNo=&amp;paraBirimi=') ile açılır:
    ///   GET  /api/alacaktakip/odeme/bilgi?yil=&amp;dosyaNo=     borç miktarı, açılış tahakkuku, taksitler (eski Listele)
    ///   POST /api/alacaktakip/odeme/taksit-ekle               Ödeme Satırı Aç (eski btnTaksitEkle_Click)
    ///   POST /api/alacaktakip/odeme/plan                      Ödeme Planı Hazırla (eski btnOdemePlani_Click)
    ///   POST /api/alacaktakip/odeme/kaydet                    taksitleri kaydeder (eski btnKaydet_Click / Kaydet)
    ///   GET  /api/alacaktakip/odeme/yazdir?yil=&amp;dosyaNo=    Excel (eski btnYazdir_Click, ATM\ATM-OdemeListe.xlt)
    ///   POST /api/alacaktakip/odeme/hesapla                   Ana Paradan Faiz Hesapla (eski btnHesapla_Click)
    ///   POST /api/alacaktakip/odeme/acilis-tahakkuk           Açılış Tahakkuku Oluştur (eski btnOdemeEmriOlusturDosya)
    ///   POST /api/alacaktakip/odeme/taksit-tahakkuk           satırdaki Tahakkuk Oluştur: faiz ve ödeme tahakkuku (eski btnOdemeEmriOlusturTaksit)
    ///   GET  /api/alacaktakip/odeme/hesap/agac, hesap/ara     hesap kodu seçimi (Sgb.hesapPlaniSec penceresi; Makbuz uçlarının aynısı,
    ///                                                          yetkisi bu sayfadan; eski HesapKod.ascx)
    /// Sayfa menüde yok; yetkisi üst sayfa Takip İşlemleri'nden (AlacakTakipListe / AlacakTakipListeAB) gelir ve eskisi gibi
    /// TNS.ATM.Yetki.SayfayaGirisYapabilirMi de aranır. Dosya, taksit ve tahakkuk yetkileri iş kurallarında (TNS.ATM, TNS.MUH) denetlenir.
    /// </summary>
    public static class OdemeUclari
    {
        public const string SayfaAdresi = "../AlacakTakip/Odeme.aspx";
        private const string Uc = "/api/alacaktakip/odeme/";

        public static readonly string[] UstSayfalar =
        {
            "../AlacakTakip/AlacakTakipListe.aspx",
            "../AlacakTakip/AlacakTakipListeAB.aspx"
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "bilgi", Bilgi);
            app.MapPost(Uc + "taksit-ekle", TaksitEkle);
            app.MapPost(Uc + "plan", Plan);
            app.MapPost(Uc + "kaydet", Kaydet);
            app.MapGet(Uc + "yazdir", Yazdir);
            app.MapPost(Uc + "hesapla", Hesapla);
            app.MapPost(Uc + "acilis-tahakkuk", AcilisTahakkuk);
            app.MapPost(Uc + "taksit-tahakkuk", TaksitTahakkuk);
            app.MapGet(Uc + "hesap/agac", HesapAgac);
            app.MapGet(Uc + "hesap/ara", HesapAra);
        }

        // ------------------------------------------------------------------ yetki ve ortak

        public static bool Yetkili(Kullanici kullanan, string klasor)
        {
            if (kullanan == null) return false;
            return SayfaYetki.Yetkili(kullanan, klasor, SayfaAdresi, UstSayfalar) && TNS.ATM.Yetki.SayfayaGirisYapabilirMi(kullanan);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Yetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IATMServis Servis() { return TNS.ATM.Arac.Tanimla(); }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static IResult Basarisiz(string hata) { return Results.Json(new { basarili = false, hata = hata ?? "" }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static bool TarihCoz(string metin, out DateTime tarih)
        {
            return DateTime.TryParseExact(Temiz(metin), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tarih);
        }

        /// <summary>yyyy-MM-dd metninden TNSDateTime; boş ya da hatalıysa boş tarih (eski Ortak.DegerAlDateTime / new TNSDateTime(metin)).</summary>
        private static TNSDateTime Tarih(string metin)
        {
            TNSDateTime t = new TNSDateTime();
            if (TarihCoz(metin, out DateTime d)) t.Yaz(d);
            return t;
        }

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

        private static string KurumKod() { return (GenelIslemlerIstemci.VarsayilanKurumBul() ?? "").Replace(".", ""); }

        /// <summary>Eski sayfalardaki DosyaListele çağrısı (muhasebe borç durumu kuruma göre).</summary>
        private static ObjectArray DosyaListele(IATMServis servisATM, Kullanici kullanan, int yil, int dosyaNo)
        {
            Dosya alacakTakip = new Dosya();
            alacakTakip.yil = yil;
            alacakTakip.dosyaNo = dosyaNo;
            alacakTakip.muhasebeBorcDurumuAl = TNS.ATM.Arac.MuhasebedenGelsin(KurumKod());
            return servisATM.DosyaListele(kullanan, alacakTakip);
        }

        private static Dosya DosyaOku(IATMServis servisATM, Kullanici kullanan, int yil, int dosyaNo)
        {
            ObjectArray bilgi = DosyaListele(servisATM, kullanan, yil, dosyaNo);
            if (bilgi == null || bilgi.objeler == null || bilgi.objeler.Count == 0) return null;
            return (Dosya)bilgi.objeler[0];
        }

        /// <summary>"yıl/belgeNo" biçimindeki ödeme emri numarasını parçalar.</summary>
        private static void BelgeCoz(string odemeEmriNo, out string belgeYil, out string belgeNo)
        {
            belgeYil = "";
            belgeNo = "";
            string[] parca = (odemeEmriNo ?? "").Split('/');
            if (parca.Length > 1)
            {
                belgeYil = parca[0];
                belgeNo = parca[1];
            }
        }

        // ------------------------------------------------------------------ bilgi ve taksitler

        /// <summary>Açılış ve her işlemden sonra (eski Listele): borç miktarı, açılış tahakkuku, taksitler.</summary>
        private static IResult Bilgi(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? dosyaNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((yil ?? 0) <= 0 || (dosyaNo ?? 0) <= 0) return Hata("Yıl ve dosya numarası bilgisi eksik.");
            try
            {
                ObjectArray dosya = DosyaListele(Servis(), kullanan, yil.Value, dosyaNo.Value);
                if (dosya == null || dosya.sonuc == null || !dosya.sonuc.islemSonuc)
                    return Hata(HataMetni(dosya?.sonuc?.hataStr, "Dosya bilgileri okunamadı."));
                if (dosya.objeler.Count == 0) return Hata("Alacak dosyası bulunamadı.");

                Dosya alacakTakip = (Dosya)dosya.objeler[0];
                double borcMiktari = TNS.ATM.Arac.BorcBul(alacakTakip, KurumKod());

                BelgeCoz(alacakTakip.odemeEmriNo, out string oeYil, out string oeNo);

                List<object> satirlar = new List<object>();
                if (alacakTakip.taksitBilgi != null)
                {
                    foreach (DosyaTaksit taksit in alacakTakip.taksitBilgi.objeler)
                    {
                        BelgeCoz(taksit.odemeEmriNo, out string odemeBelgeYil, out string odemeBelgeNo);
                        BelgeCoz(taksit.faizOdemeEmriNo, out string _, out string faizBelgeNo);

                        satirlar.Add(new
                        {
                            ODENDI = taksit.odendi,
                            SIRANO = taksit.siraNo,
                            TUTAR = taksit.miktar,
                            FAIZTUTAR = taksit.faizMiktar,
                            TAKSITFAIZI = taksit.taksitTarihiFaizi,
                            SATIRTUTAR = taksit.SATIRTUTAR,
                            ODEMETARIH = TarihYaz(taksit.sonOdemeTarihi),
                            GERCEKODEMETARIH = TarihYaz(taksit.odemeTarihi),
                            MAKBUZNO = taksit.makbuzNo ?? "",
                            ODEMEEMRIYIL = odemeBelgeYil,
                            ODEMEEMRINO = odemeBelgeNo,
                            FAIZODEMEEMRINO = faizBelgeNo
                        });
                    }
                }

                return Results.Json(new
                {
                    yil = yil.Value,
                    dosyaNo = dosyaNo.Value,
                    borcMiktari = borcMiktari,
                    odemeEmriYil = oeYil,
                    odemeEmriNo = oeNo,
                    odemeEmriMetin = oeNo != "" ? alacakTakip.odemeEmriNo : "",
                    satirlar = satirlar
                });
            }
            catch (Exception)
            {
                return Hata("Dosya bilgileri okunamadı.");
            }
        }

        /// <summary>Ödeme Satırı Aç (eski btnTaksitEkle_Click): dosyaya bir taksit satırı ekler.</summary>
        private static async Task<IResult> TaksitEkle(HttpContext baglam, IWebHostEnvironment ortam, OdemeDosyaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.yil <= 0 || istek.dosyaNo <= 0) return Basarisiz("Yıl ve dosya numarası bilgisi eksik.");
            try
            {
                OdemePlani odemePlani = new OdemePlani();
                odemePlani.yil = istek.yil;
                odemePlani.dosyaNo = istek.dosyaNo;
                odemePlani.taksitSayisi = 1;

                Sonuc sonuc = Servis().TaksitEkle(kullanan, odemePlani);
                if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "Taksit eklenemedi."));
                return Results.Json(new { basarili = true, bilgi = "Taksit eklendi." });
            }
            catch (Exception)
            {
                return Basarisiz("Taksit eklenemedi.");
            }
        }

        /// <summary>Ödeme Planı Hazırla (eski btnOdemePlani_Click): taksit sayısı, periyot (ay) ve ilk ödeme tarihine göre taksitler.</summary>
        private static async Task<IResult> Plan(HttpContext baglam, IWebHostEnvironment ortam, OdemePlanIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.yil <= 0 || istek.dosyaNo <= 0) return Basarisiz("Yıl ve dosya numarası bilgisi eksik.");
            try
            {
                OdemePlani odemePlani = new OdemePlani();
                odemePlani.yil = istek.yil;
                odemePlani.dosyaNo = istek.dosyaNo;
                odemePlani.taksitSayisi = istek.taksitSayisi;
                odemePlani.periyod = istek.periyod;
                odemePlani.ilkOdemeTarihi = Tarih(istek.ilkOdemeTarihi);

                Sonuc sonuc = Servis().OdemePlaniHazirla(kullanan, odemePlani);
                if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "Ödeme planı hazırlanamadı."));
                return Results.Json(new { basarili = true, bilgi = "Ödeme planı başarıyla hazırlandı." });
            }
            catch (Exception)
            {
                return Basarisiz("Ödeme planı hazırlanamadı.");
            }
        }

        /// <summary>
        /// Kaydet (eski btnKaydet_Click + Kaydet): tablodaki bütün taksitler. Taksit tutarları toplamı borç miktarını geçemez
        /// (borç miktarı eskiden sayfadaki gizli alandan, şimdi dosyadan yeniden okunur).
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, OdemeKaydetIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.yil <= 0 || istek.dosyaNo <= 0) return Basarisiz("Yıl ve dosya numarası bilgisi eksik.");
            if (istek.taksitler == null || istek.taksitler.Count == 0) return Basarisiz("Herhangi bir taksit seçilmemiş!");
            try
            {
                IATMServis servisATM = Servis();

                decimal toplamTutar = 0;
                Dosya dosya = new Dosya();
                dosya.yil = istek.yil;
                dosya.dosyaNo = istek.dosyaNo;
                dosya.taksitBilgi = new ObjectArray();
                foreach (OdemeTaksitSatiri row in istek.taksitler)
                {
                    if (row == null) continue;
                    DosyaTaksit p = new DosyaTaksit();
                    p.siraNo = row.SIRANO;
                    p.miktar = row.TUTAR ?? 0;
                    p.taksitTarihiFaizi = row.TAKSITFAIZI ?? 0;
                    p.faizMiktar = row.FAIZTUTAR ?? 0;
                    p.odemeTarihi = Tarih(row.GERCEKODEMETARIH);
                    p.sonOdemeTarihi = Tarih(row.ODEMETARIH);
                    p.makbuzNo = Temiz(row.MAKBUZNO);

                    toplamTutar += Convert.ToDecimal(p.miktar);
                    dosya.taksitBilgi.Ekle(p);
                }

                if (dosya.taksitBilgi.objeler.Count <= 0) return Basarisiz("Herhangi bir taksit seçilmemiş!");

                Dosya kayitli = DosyaOku(servisATM, kullanan, istek.yil, istek.dosyaNo);
                if (kayitli == null) return Basarisiz("Alacak dosyası bulunamadı.");
                decimal borcMiktari = Math.Round(Convert.ToDecimal(TNS.ATM.Arac.BorcBul(kayitli, KurumKod())), 2);
                if (Math.Round(toplamTutar, 2) > borcMiktari) return Basarisiz("Taksit tutarları toplamı borç miktarından büyük olamaz");

                Sonuc sonuc = servisATM.TaksitOdendi(kullanan, dosya);
                if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "Kayıt işlemi yapılamadı."));
                return Results.Json(new { basarili = true, bilgi = "İşlem başarıyla tamamlandı." });
            }
            catch (Exception)
            {
                return Basarisiz("Kayıt işlemi yapılamadı.");
            }
        }

        /// <summary>Yazdır (eski btnYazdir_Click): dosyanın taksit listesi, Excel.</summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? dosyaNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((yil ?? 0) <= 0 || (dosyaNo ?? 0) <= 0) return Hata("Yıl ve dosya numarası bilgisi eksik.");
            string sablon = OdemeRaporlari.SablonBul(ortam.ContentRootPath, OdemeRaporlari.OdemeListeSablon);
            if (sablon == null) return Hata("Rapor şablonu bulunamadı (ATM\\" + OdemeRaporlari.OdemeListeSablon + ").");
            try
            {
                IATMServis servisATM = Servis();
                // Eski kod dosyayı da okuyordu (okuma yetkisi iş kuralında denetlenir); dosya bulunamazsa rapor üretilmez.
                if (DosyaOku(servisATM, kullanan, yil.Value, dosyaNo.Value) == null) return Hata("Alacak dosyası bulunamadı.");

                ObjectArray bilgi = servisATM.TaksitListele(kullanan, yil.Value, dosyaNo.Value);
                if (bilgi == null || bilgi.sonuc == null || !bilgi.sonuc.islemSonuc || bilgi.objeler.Count <= 0)
                    return Hata("Yazdırılacak taksit bulunamadı.");

                AsposeLisans.Yukle();
                string dosya = OdemeRaporlari.OdemeListe(dosyaNo.Value, bilgi, sablon);
                string gidenAd = "ATM-OdemeListe_" + yil.Value + "_" + dosyaNo.Value;
                return DosyaIslem.DosyaGonder(dosya, gidenAd, true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }

        // ------------------------------------------------------------------ ana paradan faiz hesapla

        /// <summary>
        /// Ana Paradan Faiz Hesapla (eski btnHesapla_Click): ödenen tutarın ana para ve faiz ayrımı. Eski kodun aynısıdır; yalnızca
        /// faiz türü oranı 9 olan dosyaların dönemleri iş kuralındaki (TNS.ATM.Arac.FaizOrani) güncel dönemlerle aynı yapıldı.
        /// </summary>
        private static async Task<IResult> Hesapla(HttpContext baglam, IWebHostEnvironment ortam, OdemeHesaplaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.yil <= 0 || istek.dosyaNo <= 0) return Basarisiz("Yıl ve dosya numarası bilgisi eksik.");

            TNSDateTime odemeTarihi = Tarih(istek.odemeTarihi);
            if (odemeTarihi.isNull) return Basarisiz("Lütfen ödeme tarihini giriniz");
            double odemeTutari = istek.odemeTutari;
            if (odemeTutari <= 0) return Basarisiz("Lütfen ödeme tutarını giriniz");

            try
            {
                Dosya f = DosyaOku(Servis(), kullanan, istek.yil, istek.dosyaNo);
                if (f == null) return Basarisiz("Alacak dosyası bulunamadı.");

                double faizOrani = 0;
                double faiz = 0;
                double taksitTarihiOrani = 0;
                double taksitTarihiFaizi = 0;
                double anaPara = 0;
                if (!TNSDateTime.IsNull(f.taksitBaslangicTarihi) && odemeTarihi.Oku() > f.taksitBaslangicTarihi.Oku())
                {
                    TNS.FaizIslemler.FaizDonem[] faizOran;
                    TNS.FaizIslemler.FaizDonem[] faizOranAy;

                    if (OrtakFonksiyonlar.EsitMi(f.faizTur.oran, 0) || OrtakFonksiyonlar.EsitMi(f.faizTur.oran, 99))
                    {
                        faizOran = TNS.FaizIslemler.faizOran;
                        faizOranAy = TNS.FaizIslemler.FaizOranAyBul(f.taksitBaslangicTarihi.Oku(), 0);
                    }
                    else if (OrtakFonksiyonlar.EsitMi(f.faizTur.oran, 9))
                    {
                        faizOran = new TNS.FaizIslemler.FaizDonem[3];
                        faizOran[0] = new TNS.FaizIslemler.FaizDonem(new DateTime(2024, 6, 1), 9.0 / 1200);
                        faizOran[1] = new TNS.FaizIslemler.FaizDonem(new DateTime(2026, 8, 1), 24.0 / 1200);
                        faizOran[2] = new TNS.FaizIslemler.FaizDonem(DateTime.MaxValue, 31.0 / 1200);
                        faizOranAy = faizOran;
                    }
                    else
                    {
                        faizOran = new TNS.FaizIslemler.FaizDonem[1];
                        faizOran[0] = new TNS.FaizIslemler.FaizDonem(DateTime.MaxValue, f.faizTur.oran / 1200);
                        faizOranAy = faizOran;
                    }
                    int ay1, gun1, ay2, gun2;

                    double toplamFaizOran = TNS.FaizIslemler.FaizHesapla(f.faizBaslangicTarihi.Oku(), f.faizBaslangicTarihi.Oku(), f.taksitBaslangicTarihi.Oku(), odemeTarihi.Oku(), DateTime.MinValue, 1000000, faizOran, faizOranAy, false, DateTime.MinValue, null, out faizOrani, out taksitTarihiOrani, out ay1, out gun1, out ay2, out gun2) / 1000000;
                    double toplamFaiz;
                    TNS.FaizIslemler.AnaParaFaizBol(odemeTutari, toplamFaizOran, out anaPara, out toplamFaiz);

                    DosyaTaksit dt = new DosyaTaksit();
                    dt.odemeTarihi = odemeTarihi;
                    dt.miktar = anaPara;
                    dt.FaizleriHesapla(f);
                    faiz = dt.faizMiktar;
                    taksitTarihiFaizi = dt.taksitTarihiFaizi;
                }
                else
                {
                    faizOrani = TNS.ATM.Arac.FaizOrani(f.faizTur.oran, odemeTarihi.Oku(), f.faizBaslangicTarihi.Oku(), f.faizBaslangicTarihi.Oku(), f.faizBaslangicTarihi.Oku(), 0);
                    TNS.FaizIslemler.AnaParaFaizBol(odemeTutari, faizOrani, out anaPara, out faiz);
                }

                return Results.Json(new
                {
                    basarili = true,
                    anaPara = anaPara.ToString("###0.00"),
                    faizTutari = faiz.ToString("###0.00"),
                    faizOrani = faizOrani.ToString("###0.000000"),
                    taksitTarihiOrani = taksitTarihiOrani.ToString("###0.000000"),
                    taksitTarihiFaizi = taksitTarihiFaizi.ToString("###0.00")
                });
            }
            catch (Exception)
            {
                return Basarisiz("Hesaplama yapılamadı.");
            }
        }

        // ------------------------------------------------------------------ tahakkuklar (ödeme emri)

        /// <summary>
        /// Açılış Tahakkuku Oluştur (eski btnOdemeEmriOlusturDosya): dosyanın borcu kadar borç / alacak hesaplı ödeme emri
        /// (varsayılan 140.01.01 / 600.05.09.01.06). Dosyanın açılış tahakkuku varsa aynı belge güncellenir.
        /// </summary>
        private static async Task<IResult> AcilisTahakkuk(HttpContext baglam, IWebHostEnvironment ortam, OdemeTahakkukIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.yil <= 0 || istek.dosyaNo <= 0) return Basarisiz("Yıl ve dosya numarası bilgisi eksik.");
            string borcHesap = Temiz(istek.borcHesap);
            string alacakHesap = Temiz(istek.alacakHesap);
            if (borcHesap == "" || alacakHesap == "") return Basarisiz("Borç ve alacak hesap kodlarını giriniz.");
            try
            {
                IATMServis servisATM = Servis();
                int yil = istek.yil;
                int dosyaNo = istek.dosyaNo;

                ObjectArray bilgi = DosyaListele(servisATM, kullanan, yil, dosyaNo);
                if (bilgi == null || bilgi.objeler == null || bilgi.objeler.Count == 0) return Basarisiz("Alacak dosyası bulunamadı.");

                OdemeEmriMIFForm oe = new OdemeEmriMIFForm();
                double tutar = 0.0;

                foreach (Dosya d in bilgi.objeler)
                {
                    // Eski hdnOdemeEmriYil / hdnOdemeEmriNo: dosyanın kayıtlı açılış tahakkuku
                    BelgeCoz(d.odemeEmriNo, out string oeYil, out string oeNo);

                    oe.muhasebe = d.saymanlikKodu;
                    oe.birim = d.birimKodu;
                    oe.islemTarih = new TNSDateTime(DateTime.Now);
                    oe.yil = OrtakFonksiyonlar.ConvertToInt(oeYil, oe.islemTarih.Yil);
                    oe.ilgiliAd = d.borcluAd;
                    oe.ilgiliNo = d.kimlikNo;
                    oe.kurum = TNS.MUH.Arac.VarsayilanKurumKod();
                    oe.aciklama = yil + "/" + dosyaNo + " numaralı alacak dosyası kapsamında oluşturulmuştur";
                    oe.belgeNo = oeNo;
                    tutar = TNS.ATM.Arac.BorcBul(d, KurumKod());
                }

                ObjectArray detay = new ObjectArray();
                OdemeEmriMIFDetay td;

                td = new OdemeEmriMIFDetay(oe.yil);
                td.hesapKod = borcHesap; //"140.01.01";
                td.borc = Math.Round(tutar, 2);
                td.alacak = 0.0;
                detay.objeler.Add(td);

                td = new OdemeEmriMIFDetay(oe.yil);
                td.hesapKod = alacakHesap; //"600.05.09.01.06";
                td.borc = 0.0;
                td.alacak = Math.Round(tutar, 2);
                detay.objeler.Add(td);

                oe.detay = detay;

                IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
                Sonuc sonuc = servisMUH.OdemeEmriMIFKaydet(kullanan, oe);
                if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "Tahakkuk oluşturulamadı."));

                string belgeNo = sonuc.anahtar;
                sonuc = servisATM.OdemeEmriKaydetDosya(yil, dosyaNo, oe.yil, belgeNo);
                if (sonuc == null || !sonuc.islemSonuc)
                    return Results.Json(new
                    {
                        basarili = false,
                        hata = HataMetni(sonuc?.hataStr, "Tahakkuk belgesi oluşturuldu ancak dosyaya bağlanamadı."),
                        belgeYil = oe.yil,
                        belgeNo = belgeNo
                    });

                return Results.Json(new { basarili = true, bilgi = "Ödeme emri belgesi başarıyla oluşturuldu.", belgeYil = oe.yil, belgeNo = belgeNo });
            }
            catch (Exception)
            {
                return Basarisiz("Tahakkuk oluşturulamadı.");
            }
        }

        /// <summary>
        /// Satırdaki Tahakkuk Oluştur (eski btnOdemeEmriOlusturTaksit): ödemesi yapılmış taksit için, faizi varsa faiz tahakkuku
        /// (borç / alacak hesapları, varsayılan 140.01.02 / 600.05.01.09.01), sonra ödeme tahakkuku (ödeme hesabı ile açılış ve faiz
        /// tahakkuklarının kapatılması, yansıtma ve 805 satırları). Önce açılış tahakkuku oluşturulmuş olmalıdır.
        /// </summary>
        private static async Task<IResult> TaksitTahakkuk(HttpContext baglam, IWebHostEnvironment ortam, OdemeTahakkukIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.yil <= 0 || istek.dosyaNo <= 0) return Basarisiz("Yıl ve dosya numarası bilgisi eksik.");

            string odemeHesapKodu = Temiz(istek.odemeHesap);
            if (string.IsNullOrEmpty(odemeHesapKodu)) return Basarisiz("Ödeme Tahakkuku hesap kodu boş bırakılmaz");

            try
            {
                IATMServis servisATM = Servis();
                int yil = istek.yil;
                int dosyaNo = istek.dosyaNo;

                ObjectArray bilgi = DosyaListele(servisATM, kullanan, yil, dosyaNo);
                if (bilgi == null || bilgi.objeler == null || bilgi.objeler.Count == 0) return Basarisiz("Alacak dosyası bulunamadı.");

                // Eski hdnOdemeEmriYil / hdnOdemeEmriNo
                BelgeCoz(((Dosya)bilgi.objeler[0]).odemeEmriNo, out string acilisYilStr, out string acilisNo);
                if (string.IsNullOrEmpty(acilisNo))
                    return Basarisiz("Açılış tahakkuk oluşturulmamış. İlk önce açılış tahakkunu oluşturun.");
                int acilisYil = OrtakFonksiyonlar.ConvertToInt(acilisYilStr, 0);

                string makbuzNo = "";
                string odemeBelgeNo = "";
                string faizBelgeNo = "";
                double odemeTutar = 0.0;
                double faizTutar = 0.0;
                TNSDateTime odemeTarihi = new TNSDateTime();

                ObjectArray taksitBilgisi = servisATM.TaksitListele(kullanan, yil, dosyaNo);
                if (taksitBilgisi != null && taksitBilgisi.objeler != null)
                {
                    foreach (DosyaTaksit taksit in taksitBilgisi.objeler)
                    {
                        if (taksit.siraNo != istek.siraNo) continue;

                        odemeTutar = taksit.miktar;
                        faizTutar = taksit.FaizToplam;
                        odemeTarihi = taksit.odemeTarihi;
                        makbuzNo = taksit.makbuzNo;

                        BelgeCoz(taksit.odemeEmriNo, out string _, out odemeBelgeNo);
                        BelgeCoz(taksit.faizOdemeEmriNo, out string _, out faizBelgeNo);
                    }
                }

                if (odemeTarihi == null || odemeTarihi.isNull)
                    return Basarisiz("İlk önce ödeme yapılan tarihi bilgisini kayıt edin.");

                IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
                string siraNo = istek.siraNo.ToString();

                if (faizTutar > 0)
                {
                    faizBelgeNo = FaizTahakkuku(kullanan, servisATM, servisMUH, bilgi, yil, dosyaNo, siraNo, odemeTarihi, faizTutar, odemeTarihi.Yil, faizBelgeNo,
                        Temiz(istek.borcHesap), Temiz(istek.alacakHesap));
                    if (faizBelgeNo.StartsWith("HATA"))
                        return Basarisiz(HataMetni(faizBelgeNo.Substring(5), "Faiz tahakkuku oluşturulamadı."));
                }

                odemeBelgeNo = OdemeTahakkuku(kullanan, servisATM, servisMUH, bilgi, yil, dosyaNo, siraNo, odemeTarihi, odemeTutar, faizTutar, odemeTarihi.Yil,
                    odemeBelgeNo, faizBelgeNo, acilisYil, acilisNo, odemeHesapKodu, out string satirlar);
                if (odemeBelgeNo.StartsWith("HATA"))
                    return Results.Json(new
                    {
                        basarili = false,
                        hata = HataMetni(odemeBelgeNo.Substring(5), "Ödeme tahakkuku oluşturulamadı."),
                        satirlar = satirlar
                    });

                return Results.Json(new
                {
                    basarili = true,
                    bilgi = "Tahakkuk oluşturuldu.",
                    ODEMEEMRIYIL = odemeTarihi.Yil.ToString(),
                    FAIZODEMEEMRINO = faizBelgeNo,
                    ODEMEEMRINO = odemeBelgeNo
                });
            }
            catch (Exception)
            {
                return Basarisiz("Tahakkuk oluşturulamadı.");
            }
        }

        /// <summary>Eski btnOdemeEmriOlusturOdeme. Hata olursa "HATA:..." döner; satirlar o zaman denenen belge satırlarıdır.</summary>
        private static string OdemeTahakkuku(Kullanici kullanan, IATMServis servisATM, IMUHServis servisMUH, ObjectArray bilgi, int dosyaYil, int dosyaNo,
            string siraNo, TNSDateTime odemeTarihi, double odemeTutar, double faizTutar, int belgeYil, string odemeBelgeNo, string faizBelgeNo,
            int acilisYil, string acilisNo, string odemeHesapKodu, out string satirlar)
        {
            satirlar = "";
            string acilisTahakkukHesapKod = "";
            string acilisTahkkukYansitmaHesapKod = "";

            string faizTahakkukHesapKod = "";
            string faizTahkkukYansitmaHesapKod = "";

            //Açılış tahakkukunun 1. satırını bul
            //açılış takkukununb 2. satırının yansıtmasını bul
            OdemeEmrindenHesapKoduBul(kullanan, servisMUH, acilisYil, acilisNo, ref acilisTahakkukHesapKod, ref acilisTahkkukYansitmaHesapKod);

            //Faiz tahakkukunun 1. satırını bul
            //Faiz takkukununb 2. satırının yansıtmasını bul
            if (faizBelgeNo != "")
                OdemeEmrindenHesapKoduBul(kullanan, servisMUH, belgeYil, faizBelgeNo, ref faizTahakkukHesapKod, ref faizTahkkukYansitmaHesapKod);

            OdemeEmriMIFForm oe = new OdemeEmriMIFForm();
            foreach (Dosya d in bilgi.objeler)
            {
                oe.yil = belgeYil;
                oe.muhasebe = d.saymanlikKodu;
                oe.birim = d.birimKodu;
                oe.islemTarih = odemeTarihi;
                oe.ilgiliAd = d.borcluAd;
                oe.ilgiliNo = d.kimlikNo;
                oe.kurum = TNS.MUH.Arac.VarsayilanKurumKod();
                oe.aciklama = dosyaYil + "/" + dosyaNo + " numaralı alacak dosyası kapsamında oluşturulmuştur (Ödeme Tahakkuku)";
                oe.belgeNo = odemeBelgeNo;
            }

            ObjectArray detay = new ObjectArray();
            OdemeEmriMIFDetay td;

            bool yansitmaYok = false;
            //TKDK ödemede 140lı hesapların yansıtma yapmadığını söyledi. Config dosyasına yazılması gerekli
            string odemeYansitmaYapilmayacakHesaplar = Ayar.Oku("OdemeYansitmaYapilmayacakHesaplar");
            //Config dosyasına eklenen kadar bu kuralı kullanan kurum etkilenmesin. TKDK config dosyasına 999 hesap kodunu ekleyeceğim Melih 01.10.2021
            if (odemeYansitmaYapilmayacakHesaplar == "") odemeYansitmaYapilmayacakHesaplar = "140";

            string[] yy = odemeYansitmaYapilmayacakHesaplar.Split(';');
            foreach (string yh in yy)
            {
                if (string.IsNullOrWhiteSpace(yh)) continue;
                if (acilisTahakkukHesapKod.StartsWith(yh)) yansitmaYok = true;
            }

            td = new OdemeEmriMIFDetay(oe.yil);
            td.hesapKod = acilisTahakkukHesapKod;
            td.borc = 0.0;
            td.alacak = Math.Round(odemeTutar, 2);
            td.mahsupBelgeNo = MahsupBelgeNobul(servisMUH, kullanan, acilisYil, acilisNo, belgeYil);
            detay.objeler.Add(td);

            if (faizTahakkukHesapKod != "")
            {
                td = new OdemeEmriMIFDetay(oe.yil);
                td.hesapKod = faizTahakkukHesapKod;
                td.borc = 0.0;
                td.alacak = Math.Round(faizTutar, 2);
                td.mahsupBelgeNo = faizBelgeNo;
                if (faizTutar > 0)
                    detay.objeler.Add(td);
            }
            td = new OdemeEmriMIFDetay(oe.yil);
            td.hesapKod = odemeHesapKodu;
            td.borc = Math.Round(odemeTutar, 2) + Math.Round(faizTutar, 2);
            td.alacak = 0.0;
            detay.objeler.Add(td);

            double odemeTutarYansit = 0;
            if (acilisTahkkukYansitmaHesapKod != "" && !yansitmaYok)
            {
                td = new OdemeEmriMIFDetay(oe.yil);
                td.hesapKod = acilisTahkkukYansitmaHesapKod;
                td.borc = 0.0;
                td.alacak = Math.Round(odemeTutar, 2);
                odemeTutarYansit = td.alacak;
                detay.objeler.Add(td);
            }

            if (faizTahkkukYansitmaHesapKod != "")
            {
                td = new OdemeEmriMIFDetay(oe.yil);
                td.hesapKod = faizTahkkukYansitmaHesapKod;
                td.borc = 0.0;
                td.alacak = Math.Round(faizTutar, 2);
                if (faizTutar > 0)
                    detay.objeler.Add(td);
            }

            if (acilisTahkkukYansitmaHesapKod != "" || faizTahkkukYansitmaHesapKod != "")
            {
                td = new OdemeEmriMIFDetay(oe.yil);
                td.hesapKod = "805";
                td.borc = Math.Round(odemeTutarYansit, 2) + Math.Round(faizTutar, 2);
                td.alacak = 0.0;
                if (Math.Round(td.borc, 2) > 0)
                    detay.objeler.Add(td);
            }

            oe.detay = detay;

            Sonuc sonuc = servisMUH.OdemeEmriMIFKaydet(kullanan, oe);
            if (sonuc != null && sonuc.islemSonuc)
            {
                odemeBelgeNo = sonuc.anahtar;
                servisATM.OdemeEmriKaydetTaksit(dosyaYil, dosyaNo, OrtakFonksiyonlar.ConvertToInt(siraNo, 0), oe.yil, odemeBelgeNo, false);
                return odemeBelgeNo;
            }
            else
            {
                // Eski kod hatayla birlikte denenen satırları da gösteriyordu.
                string satir = "";
                foreach (OdemeEmriMIFDetay d in oe.detay.objeler)
                    satir += string.Format("{0} {1} {2}\n", d.hesapKod, d.borc, d.alacak);
                satirlar = satir.TrimEnd('\n');
                return "HATA:" + (sonuc != null ? sonuc.hataStr : "");
            }
        }

        private static string MahsupBelgeNobul(IMUHServis servisMUH, Kullanici kullanan, int dosyaOEYil, string dosyaOEBelgeNo, int belgeYil)
        {
            if (belgeYil > dosyaOEYil)
                return TNS.MUH.Arac.AcilisFisiBul(servisMUH, kullanan, belgeYil);
            else
                return dosyaOEBelgeNo;
        }

        /// <summary>Eski btnOdemeEmriOlusturFaiz. Hata olursa "HATA:..." döner.</summary>
        private static string FaizTahakkuku(Kullanici kullanan, IATMServis servisATM, IMUHServis servisMUH, ObjectArray bilgi, int dosyaYil, int dosyaNo,
            string siraNo, TNSDateTime odemeTarihi, double faizTutar, int belgeYil, string belgeNo, string borcHesap, string alacakHesap)
        {
            OdemeEmriMIFForm oe = new OdemeEmriMIFForm();

            foreach (Dosya d in bilgi.objeler)
            {
                oe.yil = belgeYil;
                oe.muhasebe = d.saymanlikKodu;
                oe.birim = d.birimKodu;
                oe.islemTarih = odemeTarihi;
                oe.ilgiliAd = d.borcluAd;
                oe.ilgiliNo = d.kimlikNo;
                oe.kurum = TNS.MUH.Arac.VarsayilanKurumKod();
                oe.aciklama = dosyaYil + "/" + dosyaNo + " numaralı alacak dosyası kapsamında oluşturulmuştur (Faiz Tahakkuku)";
                oe.belgeNo = belgeNo;
            }

            ObjectArray detay = new ObjectArray();
            OdemeEmriMIFDetay td;

            td = new OdemeEmriMIFDetay(oe.yil);
            td.hesapKod = borcHesap; //"140.01.02";
            td.borc = Math.Round(faizTutar, 2);
            td.alacak = 0.0;
            detay.objeler.Add(td);

            td = new OdemeEmriMIFDetay(oe.yil);
            td.hesapKod = alacakHesap; //"600.05.01.09.01";
            td.borc = 0.0;
            td.alacak = Math.Round(faizTutar, 2);
            detay.objeler.Add(td);

            oe.detay = detay;

            Sonuc sonuc = servisMUH.OdemeEmriMIFKaydet(kullanan, oe);
            if (sonuc != null && sonuc.islemSonuc)
            {
                belgeNo = sonuc.anahtar;
                servisATM.OdemeEmriKaydetTaksit(dosyaYil, dosyaNo, OrtakFonksiyonlar.ConvertToInt(siraNo, 0), oe.yil, belgeNo, true);
                return belgeNo;
            }
            else
                return "HATA:" + (sonuc != null ? sonuc.hataStr : "");
        }

        /// <summary>Eski OdemeEmrindenHesapKoduBul: belgenin 1. satırının hesap kodu ve 2. satırının yansıtma hesabı.</summary>
        private static void OdemeEmrindenHesapKoduBul(Kullanici kullanan, IMUHServis servisMUH, int yil, string belgeNo, ref string hesapKodu, ref string yansitmaHesapKodu)
        {
            OdemeEmriMIFForm form = new OdemeEmriMIFForm();
            form.yil = yil;
            form.belgeNo = belgeNo;
            ObjectArray oe = servisMUH.OdemeEmriMIFTekBelgeListele(kullanan, form);

            foreach (OdemeEmriMIFForm o in oe.objeler)
            {
                foreach (OdemeEmriMIFDetay od in o.detay.objeler)
                {
                    if (od.satirNo == 1) hesapKodu = od.hesapKod;
                    if (od.satirNo == 2) yansitmaHesapKodu = od.hesapKod;
                }
            }

            if (yansitmaHesapKodu != "")
            {
                HesapPlaniYansitma y = new HesapPlaniYansitma();
                y.yil = yil;
                y.hesapKod = yansitmaHesapKodu;
                ObjectArray yListe = servisMUH.YansitmaListele(y);
                foreach (HesapPlaniYansitma hy in yListe.objeler)
                {
                    yansitmaHesapKodu = hy.yansitmaHesapKod;
                }
            }
        }

        // ------------------------------------------------------------------ hesap kodu seçimi (eski HesapKod.ascx)
        // Sgb.hesapPlaniSec (makbuzsecim.js) penceresinin istediği uçlar; Makbuz uçlarının (MakbuzUclari.HesapAgac / HesapAra) aynısı,
        // yalnızca yetki bu sayfadan gelir (Makbuz Bilgileri yetkisi olmayan Alacak Takip kullanıcıları da hesap seçebilsin).

        private static int HesapYil(int? yil)
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

                string[] hesapKirilimlar = ust.Split('.');
                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.yil = y;
                hs.hesapKod = ust;
                if (ust.Length < 3) hs.seviye = 1;
                else hs.seviye = hesapKirilimlar.Length + 1;

                IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
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
                h.yil = HesapYil(yil);
                h.hesapKodHesapAd = metin;
                h.detay = sonKirilim != "0";
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
    }
}
