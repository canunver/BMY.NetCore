//------------------------------------------------------------------------------
// KPS web servisi istemcisi.
//
// Bu dosya, Visual Studio'nun .NET Framework için ürettiği eski Reference.cs dosyasından
// _araclar/asmx_istemci_uret.py ile dönüştürülmüştür. Proxy sınıfı TNS.Soap.SoapIstemci tabanlıdır
// (HttpClient + XmlSerializer); veri sınıfları eski üretilmiş koddan değiştirilmeden alınmıştır.
//------------------------------------------------------------------------------
#pragma warning disable 1591
namespace Mernis.KPS.Types.KPSWse {
    using System;
    using System.Xml.Serialization;
    using TNS.Soap;

    public partial class KPS : SoapIstemci {

        private static readonly Type[] ekTipler = new Type[] { typeof(KPSVeriTablosu), typeof(CustomBE) };

        protected override Type[] EkTipler {
            get { return ekTipler; }
        }

        public KPS() {
            this.Url = global::Mernis.KPS.Types.Properties.Settings.Default.Mernis_KPS_Types_KPSWse_KPS;
        }

        public KisiBilgisiC[] TCKimlikNodanKisiBilgisiSorgula(TCKimlikNoG[] list) {
            return (KisiBilgisiC[])Cagir("http://kps.nvi.gov.tr/WS/TCKimlikNodanKisiBilgisiSorgula", "TCKimlikNodanKisiBilgisiSorgula", "http://kps.nvi.gov.tr/WS", typeof(KisiBilgisiC[]), "TCKimlikNodanKisiBilgisiSorgulaResult",
                new SoapParametre("list", list, typeof(TCKimlikNoG[])));
        }

        public KisiBilgisiCT KisiListesiSorgula(int IlceKodu, System.DateTime BaslangicTarihi, System.DateTime BitisTarihi, CinsiyetTipi Cinsiyet) {
            return (KisiBilgisiCT)Cagir("http://kps.nvi.gov.tr/WS/KisiListesiSorgula", "KisiListesiSorgula", "http://kps.nvi.gov.tr/WS", typeof(KisiBilgisiCT), "KisiListesiSorgulaResult",
                new SoapParametre("IlceKodu", IlceKodu, typeof(int)),
                new SoapParametre("BaslangicTarihi", BaslangicTarihi, typeof(System.DateTime)),
                new SoapParametre("BitisTarihi", BitisTarihi, typeof(System.DateTime)),
                new SoapParametre("Cinsiyet", Cinsiyet, typeof(CinsiyetTipi)));
        }

        public KisiBilgisiCT KisiListesiSayfala(SayfaIstekKomutu komut) {
            return (KisiBilgisiCT)Cagir("http://kps.nvi.gov.tr/WS/KisiListesiSayfala", "KisiListesiSayfala", "http://kps.nvi.gov.tr/WS", typeof(KisiBilgisiCT), "KisiListesiSayfalaResult",
                new SoapParametre("komut", komut, typeof(SayfaIstekKomutu)));
        }

        public OlayBilgisiCT OlayBilgisiSorgula(int ilceKodu, System.DateTime baslangicTarihi, System.DateTime bitisTarihi, NufusOlayTipi tip) {
            return (OlayBilgisiCT)Cagir("http://kps.nvi.gov.tr/WS/OlayBilgisiSorgula", "OlayBilgisiSorgula", "http://kps.nvi.gov.tr/WS", typeof(OlayBilgisiCT), "OlayBilgisiSorgulaResult",
                new SoapParametre("ilceKodu", ilceKodu, typeof(int)),
                new SoapParametre("baslangicTarihi", baslangicTarihi, typeof(System.DateTime)),
                new SoapParametre("bitisTarihi", bitisTarihi, typeof(System.DateTime)),
                new SoapParametre("tip", tip, typeof(NufusOlayTipi)));
        }

        public OlayBilgisiCT OlayListesiSayfala(SayfaIstekKomutu komut) {
            return (OlayBilgisiCT)Cagir("http://kps.nvi.gov.tr/WS/OlayListesiSayfala", "OlayListesiSayfala", "http://kps.nvi.gov.tr/WS", typeof(OlayBilgisiCT), "OlayListesiSayfalaResult",
                new SoapParametre("komut", komut, typeof(SayfaIstekKomutu)));
        }

        public CiltKoduC[] CiltAdindanCiltKoduSorgula(int ilceKodu, string ciltAdi) {
            return (CiltKoduC[])Cagir("http://kps.nvi.gov.tr/WS/CiltAdindanCiltKoduSorgula", "CiltAdindanCiltKoduSorgula", "http://kps.nvi.gov.tr/WS", typeof(CiltKoduC[]), "CiltAdindanCiltKoduSorgulaResult",
                new SoapParametre("ilceKodu", ilceKodu, typeof(int)),
                new SoapParametre("ciltAdi", ciltAdi, typeof(string)));
        }

        public CiltAdiC[] CiltKodundanCiltAdiSorgula(int ilceKodu, int ciltKodu) {
            return (CiltAdiC[])Cagir("http://kps.nvi.gov.tr/WS/CiltKodundanCiltAdiSorgula", "CiltKodundanCiltAdiSorgula", "http://kps.nvi.gov.tr/WS", typeof(CiltAdiC[]), "CiltKodundanCiltAdiSorgulaResult",
                new SoapParametre("ilceKodu", ilceKodu, typeof(int)),
                new SoapParametre("ciltKodu", ciltKodu, typeof(int)));
        }

        public IlBilgisi[] IlListesiGetir() {
            return (IlBilgisi[])Cagir("http://kps.nvi.gov.tr/WS/IlListesiGetir", "IlListesiGetir", "http://kps.nvi.gov.tr/WS", typeof(IlBilgisi[]), "IlListesiGetirResult");
        }

        public IlceBilgisi[] IlceListesiGetir() {
            return (IlceBilgisi[])Cagir("http://kps.nvi.gov.tr/WS/IlceListesiGetir", "IlceListesiGetir", "http://kps.nvi.gov.tr/WS", typeof(IlceBilgisi[]), "IlceListesiGetirResult");
        }

        public IlceBilgisi[] IleBagliIlceListesiGetir(int ilKodu) {
            return (IlceBilgisi[])Cagir("http://kps.nvi.gov.tr/WS/IleBagliIlceListesiGetir", "IleBagliIlceListesiGetir", "http://kps.nvi.gov.tr/WS", typeof(IlceBilgisi[]), "IleBagliIlceListesiGetirResult",
                new SoapParametre("ilKodu", ilKodu, typeof(int)));
        }

        public NufusKayitOrnegiC[] KayitYerindenNufusKayitOrnegiSorgula(NKOKayitYeriG[] list) {
            return (NufusKayitOrnegiC[])Cagir("http://kps.nvi.gov.tr/WS/KayitYerindenNufusKayitOrnegiSorgula", "KayitYerindenNufusKayitOrnegiSorgula", "http://kps.nvi.gov.tr/WS", typeof(NufusKayitOrnegiC[]), "KayitYerindenNufusKayitOrnegiSorgulaResult",
                new SoapParametre("list", list, typeof(NKOKayitYeriG[])));
        }

        public KisiBilgisiC[] KayitYerindenKisiBilgisiSorgula(KayitYerindenKimlikBilgisiG[] list) {
            return (KisiBilgisiC[])Cagir("http://kps.nvi.gov.tr/WS/KayitYerindenKisiBilgisiSorgula", "KayitYerindenKisiBilgisiSorgula", "http://kps.nvi.gov.tr/WS", typeof(KisiBilgisiC[]), "KayitYerindenKisiBilgisiSorgulaResult",
                new SoapParametre("list", list, typeof(KayitYerindenKimlikBilgisiG[])));
        }

        public NufusCuzdanC[] NufusCuzdaniDogrulama(TCKimlikNoG[] list) {
            return (NufusCuzdanC[])Cagir("http://kps.nvi.gov.tr/WS/NufusCuzdaniDogrulama", "NufusCuzdaniDogrulama", "http://kps.nvi.gov.tr/WS", typeof(NufusCuzdanC[]), "NufusCuzdaniDogrulamaResult",
                new SoapParametre("list", list, typeof(TCKimlikNoG[])));
        }

        public NufusCuzdanSatisYerC[] NufusCuzdaniSatisYeriDogrulama(NufusCuzdanSatisYerG[] list) {
            return (NufusCuzdanSatisYerC[])Cagir("http://kps.nvi.gov.tr/WS/NufusCuzdaniSatisYeriDogrulama", "NufusCuzdaniSatisYeriDogrulama", "http://kps.nvi.gov.tr/WS", typeof(NufusCuzdanSatisYerC[]), "NufusCuzdaniSatisYeriDogrulamaResult",
                new SoapParametre("list", list, typeof(NufusCuzdanSatisYerG[])));
        }

        public KimlikBilgisiDegisenlerCT KimlikBilgisiDegisenlerSorgula(System.DateTime BaslangicTarihi, System.DateTime BitisTarihi) {
            return (KimlikBilgisiDegisenlerCT)Cagir("http://kps.nvi.gov.tr/WS/KimlikBilgisiDegisenlerSorgula", "KimlikBilgisiDegisenlerSorgula", "http://kps.nvi.gov.tr/WS", typeof(KimlikBilgisiDegisenlerCT), "KimlikBilgisiDegisenlerSorgulaResult",
                new SoapParametre("BaslangicTarihi", BaslangicTarihi, typeof(System.DateTime)),
                new SoapParametre("BitisTarihi", BitisTarihi, typeof(System.DateTime)));
        }

        public KimlikBilgisiDegisenlerCT KimlikBilgisiDegisenlerSayfala(SayfaIstekKomutu komut) {
            return (KimlikBilgisiDegisenlerCT)Cagir("http://kps.nvi.gov.tr/WS/KimlikBilgisiDegisenlerSayfala", "KimlikBilgisiDegisenlerSayfala", "http://kps.nvi.gov.tr/WS", typeof(KimlikBilgisiDegisenlerCT), "KimlikBilgisiDegisenlerSayfalaResult",
                new SoapParametre("komut", komut, typeof(SayfaIstekKomutu)));
        }

        public DogumIstatistigiC[] DogumIstatistigiSorgula(int ilkod, int ilcekod, int yil) {
            return (DogumIstatistigiC[])Cagir("http://kps.nvi.gov.tr/WS/DogumIstatistigiSorgula", "DogumIstatistigiSorgula", "http://kps.nvi.gov.tr/WS", typeof(DogumIstatistigiC[]), "DogumIstatistigiSorgulaResult",
                new SoapParametre("ilkod", ilkod, typeof(int)),
                new SoapParametre("ilcekod", ilcekod, typeof(int)),
                new SoapParametre("yil", yil, typeof(int)));
        }

