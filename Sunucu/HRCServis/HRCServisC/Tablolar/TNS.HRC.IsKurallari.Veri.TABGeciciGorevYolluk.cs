using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Geçici görev yolluk bilgisini tutan PSLGECICIGOREVYOLLUKBELGE, PSLGECICIGOREVYOLLUKDETAY ve PSLGECICIGOREVYOLLUKTOPLU veri tabanı tabloları ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABGeciciGorevYolluk : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGeciciGorevYolluk(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Geçici görev yolluk tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "PSLGECICIGOREVYOLLUKBELGE";

        /// <summary>
        /// Geçici görev yolluk detay tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADDETAY = "PSLGECICIGOREVYOLLUKDETAY";

        /// <summary>
        /// Geçici görev yolluk toplu tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADTOPLU = "PSLGECICIGOREVYOLLUKTOPLU";

        /// <summary>
        /// Personel tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADPERSONEL = "HRCPERSONEL";

        /// <summary>
        /// Süreç tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADSUREC = "HRCSUREC";

        /// <summary>
        /// Geçici görev yolluk bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 3,
            /// <summary>
            /// BELGETUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGETUR = 4,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 5,
            /// <summary>
            /// KADROEK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KADROEK = 6,
            /// <summary>
            /// GUNDELIKYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GUNDELIKYIL = 7,
            /// <summary>
            /// GUNDELIKKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GUNDELIKKOD = 8,
            /// <summary>
            /// DOVUZKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOVIZKODU = 9,
            /// <summary>
            /// DOVIZKURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOVIZKURU = 10,
            /// <summary>
            /// UNVANKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UNVANKODU = 11,
            /// <summary>
            /// BILDIRIMTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BILDIRIMTARIH = 12,
            /// <summary>
            /// ODEMEEMRINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 13,
            /// <summary>
            /// EKLENECEKACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKLENECEKACIKLAMA = 14,
            /// <summary>
            /// EKLECNECEKTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKLENECEKTUTAR = 15,
            /// <summary>
            /// EKLENECEKPARABIRIMIKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKLENECEKPARABIRIMIKOD = 16,
            /// <summary>
            /// CIKARILACAKACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CIKARILACAKACIKLAMA = 17,
            /// <summary>
            /// CIKARILACAKTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CIKARILACAKTUTAR = 18,
            /// <summary>
            /// CIKARILACAKPARABIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CIKARILACAKPARABIRIMIKOD = 19,
            /// <summary>
            /// ARTTIRIMLIHESAPLA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ARTTIRIMLIHESAPLA = 20,
            /// <summary>
            /// KAYITSIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KAYITSIRANO = 21,
            /// <summary>
            /// ODEMEORANI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEORANI = 22,
            /// <summary>
            /// EMANETORANI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EMANETORANI = 23,
            /// <summary>
            /// AVANSALINDIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AVANSALINDIMI = 24,
            /// <summary>
            /// KURTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURTARIHI = 25,
            /// <summary>
            /// KONAKLAMATAMAMIODENECEKMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KONAKLAMATAMAMIODENECEKMI = 26,
            YURTICIKONAKLAMATURU = 27
        }

        /// <summary>
        /// Geçici görev yolluk detay bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarDetay
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 3,
            /// <summary>
            /// BELGETUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGETUR = 4,
            /// <summary>
            /// TCKIMLIKNOBELGE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNOBELGE = 5,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 6,
            /// <summary>
            /// YOLCULUKTIPI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YOLCULUKTIPI = 7,
            /// <summary>
            /// YOLCULUKGUNSAYISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YOLCULUKGUNSAYISI = 8,
            /// <summary>
            /// YOLCULUKGUNSAYISISTR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YOLCULUKGUNSAYISISTR = 9,
            /// <summary>
            /// NEREDEN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            NEREDEN = 10,
            /// <summary>
            /// NEREYE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            NEREYE = 11,
            /// <summary>
            /// HAREKETTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAREKETTARIH = 12,
            /// <summary>
            /// HAREKETSAAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAREKETSAAT = 13,
            /// <summary>
            /// YEVMIYETUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YEVMIYETUTAR = 14,
            /// <summary>
            /// YEVMIYEPARABIRIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YEVMIYEPARABIRIMI = 15,
            /// <summary>
            /// IKAMETTERMINALARAC alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IKAMETTERMINALARAC = 16,
            /// <summary>
            /// IKAMETTERMINALHARC alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IKAMETTERMINALHARC = 17,
            /// <summary>
            /// IKAMETTERMINALPARABIRIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IKAMETTERMINALPARABIRIMI = 18,
            /// <summary>
            /// YOLCULUKARACTIPI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YOLCULUKARACTIPI = 19,
            /// <summary>
            /// YOLCULUKHARCANAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YOCULUKHARCANAN = 20,
            /// <summary>
            /// YOLCULUKPARABIRIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YOCULUKPARABIRIMI = 21,
            /// <summary>
            /// TERMINALIKAMETARAC alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TERMINALIKAMETARAC = 22,
            /// <summary>
            /// TERMINALIKAMETHARC alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TERMINALIKMETHARC = 23,
            /// <summary>
            /// TERMINALIKAMETPARABIRIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TERMINALIKMETPARABIRIMI = 24,
            /// <summary>
            /// GOREVGUNSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GOREVGUNSAYI = 25,
            /// <summary>
            /// GUNLUKTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GUNLUKTUTAR = 26,
            /// <summary>
            /// KONAKLAMAGUNSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KONAKLANANGUNSAYI = 27,
            /// <summary>
            /// KONAKLAMAFATURA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KONAKLAMAFATURA = 28,
            /// <summary>
            /// KONAKLAMAFATURAPARABIRIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KONAKLAMAFATURAPARABIRIMI = 29,
            /// <summary>
            /// HAMALBAGAJ alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAMALBAGAJ = 30,
            /// <summary>
            /// HAMALBAGAJPARABIRIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAMALBAGAJPARABIRIMI = 31,
            /// <summary>
            /// TOPLAMKONAKLAMATUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TOPLAMKONAKLAMATUTAR = 32,
            /// <summary>
            /// TOPLAMTUTARDOVIZ alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TOPLAMTUTARDOVIZ = 33,
            /// <summary>
            /// TOPLAMTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TOPLAMTUTAR = 34,
            /// <summary>
            /// KAYITSIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KAYITSIRANO = 35,
        }

        /// <summary>
        /// Geçici görev yolluk toplu bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarToplu
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 3,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 4,
            /// <summary>
            /// BELGETUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGETUR = 5,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 6,
            /// <summary>
            /// CEKTIGIAVANS alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CEKTIGIAVANS = 7,
            /// <summary>
            /// YOLLUKTUTARI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YOLLUKTUTARI = 8,
            /// <summary>
            /// PTT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PTT = 9,
            /// <summary>
            /// DAMGAVERGISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DAMGAVERGISI = 10,
            /// <summary>
            /// ELEGECEN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ELEGECEN = 11,
            /// <summary>
            /// MAHSUPEDILEN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MAHSUPEDILEN = 12,
            /// <summary>
            /// ODENECEKTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENECEKTUTAR = 13,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 14,
            /// <summary>
            /// ODEMEEMRINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 15,
            /// <summary>
            /// GELIRVERGISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GELIRVERGISI = 16,
        }

        /// <summary>
        /// Personel tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarPersonel
        {
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 0,
            /// <summary>
            /// ADSOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADSOYAD = 1,
            /// <summary>
            /// UNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UNVAN = 2,
            /// <summary>
            /// MUHASEBEKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKODU = 3,
            /// <summary>
            /// BIRIMKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKODU = 4,
            /// <summary>
            /// BANKA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BANKA = 5,
            /// <summary>
            /// SUBE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SUBE = 6,
            /// <summary>
            /// HESAPNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HESAPNO = 7,
            /// <summary>
            /// ISTIHDAMSEKLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISTIHDAMSEKLI = 8,
        }

        /// <summary>
        /// Süreç bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarSurec
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 1,
            /// <summary>
            /// SURECDURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECDURUM = 2,
        }

        /// <summary>
        /// Geçici görev yolluk tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "BELGETUR", "TCKIMLIKNO", "KADROEK", "GUNDELIKYIL", "GUNDELIKKOD", "DOVIZKODU", "DOVIZKURU", "UNVANKODU", "BILDIRIMTARIH", "ODEMEEMRINO", "EKLENECEKACIKLAMA", "EKLENECEKTUTAR", "EKLENECEKPARABIRIMIKOD", "CIKARILACAKACIKLAMA", "CIKARILACAKTUTAR", "CIKARILACAKPARABIRIMIKOD", "ARTTIRIMLIHESAPLA", "KAYITSIRANO", "ODEMEORANI", "EMANETORANI", "AVANSALINDIMI", "KURTARIHI", "KONAKLAMATAMAMIODENECEKMI", "YURTICIKONAKLAMATURU" };

        /// <summary>
        /// Geçici görev yolluk detay tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARDETAY = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "BELGETUR", "TCKIMLIKNOBELGE", "SIRANO", "YOLCULUKTIPI", "YOLCULUKGUNSAYISI", "YOLCULUKGUNSAYISISTR", "NEREDEN", "NEREYE", "HAREKETTARIH", "HAREKETSAAT", "YEVMIYETUTAR", "YEVMIYEPARABIRIMI", "IKAMETTERMINALARAC", "IKAMETTERMINALHARC", "IKAMETTERMINALPARABIRIMI", "YOLCULUKARACTIPI", "YOCULUKHARCANAN", "YOCULUKPARABIRIMI", "TERMINALIKAMETARAC", "TERMINALIKMETHARC", "TERMINALIKMETPARABIRIMI", "GOREVGUNSAYI", "GUNLUKTUTAR", "KONAKLANANGUNSAYI", "KONAKLAMAFATURA", "KONAKLAMAFATURAPARABIRIMI", "HAMALBAGAJ", "HAMALBAGAJPARABIRIMI", "TOPLAMKONAKLAMATUTAR", "TOPLAMTUTARDOVIZ", "TOPLAMTUTAR", "KAYITSIRANO" };

        /// <summary>
        /// Geçici görev yolluk toplu tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARTOPLU = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TCKIMLIKNO", "BELGETUR", "SIRANO", "CEKTIGIAVANS", "YOLLUKTUTARI", "PTT", "DAMGAVERGISI", "ELEGECEN", "MAHSUPEDILEN", "ODENECEKTUTAR", "ACIKLAMA", "ODEMEEMRINO", "GELIRVERGISI" };

        /// <summary>
        /// Personel tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARPERSONEL = { "TCKIMLIKNO", "ADSOYAD", "UNVAN", "MUHASEBEKODU", "BIRIMKODU", "BANKA", "SUBE", "HESAPNO", "ISTIHDAMSEKLI" };

        /// <summary>
        /// Süreç tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARSUREC = { "YIL", "SURECNO", "SURECDURUM" };

        /// <summary>
        /// Geçici görev yolluk bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sb">Eklenecek geçici görev yolluk  bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(GeciciGorevYollukBelge sb)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sb.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sb.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", sb.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", sb.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)sb.belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYITSIRANO], "=", sb.kayitSiraNo));
            if (sb.tcKimlikNo != "" && sb.surecNo == "-1")//Dışardan şifresiz giriş 
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", sb.tcKimlikNo));

            IDataReader dr = veriErisim.DataReaderDondur(sql);

            bool sonuc = dr.Read();
            dr.Close();

            if (sonuc) //Kayıt var, güncelle
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], sb.tcKimlikNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROEK], sb.kadroEk) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GUNDELIKYIL], sb.gundelikYil) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GUNDELIKKOD], sb.gundelikKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], (int)sb.belgeTur) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOVIZKODU], sb.dovizKodu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UNVANKODU], sb.unvanKodu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BILDIRIMTARIH], sb.bildirimTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOVIZKURU], sb.dovizKuru) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKLENECEKACIKLAMA], sb.eklenecekAciklama) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKLENECEKTUTAR], sb.eklenecekTutar) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKLENECEKPARABIRIMIKOD], sb.eklenecekTutarParaBirimi.kod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CIKARILACAKACIKLAMA], sb.cikarilacakAciklama) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CIKARILACAKTUTAR], sb.cikarilacakTutar) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CIKARILACAKPARABIRIMIKOD], sb.cikarilacakTutarParaBirimi.kod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ARTTIRIMLIHESAPLA], OrtakFonksiyonlar.ConvertToInt(sb.arttirimliHesapla, 0)) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEORANI], sb.odemeOrani) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMANETORANI], sb.emanetOrani) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AVANSALINDIMI], OrtakFonksiyonlar.ConvertToInt(sb.avansAlindiMi, 0)) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURTARIHI], sb.kurTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KONAKLAMATAMAMIODENECEKMI], OrtakFonksiyonlar.ConvertToInt(sb.konaklamaTamamiOdenecekMi, 0)) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YURTICIKONAKLAMATURU], sb.yurtIciKonaklamaTuru);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sb.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sb.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", sb.birimKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", sb.surecNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)sb.belgeTur));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYITSIRANO], "=", sb.kayitSiraNo));
                if (sb.tcKimlikNo != "" && sb.surecNo == "-1")//Dışardan şifresiz giriş 
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", sb.tcKimlikNo));
            }
            else //Kayıt yok, ekle
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(sb.yil) + ",";
                sql += VeriErisim.DegerYap(sb.muhasebeKod) + ",";
                sql += VeriErisim.DegerYap(sb.birimKod) + ",";
                sql += VeriErisim.DegerYap(sb.surecNo) + ",";
                sql += VeriErisim.DegerYap((int)sb.belgeTur) + ",";
                sql += VeriErisim.DegerYap(sb.tcKimlikNo) + ",";
                sql += VeriErisim.DegerYap(sb.kadroEk) + ",";
                sql += VeriErisim.DegerYap(sb.gundelikYil) + ",";
                sql += VeriErisim.DegerYap(sb.gundelikKod) + ",";
                sql += VeriErisim.DegerYap(sb.dovizKodu) + ",";
                sql += VeriErisim.DegerYap(sb.dovizKuru) + ",";
                sql += VeriErisim.DegerYap(sb.unvanKodu) + ",";
                sql += VeriErisim.DegerYap(sb.bildirimTarihi) + ",";
                sql += VeriErisim.DegerYap(sb.odemeEmriNo) + ",";
                sql += VeriErisim.DegerYap(sb.eklenecekAciklama) + ",";
                sql += VeriErisim.DegerYap(sb.eklenecekTutar) + ",";
                sql += VeriErisim.DegerYap(sb.eklenecekTutarParaBirimi.kod) + ",";
                sql += VeriErisim.DegerYap(sb.cikarilacakAciklama) + ",";
                sql += VeriErisim.DegerYap(sb.cikarilacakTutar) + ",";
                sql += VeriErisim.DegerYap(sb.cikarilacakTutarParaBirimi.kod) + ",";
                sql += VeriErisim.DegerYap(OrtakFonksiyonlar.ConvertToInt(sb.arttirimliHesapla, 0)) + ",";
                sql += VeriErisim.DegerYap(sb.kayitSiraNo) + ",";
                sql += VeriErisim.DegerYap(sb.odemeOrani) + ",";
                sql += VeriErisim.DegerYap(sb.emanetOrani) + ",";
                sql += VeriErisim.DegerYap(OrtakFonksiyonlar.ConvertToInt(sb.avansAlindiMi, 0)) + ",";
                sql += VeriErisim.DegerYap(sb.kurTarihi) + ",";
                sql += VeriErisim.DegerYap(OrtakFonksiyonlar.ConvertToInt(sb.konaklamaTamamiOdenecekMi, 0)) + ",";
                sql += VeriErisim.DegerYap(sb.yurtIciKonaklamaTuru) + ")";
            }
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Geçici görev yolluk bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="sb">Listeleme için kullanılacak geçici görev yolluk  kıstas parametresi</param>
        /// <returns>
        /// Geçici görev yolluk bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(GeciciGorevYollukBelge sb)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + DBDegiskeni.TABLOADPERSONEL + ".AD" + "," + DBDegiskeni.TABLOADPERSONEL + ".SOYAD" + "," +
                DBDegiskeni.TABLOADUNVAN + ".AD" + "," + TABSurekliGorevGundelikTanim.TABLOAD + "." + TABSurekliGorevGundelikTanim.ALANLAR[(int)TABSurekliGorevGundelikTanim.ENUMAlanlar.DEGER];
            sql += " FROM " + TABLOAD + " LEFT OUTER JOIN " + DBDegiskeni.TABLOADPERSONEL + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + DBDegiskeni.TABLOADPERSONEL + ".TCKIMLIKNO ";
            sql += " LEFT OUTER JOIN " + TABSurekliGorevGundelikTanim.TABLOAD + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GUNDELIKKOD] + " = " + TABSurekliGorevGundelikTanim.TABLOAD + "." + TABSurekliGorevGundelikTanim.ALANLAR[(int)TABSurekliGorevGundelikTanim.ENUMAlanlar.KOD];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + TABSurekliGorevGundelikTanim.TABLOAD + "." + TABSurekliGorevGundelikTanim.ALANLAR[(int)TABSurekliGorevGundelikTanim.ENUMAlanlar.YIL];

            sql += " LEFT OUTER JOIN " + DBDegiskeni.TABLOADUNVAN + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.UNVANKODU] + " = " + DBDegiskeni.TABLOADUNVAN + ".KOD ";

            sql = VeriErisim.KosulEkle(sql, " WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", sb.yil));
            if (sb.muhasebeKod != 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sb.muhasebeKod));
            if (!string.IsNullOrWhiteSpace(sb.birimKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", sb.birimKod));
            if (sb.surecNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", sb.surecNo));
            if (sb.belgeTur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)sb.belgeTur));
            if (!string.IsNullOrWhiteSpace(sb.tcKimlikNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", sb.tcKimlikNo));
            if (sb.kayitSiraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYITSIRANO], "=", sb.kayitSiraNo));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Geçici görev yolluk bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="sb">Listeleme için kullanılacak geçici görev yolluk  kıstas parametresi</param>
        /// <returns>
        /// Geçici görev yolluk bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader OncedenKayitliMi(GeciciGorevYollukBelge sb)
        {
            string sql = "SELECT PSLGECICIGOREVYOLLUKBELGE.YIL, PSLGECICIGOREVYOLLUKBELGE.MUHASEBEKOD, PSLGECICIGOREVYOLLUKBELGE.BIRIMKOD, " +
                      " PSLGECICIGOREVYOLLUKBELGE.SURECNO, PSLGECICIGOREVYOLLUKBELGE.BELGETUR, PSLGECICIGOREVYOLLUKBELGE.TCKIMLIKNO, " +
                      " PSLGECICIGOREVYOLLUKDETAY.HAREKETTARIH " +
                      " FROM         PSLGECICIGOREVYOLLUKBELGE INNER JOIN " +
                      " PSLGECICIGOREVYOLLUKDETAY ON PSLGECICIGOREVYOLLUKBELGE.YIL = PSLGECICIGOREVYOLLUKDETAY.YIL AND " +
                      " PSLGECICIGOREVYOLLUKBELGE.SURECNO = PSLGECICIGOREVYOLLUKDETAY.SURECNO " +
                      " INNER JOIN HRCSUREC ON " +
                      " PSLGECICIGOREVYOLLUKBELGE.YIL = HRCSUREC.YIL " +
                      " AND PSLGECICIGOREVYOLLUKBELGE.SURECNO = HRCSUREC.SURECNO";
            sql = VeriErisim.KosulEkle(sql, " WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", sb.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCSUREC", "SURECDURUM", "<>", 9));
            if (sb.muhasebeKod != 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sb.muhasebeKod));
            if (sb.birimKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", sb.birimKod));
            if (sb.tcKimlikNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TCKIMLIKNOBELGE], "=", sb.tcKimlikNo));
            if (((GeciciGorevYollukDetay)sb.detay[0]) != null && (!((GeciciGorevYollukDetay)sb.detay[0]).hareketTarihi.isNull))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PSLGECICIGOREVYOLLUKDETAY", "HAREKETTARIH", "=", ((GeciciGorevYollukDetay)sb.detay[0]).hareketTarihi));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Geçici görev yolluk bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="sb">Listeleme için kullanılacak geçici görev yolluk  kıstas parametresi</param>
        /// <returns>
        /// Geçici görev yolluk bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader TumListe(GeciciGorevYollukBelge sb)
        {
            string sql = "SELECT     PSLGECICIGOREVYOLLUKBELGE.YIL, PSLGECICIGOREVYOLLUKBELGE.MUHASEBEKOD, PSLGECICIGOREVYOLLUKBELGE.BIRIMKOD, " +
                      " PSLGECICIGOREVYOLLUKBELGE.SURECNO, PSLGECICIGOREVYOLLUKBELGE.ODEMEEMRINO, FORM.ISLEMTARIH " +

                    " FROM         PSLGECICIGOREVYOLLUKBELGE LEFT OUTER JOIN " +
                      " MUHMIFFORM AS FORM ON PSLGECICIGOREVYOLLUKBELGE.ODEMEEMRINO = FORM.BELGENO AND  " +
                      " FORM.YIL = PSLGECICIGOREVYOLLUKBELGE.YIL LEFT OUTER JOIN " +
                      " HRCSUREC ON PSLGECICIGOREVYOLLUKBELGE.YIL = HRCSUREC.YIL AND PSLGECICIGOREVYOLLUKBELGE.SURECNO = HRCSUREC.SURECNO " +

                    " WHERE     (HRCSUREC.SURECDURUM <> 9) AND (PSLGECICIGOREVYOLLUKBELGE.ODEMEEMRINO <> '') AND (PSLGECICIGOREVYOLLUKBELGE.YIL = " + sb.yil + ") ";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Geçici görev yolluk detay bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sd">Eklenecek geçici görev yolluk detay bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetDetay(GeciciGorevYollukDetay sd)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(sd.yil) + ",";
            sql += VeriErisim.DegerYap(sd.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(sd.birimKod) + ",";
            sql += VeriErisim.DegerYap(sd.surecNo) + ",";
            sql += VeriErisim.DegerYap((int)sd.belgeTur) + ",";
            sql += VeriErisim.DegerYap(sd.tcKimlikNoBelge) + ",";
            sql += VeriErisim.DegerYap(sd.siraNo) + ",";
            sql += VeriErisim.DegerYap(sd.yolculukTipi) + ",";
            sql += VeriErisim.DegerYap(sd.yolculukGunSayi) + ",";
            sql += VeriErisim.DegerYap(sd.yolculukGunSayiStr) + ",";
            sql += VeriErisim.DegerYap(sd.nereden) + ",";
            sql += VeriErisim.DegerYap(sd.nereye) + ",";
            sql += VeriErisim.DegerYap(sd.hareketTarihi) + ",";
            sql += VeriErisim.DegerYap(sd.hareketSaati) + ",";
            sql += VeriErisim.DegerYap(sd.yevmiyeTutar) + ",";
            sql += VeriErisim.DegerYap(sd.yevmiyeParaBirimi.kod) + ",";
            sql += VeriErisim.DegerYap(sd.ikamettenTerminaleAracTipi) + ",";
            sql += VeriErisim.DegerYap(sd.ikamettenTerminaleHarcanan) + ",";
            sql += VeriErisim.DegerYap(sd.ikamettenTerminaleParaBirimi.kod) + ",";
            sql += VeriErisim.DegerYap(sd.yolculukAracTipi) + ",";
            sql += VeriErisim.DegerYap(sd.yolculuktaHarcanan) + ",";
            sql += VeriErisim.DegerYap(sd.yolculuktaParaBirimi.kod) + ",";
            sql += VeriErisim.DegerYap(sd.terminaldenIkameteAracTipi) + ",";
            sql += VeriErisim.DegerYap(sd.terminaldenIkameteHarcanan) + ",";
            sql += VeriErisim.DegerYap(sd.terminaldenIkameteParaBirimi.kod) + ",";
            sql += VeriErisim.DegerYap(sd.gorevGunSayisi) + ",";
            sql += VeriErisim.DegerYap(sd.gorevYevmiyeToplami) + ",";
            sql += VeriErisim.DegerYap(sd.konaklananGunSayi) + ",";
            sql += VeriErisim.DegerYap(sd.konaklamaGunlukFatura) + ",";
            sql += VeriErisim.DegerYap(sd.konaklamaGunlukFaturaParaBirimi.kod) + ",";
            sql += VeriErisim.DegerYap(sd.hamalBagaj) + ",";
            sql += VeriErisim.DegerYap(sd.hamalBagajParaBirimi.kod) + ",";
            sql += VeriErisim.DegerYap(sd.toplamKonaklamaOdenen) + ",";
            sql += VeriErisim.DegerYap(sd.toplamTutarDoviz) + ",";
            sql += VeriErisim.DegerYap(sd.toplamTutar) + ",";
            sql += VeriErisim.DegerYap(sd.kayitSiraNo) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Toplu geçici görev yolluk bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sd">Eklenecek toplu geçici görev yolluk bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetToplu(GeciciGorevYollukToplu sd)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc(false, "", "");

            sql = "UPDATE " + TABLOADTOPLU + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.CEKTIGIAVANS], sd.cektigiAvans) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.YOLLUKTUTARI], sd.yollukTutari) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.PTT], sd.ptt) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.DAMGAVERGISI], sd.damgaVergisi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.ELEGECEN], sd.eleGecen) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.MAHSUPEDILEN], sd.mahsupEdilen) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.ODENECEKTUTAR], sd.odenecekTutar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.ACIKLAMA], sd.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.ODEMEEMRINO], sd.odemeEmriNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.GELIRVERGISI], sd.gelirVergisi);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.YIL], "=", sd.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.MUHASEBEKOD], "=", sd.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.BIRIMKOD], "=", sd.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.SURECNO], "=", sd.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.BELGETUR], "=", sd.belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.SIRANO], "=", sd.siraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.TCKIMLIKNO], "=", sd.tcKimlikNo));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOADTOPLU + " (" + VeriErisim.TumAlanlar("", ALANLARTOPLU) + ") VALUES (";
                sql += VeriErisim.DegerYap(sd.yil) + ",";
                sql += VeriErisim.DegerYap(sd.muhasebeKod) + ",";
                sql += VeriErisim.DegerYap(sd.birimKod) + ",";
                sql += VeriErisim.DegerYap(sd.surecNo) + ",";
                sql += VeriErisim.DegerYap(sd.tcKimlikNo) + ",";
                sql += VeriErisim.DegerYap(sd.belgeTur) + ",";
                sql += VeriErisim.DegerYap(sd.siraNo) + ",";
                sql += VeriErisim.DegerYap(sd.cektigiAvans) + ",";
                sql += VeriErisim.DegerYap(sd.yollukTutari) + ",";
                sql += VeriErisim.DegerYap(sd.ptt) + ",";
                sql += VeriErisim.DegerYap(sd.damgaVergisi) + ",";
                sql += VeriErisim.DegerYap(sd.eleGecen) + ",";
                sql += VeriErisim.DegerYap(sd.mahsupEdilen) + ",";
                sql += VeriErisim.DegerYap(sd.odenecekTutar) + ",";
                sql += VeriErisim.DegerYap(sd.aciklama) + ",";
                sql += VeriErisim.DegerYap(sd.odemeEmriNo) + ",";
                sql += VeriErisim.DegerYap(sd.gelirVergisi) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Toplu geçici görev yolluk bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sd">Silinecek toplu geçici görev yolluk bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc SilDetay(GeciciGorevYollukToplu sd)
        {
            Sonuc sonuc = new Sonuc();

            string sql = "DELETE FROM " + TABLOADDETAY;

            //Fişe ait bütün satırlar
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", sd.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", sd.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMKOD], "=", sd.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO], "=", sd.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BELGETUR], "=", sd.belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TCKIMLIKNOBELGE], "=", sd.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.KAYITSIRANO], "=", sd.siraNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Geçici görev yolluk detay bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sd">Silinecek geçici görev yolluk detay bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc SilDetay(GeciciGorevYollukDetay sd)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;

            //Fişe ait bütün satırlar
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", sd.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", sd.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMKOD], "=", sd.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO], "=", sd.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BELGETUR], "=", (int)sd.belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.KAYITSIRANO], "=", sd.kayitSiraNo));
            if (sd.tcKimlikNoBelge != "" && sd.surecNo == "-1")//Dışardan şifresiz giriş 
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TCKIMLIKNOBELGE], "=", sd.tcKimlikNoBelge));

            //Tek satır
            if (sd.siraNo != 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.SIRANO], "=", sd.siraNo));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SilDetay(int yil, string surecNo)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=",  yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO], "=", surecNo));

            return KomutCalistir(sql, 0);
        }


        public Sonuc SilAna(int yil, string surecNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecNo));

            return KomutCalistir(sql, 0);
        }


        /// <summary>
        /// Toplu geçici görev yolluk bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sd">Silinecek toplu geçici görev yolluk bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc SilToplu(GeciciGorevYollukToplu sd)
        {
            Sonuc sonuc = new Sonuc();

            string sql = "DELETE FROM " + TABLOADTOPLU;

            //Fişe ait bütün satırlar
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.YIL], "=", sd.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.MUHASEBEKOD], "=", sd.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.BIRIMKOD], "=", sd.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.SURECNO], "=", sd.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.BELGETUR], "=", sd.belgeTur));
            if (sd.tcKimlikNo != "" && sd.surecNo == "-1")//Dışardan şifresiz giriş 
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.TCKIMLIKNO], "=", sd.tcKimlikNo));

            //Tek satır
            if (sd.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.SIRANO], "=", sd.siraNo));

            sonuc = KomutCalistir(sql, 0);

            if (sonuc.islemSonuc)
            {
                sonuc = SilDetay(sd);

                if (sonuc.islemSonuc)
                {
                    string sql2 = "DELETE FROM " + TABLOAD;

                    //Fişe ait bütün satırlar
                    sql2 = VeriErisim.KosulEkle(sql2, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", sd.yil));
                    sql2 = VeriErisim.KosulEkle(sql2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sd.muhasebeKod));
                    sql2 = VeriErisim.KosulEkle(sql2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", sd.birimKod));
                    sql2 = VeriErisim.KosulEkle(sql2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", sd.surecNo));
                    sql2 = VeriErisim.KosulEkle(sql2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", sd.belgeTur));
                    sql2 = VeriErisim.KosulEkle(sql2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", sd.tcKimlikNo));
                    sql2 = VeriErisim.KosulEkle(sql2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYITSIRANO], "=", sd.siraNo));

                    return KomutCalistir(sql2, 0);
                }
                else
                    return sonuc;
            }
            else
                return sonuc;


            //return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Geçici görev yolluk detay bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="sd">Listeleme için kullanılacak geçici görev yolluk detay kıstas parametresi</param>
        /// <returns>
        /// Geçici görev yolluk detay bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleDetay(GeciciGorevYollukBelge sd)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + " FROM " + TABLOADDETAY;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", sd.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlar.MUHASEBEKOD], "=", sd.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlar.BIRIMKOD], "=", sd.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO], "=", sd.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BELGETUR], "=", (int)sd.belgeTur));
            if (sd.kayitSiraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.KAYITSIRANO], "=", sd.kayitSiraNo));
            if (sd.tcKimlikNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TCKIMLIKNOBELGE], "=", sd.tcKimlikNo));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Toplu geçici görev yolluk bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="sd">Listeleme için kullanılacak toplu geçici görev yolluk kıstas parametresi</param>
        /// <returns>
        /// Toplu geçici görev yolluk detay bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleToplu(GeciciGorevYollukToplu sd)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTOPLU, ALANLARTOPLU) + ", " +
                TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.ADSOYAD] + ", " +
                TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.UNVAN] + ", " +
                TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.BANKA] + ", " +
                TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.SUBE] + ", " +
                TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.HESAPNO];

            sql += " FROM " + TABLOADTOPLU +
                " LEFT OUTER JOIN " + TABLOADPERSONEL + " ON " +
                    TABLOADTOPLU + "." + ALANLARTOPLU[(int)ENUMAlanlarToplu.TCKIMLIKNO] + " = " + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.TCKIMLIKNO];

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.YIL], "=", sd.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.MUHASEBEKOD], "=", sd.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.BIRIMKOD], "=", sd.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.SURECNO], "=", sd.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.BELGETUR], "=", sd.belgeTur));
            if (sd.tcKimlikNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.TCKIMLIKNO], "=", sd.tcKimlikNo));

            sql += " ORDER BY  " + ALANLARTOPLU[(int)ENUMAlanlarToplu.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Toplu geçici görev yolluk banka listesi bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="sd">Listeleme için kullanılacak toplu geçici görev yolluk kıstas parametresi</param>
        /// <returns>
        /// Toplu geçici görev yolluk detay bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleTopluBankaListesi(GeciciGorevYollukToplu sd)
        {
            string sql = "SELECT " + TABLOADTOPLU + "." + ALANLARTOPLU[(int)ENUMAlanlarToplu.TCKIMLIKNO] + ", " +
                "SUM(" + TABLOADTOPLU + "." + ALANLARTOPLU[(int)ENUMAlanlarToplu.ODENECEKTUTAR] + ") ODENECEKTUTAR, " +
                TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.ADSOYAD] + ", " +
                TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.UNVAN] + ", " +
                TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.BANKA] + ", " +
                TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.SUBE] + ", " +
                TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.HESAPNO];

            sql += " FROM " + TABLOADTOPLU +
                " LEFT OUTER JOIN " + TABLOADPERSONEL + " ON " +
                    TABLOADTOPLU + "." + ALANLARTOPLU[(int)ENUMAlanlarToplu.TCKIMLIKNO] + " = " + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.TCKIMLIKNO];

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.YIL], "=", sd.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.MUHASEBEKOD], "=", sd.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.BIRIMKOD], "=", sd.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.SURECNO], "=", sd.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.BELGETUR], "=", sd.belgeTur));
            if (sd.tcKimlikNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTOPLU, ALANLARTOPLU[(int)ENUMAlanlarToplu.TCKIMLIKNO], "=", sd.tcKimlikNo));

            sql += " GROUP BY  " + TABLOADTOPLU + "." + ALANLARTOPLU[(int)ENUMAlanlarToplu.TCKIMLIKNO] + ", " +
                TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.ADSOYAD] + ", " +
                TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.UNVAN] + ", " +
                TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.BANKA] + ", " +
                TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.SUBE] + ", " +
                TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.HESAPNO];

            sql += " ORDER BY  " + ALANLARTOPLU[(int)ENUMAlanlarToplu.TCKIMLIKNO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Geçici görev yolluk belgesine ait ödeme emri belge numarası bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="surecNo">Listeleme için kullanılacak süreç numarası kıstas parametresi</param>
        /// <returns>
        /// Geçici görev yolluk belgesine ait ödeme emri belge numarası bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleOdemeEmri(int yil, string surecNo)
        {
            string sql = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "," +
                              TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO] +
                  " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecNo));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Geçici görev yolluk belgesine ait ödeme emri belge numarası bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek geçici görev yolluk belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc OdemeEmriNoKaydet(GeciciGorevYollukBelge bilgi)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], bilgi.odemeEmriNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)bilgi.belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYITSIRANO], "=", bilgi.kayitSiraNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Toplu geçici görev yolluk belgesine ait ödeme emri belge numarası bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek toplu geçici görev yolluk bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc OdemeEmriNoKaydet(GeciciGorevYollukToplu bilgi)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADTOPLU + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.ODEMEEMRINO], bilgi.odemeEmriNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTOPLU[(int)ENUMAlanlarToplu.BELGETUR], "=", bilgi.belgeTur));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Şifresiz girişten kaydedilen geçici görev yolluk bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sd">Silinecek geçici görev yolluk belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc HavuzSil(GeciciGorevYollukBelge sd)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", sd.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sd.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", sd.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", sd.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)sd.belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", sd.tcKimlikNo));

            Sonuc sonuc = KomutCalistir(sql, 0);

            if (sonuc.islemSonuc)
            {
                sql = "DELETE FROM " + TABLOADDETAY;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", sd.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", sd.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMKOD], "=", sd.birimKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO], "=", sd.surecNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BELGETUR], "=", (int)sd.belgeTur));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TCKIMLIKNOBELGE], "=", sd.tcKimlikNo));

                sonuc = KomutCalistir(sql, 0);
            }

            return sonuc;
        }

        /// <summary>
        /// Geçici görev yolluk taksi harcaması istatistik bilgisini (kur ile hesaplayarak) veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak yolluk istatistik kıstas parametresi</param>
        /// <returns>
        /// Geçici görev yolluk taksi harcaması istatistik bilgisini (kur ile hesaplayarak) bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleTaksiIstatistikKurlu(PSLYollukIstatistikSinif kriter)
        {
            string sql = "";
            string kosul = "";
            string groupBy = "";
            string orderBy = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "/1000"
                + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR]
                + ", " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI"
                + ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.IKAMETTERMINALHARC] + "*" + TABKurBilgisi.TABLOAD + "." + TABKurBilgisi.ALANLAR[(int)TABKurBilgisi.ENUMAlanlar.KUR] + ") +"
                 + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TERMINALIKMETHARC] + "*" + TABKurBilgisi.TABLOAD + "." + TABKurBilgisi.ALANLAR[(int)TABKurBilgisi.ENUMAlanlar.KUR] + ")"
                       + ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.IKAMETTERMINALHARC] + ") +"
                 + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TERMINALIKMETHARC] + ")"
          + ", " + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TCKIMLIKNOBELGE] + "))"
             + ", " + VeriErisim.IslecBul("COUNT") + "(*)"

                 + " FROM " + TABLOAD;

            sql += " INNER JOIN " + TABLOADDETAY + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.BELGETUR];

            sql += " INNER JOIN " + TABKurBilgisi.TABLOAD + " ON ";
            sql += TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + " = " + TABKurBilgisi.TABLOAD + "." + TABKurBilgisi.ALANLAR[(int)TABKurBilgisi.ENUMAlanlar.YIL] + " AND ";
            sql += TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO] + " = " + TABKurBilgisi.TABLOAD + "." + TABKurBilgisi.ALANLAR[(int)TABKurBilgisi.ENUMAlanlar.SURECNO] + " AND ";
            sql += TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TERMINALIKMETPARABIRIMI] + " = " + TABKurBilgisi.TABLOAD + "." + TABKurBilgisi.ALANLAR[(int)TABKurBilgisi.ENUMAlanlar.PARABIRIMIKOD];

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABKurBilgisi.TABLOAD, TABKurBilgisi.ALANLAR[(int)TABKurBilgisi.ENUMAlanlar.TIP], "=", 1));

            sql += " INNER JOIN " + TABSurec.TABLOAD + " ON ";
            sql += TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO];
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<", (int)ENUMSurecDurum.IPTAL));

            sql += " INNER JOIN " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + " ON ";
            sql += "  " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".ILKODU" + "=" + TABLOAD + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD] + "/1000";

            sql += " INNER JOIN " + DBDegiskeni.MUHMIF + " ON ";
            sql += "  " + DBDegiskeni.MUHMIF + ".BELGENO" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO];
            sql += " AND " + DBDegiskeni.MUHMIF + ".YIL" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(DBDegiskeni.MUHMIF, "HESAPKOD", "LIKE", "830%"));

            sql += " INNER JOIN " + DBDegiskeni.MUHMIFFORM + " ON ";
            sql += "  " + DBDegiskeni.MUHMIF + ".BELGENO" + "=" + DBDegiskeni.MUHMIFFORM + ".BELGENO";
            sql += " AND " + DBDegiskeni.MUHMIF + ".YIL" + "=" + DBDegiskeni.MUHMIFFORM + ".YIL";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(DBDegiskeni.MUHMIFFORM, "FORMDURUM", "<", 9));


            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], "<>", ""));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", kriter.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "<>", -1));

            groupBy = " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD]
                + "/1000" + ", " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI";
            orderBy = " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD]
                + "/1000";
            if (kosul != "")
                sql += " WHERE " + kosul;

            if (groupBy != "")
                sql += groupBy + orderBy;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Geçici görev yolluk taksi harcaması istatistik bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak yolluk istatistik kıstas parametresi</param>
        /// <returns>
        /// Geçici görev yolluk taksi harcaması istatistik bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleTaksiIstatistikKursuz(PSLYollukIstatistikSinif kriter)
        {
            string sql = "";
            string kosul = "";
            string groupBy = "";
            string orderBy = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "/1000"
                + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR]
                + ", " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI"
                + ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.IKAMETTERMINALHARC] + ") +"
                 + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TERMINALIKMETHARC] + ")"
                  + ", " + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TCKIMLIKNOBELGE] + "))"
          + ", " + VeriErisim.IslecBul("COUNT") + "(*)"
                 + " FROM " + TABLOAD;

            sql += " INNER JOIN " + TABLOADDETAY + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.BELGETUR];

            sql += " INNER JOIN " + TABSurec.TABLOAD + " ON ";
            sql += TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO];
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<", (int)ENUMSurecDurum.IPTAL));

            sql += " INNER JOIN " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + " ON ";
            sql += "  " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".ILKODU" + "=" + TABLOAD + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD] + "/1000";

            sql += " INNER JOIN " + DBDegiskeni.MUHMIF + " ON ";
            sql += "  " + DBDegiskeni.MUHMIF + ".BELGENO" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO];
            sql += " AND " + DBDegiskeni.MUHMIF + ".YIL" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(DBDegiskeni.MUHMIF, "HESAPKOD", "LIKE", "830%"));

            sql += " INNER JOIN " + DBDegiskeni.MUHMIFFORM + " ON ";
            sql += "  " + DBDegiskeni.MUHMIF + ".BELGENO" + "=" + DBDegiskeni.MUHMIFFORM + ".BELGENO";
            sql += " AND " + DBDegiskeni.MUHMIF + ".YIL" + "=" + DBDegiskeni.MUHMIFFORM + ".YIL";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(DBDegiskeni.MUHMIFFORM, "FORMDURUM", "<", 9));


            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], "<>", ""));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", kriter.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "<>", -1));

            groupBy = " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD]
                + "/1000" + ", " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI";
            orderBy = " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD]
                + "/1000";
            if (kosul != "")
                sql += " WHERE " + kosul;

            if (groupBy != "")
                sql += groupBy + orderBy;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Geçici görev yolluk yol harcaması istatistik bilgisini (kur ile hesaplayarak) veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak yolluk istatistik kıstas parametresi</param>
        /// <returns>
        /// Geçici görev yolluk yol harcaması istatistik bilgisini (kur ile hesaplayarak) bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleYolIstatistikKurlu(PSLYollukIstatistikSinif kriter)
        {
            string sql = "";
            string kosul = "";
            string groupBy = "";
            string orderBy = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "/1000"
                + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR]
                + ", " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI"
                + ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YOCULUKHARCANAN] + "*" + TABKurBilgisi.TABLOAD + "." + TABKurBilgisi.ALANLAR[(int)TABKurBilgisi.ENUMAlanlar.KUR] + ")"
                       + ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YOCULUKHARCANAN] + ")"
          + ", " + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TCKIMLIKNOBELGE] + "))"
             + ", " + VeriErisim.IslecBul("COUNT") + "(*)"
                 + " FROM " + TABLOAD;

            sql += " INNER JOIN " + TABLOADDETAY + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.BELGETUR];

            sql += " INNER JOIN " + TABKurBilgisi.TABLOAD + " ON ";
            sql += TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + " = " + TABKurBilgisi.TABLOAD + "." + TABKurBilgisi.ALANLAR[(int)TABKurBilgisi.ENUMAlanlar.YIL] + " AND ";
            sql += TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO] + " = " + TABKurBilgisi.TABLOAD + "." + TABKurBilgisi.ALANLAR[(int)TABKurBilgisi.ENUMAlanlar.SURECNO] + " AND ";
            sql += TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YOCULUKPARABIRIMI] + " = " + TABKurBilgisi.TABLOAD + "." + TABKurBilgisi.ALANLAR[(int)TABKurBilgisi.ENUMAlanlar.PARABIRIMIKOD];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABKurBilgisi.TABLOAD, TABKurBilgisi.ALANLAR[(int)TABKurBilgisi.ENUMAlanlar.TIP], "=", 1));

            sql += " INNER JOIN " + TABSurec.TABLOAD + " ON ";
            sql += TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO];
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<", (int)ENUMSurecDurum.IPTAL));

            sql += " INNER JOIN " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + " ON ";
            sql += "  " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".ILKODU" + "=" + TABLOAD + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD] + "/1000";

            sql += " INNER JOIN " + DBDegiskeni.MUHMIF + " ON ";
            sql += "  " + DBDegiskeni.MUHMIF + ".BELGENO" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO];
            sql += " AND " + DBDegiskeni.MUHMIF + ".YIL" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(DBDegiskeni.MUHMIF, "HESAPKOD", "LIKE", "830%"));

            sql += " INNER JOIN " + DBDegiskeni.MUHMIFFORM + " ON ";
            sql += "  " + DBDegiskeni.MUHMIF + ".BELGENO" + "=" + DBDegiskeni.MUHMIFFORM + ".BELGENO";
            sql += " AND " + DBDegiskeni.MUHMIF + ".YIL" + "=" + DBDegiskeni.MUHMIFFORM + ".YIL";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(DBDegiskeni.MUHMIFFORM, "FORMDURUM", "<", 9));


            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], "<>", ""));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", kriter.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "<>", -1));


            groupBy = " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD]
                + "/1000" + ", " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI";
            orderBy = " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD]
                + "/1000";
            if (kosul != "")
                sql += " WHERE " + kosul;

            if (groupBy != "")
                sql += groupBy + orderBy;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Geçici görev yolluk yol harcaması istatistik bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak yolluk istatistik kıstas parametresi</param>
        /// <returns>
        /// Geçici görev yolluk yol harcaması istatistik bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleYolIstatistikKursuz(PSLYollukIstatistikSinif kriter)
        {
            string sql = "";
            string kosul = "";
            string groupBy = "";
            string orderBy = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "/1000"
                + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR]
                + ", " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI"
                + ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YOCULUKHARCANAN] + ")"
                  + ", " + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TCKIMLIKNOBELGE] + "))"
                     + ", " + VeriErisim.IslecBul("COUNT") + "(*)"
                 + " FROM " + TABLOAD;

            sql += " INNER JOIN " + TABLOADDETAY + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.BELGETUR];

            sql += " INNER JOIN " + TABSurec.TABLOAD + " ON ";
            sql += TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO];
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<", (int)ENUMSurecDurum.IPTAL));

            sql += " INNER JOIN " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + " ON ";
            sql += "  " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".ILKODU" + "=" + TABLOAD + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD] + "/1000";

            sql += " INNER JOIN " + DBDegiskeni.MUHMIF + " ON ";
            sql += "  " + DBDegiskeni.MUHMIF + ".BELGENO" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO];
            sql += " AND " + DBDegiskeni.MUHMIF + ".YIL" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(DBDegiskeni.MUHMIF, "HESAPKOD", "LIKE", "830%"));

            sql += " INNER JOIN " + DBDegiskeni.MUHMIFFORM + " ON ";
            sql += "  " + DBDegiskeni.MUHMIF + ".BELGENO" + "=" + DBDegiskeni.MUHMIFFORM + ".BELGENO";
            sql += " AND " + DBDegiskeni.MUHMIF + ".YIL" + "=" + DBDegiskeni.MUHMIFFORM + ".YIL";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(DBDegiskeni.MUHMIFFORM, "FORMDURUM", "<", 9));


            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], "<>", ""));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", kriter.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "<>", -1));

            groupBy = " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD]
                + "/1000" + ", " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI";
            orderBy = " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD]
                + "/1000";
            if (kosul != "")
                sql += " WHERE " + kosul;

            if (groupBy != "")
                sql += groupBy + orderBy;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Geçici görev yolluk otel harcaması istatistik bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak yolluk istatistik kıstas parametresi</param>
        /// <returns>
        /// Geçici görev yolluk otel harcaması istatistik bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleOtelIstatistikKursuz(PSLYollukIstatistikSinif kriter)
        {
            string sql = "";
            string kosul = "";
            string groupBy = "";
            string orderBy = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "/1000"
                + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR]
                + ", " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI"
                + ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMKONAKLAMATUTAR] + ")"
                  + ", " + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TCKIMLIKNOBELGE] + "))"
                     + ", " + VeriErisim.IslecBul("COUNT") + "(*)"
                 + " FROM " + TABLOAD;

            sql += " INNER JOIN " + TABLOADDETAY + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.BELGETUR];

            sql += " INNER JOIN " + TABSurec.TABLOAD + " ON ";
            sql += TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO];
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<", (int)ENUMSurecDurum.IPTAL));

            sql += " INNER JOIN " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + " ON ";
            sql += "  " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".ILKODU" + "=" + TABLOAD + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD] + "/1000";

            sql += " INNER JOIN " + DBDegiskeni.MUHMIF + " ON ";
            sql += "  " + DBDegiskeni.MUHMIF + ".BELGENO" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO];
            sql += " AND " + DBDegiskeni.MUHMIF + ".YIL" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(DBDegiskeni.MUHMIF, "HESAPKOD", "LIKE", "830%"));

            sql += " INNER JOIN " + DBDegiskeni.MUHMIFFORM + " ON ";
            sql += "  " + DBDegiskeni.MUHMIF + ".BELGENO" + "=" + DBDegiskeni.MUHMIFFORM + ".BELGENO";
            sql += " AND " + DBDegiskeni.MUHMIF + ".YIL" + "=" + DBDegiskeni.MUHMIFFORM + ".YIL";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(DBDegiskeni.MUHMIFFORM, "FORMDURUM", "<", 9));


            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], "<>", ""));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", kriter.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "<>", -1));

            groupBy = " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD]
                + "/1000" + ", " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI";
            orderBy = " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD]
                + "/1000";
            if (kosul != "")
                sql += " WHERE " + kosul;

            if (groupBy != "")
                sql += groupBy + orderBy;

            return veriErisim.DataReaderDondur(sql);
        }


        /// <summary>
        /// Geçici görev yolluk toplam yolculuk gün sayısı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="muhasebeKod">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birimKod">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <param name="belgeTur">Listeleme için kullanılacak belge tür kıstas parametresi</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse sadece il bazında listeleme yapar</param>
        /// <returns>
        /// Geçici görev yolluk toplam yolculuk gün sayısı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public double ToplamYolculukGunSayisi(int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl)
        {
            string sql = "";

            sql = "SELECT SUM(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YOLCULUKGUNSAYISI] + ") AS TOPLAMYOLCULUKGUNSAYISI ";
            sql += " FROM " + TABLOADDETAY + " INNER JOIN " + TABLOADSUREC + " ON ";
            sql += TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.YIL] + " AND ";
            sql += TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO];

            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "<>", 9));
            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", yil));
            if (birimKod != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", muhasebeKod));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMKOD], "=", birimKod));
            }
            else
            {
                if (sadeceIl)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], ">=", muhasebeKod * 1000));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "<", (muhasebeKod + 1) * 1000));
                }
                else
                {
                    if (muhasebeKod > 0)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", muhasebeKod));
                }
            } if (belgeTur != ENUMYollukGundelikTuru.TANIMSIZ)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BELGETUR], "=", (int)belgeTur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Geçici görev yolluk toplam görev gün sayısı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="muhasebeKod">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birimKod">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <param name="belgeTur">Listeleme için kullanılacak belge tür kıstas parametresi</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse sadece il bazında listeleme yapar</param>
        /// <returns>
        /// Geçici görev yolluk toplam görev gün sayısı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public double ToplamGorevGunSayisi(int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl)
        {
            string sql = "";

            sql = "SELECT SUM(" + ALANLARDETAY[(int)ENUMAlanlarDetay.GOREVGUNSAYI] + ") AS TOPLAMGOREVGUNSAYISI ";
            sql += " FROM " + TABLOADDETAY + " INNER JOIN " + TABLOADSUREC + " ON ";
            sql += TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.YIL] + " AND ";
            sql += TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO];

            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "<>", 9));
            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", yil));
            if (birimKod != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", muhasebeKod));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMKOD], "=", birimKod));
            }
            else
            {
                if (sadeceIl)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], ">=", muhasebeKod * 1000));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "<", (muhasebeKod + 1) * 1000));
                }
                else
                {
                    if (muhasebeKod > 0)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", muhasebeKod));
                }
            } if (belgeTur != ENUMYollukGundelikTuru.TANIMSIZ)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BELGETUR], "=", (int)belgeTur));


            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Geçici görev yolluk toplam yevmiye tutarı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="muhasebeKod">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birimKod">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <param name="belgeTur">Listeleme için kullanılacak belge tür kıstas parametresi</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse sadece il bazında listeleme yapar</param>
        /// <returns>
        /// Geçici görev yolluk toplam yevmiye tutarı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public double ToplamYevmiyeTutar(int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl)
        {
            string sql = "";

            sql = "SELECT SUM(" + ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR] + ") AS ODENEN ";
            sql += " FROM " + TABLOADDETAY + " INNER JOIN " + TABLOADSUREC + " ON ";
            sql += TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.YIL] + " AND ";
            sql += TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO];

            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "<>", 9));
            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", yil));
            if (birimKod != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", muhasebeKod));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMKOD], "=", birimKod));
            }
            else
            {
                if (sadeceIl)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], ">=", muhasebeKod * 1000));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "<", (muhasebeKod + 1) * 1000));
                }
                else
                {
                    if (muhasebeKod > 0)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", muhasebeKod));
                }
            } if (belgeTur != ENUMYollukGundelikTuru.TANIMSIZ)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BELGETUR], "=", (int)belgeTur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Geçici görev yolluk toplam görevlendirilen personel sayısı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="muhasebeKod">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birimKod">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <param name="belgeTur">Listeleme için kullanılacak belge tür kıstas parametresi</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse sadece il bazında listeleme yapar</param>
        /// <returns>
        /// Geçici görev yolluk toplam görevlendirilen personel sayısı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public double ToplamAdamSayisi(int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl)
        {
            string sql = "";

            sql = "SELECT COUNT(*) AS ADAMSAYI ";
            sql += " FROM " + TABLOAD + " INNER JOIN " + TABLOADSUREC + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO];

            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "<>", 9));
            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (birimKod != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimKod));
            }
            else
            {
                if (sadeceIl)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ">=", muhasebeKod * 1000));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "<", (muhasebeKod + 1) * 1000));
                }
                else
                {
                    if (muhasebeKod > 0)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                }
            }
            if (belgeTur != ENUMYollukGundelikTuru.TANIMSIZ)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)belgeTur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Geçici görev yolluk toplam gerçekleştirme bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="muhasebeKod">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birimKod">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <param name="belgeTur">Listeleme için kullanılacak belge tür kıstas parametresi</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse sadece il bazında listeleme yapar</param>
        /// <returns>
        /// Geçici görev yolluk toplam gerçekleştirme personel sayısı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ToplamGerceklesmeListele(int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl)
        {
            string sql = "";

            sql = "SELECT " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.NEREYE] +
                ", COUNT(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] +
                ") AS GOREVSAYI, SUM(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.GOREVGUNSAYI] +
                ") AS TOPLAMGOREVGUNSAYI, SUM(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR] +
                ") AS TUTAR";

            sql += " FROM " + TABLOAD + " INNER JOIN " + TABLOADSUREC + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO];

            sql += " LEFT OUTER JOIN " + TABLOADDETAY + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO];

            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "<>", 9));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YOLCULUKTIPI], "=", 1));
            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));



            if (birimKod != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimKod));
            }
            else
            {
                if (sadeceIl)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ">=", muhasebeKod * 1000));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "<", (muhasebeKod + 1) * 1000));
                }
                else
                {
                    if (muhasebeKod > 0)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                }
            }
            if (belgeTur != ENUMYollukGundelikTuru.TANIMSIZ)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)belgeTur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "," + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.NEREYE];
            sql += " ORDER BY " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.NEREYE];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Denetim Elemanları Geçici görev yolluk toplam gerçekleştirme bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="muhasebeKod">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birimKod">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse sadece il bazında listeleme yapar</param>
        /// <returns>
        /// Denetim Elemanları Geçici görev yolluk toplam gerçekleştirme personel sayısı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ToplamGerceklesmeDenetimListele(int yil, int muhasebeKod, string birimKod, bool sadeceIl)
        {
            string sql = "";

            sql = "SELECT PSLSUREKLIGOREVGUNDELIKTANIM.AD, COUNT(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + ") AS GOREVSAYI, SUM(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.GOREVGUNSAYI] + ") AS TOPLAMGOREVGUNSAYI, SUM(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR] + ") AS TUTAR";
            sql += " FROM ";
            sql += TABLOAD + " LEFT OUTER JOIN HRCSUREC ON PSLGECICIGOREVYOLLUKBELGE.YIL = HRCSUREC.YIL AND PSLGECICIGOREVYOLLUKBELGE.SURECNO = HRCSUREC.SURECNO " +
                             " LEFT OUTER JOIN PSLSUREKLIGOREVGUNDELIKTANIM ON PSLGECICIGOREVYOLLUKBELGE.YIL = PSLSUREKLIGOREVGUNDELIKTANIM.YIL AND " +
                                    " PSLGECICIGOREVYOLLUKBELGE.GUNDELIKKOD = PSLSUREKLIGOREVGUNDELIKTANIM.KOD ";
            sql += " LEFT OUTER JOIN " + TABLOADDETAY + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO];

            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "<>", 9));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YOLCULUKTIPI], "=", 1));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)ENUMYollukGundelikTuru.DENETIMELEMANI));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("PSLSUREKLIGOREVGUNDELIKTANIM", "TUR", "=", (int)ENUMYollukGundelikTuru.DENETIMELEMANI));
            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("PSLSUREKLIGOREVGUNDELIKTANIM", "YIL", "=", yil));
            if (birimKod != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimKod));
            }
            else
            {
                if (sadeceIl)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ">=", muhasebeKod * 1000));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "<", (muhasebeKod + 1) * 1000));
                }
                else
                {
                    if (muhasebeKod > 0)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                }
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + ",PSLSUREKLIGOREVGUNDELIKTANIM.AD ";
            sql += " ORDER BY PSLSUREKLIGOREVGUNDELIKTANIM.AD ";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yurt içi geçici görev yolluk istatistik bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="tur">Listeleme için kullanılacak belge tür kıstas parametresi</param>
        /// <returns>
        /// Yurt içi geçici görev yolluk istatistik bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleYIGGYIstatistik(int yil, ENUMYIGGYTur tur)
        {
            string sql = "";

            sql = "SELECT  COUNT(A.SURECNO) AS GOREVSAYISI, SUM(A.GOREVGUNSAYI) AS TOPLAMGOREVGUNSAYI, SUM(A.TOPLAMTUTAR) AS TOPLAMTUTAR " +
                  " FROM " +
                  " (SELECT     PSLGECICIGOREVYOLLUKBELGE.YIL, PSLGECICIGOREVYOLLUKBELGE.MUHASEBEKOD, PSLGECICIGOREVYOLLUKBELGE.BIRIMKOD, " +
                      " PSLGECICIGOREVYOLLUKBELGE.SURECNO, SUM(PSLGECICIGOREVYOLLUKDETAY.TOPLAMTUTAR) AS TOPLAMTUTAR, " +
                     " SUM( PSLGECICIGOREVYOLLUKDETAY.GOREVGUNSAYI) AS GOREVGUNSAYI " +
                      "  FROM         PSLGECICIGOREVYOLLUKBELGE LEFT OUTER JOIN " +
                      " PSLGECICIGOREVYOLLUKDETAY ON PSLGECICIGOREVYOLLUKBELGE.SURECNO = PSLGECICIGOREVYOLLUKDETAY.SURECNO AND " +
                      " PSLGECICIGOREVYOLLUKBELGE.YIL = PSLGECICIGOREVYOLLUKDETAY.YIL LEFT OUTER JOIN " +
                      " HRCSUREC ON PSLGECICIGOREVYOLLUKBELGE.YIL = HRCSUREC.YIL AND PSLGECICIGOREVYOLLUKBELGE.SURECNO = HRCSUREC.SURECNO " +
                        " WHERE     (HRCSUREC.SURECDURUM <> 9) AND (PSLGECICIGOREVYOLLUKBELGE.ODEMEEMRINO <> '') AND (PSLGECICIGOREVYOLLUKBELGE.YIL = " + yil + ") AND (PSLGECICIGOREVYOLLUKBELGE.BELGETUR <> 3) " +
                        " GROUP BY PSLGECICIGOREVYOLLUKBELGE.YIL, PSLGECICIGOREVYOLLUKBELGE.MUHASEBEKOD, PSLGECICIGOREVYOLLUKBELGE.BIRIMKOD, " +
                      " PSLGECICIGOREVYOLLUKBELGE.SURECNO) AS A";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yurt dışı geçici görev yolluk istatistik bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="tur">Listeleme için kullanılacak belge tür kıstas parametresi</param>
        /// <returns>
        /// Yurt dışı geçici görev yolluk istatistik bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleYDGGYIstatistik(int yil, ENUMYDGGYTur tur)
        {
            string sql = "";

            sql = "SELECT  COUNT(A.SURECNO) AS GOREVSAYISI, SUM(A.GOREVGUNSAYI) AS TOPLAMGOREVGUNSAYI, SUM(A.TOPLAMTUTAR) AS TOPLAMTUTAR " +
                  " FROM " +
                  " (SELECT     PSLGECICIGOREVYOLLUKBELGE.YIL, PSLGECICIGOREVYOLLUKBELGE.MUHASEBEKOD, PSLGECICIGOREVYOLLUKBELGE.BIRIMKOD, " +
                      " PSLGECICIGOREVYOLLUKBELGE.SURECNO, SUM(PSLGECICIGOREVYOLLUKDETAY.TOPLAMTUTAR) AS TOPLAMTUTAR, " +
                     " SUM( PSLGECICIGOREVYOLLUKDETAY.GOREVGUNSAYI) AS GOREVGUNSAYI " +
                      "  FROM         PSLGECICIGOREVYOLLUKBELGE LEFT OUTER JOIN " +
                      " PSLGECICIGOREVYOLLUKDETAY ON PSLGECICIGOREVYOLLUKBELGE.SURECNO = PSLGECICIGOREVYOLLUKDETAY.SURECNO AND " +
                      " PSLGECICIGOREVYOLLUKBELGE.YIL = PSLGECICIGOREVYOLLUKDETAY.YIL LEFT OUTER JOIN " +
                      " HRCSUREC ON PSLGECICIGOREVYOLLUKBELGE.YIL = HRCSUREC.YIL AND PSLGECICIGOREVYOLLUKBELGE.SURECNO = HRCSUREC.SURECNO " +
                        " WHERE     (HRCSUREC.SURECDURUM <> 9) AND (PSLGECICIGOREVYOLLUKBELGE.ODEMEEMRINO <> '') AND (PSLGECICIGOREVYOLLUKBELGE.YIL = " + yil + ") AND (PSLGECICIGOREVYOLLUKBELGE.BELGETUR = 3) " +
                        " GROUP BY PSLGECICIGOREVYOLLUKBELGE.YIL, PSLGECICIGOREVYOLLUKBELGE.MUHASEBEKOD, PSLGECICIGOREVYOLLUKBELGE.BIRIMKOD, " +
                      " PSLGECICIGOREVYOLLUKBELGE.SURECNO) AS A";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yurt içi sürekli görev yolluk istatistik bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="tur">Listeleme için kullanılacak belge tür kıstas parametresi</param>
        /// <returns>
        /// Yurt içi sürekli görev yolluk istatistik bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleYISGYIstatistik(int yil, ENUMYISGYTur tur)
        {
            string sql = "";

            if (tur == ENUMYISGYTur.EMEKLI)
            {
                sql = "SELECT COUNT(A.SURECNO) AS ATAMASAYISI, SUM(A.EMEKLILIKMAASI) AS TOPLAMTUTAR " +
                      " FROM " +
                      " (SELECT     PSLSUREKLIGOREVYOLLUKBELGE.YIL, PSLSUREKLIGOREVYOLLUKBELGE.MUHASEBEKOD, PSLSUREKLIGOREVYOLLUKBELGE.BIRIMKOD, " +
                          " PSLSUREKLIGOREVYOLLUKBELGE.SURECNO, PSLSUREKLIGOREVYOLLUKBELGE.EMEKLILIKMAASI--, PSLSUREKLIGOREVYOLLUKBELGE.EMEKLILIKMAASIMI " +
                            " FROM         PSLSUREKLIGOREVYOLLUKBELGE LEFT OUTER JOIN " +
                          " PSLSUREKLIGOREVYOLLUKDETAY ON PSLSUREKLIGOREVYOLLUKBELGE.YIL = PSLSUREKLIGOREVYOLLUKDETAY.YIL AND " +
                          " PSLSUREKLIGOREVYOLLUKBELGE.SURECNO = PSLSUREKLIGOREVYOLLUKDETAY.SURECNO LEFT OUTER JOIN " +
                          " HRCSUREC ON PSLSUREKLIGOREVYOLLUKBELGE.YIL = HRCSUREC.YIL AND PSLSUREKLIGOREVYOLLUKBELGE.SURECNO = HRCSUREC.SURECNO " +
                            " WHERE   (PSLSUREKLIGOREVYOLLUKBELGE.YIL = " + yil + ") AND  (HRCSUREC.SURECDURUM <> 9) AND (PSLSUREKLIGOREVYOLLUKBELGE.ODEMEEMRINO <> '') AND (PSLSUREKLIGOREVYOLLUKBELGE.EMEKLILIKMAASIMI = 1) and (PSLSUREKLIGOREVYOLLUKBELGE.BELGETUR=1) " +
                            " group by PSLSUREKLIGOREVYOLLUKBELGE.YIL, PSLSUREKLIGOREVYOLLUKBELGE.MUHASEBEKOD, PSLSUREKLIGOREVYOLLUKBELGE.BIRIMKOD, " +
                          " PSLSUREKLIGOREVYOLLUKBELGE.SURECNO, PSLSUREKLIGOREVYOLLUKBELGE.EMEKLILIKMAASI) AS A";
            }
            else if (tur == ENUMYISGYTur.YERDEGISTIRME)
            {
                sql = "SELECT COUNT(A.SURECNO) AS ATAMASAYISI, SUM(A.TOPLAMTUTAR) AS TOPLAMTUTAR " +
                     " FROM " +
                     " (SELECT     PSLSUREKLIGOREVYOLLUKBELGE.YIL, PSLSUREKLIGOREVYOLLUKBELGE.MUHASEBEKOD, PSLSUREKLIGOREVYOLLUKBELGE.BIRIMKOD, " +
                      " PSLSUREKLIGOREVYOLLUKBELGE.SURECNO,SUM( PSLSUREKLIGOREVYOLLUKDETAY.TOPLAMTUTAR)AS TOPLAMTUTAR " +
                        " FROM         PSLSUREKLIGOREVYOLLUKBELGE LEFT OUTER JOIN " +
                      " PSLSUREKLIGOREVYOLLUKDETAY ON PSLSUREKLIGOREVYOLLUKBELGE.YIL = PSLSUREKLIGOREVYOLLUKDETAY.YIL AND " +
                      " PSLSUREKLIGOREVYOLLUKBELGE.SURECNO = PSLSUREKLIGOREVYOLLUKDETAY.SURECNO LEFT OUTER JOIN " +
                      " HRCSUREC ON PSLSUREKLIGOREVYOLLUKBELGE.YIL = HRCSUREC.YIL AND PSLSUREKLIGOREVYOLLUKBELGE.SURECNO = HRCSUREC.SURECNO " +
                        " WHERE     (HRCSUREC.SURECDURUM <> 9) AND (PSLSUREKLIGOREVYOLLUKBELGE.ODEMEEMRINO <> '') AND (PSLSUREKLIGOREVYOLLUKBELGE.EMEKLILIKMAASIMI = 0) and (PSLSUREKLIGOREVYOLLUKBELGE.BELGETUR=1) " +
                      " AND (PSLSUREKLIGOREVYOLLUKBELGE.YIL = " + yil + ") " +
                        " GROUP BY PSLSUREKLIGOREVYOLLUKBELGE.YIL, PSLSUREKLIGOREVYOLLUKBELGE.MUHASEBEKOD, PSLSUREKLIGOREVYOLLUKBELGE.BIRIMKOD, " +
                      " PSLSUREKLIGOREVYOLLUKBELGE.SURECNO) AS A";
            }

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yurt dışı sürekli görev yolluk istatistik bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="tur">Listeleme için kullanılacak belge tür kıstas parametresi</param>
        /// <returns>
        /// Yurt dışı sürekli görev yolluk istatistik bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleYDSGYIstatistik(int yil, ENUMYDSGYTur tur)
        {
            string sql = "";
            sql = "SELECT COUNT(A.SURECNO) AS ATAMASAYISI, SUM(A.TOPLAMTUTAR) AS TOPLAMTUTAR " +
                 " FROM " +
                 " (SELECT     PSLSUREKLIGOREVYOLLUKBELGE.YIL, PSLSUREKLIGOREVYOLLUKBELGE.MUHASEBEKOD, PSLSUREKLIGOREVYOLLUKBELGE.BIRIMKOD, " +
                  " PSLSUREKLIGOREVYOLLUKBELGE.SURECNO,SUM( PSLSUREKLIGOREVYOLLUKDETAY.TOPLAMTUTAR)AS TOPLAMTUTAR " +
                    " FROM         PSLSUREKLIGOREVYOLLUKBELGE LEFT OUTER JOIN " +
                  " PSLSUREKLIGOREVYOLLUKDETAY ON PSLSUREKLIGOREVYOLLUKBELGE.YIL = PSLSUREKLIGOREVYOLLUKDETAY.YIL AND " +
                  " PSLSUREKLIGOREVYOLLUKBELGE.SURECNO = PSLSUREKLIGOREVYOLLUKDETAY.SURECNO LEFT OUTER JOIN " +
                  " HRCSUREC ON PSLSUREKLIGOREVYOLLUKBELGE.YIL = HRCSUREC.YIL AND PSLSUREKLIGOREVYOLLUKBELGE.SURECNO = HRCSUREC.SURECNO " +
                    " WHERE     (HRCSUREC.SURECDURUM <> 9) AND (PSLSUREKLIGOREVYOLLUKBELGE.ODEMEEMRINO <> '') AND (PSLSUREKLIGOREVYOLLUKBELGE.EMEKLILIKMAASIMI = 0) and (PSLSUREKLIGOREVYOLLUKBELGE.BELGETUR=2) " +
                  " AND (PSLSUREKLIGOREVYOLLUKBELGE.YIL = " + yil + ") " +
                    " GROUP BY PSLSUREKLIGOREVYOLLUKBELGE.YIL, PSLSUREKLIGOREVYOLLUKBELGE.MUHASEBEKOD, PSLSUREKLIGOREVYOLLUKBELGE.BIRIMKOD, " +
                  " PSLSUREKLIGOREVYOLLUKBELGE.SURECNO) AS A";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
