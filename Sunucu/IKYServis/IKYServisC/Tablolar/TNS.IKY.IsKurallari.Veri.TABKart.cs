using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABKart : TABOrtak
    {
        public TABKart(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADKART = "IKYPERSONELKART";
        public enum ENUMAlanlarKart : int
        {
            KOD = 0,
            PKOD = 1,
            ZimmetTarihi = 2,
            IadeTarihi = 3,
            kartTipi = 4,
            kartNo = 5,
            Yakinligi = 6,
            Plaka = 7,
            AdiSoyadi = 8,
            TagCihazNo = 9,
            ZimmetEdenPersonel = 10,
            IadeAlanPersonel = 11,
            Aciklama = 12,
            AktifMi = 13,
            KayipTarihi = 14
        }
        static string[] ALANLARKART = { "KOD", "PKOD", "ZimmetTarihi", "IadeTarihi", "kartTipi", "kartNo", "Yakinligi", "Plaka", "AdiSoyadi", "TagCihazNo", "ZimmetEdenPersonel", "IadeAlanPersonel", "Aciklama", "AktifMi", "KayipTarihi" };

        public IDataReader ListeleKart(PersonelKart kart)
        {
            string sql = "", kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADKART, ALANLARKART) + " , IKYSOZLUK.KAYITAD, IKYPERSONEL.AD + ' ' + IKYPERSONEL.SOYAD,  " +
                " IKYPERSONEL2.AD + ' ' + IKYPERSONEL2.SOYAD FROM " + TABLOADKART;
            sql += " LEFT OUTER JOIN IKYSOZLUK ON IKYSOZLUK.KOD = " + ALANLARKART[(int)ENUMAlanlarKart.kartTipi];
            sql += " LEFT OUTER JOIN IKYPERSONEL ON IKYPERSONEL.KOD = " + TABLOADKART +"."+ ALANLARKART[(int)ENUMAlanlarKart.ZimmetEdenPersonel];
            sql += " LEFT OUTER JOIN IKYPERSONEL as IKYPERSONEL2  ON IKYPERSONEL2.KOD = " + TABLOADKART + "." + ALANLARKART[(int)ENUMAlanlarKart.IadeAlanPersonel];

            if (kart.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKART, ALANLARKART[(int)ENUMAlanlarKart.KOD], "=", kart.kod));

            if (kart.personelKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKART, ALANLARKART[(int)ENUMAlanlarKart.PKOD], "=", kart.personelKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARKART[(int)ENUMAlanlarKart.ZimmetTarihi] + " desc";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetKart(PersonelKart p)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "UPDATE " + TABLOADKART + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARKART[(int)ENUMAlanlarKart.ZimmetTarihi], p.zimmetTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKART[(int)ENUMAlanlarKart.IadeTarihi], p.iadeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKART[(int)ENUMAlanlarKart.kartTipi], p.kartTipi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKART[(int)ENUMAlanlarKart.kartNo], p.kartNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKART[(int)ENUMAlanlarKart.Yakinligi], (int)p.yakinligi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKART[(int)ENUMAlanlarKart.Plaka], p.plaka) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKART[(int)ENUMAlanlarKart.AdiSoyadi], p.adiSoyadi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKART[(int)ENUMAlanlarKart.TagCihazNo], p.tagCihazNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKART[(int)ENUMAlanlarKart.ZimmetEdenPersonel], p.zimmetEdenPersonel) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKART[(int)ENUMAlanlarKart.IadeAlanPersonel], p.iadeAlanPersonel) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKART[(int)ENUMAlanlarKart.Aciklama], p.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKART[(int)ENUMAlanlarKart.AktifMi], p.aktifMi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKART[(int)ENUMAlanlarKart.KayipTarihi], p.kayipTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKART[(int)ENUMAlanlarKart.KOD], "=", p.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                sql = "INSERT INTO " + TABLOADKART + " (" + VeriErisim.TumAlanlar("", ALANLARKART) + ") VALUES (";
                sql += VeriErisim.DegerYap(p.kod) + ",";
                sql += VeriErisim.DegerYap(p.personelKod) + ",";
                sql += VeriErisim.DegerYap(p.zimmetTarihi) + ",";
                sql += VeriErisim.DegerYap(p.iadeTarihi) + ",";
                sql += VeriErisim.DegerYap(p.kartTipi) + ",";
                sql += VeriErisim.DegerYap(p.kartNo) + ",";
                sql += VeriErisim.DegerYap((int)p.yakinligi) + ",";
                sql += VeriErisim.DegerYap(p.plaka) + ",";
                sql += VeriErisim.DegerYap(p.adiSoyadi) + ",";
                sql += VeriErisim.DegerYap(p.tagCihazNo) + ",";
                sql += VeriErisim.DegerYap(p.zimmetEdenPersonel) + ",";
                sql += VeriErisim.DegerYap(p.iadeAlanPersonel) + ",";
                sql += VeriErisim.DegerYap(p.aciklama) + ",";
                sql += VeriErisim.DegerYap(p.aktifMi) + ",";
                sql += VeriErisim.DegerYap(p.kayipTarihi) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc SilKart(PersonelKart p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADKART;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADKART, ALANLARKART[(int)ENUMAlanlarKart.KOD], "=", p.kod));

            return KomutCalistir(sql, 0);
        }
    }
}
