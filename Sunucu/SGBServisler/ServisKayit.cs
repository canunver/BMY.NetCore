using System;
using System.Collections.Generic;
using System.Linq;

namespace TNS.Api
{
    /// <summary>
    /// Servis arayüzlerini (IKHMServis ...) iş kuralları sınıflarına (KHMServis ...) bağlar.
    /// Eski Sunucu\Sunucu\web.config içindeki &lt;wellknown type="..." objectUri="XServis.rem"&gt; satırlarının karşılığıdır.
    ///
    /// Tek katmanlı kullanımda web uygulaması açılışta ServisKayit.Varsayilanlar() çağırır;
    /// bundan sonra Arac.Tanimla() ağ çağrısı yapmadan doğrudan iş kuralları nesnesini döndürür.
    /// </summary>
    public static class ServisKayit
    {
        private class Kayit
        {
            public Type Arayuz;
            public Func<object> Fabrika;
        }

        private static readonly object kilit = new object();
        private static readonly Dictionary<string, Kayit> kayitlar = new Dictionary<string, Kayit>(StringComparer.OrdinalIgnoreCase);
        private static bool varsayilanlarKaydedildi;

        /// <summary>Taşınmış bütün servisleri kaydeder. Uygulama açılışında bir kez çağrılır.</summary>
        public static void Varsayilanlar()
        {
            lock (kilit)
            {
                if (varsayilanlarKaydedildi) return;
                varsayilanlarKaydedildi = true;

                // Yeni taşınan her servis için buraya bir satır eklenir.
                Ekle<TNS.DEG.IDEGServis>("DEG", () => new TNS.DEG.DEGServis());
                Ekle<TNS.UZY.IUZYServis>("UZY", () => new TNS.UZY.UZYServis());
                Ekle<TNS.KYM.IKYMServis>("KYM", () => new TNS.KYM.IsKurallari.KYMServis());
                Ekle<TNS.ORG.IORGServis>("ORG", () => new TNS.KYM.IsKurallari.KYMServis());
                Ekle<TNS.KHM.IKHMServis>("KHM", () => new TNS.KHM.IsKurallari.KHMServis());
            }
        }

        public static void Ekle<T>(string ad, Func<T> fabrika) where T : class
        {
            lock (kilit)
            {
                kayitlar[ad] = new Kayit { Arayuz = typeof(T), Fabrika = () => fabrika() };
            }
            // Arac.Tanimla() -> ServisIstemci.Olustur<T>() bu kaydı görünce doğrudan nesneyi döndürür.
            ServisIstemci.YerelKaydet<T>(fabrika);
        }

        public static bool Bul(string ad, out Type arayuz, out Func<object> fabrika)
        {
            lock (kilit)
            {
                Kayit k;
                if (ad != null && kayitlar.TryGetValue(ad, out k))
                {
                    arayuz = k.Arayuz;
                    fabrika = k.Fabrika;
                    return true;
                }
            }
            arayuz = null;
            fabrika = null;
            return false;
        }

        public static IEnumerable<string> Adlar()
        {
            lock (kilit)
            {
                return kayitlar.Keys.OrderBy(a => a).ToList();
            }
        }
    }
}
