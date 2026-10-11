using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Parametrik Tüketim bilgilerini tutan PARTUKETIM veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABPARTUKETIM : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABPARTUKETIM(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PARTUKETIM";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// Aşama alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ASAMA = 1,
            
            /// <summary>
            /// Kurumsal Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 2,
            
            /// <summary>
            /// Fonksiyonel Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FONKOD = 3,
            
            /// <summary>
            /// Finansal Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FINKOD = 4,
            
            /// <summary>
            /// Ekonomik Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 5,
            
            /// <summary>
            /// Alt Faaliyet alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 6,
            
            /// <summary>
            /// Kurumsal Yapı Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURUMSALYAPIKOD = 7,
            
            /// <summary>
            /// Tüketim Tip alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUKETIMTIP = 8,
            
            /// <summary>
            /// Miktar alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 9,
            
            /// <summary>
            /// Ortalama Fiyat alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORTALAMAFIYAT = 10,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "YIL"
            , "ASAMA"
            , "KURKOD"
            , "FONKOD"
            , "FINKOD"
            , "EKOKOD"
            , "PEBKOD"
            , "KURUMSALYAPIKOD"
            , "TUKETIMTIP"
            , "MIKTAR"
            , "ORTALAMAFIYAT"
        };

        /// <summary>
        /// ParTuketim yapısında duran anahtar alanları kullanarak, ilgili kayıtları veri tabanından listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parTuketim">Anahtar değişkenleri tutan ParTuketim türünde nesne</param>
        /// <returns>Oluşturulan select komutunun çalıştırılması sonucunu, IDataReader yapısında nesne döndürür</returns>
        public IDataReader Listele(ParTuketim parTuketim)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (parTuketim.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", parTuketim.yil));

            if (parTuketim.asama > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ASAMA], "=", (int)parTuketim.asama));

            if (parTuketim.kur != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", "%" + parTuketim.kur.Replace(".", "") + "%"));

            if (parTuketim.fon != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FONKOD], "LIKE", "%" + parTuketim.fon.Replace(".", "") + "%"));

            if (parTuketim.fin != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FINKOD], "LIKE", "%" + parTuketim.fin.Replace(".", "") + "%"));

            if (parTuketim.eko != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", "%" + parTuketim.eko.Replace(".", "") + "%"));

            if (parTuketim.peb != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PEBKOD], "LIKE", "%" + parTuketim.peb.Replace(".", "") + "%"));

            if (parTuketim.kurumsalYapiKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMSALYAPIKOD], "LIKE", "%" + parTuketim.kurumsalYapiKod.Replace(".", "") + "%"));

            if (parTuketim.tuketimTip.kod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUKETIMTIP], "LIKE", "%" + parTuketim.tuketimTip.kod + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL] + " ," + ALANLAR[(int)ENUMAlanlar.ASAMA] + " ," + ALANLAR[(int)ENUMAlanlar.TUKETIMTIP];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// ParTuketim yapısında duran bilgileri  veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parTuketim">Bilgileri tutan ParTuketim türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında nesne döndürür</returns>
        public Sonuc Kaydet(ParTuketim parTuketim)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(parTuketim.yil) + ",";
            sql += VeriErisim.DegerYap((int)parTuketim.asama) + ",";
            sql += VeriErisim.DegerYap(parTuketim.kur.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(parTuketim.fon.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(parTuketim.fin.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(parTuketim.eko.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(parTuketim.peb.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(parTuketim.kurumsalYapiKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(parTuketim.tuketimTip.kod) + ",";
            sql += VeriErisim.DegerYap(parTuketim.miktar) + ",";
            sql += VeriErisim.DegerYap(parTuketim.ortalamaFiyat) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MIKTAR], parTuketim.miktar) + " , ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORTALAMAFIYAT], parTuketim.ortalamaFiyat);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", parTuketim.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ASAMA], "=", (int)parTuketim.asama));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", parTuketim.kur.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", parTuketim.fon.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", parTuketim.fin.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", parTuketim.eko.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", parTuketim.peb.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMSALYAPIKOD], "=", parTuketim.kurumsalYapiKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUKETIMTIP], "=", parTuketim.tuketimTip.kod));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// ParTüketim yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parTuketim">Anahtar değişkenleri tutan ParTuketim türünde nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(ParTuketim parTuketim)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", parTuketim.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ASAMA], "=", (int)parTuketim.asama));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", parTuketim.kur.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", parTuketim.fon.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", parTuketim.fin.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", parTuketim.eko.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", parTuketim.peb.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMSALYAPIKOD], "=", parTuketim.kurumsalYapiKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUKETIMTIP], "=", parTuketim.tuketimTip.kod));

            return KomutCalistir(sql, 0);
        }
    }
}
