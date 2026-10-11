//------------------------------------------------------------------------------
// Service web servisi istemcisi.
//
// Bu dosya, Visual Studio'nun .NET Framework için ürettiği eski Reference.cs dosyasından
// _araclar/asmx_istemci_uret.py ile dönüştürülmüştür. Proxy sınıfı TNS.Soap.SoapIstemci tabanlıdır
// (HttpClient + XmlSerializer); veri sınıfları eski üretilmiş koddan değiştirilmeden alınmıştır.
//------------------------------------------------------------------------------
#pragma warning disable 1591
namespace TRMServisC.tr.gov.tarim.cezali {
    using System;
    using System.Xml.Serialization;
    using TNS.Soap;

    public partial class Service : SoapIstemci {

        private static readonly Type[] ekTipler = new Type[] { typeof(BaseServiceEntity) };

        protected override Type[] EkTipler {
            get { return ekTipler; }
        }

        public Service() {
            this.Url = global::TRMServisC.Properties.Settings.Default.TRMServisC_tr_gov_tarim_cezali_Service;
        }

        public Isletme isletmeBilgi(string kullaniciAdi, string sifre, string kimlikNo, string vergiNo, string UretimSezonu) {
            return (Isletme)Cagir("http://webinfo.tarim.gov.tr/isletmeBilgi", "isletmeBilgi", "http://webinfo.tarim.gov.tr", typeof(Isletme), "isletmeBilgiResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("kimlikNo", kimlikNo, typeof(string)),
                new SoapParametre("vergiNo", vergiNo, typeof(string)),
                new SoapParametre("UretimSezonu", UretimSezonu, typeof(string)));
        }

        public IsletmeSoykutuk soykutukBilgi(string kullaniciAdi, string sifre, string isletmeNo, string kimlikNo, string vergiNo) {
            return (IsletmeSoykutuk)Cagir("http://webinfo.tarim.gov.tr/soykutukBilgi", "soykutukBilgi", "http://webinfo.tarim.gov.tr", typeof(IsletmeSoykutuk), "soykutukBilgiResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("isletmeNo", isletmeNo, typeof(string)),
                new SoapParametre("kimlikNo", kimlikNo, typeof(string)),
                new SoapParametre("vergiNo", vergiNo, typeof(string)));
        }

