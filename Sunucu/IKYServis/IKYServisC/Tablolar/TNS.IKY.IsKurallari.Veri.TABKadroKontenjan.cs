using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABKadroKontenjan : TABOrtak
    {
        public TABKadroKontenjan(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "IKYKADROKONTENJAN";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            ORGANIZASYONKOD = 1,
            ISLEMNO = 2,
            ISLEMTUR = 3,
            ISLEMTARIH = 4,
            ACIKLAMA = 5,
            DAYANAKBELGETARIH = 6,
            DAYANAKBELGENO = 7,
            PERSONELTUR = 8
        }

        public static string[] ALANLAR = { "KOD", "ORGANIZASYONKOD", "ISLEMNO", "ISLEMTUR",
                                           "ISLEMTARIH", "ACIKLAMA", "DAYANAKBELGETARIH", "DAYANAKBELGENO","PERSONELTUR" };

        public Sonuc Kaydet(KadroKontenjan kk)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], kk.organizasyonKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMNO], kk.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTUR], (int)kk.islemTur) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], kk.islemTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], kk.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAYANAKBELGETARIH], kk.dayanakBelgeTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAYANAKBELGENO], kk.dayanakBelgeNo) + ",";
            if (kk.personelTur > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PERSONELTUR], (int)kk.personelTur);
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PERSONELTUR], (string)null);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kk.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(kk.kod) + ",";
                sql += VeriErisim.DegerYap(kk.organizasyonKod) + ",";
                sql += VeriErisim.DegerYap(kk.kod) + ",";
                sql += VeriErisim.DegerYap((int)kk.islemTur) + ",";
                sql += VeriErisim.DegerYap(kk.islemTarih) + ",";
                sql += VeriErisim.DegerYap(kk.aciklama) + ",";
                sql += VeriErisim.DegerYap(kk.dayanakBelgeTarih) + ",";
                sql += VeriErisim.DegerYap(kk.dayanakBelgeNo) + ",";
                if (kk.personelTur > 0)
                    sql += VeriErisim.DegerYap((int)kk.personelTur) + ")";
                else
                    sql += VeriErisim.DegerYap((string)null) + ")";
            }

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(KadroKontenjan kk)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", kk.kod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(KadroKontenjan kk)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;

            if (kk.kod > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", kk.kod);

            if (kk.organizasyonKod > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], "=", kk.organizasyonKod);

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
            
            if((int)kk.personelTur>0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PERSONELTUR], "=", (int)kk.personelTur));

            if (kk.detaylar != null && kk.detaylar.Length > 0)
            {
                string icSQL = "SELECT 1 FROM " + TABKadroKontenjanDetay.TABLOAD;
                icSQL += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " + TABKadroKontenjanDetay.TABLOAD + "." + TABKadroKontenjanDetay.ALANLAR[(int)TABKadroKontenjanDetay.ENUMAlanlar.KKOD];
                //icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + " = " + TABKadroKontenjanDetay.TABLOAD + "." + TABKadroKontenjanDetay.ALANLAR[(int)TABKadroKontenjanDetay.ENUMAlanlar.KURKOD];
                //icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + " = " + TABKadroKontenjanDetay.TABLOAD + "." + TABKadroKontenjanDetay.ALANLAR[(int)TABKadroKontenjanDetay.ENUMAlanlar.MUHASEBEKOD];
                //icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + " = " + TABKadroKontenjanDetay.TABLOAD + "." + TABKadroKontenjanDetay.ALANLAR[(int)TABKadroKontenjanDetay.ENUMAlanlar.BIRIMKOD];
                //icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO] + " = " + TABKadroKontenjanDetay.TABLOAD + "." + TABKadroKontenjanDetay.ALANLAR[(int)TABKadroKontenjanDetay.ENUMAlanlar.SIRANO];
                if (kk.detaylar[0].unvanKod > 0)
                    icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl(TABKadroKontenjanDetay.TABLOAD, TABKadroKontenjanDetay.ALANLAR[(int)TABKadroKontenjanDetay.ENUMAlanlar.UNVANKOD], "=", kk.detaylar[0].unvanKod));
                if (kk.detaylar[0].kadroDerece > 0)
                    icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl(TABKadroKontenjanDetay.TABLOAD, TABKadroKontenjanDetay.ALANLAR[(int)TABKadroKontenjanDetay.ENUMAlanlar.KADRODERECE], "=", kk.detaylar[0].kadroDerece));

                kosul = VeriErisim.KosulEkle(kosul, "AND", "EXISTS (" + icSQL + ")");
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;
            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        private int SiraNoAl(KadroKontenjan kk)
        {
            //string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KURKOD], ALANLAR[(int)ENUMAlanlar.BIRIMKOD] };
            //string[] strDeger = { kk.mb != null ? kk.mb.kurum.Replace(".", "") : string.Empty, kk.mb != null ? kk.mb.birimKod : string.Empty };
            //string[] intAlan = { ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] };
            //int[] intDeger = { kk.orgBirimKod, kk.mb != null ? kk.mb.muhasebe : 0 };
            //return TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMNO], strAlan, strDeger, intAlan, intDeger);
            return 1;
        }
    }
}