using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TNS;

namespace Istemci.Kullanicilar
{
    /// <summary>
    /// Ayda bir kez çalışan arka plan işi: KULLANICI_BILGI tablosunda saklama süresinden (varsayılan 2 yıl) eski sayfa isteği
    /// kayıtlarını KULLANICI_BILGI_ARSIV tablosuna küçük partilerle taşır (sunucuda KullaniciBilgiArsivi.Tasi). Arşiv tablosu ve
    /// ISLEMTARIHI index'leri yoksa ilk çalışmada oluşturulur (SQL betiği: _araclar\KullaniciBilgiArsiv.sql).
    ///
    /// IIS gece uygulamayı durdurabildiği için iş belli bir dakikaya bağlı değildir: yarım saatte bir bakılır, o ay henüz
    /// çalışmadıysa ve saat KullaniciBilgiArsivSaat'i geçtiyse çalışır (ayın ilk uygun saatinde). Son çalışmanın ayı (yyyy-MM)
    /// App_Data\KullaniciBilgiArsiv.son dosyasında tutulur.
    /// appSettings:
    ///   KullaniciBilgiArsivle       "0" ise iş çalışmaz (ör. aynı veri tabanını kullanan ikinci web sunucusunda). Varsayılan "1".
    ///   KullaniciBilgiSaklamaYil    ana tabloda tutulacak yıl. Varsayılan 2.
    ///   KullaniciBilgiArsivSaat     işin en erken başlayacağı saat (0-23). Varsayılan 3.
    ///   KullaniciBilgiArsivParti    bir partide taşınacak satır. Varsayılan 5000.
    /// </summary>
    public class KullaniciBilgiArsivIsi : BackgroundService
    {
        private readonly ILogger<KullaniciBilgiArsivIsi> gunluk;
        private readonly string sonDosya;

        public KullaniciBilgiArsivIsi(ILogger<KullaniciBilgiArsivIsi> gunluk, IHostEnvironment ortam)
        {
            this.gunluk = gunluk;
            sonDosya = Path.Combine(ortam.ContentRootPath, "App_Data", "KullaniciBilgiArsiv.son");
        }

        private static int Sayi(string anahtar, int varsayilan)
        {
            return OrtakFonksiyonlar.ConvertToInt(Ayar.Oku(anahtar, varsayilan.ToString(CultureInfo.InvariantCulture)), varsayilan);
        }

        private DateTime SonCalisma()
        {
            try
            {
                if (File.Exists(sonDosya) &&
                    DateTime.TryParseExact(File.ReadAllText(sonDosya).Trim(), "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                    return t;
            }
            catch (Exception)
            {
            }
            return DateTime.MinValue;
        }

        protected override async Task ExecuteAsync(CancellationToken durdur)
        {
            if (Ayar.Oku("KullaniciBilgiArsivle", "1") == "0") return;

            // Uygulama açılışını yavaşlatmamak için ilk bakış biraz sonra.
            try { await Task.Delay(TimeSpan.FromMinutes(2), durdur); } catch (TaskCanceledException) { return; }

            while (!durdur.IsCancellationRequested)
            {
                try
                {
                    int saat = Math.Max(0, Math.Min(23, Sayi("KullaniciBilgiArsivSaat", 3)));
                    DateTime simdi = DateTime.Now;
                    DateTime buAy = new DateTime(simdi.Year, simdi.Month, 1);
                    if (SonCalisma() < buAy && simdi.Hour >= saat)
                    {
                        int yil = Sayi("KullaniciBilgiSaklamaYil", 2);
                        int parti = Sayi("KullaniciBilgiArsivParti", 5000);
                        Sonuc s = await Task.Run(() => TNS.KYM.Arac.Tanimla().KullaniciBilgiArsiveTasi(yil, parti), durdur);
                        if (s != null && s.islemSonuc)
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(sonDosya));
                            File.WriteAllText(sonDosya, simdi.ToString("yyyy-MM", CultureInfo.InvariantCulture));
                            gunluk.LogInformation("KULLANICI_BILGI arşive taşıma bitti: {Bilgi}", s.bilgiStr);
                        }
                        else
                            gunluk.LogWarning("KULLANICI_BILGI arşive taşıma yapılamadı: {Hata}", s != null ? s.hataStr : "");
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception e)
                {
                    gunluk.LogWarning(e, "KULLANICI_BILGI arşive taşıma yapılamadı.");
                }

                try { await Task.Delay(TimeSpan.FromMinutes(30), durdur); } catch (TaskCanceledException) { return; }
            }
        }
    }
}
