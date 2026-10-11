using System;
using TNS.Veri;
using System.Data;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABEvrakAdresDefteri : TABOrtak
    {
        public TABEvrakAdresDefteri(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADADRESDEFTER = "EVREVRAKADRESDEFTERI";

        enum ENUMAlanlarAdresDefter
        {
            KOD = 0,
            ADRES = 1,
            AD = 2,
            ISLEMYAPAN = 3,
            ISLEMYAPANBIRIMKOD = 4,
            ISLEMTARIH = 5,
        }

        static string[] ALANLARADRESDEFTER = { "KOD", "ADRES", "ADI", "ISLEMYAPAN", "ISLEMYAPANBIRIMKOD", "ISLEMTARIH" };

        private string sql = "";

        public Sonuc Kaydet(EvrakAdresDefteri form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            EvrakAdresGrubTablosu agt = new EvrakAdresGrubTablosu();

            if (form.kod == 0)
            {
                form.kod = AdresKodAl();

                sql = "INSERT INTO " + TABLOADADRESDEFTER + " (" + VeriErisim.TumAlanlar("", ALANLARADRESDEFTER) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(form.kod) + ", ";
                sql = sql + VeriErisim.DegerYap(form.adres) + ", ";
                sql = sql + VeriErisim.DegerYap(form.adi) + ", ";
                sql = sql + VeriErisim.DegerYap(form.islemYapan) + ", ";
                sql = sql + VeriErisim.DegerYap(form.islemYapanBirimKod) + ", ";
                sql = sql + VeriErisim.DegerYap(form.islemTarihi) + ")";
            }

            else
            {
                sql = "UPDATE " + TABLOADADRESDEFTER + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLARADRESDEFTER[(int)ENUMAlanlarAdresDefter.ADRES], form.adres) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARADRESDEFTER[(int)ENUMAlanlarAdresDefter.AD], form.adi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARADRESDEFTER[(int)ENUMAlanlarAdresDefter.ISLEMYAPAN], form.islemYapan) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARADRESDEFTER[(int)ENUMAlanlarAdresDefter.ISLEMYAPANBIRIMKOD], form.islemYapanBirimKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARADRESDEFTER[(int)ENUMAlanlarAdresDefter.ISLEMTARIH], form.islemTarihi) + " ";
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLARADRESDEFTER[(int)ENUMAlanlarAdresDefter.KOD], "=", form.kod);
            }

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc Sil(int kod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "DELETE FROM " + TABLOADADRESDEFTER + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLARADRESDEFTER[(int)ENUMAlanlarAdresDefter.KOD], "=", kod);

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
                return sonuc;
            //else
            //    SilAdresGrup(adresKod);
            return sonuc;
        }

        public IDataReader Listele(EvrakAdresDefteri form,int grupKod)
        {
            string sql = "SELECT ";

            sql += "EVREVRAKADRESDEFTERI.KOD,EVREVRAKADRESDEFTERI.ADRES,EVREVRAKADRESDEFTERI.ADI,EVREVRAKADRESDEFTERI.ISLEMYAPAN ,EVREVRAKADRESDEFTERI.ISLEMYAPANBIRIMKOD ,EVREVRAKADRESDEFTERI.ISLEMTARIH ";
            sql += " FROM EVREVRAKADRESDEFTERI ";

            if(grupKod!=0)
                sql += " INNER JOIN EVREVRAKADRESGRUPTABLOSU ON EVREVRAKADRESGRUPTABLOSU.ADRESKOD=EVREVRAKADRESDEFTERI.KOD ";

            string kosul = "";

            if (form.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKADRESDEFTERI", "KOD", "=", form.kod));

            if (!string.IsNullOrEmpty(form.islemYapanBirimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKADRESDEFTERI", "ISLEMYAPANBIRIMKOD", "=", form.islemYapanBirimKod));

            if (grupKod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKADRESGRUPTABLOSU", "GRUPKOD", "=", grupKod));

            if (kosul != "")
            {
                sql += " WHERE " + kosul;
            }

            sql += " ORDER BY ISLEMTARIH desc";

            return veriErisim.DataReaderDondur(sql);
        }

        private int AdresKodAl()
        {
            sql = "SELECT MAX(" + ALANLARADRESDEFTER[(int)ENUMAlanlarAdresDefter.KOD] + ") FROM " + TABLOADADRESDEFTER;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }
    }
}

