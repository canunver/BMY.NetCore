using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABYatirimMalzemeTanim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYatirimMalzemeTanim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Malzeme grubu bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TRMYATIRIMMALZEMETANIM";

        /// <summary>
        /// Malzeme grubu bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            MALZEMETURKOD = 1,
            ADI = 2,
            AKTIF = 3,
            REFERENSFIYAT = 4,
            OZELLIK = 5,
            MIKTAR = 6,
        }

        /// <summary>
        /// Malzeme grubu tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "MALZEMETURKOD", "ADI", "AKTIF", "REFERENSFIYAT", "OZELLIK", "MIKTAR" };

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(YatirimMalzemeTanim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MALZEMETURKOD], bilgi.tur.kod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], bilgi.ad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKTIF], bilgi.aktif) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.REFERENSFIYAT], bilgi.referansFiyat) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MIKTAR], bilgi.miktar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OZELLIK], bilgi.ozellik);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.tur.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.ad) + ",";
                sql += VeriErisim.DegerYap(bilgi.aktif) + ",";
                sql += VeriErisim.DegerYap(bilgi.referansFiyat) + ",";
                sql += VeriErisim.DegerYap(bilgi.ozellik) + ",";
                sql += VeriErisim.DegerYap(bilgi.miktar) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(YatirimMalzemeTanim bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Malzeme grubu bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(YatirimMalzemeTanim bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",TRMYATIRIMMALZEMETUR.ADI AS TADI,TRMYATIRIMMALZEMEGRUPANA.KOD,TRMYATIRIMMALZEMEGRUPANA.ADI FROM " + TABLOAD;
            sql += " INNER JOIN  TRMYATIRIMMALZEMETUR ON TRMYATIRIMMALZEMETANIM.MALZEMETURKOD = TRMYATIRIMMALZEMETUR.KOD";
            sql += " INNER JOIN  TRMYATIRIMMALZEMEGRUPANA ON TRMYATIRIMMALZEMETUR.ANAKOD = TRMYATIRIMMALZEMEGRUPANA.KOD";

            if (bilgi.kod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (bilgi.tur.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALZEMETURKOD], "=", bilgi.tur.kod));

            if (bilgi.referansFiyat > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.REFERENSFIYAT], "=", bilgi.referansFiyat));

            if (!string.IsNullOrEmpty(bilgi.ozellik))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.OZELLIK], "=", bilgi.ozellik));

            if (!string.IsNullOrEmpty(bilgi.ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ADI], " LIKE ", "%" + bilgi.ad + "%"));

            if (bilgi.aktif > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AKTIF], "=", bilgi.aktif));

            if (bilgi.miktar > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MIKTAR], "=", bilgi.miktar));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY TRMYATIRIMMALZEMEGRUPANA.ADI, TRMYATIRIMMALZEMETUR.ADI," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ADI];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
