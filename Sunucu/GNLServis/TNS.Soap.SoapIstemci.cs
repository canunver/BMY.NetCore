using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TNS.Soap
{
    /// <summary>Bir SOAP işlemine gönderilen parametre: eleman adı, değeri ve bildirilen tipi.</summary>
    public struct SoapParametre
    {
        public string Ad;
        public object Deger;
        public Type Tip;

        public SoapParametre(string ad, object deger, Type tip)
        {
            Ad = ad;
            Deger = deger;
            Tip = tip;
        }
    }

    /// <summary>
    /// ASMX (SOAP 1.1, document/literal, wrapped) web servisleri için istemci taban sınıfı.
    /// .NET Framework'teki SoapHttpClientProtocol ve WSE 2.0 WebServicesClientProtocol sınıflarının yerine kullanılır;
    /// eski "Web References" altındaki Reference.cs dosyaları bu sınıftan türeyecek şekilde yeniden üretilir
    /// (_araclar/asmx_istemci_uret.py).
    ///
    /// KullaniciAdi doluysa isteğe WSE 2.0 biçiminde WS-Security UsernameToken (parola özetli) ve
    /// WS-Addressing başlıkları eklenir.
    /// </summary>
    public abstract class SoapIstemci
    {
        private const string SoapNs = "http://schemas.xmlsoap.org/soap/envelope/";
        private const string WsaNs = "http://schemas.xmlsoap.org/ws/2004/03/addressing";
        private const string WsseNs = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
        private const string WsuNs = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";
        private const string ParolaOzetTipi = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordDigest";
        private const string ParolaDuzTipi = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText";

        private static readonly ConcurrentDictionary<string, XmlSerializer> serilestiriciler = new ConcurrentDictionary<string, XmlSerializer>();

        protected SoapIstemci()
        {
            Timeout = 100000;
            ParolaOzetiGonder = true;
        }

        /// <summary>Servis adresi (…/Servis.asmx).</summary>
        public string Url { get; set; }

        /// <summary>Zaman aşımı, milisaniye.</summary>
        public int Timeout { get; set; }

        public IWebProxy Proxy { get; set; }

        public ICredentials Credentials { get; set; }

        public bool UseDefaultCredentials { get; set; }

        /// <summary>WS-Security UsernameToken kullanıcı adı. Boşsa güvenlik başlığı eklenmez.</summary>
        public string KullaniciAdi { get; set; }

        /// <summary>WS-Security UsernameToken parolası.</summary>
        public string Parola { get; set; }

        /// <summary>true: parola özeti (PasswordDigest, WSE'deki SendHashed) gönderilir; false: düz metin.</summary>
        public bool ParolaOzetiGonder { get; set; }

        /// <summary>Serileştirmede tanınması gereken ek tipler (eski proxy sınıfındaki XmlInclude satırları).</summary>
        protected virtual Type[] EkTipler
        {
            get { return null; }
        }

        private XmlSerializer Serilestirici(Type tip, string kokAd, string adAlani)
        {
            string anahtar = GetType().FullName + "|" + tip.FullName + "|" + kokAd + "|" + adAlani;
            return serilestiriciler.GetOrAdd(anahtar, delegate(string a)
            {
                XmlRootAttribute kok = new XmlRootAttribute(kokAd);
                kok.Namespace = adAlani;
                return new XmlSerializer(tip, null, EkTipler ?? new Type[0], kok, adAlani);
            });
        }

        /// <summary>SOAP işlemini çağırır ve {islem}Response/{sonucAd} elemanını donusTipi'ne çevirir.</summary>
        /// <param name="soapAction">SOAPAction başlığı</param>
        /// <param name="islem">İşlem (metot) adı; istek sarmalayıcı elemanının adı</param>
        /// <param name="adAlani">İstek ve yanıt XML ad alanı</param>
        /// <param name="donusTipi">Dönüş tipi; dönüş yoksa null</param>
        /// <param name="sonucAd">Yanıttaki sonuç elemanının adı (çoğunlukla {islem}Result)</param>
        protected object Cagir(string soapAction, string islem, string adAlani, Type donusTipi, string sonucAd, params SoapParametre[] parametreler)
        {
            if (string.IsNullOrWhiteSpace(Url))
                throw new InvalidOperationException(GetType().Name + " web servis adresi (Url) tanımlı değil.");

            string zarf = ZarfOlustur(soapAction, islem, adAlani, parametreler);

            using (HttpClientHandler isleyici = new HttpClientHandler())
            {
                if (Proxy != null)
                {
                    isleyici.Proxy = Proxy;
                    isleyici.UseProxy = true;
                }
                if (Credentials != null) isleyici.Credentials = Credentials;
                else if (UseDefaultCredentials) isleyici.UseDefaultCredentials = true;

                using (HttpClient http = new HttpClient(isleyici))
                using (HttpRequestMessage istek = new HttpRequestMessage(HttpMethod.Post, Url))
                {
                    http.Timeout = TimeSpan.FromMilliseconds(Timeout > 0 ? Timeout : 100000);
                    istek.Content = new StringContent(zarf, new UTF8Encoding(false), "text/xml");
                    istek.Headers.TryAddWithoutValidation("SOAPAction", "\"" + soapAction + "\"");

                    using (HttpResponseMessage yanit = http.Send(istek))
                    using (Stream akis = yanit.Content.ReadAsStream())
                    {
                        XmlDocument belge = new XmlDocument();
                        try
                        {
                            belge.Load(akis);
                        }
                        catch (XmlException)
                        {
                            throw new Exception(GetType().Name + "." + islem + " çağrısı başarısız (HTTP " + (int)yanit.StatusCode + "); yanıt XML değil.");
                        }

                        XmlNamespaceManager ns = new XmlNamespaceManager(belge.NameTable);
                        ns.AddNamespace("soap", SoapNs);
                        ns.AddNamespace("t", adAlani);

                        XmlNode hata = belge.SelectSingleNode("/soap:Envelope/soap:Body/soap:Fault", ns);
                        if (hata != null)
                        {
                            XmlNode aciklama = hata.SelectSingleNode("faultstring");
                            throw new Exception(GetType().Name + "." + islem + " hata döndürdü: " + (aciklama != null ? aciklama.InnerText : hata.InnerText));
                        }
                        if (!yanit.IsSuccessStatusCode)
                            throw new Exception(GetType().Name + "." + islem + " çağrısı başarısız (HTTP " + (int)yanit.StatusCode + ").");

                        if (donusTipi == null || donusTipi == typeof(void)) return null;

                        XmlNode sonuc = belge.SelectSingleNode("/soap:Envelope/soap:Body/t:" + islem + "Response/t:" + sonucAd, ns);
                        if (sonuc == null)
                            return donusTipi.IsValueType ? Activator.CreateInstance(donusTipi) : null;

                        using (XmlNodeReader okuyucu = new XmlNodeReader(sonuc))
                        {
                            return Serilestirici(donusTipi, sonucAd, adAlani).Deserialize(okuyucu);
                        }
                    }
                }
            }
        }

        private string ZarfOlustur(string soapAction, string islem, string adAlani, SoapParametre[] parametreler)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings ayar = new XmlWriterSettings();
            ayar.OmitXmlDeclaration = true;
            using (XmlWriter w = XmlWriter.Create(sb, ayar))
            {
                w.WriteStartElement("soap", "Envelope", SoapNs);
                w.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                w.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");

                if (!string.IsNullOrEmpty(KullaniciAdi))
                {
                    w.WriteAttributeString("xmlns", "wsa", null, WsaNs);
                    w.WriteAttributeString("xmlns", "wsse", null, WsseNs);
                    w.WriteAttributeString("xmlns", "wsu", null, WsuNs);
                    w.WriteStartElement("soap", "Header", SoapNs);
                    GuvenlikBasligiYaz(w, soapAction);
                    w.WriteEndElement();
                }

                w.WriteStartElement("soap", "Body", SoapNs);
                w.WriteStartElement(islem, adAlani);
                if (parametreler != null)
                {
                    XmlSerializerNamespaces adAlanlari = new XmlSerializerNamespaces();
                    adAlanlari.Add("", adAlani);
                    foreach (SoapParametre p in parametreler)
                    {
                        if (p.Deger == null) continue;
                        Serilestirici(p.Tip ?? p.Deger.GetType(), p.Ad, adAlani).Serialize(w, p.Deger, adAlanlari);
                    }
                }
                w.WriteEndElement();
                w.WriteEndElement();
                w.WriteEndElement();
            }
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + sb.ToString();
        }

        /// <summary>WSE 2.0'ın ürettiği başlıkların karşılığı: WS-Addressing + Timestamp + UsernameToken.</summary>
        private void GuvenlikBasligiYaz(XmlWriter w, string soapAction)
        {
            DateTime simdi = DateTime.UtcNow;
            string olusturma = simdi.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
            string bitis = simdi.AddMinutes(5).ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);

            w.WriteElementString("wsa", "Action", WsaNs, soapAction);
            w.WriteElementString("wsa", "MessageID", WsaNs, "uuid:" + Guid.NewGuid().ToString());
            w.WriteStartElement("wsa", "ReplyTo", WsaNs);
            w.WriteElementString("wsa", "Address", WsaNs, WsaNs + "/role/anonymous");
            w.WriteEndElement();
            w.WriteElementString("wsa", "To", WsaNs, Url);

            w.WriteStartElement("wsse", "Security", WsseNs);
            w.WriteAttributeString("soap", "mustUnderstand", SoapNs, "1");

            w.WriteStartElement("wsu", "Timestamp", WsuNs);
            w.WriteAttributeString("wsu", "Id", WsuNs, "Timestamp-" + Guid.NewGuid().ToString());
            w.WriteElementString("wsu", "Created", WsuNs, olusturma);
            w.WriteElementString("wsu", "Expires", WsuNs, bitis);
            w.WriteEndElement();

            w.WriteStartElement("wsse", "UsernameToken", WsseNs);
            w.WriteAttributeString("wsu", "Id", WsuNs, "SecurityToken-" + Guid.NewGuid().ToString());
            w.WriteElementString("wsse", "Username", WsseNs, KullaniciAdi);

            byte[] nonce = new byte[16];
            using (RandomNumberGenerator rastgele = RandomNumberGenerator.Create()) rastgele.GetBytes(nonce);

            w.WriteStartElement("wsse", "Password", WsseNs);
            if (ParolaOzetiGonder)
            {
                // Özet = Base64( SHA1( nonce + oluşturma zamanı + parola ) )
                byte[] zaman = Encoding.UTF8.GetBytes(olusturma);
                byte[] parola = Encoding.UTF8.GetBytes(Parola ?? "");
                byte[] hepsi = new byte[nonce.Length + zaman.Length + parola.Length];
                Buffer.BlockCopy(nonce, 0, hepsi, 0, nonce.Length);
                Buffer.BlockCopy(zaman, 0, hepsi, nonce.Length, zaman.Length);
                Buffer.BlockCopy(parola, 0, hepsi, nonce.Length + zaman.Length, parola.Length);
                byte[] ozet;
                using (SHA1 sha = SHA1.Create()) ozet = sha.ComputeHash(hepsi);
                w.WriteAttributeString("Type", ParolaOzetTipi);
                w.WriteString(Convert.ToBase64String(ozet));
            }
            else
            {
                w.WriteAttributeString("Type", ParolaDuzTipi);
                w.WriteString(Parola ?? "");
            }
            w.WriteEndElement();

            w.WriteElementString("wsse", "Nonce", WsseNs, Convert.ToBase64String(nonce));
            w.WriteElementString("wsu", "Created", WsuNs, olusturma);
            w.WriteEndElement();

            w.WriteEndElement();
        }
    }
}
