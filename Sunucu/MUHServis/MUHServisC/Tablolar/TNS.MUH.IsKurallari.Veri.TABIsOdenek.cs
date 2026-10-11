using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHISODENEK veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIsOdenek : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIsOdenek(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHISODENEK";

        public enum ENUMAlanlar
        {
            ISKOD = 0,
            YIL = 1,
            KURKOD = 2,
            FONKOD = 3,
            FINKOD = 4,
            EKOKOD = 5,
            PEBKOD = 6,
            BLOKETUTAR = 7,
        }

        static string[] ALANLAR = { "ISKOD", "YIL", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "PEBKOD", "BLOKETUTAR" };

        public Sonuc Kaydet(IsOdenek form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(form.isKod) + ", ";
            sql = sql + VeriErisim.DegerYap(form.yil) + ", ";
            sql = sql + VeriErisim.DegerYap(form.kurKod) + ", ";
            sql = sql + VeriErisim.DegerYap(form.fonKod) + ", ";
            sql = sql + VeriErisim.DegerYap(form.finKod) + ", ";
            sql = sql + VeriErisim.DegerYap(form.ekoKod) + ", ";
            sql = sql + VeriErisim.DegerYap(form.pebKod) + ", ";
            sql = sql + VeriErisim.DegerYap(form.blokeTutar) + ") ";

            Sonuc sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sonuc = Guncelle(form);
            }

            return sonuc;
        }

        public Sonuc Guncelle(IsOdenek form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], form.kurKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], form.fonKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], form.finKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], form.ekoKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], form.pebKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BLOKETUTAR], form.blokeTutar);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISKOD], "=", form.isKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(IsOdenek form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISKOD], "=", form.isKod));
            if (form.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(IsOdenek form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISKOD], "=", form.isKod));
            if (form.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader DurumListele(int yil, int muhasebe, string birim, int isKod)
        {
            string sql = "SELECT KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, SUM(BLOKETUTAR)";
            sql += " FROM HRCBLOKEODENEK ";

            sql += " WHERE " + VeriErisim.KosulOl("", "YIL", "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", "MUHASEBEKOD", " = ", muhasebe);
            sql += " AND " + VeriErisim.KosulOl("", "BIRIMKOD", " = ", birim);
            sql += " AND " + VeriErisim.KosulOl("", "ISKOD", "=", isKod);

            sql += " GROUP BY KURKOD,FONKOD,FINKOD,EKOKOD,PEBKOD";

            return veriErisim.DataReaderDondur(sql);
        }

    }
}
