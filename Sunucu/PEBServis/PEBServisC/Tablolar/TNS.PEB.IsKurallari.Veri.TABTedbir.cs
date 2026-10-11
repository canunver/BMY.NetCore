using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Birimlerin yaptıkları işlerle ilgili aldıkları tedbirlere ait bilgileri tutan
    /// PEBTEDBIR, PEBTEDBIRDETAY ve PEBTEDBIRDETAYMEVCUTDURUM veri tabanı tabloları
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABTedbir : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTedbir(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tedbirin üst bilgilerinin tutulduğu tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADTedbir = "PEBTEDBIR";

        /// <summary>
        /// Tedbirin üst bilgilerinin tutulduğu tablonun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarTedbir : int
        {
            /// <summary>
            /// YIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// BASLANGICTARIH alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            BASLANGICTARIH = 2
        }

        /// <summary>
        /// Tedbirin üst bilgilerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARTedbir = { "YIL", "KURKOD", "BASLANGICTARIH" };

        /// <summary>
        /// Tedbirin detay bilgilerinin tutulduğu tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADTedbirDetay = "PEBTEDBIRDETAY";

        /// <summary>
        /// Tedbirin detay bilgilerinin tutulduğu tablonun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarTedbirDetay : int
        {
            /// <summary>
            /// YIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// SIRANO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,

            /// <summary>
            /// ANAEKSEN alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ANAEKSEN = 3,

            /// <summary>
            /// ALTEKSEN alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ALTEKSEN = 4,

            /// <summary>
            /// ONCELIKTEDBIR alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ONCELIKTEDBIR = 5,

            /// <summary>
            /// ISBIRLIKCIIDARE alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ISBIRLIKCIIDARE = 6,

            /// <summary>
            /// SURE alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SURE = 7,

            /// <summary>
            /// ISLEMACIKLAMA alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ISLEMACIKLAMA = 8,

            /// <summary>
            /// BASLANGICDURUM alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            BASLANGICDURUM = 9
        }

        /// <summary>
        /// Tedbirin detay bilgilerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARTedbirDetay = { "YIL", "KURKOD", "SIRANO", "ANAEKSEN", "ALTEKSEN", "ONCELIKTEDBIR",
                                              "ISBIRLIKCIIDARE", "SURE", "ISLEMACIKLAMA", "BASLANGICDURUM" };

        /// <summary>
        /// Tedbirin mevcut durum bilgilerinin tutulduğu tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADTedbirDetayMevcut = "PEBTEDBIRDETAYMEVCUTDURUM";

        /// <summary>
        /// Tedbirin mevcut durum bilgilerinin tutulduğu tablonun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarTedbirDetayMevcut : int
        {
            /// <summary>
            /// YIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// SIRANO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,

            /// <summary>
            /// MEVCUTTARIH alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            MEVCUTTARIH = 3,

            /// <summary>
            /// MEVCUTDURUM alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            MEVCUTDURUM = 4,

            /// <summary>
            /// YETKILIGORUS alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YETKILIGORUS = 5
        }

        /// <summary>
        /// Tedbirin mevcut durum bilgilerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARTedbirDetayMevcut = { "YIL", "KURKOD", "SIRANO", "MEVCUTTARIH", "MEVCUTDURUM", "YETKILIGORUS" };

        /// <summary>
        /// Tedbir üst bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="tedbir">Tedbir üst bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KaydetTedbir(Tedbir tedbir)
        {
            string sql = "INSERT INTO " + TABLOADTedbir + " (" + VeriErisim.TumAlanlar("", ALANLARTedbir) + ") VALUES (";
            sql += VeriErisim.DegerYap(tedbir.yil) + ",";
            sql += VeriErisim.DegerYap(tedbir.kurKod) + ",";
            sql += VeriErisim.DegerYap(tedbir.baslangicTarih) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan tedbir üst bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="tedbir">Tedbir üst bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc GuncelleTedbir(Tedbir tedbir)
        {
            string sql = "UPDATE " + TABLOADTedbir + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARTedbir[(int)ENUMAlanlarTedbir.BASLANGICTARIH], tedbir.baslangicTarih);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTedbir[(int)ENUMAlanlarTedbir.YIL], "=", tedbir.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTedbir[(int)ENUMAlanlarTedbir.KURKOD], "=", tedbir.kurKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Tedbir detay bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="td">Tedbir detay bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KaydetTedbirDetay(TedbirDetay td)
        {
            string sql = "INSERT INTO " + TABLOADTedbirDetay + " (" + VeriErisim.TumAlanlar("", ALANLARTedbirDetay) + ") VALUES (";
            sql += VeriErisim.DegerYap(td.yil) + ",";
            sql += VeriErisim.DegerYap(td.kurKod) + ",";
            sql += VeriErisim.DegerYap(td.siraNo) + ",";
            sql += VeriErisim.DegerYap(td.anaEksen) + ",";
            sql += VeriErisim.DegerYap(td.altEksen) + ",";
            sql += VeriErisim.DegerYap(td.oncelikTedbir) + ",";
            sql += VeriErisim.DegerYap(td.isbirlikciIdare) + ",";
            sql += VeriErisim.DegerYap(td.sure) + ",";
            sql += VeriErisim.DegerYap(td.islemAciklama) + ",";
            sql += VeriErisim.DegerYap(td.baslangicDurum) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan tedbir detay bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="td">Tedbir detay bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc GuncelleTedbirDetay(TedbirDetay td)
        {
            string sql = "UPDATE " + TABLOADTedbirDetay + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.ANAEKSEN], td.anaEksen) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.ALTEKSEN], td.altEksen) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.ONCELIKTEDBIR], td.oncelikTedbir) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.ISBIRLIKCIIDARE], td.isbirlikciIdare) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.SURE], td.sure) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.ISLEMACIKLAMA], td.islemAciklama) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.BASLANGICDURUM], td.baslangicDurum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.YIL], "=", td.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.KURKOD], "=", td.kurKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.SIRANO], "=", td.siraNo));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen tedbir detay bilgilerini kullanarak,
        /// ilgili tedbirin detay bilgilerini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="td">Tedbir detay bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc SilTedbirDetay(TedbirDetay td)
        {
            string sql = "DELETE FROM " + TABLOADTedbirDetay;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.YIL], "=", td.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.KURKOD], "=", td.kurKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.SIRANO], ">", td.siraNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Tedbir mevcut durum bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="td">Tedbir mevcut durum bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KaydetTedbirDetayMevcut(TedbirDetay td)
        {
            string sql = "INSERT INTO " + TABLOADTedbirDetayMevcut + " (" + VeriErisim.TumAlanlar("", ALANLARTedbirDetayMevcut) + ") VALUES (";
            sql += VeriErisim.DegerYap(td.yil) + ",";
            sql += VeriErisim.DegerYap(td.kurKod) + ",";
            sql += VeriErisim.DegerYap(td.siraNo) + ",";
            sql += VeriErisim.DegerYap(td.mevcutTarih) + ",";
            sql += VeriErisim.DegerYap(td.mevcutDurum) + ",";
            sql += VeriErisim.DegerYap(td.yetkiliGorusu) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilenleri kullanarak,
        /// ilgili tedbirin mevcut durum bilgilerini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="td">Tedbir mevcut durum bilgilerini tutan nesne</param>
        /// <param name="detayinTumu">Tedbire ait tüm mevcut durum bilgilerinin mi yoksa sadece ilgili tarihtekilerinin mi silineceği bilgisi</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc SilTedbirDetayMevcut(TedbirDetay td, bool detayinTumu)
        {
            string sql = "DELETE FROM " + TABLOADTedbirDetayMevcut;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTedbirDetayMevcut[(int)ENUMAlanlarTedbirDetayMevcut.YIL], "=", td.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTedbirDetayMevcut[(int)ENUMAlanlarTedbirDetayMevcut.KURKOD], "=", td.kurKod));
            if (detayinTumu)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTedbirDetayMevcut[(int)ENUMAlanlarTedbirDetayMevcut.SIRANO], ">", td.siraNo));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTedbirDetayMevcut[(int)ENUMAlanlarTedbirDetayMevcut.MEVCUTTARIH], "=", td.mevcutTarih));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen yılı ve birim kodunu kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve tedbirlere ait
        /// mevcut durum tarihlerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Verilen kriterlere uygun olan mevcut durum tarihlerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader ListeleMevcutTarih(int yil, int kurKod)
        {
            string sql = "SELECT DISTINCT " + ALANLARTedbirDetayMevcut[(int)ENUMAlanlarTedbirDetayMevcut.MEVCUTTARIH] + " FROM " + TABLOADTedbirDetayMevcut;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTedbirDetayMevcut[(int)ENUMAlanlarTedbirDetayMevcut.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTedbirDetayMevcut[(int)ENUMAlanlarTedbirDetayMevcut.KURKOD], "=", kurKod));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen tedbir üst bilgilerini kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve tedbirlere ait
        /// üst ve detay bilgileri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="tedbir">Tedbir üst bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan tedbir üst ve detay bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader ListeleTedbir(Tedbir tedbir)
        {
            string sql = "SELECT " + TABLOADTedbir + "." + ALANLARTedbir[(int)ENUMAlanlarTedbir.BASLANGICTARIH];
            sql += ", " + TABLOADTedbirDetay + "." + ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.SIRANO];
            sql += ", " + TABLOADTedbirDetay + "." + ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.ANAEKSEN];
            sql += ", " + TABLOADTedbirDetay + "." + ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.ALTEKSEN];
            sql += ", " + TABLOADTedbirDetay + "." + ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.ONCELIKTEDBIR];
            sql += ", " + TABLOADTedbirDetay + "." + ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.ISBIRLIKCIIDARE];
            sql += ", " + TABLOADTedbirDetay + "." + ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.SURE];
            sql += ", " + TABLOADTedbirDetay + "." + ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.ISLEMACIKLAMA];
            sql += ", " + TABLOADTedbirDetay + "." + ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.BASLANGICDURUM];
            sql += " FROM " + TABLOADTedbir + ", " + TABLOADTedbirDetay;
            sql += " WHERE " + TABLOADTedbir + "." + ALANLARTedbir[(int)ENUMAlanlarTedbir.YIL] + "=" + TABLOADTedbirDetay + "." + ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.YIL];
            sql += " AND " + TABLOADTedbir + "." + ALANLARTedbir[(int)ENUMAlanlarTedbir.KURKOD] + "=" + TABLOADTedbirDetay + "." + ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.KURKOD];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTedbir, ALANLARTedbir[(int)ENUMAlanlarTedbir.YIL], "=", tedbir.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTedbir, ALANLARTedbir[(int)ENUMAlanlarTedbir.KURKOD], "=", tedbir.kurKod));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen tedbir üst bilgilerini kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve tedbirlere ait
        /// ilgili tarihteki mevcut durum bilgileri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="tedbir">Tedbir üst bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan tedbir mevcut durum bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader ListeleMevcutDurum(Tedbir tedbir)
        {
            string sql = "SELECT " + TABLOADTedbirDetayMevcut + "." + ALANLARTedbirDetayMevcut[(int)ENUMAlanlarTedbirDetayMevcut.SIRANO];
            sql += ", " + TABLOADTedbirDetayMevcut + "." + ALANLARTedbirDetayMevcut[(int)ENUMAlanlarTedbirDetayMevcut.MEVCUTDURUM];
            sql += ", " + TABLOADTedbirDetayMevcut + "." + ALANLARTedbirDetayMevcut[(int)ENUMAlanlarTedbirDetayMevcut.YETKILIGORUS];
            sql += " FROM " + TABLOADTedbirDetayMevcut;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADTedbirDetayMevcut, ALANLARTedbirDetayMevcut[(int)ENUMAlanlarTedbirDetayMevcut.YIL], "=", tedbir.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTedbirDetayMevcut, ALANLARTedbirDetayMevcut[(int)ENUMAlanlarTedbirDetayMevcut.KURKOD], "=", tedbir.kurKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTedbirDetayMevcut, ALANLARTedbirDetayMevcut[(int)ENUMAlanlarTedbirDetayMevcut.MEVCUTTARIH], "=", tedbir.mevcutTarih));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Select sql cümlesini oluşturup, çalıştıran ve tedbir
        /// yapabilen birimleri DataReader yapısında döndüren yordam
        /// </summary>
        /// <returns>Tedbir yapabilen birimleri tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader ListeleTedbirBirim()
        {
            string sql = "SELECT * FROM PEBTEDBIRKUR";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen tedbir üst bilgilerini kullanarak,
        /// ilgili tertibin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="tedbir">Tedbir üst bilgilerini tutan nesne</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool TedbirVarMi(Tedbir tedbir)
        {
            string[] intAlan = { ALANLARTedbir[(int)ENUMAlanlarTedbir.YIL], ALANLARTedbir[(int)ENUMAlanlarTedbir.KURKOD] };
            int[] intDeger = { tedbir.yil, tedbir.kurKod };
            return TABGenel.KayitVarmi(veriErisim, TABLOADTedbir, new string[0], new string[0], intAlan, intDeger);
        }

        /// <summary>
        /// Parametre olarak verilen tedbir detay bilgilerini kullanarak, ilgili
        /// tertip detayın veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="td">Tedbir detay bilgilerini tutan nesne</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool TedbirDetayVarMi(TedbirDetay td)
        {
            string[] intAlan = { ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.YIL], ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.KURKOD], ALANLARTedbirDetay[(int)ENUMAlanlarTedbirDetay.SIRANO] };
            int[] intDeger = { td.yil, td.kurKod, td.siraNo };
            return TABGenel.KayitVarmi(veriErisim, TABLOADTedbirDetay, new string[0], new string[0], intAlan, intDeger);
        }
    }
}