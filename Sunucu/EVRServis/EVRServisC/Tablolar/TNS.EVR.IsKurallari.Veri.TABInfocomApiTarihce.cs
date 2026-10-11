using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABInfocomApiTarihce : TABOrtak
    {
        public TABInfocomApiTarihce(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "INFOCOMAPITARIHCE";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            ISLEMYAPAN = 1,
            ISLEMYAPANAD = 2,
            ISLEMTARIH = 3,
            PERSONIDNP = 4,
            ORGANIZATIONINN = 5,
            SERVIS = 6,
            TALEP = 7,
            CEVAP = 8,
        }

        public static string[] ALANLAR = { "KOD", "ISLEMYAPAN", "ISLEMYAPANAD", "ISLEMTARIH", "PERSONIDNP", "ORGANIZATIONINN", "SERVIS", "TALEP", "CEVAP" };

        public Sonuc Kaydet(InfocomApiTarihce form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kod) + ",";
            sql += VeriErisim.DegerYap(form.islemYapan) + ",";
            sql += VeriErisim.DegerYap(form.islemYapanAd) + ",";
            sql += VeriErisim.DegerYap(form.islemTarih) + ",";
            sql += VeriErisim.DegerYap(form.personIdnp) + ",";
            sql += VeriErisim.DegerYap(form.organizationInn) + ",";
            sql += VeriErisim.DegerYap(form.servis) + ",";
            sql += VeriErisim.DegerYap(form.talep) + ",";
            sql += VeriErisim.DegerYap(form.cevap) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc CevapKaydet(InfocomApiTarihce form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CEVAP], form.cevap);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(InfocomApiTarihce form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (!string.IsNullOrWhiteSpace(form.kod))
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PERSONIDNP], "=", form.personIdnp));

            return veriErisim.DataReaderDondur(sql);
        }

    }
}
