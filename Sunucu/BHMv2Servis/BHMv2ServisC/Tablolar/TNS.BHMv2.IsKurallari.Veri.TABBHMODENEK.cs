using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BKD;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Ödenek tablolarıyla (GİDER, GELİR, FİNANSMAN Ödenekleri) ilgili işlemlerin tanımlarını tutan soyut sınıf.
    /// Bu sayede ödenek tablolarıyla ilgili işlemler tek bir noktadan yönetilir.
    /// </summary>
    public abstract class TABBHMODENEK : TABOrtak
    {
        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMODENEK(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Verilen ödenek bilgisini; ilgili tertibin yılı, senaryo ve teklif yılına ekleyen
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
        /// Parametre olarak verilen kıstasları kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam
        /// </summary>
        /// <param name="yil">Listelenecek kayıtların yıl bilgisi</param>
        /// <param name="senaryoKod">Listelenecek kayıtların senaryo kodu</param>
        /// <param name="teklifYil">Ödenekler çok yıllı teklif edildiğinden, verilen yılda hangi çok yıllı ödeneğin listeleneceği bilgisi (-1 ise hepsi, 0 ise ilk yıl, 1 ise ikinci yıl ...)</param>
        /// <param name="kodlar">Listelenecek tertiplerin kıstasını tutan, tertip boyutları dizisi</param>
        /// <returns>Select komutunun çalıştırılmasının sonucunda elde edilen DataReader nesnesini döndürür.</returns>
        public abstract IDataReader TertipListesi(int yil, string senaryoKod, int teklifYil, string[] kodlar);

        /// <summary>
        /// Parametre olarak verilen kıstasları kullanarak, bir tertibe ait çok yıllı ödenekleri silen, 
        /// delete sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="yil">Silinecek kaydın yıl bilgisi</param>
        /// <param name="senaryoKod">Silinecek kaydın senaryo kodu</param>
        /// <param name="tertip">Silinecek tertibin bilgisini tutan Tertip nesnesi</param>
        /// <returns>Silme komutunun çalıştırılmasının sonucu, Sonuc yapısında döndürür</returns>
        public abstract Sonuc CokYilliOdenekSil(int yil, string senaryoKod, Tertip tertip);

        /// <summary>
        /// Ödenekle ilgili bir yıl, senaryo kod ve kurumsal kod bazında yapılan son değişiklik zamanını
        /// verilen zaman tablosu kullanılarak kaydeden yordam. 
        /// Kaydedilen zaman sayesinde, Rapor alınırken, raporun üretim zamanı 
        /// ve verinin son değiştiği tarih kontrol edilerek yeni rapor üretilip üretilmeyeceğine karar verilir.
        /// </summary>
        /// <param name="tabZaman">Değişim zamanı bilgisini kaydetmek için kullanılacak table nesnesi</param>
        /// <param name="yil">Değişim zamanının yıl bilgisi. Her yıl için ayrı değişim zamanı tutulur.</param>
        /// <param name="senaryoKod">Değişim zamanının senaryo bilgisi. Her senaryo için ayrı değişim zamanı tutulur.</param>
        /// <param name="kurKod">Değişim zamanının kurumsal kod bilgisi. Her kurumsal kod için ayrı değişim zamanı tutulur.</param>
        /// <param name="kullaniciKod">Değişimi yapan kullanıcının kodu</param>
        /// <returns>Değişim kayıt işleminin sonucu, Sonuc yapısında döndürülür.</returns>
        public abstract Sonuc DegisimZamaniAyarla(TABDEGISIMZAMAN tabZaman, int yil, string senaryoKod, string kurKod, string kullaniciKod);

        public abstract Sonuc OdenekKopyala(Tertip hedefTertip, string hedefAsama, int kaynakYil, string kaynakAsama);

        public abstract Sonuc CokYilliOdenekSilAciklama(int yil, string senaryoKod, Tertip tertip);

        public abstract Sonuc EkleAciklama(int yil, string senaryoKod, int i, Tertip tertip, string aciklama);
    }
}
