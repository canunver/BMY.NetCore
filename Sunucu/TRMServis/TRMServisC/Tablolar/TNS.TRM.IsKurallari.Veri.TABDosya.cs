using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{
    public class TABDosya : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABDosya(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Kaynak Türü  bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TRMHIBEDOSYA";

        /// <summary>
        /// Kaynak Türü bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            ADI = 1,
            TARIH = 2,
            BOYUT = 3,
            TIP = 4,
            TUR = 5,
            KAYITNO = 6,
            PREVRAKNO = 7,
            KAYITEDEN = 8,
            KAYITTARIHI = 9,
        }

        /// <summary>
        /// Kaynak Türü tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "ADI", "TARIH", "BOYUT", "TIP", "TUR", "KAYITNO", "PREVRAKNO", "KAYITEDEN", "KAYITTARIHI" };

        /// <summary>
        /// Kaynak Türü  bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(EklenenDosya bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            veriErisim.hata = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ad) + ",";
            sql += VeriErisim.DegerYap(bilgi.dosyaTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.boyut) + ",";
            sql += VeriErisim.DegerYap(bilgi.tip) + ",";
            sql += VeriErisim.DegerYap(bilgi.tur) + ",";
            sql += VeriErisim.DegerYap(bilgi.kayitNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.evrakNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.kayitEden) + ",";
            sql += VeriErisim.DegerYap(bilgi.kayitTarihi) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(string kod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

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
        public IDataReader Listele(EklenenDosya bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (!string.IsNullOrEmpty(bilgi.tip))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], " = ", bilgi.tip));

            if (!string.IsNullOrEmpty(bilgi.kayitEden))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYITEDEN], "=", bilgi.kayitEden));

            if (!string.IsNullOrEmpty(bilgi.kayitNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYITNO], "=", bilgi.kayitNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KAYITTARIHI] + "," + ALANLAR[(int)ENUMAlanlar.TUR];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
