using System;
using System.Data;
using TNS.Veri;
using System.Text;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Harcama Yönetimi raporlar sınıfı
    /// </summary>
    public class TABRaporlarv2 : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABRaporlarv2(VeriErisim veriErisim) : base(veriErisim) { }

        #region "TABLOLAR"
        /// <summary>
        /// Süreç bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCSUREC";

        /// <summary>
        /// Süreç bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
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
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 2,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 3,
            /// <summary>
            /// ISLEMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMKOD = 4,
            /// <summary>
            /// SURECTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECTARIH = 5,
            /// <summary>
            /// SURECDURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECDURUM = 6,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 7,
            /// <summary>
            /// ISINADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISINADI = 8,
            /// <summary>
            /// YILLARASARIISLEMMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YILLARASARIISLEMMI = 9,
        }

        /// <summary>
        /// Süreç tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "SURECNO", "MUHASEBEKOD", "BIRIMKOD", "ISLEMKOD", "SURECTARIH", "SURECDURUM", "ACIKLAMA", "ISINADI", "YILLARASARIISLEMMI" };

        /// <summary>
        /// Süreç durum bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADDURUM = "HRCSURECDURUM";

        /// <summary>
        /// Süreç durum bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarDurum
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
            /// ISLEMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMKOD = 2,
            /// <summary>
            /// PARCAKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PARCAKOD = 3,
            /// <summary>
            /// TARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TARIH = 4,
            /// <summary>
            /// KULLANICIAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANICIAD = 5,
            /// <summary>
            /// DURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DURUM = 6,
        }

        /// <summary>
        /// Süreç durum tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARDURUM = { "YIL", "SURECNO", "ISLEMKOD", "PARCAKOD", "TARIH", "KULLANICIAD", "DURUM" };

        /// <summary>
        /// İş bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADIS = "HRCIS";

        /// <summary>
        /// İş bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarIS
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 1,
            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 2,
            /// <summary>
            /// TUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUR = 3,
            /// <summary>
            /// ODEMEHESAPKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEHESAPKOD = 4
        }

        /// <summary>
        /// İş tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARIS = { "KOD", "AD", "EKOKOD", "TUR", "ODEMEHESAPKOD" };

        /// <summary>
        /// Muhasebe birim bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADMUHASEBEVIEW = "HRCMUHASEBEVIEW";

        /// <summary>
        /// Muhasebe birim bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarMUHASEBEVIEW
        {
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 0,
            /// <summary>
            /// MUHASEBEAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEAD = 1,
        }

        /// <summary>
        /// Muhasebe birim tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARMUHASEBEVIEW = { "MUHASEBEKOD", "MUHASEBEAD" };

        /// <summary>
        /// Parça bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADPARCA = "HRCPARCA";

        /// <summary>
        /// Parça bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarPARCA
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 1,
        }

        /// <summary>
        /// Parça tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARPARCA = { "KOD", "AD" };

        /// <summary>
        /// Hatırlatma bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADHATIRLATMA = "HRCHATIRLATMA";

        /// <summary>
        /// Hatırlatma bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarHATIRLATMA
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// KULLANICIAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANICIAD = 1,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 2,
        }

        /// <summary>
        /// Hatırlatma tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARHATIRLATMA = { "KOD", "KULLANICIAD", "ACIKLAMA" };

        /// <summary>
        /// Tertip bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADTERTIP = "HRCTERTIP";

        /// <summary>
        /// Tertip bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarTERTIP
        {
            /// <summary>
            /// TERTIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TERTIP = 0,
            /// <summary>
            /// KULLANICIAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANICIAD = 1,
        }

        /// <summary>
        /// Tertip tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARTERTIP = { "TERTIP", "KULLANICIAD" };

        /// <summary>
        /// Geçici görev yolluk belge bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADGECICIGOREVYOLLUKBELGE = "PSLGECICIGOREVYOLLUKBELGE";

        /// <summary>
        /// Geçici görev yolluk belge bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarGECICIGOREVYOLLUKBELGE
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
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 2,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 3,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 4,
        }

        /// <summary>
        /// Geçici görev yolluk belge tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARGECICIGOREVYOLLUKBELGE = { "YIL", "SURECNO", "MUHASEBEKOD", "BIRIMKOD", "TCKIMLIKNO" };

        /// <summary>
        /// Sürekli görev yolluk belge bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADSUREKLIGOREVYOLLUKBELGE = "PSLSUREKLIGOREVYOLLUKBELGE";

        /// <summary>
        /// Sürekli görev yolluk belge bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSUREKLIGOREVYOLLUKBELGE
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
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 2,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 3,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 4,
        }

        /// <summary>
        /// Sürekli görev yolluk belge tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARSUREKLIGOREVYOLLUKBELGE = { "YIL", "SURECNO", "MUHASEBEKOD", "BIRIMKOD", "TCKIMLIKNO" };

        /// <summary>
        /// Personel yolluk bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADYOLLUKLARVIEW = "PSLYOLLUKLARVIEW";

        /// <summary>
        /// Personel yolluk bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarYOLLUKLARVIEW
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
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 2,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 3,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 4,
        }

        /// <summary>
        /// Personel yolluk tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARYOLLUKLARVIEW = { "YIL", "SURECNO", "MUHASEBEKOD", "BIRIMKOD", "TCKIMLIKNO" };

        /// <summary>
        /// Süreç muhasebe işlem fişi bağlantı bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADMUHMIFBAG = "HRCSURECMUHMIFBAG";

        /// <summary>
        /// Süreç muhasebe işlem fişi bağlantı bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarMUHMIFBAG
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
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 2,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 3,
            /// <summary>
            /// ODEMEEMRINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 4,
        }

        /// <summary>
        /// Süreç muhasebe işlem fişi bağlantı tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARMUHMIFBAG = { "YIL", "SURECNO", "MUHASEBEKOD", "BIRIMKOD", "ODEMEEMRINO" };

        /// <summary>
        /// Onay form bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADONAYFORM = "HRCIHALEONAYFORM";

        /// <summary>
        /// Onay form bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarONAYFORM
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
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 2,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 3,
            /// <summary>
            /// IHALEUSULU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEUSULU = 4,
        }

        /// <summary>
        /// Onay form tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARONAYFORM = { "YIL", "SURECNO", "MUHASEBEKOD", "BIRIMKOD", "IHALEUSULU" };

        /// <summary>
        /// Onay form belge bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADONAY = "HRCIHALEONAYFORM";

        /// <summary>
        /// Onay form belge bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarOnay
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
            /// IDAREADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IDAREADI = 4,
            /// <summary>
            /// BELGETARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGETARIH = 5,
            /// <summary>
            /// EVRAKSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EVRAKSAYI = 6,
            /// <summary>
            /// ISTANIM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISTANIM = 7,
            /// <summary>
            /// ISNITELIK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISNITELIK = 8,
            /// <summary>
            /// ISMIKTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISMIKTAR = 9,
            /// <summary>
            /// ODENEKYILI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENEKYILI = 10,
            /// <summary>
            /// YAKLASIKMALIYET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKLASIKMALIYET = 11,
            /// <summary>
            /// YMPARABIRIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YMPARABIRIMI = 12,
            /// <summary>
            /// YMTIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YMTIP = 13,
            /// <summary>
            /// DOKUMANBEDELI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOKUMANBEDELI = 14,
            /// <summary>
            /// BUTCETERTIBI1 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BUTCETERTIBI1 = 15,
            /// <summary>
            /// BUTCETERTIBI2 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BUTCETERTIBI2 = 16,
            /// <summary>
            /// ODENEKTUTAR1 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENEKTUTAR1 = 17,
            /// <summary>
            /// ODENEKTUTAR2 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENEKTUTAR2 = 18,
            /// <summary>
            /// PROJENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PROJENO = 19,
            /// <summary>
            /// AVANSVARMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AVANSVARMI = 20,
            /// <summary>
            /// AVANSSARTLARI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AVANSSARTLARI = 21,
            /// <summary>
            /// IHALEUSULU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEUSULU = 22,
            /// <summary>
            /// IHALEMADDESI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEMADDESI = 23,
            /// <summary>
            /// ILANVERILECEKMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILANVERILECEKMI = 24,
            /// <summary>
            /// ILANSEKLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILANSEKLI = 25,
            /// <summary>
            /// ONYETERLILIK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONYETERLILIK = 26,
            /// <summary>
            /// FIYATFARKIVARMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIYATFARKIVARMI = 27,
            /// <summary>
            /// FIYATFARKI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIYATFARKI = 28,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 29,
            /// <summary>
            /// GERCEKGOREVLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GERCEKGOREVLI = 30,
            /// <summary>
            /// GERCEKGOREVLIUNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GERCEKGOREVLIUNVAN = 31,
            /// <summary>
            /// ONAYASUNUMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONAYASUNUMTARIHI = 32,
            /// <summary>
            /// IHALEYETKILISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEYETKILISI = 33,
            /// <summary>
            /// IHALEYETKILISIUNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEYETKILISIUNVAN = 34,
            /// <summary>
            /// ONAYLAMATARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONAYLAMATARIHI = 35,
            /// <summary>
            /// IHALEMAKAMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEMAKAMI = 36,
            /// <summary>
            /// KOMISYONVARMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOMISYONVARMI = 37,
            /// <summary>
            /// HTGSURESI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HTGSURESI = 38,
            /// <summary>
            /// HTGUSULU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HTGUSULU = 39,
            /// <summary>
            /// HTGGOREVLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HTGGOREVLI = 40,
            /// <summary>
            /// HTMIHMANDARVARMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HTMIHMANDARVARMI = 41,
            /// <summary>
            /// HTPERTCNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HTPERTCNO = 42,
            /// <summary>
            /// HRCTALIMAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HRCTALIMAT = 43,
            /// <summary>
            /// ONILANYILCEYREK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONILANYILCEYREK = 44,
            /// <summary>
            /// ONILANGERCEKGOREVLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONILANGERCEKGOREVLI = 45,
            /// <summary>
            /// ONILANGERCEKGOREVLIUNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONILANGERCEKGOREVLIUNVAN = 46,
            /// <summary>
            /// ONILANIHALEYETKILI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONILANIHALEYETKILI = 47,
            /// <summary>
            /// ONILANIHALEYETKILIUNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONILANIHALEYETKILIUNVAN = 48,
            /// <summary>
            /// ONILANONAYLAMATARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONILANONAYLAMATARIHI = 49,
            /// <summary>
            /// ONILANONAYASUNUMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONILANONAYASUNUMTARIHI = 50,
            /// <summary>
            /// ONILANVARMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONILANVARMI = 51,
            /// <summary>
            /// DOKUMANPOSTASATIS alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOKUMANPOSTASATIS = 52,
            /// <summary>
            /// DOKUMANYURTICIBEDEL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOKUMANYURTICIBEDEL = 53,
            /// <summary>
            /// DOKUMANYURTDISIBEDEL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOKUMANYURTDISIBEDEL = 54,
        }

        /// <summary>
        /// Onay form belge tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARONAY ={ "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "IDAREADI", "BELGETARIH",
        "EVRAKSAYI", "ISTANIM", "ISNITELIK", "ISMIKTAR", "ODENEKYILI", "YAKLASIKMALIYET", "YMPARABIRIMI", 
        "YMTIP", "DOKUMANBEDELI", "BUTCETERTIBI1", "BUTCETERTIBI2", "ODENEKTUTAR1", "ODENEKTUTAR2", 
        "PROJENO", "AVANSVARMI", "AVANSSARTLARI", "IHALEUSULU", "IHALEMADDESI", "ILANVERILECEKMI", 
        "ILANSEKLI", "ONYETERLILIK", "FIYATFARKIVARMI", "FIYATFARKI", "ACIKLAMA", "GERCEKGOREVLI", 
        "GERCEKGOREVLIUNVAN", "ONAYASUNUMTARIHI", "IHALEYETKILISI", "IHALEYETKILISIUNVAN", "ONAYLAMATARIHI", 
        "IHALEMAKAMI", "KOMISYONVARMI", "HTGSURESI","HTGUSULU","HTGGOREVLI","HTMIHMANDARVARMI","HTPERTCNO","HRCTALIMATI",
        "ONILANYILCEYREK","ONILANGERCEKGOREVLI","ONILANGERCEKGOREVLIUNVAN","ONILANIHALEYETKILI","ONILANIHALEYETKILIUNVAN",
        "ONILANONAYLAMATARIHI","ONILANONAYASUNUMTARIHI","ONILANVARMI","DOKUMANPOSTASATIS","DOKUMANYURTICIBEDEL","DOKUMANYURTDISIBEDEL"};

        /// <summary>
        /// Onay form tertip bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADONAYTERTIP = "HRCIHALEONAYTERTIP";

        /// <summary>
        /// Onay form tertip bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarOnayTertip
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
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,
            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 3,
            /// <summary>
            /// FONKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FONKOD = 4,
            /// <summary>
            /// FINKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FINKOD = 5,
            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 6,
            /// <summary>
            /// ODENEKTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENEKTUTAR = 7,
            /// <summary>
            /// BLOKETUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BLOKETUTAR = 8,
            /// <summary>
            /// PEBKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 9,
            /// <summary>
            /// KULLANILACAKTUTAR
            /// </summary>
            KULLANILACAKTUTAR = 10,
        }

        /// <summary>
        /// Onay form tertip tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARONAYTERTIP = { "YIL", "SURECNO", "SIRANO", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "ODENEKTUTAR", "BLOKETUTAR", "PEBKOD", "KULLANILACAKTUTAR" };

        /// <summary>
        /// Ödeme Emri Belge bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADODEMEEMRIFORM = "HRCODEMEEMRIFORM";

        /// <summary>
        /// Ödeme Emri Belge bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarOdemeEmriForm
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
            /// PRODEMEEMRINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRODEMEEMRINO = 4,
            /// <summary>
            /// PRFATURANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRFATURANO = 5,
            /// <summary>
            /// ODEMEEMRINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 6,
            /// <summary>
            /// ODEMEEMRITARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRITARIH = 7,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 8,
            /// <summary>
            /// BELGETUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGETUR = 9,
        }

        /// <summary>
        /// Ödeme Emri Belge tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARODEMEEMRIFORM = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "PRODEMEEMRINO", "PRFATURANO", "ODEMEEMRINO", "ODEMEEMRITARIH", "ACIKLAMA", "BELGETUR" };

        /// <summary>
        /// Ödeme Emri Belge Detay bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADODEMEEMRIDETAY = "HRCODEMEEMRIDETAY";

        /// <summary>
        /// Ödeme Emri Belge Detay bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarOdemeEmriDetay
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
            /// PRODEMEEMRINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRODEMEEMRINO = 4,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 5,
            /// <summary>
            /// HESAPPLANKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HESAPPLANKOD = 6,
            /// <summary>
            /// TERTIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TERTIP = 7,
            /// <summary>
            /// YANSITMAHESAPKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YANSITMAHESAPKOD = 8,
            /// <summary>
            /// BORC alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BORC = 9,
            /// <summary>
            /// ALACAK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ALACAK = 10
        }

        /// <summary>
        /// Ödeme Emri Belge Detay tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARODEMEEMRIDETAY = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "PRODEMEEMRINO", "SIRANO", "HESAPPLANKOD", "TERTIP", "YANSITMAHESAPKOD", "BORC", "ALACAK" };

        /// <summary>
        /// Teklif edilen fiyat bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADTEKLIFEDILENFIYAT = "HRCTEKLIFEDILENFIYAT";

        /// <summary>
        /// Teklif edilen fiyat bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarTeklifEdilenFiyat
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
            /// TARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TARIH = 4,
            /// <summary>
            /// SAAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SAAT = 5,
            /// <summary>
            /// TEKLIFTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFTUTAR = 6,
            /// <summary>
            /// FIYATDISITEKLIFTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIYATDISITEKLIFTUTAR = 7,
            /// <summary>
            /// YERLIMALITEKLIFTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YERLIMALITEKLIFTUTAR = 8,
            /// <summary>
            /// YERLIMALITEKLIFTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMAKODU = 9,
            /// <summary>
            /// TEKLIFTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFTUR = 10,
        }

        /// <summary>
        /// Teklif edilen fiyat tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARTEKLIFEDILENFIYAT = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TARIH", "SAAT", "TEKLIFTUTAR", "FIYATDISITEKLIFTUTAR", "YERLIMALITEKLIFTUTAR", "FIRMAKODU", "TEKLIFTUR" };


        /// <summary>
        /// Hakediş bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADHAKEDIS = "HRCHAKEDIS";

        /// <summary>
        /// Hakediş bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarHakedis
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
            /// SOZLESMEBEDEL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SOZLESMEBEDEL = 4,
            /// <summary>
            /// IHALETARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALETARIHI = 5,
            /// <summary>
            /// IHALEKARARTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEKARARTARIHI = 6,
            /// <summary>
            /// IHALEKARARNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEKARARNO = 7,
            /// <summary>
            /// SOZLESMETARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SOZLESMETARIHI = 8,
            /// <summary>
            /// SOZLESMENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SOZLESMENO = 9,
            /// <summary>
            /// TESLIMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TESLIMTARIHI = 10,
            /// <summary>
            /// ISINSURESI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISINSURESI = 11,
            /// <summary>
            /// BITIMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BITIMTARIHI = 12,
        }

        /// <summary>
        /// Hakediş bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARHAKEDIS = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "SOZLESMEBEDEL", "IHALETARIHI", "IHALEKARARTARIHI", "IHALEKARARNO", "SOZLESMETARIHI", "SOZLESMENO", "TESLIMTARIHI", "ISINSURESI", "BITIMTARIHI" };


        /// <summary>
        /// Fatura tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADFATURAFORM = "HRCFATURAFORM";

        /// <summary>
        /// Fatura tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarFaturaForm
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
            /// PRFATURANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRFATURANO = 4,
            /// <summary>
            /// FIRMANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMANO = 5,
            /// <summary>
            /// FATURANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURANO = 6,
            /// <summary>
            /// FATURATARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURATARIH = 7,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 8,
            /// <summary>
            /// TASINIRNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINIRNO = 9,
            /// <summary>
            /// ODEMEEMRINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 10,
            /// <summary>
            /// PERAMIRI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PERAMIRI = 11,
            /// <summary>
            /// PERAMIRIUNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PERAMIRIUNVAN = 12,
            /// <summary>
            /// GENELTOPLAM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GENELTOPLAM = 13,
            /// <summary>
            /// OIV alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OIV = 14,
            /// <summary>
            /// KDVTEVKIFATI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KDVTEVKIFATI = 15,
            /// <summary>
            /// DURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DURUM = 16,
            /// <summary>
            /// ISKONTO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISKONTO = 17,
            /// <summary>
            /// ISKONTOORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISKONTOORAN = 18,
            /// <summary>
            /// ODEMEEMRIYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRIYIL = 19,
            PARABIRIMI = 20,
        }

        /// <summary>
        /// Fatura tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARFATURAFORM = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "PRFATURANO", "FIRMANO", "FATURANO", "FATURATARIH", "ACIKLAMA", "TASINIRNO", "ODEMEEMRINO", "PERAMIRI", "PERAMIRIUNVAN", "GENELTOPLAM", "OIV", "KDVTEVKIFATI", "DURUM", "ISKONTO", "ISKONTOORAN", "ODEMEEMRIYIL", "PARABIRIMI" };



        /// <summary>
        /// Taşınır belge bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADTASINIRFORM = "HRCTASINIRFORM";

        /// <summary>
        /// Taşınır belge bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarTasinirForm
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
            /// PRTASINIRNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRTASINIRNO = 4,
            /// <summary>
            /// PRFATURANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRFATURANO = 5,
            /// <summary>
            /// TASINIRNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINIRNO = 6,
            /// <summary>
            /// TASINIRTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINIRTARIH = 7,
            /// <summary>
            /// HARCAMABIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 8,
            /// <summary>
            /// AMBARKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 9,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 10
        }

        /// <summary>
        /// Taşınır belge tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARTASINIRFORM = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "PRTASINIRNO", "PRFATURANO", "TASINIRNO", "TASINIRTARIH", "HARCAMABIRIMKOD", "AMBARKOD", "ACIKLAMA" };

        /// <summary>
        /// Taşınır belge detay bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADTASINIRDETAY = "HRCTASINIRDETAY";

        /// <summary>
        /// Taşınır belge detay bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarTasinirDetay
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
            /// PRTASINIRNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRTASINIRNO = 4,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 5,
            /// <summary>
            /// ADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADI = 6,
            /// <summary>
            /// HESAPPLANKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HESAPPLANKOD = 7,
            /// <summary>
            /// MIKTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 8,
            /// <summary>
            /// OLCUBIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OLCUBIRIMKOD = 9,
            /// <summary>
            /// KDVORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KDVORAN = 10,
            /// <summary>
            /// BIRIMFIYAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYAT = 11,
            /// <summary>
            /// OLUSTURULDU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OLUSTURULDU = 12,
            /// <summary>
            /// FATURADETAYSIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURADETAYSIRANO = 13
        }

        /// <summary>
        /// Taşınır belge detay tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARTASINIRDETAY = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "PRTASINIRNO", "SIRANO", "ADI", "HESAPPLANKOD", "MIKTAR", "OLCUBIRIMKOD", "KDVORAN", "BIRIMFIYAT", "OLUSTURULDU", "FATURADETAYSIRANO" };



        /// <summary>
        /// Süreç bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADSUREC = "HRCSUREC";

        /// <summary>
        /// Süreç bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSurec
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
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 2,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 3,
            /// <summary>
            /// ISLEMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMKOD = 4,
            /// <summary>
            /// SURECTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECTARIH = 5,
            /// <summary>
            /// SURECDURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECDURUM = 6,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 7,
            /// <summary>
            /// ISINADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISINADI = 8,
            /// <summary>
            /// YILLARASARIISLEMMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YILLARASARIISLEMMI = 9,
        }

        /// <summary>
        /// Süreç tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARSUREC = { "YIL", "SURECNO", "MUHASEBEKOD", "BIRIMKOD", "ISLEMKOD", "SURECTARIH", "SURECDURUM", "ACIKLAMA", "ISINADI", "YILLARASARIISLEMMI" };


        /// <summary>
        /// İhale ilan bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADIHALEILANBILGISI = "HRCIHALEILANBILGISI";

        /// <summary>
        /// İhale ilan bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarIhaleIlanBilgisi
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
            /// TARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TARIH = 4,
            /// <summary>
            /// TESLIMYERI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TESLIMYERI = 5,
            /// <summary>
            /// TESLIMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TESLIMTARIHI = 6,
            /// <summary>
            /// IHALEYERI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEYERI = 7,
            /// <summary>
            /// IHALETARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALETARIHI = 8,
            /// <summary>
            /// IHALESAATI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALESAATI = 9,
            /// <summary>
            /// IHALETEMINYERI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALETEMINYERI = 10,
            /// <summary>
            /// TEKLIFSURESI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFSURESI = 11,
            /// <summary>
            /// ISSURESI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISSURESI = 12,
            /// <summary>
            /// TEKLIFTURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFTURU = 13,
            /// <summary>
            /// TEKLIFBELIRLEMETURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFBELIRLEMETURU = 14,
            /// <summary>
            /// KATILIMTURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KATILIMTURU = 15,
            /// <summary>
            /// INDIRIMORANI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            INDIRIMORANI = 16,
            /// <summary>
            /// KONSORSIYUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KONSORSIYUM = 17,
            /// <summary>
            /// ILANADEDI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILANADEDI = 18,
            /// <summary>
            /// ILANSEKLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILANSEKLI = 19,
            /// <summary>
            /// KAYNAKTURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KAYNAKTURU = 20,
            /// <summary>
            /// KISMITEKLIFEACIKMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KISMITEKLIFEACIKMI = 21,
            /// <summary>
            /// SEKTORKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SEKTORKODU = 22,
            /// <summary>
            /// SEKTORADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SEKTORADI = 23,
            /// <summary>
            /// YMAKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YMAKOD = 24,
            /// <summary>
            /// YMABASLANGICTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YMABASLANGICTARIHI = 25,
            /// <summary>
            /// YMABITISTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YMABITISTARIHI = 26,
            /// <summary>
            /// YMAACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YMAACIKLAMA = 27,
            /// <summary>
            /// ONYETERLIKDAVETTURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONYETERLIKDAVETTURU = 28,
            /// <summary>
            /// ONYETERLIKDAVETSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONYETERLIKDAVETSAYI = 29,
        }

        /// <summary>
        /// İhale ilan bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARIHALEILANBILGISI = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TARIH", "TESLIMYERI", "TESLIMTARIHI", "IHALEYERI", "IHALETARIHI", "IHALESAATI", "IHALETEMINYERI", "TEKLIFSURESI", "ISSURESI", "TEKLIFTURU", "TEKLIFBELIRLEMETURU", "KATILIMTURU", "INDIRIMORANI", "KONSORSIYUM", "ILANADEDI", "ILANSEKLI", "KAYNAKTURU", "KISMITEKLIFEACIKMI", "SEKTORKODU", "SEKTORADI", "YMAKOD", "YMABASLANGICTARIHI", "YMABITISTARIHI", "YMAACIKLAMA", "ONYETERLIKDAVETTURU", "ONYETERLIKDAVETSAYI" };

        /// <summary>
        /// İhaleye teklif vermesi için firmaya yapılan davet bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADIHALENO = "HRCIHALENUMARASI";

        /// <summary>
        /// İhale numarası bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarIhaleNo
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
            /// IHALENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALENO = 4,
        }

        /// <summary>
        /// İhale numarası bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARIHALENO = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "IHALENO" };



        /// <summary>
        /// İhale sözleşme sonu ücret bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADSOZLESMESONUUCRET = "HRCIHALESOZLESMESONUUCRET";

        /// <summary>
        /// İhale sözleşme sonu ücret bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarSozlesmeSonuUcret
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
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 4,
            /// <summary>
            /// TUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUTAR = 5,
            /// <summary>
            /// BELGETARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGETARIHI = 6,
            /// <summary>
            /// BELGENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGENO = 7,
        }

        /// <summary>
        /// İhale sözleşme sonu ücret tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARSOZLESMESONUUCRET = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "SIRANO", "TUTAR", "BELGETARIHI", "BELGENO" };


        #endregion

        public string SurecYilSurecNoQueryOlustur(SurecSorguKriter bilgi)
        {
            string sql = "";
            string select = "";
            string from = "";
            string innerJoin = "";
            string where = "";
            string kosul = "";
            string orderBy = "";

            //SELECT//
            select += "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL]
                          + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO];
            //--------------------->

            //FROM//
            from += " FROM " + TABLOAD;

            innerJoin += " INNER JOIN " + TABLOADMUHASEBEVIEW + " ON " +
                        TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + TABLOADMUHASEBEVIEW + "." + ALANLARMUHASEBEVIEW[(int)ENUMAlanlarMUHASEBEVIEW.MUHASEBEKOD];

            innerJoin += " INNER JOIN " + TABLOADIS + " ON " +
                        TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEMKOD] + "=" + TABLOADIS + "." + ALANLARIS[(int)ENUMAlanlarIS.KOD];

            //innerJoin += " LEFT OUTER JOIN " + TABLOADDURUM + " ON " +
            //            TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADDURUM + "." + ALANLARDURUM[(int)ENUMAlanlarDurum.YIL]
            //            + " AND " +
            //            TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADDURUM + "." + ALANLARDURUM[(int)ENUMAlanlarDurum.SURECNO]
            //            + " AND " +
            //            TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEMKOD] + "=" + TABLOADDURUM + "." + ALANLARDURUM[(int)ENUMAlanlarDurum.ISLEMKOD];
            //--------------------->

            //WHERE//
            kosul = "";
            if (bilgi.yil > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
                //kosul = VeriErisim.KosulEkle(kosul, "OR", "(HRCSUREC.YILLARASARIISLEMMI>0 AND HRCSUREC.YIL<" + bilgi.yil + "))");

            }
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
            {
                string[] birimler = bilgi.birim.Split(';');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                {
                    if (birimler[i].Trim() != "")
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimler[i].Trim()));
                }
                if (birimKosul != "")
                    kosul += " AND (" + birimKosul + ")";
            }
            if (bilgi.surecDurum > 0)
            {
                if (bilgi.surecDurum <= 2)
                {
                    string kosulSurec = "";
                    kosulSurec = VeriErisim.KosulEkle(kosulSurec, "", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECDURUM], "=", (int)ENUMSurecDurum.YENI));
                    kosulSurec = VeriErisim.KosulEkle(kosulSurec, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECDURUM], "=", (int)ENUMSurecDurum.DEVAMEDIYOR));
                    kosul += " AND (" + kosulSurec + ")";
                }
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECDURUM], "=", bilgi.surecDurum));
            }
            else
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));
            if (bilgi.islem.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMKOD], "=", bilgi.islem.kod));
            if (bilgi.isinAdi != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISINADI], "LIKE", "%" + bilgi.isinAdi + "%"));
            if (bilgi.surecNoBasla != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], ">=", bilgi.surecNoBasla));
            if (bilgi.surecNoBitis != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "<=", bilgi.surecNoBitis));
            if (!bilgi.surecTarihBasla.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECTARIH], ">=", bilgi.surecTarihBasla));
            if (!bilgi.surecTarihBitis.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECTARIH], "<=", bilgi.surecTarihBitis));
            //if (bilgi.parcaDurum.parcaKod > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDURUM, ALANLARDURUM[(int)ENUMAlanlarDurum.PARCAKOD], "=", bilgi.parcaDurum.parcaKod));
            //if (bilgi.parcaDurum.durum > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDURUM, ALANLARDURUM[(int)ENUMAlanlarDurum.DURUM], "=", bilgi.parcaDurum.durum));
            where += " WHERE " + kosul;
            //---------------------->

            //ORDERBY//
            orderBy += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SURECNO] + "," + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + ALANLAR[(int)ENUMAlanlar.BIRIMKOD];
            //----------------------->


            ////////Yolluklar----------------------->
            if (bilgi.islem.kod == 0)
            {
                if (bilgi.tckimlikno != "")
                {
                    innerJoin += " LEFT OUTER JOIN " + TABLOADYOLLUKLARVIEW + " ON " +
                                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADYOLLUKLARVIEW + "." + ALANLARYOLLUKLARVIEW[(int)ENUMAlanlarYOLLUKLARVIEW.YIL]
                                + " AND " +
                                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + TABLOADYOLLUKLARVIEW + "." + ALANLARYOLLUKLARVIEW[(int)ENUMAlanlarYOLLUKLARVIEW.MUHASEBEKOD]
                                + " AND " +
                                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + "=" + TABLOADYOLLUKLARVIEW + "." + ALANLARYOLLUKLARVIEW[(int)ENUMAlanlarYOLLUKLARVIEW.BIRIMKOD]
                                + " AND " +
                                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADYOLLUKLARVIEW + "." + ALANLARYOLLUKLARVIEW[(int)ENUMAlanlarYOLLUKLARVIEW.SURECNO];

                    where = VeriErisim.KosulEkle(where, "AND", VeriErisim.KosulOl(TABLOADYOLLUKLARVIEW, ALANLARYOLLUKLARVIEW[(int)ENUMAlanlarYOLLUKLARVIEW.TCKIMLIKNO], "=", bilgi.tckimlikno));
                }
            }
            //Geçici Görev Yolluk
            else if (bilgi.islem.kod == 24 || bilgi.islem.kod == 25 || bilgi.islem.kod == 26)
            {
                if (bilgi.tckimlikno != "")
                {
                    innerJoin += " LEFT OUTER JOIN " + TABLOADGECICIGOREVYOLLUKBELGE + " ON " +
                                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADGECICIGOREVYOLLUKBELGE + "." + ALANLARGECICIGOREVYOLLUKBELGE[(int)ENUMAlanlarGECICIGOREVYOLLUKBELGE.YIL]
                                + " AND " +
                                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + TABLOADGECICIGOREVYOLLUKBELGE + "." + ALANLARGECICIGOREVYOLLUKBELGE[(int)ENUMAlanlarGECICIGOREVYOLLUKBELGE.MUHASEBEKOD]
                                + " AND " +
                                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + "=" + TABLOADGECICIGOREVYOLLUKBELGE + "." + ALANLARGECICIGOREVYOLLUKBELGE[(int)ENUMAlanlarGECICIGOREVYOLLUKBELGE.BIRIMKOD]
                                + " AND " +
                                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADGECICIGOREVYOLLUKBELGE + "." + ALANLARGECICIGOREVYOLLUKBELGE[(int)ENUMAlanlarGECICIGOREVYOLLUKBELGE.SURECNO];

                    where = VeriErisim.KosulEkle(where, "AND", VeriErisim.KosulOl(TABLOADGECICIGOREVYOLLUKBELGE, ALANLARGECICIGOREVYOLLUKBELGE[(int)ENUMAlanlarGECICIGOREVYOLLUKBELGE.TCKIMLIKNO], "=", bilgi.tckimlikno));
                }
            }
            //Sürekli Görev Yolluk
            else if (bilgi.islem.kod == 12 || bilgi.islem.kod == 13)
            {
                if (bilgi.tckimlikno != "")
                {
                    innerJoin += " LEFT OUTER JOIN " + TABLOADSUREKLIGOREVYOLLUKBELGE + " ON " +
                                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADSUREKLIGOREVYOLLUKBELGE + "." + ALANLARSUREKLIGOREVYOLLUKBELGE[(int)ENUMAlanlarSUREKLIGOREVYOLLUKBELGE.YIL]
                                + " AND " +
                                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + TABLOADSUREKLIGOREVYOLLUKBELGE + "." + ALANLARSUREKLIGOREVYOLLUKBELGE[(int)ENUMAlanlarSUREKLIGOREVYOLLUKBELGE.MUHASEBEKOD]
                                + " AND " +
                                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + "=" + TABLOADSUREKLIGOREVYOLLUKBELGE + "." + ALANLARSUREKLIGOREVYOLLUKBELGE[(int)ENUMAlanlarSUREKLIGOREVYOLLUKBELGE.BIRIMKOD]
                                + " AND " +
                                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADSUREKLIGOREVYOLLUKBELGE + "." + ALANLARSUREKLIGOREVYOLLUKBELGE[(int)ENUMAlanlarSUREKLIGOREVYOLLUKBELGE.SURECNO];

                    where = VeriErisim.KosulEkle(where, "AND", VeriErisim.KosulOl(TABLOADSUREKLIGOREVYOLLUKBELGE, ALANLARSUREKLIGOREVYOLLUKBELGE[(int)ENUMAlanlarSUREKLIGOREVYOLLUKBELGE.TCKIMLIKNO], "=", bilgi.tckimlikno));
                }
            }
            ////////-------------------------------->


            if (bilgi.odemeEmriNo != "")
            {
                innerJoin += " LEFT OUTER JOIN " + TABLOADMUHMIFBAG + " ON " +
                            TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADMUHMIFBAG + "." + ALANLARMUHMIFBAG[(int)ENUMAlanlarMUHMIFBAG.YIL]
                            + " AND " +
                            TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + TABLOADMUHMIFBAG + "." + ALANLARMUHMIFBAG[(int)ENUMAlanlarMUHMIFBAG.MUHASEBEKOD]
                            + " AND " +
                            TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + "=" + TABLOADMUHMIFBAG + "." + ALANLARMUHMIFBAG[(int)ENUMAlanlarMUHMIFBAG.BIRIMKOD]
                            + " AND " +
                            TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADMUHMIFBAG + "." + ALANLARMUHMIFBAG[(int)ENUMAlanlarMUHMIFBAG.SURECNO];

                where = VeriErisim.KosulEkle(where, "AND", VeriErisim.KosulOl(TABLOADMUHMIFBAG, ALANLARMUHMIFBAG[(int)ENUMAlanlarMUHMIFBAG.ODEMEEMRINO], "=", bilgi.odemeEmriNo));
            }

            if (bilgi.ihaleUsulu > 0)
            {
                innerJoin += " LEFT OUTER JOIN " + TABLOADONAYFORM + " ON " +
                                           TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADONAYFORM + "." + ALANLARONAYFORM[(int)ENUMAlanlarONAYFORM.YIL]
                                           + " AND " +
                                           TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + TABLOADONAYFORM + "." + ALANLARONAYFORM[(int)ENUMAlanlarONAYFORM.MUHASEBEKOD]
                                           + " AND " +
                                           TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + "=" + TABLOADONAYFORM + "." + ALANLARONAYFORM[(int)ENUMAlanlarONAYFORM.BIRIMKOD]
                                           + " AND " +
                                           TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADONAYFORM + "." + ALANLARONAYFORM[(int)ENUMAlanlarONAYFORM.SURECNO];

                where = VeriErisim.KosulEkle(where, "AND", VeriErisim.KosulOl(TABLOADONAYFORM, ALANLARONAYFORM[(int)ENUMAlanlarONAYFORM.IHALEUSULU], "=", bilgi.ihaleUsulu));
            }

            sql = select + from + innerJoin + where;// +orderBy;
            return sql;
        }

        public IDataReader SurecYilSurecNoListele(SurecSorguKriter bilgi)
        {
            return veriErisim.DataReaderDondur(SurecYilSurecNoQueryOlustur(bilgi));
        }

        public IDataReader SurecBilgiListele(SurecSorguKriter bilgi)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADSUREC, ALANLARSUREC) + "," + VeriErisim.TumAlanlar(TABLOADIS, ALANLARIS) + " FROM " + TABLOADSUREC + "," + TABLOADIS;
            sql += " WHERE " + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.ISLEMKOD] + "=" + TABLOADIS + "." + ALANLARIS[(int)ENUMAlanlarIS.KOD] + " AND ";
            sql += " EXISTS ";
            sql += "(";
            sql += "SELECT 1 FROM (" + SurecYilSurecNoQueryOlustur(bilgi) + ") AS A WHERE ";
            sql += "A.YIL=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.YIL];
            sql += " AND ";
            sql += "A.SURECNO=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO];
            sql += ")";
            sql += " ORDER BY ";
            sql += TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.YIL] + ",";
            sql += TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.MUHASEBEKOD] + ",";
            sql += TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.BIRIMKOD] + ",";
            sql += TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO];
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader OnayBelgesiBilgiListele(SurecSorguKriter bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARONAY) + " FROM " + TABLOADONAY;
            sql += " WHERE ";
            sql += " EXISTS ";
            sql += "(";
            sql += "SELECT 1 FROM (" + SurecYilSurecNoQueryOlustur(bilgi) + ") AS A WHERE ";
            sql += "A.YIL=" + TABLOADONAY + "." + ALANLARONAY[(int)ENUMAlanlarOnay.YIL];
            sql += " AND ";
            sql += "A.SURECNO=" + TABLOADONAY + "." + ALANLARONAY[(int)ENUMAlanlarOnay.SURECNO];
            sql += ")";
            sql += " ORDER BY ";
            sql += TABLOADONAY + "." + ALANLARONAY[(int)ENUMAlanlarOnay.YIL] + ",";
            sql += TABLOADONAY + "." + ALANLARONAY[(int)ENUMAlanlarOnay.MUHASEBEKOD] + ",";
            sql += TABLOADONAY + "." + ALANLARONAY[(int)ENUMAlanlarOnay.BIRIMKOD] + ",";
            sql += TABLOADONAY + "." + ALANLARONAY[(int)ENUMAlanlarOnay.SURECNO];
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader OnayBelgesiTertipBilgiListele(SurecSorguKriter bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARONAYTERTIP) + " FROM " + TABLOADONAYTERTIP;
            sql += " WHERE ";
            sql += " EXISTS ";
            sql += "(";
            sql += "SELECT 1 FROM (" + SurecYilSurecNoQueryOlustur(bilgi) + ") AS A WHERE ";
            sql += "A.YIL=" + TABLOADONAYTERTIP + "." + ALANLARONAYTERTIP[(int)ENUMAlanlarOnayTertip.YIL];
            sql += " AND ";
            sql += "A.SURECNO=" + TABLOADONAYTERTIP + "." + ALANLARONAYTERTIP[(int)ENUMAlanlarOnayTertip.SURECNO];
            sql += ")";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader AvansBelgesiBilgiListele(SurecSorguKriter bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARODEMEEMRIFORM) + " FROM " + TABLOADODEMEEMRIFORM;
            sql += " WHERE ";
            sql += " EXISTS ";
            sql += "(";
            sql += "SELECT 1 FROM (" + SurecYilSurecNoQueryOlustur(bilgi) + ") AS A WHERE ";
            sql += "A.YIL=" + TABLOADODEMEEMRIFORM + "." + ALANLARODEMEEMRIFORM[(int)ENUMAlanlarOdemeEmriForm.YIL];
            sql += " AND ";
            sql += "A.SURECNO=" + TABLOADODEMEEMRIFORM + "." + ALANLARODEMEEMRIFORM[(int)ENUMAlanlarOdemeEmriForm.SURECNO];
            sql += ")";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader IhaleIlanBilgiListele(SurecSorguKriter bilgi)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADIHALEILANBILGISI, ALANLARIHALEILANBILGISI) + "," + TABLOADIHALENO + "." + ALANLARIHALENO[(int)ENUMAlanlarIhaleNo.IHALENO] +
                     " FROM " + TABLOADIHALEILANBILGISI + " LEFT OUTER JOIN " + TABLOADIHALENO + " ON " +
                     TABLOADIHALEILANBILGISI + "." + ALANLARIHALEILANBILGISI[(int)ENUMAlanlarIhaleIlanBilgisi.YIL] + " = " + TABLOADIHALENO + "." + ALANLARIHALENO[(int)ENUMAlanlarIhaleNo.YIL] + " AND " +
                     TABLOADIHALEILANBILGISI + "." + ALANLARIHALEILANBILGISI[(int)ENUMAlanlarIhaleIlanBilgisi.SURECNO] + " = " + TABLOADIHALENO + "." + ALANLARIHALENO[(int)ENUMAlanlarIhaleNo.SURECNO] + " AND ";
            sql += " EXISTS ";
            sql += "(";
            sql += "SELECT 1 FROM (" + SurecYilSurecNoQueryOlustur(bilgi) + ") AS A WHERE ";
            sql += "A.YIL=" + TABLOADIHALEILANBILGISI + "." + ALANLARIHALEILANBILGISI[(int)ENUMAlanlarIhaleIlanBilgisi.YIL];
            sql += " AND ";
            sql += "A.SURECNO=" + TABLOADIHALEILANBILGISI + "." + ALANLARIHALEILANBILGISI[(int)ENUMAlanlarIhaleIlanBilgisi.SURECNO];
            sql += ")";
            sql += " ORDER BY ";
            sql += TABLOADIHALEILANBILGISI + "." + ALANLARIHALEILANBILGISI[(int)ENUMAlanlarIhaleIlanBilgisi.YIL] + ",";
            sql += TABLOADIHALEILANBILGISI + "." + ALANLARIHALEILANBILGISI[(int)ENUMAlanlarIhaleIlanBilgisi.MUHASEBEKOD] + ",";
            sql += TABLOADIHALEILANBILGISI + "." + ALANLARIHALEILANBILGISI[(int)ENUMAlanlarIhaleIlanBilgisi.BIRIMKOD] + ",";
            sql += TABLOADIHALEILANBILGISI + "." + ALANLARIHALEILANBILGISI[(int)ENUMAlanlarIhaleIlanBilgisi.SURECNO];
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader TeklifEdilenFiyatBilgiListele(SurecSorguKriter bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTEKLIFEDILENFIYAT, ALANLARTEKLIFEDILENFIYAT) + "," +
                        TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.AD] +
                        ", HRCKAZANANFIRMA.FIRMAKOD1, HRCFIRMABILGISI_1.VERGINO AS FIRMA1VERGINO, HRCFIRMABILGISI_1.AD AS FIRMA1AD, HRCKAZANANFIRMA.FIRMAKOD2 " +
                //" FROM " + TABLOADTEKLIFEDILENFIYAT +
                //" LEFT OUTER JOIN " + TABFirmaBilgisi.TABLOAD +
                //" ON " + TABLOADTEKLIFEDILENFIYAT + "." + ALANLARTEKLIFEDILENFIYAT[(int)ENUMAlanlarTeklifEdilenFiyat.FIRMAKODU] + " = " + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.KOD];
                   " FROM         HRCFIRMABILGISI RIGHT OUTER JOIN " +
                   "    HRCTEKLIFEDILENFIYAT ON HRCFIRMABILGISI.MUHASEBEKOD = HRCTEKLIFEDILENFIYAT.MUHASEBEKOD AND  " +
                    "   HRCFIRMABILGISI.KOD = HRCTEKLIFEDILENFIYAT.FIRMAKODU LEFT OUTER JOIN " +
                     "  HRCFIRMABILGISI AS HRCFIRMABILGISI_1 RIGHT OUTER JOIN " +
                     "  HRCKAZANANFIRMA ON HRCFIRMABILGISI_1.MUHASEBEKOD = HRCKAZANANFIRMA.MUHASEBEKOD AND  " +
                     "  HRCFIRMABILGISI_1.KOD = HRCKAZANANFIRMA.FIRMAKOD1 ON HRCTEKLIFEDILENFIYAT.YIL = HRCKAZANANFIRMA.YIL AND " +
                     "  HRCTEKLIFEDILENFIYAT.SURECNO = HRCKAZANANFIRMA.SURECNO ";
            sql += " WHERE ";
            sql += " EXISTS ";
            sql += "(";
            sql += "SELECT 1 FROM (" + SurecYilSurecNoQueryOlustur(bilgi) + ") AS A WHERE ";
            sql += "A.YIL=" + TABLOADTEKLIFEDILENFIYAT + "." + ALANLARTEKLIFEDILENFIYAT[(int)ENUMAlanlarTeklifEdilenFiyat.YIL];
            sql += " AND ";
            sql += "A.SURECNO=" + TABLOADTEKLIFEDILENFIYAT + "." + ALANLARTEKLIFEDILENFIYAT[(int)ENUMAlanlarTeklifEdilenFiyat.SURECNO];
            sql += ")";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader PiyasaFiyatTeklifBilgiListele(SurecSorguKriter bilgi)
        {
            string sql = "";
            sql = "SELECT HRCTEKLIF.*, HRCTEKLIFDONEM.TARIH, HRCTEKLIFMALZEME.ADI, HRCTEKLIFMALZEME.MIKTAR, HRCTEKLIFMALZEME.OLCUBIRIMKOD,HRCOLCUBIRIM.AD,HRCTEKLIFMALZEME.MALZEMEKOD";
            sql += ", HRCFIRMABILGISI.AD, HRCFIRMABILGISI.ADRES, HRCFIRMABILGISI.KOD, HRCFIRMABILGISI.VERGINO ";
            sql += " FROM HRCFIRMABILGISI INNER JOIN";
            sql += " HRCTEKLIF ON HRCFIRMABILGISI.KOD = HRCTEKLIF.FIRMANO AND HRCFIRMABILGISI.MUHASEBEKOD = HRCTEKLIF.MUHASEBEKOD LEFT OUTER JOIN";
            sql += " HRCOLCUBIRIM INNER JOIN";
            sql += " HRCTEKLIFMALZEME ON HRCOLCUBIRIM.KOD = HRCTEKLIFMALZEME.OLCUBIRIMKOD ON HRCTEKLIF.YIL = HRCTEKLIFMALZEME.YIL AND ";
            sql += " HRCTEKLIF.SURECNO = HRCTEKLIFMALZEME.SURECNO AND HRCTEKLIF.MALZEMENO = HRCTEKLIFMALZEME.MALZEMENO AND ";
            sql += " HRCTEKLIF.TIP = HRCTEKLIFMALZEME.TIP LEFT OUTER JOIN";
            sql += " HRCTEKLIFDONEM ON HRCTEKLIF.YIL = HRCTEKLIFDONEM.YIL AND HRCTEKLIF.SURECNO = HRCTEKLIFDONEM.SURECNO AND ";
            sql += " HRCTEKLIF.DONEMNO = HRCTEKLIFDONEM.DONEMNO LEFT OUTER JOIN";
            sql += " HRCTEKLIFFIRMA ON HRCTEKLIF.YIL = HRCTEKLIFFIRMA.YIL AND HRCTEKLIF.SURECNO = HRCTEKLIFFIRMA.SURECNO AND ";
            sql += " HRCTEKLIF.TIP = HRCTEKLIFFIRMA.TIP AND HRCTEKLIF.FIRMANO = HRCTEKLIFFIRMA.FIRMANO";

            sql += " WHERE HRCTEKLIF.TIP = 3 AND ";
            sql += " EXISTS ";
            sql += "(";
            sql += "SELECT 1 FROM (" + SurecYilSurecNoQueryOlustur(bilgi) + ") AS A WHERE ";
            sql += "A.YIL = HRCTEKLIF.YIL";
            sql += " AND ";
            sql += "A.SURECNO = HRCTEKLIF.SURECNO";
            sql += ")";
            sql += " ORDER BY HRCTEKLIF.YIL, HRCTEKLIF.MUHASEBEKOD, HRCTEKLIF.BIRIMKOD, HRCTEKLIF.SURECNO";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader PiyasaFiyatToplamTeklifBilgiListele(SurecSorguKriter bilgi)
        {
            string sql = "";

            sql += "  SELECT PIYASAFIYATTEKLIF.YIL, PIYASAFIYATTEKLIF.SURECNO,PIYASAFIYATTEKLIF.VERGINO, PIYASAFIYATTEKLIF.FIRMAAD, ";
            sql += " PIYASAFIYATTEKLIF.KAZANAN, SUM(PIYASAFIYATTEKLIF.SATIRTOPLAM) AS TEKLIFTOPLAM ";

            sql += " FROM ";
            sql += " (SELECT     HRCTEKLIF.YIL, HRCTEKLIF.SURECNO,  ";
            sql += " HRCFIRMABILGISI.VERGINO, HRCFIRMABILGISI.AD AS FIRMAAD, HRCTEKLIF.KAZANAN,(HRCTEKLIFMALZEME.MIKTAR * HRCTEKLIF.BIRIMFIYAT) AS SATIRTOPLAM ";
            sql += " FROM HRCFIRMABILGISI INNER JOIN ";
            sql += " HRCTEKLIF ON HRCFIRMABILGISI.KOD = HRCTEKLIF.FIRMANO AND HRCFIRMABILGISI.MUHASEBEKOD = HRCTEKLIF.MUHASEBEKOD LEFT OUTER JOIN ";
            sql += " HRCTEKLIFMALZEME ON HRCTEKLIF.YIL = HRCTEKLIFMALZEME.YIL AND HRCTEKLIF.SURECNO = HRCTEKLIFMALZEME.SURECNO AND  ";
            sql += " HRCTEKLIF.MALZEMENO = HRCTEKLIFMALZEME.MALZEMENO AND HRCTEKLIF.TIP = HRCTEKLIFMALZEME.TIP LEFT OUTER JOIN ";
            sql += " HRCTEKLIFFIRMA ON HRCTEKLIF.YIL = HRCTEKLIFFIRMA.YIL AND HRCTEKLIF.SURECNO = HRCTEKLIFFIRMA.SURECNO AND  ";
            sql += " HRCTEKLIF.TIP = HRCTEKLIFFIRMA.TIP AND HRCTEKLIF.FIRMANO = HRCTEKLIFFIRMA.FIRMANO ";
            sql += " WHERE HRCTEKLIF.TIP = 3 AND  EXISTS  ";

            sql += "(SELECT 1 FROM (" + SurecYilSurecNoQueryOlustur(bilgi) + ") AS A WHERE ";
            sql += "A.YIL = HRCTEKLIF.YIL";
            sql += " AND ";
            sql += "A.SURECNO = HRCTEKLIF.SURECNO";
            sql += ")";

            sql += " GROUP BY   HRCTEKLIF.YIL, HRCTEKLIF.SURECNO, HRCFIRMABILGISI.AD, ";
            sql += " HRCFIRMABILGISI.VERGINO, HRCTEKLIF.KAZANAN,HRCTEKLIFMALZEME.MIKTAR,HRCTEKLIF.BIRIMFIYAT ";
            sql += " ) AS PIYASAFIYATTEKLIF ";
            sql += " GROUP BY PIYASAFIYATTEKLIF.YIL, PIYASAFIYATTEKLIF.SURECNO,PIYASAFIYATTEKLIF.VERGINO, PIYASAFIYATTEKLIF.FIRMAAD,PIYASAFIYATTEKLIF.KAZANAN ";
            sql += " ORDER BY PIYASAFIYATTEKLIF.YIL, PIYASAFIYATTEKLIF.SURECNO,PIYASAFIYATTEKLIF.KAZANAN DESC,PIYASAFIYATTEKLIF.VERGINO ";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KazananFirmaBilgiListele(SurecSorguKriter bilgi)
        {
            StringBuilder sqlIhaleKazanan = new StringBuilder();
            sqlIhaleKazanan.Append("SELECT ");
            sqlIhaleKazanan.Append("HRCKAZANANFIRMA.YIL, HRCKAZANANFIRMA.MUHASEBEKOD, HRCKAZANANFIRMA.BIRIMKOD, HRCKAZANANFIRMA.SURECNO, HRCKAZANANFIRMA.FIRMAKOD1, ");
            sqlIhaleKazanan.Append("HRCKAZANANFIRMA.FIRMAKOD2, HRCFIRMABILGISI_1.VERGINO AS FIRMA1VERGINO, HRCFIRMABILGISI_1.AD AS FIRMA1AD,  ");
            sqlIhaleKazanan.Append("HRCFIRMABILGISI.VERGINO AS FIRMA2VERGINO, HRCFIRMABILGISI.AD AS FIRMA2AD ");
            sqlIhaleKazanan.Append("FROM ");
            sqlIhaleKazanan.Append("HRCKAZANANFIRMA INNER JOIN ");
            sqlIhaleKazanan.Append("HRCSUREC ON HRCKAZANANFIRMA.YIL = HRCSUREC.YIL AND HRCKAZANANFIRMA.SURECNO = HRCSUREC.SURECNO LEFT OUTER JOIN ");
            sqlIhaleKazanan.Append("HRCFIRMABILGISI ON HRCKAZANANFIRMA.MUHASEBEKOD = HRCFIRMABILGISI.MUHASEBEKOD AND  ");
            sqlIhaleKazanan.Append("HRCKAZANANFIRMA.FIRMAKOD2 = HRCFIRMABILGISI.KOD LEFT OUTER JOIN ");
            sqlIhaleKazanan.Append("HRCFIRMABILGISI AS HRCFIRMABILGISI_1 ON HRCKAZANANFIRMA.MUHASEBEKOD = HRCFIRMABILGISI_1.MUHASEBEKOD AND  ");
            sqlIhaleKazanan.Append("HRCKAZANANFIRMA.FIRMAKOD1 = HRCFIRMABILGISI_1.KOD ");
            sqlIhaleKazanan.Append("WHERE ");
            sqlIhaleKazanan.Append("(HRCSUREC.SURECDURUM <> 9) ");

            StringBuilder sqlPiyasaFiyatKazanan = new StringBuilder();
            sqlPiyasaFiyatKazanan.Append("SELECT DISTINCT  ");
            sqlPiyasaFiyatKazanan.Append("HRCTEKLIF.YIL, HRCTEKLIF.MUHASEBEKOD, HRCTEKLIF.BIRIMKOD, HRCTEKLIF.SURECNO, HRCTEKLIF.FIRMANO AS FIRMAKOD1, NULL AS FIRMAKOD2,  ");
            sqlPiyasaFiyatKazanan.Append("HRCFIRMABILGISI.VERGINO AS FIRMA1VERGINO, HRCFIRMABILGISI.AD AS FIRMA1AD, NULL AS FIRMA2VERGINO, NULL AS FIRMA2AD ");
            sqlPiyasaFiyatKazanan.Append("FROM ");
            sqlPiyasaFiyatKazanan.Append("HRCTEKLIF INNER JOIN ");
            sqlPiyasaFiyatKazanan.Append("HRCSUREC ON HRCTEKLIF.YIL = HRCSUREC.YIL AND HRCTEKLIF.SURECNO = HRCSUREC.SURECNO INNER JOIN ");
            sqlPiyasaFiyatKazanan.Append("HRCFIRMABILGISI ON HRCTEKLIF.MUHASEBEKOD = HRCFIRMABILGISI.MUHASEBEKOD AND HRCTEKLIF.FIRMANO = HRCFIRMABILGISI.KOD ");
            sqlPiyasaFiyatKazanan.Append("WHERE ");
            sqlPiyasaFiyatKazanan.Append("(HRCTEKLIF.TIP = 3) AND (HRCTEKLIF.KAZANAN = 1) AND (HRCSUREC.SURECDURUM <> 9) ");

            StringBuilder sqlKazanan = new StringBuilder();
            sqlKazanan.Append("(");
            sqlKazanan.Append(sqlIhaleKazanan);
            sqlKazanan.Append(" UNION ");
            sqlKazanan.Append(sqlPiyasaFiyatKazanan);
            sqlKazanan.Append(")");

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT * FROM ");
            sql.Append(sqlKazanan);
            sql.Append(" AS KAZANAN ");
            sql.Append("WHERE ");
            sql.Append("EXISTS ");
            sql.Append("( ");
            sql.Append("SELECT 1 FROM (" + SurecYilSurecNoQueryOlustur(bilgi) + ") AS A WHERE ");
            sql.Append("KAZANAN.YIL = A.YIL ");
            sql.Append("AND ");
            sql.Append("KAZANAN.SURECNO = A.SURECNO ");
            sql.Append(")");

            return veriErisim.DataReaderDondur(sql.ToString());
        }

        public IDataReader HakedisBilgiListele(SurecSorguKriter bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARHAKEDIS) + " FROM " + TABLOADHAKEDIS;
            sql += " WHERE ";
            sql += " EXISTS ";
            sql += "(";
            sql += "SELECT 1 FROM (" + SurecYilSurecNoQueryOlustur(bilgi) + ") AS A WHERE ";
            sql += "A.YIL = " + TABLOADHAKEDIS + "." + ALANLARHAKEDIS[(int)ENUMAlanlarHakedis.YIL];
            sql += " AND ";
            sql += "A.SURECNO = " + TABLOADHAKEDIS + "." + ALANLARHAKEDIS[(int)ENUMAlanlarHakedis.SURECNO];
            sql += ")";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader FaturaBilgiListele(SurecSorguKriter bilgi)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADFATURAFORM, ALANLARFATURAFORM);
            sql += ",HRCFIRMABILGISI.KOD, HRCFIRMABILGISI.MUHASEBEKOD, HRCFIRMABILGISI.BIRIMKOD, HRCFIRMABILGISI.AD";
            sql += ",HRCFIRMABILGISI.VERGINO, HRCFIRMABILGISI.VERGIDAIRESI, HRCFIRMABILGISI.BANKA, HRCFIRMABILGISI.HESAPNO";
            sql += ",HRCFIRMABILGISI.EMAIL, HRCFIRMABILGISI.FIRMAYETKILISI";
            sql += ",HRCFIRMABILGISI.TELEFON, HRCFIRMABILGISI.FAKS, HRCFIRMABILGISI.ADRES, HRCFIRMABILGISI.UYRUK, HRCFIRMABILGISI.GORULMESIN";
            sql += ",HRCFIRMABILGISI.YASAKLI, HRCFIRMABILGISI.CEPTEL, HRCFIRMABILGISI.WEB";
            sql += " FROM " + TABLOADFATURAFORM;
            sql += " LEFT OUTER JOIN";
            sql += " HRCFIRMABILGISI ON HRCFATURAFORM.MUHASEBEKOD = HRCFIRMABILGISI.MUHASEBEKOD AND HRCFATURAFORM.FIRMANO = HRCFIRMABILGISI.KOD ";


            sql += " WHERE ";
            sql += " EXISTS ";
            sql += "(";
            sql += "SELECT 1 FROM (" + SurecYilSurecNoQueryOlustur(bilgi) + ") AS A WHERE ";
            sql += "A.YIL = " + TABLOADFATURAFORM + "." + ALANLARFATURAFORM[(int)ENUMAlanlarHakedis.YIL];
            sql += " AND ";
            sql += "A.SURECNO = " + TABLOADFATURAFORM + "." + ALANLARFATURAFORM[(int)ENUMAlanlarHakedis.SURECNO];
            sql += ")";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader TasinirBilgiListele(SurecSorguKriter bilgi)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTASINIRFORM, ALANLARTASINIRFORM) + " FROM " + TABLOADTASINIRFORM;

            sql += " WHERE ";
            sql += " EXISTS ";
            sql += "(";
            sql += "SELECT 1 FROM (" + SurecYilSurecNoQueryOlustur(bilgi) + ") AS A WHERE ";
            sql += "A.YIL = " + TABLOADTASINIRFORM + "." + ALANLARTASINIRFORM[(int)ENUMAlanlarHakedis.YIL];
            sql += " AND ";
            sql += "A.SURECNO = " + TABLOADTASINIRFORM + "." + ALANLARTASINIRFORM[(int)ENUMAlanlarHakedis.SURECNO];
            sql += ")";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader TasinirDetayBilgiListele(TasinirForm bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARTASINIRDETAY) + " FROM " + TABLOADTASINIRDETAY;
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARTASINIRDETAY[(int)ENUMAlanlarTasinirDetay.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARTASINIRDETAY[(int)ENUMAlanlarTasinirDetay.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARTASINIRDETAY[(int)ENUMAlanlarTasinirDetay.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARTASINIRDETAY[(int)ENUMAlanlarTasinirDetay.SURECNO], "=", bilgi.surecNo));
            if (bilgi.prTasinirNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARTASINIRDETAY[(int)ENUMAlanlarTasinirDetay.PRTASINIRNO], "=", bilgi.prTasinirNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARTASINIRDETAY[(int)ENUMAlanlarTasinirDetay.MUHASEBEKOD] + ", " +
                ALANLARTASINIRDETAY[(int)ENUMAlanlarTasinirDetay.BIRIMKOD] + "," +
                ALANLARTASINIRDETAY[(int)ENUMAlanlarTasinirDetay.SURECNO] + "," +
                ALANLARTASINIRDETAY[(int)ENUMAlanlarTasinirDetay.PRTASINIRNO] + "," +
                ALANLARTASINIRDETAY[(int)ENUMAlanlarTasinirDetay.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader IhaleSozlesmeSonuUcretBilgiListele(SurecSorguKriter bilgi)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADSOZLESMESONUUCRET, ALANLARSOZLESMESONUUCRET) + " FROM " + TABLOADSOZLESMESONUUCRET;

            sql += " WHERE ";
            sql += " EXISTS ";
            sql += "(";
            sql += "SELECT 1 FROM (" + SurecYilSurecNoQueryOlustur(bilgi) + ") AS A WHERE ";
            sql += "A.YIL = " + TABLOADSOZLESMESONUUCRET + "." + ALANLARSOZLESMESONUUCRET[(int)ENUMAlanlarSozlesmeSonuUcret.YIL];
            sql += " AND ";
            sql += "A.SURECNO = " + TABLOADSOZLESMESONUUCRET + "." + ALANLARSOZLESMESONUUCRET[(int)ENUMAlanlarSozlesmeSonuUcret.SURECNO];
            sql += ")";

            sql += " ORDER BY " + TABLOADSOZLESMESONUUCRET + "." + ALANLARSOZLESMESONUUCRET[(int)ENUMAlanlarSozlesmeSonuUcret.YIL] + " DESC ";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader MufYaklasikMaliyetListele(SurecSorguKriter kriter)
        {
            string sql = "";
            string select = "";
            string from = "";

            string where = "";
            string kosul = "";
            string groupBy = "";

            select = "SELECT YIL, MUHASEBEKOD, BIRIMKOD, SURECNO, MALZEMENO, COUNT(FIRMANO) AS FIRMASAYISI, SUM(BIRIMFIYAT) AS TOPLAMBIRIMFIYAT";
            from = " FROM HRCTEKLIF";

            if (kriter.yil > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND ", VeriErisim.KosulOl("HRCTEKLIF", ALANLAR[(int)ENUMAlanlar.YIL], "=", kriter.yil));
                //kosul = VeriErisim.KosulEkle(kosul, "OR", "(HRCSUREC.YILLARASARIISLEMMI>0 AND HRCSUREC.YIL<" + bilgi.yil + "))");
            }
            if (kriter.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("HRCTEKLIF", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebe));
            if (kriter.birim != "")
            {
                string[] birimler = kriter.birim.Split(';');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                {
                    if (birimler[i].Trim() != "")
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("HRCTEKLIF", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimler[i].Trim()));
                }
                if (birimKosul != "")
                    kosul += " AND (" + birimKosul + ")";
            }
            if (kriter.surecNoBasla != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("HRCTEKLIF", ALANLAR[(int)ENUMAlanlar.SURECNO], ">=", kriter.surecNoBasla));
            if (kriter.surecNoBitis != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("HRCTEKLIF", ALANLAR[(int)ENUMAlanlar.SURECNO], "<=", kriter.surecNoBitis));
            where = " WHERE (TIP = 1) AND " + kosul;

            groupBy = " GROUP BY YIL, MUHASEBEKOD, BIRIMKOD, SURECNO, MALZEMENO";

            sql = select + from + where + groupBy;
            return veriErisim.DataReaderDondur(sql);
        }
    }
}