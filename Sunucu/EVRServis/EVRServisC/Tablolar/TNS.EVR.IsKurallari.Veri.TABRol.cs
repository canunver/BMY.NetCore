using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABRol : TABOrtak
    {
        public TABRol(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVRROL";
        public static string TABLOADYETKI = "EVRROLYETKI";
        public static string TABLOADKULLANICI = "EVRKULLANICIROL";

        public enum ENUMAlanlar : int
        {
            KOD = 0, AD = 1
        }
        public enum ENUMAlanlarYetki : int
        {
            ROLKOD = 0, YETKIKOD = 1, DEGER = 2
        }
        public enum ENUMAlanlarKullanici : int
        {
            TCKIMLIKNO = 0, ROLKOD = 1, AD = 2, SOYAD = 3
        }

        public static string[] ALANLAR = { "KOD", "AD" };
        public static string[] ALANLARYETKI = { "ROLKOD", "YETKIKOD", "DEGER" };
        public static string[] ALANLARKULLANICI = { "TCKIMLIKNO", "ROLKOD", "AD", "SOYAD" };

        public Sonuc Kaydet(Rol form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            form.kod = this.KodAl();
            if (form.kod > 0)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ",";
                sql += VeriErisim.DegerYap(form.ad) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc Guncelle(Rol form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Kaydet(RolYetki form)
        {
            string sql = "INSERT INTO " + TABLOADYETKI + " (" + VeriErisim.TumAlanlar("", ALANLARYETKI) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.rol.kod) + ",";
            sql += VeriErisim.DegerYap(form.yetkiEnum.kod) + ",";
            sql += VeriErisim.DegerYap(form.yetkiDeger) + ")";

            return KomutCalistir(sql, 1);
        }
        public Sonuc Kaydet(KullaniciRol form)
        {
            string sql = "INSERT INTO " + TABLOADKULLANICI + " (" + VeriErisim.TumAlanlar("", ALANLARKULLANICI) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(form.rol.kod) + ",";
            sql += VeriErisim.DegerYap(form.ad) + ",";
            sql += VeriErisim.DegerYap(form.soyad) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(Rol form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            if (form.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader ListeleYetki(RolYetki form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARYETKI) + " FROM " + TABLOADYETKI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARYETKI[(int)ENUMAlanlarYetki.ROLKOD], "=", form.rol.kod));
            if (form.yetkiEnum.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARYETKI[(int)ENUMAlanlarYetki.YETKIKOD], "=", form.yetkiEnum.kod));

            sql += " ORDER BY " + ALANLARYETKI[(int)ENUMAlanlarYetki.YETKIKOD];

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader ListeleKullanici(KullaniciRol form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARKULLANICI) + " FROM " + TABLOADKULLANICI;
            if (form.rol.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKULLANICI[(int)ENUMAlanlarKullanici.ROLKOD], "=", form.rol.kod));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKULLANICI[(int)ENUMAlanlarKullanici.TCKIMLIKNO], "=", form.tcKimlikNo));

            sql += " ORDER BY " + ALANLARKULLANICI[(int)ENUMAlanlarKullanici.ROLKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public int KullaniciYetkiDegerBul(ENUMYetkiler yetki, string tcKimlikNo)
        {
            string sql = "SELECT TOP(1) " + TABLOADYETKI + "." + ALANLARYETKI[(int)ENUMAlanlarYetki.DEGER] + " FROM " + TABLOADYETKI;
            sql += " INNER JOIN EVRKULLANICIROL ON EVRROLYETKI.ROLKOD = EVRKULLANICIROL.ROLKOD ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADYETKI, ALANLARYETKI[(int)ENUMAlanlarYetki.YETKIKOD], "=", (int)yetki));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADKULLANICI, ALANLARKULLANICI[(int)ENUMAlanlarKullanici.TCKIMLIKNO], "=", tcKimlikNo));

            sql += " ORDER BY " + ALANLARYETKI[(int)ENUMAlanlarYetki.DEGER] + " DESC";

            return veriErisim.TekKolonSQLInt(sql);
        }


        public Sonuc Sil(Rol form)
        {
            Sonuc sonuc = this.SilYetki(form.kod);

            if (sonuc.islemSonuc)
                sonuc = this.SilKullanici(form.kod);

            if (sonuc.islemSonuc)
            {
                string sql = "DELETE FROM " + TABLOAD;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc SilYetki(int rolKod)
        {
            string sql = "DELETE FROM " + TABLOADYETKI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARYETKI[(int)ENUMAlanlarYetki.ROLKOD], "=", rolKod));

            return KomutCalistir(sql, 0);
        }
        public Sonuc SilKullanici(int rolKod)
        {
            string sql = "DELETE FROM " + TABLOADKULLANICI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKULLANICI[(int)ENUMAlanlarKullanici.ROLKOD], "=", rolKod));

            return KomutCalistir(sql, 0);
        }

        private int KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

    }
}
