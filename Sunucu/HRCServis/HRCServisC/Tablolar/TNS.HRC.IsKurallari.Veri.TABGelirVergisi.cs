using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Gelir vergisi tanım bilgisini tutan PSLGELIRVERGISI veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABGelirVergisi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGelirVergisi(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Gelir vergisi tanım bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "PSLGELIRVERGISI";

        /// <summary>
        /// Gelir vergisi tanım bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,
            /// <summary>
            /// YUZDE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YUZDE = 2,
            /// <summary>
            /// MIKTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 3,
            /// <summary>
            /// VERGISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            VERGISI = 4,
        }

        /// <summary>
        /// Gelir vergisi tanım bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "SIRANO", "YUZDE", "MIKTAR", "VERGISI" };

        /// <summary>
        /// Gelir vergisi tanım bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="list">Eklenecek gelir vergisi tanım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray list)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            int sira = 1;
            foreach (PSLGelirVergisi var in list.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(var.yil) + ",";
                sql += sira + ",";
                sql += VeriErisim.DegerYap(var.yuzde) + ",";
                sql += VeriErisim.DegerYap(var.miktar) + ",";
                sql += VeriErisim.DegerYap(var.vergisi) + ")";
                sonuc = KomutCalistir(sql, 0);

                if (!sonuc.islemSonuc)
                    break;
                sira++;
            }
            return sonuc;
        }

        /// <summary>
        /// Gelir vergisi tanım bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek gelir vergisi tanım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(TNS.HRC.PSLGelirVergisi bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Gelir vergisi tanım bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak gelir vergisi tanım bilgisi kıstas parametresi</param>
        /// <returns>
        /// Gelir vergisi tanım bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(TNS.HRC.PSLGelirVergisi bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YUZDE] + " ASC";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}