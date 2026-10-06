using System;
using System.Runtime;
using System.Runtime.Remoting;
using System.Data.SqlClient;
using System.Security.Cryptography;
using TNS;
using TNS.Veri;
using TNS.KYM;
using TNS.KYM.IsKurallari;
using System.Text;
using TNS.ORG;
using System.IO;
using System.Threading;
using TNS.ORG.IsKurallari;

namespace TNS.KYM.IsKurallari
{
    /// <summary>
    /// Kullanıcı sunucusu tarafından sunulan işlemlerin gerçekleştiren sınıf. Bu sınıf ilgili yordamları çağırır.
    /// </summary>
    [Serializable()]
    public class KYMServis : MarshalByRefObject, TNS.KYM.IKYMServis, TNS.ORG.IORGServis
    {
        static bool zamanBasladi = false;
        public static bool tabloDegisimYap = true;
        static KYMServis()
        {
            lock ("OncekiIslemiBiteneKadarBekle")
            {
                if (zamanBasladi == false)
                {
                    zamanBasladi = true;
                    Zamanlayici.Start();

                    //Temp klasörünü temizle
                    Thread thread = new Thread(TempKlasorunuTemizle);
                    thread.IsBackground = true;
                    thread.Start();

                    //TabloDegisimYap();
                }
                if (tabloDegisimYap)
                {
                    tabloDegisimYap = false;
                    TNS.KYM.IsKurallari.KYMTabloDegisiklikleri.Kaydet();
                }
            }
        }

        //static bool tabloGuncelle = false;
        //private static void TabloDegisimYap()
        //{
        //    if (tabloGuncelle == false)
        //    {
        //        tabloGuncelle = true;
        //        TNS.Veri.VeriErisim ve = TNS.Veri.VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(null));


        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE KYMEKLIDOSYA ADD ONAYLAYAN VARCHAR(50), ONAYZAMAN DATETIME");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery("CREATE TABLE KYMKULLANICIMESAJ (KULLANICIKODU varchar(20) NOT NULL, MESAJNO int NOT NULL, TARIH datetime NULL, MESAJ varchar(2000) NULL, PRIMARY KEY CLUSTERED (KULLANICIKODU ASC, MESAJNO ASC))");

