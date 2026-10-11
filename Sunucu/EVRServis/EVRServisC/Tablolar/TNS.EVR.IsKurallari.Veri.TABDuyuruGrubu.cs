using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{
    public class TABDuyuruGrubu : TABOrtak
    {
        public TABDuyuruGrubu(VeriErisim veriErisim, string tabloAd) : base(veriErisim) { TABLOAD = tabloAd; }

        private string TABLOAD = "EVRDUYURUGRUBU";
        private string TABLOADKISILER = "EVRDUYURUGRUBUKISILER";

        private enum ENUMAlanlar { DUYURUGRUBUID = 0, GRUPADI = 1, ISLEMYAPAN = 2, ISLEMTARIH = 3, ISLEMYAPANBIRIMKOD=4 }
        private enum ENUMAlanlarKisi { DUYURUGRUBUKISILERID = 0, DUYURUGRUBUID = 1, KISIKIMLIKNUMARASI = 2, KISIBIRIMKOD = 3, TIP=4 }


        private string[] ALANLAR = { "DUYURUGRUBUID", "GRUPADI", "ISLEMYAPAN", "ISLEMTARIH", "ISLEMYAPANBIRIMKOD" };
        private string[] ALANLARKISI = { "DUYURUGRUBUKISILERID", "DUYURUGRUBUID", "KISIKIMLIKNUMARASI", "KISIBIRIMKOD", "TIP" };

        private string sql = "";

        public Sonuc Kaydet(DuyuruGrubu form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (form.grupID == 0)
            {
                form.grupID = KodAl();

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(form.grupID) + ", ";
                sql = sql + VeriErisim.DegerYap(form.grupAdi) + ", ";
                sql = sql + VeriErisim.DegerYap(form.islemYapan) + ", ";
                sql = sql + VeriErisim.DegerYap(form.islemTarihi) + ",";
                sql = sql + VeriErisim.DegerYap(form.islemYapanBirimKod) + ")";
            }
            else
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GRUPADI], form.grupAdi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], form.islemYapan) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], form.islemTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANBIRIMKOD], form.islemYapanBirimKod) ;
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DUYURUGRUBUID], "=", form.grupID);
            }

            sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = form.grupID.ToString();
            return sonuc;
        }

        public Sonuc KaydetKisiler(VeriErisim veriErisim, ArrayList liste, string duyuruGrubuID)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            int duyuruID = OrtakFonksiyonlar.ConvertToInt(duyuruGrubuID, 0);
            sonuc = SilKisiler(duyuruID);

            foreach (DuyuruGrubuKisiler form in liste)
            {
                form.grupKisilerID = KodAlKisiler();

                sql = "INSERT INTO " + TABLOADKISILER + " (" + VeriErisim.TumAlanlar("", ALANLARKISI) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(form.grupKisilerID) + ", ";
                sql = sql + VeriErisim.DegerYap(duyuruGrubuID) + ",";
                sql = sql + VeriErisim.DegerYap(form.KisiKimlikNo) + ",";
                sql = sql + VeriErisim.DegerYap(form.KisiBirimKod) + ",";
                sql = sql + VeriErisim.DegerYap(form.tip) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc) { break; }
            }

            return sonuc;
        }

        public Sonuc Sil(DuyuruGrubu form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DUYURUGRUBUID], "=", form.grupID);

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
                return sonuc;
            else
                SilKisiler(form.grupID);
            return sonuc;
        }

        public Sonuc SilKisiler(int grupID)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            sql = "DELETE FROM " + TABLOADKISILER + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLARKISI[(int)ENUMAlanlarKisi.DUYURUGRUBUID], "=", grupID);

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
                return sonuc;

            return sonuc;
        }

        public IDataReader Listele(DuyuruGrubu bilgi)
        {
            string kriter = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            if (bilgi.grupID > 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DUYURUGRUBUID], "=", bilgi.grupID));

            if (bilgi.grupAdi != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GRUPADI], " LIKE ", "%" + bilgi.grupAdi + "%"));

            if (bilgi.islemYapanBirimKod!="")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPANBIRIMKOD], "=", bilgi.islemYapanBirimKod));

            if (kriter != "")
                sql += " WHERE " + kriter;

            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleKisiler(DuyuruGrubuKisiler bilgi)
        {
            string kriter = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADKISILER, ALANLARKISI) + " FROM " + TABLOADKISILER;
            if (bilgi.grupID > 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOADKISILER, ALANLARKISI[(int)ENUMAlanlarKisi.DUYURUGRUBUID], "=", bilgi.grupID));
            if (bilgi.tip > 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOADKISILER, ALANLARKISI[(int)ENUMAlanlarKisi.TIP], "=", bilgi.tip));

            if (kriter != "")
                sql += " WHERE " + kriter;

            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        private int KodAl()
        {
            sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.DUYURUGRUBUID] + ") FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

        private int KodAlKisiler()
        {
            sql = "SELECT MAX(" + ALANLARKISI[(int)ENUMAlanlarKisi.DUYURUGRUBUKISILERID] + ") FROM " + TABLOADKISILER;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }
    }
}
