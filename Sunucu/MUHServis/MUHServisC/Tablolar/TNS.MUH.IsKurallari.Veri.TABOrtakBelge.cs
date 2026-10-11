using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHORTAKBELGE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABOrtakBelge : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABOrtakBelge(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHORTAKBELGE";

        public enum ENUMAlanlar { KOD = 0, ILGIKOD = 1, TIP = 2, BELGEADI = 3, BELGE = 4 }

        static string[] ALANLAR = { "KOD", "ILGIKOD", "TIP", "BELGEADI", "BELGE" };

        public IDataReader ListeleBelgeler(OrtakBelge form)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (!string.IsNullOrWhiteSpace(form.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            if (!string.IsNullOrWhiteSpace(form.ilgiKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILGIKOD], "=", form.ilgiKod));

            if (form.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIP], "=", (int)form.tip));

            if (!string.IsNullOrWhiteSpace(form.belgeAdi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGEADI], "=", form.belgeAdi));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.BELGEADI];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetBelgeler(OrtakBelge form)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kod) + ",";
            sql += VeriErisim.DegerYap(form.ilgiKod) + ",";
            sql += VeriErisim.DegerYap((int)form.tip) + ",";
            sql += VeriErisim.DegerYap(form.belgeAdi) + ",";
            string imgStr = "null";
            sql += imgStr + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilBelgeler(OrtakBelge form)
        {
            string sql = "", kosul = "";

            sql = "DELETE FROM " + TABLOAD;

            if (!string.IsNullOrWhiteSpace(form.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
            {
                if (!string.IsNullOrWhiteSpace(form.ilgiKod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILGIKOD], "=", form.ilgiKod));

                if (form.tip > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", (int)form.tip));
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            return KomutCalistir(sql, string.IsNullOrWhiteSpace(form.kod) ? 0 : 1);
        }
    }
}
