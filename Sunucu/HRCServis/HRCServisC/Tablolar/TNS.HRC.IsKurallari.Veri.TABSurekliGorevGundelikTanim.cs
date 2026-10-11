using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Yolluk gündelik tanım bilgisini tutan PSLSUREKLIGOREVGUNDELIKTANIM veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABSurekliGorevGundelikTanim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSurekliGorevGundelikTanim(VeriErisim veriErisim)
            : base(veriErisim)
        {
            if (veriErisim.baglantiTip == "ORACLE")
                TABLOAD = "PSLSURGORGUNTNM";
        }

        /// <summary>
        /// Yolluk gündelik tanım bilgisi tablo adını tutan sabit değişken
        /// </summary>

        public static string TABLOAD = "PSLSUREKLIGOREVGUNDELIKTANIM";

        /// <summary>
        /// Yolluk gündelik tanım bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 1,
            /// <summary>
            /// DEGER alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DEGER = 2,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 3,
            /// <summary>
            /// TUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUR = 4,
            /// <summary>
            /// AYKADRODERECEBASLANGIC alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AYKADRODERECEBASLANGIC = 5,
            /// <summary>
            /// AYKADRODERECEBITIS alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AYKADRODERECEBITIS = 6,
            /// <summary>
            /// EKGOSTERGEBASLANGIC alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKGOSTERGEBASLANGIC = 7,
            /// <summary>
            /// EKGOSTERGEBITIS alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKGOSTERGEBITIS = 8,
            /// <summary>
            /// ULKEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ULKEKOD = 9,
            /// <summary>
            /// PARABIRIMIKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PARABIRIMIKOD = 10,
        }

        /// <summary>
        /// Yolluk gündelik tanım tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "KOD", "DEGER", "AD", "TUR", "AYKADRODERECEBASLANGIC", "AYKADRODERECEBITIS", "EKGOSTERGEBASLANGIC", "EKGOSTERGEBITIS", "ULKEKOD", "PARABIRIMIKOD" };

        /// <summary>
        /// Yolluk gündelik tanım bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADPARABIRIMI = "HRCPARABIRIMI";

        /// <summary>
        /// Yolluk gündelik tanım bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarParaBirimi : int
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
        /// Yolluk gündelik tanım tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARPARABIRIMI = { "KOD", "ADI", "KISALTMA" };

        /// <summary>
        /// Yolluk gündelik tanım bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADULKE = "HRCULKE";

        /// <summary>
        /// Yolluk gündelik tanım bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarUlke : int
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 1,
        }

        /// <summary>
        /// Yolluk gündelik tanım tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARULKE = { "KOD", "AD" };

        /// <summary>
        /// Yolluk gündelik tanım bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="gt">Eklenecek yolluk gündelik tanım bilgisini tutan nesne</param>
        /// <param name="zorunluYeni">Kayıt varsa hata verir</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(YollukGundelikTanim gt, bool zorunluYeni)
        {
            string sql = "SELECT YIL, KOD FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", gt.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", gt.kod));

            IDataReader dr = veriErisim.DataReaderDondur(sql);

            bool sonuc = dr.Read();
            dr.Close();

            if (sonuc) //Kayıt var, güncelle
            {
                if (zorunluYeni)
                    return new Sonuc($"{gt.yil}-{gt.kod} zaten tanımlı");
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGER], gt.deger) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], gt.ad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], (int)gt.tur) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AYKADRODERECEBASLANGIC], gt.ayKadroDereceBaslangic) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AYKADRODERECEBITIS], gt.ayKadroDereceBitis) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKGOSTERGEBASLANGIC], gt.ekGostergeBaslangic) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKGOSTERGEBITIS], gt.ekGostergeBitis) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ULKEKOD], gt.ulke.kod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARABIRIMIKOD], gt.paraBirimi.kod);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", gt.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", gt.kod));
            }
            else //Kayıt yok, ekle
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(gt.yil) + ",";
                sql += VeriErisim.DegerYap(gt.kod) + ",";
                sql += VeriErisim.DegerYap(gt.deger) + ",";
                sql += VeriErisim.DegerYap(gt.ad) + ",";
                sql += VeriErisim.DegerYap((int)gt.tur) + ",";
                sql += VeriErisim.DegerYap(gt.ayKadroDereceBaslangic) + ",";
                sql += VeriErisim.DegerYap(gt.ayKadroDereceBitis) + ",";
                sql += VeriErisim.DegerYap(gt.ekGostergeBaslangic) + ",";
                sql += VeriErisim.DegerYap(gt.ekGostergeBitis) + ",";
                sql += VeriErisim.DegerYap(gt.ulke.kod) + ",";
                sql += VeriErisim.DegerYap(gt.paraBirimi.kod) + ")";
            }

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Yolluk gündelik tanım bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="gt">Listeleme için kullanılacak yolluk gündelik tanım bilgisi kıstas parametresi</param>
        /// <returns>
        /// Yolluk gündelik tanım bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(YollukGundelikTanim gt)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," +
                        TABLOADPARABIRIMI + "." + ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.KISALTMA] + "," +
                        TABLOADULKE + "." + ALANLARULKE[(int)ENUMAlanlarUlke.AD] + "," +
                        TABLOADPARABIRIMI + "." + ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.ADI] +
                        " FROM " + TABLOAD +
                        " LEFT OUTER JOIN " + TABLOADPARABIRIMI + " ON " +
                        TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PARABIRIMIKOD] + "=" + TABLOADPARABIRIMI + "." + ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.KOD] +
                        " LEFT OUTER JOIN " + TABLOADULKE + " ON " +
                        TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ULKEKOD] + "=" + TABLOADULKE + "." + ALANLARULKE[(int)ENUMAlanlarUlke.KOD];

            if (gt != null)
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", gt.yil));

                if (gt.kod != string.Empty)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", gt.kod));

                if (gt.ad != string.Empty)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", gt.ad + "%"));

                if (gt.tur > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", (int)gt.tur));
            }
            sql += " ORDER BY 1,2,4,3";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yolluk gündelik tanım bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="gt">Silinecek yolluk gündelik tanım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(YollukGundelikTanim gt)
        {
            string sql = "DELETE FROM " + TABLOAD;
            if (gt.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", gt.yil));
            if (!string.IsNullOrEmpty(gt.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", gt.kod));
            else
            {
                if (gt.tur != ENUMYollukGundelikTuru.TANIMSIZ)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", (int)gt.tur));
            }

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Yolluk gündelik bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, doube döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl bilgisi kıstas parametresi</param>
        /// <param name="gKod">Listeleme için kullanılacak gündelik kod bilgisi kıstas parametresi</param>
        /// <returns>
        /// Yolluk gündelik bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// double nesnesini döndürür
        /// </returns>
        public double GundelikDegerAl(int yil, string gKod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.DEGER] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", gKod));

            return veriErisim.TekKolonSQLDbl(sql);
        }
    }
}
