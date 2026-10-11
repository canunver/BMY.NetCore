using System;
using System.Data;
using TNS.Veri;

namespace TNS.ATM.IsKurallari.Veri
{
    /// <summary>
    /// Dosya hesap bilgilerini tutan ATMHESAPHAREKETTASARIMCOCUK veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABATMHesapHareketTasarimAna : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABATMHesapHareketTasarimAna(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "ATMHESAPHAREKETTASARIMANA";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// AD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 1,
            /// <summary>
            /// AKTIF numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AKTIF = 2,
            /// <summary>
            /// ISLEMYAPANAD doğuş tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPANAD = 3,
            /// <summary>
            /// ISLEMYAPANKOD adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPANKOD = 4,
            /// <summary>
            /// ISLEMTARIHI adres alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIHI = 5
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "AD", "AKTIF",
                                    "ISLEMYAPANKOD", "ISLEMYAPANAD",  "ISLEMTARIHI" };
        /// <summary>
        /// bilgileri veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapHareketTasarimAna">The hesap hareket tasarim ana.</param>
        /// <returns>
        /// Ekleme komutunun çalıştırılması sonucunda Sonuc yapısında nesne döndürür
        /// </returns>
        public Sonuc Kaydet(DosyaHesapHareketTasarimAna hesapHareketTasarimAna)
        {
            // int kod = TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], null, null, null, null);

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(hesapHareketTasarimAna.kod) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarimAna.ad) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarimAna.aktif) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarimAna.islemYapanAd) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarimAna.islemYapanKod) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarimAna.islemTarihi) + ")";
            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = hesapHareketTasarimAna.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Dosya hesap hareket tasarım ana tipinde bulunan alacak takip bilgilerini veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapHareketTasarimAna">The hesap hareket tasarim ana.</param>
        /// <returns>
        /// Güncelleme komutunun çalıştırılması sonucunda Sonuc yapısında nesne döndürür
        /// </returns>
        public Sonuc Guncelle(DosyaHesapHareketTasarimAna hesapHareketTasarimAna)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], hesapHareketTasarimAna.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKTIF], hesapHareketTasarimAna.aktif) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], hesapHareketTasarimAna.islemYapanAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKOD], hesapHareketTasarimAna.islemYapanKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], hesapHareketTasarimAna.islemTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hesapHareketTasarimAna.kod));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = hesapHareketTasarimAna.kod.ToString();

            return sonuc;
        }

        /// <summary>
        /// Dosya hesap hareket tasarım ana yapısında duran anahtar alanları kullanarak, ilgili kayıtları veri tabanından listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="DosyaHesapHareketTasarimAna">The hesap hareket tasarim ana.</param>
        /// <returns>
        /// Select komutunun çalıştırılması sonucu IDataReader yapısında nesne döndürür
        /// </returns>
        public IDataReader Listele(DosyaHesapHareketTasarimAna hesapHareketTasarimAna)
        {

            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            string kosul = "";

            if (hesapHareketTasarimAna.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", hesapHareketTasarimAna.kod));
            if (hesapHareketTasarimAna.aktif != -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AKTIF], "=", hesapHareketTasarimAna.aktif));
            if (!string.IsNullOrEmpty(hesapHareketTasarimAna.ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], " LIKE ", "%" + hesapHareketTasarimAna.ad + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY AD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Dosya hesap hareket tasarım ana bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <returns>
        /// Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Sil(DosyaHesapHareketTasarimAna bilgi)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", (bilgi.kod)));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Kod the no bul.
        /// </summary>
        /// <returns></returns>
        public int KodBul()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;
            return (int)veriErisim.TekKolonSQLDbl(sql) + 1;
        }
    }
}
