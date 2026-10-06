using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TNS.Api
{
    /// <summary>
    /// Bir servis arayüzünü (IKHMServis, IKYMServis ...) çalışma zamanında uygulayan istemci sınıfı.
    /// Arayüzdeki her metot çağrısını API'ye HTTP POST olarak gönderir, dönen JSON'u metodun dönüş tipine çevirir.
    /// Eski .NET Remoting'deki Activator.GetObject'in yerini alır; Arac.Tanimla() bunu kullanır.
    /// </summary>
    public class ServisIstemci : DispatchProxy
    {
        private static readonly ConcurrentDictionary<Type, Func<object>> yerelServisler = new ConcurrentDictionary<Type, Func<object>>();

        /// <summary>Bütün çağrılarda kullanılan HttpClient. Gerekirse uygulama açılışında değiştirilebilir.</summary>
        public static HttpClient Http = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };

        /// <summary>
        /// Her istek gönderilmeden önce çağrılır. Oturum çerezi / yetki başlığı gibi bilgileri eklemek için kullanılır.
        /// </summary>
        public static Action<HttpRequestMessage> IstekHazirla;

        private Type arayuz;
        private string adres;

        /// <summary>
        /// Servis aynı uygulamanın içinde çalışıyorsa (API sunucusu) HTTP'ye çıkmadan doğrudan nesnenin kullanılmasını sağlar.
        /// </summary>
        public static void YerelKaydet<T>(Func<T> fabrika) where T : class
        {
            yerelServisler[typeof(T)] = () => fabrika();
        }

        /// <summary>
        /// T arayüzünü uygulayan nesneyi döndürür.
        /// </summary>
        /// <param name="servisAdresi">Servisin API adresi, örnek: http://sunucu/api/KHM (web.config'deki KHMServisURL değeri)</param>
        public static T Olustur<T>(string servisAdresi) where T : class
        {
            Func<object> fabrika;
            if (yerelServisler.TryGetValue(typeof(T), out fabrika)) return (T)fabrika();

            if (string.IsNullOrWhiteSpace(servisAdresi))
                throw new InvalidOperationException(typeof(T).Name + " servisi bu uygulamada kayıtlı değil (ServisKayit) ve API adresi de tanımlı değil (appSettings içindeki ...ServisURL anahtarı).");

            if (servisAdresi.Trim().EndsWith(".rem", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(typeof(T).Name + " için tanımlı adres (" + servisAdresi + ") eski Remoting adresi. Bu servis henüz yeni API'ye taşınmadı ya da ...ServisURL değeri güncellenmedi.");

            T nesne = Create<T, ServisIstemci>();
            ServisIstemci istemci = (ServisIstemci)(object)nesne;
            istemci.arayuz = typeof(T);
            istemci.adres = servisAdresi.Trim().TrimEnd('/');
            return nesne;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (args == null) args = new object[0];
            string url = adres + "/" + targetMethod.Name + "?imza=" + Uri.EscapeDataString(ServisProtokol.Imza(targetMethod));

            using (HttpRequestMessage istek = new HttpRequestMessage(HttpMethod.Post, url))
            {
                istek.Content = new StringContent(ServisProtokol.GovdeYaz(targetMethod, args), Encoding.UTF8, "application/json");
                if (IstekHazirla != null) IstekHazirla(istek);

                using (HttpResponseMessage yanit = Http.Send(istek))
                {
                    string metin;
                    using (StreamReader okuyucu = new StreamReader(yanit.Content.ReadAsStream(), Encoding.UTF8))
                        metin = okuyucu.ReadToEnd();

                    if (!yanit.IsSuccessStatusCode) HataFirlat(targetMethod, (int)yanit.StatusCode, metin);
                    return ServisProtokol.SonucOku(targetMethod, metin, args);
                }
            }
        }

        private void HataFirlat(MethodInfo m, int durumKodu, string metin)
        {
            string hata = null;
            Sonuc sonuc = null;
            try
            {
                JObject o = ApiJson.NesneAyristir(metin);
                hata = (string)o["hata"];
                if (o["sonuc"] != null && o["sonuc"].Type == JTokenType.Object)
                    sonuc = (Sonuc)ApiJson.TokenOku(o["sonuc"], typeof(Sonuc));
            }
            catch (Exception)
            {
                // Gövde JSON değilse ham metin kullanılır.
            }
            if (sonuc != null) throw new TNSIslemHatasi(sonuc);
            throw new Exception(arayuz.Name + "." + m.Name + " çağrısı başarısız (HTTP " + durumKodu + "): " + (hata ?? metin));
        }
    }
}
