//------------------------------------------------------------------------------
// GIBServis web servisi istemcisi.
//
// Bu dosya, Visual Studio'nun .NET Framework için ürettiği eski Reference.cs dosyasından
// _araclar/asmx_istemci_uret.py ile dönüştürülmüştür. Proxy sınıfı TNS.Soap.SoapIstemci tabanlıdır
// (HttpClient + XmlSerializer); veri sınıfları eski üretilmiş koddan değiştirilmeden alınmıştır.
//------------------------------------------------------------------------------
#pragma warning disable 1591
namespace HRCServisC.tr.gov.sgb.uygulama.gib {
    using System;
    using System.Xml.Serialization;
    using TNS.Soap;

    public partial class GIBServis : SoapIstemci {

        public GIBServis() {
            this.Url = global::HRCServisC.Properties.Settings.Default.HRCServisC_tr_gov_sgb_uygulama_GIBServis;
        }

        public FirmaBilgisi FirmaKisiBilgisiAl(string vergiNo, string tcKimlikNoKullanici, string kurumKod) {
            return (FirmaBilgisi)Cagir("http://uygulama.sgb.gov.tr/SGBServis/FirmaKisiBilgisiAl", "FirmaKisiBilgisiAl", "http://uygulama.sgb.gov.tr/SGBServis/", typeof(FirmaBilgisi), "FirmaKisiBilgisiAlResult",
                new SoapParametre("vergiNo", vergiNo, typeof(string)),
                new SoapParametre("tcKimlikNoKullanici", tcKimlikNoKullanici, typeof(string)),
                new SoapParametre("kurumKod", kurumKod, typeof(string)));
        }

        public borcuYokturKaydetOutput BorcuYokDilekceKaydet(borcuYokturKaydetInput dki, string tcKimlikNoKullanici, string kurumKod) {
            return (borcuYokturKaydetOutput)Cagir("http://uygulama.sgb.gov.tr/SGBServis/BorcuYokDilekceKaydet", "BorcuYokDilekceKaydet", "http://uygulama.sgb.gov.tr/SGBServis/", typeof(borcuYokturKaydetOutput), "BorcuYokDilekceKaydetResult",
                new SoapParametre("dki", dki, typeof(borcuYokturKaydetInput)),
                new SoapParametre("tcKimlikNoKullanici", tcKimlikNoKullanici, typeof(string)),
                new SoapParametre("kurumKod", kurumKod, typeof(string)));
        }

        public borcuYokturSorguOutput BorcuYokDilekceSorgulama(dilekceSorguInput dsi, string tcKimlikNoKullanici, string kurumKod) {
            return (borcuYokturSorguOutput)Cagir("http://uygulama.sgb.gov.tr/SGBServis/BorcuYokDilekceSorgulama", "BorcuYokDilekceSorgulama", "http://uygulama.sgb.gov.tr/SGBServis/", typeof(borcuYokturSorguOutput), "BorcuYokDilekceSorgulamaResult",
                new SoapParametre("dsi", dsi, typeof(dilekceSorguInput)),
                new SoapParametre("tcKimlikNoKullanici", tcKimlikNoKullanici, typeof(string)),
                new SoapParametre("kurumKod", kurumKod, typeof(string)));
        }

        public Fatura[] EFaturaSorgulama(string satici, string faturaID, string odenecekTutar, string tcKimlikNoKullanici, string kurumKod) {
            return (Fatura[])Cagir("http://uygulama.sgb.gov.tr/SGBServis/EFaturaSorgulama", "EFaturaSorgulama", "http://uygulama.sgb.gov.tr/SGBServis/", typeof(Fatura[]), "EFaturaSorgulamaResult",
                new SoapParametre("satici", satici, typeof(string)),
                new SoapParametre("faturaID", faturaID, typeof(string)),
                new SoapParametre("odenecekTutar", odenecekTutar, typeof(string)),
                new SoapParametre("tcKimlikNoKullanici", tcKimlikNoKullanici, typeof(string)),
                new SoapParametre("kurumKod", kurumKod, typeof(string)));
        }

    }

    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://uygulama.sgb.gov.tr/SGBServis/")]
    public partial class FirmaBilgisi {
        
        private int kodField;
        
        private int muhasebeField;
        
        private string birimField;
        
        private string adField;
        
        private string vergiNoField;
        
        private string vergiDairesiField;
        
        private string bankaField;
        
        private string hesapNoField;
        
        private string eMailField;
        
        private string firmaYetkilisiField;
        
        private string telefonField;
        
        private string faksField;
        
        private string adresField;
        
        private int uyrukField;
        
        private int gorulmesinField;
        
        private int yasakliField;
        
        private string cepTelField;
        
        private string webField;
        
        private int faturaField;
        
