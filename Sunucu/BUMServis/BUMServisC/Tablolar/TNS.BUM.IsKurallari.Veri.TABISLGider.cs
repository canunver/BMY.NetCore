using System.Data;
using TNS.Veri;
using TNS.BUM;

namespace TNS.BUM.IsKurallari.Veri
{
    /// <summary>
    /// Gider bilgilerini tutan ISLGIDER veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABISLGider:TABOrtak
	{

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABISLGider(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "ISLGIDER"; 

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR={"YIL", "KURUMKOD", "ISLEM", "SIRANO", "KURKOD","FONKOD", "FINKOD", "EKOKOD", "ODENEK", "FORMDURUM", "YILDIZYER"};

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
            /// Ödenek alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENEK=8,
            /// <summary>
            /// Form durumu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FORMDURUM=9, 
            /// <summary>
            /// Yıldız yer alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YILDIZYER=10
        }

        
        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu, Sıra No ve durum bilgilerine göre,
        /// Gider bilgilerini veri tabanında güncelleyen,
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
        /// Parametre olarak Bağlaç, Tablo Adına göre,
        /// Hazine Yardımına ilişkin Gider Koşulu sql cümlesini oluşturan fonksiyon
        /// </summary>
        /// <param name="baglac">Bağlaç ifadesini tutan, String türünde nesne</param>
        /// <param name="tabloAd">Tablo adı bilgisini tutan, String türünde nesne</param>
        /// <param name="olmayan">Olmayan değerine göre Fonksiyonel Kod ile ilgili satırın eklenip eklenmeyeceğini tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, sql koşul cümlesini tutan String türünde nesne döndürür</returns>
        public static string HazineYardimiGiderKosul(string baglac, string tabloAd, bool olmayan)
        {
            if(baglac != "") baglac = " " + baglac + " ";
            if(tabloAd != "") tabloAd = tabloAd + ".";
            string donusDeger;
            if (olmayan)
                baglac+=" (NOT ";
            donusDeger=baglac + "("+ tabloAd +"FONKOD LIKE '0161%' OR "+ tabloAd + "FONKOD LIKE '0162%')";
            if (olmayan)
                donusDeger+=")";
            return donusDeger;
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
            string sql="SELECT SUM(" + ALANLAR[(int)ENUMAlanlar.ODENEK]+ ") FROM " + TABLOAD;
            sql=VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL],"=", yil)); 
            sql=VeriErisim.KosulEkle(sql," AND ", VeriErisim.KosulOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.KURUMKOD],"=",kurum.Replace(".","")));
            sql=VeriErisim.KosulEkle(sql," AND ", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            sql=VeriErisim.KosulEkle(sql," AND ", VeriErisim.KosulOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.SIRANO],"=", siraNo));
            sql=VeriErisim.KosulEkle(sql," AND ", VeriErisim.KosulOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.ODENEK],">", (int)0));
            if(islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AKTARMA)) || islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.EKLEME)))
                sql+=HazineYardimiGiderKosul("AND", "", true);

            return veriErisim.TekKolonSQLDbl(sql);
        }

		/// <summary>
		/// Bir bütçe işleminin detay gider kayıtları listelenir.
		/// </summary>
        /// <param name="yil">Detayı listelenecek bütçe işleminin yılı.</param>
        /// <param name="kurum">Detayı listelenecek bütçe işleminin kurum bilgisi.</param>
        /// <param name="islemKod">Detayı listelenecek bütçe işleminin işlem kodu bilgisi.</param>
        /// <param name="siraNo">Detayı listelenecek bütçe işleminin sıra no bilgisi.</param>
        /// <returns>Verilen bütçe işleminin; KurKod, FonKod, FinKod, EkoKod, Odenek, Yıldızyer bilgileri IDataReader'da döndürülür.</returns>
		public IDataReader Listele(int yil, string kurum, string islemKod, int siraNo)
		{
            string sql="SELECT " + VeriErisim.IslecBul("SIGN", veriErisim.baglantiAd) + "(" + ALANLAR[(int)ENUMAlanlar.ODENEK]+"), "+ALANLAR[(int)ENUMAlanlar.KURKOD]+ ", " + ALANLAR[(int)ENUMAlanlar.FONKOD]+ ", " + ALANLAR[(int)ENUMAlanlar.FINKOD]+ ", " + ALANLAR[(int)ENUMAlanlar.EKOKOD]+ ", " + ALANLAR[(int)ENUMAlanlar.ODENEK]+ ", " + ALANLAR[(int)ENUMAlanlar.YILDIZYER]+ " FROM " + TABLOAD;
            sql=VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL],"=", yil)); 
            sql=VeriErisim.KosulEkle(sql," AND " , VeriErisim.KosulOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.KURUMKOD],"=",kurum.Replace(".","")));
            sql=VeriErisim.KosulEkle(sql," AND " , GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            sql=VeriErisim.KosulEkle(sql," AND " , VeriErisim.KosulOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.SIRANO],"=", siraNo));
            sql+=" order by 1,2,3,4,5";
            return veriErisim.DataReaderDondur(sql);
        }

		/// <summary>
		/// Verilen bütçe işlemindeki kurumların listesini bulur ve döndürür. 
		/// Tablodan verilen koşula uyan kayıtların, KurKod alanının ilk dört hanesi farklı olanları 
		/// seçen sorgu oluşturulur ve IDataReader olarak döndürülür.
		/// </summary>
		/// <param name="yil">Kurum listesinin döndürüleceği işlemin yıl bilgisi.</param>
		/// <param name="kurum">Kurum listesinin döndürüleceği işlemin kurum bilgisi.</param>
		/// <param name="islemKod">Kurum listesinin döndürüleceği işlemin işlem kodu bilgisi.</param>
		/// <param name="siraNo">Kurum listesinin döndürüleceği işlemin sıra no bilgisi.</param>
		/// <returns>Bir bütçe işlemindeki, farklı kurumlar TNSCollection içerisinde döndürülür.</returns>
 		public IDataReader KurumlarListesi(int yil, string kurum, string islemKod, int siraNo)
		{
            string sql="SELECT DISTINCT LEFT(" + ALANLAR[(int)ENUMAlanlar.KURKOD]+ ",4) FROM " + TABLOAD;
            sql=VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL],"=", yil)); 
            sql=VeriErisim.KosulEkle(sql," AND ", VeriErisim.KosulOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.KURUMKOD],"=",kurum.Replace(".","")));
            sql=VeriErisim.KosulEkle(sql," AND ", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            sql=VeriErisim.KosulEkle(sql," AND ", VeriErisim.KosulOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.SIRANO],"=", siraNo));
            sql+=" order by 1";
            return veriErisim.DataReaderDondur(sql);
        }

		/// <summary>
		/// Verilen yıl, işlem kodu ve tertiplerin, verilen onay grupkodunda olanlarının toplamları;
		/// 0'dan büyük ve sıfırdan küçük olmak üzere bulur. 
		/// 0'dan büyükleri eklenen'e; 0'dan küçükleri ise düşülen'e pozitif olarak atar.
		/// </summary>
		/// <param name="yil">Verilen tertibe uyan kayıtların toplamlarının bulunacağı yıl.</param>
		/// <param name="islemKod">Verilen tertibe uyan kayıtların toplamlarının bulunacağı işlem.</param>
		/// <param name="kodlar">Verilen tertip kıstası</param>
		/// <param name="formDurumGrupKod">Verilen tertiplerin işlem onay durumu kıstası</param>
		/// <param name="eklenen">0'dan büyük olan değerleri tutacak olan değişken</param>
		/// <param name="dusulen">0'dan küçük olan değerleri pozitif olarak tutacak olan değişken</param>
 		public void EklenenDusulenBul(int yil, string islemKod, string[] kodlar, ENUMFormDurumGrup formDurumGrupKod, out double eklenen, out double dusulen)
		{
            eklenen=0;
            dusulen=0;

            string sql="SELECT " + VeriErisim.IslecBul("SIGN", veriErisim.baglantiAd) + "(" + ALANLAR[(int)ENUMAlanlar.ODENEK]+"), SUM("+ALANLAR[(int)ENUMAlanlar.ODENEK]+ ") FROM " + TABLOAD;
            sql=VeriErisim.KosulEkle(sql, "WHERE", GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false)); 
            if(islemKod.Length==2)
                sql=VeriErisim.KosulEkle(sql," AND ", VeriErisim.KosulOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.ISLEM],"LIKE",islemKod+"%"));
            else
                sql=VeriErisim.KosulEkle(sql," AND ", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            sql=GenelKosul.FormDurumKosulOl(sql, "AND", formDurumGrupKod, "", ALANLAR[(int)ENUMAlanlar.FORMDURUM]);
            sql+=" group by "+VeriErisim.IslecBul("SIGN", veriErisim.baglantiAd) + "(" + ALANLAR[(int)ENUMAlanlar.ODENEK]+")";



            IDataReader dr=veriErisim.DataReaderDondur(sql);
            if(veriErisim.hata!="")
            {
                eklenen=-1;
                dusulen=-1;
            }
            else
            {
                while(dr.Read())
                {
                    int isaret=(int)VeriErisim.DegerAl(dr, 0, (double) 0);
                    if(isaret<0)
                        dusulen=0-VeriErisim.DegerAl(dr, 1, (double) 0);
                    else
                        eklenen=VeriErisim.DegerAl(dr, 1, (double) 0);
                }
                dr.Close();
            }

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
		/// Verilen bütçe işlemi detay kaydı, insert SQL'ine çevrilir ve çalıştırılır.
		/// </summary>
        /// <param name="yil">Verilen bütçe işlemi detay kaydının yıl bilgisi.</param>
        /// <param name="kurum">Verilen bütçe işlemi detay kaydının kurum bilgisi.</param>
        /// <param name="islemKod">Verilen bütçe işlemi detay kaydının işlemkodu (serbest, ekleme) bilgisi.</param>
        /// <param name="siraNo">Verilen bütçe işlemi detay kaydının sıra no bilgisi.</param>
        /// <param name="kodlar">Verilen bütçe işlemi detay kaydının kod bilgileri (kur, fon, fin, eko)</param>
		/// <param name="odenek">Verilen bütçe işlemi detay kaydının ödenek bilgisi</param>
		/// <param name="formDurum">Verilen bütçe işlemi detay kaydının form durumu</param>
		/// <param name="yildizYer">Verilen bütçe işlemi detay kaydındaki tertibin, yeni olup olmadığı</param>
        /// <returns>Yapılan işlemin sonucu, sonuç yapısında döndürülür.</returns>
		public Sonuc Kaydet(int yil, string kurum, string islemKod, int siraNo, string[] kodlar, double odenek, int formDurum, int yildizYer)
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
            sql += VeriErisim.DegerYap(odenek) + ", ";
            sql += VeriErisim.DegerYap(formDurum) + ", ";
            sql += VeriErisim.DegerYap(yildizYer) + ") ";

            Sonuc s=KomutCalistir(sql, 1);
            return s;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kodlar bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Yıl içinde Bütçe işlemleriyle açılan yeni tertiplerini açıldığı yeri döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem Kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun yeni tertip açıldıysa true açılmamışsa false değerini tutan Bool türünde nesne döndürür</returns>
        public bool YildizVarMi(int yil, string[] kodlar, out string kurum, out string islemKod, out int siraNo)
        {
            string sql="SELECT " + ALANLAR[(int)ENUMAlanlar.KURUMKOD]+", "+ALANLAR[(int)ENUMAlanlar.ISLEM]+", "+ALANLAR[(int)ENUMAlanlar.SIRANO] + " FROM " + TABLOAD;
            sql=VeriErisim.KosulEkle(sql, "WHERE", GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false)); 
            sql=GenelKosul.FormDurumKosulOl(sql, "AND", ENUMFormDurumGrup.YENI_ONAYLI, "", ALANLAR[(int)ENUMAlanlar.FORMDURUM]);
            sql=VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("",ALANLAR[(int)ENUMAlanlar.YILDIZYER],">",(int) 0));
            sql+=" order by 1,2,3";
            kurum="";
            islemKod="";
            siraNo=0;
            IDataReader dr=veriErisim.DataReaderDondur(sql);
            if(veriErisim.hata=="")
            {
                if(dr!=null)
                {
                    if(dr.Read())
                    {
                        kurum=VeriErisim.DegerAl(dr, 0, "");
                        islemKod=VeriErisim.DegerAl(dr, 1, "");
                        siraNo=VeriErisim.DegerAl(dr, 2, (int)0);
                    }
                    dr.Close();
                }
            }
            return kurum!="";

        }

	}

} 
