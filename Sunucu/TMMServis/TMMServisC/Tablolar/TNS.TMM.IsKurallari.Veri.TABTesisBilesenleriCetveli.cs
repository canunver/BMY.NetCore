using System.Data;
using TNS.Veri;
using System;

namespace TNS.TMM.IsKurallari.Veri
{
    public class TABTesisBilesenleriCetveli : TABOrtak
    {
        public TABTesisBilesenleriCetveli(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        static string TABLOAD = "TASTESISBILESENLERICETVELI";
        public enum ENUMAlanlar : int
        {
            REFID = 0,
            MUHASEBEKOD = 1,
            HARCAMABIRIMKOD = 2,
            AMBARKOD = 3,
            BELGENO = 4,
            BELGETARIHI = 5,
            DURUM = 6,
            SICILNO = 7,
            HESAPPLANKOD = 8,
            BULUNDUGUYER = 9
        }
        static string[] ALANLAR = { "REFID", "MUHASEBEKOD", "HARCAMABIRIMKOD", "AMBARKOD", "BELGENO", "BELGETARIHI", "DURUM", "SICILNO", "HESAPPLANKOD", "BULUNDUGUYER" };

        static string TABLOADDETAY = "TASTESISBILESENLERICETVELIDETAY";
        public enum ENUMAlanlarDetay : int
        {
            REFID = 0,
            REFIDANA = 1,
            SIRANO = 2,
            ADI = 3,
            MIKTAR = 4,
            OZELLIK = 5,
        }
        static string[] ALANLARDETAY = { "REFID", "REFIDANA", "SIRANO", "ADI", "MIKTAR", "OZELLIK" };

        public Sonuc Kaydet(TesisBilesenleri tk)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(tk.refId) + ",";
            sql += VeriErisim.DegerYap(tk.muhasebe.kod) + ",";
            sql += VeriErisim.DegerYap(tk.harcamaBirimi.kod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tk.ambar.kod) + ",";
            sql += VeriErisim.DegerYap(tk.belgeNo) + ",";
            sql += VeriErisim.DegerYap(tk.belgeTarihi) + ",";
            sql += VeriErisim.DegerYap(tk.durum) + ",";
            sql += VeriErisim.DegerYap(tk.sicilNo) + ",";
            sql += VeriErisim.DegerYap(tk.hesapPlanKod) + ",";
            sql += VeriErisim.DegerYap(tk.bulunduguYer) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetDetay(TesisBilesenleriDetay tk)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(tk.refID) + ",";
            sql += VeriErisim.DegerYap(tk.refIDAna) + ",";
            sql += VeriErisim.DegerYap(tk.siraNo) + ",";
            sql += VeriErisim.DegerYap(tk.adi) + ",";
            sql += VeriErisim.DegerYap(tk.miktar) + ",";
            sql += VeriErisim.DegerYap(tk.ozellik) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(TesisBilesenleri tk)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETARIHI], tk.belgeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SICILNO], tk.sicilNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD], tk.hesapPlanKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BULUNDUGUYER], tk.bulunduguYer);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.REFID], "=", tk.refId));

            return KomutCalistir(sql, 1);
        }

        public Sonuc DurumGuncelle(string refId, int durumKod)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], durumKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.REFID], "=", refId));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilDetay(string refIDAna)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.REFIDANA], "=", refIDAna));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(TesisBilesenleri tk)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", TASIL.KOD, TASIL.AD, TASILCE.KOD, TASILCE.AD";
            sql += " FROM TASTESISBILESENLERICETVELI LEFT OUTER JOIN ";
            sql += " TASMUHASEBE ON TASTESISBILESENLERICETVELI.MUHASEBEKOD = TASMUHASEBE.KOD LEFT OUTER JOIN ";
            sql += " TASILCE ON TASMUHASEBE.ILKOD = TASILCE.ILKOD AND TASMUHASEBE.ILCEKOD = TASILCE.KOD LEFT OUTER JOIN ";
            sql += " TASIL ON TASMUHASEBE.ILKOD = TASIL.KOD ";

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(tk.muhasebe.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tk.muhasebe.kod));

            if (!string.IsNullOrEmpty(tk.harcamaBirimi.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "LIKE", tk.harcamaBirimi.kod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(tk.ambar.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", tk.ambar.kod));

            if (tk.belgeNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], "=", tk.belgeNo));

            if (tk.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", tk.durum));

            if (tk.refId != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.REFID], "=", tk.refId));

            if (tk.sicilNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SICILNO], "=", tk.sicilNo));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY MUHASEBEKOD, HARCAMABIRIMKOD, BELGENO";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleDetay(TesisBilesenleriDetay tk)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY);
            sql += " FROM " + TABLOADDETAY;

            string kosul = string.Empty;

            if (tk.refID != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.REFID], "=", tk.refID));

            if (tk.refIDAna != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.REFIDANA], "=", tk.refIDAna));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY SIRANO";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}