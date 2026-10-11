using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABKoy : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKoy(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Koy bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TRMKOY";

        /// <summary>
        /// Koy bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            ILKOD = 1,
            ILCEKOD = 2,
            ILMERNISNO = 3,
            ILCEMERNISNO = 4,
            BUCAKMERNISNO = 5,
            KOYMERNISNO = 6,
            ADI = 7,
            KOORDINAT = 8,
            KAPALI = 9,
        }

        /// <summary>
        /// Köy tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "ILKOD", "ILCEKOD", "ILMERNISNO", "ILCEMERNISNO", "BUCAKMERNISNO", "KOYMERNISNO", "ADI", "KOORDINAT", "KAPALI" };

        /// <summary>
        /// Koy bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(Koy bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILMERNISNO], bilgi.ilMernisNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILCEMERNISNO], bilgi.ilceMernisNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BUCAKMERNISNO], bilgi.bucakMernisNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOYMERNISNO], bilgi.koyMernisNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], bilgi.adi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], bilgi.ilKod.Replace(".", "")) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILCEKOD], bilgi.ilceKod.Replace(".", "")) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOORDINAT], bilgi.koordinat) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAPALI], bilgi.kapali);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.ilKod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(bilgi.ilceKod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(bilgi.ilMernisNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.ilceMernisNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.bucakMernisNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.koyMernisNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.adi) + ",";
                sql += VeriErisim.DegerYap(bilgi.koordinat) + ",";
                sql += VeriErisim.DegerYap(bilgi.kapali) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc KoyleriKapat(int ilceMernisKod, string ilceKod)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAPALI], 1);
            if (ilceMernisKod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILCEMERNISNO], "=", ilceMernisKod));
            if (ilceKod != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILCEKOD], "=", ilceKod));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Koy bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(Koy bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Koy bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Koy bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(Koy bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",ORGCOGRAFYA.AD, ORGCOGRAFYA2.AD" + " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN ORGCOGRAFYA AS ORGCOGRAFYA2 ON " + TABLOAD + ".ILCEKOD = ORGCOGRAFYA2.KOD LEFT OUTER JOIN ORGCOGRAFYA ON " + TABLOAD + ".ILKOD = ORGCOGRAFYA.KOD";

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (bilgi.adi != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ADI], " LIKE ", "%" + bilgi.adi + "%"));

            if (bilgi.koordinat != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOORDINAT], " LIKE ", "%" + bilgi.koordinat + "%"));

            if (bilgi.ilKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILKOD], "=", bilgi.ilKod.Replace(".", "")));

            if (bilgi.ilceKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILCEKOD], "=", bilgi.ilceKod.Replace(".", "")));

            if (bilgi.ilMernisNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILMERNISNO], "=", bilgi.ilMernisNo));

            if (bilgi.ilceMernisNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILCEMERNISNO], "=", bilgi.ilceMernisNo));

            if (bilgi.koyMernisNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOYMERNISNO], "=", bilgi.koyMernisNo));

            if (bilgi.kapali > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAPALI], "=", bilgi.kapali));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ADI];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader CografyaListele(string kod, bool sadeceDetay)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT * FROM ORGCOGRAFYA";

            if (kod != "")
            {
                string baglac = "";
                if (sadeceDetay)
                    baglac = "%";
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("ORGCOGRAFYA", "KOD", "LIKE", kod.Replace(".", "") + baglac));
            }
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY AD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tabloda kullanılan en büyük kod numarasını döndürür
        /// </summary>
        /// <returns>Verilen kriter bilgisine uygun kayıt bulunursa true, yoksa false döndürür</returns>
        public int EnBuyukKod()
        {
            string sql = "";

            sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")";
            sql += " FROM " + TABLOAD;

            int deger = veriErisim.TekKolonSQLInt(sql);

            return deger + 1;
        }
    }
}
