using System.Data;
using TNS.Veri;
using TNS.BKD;


namespace TNS.BUM.IsKurallari.Veri
{
    /// <summary>
    /// Bütçe işlemleri ile ilgili olarak Tablo işlemlerini tutan, Bütçe Sınıfı 
    /// </summary>
    public abstract class TABButce : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABButce(VeriErisim veriErisim) : base(veriErisim) { }
        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kullanarak,
        /// Kesintili Başlangıç Ödeneği(KBÖ) toplamını YSOTBUL rutinini çağırarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasKodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kesintili Başlangıç Ödeneğini tutan Double türünde nesne döndürür</returns>
        public abstract double KBOBul(int yil, string[] kistasKodlar);
        /// <summary>
        /// Parametre olarak verilen Yıl, Kodlar bilgisini kıstas olarak kullanarak,
        /// verilen kıstasa uygun Yıl Sonu Ödenek Toplamını döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasKodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="kboBul">Kesintili Başlangıç Ödeneği ile ilgili Sql cümlesinin değişip değişmeyeceğini tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Yıl Sonu Ödenek Toplamını Tutan Double türünde nesne döndürür</returns>
        public abstract double YSOTBul(int yil, string[] kistasKodlar, bool kboBul);
        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Serbest toplamını döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasKodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Yıl Sonu Ödenek Toplamını Tutan Double türünde nesne döndürür</returns>
        public abstract double SerbestBul(int yil, string[] kistasKodlar);
        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisine göre,
        /// verilen kıstaslara uygun Kanun, KBÖ, Eklenek, Düşülen, Serbest ve Taslakları döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasKodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="kanun">Kanun bilgisini tutan, Double türünde nesne</param>
        /// <param name="kbo">Kesintili Başlangıç Ödeneği bilgisini tutan, Double türünde nesne</param>
        /// <param name="eklenen">Eklenen bilgisini tutan, Double türünde nesne</param>
        /// <param name="dusulen">Düşülen bilgisini tutan, Double türünde nesne</param>
        /// <param name="serbest">Serbest bilgisini tutan, Double türünde nesne</param>
        /// <param name="eklenen_taslak">Eklenen Taslak bilgisini tutan, Double türünde nesne</param>
        /// <param name="dusulen_taslak">Düşülen Taslak bilgisini tutan, Double türünde nesne</param>
        /// <param name="serbest_taslak">Serbest Taslak bilgisini tutan, Double türünde nesne</param>
        public abstract void BilgiBul(int yil, string[] kistasKodlar, out double kanun, out double kbo,
            out double eklenen, out double dusulen, out double serbest,
            out double eklenen_taslak, out double dusulen_taslak, out double serbest_taslak);
        /// <summary>
        /// Parametre olarak verien Yıl ve Kodlar bilgisini kıstas olarak kullanarak,
        /// Bütçe Tertip bilgilerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="kbolu">KBO parametresine göre ilgili koşulun eklenip eklenmeyeceği bilgisini tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Bütçe Tertip bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public abstract IDataReader ButceTertipListele(int yil, string[] kodlar, bool kbolu);
        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kullanarak,
        /// veri tabanından eklenen, düşülen ve serbest miktarlarını güncelleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="eklenen">Eklenen bilgisini tutan, Double türünde nesne</param>
        /// <param name="dusulen">Düşülen bilgisini tutan, Double türünde nesne</param>
        /// <param name="serbest">Serbest bilgisini tutan, Double türünde nesne</param>
        /// <param name="taslakMi">Taslak mı parametresine göre Eklenen, Düşülen, Serbest değerlerinin mi yoksa taslaklarının mı güncelleneceği bilgisini tutan, Bool türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public abstract Sonuc TaslakDegistir(int yil, string[] kodlar, double eklenen, double dusulen, double serbest, bool taslakMi);
        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kullanarak,
        /// Eklenen, Düşülen, Serbest ve Taslaklar miktarlarını veri tabanında güncelleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="eklenen">Eklenen bilgisini tutan, Double türünde nesne</param>
        /// <param name="dusulen">Düşülen bilgisini tutan, Double türünde nesne</param>
        /// <param name="serbest">Serbest bilgisini tutan, Double türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public abstract Sonuc TaslaktanOnaya(int yil, string[] kodlar, double eklenen, double dusulen, double serbest);
        /// <summary>
        /// Parametre olarak verilen AHP Serbest Ay, Yıl ve Kodlar bilgisini kullanarak
        /// Serbest miktarını veri tabanında güncelleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="ahpSerbestAy">AHP Serbest Ay bilgisini tutan, Int türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasKodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public abstract Sonuc SerbestleriHesapla(int ahpSerbestAy, int yil, string[] kistasKodlar);
        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kıstas olarak kullanarak,
        /// veri tabanından Tertip Listesini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="ekKosul">Sql cümlesine eklenecek ek koşul ifadesini tutan, String türünde nesne</param>
        /// <param name="kurYapi">Kurumsal Yapı bilgisini tutan, String türünde nesne</param>
        /// <param name="hiyerarsik">Hiyerarşi bilgisinin uygulanıp uygulanmayacağı bilgisini tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Tertip Listesini tutan IDataReader türünde nesne döndürür</returns>
        public abstract IDataReader TertipListesiBul(int yil, string[] kodlar, string ekKosul, string kurYapi, bool hiyerarsik);
        /// <summary>
        /// Verilen Bütçe Bilgierini veri tabanına kaydeden,
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="kanun">Kanun bilgisini tutan, Double türünde nesne</param>
        /// <param name="kbo">KBÖ bilgisini tutan, Double türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public abstract Sonuc Kaydet(int yil, string[] kodlar, double kanun, double kbo);
        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kullanarak,
        /// İlgili Bütçe bilgilerini veri tabanından silen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public abstract Sonuc Sil(int yil, string[] kodlar);
        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kullanarak,
        /// veri tabanına yeni bilgileri ekleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public abstract Sonuc IslemdekiYenileriEkle(int yil, string[] kodlar);
        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kullanarak,
        /// Serbest miktarları veri tabanından güncelleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="serbest">Serbest bilgisini tutan, Double türünde nesne</param>
        /// <param name="serbestTaslak">Serbest Taslak bilgisini tutan, Double türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public abstract Sonuc SerbestleriYaz(int yil, string[] kodlar, double serbest, double serbestTaslak);
    }

    /// <summary>
    /// Ayrıntılı Programla ilgili Tablo işlemlerini tutan, Ayrıntılı Program Sınıfı
    /// </summary>
    public abstract class TABAyrintiliProgram : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAyrintiliProgram(VeriErisim veriErisim) : base(veriErisim) { }
        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem kodu, Sıra No, Durum bilgilerine göre
        /// Ayrıntılı Program bilgilerini veri tabanında güncelleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <param name="durum">Durum bilgisini tutan, Int türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public abstract Sonuc OnayKaydet(int yil, string kurum, string islemKod, int siraNo, int durum);
        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem kodu, Sıra no ve Durum bilgilerine göre,
        /// Ayrıntılı Program bilgilerini veri tabanında güncelleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <param name="durum">Durum bilgisini tutan, Int türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public abstract Sonuc EskiOnaylariDegistir(int yil, string kurum, string islemKod, int siraNo, int durum);
        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kıstas olarak kullanarak,
        /// Tertiplere ilişkin durum bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl  bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal ve Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="onayDurum">Durum bilgisini tutan, ENUMFormDurumGrup türünde nesne</param>
        /// <param name="sadeceRevizeler">Revize bilgisine göre koşul eklenip eklenmeyeceği bilgisini tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Tertip Durum bilgilerini (Ay, Ödenek) tutan IDataReader türünde nesne döndürür</returns>
        public abstract IDataReader TertipDurum(int yil, string[] kodlar, ENUMFormDurumGrup onayDurum, bool sadeceRevizeler);
        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisine göre,
        /// Revize olmayan Onaylı Tertip durumlarını TertipDurum rutinini çalıştırarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurum, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="onayDurum">Durum bilgisini tutan, ENUMFormDurumGrup türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Tertip Durum bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public abstract IDataReader TertipDurum(int yil, string[] kodlar, ENUMFormDurumGrup onayDurum);
        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisine göre,
        /// Onaylı Tertip durumlarını TertipDurum rutinini çalıştırarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurum, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Tertip Durum bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public abstract IDataReader TertipDurum(int yil, string[] kodlar);
        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem, Sıra no bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Ayrıntılı Program bilgilerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Ayrıntılı Program bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public abstract IDataReader Listele(int yil, string kurum, string islem, int siraNo);
        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem, Aşama ve Kodlar bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Aylık Ödenek bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal ve Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Aylık Ödenek bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public abstract IDataReader AylikOdenek(int yil, string kurum, string islem, int asama, string[] kodlar);
        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem, Aşama ve Kodlar bilgisini kıstas olarak kullanarak,
        /// Kıstasa uygun Ayrıntılı Program kayıt sayısını döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kodlar bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Ayrıntılı Program kayıt sayısını tutan Double türünde nesne döndürür</returns>
        public abstract double Kontrol(int yil, string kurum, string islem, int asama, string[] kodlar);
        /// <summary>
        /// Parametre olarak verilen Ayrıntılı Program bilgilerini veri tabanına kaydeden,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kodlar bilgisini tutan, String dizi türünde nesne</param>
        /// <param name="ay">Ay bilgisini tutan, Int türünde nesne</param>
        /// <param name="odenek">Ödenek bilgisini tutan, Double türünde nesne</param>
        /// <param name="durum">Durum bilgisini tutan, Int türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public abstract Sonuc Kaydet(int yil, string kurum, string islem, int asama, string[] kodlar, int ay, double odenek, ENUMFormDurum durum);
        /// <summary>
        /// Parametre olarak verilen bilgileri kullanarak,
        /// İlgili Ayrıntılı Program bilgilerini veri tabanından silen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public abstract Sonuc Sil(int yil, string kurum, string islem, int asama, string[] kodlar);
        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu, Sıra No bilgilerini kullanarak,
        /// İlgili Ayrıntılı Program bilgilerini veri tabanından silen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public abstract Sonuc Sil(int yil, string kurum, string islemKod, int siraNo);
        /// <summary>
        /// Parametre olarak verilen Yıl, İşlem Kodu, Sıra No ve Kodlar bilgisini kıstas olarak kullanarak,
        /// Tertip toplamını döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra No bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kodlar bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Tertip Toplamını tutan Double türünde nesne döndürür</returns>
        public abstract double TertipToplamiBul(int yil, string islemKod, int siraNo, string[] kodlar);
        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu, Sıra No bilgisini kıstas olarak kullanarak,
        /// Ayrıntılı Program toplamını döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Ayrıntılı Program Toplamını tutan Double türünde nesne döndürür</returns>
        public abstract double ToplamBul(int yil, string kurum, string islemKod, int siraNo);
        /// <summary>
        /// Parametre olarak verilen Yıl, Kodlar bilgisini kullanarak,
        /// Yeni Onaylı Revize olanların toplam değerini ToplamBulRevize rutinini çağırarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="hepsi">Hepsi parametresine göre Revize çeşitlerine göre (AHP,IHP) koşulun eklenip eklenmeyeceğini belirten, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Revize toplamını tutan Double türünde nesne döndürür</returns>
        public abstract double ToplamBulRevize(int yil, string[] kodlar, bool hepsi);
        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kıstas olarak kullanarak,
        /// Kıstasa uygun Revize toplam değerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="hepsi">Hepsi parametresine göre Revize çeşitlerine göre (AHP,IHP) koşulun eklenip eklenmeyeceğini belirten, Bool türünde nesne</param>
        /// <param name="ay">Ay bilgisini tutan, Int türünde nesne</param>
        /// <param name="onayDurum">Durum bilgisini tutan, ENUMFormDurumGrup türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Revize Toplamı değerini tutan Double türünde nesne döndürür</returns>
        public abstract double ToplamBulRevize(int yil, string[] kodlar, bool hepsi, int ay, ENUMFormDurumGrup onayDurum);

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem, Sıra no bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Gider bilgilerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasTertip">Secilecek kayıtların kıstasının tutulduğu tertip nesnesi.</param>
        /// <param name="secAlan">Ay ve ödenek dışında sonuç kümesinde bulunacak alanlar.</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Gider bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public abstract IDataReader ListeleKodBilgi(string islem, int siraNo, Tertip kistasTertip, string secAlan);
    }

    /// <summary>
    /// Tablolarla ilgili olarak ek koşul alanlarını üreten Genel Koşul Sınıfı
    /// </summary>
    public class GenelKosul
    {
        /// <summary>
        /// Parametre olarak verilen Tablo koduna göre ilgili Tablo sınıfını döndüren fonksiyon
        /// </summary>
        /// <param name="tabloKod">Tablo kodunu tutan, ENUMTabloKod türünde nesne</param>
        /// <param name="ve">Veri Erişim bilgisini tutan, VeriErisim türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, TABButce türünde nesne döndürür</returns>
        public static TABButce TabButceOl(ENUMTabloKod tabloKod, VeriErisim ve)
        {
            if (tabloKod == ENUMTabloKod.TABGelir || tabloKod == ENUMTabloKod.TABGelirGerceklesmesi || tabloKod == ENUMTabloKod.TABGelirTahmini)
                return new TABButceGelir(ve);
            else if (tabloKod == ENUMTabloKod.TABFinansman)// || tabloKod == ENUMTabloKod.TABFinansmanGerceklesmesi || tabloKod == ENUMTabloKod.TABFinansmanTahmini)
                return new TABButceFinansman(ve);
            else
                return new TABButceGider(ve);
        }

        /// <summary>
        /// Parametre olarak verilen Tablo koduna göre ilgili Tablo sınıfını döndüren fonksiyon
        /// </summary>
        /// <param name="tabloKod">Tablo kodunu tutan, ENUMTabloKod türünde nesne</param>
        /// <param name="ve">Veri Erişim bilgisini tutan, VeriErisim türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, TABAyrintiliProgram türünde nesne döndürür</returns>
        public static TABAyrintiliProgram TabISLAPOl(ENUMTabloKod tabloKod, VeriErisim ve)
        {
            if (tabloKod == ENUMTabloKod.TABGelir || tabloKod == ENUMTabloKod.TABGelirGerceklesmesi || tabloKod == ENUMTabloKod.TABGelirTahmini)
                return new TABISLAPGelir(ve);
            else if (tabloKod == ENUMTabloKod.TABFinansman) // || tabloKod == ENUMTabloKod.TABFinansmanGerceklesmesi || tabloKod == ENUMTabloKod.TABFinansmanTahmini)
                return new TABISLAPFinansman(ve);
            else
                return new TABISLAPGider(ve);
        }

        /// <summary>
        /// Genel Koşul Sınıfının oluşturucu(Constructer) fonksiyonu
        /// </summary>
        public GenelKosul()
        {
            
        }

        /// <summary>
        /// Parametre olarak verilen Koşul, Tablo Adı, Alan Adı, Yer, Kodlar bilgisine göre,
        /// Sql koşul cümlesini oluşturan fonksiyon
        /// </summary>
        /// <param name="kosul">Koşul ifadesini tutan, String türünde nesne</param>
        /// <param name="tabloAd">Tablo adı bilgisini tutan, String türünde nesne</param>
        /// <param name="alanAd">Alan adı bilgisini tutan, String türünde nesne</param>
        /// <param name="yer">Yer bilgisini tutan, String türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="kurumMu">Kurum parametresine göre ek koşul ifadesi eklenip eklenmeyeceğini tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Koşul cümlesini içeren String türünde nesne döndürür</returns>
        public static string KosulEkle(string kosul, string tabloAd, string alanAd, int yer, string[] kodlar, bool kurumMu)
        {
            if (yer < kodlar.Length)
            {
                string kosulParam = kodlar[yer];
                if (!string.IsNullOrEmpty(kosulParam))
                {
                    string[] sa = kosulParam.Split(',');
                    int say = sa.Length;
                    string icKosul = "";
                    for (int i = 0; i < sa.Length; i++)
                    {
                        string enIcKosul = "";
                        string icParca = (string)sa[i];
                        if (icParca.Length == 2 && kurumMu)
                            enIcKosul = "(" + VeriErisim.KosulOl(tabloAd, alanAd, ">", icParca) + " AND " + VeriErisim.KosulOl(tabloAd, alanAd, "<", icParca + BKD.Arac.BagimsizKurumBaslangic()) + ")";
                        else
                            enIcKosul = VeriErisim.KosulOl(tabloAd, alanAd, "LIKE", icParca.Replace(".", "") + "%");
                        icKosul = VeriErisim.KosulEkle(icKosul, "OR", enIcKosul);
                    }
                    icKosul = "(" + icKosul + ")";
                    return VeriErisim.KosulEkle(kosul, "AND", icKosul);
                }
                else
                    return kosul;
            }
            else
                return kosul;
        }

        /// <summary>
        /// Parametre olarak verilen Ek, İşleç, Alan Adı ve Parametrelere göre,
        /// İşleçi üreten fonksiyon
        /// </summary>
        /// <param name="ek">Ek ifadeyi tutan, String türünde nesne</param>
        /// <param name="islec">İşleç bilgisini tutan, String türünde nesne</param>
        /// <param name="alanAd">Alan adını tutan, String türünde nesne</param>
        /// <param name="islecParametreler">İşleç parametrelerini tutan, String dizi türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, İşleç ifadesini tutan String türünde nesne döndürür</returns>
        public static string Islec(string ek, string islec, string alanAd, params string[] islecParametreler)
        {
            string donusDeger = "";
            if (!string.IsNullOrEmpty(ek))
                donusDeger += ek + " ";
            if (!string.IsNullOrEmpty(islec))
                donusDeger += islec + "(" + alanAd + ")";
            else
                donusDeger += alanAd;
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Tablo Adı, Alan Adı, İşleç ve Kurum Kodu bilgilerine göre
        /// Sql koşul cümlesi oluşturan fonksiyon
        /// </summary>
        /// <param name="tabloAd">Tablo adını tutan, String türünde nesne</param>
        /// <param name="alanAd">Alan adını tutan, String türünde nesne</param>
        /// <param name="islec">İşleç bilgisini tutan, String türünde nesne</param>
        /// <param name="kurumKod">Kurum kodu bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Koşul cümlesini içeren String türünde nesne döndürür</returns>
        public static string KurumKosulOl(string tabloAd, string alanAd, string islec, string kurumKod)
        {
            string kosul;

            if (kurumKod.Length == 2)
            {
                kosul = VeriErisim.KosulOl(tabloAd, alanAd, ">=", kurumKod);
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, alanAd, "<", kurumKod + BKD.Arac.BagimsizKurumBaslangic()));
                kosul = "(" + kosul + ")";
            }
            else if (kurumKod.IndexOf("/") >= 0)
            {
                kosul = "";
                string parca1 = "", parca2 = "";
                OrtakFonksiyonlar.StringiIkiyeBol(kurumKod, "/", ref parca1, ref parca2);

                kosul = VeriErisim.KosulOl(tabloAd, alanAd, ">=", parca1.Replace(".", ""));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, alanAd, "<", parca2.Replace(".", "")));
                kosul = "(" + kosul + ")";
            }
            else
                kosul = VeriErisim.KosulOl(tabloAd, alanAd, "LIKE", kurumKod.Replace(".", "") + "%");
            return kosul;
        }

        /// <summary>
        /// Parametre olarak verilen Tablo Adı, Alan Adı, İşleç ve Kod bilgisine göre
        /// Sql koşul cümlesi oluşturan fonksiyon
        /// </summary>
        /// <param name="tabloAd">Tablo adını tutan, String türünde nesne</param>
        /// <param name="alanAd">Alan adını tutan, String türünde nesne</param>
        /// <param name="islec">İşleç bilgisini tutan, String türünde nesne</param>
        /// <param name="kod">Kod bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Koşul cümlesini içeren String türünde nesne döndürür</returns>
        public static string KodKosulOl(string tabloAd, string alanAd, string islec, string kod)
        {
            string kosul = "";

            if (kod.IndexOf("/") >= 0)
            {
                string parca1 = "", parca2 = "";
                OrtakFonksiyonlar.StringiIkiyeBol(kod, "/", ref parca1, ref parca2);

                kosul = VeriErisim.KosulOl(tabloAd, alanAd, ">=", parca1.Replace(".", ""));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, alanAd, "<", parca2.Replace(".", "")));
                kosul = "(" + kosul + ")";
            }
            else if (kod.IndexOf(";") >= 0)
            {
                string[] kodlar = kod.Split(';');
                for (int i = 0; i < kodlar.Length; i++)
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(tabloAd, alanAd, "LIKE", kodlar[i].Replace(".", "") + "%"));

                kosul = "(" + kosul + ")";
            }
            else
                kosul = VeriErisim.KosulOl(tabloAd, alanAd, "LIKE", kod.Replace(".", "") + "%");
            return kosul;
        }

        /// <summary>
        /// Parametre olarak verilen Tablo Adı ve Alan Adına göre,
        /// Sql alan ifadesini döndüren fonksiyon
        /// </summary>
        /// <param name="tabloAd">Tablo adını tutan, String türünde nesne</param>
        /// <param name="alanAd">Alan adını tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Sql alan ifadesini tutan String türünde nesne döndürür</returns>
        public static string IslemAlanOl(string tabloAd, string alanAd)
        {
            string donusDeger;
            if (tabloAd == null || tabloAd == "")
                donusDeger = alanAd;
            else
                donusDeger = tabloAd + "." + alanAd;

            return VeriErisim.IslecBul("SUBSTRING") + "(" + donusDeger + ",1,4)";
        }

        /// <summary>
        /// Parametre olarak verilen Tablo Adı, Alan Adı ve Koşul bilgilerine göre,
        /// Sql koşul cümlesi oluşturan fonksiyon
        /// </summary>
        /// <param name="tabloAd">Tablo adı bilgisini tutan, String türünde nesne</param>
        /// <param name="alanAd">Alan Adı bilgisini tutan, String türünde nesne</param>
        /// <param name="kosul">Koşul ifadesini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Koşul cümlesini içeren String türünde nesne döndürür</returns>
        public static string IslemKosulOl(string tabloAd, string alanAd, string kosul)
        {
            if (kosul.Length == 2)
                return VeriErisim.KosulOl("", IslemAlanOl(tabloAd, alanAd), "LIKE", kosul.Replace(".", "") + "%");
            else
                return VeriErisim.KosulOl("", IslemAlanOl(tabloAd, alanAd), "=", kosul.Replace(".", "").Substring(0, 4));
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Aşama, Ek Koşul ve Kod bilgilerine göre,
        /// Sql koşul cümlesi oluşturan fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="ekKosul">Ek koşul ifadesini tutan, String türünde nesne</param>
        /// <param name="kistasKodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="cokYil">Çok yıla göre koşul ifadesinin eklenip eklenmeyeceğini tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Koşul cümlesini içeren String türünde nesne döndürür</returns>
        public static string KistasKosulOl(int yil, int asama, string ekKosul, string[] kistasKodlar, bool cokYil)
        {
            string kosul = "";

            if (yil != -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "YIL", "=", yil));

            if (asama != -1)
            {
                if (cokYil)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl("", "ASAMA", "=", asama));
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", "ASAMA", "=", asama + 10));
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", "ASAMA", "=", asama + 20)) + ")";
                }
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "ASAMA", "=", asama));
            }

            if (kistasKodlar.Length >= 1 && !string.IsNullOrEmpty(kistasKodlar[0]))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", KurumKosulOl("", "KURKOD", "=", (string)kistasKodlar[0]));
            }

            if (kistasKodlar.Length >= 2 && !string.IsNullOrEmpty(kistasKodlar[1]))
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl("", "FONKOD", "LIKE", ((string)kistasKodlar[1]).Replace(".", "")));
            if (kistasKodlar.Length >= 3 && !string.IsNullOrEmpty(kistasKodlar[2]))
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl("", "FINKOD", "LIKE", ((string)kistasKodlar[2]).Replace(".", "")));
            if (kistasKodlar.Length >= 4 && !string.IsNullOrEmpty(kistasKodlar[3]))
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl("", "EKOKOD", "LIKE", ((string)kistasKodlar[3]).Replace(".", "")));
            if (kistasKodlar.Length >= 5 && !string.IsNullOrEmpty(kistasKodlar[4]))
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl("", "PEBKOD", "LIKE", ((string)kistasKodlar[4]).Replace(".", "")));
            if (ekKosul != "")
            {
                if (kosul != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", ekKosul);
                else
                    kosul = ekKosul;
            }

            return kosul;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Aşama, Ek Koşul ve Kod bilgilerine göre,
        /// Sql koşul cümlesi oluşturan fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="ekKosul">Ek koşul ifadesini tutan, String türünde nesne</param>
        /// <param name="kistasKodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="cokYil">Çok yıla göre koşul ifadesinin eklenip eklenmeyeceğini tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Koşul cümlesini içeren String türünde nesne döndürür</returns>
        public static string KistasKosulOlEski(int yil, int asama, string ekKosul, string[] kistasKodlar, bool cokYil)
        {
            string kosul = "";

            if (yil != -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "YIL", "=", yil));

            if (asama != -1)
            {
                if (cokYil)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl("", "ASAMA", "=", asama));
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", "ASAMA", "=", asama + 10));
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", "ASAMA", "=", asama + 20)) + ")";
                }
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "ASAMA", "=", asama));
            }

            if (kistasKodlar.Length >= 1 && (string)kistasKodlar[0] != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", KurumKosulOl("", "KURUMSALKOD", "=", (string)kistasKodlar[0]));
            }

            if (kistasKodlar.Length >= 2 && (string)kistasKodlar[1] != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl("", "FONKSIYONELKOD", "LIKE", ((string)kistasKodlar[1]).Replace(".", "")));
            if (kistasKodlar.Length >= 3 && (string)kistasKodlar[2] != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl("", "FINANSKOD", "LIKE", ((string)kistasKodlar[2]).Replace(".", "")));
            if (kistasKodlar.Length >= 4 && (string)kistasKodlar[3] != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", KodKosulOl("", "EKONOMIKKOD", "LIKE", ((string)kistasKodlar[3]).Replace(".", "")));
            if (ekKosul != "")
            {
                if (kosul != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", ekKosul);
                else
                    kosul = ekKosul;
            }

            return kosul;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Ek Koşul ve Kod bilgilerine göre,
        /// Sql koşul cümlesi oluşturan fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="ekKosul">Ek Koşul ifadesini tutan, String türünde nesne</param>
        /// <param name="kistasKodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Koşul cümlesini içeren String türünde nesne döndürür</returns>
        public static string KistasKosulOlFinansman(int yil, string ekKosul, string[] kistasKodlar)
        {
            string kosul = "";

            if (yil != -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "YIL", "=", yil));

            if (kistasKodlar.Length >= 1 && (string)kistasKodlar[0] != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", KurumKosulOl("", "KURKOD", "=", (string)kistasKodlar[0]));
            }

            if (kistasKodlar.Length >= 2 && (string)kistasKodlar[1] != "")
                if (kistasKodlar.Length >= 3 && (string)kistasKodlar[2] != "")
                {
                    kosul = kosul + "AND(";
                    kosul = VeriErisim.KosulEkle(kosul, "", VeriErisim.KosulOl("", "EKOKOD", "LIKE", ((string)kistasKodlar[1]).Replace(".", "") + "%"));
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", "PEBKOD", "LIKE", ((string)kistasKodlar[2]).Replace(".", "") + "%"));
                    kosul = kosul + ")";
                }
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "EKOKOD", "LIKE", ((string)kistasKodlar[1]).Replace(".", "") + "%"));

            if (ekKosul != "")
            {
                if (kosul != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", ekKosul);
                else
                    kosul = ekKosul;
            }

            return kosul;
        }

        /// <summary>
        /// Parametre olarak verilen Ön Koşul, Bağlaç, Form Durumu, Tablo Adı ve Alan Adına göre,
        /// Sql koşul cümlesi oluşturan fonksiyon
        /// </summary>
        /// <param name="onKosul">Ön Koşul ifadesini tutan, String türünde nesne</param>
        /// <param name="baglac">Bağlaç ifadesini tutan, String türünde nesne</param>
        /// <param name="formDurumGrup">Form durumu bilgisini tutran, ENUMFormDurumGrup türünde nesne</param>
        /// <param name="tabloAd">Tablo adını tutan, String türünde nesne</param>
        /// <param name="alanAd">Alan adı bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Koşul cümlesini içeren String türünde nesne döndürür</returns>
        public static string FormDurumKosulOl(string onKosul, string baglac, ENUMFormDurumGrup formDurumGrup, string tabloAd, string alanAd)
        {
            if (formDurumGrup == ENUMFormDurumGrup.YENI)
                onKosul = VeriErisim.KosulEkle(onKosul, baglac, VeriErisim.KosulOl(tabloAd, alanAd, "<=", (int)ENUMFormDurum.YENIKAYIT));
            else if (formDurumGrup == ENUMFormDurumGrup.YENI_ONAYLI)
                onKosul = VeriErisim.KosulEkle(onKosul, baglac, VeriErisim.KosulOl(tabloAd, alanAd, "<=", (int)ENUMFormDurum.ONAYLI));
            else if (formDurumGrup == ENUMFormDurumGrup.ONAYLI)
            {
                onKosul = VeriErisim.KosulEkle(onKosul, baglac, VeriErisim.KosulOl(tabloAd, alanAd, ">=", (int)ENUMFormDurum.DYDOGRU));
                onKosul = VeriErisim.KosulEkle(onKosul, baglac, VeriErisim.KosulOl(tabloAd, alanAd, "<=", (int)ENUMFormDurum.ONAYLI));
            }
            else if (formDurumGrup == ENUMFormDurumGrup.ONAYLI)
            {
                onKosul = VeriErisim.KosulEkle(onKosul, baglac, VeriErisim.KosulOl(tabloAd, alanAd, ">=", (int)ENUMFormDurum.SAYISTAYRED));
            }
            return onKosul;
        }
    }
}
