using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// Taşıt talep bilgilerini tutan YATTASITLISTESI veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABYATTasitListesi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATTasitListesi(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATTASITLISTESI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURUM alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KURUM = 1,

            /// <summary>
            /// SEKTOR alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SEKTOR = 2,

            /// <summary>
            /// SIRANO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 3,

            /// <summary>
            /// TASITSIRANO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TASITSIRANO = 4,

            /// <summary>
            /// MEVCUTADET alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            MEVCUTADET = 5,

            /// <summary>
            /// MEVCUTMODEL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            MEVCUTMODEL = 6,

            /// <summary>
            /// TALEPADET alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TALEPADET = 7,

            /// <summary>
            /// TALEPACIKLAMA alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TALEPACIKLAMA = 8
        }

        /// <summary>
        /// Taşıt taleplerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "KURUM", "SEKTOR", "SIRANO", "TASITSIRANO", "MEVCUTADET", "MEVCUTMODEL", "TALEPADET", "TALEPACIKLAMA" };

        /// <summary>
        /// Taşıt talep bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurumKod">Kurum kodu</param>
        /// <param name="sektor">Sektör kodu</param>
        /// <param name="siraNo">Kaydın veri tabanındaki sıra numarası</param>
        /// <param name="tasitSiraNo">Taşıtın cinsi</param>
        /// <param name="mevcutAdet">Mevcut taşıt sayısı</param>
        /// <param name="mevcutModel">Mevcut taşıtın modeli</param>
        /// <param name="talepAdet">Talep edilen miktar</param>
        /// <param name="talepAciklama">Talebe ilişkin açıklama</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(int yil, string kurumKod, string sektor, int siraNo, string tasitSiraNo, int mevcutAdet, string mevcutModel, int talepAdet, string talepAciklama)
        {
            string sql;
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(kurumKod.Replace(".","")) + ",";
            sql += VeriErisim.DegerYap(sektor) + ",";
            sql += VeriErisim.DegerYap(siraNo) + ",";
            sql += VeriErisim.DegerYap(tasitSiraNo) + ",";
            sql += VeriErisim.DegerYap(mevcutAdet) + ",";
            sql += VeriErisim.DegerYap(mevcutModel) + ",";
            sql += VeriErisim.DegerYap(talepAdet) + ",";
            sql += VeriErisim.DegerYap(talepAciklama) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        /// <summary>
        /// Verilen yılı, kurum kodunu ve sektör kodunu kullanarak,
        /// ilgili taşıt bedellerini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurumKod">Kurum kodu</param>
        /// <param name="sektor">Sektör kodu</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(int yil, string kurumKod, string sektor)
        {
            string sql;
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUM], "=", kurumKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SEKTOR], "=", sektor.Replace(".", "")));

            Sonuc sonuc = KomutCalistir(sql, 0);
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen yılı, kurum kodunu ve sektör kodunu kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve taşıt taleplerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurumKod">Kurum kodu</param>
        /// <param name="sektor">Sektör kodu</param>
        /// <returns>Verilen kriterlere uygun olan taşıt taleplerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(int yil, string kurumKod, string sektor)
        {
            string sql;
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUM], "=", kurumKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SEKTOR], "=", sektor.Replace(".", "")));
            sql += " ORDER BY 1,2,3,4";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}