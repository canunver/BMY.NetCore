using System;
using System.Data;
using TNS.Veri;

namespace TNS.ATM.IsKurallari.Veri
{
    /// <summary>
    /// Dosya hesap bilgilerini tutan ATMHESAPHAREKETTASARIMCOCUK veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABATMHesapHareketTasarim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABATMHesapHareketTasarim(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "ATMHESAPHAREKETTASARIM";

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
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,
            /// <summary>
            /// HESAPKODU numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPKODU = 2,
            /// <summary>
            /// YANSITMAHESAPKODU numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YANSITMAHESAPKODU = 3,
            /// <summary>
            /// KDVHESAPKODU numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KDVHESAPKODU = 4,
            /// <summary>
            /// ALTHESAP numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTHESAP = 5,
            /// <summary>
            /// BELGESATIRNO numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BELGESATIRNO = 6,
            /// <summary>
            /// ORAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORAN = 7,
            /// <summary>
            /// ADI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ADI = 8,
            /// <summary>
            /// BORCALACAK kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORCALACAK = 9,
            /// <summary>
            /// ISLEMYAPANAD doğuş tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPANAD = 10,
            /// <summary>
            /// ISLEMYAPANKOD adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPANKOD = 11,
            /// <summary>
            /// ISLEMTARIHI adres alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIHI = 12,
            /// <summary>
            /// ANAKOD adres alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ANAKOD = 13,
            ALANADI = 14,
            FORMUL = 15

        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "SIRANO", "HESAPKODU", "YANSITMAHESAPKODU","KDVHESAPKODU","ALTHESAP","BELGESATIRNO","ORAN","ADI", "BORCALACAK", "ISLEMYAPANAD",
                                    "ISLEMYAPANKOD", "ISLEMTARIHI", "ANAKOD","ALANADI","FORMUL" };
        /// <summary>
        /// bilgileri veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapHareketTasarim">The hesap hareket tasarim.</param>
        /// <returns>
        /// Ekleme komutunun çalıştırılması sonucunda Sonuc yapısında nesne döndürür
        /// </returns>
        public Sonuc Kaydet(DosyaHesapHareketTasarim hesapHareketTasarim)
        {
            // int kod = TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], null, null, null, null);

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(hesapHareketTasarim.kod) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarim.siraNo) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarim.hesapKodu) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarim.yansitmaHesapKodu) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarim.kdvHesapKodu) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarim.altHesap) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarim.belgeSatirNo) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarim.oran) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarim.adi) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarim.borcAlacak) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarim.islemYapanAd) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarim.islemYapanKod) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarim.islemTarihi) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarim.anaKod) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarim.alanAdi) + ",";
            sql += VeriErisim.DegerYap(hesapHareketTasarim.formul) + ")";
            Sonuc sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        /// <summary>
        /// Dosya hesap hareket tasarım tipinde bulunan alacak takip bilgilerini veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapHareketTasarim">The hesap hareket tasarim.</param>
        /// <returns>
        /// Güncelleme komutunun çalıştırılması sonucunda Sonuc yapısında nesne döndürür
        /// </returns>
        public Sonuc Guncelle(DosyaHesapHareketTasarim hesapHareketTasarim)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], hesapHareketTasarim.siraNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAPKODU], hesapHareketTasarim.hesapKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YANSITMAHESAPKODU], hesapHareketTasarim.yansitmaHesapKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVHESAPKODU], hesapHareketTasarim.kdvHesapKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTHESAP], hesapHareketTasarim.altHesap) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGESATIRNO], hesapHareketTasarim.belgeSatirNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORAN], hesapHareketTasarim.oran) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], hesapHareketTasarim.adi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BORCALACAK], hesapHareketTasarim.borcAlacak) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], hesapHareketTasarim.islemYapanAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKOD], hesapHareketTasarim.islemYapanKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], hesapHareketTasarim.islemTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALANADI], hesapHareketTasarim.alanAdi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FORMUL], hesapHareketTasarim.formul) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], hesapHareketTasarim.anaKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hesapHareketTasarim.kod));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = hesapHareketTasarim.kod.ToString();

            return sonuc;
        }

        /// <summary>
        /// Dosya hesap hareket tasarım yapısında duran anahtar alanları kullanarak, ilgili kayıtları veri tabanından listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapHareketTasarim">The hesap hareket tasarim.</param>
        /// <returns>
        /// Select komutunun çalıştırılması sonucu IDataReader yapısında nesne döndürür
        /// </returns>
        public IDataReader Listele(DosyaHesapHareketTasarim hesapHareketTasarim)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            string kosul = "";

            if (hesapHareketTasarim.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", hesapHareketTasarim.kod));
            if (hesapHareketTasarim.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", hesapHareketTasarim.siraNo));
            if (hesapHareketTasarim.anaKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANAKOD], "=", hesapHareketTasarim.anaKod));
            if (!string.IsNullOrEmpty(hesapHareketTasarim.hesapKodu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HESAPKODU], "=", hesapHareketTasarim.hesapKodu));
            if (!string.IsNullOrEmpty(hesapHareketTasarim.adi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ADI], "=", hesapHareketTasarim.adi));
            if (hesapHareketTasarim.borcAlacak != -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BORCALACAK], "=", hesapHareketTasarim.borcAlacak));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY SIRANO";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Dosya hesap hareket tasarım bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <returns>
        /// Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Sil(DosyaHesapHareketTasarim tasarim)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], "=", (tasarim.anaKod)));
            return KomutCalistir(sql, 0);
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
