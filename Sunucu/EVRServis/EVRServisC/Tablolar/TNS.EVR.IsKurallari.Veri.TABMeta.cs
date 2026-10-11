using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABMeta : TABOrtak
    {
        public TABMeta(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVRMETA";
        public enum ENUMAlanlar : int { KOD, AD, TIP, ZORUNLU, ISLEMYAPAN, EVRAKBIRIMKOD, DETAY, KLASOR, GRUPKOD }
        public static string[] ALANLAR = { "KOD", "AD", "TIP", "ZORUNLU", "ISLEMYAPAN", "EVRAKBIRIMKOD", "DETAY", "KLASOR", "GRUPKOD" };

        public Sonuc Kaydet(Meta form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            form.kod = this.KodAl();
            if (form.kod > 0)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ",";
                sql += VeriErisim.DegerYap(form.ad) + ",";
                sql += VeriErisim.DegerYap(form.tip) + ",";
                sql += VeriErisim.DegerYap(form.zorunlu) + ",";
                sql += VeriErisim.DegerYap(form.islemYapan) + ",";
                sql += VeriErisim.DegerYap(form.evrakBirimKod) + ",";
                sql += VeriErisim.DegerYap(form.detay) + ",";
                sql += VeriErisim.DegerYap(form.klasor) + ",";
                sql += VeriErisim.DegerYap(form.grupKod) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc Guncelle(Meta form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIP], form.tip) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ZORUNLU], form.zorunlu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DETAY], form.detay) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KLASOR], form.klasor) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GRUPKOD], form.grupKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(Meta form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (form.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKOD], "=", form.evrakBirimKod));

            if (form.grupKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GRUPKOD], "=", form.grupKod));
            
            if (form.klasor > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KLASOR], "=", form.klasor));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(Meta form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            return KomutCalistir(sql, 1);
        }

        private int KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }


        public int ArsivKayitliMetaVarMi(int metaKod)
        {
            string sql = "SELECT COUNT(*) FROM EVRARSIVMETA WHERE (METAKOD = " + VeriErisim.DegerYap(metaKod) + ")";
            return veriErisim.TekKolonSQLInt(sql);
        }

        public int EvrakKayitliMetaVarMi(int metaKod)
        {
            string sql = "SELECT COUNT(*) FROM EVREVRAKMETA WHERE (METAKOD = " + VeriErisim.DegerYap(metaKod) + ")";
            return veriErisim.TekKolonSQLInt(sql);
        }
    }
}
