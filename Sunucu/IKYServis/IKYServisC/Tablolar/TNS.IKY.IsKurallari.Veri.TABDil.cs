using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABPersonelDil : TABOrtak
    {
        public TABPersonelDil(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "IKYPERSONELDIL";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            PKOD = 1,
            DILKOD = 2,
            PUAN = 3,
            TARIH = 4,
            DUZEY = 5,
            DONEMAY = 6,
            DONEMYIL = 7,
            BELGETUR = 8,
            BELGETARIHI = 9,
            BELGENO = 10,
            BORDROYAESAS = 11,
            DILSEVIYESI = 12,
            KURUMFAYDALANIYOR = 13
        }
        static string[] ALANLAR = { "KOD", "PKOD", "DILKOD", "PUAN", "TARIH", "DUZEY", "DONEMAY", "DONEMYIL", "BELGETUR", "BELGETARIHI", "BELGENO", "BORDROYAESAS", "DILSEVIYESI", "KURUMFAYDALANIYOR" };

        public IDataReader Listele(PersonelDil p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " , s1.KAYITAD as dilAdi, s2.KAYITAD as belgeAdi ";
            sql += " FROM " + TABLOAD;
            sql += " left outer join IKYSOZLUK S1 ON S1.KOD = " + ALANLAR[(int)ENUMAlanlar.DILKOD];
            sql += " left outer join IKYSOZLUK S2 ON S2.KOD = " + ALANLAR[(int)ENUMAlanlar.BELGETUR];

            if (p.personelKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.personelKod));

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            //if (p.dilKod > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DILKODU], "=", p.dilKod));

            //if (!string.IsNullOrEmpty(p.duzey))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DUZEY], "=", p.duzey));

            //if (p.puan > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PUAN], "=", p.puan));

            //if (p.donemAy > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMAY], "=", p.donemAy));

            //if (p.donemYil > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMYIL], "=", p.donemYil));

            //if (p.belgeTur > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", p.belgeTur));

            //if (!p.belgeTarihi.isNull)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETARIHI], "=", p.belgeTarihi));

            //if (!string.IsNullOrEmpty(p.belgeNo))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], "LIKE", "%" + p.belgeNo + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(PersonelDil p)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.personelKod) + ",";
            if (p.dilKod > 0)
                sql += VeriErisim.DegerYap(p.dilKod) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.puan) + ",";
            sql += VeriErisim.DegerYap(p.tarih) + ",";
            sql += VeriErisim.DegerYap(p.duzey) + ",";
            sql += VeriErisim.DegerYap(p.donemAy) + ",";
            sql += VeriErisim.DegerYap(p.donemYil) + ",";
            if (p.belgeTur > 0)
                sql += VeriErisim.DegerYap(p.belgeTur) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.belgeTarihi) + ",";
            sql += VeriErisim.DegerYap(p.belgeNo) + ",";
            sql += VeriErisim.DegerYap(p.bordroyaEsas) + ",";
            sql += VeriErisim.DegerYap(p.dilSeviyesi) + ",";
            sql += VeriErisim.DegerYap(p.kurumFaydalaniyor) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(PersonelDil p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            if (p.dilKod > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DILKOD], p.dilKod) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DILKOD], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PUAN], p.puan) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH], p.tarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DUZEY], p.duzey) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DONEMAY], p.donemAy) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DONEMYIL], p.donemYil) + ",";
            if (p.belgeTur > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], p.belgeTur) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETARIHI], p.belgeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], p.belgeNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BORDROYAESAS], p.bordroyaEsas) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DILSEVIYESI], p.dilSeviyesi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURUMFAYDALANIYOR], p.kurumFaydalaniyor);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(PersonelDil p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        internal static string ListeleUstTablo(string ustTabloAlan)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " , s1.KAYITAD as dilAdi, s2.KAYITAD as belgeAdi ";
            sql += " FROM " + TABLOAD;
            sql += " left outer join IKYSOZLUK S1 ON S1.KOD = " + ALANLAR[(int)ENUMAlanlar.DILKOD];
            sql += " left outer join IKYSOZLUK S2 ON S2.KOD = " + ALANLAR[(int)ENUMAlanlar.BELGETUR];
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PKOD] + "=" + ustTabloAlan;
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];
            return sql;
        }
    }
}
