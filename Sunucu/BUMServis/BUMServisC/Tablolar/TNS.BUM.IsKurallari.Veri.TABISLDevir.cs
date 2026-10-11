using System.Data;
using TNS.Veri;
using TNS.BUM;

namespace TNS.BUM.IsKurallari.Veri
{
    /// <summary>
    /// Devir bilgilerini tutan ISLDEVIR veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABISLDevir:TABOrtak
	{
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABISLDevir(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "ISLDEVIR"; 

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR={"YIL", "KURUMKOD", "ISLEM", "SIRANO", "KURKOD","FONKOD", "FINKOD", "EKOKOD", "KURKODE","FONKODE", "FINKODE", "EKOKODE","ODENEK", "FORMDURUM"};

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL=0,
            /// <summary>
            /// Kurum kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURUMKOD=1,
            /// <summary>
            /// İşlem alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEM=2,
            /// <summary>
            /// Sıra no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO=3,
            /// <summary>
            /// Kurumsal kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD=4,
            /// <summary>
            /// Fonksiyonel kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FONKOD=5,
            /// <summary>
            /// Finansal kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FINKOD=6,
            /// <summary>
            /// Ekonomik kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD=7, 
            /// <summary>
            /// Bir önceki yıla ait Kurumsal kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKODE=8,
            /// <summary>
            /// Bir önceki yıla ait Fonksiyonel kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FONKODE=9,
            /// <summary>
            /// Bir önceki yıla ait Finansal kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FINKODE=10,
            /// <summary>
            /// Bir önceki yıla ait Ekonomik kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKODE=11, 
            /// <summary>
            /// Ödenek alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENEK=12,
            /// <summary>
            /// Form durumu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FORMDURUM=13
        }

     
        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu, Sıra No ve durum bilgilerine göre
        /// Devir bilgilerini veri tabanından güncelleyen,
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
            string sql="";

            sql = "UPDATE " + TABLOAD +" SET "; 
            sql += VeriErisim.AtamaOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.FORMDURUM], durum)+ " " ;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".","")));
            sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));

            return	KomutCalistir(sql, 0);
        }

		
        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu, Sıra No ve Kodlar dizini kıstas olarak kullanarak,
        /// kıstasa uygun Devir bilgilerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurum, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Devir bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public string Listele(int yil, string kurum, string islemKod, int siraNo, string[] kodlar)
		{
			string islecArti=VeriErisim.IslecBul("+", veriErisim.baglantiAd);

            string sql="SELECT " + ALANLAR[(int)ENUMAlanlar.KURKODE]+ islecArti+"'-'"+islecArti + ALANLAR[(int)ENUMAlanlar.FONKODE]+ islecArti+"'-'"+islecArti + ALANLAR[(int)ENUMAlanlar.FINKODE]+ islecArti+"'-'"+islecArti + ALANLAR[(int)ENUMAlanlar.EKOKODE]+ " FROM " + TABLOAD;
            sql=VeriErisim.KosulEkle(sql, "WHERE", GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false)); 
            sql=VeriErisim.KosulEkle(sql," AND " , VeriErisim.KosulOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.KURUMKOD],"=",kurum.Replace(".","")));
            sql=VeriErisim.KosulEkle(sql," AND " , GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            sql=VeriErisim.KosulEkle(sql," AND " , VeriErisim.KosulOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.SIRANO],"=", siraNo));

            return veriErisim.TekKolonSQLStr(sql);
        }

		/// <summary>
		/// Parametre olarak verilen Yıl, Kurum, İşlem Kodu ve Sıra no bilgisini kullanarak,
        /// İlgili devir bilgilerini veri tabanından silen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
		/// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(int yil, string kurum, string islemKod, int siraNo)
		{
            string sql="DELETE FROM " + TABLOAD;
            sql=VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL] ,"=",yil));
            sql=VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD] ,"=",kurum.Replace(".","")));
            sql=VeriErisim.KosulEkle(sql, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            if(siraNo>-1)
                sql=VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO] ," = ", siraNo));

            Sonuc s=KomutCalistir(sql, 0);
            return s;
        }

        /// <summary>
        /// Verilen devir bilgilerini veri tabanına kaydeden,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="kodlarE">Bir önceki yıla ait Kurumsal, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="odenek">Ödenek bilgisini tutan, Double türünde nesne</param>
        /// <param name="formDurum">Form durum bilgisini tutan, Int türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(int yil, string kurum, string islemKod, int siraNo, string[] kodlar, string[] kodlarE, double odenek, int formDurum)
		{
            string sql="INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") Values (";
            sql += VeriErisim.DegerYap(yil) + ", ";
            sql += VeriErisim.DegerYap(kurum.Replace(".","")) + ", ";
            sql += VeriErisim.DegerYap(islemKod.Replace(".","")) + ", ";
            sql += VeriErisim.DegerYap(siraNo) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlar[0]).Replace(".","")) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlar[1]).Replace(".","")) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlar[2]).Replace(".","")) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlar[3]).Replace(".","")) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlarE[0]).Replace(".","")) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlarE[1]).Replace(".","")) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlarE[2]).Replace(".","")) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlarE[3]).Replace(".","")) + ", ";
            sql += VeriErisim.DegerYap(odenek) + ", ";
            sql += VeriErisim.DegerYap(formDurum) + ") ";


            Sonuc s=KomutCalistir(sql, 1);
            return s;
        }

	}

}
