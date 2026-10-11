//------------------------------------------------------------------------------
// SuFatura web servisi istemcisi.
//
// Bu dosya, Visual Studio'nun .NET Framework için ürettiği eski Reference.cs dosyasından
// _araclar/asmx_istemci_uret.py ile dönüştürülmüştür. Proxy sınıfı TNS.Soap.SoapIstemci tabanlıdır
// (HttpClient + XmlSerializer); veri sınıfları eski üretilmiş koddan değiştirilmeden alınmıştır.
//------------------------------------------------------------------------------
#pragma warning disable 1591
namespace HRCServisC.tr.gov.sgb.uygulama.su {
    using System;
    using System.Xml.Serialization;
    using TNS.Soap;

    public partial class SuFatura : SoapIstemci {

        public SuFatura() {
            this.Url = global::HRCServisC.Properties.Settings.Default.HRCServisC_tr_gov_sgb_uygulama_su_SuFatura;
        }

        public System.Data.DataTable AskiFaturaSorgula(string aboneNo, string donem) {
            return (System.Data.DataTable)Cagir("http://uygulama.sgb.gov.tr/SGBServis/AskiFaturaSorgula", "AskiFaturaSorgula", "http://uygulama.sgb.gov.tr/SGBServis/", typeof(System.Data.DataTable), "AskiFaturaSorgulaResult",
                new SoapParametre("aboneNo", aboneNo, typeof(string)),
                new SoapParametre("donem", donem, typeof(string)));
        }

        public System.Data.DataTable BuskiFaturaSorgula(string aboneNo, string donem) {
            return (System.Data.DataTable)Cagir("http://uygulama.sgb.gov.tr/SGBServis/BuskiFaturaSorgula", "BuskiFaturaSorgula", "http://uygulama.sgb.gov.tr/SGBServis/", typeof(System.Data.DataTable), "BuskiFaturaSorgulaResult",
                new SoapParametre("aboneNo", aboneNo, typeof(string)),
                new SoapParametre("donem", donem, typeof(string)));
        }

