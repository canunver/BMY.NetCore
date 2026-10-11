using System;
using System.Collections.Generic;
using System.Text;
using TNS.YAT;
using TNS;
using TNS.KYM;
using TNS.UZY;
using TNS.Veri;
using TNS.BKD;

namespace TNS.YAT.IsKurallari
{
    /// <summary>
    /// IYATServis arayüzünden türeyen, istemciden gelen çağrıları
    /// ilgili sınıflardaki ilgili yordamlara yönlendiren fonksiyonları tutan sınıf
    /// </summary>
    [Serializable()]
    public class YATServis : MarshalByRefObject, IYATServis
    {
        private static IUZYServis servisUZY = null;

        //static bool tabloGuncelle = false;

        /// <summary>
        /// Parametresiz Yatırım servisi nesnesi oluşturucu yordam
        /// </summary>
        public YATServis()
        {
            //TabloDegisimYap();
        }

        //private void TabloDegisimYap()
        //{
        //    if (tabloGuncelle == false)
        //    {
        //        tabloGuncelle = true;
        //        TNS.Veri.VeriErisim ve = TNS.Veri.VeriErisim.VeriErisimBul(null, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(null));

        //        ve.hata = "";
        //        ve.ExecuteNonquery(@"ALTER TABLE YATBUTCEKAYNAK ADD YIKOBMU INT NOT NULL DEFAULT 0");

        //        ve.hata = "";
        //        ve.ExecuteNonquery(@"CREATE TABLE YATKALKINMABOLGE
        //            (BOLGEKOD VARCHAR(10)
        //            , BOLGEAD VARCHAR(200)
        //            , PRIMARY KEY (BOLGEKOD))");

        //        ve.hata = "";
        //        ve.ExecuteNonquery(@"CREATE TABLE YATKALKINMABOLGEIL
        //            (BOLGEKOD VARCHAR(10)
        //            , ILKOD VARCHAR(10)
        //            , PRIMARY KEY (BOLGEKOD, ILKOD))");

        //        ve.hata = "";
        //        ve.ExecuteNonquery(@"CREATE TABLE YATPROJEKALKINMABOLGE
        //            (YIL INT
        //            , SENARYOKOD VARCHAR(20)
        //            , PROJENO INT
        //            , ALTPROJENO INT
        //            , BOLGEKOD VARCHAR(20)
        //            , PRIMARY KEY (YIL, SENARYOKOD, PROJENO, ALTPROJENO, BOLGEKOD))");

        //        ve.hata = "";
        //        ve.ExecuteNonquery(@"ALTER TABLE YATPROJEEKLIDOSYA ADD YIL INT NOT NULL DEFAULT 2016");

        //        ve.hata = "";
        //        ve.ExecuteNonquery(@"CREATE TABLE YATALTPROJEDETAYPROGRAMI
        //                                (YIL INT
        //                                , SENARYOKOD VARCHAR(20)
        //                                , ALTPROJENO INT
        //                                , SIRANO INT
        //                                , YER VARCHAR(200)
        //                                , BIRIMI VARCHAR(200)
        //                                , SAYMANLIKKOD INT
        //                                , BIRIMKOD VARCHAR(3)
        //                                , ODENEK MONEY
        //                                , ACIKLAMA VARCHAR(2000)
        //                                , PRIMARY KEY (YIL, SENARYOKOD, ALTPROJENO, SIRANO))");

        //        ve.hata = "";
        //        ve.ExecuteNonquery(@"CREATE TABLE YATYOLLUKPLAN(YIL INT
        //         , SENARYOKOD VARCHAR(20)
        //         , PEBKOD VARCHAR(20)
        //         , SIRANO INT
        //         , DAIRESI VARCHAR(200)
        //         , KONUSU VARCHAR(200)
        //         , MUHTELIFMI INT
        //         , SURESI INT
        //         , KISISAYISI INT
        //         , YAPILACAGIAY VARCHAR(200)
        //         , YERI VARCHAR(200)
        //         , KURKOD VARCHAR(20)
        //         , FONKOD VARCHAR(20)
        //         , FINKOD VARCHAR(20)
        //         , EKOKOD VARCHAR(20)
        //         , MALIYET MONEY
        //         , PRIMARY KEY (YIL, SENARYOKOD, PEBKOD, SIRANO))");

        //        ve.hata = "";
        //        ve.ExecuteNonquery(@"CREATE TABLE YATKARAKTERISTIKTANIM(DEGER VARCHAR(50), DEGERKISA VARCHAR(10), EKTUR INT, SIRA VARCHAR(10), PRIMARY KEY (DEGER))");

        //        ve.hata = "";
        //        ve.ExecuteNonquery(@"ALTER TABLE YATBUTCEKAYNAK ADD PBKOD INT NOT NULL DEFAULT 1, DIPNOT VARCHAR(2000)");

        //        ve.hata = "";
        //        ve.ExecuteNonquery(@"CREATE TABLE YATKREDIDETAY(
        //                                BUTCEYIL INT,
        //                                SENARYOKOD VARCHAR(20),
        //                                KAYNAKNO INT,
        //                                YIL INT,
        //                                ANLASMATUTARI MONEY,
        //                                KULLANILANTUTAR MONEY,
        //                                KULLANILACAKTUTAR MONEY, PRIMARY KEY (BUTCEYIL, SENARYOKOD, KAYNAKNO , YIL))");

        //        ve.hata = "";
        //        ve.ExecuteNonquery(@"ALTER TABLE YATKREDIDETAY ADD ANLASMATUTARIIC MONEY, KULLANILANTUTARIC MONEY, KULLANILACAKTUTARIC MONEY");

        //        ve.hata = "";
        //        ve.ExecuteNonquery(@"CREATE TABLE YATPROJEEKLIDOSYA(
        //              PROJEKODU INT
        //            , ALTPROJEKODU INT
        //            , FIZIKIDOSYAAD VARCHAR(50)
        //            , SIRANO VARCHAR(50)
        //            , DOSYAAD VARCHAR(500)
        //            , ACIKLAMA VARCHAR(500)
        //            , YUKLEMEZAMANI DATETIME
        //            , KULLANICIKODU VARCHAR(50)
        //            , PRIMARY KEY (PROJEKODU, ALTPROJEKODU, FIZIKIDOSYAAD))");

