using System.Data;
using TNS.Veri;
using TNS.BUM;

namespace TNS.BUM.IsKurallari.Veri
{
    /// <summary>
    /// Finansman bilgilerini tutan ISLFINANSMAN veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABISLFinansman:TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABISLFinansman(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "ISLFINANSMAN"; 

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR={"YIL", "KURUMKOD", "ISLEM", "SIRANO", "KURKOD", "EKOKOD", "ODENEK", "FORMDURUM", "YILDIZYER"};

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
            /// Ekonomik kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD=5, 
            /// <summary>
            /// Ödenek alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENEK=6,
            /// <summary>
            /// Form durumu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FORMDURUM=7, 
            /// <summary>
            /// Yıldız yer alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YILDIZYER=8
        }

        
        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu, Sıra No ve durum bilgilerine göre,
        /// Finansman bilgilerini veri tabanında güncelleyen,
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
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu ve Sıra no bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Finansman bilgilerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Finansman bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(int yil, string kurum, string islemKod, int siraNo)
        {
            string sql="SELECT " + VeriErisim.IslecBul("SIGN", veriErisim.baglantiAd) + "(" + ALANLAR[(int)ENUMAlanlar.ODENEK]+"), "+ALANLAR[(int)ENUMAlanlar.KURKOD]+ ", " + ALANLAR[(int)ENUMAlanlar.EKOKOD]+ ", " + ALANLAR[(int)ENUMAlanlar.ODENEK]+ ", " + ALANLAR[(int)ENUMAlanlar.YILDIZYER]+ " FROM " + TABLOAD;
            sql=VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL],"=", yil)); 
            sql=VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.KURUMKOD],"=",kurum.Replace(".","")));
            sql=VeriErisim.KosulEkle(sql, " AND ", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            sql=VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.SIRANO],"=", siraNo));
            sql+=" order by 1,2,3";
            return veriErisim.DataReaderDondur(sql);
        }

        
        /// <summary>
        /// Parametre olarak verilen Yıl, İşlem Kodu ve Kodlar bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Eklenen ve Düşülen miktarları bulan,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="kodlar">Kurum, Fonksiyonel, Finansal, Ekonomik kodlar bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="formDurumGrupKod">Grup kodu bilgisini tutan, ENUMFormDurumGrup türünde nesne</param>
        /// <param name="eklenen">Eklenen miktar bilgisini tutan, Double türünde nesne</param>
        /// <param name="dusulen">Düşülen miktar bilgisiin tutan, Double türünde nesne</param>
        public void EklenenDusulenBul(int yil, string islemKod, string[] kodlar, ENUMFormDurumGrup formDurumGrupKod, out double eklenen, out double dusulen)
        {
            eklenen=0;
            dusulen=0;

            string sql="SELECT " + VeriErisim.IslecBul("SIGN", veriErisim.baglantiAd) + "(" + ALANLAR[(int)ENUMAlanlar.ODENEK]+"), SUM("+ALANLAR[(int)ENUMAlanlar.ODENEK]+ ") FROM " + TABLOAD;
            sql=VeriErisim.KosulEkle(sql, "WHERE", GenelKosul.KistasKosulOlFinansman(yil, "", kodlar)); 
            if(islemKod.Length<=4)
                sql=VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.ISLEM],"LIKE",islemKod+"%"));
            else
                sql=VeriErisim.KosulEkle(sql, " AND ", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
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
        /// Parametre olarak verilen Yıl, Kurum, İşlem kodu, Sıra no bilgilerini kullanarak,
        /// İlgili Finansman bilgilerini veri tabanından silen,
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
        /// Verilen finansman bilgilerini veri tabanına kaydeden,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="odenek">Ödenek bilgisini tutan, Double türünde nesne</param>
        /// <param name="formDurum">Form durumu bilgisini tutan, Int türünde nesne</param>
        /// <param name="yildizYer">Yıldız yer bilgisini tutan, Int türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(int yil, string kurum, string islemKod, int siraNo, string[] kodlar, double odenek, int formDurum, int yildizYer)
        {
            string sql="INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") Values (";
            sql += VeriErisim.DegerYap(yil) + ", ";
            sql += VeriErisim.DegerYap(kurum.Replace(".","")) + ", ";
            sql += VeriErisim.DegerYap(islemKod.Replace(".","")) + ", ";
            sql += VeriErisim.DegerYap(siraNo) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlar[0]).Replace(".","")) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlar[1]).Replace(".","")) + ", ";
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
            sql=VeriErisim.KosulEkle(sql, "WHERE", GenelKosul.KistasKosulOlFinansman(yil, "", kodlar)); 
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
