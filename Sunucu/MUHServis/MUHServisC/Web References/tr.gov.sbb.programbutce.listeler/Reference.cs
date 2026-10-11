//------------------------------------------------------------------------------
// Listeler web servisi istemcisi.
//
// Bu dosya, Visual Studio'nun .NET Framework için ürettiği eski Reference.cs dosyasından
// _araclar/asmx_istemci_uret.py ile dönüştürülmüştür. Proxy sınıfı TNS.Soap.SoapIstemci tabanlıdır
// (HttpClient + XmlSerializer); veri sınıfları eski üretilmiş koddan değiştirilmeden alınmıştır.
//------------------------------------------------------------------------------
#pragma warning disable 1591
namespace MUHServisC.tr.gov.sbb.programbutce.listeler {
    using System;
    using System.Xml.Serialization;
    using TNS.Soap;

    public partial class Listeler : SoapIstemci {

        public Listeler() {
            this.Url = global::MUHServisC.Properties.Settings.Default.MUHServisC_tr_gov_sbb_programbutce_listeler_Listeler;
        }

        public ABSKodlarW[] KodListele(int yil, EnumKodTurleriW kodTuru, string ustKod) {
            return (ABSKodlarW[])Cagir("http://tempuri.org/KodListele", "KodListele", "http://tempuri.org/", typeof(ABSKodlarW[]), "KodListeleResult",
                new SoapParametre("yil", yil, typeof(int)),
                new SoapParametre("kodTuru", kodTuru, typeof(EnumKodTurleriW)),
                new SoapParametre("ustKod", ustKod, typeof(string)));
        }

        public ABSKodlarW[] DegisenKodListele(int yil, EnumKodTurleriW kodTuru, System.DateTime degisiklikZamani) {
            return (ABSKodlarW[])Cagir("http://tempuri.org/DegisenKodListele", "DegisenKodListele", "http://tempuri.org/", typeof(ABSKodlarW[]), "DegisenKodListeleResult",
                new SoapParametre("yil", yil, typeof(int)),
                new SoapParametre("kodTuru", kodTuru, typeof(EnumKodTurleriW)),
                new SoapParametre("degisiklikZamani", degisiklikZamani, typeof(System.DateTime)));
        }

        public ABSKodlarWI[] PAFListele(int yil, EnumPAFTurW kodTuru, string kurumKod, string kullaniciKodu, string parola) {
            return (ABSKodlarWI[])Cagir("http://tempuri.org/PAFListele", "PAFListele", "http://tempuri.org/", typeof(ABSKodlarWI[]), "PAFListeleResult",
                new SoapParametre("yil", yil, typeof(int)),
                new SoapParametre("kodTuru", kodTuru, typeof(EnumPAFTurW)),
                new SoapParametre("kurumKod", kurumKod, typeof(string)),
                new SoapParametre("kullaniciKodu", kullaniciKodu, typeof(string)),
                new SoapParametre("parola", parola, typeof(string)));
        }

        public ABSKodlarWI PAFBilgiGetir(int yil, EnumPAFTurW kodTuru, long PAFKod, string kullaniciKodu, string parola) {
            return (ABSKodlarWI)Cagir("http://tempuri.org/PAFBilgiGetir", "PAFBilgiGetir", "http://tempuri.org/", typeof(ABSKodlarWI), "PAFBilgiGetirResult",
                new SoapParametre("yil", yil, typeof(int)),
                new SoapParametre("kodTuru", kodTuru, typeof(EnumPAFTurW)),
                new SoapParametre("PAFKod", PAFKod, typeof(long)),
                new SoapParametre("kullaniciKodu", kullaniciKodu, typeof(string)),
                new SoapParametre("parola", parola, typeof(string)));
        }

        public ABSKodlarW KodBilgiGetir(EnumKodTurleriW kodTuru, string kod) {
            return (ABSKodlarW)Cagir("http://tempuri.org/KodBilgiGetir", "KodBilgiGetir", "http://tempuri.org/", typeof(ABSKodlarW), "KodBilgiGetirResult",
                new SoapParametre("kodTuru", kodTuru, typeof(EnumKodTurleriW)),
                new SoapParametre("kod", kod, typeof(string)));
        }

