using System.Data;
using TNS.Veri;
using System;
using System.Text.RegularExpressions;

namespace TNS.TRM.IsKurallari.Veri
{
    public class TABHibeYatirimHakedis : TABOrtak
    {
        public TABHibeYatirimHakedis(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "TRMHIBEYATIRIMHAKEDIS";

        public enum ENUMAlanlar
        {
            HIBEKOD = 0,
            SOZLESMEKOD = 1,
            HAKEDISKOD = 2,
            FATURANO = 3,
            FATURATARIH = 4,
            FATURATUTAR = 5,
            HAKEDISDURUM = 6,
            ONAYTARIH = 7,
            REDACIKLAMA = 8,
            ODEMETARIH = 9,
            ODEMEGRUPNO = 10,
            ODEMEKOD = 11,
            KONTROL = 12,
            KONTROLTARIH = 13,
            ESASFATURATUTAR = 14,//A iş Planı için referans fiyat üstünde fatura gelmesi durumunda faturanın gerçek tutarını kayıt altına almak için
            SONODEME = 15,
        }
        public static string[] ALANLAR = { "HIBEKOD", "SOZLESMEKOD", "HAKEDISKOD", "FATURANO", "FATURATARIH",
                                           "FATURATUTAR", "HAKEDISDURUM", "ONAYTARIH", "REDACIKLAMA","ODEMETARIH","ODEMEGRUPNO","ODEMEKOD","KONTROL","KONTROLTARIH","ESASFATURATUTAR", "SONODEME" };

        public Sonuc Kaydet(HibeYatirimHakedis hakedis)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar(string.Empty, ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(hakedis.hibeKod) + ",";
            sql += VeriErisim.DegerYap(hakedis.sozlesmeKod) + ",";
            sql += VeriErisim.DegerYap(hakedis.hakedisKod) + ",";
            sql += VeriErisim.DegerYap(hakedis.faturaNo) + ",";
            sql += VeriErisim.DegerYap(hakedis.faturaTarih) + ",";
            sql += VeriErisim.DegerYap(hakedis.faturaTutar) + ",";
            sql += VeriErisim.DegerYap((int)hakedis.hakedisDurum) + ",";
            sql += VeriErisim.DegerYap(hakedis.onayTarih) + ",";
            sql += VeriErisim.DegerYap(hakedis.redAciklama) + ",";
            sql += VeriErisim.DegerYap(hakedis.odemeTarih) + ",";
            sql += VeriErisim.DegerYap(hakedis.odemeGrupNo) + ",";
            sql += VeriErisim.DegerYap(hakedis.odemeKod) + ",";
            sql += VeriErisim.DegerYap(0) + ",";
            sql += VeriErisim.DegerYap(new TNSDateTime()) + ",";
            sql += VeriErisim.DegerYap(hakedis.esasFaturaTutar) + ",";
            sql += VeriErisim.DegerYap(hakedis.sonOdeme) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(HibeYatirimHakedis hakedis)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hakedis.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", hakedis.sozlesmeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISKOD], "=", hakedis.hakedisKod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilHepsi(HibeYatirimHakedis hakedis)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hakedis.hibeKod));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", hakedis.sozlesmeKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc Guncelle(HibeYatirimHakedis hakedis)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.FATURANO], hakedis.faturaNo) + ",";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.FATURATARIH], hakedis.faturaTarih) + ",";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.FATURATUTAR], hakedis.faturaTutar) + ",";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SONODEME], hakedis.sonOdeme) + ",";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ESASFATURATUTAR], hakedis.esasFaturaTutar);
            //sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISDURUM], (int)hakedis.hakedisDurum) + ",";
            //sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ONAYTARIH], hakedis.onayTarih) + ",";
            //sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.REDACIKLAMA], hakedis.redAciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hakedis.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISKOD], "=", hakedis.hakedisKod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleDurum(HibeYatirimHakedis hakedis)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISDURUM], (int)hakedis.hakedisDurum);
            if (hakedis.hakedisDurum == ENUMHibeYatirimHakedisDurum.ONAYLANDI)
                sql += "," + VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ONAYTARIH], hakedis.onayTarih);
            else if (hakedis.hakedisDurum == ENUMHibeYatirimHakedisDurum.RED)
                sql += "," + VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.REDACIKLAMA], hakedis.redAciklama);
            if (hakedis.hakedisDurum == ENUMHibeYatirimHakedisDurum.ODENDI)
                sql += "," + VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ODEMETARIH], hakedis.odemeTarih);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hakedis.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISKOD], "=", hakedis.hakedisKod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleOdemeGrupNo(HibeYatirimHakedis hakedis)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ODEMEGRUPNO], hakedis.odemeGrupNo) + ",";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KONTROL], 0) + ",";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KONTROLTARIH], new TNSDateTime());
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hakedis.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISKOD], "=", hakedis.hakedisKod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleKontrol(HibeYatirimHakedis hakedis)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KONTROL], hakedis.kontrol) + ",";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KONTROLTARIH], hakedis.kontrolTarih);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hakedis.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISKOD], "=", hakedis.hakedisKod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(HibeYatirimHakedis hakedis)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar(string.Empty, ALANLAR) + " FROM " + TABLOAD;

            if (hakedis.hibeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hakedis.hibeKod));
            if (hakedis.sozlesmeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", hakedis.sozlesmeKod));
            if (hakedis.hakedisKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISKOD], "=", hakedis.hakedisKod));
            if ((int)hakedis.hakedisDurum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISDURUM], "=", (int)hakedis.hakedisDurum));
            if (!string.IsNullOrEmpty(hakedis.odemeGrupNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ODEMEGRUPNO], "=", hakedis.odemeGrupNo));
            if (hakedis.odemeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ODEMEKOD], "=", hakedis.odemeKod));

            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleUygulamaSozlesmesiVeHakedisler(int hibeKod)
        {
            string sql = "select " + VeriErisim.TumAlanlar("TRMHIBEYATIRIMUYGULAMASOZLESME", TABHibeYatirimUygulamaSozlesme.ALANLAR) + ", ";
            sql += VeriErisim.TumAlanlar("TRMHIBEYATIRIMHAKEDIS", ALANLAR) + " from TRMHIBEYATIRIMUYGULAMASOZLESME";
            sql += " left outer join TRMHIBEYATIRIMHAKEDIS";
            sql += " on TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD = TRMHIBEYATIRIMHAKEDIS.HIBEKOD and TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMEKOD = TRMHIBEYATIRIMHAKEDIS.SOZLESMEKOD";
            sql += " where TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD = '" + hibeKod + "' and ((TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM != '" + (int)ENUMHibeYatirimHakedisDurum.HATALI + "' and TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM != '" + (int)ENUMHibeYatirimHakedisDurum.RED + "') or TRMHIBEYATIRIMHAKEDIS.HAKEDISDURUM is null)";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleOdemeGonder(HibeYatirim bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("A", TABHibeYatirim.ALANLAR);
            sql += ", " + VeriErisim.TumAlanlar("B", TABHibeYatirimUygulamaSozlesme.ALANLAR);
            sql += ", " + VeriErisim.TumAlanlar("C", ALANLAR);
            sql += ", D.ADI, E.ADI, F.ADI, H. AD, I.AD, G.ADI";
            sql += $" FROM {TABHibeYatirim.TABLOAD} A ";
            sql += $" INNER JOIN {TABHibeYatirimUygulamaSozlesme.TABLOAD} B ON A.KOD = B.HIBEKOD";
            sql += $" INNER JOIN {TABLOAD} C ON B.HIBEKOD = C.HIBEKOD AND B.SOZLESMEKOD = C.SOZLESMEKOD";
            sql += $" INNER JOIN {TABYatirimNitelik.TABLOAD} D ON D.KOD = A.PROJENITELIK";
            sql += $" INNER JOIN {TABYatirimKonu.TABLOAD} E ON E.KOD = A.PROJEKONU";
            sql += $" INNER JOIN {TABYatirimKonu.TABLOAD} F ON F.KOD = A.PROJEALTKONU";
            sql += $" LEFT OUTER JOIN {TABKoy.TABLOAD} G ON G.KOD = A.KOYKOD";
            sql += $" LEFT OUTER JOIN ORGCOGRAFYA H ON H.KOD = A.ILKOD";
            sql += $" LEFT OUTER JOIN ORGCOGRAFYA I ON I.KOD = A.ILCEKOD";
            //sql += " AND B.SOZLESMEDURUM = " + (int)ENUMHibeYatirimUygulamaSozlesmeDurum.DEVAMEDIYOR;

            string kosul = string.Empty;
            if (bilgi.etapNo > 0)
                kosul = VeriErisim.KosulOl("A", TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.ETAPNO], "=", bilgi.etapNo);
            if (!string.IsNullOrEmpty(bilgi.ilKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.ILKOD], "=", bilgi.ilKod.Trim().Replace(".", "")));
            if (bilgi.projeNitelik > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.PROJENITELIK], "=", bilgi.projeNitelik));
            if (!string.IsNullOrEmpty(bilgi.projeAltKonu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.PROJEALTKONU], "LIKE", bilgi.projeAltKonu + "%"));
            if (!string.IsNullOrEmpty(bilgi.basvuruNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.BASVURUNO], "=", bilgi.basvuruNo));
            if (!string.IsNullOrEmpty(bilgi.kimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.KIMLIKNO], "=", bilgi.kimlikNo));
            if (!string.IsNullOrEmpty(bilgi.ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.AD] + " + ' ' + " + TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.SOYAD], "LIKE", "%" + bilgi.ad + "%"));
            if (bilgi.uygSozlesmeBilgileri != null && bilgi.uygSozlesmeBilgileri.Length > 0)
            {
                if (!string.IsNullOrEmpty(bilgi.uygSozlesmeBilgileri[0].kimlikNo))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("B", TABHibeYatirimUygulamaSozlesme.ALANLAR[(int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.KIMLIKNO], "=", bilgi.uygSozlesmeBilgileri[0].kimlikNo));
                if (!string.IsNullOrEmpty(bilgi.uygSozlesmeBilgileri[0].yukleniciAd))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("B", TABHibeYatirimUygulamaSozlesme.ALANLAR[(int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.YUKLENICIAD], "LIKE", "%" + bilgi.uygSozlesmeBilgileri[0].yukleniciAd + "%"));
                if ((int)bilgi.uygSozlesmeBilgileri[0].sozlesmeTur > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("B", TABHibeYatirimUygulamaSozlesme.ALANLAR[(int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.SOZLESMETUR], "=", (int)bilgi.uygSozlesmeBilgileri[0].sozlesmeTur));

                if (bilgi.uygSozlesmeBilgileri[0].hakedisBilgileri != null && bilgi.uygSozlesmeBilgileri[0].hakedisBilgileri.Length > 0 && (int)bilgi.uygSozlesmeBilgileri[0].hakedisBilgileri[0].hakedisDurum > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("C", ALANLAR[(int)ENUMAlanlar.HAKEDISDURUM], "=", (int)bilgi.uygSozlesmeBilgileri[0].hakedisBilgileri[0].hakedisDurum));

                if (bilgi.uygSozlesmeBilgileri[0].hakedisBilgileri != null && bilgi.uygSozlesmeBilgileri[0].hakedisBilgileri.Length > 0 && bilgi.uygSozlesmeBilgileri[0].hakedisBilgileri[0].odemeGrupNo != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("C", ALANLAR[(int)ENUMAlanlar.ODEMEGRUPNO], "=", bilgi.uygSozlesmeBilgileri[0].hakedisBilgileri[0].odemeGrupNo));

            }

            if (bilgi.yatirimOncelikTur == 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.PROJEKONU], "<", "51"));
            if (bilgi.yatirimOncelikTur == 1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.PROJEKONU], ">=", "51"));
            if (bilgi.altYapiGrup != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", TABHibeYatirim.ALANLAR[(int)TABHibeYatirim.ENUMAlanlar.ALTYAPIGRUP], "=", bilgi.altYapiGrup));


            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY C." + ALANLAR[(int)ENUMAlanlar.ONAYTARIH];

            return veriErisim.DataReaderDondur(sql);
        }

        public ENUMHibeYatirimHakedisDurum DurumGetir(HibeYatirimHakedis hakedis)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.HAKEDISDURUM] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hakedis.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISKOD], "=", hakedis.hakedisKod));

            return (ENUMHibeYatirimHakedisDurum)veriErisim.TekKolonSQLInt(sql);
        }

        public int SayacDegerAl(int hibeKod, int sozlesmeKod)
        {
            return Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.HAKEDISKOD], null, null, new string[] { ALANLAR[(int)ENUMAlanlar.HIBEKOD], ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD] }, new int[] { hibeKod, sozlesmeKod });
        }

        public decimal FaturaTutarToplamAl(int hibeKod, int sozlesmeKod, int hakedisKod, bool kendisi)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("SUM") + "(" + ALANLAR[(int)ENUMAlanlar.FATURATUTAR] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", sozlesmeKod));
            if (kendisi)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISKOD], "=", hakedisKod));
            else
            {
                if (hakedisKod > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISKOD], "<>", hakedisKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISDURUM], "<>", (int)ENUMHibeYatirimHakedisDurum.HATALI));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISDURUM], "<>", (int)ENUMHibeYatirimHakedisDurum.RED));
            }

            return veriErisim.TekKolonSQLDecimal(sql);
        }

        public decimal FaturaTutarToplamAlUygSozIptalOlanlar(int hibeKod, int sozlesmeKod, ENUMHibeYatirimUygulamaSozlesmeTur sozlesmeTur)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("SUM") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FATURATUTAR] + ")";
            sql += " FROM " + TABLOAD + ", " + TABHibeYatirimUygulamaSozlesme.TABLOAD;
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HIBEKOD] + " = " + TABHibeYatirimUygulamaSozlesme.TABLOAD + "." + TABHibeYatirimUygulamaSozlesme.ALANLAR[(int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.HIBEKOD];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD] + " = " + TABHibeYatirimUygulamaSozlesme.TABLOAD + "." + TABHibeYatirimUygulamaSozlesme.ALANLAR[(int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.SOZLESMEKOD];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "<>", sozlesmeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HAKEDISDURUM], "<>", (int)ENUMHibeYatirimHakedisDurum.HATALI));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HAKEDISDURUM], "<>", (int)ENUMHibeYatirimHakedisDurum.RED));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABHibeYatirimUygulamaSozlesme.TABLOAD, TABHibeYatirimUygulamaSozlesme.ALANLAR[(int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.SOZLESMETUR], "=", (int)sozlesmeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABHibeYatirimUygulamaSozlesme.TABLOAD, TABHibeYatirimUygulamaSozlesme.ALANLAR[(int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.SOZLESMEDURUM], "=", (int)ENUMHibeYatirimUygulamaSozlesmeDurum.IPTAL));

            return veriErisim.TekKolonSQLDecimal(sql);
        }

        public IDataReader ListeleOdeme(int etapNo, int ilTur, int durum, int odemeKod, bool kontrolEdilen, TNSDateTime baslamaTarih, TNSDateTime bitisTarihi)
        {
            string sql = "SELECT TRMHIBEYATIRIM.ETAPNO, HAK_A.ODEMEGRUPNO, TRMHIBEYATIRIM.KIMLIKNO, ORGCOGRAFYA.AD AS ILAD, ORGCOGRAFYA_1.AD AS ILCEAD, TRMHIBEYATIRIM.BASVURUNO, ";
            sql += " TRMHIBEYATIRIM.AD AS YATIRIMCIAD, TRMHIBEYATIRIMUYGULAMASOZLESME.YUKLENICIAD, TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMETUR, ";
            sql += " TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMETUTAR AS SOZTUTAR, SUM(HAK_A.FATURATUTAR) AS FATURATUTAR, TRMHIBEYATIRIM.ILKOD, HAK_A.HAKEDISDURUM, ";
            sql += " HAK_A.SOZLESMEKOD, HAK_A.HIBEKOD, ";

            sql += " (SELECT SUM(FATURATUTAR) AS ONCEDEN ";
            sql += " FROM TRMHIBEYATIRIMHAKEDIS AS HAK_B ";
            sql += " WHERE (HAKEDISDURUM = 20) AND (HAK_A.SOZLESMEKOD = SOZLESMEKOD) AND (HAK_A.HIBEKOD = HIBEKOD)) AS ONCEDENODENEN,TRMBANKASUBE.KOD, TRMHIBEYATIRIM.SOYAD, TRMHIBEYATIRIM.BABAAD, TRMHIBEYATIRIM.DOGUMTARIH, HAK_A.KONTROL,HAK_A.KONTROLTARIH,HAK_A.HAKEDISKOD,HAK_A.ODEMETARIH, TRMHIBEYATIRIM.TELEFON ";
            sql += " FROM TRMHIBEYATIRIMHAKEDIS AS HAK_A ";
            sql += " INNER JOIN TRMHIBEYATIRIM ON HAK_A.HIBEKOD = TRMHIBEYATIRIM.KOD ";
            sql += " INNER JOIN TRMHIBEYATIRIMUYGULAMASOZLESME ON HAK_A.HIBEKOD = TRMHIBEYATIRIMUYGULAMASOZLESME.HIBEKOD AND HAK_A.SOZLESMEKOD = TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMEKOD  ";
            sql += " INNER JOIN ORGCOGRAFYA ON TRMHIBEYATIRIM.ILKOD = ORGCOGRAFYA.KOD  ";
            sql += " INNER JOIN ORGCOGRAFYA AS ORGCOGRAFYA_1 ON TRMHIBEYATIRIM.ILCEKOD = ORGCOGRAFYA_1.KOD ";
            sql += " INNER JOIN TRMHIBEYATIRIMSOZLESME ON TRMHIBEYATIRIM.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD ";
            sql += " INNER JOIN TRMBANKASUBE ON TRMHIBEYATIRIMSOZLESME.BANKASUBEKOD = TRMBANKASUBE.KOD ";

            if (etapNo > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ETAPNO", "=", etapNo));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ETAPNO", ">", etapNo));

            if (odemeKod > -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HAK_A", "ODEMEKOD", "=", odemeKod));

            if (durum > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HAK_A", "HAKEDISDURUM", "=", durum));

            if (ilTur > -1)
            {
                //Ödeme türü kaldırıldı. melih 01.02.2023
                //if (ilTur == 1 || ilTur == 3)//GAP ve GAP Eksik Ödeme
                //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "=", "1"));
                //else if (ilTur == 10 || ilTur == 11)//KOP
                //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "=", "3"));
                //else if (ilTur == 20 || ilTur == 21)//DAP
                //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "=", "4"));
                //else if (ilTur == 30 || ilTur == 31)//DOKAP
                //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "=", "5"));
                //else if (ilTur == 2 || ilTur == 4)//Diğer İller ve diğer iller eksik ödeme
                //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "=", "0"));
            }

            if (kontrolEdilen)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HAK_A", "KONTROL", ">", "0"));

            if (!baslamaTarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HAK_A", "ODEMETARIH", ">=", baslamaTarih));

            if (!bitisTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HAK_A", "ODEMETARIH", "<=", bitisTarihi));

            sql += " GROUP BY TRMHIBEYATIRIM.ETAPNO, HAK_A.ODEMEGRUPNO, TRMHIBEYATIRIM.KIMLIKNO, ORGCOGRAFYA.AD, ORGCOGRAFYA_1.AD, TRMHIBEYATIRIM.BASVURUNO, TRMHIBEYATIRIM.AD,  ";
            sql += " TRMHIBEYATIRIMUYGULAMASOZLESME.YUKLENICIAD, TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMETUR, TRMHIBEYATIRIMUYGULAMASOZLESME.SOZLESMETUTAR,  ";
            sql += " TRMHIBEYATIRIM.ILKOD, HAK_A.HAKEDISDURUM, HAK_A.SOZLESMEKOD, HAK_A.HIBEKOD, TRMBANKASUBE.KOD, TRMHIBEYATIRIM.SOYAD, TRMHIBEYATIRIM.BABAAD, TRMHIBEYATIRIM.DOGUMTARIH, HAK_A.KONTROL, HAK_A.KONTROLTARIH,HAK_A.HAKEDISKOD,HAK_A.ODEMETARIH, TRMHIBEYATIRIM.TELEFON ";
            sql += " ORDER BY TRMHIBEYATIRIM.ETAPNO, HAK_A.ODEMEGRUPNO, TRMHIBEYATIRIM.KIMLIKNO";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc OdemeBaslatildiYap(int hibeKod, int sozlesmeKod, string odemeGrupNo, int odemeKod)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ODEMEKOD], odemeKod) + ",";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISDURUM], (int)ENUMHibeYatirimHakedisDurum.ODEMEBASLAT);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", sozlesmeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ODEMEGRUPNO], "=", odemeGrupNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISDURUM], "=", (int)ENUMHibeYatirimHakedisDurum.ODEMEDE));

            return KomutCalistir(sql, 0);
        }

        public Sonuc OdemeDurumDegistir(int odemeKod, int durum)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISDURUM], durum);

            if (durum == (int)ENUMHibeYatirimHakedisDurum.ODENDI)
                sql += "," + VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ODEMETARIH], new TNSDateTime(DateTime.Now.ToString()));

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ODEMEKOD], "=", odemeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISDURUM], "<", durum));

            return KomutCalistir(sql, 0);
        }

        public IDataReader OdemeKodunaGoreOdenenHakedisler(int odemeKod)
        {
            string sql = "";

            sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.HIBEKOD] + "," + ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD] + ", SUM(" + ALANLAR[(int)ENUMAlanlar.FATURATUTAR] + ") FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ODEMEKOD], "=", odemeKod));
            sql += " GROUP BY " + ALANLAR[(int)ENUMAlanlar.HIBEKOD] + "," + ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD];
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.HIBEKOD] + "," + ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader OdemeKodunaGoreOdenenHakedislerGroupByHibeKod(int odemeKod)
        {
            string sql = "";

            sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.HIBEKOD] + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ODEMEKOD], "=", odemeKod));
            sql += " GROUP BY " + ALANLAR[(int)ENUMAlanlar.HIBEKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader DigerOdenenHakedisler(HibeYatirimHakedis hakedis)
        {
            string sql = "";

            sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.HIBEKOD] + "," + ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD] + ", SUM(" + ALANLAR[(int)ENUMAlanlar.FATURATUTAR] + ") FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hakedis.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", hakedis.sozlesmeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISDURUM], "=", (int)hakedis.hakedisDurum));

            sql += " GROUP BY " + ALANLAR[(int)ENUMAlanlar.HIBEKOD] + "," + ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleHakedisOdenmeyen(int hibeKod)
        {
            string sql = "SELECT COUNT(" + ALANLAR[(int)TABHibeYatirimHakedis.ENUMAlanlar.HIBEKOD] + ") FROM " + TABHibeYatirimHakedis.TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)TABHibeYatirimHakedis.ENUMAlanlar.HIBEKOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)TABHibeYatirimHakedis.ENUMAlanlar.HAKEDISDURUM], "<", 20));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc YatirimciyiOdemedenCikart(HibeYatirimHakedis hakedis)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ODEMEGRUPNO], "") + ",";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ODEMEKOD], 0) + ",";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ODEMETARIH], new TNSDateTime()) + ",";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HAKEDISDURUM], (int)ENUMHibeYatirimHakedisDurum.YENI);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hakedis.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HAKEDISKOD], "=", hakedis.hakedisKod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc YatirimciyiOdemedenCikartOdemeSatiriRakamDuzelt(int kod, decimal tutar)
        {
            decimal komisyon = Math.Round(tutar * (decimal)0.002, 2);

            string sql = "UPDATE TRMHIBEMAKINAODEME SET ";
            sql += " MIKTAR=MIKTAR-" + tutar.ToString().Replace(",", ".");
            sql += ",YAKLASIKBEDEL=YAKLASIKBEDEL-" + tutar.ToString().Replace(",", ".");
            sql += ",ODENEK=ODENEK-" + tutar.ToString().Replace(",", ".");
            sql += ",HIBE=HIBE-" + (tutar - komisyon).ToString().Replace(",", "."); ;
            sql += ",KOMISYON=KOMISYON-" + komisyon.ToString().Replace(",", ".");
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "KOD", "=", kod));

            return KomutCalistir(sql, 1);
        }


    }
}