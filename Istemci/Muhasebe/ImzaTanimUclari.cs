using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
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
    public class ImzaTanimKaydetIstegi
    {
        public int kod { get; set; }
        public string ad { get; set; }
        public int belge { get; set; }
        public List<ImzaSatirIstegi> satirlar { get; set; }
    }

    public class ImzaTanimKodIstegi
    {
        public int kod { get; set; }
    }

    /// <summary>
    /// İmza Tanımları (eski ButceMuhasebe/ImzaTanim.aspx): kullanıcının adlandırılmış imza tanımları (belge türü + imza yerleri).
    /// Ödeme Emri / MİF formunun Yazdır menüsündeki "+" düğmesinden pencere (Sgb.pencere('ImzaTanim', ...)) olarak açılır.
    /// Uçlar /api/imzatanim/*:
    ///   liste   (GET)                                 : { satirlar: [ { kod, ad, belgeTur, belgeTurAd, kullaniciKodu } ], secili, yonetici }
    ///   detay   (GET kod, belge)                      : { kod, ad, belgeTur, satirlar: [ { deger, imzaYeri, kullanilmiyor, adSoyad, unvanRutbe, gorevUnvani, tarihYaz } ] }
    ///   kaydet  (POST { kod, ad, belge, satirlar })   : { basarili, bilgi, anahtar } / { hata }
    ///   sil     (POST { kod })                        : { basarili, bilgi } / { hata }
    ///   gecerli (POST { kod })                        : { basarili, bilgi, kod } / { hata } — formun Yazdır menüsündeki imza seçimi
    ///             (eski OdemeEmriMIF2 ddlImzaTanimYazdir_Select: ImzaTanimGecerliImzaYap(kod, "999") + MUHASEBEIMZATANIMKOD kullanıcı değişkeni).
    /// Yetki: tanım sayfası ve kaydet / sil / detay eski sayfadaki gibi yalnız sistem yöneticisi ve muhasebe yöneticisi;
    /// liste ve gecerli bunlara ek olarak Ödeme Emri / MİF menü seçeneklerini görebilen kullanıcılar.
    /// </summary>
    public static class ImzaTanimUclari
    {
        public const string Kok = "/api/imzatanim/";
        public const string SayfaAdresi = "../ButceMuhasebe/ImzaTanim.aspx";
        public const string SeciliDegisken = "MUHASEBEIMZATANIMKOD";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Kok + "liste", Liste);
            app.MapGet(Kok + "detay", Detay);
            app.MapPost(Kok + "kaydet", Kaydet);
            app.MapPost(Kok + "sil", Sil);
            app.MapPost(Kok + "gecerli", Gecerli);
        }

        /// <summary>Tanım sayfası ve tanım değiştiren uçlar: eski sayfadaki gibi sistem yöneticisi ya da muhasebe yöneticisi.</summary>
        public static bool TanimYetkisi(Kullanici kullanan)
        {
            return MuhasebeUclari.TanimYetkisiVar(kullanan);
        }

        /// <summary>Liste ve geçerli imza seçimi: yöneticiler ya da Ödeme Emri / MİF ekranını kullanabilenler.</summary>
        public static bool KullanimYetkisi(Kullanici kullanan, string klasor)
        {
            return TanimYetkisi(kullanan) || SayfaYetki.Yetkili(kullanan, klasor, SayfaAdresi, TanimAciklamaUclari.UstSayfalar);
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        /// <summary>Belge türünün adı (eski BelgeTurAdGetir / BelgeTurDoldur).</summary>
        public static string BelgeTurAd(ENUMBelgeTuru belgeTur)
        {
            switch (belgeTur)
            {
                case ENUMBelgeTuru.ODEMEEMRI: return "Ödeme Emri";
                case ENUMBelgeTuru.MIF: return "Muhasebe İşlem Fişi";
                case ENUMBelgeTuru.ODENEKGONDERME: return "Ödenek Gönderme";
                case ENUMBelgeTuru.TENKIS: return "Tenkis";
                case ENUMBelgeTuru.ODEMEEMRI_YUKSEK: return "Ödeme Emri (Yüksek)";
                case ENUMBelgeTuru.MIF_YUKSEK: return "Muhasebe İşlem Fişi (Yüksek)";
                case ENUMBelgeTuru.ODENEKGONDERME_YUKSEK: return "Ödenek Gönderme (Yüksek)";
                case ENUMBelgeTuru.TENKIS_YUKSEK: return "Tenkis (Yüksek)";
                default: return "";
            }
        }

        // Belge türüne göre imza yerleri (eski ListeleDetay): (değer, imza yeri), satır sırasıyla.
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
        /// İmza tanımları (eski Listele / OdemeEmriMIF2 StoreImzaTanim_Refresh). secili: kullanıcının en son seçtiği tanım
        /// (MUHASEBEIMZATANIMKOD; yoksa 0). yonetici: tanım sayfasını açabilir mi (formdaki "+" düğmesi için).
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            return Oku(baglam, k => KullanimYetkisi(k, ortam.ContentRootPath), kullanan =>
            {
                List<object> satirlar = new List<object>();
                ImzaTanim[] liste = Servis().ImzaTanimListele(kullanan, new ImzaTanim());
                if (liste != null)
                    foreach (ImzaTanim t in liste)
                        satirlar.Add(new
                        {
                            kod = t.kod,
                            ad = t.ad ?? "",
                            belgeTur = (int)t.belgeTur,
                            belgeTurAd = BelgeTurAd(t.belgeTur),
                            kullaniciKodu = t.kullaniciKodu ?? ""
                        });
                int secili = OrtakFonksiyonlar.ConvertToInt(GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, SeciliDegisken), 0);
                return Results.Json(new { satirlar, secili, yonetici = TanimYetkisi(kullanan) });
            }, "İmza tanımları okunamadı.");
        }

        /// <summary>
        /// Eski ListeleDetay(imzaTanimKod): kod verilmişse tanım okunur (adı ve belge türü onun), değilse verilen belge türünün
        /// boş imza yerleri döner. Kayıtlı imzalar sırayla okunur; her satır kendi imza yerine, imza yeri boşsa sıradaki yere yazılır.
        /// </summary>
        private static IResult Detay(HttpContext baglam, int? kod, int? belge)
        {
            return Oku(baglam, TanimYetkisi, kullanan =>
            {
                ImzaTanim form = new ImzaTanim();
                form.detaylar = new ImzaTanimDetay[0];
                ENUMBelgeTuru secBelge = (ENUMBelgeTuru)(belge ?? 0);

                if ((kod ?? 0) > 0)
                {
                    form = Servis().ImzaTanimOku(kullanan, new ImzaTanim() { kod = kod.Value });
                    if (form == null) return Hata("İmza tanımı bulunamadı.");
                    secBelge = form.belgeTur;
                }

                ImzaTanimDetay[] detaylar = form.detaylar ?? new ImzaTanimDetay[0];
                (string deger, string ad)[] yerler = ImzaYerleri(secBelge);
                List<object> satirlar = new List<object>();
                for (int satir = 0; satir < 7; satir++)
                {
                    ImzaBilgisi imza = satir < detaylar.Length && detaylar[satir] != null ? detaylar[satir] : new ImzaBilgisi();
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
                return Results.Json(new { kod = form.kod, ad = form.ad ?? "", belgeTur = (int)secBelge, satirlar });
            }, "İmza bilgileri okunamadı.");
        }

        /// <summary>Eski btnKaydet_Click: kod 0 ise yeni tanım; kullanıcı kodu oturumdaki kullanıcı.</summary>
        private static Task<IResult> Kaydet(HttpContext baglam, IAntiforgery sahtecilik, ImzaTanimKaydetIstegi istek)
        {
            return Yaz(baglam, sahtecilik, TanimYetkisi, kullanan =>
            {
                if (istek == null || istek.belge <= 0) return Hata("Lütfen işlem yapacağınız belgeyi seçin.");

                List<ImzaTanimDetay> detayList = new List<ImzaTanimDetay>();
                foreach (ImzaSatirIstegi s in istek.satirlar ?? new List<ImzaSatirIstegi>())
                {
                    if (s == null) continue;
                    ImzaTanimDetay detay = new ImzaTanimDetay();
                    detay.imzaYer = s.deger ?? "";
                    detay.adSoyad = s.adSoyad ?? "";
                    detay.unvan = s.unvanRutbe ?? "";
                    detay.kullanilmiyor = s.kullanilmiyor ? 1 : 0;
                    detay.gorevUnvan = s.gorevUnvani ?? "";
                    detay.tarih = s.tarihYaz ? 1 : 0;
                    detayList.Add(detay);
                }

                ImzaTanim form = new ImzaTanim();
                form.kod = istek.kod;
                form.ad = istek.ad ?? "";
                form.kullaniciKodu = kullanan.kullaniciKodu;
                form.belgeTur = (ENUMBelgeTuru)istek.belge;
                form.detaylar = detayList.ToArray();

                return SonucYaniti(Servis().ImzaTanimKaydet(kullanan, form), "Kayıt işlemi tamamlandı.");
            });
        }

        private static Task<IResult> Sil(HttpContext baglam, IAntiforgery sahtecilik, ImzaTanimKodIstegi istek)
        {
            return Yaz(baglam, sahtecilik, TanimYetkisi, kullanan =>
            {
                if (istek == null || istek.kod <= 0) return Hata("Lütfen silmek istediğiniz imza tanımını listeden seçin.");
                return SonucYaniti(Servis().ImzaTanimSil(kullanan, new ImzaTanim() { kod = istek.kod }), "Seçilen imza bilgileri silindi.");
            });
        }

        /// <summary>
        /// Formun Yazdır menüsünde imza tanımı seçimi (eski ddlImzaTanimYazdir_Select'in yazdırma dışındaki kısmı):
        /// tanım "999" birim koduyla geçerli imza yapılır ve kullanıcının MUHASEBEIMZATANIMKOD değişkenine yazılır.
        /// </summary>
        private static Task<IResult> Gecerli(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, ImzaTanimKodIstegi istek)
        {
            return Yaz(baglam, sahtecilik, k => KullanimYetkisi(k, ortam.ContentRootPath), kullanan =>
            {
                int kod = istek == null ? 0 : istek.kod;
                Sonuc sonuc = Servis().ImzaTanimGecerliImzaYap(kullanan, kod, "999");
                if (sonuc == null || !sonuc.islemSonuc)
                    return Hata(HataMetni(sonuc?.hataStr, "İmza tanımı geçerli yapılamadı."));
                GenelIslemler.KullaniciDegiskenSakla(kullanan, SeciliDegisken, kod + "");
                return Results.Json(new { basarili = true, bilgi = "İmza tanımı geçerli yapıldı.", kod });
            });
        }
    }
}
