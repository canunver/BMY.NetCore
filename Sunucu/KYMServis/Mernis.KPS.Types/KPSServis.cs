using Microsoft.Web.Services2.Security.Tokens;

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

            UsernameToken userName = new UsernameToken(kullanici, parola, PasswordOption.SendHashed);
            service.RequestSoapContext.Security.Tokens.Add(userName);
            return service;
        }

    }

}
