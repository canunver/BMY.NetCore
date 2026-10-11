using System;
using TNS.KYM;
using TNS.BKD;
using TNS.BUM;
using TNS.Veri;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using TNS.UZY;
using TNS.MUH;
using TNS.BHMv2;
using TNS.DEG;


namespace TNS.BUM.IsKurallari
{
    /// <summary>
    /// Sertifika politikası sınıfı
    /// </summary>
    class CertificatePolicy /* .NET 8: ICertificatePolicy yok; sinif kullanilmiyor */
    {
        /// <summary>
        /// X509 sertifikasını doğrulayan ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="servicepoint">Servis noktasını tutan, ServicePoint türünde nesne</param>
        /// <param name="certificate">X509 Sertifikasını tutan, X509Certificate türünde nesne</param>
        /// <param name="request">Web isteğini tutan, WebRequest türünde nesne</param>
        /// <param name="problem">Problem tipini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, True değeri döndürülür</returns>
        public bool CheckValidationResult(ServicePoint servicepoint, X509Certificate certificate, WebRequest request, int problem)
        {
            return true;
        }
    }


    /// <summary>
    /// Bütçe Uygulama Servis Sınıfı
    /// </summary>
    [Serializable()]
    public class BUMServis : MarshalByRefObject, IBUMServis
    {
        public static bool tabloDegisimYap = true;

        static BUMServis()
        {
            lock ("OncekiIslemiBiteneKadarBekle")
            {
                if (tabloDegisimYap)
                {
                    tabloDegisimYap = false;
                    TNS.BUM.IsKurallari.BUMTabloDegisiklikleri.Kaydet();
                }
            }
        }

        /// <summary>
        /// Konfigürasyon dosyasından, servisin url'sini alarak Bütçe Kod Servisini Tanımlayan fonksiyon
        /// </summary>
        /// <returns>Fonksiyonun çalışması sonucunda, Bütçe Kod Servisini tutan, IBKDServis türünde nesne döndürür</returns>
        public static IBKDServis TanimlaBKD()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("BKDServisURL");
            IBKDServis servis = TNS.Api.ServisIstemci.Olustur<IBKDServis>(sUrl);

            return servis;
        }

        /// <summary>
        /// Konfigürasyon dosyasından, servisin url'sini alarak Uzay Servisini Tanımlayan fonksiyon
        /// </summary>
        /// <returns>Fonksiyonun çalışması sonucunda, Uzay servisini tutan, IUZYServis türünde nesne döndürür</returns>
        public static IUZYServis TanimlaUZY()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("UZYServisURL");
            IUZYServis servis = TNS.Api.ServisIstemci.Olustur<IUZYServis>(sUrl);

