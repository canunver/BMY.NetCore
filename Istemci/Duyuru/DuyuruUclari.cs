using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;

namespace Istemci.Duyuru
{
    /// <summary>Duyuru Girişi sayfasının kaydet isteği.</summary>
    public class DuyuruKayitIstegi
    {
        public int Kod { get; set; }
        /// <summary>yyyy-MM-dd</summary>
        public string Tarih { get; set; }
        public string Konu { get; set; }
        public int Onem { get; set; }
        public int[] KullaniciTipleri { get; set; }
        public bool Gizli { get; set; }
        public bool Pencerede { get; set; }
        /// <summary>Duyuru metni (HTML).</summary>
        public string Metin { get; set; }
    }

    public class DuyuruSilIstegi
    {
        public int Kod { get; set; }
    }

    /// <summary>
    /// Duyuru sayfalarının JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/duyuru/goster?penceremi=1 : açılış sayfasındaki duyuru ekranı (eski Duyuru/Duyuru.aspx)
    ///   GET  /api/duyuru/tanimlar           : Duyuru Girişi sayfasının seçenekleri (önem, kullanıcı grupları)
    ///   GET  /api/duyuru/liste?...          : Duyuru Girişi sayfasındaki arama (eski DuyuruGiris.aspx Listele)
    ///   GET  /api/duyuru/{kod}              : tek duyuru
    ///   POST /api/duyuru/kaydet, /sil
    /// Duyuru sınıfının alanları (public field) JSON'a kendiliğinden yazılmadığı için yanıtlar burada açıkça kurulur.
    /// </summary>
    public static class DuyuruUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/duyuru/goster", Goster);
            app.MapGet("/api/duyuru/tanimlar", Tanimlar);
            app.MapGet("/api/duyuru/liste", Liste);
            app.MapGet("/api/duyuru/{kod:int}", Oku);
            app.MapPost("/api/duyuru/kaydet", Kaydet);
            app.MapPost("/api/duyuru/sil", Sil);
        }

        /// <summary>Duyuru Girişi sayfasını yalnızca sistem yöneticisi ve destek başvuru yöneticisi kullanır (eski DuyuruGiris.aspx).</summary>
        public static bool GirisYetkisiVar(Kullanici kullanan)
        {
            return kullanan != null &&
                (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                 kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI));
        }

        private static bool DestekYoneticisi(Kullanici kullanan)
        {
            return Yetki.DestekYoneticisiMi(kullanan).islemSonuc;
        }

        // ------------------------------------------------------------------ duyuru ekranı

        /// <summary>
        /// Kullanıcının tiplerine uygun, açık (gizli olmayan) duyurular; diğer modüllerin duyuruları da eklenir.
        /// penceremi=1 ise yalnızca "pencerede göster" işaretli olanlar. penceredeVar: açılışta pencere açılmalı mı.
        /// </summary>
        private static IResult Goster(HttpContext baglam, string penceremi)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();

            TNS.KYM.Duyuru arama = new TNS.KYM.Duyuru();
            arama.gizle = ENUMGizle.GORUNUR;
            arama.pencerede = string.IsNullOrWhiteSpace(penceremi) ? ENUMPencerede.TANIMSIZ : ENUMPencerede.GORUNUR;

            List<string> tipler = new List<string>();
            if (kullanan.kullaniciTipi != null)
                foreach (ENUMKullaniciTipi kt in kullanan.kullaniciTipi)
                    tipler.Add(((int)kt).ToString());
            arama.KullaniciTipleriOl(string.Join(";", tipler));

            ObjectArray<TNS.KYM.Duyuru> sonuc = TNS.KYM.Arac.Tanimla().DUYListesi(kullanan, arama, true);
            if (!sonuc.sonuc.islemSonuc)
                return Results.Json(new { hata = Metin.Duyuru("IslemHata") });

            bool penceredeVar = false;
            List<object> duyurular = new List<object>();
            foreach (TNS.KYM.Duyuru d in sonuc.objeler)
            {
                if (d.pencerede == ENUMPencerede.GORUNUR) penceredeVar = true;
                duyurular.Add(new
                {
                    kod = d.kod,
                    onem = d.onem,
                    onemAd = OnemAd(d.onem),
                    tarih = d.tarih.ToString(),
                    konu = d.konu,
                    // Duyuru metni yönetici tarafından HTML olarak girilir ve eskisi gibi biçimiyle gösterilir.
                    metin = d.duyuru
                });
            }

            return Results.Json(new
            {
                duyurular = duyurular,
                penceredeVar = penceredeVar,
                bos = Metin.Duyuru("FRMDYU001"),
                pencereBaslik = Metin.Duyuru("FRMDYG014"),
                kapat = Metin.Duyuru("Kapat")
            });
        }

        private static string OnemAd(int onem)
        {
            switch (onem)
            {
                case 1: return Metin.Duyuru("FRMDYU002");
                case 2: return Metin.Duyuru("FRMDYU003");
                case 3: return Metin.Duyuru("FRMDYU004");
                default: return "";
            }
        }

        // ------------------------------------------------------------------ Duyuru Girişi

        private static IResult Tanimlar(HttpContext baglam)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!GirisYetkisiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            bool destek = DestekYoneticisi(kullanan);
            List<object> tipler = new List<object>();
            // Destek yöneticisi yalnızca destek kullanıcı tiplerine duyuru yapabilir; "Bütün Kullanıcılar" seçeneği ona çıkmaz.
            if (!destek)
                tipler.Add(new { kod = 0, ad = Metin.Duyuru("FRMDYG013") });
            foreach (ENUMKullaniciTipi kt in TNS.KYM.Arac.KTListesi(kullanan))
            {
                int kod = TNS.KYM.Arac.KullaniciTipiKodu(kt);
                // 0 (MISAFIR) yukarıdaki "Bütün Kullanıcılar" seçeneğidir.
                if (kt == ENUMKullaniciTipi.TANIMSIZ || kod == 0) continue;
                if (destek && !TNS.KYM.Arac.DestekKullaniciTipindenMi(kod)) continue;
                tipler.Add(new { kod = kod, ad = TNS.KYM.Arac.KullaniciTipiAdi(kt) ?? kt.ToString() });
            }

            return Results.Json(new
            {
                onemler = new[]
                {
                    new { kod = 3, ad = Metin.Duyuru("FRMDYG005") },
                    new { kod = 2, ad = Metin.Duyuru("FRMDYG004") },
                    new { kod = 1, ad = Metin.Duyuru("FRMDYG003") }
                },
                kullaniciTipleri = tipler,
                destekYoneticisi = destek
            });
        }

        /// <summary>gizle / pencerede: 0 = hepsi, 1 = görünür (açık / pencerede), 2 = görünmez (gizli / pencere dışı).</summary>
        private static IResult Liste(HttpContext baglam, string konu, string basla, string bitis, int? onem, int? gizle, int? pencerede)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!GirisYetkisiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            TNS.KYM.Duyuru arama = new TNS.KYM.Duyuru();
            arama.konu = konu ?? "";
            arama.baslaTarih = TarihOku(basla);
            arama.bitisTarih = TarihOku(bitis);
            arama.onem = onem ?? 0;
            arama.gizle = Enum.IsDefined(typeof(ENUMGizle), gizle ?? 0) ? (ENUMGizle)(gizle ?? 0) : ENUMGizle.TANIMSIZ;
            arama.pencerede = Enum.IsDefined(typeof(ENUMPencerede), pencerede ?? 0) ? (ENUMPencerede)(pencerede ?? 0) : ENUMPencerede.TANIMSIZ;

            ObjectArray<TNS.KYM.Duyuru> sonuc = TNS.KYM.Arac.Tanimla().DUYListesi(kullanan, arama, false);
            if (!sonuc.sonuc.islemSonuc)
                return Results.Json(new { hata = Metin.Duyuru("IslemHata") });

            List<object> satirlar = new List<object>();
            foreach (TNS.KYM.Duyuru d in sonuc.objeler)
            {
                // Görev yönetimi gibi kaynaklardan eklenen kayıtların kodu yoktur; düzenlenemezler.
                if (d.kod <= 0) continue;
                satirlar.Add(new
                {
                    kod = d.kod,
                    tarih = TarihYaz(d.tarih),
                    onem = d.onem,
                    onemAd = OnemAd(d.onem),
                    konu = d.konu,
                    gizli = d.gizle == ENUMGizle.GORUNMEZ,
                    pencerede = d.pencerede == ENUMPencerede.GORUNUR,
                    ozet = d.duyuruTemiz
                });
            }
            return Results.Json(new { satirlar = satirlar });
        }

        private static IResult Oku(HttpContext baglam, int kod)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!GirisYetkisiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            TNS.KYM.Duyuru d;
            try
            {
                d = TNS.KYM.Arac.Tanimla().DUYOku(kod);
            }
            catch (Exception)
            {
                return Results.NotFound();
            }
            if (d == null || d.kod <= 0) return Results.NotFound();

            bool destek = DestekYoneticisi(kullanan);
            List<int> tipler = new List<int>();
            if (d.kullaniciTipleri != null)
                foreach (ENUMKullaniciTipi kt in d.kullaniciTipleri)
                {
                    int ktKod = (int)kt;
                    // Destek yöneticisi destek dışı tipleri görmez (eski Goster).
                    if (destek && !TNS.KYM.Arac.DestekKullaniciTipindenMi(ktKod)) continue;
                    if (kt != ENUMKullaniciTipi.TANIMSIZ) tipler.Add(ktKod);
                }

            return Results.Json(new
            {
                kod = d.kod,
                tarih = TarihYaz(d.tarih),
                konu = d.konu,
                onem = d.onem,
                kullaniciTipleri = tipler,
                gizli = d.gizle == ENUMGizle.GORUNMEZ,
                pencerede = d.pencerede == ENUMPencerede.GORUNUR,
                metin = d.duyuru
            });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, DuyuruKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!GirisYetkisiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null) return Results.BadRequest();

            int[] tipler = istek.KullaniciTipleri ?? new int[0];
            if (DestekYoneticisi(kullanan) && tipler.Length == 0)
                return Hata(Metin.Duyuru("FRMDYG026"));

            string metin = (istek.Metin ?? "").Trim();
            if (metin == "")
                return Hata(Metin.Duyuru("FRMDYG015"));
            if (OrtakFonksiyonlar.TehlikeliDizeVar(metin, out string _))
                return Hata(Metin.Duyuru("Tehlikeli"));

            TNS.KYM.Duyuru d = new TNS.KYM.Duyuru();
            d.kod = Math.Max(0, istek.Kod);
            d.konu = istek.Konu ?? "";
            d.onem = istek.Onem;
            d.tarih = TarihOku(istek.Tarih);
            d.KullaniciTipleriOl(string.Join(";", tipler));
            d.gizle = istek.Gizli ? ENUMGizle.GORUNMEZ : ENUMGizle.GORUNUR;
            d.pencerede = istek.Pencerede ? ENUMPencerede.GORUNUR : ENUMPencerede.GORUNMEZ;
            // Eski sayfadaki düzeltme: bağlantılar yeni sekmede açılır (editör zaten eklediyse dokunulmaz).
            d.duyuru = Regex.Replace(metin, @"<a\s(?![^>]*target=)", "<a target=\"_blank\" ", RegexOptions.IgnoreCase);

            Sonuc sonuc = TNS.KYM.Arac.Tanimla().DUYKaydet(kullanan, d);
            if (!sonuc.islemSonuc)
                return Hata(string.IsNullOrEmpty(sonuc.hataStr) ? Metin.Duyuru("IslemHata") : sonuc.hataStr);

            return Results.Json(new { basarili = true, kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, d.kod), bilgi = Metin.Duyuru("Kaydedildi") });
        }

        private static async Task<IResult> Sil(HttpContext baglam, DuyuruSilIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!GirisYetkisiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || istek.Kod <= 0) return Hata(Metin.Duyuru("FRMDYG021"));

            Sonuc sonuc = TNS.KYM.Arac.Tanimla().DUYSil(kullanan, istek.Kod);
            if (!sonuc.islemSonuc)
                return Hata(string.IsNullOrEmpty(sonuc.hataStr) ? Metin.Duyuru("IslemHata") : sonuc.hataStr);

            return Results.Json(new { basarili = true, bilgi = Metin.Duyuru("Silindi") });
        }

        // ------------------------------------------------------------------ yardımcılar

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        /// <summary>Tarayıcıdan gelen yyyy-MM-dd tarihi; boş ya da hatalıysa boş tarih.</summary>
        private static TNSDateTime TarihOku(string deger)
        {
            if (DateTime.TryParseExact(deger ?? "", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                return new TNSDateTime(t);
            return new TNSDateTime();
        }

        private static string TarihYaz(TNSDateTime tarih)
        {
            return tarih == null || tarih.isNull ? "" : tarih.Oku().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}
