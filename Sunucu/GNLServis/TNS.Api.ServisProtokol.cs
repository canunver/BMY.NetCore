using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace TNS.Api
{
    /// <summary>
    /// Servis arayüzündeki bir metodun API çağrısına nasıl çevrildiğini tanımlar.
    /// Sunucu (dağıtıcı) ve istemci (ServisIstemci) aynı kuralları buradan kullanır.
    ///
    ///   POST {servisAdresi}/{MetotAdi}?imza=Tip1,Tip2
    ///   Gövde : { "parametreAdi": değer, ... }   (out parametreler gönderilmez)
    ///   Yanıt : metodun dönüş değeri (JSON). ref/out parametre varsa { "donus": ..., "cikis": { "ad": değer } }
    ///   Hata  : HTTP 500 ve { "hata": "...", "sonuc": { ... } }
    /// </summary>
    public static class ServisProtokol
    {
        /// <summary>Aynı adlı metotları (overload) ayırmak için parametre tip adlarından oluşan imza.</summary>
        public static string Imza(MethodInfo m)
        {
            return string.Join(",", m.GetParameters().Select(p => TipAd(p.ParameterType)));
        }

        private static string TipAd(Type t)
        {
            if (t.IsByRef) t = t.GetElementType();
            if (t.IsArray) return TipAd(t.GetElementType()) + "[]";
            if (t.IsGenericType)
            {
                string ad = t.Name;
                int i = ad.IndexOf('`');
                if (i > 0) ad = ad.Substring(0, i);
                return ad + "<" + string.Join(";", t.GetGenericArguments().Select(TipAd)) + ">";
            }
            return t.Name;
        }

        public static IEnumerable<MethodInfo> Metotlar(Type arayuz)
        {
            foreach (MethodInfo m in arayuz.GetMethods()) yield return m;
            foreach (Type ust in arayuz.GetInterfaces())
                foreach (MethodInfo m in ust.GetMethods()) yield return m;
        }

        /// <summary>
        /// Çağrılacak metodu bulur. imza verilmişse birebir eşleşen, verilmemişse (JavaScript çağrıları)
        /// gövdedeki parametre adlarına uyan metot seçilir.
        /// </summary>
        public static MethodInfo MetotBul(Type arayuz, string ad, string imza, ICollection<string> govdeAnahtarlari)
        {
            List<MethodInfo> adaylar = Metotlar(arayuz).Where(m => m.Name == ad).ToList();
            if (adaylar.Count == 0)
                adaylar = Metotlar(arayuz).Where(m => string.Equals(m.Name, ad, StringComparison.OrdinalIgnoreCase)).ToList();
            if (adaylar.Count == 0)
                throw new MissingMethodException(arayuz.Name + " arayüzünde '" + ad + "' adlı metot yok.");

            if (!string.IsNullOrEmpty(imza))
            {
                MethodInfo bulunan = adaylar.FirstOrDefault(m => Imza(m) == imza);
                if (bulunan == null)
                    throw new MissingMethodException(arayuz.Name + "." + ad + " için '" + imza + "' imzalı metot yok.");
                return bulunan;
            }

            if (adaylar.Count == 1) return adaylar[0];

            HashSet<string> anahtarlar = new HashSet<string>(govdeAnahtarlari ?? new string[0], StringComparer.OrdinalIgnoreCase);
            List<MethodInfo> uyan = adaylar.Where(m => anahtarlar.SetEquals(m.GetParameters().Where(p => !p.IsOut).Select(p => p.Name))).ToList();
            if (uyan.Count == 1) return uyan[0];

            throw new AmbiguousMatchException(arayuz.Name + "." + ad + " metodunun birden fazla sürümü var; 'imza' sorgu parametresi ile seçin: "
                + string.Join(" | ", adaylar.Select(Imza)));
        }

        public static string GovdeYaz(MethodInfo m, object[] args)
        {
            JObject govde = new JObject();
            ParameterInfo[] ps = m.GetParameters();
            for (int i = 0; i < ps.Length; i++)
            {
                if (ps[i].IsOut) continue;
                govde[ps[i].Name] = ApiJson.TokenYaz(args[i], ParametreTipi(ps[i]));
            }
            return govde.ToString(Newtonsoft.Json.Formatting.None);
        }

        public static object[] GovdeOku(MethodInfo m, JObject govde)
        {
            ParameterInfo[] ps = m.GetParameters();
            object[] args = new object[ps.Length];
            for (int i = 0; i < ps.Length; i++)
            {
                Type t = ParametreTipi(ps[i]);
                JToken deger = ps[i].IsOut || govde == null ? null : govde.GetValue(ps[i].Name, StringComparison.OrdinalIgnoreCase);
                args[i] = ApiJson.TokenOku(deger, t);
            }
            return args;
        }

        public static string SonucYaz(MethodInfo m, object donus, object[] args)
        {
            ParameterInfo[] ps = m.GetParameters();
            if (!ps.Any(p => p.ParameterType.IsByRef))
                return m.ReturnType == typeof(void) ? "null" : ApiJson.Yaz(donus, m.ReturnType);

            JObject o = new JObject();
            o["donus"] = m.ReturnType == typeof(void) ? JValue.CreateNull() : ApiJson.TokenYaz(donus, m.ReturnType);
            JObject cikis = new JObject();
            for (int i = 0; i < ps.Length; i++)
                if (ps[i].ParameterType.IsByRef) cikis[ps[i].Name] = ApiJson.TokenYaz(args[i], ParametreTipi(ps[i]));
            o["cikis"] = cikis;
            return o.ToString(Newtonsoft.Json.Formatting.None);
        }

        public static object SonucOku(MethodInfo m, string json, object[] args)
        {
            ParameterInfo[] ps = m.GetParameters();
            if (!ps.Any(p => p.ParameterType.IsByRef))
                return m.ReturnType == typeof(void) ? null : ApiJson.Oku(json, m.ReturnType);

            JObject o = ApiJson.NesneAyristir(json);
            JObject cikis = o["cikis"] as JObject;
            for (int i = 0; i < ps.Length; i++)
                if (ps[i].ParameterType.IsByRef && cikis != null)
                    args[i] = ApiJson.TokenOku(cikis.GetValue(ps[i].Name, StringComparison.OrdinalIgnoreCase), ParametreTipi(ps[i]));
            return m.ReturnType == typeof(void) ? null : ApiJson.TokenOku(o["donus"], m.ReturnType);
        }

        private static Type ParametreTipi(ParameterInfo p)
        {
            return p.ParameterType.IsByRef ? p.ParameterType.GetElementType() : p.ParameterType;
        }
    }
}
