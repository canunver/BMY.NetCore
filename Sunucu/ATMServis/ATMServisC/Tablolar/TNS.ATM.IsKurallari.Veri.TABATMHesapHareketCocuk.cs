using System;
using System.Data;
using TNS.Veri;

namespace TNS.ATM.IsKurallari.Veri
{
    /// <summary>
    /// Dosya hesap bilgilerini tutan ATMHESAPHAREKETCOCUK veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABATMHesapHareketCocuk : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABATMHesapHareketCocuk(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "ATMHESAPHAREKETCOCUK";

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
            /// ANAKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ANAKOD = 1,
            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,
            /// <summary>
            /// HESAPKODU numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPKODU = 3,
            /// <summary>
            /// ADI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ADI = 4,
            /// <summary>
            /// BORCALACAK kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORC = 5,
            /// <summary>
            /// ALACAK kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALACAK = 6,
            /// <summary>
            /// ISLEMYAPANAD doğuş tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPANAD = 5,
            /// <summary>
            /// ISLEMYAPANKOD adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPANKOD = 6,
            /// <summary>
            /// ISLEMTARIHI adres alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIHI = 7,
            URETILENSATIR = 8

        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "ANAKOD", "SIRANO", "HESAPKODU", "ADI", "BORC", "ALACAK", "ISLEMYAPANAD",
                                    "ISLEMYAPANKOD", "ISLEMTARIHI","URETILENSATIR" };
        /// <summary>
        /// bilgileri veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapHareketCocuk">The hesap hareket cocuk.</param>
        /// <returns>
        /// Ekleme komutunun çalıştırılması sonucunda Sonuc yapısında nesne döndürür
        /// </returns>
        public Sonuc Kaydet(DosyaHesapHareketCocuk hesapHareketCocuk)
        {
            int kod = TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], null, null, null, null);
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(kod) + ",";
            sql += VeriErisim.DegerYap(hesapHareketCocuk.anaKod) + ",";
            sql += VeriErisim.DegerYap(hesapHareketCocuk.siraNo) + ",";
            sql += VeriErisim.DegerYap(hesapHareketCocuk.hesapKodu) + ",";
            sql += VeriErisim.DegerYap(hesapHareketCocuk.adi) + ",";
            sql += VeriErisim.DegerYap(hesapHareketCocuk.borc) + ",";
            sql += VeriErisim.DegerYap(hesapHareketCocuk.alacak) + ",";
            sql += VeriErisim.DegerYap(hesapHareketCocuk.islemYapanAd) + ",";
            sql += VeriErisim.DegerYap(hesapHareketCocuk.islemYapanKod) + ",";
            sql += VeriErisim.DegerYap(hesapHareketCocuk.islemTarihi) + ",";
            sql += VeriErisim.DegerYap(hesapHareketCocuk.uretilenSatir) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        /// <summary>
        /// Dosya hesap hareket tasarım tipinde bulunan alacak takip bilgilerini veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapHareketCocuk">hesapHareketCocuk türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda Sonuc yapısında nesne döndürür</returns>
        public Sonuc Guncelle(DosyaHesapHareketCocuk hesapHareketCocuk)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], hesapHareketCocuk.siraNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAPKODU], hesapHareketCocuk.hesapKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], hesapHareketCocuk.adi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BORC], hesapHareketCocuk.borc) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALACAK], hesapHareketCocuk.alacak) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALACAK], hesapHareketCocuk.uretilenSatir) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], hesapHareketCocuk.islemYapanAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKOD], hesapHareketCocuk.islemYapanKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], hesapHareketCocuk.islemTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hesapHareketCocuk.kod));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = hesapHareketCocuk.kod.ToString();

            return sonuc;
        }

        /// <summary>
        /// Dosya hesap hareket cocuk yapısında duran anahtar alanları kullanarak, ilgili kayıtları veri tabanından listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapHareketCocuk">The hesap hareket cocuk.</param>
        /// <returns>
        /// Select komutunun çalıştırılması sonucu IDataReader yapısında nesne döndürür
        /// </returns>
        public IDataReader Listele(DosyaHesapHareketCocuk hesapHareketCocuk)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " ,ATMDOSYA.KIMLIKNO,ATMDOSYA.YIL,ATMDOSYA.SAYMANLIK,ATMDOSYA.BIRIMKODU,ATMDOSYA.DOSYANO,ATMDOSYA.BORCLUAD,ATMHESAPHAREKETANA.TASARIMKOD";
            sql += " FROM " + TABLOAD;
            sql += " INNER JOIN ATMHESAPHAREKETANA ON ATMHESAPHAREKETCOCUK.ANAKOD=ATMHESAPHAREKETANA.KOD ";
            sql += " INNER JOIN ATMDOSYA ON ATMDOSYA.YIL=ATMHESAPHAREKETANA.YIL AND ATMDOSYA.DOSYANO=ATMHESAPHAREKETANA.DOSYANO ";
            string kosul = "";

            if (hesapHareketCocuk.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", hesapHareketCocuk.kod));
            if (hesapHareketCocuk.anaKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANAKOD], "=", hesapHareketCocuk.anaKod));
            if (hesapHareketCocuk.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", hesapHareketCocuk.siraNo));
            if (!string.IsNullOrEmpty(hesapHareketCocuk.hesapKodu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HESAPKODU], "=", hesapHareketCocuk.hesapKodu));
            if (!string.IsNullOrEmpty(hesapHareketCocuk.adi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ADI], "=", hesapHareketCocuk.adi));
            if (hesapHareketCocuk.borc != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BORC], "=", hesapHareketCocuk.borc));
            if (hesapHareketCocuk.alacak != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALACAK], "=", hesapHareketCocuk.alacak));
            if (!string.IsNullOrEmpty(hesapHareketCocuk.anaKodList))
            {
                if (kosul != "")
                    kosul += " AND";
                kosul += " (" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ANAKOD] + " IN (" + hesapHareketCocuk.anaKodList.Trim(',') + ")) ";
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Dosya hesap hareket cocuk bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek taşınır belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(DosyaHesapHareketCocuk bilgi)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            if (bilgi.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", (bilgi.kod)));
            if (bilgi.anaKod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], "=", (bilgi.anaKod)));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Kod the no bul.
        /// </summary>
        /// <param name="kod">The kod.</param>
        /// <returns></returns>
        public int KodBul(int kod)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;
            if (kod != -1)
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
                return (int)veriErisim.TekKolonSQLDbl(sql) + 1;
            }
            else
                return (int)veriErisim.TekKolonSQLDbl(sql) + 1;
        }
    }
}
