using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Kamu İhale Kurumu bağlantı bilgisini tutan HRCKIKBILGILERI veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABKIKBilgileri : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKIKBilgileri(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Kamu İhale Kurumu bağlantı bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCKIKBILGILERI";

        /// <summary>
        /// Kamu İhale Kurumu bağlantı bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
        {
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 0,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 1,
            /// <summary>
            /// KULLANICIADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANICIADI = 2,
            /// <summary>
            /// PAROLA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PAROLA = 3,
        }

        /// <summary>
        /// Kamu İhale Kurumu bağlantı tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "MUHASEBEKOD", "BIRIMKOD", "KULLANICIADI", "PAROLA" };

        /// <summary>
        /// Kamu İhale Kurumu bağlantı bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek kamu İhale Kurumu bağlantı bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (KIKBilgileri deger in bilgi.objeler)
            {
                if (!deger.sil)
                {
                    sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                    sql += VeriErisim.DegerYap(deger.muhasebe) + ",";
                    sql += VeriErisim.DegerYap(deger.birim) + ",";
                    sql += VeriErisim.DegerYap(deger.kullaniciAdi) + ",";
                    sql += VeriErisim.DegerYap(deger.parola) + ")";

                    KomutCalistir(sql, 1);
                }
            }
            return sonuc;
        }

        /// <summary>
        /// Kamu İhale Kurumu bağlantı bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil()
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;

            sonuc = KomutCalistir(sql, 0);
            
            return sonuc;
        }

        /// <summary>
        /// Kamu İhale Kurumu bağlantı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak kamu İhale Kurumu bağlantı bilgisi kıstas parametresi</param>
        /// <returns>
        /// Kamu İhale Kurumu bağlantı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(KIKBilgileri bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + ALANLAR[(int)ENUMAlanlar.BIRIMKOD];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}