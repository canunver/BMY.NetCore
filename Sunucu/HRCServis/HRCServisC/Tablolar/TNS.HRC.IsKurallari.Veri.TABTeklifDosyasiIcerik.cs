using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Teklif dosyası içerik bilgisini tutan HRCTEKLIFDOSYASIICERIK veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTeklifDosyasiIcerik : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTeklifDosyasiIcerik(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Teklif dosyası içerik bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCTEKLIFDOSYASIICERIK";

        /// <summary>
        /// Teklif dosyası içerik bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
        {
            /// <summary>
            /// ISTUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISTUR = 0,
            /// <summary>
            /// IHALEUSUL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEUSUL = 1,
            /// <summary>
            /// SIRA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRA = 2,
            /// <summary>
            /// ADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADI = 3,
            /// <summary>
            /// DOSYAADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOSYADI = 4,
            /// <summary>
            /// DEGKRITERI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DEGKRITERI = 5,
        }

        /// <summary>
        /// Teklif dosyası içerik tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "ISTUR", "IHALEUSUL", "SIRA", "ADI", "DOSYAADI", "DEGKRITERI" };

        /// <summary>
        /// Teklif dosyası içerik bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="liste">Eklenecek teklif dosyası içerik bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (TeklifDosyasiIcerik f in liste.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(f.isTuru) + ",";
                sql += VeriErisim.DegerYap(f.ihaleUsul) + ",";
                sql += VeriErisim.DegerYap(f.sira) + ",";
                sql += VeriErisim.DegerYap(f.adi) + ",";
                sql += VeriErisim.DegerYap(f.dosyaAdi) + ",";
                sql += VeriErisim.DegerYap(f.degKriteri) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    break;
            }

            return sonuc;
        }

        /// <summary>
        /// Teklif dosyası içerik bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak teklif dosyası içerik bilgisi kıstas parametresi</param>
        /// <returns>
        /// Teklif dosyası içerik bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(TeklifDosyasiIcerik bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (bilgi.isTuru != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISTUR], "=", bilgi.isTuru));
            if (bilgi.ihaleUsul != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEUSUL], "=", bilgi.ihaleUsul));
            if (bilgi.sira != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRA], "=", bilgi.sira));
            if (bilgi.adi != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ADI], "LIKE", "%" + bilgi.adi + "%"));
            if (bilgi.dosyaAdi != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ADI], "LIKE", "%" + bilgi.dosyaAdi + "%"));
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRA];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Teklif dosyası içerik bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek teklif dosyası içerik bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(TeklifDosyasiIcerik bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            if (bilgi.isTuru > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISTUR], "=", (bilgi.isTuru)));
            if (bilgi.ihaleUsul > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEUSUL], "=", (bilgi.ihaleUsul)));
            return KomutCalistir(sql, 0);
        }

    }
}