        public ABSKodlarWI[] DegisenPAFListele(int yil, EnumPAFTurW kodTuru, string kurumKod, System.DateTime degisiklikZamani, string kullaniciKodu, string parola) {
            return (ABSKodlarWI[])Cagir("http://tempuri.org/DegisenPAFListele", "DegisenPAFListele", "http://tempuri.org/", typeof(ABSKodlarWI[]), "DegisenPAFListeleResult",
                new SoapParametre("yil", yil, typeof(int)),
                new SoapParametre("kodTuru", kodTuru, typeof(EnumPAFTurW)),
                new SoapParametre("kurumKod", kurumKod, typeof(string)),
                new SoapParametre("degisiklikZamani", degisiklikZamani, typeof(System.DateTime)),
                new SoapParametre("kullaniciKodu", kullaniciKodu, typeof(string)),
                new SoapParametre("parola", parola, typeof(string)));
        }

        public ABSKodlarWI[] IliskiliAltFaaliyetListele(string kullaniciKodu, string parola, int yil, string kurumKod, string ekoKod) {
            return (ABSKodlarWI[])Cagir("http://tempuri.org/IliskiliAltFaaliyetListele", "IliskiliAltFaaliyetListele", "http://tempuri.org/", typeof(ABSKodlarWI[]), "IliskiliAltFaaliyetListeleResult",
                new SoapParametre("kullaniciKodu", kullaniciKodu, typeof(string)),
                new SoapParametre("parola", parola, typeof(string)),
                new SoapParametre("yil", yil, typeof(int)),
                new SoapParametre("kurumKod", kurumKod, typeof(string)),
                new SoapParametre("ekoKod", ekoKod, typeof(string)));
        }

        public long[] ButceIslemiOzetBilgiSorgula(string kullaniciKodu, string parola, int yil, System.DateTime baslangictarihi, System.DateTime bitisTarihi, string kurum, string islemTuru) {
            return (long[])Cagir("http://tempuri.org/ButceIslemiOzetBilgiSorgula", "ButceIslemiOzetBilgiSorgula", "http://tempuri.org/", typeof(long[]), "ButceIslemiOzetBilgiSorgulaResult",
                new SoapParametre("kullaniciKodu", kullaniciKodu, typeof(string)),
                new SoapParametre("parola", parola, typeof(string)),
                new SoapParametre("yil", yil, typeof(int)),
                new SoapParametre("baslangictarihi", baslangictarihi, typeof(System.DateTime)),
                new SoapParametre("bitisTarihi", bitisTarihi, typeof(System.DateTime)),
                new SoapParametre("kurum", kurum, typeof(string)),
                new SoapParametre("islemTuru", islemTuru, typeof(string)));
        }

        public ButceIslemBilgi[] butceIslemiDetayBilgileriniSorgulaTek(string kullaniciKodu, string parola, long formId) {
            return (ButceIslemBilgi[])Cagir("http://tempuri.org/butceIslemiDetayBilgileriniSorgulaTek", "butceIslemiDetayBilgileriniSorgulaTek", "http://tempuri.org/", typeof(ButceIslemBilgi[]), "butceIslemiDetayBilgileriniSorgulaTekResult",
                new SoapParametre("kullaniciKodu", kullaniciKodu, typeof(string)),
                new SoapParametre("parola", parola, typeof(string)),
                new SoapParametre("formId", formId, typeof(long)));
        }

        public ButceIslemBilgi[] butceIslemiDetayBilgileriniSorgula(string kullaniciKodu, string parola, long[] butceIdler) {
            return (ButceIslemBilgi[])Cagir("http://tempuri.org/butceIslemiDetayBilgileriniSorgula", "butceIslemiDetayBilgileriniSorgula", "http://tempuri.org/", typeof(ButceIslemBilgi[]), "butceIslemiDetayBilgileriniSorgulaResult",
                new SoapParametre("kullaniciKodu", kullaniciKodu, typeof(string)),
                new SoapParametre("parola", parola, typeof(string)),
                new SoapParametre("butceIdler", butceIdler, typeof(long[])));
        }

