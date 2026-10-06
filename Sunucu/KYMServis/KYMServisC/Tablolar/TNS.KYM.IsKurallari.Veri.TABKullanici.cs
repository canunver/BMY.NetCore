using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM.IsKurallari;

namespace TNS.KYM.IsKurallari.Veri
{
    /// <summary>
    /// Sisteme giriş yapabilecek kullanıcı bilgilerini tutan "KULLANICI" veri tabanı tablosu,
    ///  ve kullanıcı bazında tutulan değişkenleri ve değerlerini tutan "KULLANICI_DEGISKEN" veri tabanı tablosu,
    ///  ve kullanıcı üzerinde yapılan işlemlerin tarihçesini tutan "KULLANICIISLEMTARIHCESI" veri tabanı tablosu,
    ///  ve kullanıcının yetkilendirilmesiyle ilgili yetki tiplerini tutan "KULLANICI_YT" veri tabanı tablosu,
    ///  ve kullanıcının çalıştığı birimlerin listesini tutan "KULLANICI_MUHASEBE" veri tabanı görünümü
    ///  ve kullanıcıların girdikleri sayfaları ve zamanlarını tutan "KULLANICI_BILGI" veri tabanı tablosu
    /// ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABKullanici : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne.</param>
        public TABKullanici(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Kullanıcı tanımlarını tutan tablonun adını tutan sabit değişken.
        /// </summary>
        public static string TABLOAD = "KULLANICI";

        /// <summary>
        /// Kullanıcı tanımlarını tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// KULLANICIKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANICIKODU = 0,

            /// <summary>
            /// MERNIS alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MERNIS = 1,

            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 2,

            /// <summary>
            /// SOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SOYAD = 3,

            /// <summary>
            /// PAROLA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PAROLA = 4,

            /// <summary>
            /// CALISTIGIKURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CALISTIGIKURUM = 5,

            /// <summary>
            /// DOGUMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOGUMTARIHI = 6,

            /// <summary>
            /// EMAIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EMAIL = 7,

            /// <summary>
            /// TELEFON alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TELEFON = 8,

            /// <summary>
            /// SONGIRISTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SONGIRISTARIHI = 9,

            /// <summary>
            /// AKTIFMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AKTIFMI = 10,

            /// <summary>
            /// CALISTIGIBIRIM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CALISTIGIBIRIM = 11,

            /// <summary>
            /// SICILNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SICILNO = 12,

            /// <summary>
            /// UNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UNVAN = 13,

            /// <summary>
            /// PAROLABITISTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PAROLABITISTARIHI = 14,

            /// <summary>
            /// SESSIONID alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SESSIONID = 15,

            /// <summary>
            /// ARAMADAGOSTERME alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ARAMADAGOSTERME = 16,

            /// <summary>
            /// AKADEMIKUNVAN
            /// </summary>
            AKADEMIKUNVAN = 17,

            /// <summary>
            /// RUMUZ
            /// </summary>
            RUMUZ = 18
        }

        /// <summary>
        /// Kullanıcı tanımlarını tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        public static string[] ALANLAR = { "KULLANICIKODU", "MERNIS", "AD", "SOYAD", "PAROLA", "CALISTIGIKURUM", "DOGUMTARIHI", "EMAIL", "TELEFON", "SONGIRISTARIHI", "AKTIFMI", "CALISTIGIBIRIM", "SICILNO", "UNVAN", "PAROLABITISTARIHI", "SESSIONID", "ARAMADAGOSTERME", "AKADEMIKUNVAN", "RUMUZ" };

        /// <summary>
        /// Kullanıcı değişkenlerini ve değerlerini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        public static string TABLOADDEGISKEN = "KULLANICI_DEGISKEN";

        /// <summary>
        /// Kullanıcı değişkenlerini ve değerlerini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarDEGISKEN : int
        {
            /// <summary>
            /// KULLANICIKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANICIKODU = 0,

            /// <summary>
            /// DEGISKEN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DEGISKEN = 1,

            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,

            /// <summary>
            /// DEGER alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DEGER = 3
        }

        /// <summary>
        /// Kullanıcı değişkenlerini ve değerlerini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        public static string[] ALANLARDEGISKEN = { "KULLANICIKODU", "DEGISKEN", "SIRANO", "DEGER" };

        /// <summary>
        /// Kullanıcılar üzerinde yapılan işlemlerin tarihçesini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        public static string TABLOADTARIHCE = "KULLANICIISLEMTARIHCESI";

        /// <summary>
        /// Kullanıcılar üzerinde yapılan işlemlerin tarihçesini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarTARIHCE : int
        {
            /// <summary>
            /// KULLANICIKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANICIKODU = 0,

            /// <summary>
            /// ISLEMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIHI = 1,

            /// <summary>
            /// ISLEMIYAPAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMIYAPAN = 2,

            /// <summary>
            /// ISLEM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEM = 3
        }

        /// <summary>
        /// Kullanıcılar üzerinde yapılan işlemlerin tarihçesini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        public static string[] ALANLARTARIHCE = { "KULLANICIKODU", "ISLEMTARIHI", "ISLEMIYAPAN", "ISLEM" };


        /// <summary>
        /// Kullanıcılar üzerinde yapılan işlemlerin tarihçesini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        public static string TABLOADYETKITARIHCE = "KULLANICIYETKIISLEMTARIHCESI";

        /// <summary>
        /// Kullanıcılar üzerinde yapılan işlemlerin tarihçesini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarYETKITARIHCE : int
        {
            /// <summary>
            /// KULLANICIKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANICIKODU = 0,

            /// <summary>
            /// ISLEMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIHI = 1,

            /// <summary>
            /// ISLEMIYAPAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMIYAPAN = 2,

            /// <summary>
            /// TUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUR = 3,

            /// <summary>
            /// ISLEM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEM = 4
        }

        /// <summary>
        /// Kullanıcılar üzerinde yapılan işlemlerin tarihçesini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        public static string[] ALANLARYETKITARIHCE = { "KULLANICIKODU", "ISLEMTARIHI", "ISLEMIYAPAN", "TUR", "ISLEM" };

        /// <summary>
        /// Kullanıcıların yetkilendirilmelerinde kullanılan yetki değerlerini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        public static string TABLOADKULLANICIYT = "KULLANICI_YT";

        /// <summary>
        /// Kullanıcıların yetkilendirilmelerinde kullanılan yetki değerlerini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarKULLANICIYT : int
        {
            /// <summary>
            /// KULLANICIKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANICIKODU = 0,

            /// <summary>
            /// YTKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YTKOD = 1,

            /// <summary>
            /// DEGER alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DEGER = 2
        }

