using System;
using System.Data;
using System.Threading;
using TNS.Veri;

namespace TNS.KYM.IsKurallari
{
    /// <summary>
    /// Sayfa isteği kayıtlarının (KULLANICI_BILGI) saklama süresi dolanlarını KULLANICI_BILGI_ARSIV tablosuna taşır.
    /// Eski sistemde her sayfa isteği KULLANICI_BILGI'ye yazıldığı için tablo çok büyüyordu. Yeni sistemde tablo tek kalır,
    /// ISLEMTARIHI üzerinde index vardır; saklama süresinden (varsayılan 2 yıl) eski kayıtlar küçük partiler halinde arşive taşınır.
    /// Taşıma ayda bir çalışır (Istemci KullaniciBilgiArsivIsi); her partide tek bir DELETE ... OUTPUT INTO komutuyla yapılır: bir satır ya taşınır ya da yerinde kalır, iki tabloda birden durmaz.
    /// Arşivdeki kayıtlar silinmez; liste ve grafik sayfaları yalnızca ana tabloyu okur.
    /// Yalnızca SQL Server veri tabanı için yazılmıştır (KYMOTURUM gibi).
    /// </summary>
    public class KullaniciBilgiArsivi
    {
        private const string BAGLANTI = "BaglantiSatiriKYM";
        public const string TABLO = "KULLANICI_BILGI";
        public const string ARSIV = "KULLANICI_BILGI_ARSIV";

        /// <summary>TABKullanici.ALANLARBILGI ile aynı alanlar, aynı sırada.</summary>
        private const string ALANLAR = "KULLANICIKODU, ISLEMTARIHI, ISLEMYERI, SAYFAADI, BILGI1, BILGI2, BILGI3, BILGI4, BILGI5";

        /// <summary>Uzun süren komutların (index oluşturma, büyük partiler) zaman aşımı (saniye).</summary>
        private const int UZUN_SURE = 3600;

        private static readonly object kilit = new object();
        private static bool calisiyor = false;

        /// <summary>
        /// Arşiv tablosu yoksa KULLANICI_BILGI ile aynı alanlarla oluşturur, iki tabloya da ISLEMTARIHI index'ini ekler.
        /// Hepsi "yoksa" koşulludur; her çalışmada güvenle çağrılır.
        /// </summary>
        public static Sonuc Hazirla(VeriErisim ve)
        {
            string sql = @"
IF OBJECT_ID(N'" + ARSIV + @"', N'U') IS NULL
    SELECT TOP 0 " + ALANLAR + @" INTO " + ARSIV + @" FROM " + TABLO + @";
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_" + TABLO + @"_ISLEMTARIHI' AND object_id = OBJECT_ID(N'" + TABLO + @"'))
    CREATE INDEX IX_" + TABLO + @"_ISLEMTARIHI ON " + TABLO + @" (ISLEMTARIHI);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_" + ARSIV + @"_ISLEMTARIHI' AND object_id = OBJECT_ID(N'" + ARSIV + @"'))
    CREATE INDEX IX_" + ARSIV + @"_ISLEMTARIHI ON " + ARSIV + @" (ISLEMTARIHI);";
            ve.hata = "";
            ve.ExecuteNonquery(sql, UZUN_SURE);
            return new Sonuc(ve.hata ?? "");
        }

        /// <summary>
        /// ISLEMTARIHI, bugünden saklamaYil yıl öncesinden eski olan kayıtları partiBoyut'luk partilerle arşive taşır.
        /// Aynı anda tek çalışma yapılır; çalışma sürerken gelen çağrı hemen döner.
        /// </summary>
        /// <param name="saklamaYil">Ana tabloda tutulacak süre (yıl). 1'den küçükse 2 alınır.</param>
        /// <param name="partiBoyut">Bir partide taşınacak satır sayısı (100 - 50000, varsayılan 5000).</param>
        /// <returns>Başarılıysa bilgi alanında taşınan satır sayısı yazar.</returns>
        public static Sonuc Tasi(int saklamaYil, int partiBoyut)
        {
            lock (kilit)
            {
                if (calisiyor) return new Sonuc(true, "", "Arşive taşıma zaten çalışıyor.");
                calisiyor = true;
            }

            try
            {
                if (saklamaYil < 1) saklamaYil = 2;
                if (partiBoyut <= 0) partiBoyut = 5000;
                partiBoyut = Math.Max(100, Math.Min(50000, partiBoyut));

                VeriErisim ve = VeriErisim.VeriErisimBul(null, BAGLANTI, null);
                try
                {
                    if (ve.baglantiTip == "ORACLE")
                        return new Sonuc("Arşive taşıma yalnızca SQL Server veri tabanında yapılır.");

                    Sonuc s = Hazirla(ve);
                    if (!s.islemSonuc) return s;

                    DateTime sinir = DateTime.Today.AddYears(-saklamaYil);
                    long toplam = 0;
                    while (true)
                    {
                        ve.hata = "";
                        ve.param.Add(new VeriPara("@PARTI", DbType.Int32, partiBoyut));
                        ve.param.Add(new VeriPara("@SINIR", DbType.DateTime, sinir));
                        int adet = ve.ExecuteNonquery(
                            "DELETE TOP (@PARTI) FROM " + TABLO +
                            " OUTPUT DELETED.KULLANICIKODU, DELETED.ISLEMTARIHI, DELETED.ISLEMYERI, DELETED.SAYFAADI, DELETED.BILGI1, DELETED.BILGI2, DELETED.BILGI3, DELETED.BILGI4, DELETED.BILGI5" +
                            " INTO " + ARSIV + " (" + ALANLAR + ")" +
                            " WHERE ISLEMTARIHI < @SINIR", UZUN_SURE);

                        if (!string.IsNullOrEmpty(ve.hata))
                            return new Sonuc(false, ve.hata, "Taşınan: " + toplam);
                        if (adet > 0) toplam += adet;
                        if (adet < partiBoyut) break;

                        // Partiler arasında kısa ara: sayfa isteklerinin yazılması (INSERT) uzun süre beklemesin.
                        Thread.Sleep(200);
                    }

                    return new Sonuc(true, "", toplam.ToString());
                }
                finally
                {
                    ve.VeriErisimKapat();
                }
            }
            finally
            {
                lock (kilit) calisiyor = false;
            }
        }

    }
}