        //        ////Manas IK da pasif olan kişinin Kullanıcı veritabanından da pasif olması. Can IK da bunu yaptığında kaldırılabilir. Melig 29.11.2019
        //        //ve.hata = "";
        //        //ve.ExecuteNonquery("UPDATE Kullanici.dbo.KULLANICI SET AKTIFMI=0 WHERE AKTIFMI=1 AND MERNIS IN (SELECT TCKIMLIKNO FROM InsanKaynaklari.dbo.IKYPERSONEL WHERE AKTIFMI=0)");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE IISLOG (
        //        //IslemSaat DateTime,
        //        //SatirNo int,
        //        //s_ip varchar(20),
        //        //cs_method varchar(20),
        //        //cs_uri_stem varchar(2000),
        //        //cs_uri_query varchar(2000),
        //        //s_port varchar(20),
        //        //cs_username varchar(200),
        //        //c_ip varchar(20),
        //        //cs_User_Agent_ varchar(200),
        //        //cs_Referer_ varchar(2000),
        //        //sc_status varchar(20),
        //        //sc_substatus varchar(20),
        //        //sc_win32_status varchar(20),
        //        //time_taken varchar(20), primary key(IslemSaat, SatirNo))");


        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE[dbo].[KYMZAMANLAYICI] (
        //        //[KOD][varchar](50) NOT NULL,
        //        //[TUR] [varchar] (50) NULL,
        //        //[ISLEMADI] [varchar] (50) NULL,
        //        //[ZAMAN] [datetime] NULL,
        //        //CONSTRAINT[PK_KYMZAMANLAYICI] PRIMARY KEY CLUSTERED
        //        //([KOD] ASC) WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE NONCLUSTERED INDEX IX_KYMZAMANLAYICI ON dbo.KYMZAMANLAYICI (
        //        //ISLEMADI, TUR) WITH(STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE[dbo].[KULLANICI_GIRISSAYISI]
        //        //([KULLANICIKODU][varchar](50) NOT NULL,
        //        //[DENEMEBASTARIHI] [datetime] NULL,
        //        //[DENEMEGIRISSAYISI] [int] NULL,
        //        //CONSTRAINT[PK_KULLANICI_GIRISSAYISI] PRIMARY KEY CLUSTERED
        //        //([KULLANICIKODU] ASC
        //        //)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
        //        //) ON[PRIMARY]");


        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE[dbo].[KYMHATA](
        //        //    KOD[varchar](50) NOT NULL,
        //        //    [TUR] [varchar] (20) NULL,
        //        //    [ISLEMYAPAN] [varchar] (300) NULL,
        //        //    [ISLEMTARIH] [datetime] NULL,
        //        //    [HATA] [varchar] (2000) NULL,
        //        //    [IPADRESI] [varchar] (50) NULL
        //        //    CONSTRAINT[PK_KYMHATA] PRIMARY KEY CLUSTERED ([KOD] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE YERLESKEODA (
        //        //    KOD int NOT NULL,
        //        //    AD varchar(200) NULL,
        //        //    GORKOD varchar(50) NULL,
        //        //    TARIF varchar(2000) NULL,
        //        //    SAYMANLIKKOD int NULL,
        //        //    BIRIMKOD varchar(3) NULL,
        //        //    ALTDALVAR int NULL,
        //        //    USTKOD int NULL,
        //        //    YERTURKOD int NULL,
        //        //    ODATURKOD int NULL,
        //        //    ILKOD varchar(20) NULL,
        //        //    ORGKOD int NULL,
        //        //    PRIMARY KEY(KOD))");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery("ALTER TABLE dbo.YERLESKEODA ADD DURUM INT NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE KYMEKLIDOSYA
        //        //    (MODUL VARCHAR(50)
        //        //    , FORM VARCHAR(50)
        //        //    , DOSYANO INT
        //        //    , DOSYAADI VARCHAR(500)
        //        //    , BUYUKLUK INT
        //        //    , YUKLEMETARIHI DATETIME
        //        //    , YUKLEYEN VARCHAR(50)
        //        //    , PRIMARY KEY (MODUL, FORM, DOSYANO))");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE KYMEKLIDOSYAPARCALARI
        //        //    (MODUL VARCHAR(50)
        //        //    , FORM VARCHAR(50)
        //        //    , DOSYANO INT
        //        //    , PARCANO INT
        //        //    , PARCA IMAGE
        //        //    , PRIMARY KEY (MODUL, FORM, DOSYANO, PARCANO))");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery("ALTER TABLE dbo.DUYURU ADD PENCEREDE INT NOT NULL DEFAULT 2, KONU varchar(500) NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE KYMBIRIMMESAJ
        //        //                        (SAYMANLIKKOD INT
        //        //                        , BIRIMKOD VARCHAR(20)
        //        //                        , KURUMKOD VARCHAR(20)
        //        //                        , SIRANO INT
        //        //                        , MESAJ VARCHAR(8000)
        //        //                        , KAYDEDEN VARCHAR(200)
        //        //                        , KAYITTARIHI DATETIME
        //        //                        , KARSILAYAN VARCHAR(200)
        //        //                        , KARSILAMATARIHI DATETIME
        //        //                        , SAYFA VARCHAR(200)
        //        //                        , DURUM INT
        //        //                        , PRIMARY KEY (SAYMANLIKKOD, BIRIMKOD, KURUMKOD, SIRANO))");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE KYMEVRAKAKTARIM
        //        //                        (EVRAKKODU VARCHAR(30)
        //        //                        , ALANKODU VARCHAR(30)
        //        //                        , ALANDEGER VARCHAR(600)
        //        //                        , PRIMARY KEY (EVRAKKODU, ALANKODU))");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery("ALTER TABLE dbo.KULLANICI ADD AKADEMIKUNVAN varchar(20) NULL, RUMUZ varchar(50) NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery("ALTER TABLE dbo.YERLESKEODA ADD MALIYETMERKEZI int NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery("ALTER TABLE dbo.YERLESKEODA ADD GORKODESKI varchar(50) NULL");

        //        ve.VeriErisimKapat();
        //    }
        //}

        static void TempKlasorunuTemizle()
        {
            try
            {
                //1 saat öncesine kadar yaratılan dosyaları sil
                string tmpKlasor = TNS.OrtakFonksiyonlar.GeciciYolBul();
                DirectoryInfo dirInfo = new DirectoryInfo(tmpKlasor);

                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    try
                    {
                        if (file.CreationTime < DateTime.Now.AddHours(-1))
                            file.Delete();
                    }
                    catch { }
                }
                foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                {
                    try
                    {
                        if (dir.CreationTime < DateTime.Now.AddHours(-1))
                            dir.Delete(true);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GeciciYolBul :" + ex.Message);
            }
        }

        /// <summary>
        /// Kullanıcı tipleri için, değer girilerek kullanıcının yapabileceği işlemleri kısıtlayan yetki tiplerinin listesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">Yetki tipi listesi döndürülürken, eğer sadece bu kullanıcıya ait olanlar döndürülecekse kullanılan ve işlemi yapan kullanıcının bilgilerini tutan parametre.</param>
        /// <param name="yetkiTipi">Veri giriş listesi döndürülecek yetki tipi parametresi.</param>
        /// <param name="yil">Yetki tipi listesi döndürülürken, eğer sadece belirli bir yıla ait olanlar döndürülecekse kullanılan parametre.</param>
        /// <param name="ekParamCagri">Yetki tipi listesi döndürülürken, her çağrı için verilen ve çağrıdan çağrıya anlamı ve biçimi değişen ek parametre. Anlam ve biçim için ilgili çağrıyı gerçekleştiren yordama bakmak gerekir.</param>
        /// <returns>Verilen kıstaslara göre elde edilen YetkiTipiListeElemani dizisi döndürülür.</returns>
        public ListeSatir[] YetkiTipiGirisListesi(Kullanici kullanan, YetkiTipi yetkiTipi, int yil, string ekParamCagri)
        {
            if (string.IsNullOrEmpty(yetkiTipi.dllAd) || string.IsNullOrEmpty(yetkiTipi.nesneAd)) return null;
            try
            {
                System.Runtime.Remoting.ObjectHandle oh = Activator.CreateInstance(yetkiTipi.dllAd, yetkiTipi.nesneAd);
                YetkiTipiListesi rr = (YetkiTipiListesi)oh.Unwrap();

                return rr.YetkiTipiListeElemanlari(kullanan, yil, ekParamCagri, yetkiTipi.ekParam);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(KullaniciResI.KYM003, yetkiTipi.dllAd, yetkiTipi.nesneAd) + ex.Message);
            }
        }


        /// <summary>
        /// Kullanıcı tipleri için, değer girilerek kullanıcının yapabileceği işlemleri kısıtlayan yetki tiplerinin listesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">Yetki tipi listesi döndürülürken, eğer sadece bu kullanıcıya ait olanlar döndürülecekse kullanılan ve işlemi yapan kullanıcının bilgilerini tutan parametre.</param>
        /// <param name="yetkiTipi">Veri giriş listesi döndürülecek yetki tipi parametresi.</param>
        /// <param name="yil">Yetki tipi listesi döndürülürken, eğer sadece belirli bir yıla ait olanlar döndürülecekse kullanılan parametre.</param>
        /// <param name="ustKod">Döndürülecek listenin ait olduğu dalın kodu.</param>
        /// <param name="ekParamCagri">Yetki tipi listesi döndürülürken, her çağrı için verilen ve çağrıdan çağrıya anlamı ve biçimi değişen ek parametre. Anlam ve biçim için ilgili çağrıyı gerçekleştiren yordama bakmak gerekir.</param>
        /// <returns>Verilen kıstaslara göre elde edilen YetkiTipiListeElemani dizisi döndürülür.</returns>
        public ListeSatir[] YetkiTipiAgacListesi(Kullanici kullanan, YetkiTipi yetkiTipi, int yil, string ustKod, string ekParamCagri)
        {
            if (string.IsNullOrEmpty(yetkiTipi.dllAd) || string.IsNullOrEmpty(yetkiTipi.nesneAd)) return null;
            try
            {
                System.Runtime.Remoting.ObjectHandle oh = Activator.CreateInstance(yetkiTipi.dllAd, yetkiTipi.nesneAd);
                YetkiTipiAgaci rr = (YetkiTipiAgaci)oh.Unwrap();

                return rr.YetkiTipiAgacElemanlari(kullanan, yil, ustKod, ekParamCagri, yetkiTipi.ekParam);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(KullaniciResI.KYM003, yetkiTipi.dllAd, yetkiTipi.nesneAd) + ex.Message);
            }
        }

        public string YetkiTipiAgacKodAd(Kullanici kullanan, YetkiTipi yetkiTipi, int yil, string kod, string ekParamCagri)
        {
            if (string.IsNullOrEmpty(yetkiTipi.dllAd) || string.IsNullOrEmpty(yetkiTipi.nesneAd)) return null;
            try
            {
                System.Runtime.Remoting.ObjectHandle oh = Activator.CreateInstance(yetkiTipi.dllAd, yetkiTipi.nesneAd);
                YetkiTipiAgaci rr = (YetkiTipiAgaci)oh.Unwrap();

                return rr.YetkiTipiKodAd(kullanan, yil, kod, ekParamCagri, yetkiTipi.ekParam);
            }
            catch (Exception ex)
            {
                return string.Format(KullaniciResI.KYM003, yetkiTipi.dllAd, yetkiTipi.nesneAd) + ex.Message;
            }
        }

        /// <summary>
        /// Verilen kıstaslara uyan kullanıcı listesini döndüren yordam.
        /// </summary>
        /// <param name="aramaBilgisi">Listelenecek kullanıcıların bilgileri tutan Kullanıcı nesnesi.</param>
        /// <param name="basla">Sonuç kümesinde bu parametre ile belirtilen sıradaki kayıttan itibaren olanlar alınır.</param>
        /// <param name="say">Sonuç kümesinde bu parametre ile verilen sayıdaki kadar eleman yer alır. 0 ise tüm elemanlar sonuç kümesinde yer alır.</param>
        /// <returns>Verilen kıstaslara uyan ve basla parametresiyle belirtilen sıradan başlayarak, say parametresi ile belirtilen kadar kayıt döndürülür.</returns>
        public ObjectArray<TNS.KYM.Kullanici> KullaniciAra(Kullanici aramaBilgisi, int basla, int say)
        {
            try
            {
                return new KullaniciServis().KullaniciAra(null, aramaBilgisi, basla, say);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Verilen kıstaslara uyan kullanıcı listesini döndüren yordam.
        /// </summary>
        /// <param name="aramaBilgisi">Listelenecek kullanıcıların bilgileri tutan Kullanıcı nesnesi.</param>
        /// <returns>Verilen kıstaslara uyan ve basla parametresiyle belirtilen sıradan başlayarak, say parametresi ile belirtilen kadar kayıt döndürülür.</returns>
        public ObjectArray<TNS.KYM.Kullanici> KullaniciRaporla(Kullanici aramaBilgisi)
        {
            try
            {
                return new KullaniciServis().KullaniciRaporla(null, aramaBilgisi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Sisteme giriş kodu veya TC kimlik numarası verilen kullanıcının bilgilerini okuyup döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="kullaniciKoduVeyaTCKimlikNo">Bilgileri döndürülecek kullanıcının sisteme giriş kodu veya TC kimlik numarası.</param>
        /// <returns>Sisteme giriş kodu veya TC kimlik numarası verilen kullanıcının bilgileri döndürülür.</returns>
        public KullaniciVAN KullaniciOku(Kullanici kullanan, string kullaniciKoduVeyaTCKimlikNo)
        {
            try
            {
                return new KullaniciServis().Oku(null, kullanan, kullaniciKoduVeyaTCKimlikNo);
            }
            catch (Exception e)
            {
                KullaniciVAN donusDeger = new KullaniciVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// "kullanici" parametresi ile verilen kullanıcı bilgilerini veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="kullanici">Kaydedilecek kullanıcının bilgilerini tutan parametre.</param>
        /// <param name="yeniMi">Doğru ise daha önce sistemde kayıtlı olmayan bir kullanıcının verilerinin kaydedildiği yordama söylenmiş olur.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KullaniciKaydet(Kullanici kullanan, Kullanici kullanici, bool yeniMi)
        {
            try
            {
                return new KullaniciServis().Kaydet(null, kullanan, kullanici, yeniMi);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// "kullanici" parametresi ile verilen kullanıcıyı pasif yapan yordam. Kullanıcıyı pasif yapan kullanıcı, pasif yapılan kullanıcıyı yönetebilmelidir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="kullanici">Pasif yapılacak kullanıcının bilgilerini tutan parametre.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KullaniciyiPasifYap(Kullanici kullanan, Kullanici kullanici)
        {
            try
            {
                return new KullaniciServis().AktifPasifYap(null, kullanan, kullanici, ENUMAktiflik.PASIF);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// "kullanici" parametresi ile verilen kullanıcıyı aktif yapan yordam. Kullanıcıyı aktif yapan kullanıcı, aktif yapılan kullanıcıyı yönetebilmelidir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="kullanici">Aktif yapılacak kullanıcının bilgilerini tutan parametre.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KullaniciyiAktifYap(Kullanici kullanan, Kullanici kullanici)
        {
            try
            {
                return new KullaniciServis().AktifPasifYap(null, kullanan, kullanici, ENUMAktiflik.AKTIF);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Kullanıcı için, yetki kontrolleri yapıldıktan sonra yeni parola üreten ve kullanıcıya mail atan yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="kullanici">Sistem tarafından üretilmiş parola atanacak kullanıcı.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ParolaAta(Kullanici kullanan, Kullanici kullanici)
        {
            try
            {
                return new KullaniciServis().ParolaAta(null, kullanan, kullanici);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// 6 basamaklı yeni parola üreten ve ürettiği parolayı sonuc yapısının anahtar alanında döndüren yordam.
        /// </summary>
        /// <returns>Üretilen parola sonuc yapısının anahtar alanında döndürülür.</returns>
        public Sonuc ParolaUret()
        {
            Sonuc donusDeger = new Sonuc(true, "", "");
            donusDeger.anahtar = new KullaniciServis().ParolaUret();
            return donusDeger;
        }

        /// <summary>
        /// Kullanıcı nesnesinde bilgileri verilen kullanıcı için ad ve soyad alanlarını kullanarak, olası kullancı adları üreten ve döndüren yordam.
        /// </summary>
        /// <param name="kullanici">Kullanıcı adı üretilecek kullanıcıya ait bilgileri tutan parametre.</param>
        /// <returns>Üretilen kullanıcı adları liste olarak döndürülür.</returns>
        public ObjectArray<TNS.KYM.Kullanici> KullaniciAdOner(Kullanici kullanici)
        {
            try
            {
                return new KullaniciServis().KullaniciAdOner(kullanici);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Sisteme giriş kullanıcı kodu ve parola ile, eğer verilen kod ve parola geçerli ise ve kullanıcı aktif ise kullanıcı sisteme girişine izin verilir ve bilgileri döndürülür.
        /// </summary>
        /// <param name="kullaniciKodu">Sisteme giriş kullanıcı kodu.</param>
        /// <param name="parola">Sisteme giriş parolası.</param>
        /// <returns>Kullanıcı kodu ve şifre geçerli ise ve kullanıcı aktif ise, aktarım nesnesinin kullanıcı alanı kullanıcı bilgileri doldurulur ve sonuc yapısının islemSonuc alanına Doğru atanır. Aksi takdirde sonuc yapısının islemSonuc alanına Yanlış atanır.</returns>
        public KullaniciVAN GirisYap(string kullaniciKodu, string parola)
        {
            try
            {
                return new KullaniciServis().GirisYap(null, kullaniciKodu, parola);
            }
            catch (Exception e)
            {
                KullaniciVAN donusDeger = new KullaniciVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Sisteme giriş kullanıcı kodu ve parolası ile, eğer verilen kod ve parola geçerli ise ve kullanıcı aktif ise kullanıcının bilgileri döndürülür.
        /// </summary>
        /// <param name="kullaniciKodu">Sisteme giriş kullanıcı kodu.</param>
        /// <param name="parola">Sisteme giriş parolası.</param>
        /// <param name="sifreli">Gönderilen parola açık mı şifreli mi bilgisi.</param>
        /// <returns>Kullanıcı kodu ve şifre geçerli ise ve kullanıcı aktif ise, aktarım nesnesinin kullanıcı alanı kullanıcı bilgileri doldurulur ve sonuc yapısının islemSonuc alanına Doğru atanır. Aksi takdirde sonuc yapısının islemSonuc alanına Yanlış atanır.</returns>
        public KullaniciVAN GirisYapabilirMi(string kullaniciKodu, string parola, bool sifreli)
        {
            try
            {
                return new KullaniciServis().GirisYapabilirMi(null, kullaniciKodu, parola, sifreli);
            }
            catch (Exception e)
            {
                KullaniciVAN donusDeger = new KullaniciVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Verilen parola doğruysa, yeni parolalar aynıysa ve yeni parolaların uzunluğu 4 karakterden fazla ise, kullanıcının parolası değiştirilir ve yeni parola değiştirme zamanı atanır.
        /// </summary>
        /// <param name="kullanan">Parolası değiştirilecek kullanıcı.</param>
        /// <param name="eskiParola">Kullanıcının şu andaki parolası.</param>
        /// <param name="yeniParola1">Kullanıcının olması istenen parolası.</param>
        /// <param name="yeniParola2">Kullanıcının olması istenen parolası.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ParolaDegistir(Kullanici kullanan, string eskiParola, string yeniParola1, string yeniParola2)
        {
            try
            {
                return new KullaniciServis().ParolaDegistir(null, kullanan, eskiParola, yeniParola1, yeniParola2);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// "kullanan" parametresi ile verilen kullanıcının, "kullanici" parametresi ile verilen kullanıcıyı yönetip yönetemediğini bulup, döndüren yordam.
        /// </summary>
        /// <param name="kullanan">Yönetme hakkına sahip mi diye kontrol edilecek kullanıcı parametresi.</param>
        /// <param name="kullanici">Yönetilip yönetilmediği bulunacak kullanıcı parametresi.</param>
        /// <returns>"kullanan", "kullanici"yi yönetiyorsa Doğru, değilse Yanlış döndürülür.</returns>
        public bool YonetiyorMu(Kullanici kullanan, Kullanici kullanici)
        {
            try
            {
                return new KullaniciServis().YonetiyorMu(kullanan, kullanici);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// TC kimlik numarası verilen kullanıcı varsa adı ve soyadı, yoksa boş dizgi döndüren yordam. 
        /// </summary>
        /// <param name="tcKimlikNo">Kullanıcı listesinde var olup olmadığı aranacak kullanıcının TC kimlik numarası.</param>
        /// <returns>TC kimlik numarası verilen kişi kayıtlı kullanıcı ise, adı ve soyadı dizgi olarak yoksa boş dizgi döndürülür..</returns>
        public string KullaniciVarMi(string tcKimlikNo)
        {
            try
            {
                return new KullaniciServis().KullaniciVarMi(tcKimlikNo);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// TC Kimlik numarası verilen kullanıcı bulunur ve Parola ata yöntemi çağrılarak, kullanıcıya yeni üretilmiş parola atınır.
        /// </summary>
        /// <param name="tcKimlikNo">Yeni üretilmiş parola atanacak kullanıcının TC kimlik numarası.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KullaniciyaYeniParolaAta(string tcKimlikNo)
        {
            try
            {
                return new KullaniciServis().KullaniciyaYeniParolaAta(tcKimlikNo);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// TC Kimlik numarası verilen kullanıcının bilgileri mernis bağlantısı ile alan ve kullanıcı veri aktarım nesnesi ile döndüren yordam.
        /// </summary>
        /// <param name="tcKimlikNo">Mernis bağlantısı ile bilgileri bulunacak kişinin TC kimlik numarası.</param>
        /// <returns>TC kimlik numarası geçerli ise  aktarım nesnesinin kullanıcı alanına kullanıcı bilgileri doldurulur ve sonuc yapısının islemSonuc alanına Doğru atanır. Aksi takdirde sonuc yapısının islemSonuc alanına Yanlış atanır.</returns>
        public KullaniciVAN MernisBilgiOku(Kullanici kullanan, string tcKimlikNo, TNSDateTime dogumTarih)
        {
            try
            {
                return new KullaniciServis().MernisBilgiOku(kullanan, tcKimlikNo, dogumTarih);
            }
            catch (Exception e)
            {
                KullaniciVAN donusDeger = new KullaniciVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public bool MerniseBagliMi()
        {
            try
            {
                return KullaniciServis.MerniseBagliMi();
            }
            catch
            {
                return false;
            }
        }

        public Sonuc TCKimlikNoKontrolu(Kullanici kullanan, string tcKimlikNo, TNSDateTime dogumTarih)
        {
            try
            {
                Sonuc s = new Sonuc();
                new KullaniciServis().TCKimlikNoKontrolu(kullanan, tcKimlikNo, dogumTarih, s);
                return s;
            }
            catch (Exception e)
            {
                return new Sonuc(e.Message);
            }
        }

        /// <summary>
        /// Mernis sistemine kayıtlı olan ilçe bilgilerini alma işlemini yapar
        /// </summary>
        //public void MernisIlIlceOku()
        //{
        //    //KullaniciServis servis = new KullaniciServis();
        //    //servis.MernisIlIlceOku();
        //}

        /// <summary>
        /// Kullanıcılar her sayfa talep ettiğinde, veri tabanına işlemle ilgili bilgileri yazan yordam. Bu sayede sistem kullanımıyla ilgili grafikler alınabilir.
        /// </summary>
        /// <param name="kullaniciKodu">Sayfayı çağıran kullanıcının sisteme giriş kodu.</param>
        /// <param name="tarih">Sayfanın çağrıldığı tarih.</param>
        /// <param name="hostName">Talebin yapıldığı yerin ip adresi.</param>
        /// <param name="sayfaAd">İşlem yapılan sayfanın adresi.</param>
        /// <param name="bilgi1">İşlemi yapan kullanıcının adı ve soyadı.</param>
        /// <param name="bilgi2">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi3">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi4">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi5">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KullaniciSessionAta(string kullaniciKodu, TNSDateTime tarih, string hostName, string sayfaAd, string bilgi1, string bilgi2, string bilgi3, string bilgi4, string bilgi5)
        {
            try
            {
                return new KullaniciServis().SessionAta(kullaniciKodu, tarih, hostName, sayfaAd, bilgi1, bilgi2, bilgi3, bilgi4, bilgi5);
            }
            catch (Exception e)
            {
                Sonuc sonuc = new Sonuc();
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
                return sonuc;
            }
        }

        /// <summary>
        /// Verilen kıstaslara göre sistemde yapılan son işlemleri listeleyen yordam. 
        /// </summary>
        /// <param name="kullaniciKodu">Verilen kullanıcının işlemleri listelenir.</param>
        /// <param name="sure">Şu andaki zamandan geriye doğru kaç dakikalık işlemlerin listeleneceği bilgisi.</param>
        /// <param name="hostName">Listelenecek kayıtların ip adresi kıstası.</param>
        /// <param name="sayfaAd">Listelenecek kayıtların sayfa adı kıstası.</param>
        /// <param name="bilgi1">Listelenecek kayıtların ad soyad kıstası.</param>
        /// <param name="bilgi2">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi3">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi4">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <param name="bilgi5">Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.</param>
        /// <returns>Verilen kıstaslara uyan site erişim bilgilerinin listesi döndürülür.</returns>
        public ObjectArray<TNS.KYM.KullaniciIslemYapan> KullaniciSessionListele(string kullaniciKodu, int sure, string hostName, string sayfaAd, string bilgi1, string bilgi2, string bilgi3, string bilgi4, string bilgi5)
        {
            try
            {
                return new KullaniciServis().SessionListele(kullaniciKodu, sure, hostName, sayfaAd, bilgi1, bilgi2, bilgi3, bilgi4, bilgi5);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        /// <summary>
        /// Sisteme erişim bilgilerinin verilen zaman aralığındaki sayılarını, verilen tarih aralığında listeleyen yordam. 
        /// </summary>
        /// <param name="kullaniciKodu">Verile bir kullanıcıya ait veriler listelenir.</param>
        /// <param name="grafikZamanAraligi">Erişim sayılarının hangi sıklıkta raporlanacağı</param>
        /// <param name="tarih1">Erişim sayılarının raporlanacağı zaman diliminin başlangıcı.</param>
        /// <param name="tarih2">Erişim sayılarının raporlanacağı zaman diliminin bitişi.</param>
        /// <param name="sayfaAd">Raporlanacak sayfaların içerdiği dizgi.</param>
        /// <returns>Verilen zaman aralığındaki her bir zaman için oluşan erişim sayısı listesi KullaniciIslemYapan sınıfının islem tarih ve adet alanlarında döndürülür.</returns>
        public ObjectArray<TNS.KYM.KullaniciIslemYapan> KullaniciSessionGrafik(string kullaniciKodu, ENUMKullanimBilgiZaman grafikZamanAraligi, TNSDateTime tarih1, TNSDateTime tarih2, string sayfaAd)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new KullaniciServis().SessionGrafik(kullaniciKodu, grafikZamanAraligi, tarih1, tarih2, sayfaAd);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Bir kullanıcının, bir değişkeninin değerlerini döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Değişken değerleri bulunacak kullanıcının sisteme giriş kodu.</param>
        /// <param name="degisken">Kullanıcının değerleri bulunacak değişkeninin ismi.</param>
        /// <returns>Kullanıcı değişkeninin değerleri liste olarak döndürülür.</returns>
        public TNSCollection<string> DegiskenleriListele(string kullaniciKodu, string degisken)
        {
            return new KullaniciServis().DegiskenleriListele(null, kullaniciKodu, degisken);
        }

        /// <summary>
        /// Kullanıcı değişkenininin değerleri, verilen değerler yapan yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Değişken değerleri saklanacak kullanıcının kodu.</param>
        /// <param name="degisken">Değerleri saklanacak değişkenin ismi.</param>
        /// <param name="degerler">Saklanacak değişkenin değerleri.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc DegiskenKaydet(string kullaniciKodu, string degisken, TNSCollection degerler)
        {
            try
            {
                return new KullaniciServis().DegiskenKaydet(null, kullaniciKodu, degisken, degerler);
            }
            catch (Exception e)
            {
                Sonuc sonuc = new Sonuc();
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
                return sonuc;
            }
        }

        /// <summary>
        /// İçişleri bakanlığı ile entegrasyon için eklenen yordam. Giriş ekranına iç işleri uygulaması tarafından verilen session numarası kullanılarak içişleri bakanlığı web servisi çağıran ve servisten gelen kullanıcı bilgilerini döndürüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="kullaniciKodu">Bilgileri döndürülecek kullanıcının sisteme giriş kodu.</param>
        /// <param name="sessionID">Giriş ekranına iç işleri uygulaması tarafından verilen oturum (session) numarası</param>
        /// <param name="giris">Doğru ise yordam sisteme giriş yapılırken çağrılıyordur. Yanlış ise kullanıcı bilgilerini görüntülemek için çağrılıyordur.</param>
        /// <returns>Bilgileri verilen kullanıcı okunmuşsa aktarım nesnesinin kullanıcı alanında döndürülür. Okunamamışsa aktarım nesnesinin sonuc alanında okuyamama nedeni döndürülür.</returns>
        public KullaniciVAN KullaniciOkuWebServisten(Kullanici kullanan, string kullaniciKodu, string sessionID, bool giris)
        {
            try
            {
                return new KullaniciServis().OkuWebServisten(kullanan, kullaniciKodu, sessionID, giris);
            }
            catch (Exception e)
            {
                KullaniciVAN donusDeger = new KullaniciVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                return donusDeger;
            }
        }

        /// <summary>
        /// İçişleri bakanlığı entegrasyonu ile sisteme giriş yapılmış ise, giriş yapan kullanıcının son giriş tarihini güncellemek için çağrılan yordam.
        /// Ayrıca kullanıcı pasif ise, hata döndürülür ve giriş yapılmasına izin verilmez.
        /// </summary>
        /// <param name="kullanici">Son giriş tarihi güncellenecek ve aktifliği kontrol edilecek kullanıcı parametresi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc GirisYapSession(Kullanici kullanici)
        {
            try
            {
                return new KullaniciServis().GirisYapSession(null, kullanici);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Kullanıcı giriş yaptığında, giriş sayfasında gösterilecek olan dururuların listesini verilen kıstaslara göre döndüren yordam.
        /// </summary>
        /// <param name="aramaBilgisi">Döndürülecek duyuruların uyacağı kriter bilgisini tutan duyuru nesnesi.</param>
        /// <returns>Verilen kriterlere uyan duyurular döndürülür.</returns>
        public ObjectArray<TNS.KYM.Duyuru> DUYListesi(Kullanici kullanan, Duyuru aramaBilgisi, bool modulDuyurulari)
        {
            try
            {
                return new DuyuruServis().Listele(null, kullanan, aramaBilgisi, modulDuyurulari);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Kodu verilen duyurunun bilgilerini döndüren yordam.
        /// </summary>
        /// <param name="duyuruKodu">Döndürülecek duyurunun kodu.</param>
        /// <returns>Kodu verilen duyuru döndürülür.</returns>
        public Duyuru DUYOku(int duyuruKodu)
        {
            return new DuyuruServis().Oku(null, duyuruKodu);
        }

        /// <summary>
        /// Bilgileri verilen duyuru nesnesini veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="duyuru">Duyuru bilgilerini tutan ve kaydedilecek nesne.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc DUYKaydet(Kullanici kullanan, Duyuru duyuru)
        {
            try
            {
                return new DuyuruServis().Kaydet(null, kullanan, duyuru);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Kodu verilen duyuru bilgilerini veri tabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="duyuruKodu">Silinecek duyurunun kodu.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc DUYSil(Kullanici kullanan, int duyuruKodu)
        {
            try
            {
                return new DuyuruServis().Sil(null, kullanan, duyuruKodu);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ListeVAN MuhasebeListesi()
        {
            try
            {
                return new MuhasebeServis().Listele(null);
            }
            catch (Exception e)
            {
                ListeVAN donusDeger = new ListeVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public MuhasebeVAN MuhasebeOku(string kod)
        {
            try
            {
                return new MuhasebeServis().Oku(null, kod);
            }
            catch (Exception e)
            {
                MuhasebeVAN donusDeger = new MuhasebeVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public MuhasebeVAN MuhasebeOkuAd(string ad)
        {
            try
            {
                return new MuhasebeServis().OkuAd(null, ad);
            }
            catch (Exception e)
            {
                MuhasebeVAN donusDeger = new MuhasebeVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc MuhasebeKaydet(Kullanici kullanan, Muhasebe muhasebe)
        {
            try
            {
                return new MuhasebeServis().Kaydet(null, kullanan, muhasebe);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc MuhasebeSil(Kullanici kullanan, string kod)
        {
            try
            {
                return new MuhasebeServis().Sil(null, kullanan, kod);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }


        /// <summary>
        /// Bir kullanıcı tipindeki kullanıcılara, email atmak için kullanılan yordam.
        /// Bu yordamı sadece sistem yöneticileri kullanabilir. Gönderilecek mesaj ve mesajın gönderileceği kişiler veri tabanına yazılır.
        /// Gönderme işlemi için bir thread başlatılır. Bu thread Her kullanıcıya emaili göndermeye çalışır. 
        /// Gönderme işlemi sonucu email için adı kaydedilmiş olan kullanıcının karşısına yazılır.
        /// Bu sayede mesajın gönderilemediği kullanıcılara (thread'in kesilmesi, eposta sunucusunun kabul etmemesi gibi sebeplerle) tekrar mesaj gönderilebilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="konu">Gönderilecek elektronik postanın konusu.</param>
        /// <param name="mesaj">Gönderilecek elektronik postanın metni.</param>
        /// <param name="ktKod">Elektronik postanın hangi kullanıcı tipindeki kullanıcılar gönderileceği</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc EPostaKaydet(Kullanici kullanan, string konu, string mesaj, int ktKod)
        {
            try
            {
                return new DuyuruServis().EPostaKaydet(kullanan, konu, mesaj, ktKod);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Sisteme kaydedilmiş mesajları ve bu mesajların kaç kişiye gönderileceğini ve şu ana kadar kaç kişiye gönderildiğini listeleyen yordam.
        /// </summary>
        /// <returns>Sisteme kaydedilmiş mesajlar ve bu mesajların kaç kişiye gönderileceği ve şu ana kadar kaç kişiye gönderildiği bilgisi Posta nesnesi listesi olarak döndürülür.</returns>
        public ObjectArray<TNS.KYM.Posta> EPostaGonderOncekiGonderiler()
        {
            try
            {
                return new DuyuruServis().EPostaGonderOncekiGonderiler();
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        /// <summary>
        /// "gonderildiMi" parametresinin değerine göre bir mesajla ilgili tüm kullanıcıları, gönderilenleri veya gönderilmeyenleri listeleyen yordam. 
        /// </summary>
        /// <param name="mesajId">Kullanıcıları döndürülecek elektronik postanın numarası.</param>
        /// <param name="gonderildiMi">-1 ise tüm kullanıcılar, 1 ise gönderilenler, 0 ise gönderilmeyenler listenir.</param>
        /// <returns>Bir mesajın gönderildiği/gönderilmediği/ilişkili tüm kullanıcıların bilgisi Posta nesnesi listesi olarak döndürülür.</returns>
        public ObjectArray<TNS.KYM.Posta> EPostaGonderilenlerListele(int mesajId, int gonderildiMi)
        {
            try
            {
                return new DuyuruServis().EPostaGonderilenlerListele(mesajId, gonderildiMi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        /// <summary>
        /// Bir elektronik postayı kullanıcılara göndermek için başlatılmış thread, tüm kullanıcılara mail atamadan sonlanmışsa, 
        /// verilen elektronik posta numarası için thread'i tekrar başlatan yordam. Daha önceki çalışmada posta atılamayan kullanıcılara posta atılır.
        /// </summary>
        /// <param name="mesajId">Posta atma işlemi tekrar başlatılacak yordam.</param>
        public void EPostaGonderTekrar(int mesajId)
        {
            try
            {
                new DuyuruServis().EPostaGonderTekrar(mesajId);
            }
            catch { }
        }

        /// <summary>
        /// Istemci tarafından gönderilmek istenilen bir e-posta gönderilmesini sağlar. Istemci web.config dosyasında SMTP ayarları olmadığı için bu metod kullanılabilir.
        /// </summary>
        /// <param name="kimden">The kimden.</param>
        /// <param name="kime">The kime.</param>
        /// <param name="konu">The konu.</param>
        /// <param name="icerik">The icerik.</param>
        /// <param name="html">if set to <c>true</c> [HTML].</param>
        /// <param name="reply">if set to <c>true</c> [reply].</param>
        /// <param name="ekDosyaIsimleri">The ek dosya isimleri.</param>
        /// <returns></returns>
        public string EPostaGonder(string kimden, string kime, string konu, string icerik, bool html, bool reply, TNS.TNSCollection ekDosyaIsimleri)
        {
            try
            {
                return DuyuruServis.EPostaGonder(kimden, kime, konu, icerik, html, reply, ekDosyaIsimleri);
            }
            catch { return "Hata"; }
        }

        /// <summary>
        /// Konfigürasyon dosyasında tanımlanmış olan "HataDosyasi" klasöründeki dosya bilgilerini ';' karakteri ile birleştirip döndüren yordam.
        /// Dosya bilgisi ise '|' karekteri ile ayrılmış değiştirilme zamanı, dosya büyüklüğü ve dosya adı bilgilerinden oluşmaktadır.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <returns>"HataDosyasi" klasöründeki dosya bilgileri ';' ayracı ile döndürülür.</returns>
        public ObjectArray<TNS.KYM.EkliDosyalar> HataDosyaListele(Kullanici kullanan)
        {
            try
            {
                return new HataDosya().HataDosyaListele(kullanan);
            }
            catch
            {
                return new ObjectArray();
            }
        }

        /// <summary>
        /// Konfigürasyon dosyasında tanımlanmış olan "HataDosyasi" klasör yoluna, verilen dosya adı eklenerek tam yolu bulunan dosyayı okuyan ve byte dizisi olarak döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="dosyaAd">İçeriği döndürülecek dosyanın ismi.</param>
        /// <returns>Verilen dosyanın içeriği byte dizisi olarak döndürülür.</returns>
        public byte[] HataDosyaVer(Kullanici kullanan, string dosyaAd)
        {
            try
            {
                return new HataDosya().HataDosyaVer(kullanan, dosyaAd);
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Konfigürasyon dosyasında tanımlanmış olan "HataDosyasi" klasör yoluna, verilen dosya adı eklenerek tam yolu bulunan dosyayı silen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="dosyaAd">Silinecek dosyanın ismi.</param>
        public void HataDosyaSil(Kullanici kullanan, string dosyaAd)
        {
            try
            {
                new HataDosya().HataDosyaSil(kullanan, dosyaAd);
            }
            catch { }
        }
        /// <summary>
        /// Filtre nesnesi ile geçirilen bilgileri veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="filtre">Kaydedilecek filtre nesnesi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc FiltreKaydet(TNS.KYM.Filtre filtre)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(filtre.kullanan);
                return FiltreServis.Kaydet(null, filtre);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        /// <summary>
        /// Verilen filtre bilgilesi veri tabanından silinir.
        /// </summary>
        /// <param name="filtre">Veri tabanından silinecek filtre nesnesi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc FiltreSil(TNS.KYM.Filtre filtre)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(filtre.kullanan);
                return FiltreServis.Sil(null, filtre);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        /// <summary>
        /// Verilen kıstasa uyan filtrelerin listesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="grupAd">Listelenecek filtrelerin kıstası</param>
        /// <param name="siraNo">0 ise grupdaki tüm kayıtlı filtreler, 0'dan farklı ise verilen sıradaki filtre kaydı okunur.</param>
        /// <returns>Verilen filtre kıstasına uyan filtrelerin listesi</returns>
        public TNS.KYM.Filtre[] FiltreListele(Kullanici kullanan, string grupAd, int siraNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return FiltreServis.Listele(null, kullanan, grupAd, siraNo);
        }

        /// <summary>
        /// Rapors the listele.
        /// </summary>
        /// <returns></returns>
        public System.Collections.Hashtable RaporListele()
        {
            return RaporServis.Listele();
        }

        /// <summary>
        /// Veri tabanı güncelleme işleminde veri tabanı bağlantı bilgileri verir.
        /// </summary>
        /// <param name="kullanan">Kullananıcı bilgisi.</param>
        /// <param name="veriErisimAd">veri tabanı erişim bilgisi.</param>
        /// <returns></returns>
        public string VeriErisimDegeriVer(Kullanici kullanan, string veriErisimAd)
        {
            return VeriErisim.VeriErisimDegerleri(veriErisimAd);
        }


        public Mesaj[] MesajListele(Mesaj mesaj)
        {
            return new MesajServis().Listele(mesaj);
        }
        public Sonuc MesajKaydet(Mesaj mesaj)
        {
            return new MesajServis().Kaydet(mesaj);
        }

        public Mevzuat[] MevzuatListele(Mevzuat mevzuat, TNSDateTime islemTarih)
        {
            return new MevzuatServis().Listele(mevzuat, islemTarih);
        }
        public Sonuc MevzuatKaydet(Mevzuat[] mevzuat)
        {
            return new MevzuatServis().Kaydet(mevzuat);
        }
        public Sonuc MevzuatSil(Mevzuat[] mevzuat)
        {
            return new MevzuatServis().Sil(mevzuat);
        }
        public Sonuc MevzuatKilitIslem(Mevzuat mevzuat, TNSDateTime islemTarih, bool kilit)
        {
            return new MevzuatServis().KilitIslem(mevzuat, islemTarih, kilit);
        }

        public MevzuatDurum MevzuatListeleSonDurum(string mevzuatKod)
        {
            return new MevzuatServis().ListeleMevzuatSonDurum(mevzuatKod);
        }

        public MevzuatSayfa MevzuatListeleSayfa(string msKod)
        {
            return new MevzuatServis().ListeleMevzuatSayfa(msKod);
        }
        public Sonuc MevzuatKaydetSayfa(MevzuatSayfa[] ms)
        {
            return new MevzuatServis().KaydetMevzuatSayfa(ms);
        }
        public Sonuc MevzuatSilSayfa(MevzuatSayfa ms)
        {
            return new MevzuatServis().SilMevzuatSayfa(ms);
        }

        /// <summary>
        /// AnketSoru nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AnketSoru yapısında AnketSoru: bilgilerini tutan değişken</param>
        public AnketSoru[] AnketSoruListele(Kullanici kullanan, AnketSoru form)
        {
            return AnketServis.Listele(null, kullanan, form);
        }


        /// <summary>
        /// AnketSoru nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AnketSoru yapısında AnketSoru: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc AnketSoruKaydet(Kullanici kullanan, AnketSoru form)
        {
            try
            {
                return AnketServis.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public string[] AnketSoruAnahtarKelimeler(int anahtarNo, string kistas)
        {
            return AnketServis.AnahtarKelimeler(null, anahtarNo, kistas);
        }

        /// <summary>
        /// Anket sorusunun, anketlerde kullanılıp kullanılamayacağı durumuna değer atayan yordam
        /// </summary>
        /// <param name="kullanan">Yordamı çağıran kullanıcı bilgilerini tutan nesne</param>
        /// <param name="soru">Soru numarasını ve durumunu tutan nesne</param>
        /// <returns>İşlem sonucu döndürülür.</returns>
        public Sonuc AnketSoruDurumDegistir(Kullanici kullanan, AnketSoru soru)
        {
            return AnketServis.SoruDurumDegistir(null, kullanan, soru);
        }


        public Sonuc AnketBilgiKaydet(Kullanici kullanan, AnketBilgi anketBilgi)
        {
            return AnketServis.AnketBilgiKaydet(null, kullanan, anketBilgi);
        }


        public AnketBilgi AnketBilgiOku(Kullanici kullanan, AnketBilgi anketKistasBilgi, bool seceneklerDahil, bool cevaplarDahil)
        {
            return AnketServis.AnketBilgiOku(null, kullanan, anketKistasBilgi, seceneklerDahil, cevaplarDahil);
        }

        public AnketBilgi[] AnketBilgiListele(Kullanici kullanan, AnketBilgi anketKistasBilgi)
        {
            return AnketServis.AnketBilgiListele(null, kullanan, anketKistasBilgi);
        }

        public Sonuc AnketBilgisineSoruKaydet(Kullanici kullanan, AnketBilgi anketBilgi)
        {
            return AnketServis.AnketBilgisineSoruKaydet(null, kullanan, anketBilgi);
        }

        public Sonuc AnketCevaplariKaydet(Kullanici kullanan, AnketBilgi anketBilgi)
        {
            return AnketServis.AnketCevaplariKaydet((VeriErisim)null, kullanan, anketBilgi);
        }

        public Sonuc AnketCevaplariSifirla(Kullanici kullanan, int hibeKod)
        {
            return AnketServis.AnketCevaplariSifirla((VeriErisim)null, kullanan, hibeKod);
        }


        public Sonuc AnketDavetiGonder(Kullanici kullanan, string modul, string anketAnahtar, int orgBirimKod, string cevapAnahtar, string mailAdres)
        {
            return AnketServis.AnketDavetiGonder((VeriErisim)null, kullanan, modul, anketAnahtar, orgBirimKod, cevapAnahtar, mailAdres);
        }


        public Sonuc AnketDavetiGecerliMi(string modul, string anketAnahtar, int orgBirimKod, string cevapAnahtar, string mailAdres, string onayKod)
        {
            return AnketServis.AnketDavetiGecerliMi(modul, anketAnahtar, orgBirimKod, cevapAnahtar, mailAdres, onayKod);
        }


        public AnketBilgi AnketIstatistikBul(Kullanici kullanan, AnketBilgi anketKistasBilgi)
        {
            return AnketServis.AnketIstatistikBul((VeriErisim)null, kullanan, anketKistasBilgi);
        }

        public AnketCaprazBilgi AnketCaprazIstatistikBul(Kullanici kullanan, AnketBilgi anketKistasBilgi)
        {
            return AnketServis.AnketCaprazIstatistikBul((VeriErisim)null, kullanan, anketKistasBilgi);
        }

        //***************************************************************
        //Organizasyon Şema Fonksiyonları
        //***************************************************************
        /// <summary>
        /// Görevlendirme bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="p">Görev bilgisi, Görev tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc GorevKaydet(Kullanici islemiYapan, Gorev p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ORGServis().GorevKaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Görev bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="p">Görev bilgisi, Görev tipinde sınıf</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        public ObjectArray<TNS.ORG.Gorev> GorevListele(Gorev p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new ORGServis().GorevListele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Görev bilgilerini sorgulayan fonksiyon
        /// </summary>
        /// <param name="p">Görev bilgisi, Görev tipinde sınıf</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        public ObjectArray<TNS.ORG.Gorev> GorevSorgula(Gorev p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new ORGServis().GorevSorgula(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Görev bilgilerini silen fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="p">Görev bilgisi, Görev tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc GorevSil(Kullanici islemiYapan, Gorev p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ORGServis().GorevSil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Görev Evrak bilgisini kaydeden fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>
        /// <param name="evrEvrakNo">Evrak numarası, String tipinde sınıf</param>
        /// <param name="prEvrakNo">Program evrak numarası, String tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc GorevEvrakBilgiKaydet(Kullanici islemiYapan, int gorevKod, string evrEvrakNo, string prEvrakNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ORGServis().GorevEvrakBilgiKaydet(islemiYapan, gorevKod, evrEvrakNo, prEvrakNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Görev Yer Tarih bilgisini kaydeden fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="liste">Liste bilgisi, Object Array tipinde sınıf</param>
        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc GorevYerTarihKaydet(Kullanici islemiYapan, ObjectArray liste, int gorevKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ORGServis().GorevYerTarihKaydet(islemiYapan, liste, gorevKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Görev Yer Tarih bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="p">Görev Yer Tarih bilgisi, Görev Yer Tarih tipinde sınıf</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        public ObjectArray GorevYerTarihListele(GorevYerTarih p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new ORGServis().GorevYerTarihListele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Görev Yer Tarih bilgilerini silen fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc GorevYerTarihSil(Kullanici islemiYapan, int gorevKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ORGServis().GorevYerTarihSil(islemiYapan, gorevKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Görev Tertip bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="liste">Liste bilgisi, Object Array tipinde sınıf</param>
        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc GorevTertipKaydet(Kullanici islemiYapan, ObjectArray liste, int gorevKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ORGServis().GorevTertipKaydet(islemiYapan, liste, gorevKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Görev Tertip bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="p">Görev Tertip bilgisi, Görev Tertip tipinde sınıf</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        public ObjectArray<TNS.ORG.GorevTertip> GorevTertipListele(GorevTertip p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new ORGServis().GorevTertipListele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Görev Tertip bilgilerini silen fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc GorevTertipSil(Kullanici islemiYapan, int gorevKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ORGServis().GorevTertipSil(islemiYapan, gorevKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Görev Personel bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="liste">Liste bilgisi, Object Array tipinde sınıf</param>
        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc GorevPersonelKaydet(Kullanici islemiYapan, ObjectArray liste, int gorevKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ORGServis().GorevPersonelKaydet(islemiYapan, liste, gorevKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Görev Personel bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="p">Görev Personel bilgisi, Görev Personel tipinde sınıf</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        public ObjectArray GorevPersonelListele(GorevPersonel p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new ORGServis().GorevPersonelListele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Görev Personel bilgilerini silen fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc GorevPersonelSil(Kullanici islemiYapan, int gorevKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ORGServis().GorevPersonelSil(islemiYapan, gorevKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        //*********************************************************************************************
        //Organizasyon Yapı
        //*********************************************************************************************
        /// <summary>
        /// Ana nesne'ye göre Organizasyonel Yapıda bağlantılı nesneleri listeler
        /// </summary>
        /// <param name="kullanici"></param>
        /// <param name="obler"></param>
        /// <returns>
        /// Object Array tipinde sınıf döndürür
        /// </returns>
        public Sonuc OrganizasyonBirimKaydet(Kullanici kullanici, TNSCollection obler)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ORGServis().OrganizasyonBirimKaydet(kullanici, obler);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Organizasyon birim Bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="p">Organizasyon birim bilgisi</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc OrganizasyonBirimKaydet(Kullanici islemiYapan, OrganizasyonBirim p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ORGServis().OrganizasyonBirimKaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray<TNS.ORG.OrganizasyonBirim> OrganizasyonBirimListeleKullanici(Kullanici kullanici, bool detayVer, bool adEkle)
        {
            try
            {
                return new ORGServis().OrganizasyonBirimListeleKullanici(kullanici, detayVer, adEkle);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Organizasyon birim bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="p">Organizasyon birim bilgisi</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        public ObjectArray<TNS.ORG.OrganizasyonBirim> OrganizasyonBirimListele(OrganizasyonBirim p, bool detayVer, bool adEkle)
        {
            try
            {
                return new ORGServis().OrganizasyonBirimListele(p, detayVer, adEkle);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Organizasyons the birim listele.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="detayVer">if set to <c>true</c> [detay ver].</param>
        /// <param name="ust">if set to <c>true</c> [ust].</param>
        /// <returns></returns>
        public ObjectArray<TNS.ORG.OrganizasyonBirim> OrganizasyonBirimListele(OrganizasyonBirim p, bool detayVer, int ustVeAyniSeviye, bool adEkle)
        {
            try
            {
                return new ORGServis().OrganizasyonBirimListele(p, detayVer, ustVeAyniSeviye, adEkle);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray<TNS.ORG.OrganizasyonBirim> Il_IlceBul(Kullanici kullanan, string plakaKod, int ilceMernisKod, bool detayVer, bool adEkle)
        {
            try
            {
                return new ORGServis().Il_IlceBul(kullanan, plakaKod, ilceMernisKod, detayVer, adEkle);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Organizasyon birim bilgilerini silen fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="p">Organizasyon birim bilgisi</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc OrganizasyonBirimSil(Kullanici islemiYapan, OrganizasyonBirim p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ORGServis().OrganizasyonBirimSil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Organizasyon birim ana birim degistirir.
        /// </summary>
        /// <param name="islemYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="kod">Kod.</param>
        /// <param name="anaBirimKod">Ana birim kodu.</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc OrganizasyonBirimAnaBirimVeSiraDegistir(Kullanici islemYapan, OrganizasyonBirim p, int yeniAnaBirimKod, int yeniSiraNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ORGServis().OrganizasyonBirimAnaBirimVeSiraDegistir(islemYapan, p, yeniAnaBirimKod, yeniSiraNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Organizasyons the semaya sablon uygula.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="obAnaKod">The ob ana kod.</param>
        /// <param name="sablonKod">The sablon kod.</param>
        /// <returns></returns>
        public Sonuc OrganizasyonSemayaSablonUygula(Kullanici kullanan, int obAnaKod, int sablonKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ORGServis().OrganizasyonSemayaSablonUygula(kullanan, obAnaKod, sablonKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Grafiks the agac olustur.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="orgBirimKod">The org birim kod.</param>
        /// <returns></returns>
        public GrafikOrgDal GrafikAgacOlustur(Kullanici kullanan, int orgBirimKod)
        {
            try
            {
                return new ORGServis().GrafikAgacOlustur(kullanan, orgBirimKod);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc OrganizasyonBirimAc(Kullanici kullanici, int orgBirimKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ORGServis().OrganizasyonBirimAc(kullanici, orgBirimKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc OrganizasyonBirimKapat(Kullanici kullanici, int orgBirimKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ORGServis().OrganizasyonBirimKapat(kullanici, orgBirimKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Genels the gorev oku.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="orgBirim">The org birim.</param>
        /// <param name="siraNo">The sira no.</param>
        /// <returns></returns>
        public BirimGenelGorev GenelGorevOku(Kullanici kullanan, OrganizasyonBirim orgBirim, int siraNo)
        {
            return new ORGServis().GenelGorevOku(kullanan, orgBirim, siraNo);
        }

        /// <summary>
        /// Genels the gorev listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="orgBirim">The org birim.</param>
        /// <returns></returns>
        public BirimGenelGorev[] GenelGorevListele(Kullanici kullanan, OrganizasyonBirim orgBirim, bool aktifler)
        {
            return new ORGServis().GenelGorevListele(kullanan, orgBirim, aktifler);
        }

        /// <summary>
        /// Yenis the gorev ekle.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="orgBirim">The org birim.</param>
        /// <returns></returns>
        public BirimGenelGorev YeniGorevEkle(Kullanici kullanan, OrganizasyonBirim orgBirim)
        {
            return new ORGServis().YeniGorevEkle(kullanan, orgBirim);
        }

        /// <summary>
        /// Genels the gorev kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="birimGenelGorev">The birim genel gorev.</param>
        /// <returns></returns>
        public Sonuc GenelGorevKaydet(Kullanici kullanan, BirimGenelGorev birimGenelGorev)
        {
            return new ORGServis().GenelGorevKaydet(kullanan, birimGenelGorev);
        }

        /// <summary>
        /// Genels the gorev yetki oku.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="birimGenelGorev">The birim genel gorev.</param>
        /// <param name="yazYetkiSiraNo">The yaz yetki sira no.</param>
        /// <returns></returns>
        public GenelGorevYetki GenelGorevYetkiOku(Kullanici kullanan, BirimGenelGorev birimGenelGorev, int yazYetkiSiraNo)
        {
            return new ORGServis().GenelGorevYetkiOku(kullanan, birimGenelGorev, yazYetkiSiraNo);
        }

        /// <summary>
        /// Genels the gorev yetki kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="genelGorevYetki">The genel gorev yetki.</param>
        /// <returns></returns>
        public Sonuc GenelGorevYetkiKaydet(Kullanici kullanan, GenelGorevYetki genelGorevYetki)
        {
            return new ORGServis().GenelGorevYetkiKaydet(kullanan, genelGorevYetki);
        }

        /// <summary>
        /// Genels the gorev kisileri ekle.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="organizasyonBirim">The organizasyon birim.</param>
        /// <param name="birimGenelGorev">The birim genel gorev.</param>
        /// <param name="personeller">The personeller.</param>
        /// <returns></returns>
        public Sonuc GenelGorevKisileriEkle(Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev, Personel[] personeller)
        {
            return new ORGServis().GenelGorevKisileriEkle(kullanan, organizasyonBirim, birimGenelGorev, personeller);
        }

        /// <summary>
        /// Genels the gorev kisileri listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="organizasyonBirim">The organizasyon birim.</param>
        /// <param name="birimGenelGorev">The birim genel gorev.</param>
        /// <returns></returns>
        public Personel[] GenelGorevKisileriListele(Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev)
        {
            return new ORGServis().GenelGorevKisileriListele(kullanan, organizasyonBirim, birimGenelGorev);
        }

        /// <summary>
        /// Kişi Genel Görevleri Listele
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">KisiGorev nesnesi</param>
        /// <returns></returns>
        public KisiGorev[] KisiGenelGorevleriListele(Kullanici kullanan, KisiGorev bilgi)
        {
            return new ORGServis().KisiGenelGorevleriListele(kullanan, bilgi);
        }

        /// <summary>
        /// Genels the gorev kisileri sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="organizasyonBirim">The organizasyon birim.</param>
        /// <param name="birimGenelGorev">The birim genel gorev.</param>
        /// <param name="personeller">The personeller.</param>
        /// <returns></returns>
        public Sonuc GenelGorevKisileriSil(Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev, Personel[] personeller)
        {
            return new ORGServis().GenelGorevKisileriSil(kullanan, organizasyonBirim, birimGenelGorev, personeller);
        }

        /// <summary>
        /// Kişiye ait görevleri sil
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgiler">Silinecek kişi görevleri</param>
        /// <returns></returns>
        public Sonuc KisiGenelGorevleriSil(Kullanici kullanan, KisiGorev[] bilgiler)
        {
            return new ORGServis().KisiGenelGorevleriSil(kullanan, bilgiler);
        }

        /// <summary>
        /// Personels the gorev yetki bul.
        /// </summary>
        /// <param name="tcKimlikNo">The tc kimlik no.</param>
        /// <returns></returns>
        public Kullanici PersonelGorevYetkiBul(string tcKimlikNo)
        {
            return new ORGServis().PersonelGorevYetkiBul(tcKimlikNo);
        }

        /// <summary>
        /// Genels the gorev yetki sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="orgKod">The org kod.</param>
        /// <param name="gorevSiraNo">The gorev sira no.</param>
        /// <param name="yazSiraNo">The yaz sira no.</param>
        /// <returns></returns>
        public Sonuc GenelGorevYetkiSil(Kullanici kullanan, int orgKod, int gorevSiraNo, int yazSiraNo)
        {
            return new ORGServis().GenelGorevYetkiSil(kullanan, orgKod, gorevSiraNo, yazSiraNo);
        }

        public Sonuc LogoKaydet(Kullanici kullanan, Logo bilgi)
        {
            try
            {
                return LogoIslem.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception ex)
            {
                return new Sonuc(ex.ToString());
            }
        }

        public Sonuc LogoSil(Kullanici kullanan, Logo[] bilgiler)
        {
            try
            {
                return LogoIslem.Sil(null, kullanan, bilgiler);
            }
            catch (Exception ex)
            {
                return new Sonuc(ex.ToString());
            }
        }

        public ObjectArray LogoListele(Kullanici kullanan, Logo bilgi)
        {
            try
            {
                return LogoIslem.Listele(null, kullanan, bilgi);
            }
            catch (Exception ex)
            {
                ObjectArray donusdeger = new ObjectArray();
                donusdeger.sonuc.DegerAta(ex.ToString());
                return donusdeger;
            }
        }

        public Sonuc LogoTabloOlustur(Kullanici kullanan)
        {
            try
            {
                return LogoIslem.TabloOlustur(null, kullanan);
            }
            catch (Exception ex)
            {
                return new Sonuc( ex.ToString());
            }
        }

        public Kullanici[] KullaniciYetkiTipleriOku(string[] kullaniciKod, int yetkiTipKod)
        {
            try
            {
                return KullaniciServis.KullaniciYetkiTipleriOku(null, kullaniciKod, yetkiTipKod);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc BuyukMetinSakla(Kullanici kullanan, string modulKod, string anahtar, string dilKod, string metin)
        {
            return MesajServis.BuyukMetinSakla(null, kullanan, modulKod, anahtar, dilKod, metin);
        }

        public Sonuc BuyukMetinListeSakla(Kullanici kullanan, string modulKod, string dilKod, TNSCollection liste)
        {
            return MesajServis.BuyukMetinListeSakla(kullanan, modulKod, dilKod, liste);
        }

        public string BuyukMetinOku(Kullanici kullanan, string modulKod, string anahtar, string dilKod)
        {
            return MesajServis.BuyukMetinOku(kullanan, modulKod, anahtar, dilKod);
        }

        public ObjectArray<TNS.StrKodStrAd> BuyukMetinListele(Kullanici kullanan, string modulKod, string anahtar, string dilKod)
        {
            return MesajServis.BuyukMetinListele(kullanan, modulKod, anahtar, dilKod, true);
        }


        public CografyaBirimSayisi[] BirimTurSayisiListele(Kullanici kullanan, int il1Ilce2Ulke3, string[] turKodlar, string[] cografiKodlar)
        {
            return new ORGServis().BirimTurSayisiListele(kullanan, il1Ilce2Ulke3, turKodlar, cografiKodlar);
        }

        public KullaniciVAN KullaniciOku(string kullaniciKoduVeyaTCKimlikNo)
        {
            try
            {
                return new KullaniciServis().Oku(null, kullaniciKoduVeyaTCKimlikNo);
            }
            catch (Exception e)
            {
                KullaniciVAN donusDeger = new KullaniciVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Cografya nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Cografya yapısında Cografya:  bilgilerini tutan değişken</param>
        public Cografya[] ListeleCografya(Kullanici kullanan, Cografya form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return KYMCografya.Listele(null, kullanan, form);
        }

        /// <summary>
        /// Cografya nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Cografya yapısında Cografya:  bilgilerini tutan değişken</param>
        public Cografya OkuCografya(Kullanici kullanan, Cografya form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return KYMCografya.Oku(null, kullanan, form);
        }

        /// <summary>
        /// Cografya nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Cografya yapısında Cografya:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetCografya(Kullanici kullanan, Cografya form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KYMCografya.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }


        /// <summary>
        /// YoneticiBildirimFormu nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YoneticiBildirimFormu yapısında YoneticiBildirimFormu: Yonetici Bildirim Formu bilgilerini tutan değişken</param>
        public YoneticiBildirimFormu[] ListYoneticiBildirimFormu(Kullanici kullanan, YoneticiBildirimFormu form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return KYMYoneticiBildirimFormu.List(null, kullanan, form);
        }

        /// <summary>
        /// YoneticiBildirimFormu nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YoneticiBildirimFormu yapısında YoneticiBildirimFormu: Yonetici Bildirim Formu bilgilerini tutan değişken</param>
        public YoneticiBildirimFormu GetYoneticiBildirimFormu(Kullanici kullanan, YoneticiBildirimFormu form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return KYMYoneticiBildirimFormu.Get(null, kullanan, form);
        }

        /// <summary>
        /// YoneticiBildirimFormu nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YoneticiBildirimFormu yapısında YoneticiBildirimFormu: Yonetici Bildirim Formu bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SaveYoneticiBildirimFormu(Kullanici kullanan, YoneticiBildirimFormu[] formlar, ENUMYoneticiBildirimFormuDurum durum)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KYMYoneticiBildirimFormu.Save(null, kullanan, formlar, durum);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc DurumGuncelleYoneticiBildirimFormu(Kullanici kullanan, YoneticiBildirimFormu[] form, ENUMYoneticiBildirimFormuDurum durum, int genelGorevKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KYMYoneticiBildirimFormu.DurumGuncelle(null, kullanan, form, durum, genelGorevKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public bool MuhasebeBirimYetkiVarMi(Kullanici kullanan, string muhasebe, string birim, ENUMOGY yetkiTur)
        {
            return new ORGServis().MuhasebeBirimYetkiVarMi(kullanan, muhasebe, birim, yetkiTur);
        }

        public bool OrganizasyonBirimYetkiVarMi(Kullanici kullanan, OrganizasyonBirim organizasyonBirim, ENUMOGY yetkiTur)
        {
            return new ORGServis().OrganizasyonBirimYetkiVarMi(kullanan, organizasyonBirim, yetkiTur);
        }

        public GorevKisitDeger[] GenelGorevKisiKisitListele(Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev, string tckimlikno)
        {
            return new ORGServis().GenelGorevKisiKisitListele(kullanan, organizasyonBirim, birimGenelGorev, tckimlikno);
        }

        public Sonuc GenelGorevKisiKisitEkle(Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev, string tckimlikno, GorevKisitDeger[] kisitlar)
        {
            return new ORGServis().GenelGorevKisiKisitEkle(kullanan, organizasyonBirim, birimGenelGorev, tckimlikno, kisitlar);
        }

        /// <summary>
        /// KullaniciMesaj nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KullaniciMesaj yapısında KullaniciMesaj:  bilgilerini tutan değişken</param>
        public KullaniciMesaj[] ListeleMesajPanosu(Kullanici kullanan, KullaniciMesaj form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return KYMMesajPanosu.Listele(null, kullanan, form);
        }

        /// <summary>
        /// KullaniciMesaj nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KullaniciMesaj yapısında KullaniciMesaj:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetMesajPanosu(Kullanici kullanan, KullaniciMesaj[] mesajlar)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KYMMesajPanosu.Kaydet(null, kullanan, mesajlar);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc GenelGorevKisiTemizle(Kullanici kullanan, string tcKimlikNo)
        {
            return new ORGServis().GenelGorevKisiTemizle(kullanan, tcKimlikNo);
        }

        public Sonuc KisiBirimiGorebilirMiKopru(Kullanici kullanan, int muhasebeKod, string birimKod)
        {
            return new ORGServis().KisiBirimiGorebilirMiKopru(kullanan, muhasebeKod, birimKod);
        }

        public bool MernisKontroluYapma()
        {
            return !TNS.KYM.IsKurallari.KullaniciServis.MerniseBagliMi();
        }

        public Sonuc KopruKodGuncelle(Kullanici kullanan)
        {
            return new ORGServis().KopruKodGuncelle(kullanan);
        }

        public ObjectArray<TNS.KYM.EvrakAktarimMeta> ListeleEvrakAktarim(Kullanici kullanan, string evrakKodu)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KYMEvrakAktarim.Listele(null, kullanan, evrakKodu);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public Sonuc KaydetEvrakAktarim(Kullanici kullanan, string evrakKodu, TNSCollection degerler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KYMEvrakAktarim.Kaydet(null, kullanan, evrakKodu, degerler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// BirimMesajAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BirimMesajAna yapısında BirimMesajAna: Birim Mesaj Ana bilgilerini tutan değişken</param>
        public ObjectArray<TNS.KYM.BirimMesaj> ListeleBirimMesajlari(Kullanici kullanan, BirimMesaj form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KYMBirimMesajlari.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// BirimMesajAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BirimMesajAna yapısında BirimMesajAna: Birim Mesaj Ana bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetBirimMesaji(Kullanici kullanan, BirimMesaj form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KYMBirimMesajlari.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc DurumDegistirBirimMesaji(Kullanici kullanan, TNSCollection mesajlar)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KYMBirimMesajlari.DurumDegistir(null, kullanan, mesajlar);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public string WebConfigOku(string degiskenAd, string defDeger)
        {
            string deg = System.Configuration.ConfigurationManager.AppSettings.Get(degiskenAd);
            if (string.IsNullOrWhiteSpace(deg)) return defDeger;
            return deg;
        }

        public int BirimMesajiSayisiBul(Kullanici kullanan, BirimMesaj form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KYMBirimMesajlari.MesajSayisiBul(null, kullanan, form);
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// EkliDosyalar nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EkliDosyalar yapısında EkliDosyalar: Sistemde tutulan dosya bilgileri bilgilerini tutan değişken</param>
        public ObjectArray<TNS.KYM.EkliDosyalar> ListeleEkliDosyalar(Kullanici kullanan, EkliDosyalar form, bool birimAdOlsun)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KYMEkliDosyalar.Listele(null, kullanan, form, true, birimAdOlsun);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// EkliDosyalar nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EkliDosyalar yapısında EkliDosyalar: Sistemde tutulan dosya bilgileri bilgilerini tutan değişken</param>
        public ObjectVAN<TNS.KYM.EkliDosyalar> OkuEkliDosyaIcerik(Kullanici kullanan, EkliDosyalar form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return KYMEkliDosyalar.OkuDosyaIcerik(null, kullanan, form);
        }

        /// <summary>
        /// EkliDosyalar nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EkliDosyalar yapısında EkliDosyalar: Sistemde tutulan dosya bilgileri bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetEkliDosya(Kullanici kullanan, EkliDosyalar form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KYMEkliDosyalar.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc SilEkliDosyalar(Kullanici kullanan, EkliDosyalar form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KYMEkliDosyalar.Sil(null, kullanan, form, false);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc OnaylaOnayKaldir(Kullanici kullanan, EkliDosyalar form, bool onayla)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return KYMEkliDosyalar.OnaylaOnayKaldir(null, kullanan, form, onayla);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray<TNS.KYM.YerleskeOda> YerleskeOdaListele(Kullanici kullanan, YerleskeOda yerleskeOda)
        {
            return YerleskeOdaServis.Listele(null, kullanan, yerleskeOda);
        }

        public Sonuc YerleskeOdaKaydet(Kullanici kullanan, YerleskeOda yerleskeOda)
        {
            return YerleskeOdaServis.Kaydet(null, kullanan, yerleskeOda);
        }

        public Sonuc YerleskeOdaUstKodGuncelle(Kullanici kullanan, YerleskeOda yerleskeOda)
        {
            return YerleskeOdaServis.GuncelleUstKod(null, kullanan, yerleskeOda);
        }

        public YerleskeOda YerleskeOdaOku(Kullanici kullanan, int odaKod)
        {
            return YerleskeOdaServis.Oku(null, kullanan, odaKod);
        }

        public Sonuc DBHataKaydet(string kullaniciKodu, string tur, string bilgi, string ipAdresi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            TNS.Veri.VeriErisim ve = TNS.Veri.VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(null));
            try
            {
                Veri.TABKullanici to = new Veri.TABKullanici(ve);

                if (bilgi.Length > 2000) bilgi = bilgi.Remove(2000);
                if (kullaniciKodu.Length > 300) kullaniciKodu = kullaniciKodu.Remove(300);

                bilgi = bilgi.Replace("\r", " ").Replace("\n", " ");

                sonuc = to.DBHataYaz(kullaniciKodu, tur, bilgi, ipAdresi);
            }
            catch (Exception e)
            {
                sonuc = new Sonuc(false, e.ToString(), "");
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public ObjectArray<TNS.KYM.Kullanici> KullaniciIslemTarihceListele(Kullanici kullanan, TNSDateTime tarih1, TNSDateTime tarih2, string islemYapilanTCNo, string islemYapilanAd, string islemiYapanKullaniciAdi, int islem)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new KullaniciServis().IslemTarihceRapor(kullanan, tarih1, tarih2, islemYapilanTCNo, islemYapilanAd, islemiYapanKullaniciAdi, islem);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc IISLogYaz(Kullanici kullanan, int sayac, TNSCollection loglar)
        {
            try
            {
                return new KullaniciServis().IISLogYaz(kullanan, sayac, loglar);
            }
            catch (Exception ex)
            {
                return new Sonuc(ex.Message);
            }
        }

        public string Ping(Kullanici kullanan, int tur)
        {
            try
            {
                return new KullaniciServis().Ping(kullanan, tur);
            }
            catch (Exception ex)
            {
                return "HataOlustu:" + ex.Message;
            }
        }

        public void IzlemeKaydet(string ipAddress, string kullaniciKodu, ENUMIzleme izlemeKod, string modulKod, string sayfaAd, string ekBilgi)
        {
            KullaniciServis.IzlemeKaydet(ipAddress, kullaniciKodu, izlemeKod, modulKod, sayfaAd, ekBilgi);
        }
    }
}

namespace TNS
{
    /// <summary>
    /// Kullanıcı modülünde kullanılan yetki tipi listelerini döndüren yordamları tutan sınıf.
    /// </summary>
    public class KYMListe : YetkiTipiListesi
    {
        /// <summary>
        /// Muhasebe listesi döndüren yordam.
        /// </summary>
        /// <returns>Muhasebe listesi döndürülür.</returns>
        public ListeSatir[] MuhasebeListesi()
        {
            return KullaniciServis.MuhasebeListesi();
        }

        /// <summary>
        /// Görme, Yazma ve Onaylama işlemleri ile ilgili, yetki listesi döndüren yordam.
        /// </summary>
        /// <returns>Görme, Yazma ve Onaylama işlemleri ile ilgili, yetki listesi döndürülür.</returns>
        public static ListeSatir[] YetkiListesiOnayli()
        {
            ListeSatir[] lv = new ListeSatir[5];
            lv[0] = new ListeSatir("1", KullaniciResI.KYMYT001);
            lv[1] = new ListeSatir("2", KullaniciResI.KYMYT002);
            lv[2] = new ListeSatir("3", KullaniciResI.KYMYT003);
            lv[3] = new ListeSatir("4", KullaniciResI.KYMYT004);
            lv[4] = new ListeSatir("5", KullaniciResI.KYMYT008);
            return lv;
        }


        /// <summary>
        /// Görme ve Yazma  işlemleri ile ilgili, yetki listesi döndüren yordam.
        /// </summary>
        /// <returns>Görme ve Yazma işlemleri ile ilgili, yetki listesi döndürülür.</returns>
        public static ListeSatir[] YetkiListesiSilmeVeOnayli() //*ht
        {
            ListeSatir[] lv = new ListeSatir[5];
            lv[0] = new ListeSatir("1", KullaniciResI.KYMYT005);
            lv[1] = new ListeSatir("2", KullaniciResI.KYMYT006);
            lv[2] = new ListeSatir("3", KullaniciResI.KYMYT007);
            lv[3] = new ListeSatir("4", KullaniciResI.KYMYT003); //*görme onaylama
            lv[4] = new ListeSatir("5", KullaniciResI.KYMYT004); //*görme yazma onaylama
            return lv;
        }

        /// <summary>
        /// Görme ve Yazma  işlemleri ile ilgili, yetki listesi döndüren yordam.
        /// </summary>
        /// <returns>Görme ve Yazma işlemleri ile ilgili, yetki listesi döndürülür.</returns>
        public static ListeSatir[] YetkiListesiOnaysiz()
        {
            ListeSatir[] lv = new ListeSatir[3];
            lv[0] = new ListeSatir("1", KullaniciResI.KYMYT005);
            lv[1] = new ListeSatir("2", KullaniciResI.KYMYT006);
            lv[2] = new ListeSatir("3", KullaniciResI.KYMYT007);

            return lv;
        }

        /// <summary>
        /// Görme, Yazma , Onaylama , Yevmiye işlemleri ile ilgili, yetki listesi döndüren yordam.
        /// </summary>
        /// <returns>Görme, Yazma , Onaylama , Yevmiye işlemleri ile ilgili, yetki listesi döndürülür.</returns>
        public static ListeSatir[] YetkiListesiYevmiyeli()
        {
            ListeSatir[] lv = new ListeSatir[6];
            lv[0] = new ListeSatir("1", KullaniciResI.KYMYT001);
            lv[1] = new ListeSatir("2", KullaniciResI.KYMYT002);
            lv[2] = new ListeSatir("3", KullaniciResI.KYMYT003);
            lv[3] = new ListeSatir("4", KullaniciResI.KYMYT004);
            lv[4] = new ListeSatir("5", KullaniciResI.KYMYT008);
            lv[5] = new ListeSatir("6", KullaniciResI.KYMYT009);

            return lv;
        }

        /// <summary>
        /// KDSs the rol listele.
        /// </summary>
        /// <returns></returns>
        public static ListeSatir[] KDSRolListele()
        {
            return KullaniciServis.KDSRolListele();
        }

        /// <summary>
        /// KDSs the birim listele.
        /// </summary>
        /// <returns></returns>
        public static ListeSatir[] KDSBirimListele()
        {
            return KullaniciServis.KDSBirimListele();
        }

        /// <summary>
        /// Gits the rol listele.
        /// </summary>
        /// <returns></returns>
        public static ListeSatir[] GITRolListele()
        {
            return KullaniciServis.GITRolListele();
        }

        /// <summary>
        /// Gits the birim listele.
        /// </summary>
        /// <returns></returns>
        public static ListeSatir[] GITBirimListele()
        {
            return KullaniciServis.GITBirimListele();
        }


        /// <summary>
        /// Kullanıcı sunucusu tarafından döndürülen yetki listelerinin giriş yordamı. Bu yordam 2, 4 ve 5 kodlarını tanır. Kod:
        ///     2 ise Muhasebe listesi
        ///     4 ise Görme, Yazma ve Onaylama işlemleri listesi
        ///     5 ise Görme ve Yazma işlemleri listesi
        ///     6 ise Görme ve Yazma, Yevmiye işlemleri listesi
        /// döndürülür.
        /// </summary>
        /// <param name="kullanan">Listeyi isteyen kullanıcının bilgisi.</param>
        /// <param name="yil">Sonuc listedeki elemanların kıstas yılı.</param>
        /// <param name="ekParamCagri">Sonuc listedeki elemanlar için verilen ek kıstas bilgisi. Bu alanın biçimi ve anlamı yordamdan yordama değişir.</param>
        /// <param name="ekParamTanim">Bir yordam, birden fazla liste döndürebilir. Bu alan döndürülecek listenin bilgisini tutar. Yetki tipi tanımlanırken, statik olarak verilir.</param>
        /// <returns>Verilen kıstaslara uyan yetki listesi döndürülür.</returns>
        public ListeSatir[] YetkiTipiListeElemanlari(Kullanici kullanan, int yil, string ekParamCagri, string ekParamTanim)
        {
            if (ekParamTanim.Equals("2"))
                return MuhasebeListesi();
            else if (ekParamTanim.Equals("4"))
                return YetkiListesiOnayli();
            else if (ekParamTanim.Equals("5"))
                return YetkiListesiOnaysiz();
            else if (ekParamTanim.Equals("6"))
                return YetkiListesiYevmiyeli();
            else if (ekParamTanim.Equals("7"))
                return YetkiListesiSilmeVeOnayli();//*ht
            else if (ekParamTanim.Equals("81"))
                return KDSRolListele();//*ht
            else if (ekParamTanim.Equals("82"))
                return KDSBirimListele();//*ht
            else if (ekParamTanim.Equals("91"))
                return GITRolListele();//*ht
            else if (ekParamTanim.Equals("92"))
                return GITBirimListele();//*ht
            throw new NotImplementedException("Bilinmeyen parametre!");
        }
    }
}