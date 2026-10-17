using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;
using static Istemci.Muhasebe.MuhTanimOrtak;

namespace Istemci.Muhasebe
{
    public class KBSSatirIstegi
    {
        public string HESAPKOD { get; set; }
        public string HESAPADI { get; set; }
        public int GOSTER { get; set; }
        public int HESAPUZUNLUK { get; set; }
        public int FONUZUNLUK { get; set; }
        public int FINUZUNLUK { get; set; }
    }

    public class KBSIstegi
    {
        public int yil { get; set; }
        public List<KBSSatirIstegi> satirlar { get; set; }
    }

    public class ImzaSatirIstegi
    {
        public string deger { get; set; }
        public bool kullanilmiyor { get; set; }
        public string adSoyad { get; set; }
        public string unvanRutbe { get; set; }
        public string gorevUnvani { get; set; }
        public bool tarihYaz { get; set; }
    }

    public class ImzaIstegi
    {
        public int belge { get; set; }
        public string birim { get; set; }
        public List<ImzaSatirIstegi> satirlar { get; set; }
    }

    public class IstatistikDegerIstegi
    {
        public int degerKod { get; set; }
        public string aciklama { get; set; }
        public string ekAlan1 { get; set; }
        public string ekAlan2 { get; set; }
        public string ekAlan3 { get; set; }
    }

    public class IstatistikIstegi
    {
        public int kod { get; set; }
        public string ad { get; set; }
        public bool oeZorunlu { get; set; }
        public bool mifZorunlu { get; set; }
        public string ekAlan1Ad { get; set; }
        public string ekAlan2Ad { get; set; }
        public string ekAlan3Ad { get; set; }
        public List<IstatistikDegerIstegi> degerler { get; set; }
    }

    /// <summary>
    /// Düzenlenebilir tablolu Muhasebe Tanımları sayfalarının uçları:
    ///   kbs        : KBS Aktarım Tanımlama (eski TanimKBSAktarim.aspx) ve Detay İşlem penceresi
    ///   imza       : İmza Bilgileri Tanımlama (eski Imza.aspx)
    ///   istatistik : İstatistik Kod Girişi (eski IstatistikKodGirisForm.aspx)
    /// </summary>
    public static class MuhTanimDigerUclari
    {
        public const string KBSAdresi = "../ButceMuhasebe/TanimKBSAktarim.aspx";
        public const string ImzaAdresi = "../ButceMuhasebe/Imza.aspx";
        public const string IstatistikAdresi = "../ButceMuhasebe/IstatistikKodGirisForm.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Kok + "kbs/liste", KBSListe);
            app.MapPost(Kok + "kbs/kaydet", KBSKaydet);
            app.MapGet(Kok + "kbs/detay", KBSDetayListe);
            app.MapPost(Kok + "kbs/detay-kaydet", KBSDetayKaydet);

            app.MapGet(Kok + "imza/liste", ImzaListe);
            app.MapPost(Kok + "imza/kaydet", ImzaKaydet);

            app.MapGet(Kok + "istatistik/kodlar", IstatistikKodlar);
            app.MapGet(Kok + "istatistik/oku", IstatistikOku);
            app.MapPost(Kok + "istatistik/kaydet", IstatistikKaydet);
            app.MapPost(Kok + "istatistik/kod-kaydet", IstatistikKodKaydet);
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        // ------------------------------------------------------------------ KBS aktarım

        private static bool KBSYetkili(Kullanici k, string klasor) { return Yetkili(k, klasor, KBSAdresi); }

