using System;
using System.Collections.Generic;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;

namespace Istemci.OrtakSayfa
{
    /// <summary>Açılış sayfasının (OrtakSayfa/GirisBilgi) JavaScript'ten çağırdığı uçlar.</summary>
    public static class GirisBilgiUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ortaksayfa/giris-bilgi", GirisBilgi);
        }

        /// <summary>
        /// Açılış sayfasında gösterilecek bilgi ekranlarını (eski portal pencereleri) ve kullanıcı bilgisini döndürür.
        /// Ekranların listesi ve yerleşimi eskisi gibi TNS.KYM.Arac.bilgiEkranlar / ModulYerlesimBul'dan gelir.
        /// </summary>
        private static IResult GirisBilgi(HttpContext baglam)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();

            List<object> ekranlar = new List<object>();
            BilgiEkranYerlesim[] yerlesim = TNS.KYM.Arac.ModulYerlesimBul(kullanan);
            for (int i = 0; i < yerlesim.Length; i++)
            {
                BilgiEkran ekran = TNS.KYM.Arac.bilgiEkranlar[yerlesim[i].ekranIndex];

                bool kullaniliyor;
                try
                {
                    kullaniliyor = ekran.KullaniliyorMu();
                }
                catch (Exception)
                {
                    // Modülün kullanılıp kullanılmadığı okunamıyorsa ekran gösterilmez.
                    kullaniliyor = false;
                }
                if (!kullaniliyor) continue;

                ekranlar.Add(new
                {
                    id = ekran.resId,
                    baslik = ekran.BaslikBul() ?? ekran.resId,
                    yukseklik = ekran.yukseklik,
                    sutun = yerlesim[i].ekranCol,
                    adres = AdresDuzenle(ekran.sayfaAdres)
                });
            }

            return Results.Json(new
            {
                kullaniciKodu = kullanan.kullaniciKodu,
                adSoyad = kullanan.ADSOYAD,
                uyari = ParolaUyarisi(kullanan),
                ekranlar = ekranlar
            });
        }

        /// <summary>
        /// Eski tanımlardaki adresi yeni yapıya uyarlar: kullanıcı denetimleri (.ascx) artık kendi sayfası olarak açılır.
        /// .aspx uzantısı sunucuda zaten atılıyor; adres sayfaya göre göreli (../Modul/Sayfa) kalır.
        /// </summary>
        private static string AdresDuzenle(string adres)
        {
            if (string.IsNullOrEmpty(adres)) return adres;
            int soru = adres.IndexOf('?');
            string yol = soru >= 0 ? adres.Substring(0, soru) : adres;
            string sorgu = soru >= 0 ? adres.Substring(soru) : "";
            if (yol.EndsWith(".ascx", StringComparison.OrdinalIgnoreCase))
                yol = yol.Substring(0, yol.Length - 5);
            // Sık Kullanılanlar penceresi (BE003) Core'da OrtakSayfa altında: sayfalar uygulama kökünün bir alt klasöründe olmalı (KOK = '../').
            if (string.Equals(yol, "../Dashboard/Genel/SikKullanilan", StringComparison.OrdinalIgnoreCase))
                yol = "../OrtakSayfa/SikKullanilan";
            return yol + sorgu;
        }

        /// <summary>Parola süresi dolmuşsa gösterilecek uyarı (eski GenelIslemler.ParolaSuresiKontrol). LDAP kullanılıyorsa kontrol yapılmaz.</summary>
        private static string ParolaUyarisi(Kullanici kullanan)
        {
            if (!string.IsNullOrWhiteSpace(Ayar.Degisken(0, "LDAP"))) return null;

            DateTime bitis;
            if (kullanan.parolaBitisTarihi == null || kullanan.parolaBitisTarihi.isNull)
                bitis = DateTime.Today.AddDays(-1);
            else
                bitis = kullanan.parolaBitisTarihi.Oku().AddDays(-1);

            if (DateTime.Today > bitis)
                return "Parola kullanım süreniz dolmuştur. Güvenlik açısından parolanızı değiştirmeniz gerekmektedir.";
            return null;
        }
    }
}
