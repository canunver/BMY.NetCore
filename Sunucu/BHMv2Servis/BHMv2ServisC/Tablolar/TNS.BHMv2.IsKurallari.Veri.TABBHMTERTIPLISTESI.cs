using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BKD;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Tertip tanım tablolarıyla (GİDER, GELİR, FİNANSMAN tertip tanımları) ilgili işlemlerin tanımlarını tutan soyut sınıf.
    /// Bu sayede tertip tanımlarıyla ilgili işlemler tek bir noktadan yönetilir.
    /// </summary>
    public abstract class TABBHMTERTIPLISTESI : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMTERTIPLISTESI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Verilen tertip nesnesi veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="tertip">Tertip yıl ve kodlarını tutan, Tertip yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public abstract Sonuc Sil(Tertip tertip);

        /// <summary>
        /// Tertip yapısında tutulan yıl, kod ve tertip açıklama bilgilerini kullanarak, tertip satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="tertip">Tertip yıl ve kodlarını tutan, Tertip yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public abstract Sonuc Ekle(Tertip tertip);

        /// <summary>
        /// Tertip yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="tertip">Listelenecek tertiplerin yıl ve kod kıstaslarını tutan, Tertip yapısında nesne</param>
        /// <param name="siralamaTuru">Parametre değeri, 1 ise ekonomik kod, 2 ise kurkd ve ekonomik koda göre sıralama yapılır.</param>
        /// <param name="tertipDurumlar">Listelecek tertiplerin uyacağı durumların listesi. Tertip bu durumlardan birinde ise döndürülür.</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public abstract IDataReader Listele(Tertip tertip, int siralamaTuru, ENUMTertipDurum[] tertipDurumlar);

        /// <summary>
        /// Verilen tertibin, ilgili yılda, kaç kez kullanıldığını bulan
        /// select sql cümlesini oluşturup, çalıştıran ve bulduğu sayısı döndüren yordam.
        /// </summary>
        /// <param name="tertip">Kullanım sayısı bulunacak tertip.</param>
        /// <returns>Tertibin kaç satırda yer aldığı bilgisi.</returns>
        public abstract int KayitSayisiBul(Tertip tertip);

        public abstract int TertipSayisiBul(Tertip tertip);

        public abstract Sonuc TertipTanimKopyalaOncYil(Tertip tertip);

        public abstract Sonuc OdenekAlacaklarKopyala(Tertip tertip);

        /// <summary>
        /// Tertip tablosundan; verilen alanlar, verilen kistas tertibe göre seçilir ve döndürülür.
        /// </summary>
        /// <param name="alanlar">seçilecek alan listesi</param>
        /// <param name="kistasTertip">Seçilecek tertiplerin uyacağı kriteri tutan tertip nesnesi.</param>
        /// <param name="alansay">Seçilecek kaç alan olduğunu tutan değişken</param>
        /// <returns>Oluşturulan sql'in çalıştırılmasıyla elde edilien data reader döndürülür.</returns>
        public abstract IDataReader FarkliTertipListele(string alanlar, Tertip kistasTertip, int alansay);
    }
}
