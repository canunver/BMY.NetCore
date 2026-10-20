using System;
using System.Globalization;
using System.IO;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;

namespace Istemci.Kullanicilar
{
    /// <summary>
    /// Kullanıcı İşlem Tarihçesi raporu: eski KYM/Raporlar.aspx'teki tek rapor ("Tarihçe Raporu", KY001). Raporlar sayfası ve menü
    /// seçeneği Core'da yoktur; rapor Kullanıcı Arama sayfasındaki "İşlem Tarihçesi" düğmesinden kriterleriyle alınır.
    ///   GET /api/kullanici/islem-tarihce?tarih1=yyyy-MM-dd&amp;tarih2=yyyy-MM-dd&amp;tc=&amp;kullanici=&amp;islemYapan=
    ///       → Excel (KYM\KullaniciIslemTarihce.xltx) ya da { hata }
    /// Rapor kodu eski btnYazdir_Click'in aynısıdır. Eski sayfada Kullanıcı Grubu alanı gizliydi; kullaniciTipi 0 gönderilir.
    /// Yetki: eski menü seçeneğinin yetkisi (Yetki="1,2,39,54": sistem yöneticisi, kullanıcı yöneticisi, destek başvuru yöneticisi,
    /// evrak belge yöneticisi).
    /// </summary>
    public static class KullaniciIslemTarihceUclari
    {
        private static readonly ENUMKullaniciTipi[] yetkiliTipler =
        {
            ENUMKullaniciTipi.SISTEMYONETICISI, ENUMKullaniciTipi.KULLANICIYONETICISI,
            ENUMKullaniciTipi.DESTEKBASVURUYONETICI, ENUMKullaniciTipi.EVRAKBELGEYONETICISI
        };

        private const string SablonAd = "KullaniciIslemTarihce.xltx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/kullanici/islem-tarihce", Rapor);
        }

        public static bool Yetkili(Kullanici kullanan)
        {
            if (kullanan == null) return false;
            foreach (ENUMKullaniciTipi kt in yetkiliTipler)
                if (kullanan.KullaniciTipiIceriyorMu((int)kt)) return true;
            return false;
        }

        /// <summary>Kurumun RaporSablonYol\KYM klasöründeki şablon önceliklidir; yoksa uygulamayla gelen RaporSablon\KYM.</summary>
        private static string SablonBul(string uygulamaKlasoru)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "KYM", SablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "KYM", SablonAd);
            return File.Exists(u) ? u : null;
        }

        private static TNSDateTime Tarih(string metin)
        {
            if (DateTime.TryParseExact((metin ?? "").Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                return new TNSDateTime(t);
            return null;
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { hata = metin });
        }

        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, string tarih1, string tarih2, string tc, string kullanici, string islemYapan)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Yetkili(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            TNSDateTime t1 = Tarih(tarih1);
            TNSDateTime t2 = Tarih(tarih2);
            if (t1 == null || t2 == null) return Hata("Tarih aralığını giriniz.");

            string sablonYol = SablonBul(ortam.ContentRootPath);
            if (sablonYol == null) return Hata("Rapor şablonu bulunamadı (KYM\\" + SablonAd + ").");

            try
            {
                ObjectArray<Kullanici> bilgi = TNS.KYM.Arac.Tanimla().KullaniciIslemTarihceListele(kullanan, t1, t2,
                    (tc ?? "").Trim(), (kullanici ?? "").Trim(), (islemYapan ?? "").Trim(), 0);

                if (bilgi == null || !bilgi.sonuc.islemSonuc)
                    return Hata(bilgi?.sonuc?.hataStr ?? "Rapor bilgileri okunamadı.");
                if (bilgi.objeler.Count <= 0)
                    return Hata("Verilen kriterlere uygun kayıt bulunamadı");

                AsposeLisans.Yukle();
                string dosya = ExceleYaz(bilgi, sablonYol);
                return DosyaIslem.DosyaGonder(dosya, "KullaniciIslemTarihce" + DosyaIslem.DosyaAdUretSade(), true, "xlsx");
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }

        /// <summary>Eski Raporlar.aspx btnYazdir_Click'teki Excel kodunun aynısı. Geçici dosyanın yolunu döndürür.</summary>
        private static string ExceleYaz(ObjectArray<Kullanici> bilgi, string sablonYol)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = DosyaIslem.DosyaAdUret("KullaniciIslemTarihce_", ".xlsx");
            XLS.DosyaAc(sablonYol, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            foreach (Kullanici kul in bilgi.objeler)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 3, satir, sutun);

                string islemAdi = kul.telefon;
                if (islemAdi == "") islemAdi = kul.rumuz;

                XLS.HucreDegerYaz(satir, sutun, kul.dogumTarihi.ToLongString()); //işlem tarihi
                XLS.HucreDegerYaz(satir, sutun + 1, kul.kullaniciKodu + "(" + kul.ad + ")");//işlem yapılan
                XLS.HucreDegerYaz(satir, sutun + 2, kul.anaAdi);//işlemi yapan
                XLS.HucreDegerYaz(satir, sutun + 3, islemAdi);//işlem adı
                XLS.HucreDegerYaz(satir, sutun + 4, kul.unvan);//açıklama
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, -1);
            XLS.DosyaSaklaTamYol();
            return XLS.SonucDosyaAd();
        }
    }
}
