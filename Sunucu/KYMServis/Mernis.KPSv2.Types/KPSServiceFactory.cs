using System;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Federation;

namespace Mernis.KPSv2.Types
{
    /// <summary>
    /// KPS v2 servislerine bağlanan WCF kanallarını üretir.
    ///
    /// Kimlik doğrulama akışı (eski app.config içindeki system.serviceModel bölümünün kod karşılığı):
    ///   1. Kullanıcı adı / parola ile NVİ kimlik doğrulama servisinden (STS, WS-Trust 1.3) simetrik anahtarlı jeton alınır.
    ///   2. Bu jeton ile KPS servisi (RoutingService.svc) çağrılır (IssuedTokenOverTransport).
    ///
    /// .NET Framework sürümünde bu iş Windows Identity Foundation (Microsoft.IdentityModel) ile elle yapılıyordu;
    /// .NET 8'de System.ServiceModel.Federation paketi jetonu kendisi alır, saklar ve süresi dolunca yeniler.
    /// </summary>
    public class KPSServiceFactory
    {
        /// <summary>NVİ kimlik doğrulama (jeton) servisi adresi.</summary>
        public const string IssuerAddress = "https://kimlikdogrulama.nvi.gov.tr/Services/Issuer.svc/IWSTrust13";

        private static KPSServiceFactory instance;
        private static readonly ConcurrentDictionary<string, ChannelFactory> fabrikalar = new ConcurrentDictionary<string, ChannelFactory>();

        private KPSServiceFactory() { }

        public static KPSServiceFactory Instance
        {
            get
            {
                if (instance == null)
                    instance = new KPSServiceFactory();
                return instance;
            }
        }

        /// <summary>Jeton servisine (STS) bağlanırken kullanılan bağlama: eski "stsIssuerServiceBinding".</summary>
        public Binding CreateIssuerBinding()
        {
            WS2007HttpBinding b = new WS2007HttpBinding(SecurityMode.TransportWithMessageCredential);
            b.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            b.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            b.Security.Message.EstablishSecurityContext = false;
            return b;
        }

        /// <summary>KPS servislerine bağlanırken kullanılan bağlama: eski "CustomBinding_..." tanımları.</summary>
        public Binding CreateServiceBinding()
        {
            WSTrustTokenParameters jeton = WSTrustTokenParameters.CreateWS2007FederationTokenParameters(
                CreateIssuerBinding(), new EndpointAddress(IssuerAddress));
            jeton.KeyType = SecurityKeyType.SymmetricKey;
            jeton.CacheIssuedTokens = true;

            WSFederationHttpBinding b = new WSFederationHttpBinding(jeton);
            b.MaxReceivedMessageSize = 1024 * 1024;
            b.SendTimeout = TimeSpan.FromMinutes(1);
            return b;
        }

        /// <summary>
        /// Verilen servis sözleşmesi için kanal üretir. Kanal fabrikası (ve onunla birlikte alınmış jeton)
        /// sözleşme + adres + kullanıcı başına bir kez oluşturulup saklanır.
        /// </summary>
        public TChannel CreateChannel<TChannel>()
        {
            KPSConfiguration ayar = KPSConfiguration.Instance;
            string anahtar = typeof(TChannel).FullName + "|" + ayar.EndPoint + "|" + ayar.Username + "|" + (ayar.Password ?? "").GetHashCode();

            ChannelFactory<TChannel> fabrika = (ChannelFactory<TChannel>)fabrikalar.GetOrAdd(anahtar, delegate(string a)
            {
                ChannelFactory<TChannel> f = new ChannelFactory<TChannel>(CreateServiceBinding(), new EndpointAddress(ayar.EndPoint));
                f.Credentials.UserName.UserName = ayar.Username;
                f.Credentials.UserName.Password = ayar.Password;
                return f;
            });

            return fabrika.CreateChannel();
        }
    }
}
