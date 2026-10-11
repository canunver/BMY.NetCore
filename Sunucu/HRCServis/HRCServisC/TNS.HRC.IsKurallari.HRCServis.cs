using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using TNS.BKD;
using TNS.DEG;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.PER;
using TNS.UZY;
using static TNS.HRC.IsKurallari.SeyyarGorevC;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Harcama Yönetimine ait işlemleri yürüten tüm fonksiyonları yöneten sınıf
    /// </summary>
    [Serializable()]
    public class HRCServis : MarshalByRefObject, IHRCServis
    {
        public static IUZYServis servisUZY = null;
        public static bool tabloDegisimYap = true;

        ///// <summary>
        ///// Kullanıcı yönetimi Servisi
        ///// </summary>
        //public static IKYMServis servisKYM
        //{
        //    get
        //    {
        //        if (_servisKYM == null)
        //        {
        //            _servisKYM = TNS.KYM.Arac.Tanimla();
        //        }
        //        return _servisKYM;
        //    }
        //}

        /// <summary>
        /// Merkez bankasi kur aktarimi yapildi mi?
        /// </summary>
        static bool kurAktarimiYapildi = false;

        //static bool tabloGuncelle = false;
        /// <summary>
        /// <see cref="HRCServis"/> varsayılan nesnesini üretir
        /// </summary>
        public HRCServis()
        {
            lock ("OncekiIslemiBiteneKadarBekle")
            {
                if (tabloDegisimYap)
                {
                    tabloDegisimYap = false;
                    TNS.HRC.IsKurallari.HRCTabloDegisiklikleri.Kaydet();
                }
            }

            //TabloDegisimYap();
            if (!kurAktarimiYapildi)
            {
                //Merkez bankasından kur bilgilerini otomatik çekmek için. AktarımServis çalışmayan kurumlarda otomatik kur çekilmiyor. Ör: Özelleştirme idaresi, EGO
                kurAktarimiYapildi = true;
                bool dovizAktarim = OrtakFonksiyonlar.ConvertToInt(ConfigurationManager.AppSettings.Get("MuhasebeDovizAktar"), 0) > 0 ? true : false;
                if (dovizAktarim)
                    HRCMerkezBankKur.Kaydet(new Kullanici(), new TNSDateTime(DateTime.Now.AddDays(-2)), new TNSDateTime(DateTime.Now));

                //Teminat mektubu kayıtlarında zamanı geçmiş olanların uyarı olarak mail ile bildirilmesi Melih 08.12.2023 Cumhurbaşkanlığı (Cumhur da aktarimservis çalışmıyor)
                Thread th = new Thread(TeminatMektubuUyari);
                th.Start();

                //birçok yerde HRCServis.servisUZY ile referans verilmiş. 
                //Bu nedenle kur aktarımı yapıldıktan sonra bu tanımlama yapılsın. 
                //Zaten diğer servislerde açılmıştır
                if (servisUZY == null)
                    servisUZY = TNS.UZY.Arac.Tanimla();

            }
        }

        //Teminat mektubu kayıtlarında zamanı geçmiş olanların uyarı olarak mail ile bildirilmesi Melih 08.12.2023
        private void TeminatMektubuUyari()
        {
            Kullanici temKullanici = new Kullanici();
            temKullanici.kullaniciKodu = "temKullanici";
            temKullanici.kullaniciTipi = new ENUMKullaniciTipi[1];
            temKullanici.kullaniciTipi[0] = TNS.KYM.ENUMKullaniciTipi.SISTEMYONETICISI;

            HRCTeminatMektubu.UyariGonder(null, temKullanici);
        }

        /// <summary>
        /// Verilen kriterlere bağlı olarak uzay servisi ile bilgi döner
        /// </summary>
        /// <param name="uzayAd">Sorgulama yapılacak uzay adı</param>
        /// <param name="kosul">Sorgulama yapılacak koşul</param>
        /// <param name="esitle">Eğer <c>true</c> olarak verilirse koşul bilgisini "=" olarak listeler eğer <c>false</c> verilirse "%LIKE%" olarak listeler</param>
        /// <param name="uzayAlan">Sorgulama sonucu döndürülecek uzaya ait alan adı</param>
        /// <returns>Verilen kriterlere bağlı olarak uzay servisi ile string yapısında bilgi döner</returns>
        public static string UzayYaziDegerGetir(string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            if (servisUZY == null)
                servisUZY = TNS.UZY.Arac.Tanimla();

            string deger = "";
            if (uzayAlan == "")
                deger = servisUZY.UzayDegeriStr(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle);
            else
                deger = servisUZY.UzayDegeriStr(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle, uzayAlan);

            return deger;
        }

        /// <summary>
        /// Verilen kriterlere bağlı olarak uzay servisi ile bilgi döner
        /// </summary>
        /// <param name="uzayAd">Sorgulama yapılacak uzay adı</param>
        /// <param name="kosul">Sorgulama yapılacak koşul</param>
        /// <param name="esitle">Eğer <c>true</c> olarak verilirse koşul bilgisini "=" olarak listeler eğer <c>false</c> verilirse "%LIKE%" olarak listeler</param>
        /// <param name="uzayAlan">Sorgulama sonucu döndürülecek uzaya ait alan adı</param>
        /// <returns>Verilen kriterlere bağlı olarak uzay servisi ile double yapısında bilgi döner</returns>
        public static double UzaySayiDegerGetir(string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            if (servisUZY == null)
                servisUZY = TNS.UZY.Arac.Tanimla();

            double deger = 0;
            if (uzayAlan == "")
                deger = servisUZY.UzayDegeriDbl(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle);
            else
                deger = servisUZY.UzayDegeriDbl(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle, uzayAlan);

            return deger;
        }

        /// <summary>
        /// Harcama Yönetiminde kullanılacak İşlem bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek olan Is nesnesi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IsKaydet(Kullanici kullanan, Is bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIs().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Harcama Yönetiminde kullanılan İşlem bilgisini veritabanından okur 
        /// ve <see cref="Is"/> nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan"><see cref="Kullanici"/> bilgilerini içeren nesne</param>
        /// <param name="bilgi">Listelemede kriter olarak kullanılacak değerleri içeren <see cref="Is"/> nesnesi</param>
        /// <returns>İstenilen kriterdeki işlem listesini <see cref="Is"/> nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IsListele(Kullanici kullanan, Is bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCIs().Listele(kullanan, bilgi);
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
        /// Harcama Yönetiminde kullanılacak İşlem bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek olan TeminatNevi nesnesi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeminatMektubuNeviKaydet(Kullanici kullanan, TeminatMektubuNevi bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeminatNevi().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }


        /// <summary>
        /// Harcama Yönetiminde kullanılan İşlem bilgisini veritabanından okur 
        /// ve <see cref="TeminatMektubuNevi"/> nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan"><see cref="Kullanici"/> bilgilerini içeren nesne</param>
        /// <param name="bilgi">Listelemede kriter olarak kullanılacak değerleri içeren <see cref="TeminatMektubuNevi"/> nesnesi</param>
        /// <returns>İstenilen kriterdeki işlem listesini <see cref="TeminatMektubuNevi"/> nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TeminatMektubuNeviListele(Kullanici kullanan, TeminatMektubuNevi bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCTeminatNevi().Listele(kullanan, bilgi);
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
        /// Harcama Yönetiminde kullanılan İşlem bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek işlem bilgisini içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeminatMektubuNeviSil(Kullanici kullanan, TeminatMektubuNevi bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeminatNevi().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray AlimlariListele(Kullanici kullanan, HizmetAlimi bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCUAStf().AlimlariListele(kullanan, bilgi);
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
        /// Harcama Yönetiminde kullanılan İşlem bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek işlem bilgisini içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IsSil(Kullanici kullanan, Is bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIs().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Harcama Yönetiminde kullanılan parça bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek parça bilgisini içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc ParcaKaydet(Kullanici kullanan, Parca bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCParca().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Harcama Yönetiminde kullanılan parça bilgisini veritabanından okur 
        /// ve Parca nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Listelemede kriter olarak kullanılacak değerleri içeren Parca nesnesi</param>
        /// <returns>İstenilen kriterdeki parça listesini Parca nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ParcaListele(Kullanici kullanan, Parca bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCParca().Listele(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Harcama Yönetiminde kullanılan parça bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek parça bilgisini içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc ParcaSil(Kullanici kullanan, Parca bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCParca().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// İlişkilendirilmiş işlem ve parça bilgilerini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek işlem ve parça bilgilerini tutan IsParca nesnesi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IsParcaKaydet(Kullanici kullanan, IsParca bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIsParca().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// İlişkilendirilmiş işlem ve parça bilgilerini veritabanından okur 
        /// ve IsParca nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="isKod">Parça bilgileri listelenecek olan işlem kodu</param>
        /// <returns>Verilen işlem koduna ait parça bilgilerini tutan nesne</returns>
        public IsParca IsParcaListele(Kullanici kullanan, int isKod)
        {
            IsParca donusDeger = new IsParca();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIsParca().Listele(kullanan, isKod);
            }
            catch
            {
            }
            return donusDeger;
        }

        /// <summary>
        /// İlişkilendirilmiş ihale ve parça bilgilerini veritabanından okur ve IsParca nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="isKod">İhaleye türüne göre parça bilgileri listelenecek olan işlem kodu</param>
        /// <returns>Verilen işlem koduna ait ihale parça bilgilerini tutan nesne</returns>
        public IsParca IsIhaleParcaListele(Kullanici kullanan, int isKod)
        {
            IsParca donusDeger = new IsParca();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIsParca().ListeleIsIhaleParca(kullanan, isKod);
            }
            catch
            {
            }
            return donusDeger;
        }

        /// <summary>
        /// İlişkilendirilmiş ihale ve parça bilgilerini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek parça bilgilerini IhaleParcaBag nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleParcaGuncelle(Kullanici kullanan, ObjectArray bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleParca().Guncelle(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// İlişkilendirilmiş ihale ve parça bilgilerini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek parça bilgilerini tutan IhaleParca nesnesi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleParcaKaydet(Kullanici kullanan, IhaleParca bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleParca().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere bağlı ilişkili parçaları veritabanından okur ve IhaleParca nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ihaleUsul">Listelemede kriter olarak kullanılacak ihalu usul kodu</param>
        /// <param name="ihaleMadde">Listelemede kriter olarak kullanılacak ihale madde kodu</param>
        /// <param name="komisyon">Listelemede kriter olarak kullanılacak komisyon var mı yok mu bilgisi</param>
        /// <param name="tur">Listelemede kriter olarak kullanılacak işlem türü bilgisi</param>
        /// <returns>Verilen kriterlere ait ihale parça bilgilerini tutan nesne</returns>
        public IhaleParca IhaleParcaListele(Kullanici kullanan, int ihaleUsul, int ihaleMadde, int komisyon, int tur)
        {
            IhaleParca donusDeger = new IhaleParca();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleParca().Listele(kullanan, ihaleUsul, ihaleMadde, komisyon, tur);
            }
            catch
            {
            }
            return donusDeger;
        }

        /// <summary>
        /// Güncelleme servisinde kullanılmak üzere ilişkilendirilmiş tüm ihale ve parça listesini veritabanından okur ve
        /// IhaleParcaBag nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns>İlişkilendirilmiş tüm ihale ve parça listesini IhaleParcaBag nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IhaleParcaTumListe(Kullanici kullanan)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleParca().GuncellemeListesi(kullanan);
            }
            catch
            {
            }
            return donusDeger;
        }

        /// <summary>
        /// İhalede kullanılacak sözleşme metnini veritabanına kaydeder ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek sözleşme metnine ait bilgileri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleSozlesmeTanimKaydet(Kullanici kullanan, IhaleSozlesmeTanim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleSozlesmeTanim().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihalede kullanılacak sözleşme metnine ait bilgileri veritabanından okur 
        /// ve IhaleSozlesmeTanim nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Listelemede kriter olarak kullanılacak değerleri içeren IhaleSozlesmeTanim nesnesi</param>
        /// <returns>İstenilen kriterdeki, ihalede kullanılacak sözleşme metnine ait bilgiyi IhaleSozlesmeTanim nesnesi olarak tutan nesne</returns>
        public IhaleSozlesmeTanim IhaleSozlesmeTanimListele(Kullanici kullanan, IhaleSozlesmeTanim bilgi)
        {
            IhaleSozlesmeTanim donusDeger = new IhaleSozlesmeTanim();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleSozlesmeTanim().Listele(kullanan, bilgi);
            }
            catch
            {
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihalede kullanılacak sözleşme metnine ait bilgileri veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner 
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgileri tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleSozlesmeKaydet(Kullanici kullanan, IhaleSozlesme bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleSozlesme().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihalede kullanılacak sözleşme metnine ait bilgileri veritabanından okur 
        /// ve IhaleSozlesme nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale sözleşme bilgisini IhaleSozlesme nesnesi olarak tutan nesne</returns>
        public IhaleSozlesme IhaleSozlesmeListele(Kullanici kullanan, IhaleSozlesme kriter)
        {
            IhaleSozlesme donusDeger = new IhaleSozlesme();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleSozlesme().Listele(kullanan, kriter);
            }
            catch
            {
            }
            return donusDeger;
        }

        /// <summary>
        /// İhaleye ait belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleBelgeKaydet(Kullanici kullanan, IhaleBelge bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleBelge().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere bağlı ihale belge bilgisini veritabanından okur 
        /// ve IhaleBelge nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>Verilen kriterlere ait ihale belge bilgisini tutan nesne</returns>
        public IhaleBelge IhaleBelgeListele(Kullanici kullanan, IhaleBelge bilgi)
        {
            IhaleBelge donusDeger = new IhaleBelge();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleBelge().Listele(kullanan, bilgi);
            }
            catch
            {
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, öğrenci eğitmen bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OgrenciEgitmenKaydet(Kullanici kullanan, OgrenciEgitmen bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOgrenciEgitmen().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere göre Öğrenci ve Eğitmen bilgilerini veritabanından okur ve OgrenciEgitmen nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren OgrenciEgitmen nesnesi</param>
        /// <returns>İstenilen kriterdeki Öğrenci ve Eğitmen bilgileri listesini OgrenciEgitmen nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray OgrenciEgitmenListele(Kullanici kullanan, OgrenciEgitmen kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOgrenciEgitmen().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, Ödenek Gönderme Belgesi gönderilecek mailleri veritabanından okur
        /// ve AboneOdenekGondermeMail nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="mail">Listelemede kriter olarak kullanılacak değerleri içeren AboneOdenekGondermeMail nesnesi</param>
        /// <returns>İstenilen kriterdeki Ödenek Gönderme Mail listesini AboneOdenekGondermeMail nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray AboneOdenekGondermeMailListele(AboneOdenekGondermeMail mail)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCAboneFatura().OdenekGondermeMailListele(mail);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen Ödenek Gönderme Mail bilgisini veritabanına kaydeder ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="mail">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc AboneOdenekGondermeMailKaydet(Kullanici kullanan, AboneOdenekGondermeMail mail)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFatura().OdenekGondermeMailKaydet(kullanan, mail);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen Ödenek Gönderme Mail bilgisini veritabanından siler ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="mail">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc AboneOdenekGondermeMailSil(Kullanici kullanan, AboneOdenekGondermeMail mail)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFatura().OdenekGondermeMailSil(kullanan, mail);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere ait mail adreslerine Ödenek Gönderme Belgesini mail olarak gönderir
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi</param>
        /// <param name="aboneTip">İşlem yapılacak abone tipi bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc AboneOdenekGondermeMailAt(Kullanici kullanan, int yil, int ay, int aboneTip)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFatura().OdenekGondermeMailGonder(kullanan, yil, ay, aboneTip);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, abone fatura bilgisini içeren AboneFatura nesne koleksiyonunu veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri AboneFatura nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc AboneFaturaKaydet(Kullanici kullanan, ObjectArray liste)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCAboneFatura().Kaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak abone fatura bilgisini veritabanından okur 
        /// ve AboneFatura nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki abone fatura listesini AboneFatura nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray AboneFaturaListele(Kullanici kullanan, AboneFatura kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                TNSDateTime kontrolTarih = new TNSDateTime();
                return new HRCAboneFatura().Listele(kullanan, kriter, kontrolTarih, true);
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
        /// Arguman olarak verilen kriterlere bağlı olarak abone fatura istatistik bilgisini veritabanından okur 
        /// ve AboneFaturaIstatistikSinif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki abone fatura istatistik bilgisi listesini AboneFaturaIstatistikSinif nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray AboneFaturaListeleIstatistik(Kullanici kullanan, AboneFaturaIstatistikSinif kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFatura().ListeleAboneFaturaIstatistik(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;

        }

        /// <summary>
        /// Arguman olarak verilen abone fatura bilgisine ait gsm ödeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc AboneFaturaGsmBilgiKaydet(Kullanici kullanan, AboneFatura kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFatura().GsmBilgiKaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen abone fatura bilgisine ait Ödeme Emri Belge Numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc AboneFaturaOdemeEmriNoKaydet(Kullanici kullanan, AboneFatura kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFatura().OdemeEmriNoKaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, süreç bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SurecKaydet(Kullanici kullanan, Surec bilgi, SurecParca spBilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCSurec().Kaydet(kullanan, bilgi, spBilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc SurecKaydet(Kullanici kullanan, Surec bilgi)
        {
            return SurecKaydet(kullanan, bilgi, null);
        }

        /// <summary>
        /// Arguman olarak verilen, sürecin açıklama bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SurecKaydetAciklama(Kullanici kullanan, Surec bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCSurec().KaydetAciklama(kullanan, bilgi);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç bilgisini veritabanından okur 
        /// ve Surec nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, süreç listesini Surec nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray SurecListele(Kullanici kullanan, Surec kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCSurec.Listele(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray SurecDurumListele(Kullanici kullanan, SurecSorguKriter kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCSurec.SurecDurumListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        //private void TabloDegisimYap()
        //{
        //    if (tabloGuncelle == false)
        //    {
        //        tabloGuncelle = true;
        //        //TNS.Veri.VeriErisim ve = TNS.Veri.VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE PSLMAASTEMELBILGITANIM ADD AILEYARDIMIKATSAYISI MONEY");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE PSLGECICIGOREVYOLLUKMANAS(YIL INT, SURECNO VARCHAR(6), YI_YD INT, KIMLIKNO VARCHAR(11), UNVANKODU INT, GEREKCE VARCHAR(200), BASLANGICTARIH DATETIME, BITISTARIH DATETIME, BUTCETERTIBI VARCHAR(50), AVANSTUTARI MONEY, AVANSODEMEEMRI VARCHAR(50), ODEMEEMRI VARCHAR(50), DURUM INT, PRIMARY KEY(YIL, SURECNO))");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE PSLGECICIGOREVYOLLUKMANASDETAY (YIL INT, SURECNO VARCHAR(6), SIRANO INT, TUR INT, GUN INT, GECELIKKONAKLAMAUCRETI MONEY, YERKOD VARCHAR(20), GUNDELIK MONEY, ODENECEK MONEY, ACIKLAMA VARCHAR(200), PRIMARY KEY(YIL, SURECNO, SIRANO) )");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE dbo.HRCGUNDELIK(KOD int NOT NULL, YIL int NULL, UNVANKOD int NULL, COGRAFYAKOD varchar(50) NULL, GUNDELIK money NULL, PARABIRIMKOD int NULL, PRIMARY KEY CLUSTERED (KOD ASC))");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE HRCGUNDELIK ADD TUR INT NOT NULL DEFAULT(1)");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE dbo.PSLGECICIGOREVYOLLUKBELGE ADD YURTICIKONAKLAMATURU INT NOT NULL DEFAULT (1)");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE dbo.HRCTEMINATMEKTUBU ADD SOZLESMEKOD int NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE dbo.HRCFIRMABILGISI ADD KARALISTE int NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE [dbo].[HRCBORDRORAPORSABLON](
        //        //                 [KOD] [int] NOT NULL,
        //        //                 [KULLANICIKODU] [varchar](30) NULL,
        //        //                 [AD] [varchar](250) NULL,
        //        //                 [ACIKLAMA] [varchar](max) NULL,
        //        //                     CONSTRAINT [PK_HRCBORDRORAPORSABLON] PRIMARY KEY CLUSTERED ([KOD] ASC
        //        //                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        //        //                    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE dbo.HRCTEKLIF ADD PARABIRIMI int NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE dbo.HRCTEKLIFGENELTOPLAM ADD PARABIRIMI int NULL, TEKLIFTARIHI datetime NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE HRCFIRMABILGISI ADD ACIKLAMA varchar(500)");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE PSLMAASTEMELBILGITANIM ADD TOPLUGORUSMESABITI MONEY");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE HRCBORDROTABLODETAY ADD YIL INT NOT NULL DEFAULT 0");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE HRCBORDROTANIMANA
        //        //    (BORDROTUR INT
        //        //    , BORDROISLEMTUR INT
        //        //    , BORDROAD VARCHAR(50)
        //        //    , KAYNAKBORDROTUR INT
        //        //    , KAYNAKBORDROISLEMTUR INT
        //        //    , PRIMARY KEY (BORDROTUR, BORDROISLEMTUR))");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE PSLMAASTEMELBILGITANIM ADD SSKTAVAN MONEY");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE [dbo].[HRCKOMISYONUYESABLON](
        //        //                 [KOD] [varchar](40) NOT NULL,
        //        //                 [SAHIBI] [varchar](50) NULL,
        //        //                 [ADI] [varchar](150) NULL,
        //        //                 [DURUM] [int] NULL,
        //        //                     CONSTRAINT [PK_HRCKOMISYONUYESABLON] PRIMARY KEY CLUSTERED 
        //        //                    ([KOD] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE [dbo].[HRCKOMISYONUYESABLONDETAY](
        //        //                 [KOD] [varchar](40) NOT NULL,
        //        //                 [KOMISYONUYESABLONKOD] [varchar](40) NULL,
        //        //                 [SIRANO] [int] NOT NULL,
        //        //                 [KOMISYONGOREV] [int] NULL,
        //        //                 [AD] [varchar](255) NULL,
        //        //                 [GOREV] [varchar](255) NULL,
        //        //                 [UNVAN] [varchar](255) NULL,
        //        //                 [SICILNO] [varchar](255) NULL,
        //        //                 [TELEFON] [varchar](255) NULL,
        //        //                 [CEPTELEFON] [varchar](255) NULL,
        //        //                  CONSTRAINT [PK_HRCKOMISYONUYESABLONDETAY] PRIMARY KEY CLUSTERED 
        //        //                  ([KOD] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE NONCLUSTERED INDEX [IX_HRCKOMISYONUYESABLONDETAY] ON [dbo].[HRCKOMISYONUYESABLONDETAY]
        //        //                    ([KOMISYONUYESABLONKOD] ASC ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE dbo.HRCTEKLIFGENELTOPLAM ADD TEKLIFTARIHI datetime NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE dbo.HRCTASINIRFORM ADD DOVIZKUR decimal(24, 18) NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE dbo.HRCODEMEEMRIFORM ADD PRFATURANOEK varchar(500) NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE dbo.HRCTEMINATMEKTUBU ADD EPOSTAUYARIGONDTARIH datetime NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE dbo.HRCABONEFATURA ADD	DAMGAVERGISI money NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"Create table HRCGGYTALEPBASLIK(
        //        //                     YIL INT, 
        //        //                     FORMKOD VARCHAR(20), 
        //        //                     MUHASEBE INT, 
        //        //                     BIRIM VARCHAR(20), 
        //        //                     KURKOD VARCHAR(20), 
        //        //                     DURUM INT, 
        //        //                     TARIH DATETIME, 
        //        //                     TCKIMLIKNO VARCHAR(20), 
        //        //                     AD VARCHAR(200), 
        //        //                     SOYAD VARCHAR(200), 
        //        //                     UNVAN VARCHAR(20), 
        //        //                     GUNDELIKYIL INT, 
        //        //                     GUNDELIKKOD VARCHAR(20), 
        //        //                     GUNDELIKTUTAR DECIMAL(18,2), 
        //        //                     GUNDELIKDOVIZKOD INT, 
        //        //                     NEREDEN VARCHAR(200), 
        //        //                     NEREYE VARCHAR(200), 
        //        //                     BASLANGICTARIH DATETIME, 
        //        //                     BITISTARIH DATETIME, 
        //        //                     SURECYIL INT, 
        //        //                     SURECNO VARCHAR(20), 
        //        //                     TUR INT, 
        //        //                     GOREVSURESI INT, 
        //        //                     GEREKCE VARCHAR(200), 
        //        //                     ACIKLAMALAR VARCHAR(200), 
        //        //                     TERTIP VARCHAR(200), 
        //        //                     AVANS MONEY, 
        //        //                     SUNANAD VARCHAR(200), 
        //        //                     SUNANUNVAN VARCHAR(200), 
        //        //                     HAZIRAD VARCHAR(200), 
        //        //                     HAZIRUNVAN VARCHAR(200), 
        //        //                     ONAYAD VARCHAR(200), 
        //        //                     ONAYUNVAN VARCHAR(200), 
        //        //                     ONAYMAKAM VARCHAR(200), 
        //        //                     AVANSODEMEEMRI VARCHAR(20), 
        //        //                     PRIMARY KEY (YIL, FORMKOD))");
        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"Create table HRCGGYTALEPDETAY(
        //        //                     YIL INT, 
        //        //                     FORMKOD VARCHAR(20), 
        //        //                     SIRANO INT, 
        //        //                     TUR INT, 
        //        //                     TARIH DATETIME, 
        //        //                     GUN VARCHAR(20), 
        //        //                     PRIMARY KEY (YIL, FORMKOD, SIRANO))");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"Create table HRCGGYTALEPBERABER(
        //        //                     YIL INT, 
        //        //                     FORMKOD VARCHAR(20), 
        //        //                     SIRANO INT, TCKIMLIK VARCHAR(20), 
        //        //                     PRIMARY KEY (YIL, FORMKOD, SIRANO))");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"create table HRCBORDROEPOSTAGONDER(SURECYIL INT, SURECNO VARCHAR(20), BORDROTUR INT, BORDROISLEMTUR INT, TCKIMLIKNO VARCHAR(20), EPOSTA VARCHAR(100), EKLEMEZAMANI DATETIME, EKLEYEN VARCHAR(100), SONUCZAMANI DATETIME, SONUCSTR VARCHAR(200), ISLEMEALINDI DATETIME, PRIMARY KEY(SURECYIL, SURECNO, BORDROTUR, BORDROISLEMTUR, TCKIMLIKNO, EKLEMEZAMANI))");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE dbo.HRCBORDRODONEM ADD BORDROTUR varchar(250) NULL, BORDROISLEM INT NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE dbo.PSLCESITLIODEMEBORDRO ADD AVANSTUTAR money NULL, AVANSODEMEEMRINO varchar(20) NULL, ODEMEGUN DATETIME NULL, ODEMESIRANO INT NULL, AVANSODEMEGUN DATETIME NULL, AVANSODEMESIRANO INT NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE dbo.HRCKOMISYONUYE ADD TESLIMALAN varchar(50) NULL, TESLIMTARIH datetime NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE dbo.PSLCESITLIODEMEBORDRO ADD PARABIRIMI int NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE dbo.HRCTEKLIFGENELTOPLAM ADD PARABIRIMI int NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE dbo.HRCFATURAFORM ADD PARABIRIMI int NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE dbo.HRCTEKLIF ADD PARABIRIMI int NULL");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"ALTER TABLE HRCBORDROPARAM ADD TIP INT NOT NULL DEFAULT 1, YONTEM INT NOT NULL DEFAULT 1, LISTE VARCHAR(2000)");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE HRCUASTFIHTIYACBASLIK (
        //        //   YIL INT NOT NULL,
        //        //   FORMKOD VARCHAR(20)  NOT NULL,
        //        //   SIRANO INT  NOT NULL,
        //        //   ACIKLAMA VARCHAR(500)  NOT NULL,
        //        //   PRIMARY KEY (YIL, FORMKOD, SIRANO))");

        //        //ve.hata = "";
        //        //ve.ExecuteNonquery(@"CREATE TABLE HRCUASTFIHTIYACDETAY (
        //        //   YIL INT NOT NULL,
        //        //   FORMKOD VARCHAR(20)  NOT NULL,
        //        //   SIRANO INT NOT NULL,
        //        //   DETAYSATIRNO INT NOT NULL,
        //        //   IHTIYACKOD VARCHAR(20) NOT NULL,
        //        //   IHTIYACAD VARCHAR(200) NOT NULL,
        //        //   MIKTAR1 DECIMAL(18,6) NOT NULL,
        //        //   OLCUBIRIMKOD1 INT NOT NULL,
        //        //   MIKTAR2 DECIMAL(18,6),
        //        //   OLCUBIRIMKOD2 INT,
        //        //   ACIKLAMA VARCHAR(200) NOT NULL,
        //        //   PRIMARY KEY (YIL, FORMKOD, SIRANO, DETAYSATIRNO))");

        //        //ve.VeriErisimKapat();
        //    }
        //}

        /// <summary>
        /// Arguman olarak verilen, sürecin durum bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SurecDurumDegistir(Kullanici kullanan, Surec kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSurec().DurumDegistir(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SurecIslemDegistir(Kullanici kullanan, Surec kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSurec().IslemDegistir(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç bilgisini veritabanından okur 
        /// ve Surec nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, süreç listesini Surec nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray SurecSorgula(Kullanici kullanan, SurecSorguKriter kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCSurec().Sorgula(kullanan, kriter);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç bilgisini veritabanından okur 
        /// ve Surec nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, süreç listesini Surec nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray SurecMuhMifBaglantiListele(Kullanici kullanan, SurecMuhMifBaglanti kriter, Tertip tertip)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSurec().ListeleSurecMuhMifBaglanti(kullanan, kriter, tertip);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, devam eden süreç bilgisini veritabanından okur 
        /// ve Surec nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, devam eden süreç listesini Surec nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray SurecDevamEdenler(Kullanici kullanan, SurecSorguKriter kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCSurec().DevamEdenler(kullanan, kriter);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, sürece ait parçaların durum bilgisini veritabanından okur 
        /// ve SurecParcaDurum nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürece ait parça durum listesini SurecParcaDurum nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray SurecParcaDurumListele(Kullanici kullanan, SurecParcaDurum kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSurec().ParcaDurumListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, süreç parçasının durum bilgisini onaylandı olarak veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SurecParcaDurumOnayVer(Kullanici kullanan, SurecParcaDurum bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCSurec().ParcaDurumOnayVer(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Arguman olarak verilen, süreç parçasının durum bilgisini devam ediyor ya da tanımsız olarak veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SurecParcaDurumOnayKaldir(Kullanici kullanan, SurecParcaDurum bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSurec().ParcaDurumOnayKaldir(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, teklife ait malzeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri TeklifMalzeme nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılan sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılan sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeklifMalzemeKaydet(Kullanici kullanan, ObjectArray liste, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifMalzeme().Kaydet(kullanan, liste, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, teklife ait malzeme bilgisini veritabanından okur 
        /// ve Teklif Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki teklife ait malzeme listesini TeklifMalzeme nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TeklifMalzemeListele(Kullanici kullanan, TeklifMalzeme kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCTeklifMalzeme().Listele(kullanan, kriter);
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
        /// Sistemde bulunan tüm ölçü birimi bilgisini veritabanından okur 
        /// ve OlcuBirim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <returns>Sistemde bulunan ölçü birimi bilgisini OlcuBirim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray OlcuBirimListele(Kullanici kullanan)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCOlcuBirim().Listele(kullanan);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public string OlcuBirimAdBul(Kullanici kullanan, int birimKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return new HRCOlcuBirim().OlcuBirimAdBul(kullanan, birimKod);
        }

        /// <summary>
        /// Arguman olarak verilen, teklif yapacak firma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri TeklifFirma nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılan sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılan sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeklifFirmaKaydet(Kullanici kullanan, ObjectArray liste, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifFirma().Kaydet(kullanan, liste, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, teklif yapacak firma bilgisini veritabanından okur 
        /// ve TeklifFirma nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, teklif yapacak firma listesini TeklifFirma nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TeklifFirmaListele(Kullanici kullanan, TeklifFirma kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCTeklifFirma().Listele(kullanan, kriter);
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
        /// Arguman olarak verilen dönem teklif bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri TeklifDonem nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılan sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılan sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeklifDonemKaydet(Kullanici kullanan, ObjectArray liste, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifDonem().Kaydet(kullanan, liste, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, dönem teklif bilgisini veritabanından okur 
        /// ve TeklifDonem nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, dönem teklif listesini TeklifDonem nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TeklifDonemListele(Kullanici kullanan, TeklifDonem kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifDonem().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, firmalar tarafından malzemelere yapılan teklif bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri Teklif nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemkod">İşlem yapılan sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılan sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeklifKaydet(Kullanici kullanan, ObjectArray liste, int islemkod, int parcaKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCTeklif().Kaydet(kullanan, liste, islemkod, parcaKod);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, firmalara ait malzeme teklif bilgisini veritabanından okur 
        /// ve Teklif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, firmalara ait malzeme teklif bilgisi listesini Teklif nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TeklifListele(Kullanici kullanan, Teklif kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklif().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public TeklifKazananFirma KazananGetir(Kullanici kullanan, int yil, string surecNo, int malzemeNo)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCTeklif().KazananGetir(null, kullanan, yil, surecNo, malzemeNo);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, Kısmi Teklif ile ihale kazanan 1. ve 2. firma bilgisini veritabanından okur 
        /// ve Teklif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki kısmi teklif kazanan firma bilgisi listesini Teklif nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TeklifKismiKazananListele(Kullanici kullanan, Teklif kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCTeklif().ListeleKismiTeklifKazanan(kullanan, kriter);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, firmalar tarafından teklif verilen malzemelere ait ortalama birim fiyat bilgisini veritabanından okur 
        /// ve Teklif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, firmalar tarafından teklif verilen malzemelere ait ortalama birim fiyat bilgisi listesini Teklif nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TeklifListeleYMOrtBirimFiyat(Kullanici kullanan, Teklif kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklif().ListeleYMOrtBirimFiyat(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, teklif genel toplam bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeklifGenelToplamKaydet(Kullanici kullanan, TeklifGenelToplam kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifGenelToplam().Kaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc TeklifGenelToplamSil(Kullanici kullanan, TeklifGenelToplam kriter, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifGenelToplam().Sil(kullanan, kriter, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, teklife ait genel toplam bilgisini veritabanından okur 
        /// ve TeklifGenelToplam nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, teklife ait genel toplam bilgisi listesini TeklifGenelToplam nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TeklifGenelToplamListele(Kullanici kullanan, TeklifGenelToplam kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifGenelToplam().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreçte tanımlı ihale için kazanan firma sayısını veritabanından okur ve döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <returns>İstenilen kriterdeki, süreçte tanımlı ihale için kazanan firma sayısı</returns>
        public int TeklifKazananFirmaSayisiGetir(Kullanici kullanan, int yil, string surecNo)
        {
            int donusDeger = 0;
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifFirma().KazananFirmaSayisiGetir(kullanan, yil, surecNo);
            }
            catch
            {

            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kazanan firma bilgisini veritabanından okur 
        /// ve TeklifFirma nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <returns>İstenilen kriterdeki, kazanan firma bilgi listesini TeklifFirma nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TeklifKazananFirmaListele(Kullanici kullanan, int yil, string surecNo)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCTeklifFirma().ListeleKazananFirma(kullanan, yil, surecNo);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray TeklifKazananFirmaListeleGenel(Kullanici kullanan, int yil, string surecNo, int malzemeNo)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCTeklif().KazananGetirListe(null, kullanan, yil, surecNo, malzemeNo);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, firmanın ihalesini kazandığı malzeme bilgisini veritabanından okur 
        /// ve TeklifMalzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <param name="firmaNo">İşlem yapılacak firma no bilgisi</param>
        /// <returns>İstenilen kriterdeki, firmanın ihalesini kazandığı malzeme bilgisi listesini TeklifMalzeme nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TeklifKazananFirmaMalzemeListele(Kullanici kullanan, int yil, string surecNo, int firmaNo)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifMalzeme().ListeleMalzemeKazananFirma(kullanan, yil, surecNo, firmaNo);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, Üretici fiyatları Endeksi bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri Ufe nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc UfeKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCUfe().Kaydet(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, Üretici Fiyatları Endeksi bilgisini veritabanından okur 
        /// ve Ufe nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, Üretici Fiyatları Endeksi bilgisi listesini Ufe nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray UfeListele(Kullanici kullanan, Ufe kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCUfe().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen yıl ve ay kriterlerinden bugune kadarki Üretici Fiyatları Endeksi oranlarını kümülatif toplam olarak veritabanından okur ve döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi</param>
        /// <returns>İstenilen kriterdeki, yıl ve ay kriterlerinden bugune kadarki Üretici Fiyatları Endeksi oranlarının kümülatif toplam bilgisi</returns>
        public double UfeToplami(Kullanici kullanan, int yil, int ay)
        {
            double donusDeger = 0.0;
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCUfe().UfeToplami(kullanan, yil, ay);
            }
            catch
            {
                donusDeger = 0.0;
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, komisyon üyeleri bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KomisyonUyeKaydet(Kullanici kullanan, KomisyonUye liste)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return KomisyonUyeKaydet(kullanan, liste, 0, 0);
        }

        /// <summary>
        /// Arguman olarak verilen, komisyon üyeleri bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KomisyonUyeKaydet(Kullanici kullanan, KomisyonUye liste, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCKomisyonUye().Kaydet(kullanan, liste, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, komisyon üye bilgisini veritabanından okur 
        /// ve KomisyonUye nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, komisyon üye bilgisi listesini tutan KomisyonUye nesnesi</returns>
        public KomisyonUye KomisyonUyeListele(Kullanici kullanan, KomisyonUye kriter)
        {
            KomisyonUye donusDeger = new KomisyonUye();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCKomisyonUye().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.uyeler.sonuc.islemSonuc = false;
                donusDeger.uyeler.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, fatura bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FaturaFormKaydet(Kullanici kullanan, FaturaForm kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFaturaForm().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fatura bilgisini veritabanından okur 
        /// ve FaturaForm nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, fatura listesini FaturaForm nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray FaturaFormListele(Kullanici kullanan, FaturaForm kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCFaturaForm().Listele(kullanan, kriter, false);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray FaturaFormListeleServis(Kullanici kullanan, FaturaForm kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCFaturaForm().ListeleServis(kullanan, kriter);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray FaturaFormListeleIhaleMalzemeli(Kullanici kullanan, FaturaForm kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCFaturaForm().Listele(kullanan, kriter, false);//true olduğunda Piyasa Fiyattaki tüm malzemeler geliyor. Oysa faturaya kayıt edenler gelecek. Cumhur 06.06.2017 Melih
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }
        /// <summary>
        /// Arguman olarak verilen, faturanın durum bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FaturaFormDurumDegistir(Kullanici kullanan, FaturaForm kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCFaturaForm().DurumDegistir(kullanan, kriter);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, fatura form kontrol bilgisini veritabanından okur 
        /// ve gerekli kontrolleri yaptıktan sonra işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FaturaFormListeleKontrol(Kullanici kullanan, FaturaForm kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFaturaForm().ListeleKontrol(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, faturaya ait ödeme emri belge numarasını veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FaturaFormOdemeEmriNoKaydet(Kullanici kullanan, FaturaForm kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFaturaForm().OdemeEmriNoKaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, taşınır form bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TasinirFormKaydet(Kullanici kullanan, TasinirForm kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTasinirForm().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, taşınır form bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TasinirFormSil(Kullanici kullanan, TasinirForm kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTasinirForm().Sil(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, taşınır form bilgisini veritabanından okur 
        /// ve TasinirForm nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, taşınır form listesini TasinirForm nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TasinirFormListele(Kullanici kullanan, TasinirForm kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTasinirForm().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ödeme emri belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OdemeEmriFormKaydet(Kullanici kullanan, OdemeEmriMIFForm kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOdemeEmriForm().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ödeme emri belge bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OdemeEmriFormSil(Kullanici kullanan, OdemeEmriMIFForm kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOdemeEmriForm().Sil(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ödeme emri belge bilgisini veritabanından okur 
        /// ve OdemeEmriMIFForm nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ödeme emri belge listesini OdemeEmriMIFForm nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray OdemeEmriFormListele(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOdemeEmriForm().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        ///// <summary>
        ///// Arguman olarak verilen kriterlere bağlı olarak, ödeme emri belgesi miktar toplam bilgisini veritabanından okur 
        ///// ve OdemeEmriMIFDetay nesne koleksiyonu olarak döner
        ///// </summary>
        ///// <param name="kullanan">The kullanan.</param>
        ///// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        ///// <param name="hesapKod">İşlem yapılacak hesap kod bilgisi</param>
        ///// <returns>İstenilen kriterdeki, ödeme emri belgesi miktar toplam bilgisi listesini OdemeEmriMIFDetay nesne koleksiyonu olarak tutan nesne</returns>
        //public ObjectArray OdemeEmriMiktarListele(Kullanici kullanan, OdemeEmriMIFForm kriter, string hesapKod)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        TNS.KYM.Arac.KulturDegistir(kullanan);
        //        donusDeger = new HRCOdemeEmriForm().ListeleMiktar(kullanan, kriter, hesapKod);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        ///// <summary>
        ///// Arguman olarak verilen kriterlere bağlı olarak, ödeme emri belgesi miktar toplam bilgisini veritabanından okur 
        ///// ve OdemeEmriMIFDetay nesne koleksiyonu olarak döner
        ///// </summary>
        ///// <param name="kullanan">The kullanan.</param>
        ///// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        ///// <param name="hesapKod">İşlem yapılacak hesap kod bilgisi</param>
        ///// <returns>İstenilen kriterdeki, ödeme emri belgesi miktar toplam bilgisi listesini OdemeEmriMIFDetay nesne koleksiyonu olarak tutan nesne</returns>
        //public ObjectArray OdemeEmriMiktarListele(Kullanici kullanan, OdemeEmriMIFForm kriter, string hesapKod, string tertip)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        TNS.KYM.Arac.KulturDegistir(kullanan);
        //        donusDeger = new HRCOdemeEmriForm().ListeleMiktar(kullanan, kriter, hesapKod, tertip);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        /// <summary>
        /// Arguman olarak verilen, ödeme emri belgesinin durumunu iptal olarak veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OdemeEmriFormIptalEt(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOdemeEmriForm().OdemeEmriIptal(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, taahhutname bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="tcKimlikNo">TC Kimlik Numarası</param>
        /// <param name="tur">Tür</param>
        /// <param name="tarih">Tarih</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TaahhutnameKaydet(Kullanici kullanan, string tcKimlikNo, int tur, TNSDateTime tarih)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new HRCTaahhutname().Kaydet(kullanan, tcKimlikNo, tur, tarih);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, taahhutname bilgisini veritabanından okur 
        /// ve [tcKimlikNo|tur|tarih] string nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="tcKimlikNo">TC Kimlik Numarası</param>
        /// <param name="tur">Tür</param>
        /// <param name="tarih">Tarih</param>
        /// <returns></returns>
        public ObjectArray TaahhutnameListele(string tcKimlikNo, int tur, TNSDateTime tarih)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCTaahhutname().Listele(tcKimlikNo, tur, tarih);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, firma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FirmaBilgisiKaydet(Kullanici kullanan, FirmaBilgisi kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCFirmaBilgisi().Kaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ilgili firmanın sisteme giriş bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="vergiNo">İşlem yapılacak firmaya ait vergi numarası</param>
        /// <param name="parola">İşlem yapılacak firmaya ait parola</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FirmaGirisYap(string vergiNo, string parola)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new HRCFirmaBilgisi().FirmaGirisYap(vergiNo, parola);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, firmanın yaptığı işleri veritabanından okur 
        /// ve Teklif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="vNo">İşlem yapılacak firmaya ait vergi numarası</param>
        /// <param name="sadeceDevamEdenler">Eğer <c>true</c> verilirse firmaya ait devam işler listelenir</param>
        /// <returns>İstenilen kriterdeki, firma işleri listesini Teklif nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray FirmaninIsleri(Kullanici kullanan, int yil, string vNo, bool sadeceDevamEdenler)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFirmaBilgisi().FirmaninIsleri(kullanan, yil, vNo, sadeceDevamEdenler);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, firma bilgisini veritabanından okur 
        /// ve FirmaBilgisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, firma bilgisi listesini FirmaBilgisi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray FirmaBilgisiListele(Kullanici kullanan, FirmaBilgisi kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCFirmaBilgisi().Listele(kullanan, null, kriter);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, firma bilgisini veritabanından okur 
        /// ve FirmaBilgisi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="firmaKod">İşlem yapılacak firma kodu</param>
        /// <returns>İstenilen kriterdeki, firma bilgisini FirmaBilgisi nesnesi olarak tutan nesne</returns>
        public FirmaBilgisi FirmaBilgisiListele(Kullanici kullanan, int firmaKod)
        {
            FirmaBilgisi kriter = new FirmaBilgisi();
            kriter.kod = firmaKod;
            ObjectArray liste = FirmaBilgisiListele(kullanan, kriter);
            if (liste.ObjeSayisi > 0)
                kriter = (FirmaBilgisi)liste[0];
            else
                kriter = new FirmaBilgisi();

            return kriter;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, firma bilgisini veritabanından okur
        /// ve FirmaBilgisi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="muhasebe">İşlem yapılacak firmanın kayıtlı olduğu muhasebe</param>
        /// <param name="vNo">İşlem yapılacak firma vergi numarası</param>
        /// <returns>
        /// İstenilen kriterdeki, firma bilgisini FirmaBilgisi nesnesi olarak tutan nesne
        /// </returns>
        public FirmaBilgisi FirmaBilgisiListele(Kullanici kullanan, int muhasebe, string vNo)
        {
            FirmaBilgisi kriter = new FirmaBilgisi();
            kriter.muhasebe = muhasebe;
            kriter.vergiNo = vNo;
            ObjectArray liste = FirmaBilgisiListele(kullanan, kriter);
            if (liste.ObjeSayisi > 0)
                kriter = (FirmaBilgisi)liste[0];
            else
                kriter = new FirmaBilgisi();

            return kriter;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, firma bilgisini veritabanından okur 
        /// ve FirmaBilgisi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, firma bilgisini FirmaBilgisi nesnesi olarak tutan nesne</returns>
        public FirmaBilgisi FirmaGirisListele(Kullanici kullanan, FirmaBilgisi kriter)
        {
            FirmaBilgisi donusDeger = new FirmaBilgisi();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFirmaBilgisi().FirmaGirisListele(kullanan, null, kriter);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, firma parola bilgisinin geçerliliğini kontrol eder, geçerliyse veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="eskiParola">İşlem yapılacak firmaya ait eski parola</param>
        /// <param name="yeniParola1">İşlem yapılacak firmaya ait yeni parola</param>
        /// <param name="yeniParola2">İşlem yapılacak firmaya ait yeni parola tekrarı</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FirmaParolaDegistir(Kullanici kullanan, string eskiParola, string yeniParola1, string yeniParola2)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFirmaBilgisi().ParolaDegistir(kullanan, eskiParola, yeniParola1, yeniParola2);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, firmanın sistemi giriş bilgisini veritabanına kaydeder,
        /// eğer firma sisteme kayıtlı ve parola bilgisi yoksa ya da yeni parola talebi varsa 
        /// firma yetkilisine mail olarak parola gönderir
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="yeniParola">Eğer <c>true</c> verilirse firma yetkilisine mail yoluyla parola gönderilir</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FirmaGirisKaydet(Kullanici kullanan, FirmaBilgisi kriter, bool yeniParola)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFirmaBilgisi().FirmaGirisKaydet(kullanan, null, kriter, yeniParola);
            }
            catch { }
            return donusDeger;
        }

        // ***
        /// <summary>
        /// Arguman olarak verilen, kullanıcı koduna ait firmanın sistem son giriş tarihi bilgisini veritabanına kaydeder
        /// </summary>
        /// <param name="kullaniciKodu">İşlem yapılacak firmaya ait kullanıcı kodu</param>
        public void FirmaGirisTarihKaydet(string kullaniciKodu)
        {
            try
            {
                new HRCFirmaBilgisi().FirmaGirisTarihKaydet(kullaniciKodu);
            }
            catch { }
        }

        /// <summary>
        /// Arguman olarak verilen, onay form bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public TNS.MUH.EksikOdenekler OnayFormKaydet(Kullanici kullanan, OnayForm kriter, int islemKod, int parcaKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCOnayForm().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                TNS.MUH.EksikOdenekler eo = new MUH.EksikOdenekler();
                eo.sonuc = new Sonuc(e.ToString());
                return eo;
            }
        }

        /// <summary>
        /// Arguman olarak verilen, onay form evrak sayı bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OnayFormEvrakSayiKaydet(Kullanici kullanan, OnayForm kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOnayForm().EvrakSayiKaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, onay form belge bilgisini veritabanından okur 
        /// ve OnayForm nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, onay form belge listesini OnayForm nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray OnayFormListele(Kullanici kullanan, OnayForm kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCOnayForm().Listele(kullanan, kriter);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, onay form belge tertip bilgisini veritabanından okur 
        /// ve OnayFormTertip nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, onay form belge tertip listesini OnayFormTertip nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray OnayFormTertipListele(Kullanici kullanan, OnayFormTertip kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOnayForm().ListeleTertip(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray BlokeTertipListele(Kullanici kullanan, OnayFormTertip kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCOnayForm().ListeleBlokeTertip(kullanan, kriter);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc BlokeTertipleriOtomatikGuncelle(Kullanici kullanan, int surecYil, string surecNo, int tertipYil, ENUMKontrolDurum kontrol)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCOnayForm().BlokeTertipleriOtomatikGuncelle(null, kullanan, surecYil, surecNo, tertipYil, kontrol);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, onay form belge tertip bilgisini veritabanından okur 
        /// ve OnayFormTertip nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, onay form belge tertip listesini OnayFormTertip nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray OnayFormTertipBenzerListele(Kullanici kullanan, OnayFormTertip kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOnayForm().ListeleTertipBenzer(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, onay form belge tertip bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OnayFormTertipGuncelle(Kullanici kullanan, OnayFormTertip kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCOnayForm().GuncelleTertip(kullanan, kriter);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc OnayFormTertipKaydet(Kullanici kullanan, OnayFormTertip kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOnayForm().KaydetTertip(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, onay form belge tertibine ait bloke edilen tutar bilgisini boş olarak veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc BlokeKaldirTertip(Kullanici kullanan, OnayFormTertip kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOnayForm().BlokeKaldirTertip(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc BlokeEkleTertip(Kullanici kullanan, OnayFormTertip kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOnayForm().BlokeEkleTertip(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        //public decimal BlokeHesapla(Kullanici kullanan, int yil, string surecNo)
        //{
        //    decimal donusDeger = decimal.Zero;
        //    try
        //    {
        //        TNS.KYM.Arac.KulturDegistir(kullanan);
        //        donusDeger = new HRCOnayForm().BlokeHesapla(kullanan, yil, surecNo);
        //    }
        //    catch
        //    {
        //    }
        //    return donusDeger;
        //}

        /// <summary>
        /// Arguman olarak verilen, kiralama bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KiralamaKaydet(Kullanici kullanan, Kiralama kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCKiralama().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kiralama bilgisini veritabanından okur 
        /// ve Kiralama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, kiralama listesini Kiralama nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray KiralamaListele(Kullanici kullanan, Kiralama kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCKiralama().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, para birimi bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc ParaBirimiKaydet(Kullanici kullanan, ParaBirimi bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCParaBirimi().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, para birimi bilgisini veritabanından okur 
        /// ve ParaBirimi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, para birimi listesini ParaBirimi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ParaBirimiListele(Kullanici kullanan, ParaBirimi kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCParaBirimi.Listele(null, kullanan, kriter);
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
        /// Arguman olarak verilen, para birimi bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc ParaBirimiSil(Kullanici kullanan, ParaBirimi bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCParaBirimi().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale tür tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri IhaleTur nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleTurKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleTur().Kaydet(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale tür tanım bilgisini veritabanından okur 
        /// ve IhaleTur nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale tür tanım bilgisi listesini IhaleTur nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IhaleTurListele(Kullanici kullanan, IhaleTur kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleTur().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale türüne ait madde tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri IhaleTurMadde nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleTurMaddeKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleTurMadde().Kaydet(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale türüne ait madde tanım bilgisini veritabanından okur 
        /// ve IhaleTurMadde nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="ihaleKod">İşlem yapılacak ihale tür kodu</param>
        /// <returns>İstenilen kriterdeki, ihale türüne ait madde tanım bilgisi listesini IhaleTurMadde nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IhaleTurMaddeListele(Kullanici kullanan, int ihaleKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleTurMadde().Listele(kullanan, ihaleKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale türüne ait madde tanım bilgisini veritabanından okur 
        /// ve IhaleTurMadde nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="ihaleKod">İşlem yapılacak ihale tür kodu</param>
        /// <param name="maddeKod">İşlem yapılacak ihale madde kodu</param>
        /// <returns>İstenilen kriterdeki, ihale türüne ait madde tanım bilgisini IhaleTurMadde nesnesi olarak tutan nesne</returns>
        public IhaleTurMadde IhaleUsulMaddeAl(Kullanici kullanan, int ihaleKod, int maddeKod)
        {
            IhaleTurMadde donusDeger = new IhaleTurMadde();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleTurMadde().IhaleUsulMaddeAl(kullanan, ihaleKod, maddeKod);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale türüne ait ilan tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri IhaleIlanTanim nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleIlanKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleIlanTanim().Kaydet(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale türüne ait ilan tanım bilgisini veritabanından okur 
        /// ve IhaleIlanTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="ihaleKod">İşlem yapılacak ihale tür kodu</param>
        /// <param name="ihaleMaddeSira">İşlem yapılacak ihale türüne ait madde kodu</param>
        /// <returns>İstenilen kriterdeki, ihale türüne ait ilan tanım bilgisi listesini IhaleIlanTanim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IhaleIlanListele(Kullanici kullanan, int ihaleKod, int ihaleMaddeSira)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleIlanTanim().Listele(kullanan, ihaleKod, ihaleMaddeSira);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale türüne ait ilan tanım bilgisini veritabanından okur 
        /// ve IhaleIlanTanim nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="ihaleKod">İşlem yapılacak ihale tür kodu</param>
        /// <param name="fiyat">İşlem yapılacak fiyat bilgisi</param>
        /// <param name="isTur">İşlem yapılacak süreç işlem kodu</param>
        /// <returns>İstenilen kriterdeki, ihale türüne ait ilan tanım bilgisini IhaleIlanTanim nesnesi olarak tutan nesne</returns>
        public IhaleIlanTanim IhaleIlanSekliAl(Kullanici kullanan, int ihaleKod, double fiyat, int isTur)
        {
            IhaleIlanTanim donusDeger = new IhaleIlanTanim();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleIlanTanim().IhaleIlanSekliAl(kullanan, ihaleKod, fiyat, isTur);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleIlanBilgisiKaydet(Kullanici kullanan, IhaleIlanBilgisi bilgi, int islemKod, int parcaKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCIhaleIlanBilgisi().Kaydet(kullanan, bilgi, islemKod, parcaKod);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale ilan bilgisini veritabanından okur 
        /// ve IhaleIlanBilgisi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale ilan bilgisini IhaleIlanBilgisi nesnesi olarak tutan nesne</returns>
        public IhaleIlanBilgisi IhaleIlanBilgisiListele(Kullanici kullanan, IhaleIlanBilgisi kriter)
        {
            IhaleIlanBilgisi donusDeger = new IhaleIlanBilgisi();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleIlanBilgisi().Listele(kullanan, kriter);
            }
            catch
            {
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisine ait belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgileri IhaleIlanBilgisiBelge nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleIlanBilgisiBelgeKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleIlanBilgisiBelge().Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisine ait belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="belge">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleIlanBilgisiBelgeKaydet(Kullanici kullanan, IhaleIlanBilgisiBelge belge, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleIlanBilgisiBelge().Kaydet(kullanan, belge, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisine ait belge bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="belge">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleIlanBilgisiBelgeSil(Kullanici kullanan, IhaleIlanBilgisiBelge belge, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleIlanBilgisiBelge().Sil(kullanan, belge, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisine ait belge bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgileri IhaleIlanBilgisiBelge nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleIlanBilgisiBelgeSil(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleIlanBilgisiBelge().Sil(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale ilan bilgisine ait belge bilgisini veritabanından okur 
        /// ve IhaleIlanBilgisiBelge nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale ilan bilgisine ait belge bilgisi listesini IhaleIlanBilgisiBelge nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IhaleIlanBilgisiBelgeListele(Kullanici kullanan, IhaleIlanBilgisiBelge kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleIlanBilgisiBelge().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisine ait belgenin sıra no bilgisini, belge listesinde bir üst sırada görünecek şekilde veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="belge">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleIlanBilgisiBelgeYukariTasi(Kullanici kullanan, IhaleIlanBilgisiBelge belge, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleIlanBilgisiBelge().YukariTasi(kullanan, belge, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisine ait belgenin sıra no bilgisini, belge listesinde bir alt sırada görünecek şekilde veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="belge">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleIlanBilgisiBelgeAsagiTasi(Kullanici kullanan, IhaleIlanBilgisiBelge belge, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleIlanBilgisiBelge().AsagiTasi(kullanan, belge, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, sayfa yardım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yardim">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc YardimKaydet(Kullanici kullanan, Yardim yardim)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCYardim().Kaydet(kullanan, yardim);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, sayfa yardım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="list">Kaydedilecek bilgileri Yardim nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc YardimGuncelleme(Kullanici kullanan, ObjectArray list)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCYardim().GuncellemeKaydet(kullanan, list);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sayfa yardım bilgisini veritabanından okur 
        /// ve Yardım nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yardim">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sayfa yardım bilgisini Yardim nesnesi olarak tutan nesne</returns>
        public Yardim YardimGoster(Kullanici kullanan, Yardim yardim)
        {
            Yardim donusDeger = new Yardim();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCYardim().Goster(kullanan, yardim);
            }
            catch
            {
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sayfa yardım bilgisini veritabanından okur 
        /// ve Yardim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yardim">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sayfa yardım listesini Yardim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray YardimListele(Kullanici kullanan, Yardim yardim)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCYardim().Listele(kullanan, yardim);
            }
            catch
            {
            }
            return donusDeger;
        }

        ///// <summary>
        ///// Arguman olarak verilen kriterlere bağlı olarak, tertibe ait bütçe gider bilgisini veritabanından okur 
        ///// ve ButceGider nesne koleksiyonu olarak döner
        ///// </summary>
        ///// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        ///// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu</param>
        ///// <returns>İstenilen kriterdeki, tertibe ait bütçe gider bilgisi listesini ButceGider nesne koleksiyonu olarak tutan nesne</returns>
        //public ObjectArray YuzdeOnRakamiVer(int yil, ObjectArray tertip)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new HRCButce().YuzdeOnRakamiVer(yil, tertip);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, tertip bilgisini veritabanından okur 
        /// ve ButceGider nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, tertip bilgi listesini ButceGider nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TertipListele(int yil, string kur, string fon, string fin, string eko)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCButce().TertipListele(yil, kur, fon, fin, eko);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, harcama bilgisini veritabanından okur 
        /// ve ButceHarcama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, harcama bilgi listesini ButceHarcama nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray HarcamaListele(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCButce().HarcamaListele(yil, ay, muhasebe, birim, kur, fon, fin, eko);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ödenek gönderme belge bilgisini veritabanından okur 
        /// ve ButceGider nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <param name="durum">İşlem yapılacak ödenek gönderme belgesinin durum bilgisi</param>
        /// <returns>İstenilen kriterdeki, ödenek gönderme belge bilgi listesini ButceGider nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray OdenekGondermeListele(int yil, int muhasebe, string birim, string kur, string fon, string fin, string eko, int durum)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCButce().OdenekGondermeListele(yil, muhasebe, birim, kur, fon, fin, eko, durum);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kullanılabilir ödenek bilgisini veritabanından okur 
        /// ve ButceGider nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="kontrolTur">İşlem yapılacak kontrol tür bilgisi (Ödenek Gönderme, Ödenek)</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, kullanılabilir ödenek listesini ButceGider nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray KullanilabilirOdenek(int yil, int kontrolTur, int muhasebe, string birim, string kur, string fon, string fin, string eko)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCButce().KullanilabilirOdenek(yil, kontrolTur, muhasebe, birim, kur, fon, fin, eko);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kurumsal kod bilgisini veritabanından okur 
        /// ve string olarak döner
        /// </summary>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <returns></returns>
        public string MuhasebeBirimKurumsalKodAl(int muhasebe, string birim)
        {
            string donusDeger = "";
            try
            {
                donusDeger = new HRCButce().MuhasebeBirimKurumsalKodAl(muhasebe, birim);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, muhasebe ve birim bilgisini veritabanından okur 
        /// ve MuhasebeBirim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <param name="kurKod">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, muhasebe ve birim bilgisi listesini MuhasebeBirim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray MuhasebeBirimListele(int muhasebe, string birim, string kurKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCButce().MuhasebeBirimListele(muhasebe, birim, kurKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, muhasebe ve birime ait tertip bilgisini veritabanından okur 
        /// ve MuhasebeBirimTertip nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, muhasebe ve birime ait tertip bilgisi listesini MuhasebeBirimTertip nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray MuhasebeBirimTertipListele(int muhasebe, string birim, string eko)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCButce().MuhasebeBirimTertipListele(muhasebe, birim, eko);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, muhasebe ve birime ait tertip bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tertip">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MuhasebeBirimTertipKaydet(Kullanici kullanan, MuhasebeBirimTertip tertip)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCButce().MuhasebeBirimTertipKaydet(kullanan, tertip);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, muhasebe ve birime ait tertip bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tertip">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MuhasebeBirimTertipSil(Kullanici kullanan, MuhasebeBirimTertip tertip)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCButce().MuhasebeBirimTertipSil(kullanan, tertip);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç için geçerli olan ekonomik kod bilgisini veritabanından okur 
        /// ve string olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <returns>İstenilen kriterdeki, süreç için geçerli olan ekonomik kod bilgisini string olarak döner</returns>
        public string SurecEkonomikKodAl(Kullanici kullanan, int yil, string surecNo)
        {
            string donusDeger = "";
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSurec().SurecEkonomikKodAl(kullanan, yil, surecNo);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, idare bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IdareBilgiKaydet(Kullanici kullanan, IdareBilgi bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCIdareBilgi().Kaydet(kullanan, bilgi);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, idare bilgisini veritabanından okur 
        /// ve IdareBilgi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, idare bilgisini IdareBilgi nesnesi olarak tutan nesne</returns>
        public IdareBilgi IdareBilgiListele(Kullanici kullanan, IdareBilgi kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCIdareBilgi().Listele(null, kullanan, kriter);
            }
            catch
            {
                return new IdareBilgi();
            }
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, idare bilgisini veritabanından okur 
        /// ve IdareBilgi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, idare bilgisini IdareBilgi nesnesi olarak tutan nesne</returns>
        public IdareBilgi KisiIdareBilgiBul(Kullanici kullanan)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCIdareBilgi().KisiIdareBilgiBul(null, kullanan);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Arguman olarak verilen, tek kaynaktan temin bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TekKaynaktanTeminKaydet(Kullanici kullanan, TekKaynaktanTemin kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTekKaynaktanTemin().Kaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, tek kaynaktan temin bilgisini veritabanından okur 
        /// ve TekKaynaktanTemin nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, tek kaynaktan temin bilgi listesini TekKaynaktanTemin nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TekKaynaktanTeminListele(Kullanici kullanan, TekKaynaktanTemin kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTekKaynaktanTemin().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleNumarasiKaydet(Kullanici kullanan, IhaleNumarasi kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleNumarasi().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale numarası bilgisini veritabanından okur 
        /// ve IhaleNumarasi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale numarası bilgisini IhaleNumarası nesnesi olarak tutan nesne</returns>
        public IhaleNumarasi IhaleNumarasiListele(Kullanici kullanan, IhaleNumarasi kriter)
        {
            IhaleNumarasi donusDeger = new IhaleNumarasi();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleNumarasi().Listele(kullanan, kriter);
            }
            catch
            {

            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, harcama talimatı bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HarcamaTalimatiKaydet(Kullanici kullanan, HarcamaTalimati kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCHarcamaTalimati().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, harcama talimatı bilgisini veritabanından okur 
        /// ve HarcamaTalimatı nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, harcama talimatı bilgisi listesini HarcamaTalimati nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray HarcamaTalimatiListele(Kullanici kullanan, HarcamaTalimati kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCHarcamaTalimati().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, harcama pusulası bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HarcamaPusulasiSil(Kullanici kullanan, FaturaForm bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFaturaForm().DetaySil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, hesap cetveli bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HesapCetveliKaydet(Kullanici kullanan, HesapCetveli liste, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCHesapCetveli().Kaydet(kullanan, liste, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hesap cetveli bilgisini veritabanından okur 
        /// ve HesapCetveli nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, hesap cetveli bilgisini HesapCetveli nesnesi olarak tutan nesne</returns>
        public HesapCetveli HesapCetveliListele(Kullanici kullanan, HesapCetveli kriter)
        {
            HesapCetveli donusDeger = new HesapCetveli();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCHesapCetveli().Listele(kullanan, kriter);
            }
            catch
            {

            }
            return donusDeger;
        }

        // ***
        /// <summary>
        /// Arguman olarak verilen, ihale dosyası içerik tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleDosyasiIcerikKaydet(Kullanici kullanan, ObjectArray kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleDosyasiIcerik().Kaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        // *** kendi içinde IhaleDosyasiIcerikKaydet fonksiyonunu çalıştırarak sonuç elde edebiliriz.
        /// <summary>
        /// Arguman olarak verilen, ihale dosyası içerik tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgileri IhaleDosyasiIcerik nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleDosyasiIcerikListeKaydet(Kullanici kullanan, ObjectArray kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleDosyasiIcerik().ListeKaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale dosyası içerik tanım bilgisini veritabanından okur 
        /// ve IhaleDosyasiIcerik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale dosyası içerik tanım bilgisi listesini IhaleDosyasiIcerik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IhaleDosyasiIcerikListele(Kullanici kullanan, IhaleDosyasiIcerik kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleDosyasiIcerik().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, sürece bağlı ihale dosyası içerik bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleDosyasiIcerikListeKaydet(Kullanici kullanan, IhaleDosyasiIcerikListe liste, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleDosyasiIcerikListe().Kaydet(kullanan, liste, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürece bağlı ihale dosyası içerik bilgisini veritabanından okur 
        /// ve IhaleDosyasiIcerikListe nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürece bağlı ihale dosyası içerik bilgisini IhaleDosyasiIcerikListe nesnesi olarak tutan nesne</returns>
        public IhaleDosyasiIcerikListe IhaleDosyasiIcerikListeAl(Kullanici kullanan, IhaleDosyasiIcerikListe kriter)
        {
            IhaleDosyasiIcerikListe donusDeger = new IhaleDosyasiIcerikListe();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleDosyasiIcerikListe().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.liste.sonuc.islemSonuc = false;
                donusDeger.liste.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale dosyası satın alma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgileri IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleDosyasiAlanKaydet(Kullanici kullanan, ObjectArray kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleDosyasiAlan().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale dosyası satın alma bilgisini veritabanından okur 
        /// ve IhaleTeklifDosyasi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale dosyası satın alma bilgisi listesini IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IhaleDosyasiAlanListele(Kullanici kullanan, IhaleTeklifDosyasi kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleDosyasiAlan().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, firmanın ihale devam edip etmeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleFirmaDevamGuncelle(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifDosyasiVeren().Guncelle(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale dosyası teslim etme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgileri IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeklifDosyasiVerenKaydet(Kullanici kullanan, ObjectArray kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifDosyasiVeren().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale dosyası teslim etme bilgisini veritabanından okur 
        /// ve IhaleTeklifDosyasi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale dosyası teslim etme bilgisi listesini IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TeklifDosyasiVerenListele(Kullanici kullanan, IhaleTeklifDosyasi kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifDosyasiVeren().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihaleye devem eden firma bilgisini veritabanından okur 
        /// ve IhaleTeklifDosyasi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihaleye devam eden firma bilgisi listesini IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray DevamEdenFirmaListele(Kullanici kullanan, IhaleTeklifDosyasi kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifDosyasiVeren().ListeleDevamEdenFirma(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale teklif dosyası komisyon tutanağı bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeklifDosyasiKomisyonTutanagiKaydet(Kullanici kullanan, TeklifDosyasiKomisyonTutanagi kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifDosyasiKomisyonTutanagi().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale teklif dosyası komisyon tutanağı bilgisini veritabanından okur 
        /// ve TeklifDosyasiKomisyonTutanagi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale teklif dosyası komisyon tutanağı bilgisini TeklifDosyasiKomisyonTutanagi nesnesi olarak tutan nesne</returns>
        public TeklifDosyasiKomisyonTutanagi TeklifDosyasiKomisyonTutanagiListele(Kullanici kullanan, TeklifDosyasiKomisyonTutanagi kriter)
        {
            TeklifDosyasiKomisyonTutanagi donusDeger = new TeklifDosyasiKomisyonTutanagi();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifDosyasiKomisyonTutanagi().Listele(kullanan, kriter);
            }
            catch
            {

            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale teklif dosyası içerik tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri TeklifDosyasiIcerik nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeklifDosyasiIcerikKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifDosyasiIcerik().Kaydet(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale teklif dosyası içerik tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// Güncelleme servisinde kullanılır.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri TeklifDosyasiIcerik nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeklifDosyasiIcerikListeKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifDosyasiIcerik().ListeKaydet(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale teklif dosyası içerik bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeklifDosyasiIcerikListeKaydet(Kullanici kullanan, TeklifDosyasiIcerikListe liste, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifDosyasiIcerikListe().Kaydet(kullanan, liste, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale teklif dosyası içerik bilgisini veritabanından okur 
        /// ve TeklifDosyasiIcerik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale teklif dosyası içerik bilgisi listesini TeklifDosyasiIcerik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TeklifDosyasiIcerikListele(Kullanici kullanan, TeklifDosyasiIcerik kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifDosyasiIcerik().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale teklif dosyası içerik bilgisini veritabanından okur 
        /// ve TeklifDosyasiIcerikListe nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale teklif dosyası içerik bilgisini TeklifDosyasiIcerikListe nesnesi olarak tutan nesne</returns>
        public TeklifDosyasiIcerikListe TeklifDosyasiIcerikListeAl(Kullanici kullanan, TeklifDosyasiIcerikListe kriter)
        {
            TeklifDosyasiIcerikListe donusDeger = new TeklifDosyasiIcerikListe();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifDosyasiIcerikListe().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.liste.sonuc.islemSonuc = false;
                donusDeger.liste.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale teklif zarfı biçim tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// Güncelleme servisinde kullanılır.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgileri TeklifZarfBicimi nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeklifZarfBicimiListeKaydet(Kullanici kullanan, ObjectArray bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifZarfBicimi().ListeKaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale teklif zarfı biçim tanım bilgisini veritabanından okur 
        /// ve TeklifZarfBicimi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale teklif zarfı biçim tanım bilgisi listesini TeklifZarfBicimi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TeklifZarfBicimiListele(Kullanici kullanan, TeklifZarfBicimi kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifZarfBicimi().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale teklif zarfı biçim tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeklifZarfBicimiKaydet(Kullanici kullanan, TeklifZarfBicimi bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifZarfBicimi().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale teklif zarfı biçim tanım bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeklifZarfBicimiSil(Kullanici kullanan, TeklifZarfBicimi bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifZarfBicimi().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, teklif veren firmaların, teklif zarf kontrol bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeklifZarfKontroluKaydet(Kullanici kullanan, TeklifZarfKontroluBaslik bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifZarfKontrolu().Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, teklif veren firmanın teklif zarfı kontrol bilgisini veritabanından okur 
        /// ve TeklifZarfKontroluBaslik nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, teklif zarf kontrol bilgisini TeklifZarfKontroluBaslik nesnesi olarak tutan nesne</returns>
        public TeklifZarfKontroluBaslik TeklifZarfKontroluListele(Kullanici kullanan, TeklifZarfKontroluBaslik kriter)
        {
            TeklifZarfKontroluBaslik donusDeger = new TeklifZarfKontroluBaslik();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifZarfKontrolu().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.satirlar.sonuc.islemSonuc = false;
                donusDeger.satirlar.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, teklif veren firmanın teklif zarfındaki eksik olan belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgileri TeklifDosyasiEksikBelge nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeklifDosyasiEksikBelgeKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifDosyasiEksikBelge().Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, teklif veren firmanın teklif zarfındaki eksik olan belge bilgisini veritabanından okur 
        /// ve TeklifDosyasiEksikBelge nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, teklif veren firmanın teklif zarfındaki eksik olan belge bilgisi listesini TeklifDosyasiEksikBelge nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TeklifDosyasiEksikBelgeListele(Kullanici kullanan, TeklifDosyasiEksikBelge kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifDosyasiEksikBelge().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, teklif veren firmanın teklif zarfındaki uygun olmayan belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgileri TeklifUygunOlmayanBelge nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeklifUygunOlmayanBelgeKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifUygunOlmayanBelge().Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, teklif veren firmanın teklif zarfındaki uygun olmayan belge bilgisini veritabanından okur 
        /// ve TeklifUygunOlmayanBelge nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, teklif veren firmanın teklif zarfındaki uygun olmayan belge bilgisi listesini TeklifUygunOlmayanBelge nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TeklifUygunOlmayanBelgeListele(Kullanici kullanan, TeklifUygunOlmayanBelge kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifUygunOlmayanBelge().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale için firma tarafından teklif edilen fiyat bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgileri TeklifEdilenFiyat nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TeklifEdilenFiyatKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTeklifEdilenFiyat().Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale için firma tarafından teklif edilen fiyat bilgisini veritabanından okur 
        /// ve TeklifEdilenFiyat nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale için firma tarafından teklif edilen fiyat bilgisi listesini TeklifEdilenFiyat nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TeklifEdilenFiyatListele(Kullanici kullanan, TeklifEdilenFiyat kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCTeklifEdilenFiyat().Listele(kullanan, kriter);
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
        /// Arguman olarak verilen, ihaleyi kazanan firma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KazananFirmaKaydet(Kullanici kullanan, KazananFirma bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCKazananFirma().Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihaleyi kazanan firma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Güncellenecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KazananFirmaGuncelle(Kullanici kullanan, KazananFirma bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCKazananFirma().Guncelle(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihaleyi kazanan firma bilgisini veritabanından okur 
        /// ve KazananFirma nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihaleyi kazanan firma bilgisini KazananFirma nesnesi olarak tutan nesne</returns>
        public KazananFirma KazananFirmaListele(Kullanici kullanan, KazananFirma kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCKazananFirma().Listele(kullanan, kriter);
            }
            catch
            {
                KazananFirma donusDeger = new KazananFirma();
                return donusDeger;
            }
        }

        /// <summary>
        /// Arguman olarak verilen, ihale kararı bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleKarariKaydet(Kullanici kullanan, IhaleKarari bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleKarari().Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale kararı bilgisini veritabanından okur 
        /// ve IhaleKarari nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale kararı bilgisini IhaleKarari nesnesi olarak tutan nesne</returns>
        public IhaleKarari IhaleKarariListele(Kullanici kullanan, IhaleKarari kriter)
        {
            IhaleKarari donusDeger = new IhaleKarari();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleKarari().Listele(kullanan, kriter);
            }
            catch
            {

            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale firma sonucu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgileri IhaleFirmaSonucu nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleFirmaSonucuKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleFirmaSonucu().Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale firma sonucu bilgisini veritabanından okur 
        /// ve IhaleFirmaSonucu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale firma sonucu listesini IhaleFirmaSonucu nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IhaleFirmaSonucuListele(Kullanici kullanan, IhaleFirmaSonucu kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleFirmaSonucu().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kısmi teklif verilebilen ihale için ihale firma sonucu bilgisini veritabanından okur 
        /// ve IhaleFirmaSonucu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, kısmi teklif verilebilen ihale için ihale firma sonucu bilgisi listesini IhaleFirmaSonucu nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IhaleFirmaSonucuListeleKismi(Kullanici kullanan, IhaleFirmaSonucu kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleFirmaSonucu().ListeleKismiTeklif(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihaleyi kazanan firmalar için düzenelenen sözleşmeye davet bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgileri IhaleSozlesmeyeDavet nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleSozlesmeyeDavetKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleSozlesmeyeDavet().Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihaleyi kazanan firmalar için düzenelenen sözleşmeye davet bilgisini veritabanından okur 
        /// ve IhaleSozlesmeyeDavet nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihaleyi kazanan firmalar için düzenelenen sözleşmeye davet bilgisi listesini IhaleSozlesmeyeDavet nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IhaleSozlesmeyeDavetListele(Kullanici kullanan, IhaleSozlesmeyeDavet kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleSozlesmeyeDavet().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, sözleşme sonucunda ödnemesi gereken ücret bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleSozlesmeSonuUcretKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleSozlesmeSonuUcret().Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sözleşme sonucunda ödnemesi gereken ücret bilgisini veritabanından okur 
        /// ve IhaleSozlesmeSonuUcret nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sözleşme sonucunda ödnemesi gereken ücret bilgisi listesini IhaleSozlesmeSonuUcret nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IhaleSozlesmeSonuUcretListele(Kullanici kullanan, IhaleSozlesmeSonuUcret kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleSozlesmeSonuUcret().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihaleye firma daveti bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgileri FirmaTeklifDaveti nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleFirmaTeklifDavetiKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleFirmaTeklifDaveti().Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihaleye firma daveti bilgisini veritabanından okur 
        /// ve FirmaTeklifDaveti nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihaleye firma daveti bilgisi listesini FirmaTeklifDaveti nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IhaleFirmaTeklifDavetiListele(Kullanici kullanan, FirmaTeklifDaveti kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleFirmaTeklifDaveti().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale sonucu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IhaleSonucuKaydet(Kullanici kullanan, IhaleSonucu bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleSonucu().Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale sonucu bilgisini veritabanından okur 
        /// ve IhaleSonucu nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale sonucu bilgisini IhaleSonucu nesnesi olarak tutan nesne</returns>
        public IhaleSonucu IhaleSonucuListele(Kullanici kullanan, IhaleSonucu kriter)
        {
            IhaleSonucu donusDeger = new IhaleSonucu();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleSonucu().Listele(kullanan, kriter);
            }
            catch
            {

            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, hakediş bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HakedisKaydet(Kullanici kullanan, Hakedis kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCHakedis().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hakediş bilgisini veritabanından okur 
        /// ve Hakedis nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, hakediş bilgisini Hakedis nesnesi olarak tutan nesne</returns>
        public Hakedis HakedisListele(Kullanici kullanan, Hakedis kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCHakedis().Listele(kullanan, kriter);
            }
            catch
            {
                return new Hakedis();
            }
        }

        /// <summary>
        /// Arguman olarak verilen, ihalenin teklif değerlendirme işlemleri bittikten sonra girilen hakediş ön bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HakedisOnBilgiKaydet(Kullanici kullanan, Hakedis kriter, int islemKod, int parcaKod, TNSCollection tertipler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCHakedis().KaydetOnBilgi(kullanan, kriter, islemKod, parcaKod, tertipler);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, hakediş ön bilgisini veritabanından okur 
        /// ve Hakedis nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, hakediş ön bilgisini Hakedis nesnesi olarak tutan nesne</returns>
        public Hakedis HakedisListeleOnBilgi(Kullanici kullanan, Hakedis kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCHakedis().Listele(kullanan, kriter);
            }
            catch
            {
                return new Hakedis();
            }
        }

        /// <summary>
        /// Arguman olarak verilen, hakediş ödeme planına fatura numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <param name="hakedisSiraNo">İşlem yapılacak hakediş sıra numarası bilgisi</param>
        /// <param name="faturaNo">İşlem yapılacak fatura numarası bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HakedisFaturaNoKaydet(Kullanici kullanan, int yil, string surecNo, int hakedisSiraNo, int faturaNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCHakedis().FaturaNoKaydet(kullanan, yil, surecNo, hakedisSiraNo, faturaNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, hakediş ödeme planı bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc HakedisOdemePlaniSil(Kullanici kullanan, HakedisOdemePlan kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCHakedis().OdemePlaniSil(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, sözleşmeli maaş bordro bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgileri PSLSozlesmeliMaasBordro nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc PSLSozlesmeliMaasBordroKaydet(Kullanici kullanan, ObjectArray kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSozlesmeliMaasBordro().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, sözleşmeli maaş bordro bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc PSLSozlesmeliMaasBordroSil(Kullanici kullanan, PSLSozlesmeliMaasBordro kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSozlesmeliMaasBordro().Sil(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sözleşmeli maaş bordro bilgisini veritabanından okur 
        /// ve PSLSozlesmeliMaasBordro nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sözleşmeli maaş bordro bilgisi listesini PSLSozlesmeliMaasBordro nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray PSLSozlesmeliMaasBordroListele(Kullanici kullanan, TNS.HRC.PSLSozlesmeliMaasBordro kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSozlesmeliMaasBordro().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sözleşmeli personel asgari geçim bordro bilgisini veritabanından okur 
        /// ve PSLSozlesmeliMaasBordro nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sözleşmeli personel asgari geçim bordro bilgisi listesini PSLSozlesmeliMaasBordro nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray PSLSozlesmeliAsgariGecimBordroListele(Kullanici kullanan, TNS.HRC.PSLSozlesmeliMaasBordro kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSozlesmeliMaasBordro().ListeleAsgariGecimBordro(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, sözleşmeli maaş ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bordro">Kaydederken kullanılacak kriter bilgisini tutan nesne</param>
        /// <param name="belgeNo">Kaydedilecek ödeme emri belge numarası bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc PSLSozlesmeliMaasMIFNoKaydet(Kullanici kullanan, PSLSozlesmeliMaasBordro bordro, string belgeNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSozlesmeliMaasBordro().MIFNoKaydet(kullanan, bordro, belgeNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sözleşme maaş ödeme emri belge numarası bilgisini veritabanından okur 
        /// ve Sonuc nesnesi olarak döner.
        /// Ödeme Emri Belge Numarası bilgisi Sonuc nesnesinin anahtar özelliğinde bulunmaktadır.
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc PSLSozlesmeliMaasMIFNoListele(Kullanici kullanan, PSLSozlesmeliMaasBordro kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSozlesmeliMaasBordro().MIFNoListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sözleşmeli maaş bordrosu için fark bordrosu hazırlanıp hazırlanmadığı bilgisini veritabanından okur 
        /// ve boolean olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sözleşmeli maaş bordrosu için fark bordrosu hazırlanıp hazırlanmadığı bilgisini boolean olarak döner</returns>
        public bool PSLSozlesmeliFarkBordrosuVarMi(Kullanici kullanan, TNS.HRC.PSLSozlesmeliMaasBordro kriter)
        {
            bool donusDeger = false;
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSozlesmeliMaasBordro().FarkBordrosuVarMi(kullanan, kriter);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, öğrenci maaş bordro bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgileri PSLOgrenciMaasBordro nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc PSLOgrenciMaasBordroKaydet(Kullanici kullanan, ObjectArray kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOgrenciMaasBordro().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, öğrenci maaş bordro bilgisini veritabanından okur 
        /// ve PSLOgrenciMaasBordro nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, öğrenci maaş bordro listesini PSLOgrenciMaasBordro nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray PSLOgrenciMaasBordroListele(Kullanici kullanan, PSLOgrenciMaasBordro kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOgrenciMaasBordro().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, öğrenci maaş ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="kriter">Kaydederken kullanılacak kriter bilgisini tutan nesne</param>
        /// <param name="belgeNo">Kaydedilecek ödeme emri belge numarası bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc PSLOgrenciMaasMIFNoKaydet(Kullanici kullanan, PSLOgrenciMaasBordro kriter, string belgeNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOgrenciMaasBordro().MIFNoKaydet(kullanan, kriter, belgeNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, öğrenci maaş ödeme emri belge numarası bilgisini veritabanından okur 
        /// ve Sonuc nesnesi olarak döner.
        /// Ödeme Emri Belge Numarası bilgisi Sonuc nesnesinin anahtar özelliğinde bulunmaktadır.
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns> 
        public Sonuc PSLOgrenciMaasMIFNoListele(Kullanici kullanan, PSLOgrenciMaasBordro kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOgrenciMaasBordro().MIFNoListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, maaş temel tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgileri PSLMaasTemelBilgisi nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns> 
        public Sonuc PSLMaasTemelBilgisiKaydet(Kullanici kullanan, ObjectArray kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCMaasTemelBilgisi().Kaydet(kullanan, kriter);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, maaş temel tanım bilgisini veritabanından okur 
        /// ve PSLMaasTemelBilgisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, maaş temel tanım bilgisi listesini PSLMaasTemelBilgisi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray PSLMaasTemelBilgisiListele(Kullanici kullanan, TNS.HRC.PSLMaasTemelBilgisi kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCMaasTemelBilgisi().Listele(kullanan, kriter);
            }
            catch (Exception ex)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(ex.Message);
                return donusDeger;
            }
        }

        /// <summary>
        /// Arguman olarak verilen, personel hastalık izni bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgileri PSLHastalikIzni nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc PSLHastalikIzniKaydet(Kullanici kullanan, ObjectArray kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCHastalikIzni().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, personel hastalık izni bilgisini veritabanından okur 
        /// ve PSLHastalikIzni nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, personel hastalık izni listesini PSLHastalikIzni nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray PSLHastalikIzniListele(Kullanici kullanan, TNS.HRC.PSLHastalikIzni kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCHastalikIzni().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, birime gelen birimden giden personel bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgileri PSLGelenGidenPersonel nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc PSLGelenGidenPersonelKaydet(Kullanici kullanan, ObjectArray kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCGelenGidenPersonel().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, birime gelen birimden giden personel bilgisini veritabanından okur 
        /// ve PSLGelenGidenPersonel nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, birime gelen birimden giden personel bilgisi listesini PSLGelenGidenPersonel nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray PSLGelenGidenPersonelListele(Kullanici kullanan, TNS.HRC.PSLGelenGidenPersonel kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCGelenGidenPersonel().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, gelir vergisi tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgileri PSLGelirVergisi nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc PSLGelirVergisiKaydet(Kullanici kullanan, ObjectArray kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCGelirVergisi().Kaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, gelir vergisi tanım bilgisini veritabanından okur 
        /// ve PSLGelirVergisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, gelir vergisi tanım listesini PSLGelirVergisi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray PSLGelirVergisiListele(Kullanici kullanan, TNS.HRC.PSLGelirVergisi kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCGelirVergisi().Listele(kullanan, kriter);
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
        /// Arguman olarak verilen, SSK Hizmet Belgesi bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc PSLSSKHizmetBelgesiKaydet(Kullanici kullanan, PSLSSKHizmetBelgesi kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSSKHizmetBelgesi().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, SSK Hizmet Belgesi bilgisini veritabanından okur 
        /// ve PSLSSKHizmetBelgesi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, SSK Hizmet Belgesi bilgisini PSLSSKHizmetBelgesi nesnesi olarak tutan nesne</returns>
        public PSLSSKHizmetBelgesi PSLSSKHizmetBelgesiListele(Kullanici kullanan, PSLSSKHizmetBelgesi kriter)
        {
            PSLSSKHizmetBelgesi donusDeger = new PSLSSKHizmetBelgesi();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSSKHizmetBelgesi().Listele(kullanan, kriter);
            }
            catch
            {

            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, personel sosyal yardım belgesi bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgileri PSLSosyalYardimi nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc PSLSosyalYardimiKaydet(Kullanici kullanan, ObjectArray kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSosyalYardim().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, personel sosyal yardım belgesine ait ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc PSLSosyalYardimiOdemeEmriNoKaydet(Kullanici kullanan, PSLSosyalYardimi kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSosyalYardim().OdemeEmriNoKaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, personel sosyal yardım belgesi bilgisini veritabanından okur 
        /// ve PSLSosyalYardimi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, personel sosyal yardım belgesi bilgisi listesini PSLSosyalYardimi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray PSLSosyalYardimiListele(Kullanici kullanan, TNS.HRC.PSLSosyalYardimi kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSosyalYardim().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, sosyal yardım tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgileri PSLSosyalYardimTemelBilgisi nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc PSLSosYardTemelBilgisiKaydet(Kullanici kullanan, ObjectArray kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSosYardTemelBilgisi().Kaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sosyal yardım tanım bilgisini veritabanından okur 
        /// ve PSLSosyalYardimTemelBilgisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sosyal yardım tanım bilgisi listesini PSLSosyalYardimTemelBilgisi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray PSLSosYardTemelBilgisiListele(Kullanici kullanan, TNS.HRC.PSLSosyalYardimTemelBilgisi kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSosYardTemelBilgisi().Listele(kullanan, kriter);
            }
            catch
            {

            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, aile yardım bildirim belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc PSLAileYardimBildirimiKaydet(Kullanici kullanan, PSLAileYardimBildirimiBaslik kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAileYardimBildirimi().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, aile yardım bildirim belge bilgisini veritabanından okur 
        /// ve PSLAileYardimBildirimiBaslik nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, aile yardım bildirim belge bilgisini PSLAileYardimBildirimiBaslik nesnesi olarak tutan nesne</returns>
        public PSLAileYardimBildirimiBaslik PSLAileYardimBildirimiListele(Kullanici kullanan, PSLAileYardimBildirimiBaslik kriter)
        {
            PSLAileYardimBildirimiBaslik donusDeger = new PSLAileYardimBildirimiBaslik();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAileYardimBildirimi().Listele(kullanan, kriter);
            }
            catch
            {

            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale takvim bilgisini veritabanından okur 
        /// ve IhaleTakvim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale takvim bilgisi listesini IhaleTakvim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IhaleTakvimListele(Kullanici kullanan, TNS.HRC.Surec kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleTakvimi().Listele(kullanan, kriter);
            }
            catch
            {

            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, sözleşmeli personel bordro tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgileri SozlesmeliBordroTanim nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc PSLSozlesmeliBordroTanimKaydet(Kullanici kullanan, ObjectArray kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSozlesmeliMaasBordroTanim().Kaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sözleşmeli personel bordro hesaplamasında kullanılacak tanım bilgisini veritabanından okur 
        /// ve SozlesmeliBordroTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sözleşmeli personel bordro hesaplamasında kullanılacak tanım bilgisi listesini SozlesmeliBordroTanim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray PSLSozlesmeliBordroTanimListele(Kullanici kullanan, TNS.HRC.SozlesmeliBordroTanim kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSozlesmeliMaasBordroTanim().Listele(kullanan, kriter);
            }
            catch
            {

            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, işçi maaş bordro hesaplamasında kullanılacak tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgileri IsciBordroTanim nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IsciBordroTanimKaydet(Kullanici kullanan, ObjectArray kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIsciMaasBordroTanim().Kaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, işçi maaş bordro hesaplamasında kullanılacak tanım bilgisini veritabanından okur 
        /// ve IsciBordroTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, işçi maaş bordro hesaplamasında kullanılacak tanım bilgisi listesini IsciBordroTanim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IsciBordroTanimListele(Kullanici kullanan, IsciBordroTanim kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIsciMaasBordroTanim().Listele(kullanan, kriter);
            }
            catch
            {

            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ek ders ücret hesaplamasında kullanılacak puantaj tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc EkDersPuantajKaydet(Kullanici kullanan, EkDersPuantaj bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCEkDersPuantaj().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ek ders ücret hesaplamasında kullanılacak puantaj tanım bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc EkDersPuantajSil(Kullanici kullanan, EkDersPuantaj bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCEkDersPuantaj().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders ücret hesaplamasında kullanılacak puantaj tanım bilgisini veritabanından okur 
        /// ve EkDersPuantajlar nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ek ders ücret hesaplamasında kullanılacak puantaj tanım bilgisini EkDersPuantajlar nesnesi olarak tutan nesne</returns>
        public EkDersPuantajlar EkDersPuantajListele(Kullanici kullanan, EkDersPuantaj kriter)
        {
            EkDersPuantajlar donusDeger = new EkDersPuantajlar();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCEkDersPuantaj().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders ücret hesaplamasında kullanılacak puantaj tanımının açıklama bilgisini veritabanından okur 
        /// ve EkDersPuantajlar nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ek ders ücret hesaplamasında kullanılacak puantaj tanımının açıklama bilgisini EkDersPuantajlar nesnesi olarak tutan nesne</returns>
        public EkDersPuantajlar EkDersPuantajAciklamaListele(Kullanici kullanan, EkDersPuantaj kriter)
        {
            EkDersPuantajlar donusDeger = new EkDersPuantajlar();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCEkDersPuantaj().ListeleAciklama(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ek ders bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc EkDersKaydet(Kullanici kullanan, EkDers bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCEkDers().Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ek ders bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc EkDersSil(Kullanici kullanan, EkDers bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCEkDers().Sil(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ek ders bilgisi için gerekli hesaplamaları yapar, ek ders bordro bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Bordro hazırlananacak bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc EkDersBordroHazirla(Kullanici kullanan, EkDers bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCEkDers().BordroHazirla(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ek ders bordro bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Bordro hazırlananacak bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc EkDersBordroSil(Kullanici kullanan, EkDersBordro bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCEkDersBordro().Sil(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ek ders bordro kesinti bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Bordro hazırlananacak bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc EkDersBordroKesintiKaydet(Kullanici kullanan, EkDersBordrolar bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCEkDersBordro().KesintiKaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ek ders bordro ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Ödeme emri belge numarasının kaydedileceği belge bilgisini tutan nesne</param>
        /// <param name="belgeNo">Kaydedilecek ödeme emri belge numarası bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc EkDersMIFNoKaydet(Kullanici kullanan, EkDersBordro bilgi, string belgeNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCEkDers().MIFNoKaydet(kullanan, bilgi, belgeNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders bordro ödeme emri belge numarası bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// Ödeme emri belge numarası Sonuc nesnesinin anahtar özelliğinde bulunur
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc EkDersMIFNoListele(Kullanici kullanan, EkDersBordro kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCEkDers().MIFNoListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders düzenlenen personel bilgisini veritabanından okur 
        /// ve EkDers nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ek ders düzenlenen personel bilgisini EkDers nesnesi olarak tutan nesne</returns>
        public EkDers EkDersPersonelListele(Kullanici kullanan, EkDers kriter)
        {
            EkDers donusDeger = new EkDers();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCEkDersPersonel().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ek ders düzenelenen personel bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc EkDersPersonelKaydet(Kullanici kullanan, EkDers bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCEkDersPersonel().Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ek ders düzenelenen personel bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc EkDersPersonelSil(Kullanici kullanan, EkDers bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCEkDersPersonel().Sil(null, kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders bilgisini veritabanından okur 
        /// ve EkDers nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The kullanici.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ek ders bilgisini EkDers nesnesi olarak tutan nesne</returns>
        public EkDers EkDersListele(Kullanici kullanan, EkDers kriter)
        {
            EkDers donusDeger = new EkDers();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCEkDers().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders bordro bilgisini veritabanından okur 
        /// ve EkDersBordrolar nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="bankaListesi">Eğer <c>true</c> verilirse banka bilgilerini de getirir</param>
        /// <returns>İstenilen kriterdeki, ek ders bordro bilgisini EkDersBordrolar nesnesi olarak tutan nesne</returns>
        public EkDersBordrolar EkDersBordroListele(Kullanici kullanan, EkDersBordro kriter, bool bankaListesi)
        {
            EkDersBordrolar donusDeger = new EkDersBordrolar();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCEkDersBordro().Listele(kullanan, kriter, bankaListesi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders bilgisini veritabanından okur 
        /// ve EkDers nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ek ders listesini EkDers nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray EkDersSorgula(Kullanici kullanan, EkDers kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCEkDers().Sorgula(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai ücret hesaplamasında kullanılacak puantaj tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FazlaMesaiPuantajKaydet(Kullanici kullanan, FazlaMesaiPuantaj bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesaiPuantaj().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai ücret hesaplamasında kullanılacak puantaj tanım bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FazlaMesaiPuantajSil(Kullanici kullanan, FazlaMesaiPuantaj bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesaiPuantaj().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai ücret hesaplamasında kullanılacak puantaj tanım bilgisini veritabanından okur 
        /// ve FazlaMesaiPuantajlar nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, fazla mesai ücret hesaplamasında kullanılacak puantaj tanım bilgisini FazlaMesaiPuantaj nesnesi olarak tutan nesne</returns>
        public FazlaMesaiPuantajlar FazlaMesaiPuantajListele(Kullanici kullanan, FazlaMesaiPuantaj kriter)
        {
            FazlaMesaiPuantajlar donusDeger = new FazlaMesaiPuantajlar();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesaiPuantaj().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FazlaMesaiKaydet(Kullanici kullanan, FazlaMesai bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesai().Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FazlaMesaiSil(Kullanici kullanan, FazlaMesai bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesai().Sil(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai bilgisi için gerekli hesaplamaları yapar, fazla mesai bordro bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FazlaMesaiBordroHazirla(Kullanici kullanan, FazlaMesai bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesai().BordroHazirla(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai bordro bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FazlaMesaiBordroSil(Kullanici kullanan, FazlaMesaiBordro bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesaiBordro().Sil(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai bordro kesinti bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FazlaMesaiBordroKesintiKaydet(Kullanici kullanan, FazlaMesaiBordrolar bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesaiBordro().KesintiKaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai bordro ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Ödeme emri belge numarasının kaydedileceği belge bilgisini tutan nesne</param>
        /// <param name="belgeNo">Kaydedilecek ödeme emri belge numarası bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FazlaMesaiMIFNoKaydet(Kullanici kullanan, FazlaMesaiBordro bilgi, string belgeNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesai().MIFNoKaydet(kullanan, bilgi, belgeNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai bordro ödeme emri belge numarası bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// Ödeme emri belge numarası Sonuc nesnesinin anahtar özelliğinde bulunur
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FazlaMesaiMIFNoListele(Kullanici kullanan, FazlaMesaiBordro kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesai().MIFNoListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai düzenlenen personel bilgisini veritabanından okur 
        /// ve FazlaMesai nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, fazla mesai düzenlenen personel bilgisini FazlaMesai nesnesi olarak tutan nesne</returns>
        public FazlaMesai FazlaMesaiPersonelListele(Kullanici kullanan, FazlaMesai kriter)
        {
            FazlaMesai donusDeger = new FazlaMesai();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesaiPersonel().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, fazla mesai düzenelenen personel bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FazlaMesaiPersonelKaydet(Kullanici kullanan, FazlaMesai bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesaiPersonel().Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        // KmL verierişim buradan kaldırılmalı, null parametre geçilmiş
        /// <summary>
        /// Arguman olarak verilen, fazla mesai düzenelenen personel bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FazlaMesaiPersonelSil(Kullanici kullanan, FazlaMesai bilgi, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesaiPersonel().Sil(null, kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai bilgisini veritabanından okur 
        /// ve FazlaMesai nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The kullanici.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, fazla mesai bilgisini FazlaMesai nesnesi olarak tutan nesne</returns>
        public FazlaMesai FazlaMesaiListele(Kullanici kullanan, FazlaMesai kriter)
        {
            FazlaMesai donusDeger = new FazlaMesai();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesai().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai bordro bilgisini veritabanından okur 
        /// ve FazlaMesaiBordrolar nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="bankaListesi">Eğer <c>true</c> verilirse banka bilgilerini de getirir</param>
        /// <returns>İstenilen kriterdeki, fazla mesai bordro bilgisini FazlaMesaiBordrolar nesnesi olarak tutan nesne</returns>
        public FazlaMesaiBordrolar FazlaMesaiBordroListele(Kullanici kullanan, FazlaMesaiBordro kriter, bool bankaListesi)
        {
            FazlaMesaiBordrolar donusDeger = new FazlaMesaiBordrolar();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesaiBordro().Listele(kullanan, kriter, bankaListesi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai bilgisini veritabanından okur 
        /// ve FazlaMesai nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, fazla mesai bilgisi listesini FazlaMesai nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray FazlaMesaiSorgula(Kullanici kullanan, FazlaMesai kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesai().Sorgula(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray FazlaMesaiPuantajUnvanListele(Kullanici kullanan)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesaiPuantajUnvan().Listele(kullanan);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc FazlaMesaiPuantajUnvanKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesaiPuantajUnvan().Kaydet(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray FazlaMesaiBordroUnvanListele(Kullanici kullanan, FazlaMesaiBordroUnvan kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesaiBordroUnvan().Listele(kullanan, kriter, false);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc FazlaMesaiBordroUnvanKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesaiBordroUnvan().Kaydet(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }



        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, personel bilgisini veritabanından okur 
        /// ve ObjectArray nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, personel bilgisini ObjectArray nesnesi olarak tutan nesne</returns>
        public ObjectArray PersonelListele(Kullanici kullanan, int muhasebe, string birim)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCFazlaMesaiPersonel().ListelePersonel(kullanan, muhasebe, birim);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, personel sayısı bilgisini veritabanından okur 
        /// ve PersonelSayisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, personel sayısı bilgisi listesini PersonelSayisi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray PersonelSayisi(int muhasebe, string birim)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCPersonel().PersonelSayisi(muhasebe, birim);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        // KmL verierişim buradan kaldırılmalı, null parametre geçilmiş
        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="liste">Kaydedilecek bilgileri YollukGundelikTanim nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc GeciciGundelikTanimListeKaydet(Kullanici kullanan, int yil, ENUMYollukGundelikTuru tur, ObjectArray liste)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return (new PSLSurekliGorevGundelikTanim()).ListeKaydet(null, yil, tur, kullanan, liste);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        // KmL verierişim buradan kaldırılmalı, null parametre geçilmiş
        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc GeciciGundelikTanimKaydet(Kullanici kullanan, YollukGundelikTanim liste, bool zorunluYeni)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return (new PSLSurekliGorevGundelikTanim()).Kaydet(null, kullanan, liste, zorunluYeni);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk tanım bilgisini veritabanından okur 
        /// ve YollukGundelikTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk tanım bilgisi listesini YollukGundelikTanim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray GeciciGundelikTanimListele(Kullanici kullanan, YollukGundelikTanim kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return (new PSLSurekliGorevGundelikTanim()).Listele(null, kriter);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk tanım bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="gt">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc GeciciGundelikTanimSil(Kullanici kullanan, YollukGundelikTanim gt)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = (new PSLSurekliGorevGundelikTanim()).Sil(null, kullanan, gt);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, sürekli görev yolluk belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="sb">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SurekliGorevYollukBelgeKaydet(Kullanici kullanan, SurekliGorevYollukBelge sb, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = (new PSLSurekliGorevYolluk()).Kaydet(kullanan, sb, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, sürekli görev yolluk belgesini, sürekli görev yolluk belge havuzundan siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="sb">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SurekliGorevYollukHavuzSil(Kullanici kullanan, SurekliGorevYollukBelge sb)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = (new PSLSurekliGorevYolluk()).HavuzSil(kullanan, sb);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk belge bilgisini veritabanından okur 
        /// ve SurekliGorevYollukBelge nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk belge bilgisini SurekliGorevYollukBelge nesnesi olarak tutan nesne</returns>
        public SurekliGorevYollukBelge SurekliGorevYollukBelgeListele(Kullanici kullanan, SurekliGorevYollukBelge kriter)
        {
            SurekliGorevYollukBelge donusDeger = new SurekliGorevYollukBelge();

            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = (new PSLSurekliGorevYolluk()).Listele(kullanan, kriter);
            }
            catch
            {

            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk belge bilgisini veritabanından okur 
        /// ve SurekliGorevYollukBelge nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk belge bilgisini SurekliGorevYollukBelge nesnesi olarak tutan nesne</returns>
        public SurekliGorevYollukBelge OncedenKayitliMi(Kullanici kullanan, SurekliGorevYollukBelge kriter)
        {
            SurekliGorevYollukBelge donusDeger = new SurekliGorevYollukBelge();

            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = (new PSLSurekliGorevYolluk()).OncedenKayitliMi(kullanan, kriter);
            }
            catch
            {

            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="sb">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc GeciciGorevYollukBelgeKaydet(Kullanici kullanan, GeciciGorevYollukBelge sb, int islemKod, int parcaKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return (new PSLGeciciGorevYolluk()).Kaydet(null, kullanan, sb, islemKod, parcaKod);
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
        /// Arguman olarak verilen, toplu geçici görev yolluk belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="liste">Kaydedilecek bilgileri GeciciGorevYollukToplu nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc GeciciGorevYollukTopluKaydet(Kullanici kullanan, ObjectArray liste, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = (new PSLGeciciGorevYolluk()).KaydetToplu(kullanan, liste, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc GeciciGorevYollukTopluBelgeKaydet(Kullanici kullanan, ObjectArray liste, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = (new PSLGeciciGorevYolluk()).KaydetTopluBelge(kullanan, liste, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk belge bilgisini veritabanından okur 
        /// ve GeciciGorevYollukBelge nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk belge bilgisi listesini GeciciGorevYollukBelge nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray GeciciGorevYollukBelgeTumListe(Kullanici kullanan, GeciciGorevYollukBelge kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = (new PSLGeciciGorevYolluk()).TumListe(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, toplu geçici görev yolluk belge bilgisini veritabanından okur 
        /// ve GeciciGorevYollukToplu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk belge bilgisi listesini GeciciGorevYollukBelge nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray GeciciGorevYollukTopluListele(Kullanici kullanan, GeciciGorevYollukToplu kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = (new PSLGeciciGorevYolluk()).ListeleToplu(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, toplu geçici görev yolluk belge bilgisini veritabanından okur 
        /// ve GeciciGorevYollukToplu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk belge bilgisi listesini GeciciGorevYollukBelge nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray GeciciGorevYollukTopluBankaListesiListele(Kullanici kullanan, GeciciGorevYollukToplu kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = (new PSLGeciciGorevYolluk()).ListeleTopluBankaListesi(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk belgesini, geçici görev yolluk belge havuzundan siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islemi yapan.</param>
        /// <param name="sb">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc GeciciGorevYollukHavuzSil(Kullanici kullanan, GeciciGorevYollukBelge sb)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return (new PSLGeciciGorevYolluk()).HavuzSil(kullanan, sb);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk belge bilgisini veritabanından okur 
        /// ve GeciciGorevYollukBelge nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk belge bilgisini GeciciGorevYollukBelge nesnesi olarak tutan nesne</returns>
        public GeciciGorevYollukBelge GeciciGorevYollukBelgeListele(Kullanici kullanan, GeciciGorevYollukBelge kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return (new PSLGeciciGorevYolluk()).Listele(kullanan, kriter);
            }
            catch
            {
                return new GeciciGorevYollukBelge();
            }
        }

        public ObjectArray GeciciGorevYollukBelgeAyrintiliListele(Kullanici kullanan, GeciciGorevYollukBelge kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = (new PSLGeciciGorevYolluk()).ListeleAyrintili(kullanan, kriter);
            }
            catch
            {

            }

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk belge bilgisini veritabanından okur 
        /// ve GeciciGorevYollukBelge nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk belge bilgisini GeciciGorevYollukBelge nesnesi olarak tutan nesne</returns>
        public GeciciGorevYollukBelge OncedenKayitliMi(Kullanici kullanan, GeciciGorevYollukBelge kriter)
        {
            GeciciGorevYollukBelge donusDeger = new GeciciGorevYollukBelge();

            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = (new PSLGeciciGorevYolluk()).OncedenKayitliMi(kullanan, kriter);
            }
            catch
            {

            }

            return donusDeger;
        }
        /// <summary>
        /// Arguman olarak verilen, çeşitli ödemeler bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgileri CesitliOdeme nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MahkemeHarcKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCMahkemeHarc.Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, çeşitli ödemeler bilgisini veritabanından okur 
        /// ve CesitliOdeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, çeşitli ödemeler bilgisi listesini CesitliOdeme nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray MahkemeHarcListele(Kullanici kullanan, MahkemeHarc kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCMahkemeHarc.Listele(kullanan, kriter);
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
        /// Arguman olarak verilen, çeşitli ödemeler bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgileri CesitliOdeme nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc CesitliOdemeKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCCesitliOdemeBordro.Kaydet(kullanan, bilgi, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, çeşitli ödemeler bilgisini veritabanından okur 
        /// ve CesitliOdeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, çeşitli ödemeler bilgisi listesini CesitliOdeme nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray CesitliOdemeListele(Kullanici kullanan, CesitliOdeme kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCCesitliOdemeBordro.Listele(kullanan, kriter);
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
        /// Arguman olarak verilen, çeşitli ödemeler ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc CesitliOdemeOdemeEmriNoKaydet(Kullanici kullanan, CesitliOdeme bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCCesitliOdemeBordro().OdemeEmriNoKaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc CesitliOdemeBankaListesiKaydet(Kullanici kullanan, CesitliOdeme co, TNSDateTime bankaOdemeGun, int bankaOdemeSiraNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return new HRCCesitliOdemeBordro().CesitliOdemeBankaListesiKaydet(kullanan, co, bankaOdemeGun, bankaOdemeSiraNo);
        }

        /// <summary>
        /// Arguman olarak verilen, işçi maaş bordro bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgileri IsciMaasBordro nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IsciMaasBordroKaydet(Kullanici kullanan, ObjectArray kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIsciMaasBordro().Kaydet(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, işçi maaş bordro bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IsciMaasBordroSil(Kullanici kullanan, IsciMaasBordro kriter, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIsciMaasBordro().Sil(kullanan, kriter, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, işçi maaş bordro bilgisini veritabanından okur 
        /// ve IsciMaasBordro nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, işçi maaş bordro bilgisi listesini IsciMaasBordro nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IsciMaasBordroListele(Kullanici kullanan, IsciMaasBordro kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIsciMaasBordro().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, rapor bilgisini veritabanından okur 
        /// ve ilgili nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, rapor bilgisi listesini ilgili nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray RaporAl(Kullanici kullanan, RaporKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCRaporlar().RaporAl(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, fatura yıl ve fatura ay bilgisini kullanarak telefon abone fatura bilgisini Telekom'un gönderdiği xml dosyasından okur
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TelekomFaturaOlustur(Kullanici kullanan, AboneFaturaTelefon kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFaturaTelekom().Aktar(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, telekom fatura bilgisini veritabanından okur 
        /// ve AboneFaturaTelefon nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="sadeceYeniler">Eğer <c>true</c> verilirse süreç oluşturulmamış telekom fatura bilgisi listelenir.</param>
        /// <returns>İstenilen kriterdeki, telekom fatura bilgisi listesini AboneFaturaTelefon nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TelekomFaturaListele(Kullanici kullanan, AboneFaturaTelefon kriter, bool sadeceYeniler)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFaturaTelekom().Listele(kriter, sadeceYeniler);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, telekom fatura bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// Telekom fatura bilgisi varsa Sonuc nesnesinin islemSonuc özelliği true olarak gelir.
        /// </summary>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TelekomFaturaKayitVarMi(AboneFaturaTelefon kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new HRCAboneFaturaTelekom().KayitVarMi(kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, telekom fatura bilgisini veritabanından okur 
        /// ve fatura bilgisi bulunan kullanıcıları listeler ve bu kullanıcılara fatura bilgisini içeren 
        /// mail gönderir.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        public void TelekomFaturaMailAt(Kullanici kullanan, AboneFaturaTelefon kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                HRCAboneFaturaTelekom x = new HRCAboneFaturaTelekom();
                x.MailGonder(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere göre fatura dosya ismi üretir ve string olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="faturaYil">İşlem yapılacak fatura yıl bilgisi.</param>
        /// <param name="faturaAy">İşlem yapılacak fatura ay bilgisi.</param>
        /// <param name="faturaNolar">İşlem yapılacak fatura numaraları. ";" ile birbirinden ayrılır</param>
        /// <returns>Kriterlere uygun fatura dosya isimlerini ";" ile ayrılmış string olarak döner</returns>
        public string TelekomFaturaDosyaIsimVer(Kullanici kullanan, int faturaYil, int faturaAy, string faturaNolar)
        {
            string donusDeger = "";
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFaturaTelekom().FaturaDosyaIsimVer(kullanan, faturaYil, faturaAy, faturaNolar);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen dosyayı okur ve byte dizisi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="faturaYil">İşlem yapılacak fatura yil bilgisi.</param>
        /// <param name="faturaAy">İşlem yapılacak fatura ay bilgisi.</param>
        /// <param name="dosyaAd">İşlem yapılacak dosya adı bilgisi.</param>
        /// <returns>Okunan dosyayı byte dizisi olarak döner</returns>
        public byte[] TelekomFaturaDosyaVer(Kullanici kullanan, int faturaYil, int faturaAy, string dosyaAd)
        {
            byte[] donusDeger = { };
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFaturaTelekom().FaturaDosyaVer(kullanan, faturaYil, faturaAy, dosyaAd);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini veritabanından okur 
        /// ve AboneFaturaElektrik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="sadeceYeniler">Eğer <c>true</c> süreç oluşturulmayan faturalar gelir</param>
        /// <returns>İstenilen kriterdeki, elektrik fatura bilgisi listesini AboneFaturaElektrik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ElektrikFaturaListele(Kullanici kullanan, AboneFaturaElektrik kriter, bool sadeceYeniler)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFaturaElektrik().Listele(kriter, sadeceYeniler);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// Elektrik fatura bilgisi varsa Sonuc nesnesinin islemSonuc özelliği true olarak gelir.
        /// </summary>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc ElektrikFaturaKayitVarMi(AboneFaturaElektrik kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new HRCAboneFaturaElektrik().KayitVarMi(kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini veritabanından okur 
        /// ve ilgili dosyaya fatura çıktısını kaydeder
        /// ve sonucu string olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yil bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <returns>Fatura çıktısı oluşturma işlem sonucu string olarak döner</returns>
        public string ElektrikEskiFaturaCiktiOlustur(Kullanici kullanan, int yil, int ay)
        {
            string donusDeger = "";
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFaturaElektrik().EskiFaturalaraCiktiOlustur(kullanan, yil, ay);
            }
            catch (Exception e)
            {

                donusDeger = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini veritabanından okur 
        /// ve ilgili kullanıcılara fatura bilgisini mail olarak gönderir
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        public void ElektrikFaturaMailAt(Kullanici kullanan, AboneFaturaElektrik kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                HRCAboneFaturaElektrik x = new HRCAboneFaturaElektrik();
                x.MailGonder(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere göre fatura dosya ismi üretir ve string olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="faturaYil">İşlem yapılacak fatura yıl bilgisi.</param>
        /// <param name="faturaAy">İşlem yapılacak fatura ay bilgisi.</param>
        /// <param name="faturaNolar">İşlem yapılacak fatura numaraları. ";" ile birbirinden ayrılır</param>
        /// <returns>Kriterlere uygun fatura dosya isimlerini ";" ile ayrılmış string olarak döner</returns>
        public string ElektrikFaturaDosyaIsimVer(Kullanici kullanan, int faturaYil, int faturaAy, string faturaNolar)
        {
            string donusDeger = "";
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFaturaElektrik().FaturaDosyaIsimVer(kullanan, faturaYil, faturaAy, faturaNolar);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen dosyayı okur ve byte dizisi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="faturaYil">İşlem yapılacak fatura yil bilgisi.</param>
        /// <param name="faturaAy">İşlem yapılacak fatura ay bilgisi.</param>
        /// <param name="dosyaAd">İşlem yapılacak dosya adı bilgisi.</param>
        /// <returns>Okunan dosyayı byte dizisi olarak döner</returns>
        public byte[] ElektrikFaturaDosyaVer(Kullanici kullanan, int faturaYil, int faturaAy, string dosyaAd)
        {
            byte[] donusDeger = { };
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFaturaElektrik().FaturaDosyaVer(kullanan, faturaYil, faturaAy, dosyaAd);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, su fatura bilgisini veritabanından okur 
        /// ve AboneFaturaSu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="sadeceYeniler">Eğer <c>true</c> süreç oluşturulmayan faturalar gelir</param>
        /// <returns>İstenilen kriterdeki, su fatura bilgisi listesini AboneFaturaSu nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray SuFaturaListele(Kullanici kullanan, AboneFaturaSu kriter, bool sadeceYeniler)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFaturaSu().Listele(kriter, sadeceYeniler);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, su fatura bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// Su fatura bilgisi varsa Sonuc nesnesinin islemSonuc özelliği true olarak gelir.
        /// </summary>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SuFaturaKayitVarMi(AboneFaturaSu kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new HRCAboneFaturaSu().KayitVarMi(kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini veritabanından okur 
        /// ve ilgili kullanıcılara fatura bilgisini mail olarak gönderir
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        public void SuFaturaMailAt(Kullanici kullanan, AboneFaturaSu kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                HRCAboneFaturaSu x = new HRCAboneFaturaSu();
                x.MailGonder(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere göre fatura dosya ismi üretir ve string olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="faturaYil">İşlem yapılacak fatura yıl bilgisi.</param>
        /// <param name="faturaAy">İşlem yapılacak fatura ay bilgisi.</param>
        /// <param name="faturaNolar">İşlem yapılacak fatura numaraları. ";" ile birbirinden ayrılır</param>
        /// <returns>Kriterlere uygun fatura dosya isimlerini ";" ile ayrılmış string olarak döner</returns>
        public string SuFaturaDosyaIsimVer(Kullanici kullanan, int faturaYil, int faturaAy, string faturaNolar)
        {
            string donusDeger = "";
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFaturaSu().FaturaDosyaIsimVer(kullanan, faturaYil, faturaAy, faturaNolar);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen dosyayı okur ve byte dizisi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="faturaYil">İşlem yapılacak fatura yıl bilgisi.</param>
        /// <param name="faturaAy">İşlem yapılacak fatura ay bilgisi.</param>
        /// <param name="dosyaAd">İşlem yapılacak dosya adı bilgisi.</param>
        /// <returns>Okunan dosyayı byte dizisi olarak döner</returns>
        public byte[] SuFaturaDosyaVer(Kullanici kullanan, int faturaYil, int faturaAy, string dosyaAd)
        {
            byte[] donusDeger = { };
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFaturaSu().FaturaDosyaVer(kullanan, faturaYil, faturaAy, dosyaAd);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, süreç hatırlatma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kod">İşlem yapılacak hatırlatma kodu bilgisi</param>
        /// <param name="aciklama">İşlem yapılacak hatırlatma açıklama bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SurecHatirlatmaKaydet(Kullanici kullanan, int kod, string aciklama)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSurec().HatirlatmaKaydet(kullanan, kod, aciklama);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç hatırlatma bilgisini veritabanından okur 
        /// ve string (kod|aciklama) nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <returns>İstenilen kriterdeki, süreç hatırlatma bilgisi listesini string (kod|aciklama) nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray SurecHatirlatmaSorgula(Kullanici kullanan)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSurec().HatirlatmaSorgula(kullanan);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, süreç tertip bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tertip">İşlem yapılacak tertip bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SurecTertipKaydet(Kullanici kullanan, string tertip)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSurec().TertipKaydet(kullanan, tertip);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, süreç tertip bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tertip">İşlem yapılacak tertip bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SurecTertipSil(Kullanici kullanan, string tertip)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSurec().TertipSil(kullanan, tertip);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç tertip bilgisini veritabanından okur 
        /// ve string nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan"> The kullanan.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kodu yıl bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kodu bilgisi</param>
        /// <returns>İstenilen kriterdeki, süreç tertip bilgisi listesini string nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray SurecTertipSorgula(Kullanici kullanan, int muhasebe, string birim)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSurec().TertipSorgula(kullanan, muhasebe, birim);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, mal hizmet yapım alım tutar bilgisini veritabanından okur 
        /// ve MHYAlimTutari nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, mal hizmet yapım alım tutar bilgisini MHYAlimTutari nesnesi olarak tutan nesne</returns>
        public MHYAlimTutari MHYAlimTutariListele(Kullanici kullanan, MHYAlimTutari kriter)
        {
            MHYAlimTutari donusDeger = new MHYAlimTutari();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCMHYAlimTutari().Listele(kullanan, kriter);
            }
            catch
            {

            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, kamu ihale kurumu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KIKBilgileriKaydet(Kullanici kullanan, ObjectArray bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCKIKBilgileri().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kamu ihale kurumu bilgisini veritabanından okur 
        /// ve KIKBilgileri nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, kamu ihale kurumu bilgisi listesini KIKBilgileri nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray KIKBilgileriListele(Kullanici kullanan, KIKBilgileri kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCKIKBilgileri().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, il bilgisini veritabanından okur 
        /// ve Il nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <returns>İstenilen kriterdeki, il billgisi listesini Il nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IlListeleKY(Kullanici kullanan, string ilkod = null)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new SeyyarGorevC().IlListele(kullanan, ilkod);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, il bilgisini veritabanından okur 
        /// ve Il nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <returns>İstenilen kriterdeki, il billgisi listesini Il nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IlListele(Kullanici kullanan)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCIl().Listele(kullanan);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, il bilgisini veritabanından okur 
        /// ve Il nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="ilKod">İşlem yapılacak il kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, il bilgisini Il nesnesi olarak tutan nesne</returns>
        public Il IlListele(Kullanici kullanan, string ilKod)
        {
            Il donusDeger = new Il();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIl().Listele(kullanan, ilKod);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, il bilgisi listesini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgileri Il nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IlListesiKaydet(Kullanici kullanan, ObjectArray bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIl().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ülke bilgisini veritabanından okur 
        /// ve Ulke nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <returns>İstenilen kriterdeki, ulke billgisi listesini Ulke nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray UlkeListele(Kullanici kullanan)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCUlke().Listele(kullanan);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ulke bilgisini veritabanından okur 
        /// ve Ulke nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="ulkeKod">İşlem yapılacak ulke kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, ulke bilgisini Ulke nesnesi olarak tutan nesne</returns>
        public Ulke UlkeListele(Kullanici kullanan, string ulkeKod)
        {
            Ulke donusDeger = new Ulke();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCUlke().Listele(kullanan, ulkeKod);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, ulke bilgisi listesini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgileri Ulke nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc UlkeListesiKaydet(Kullanici kullanan, ObjectArray bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCUlke().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, abone no bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc AboneNoKaydet(Kullanici kullanan, AboneNo bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCAboneNo().Kaydet(kullanan, bilgi);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, abone no bilgisini veritabanından okur 
        /// ve AboneNo nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, abone no listesini AboneNo nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray AboneNoListele(Kullanici kullanan, AboneNo kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneNo().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, tedavi bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TedaviKaydet(Kullanici kullanan, Tedavi liste, int islemKod, int parcaKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLTedavi().Kaydet(kullanan, liste, islemKod, parcaKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yıllık tedavi harcama ortalama bilgisini veritabanından okur 
        /// ve TedaviIstatistik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, yıllık tedavi harcama ortalama bilgisi listesini TedaviIstatistik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TedaviListeleIstatistik(Kullanici kullanan, TedaviIstatistik kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = (new PSLTedavi()).ListeleYillikTedaviOrtalamasi(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;

        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, tedavi bilgisini veritabanından okur 
        /// ve Tedavi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, tedavi bilgisini Tedavi nesnesi olarak tutan nesne</returns>
        public Tedavi TedaviListele(Kullanici kullanan, Tedavi kriter)
        {
            Tedavi donusDeger = new Tedavi();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLTedavi().Listele(kullanan, kriter);
            }
            catch
            {
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, tedavi bilgisine ait ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TedaviOdemeEmriNoKaydet(Kullanici kullanan, Tedavi kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLTedavi().OdemeEmriNoKaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, personel bilgisini veritabanından okur 
        /// ve PersonelBilgi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tcKimlikNo">İşlem yapılacak TC Kimlik No bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kodu bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kodu bilgisi.</param>
        /// <returns>İstenilen kriterdeki, personel bilgisini PersonelBilgi nesnesi olarak tutan nesne</returns>
        public PersonelBilgi PersonelBilgiListele(Kullanici kullanan, string tcKimlikNo, int muhasebe, string birim)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCPersonel().Listele(null, kullanan, tcKimlikNo, muhasebe, birim);
            }
            catch (Exception ex)
            {
                OrtakFonksiyonlar.HataStrYaz(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, öğrenci eğitmen bilgisini veritabanından okur 
        /// ve PersonelBilgi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tcKimlikNo">İşlem yapılacak TC Kimlik No bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kodu bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kodu bilgisi.</param>
        /// <returns>İstenilen kriterdeki, personel bilgisini PersonelBilgi nesnesi olarak tutan nesne</returns>
        public PersonelBilgi PersonelOgrenciEgitmenListele(Kullanici kullanan, string tcKimlikNo, int muhasebe, string birim)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCPersonel().ListelePersonelOgrenciEgitmen(kullanan, tcKimlikNo, muhasebe, birim);
            }
            catch
            {
                return new PersonelBilgi();
            }
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, personel aile bilgisini veritabanından okur 
        /// ve PersonelAileBilgi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, personel aile bilgisi listesini PersonelAileBilgi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray PersonelAileListele(PersonelAileBilgi kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCPersonel().AileListele(kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk bilgisine ait ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc GeciciGorevYollukOdemeEmriNoKaydet(Kullanici kullanan, GeciciGorevYollukBelge kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLGeciciGorevYolluk().OdemeEmriNoKaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, toplu geçici görev yolluk bilgisine ait ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc GeciciGorevYollukOdemeEmriNoKaydet(Kullanici kullanan, GeciciGorevYollukToplu kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLGeciciGorevYolluk().OdemeEmriNoKaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, sürekli görev yolluk bilgisine ait ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SurekliGorevYollukOdemeEmriNoKaydet(Kullanici kullanan, SurekliGorevYollukBelge kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLSurekliGorevYolluk().OdemeEmriNoKaydet(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, kur bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri KurBilgisi nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KurBilgisiKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCKurBilgisi().Kaydet(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kur bilgisini veritabanından okur 
        /// ve KurBilgisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, kur bilgisi listesini KurBilgisi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray KurBilgisiListele(Kullanici kullanan, KurBilgisi kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCKurBilgisi().Listele(null, kullanan, kriter);
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
        /// Arguman olarak verilen, avans tutarı hesaplamasında kullanılan avans hesap bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc AvansHesapKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAvansHesap().Kaydet(kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, avans tutarı hesaplamasında kullanılan avans hesap bilgisini veritabanından okur 
        /// ve AvansHesap nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, avans tutarı hesaplamasında kullanılan avans hesap bilgisi listesini AvansHesap nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray AvansHesapListele(Kullanici kullanan, AvansHesap kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAvansHesap().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, borcu yok belgesi durumuna göre borcu yok belgelerini bir sonraki duruma taşır
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="belge">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc BorcuYokBelgeleriGonder(BorcuYokBelgesi belge)
        {
            Sonuc donusDeger = new Sonuc(true, "", "");
            try
            {
                Kullanici kullanan = new Kullanici();
                kullanan.kullaniciKodu = "aktarim";
                kullanan.kullaniciTipi = new ENUMKullaniciTipi[1];
                kullanan.kullaniciTipi[0] = ENUMKullaniciTipi.SISTEMYONETICISI;

                belge.durum = (int)ENUMBorcuYokDurum.SIRADA;
                donusDeger = new HRCBorcuYokBelgesi().SiradaBekleyenleriGonder(kullanan, belge);

                belge.durum = (int)ENUMBorcuYokDurum.GONDERILDI;
                donusDeger = new HRCBorcuYokBelgesi().CevaplananlariGoster(kullanan, belge);

                belge.durum = (int)ENUMBorcuYokDurum.CEVAPLANMADI;
                donusDeger = new HRCBorcuYokBelgesi().CevaplananlariGoster(kullanan, belge);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();

                OrtakFonksiyonlar.HataStrYaz("Borcu Yok Belgesi Kontrol Hata. " + e.Message + "*" + e.StackTrace);
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, borcu yok belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="belge">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc BorcuYokKaydet(Kullanici kullanan, BorcuYokBelgesi belge)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCBorcuYokBelgesi().Kaydet(kullanan, belge, false);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc BorcuYokSil(Kullanici kullanan, TNSCollection belgeler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCBorcuYokBelgesi().Sil(kullanan, belgeler);
            }
            catch (Exception ex)
            {
                return new Sonuc(false, ex.ToString(), "");
            }
        }


        /// <summary>
        /// Arguman olarak verilen, borcu belge durum bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc BorcuYokBelgeDurumDegistir(Kullanici kullanan, BorcuYokBelgesi bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCBorcuYokBelgesi().BelgeDurumDegistir(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, borcu yok belge bilgisini veritabanından okur 
        /// ve BorcuYokBelgesi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, borcu yok belge bilgisi listesini BorcuYokBelgesi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray BorcuYokListele(Kullanici kullanan, BorcuYokBelgesi kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCBorcuYokBelgesi().Listele(kullanan, kriter, false);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, borcu yok belge bilgisini veritabanından okur 
        /// ve BorcuYokBelgesi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, borcu yok belge bilgisini BorcuYokBelgesi nesnesi olarak tutan nesne</returns>
        public BorcuYokBelgesi BorcuYokAc(Kullanici kullanan, BorcuYokBelgesi kriter)
        {

            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCBorcuYokBelgesi().BorcuYokAc(kullanan, kriter);
            }
            catch (Exception ex)
            {
                kriter.cevapKoduGeldi = false;
                kriter.durum = (int)ENUMBorcuYokDurum.DILEKCESORGUHATASI;
                kriter.hataMesaji = ex.Message;
                return kriter;
            }
        }

        /// <summary>
        /// Gelirler İdaresi Başkanlığından alınan servislerini kullanma bilgilerini veritabanına kaydeder
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="sorgulanan">İşlem yapılacak sorgulanan bilgisi.</param>
        /// <param name="kullanan">İşlem yapılacak işlem yapan kullanıcı adı bilgisi.</param>
        /// <param name="aciklama">İşlem yapılacak aciklama bilgisi.</param>
        /// <param name="kurum">İşlem yapılacak kurum bilgisi.</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc GIBLogKaydet(string sorgulanan, string kullanan, string aciklama, string kurum)
        {
            try
            {
                return new HRCBorcuYokBelgesi().GIBLogKaydet(sorgulanan, kullanan, aciklama, kurum);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }


        public Sonuc BorcuYokSGKKaydet(Kullanici kullanan, BorcuYokBelgesiSGK belge)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCBorcuYokBelgesiSGK().Kaydet(kullanan, belge);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc BorcuYokSGKBelgeDurumDegistir(Kullanici kullanan, BorcuYokBelgesiSGK bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCBorcuYokBelgesiSGK().BelgeDurumDegistir(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray BorcuYokSGKListele(Kullanici kullanan, BorcuYokBelgesiSGK kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCBorcuYokBelgesiSGK().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }


        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini ilgili web servisden okur
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc ElektrikFaturaAktar(Kullanici kullanan, int yil, int ay)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFaturaElektrik().Aktar(kullanan, yil, ay);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, su fatura bilgisini ilgili web servisden okur
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SuFaturaAktar(Kullanici kullanan, int yil, int ay)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAboneFaturaSu().Aktar(kullanan, yil, ay);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, abone fatura bilgisine ait süreç oluşturma işlemlerini yapar
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="aboneTip">İşlem yapılacak abone tip bilgisi.</param>
        public void AboneServisiSurecOlustur(Kullanici kullanan, int yil, int ay, int aboneTip)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                new HRCAboneServis().SurecOlustur(kullanan, yil, ay, aboneTip);
            }
            catch (Exception ex) { string a = ex.Message; }
        }

        /// <summary>
        /// Arguman olarak verilen, abone fatura bilgisine ait ödeme emri belgesi oluşturma işlemlerini yapar
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="aboneTip">İşlem yapılacak abone tip bilgisi.</param>
        public void AboneServisiOdemeEmriOlustur(Kullanici kullanan, int yil, int ay, int aboneTip)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                new HRCAboneServis().OdemeEmriOlustur(kullanan, yil, ay, aboneTip);
            }
            catch { }
        }

        /// <summary>
        /// Arguman olarak verilen, abone fatura bilgisine ait ödenek gönderme belgesi oluşturma işlemlerini yapar
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="aboneTip">İşlem yapılacak abone tip bilgisi.</param>
        public void AboneServisiOdenekGonderme(Kullanici kullanan, int yil, int ay, int aboneTip)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                new HRCAboneServis().OdenekGonderme(kullanan, yil, ay, aboneTip);
            }
            catch { }
        }

        /// <summary>
        /// Arguman olarak verilen, abone fatura bilgisine ait mail gönderme işlemlerini yapar
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="aboneTip">İşlem yapılacak abone tip bilgisi.</param>
        public void AboneServisiMailGonder(Kullanici kullanan, int yil, int ay, int aboneTip)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                new HRCAboneServis().MailGonder(kullanan, yil, ay, aboneTip);
            }
            catch { }
        }

        /// <summary>
        /// Arguman olarak verilen, avans artığı bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc AvansArtigiKaydet(Kullanici kullanan, AvansArtigi bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAvansArtigi().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, avans artığı bilgisini veritabanından okur 
        /// ve AvansArtigi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, avans artığı listesini AvansArtigi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray AvansArtigiListele(Kullanici kullanan, AvansArtigi kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAvansArtigi().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, avans artığı bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc AvansArtigiSil(Kullanici kullanan, AvansArtigi kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCAvansArtigi().Sil(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, süreç parça bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SurecParcaKaydet(Kullanici kullanan, SurecParca bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSurecParca().Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç parça bilgisini veritabanından okur 
        /// ve SurecParca nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, süreç parça bilgisini SurecParca nesnesi olarak tutan nesne</returns>
        public SurecParca SurecParcaListele(Kullanici kullanan, SurecParca kriter)
        {
            SurecParca donusDeger = new SurecParca();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSurecParca().ListeleTek(kullanan, kriter);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç parça bilgisini veritabanından okur 
        /// ve SurecParca nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, süreç parça listesini SurecParca nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray SurecParcaListelee(Kullanici kullanan, SurecParca kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSurecParca().ListeleCok(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, süreç parça bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SurecParcaSil(Kullanici kullanan, SurecParca bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCSurecParca().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk istatistik bilgisini veritabanından okur 
        /// ve PSLYollukIstatistikSinif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk istatistik bilgisi listesini PSLYollukIstatistikSinif nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray GeciciGorevYollukIstatistik(Kullanici kullanan, PSLYollukIstatistikSinif kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = (new PSLGeciciGorevYolluk()).ListeleIstatistik(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;

        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk istatistik bilgisini veritabanından okur 
        /// ve PSLYollukIstatistikSinif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk istatistik bilgisi listesini PSLYollukIstatistikSinif nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray SurekliGorevYollukIstatistik(Kullanici kullanan, PSLYollukIstatistikSinif kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = (new PSLSurekliGorevYolluk()).ListeleIstatistikKurlu(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;

        }

        /// <summary>
        /// Arguman olarak verilen, şartname ihale ilan bilgisini veritabanına kaydeder
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc SartnameIhaleIlanBilgisiKaydet(Kullanici kullanan, IhaleIlanBilgisi bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCIhaleIlanBilgisi().SartnameIhaleIlanBilgisiKaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// MerkezBankasiKurSinif nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MerkezBankasiKurSinif yapısında MerkezBankasiKurSinif: bilgilerini tutan değişken</param>
        /// <returns></returns>
        public MerkezBankasiKurSinif MerkezBankKurOku(Kullanici kullanan, MerkezBankasiKurSinif form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCMerkezBankKur.Oku(null, kullanan, form);
            }
            catch (Exception exc)
            {
                MerkezBankasiKurSinif donDeger = new MerkezBankasiKurSinif();
                donDeger.sonuc.islemSonuc = false;
                donDeger.sonuc.hataStr = exc.Message;
                return donDeger;
            }
        }

        /// <summary>
        /// MerkezBankasiKurSinif nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tarihBas">Başlangıç tarihi</param>
        /// <param name="tarihBit">Bitiş tarihi</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc MerkezBankKurKaydet(Kullanici kullanan, TNSDateTime tarihBas, TNSDateTime tarihBit)
        {
            try
            {
                //TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCMerkezBankKur.Kaydet(kullanan, tarihBas, tarihBit);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }


        /// <summary>
        /// Merkezs the bank serbest kur guncelle.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public Sonuc MerkezBankSerbestKurGuncelle(Kullanici kullanan, MerkezBankasiKurSinif form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCMerkezBankKur.SerbestKurGuncelle(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// MerkezBankasiKurSinif nesnesini veri tabanından okuyan fonksiyon ve içinde MerkezBankasiKurSinif nesleri tutan ObjectArray döndürür
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="form">MerkezBankasiKurSinif yapısında MerkezBankasiKurSinif: bilgilerini tutan değişken</param>
        /// <param name="kurlar">Hangi kurların gösterileceği bilgisi</param>
        /// <returns></returns>
        public ObjectArray MerkezBankKurDegerleri(MerkezBankasiKurSinif form, string[] kurlar)
        {
            ObjectArray donDeger = new ObjectArray();
            try
            {
                donDeger = HRCMerkezBankKur.KurDegerleri(form, kurlar);
            }
            catch (Exception exc)
            {

                donDeger.sonuc.islemSonuc = false;
                donDeger.sonuc.hataStr = exc.Message;

                return donDeger;
            }

            return donDeger;
        }

        /// <summary>
        /// MerkezBankasiKurSinif nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tarihBas">Başlangıç tarihi</param>
        /// <param name="tarihBit">Bitiş tarihi</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc GunlukKurKaydet(Kullanici kullanan, MerkezBankasiKurSinif form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCMerkezBankKur.GunlukKurlariKaydet(null, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }


        /// <summary>
        /// SurecSifreSinif nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SurecSifreSinif yapısında SurecSifreSinif: bilgilerini tutan değişken</param>
        public SurecSifreSinif SurecSifreOku(Kullanici kullanan, SurecSifreSinif form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return HRCSurecSifre.Oku(null, kullanan, form);
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç şifre bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kriterSinif">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SurecSifreDogrula(SurecSifreSinif kriterSinif)
        {
            return HRCSurecSifre.SifreDogrula(kriterSinif);
        }

        /// <summary>
        /// Arguman olarak verilen, süreç şifre bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SurecSifreSil(Kullanici kullanan, SurecSifreSinif form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCSurecSifre.Sil(null, kullanan, form);
            }
            catch (Exception exc)
            {
                return new Sonuc(false, exc.Message, "");
            }

        }

        /// <summary>
        /// SurecSifreSinif nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SurecSifreSinif yapısında SurecSifreSinif: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SurecSifreKaydet(Kullanici kullanan, SurecSifreSinif form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCSurecSifre.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk toplam yolculuk gün sayısı bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk toplam yolculuk gün sayısı bilgisini double olarak döner</returns>
        public double GGYToplamYolculukGunSayisi(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl)
        {
            double donusDeger = 0.0;
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLGeciciGorevYolluk().ToplamYolculukGunSayisiGetir(kullanan, yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            }
            catch
            {
                donusDeger = 0.0;
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk toplam görev gün sayısı bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk toplam görev gün sayısı bilgisini double olarak döner</returns>
        public double GGYToplamGorevGunSayisi(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl)
        {
            double donusDeger = 0.0;
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLGeciciGorevYolluk().ToplamGorevGunSayisiGetir(kullanan, yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            }
            catch
            {
                donusDeger = 0.0;
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk toplam gün sayısı bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk toplam gün sayısı bilgisini double olarak döner</returns>
        public double GGYToplamGunSayisi(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl)
        {
            double donusDeger = 0.0;
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLGeciciGorevYolluk().ToplamGunSayisiGetir(kullanan, yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            }
            catch
            {
                donusDeger = 0.0;
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk toplam yevmiye tutar bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk toplam yevmiye tutar bilgisini double olarak döner</returns>
        public double GGYToplamYevmiyeTutar(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl)
        {
            double donusDeger = 0.0;
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLGeciciGorevYolluk().ToplamYevmiyeTutarGetir(kullanan, yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            }
            catch
            {
                donusDeger = 0.0;
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk toplam adam sayısı bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk toplam adam sayısı bilgisini double olarak döner</returns>
        public double GGYToplamAdamSayisi(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl)
        {
            double donusDeger = 0.0;
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLGeciciGorevYolluk().ToplamAdamSayisiGetir(kullanan, yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            }
            catch
            {
                donusDeger = 0.0;
            }
            return donusDeger;
        }

        /// <summary>
        /// Belirtilen parametrelere ait Harcama Yönetimi sisteminde gerçekleşen verileri döndürür.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Gerçekleşmenin yılı</param>
        /// <param name="muhasebeKod">Gerçekleşmenin yapıldığı muhasebeKod</param>
        /// <param name="birimKod">Gerçekleşmenin yapıldığı birimKod</param>
        /// <param name="belgeTur">Gerçekleşmenin türü (Yurt içi, Yurt dışı)</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İşlem sonucunu döndüren, ObjectArray sınıfında nesne içinde ObjectArrayler içerir ve her bir ObjectArray içerisinde sırasıyla Ülke Adı, Görev Sayısı, Toplam Görev Gün Sayısı ve Toplam Tutar bilgileri bulunur.</returns>
        public ObjectArray GGYToplamGerceklesmeListele(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLGeciciGorevYolluk().ToplamGerceklesmeBilgileriListele(kullanan, yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, denetim elemanları için geçici görev yolluk toplam gerçekleşme bilgisini veritabanından okur 
        /// ve (gündelik adı, görev sayısı, toplam görev gün sayısı, toplam tutar) dizisi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, denetim elemanları için geçici görev yolluk toplam gerçekleşme bilgisini, 
        /// (gündelik adı, görev sayısı, toplam görev gün sayısı, toplam tutar) dizisi olarak tutan nesne</returns>
        public ObjectArray GGYToplamGerceklesmeDenetimListele(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, bool sadeceIl)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLGeciciGorevYolluk().ToplamGerceklesmeDenetimBilgileriListele(kullanan, yil, muhasebeKod, birimKod, sadeceIl);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk toplam yevmiye tutar bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk toplam yevmiye tutar bilgisini double olarak döner</returns>
        public double SGYToplamYevmiyeTutar(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl)
        {
            double donusDeger = 0.0;
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLSurekliGorevYolluk().ToplamYevmiyeTutarGetir(kullanan, yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            }
            catch
            {
                donusDeger = 0.0;
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk ortalama mesafe bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk ortalama mesafe bilgisini double olarak döner</returns>
        public double SGYOrtalamaMesafe(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl)
        {
            double donusDeger = 0.0;
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLSurekliGorevYolluk().OrtalamaMesafeGetir(kullanan, yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            }
            catch
            {
                donusDeger = 0.0;
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk toplam adam sayısı bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk toplam adam sayısı bilgisini double olarak döner</returns>
        public double SGYToplamAdamSayisi(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl)
        {
            double donusDeger = 0.0;
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLSurekliGorevYolluk().ToplamAdamSayisiGetir(kullanan, yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            }
            catch
            {
                donusDeger = 0.0;
            }
            return donusDeger;
        }

        /// <summary>
        /// Belirtilen parametrelere ait Harcama Yönetimi sisteminde gerçekleşen verileri döndürür.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Gerçekleşmenin yılı</param>
        /// <param name="muhasebeKod">Gerçekleşmenin yapıldığı muhasebeKod</param>
        /// <param name="birimKod">Gerçekleşmenin yapıldığı birimKod</param>
        /// <param name="belgeTur">Gerçekleşmenin türü (Yurt içi, Yurt dışı)</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İşlem sonucunu döndüren, ObjectArray sınıfında nesne içinde ObjectArrayler içerir ve her bir ObjectArray içerisinde sırasıyla Gerçekleşme türü (TAYIN,EMEKLI), Adam Sayısı ve Ödeme Tutarı bilgileri bulunur.</returns>
        public ObjectArray SGYToplamGerceklesmeListele(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLSurekliGorevYolluk().ToplamGerceklesmeBilgileriListele(kullanan, yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, tayin ve emeklilik için düzenlenen sürekli görev yolluk gerçekleşme bilgisini veritabanından okur 
        /// ve (tür adı, kişi sayısı, ödenen tutar) nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, tayin ve emeklilik için düzenlenen sürekli görev yolluk gerçekleşme bilgisini
        /// (tür adı, kişi sayısı, ödenen tutar) nesne koleksiyonu olarak döner</returns>
        public ObjectArray SGYToplamGerceklesmeTayinEmekliListele(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new PSLSurekliGorevYolluk().ToplamGerceklesmeBilgileriTayinEmekliListele(kullanan, yil, muhasebeKod, birimKod, belgeTur, sadeceIl);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, tertiplere ait aylık harcama bilgisini veritabanından okur 
        /// ve AylikDagilim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="kurKod">İşlem yapılacak kurumsal kod bilgisi.</param>
        /// <param name="islemKodListesi">İşlem yapılacak işlem kod listesi.</param>
        /// <returns>İstenilen kriterdeki, tertiplere ait aylık harcama bilgisi listesini AylikDagilim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray HarcamaTertipAylikDagilim(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, string kurKod, int[] islemKodListesi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOdemeEmriForm().ListeleAylikDagilim(kullanan, yil, muhasebeKod, birimKod, kurKod, islemKodListesi);

            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, toplam tertip harcama bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="kurKod">İşlem yapılacak kurumsal kod bilgisi.</param>
        /// <param name="islemKodListesi">İşlem yapılacak işlem kod listesi.</param>
        /// <returns>İstenilen kriterdeki, toplam tertip harcama bilgisini double olarak döner</returns>
        public double HarcamaTertipToplam(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, string kurKod, int[] islemKodListesi)
        {
            double donusDeger = 0;
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCOdemeEmriForm().ToplamAylikDagilim(kullanan, yil, muhasebeKod, birimKod, kurKod, islemKodListesi);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürecin yıllara sari olup olmadığı bilgisini veritabanından okur 
        /// ve bool olarak döner
        /// </summary>
        /// <param name="surecYil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <returns>İstenilen kriterdeki, sürecin yıllara sari olup olmadığı bilgisini bool olarak döner</returns>
        public bool YillaraSariIslemMi(int surecYil, string surecNo)
        {
            bool donusDeger = false;
            try
            {
                donusDeger = new HRCSurec().YillaraSariIslemMi(surecYil, surecNo);
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yurt içi geçici görev yolluk istatistik bilgisini veritabanından okur 
        /// ve YIGGYIstatistik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tur">İşlem yapılacak tür bilgisi.</param>
        /// <returns>İstenilen kriterdeki, yurt içi geçici görev yolluk istatistik bilgisi listesini YIGGYIstatistik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleYIGGYIstatistik(int yil, ENUMYIGGYTur tur)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PSLGeciciGorevYolluk().ListeleYIGGYIstatistik(yil, tur);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yurt dışı geçici görev yolluk istatistik bilgisini veritabanından okur 
        /// ve YDGGYIstatistik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tur">İşlem yapılacak tür bilgisi.</param>
        /// <returns>İstenilen kriterdeki, yurt dışı geçici görev yolluk istatistik bilgisi listesini YDGGYIstatistik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleYDGGYIstatistik(int yil, ENUMYDGGYTur tur)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PSLGeciciGorevYolluk().ListeleYDGGYIstatistik(yil, tur);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yurt içi sürekli görev yolluk istatistik bilgisini veritabanından okur 
        /// ve YISGYIstatistik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tur">İşlem yapılacak tür bilgisi.</param>
        /// <returns>İstenilen kriterdeki, yurt içi geçici görev yolluk istatistik bilgisi listesini YISGYIstatistik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleYISGYIstatistik(int yil, ENUMYISGYTur tur)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PSLGeciciGorevYolluk().ListeleYISGYIstatistik(yil, tur);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yurt dışı sürekli görev yolluk istatistik bilgisini veritabanından okur 
        /// ve YDSGYIstatistik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tur">İşlem yapılacak tür bilgisi.</param>
        /// <returns>İstenilen kriterdeki, yurt dışı sürekli görev yolluk istatistik bilgisi listesini YDSGYIstatistik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleYDSGYIstatistik(int yil, ENUMYDSGYTur tur)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PSLGeciciGorevYolluk().ListeleYDSGYIstatistik(yil, tur);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura istatistik bilgisini veritabanından okur 
        /// ve ElektrikIstatistik nesnesi olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, elektrik fatura istatistik bilgisini ElektrikIstatistik nesnesi olarak tutan nesne</returns>
        public ElektrikIstatistik ListeleElektrikIstatistik(int yil, int muhasebe, string birim)
        {
            ElektrikIstatistik donusDeger = new ElektrikIstatistik();
            try
            {
                donusDeger = new HRCAboneFaturaElektrik().ListeleElektrikIstatistik(yil, muhasebe, birim);
            }
            catch
            {
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, su fatura istatistik bilgisini veritabanından okur 
        /// ve SuIstatistik nesnesi olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, su fatura istatistik bilgisini SuIstatistik nesnesi olarak tutan nesne</returns>
        public SuIstatistik ListeleSuIstatistik(int yil, int muhasebe, string birim)
        {
            SuIstatistik donusDeger = new SuIstatistik();
            try
            {
                donusDeger = new HRCAboneFaturaSu().ListeleSuIstatistik(yil, muhasebe, birim);
            }
            catch
            {
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, telekom fatura istatistik bilgisini veritabanından okur 
        /// ve TelekomIstatistik nesnesi olarak döner
        /// </summary>
        /// <param name="tip">İşlem yapılacak abone tip bilgisi.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, telekom fatura istatistik bilgisini TelekomIstatistik nesnesi olarak tutan nesne</returns>
        public TelekomIstatistik ListeleTelekomIstatistik(ENUMAboneTip tip, int yil, int muhasebe, string birim)
        {
            TelekomIstatistik donusDeger = new TelekomIstatistik();
            try
            {
                donusDeger = new HRCAboneFaturaTelekom().ListeleTelekomIstatistik(tip, yil, muhasebe, birim);
            }
            catch
            {
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, onay belgesi harcama bilgisini veritabanından okur 
        /// ve BHMHarcama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılcak ay bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <param name="peb">İşlem yapılacak alt faaliyet kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, onay belgesi harcama bilgisi listesini BHMHarcama nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray BHMOnayBelgesiHarcamaListesi(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCOnayForm().BHMOnayBelgesiHarcamaListesi(yil, ay, muhasebe, birim, kur, fon, fin, eko, peb);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, alım harcama bilgisini veritabanından okur 
        /// ve BHMHarcama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılcak ay bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <param name="peb">İşlem yapılacak alt faaliyet kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, alım harcama bilgisi listesini BHMHarcama nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray BHMAlimHarcamaListesi(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCTeklif().BHMAlimHarcamaListesi(yil, ay, muhasebe, birim, kur, fon, fin, eko, peb);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fatura harcama bilgisini veritabanından okur 
        /// ve BHMHarcama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılcak ay bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <param name="peb">İşlem yapılacak alt faaliyet kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, fatura harcama bilgisi listesini BHMHarcama nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray BHMFaturaHarcamaListesi(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCFaturaForm().BHMFaturaHarcamaListesi(yil, ay, muhasebe, birim, kur, fon, fin, eko, peb);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, taşınır harcama bilgisini veritabanından okur 
        /// ve BHMHarcama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılcak ay bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <param name="peb">İşlem yapılacak alt faaliyet kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, taşınır harcama bilgisi listesini BHMHarcama nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray BHMTeslimHarcamaListesi(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCTasinirForm().BHMTeslimHarcamaListesi(yil, ay, muhasebe, birim, kur, fon, fin, eko, peb);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        ///// <summary>
        ///// Arguman olarak verilen kriterlere bağlı olarak, ödeme yapılan harcama bilgisini veritabanından okur 
        ///// ve BHMHarcama nesne koleksiyonu olarak döner
        ///// </summary>
        ///// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        ///// <param name="ay">İşlem yapılcak ay bilgisi.</param>
        ///// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        ///// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        ///// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        ///// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        ///// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        ///// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        ///// <param name="peb">İşlem yapılacak alt faaliyet kod bilgisi.</param>
        ///// <returns>İstenilen kriterdeki, ödeme yapılan harcama bilgisi listesini BHMHarcama nesne koleksiyonu olarak tutan nesne</returns>
        //public ObjectArray BHMOdemeHarcamaListesi(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = new HRCOdemeEmriForm().BHMOdemeHarcamaListesi(yil, ay, muhasebe, birim, kur, fon, fin, eko, peb);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        public ObjectArray SurecBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCRaporlarv2().SurecBilgiListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray OnayBelgesiBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCRaporlarv2().OnayBelgesiBilgiListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray OnayBelgesiTertipBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCRaporlarv2().OnayBelgesiTertipBilgiListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray AvansFormBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCRaporlarv2().AvansFormBilgiListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray TeklifEdilenFiyatBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCRaporlarv2().TeklifEdilenFiyatBilgiListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray PiyasaFiyatBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCRaporlarv2().PiyasaFiyatBilgiListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray PiyasaFiyatBilgiToplamListesi(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCRaporlarv2().PiyasaFiyatBilgiToplamListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray KazananFirmaBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCRaporlarv2().KazananFirmaBilgiListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray HakedisBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCRaporlarv2().HakedisBilgiListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray FaturaBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCRaporlarv2().FaturaBilgiListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray TasinirBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCRaporlarv2().KazananFirmaBilgiListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray IhaleIlanBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCRaporlarv2().IhaleIlanBilgiListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray IhaleSozlesmeSonuUcretBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCRaporlarv2().IhaleSozlesmeSonuUcretBilgiListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray MufYaklasikMaliyetListesi(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCRaporlarv2().MufYaklasikMaliyetListele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc MPRevizeNoTalepNoSil(Kullanici kullanan, MPRevizeNoTalepNo bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCMPRevizeNoTalepNo().Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc MPRevizeNoTalepNoKaydet(Kullanici kullanan, MPRevizeNoTalepNo bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCMPRevizeNoTalepNo().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public MPRevizeNoTalepNo MPRevizeNoTalepNoListele(Kullanici kullanan, MPRevizeNoTalepNo kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCMPRevizeNoTalepNo().Listele(kullanan, kriter);
            }
            catch
            {
                return new MPRevizeNoTalepNo();
            }
        }

        public Sonuc DetayHesapla(Kullanici kullanan, int yil, int asama)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return SeyyarGorevC.DetayHesapla((TNS.Veri.VeriErisim)null, kullanan, yil, asama);
        }

        public Sonuc OncekiAsamadanAl(Kullanici kullanan, int hedefYil, int hedefAsama)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return SeyyarGorevC.OncekiAsamadanAl((TNS.Veri.VeriErisim)null, kullanan, hedefYil, hedefAsama);
        }

        public Sonuc DurumDegistirSeyyarGorevRevize(Kullanici kullanan, TNSCollection belgeler)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return SeyyarGorevC.DurumDegistirSeyyarGorevRevize((TNS.Veri.VeriErisim)null, kullanan, belgeler);
        }

        public ObjectArray ListeleSeyyarGorevRevize(Kullanici kullanan, SeyyarGorevRevize kistasForm)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return SeyyarGorevC.ListeleSeyyarGorevRevize((TNS.Veri.VeriErisim)null, kullanan, kistasForm);
        }

        //public ObjectArray HarcamaBirimiListele(Kullanici kullanan, MuhasebeBirim muhasebeBirim, bool kurumsalYapili)
        //{
        //    TNS.KYM.Arac.KulturDegistir(kullanan);
        //    return SeyyarGorevC.HarcamaBirimiListele((TNS.Veri.VeriErisim)null, kullanan, muhasebeBirim, kurumsalYapili);
        //}

        public Sonuc DurumAtaSeyyarGorevRevize(Kullanici kullanan, SeyyarGorevRevize revForm)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return SeyyarGorevC.DurumAtaSeyyarGorevRevize((TNS.Veri.VeriErisim)null, kullanan, revForm);
        }

        public BKD.TavanGenel SeyyarGorevBilgiBul(Kullanici kullanan, int yil, string ilKod, int haricBelgeNoDagitilan)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return SeyyarGorevC.SeyyarGorevBilgiBul((TNS.Veri.VeriErisim)null, kullanan, yil, ilKod, haricBelgeNoDagitilan);
        }

        //public ObjectArray SeyyarGorevBilgiListele(Kullanici kullanan, int yil, string ilKod, string belgeNo, int haricBelgeNoDagitilan)
        //{
        //    TNS.KYM.Arac.KulturDegistir(kullanan);
        //    TNSCollection ilKodlar = null;
        //    if (!string.IsNullOrWhiteSpace(ilKod))
        //    {
        //        ilKodlar = new TNSCollection();
        //        ilKodlar.Add(ilKod);
        //    }
        //    return SeyyarGorevC.SeyyarGorevBilgiBul(null, kullanan, yil, ilKodlar, belgeNo, haricBelgeNoDagitilan);
        //}

        public ObjectArray SeyyarGorevBilgiBulBirim(Kullanici kullanan, int yil, TNSCollection kyKodListesi, int belgeNo, bool onaylilar)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return SeyyarGorevC.ListeleSeyyarBirim((TNS.Veri.VeriErisim)null, kullanan, yil, kyKodListesi, belgeNo, onaylilar);
        }

        public ObjectArray SeyyarGorevIslemListele(Kullanici kullanan, SeyyarGorevRevize kistasForm, bool onaylilar)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return SeyyarGorevC.SeyyarGorevIslemListele(kullanan, kistasForm, onaylilar);
        }

        /// <summary>
        /// SeyyarGorevRevize nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SeyyarGorevRevize yapısında SeyyarGorevRevize:  bilgilerini tutan değişken</param>
        public SeyyarGorevRevize OkuSeyyarGorevRevize(Kullanici kullanan, int yil, ENUMRevizeFormBelgeTur belgeTur, int belgeNo, bool tumIl, bool onaylilar)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return SeyyarGorevC.OkuSeyyarGorevRevizeForm((TNS.Veri.VeriErisim)null, kullanan, yil, belgeTur, belgeNo, true, true, tumIl, onaylilar);
        }

        /// <summary>
        /// SeyyarGorevRevize nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SeyyarGorevRevize yapısında SeyyarGorevRevize:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetSeyyarGorevRevize(Kullanici kullanan, SeyyarGorevRevize form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return SeyyarGorevC.KaydetSeyyarGorevRevize((TNS.Veri.VeriErisim)null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public ObjectArray ListeleSeyyarGorevIlTavan(Kullanici kullanan, int yil, string senaryoKod, BKD.TavanGenel kistas)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return SeyyarGorevC.ListeleSeyyarIlTavan((TNS.Veri.VeriErisim)null, kullanan, yil, senaryoKod, null, true, true);
        }

        //public ObjectArray ListeleSeyyarGorevDOSE(Kullanici kullanan, int yil, string senaryoKod, BHMv2.TavanGenel kistas)
        //{
        //    TNS.KYM.Arac.KulturDegistir(kullanan);
        //    return SeyyarGorevC.ListeleSeyyarGorevDOSE((TNS.Veri.VeriErisim)null, kullanan, yil, senaryoKod, kistas);
        //}

        public SeyyarGorevDurumAna SeyyarGorevDurumBul(Kullanici kullanan, int yil, int saymanlik, string birim, string kurKod, int islemKod, int surecYil, string surecNo, string oeNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return SeyyarGorevC.SeyyarGorevDurumBul(null, kullanan, yil, saymanlik, birim, kurKod, islemKod, surecYil, surecNo, oeNo);
        }

        public Sonuc SeyyarGorevKaydet(Kullanici kullanan, int yil, int asama, SeyyarGorevI[] sgler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return SeyyarGorevC.Kaydet(null, kullanan, yil, asama, sgler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc SeyyarGorevSil(Kullanici kullanan, ObjectArray seyyargorevlist)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                //
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray SeyyarGorevListele(Kullanici kullanan, int yil, string harcanabirimi, int asama)
        {
            try
            {
                return SeyyarGorevC.Listele(null, kullanan, yil, harcanabirimi, asama);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc SeyyarGorevOnayKaydet(Kullanici kullanan, SeyyarGorevI sg)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = SeyyarGorevC.OnayKaydet(null, kullanan, sg);
            }
            catch (Exception e)
            {

                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SeyyarGorevOnayKaldir(Kullanici kullanan, SeyyarGorevI sg)
        {

            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = SeyyarGorevC.OnaySil(null, kullanan, sg);
            }
            catch (Exception e)
            {

                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray HarcamaBirimiPersonelListesi(Kullanici kullanan, string harcamaBirmi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = SeyyarGorevC.PersonelListele(null, kullanan, harcamaBirmi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray SeyyarGorevAdamGunTespitListele(Kullanici kullanan, int yil, string harcamabirimi, int asama, bool harcamaOlsun, bool perSayOlsun, bool unvanlarOlsun)
        {
            try
            {
                return SeyyarGorevC.SeyyarGorevAdamGunTespitListele(null, kullanan, yil, harcamabirimi, asama, harcamaOlsun, perSayOlsun, unvanlarOlsun, false);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc SeyyarGorevAdamGunTespitKaydet(Kullanici kullanan, SeyyarGorevI sg)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = SeyyarGorevC.SeyyarGorevAdamGunTespitKaydet(null, kullanan, sg);
            }
            catch (Exception e)
            {

                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray SeyyarGorevUnvanTespitListele(Kullanici kullanan, int yil, int istihdamSekli, int unvanKod, int asama, string harcamaBirimi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = SeyyarGorevC.SeyyarGorevUnvanTespitListele(null, kullanan, yil, istihdamSekli, unvanKod, asama, harcamaBirimi);
            }
            catch (Exception e)
            {

                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SeyyarGorevUnvanTespitKaydet(Kullanici kullanan, SeyyarGorevI sg)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = SeyyarGorevC.SeyyarGorevUnvanTespitKaydet(null, kullanan, sg);
            }
            catch (Exception e)
            {

                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray PersonelHarcamaBirimiListele(Kullanici kullanan, int istihdamSekli, int unvanKodu)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = SeyyarGorevC.PersonelHarcamaBirimiListele(null, kullanan, istihdamSekli, unvanKodu);
            }
            catch (Exception e)
            {

                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray RaporIhaleFirmaListele(Kullanici kullanan, RaporIhaleFirma kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                TNSDateTime kontrolTarih = new TNSDateTime();
                donusDeger = new HRCFaturaForm().RaporIhaleFirmaListele(kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc TurkcellLogKaydet(Kullanici kullanan, TurkcellLog bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTurkcellLog().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray TurkcellLogListele(Kullanici kullanan, TurkcellLog kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTurkcellLog().Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public string TurkcellLogDosyaAdiAl(Kullanici kullanan)
        {
            string donusDeger = string.Empty;
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCTurkcellLog().DosyaAdiAl(kullanan);
            }
            catch
            {

            }
            return donusDeger;
        }

        public Sonuc TeminatMektubuKaydet(Kullanici kullanan, TeminatMektubu bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCTeminatMektubu.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        public Sonuc TeminatMektubuGirisOdemeEmriGuncelle(Kullanici kullanan, int dosyaYil, string dosyaNo, int belgeYil, string belgeNo, TNSDateTime belgeTarihi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCTeminatMektubu.GuncelleGirisOdemeEmri(null, kullanan, dosyaYil, dosyaNo, belgeYil, belgeNo, belgeTarihi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        public Sonuc TeminatMektubuCikisOdemeEmriGuncelle(Kullanici kullanan, int cikisKod, int belgeYil, string belgeNo, TNSDateTime belgeTarihi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCTeminatMektubu.GuncelleCikisOdemeEmri(null, kullanan, cikisKod, belgeYil, belgeNo, belgeTarihi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        public Sonuc TeminatMektubuSil(Kullanici kullanan, TeminatMektubu bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCTeminatMektubu.Sil(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        public ObjectArray TeminatMektubuListele(Kullanici kullanan, TeminatMektubu bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCTeminatMektubu.Listele(null, kullanan, bilgi, false);
            }
            catch (Exception e)
            {
                ObjectArray sonuclar = new ObjectArray();
                sonuclar.sonuc.DegerAta(e.Message);
                return sonuclar;
            }
        }

        public ObjectArray TeminatMektubuCikisListele(Kullanici kullanan, TeminatMektubuCikisBilgileri bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCTeminatMektubu.ListeleCikis(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                ObjectArray sonuclar = new ObjectArray();
                sonuclar.sonuc.DegerAta(e.Message);
                return sonuclar;
            }
        }

        public Sonuc TeminatMektubuUyariGonder(Kullanici kullanan)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCTeminatMektubu.UyariGonder(null, kullanan);
            }
            catch (Exception e)
            {
                return new Sonuc(e.Message);
            }
        }

        public Sonuc SatinAlmaTalepFormKaydet(Kullanici kullanan, SatinAlmaTalepForm bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCUAStf().Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public SatinAlmaTalepIhtiyacBaslik SatinAlmaTalepIhtiyacBaslikAc(Kullanici kullanan, int yil, string formKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return new HRCUAStf().IhtiyacBaslikAc(kullanan, yil, formKod);
        }

        public void IhtiyacSatiriAdDegistir(Kullanici kullanan, int yil, string formKod, int siraNo, string yeniDeger)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            new HRCUAStf().IhtiyacSatiriAdDegistir(kullanan, yil, formKod, siraNo, yeniDeger);
        }

        public Sonuc IhtiyacSatiriSil(Kullanici kullanan, int yil, string formKod, int siraNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return new HRCUAStf().IhtiyacSatiriSil(kullanan, yil, formKod, siraNo);
        }

        public Sonuc IhtiyacSatiriDetayKaydet(Kullanici kullanan, int yil, string formKod, int siraNo, TNSCollection ihtiyacListesi)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return new HRCUAStf().DetayKaydet(kullanan, yil, formKod, siraNo, ihtiyacListesi);
        }

        public ObjectArray SatinAlmaTalepListeleBaslik(Kullanici kullanan, int yil, string formKod, int siraNo, bool detayVer)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return new HRCUAStf().ListeleBaslik(null, kullanan, yil, formKod, siraNo, detayVer);
        }

        public ObjectArray SatinAlmaTalepFormListele(Kullanici kullanan, SatinAlmaTalepForm kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCUAStf().Listele(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc SatinAlmaTalepFormDurumGuncelle(Kullanici kullanan, int yil, string formKod, ENUMSatinalmaTalepFormDurum durum)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCUAStf().DurumGuncelle(kullanan, yil, formKod, durum);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SatinAlmaTalepFormSurecBilgiGuncelle(Kullanici kullanan, int yil, string formKod, int surecYil, string surecNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = new HRCUAStf().SurecBilgiGuncelle(kullanan, yil, formKod, surecYil, surecNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray SatinAlmaTalepFormSorgula(Kullanici kullanan, SatinAlmaTalepFormSorgu kriter, bool SureceBaglanacakAvansliAlimMi, string surecNo)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCUAStf().Sorgula(kullanan, kriter, SureceBaglanacakAvansliAlimMi, surecNo);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public TNSCollection SatinAlmaTalepFormIhtiyacDetayListele(Kullanici kullanan, int yil, string formKod, int siraNo = 0)
        {
            return new HRCUAStf().ListeleDetay(null, kullanan, yil, formKod, siraNo);
        }

        public TNSCollection SatinAlmaTalepFormIhtiyacDetayListeleSurec(Kullanici kullanan, int yil, string surecNo, int siraNo = 0)
        {
            return new HRCUAStf().ListeleDetay(null, kullanan, yil, surecNo, siraNo);
        }

        public Sonuc SatinAlmaTalepFormTeklifKaydet(Kullanici kullanan, int yil, string surecNo, int firmaKod, int tip, TNSCollection bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCUAStf().KaydetTeklif(kullanan, yil, surecNo, firmaKod, tip, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc TeklifMalzemeDetaySil(Kullanici kullanan, int yil, string surecNo, int firmaKod, int tip, int siraNo)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCUAStf().TeklifMalzemeDetaySil(null, kullanan, yil, surecNo, firmaKod, tip, siraNo);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray SatinAlmaTalepFormTeklifListele(Kullanici kullanan, int yil, string surecNo, int firmaKod, int tip, int siraNo)
        {

            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCUAStf().ListeleTeklif(kullanan, yil, surecNo, firmaKod, tip, siraNo);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc GeciciGorevYollukTalepBaslikKaydet(Kullanici kullanan, GeciciGorevYollukTalepBaslik bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new PSLGeciciGorevYollukTalep().Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray GeciciGorevYollukTalepBaslikListele(Kullanici kullanan, GeciciGorevYollukTalepBaslik kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new PSLGeciciGorevYollukTalep().Listele(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc = new Sonuc(false, e.ToString(), "");
                return donusDeger;
            }
        }

        public Sonuc GeciciGorevYollukTalepOnaylandi(Kullanici kullanan, GeciciGorevYollukTalepBaslik form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new PSLGeciciGorevYollukTalep().Onaylandi(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc IhtiyacPlaniKaydet(Kullanici kullanan, ObjectArray ihtiyacPlani)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new HRCUAIhtiyacPlani().Kaydet(kullanan, ihtiyacPlani);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc IhtiyacPlaniKaydet(Kullanici kullanan, IhtiyacPlaniSatir ihtiyacPlani)
        {
            try
            {
                return new HRCUAIhtiyacPlani().Kaydet(kullanan, ihtiyacPlani);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc IhtiyacPlaniSil(Kullanici kullanan, IhtiyacPlaniSatir ihtiyacPlani)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new HRCUAIhtiyacPlani().Sil(kullanan, ihtiyacPlani);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc IhtiyacPlaniSil(Kullanici kullanan, ObjectArray ihtiyacPlani)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new HRCUAIhtiyacPlani().Sil(kullanan, ihtiyacPlani);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray IhtiyacPlaniListele(Kullanici kullanan, IhtiyacPlaniSatir ihtiyacPlaniSatir)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new HRCUAIhtiyacPlani().Listele(kullanan, ihtiyacPlaniSatir);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public string TalepNoBul(int yil, string surecNo)
        {
            return new HRCUAStf().TalepNoBul(yil, surecNo);
        }

        public int HizmetAlimTurBul(int isKod)
        {
            return new HRCIs().HizmetAlimTurBul(isKod);
        }

        public string IsEkoKodBul(int isKod)
        {
            return new HRCIs().IsEkoKodBul(isKod);
        }

        public Sonuc UAAvansTalepKaydet(Kullanici kullanan, UAAvansTalep bilgi)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return UAAvans.TalepKaydet(null, kullanan, bilgi);
        }

        public UAAvansTalep UAAvansTalepOku(Kullanici kullanan, UAAvansTalep kriter)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return UAAvans.TalepOku(null, kullanan, kriter);
        }

        public Sonuc UAAvansTalepOEIliskiKur(Kullanici kullanan, int surecYil, string surecNo, string odemeEmriNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return UAAvans.OEIliskiKur(null, kullanan, surecYil, surecNo, odemeEmriNo);
        }

        public Sonuc UAAvansKapatmaOEIliskiKur(Kullanici kullanan, int surecYil, string surecNo, string odemeEmriNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return UAAvans.KapatmaOEIliskiKur(null, kullanan, surecYil, surecNo, odemeEmriNo);
        }

        public Sonuc UAAvansKalanKapatmaOEIliskiKur(Kullanici kullanan, int surecYil, string surecNo, string kalanOdemeEmriNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return UAAvans.KapatmaKalanOEIliskiKur(null, kullanan, surecYil, surecNo, kalanOdemeEmriNo);
        }

        public Sonuc UAAvansTalepKapat(Kullanici kullanan, UAAvansKapatma bilgi)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return UAAvans.TalepKapat(null, kullanan, bilgi);
        }

        public UAAvansKapatma UAAvansKapatmaOku(Kullanici kullanan, UAAvansKapatma bilgi)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return UAAvans.AvansKapatmaOku(null, kullanan, bilgi);
        }

        public double UAAvansFaturaTopla(Kullanici kullanan, UAAvansTalep kriter)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return UAAvans.FaturaTopla(null, kullanan, kriter.surecYil, kriter.surecNo);
        }

        public OnMaliKontrolAna OnMaliKontrolListele(Kullanici kullanan, int surecYil, string surecNo, ENUMOnMaliKontrolTur onMaliKontrolTur, string odemeEmriNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return HRCOnMaliKontrol.Listele(null, kullanan, surecYil, surecNo, onMaliKontrolTur, odemeEmriNo);
        }

        public Sonuc OnMaliKontrolKaydet(Kullanici kullanan, OnMaliKontrolAna omk)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return HRCOnMaliKontrol.Kaydet(null, kullanan, omk);
        }

        /// <summary>
        /// KismiGeciciKabul nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KismiGeciciKabul yapısında KismiGeciciKabul: bilgilerini tutan değişken</param>
        public KismiGeciciKabul KismiGeciciKabulOku(Kullanici kullanan, KismiGeciciKabul form)
        {
            return HRCkismiGeciciKabul.Oku(null, kullanan, form);
        }

        /// <summary>
        /// KismiGeciciKabul nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KismiGeciciKabul yapısında KismiGeciciKabul: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KismiGeciciKabulKaydet(Kullanici kullanan, KismiGeciciKabul form)
        {
            try
            {
                return HRCkismiGeciciKabul.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// IhaleKabul nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IhaleKabul yapısında IhaleKabul: bilgilerini tutan değişken</param>
        public IhaleKabul[] KabulFormListele(Kullanici kullanan, IhaleKabul form)
        {
            return HRCkabulForm.Listele(null, kullanan, form, false);
        }

        /// <summary>
        /// IhaleKabul nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IhaleKabul yapısında IhaleKabul: bilgilerini tutan değişken</param>
        public IhaleKabul KabulFormOku(Kullanici kullanan, IhaleKabul form)
        {
            return HRCkabulForm.Oku(null, kullanan, form);
        }

        /// <summary>
        /// IhaleKabul nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IhaleKabul yapısında IhaleKabul: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KabulFormKaydet(Kullanici kullanan, IhaleKabul form)
        {
            try
            {
                return HRCkabulForm.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc KabulFormMalzemeKaydet(Kullanici kullanan, int surecYil, string surecNo, int tutanakNo, IhaleKabulMalzeme[] malListesi)
        {
            try
            {
                return HRCkabulForm.MalzemeKaydet(null, kullanan, surecYil, surecNo, tutanakNo, malListesi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public IhaleKabulMalzeme[] KabulFormMalzemeListele(Kullanici kullanan, int surecYil, string surecNo, int tutanakNo)
        {
            return HRCkabulForm.MalzemeListele(null, kullanan, surecYil, surecNo, tutanakNo);
        }

        /// <summary>
        /// CalisanBordroAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CalisanBordroAna yapısında CalisanBordroAna: bilgilerini tutan değişken</param>
        public CalisanBordroAna CalisanBordroOku(Kullanici kullanan, CalisanBordroAna form)
        {
            return HRCCalisanBordro.Oku(null, kullanan, form);
        }
        /// <summary>
        /// Çalışanların Bordrolarını viewden okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tarih"> Kullanıcıdan alınan bordro tarihi</param>
        /// <returns>İşlem sonucunu döndüren, CalisanBordro sınıfında nesne</returns>
        public CalisanBordroAna BordroViewOku(Kullanici kullanan, string tarih, string kume)
        {
            return HRCCalisanBordro.BordroViewOku(null, kullanan, tarih, kume);
        }

        /// <summary>
        /// CalisanBordroAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CalisanBordroAna yapısında CalisanBordroAna: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc CalisanBordroKaydet(Kullanici kullanan, CalisanBordroAna form)
        {
            try
            {
                return HRCCalisanBordro.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// CalisanBordroGelirKesinti nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CalisanBordroGelirKesinti yapısında CalisanBordroGelirKesinti: bilgilerini tutan değişken</param>
        public CalisanBordroGelirKesinti[] CalisanBordroGelirKesintiListele(Kullanici kullanan)
        {
            return HRCCalisanBordroGelirKesinti.Listele(null, kullanan);
        }

        /// <summary>
        /// CalisanBordroGelirKesinti nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CalisanBordroGelirKesinti yapısında CalisanBordroGelirKesinti: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc CalisanBordroGelirKesintiKaydet(Kullanici kullanan, CalisanBordroGelirKesinti form)
        {
            try
            {
                return HRCCalisanBordroGelirKesinti.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc BordroOdemeEmriKaydet(Kullanici kullanan, string surecNo, int surecYil, string olusanOdemeEmriNo)
        {
            try
            {
                return HRCCalisanBordro.BordroOdemeEmriKaydet(null, kullanan, surecNo, surecYil, olusanOdemeEmriNo);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc SurecYetkisiVarMi(Kullanici kullanan, int surecYil, string surecNo, ENUMKullaniciYetkiTipi yetkiTipi)
        {
            return new TNS.HRC.IsKurallari.HRCSurec().SurecYetkisiVarMi(null, kullanan, surecYil, surecNo, yetkiTipi);
        }
        /// <summary>
        /// PSLGeciciGorevUcak nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PSLGeciciGorevUcak yapısında PSLGeciciGorevUcak:  bilgilerini tutan değişken</param>
        public PSLGeciciGorevUcak[] ListelePSLGeciciGorevUcak(Kullanici kullanan, PSLGeciciGorevUcak form)
        {
            return HRCPSLGeciciGorevUcak.Listele(null, kullanan, form);
        }


        /// <summary>
        /// PSLGeciciGorevUcak nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PSLGeciciGorevUcak yapısında PSLGeciciGorevUcak:  bilgilerini tutan değişken</param>
        public PSLGeciciGorevUcak OkuPSLGeciciGorevUcak(Kullanici kullanan, PSLGeciciGorevUcak form)
        {
            return HRCPSLGeciciGorevUcak.Oku(null, kullanan, form);
        }


        /// <summary>
        /// PSLGeciciGorevUcak nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PSLGeciciGorevUcak yapısında PSLGeciciGorevUcak:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetPSLGeciciGorevUcak(Kullanici kullanan, PSLGeciciGorevUcak form)
        {
            try
            {
                return HRCPSLGeciciGorevUcak.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc BordroBankaListesiKaydet(Kullanici kullanan, string surecNo, int surecYil, TNSDateTime bankaOdemeGun, int bankaOdemeSiraNo)
        {
            try
            {
                return HRCCalisanBordro.BordroBankaListesiKaydet(null, kullanan, surecNo, surecYil, bankaOdemeGun, bankaOdemeSiraNo);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Arguman olarak verilen, çeşitli ödemeler ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc CesitliOdemeAvansOdemeEmriNoKaydet(Kullanici kullanan, CesitliOdeme bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCCesitliOdemeBordro().AvansOdemeEmriNoKaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc CesitliOdemeAvansBankaListesiKaydet(Kullanici kullanan, CesitliOdeme co, TNSDateTime avansBankaOdemeGun, int avansBankaOdemeSiraNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return new HRCCesitliOdemeBordro().CesitliOdemeAvansBankaListesiKaydet(kullanan, co, avansBankaOdemeGun, avansBankaOdemeSiraNo);
        }

        /// <summary>
        /// DovizOdeme nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DovizOdeme yapısında DovizOdeme:  bilgilerini tutan değişken</param>
        public DovizOdeme[] ListDovizOdeme(Kullanici kullanan, DovizOdeme form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return HRCDovizOdeme.List(null, kullanan, form);
        }

        /// <summary>
        /// DovizOdeme nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DovizOdeme yapısında DovizOdeme:  bilgilerini tutan değişken</param>
        public DovizOdeme GetDovizOdeme(Kullanici kullanan, DovizOdeme form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return HRCDovizOdeme.Get(null, kullanan, form);
        }

        /// <summary>
        /// DovizOdeme nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DovizOdeme yapısında DovizOdeme:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SaveDovizOdeme(Kullanici kullanan, DovizOdeme form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCDovizOdeme.Save(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc DovizOdemeOdemeEmriNoKaydet(Kullanici kullanan, DovizOdeme form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCDovizOdeme.DovizOdemeOdemeEmriNoKaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// AvukatOdemesi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AvukatOdemesi yapısında AvukatOdemesi:  bilgilerini tutan değişken</param>
        public AvukatOdemesi[] ListeleAvukatOdemesi(Kullanici kullanan, AvukatOdemesi form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return HRCAvukatOdemesi.Listele(null, kullanan, form);
        }

        /// <summary>
        /// AvukatOdemesi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AvukatOdemesi yapısında AvukatOdemesi:  bilgilerini tutan değişken</param>
        public AvukatOdemesi OkuAvukatOdemesi(Kullanici kullanan, AvukatOdemesi form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return HRCAvukatOdemesi.Oku(null, kullanan, form);
        }

        /// <summary>
        /// AvukatOdemesi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AvukatOdemesi yapısında AvukatOdemesi:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetAvukatOdemesi(Kullanici kullanan, AvukatOdemesi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCAvukatOdemesi.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc AvukatOdemesiOdemeEmriNoKaydet(Kullanici kullanan, AvukatOdemesi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCAvukatOdemesi.AvukatOdemesiOdemeEmriNoKaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public SurecOzetBilgi SurecOzetBilgiBul(Kullanici kullanan, int surecYil, string surecNo)
        {
            return HRCSurec.SurecOzetBilgiBul(null, kullanan, surecYil, surecNo);
        }

        public Sonuc YollukBelgeSil(Kullanici kullanan, int yil, string surecNo)
        {
            return PSLGeciciGorevYolluk.YollukBelgeSil(null, kullanan, yil, surecNo);
        }

        public Sonuc BordroYapiSakla(Kullanici kullanan, BordroYapi bordroYapi)
        {
            return BordroYapiIslem.BordroYapiSakla(null, kullanan, bordroYapi);
        }

        public BordroYapi BordroYapiOku(Kullanici kullanan, int bordroYapiTur, int bordroIslemTur, bool sadeceKedisi)
        {
            return BordroYapiIslem.BordroYapiOku(null, kullanan, bordroYapiTur, bordroIslemTur, sadeceKedisi, null, null, false);
        }

        public ObjectArray BordroYapiDegiskenListele(Kullanici kullanan, int islemTur)
        {
            return BordroYapiIslem.BordroYapiDegiskenListele(null, kullanan, islemTur);
        }

        public ObjectArray BordroYapiDegiskenOku(Kullanici kullanan, int islemTur, string formulAd)
        {
            return BordroYapiIslem.BordroYapiDegiskenOku(null, kullanan, islemTur, formulAd);
        }

        public CalisanDinamikBordro CalisanDinamikBordroOku(Kullanici kullanan, BordroYapi bordroYapi, BordroKistas bordroKistas)
        {
            return BordroYapiIslem.CalisanDinamikBordroOku(null, kullanan, bordroYapi, bordroKistas, false);
        }

        public CalisanDinamikBordroDonem OncekiDonemBul(int surecYil, string surecNo)
        {
            return BordroYapiIslem.OncekiDonemBul(null, surecYil, surecNo);
        }

        public ObjectArray CalisanDinamikBordroListele(Kullanici kullanan, BordroKistas bordroKistas)
        {
            return BordroYapiIslem.CalisanDinamikBordroListele(null, kullanan, bordroKistas);
        }

        public Sonuc CalisanDinamikBordroSil(Kullanici kullanan, int surecYil, string surecNo, BordroYapi bordroYapi, TNSCollection tcListe)
        {
            return BordroYapiIslem.CalisanDinamikBordroSil(null, kullanan, surecYil, surecNo, bordroYapi, tcListe);
        }

        public Sonuc CalisanDinamikBordroKaydet(Kullanici kullanan, int yil, string surecNo, BordroYapi bordroYapi, System.Collections.ArrayList satirlar)
        {
            return BordroYapiIslem.CalisanDinamikBordroKaydet(null, kullanan, yil, surecNo, bordroYapi, satirlar);
        }

        public Sonuc CalisanDinamikBordroOnayliMi(Kullanici kullanan, int yil, string surecNo, BordroYapi bordroYapi)
        {
            try
            {
                CalisanDinamikBordroDonem cdbdForm;
                return BordroYapiIslem.CalisanDinamikBordroOnayliMi(null, kullanan, yil, surecNo, bordroYapi, out cdbdForm);
            }
            catch (Exception ex)
            {
                return new Sonuc(ex.Message);
            }
        }

        public Sonuc CalisanDinamikBordroOnayla(Kullanici kullanan, int yil, string surecNo, BordroYapi bordroYapi, int onayKod)
        {
            return BordroYapiIslem.CalisanDinamikBordroOnayla(null, kullanan, yil, surecNo, bordroYapi, onayKod);
        }

        public Sonuc BordroMailiGonderEkle(Kullanici kullanan, int surecYil, string surecNo, BordroYapi bordroYapi, TNSCollection tcListe)
        {
            return BordroYapiIslem.BordroMailiGonderEkle(null, kullanan, surecYil, surecNo, bordroYapi, tcListe);
        }

        public ObjectArray BordroMailiBekleyenListele(Kullanici kullanan)
        {
            return BordroYapiIslem.BordroMailiBekleyenListele(null, kullanan);
        }

        public Sonuc BordroMailBirak(Kullanici kullanan, BordroMail bm)
        {
            return BordroYapiIslem.BordroMailBirak(null, kullanan, bm);
        }

        public Sonuc BordroSablonSakla(Kullanici kullanan, BordroSablonDosya bsd, bool yeni)
        {
            return BordroYapiIslem.BordroSablonSakla(null, kullanan, bsd, yeni);
        }

        public Sonuc BordroSablonSil(Kullanici kullanan, BordroSablonDosya bsd)
        {
            return BordroYapiIslem.BordroSablonSil(null, kullanan, bsd);
        }

        public ObjectArray BordroSablonListele(Kullanici kullanan, int bordroTur, int islemTur, ENUMUCDURUM sayfa, ENUMUCDURUM ortakRapor)
        {
            return BordroYapiIslem.BordroSablonListele(null, kullanan, bordroTur, islemTur, sayfa, ortakRapor);
        }


        public ObjectArray YuzdeOnListele(Kullanici kullanan, int yil)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return HRCOnayForm.YuzdeOnListele(null, kullanan, yil);
        }

        public FaturaTum[] ListeleFaturaTum(Kullanici kullanan, FaturaTum form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return HRCFaturaTum.Listele(this, null, kullanan, form);
        }

        /// <summary>
        /// DogrudanTeminPlan nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DogrudanTeminPlan yapısında DogrudanTeminPlan:  bilgilerini tutan değişken</param>
        public ObjectArray ListeleDogrudanTeminPlan(Kullanici kullanan, DogrudanTeminPlan form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return HRCDogrudanTeminPlan.Listele(null, kullanan, form);
        }

        public ObjectArray IstatistikDogrudanTeminPlan(Kullanici kullanan, int yil, string senaryoKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return HRCDogrudanTeminPlan.IstatistikUret(null, kullanan, yil, senaryoKod);
        }

        /// <summary>
        /// DogrudanTeminPlan nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DogrudanTeminPlan yapısında DogrudanTeminPlan:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetDogrudanTeminPlan(Kullanici kullanan, int yil, string senaryoKod, int saymanlik, string birim, string harcamaTuru, DogrudanTeminPlan[] form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCDogrudanTeminPlan.Kaydet(null, kullanan, yil, senaryoKod, saymanlik, birim, harcamaTuru, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// YuzdeOnKapatAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YuzdeOnKapatAna yapısında YuzdeOnKapatAna:  bilgilerini tutan değişken</param>
        public ObjectArray ListeleYuzdeOnKapat(Kullanici kullanan, YuzdeOnKapat form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return HRCYuzdeOnKapat.Listele(null, kullanan, form);
        }

        /// <summary>
        /// YuzdeOnKapatAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YuzdeOnKapatAna yapısında YuzdeOnKapatAna:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetYuzdeOnKapat(Kullanici kullanan, YuzdeOnKapat form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCYuzdeOnKapat.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc YuzdeOnAcikMi(Kullanici kullanan, int yil, string ihaleMaddeAdi, string malHizmetYapim, TNSCollection tertipler, int saymanlikKodu, string birimKodu)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCYuzdeOnKapat.YuzdeOnAcikMi(null, kullanan, yil, ihaleMaddeAdi, malHizmetYapim, tertipler, saymanlikKodu, birimKodu);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectVAN CalisanDinamikBordroDonemOku(Kullanici kullanan, CalisanDinamikBordroDonem calisanDinamikBordroDonem)
        {
            return BordroYapiIslem.DonemOku(null, kullanan, calisanDinamikBordroDonem, null);
        }

        public Sonuc CalisanDinamikBordroDonemKaydet(Kullanici kullanan, CalisanDinamikBordroDonem calisanDinamikBordroDonem)
        {
            return BordroYapiIslem.DonemKaydet(null, kullanan, calisanDinamikBordroDonem);
        }

        public BordroMuhasebeKayitlari CalisanDinamikBordroHesapOku(Kullanici kullanan, int yil, string surecNo, TNSCollection bordroTurler, int bordroIslemTur, CalisanDinamikBordroDonem bordroDonem, int avansIslemKod, string odemeEmriHesapKod)
        {
            return BordroYapiIslem.HesapOku(null, kullanan, yil, surecNo, bordroTurler, bordroIslemTur, bordroDonem, avansIslemKod, odemeEmriHesapKod);
        }

        public Sonuc CalisanDinamikBordroVeriAktar(Kullanici kullanan, int yil, string surecNo, TNSCollection bordroTurler, int bordroIslemTur, int aktar)
        {
            return BordroYapiIslem.BordroVeriAktar(null, kullanan, yil, surecNo, bordroTurler, bordroIslemTur, aktar);
        }

        /// <summary>
        /// BordroTabloTanim nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroTabloTanim yapısında BordroTabloTanim: Bordro Tablo Tanım bilgilerini tutan değişken</param>
        public ObjectArray ListeleBordroTablo(Kullanici kullanan, BordroTabloDetay form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCBordroTablo.Listele(null, kullanan, form, true);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// BordroTabloTanim nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroTabloTanim yapısında BordroTabloTanim: Bordro Tablo Tanım bilgilerini tutan değişken</param>
        public decimal OkuBordroTablo(Kullanici kullanan, int tabloKod, int unvanKod, int derece, int yil)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCBordroTablo.Oku(null, kullanan, tabloKod, unvanKod, derece, yil);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// BordroTabloTanim nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroTabloTanim yapısında BordroTabloTanim: Bordro Tablo Tanım bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public ObjectArray ListeleBordroTabloAna(Kullanici kullanan)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCBordroTablo.ListeleTanim(null, kullanan);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// BordroTabloTanim nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroTabloTanim yapısında BordroTabloTanim: Bordro Tablo Tanım bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetBordroTablo(Kullanici kullanan, int tabloKod, int yil, int unvanKod, TNSCollection degerler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCBordroTablo.Kaydet(null, kullanan, tabloKod, yil, unvanKod, degerler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// BordroParamAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroParamAna yapısında BordroParamAna: Bordro Param Ana bilgilerini tutan değişken</param>
        public ObjectArray ListelePersonelBordroParam(Kullanici kullanan)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCPersonelBordroParam.Listele(null, kullanan);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// BordroParamAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroParamAna yapısında BordroParamAna: Bordro Param Ana bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetPersonelBordroParam(Kullanici kullanan, TNSCollection degiskenler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCPersonelBordroParam.Kaydet(null, kullanan, degiskenler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// BordroIslemTur nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroIslemTur yapısında BordroIslemTur: Bordro İşlem Tür bilgilerini tutan değişken</param>
        public ObjectVAN OkuBordroIslemTur(Kullanici kullanan, BordroIslemTur form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BordroYapiIslem.BordroIslemTurOku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// BordroIslemTur nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroIslemTur yapısında BordroIslemTur: Bordro İşlem Tür bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetBordroIslemTur(Kullanici kullanan, BordroIslemTur form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BordroYapiIslem.BordroIslemTurKaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// BordroIslemTur nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroIslemTur yapısında BordroIslemTur: Bordro İşlem Tür bilgilerini tutan değişken</param>
        public ObjectArray BordroIslemTurListele(Kullanici kullanan, BordroIslemTur form)
        {
            return BordroYapiIslem.BordroIslemTurListele(null, kullanan, form);
        }

        public Sonuc BordroTabloArttir(Kullanici kullanan, int tabloKod, int yil, double artisKatsayisi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCBordroTablo.BordroTabloArttir(null, kullanan, tabloKod, yil, artisKatsayisi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc KaydetBordroTabloTanim(Kullanici kullanan, BordroTabloAna tabloTanim)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCBordroTablo.KaydetBordroTabloTanim(null, kullanan, tabloTanim);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc KopyalaBordroTablo(Kullanici kullanan, int yil, int hedefKod, int kaynakKod, int unvanKod, bool onceSil)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCBordroTablo.KopyalaBordroTablo(null, kullanan, yil, hedefKod, kaynakKod, unvanKod, onceSil);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }


        public Sonuc KaydetButceBildirim(Kullanici kullanan, ButceBildirim form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCButceBildirim.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public ButceBildirim[] ListeleButceBildirim(Kullanici kullanan, ButceBildirim form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCButceBildirim.Listele(null, kullanan, form);
            }
            catch
            {
                return null;
            }
        }
        public Sonuc SilButceBildirim(Kullanici kullanan, ButceBildirim form)
        {
            try
            {
                return HRCButceBildirim.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc ManasSatinAlmaIsTanimKaydet(Kullanici kullanan, ManasSatinAlmaIsTanim form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCManasSatinAlmaIsTanim.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public Sonuc ManasSatinAlmaOnayKaydet(Kullanici kullanan, ManasSatinAlmaIsTanim form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCManasSatinAlmaIsTanim.KaydetOnay(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public Sonuc ManasSatinAlmaIsTanimSil(Kullanici kullanan, ManasSatinAlmaIsTanim form)
        {
            try
            {
                return HRCManasSatinAlmaIsTanim.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public ManasSatinAlmaIsTanim[] ManasSatinAlmaIsTanimListele(Kullanici kullanan, ManasSatinAlmaIsTanim form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCManasSatinAlmaIsTanim.Listele(null, kullanan, form);
            }
            catch
            {
                return null;
            }
        }
        public ManasSatinAlmaIsTanim ManasSatinAlmaIsTanimOku(Kullanici kullanan, ManasSatinAlmaIsTanim form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCManasSatinAlmaIsTanim.Oku(null, kullanan, form);
            }
            catch
            {
                return null;
            }
        }

        public Sonuc ManasSatinAlmaGorevliKaydet(Kullanici kullanan, ManasSatinAlmaGorevli[] formList, int satinAlmaKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCManasSatinAlmaGorevli.Kaydet(null, kullanan, formList, satinAlmaKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public ManasSatinAlmaGorevli[] ManasSatinAlmaGorevliListele(Kullanici kullanan, ManasSatinAlmaGorevli form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCManasSatinAlmaGorevli.Listele(null, kullanan, form);
            }
            catch
            {
                return null;
            }
        }

        public Sonuc ManasSatinAlmaUrunKaydet(Kullanici kullanan, ManasSatinAlmaUrun[] formList, int satinAlmaKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCManasSatinAlmaUrun.Kaydet(null, kullanan, formList, satinAlmaKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public ManasSatinAlmaUrun[] ManasSatinAlmaUrunListele(Kullanici kullanan, ManasSatinAlmaUrun form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCManasSatinAlmaUrun.Listele(null, kullanan, form);
            }
            catch
            {
                return null;
            }
        }

        public Sonuc ManasSatinAlmaFirmaKaydet(Kullanici kullanan, ManasSatinAlmaFirma[] formList, int satinAlmaKod, int tur)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCManasSatinAlmaFirma.Kaydet(null, kullanan, formList, satinAlmaKod, tur);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public ManasSatinAlmaFirma[] ManasSatinAlmaFirmaListele(Kullanici kullanan, ManasSatinAlmaFirma form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCManasSatinAlmaFirma.Listele(null, kullanan, form);
            }
            catch
            {
                return null;
            }
        }

        public Sonuc ManasSatinAlmaTeklifKaydet(Kullanici kullanan, ManasSatinAlmaTeklif[] formList, ManasSatinAlmaTeklifForm tf)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCManasSatinAlmaTeklif.Kaydet(null, kullanan, formList, tf);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ManasSatinAlmaTeklifForm ManasSatinAlmaTeklifListele(Kullanici kullanan, ManasSatinAlmaTeklif form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCManasSatinAlmaTeklif.Listele(null, kullanan, form);
            }
            catch
            {
                return null;
            }
        }

        public Sonuc KomisyonUyeSablonKaydet(Kullanici kullanan, KomisyonUyeSablon sablon)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCKomisyonUyeSablon.Kaydet(null, kullanan, sablon);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc KomisyonUyeSablonSil(Kullanici kullanan, KomisyonUyeSablon sablon)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCKomisyonUyeSablon.Sil(null, kullanan, sablon);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray KomisyonUyeSablonListele(Kullanici kullanan, KomisyonUyeSablon sablon)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return new HRCKomisyonUyeSablon().Listele(kullanan, sablon);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc BordroKontrolEt(Kullanici kullanan, int yil, int ay, int muhasebe, string birim, int bordroTur, int bordroIslem)
        {
            return BordroYapiIslem.BordroKontrolEt(null, kullanan, yil, ay, muhasebe, birim, bordroTur, bordroIslem);
        }

        public Sonuc GundelikKaydet(Kullanici kullanan, Gundelik bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCGundelik.Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }
        public ObjectArray GundelikListele(Kullanici kullanan, Gundelik kriter)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCGundelik.Listele(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }
        public Sonuc GundelikSil(Kullanici kullanan, Gundelik bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCGundelik.Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public Sonuc GundelikSonrakiYilaKopyala(Kullanici kullanan, Gundelik bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCGundelik.SonrakiYilaKopyala(kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public ObjectVAN GeciciGorevYollukManasOku(Kullanici kullanan, GeciciGorevYollukManas form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCGundelik.GeciciGorevYollukManasOku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN donusDeger = new ObjectVAN();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        public Sonuc GeciciGorevYollukManasOEIliskilendir(Kullanici kullanan, GeciciGorevYollukManas form, bool avans)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCGundelik.GeciciGorevYollukManasOEIliskilendir(null, kullanan, form, avans);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public Sonuc GeciciGorevYollukManasKaydet(Kullanici kullanan, GeciciGorevYollukManas form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCGundelik.GeciciGorevYollukManasKaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public GeciciGorevYollukManas GeciciGorevYollukManasHesapla(Kullanici kullanan, GeciciGorevYollukManas form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                HRCGundelik.GeciciGorevYollukManasHesapla(null, kullanan, form);
            }
            catch
            {
            }
            return form;
        }


        public Sonuc BordroRaporSablonKaydet(Kullanici kullanan, BordroRaporSablon form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCBordroRaporSablon.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public ObjectArray BordroRaporSablonListele(Kullanici kullanan, BordroRaporSablon sablon)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCBordroRaporSablon.Listele(null, kullanan, sablon);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public ObjectArray PersonelMaliyetHesapla(Kullanici kullanan, string tcKimlikNo, TNSDateTime basTarih, TNSDateTime bitTarih, int ekParam)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCBordroRaporSablon.PersonelMaliyetHesapla(null, kullanan, tcKimlikNo, basTarih, bitTarih, ekParam);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray BordroKisiBorcuBilgiBul(Kullanici kullanan, int personelTip)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HRCBordroRaporSablon.BordroKisiBorcuBilgiBul(null, kullanan, personelTip);
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
        /// DBIkraz nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBIkraz yapısında DBIkraz:  bilgilerini tutan değişken</param>
        public ObjectArray ListeleDBIkrazlar(Kullanici kullanan, DBIkraz form, bool detayOku)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.ListeleDBIkrazlar(null, kullanan, form, detayOku);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }


        /// <summary>
        /// DBIkraz nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBIkraz yapısında DBIkraz:  bilgilerini tutan değişken</param>
        public ObjectVAN OkuDBIkraz(Kullanici kullanan, DBIkraz form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.OkuDBIkraz(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// DBIkraz nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBIkraz yapısında DBIkraz:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetDBIkraz(Kullanici kullanan, DBIkraz form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.KaydetDBIkraz(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// DBBilesen nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBBilesen yapısında DBBilesen:  bilgilerini tutan değişken</param>
        public ObjectArray ListeleDBBilesenler(Kullanici kullanan, DBBilesen form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.ListeleDBBilesenler(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// DBBilesen nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBBilesen yapısında DBBilesen:  bilgilerini tutan değişken</param>
        public ObjectVAN OkuDBBilesen(Kullanici kullanan, DBBilesen form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.OkuDBBilesen(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }


        /// <summary>
        /// DBBilesen nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBBilesen yapısında DBBilesen:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetDBBilesen(Kullanici kullanan, DBBilesen form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.KaydetDBBilesen(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// DBAltBilesen nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBAltBilesen yapısında DBAltBilesen:  bilgilerini tutan değişken</param>
        public ObjectArray ListeleDBAltBilesenler(Kullanici kullanan, DBAltBilesen form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.ListeleDBAltBilesenler(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }


        /// <summary>
        /// DBAltBilesen nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBAltBilesen yapısında DBAltBilesen:  bilgilerini tutan değişken</param>
        public ObjectVAN OkuDBAltBilesen(Kullanici kullanan, DBAltBilesen form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.OkuDBAltBilesen(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// DBAltBilesen nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBAltBilesen yapısında DBAltBilesen:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetDBAltBilesen(Kullanici kullanan, DBAltBilesen form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.KaydetDBAltBilesen(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// DBButceRef nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBButceRef yapısında DBButceRef:  bilgilerini tutan değişken</param>
        public ObjectArray ListeleDBButceRef(Kullanici kullanan, DBButceRef form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.ListeleButceRef(null, kullanan, form, false);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// DBButceRef nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="butceRefID">butceRefID:  lotları okunacak bütçe ref</param>
        public ObjectArray ListeleDBButceRefLot(Kullanici kullanan, int butceRefID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.ListeleButceRefLot(null, kullanan, butceRefID);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// DBButceRef nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBButceRef yapısında DBButceRef:  bilgilerini tutan değişken</param>
        public ObjectVAN OkuDBButceRef(Kullanici kullanan, DBButceRef form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.OkuButceRef(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// DBButceRef nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBButceRef yapısında DBButceRef:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetDBButceRef(Kullanici kullanan, DBButceRef form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.KaydetButceRef(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc SilDBButceRef(Kullanici kullanan, int butceRefID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.SilDBButceRef(null, kullanan, butceRefID);
            }
            catch (Exception e)
            {
                Sonuc s = new Sonuc();
                s.DegerAta(e.ToString());
                return s;
            }
        }

        public ObjectArray ListeleDBSatinAlma(Kullanici kullanan, DBSatinAlma satinAlma, int siralama = 1)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.ListeleSatinAlma(null, kullanan, satinAlma, siralama);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public ObjectVAN OkuDBSatinAlma(Kullanici kullanan, DBSatinAlma satinAlma)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.OkuSatinAlma(null, kullanan, satinAlma);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        public Sonuc KaydetDBSatinAlma(Kullanici kullanan, DBSatinAlma satinAlma)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.KaydetSatinAlma(null, kullanan, satinAlma);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc KaydetDBSatinAlmaGerceklesmePlani(Kullanici kullanan, DBSatinAlmaGerceklesmePlan gercPlan)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.KaydetDBSatinAlmaGerceklesmePlani(null, kullanan, gercPlan);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }




        public ObjectArray ListeleDBDosyaBilgileri(Kullanici kullanan, int kod, string modulKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBIkrazlar.ListeleDBDosyaBilgileri(null, kullanan, kod, modulKod);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public ObjectVAN OkuDBHakedis(Kullanici kullanan, int hakedisID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBHakedisIslemleri.OkuDBHakedis(null, kullanan, hakedisID);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        public Sonuc KaydetDBHakedis(Kullanici kullanan, DBHakedis form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBHakedisIslemleri.KaydetDBHakedis(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public Sonuc GuncelleDBHakedisSahibi(Kullanici kullanan, DBHakedisSahibi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBHakedisIslemleri.GuncelleDBHakedisSahibi(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public Sonuc DBHakedisSahibiNumaraSifirla(Kullanici kullanan, int hakedisID, int siraNo)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBHakedisIslemleri.DBHakedisSahibiNumaraSifirla(null, kullanan, hakedisID, siraNo);
            }
            catch (Exception e) { return new Sonuc(e.ToString()); }
        }

        public Sonuc DBHakedisSifirNumaralariAta(Kullanici kullanan, int hakedisID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBHakedisIslemleri.DBHakedisSifirNumaralariAta(null, kullanan, hakedisID);
            }
            catch (Exception e) { return new Sonuc(e.ToString()); }
        }

        public Sonuc SilDBHakedis(Kullanici kullanan, int hakedisID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBHakedisIslemleri.SilDBHakedis(null, kullanan, hakedisID);
            }
            catch (Exception e)
            {
                Sonuc s = new Sonuc();
                s.DegerAta(e.ToString());
                return s;
            }
        }

        public ObjectArray ListeleDBHakedis(Kullanici kullanan, DBHakedis hakedisKistas, bool detayOku, int siralaYontem)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBHakedisIslemleri.ListeleDBHakedis(null, kullanan, hakedisKistas, detayOku, siralaYontem);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public Sonuc KaydetDBDosyaBilgisi(Kullanici kullanan, DBYuklenebilecekDosyalar form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.KaydetDBDosyaBilgisi(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc DBHakedisDurumDegistir(Kullanici kullanan, int hakedisID, int yeniDurumKod, string gerekce)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBHakedisIslemleri.DBHakedisDurumDegistir(null, kullanan, hakedisID, yeniDurumKod, gerekce);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray ListeleMerkezNumaratorler(Kullanici kullanan, DBMerkezBankasiNumarator dbMerkezBankasiNumarator)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBHakedisIslemleri.ListeleMerkezNumaratorler(null, kullanan, dbMerkezBankasiNumarator, false);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public ObjectVAN OkuMerkezNumarator(Kullanici kullanan, int merkezNumaratorID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBHakedisIslemleri.OkuMerkezNumarator(null, kullanan, merkezNumaratorID);
            }
            catch (Exception e)
            {
                ObjectVAN oa = new ObjectVAN();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public ObjectArray MerkezeGonderilmeyenHakSahipleri(Kullanici kullanan, int altBilesenID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBHakedisIslemleri.MerkezeGonderilmeyenHakSahipleri(null, kullanan, altBilesenID);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public Sonuc YeniMerkezNumarasiAc(Kullanici kullanan, int altBilesenID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBHakedisIslemleri.YeniMerkezNumarasiAc(null, kullanan, altBilesenID);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc MerkezNumaratoreEkle(Kullanici kullanan, int hakedisID, int numarator, int merkezNumaratorID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBHakedisIslemleri.MerkezNumaratoreEkle(null, kullanan, hakedisID, numarator, merkezNumaratorID);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc MerkezNumaratordenCikart(Kullanici kullanan, int hakedisID, int numarator, int merkezNumaratorID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBHakedisIslemleri.MerkezNumaratordenCikart(null, kullanan, hakedisID, numarator, merkezNumaratorID);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray ListeleDBSozlesmeler(Kullanici kullanan, DBSozlesme sozlesme, int siralama = 1)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBIkrazlar.ListeleDBSozlesmeler(null, kullanan, sozlesme, siralama);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public ObjectArray ListeleSozlesmesizOdemeler(Kullanici kullanan, DBSozlesme sozlesme, int siralama = 1)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBIkrazlar.ListeleSozlesmesizOdemeler(null, kullanan, sozlesme, siralama);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public Sonuc KaydetDBSozlesme(Kullanici kullanan, DBSozlesme form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBIkrazlar.KaydetDBSozlesme(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// DBImza nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBImza yapısında DBImza:  bilgilerini tutan değişken</param>
        public ObjectArray ListeleDBImzalar(Kullanici kullanan, DBImza form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBHakedisIslemleri.ListeleDBImzalar(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// DBImza nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBImza yapısında DBImza:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetDBImzalar(Kullanici kullanan, DBImza form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBHakedisIslemleri.KaydetDBImzalar(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc MerkezNumaratorImzaKaydet(Kullanici kullanan, DBMerkezBankasiNumarator form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBHakedisIslemleri.MerkezNumaratorImzaKaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc MerkezNumarasiDurumDegistir(Kullanici kullanan, DBMerkezBankasiNumarator form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBHakedisIslemleri.MerkezNumarasiDurumDegistir(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public ObjectVAN GetOdemePlani(Kullanici kullanan, int sozlesmeID, int odemePlanID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBIkrazlar.DBSozlesmeOdemePlaniOku(null, kullanan, sozlesmeID, odemePlanID);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        public ObjectArray ListeleOdemePlani(Kullanici kullanan, int sozlesmeID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBIkrazlar.ListeleOdemePlani(null, kullanan, sozlesmeID);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public Sonuc KaydetDBOdemePlani(Kullanici kullanan, DBOdemePlani form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBIkrazlar.KaydetDBSozlesmeOdemePlani(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc DurumDegistirDBOdemePlani(Kullanici kullanan, DBOdemePlani form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBIkrazlar.DurumDegistirDBSozlesmeOdemePlani(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectVAN DBSozlesmeNoAl(Kullanici kullanan, int sozlesmeID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBIkrazlar.DBSozlesmeNoAl(null, kullanan, sozlesmeID);
            }
            catch (Exception e)
            {
                ObjectVAN oa = new ObjectVAN();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public ObjectVAN DBSozlesmeKapat(Kullanici kullanan, int sozlesmeID, int durum, string gerekce)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBIkrazlar.DBSozlesmeKapat(null, kullanan, sozlesmeID, durum, gerekce);
            }
            catch (Exception e)
            {
                ObjectVAN oa = new ObjectVAN();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }
        public Sonuc DBSilSozlesme(Kullanici kullanan, int sozlesmeID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.DBSilSozlesme(null, kullanan, sozlesmeID);
            }
            catch (Exception e)
            {
                Sonuc s = new Sonuc();
                s.DegerAta(e.ToString());
                return s;
            }
        }

        public Sonuc SilDBSatinAlmaPlani(Kullanici kullanan, int satinAlmaID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.SilDBSatinAlmaPlani(null, kullanan, satinAlmaID);
            }
            catch (Exception e)
            {
                Sonuc s = new Sonuc();
                s.DegerAta(e.ToString());
                return s;
            }
        }

        public ObjectArray ListeleDBEkstreler(Kullanici kullanan, TNSDateTime baslangicTarih, TNSDateTime bitisTarih, int ikrazID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBIkrazlar.ListeleDBEkstreler(null, kullanan, baslangicTarih, bitisTarih, ikrazID);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public DBNakitPozisyonu OkuDBNakitPozisyonu(Kullanici kullanan, int ikrazID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.OkuDBNakitPozisyonu(null, kullanan, ikrazID);
            }
            catch (Exception e)
            {
                DBNakitPozisyonu cevap = new DBNakitPozisyonu();
                cevap.sonuc.DegerAta(e.ToString());
                return cevap;
            }
        }

        public Sonuc DBEkstrelerVeriEkle(Kullanici kullanan, DBEkstre ekstre)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);

                return DBIkrazlar.DBEkstrelerVeriEkle(null, kullanan, ekstre);
            }
            catch (Exception e)
            {
                Sonuc s = new Sonuc();
                s.DegerAta(e.ToString());
                return s;
            }
        }

        public Sonuc KaydetDBSGBNumarator(Kullanici kullanan, DBEkstre[] liste)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.KaydetDBSGBNumarator(null, kullanan, liste);
            }
            catch (Exception e)
            {
                Sonuc s = new Sonuc();
                s.DegerAta(e.ToString());
                return s;
            }
        }

        public Sonuc DurumDegistirDBSatinAlmaGerceklesmePlani(Kullanici kullanan, int satinAlmaID, int dagilimID, int durumKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DBIkrazlar.DurumDegistirDBSatinAlmaGerceklesmePlani(null, kullanan, satinAlmaID, dagilimID, durumKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
    }


    public class HRCYetki : TNS.KYM.IYetki
    {
        public Sonuc YetkiKontrolu(Kullanici kullanan, TNS.KYM.ENUMIslemTuru islemTur, string oncekiKaydeden, int saymanlikKod, string birimKod, string kurumsalKodModulkod, string ekBilgi)
        {
            if (!string.IsNullOrWhiteSpace(kurumsalKodModulkod) && !string.IsNullOrWhiteSpace(ekBilgi))
            {
                string[] ekbilgiP = ekBilgi.Split('_');
                int basla = 0;

                while (basla < ekbilgiP[0].Length)
                {
                    char harf = ekbilgiP[0][basla];
                    if (Char.IsNumber(harf)) break;
                    basla++;
                }

                int kod = OrtakFonksiyonlar.ConvertToInt(ekbilgiP[0].Substring(basla), 0);
                if (kod > 0)
                {
                    if (kurumsalKodModulkod.StartsWith("DBH") || kurumsalKodModulkod.StartsWith("HKS"))
                    {
                        ObjectVAN ov = DBHakedisIslemleri.OkuDBHakedis(null, kullanan, kod, false);
                        if (!ov.sonuc.islemSonuc) return ov.sonuc;
                        if (islemTur == KYM.ENUMIslemTuru.SILME || islemTur == KYM.ENUMIslemTuru.KAYIT || islemTur == KYM.ENUMIslemTuru.GUNCELLEME)
                            return DBHakedisIslemleri.HakedisYazilabilirMi((DBHakedis)ov.o, ov.sonuc, kurumsalKodModulkod.StartsWith("HKS"), kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI));
                        return ov.sonuc;
                    }
                    else if (kurumsalKodModulkod.StartsWith("DBS"))
                    {
                        ObjectVAN ov = DBIkrazlar.OkuDBSozlesme(null, kullanan, kod);
                        return ov.sonuc;
                    }
                    else if (kurumsalKodModulkod.StartsWith("DBB"))
                    {
                        ObjectVAN ov = DBIkrazlar.OkuButceRef(null, kullanan, new DBButceRef() { butceRefID = kod });
                        return ov.sonuc;
                    }
                    else if (kurumsalKodModulkod.StartsWith("DBİ"))
                    {
                        if (DBIkrazlar.YazmaYetkisiVarMi(kullanan, 0))
                            return new Sonuc();
                    }
                }
            }
            return new Sonuc("İşlemi Yapmaya Yetkiniz Bulunmamaktadır!");
        }
    }
}
