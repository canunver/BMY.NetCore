using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABCezaOdul : TABOrtak
    {
        public TABCezaOdul(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADCEZAODUL = "IKYPERSONELCEZAODUL";
        public enum ENUMALANLARCEZAODUL : int
        {
            KOD = 0,
            PKOD = 1,
            VerenKurum = 2,
            VerenAdi = 3,
            VerenUnvani = 4,
            AlanUnvani = 5,
            Cinsi = 6,
            Sebebi = 7,
            BelgeCinsi = 8,
            BelgeNo = 9,
            BelgeTarihi = 10,
            CezaOdulTipi = 11,
            Tarih = 12,
            Dayanak = 13
        }
        static string[] ALANLARCEZAODUL = { "KOD", "PKOD", "VerenKurum", "VerenAdi", "VerenUnvani", "AlanUnvani", "Cinsi", "Sebebi", "BelgeCinsi", "BelgeNo", "BelgeTarihi", "CezaOdulTipi", "Tarih", "Dayanak" };

        public IDataReader ListeleCezaOdul(PersonelCezaOdul CezaOdul)
        {
            string sql = "", kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADCEZAODUL, ALANLARCEZAODUL) + " , IKYSOZLUK.KAYITAD, IKYSOZLUK.KAYITKOD" + " FROM " + TABLOADCEZAODUL;
            sql += " LEFT OUTER JOIN IKYSOZLUK ON IKYSOZLUK.KOD = " + ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.CezaOdulTipi];

            if (CezaOdul.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADCEZAODUL, ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.KOD], "=", CezaOdul.kod));

            if (CezaOdul.personelKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADCEZAODUL, ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.PKOD], "=", CezaOdul.personelKod));

            if (!string.IsNullOrWhiteSpace(CezaOdul.cezaOdulTipiKayitKodu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABSozluk.TABLOAD, TABSozluk.ALANLAR[(int)TABSozluk.ENUMAlanlar.KAYITKOD], "LIKE", CezaOdul.cezaOdulTipiKayitKodu + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.Tarih] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetCezaOdul(PersonelCezaOdul p)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "UPDATE " + TABLOADCEZAODUL + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.VerenKurum], p.verenKurum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.VerenAdi], p.verenAdi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.VerenUnvani], p.verenUnvani) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.AlanUnvani], p.alanUnvani) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.Cinsi], p.cinsi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.Sebebi], p.sebebi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.BelgeCinsi], p.belgeCinsi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.BelgeNo], p.belgeNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.BelgeTarihi], p.belgeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.CezaOdulTipi], p.cezaOdulTipi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.Tarih], p.tarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.Dayanak], p.dayanak);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.KOD], "=", p.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                sql = "INSERT INTO " + TABLOADCEZAODUL + " (" + VeriErisim.TumAlanlar("", ALANLARCEZAODUL) + ") VALUES (";
                sql += VeriErisim.DegerYap(p.kod) + ",";
                sql += VeriErisim.DegerYap(p.personelKod) + ",";
                sql += VeriErisim.DegerYap(p.verenKurum) + ",";
                sql += VeriErisim.DegerYap(p.verenAdi) + ",";
                sql += VeriErisim.DegerYap(p.verenUnvani) + ",";
                sql += VeriErisim.DegerYap(p.alanUnvani) + ",";
                sql += VeriErisim.DegerYap(p.cinsi) + ",";
                sql += VeriErisim.DegerYap(p.sebebi) + ",";
                sql += VeriErisim.DegerYap(p.belgeCinsi) + ",";
                sql += VeriErisim.DegerYap(p.belgeNo) + ",";
                sql += VeriErisim.DegerYap(p.belgeTarihi) + ",";
                sql += VeriErisim.DegerYap(p.cezaOdulTipi) + ",";
                sql += VeriErisim.DegerYap(p.tarih) + ",";
                sql += VeriErisim.DegerYap(p.dayanak) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc SilCezaOdul(PersonelCezaOdul p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADCEZAODUL;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADCEZAODUL, ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.KOD], "=", p.kod));

            return KomutCalistir(sql, 0);
        }

        internal static string ListeleUstTablo(int cezalarOlsun, string ustTabloAlan)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADCEZAODUL, ALANLARCEZAODUL) + " , IKYSOZLUK.KAYITAD, IKYSOZLUK.KAYITKOD" + " FROM " + TABLOADCEZAODUL;
            sql += " LEFT OUTER JOIN IKYSOZLUK ON IKYSOZLUK.KOD = " + ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.CezaOdulTipi];

            sql += " WHERE " + TABLOADCEZAODUL + "." + ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.PKOD] + "=" + ustTabloAlan;

            string cezaKod;
            if (cezalarOlsun == 1)
                cezaKod = "30.00001";
            else if (cezalarOlsun == 2)
                cezaKod = "30.00002";
            else
                cezaKod = "";
            if (cezaKod != "")
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABSozluk.TABLOAD, TABSozluk.ALANLAR[(int)TABSozluk.ENUMAlanlar.KAYITKOD], "LIKE", cezaKod + "%"));
            }

            sql += " ORDER BY " + ALANLARCEZAODUL[(int)ENUMALANLARCEZAODUL.Tarih] + " DESC";

            return sql;
        }
    }
}
