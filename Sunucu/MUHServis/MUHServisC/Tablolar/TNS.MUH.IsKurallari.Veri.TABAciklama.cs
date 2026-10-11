using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    public class TABAciklama : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAciklama(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "MUHACIKLAMA";

        public enum ENUMAlanlar
        {
            KOD = 0,
            MUHASEBEKOD = 1,
            BIRIMKOD = 2,
            TIPI = 3,
            ADI = 4,
            ACIKLAMA = 5,
            KAYITEDEN = 6,
            PAYLASIM = 7
        }
        static string[] ALANLAR = { "KOD", "MUHASEBEKOD", "BIRIMKOD", "TIPI", "ADI", "ACIKLAMA", "KAYITEDEN", "PAYLASIM" };

        public Sonuc Kaydet(AciklamaBilgi bilgi)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(bilgi.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.muhasebeKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.birimKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.tipi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.adi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.aciklama) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.kayitEden) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.paylasilan) + ") ";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(AciklamaBilgi bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], bilgi.adi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(AciklamaBilgi bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (bilgi.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebeKod));

            if (bilgi.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.muhasebeKod));

            if (bilgi.kayitEden != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYITEDEN], "=", bilgi.kayitEden));

            if (bilgi.paylasilan > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PAYLASIM], "=", bilgi.paylasilan));

            if (bilgi.tipi > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIPI], "=", bilgi.tipi));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(AciklamaBilgi bilgi)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

    }
}