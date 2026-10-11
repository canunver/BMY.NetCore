using System;
using TNS.Veri;
using System.Data;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABEvrakAdresGrubu : TABOrtak
    {
        public TABEvrakAdresGrubu(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADADRESGRUBU = "EVREVRAKADRESGRUBUADI";

        enum ENUMAlanlarAdresGrubu
        {
            KOD = 0,
            AD = 1,
            ISLEMYAPAN = 2,
            ISLEMYAPANBIRIMKOD = 3,
            ISLEMTARIH = 4,
        }

        static string[] ALANLARADRESGRUP = { "KOD", "ADI", "ISLEMYAPAN", "ISLEMYAPANBIRIMKOD", "ISLEMTARIH" };

        private string sql = "";

        public Sonuc Kaydet(EvrakAdresGrubuAdi form, ObjectArray adresListesi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (form.kod == 0)
            {
                form.kod = KodAlGrupNo();

                sql = "INSERT INTO " + TABLOADADRESGRUBU + " (" + VeriErisim.TumAlanlar("", ALANLARADRESGRUP) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(form.kod) + ", ";
                sql = sql + VeriErisim.DegerYap(form.adi) + ", ";
                sql = sql + VeriErisim.DegerYap(form.islemYapan) + ", ";
                sql = sql + VeriErisim.DegerYap(form.islemYapanBirimKod) + ", ";
                sql = sql + VeriErisim.DegerYap(form.islemTarihi) + ")";
            }

            else
            {
                sql = "UPDATE " + TABLOADADRESGRUBU + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLARADRESGRUP[(int)ENUMAlanlarAdresGrubu.KOD], form.kod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARADRESGRUP[(int)ENUMAlanlarAdresGrubu.AD], form.adi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARADRESGRUP[(int)ENUMAlanlarAdresGrubu.ISLEMYAPAN], form.islemYapan) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARADRESGRUP[(int)ENUMAlanlarAdresGrubu.ISLEMYAPANBIRIMKOD], form.islemYapanBirimKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARADRESGRUP[(int)ENUMAlanlarAdresGrubu.ISLEMTARIH], form.islemTarihi) + " ";
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLARADRESGRUP[(int)ENUMAlanlarAdresGrubu.KOD], "=", form.kod);
            }

            sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = form.kod.ToString();
            sonuc= KaydetAdresGrup(adresListesi, sonuc.anahtar);
            return sonuc;
        }

        public Sonuc Sil(int grupKod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "DELETE FROM " + TABLOADADRESGRUBU + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLARADRESGRUP[(int)ENUMAlanlarAdresGrubu.KOD], "=", grupKod);

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
                return sonuc;
            else
                SilAdresGrup(grupKod);
            return sonuc;
        }

        public IDataReader Listele(EvrakAdresGrubuAdi form)
        {
            string sql = "SELECT ";

            sql += "KOD ,ADI,ISLEMYAPAN ,ISLEMYAPANBIRIMKOD ,ISLEMTARIH ";
            sql += " FROM EVREVRAKADRESGRUBUADI ";

            string kosul = "";

            if (form.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKADRESDEFTERI", "KOD", "=", form.kod));

            if (kosul != "")
            {
                sql += " WHERE " + kosul;
            }

                return veriErisim.DataReaderDondur(sql);
        }

        private int KodAlGrupNo()
        {
            sql = "SELECT MAX(" + ALANLARADRESGRUP[(int)ENUMAlanlarAdresGrubu.KOD] + ") FROM " + TABLOADADRESGRUBU;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }


        //
        public static string TABLOAD = "EVREVRAKADRESGRUPTABLOSU";

        enum ENUMAlanlar
        {
            KOD = 0,
            ADRESKOD = 1,
            GRUPKOD = 2,
        }

        static string[] ALANLAR = { "KOD", "ADRESKOD", "GRUPKOD" };

        public Sonuc KaydetAdresGrup(ObjectArray adresListesi,string grupNo)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            int grupKod = OrtakFonksiyonlar.ConvertToInt(grupNo, 0);

            sonuc = SilAdresGrup(grupKod);

            foreach (EvrakAdresGrubTablosu form in adresListesi.objeler)
            {
                form.kod = KodAl();

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(form.kod) + ", ";
                sql = sql + VeriErisim.DegerYap(form.adresKod) + ",";
                sql = sql + VeriErisim.DegerYap(grupKod) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc) { break; }
            }

            return sonuc;
        }

        public Sonuc SilAdresGrup(int grupNo)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            sql = "DELETE FROM EVREVRAKADRESGRUPTABLOSU WHERE ";
            sql += "GRUPKOD= " + "'" + grupNo + "'";
            sonuc = KomutCalistir(sql, 1);

            if (sonuc.kayitSay == 0)
                sonuc.islemSonuc = true;

            if (!sonuc.islemSonuc)
                return sonuc;
            return sonuc;
        }

        private int KodAl()
        {
            sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

        public IDataReader ListeleGrupAdres(string[] gruplar)
        {
            string sql = "SELECT DISTINCT EVREVRAKADRESDEFTERI.KOD,EVREVRAKADRESDEFTERI.ADRES FROM EVREVRAKADRESDEFTERI ";
            sql += " INNER JOIN EVREVRAKADRESGRUPTABLOSU ON EVREVRAKADRESGRUPTABLOSU.ADRESKOD=EVREVRAKADRESDEFTERI.KOD ";
            sql += " WHERE EVREVRAKADRESGRUPTABLOSU.GRUPKOD in (";
            foreach (string  grupNo in gruplar)
                sql += VeriErisim.DegerYap(grupNo) + ",";
            sql = sql.TrimEnd(',');
            sql += ")";
            sql += " ORDER BY EVREVRAKADRESDEFTERI.KOD";

            return veriErisim.DataReaderDondur(sql);         
        }

    }
}
