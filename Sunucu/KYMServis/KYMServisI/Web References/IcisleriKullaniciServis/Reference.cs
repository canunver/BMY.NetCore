//------------------------------------------------------------------------------
// İçişleri Bakanlığı YetkiSGB web servisi (ASMX, SOAP 1.1) istemcisi.
//
// Eski sürüm Visual Studio'nun ürettiği System.Web.Services.Protocols.SoapHttpClientProtocol
// tabanlı sınıftı; .NET 8'de bu altyapı olmadığı için istemci HttpClient + XmlSerializer ile
// elle yazıldı. Sınıf ve metot adları eskisiyle aynıdır. Veri sınıfları (SGBKullanici,
// KullaniciTipi) eski üretilmiş koddan değiştirilmeden alındı.
//------------------------------------------------------------------------------
#pragma warning disable 1591
namespace KYMServisI.IcisleriKullaniciServis {
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public partial class YetkiSGB {

        private const string AdAlani = "http://tempuri.org/";
        private const string SoapAdAlani = "http://schemas.xmlsoap.org/soap/envelope/";
        private static readonly ConcurrentDictionary<string, XmlSerializer> serilestiriciler = new ConcurrentDictionary<string, XmlSerializer>();

        public YetkiSGB() {
            this.Url = global::KYMServisI.Properties.Settings.Default.KYMServisI_IcisleriKullaniciServis_YetkiSGB;
            this.Timeout = 100000;
        }

        /// <summary>Servis adresi (…/YetkiSGB.asmx).</summary>
        public string Url { get; set; }

        /// <summary>Zaman aşımı, milisaniye.</summary>
        public int Timeout { get; set; }

        public IWebProxy Proxy { get; set; }

        public ICredentials Credentials { get; set; }

        public bool UseDefaultCredentials { get; set; }

        public SGBKullanici GirisYap(string KullaniciID, string SessionID) {
            return (SGBKullanici)Cagir("GirisYap", typeof(SGBKullanici),
                new Parametre("KullaniciID", KullaniciID), new Parametre("SessionID", SessionID));
        }

        public SGBKullanici KullaniciOku(string KullaniciID) {
            return (SGBKullanici)Cagir("KullaniciOku", typeof(SGBKullanici), new Parametre("KullaniciID", KullaniciID));
        }

        public System.Data.DataSet KullaniciListele(SGBKullanici kriter) {
            return (System.Data.DataSet)Cagir("KullaniciListele", typeof(System.Data.DataSet), new Parametre("kriter", kriter));
        }

        public System.Data.DataSet KullaniciListeleTest(string ad, string soyad, string muhasebeBirimi, string kullaniciAdi, string tcno, string kullanicitipi) {
            return (System.Data.DataSet)Cagir("KullaniciListeleTest", typeof(System.Data.DataSet),
                new Parametre("ad", ad), new Parametre("soyad", soyad), new Parametre("muhasebeBirimi", muhasebeBirimi),
                new Parametre("kullaniciAdi", kullaniciAdi), new Parametre("tcno", tcno), new Parametre("kullanicitipi", kullanicitipi));
        }

        public System.Data.DataSet Muhasebe() {
            return (System.Data.DataSet)Cagir("Muhasebe", typeof(System.Data.DataSet));
        }

        public string[] SGBMenu(string KullaniciID) {
            return (string[])Cagir("SGBMenu", typeof(string[]), new Parametre("KullaniciID", KullaniciID));
        }

        private struct Parametre {
            public string Ad;
            public object Deger;
            public Parametre(string ad, object deger) {
                Ad = ad;
                Deger = deger;
            }
        }

        private static XmlSerializer Serilestirici(Type tip, string kokAd) {
            return serilestiriciler.GetOrAdd(tip.FullName + "|" + kokAd, delegate(string anahtar) {
                XmlRootAttribute kok = new XmlRootAttribute(kokAd);
                kok.Namespace = AdAlani;
                return new XmlSerializer(tip, kok);
            });
        }

        private string ZarfOlustur(string islem, Parametre[] parametreler) {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings ayar = new XmlWriterSettings();
            ayar.OmitXmlDeclaration = true;
            ayar.Encoding = new UTF8Encoding(false);
            using (XmlWriter w = XmlWriter.Create(sb, ayar)) {
                w.WriteStartElement("soap", "Envelope", SoapAdAlani);
                w.WriteStartElement("soap", "Body", SoapAdAlani);
                w.WriteStartElement(islem, AdAlani);
                foreach (Parametre p in parametreler) {
                    if (p.Deger == null) continue;
                    if (p.Deger is string) {
                        w.WriteElementString(p.Ad, AdAlani, (string)p.Deger);
                    }
                    else {
                        XmlSerializerNamespaces bos = new XmlSerializerNamespaces();
                        bos.Add("", AdAlani);
                        Serilestirici(p.Deger.GetType(), p.Ad).Serialize(w, p.Deger, bos);
                    }
                }
                w.WriteEndElement();
                w.WriteEndElement();
                w.WriteEndElement();
            }
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + sb.ToString();
        }

        private object Cagir(string islem, Type donusTipi, params Parametre[] parametreler) {
            if (string.IsNullOrWhiteSpace(this.Url))
                throw new InvalidOperationException("YetkiSGB web servis adresi (Url) tanımlı değil.");

            using (HttpClientHandler isleyici = new HttpClientHandler()) {
                if (this.Proxy != null) {
                    isleyici.Proxy = this.Proxy;
                    isleyici.UseProxy = true;
                }
                if (this.Credentials != null) isleyici.Credentials = this.Credentials;
                else if (this.UseDefaultCredentials) isleyici.UseDefaultCredentials = true;

                using (HttpClient http = new HttpClient(isleyici))
                using (HttpRequestMessage istek = new HttpRequestMessage(HttpMethod.Post, this.Url)) {
                    http.Timeout = TimeSpan.FromMilliseconds(this.Timeout > 0 ? this.Timeout : 100000);
                    istek.Content = new StringContent(ZarfOlustur(islem, parametreler), new UTF8Encoding(false), "text/xml");
                    istek.Headers.TryAddWithoutValidation("SOAPAction", "\"" + AdAlani + islem + "\"");

                    using (HttpResponseMessage yanit = http.Send(istek))
                    using (Stream akis = yanit.Content.ReadAsStream()) {
                        XmlDocument belge = new XmlDocument();
                        try {
                            belge.Load(akis);
                        }
                        catch (XmlException) {
                            throw new Exception("YetkiSGB." + islem + " çağrısı başarısız (HTTP " + (int)yanit.StatusCode + "); yanıt XML değil.");
                        }

                        XmlNamespaceManager ns = new XmlNamespaceManager(belge.NameTable);
                        ns.AddNamespace("soap", SoapAdAlani);
                        ns.AddNamespace("t", AdAlani);

                        XmlNode hata = belge.SelectSingleNode("/soap:Envelope/soap:Body/soap:Fault", ns);
                        if (hata != null) {
                            XmlNode aciklama = hata.SelectSingleNode("faultstring");
                            throw new Exception("YetkiSGB." + islem + " hata döndürdü: " + (aciklama != null ? aciklama.InnerText : hata.InnerText));
                        }
                        if (!yanit.IsSuccessStatusCode)
                            throw new Exception("YetkiSGB." + islem + " çağrısı başarısız (HTTP " + (int)yanit.StatusCode + ").");

                        XmlNode sonuc = belge.SelectSingleNode("/soap:Envelope/soap:Body/t:" + islem + "Response/t:" + islem + "Result", ns);
                        if (sonuc == null) return null;

                        using (XmlNodeReader okuyucu = new XmlNodeReader(sonuc)) {
                            return Serilestirici(donusTipi, islem + "Result").Deserialize(okuyucu);
                        }
                    }
                }
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class SGBKullanici {
        
        private string kullaniciKoduField;
        
        private string kimlikNumarasiField;
        
        private string adField;
        
        private string soyadField;
        
        private string parolaField;
        
        private string calistigiKurumKoduField;
        
        private string calistigiKurumKoduAdiField;
        
        private System.DateTime dogumTarihiField;
        
        private string emailAdresiField;
        
        private string telefonField;
        
        private System.DateTime sonGirisTarihiField;
        
        private int aktifMiField;
        
        private string calistigiBirimKoduField;
        
        private string calistigiBirimAdiField;
        
        private string sicilNoField;
        
        private string unvanField;
        
        private System.DateTime parolaBitisTarihiField;
        
        private string sessionIdField;
        
        private KullaniciTipi[] kullaniciTipiField;
        
        private bool girisSonucField;
        
        private string hataField;
        
        private int aramadaGostermeField;
        
        /// <remarks/>
        public string KullaniciKodu {
            get {
                return this.kullaniciKoduField;
            }
            set {
                this.kullaniciKoduField = value;
            }
        }
        
        /// <remarks/>
        public string KimlikNumarasi {
            get {
                return this.kimlikNumarasiField;
            }
            set {
                this.kimlikNumarasiField = value;
            }
        }
        
        /// <remarks/>
        public string Ad {
            get {
                return this.adField;
            }
            set {
                this.adField = value;
            }
        }
        
        /// <remarks/>
        public string Soyad {
            get {
                return this.soyadField;
            }
            set {
                this.soyadField = value;
            }
        }
        
        /// <remarks/>
        public string Parola {
            get {
                return this.parolaField;
            }
            set {
                this.parolaField = value;
            }
        }
        
        /// <remarks/>
        public string CalistigiKurumKodu {
            get {
                return this.calistigiKurumKoduField;
            }
            set {
                this.calistigiKurumKoduField = value;
            }
        }
        
        /// <remarks/>
        public string CalistigiKurumKoduAdi {
            get {
                return this.calistigiKurumKoduAdiField;
            }
            set {
                this.calistigiKurumKoduAdiField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime DogumTarihi {
            get {
                return this.dogumTarihiField;
            }
            set {
                this.dogumTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string EmailAdresi {
            get {
                return this.emailAdresiField;
            }
            set {
                this.emailAdresiField = value;
            }
        }
        
        /// <remarks/>
        public string Telefon {
            get {
                return this.telefonField;
            }
            set {
                this.telefonField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime SonGirisTarihi {
            get {
                return this.sonGirisTarihiField;
            }
            set {
                this.sonGirisTarihiField = value;
            }
        }
        
        /// <remarks/>
        public int AktifMi {
            get {
                return this.aktifMiField;
            }
            set {
                this.aktifMiField = value;
            }
        }
        
        /// <remarks/>
        public string CalistigiBirimKodu {
            get {
                return this.calistigiBirimKoduField;
            }
            set {
                this.calistigiBirimKoduField = value;
            }
        }
        
        /// <remarks/>
        public string CalistigiBirimAdi {
            get {
                return this.calistigiBirimAdiField;
            }
            set {
                this.calistigiBirimAdiField = value;
            }
        }
        
        /// <remarks/>
        public string SicilNo {
            get {
                return this.sicilNoField;
            }
            set {
                this.sicilNoField = value;
            }
        }
        
        /// <remarks/>
        public string Unvan {
            get {
                return this.unvanField;
            }
            set {
                this.unvanField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime ParolaBitisTarihi {
            get {
                return this.parolaBitisTarihiField;
            }
            set {
                this.parolaBitisTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string SessionId {
            get {
                return this.sessionIdField;
            }
            set {
                this.sessionIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public KullaniciTipi[] kullaniciTipi {
            get {
                return this.kullaniciTipiField;
            }
            set {
                this.kullaniciTipiField = value;
            }
        }
        
        /// <remarks/>
        public bool GirisSonuc {
            get {
                return this.girisSonucField;
            }
            set {
                this.girisSonucField = value;
            }
        }
        
        /// <remarks/>
        public string Hata {
            get {
                return this.hataField;
            }
            set {
                this.hataField = value;
            }
        }
        
        /// <remarks/>
        public int AramadaGosterme {
            get {
                return this.aramadaGostermeField;
            }
            set {
                this.aramadaGostermeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class KullaniciTipi {
        
        private string adField;
        
        private int kodField;
        
        /// <remarks/>
        public string ad {
            get {
                return this.adField;
            }
            set {
                this.adField = value;
            }
        }
        
        /// <remarks/>
        public int kod {
            get {
                return this.kodField;
            }
            set {
                this.kodField = value;
            }
        }
    }
}

#pragma warning restore 1591
