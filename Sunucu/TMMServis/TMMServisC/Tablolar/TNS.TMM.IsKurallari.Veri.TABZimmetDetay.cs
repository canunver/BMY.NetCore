using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Zimmet Detay bilgilerini tutan TASZIMMETDETAY veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABZimmetDetay : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABZimmetDetay(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASZIMMETDETAY";

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
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 2,
            /// <summary>
            /// Fiş no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISNO = 3,
            /// <summary>
            /// Belge türü alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BELGETUR = 4,
            /// <summary>
            /// Sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SICILNO = 5,
            /// <summary>
            /// Sıra no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 6,
            /// <summary>
            /// Hesap plan kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPPLANKOD = 7,
            /// <summary>
            /// Özellik alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELLIK = 8,
            /// <summary>
            /// Kdv oran alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KDVORAN = 9,
            /// <summary>
            /// Birim fiyat alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYAT = 10,
            /// <summary>
            /// Teslim durumu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TESLIMDURUM = 11
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO", "BELGETUR", "SICILNO", "SIRANO", "HESAPPLANKOD", "OZELLIK", "KDVORAN", "BIRIMFIYAT", "TESLIMDURUM" };

        /// <summary>
        /// Zimmet Detay bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="zd">Zimmet Detay bilgilerini tutan, ZimmetDetay türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(ZimmetDetay zd)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(zd.yil) + ",";
            sql += VeriErisim.DegerYap(zd.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(zd.harcamaBirimKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(zd.fisNo) + ",";
            sql += VeriErisim.DegerYap(zd.belgeTur) + ",";
            sql += VeriErisim.DegerYap(zd.prSicilNo) + ",";
            sql += VeriErisim.DegerYap(zd.siraNo) + ",";
            sql += VeriErisim.DegerYap(zd.hesapPlanKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(zd.ozellik) + ",";
            sql += VeriErisim.DegerYap(zd.kdvOran) + ",";
            sql += VeriErisim.DegerYap(zd.birimFiyat) + ",";
            sql += VeriErisim.DegerYap(zd.teslimDurum) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Zimmet Detay bilgisini kullanarak
        /// Zimmet Detay  bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="zd">Zimmet Detay bilgisini tutan, ZimmetDetay türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(ZimmetDetay zd)
        {
            string sql = "DELETE FROM " + TABLOAD;

            //Fişe ait bütün satırlar
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", zd.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", zd.fisNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", zd.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", zd.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", zd.belgeTur));

            //Tek satır
            if (zd.siraNo != 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", zd.siraNo));
            if (zd.prSicilNo != 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SICILNO], "=", zd.prSicilNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen Zimmet Detay bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Zimmet Detay bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="zd">Zimmet Detay bilgisini tutan, ZimmetDetay türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Zimmet Detay bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(ZimmetDetay zd)
        {
            //Barkod yazdırmada sicil özellikleri çıkmıyor
            //string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",TASHESAPPLAN.ACIKLAMA FROM " + TABLOAD;
            //sql += " JOIN TASHESAPPLAN ON TASZIMMETDETAY.HESAPPLANKOD = TASHESAPPLAN.KOD";

            //Barkod yazdırmada gerekli oldu. Yukarıdaki kısım yerine burası konuldu.
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",TASHESAPPLAN.ACIKLAMA,ADI,YAZARADI,DISSICILNO,YAYINTARIHI,CILTNO,YERIKONUSU,TASMARKA.AD, TASMODEL.AD,SASENO, TASSICILNOOZELLIK.EKNO, TASSICILNOOZELLIK.GIAI";

            //TMM.IsKurallari.Genel.GorSicilNoGetir tek tek sicilleri sql'e sorarak getirdiği için yerine burası eklenedi.
            sql += ",(SELECT SICILNO FROM TASSICILNOHAREKET A  WHERE A.DURUM=1 AND A.SIRANO=(SELECT MAX(B.SIRANO) FROM TASSICILNOHAREKET B WHERE B.PRSICILNO=A.PRSICILNO AND A.MUHASEBEKOD=B.MUHASEBEKOD AND A.HARCAMABIRIMKOD=B.HARCAMABIRIMKOD) AND (A.MUHASEBEKOD = TASZIMMETDETAY.MUHASEBEKOD) AND (A.HARCAMABIRIMKOD = TASZIMMETDETAY.HARCAMABIRIMKOD) AND (A.PRSICILNO = TASZIMMETDETAY.SICILNO)) AS GORSICILNO ";

            sql += ", ONCEKIYER.NEREYEGITTI, ONCEKIYER.NEREYEGITTIAD";

            //sql += " FROM TASHESAPPLAN RIGHT OUTER JOIN";
            //sql += " TASSICILNOOZELLIK RIGHT OUTER JOIN";
            //sql += " TASZIMMETDETAY ON TASSICILNOOZELLIK.PRSICILNO = TASZIMMETDETAY.SICILNO ON TASHESAPPLAN.KOD = TASZIMMETDETAY.HESAPPLANKOD";
            sql += " FROM TASZIMMETDETAY LEFT OUTER JOIN";
            sql += " TASMODEL RIGHT OUTER JOIN";
            sql += " TASSICILNOOZELLIK ON TASMODEL.KOD = TASSICILNOOZELLIK.MODELKOD ON TASZIMMETDETAY.SICILNO = TASSICILNOOZELLIK.PRSICILNO LEFT OUTER JOIN";
            sql += " TASHESAPPLAN ON TASZIMMETDETAY.HESAPPLANKOD = TASHESAPPLAN.KOD LEFT OUTER JOIN";
            sql += " TASMARKA ON TASSICILNOOZELLIK.MARKAKOD = TASMARKA.KOD";


            //Eski zimmet verilen yer bilgisi
            //Düzeltme: Zimmet formunda çoklama oluyor Meclis 03.10.2023 melih
            //LEFT OUTER JOIN TASODA ON A.NEREYEGITTI = TASODA.KOD --olan satır
            //LEFT OUTER JOIN TASODA ON A.NEREYEGITTI = TASODA.KOD AND TASODA.HARCAMABIRIMKOD = A.HARCAMABIRIMKOD --olarak düzeltildi.

            string eskiZimmetSql = @"
            LEFT OUTER JOIN (SELECT A.MUHASEBEKOD, A.HARCAMABIRIMKOD, A.PRSICILNO,  A.FISNO, A.NEREYEGITTI, TASODA.AD AS NEREYEGITTIAD FROM TASSICILNOHAREKET A 
                                LEFT OUTER JOIN TASODA ON A.NEREYEGITTI = TASODA.KOD AND TASODA.HARCAMABIRIMKOD=A.HARCAMABIRIMKOD 
                                WHERE A.DURUM = 1 
					            AND A.SIRANO = (SELECT MAX(B.SIRANO) FROM TASSICILNOHAREKET B WHERE B.PRSICILNO = A.PRSICILNO AND A.ISLEMTURUKOD = B.ISLEMTURUKOD AND B.SIRANO < (SELECT B.SIRANO FROM TASSICILNOHAREKET B WHERE B.PRSICILNO=A.PRSICILNO AND B.YIL = {YIL} AND B.MUHASEBEKOD = {MUHASEBEKOD} AND B.HARCAMABIRIMKOD= {HARCAMABIRIMKOD} AND B.FISNO = {FISNO} AND A.ISLEMTURUKOD = B.ISLEMTURUKOD) ) 
					            AND (A.ISLEMTURUKOD = 90) 
				            ) AS ONCEKIYER ON (ONCEKIYER.PRSICILNO = TASZIMMETDETAY.SICILNO)  
            ";

            eskiZimmetSql = eskiZimmetSql.Replace("{YIL}", VeriErisim.DegerYap(zd.yil));
            eskiZimmetSql = eskiZimmetSql.Replace("{MUHASEBEKOD}", VeriErisim.DegerYap(zd.muhasebeKod));
            eskiZimmetSql = eskiZimmetSql.Replace("{HARCAMABIRIMKOD}", VeriErisim.DegerYap(zd.harcamaBirimKod.Replace(".", "")));
            eskiZimmetSql = eskiZimmetSql.Replace("{FISNO}", VeriErisim.DegerYap(zd.fisNo));

            sql += eskiZimmetSql;


            //Fişe ait bütün satırlar
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", zd.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", zd.fisNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", zd.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", zd.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", zd.belgeTur));

            //Tek satır
            if (zd.siraNo != 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", zd.siraNo));

            sql += " ORDER BY " + TABLOAD + ".SIRANO";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