        //        ve.hata = "";
        //        ve.ExecuteNonquery(@"CREATE TABLE YATYENIPROJEBUTCE(
        //              YIL INT, SENARYOKOD VARCHAR(20), KAYNAKNO INT, EKOKOD VARCHAR(20)
        //            , PROJETUTARIDIS MONEY
        //            , PROJETUTARIIC MONEY
        //            , DISKREDI0 MONEY
        //            , ICKAYNAK0 MONEY
        //            , DISKREDI1 MONEY
        //            , ICKAYNAK1 MONEY
        //            , DISKREDI2 MONEY
        //            , ICKAYNAK2 MONEY
        //            , PRIMARY KEY (YIL, SENARYOKOD , KAYNAKNO, EKOKOD))");

        //        ve.hata = "";
        //        ve.ExecuteNonquery(@"CREATE TABLE YATDEFLATOR (YIL INT, SENARYOKOD VARCHAR(20), SEKTORKOD VARCHAR(20), DEFLATOR DECIMAL(18,10), PRIMARY KEY (YIL, SENARYOKOD, SEKTORKOD))");

        //        ve.hata = "";
        //        ve.ExecuteNonquery(@"CREATE TABLE YATALTPROJEYETKI (KULLANICIKODU VARCHAR(20), PROJENO INT, ALTPROJENO INT, PRIMARY KEY (KULLANICIKODU, PROJENO, ALTPROJENO))");

        //        ve.hata = "";
        //        ve.VeriErisimKapat();
        //    }
        //}

        /// <summary>
        /// Gönderilen uzaydan verilen kriterlere uygun olan uzay alan bilgisini çeken yordam
        /// uzayAlan parametresi boş ise varsayılan kullanılır.
        /// </summary>
        /// <param name="uzayAd">Uzay adı</param>
        /// <param name="kosul">Uzaya verilecek koşul bilgisi</param>
        /// <param name="esitle">esitle true ise koşullar koşul alanlarına eşitlenir, false ise like kullanılır.</param>
        /// <param name="uzayAlan">Değeri istenen uzay alanı</param>
        /// <returns>İstenen uzay alanı değeri döndürülür.</returns>
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
        /// Gönderilen uzaydan verilen kriterlere uygun olan uzay alan bilgisini çeken yordam
        /// uzayAlan parametresi boş ise varsayılan kullanılır.
        /// </summary>
        /// <param name="ve">Veri tabanına erişim bağlantı nesnesi</param>
        /// <param name="uzayAd">Uzay adı</param>
        /// <param name="kosul">Uzaya verilecek koşul bilgisi</param>
        /// <param name="esitle">esitle true ise koşullar koşul alanlarına eşitlenir, false ise like kullanılır.</param>
        /// <param name="uzayAlan">Değeri istenen uzay alanı</param>
        /// <returns>İstenen uzay alanı değeri döndürülür.</returns>
        public static string UzayYaziDegerGetir(VeriErisim ve, string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            if (servisUZY == null)
                servisUZY = TNS.UZY.Arac.Tanimla();

            string sql = servisUZY.UzayKosulStr(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle, uzayAlan);
            return ve.TekKolonSQLStr(sql);
        }

