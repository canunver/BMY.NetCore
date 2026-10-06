using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.ORG.IsKurallari.Veri
{
    /// <summary>
    /// Görev veri tabanı tablosu ile ilgili olarak listeleme, kaydetme ve silme işlemini yapan sınıf
    /// </summary>
    public class TABGorev : TABOrtak
    {
        /// <summary>
        ///  Sınıfın oluşturucu(Constructer) fonksiyonu
        /// </summary>
        /// <param name="veriErisim">Veri Erişim tipinde sınıf</param>
        public TABGorev(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adı
        /// </summary>
        static string TABLOAD = "PERGorev";

        /// <summary>
        /// Tablo alanları indisi
        /// </summary>
        enum ENUMAlanlar : int
        {
            /// <summary>
            /// Görev Kodu
            /// </summary>
            KOD = 0,
            /// <summary>
            /// Muhasebe Kodu
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// Birim Kodu
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// Amaç
            /// </summary>
            AMAC = 3,
            /// <summary>
            /// Tür
            /// </summary>
            TUR = 4,
            /// <summary>
            /// Durum
            /// </summary>
            DURUM = 5,
            /// <summary>
            /// İşlem Kodu
            /// </summary>
            ISLEMKOD = 6,
            /// <summary>
            /// Evrak Numarası
            /// </summary>
            EVREVRAKNO = 7,
            /// <summary>
            /// Program Evrak Numarası
            /// </summary>
            PREVRAKNO = 8,
        }

        /// <summary>
        /// Tablo alanları
        /// </summary>
        static string[] ALANLAR = { "KOD", "MUHASEBEKOD", "BIRIMKOD", "AMAC", "TUR", "DURUM", "ISLEMKOD", "EVREVRAKNO", "PREVRAKNO" };

        /// <summary>
        /// Harcama modülü Muhasebe View tablosunun tanımlanması
        /// </summary>
        public static string TABLOADMUHASEBEVIEW = "HRCMUHASEBEVIEW";

        /// <summary>
        /// Tablo alanları indis tanımlamaları
        /// </summary>
        public enum ENUMAlanlarMUHASEBEVIEW
        {
            /// <summary>
            /// Muhasebe kodu
            /// </summary>
            MUHASEBEKOD = 0,
            /// <summary>
            /// Muhasebe adı
            /// </summary>
            MUHASEBEAD = 1,
        }

        /// <summary>
        /// Harcama modülü Muhasebe View tablosunun alanlarının tanımlanması
        /// </summary>
        public static string[] ALANLARMUHASEBEVIEW = { "MUHASEBEKOD", "MUHASEBEAD" };

        /// <summary>
        /// Sorgulama işlemi yapan fonksiyon
        /// </summary>
        /// <param name="p">Görev tipinde sınıf</param>
        /// <returns>IDataReader tipinde sınıf döndürür</returns>
        public IDataReader Sorgula(Gorev p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT    PERGOREV.KOD, PERGOREV.MUHASEBEKOD, PERGOREV.BIRIMKOD, PERGOREV.AMAC, PERGOREV.TUR, PERGOREV.DURUM, PERGOREV.ISLEMKOD,PERGOREV.EVREVRAKNO,PERGOREV.PREVRAKNO, " +
                      " PERMUHASEBEVIEW.MUHASEBEAD, PERMUHASEBEBIRIMVIEW.BIRIMAD " +
                    " FROM         PERGOREV INNER JOIN " +
                      " PERMUHASEBEVIEW ON PERGOREV.MUHASEBEKOD = PERMUHASEBEVIEW.MUHASEBEKOD INNER JOIN " +
                      " PERMUHASEBEBIRIMVIEW ON PERGOREV.MUHASEBEKOD = PERMUHASEBEBIRIMVIEW.MUHASEBEKOD AND " +
                      " PERGOREV.BIRIMKOD = PERMUHASEBEBIRIMVIEW.BIRIMKOD";

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            if (p.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", p.muhasebeKod));

            if (p.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", p.birimKod));

            if (p.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", p.tur));

            if (p.islemKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMKOD], "=", p.islemKod));

            if (p.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", p.durum));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Listeleme işlemi yapan fonksiyon
        /// </summary>
        /// <param name="p">Görev tipinde sınıf</param>
        /// <returns>IDataReader tipinde sınıf döndürür</returns>
        public IDataReader Listele(Gorev p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            if (p.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", p.muhasebeKod));

            if (p.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", p.birimKod));

            if (p.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", p.tur));

            if (p.islemKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMKOD], "=", p.islemKod));

            if (p.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", p.durum));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Kaydetme işlemi yapan fonksiyon
        /// </summary>
        /// <param name="p">Görev tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc Kaydet(Gorev p)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(p.birimKod) + ",";
            sql += VeriErisim.DegerYap(p.amac) + ",";
            sql += VeriErisim.DegerYap(p.tur) + ",";
            sql += VeriErisim.DegerYap(p.durum) + ",";
            sql += VeriErisim.DegerYap(p.islemKod) + ",";
            sql += VeriErisim.DegerYap(p.evrEvrakNo) + ",";
            sql += VeriErisim.DegerYap(p.prEvrakNo) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMAC], p.amac) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], p.tur) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMKOD], p.islemKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], p.durum) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], p.evrEvrakNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], p.prEvrakNo);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Evrak Bilgisi kaydetme işlemi yapan fonksiyon
        /// </summary>
        /// <param name="kod">Evrak kodu</param>
        /// <param name="evrEvrakNo">Evrak numarası</param>
        /// <param name="prEvrakNo">Program evrak numarası</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc KaydetEvrakBilgi(int kod, string evrEvrakNo, string prEvrakNo)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], evrEvrakNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], prEvrakNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        /// <summary>
        /// Silme işlemi yapan fonksiyon
        /// </summary>
        /// <param name="p">Görev tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc Sil(Gorev p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.birimKod));

            return KomutCalistir(sql, 0);
        }
    }
}
