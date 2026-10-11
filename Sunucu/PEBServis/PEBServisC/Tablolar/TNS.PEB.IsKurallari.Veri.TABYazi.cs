using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Stratejik plan detay bilgilerini tutan STRYAZI veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABYazi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYazi(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "STRYAZI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// DONEMBYIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DONEMBYIL = 0,

            /// <summary>
            /// KODTUR alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KODTUR = 1,

            /// <summary>
            /// KOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KOD = 2,

            /// <summary>
            /// DILKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DILKOD = 3,

            /// <summary>
            /// KISAYAZI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KISAYAZI = 4,

            /// <summary>
            /// UZUNYAZI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            UZUNYAZI = 5,

            /// <summary>
            /// DOSYA alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DOSYA = 6,

            /// <summary>
            /// EBUTCEKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            EBUTCEKOD = 7,
            KURUMKOD = 8
        }

        /// <summary>
        /// Stratejik plan detay bilgilerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "DONEMBYIL", "KODTUR", "KOD", "DILKOD", "KISAYAZI", "UZUNYAZI", "DOSYA", "EBUTCEKOD", "KURUMKOD" };

        /// <summary>
        /// Stratejik plan detay bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="yazi">Stratejik plan detayı</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(StratejikYazi yazi)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(yazi.donemBYil) + ",";
            sql += VeriErisim.DegerYap(yazi.kodTur) + ",";
            sql += VeriErisim.DegerYap(yazi.kod) + ",";
            sql += VeriErisim.DegerYap(yazi.dil.kod) + ",";
            sql += VeriErisim.DegerYap(yazi.kisaYazi) + ",";
            sql += VeriErisim.DegerYap(yazi.uzunYazi) + ",";
            sql += VeriErisim.DegerYap(yazi.dosya) + ",";
            sql += VeriErisim.DegerYap(yazi.ebutceKod) + ",";
            sql += VeriErisim.DegerYap(yazi.kurumKod.Replace(".", "")) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan stratejik plan detay bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="yazi">Stratejik plan detayı</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(StratejikYazi yazi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISAYAZI], yazi.kisaYazi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UZUNYAZI], yazi.uzunYazi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOSYA], yazi.dosya) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EBUTCEKOD], yazi.ebutceKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", yazi.donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KODTUR], "=", yazi.kodTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", yazi.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DILKOD], "=", yazi.dil.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", yazi.kurumKod.Replace(".", "")));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen stratejik yazı nesnesini kullanarak,
        /// ilgili stratejik plan detayını veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="yazi">Stratejik plan detayı</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(StratejikYazi yazi)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", yazi.donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KODTUR], "=", yazi.kodTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", yazi.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", yazi.kurumKod.Replace(".", "")));
            //Bir tema siliniyorsa altındaki bütün dilleri ile silinmeli
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DILKOD], "=", yazi.dil.kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Verilen stratejik yazı nesnesini kullanarak stratejik plan detayının
        /// veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="yazi">Stratejik plan detayı</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool YaziVarMi(StratejikYazi yazi)
        {
            string[] strAlan ={ ALANLAR[(int)ENUMAlanlar.KURUMKOD] };
            string[] strDeger = { yazi.kurumKod.Replace(".", "")};
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.DONEMBYIL], ALANLAR[(int)ENUMAlanlar.KODTUR], ALANLAR[(int)ENUMAlanlar.KOD], ALANLAR[(int)ENUMAlanlar.DILKOD] };
            int[] intDeger = { yazi.donemBYil, yazi.kodTur, yazi.kod, yazi.dil.kod };
            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }

        public Sonuc Kopyala(int kaynakYil, int hedefYil)
        {
            string sql = $"INSERT INTO {TABLOAD}(" + VeriErisim.TumAlanlar("", ALANLAR) + ")";
            sql += $" SELECT {hedefYil} DONEMBYIL, " + VeriErisim.TumAlanlarEksik("", ALANLAR, new int[] { 0 }) + " FROM " + TABLOAD;
            sql += $" WHERE DONEMBYIL={kaynakYil} AND 1 NOT IN (SELECT 1 FROM {TABLOAD} IC WHERE IC.DONEMBYIL={hedefYil} AND IC.KODTUR = {TABLOAD}.KODTUR AND IC.KOD = {TABLOAD}.KOD AND IC.DILKOD = {TABLOAD}.DILKOD) ";

            return KomutCalistir(sql, 0);
        }
    }
}