        private string parolaField;
        
        private TNSDateTime sonGirisTarihField;
        
        private string bicSwiftField;
        
        private string kisiUnvanField;
        
        private int ekGostergeField;
        
        private int dereceField;
        
        private string aciklamaField;
        
        private int aktifDurumField;
        
        private bool personeldeDeAraField;
        
        /// <remarks/>
        public int kod {
            get {
                return this.kodField;
            }
            set {
                this.kodField = value;
            }
        }
        
        /// <remarks/>
        public int muhasebe {
            get {
                return this.muhasebeField;
            }
            set {
                this.muhasebeField = value;
            }
        }
        
        /// <remarks/>
        public string birim {
            get {
                return this.birimField;
            }
            set {
                this.birimField = value;
            }
        }
        
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
        public string vergiNo {
            get {
                return this.vergiNoField;
            }
            set {
                this.vergiNoField = value;
            }
        }
        
        /// <remarks/>
        public string vergiDairesi {
            get {
                return this.vergiDairesiField;
            }
            set {
                this.vergiDairesiField = value;
            }
        }
        
        /// <remarks/>
        public string banka {
            get {
                return this.bankaField;
            }
            set {
                this.bankaField = value;
            }
        }
        
        /// <remarks/>
        public string hesapNo {
            get {
                return this.hesapNoField;
            }
            set {
                this.hesapNoField = value;
            }
        }
        
        /// <remarks/>
        public string eMail {
            get {
                return this.eMailField;
            }
            set {
                this.eMailField = value;
            }
        }
        
        /// <remarks/>
        public string firmaYetkilisi {
            get {
                return this.firmaYetkilisiField;
            }
            set {
                this.firmaYetkilisiField = value;
            }
        }
        
        /// <remarks/>
        public string telefon {
            get {
                return this.telefonField;
            }
            set {
                this.telefonField = value;
            }
        }
        
        /// <remarks/>
        public string faks {
            get {
                return this.faksField;
            }
            set {
                this.faksField = value;
            }
        }
        
        /// <remarks/>
        public string adres {
            get {
                return this.adresField;
            }
            set {
                this.adresField = value;
            }
        }
        
        /// <remarks/>
        public int uyruk {
            get {
                return this.uyrukField;
            }
            set {
                this.uyrukField = value;
            }
        }
        
        /// <remarks/>
        public int gorulmesin {
            get {
                return this.gorulmesinField;
            }
            set {
                this.gorulmesinField = value;
            }
        }
        
        /// <remarks/>
        public int yasakli {
            get {
                return this.yasakliField;
            }
            set {
                this.yasakliField = value;
            }
        }
        
        /// <remarks/>
        public string cepTel {
            get {
                return this.cepTelField;
            }
            set {
                this.cepTelField = value;
            }
        }
        
        /// <remarks/>
        public string web {
            get {
                return this.webField;
            }
            set {
                this.webField = value;
            }
        }
        
        /// <remarks/>
        public int fatura {
            get {
                return this.faturaField;
            }
            set {
                this.faturaField = value;
            }
        }
        
        /// <remarks/>
        public string parola {
            get {
                return this.parolaField;
            }
            set {
                this.parolaField = value;
            }
        }
        
        /// <remarks/>
        public TNSDateTime sonGirisTarih {
            get {
                return this.sonGirisTarihField;
            }
            set {
                this.sonGirisTarihField = value;
            }
        }
        
        /// <remarks/>
        public string bicSwift {
            get {
                return this.bicSwiftField;
            }
            set {
                this.bicSwiftField = value;
            }
        }
        
        /// <remarks/>
        public string kisiUnvan {
            get {
                return this.kisiUnvanField;
            }
            set {
                this.kisiUnvanField = value;
            }
        }
        
        /// <remarks/>
        public int ekGosterge {
            get {
                return this.ekGostergeField;
            }
            set {
                this.ekGostergeField = value;
            }
        }
        
        /// <remarks/>
        public int derece {
            get {
                return this.dereceField;
            }
            set {
                this.dereceField = value;
            }
        }
        
        /// <remarks/>
        public string aciklama {
            get {
                return this.aciklamaField;
            }
            set {
                this.aciklamaField = value;
            }
        }
        
        /// <remarks/>
        public int aktifDurum {
            get {
                return this.aktifDurumField;
            }
            set {
                this.aktifDurumField = value;
            }
        }
        
