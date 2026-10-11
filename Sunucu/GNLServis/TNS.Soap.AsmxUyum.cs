// Visual Studio'nun .NET Framework için ürettiği ASMX istemci dosyalarının (Web References\...\Reference.cs)
// .NET 8'de DEĞİŞTİRİLMEDEN derlenip çalışabilmesi için uyum katmanı.
//
// .NET 8'de System.Web.Services yoktur. Burada aynı adlarla en az gereken tipler tanımlanır; SoapHttpClientProtocol.Invoke
// isteği HttpClient ile gönderir, gövdeyi eski altyapının yaptığı gibi XmlReflectionImporter.ImportMembersMapping ile
// (metodun ve parametrelerin Xml... nitelikleri, "xSpecified" parametreleri, sarmalı/Bare biçim) serileştirir.
// Desteklenen: SOAP 1.1, document/literal. Desteklenmeyen: Async/Begin/End çağrıları, RPC/encoded biçim, SOAP başlıkları.
//
// _araclar/asmx_istemci_uret.py ile TNS.Soap.SoapIstemci'ye çevrilmiş dosyalar bu katmanı kullanmaz; ikisi birlikte yaşar.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace System.Web.Services
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
    public sealed class WebServiceBindingAttribute : Attribute
    {
        public WebServiceBindingAttribute() { }
        public WebServiceBindingAttribute(string name) { Name = name; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string Location { get; set; }
    }
}

namespace System.Web.Services.Description
{
    public enum SoapBindingUse { Default = 0, Encoded = 1, Literal = 2 }
}

namespace System.Web.Services.Protocols
{
    using System.Web.Services.Description;

    public enum SoapParameterStyle { Default = 0, Bare = 1, Wrapped = 2 }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SoapDocumentMethodAttribute : Attribute
    {
        public SoapDocumentMethodAttribute() { }
        public SoapDocumentMethodAttribute(string action) { Action = action; }
        public string Action { get; set; }
        public string RequestNamespace { get; set; }
        public string ResponseNamespace { get; set; }
        public string RequestElementName { get; set; }
        public string ResponseElementName { get; set; }
        public string Binding { get; set; }
        public bool OneWay { get; set; }
        public SoapBindingUse Use { get; set; }
        public SoapParameterStyle ParameterStyle { get; set; }
    }

    public class InvokeCompletedEventArgs : AsyncCompletedEventArgs
    {
        private readonly object[] results;

        public InvokeCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }

