using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Ölçü birim bilgilerini tutan PEBOLCUBIRIM veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABOlcuBirim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABOlcuBirim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBOLCUBIRIM";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// OLCUBIRIMKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            OLCUBIRIMKOD = 0,

            /// <summary>
            /// OLCUBIRIMAD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            OLCUBIRIMAD = 1,

            /// <summary>
            /// EBUTCEKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            EBUTCEKOD = 2
        }

        /// <summary>
        /// Ölçü birimlerin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "OLCUBIRIMKOD", "OLCUBIRIMAD", "EBUTCEKOD" };

        /// <summary>
        /// Ölçü birim bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Ölçü birimin programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="olcu">Ölçü birim bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(OlcuBirim olcu)
        {
            olcu.kod = OlcuBirimKodAl();
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(olcu.kod) + ",";
            sql += VeriErisim.DegerYap(olcu.ad) + ",";
            sql += VeriErisim.DegerYap(olcu.ebutceKod) + ")";
            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = olcu.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan ölçü birim bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="olcu">Ölçü birim bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(OlcuBirim olcu)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OLCUBIRIMAD], olcu.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EBUTCEKOD], olcu.ebutceKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.OLCUBIRIMKOD], "=", olcu.kod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen ölçü birimi kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve ölçü birimleri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="olcu">Ölçü birim bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan ölçü birimleri tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(OlcuBirim olcu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (olcu.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.OLCUBIRIMKOD], "=", olcu.kod));
            if (!string.IsNullOrEmpty(olcu.ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.OLCUBIRIMAD], "LIKE", "%" + olcu.ad + "%"));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            //sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.OLCUBIRIMAD];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen ölçü birimi kullanarak,
        /// ilgili ölçü birimi veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="olcu">Ölçü birim bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(OlcuBirim olcu)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.OLCUBIRIMKOD], "=", olcu.kod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen ölçü birim programatik kodunu kullanarak,
        /// ölçü birimin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="olcuKod">Ölçü birimin programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool OlcuBirimVarMi(int olcuKod)
        {
            return KayitVarmi(TABLOAD, olcuKod);
        }

        /// <summary>
        /// Parametre olarak verilen ölçü birimin, performans göstergelerinden
        /// herhangi birinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="olcuKod">Ölçü birimin programatik kodu</param>
        /// <returns>Ölçü birim; performans göstergelerinde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool OlcuBirimKullanilmisMi(int olcuKod)
        {
            return KayitVarmi("PEBGOSTERGE", olcuKod);
        }

        /// <summary>
        /// Parametre olarak gönderilen tabloda verilen ölçü birim var mı kontrolü yapan yordam
        /// </summary>
        /// <param name="tablo">Tablo adı</param>
        /// <param name="olcuKod">Ölçü birimin programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        private bool KayitVarmi(string tablo, int olcuKod)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.OLCUBIRIMKOD] };
            int[] intDeger = { olcuKod };
            return TABGenel.KayitVarmi(veriErisim, tablo, new string[0], new string[0], intAlan, intDeger);
        }

        /// <summary>
        /// Yeni ölçü birim programatik kodu döndüren yordam
        /// </summary>
        /// <returns>Yeni ölçü birim programatik kodu döndürülür.</returns>
        private int OlcuBirimKodAl()
        {
            return TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.OLCUBIRIMKOD], new string[0], new string[0], new string[0], new int[0]);
        }
    }
}