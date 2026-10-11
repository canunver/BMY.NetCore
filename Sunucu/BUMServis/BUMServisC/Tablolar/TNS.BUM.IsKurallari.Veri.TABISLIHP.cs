// Static Model
using System.Data;
using TNS.Veri;

namespace TNS.BUM.IsKurallari.Veri
{

    /// <summary>
    /// Veritabanında ki icmal tablosudur.Yil, KurumKodu, Islem, SiraNo,EkoKod, Ay, Odenek, FormDurum bilgilerini saklar.
    /// </summary>
    public class TABISLIHP : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABISLIHP(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "ISLIHP";

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "YIL", "KURUMKOD", "ISLEM", "SIRANO", "EKOKOD", "AY", "ODENEK", "FORMDURUM" };

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Kurum kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURUMKOD = 1,
            /// <summary>
            /// İşlem alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEM = 2,
            /// <summary>
            /// Sıra no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 3,
            /// <summary>
            /// Ekonomik kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 4,
            /// <summary>
            /// Ay alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AY = 5,
            /// <summary>
            /// Ödenek alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENEK = 6,
            /// <summary>
            /// Form durumu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FORMDURUM = 7
        }

        /// <summary>
        /// Kendisine  verilen yıl, kurum, işlem, aşama ve ekonomik koda göre
        /// tablodaki değerlerin ay bazında toplamını alan, sorgu oluşturur.
        /// sorgu ay ve toplam rakam elde edilir ve datareader olarak döndürülür.
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String  türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nense</param>
        /// <param name="ekoKod">Ekonomik kod bilgisini tutan, String türünde nesne</param>
        /// <param name="formDurumGrup">Durum bilgisini tutan, ENUMFormDurumGrup türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, Aylık Ödenek bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader IHPAylikOdenek(int yil, string kurum, string islem, int asama, string ekoKod, ENUMFormDurumGrup formDurumGrup)
        {
            string kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[0], "=", yil);
            if (!string.IsNullOrEmpty(kurum))
                kosul = VeriErisim.KosulEkle(kosul, " AND ", GenelKosul.KurumKosulOl(TABLOAD, ALANLAR[1], "=", kurum));
            if (!string.IsNullOrEmpty(islem))
                kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[2], "=", islem.Replace(".", "")));
            if (asama >= 0)
                kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[3], "=", asama));
            if (!string.IsNullOrEmpty(ekoKod))
                kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[4], " LIKE ", ekoKod.Replace(".", "") + "%"));

            kosul = GenelKosul.FormDurumKosulOl(kosul, " AND ", formDurumGrup, TABLOAD, ALANLAR[7]);

            string sql = "SELECT AY, SUM(ODENEK) from " + TABLOAD + " where " + kosul + " Group By AY Order By 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yıl, kurum, aşamayı, ekokod ve ödeneği alıp, INSERT sorgusu oluşturup, 
        /// çalıştırır. İşlembaşarılı olmuşsa, sonuçta true, olmamışsa false ve 
        /// nedenini döndürür. 
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="ekoKod">Ekonomik Kod bilgisini tutan, String türünde nesne</param>
        /// <param name="ay">Ay bilgisini tutan, Int türünde nesne</param>
        /// <param name="odenek">Ödenek bilgisini tutan, Double türünde nesne</param>
        /// <param name="durum">Durum bilgisini tutan, Int türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc IHPKaydet(int yil, string kurum, string islem, int asama, string ekoKod, int ay, double odenek, int durum)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";
            if (kurum.Length < 2)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ButceUygulamaResI.BUM332;
                return sonuc;
            }
            if (odenek < 0 && islem.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHP)))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ButceUygulamaResI.BUM333;
                return sonuc;
            }

            sql = "insert into " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (" +
            VeriErisim.DegerYap(yil) + ", " +
            VeriErisim.DegerYap(kurum.Replace(".", "")) + ", " +
            VeriErisim.DegerYap(islem.Replace(".", "")) + ", " +
            VeriErisim.DegerYap(asama) + ", " +
            VeriErisim.DegerYap(ekoKod.Replace(".", "")) + ", " +
            VeriErisim.DegerYap(ay) + ", " +
            VeriErisim.DegerYap(odenek) + ", " +
            VeriErisim.DegerYap(durum) + ")";

            sonuc = KomutCalistir(sql, 0);

            return sonuc;
        }


        /// <summary>
        ///  Verilen kuruma ait, bir ekonomik kodun tüm ay bilgilerini siler. DELETE sorgusu oluşturup, çalıştırır.Başarılı olursa, sonuçta true, olmazsa false ve sebebi döndürülür.
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="ekoKod">Ekonomik Kod bilgisini tutan, String türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc IHPSil(int yil, string kurum, string islem, int asama, string ekoKod)
        {
            string kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[0], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[1], "=", kurum.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[2], "=", islem.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[3], "=", asama));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[4], " LIKE ", ekoKod.Replace(".", "") + "%"));

            string sql = "DELETE FROM " + TABLOAD + " WHERE " + kosul;
            return KomutCalistir(sql, 0);
        }


        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem, Aşama ve Ekonomik Kod bilgilerini kıstas olarak kullanarak,
        /// verilen kıstasa uygun IHP kayıt sayısını döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="ekoKod">Ekonomik Kod bilgisini tutan, String türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun IHP Kayıt sayısını tutan Double türünde nesne döndürür</returns>
        public double IHPKontrol(int yil, string kurum, string islem, int asama, string ekoKod)
        {

            string kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[0], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[1], "=", kurum.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[2], "=", islem.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[3], "=", asama));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[4], "LIKE", ekoKod.Replace(".", "") + "%"));
            //kosul=VeriErisim.KosulEkle(kosul," AND " ,VeriErisim.KosulOl(TABLOAD,ALANLAR[5],"=",ay));

            string sql = "SELECT count(*) from " + TABLOAD + " where " + kosul;
            return veriErisim.TekKolonSQLDbl(sql);
        }

        
        /// <summary>
        /// Parametre olarak verilen  Yıl, Kurum, İşlem Kodu, Sıra No ve Durum bilgisine göre
        /// IHP bilgilerini güncelleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <param name="durum">Durum bilgisini tutan, Int türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc OnayKaydet(int yil, string kurum, string islemKod, int siraNo, int durum)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FORMDURUM], durum) + " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));

            return KomutCalistir(sql, 0);
        }
        /// <summary>
        /// Verilen bütçe işlemine ait tüm detay gider kayıtları silinir.
        /// </summary>
        /// <param name="yil">Detay kayıtları silinecek bütçe işleminin yıl bilgisi.</param>
        /// <param name="kurum">Detay kayıtları silinecek bütçe işleminin kurum bilgisi.</param>
        /// <param name="islemKod">Detay kayıtları silinecek bütçe işleminin işlemkodu (serbest, ekleme) bilgisi.</param>
        /// <param name="siraNo">Detay kayıtları silinecek bütçe işleminin sıra no bilgisi.</param>
        /// <returns>Yapılan işlemin sonucu, sonuç yapısında döndürülür.</returns>
        public Sonuc Sil(int yil, string kurum, string islemKod, int siraNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            if (siraNo > -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], " = ", siraNo));

            Sonuc s = KomutCalistir(sql, 0);
            return s;
        }

        /// <summary>
        /// Verilen bütçe formuna ait satırların toplamı bulunur. 
        /// Toplam bulunurken sıfırdan büyükler ve aktarme ve ekleme işlemleri için hazine yardımı olmayan tertiplerin toplamı bulunmalıdır. 
        /// Oluşturulan sql çalıştırılır ve dönen double rakam döndürülür.
        /// </summary>
        /// <param name="yil">Toplam rakamı bulunacak işlemin yılı.</param>
        /// <param name="kurum">Toplam rakamı bulunacak işlemin kurum bilgisi.</param>
        /// <param name="islemKod">Toplam rakamı bulunacak işlemin işlem kodu bilgisi.</param>
        /// <param name="siraNo">Toplam rakamı bulunacak işlemin sıra no bilgisi.</param>
        /// <returns>Verilen formun toplamı double olarak.</returns>
        public double ToplamBul(int yil, string kurum, string islemKod, int siraNo)
        {
            string sql = "SELECT SUM(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, " AND ", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));
            sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODENEK], ">", (int)0));

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem, Sıra No bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun IHP bilgilerini dönen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun IHP bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(int yil, string kurum, string islem, int siraNo)
        {
            string kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, " AND ", GenelKosul.KurumKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islem.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));

            string sql = "SELECT EKOKOD, AY, ODENEK from " + TABLOAD + " where " + kosul + " Order By 1,2";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kod bilgilerine göre,
        /// Form Durumu Hepsi olan kayıtların Aylık Ödenek bilgisini IHAylikOdenek rutinini çağırarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Fonksiyonun çalışması durumunda, Aylık Ödenek bilgisiin tutan Double türünde nesne döndürür</returns>
        public IDataReader HPTertipDurum(int yil, string[] kodlar)
        {
            return IHPAylikOdenek(yil, kodlar[0], "", -1, kodlar[1], ENUMFormDurumGrup.HEPSI);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kod bilgilerine göre,
        /// Yeni Onaylıların Toplamını ToplamBulRevize rutinini çağırarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Fonksiyonun çalışması durumunda, Revize Toplamını tutan Double türünde nesne döndürür</returns>
        public double ToplamBulRevize(int yil, string[] kodlar)
        {
            return ToplamBulRevize(yil, kodlar, ENUMFormDurumGrup.YENI_ONAYLI);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kod bilgisini kıstas olarak kullanarak,
        /// verilen kıstasa uygun Revize olanların Toplamını döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="onayDurum">Durum bilgisini tutan, ENUMFormDurumGrup türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Revize Toplamını tutan Double türünde nesne döndürür</returns>
        public double ToplamBulRevize(int yil, string[] kodlar, ENUMFormDurumGrup onayDurum)
        {
            string kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, " AND ", GenelKosul.KurumKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", (string)kodlar[0]));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", ((string)kodlar[1]).Replace(".", "") + "%"));

            kosul = GenelKosul.FormDurumKosulOl(kosul, "AND", onayDurum, "", ALANLAR[(int)ENUMAlanlar.FORMDURUM]);
            kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], "LIKE", BISabitler.IslemKod(ENUMIslemKod.IHP).Replace(".", "") + "%"));
            kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], "LIKE", BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE).Replace(".", "") + "%"));
            kosul = kosul + ")";

            string sql = "SELECT SUM(ODENEK) from " + TABLOAD + " where " + kosul;

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kod bilgisini kıstas olarak kullanarak,
        /// verilen kıstasa uygun Tertip Durum bilgilerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="onayDurum">Durum bilgisini tutan, ENUMFormDurumGrup türünde nesne</param>
        /// <param name="sadeceRevizeler">Revizelerle ilgili koşulun eklenip eklenmeyeceği bilgisini tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Tertip Durum bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader TertipDurum(int yil, string[] kodlar, ENUMFormDurumGrup onayDurum, bool sadeceRevizeler)
        {
            string kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, " AND ", GenelKosul.KurumKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", (string)kodlar[0]));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", ((string)kodlar[1]).Replace(".", "") + "%"));

            kosul = GenelKosul.FormDurumKosulOl(kosul, "AND", onayDurum, "", ALANLAR[(int)ENUMAlanlar.FORMDURUM]);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], "LIKE", "01%"));

            string sql = "SELECT AY, SUM(ODENEK) from " + TABLOAD + " where " + kosul + " Group By AY Order By 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kod bilgilerine göre,
        /// Tertip Durum bilgisini TertipDurum rutinini çağırarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="onayDurum">Durum bilgisini tutan, ENUMFormDurumGrup türünde nesne</param>
        /// <returns>Fonksiyonun çalışması durumunda, Tertip Durum bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader TertipDurum(int yil, string[] kodlar, ENUMFormDurumGrup onayDurum)
        {
            return TertipDurum(yil, kodlar, onayDurum, false);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kod bilgilerine göre,
        /// Onaylı kayıtlar için Tertip Durum bilgisini TertipDurum rutinini çağırarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Fonksiyonun çalışması durumunda, Tertip Durum bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader TertipDurum(int yil, string[] kodlar)
        {
            return TertipDurum(yil, kodlar, ENUMFormDurumGrup.ONAYLI, false);
        }
    }

} 