        /// <summary>
        /// Gönderilen uzaydan verilen kriterlere uygun olan uzay alan bilgisini çeken yordam
        /// uzayAlan parametresi boş ise varsayılan kullanılır.
        /// </summary>
        /// <param name="uzayAd">Uzay adı</param>
        /// <param name="kosul">Uzaya verilecek koşul bilgisi</param>
        /// <param name="esitle">esitle true ise koşullar koşul alanlarına eşitlenir, false ise like kullanılır.</param>
        /// <param name="uzayAlan">Değeri istenen uzay alanı</param>
        /// <returns>İstenen uzay alanı değeri döndürülür.</returns>
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
        /// Proje tür varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="projeTur">Proje tür sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ProjeTurKaydet(Kullanici kullanan, Sozluk projeTur)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = ProjeTur.Kaydet(kullanan, projeTur);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan proje tür bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="projeTur">Proje tür kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında proje türleri listesi döndürülür.</returns>
        public BilgiVAN ProjeTurListele(Kullanici kullanan, Sozluk projeTur)
        {
            try
            {
                return ProjeTur.Listele(null, kullanan, projeTur);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Proje tür, yatırım projelerinde ve/veya alt projelerinde kullanılmamışsa silen,
        /// kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="projeTur">Proje tür sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ProjeTurSil(Kullanici kullanan, Sozluk projeTur)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = ProjeTur.Sil(kullanan, projeTur);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Doküman tür varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dokumanTur">Doküman tür sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc DokumanTurKaydet(Kullanici kullanan, Sozluk dokumanTur)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = DokumanTur.Kaydet(kullanan, dokumanTur);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan doküman tür bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dokumanTur">Doküman tür kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında doküman türleri listesi döndürülür.</returns>
        public BilgiVAN DokumanTurListele(Kullanici kullanan, Sozluk dokumanTur)
        {
            BilgiVAN donusDeger = new BilgiVAN();
            try
            {
                donusDeger = DokumanTur.Listele(null, kullanan, dokumanTur);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Doküman tür, alt projelere doküman eklenirken kullanılmamışsa silen,
        /// kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dokumanTur">Doküman tür sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc DokumanTurSil(Kullanici kullanan, Sozluk dokumanTur)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = DokumanTur.Sil(kullanan, dokumanTur);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// İl varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="il">İl sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc IlKaydet(Kullanici kullanan, Sozluk il)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = Il.Kaydet(kullanan, il);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan il bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="il">İl kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında il listesi döndürülür.</returns>
        public BilgiVAN IlListele(Kullanici kullanan, Sozluk il)
        {
            try
            {
                return Il.Listele(null, kullanan, il);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// İl, ilçe tanımlamada kullanılmamışsa silen,
        /// kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="il">İl sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc IlSil(Kullanici kullanan, Sozluk il)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = Il.Sil(kullanan, il);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// İlçe varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ilçe sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc IlceKaydet(Kullanici kullanan, IlceSozluk ilce)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = Ilce.Kaydet(kullanan, ilce);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ilçe bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ilçe sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ilçe listesi döndürülür.</returns>
        public BilgiVAN IlceListele(Kullanici kullanan, IlceSozluk ilce)
        {
            try
            {
                return Ilce.Listele(null, kullanan, ilce);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// İlçe, yatırım projelerinde ve/veya alt projelerinde kullanılmamışsa silen,
        /// kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc IlceSil(Kullanici kullanan, IlceSozluk ilce)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = Ilce.Sil(kullanan, ilce);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ilçe bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ilçe listesi döndürülür.</returns>
        public BilgiVAN IlIlceOneriListele(Kullanici kullanan, IlceSozluk ilce)
        {
            try
            {
                return Ilce.ListeleOneri(null, kullanan, ilce);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        ///// <summary>
        ///// Sektör varsa güncelleyen, yoksa kaydeden yordam
        ///// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="sektor">Sektör sözlük bilgisini tutan nesne</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //public Sonuc SektorKaydet(Kullanici kullanan, SektorSozluk sektor)
        //{
        //    try
        //    {
        //        return Sektor.Kaydet(kullanan, sektor);
        //    }
        //    catch (Exception e)
        //    {
        //        Sonuc donusDeger = new Sonuc();
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //        return donusDeger;
        //    }
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan sektör bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="sektor">Sektör kriteri</param>
        ///// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sektör listesi döndürülür.</returns>
        //public BilgiVAN SektorListele(Kullanici kullanan, int yil, SektorSozluk sektor)
        //{
        //    try
        //    {
        //        return Sektor.Listele(null, kullanan, yil, sektor);
        //    }
        //    catch (Exception e)
        //    {
        //        BilgiVAN donusDeger = new BilgiVAN();
        //        donusDeger.sonuc.DegerAta(e.ToString());
        //        return donusDeger;
        //    }
        //}

        ///// <summary>
        ///// Sektör, yatırım projelerinde kullanılmamışsa silen,
        ///// kullanılmışsa uyarı döndüren yordam
        ///// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="sektor">Sektör sözlük bilgisini tutan nesne</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //public Sonuc SektorSil(Kullanici kullanan, SektorSozluk sektor)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = Sektor.Sil(kullanan, sektor);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        /// <summary>
        /// Yatırım projesi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen proje nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="proje">Yatırım projesi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ProjeKaydet(Kullanici kullanan, int yil, string senaryoKod, Proje proje, bool yeni)
        {
            try
            {
                return ProjeClass.Kaydet(null, kullanan, yil, senaryoKod, proje, yeni);
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
        /// Yatırım projesi, yatırım alt projelerinde kullanılmamışsa silen,
        /// kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// proje nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="projeler">Proje nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ProjeSil(Kullanici kullanan, int yil, string senaryoKod, TNSCollection projeler)
        {
            try
            {
                return ProjeClass.Sil(null, kullanan, yil, senaryoKod, projeler);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public ObjectArray ProjeListeleFaal(Kullanici kullanan, int yil, string senaryoKod, string altFaalKod)
        {
            try
            {
                return ProjeClass.ProjeListeleFaal((VeriErisim)null, kullanan, yil, senaryoKod, altFaalKod);
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
        /// Verilen yıla uygun olan yatırım projelerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public BilgiVAN ProjeListele(Kullanici kullanan, int yil, string senaryoKod, Proje proje, string kurKod, bool altProjelerle)
        {
            try
            {
                return ProjeClass.Listele(null, kullanan, yil, senaryoKod, proje, kurKod, altProjelerle);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Verilen yıla uygun olan yatırım projelerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">senaryoKod</param>
        /// <param name="kurKod">kurKod</param>
        /// <param name="kriter">kriter</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public BilgiVAN ListeleProjeTanim(Kullanici kullanan, int yil, string senaryoKod, string kurKod, string kriter)
        {
            try
            {
                return ProjeClass.ListeleProjeTanim(null, kullanan, yil, senaryoKod, kurKod, kriter);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Verilen yıla uygun olan yatırım projelerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">senaryoKod</param>
        /// <param name="kurKod">kurKod</param>
        /// <param name="kriter">kriter</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public BilgiVAN ListeleAltProjeTanim(Kullanici kullanan, int yil, string senaryoKod, string kurKod, int projeKod, string kriter)
        {
            try
            {
                return ProjeClass.ListeleAltProjeTanim(null, kullanan, yil, senaryoKod, kurKod, projeKod, kriter);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Yatırım alt projesi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen alt proje nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProje">Yatırım alt projesi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc AltProjeKaydet(Kullanici kullanan, int yil, string senaryoKod, AltProje altProje, bool yeni)
        {
            try
            {
                return AltProjeClass.Kaydet(null, kullanan, yil, senaryoKod, altProje, yeni);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc AltProjeGerekceKaydet(Kullanici kullanan, int yil, string senaryoKod, AltProje[] gerekceler)
        {
            try
            {
                return AltProjeClass.GerekceKaydet(null, kullanan, yil, senaryoKod, gerekceler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Yatırım alt projesi; bütçe kaynaklarda, dokümanlarda ve kredi taleplerde kullanılmamışsa silen,
        /// kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// alt proje nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProjeler">Alt proje nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc AltProjeSil(Kullanici kullanan, int yil, string senaryoKod, TNSCollection altProjeler)
        {
            try
            {
                return AltProjeClass.Sil(null, kullanan, yil, senaryoKod, altProjeler);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Verilen yıla ve yatırım projesine uygun olan yatırım alt projelerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım alt projeleri döndürülür.</returns>
        public BilgiVAN AltProjeListele(Kullanici kullanan, int yil, string senaryoKod, AltProje altProje, string sektorKod, bool detayOku, bool detayRakamOku)
        {
            try
            {
                return AltProjeClass.Listele(null, kullanan, yil, senaryoKod, altProje, sektorKod, detayOku, detayRakamOku);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Bütçe kaynak varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen bütçe kaynak nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynak">Bütçe kaynak bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceKaynakKaydet(Kullanici kullanan, int yil, string senaryoKod, ButceKaynak kaynak, bool yeni)
        {
            try
            {
                return ButceKaynakClass.Kaydet(null, kullanan, yil, senaryoKod, kaynak, yeni, true);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Bütçe kaynağın detay bilgilerini, ödenek teklif  bilgilerini,
        /// harcama  bilgilerini ve geri ödeme bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen bütçe kaynak nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynak">Bütçe kaynak bilgisini tutan nesne</param>
        /// <param name="yil">Kaydedilecek bütçe yılı</param>
        /// <param name="senaryoKod">Gelen verilerin kaydedileceği senaryo</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceKaynakDetayKaydet(Kullanici kullanan, ButceKaynak kaynak, int yil, string senaryoKod)
        {
            try
            {
                return ButceKaynakClass.KaydetDetay(null, kullanan, kaynak, yil, senaryoKod);
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
        /// Bütçe kaynak, herhangi bir detay bilgiye sahip değilse silen,
        /// detay bilgileri varsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// bütçe kaynak nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynakNo">silinecek kaynağın numarası</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceKaynakSil(Kullanici kullanan, int yil, string senaryoKod, int altProjeNo, int kaynakNo)
        {
            try
            {
                return ButceKaynakClass.Sil(null, kullanan, yil, senaryoKod, altProjeNo, kaynakNo);
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
        /// Bütçe kaynağın detay bilgilerini, ödenek teklif  bilgilerini,
        /// harcama  bilgilerini ve geri ödeme bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// bütçe kaynak nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynakNo">Silinecek kaynağın kodu</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceKaynakDetaySil(Kullanici kullanan, int yil, string senaryoKod, int altProjeKodu, int kaynakNo)
        {
            try
            {
                return ButceKaynakClass.SilDetay(null, kullanan, yil, senaryoKod, altProjeKodu, kaynakNo);
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
        /// Verilen kriterlere uygun olan bütçe kaynak bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynakNo">Listelenmek istenen bütçe kaynağın programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="detayVer">Bütçe kaynağın detay bilgileri listelensin mi listelenmesin mi</param>
        /// <param name="tumYillar">Projenin geçmiş yıl rakamlarını da getirir.</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında bütçe kaynaklar döndürülür.</returns>
        public BilgiVAN ButceKaynakBilgiOku(Kullanici kullanan, int kaynakNo, int yil, string senaryoKod, string sektorKod, bool detayVer, bool tumYillar, bool tekYil, string projeAd)
        {
            try
            {
                return ButceKaynakClass.BilgiOku(null, kullanan, 0, 0, kaynakNo, yil, senaryoKod, sektorKod, null, detayVer, false, null, tekYil, projeAd, false);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Verilen kriterlere uygun olan bütçe kaynak bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProjeKodu">Yatırım alt projesinin programatik kodu</param>
        /// <param name="kaynakNo">Listelenmek istenen bütçe kaynağın programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="tur">Listelenmek istenen bütçe kaynak türleri</param>
        /// <param name="detayVer">Bütçe kaynağın detay bilgileri listelensin mi listelenmesin mi</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında bütçe kaynaklar döndürülür.</returns>
        public BilgiVAN ButceKaynakListele(Kullanici kullanan, int projeKodu, int altProjeKodu, int kaynakNo, int yil, string senaryoKod, ENUMButceKaynakTur[] tur, bool detayVer)
        {
            try
            {
                return ButceKaynakClass.BilgiOku(null, kullanan, projeKodu, altProjeKodu, kaynakNo, yil, senaryoKod, string.Empty, tur, detayVer, false, null, false, null, false);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray KrediliKaynaklariListele(Kullanici kullanan, int yil, string senaryoKod, int projeKodu, int altProjeKodu, ENUMButceKaynakTur[] kaynakTurler)
        {
            try
            {
                return ButceKrediDetayClass.KrediliKaynaklariListele(null, kullanan, yil, senaryoKod, projeKodu, altProjeKodu, kaynakTurler);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.Message);
                return oa;
            }
        }

        /// <summary>
        /// Alt faaliyet-yatırım bağı üzerinden bütçe hazırlık ödenek tekliflerini bulup,
        /// bütçe kaynak bazında ödenek toplamlarını hesaplayan ve kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceKaynakOdenekAktar(Kullanici kullanan)
        {
            try
            {
                return ButceKaynakTeklifClass.OdenekAktar(kullanan);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Alt faaliyet-yatırım bağı üzerinden bütçe hazırlık ödenek tekliflerini bulup,
        /// bütçe kaynak bazında ödenek toplamlarını hesaplayan ve kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceKaynakOdenekAktar(Kullanici kullanan, int yil)
        {
            try
            {
                return ButceKaynakTeklifClass.OdenekAktar(kullanan, yil);
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
        /// Alt faaliyet-yatırım bağı üzerinden bütçe hazırlık ödenek tekliflerini bulup,
        /// bütçe kaynak bazında ödenek toplamlarını hesaplayan ve kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceKaynakOdenekAktar(Kullanici kullanan, int yil, string senaryoKod)
        {
            try
            {
                return ButceKaynakTeklifClass.OdenekAktar(kullanan, yil, senaryoKod);
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
        /// Kredi talep varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen kredi talep nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="krediTalep">Kredi talep bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KrediTalepKaydet(Kullanici kullanan, int yil, string senaryoKod, KrediTalep krediTalep)
        {
            try
            {
                return KrediTalepClass.Kaydet(null, kullanan, yil, senaryoKod, krediTalep);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Verilen kriterlere uygun olan kredi talepleri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProjeKodu">Yatırım alt projesinin programatik kodu</param>
        /// <param name="krediNo">Kredi talebin programatik kodu</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kredi talepler döndürülür.</returns>
        public BilgiVAN KrediTalepListele(Kullanici kullanan, int altProjeKodu, int krediNo)
        {
            try
            {
                return KrediTalepClass.Listele(null, kullanan, altProjeKodu, krediNo);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Verilen kriterlere uygun olan kredi talepleri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="altProjeKodu">Yatırım alt projesinin programatik kodu</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kredi talepler döndürülür.</returns>
        public BilgiVAN KrediTalepListele(int altProjeKodu, Kullanici kullanan, int yil)
        {
            BilgiVAN donusDeger = new BilgiVAN();
            try
            {
                donusDeger = KrediTalepClass.Listele(null, altProjeKodu, kullanan, yil);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Doküman varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen doküman nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dokuman">Doküman bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc DokumanKaydet(Kullanici kullanan, int yil, string senaryoKod, Dokuman dokuman)
        {
            try
            {
                return DokumanClass.Kaydet(null, kullanan, yil, senaryoKod, dokuman);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Verilen kriterlere uygun olan dokümanları döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProjeKodu">Yatırım alt projesinin programatik kodu</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında dokümanlar döndürülür.</returns>
        public BilgiVAN DokumanListele(Kullanici kullanan, int altProjeKodu)
        {
            try
            {
                return DokumanClass.Listele(null, kullanan, altProjeKodu);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Doküman programatik kodu verilen dokümanı silen yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="siraNo">Doküman programatik kodu</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc DokumanSil(Kullanici kullanan, string siraNo)
        {
            try
            {
                return DokumanClass.Sil(null, kullanan, siraNo);
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
        /// DPT Tablo-2 raporunu üreten yordam
        /// Rapor; yatırım projeleri ve alt projeleri bazında maliyet, ödenek teklif ve harcama bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="sektorNo">Sektör kriteri</param>
        /// <param name="etudMu">Etüd mü değil mi</param>
        /// <param name="projeNo">Proje Numarası</param>
        /// <param name="rakamlarsiz">Ödenek Harcama rakamlarını okuma</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public BilgiVAN ProjeAltProjeListele(Kullanici kullanan, int yil, string senaryoKod, string sektorNo, string etudMu, int projeNo, int projeTur, ENUMButceKaynakTur[] bkTur, bool rakamlarsiz, int ilkYil, bool kullanilmiyor, string kurKod, int altProjeNo, bool eskiYillarKBO, string araString, bool yetkiYok)
        {
            try
            {
                return Raporlar.ProjeAltProjeListele(null, kullanan, yil, senaryoKod, sektorNo, etudMu, projeNo, projeTur, bkTur, rakamlarsiz, ilkYil, kullanilmiyor, kurKod, altProjeNo, eskiYillarKBO, araString, yetkiYok);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Yatırım projeleri listesi raporunu üreten yordam
        /// Rapor; yatırım projeleri ve alt projeleri bazında maliyet, ödenek teklif ve harcama bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="sektorNo">Sektör kriteri</param>
        /// <param name="projeNo">Yatırım projesinin programatik kodu</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public BilgiVAN Tablo2OzelListele(Kullanici kullanan, int yil, string senaryoKod, string sektorNo, int projeNo, string kurKod)
        {
            try
            {
                return Raporlar.ProjeAltProjeListele(null, kullanan, yil, senaryoKod, sektorNo, "H", -1, -1, null, false, 0, false, kurKod, -1, true, "", false);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        ///// <summary>
        ///// DPT Tablo-4 raporunu üreten yordam
        ///// Rapor; proje tutar ve karakteristik bilgilerini içerir.
        ///// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="senaryoKod">Verilerin alınacağı senaryo kod</param>
        ///// <param name="sektorNo">Sektör kriteri</param>
        ///// <param name="projeTuru">Proje tür kriteri</param>
        ///// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında Tablo4 nesneleri döndürülür.</returns>
        //public BilgiVAN Tablo4Listele(Kullanici kullanan, int yil, string senaryoKod, string sektorNo, int projeTuru)
        //{
        //    BilgiVAN donusDeger = new BilgiVAN();
        //    try
        //    {
        //        donusDeger = Raporlar.Tablo4Listele(null, kullanan, yil, senaryoKod, sektorNo, projeTuru);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        ///// <summary>
        ///// DPT Tablo-5 raporunu üreten yordam
        ///// Rapor; istenen projenin yıllar itibariyle proje tutar, ödenek teklif ve harcama bilgilerini içerir.
        ///// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        ///// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında bir tane proje nesnesi döndürülür.</returns>
        //public BilgiVAN Tablo5Listele(Kullanici kullanan, int projeKodu)
        //{
        //    BilgiVAN donusDeger = new BilgiVAN();
        //    try
        //    {
        //        donusDeger = Raporlar.Tablo5Listele(null, kullanan, projeKodu);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        /// <summary>
        /// DPT Tablo-6 raporunu üreten yordam
        /// Rapor; istenen kredinin kimlik bilgilerini, projesinin kimlik bilgilerini ve yıllar itibariyle rakamsal bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynak">Kredinin programatik kodu</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında proje nesneleri ve detayında da kredi bilgileri döndürülür.</returns>
        public BilgiVAN Tablo6Listele(Kullanici kullanan, int kaynak)
        {
            BilgiVAN donusDeger = new BilgiVAN();
            try
            {
                donusDeger = Raporlar.Tablo6Listele(null, kullanan, kaynak);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// DPT Tablo-7 raporunu üreten yordam
        /// Rapor; istenen kredi talebin ve projesinin bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="krediTalepNo">Kredi talebin programatik kodu</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında proje nesneleri ve detayında da kredi talep bilgileri döndürülür.</returns>
        public BilgiVAN Tablo7Listele(Kullanici kullanan, int yil, string senaryoKod, int krediTalepNo)
        {
            try
            {
                return Raporlar.Tablo7Listele(null, kullanan, yil, senaryoKod, krediTalepNo);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// DPT Tablo-10 raporunu üreten yordam
        /// Rapor; yatırım projeleri ve alt projeleri bazında kamulaştırma maliyet,
        /// kamulaştırma ödenek teklif ve kamulaştırma harcama bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="sektorNo">Sektör kriteri</param>
        /// <param name="kurKod">Kurumsal Kod</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public BilgiVAN Tablo10Listele(Kullanici kullanan, int yil, string senaryoKod, string sektorNo, string kurKod)
        {
            BilgiVAN donusDeger = new BilgiVAN();
            try
            {
                donusDeger = Raporlar.Tablo10Listele(null, kullanan, yil, senaryoKod, sektorNo, kurKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// DPT Tablo-12-13 raporunu üreten yordam
        /// Rapor; model bazında yatırım projelerinin tutar ve karakteristik bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="sektorNo">Sektor kriteri</param>
        /// <param name="model">Model kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public BilgiVAN Tablo1213Listele(Kullanici kullanan, int yil, string sektorNo, int model, string senaryoKod, string kurKod)
        {
            try
            {
                return Raporlar.Tablo1213Listele(null, kullanan, yil, sektorNo, model, senaryoKod, kurKod);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Yatırım teklifleri dağılım raporunu üreten yordam
        /// Rapor; birim, yatırım projesi ve tertip bazında yatırım projeleri ödenek teklif ve harcama bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="sektorKod">Sektor kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public BilgiVAN TeklifDagilimListele(Kullanici kullanan, int yil, string senaryoKod, string sektorKod, string kurKod)
        {
            try
            {
                return Raporlar.TeklifDagilimListele(null, kullanan, yil, senaryoKod, sektorKod, kurKod);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Birimlere ve sektörlere göre yatırım teklifleri dağılım raporunu üreten yordam
        /// Rapor; birim ve sektör bazında yatırım projeleri ödenek teklif ve harcama bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında teklif dağılım listesi döndürülür.</returns>
        public BilgiVAN SektorDagilimListele(Kullanici kullanan, int yil, string senaryoKod, string kurKod)
        {
            try
            {
                return Raporlar.SektorDagilimListele(null, kullanan, yil, senaryoKod, kurKod);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Birimlerin yatırım projeleri ve ödenek teklifleri raporunu üreten yordam
        /// Rapor; istenen birime ait yatırım projelerini ve ödenek tekliflerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="birimKodu">Birim kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public BilgiVAN BirimProjeleriListele(Kullanici kullanan, int yil, string senaryoKod, string birimKodu, bool altProjelerDahil)
        {
            try
            {
                return Raporlar.BirimProjeleriListele(null, kullanan, yil, senaryoKod, birimKodu, altProjelerDahil);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Konsolide yatırım bilgileri raporunu üreten yordam
        /// Rapor; yatırım projelerini, alt projelerini, proje maliyetini, ödenek tekliflerini ve harcamalarını içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public BilgiVAN KonsolideYatirimListele(Kullanici kullanan, int yil, string kurKod)
        {
            try
            {
                return Raporlar.KonsolideYatirimListele(null, kullanan, yil, "", kurKod);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Proje takip raporunu üreten yordam
        /// Rapor; yatırım projesinin yıllar itibariyle maliyet, ödenek teklif ve harcama bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public BilgiVAN ProjeTakipTablosu(Kullanici kullanan, int yil, string kurKod)
        {
            try
            {
                return Raporlar.ProjeTakipTablosu(null, kullanan, yil, kurKod);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }

        }

        /// <summary>
        /// Projeler arası ödenek aktarması raporunu üreten yordam
        /// Rapor; yatırım projelerini, alt projelerini, proje maliyetini, ödenek tekliflerini ve harcamalarını içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="sektorNo">Sektör kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public BilgiVAN ProjelerArasiOdenekAktarmasiTablosu(Kullanici kullanan, int yil, string sektorNo, string kurKod)
        {
            BilgiVAN donusDeger = new BilgiVAN();
            try
            {
                donusDeger = Raporlar.KonsolideYatirimListele(null, kullanan, yil, sektorNo, kurKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Projelere ve tertiplere göre yatırım teklifleri dağılım raporunu üreten yordam
        /// Rapor; yatırım projeleri ve tertipler bazında ödenek teklif bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="sektor">Sektör kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public BilgiVAN TertipDagilimListele(Kullanici kullanan, int yil, string senaryoKod, string sektor, string kurKod)
        {
            try
            {
                return Raporlar.TertipDagilimListele(null, kullanan, yil, senaryoKod, sektor, kurKod);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// DPT Vize Cetveli raporunu üreten yordam
        /// Rapor; sektörler ve tertipler bazında ödenek teklif bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="tertip">Tertip bilgilerini tutan nesne</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında DPTVizeBilgi nesneleri döndürülür.</returns>
        public BilgiVAN DPTVizeCetveliRaporla(Kullanici kullanan, int yil, string senaryoKod, Tertip tertip)
        {
            try
            {
                return Raporlar.DPTVizeCetveliRaporla(null, kullanan, yil, senaryoKod, tertip);
            }
            catch (Exception e)
            {
                BilgiVAN donusDeger = new BilgiVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Verilen yıla uygun olan Hizmet alımı suretiyle kullanılan/kullanılacak
        /// taşıtlara ilişkin ödenek teklif bilgilerini döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında TasitAlimBedeliBilgisi nesneleri döndürülür.</returns>
        public ObjectArray TasitAlimBedeliListele(int yil)
        {
            ObjectArray donusDeger;
            try
            {
                donusDeger = TasitBilgi.TasitAlimBedeliListele(null, yil);
            }
            catch (Exception e)
            {
                donusDeger = new ObjectArray();
                donusDeger.sonuc = new Sonuc(false, e.ToString(), "");
            }
            return donusDeger;
        }

        /// <summary>
        /// Hizmet alımı suretiyle kullanılan/kullanılacak
        /// taşıtlara ilişkin ödenek teklif bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl bilgisi</param>
        /// <param name="tasitAlimBedelleri">TasitAlimBedeliBilgisi nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc TasitAlimBedeliKaydet(Kullanici kullanan, int yil, TNSCollection tasitAlimBedelleri)
        {
            Sonuc donusDeger;
            try
            {
                donusDeger = TasitBilgi.TasitAlimBedeliKaydet(null, kullanan, yil, tasitAlimBedelleri);
            }
            catch (Exception e)
            {
                donusDeger = new Sonuc(false, e.ToString(), "");
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşıt talep bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurumKod">Kurum kriteri</param>
        /// <param name="sektorKod">Sektör kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında TasitTalepBilgi nesneleri döndürülür.</returns>
        public ObjectArray TasitListesiBul(Kullanici kullanan, int yil, string kurumKod, string sektorKod)
        {
            ObjectArray donusDeger;
            try
            {
                donusDeger = TasitBilgi.TasitListesiBul(null, kullanan, yil, kurumKod, sektorKod);
            }
            catch (Exception e)
            {
                donusDeger = new ObjectArray();
                donusDeger.sonuc = new Sonuc(false, e.ToString(), "");
            }
            return donusDeger;
        }

        /// <summary>
        /// Taşıt talep bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl bilgisi</param>
        /// <param name="kurumKod">Kurum bilgisi</param>
        /// <param name="sektorKod">Sektör bilgisi</param>
        /// <param name="tasitListesi">TasitTalepBilgi nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc TasitListesiKaydet(Kullanici kullanan, int yil, string kurumKod, string sektorKod, TNSCollection tasitListesi)
        {
            Sonuc donusDeger;
            try
            {
                donusDeger = TasitBilgi.TasitListesiKaydet(null, kullanan, yil, kurumKod, sektorKod, tasitListesi);
            }
            catch (Exception e)
            {
                donusDeger = new Sonuc(false, e.ToString(), "");
            }
            return donusDeger;
        }

        public BilgiVAN VizeCetveliRaporla(Kullanici kullanan, int yil, string senaryoKod, string sektorNo, string birimKod)
        {
            return IsKurallari.Raporlar.VizeCetveliRaporla(null, kullanan, yil, senaryoKod, sektorNo, birimKod);
        }

        public Sonuc AsamaKopyala(Kullanici kullanan, int kaynakYil, string kaynakAsama, int hedefYil, string hedefAsama, bool[] kopyalanacaklar, TNSCollection projeKodlar)
        {
            try
            {
                return ButceKaynakTeklifClass.AsamaKopyala(kullanan, kaynakYil, kaynakAsama, hedefYil, hedefAsama, kopyalanacaklar, projeKodlar);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray ProjeHarcamalari(Kullanici kullanan, int yil, string senaryoKod, Tertip kistasTertip, string kurKod, int avansDurum, bool adlariBul)
        {
            return ButceKaynakTeklifClass.ProjeHarcamalari(kullanan, yil, kistasTertip, senaryoKod, kurKod, avansDurum, adlariBul);
        }

        public ObjectArray ProjeSunumBilgileri(Kullanici kullanan, int yil, string senaryoKod, string senaryoKodOnc, Tertip kistasTertip, int Hepsi1Eski2Yeni3)
        {
            return ButceKaynakTeklifClass.ProjeSunumBilgileri(kullanan, yil, kistasTertip, senaryoKod, senaryoKodOnc, Hepsi1Eski2Yeni3);
        }

        public ObjectArray ProjeIslemleri(Kullanici kullanan, TNS.BHMv2.MPRevizeFormSorgu form)
        {
            return ButceKaynakTeklifClass.ProjeIslemleri(kullanan, form);
        }

        public ObjectArray AltProjeListeleYeni(Kullanici kullanan, AltProjeTeklif form)
        {
            try
            {
                return AltProjeClass.AltProjeListeleYeni((VeriErisim)null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public Sonuc AltProjeKaydetYeni(Kullanici kullanan, int yil, string kurKod, string sektorKod, AltProjeTeklif[] projeGerekceler)
        {
            try
            {
                return AltProjeClass.AltProjeKaydetYeni((VeriErisim)null, kullanan, yil, kurKod, sektorKod, projeGerekceler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray MaliYapiBul(Kullanici kullanan, int raporYil, string raporSenaryoKod, int butceYil, string senaryoKod, ENUMProjeMaliYapiKod eNUMProjeMaliYapiTur, int projeNo, int altProjeNo, bool oncekiYillarDahil, bool detayAltProje)
        {
            return ButceKaynakTeklifClass.MaliYapiBul(null, kullanan, raporYil, raporSenaryoKod, butceYil, senaryoKod, eNUMProjeMaliYapiTur, projeNo, altProjeNo, oncekiYillarDahil, detayAltProje);
        }

        public ObjectArray OdenekListesiBul(Kullanici kullanan, int butceYil, string senaryoKod, int teklifYil, int projeNo, int altProjeNo)
        {
            return ButceKaynakTeklifClass.OdenekListesiBul(null, kullanan, butceYil, senaryoKod, teklifYil, projeNo, altProjeNo);
        }

        public TNS.BHMv2.UcgenSatir[] OdenekListesiBulTam(Kullanici kullanan, int butceYil, string senaryoKod, int projeNo, int altProjeNo)
        {
            return ButceKaynakTeklifClass.OdenekListesiBulTam(null, kullanan, butceYil, senaryoKod, projeNo, altProjeNo);
        }

        public ObjectArray OdenekListesiBulEko2(Kullanici kullanan, int butceYil, string senaryoKod, int projeNo, int altProjeNo)
        {
            return ButceKaynakTeklifClass.OdenekListesiBulEko2(null, kullanan, butceYil, senaryoKod, projeNo, altProjeNo);
        }

        public System.Collections.Hashtable AltProjeBilgiListele(Kullanici kullanan, int yil, string senaryoKod, int projeKodu, int altProjeKodu, string[] alanlar)
        {
            return AltProjeClass.AltProjeBilgiListele((VeriErisim)null, kullanan, yil, senaryoKod, projeKodu, altProjeKodu, alanlar);
        }

        public System.Collections.Hashtable AltProjeBilgiListeleTum(Kullanici kullanan, int yil, string senaryoKod, string[] alanlar)
        {
            return AltProjeClass.AltProjeBilgiListeleTum((VeriErisim)null, kullanan, yil, senaryoKod, alanlar);
        }

        public Sonuc AltProjeBilgiKaydet(Kullanici kullanan, int yil, string senaryoKod, int projeKodu, int altProjeKodu, System.Collections.Hashtable degerler)
        {
            return AltProjeClass.AltProjeBilgiKaydet((VeriErisim)null, kullanan, yil, senaryoKod, projeKodu, altProjeKodu, degerler);
        }

        public Sonuc YATKaynakKaydetTopluVeri(Kullanici kullanan, int yil, string senaryoKod, ButceKaynak[] bklar, ENUMProjeMaliYapiKod[] teklifYillar)
        {
            return ButceKaynakClass.YATKaynakKaydetTopluVeri(null, kullanan, yil, senaryoKod, bklar, teklifYillar);
        }

        public AltFaalBilgi AltFaaliyetBilgiBul(Kullanici kullanan, int yil, string pebKod, string kurKod, string senaryoKod, bool illleriBul)
        {
            return ButceKaynakTeklifClass.AltFaaliyetBilgiBul(null, kullanan, yil, pebKod, kurKod, senaryoKod, illleriBul);
        }

        public ObjectArray TanimSozlukListele(Kullanici kullanan, int sozlukTur, int yil, string senaryoKod)
        {
            return Genel.TanimSozlukListele(null, kullanan, sozlukTur, yil, senaryoKod);
        }

        public Sonuc TanimSozlukEkle(Kullanici kullanan, int sozlukTur, Sozluk sozluk)
        {
            return Genel.TanimSozlukEkle((VeriErisim)null, kullanan, sozlukTur, sozluk, true);
        }

        /// <summary>
        /// ALTPROJEYETKI nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ALTPROJEYETKI yapısında ALTPROJEYETKI:  bilgilerini tutan değişken</param>
        public ObjectArray ListeleYATYetkiGiris(Kullanici kullanan, int projeKod, int altProjeKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return AltProjeClass.ListeleYetki(null, kullanan, projeKod, altProjeKod);
        }

        /// <summary>
        /// ALTPROJEYETKI nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ALTPROJEYETKI yapısında ALTPROJEYETKI:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetYATYetkiGiris(Kullanici kullanan, int projeKod, int altProjeKod, TNSCollection kullanicilar, int yil, string senaryoKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return AltProjeClass.KaydetYetki(null, kullanan, projeKod, altProjeKod, kullanicilar, yil, senaryoKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public decimal DeflatorBul(Kullanici kullanan, int butceYil, string senaryoKod, string sektorKod)
        {
            return Genel.DeflatorBul((VeriErisim)null, kullanan, butceYil, senaryoKod, sektorKod);
        }

        /// <summary>
        /// ProjeEkliDosya nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ProjeEkliDosya yapısında ProjeEkliDosya:  bilgilerini tutan değişken</param>
        public ObjectArray ProjeDosyalariListele(Kullanici kullanan, ProjeEkliDosya form, int yil, string senaryoKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return YATProjeEkliDosya.Listele(null, kullanan, form, yil, senaryoKod);
        }

        /// <summary>
        /// ProjeEkliDosya nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ProjeEkliDosya yapısında ProjeEkliDosya:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc ProjeDosyasiSakla(Kullanici kullanan, ProjeEkliDosya form, int yil, string senaryoKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return YATProjeEkliDosya.Kaydet(null, kullanan, form, yil, senaryoKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc ProjeDosyasiSil(Kullanici kullanan, ProjeEkliDosya form, int yil, string senaryoKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return YATProjeEkliDosya.Sil(null, kullanan, form, yil, senaryoKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc AltProjeTasi(Kullanici kullanan, int yil, string senaryoKod, int apKod, int pKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return AltProjeClass.AltProjeTasi(null, kullanan, yil, senaryoKod, apKod, pKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }


        public Sonuc KaynakTasi(Kullanici kullanan, int yil, string senaryoKod, int kaynakNo, int apKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ButceKaynakClass.KaynakTasi(null, kullanan, yil, senaryoKod, kaynakNo, apKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray ListeleDeflator(Kullanici kullanan, int yil, string senaryoKod, string sektorKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return YATDeflator.Listele(null, kullanan, yil, senaryoKod, sektorKod);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public ObjectVAN OkuDeflator(Kullanici kullanan, int yil, string senaryoKod, string sektorKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return YATDeflator.Oku(null, kullanan, yil, senaryoKod, sektorKod);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        public Sonuc KaydetDeflator(Kullanici kullanan, int yil, string senaryoKod, TNSCollection deflatorler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return YATDeflator.Kaydet(null, kullanan, yil, senaryoKod, deflatorler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc KarekteristikKaydet(Kullanici kullanan, KarakteristikGiris[] form)
        {
            return ProjeClass.KarekteristikKaydet(null, kullanan, form);
        }

        public KarakteristikGiris[] KarekteristikListele(Kullanici kullanan)
        {
            return ProjeClass.KarekteristikListele(null, kullanan);
        }

        /// <summary>
        /// YollukPlanAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YollukPlanAna yapısında YollukPlanAna: Yıl ve Faaliyet Seçimi bilgilerini tutan değişken</param>
        public ObjectArray ListeleYollukPlanlama(Kullanici kullanan, int yil, string senaryoKod, Tertip tertip, string pebKodListe)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return YATYollukPlanlama.Listele(null, kullanan, yil, senaryoKod, tertip, pebKodListe);
            }
            catch (Exception e)
            {
                ObjectArray ov = new ObjectArray();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// YollukPlanAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YollukPlanAna yapısında YollukPlanAna: Yıl ve Faaliyet Seçimi bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetYollukPlanlama(Kullanici kullanan, int yil, string senaryoKod, Tertip tertip, TNSCollection formlar)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return YATYollukPlanlama.Kaydet(null, kullanan, yil, senaryoKod, tertip, formlar);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// AltProje nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AltProje yapısında AltProje: Alt Proje bilgilerini tutan değişken</param>
        public ObjectArray ListeleAltProjeDetayProgrami(Kullanici kullanan, AltProjeDetayProgrami form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return YATAltProjeDetayProgrami.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// AltProje nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AltProje yapısında AltProje: Alt Proje bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetAltProjeDetayProgrami(Kullanici kullanan, TNSCollection form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return YATAltProjeDetayProgrami.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// KalkinmaBolge nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KalkinmaBolge yapısında KalkinmaBolge: Kalkinma Bölgelesi bilgilerini tutan değişken</param>
        public ObjectArray ListeleKalkinmaBolgeleriTanim(Kullanici kullanan, KalkinmaBolge form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return YATKalkinmaBolgeleri.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// KalkinmaBolge nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KalkinmaBolge yapısında KalkinmaBolge: Kalkinma Bölgelesi bilgilerini tutan değişken</param>
        public ObjectVAN OkuKalkinmaBolgeleriTamim(Kullanici kullanan, KalkinmaBolge form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return YATKalkinmaBolgeleri.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// KalkinmaBolge nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KalkinmaBolge yapısında KalkinmaBolge: Kalkinma Bölgelesi bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetKalkinmaBolgeleriTamim(Kullanici kullanan, KalkinmaBolge form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return YATKalkinmaBolgeleri.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc ProjeAltProjeYazmaGormeHakkiVarMi(Kullanici kullanan, int projeKodu, int altProjeKodu, int yil, string senaryoKod, TNS.BHMv2.ENUMOGY ogy, bool hazirlik)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ProjeClass.ProjeAltProjeYazmaGormeHakkiVarMi(null, kullanan, projeKodu, altProjeKodu, yil, senaryoKod, ogy, hazirlik);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray ProjeListele(Kullanici kullanan, int yil, string senaryoKod, Proje proje, bool tekYil, bool eskiYillarKBO, int ekoUz)
        {
            try
            {
                return ProjeClass.ListeleTum(null, kullanan, yil, senaryoKod, proje, tekYil, eskiYillarKBO, ekoUz);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }
    }
}