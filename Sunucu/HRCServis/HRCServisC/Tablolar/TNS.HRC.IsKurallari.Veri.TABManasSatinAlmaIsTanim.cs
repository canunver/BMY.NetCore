using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    public class TABManasSatinAlmaIsTanim : TABOrtak
    {
        public TABManasSatinAlmaIsTanim(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "MANASSATINALMAISTANIM";
        public enum ENUMAlanlar { KOD = 0, TARIH = 1, AD = 2, NITELIK = 3, IHALEUSULU = 4, IHALEMADDESI = 5, ALIMTURU = 6, PARABIRIMI = 7, ALIMYONTEMI = 8, ONAYTARIHI = 9 }
        public static string[] ALANLAR = { "KOD", "TARIH", "AD", "NITELIK", "IHALEUSULU", "IHALEMADDESI", "ALIMTURU", "PARABIRIMI", "ALIMYONTEMI", "ONAYTARIHI" };

        public Sonuc Kaydet(ManasSatinAlmaIsTanim form)
        {

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kod) + ",";
            sql += VeriErisim.DegerYap(form.tarih) + ",";
            sql += VeriErisim.DegerYap(form.ad) + ",";
            sql += VeriErisim.DegerYap(form.nitelik) + ",";
            sql += VeriErisim.DegerYap(form.ihaleUsulu) + ",";
            sql += VeriErisim.DegerYap(form.ihaleMaddesi) + ",";
            sql += VeriErisim.DegerYap(form.alimTuru) + ",";
            sql += VeriErisim.DegerYap(form.paraBirimi.kod) + ",";
            sql += VeriErisim.DegerYap(form.alimYontemi) + ",";
            sql += VeriErisim.DegerYap(form.onayTarihi) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(ManasSatinAlmaIsTanim form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH], form.tarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NITELIK], form.nitelik) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALEUSULU], form.ihaleUsulu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALEMADDESI], form.ihaleMaddesi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALIMTURU], form.alimTuru) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARABIRIMI], form.paraBirimi.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALIMYONTEMI], form.alimYontemi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleOnay(ManasSatinAlmaIsTanim form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYTARIHI], form.onayTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(ManasSatinAlmaIsTanim form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", HRCPARABIRIMI.ADI, HRCPARABIRIMI.KISALTMA";
            sql += " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN HRCPARABIRIMI ON MANASSATINALMAISTANIM.PARABIRIMI = HRCPARABIRIMI.KOD";

            if (form.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
            {
                string ekKosul = "";

                if (form.ad != "")
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", form.ad));
                if (form.nitelik != "")
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.NITELIK], "LIKE", form.nitelik));
                if (form.ihaleUsulu > 0)
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IHALEUSULU], "=", form.ihaleUsulu));
                if (form.alimTuru > 0)
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALIMTURU], "=", form.alimTuru));
                if (form.paraBirimi.kod > 0)
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PARABIRIMI], "=", form.paraBirimi.kod));
                if (form.alimYontemi > 0)
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALIMYONTEMI], "=", form.alimYontemi));

                if (ekKosul != "")
                    sql += " WHERE " + ekKosul;
            }

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(ManasSatinAlmaIsTanim form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }
    }
}