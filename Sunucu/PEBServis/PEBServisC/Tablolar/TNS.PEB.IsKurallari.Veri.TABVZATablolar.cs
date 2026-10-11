using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABVZATablolar : TABOrtak
    {
        public TABVZATablolar(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADDEGSABLON = "STRVZADEGSABLON";
        public static string TABLOADDEGSABLONDETAY = "STRVZADEGSABLONDETAY";
        public static string TABLOADDEGSABLONBIRIMTUR = "STRVZADEGSABLONBIRIMTUR";
        //public static string TABLOADKARSILASTIRMAGRUBU = "STRVZAKARGRUP";
        //public static string TABLOADKARSILASTIRMAGRUBUBIRIMTUR = "STRVZAKARGRUPBIRIMTUR";

        public static string TABLOADANALIZTANIM = "STRVZAANALIZTANIM";
        public static string TABLOADANALIZCOGRAFYA = "STRVZAANALIZCOGRAFYA";
        public static string TABLOADANALIZISLEMANA = "STRVZAANALIZISLEMANA";
        public static string TABLOADANALIZISLEMDETAY = "STRVZAANALIZISLEMDETAY";

        public static string TABLOADSONUC = "STRVZASONUC";
        public static string TABLOADSONUCDEGISKEN = "STRVZASONUCDEGISKEN";

        public enum ENUMAlanlarDEGSABLON : int
        {
            KOD = 0,
            METIN = 1
        }

        public enum ENUMAlanlarDEGSABLONDETAY : int
        {
            KOD = 0,
            GIRDICIKTI = 1,
            GOSTERGEKOD = 2
        }

        public enum ENUMAlanlarDEGSABLONBIRIMTUR : int
        {
            KOD = 0,
            BIRIMTUR = 3
        }

        //public enum ENUMAlanlarKARSILASTIRMAGRUBU : int
        //{
        //    KOD = 0,
        //    METIN = 1,
        //    COGRAFIBIRIM = 2,
        //    KARGRUP = 3
        //}

        //public enum ENUMAlanlarKARSILASTIRMAGRUBUBIRIMTUR : int
        //{
        //    KOD = 0,
        //    BIRIMTUR = 1
        //}

        public enum ENUMAlanlarANALIZTANIM : int
        {
            KOD = 0,
            ALTKOD = 1,
            METIN = 2,
            DEGISKENKOD = 3,
            KARGRUPKOD = 4,
            COZUMYONTEM = 5,
            VARSAYILAN = 6
        }

        public enum ENUMAlanlarANALIZCOGRAFYA : int
        {
            KOD = 0,
            ALTKOD = 1,
            COGRAFIKOD = 2
        }

        public enum ENUMAlanlarANALIZISLEMANA : int
        {
            KOD = 0,
            ALTKOD = 1,
            YIL = 2,
            DURUM = 3,
            BASLATAN = 4,
            TARIH = 5
        }

        public enum ENUMAlanlarANALIZISLEMDETAY : int
        {
            KOD = 0,
            ALTKOD = 1,
            YIL = 2,
            COGRAFIKOD = 3,
            DURUM = 4
        }

        public enum ENUMAlanlarSONUC : int
        {
            ORGKOD = 0, YIL = 1, GRUPKOD = 2, ALTKOD = 3, COGRAFIKOD = 4, AMAC = 5, ORNEK = 6
        }

        public enum ENUMAlanlarSONUCDEGISKEN : int
        {
            ORGKOD = 0, YIL = 1, GRUPKOD = 2, ALTKOD = 3, COGRAFIKOD = 4, GOSTERGEKOD = 5, GDEGER = 6, CDEGER = 7, ORAN = 8
        }

        public static string[] ALANLARDEGSABLON = { "KOD", "METIN" };
        public static string[] ALANLARDEGSABLONDETAY = { "KOD", "GIRDICIKTI", "GOSTERGEKOD" };
        public static string[] ALANLARDEGSABLONBIRIMTUR = { "KOD", "BIRIMTUR" };
        //public static string[] ALANLARKARSILASTIRMAGRUBU = { "KOD", "METIN", "COGRAFIBIRIM", "KARGRUP" };
        //public static string[] ALANLARKARSILASTIRMAGRUBUBIRIMTUR = { "KOD", "BIRIMTUR" };

        public static string[] ALANLARANALIZTANIM = { "KOD", "ALTKOD", "METIN", "DEGISKENKOD", "KARGRUPKOD", "COZUMYONTEM", "VARSAYILAN" };
        public static string[] ALANLARANALIZCOGRAFYA = { "KOD", "ALTKOD", "COGRAFIKOD" };
        public static string[] ALANLARANALIZISLEMANA = { "KOD", "ALTKOD", "YIL", "DURUM", "BASLATAN", "TARIH" };
        public static string[] ALANLARANALIZISLEMDETAY = { "KOD", "ALTKOD", "YIL", "COGRAFIKOD", "DURUM" };

        public static string[] ALANLARSONUC = { "ORGKOD", "YIL", "GRUPKOD", "ALTKOD", "COGRAFIKOD", "AMAC", "ORNEK" };
        public static string[] ALANLARSONUCDEGISKEN = { "ORGKOD", "YIL", "GRUPKOD", "ALTKOD", "COGRAFIKOD", "GOSTERGEKOD", "GDEGER", "CDEGER", "ORAN" };

        /*        
         create table STRVZADEGSABLON
            (
            KOD          int,
            METIN        varchar(255),
            primary key (KOD)
            )
         create table STRVZADEGSABLONDETAY
            (
            KOD         int,
            GIRDICIKTI  int,
            GOSTERGEKOD int,
            primary key (KOD, GIRDICIKTI, GOSTERGEKOD)
            )
         create table STRVZADEGSABLONBIRIMTUR
            (
            KOD         int,
            BIRIMTUR  VARCHAR(20)
            primary key (KOD, BIRIMTUR)
            )
         create table STRVZAKARGRUP
            (
            KOD          int,
            METIN        varchar(255),
            COGRAFIBIRIM int,
            KARGRUP      int,
            primary key (KOD)
            )
         create table STRVZAKARGRUPBIRIMTUR
            (
            KOD         int,
            BIRIMTUR  VARCHAR(20)
            primary key (KOD, BIRIMTUR)
            )
        create table STRVZAANALIZTANIM
            (
            KOD          int,
            ALTKOD       int,
            METIN        varchar(255),
            DEGISKENKOD  int,
            KARGRUPKOD   int,
            COZUMYONTEM  int,
            primary key (KOD, ALTKOD)
            )

        create table STRVZAANALIZCOGRAFYA 
            (
            KOD int, ALTKOD int, COGRAFIKOD varchar(20),
            primary key (KOD, ALTKOD, COGRAFIKOD)
            )
        create table  STRVZAANALIZISLEMANA
            (
            KOD int, ALTKOD int, YIL int, DURUM int, BASLATAN varchar(65), TARIH datetime,
            primary key (KOD, ALTKOD, YIL)
            )
        create table  STRVZAANALIZISLEMDETAY
            (
            KOD int, ALTKOD int, YIL int, COGRAFIKOD varchar(20), DURUM int,
            primary key (KOD, ALTKOD, YIL, COGRAFIKOD)
            )
        create table  STRVZASONUC
            (
            ORGKOD int, YIL int, GRUPKOD int, ALTKOD int, COGRAFIKOD varchar(20), AMAC money, ORNEK int,
            primary key (ORGKOD, YIL, GRUPKOD, ALTKOD)
            )
        create table  STRVZASONUCDEGISKEN
            (
            ORGKOD int, YIL int, GRUPKOD int, ALTKOD int, COGRAFIKOD varchar(20), GOSTERGEKOD int, GDEGER money, CDEGER money, ORAN money,
            primary key (ORGKOD , YIL, GRUPKOD, ALTKOD, GOSTERGEKOD)
            )
         * 
         * 
        */
        public int EnYuksekSablonIDBul()
        {
            string sql = "SELECT MAX(" + ALANLARDEGSABLON[(int)ENUMAlanlarDEGSABLON.KOD] + ") FROM " + TABLOADDEGSABLON;

            return (int)veriErisim.TekKolonSQLDbl(sql);
        }

        public Sonuc DegiskenSablonEkle(DegiskenSablon d)
        {
            string sql = "INSERT INTO " + TABLOADDEGSABLON + " (" + VeriErisim.TumAlanlar("", ALANLARDEGSABLON) + ") VALUES (";
            sql += VeriErisim.DegerYap(d.kod) + ",";
            sql += VeriErisim.DegerYap(d.metin) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader DegiskenSablonListele()
        {
            string sql = "SELECT " + ALANLARDEGSABLON[(int)ENUMAlanlarDEGSABLON.KOD] + ", " + ALANLARDEGSABLON[(int)ENUMAlanlarDEGSABLON.METIN] + " FROM " + TABLOADDEGSABLON;
            sql += " ORDER BY " + ALANLARDEGSABLON[(int)ENUMAlanlarDEGSABLON.KOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc DegiskenSablonAdDegistir(int kod, string metin)
        {
            string sql = "UPDATE " + TABLOADDEGSABLON + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDEGSABLON[(int)ENUMAlanlarDEGSABLON.METIN], metin);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDEGSABLON[(int)ENUMAlanlarDEGSABLON.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc DegiskenSablonSil(int kod)
        {
            string sql = "DELETE FROM " + TABLOADDEGSABLON;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDEGSABLON[(int)ENUMAlanlarDEGSABLON.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }

        //public IDataReader KarsilastirmaGruplariListele(int kod)
        //{
        //    string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARKARSILASTIRMAGRUBU) + " FROM " + TABLOADKARSILASTIRMAGRUBU;
        //    if (kod >= 0)
        //        sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKARSILASTIRMAGRUBU[(int)ENUMAlanlarKARSILASTIRMAGRUBU.KOD], "=", kod));
        //    sql += " ORDER BY " + ALANLARDEGSABLON[(int)ENUMAlanlarKARSILASTIRMAGRUBU.KOD];

        //    return veriErisim.DataReaderDondur(sql);
        //}

        //public Sonuc KarsilastirmaGrubuAdDegistir(int kod, string metin)
        //{
        //    string sql = "UPDATE " + TABLOADKARSILASTIRMAGRUBU + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLARKARSILASTIRMAGRUBU[(int)ENUMAlanlarKARSILASTIRMAGRUBU.METIN], metin);
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKARSILASTIRMAGRUBU[(int)ENUMAlanlarKARSILASTIRMAGRUBU.KOD], "=", kod));

        //    return KomutCalistir(sql, 1);
        //}

        //public Sonuc KarsilastirmaGrubuSil(int kod)
        //{
        //    string sql = "DELETE FROM " + TABLOADKARSILASTIRMAGRUBU;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKARSILASTIRMAGRUBU[(int)ENUMAlanlarKARSILASTIRMAGRUBU.KOD], "=", kod));

        //    return KomutCalistir(sql, 1);
        //}

        //public int EnYuksekKGIDBul()
        //{
        //    string sql = "SELECT MAX(" + ALANLARKARSILASTIRMAGRUBU[(int)ENUMAlanlarKARSILASTIRMAGRUBU.KOD] + ") FROM " + TABLOADKARSILASTIRMAGRUBU;

        //    return (int)veriErisim.TekKolonSQLDbl(sql);
        //}

        //public Sonuc KarsilastirmaGrubuTanimEkle(KarsilastirmaGrubu d)
        //{
        //    string sql = "INSERT INTO " + TABLOADKARSILASTIRMAGRUBU + " (" + VeriErisim.TumAlanlar("", ALANLARKARSILASTIRMAGRUBU) + ") VALUES (";
        //    sql += VeriErisim.DegerYap(d.kod) + ",";
        //    sql += VeriErisim.DegerYap(d.metin) + ",1,1)";

        //    return KomutCalistir(sql, 1);
        //}

        public Sonuc DegiskenleriSil(int kod)
        {
            string sql = "DELETE FROM " + TABLOADDEGSABLONDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDEGSABLONDETAY[(int)ENUMAlanlarDEGSABLONDETAY.KOD], "=", kod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc BirimleriSil(int kod)
        {
            string sql = "DELETE FROM " + TABLOADDEGSABLONBIRIMTUR;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDEGSABLONBIRIMTUR[(int)ENUMAlanlarDEGSABLONBIRIMTUR.KOD], "=", kod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc DegiskenEkle(DegiskenSablon d, int girdiCikti, int i)
        {
            int gostergeKod;
            try
            {
                if (girdiCikti == 1) gostergeKod = d.girdiler[i].kod;
                else if (girdiCikti == 2) gostergeKod = d.ciktilar[i].kod;
                else
                    return new Sonuc(false, "Girdi çıktı kodu 1 veya 2 olmalı!", "");
            }
            catch
            {
                return new Sonuc(false, "Girdi çıktı dizisinde aşma oldu!", "");
            }

            string sql = "INSERT INTO " + TABLOADDEGSABLONDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDEGSABLONDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(d.kod) + ",";
            sql += VeriErisim.DegerYap(girdiCikti) + ",";
            sql += VeriErisim.DegerYap(gostergeKod) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc OrgTurEkle(DegiskenSablon d, int i)
        {
            string birimTurKod;
            try
            {
                birimTurKod = ((SpGenelHavuzOrganizasyon)d.orgTurler[i]).orgKod;
            }
            catch
            {
                return new Sonuc(false, "Organizasyon türler dizisinde aşma oldu!", "");
            }
            string sql = "INSERT INTO " + TABLOADDEGSABLONBIRIMTUR + " (" + VeriErisim.TumAlanlar("", ALANLARDEGSABLONBIRIMTUR) + ") VALUES (";
            sql += VeriErisim.DegerYap(d.kod) + ",";
            sql += VeriErisim.DegerYap(birimTurKod.Replace(".", "")) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader DegiskenleriOku(int kod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARDEGSABLONDETAY) + " FROM " + TABLOADDEGSABLONDETAY;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARDEGSABLONDETAY[(int)ENUMAlanlarDEGSABLONDETAY.KOD], "=", kod);

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader SablonOrgTurListele(int kod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARDEGSABLONBIRIMTUR) + " FROM " + TABLOADDEGSABLONBIRIMTUR;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARDEGSABLONBIRIMTUR[(int)ENUMAlanlarDEGSABLONBIRIMTUR.KOD], "=", kod);

            return veriErisim.DataReaderDondur(sql);
        }

        //public Sonuc KGBirimleriSil(int kod)
        //{
        //    string sql = "DELETE FROM " + TABLOADKARSILASTIRMAGRUBUBIRIMTUR;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKARSILASTIRMAGRUBUBIRIMTUR[(int)ENUMAlanlarKARSILASTIRMAGRUBUBIRIMTUR.KOD], "=", kod));

        //    return KomutCalistir(sql, 0);
        //}

        //public Sonuc KGOrgTurEkle(KarsilastirmaGrubu d, int i)
        //{
        //    string birimTurKod;
        //    try
        //    {
        //        birimTurKod = ((SpGenelHavuzOrganizasyon)d.orgTurler[i]).orgKod;
        //    }
        //    catch
        //    {
        //        return new Sonuc(false, "Organizasyon türler dizisinde aşma oldu!", "");
        //    }
        //    string sql = "INSERT INTO " + TABLOADKARSILASTIRMAGRUBUBIRIMTUR + " (" + VeriErisim.TumAlanlar("", ALANLARKARSILASTIRMAGRUBUBIRIMTUR) + ") VALUES (";
        //    sql += VeriErisim.DegerYap(d.kod) + ",";
        //    sql += VeriErisim.DegerYap(birimTurKod.Replace(".", "")) + ")";

        //    return KomutCalistir(sql, 1);
        //}

        //public Sonuc KGBilgiGuncelle(KarsilastirmaGrubu d)
        //{
        //    string sql = "UPDATE " + TABLOADKARSILASTIRMAGRUBU + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLARKARSILASTIRMAGRUBU[(int)ENUMAlanlarKARSILASTIRMAGRUBU.COGRAFIBIRIM], (int)d.cografiBirim);
        //    sql += "," + VeriErisim.AtamaOl("", ALANLARKARSILASTIRMAGRUBU[(int)ENUMAlanlarKARSILASTIRMAGRUBU.KARGRUP], (int)d.karsilastirmaGrubu);
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKARSILASTIRMAGRUBU[(int)ENUMAlanlarKARSILASTIRMAGRUBU.KOD], "=", d.kod));

        //    return KomutCalistir(sql, 1);
        //}

        //public IDataReader KGOrgTurListele(int kod)
        //{
        //    string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARKARSILASTIRMAGRUBUBIRIMTUR) + " FROM " + TABLOADKARSILASTIRMAGRUBUBIRIMTUR;
        //    sql += " WHERE " + VeriErisim.KosulOl("", ALANLARKARSILASTIRMAGRUBUBIRIMTUR[(int)ENUMAlanlarKARSILASTIRMAGRUBUBIRIMTUR.KOD], "=", kod);

        //    return veriErisim.DataReaderDondur(sql);
        //}

        public int EnYuksekAnalizGrupIDBul(int kod)
        {
            string sql;
            if (kod == 0)
                sql = "SELECT MAX(" + ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.KOD] + ") FROM " + TABLOADANALIZTANIM;
            else
            {
                sql = "SELECT MAX(" + ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.ALTKOD] + ") FROM " + TABLOADANALIZTANIM;
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.KOD], "=", kod);
            }

            return (int)veriErisim.TekKolonSQLDbl(sql);
        }

        public Sonuc AnalizGrupEkle(int kod, int altKod, string metin, int degiskenKod, ENUMCografiBirim kgKod, int cozumYontem)
        {
            string sql = "INSERT INTO " + TABLOADANALIZTANIM + " (" + VeriErisim.TumAlanlar("", ALANLARANALIZTANIM) + ") VALUES (";
            sql += VeriErisim.DegerYap(kod) + ",";
            sql += VeriErisim.DegerYap(altKod) + ",";
            sql += VeriErisim.DegerYap(metin) + ",";
            sql += VeriErisim.DegerYap(degiskenKod) + ",";
            sql += VeriErisim.DegerYap((int)kgKod) + ",";
            sql += VeriErisim.DegerYap(cozumYontem) + ", 0)";

            return KomutCalistir(sql, 1);
        }

        public Sonuc AnalizGrupSil(int grupKod, int altKod)
        {
            string sql = "DELETE FROM " + TABLOADANALIZTANIM;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADANALIZTANIM, ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.KOD], "=", grupKod));
            if (altKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADANALIZTANIM, ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.ALTKOD], "=", altKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader AnalizGrupListele()
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARANALIZTANIM) + " FROM " + TABLOADANALIZTANIM + " ORDER BY 1,2";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc AnalizGrupAdDegistir(int kod, int altKod, string metin)
        {
            string sql = "UPDATE " + TABLOADANALIZTANIM + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.METIN], metin);
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.KOD], "=", kod);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.ALTKOD], "=", altKod);

            return KomutCalistir(sql, 1);
        }

        public IDataReader AnalizDetayOku(int grupKod, int altKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARANALIZTANIM) + " FROM " + TABLOADANALIZTANIM;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.KOD], "=", grupKod);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.ALTKOD], "=", altKod);

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc AnalizGrupCografiKodEkle(int grupKod, int altKod, string cogKod)
        {
            string sql = "INSERT INTO " + TABLOADANALIZCOGRAFYA + " (" + VeriErisim.TumAlanlar("", ALANLARANALIZCOGRAFYA) + ") VALUES (";
            sql += VeriErisim.DegerYap(grupKod) + ",";
            sql += VeriErisim.DegerYap(altKod) + ",";
            sql += VeriErisim.DegerYap(cogKod.Replace(".", "")) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc AnalizGrupCografiKodSil(int grupKod, int altKod)
        {
            string sql = "DELETE FROM " + TABLOADANALIZCOGRAFYA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADANALIZCOGRAFYA, ALANLARANALIZCOGRAFYA[(int)ENUMAlanlarANALIZCOGRAFYA.KOD], "=", grupKod));
            if (altKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADANALIZCOGRAFYA, ALANLARANALIZCOGRAFYA[(int)ENUMAlanlarANALIZCOGRAFYA.ALTKOD], "=", altKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader CografiKodlariListele(int grupKod, int altKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARANALIZCOGRAFYA) + " FROM " + TABLOADANALIZCOGRAFYA;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARANALIZCOGRAFYA[(int)ENUMAlanlarANALIZCOGRAFYA.KOD], "=", grupKod);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARANALIZCOGRAFYA[(int)ENUMAlanlarANALIZCOGRAFYA.ALTKOD], "=", altKod);

            return veriErisim.DataReaderDondur(sql);
        }

        public int VZAIslemDurumBul(int islemYil, int grupKod, int altKod)
        {
            string sql = "SELECT " + ALANLARANALIZISLEMANA[(int)ENUMAlanlarANALIZISLEMANA.DURUM] + " FROM " + TABLOADANALIZISLEMANA;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARANALIZISLEMANA[(int)ENUMAlanlarANALIZISLEMANA.KOD], "=", grupKod);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARANALIZISLEMANA[(int)ENUMAlanlarANALIZISLEMANA.ALTKOD], "=", altKod);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARANALIZISLEMANA[(int)ENUMAlanlarANALIZISLEMANA.YIL], "=", islemYil);

            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc VZAIslemAnaKayitEkle(int islemYil, int grupKod, int altKod, string kullaniciKod, TNSDateTime islemZamani)
        {
            string sql = "INSERT INTO " + TABLOADANALIZISLEMANA + " (" + VeriErisim.TumAlanlar("", ALANLARANALIZISLEMANA) + ") VALUES (";
            sql += VeriErisim.DegerYap(grupKod) + ",";
            sql += VeriErisim.DegerYap(altKod) + ",";
            sql += VeriErisim.DegerYap(islemYil) + ",";
            sql += VeriErisim.DegerYap(1) + ",";
            sql += VeriErisim.DegerYap(kullaniciKod) + ",";
            sql += VeriErisim.DegerYap(islemZamani) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc VZAIslemAnaSil(int grupKod, int altKod)
        {
            string sql = "DELETE FROM " + TABLOADANALIZISLEMANA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADANALIZISLEMANA, ALANLARANALIZISLEMANA[(int)ENUMAlanlarANALIZISLEMANA.KOD], "=", grupKod));
            if (altKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADANALIZISLEMANA, ALANLARANALIZISLEMANA[(int)ENUMAlanlarANALIZISLEMANA.ALTKOD], "=", altKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc VZAIslemDetayKayitEkle(int islemYil, int grupKod, int altKod, string cogKod)
        {
            string sql = "INSERT INTO " + TABLOADANALIZISLEMDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARANALIZISLEMDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(grupKod) + ",";
            sql += VeriErisim.DegerYap(altKod) + ",";
            sql += VeriErisim.DegerYap(islemYil) + ",";
            sql += VeriErisim.DegerYap(cogKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(1) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc VZAIslemDetaySil(int grupKod, int altKod)
        {
            string sql = "DELETE FROM " + TABLOADANALIZISLEMDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADANALIZISLEMDETAY, ALANLARANALIZISLEMDETAY[(int)ENUMAlanlarANALIZISLEMDETAY.KOD], "=", grupKod));
            if (altKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADANALIZISLEMDETAY, ALANLARANALIZISLEMDETAY[(int)ENUMAlanlarANALIZISLEMDETAY.ALTKOD], "=", altKod));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grupKod"></param>
        /// <param name="altKod"></param>
        /// <param name="islemYil"></param>
        /// <param name="listelenecekler">0: hepsi, 1: bekleyenler, 2: işleme alınmışlar (Hepsi), 3: başarılılar, 4: başarısızlar</param>
        /// <param name="kayitSay">Döndürülecek kayıt sayısı 1 ise durumu'a göre sıralama yapılır.</param>
        /// <returns></returns>
        public IDataReader IslemDetayListele(int grupKod, int altKod, int islemYil, int listelenecekler, int kayitSay)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARANALIZISLEMDETAY) + " FROM " + TABLOADANALIZISLEMDETAY;
            string kosul = "";
            if (islemYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARANALIZISLEMDETAY[(int)ENUMAlanlarANALIZISLEMDETAY.YIL], "=", islemYil));
            if (grupKod > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARANALIZISLEMDETAY[(int)ENUMAlanlarANALIZISLEMDETAY.KOD], "=", grupKod));
                if (altKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARANALIZISLEMDETAY[(int)ENUMAlanlarANALIZISLEMDETAY.ALTKOD], "=", altKod));
            }
            if (listelenecekler > 0)
            {
                if (listelenecekler == 1)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARANALIZISLEMDETAY[(int)ENUMAlanlarANALIZISLEMDETAY.DURUM], "<", 5));
                else if (listelenecekler == 2)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARANALIZISLEMDETAY[(int)ENUMAlanlarANALIZISLEMDETAY.DURUM], ">=", 5));
                else if (listelenecekler == 3)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARANALIZISLEMDETAY[(int)ENUMAlanlarANALIZISLEMDETAY.DURUM], "=", 5));
                else if (listelenecekler == 4)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARANALIZISLEMDETAY[(int)ENUMAlanlarANALIZISLEMDETAY.DURUM], ">", 5));
            }
            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            if (kayitSay == 1)
                sql += " ORDER BY " + ALANLARANALIZISLEMDETAY[(int)ENUMAlanlarANALIZISLEMDETAY.DURUM];
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc IslemDurumGuncelle(int islemYil, int grupKod, int altKod, string cogKod, int durum)
        {
            string sql = "UPDATE " + TABLOADANALIZISLEMDETAY + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLARANALIZISLEMDETAY[(int)ENUMAlanlarANALIZISLEMDETAY.DURUM], durum);
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARANALIZISLEMDETAY[(int)ENUMAlanlarANALIZISLEMDETAY.KOD], "=", grupKod);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARANALIZISLEMDETAY[(int)ENUMAlanlarANALIZISLEMDETAY.ALTKOD], "=", altKod);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARANALIZISLEMDETAY[(int)ENUMAlanlarANALIZISLEMDETAY.YIL], "=", islemYil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARANALIZISLEMDETAY[(int)ENUMAlanlarANALIZISLEMDETAY.COGRAFIKOD], "=", cogKod.Replace(".", ""));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SonucEkle(int islemYil, int grupKod, int altKod, string cogKod, int orgKod, double amac, int ornekSay)
        {
            string sql = "INSERT INTO " + TABLOADSONUC + " (" + VeriErisim.TumAlanlar("", ALANLARSONUC) + ") VALUES (";
            sql += VeriErisim.DegerYap(orgKod) + ",";
            sql += VeriErisim.DegerYap(islemYil) + ",";
            sql += VeriErisim.DegerYap(grupKod) + ",";
            sql += VeriErisim.DegerYap(altKod) + ",";
            sql += VeriErisim.DegerYap(cogKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(amac) + ",";
            sql += VeriErisim.DegerYap(ornekSay) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SonucSil(int grupKod, int altKod)
        {
            string sql = "DELETE FROM " + TABLOADSONUC;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADSONUC, ALANLARSONUC[(int)ENUMAlanlarSONUC.GRUPKOD], "=", grupKod));
            if (altKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSONUC, ALANLARSONUC[(int)ENUMAlanlarSONUC.ALTKOD], "=", altKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SonucDegiskenEkle(int islemYil, int grupKod, int altKod, string cogKod, int orgKod, int gostergeKod, double girisDeger, double cozumDeger, double degisimOran)
        {
            string sql = "INSERT INTO " + TABLOADSONUCDEGISKEN + " (" + VeriErisim.TumAlanlar("", ALANLARSONUCDEGISKEN) + ") VALUES (";
            sql += VeriErisim.DegerYap(orgKod) + ",";
            sql += VeriErisim.DegerYap(islemYil) + ",";
            sql += VeriErisim.DegerYap(grupKod) + ",";
            sql += VeriErisim.DegerYap(altKod) + ",";
            sql += VeriErisim.DegerYap(cogKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(gostergeKod) + ",";
            sql += VeriErisim.DegerYap(girisDeger) + ",";
            sql += VeriErisim.DegerYap(cozumDeger) + ",";
            sql += VeriErisim.DegerYap(degisimOran) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SonucDegiskenSil(int grupKod, int altKod)
        {
            string sql = "DELETE FROM " + TABLOADSONUCDEGISKEN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADSONUCDEGISKEN, ALANLARSONUCDEGISKEN[(int)ENUMAlanlarSONUCDEGISKEN.GRUPKOD], "=", grupKod));
            if (altKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSONUCDEGISKEN, ALANLARSONUCDEGISKEN[(int)ENUMAlanlarSONUCDEGISKEN.ALTKOD], "=", altKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader SonucBirimlerOku(int islemYil, int grupKod, int altKod, string cogKod)
        {
            string sql = "SELECT " + ALANLARSONUC[(int)ENUMAlanlarSONUC.ORGKOD] + ", " + ALANLARSONUC[(int)ENUMAlanlarSONUC.AMAC] + ", " + ALANLARSONUC[(int)ENUMAlanlarSONUC.ORNEK] + " FROM " + TABLOADSONUC;
            string kosul = "";
            if (islemYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUC[(int)ENUMAlanlarSONUC.YIL], "=", islemYil));
            if (grupKod > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUC[(int)ENUMAlanlarSONUC.GRUPKOD], "=", grupKod));
                if (altKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUC[(int)ENUMAlanlarSONUC.ALTKOD], "=", altKod));
            }
            if (!string.IsNullOrEmpty(cogKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUC[(int)ENUMAlanlarSONUC.COGRAFIKOD], "=", cogKod.Replace(".", "")));


            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader SonucDegiskenOku(int orgKod, int islemYil, int grupKod, int altKod, int sablonKod, int girdiCikti)
        {
            string sql = "SELECT " + TABLOADSONUCDEGISKEN + "." + ALANLARSONUCDEGISKEN[(int)ENUMAlanlarSONUCDEGISKEN.GOSTERGEKOD] + ", " + ALANLARSONUCDEGISKEN[(int)ENUMAlanlarSONUCDEGISKEN.GDEGER] + ", " + ALANLARSONUCDEGISKEN[(int)ENUMAlanlarSONUCDEGISKEN.CDEGER] + ", " + ALANLARSONUCDEGISKEN[(int)ENUMAlanlarSONUCDEGISKEN.ORAN] + " FROM " + TABLOADSONUCDEGISKEN + ", " + TABLOADDEGSABLONDETAY;
            string kosul = "";
            if (orgKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUCDEGISKEN[(int)ENUMAlanlarSONUCDEGISKEN.ORGKOD], "=", orgKod));
            if (islemYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUCDEGISKEN[(int)ENUMAlanlarSONUCDEGISKEN.YIL], "=", islemYil));
            if (grupKod > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUCDEGISKEN[(int)ENUMAlanlarSONUCDEGISKEN.GRUPKOD], "=", grupKod));
                if (altKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUCDEGISKEN[(int)ENUMAlanlarSONUCDEGISKEN.ALTKOD], "=", altKod));
            }
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDEGSABLONDETAY, ALANLARDEGSABLONDETAY[(int)ENUMAlanlarDEGSABLONDETAY.KOD], "=", sablonKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDEGSABLONDETAY, ALANLARDEGSABLONDETAY[(int)ENUMAlanlarDEGSABLONDETAY.GIRDICIKTI], "=", girdiCikti));
            kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOADDEGSABLONDETAY + "." + ALANLARDEGSABLONDETAY[(int)ENUMAlanlarDEGSABLONDETAY.GOSTERGEKOD] + "=" + TABLOADSONUCDEGISKEN + "." + ALANLARSONUCDEGISKEN[(int)ENUMAlanlarSONUCDEGISKEN.GOSTERGEKOD]);

            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);

            return veriErisim.DataReaderDondur(sql);
        }


        public Sonuc IslemSonucDegiskenSil(int islemYil, int grupKod, int altKod, string cogKod)
        {
            string sql = "DELETE FROM " + TABLOADSONUCDEGISKEN;
            string kosul = "";
            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUCDEGISKEN[(int)ENUMAlanlarSONUCDEGISKEN.ORGKOD], "=", orgKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUCDEGISKEN[(int)ENUMAlanlarSONUCDEGISKEN.YIL], "=", islemYil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUCDEGISKEN[(int)ENUMAlanlarSONUCDEGISKEN.GRUPKOD], "=", grupKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUCDEGISKEN[(int)ENUMAlanlarSONUCDEGISKEN.ALTKOD], "=", altKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUCDEGISKEN[(int)ENUMAlanlarSONUCDEGISKEN.COGRAFIKOD], "=", cogKod.Replace(".", "")));

            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);

            return KomutCalistir(sql, 0);
        }

        public Sonuc IslemSonucAnaSil(int islemYil, int grupKod, int altKod, string cogKod)
        {
            string sql = "DELETE FROM " + TABLOADSONUC;
            string kosul = "";
            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUC[(int)ENUMAlanlarSONUC.ORGKOD], "=", orgKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUC[(int)ENUMAlanlarSONUC.YIL], "=", islemYil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUC[(int)ENUMAlanlarSONUC.GRUPKOD], "=", grupKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUC[(int)ENUMAlanlarSONUC.ALTKOD], "=", altKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSONUC[(int)ENUMAlanlarSONUC.COGRAFIKOD], "=", cogKod.Replace(".", "")));

            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);

            return KomutCalistir(sql, 0);
        }

        public double VZANotBul(int yil, int orgKod)
        {
            string sql = "SELECT AVG(AMAC) FROM " + TABLOADSONUC + " S, " + TABLOADANALIZTANIM + " T";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("S", ALANLARSONUC[(int)ENUMAlanlarSONUC.ORGKOD], "=", orgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", ALANLARSONUC[(int)ENUMAlanlarSONUC.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", "T." + ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.KOD] + " = S." + ALANLARSONUC[(int)ENUMAlanlarSONUC.GRUPKOD]);
            sql = VeriErisim.KosulEkle(sql, "AND", "T." + ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.ALTKOD] + " = 0");
            sql = VeriErisim.KosulEkle(sql, "AND", "T." + ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.VARSAYILAN] + " = 1");

            return veriErisim.TekKolonSQLDbl(sql);
        }

        public Sonuc VZAAnalizGrupVarsayilanYap(int grupKod, int varsayilan)
        {
            string sql = "UPDATE " + TABLOADANALIZTANIM + " SET " + ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.VARSAYILAN] + " = " + VeriErisim.DegerYap(varsayilan);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.KOD], "=", grupKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.ALTKOD], "=", 0));

            return KomutCalistir(sql, 1);
        }

        public IDataReader VZABirimGrupAltGrupBul(int yil, int orgKod)
        {
            string sql = "SELECT DISTINCT S.GRUPKOD, S.ALTKOD, S.AMAC, S.ORNEK FROM " + TABLOADSONUC + " S, " + TABLOADANALIZTANIM + " T";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("S", ALANLARSONUC[(int)ENUMAlanlarSONUC.ORGKOD], "=", orgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", ALANLARSONUC[(int)ENUMAlanlarSONUC.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", "T." + ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.KOD] + " = S." + ALANLARSONUC[(int)ENUMAlanlarSONUC.GRUPKOD]);
            sql = VeriErisim.KosulEkle(sql, "AND", "T." + ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.ALTKOD] + " = 0");
            sql = VeriErisim.KosulEkle(sql, "AND", "T." + ALANLARANALIZTANIM[(int)ENUMAlanlarANALIZTANIM.VARSAYILAN] + " = 1");
            sql += " ORDER BY 1,2";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}