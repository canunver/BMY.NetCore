using System.Data;
using TNS.Veri;
using System;

namespace TNS.TMM.IsKurallari.Veri
{
    public class TABUretimFormu : TABOrtak
    {
        public TABUretimFormu(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        static string TABLOAD = "TASURETIMFORMU";
        public enum ENUMAlanlar : int
        {
            KOD = 0,
            FISNO = 1,
            FISTARIH = 2,
            ANAURUNHESAPPLANKOD = 3,
            MIKTAR = 4,
            GIRENMUHASEBEKOD = 5,
            GIRENHARCAMABIRIMKOD = 6,
            GIRENAMBARKOD = 7,
            CIKANMUHASEBEKOD = 8,
            CIKANHARCAMABIRIMKOD = 9,
            CIKANAMBARKOD = 10,
            DURUM = 11,
            ISLEMYAPAN = 12,
            GIRENTIFFISNO = 13,
            CIKANTIFFISNO = 14

        }
        static string[] ALANLAR = { "KOD", "FISNO", "FISTARIH", "ANAURUNHESAPPLANKOD", "MIKTAR", "GIRENMUHASEBEKOD", "GIRENHARCAMABIRIMKOD", "GIRENAMBARKOD", "CIKANMUHASEBEKOD", "CIKANHARCAMABIRIMKOD", "CIKANAMBARKOD", "DURUM", "ISLEMYAPAN", "GIRENTIFFISNO", "CIKANTIFFISNO" };

        public Sonuc Kaydet(UretimFormu tk)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(tk.kod) + ",";
            sql += VeriErisim.DegerYap(tk.fisNo) + ",";
            sql += VeriErisim.DegerYap(tk.fisTarihi) + ",";
            sql += VeriErisim.DegerYap(tk.anaUrun.hesapKod) + ",";
            sql += VeriErisim.DegerYap(tk.miktar) + ",";
            sql += VeriErisim.DegerYap(tk.girenMuhasebe.kod) + ",";
            sql += VeriErisim.DegerYap(tk.girenHarcamaBirimi.kod) + ",";
            sql += VeriErisim.DegerYap(tk.girenAmbar.kod) + ",";
            sql += VeriErisim.DegerYap(tk.cikanMuhasebe.kod) + ",";
            sql += VeriErisim.DegerYap(tk.cikanHarcamaBirimi.kod) + ",";
            sql += VeriErisim.DegerYap(tk.cikanAmbar.kod) + ",";
            sql += VeriErisim.DegerYap(tk.durum) + ",";
            sql += VeriErisim.DegerYap(tk.islemYapan) + ",";
            sql += VeriErisim.DegerYap(tk.girenFisNo) + ",";
            sql += VeriErisim.DegerYap(tk.cikanFisNo) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(UretimFormu tk)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FISTARIH], tk.fisTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANAURUNHESAPPLANKOD], tk.anaUrun.hesapKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], tk.islemYapan) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GIRENMUHASEBEKOD], tk.girenMuhasebe.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GIRENHARCAMABIRIMKOD], tk.girenHarcamaBirimi.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GIRENAMBARKOD], tk.girenAmbar.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CIKANMUHASEBEKOD], tk.cikanMuhasebe.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CIKANHARCAMABIRIMKOD], tk.cikanHarcamaBirimi.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CIKANAMBARKOD], tk.cikanAmbar.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GIRENTIFFISNO], tk.girenFisNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CIKANTIFFISNO], tk.cikanFisNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MIKTAR], tk.miktar);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", tk.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc DurumGuncelle(string kod, int durumKod)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], durumKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(UretimFormu tk)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", TASMUHASEBE.AD, TASMUHASEBE_1.AD, TASHARCAMABIRIM.AD, TASHARCAMABIRIM_1.AD, TASAMBAR.AD, TASAMBAR_1.AD, TASHESAPPLAN.ACIKLAMA";
            sql += " FROM TASURETIMFORMU ";
            sql += " INNER JOIN TASMUHASEBE ON TASURETIMFORMU.GIRENMUHASEBEKOD = TASMUHASEBE.KOD";
            sql += " INNER JOIN TASMUHASEBE AS TASMUHASEBE_1 ON TASURETIMFORMU.CIKANMUHASEBEKOD = TASMUHASEBE_1.KOD";
            sql += " INNER JOIN TASHARCAMABIRIM AS TASHARCAMABIRIM ON TASURETIMFORMU.GIRENMUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD AND TASURETIMFORMU.GIRENHARCAMABIRIMKOD = TASHARCAMABIRIM.KOD";
            sql += " INNER JOIN TASHARCAMABIRIM AS TASHARCAMABIRIM_1 ON TASURETIMFORMU.CIKANMUHASEBEKOD = TASHARCAMABIRIM_1.MUHASEBEKOD AND TASURETIMFORMU.CIKANHARCAMABIRIMKOD = TASHARCAMABIRIM_1.KOD";
            sql += " INNER JOIN TASAMBAR ON TASURETIMFORMU.GIRENMUHASEBEKOD = TASAMBAR.MUHASEBEKOD AND TASURETIMFORMU.GIRENHARCAMABIRIMKOD = TASAMBAR.HARCAMABIRIMKOD AND TASURETIMFORMU.GIRENAMBARKOD = TASAMBAR.KOD";
            sql += " INNER JOIN TASAMBAR AS TASAMBAR_1 ON TASURETIMFORMU.CIKANMUHASEBEKOD = TASAMBAR_1.MUHASEBEKOD AND TASURETIMFORMU.CIKANHARCAMABIRIMKOD = TASAMBAR_1.HARCAMABIRIMKOD AND TASURETIMFORMU.CIKANAMBARKOD = TASAMBAR_1.KOD";
            sql += " INNER JOIN TASHESAPPLAN ON TASURETIMFORMU.ANAURUNHESAPPLANKOD = TASHESAPPLAN.KOD";

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(tk.cikanMuhasebe.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.CIKANMUHASEBEKOD], "=", tk.cikanMuhasebe.kod));

            if (!string.IsNullOrEmpty(tk.cikanHarcamaBirimi.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.CIKANHARCAMABIRIMKOD], "LIKE", tk.cikanHarcamaBirimi.kod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(tk.cikanAmbar.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.CIKANAMBARKOD], "=", tk.cikanAmbar.kod));

            if (!string.IsNullOrEmpty(tk.girenMuhasebe.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GIRENMUHASEBEKOD], "=", tk.girenMuhasebe.kod));

            if (!string.IsNullOrEmpty(tk.girenHarcamaBirimi.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GIRENHARCAMABIRIMKOD], "LIKE", tk.girenHarcamaBirimi.kod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(tk.girenAmbar.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GIRENAMBARKOD], "=", tk.girenAmbar.kod));

            if (!string.IsNullOrEmpty(tk.girenFisNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GIRENTIFFISNO], "=", tk.girenFisNo));

            if (tk.sorguYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "YEAR(FISTARIH)", "=", tk.sorguYil));

            if (tk.fisNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", tk.fisNo));

            if (tk.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", tk.durum));

            if (tk.kod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", tk.kod));

            if (tk.anaUrun.hesapKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANAURUNHESAPPLANKOD], "=", tk.anaUrun.hesapKod));

            if (!tk.sorguBelgeTarihBasla.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISTARIH], ">=", tk.sorguBelgeTarihBasla));
            if (!tk.sorguBelgeTarihBitis.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISTARIH], "<=", tk.sorguBelgeTarihBitis));
            if (tk.sorguBelgeNoBasla > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], ">=", tk.sorguBelgeNoBasla));
            if (tk.sorguBelgeNoBitis > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "<=", tk.sorguBelgeNoBitis));
            if (tk.islemYapan != string.Empty)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "LIKE", "%" + tk.islemYapan + "%"));


            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY CIKANMUHASEBEKOD, CIKANHARCAMABIRIMKOD, CIKANAMBARKOD, FISNO";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}