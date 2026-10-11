using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABEYazismaBilgileri : TABOrtak
    {
        public TABEYazismaBilgileri(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVREYAZISMABILGI";

        public enum ENUMAlanlar : int
        {
            SERVISTUR = 0,
            SERVISIDARIKIMLIKKOD = 1,
            SERVISKURUMAD = 2,
            KULLANICIAD = 3,
            PAROLA = 4,
        }

        public static string[] ALANLAR = { "SERVISTUR", "SERVISIDARIKIMLIKKOD", "SERVISKURUMAD", "KULLANICIAD", "PAROLA" };

        public Sonuc Kaydet(EYazismaBilgi form)
        {
            Sonuc sonuc = Sil(form.servisTur);
            if (sonuc.islemSonuc)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.servisTur) + ",";
                sql += VeriErisim.DegerYap(form.servisIdariKimlikKod) + ",";
                sql += VeriErisim.DegerYap(form.servisKurumAd) + ",";
                sql += VeriErisim.DegerYap(form.kullaniciAd) + ",";
                sql += VeriErisim.DegerYap(form.parola) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        public IDataReader Oku(EYazismaBilgi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (!string.IsNullOrWhiteSpace(form.kullaniciAd) && !string.IsNullOrWhiteSpace(form.parola))
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIAD], "=", form.kullaniciAd));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PAROLA], "=", form.parola));
            }
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SERVISTUR], "=", form.servisTur));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader Listele()
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(int servisTur)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SERVISTUR], "=", servisTur));
            return KomutCalistir(sql, 0);
        }

    }
}
