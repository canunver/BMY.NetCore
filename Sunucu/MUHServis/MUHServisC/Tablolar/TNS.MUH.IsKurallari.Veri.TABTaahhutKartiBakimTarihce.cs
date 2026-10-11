using System;
using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    public class TABTaahhutKartiBakimTarihce : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTaahhutKartiBakimTarihce(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHTAAHHUTKARTIBAKIMTARIHCE";

        public enum ENUMAlanlar
        {
            KOD = 0,
            TAAHHUTKARTIKOD = 1,
            GIRISTARIH = 2,
            GIRISACIKLAMA = 3,
            CIKISTARIH = 4,
            CIKISACIKLAMA = 5
        }

        static string[] ALANLAR = { "KOD", "TAAHHUTKARTIKOD", "GIRISTARIH", "GIRISACIKLAMA", "CIKISTARIH", "CIKISACIKLAMA" };

        public Sonuc Kaydet(TaahhutKartiBakimTarihce form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kod) + ", ";
            sql += VeriErisim.DegerYap(form.taahhutKartiKod) + ", ";
            sql += VeriErisim.DegerYap(form.girisTarihi) + ", ";
            sql += VeriErisim.DegerYap(form.girisAciklama) + ", ";
            sql += VeriErisim.DegerYap(form.cikisTarihi) + ", ";
            sql += VeriErisim.DegerYap(form.cikisAciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(TaahhutKartiBakimTarihce form)
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

        public Sonuc Guncelle(TaahhutKartiBakimTarihce form)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GIRISTARIH], form.girisTarihi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GIRISACIKLAMA], form.girisAciklama) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CIKISTARIH], form.cikisTarihi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CIKISACIKLAMA], form.cikisAciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(TaahhutKartiBakimTarihce form)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (form.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            if (form.taahhutKartiKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKARTIKOD], "=", form.taahhutKartiKod));
            if (form.girisTarihi != null && !form.girisTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GIRISTARIH], "=", form.girisTarihi));
            if (form.cikisTarihi != null && !form.cikisTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.CIKISTARIH], "=", form.cikisTarihi));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

    }
}
