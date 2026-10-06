using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.KYM.IsKurallari.Veri
{
    /// <summary>
    /// Kullanıcının girdiği filtreyi alanları ve değerleriyle tutan BHMFILTRE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABFILTRE : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABFILTRE(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Filtre tablosunun adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "FILTRE";

        /// <summary>
        /// Filtre tablosunun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// KULLANICIKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANICIKODU = 0,

            /// <summary>
            /// GRUP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GRUP = 1,

            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,

            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 3,

            /// <summary>
            /// KRITER alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KRITER = 4,
        }

        /// <summary>
        /// Filtre tablosunun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KULLANICIKODU", "GRUP", "SIRANO", "AD", "KRITER" };

        /// <summary>
        /// Filtrenin bilgilerini (grubu, kullanıcının girdiği ismi, xml formatında değer bilgilerini) veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="filtre">Eklenecek filtrenin bilgilerini tutan nesne</param>
        /// <returns>Ekleme komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(Filtre filtre)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(filtre.kullanan.kullaniciKodu) + ",";
            sql += VeriErisim.DegerYap(filtre.grup) + ",";
            sql += VeriErisim.DegerYap(filtre.siraNo) + ",";
            sql += VeriErisim.DegerYap(filtre.ad) + ",";
            sql += VeriErisim.DegerYap(filtre.kriter) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Kullanıcının daha önce kaydettiği bir filtrenin ismini, grubunu veya alan değerlerini değişteren
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="filtre">Güncellenecek filtrenin bilgilerini tutan nesne</param>
        /// <returns>Ekleme komutunun çalıştırılmasının sonucu, Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(Filtre filtre)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], filtre.ad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KRITER], filtre.kriter);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", filtre.kullanan.kullaniciKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GRUP], "=", filtre.grup));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", filtre.siraNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Kullanıcıya ait verilen filtre grubundaki tüm filtreleri listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="kullaniciKodu">Listelenecek filtrelerin sahibi olan kullanıcının kodu.</param>
        /// <param name="grupAd">Listelenecek filtrelerin grubunu gösteren dizgi.</param>
        /// <param name="siraNo">0 ise grupdaki tüm kayıtlı filtreler, 0'dan farklı ise verilen sıradaki filtre kaydı okunur.</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public IDataReader Listele(string kullaniciKodu, string grupAd, int siraNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", kullaniciKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GRUP], "=", grupAd));
            if (siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Kullanıcıya ait numarası verilen filtreyi silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="filtre">Silinecek filtrenin kullanıcısını ve sırasını tutan nesne</param>
        /// <returns>Silme komutunun çalıştırılmasının sonucu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(Filtre filtre)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", filtre.kullanan.kullaniciKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GRUP], "=", filtre.grup));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", filtre.siraNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Kullanıcıya ait en son kayıtlı filtrenin numarasının bir fazlasını yeni filtre kaydında kullanılması için döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Yeni filtre kaydı yapılan kullanıcının kodu</param>
        /// <param name="grupAd">Listelenecek filtrelerin grubunu gösteren dizgi.</param>
        /// <returns>Verilen kullanıcıya ait filtrelerin sıra numarasının bir fazlası döndürülür.</returns>
        public int AnahtarBul(string kullaniciKodu, string grupAd)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.SIRANO] + ") FROM " + TABLOAD;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", kullaniciKodu);
            sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GRUP], "=", grupAd);
            int tut = veriErisim.TekKolonSQLInt(sql);
            tut++;
            return tut;
        }
    }
}