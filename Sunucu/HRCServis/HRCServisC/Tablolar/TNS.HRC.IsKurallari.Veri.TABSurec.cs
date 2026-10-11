using System;
using System.Data;
using TNS.Veri;
using TNS.BKD;
using System.Reflection;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Süreç bilgisini tutan HRCSUREC veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABSurec : TABOrtak
    {
        static bool yenialan = true;

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSurec(VeriErisim veriErisim) : base(veriErisim) { }

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

        public static string TABLOADTARIHCE = "HRCSURECTARIHCE";
        public enum ENUMAlanlarTarihce
        {
            //SIRANO alanı Identity olduğu için eklenmedi
            KOD = 0,
            YIL = 1,
            SURECNO = 2,
            ISLEMKOD = 3,
            PARCAKOD = 4,
            TARIH = 5,
            KULLANICIAD = 6,
            DURUM = 7,
        }
        public static string[] ALANLARTARIHCE = { "KOD", "YIL", "SURECNO", "ISLEMKOD", "PARCAKOD", "TARIH", "KULLANICIAD", "DURUM" };

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
            /// <summary>
            /// 
            /// </summary>
            KURKOD = 5,
            /// <summary>
            /// 
            /// </summary>
            FONKOD = 6,
            /// <summary>
            /// 
            /// </summary>
            FINKOD = 7,
            /// <summary>
            /// 
            /// </summary>
            EKOKOD = 8,
            /// <summary>
            /// 
            /// </summary>
            PEBKOD = 9,
            /// <summary>
            /// 
            /// </summary>
            BORC = 10,
        }

        /// <summary>
        /// Süreç muhasebe işlem fişi bağlantı tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARMUHMIFBAG = { "YIL", "SURECNO", "MUHASEBEKOD", "BIRIMKOD", "ODEMEEMRINO", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "PEBKOD", "BORC" };

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
        /// Süreç bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek süreç bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(Surec bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.islem.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecTarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecDurum) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.isinAdi) + ",";
            sql += VeriErisim.DegerYap(bilgi.yillaraSariIslemMi) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Süreç açıklama bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek süreç bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetAciklama(Surec bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SURECDURUM], bilgi.surecDurum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISINADI], bilgi.isinAdi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YILLARASARIISLEMMI], bilgi.yillaraSariIslemMi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDurum.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDurum.SURECNO], "=", bilgi.surecNo));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Süreç durum bilgisini veri tabanında güncellemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Güncellenecek süreç bilgisini tutan nesne</param>
        /// <returns>Oluşturulan update komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc DurumDegistir(Surec bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SURECDURUM], bilgi.surecDurum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc IslemDegistir(Surec bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMKOD], bilgi.islem.kod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Süreç bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak süreç bilgisi kıstas parametresi</param>
        /// <returns>
        /// Süreç bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(Surec bilgi, string kullaniciKod, string isKosul)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + VeriErisim.TumAlanlar(TABLOADIS, ALANLARIS) + ", HRCMUHASEBEVIEW.MUHASEBEAD, HRCMUHASEBEBIRIMVIEW.BIRIMAD";
            sql += " FROM " + TABLOAD;
            sql += " INNER JOIN HRCMUHASEBEVIEW ON HRCSUREC.MUHASEBEKOD = HRCMUHASEBEVIEW.MUHASEBEKOD ";
            sql += " INNER JOIN HRCMUHASEBEBIRIMVIEW ON HRCSUREC.MUHASEBEKOD = HRCMUHASEBEBIRIMVIEW.MUHASEBEKOD AND HRCSUREC.BIRIMKOD = HRCMUHASEBEBIRIMVIEW.BIRIMKOD";
            sql += " INNER JOIN HRCIS ON HRCSUREC.ISLEMKOD = HRCIS.KOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));

            if (bilgi.yil > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
                if (string.IsNullOrWhiteSpace(bilgi.surecNo))
                    sql = VeriErisim.KosulEkle(sql, "OR", "(YILLARASARIISLEMMI>0 AND HRCSUREC.YIL<" + bilgi.yil + "))");
                else
                    sql += ")";
            }
            if (!string.IsNullOrWhiteSpace(bilgi.surecNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            else
            {
                if (bilgi.muhasebe > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));

                if (!string.IsNullOrWhiteSpace(bilgi.birim))
                {
                    string[] birimler = bilgi.birim.Split(';');
                    string birimKosul = "";
                    for (int i = 0; i < birimler.Length; i++)
                    {
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimler[i]));
                    }
                    if (birimKosul != "")
                        sql += " AND (" + birimKosul + ")";
                }
                if (bilgi.surecDurum > 0)
                {
                    if (bilgi.surecDurum <= 2)
                    {
                        string kosul = "";
                        kosul = VeriErisim.KosulEkle(kosul, "", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECDURUM], "=", 1));
                        kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECDURUM], "=", 2));
                        sql += " AND " + kosul;
                    }
                    else
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECDURUM], "=", bilgi.surecDurum));
                }

                if (bilgi.surecTarih != null && !bilgi.surecTarih.isNull)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECTARIH], "=", bilgi.surecTarih));

                if (bilgi.islem != null && bilgi.islem.kod > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMKOD], "=", bilgi.islem.kod));

                if (bilgi.yillaraSariIslemMi > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YILLARASARIISLEMMI], "=", bilgi.yillaraSariIslemMi));

                if (!string.IsNullOrWhiteSpace(isKosul))
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEMKOD] + " IN " + "(" + isKosul.Replace(';', ',') + ")");
                }

                if (!string.IsNullOrWhiteSpace(kullaniciKod))
                {
                    string icKosul = "1 IN (SELECT 1 FROM HRCSURECDURUM D WHERE D.YIL = " + TABLOAD + ".YIL AND D.SURECNO=" + TABLOAD + ".SURECNO AND D.KULLANICIAD = " + VeriErisim.DegerYap(kullaniciKod);
                    sql = VeriErisim.KosulEkle(sql, "AND", "(" + icKosul + "))");
                }
            }
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + ", " + ALANLAR[(int)ENUMAlanlar.SURECNO];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Süreç bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak süreç bilgisi kıstas parametresi</param>
        /// <returns>
        /// Süreç bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleSurecMuhMifBaglanti(SurecMuhMifBaglanti bilgi, Tertip t)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADMUHMIFBAG, ALANLARMUHMIFBAG);
            sql += " FROM " + TABLOADMUHMIFBAG;

            string kosul = string.Empty;
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl(TABLOADMUHMIFBAG, ALANLARMUHMIFBAG[(int)ENUMAlanlarMUHMIFBAG.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMUHMIFBAG, ALANLARMUHMIFBAG[(int)ENUMAlanlarMUHMIFBAG.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (!string.IsNullOrEmpty(bilgi.birim))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMUHMIFBAG, ALANLARMUHMIFBAG[(int)ENUMAlanlarMUHMIFBAG.BIRIMKOD], "=", bilgi.birim));

            if (t != null)
            {
                if (t.kodlar.Length > 0 && !string.IsNullOrEmpty(t.kodlar[0]))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMUHMIFBAG, ALANLARMUHMIFBAG[(int)ENUMAlanlarMUHMIFBAG.KURKOD], "=", t.kodlar[0]));
                if (t.kodlar.Length > 1 && !string.IsNullOrEmpty(t.kodlar[1]))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMUHMIFBAG, ALANLARMUHMIFBAG[(int)ENUMAlanlarMUHMIFBAG.FONKOD], "=", t.kodlar[1]));
                if (t.kodlar.Length > 2 && !string.IsNullOrEmpty(t.kodlar[2]))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMUHMIFBAG, ALANLARMUHMIFBAG[(int)ENUMAlanlarMUHMIFBAG.FINKOD], "=", t.kodlar[2]));
                if (t.kodlar.Length > 3 && !string.IsNullOrEmpty(t.kodlar[3]))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMUHMIFBAG, ALANLARMUHMIFBAG[(int)ENUMAlanlarMUHMIFBAG.EKOKOD], "LIKE", t.kodlar[3] + "%"));
                if (t.kodlar.Length > 4 && !string.IsNullOrEmpty(t.kodlar[4]))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMUHMIFBAG, ALANLARMUHMIFBAG[(int)ENUMAlanlarMUHMIFBAG.PEBKOD], "=", t.kodlar[4]));
            }

            if (!string.IsNullOrEmpty(kosul))
            {
                sql += " WHERE " + kosul;
            }

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + ", " + ALANLAR[(int)ENUMAlanlar.SURECNO] + " DESC";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Süreç bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak süreç sorgu kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Süreç bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Sorgula(SurecSorguKriter bilgi, string kullaniciKod, string isKosul)
        {
            string sql = "";
            string select = "";
            string from = "";
            string innerJoin = "";
            string where = "";
            string kosul = "";
            string orderBy = "";

            //SELECT//
            select += "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR)
                          + "," + VeriErisim.TumAlanlar(TABLOADIS, ALANLARIS)
                          + "," + TABLOADMUHASEBEVIEW + "." + ALANLARMUHASEBEVIEW[(int)ENUMAlanlarMUHASEBEVIEW.MUHASEBEAD]
                          + "," + TABLOADDURUM + "." + ALANLARDURUM[(int)ENUMAlanlarDurum.PARCAKOD]
                          + "," + TABLOADDURUM + "." + ALANLARDURUM[(int)ENUMAlanlarDurum.TARIH]
                          + "," + TABLOADDURUM + "." + ALANLARDURUM[(int)ENUMAlanlarDurum.KULLANICIAD]
                          + "," + "(SELECT SUM(KULLANILACAKTUTAR) FROM  HRCIHALEONAYTERTIP T WHERE T.YIL=HRCSUREC.YIL AND T.SURECNO=HRCSUREC.SURECNO) AS ONAYTUTAR"
                          + "," + "(SELECT SUM(GENELTOPLAM) FROM  HRCTEKLIFGENELTOPLAM T WHERE T.YIL=HRCSUREC.YIL AND T.SURECNO=HRCSUREC.SURECNO AND T.TIP=1) AS YAKLASIKMALIYET"
                          + "," + "(SELECT SUM(GENELTOPLAM) FROM  HRCTEKLIFGENELTOPLAM T WHERE T.YIL=HRCSUREC.YIL AND T.SURECNO=HRCSUREC.SURECNO AND T.TIP=3) AS PIYASAFIYAT"
                          + "," + "(SELECT SUM(KDVLISOZLESMETUTAR) FROM  HRCIHALEONAYTERTIP T WHERE T.YIL=HRCSUREC.YIL AND T.SURECNO=HRCSUREC.SURECNO) AS ONAYTUTAR"
                          + "," + "(SELECT COUNT(*) FROM HRCODEMEEMRIFORM T WHERE T.YIL=HRCSUREC.YIL AND T.SURECNO=HRCSUREC.SURECNO) AS ODEMEEMRIVAR"
                          + "," + "(SELECT COUNT(*) FROM HRCSURECSIFRE T WHERE T.YIL=HRCSUREC.YIL AND T.SURECNO=HRCSUREC.SURECNO) AS SIFREVAR";
            //--------------------->

            //FROM//
            from += " FROM " + TABLOAD;

            innerJoin += " INNER JOIN " + TABLOADMUHASEBEVIEW + " ON " +
                        TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + TABLOADMUHASEBEVIEW + "." + ALANLARMUHASEBEVIEW[(int)ENUMAlanlarMUHASEBEVIEW.MUHASEBEKOD];

            innerJoin += " INNER JOIN " + TABLOADIS + " ON " +
                        TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEMKOD] + "=" + TABLOADIS + "." + ALANLARIS[(int)ENUMAlanlarIS.KOD];

            innerJoin += " LEFT OUTER JOIN " + TABLOADDURUM + " ON " +
                        TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADDURUM + "." + ALANLARDURUM[(int)ENUMAlanlarDurum.YIL]
                        + " AND " +
                        TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADDURUM + "." + ALANLARDURUM[(int)ENUMAlanlarDurum.SURECNO]
                        + " AND " +
                        TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEMKOD] + "=" + TABLOADDURUM + "." + ALANLARDURUM[(int)ENUMAlanlarDurum.ISLEMKOD];
            //--------------------->

            //WHERE//
            kosul = "";
            if (bilgi.yil > 0 || bilgi.muhasebeZorunlu)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND ", "(" + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
                kosul = VeriErisim.KosulEkle(kosul, "OR", "(HRCSUREC.YILLARASARIISLEMMI>0 AND HRCSUREC.YIL<" + bilgi.yil + "))");
            }

            if (bilgi.muhasebe > 0 || bilgi.muhasebeZorunlu)
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


            if (bilgi.malHizmetYapim == 1 || bilgi.malHizmetYapim == 2)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIS, ALANLARIS[(int)ENUMAlanlarIS.TUR], bilgi.malHizmetYapim == 1 ? "<>" : "=", 0));
            }

            if (bilgi.islem.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMKOD], "=", bilgi.islem.kod));
            if (bilgi.isinAdi != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISINADI], "LIKE", "%" + bilgi.isinAdi + "%"));
            if (bilgi.aciklama != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + bilgi.aciklama + "%"));
            if (bilgi.surecNoBasla != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], ">=", bilgi.surecNoBasla));
            if (bilgi.surecNoBitis != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "<=", bilgi.surecNoBitis));
            if (!bilgi.surecTarihBasla.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECTARIH], ">=", bilgi.surecTarihBasla));
            if (!bilgi.surecTarihBitis.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECTARIH], "<=", bilgi.surecTarihBitis));
            if (bilgi.parcaDurum.parcaKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDURUM, ALANLARDURUM[(int)ENUMAlanlarDurum.PARCAKOD], "=", bilgi.parcaDurum.parcaKod));
            if (bilgi.parcaDurum.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDURUM, ALANLARDURUM[(int)ENUMAlanlarDurum.DURUM], "=", bilgi.parcaDurum.durum));

            if (!string.IsNullOrWhiteSpace(isKosul))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEMKOD] + " IN " + "(" + isKosul.Replace(';', ',') + ")");
            }

            if (!string.IsNullOrWhiteSpace(kullaniciKod))
            {
                string icKosul = "1 IN (SELECT 1 FROM HRCSURECDURUM D WHERE D.YIL = " + TABLOAD + ".YIL AND D.SURECNO=" + TABLOAD + ".SURECNO AND D.KULLANICIAD = " + VeriErisim.DegerYap(kullaniciKod);
                kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + "))");
            }

            if (!string.IsNullOrWhiteSpace(bilgi.firmaAdi) || !string.IsNullOrWhiteSpace(bilgi.firmaVNO))
            {
                string icKosul2 = "";
                if (bilgi.firmaVNO != "")
                    icKosul2 = "VERGINO LIKE '%" + bilgi.firmaVNO + "%'";

                if (bilgi.firmaAdi != "")
                {
                    if (icKosul2 != "") icKosul2 += " OR ";
                    icKosul2 = "AD LIKE '%" + bilgi.firmaAdi + "%'";
                }

                string icKosul = "";
                icKosul += "(HRCSUREC.SURECNO IN (SELECT SURECNO FROM HRCTEKLIF WHERE YIL=" + bilgi.yil + " AND FIRMANO IN (SELECT KOD FROM HRCFIRMABILGISI WHERE " + icKosul2 + ")))";
                icKosul += " OR ";
                icKosul += "(HRCSUREC.SURECNO IN (SELECT SURECNO FROM HRCFATURAFORM WHERE YIL=" + bilgi.yil + " AND FIRMANO IN (SELECT KOD FROM HRCFIRMABILGISI WHERE " + icKosul2 + ")))";
                kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + ")");
            }



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

            if (bilgi.ihaleUsulu > 0 || bilgi.alimIsleri)
            {
                innerJoin += " LEFT OUTER JOIN " + TABLOADONAYFORM + " ON " +
                                           TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADONAYFORM + "." + ALANLARONAYFORM[(int)ENUMAlanlarONAYFORM.YIL]
                                           + " AND " +
                                           TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + TABLOADONAYFORM + "." + ALANLARONAYFORM[(int)ENUMAlanlarONAYFORM.MUHASEBEKOD]
                                           + " AND " +
                                           TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + "=" + TABLOADONAYFORM + "." + ALANLARONAYFORM[(int)ENUMAlanlarONAYFORM.BIRIMKOD]
                                           + " AND " +
                                           TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADONAYFORM + "." + ALANLARONAYFORM[(int)ENUMAlanlarONAYFORM.SURECNO];

                if (bilgi.ihaleUsulu > 0)
                    where = VeriErisim.KosulEkle(where, "AND", VeriErisim.KosulOl(TABLOADONAYFORM, ALANLARONAYFORM[(int)ENUMAlanlarONAYFORM.IHALEUSULU], "=", bilgi.ihaleUsulu));
                else
                    where = VeriErisim.KosulEkle(where, "AND", VeriErisim.KosulOl(TABLOADONAYFORM, ALANLARONAYFORM[(int)ENUMAlanlarONAYFORM.IHALEUSULU], ">", 0));
            }

            sql = select + from + innerJoin + where + orderBy;
            //OrtakFonksiyonlar.HataStrYaz(sql);
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Devam eden süreç bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak süreç sorgu kriter bilgisi kıstas parametresi</param>
        /// <returns>
        /// Devam eden süreç bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader DevamEdenler(SurecSorguKriter bilgi, string kullaniciKod, string isKosul)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + VeriErisim.TumAlanlar(TABLOADIS, ALANLARIS);
            sql += " FROM " + TABLOAD + " INNER JOIN " + TABLOADIS + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEMKOD] + "=" + TABLOADIS + "." + ALANLARIS[(int)ENUMAlanlarIS.KOD];

            if (bilgi.yil > 0)
            {
                string yilKosul = "(" + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil) + " OR (HRCSUREC.YIL<" + bilgi.yil + " AND HRCSUREC.YILLARASARIISLEMMI=1 AND (HRCSUREC.SURECDURUM=1 OR HRCSUREC.SURECDURUM=2)))";
                kosul = VeriErisim.KosulEkle(kosul, "AND", yilKosul);
            }

            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));

            if (bilgi.malHizmetYapim == 1 || bilgi.malHizmetYapim == 2)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIS, ALANLARIS[(int)ENUMAlanlarIS.TUR], bilgi.malHizmetYapim == 1 ? "<>" : "=", 0));
            }

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

            if (!string.IsNullOrWhiteSpace(isKosul))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEMKOD] + " IN " + "(" + isKosul.Replace(';', ',') + ")");
            }

            if (!string.IsNullOrWhiteSpace(kullaniciKod))
            {
                string icKosul = "1 IN (SELECT 1 FROM HRCSURECDURUM D WHERE D.YIL = " + TABLOAD + ".YIL AND D.SURECNO=" + TABLOAD + ".SURECNO AND D.KULLANICIAD = " + VeriErisim.DegerYap(kullaniciKod);
                kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + "))");
            }

            sql += " WHERE " + kosul;
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SURECNO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Süreç parça durum bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Eklenecek yıl bilgisini tutan nesne</param>
        /// <param name="surecNo">Eklenecek süreç numarası bilgisini tutan nesne</param>
        /// <param name="islemKod">Eklenecek işlem kod bilgisini tutan nesne</param>
        /// <param name="parcaKod">Eklenecek parça kod bilgisini tutan nesne</param>
        /// <param name="durum">Eklenecek durum bilgisini tutan nesne</param>
        /// <param name="kullaniciAd">Eklenecek kullanıcı adı bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc ParcaDurumKaydet(int yil, string surecNo, int islemKod, int parcaKod, ENUMSurecParcaDurum durum, string kullaniciAd)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOADDURUM + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDURUM[(int)ENUMAlanlarDurum.TARIH], new TNSDateTime(DateTime.Now.ToString())) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARDURUM[(int)ENUMAlanlarDurum.KULLANICIAD], kullaniciAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARDURUM[(int)ENUMAlanlarDurum.DURUM], (int)durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDurum.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDurum.SURECNO], "=", surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDurum.PARCAKOD], "=", parcaKod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "INSERT INTO " + TABLOADDURUM + " (" + VeriErisim.TumAlanlar("", ALANLARDURUM) + ") VALUES (";
                sql += VeriErisim.DegerYap(yil) + ",";
                sql += VeriErisim.DegerYap(surecNo) + ",";
                sql += VeriErisim.DegerYap(islemKod) + ",";
                sql += VeriErisim.DegerYap(parcaKod) + ",";
                sql += VeriErisim.DegerYap(new TNSDateTime(DateTime.Now.ToString())) + ",";
                sql += VeriErisim.DegerYap(kullaniciAd) + ",";
                sql += VeriErisim.DegerYap((int)durum) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc ParcaTarihceKaydet(int yil, string surecNo, int islemKod, int parcaKod, ENUMSurecParcaDurum durum, string kullaniciAd)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADTARIHCE + " (" + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + ") VALUES (";
            sql += VeriErisim.DegerYap(Guid.NewGuid().ToString().Replace("-", "")) + ",";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(surecNo) + ",";
            //sql += VeriErisim.DegerYap(siraNo) + ",";
            sql += VeriErisim.DegerYap(islemKod) + ",";
            sql += VeriErisim.DegerYap(parcaKod) + ",";
            sql += VeriErisim.DegerYap(new TNSDateTime(DateTime.Now.ToString())) + ",";
            sql += VeriErisim.DegerYap(kullaniciAd) + ",";
            sql += VeriErisim.DegerYap((int)durum) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Süreç durum bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Eklenecek yıl bilgisini tutan nesne</param>
        /// <param name="surecNo">Eklenecek süreç numarası bilgisini tutan nesne</param>
        /// <param name="durum">Eklenecek durum bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc SurecDurumKaydet(int yil, string surecNo, ENUMSurecDurum durum)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SURECDURUM], (int)durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecNo));

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        /// <summary>
        /// Süreç parça durum bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak süreç parça durum bilgisi kıstas parametresi</param>
        /// <returns>
        /// Süreç parça durum bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ParcaDurumListele(SurecParcaDurum bilgi)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARDURUM) + "," + TABLOADPARCA + "." + ALANLARPARCA[(int)ENUMAlanlarPARCA.AD] + " FROM " + TABLOADDURUM;
            sql += " LEFT OUTER JOIN " + TABLOADPARCA + " ON ";
            sql += TABLOADDURUM + "." + ALANLARDURUM[(int)ENUMAlanlarDurum.PARCAKOD] + "=" + TABLOADPARCA + "." + ALANLARPARCA[(int)ENUMAlanlarPARCA.KOD];

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDurum.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDurum.SURECNO], "=", bilgi.surecNo));

            if (bilgi.parcaKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDurum.PARCAKOD], "=", bilgi.parcaKod));

            sql += " ORDER BY " + ALANLARDURUM[(int)ENUMAlanlarDurum.SURECNO] + "," + ALANLARDURUM[(int)ENUMAlanlarDurum.PARCAKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Hatırlatma bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kullanici">Listeleme için kullanılacak kullanıcı adı kıstas parametresi</param>
        /// <returns>
        /// Hatırlatma bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader HatirlatmaSorgula(string kullanici)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARHATIRLATMA) + " FROM " + TABLOADHATIRLATMA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARHATIRLATMA[(int)ENUMAlanlarHATIRLATMA.KULLANICIAD], "=", kullanici));

            sql += " ORDER BY " + ALANLARHATIRLATMA[(int)ENUMAlanlarHATIRLATMA.KOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Hatırlatma bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kullanici">Eklenecek kullanıcı adı bilgisini tutan nesne</param>
        /// <param name="kod">Eklenecek kod bilgisini tutan nesne</param>
        /// <param name="aciklama">Eklenecek açıklama bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc HatirlatmaKaydet(string kullanici, int kod, string aciklama)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADHATIRLATMA + " (" + VeriErisim.TumAlanlar("", ALANLARHATIRLATMA) + ") VALUES (";
            sql += VeriErisim.DegerYap(kod) + ",";
            sql += VeriErisim.DegerYap(kullanici) + ",";
            sql += VeriErisim.DegerYap(aciklama) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Eklenecek bilgisini tutan nesne bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kullanici">Silinecek kullanıcı adı bilgisini tutan nesne</param>
        /// <param name="kod">Silinecek kod bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc HatirlatmaSil(string kullanici, int kod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "DELETE FROM " + TABLOADHATIRLATMA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARHATIRLATMA[(int)ENUMAlanlarHATIRLATMA.KULLANICIAD], "=", kullanici));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARHATIRLATMA[(int)ENUMAlanlarHATIRLATMA.KOD], "=", kod));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Tertip bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kullanici">Listeleme için kullanılacak kullanıcı adı kıstas parametresi</param>
        /// <returns>
        /// Tertip bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader TertipSorgula(string kullanici)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARTERTIP) + " FROM " + TABLOADTERTIP;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KULLANICIAD], "=", kullanici));

            sql += " ORDER BY " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.TERTIP];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tertip bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kullanici">Eklenecek kullanıcı adı bilgisini tutan nesne</param>
        /// <param name="tertip">Eklenecek tertip bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc TertipKaydet(string kullanici, string tertip)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADTERTIP + " (" + VeriErisim.TumAlanlar("", ALANLARTERTIP) + ") VALUES (";
            sql += VeriErisim.DegerYap(tertip) + ",";
            sql += VeriErisim.DegerYap(kullanici) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Tertip bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kullanici">Silinecek kullanıcı adı bilgisini tutan nesne</param>
        /// <param name="tertip">Silinecek tertip bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc TertipSil(string kullanici, string tertip)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "DELETE FROM " + TABLOADTERTIP;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KULLANICIAD], "=", kullanici));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.TERTIP], "=", tertip));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Devam eden süreç bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="muhasebe">Listeleme için kullanılacak muhasebe kod bilgisi kıstas parametresi</param>
        /// <param name="birim">Listeleme için kullanılacak birim kod bilgisi kıstas parametresi</param>
        /// <param name="tertip">Listeleme için kullanılacak tertip bilgisi kıstas parametresi</param>
        /// <returns>
        /// Devam eden süreç bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader DevamEdenIsler(int muhasebe, string birim, string tertip)
        {
            string sql = "";

            sql = "SELECT SUM(YAKLASIKMALIYET) FROM HRCIHALEONAYFORM O,HRCSUREC S";
            sql += " WHERE O.YIL=S.YIL AND O.SURECNO=S.SURECNO AND S.SURECDURUM<5";
            sql += " AND O.MUHASEBEKOD=" + muhasebe;


            if (birim.IndexOf(',') > 0)
            {
                string[] birimler = birim.Split(',');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                {
                    birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", " O.BIRIMKOD='" + birimler[i].Replace(".", "") + "'");
                }
                if (birimKosul != "")
                    sql += " AND (" + birimKosul + ")";
            }

            sql += " AND (O.BUTCETERTIBI1 LIKE '" + tertip + "%' OR O.BUTCETERTIBI2='" + tertip + "%')";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Sürecin yıllara sari olup olmadığı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="surecYil">Listeleme için kullanılacak süreç yıl bilgisi kıstas parametresi</param>
        /// <param name="surecNo">Listeleme için kullanılacak süreç numarası bilgisi kıstas parametresi</param>
        /// <returns>
        /// Sürecin yıllara sari olup olmadığı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public int YillaraSariIslemMi(int surecYil, string surecNo)
        {
            string sql = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YILLARASARIISLEMMI] + " FROM " + TABLOAD + " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecNo));

            return veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Yenis the alan olustur.
        /// </summary>
        public void YeniAlanOlustur()
        {
            if (yenialan)
            {
                yenialan = false;
                string sql = "";
                Sonuc sonuc = new Sonuc();
                try
                {
                    sql = "ALTER TABLE HRCMERKEZBANKKUR ADD SERBESTALIS decimal(18,6)";
                    sonuc = KomutCalistir(sql, 0, 0);
                }
                catch { }
                veriErisim.hata = "";

                try
                {
                    sql = "ALTER TABLE HRCMERKEZBANKKUR ADD SERBESTSATIS decimal(18,6)";
                    sonuc = KomutCalistir(sql, 0, 0);
                }
                catch { }
                veriErisim.hata = "";
            }
        }

        public IDataReader SurecBaslatanBul(int surecYil, string surecNo)
        {
            string sql = "SELECT TARIH, KULLANICIAD FROM HRCSURECDURUM ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SURECNO", "=", surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "PARCAKOD", "=", 0));
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader SurecDurumListele(int surecYil, string surecNo)
        {
            string sql = "SELECT HRCSURECTARIHCE.YIL, HRCSURECTARIHCE.SURECNO, HRCSURECTARIHCE.ISLEMKOD, HRCSURECTARIHCE.PARCAKOD, HRCSURECTARIHCE.TARIH, HRCSURECTARIHCE.KULLANICIAD, HRCSURECTARIHCE.DURUM, HRCPARCA.AD, HRCIS.AD AS Expr1, Kullanici.dbo.KULLANICI.AD AS Expr2, Kullanici.dbo.KULLANICI.SOYAD";
            sql += " FROM   HRCSURECTARIHCE";
            sql += " INNER JOIN HRCPARCA ON HRCSURECTARIHCE.PARCAKOD = HRCPARCA.KOD";
            sql += " INNER JOIN HRCIS ON HRCSURECTARIHCE.ISLEMKOD = HRCIS.KOD";
            sql += " INNER JOIN Kullanici.dbo.KULLANICI ON HRCSURECTARIHCE.KULLANICIAD = Kullanici.dbo.KULLANICI.KULLANICIKODU";
            sql += " INNER JOIN HRCSURECPARCA ON HRCSURECTARIHCE.YIL = HRCSURECPARCA.YIL AND HRCSURECTARIHCE.SURECNO = HRCSURECPARCA.SURECNO AND HRCSURECTARIHCE.ISLEMKOD = HRCSURECPARCA.ISLEMKOD AND HRCSURECTARIHCE.PARCAKOD = HRCSURECPARCA.PARCAKOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("HRCSURECTARIHCE", "YIL", "=", surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCSURECTARIHCE", "SURECNO", "=", surecNo));

            sql += " ORDER BY HRCSURECTARIHCE.YIL, HRCSURECTARIHCE.SURECNO, HRCSURECTARIHCE.SIRANO, HRCSURECTARIHCE.TARIH";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader OzetBilgiBul(int surecYil, string surecNo)
        {
            string sql = null;

            sql = "SELECT HRCSUREC.YIL, HRCSUREC.SURECNO, HRCSUREC.MUHASEBEKOD, HRCSUREC.BIRIMKOD, HRCSUREC.ISLEMKOD, HRCSUREC.SURECTARIH, HRCSUREC.SURECDURUM, HRCSUREC.ACIKLAMA, ";
            sql += "HRCSUREC.ISINADI, HRCSUREC.YILLARASARIISLEMMI, HRCIS.AD, HRCIS.TUR, ";
            sql += "HRCIHALEONAYFORM.ISTANIM, HRCIHALEONAYFORM.ISNITELIK, ";
            sql += "HRCIHALEONAYFORM.ISMIKTAR, HRCIHALEONAYFORM.IHALEUSULU, HRCIHALEONAYFORM.IHALEMADDESI, HRCIHALETURMADDE.ADI, HRCIHALETURMADDE.ACIKLAMA";

            sql += " FROM HRCIHALETURMADDE INNER JOIN";
            sql += " HRCIHALEONAYFORM ON HRCIHALETURMADDE.IHALEKOD = HRCIHALEONAYFORM.IHALEUSULU AND HRCIHALETURMADDE.KOD = HRCIHALEONAYFORM.IHALEMADDESI RIGHT OUTER JOIN";
            sql += " HRCSUREC INNER JOIN";
            sql += " HRCIS ON HRCSUREC.ISLEMKOD = HRCIS.KOD ON HRCIHALEONAYFORM.YIL = HRCSUREC.YIL AND HRCIHALEONAYFORM.SURECNO = HRCSUREC.SURECNO";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecNo));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}