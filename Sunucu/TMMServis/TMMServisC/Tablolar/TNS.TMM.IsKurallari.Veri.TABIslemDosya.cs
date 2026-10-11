using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    public class TABIslemDosya : TABOrtak
    {
        public TABIslemDosya(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "TASISLEMDOSYA";
        static string[] ALANLAR = { "DOSYAID", "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO", "SIRANO", "RESIM", "ADI", "BOYUTU", "EKLEYENKISIAD", "EKLEYENKISIKOD", "EKLEMETARIHI" };

        public enum ENUMAlanlar
        {
            DOSYAID = 0,
            YIL = 1,
            MUHASEBEKOD = 2,
            HARCAMABIRIMKOD = 3,
            FISNO = 4,
            SIRANO = 5,
            RESIM = 6,
            ADI = 7,
            BOYUTU = 8,
            EKLEYENKISIAD = 9,
            EKLEYENKISIKOD = 10,
            EKLEMETARIHI = 11
        }

        /// <summary>
        /// İlgili tabloya verilen bilgileri kayıt işlemini yapar.
        /// </summary>
        /// <param name="bilgi">İşlem yapılacak bilgileri içerir</param>
        /// <returns>Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.</returns>
        public Sonuc Kaydet(IslemDosya bilgi)
        {
            string imgStr = "";

            if (bilgi.resim != null && bilgi.resim.Length > 0)
                imgStr = "0x" + System.BitConverter.ToString(bilgi.resim).Replace("-", string.Empty);
            else
                imgStr = "null";

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(bilgi.dosyaKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.yil) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.muhasebeKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.harcamaBirimKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.fisNo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.siraNo) + ", ";
            sql = sql + imgStr + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.adi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.boyutu) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.ekleyenKisiAdi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.ekleyenKisiKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.eklemeTarihi) + ") ";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// İlgili tabloya verilen bilgileri güncelleme işlemini yapar.
        /// </summary>
        /// <param name="bilgi">İşlem yapılacak bilgileri içerir</param>
        /// <returns>Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.</returns>
        public Sonuc Guncelle(IslemDosya bilgi)
        {
            string imgStr = "";
            if (bilgi.resim != null && bilgi.resim.Length > 0)
                imgStr = "0x" + System.BitConverter.ToString(bilgi.resim).Replace("-", string.Empty);
            else
                imgStr = "null";

            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += ALANLAR[(int)ENUMAlanlar.RESIM] + "=" + imgStr + " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAID], "=", bilgi.dosyaKod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre ilgili tablodan listeleme işlemini yapar.
        /// </summary>
        /// <param name="bilgi">İşlem yapılacak bilgileri içerir</param>
        /// <returns>IDataReader yapısı içinde, SQL çalışması sonucunda oluşan verileri döndürür.</returns>
        public IDataReader Getir(IslemDosya bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT *";
            sql += " FROM " + TABLOAD;
            if (!string.IsNullOrEmpty(bilgi.fisNo))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebeKod));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", bilgi.harcamaBirimKod));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", bilgi.fisNo));
            }
            if (bilgi.dosyaKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAID], "=", bilgi.dosyaKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.DOSYAID];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// İlgili tablodan verilen bilgilere ait verileri silme işlemini yapar.
        /// </summary>
        /// <param name="bilgi">İşlem yapılacak bilgileri içerir</param>
        /// <returns>Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.</returns>
        public Sonuc Sil(IslemDosya bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAID], "=", bilgi.dosyaKod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen kriterlere ait bilgilerin ilgili tabloda kullanımını kontrol eder.
        /// </summary>
        /// <param name="bilgi">İşlem yapılacak bilgileri içerir</param>
        /// <returns>Verilen kriter bilgisine uygun kayıt bulunursa true, yoksa false döndürür</returns>
        public int SiraNoVer(IslemDosya bilgi)
        {
            string sql = "";

            sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.SIRANO] + ") ";
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", bilgi.fisNo));

            return this.veriErisim.TekKolonSQLInt(sql) + 1;
        }

        private int KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.DOSYAID] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

    }
}