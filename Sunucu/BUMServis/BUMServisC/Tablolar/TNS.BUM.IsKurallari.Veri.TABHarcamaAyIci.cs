using TNS;
using TNS.Veri;
using TNS.BUM.IsKurallari.Veri;
using System.Data;

namespace TNS.BUM.IsKurallari.Veri
{

    /// <summary>
    /// Aylık Harcama bilgilerini tutan HARCAMAAYICI veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABHarcamaAyIci:TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHarcamaAyIci(VeriErisim veriErisim) : base(veriErisim) { }
                    
        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD="HARCAMAAYICI";
        
        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "YIL", "AY", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "HARCAMA"};
        
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
            /// Ay alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AY=1,
            /// <summary>
            /// Kurumsal kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD=2,
            /// <summary>
            /// Fonksiyonel kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FONKOD=3,
            /// <summary>
            /// Finansal kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FINKOD=4,
            /// <summary>
            /// Ekonomik kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD=5,            
            /// <summary>
            /// Harcama alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMA=6,           
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Harcama bilgisini dönen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal ve Ekonomik kod bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Harcama bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader HarcamaAylikOdenek(int yil, string[] kodlar)
        {
            string kosul = GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false);
            string sql = "SELECT AY, SUM(HARCAMA) from " + TABLOAD + " where " + kosul;
            sql = sql + " group by " + " AY";
            sql = sql + " order by " + " AY";

            return veriErisim.DataReaderDondur(sql);
        }

        /*
        public double HarcamaBul(int yil, string kurKod,string fonKod,string finKod, string ekoKod,string hesapKod)
        {

            TNSCollection kistasKodlar=new TNSCollection();
            kurKod=kurKod.Replace(".","");
            fonKod=fonKod.Replace(".","");
            finKod=finKod.Replace(".","");
            ekoKod=ekoKod.Replace(".","");

            kistasKodlar.Add(kurKod);
            kistasKodlar.Add(fonKod);
            kistasKodlar.Add(finKod);
            kistasKodlar.Add(ekoKod);

            string kosul = GenelKosul.KistasKosulOl(yil,-1,"",kistasKodlar,false);
            kosul=VeriErisim.KosulEkle(kosul,"AND",VeriErisim.KosulOl("", "MUHASEBEKOD",   "LIKE",hesapKod + "%"));          
            string sql="";  
            sql= "SELECT SUM(BORC-ALACAK) from MUHASEBAT " + " where " + kosul ;            

            return veriErisim.TekKolonSQLDbl(sql);  
            
        }
//        public double AvansBul(int yil,string kurum)
//        {
//            string kosul=VeriErisim.KosulEkle(kosul,"AND",VeriErisim.KosulOl("", "YIL", "=",yil));          
//            kosul=VeriErisim.KosulEkle(kosul,"AND",VeriErisim.KosulOl("", "KURKOD", "LIKE",kurum + "%"));          
//            return HarcamaBul(yil,kurum,"","","",hesapKod);
//        }
//		
*/
 


    }

}
