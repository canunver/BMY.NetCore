using System;
using System.Data;
using TNS.Veri;
using TNS.BKD;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Para birimi bilgisini tutan HRCPARABIRIMI veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTurkcellLog : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTurkcellLog(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Para birimi bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCTURKCELLLOG";

        /// <summary>
        /// Para birimi bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// ADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 1,
            /// <summary>
            /// KISALTMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIH = 2,
            ISLEMTUR = 3,
            TRANSACTIONID = 4,
            DOSYAADI = 5,
            FATURABILGI = 6,
            ISLEMSONUC = 7,
            ACIKLAMA = 8,
        }

        /// <summary>
        /// Para birimi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "SURECNO", "ISLEMTARIH", "ISLEMTUR", "TRANSACTIONID", "DOSYAADI", "FATURABILGI", "ISLEMSONUC", "ACIKLAMA" };

        /// <summary>
        /// Para birimi bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(TurkcellLog bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.islemTarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.islemTur) + ",";
            sql += VeriErisim.DegerYap(bilgi.transactionId) + ",";
            sql += VeriErisim.DegerYap(bilgi.dosyaAdi) + ",";
            sql += VeriErisim.DegerYap(bilgi.faturaBilgi) + ",";
            sql += VeriErisim.DegerYap(bilgi.islemSonuc) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ")";

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        /// <summary>
        /// Para birimi bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(TurkcellLog bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TRANSACTIONID], "=", bilgi.transactionId));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Para birimi bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Para birimi bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(TurkcellLog bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));

            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            if (bilgi.dosyaAdi != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAADI], "=", bilgi.dosyaAdi));

            if (bilgi.transactionId != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TRANSACTIONID], "=", bilgi.transactionId));

            if (!bilgi.islemTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], "=", bilgi.islemTarih));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.DOSYAADI] + " DESC ";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}