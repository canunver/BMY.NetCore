using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABAile : TABOrtak
    {
        public TABAile(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADAILE = "IKYPERSONELAILE";
        public enum ENUMAlanlarAile : int
        {
            KOD = 0,
            PKOD = 1,
            TCKIMLIKNO = 2,
            AD = 3,
            SOYAD = 4,
            CINSIYET = 5,
            YAKINLIKDERECESI = 6,
            DURUM = 7,
            DOGUMTARIHI = 8,
            BAKMAKLAYUKUMLU = 9,
            YUKUMLULUKBITISTARIHI = 10,
            OGRENIMTIPI = 11,
            MESLEK = 12,
            SAKATLIKDERECESI=13,
            AGI = 14
        }

        static string[] ALANLARAILE = { "KOD", "PKOD", "TCKIMLIKNO", "AD", "SOYAD", "CINSIYET", "YAKINLIKDERECESI", "DURUM", "DOGUMTARIHI", "BAKMAKLAYUKUMLU", "YUKUMLULUKBITISTARIHI", "OGRENIMTIPI", "MESLEK", "SAKATLIKDERECESI", "AGI" };

        public IDataReader ListeleAile(int aileKod, int personelKod, int yukumlu)
        {
            string sql = "", kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADAILE, ALANLARAILE) + ", DATEDIFF(YEAR, DOGUMTARIHI, GETDATE()) as yas, s3.KAYITAD as ogrenim, s2.KAYITKOD as durumu, s2.KAYITAD as durumuAd" + " FROM " + TABLOADAILE;
            sql += " LEFT OUTER JOIN IKYSOZLUK s2 on s2.kod = " + TABLOADAILE + ".DURUM ";
            sql += " LEFT OUTER JOIN IKYSOZLUK s3 on s3.kod = " + TABLOADAILE + ".OGRENIMTIPI";

            if (aileKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADAILE, ALANLARAILE[(int)ENUMAlanlarAile.KOD], "=", aileKod));

            if (personelKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADAILE, ALANLARAILE[(int)ENUMAlanlarAile.PKOD], "=", personelKod));

            if (yukumlu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADAILE, ALANLARAILE[(int)ENUMAlanlarAile.BAKMAKLAYUKUMLU], "=", yukumlu));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARAILE[(int)ENUMAlanlarAile.YAKINLIKDERECESI];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc GuncelleAile(PersonelAile p)
        {
            string sql = "";
            sql = "UPDATE " + TABLOADAILE + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.TCKIMLIKNO], p.tcKimlikNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.AD], p.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.SOYAD], p.soyad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.CINSIYET], (int)p.cinsiyet) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.YAKINLIKDERECESI], (int)p.yakinlik) + ",";
            if (p.durum > 0)
                sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.DURUM], p.durum) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.DURUM], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.DOGUMTARIHI], p.dogumTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.BAKMAKLAYUKUMLU], p.bakmaklaYukumlu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.SAKATLIKDERECESI], p.sakatlikDerecesi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.AGI], p.agi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.YUKUMLULUKBITISTARIHI], p.yukumlulukBitisTarihi) + ",";
            if (p.ogrenimTipi > 0)
                sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.OGRENIMTIPI], p.ogrenimTipi) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.OGRENIMTIPI], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.MESLEK], p.meslek);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARAILE[(int)ENUMAlanlarAile.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetAile(PersonelAile p)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOADAILE + " (" + VeriErisim.TumAlanlar("", ALANLARAILE) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.personelKod) + ",";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.ad) + ",";
            sql += VeriErisim.DegerYap(p.soyad) + ",";
            sql += VeriErisim.DegerYap((int)p.cinsiyet) + ",";
            sql += VeriErisim.DegerYap((int)p.yakinlik) + ",";
            if (p.durum > 0)
                sql += VeriErisim.DegerYap(p.durum) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.dogumTarihi) + ",";
            sql += VeriErisim.DegerYap(p.bakmaklaYukumlu) + ",";
            sql += VeriErisim.DegerYap(p.yukumlulukBitisTarihi) + ",";
            if (p.ogrenimTipi > 0)
                sql += VeriErisim.DegerYap(p.ogrenimTipi) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.meslek) + ",";
            sql += VeriErisim.DegerYap(p.sakatlikDerecesi) + ",";
            sql += VeriErisim.DegerYap(p.agi) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilAile(PersonelAile p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADAILE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADAILE, ALANLARAILE[(int)ENUMAlanlarAile.KOD], "=", p.kod));

            return KomutCalistir(sql, 0);
        }

        internal static string ListeleUstTablo(string ustTabloAlan)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADAILE, ALANLARAILE) + ", DATEDIFF(YEAR, DOGUMTARIHI, GETDATE()) as yas, s3.KAYITAD as ogrenim, s2.KAYITKOD as durumu, s2.KAYITAD as durumuAd" + " FROM " + TABLOADAILE;
            sql += " LEFT OUTER JOIN IKYSOZLUK s2 on s2.kod = " + TABLOADAILE + ".DURUM ";
            sql += " LEFT OUTER JOIN IKYSOZLUK s3 on s3.kod = " + TABLOADAILE + ".OGRENIMTIPI";
            sql += " WHERE " + TABLOADAILE + "." + ALANLARAILE[(int)ENUMAlanlarAile.PKOD] + " = " + ustTabloAlan;
            sql += " ORDER BY " + ALANLARAILE[(int)ENUMAlanlarAile.YAKINLIKDERECESI];
            return sql;
        }
    }
}
