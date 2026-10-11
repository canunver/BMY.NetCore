using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{
    public class TABDTVTBirim : TABOrtak
    {
        public TABDTVTBirim(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVRDTVT";

        public enum ENUMAlanlar : int
        {
            IDAREKIMLIKKODU = 0,
            PARENTIDAREKIMLIKKODU = 1,
            AD = 2,
            KURUMKODU = 3,
            TIPKOD = 4,
            ALTKURUMVARMI = 5,
            ANAKOD = 6,
            BIRIMKOD = 7,
            DAGILIMKOD1 = 8,
            DAGILIMKOD2 = 9,
            DAGILIMKOD3 = 10,
            DAGILIMKOD4 = 11,
            ALTBIRIMKOD1 = 12,
            ALTBIRIMKOD2 = 13,
            ALTBIRIMKOD3 = 14,
            ALTBIRIMKOD4 = 15,
            ADRES = 16,
            TELEFON = 17,
            FAKS = 18,
            WEBADRESI = 19,
            EPOSTA = 20,
            KEPADRES = 21,
            SONGUNCELLEMEZAMANI = 22,
            DISYAZISMADURUMU = 23,
            DURUM = 24,
            EYAZISMASERVISTUR = 25
        }

        public static string[] ALANLAR = { "IDAREKIMLIKKODU", "PARENTIDAREKIMLIKKODU", "AD", "KURUMKODU", "TIPKOD", "ALTKURUMVARMI", "ANAKOD", 
                                       "BIRIMKOD", "DAGILIMKOD1", "DAGILIMKOD2", "DAGILIMKOD3","DAGILIMKOD4",
                                       "ALTBIRIMKOD1","ALTBIRIMKOD2","ALTBIRIMKOD3","ALTBIRIMKOD4",
                                       "ADRES","TELEFON","FAKS","WEBADRESI","EPOSTA","KEPADRES","SONGUNCELLEMEZAMANI","DISYAZISMADURUMU","DURUM",
                                       "EYAZISMASERVISTUR"};

        public static string TABLOADBILGI = "EVRDTVTBILGI";

        public enum ENUMAlanlarBilgi : int
        {
            IDAREKIMLIKKODU = 0,
            KURUMAD = 1,
            SONGUNCELLEMEZAMANI = 2,
            SONGUNCELLEMESAYISI = 3
        }

        public static string[] ALANLARBILGI = { "IDAREKIMLIKKODU", "KURUMAD", "SONGUNCELLEMEZAMANI", "SONGUNCELLEMESAYISI" };

        public Sonuc Kaydet(DTVTBirim form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (!string.IsNullOrEmpty(form.idareKimlikKodu))
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.idareKimlikKodu) + ",";
                sql += VeriErisim.DegerYap(form.parentIdareKimlikKodu) + ",";
                sql += VeriErisim.DegerYap(form.ad) + ",";
                sql += VeriErisim.DegerYap(form.kurumKodu) + ",";
                sql += VeriErisim.DegerYap(form.tipkod) + ",";
                sql += VeriErisim.DegerYap(form.altKurumVarmi) + ",";
                sql += VeriErisim.DegerYap(form.anakod) + ",";
                sql += VeriErisim.DegerYap(form.birimkod) + ",";
                sql += VeriErisim.DegerYap(form.dagilimkod1) + ",";
                sql += VeriErisim.DegerYap(form.dagilimkod2) + ",";
                sql += VeriErisim.DegerYap(form.dagilimkod3) + ",";
                sql += VeriErisim.DegerYap(form.dagilimkod4) + ",";
                sql += VeriErisim.DegerYap(form.altbirimkod1) + ",";
                sql += VeriErisim.DegerYap(form.altbirimkod2) + ",";
                sql += VeriErisim.DegerYap(form.altbirimkod3) + ",";
                sql += VeriErisim.DegerYap(form.altbirimkod4) + ",";
                sql += VeriErisim.DegerYap(form.adres) + ",";
                sql += VeriErisim.DegerYap(form.telefon) + ",";
                sql += VeriErisim.DegerYap(form.faks) + ",";
                sql += VeriErisim.DegerYap(form.webAdresi) + ",";
                sql += VeriErisim.DegerYap(form.ePosta) + ",";
                sql += VeriErisim.DegerYap(form.kepAdres) + ",";
                sql += VeriErisim.DegerYap((form.sonGuncellemeZamani.Year > 1 ? new TNSDateTime(form.sonGuncellemeZamani) : new TNSDateTime())) + ",";
                sql += VeriErisim.DegerYap(form.disYazismaDurumu) + ",";
                sql += VeriErisim.DegerYap(form.durum) + ",";
                sql += VeriErisim.DegerYap(form.eYazismaServisTur) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";
                    sonuc = Guncelle(form);
                }

            }
            return sonuc;
        }

        public Sonuc Guncelle(DTVTBirim form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARENTIDAREKIMLIKKODU], form.parentIdareKimlikKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURUMKODU], form.kurumKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIPKOD], form.tipkod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTKURUMVARMI], form.altKurumVarmi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], form.anakod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], form.birimkod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAGILIMKOD1], form.dagilimkod1) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAGILIMKOD2], form.dagilimkod2) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAGILIMKOD3], form.dagilimkod3) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAGILIMKOD4], form.dagilimkod4) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTBIRIMKOD1], form.altbirimkod1) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTBIRIMKOD2], form.altbirimkod2) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTBIRIMKOD3], form.altbirimkod3) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTBIRIMKOD4], form.altbirimkod4) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADRES], form.adres) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TELEFON], form.telefon) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAKS], form.faks) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.WEBADRESI], form.webAdresi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EPOSTA], form.ePosta) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KEPADRES], form.kepAdres) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SONGUNCELLEMEZAMANI], (form.sonGuncellemeZamani.Year > 1 ? new TNSDateTime(form.sonGuncellemeZamani) : new TNSDateTime())) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DISYAZISMADURUMU], form.disYazismaDurumu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], form.durum);
            if (form.eYazismaServisTur > 0)
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EYAZISMASERVISTUR], form.eYazismaServisTur);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IDAREKIMLIKKODU], "=", form.idareKimlikKodu));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(string query)
        {
            string sql = "SELECT DISTINCT " + ALANLAR[(int)ENUMAlanlar.AD] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DISYAZISMADURUMU], "=", "Dış Yazışma Yapar"));

            foreach (var item in query.Split(' '))
                if (!string.IsNullOrEmpty(item))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + item.Trim() + "%"));

            sql += " ORDER BY AD";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader DisYazismaYapanKurumlariGetir(string idareKimlikKodu, TNSDateTime guncellemeZamani)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD + " AS A";
            sql += " WHERE ((KURUMKODU LIKE + '' + ";
            sql += " (SELECT KURUMKODU FROM EVRDTVT AS B ";
            sql += " WHERE (IDAREKIMLIKKODU = " + VeriErisim.DegerYap(idareKimlikKodu) + ")) + '%') OR (DISYAZISMADURUMU = 'Dış Yazışma Yapar'))";
            if (!guncellemeZamani.isNull && guncellemeZamani.Yil > 1)
                sql += "AND (SONGUNCELLEMEZAMANI > " + VeriErisim.DegerYap(guncellemeZamani) + ")";

            return veriErisim.DataReaderDondur(sql);
        }

        public int DisYazismaYapanKurumSayisi(string idareKimlikKodu, TNSDateTime guncellemeZamani)
        {
            string sql = "SELECT  COUNT(*) FROM " + TABLOAD + " AS A";
            sql += " WHERE ((KURUMKODU LIKE + '' + ";
            sql += " (SELECT KURUMKODU FROM EVRDTVT AS B ";
            sql += " WHERE (IDAREKIMLIKKODU = " + VeriErisim.DegerYap(idareKimlikKodu) + ")) + '%') OR (DISYAZISMADURUMU = 'Dış Yazışma Yapar'))";
            if (!guncellemeZamani.isNull && guncellemeZamani.Yil > 1)
                sql += "AND (SONGUNCELLEMEZAMANI > " + VeriErisim.DegerYap(guncellemeZamani) + ")";

            return veriErisim.TekKolonSQLInt(sql);
        }

        public IDataReader AltKurumlariGetir(string idareKimlikKodu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (string.IsNullOrEmpty(idareKimlikKodu))
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", VeriErisim.IslecBul("LEN") + "(" + ALANLAR[(int)ENUMAlanlar.KURUMKODU] + ")", "<=", 4));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PARENTIDAREKIMLIKKODU], "=", idareKimlikKodu));

            sql += " ORDER BY AD";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader OkuDTVTBirim(string idareKimlikKodu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IDAREKIMLIKKODU], "=", idareKimlikKodu));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(DTVTBirimBilgi form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (!string.IsNullOrEmpty(form.idareKimlikKodu))
            {
                string sql = "INSERT INTO " + TABLOADBILGI + " (" + VeriErisim.TumAlanlar("", ALANLARBILGI) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.idareKimlikKodu) + ",";
                sql += VeriErisim.DegerYap(form.kurumAd) + ",";
                sql += VeriErisim.DegerYap((form.sonGuncellemeZamani.Year > 1 ? new TNSDateTime(form.sonGuncellemeZamani) : new TNSDateTime())) + ",";
                sql += VeriErisim.DegerYap(form.sonGuncellemeSayisi) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";
                    sonuc = Guncelle(form);
                }

            }
            return sonuc;
        }

        public Sonuc Guncelle(DTVTBirimBilgi form)
        {
            string sql = "UPDATE " + TABLOADBILGI + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.KURUMAD], form.kurumAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.SONGUNCELLEMEZAMANI], (form.sonGuncellemeZamani.Year > 1 ? new TNSDateTime(form.sonGuncellemeZamani) : new TNSDateTime())) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.SONGUNCELLEMESAYISI], form.sonGuncellemeSayisi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.IDAREKIMLIKKODU], "=", form.idareKimlikKodu));

            return KomutCalistir(sql, 1);
        }

        public IDataReader GuncellemeBilgisiGetir(string idareKimlikKodu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARBILGI) + " FROM " + TABLOADBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.IDAREKIMLIKKODU], "=", idareKimlikKodu));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KepAdresGuncelle(DTVTBirim form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KEPADRES], form.kepAdres);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IDAREKIMLIKKODU], "=", form.idareKimlikKodu));

            return KomutCalistir(sql, 1);
        }

    }
}
