using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Yıl sonu gerçekleşme tahmini tablolarıyla (GİDER, GELİR, FİNANSMAN gerçekleşmeleri) ilgili işlemlerin tanımlarını tutan soyut sınıf.
    /// Bu sayede gerçekleşme tahmini tablolarıyla ilgili işlemler tek bir noktadan yönetilir.
    /// </summary>
    public abstract class TABBHMTAHMIN : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMTAHMIN(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Verilen gerçekleşme tahmini bilgisini; ilgili tertibin yılı, senaryo ve teklif yılına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Eklenecek ödenek bilgisinin yıl parametresi</param>
        /// <param name="senaryoKod">Eklenecek ödenek bilgisinin ay parametresi</param>
        /// <param name="teklifYil">Ödenekler çok yıllı teklif edildiğinden, verilen yılda hangi çok yıllı ödeneğe ait kaydın ekleneceği bilgisi</param>
        /// <param name="tertip">Eklenecek ödenek bilgisinin tertibini tutan, Tertip yapısında nesne</param>
        /// <param name="odenek">Eklenecek ödeneğin miktarını tutan parametre</param>
        /// <returns>Ekleme komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür</returns>
        public abstract Sonuc Ekle(int yil, string senaryoKod, int teklifYil, Tertip tertip, double odenek);

        /// <summary>
        /// Parametre olarak verilen kıstasları kullanarak, bir tertibe ait çok yıllı gerçekleşme tahminlerini silen, 
        /// delete sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="yil">Silinecek kaydın yıl bilgisi</param>
        /// <param name="senaryoKod">Silinecek kaydın senaryo kodu</param>
        /// <param name="tertip">Silinecek tertibin bilgisini tutan Tertip nesnesi</param>
        /// <returns>Silme komutunun çalıştırılmasının sonucu, Sonuc yapısında döndürür</returns>
        public abstract Sonuc CokYilliOdenekSil(int yil, string senaryoKod, Tertip tertip);

        public abstract IDataReader Listele(string senaryoKod, Tertip kistas);

        public abstract Sonuc CokYilliOdenekSilAcik(int yil, string senaryoKod, Tertip tertip);

        public abstract Sonuc EkleAcik(int yil, string senaryoKod, int teklifYil, Tertip tertip, string aciklama);
    }
}