            return servis;
        }

        /// <summary>
        /// Konfigürasyon dosyasından, servisin url'sini alarak Muhasebe Servisini Tanımlayan fonksiyon
        /// </summary>
        /// <returns>Fonksiyonun çalışması sonucunda, Muhasebe servisini tutan, IMUHServis türünde nesne döndürür</returns>
        public static IMUHServis TanimlaMUH()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("MUHServisURL");
            IMUHServis servis = TNS.Api.ServisIstemci.Olustur<IMUHServis>(sUrl);

            return servis;
        }

        /// <summary>
        /// Konfigürasyon dosyasından, servisin url'sini alarak Bütçe Hazırlık Servisini Tanımlayan fonksiyon
        /// </summary>
        /// <returns>Fonksiyonun çalışması sonucunda, Bütçe Hazırlık servisini tutan, IBHMServis türünde nesne döndürür</returns>
        public static IBHMv2Servis TanimlaBHM()
        {
            return TNS.BHMv2.Arac.Tanimla();
        }

        /// <summary>
        /// Parametre olarak verilen Yıl parametresine göre Bağlantı Satırını döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Bağlantı satırını tutan String türünde nesne döndürür</returns>
        public static string BaglantiSatiriBul(int yil)
        {
            return "BaglantiSatiriBUM";
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama, Tertip bilgilerine göre,
        /// İcmal Harcama Programı bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, İcmal Harcama Programı bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        public ObjectArray IHPListele(TNS.KYM.Kullanici kullanan, int yil, int asama, Tertip tertip)
        {
            ObjectArray donusDeger;
            try
            {
                donusDeger = IcmalHarcamaServisi.IHPListele(null, kullanan, yil, asama, tertip);
            }
            catch (Exception e)
            {
                donusDeger = new ObjectArray();
                donusDeger.sonuc = new Sonuc(false, e.Message, "");
            }
            return donusDeger;

        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama ve Tertip bilgilerini göre,
        /// İcmal Harcama Programının KBÖ ve Serbest bilgilerinin toplamını bularak bilgilerini listeleyen program
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, İcmal Harcama Programı bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        public ObjectArray IHPListeleToplam(TNS.KYM.Kullanici kullanan, int yil, int asama, Tertip tertip)
        {
            ObjectArray donusDeger;
            try
            {
                donusDeger = IcmalHarcamaServisi.IHPListeleToplam(null, kullanan, yil, asama, tertip);
            }
            catch (Exception e)
            {
                donusDeger = new ObjectArray();
                donusDeger.sonuc = new Sonuc(false, e.Message, "");
            }
            return donusDeger;

        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama ve İcmal Satırları bilgilerine göre,
        /// İcmal Harcama Programı bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="icmalSatirlari">İcmal satırları bilgilerini tutan, TNSCollection türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, kaydetme işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc IHPKaydet(TNS.KYM.Kullanici kullanan, int yil, int asama, TNSCollection icmalSatirlari)
        {
            Sonuc donusDeger;

            try
            {
                donusDeger = IcmalHarcamaServisi.IHPKaydet(null, kullanan, yil, asama, icmalSatirlari);
            }
            catch (Exception e)
            {
                donusDeger = new Sonuc(false, e.Message, "");
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama ve Tertip bilgilerine göre,
        /// İcmal Harcama Programı bilgisini okuyan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasTertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, İcmal Harcama Programı bilgisini tutan Object türünde nesne döndürür</returns>
        public Object IHPOku(Kullanici kullanan, int yil, int asama, Tertip kistasTertip)
        {
            try
            {
                return IcmalHarcamaServisi.IHPOku(null, kullanan, yil, asama, kistasTertip);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama ve KurumBirim bilgilerine göre,
        /// İcmal Harcama Programını onaylayan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurumBirim">Kurum Birim bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Onaylama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc IHPOnayla(TNS.KYM.Kullanici kullanan, int yil, int asama, string kurumBirim)
        {
            Sonuc donusDeger;
            try
            {
                donusDeger = IcmalHarcamaServisi.IHPOnayla(null, kullanan, yil, asama, kurumBirim);
            }
            catch (Exception e)
            {
                donusDeger = new Sonuc(false, e.Message, "");
            }

            return donusDeger;

        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama ve KurumBirim bilgilerine göre,
        /// İcmal Harcama Programının Onayını kaldıran fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurumBirim">Kurum Birim bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Onay kaldırma işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc IHPOnayKaldir(Kullanici kullanan, int yil, int asama, string kurumBirim)
        {
            Sonuc donusDeger;
            try
            {
                //                donusDeger = new IcmalHarcamaServisi().IHPOnayKaldir(null, kullanan, yil, asama, tertip);
                donusDeger = new Sonuc(false, "e.Message", "");
            }
            catch (Exception e)
            {
                donusDeger = new Sonuc(false, e.Message, "");
            }

            return donusDeger;

        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum Birim, Aşama ve Tertip bilgilerine göre,
        /// Ayrıntılı Program bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurumBirim">Kurum Birim bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertipBilgileri">Tertip bilgilerini tutan, TNSCollection türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, kaydetme işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc APKaydet(Kullanici kullanan, int yil, string kurumBirim, int asama, TertipOdenek[] tertipBilgileri)
        {
            try
            {
                return AylikHarcamaServisi.APKaydet(null, kullanan, yil, kurumBirim, asama, tertipBilgileri);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama ve Tertip bilgilerine göre,
        /// Ayrıntılı Program bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasTertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Ayrıntılı Program bilgilerin tutan ObjectArray türünde nesne döndürür</returns>
        public TertipOdenek[] APListele(Kullanici kullanan, int asama, Tertip kistasTertip)
        {
            return AylikHarcamaServisi.APListele(null, kullanan, asama, kistasTertip);
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama ve Tertip bilgilerine göre
        /// Ayrıntılı Program bilgisini okuyan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasTertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Ayrıntılı Program bilgisini tutan Object türünde nesne döndürür</returns>
        public TertipOdenek APOku(Kullanici kullanan, int yil, int asama, Tertip kistasTertip)
        {
            return AylikHarcamaServisi.APOku(null, kullanan, yil, asama, kistasTertip);
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama, Tertip ve Ekonomik Seviye bilgilerine göre,
        /// Ayrıntılı Program bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasTertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <param name="ukb">Sonuç kümesinde bulunacak tertibin hangi kodlardan oluşacağı bilgisi.</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Ayrıntılı Program bilgilerin tutan ObjectArray türünde nesne döndürür</returns>
        public TertipOdenek[] APListeleKodBilgi(Kullanici kullanan, int asama, Tertip kistasTertip, UcgenKodBilgileri ukb)
        {
            return AylikHarcamaServisi.APListeleKodBilgi(null, kullanan, asama, kistasTertip, ukb);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Tertip bilgisine göre,
        /// Tertip listesini döndüren fonksiyon.
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <param name="hiyerarsik">Hiyerarşik parametresine göre kırılımlı olarak getirip getirmeyeceği bilgisini tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, tertip bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        public ObjectArray TertipListesiBul(int yil, Tertip tertip, bool hiyerarsik)
        {
            try
            {
                return ButceIslemServisi.TertipListesiBul(null, yil, tertip, hiyerarsik);
            }
            catch (Exception e)
            {
                ObjectArray a = new ObjectArray();
                a.sonuc.islemSonuc = false;
                a.sonuc.hataStr = e.ToString();
                return a;
            }
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, İşlem Türü ve Kodlar bilgisine göre,
        /// Onaylı Bütçe İşlem Formlarında Eklenen ve Düşülen Tertipleri bulan fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="islemTur">İşlem türü bilgisini tutan, String türünde nesne</param>
        /// <param name="kodlar">Kodlar bilgisini tutan, TNSCollection türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Eklenen ve Düşülen Tertip bilgilerini tutan BITertipBilgi türünde nesne döndürür</returns>
        public BITertipBilgi EklenenDusulenBulOnayli(int yil, string islemTur, TNSCollection kodlar)
        {
            BITertipBilgi tBilgi = new BITertipBilgi();
            string[] icKodlar = new string[kodlar.Count];
            for (int i = 0; i < kodlar.Count; i++)
            {
                icKodlar[i] = (string)kodlar[i];
            }
            tBilgi = ButceIslemServisi.EklenenDusulenBul(yil, islemTur, icKodlar, ENUMFormDurumGrup.ONAYLI);
            return tBilgi;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl ve Tertip bilgilerine göre,
        /// Harcamaları listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Harcama bilgisini tutan Object türünde nesne döndürür</returns>
        public Object HarcamaListeleAy(Kullanici kullanan, int yil, Tertip tertip)
        {
            try
            {
                return IcmalHarcamaServisi.HarcamaListeleAy(null, kullanan, yil, tertip);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        /// <summary>
        /// Parametre olarak verilen Üst kod bilgisine göre,
        /// Bütçe İşlemlerini listeleyen fonksiyon
        /// </summary>
        /// <param name="ustKod">Üst kod bilgisini tutan, String türünde nesne</param>
        /// <param name="hiyerarsik">Listelemenin hiyerarşik biçimde yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <param name="anaKodlar">Listelemenin ana kodlara göre yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Bütçe İşlem bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        public ObjectArray ButceIslemListesi(string ustKod, bool hiyerarsik, bool anaKodlar)
        {
            try
            {
                return ButceIslemServisi.ButceIslemListesi(ustKod, hiyerarsik, anaKodlar);
            }
            catch (Exception e)
            {
                ObjectArray lv = new ObjectArray();
                lv.sonuc.islemSonuc = false;
                lv.sonuc.hataStr = e.Message;
                return lv;
            }
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Bütçe İşlem Formu bilgilerine göre,
        /// Bütçe İşlem bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="form">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <param name="sadeceOzet">Listelemenin özet şekilde yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Bütçe İşlem bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        public ObjectArray BIListele(Kullanici kullanan, BIForm form, bool sadeceOzet)
        {
            try
            {
                return ButceIslemServisi.BIListele(null, kullanan, form, sadeceOzet);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                return donusDeger;
            }
        }

        /// <summary>
        /// Paramatre olarak verilen Dayanak bilgilerine göre,
        /// Dayanak bilgisi okuyan fonksiyon
        /// </summary>
        /// <param name="dayanak">Dayanak bilgisini tutan, BIDayanak türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Dayanak bilgisini tutan BIDayanak türünde nesne döndürür</returns>
        public BIDayanak DayanakOku(BIDayanak dayanak)
        {
            return ButceIslemServisi.DayanakOku(null, dayanak);
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı bilgilerine göre,
        /// Kaynak Yılda bulunan Dayanak bilgilerini Hedef Yıla kopyalayan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="kaynakYil">Kaynak yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="hedefYil">Hedef yıl bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kopyalama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc DayanakKopyala(Kullanici kullanan, int kaynakYil, int hedefYil)
        {
            return ButceIslemServisi.DayanakKopyala((VeriErisim)null, kullanan, kaynakYil, hedefYil);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve İşlem Kodu bilgisine göre,
        /// Dayanak bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Dayanak bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        public ObjectArray BIDayanakListesi(int yil, int islemKod)
        {
            try
            {
                return ButceIslemServisi.BIDayanakListesi(null, yil, islemKod);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger;
                donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                return donusDeger;
            }
        }

        /// <summary>
        /// Parametre olarak verilen Yıl bilgisine göre,
        /// Yedek tertipleri listeleyen fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Yedek tertip bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        public ObjectArray YedekTertipListesi(int yil)
        {
            try
            {
                return ButceIslemServisi.YedekTertipListesi(yil);
            }
            catch (Exception e)
            {
                ObjectArray lv = new ObjectArray();
                lv.sonuc.islemSonuc = false;
                lv.sonuc.hataStr = e.Message;
                return lv;
            }
        }

        /// <summary>
        /// Parametre olarak verilen  Kullanıcı ve Dayanak bilgilerine göre,
        /// Dayanak bilgisini kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="form">Dayanak bilgisini tutan, BIDayanak türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, kaydetme işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc DayanakKaydet(Kullanici kullanan, BIDayanak form)
        {
            try
            {
                return ButceIslemServisi.DayanakKaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Tertip bilgilerine göre,
        /// Ayrıntılı Harcama Programının son durumunu getiren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Ayrıntılı Harcama Programının son durumunu tutan ObjectArray türünde nesne döndürür</returns>
        public ObjectArray AHPSonDurumGoster(int yil, Tertip tertip)
        {
            try
            {
                if (tertip.tabloKod == ENUMTabloKod.TABGiderIcmal)
                    return IcmalHarcamaServisi.HPSonDurumGoster(null, yil, tertip);
                else
                    return AylikHarcamaServisi.APSonDurumGoster(null, yil, tertip);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc = new Sonuc(false, e.Message, "");
                return donusDeger;
            }
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu ve Sıra numarası bilgilerine göre,
        /// Kurumların listesini döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra numarası bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kurumlar listesini tutan ObjectArray türünde nesne döndürür</returns>
        public ObjectArray BIKurumlarListesi(int yil, string kurum, string islemKod, int siraNo)
        {
            try
            {
                return ButceIslemServisi.BIKurumlarListesi(null, yil, kurum, islemKod, siraNo);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray(); ;
                donusDeger.sonuc = new Sonuc(false, e.Message, "");
                return donusDeger;
            }
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum, İşlem Kodu ve Sıra Numarası bilgilerine göre,
        /// Bütçe işlem bilgisini okuyan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra numarası bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Bütçe İşlem bilgisini tutan Object türünde nesne döndürür</returns>
        public object BIOku(Kullanici kullanan, int yil, string kurum, string islemKod, int siraNo)
        {
            try
            {
                return ButceIslemServisi.BIOku(null, kullanan, yil, kurum, islemKod, siraNo, true);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        ///// <summary>
        ///// Parametre olarak verilen Kullanıcı, Yıl, Kurum, İşlem Kodu ve Sıra Numarası bilgilerine göre,
        ///// Bütçe işlem bilgisini tertip hiyerarşisine göre kırarak okuyan fonksiyon
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        ///// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        ///// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        ///// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        ///// <param name="siraNo">Sıra numarası bilgisini tutan, Int türünde nesne</param>
        ///// <returns>Fonksiyonun çalışması sonucunda, Bütçe İşlem bilgisini tutan Object türünde nesne döndürür</returns>
        //public object BIOkuUcgen(Kullanici kullanan, int yil, string kurum, string islemKod, int siraNo)
        //{
        //    try
        //    {
        //        return ButceIslemServisi.BIOkuUcgen(null, kullanan, yil, kurum, islemKod, siraNo);
        //    }
        //    catch (Exception e)
        //    {
        //        return new Sonuc(false, e.Message, "");
        //    }
        //}

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Bütçe İşlem Form bilgilerine göre,
        /// Bütçe İşlemini kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="islem">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <param name="aktarim">Aktarım yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kaydetme işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc BIKaydet(Kullanici kullanan, BIForm islem, bool aktarim)
        {
            try
            {
                return ButceIslemServisi.BIKaydet(null, kullanan, islem, aktarim);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message + e.StackTrace, "");
            }
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı ve Bütçe İşlem Form bilgilerine göre,
        /// Bütçe İşlemini Onaylayan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="islem">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <param name="bumkoOnayiMi">BÜMKO'ya göre Onaylama işleminin yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Onaylama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc BIOnayla(Kullanici kullanan, BIForm islem, bool bumkoOnayiMi)
        {
            try
            {
                return ButceIslemServisi.BIOnayla(null, kullanan, islem, bumkoOnayiMi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı ve Bütçe İşlem Form bilgilerine göre,
        /// Bütçe İşlemini İptal eden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="islem">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, İptal işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc BIIptal(Kullanici kullanan, BIForm islem)
        {
            try
            {
                return ButceIslemServisi.BIIptal(null, kullanan, islem);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı ve Bütçe İşlem Form bilgilerine göre,
        /// BUMKO Kodu atayan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="islem">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, BÜMKO Kod atama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc BIBUMKOKodAta(Kullanici kullanan, BIForm islem)
        {
            try
            {
                return ButceIslemServisi.BIBUMKOKodAta(null, kullanan, islem);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl ve Kurum bilgilerini göre,
        /// Bütçeleri hazırlayıp sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="tabloKod">Tablo kodu bilgisini tutan, ENUMTabloKod türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, bütçeleri hazırlama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc ButceleriHazirla(TNS.KYM.Kullanici kullanan, int yil, string kurum, ENUMTabloKod tabloKod)
        {
            try
            {
                return ButceIslemServisi.ButceleriHazirla(null, kullanan, yil, kurum, tabloKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama, Tertip ve Yöntem bilgilerine göre,
        /// Ayrıntılı Program Taslak oluşturan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <param name="yontem">Yöntem bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Taslak bilgisini tutan TertiplerVAN türünde nesne döndürür</returns>
        public TertiplerVAN TaslakOlusturAP(Kullanici kullanan, int yil, int asama, Tertip tertip, int yontem)
        {
            try
            {
                return AylikHarcamaServisi.TaslakOlustur(null, kullanan, yil, asama, tertip, yontem);
            }
            catch (Exception e)
            {
                TertiplerVAN donusDeger = new TertiplerVAN();
                donusDeger.sonuc = new Sonuc(false, e.Message, "");
                return donusDeger;
            }

        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum ve Aşama bilgilerine göre,
        /// Ayrıntılı Programı Onaylayan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, onaylama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc OnaylaAP(Kullanici kullanan, int yil, string kurum, int asama)
        {
            try
            {
                return AylikHarcamaServisi.OnaylaAP(null, kullanan, yil, kurum, asama);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum ve Aşama bilgilerine göre,
        /// Ayrıntılı Programın Onayını kaldıran fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, onay kaldırma işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc OnayKaldirAP(Kullanici kullanan, int yil, string kurum, int asama)
        {
            try
            {
                return AylikHarcamaServisi.OnayKaldirAP(null, kullanan, yil, kurum, asama);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum ve Serbest bilgilerine göre,
        /// Serbestleri hesaplayıp sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="ahpSerbestBilgi">Serbest bilgiyi tutan, TNSCollection türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, hesaplama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc SerbestleriHesapla(Kullanici kullanan, int yil, string kurum, TNSCollection ahpSerbestBilgi)
        {
            try
            {
                return ButceIslemServisi.SerbestleriHesapla(null, kullanan, yil, kurum, ahpSerbestBilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum ve Tertip bilgilerine göre,
        /// Bloke bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="trpler">Tertip bilgilerini tutan, TNSCollection türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, kaydetme işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc BlokeKaydet(int yil, string kurum, TNSCollection trpler)
        {
            try
            {
                return AylikHarcamaServisi.BlokeKaydet(null, yil, kurum, trpler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }


        /// <summary>
        /// Parametre olarak verilen Yıl ve Kurum bilgilerine göre,
        /// Bloke bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Bloke bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        public ObjectArray BlokeListele(int yil, string kurum)
        {
            try
            {
                return AylikHarcamaServisi.BlokeListele(null, yil, kurum);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc = new Sonuc(false, e.Message, "");
                return donusDeger;
            }
        }

        public ObjectArray ListeleGunlukNakitIhtiyac(Kullanici kullanan, TNSDateTime tarih, string birimKod, bool tarihAraligiOlsun)
        {
            try
            {
                return NakitIhtiyacServisi.ListeleGunlukNakitIhtiyac(null, kullanan, tarih, birimKod, tarihAraligiOlsun);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc = new Sonuc(false, e.Message, "");
                return donusDeger;
            }
        }

        public Sonuc KaydetGunlukNakitIhtiyac(Kullanici kullanan, TNSDateTime tarih, string birimKod, GunDegerHarcamaTur[] harcamaTutarlari)
        {
            try
            {
                return NakitIhtiyacServisi.KaydetGunlukNakitIhtiyac(null, kullanan, tarih, birimKod, harcamaTutarlari);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
    }
}
