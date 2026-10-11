using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Taahhutname bilgisini tutan HRCTAAHHUTNAME veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTaahhutname : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTaahhutname(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Taahhutname bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCTAAHHUTNAME";

        /// <summary>
        /// Taahhutname bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 0,
            /// <summary>
            /// TUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUR = 1,
            /// <summary>
            /// TARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TARIH = 2,
        }

        /// <summary>
        /// Taahhutname tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR ={ "TCKIMLIKNO", "TUR", "TARIH" };

        /// <summary>
        /// Taahhutname bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tcKimlikNo">Eklenecek TC Kimlik Numarası bilgisini tutan nesne</param>
        /// <param name="tur">Eklenecek tür bilgisini tutan nesne</param>
        /// <param name="tarih">Eklenecek tarih bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Kaydet(string tcKimlikNo, int tur, TNSDateTime tarih)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(tur) + ",";
            sql += VeriErisim.DegerYap(tarih) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "Bu kişi daha önce tahhaütnameyi onaylamış.";
                sonuc.hataStr = veriErisim.hata;
            }

            return sonuc;
        }

        /// <summary>
        /// Taahhutname bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="tcKimlikNo">Eklenecek TC Kimlik Numarası bilgisini tutan nesne</param>
        /// <param name="tur">Eklenecek tür bilgisini tutan nesne</param>
        /// <param name="tarih">Eklenecek tarih bilgisini tutan nesne</param>
        /// <returns>
        /// Taahhutname bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(string tcKimlikNo, int tur, TNSDateTime tarih)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (tcKimlikNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", tcKimlikNo));

            if (tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", tur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}