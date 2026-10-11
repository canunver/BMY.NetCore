using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABOnayFormTertip : TABOrtak
    {
        public TABOnayFormTertip(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADTERTIP = "GELIHALEONAYTERTIP";
        public enum ENUMAlanlarTertip
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 1,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,
            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 3,
            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 4,
            /// <summary>
            /// PEBKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 5,
            /// <summary>
            /// ODENEKTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENEKTUTAR = 6,
            /// <summary>
            /// BLOKETUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BLOKETUTAR = 7,
            /// <summary>
            /// KULLANILACAKTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANILACAKTUTAR = 8,
            /// <summary>
            /// KDVTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KDVTUTAR = 9,
        }

        public static string[] ALANLARTERTIP = { "YIL", "SURECNO", "SIRANO", "KURKOD", "EKOKOD", "PEBKOD", "ODENEKTUTAR", "BLOKETUTAR", "KULLANILACAKTUTAR", "KDVTUTAR" };

        public IDataReader TertipListele(OnayFormTertip bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTERTIP, ALANLARTERTIP) + "," +
                TABSurec.TABLOADSUREC + "." + TABSurec.ALANLARSUREC[(int)TABSurec.ENUMAlanlarSurec.MUHASEBEKOD] + "," +
                TABSurec.TABLOADSUREC + "." + TABSurec.ALANLARSUREC[(int)TABSurec.ENUMAlanlarSurec.BIRIMKOD];
            sql += " FROM " + TABLOADTERTIP;
            sql += " INNER JOIN " + TABSurec.TABLOADSUREC + " ON ";
            sql += TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL] + "=" + TABSurec.TABLOADSUREC + "." + TABSurec.ALANLARSUREC[(int)TABSurec.ENUMAlanlarSurec.YIL] + " AND " +
                   TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO] + "=" + TABSurec.TABLOADSUREC + "." + TABSurec.ALANLARSUREC[(int)TABSurec.ENUMAlanlarSurec.SURECNO];

            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.BLOKETUTAR], ">", 0));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOADSUREC, TABSurec.ALANLARSUREC[(int)TABSurec.ENUMAlanlarSurec.SURECDURUM], "<>", (int)TNS.HRC.ENUMSurecDurum.IPTAL));
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL], "=", bilgi.yil));
            if (!string.IsNullOrEmpty(bilgi.surecNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO], "=", bilgi.surecNo));
            //if (bilgi.tertipYil > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.TERTIPYIL], "=", bilgi.tertipYil));
            if (bilgi.kurKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.KURKOD], "=", bilgi.kurKod));
            if (bilgi.ekoKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.EKOKOD], "=", bilgi.ekoKod));
            if (bilgi.pebKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.PEBKOD], "=", bilgi.pebKod));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOADSUREC, TABSurec.ALANLARSUREC[(int)TABSurec.ENUMAlanlarSurec.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSurec.TABLOADSUREC, TABSurec.ALANLARSUREC[(int)TABSurec.ENUMAlanlarSurec.BIRIMKOD], "=", bilgi.birim));
            if (kosul != "")
                sql += " WHERE " + kosul;
            sql += " ORDER BY " + TABSurec.TABLOADSUREC + "." + TABSurec.ALANLARSUREC[(int)TABSurec.ENUMAlanlarSurec.MUHASEBEKOD] + "," +
                                  TABSurec.TABLOADSUREC + "." + TABSurec.ALANLARSUREC[(int)TABSurec.ENUMAlanlarSurec.BIRIMKOD] + "," +
                                  TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO] + " DESC";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc TertipKaydet(OnayFormTertip bilgi)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";
            sql = "INSERT INTO " + TABLOADTERTIP + " (" + VeriErisim.TumAlanlar("", ALANLARTERTIP) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.kurKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ekoKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.pebKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.odenekTutari) + ",";
            sql += VeriErisim.DegerYap(bilgi.blokeTutari) + ",";
            sql += VeriErisim.DegerYap(bilgi.kullanilacakTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.kdvTutar) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }

        public Sonuc TertipSil(OnayFormTertip bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOADTERTIP;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO], "=", bilgi.surecNo));
            if (bilgi.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.SIRANO], "=", bilgi.siraNo));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

    }
}
