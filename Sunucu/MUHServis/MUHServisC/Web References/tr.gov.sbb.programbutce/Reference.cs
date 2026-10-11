//------------------------------------------------------------------------------
// OdenekGondermeTenkisServis web servisi istemcisi.
//
// Bu dosya, Visual Studio'nun .NET Framework için ürettiği eski Reference.cs dosyasından
// _araclar/asmx_istemci_uret.py ile dönüştürülmüştür. Proxy sınıfı TNS.Soap.SoapIstemci tabanlıdır
// (HttpClient + XmlSerializer); veri sınıfları eski üretilmiş koddan değiştirilmeden alınmıştır.
//------------------------------------------------------------------------------
#pragma warning disable 1591
namespace MUHServisC.tr.gov.sbb.programbutce {
    using System;
    using System.Xml.Serialization;
    using TNS.Soap;

    public partial class OdenekGondermeTenkisServis : SoapIstemci {

        public OdenekGondermeTenkisServis() {
            this.Url = global::MUHServisC.Properties.Settings.Default.MUHServisC_tr_gov_sbb_programbutce_OdenekGondermeTenkisServis;
        }

        public Sonuc OdenekGondermeKaydet(string kullaniciKodu, string parola, GondermeTenkisFormW form) {
            return (Sonuc)Cagir("http://tempuri.org/OdenekGondermeKaydet", "OdenekGondermeKaydet", "http://tempuri.org/", typeof(Sonuc), "OdenekGondermeKaydetResult",
                new SoapParametre("kullaniciKodu", kullaniciKodu, typeof(string)),
                new SoapParametre("parola", parola, typeof(string)),
                new SoapParametre("form", form, typeof(GondermeTenkisFormW)));
        }

        public Sonuc OdenekGondermeKontrol(string kullaniciKodu, string parola, GondermeTenkisFormW form) {
            return (Sonuc)Cagir("http://tempuri.org/OdenekGondermeKontrol", "OdenekGondermeKontrol", "http://tempuri.org/", typeof(Sonuc), "OdenekGondermeKontrolResult",
                new SoapParametre("kullaniciKodu", kullaniciKodu, typeof(string)),
                new SoapParametre("parola", parola, typeof(string)),
                new SoapParametre("form", form, typeof(GondermeTenkisFormW)));
        }

