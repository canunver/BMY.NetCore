//------------------------------------------------------------------------------
// TedasFatura web servisi istemcisi.
//
// Bu dosya, Visual Studio'nun .NET Framework için ürettiği eski Reference.cs dosyasından
// _araclar/asmx_istemci_uret.py ile dönüştürülmüştür. Proxy sınıfı TNS.Soap.SoapIstemci tabanlıdır
// (HttpClient + XmlSerializer); veri sınıfları eski üretilmiş koddan değiştirilmeden alınmıştır.
//------------------------------------------------------------------------------
#pragma warning disable 1591
namespace HRCServisC.tr.gov.sgb.uygulama.tedas {
    using System;
    using System.Xml.Serialization;
    using TNS.Soap;

    public partial class TedasFatura : SoapIstemci {

        public TedasFatura() {
            this.Url = global::HRCServisC.Properties.Settings.Default.HRCServisC_tr_gov_sgb_uygulama_tedas_TedasFatura;
        }

        public System.Xml.XmlNode FaturaSorgula(int ilKodu, int ilBolgeKodu, int ilceKodu, int kasabaKodu, int koyKodu, string aboneNo, string donem) {
            return (System.Xml.XmlNode)Cagir("http://uygulama.sgb.gov.tr/SGBServis/FaturaSorgula", "FaturaSorgula", "http://uygulama.sgb.gov.tr/SGBServis/", typeof(System.Xml.XmlNode), "FaturaSorgulaResult",
                new SoapParametre("ilKodu", ilKodu, typeof(int)),
                new SoapParametre("ilBolgeKodu", ilBolgeKodu, typeof(int)),
                new SoapParametre("ilceKodu", ilceKodu, typeof(int)),
                new SoapParametre("kasabaKodu", kasabaKodu, typeof(int)),
                new SoapParametre("koyKodu", koyKodu, typeof(int)),
                new SoapParametre("aboneNo", aboneNo, typeof(string)),
                new SoapParametre("donem", donem, typeof(string)));
        }

    }

}

#pragma warning restore 1591
