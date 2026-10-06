using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;
using System.Collections;

namespace TNS.KYM.IsKurallari.Veri
{
    public class TABMevzuat : TABOrtak
    {
        public TABMevzuat(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "MEVZUAT";
        public enum ENUMAlanlar : int { KOD, ANAKOD, KLONKOD, SIRA, AD, ACIKLAMA, BALANGICTARIH, BITISTARIH, TURKOD, GUNCELLEME, SIL, BILGIKOD, BILGINO }
        public static string[] ALANLAR = { "KOD", "ANAKOD", "KLONKOD", "SIRA", "AD", "ACIKLAMA", "BALANGICTARIH", "BITISTARIH", "TURKOD", "GUNCELLEME", "SIL", "BILGIKOD", "BILGINO" };

        public static string TABLOADDURUM = "MEVZUATDURUM";
        public enum ENUMAlanlarDURUM : int { KOD, KILIT, ISLEMTARIH }
        public static string[] ALANLARDURUM = { "KOD", "KILIT", "ISLEMTARIH" };

        public static string TABLOADSAYFA = "MEVZUATSAYFA";
        public enum ENUMAlanlarSAYFA : int { SAYFAKOD, MEVZUATKOD, SIRA }
        public static string[] ALANLARSAYFA = { "SAYFAKOD", "MEVZUATKOD", "SIRA" };

        public IDataReader Listele(Mevzuat mevzuat, TNSDateTime islemTarih, int sil)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            string kosul = "";

            if (!string.IsNullOrEmpty(mevzuat.anaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], "=", mevzuat.anaKod));
            if (!string.IsNullOrEmpty(mevzuat.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", mevzuat.kod));
            if (!string.IsNullOrEmpty(mevzuat.klonKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KLONKOD], "=", mevzuat.klonKod));

            if (!islemTarih.isNull)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BALANGICTARIH], "<", new TNSDateTime(islemTarih.Oku().AddDays(1).ToShortDateString())));
                string ekKosul = "";
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BITISTARIH], ">", new TNSDateTime(islemTarih.Oku().AddDays(1).ToShortDateString())));
                ekKosul += " OR " + ALANLAR[(int)ENUMAlanlar.BITISTARIH] + " IS NULL";
                kosul += " AND (" + ekKosul + ")";
            }

            if (kosul == "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], "=", ""));

            if (sil == 0 || sil == 1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIL], "=", sil));

            sql += " WHERE " + kosul;
            sql += " ORDER BY 2,9,4";
            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc Kaydet(Mevzuat[] mevzuat)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            foreach (Mevzuat form in mevzuat)
            {
                if (form.guncelleme == 0)
                {
                    sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                    sql += VeriErisim.DegerYap(form.kod) + ", ";
                    sql += VeriErisim.DegerYap(form.anaKod) + ", ";
                    sql += VeriErisim.DegerYap(form.klonKod) + ", ";
                    sql += VeriErisim.DegerYap(form.sira) + ", ";
                    sql += VeriErisim.DegerYap(form.ad) + ", ";
                    sql += VeriErisim.DegerYap(form.aciklama) + ", ";
                    sql += VeriErisim.DegerYap(form.baslangicTarih) + ", ";
                    sql += VeriErisim.DegerYap(form.bitisTarih) + ", ";
                    sql += VeriErisim.DegerYap(form.turKod) + ", ";
                    sql += VeriErisim.DegerYap(1) + ", ";
                    sql += VeriErisim.DegerYap(form.sil) + ", ";
                    sql += VeriErisim.DegerYap(form.bilgiKod) + ", ";
                    sql += VeriErisim.DegerYap(form.bilgiNo) + ")";
                }
                else
                {
                    sql = "UPDATE " + TABLOAD + " SET ";
                    if (!form.bitisTarih.isNull)
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISTARIH], form.bitisTarih);
                    else
                    {
                        if (form.bilgiKod != "")
                            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BILGIKOD], form.bilgiKod) + " ,";
                        if (form.bilgiNo > 0)
                            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BILGINO], form.bilgiNo) + " ,";
                        if (form.anaKod != "")
                            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], form.anaKod) + " ,";
                        if (form.klonKod != "")
                            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KLONKOD], form.klonKod) + " ,";
                        if (form.sira > 0)
                            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRA], form.sira) + " ,";
                        //if (form.ad != "")
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad) + " ,";
                        //if (form.aciklama != "")
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], form.aciklama);
                        //else
                        //    sql = sql.TrimEnd(',');
                    }
                    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
                }

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc) { break; }
            }
            return sonuc;
        }
        public Sonuc Sil(ArrayList mevzuatKodList)
        {
            string kosul = "";
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIL], 1);
            foreach (string kod in mevzuatKodList)
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;
            return KomutCalistir(sql, 0);
        }
        public Sonuc Kilitle(ArrayList mevzuatKodList)
        {
            string kosul = "";
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GUNCELLEME], 0);
            foreach (string kod in mevzuatKodList)
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;
            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleMevzuatSonDurum(string mevzuatKod)
        {
            string sql = "SELECT TOP(1)" + VeriErisim.TumAlanlar("", ALANLARDURUM) + " FROM " + TABLOADDURUM;
            string kosul = "";

            if (!string.IsNullOrEmpty(mevzuatKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", mevzuatKod));

            sql += " WHERE " + kosul;
            sql += " ORDER BY 3 DESC";
            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc KaydetMevzuatDurum(MevzuatDurum mDurum)
        {
            string sql = "INSERT INTO " + TABLOADDURUM + " (" + VeriErisim.TumAlanlar("", ALANLARDURUM) + ") VALUES (";
            sql += VeriErisim.DegerYap(mDurum.kod) + ", ";
            sql += VeriErisim.DegerYap(mDurum.kilit) + ", ";
            sql += VeriErisim.DegerYap(mDurum.islemTarih) + ")";

            return KomutCalistir(sql, 1);
        }


        public IDataReader ListeleMevzuatSayfa(string msKod)
        {
            string sql = "SELECT MS.SAYFAKOD, MS.SIRA AS MSSIRA, M.KOD, M.ANAKOD, M.KLONKOD, M.SIRA AS MSIRA, M.AD, M.ACIKLAMA, M.BALANGICTARIH, M.BITISTARIH, M.TURKOD, M.GUNCELLEME, M.SIL, M.BILGIKOD, M.BILGINO ";
            sql += "FROM MEVZUATSAYFA AS MS INNER JOIN MEVZUAT AS M ON MS.MEVZUATKOD = M.KOD ";
            sql += string.Format("WHERE (MS.SAYFAKOD = {0}) ", VeriErisim.DegerYap(msKod));
            sql += "ORDER BY 2";
            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc KaydetMevzuatSayfa(MevzuatSayfa ms)
        {
            string sql = "INSERT INTO " + TABLOADSAYFA + " (" + VeriErisim.TumAlanlar("", ALANLARSAYFA) + ") VALUES (";
            sql += VeriErisim.DegerYap(ms.sayfaKod) + ", ";
            sql += VeriErisim.DegerYap(ms.mevzuatKod) + ", ";
            sql += VeriErisim.DegerYap(ms.sira) + ")";
            return KomutCalistir(sql, 1);
        }
        public Sonuc SilMevzuatSayfa(MevzuatSayfa ms)
        {
            string sql = "DELETE FROM " + TABLOADSAYFA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSAYFA[(int)ENUMAlanlarSAYFA.SAYFAKOD], "=", ms.sayfaKod));
            if (!string.IsNullOrEmpty(ms.mevzuatKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSAYFA[(int)ENUMAlanlarSAYFA.MEVZUATKOD], "=", ms.mevzuatKod));

            return KomutCalistir(sql, 0);
        }
    }
}