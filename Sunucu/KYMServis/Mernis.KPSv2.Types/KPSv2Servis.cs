using System.ServiceModel;
using Mernis.KPSv2.Types.AksOkuBucakServis;
using Mernis.KPSv2.Types.BilesikKutukSorgulaKimlikNoServis;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Mernis.KPSv2.Types.AksIlceyeBagliBucakVeKoyServis;
using Mernis.KPSv2.Types.AksOkuTumIlceyeBagliMahalleServis;

namespace Mernis.KPSv2.Types
{
    public class KPSv2Servis
    {
        public KPSv2Servis()
        {
            KPSConfiguration.Instance.EndPoint = "https://kpsv2.nvi.gov.tr/Services/RoutingService.svc";
        }

        public static BilesikKutukSorgulaKimlikNoServis.BilesikKutukSorgulaKimlikNoServis BilesikKutukSorgulaTCKimlikNoServisOlustur()
        {
            ChannelFactory<BilesikKutukSorgulaKimlikNoServis.BilesikKutukSorgulaKimlikNoServis> channelFactory = new ChannelFactory<BilesikKutukSorgulaKimlikNoServis.BilesikKutukSorgulaKimlikNoServis>("CustomBinding_BilesikKutukSorgulaKimlikNoServis", new EndpointAddress(KPSConfiguration.Instance.EndPoint));
            channelFactory.Credentials.SupportInteractive = false;
            channelFactory.ConfigureChannelFactory();
            return channelFactory.CreateChannelWithIssuedToken(KPSServiceFactory.Instance.CreateToken());
        }

        public static KimlikNoSorgulaAdresServis.KimlikNoSorgulaAdresServis KimlikNoSorgulaAdresServisOlustur()
        {
            ChannelFactory<KimlikNoSorgulaAdresServis.KimlikNoSorgulaAdresServis> channelFactory = new ChannelFactory<KimlikNoSorgulaAdresServis.KimlikNoSorgulaAdresServis>("CustomBinding_KimlikNoSorgulaAdresServis", new EndpointAddress(KPSConfiguration.Instance.EndPoint));
            channelFactory.Credentials.SupportInteractive = false;
            channelFactory.ConfigureChannelFactory();
            return channelFactory.CreateChannelWithIssuedToken(KPSServiceFactory.Instance.CreateToken());
        }

        public static AksOkuBucakServis.AksOkuBucakServis AksOkuBucakServisOlustur()
        {
            ChannelFactory<AksOkuBucakServisChannel> channelFactory = new ChannelFactory<AksOkuBucakServisChannel>("CustomBinding_AksOkuBucakServis", new EndpointAddress(KPSConfiguration.Instance.EndPoint));
            channelFactory.Credentials.SupportInteractive = false;
            channelFactory.ConfigureChannelFactory();
            return channelFactory.CreateChannelWithIssuedToken(KPSServiceFactory.Instance.CreateToken());
        }

        public static AksIlceyeBagliBucakVeKoyServis.AksIlceyeBagliBucakVeKoyServis AksIlceyeBagliBucakVeKoyServisOlustur()
        {
            ChannelFactory<AksIlceyeBagliBucakVeKoyServisChannel> channelFactory = new ChannelFactory<AksIlceyeBagliBucakVeKoyServisChannel>("CustomBinding_AksIlceyeBagliBucakVeKoyServis", new EndpointAddress(KPSConfiguration.Instance.EndPoint));
            channelFactory.Credentials.SupportInteractive = false;
            channelFactory.ConfigureChannelFactory();
            return channelFactory.CreateChannelWithIssuedToken(KPSServiceFactory.Instance.CreateToken());
        }

        public static AksOkuTumIlceyeBagliMahalleServis.AksOkuTumIlceyeBagliMahalleServis AksOkuTumIlceyeBagliMahalleServisOlustur()
        {
            ChannelFactory<AksOkuTumIlceyeBagliMahalleServisChannel> channelFactory = new ChannelFactory<AksOkuTumIlceyeBagliMahalleServisChannel>("CustomBinding_AksOkuTumIlceyeBagliMahalleServis", new EndpointAddress(KPSConfiguration.Instance.EndPoint));
            channelFactory.Credentials.SupportInteractive = false;
            channelFactory.ConfigureChannelFactory();
            return channelFactory.CreateChannelWithIssuedToken(KPSServiceFactory.Instance.CreateToken());
        }
    }
}
