using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Parametrik Tüketim Ortalama Fiyat bilgilerini tutan PARTUKETIMORTALAMAFIYAT veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABPARTUKETIMORTALAMAFIYAT : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABPARTUKETIMORTALAMAFIYAT(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PARTUKETIMORTALAMAFIYAT";

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
            /// Tüketim Tip alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUKETIMTIP = 1,

            /// <summary>
            /// Ortalama Fiyat alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORTALAMAFIYAT = 2,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "YIL"
            , "TUKETIMTIP"
            , "ORTALAMAFIYAT"
        };

        /// <summary>
        /// ParTuketimOrtalamaFiyat yapısında duran anahtar alanları kullanarak, ilgili kayıtları veri tabanından listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parTuketimOrtalamaFiyat">Anahtar değişkenleri tutan ParTuketimOrtalamaFiyat türünde nesne</param>
        /// <returns>Oluşturulan select komutunun çalıştırılması sonucunu, IDataReader yapısında nesne döndürür</returns>
        public IDataReader Listele(ParTuketimOrtalamaFiyat parTuketimOrtalamaFiyat)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (parTuketimOrtalamaFiyat.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", parTuketimOrtalamaFiyat.yil));

            if (parTuketimOrtalamaFiyat.tuketimTip != null)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUKETIMTIP], "LIKE", "%" + parTuketimOrtalamaFiyat.tuketimTip.kod + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL] + " ," + ALANLAR[(int)ENUMAlanlar.TUKETIMTIP];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// ParTuketimOrtalamaFiyat yapısında duran bilgileri  veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parTuketimOrtalamaFiyat">Bilgileri tutan ParTuketimOrtalamaFiyat türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında nesne döndürür</returns>
        public Sonuc Kaydet(ParTuketimOrtalamaFiyat parTuketimOrtalamaFiyat)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(parTuketimOrtalamaFiyat.yil) + ",";
            sql += VeriErisim.DegerYap(parTuketimOrtalamaFiyat.tuketimTip.kod) + ",";
            sql += VeriErisim.DegerYap(parTuketimOrtalamaFiyat.ortalamaFiyat) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORTALAMAFIYAT], parTuketimOrtalamaFiyat.ortalamaFiyat);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", parTuketimOrtalamaFiyat.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUKETIMTIP], "=", parTuketimOrtalamaFiyat.tuketimTip.kod));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// ParTuketimOrtalamaFiyat yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parTuketimOrtalamaFiyat">Anahtar değişkenleri tutan ParTuketimOrtalamaFiyat türünde nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(ParTuketimOrtalamaFiyat parTuketimOrtalamaFiyat)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", parTuketimOrtalamaFiyat.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUKETIMTIP], "=", parTuketimOrtalamaFiyat.tuketimTip.kod));

            return KomutCalistir(sql, 0);
        }
    }
}
