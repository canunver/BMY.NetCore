using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABHibeYatirimOncelik : TABOrtak
    {
        public TABHibeYatirimOncelik(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "TRMHIBEYATIRIMONCELIK";
        public enum ENUMAlanlar : int
        {
            ETAPNO = 0,
            ILKOD = 1,
            SIRANO = 2,
            KONUKOD = 3,
            TUR = 4,
        }
        public static string[] ALANLAR = { "ETAPNO", "ILKOD", "SIRANO", "KONUKOD", "TUR" };

        public Sonuc Kaydet(HibeYatirimOncelik form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.etapNo) + ",";
            sql += VeriErisim.DegerYap(form.ilKod) + ",";
            sql += VeriErisim.DegerYap(form.siraNo) + ",";
            sql += VeriErisim.DegerYap(form.konu.kod) + ",";
            sql += VeriErisim.DegerYap(form.tur) + ")";
            return KomutCalistir(sql, 1);
        }
        public IDataReader Listele(HibeYatirimOncelik form)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",TRMYATIRIMKONU.KISAADI FROM " + TABLOAD;
            sql += " INNER JOIN TRMYATIRIMKONU ON TRMHIBEYATIRIMONCELIK.KONUKOD = TRMYATIRIMKONU.KOD";

            if (form.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", form.etapNo));
            if (form.ilKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILKOD], "=", form.ilKod));
            if (form.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));

            //0 normal //1 Kırsal eko.alt yapı
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", form.tur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc Sil(HibeYatirimOncelik form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", form.etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], "=", form.ilKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", form.tur));
            return KomutCalistir(sql, 0);
        }
    }
}