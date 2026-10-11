using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABPersonelSozlesme : TABOrtak
    {
        public TABPersonelSozlesme(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "IKYPERSONELSOZLESME";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            YIL = 1,
            TCKIMLIKNO = 2,
            ADSOYAD = 3,
            BASLANGICTARIHI = 4,
            BITISTARIHI = 5,
            TUTAR = 6,
            SOZLESMEKOD = 7

        }
        static string[] ALANLAR = { "KOD", "YIL", "TCKIMLIKNO", "ADSOYAD", "BASLANGICTARIHI", "BITISTARIHI", "TUTAR", "SOZLESMEKOD" };

        public IDataReader Listele(PersonelSozlesme p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            if (!string.IsNullOrWhiteSpace(p.tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tcKimlikNo));

            if (p.sozlesmeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", p.sozlesmeKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(PersonelSozlesme p)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.yil) + ",";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.adSoyad) + ",";
            sql += VeriErisim.DegerYap(p.basTarih) + ",";
            sql += VeriErisim.DegerYap(p.bitTarih) + ",";
            sql += VeriErisim.DegerYap(p.tutar) + ",";
            sql += VeriErisim.DegerYap(p.sozlesmeKod) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(PersonelSozlesme p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], p.tcKimlikNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADSOYAD], p.adSoyad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI], p.basTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISTARIHI], p.bitTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUTAR], p.tutar);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(PersonelSozlesme p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetSozlesmeKod(PersonelSozlesme p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], p.sozlesmeKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

    }
}
