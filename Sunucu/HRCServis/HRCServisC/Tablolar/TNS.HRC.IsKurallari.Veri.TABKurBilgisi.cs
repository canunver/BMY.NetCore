using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Kur bilgisini tutan HRCKURBILGISI veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABKurBilgisi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKurBilgisi(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Kur bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCKURBILGISI";

        /// <summary>
        /// Kur bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 3,
            /// <summary>
            /// PARABIRIMIKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PARABIRIMIKOD = 4,
            /// <summary>
            /// KUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KUR = 5,
            /// <summary>
            /// TIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TIP = 6,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 7,
        }

        /// <summary>
        /// Kur bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR ={ "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "PARABIRIMIKOD", "KUR", "TIP", "TCKIMLIKNO" };

        /// <summary>
        /// Para birimi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADPARABIRIMI = "HRCPARABIRIMI";

        /// <summary>
        /// Para birimi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarParaBirimi
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// ADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADI = 1,
            /// <summary>
            /// KISALTMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KISALTMA = 2,
        }

        /// <summary>
        /// Para birimi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARPARABIRIMI ={ "KOD", "ADI", "KISALTMA" };

        /// <summary>
        /// Kur bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek kur bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Kaydet(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(((KurBilgisi)bilgi.objeler[i]).yil) + ",";
                sql += VeriErisim.DegerYap(((KurBilgisi)bilgi.objeler[i]).muhasebe) + ",";
                sql += VeriErisim.DegerYap(((KurBilgisi)bilgi.objeler[i]).birim) + ",";
                sql += VeriErisim.DegerYap(((KurBilgisi)bilgi.objeler[i]).surecNo) + ",";
                sql += VeriErisim.DegerYap(((KurBilgisi)bilgi.objeler[i]).paraBirimi.kod) + ",";
                sql += VeriErisim.DegerYap(((KurBilgisi)bilgi.objeler[i]).kur) + ",";
                sql += VeriErisim.DegerYap((int)((KurBilgisi)bilgi.objeler[i]).tip) + ",";
                sql += VeriErisim.DegerYap(((KurBilgisi)bilgi.objeler[i]).tckimlikno) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Kur bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek kur bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Sil(KurBilgisi bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (bilgi.yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (bilgi.muhasebe)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (bilgi.birim)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (bilgi.surecNo)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", (int)(bilgi.tip)));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Kur bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak kur bilgisi kıstas parametresi</param>
        /// <returns>
        /// Kur bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(KurBilgisi bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) +
              "," + TABLOADPARABIRIMI + "." + ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.ADI] +
              "," + TABLOADPARABIRIMI + "." + ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.KISALTMA];

            sql += " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN " + TABLOADPARABIRIMI + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PARABIRIMIKOD] + "=" + TABLOADPARABIRIMI + "." + ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.KOD];

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.paraBirimi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PARABIRIMIKOD], "=", bilgi.paraBirimi.kod));
            if (bilgi.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", (int)bilgi.tip));
            if (bilgi.tckimlikno != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", bilgi.tckimlikno));
            //else
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", "NULL"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL] + ", " + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + ALANLAR[(int)ENUMAlanlar.BIRIMKOD];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}