        /// <summary>
        /// Kullanıcıların yetkilendirilmelerinde kullanılan yetki değerlerini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        public static string[] ALANLARKULLANICIYT = { "KULLANICIKODU", "YTKOD", "DEGER" };

        /// <summary>
        /// Kullanıcıların çalıştığı birimleri tutan görünümün adını tutan sabit değişken.
        /// </summary>
        public static string TABLOADMUHASEBE = "KULLANICI_MUHASEBE";

        /// <summary>
        /// Kullanıcıların çalıştığı birimleri tutan görünümün alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarMUHASEBE : int
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
        /// Kullanıcıların çalıştığı birimleri tutan görünümün alanlarının isimlerini tutan sabit dizi..
        /// </summary>
        public static string[] ALANLARMUHASEBE = { "KOD", "AD" };

        /// <summary>
        /// Kullanıcıların girdikleri sayfalarla ilgili bilgileri tutan tablonun adını tutan sabit değişken.
        /// </summary>
        public static string TABLOADBILGI = "KULLANICI_BILGI";

        /// <summary>
        /// Kullanıcıların çalıştığı birimleri tutan görünümün alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarBILGI : int
        {
            /// <summary>
            /// KULLANICIKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANICIKODU = 0,

            /// <summary>
            /// ISLEMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIHI = 1,

            /// <summary>
            /// ISLEMYERI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMYERI = 2,

            /// <summary>
            /// SAYFAADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SAYFAADI = 3,

            /// <summary>
            /// BILGI1 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BILGI1 = 4,

            /// <summary>
            /// BILGI2 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BILGI2 = 5,

            /// <summary>
            /// BILGI3 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BILGI3 = 6,

            /// <summary>
            /// BILGI4 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BILGI4 = 7,

            /// <summary>
            /// BILGI5 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BILGI5 = 8
        }

        /// <summary>
        /// Kullanıcıların girdikleri sayfalarla ilgili bilgileri tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        public static string[] ALANLARBILGI = { "KULLANICIKODU", "ISLEMTARIHI", "ISLEMYERI", "SAYFAADI", "BILGI1", "BILGI2", "BILGI3", "BILGI4", "BILGI5" };

        /// <summary>
        /// Kullanıcıların tek seferde belirli zaman aralığında giriş yaparken yanlış şifre girişlerinin sayısını tutan tablonun adını tutan sabit değişken
        /// </summary>
        public static string TABLOADGIRISSAYISI = "[KULLANICI_GIRISSAYISI]";
        /// <summary>
        /// Kullanıcıların tek seferde belirli zaman aralığında giriş yaparken yanlış şifre girişlerinin sayısını tutan görünümün alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarDENEMEGIRISSAYISI : int
        {
            /// <summary>
            /// KULLANICIKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANICIKODU = 0,
            /// <summary>
            /// DENEMEBASTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DENEMEBASTARIHI = 1,
            /// <summary>
            /// DENEMEGIRISSAYISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DENEMEGIRISSAYISI = 2
        }
        /// <summary>
        /// Kullanıcıların tek seferde belirli zaman aralığında giriş yaparken yanlış şifre girişlerinin sayısını tutan sabit dizi.
        /// </summary>
        public static string[] ALANLARGIRISSAYISI = { "KULLANICIKODU", "DENEMEBASTARIHI", "DENEMEGIRISSAYISI" };


        /// <summary>
        /// Sistemde verilen kullanıcı koduna sahip kaç kullanıcı olduğunu bulan
        /// sql cümlesini oluşturup, çalıştıran ve kaç kullanıcı olduğunu döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Aranacak kullanıcı kodu.</param>
        /// <returns>Verilen kullanıcı koduna sahip kullanıcı sayısını döndürür.</returns>
        public int KullaniciKoduSayiBul(string kullaniciKodu)
        {
            int donDeger;
            string sql = "SELECT COUNT(*) FROM " + TABLOAD + " WHERE " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", kullaniciKodu);
            IDataReader r = veriErisim.DataReaderDondur(sql);
            if (veriErisim.hata == "")
            {
                if (r.Read())
                    donDeger = VeriErisim.DegerAl(r, 0, 0);
                else
                    donDeger = -2;
                r.Close();
            }
            else
                donDeger = -1;
            return donDeger;
        }

