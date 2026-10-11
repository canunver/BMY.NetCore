using System;
using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{
    public class TABHibeYatirimBasvuruTablolari : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHibeYatirimBasvuruTablolari(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADOrtaklar = "TRMHIBEYATIRIMORTAKLAR";

        /// <summary>
        /// TRMHIBEYATIRIMYONETIMKURULU bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADYonetimKurulu = "TRMHIBEYATIRIMYONETIMKURULU";
        /// <summary>
        /// TRMHIBEYATIRIMFONKAYNAK bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADYonetimFonKaynak = "TRMHIBEYATIRIMFONKAYNAK";
        /// <summary>
        /// TRMHIBEYATIRIMGIDERLERTABLOSU bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADGiderler = "TRMHIBEYATIRIMGIDERLERTABLOSU";
        /// <summary>
        /// TRMHIBEYATIRIMGIDERLERTABLOSUDETAY bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADGiderlerDetay = "TRMHIBEYATIRIMGIDERLERTABLOSUDETAY";
        /// <summary>
        /// TRMHIBEYATIRIMSUREEYLEMPLANI bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADSureEylemPlani = "TRMHIBEYATIRIMSUREEYLEMPLANI";
        /// <summary>
        /// TRMHIBEYATIRIMSUREEYLEMPLANIDETAY bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADSureEylemPlaniDetay = "TRMHIBEYATIRIMSUREEYLEMPLANIDETAY";
        /// <summary>
        /// TRMHIBEYATIRIMSUREEYLEMPLANI bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADIsTakvimi = "TRMHIBEYATIRIMISTAKVIMI";
        /// <summary>
        /// TRMHIBEYATIRIMSUREEYLEMPLANIDETAY bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADIsTakvimiDetay = "TRMHIBEYATIRIMISTAKVIMIDETAY";
        /// <summary>
        /// TRMHIBEYATIRIMPROJEBUTCESI bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADProjeButcesi = "TRMHIBEYATIRIMPROJEBUTCESI";
        /// <summary>
        /// TRMHIBEYATIRIMPROJEBUTCESIDETAY bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADProjeButcesiDetay = "TRMHIBEYATIRIMPROJEBUTCESIDETAY";
        /// <summary>
        /// TRMHIBEYATIRIMFINANSMANKAYNAGI bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADFinansmanKaynagi = "TRMHIBEYATIRIMFINANSMANKAYNAGI";
        /// <summary>
        /// TRMHIBEYATIRIMFINANSMANKAYNAGIDETAY bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADFinansmanKaynagiDetay = "TRMHIBEYATIRIMFINANSMANKAYNAGIDETAY";

        /// <summary>
        /// TRMHIBEYATIRIMYONETIMKURULU bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarYonetimKurulu
        {
            /// <summary>
            /// HIBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEKOD = 0,
            /// <summary>
            /// SATIRNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SATIRNO = 1,
            /// <summary>
            /// ADSOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADSOYAD = 2,
            /// <summary>
            /// MESLEK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MESLEK = 3,
            /// <summary>
            /// CINSIYET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CINSIYET = 4,
            /// <summary>
            /// POZISYON alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            POZISYON = 5,
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 6,
        }

        public enum ENUMAlanlarOrtaklar
        {
            HIBEKOD = 0,
            SATIRNO = 1,
            KIMLIKNO = 2,
            ADSOYAD = 3,
            YUZDE = 4
        }

        /// <summary>
        /// TRMHIBEYATIRIMFONKAYNAK bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarFonKaynak
        {
            /// <summary>
            /// HIBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEKOD = 0,
            /// <summary>
            /// SATIRNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SATIRNO = 1,
            /// <summary>
            /// FONTIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FONTIP = 2,
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 3,
            /// <summary>
            /// PROJEADIREFNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PROJEADIREFNO = 4,
            /// <summary>
            /// KURULUSPROGRAMADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURULUSPROGRAMADI = 5,
            /// <summary>
            /// TUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUTAR = 6,
            /// <summary>
            /// HIBEKREDI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEKREDI = 7,
            /// <summary>
            /// PROJEASAMASI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PROJEASAMASI = 8
        }

        /// <summary>
        /// TRMHIBEYATIRIMGIDERLERTABLOSU bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarGiderler
        {
            /// <summary>
            /// HIBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEKOD = 0,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,
            /// <summary>
            /// TIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TIP = 2,
            /// <summary>
            /// AYNI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AYNI = 3,
            /// <summary>
            /// GIDERKALEMLERI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GIDERKALEMLERI = 4,
            /// <summary>
            /// BASLANGICTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BASLANGICTARIHI = 5,
            /// <summary>
            /// BITISTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BITISTARIHI = 6,
            /// <summary>
            /// TOPLAMGIDER alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TOPLAMGIDER = 7
        }

        /// <summary>
        /// TRMHIBEYATIRIMGIDERLERTABLOSUDETAY bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarGiderlerDetay
        {
            /// <summary>
            /// HIBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEKOD = 0,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,
            /// <summary>
            /// TIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TIP = 2,
            /// <summary>
            /// AYNI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AYNI = 3,
            /// <summary>
            /// AY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AY = 4,
            /// <summary>
            /// GIDER alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GIDER = 5
        }

        /// <summary>
        /// TRMHIBEYATIRIMSUREEYLEMPLANI bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSureEylemPlani
        {
            /// <summary>
            /// HIBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEKOD = 0,
            /// <summary>
            /// SATIRNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SATIRNO = 1,
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 2,
            /// <summary>
            /// FAALIYETNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FAALIYETNO = 3,
            /// <summary>
            /// ASAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ASAMA = 4,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 5,
            /// <summary>
            /// KURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURUM = 6
        }

        /// <summary>
        /// TRMHIBEYATIRIMSUREEYLEMPLANIDETAY bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSureEylemPlaniDetay
        {
            /// <summary>
            /// HIBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEKOD = 0,
            /// <summary>
            /// SATIRNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SATIRNO = 1,
            /// <summary>
            /// AY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AY = 2,
            /// <summary>
            /// CALISMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CALISMA = 3
        }

        /// <summary>
        /// TRMHIBEYATIRIMISTAKVIMI bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarIsTakvimi
        {
            /// <summary>
            /// HIBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEKOD = 0,
            /// <summary>
            /// TIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TIP = 1,
            /// <summary>
            /// TUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUR = 2,
            /// <summary>
            /// KONU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KONU = 3
        }

        /// <summary>
        /// TRMHIBEYATIRIMISTAKVIMIDETAY bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarIsTakvimiDetay
        {
            /// <summary>
            /// HIBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEKOD = 0,
            /// <summary>
            /// TIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TIP = 1,
            /// <summary>
            /// TUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUR = 2,
            /// <summary>
            /// AY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AY = 3,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 4
        }

        /// <summary>
        /// TRMHIBEYATIRIMPROJEBUTCESI bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarProjeButcesi
        {
            /// <summary>
            /// HIBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEKOD = 0,
            /// <summary>
            /// GIDERKALEMTURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GIDERKALEMTURU = 1,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,
            /// <summary,>
            /// GIDERKALEMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GIDERKALEMI = 3,
            /// <summary>
            /// TOPLAMTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TOPLAMTUTAR = 4
        }

        /// <summary>
        /// TRMHIBEYATIRIMPROJEBUTCESIDETAY bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarProjeButcesiDetay
        {
            /// <summary>
            /// HIBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEKOD = 0,
            /// <summary>
            /// GIDERKALEMTURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GIDERKALEMTURU = 1,
            /// <summary>
            /// AY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AY = 2,
            /// <summary>
            /// TUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUTAR = 3
        }

        /// <summary>
        /// TRMHIBEYATIRIMPROJEBUTCESI bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarFinansmanKaynagi
        {
            /// <summary>
            /// HIBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEKOD = 0,
            /// <summary>
            /// GIDERKALEMTURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GIDERKALEMTURU = 1,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,
            /// <summary,>
            /// GIDERKALEMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GIDERKALEMI = 3,
            /// <summary>
            /// TOPLAMTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TOPLAMTUTAR = 4
        }

        /// <summary>
        /// TRMHIBEYATIRIMPROJEBUTCESIDETAY bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarFinansmanKaynagiDetay
        {
            /// <summary>
            /// HIBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEKOD = 0,
            /// <summary>
            /// GIDERKALEMTURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GIDERKALEMTURU = 1,
            /// <summary>
            /// KAYNAKTIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KAYNAKTIP = 2,
            /// <summary>
            /// TUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUTAR = 3,
            /// <summary>
            /// ORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ORAN = 4
        }

        /// <summary>
        /// TRMHIBEYATIRIMYONETIMKURULU tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARYonetimKurulu = { "HIBEKOD", "SATIRNO", "ADSOYAD", "MESLEK", "CINSIYET", "POZISYON", "YIL" };

        public static string[] ALANLAROrtaklar = { "HIBEKOD", "SATIRNO", "KIMLIKNO", "ADSOYAD", "YUZDE" };

        /// <summary>
        /// TRMHIBEYATIRIMFONKAYNAK tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary
        public static string[] ALANLARFonKaynak = { "HIBEKOD", "SATIRNO", "FONTIP", "YIL", "PROJEADIREFNO", "KURULUSPROGRAMADI", "TUTAR", "HIBEKREDI", "PROJEASAMASI" };

        /// <summary>
        /// TRMHIBEYATIRIMGIDERLERTABLOSU tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary
        public static string[] ALANLARGiderler = { "HIBEKOD", "SIRANO", "TIP", "AYNI", "GIDERKALEMI", "BASLANGICTARIHI", "BITISTARIHI", "TOPLAMGIDER" };

        /// <summary>
        /// TRMHIBEYATIRIMGIDERLERTABLOSUDETAY tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary
        public static string[] ALANLARGiderlerDetay = { "HIBEKOD", "SIRANO", "TIP", "AYNI", "AY", "GIDER" };

        /// <summary>
        /// TRMHIBEYATIRIMSUREEYLEMPLANI tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary
        public static string[] ALANLARSureEylemPlani = { "HIBEKOD", "SATIRNO", "YIL", "FAALIYETNO", "ASAMA", "ACIKLAMA", "KURUM" };

        /// <summary>
        /// TRMHIBEYATIRIMSUREEYLEMPLANIDETAY tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary
        public static string[] ALANLARSureEylemPlaniDetay = { "HIBEKOD", "SATIRNO", "AY", "CALISMA" };

        /// <summary>
        /// TRMHIBEYATIRIMISTAKVIMI tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary
        public static string[] ALANLARIsTakvimi = { "HIBEKOD", "TIP", "TUR", "KONU" };

        /// <summary>
        /// TRMHIBEYATIRIMISTAKVIMIDETAY tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary
        public static string[] ALANLARIsTakvimiDetay = { "HIBEKOD", "TIP", "TUR", "AY", "ACIKLAMA" };

        /// <summary>
        /// TRMHIBEYATIRIMPROJEBUTCESI tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary
        public static string[] ALANLARProjeButcesi = { "HIBEKOD", "GIDERKALEMTURU", "SIRANO", "GIDERKALEMI", "TOPLAMTUTAR" };

        /// <summary>
        /// TRMHIBEYATIRIMPROJEBUTCESIDETAY tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary
        public static string[] ALANLARProjeButcesiDetay = { "HIBEKOD", "GIDERKALEMTURU", "AY", "TUTAR" };


        /// <summary>
        /// TRMHIBEYATIRIMFINANSMANKAYNAGI tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary
        public static string[] ALANLARFinansmanKaynagi = { "HIBEKOD", "GIDERKALEMTURU", "SIRANO", "GIDERKALEMI", "TOPLAMTUTAR" };

        /// <summary>
        /// TRMHIBEYATIRIMFINANSMANKAYNAGIDETAY tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary
        public static string[] ALANLARFinansmanKaynagiDetay = { "HIBEKOD", "GIDERKALEMTURU", "KAYNAKTIP", "TUTAR", "ORAN" };

        /// <summary>
        /// Yonetim Kurulu bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetYonetimKurulu(YonetimKurulu bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADYonetimKurulu + " (" + VeriErisim.TumAlanlar("", ALANLARYonetimKurulu) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.satirNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.adSoyad) + ",";
            sql += VeriErisim.DegerYap(bilgi.meslek) + ",";
            sql += VeriErisim.DegerYap(bilgi.cinsiyet) + ",";
            sql += VeriErisim.DegerYap(bilgi.pozisyon) + ",";
            sql += VeriErisim.DegerYap(bilgi.yil) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        public Sonuc SilYonetimKurulu(int hibeKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADYonetimKurulu;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARYonetimKurulu[(int)ENUMAlanlarYonetimKurulu.HIBEKOD], "=", hibeKod));

            return KomutCalistir(sql, 0);
        }
        /// <summary>
        /// Yonetim Kurulu bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Yonetim Kurulu bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader YonetimKuruluListele(int hibeKod)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADYonetimKurulu, ALANLARYonetimKurulu) + " FROM " + TABLOADYonetimKurulu;

            if (hibeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADYonetimKurulu, ALANLARYonetimKurulu[(int)ENUMAlanlarYonetimKurulu.HIBEKOD], "=", hibeKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADYonetimKurulu + "." + ALANLARYonetimKurulu[(int)ENUMAlanlarYonetimKurulu.SATIRNO];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetOrtaklar(Ortaklar bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADOrtaklar + " (" + VeriErisim.TumAlanlar("", ALANLAROrtaklar) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.satirNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.kimlikNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.adSoyad) + ",";
            sql += VeriErisim.DegerYap(bilgi.yuzde) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        public Sonuc SilOrtaklar(int hibeKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADOrtaklar;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROrtaklar[(int)ENUMAlanlarOrtaklar.HIBEKOD], "=", hibeKod));

            return KomutCalistir(sql, 0);
        }
        public IDataReader OrtaklarListele(int hibeKod)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADOrtaklar, ALANLAROrtaklar) + " FROM " + TABLOADOrtaklar;

            if (hibeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOrtaklar, ALANLAROrtaklar[(int)ENUMAlanlarOrtaklar.HIBEKOD], "=", hibeKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADOrtaklar + "." + ALANLAROrtaklar[(int)ENUMAlanlarOrtaklar.SATIRNO];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader OrtaklarListele(Ortaklar ortaklar)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADOrtaklar, ALANLAROrtaklar) + " FROM " + TABLOADOrtaklar;
            kosul = " HIBEKOD IN (SELECT KOD FROM TRMHIBEYATIRIM WHERE ETAPNO=" + ortaklar.satirNo + " AND DURUM>0)";//SATIRNO İÇİNDE ETAPNO TAŞINIYOR

            if (ortaklar.hibeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOrtaklar, ALANLAROrtaklar[(int)ENUMAlanlarOrtaklar.HIBEKOD], "=", ortaklar.hibeKod));

            if (ortaklar.kimlikNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOrtaklar, ALANLAROrtaklar[(int)ENUMAlanlarOrtaklar.KIMLIKNO], "=", ortaklar.kimlikNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader OrtaklarHibeListele(HibeYatirim hibe)
        {
            string sql = "";

            sql = "SELECT * FROM TRMHIBEYATIRIM ";
            sql += " WHERE ";
            sql += " ETAPNO=" + hibe.etapNo;
            sql += " AND KIMLIKNO='" + hibe.kimlikNo + "'";
            sql += " AND DURUM>0";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Fon Kaynak bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetFonKaynak(YonetimFonKaynak bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADYonetimFonKaynak + " (" + VeriErisim.TumAlanlar("", ALANLARFonKaynak) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.satirNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.fonTip) + ",";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.projeAdiReferansNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.kurulusProgramAdi) + ",";
            sql += VeriErisim.DegerYap(bilgi.tutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.hibeKredi) + ",";
            sql += VeriErisim.DegerYap(bilgi.projeAsamasi) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        public Sonuc SilFonKaynak(int hibeKod, int fonTip)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADYonetimFonKaynak;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARFonKaynak[(int)ENUMAlanlarFonKaynak.HIBEKOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARFonKaynak[(int)ENUMAlanlarFonKaynak.FONTIP], "=", fonTip));

            return KomutCalistir(sql, 0);
        }
        /// <summary>
        /// Fon Kaynak bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Yonetim Kurulu bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader FonKaynakListele(int hibeKod, int fonTip)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADYonetimFonKaynak, ALANLARFonKaynak) + " FROM " + TABLOADYonetimFonKaynak;

            if (hibeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADYonetimFonKaynak, ALANLARFonKaynak[(int)ENUMAlanlarFonKaynak.HIBEKOD], "=", hibeKod));
            if (fonTip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADYonetimFonKaynak, ALANLARFonKaynak[(int)ENUMAlanlarFonKaynak.FONTIP], "=", fonTip));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADYonetimFonKaynak + "." + ALANLARFonKaynak[(int)ENUMAlanlarFonKaynak.SATIRNO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Giderler Tablosu bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetGiderler(YatirimGiderlerTablosu bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADGiderler + " (" + VeriErisim.TumAlanlar("", ALANLARGiderler) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.tip) + ",";
            sql += VeriErisim.DegerYap(bilgi.ayni) + ",";
            sql += VeriErisim.DegerYap(bilgi.giderKalemi) + ",";
            sql += VeriErisim.DegerYap(bilgi.baslangicTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.bitisTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.toplamGider) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        public Sonuc SilGiderler(YatirimGiderlerTablosu bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADGiderler;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGiderler[(int)ENUMAlanlarGiderler.HIBEKOD], "=", bilgi.hibeKod));

            if (bilgi.ayni > 0)//Ayni 1 ise tip 1 veya 2 olabilir. Eğer daha önceden tip 1 ve 2 olarak kayıt edildi ise ve şimdi 2 nolu tip silinmek isteniyor ve listede sadece 1 kaldıysa 2ler silinmediği için olanlar kalıyor
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.AYNI], "=", bilgi.ayni));
            }
            else
            {
                sql = VeriErisim.KosulEkle(sql, "AND ", VeriErisim.KosulOl("", ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.TIP], "=", bilgi.tip));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.AYNI], "=", bilgi.ayni));
            }

            return KomutCalistir(sql, 0);
        }
        public Sonuc KaydetGiderlerDetay(YatirimGiderlerTablosuDetay bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADGiderlerDetay + " (" + VeriErisim.TumAlanlar("", ALANLARGiderlerDetay) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.tip) + ",";
            sql += VeriErisim.DegerYap(bilgi.ayni) + ",";
            sql += VeriErisim.DegerYap(bilgi.ay) + ",";
            sql += VeriErisim.DegerYap(bilgi.gider) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        public Sonuc SilGiderlerDetay(YatirimGiderlerTablosuDetay bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADGiderlerDetay;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.HIBEKOD], "=", bilgi.hibeKod));

            if (bilgi.ayni > 0)//Ayni 1 ise tip 1 veya 2 olabilir. Eğer daha önceden tip 1 ve 2 olarak kayıt edildi ise ve şimdi 2 nolu tip silinmek isteniyor ve listede sadece 1 kaldıysa 2ler silinmediği için olanlar kalıyor
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.AYNI], "=", bilgi.ayni));
            }
            else
            {
                sql = VeriErisim.KosulEkle(sql, "AND ", VeriErisim.KosulOl("", ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.TIP], "=", bilgi.tip));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.AYNI], "=", bilgi.ayni));
            }


            return KomutCalistir(sql, 0);
        }
        /// <summary>
        /// Giderler Tablosu bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Giderler Tablosu bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader GiderleriListele(YatirimGiderlerTablosu bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADGiderler, ALANLARGiderler);// +", " + ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.AY] + ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.GIDER];
            sql += " FROM " + TABLOADGiderler;
            //sql += " LEFT OUTER JOIN " + TABLOADGiderlerDetay + " ON " + TABLOADGiderler + "." + ALANLARGiderler[(int)ENUMAlanlarGiderler.HIBEKOD] + "=" + TABLOADGiderlerDetay + "." + ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.HIBEKOD];
            //sql += " AND " + TABLOADGiderler + "." + ALANLARGiderler[(int)ENUMAlanlarGiderler.SIRANO] + "=" + TABLOADGiderlerDetay + "." + ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.SIRANO]; 
            //sql += " AND " + TABLOADGiderler + "." + ALANLARGiderler[(int)ENUMAlanlarGiderler.TIP] + "=" + TABLOADGiderlerDetay + "." + ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.TIP];
            //sql += " AND " + TABLOADGiderler + "." + ALANLARGiderler[(int)ENUMAlanlarGiderler.AYNI] + "=" + TABLOADGiderlerDetay + "." + ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.AYNI];

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADGiderler, ALANLARGiderler[(int)ENUMAlanlarGiderler.HIBEKOD], "=", bilgi.hibeKod));
            if (bilgi.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADGiderler, ALANLARGiderler[(int)ENUMAlanlarGiderler.TIP], "=", bilgi.tip));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADGiderler, ALANLARGiderler[(int)ENUMAlanlarGiderler.AYNI], "=", bilgi.ayni));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADGiderler + "." + ALANLARGiderler[(int)ENUMAlanlarGiderler.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader GiderleriListeleDetay(YatirimGiderlerTablosu bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADGiderlerDetay, ALANLARGiderlerDetay);
            sql += " FROM " + TABLOADGiderlerDetay;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADGiderlerDetay, ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.HIBEKOD], "=", bilgi.hibeKod));
            if (bilgi.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADGiderlerDetay, ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.TIP], "=", bilgi.tip));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADGiderlerDetay, ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.AYNI], "=", bilgi.ayni));
            if (bilgi.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADGiderlerDetay, ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.SIRANO], "=", bilgi.siraNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADGiderlerDetay + "." + ALANLARGiderlerDetay[(int)ENUMAlanlarGiderlerDetay.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Sure Eylem Plani bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetSureEylemPlani(YatirimSureEylemPlani bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADSureEylemPlani + " (" + VeriErisim.TumAlanlar("", ALANLARSureEylemPlani) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.satirNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.faaliyetNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.asama) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.kurum) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        public Sonuc SilSureEylemPlani(YatirimSureEylemPlani bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADSureEylemPlani;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSureEylemPlani[(int)ENUMAlanlarSureEylemPlani.HIBEKOD], "=", bilgi.hibeKod));

            return KomutCalistir(sql, 0);
        }
        public Sonuc KaydetSureEylemPlaniDetay(YatirimSureEylemPlaniDetay bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADSureEylemPlaniDetay + " (" + VeriErisim.TumAlanlar("", ALANLARSureEylemPlaniDetay) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.satirNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.ay) + ",";
            sql += VeriErisim.DegerYap(bilgi.calisma) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        public Sonuc SilSureEylemPlaniDetay(YatirimSureEylemPlani bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADSureEylemPlaniDetay;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSureEylemPlaniDetay[(int)ENUMAlanlarSureEylemPlaniDetay.HIBEKOD], "=", bilgi.hibeKod));

            return KomutCalistir(sql, 0);
        }
        /// <summary>
        /// Süre Eylem Planı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Süre Eylem Planı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader SureEylemPlaniListele(YatirimSureEylemPlani bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADSureEylemPlani, ALANLARSureEylemPlani);
            sql += " FROM " + TABLOADSureEylemPlani;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSureEylemPlani, ALANLARSureEylemPlani[(int)ENUMAlanlarSureEylemPlani.HIBEKOD], "=", bilgi.hibeKod));
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSureEylemPlani, ALANLARSureEylemPlani[(int)ENUMAlanlarSureEylemPlani.YIL], "=", bilgi.yil));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADSureEylemPlani + "." + ALANLARSureEylemPlani[(int)ENUMAlanlarGiderler.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader SureEylemPlaniDetayListele(YatirimSureEylemPlani bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADSureEylemPlaniDetay, ALANLARSureEylemPlaniDetay);
            sql += " FROM " + TABLOADSureEylemPlaniDetay;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSureEylemPlaniDetay, ALANLARSureEylemPlaniDetay[(int)ENUMAlanlarSureEylemPlaniDetay.HIBEKOD], "=", bilgi.hibeKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSureEylemPlaniDetay, ALANLARSureEylemPlaniDetay[(int)ENUMAlanlarSureEylemPlaniDetay.SATIRNO], "=", bilgi.satirNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADSureEylemPlaniDetay + "." + ALANLARSureEylemPlaniDetay[(int)ENUMAlanlarSureEylemPlaniDetay.SATIRNO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// İş Takvimi bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetIsTakvimi(YatirimIsTakvimi bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADIsTakvimi + " (" + VeriErisim.TumAlanlar("", ALANLARIsTakvimi) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.tip) + ",";
            sql += VeriErisim.DegerYap(bilgi.tur) + ",";
            sql += VeriErisim.DegerYap(bilgi.konu) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        public Sonuc SilIsTakvimi(YatirimIsTakvimi bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADIsTakvimi;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIsTakvimi[(int)ENUMAlanlarIsTakvimi.HIBEKOD], "=", bilgi.hibeKod));

            return KomutCalistir(sql, 0);
        }
        public Sonuc KaydetIsTakvimiDetay(YatirimIsTakvimiDetay bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADIsTakvimiDetay + " (" + VeriErisim.TumAlanlar("", ALANLARIsTakvimiDetay) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.tip) + ",";
            sql += VeriErisim.DegerYap(bilgi.tur) + ",";
            sql += VeriErisim.DegerYap(bilgi.ay) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        public Sonuc SilIsTakvimiDetay(YatirimIsTakvimi bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADIsTakvimiDetay;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIsTakvimiDetay[(int)ENUMAlanlarIsTakvimiDetay.HIBEKOD], "=", bilgi.hibeKod));

            return KomutCalistir(sql, 0);
        }
        /// <summary>
        /// İş Takvimi bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// İş Takvimi bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader IsTakvimiListele(YatirimIsTakvimi bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADIsTakvimi, ALANLARIsTakvimi);
            sql += " FROM " + TABLOADIsTakvimi;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIsTakvimi, ALANLARIsTakvimi[(int)ENUMAlanlarIsTakvimi.HIBEKOD], "=", bilgi.hibeKod));
            if (bilgi.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIsTakvimi, ALANLARIsTakvimi[(int)ENUMAlanlarIsTakvimi.TIP], "=", bilgi.tip));
            if (bilgi.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIsTakvimi, ALANLARIsTakvimi[(int)ENUMAlanlarIsTakvimi.TUR], "=", bilgi.tur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADIsTakvimi + "." + ALANLARIsTakvimi[(int)ENUMAlanlarIsTakvimi.TIP] + ", " + TABLOADIsTakvimi + "." + ALANLARIsTakvimi[(int)ENUMAlanlarIsTakvimi.TUR];

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader IsTakvimiDetayListele(YatirimIsTakvimi bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADIsTakvimiDetay, ALANLARIsTakvimiDetay);
            sql += " FROM " + TABLOADIsTakvimiDetay;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIsTakvimiDetay, ALANLARIsTakvimiDetay[(int)ENUMAlanlarIsTakvimiDetay.HIBEKOD], "=", bilgi.hibeKod));
            if (bilgi.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIsTakvimiDetay, ALANLARIsTakvimiDetay[(int)ENUMAlanlarIsTakvimiDetay.TIP], "=", bilgi.tip));
            if (bilgi.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIsTakvimiDetay, ALANLARIsTakvimiDetay[(int)ENUMAlanlarIsTakvimiDetay.TUR], "=", bilgi.tur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Proje Bütcesi bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetProjeButcesi(YatirimProjeButcesi bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADProjeButcesi + " (" + VeriErisim.TumAlanlar("", ALANLARProjeButcesi) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.giderKalemTuru) + ",";
            sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.giderKalemi) + ",";
            sql += VeriErisim.DegerYap(bilgi.toplamTutar) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        public Sonuc SilProjeButcesi(YatirimProjeButcesi bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADProjeButcesi;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARProjeButcesi[(int)ENUMAlanlarProjeButcesi.HIBEKOD], "=", bilgi.hibeKod));

            return KomutCalistir(sql, 0);
        }
        public Sonuc KaydetProjeButcesiDetay(YatirimProjeButcesiDetay bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADProjeButcesiDetay + " (" + VeriErisim.TumAlanlar("", ALANLARProjeButcesiDetay) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.giderKalemTuru) + ",";
            sql += VeriErisim.DegerYap(bilgi.ay) + ",";
            sql += VeriErisim.DegerYap(bilgi.tutar) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        public Sonuc SilProjeButcesiDetay(YatirimProjeButcesi bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADProjeButcesiDetay;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARProjeButcesiDetay[(int)ENUMAlanlarProjeButcesiDetay.HIBEKOD], "=", bilgi.hibeKod));

            return KomutCalistir(sql, 0);
        }
        /// <summary>
        /// Proje Bütcesi bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Proje Bütcesi bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ProjeButcesiListele(YatirimProjeButcesi bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADProjeButcesi, ALANLARProjeButcesi);
            sql += " FROM " + TABLOADProjeButcesi;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADProjeButcesi, ALANLARProjeButcesi[(int)ENUMAlanlarProjeButcesi.HIBEKOD], "=", bilgi.hibeKod));
            if (bilgi.giderKalemTuru > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADProjeButcesi, ALANLARProjeButcesi[(int)ENUMAlanlarProjeButcesi.GIDERKALEMTURU], "=", bilgi.giderKalemTuru));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADProjeButcesi + "." + ALANLARProjeButcesi[(int)ENUMAlanlarProjeButcesi.GIDERKALEMTURU];

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader ProjeButcesiDetayListele(YatirimProjeButcesi bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADProjeButcesiDetay, ALANLARProjeButcesiDetay);
            sql += " FROM " + TABLOADProjeButcesiDetay;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADProjeButcesiDetay, ALANLARProjeButcesiDetay[(int)ENUMAlanlarProjeButcesiDetay.HIBEKOD], "=", bilgi.hibeKod));
            if (bilgi.giderKalemTuru > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADProjeButcesiDetay, ALANLARProjeButcesiDetay[(int)ENUMAlanlarProjeButcesiDetay.GIDERKALEMTURU], "=", bilgi.giderKalemTuru));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Finansman Kaynağı bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetFinansmanKaynagi(YatirimFinansmanKaynagi bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADFinansmanKaynagi + " (" + VeriErisim.TumAlanlar("", ALANLARFinansmanKaynagi) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.giderKalemTuru) + ",";
            sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.giderKalemi) + ",";
            sql += VeriErisim.DegerYap(bilgi.toplamTutar) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        public Sonuc SilFinansmanKaynagi(YatirimFinansmanKaynagi bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADFinansmanKaynagi;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARFinansmanKaynagi[(int)ENUMAlanlarFinansmanKaynagi.HIBEKOD], "=", bilgi.hibeKod));

            return KomutCalistir(sql, 0);
        }
        public Sonuc KaydetFinansmanKaynagiDetay(YatirimFinansmanKaynagiDetay bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADFinansmanKaynagiDetay + " (" + VeriErisim.TumAlanlar("", ALANLARFinansmanKaynagiDetay) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.giderKalemTuru) + ",";
            sql += VeriErisim.DegerYap(bilgi.kaynakTipi) + ",";
            sql += VeriErisim.DegerYap(bilgi.tutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.oran) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        public Sonuc SilFinansmanKaynagiDetay(YatirimFinansmanKaynagi bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADFinansmanKaynagiDetay;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARFinansmanKaynagiDetay[(int)ENUMAlanlarFinansmanKaynagiDetay.HIBEKOD], "=", bilgi.hibeKod));

            return KomutCalistir(sql, 0);
        }
        /// <summary>
        /// Finansman Kaynağı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Proje Bütcesi bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader FinansmanKaynagiListele(YatirimFinansmanKaynagi bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADFinansmanKaynagi, ALANLARFinansmanKaynagi);
            sql += " FROM " + TABLOADFinansmanKaynagi;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADFinansmanKaynagi, ALANLARFinansmanKaynagi[(int)ENUMAlanlarFinansmanKaynagi.HIBEKOD], "=", bilgi.hibeKod));
            if (bilgi.giderKalemTuru > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADFinansmanKaynagi, ALANLARFinansmanKaynagi[(int)ENUMAlanlarFinansmanKaynagi.GIDERKALEMTURU], "=", bilgi.giderKalemTuru));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADFinansmanKaynagi + "." + ALANLARFinansmanKaynagi[(int)ENUMAlanlarFinansmanKaynagi.GIDERKALEMTURU];

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader FinansmanKaynagiDetayListele(YatirimFinansmanKaynagi bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADFinansmanKaynagiDetay, ALANLARFinansmanKaynagiDetay);
            sql += " FROM " + TABLOADFinansmanKaynagiDetay;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADFinansmanKaynagiDetay, ALANLARFinansmanKaynagiDetay[(int)ENUMAlanlarFinansmanKaynagiDetay.HIBEKOD], "=", bilgi.hibeKod));
            if (bilgi.giderKalemTuru > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADFinansmanKaynagiDetay, ALANLARFinansmanKaynagiDetay[(int)ENUMAlanlarFinansmanKaynagiDetay.GIDERKALEMTURU], "=", bilgi.giderKalemTuru));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