        public sonFatura IskiFaturaSorgula(string aboneNo, string donem) {
            return (sonFatura)Cagir("http://uygulama.sgb.gov.tr/SGBServis/IskiFaturaSorgula", "IskiFaturaSorgula", "http://uygulama.sgb.gov.tr/SGBServis/", typeof(sonFatura), "IskiFaturaSorgulaResult",
                new SoapParametre("aboneNo", aboneNo, typeof(string)),
                new SoapParametre("donem", donem, typeof(string)));
        }

    }

    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class sonFatura {
        
        private string adSoyadField;
        
        private string mukaveleNoField;
        
        private string hesapKesimTarihiField;
        
        private string faturaTuruField;
        
        private string adresField;
        
        private string binaKimlikNoField;
        
        private string donemField;
        
        private string grup1Field;
        
        private string grup2Field;
        
        private string evIsyeriField;
        
        private string ilkOkumaTarihiField;
        
        private string ilkOkumaIsaretiField;
        
        private string ilkOkumaSayacDurKoduField;
        
        private string ilkOkumaOkuyucuKoduField;
        
        private string ilkOkumaSayacBilgisiField;
        
        private string sonOkumaTarihiField;
        
        private string sonOkumaIsaretiField;
        
        private string sonOkumaSayacDurKoduField;
        
        private string sonOkumaOkuyucuKoduField;
        
        private string sonOkumaSayacBilgisiField;
        
        private string faturaNoField;
        
        private string faturaTarihiField;
        
        private string gunSayisiField;
        
        private string sarfiyatMetreKupGunField;
        
        private string suBirimFiyatiField;
        
        private string kSubBirimFiyatiField;
        
        private string ekMetreKupField;
        
        private string farkMetreKupField;
        
        private string ctvMetreKupField;
        
        private string suBedeliField;
        
        private string kSubBedeliField;
        
        private string bakimBedeliField;
        
        private string kdvField;
        
        private string cevreTvField;
        
        private string onKusuratField;
        
        private string donemTutariField;
        
        private string gecmisDonemBorcuField;
        
        private string gecmisDonemGecikmeCezasiField;
        
        private string gecmisDonemAlacakField;
        
        private string ctvGecmisDonemBorcuField;
        
        private string ctvGecmisDonemGecikmeCezasiField;
        
        private string odenecekTutarField;
        
        private string sonOdemeTarihiField;
        
        private string aciklamaField;
        
        private string seflikTelNoField;
        
        private string seflikFaxNoField;
        
        private string subeAdiField;
        
        /// <remarks/>
        public string AdSoyad {
            get {
                return this.adSoyadField;
            }
            set {
                this.adSoyadField = value;
            }
        }
        
        /// <remarks/>
        public string MukaveleNo {
            get {
                return this.mukaveleNoField;
            }
            set {
                this.mukaveleNoField = value;
            }
        }
        
        /// <remarks/>
        public string HesapKesimTarihi {
            get {
                return this.hesapKesimTarihiField;
            }
            set {
                this.hesapKesimTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string FaturaTuru {
            get {
                return this.faturaTuruField;
            }
            set {
                this.faturaTuruField = value;
            }
        }
        
        /// <remarks/>
        public string Adres {
            get {
                return this.adresField;
            }
            set {
                this.adresField = value;
            }
        }
        
        /// <remarks/>
        public string BinaKimlikNo {
            get {
                return this.binaKimlikNoField;
            }
            set {
                this.binaKimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public string Donem {
            get {
                return this.donemField;
            }
            set {
                this.donemField = value;
            }
        }
        
        /// <remarks/>
        public string Grup1 {
            get {
                return this.grup1Field;
            }
            set {
                this.grup1Field = value;
            }
        }
        
        /// <remarks/>
        public string Grup2 {
            get {
                return this.grup2Field;
            }
            set {
                this.grup2Field = value;
            }
        }
        
        /// <remarks/>
        public string EvIsyeri {
            get {
                return this.evIsyeriField;
            }
            set {
                this.evIsyeriField = value;
            }
        }
        
        /// <remarks/>
        public string IlkOkumaTarihi {
            get {
                return this.ilkOkumaTarihiField;
            }
            set {
                this.ilkOkumaTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string IlkOkumaIsareti {
            get {
                return this.ilkOkumaIsaretiField;
            }
            set {
                this.ilkOkumaIsaretiField = value;
            }
        }
        
        /// <remarks/>
        public string IlkOkumaSayacDurKodu {
            get {
                return this.ilkOkumaSayacDurKoduField;
            }
            set {
                this.ilkOkumaSayacDurKoduField = value;
            }
        }
        
        /// <remarks/>
        public string IlkOkumaOkuyucuKodu {
            get {
                return this.ilkOkumaOkuyucuKoduField;
            }
            set {
                this.ilkOkumaOkuyucuKoduField = value;
            }
        }
        
        /// <remarks/>
        public string IlkOkumaSayacBilgisi {
            get {
                return this.ilkOkumaSayacBilgisiField;
            }
            set {
                this.ilkOkumaSayacBilgisiField = value;
            }
        }
        
        /// <remarks/>
        public string SonOkumaTarihi {
            get {
                return this.sonOkumaTarihiField;
            }
            set {
                this.sonOkumaTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string SonOkumaIsareti {
            get {
                return this.sonOkumaIsaretiField;
            }
            set {
                this.sonOkumaIsaretiField = value;
            }
        }
        
        /// <remarks/>
        public string SonOkumaSayacDurKodu {
            get {
                return this.sonOkumaSayacDurKoduField;
            }
            set {
                this.sonOkumaSayacDurKoduField = value;
            }
        }
        
        /// <remarks/>
        public string SonOkumaOkuyucuKodu {
            get {
                return this.sonOkumaOkuyucuKoduField;
            }
            set {
                this.sonOkumaOkuyucuKoduField = value;
            }
        }
        
        /// <remarks/>
        public string SonOkumaSayacBilgisi {
            get {
                return this.sonOkumaSayacBilgisiField;
            }
            set {
                this.sonOkumaSayacBilgisiField = value;
            }
        }
        
        /// <remarks/>
        public string FaturaNo {
            get {
                return this.faturaNoField;
            }
            set {
                this.faturaNoField = value;
            }
        }
        
        /// <remarks/>
        public string FaturaTarihi {
            get {
                return this.faturaTarihiField;
            }
            set {
                this.faturaTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string GunSayisi {
            get {
                return this.gunSayisiField;
            }
            set {
                this.gunSayisiField = value;
            }
        }
        
        /// <remarks/>
        public string SarfiyatMetreKupGun {
            get {
                return this.sarfiyatMetreKupGunField;
            }
            set {
                this.sarfiyatMetreKupGunField = value;
            }
        }
        
        /// <remarks/>
        public string SuBirimFiyati {
            get {
                return this.suBirimFiyatiField;
            }
            set {
                this.suBirimFiyatiField = value;
            }
        }
        
        /// <remarks/>
        public string KSubBirimFiyati {
            get {
                return this.kSubBirimFiyatiField;
            }
            set {
                this.kSubBirimFiyatiField = value;
            }
        }
        
        /// <remarks/>
        public string EkMetreKup {
            get {
                return this.ekMetreKupField;
            }
            set {
                this.ekMetreKupField = value;
            }
        }
        
        /// <remarks/>
        public string FarkMetreKup {
            get {
                return this.farkMetreKupField;
            }
            set {
                this.farkMetreKupField = value;
            }
        }
        
        /// <remarks/>
        public string CtvMetreKup {
            get {
                return this.ctvMetreKupField;
            }
            set {
                this.ctvMetreKupField = value;
            }
        }
        
        /// <remarks/>
        public string SuBedeli {
            get {
                return this.suBedeliField;
            }
            set {
                this.suBedeliField = value;
            }
        }
        
        /// <remarks/>
        public string KSubBedeli {
            get {
                return this.kSubBedeliField;
            }
            set {
                this.kSubBedeliField = value;
            }
        }
        
        /// <remarks/>
        public string BakimBedeli {
            get {
                return this.bakimBedeliField;
            }
            set {
                this.bakimBedeliField = value;
            }
        }
        
        /// <remarks/>
        public string Kdv {
            get {
                return this.kdvField;
            }
            set {
                this.kdvField = value;
            }
        }
        
        /// <remarks/>
        public string CevreTv {
            get {
                return this.cevreTvField;
            }
            set {
                this.cevreTvField = value;
            }
        }
        
        /// <remarks/>
        public string OnKusurat {
            get {
                return this.onKusuratField;
            }
            set {
                this.onKusuratField = value;
            }
        }
        
        /// <remarks/>
        public string DonemTutari {
            get {
                return this.donemTutariField;
            }
            set {
                this.donemTutariField = value;
            }
        }
        
        /// <remarks/>
        public string GecmisDonemBorcu {
            get {
                return this.gecmisDonemBorcuField;
            }
            set {
                this.gecmisDonemBorcuField = value;
            }
        }
        
        /// <remarks/>
        public string GecmisDonemGecikmeCezasi {
            get {
                return this.gecmisDonemGecikmeCezasiField;
            }
            set {
                this.gecmisDonemGecikmeCezasiField = value;
            }
        }
        
        /// <remarks/>
        public string GecmisDonemAlacak {
            get {
                return this.gecmisDonemAlacakField;
            }
            set {
                this.gecmisDonemAlacakField = value;
            }
        }
        
        /// <remarks/>
        public string CtvGecmisDonemBorcu {
            get {
                return this.ctvGecmisDonemBorcuField;
            }
            set {
                this.ctvGecmisDonemBorcuField = value;
            }
        }
        
        /// <remarks/>
        public string CtvGecmisDonemGecikmeCezasi {
            get {
                return this.ctvGecmisDonemGecikmeCezasiField;
            }
            set {
                this.ctvGecmisDonemGecikmeCezasiField = value;
            }
        }
        
        /// <remarks/>
        public string OdenecekTutar {
            get {
                return this.odenecekTutarField;
            }
            set {
                this.odenecekTutarField = value;
            }
        }
        
        /// <remarks/>
        public string SonOdemeTarihi {
            get {
                return this.sonOdemeTarihiField;
            }
            set {
                this.sonOdemeTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string Aciklama {
            get {
                return this.aciklamaField;
            }
            set {
                this.aciklamaField = value;
            }
        }
        
        /// <remarks/>
        public string SeflikTelNo {
            get {
                return this.seflikTelNoField;
            }
            set {
                this.seflikTelNoField = value;
            }
        }
        
        /// <remarks/>
        public string SeflikFaxNo {
            get {
                return this.seflikFaxNoField;
            }
            set {
                this.seflikFaxNoField = value;
            }
        }
        
        /// <remarks/>
        public string SubeAdi {
            get {
                return this.subeAdiField;
            }
            set {
                this.subeAdiField = value;
            }
        }
    }
}

#pragma warning restore 1591
