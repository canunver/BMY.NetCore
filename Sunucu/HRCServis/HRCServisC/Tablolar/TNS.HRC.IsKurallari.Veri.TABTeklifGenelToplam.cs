using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Teklif genel toplam bilgisini tutan HRCTEKLIFGENELTOPLAM veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTeklifGenelToplam : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTeklifGenelToplam(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Teklif genel toplam bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCTEKLIFGENELTOPLAM";

        /// <summary>
        /// Teklif genel toplam bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 3,
            /// <summary>
            /// TIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TIP = 4,
            /// <summary>
            /// GENELTOPLAM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GENELTOPLAM = 5,
            PARABIRIMI = 6,
            TEKLIFTARIHI = 7
        }

        /// <summary>
        /// Teklif genel toplam tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TIP", "GENELTOPLAM", "PARABIRIMI", "TEKLIFTARIHI" };

        /// <summary>
        /// Teklif genel toplam bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kriter">Eklenecek teklif genel toplam bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(TeklifGenelToplam kriter)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(kriter.yil) + ",";
            sql += VeriErisim.DegerYap(kriter.muhasebe) + ",";
            sql += VeriErisim.DegerYap(kriter.birim) + ",";
            sql += VeriErisim.DegerYap(kriter.surecNo) + ",";
            sql += VeriErisim.DegerYap(kriter.tip) + ",";
            sql += VeriErisim.DegerYap(kriter.genelToplam) + ",";
            sql += VeriErisim.DegerYap(kriter.paraBirimi) + ",";
            sql += VeriErisim.DegerYap(kriter.teklifTarihi) + ")";

            sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GENELTOPLAM], kriter.genelToplam) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEKLIFTARIHI], kriter.teklifTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARABIRIMI], kriter.paraBirimi);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kriter.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", kriter.birim));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", kriter.surecNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", kriter.tip));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Teklif genel toplam bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak teklif genel toplam bilgisi kıstas parametresi</param>
        /// <returns>
        /// Teklif genel toplam bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(TeklifGenelToplam kriter)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (kriter.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kriter.yil));
            if (kriter.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebe));
            if (kriter.birim != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", kriter.birim));
            if (kriter.surecNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", kriter.surecNo));
            if (kriter.tip > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", kriter.tip));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.TIP] + ", " + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + ALANLAR[(int)ENUMAlanlar.BIRIMKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Firma teklifleri bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek firma teklif bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(TeklifGenelToplam f)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            sql = "DELETE FROM " + "HRCTEKLIFMALZEME";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (f.yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (f.muhasebe)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (f.birim)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (f.surecNo)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", (f.tip)));

            sonuc = KomutCalistir(sql, 0);
            if (!sonuc.islemSonuc) return sonuc;

            sql = "DELETE FROM " + "HRCTEKLIFGENELTOPLAM";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (f.yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (f.muhasebe)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (f.birim)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (f.surecNo)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", (f.tip)));

            sonuc = KomutCalistir(sql, 0);
            if (!sonuc.islemSonuc) return sonuc;

            sql = "DELETE FROM " + "HRCTEKLIFFIRMA";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (f.yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (f.muhasebe)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (f.birim)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (f.surecNo)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", (f.tip)));

            sonuc = KomutCalistir(sql, 0);
            if (!sonuc.islemSonuc) return sonuc;

            sql = "DELETE FROM " + "HRCTEKLIF";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (f.yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (f.muhasebe)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (f.birim)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (f.surecNo)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", (f.tip)));

            sonuc = KomutCalistir(sql, 0);
            if (!sonuc.islemSonuc) return sonuc;

            return sonuc;
        }

    }
}