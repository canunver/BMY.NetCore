using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    public class TABTasinirResim : TABOrtak
    {
        public TABTasinirResim(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "TASRESIM";
        static string[] ALANLAR = { "RESIMID", "PRSICILNO", "SIRANO", "RESIM", "ADI", "BOYUTU", "EKLEYENKISIAD", "EKLEYENKISIKOD", "EKLEMETARIHI" };

        public enum ENUMAlanlar
        {
            RESIMID = 0,
            PRSICILNO = 1,
            SIRANO = 2,
            RESIM = 3,
            ADI = 4,
            BOYUTU = 5,
            EKLEYENKISIAD = 6,
            EKLEYENKISIKOD = 7,
            EKLEMETARIHI = 8,

        }

        /// <summary>
        /// İlgili tabloya verilen bilgileri kayıt işlemini yapar.
        /// </summary>
        /// <param name="resim">İşlem yapılacak bilgileri içerir</param>
        /// <returns>Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.</returns>
        public Sonuc Kaydet(TasResim resim)
        {
            string imgStr = "";

            if (resim.resim != null && resim.resim.Length > 0)
                imgStr = "0x" + System.BitConverter.ToString(resim.resim).Replace("-", string.Empty);
            else
                imgStr = "null";

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(resim.resimID) + ", ";
            sql = sql + VeriErisim.DegerYap(resim.prSicilNo) + ", ";
            sql = sql + VeriErisim.DegerYap(resim.siraNo) + ", ";
            sql = sql + imgStr + ", ";
            sql = sql + VeriErisim.DegerYap(resim.adi) + ", ";
            sql = sql + VeriErisim.DegerYap(resim.boyutu) + ", ";
            sql = sql + VeriErisim.DegerYap(resim.ekleyenKisiAdi) + ", ";
            sql = sql + VeriErisim.DegerYap(resim.ekleyenKisiKod) + ", ";
            sql = sql + VeriErisim.DegerYap(resim.eklemeTarihi) + ") ";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// İlgili tabloya verilen bilgileri güncelleme işlemini yapar.
        /// </summary>
        /// <param name="resim">İşlem yapılacak bilgileri içerir</param>
        /// <returns>Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.</returns>
        public Sonuc Guncelle(TasResim resim)
        {
            string imgStr = "";
            if (resim.resim != null && resim.resim.Length > 0)
                imgStr = "0x" + System.BitConverter.ToString(resim.resim).Replace("-", string.Empty);
            else
                imgStr = "null";

            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += ALANLAR[(int)ENUMAlanlar.RESIM] + "=" + imgStr + " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.RESIMID], "=", resim.resimID));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre ilgili tablodan listeleme işlemini yapar.
        /// </summary>
        /// <param name="tcKimklikNo">İşlem yapılacak bilgileri içerir</param>
        /// <returns>IDataReader yapısı içinde, SQL çalışması sonucunda oluşan verileri döndürür.</returns>
        public IDataReader Getir(int prSicilNo, string resimID)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT *";
            sql += " FROM " + TABLOAD;
            if (prSicilNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicilNo));
            if (!string.IsNullOrEmpty(resimID))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.RESIMID], "=", resimID));
            sql += " WHERE " + kosul;
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.RESIMID];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// İlgili tablodan verilen bilgilere ait verileri silme işlemini yapar.
        /// </summary>
        /// <param name="resim">İşlem yapılacak bilgileri içerir</param>
        /// <returns>Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.</returns>
        public Sonuc Sil(TasResim resim)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.RESIMID], "=", resim.resimID));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen kriterlere ait bilgilerin ilgili tabloda kullanımını kontrol eder.
        /// </summary>
        /// <param name="resim">İşlem yapılacak bilgileri içerir</param>
        /// <returns>Verilen kriter bilgisine uygun kayıt bulunursa true, yoksa false döndürür</returns>
        public int SiraNoVer(TasResim resim)
        {
            string sql = "";

            sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.SIRANO] + ") ";
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", resim.prSicilNo));

            return this.veriErisim.TekKolonSQLInt(sql) + 1;
        }

        private int KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.RESIMID] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

    }
}