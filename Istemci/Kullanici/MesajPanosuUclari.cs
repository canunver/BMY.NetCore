using System;
using System.Collections.Generic;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;

namespace Istemci.Kullanicilar
{
    /// <summary>
    /// Sistem Mesajları bilgi ekranının (eski KYM/MesajPanosuForm.aspx) JavaScript'ten çağırdığı uç:
    ///   GET /api/mesajpanosu/liste : oturumdaki kullanıcıya yazılmış sistem mesajları (IKYMServis.ListeleMesajPanosu).
    /// Mesajları başka işlemler yazar (ör. ödenek gönderme); bu ekran yalnızca listeler.
    /// </summary>
    public static class MesajPanosuUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/mesajpanosu/liste", Liste);
        }

        private static IResult Liste(HttpContext baglam)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();

            List<object> mesajlar = new List<object>();
            try
            {
                KullaniciMesaj kistas = new KullaniciMesaj();
                kistas.kullaniciKodu = kullanan.kullaniciKodu;
                KullaniciMesaj[] liste = TNS.KYM.Arac.Tanimla().ListeleMesajPanosu(kullanan, kistas) ?? new KullaniciMesaj[0];
                foreach (KullaniciMesaj m in liste)
                {
                    bool tarihVar = m.tarih != null && !m.tarih.isNull;
                    DateTime t = tarihVar ? m.tarih.Oku() : DateTime.MinValue;
                    mesajlar.Add(new
                    {
                        m.mesajNo,
                        // Eski ekrandaki gibi gün.ay; tam tarih ipucunda gösterilir.
                        tarih = tarihVar ? t.ToString("dd.MM") : "",
                        tarihTam = tarihVar ? t.ToString("dd.MM.yyyy HH:mm") : "",
                        m.mesaj
                    });
                }
            }
            catch (Exception ex)
            {
                return Results.Json(new { hata = ex.Message, mesajlar });
            }

            return Results.Json(new { mesajlar });
        }
    }
}