        public OlumIstatistigiC[] OlumIstatistigiSorgula(int ilkod, int ilcekod, int yil) {
            return (OlumIstatistigiC[])Cagir("http://kps.nvi.gov.tr/WS/OlumIstatistigiSorgula", "OlumIstatistigiSorgula", "http://kps.nvi.gov.tr/WS", typeof(OlumIstatistigiC[]), "OlumIstatistigiSorgulaResult",
                new SoapParametre("ilkod", ilkod, typeof(int)),
                new SoapParametre("ilcekod", ilcekod, typeof(int)),
                new SoapParametre("yil", yil, typeof(int)));
        }

        public EvlenmeIstatistigiC[] EvlenmeIstatistigiSorgula(int ilkod, int ilcekod, int yil) {
            return (EvlenmeIstatistigiC[])Cagir("http://kps.nvi.gov.tr/WS/EvlenmeIstatistigiSorgula", "EvlenmeIstatistigiSorgula", "http://kps.nvi.gov.tr/WS", typeof(EvlenmeIstatistigiC[]), "EvlenmeIstatistigiSorgulaResult",
                new SoapParametre("ilkod", ilkod, typeof(int)),
                new SoapParametre("ilcekod", ilcekod, typeof(int)),
                new SoapParametre("yil", yil, typeof(int)));
        }

        public BosanmaIstatistigiC[] BosanmaIstatistigiSorgula(int ilkod, int ilcekod, int yil) {
            return (BosanmaIstatistigiC[])Cagir("http://kps.nvi.gov.tr/WS/BosanmaIstatistigiSorgula", "BosanmaIstatistigiSorgula", "http://kps.nvi.gov.tr/WS", typeof(BosanmaIstatistigiC[]), "BosanmaIstatistigiSorgulaResult",
                new SoapParametre("ilkod", ilkod, typeof(int)),
                new SoapParametre("ilcekod", ilcekod, typeof(int)),
                new SoapParametre("yil", yil, typeof(int)));
        }

        public KisiBilgisiC[] KisiBilgisindenKimlikBilgisiSorgula(KimlikBilgisiG[] list) {
            return (KisiBilgisiC[])Cagir("http://kps.nvi.gov.tr/WS/KisiBilgisindenKimlikBilgisiSorgula", "KisiBilgisindenKimlikBilgisiSorgula", "http://kps.nvi.gov.tr/WS", typeof(KisiBilgisiC[]), "KisiBilgisindenKimlikBilgisiSorgulaResult",
                new SoapParametre("list", list, typeof(KimlikBilgisiG[])));
        }

        public NufusKayitOrnegiC[] TcNodanNKOSorgula(NKOTCKimlikNoG[] list) {
            return (NufusKayitOrnegiC[])Cagir("http://kps.nvi.gov.tr/WS/TcNodanNKOSorgula", "TcNodanNKOSorgula", "http://kps.nvi.gov.tr/WS", typeof(NufusKayitOrnegiC[]), "TcNodanNKOSorgulaResult",
                new SoapParametre("list", list, typeof(NKOTCKimlikNoG[])));
        }

        public EvlenmeC[] KayitYerindenEvlenmeBilgisiSorgula(EvlenmeG[] list) {
            return (EvlenmeC[])Cagir("http://kps.nvi.gov.tr/WS/KayitYerindenEvlenmeBilgisiSorgula", "KayitYerindenEvlenmeBilgisiSorgula", "http://kps.nvi.gov.tr/WS", typeof(EvlenmeC[]), "KayitYerindenEvlenmeBilgisiSorgulaResult",
                new SoapParametre("list", list, typeof(EvlenmeG[])));
        }

        public KayipCuzdanCT KayipCuzdanListesiSorgula(System.DateTime BaslangicTarihi, System.DateTime BitisTarihi) {
            return (KayipCuzdanCT)Cagir("http://kps.nvi.gov.tr/WS/KayipCuzdanListesiSorgula", "KayipCuzdanListesiSorgula", "http://kps.nvi.gov.tr/WS", typeof(KayipCuzdanCT), "KayipCuzdanListesiSorgulaResult",
                new SoapParametre("BaslangicTarihi", BaslangicTarihi, typeof(System.DateTime)),
                new SoapParametre("BitisTarihi", BitisTarihi, typeof(System.DateTime)));
        }

        public KayipCuzdanCT KayipCuzdanListesiSayfala(SayfaIstekKomutu komut) {
            return (KayipCuzdanCT)Cagir("http://kps.nvi.gov.tr/WS/KayipCuzdanListesiSayfala", "KayipCuzdanListesiSayfala", "http://kps.nvi.gov.tr/WS", typeof(KayipCuzdanCT), "KayipCuzdanListesiSayfalaResult",
                new SoapParametre("komut", komut, typeof(SayfaIstekKomutu)));
        }

        public KayipCuzdanC[] TCKimlikNodanKayipCuzdanBilgisiSorgula(long TCKimlikNo) {
            return (KayipCuzdanC[])Cagir("http://kps.nvi.gov.tr/WS/TCKimlikNodanKayipCuzdanBilgisiSorgula", "TCKimlikNodanKayipCuzdanBilgisiSorgula", "http://kps.nvi.gov.tr/WS", typeof(KayipCuzdanC[]), "TCKimlikNodanKayipCuzdanBilgisiSorgulaResult",
                new SoapParametre("TCKimlikNo", TCKimlikNo, typeof(long)));
        }

        public KayipCuzdanC[] NufusCuzdanSeriNodanKayipCuzdanBilgisiSorgula(string CuzdanSeri, int CuzdanNo) {
            return (KayipCuzdanC[])Cagir("http://kps.nvi.gov.tr/WS/NufusCuzdanSeriNodanKayipCuzdanBilgisiSorgula", "NufusCuzdanSeriNodanKayipCuzdanBilgisiSorgula", "http://kps.nvi.gov.tr/WS", typeof(KayipCuzdanC[]), "NufusCuzdanSeriNodanKayipCuzdanBilgisiSorgulaResult",
                new SoapParametre("CuzdanSeri", CuzdanSeri, typeof(string)),
                new SoapParametre("CuzdanNo", CuzdanNo, typeof(int)));
        }

        public KayipCuzdanEskiC[] NufusCuzdanSeriNodanEskiKayipCuzdanBilgisiSorgula(string CuzdanSeri, int CuzdanNo) {
            return (KayipCuzdanEskiC[])Cagir("http://kps.nvi.gov.tr/WS/NufusCuzdanSeriNodanEskiKayipCuzdanBilgisiSorgula", "NufusCuzdanSeriNodanEskiKayipCuzdanBilgisiSorgula", "http://kps.nvi.gov.tr/WS", typeof(KayipCuzdanEskiC[]), "NufusCuzdanSeriNodanEskiKayipCuzdanBilgisiSorgulaResult",
                new SoapParametre("CuzdanSeri", CuzdanSeri, typeof(string)),
                new SoapParametre("CuzdanNo", CuzdanNo, typeof(int)));
        }

        public AileListesiC[] TCKimlikNodanAileListesiSorgula(AileListesiTCKimlikNoG[] list) {
            return (AileListesiC[])Cagir("http://kps.nvi.gov.tr/WS/TCKimlikNodanAileListesiSorgula", "TCKimlikNodanAileListesiSorgula", "http://kps.nvi.gov.tr/WS", typeof(AileListesiC[]), "TCKimlikNodanAileListesiSorgulaResult",
                new SoapParametre("list", list, typeof(AileListesiTCKimlikNoG[])));
        }

        public YabanciKisiBilgisiC[] YabanciKimlikNodanYabanciKisiBilgisiSorgula(TCKimlikNoG[] list) {
            return (YabanciKisiBilgisiC[])Cagir("http://kps.nvi.gov.tr/WS/YabanciKimlikNodanYabanciKisiBilgisiSorgula", "YabanciKimlikNodanYabanciKisiBilgisiSorgula", "http://kps.nvi.gov.tr/WS", typeof(YabanciKisiBilgisiC[]), "YabanciKimlikNodanYabanciKisiBilgisiSorgulaResult",
                new SoapParametre("list", list, typeof(TCKimlikNoG[])));
        }

        public YabanciKisiBilgisiC[] YabanciKisiBilgisindenKimlikBilgisiSorgula(KimlikBilgisiG[] list) {
            return (YabanciKisiBilgisiC[])Cagir("http://kps.nvi.gov.tr/WS/YabanciKisiBilgisindenKimlikBilgisiSorgula", "YabanciKisiBilgisindenKimlikBilgisiSorgula", "http://kps.nvi.gov.tr/WS", typeof(YabanciKisiBilgisiC[]), "YabanciKisiBilgisindenKimlikBilgisiSorgulaResult",
                new SoapParametre("list", list, typeof(KimlikBilgisiG[])));
        }

        public KisiAdresBilgisiC[] TCKimlikNodanYerlesimYeriAdresBilgisiSorgula(TCKimlikNoG[] list) {
            return (KisiAdresBilgisiC[])Cagir("http://kps.nvi.gov.tr/WS/TCKimlikNodanYerlesimYeriAdresBilgisiSorgula", "TCKimlikNodanYerlesimYeriAdresBilgisiSorgula", "http://kps.nvi.gov.tr/WS", typeof(KisiAdresBilgisiC[]), "TCKimlikNodanYerlesimYeriAdresBilgisiSorgulaResult",
                new SoapParametre("list", list, typeof(TCKimlikNoG[])));
        }

