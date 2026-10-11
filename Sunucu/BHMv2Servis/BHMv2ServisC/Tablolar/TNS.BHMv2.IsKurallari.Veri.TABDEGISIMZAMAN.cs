using System;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;
using System.Data;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Veri değişim zamanlarının tutulduğu BHMODENEKZAMAN veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABDEGISIMZAMAN : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABDEGISIMZAMAN(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Veri değişim zamanlarının tutulduğu tablonun adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMODENEKZAMAN";

        /// <summary>
        /// Veri değişim zamanlarının tutulduğu tablonun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// SENARYO_AY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SENARYO_AY = 1,

            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 2,

            /// <summary>
            /// TUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUR = 3,

            /// <summary>
            /// ZAMAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ZAMAN = 4,

            /// <summary>
            /// KULLANICIKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANICIKOD = 5,
        }

        /// <summary>
        /// Veri değişim zamanlarının tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "YIL",
            "SENARYO_AY",
            "KURKOD",
            "TUR",
            "ZAMAN",
            "KULLANICIKOD"
        };

        /// <summary>
        /// Verinin değişim zamanı ile ilgili bilgileri tutan DegisimZaman nesnesini veri tabanına kaydeden yordam.
        /// Eğer veriye ait anahtarla ilgili kayıt varsa, veri güncellenir; kayıt yoksa veri eklenir.
        /// </summary>
        /// <param name="degZaman">Verinin değişim zamanı ile ilgili bilgileri tutan nesne.</param>
        /// <returns>Oluşturulan güncelleme ve gerekiyorsa ekleme sql komutunun çalıştırılmasının sonucu, Sonuc yapısında döndürülür.</returns>
        public Sonuc OdenekZamanKaydet(DegisimZaman degZaman)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ZAMAN], new TNSDateTime(DateTime.Now)) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKOD], degZaman.kullaniciKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", degZaman.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYO_AY], "=", degZaman.senaryoAy.Replace(".","")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", degZaman.kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", (int)degZaman.tur));
            Sonuc sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ")" + " VALUES (";
                sql += VeriErisim.DegerYap(degZaman.yil) + ", ";
                sql += VeriErisim.DegerYap(degZaman.senaryoAy.Replace(".", "")) + ", ";
                sql += VeriErisim.DegerYap(degZaman.kurKod.Replace(".", "")) + ", ";
                sql += VeriErisim.DegerYap((int)degZaman.tur) + ", ";
                sql += VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ",";
                sql += VeriErisim.DegerYap(degZaman.kullaniciKod) + ")";
                return KomutCalistir(sql, 1);
            }
            else
                return sonuc;
        }

        /// <summary>
        /// Verilen veri değişim zamanı kıstasları birbirine "veya" komutu ile eklenerek oluşturulan sorgu ile,
        /// bu verilerden en son değişeninin zamanını döndüren yordam.
        /// </summary>
        /// <param name="degZamanlar">Değişim zamanı bulunmak istenen veri ve anahtarlarını tutan dizi</param>
        /// <returns>Verilen veriler ve anahtarları kullanılarak bulunan en son değişim zamanı döndürülür.</returns>
        public TNSDateTime OdenekKayitZamani(DegisimZaman[] degZamanlar)
        {
            string kosul = "";
            for (int i = 0; i < degZamanlar.Length; i++)
            {
                string icKosul = "(" + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", degZamanlar[i].yil);
                icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYO_AY], "=", degZamanlar[i].senaryoAy.Replace(".", "")));
                if (degZamanlar[i].tur == ENUMZamanDamgaTur.GIDER)
                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", degZamanlar[i].kurKod.Replace(".", "") + "%"));
                else
                    icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", degZamanlar[i].kurKod.Replace(".", "")));
                icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", (int)degZamanlar[i].tur)) + ")";
                if (!string.IsNullOrEmpty(kosul))
                    kosul += " OR ";
                kosul += icKosul;
            }
            return veriErisim.TekKolonSQLDate("SELECT MAX(ZAMAN) FROM " + TABLOAD + " WHERE " + kosul);
        }
    }
}
