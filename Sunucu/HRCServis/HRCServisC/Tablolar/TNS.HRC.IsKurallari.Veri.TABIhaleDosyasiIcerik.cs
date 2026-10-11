using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// İhale Dosya İçerik bilgisini tutan HRCIHALEDOSYASIICERIK veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIhaleDosyasiIcerik : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIhaleDosyasiIcerik(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// İhale Dosya İçerik bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCIHALEDOSYASIICERIK";

        /// <summary>
        /// İhale Dosya İçerik bilgisi tablo alanlarının sırasını tutan enumeration
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
            DOSYAADI = 4,
        }

        /// <summary>
        /// İhale Dosya İçerik bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "ISTUR", "IHALEUSUL", "SIRA", "ADI", "DOSYAADI" };

        /// <summary>
        /// İhale Dosya İçerik bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="liste">Eklenecek İhale Dosya İçerik bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Kaydet(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (IhaleDosyasiIcerik f in liste.objeler)
            {

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(f.isTuru) + ",";
                sql += VeriErisim.DegerYap(f.ihaleUsul) + ",";
                sql += VeriErisim.DegerYap(f.sira) + ",";
                sql += VeriErisim.DegerYap(f.adi) + ",";
                sql += VeriErisim.DegerYap(f.dosyaAdi) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    break;
            }

            return sonuc;
        }

        /// <summary>
        /// İhale Dosya İçerik bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak İhale Dosya İçerik bilgisi kıstas parametresi</param>
        /// <returns>
        /// İhale Dosya İçerik bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(IhaleDosyasiIcerik bilgi)
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

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRA];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// İhale Dosya İçerik bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek İhale Dosya İçerik bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Sil(IhaleDosyasiIcerik bilgi)
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