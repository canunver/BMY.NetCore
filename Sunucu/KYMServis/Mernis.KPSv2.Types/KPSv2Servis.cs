using Mernis.KPSv2.Types.AksIlceyeBagliBucakVeKoyServis;
using Mernis.KPSv2.Types.AksOkuBucakServis;
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
            return KPSServiceFactory.Instance.CreateChannel<BilesikKutukSorgulaKimlikNoServis.BilesikKutukSorgulaKimlikNoServis>();
        }

        public static KimlikNoSorgulaAdresServis.KimlikNoSorgulaAdresServis KimlikNoSorgulaAdresServisOlustur()
        {
            return KPSServiceFactory.Instance.CreateChannel<KimlikNoSorgulaAdresServis.KimlikNoSorgulaAdresServis>();
        }

        public static AksOkuBucakServis.AksOkuBucakServis AksOkuBucakServisOlustur()
        {
            return KPSServiceFactory.Instance.CreateChannel<AksOkuBucakServisChannel>();
        }

        public static AksIlceyeBagliBucakVeKoyServis.AksIlceyeBagliBucakVeKoyServis AksIlceyeBagliBucakVeKoyServisOlustur()
        {
            return KPSServiceFactory.Instance.CreateChannel<AksIlceyeBagliBucakVeKoyServisChannel>();
        }

        public static AksOkuTumIlceyeBagliMahalleServis.AksOkuTumIlceyeBagliMahalleServis AksOkuTumIlceyeBagliMahalleServisOlustur()
        {
            return KPSServiceFactory.Instance.CreateChannel<AksOkuTumIlceyeBagliMahalleServisChannel>();
        }
    }
}
