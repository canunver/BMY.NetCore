using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;
using System.Collections;

namespace TNS.KYM.IsKurallari.Veri
{
    public class TABMesaj : TABOrtak
    {
        public TABMesaj(VeriErisim veriErisim) : base(veriErisim) { }


        public static string TABLOAD = "MESAJ";
        public static string TABLOADBUYUKMETIN = "BUYUKMETIN";

        public enum ENUMAlanlar : int { TARIH, KULLANICI, ALICI, MESAJ, OKUNDU }
        public enum ENUMAlanlarBUYUKMETIN : int { MODUL, ANAHTAR, DIL, SATIRNO, METIN }

        public static string[] ALANLAR = { "TARIH", "KULLANICI", "ALICI", "MESAJ", "OKUNDU" };
        public static string[] ALANLARBUYUKMETIN = { "MODUL", "ANAHTAR", "DIL", "SATIRNO", "METIN" };
        /*
         * 
         create table BUYUKMETIN
         (
            MODUL varchar(10),
            ANAHTAR varchar(20),
            DIL varchar(5),
            SATIRNO int,
            METIN varchar(260),
            PRIMARY KEY(MODUL, ANAHTAR, DIL, SATIRNO)
         )
         * 
         * */
        public IDataReader Listele(Mesaj mesaj)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql += " WHERE 1=1 ";
            if (!mesaj.tarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TARIH], ">", mesaj.tarih));
            if (!string.IsNullOrEmpty(mesaj.kullanici))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICI], "=", mesaj.kullanici));
            if (!string.IsNullOrEmpty(mesaj.alici))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALICI], "=", mesaj.alici));
            if (mesaj.okundu > -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.OKUNDU], "=", mesaj.okundu));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(Mesaj mesaj)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(mesaj.tarih) + ", ";
            sql += VeriErisim.DegerYap(mesaj.kullanici) + ", ";
            sql += VeriErisim.DegerYap(mesaj.alici) + ", ";
            sql += VeriErisim.DegerYap(mesaj.mesaj) + ", ";
            sql += VeriErisim.DegerYap(mesaj.okundu) + " )";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public IDataReader BuyukMetinOku(string modulKod, string anahtar, string dilKod, bool like)
        {
            string sql = "";
            sql = "select " + ALANLARBUYUKMETIN[(int)ENUMAlanlarBUYUKMETIN.ANAHTAR] + ", " + ALANLARBUYUKMETIN[(int)ENUMAlanlarBUYUKMETIN.SATIRNO] + ", " + ALANLARBUYUKMETIN[(int)ENUMAlanlarBUYUKMETIN.METIN];
            sql = sql + " FROM " + TABLOADBUYUKMETIN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBUYUKMETIN[(int)ENUMAlanlarBUYUKMETIN.MODUL], "=", modulKod));
            if (like)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUYUKMETIN[(int)ENUMAlanlarBUYUKMETIN.ANAHTAR], "LIKE", anahtar.Replace("_", "[_]") + "%"));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUYUKMETIN[(int)ENUMAlanlarBUYUKMETIN.ANAHTAR], "=", anahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUYUKMETIN[(int)ENUMAlanlarBUYUKMETIN.DIL], "=", dilKod));
            sql = sql + " order by 1, 2";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc BuyukMetinSatirSakla(string modulKod, string anahtar, string dilKod, int satir, string parca)
        {
            string sql = "INSERT INTO " + TABLOADBUYUKMETIN + " (" + VeriErisim.TumAlanlar("", ALANLARBUYUKMETIN) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(modulKod) + ", ";
            sql = sql + VeriErisim.DegerYap(anahtar) + ", ";
            sql = sql + VeriErisim.DegerYap(dilKod) + ", ";
            sql = sql + VeriErisim.DegerYap(satir) + ", ";
            sql = sql + VeriErisim.DegerYap(parca) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc BuyukMetinSil(string modulKod, string anahtar, string dilKod)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOADBUYUKMETIN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBUYUKMETIN[(int)ENUMAlanlarBUYUKMETIN.MODUL], "=", modulKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUYUKMETIN[(int)ENUMAlanlarBUYUKMETIN.ANAHTAR], "=", anahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBUYUKMETIN[(int)ENUMAlanlarBUYUKMETIN.DIL], "=", dilKod));
            return KomutCalistir(sql, 0);
        }
    }
}