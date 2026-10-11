using System;
using System.Data;
using TNS.Veri;

namespace TNS.ATM.IsKurallari.Veri
{
    /// <summary>
    /// Dosya hareket bilgilerini tutan ATMHESAPHAREKETANA veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABATMHesapHareketAna : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABATMHesapHareketAna(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "ATMHESAPHAREKETANA";

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
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 1,
            /// <summary>
            /// Dosya numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DOSYANO = 2,
            /// <summary>
            /// TARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TASARIMKOD = 3,
            /// <summary>
            /// TARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TARIH = 4,
            /// <summary>
            /// ODEMEEMRINO  alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 5,
            /// <summary>
            /// TOPLAMTUTAR  alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TOPLAMTUTAR = 6,
            /// <summary>
            /// ISLEMYAPANAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPANAD = 7,
            /// <summary>
            /// ISLEMYAPANKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPANKOD = 8,
            /// <summary>
            /// ISLEMTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIHI = 9
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "YIL", "DOSYANO", "TASARIMKOD", "TARIH"
                                  , "ODEMEEMRINO", "TOPLAMTUTAR", "ISLEMYAPANAD",
                                    "ISLEMYAPANKOD", "ISLEMTARIHI" };
        /// <summary>
        /// bilgileri veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapHareketAna">Alacak takip bilgilerini tutan hesapHareketAna türünde nesne</param>
        /// <returns>
        /// Ekleme komutunun çalıştırılması sonucunda Sonuc yapısında nesne döndürür
        /// </returns>
        public Sonuc Kaydet(DosyaHesapHareketAna hesapHareketAna)
        {
            int kod = TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], null, null, null, null);

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(kod) + ",";
            sql += VeriErisim.DegerYap(hesapHareketAna.yil) + ",";
            sql += VeriErisim.DegerYap(hesapHareketAna.dosyaNo) + ",";
            sql += VeriErisim.DegerYap(hesapHareketAna.tasarimKod) + ",";
            sql += VeriErisim.DegerYap(hesapHareketAna.tarih) + ",";
            sql += VeriErisim.DegerYap(hesapHareketAna.odemeEmriNo) + ",";
            sql += VeriErisim.DegerYap(hesapHareketAna.toplamTutar) + ",";
            sql += VeriErisim.DegerYap(hesapHareketAna.islemYapanAd) + ",";
            sql += VeriErisim.DegerYap(hesapHareketAna.islemYapanKod) + ",";
            sql += VeriErisim.DegerYap(hesapHareketAna.islemTarihi) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = kod.ToString();

            return sonuc;
        }

        /// <summary>
        /// DosyaHesapHareketAna tipinde bulunan alacak takip bilgilerini veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapHareketAna">The hesap hareket ana.</param>
        /// <returns>
        /// Güncelleme komutunun çalıştırılması sonucunda Sonuc yapısında nesne döndürür
        /// </returns>
        public Sonuc Guncelle(DosyaHesapHareketAna hesapHareketAna)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASARIMKOD], hesapHareketAna.tasarimKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH], hesapHareketAna.tarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TOPLAMTUTAR], hesapHareketAna.toplamTutar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], hesapHareketAna.islemYapanAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKOD], hesapHareketAna.islemYapanKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], hesapHareketAna.islemTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hesapHareketAna.kod));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = hesapHareketAna.kod.ToString();

            return sonuc;
        }

        /// <summary>
        /// Dosya yapısında duran anahtar alanları kullanarak, ilgili kayıtları veri tabanından listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapHareketAna">The hesap hareket ana.</param>
        /// <returns>
        /// Select komutunun çalıştırılması sonucu IDataReader yapısında nesne döndürür
        /// </returns>
        public IDataReader Listele(DosyaHesapHareketAna hesapHareketAna)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",ATMHESAPHAREKETTASARIMANA.AD FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN ATMHESAPHAREKETTASARIMANA ON ATMHESAPHAREKETANA.TASARIMKOD = ATMHESAPHAREKETTASARIMANA.KOD";

            if (hesapHareketAna.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", hesapHareketAna.yil));

            if (hesapHareketAna.dosyaNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", hesapHareketAna.dosyaNo));

            if (!hesapHareketAna.tarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TARIH], "=", hesapHareketAna.tarih));

            if (!string.IsNullOrEmpty(hesapHareketAna.odemeEmriNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], "=", hesapHareketAna.odemeEmriNo));

            if (hesapHareketAna.toplamTutar > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TOPLAMTUTAR], "=", hesapHareketAna.toplamTutar));

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DOSYANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// DosyaHesapHareketAna bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek taşınır belge bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Sil(DosyaHesapHareketAna bilgi)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", (bilgi.kod)));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Dosya yapısında duran anahtar alanları kullanarak, ilgili kayıtları veri tabanından listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapHareketAna">The hesap hareket ana.</param>
        /// <returns>
        /// Select komutunun çalıştırılması sonucu IDataReader yapısında nesne döndürür
        /// </returns>
        public IDataReader ListeleDosya(DosyaHesapHareketAna hesapHareketAna)
        {
            string kosul = "";
            string sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + ",";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DOSYANO] + ",";
            sql += " ATMDOSYA.BIRIMKODU,ATMDOSYA.BORCLUAD,ATMDOSYA.SAYMANLIK, ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TOPLAMTUTAR] + ",";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO];
            sql += ",ATMHESAPHAREKETTASARIMANA.KOD,ATMHESAPHAREKETTASARIMANA.AD";
            sql += " FROM " + TABLOAD;
            sql += " INNER JOIN ";
            sql += "ATMDOSYA" + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + "ATMDOSYA.YIL" + " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DOSYANO] + " = " + "ATMDOSYA.DOSYANO";
            sql += " LEFT OUTER JOIN ATMHESAPHAREKETTASARIMANA ON ATMHESAPHAREKETANA.TASARIMKOD = ATMHESAPHAREKETTASARIMANA.KOD";

            if (hesapHareketAna.odemeEmriNo == "!!!")
                kosul = VeriErisim.KosulEkle(kosul, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], "=", ""));

            if (kosul != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DOSYANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// bilgileri veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapHareketAna">The hesap hareket ana.</param>
        /// <returns>
        /// Ekleme komutunun çalıştırılması sonucunda Sonuc yapısında nesne döndürür
        /// </returns>
        public Sonuc KaydetOdemeEmriNo(DosyaHesapHareketAna hesapHareketAna)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], hesapHareketAna.odemeEmriNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hesapHareketAna.kod));

            Sonuc sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }
    }
}