        public ortualtiUretici ortualtiBilgi(string kullaniciAdi, string sifre, string kimlikNo) {
            return (ortualtiUretici)Cagir("http://webinfo.tarim.gov.tr/ortualtiBilgi", "ortualtiBilgi", "http://webinfo.tarim.gov.tr", typeof(ortualtiUretici), "ortualtiBilgiResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("kimlikNo", kimlikNo, typeof(string)));
        }

        public SuUrunleri suUrunleriBilgi(string kullaniciAdi, string sifre, string kimlikNo, string vergiNo, string yetistiriciBelgeNo) {
            return (SuUrunleri)Cagir("http://webinfo.tarim.gov.tr/suUrunleriBilgi", "suUrunleriBilgi", "http://webinfo.tarim.gov.tr", typeof(SuUrunleri), "suUrunleriBilgiResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("kimlikNo", kimlikNo, typeof(string)),
                new SoapParametre("vergiNo", vergiNo, typeof(string)),
                new SoapParametre("yetistiriciBelgeNo", yetistiriciBelgeNo, typeof(string)));
        }

        public OrganikTarim organikTarimBilgi(string kullaniciAdi, string sifre, string kimlikNo, string vergiNo) {
            return (OrganikTarim)Cagir("http://webinfo.tarim.gov.tr/organikTarimBilgi", "organikTarimBilgi", "http://webinfo.tarim.gov.tr", typeof(OrganikTarim), "organikTarimBilgiResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("kimlikNo", kimlikNo, typeof(string)),
                new SoapParametre("vergiNo", vergiNo, typeof(string)));
        }

        public SorguGetir CKSSorgu(string kullaniciAdi, string sifre, string koyKodu, string adaNo, string parselNo, string UretimSezonu) {
            return (SorguGetir)Cagir("http://webinfo.tarim.gov.tr/CKSSorgu", "CKSSorgu", "http://webinfo.tarim.gov.tr", typeof(SorguGetir), "CKSSorguResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("koyKodu", koyKodu, typeof(string)),
                new SoapParametre("adaNo", adaNo, typeof(string)),
                new SoapParametre("parselNo", parselNo, typeof(string)),
                new SoapParametre("UretimSezonu", UretimSezonu, typeof(string)));
        }

        public cksUrun getCksUrun(string kullaniciAdi, string sifre) {
            return (cksUrun)Cagir("http://webinfo.tarim.gov.tr/getCksUrun", "getCksUrun", "http://webinfo.tarim.gov.tr", typeof(cksUrun), "getCksUrunResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)));
        }

        public araziUrun lockAraziUrun(string kullaniciAdi, string sifre, string araziUrunID) {
            return (araziUrun)Cagir("http://webinfo.tarim.gov.tr/lockAraziUrun", "lockAraziUrun", "http://webinfo.tarim.gov.tr", typeof(araziUrun), "lockAraziUrunResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("araziUrunID", araziUrunID, typeof(string)));
        }

        public araziUrun unlockAraziUrun(string kullaniciAdi, string sifre, string araziUrunID) {
            return (araziUrun)Cagir("http://webinfo.tarim.gov.tr/unlockAraziUrun", "unlockAraziUrun", "http://webinfo.tarim.gov.tr", typeof(araziUrun), "unlockAraziUrunResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("araziUrunID", araziUrunID, typeof(string)));
        }

        public araziUrun TarsimPolice(string kullaniciAdi, string sifre, string araziUrunID, string policeNo, string baslangicTarihi, string bitisTarihi, string zeyilNo, string zeyilTur, string zeyilBaslangicTarihi, string bransTipi, string devletPrimDestek) {
            return (araziUrun)Cagir("http://webinfo.tarim.gov.tr/TarsimPolice", "TarsimPolice", "http://webinfo.tarim.gov.tr", typeof(araziUrun), "TarsimPoliceResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("araziUrunID", araziUrunID, typeof(string)),
                new SoapParametre("policeNo", policeNo, typeof(string)),
                new SoapParametre("baslangicTarihi", baslangicTarihi, typeof(string)),
                new SoapParametre("bitisTarihi", bitisTarihi, typeof(string)),
                new SoapParametre("zeyilNo", zeyilNo, typeof(string)),
                new SoapParametre("zeyilTur", zeyilTur, typeof(string)),
                new SoapParametre("zeyilBaslangicTarihi", zeyilBaslangicTarihi, typeof(string)),
                new SoapParametre("bransTipi", bransTipi, typeof(string)),
                new SoapParametre("devletPrimDestek", devletPrimDestek, typeof(string)));
        }

        public Isletme isletmeBilgiTarsim(string kullaniciAdi, string sifre, string kimlikNo, string vergiNo, string UretimSezonu) {
            return (Isletme)Cagir("http://webinfo.tarim.gov.tr/isletmeBilgiTarsim", "isletmeBilgiTarsim", "http://webinfo.tarim.gov.tr", typeof(Isletme), "isletmeBilgiTarsimResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("kimlikNo", kimlikNo, typeof(string)),
                new SoapParametre("vergiNo", vergiNo, typeof(string)),
                new SoapParametre("UretimSezonu", UretimSezonu, typeof(string)));
        }

        public string setIsletmeBilgi(string kullaniciAdi, string sifre, string TCKimlikNo, string VergiNo, string HesapNo, string Adi, string Soyadi, string BabaAdi, string DogumTarihi) {
            return (string)Cagir("http://webinfo.tarim.gov.tr/setIsletmeBilgi", "setIsletmeBilgi", "http://webinfo.tarim.gov.tr", typeof(string), "setIsletmeBilgiResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("TCKimlikNo", TCKimlikNo, typeof(string)),
                new SoapParametre("VergiNo", VergiNo, typeof(string)),
                new SoapParametre("HesapNo", HesapNo, typeof(string)),
                new SoapParametre("Adi", Adi, typeof(string)),
                new SoapParametre("Soyadi", Soyadi, typeof(string)),
                new SoapParametre("BabaAdi", BabaAdi, typeof(string)),
                new SoapParametre("DogumTarihi", DogumTarihi, typeof(string)));
        }

        public IlIlceKod getIlIlceKod(string kullaniciAdi, string sifre) {
            return (IlIlceKod)Cagir("http://webinfo.tarim.gov.tr/getIlIlceKod", "getIlIlceKod", "http://webinfo.tarim.gov.tr", typeof(IlIlceKod), "getIlIlceKodResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)));
        }

        public OdemeTuru getOdemeTuru(string kullaniciAdi, string sifre) {
            return (OdemeTuru)Cagir("http://webinfo.tarim.gov.tr/getOdemeTuru", "getOdemeTuru", "http://webinfo.tarim.gov.tr", typeof(OdemeTuru), "getOdemeTuruResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)));
        }

        public HoldingInfoServiceEntity GetHoldingById(string kullaniciAdi, string sifre, string isletmeNo, string TCKimlikNo, string VergiNo) {
            return (HoldingInfoServiceEntity)Cagir("http://webinfo.tarim.gov.tr/GetHoldingById", "GetHoldingById", "http://webinfo.tarim.gov.tr", typeof(HoldingInfoServiceEntity), "GetHoldingByIdResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("isletmeNo", isletmeNo, typeof(string)),
                new SoapParametre("TCKimlikNo", TCKimlikNo, typeof(string)),
                new SoapParametre("VergiNo", VergiNo, typeof(string)));
        }

        public AnimalInfoInHoldingServiceEntity[] GetAnimalsInHolding(string kullaniciAdi, string sifre, string isletmeNo, string TCKimlikNo, string VergiNo) {
            return (AnimalInfoInHoldingServiceEntity[])Cagir("http://webinfo.tarim.gov.tr/GetAnimalsInHolding", "GetAnimalsInHolding", "http://webinfo.tarim.gov.tr", typeof(AnimalInfoInHoldingServiceEntity[]), "GetAnimalsInHoldingResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("isletmeNo", isletmeNo, typeof(string)),
                new SoapParametre("TCKimlikNo", TCKimlikNo, typeof(string)),
                new SoapParametre("VergiNo", VergiNo, typeof(string)));
        }

        public DestekDisiBilgileri getDestekDisiBilgi(string TCKimlikNo, string vergiNo) {
            return (DestekDisiBilgileri)Cagir("http://webinfo.tarim.gov.tr/getDestekDisiBilgi", "getDestekDisiBilgi", "http://webinfo.tarim.gov.tr", typeof(DestekDisiBilgileri), "getDestekDisiBilgiResult",
                new SoapParametre("TCKimlikNo", TCKimlikNo, typeof(string)),
                new SoapParametre("vergiNo", vergiNo, typeof(string)));
        }

        public IsletmeDestek GetIsletmeDestek(string kullaniciAdi, string sifre, string IsletmeNolar) {
            return (IsletmeDestek)Cagir("http://webinfo.tarim.gov.tr/GetIsletmeDestek", "GetIsletmeDestek", "http://webinfo.tarim.gov.tr", typeof(IsletmeDestek), "GetIsletmeDestekResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("IsletmeNolar", IsletmeNolar, typeof(string)));
        }

        public YerlesimBilgileri GetYerlesimBilgileri(string kullaniciAdi, string sifre, string IlTrafikKodu) {
            return (YerlesimBilgileri)Cagir("http://webinfo.tarim.gov.tr/GetYerlesimBilgileri", "GetYerlesimBilgileri", "http://webinfo.tarim.gov.tr", typeof(YerlesimBilgileri), "GetYerlesimBilgileriResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("IlTrafikKodu", IlTrafikKodu, typeof(string)));
        }

        public IsletmeDetayBilgileri GetIsletmeDetayBilgileri(string kullaniciAdi, string sifre, string TcKimlikNoVergiNo, string UretimSezonu, string KoyID, string AdaNo, string ParselNo) {
            return (IsletmeDetayBilgileri)Cagir("http://webinfo.tarim.gov.tr/GetIsletmeDetayBilgileri", "GetIsletmeDetayBilgileri", "http://webinfo.tarim.gov.tr", typeof(IsletmeDetayBilgileri), "GetIsletmeDetayBilgileriResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("TcKimlikNoVergiNo", TcKimlikNoVergiNo, typeof(string)),
                new SoapParametre("UretimSezonu", UretimSezonu, typeof(string)),
                new SoapParametre("KoyID", KoyID, typeof(string)),
                new SoapParametre("AdaNo", AdaNo, typeof(string)),
                new SoapParametre("ParselNo", ParselNo, typeof(string)));
        }

        public KoyUretimBilgileri GetKoyUretimBilgileri(string kullaniciAdi, string sifre, string UretimSezonu, string KoyID) {
            return (KoyUretimBilgileri)Cagir("http://webinfo.tarim.gov.tr/GetKoyUretimBilgileri", "GetKoyUretimBilgileri", "http://webinfo.tarim.gov.tr", typeof(KoyUretimBilgileri), "GetKoyUretimBilgileriResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("UretimSezonu", UretimSezonu, typeof(string)),
                new SoapParametre("KoyID", KoyID, typeof(string)));
        }

        public IsletmeDetayBilgileri GetIsletmeBilgileri(string kullaniciAdi, string sifre, string TcKimlikNoVergiNo, string UretimSezonu) {
            return (IsletmeDetayBilgileri)Cagir("http://webinfo.tarim.gov.tr/GetIsletmeBilgileri", "GetIsletmeBilgileri", "http://webinfo.tarim.gov.tr", typeof(IsletmeDetayBilgileri), "GetIsletmeBilgileriResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("TcKimlikNoVergiNo", TcKimlikNoVergiNo, typeof(string)),
                new SoapParametre("UretimSezonu", UretimSezonu, typeof(string)));
        }

        public IsletmeDetayBilgileriDetay GetIsletmeBilgileriDetay(string kullaniciAdi, string sifre, string TcKimlikNoVergiNo, string UretimSezonu) {
            return (IsletmeDetayBilgileriDetay)Cagir("http://webinfo.tarim.gov.tr/GetIsletmeBilgileriDetay", "GetIsletmeBilgileriDetay", "http://webinfo.tarim.gov.tr", typeof(IsletmeDetayBilgileriDetay), "GetIsletmeBilgileriDetayResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("TcKimlikNoVergiNo", TcKimlikNoVergiNo, typeof(string)),
                new SoapParametre("UretimSezonu", UretimSezonu, typeof(string)));
        }

        public IsletmeAKS GetAKSBilgileri(string kullaniciAdi, string sifre, string TcKimlikNoVergiNo) {
            return (IsletmeAKS)Cagir("http://webinfo.tarim.gov.tr/GetAKSBilgileri", "GetAKSBilgileri", "http://webinfo.tarim.gov.tr", typeof(IsletmeAKS), "GetAKSBilgileriResult",
                new SoapParametre("kullaniciAdi", kullaniciAdi, typeof(string)),
                new SoapParametre("sifre", sifre, typeof(string)),
                new SoapParametre("TcKimlikNoVergiNo", TcKimlikNoVergiNo, typeof(string)));
        }

    }

    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class Isletme {
        
        private string dogrulamaField;
        
        private string uretimSezonuField;
        
        private string isletmeDurumuField;
        
        private string isletmeDurumuIDField;
        
        private string tuzelKisilikTuruField;
        
        private string isletmeNoField;
        
        private string ilField;
        
        private string ilKoduField;
        
        private string ilceField;
        
        private string ilceKoduField;
        
        private string bucakField;
        
        private string bucakKoduField;
        
        private string koyField;
        
        private string koyKoduField;
        
        private string kimlikNoField;
        
        private string vergiNoField;
        
        private string adiField;
        
        private string soyadiField;
        
        private string babaAdiField;
        
        private string dogumTarihiField;
        
        private string evTelField;
        
        private string isTelField;
        
        private string cepTelField;
        
        private Arazi[] araziField;
        
        /// <remarks/>
        public string dogrulama {
            get {
                return this.dogrulamaField;
            }
            set {
                this.dogrulamaField = value;
            }
        }
        
        /// <remarks/>
        public string uretimSezonu {
            get {
                return this.uretimSezonuField;
            }
            set {
                this.uretimSezonuField = value;
            }
        }
        
        /// <remarks/>
        public string isletmeDurumu {
            get {
                return this.isletmeDurumuField;
            }
            set {
                this.isletmeDurumuField = value;
            }
        }
        
        /// <remarks/>
        public string isletmeDurumuID {
            get {
                return this.isletmeDurumuIDField;
            }
            set {
                this.isletmeDurumuIDField = value;
            }
        }
        
        /// <remarks/>
        public string TuzelKisilikTuru {
            get {
                return this.tuzelKisilikTuruField;
            }
            set {
                this.tuzelKisilikTuruField = value;
            }
        }
        
        /// <remarks/>
        public string isletmeNo {
            get {
                return this.isletmeNoField;
            }
            set {
                this.isletmeNoField = value;
            }
        }
        
        /// <remarks/>
        public string il {
            get {
                return this.ilField;
            }
            set {
                this.ilField = value;
            }
        }
        
        /// <remarks/>
        public string ilKodu {
            get {
                return this.ilKoduField;
            }
            set {
                this.ilKoduField = value;
            }
        }
        
        /// <remarks/>
        public string ilce {
            get {
                return this.ilceField;
            }
            set {
                this.ilceField = value;
            }
        }
        
        /// <remarks/>
        public string ilceKodu {
            get {
                return this.ilceKoduField;
            }
            set {
                this.ilceKoduField = value;
            }
        }
        
        /// <remarks/>
        public string bucak {
            get {
                return this.bucakField;
            }
            set {
                this.bucakField = value;
            }
        }
        
        /// <remarks/>
        public string bucakKodu {
            get {
                return this.bucakKoduField;
            }
            set {
                this.bucakKoduField = value;
            }
        }
        
        /// <remarks/>
        public string koy {
            get {
                return this.koyField;
            }
            set {
                this.koyField = value;
            }
        }
        
        /// <remarks/>
        public string koyKodu {
            get {
                return this.koyKoduField;
            }
            set {
                this.koyKoduField = value;
            }
        }
        
        /// <remarks/>
        public string kimlikNo {
            get {
                return this.kimlikNoField;
            }
            set {
                this.kimlikNoField = value;
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
        public string adi {
            get {
                return this.adiField;
            }
            set {
                this.adiField = value;
            }
        }
        
        /// <remarks/>
        public string soyadi {
            get {
                return this.soyadiField;
            }
            set {
                this.soyadiField = value;
            }
        }
        
        /// <remarks/>
        public string babaAdi {
            get {
                return this.babaAdiField;
            }
            set {
                this.babaAdiField = value;
            }
        }
        
        /// <remarks/>
        public string dogumTarihi {
            get {
                return this.dogumTarihiField;
            }
            set {
                this.dogumTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string EvTel {
            get {
                return this.evTelField;
            }
            set {
                this.evTelField = value;
            }
        }
        
        /// <remarks/>
        public string IsTel {
            get {
                return this.isTelField;
            }
            set {
                this.isTelField = value;
            }
        }
        
        /// <remarks/>
        public string CepTel {
            get {
                return this.cepTelField;
            }
            set {
                this.cepTelField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Arazi[] arazi {
            get {
                return this.araziField;
            }
            set {
                this.araziField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class Arazi {
        
        private string kesifRaporTarihiField;
        
        private string belgeTuruField;
        
        private string araziIDField;
        
        private string ilField;
        
        private string ilKoduField;
        
        private string ilceField;
        
        private string ilceKoduField;
        
        private string bucakField;
        
        private string bucakKoduField;
        
        private string koyField;
        
        private string koyKoduField;
        
        private string adaNoField;
        
        private string parselNoField;
        
        private string parselAlanField;
        
        private string suDurumuField;
        
        private string araziAlanField;
        
        private string bolmeNoField;
        
        private string sahaTahsisIzinNoField;
        
        private string kendineAitKullanilanAlanField;
        
        private Urun[] urunField;
        
        /// <remarks/>
        public string KesifRaporTarihi {
            get {
                return this.kesifRaporTarihiField;
            }
            set {
                this.kesifRaporTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string BelgeTuru {
            get {
                return this.belgeTuruField;
            }
            set {
                this.belgeTuruField = value;
            }
        }
        
        /// <remarks/>
        public string araziID {
            get {
                return this.araziIDField;
            }
            set {
                this.araziIDField = value;
            }
        }
        
        /// <remarks/>
        public string il {
            get {
                return this.ilField;
            }
            set {
                this.ilField = value;
            }
        }
        
        /// <remarks/>
        public string ilKodu {
            get {
                return this.ilKoduField;
            }
            set {
                this.ilKoduField = value;
            }
        }
        
        /// <remarks/>
        public string ilce {
            get {
                return this.ilceField;
            }
            set {
                this.ilceField = value;
            }
        }
        
        /// <remarks/>
        public string ilceKodu {
            get {
                return this.ilceKoduField;
            }
            set {
                this.ilceKoduField = value;
            }
        }
        
        /// <remarks/>
        public string bucak {
            get {
                return this.bucakField;
            }
            set {
                this.bucakField = value;
            }
        }
        
        /// <remarks/>
        public string bucakKodu {
            get {
                return this.bucakKoduField;
            }
            set {
                this.bucakKoduField = value;
            }
        }
        
        /// <remarks/>
        public string koy {
            get {
                return this.koyField;
            }
            set {
                this.koyField = value;
            }
        }
        
        /// <remarks/>
        public string koyKodu {
            get {
                return this.koyKoduField;
            }
            set {
                this.koyKoduField = value;
            }
        }
        
        /// <remarks/>
        public string adaNo {
            get {
                return this.adaNoField;
            }
            set {
                this.adaNoField = value;
            }
        }
        
        /// <remarks/>
        public string parselNo {
            get {
                return this.parselNoField;
            }
            set {
                this.parselNoField = value;
            }
        }
        
        /// <remarks/>
        public string parselAlan {
            get {
                return this.parselAlanField;
            }
            set {
                this.parselAlanField = value;
            }
        }
        
        /// <remarks/>
        public string suDurumu {
            get {
                return this.suDurumuField;
            }
            set {
                this.suDurumuField = value;
            }
        }
        
        /// <remarks/>
        public string araziAlan {
            get {
                return this.araziAlanField;
            }
            set {
                this.araziAlanField = value;
            }
        }
        
        /// <remarks/>
        public string BolmeNo {
            get {
                return this.bolmeNoField;
            }
            set {
                this.bolmeNoField = value;
            }
        }
        
        /// <remarks/>
        public string SahaTahsisIzinNo {
            get {
                return this.sahaTahsisIzinNoField;
            }
            set {
                this.sahaTahsisIzinNoField = value;
            }
        }
        
        /// <remarks/>
        public string KendineAitKullanilanAlan {
            get {
                return this.kendineAitKullanilanAlanField;
            }
            set {
                this.kendineAitKullanilanAlanField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Urun[] urun {
            get {
                return this.urunField;
            }
            set {
                this.urunField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class Urun {
        
        private string urunIDField;
        
        private string araziUrunIDField;
        
        private string urunAdiField;
        
        private string uretimCesidiField;
        
        private string sertifikaliVerimField;
        
        private string sertifikasizVerimField;
        
        private string sertifikaliVerimSuluField;
        
        private string sertifikasizVerimSuluField;
        
        private string ortualtiVerimField;
        
        private string araziVerimField;
        
        private string ekilenAlanField;
        
        private UrunSertifika[] urunSertifikaField;
        
        /// <remarks/>
        public string urunID {
            get {
                return this.urunIDField;
            }
            set {
                this.urunIDField = value;
            }
        }
        
        /// <remarks/>
        public string araziUrunID {
            get {
                return this.araziUrunIDField;
            }
            set {
                this.araziUrunIDField = value;
            }
        }
        
        /// <remarks/>
        public string urunAdi {
            get {
                return this.urunAdiField;
            }
            set {
                this.urunAdiField = value;
            }
        }
        
        /// <remarks/>
        public string uretimCesidi {
            get {
                return this.uretimCesidiField;
            }
            set {
                this.uretimCesidiField = value;
            }
        }
        
        /// <remarks/>
        public string sertifikaliVerim {
            get {
                return this.sertifikaliVerimField;
            }
            set {
                this.sertifikaliVerimField = value;
            }
        }
        
        /// <remarks/>
        public string sertifikasizVerim {
            get {
                return this.sertifikasizVerimField;
            }
            set {
                this.sertifikasizVerimField = value;
            }
        }
        
        /// <remarks/>
        public string sertifikaliVerimSulu {
            get {
                return this.sertifikaliVerimSuluField;
            }
            set {
                this.sertifikaliVerimSuluField = value;
            }
        }
        
        /// <remarks/>
        public string sertifikasizVerimSulu {
            get {
                return this.sertifikasizVerimSuluField;
            }
            set {
                this.sertifikasizVerimSuluField = value;
            }
        }
        
        /// <remarks/>
        public string ortualtiVerim {
            get {
                return this.ortualtiVerimField;
            }
            set {
                this.ortualtiVerimField = value;
            }
        }
        
        /// <remarks/>
        public string araziVerim {
            get {
                return this.araziVerimField;
            }
            set {
                this.araziVerimField = value;
            }
        }
        
        /// <remarks/>
        public string ekilenAlan {
            get {
                return this.ekilenAlanField;
            }
            set {
                this.ekilenAlanField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public UrunSertifika[] urunSertifika {
            get {
                return this.urunSertifikaField;
            }
            set {
                this.urunSertifikaField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class UrunSertifika {
        
        private string sertifikaKurulusuField;
        
        private string sertifikaNoField;
        
        private string sertifikaTarihiField;
        
        private string partiNoField;
        
        private string miktarField;
        
        private string alanField;
        
        /// <remarks/>
        public string SertifikaKurulusu {
            get {
                return this.sertifikaKurulusuField;
            }
            set {
                this.sertifikaKurulusuField = value;
            }
        }
        
        /// <remarks/>
        public string SertifikaNo {
            get {
                return this.sertifikaNoField;
            }
            set {
                this.sertifikaNoField = value;
            }
        }
        
        /// <remarks/>
        public string SertifikaTarihi {
            get {
                return this.sertifikaTarihiField;
            }
            set {
                this.sertifikaTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string PartiNo {
            get {
                return this.partiNoField;
            }
            set {
                this.partiNoField = value;
            }
        }
        
        /// <remarks/>
        public string Miktar {
            get {
                return this.miktarField;
            }
            set {
                this.miktarField = value;
            }
        }
        
        /// <remarks/>
        public string Alan {
            get {
                return this.alanField;
            }
            set {
                this.alanField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class Koloni {
        
        private string koloniNameField;
        
        /// <remarks/>
        public string KoloniName {
            get {
                return this.koloniNameField;
            }
            set {
                this.koloniNameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class KoloniVarligi {
        
        private string koloniVarligiIDField;
        
        private string koloniTipiField;
        
        private string koloniTipiNameField;
        
        private string ariIrkiEkotipiField;
        
        private string ariIrkiEkotipiNameField;
        
        private string yerleskeTarihiField;
        
        private string koloniFaaliyetiIDField;
        
        private string koloniFaaliyetiField;
        
        private Koloni[] koloniField;
        
        /// <remarks/>
        public string koloniVarligiID {
            get {
                return this.koloniVarligiIDField;
            }
            set {
                this.koloniVarligiIDField = value;
            }
        }
        
        /// <remarks/>
        public string koloniTipi {
            get {
                return this.koloniTipiField;
            }
            set {
                this.koloniTipiField = value;
            }
        }
        
        /// <remarks/>
        public string koloniTipiName {
            get {
                return this.koloniTipiNameField;
            }
            set {
                this.koloniTipiNameField = value;
            }
        }
        
        /// <remarks/>
        public string ariIrkiEkotipi {
            get {
                return this.ariIrkiEkotipiField;
            }
            set {
                this.ariIrkiEkotipiField = value;
            }
        }
        
        /// <remarks/>
        public string ariIrkiEkotipiName {
            get {
                return this.ariIrkiEkotipiNameField;
            }
            set {
                this.ariIrkiEkotipiNameField = value;
            }
        }
        
        /// <remarks/>
        public string yerleskeTarihi {
            get {
                return this.yerleskeTarihiField;
            }
            set {
                this.yerleskeTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string koloniFaaliyetiID {
            get {
                return this.koloniFaaliyetiIDField;
            }
            set {
                this.koloniFaaliyetiIDField = value;
            }
        }
        
        /// <remarks/>
        public string koloniFaaliyeti {
            get {
                return this.koloniFaaliyetiField;
            }
            set {
                this.koloniFaaliyetiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Koloni[] koloni {
            get {
                return this.koloniField;
            }
            set {
                this.koloniField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class Arilik {
        
        private string arilikIDField;
        
        private string ilIDField;
        
        private string ilAdiField;
        
        private string ilceIDField;
        
        private string ilceAdiField;
        
        private string koyIDField;
        
        private string koyAdiField;
        
        private string arilikAdiField;
        
        private string floraTakibiIDField;
        
        private string floraTakibiNameField;
        
        private string konaklamaTarihiField;
        
        private KoloniVarligi[] koloniVarligiField;
        
        /// <remarks/>
        public string arilikID {
            get {
                return this.arilikIDField;
            }
            set {
                this.arilikIDField = value;
            }
        }
        
        /// <remarks/>
        public string IlID {
            get {
                return this.ilIDField;
            }
            set {
                this.ilIDField = value;
            }
        }
        
        /// <remarks/>
        public string IlAdi {
            get {
                return this.ilAdiField;
            }
            set {
                this.ilAdiField = value;
            }
        }
        
        /// <remarks/>
        public string IlceID {
            get {
                return this.ilceIDField;
            }
            set {
                this.ilceIDField = value;
            }
        }
        
        /// <remarks/>
        public string IlceAdi {
            get {
                return this.ilceAdiField;
            }
            set {
                this.ilceAdiField = value;
            }
        }
        
        /// <remarks/>
        public string KoyID {
            get {
                return this.koyIDField;
            }
            set {
                this.koyIDField = value;
            }
        }
        
        /// <remarks/>
        public string KoyAdi {
            get {
                return this.koyAdiField;
            }
            set {
                this.koyAdiField = value;
            }
        }
        
        /// <remarks/>
        public string arilikAdi {
            get {
                return this.arilikAdiField;
            }
            set {
                this.arilikAdiField = value;
            }
        }
        
        /// <remarks/>
        public string floraTakibiID {
            get {
                return this.floraTakibiIDField;
            }
            set {
                this.floraTakibiIDField = value;
            }
        }
        
        /// <remarks/>
        public string floraTakibiName {
            get {
                return this.floraTakibiNameField;
            }
            set {
                this.floraTakibiNameField = value;
            }
        }
        
        /// <remarks/>
        public string konaklamaTarihi {
            get {
                return this.konaklamaTarihiField;
            }
            set {
                this.konaklamaTarihiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public KoloniVarligi[] koloniVarligi {
            get {
                return this.koloniVarligiField;
            }
            set {
                this.koloniVarligiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class IsletmeAKS {
        
        private string dogrulamaField;
        
        private string isletmeTurIDField;
        
        private string kisilikNameField;
        
        private string isletmeNoField;
        
        private string kimlikNoField;
        
        private string vergiNoField;
        
        private string adiField;
        
        private string soyadiField;
        
        private string babaAdiField;
        
        private string dogumTarihiField;
        
        private string telefon1Field;
        
        private string telefon2Field;
        
        private string adresField;
        
        private Arilik[] arilikField;
        
        /// <remarks/>
        public string dogrulama {
            get {
                return this.dogrulamaField;
            }
            set {
                this.dogrulamaField = value;
            }
        }
        
        /// <remarks/>
        public string isletmeTurID {
            get {
                return this.isletmeTurIDField;
            }
            set {
                this.isletmeTurIDField = value;
            }
        }
        
        /// <remarks/>
        public string kisilikName {
            get {
                return this.kisilikNameField;
            }
            set {
                this.kisilikNameField = value;
            }
        }
        
        /// <remarks/>
        public string isletmeNo {
            get {
                return this.isletmeNoField;
            }
            set {
                this.isletmeNoField = value;
            }
        }
        
        /// <remarks/>
        public string kimlikNo {
            get {
                return this.kimlikNoField;
            }
            set {
                this.kimlikNoField = value;
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
        public string adi {
            get {
                return this.adiField;
            }
            set {
                this.adiField = value;
            }
        }
        
        /// <remarks/>
        public string soyadi {
            get {
                return this.soyadiField;
            }
            set {
                this.soyadiField = value;
            }
        }
        
        /// <remarks/>
        public string babaAdi {
            get {
                return this.babaAdiField;
            }
            set {
                this.babaAdiField = value;
            }
        }
        
        /// <remarks/>
        public string dogumTarihi {
            get {
                return this.dogumTarihiField;
            }
            set {
                this.dogumTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string telefon1 {
            get {
                return this.telefon1Field;
            }
            set {
                this.telefon1Field = value;
            }
        }
        
        /// <remarks/>
        public string telefon2 {
            get {
                return this.telefon2Field;
            }
            set {
                this.telefon2Field = value;
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
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Arilik[] arilik {
            get {
                return this.arilikField;
            }
            set {
                this.arilikField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class FaturalarDetay {
        
        private string firmaTCKimlikVergiNoField;
        
        private string firmaAdiField;
        
        private string aciklamaField;
        
        private string faturaNoField;
        
        private string faturaTarihiField;
        
        private double miktarField;
        
        private double tutarField;
        
        /// <remarks/>
        public string FirmaTCKimlikVergiNo {
            get {
                return this.firmaTCKimlikVergiNoField;
            }
            set {
                this.firmaTCKimlikVergiNoField = value;
            }
        }
        
        /// <remarks/>
        public string FirmaAdi {
            get {
                return this.firmaAdiField;
            }
            set {
                this.firmaAdiField = value;
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
        public double Miktar {
            get {
                return this.miktarField;
            }
            set {
                this.miktarField = value;
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
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class DesteklerDetay {
        
        private string destekAdiField;
        
        private double tutarField;
        
        /// <remarks/>
        public string DestekAdi {
            get {
                return this.destekAdiField;
            }
            set {
                this.destekAdiField = value;
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
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class HissedarlarDetay {
        
        private string tCKimlikNoField;
        
        private string vergiNoField;
        
        private string adiField;
        
        private string soyadiField;
        
        private decimal alanField;
        
        /// <remarks/>
        public string TCKimlikNo {
            get {
                return this.tCKimlikNoField;
            }
            set {
                this.tCKimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public string VergiNo {
            get {
                return this.vergiNoField;
            }
            set {
                this.vergiNoField = value;
            }
        }
        
        /// <remarks/>
        public string Adi {
            get {
                return this.adiField;
            }
            set {
                this.adiField = value;
            }
        }
        
        /// <remarks/>
        public string Soyadi {
            get {
                return this.soyadiField;
            }
            set {
                this.soyadiField = value;
            }
        }
        
        /// <remarks/>
        public decimal Alan {
            get {
                return this.alanField;
            }
            set {
                this.alanField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class UrunlerDetay {
        
        private string urunAdiField;
        
        private string uretimCesidiField;
        
        private string ekilenAlanField;
        
        private string urunIDField;
        
        private string araziUrunIDField;
        
        private string verimField;
        
        /// <remarks/>
        public string urunAdi {
            get {
                return this.urunAdiField;
            }
            set {
                this.urunAdiField = value;
            }
        }
        
        /// <remarks/>
        public string uretimCesidi {
            get {
                return this.uretimCesidiField;
            }
            set {
                this.uretimCesidiField = value;
            }
        }
        
        /// <remarks/>
        public string ekilenAlan {
            get {
                return this.ekilenAlanField;
            }
            set {
                this.ekilenAlanField = value;
            }
        }
        
        /// <remarks/>
        public string urunID {
            get {
                return this.urunIDField;
            }
            set {
                this.urunIDField = value;
            }
        }
        
        /// <remarks/>
        public string AraziUrunID {
            get {
                return this.araziUrunIDField;
            }
            set {
                this.araziUrunIDField = value;
            }
        }
        
        /// <remarks/>
        public string Verim {
            get {
                return this.verimField;
            }
            set {
                this.verimField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class ArazilerDetay {
        
        private string kesifRaporTarihiField;
        
        private string belgeTuruField;
        
        private string araziIDField;
        
        private string ilKoduField;
        
        private string ilField;
        
        private string ilceKoduField;
        
        private string ilceField;
        
        private string bucakKoduField;
        
        private string bucakField;
        
        private string koyKoduField;
        
        private string koyField;
        
        private string adaNoField;
        
        private string parselNoField;
        
        private string parselAlanField;
        
        private string suDurumuField;
        
        private string araziAlanField;
        
        private string bolmeNoField;
        
        private string sahaTahsisIzinNoField;
        
        private string kendineAitKullanilanAlanField;
        
        private UrunlerDetay[] urunField;
        
        private HissedarlarDetay[] hissedarField;
        
        /// <remarks/>
        public string KesifRaporTarihi {
            get {
                return this.kesifRaporTarihiField;
            }
            set {
                this.kesifRaporTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string BelgeTuru {
            get {
                return this.belgeTuruField;
            }
            set {
                this.belgeTuruField = value;
            }
        }
        
        /// <remarks/>
        public string araziID {
            get {
                return this.araziIDField;
            }
            set {
                this.araziIDField = value;
            }
        }
        
        /// <remarks/>
        public string ilKodu {
            get {
                return this.ilKoduField;
            }
            set {
                this.ilKoduField = value;
            }
        }
        
        /// <remarks/>
        public string il {
            get {
                return this.ilField;
            }
            set {
                this.ilField = value;
            }
        }
        
        /// <remarks/>
        public string ilceKodu {
            get {
                return this.ilceKoduField;
            }
            set {
                this.ilceKoduField = value;
            }
        }
        
        /// <remarks/>
        public string ilce {
            get {
                return this.ilceField;
            }
            set {
                this.ilceField = value;
            }
        }
        
        /// <remarks/>
        public string bucakKodu {
            get {
                return this.bucakKoduField;
            }
            set {
                this.bucakKoduField = value;
            }
        }
        
        /// <remarks/>
        public string bucak {
            get {
                return this.bucakField;
            }
            set {
                this.bucakField = value;
            }
        }
        
        /// <remarks/>
        public string koyKodu {
            get {
                return this.koyKoduField;
            }
            set {
                this.koyKoduField = value;
            }
        }
        
        /// <remarks/>
        public string koy {
            get {
                return this.koyField;
            }
            set {
                this.koyField = value;
            }
        }
        
        /// <remarks/>
        public string adaNo {
            get {
                return this.adaNoField;
            }
            set {
                this.adaNoField = value;
            }
        }
        
        /// <remarks/>
        public string parselNo {
            get {
                return this.parselNoField;
            }
            set {
                this.parselNoField = value;
            }
        }
        
        /// <remarks/>
        public string parselAlan {
            get {
                return this.parselAlanField;
            }
            set {
                this.parselAlanField = value;
            }
        }
        
        /// <remarks/>
        public string suDurumu {
            get {
                return this.suDurumuField;
            }
            set {
                this.suDurumuField = value;
            }
        }
        
        /// <remarks/>
        public string araziAlan {
            get {
                return this.araziAlanField;
            }
            set {
                this.araziAlanField = value;
            }
        }
        
        /// <remarks/>
        public string BolmeNo {
            get {
                return this.bolmeNoField;
            }
            set {
                this.bolmeNoField = value;
            }
        }
        
        /// <remarks/>
        public string SahaTahsisIzinNo {
            get {
                return this.sahaTahsisIzinNoField;
            }
            set {
                this.sahaTahsisIzinNoField = value;
            }
        }
        
        /// <remarks/>
        public string KendineAitKullanilanAlan {
            get {
                return this.kendineAitKullanilanAlanField;
            }
            set {
                this.kendineAitKullanilanAlanField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public UrunlerDetay[] urun {
            get {
                return this.urunField;
            }
            set {
                this.urunField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public HissedarlarDetay[] hissedar {
            get {
                return this.hissedarField;
            }
            set {
                this.hissedarField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class IsletmelerDetay {
        
        private string uretimSezonuField;
        
        private string isletmeDurumuField;
        
        private string kimlikNoField;
        
        private string vergiNoField;
        
        private string adiField;
        
        private string soyadiField;
        
        private string babaAdiField;
        
        private string dogumTarihiField;
        
        private string ilKoduField;
        
        private string ilField;
        
        private string ilceKoduField;
        
        private string ilceField;
        
        private string bucakKoduField;
        
        private string bucakField;
        
        private string koyKoduField;
        
        private string koyField;
        
        private string evTelField;
        
        private string isTelField;
        
        private string cepTelField;
        
        private string isletmeNoField;
        
        private ArazilerDetay[] arazilerField;
        
        private DesteklerDetay[] desteklerField;
        
        private FaturalarDetay[] faturalarField;
        
        /// <remarks/>
        public string uretimSezonu {
            get {
                return this.uretimSezonuField;
            }
            set {
                this.uretimSezonuField = value;
            }
        }
        
        /// <remarks/>
        public string isletmeDurumu {
            get {
                return this.isletmeDurumuField;
            }
            set {
                this.isletmeDurumuField = value;
            }
        }
        
        /// <remarks/>
        public string kimlikNo {
            get {
                return this.kimlikNoField;
            }
            set {
                this.kimlikNoField = value;
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
        public string adi {
            get {
                return this.adiField;
            }
            set {
                this.adiField = value;
            }
        }
        
        /// <remarks/>
        public string soyadi {
            get {
                return this.soyadiField;
            }
            set {
                this.soyadiField = value;
            }
        }
        
        /// <remarks/>
        public string babaAdi {
            get {
                return this.babaAdiField;
            }
            set {
                this.babaAdiField = value;
            }
        }
        
        /// <remarks/>
        public string dogumTarihi {
            get {
                return this.dogumTarihiField;
            }
            set {
                this.dogumTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string ilKodu {
            get {
                return this.ilKoduField;
            }
            set {
                this.ilKoduField = value;
            }
        }
        
        /// <remarks/>
        public string il {
            get {
                return this.ilField;
            }
            set {
                this.ilField = value;
            }
        }
        
        /// <remarks/>
        public string ilceKodu {
            get {
                return this.ilceKoduField;
            }
            set {
                this.ilceKoduField = value;
            }
        }
        
        /// <remarks/>
        public string ilce {
            get {
                return this.ilceField;
            }
            set {
                this.ilceField = value;
            }
        }
        
        /// <remarks/>
        public string bucakKodu {
            get {
                return this.bucakKoduField;
            }
            set {
                this.bucakKoduField = value;
            }
        }
        
        /// <remarks/>
        public string bucak {
            get {
                return this.bucakField;
            }
            set {
                this.bucakField = value;
            }
        }
        
        /// <remarks/>
        public string koyKodu {
            get {
                return this.koyKoduField;
            }
            set {
                this.koyKoduField = value;
            }
        }
        
        /// <remarks/>
        public string koy {
            get {
                return this.koyField;
            }
            set {
                this.koyField = value;
            }
        }
        
        /// <remarks/>
        public string EvTel {
            get {
                return this.evTelField;
            }
            set {
                this.evTelField = value;
            }
        }
        
        /// <remarks/>
        public string IsTel {
            get {
                return this.isTelField;
            }
            set {
                this.isTelField = value;
            }
        }
        
        /// <remarks/>
        public string CepTel {
            get {
                return this.cepTelField;
            }
            set {
                this.cepTelField = value;
            }
        }
        
        /// <remarks/>
        public string IsletmeNo {
            get {
                return this.isletmeNoField;
            }
            set {
                this.isletmeNoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public ArazilerDetay[] araziler {
            get {
                return this.arazilerField;
            }
            set {
                this.arazilerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public DesteklerDetay[] destekler {
            get {
                return this.desteklerField;
            }
            set {
                this.desteklerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public FaturalarDetay[] faturalar {
            get {
                return this.faturalarField;
            }
            set {
                this.faturalarField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class IsletmeDetayBilgileriDetay {
        
        private string dogrulamaField;
        
        private IsletmelerDetay[] isletmelerField;
        
        /// <remarks/>
        public string dogrulama {
            get {
                return this.dogrulamaField;
            }
            set {
                this.dogrulamaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public IsletmelerDetay[] isletmeler {
            get {
                return this.isletmelerField;
            }
            set {
                this.isletmelerField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class UretimBilgileri {
        
        private string ilAdiField;
        
        private string ilceAdiField;
        
        private string bucakAdiField;
        
        private string koyAdiField;
        
        private string suDurumuIDField;
        
        private string suDurumuField;
        
        private string uretimCesidiField;
        
        private string urunAdiField;
        
        private string alanField;
        
        /// <remarks/>
        public string IlAdi {
            get {
                return this.ilAdiField;
            }
            set {
                this.ilAdiField = value;
            }
        }
        
        /// <remarks/>
        public string IlceAdi {
            get {
                return this.ilceAdiField;
            }
            set {
                this.ilceAdiField = value;
            }
        }
        
        /// <remarks/>
        public string BucakAdi {
            get {
                return this.bucakAdiField;
            }
            set {
                this.bucakAdiField = value;
            }
        }
        
        /// <remarks/>
        public string KoyAdi {
            get {
                return this.koyAdiField;
            }
            set {
                this.koyAdiField = value;
            }
        }
        
        /// <remarks/>
        public string SuDurumuID {
            get {
                return this.suDurumuIDField;
            }
            set {
                this.suDurumuIDField = value;
            }
        }
        
        /// <remarks/>
        public string SuDurumu {
            get {
                return this.suDurumuField;
            }
            set {
                this.suDurumuField = value;
            }
        }
        
        /// <remarks/>
        public string UretimCesidi {
            get {
                return this.uretimCesidiField;
            }
            set {
                this.uretimCesidiField = value;
            }
        }
        
        /// <remarks/>
        public string UrunAdi {
            get {
                return this.urunAdiField;
            }
            set {
                this.urunAdiField = value;
            }
        }
        
        /// <remarks/>
        public string Alan {
            get {
                return this.alanField;
            }
            set {
                this.alanField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class KoyUretimBilgileri {
        
        private string dogrulamaField;
        
        private UretimBilgileri[] uretimBilgiField;
        
        /// <remarks/>
        public string dogrulama {
            get {
                return this.dogrulamaField;
            }
            set {
                this.dogrulamaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public UretimBilgileri[] uretimBilgi {
            get {
                return this.uretimBilgiField;
            }
            set {
                this.uretimBilgiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class Faturalar {
        
        private string firmaTCKimlikVergiNoField;
        
        private string firmaAdiField;
        
        private string aciklamaField;
        
        private string faturaNoField;
        
        private string faturaTarihiField;
        
        private double miktarField;
        
        private double tutarField;
        
        /// <remarks/>
        public string FirmaTCKimlikVergiNo {
            get {
                return this.firmaTCKimlikVergiNoField;
            }
            set {
                this.firmaTCKimlikVergiNoField = value;
            }
        }
        
        /// <remarks/>
        public string FirmaAdi {
            get {
                return this.firmaAdiField;
            }
            set {
                this.firmaAdiField = value;
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
        public double Miktar {
            get {
                return this.miktarField;
            }
            set {
                this.miktarField = value;
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
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class Destekler {
        
        private string destekAdiField;
        
        private double tutarField;
        
        /// <remarks/>
        public string DestekAdi {
            get {
                return this.destekAdiField;
            }
            set {
                this.destekAdiField = value;
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
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class Hissedarlar {
        
        private string tCKimlikNoField;
        
        private string vergiNoField;
        
        private string adiField;
        
        private string soyadiField;
        
        private decimal alanField;
        
        /// <remarks/>
        public string TCKimlikNo {
            get {
                return this.tCKimlikNoField;
            }
            set {
                this.tCKimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public string VergiNo {
            get {
                return this.vergiNoField;
            }
            set {
                this.vergiNoField = value;
            }
        }
        
        /// <remarks/>
        public string Adi {
            get {
                return this.adiField;
            }
            set {
                this.adiField = value;
            }
        }
        
        /// <remarks/>
        public string Soyadi {
            get {
                return this.soyadiField;
            }
            set {
                this.soyadiField = value;
            }
        }
        
        /// <remarks/>
        public decimal Alan {
            get {
                return this.alanField;
            }
            set {
                this.alanField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class Urunler {
        
        private string urunAdiField;
        
        private string uretimCesidiField;
        
        private string ekilenAlanField;
        
        /// <remarks/>
        public string urunAdi {
            get {
                return this.urunAdiField;
            }
            set {
                this.urunAdiField = value;
            }
        }
        
        /// <remarks/>
        public string uretimCesidi {
            get {
                return this.uretimCesidiField;
            }
            set {
                this.uretimCesidiField = value;
            }
        }
        
        /// <remarks/>
        public string ekilenAlan {
            get {
                return this.ekilenAlanField;
            }
            set {
                this.ekilenAlanField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class Araziler {
        
        private string kesifRaporTarihiField;
        
        private string belgeTuruField;
        
        private string ilKoduField;
        
        private string ilField;
        
        private string ilceKoduField;
        
        private string ilceField;
        
        private string bucakKoduField;
        
        private string bucakField;
        
        private string koyKoduField;
        
        private string koyField;
        
        private string adaNoField;
        
        private string parselNoField;
        
        private string parselAlanField;
        
        private string suDurumuField;
        
        private string araziAlanField;
        
        private string bolmeNoField;
        
        private string sahaTahsisIzinNoField;
        
        private Urunler[] urunField;
        
        private Hissedarlar[] hissedarField;
        
        /// <remarks/>
        public string KesifRaporTarihi {
            get {
                return this.kesifRaporTarihiField;
            }
            set {
                this.kesifRaporTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string BelgeTuru {
            get {
                return this.belgeTuruField;
            }
            set {
                this.belgeTuruField = value;
            }
        }
        
        /// <remarks/>
        public string ilKodu {
            get {
                return this.ilKoduField;
            }
            set {
                this.ilKoduField = value;
            }
        }
        
        /// <remarks/>
        public string il {
            get {
                return this.ilField;
            }
            set {
                this.ilField = value;
            }
        }
        
        /// <remarks/>
        public string ilceKodu {
            get {
                return this.ilceKoduField;
            }
            set {
                this.ilceKoduField = value;
            }
        }
        
        /// <remarks/>
        public string ilce {
            get {
                return this.ilceField;
            }
            set {
                this.ilceField = value;
            }
        }
        
        /// <remarks/>
        public string bucakKodu {
            get {
                return this.bucakKoduField;
            }
            set {
                this.bucakKoduField = value;
            }
        }
        
        /// <remarks/>
        public string bucak {
            get {
                return this.bucakField;
            }
            set {
                this.bucakField = value;
            }
        }
        
        /// <remarks/>
        public string koyKodu {
            get {
                return this.koyKoduField;
            }
            set {
                this.koyKoduField = value;
            }
        }
        
        /// <remarks/>
        public string koy {
            get {
                return this.koyField;
            }
            set {
                this.koyField = value;
            }
        }
        
        /// <remarks/>
        public string adaNo {
            get {
                return this.adaNoField;
            }
            set {
                this.adaNoField = value;
            }
        }
        
        /// <remarks/>
        public string parselNo {
            get {
                return this.parselNoField;
            }
            set {
                this.parselNoField = value;
            }
        }
        
        /// <remarks/>
        public string parselAlan {
            get {
                return this.parselAlanField;
            }
            set {
                this.parselAlanField = value;
            }
        }
        
        /// <remarks/>
        public string suDurumu {
            get {
                return this.suDurumuField;
            }
            set {
                this.suDurumuField = value;
            }
        }
        
        /// <remarks/>
        public string araziAlan {
            get {
                return this.araziAlanField;
            }
            set {
                this.araziAlanField = value;
            }
        }
        
        /// <remarks/>
        public string BolmeNo {
            get {
                return this.bolmeNoField;
            }
            set {
                this.bolmeNoField = value;
            }
        }
        
        /// <remarks/>
        public string SahaTahsisIzinNo {
            get {
                return this.sahaTahsisIzinNoField;
            }
            set {
                this.sahaTahsisIzinNoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Urunler[] urun {
            get {
                return this.urunField;
            }
            set {
                this.urunField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Hissedarlar[] hissedar {
            get {
                return this.hissedarField;
            }
            set {
                this.hissedarField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class Isletmeler {
        
        private string uretimSezonuField;
        
        private string isletmeDurumuField;
        
        private string kimlikNoField;
        
        private string vergiNoField;
        
        private string adiField;
        
        private string soyadiField;
        
        private string babaAdiField;
        
        private string dogumTarihiField;
        
        private string ilKoduField;
        
        private string ilField;
        
        private string ilceKoduField;
        
        private string ilceField;
        
        private string bucakKoduField;
        
        private string bucakField;
        
        private string koyKoduField;
        
        private string koyField;
        
        private Araziler[] arazilerField;
        
        private Destekler[] desteklerField;
        
        private Faturalar[] faturalarField;
        
        /// <remarks/>
        public string uretimSezonu {
            get {
                return this.uretimSezonuField;
            }
            set {
                this.uretimSezonuField = value;
            }
        }
        
        /// <remarks/>
        public string isletmeDurumu {
            get {
                return this.isletmeDurumuField;
            }
            set {
                this.isletmeDurumuField = value;
            }
        }
        
        /// <remarks/>
        public string kimlikNo {
            get {
                return this.kimlikNoField;
            }
            set {
                this.kimlikNoField = value;
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
        public string adi {
            get {
                return this.adiField;
            }
            set {
                this.adiField = value;
            }
        }
        
        /// <remarks/>
        public string soyadi {
            get {
                return this.soyadiField;
            }
            set {
                this.soyadiField = value;
            }
        }
        
        /// <remarks/>
        public string babaAdi {
            get {
                return this.babaAdiField;
            }
            set {
                this.babaAdiField = value;
            }
        }
        
        /// <remarks/>
        public string dogumTarihi {
            get {
                return this.dogumTarihiField;
            }
            set {
                this.dogumTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string ilKodu {
            get {
                return this.ilKoduField;
            }
            set {
                this.ilKoduField = value;
            }
        }
        
        /// <remarks/>
        public string il {
            get {
                return this.ilField;
            }
            set {
                this.ilField = value;
            }
        }
        
        /// <remarks/>
        public string ilceKodu {
            get {
                return this.ilceKoduField;
            }
            set {
                this.ilceKoduField = value;
            }
        }
        
        /// <remarks/>
        public string ilce {
            get {
                return this.ilceField;
            }
            set {
                this.ilceField = value;
            }
        }
        
        /// <remarks/>
        public string bucakKodu {
            get {
                return this.bucakKoduField;
            }
            set {
                this.bucakKoduField = value;
            }
        }
        
        /// <remarks/>
        public string bucak {
            get {
                return this.bucakField;
            }
            set {
                this.bucakField = value;
            }
        }
        
        /// <remarks/>
        public string koyKodu {
            get {
                return this.koyKoduField;
            }
            set {
                this.koyKoduField = value;
            }
        }
        
        /// <remarks/>
        public string koy {
            get {
                return this.koyField;
            }
            set {
                this.koyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Araziler[] araziler {
            get {
                return this.arazilerField;
            }
            set {
                this.arazilerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Destekler[] destekler {
            get {
                return this.desteklerField;
            }
            set {
                this.desteklerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Faturalar[] faturalar {
            get {
                return this.faturalarField;
            }
            set {
                this.faturalarField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class IsletmeDetayBilgileri {
        
        private string dogrulamaField;
        
        private Isletmeler[] isletmelerField;
        
        /// <remarks/>
        public string dogrulama {
            get {
                return this.dogrulamaField;
            }
            set {
                this.dogrulamaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Isletmeler[] isletmeler {
            get {
                return this.isletmelerField;
            }
            set {
                this.isletmelerField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class YerlesimKodu {
        
        private string ilIDField;
        
        private string ilAdiField;
        
        private string ilceIDField;
        
        private string ilceAdiField;
        
        private string bucakIDField;
        
        private string bucakAdiField;
        
        private string koyIDField;
        
        private string koyAdiField;
        
        /// <remarks/>
        public string IlID {
            get {
                return this.ilIDField;
            }
            set {
                this.ilIDField = value;
            }
        }
        
        /// <remarks/>
        public string IlAdi {
            get {
                return this.ilAdiField;
            }
            set {
                this.ilAdiField = value;
            }
        }
        
        /// <remarks/>
        public string IlceID {
            get {
                return this.ilceIDField;
            }
            set {
                this.ilceIDField = value;
            }
        }
        
        /// <remarks/>
        public string IlceAdi {
            get {
                return this.ilceAdiField;
            }
            set {
                this.ilceAdiField = value;
            }
        }
        
        /// <remarks/>
        public string BucakID {
            get {
                return this.bucakIDField;
            }
            set {
                this.bucakIDField = value;
            }
        }
        
        /// <remarks/>
        public string BucakAdi {
            get {
                return this.bucakAdiField;
            }
            set {
                this.bucakAdiField = value;
            }
        }
        
        /// <remarks/>
        public string KoyID {
            get {
                return this.koyIDField;
            }
            set {
                this.koyIDField = value;
            }
        }
        
        /// <remarks/>
        public string KoyAdi {
            get {
                return this.koyAdiField;
            }
            set {
                this.koyAdiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class YerlesimBilgileri {
        
        private string dogrulamaField;
        
        private YerlesimKodu[] yerlesimBilgiField;
        
        /// <remarks/>
        public string dogrulama {
            get {
                return this.dogrulamaField;
            }
            set {
                this.dogrulamaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public YerlesimKodu[] YerlesimBilgi {
            get {
                return this.yerlesimBilgiField;
            }
            set {
                this.yerlesimBilgiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class ClassDestekler {
        
        private string destekAdiField;
        
        private double tutarField;
        
        /// <remarks/>
        public string DestekAdi {
            get {
                return this.destekAdiField;
            }
            set {
                this.destekAdiField = value;
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
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class ClassIsletmeler {
        
        private string dogrulamaField;
        
        private string isletmeNoField;
        
        private string adiField;
        
        private string soyadiField;
        
        private ClassDestekler[] desteklerField;
        
        /// <remarks/>
        public string dogrulama {
            get {
                return this.dogrulamaField;
            }
            set {
                this.dogrulamaField = value;
            }
        }
        
        /// <remarks/>
        public string IsletmeNo {
            get {
                return this.isletmeNoField;
            }
            set {
                this.isletmeNoField = value;
            }
        }
        
        /// <remarks/>
        public string adi {
            get {
                return this.adiField;
            }
            set {
                this.adiField = value;
            }
        }
        
        /// <remarks/>
        public string soyadi {
            get {
                return this.soyadiField;
            }
            set {
                this.soyadiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public ClassDestekler[] destekler {
            get {
                return this.desteklerField;
            }
            set {
                this.desteklerField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class IsletmeDestek {
        
        private ClassIsletmeler[] isletmelerField;
        
        private string dogrulamaField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public ClassIsletmeler[] Isletmeler {
            get {
                return this.isletmelerField;
            }
            set {
                this.isletmelerField = value;
            }
        }
        
        /// <remarks/>
        public string dogrulama {
            get {
                return this.dogrulamaField;
            }
            set {
                this.dogrulamaField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class Bilgi {
        
        private string isletmeNoField;
        
        private string isletmeAdiField;
        
        private string nedeniField;
        
        private string kararTarihiField;
        
        private string baslangicTarihiField;
        
        private string bitisTarihiField;
        
        /// <remarks/>
        public string isletmeNo {
            get {
                return this.isletmeNoField;
            }
            set {
                this.isletmeNoField = value;
            }
        }
        
        /// <remarks/>
        public string isletmeAdi {
            get {
                return this.isletmeAdiField;
            }
            set {
                this.isletmeAdiField = value;
            }
        }
        
        /// <remarks/>
        public string Nedeni {
            get {
                return this.nedeniField;
            }
            set {
                this.nedeniField = value;
            }
        }
        
        /// <remarks/>
        public string KararTarihi {
            get {
                return this.kararTarihiField;
            }
            set {
                this.kararTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string BaslangicTarihi {
            get {
                return this.baslangicTarihiField;
            }
            set {
                this.baslangicTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string BitisTarihi {
            get {
                return this.bitisTarihiField;
            }
            set {
                this.bitisTarihiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class DestekDisiBilgileri {
        
        private string dogrulamaField;
        
        private Bilgi[] bilgilerField;
        
        /// <remarks/>
        public string dogrulama {
            get {
                return this.dogrulamaField;
            }
            set {
                this.dogrulamaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Bilgi[] bilgiler {
            get {
                return this.bilgilerField;
            }
            set {
                this.bilgilerField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public partial class ServiceResult {
        
        private bool succededField;
        
        private string errorMessageField;
        
        private System.Nullable<int> errorCodeField;
        
        /// <remarks/>
        public bool Succeded {
            get {
                return this.succededField;
            }
            set {
                this.succededField = value;
            }
        }
        
        /// <remarks/>
        public string ErrorMessage {
            get {
                return this.errorMessageField;
            }
            set {
                this.errorMessageField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> ErrorCode {
            get {
                return this.errorCodeField;
            }
            set {
                this.errorCodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(SlaughteredAnimalInfoServiceEntity))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(BreedInfoInHoldingServiceEntity))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AnimalInfoInHoldingServiceEntity))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AnimalServiceEntity))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AnimalInfoServiceEntity))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(HoldingInfoServiceEntity))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public abstract partial class BaseServiceEntity {
        
        private ServiceResult resultField;
        
        /// <remarks/>
        public ServiceResult Result {
            get {
                return this.resultField;
            }
            set {
                this.resultField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public partial class SlaughteredAnimalInfoServiceEntity : BaseServiceEntity {
        
        private string tagIdField;
        
        private string provinceField;
        
        private string districtField;
        
        private string neighbourhoodField;
        
        private System.Nullable<long> ownerNationalIdField;
        
        private string ownerNameField;
        
        private string ownerSurnameField;
        
        private string holdingIdField;
        
        private System.Nullable<PrmAnimalSexType> sexField;
        
        private System.Nullable<System.DateTime> birthDateField;
        
        private System.Nullable<System.DateTime> deathDateField;
        
        private System.Nullable<PrmAnimalDeathType> deathTypeField;
        
        private string fatherIdField;
        
        private string motherIdField;
        
        private string birthHoldingIdField;
        
        private System.Nullable<PrmAnimalBreed> breedField;
        
        /// <remarks/>
        public string TagId {
            get {
                return this.tagIdField;
            }
            set {
                this.tagIdField = value;
            }
        }
        
        /// <remarks/>
        public string Province {
            get {
                return this.provinceField;
            }
            set {
                this.provinceField = value;
            }
        }
        
        /// <remarks/>
        public string District {
            get {
                return this.districtField;
            }
            set {
                this.districtField = value;
            }
        }
        
        /// <remarks/>
        public string Neighbourhood {
            get {
                return this.neighbourhoodField;
            }
            set {
                this.neighbourhoodField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<long> OwnerNationalId {
            get {
                return this.ownerNationalIdField;
            }
            set {
                this.ownerNationalIdField = value;
            }
        }
        
        /// <remarks/>
        public string OwnerName {
            get {
                return this.ownerNameField;
            }
            set {
                this.ownerNameField = value;
            }
        }
        
        /// <remarks/>
        public string OwnerSurname {
            get {
                return this.ownerSurnameField;
            }
            set {
                this.ownerSurnameField = value;
            }
        }
        
        /// <remarks/>
        public string HoldingId {
            get {
                return this.holdingIdField;
            }
            set {
                this.holdingIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmAnimalSexType> Sex {
            get {
                return this.sexField;
            }
            set {
                this.sexField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<System.DateTime> BirthDate {
            get {
                return this.birthDateField;
            }
            set {
                this.birthDateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<System.DateTime> DeathDate {
            get {
                return this.deathDateField;
            }
            set {
                this.deathDateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmAnimalDeathType> DeathType {
            get {
                return this.deathTypeField;
            }
            set {
                this.deathTypeField = value;
            }
        }
        
        /// <remarks/>
        public string FatherId {
            get {
                return this.fatherIdField;
            }
            set {
                this.fatherIdField = value;
            }
        }
        
        /// <remarks/>
        public string MotherId {
            get {
                return this.motherIdField;
            }
            set {
                this.motherIdField = value;
            }
        }
        
        /// <remarks/>
        public string BirthHoldingId {
            get {
                return this.birthHoldingIdField;
            }
            set {
                this.birthHoldingIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmAnimalBreed> Breed {
            get {
                return this.breedField;
            }
            set {
                this.breedField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public enum PrmAnimalSexType {
        
        /// <remarks/>
        Male,
        
        /// <remarks/>
        Female,
        
        /// <remarks/>
        Unknown,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public enum PrmAnimalDeathType {
        
        /// <remarks/>
        CombineCut,
        
        /// <remarks/>
        Destruction,
        
        /// <remarks/>
        SeperateCut,
        
        /// <remarks/>
        PersonalDestruction,
        
        /// <remarks/>
        PersonalCut,
        
        /// <remarks/>
        Accidentally,
        
        /// <remarks/>
        Stillborn,
        
        /// <remarks/>
        Natural,
        
        /// <remarks/>
        Transport,
        
        /// <remarks/>
        Illness,
        
        /// <remarks/>
        Sacrificial,
        
        /// <remarks/>
        Cancer,
        
        /// <remarks/>
        Unknown,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public enum PrmAnimalBreed {
        
        /// <remarks/>
        BELCIKAMAVISI,
        
        /// <remarks/>
        GAKMGUNEYDOGUANADOLUKIRMIZISI,
        
        /// <remarks/>
        DAKMDOGUANADOLUKIRMIZISI,
        
        /// <remarks/>
        BOZIRKM,
        
        /// <remarks/>
        YERLIGUNEYSARISI,
        
        /// <remarks/>
        ZAVOTM,
        
        /// <remarks/>
        YERLIGUNEYSARISIM,
        
        /// <remarks/>
        BELCIKAMAVISIMELEZI,
        
        /// <remarks/>
        LIMOUSIN,
        
        /// <remarks/>
        MANDA,
        
        /// <remarks/>
        YERLI,
        
        /// <remarks/>
        YERLIKARA,
        
        /// <remarks/>
        GAKGUNEYDOGUANADOLUKIRMIZISI,
        
        /// <remarks/>
        DAKDOGUANADOLUKIRMIZISI,
        
        /// <remarks/>
        BOZIRK,
        
        /// <remarks/>
        ZAVOT,
        
        /// <remarks/>
        ESMERIRK,
        
        /// <remarks/>
        KIRIM,
        
        /// <remarks/>
        BONIHAD,
        
        /// <remarks/>
        MARAS,
        
        /// <remarks/>
        HOLSTAYN,
        
        /// <remarks/>
        BRANGUS,
        
        /// <remarks/>
        MONTOFON,
        
        /// <remarks/>
        JERSEY,
        
        /// <remarks/>
        SIMENTAL,
        
        /// <remarks/>
        SAROLE,
        
        /// <remarks/>
        MONTBELIARDE,
        
        /// <remarks/>
        ANGUS,
        
        /// <remarks/>
        LIMOUSINE,
        
        /// <remarks/>
        HOLSTAYNM,
        
        /// <remarks/>
        KBHOLSTAYNM,
        
        /// <remarks/>
        MONTOFONM,
        
        /// <remarks/>
        JERSEYM,
        
        /// <remarks/>
        SIMENTALM,
        
        /// <remarks/>
        SAROLEM,
        
        /// <remarks/>
        MONTBELIARDEM,
        
        /// <remarks/>
        ANGUSM,
        
        /// <remarks/>
        LIMOUSINEM,
        
        /// <remarks/>
        HE,
        
        /// <remarks/>
        HEREFORDM,
        
        /// <remarks/>
        SRB,
        
        /// <remarks/>
        ISVECKIRMIZISI,
        
        /// <remarks/>
        HEREFORD,
        
        /// <remarks/>
        ISVECKIRMIZISIM,
        
        /// <remarks/>
        KIRMIZIBEYAZALACA,
        
        /// <remarks/>
        ESMERIRKM,
        
        /// <remarks/>
        YERLIKARAM,
        
        /// <remarks/>
        PIEMONTESE,
        
        /// <remarks/>
        PIEMONTESEM,
        
        /// <remarks/>
        BRANGUSM,
        
        /// <remarks/>
        CHIANINA,
        
        /// <remarks/>
        CHIANINAM,
        
        /// <remarks/>
        REDANGUS,
        
        /// <remarks/>
        REDANGUSM,
        
        /// <remarks/>
        MARCHIGIANA,
        
        /// <remarks/>
        MARCHIGIANAM,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public partial class BreedInfoInHoldingServiceEntity : BaseServiceEntity {
        
        private string holdingIdField;
        
        private System.Nullable<int> breedCodeField;
        
        private string breedDescriptionField;
        
        private System.Nullable<int> breedCountField;
        
        /// <remarks/>
        public string HoldingId {
            get {
                return this.holdingIdField;
            }
            set {
                this.holdingIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> BreedCode {
            get {
                return this.breedCodeField;
            }
            set {
                this.breedCodeField = value;
            }
        }
        
        /// <remarks/>
        public string BreedDescription {
            get {
                return this.breedDescriptionField;
            }
            set {
                this.breedDescriptionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> BreedCount {
            get {
                return this.breedCountField;
            }
            set {
                this.breedCountField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public partial class AnimalInfoInHoldingServiceEntity : BaseServiceEntity {
        
        private string tagIdField;
        
        private string holdingIdField;
        
        private System.Nullable<int> sexField;
        
        private string sexDescriptionField;
        
        private System.Nullable<System.DateTime> birthDateField;
        
        private string fatherIdField;
        
        private string motherIdField;
        
        private string birthHoldingIdField;
        
        private System.Nullable<int> breedCodeField;
        
        private string breedDescriptionField;
        
        /// <remarks/>
        public string TagId {
            get {
                return this.tagIdField;
            }
            set {
                this.tagIdField = value;
            }
        }
        
        /// <remarks/>
        public string HoldingId {
            get {
                return this.holdingIdField;
            }
            set {
                this.holdingIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> Sex {
            get {
                return this.sexField;
            }
            set {
                this.sexField = value;
            }
        }
        
        /// <remarks/>
        public string SexDescription {
            get {
                return this.sexDescriptionField;
            }
            set {
                this.sexDescriptionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<System.DateTime> BirthDate {
            get {
                return this.birthDateField;
            }
            set {
                this.birthDateField = value;
            }
        }
        
        /// <remarks/>
        public string FatherId {
            get {
                return this.fatherIdField;
            }
            set {
                this.fatherIdField = value;
            }
        }
        
        /// <remarks/>
        public string MotherId {
            get {
                return this.motherIdField;
            }
            set {
                this.motherIdField = value;
            }
        }
        
        /// <remarks/>
        public string BirthHoldingId {
            get {
                return this.birthHoldingIdField;
            }
            set {
                this.birthHoldingIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> BreedCode {
            get {
                return this.breedCodeField;
            }
            set {
                this.breedCodeField = value;
            }
        }
        
        /// <remarks/>
        public string BreedDescription {
            get {
                return this.breedDescriptionField;
            }
            set {
                this.breedDescriptionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AnimalInfoServiceEntity))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public partial class AnimalServiceEntity : BaseServiceEntity {
        
        private string tagIdField;
        
        private string nameField;
        
        private System.Nullable<PrmAnimalSexType> sexField;
        
        private System.Nullable<PrmAnimalBreed> breedField;
        
        private System.Nullable<long> earTagFixedByNationalIdField;
        
        private System.Nullable<System.DateTime> earTagFixedDateField;
        
        private System.Nullable<System.DateTime> birthDateField;
        
        private System.Nullable<PrmAnimalUsageType> animalUsageTypeField;
        
        private string birthHoldingIdField;
        
        private System.Nullable<int> birthHerdField;
        
        private System.Nullable<PrmBirthType> birthTypeField;
        
        private System.Nullable<long> ownerNationalIdField;
        
        private System.Nullable<long> ownerFirmIdField;
        
        private System.Nullable<PrmAnswer> pedigreeField;
        
        private System.Nullable<PrmPedigreeValue> pedigreeValueField;
        
        private string fatherIdField;
        
        private string motherIdField;
        
        private System.Nullable<PrmAnswer> castrationStatusField;
        
        private System.Nullable<System.DateTime> castrationDateField;
        
        private string animalCommentField;
        
        /// <remarks/>
        public string TagId {
            get {
                return this.tagIdField;
            }
            set {
                this.tagIdField = value;
            }
        }
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmAnimalSexType> Sex {
            get {
                return this.sexField;
            }
            set {
                this.sexField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmAnimalBreed> Breed {
            get {
                return this.breedField;
            }
            set {
                this.breedField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<long> EarTagFixedByNationalId {
            get {
                return this.earTagFixedByNationalIdField;
            }
            set {
                this.earTagFixedByNationalIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<System.DateTime> EarTagFixedDate {
            get {
                return this.earTagFixedDateField;
            }
            set {
                this.earTagFixedDateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<System.DateTime> BirthDate {
            get {
                return this.birthDateField;
            }
            set {
                this.birthDateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmAnimalUsageType> AnimalUsageType {
            get {
                return this.animalUsageTypeField;
            }
            set {
                this.animalUsageTypeField = value;
            }
        }
        
        /// <remarks/>
        public string BirthHoldingId {
            get {
                return this.birthHoldingIdField;
            }
            set {
                this.birthHoldingIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> BirthHerd {
            get {
                return this.birthHerdField;
            }
            set {
                this.birthHerdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmBirthType> BirthType {
            get {
                return this.birthTypeField;
            }
            set {
                this.birthTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<long> OwnerNationalId {
            get {
                return this.ownerNationalIdField;
            }
            set {
                this.ownerNationalIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<long> OwnerFirmId {
            get {
                return this.ownerFirmIdField;
            }
            set {
                this.ownerFirmIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmAnswer> Pedigree {
            get {
                return this.pedigreeField;
            }
            set {
                this.pedigreeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmPedigreeValue> PedigreeValue {
            get {
                return this.pedigreeValueField;
            }
            set {
                this.pedigreeValueField = value;
            }
        }
        
        /// <remarks/>
        public string FatherId {
            get {
                return this.fatherIdField;
            }
            set {
                this.fatherIdField = value;
            }
        }
        
        /// <remarks/>
        public string MotherId {
            get {
                return this.motherIdField;
            }
            set {
                this.motherIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmAnswer> CastrationStatus {
            get {
                return this.castrationStatusField;
            }
            set {
                this.castrationStatusField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<System.DateTime> CastrationDate {
            get {
                return this.castrationDateField;
            }
            set {
                this.castrationDateField = value;
            }
        }
        
        /// <remarks/>
        public string AnimalComment {
            get {
                return this.animalCommentField;
            }
            set {
                this.animalCommentField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public enum PrmAnimalUsageType {
        
        /// <remarks/>
        Stood,
        
        /// <remarks/>
        Meat,
        
        /// <remarks/>
        Milk,
        
        /// <remarks/>
        Research,
        
        /// <remarks/>
        MultiAim,
        
        /// <remarks/>
        Bullock,
        
        /// <remarks/>
        Fatten,
        
        /// <remarks/>
        FattenKastre,
        
        /// <remarks/>
        SyntheticGBull,
        
        /// <remarks/>
        NaturalGBull,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public enum PrmBirthType {
        
        /// <remarks/>
        Single,
        
        /// <remarks/>
        Twin,
        
        /// <remarks/>
        Triplet,
        
        /// <remarks/>
        Quad,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public enum PrmAnswer {
        
        /// <remarks/>
        Yes,
        
        /// <remarks/>
        No,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public enum PrmPedigreeValue {
        
        /// <remarks/>
        BlueCertificate,
        
        /// <remarks/>
        WhiteCertificate,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public partial class AnimalInfoServiceEntity : AnimalServiceEntity {
        
        private string holdingIdField;
        
        private System.Nullable<int> herdField;
        
        private System.Nullable<PrmAnimalStatus> statusField;
        
        private System.Nullable<PrmAnimalSpecies> speciesField;
        
        private System.Nullable<System.DateTime> deathDateField;
        
        private System.Nullable<PrmAnimalDeathType> deathTypeField;
        
        private System.Nullable<System.DateTime> foundDateField;
        
        private System.Nullable<System.DateTime> lostDateField;
        
        private System.Nullable<PrmAnswer> hasGivenBirthField;
        
        private System.Nullable<PrmAnswer> importedField;
        
        private System.Nullable<PrmAnswer> importedForSlaughterField;
        
        private System.Nullable<decimal> liveWeightField;
        
        private System.Nullable<PrmAnswer> suspectField;
        
        /// <remarks/>
        public string HoldingId {
            get {
                return this.holdingIdField;
            }
            set {
                this.holdingIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> Herd {
            get {
                return this.herdField;
            }
            set {
                this.herdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmAnimalStatus> Status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmAnimalSpecies> Species {
            get {
                return this.speciesField;
            }
            set {
                this.speciesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<System.DateTime> DeathDate {
            get {
                return this.deathDateField;
            }
            set {
                this.deathDateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmAnimalDeathType> DeathType {
            get {
                return this.deathTypeField;
            }
            set {
                this.deathTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<System.DateTime> FoundDate {
            get {
                return this.foundDateField;
            }
            set {
                this.foundDateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<System.DateTime> LostDate {
            get {
                return this.lostDateField;
            }
            set {
                this.lostDateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmAnswer> HasGivenBirth {
            get {
                return this.hasGivenBirthField;
            }
            set {
                this.hasGivenBirthField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmAnswer> Imported {
            get {
                return this.importedField;
            }
            set {
                this.importedField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmAnswer> ImportedForSlaughter {
            get {
                return this.importedForSlaughterField;
            }
            set {
                this.importedForSlaughterField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<decimal> LiveWeight {
            get {
                return this.liveWeightField;
            }
            set {
                this.liveWeightField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmAnswer> Suspect {
            get {
                return this.suspectField;
            }
            set {
                this.suspectField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public enum PrmAnimalStatus {
        
        /// <remarks/>
        Alive,
        
        /// <remarks/>
        Dead,
        
        /// <remarks/>
        Deleted,
        
        /// <remarks/>
        Exported,
        
        /// <remarks/>
        Stolen,
        
        /// <remarks/>
        Lost,
        
        /// <remarks/>
        Unknown,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public enum PrmAnimalSpecies {
        
        /// <remarks/>
        Mule,
        
        /// <remarks/>
        Donkey,
        
        /// <remarks/>
        Cattle,
        
        /// <remarks/>
        Buffalo,
        
        /// <remarks/>
        Winged,
        
        /// <remarks/>
        Goat,
        
        /// <remarks/>
        Wild,
        
        /// <remarks/>
        Horse,
        
        /// <remarks/>
        Dog,
        
        /// <remarks/>
        Cat,
        
        /// <remarks/>
        Sheep,
        
        /// <remarks/>
        Pig,
        
        /// <remarks/>
        Fish,
        
        /// <remarks/>
        Bee,
        
        /// <remarks/>
        Rabbit,
        
        /// <remarks/>
        Common,
        
        /// <remarks/>
        Other,
        
        /// <remarks/>
        Gazelle,
        
        /// <remarks/>
        Bat,
        
        /// <remarks/>
        Mouse,
        
        /// <remarks/>
        LaboratoryAnimal,
        
        /// <remarks/>
        Pigeon,
        
        /// <remarks/>
        Cavy,
        
        /// <remarks/>
        Cow,
        
        /// <remarks/>
        Lamb,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public partial class HoldingInfoServiceEntity : BaseServiceEntity {
        
        private string holdingIdField;
        
        private string provinceField;
        
        private string districtField;
        
        private string villageField;
        
        private System.Nullable<long> ownerFirmIdField;
        
        private string ownerFirmNameField;
        
        private string ownerFirmTaxNumberField;
        
        private System.Nullable<long> ownerNationalIdField;
        
        private string ownerNameField;
        
        private string ownerSurnameField;
        
        private string ownerFatherNameField;
        
        private System.Nullable<PrmHoldingStatus> statusField;
        
        /// <remarks/>
        public string HoldingId {
            get {
                return this.holdingIdField;
            }
            set {
                this.holdingIdField = value;
            }
        }
        
        /// <remarks/>
        public string Province {
            get {
                return this.provinceField;
            }
            set {
                this.provinceField = value;
            }
        }
        
        /// <remarks/>
        public string District {
            get {
                return this.districtField;
            }
            set {
                this.districtField = value;
            }
        }
        
        /// <remarks/>
        public string Village {
            get {
                return this.villageField;
            }
            set {
                this.villageField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<long> OwnerFirmId {
            get {
                return this.ownerFirmIdField;
            }
            set {
                this.ownerFirmIdField = value;
            }
        }
        
        /// <remarks/>
        public string OwnerFirmName {
            get {
                return this.ownerFirmNameField;
            }
            set {
                this.ownerFirmNameField = value;
            }
        }
        
        /// <remarks/>
        public string OwnerFirmTaxNumber {
            get {
                return this.ownerFirmTaxNumberField;
            }
            set {
                this.ownerFirmTaxNumberField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<long> OwnerNationalId {
            get {
                return this.ownerNationalIdField;
            }
            set {
                this.ownerNationalIdField = value;
            }
        }
        
        /// <remarks/>
        public string OwnerName {
            get {
                return this.ownerNameField;
            }
            set {
                this.ownerNameField = value;
            }
        }
        
        /// <remarks/>
        public string OwnerSurname {
            get {
                return this.ownerSurnameField;
            }
            set {
                this.ownerSurnameField = value;
            }
        }
        
        /// <remarks/>
        public string OwnerFatherName {
            get {
                return this.ownerFatherNameField;
            }
            set {
                this.ownerFatherNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<PrmHoldingStatus> Status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://turkvet.gov.tr/")]
    public enum PrmHoldingStatus {
        
        /// <remarks/>
        Active,
        
        /// <remarks/>
        Passive,
        
        /// <remarks/>
        Closed,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class odeme {
        
        private string odemeIDField;
        
        private string odemeAdiField;
        
        /// <remarks/>
        public string odemeID {
            get {
                return this.odemeIDField;
            }
            set {
                this.odemeIDField = value;
            }
        }
        
        /// <remarks/>
        public string odemeAdi {
            get {
                return this.odemeAdiField;
            }
            set {
                this.odemeAdiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class OdemeTuru {
        
        private string dogrulamaField;
        
        private odeme[] odemelerField;
        
        /// <remarks/>
        public string dogrulama {
            get {
                return this.dogrulamaField;
            }
            set {
                this.dogrulamaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public odeme[] odemeler {
            get {
                return this.odemelerField;
            }
            set {
                this.odemelerField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class Ilceler {
        
        private string ilceIDField;
        
        private string ilceAdiField;
        
        /// <remarks/>
        public string IlceID {
            get {
                return this.ilceIDField;
            }
            set {
                this.ilceIDField = value;
            }
        }
        
        /// <remarks/>
        public string IlceAdi {
            get {
                return this.ilceAdiField;
            }
            set {
                this.ilceAdiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class Iller {
        
        private string ilIDField;
        
        private string ilAdiField;
        
        private Ilceler[] ilcelerField;
        
        /// <remarks/>
        public string IlID {
            get {
                return this.ilIDField;
            }
            set {
                this.ilIDField = value;
            }
        }
        
        /// <remarks/>
        public string IlAdi {
            get {
                return this.ilAdiField;
            }
            set {
                this.ilAdiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Ilceler[] Ilceler {
            get {
                return this.ilcelerField;
            }
            set {
                this.ilcelerField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class IlIlceKod {
        
        private string dogrulamaField;
        
        private Iller[] illerBilgiField;
        
        /// <remarks/>
        public string dogrulama {
            get {
                return this.dogrulamaField;
            }
            set {
                this.dogrulamaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Iller[] IllerBilgi {
            get {
                return this.illerBilgiField;
            }
            set {
                this.illerBilgiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class araziUrun {
        
        private string bilgiField;
        
        private string hataDurumuField;
        
        /// <remarks/>
        public string bilgi {
            get {
                return this.bilgiField;
            }
            set {
                this.bilgiField = value;
            }
        }
        
        /// <remarks/>
        public string hataDurumu {
            get {
                return this.hataDurumuField;
            }
            set {
                this.hataDurumuField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class urun {
        
        private string urunIDField;
        
        private string urunAdiField;
        
        /// <remarks/>
        public string urunID {
            get {
                return this.urunIDField;
            }
            set {
                this.urunIDField = value;
            }
        }
        
        /// <remarks/>
        public string urunAdi {
            get {
                return this.urunAdiField;
            }
            set {
                this.urunAdiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class cksUrun {
        
        private string dogrulamaField;
        
        private urun[] urunlerField;
        
        /// <remarks/>
        public string dogrulama {
            get {
                return this.dogrulamaField;
            }
            set {
                this.dogrulamaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public urun[] urunler {
            get {
                return this.urunlerField;
            }
            set {
                this.urunlerField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class UrunSorgu {
        
        private string urunIDField;
        
        private string urunAdiField;
        
        private string uretimCesidiField;
        
        private string sertifikaliVerimField;
        
        private string sertifikasizVerimField;
        
        private string sertifikaliVerimSuluField;
        
        private string sertifikasizVerimSuluField;
        
        private string ortualtiVerimField;
        
        private string ekilenAlanField;
        
        /// <remarks/>
        public string urunID {
            get {
                return this.urunIDField;
            }
            set {
                this.urunIDField = value;
            }
        }
        
        /// <remarks/>
        public string urunAdi {
            get {
                return this.urunAdiField;
            }
            set {
                this.urunAdiField = value;
            }
        }
        
        /// <remarks/>
        public string uretimCesidi {
            get {
                return this.uretimCesidiField;
            }
            set {
                this.uretimCesidiField = value;
            }
        }
        
        /// <remarks/>
        public string sertifikaliVerim {
            get {
                return this.sertifikaliVerimField;
            }
            set {
                this.sertifikaliVerimField = value;
            }
        }
        
        /// <remarks/>
        public string sertifikasizVerim {
            get {
                return this.sertifikasizVerimField;
            }
            set {
                this.sertifikasizVerimField = value;
            }
        }
        
        /// <remarks/>
        public string sertifikaliVerimSulu {
            get {
                return this.sertifikaliVerimSuluField;
            }
            set {
                this.sertifikaliVerimSuluField = value;
            }
        }
        
        /// <remarks/>
        public string sertifikasizVerimSulu {
            get {
                return this.sertifikasizVerimSuluField;
            }
            set {
                this.sertifikasizVerimSuluField = value;
            }
        }
        
        /// <remarks/>
        public string ortualtiVerim {
            get {
                return this.ortualtiVerimField;
            }
            set {
                this.ortualtiVerimField = value;
            }
        }
        
        /// <remarks/>
        public string ekilenAlan {
            get {
                return this.ekilenAlanField;
            }
            set {
                this.ekilenAlanField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class AraziSorgu {
        
        private string ilField;
        
        private string ilKoduField;
        
        private string ilceField;
        
        private string ilceKoduField;
        
        private string bucakField;
        
        private string bucakKoduField;
        
        private string koyField;
        
        private string koyKoduField;
        
        private string adaNoField;
        
        private string parselNoField;
        
        private string parselAlanField;
        
        private string suDurumuField;
        
        private string araziAlanField;
        
        private UrunSorgu[] urunSorguField;
        
        /// <remarks/>
        public string il {
            get {
                return this.ilField;
            }
            set {
                this.ilField = value;
            }
        }
        
        /// <remarks/>
        public string ilKodu {
            get {
                return this.ilKoduField;
            }
            set {
                this.ilKoduField = value;
            }
        }
        
        /// <remarks/>
        public string ilce {
            get {
                return this.ilceField;
            }
            set {
                this.ilceField = value;
            }
        }
        
        /// <remarks/>
        public string ilceKodu {
            get {
                return this.ilceKoduField;
            }
            set {
                this.ilceKoduField = value;
            }
        }
        
        /// <remarks/>
        public string bucak {
            get {
                return this.bucakField;
            }
            set {
                this.bucakField = value;
            }
        }
        
        /// <remarks/>
        public string bucakKodu {
            get {
                return this.bucakKoduField;
            }
            set {
                this.bucakKoduField = value;
            }
        }
        
        /// <remarks/>
        public string koy {
            get {
                return this.koyField;
            }
            set {
                this.koyField = value;
            }
        }
        
        /// <remarks/>
        public string koyKodu {
            get {
                return this.koyKoduField;
            }
            set {
                this.koyKoduField = value;
            }
        }
        
        /// <remarks/>
        public string adaNo {
            get {
                return this.adaNoField;
            }
            set {
                this.adaNoField = value;
            }
        }
        
        /// <remarks/>
        public string parselNo {
            get {
                return this.parselNoField;
            }
            set {
                this.parselNoField = value;
            }
        }
        
        /// <remarks/>
        public string parselAlan {
            get {
                return this.parselAlanField;
            }
            set {
                this.parselAlanField = value;
            }
        }
        
        /// <remarks/>
        public string suDurumu {
            get {
                return this.suDurumuField;
            }
            set {
                this.suDurumuField = value;
            }
        }
        
        /// <remarks/>
        public string araziAlan {
            get {
                return this.araziAlanField;
            }
            set {
                this.araziAlanField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public UrunSorgu[] urunSorgu {
            get {
                return this.urunSorguField;
            }
            set {
                this.urunSorguField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class IsletmeSorgu {
        
        private string uretimSezonuField;
        
        private string isletmeDurumuField;
        
        private string isletmeNoField;
        
        private string ilField;
        
        private string ilKoduField;
        
        private string ilceField;
        
        private string ilceKoduField;
        
        private string bucakField;
        
        private string bucakKoduField;
        
        private string koyField;
        
        private string koyKoduField;
        
        private string kimlikNoField;
        
        private string vergiNoField;
        
        private string adiField;
        
        private string soyadiField;
        
        private string babaAdiField;
        
        private string dogumTarihiField;
        
        private string evTelField;
        
        private string isTelField;
        
        private string cepTelField;
        
        private AraziSorgu[] araziSorguField;
        
        /// <remarks/>
        public string uretimSezonu {
            get {
                return this.uretimSezonuField;
            }
            set {
                this.uretimSezonuField = value;
            }
        }
        
        /// <remarks/>
        public string isletmeDurumu {
            get {
                return this.isletmeDurumuField;
            }
            set {
                this.isletmeDurumuField = value;
            }
        }
        
        /// <remarks/>
        public string isletmeNo {
            get {
                return this.isletmeNoField;
            }
            set {
                this.isletmeNoField = value;
            }
        }
        
        /// <remarks/>
        public string il {
            get {
                return this.ilField;
            }
            set {
                this.ilField = value;
            }
        }
        
        /// <remarks/>
        public string ilKodu {
            get {
                return this.ilKoduField;
            }
            set {
                this.ilKoduField = value;
            }
        }
        
        /// <remarks/>
        public string ilce {
            get {
                return this.ilceField;
            }
            set {
                this.ilceField = value;
            }
        }
        
        /// <remarks/>
        public string ilceKodu {
            get {
                return this.ilceKoduField;
            }
            set {
                this.ilceKoduField = value;
            }
        }
        
        /// <remarks/>
        public string bucak {
            get {
                return this.bucakField;
            }
            set {
                this.bucakField = value;
            }
        }
        
        /// <remarks/>
        public string bucakKodu {
            get {
                return this.bucakKoduField;
            }
            set {
                this.bucakKoduField = value;
            }
        }
        
        /// <remarks/>
        public string koy {
            get {
                return this.koyField;
            }
            set {
                this.koyField = value;
            }
        }
        
        /// <remarks/>
        public string koyKodu {
            get {
                return this.koyKoduField;
            }
            set {
                this.koyKoduField = value;
            }
        }
        
        /// <remarks/>
        public string kimlikNo {
            get {
                return this.kimlikNoField;
            }
            set {
                this.kimlikNoField = value;
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
        public string adi {
            get {
                return this.adiField;
            }
            set {
                this.adiField = value;
            }
        }
        
        /// <remarks/>
        public string soyadi {
            get {
                return this.soyadiField;
            }
            set {
                this.soyadiField = value;
            }
        }
        
        /// <remarks/>
        public string babaAdi {
            get {
                return this.babaAdiField;
            }
            set {
                this.babaAdiField = value;
            }
        }
        
        /// <remarks/>
        public string dogumTarihi {
            get {
                return this.dogumTarihiField;
            }
            set {
                this.dogumTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string EvTel {
            get {
                return this.evTelField;
            }
            set {
                this.evTelField = value;
            }
        }
        
        /// <remarks/>
        public string IsTel {
            get {
                return this.isTelField;
            }
            set {
                this.isTelField = value;
            }
        }
        
        /// <remarks/>
        public string CepTel {
            get {
                return this.cepTelField;
            }
            set {
                this.cepTelField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public AraziSorgu[] araziSorgu {
            get {
                return this.araziSorguField;
            }
            set {
                this.araziSorguField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class SorguGetir {
        
        private string dogrulamaField;
        
        private IsletmeSorgu[] isletmeSorguField;
        
        /// <remarks/>
        public string dogrulama {
            get {
                return this.dogrulamaField;
            }
            set {
                this.dogrulamaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public IsletmeSorgu[] isletmeSorgu {
            get {
                return this.isletmeSorguField;
            }
            set {
                this.isletmeSorguField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class OrganikTarim {
        
        private string dogrulamaField;
        
        private string isletmeTipiField;
        
        private string vergiNoField;
        
        private string kimlikNoField;
        
        private decimal organikAlanField;
        
        private decimal toplamAlanField;
        
        /// <remarks/>
        public string dogrulama {
            get {
                return this.dogrulamaField;
            }
            set {
                this.dogrulamaField = value;
            }
        }
        
        /// <remarks/>
        public string isletmeTipi {
            get {
                return this.isletmeTipiField;
            }
            set {
                this.isletmeTipiField = value;
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
        public string kimlikNo {
            get {
                return this.kimlikNoField;
            }
            set {
                this.kimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public decimal organikAlan {
            get {
                return this.organikAlanField;
            }
            set {
                this.organikAlanField = value;
            }
        }
        
        /// <remarks/>
        public decimal toplamAlan {
            get {
                return this.toplamAlanField;
            }
            set {
                this.toplamAlanField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class SuTesisi {
        
        private string ilField;
        
        private string ilKoduField;
        
        private string ilceField;
        
        private string ilceKoduField;
        
        private string bucakField;
        
        private string bucakKoduField;
        
        private string koyField;
        
        private string koyKoduField;
        
        private string urunIDField;
        
        private string urunField;
        
        private string kapasiteField;
        
        private string tesisIDField;
        
        /// <remarks/>
        public string il {
            get {
                return this.ilField;
            }
            set {
                this.ilField = value;
            }
        }
        
        /// <remarks/>
        public string ilKodu {
            get {
                return this.ilKoduField;
            }
            set {
                this.ilKoduField = value;
            }
        }
        
        /// <remarks/>
        public string ilce {
            get {
                return this.ilceField;
            }
            set {
                this.ilceField = value;
            }
        }
        
        /// <remarks/>
        public string ilceKodu {
            get {
                return this.ilceKoduField;
            }
            set {
                this.ilceKoduField = value;
            }
        }
        
        /// <remarks/>
        public string bucak {
            get {
                return this.bucakField;
            }
            set {
                this.bucakField = value;
            }
        }
        
        /// <remarks/>
        public string bucakKodu {
            get {
                return this.bucakKoduField;
            }
            set {
                this.bucakKoduField = value;
            }
        }
        
        /// <remarks/>
        public string koy {
            get {
                return this.koyField;
            }
            set {
                this.koyField = value;
            }
        }
        
        /// <remarks/>
        public string koyKodu {
            get {
                return this.koyKoduField;
            }
            set {
                this.koyKoduField = value;
            }
        }
        
        /// <remarks/>
        public string urunID {
            get {
                return this.urunIDField;
            }
            set {
                this.urunIDField = value;
            }
        }
        
        /// <remarks/>
        public string urun {
            get {
                return this.urunField;
            }
            set {
                this.urunField = value;
            }
        }
        
        /// <remarks/>
        public string kapasite {
            get {
                return this.kapasiteField;
            }
            set {
                this.kapasiteField = value;
            }
        }
        
        /// <remarks/>
        public string tesisID {
            get {
                return this.tesisIDField;
            }
            set {
                this.tesisIDField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class SuUrunleri {
        
        private string dogrulamaField;
        
        private string isletmeDurumuField;
        
        private string isletmeNoField;
        
        private string ilField;
        
        private string ilKoduField;
        
        private string ilceField;
        
        private string ilceKoduField;
        
        private string bucakField;
        
        private string bucakKoduField;
        
        private string koyField;
        
        private string koyKoduField;
        
        private string kimlikNoField;
        
        private string vergiNoField;
        
        private string adiField;
        
        private string soyadiField;
        
        private string babaAdiField;
        
        private string dogumTarihiField;
        
        private string yetistiricilikBelgeNoField;
        
        private string tesisAdiField;
        
        private SuTesisi[] suTesisleriField;
        
        /// <remarks/>
        public string dogrulama {
            get {
                return this.dogrulamaField;
            }
            set {
                this.dogrulamaField = value;
            }
        }
        
        /// <remarks/>
        public string isletmeDurumu {
            get {
                return this.isletmeDurumuField;
            }
            set {
                this.isletmeDurumuField = value;
            }
        }
        
        /// <remarks/>
        public string isletmeNo {
            get {
                return this.isletmeNoField;
            }
            set {
                this.isletmeNoField = value;
            }
        }
        
        /// <remarks/>
        public string il {
            get {
                return this.ilField;
            }
            set {
                this.ilField = value;
            }
        }
        
        /// <remarks/>
        public string ilKodu {
            get {
                return this.ilKoduField;
            }
            set {
                this.ilKoduField = value;
            }
        }
        
        /// <remarks/>
        public string ilce {
            get {
                return this.ilceField;
            }
            set {
                this.ilceField = value;
            }
        }
        
        /// <remarks/>
        public string ilceKodu {
            get {
                return this.ilceKoduField;
            }
            set {
                this.ilceKoduField = value;
            }
        }
        
        /// <remarks/>
        public string bucak {
            get {
                return this.bucakField;
            }
            set {
                this.bucakField = value;
            }
        }
        
        /// <remarks/>
        public string bucakKodu {
            get {
                return this.bucakKoduField;
            }
            set {
                this.bucakKoduField = value;
            }
        }
        
        /// <remarks/>
        public string koy {
            get {
                return this.koyField;
            }
            set {
                this.koyField = value;
            }
        }
        
        /// <remarks/>
        public string koyKodu {
            get {
                return this.koyKoduField;
            }
            set {
                this.koyKoduField = value;
            }
        }
        
        /// <remarks/>
        public string kimlikNo {
            get {
                return this.kimlikNoField;
            }
            set {
                this.kimlikNoField = value;
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
        public string adi {
            get {
                return this.adiField;
            }
            set {
                this.adiField = value;
            }
        }
        
        /// <remarks/>
        public string soyadi {
            get {
                return this.soyadiField;
            }
            set {
                this.soyadiField = value;
            }
        }
        
        /// <remarks/>
        public string babaAdi {
            get {
                return this.babaAdiField;
            }
            set {
                this.babaAdiField = value;
            }
        }
        
        /// <remarks/>
        public string dogumTarihi {
            get {
                return this.dogumTarihiField;
            }
            set {
                this.dogumTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string yetistiricilikBelgeNo {
            get {
                return this.yetistiricilikBelgeNoField;
            }
            set {
                this.yetistiricilikBelgeNoField = value;
            }
        }
        
        /// <remarks/>
        public string TesisAdi {
            get {
                return this.tesisAdiField;
            }
            set {
                this.tesisAdiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public SuTesisi[] SuTesisleri {
            get {
                return this.suTesisleriField;
            }
            set {
                this.suTesisleriField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class SeraUrun {
        
        private string turIDField;
        
        private string turAdiField;
        
        private string uretimMetoduIDField;
        
        private string uretimMetoduAdiField;
        
        private string uretimMateryaliIDField;
        
        private string uretimMateryaliAdiField;
        
        private string uretimDonemiIDField;
        
        private string uretimDonemiAdiField;
        
        private string ekimDikimTarihiField;
        
        private string hasatBaslangicTarihiField;
        
        private string hasatBitisTarihiField;
        
        private int uretimMiktariField;
        
        private string katalogMiktarIDField;
        
        private string katalogMiktarAdiField;
        
        private int alanField;
        
        /// <remarks/>
        public string TurID {
            get {
                return this.turIDField;
            }
            set {
                this.turIDField = value;
            }
        }
        
        /// <remarks/>
        public string TurAdi {
            get {
                return this.turAdiField;
            }
            set {
                this.turAdiField = value;
            }
        }
        
        /// <remarks/>
        public string UretimMetoduID {
            get {
                return this.uretimMetoduIDField;
            }
            set {
                this.uretimMetoduIDField = value;
            }
        }
        
        /// <remarks/>
        public string UretimMetoduAdi {
            get {
                return this.uretimMetoduAdiField;
            }
            set {
                this.uretimMetoduAdiField = value;
            }
        }
        
        /// <remarks/>
        public string UretimMateryaliID {
            get {
                return this.uretimMateryaliIDField;
            }
            set {
                this.uretimMateryaliIDField = value;
            }
        }
        
        /// <remarks/>
        public string UretimMateryaliAdi {
            get {
                return this.uretimMateryaliAdiField;
            }
            set {
                this.uretimMateryaliAdiField = value;
            }
        }
        
        /// <remarks/>
        public string UretimDonemiID {
            get {
                return this.uretimDonemiIDField;
            }
            set {
                this.uretimDonemiIDField = value;
            }
        }
        
        /// <remarks/>
        public string UretimDonemiAdi {
            get {
                return this.uretimDonemiAdiField;
            }
            set {
                this.uretimDonemiAdiField = value;
            }
        }
        
        /// <remarks/>
        public string EkimDikimTarihi {
            get {
                return this.ekimDikimTarihiField;
            }
            set {
                this.ekimDikimTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string HasatBaslangicTarihi {
            get {
                return this.hasatBaslangicTarihiField;
            }
            set {
                this.hasatBaslangicTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string HasatBitisTarihi {
            get {
                return this.hasatBitisTarihiField;
            }
            set {
                this.hasatBitisTarihiField = value;
            }
        }
        
        /// <remarks/>
        public int UretimMiktari {
            get {
                return this.uretimMiktariField;
            }
            set {
                this.uretimMiktariField = value;
            }
        }
        
        /// <remarks/>
        public string KatalogMiktarID {
            get {
                return this.katalogMiktarIDField;
            }
            set {
                this.katalogMiktarIDField = value;
            }
        }
        
        /// <remarks/>
        public string KatalogMiktarAdi {
            get {
                return this.katalogMiktarAdiField;
            }
            set {
                this.katalogMiktarAdiField = value;
            }
        }
        
        /// <remarks/>
        public int Alan {
            get {
                return this.alanField;
            }
            set {
                this.alanField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class SeraArazi {
        
        private string belgeTipiIDField;
        
        private string belgeTipiAdiField;
        
        private string tasarrufSekliIDField;
        
        private string tasarrufSekliAdiField;
        
        private string ilIDField;
        
        private string ilAdiField;
        
        private string ilceIDField;
        
        private string ilceAdiField;
        
        private string bucakIDField;
        
        private string bucakAdiField;
        
        private string koyIDField;
        
        private string koyAdiField;
        
        private string adaNoField;
        
        private string parselNoField;
        
        private int parselAlaniField;
        
        private int ortualtiAlaniField;
        
        /// <remarks/>
        public string BelgeTipiID {
            get {
                return this.belgeTipiIDField;
            }
            set {
                this.belgeTipiIDField = value;
            }
        }
        
        /// <remarks/>
        public string BelgeTipiAdi {
            get {
                return this.belgeTipiAdiField;
            }
            set {
                this.belgeTipiAdiField = value;
            }
        }
        
        /// <remarks/>
        public string TasarrufSekliID {
            get {
                return this.tasarrufSekliIDField;
            }
            set {
                this.tasarrufSekliIDField = value;
            }
        }
        
        /// <remarks/>
        public string TasarrufSekliAdi {
            get {
                return this.tasarrufSekliAdiField;
            }
            set {
                this.tasarrufSekliAdiField = value;
            }
        }
        
        /// <remarks/>
        public string IlID {
            get {
                return this.ilIDField;
            }
            set {
                this.ilIDField = value;
            }
        }
        
        /// <remarks/>
        public string IlAdi {
            get {
                return this.ilAdiField;
            }
            set {
                this.ilAdiField = value;
            }
        }
        
        /// <remarks/>
        public string IlceID {
            get {
                return this.ilceIDField;
            }
            set {
                this.ilceIDField = value;
            }
        }
        
        /// <remarks/>
        public string IlceAdi {
            get {
                return this.ilceAdiField;
            }
            set {
                this.ilceAdiField = value;
            }
        }
        
        /// <remarks/>
        public string BucakID {
            get {
                return this.bucakIDField;
            }
            set {
                this.bucakIDField = value;
            }
        }
        
        /// <remarks/>
        public string BucakAdi {
            get {
                return this.bucakAdiField;
            }
            set {
                this.bucakAdiField = value;
            }
        }
        
        /// <remarks/>
        public string KoyID {
            get {
                return this.koyIDField;
            }
            set {
                this.koyIDField = value;
            }
        }
        
        /// <remarks/>
        public string KoyAdi {
            get {
                return this.koyAdiField;
            }
            set {
                this.koyAdiField = value;
            }
        }
        
        /// <remarks/>
        public string AdaNo {
            get {
                return this.adaNoField;
            }
            set {
                this.adaNoField = value;
            }
        }
        
        /// <remarks/>
        public string ParselNo {
            get {
                return this.parselNoField;
            }
            set {
                this.parselNoField = value;
            }
        }
        
        /// <remarks/>
        public int ParselAlani {
            get {
                return this.parselAlaniField;
            }
            set {
                this.parselAlaniField = value;
            }
        }
        
        /// <remarks/>
        public int OrtualtiAlani {
            get {
                return this.ortualtiAlaniField;
            }
            set {
                this.ortualtiAlaniField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class Ortualti {
        
        private string ortualtiNoField;
        
        private string ortualtiTipiIDField;
        
        private string ortualtiTipiAdiField;
        
        private string konstruksionIDField;
        
        private string konstruksionAdiField;
        
        private string malzemeIDField;
        
        private string malzemeAdiField;
        
        private string isitmaTipiIDField;
        
        private string isitmaTipiAdiField;
        
        private string yetistiricilikYontemiIDField;
        
        private string yetistiricilikYontemiAdiField;
        
        private string katiYontemIDField;
        
        private string katiYontemAdiField;
        
        private string suYontemIDField;
        
        private string suYontemAdiField;
        
        private int alanField;
        
        private SeraArazi[] araziField;
        
        private SeraUrun[] urunField;
        
        /// <remarks/>
        public string OrtualtiNo {
            get {
                return this.ortualtiNoField;
            }
            set {
                this.ortualtiNoField = value;
            }
        }
        
        /// <remarks/>
        public string OrtualtiTipiID {
            get {
                return this.ortualtiTipiIDField;
            }
            set {
                this.ortualtiTipiIDField = value;
            }
        }
        
        /// <remarks/>
        public string OrtualtiTipiAdi {
            get {
                return this.ortualtiTipiAdiField;
            }
            set {
                this.ortualtiTipiAdiField = value;
            }
        }
        
        /// <remarks/>
        public string KonstruksionID {
            get {
                return this.konstruksionIDField;
            }
            set {
                this.konstruksionIDField = value;
            }
        }
        
        /// <remarks/>
        public string KonstruksionAdi {
            get {
                return this.konstruksionAdiField;
            }
            set {
                this.konstruksionAdiField = value;
            }
        }
        
        /// <remarks/>
        public string MalzemeID {
            get {
                return this.malzemeIDField;
            }
            set {
                this.malzemeIDField = value;
            }
        }
        
        /// <remarks/>
        public string MalzemeAdi {
            get {
                return this.malzemeAdiField;
            }
            set {
                this.malzemeAdiField = value;
            }
        }
        
        /// <remarks/>
        public string IsitmaTipiID {
            get {
                return this.isitmaTipiIDField;
            }
            set {
                this.isitmaTipiIDField = value;
            }
        }
        
        /// <remarks/>
        public string IsitmaTipiAdi {
            get {
                return this.isitmaTipiAdiField;
            }
            set {
                this.isitmaTipiAdiField = value;
            }
        }
        
        /// <remarks/>
        public string YetistiricilikYontemiID {
            get {
                return this.yetistiricilikYontemiIDField;
            }
            set {
                this.yetistiricilikYontemiIDField = value;
            }
        }
        
        /// <remarks/>
        public string YetistiricilikYontemiAdi {
            get {
                return this.yetistiricilikYontemiAdiField;
            }
            set {
                this.yetistiricilikYontemiAdiField = value;
            }
        }
        
        /// <remarks/>
        public string KatiYontemID {
            get {
                return this.katiYontemIDField;
            }
            set {
                this.katiYontemIDField = value;
            }
        }
        
        /// <remarks/>
        public string KatiYontemAdi {
            get {
                return this.katiYontemAdiField;
            }
            set {
                this.katiYontemAdiField = value;
            }
        }
        
        /// <remarks/>
        public string SuYontemID {
            get {
                return this.suYontemIDField;
            }
            set {
                this.suYontemIDField = value;
            }
        }
        
        /// <remarks/>
        public string SuYontemAdi {
            get {
                return this.suYontemAdiField;
            }
            set {
                this.suYontemAdiField = value;
            }
        }
        
        /// <remarks/>
        public int Alan {
            get {
                return this.alanField;
            }
            set {
                this.alanField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public SeraArazi[] arazi {
            get {
                return this.araziField;
            }
            set {
                this.araziField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public SeraUrun[] urun {
            get {
                return this.urunField;
            }
            set {
                this.urunField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class ortualtiUretici {
        
        private string isletmeTipiIDField;
        
        private string dogrulamaField;
        
        private string ilField;
        
        private string ilKoduField;
        
        private string ilceField;
        
        private string ilceKoduField;
        
        private string koyField;
        
        private string koyKoduField;
        
        private string vergiNoField;
        
        private string kimlikNoField;
        
        private string adiField;
        
        private string soyadiField;
        
        private Ortualti[] ortualtiField;
        
        /// <remarks/>
        public string IsletmeTipiID {
            get {
                return this.isletmeTipiIDField;
            }
            set {
                this.isletmeTipiIDField = value;
            }
        }
        
        /// <remarks/>
        public string dogrulama {
            get {
                return this.dogrulamaField;
            }
            set {
                this.dogrulamaField = value;
            }
        }
        
        /// <remarks/>
        public string il {
            get {
                return this.ilField;
            }
            set {
                this.ilField = value;
            }
        }
        
        /// <remarks/>
        public string ilKodu {
            get {
                return this.ilKoduField;
            }
            set {
                this.ilKoduField = value;
            }
        }
        
        /// <remarks/>
        public string ilce {
            get {
                return this.ilceField;
            }
            set {
                this.ilceField = value;
            }
        }
        
        /// <remarks/>
        public string ilceKodu {
            get {
                return this.ilceKoduField;
            }
            set {
                this.ilceKoduField = value;
            }
        }
        
        /// <remarks/>
        public string koy {
            get {
                return this.koyField;
            }
            set {
                this.koyField = value;
            }
        }
        
        /// <remarks/>
        public string koyKodu {
            get {
                return this.koyKoduField;
            }
            set {
                this.koyKoduField = value;
            }
        }
        
        /// <remarks/>
        public string VergiNo {
            get {
                return this.vergiNoField;
            }
            set {
                this.vergiNoField = value;
            }
        }
        
        /// <remarks/>
        public string kimlikNo {
            get {
                return this.kimlikNoField;
            }
            set {
                this.kimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public string adi {
            get {
                return this.adiField;
            }
            set {
                this.adiField = value;
            }
        }
        
        /// <remarks/>
        public string soyadi {
            get {
                return this.soyadiField;
            }
            set {
                this.soyadiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Ortualti[] ortualti {
            get {
                return this.ortualtiField;
            }
            set {
                this.ortualtiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class Hayvan {
        
        private string kulakNoField;
        
        private string adiField;
        
        private string dogumTarihiField;
        
        private string cinsiyetiField;
        
        private string tipiField;
        
        private string gunField;
        
        /// <remarks/>
        public string kulakNo {
            get {
                return this.kulakNoField;
            }
            set {
                this.kulakNoField = value;
            }
        }
        
        /// <remarks/>
        public string adi {
            get {
                return this.adiField;
            }
            set {
                this.adiField = value;
            }
        }
        
        /// <remarks/>
        public string dogumTarihi {
            get {
                return this.dogumTarihiField;
            }
            set {
                this.dogumTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string cinsiyeti {
            get {
                return this.cinsiyetiField;
            }
            set {
                this.cinsiyetiField = value;
            }
        }
        
        /// <remarks/>
        public string tipi {
            get {
                return this.tipiField;
            }
            set {
                this.tipiField = value;
            }
        }
        
        /// <remarks/>
        public string gun {
            get {
                return this.gunField;
            }
            set {
                this.gunField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webinfo.tarim.gov.tr")]
    public partial class IsletmeSoykutuk {
        
        private string dogrulamaField;
        
        private string adiSoyadiField;
        
        private string kimlikNoField;
        
        private string vergiNoField;
        
        private string isletmeNoField;
        
        private string ilAdiField;
        
        private string ilceAdiField;
        
        private string koyAdiField;
        
        private string isletmeTuruField;
        
        private string projeKoduField;
        
        private string kayitTarihiField;
        
        private Hayvan[] hayvanField;
        
        /// <remarks/>
        public string dogrulama {
            get {
                return this.dogrulamaField;
            }
            set {
                this.dogrulamaField = value;
            }
        }
        
        /// <remarks/>
        public string adiSoyadi {
            get {
                return this.adiSoyadiField;
            }
            set {
                this.adiSoyadiField = value;
            }
        }
        
        /// <remarks/>
        public string kimlikNo {
            get {
                return this.kimlikNoField;
            }
            set {
                this.kimlikNoField = value;
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
        public string isletmeNo {
            get {
                return this.isletmeNoField;
            }
            set {
                this.isletmeNoField = value;
            }
        }
        
        /// <remarks/>
        public string ilAdi {
            get {
                return this.ilAdiField;
            }
            set {
                this.ilAdiField = value;
            }
        }
        
        /// <remarks/>
        public string ilceAdi {
            get {
                return this.ilceAdiField;
            }
            set {
                this.ilceAdiField = value;
            }
        }
        
        /// <remarks/>
        public string koyAdi {
            get {
                return this.koyAdiField;
            }
            set {
                this.koyAdiField = value;
            }
        }
        
        /// <remarks/>
        public string isletmeTuru {
            get {
                return this.isletmeTuruField;
            }
            set {
                this.isletmeTuruField = value;
            }
        }
        
        /// <remarks/>
        public string projeKodu {
            get {
                return this.projeKoduField;
            }
            set {
                this.projeKoduField = value;
            }
        }
        
        /// <remarks/>
        public string kayitTarihi {
            get {
                return this.kayitTarihiField;
            }
            set {
                this.kayitTarihiField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Hayvan[] hayvan {
            get {
                return this.hayvanField;
            }
            set {
                this.hayvanField = value;
            }
        }
    }
}

#pragma warning restore 1591
