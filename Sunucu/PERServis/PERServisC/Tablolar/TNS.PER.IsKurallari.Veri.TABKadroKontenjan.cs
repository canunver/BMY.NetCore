using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PER.IsKurallari.Veri
{
    public class TABKadroKontenjan : TABOrtak
    {
        public TABKadroKontenjan(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "PERKADROKONTENJAN";

        public enum ENUMAlanlar : int
        {
            ORGBIRIMKOD = 0,
            KURKOD = 1,
            MUHASEBEKOD = 2,
            BIRIMKOD = 3,
            SIRANO = 4,
            ISLEMTUR = 5,
            ISLEMTARIH = 6,
            ACIKLAMA = 7,
            DAYANAKBELGETARIH = 8,
            DAYANAKBELGENO = 9
        }

        public static string[] ALANLAR = { "ORGBIRIMKOD", "KURKOD", "MUHASEBEKOD", "BIRIMKOD", "SIRANO", "ISLEMTUR",
                                           "ISLEMTARIH", "ACIKLAMA", "DAYANAKBELGETARIH", "DAYANAKBELGENO" };

        public Sonuc Kaydet(KadroKontenjan kk)
        {
            if (kk.siraNo <= 0)
                kk.siraNo = SiraNoAl(kk);

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(kk.orgBirimKod) + ",";
            if (kk.mb != null)
            {
                sql += VeriErisim.DegerYap(kk.mb.kurum.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(kk.mb.muhasebe) + ",";
                sql += VeriErisim.DegerYap(kk.mb.birimKod) + ",";
            }
            else
            {
                sql += VeriErisim.DegerYap(string.Empty) + ",";
                sql += VeriErisim.DegerYap(0) + ",";
                sql += VeriErisim.DegerYap(string.Empty) + ",";
            }
            sql += VeriErisim.DegerYap(kk.siraNo) + ",";
            sql += VeriErisim.DegerYap((int)kk.islemTur) + ",";
            sql += VeriErisim.DegerYap(kk.islemTarih) + ",";
            sql += VeriErisim.DegerYap(kk.aciklama) + ",";
            sql += VeriErisim.DegerYap(kk.dayanakBelgeTarih) + ",";
            sql += VeriErisim.DegerYap(kk.dayanakBelgeNo) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(KadroKontenjan kk)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", kk.orgBirimKod));
            if (kk.mb != null)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kk.mb.kurum.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kk.mb.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", kk.mb.birimKod));
            }
            else
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", string.Empty));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", 0));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", string.Empty));
            }
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", kk.siraNo));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(KadroKontenjan kk)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (kk.orgBirimKod > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", kk.orgBirimKod);
            if (kk.mb != null)
            {
                if (!string.IsNullOrEmpty(kk.mb.kurum))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kk.mb.kurum.Replace(".", "")));
                if (kk.mb.muhasebe > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kk.mb.muhasebe));
                if (!string.IsNullOrEmpty(kk.mb.birimKod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", kk.mb.birimKod));
            }
            if (kk.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", kk.siraNo));
            if ((int)kk.islemTur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTUR], "=", (int)kk.islemTur));
            if (kk.islemTarih != null)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], "=", kk.islemTarih));
            if (!string.IsNullOrEmpty(kk.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + kk.aciklama + "%"));
            if (kk.dayanakBelgeTarih != null)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DAYANAKBELGETARIH], "=", kk.dayanakBelgeTarih));
            if (!string.IsNullOrEmpty(kk.dayanakBelgeNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DAYANAKBELGENO], "LIKE", kk.dayanakBelgeNo + "%"));

            if (kk.detaylar != null && kk.detaylar.Length > 0)
            {
                string icSQL = "SELECT 1 FROM " + TABKadroKontenjanDetay.TABLOAD;
                icSQL += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD] + " = " + TABKadroKontenjanDetay.TABLOAD + "." + TABKadroKontenjanDetay.ALANLAR[(int)TABKadroKontenjanDetay.ENUMAlanlar.ORGBIRIMKOD];
                icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + " = " + TABKadroKontenjanDetay.TABLOAD + "." + TABKadroKontenjanDetay.ALANLAR[(int)TABKadroKontenjanDetay.ENUMAlanlar.KURKOD];
                icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + " = " + TABKadroKontenjanDetay.TABLOAD + "." + TABKadroKontenjanDetay.ALANLAR[(int)TABKadroKontenjanDetay.ENUMAlanlar.MUHASEBEKOD];
                icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + " = " + TABKadroKontenjanDetay.TABLOAD + "." + TABKadroKontenjanDetay.ALANLAR[(int)TABKadroKontenjanDetay.ENUMAlanlar.BIRIMKOD];
                icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO] + " = " + TABKadroKontenjanDetay.TABLOAD + "." + TABKadroKontenjanDetay.ALANLAR[(int)TABKadroKontenjanDetay.ENUMAlanlar.SIRANO];
                if (kk.detaylar[0].unvanKod > 0)
                    icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl(TABKadroKontenjanDetay.TABLOAD, TABKadroKontenjanDetay.ALANLAR[(int)TABKadroKontenjanDetay.ENUMAlanlar.UNVANKOD], "=", kk.detaylar[0].unvanKod));
                if (kk.detaylar[0].kadroDerece > 0)
                    icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl(TABKadroKontenjanDetay.TABLOAD, TABKadroKontenjanDetay.ALANLAR[(int)TABKadroKontenjanDetay.ENUMAlanlar.KADRODERECE], "=", kk.detaylar[0].kadroDerece));

                kosul = VeriErisim.KosulEkle(kosul, "AND", "EXISTS (" + icSQL + ")");
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        private int SiraNoAl(KadroKontenjan kk)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KURKOD], ALANLAR[(int)ENUMAlanlar.BIRIMKOD] };
            string[] strDeger = { kk.mb != null ? kk.mb.kurum.Replace(".","") : string.Empty, kk.mb != null ? kk.mb.birimKod : string.Empty };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] };
            int[] intDeger = { kk.orgBirimKod, kk.mb != null ? kk.mb.muhasebe : 0 };
            return TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], strAlan, strDeger, intAlan, intDeger);
        }
    }
}