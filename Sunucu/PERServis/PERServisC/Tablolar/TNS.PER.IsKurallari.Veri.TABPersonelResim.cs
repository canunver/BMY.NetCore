using System;
using System.Data;
using TNS.PER.IsKurallari;
using TNS.Veri;

namespace TNS.PER.IsKurallari.Veri
{
    public class TABPersonelResim : TABOrtak
    {
        public TABPersonelResim(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "PERPERSONELRESIM";
        static string[] ALANLAR = { "TCKIMLIKNO", "RESIM" };
        public enum ENUMAlanlar
        {
            TCKIMLIKNO = 0,
            RESIM = 1,
        }

        /// <summary>
        /// İlgili tabloya verilen bilgileri kayıt işlemini yapar.
        /// </summary>
        /// <param name="resim">İşlem yapılacak bilgileri içerir</param>
        /// <returns>Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.</returns>
        public Sonuc Kaydet(PersonelResim resim)
        {
            string imgStr = "0x" + System.BitConverter.ToString(resim.resim).Replace("-", string.Empty);
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(resim.tcKimlikNo) + ", ";
            sql = sql + imgStr + ") ";

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// İlgili tabloya verilen bilgileri güncelleme işlemini yapar.
        /// </summary>
        /// <param name="resim">İşlem yapılacak bilgileri içerir</param>
        /// <returns>Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.</returns>
        public Sonuc Guncelle(PersonelResim resim)
        {
            string imgStr = "0x" + System.BitConverter.ToString(resim.resim).Replace("-", string.Empty);
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += ALANLAR[(int)ENUMAlanlar.RESIM] + "=" + imgStr + " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", resim.tcKimlikNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen kriter bilgilerine göre ilgili tablodan listeleme işlemini yapar.
        /// </summary>
        /// <param name="tcKimklikNo">İşlem yapılacak bilgileri içerir</param>
        /// <returns>IDataReader yapısı içinde, SQL çalışması sonucunda oluşan verileri döndürür.</returns>
        public IDataReader Getir(string tcKimklikNo)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.RESIM];
            sql += " FROM " + TABLOAD;
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", tcKimklikNo));
            sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// İlgili tablodan verilen bilgilere ait verileri silme işlemini yapar.
        /// </summary>
        /// <param name="resim">İşlem yapılacak bilgileri içerir</param>
        /// <returns>Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.</returns>
        public Sonuc Sil(PersonelResim resim)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", resim.tcKimlikNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen kriterlere ait bilgilerin ilgili tabloda kullanımını kontrol eder.
        /// </summary>
        /// <param name="resim">İşlem yapılacak bilgileri içerir</param>
        /// <returns>Verilen kriter bilgisine uygun kayıt bulunursa true, yoksa false döndürür</returns>
        public bool VarMi(PersonelResim resim)
        {
            string sql = "";

            sql = "SELECT COUNT(*)";
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", resim.tcKimlikNo));

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }
    }
}
