using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABImzaYolu : TABOrtak
    {
        public TABImzaYolu(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVRIMZAYOLU";
        public static string TABLOADDETAY = "EVRIMZAYOLUDETAY";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            AD = 1,
            TCKIMLIKNO = 2
        }
        public enum ENUMAlanlarDetay : int
        {
            IMZAYOLUKOD = 0,
            SIRANO = 1,
            ISLEMTIP = 2,
            TCKIMLIKNO = 3,
            AD = 4,
            SOYAD = 5,
            EVRAKBIRIMKOD = 6,
            UNVAN = 7,
            AKADEMIKUNVAN = 8
        }

        public static string[] ALANLAR = { "KOD", "AD", "TCKIMLIKNO" };
        public static string[] ALANLARDETAY = { "IMZAYOLUKOD", "SIRANO", "ISLEMTIP", "TCKIMLIKNO", "AD", "SOYAD", "EVRAKBIRIMKOD", "UNVAN", "AKADEMIKUNVAN" };

        public Sonuc Kaydet(ImzaYolu form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            form.kod = this.KodAl();
            if (form.kod > 0)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ",";
                sql += VeriErisim.DegerYap(form.ad) + ",";
                sql += VeriErisim.DegerYap(form.tcKimlikNo) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc Guncelle(ImzaYolu form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }
        public Sonuc Kaydet(ImzaYoluDetay detay)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(detay.imzaYoluKod) + ",";
            sql += VeriErisim.DegerYap(detay.siraNo) + ",";
            sql += VeriErisim.DegerYap(detay.islemTip) + ",";
            sql += VeriErisim.DegerYap(detay.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(detay.ad) + ",";
            sql += VeriErisim.DegerYap(detay.soyad) + ",";
            sql += VeriErisim.DegerYap(detay.evrakBirimKod) + ",";
            sql += VeriErisim.DegerYap(detay.unvan) + ",";
            sql += VeriErisim.DegerYap(detay.akademikUnvan) + ")";


            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(ImzaYolu form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (form.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", form.tcKimlikNo));

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader ListeleDetay(int imzaYoluKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + ",EVRBIRIM.AD FROM " + TABLOADDETAY;
            sql += " INNER JOIN EVRBIRIM ON EVRIMZAYOLUDETAY.EVRAKBIRIMKOD = EVRBIRIM.KOD ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.IMZAYOLUKOD], "=", imzaYoluKod));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(ImzaYolu form)
        {
            Sonuc sonuc = this.SilDetay(form.kod);
            if (sonuc.islemSonuc)
            {
                string sql = "DELETE FROM " + TABLOAD;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc SilDetay(int imzaYoluKod)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.IMZAYOLUKOD], "=", imzaYoluKod));

            return KomutCalistir(sql, 0);
        }


        private int KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

    }
}
