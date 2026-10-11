using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// Hizmet Alımı Görevli bilgilerini tutan BHFHIZMETALIMIGOREVLI veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHizmetAlimiGorevli : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHizmetAlimiGorevli(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "BHFHIZMETALIMIGOREVLI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Kurumsal alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURUMSAL = 1,
            /// <summary>
            /// Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 2,
            /// <summary>
            /// Özel Görevli Temizlik Adet 2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLITEMIZLIKADET2 = 3,
            /// <summary>
            /// Özel Görevli Temizlik Süre 2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLITEMIZLIKSURE2 = 4,
            /// <summary>
            /// Özel Görevli Temizlik Maliyet 2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLITEMIZLIKMALIYET2 = 5,
            /// <summary>
            /// Özel Görevli Güvenlik Adet 2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLIGUVENLIKADET2 = 6,
            /// <summary>
            /// Özel Görevli Güvenlik Süre 2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLIGUVENLIKSURE2 = 7,
            /// <summary>
            /// Özel Görevli Güvenlik Maliyet 2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLIGUVENLIKMALIYET2 = 8,
            /// <summary>
            /// Özel Görevli Şoför Adet 2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLISOFORADET2 = 9,
            /// <summary>
            /// Özel Görevli Şoför Süre 2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLISOFORSURE2 = 10,
            /// <summary>
            /// Özel Görevli Şoför Maliyet 2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLISOFORMALIYET2 = 11,
            /// <summary>
            /// Özel Görevli Diğer Adet 2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLIDIGERADET2 = 12,
            /// <summary>
            /// Özel Görevli Diğer Süre 2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLIDIGERSURE2 = 13,
            /// <summary>
            /// Özel Görevli Diğer Maliyet 2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLIDIGERMALIYET2 = 14,
            /// <summary>
            /// Özel Görevli Temizlik Adet 1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLITEMIZLIKADET1 = 15,
            /// <summary>
            /// Özel Görevli Temizlik Süre 1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLITEMIZLIKSURE1 = 16,
            /// <summary>
            /// Özel Görevli Temizlik Maliyet 1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLITEMIZLIKMALIYET1 = 17,
            /// <summary>
            /// Özel Görevli Güvenlik Adet 1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLIGUVENLIKADET1 = 18,
            /// <summary>
            /// Özel Görevli Güvenlik Süre 1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLIGUVENLIKSURE1 = 19,
            /// <summary>
            /// Özel Görevli Güvenlik Maliyet 1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLIGUVENLIKMALIYET1 = 20,
            /// <summary>
            /// Özel Görevli Şoför Adet 1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLISOFORADET1 = 21,
            /// <summary>
            /// Özel Görevlik Şoför Süre 1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLISOFORSURE1 = 22,
            /// <summary>
            /// Özel Görevli Şoför Maliyet 1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLISOFORMALIYET1 = 23,
            /// <summary>
            /// Özel Görevli Diğer Adet 1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLIDIGERADET1 = 24,
            /// <summary>
            /// Özel Görevli Diğer Süre 1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLIDIGERSURE1 = 25,
            /// <summary>
            /// Özel Görevli Diğer Maliyet 1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OZELGOREVLIDIGERMALIYET1 = 26
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "KURUMSAL", "KOD", "OZELGOREVLITEMIZLIKADET2", "OZELGOREVLITEMIZLIKSURE2", "OZELGOREVLITEMIZLIKMALIYET2", "OZELGOREVLIGUVENLIKADET2", "OZELGOREVLIGUVENLIKSURE2", "OZELGOREVLIGUVENLIKMALIYET2", "OZELGOREVLISOFORADET2", "OZELGOREVLISOFORSURE2", "OZELGOREVLISOFORMALIYET2", "OZELGOREVLIDIGERADET2", "OZELGOREVLIDIGERSURE2", "OZELGOREVLIDIGERMALIYET2", "OZELGOREVLITEMIZLIKADET1", "OZELGOREVLITEMIZLIKSURE1", "OZELGOREVLITEMIZLIKMALIYET1", "OZELGOREVLIGUVENLIKADET1", "OZELGOREVLIGUVENLIKSURE1", "OZELGOREVLIGUVENLIKMALIYET1", "OZELGOREVLISOFORADET1", "OZELGOREVLISOFORSURE1", "OZELGOREVLISOFORMALIYET1", "OZELGOREVLIDIGERADET1", "OZELGOREVLIDIGERSURE1", "OZELGOREVLIDIGERMALIYET1" };

        /// <summary>
        /// Tablo alanlarının tiplerini tutan sabit dizi
        /// </summary>
        static int[] ALANTIP = { (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.STRING, (int)ENUMAlanTip.STRING, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC };

        /// <summary>
        /// Yıl ve Kurum anahtar alanlarını kullanarak ilgili alanları veri tabanından listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl değerini tutan Int tipinde nesne</param>
        /// <param name="kurum">Kurum değerini tutan String tipinde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu IDataReader yapısında nesne döndürür</returns>
        public IDataReader Listele(int yil, string kurum)
        {
            string sql;
            sql = "select " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql = sql + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMSAL], "LIKE", kurum.Replace(".", "") + "%"));
            sql = sql + " order by 1,2,3";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yil, Kurum, Kod ve Özel Görevli Tipi, Adeti ve sürelerine göre gelen değerleri kullanarak ilgili veri tabanına bilgileri kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yil değerini tutan Int tipinde nesne</param>
        /// <param name="kurum">Kurum değerini tutan String tipinde nesne</param>
        /// <param name="kod">Kod değerini tutan String tipinde nesne</param>
        /// <param name="ozelgorevlitemizlikadet2">Özel Görevli Temizlik Adet 2 değerini tutan Int tipinde nesne</param>
        /// <param name="ozelgorevlitemizliksure2">Özel Görevli Temizlik Süre 2 değerini tutan Int tipinde nesne</param>
        /// <param name="ozelgorevlitemizlikmaliyet2">Özel Görevli Temizlik Maliyet 2 değerini tutan Double tipinde nesne</param>
        /// <param name="ozelgorevliguvenlikadet2">Özel Görevli Güvenlik Adet 2 değerini tutan Int tipinde nesne</param>
        /// <param name="ozelgorevliguvenliksure2">Özel Görevli Güvenlik Süre 2 değerini tutan Int tipinde nesne</param>
        /// <param name="ozelgorevliguvenlikmaliyet2">Özel Görevli Güvenlik Maliyet 2 değerini tutan Double tipinde nesne</param>
        /// <param name="ozelgorevlisoforadet2">Özel Görevli Şoför Adet 2 değerini tutan Int tipinde nesne</param>
        /// <param name="ozelgorevlisoforsure2">Özel Görevli Şoför Süre 2 değerini tutan Int tipinde nesne</param>
        /// <param name="ozelgorevlisoformaliyet2">Özel Görevli Şoför Maliyet 2 değerini tutan Double tipinde nesne</param>
        /// <param name="ozelgorevlidigeradet2">Özel Görevli Diğer Adet 2 değerini tutan Int tipinde nesne</param>
        /// <param name="ozelgorevlidigersure2">Özel Görevli Diğer Süre 2 değerini tutan Int tipinde nesne</param>
        /// <param name="ozelgorevlidigermaliyet2">Özel Görevli Diğer Maliyet 2 değerini tutan Double tipinde nesne</param>
        /// <param name="ozelgorevlitemizlikadet1">Özel Görevli Temizlik Adet 1 değerini tutan Int tipinde nesne</param>
        /// <param name="ozelgorevlitemizliksure1">Özel Görevli Temizlik Süre 1 değerini tutan Int tipinde nesne</param>
        /// <param name="ozelgorevlitemizlikmaliyet1">Özel Görevli Temizlik Maliyet 1 değerini tutan Double tipinde nesne</param>
        /// <param name="ozelgorevliguvenlikadet1">Özel Görevli Güvenlik Adet 1 değerini tutan Int tipinde nesne</param>
        /// <param name="ozelgorevliguvenliksure1">Özel Görevli Güvenlik Süre 1 değerini tutan Int tipinde nesne</param>
        /// <param name="ozelgorevliguvenlikmaliyet1">Özel Görevli Güvenlik Maliyet 1 değerini tutan Double tipinde nesne</param>
        /// <param name="ozelgorevlisoforadet1">Özel Görevli Şoför Adet 1 değerini tutan Int tipinde nesne</param>
        /// <param name="ozelgorevlisoforsure1">Özel Görevli Şoför Süre 1 değerini tutan Int tipinde nesne</param>
        /// <param name="ozelgorevlisoformaliyet1">Özel Görevli Şoför Maliyet 1 değerini tutan Double tipinde nesne</param>
        /// <param name="ozelgorevlidigeradet1">Özel Görevli Diğer Adet 1 değerini tutan Int tipinde nesne</param>
        /// <param name="ozelgorevlidigersure1">Özel Görevli Diğer Süre 1 değerini tutan Int tipinde nesne</param>
        /// <param name="ozelgorevlidigermaliyet1">Özel Görevli Diğer Maliyet 1 değerini tutan Double tipinde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda Sonuc tipinde nesne döndürür</returns>
        public Sonuc Kaydet(int yil, string kurum, string kod, int ozelgorevlitemizlikadet2, int ozelgorevlitemizliksure2, double ozelgorevlitemizlikmaliyet2, int ozelgorevliguvenlikadet2, int ozelgorevliguvenliksure2, double ozelgorevliguvenlikmaliyet2, int ozelgorevlisoforadet2, int ozelgorevlisoforsure2, double ozelgorevlisoformaliyet2, int ozelgorevlidigeradet2, int ozelgorevlidigersure2, double ozelgorevlidigermaliyet2, int ozelgorevlitemizlikadet1, int ozelgorevlitemizliksure1, double ozelgorevlitemizlikmaliyet1, int ozelgorevliguvenlikadet1, int ozelgorevliguvenliksure1, double ozelgorevliguvenlikmaliyet1, int ozelgorevlisoforadet1, int ozelgorevlisoforsure1, double ozelgorevlisoformaliyet1, int ozelgorevlidigeradet1, int ozelgorevlidigersure1, double ozelgorevlidigermaliyet1)
        {
            string sql = "";
            sql = "insert into " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (" +
                VeriErisim.DegerYap(yil) + ", " +
                VeriErisim.DegerYap(kurum.Replace(".", "")) + ", " +
                VeriErisim.DegerYap(kod) + ", " +
                VeriErisim.DegerYap(ozelgorevlitemizlikadet2) + ", " +
                VeriErisim.DegerYap(ozelgorevlitemizliksure2) + ", " +
                VeriErisim.DegerYap(ozelgorevlitemizlikmaliyet2) + ", " +
                VeriErisim.DegerYap(ozelgorevliguvenlikadet2) + ", " +
                VeriErisim.DegerYap(ozelgorevliguvenliksure2) + ", " +
                VeriErisim.DegerYap(ozelgorevliguvenlikmaliyet2) + ", " +
                VeriErisim.DegerYap(ozelgorevlisoforadet2) + ", " +
                VeriErisim.DegerYap(ozelgorevlisoforsure2) + ", " +
                VeriErisim.DegerYap(ozelgorevlisoformaliyet2) + ", " +
                VeriErisim.DegerYap(ozelgorevlidigeradet2) + ", " +
                VeriErisim.DegerYap(ozelgorevlidigersure2) + ", " +
                VeriErisim.DegerYap(ozelgorevlidigermaliyet2) + ", " +
                VeriErisim.DegerYap(ozelgorevlitemizlikadet1) + ", " +
                VeriErisim.DegerYap(ozelgorevlitemizliksure1) + ", " +
                VeriErisim.DegerYap(ozelgorevlitemizlikmaliyet1) + ", " +
                VeriErisim.DegerYap(ozelgorevliguvenlikadet1) + ", " +
                VeriErisim.DegerYap(ozelgorevliguvenliksure1) + ", " +
                VeriErisim.DegerYap(ozelgorevliguvenlikmaliyet1) + ", " +
                VeriErisim.DegerYap(ozelgorevlisoforadet1) + ", " +
                VeriErisim.DegerYap(ozelgorevlisoforsure1) + ", " +
                VeriErisim.DegerYap(ozelgorevlisoformaliyet1) + ", " +
                VeriErisim.DegerYap(ozelgorevlidigeradet1) + ", " +
                VeriErisim.DegerYap(ozelgorevlidigersure1) + ", " +
                VeriErisim.DegerYap(ozelgorevlidigermaliyet1) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Yıl, Kurum alanlarını kullanarak veri tabanında ilgili alanları silen 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl değerini tutan Int tipinde nesne</param>
        /// <param name="kurum">Kurum değerini tutan String tipinde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda Sonuc tipinde nesne döndürür</returns>
        public Sonuc Sil(int yil, string kurum)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMSAL], "=", kurum.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }
    }
}
