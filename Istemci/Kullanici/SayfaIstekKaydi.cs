using System;
using System.Threading.Tasks;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TNS;
using TNS.KYM;

namespace Istemci.Kullanicilar
{
    /// <summary>
    /// Sayfa isteklerinin kullanıcı veri tabanındaki KULLANICI_BILGI tablosuna yazılması (eski OturumBilgisiIslem.KullaniciVeriTabaninaYaz).
    /// Eski sistemde oturumdaki kullanıcı her .aspx sayfasını açtığında (Ext ajax istekleri hariç) bir satır yazılıyordu; burada da
    /// yalnızca sayfa istekleri yazılır: css/js/resim dosyaları (UseStaticFiles önce yanıtlar) ve /api çağrıları yazılmaz.
    /// Alanlar eskisiyle aynıdır: KULLANICIKODU, ISLEMTARIHI, ISLEMYERI (IP), SAYFAADI (adres, eskisi gibi .aspx ile), BILGI1 (ad soyad).
    /// Kayıt, yanıtı bekletmemek için arka planda yazılır; yazılamazsa sayfa etkilenmez.
    /// Bu kayıtlar KYM/CalisanKullaniciListe ve KYM/CalisanKullaniciGrafik sayfalarının kaynağıdır; eski kayıtlar
    /// KullaniciBilgiArsivIsi ile KULLANICI_BILGI_ARSIV tablosuna taşınır.
    /// appSettings: SayfaIstekKaydi = "0" ise hiç yazılmaz (varsayılan "1").
    /// </summary>
    public static class SayfaIstekKaydi
    {
        public static void Kullan(WebApplication app)
        {
            if (Ayar.Oku("SayfaIstekKaydi", "1") == "0") return;

            app.Use(async (baglam, sonraki) =>
            {
                await sonraki(baglam);
                try
                {
                    Yaz(baglam);
                }
                catch (Exception)
                {
                    // Kayıt yazılamaması sayfayı bozmamalı.
                }
            });
        }

        private static void Yaz(HttpContext baglam)
        {
            if (!HttpMethods.IsGet(baglam.Request.Method)) return;
            if (baglam.Response.StatusCode != StatusCodes.Status200OK) return;
            if (baglam.Request.Path.StartsWithSegments("/api")) return;

            // Yalnızca Razor sayfaları (eski .aspx karşılıkları).
            Endpoint uc = baglam.GetEndpoint();
            if (uc == null || uc.Metadata.GetMetadata<CompiledPageActionDescriptor>() == null) return;

            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null || string.IsNullOrEmpty(kullanan.kullaniciKodu)) return;

            string yol = baglam.Request.PathBase.ToString() + baglam.Request.Path.ToString();
            if (!yol.EndsWith("/")) yol += ".aspx";
            string kullaniciKodu = kullanan.kullaniciKodu;
            string adSoyad = (kullanan.ad + " " + kullanan.soyad).Trim();
            string ip = Ayar.IPAdresi(baglam);
            TNSDateTime tarih = new TNSDateTime(DateTime.Now);

            Task.Run(() =>
            {
                try
                {
                    TNS.KYM.Arac.Tanimla().KullaniciSessionAta(kullaniciKodu, tarih, ip, yol, adSoyad, "", "", "", "");
                }
                catch (Exception)
                {
                }
            });
        }
    }
}