        /// <summary>
        /// Sistemde verilen TC Kimlik numarasına sahip kaç kullanıcı olduğunu bulan
        /// sql cümlesini oluşturup, çalıştıran ve kaç kullanıcı olduğunu döndüren yordam.
        /// </summary>
        /// <param name="tcKimlikNo">Aranacak TC kimlik numarası.</param>
        /// <returns>Verilen kullanıcı koduna sahip kullanıcı sayısını döndürür.</returns>
        public int MernisSayiBul(string tcKimlikNo)
        {
            int donDeger;
            string sql = "SELECT COUNT(*) FROM " + TABLOAD + " WHERE " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MERNIS], "=", tcKimlikNo);
            IDataReader r = veriErisim.DataReaderDondur(sql);
            if (veriErisim.hata == "")
            {
                if (r.Read())
                    donDeger = VeriErisim.DegerAl(r, 0, 0);
                else
                    donDeger = -2;
                r.Close();
            }
            else
                donDeger = -1;
            return donDeger;
        }

        /// <summary>
        /// Veri tabanında kayıtlı ve verilen sql koşuluna uyan kullanıcı listesini döndürmek için 
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="kosul">Listelenecek kullanıcıları belirleyen sql koşulu.</param>
        /// <returns>Kullanıcı listesi bulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        private System.Data.IDataReader Listele(string kosul)
        {
            string sql = "select " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "<>", "sistem"));
            if (kosul != "")
                sql = sql + " AND (" + kosul + ")";
            sql = sql + " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SOYAD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KULLANICIKODU];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Veri tabanında kayıtlı ve verilen sql koşuluna uyan kullanıcı listesini döndürmek için 
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="kosul">Listelenecek kullanıcıları belirleyen sql koşulu.</param>
        /// <returns>Kullanıcı listesi bulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        private System.Data.IDataReader ListeleYetkiDegerleriIle(string kosul)
        {
            string sql = "select " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", KULLANICI_YT.YTKOD, KULLANICI_YT.DEGER FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN KULLANICI_YT ON KULLANICI_YT.KULLANICIKODU=KULLANICI.KULLANICIKODU AND (KULLANICI_YT.YTKOD>0 AND DEGER!='')";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "<>", "sistem"));
            if (kosul != "")
                sql = sql + " AND (" + kosul + ")";
            sql = sql + " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SOYAD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KULLANICIKODU];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Veri tabanında kayıtlı ve verilen kıstasa uyan kullanıcı listesini döndürmek için 
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="kullanici">Seçilecek kullanıcılarla ilgili kıstasları tutan nesne.</param>
        /// <param name="yetkiDegerleriIle">true ise Kullanıcı tablosu ile yetki tipleri tablosunu join eden sorgunun sonucunu döndürür</param>
        /// <returns>Kullanıcı listesi bulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public System.Data.IDataReader ListeleKullanici(Kullanici kullanici, bool yetkiDegerleriIle = false)
        {
            string kosul = "", icKosul = "", tekKosul;
            if (kullanici.mernis != null && kullanici.mernis != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MERNIS], "=", kullanici.mernis));
            //kullanici.
            if (!String.IsNullOrEmpty(kullanici.ADSOYAD))
            {
                //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD] + ' ' + ALANLAR[(int)ENUMAlanlar.SOYAD], "LIKE", kullanici.ad + "%"));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "KULLANICI.AD + ' ' + KULLANICI.SOYAD", "LIKE", "%" + kullanici.ADSOYAD + "%"));
            }
            //if (kullanici.soyad != null && kullanici.soyad != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SOYAD], "LIKE", kullanici.soyad + "%"));
            if (kullanici.kullaniciKodu != null && kullanici.kullaniciKodu != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "LIKE", kullanici.kullaniciKodu + "%"));
            if (kullanici.calistigiKurum != null && kullanici.calistigiKurum != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.CALISTIGIKURUM], "LIKE", kullanici.calistigiKurum + "%"));
            if (kullanici.calistigiBirim != null && kullanici.calistigiBirim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.CALISTIGIBIRIM], "LIKE", "%" + kullanici.calistigiBirim + "%"));
            if (kullanici.aktifMi != ENUMAktiflik.TANIMSIZ)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AKTIFMI], "=", (int)kullanici.aktifMi));
            if (kullanici.emailAdresi != null && kullanici.emailAdresi != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EMAIL], "=", kullanici.emailAdresi));

            if (kullanici.kullaniciTipi != null)
            {
                string ktKosul = "";
                for (int i = 0; i < kullanici.kullaniciTipi.Length; i++)
                {
                    if (kullanici.kullaniciTipi[i] != ENUMKullaniciTipi.TANIMSIZ)
                    {
                        if (!String.IsNullOrEmpty(ktKosul))
                            ktKosul += ", ";
                        ktKosul += ((int)kullanici.kullaniciTipi[i]).ToString();
                    }
                }
                if (!String.IsNullOrEmpty(ktKosul))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "1 in (SELECT 1 FROM KULLANICININTIPLERI WHERE KULLANICININTIPLERI.KULLANICIKODU=KULLANICI.KULLANICIKODU AND KULLANICININTIPLERI.KTKOD IN (" + ktKosul + "))");
                }
            }

            if (kullanici.aramadaGosterme > -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ARAMADAGOSTERME], "=", kullanici.aramadaGosterme));
            if (kullanici.yetkiTipiDegerleri != null)
            {
                try
                {
                    foreach (System.Collections.DictionaryEntry de in kullanici.yetkiTipiDegerleri)
                    {
                        string ytAd = (string)de.Key;
                        string ytDeger = HashTableArrayList.ArrayListToStr(de.Value);
                        if (!String.IsNullOrEmpty(ytAd) && !String.IsNullOrEmpty(ytDeger))
                        {
                            YetkiTipi yt = TNS.KYM.Arac.YetkiTipiBul(ytAd);
                            tekKosul = "";
                            tekKosul = VeriErisim.KosulEkle(tekKosul, "", VeriErisim.KosulOl(TABLOADKULLANICIYT, ALANLARKULLANICIYT[(int)ENUMAlanlarKULLANICIYT.YTKOD], "=", yt.kod));
                            tekKosul = VeriErisim.KosulEkle(tekKosul, "AND", VeriErisim.KosulOl(TABLOADKULLANICIYT, ALANLARKULLANICIYT[(int)ENUMAlanlarKULLANICIYT.DEGER], "LIKE", ytDeger + "%"));
                            tekKosul = "(" + tekKosul + ")";
                            icKosul = VeriErisim.KosulEkle(icKosul, "OR", tekKosul);
                        }
                    }
                }
                catch (Exception)
                {
                    icKosul = "";
                }
            }
            if (icKosul != "")
            {
                icKosul = "SELECT 1 FROM " + TABLOADKULLANICIYT + " WHERE " + TABLOADKULLANICIYT + "." + ALANLARKULLANICIYT[(int)ENUMAlanlarKULLANICIYT.KULLANICIKODU] + " = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KULLANICIKODU] + " AND (" + icKosul + ")";
                kosul = VeriErisim.KosulEkle(kosul, "AND", " 1 IN (" + icKosul + ")");
            }
            if (yetkiDegerleriIle)
                return ListeleYetkiDegerleriIle(kosul);
            else
                return Listele(kosul);
        }


        /// <summary>
        /// Veri tabanında kayıtlı ve verilen kıstasa uyan kullanıcı listesini döndürmek için 
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Eğer 'sessionMi' parametresi Yanlış ise bu parametre kullanıcı kodu veya TC Kimlik numarasıdır. 'sessionMi' parametresi Doğru ise verilen değer session değeridir.</param>
        /// <param name="sessionMi">Verilen 'kullaniciKodu' değerininin kullanıcı kodu veya TC kimlik numarası yoksa session mı olduğı bilgisi. </param>
        /// <returns>Kullanıcı listesi bulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public System.Data.IDataReader Oku(string kullaniciKodu, bool sessionMi)
        {
            if (sessionMi)
                return Listele(VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SESSIONID], "=", kullaniciKodu));
            else
            {
                string kosul = "";
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", kullaniciKodu);
                kosul += " OR " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MERNIS], "=", kullaniciKodu);
                kosul += " OR " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EMAIL], "=", kullaniciKodu);
                //if (!string.IsNullOrWhiteSpace(kullaniciKodu))
                //    kosul += " OR " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.RUMUZ], "=", kullaniciKodu);
                kosul = "(" + kosul + ")";
                return Listele(kosul);
            }
        }

        public System.Data.IDataReader OkuRumuz(string kullaniciKodu)
        {
            string kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.RUMUZ], "=", kullaniciKodu);
            kosul += " OR " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.RUMUZ], "=", kullaniciKodu.ToUpper(new System.Globalization.CultureInfo("en")));
            kosul += " OR " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.RUMUZ], "=", kullaniciKodu.ToUpper());
            kosul = "(" + kosul + ")";

            return Listele(kosul);
        }

        /// <summary>
        /// Kullanıcıya ait yetki tipi kodlarını ve değerlerini bulan
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Kendisine ait yetki tipi değerleri bulunacak kullanıcının kodu.</param>
        /// <returns>Yetki tipi kodlarını ve değerlerini bulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public System.Data.IDataReader YTOku(string kullaniciKodu)
        {
            string sql = "select KULLANICI_YT.YTKOD, KULLANICI_YT.DEGER FROM KULLANICI_YT ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADKULLANICIYT, ALANLARKULLANICIYT[(int)ENUMAlanlarKULLANICIYT.KULLANICIKODU], "=", kullaniciKodu));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak gelen kullanıcının yeni parolasını ve parola bitiş zamanını güncelleyen
        /// update sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Parolası ve parola bitiş zamanını güncellenecek kullanıcı.</param>
        /// <param name="parola">Kullanıcının yeni parolasıç</param>
        /// <param name="zaman">Kullanıcının parola bitiş zamanı.</param>
        /// <returns>Oluşturulan update komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc GuncelleParola(string kullaniciKodu, string parola, TNSDateTime zaman)
        {
            string sql = "UPDATE " + TABLOAD + " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PAROLA], parola) + ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PAROLABITISTARIHI], zaman);
            sql = sql + " WHERE " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], kullaniciKodu);
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen kullanıcının, sisteme giriş zamanını güncelleyen
        /// update sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Giriş zamanı güncellenecek kullanıcı.</param>
        /// <param name="zaman">Kullanıcının sisteme giriş zamanı.</param>
        /// <returns>Oluşturulan update komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc GuncelleSonGiris(string kullaniciKodu, TNSDateTime zaman)
        {
            string sql = "UPDATE " + TABLOAD + " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SONGIRISTARIHI], zaman);// + ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKTIFMI], 1);
            sql = sql + " WHERE " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], kullaniciKodu);
            Sonuc s = KomutCalistir(sql, 1);
            if (!s.islemSonuc)
            {
                this.veriErisim.hata += "(" + sql + ")";
            }
            return s;
        }

        /// <summary>
        /// Parametre olarak gelen kullanıcının aktiflik bilgisini, gelen aktifPasif parametresini
        /// kullanarak değiştirir. UPDATE SQLini oluşturur ve çalıştırır. 
        /// Sonuc yapısında, eğer komut çalışmışsa, true, çalışmamışsa false döndürür. 
        /// Hata olmuşsa, hata stringini, sonuc yapısının ilgili alanına koyar. 
        /// (0 kayıt güncellenmişse de hata döndürür.)
        /// </summary>
        /// <param name="kullanici">Aktiflik bilgisi güncellenecek kullanıcı.</param>
        /// <param name="aktifPasif">Kullanıcının aktif mi, pasif mi yapılacağı bilgisi.</param>
        /// <returns>Oluşturulan update komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc GuncelleAktifPasif(Kullanici kullanici, ENUMAktiflik aktifPasif)
        {
            string sql = "UPDATE " + TABLOAD + " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKTIFMI], (int)aktifPasif);

            //Kullanıcı pasif hale getirildiğinde aktif olabileceği tarih PAROLABITISTARIHI alanında tutuluyor. Melih
            if (aktifPasif == ENUMAktiflik.PASIF)
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PAROLABITISTARIHI], kullanici.parolaBitisTarihi);

            if (aktifPasif == ENUMAktiflik.AKTIF)
            {
                if (!string.IsNullOrWhiteSpace(kullanici.calistigiKurum))
                    sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CALISTIGIKURUM], kullanici.calistigiKurum.Replace(".", ""));
                if (!string.IsNullOrWhiteSpace(kullanici.emailAdresi))
                    sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMAIL], kullanici.emailAdresi);
            }

            if (!string.IsNullOrWhiteSpace(kullanici.unvan))
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UNVAN], kullanici.unvan);

            if (!string.IsNullOrWhiteSpace(kullanici.sicilNo))
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SICILNO], kullanici.sicilNo);

            sql = sql + " WHERE " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], kullanici.kullaniciKodu);
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Bir kullanıcının tüm yetki tipi değerlerini silen 
        /// delete sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="kullanici">Yetki tipi değerleri silinecek kullanıcı.</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc YetkiTipiDegerleriniSil(Kullanici kullanici)
        {
            string sql = "delete from " + TABLOADKULLANICIYT + " WHERE ";
            sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", kullanici.kullaniciKodu));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Bir kullanıcının bir yetki tipini kaydeden 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Yetki tipi değeri kaydedilecek kullanıcının kodu.</param>
        /// <param name="ytKod">Kaydedilecek yetki tipi değerinin kodu.</param>
        /// <param name="yetkiTipiDeger">Kaydedilecek yetki tipi değeri.</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc YetkiTipiDegeriKaydet(string kullaniciKodu, int ytKod, string yetkiTipiDeger)
        {
            string sql = "insert into " + TABLOADKULLANICIYT + "(" + VeriErisim.TumAlanlar("", ALANLARKULLANICIYT) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(kullaniciKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(ytKod) + ", ";
            sql = sql + VeriErisim.DegerYap(yetkiTipiDeger) + ")";
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Bir kullanıcıyı veri tabanına kaydeden 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="kullanici">Veri tabanına kaydedilecek kullanıcının bilgilerini tutan nesne.</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc Ekle(Kullanici kullanici)
        {
            string sql = "insert into " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(kullanici.kullaniciKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(kullanici.mernis) + ", ";
            sql = sql + VeriErisim.DegerYap(kullanici.ad) + ", ";
            sql = sql + VeriErisim.DegerYap(kullanici.soyad) + ", ";
            sql = sql + VeriErisim.DegerYap(kullanici.parola) + ", ";
            sql = sql + VeriErisim.DegerYap(kullanici.calistigiKurum) + ", ";
            sql = sql + VeriErisim.DegerYap(kullanici.dogumTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(kullanici.emailAdresi) + ", ";
            sql = sql + VeriErisim.DegerYap(kullanici.telefon) + ", ";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime()) + ", ";
            sql = sql + VeriErisim.DegerYap((int)ENUMAktiflik.AKTIF) + ", ";
            sql = sql + VeriErisim.DegerYap(kullanici.calistigiBirim) + ", ";
            sql = sql + VeriErisim.DegerYap(kullanici.sicilNo) + ", ";
            sql = sql + VeriErisim.DegerYap(kullanici.unvan) + ", ";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ", ";
            sql = sql + VeriErisim.DegerYap(kullanici.sessionId) + ", ";
            sql = sql + VeriErisim.DegerYap(kullanici.aramadaGosterme) + ", ";
            sql = sql + VeriErisim.DegerYap(kullanici.akademikUnvan) + ", ";
            sql = sql + VeriErisim.DegerYap(kullanici.rumuz) + ") ";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Veri tabanında varolan bir kullanıcının bilgilerini güncelleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="kullanici">Güncellenecek kullanıcı kaydının yeni bilgilerini tutan nesne.</param>
        /// <returns>Oluşturulan update komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc Guncelle(Kullanici kullanici)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MERNIS], kullanici.mernis) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], kullanici.ad) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOYAD], kullanici.soyad) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CALISTIGIKURUM], kullanici.calistigiKurum) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOGUMTARIHI], kullanici.dogumTarihi) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMAIL], kullanici.emailAdresi) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TELEFON], kullanici.telefon) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CALISTIGIBIRIM], kullanici.calistigiBirim) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SICILNO], kullanici.sicilNo) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UNVAN], kullanici.unvan) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SESSIONID], "") + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ARAMADAGOSTERME], "") + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKADEMIKUNVAN], kullanici.akademikUnvan) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RUMUZ], kullanici.rumuz) + " ";
            sql = sql + " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", kullanici.kullaniciKodu);
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Veri tabanında varolan bir kullanıcının sadece kişisel bilgiler kısmını güncelleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="kullanici">Güncellenecek kullanıcı kaydının yeni bilgilerini tutan nesne.</param>
        /// <returns>Oluşturulan update komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc KisiselBilgileriKaydet(Kullanici kullanici)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOGUMTARIHI], kullanici.dogumTarihi) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMAIL], kullanici.emailAdresi) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TELEFON], kullanici.telefon) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CALISTIGIBIRIM], kullanici.calistigiBirim) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SICILNO], kullanici.sicilNo) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UNVAN], kullanici.unvan) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ARAMADAGOSTERME], kullanici.aramadaGosterme) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKADEMIKUNVAN], kullanici.akademikUnvan) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RUMUZ], kullanici.rumuz) + " ";
            sql = sql + " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", kullanici.kullaniciKodu);
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Kullanıcılar üzerinde yapılan işlemlerin tarihçesini kaydeden sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="islemiYapanKulllaniciKodu">İşlemi yapan kullanıcının kodu.</param>
        /// <param name="islemYapilanKullaniciKodu">Üzerinde işlem yapılan kullanıcının kodu.</param>
        /// <param name="islem">Yapılan işlemin kodu.</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc TarihceKaydet(string islemiYapanKulllaniciKodu, string islemYapilanKullaniciKodu, int islem)
        {
            string sql = "insert into " + TABLOADTARIHCE + "(" + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(islemYapilanKullaniciKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime(System.DateTime.Now)) + ", ";
            sql = sql + VeriErisim.DegerYap(islemiYapanKulllaniciKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(islem) + ") ";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Kullanıcı yetkileri üzerinde yapılan işlemlerin tarihçesini kaydeden sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="islemiYapanKulllaniciKodu">İşlemi yapan kullanıcının kodu.</param>
        /// <param name="islemYapilanKullaniciKodu">Üzerinde işlem yapılan kullanıcının kodu.</param>
        /// <param name="tur">Yapılan işlemin turu.</param>
        /// <param name="islem">Yapılan işlemin kodu.</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc YetkiTarihceKaydet(string islemiYapanKulllaniciKodu, string islemYapilanKullaniciKodu, int tur, string islem)
        {
            string sql = "insert into " + TABLOADYETKITARIHCE + "(" + VeriErisim.TumAlanlar("", ALANLARYETKITARIHCE) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(islemYapilanKullaniciKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime(System.DateTime.Now)) + ", ";
            sql = sql + VeriErisim.DegerYap(islemiYapanKulllaniciKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(tur) + ", ";
            sql = sql + VeriErisim.DegerYap(islem) + ") ";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KullaniciDegisiklikTarihceKaydet(string islemiYapanKulllaniciKodu, string islemYapilanKullaniciKodu, int tur, string islem)
        {
            string sql = "insert into " + TABLOADYETKITARIHCE + "(" + VeriErisim.TumAlanlar("", ALANLARYETKITARIHCE) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(islemYapilanKullaniciKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime(System.DateTime.Now)) + ", ";
            sql = sql + VeriErisim.DegerYap(islemiYapanKulllaniciKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(tur) + ", ";
            sql = sql + VeriErisim.DegerYap(islem) + ") ";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Kullanıcıların girdikleri sayfalar ve bilgilerini kaydeden sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Sayfayı açan kullanıcının kodu.</param>
        /// <param name="tarih">Sayfanın açılış tarihi.</param>
        /// <param name="hostName">Talebin yapıldığı yerin ip adresi.</param>
        /// <param name="sayfaAd">İşlem yapılan sayfanın adresi.</param>
        /// <param name="bilgi1">İşlemi yapan kullanıcının adı ve soyadı.</param>
        /// <param name="bilgi2">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi3">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi4">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi5">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc KaydetSession(string kullaniciKodu, TNSDateTime tarih, string hostName, string sayfaAd, string bilgi1, string bilgi2, string bilgi3, string bilgi4, string bilgi5)
        {
            string sql = "INSERT INTO " + "KULLANICI_BILGI" + " (" + VeriErisim.TumAlanlar("", ALANLARBILGI) + ") VALUES (";
            sql += VeriErisim.DegerYap(kullaniciKodu) + ", ";
            sql += VeriErisim.DegerYap(tarih) + ", ";
            sql += VeriErisim.DegerYap(hostName) + ", ";
            sql += VeriErisim.DegerYap(sayfaAd) + ", ";
            sql += VeriErisim.DegerYap(bilgi1) + ", ";
            sql += VeriErisim.DegerYap(bilgi2) + ", ";
            sql += VeriErisim.DegerYap(bilgi3) + ", ";
            sql += VeriErisim.DegerYap(bilgi4) + ", ";
            sql += VeriErisim.DegerYap(bilgi5) + ")";
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Verilen kriterlere uyan ve kullanıcıların girdikleri son sayfaların listesini getiren 
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Listelenecek sayfaları açan kullanıcıların kodu.</param>
        /// <param name="tarih">Listelenecek sayfaların açıldığı tarihin başlangıcı.</param>
        /// <param name="hostName">Listelenecek sayfaların talep edildiği yerin ip adresi.</param>
        /// <param name="sayfaAd">Listelenecek sayfaların adresi. (like)</param>
        /// <param name="bilgi1">Listelenecek sayfaları açan kişilerin adı ve soyadı.</param>
        /// <param name="bilgi2">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi3">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi4">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi5">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <returns>Kullanıcıların girdikleri son sayfaların listesini bulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public System.Data.IDataReader SessionListele(string kullaniciKodu, TNSDateTime tarih, string hostName, string sayfaAd, string bilgi1, string bilgi2, string bilgi3, string bilgi4, string bilgi5)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARBILGI);
            sql += " FROM " + TABLOADBILGI + " A";

            //kosul = ALANLARBILGI[(int)ENUMAlanlarBILGI.ISLEMTARIHI] + " IN (SELECT MAX(" + ALANLARBILGI[(int)ENUMAlanlarBILGI.ISLEMTARIHI] + ") FROM " + TABLOADBILGI + " B WHERE A." + ALANLARBILGI[(int)ENUMAlanlarBILGI.KULLANICIKODU] + "=" + "B." + ALANLARBILGI[(int)ENUMAlanlarBILGI.KULLANICIKODU] + ")";
            kosul = VeriErisim.KosulEkle(kosul, "WHERE", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBILGI.ISLEMTARIHI], ">=", tarih));

            if (kullaniciKodu != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBILGI.KULLANICIKODU], "=", kullaniciKodu));
            if (sayfaAd != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBILGI.SAYFAADI], "LIKE", "%" + sayfaAd + "%"));
            if (hostName != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBILGI.ISLEMYERI], "=", hostName));
            if (bilgi1 != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBILGI.BILGI1], "=", bilgi1));
            if (bilgi2 != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBILGI.BILGI2], "=", bilgi2));
            if (bilgi3 != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBILGI.BILGI3], "=", bilgi3));
            if (bilgi4 != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBILGI.BILGI4], "=", bilgi4));
            if (bilgi5 != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBILGI.BILGI5], "=", bilgi5));

            sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARBILGI[(int)ENUMAlanlarBILGI.ISLEMTARIHI] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Zamana göre sayfa erişim sayılarını bulan
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Zamana göre sayıları bulunacak sayfaları açan kullanıcıların kodu.</param>
        /// <param name="grafikZamanAraligi">Bulunacak sayıların zaman dilimi.</param>
        /// <param name="tarih1">Zamana göre sayıları bulunacak sayfaların açıldığı tarihin başlangıcı.</param>
        /// <param name="tarih2">Zamana göre sayıları bulunacak sayfaların açıldığı tarihin bitişi.</param>
        /// <param name="sayfaAd">Zamana göre sayıları bulunacak sayfaların adresi. (like)</param>
        /// <returns>Zamana göre sayfa erişim sayılarını bulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public System.Data.IDataReader SessionGrafik(string kullaniciKodu, ENUMKullanimBilgiZaman grafikZamanAraligi, TNSDateTime tarih1, TNSDateTime tarih2, string sayfaAd)
        {
            string zaman = "";

            if (this.veriErisim.baglantiTip == "ORACLE")
            {
                if (grafikZamanAraligi == ENUMKullanimBilgiZaman.ZAMAN_15DK) //15 dk
                    zaman = "TRUNC(ISLEMTARIHI)+ (TRUNC(TRUNC((ISLEMTARIHI-TRUNC(ISLEMTARIHI))*1440)/15)*15)/1440";
                else if (grafikZamanAraligi == ENUMKullanimBilgiZaman.ZAMAN_1SAAT) //1 saat
                    zaman = "TRUNC(ISLEMTARIHI,'HH')";
                else if (grafikZamanAraligi == ENUMKullanimBilgiZaman.ZAMAN_GUN) //gün
                    zaman = "TRUNC(ISLEMTARIHI,'DD')";
                else if (grafikZamanAraligi == ENUMKullanimBilgiZaman.ZAMAN_HAFTA) //hafta
                    zaman = "TRUNC(ISLEMTARIHI,'WW')+1";
                else //if (grafikZamanAraligi == ENUMKullanimBilgiZaman.ZAMAN_AY) //ay
                    zaman = "TRUNC(ISLEMTARIHI,'MONTH')";
            }
            else
            {
                long dakika = 0;
                if (grafikZamanAraligi == ENUMKullanimBilgiZaman.ZAMAN_15DK) //15 dk
                    dakika = 15;
                else if (grafikZamanAraligi == ENUMKullanimBilgiZaman.ZAMAN_1SAAT) //1 saat
                    dakika = 60;
                else if (grafikZamanAraligi == ENUMKullanimBilgiZaman.ZAMAN_GUN) //gün
                    dakika = 60 * 24;
                else if (grafikZamanAraligi == ENUMKullanimBilgiZaman.ZAMAN_HAFTA) //hafta
                    dakika = 60 * 24 * 7;
                else //if (grafikZamanAraligi == ENUMKullanimBilgiZaman.ZAMAN_AY) //ay
                    dakika = 60 * 24 * 7 * 4;

                zaman = "DATEADD(minute, DATEDIFF(minute, 0, " + ALANLARBILGI[(int)ENUMAlanlarBILGI.ISLEMTARIHI] + ") / " + dakika + " * " + dakika + ", 0)";
            }

            string kosul = "";
            string sql = "SELECT " + zaman + " AS ZAMAN, COUNT(*) AS ADET";

            sql += " FROM " + TABLOADBILGI;

            if (!string.IsNullOrEmpty(kullaniciKodu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBILGI.KULLANICIKODU], "=", kullaniciKodu));
            if (!tarih1.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBILGI.ISLEMTARIHI], ">=", tarih1));
            if (!tarih2.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBILGI.ISLEMTARIHI], "<", new TNSDateTime(tarih2.Oku().AddDays(1))));
            if (sayfaAd != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBILGI.SAYFAADI], "LIKE", "%" + sayfaAd + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " GROUP BY " + zaman;
            sql += " ORDER BY ZAMAN";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Bir kullanıcının verilen değişkenini silen 
        /// delete sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Verilen değişkeni silinecek kullanıcı.</param>
        /// <param name="degiskenAd">Silinecek değişkenin adı.</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc DegiskenleriSil(string kullaniciKodu, string degiskenAd)
        {
            string sql = "DELETE FROM " + TABLOADDEGISKEN;
            sql = VeriErisim.KosulEkle(sql, " WHERE ", VeriErisim.KosulOl("", ALANLARDEGISKEN[(int)ENUMAlanlarDEGISKEN.KULLANICIKODU], "=", kullaniciKodu));
            sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("", ALANLARDEGISKEN[(int)ENUMAlanlarDEGISKEN.DEGISKEN], "=", degiskenAd));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Bir kullanıcının, bir değişkeninin, verilen sıradaki değerini ekleyen
        /// insert sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Verilen değişken değeri eklenecek kullanıcı.</param>
        /// <param name="degiskenAd">Eklenecek değişkenin adı.</param>
        /// <param name="siraNo">Eklenecek değerin hangi sırada olduğu.</param>
        /// <param name="deger">Eklenecek değişken değer.</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc DegiskenDegerEkle(string kullaniciKodu, string degiskenAd, int siraNo, string deger)
        {
            string sql = "INSERT INTO " + TABLOADDEGISKEN + "(" + VeriErisim.TumAlanlar("", ALANLARDEGISKEN) + ")  VALUES (";
            sql = sql + VeriErisim.DegerYap(kullaniciKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(degiskenAd) + ", ";
            sql = sql + VeriErisim.DegerYap(siraNo) + ", ";
            sql = sql + VeriErisim.DegerYap(deger) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Kullanıcıya ait bir değişkenin veya verilen ada benzeyen değişkenlerin veya tüm değişkenlerin adlarını ve değerlerini bulan
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Kendisine ait değişkenler ve değerleri bulunacak kullanıcının kodu.</param>
        /// <param name="degisken">Değeri bulunacak değişkenlerin adı, boş ise tüm değişkenler, içinde % karakteri geçiyorsa, benzer değişkenler listelenir.</param>
        /// <returns>Değişkenleri ve değerleri bulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public IDataReader DegiskenleriListele(string kullaniciKodu, string degisken)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARDEGISKEN) + " FROM " + TABLOADDEGISKEN;
            sql = VeriErisim.KosulEkle(sql, " WHERE ", VeriErisim.KosulOl("", ALANLARDEGISKEN[(int)ENUMAlanlarDEGISKEN.KULLANICIKODU], "=", kullaniciKodu));
            if (degisken != "")
            {
                string kosulIslec = "=";
                if (degisken.IndexOf("%") > -1)
                    kosulIslec = "LIKE";

                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDEGISKEN[(int)ENUMAlanlarDEGISKEN.DEGISKEN], kosulIslec, degisken));
            }

            sql += " ORDER BY 1,2,3";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Muhasebe listesini döndüren select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <returns>Muhasebe listesini döndüren select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public IDataReader MuhasebeListele()
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARMUHASEBE) + " FROM " + TABLOADMUHASEBE;

            sql += " ORDER BY 1,2";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KDSRolListele()
        {
            string sql = "SELECT * FROM KDSYETKISABLON";
            sql += " ORDER BY 1,2";
            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader KDSBirimListele()
        {
            string sql = "SELECT * FROM KDSBIRIM";
            sql += " ORDER BY 1,2";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader GITRolListele()
        {
            string sql = "SELECT * FROM GITYETKISABLON";
            sql += " ORDER BY 1,2";
            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader GITBirimListele()
        {
            string sql = "SELECT * FROM GITBIRIM";
            sql += " ORDER BY 1,2";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader YTOku(string[] kullaniciKodu, int yetkiTipKod)
        {
            string sql = "SELECT KULLANICI_YT.KULLANICIKODU, KULLANICI_YT.YTKOD, KULLANICI_YT.DEGER FROM KULLANICI_YT ";
            sql += " WHERE (KULLANICIKODU IN (";
            foreach (string kKod in kullaniciKodu)
                sql += string.Format("'{0}',", kKod);
            sql = sql.Substring(0, sql.Length - 1);
            sql += ")) ";
            if (yetkiTipKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADKULLANICIYT, ALANLARKULLANICIYT[(int)ENUMAlanlarKULLANICIYT.YTKOD], "=", yetkiTipKod));

            sql += " ORDER BY KULLANICIKODU ";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc DBHataYaz(string kullaniciKodu, string tur, string bilgi, string ipAdresi)
        {
            string kod = Guid.NewGuid().ToString();

            string sql = "INSERT INTO KYMHATA(KOD,TUR, ISLEMYAPAN, ISLEMTARIH, HATA, IPADRESI)  VALUES (";
            sql = sql + VeriErisim.DegerYap(kod) + ", ";
            sql = sql + VeriErisim.DegerYap(tur) + ", ";
            sql = sql + VeriErisim.DegerYap(kullaniciKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.Replace("'", " ").Replace("\"", " ")) + ",";
            sql = sql + VeriErisim.DegerYap(ipAdresi) + ")";
            return KomutCalistir(sql, 1);
        }

        //public IDataReader 
        public IDataReader KullaniciGirisSayisiListele(string kullaniciKodu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADGIRISSAYISI, ALANLARGIRISSAYISI) + " FROM " + TABLOADGIRISSAYISI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADGIRISSAYISI, ALANLARGIRISSAYISI[(int)ENUMAlanlarDENEMEGIRISSAYISI.KULLANICIKODU], "=", kullaniciKodu));
            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc KullaniciGirisSayisiKaydet(string kullaniciKodu, int denemeGirisSayisi)
        {
            string sql = "INSERT INTO " + TABLOADGIRISSAYISI + "(" + VeriErisim.TumAlanlar("", ALANLARGIRISSAYISI) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(kullaniciKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime(System.DateTime.Now)) + ", ";
            sql = sql + VeriErisim.DegerYap(denemeGirisSayisi) + ") ";
            return KomutCalistir(sql, 1);
        }

        public Sonuc KullaniciGirisSayisiGuncelle(string kullaniciKodu, int denemeGirisSayisi)
        {
            string sql = "UPDATE " + TABLOADGIRISSAYISI + " SET ";
            sql = sql + VeriErisim.AtamaOl("", ALANLARGIRISSAYISI[(int)ENUMAlanlarDENEMEGIRISSAYISI.DENEMEBASTARIHI], new TNSDateTime(System.DateTime.Now)) + ", ";
            sql = sql + VeriErisim.AtamaOl("", ALANLARGIRISSAYISI[(int)ENUMAlanlarDENEMEGIRISSAYISI.DENEMEGIRISSAYISI], denemeGirisSayisi) + " ";
            sql = sql + " WHERE " + VeriErisim.KosulOl("", ALANLARGIRISSAYISI[(int)ENUMAlanlarDENEMEGIRISSAYISI.KULLANICIKODU], "=", kullaniciKodu);
            return KomutCalistir(sql, 1);
        }

        public IDataReader IslemTarihceRapor(TNSDateTime tarih1, TNSDateTime tarih2, string islemYapilanTCNo, string islemYapilanAd, string islemiYapanKullaniciAdi, int kullaniciTipi)
        {
            string sql = "SELECT KULLANICIISLEMTARIHCESI.KULLANICIKODU AS KULLANICIKODU, ISLEMTARIHI, ISLEMIYAPAN, ISLEM AS TARIHCEISLEM, '' AS YETKIISLEM, 0 AS TUR, KULLANICI.AD+' '+KULLANICI.SOYAD, KULLANICI.MERNIS";
            sql += " FROM " + TABLOADTARIHCE;
            sql += " LEFT OUTER JOIN KULLANICI ON KULLANICI.KULLANICIKODU = KULLANICIISLEMTARIHCESI.KULLANICIKODU";

            sql = VeriErisim.KosulEkle(sql, "WHERE ", VeriErisim.KosulOl("", "ISLEMTARIHI", ">=", tarih1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ISLEMTARIHI", "<=", tarih2));

            if (islemYapilanTCNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("KULLANICI", "MERNIS", "=", islemYapilanTCNo));

            if (islemYapilanAd != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("KULLANICIISLEMTARIHCESI", "KULLANICIKODU", "=", islemYapilanAd));

            if (islemiYapanKullaniciAdi != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ISLEMIYAPAN", "=", islemiYapanKullaniciAdi));

            sql += " UNION";

            sql += " SELECT KULLANICIYETKIISLEMTARIHCESI.KULLANICIKODU AS KULLANICIKODU, ISLEMTARIHI, ISLEMIYAPAN, 0 AS TARIHCEISLEM, ISLEM AS YETKIISLEM, TUR, KULLANICI.AD+' '+KULLANICI.SOYAD, KULLANICI.MERNIS";
            sql += " FROM " + TABLOADYETKITARIHCE;
            sql += " LEFT OUTER JOIN KULLANICI ON KULLANICI.KULLANICIKODU = KULLANICIYETKIISLEMTARIHCESI.KULLANICIKODU";

            sql = VeriErisim.KosulEkle(sql, "WHERE ", VeriErisim.KosulOl("", "ISLEMTARIHI", ">=", tarih1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ISLEMTARIHI", "<=", tarih2));

            if (islemYapilanTCNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("KULLANICI", "MERNIS", "=", islemYapilanTCNo));

            if (islemYapilanAd != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("KULLANICIYETKIISLEMTARIHCESI", "KULLANICIKODU", "=", islemYapilanAd));

            if (islemiYapanKullaniciAdi != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ISLEMIYAPAN", "=", islemiYapanKullaniciAdi));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc LogYaz(int satirNo, IISLogSatir logSatir)
        {
            string sql = "INSERT INTO IISLOG VALUES (";
            sql = sql + VeriErisim.DegerYap(logSatir.IslemSaat) + ", ";
            sql = sql + VeriErisim.DegerYap(satirNo) + ", ";
            sql = sql + VeriErisim.DegerYap(logSatir.s_ip) + ", ";
            sql = sql + VeriErisim.DegerYap(logSatir.cs_method) + ", ";
            sql = sql + VeriErisim.DegerYap(logSatir.cs_uri_stem) + ", ";
            sql = sql + VeriErisim.DegerYap(logSatir.cs_uri_query) + ", ";
            sql = sql + VeriErisim.DegerYap(logSatir.s_port) + ", ";
            sql = sql + VeriErisim.DegerYap(logSatir.cs_username) + ", ";
            sql = sql + VeriErisim.DegerYap(logSatir.c_ip) + ", ";
            sql = sql + VeriErisim.DegerYap(logSatir.cs_User_Agent_) + ", ";
            sql = sql + VeriErisim.DegerYap(logSatir.cs_Referer_) + ", ";
            sql = sql + VeriErisim.DegerYap(logSatir.sc_status) + ", ";
            sql = sql + VeriErisim.DegerYap(logSatir.sc_substatus) + ", ";
            sql = sql + VeriErisim.DegerYap(logSatir.sc_win32_status) + ", ";
            sql = sql + VeriErisim.DegerYap(logSatir.time_taken) + ") ";
            return KomutCalistir(sql, 0);
        }
    }
}

/*
        public Sonuc SessionSil(string sessionId)
        {
            string sql = "UPDATE " + TABLOAD + " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SESSIONID], "");
            sql = sql + " WHERE " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SESSIONID], sessionId);
            sql = sql + " ORDER BY 1,2,3";
            return KomutCalistir(sql, 0);
        }


 * /*
       Sistem kayıtlı, ve paremetre olarak kendisine geçirilen kuruma ait kullanıcıları bulacak sql koşulunu oluşturup, 
       Listele metodunu çağırır.
   * /
        public System.Data.IDataReader ListeleKurum(string kurum)
        {
            return Listele(VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.CALISTIGIKURUM], "=", kurum));
        }

        /*
                 Kullanıcının silinen yetkilerine ait yetki tiplerini siler
             * /
        public Sonuc YTSil(string kullaniciKodu)
        {
            string sql = "";
            sql = "DELETE FROM KULLANICI_YT ";
            sql += " WHERE " + VeriErisim.KosulOl("KULLANICI_YT", "KULLANICIKODU", "=", kullaniciKodu);
            sql += " AND YTKOD IN (SELECT YTKOD FROM KT_YT ";
            sql += "WHERE KTKOD NOT IN(SELECT KTKOD FROM KULLANICININTIPLERI WHERE ";
            sql += VeriErisim.KosulOl("KULLANICININTIPLERI", "KULLANICIKODU", "=", kullaniciKodu);
            sql += "))";

            return KomutCalistir(sql, 0);
        }


        public IDataReader TabloIsimleriListe(string tip)
        {
            string sql = "";
            if (tip == "ORACLE")
                sql = "SELECT TABLE_NAME FROM USER_TABLES ORDER BY TABLE_NAME";
            else
                sql = "SELECT NAME FROM SYSOBJECTS WHERE TYPE='U' ORDER BY NAME";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader TabloIcerikListele(string tabloAd)
        {
            string sql = "SELECT * FROM " + tabloAd;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc TabloIcerikKaydet(string sql)
        {
            return KomutCalistir(sql, 1);
        }

        /*
            Son girişinin üzerinden 1 yıl geçmiş kullanıcıların listesini bulan sql hazırlanır ve recordset olarak döndürülür.
        * /
        public System.Data.IDataReader ListeleKullanmayanlar()
        {
            string kosul = "";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SONGIRISTARIHI], "<", new TNSDateTime(DateTime.Now.AddDays(-365))));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AKTIFMI], "=", (int)ENUMAktiflik.AKTIF));
            return Listele(kosul);
        }



*/
