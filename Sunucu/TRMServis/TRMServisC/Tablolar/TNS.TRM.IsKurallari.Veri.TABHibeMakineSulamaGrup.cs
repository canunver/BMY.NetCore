using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{
    public class TABHibeMakineSulamaGrup : TABOrtak
    {
        public TABHibeMakineSulamaGrup(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "TRMHIBEMAKINAGRUP";
        public enum ENUMAlanlar
        {
            KOD = 0,
            ILKOD = 1,
            DURUM = 2,
            ODEMEKOD = 3,
            TIPI = 4,
            KONTROLTARIHI = 5,

        }
        public static string[] ALANLAR = { "KOD", "ILKOD", "DURUM", "ODEMEKOD", "TIPI", "KONTROLTARIHI" };

        public Sonuc Kaydet(HibeMakineSulamaGrup g)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(g.kod) + ",";
            sql += VeriErisim.DegerYap(g.ilKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(g.durum) + ",";
            sql += VeriErisim.DegerYap(g.odemeKod) + ",";
            sql += VeriErisim.DegerYap(g.tipi) + ",";
            sql += VeriErisim.DegerYap(g.kontrolTarih) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(HibeMakineSulamaGrup g)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], g.ilKod.Replace(".", "")) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], g.durum) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEKOD], g.odemeKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", g.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleKontrol(HibeMakineSulamaGrup g)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KONTROLTARIHI], g.kontrolTarih);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", g.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(HibeMakineSulamaGrup g)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", g.kod));

            if (g.durum > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", g.durum));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(HibeMakineSulamaGrup g, string[] yetkiliIller, params int[] ekDurumlar)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(g.kod))
                kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", g.kod);
            if (!string.IsNullOrEmpty(g.ilKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], "=", g.ilKod));
            if (g.durum > 0)
            {
                if (string.IsNullOrEmpty(kosul))
                    kosul = "(" + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", g.durum);
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", g.durum));
                if (ekDurumlar != null && ekDurumlar.Length > 0)
                {
                    foreach (int ekDurum in ekDurumlar)
                        kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", ekDurum));
                }
                kosul += ")";
            }
            if (g.odemeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ODEMEKOD], "=", g.odemeKod));
            if (g.tipi > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIPI], "=", g.tipi));

            if (yetkiliIller != null && yetkiliIller.Length > 0)
            {
                string ilKosul = string.Empty;
                foreach (string il in yetkiliIller)
                {
                    if (string.IsNullOrEmpty(il.Trim()))
                        continue;
                    if (!string.IsNullOrEmpty(ilKosul))
                        ilKosul += ", ";
                    ilKosul += VeriErisim.DegerYap(il.Trim().Replace(".", ""));
                }
                if (!string.IsNullOrEmpty(ilKosul))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", ALANLAR[(int)ENUMAlanlar.ILKOD] + " IN (" + ilKosul + ")");
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];
            return veriErisim.DataReaderDondur(sql);
        }
    }
}