        public object[] Results
        {
            get { return results; }
        }
    }

    /// <summary>Eski System.Web.Services.Protocols.SoapHttpClientProtocol'ün karşılığı (yalnızca eşzamanlı çağrılar).</summary>
    public class SoapHttpClientProtocol : IDisposable
    {
        private const string SoapNs = "http://schemas.xmlsoap.org/soap/envelope/";

        private class Islem
        {
            public string Action;
            public bool TekYon;
            public XmlSerializer Istek;
            public XmlSerializer Yanit;
            public int[] IstekSirasi;   // istek üyelerinin parametre dizisindeki yerleri
            public int YanitUyeSayisi;
        }

        private static readonly ConcurrentDictionary<MethodInfo, Islem> islemler = new ConcurrentDictionary<MethodInfo, Islem>();

        public SoapHttpClientProtocol()
        {
            Timeout = 100000;
            ClientCertificates = new X509CertificateCollection();
        }

        public string Url { get; set; }
        /// <summary>Zaman aşımı, milisaniye.</summary>
        public int Timeout { get; set; }
        public IWebProxy Proxy { get; set; }
        public ICredentials Credentials { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public bool PreAuthenticate { get; set; }
        public bool AllowAutoRedirect { get; set; }
        public string UserAgent { get; set; }
        public Encoding RequestEncoding { get; set; }
        public CookieContainer CookieContainer { get; set; }
        public X509CertificateCollection ClientCertificates { get; private set; }

        public void Dispose() { }
        public void Abort() { }

        protected void CancelAsync(object userState) { }

        protected void InvokeAsync(string methodName, object[] parameters, SendOrPostCallback callback)
        {
            throw new NotSupportedException(GetType().Name + "." + methodName + "Async: eşzamansız ASMX çağrıları yeni sistemde desteklenmiyor; eşzamanlı metodu kullanın.");
        }

        protected void InvokeAsync(string methodName, object[] parameters, SendOrPostCallback callback, object userState)
        {
            InvokeAsync(methodName, parameters, callback);
        }

        protected IAsyncResult BeginInvoke(string methodName, object[] parameters, AsyncCallback callback, object asyncState)
        {
            throw new NotSupportedException(GetType().Name + ".Begin" + methodName + ": eşzamansız ASMX çağrıları yeni sistemde desteklenmiyor; eşzamanlı metodu kullanın.");
        }

        protected object[] EndInvoke(IAsyncResult asyncResult)
        {
            throw new NotSupportedException("Eşzamansız ASMX çağrıları yeni sistemde desteklenmiyor.");
        }

        private MethodInfo MetotBul(string ad, object[] parametreler)
        {
            MethodInfo bulunan = null;
            foreach (MethodInfo m in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (m.Name != ad || m.GetCustomAttribute<SoapDocumentMethodAttribute>() == null) continue;
                int giren = 0;
                foreach (ParameterInfo p in m.GetParameters()) if (!p.IsOut) giren++;
                if (giren == (parametreler == null ? 0 : parametreler.Length)) return m;
                bulunan = bulunan ?? m;
            }
            if (bulunan == null)
                throw new InvalidOperationException(GetType().Name + "." + ad + " için SoapDocumentMethod niteliği taşıyan metot bulunamadı.");
            return bulunan;
        }

        private Islem IslemHazirla(MethodInfo metot)
        {
            return islemler.GetOrAdd(metot, delegate(MethodInfo m)
            {
                SoapDocumentMethodAttribute nitelik = m.GetCustomAttribute<SoapDocumentMethodAttribute>();
                if (nitelik.Use == SoapBindingUse.Encoded)
                    throw new NotSupportedException(m.DeclaringType.Name + "." + m.Name + ": RPC/encoded SOAP işlemleri desteklenmiyor.");

                WebServiceBindingAttribute baglama = m.DeclaringType.GetCustomAttribute<WebServiceBindingAttribute>();
                string varsayilanNs = baglama != null && !string.IsNullOrEmpty(baglama.Namespace) ? baglama.Namespace : "http://tempuri.org/";
                bool sarmali = nitelik.ParameterStyle != SoapParameterStyle.Bare;
                string istekNs = nitelik.RequestNamespace ?? varsayilanNs;
                string yanitNs = nitelik.ResponseNamespace ?? varsayilanNs;

                XmlReflectionImporter aktarici = new XmlReflectionImporter(varsayilanNs);
                aktarici.IncludeTypes(m.DeclaringType);

                List<XmlReflectionMember> istekUyeleri = new List<XmlReflectionMember>();
                List<int> istekSirasi = new List<int>();
                List<XmlReflectionMember> yanitUyeleri = new List<XmlReflectionMember>();

                if (m.ReturnType != typeof(void))
                {
                    XmlReflectionMember donus = new XmlReflectionMember();
                    donus.MemberName = m.Name + "Result";
                    donus.MemberType = m.ReturnType;
                    donus.IsReturnValue = true;
                    donus.XmlAttributes = new XmlAttributes(m.ReturnTypeCustomAttributes);
                    yanitUyeleri.Add(donus);
                }

                ParameterInfo[] parametreler = m.GetParameters();
                int girenSira = 0;
                for (int i = 0; i < parametreler.Length; i++)
                {
                    ParameterInfo p = parametreler[i];
                    XmlReflectionMember uye = new XmlReflectionMember();
                    uye.MemberName = p.Name;
                    uye.MemberType = p.ParameterType.IsByRef ? p.ParameterType.GetElementType() : p.ParameterType;
                    uye.XmlAttributes = new XmlAttributes(p);
                    if (!p.IsOut)
                    {
                        istekUyeleri.Add(uye);
                        istekSirasi.Add(girenSira++);
                    }
                    if (p.IsOut || p.ParameterType.IsByRef) yanitUyeleri.Add(uye);
                }

                Islem islem = new Islem();
                islem.Action = nitelik.Action ?? "";
                islem.TekYon = nitelik.OneWay;
                islem.IstekSirasi = istekSirasi.ToArray();
                islem.YanitUyeSayisi = yanitUyeleri.Count;

                XmlMembersMapping istekEsleme = aktarici.ImportMembersMapping(nitelik.RequestElementName ?? m.Name, istekNs, istekUyeleri.ToArray(), sarmali);
                List<XmlMapping> eslemeler = new List<XmlMapping>();
                eslemeler.Add(istekEsleme);
                if (yanitUyeleri.Count > 0)
                    eslemeler.Add(aktarici.ImportMembersMapping(nitelik.ResponseElementName ?? (m.Name + "Response"), yanitNs, yanitUyeleri.ToArray(), sarmali));

                XmlSerializer[] seri = XmlSerializer.FromMappings(eslemeler.ToArray());
                islem.Istek = seri[0];
                islem.Yanit = seri.Length > 1 ? seri[1] : null;
                return islem;
            });
        }

        /// <summary>Üretilmiş proxy metotlarının çağırdığı yöntem: dönüş değeri [0]'da, ardından out/ref parametreleri.</summary>
        protected object[] Invoke(string methodName, object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(Url))
                throw new InvalidOperationException(GetType().Name + " web servis adresi (Url) tanımlı değil.");

            MethodInfo metot = MetotBul(methodName, parameters);
            Islem islem = IslemHazirla(metot);

            object[] degerler = new object[islem.IstekSirasi.Length];
            for (int i = 0; i < degerler.Length; i++)
                degerler[i] = parameters != null && islem.IstekSirasi[i] < parameters.Length ? parameters[islem.IstekSirasi[i]] : null;

            StringBuilder sb = new StringBuilder();
            XmlWriterSettings ayar = new XmlWriterSettings();
            ayar.OmitXmlDeclaration = true;
            using (XmlWriter w = XmlWriter.Create(sb, ayar))
            {
                w.WriteStartElement("soap", "Envelope", SoapNs);
                w.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                w.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
                w.WriteStartElement("soap", "Body", SoapNs);
                islem.Istek.Serialize(w, degerler);
                w.WriteEndElement();
                w.WriteEndElement();
            }
            string zarf = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + sb.ToString();

            using (HttpClientHandler isleyici = new HttpClientHandler())
            {
                if (Proxy != null)
                {
                    isleyici.Proxy = Proxy;
                    isleyici.UseProxy = true;
                }
                if (Credentials != null) isleyici.Credentials = Credentials;
                else if (UseDefaultCredentials) isleyici.UseDefaultCredentials = true;
                if (CookieContainer != null) isleyici.CookieContainer = CookieContainer;
                if (ClientCertificates.Count > 0)
                {
                    isleyici.ClientCertificateOptions = ClientCertificateOption.Manual;
                    isleyici.ClientCertificates.AddRange(ClientCertificates);
                }

                using (HttpClient http = new HttpClient(isleyici))
                using (HttpRequestMessage istek = new HttpRequestMessage(HttpMethod.Post, Url))
                {
                    http.Timeout = Timeout > 0 ? TimeSpan.FromMilliseconds(Timeout) : System.Threading.Timeout.InfiniteTimeSpan;
                    istek.Content = new StringContent(zarf, new UTF8Encoding(false), "text/xml");
                    istek.Headers.TryAddWithoutValidation("SOAPAction", "\"" + islem.Action + "\"");
                    if (!string.IsNullOrEmpty(UserAgent)) istek.Headers.TryAddWithoutValidation("User-Agent", UserAgent);

                    using (HttpResponseMessage yanit = http.Send(istek))
                    using (Stream akis = yanit.Content.ReadAsStream())
                    {
                        if (islem.TekYon) return new object[0];

                        XmlDocument belge = new XmlDocument();
                        try
                        {
                            belge.Load(akis);
                        }
                        catch (XmlException)
                        {
                            throw new Exception(GetType().Name + "." + methodName + " çağrısı başarısız (HTTP " + (int)yanit.StatusCode + "); yanıt XML değil.");
                        }

                        XmlNamespaceManager ns = new XmlNamespaceManager(belge.NameTable);
                        ns.AddNamespace("soap", SoapNs);
                        XmlNode govde = belge.SelectSingleNode("/soap:Envelope/soap:Body", ns);
                        XmlNode hata = govde == null ? null : govde.SelectSingleNode("soap:Fault", ns);
                        if (hata != null)
                        {
                            XmlNode aciklama = hata.SelectSingleNode("faultstring");
                            throw new Exception(GetType().Name + "." + methodName + " hata döndürdü: " + (aciklama != null ? aciklama.InnerText : hata.InnerText));
                        }
                        if (!yanit.IsSuccessStatusCode || govde == null)
                            throw new Exception(GetType().Name + "." + methodName + " çağrısı başarısız (HTTP " + (int)yanit.StatusCode + ").");

                        if (islem.Yanit == null) return new object[0];

                        XmlNode ilk = null;
                        foreach (XmlNode c in govde.ChildNodes)
                            if (c.NodeType == XmlNodeType.Element) { ilk = c; break; }
                        if (ilk == null) return new object[islem.YanitUyeSayisi];

                        using (XmlNodeReader okuyucu = new XmlNodeReader(ilk))
                        {
                            okuyucu.MoveToContent();
                            object[] sonuc = (object[])islem.Yanit.Deserialize(okuyucu);
                            return sonuc ?? new object[islem.YanitUyeSayisi];
                        }
                    }
                }
            }
        }
    }
}
