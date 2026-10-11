using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari.Veri
{
    public class TABMPGREVIZETALEP : TABOrtak
    {
        public TABMPGREVIZETALEP(VeriErisim veriErisim) : base(veriErisim) { }

        /*
         CREATE TABLE MPGREVIZETALEPTERTIP(
            YIL INT NOT NULL,
            KYKOD VARCHAR(20) NOT NULL,
            REVIZETALEPNO INT NOT NULL,
            KURKOD VARCHAR(20) NOT NULL,
            FONKOD VARCHAR(20) NOT NULL,
            FINKOD VARCHAR(20) NOT NULL,
            EKOKOD VARCHAR(20) NOT NULL,
            PEBKOD VARCHAR(20) NOT NULL,
            AY INT NOT NULL,
            TALEPMI INT NOT NULL,
            ODENEK MONEY NOT NULL,
            DURUM INT NOT NULL,
            PRIMARY KEY(
            YIL,
            KYKOD,
            REVIZETALEPNO,
            KURKOD,
            FONKOD,
            FINKOD,
            EKOKOD,
            PEBKOD,
            AY,
            TALEPMI
            )
         )
        */
        public static string TABLOADREVIZETALEPTERTIP = "MPGREVIZETALEPTERTIP";
        public enum ENUMAlanlarREVIZETALEPTERTIP
        {
            YIL = 0,
            KYKOD = 1,
            REVIZETALEPNO = 2,
            KURKOD = 3,
            FONKOD = 4,
            FINKOD = 5,
            EKOKOD = 6,
            PEBKOD = 7,
            AY = 8,
            TALEPMI = 9,
            ODENEK = 10,
            DURUM = 11,
        }

        public static string[] ALANLARREVIZETALEPTERTIP = 
        { 
            "YIL",
            "KYKOD",
            "REVIZETALEPNO",
            "KURKOD",
            "FONKOD",
            "FINKOD",
            "EKOKOD",
            "PEBKOD",
            "AY",
            "TALEPMI",
            "ODENEK",
            "DURUM"
        };

        /*
         CREATE TABLE MPGREVIZETALEP
         (
            YIL INT NOT NULL,
            KYKOD VARCHAR(20) NOT NULL,
            REVIZETALEPNO INT NOT NULL,
            EVRAKTARIH DATETIME NOT NULL,
            EVRAKNO VARCHAR(50) NOT NULL,
            EVRAKILGI VARCHAR(50) NULL,
            TALEPACIKLAMA VARCHAR(200) NOT NULL,
            CEVAPACIKLAMA VARCHAR(200) NULL,
            FORMDURUM INT NOT NULL,
            KAYDEDEN VARCHAR(50) NOT NULL,
            KAYITTARIH DATETIME NOT NULL,
            ONAYLAYAN VARCHAR(50) NULL,
            ONAYTARIH DATETIME NULL,
            PRIMARY KEY(
            YIL,
            KYKOD,
            REVIZETALEPNO
            )
         )
         * 
         * 
        */
        public static string TABLOADREVIZETALEP = "MPGREVIZETALEP";
        public enum ENUMAlanlarREVIZETALEP
        {
            YIL = 0,
            KYKOD = 1,
            REVIZETALEPNO = 2,
            EVRAKTARIH = 3,
            EVRAKNO = 4,
            EVRAKILGI = 5,
            TALEPACIKLAMA = 6,
            CEVAPACIKLAMA = 7,
            FORMDURUM = 8,
            KAYDEDEN = 9,
            KAYITTARIH = 10,
            ONAYLAYAN = 11,
            ONAYTARIH = 12,
        }

        public static string[] ALANLARREVIZETALEP = 
        { 
            "YIL",
            "KYKOD",
            "REVIZETALEPNO",
            "EVRAKTARIH",
            "EVRAKNO",
            "EVRAKILGI",
            "TALEPACIKLAMA",
            "CEVAPACIKLAMA",
            "FORMDURUM",
            "KAYDEDEN",
            "KAYITTARIH",
            "ONAYLAYAN",
            "ONAYTARIH"
        };

        public Sonuc RevizeTalepDetaySil(int yil, string kyKod, int revizeTalepNo)
        {
            string sql = "DELETE FROM " + TABLOADREVIZETALEPTERTIP;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARREVIZETALEPTERTIP[(int)ENUMAlanlarREVIZETALEPTERTIP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARREVIZETALEPTERTIP[(int)ENUMAlanlarREVIZETALEPTERTIP.KYKOD], "=", kyKod.Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl("", ALANLARREVIZETALEPTERTIP[(int)ENUMAlanlarREVIZETALEPTERTIP.REVIZETALEPNO], "=", revizeTalepNo);

            return KomutCalistir(sql, 0);
        }

        public Sonuc RevizeTalepKarsilananSil(int yil, string kyKod, int revizeTalepNo)
        {
            string sql = "DELETE FROM " + TABLOADREVIZETALEPTERTIP;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARREVIZETALEPTERTIP[(int)ENUMAlanlarREVIZETALEPTERTIP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARREVIZETALEPTERTIP[(int)ENUMAlanlarREVIZETALEPTERTIP.KYKOD], "=", kyKod.Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl("", ALANLARREVIZETALEPTERTIP[(int)ENUMAlanlarREVIZETALEPTERTIP.REVIZETALEPNO], "=", revizeTalepNo);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARREVIZETALEPTERTIP[(int)ENUMAlanlarREVIZETALEPTERTIP.TALEPMI], "=", 0);

            return KomutCalistir(sql, 0);
        }

        public Sonuc RevizeTalepSil(int yil, string kyKod, int revizeTalepNo)
        {
            string sql = "DELETE FROM " + TABLOADREVIZETALEP;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.KYKOD], "=", kyKod.Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl("", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.REVIZETALEPNO], "=", revizeTalepNo);

            return KomutCalistir(sql, 0);
        }

        public int RevizeTalepSiraNoBul(int yil, string kyKod)
        {
            string sql = "SELECT MAX(REVIZETALEPNO) FROM " + TABLOADREVIZETALEP;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.KYKOD], "=", kyKod.Replace(".", ""));

            int donusDeger = veriErisim.TekKolonSQLInt(sql);
            if (veriErisim.hata != "")
                return 0;
            return donusDeger + 1;
        }

        public Sonuc RevizeTalepEkle(MPRevizeTalep form, string kullaniciKodu)
        {
            string sql = "INSERT INTO " + TABLOADREVIZETALEP + "(" + VeriErisim.TumAlanlar("", ALANLARREVIZETALEP) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.kurumsalYapi.kod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.revizeTalepNo);

            sql += "," + VeriErisim.DegerYap(form.evrakTarih);
            sql += "," + VeriErisim.DegerYap(form.evrakNo);
            sql += "," + VeriErisim.DegerYap(form.evrakIlgi);
            sql += "," + VeriErisim.DegerYap(form.talepAciklama);
            sql += "," + VeriErisim.DegerYap((string)null);
            sql += "," + VeriErisim.DegerYap((int)form.formDurum);
            sql += "," + VeriErisim.DegerYap(kullaniciKodu);
            sql += "," + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql += "," + VeriErisim.DegerYap((string)null);
            sql += "," + VeriErisim.DegerYap((string)null);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc RevizeTalepDetayEkle(MPRevizeTalep form, MPRevizeSatir satir, int ay, bool talepMi)
        {
            double odenek = 0;
            if (talepMi) odenek = satir.odenek[ay];
            else odenek = satir.karsilananOdenek[ay];
            if (odenek == 0) return new Sonuc(true, "", "");
            string sql = "INSERT INTO " + TABLOADREVIZETALEPTERTIP + "(" + VeriErisim.TumAlanlar("", ALANLARREVIZETALEPTERTIP) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.kurumsalYapi.kod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.revizeTalepNo);
            sql += "," + VeriErisim.DegerYap(satir.tertip.kodlar[0].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(satir.tertip.kodlar[1].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(satir.tertip.kodlar[2].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(satir.tertip.kodlar[3].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(satir.tertip.kodlar[4].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(ay + 1);
            sql += "," + VeriErisim.DegerYap(talepMi ? 1 : 0);
            sql += "," + VeriErisim.DegerYap(odenek);
            sql += "," + VeriErisim.DegerYap((int)0);
            //sql += "," + VeriErisim.DegerYap((int)form.formDurum);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public IDataReader RevizeTalepListele(MPRevizeTalep kistasForm)
        {
            string tumAlanlar = VeriErisim.TumAlanlar("A", ALANLARREVIZETALEP);
            string sql = "SELECT " + tumAlanlar + ", SUM(B.ODENEK), SUM(C.ODENEK) FROM ";
            sql += "MPGREVIZETALEP A LEFT OUTER JOIN MPGREVIZETALEPTERTIP AS B ON";
            sql += " A.YIL = B.YIL";
            sql += " AND A.KYKOD = B.KYKOD";
            sql += " AND A.REVIZETALEPNO = B.REVIZETALEPNO";
            sql += " AND 1 = B.TALEPMI";
            sql += " LEFT OUTER JOIN MPGREVIZETALEPTERTIP AS C ON ";
            sql += " A.YIL = C.YIL";
            sql += " AND A.KYKOD = C.KYKOD";
            sql += " AND A.REVIZETALEPNO = C.REVIZETALEPNO";
            sql += " AND 0 = C.TALEPMI";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("A", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.YIL], "=", kistasForm.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.KYKOD], "LIKE", kistasForm.kurumsalYapi.kod.Replace(".", "") + "%"));
            if (kistasForm.revizeTalepNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.REVIZETALEPNO], "=", kistasForm.revizeTalepNo));
            else
            {
                if (kistasForm.satirlar != null && kistasForm.satirlar.Length == 1)
                {
                    Tertip kisTer = kistasForm.satirlar[0].tertip;
                    if (!string.IsNullOrEmpty(kisTer.TertipStrOl()))
                    {
                        string icSql = "SELECT DISTINCT 1 FROM " + TABLOADREVIZETALEPTERTIP + " IC";
                        icSql += " WHERE IC.YIL = A.YIL";
                        icSql += " AND IC.KYKOD = A.KYKOD";
                        icSql += " AND IC.REVIZETALEPNO = A.REVIZETALEPNO";
                        icSql += TABBHMGENEL.AlanKosulOl("IC", ALANLARREVIZETALEPTERTIP[(int)ENUMAlanlarREVIZETALEPTERTIP.KURKOD], kisTer, 0);
                        icSql += TABBHMGENEL.AlanKosulOl("IC", ALANLARREVIZETALEPTERTIP[(int)ENUMAlanlarREVIZETALEPTERTIP.FONKOD], kisTer, 1);
                        icSql += TABBHMGENEL.AlanKosulOl("IC", ALANLARREVIZETALEPTERTIP[(int)ENUMAlanlarREVIZETALEPTERTIP.FINKOD], kisTer, 2);
                        icSql += TABBHMGENEL.AlanKosulOl("IC", ALANLARREVIZETALEPTERTIP[(int)ENUMAlanlarREVIZETALEPTERTIP.EKOKOD], kisTer, 3);
                        icSql += TABBHMGENEL.AlanKosulOl("IC", ALANLARREVIZETALEPTERTIP[(int)ENUMAlanlarREVIZETALEPTERTIP.PEBKOD], kisTer, 4);
                        sql = VeriErisim.KosulEkle(sql, "AND", "( 1 in (" + icSql + "))");
                    }
                }

                if (kistasForm.formDurum == ENUMRevizeFormDurum.IptalOlmayan)
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.FORMDURUM], "<=", (int)ENUMRevizeFormDurum.Musvette));
                }
                else if (kistasForm.formDurum != ENUMRevizeFormDurum.TANIMSIZ)
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.FORMDURUM], "=", (int)kistasForm.formDurum));
                }

                if (!string.IsNullOrWhiteSpace(kistasForm.evrakNo))
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.EVRAKNO], "=", kistasForm.evrakNo));
                }
            }
            sql += " GROUP BY " + tumAlanlar;
            sql += " ORDER BY " + ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.EVRAKTARIH] + " DESC";
            return this.veriErisim.DataReaderDondur(sql);
        }

        public IDataReader TalepDetayOku(int yil, string kyKod, int revizeTalepNo, bool sadeceTalepler)
        {
            string tumAlanlar = VeriErisim.TumAlanlar("", ALANLARREVIZETALEPTERTIP);
            string sql = "SELECT " + tumAlanlar + " FROM " + TABLOADREVIZETALEPTERTIP;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARREVIZETALEPTERTIP[(int)ENUMAlanlarREVIZETALEPTERTIP.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARREVIZETALEPTERTIP[(int)ENUMAlanlarREVIZETALEPTERTIP.KYKOD], "=", kyKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARREVIZETALEPTERTIP[(int)ENUMAlanlarREVIZETALEPTERTIP.REVIZETALEPNO], "=", revizeTalepNo));
            if (sadeceTalepler)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARREVIZETALEPTERTIP[(int)ENUMAlanlarREVIZETALEPTERTIP.TALEPMI], "=", 1));
            sql += " ORDER BY 1,2,3,4,5,6,7,8,10,9";
            return this.veriErisim.DataReaderDondur(sql);
        }

        public Sonuc RevizeTalepDurumDegistir(int yil, string kyKod, int revizeTalepNo, ENUMRevizeFormDurum durum)
        {
            string sql = "UPDATE " + TABLOADREVIZETALEP;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.FORMDURUM], (int)durum);
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.KYKOD], "=", kyKod.Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl("", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.REVIZETALEPNO], "=", revizeTalepNo);
            return KomutCalistir(sql, 0);
        }

        public ENUMRevizeFormDurum RevizeTalepDurumBul(int yil, string kyKod, int revizeTalepNo)
        {
            string sql = "SELECT " + ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.FORMDURUM] + " FROM " + TABLOADREVIZETALEP;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.KYKOD], "=", kyKod.Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl("", ALANLARREVIZETALEP[(int)ENUMAlanlarREVIZETALEP.REVIZETALEPNO], "=", revizeTalepNo);
            int tut = this.veriErisim.TekKolonSQLInt(sql);
            return (ENUMRevizeFormDurum)tut;
        }
    }
}
