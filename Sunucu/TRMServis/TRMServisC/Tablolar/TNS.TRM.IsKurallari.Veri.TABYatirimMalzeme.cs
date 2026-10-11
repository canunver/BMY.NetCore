using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABYatirimMalzeme : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYatirimMalzeme(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Malzeme grubu bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TRMYATIRIMMALZEME";

        /// <summary>
        /// Malzeme grubu bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            HIBEKOD = 1,
            MALZEMETURKOD = 2,
            MARKA = 3,
            MODEL = 4,
            OZELLIK = 5,
            TUTAR = 6,
            MALZEMEKOD = 7,
            REFERANSTUTAR = 8,
            MIKTAR = 9

        }

        /// <summary>
        /// Malzeme grubu tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "HIBEKOD", "MALZEMETURKOD", "MARKA", "MODEL", "OZELLIK", "TUTAR", "MALZEMEKOD", "REFERANSTUTAR", "MIKTAR" };

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(YatirimMalzeme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MALZEMETURKOD], bilgi.malzemeTurKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MARKA], bilgi.marka) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MODEL], bilgi.model) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OZELLIK], bilgi.ozellik) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUTAR], bilgi.tutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MALZEMEKOD], bilgi.malzeme.kod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MIKTAR], bilgi.malzeme.miktar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.REFERANSTUTAR], bilgi.referansTutar);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Kaydet(YatirimMalzeme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.malzemeTurKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.marka) + ",";
            sql += VeriErisim.DegerYap(bilgi.model) + ",";
            sql += VeriErisim.DegerYap(bilgi.ozellik) + ",";
            sql += VeriErisim.DegerYap(bilgi.tutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.malzeme.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.referansTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.miktar) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(YatirimMalzeme bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", bilgi.hibeKod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilHibeKoduyla(int hibeKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hibeKod));

            return KomutCalistir(sql, 0);
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
        public IDataReader Listele(YatirimMalzeme bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",TRMYATIRIMMALZEMETUR.ADI, TRMHIBEYATIRIM.ETAPNO, TRMHIBEYATIRIM.BASVURUNO, TRMHIBEYATIRIM.KIMLIKNO, TRMHIBEYATIRIM.AD, TRMHIBEYATIRIM.SOYAD, ORGCOGRAFYA.AD, TRMHIBEYATIRIM.PROJEADI, TRMYATIRIMNITELIK.ADI, TRMYATIRIMKONU.ADI,TRMYATIRIMMALZEMETANIM.ADI, TRMYATIRIMMALZEMEGRUPANA.KOD, TRMYATIRIMMALZEMEGRUPANA.ADI,TRMYATIRIMMALZEMETANIM.OZELLIK";
            sql += " FROM TRMYATIRIMMALZEMEGRUPANA ";
            sql += " LEFT OUTER JOIN TRMYATIRIMMALZEMETUR ON TRMYATIRIMMALZEMEGRUPANA.KOD = TRMYATIRIMMALZEMETUR.ANAKOD ";
            sql += " RIGHT OUTER JOIN TRMYATIRIMMALZEME ";
            sql += " INNER JOIN TRMHIBEYATIRIM ON TRMYATIRIMMALZEME.HIBEKOD = TRMHIBEYATIRIM.KOD ";
            sql += " LEFT OUTER JOIN ORGCOGRAFYA ON TRMHIBEYATIRIM.ILKOD = ORGCOGRAFYA.KOD ";
            sql += " LEFT OUTER JOIN TRMYATIRIMKONU ON TRMHIBEYATIRIM.PROJEKONU = TRMYATIRIMKONU.KOD ";
            sql += " LEFT OUTER JOIN TRMYATIRIMNITELIK ON TRMHIBEYATIRIM.PROJENITELIK = TRMYATIRIMNITELIK.KOD ON TRMYATIRIMMALZEMETUR.KOD = TRMYATIRIMMALZEME.MALZEMETURKOD ";
            sql += " LEFT OUTER JOIN TRMYATIRIMMALZEMETANIM ON TRMYATIRIMMALZEME.MALZEMEKOD = TRMYATIRIMMALZEMETANIM.KOD";

            if (bilgi.sorguDurum > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "DURUM", "=", bilgi.sorguDurum));
            }
            else
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "DURUM", ">=", 1));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "DURUM", "<", 90));
            }

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (bilgi.hibeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", bilgi.hibeKod));

            if (bilgi.malzemeAnaGrupKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMYATIRIMMALZEMETUR", "ANAKOD", "=", bilgi.malzemeAnaGrupKod));

            if (bilgi.malzemeTurKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALZEMETURKOD], "=", bilgi.malzemeTurKod));

            if (bilgi.malzeme.kod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALZEMEKOD], "=", bilgi.malzeme.kod));

            if (bilgi.hibe.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ETAPNO", "=", bilgi.hibe.etapNo));

            if (bilgi.hibe.ilKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ILKOD", "=", bilgi.hibe.ilKod));

            if (bilgi.hibe.projeNitelik > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "PROJENITELIK", "=", bilgi.hibe.projeNitelik));

            if (bilgi.hibe.projeKonu != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "PROJEKONU", "=", bilgi.hibe.projeKonu));


            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY TRMHIBEYATIRIM.BASVURUNO," + ALANLAR[(int)ENUMAlanlar.MALZEMETURKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleIstatistik(YatirimMalzeme bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT TRMHIBEYATIRIM.ETAPNO, ORGCOGRAFYA.AD, TRMYATIRIMMALZEMETUR.ADI, SUM(TRMYATIRIMMALZEME.TUTAR), COUNT(*) ";
            sql += " FROM TRMYATIRIMMALZEME ";
            sql += " INNER JOIN TRMHIBEYATIRIM ON TRMYATIRIMMALZEME.HIBEKOD = TRMHIBEYATIRIM.KOD ";
            sql += " LEFT OUTER JOIN ORGCOGRAFYA ON TRMHIBEYATIRIM.ILKOD = ORGCOGRAFYA.KOD";
            sql += " LEFT OUTER JOIN TRMYATIRIMMALZEMETUR ON TRMYATIRIMMALZEME.MALZEMETURKOD = TRMYATIRIMMALZEMETUR.KOD";

            if (bilgi.sorguDurum > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "DURUM", "=", bilgi.sorguDurum));
            }
            else
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "DURUM", ">=", 1));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "DURUM", "<", 90));
            }

            if (bilgi.malzemeAnaGrupKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMYATIRIMMALZEMETUR", "ANAKOD", "=", bilgi.malzemeAnaGrupKod));

            if (bilgi.malzemeTurKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALZEMETURKOD], "=", bilgi.malzemeTurKod));

            if (bilgi.hibe.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ETAPNO", "=", bilgi.hibe.etapNo));

            if (bilgi.hibe.ilKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ILKOD", "=", bilgi.hibe.ilKod));

            if (bilgi.hibe.projeNitelik > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "PROJENITELIK", "=", bilgi.hibe.projeNitelik));

            if (bilgi.hibe.projeKonu != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "PROJEKONU", "=", bilgi.hibe.projeKonu));


            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " GROUP BY TRMHIBEYATIRIM.ETAPNO, ORGCOGRAFYA.AD, TRMYATIRIMMALZEMETUR.ADI";

            sql += " ORDER BY TRMHIBEYATIRIM.ETAPNO, ORGCOGRAFYA.AD, TRMYATIRIMMALZEMETUR.ADI";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
