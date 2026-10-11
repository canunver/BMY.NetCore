using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Parametrik Taşınır Ortalama fiyat bilgilerini tutan PARTASINIRORTALAMAFIYAT veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABPARTASINIRORTALAMAFIYAT : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABPARTASINIRORTALAMAFIYAT(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PARTASINIRORTALAMAFIYAT";

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
            /// Hesap Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPKOD = 1,

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
            , "HESAPKOD"
            , "ORTALAMAFIYAT"
        };

        /// <summary>
        /// ParTasinirOrtalamaFiyat yapısında duran anahtar alanları kullanarak, ilgili kayıtları veri tabanından listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parTasinirOrtalamaFiyat">Anahtar değişkenleri tutan ParTasinirOrtalamaFiyat türünde nesne</param>
        /// <returns>Oluşturulan select komutunun çalıştırılması sonucunu, IDataReader yapısında nesne döndürür</returns>
        public IDataReader Listele(ParTasinirOrtalamaFiyat parTasinirOrtalamaFiyat)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (parTasinirOrtalamaFiyat.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", parTasinirOrtalamaFiyat.yil));

            if (parTasinirOrtalamaFiyat.hesapkod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HESAPKOD], "LIKE", "%" + parTasinirOrtalamaFiyat.hesapkod.Replace(".", "") + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL] + " ," + ALANLAR[(int)ENUMAlanlar.HESAPKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// ParTasinirOrtalamaFiyat yapısında duran bilgileri  veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parTasinirOrtalamaFiyat">Bilgileri tutan ParTasinirOrtalamaFiyat türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında nesne döndürür</returns>
        public Sonuc Kaydet(ParTasinirOrtalamaFiyat parTasinirOrtalamaFiyat)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(parTasinirOrtalamaFiyat.yil) + ",";
            sql += VeriErisim.DegerYap(parTasinirOrtalamaFiyat.hesapkod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(parTasinirOrtalamaFiyat.ortalamafiyat) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORTALAMAFIYAT], parTasinirOrtalamaFiyat.ortalamafiyat);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", parTasinirOrtalamaFiyat.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "=", parTasinirOrtalamaFiyat.hesapkod.Replace(".", "")));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// ParTasinirOrtalamaFiyat yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parTasinirOrtalamaFiyat">Anahtar değişkenleri tutan ParTasinirOrtalamaFiyat türünde nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında nesne döndürür</returns>
        public Sonuc Sil(ParTasinirOrtalamaFiyat parTasinirOrtalamaFiyat)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", parTasinirOrtalamaFiyat.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "=", parTasinirOrtalamaFiyat.hesapkod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }
    }
}
