using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.Veri;

namespace TNS.BUM.IsKurallari.Veri
{
    public class TABBUMNAKITPLANI : TABOrtak
    {
        public TABBUMNAKITPLANI(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "BUMNAKITPLANI";
        static string[] ALANLAR = { "TUR", "TARIH", "MUHASEBEKOD", "BIRIMKOD", "KURKOD", "HARCAMATUR", "MIKTAR" };

        public IDataReader ListeleGunlukNakitIhtiyac(int tur, TNSDateTime tarih1, TNSDateTime tarih2, int muhasebeKod, string birimKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", M.MUHASEBEAD, B.BIRIMAD FROM " + TABLOAD;
            sql += " INNER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM B ON B.MUHASEBEKOD = BUMNAKITPLANI.MUHASEBEKOD AND B.BIRIMKOD = BUMNAKITPLANI.BIRIMKOD AND B.KURKOD = BUMNAKITPLANI.KURKOD";

            sql += "  INNER JOIN ButceIslemleri.dbo.MUHMUHASEBE M ON M.MUHASEBEKOD = B.MUHASEBEKOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, "TUR", "=", tur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, "TARIH", ">=", tarih1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, "TARIH", "<=", tarih2));
            if (muhasebeKod > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, "MUHASEBEKOD", "=", muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, "BIRIMKOD", "=", birimKod));
            }
            sql += " ORDER BY 3,4,5,2 ";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc SilGunlukNakitIhtiyac(int tur, TNSDateTime tarih, int muhasebeKod, string birimKod)
        {
            string sql = "DELETE FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, "TUR", "=", tur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, "TARIH", "=", tarih));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, "MUHASEBEKOD", "=", muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, "BIRIMKOD", "=", birimKod));
            return KomutCalistir(sql, 0);
        }

        public Sonuc EkleGunlukNakitIhtiyac(int tur, TNSDateTime tarih, int muhasebeKod, string birimKod, string kurum, int harcamaTur, double miktar)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(tur) + ",";
            sql += VeriErisim.DegerYap(tarih) + ",";
            sql += VeriErisim.DegerYap(muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(birimKod) + ",";
            sql += VeriErisim.DegerYap(kurum.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(harcamaTur) + ",";
            sql += VeriErisim.DegerYap(miktar);
            sql += ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc EkleVeyaGuncelleGunlukNakitIhtiyac(int tur, TNSDateTime tarih, int muhasebeKod, string birimKod, string kurum, int harcamaTur, double miktar)
        {
            // Önce UPDATE dene
            string sqlUpdate = "UPDATE " + TABLOAD + " SET ";
            sqlUpdate += "MIKTAR = " + VeriErisim.DegerYap(miktar);

            sqlUpdate = VeriErisim.KosulEkle(sqlUpdate, "WHERE", VeriErisim.KosulOl(TABLOAD, "TUR", "=", tur));
            sqlUpdate = VeriErisim.KosulEkle(sqlUpdate, "AND", VeriErisim.KosulOl(TABLOAD, "TARIH", "=", tarih));
            sqlUpdate = VeriErisim.KosulEkle(sqlUpdate, "AND", VeriErisim.KosulOl(TABLOAD, "MUHASEBEKOD", "=", muhasebeKod));
            sqlUpdate = VeriErisim.KosulEkle(sqlUpdate, "AND", VeriErisim.KosulOl(TABLOAD, "BIRIMKOD", "=", birimKod));
            sqlUpdate = VeriErisim.KosulEkle(sqlUpdate, "AND", VeriErisim.KosulOl(TABLOAD, "HARCAMATUR", "=", harcamaTur));

            Sonuc sonuc = KomutCalistir(sqlUpdate, 1);

            // Eğer update hiçbir satır etkilemediyse → INSERT yap
            if (sonuc.kayitSay == 0)
            {
                return EkleGunlukNakitIhtiyac(tur, tarih, muhasebeKod, birimKod, kurum, harcamaTur, miktar);
            }

            return sonuc;
        }
    }
}