        public YerlesimYerindeOturanlarC[] AdresNodanYerlesimYerindeOturanlariSorgula(AdresNoG[] list) {
            return (YerlesimYerindeOturanlarC[])Cagir("http://kps.nvi.gov.tr/WS/AdresNodanYerlesimYerindeOturanlariSorgula", "AdresNodanYerlesimYerindeOturanlariSorgula", "http://kps.nvi.gov.tr/WS", typeof(YerlesimYerindeOturanlarC[]), "AdresNodanYerlesimYerindeOturanlariSorgulaResult",
                new SoapParametre("list", list, typeof(AdresNoG[])));
        }

    }

    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class TCKimlikNoG : KPSSorgusu {
        
        private long tCKimlikNoField;
        
        /// <remarks/>
        public long TCKimlikNo {
            get {
                return this.tCKimlikNoField;
            }
            set {
                this.tCKimlikNoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AdresNoG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AileListesiTCKimlikNoG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(EvlenmeG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NKOTCKimlikNoG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KimlikBilgisiG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KayitYerindenKimlikBilgisiG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NKOKayitYeriG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(TCKimlikNoG))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public abstract partial class KPSSorgusu : SearchInfoBE {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NufusCuzdanSatisYerG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KPSSorgusu))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AdresNoG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AileListesiTCKimlikNoG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(EvlenmeG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NKOTCKimlikNoG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KimlikBilgisiG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KayitYerindenKimlikBilgisiG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NKOKayitYeriG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(TCKimlikNoG))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public abstract partial class SearchInfoBE : CustomBE {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(IlceBilgisi))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(IlBilgisi))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(YurtDisiAdresBilgisi))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KoyAdresBilgisi))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(BeldeAdresBilgisi))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(IlIlceMerkeziAdresBilgisi))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AdresBilgisi))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AileBilgisi))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(EvlenmeBilgisi))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NKOOlayBilgisi))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KisiOlayTarihBilgisi))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NKOKisiBilgisi))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KPSSorguSonucu))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(TCKimlikNoC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(YerlesimYerindeOturanlarC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KisiAdresBilgisiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(YabanciKisiBilgisiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AileListesiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KayipCuzdanEskiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KayipCuzdanC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(EvlenmeC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(BosanmaIstatistigiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(EvlenmeIstatistigiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(OlumIstatistigiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(DogumIstatistigiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KimlikBilgisiDegisenlerC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NufusCuzdanSatisYerC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NufusCuzdanC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NufusKayitOrnegiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CiltAdiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CiltKoduC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(OlayBilgisiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KisiBilgisiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KisiTemelBilgisi))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KisiDurumBilgisi))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KisiKayitYeriBilgisi))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KisiKayitYeriKodBilgisi))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(SearchInfoBE))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NufusCuzdanSatisYerG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KPSSorgusu))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AdresNoG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AileListesiTCKimlikNoG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(EvlenmeG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NKOTCKimlikNoG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KimlikBilgisiG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KayitYerindenKimlikBilgisiG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NKOKayitYeriG))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(TCKimlikNoG))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public abstract partial class CustomBE {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class SayfaIstekKomutu {
        
        private string istekNoField;
        
        /// <remarks/>
        public string IstekNo {
            get {
                return this.istekNoField;
            }
            set {
                this.istekNoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class SayfalamaBilgisi {
        
        private int sayfaNoField;
        
        private int toplamSayfaField;
        
        private string istekNoField;
        
        private bool sonSayfaField;
        
        /// <remarks/>
        public int SayfaNo {
            get {
                return this.sayfaNoField;
            }
            set {
                this.sayfaNoField = value;
            }
        }
        
        /// <remarks/>
        public int ToplamSayfa {
            get {
                return this.toplamSayfaField;
            }
            set {
                this.toplamSayfaField = value;
            }
        }
        
        /// <remarks/>
        public string IstekNo {
            get {
                return this.istekNoField;
            }
            set {
                this.istekNoField = value;
            }
        }
        
        /// <remarks/>
        public bool SonSayfa {
            get {
                return this.sonSayfaField;
            }
            set {
                this.sonSayfaField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KayipCuzdanCT))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KimlikBilgisiDegisenlerCT))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(OlayBilgisiCT))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KisiBilgisiCT))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class KPSVeriTablosu {
        
        private SayfalamaBilgisi sayfaBilgisiField;
        
        /// <remarks/>
        public SayfalamaBilgisi SayfaBilgisi {
            get {
                return this.sayfaBilgisiField;
            }
            set {
                this.sayfaBilgisiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class KayipCuzdanCT : KPSVeriTablosu {
        
        private KayipCuzdanC[] listeField;
        
        /// <remarks/>
        public KayipCuzdanC[] Liste {
            get {
                return this.listeField;
            }
            set {
                this.listeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class KayipCuzdanC : KPSSorguSonucu {
        
        private string kayipCuzdanSeriField;
        
        private long kayipCuzdanNoField;
        
        private TarihBilgisi kayipTarihField;
        
        private long tCKimlikNoField;
        
        private string adField;
        
        private string soyadField;
        
        private string babaAdField;
        
        private string anaAdField;
        
        private string dogumYerField;
        
        private TarihBilgisi dogumTarihField;
        
        private string yeniCuzdanSeriField;
        
        private long yeniCuzdanNoField;
        
        private TarihBilgisi yeniCuzdanVerilmeTarihField;
        
        private int yeniCuzdanVerenIlceKodField;
        
        private string yeniCuzdanVerenIlceAdField;
        
        private CuzdanKayipTipi kayipTurField;
        
        /// <remarks/>
        public string KayipCuzdanSeri {
            get {
                return this.kayipCuzdanSeriField;
            }
            set {
                this.kayipCuzdanSeriField = value;
            }
        }
        
        /// <remarks/>
        public long KayipCuzdanNo {
            get {
                return this.kayipCuzdanNoField;
            }
            set {
                this.kayipCuzdanNoField = value;
            }
        }
        
        /// <remarks/>
        public TarihBilgisi KayipTarih {
            get {
                return this.kayipTarihField;
            }
            set {
                this.kayipTarihField = value;
            }
        }
        
        /// <remarks/>
        public long TCKimlikNo {
            get {
                return this.tCKimlikNoField;
            }
            set {
                this.tCKimlikNoField = value;
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
        public string BabaAd {
            get {
                return this.babaAdField;
            }
            set {
                this.babaAdField = value;
            }
        }
        
        /// <remarks/>
        public string AnaAd {
            get {
                return this.anaAdField;
            }
            set {
                this.anaAdField = value;
            }
        }
        
        /// <remarks/>
        public string DogumYer {
            get {
                return this.dogumYerField;
            }
            set {
                this.dogumYerField = value;
            }
        }
        
        /// <remarks/>
        public TarihBilgisi DogumTarih {
            get {
                return this.dogumTarihField;
            }
            set {
                this.dogumTarihField = value;
            }
        }
        
        /// <remarks/>
        public string YeniCuzdanSeri {
            get {
                return this.yeniCuzdanSeriField;
            }
            set {
                this.yeniCuzdanSeriField = value;
            }
        }
        
        /// <remarks/>
        public long YeniCuzdanNo {
            get {
                return this.yeniCuzdanNoField;
            }
            set {
                this.yeniCuzdanNoField = value;
            }
        }
        
        /// <remarks/>
        public TarihBilgisi YeniCuzdanVerilmeTarih {
            get {
                return this.yeniCuzdanVerilmeTarihField;
            }
            set {
                this.yeniCuzdanVerilmeTarihField = value;
            }
        }
        
        /// <remarks/>
        public int YeniCuzdanVerenIlceKod {
            get {
                return this.yeniCuzdanVerenIlceKodField;
            }
            set {
                this.yeniCuzdanVerenIlceKodField = value;
            }
        }
        
        /// <remarks/>
        public string YeniCuzdanVerenIlceAd {
            get {
                return this.yeniCuzdanVerenIlceAdField;
            }
            set {
                this.yeniCuzdanVerenIlceAdField = value;
            }
        }
        
        /// <remarks/>
        public CuzdanKayipTipi KayipTur {
            get {
                return this.kayipTurField;
            }
            set {
                this.kayipTurField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class TarihBilgisi {
        
        private int yilField;
        
        private int ayField;
        
        private int gunField;
        
        /// <remarks/>
        public int Yil {
            get {
                return this.yilField;
            }
            set {
                this.yilField = value;
            }
        }
        
        /// <remarks/>
        public int Ay {
            get {
                return this.ayField;
            }
            set {
                this.ayField = value;
            }
        }
        
        /// <remarks/>
        public int Gun {
            get {
                return this.gunField;
            }
            set {
                this.gunField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public enum CuzdanKayipTipi {
        
        /// <remarks/>
        DoluKayipNufusCuzdani,
        
        /// <remarks/>
        BosKayipNufusCuzdani,
        
        /// <remarks/>
        BosKayipAileCuzdani,
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(TCKimlikNoC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(YerlesimYerindeOturanlarC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KisiAdresBilgisiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(YabanciKisiBilgisiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AileListesiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KayipCuzdanEskiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KayipCuzdanC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(EvlenmeC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(BosanmaIstatistigiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(EvlenmeIstatistigiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(OlumIstatistigiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(DogumIstatistigiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KimlikBilgisiDegisenlerC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NufusCuzdanSatisYerC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NufusCuzdanC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NufusKayitOrnegiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CiltAdiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CiltKoduC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(OlayBilgisiC))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(KisiBilgisiC))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public abstract partial class KPSSorguSonucu : CustomBE {
        
        private int hataKodField;
        
        private string hataField;
        
        private long sorguPozisyonField;
        
        /// <remarks/>
        public int HataKod {
            get {
                return this.hataKodField;
            }
            set {
                this.hataKodField = value;
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
        public long SorguPozisyon {
            get {
                return this.sorguPozisyonField;
            }
            set {
                this.sorguPozisyonField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class TCKimlikNoC : KPSSorguSonucu {
        
        private long tCKimlikNoField;
        
        private string adField;
        
        private string soyadField;
        
        private int dogumYilField;
        
        /// <remarks/>
        public long TCKimlikNo {
            get {
                return this.tCKimlikNoField;
            }
            set {
                this.tCKimlikNoField = value;
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
        public int DogumYil {
            get {
                return this.dogumYilField;
            }
            set {
                this.dogumYilField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class YerlesimYerindeOturanlarC : KPSSorguSonucu {
        
        private long adresNoField;
        
        private TCKimlikNoC[] kisiListesiField;
        
        /// <remarks/>
        public long AdresNo {
            get {
                return this.adresNoField;
            }
            set {
                this.adresNoField = value;
            }
        }
        
        /// <remarks/>
        public TCKimlikNoC[] KisiListesi {
            get {
                return this.kisiListesiField;
            }
            set {
                this.kisiListesiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class KisiAdresBilgisiC : KPSSorguSonucu {
        
        private long tCKimlikNoField;
        
        private AdresBilgisi yerlesimYeriAdresiField;
        
        /// <remarks/>
        public long TCKimlikNo {
            get {
                return this.tCKimlikNoField;
            }
            set {
                this.tCKimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public AdresBilgisi YerlesimYeriAdresi {
            get {
                return this.yerlesimYeriAdresiField;
            }
            set {
                this.yerlesimYeriAdresiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class AdresBilgisi : CustomBE {
        
        private AdresTipi tipField;
        
        private long adresNoField;
        
        private IlIlceMerkeziAdresBilgisi ilIlceMerkeziAdresiField;
        
        private BeldeAdresBilgisi beldeAdresiField;
        
        private KoyAdresBilgisi koyAdresiField;
        
        private YurtDisiAdresBilgisi yurtDisiAdresiField;
        
        /// <remarks/>
        public AdresTipi Tip {
            get {
                return this.tipField;
            }
            set {
                this.tipField = value;
            }
        }
        
        /// <remarks/>
        public long AdresNo {
            get {
                return this.adresNoField;
            }
            set {
                this.adresNoField = value;
            }
        }
        
        /// <remarks/>
        public IlIlceMerkeziAdresBilgisi IlIlceMerkeziAdresi {
            get {
                return this.ilIlceMerkeziAdresiField;
            }
            set {
                this.ilIlceMerkeziAdresiField = value;
            }
        }
        
        /// <remarks/>
        public BeldeAdresBilgisi BeldeAdresi {
            get {
                return this.beldeAdresiField;
            }
            set {
                this.beldeAdresiField = value;
            }
        }
        
        /// <remarks/>
        public KoyAdresBilgisi KoyAdresi {
            get {
                return this.koyAdresiField;
            }
            set {
                this.koyAdresiField = value;
            }
        }
        
        /// <remarks/>
        public YurtDisiAdresBilgisi YurtDisiAdresi {
            get {
                return this.yurtDisiAdresiField;
            }
            set {
                this.yurtDisiAdresiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public enum AdresTipi {
        
        /// <remarks/>
        Bilinmiyor,
        
        /// <remarks/>
        IlIlceMerkeziAdresi,
        
        /// <remarks/>
        BeldeAdresi,
        
        /// <remarks/>
        KoyAdresi,
        
        /// <remarks/>
        YurtDisiAdresi,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class IlIlceMerkeziAdresBilgisi : CustomBE {
        
        private int ilKoduField;
        
        private string ilField;
        
        private int ilceKoduField;
        
        private string ilceField;
        
        private int mahalleKodField;
        
        private string mahalleField;
        
        private int csbmKoduField;
        
        private string csbmField;
        
        private string disKapiNoField;
        
        private string icKapiNoField;
        
        /// <remarks/>
        public int IlKodu {
            get {
                return this.ilKoduField;
            }
            set {
                this.ilKoduField = value;
            }
        }
        
        /// <remarks/>
        public string Il {
            get {
                return this.ilField;
            }
            set {
                this.ilField = value;
            }
        }
        
        /// <remarks/>
        public int IlceKodu {
            get {
                return this.ilceKoduField;
            }
            set {
                this.ilceKoduField = value;
            }
        }
        
        /// <remarks/>
        public string Ilce {
            get {
                return this.ilceField;
            }
            set {
                this.ilceField = value;
            }
        }
        
        /// <remarks/>
        public int MahalleKod {
            get {
                return this.mahalleKodField;
            }
            set {
                this.mahalleKodField = value;
            }
        }
        
        /// <remarks/>
        public string Mahalle {
            get {
                return this.mahalleField;
            }
            set {
                this.mahalleField = value;
            }
        }
        
        /// <remarks/>
        public int CsbmKodu {
            get {
                return this.csbmKoduField;
            }
            set {
                this.csbmKoduField = value;
            }
        }
        
        /// <remarks/>
        public string Csbm {
            get {
                return this.csbmField;
            }
            set {
                this.csbmField = value;
            }
        }
        
        /// <remarks/>
        public string DisKapiNo {
            get {
                return this.disKapiNoField;
            }
            set {
                this.disKapiNoField = value;
            }
        }
        
        /// <remarks/>
        public string IcKapiNo {
            get {
                return this.icKapiNoField;
            }
            set {
                this.icKapiNoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class BeldeAdresBilgisi : CustomBE {
        
        private int ilKoduField;
        
        private string ilField;
        
        private int ilceKoduField;
        
        private string ilceField;
        
        private int bucakKodField;
        
        private string bucakField;
        
        private int koyKayitNoField;
        
        private int koyKodField;
        
        private string koyField;
        
        private int mahalleKodField;
        
        private string mahalleField;
        
        private int csbmKoduField;
        
        private string csbmField;
        
        private string disKapiNoField;
        
        private string icKapiNoField;
        
        /// <remarks/>
        public int IlKodu {
            get {
                return this.ilKoduField;
            }
            set {
                this.ilKoduField = value;
            }
        }
        
        /// <remarks/>
        public string Il {
            get {
                return this.ilField;
            }
            set {
                this.ilField = value;
            }
        }
        
        /// <remarks/>
        public int IlceKodu {
            get {
                return this.ilceKoduField;
            }
            set {
                this.ilceKoduField = value;
            }
        }
        
        /// <remarks/>
        public string Ilce {
            get {
                return this.ilceField;
            }
            set {
                this.ilceField = value;
            }
        }
        
        /// <remarks/>
        public int BucakKod {
            get {
                return this.bucakKodField;
            }
            set {
                this.bucakKodField = value;
            }
        }
        
        /// <remarks/>
        public string Bucak {
            get {
                return this.bucakField;
            }
            set {
                this.bucakField = value;
            }
        }
        
        /// <remarks/>
        public int KoyKayitNo {
            get {
                return this.koyKayitNoField;
            }
            set {
                this.koyKayitNoField = value;
            }
        }
        
        /// <remarks/>
        public int KoyKod {
            get {
                return this.koyKodField;
            }
            set {
                this.koyKodField = value;
            }
        }
        
        /// <remarks/>
        public string Koy {
            get {
                return this.koyField;
            }
            set {
                this.koyField = value;
            }
        }
        
        /// <remarks/>
        public int MahalleKod {
            get {
                return this.mahalleKodField;
            }
            set {
                this.mahalleKodField = value;
            }
        }
        
        /// <remarks/>
        public string Mahalle {
            get {
                return this.mahalleField;
            }
            set {
                this.mahalleField = value;
            }
        }
        
        /// <remarks/>
        public int CsbmKodu {
            get {
                return this.csbmKoduField;
            }
            set {
                this.csbmKoduField = value;
            }
        }
        
        /// <remarks/>
        public string Csbm {
            get {
                return this.csbmField;
            }
            set {
                this.csbmField = value;
            }
        }
        
        /// <remarks/>
        public string DisKapiNo {
            get {
                return this.disKapiNoField;
            }
            set {
                this.disKapiNoField = value;
            }
        }
        
        /// <remarks/>
        public string IcKapiNo {
            get {
                return this.icKapiNoField;
            }
            set {
                this.icKapiNoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class KoyAdresBilgisi : CustomBE {
        
        private int ilKoduField;
        
        private string ilField;
        
        private int ilceKoduField;
        
        private string ilceField;
        
        private int bucakKodField;
        
        private string bucakField;
        
        private int koyKayitNoField;
        
        private int koyKodField;
        
        private string koyField;
        
        private int mahalleKodField;
        
        private string mahalleField;
        
        private int csbmKoduField;
        
        private string csbmField;
        
        private string disKapiNoField;
        
        private string icKapiNoField;
        
        /// <remarks/>
        public int IlKodu {
            get {
                return this.ilKoduField;
            }
            set {
                this.ilKoduField = value;
            }
        }
        
        /// <remarks/>
        public string Il {
            get {
                return this.ilField;
            }
            set {
                this.ilField = value;
            }
        }
        
        /// <remarks/>
        public int IlceKodu {
            get {
                return this.ilceKoduField;
            }
            set {
                this.ilceKoduField = value;
            }
        }
        
        /// <remarks/>
        public string Ilce {
            get {
                return this.ilceField;
            }
            set {
                this.ilceField = value;
            }
        }
        
        /// <remarks/>
        public int BucakKod {
            get {
                return this.bucakKodField;
            }
            set {
                this.bucakKodField = value;
            }
        }
        
        /// <remarks/>
        public string Bucak {
            get {
                return this.bucakField;
            }
            set {
                this.bucakField = value;
            }
        }
        
        /// <remarks/>
        public int KoyKayitNo {
            get {
                return this.koyKayitNoField;
            }
            set {
                this.koyKayitNoField = value;
            }
        }
        
        /// <remarks/>
        public int KoyKod {
            get {
                return this.koyKodField;
            }
            set {
                this.koyKodField = value;
            }
        }
        
        /// <remarks/>
        public string Koy {
            get {
                return this.koyField;
            }
            set {
                this.koyField = value;
            }
        }
        
        /// <remarks/>
        public int MahalleKod {
            get {
                return this.mahalleKodField;
            }
            set {
                this.mahalleKodField = value;
            }
        }
        
        /// <remarks/>
        public string Mahalle {
            get {
                return this.mahalleField;
            }
            set {
                this.mahalleField = value;
            }
        }
        
        /// <remarks/>
        public int CsbmKodu {
            get {
                return this.csbmKoduField;
            }
            set {
                this.csbmKoduField = value;
            }
        }
        
        /// <remarks/>
        public string Csbm {
            get {
                return this.csbmField;
            }
            set {
                this.csbmField = value;
            }
        }
        
        /// <remarks/>
        public string DisKapiNo {
            get {
                return this.disKapiNoField;
            }
            set {
                this.disKapiNoField = value;
            }
        }
        
        /// <remarks/>
        public string IcKapiNo {
            get {
                return this.icKapiNoField;
            }
            set {
                this.icKapiNoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class YurtDisiAdresBilgisi : CustomBE {
        
        private string ulkeField;
        
        private string sehirField;
        
        private string acikAdresField;
        
        /// <remarks/>
        public string Ulke {
            get {
                return this.ulkeField;
            }
            set {
                this.ulkeField = value;
            }
        }
        
        /// <remarks/>
        public string Sehir {
            get {
                return this.sehirField;
            }
            set {
                this.sehirField = value;
            }
        }
        
        /// <remarks/>
        public string AcikAdres {
            get {
                return this.acikAdresField;
            }
            set {
                this.acikAdresField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class YabanciKisiBilgisiC : KPSSorguSonucu {
        
        private long kimlikNoField;
        
        private string egmSahisIdField;
        
        private string adField;
        
        private string soyadField;
        
        private CinsiyetTipi cinsiyetField;
        
        private string babaAdField;
        
        private string anneAdField;
        
        private TarihBilgisi dogumTarihField;
        
        private string dogumYerField;
        
        private MedeniHalTipi medeniHalField;
        
        private int uyrukKodField;
        
        private string uyrukAdField;
        
        private int ikametDuzenleyenIlKodField;
        
        private string ikametDuzenleyenIlAdField;
        
        private string ikametTezkereNoField;
        
        private int ikametSuresiKodField;
        
        private string ikametSuresiAdField;
        
        private TarihBilgisi izinBaslangicTarihField;
        
        private string babaEgmSahisIdField;
        
        private long babaKimlikNoField;
        
        private string anneEgmSahisIdField;
        
        private long anneKimlikNoField;
        
        private string esEgmSahisIdField;
        
        private int ikametAdresIlKodField;
        
        private string ikametAdresIlAdField;
        
        private string ikametAdresIlceAdField;
        
        private string ikametAdresField;
        
        /// <remarks/>
        public long KimlikNo {
            get {
                return this.kimlikNoField;
            }
            set {
                this.kimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public string EgmSahisId {
            get {
                return this.egmSahisIdField;
            }
            set {
                this.egmSahisIdField = value;
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
        public CinsiyetTipi Cinsiyet {
            get {
                return this.cinsiyetField;
            }
            set {
                this.cinsiyetField = value;
            }
        }
        
        /// <remarks/>
        public string BabaAd {
            get {
                return this.babaAdField;
            }
            set {
                this.babaAdField = value;
            }
        }
        
        /// <remarks/>
        public string AnneAd {
            get {
                return this.anneAdField;
            }
            set {
                this.anneAdField = value;
            }
        }
        
        /// <remarks/>
        public TarihBilgisi DogumTarih {
            get {
                return this.dogumTarihField;
            }
            set {
                this.dogumTarihField = value;
            }
        }
        
        /// <remarks/>
        public string DogumYer {
            get {
                return this.dogumYerField;
            }
            set {
                this.dogumYerField = value;
            }
        }
        
        /// <remarks/>
        public MedeniHalTipi MedeniHal {
            get {
                return this.medeniHalField;
            }
            set {
                this.medeniHalField = value;
            }
        }
        
        /// <remarks/>
        public int UyrukKod {
            get {
                return this.uyrukKodField;
            }
            set {
                this.uyrukKodField = value;
            }
        }
        
        /// <remarks/>
        public string UyrukAd {
            get {
                return this.uyrukAdField;
            }
            set {
                this.uyrukAdField = value;
            }
        }
        
        /// <remarks/>
        public int IkametDuzenleyenIlKod {
            get {
                return this.ikametDuzenleyenIlKodField;
            }
            set {
                this.ikametDuzenleyenIlKodField = value;
            }
        }
        
        /// <remarks/>
        public string IkametDuzenleyenIlAd {
            get {
                return this.ikametDuzenleyenIlAdField;
            }
            set {
                this.ikametDuzenleyenIlAdField = value;
            }
        }
        
        /// <remarks/>
        public string IkametTezkereNo {
            get {
                return this.ikametTezkereNoField;
            }
            set {
                this.ikametTezkereNoField = value;
            }
        }
        
        /// <remarks/>
        public int IkametSuresiKod {
            get {
                return this.ikametSuresiKodField;
            }
            set {
                this.ikametSuresiKodField = value;
            }
        }
        
        /// <remarks/>
        public string IkametSuresiAd {
            get {
                return this.ikametSuresiAdField;
            }
            set {
                this.ikametSuresiAdField = value;
            }
        }
        
        /// <remarks/>
        public TarihBilgisi IzinBaslangicTarih {
            get {
                return this.izinBaslangicTarihField;
            }
            set {
                this.izinBaslangicTarihField = value;
            }
        }
        
        /// <remarks/>
        public string BabaEgmSahisId {
            get {
                return this.babaEgmSahisIdField;
            }
            set {
                this.babaEgmSahisIdField = value;
            }
        }
        
        /// <remarks/>
        public long BabaKimlikNo {
            get {
                return this.babaKimlikNoField;
            }
            set {
                this.babaKimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public string AnneEgmSahisId {
            get {
                return this.anneEgmSahisIdField;
            }
            set {
                this.anneEgmSahisIdField = value;
            }
        }
        
        /// <remarks/>
        public long AnneKimlikNo {
            get {
                return this.anneKimlikNoField;
            }
            set {
                this.anneKimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public string EsEgmSahisId {
            get {
                return this.esEgmSahisIdField;
            }
            set {
                this.esEgmSahisIdField = value;
            }
        }
        
        /// <remarks/>
        public int IkametAdresIlKod {
            get {
                return this.ikametAdresIlKodField;
            }
            set {
                this.ikametAdresIlKodField = value;
            }
        }
        
        /// <remarks/>
        public string IkametAdresIlAd {
            get {
                return this.ikametAdresIlAdField;
            }
            set {
                this.ikametAdresIlAdField = value;
            }
        }
        
        /// <remarks/>
        public string IkametAdresIlceAd {
            get {
                return this.ikametAdresIlceAdField;
            }
            set {
                this.ikametAdresIlceAdField = value;
            }
        }
        
        /// <remarks/>
        public string IkametAdres {
            get {
                return this.ikametAdresField;
            }
            set {
                this.ikametAdresField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public enum CinsiyetTipi {
        
        /// <remarks/>
        Bilinmiyor,
        
        /// <remarks/>
        Erkek,
        
        /// <remarks/>
        Kadin,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public enum MedeniHalTipi {
        
        /// <remarks/>
        Bilinmeyen,
        
        /// <remarks/>
        Bekar,
        
        /// <remarks/>
        Evli,
        
        /// <remarks/>
        Bosanmis,
        
        /// <remarks/>
        Dul,
        
        /// <remarks/>
        EvliliginFeshi,
        
        /// <remarks/>
        EvliliginIptali,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class AileListesiC : KPSSorguSonucu {
        
        private long tCKimlikNoField;
        
        private AileBilgisi[] aileListesiField;
        
        /// <remarks/>
        public long TCKimlikNo {
            get {
                return this.tCKimlikNoField;
            }
            set {
                this.tCKimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public AileBilgisi[] AileListesi {
            get {
                return this.aileListesiField;
            }
            set {
                this.aileListesiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class AileBilgisi : CustomBE {
        
        private long tCKimlikNoField;
        
        private KisiTemelBilgisi temelBilgisiField;
        
        private KisiDurumBilgisi durumBilgisiField;
        
        private KisiKayitYeriBilgisi kayitYeriBilgisiField;
        
        private YakinlikTipi yakinlikField;
        
        /// <remarks/>
        public long TCKimlikNo {
            get {
                return this.tCKimlikNoField;
            }
            set {
                this.tCKimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public KisiTemelBilgisi TemelBilgisi {
            get {
                return this.temelBilgisiField;
            }
            set {
                this.temelBilgisiField = value;
            }
        }
        
        /// <remarks/>
        public KisiDurumBilgisi DurumBilgisi {
            get {
                return this.durumBilgisiField;
            }
            set {
                this.durumBilgisiField = value;
            }
        }
        
        /// <remarks/>
        public KisiKayitYeriBilgisi KayitYeriBilgisi {
            get {
                return this.kayitYeriBilgisiField;
            }
            set {
                this.kayitYeriBilgisiField = value;
            }
        }
        
        /// <remarks/>
        public YakinlikTipi Yakinlik {
            get {
                return this.yakinlikField;
            }
            set {
                this.yakinlikField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class KisiTemelBilgisi : CustomBE {
        
        private string adField;
        
        private string soyadField;
        
        private CinsiyetTipi cinsiyetField;
        
        private string babaAdField;
        
        private string babaSoyadField;
        
        private string anaAdField;
        
        private string anaSoyadField;
        
        private TarihBilgisi dogumTarihField;
        
        private string dogumYerField;
        
        private string kizlikSoyadField;
        
        private int ozurOranField;
        
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
        public CinsiyetTipi Cinsiyet {
            get {
                return this.cinsiyetField;
            }
            set {
                this.cinsiyetField = value;
            }
        }
        
        /// <remarks/>
        public string BabaAd {
            get {
                return this.babaAdField;
            }
            set {
                this.babaAdField = value;
            }
        }
        
        /// <remarks/>
        public string BabaSoyad {
            get {
                return this.babaSoyadField;
            }
            set {
                this.babaSoyadField = value;
            }
        }
        
        /// <remarks/>
        public string AnaAd {
            get {
                return this.anaAdField;
            }
            set {
                this.anaAdField = value;
            }
        }
        
        /// <remarks/>
        public string AnaSoyad {
            get {
                return this.anaSoyadField;
            }
            set {
                this.anaSoyadField = value;
            }
        }
        
        /// <remarks/>
        public TarihBilgisi DogumTarih {
            get {
                return this.dogumTarihField;
            }
            set {
                this.dogumTarihField = value;
            }
        }
        
        /// <remarks/>
        public string DogumYer {
            get {
                return this.dogumYerField;
            }
            set {
                this.dogumYerField = value;
            }
        }
        
        /// <remarks/>
        public string KizlikSoyad {
            get {
                return this.kizlikSoyadField;
            }
            set {
                this.kizlikSoyadField = value;
            }
        }
        
        /// <remarks/>
        public int OzurOran {
            get {
                return this.ozurOranField;
            }
            set {
                this.ozurOranField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class KisiDurumBilgisi : CustomBE {
        
        private KisiDurumTipi durumField;
        
        private MedeniHalTipi medeniHalField;
        
        private DinTipi dinField;
        
        private TarihBilgisi olumTarihField;
        
        private string olumYerField;
        
        /// <remarks/>
        public KisiDurumTipi Durum {
            get {
                return this.durumField;
            }
            set {
                this.durumField = value;
            }
        }
        
        /// <remarks/>
        public MedeniHalTipi MedeniHal {
            get {
                return this.medeniHalField;
            }
            set {
                this.medeniHalField = value;
            }
        }
        
        /// <remarks/>
        public DinTipi Din {
            get {
                return this.dinField;
            }
            set {
                this.dinField = value;
            }
        }
        
        /// <remarks/>
        public TarihBilgisi OlumTarih {
            get {
                return this.olumTarihField;
            }
            set {
                this.olumTarihField = value;
            }
        }
        
        /// <remarks/>
        public string OlumYer {
            get {
                return this.olumYerField;
            }
            set {
                this.olumYerField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public enum KisiDurumTipi {
        
        /// <remarks/>
        Bilinmeyen,
        
        /// <remarks/>
        Acik,
        
        /// <remarks/>
        Kapali,
        
        /// <remarks/>
        KapaliOlum,
        
        /// <remarks/>
        KapaliGaiplik,
        
        /// <remarks/>
        KapaliVatandasliktanCikma,
        
        /// <remarks/>
        KapaliKayitSilme,
        
        /// <remarks/>
        KapaliMukerrerKayitSilme,
        
        /// <remarks/>
        KapaliDiger,
        
        /// <remarks/>
        KapaliCiltBolmeBirlestirme,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public enum DinTipi {
        
        /// <remarks/>
        Bilinmeyen,
        
        /// <remarks/>
        Islam,
        
        /// <remarks/>
        Hiristiyan,
        
        /// <remarks/>
        Musevi,
        
        /// <remarks/>
        Suryani,
        
        /// <remarks/>
        SuryaniKadim,
        
        /// <remarks/>
        Budist,
        
        /// <remarks/>
        Dinsiz,
        
        /// <remarks/>
        DigerDin,
        
        /// <remarks/>
        BilinmeyenDin,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class KisiKayitYeriBilgisi : CustomBE {
        
        private int ilKodField;
        
        private string ilAdField;
        
        private int ilceKodField;
        
        private string ilceAdField;
        
        private int ciltKodField;
        
        private string ciltAdField;
        
        private int aileSiraNoField;
        
        private int bireySiraNoField;
        
        /// <remarks/>
        public int IlKod {
            get {
                return this.ilKodField;
            }
            set {
                this.ilKodField = value;
            }
        }
        
        /// <remarks/>
        public string IlAd {
            get {
                return this.ilAdField;
            }
            set {
                this.ilAdField = value;
            }
        }
        
        /// <remarks/>
        public int IlceKod {
            get {
                return this.ilceKodField;
            }
            set {
                this.ilceKodField = value;
            }
        }
        
        /// <remarks/>
        public string IlceAd {
            get {
                return this.ilceAdField;
            }
            set {
                this.ilceAdField = value;
            }
        }
        
        /// <remarks/>
        public int CiltKod {
            get {
                return this.ciltKodField;
            }
            set {
                this.ciltKodField = value;
            }
        }
        
        /// <remarks/>
        public string CiltAd {
            get {
                return this.ciltAdField;
            }
            set {
                this.ciltAdField = value;
            }
        }
        
        /// <remarks/>
        public int AileSiraNo {
            get {
                return this.aileSiraNoField;
            }
            set {
                this.aileSiraNoField = value;
            }
        }
        
        /// <remarks/>
        public int BireySiraNo {
            get {
                return this.bireySiraNoField;
            }
            set {
                this.bireySiraNoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public enum YakinlikTipi {
        
        /// <remarks/>
        Kendisi,
        
        /// <remarks/>
        Esi,
        
        /// <remarks/>
        Oglu,
        
        /// <remarks/>
        Kizi,
        
        /// <remarks/>
        EskiEsi,
        
        /// <remarks/>
        Babasi,
        
        /// <remarks/>
        Annesi,
        
        /// <remarks/>
        Kardesi,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class KayipCuzdanEskiC : KPSSorguSonucu {
        
        private string kayipCuzdanSeriField;
        
        private long kayipCuzdanNoField;
        
        private int kayipOlduguYilField;
        
        private string adField;
        
        private string soyadField;
        
        private string babaAdField;
        
        private TarihBilgisi dogumTarihField;
        
        /// <remarks/>
        public string KayipCuzdanSeri {
            get {
                return this.kayipCuzdanSeriField;
            }
            set {
                this.kayipCuzdanSeriField = value;
            }
        }
        
        /// <remarks/>
        public long KayipCuzdanNo {
            get {
                return this.kayipCuzdanNoField;
            }
            set {
                this.kayipCuzdanNoField = value;
            }
        }
        
        /// <remarks/>
        public int KayipOlduguYil {
            get {
                return this.kayipOlduguYilField;
            }
            set {
                this.kayipOlduguYilField = value;
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
        public string BabaAd {
            get {
                return this.babaAdField;
            }
            set {
                this.babaAdField = value;
            }
        }
        
        /// <remarks/>
        public TarihBilgisi DogumTarih {
            get {
                return this.dogumTarihField;
            }
            set {
                this.dogumTarihField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class EvlenmeC : KPSSorguSonucu {
        
        private long tCKimlikNoField;
        
        private KisiTemelBilgisi temelBilgisiField;
        
        private KisiKayitYeriBilgisi kayitYeriField;
        
        private EvlenmeBilgisi[] evlenmeListesiField;
        
        /// <remarks/>
        public long TCKimlikNo {
            get {
                return this.tCKimlikNoField;
            }
            set {
                this.tCKimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public KisiTemelBilgisi TemelBilgisi {
            get {
                return this.temelBilgisiField;
            }
            set {
                this.temelBilgisiField = value;
            }
        }
        
        /// <remarks/>
        public KisiKayitYeriBilgisi KayitYeri {
            get {
                return this.kayitYeriField;
            }
            set {
                this.kayitYeriField = value;
            }
        }
        
        /// <remarks/>
        public EvlenmeBilgisi[] EvlenmeListesi {
            get {
                return this.evlenmeListesiField;
            }
            set {
                this.evlenmeListesiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class EvlenmeBilgisi : CustomBE {
        
        private long esTCKimlikNoField;
        
        private KisiTemelBilgisi esTemelBilgisiField;
        
        private KisiKayitYeriBilgisi esKayitYeriBilgisiField;
        
        private TarihBilgisi evlenmeTarihiField;
        
        private string evlenmeYeriField;
        
        /// <remarks/>
        public long EsTCKimlikNo {
            get {
                return this.esTCKimlikNoField;
            }
            set {
                this.esTCKimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public KisiTemelBilgisi EsTemelBilgisi {
            get {
                return this.esTemelBilgisiField;
            }
            set {
                this.esTemelBilgisiField = value;
            }
        }
        
        /// <remarks/>
        public KisiKayitYeriBilgisi EsKayitYeriBilgisi {
            get {
                return this.esKayitYeriBilgisiField;
            }
            set {
                this.esKayitYeriBilgisiField = value;
            }
        }
        
        /// <remarks/>
        public TarihBilgisi EvlenmeTarihi {
            get {
                return this.evlenmeTarihiField;
            }
            set {
                this.evlenmeTarihiField = value;
            }
        }
        
        /// <remarks/>
        public string EvlenmeYeri {
            get {
                return this.evlenmeYeriField;
            }
            set {
                this.evlenmeYeriField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class BosanmaIstatistigiC : KPSSorguSonucu {
        
        private int ilKodField;
        
        private string ilAdField;
        
        private int ilceKodField;
        
        private string ilceAdField;
        
        private int yilField;
        
        private int ayField;
        
        private int sayiField;
        
        /// <remarks/>
        public int IlKod {
            get {
                return this.ilKodField;
            }
            set {
                this.ilKodField = value;
            }
        }
        
        /// <remarks/>
        public string IlAd {
            get {
                return this.ilAdField;
            }
            set {
                this.ilAdField = value;
            }
        }
        
        /// <remarks/>
        public int IlceKod {
            get {
                return this.ilceKodField;
            }
            set {
                this.ilceKodField = value;
            }
        }
        
        /// <remarks/>
        public string IlceAd {
            get {
                return this.ilceAdField;
            }
            set {
                this.ilceAdField = value;
            }
        }
        
        /// <remarks/>
        public int Yil {
            get {
                return this.yilField;
            }
            set {
                this.yilField = value;
            }
        }
        
        /// <remarks/>
        public int Ay {
            get {
                return this.ayField;
            }
            set {
                this.ayField = value;
            }
        }
        
        /// <remarks/>
        public int Sayi {
            get {
                return this.sayiField;
            }
            set {
                this.sayiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class EvlenmeIstatistigiC : KPSSorguSonucu {
        
        private int ilKodField;
        
        private string ilAdField;
        
        private int ilceKodField;
        
        private string ilceAdField;
        
        private int yilField;
        
        private int ayField;
        
        private int sayiField;
        
        /// <remarks/>
        public int IlKod {
            get {
                return this.ilKodField;
            }
            set {
                this.ilKodField = value;
            }
        }
        
        /// <remarks/>
        public string IlAd {
            get {
                return this.ilAdField;
            }
            set {
                this.ilAdField = value;
            }
        }
        
        /// <remarks/>
        public int IlceKod {
            get {
                return this.ilceKodField;
            }
            set {
                this.ilceKodField = value;
            }
        }
        
        /// <remarks/>
        public string IlceAd {
            get {
                return this.ilceAdField;
            }
            set {
                this.ilceAdField = value;
            }
        }
        
        /// <remarks/>
        public int Yil {
            get {
                return this.yilField;
            }
            set {
                this.yilField = value;
            }
        }
        
        /// <remarks/>
        public int Ay {
            get {
                return this.ayField;
            }
            set {
                this.ayField = value;
            }
        }
        
        /// <remarks/>
        public int Sayi {
            get {
                return this.sayiField;
            }
            set {
                this.sayiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class OlumIstatistigiC : KPSSorguSonucu {
        
        private int ilKodField;
        
        private string ilAdField;
        
        private int ilceKodField;
        
        private string ilceAdField;
        
        private int yilField;
        
        private int ayField;
        
        private CinsiyetTipi cinsiyetField;
        
        private int sayiField;
        
        /// <remarks/>
        public int IlKod {
            get {
                return this.ilKodField;
            }
            set {
                this.ilKodField = value;
            }
        }
        
        /// <remarks/>
        public string IlAd {
            get {
                return this.ilAdField;
            }
            set {
                this.ilAdField = value;
            }
        }
        
        /// <remarks/>
        public int IlceKod {
            get {
                return this.ilceKodField;
            }
            set {
                this.ilceKodField = value;
            }
        }
        
        /// <remarks/>
        public string IlceAd {
            get {
                return this.ilceAdField;
            }
            set {
                this.ilceAdField = value;
            }
        }
        
        /// <remarks/>
        public int Yil {
            get {
                return this.yilField;
            }
            set {
                this.yilField = value;
            }
        }
        
        /// <remarks/>
        public int Ay {
            get {
                return this.ayField;
            }
            set {
                this.ayField = value;
            }
        }
        
        /// <remarks/>
        public CinsiyetTipi Cinsiyet {
            get {
                return this.cinsiyetField;
            }
            set {
                this.cinsiyetField = value;
            }
        }
        
        /// <remarks/>
        public int Sayi {
            get {
                return this.sayiField;
            }
            set {
                this.sayiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class DogumIstatistigiC : KPSSorguSonucu {
        
        private int ilKodField;
        
        private string ilAdField;
        
        private int ilceKodField;
        
        private string ilceAdField;
        
        private int yilField;
        
        private int ayField;
        
        private CinsiyetTipi cinsiyetField;
        
        private int sayiField;
        
        /// <remarks/>
        public int IlKod {
            get {
                return this.ilKodField;
            }
            set {
                this.ilKodField = value;
            }
        }
        
        /// <remarks/>
        public string IlAd {
            get {
                return this.ilAdField;
            }
            set {
                this.ilAdField = value;
            }
        }
        
        /// <remarks/>
        public int IlceKod {
            get {
                return this.ilceKodField;
            }
            set {
                this.ilceKodField = value;
            }
        }
        
        /// <remarks/>
        public string IlceAd {
            get {
                return this.ilceAdField;
            }
            set {
                this.ilceAdField = value;
            }
        }
        
        /// <remarks/>
        public int Yil {
            get {
                return this.yilField;
            }
            set {
                this.yilField = value;
            }
        }
        
        /// <remarks/>
        public int Ay {
            get {
                return this.ayField;
            }
            set {
                this.ayField = value;
            }
        }
        
        /// <remarks/>
        public CinsiyetTipi Cinsiyet {
            get {
                return this.cinsiyetField;
            }
            set {
                this.cinsiyetField = value;
            }
        }
        
        /// <remarks/>
        public int Sayi {
            get {
                return this.sayiField;
            }
            set {
                this.sayiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class KimlikBilgisiDegisenlerC : KPSSorguSonucu {
        
        private long tCKimlikNoField;
        
        private KisiTemelBilgisi temelBilgisiField;
        
        /// <remarks/>
        public long TCKimlikNo {
            get {
                return this.tCKimlikNoField;
            }
            set {
                this.tCKimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public KisiTemelBilgisi TemelBilgisi {
            get {
                return this.temelBilgisiField;
            }
            set {
                this.temelBilgisiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class NufusCuzdanSatisYerC : KPSSorguSonucu {
        
        private string satilanYerField;
        
        private CinsiyetTipi cuzdanTurField;
        
        /// <remarks/>
        public string SatilanYer {
            get {
                return this.satilanYerField;
            }
            set {
                this.satilanYerField = value;
            }
        }
        
        /// <remarks/>
        public CinsiyetTipi CuzdanTur {
            get {
                return this.cuzdanTurField;
            }
            set {
                this.cuzdanTurField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class NufusCuzdanC : KPSSorguSonucu {
        
        private long tCKimlikNoField;
        
        private string cuzdanSeriField;
        
        private long cuzdanNoField;
        
        private string adField;
        
        private string soyadField;
        
        private string babaAdField;
        
        private string anaAdField;
        
        private string dogumYerField;
        
        private TarihBilgisi dogumTarihField;
        
        private int verildigiIlceKodField;
        
        private string verildigiIlceAdField;
        
        private TarihBilgisi verilmeTarihField;
        
        private CuzdanVerilmeNedenTipi verilmeNedenField;
        
        private long kayitNoField;
        
        /// <remarks/>
        public long TCKimlikNo {
            get {
                return this.tCKimlikNoField;
            }
            set {
                this.tCKimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public string CuzdanSeri {
            get {
                return this.cuzdanSeriField;
            }
            set {
                this.cuzdanSeriField = value;
            }
        }
        
        /// <remarks/>
        public long CuzdanNo {
            get {
                return this.cuzdanNoField;
            }
            set {
                this.cuzdanNoField = value;
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
        public string BabaAd {
            get {
                return this.babaAdField;
            }
            set {
                this.babaAdField = value;
            }
        }
        
        /// <remarks/>
        public string AnaAd {
            get {
                return this.anaAdField;
            }
            set {
                this.anaAdField = value;
            }
        }
        
        /// <remarks/>
        public string DogumYer {
            get {
                return this.dogumYerField;
            }
            set {
                this.dogumYerField = value;
            }
        }
        
        /// <remarks/>
        public TarihBilgisi DogumTarih {
            get {
                return this.dogumTarihField;
            }
            set {
                this.dogumTarihField = value;
            }
        }
        
        /// <remarks/>
        public int VerildigiIlceKod {
            get {
                return this.verildigiIlceKodField;
            }
            set {
                this.verildigiIlceKodField = value;
            }
        }
        
        /// <remarks/>
        public string VerildigiIlceAd {
            get {
                return this.verildigiIlceAdField;
            }
            set {
                this.verildigiIlceAdField = value;
            }
        }
        
        /// <remarks/>
        public TarihBilgisi VerilmeTarih {
            get {
                return this.verilmeTarihField;
            }
            set {
                this.verilmeTarihField = value;
            }
        }
        
        /// <remarks/>
        public CuzdanVerilmeNedenTipi verilmeNeden {
            get {
                return this.verilmeNedenField;
            }
            set {
                this.verilmeNedenField = value;
            }
        }
        
        /// <remarks/>
        public long KayitNo {
            get {
                return this.kayitNoField;
            }
            set {
                this.kayitNoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public enum CuzdanVerilmeNedenTipi {
        
        /// <remarks/>
        Dogum,
        
        /// <remarks/>
        Yeniden,
        
        /// <remarks/>
        Kayip,
        
        /// <remarks/>
        Yenileme,
        
        /// <remarks/>
        Degistirme,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class NufusKayitOrnegiC : KPSSorguSonucu {
        
        private KisiKayitYeriBilgisi kayitYeriField;
        
        private NKOKisiBilgisi[] kisiListesiField;
        
        private NKOOlayBilgisi[] olayListesiField;
        
        /// <remarks/>
        public KisiKayitYeriBilgisi KayitYeri {
            get {
                return this.kayitYeriField;
            }
            set {
                this.kayitYeriField = value;
            }
        }
        
        /// <remarks/>
        public NKOKisiBilgisi[] KisiListesi {
            get {
                return this.kisiListesiField;
            }
            set {
                this.kisiListesiField = value;
            }
        }
        
        /// <remarks/>
        public NKOOlayBilgisi[] OlayListesi {
            get {
                return this.olayListesiField;
            }
            set {
                this.olayListesiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class NKOKisiBilgisi : CustomBE {
        
        private long tCKimlikNoField;
        
        private KisiTemelBilgisi temelBilgisiField;
        
        private KisiDurumBilgisi durumBilgisiField;
        
        private KisiKayitYeriBilgisi kayitYeriBilgisiField;
        
        private KisiOlayTarihBilgisi olayTarihBilgisiField;
        
        private YakinlikTipi yakinlikField;
        
        /// <remarks/>
        public long TCKimlikNo {
            get {
                return this.tCKimlikNoField;
            }
            set {
                this.tCKimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public KisiTemelBilgisi TemelBilgisi {
            get {
                return this.temelBilgisiField;
            }
            set {
                this.temelBilgisiField = value;
            }
        }
        
        /// <remarks/>
        public KisiDurumBilgisi DurumBilgisi {
            get {
                return this.durumBilgisiField;
            }
            set {
                this.durumBilgisiField = value;
            }
        }
        
        /// <remarks/>
        public KisiKayitYeriBilgisi KayitYeriBilgisi {
            get {
                return this.kayitYeriBilgisiField;
            }
            set {
                this.kayitYeriBilgisiField = value;
            }
        }
        
        /// <remarks/>
        public KisiOlayTarihBilgisi OlayTarihBilgisi {
            get {
                return this.olayTarihBilgisiField;
            }
            set {
                this.olayTarihBilgisiField = value;
            }
        }
        
        /// <remarks/>
        public YakinlikTipi Yakinlik {
            get {
                return this.yakinlikField;
            }
            set {
                this.yakinlikField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class KisiOlayTarihBilgisi : CustomBE {
        
        private TarihBilgisi tescilTarihField;
        
        private TarihBilgisi evlenmeTarihField;
        
        private TarihBilgisi bosanmaTarihField;
        
        /// <remarks/>
        public TarihBilgisi TescilTarih {
            get {
                return this.tescilTarihField;
            }
            set {
                this.tescilTarihField = value;
            }
        }
        
        /// <remarks/>
        public TarihBilgisi EvlenmeTarih {
            get {
                return this.evlenmeTarihField;
            }
            set {
                this.evlenmeTarihField = value;
            }
        }
        
        /// <remarks/>
        public TarihBilgisi BosanmaTarih {
            get {
                return this.bosanmaTarihField;
            }
            set {
                this.bosanmaTarihField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class NKOOlayBilgisi : CustomBE {
        
        private KisiKayitYeriKodBilgisi kayitYeriField;
        
        private TarihBilgisi olayTarihField;
        
        private NKOOlayTipi olayTipiField;
        
        private string dusunceField;
        
        private string adField;
        
        private string soyadField;
        
        /// <remarks/>
        public KisiKayitYeriKodBilgisi KayitYeri {
            get {
                return this.kayitYeriField;
            }
            set {
                this.kayitYeriField = value;
            }
        }
        
        /// <remarks/>
        public TarihBilgisi OlayTarih {
            get {
                return this.olayTarihField;
            }
            set {
                this.olayTarihField = value;
            }
        }
        
        /// <remarks/>
        public NKOOlayTipi OlayTipi {
            get {
                return this.olayTipiField;
            }
            set {
                this.olayTipiField = value;
            }
        }
        
        /// <remarks/>
        public string Dusunce {
            get {
                return this.dusunceField;
            }
            set {
                this.dusunceField = value;
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
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class KisiKayitYeriKodBilgisi : CustomBE {
        
        private int ilceKodField;
        
        private int ciltKodField;
        
        private int aileSiraNoField;
        
        private int bireySiraNoField;
        
        /// <remarks/>
        public int IlceKod {
            get {
                return this.ilceKodField;
            }
            set {
                this.ilceKodField = value;
            }
        }
        
        /// <remarks/>
        public int CiltKod {
            get {
                return this.ciltKodField;
            }
            set {
                this.ciltKodField = value;
            }
        }
        
        /// <remarks/>
        public int AileSiraNo {
            get {
                return this.aileSiraNoField;
            }
            set {
                this.aileSiraNoField = value;
            }
        }
        
        /// <remarks/>
        public int BireySiraNo {
            get {
                return this.bireySiraNoField;
            }
            set {
                this.bireySiraNoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public enum NKOOlayTipi {
        
        /// <remarks/>
        Bilinmeyen,
        
        /// <remarks/>
        DigerOlaylar,
        
        /// <remarks/>
        SoybagiDuzeltme,
        
        /// <remarks/>
        SoyBagininReddi,
        
        /// <remarks/>
        EvlatEdinme,
        
        /// <remarks/>
        EvlatliginReddi,
        
        /// <remarks/>
        Bosanma,
        
        /// <remarks/>
        BosanmaIptali,
        
        /// <remarks/>
        KayitDuzeltme,
        
        /// <remarks/>
        Evlenme,
        
        /// <remarks/>
        EvlenmeninFeshiIptali,
        
        /// <remarks/>
        Gaiplik,
        
        /// <remarks/>
        IdareceKayitDuzeltme,
        
        /// <remarks/>
        KayitSilme,
        
        /// <remarks/>
        KayitTasima,
        
        /// <remarks/>
        MukerrerKayitSilme,
        
        /// <remarks/>
        Nakil,
        
        /// <remarks/>
        Diger,
        
        /// <remarks/>
        SosyalGuvenlik,
        
        /// <remarks/>
        Tanima,
        
        /// <remarks/>
        Vasiyet,
        
        /// <remarks/>
        Vatandaslik,
        
        /// <remarks/>
        Velayet,
        
        /// <remarks/>
        Dogum,
        
        /// <remarks/>
        YesilKart,
        
        /// <remarks/>
        Olum,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class CiltAdiC : KPSSorguSonucu {
        
        private string ciltAdiField;
        
        private string ciltTipiField;
        
        /// <remarks/>
        public string CiltAdi {
            get {
                return this.ciltAdiField;
            }
            set {
                this.ciltAdiField = value;
            }
        }
        
        /// <remarks/>
        public string CiltTipi {
            get {
                return this.ciltTipiField;
            }
            set {
                this.ciltTipiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class CiltKoduC : KPSSorguSonucu {
        
        private int ciltKoduField;
        
        private string ciltTipiField;
        
        /// <remarks/>
        public int CiltKodu {
            get {
                return this.ciltKoduField;
            }
            set {
                this.ciltKoduField = value;
            }
        }
        
        /// <remarks/>
        public string CiltTipi {
            get {
                return this.ciltTipiField;
            }
            set {
                this.ciltTipiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class OlayBilgisiC : KPSSorguSonucu {
        
        private long tCKimlikNoField;
        
        private TarihBilgisi olayTarihField;
        
        /// <remarks/>
        public long TCKimlikNo {
            get {
                return this.tCKimlikNoField;
            }
            set {
                this.tCKimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public TarihBilgisi OlayTarih {
            get {
                return this.olayTarihField;
            }
            set {
                this.olayTarihField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class KisiBilgisiC : KPSSorguSonucu {
        
        private long tCKimlikNoField;
        
        private KisiTemelBilgisi temelBilgisiField;
        
        private KisiDurumBilgisi durumBilgisiField;
        
        private KisiKayitYeriBilgisi kayitYeriBilgisiField;
        
        /// <remarks/>
        public long TCKimlikNo {
            get {
                return this.tCKimlikNoField;
            }
            set {
                this.tCKimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public KisiTemelBilgisi TemelBilgisi {
            get {
                return this.temelBilgisiField;
            }
            set {
                this.temelBilgisiField = value;
            }
        }
        
        /// <remarks/>
        public KisiDurumBilgisi DurumBilgisi {
            get {
                return this.durumBilgisiField;
            }
            set {
                this.durumBilgisiField = value;
            }
        }
        
        /// <remarks/>
        public KisiKayitYeriBilgisi KayitYeriBilgisi {
            get {
                return this.kayitYeriBilgisiField;
            }
            set {
                this.kayitYeriBilgisiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class KimlikBilgisiDegisenlerCT : KPSVeriTablosu {
        
        private KimlikBilgisiDegisenlerC[] listeField;
        
        /// <remarks/>
        public KimlikBilgisiDegisenlerC[] Liste {
            get {
                return this.listeField;
            }
            set {
                this.listeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class OlayBilgisiCT : KPSVeriTablosu {
        
        private OlayBilgisiC[] listeField;
        
        /// <remarks/>
        public OlayBilgisiC[] Liste {
            get {
                return this.listeField;
            }
            set {
                this.listeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class KisiBilgisiCT : KPSVeriTablosu {
        
        private KisiBilgisiC[] listeField;
        
        /// <remarks/>
        public KisiBilgisiC[] Liste {
            get {
                return this.listeField;
            }
            set {
                this.listeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class IlceBilgisi : CustomBE {
        
        private int kodField;
        
        private string adField;
        
        private int ilKodField;
        
        /// <remarks/>
        public int Kod {
            get {
                return this.kodField;
            }
            set {
                this.kodField = value;
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
        public int IlKod {
            get {
                return this.ilKodField;
            }
            set {
                this.ilKodField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class IlBilgisi : CustomBE {
        
        private int kodField;
        
        private string adField;
        
        /// <remarks/>
        public int Kod {
            get {
                return this.kodField;
            }
            set {
                this.kodField = value;
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
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class NufusCuzdanSatisYerG : SearchInfoBE {
        
        private string cuzdanSeriField;
        
        private int cuzdanNoField;
        
        /// <remarks/>
        public string CuzdanSeri {
            get {
                return this.cuzdanSeriField;
            }
            set {
                this.cuzdanSeriField = value;
            }
        }
        
        /// <remarks/>
        public int CuzdanNo {
            get {
                return this.cuzdanNoField;
            }
            set {
                this.cuzdanNoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class AdresNoG : KPSSorgusu {
        
        private long adresNoField;
        
        /// <remarks/>
        public long AdresNo {
            get {
                return this.adresNoField;
            }
            set {
                this.adresNoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class AileListesiTCKimlikNoG : KPSSorgusu {
        
        private long tCKimlikNoField;
        
        private AileListesiTipi listeTipiField;
        
        private int eskiEsListeleField;
        
        /// <remarks/>
        public long TCKimlikNo {
            get {
                return this.tCKimlikNoField;
            }
            set {
                this.tCKimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public AileListesiTipi ListeTipi {
            get {
                return this.listeTipiField;
            }
            set {
                this.listeTipiField = value;
            }
        }
        
        /// <remarks/>
        public int EskiEsListele {
            get {
                return this.eskiEsListeleField;
            }
            set {
                this.eskiEsListeleField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public enum AileListesiTipi {
        
        /// <remarks/>
        AileListesi,
        
        /// <remarks/>
        TumAileListesi,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class EvlenmeG : KPSSorgusu {
        
        private KisiKayitYeriKodBilgisi kayitYeriField;
        
        /// <remarks/>
        public KisiKayitYeriKodBilgisi KayitYeri {
            get {
                return this.kayitYeriField;
            }
            set {
                this.kayitYeriField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class NKOTCKimlikNoG : KPSSorgusu {
        
        private long tCKimlikNoField;
        
        private NufusKayitOrnegiTipi nKOTipiField;
        
        private int vukuatliField;
        
        private int eskiEsListeleField;
        
        /// <remarks/>
        public long TCKimlikNo {
            get {
                return this.tCKimlikNoField;
            }
            set {
                this.tCKimlikNoField = value;
            }
        }
        
        /// <remarks/>
        public NufusKayitOrnegiTipi NKOTipi {
            get {
                return this.nKOTipiField;
            }
            set {
                this.nKOTipiField = value;
            }
        }
        
        /// <remarks/>
        public int Vukuatli {
            get {
                return this.vukuatliField;
            }
            set {
                this.vukuatliField = value;
            }
        }
        
        /// <remarks/>
        public int EskiEsListele {
            get {
                return this.eskiEsListeleField;
            }
            set {
                this.eskiEsListeleField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public enum NufusKayitOrnegiTipi {
        
        /// <remarks/>
        KisiKayitOrnegi,
        
        /// <remarks/>
        AileKayitOrnegi,
        
        /// <remarks/>
        NufusAileKayitOrnegi,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class KimlikBilgisiG : KPSSorgusu {
        
        private string adField;
        
        private string soyadField;
        
        private CinsiyetTipi cinsiyetField;
        
        private string babaAdField;
        
        private string anaAdField;
        
        private TarihBilgisi dogumTarihField;
        
        private string dogumYerField;
        
        private KimlikBilgisiSorguTipi sorguTipiField;
        
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
        public CinsiyetTipi Cinsiyet {
            get {
                return this.cinsiyetField;
            }
            set {
                this.cinsiyetField = value;
            }
        }
        
        /// <remarks/>
        public string BabaAd {
            get {
                return this.babaAdField;
            }
            set {
                this.babaAdField = value;
            }
        }
        
        /// <remarks/>
        public string AnaAd {
            get {
                return this.anaAdField;
            }
            set {
                this.anaAdField = value;
            }
        }
        
        /// <remarks/>
        public TarihBilgisi DogumTarih {
            get {
                return this.dogumTarihField;
            }
            set {
                this.dogumTarihField = value;
            }
        }
        
        /// <remarks/>
        public string DogumYer {
            get {
                return this.dogumYerField;
            }
            set {
                this.dogumYerField = value;
            }
        }
        
        /// <remarks/>
        public KimlikBilgisiSorguTipi SorguTipi {
            get {
                return this.sorguTipiField;
            }
            set {
                this.sorguTipiField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public enum KimlikBilgisiSorguTipi {
        
        /// <remarks/>
        TamEsitlik,
        
        /// <remarks/>
        KelimeIcermesi,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class KayitYerindenKimlikBilgisiG : KPSSorgusu {
        
        private int ilceKodField;
        
        private int ciltKodField;
        
        private int aileSiraNoField;
        
        private int bireySiraNoField;
        
        /// <remarks/>
        public int IlceKod {
            get {
                return this.ilceKodField;
            }
            set {
                this.ilceKodField = value;
            }
        }
        
        /// <remarks/>
        public int CiltKod {
            get {
                return this.ciltKodField;
            }
            set {
                this.ciltKodField = value;
            }
        }
        
        /// <remarks/>
        public int AileSiraNo {
            get {
                return this.aileSiraNoField;
            }
            set {
                this.aileSiraNoField = value;
            }
        }
        
        /// <remarks/>
        public int BireySiraNo {
            get {
                return this.bireySiraNoField;
            }
            set {
                this.bireySiraNoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public partial class NKOKayitYeriG : KPSSorgusu {
        
        private KisiKayitYeriKodBilgisi kayitYeriField;
        
        private NufusKayitOrnegiTipi nKOTipiField;
        
        private int vukuatliField;
        
        private int eskiEsListeleField;
        
        /// <remarks/>
        public KisiKayitYeriKodBilgisi KayitYeri {
            get {
                return this.kayitYeriField;
            }
            set {
                this.kayitYeriField = value;
            }
        }
        
        /// <remarks/>
        public NufusKayitOrnegiTipi NKOTipi {
            get {
                return this.nKOTipiField;
            }
            set {
                this.nKOTipiField = value;
            }
        }
        
        /// <remarks/>
        public int Vukuatli {
            get {
                return this.vukuatliField;
            }
            set {
                this.vukuatliField = value;
            }
        }
        
        /// <remarks/>
        public int EskiEsListele {
            get {
                return this.eskiEsListeleField;
            }
            set {
                this.eskiEsListeleField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://kps.nvi.gov.tr/WS")]
    public enum NufusOlayTipi {
        
        /// <remarks/>
        DolayliSoybagiDuzeltmeSilme,
        
        /// <remarks/>
        SoybagiDuzeltme,
        
        /// <remarks/>
        SoyBagininReddi,
        
        /// <remarks/>
        EvlatEdinme,
        
        /// <remarks/>
        EvlatliginReddi,
        
        /// <remarks/>
        Bosanma,
        
        /// <remarks/>
        BosanmaIptali,
        
        /// <remarks/>
        KayitDuzeltme,
        
        /// <remarks/>
        Evlenme,
        
        /// <remarks/>
        EvlenmeninFeshiIptali,
        
        /// <remarks/>
        Gaiplik,
        
        /// <remarks/>
        IdareceKayitDuzeltme,
        
        /// <remarks/>
        KayitSilme,
        
        /// <remarks/>
        KayitTasima,
        
        /// <remarks/>
        MukerrerKayitSilme,
        
        /// <remarks/>
        Nakil,
        
        /// <remarks/>
        SinifsizOlaylar,
        
        /// <remarks/>
        SosyalGuvenlik,
        
        /// <remarks/>
        Tanima,
        
        /// <remarks/>
        Vasiyet,
        
        /// <remarks/>
        VatandasligaAlma,
        
        /// <remarks/>
        Velayet,
        
        /// <remarks/>
        YesilKart,
        
        /// <remarks/>
        Dogum,
        
        /// <remarks/>
        Olum,
        
        /// <remarks/>
        GaipliginFeshi,
        
        /// <remarks/>
        OlumunFeshi,
        
        /// <remarks/>
        SosyalGuvenlikIptali,
        
        /// <remarks/>
        TanimaninIptali,
        
        /// <remarks/>
        VasiyetIptali,
        
        /// <remarks/>
        MirastanFeragat,
        
        /// <remarks/>
        OlunceyeKadarBakimSozlesmesi,
        
        /// <remarks/>
        MirastanIsgat,
        
        /// <remarks/>
        VatandasliktanCikma,
        
        /// <remarks/>
        YenidenKayit,
        
        /// <remarks/>
        CiftUyrukluluk,
        
        /// <remarks/>
        VatandaslikIptali,
    }
}

#pragma warning restore 1591