        private static IResult KBSListe(HttpContext baglam, IWebHostEnvironment ortam, int? yil)
        {
            return Oku(baglam, k => KBSYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                ObjectArray yler = Servis().KBSAktarimListele(kullanan, yil ?? DateTime.Now.Year);
                if (yler == null || !yler.sonuc.islemSonuc) return Hata(HataMetni(yler?.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (KBSAktarim y in yler.objeler)
                    satirlar.Add(new
                    {
                        KOD = y.kod ?? "",
                        YIL = y.yil,
                        HESAPKOD = y.hesapKod ?? "",
                        HESAPADI = (y.hesapAd ?? "").Replace("'", ""),
                        GOSTER = y.goster,
                        HESAPUZUNLUK = y.hesapUzunluk,
                        FONUZUNLUK = y.fonUzunluk,
                        FINUZUNLUK = y.finUzunluk
                    });
                return Results.Json(new { satirlar });
            });
        }

        private static Task<IResult> KBSKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, KBSIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => KBSYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || istek.yil <= 0) return Hata("Yıl bilgisi boş bırakılmış.");
                ObjectArray bilgi = new ObjectArray();
                foreach (KBSSatirIstegi r in istek.satirlar ?? new List<KBSSatirIstegi>())
                    bilgi.objeler.Add(new KBSAktarim
                    {
                        hesapKod = r.HESAPKOD ?? "",
                        hesapAd = r.HESAPADI ?? "",
                        goster = r.GOSTER,
                        hesapUzunluk = r.HESAPUZUNLUK,
                        fonUzunluk = r.FONUZUNLUK,
                        finUzunluk = r.FINUZUNLUK
                    });
                Sonuc s = Servis().KBSAktarimKaydet(kullanan, bilgi, istek.yil);
                return s.islemSonuc ? Tamam("Bilgiler başarıyla kayıt edildi") : Hata(HataMetni(s.hataStr, "Kayıt işlemi yapılamadı."));
            });
        }

        /// <summary>Detay İşlem penceresi (eski ListeleDetay): kayıtlı hesap kodları ve 50 boş satır.</summary>
        private static IResult KBSDetayListe(HttpContext baglam, IWebHostEnvironment ortam, int? yil)
        {
            return Oku(baglam, k => KBSYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                ObjectArray yler = Servis().KBSAktarimDetayListele(kullanan, yil ?? DateTime.Now.Year);
                if (yler == null || !yler.sonuc.islemSonuc) return Hata(HataMetni(yler?.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (KBSAktarim y in yler.objeler) satirlar.Add(new { HESAPKOD = y.hesapKod ?? "" });
                for (int i = 0; i < 50; i++) satirlar.Add(new { HESAPKOD = "" });
                return Results.Json(new { satirlar });
            });
        }

        private static Task<IResult> KBSDetayKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, KBSIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => KBSYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || istek.yil <= 0) return Hata("Yıl bilgisi boş bırakılmış.");
                ObjectArray bilgi = new ObjectArray();
                foreach (KBSSatirIstegi r in istek.satirlar ?? new List<KBSSatirIstegi>())
                {
                    string hesap = (r.HESAPKOD ?? "").Replace(".", "");
                    if (hesap.Trim() != "") bilgi.objeler.Add(new KBSAktarim { hesapKod = hesap });
                }
                Sonuc s = Servis().KBSAktarimDetayKaydet(kullanan, bilgi, istek.yil);
                return s.islemSonuc ? Tamam("Bilgiler başarıyla kayıt edildi") : Hata(HataMetni(s.hataStr, "Kayıt işlemi yapılamadı."));
            });
        }

        // ------------------------------------------------------------------ imza

        private static bool ImzaYetkili(Kullanici k, string klasor) { return Yetkili(k, klasor, ImzaAdresi, false); }

        // Belge türüne göre imza yerleri (eski ddlBelge_SelectedIndexChanged): (değer, imza yeri).
        private static readonly (string deger, string ad)[] OEYerleri =
            { ("1", "Gerçekleştirme Görevlisi"), ("3", "Harcama Yetkilisi"), ("4", "Muhasebe Yetkilisi"), ("5", "Düzenleyen"), ("6", "Kontrol Eden") };
        private static readonly (string deger, string ad)[] MIFYerleri =
            { ("1", "Gerçekleştirme Görevlisi"), ("3", "Harcama Yetkilisi"), ("4", "Muhasebe Yetkilisi"), ("5", "Veznedar"), ("6", "Düzenleyen"),
              ("7", "Kontrol Eden"), ("2", "Mali Hizmetler Birimi Yöneticisi") };
        private static readonly (string deger, string ad)[] OGYerleri =
            { ("1", "Mali Hizmetler Birimi Yöneticisi"), ("2", "Harcama Yetkilisi") };

        private static (string deger, string ad)[] ImzaYerleri(ENUMBelgeTuru b)
        {
            switch (b)
            {
                case ENUMBelgeTuru.ODEMEEMRI:
                case ENUMBelgeTuru.ODEMEEMRI_YUKSEK: return OEYerleri;
                case ENUMBelgeTuru.MIF:
                case ENUMBelgeTuru.MIF_YUKSEK: return MIFYerleri;
                case ENUMBelgeTuru.ODENEKGONDERME:
                case ENUMBelgeTuru.ODENEKGONDERME_YUKSEK:
                case ENUMBelgeTuru.TENKIS:
                case ENUMBelgeTuru.TENKIS_YUKSEK: return OGYerleri;
                default: return new (string, string)[0];
            }
        }

        /// <summary>
        /// Listele: eski sayfanın 7 satırlık eşleştirmesinin aynısı. Kayıtlı imzalar sırayla okunur; her satır kendi imza yerine,
        /// imza yeri boşsa sıradaki yere yazılır.
        /// </summary>
        private static IResult ImzaListe(HttpContext baglam, IWebHostEnvironment ortam, int? belge, string birim)
        {
            return Oku(baglam, k => ImzaYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if ((belge ?? 0) <= 0) return Hata("Lütfen işlem yapacağınız belgeyi seçin.");
                ENUMBelgeTuru secBelge = (ENUMBelgeTuru)belge.Value;
                ObjectArray iV = Servis().ImzaListele(kullanan, secBelge, 0, Temiz(birim));
                if (iV == null || !iV.sonuc.islemSonuc) return Hata(HataMetni(iV?.sonuc.hataStr, "Liste okunamadı."));
                (string deger, string ad)[] yerler = ImzaYerleri(secBelge);
                List<object> satirlar = new List<object>();
                for (int satir = 0; satir < 7; satir++)
                {
                    ImzaBilgisi imza = satir < iV.objeler.Count ? (ImzaBilgisi)iV.objeler[satir] : new ImzaBilgisi();
                    string yer = imza.imzaYer ?? "";
                    for (int i = 0; i < yerler.Length; i++)
                    {
                        if ((yer == "" && satir == i) || yer == yerler[i].deger)
                        {
                            satirlar.Add(new
                            {
                                deger = yerler[i].deger,
                                imzaYeri = yerler[i].ad,
                                kullanilmiyor = imza.kullanilmiyor == 1,
                                adSoyad = imza.adSoyad ?? "",
                                unvanRutbe = imza.unvan ?? "",
                                gorevUnvani = imza.gorevUnvan ?? "",
                                tarihYaz = imza.tarih == 1
                            });
                            break;
                        }
                    }
                }
                return Results.Json(new { satirlar });
            });
        }

        private static Task<IResult> ImzaKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, ImzaIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => ImzaYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || istek.belge <= 0) return Hata("Lütfen işlem yapacağınız belgeyi seçin.");
                ObjectArray imzalar = new ObjectArray();
                foreach (ImzaSatirIstegi item in istek.satirlar ?? new List<ImzaSatirIstegi>())
                    imzalar.objeler.Add(new ImzaBilgisi
                    {
                        kullaniciKodu = kullanan.kullaniciKodu,
                        belgeTur = (ENUMBelgeTuru)istek.belge,
                        muhasebe = 0,
                        birim = Temiz(istek.birim),
                        imzaYer = item.deger ?? "",
                        adSoyad = item.adSoyad ?? "",
                        unvan = item.unvanRutbe ?? "",
                        kullanilmiyor = item.kullanilmiyor ? 1 : 0,
                        gorevUnvan = item.gorevUnvani ?? "",
                        tarih = item.tarihYaz ? 1 : 0
                    });
                return SonucYaniti(Servis().ImzaKaydet(kullanan, imzalar), "Kayıt işlemi başarıyla tamamlandı.");
            });
        }

        // ------------------------------------------------------------------ istatistik kodları

        private static bool IstatistikYetkili(Kullanici k, string klasor) { return Yetkili(k, klasor, IstatistikAdresi, false); }

        private static object KodSatir(IstatistikKod t)
        {
            return new
            {
                kod = t.kod,
                ad = t.ad ?? "",
                zorunlu = t.zorunlu,
                oeZorunlu = t.zorunlu == 1 || t.zorunlu == 11,
                mifZorunlu = t.zorunlu == 10 || t.zorunlu == 11,
                ekAlan1Ad = t.ekAlan1Ad ?? "",
                ekAlan2Ad = t.ekAlan2Ad ?? "",
                ekAlan3Ad = t.ekAlan3Ad ?? ""
            };
        }

        private static IResult IstatistikKodlar(HttpContext baglam, IWebHostEnvironment ortam)
        {
            return Oku(baglam, k => IstatistikYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                ObjectArray oa = Servis().ListeleIstatistikTanim(kullanan, new IstatistikKod(), false, false, false);
                if (oa == null || !oa.sonuc.islemSonuc) return Hata(HataMetni(oa?.sonuc.hataStr, "Liste okunamadı."));
                List<object> kodlar = new List<object>();
                foreach (IstatistikKod t in oa.objeler) kodlar.Add(KodSatir(t));
                return Results.Json(new { kodlar });
            });
        }

        private static IResult IstatistikOku(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            return Oku(baglam, k => IstatistikYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                IstatistikKod form = Servis().OkuIstatistikKodGiris(kullanan, new IstatistikKod { kod = kod ?? 0 }, false, false);
                if (form == null) return Hata("Verilen kriterlere uygun kayıt bulunamadı.");
                List<object> degerler = new List<object>();
                foreach (IstatistikKodDeger d in form.istatistikKodDeger)
                    degerler.Add(new { degerKod = d.degerKod, aciklama = d.aciklama ?? "", ekAlan1 = d.ekAlan1 ?? "", ekAlan2 = d.ekAlan2 ?? "", ekAlan3 = d.ekAlan3 ?? "" });
                return Results.Json(new { kod = KodSatir(form), degerler });
            });
        }

        private static Task<IResult> IstatistikKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, IstatistikIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => IstatistikYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null || istek.kod <= 0) return Hata("Anahtar alanları girmelisiniz.");
                IstatistikKod form = new IstatistikKod();
                form.kod = istek.kod;
                foreach (IstatistikDegerIstegi d in istek.degerler ?? new List<IstatistikDegerIstegi>())
                    form.istatistikKodDeger.Add(new IstatistikKodDeger(d.degerKod, d.aciklama ?? "", d.ekAlan1 ?? "", d.ekAlan2 ?? "", d.ekAlan3 ?? ""));
                Sonuc s = Servis().KaydetIstatistikKodGiris(kullanan, form);
                return s.islemSonuc ? Tamam("Kayıt işlemi başarıyla gerçekleşti.") : Hata(HataMetni(s.hataStr, "Kayıt işlemi yapılamadı."));
            });
        }

        /// <summary>İstatistik kodu tanımı (eski btnKodKaydet_Click): yeni ya da var olan kodun adı, zorunluluğu, ek alan adları.</summary>
        private static Task<IResult> IstatistikKodKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, IstatistikIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => IstatistikYetkili(k, ortam.ContentRootPath), kullanan =>
            {
                if (istek == null) return Hata("İstek okunamadı.");
                if (Temiz(istek.ad) == "") return Hata("Ad bilgisi boş bırakılmış.");
                IstatistikKod form = new IstatistikKod();
                form.kod = istek.kod;
                form.ad = Temiz(istek.ad);
                form.ZorunlulukAta(istek.oeZorunlu, istek.mifZorunlu);
                form.ekAlan1Ad = Temiz(istek.ekAlan1Ad);
                form.ekAlan2Ad = Temiz(istek.ekAlan2Ad);
                form.ekAlan3Ad = Temiz(istek.ekAlan3Ad);
                Sonuc s = Servis().KaydetIstatistikKodTanim(kullanan, form);
                if (!s.islemSonuc) return Hata(HataMetni(s.hataStr, "Kayıt işlemi yapılamadı."));
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti.", anahtar = s.anahtar ?? "" });
            });
        }
    }
}
