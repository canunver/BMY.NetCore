using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABKurs : TABOrtak
    {
        public TABKurs(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "IKYPERSONELKURS";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            PKOD = 1,
            KURSKODU = 2,
            TARIH = 3,
            ALDIGIYER = 4,
            BASLAMATARIHI = 5,
            BITIRMETARIHI = 6,
            PUAN = 7,
            DERECE = 8,
            KURSADI = 9,
            DERSSAATI = 10,
            EGITIMYERI = 11,
            YIL = 12,
            DONEM = 13,
            GUN = 14,
            DOSYANO = 15,
            ACIKLAMA = 16
        }
        static string[] ALANLAR = { "KOD", "PKOD", "KURSKODU", "TARIH", "ALDIGIYER", "BASLAMATARIHI", "BITIRMETARIHI", "PUAN", "DERECE", "KURSADI", "DERSSAATI", "EGITIMYERI", "YIL", "DONEM", "GUN", "DOSYANO", "ACIKLAMA" };

        public IDataReader Listele(PersonelKurs p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", IKYSOZLUK.HITAPID, SOZLUK2.HITAPID "; ;
            sql += " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN " + TABSozluk.TABLOAD + " ON " + ALANLAR[(int)ENUMAlanlar.KURSKODU] + "=" + TABSozluk.TABLOAD + "." + TABSozluk.ALANLAR[(int)TABSozluk.ENUMAlanlar.KOD];
            sql += " LEFT OUTER JOIN  IKYSOZLUK AS SOZLUK2 ON EGITIMYERI = SOZLUK2.KOD";

            if (p.personelKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.personelKod));

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            //if (!string.IsNullOrEmpty(p.aciklama))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + p.aciklama + "%"));

            //if (!string.IsNullOrEmpty(p.aldigiYer))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALDIGIYER], "LIKE", "%" + p.aldigiYer + "%"));

            //if (!string.IsNullOrEmpty(p.derece))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DERECE], "=", p.derece));

            //if (p.dersSaati > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DERSSAATI], "=", p.dersSaati));

            //if (!string.IsNullOrEmpty(p.donem))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEM], "=", p.donem));

            //if (!string.IsNullOrEmpty(p.dosyaNo))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", p.dosyaNo));

            //if (!string.IsNullOrEmpty(p.egitimYeri))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EGITIMYERI], "LIKE", "%" + p.egitimYeri + "%"));

            //if (p.gun > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GUN], "=", p.gun));

            //if (!string.IsNullOrEmpty(p.kursAdi))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURSADI], "LIKE", "%" + p.kursAdi + "%"));

            //if (p.kursKod > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURSKODU], "=", p.kursKod));

            //if (p.kod > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", p.siraNo));

            //if (!string.IsNullOrEmpty(p.tcKimlikNo))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tcKimlikNo));

            //if (p.yil > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", p.yil));


            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.BITIRMETARIHI] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(PersonelKurs p)
        {
            string sql = "";
            //p.personelKod = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + "=" + p.tcKimlikNo);

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.personelKod) + ",";
            if (p.kursKod>0)
                sql += VeriErisim.DegerYap(p.kursKod) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.tarih) + ",";
            sql += VeriErisim.DegerYap(p.aldigiYer) + ",";
            sql += VeriErisim.DegerYap(p.baslamaTarihi) + ",";
            sql += VeriErisim.DegerYap(p.bitirmeTarihi) + ",";
            sql += VeriErisim.DegerYap(p.puan) + ",";
            sql += VeriErisim.DegerYap(p.derece) + ",";
            sql += VeriErisim.DegerYap(p.kursAdi) + ",";
            sql += VeriErisim.DegerYap(p.dersSaati) + ",";
            if (p.egitimYeri > 0)
                sql += VeriErisim.DegerYap(p.egitimYeri) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.yil) + ",";
            sql += VeriErisim.DegerYap(p.donem) + ",";
            sql += VeriErisim.DegerYap(p.gun) + ",";
            sql += VeriErisim.DegerYap(p.dosyaNo) + ",";
            sql += VeriErisim.DegerYap(p.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(PersonelKurs p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            if (p.kursKod > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURSKODU], p.kursKod) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURSKODU], (string)null) + ",";     
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH], p.tarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALDIGIYER], p.aldigiYer) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLAMATARIHI], p.baslamaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITIRMETARIHI], p.bitirmeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PUAN], p.puan) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DERECE], p.derece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURSADI], p.kursAdi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DERSSAATI], p.dersSaati) + ",";
            if (p.egitimYeri > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EGITIMYERI], p.egitimYeri) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EGITIMYERI], (string)null) + ",";  
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], p.yil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DONEM], p.donem) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GUN], p.gun) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], p.dosyaNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], p.aciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.personelKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(PersonelKurs p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.personelKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 0);
        }
    }
}
