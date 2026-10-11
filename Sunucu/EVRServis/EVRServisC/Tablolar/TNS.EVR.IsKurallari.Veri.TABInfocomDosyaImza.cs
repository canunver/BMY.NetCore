using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABInfocomDosyaImza : TABOrtak
    {
        public TABInfocomDosyaImza(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "INFOCOMDOSYAIMZA";

        public enum ENUMAlanlar : int
        {
            EVREVRAKNO = 0,
            SIRANO = 1,
            PERSONIDNP = 2,
            HASH = 3,
            SIGN = 4,
            ISLEMYAPAN = 5,
            ISLEMYAPANAD = 6,
            ISLEMTARIH = 7,
            VEKIL = 8,
            VEKILAD = 9,
        }

        public static string[] ALANLAR = { "EVREVRAKNO", "SIRANO", "PERSONIDNP", "HASH", "SIGN", "ISLEMYAPAN", "ISLEMYAPANAD", "ISLEMTARIH", "VEKIL", "VEKILAD" };

        public Sonuc Kaydet(InfocomDosyaImza form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.evrEvrakNo) + ",";
            sql += VeriErisim.DegerYap(form.siraNo) + ",";
            sql += VeriErisim.DegerYap(form.personIdnp) + ",";
            sql += VeriErisim.DegerYap(form.hash) + ",";
            sql += VeriErisim.DegerYap(form.sign) + ",";
            sql += VeriErisim.DegerYap(form.islemYapan) + ",";
            sql += VeriErisim.DegerYap(form.islemYapanAd) + ",";
            sql += VeriErisim.DegerYap(form.islemTarih) + ",";
            sql += VeriErisim.DegerYap(form.vekil) + ",";
            sql += VeriErisim.DegerYap(form.vekilAd) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(InfocomDosyaImza form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", form.evrEvrakNo));

            if (form.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));

            if (!string.IsNullOrWhiteSpace(form.personIdnp))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PERSONIDNP], "=", form.personIdnp));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(InfocomDosyaImza form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", form.evrEvrakNo));

            if (form.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));

            if (!string.IsNullOrWhiteSpace(form.personIdnp))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PERSONIDNP], "=", form.personIdnp));

            return KomutCalistir(sql, 0);
        }
    }
}