        public OdenekHarcamaTahminWI[] HarcamaListesi(string kullaniciKodu, string parola, int yil, int ay, string kurKod) {
            return (OdenekHarcamaTahminWI[])Cagir("http://tempuri.org/HarcamaListesi", "HarcamaListesi", "http://tempuri.org/", typeof(OdenekHarcamaTahminWI[]), "HarcamaListesiResult",
                new SoapParametre("kullaniciKodu", kullaniciKodu, typeof(string)),
                new SoapParametre("parola", parola, typeof(string)),
                new SoapParametre("yil", yil, typeof(int)),
                new SoapParametre("ay", ay, typeof(int)),
                new SoapParametre("kurKod", kurKod, typeof(string)));
        }

    }

    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public enum EnumKodTurleriW {
        
        /// <remarks/>
        Kurumsal,
        
        /// <remarks/>
        Fonksiyonel,
        
        /// <remarks/>
        FinansTipi,
        
        /// <remarks/>
        Ekonomik,
        
        /// <remarks/>
        GelirEkonomik,
        
        /// <remarks/>
        FinansmanEkonomik,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ABSKodlarW {
        
        private string kodField;
        
        private string adField;
        
        private ENUMAktiflik durumField;
        
        /// <remarks/>
        public string kod {
            get {
                return this.kodField;
            }
            set {
                this.kodField = value;
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
        public ENUMAktiflik durum {
            get {
                return this.durumField;
            }
            set {
                this.durumField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public enum ENUMAktiflik {
        
        /// <remarks/>
        PASIF,
        
        /// <remarks/>
        AKTIF,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class OdenekHarcamaTahminWI {
        
        private string vknField;
        
        private string hesapKodField;
        
        private double harcamaField;
        
        private bool harcamaFieldSpecified;
        
        private double eklenenField;
        
        private bool eklenenFieldSpecified;
        
        private double dusulenField;
        
        private bool dusulenFieldSpecified;
        
        private long programIdField;
        
        private int programTurField;
        
        private long altProgramIdField;
        
        private long faaliyetIdField;
        
        private long altFaaliyetIdField;
        
        private string kurKodField;
        
        private string fonKodField;
        
        private string finKodField;
        
        private string ekoKodField;
        
        private string sektorKodField;
        
        private long projeIdField;
        
        private bool projeIdFieldSpecified;
        
        private string dptProjeNoField;
        
        private long altProjeIdField;
        
        private bool altProjeIdFieldSpecified;
        
        private string fisAciklamaField;
        
        private string fisBaslangicFarkSebepField;
        
        private string fisOncekiYilFarkSebepField;
        
        private string tavanUstuTalepSebepField;
        
        private double teklifField;
        
        private bool teklifFieldSpecified;
        
        private double tahmin1Field;
        
        private bool tahmin1FieldSpecified;
        
        private double tahmin2Field;
        
        private bool tahmin2FieldSpecified;
        
        private double yshtField;
        
        private bool yshtFieldSpecified;
        
        private double odenekUstuField;
        
        private bool odenekUstuFieldSpecified;
        
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
        public string hesapKod {
            get {
                return this.hesapKodField;
            }
            set {
                this.hesapKodField = value;
            }
        }
        
        /// <remarks/>
        public double harcama {
            get {
                return this.harcamaField;
            }
            set {
                this.harcamaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool harcamaSpecified {
            get {
                return this.harcamaFieldSpecified;
            }
            set {
                this.harcamaFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public double eklenen {
            get {
                return this.eklenenField;
            }
            set {
                this.eklenenField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool eklenenSpecified {
            get {
                return this.eklenenFieldSpecified;
            }
            set {
                this.eklenenFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public double dusulen {
            get {
                return this.dusulenField;
            }
            set {
                this.dusulenField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool dusulenSpecified {
            get {
                return this.dusulenFieldSpecified;
            }
            set {
                this.dusulenFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public long ProgramId {
            get {
                return this.programIdField;
            }
            set {
                this.programIdField = value;
            }
        }
        
        /// <remarks/>
        public int ProgramTur {
            get {
                return this.programTurField;
            }
            set {
                this.programTurField = value;
            }
        }
        
        /// <remarks/>
        public long AltProgramId {
            get {
                return this.altProgramIdField;
            }
            set {
                this.altProgramIdField = value;
            }
        }
        
        /// <remarks/>
        public long FaaliyetId {
            get {
                return this.faaliyetIdField;
            }
            set {
                this.faaliyetIdField = value;
            }
        }
        
        /// <remarks/>
        public long AltFaaliyetId {
            get {
                return this.altFaaliyetIdField;
            }
            set {
                this.altFaaliyetIdField = value;
            }
        }
        
        /// <remarks/>
        public string KurKod {
            get {
                return this.kurKodField;
            }
            set {
                this.kurKodField = value;
            }
        }
        
        /// <remarks/>
        public string FonKod {
            get {
                return this.fonKodField;
            }
            set {
                this.fonKodField = value;
            }
        }
        
        /// <remarks/>
        public string FinKod {
            get {
                return this.finKodField;
            }
            set {
                this.finKodField = value;
            }
        }
        
        /// <remarks/>
        public string EkoKod {
            get {
                return this.ekoKodField;
            }
            set {
                this.ekoKodField = value;
            }
        }
        
        /// <remarks/>
        public string SektorKod {
            get {
                return this.sektorKodField;
            }
            set {
                this.sektorKodField = value;
            }
        }
        
        /// <remarks/>
        public long ProjeId {
            get {
                return this.projeIdField;
            }
            set {
                this.projeIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ProjeIdSpecified {
            get {
                return this.projeIdFieldSpecified;
            }
            set {
                this.projeIdFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public string DptProjeNo {
            get {
                return this.dptProjeNoField;
            }
            set {
                this.dptProjeNoField = value;
            }
        }
        
        /// <remarks/>
        public long AltProjeId {
            get {
                return this.altProjeIdField;
            }
            set {
                this.altProjeIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AltProjeIdSpecified {
            get {
                return this.altProjeIdFieldSpecified;
            }
            set {
                this.altProjeIdFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public string FisAciklama {
            get {
                return this.fisAciklamaField;
            }
            set {
                this.fisAciklamaField = value;
            }
        }
        
        /// <remarks/>
        public string FisBaslangicFarkSebep {
            get {
                return this.fisBaslangicFarkSebepField;
            }
            set {
                this.fisBaslangicFarkSebepField = value;
            }
        }
        
        /// <remarks/>
        public string FisOncekiYilFarkSebep {
            get {
                return this.fisOncekiYilFarkSebepField;
            }
            set {
                this.fisOncekiYilFarkSebepField = value;
            }
        }
        
        /// <remarks/>
        public string TavanUstuTalepSebep {
            get {
                return this.tavanUstuTalepSebepField;
            }
            set {
                this.tavanUstuTalepSebepField = value;
            }
        }
        
        /// <remarks/>
        public double teklif {
            get {
                return this.teklifField;
            }
            set {
                this.teklifField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool teklifSpecified {
            get {
                return this.teklifFieldSpecified;
            }
            set {
                this.teklifFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public double tahmin1 {
            get {
                return this.tahmin1Field;
            }
            set {
                this.tahmin1Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tahmin1Specified {
            get {
                return this.tahmin1FieldSpecified;
            }
            set {
                this.tahmin1FieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public double tahmin2 {
            get {
                return this.tahmin2Field;
            }
            set {
                this.tahmin2Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tahmin2Specified {
            get {
                return this.tahmin2FieldSpecified;
            }
            set {
                this.tahmin2FieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public double ysht {
            get {
                return this.yshtField;
            }
            set {
                this.yshtField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool yshtSpecified {
            get {
                return this.yshtFieldSpecified;
            }
            set {
                this.yshtFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public double odenekUstu {
            get {
                return this.odenekUstuField;
            }
            set {
                this.odenekUstuField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool odenekUstuSpecified {
            get {
                return this.odenekUstuFieldSpecified;
            }
            set {
                this.odenekUstuFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ButceIslemSatir {
        
        private string altislemKoduField;
        
        private string tertipField;
        
        private double tutarField;
        
        private string aciklamaField;
        
        /// <remarks/>
        public string altislemKodu {
            get {
                return this.altislemKoduField;
            }
            set {
                this.altislemKoduField = value;
            }
        }
        
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
        public double Tutar {
            get {
                return this.tutarField;
            }
            set {
                this.tutarField = value;
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
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ButceIslemBilgi {
        
        private System.DateTime islemTarihField;
        
        private string islemKoduField;
        
        private EnumIslemTipi islemTipiField;
        
        private long islemIdField;
        
        private int yilField;
        
        private int islemNoField;
        
        private string islemyapanKurumField;
        
        private string aciklamaField;
        
        private long iptalEdilenIslemIdField;
        
        private ButceIslemSatir[] giderlerField;
        
        private ButceIslemSatir[] gelirlerField;
        
        private ButceIslemSatir[] finansmanlarField;
        
        /// <remarks/>
        public System.DateTime islemTarih {
            get {
                return this.islemTarihField;
            }
            set {
                this.islemTarihField = value;
            }
        }
        
        /// <remarks/>
        public string islemKodu {
            get {
                return this.islemKoduField;
            }
            set {
                this.islemKoduField = value;
            }
        }
        
        /// <remarks/>
        public EnumIslemTipi islemTipi {
            get {
                return this.islemTipiField;
            }
            set {
                this.islemTipiField = value;
            }
        }
        
        /// <remarks/>
        public long islemId {
            get {
                return this.islemIdField;
            }
            set {
                this.islemIdField = value;
            }
        }
        
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
        public int islemNo {
            get {
                return this.islemNoField;
            }
            set {
                this.islemNoField = value;
            }
        }
        
        /// <remarks/>
        public string islemyapanKurum {
            get {
                return this.islemyapanKurumField;
            }
            set {
                this.islemyapanKurumField = value;
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
        public long iptalEdilenIslemId {
            get {
                return this.iptalEdilenIslemIdField;
            }
            set {
                this.iptalEdilenIslemIdField = value;
            }
        }
        
        /// <remarks/>
        public ButceIslemSatir[] giderler {
            get {
                return this.giderlerField;
            }
            set {
                this.giderlerField = value;
            }
        }
        
        /// <remarks/>
        public ButceIslemSatir[] gelirler {
            get {
                return this.gelirlerField;
            }
            set {
                this.gelirlerField = value;
            }
        }
        
        /// <remarks/>
        public ButceIslemSatir[] finansmanlar {
            get {
                return this.finansmanlarField;
            }
            set {
                this.finansmanlarField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public enum EnumIslemTipi {
        
        /// <remarks/>
        HATA,
        
        /// <remarks/>
        EKLEME,
        
        /// <remarks/>
        IPTAL,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ABSKodlarWI {
        
        private long idField;
        
        private string adField;
        
        private ENUMAktiflik durumField;
        
        private string programSorumluKurumKodField;
        
        private long aFProgramIdField;
        
        private long aFAltProgramIdField;
        
        private long aFFaaliyetIdField;
        
        private string aFFonksiyonKodField;
        
        private string aFKurKodField;
        
        /// <remarks/>
        public long id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
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
        public ENUMAktiflik durum {
            get {
                return this.durumField;
            }
            set {
                this.durumField = value;
            }
        }
        
        /// <remarks/>
        public string programSorumluKurumKod {
            get {
                return this.programSorumluKurumKodField;
            }
            set {
                this.programSorumluKurumKodField = value;
            }
        }
        
        /// <remarks/>
        public long AFProgramId {
            get {
                return this.aFProgramIdField;
            }
            set {
                this.aFProgramIdField = value;
            }
        }
        
        /// <remarks/>
        public long AFAltProgramId {
            get {
                return this.aFAltProgramIdField;
            }
            set {
                this.aFAltProgramIdField = value;
            }
        }
        
        /// <remarks/>
        public long AFFaaliyetId {
            get {
                return this.aFFaaliyetIdField;
            }
            set {
                this.aFFaaliyetIdField = value;
            }
        }
        
        /// <remarks/>
        public string AFFonksiyonKod {
            get {
                return this.aFFonksiyonKodField;
            }
            set {
                this.aFFonksiyonKodField = value;
            }
        }
        
        /// <remarks/>
        public string AFKurKod {
            get {
                return this.aFKurKodField;
            }
            set {
                this.aFKurKodField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public enum EnumPAFTurW {
        
        /// <remarks/>
        AltFaaliyet,
        
        /// <remarks/>
        Program,
        
        /// <remarks/>
        AltProgram,
        
        /// <remarks/>
        Faaliyet,
    }
}

#pragma warning restore 1591
