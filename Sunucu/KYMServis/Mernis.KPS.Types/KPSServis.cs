
namespace Mernis.KPS.Types
{
    public abstract class KPSServis
    {
        public static Mernis.KPS.Types.KPSWse.KPS Create()
        {
            Mernis.KPS.Types.KPSWse.KPS service = new Mernis.KPS.Types.KPSWse.KPS();

            string proxyURL = System.Configuration.ConfigurationManager.AppSettings.Get("MernisProxyURL") + "";
            string proxyUser = System.Configuration.ConfigurationManager.AppSettings.Get("MernisProxyUser") + "";
            string proxyPassword = System.Configuration.ConfigurationManager.AppSettings.Get("MernisProxyPassword") + "";

            System.Net.WebProxy proxy = new System.Net.WebProxy();
            if (proxyURL.Trim() != "")
            {
                proxy = new System.Net.WebProxy(proxyURL);
                proxy.Credentials = new System.Net.NetworkCredential(proxyUser, proxyPassword);
            }

            service.Url = System.Configuration.ConfigurationManager.AppSettings.Get("MernisURL") + "";
            string kullanici = System.Configuration.ConfigurationManager.AppSettings.Get("MernisUser") + "";
            string parola = System.Configuration.ConfigurationManager.AppSettings.Get("MernisPassword") + "";

            // WSE 2.0 UsernameToken (PasswordOption.SendHashed) karşılığı: SoapIstemci parola özetli güvenlik başlığını ekler.
            service.KullaniciAdi = kullanici;
            service.Parola = parola;
            service.ParolaOzetiGonder = true;
            return service;
        }

    }

}
