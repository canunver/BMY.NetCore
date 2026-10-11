using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.HRC.IsKurallari.Veri
{
    public class TABTeminatMektubu : TABOrtak
    {
        public TABTeminatMektubu(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "HRCTEMINATMEKTUBU";
        public static string[] ALANLAR ={
                                    "YIL",
                                    "DOSYANO",
                                    "MUHASEBE",
                                    "BIRIM",
                                    "KAYITTARIHI",
                                    "FIRMAKOD",
                                    "DOSYATARIHI",
                                    "NUMARASI",
                                    "BANKAKOD",
                                    "ALINISSEBEBI",
                                    "VADETARIHI",
                                    "NEVI",
                                    "TUTAR",
                                    "MUHISLEMGIRISBELGEYIL",
                                    "MUHISLEMGIRISBELGENO",
                                    "MUHISLEMGIRISBELGETARIHI",
                                    "CIKISTARIHI",
                                    "IADENEDEN",
                                    "MUHISLEMCIKISBELGEYIL",
                                    "MUHISLEMCIKISBELGENO",
                                    "MUHISLEMCIKISBELGETARIH",
                                    "BANKASUBEKOD",
                                    "SURESIZ",
                                    "SOZLESMEKOD"
                                       };
        public enum ENUMAlanlar
        {
            YIL = 0,
            DOSYANO = 1,
            MUHASEBE = 2,
            BIRIM = 3,
            KAYITTARIHI = 4,
            FIRMAKOD = 5,
            DOSYATARIHI = 6,
            NUMARASI = 7,
            BANKAKOD = 8,
            ALINISSEBEBI = 9,
            VADETARIHI = 10,
            NEVI = 11,
            TUTAR = 12,
            MUHISLEMGIRISBELGEYIL = 13,
            MUHISLEMGIRISBELGENO = 14,
            MUHISLEMGIRISBELGETARIHI = 15,
            CIKISTARIHI = 16,
            IADENEDEN = 17,
            MUHISLEMCIKISBELGEYIL = 18,
            MUHISLEMCIKISBELGENO = 19,
            MUHISLEMCIKISBELGETARIH = 20,
            BANKASUBEKOD = 21,
            SURESIZ = 22,
            SOZLESMEKOD = 23
        }

        //

        public static string TABLOADCIKIS = "HRCTEMINATMEKTUBUCIKIS";
        public static string[] ALANLARCIKIS ={
                                    "KOD",
                                    "YIL",
                                    "DOSYANO",
                                    "CIKISIADETUTAR",
                                    "CIKISTARIHI",
                                    "IADENEDEN",
                                    "MUHISLEMCIKISBELGEYIL",
                                    "MUHISLEMCIKISBELGENO",
                                    "MUHISLEMCIKISBELGETARIH",
                                    "DURUM"
                                       };
        public enum ENUMAlanlarCikis
        {
            KOD = 0,
            YIL = 1,
            DOSYANO = 2,
            CIKISIADETUTAR = 3,
            CIKISTARIHI = 4,
            IADENEDEN = 5,
            MUHISLEMCIKISBELGEYIL = 6,
            MUHISLEMCIKISBELGENO = 7,
            MUHISLEMCIKISBELGETARIH = 8,
            DURUM = 9
        }

        public Sonuc Kaydet(TeminatMektubu mektup)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(mektup.yil) + ",";
            sql += VeriErisim.DegerYap(mektup.dosyaNo) + ",";
            sql += VeriErisim.DegerYap(mektup.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(mektup.birimKod) + ",";
            sql += VeriErisim.DegerYap(mektup.kayitTarihi) + ",";
            sql += VeriErisim.DegerYap(mektup.giris.firma.kod) + ",";
            sql += VeriErisim.DegerYap(mektup.giris.dosyaTarih) + ",";
            sql += VeriErisim.DegerYap(mektup.giris.numara) + ",";
            sql += VeriErisim.DegerYap(mektup.giris.banka.kod) + ",";
            sql += VeriErisim.DegerYap(mektup.giris.alinisSebebi) + ",";
            sql += VeriErisim.DegerYap(mektup.giris.vadeTarih) + ",";
            sql += VeriErisim.DegerYap(mektup.giris.nevi) + ",";
            sql += VeriErisim.DegerYap(mektup.giris.tutar) + ",";
            sql += VeriErisim.DegerYap(mektup.giris.muhasebeIslem.yil) + ",";
            sql += VeriErisim.DegerYap(mektup.giris.muhasebeIslem.belgeNo) + ",";
            sql += VeriErisim.DegerYap(mektup.giris.muhasebeIslem.belgeTarihi) + ",";
            sql += VeriErisim.DegerYap(mektup.cikis.tarih) + ",";
            sql += VeriErisim.DegerYap(mektup.cikis.iadeNeden) + ",";
            sql += VeriErisim.DegerYap(mektup.cikis.muhasebeIslem.yil) + ",";
            sql += VeriErisim.DegerYap(mektup.cikis.muhasebeIslem.belgeNo) + ",";
            sql += VeriErisim.DegerYap(mektup.cikis.muhasebeIslem.belgeTarihi) + ",";
            sql += VeriErisim.DegerYap(mektup.giris.bankaSube.kod) + ",";
            sql += VeriErisim.DegerYap(mektup.suresiz) + ",";
            sql += VeriErisim.DegerYap(mektup.sozlesmeKod) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetCikis(TeminatMektubuCikisBilgileri mektupCikis)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOADCIKIS + " (" + VeriErisim.TumAlanlar("", ALANLARCIKIS) + ") VALUES (";
            sql += VeriErisim.DegerYap(mektupCikis.kod) + ",";
            sql += VeriErisim.DegerYap(mektupCikis.yil) + ",";
            sql += VeriErisim.DegerYap(mektupCikis.dosyaNo) + ",";
            sql += VeriErisim.DegerYap(mektupCikis.tutar) + ",";
            sql += VeriErisim.DegerYap(mektupCikis.cikisTarih) + ",";
            sql += VeriErisim.DegerYap(mektupCikis.iadeNeden) + ",";
            sql += VeriErisim.DegerYap(mektupCikis.muhasebeIslem.yil) + ",";
            sql += VeriErisim.DegerYap(mektupCikis.muhasebeIslem.belgeNo) + ",";
            sql += VeriErisim.DegerYap(mektupCikis.muhasebeIslem.belgeTarihi) + ",";
            sql += VeriErisim.DegerYap(mektupCikis.durum) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(TeminatMektubu mektup)
        {
            string sql = "";
            sql = "UPDATE  " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBE], mektup.muhasebeKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], mektup.birimKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKOD], mektup.giris.firma.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOSYATARIHI], mektup.giris.dosyaTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NUMARASI], mektup.giris.numara) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BANKAKOD], mektup.giris.banka.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALINISSEBEBI], mektup.giris.alinisSebebi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VADETARIHI], mektup.giris.vadeTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NEVI], mektup.giris.nevi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUTAR], mektup.giris.tutar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BANKASUBEKOD], mektup.giris.bankaSube.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SURESIZ], mektup.suresiz) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], mektup.sozlesmeKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", mektup.dosyaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", mektup.yil));
            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleGirisOdemeEmri(int dosyaYil, string dosyaNo, int belgeYil, string belgeNo, TNSDateTime belgeTarihi)
        {
            string sql = "";
            sql = "UPDATE  " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHISLEMGIRISBELGEYIL], belgeYil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHISLEMGIRISBELGENO], belgeNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHISLEMGIRISBELGETARIHI], belgeTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", dosyaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", dosyaYil));
            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleCikisOdemeEmri(int cikisKod, int belgeYil, string belgeNo, TNSDateTime belgeTarihi)
        {
            string sql = "";
            sql = "UPDATE " + TABLOADCIKIS + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARCIKIS[(int)ENUMAlanlarCikis.MUHISLEMCIKISBELGEYIL], belgeYil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCIKIS[(int)ENUMAlanlarCikis.MUHISLEMCIKISBELGENO], belgeNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCIKIS[(int)ENUMAlanlarCikis.MUHISLEMCIKISBELGETARIH], belgeTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARCIKIS[(int)ENUMAlanlarCikis.KOD], "=", cikisKod));
            return KomutCalistir(sql, 1);
        }


        public Sonuc Sil(TeminatMektubu mektup)
        {
            string sql = "";
            sql = " DELETE FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", mektup.dosyaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", mektup.yil));
            return KomutCalistir(sql, 0);
        }

        public Sonuc SilCikis(int yil, string dosyaNo)
        {
            string sql = "";
            sql = " DELETE FROM " + TABLOADCIKIS;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARCIKIS[(int)ENUMAlanlarCikis.DOSYANO], "=", dosyaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARCIKIS[(int)ENUMAlanlarCikis.YIL], "=", yil));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Listeles the specified mektup.
        /// </summary>
        /// <param name="mektup">The mektup.</param>
        /// <returns></returns>
        public IDataReader Listele(TeminatMektubu mektup, bool uyarilacaklar)
        {
            string selectParca = " SELECT " + VeriErisim.TumAlanlar("A", ALANLAR) + ",";
            selectParca += TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.AD] + " FIRMAD,";
            selectParca += TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.VERGINO] + " VERGINO,";
            selectParca += " HRCPERBANKAVIEW.AD BANKAAD , HRCPERBANKASUBEVIEW.AD BANKASUBEAD, HRCTEMINATMEKTUBUNEVI.ADI ";
            selectParca += " ,(SELECT SUM(CIKISIADETUTAR) FROM HRCTEMINATMEKTUBUCIKIS AS C WHERE (A.YIL = YIL) AND (A.DOSYANO = DOSYANO))";
            selectParca += " ,(SELECT MAX(CIKISTARIHI) FROM HRCTEMINATMEKTUBUCIKIS AS C WHERE (A.YIL = YIL) AND (A.DOSYANO = DOSYANO))";
            selectParca += " ,(SELECT MAX(CIKISTARIHI) FROM HRCTEMINATMEKTUBUCIKIS AS C WHERE (A.YIL = YIL) AND (A.DOSYANO = DOSYANO))";
            selectParca += " FROM " + TABLOAD + " A LEFT JOIN " + TABFirmaBilgisi.TABLOAD + " ON ";
            selectParca += "A." + ALANLAR[(int)ENUMAlanlar.FIRMAKOD] + "=" + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.KOD];
            selectParca += " LEFT JOIN HRCPERBANKAVIEW ON " + "A." + ALANLAR[(int)ENUMAlanlar.BANKAKOD] + "=" + "HRCPERBANKAVIEW.KOD";
            selectParca += " LEFT JOIN HRCPERBANKASUBEVIEW ON " + "A." + ALANLAR[(int)ENUMAlanlar.BANKASUBEKOD] + "=" + "HRCPERBANKASUBEVIEW.KOD AND ";
            selectParca += "A." + ALANLAR[(int)ENUMAlanlar.BANKAKOD] + "=" + "HRCPERBANKASUBEVIEW.BANKAKODU";
            selectParca += " LEFT JOIN HRCTEMINATMEKTUBUNEVI ON " + "A." + ALANLAR[(int)ENUMAlanlar.NEVI] + "=" + "HRCTEMINATMEKTUBUNEVI.KOD";

            string sql = "";
            if (mektup.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", mektup.yil));

            if (!string.IsNullOrEmpty(mektup.dosyaNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", mektup.dosyaNo));

            if (mektup.muhasebeKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBE], "=", mektup.muhasebeKod));

            if (mektup.birimKod != "")
            {
                string icKosul = "";
                string[] birimler = mektup.birimKod.Split(';');

                foreach (string birim in birimler)
                {
                    if (birim.Trim() != "")
                        icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", birim));
                }
                if (icKosul != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", "(" + icKosul + ")");
            }

            if (!string.IsNullOrEmpty(mektup.iadeNeden))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IADENEDEN], " LIKE %", mektup.iadeNeden + "%"));

            if (mektup.giris.nevi > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.NEVI], "=", mektup.giris.nevi));

            if (mektup.giris.vadeTarih != null && mektup.giris.vadeTarih.Yil > 1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.VADETARIHI], ">=", mektup.giris.vadeTarih));

            if (mektup.giris.vadeTarih2 != null && mektup.giris.vadeTarih2.Yil > 1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.VADETARIHI], "<=", mektup.giris.vadeTarih2));

            if (mektup.giris.firma.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKOD], "=", mektup.giris.firma.kod));
            else if (!string.IsNullOrWhiteSpace(mektup.giris.firma.ad))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABFirmaBilgisi.TABLOAD, TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.AD], "LIKE", mektup.giris.firma.ad + "%"));
            else if (!string.IsNullOrWhiteSpace(mektup.giris.firma.vergiNo))
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABFirmaBilgisi.TABLOAD, TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.VERGINO], "=", mektup.giris.firma.vergiNo));
                sql = sql.Replace("LEFT JOIN " + TABFirmaBilgisi.TABLOAD, "INNER JOIN " + TABFirmaBilgisi.TABLOAD);
            }

            string cikisIslec = "";
            if (mektup.cikis.durum == (int)ENUMCikisDurum.CIKISYAPILAN)
                cikisIslec = ">=";
            else if (mektup.cikis.durum == (int)ENUMCikisDurum.CIKISYAPILMAYAN)
                cikisIslec = "<";
            if (cikisIslec != "")
                sql = VeriErisim.KosulEkle(sql, "AND", "ISNULL((SELECT SUM(CIKISIADETUTAR) FROM HRCTEMINATMEKTUBUCIKIS AS C WHERE (A.YIL = YIL) AND (A.DOSYANO = DOSYANO)), 0) " + cikisIslec + " A.TUTAR");

            if (uyarilacaklar)
                sql = VeriErisim.KosulEkle(sql, "AND", "A.EPOSTAUYARIGONDTARIH IS NULL");

            sql = VeriErisim.KosulEkle(selectParca, "WHERE", sql);
            if (uyarilacaklar)
                sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.VADETARIHI];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleCikis(TeminatMektubuCikisBilgileri mektupCikis)
        {
            string selectParca = " SELECT " + VeriErisim.TumAlanlar(TABLOADCIKIS, ALANLARCIKIS);
            selectParca += " FROM " + TABLOADCIKIS;
            string sql = "";
            if (mektupCikis.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARCIKIS[(int)ENUMAlanlarCikis.YIL], "=", mektupCikis.yil));

            if (!string.IsNullOrEmpty(mektupCikis.dosyaNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARCIKIS[(int)ENUMAlanlarCikis.DOSYANO], "=", mektupCikis.dosyaNo));

            sql = VeriErisim.KosulEkle(selectParca, "WHERE", sql);

            return veriErisim.DataReaderDondur(sql);
        }

        public int getMaxDosyaNo(TeminatMektubu mektup)
        {
            string sql = "";
            sql = " SELECT ISNULL( MAX(" + ALANLAR[(int)ENUMAlanlar.DOSYANO] + "),0)+1 FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql);
        }

        public int getCikisMaxKod(TeminatMektubuCikisBilgileri cikis)
        {
            string sql = "";
            sql = " SELECT MAX(" + ALANLARCIKIS[(int)ENUMAlanlarCikis.KOD] + ")" + "FROM " + TABLOADCIKIS;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

        public Sonuc UyariGondermeTarihiYaz(TeminatMektubu mektup, TNSDateTime tar)
        {
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET EPOSTAUYARIGONDTARIH = " + VeriErisim.DegerYap(tar);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", mektup.dosyaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", mektup.yil));
            return KomutCalistir(sql, 0);
        }
    }
}