        public GondermeTenkisFormW OdenekGondermeBelgeDurum(string kullaniciKodu, string parola, GondermeTenkisFormW form) {
            return (GondermeTenkisFormW)Cagir("http://tempuri.org/OdenekGondermeBelgeDurum", "OdenekGondermeBelgeDurum", "http://tempuri.org/", typeof(GondermeTenkisFormW), "OdenekGondermeBelgeDurumResult",
                new SoapParametre("kullaniciKodu", kullaniciKodu, typeof(string)),
                new SoapParametre("parola", parola, typeof(string)),
                new SoapParametre("form", form, typeof(GondermeTenkisFormW)));
        }

    }

    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class GondermeTenkisFormW {
        
        private int yilField;
        
        private EnumodenekIslemTuruW belgeTurField;
        
        private long belgeNoField;
        
        private string kurumField;
        
        private string kurumAdField;
        
        private int muhasebeField;
        
        private string muhasebeAdField;
        
        private string vknField;
        
        private string vknAdField;
        
        private string aciklamaField;
        
        private string yaziNoField;
        
        private string yaziTarihField;
        
        private string istekYaziNoField;
        
        private string istekYaziTarihField;
        
        private int formDurumField;
        
        private string formDurumTarihField;
        
        private string islemYapanField;
        
        private string iptalNedenField;
        
        private int icmalNoField;
        
        private string icmalTarihField;
        
        private string onayNoField;
        
        private string onayTarihField;
        
        private string islemTarihField;
        
        private int ayField;
        
        private GondermeTenkisDetayW[] detayField;
        
        /// <remarks/>
        public int yil {
            get {
                return this.yilField;
            }
            set {
                this.yilField = value;
            }
        }
        
        /// <remarks/>
        public EnumodenekIslemTuruW belgeTur {
            get {
                return this.belgeTurField;
            }
            set {
                this.belgeTurField = value;
            }
        }
        
        /// <remarks/>
        public long belgeNo {
            get {
                return this.belgeNoField;
            }
            set {
                this.belgeNoField = value;
            }
        }
        
        /// <remarks/>
        public string kurum {
            get {
                return this.kurumField;
            }
            set {
                this.kurumField = value;
            }
        }
        
        /// <remarks/>
        public string kurumAd {
            get {
                return this.kurumAdField;
            }
            set {
                this.kurumAdField = value;
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
        public string muhasebeAd {
            get {
                return this.muhasebeAdField;
            }
            set {
                this.muhasebeAdField = value;
            }
        }
        
        /// <remarks/>
        public string vkn {
            get {
                return this.vknField;
            }
            set {
                this.vknField = value;
            }
        }
        
        /// <remarks/>
        public string vknAd {
            get {
                return this.vknAdField;
            }
            set {
                this.vknAdField = value;
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
        public string yaziNo {
            get {
                return this.yaziNoField;
            }
            set {
                this.yaziNoField = value;
            }
        }
        
        /// <remarks/>
        public string yaziTarih {
            get {
                return this.yaziTarihField;
            }
            set {
                this.yaziTarihField = value;
            }
        }
        
        /// <remarks/>
        public string istekYaziNo {
            get {
                return this.istekYaziNoField;
            }
            set {
                this.istekYaziNoField = value;
            }
        }
        
        /// <remarks/>
        public string istekYaziTarih {
            get {
                return this.istekYaziTarihField;
            }
            set {
                this.istekYaziTarihField = value;
            }
        }
        
        /// <remarks/>
        public int formDurum {
            get {
                return this.formDurumField;
            }
            set {
                this.formDurumField = value;
            }
        }
        
        /// <remarks/>
        public string formDurumTarih {
            get {
                return this.formDurumTarihField;
            }
            set {
                this.formDurumTarihField = value;
            }
        }
        
        /// <remarks/>
        public string islemYapan {
            get {
                return this.islemYapanField;
            }
            set {
                this.islemYapanField = value;
            }
        }
        
        /// <remarks/>
        public string iptalNeden {
            get {
                return this.iptalNedenField;
            }
            set {
                this.iptalNedenField = value;
            }
        }
        
        /// <remarks/>
        public int icmalNo {
            get {
                return this.icmalNoField;
            }
            set {
                this.icmalNoField = value;
            }
        }
        
        /// <remarks/>
        public string icmalTarih {
            get {
                return this.icmalTarihField;
            }
            set {
                this.icmalTarihField = value;
            }
        }
        
        /// <remarks/>
        public string onayNo {
            get {
                return this.onayNoField;
            }
            set {
                this.onayNoField = value;
            }
        }
        
        /// <remarks/>
        public string onayTarih {
            get {
                return this.onayTarihField;
            }
            set {
                this.onayTarihField = value;
            }
        }
        
        /// <remarks/>
        public string islemTarih {
            get {
                return this.islemTarihField;
            }
            set {
                this.islemTarihField = value;
            }
        }
        
        /// <remarks/>
        public int ay {
            get {
                return this.ayField;
            }
            set {
                this.ayField = value;
            }
        }
        
        /// <remarks/>
        public GondermeTenkisDetayW[] detay {
            get {
                return this.detayField;
            }
            set {
                this.detayField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public enum EnumodenekIslemTuruW : int{
        
        /// <remarks/>
        ODENEK = 3,
        
        /// <remarks/>
        TENKIS = 4,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class GondermeTenkisDetayW {
        
        private string tertipField;
        
        private double miktarField;
        
        /// <remarks/>
        public string tertip {
            get {
                return this.tertipField;
            }
            set {
                this.tertipField = value;
            }
        }
        
        /// <remarks/>
        public double miktar {
            get {
                return this.miktarField;
            }
            set {
                this.miktarField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class Sonuc {
        
        private bool islemSonucField;
        
        private string hataStrField;
        
        private string bilgiStrField;
        
        private int kayitSayField;
        
        private string anahtarField;
        
        private object tagField;
        
        /// <remarks/>
        public bool islemSonuc {
            get {
                return this.islemSonucField;
            }
            set {
                this.islemSonucField = value;
            }
        }
        
        /// <remarks/>
        public string hataStr {
            get {
                return this.hataStrField;
            }
            set {
                this.hataStrField = value;
            }
        }
        
        /// <remarks/>
        public string bilgiStr {
            get {
                return this.bilgiStrField;
            }
            set {
                this.bilgiStrField = value;
            }
        }
        
        /// <remarks/>
        public int kayitSay {
            get {
                return this.kayitSayField;
            }
            set {
                this.kayitSayField = value;
            }
        }
        
        /// <remarks/>
        public string anahtar {
            get {
                return this.anahtarField;
            }
            set {
                this.anahtarField = value;
            }
        }
        
        /// <remarks/>
        public object tag {
            get {
                return this.tagField;
            }
            set {
                this.tagField = value;
            }
        }
    }
}

#pragma warning restore 1591
