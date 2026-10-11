using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    public class TABTaahhutKartiSozlesme : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTaahhutKartiSozlesme(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHTAAHHUTKARTISOZLESME";

        public enum ENUMAlanlar
        {
            KOD = 0,
            TAAHHUTKARTIKOD = 1,
            TUR = 2,
            MADDENO = 3,
            AD = 4,
            ACIKLAMA = 5
        }
        static string[] ALANLAR = { "KOD", "TAAHHUTKARTIKOD", "TUR", "MADDENO", "AD", "ACIKLAMA" };

        public Sonuc Kaydet(TaahhutKartiSozlesme form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kod) + ", ";
            sql += VeriErisim.DegerYap(form.taahhutKartiKod) + ", ";
            sql += VeriErisim.DegerYap((int)form.tur) + ", ";
            sql += VeriErisim.DegerYap(form.maddeNo) + ", ";
            sql += VeriErisim.DegerYap(form.ad) + ", ";
            sql += VeriErisim.DegerYap(form.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(TaahhutKartiSozlesme form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            string kosul = "";

            if (form.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            if (form.taahhutKartiKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKARTIKOD], "=", form.taahhutKartiKod));

            if (kosul != "")
                sql += " WHERE " + kosul;
            else
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(TaahhutKartiSozlesme form)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (form.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            if (form.taahhutKartiKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKARTIKOD], "=", form.taahhutKartiKod));
            if (form.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", (int)form.tur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

    }
}
