using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Security.Cryptography;

namespace TNS.KPM.IsKurallari.Veri
{
    public class TABIstek : TABOrtak
    {
        public TABIstek(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "KPMISTEK";
        enum ENUMAlanlar : int
        {
            ISTEKNO = 0,
            TCKIMLIKNO = 1,
            ISTEKTUR = 2,
            ADSOYAD = 3,
            ISLEMYAPAN = 4,
            ISLEMTARIH = 5
        }
        static string[] ALANLAR = { "ISTEKNO", "TCKIMLIKNO", "ISTEKTUR", "ADSOYAD", "ISLEMYAPAN", "ISLEMTARIH" };

        static string TABLOADKIMLIK = "KPMISTEKKIMLIK";
        enum ENUMAlanlarKimlik : int
        {
            TCKIMLIKNO = 0,
            VERI = 1,
            ISLEMTARIH = 2
        }
        static string[] ALANLARKIMLIK = { "TCKIMLIKNO", "VERI", "ISLEMTARIH" };

        public Sonuc Kaydet(Istek istek)
        {
            istek.istekNo = IstekNoAl(istek.tcKimlikNo);

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(istek.istekNo) + ",";
            sql += VeriErisim.DegerYap(istek.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(istek.istekTur) + ",";
            sql += VeriErisim.DegerYap(istek.adSoyad) + ",";
            sql += VeriErisim.DegerYap(istek.islemYapan) + ",";
            sql += VeriErisim.DegerYap(new TNSDateTime(System.DateTime.Now)) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = istek.istekNo.ToString();

            return sonuc;
        }

        public IDataReader Say(Istek istek)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            string kosul = VeriErisim.KosulEkle("", "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], ">=", istek.islemTarih));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", istek.islemYapan));
            sql += " WHERE " + kosul;
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader Listele(Istek istek)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (istek.istekNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISTEKNO], "=", istek.istekNo));
            if (istek.istekTur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISTEKTUR], "=", istek.istekTur));
            if (!string.IsNullOrEmpty(istek.tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", istek.tcKimlikNo));
            if (!istek.islemTarih.isNull)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], ">=", istek.islemTarih));
                if (istek.islemTarih2.isNull)
                {
                    istek.islemTarih.AddDays(1);
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], "<", istek.islemTarih));
                }
            }
            if (!istek.islemTarih2.isNull)
            {
                istek.islemTarih2.AddDays(1);
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], "<=", istek.islemTarih2));
            }
            if (istek.islemYapan != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", istek.islemYapan));
            else if (istek.sorguIslemYapan != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", istek.sorguIslemYapan));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY ISLEMTARIH DESC,TCKIMLIKNO";

            return veriErisim.DataReaderDondur(sql);
        }

        private int IstekNoAl(string tcKimlikNo)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] };
            string[] strDeger = { tcKimlikNo };
            string[] intAlan = { };
            int[] intDeger = { };

            string istekNo = TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.ISTEKNO], strAlan, strDeger, intAlan, intDeger).ToString();

            return OrtakFonksiyonlar.ConvertToInt(istekNo, 0);
        }

        public Sonuc KaydetKimlik(string tcKimlikNo, string veri)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "INSERT INTO " + TABLOADKIMLIK + " (" + VeriErisim.TumAlanlar("", ALANLARKIMLIK) + ") VALUES (";
            sql += VeriErisim.DegerYap(tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(veri) + ",";
            sql += VeriErisim.DegerYap(new TNSDateTime(System.DateTime.Now)) + ")";
            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOADKIMLIK + " SET ";
                sql += VeriErisim.AtamaOl(TABLOADKIMLIK, ALANLARKIMLIK[(int)ENUMAlanlarKimlik.VERI], veri) + ",";
                sql += VeriErisim.AtamaOl(TABLOADKIMLIK, ALANLARKIMLIK[(int)ENUMAlanlarKimlik.ISLEMTARIH], new TNSDateTime(System.DateTime.Now));
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADKIMLIK, ALANLARKIMLIK[(int)ENUMAlanlarKimlik.TCKIMLIKNO], "=", tcKimlikNo));
                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public IDataReader ListeleKimlik(string tcKimlikNo)
        {
            TNSDateTime tarih = new TNSDateTime(DateTime.Now);
            tarih.AddDays(-5);

            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADKIMLIK, ALANLARKIMLIK);
            sql += " FROM " + TABLOADKIMLIK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADKIMLIK, ALANLARKIMLIK[(int)ENUMAlanlarKimlik.TCKIMLIKNO], "=", tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADKIMLIK, ALANLARKIMLIK[(int)ENUMAlanlarKimlik.ISLEMTARIH], ">", tarih));

            return veriErisim.DataReaderDondur(sql);
        }

        //public static string YetkiDegerOrla(string[] yetkiDeger)
        //{
        //    string donusDeger = string.Empty;
        //    bool orlandiMi = false;
        //    foreach (string s in yetkiDeger)
        //    {
        //        if (!string.IsNullOrEmpty(s))
        //        {
        //            string[] parcaliYetki = s.Split('-');
        //            if (parcaliYetki.Length == 2)
        //            {
        //                string ekleme = string.Empty;
        //                if (parcaliYetki[0] != "*")
        //                    ekleme += TABLOAD + "." + "MUHASEBEKOD" + "=" + parcaliYetki[0];

        //                if (parcaliYetki[1] != "*")
        //                {
        //                    if (!string.IsNullOrEmpty(ekleme))
        //                        ekleme += " AND ";

        //                    ekleme += TABLOAD + "." + "BIRIMKOD" + "=" + parcaliYetki[1];
        //                }

        //                if (!string.IsNullOrEmpty(donusDeger) && !string.IsNullOrEmpty(ekleme))
        //                {
        //                    orlandiMi = true;
        //                    donusDeger += " OR ";
        //                }

        //                if (!string.IsNullOrEmpty(ekleme))
        //                    donusDeger += "(" + ekleme + ")";
        //            }
        //        }
        //    }

        //    if (orlandiMi)
        //        donusDeger = "(" + donusDeger + ")";

        //    return donusDeger;
        //}
    }
}