        /// <remarks/>
        public bool personeldeDeAra {
            get {
                return this.personeldeDeAraField;
            }
            set {
                this.personeldeDeAraField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://uygulama.sgb.gov.tr/SGBServis/")]
    public partial class TNSDateTime {
        
        private bool isNullField;
        
        /// <remarks/>
        public bool isNull {
            get {
                return this.isNullField;
            }
            set {
                this.isNullField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://uygulama.sgb.gov.tr/SGBServis/")]
    public partial class Fatura {
        
        private string adresField;
        
        private string aliciTCKNField;
        
        private string aliciVKNField;
        
        private string faturaDuzenlemeTarihiField;
        
        private string unvanField;
        
        /// <remarks/>
        public string adres {
            get {
                return this.adresField;
            }
            set {
                this.adresField = value;
            }
        }
        
        /// <remarks/>
        public string aliciTCKN {
            get {
                return this.aliciTCKNField;
            }
            set {
                this.aliciTCKNField = value;
            }
        }
        
        /// <remarks/>
        public string aliciVKN {
            get {
                return this.aliciVKNField;
            }
            set {
                this.aliciVKNField = value;
            }
        }
        
        /// <remarks/>
        public string faturaDuzenlemeTarihi {
            get {
                return this.faturaDuzenlemeTarihiField;
            }
            set {
                this.faturaDuzenlemeTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string unvan {
            get {
                return this.unvanField;
            }
            set {
                this.unvanField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.borcuyokturisemri.webservis.cs.com/")]
    public partial class cevapVermeyenVergiDairesi {
        
        private string vergiDairesiAdiField;
        
        private string vergiDairesiKoduField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string vergiDairesiAdi {
            get {
                return this.vergiDairesiAdiField;
            }
            set {
                this.vergiDairesiAdiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string vergiDairesiKodu {
            get {
                return this.vergiDairesiKoduField;
            }
            set {
                this.vergiDairesiKoduField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.borcuyokturisemri.webservis.cs.com/")]
    public partial class vergiDairesiBorcDetayi {
        
        private string ikincilSorumlulukField;
        
        private string miktarField;
        
        private string muvafakatField;
        
        private string vergiDairesiAdiField;
        
        private string vergiDairesiKoduField;
        
        private string vergiTuruField;
        
        private string yilField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ikincilSorumluluk {
            get {
                return this.ikincilSorumlulukField;
            }
            set {
                this.ikincilSorumlulukField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string miktar {
            get {
                return this.miktarField;
            }
            set {
                this.miktarField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string muvafakat {
            get {
                return this.muvafakatField;
            }
            set {
                this.muvafakatField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string vergiDairesiAdi {
            get {
                return this.vergiDairesiAdiField;
            }
            set {
                this.vergiDairesiAdiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string vergiDairesiKodu {
            get {
                return this.vergiDairesiKoduField;
            }
            set {
                this.vergiDairesiKoduField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string vergiTuru {
            get {
                return this.vergiTuruField;
            }
            set {
                this.vergiTuruField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string yil {
            get {
                return this.yilField;
            }
            set {
                this.yilField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.borcuyokturisemri.webservis.cs.com/")]
    public partial class borcuYokturSorguOutput {
        
        private string[] aciklamaField;
        
        private string adSoyadField;
        
        private vergiDairesiBorcDetayi[] borcDetaylariField;
        
        private bool borcuYokturField;
        
        private cevapVermeyenVergiDairesi[] cevapVermeyenVergiDaireleriField;
        
        private bool cevaplandiField;
        
        private string dilekceCevapTarihiField;
        
        private string dilekceIdField;
        
        private string dilekceTalepTarihiField;
        
        private durum durumField;
        
        private string kesintininAktarilacagiVergiDairesiAdiField;
        
        private string kesintininAktarilacagiVergiDairesiIBANField;
        
        private string kesintininAktarilacagiVergiDairesiKoduField;
        
        private string sorumluTutulmayacakToplamField;
        
        private string sorumluVeyaKesintiyeTabiTutulacakToplamField;
        
        private string talepNedeniField;
        
        private string tcKimlikNumarasiField;
        
        private string unvanField;
        
        private string vergiKimlikNumarasiField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("aciklama", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string[] aciklama {
            get {
                return this.aciklamaField;
            }
            set {
                this.aciklamaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string adSoyad {
            get {
                return this.adSoyadField;
            }
            set {
                this.adSoyadField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("borcDetaylari", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public vergiDairesiBorcDetayi[] borcDetaylari {
            get {
                return this.borcDetaylariField;
            }
            set {
                this.borcDetaylariField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool borcuYoktur {
            get {
                return this.borcuYokturField;
            }
            set {
                this.borcuYokturField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("cevapVermeyenVergiDaireleri", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public cevapVermeyenVergiDairesi[] cevapVermeyenVergiDaireleri {
            get {
                return this.cevapVermeyenVergiDaireleriField;
            }
            set {
                this.cevapVermeyenVergiDaireleriField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool cevaplandi {
            get {
                return this.cevaplandiField;
            }
            set {
                this.cevaplandiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string dilekceCevapTarihi {
            get {
                return this.dilekceCevapTarihiField;
            }
            set {
                this.dilekceCevapTarihiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string dilekceId {
            get {
                return this.dilekceIdField;
            }
            set {
                this.dilekceIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string dilekceTalepTarihi {
            get {
                return this.dilekceTalepTarihiField;
            }
            set {
                this.dilekceTalepTarihiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public durum durum {
            get {
                return this.durumField;
            }
            set {
                this.durumField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string kesintininAktarilacagiVergiDairesiAdi {
            get {
                return this.kesintininAktarilacagiVergiDairesiAdiField;
            }
            set {
                this.kesintininAktarilacagiVergiDairesiAdiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string kesintininAktarilacagiVergiDairesiIBAN {
            get {
                return this.kesintininAktarilacagiVergiDairesiIBANField;
            }
            set {
                this.kesintininAktarilacagiVergiDairesiIBANField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string kesintininAktarilacagiVergiDairesiKodu {
            get {
                return this.kesintininAktarilacagiVergiDairesiKoduField;
            }
            set {
                this.kesintininAktarilacagiVergiDairesiKoduField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string sorumluTutulmayacakToplam {
            get {
                return this.sorumluTutulmayacakToplamField;
            }
            set {
                this.sorumluTutulmayacakToplamField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string sorumluVeyaKesintiyeTabiTutulacakToplam {
            get {
                return this.sorumluVeyaKesintiyeTabiTutulacakToplamField;
            }
            set {
                this.sorumluVeyaKesintiyeTabiTutulacakToplamField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string talepNedeni {
            get {
                return this.talepNedeniField;
            }
            set {
                this.talepNedeniField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string tcKimlikNumarasi {
            get {
                return this.tcKimlikNumarasiField;
            }
            set {
                this.tcKimlikNumarasiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string unvan {
            get {
                return this.unvanField;
            }
            set {
                this.unvanField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string vergiKimlikNumarasi {
            get {
                return this.vergiKimlikNumarasiField;
            }
            set {
                this.vergiKimlikNumarasiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.borcuyokturisemri.webservis.cs.com/")]
    public partial class durum {
        
        private string durumKodAciklamasiField;
        
        private string durumKoduField;
        
        private string hataDetayBilgisiField;
        
        private bool sonucField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string durumKodAciklamasi {
            get {
                return this.durumKodAciklamasiField;
            }
            set {
                this.durumKodAciklamasiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string durumKodu {
            get {
                return this.durumKoduField;
            }
            set {
                this.durumKoduField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string hataDetayBilgisi {
            get {
                return this.hataDetayBilgisiField;
            }
            set {
                this.hataDetayBilgisiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool sonuc {
            get {
                return this.sonucField;
            }
            set {
                this.sonucField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.borcuyokturisemri.webservis.cs.com/")]
    public partial class dilekceSorguInput {
        
        private string dilekceIDField;
        
        private string kurumKoduField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string dilekceID {
            get {
                return this.dilekceIDField;
            }
            set {
                this.dilekceIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string kurumKodu {
            get {
                return this.kurumKoduField;
            }
            set {
                this.kurumKoduField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.borcuyokturisemri.webservis.cs.com/")]
    public partial class borcuYokturKaydetOutput {
        
        private durum durumField;
        
        private string idField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public durum durum {
            get {
                return this.durumField;
            }
            set {
                this.durumField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ID {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.borcuyokturisemri.webservis.cs.com/")]
    public partial class borcuYokturKaydetInput {
        
        private string dilekceBaslangicTarihiField;
        
        private string dilekceBitisTarihiField;
        
        private string kurumKoduField;
        
        private string tcKimlikNumarasiField;
        
        private string vergiKimlikNumarasiField;
        
        private string yaziTalepNedeniField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string dilekceBaslangicTarihi {
            get {
                return this.dilekceBaslangicTarihiField;
            }
            set {
                this.dilekceBaslangicTarihiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string dilekceBitisTarihi {
            get {
                return this.dilekceBitisTarihiField;
            }
            set {
                this.dilekceBitisTarihiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string kurumKodu {
            get {
                return this.kurumKoduField;
            }
            set {
                this.kurumKoduField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string tcKimlikNumarasi {
            get {
                return this.tcKimlikNumarasiField;
            }
            set {
                this.tcKimlikNumarasiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string vergiKimlikNumarasi {
            get {
                return this.vergiKimlikNumarasiField;
            }
            set {
                this.vergiKimlikNumarasiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string yaziTalepNedeni {
            get {
                return this.yaziTalepNedeniField;
            }
            set {
                this.yaziTalepNedeniField = value;
            }
        }
    }
}

#pragma warning restore 1591
