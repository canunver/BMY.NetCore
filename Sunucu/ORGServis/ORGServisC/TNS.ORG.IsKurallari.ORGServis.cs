using System;
using TNS.KYM;
using TNS.ORG;
using TNS.UZY;
using System.Collections;
using TNS.ORG.IsKurallari;

namespace TNS.ORG.IsKurallari
{
    /// <summary>
    /// Personel Servis tanımlaması
    /// </summary>
    [Serializable()]
    public class ORGServis
    {
        /// <summary>
        /// Uzay Servis tanımlaması
        /// </summary>
        private static IUZYServis servisUZY = null;

        /// <summary>
        /// Personel Servis sınıfının oluşturucu(Constructer) fonksiyonu
        /// </summary>
        public ORGServis()
        {
            //if (servisUZY == null)
            //    servisUZY = TNS.Api.ServisIstemci.Olustur<IUZYServis>(System.Configuration.ConfigurationManager.AppSettings.Get("UZYServisURL"));
        }

        /// <summary>
        /// Uzay servisine bağlanarak verilen koşul ve değerlerine göre servisi çağıran ve sonucu döndüren fonksiyon
        /// </summary>
        /// <param name="uzayAd">Uzay adını belirten string tipinde sınıf</param>
        /// <param name="kosul">Koşulu belirten string tipinde sınıf</param>
        /// <param name="esitle">Koşulun eşitlenip eşitlenmeyeceğini belirten Bool tipinde sınıf</param>
        /// <param name="uzayAlan">Uzay Alanı belirten string tipinde sınıf</param>
        /// <returns>String tipinde sınıf döndürür</returns>
        public static string UzayYaziDegerGetir(string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            if (servisUZY == null)
                servisUZY = TNS.UZY.Arac.Tanimla();

            string deger = "";
            if (uzayAd == "")
                deger = servisUZY.UzayDegeriStr(new Kullanici(), uzayAd, kosul, esitle);
            else
                deger = servisUZY.UzayDegeriStr(new Kullanici(), uzayAd, kosul, esitle, uzayAlan);

            return deger;
        }

        /// <summary>
        /// Uzay servisine bağlanarak verilen koşul ve değerlerine göre servisi çağıran ve sonucu döndüren fonksiyon
        /// </summary>
        /// <param name="uzayAd">Uzay adını belirten string tipinde sınıf</param>
        /// <param name="kosul">Koşulu belirten string tipinde sınıf</param>
        /// <param name="esitle">Koşulun eşitlenip eşitlenmeyeceğini belirten Bool tipinde sınıf</param>
        /// <param name="uzayAlan">Uzay Alanı belirten string tipinde sınıf</param>
        /// <returns>Double tipinde sınıf döndürür</returns>
        public static double UzaySayiDegerGetir(string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            if (servisUZY == null)
                servisUZY = TNS.UZY.Arac.Tanimla();

            double deger = 0;
            if (uzayAd == "")
                deger = servisUZY.UzayDegeriDbl(new Kullanici(), uzayAd, kosul, esitle);
            else
                deger = servisUZY.UzayDegeriDbl(new Kullanici(), uzayAd, kosul, esitle, uzayAlan);

            return deger;
        }

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
                donusDeger = new PERGorev().Kaydet(islemiYapan, p);
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
        public ObjectArray GorevListele(Gorev p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERGorev().Listele(p);
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
        public ObjectArray GorevSorgula(Gorev p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERGorev().Sorgula(p);
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
                donusDeger = new PERGorev().Sil(islemiYapan, p);
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
                donusDeger = new PERGorev().KaydetEvrakBilgi(islemiYapan, gorevKod, evrEvrakNo, prEvrakNo);
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
                donusDeger = new PERGorevYerTarih().Kaydet(islemiYapan, liste, gorevKod);
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
                donusDeger = new PERGorevYerTarih().Listele(p);
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
                donusDeger = new PERGorevYerTarih().Sil(islemiYapan, gorevKod);
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
                donusDeger = new PERGorevTertip().Kaydet(islemiYapan, liste, gorevKod);
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
        public ObjectArray GorevTertipListele(GorevTertip p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERGorevTertip().Listele(p);
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
                donusDeger = new PERGorevTertip().Sil(islemiYapan, gorevKod);
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
                donusDeger = new PERGorevPersonel().Kaydet(islemiYapan, liste, gorevKod);
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
                donusDeger = new PERGorevPersonel().Listele(p);
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
                donusDeger = new PERGorevPersonel().Sil(islemiYapan, gorevKod);
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
            try
            {
                return ORGBirim.Kaydet(null, kullanici, obler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Organizasyon birim Bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="p">Organizasyon birim bilgisi</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc OrganizasyonBirimKaydet(Kullanici islemiYapan, OrganizasyonBirim p)
        {
            try
            {
                return ORGBirim.Kaydet(null, islemiYapan, p, false);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc OrganizasyonBirimGuncelle(Kullanici islemiYapan, int orgBirimKod, int yeniAnaBirimKod, int eskiAnaBirimKod, int yeniSira)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = ORGBirim.Guncelle(null, islemiYapan, orgBirimKod, yeniAnaBirimKod, eskiAnaBirimKod, yeniSira);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Bir kullanıcının görebileceği birimleri listeler
        /// </summary>
        /// <param name="kullanici">Organizasyon birim bilgisi</param>
        /// <param name="detayVer">Birime ait harcama birimlerini ve bütçe tertiplerini de listeler.</param>
        /// <param name="adEkle">Organizasyon birime ait kodlarının açıklamasını da döndürür.</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        public ObjectArray OrganizasyonBirimListeleKullanici(Kullanici kullanici, bool detayVer, bool adEkle)
        {
            try
            {
                return ORGBirim.ListeleKullanici(null, kullanici, detayVer, adEkle);
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
        public ObjectArray OrganizasyonBirimListele(OrganizasyonBirim p, bool detayVer, bool adEkle)
        {
            try
            {
                return ORGBirim.Listele(null, p, detayVer, adEkle);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray Il_IlceBul(Kullanici kullanan, string plakaKod, int ilceMernisKod, bool detayVer, bool adEkle)
        {
            try
            {
                return ORGBirim.Il_IlceBul(null, kullanan, plakaKod, ilceMernisKod, detayVer, adEkle);
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
        public ObjectArray OrganizasyonBirimListele(OrganizasyonBirim p, bool detayVer, int ustVeAyniSeviye, bool adEkle)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = ORGBirim.Listele(null, p, detayVer, ustVeAyniSeviye, adEkle);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
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
                donusDeger = ORGBirim.Sil(null, islemiYapan, p);
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
                donusDeger = ORGBirim.AnaBirimVeSiraDegistir(null, islemYapan, p, yeniAnaBirimKod, yeniSiraNo);
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
                donusDeger = ORGBirim.SablonUygula(null, kullanan, obAnaKod, sablonKod);
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
                return ORGBirim.GrafikAgacOlustur(null, kullanan, orgBirimKod);
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
                donusDeger = ORGBirim.BirimAc(null, kullanici, orgBirimKod);
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
            try
            {
                return ORGBirim.BirimKapat(null, kullanici, orgBirimKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
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
            return GenelGorevIslemler.GenelGorevOku(null, kullanan, orgBirim, siraNo, true);
        }

        /// <summary>
        /// Genels the gorev listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="orgBirim">The org birim.</param>
        /// <returns></returns>
        public BirimGenelGorev[] GenelGorevListele(Kullanici kullanan, OrganizasyonBirim orgBirim, bool aktifler)
        {
            return GenelGorevIslemler.GenelGorevListele(null, kullanan, orgBirim, aktifler, 0);
        }

        /// <summary>
        /// Yenis the gorev ekle.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="orgBirim">The org birim.</param>
        /// <returns></returns>
        public BirimGenelGorev YeniGorevEkle(Kullanici kullanan, OrganizasyonBirim orgBirim)
        {
            return GenelGorevIslemler.YeniGorevEkle(null, kullanan, orgBirim);
        }

        /// <summary>
        /// Genels the gorev kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="birimGenelGorev">The birim genel gorev.</param>
        /// <returns></returns>
        public Sonuc GenelGorevKaydet(Kullanici kullanan, BirimGenelGorev birimGenelGorev)
        {
            return GenelGorevIslemler.GenelGorevKaydet(null, kullanan, birimGenelGorev);
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
            return GenelGorevIslemler.GenelGorevYetkiOku(null, kullanan, birimGenelGorev, yazYetkiSiraNo);
        }

        /// <summary>
        /// Genels the gorev yetki kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="genelGorevYetki">The genel gorev yetki.</param>
        /// <returns></returns>
        public Sonuc GenelGorevYetkiKaydet(Kullanici kullanan, GenelGorevYetki genelGorevYetki)
        {
            return GenelGorevIslemler.GenelGorevYetkiKaydet(null, kullanan, genelGorevYetki);
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
            return GenelGorevIslemler.GenelGorevKisileriEkle(null, kullanan, organizasyonBirim, birimGenelGorev, personeller);
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
            return GenelGorevIslemler.GenelGorevKisileriListele(null, kullanan, organizasyonBirim, birimGenelGorev);
        }

        /// <summary>
        /// Kişi Genel Görevleri Listele
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">KisiGorev nesnesi</param>
        /// <returns></returns>
        public KisiGorev[] KisiGenelGorevleriListele(Kullanici kullanan, KisiGorev bilgi)
        {
            return GenelGorevIslemler.KisiGenelGorevleriListele(null, kullanan, bilgi);
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
            return GenelGorevIslemler.GenelGorevKisileriSil(null, kullanan, organizasyonBirim, birimGenelGorev, personeller);
        }

        public Sonuc GenelGorevKisiKisitEkle(Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev, string tckimlikno, GorevKisitDeger[] gorevKisitlar)
        {
            return GenelGorevIslemler.GenelGorevKisiKisitEkle(null, kullanan, organizasyonBirim, birimGenelGorev, tckimlikno, gorevKisitlar);
        }

        public GorevKisitDeger[] GenelGorevKisiKisitListele(Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev, string tckimlikno)
        {
            return GenelGorevIslemler.GenelGorevKisiKisitListele(null, kullanan, organizasyonBirim, birimGenelGorev, tckimlikno);
        }

        /// <summary>
        /// Kişiye ait görevleri sil
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgiler">Slinecek kişi görevleri</param>
        /// <returns></returns>
        public Sonuc KisiGenelGorevleriSil(Kullanici kullanan, KisiGorev[] bilgiler)
        {
            return GenelGorevIslemler.KisiGenelGorevleriSil(null, kullanan, bilgiler);
        }

        public Sonuc GenelGorevKisiTemizle(Kullanici kullanan, string tcKimlikNo)
        {
            return GenelGorevIslemler.GenelGorevKisiTemizle(null, kullanan, tcKimlikNo);
        }

        /// <summary>
        /// Personels the gorev yetki bul.
        /// </summary>
        /// <param name="tcKimlikNo">The tc kimlik no.</param>
        /// <returns></returns>
        public Kullanici PersonelGorevYetkiBul(string tcKimlikNo)
        {
            return GenelGorevIslemler.PersonelGorevYetkiBul(null, tcKimlikNo);
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
            return GenelGorevIslemler.GenelGorevYetkiSil(null, kullanan, orgKod, gorevSiraNo, yazSiraNo);
        }

        public CografyaBirimSayisi[] BirimTurSayisiListele(Kullanici kullanan, int il1Ilce2Ulke3, string[] turKodlar, string[] ilKodlar)
        {
            return ORGBirim.BirimTurSayisiListele(null, kullanan, il1Ilce2Ulke3, turKodlar, ilKodlar);
        }

        public bool MuhasebeBirimYetkiVarMi(Kullanici kullanan, string muhasebe, string birim, ENUMOGY yetkiTur)
        {
            TNS.ORG.OrganizasyonBirim ob = new ORG.OrganizasyonBirim();
            ob.muhasebekodu = muhasebe;
            ob.muhasebebirim = birim;
            ob.harcamaMi = 1;

            ObjectArray oa = OrganizasyonBirimListele(ob, false, 0, false);
            if (!oa.sonuc.islemSonuc)
            {
                return false;
            }
            if (oa.ObjeSayisi == 0)
            {
                return false;
            }
            return OrganizasyonBirimYetkiVarMi(kullanan, (OrganizasyonBirim)oa.objeler[0], yetkiTur);
        }

        public bool OrganizasyonBirimYetkiVarMi(Kullanici kullanan, OrganizasyonBirim organizasyonBirim, ENUMOGY yetkiTur)
        {
            Sonuc s = GenelGorevIslemler.KullananBirimeKayitYapabilirMi(kullanan, organizasyonBirim);
            return s.islemSonuc;
        }

        public Sonuc KisiBirimiGorebilirMiKopru(Kullanici kullanan, int muhasebeKod, string birimKod)
        {
            string kopruKodnv = kullanan.YetkiTipiDegeriBul("KopruKod");
            if (string.IsNullOrWhiteSpace(kopruKodnv))
                return new Sonuc(false, "", "");
            string[] kopruKodDizi = kopruKodnv.Split(';', ',');

            TNS.ORG.IORGServis orgs = TNS.ORG.Arac.Tanimla();
            ORG.OrganizasyonBirim ob = new ORG.OrganizasyonBirim();
            ob.muhasebekodu = muhasebeKod.ToString();
            ob.muhasebebirim = birimKod;
            ObjectArray birimler = orgs.OrganizasyonBirimListele(ob, false, false);
            if (!birimler.sonuc.islemSonuc || birimler.ObjeSayisi <= 0)
                return new Sonuc(false, "", "");

            ob = (ORG.OrganizasyonBirim)birimler[0];

            foreach (string kopruKod in kopruKodDizi)
            {
                if (!string.IsNullOrWhiteSpace(kopruKod))
                    if (ob.kopruKod.StartsWith(kopruKod))
                        return new Sonuc(true, "", "");
            }
            return new Sonuc(false, "", "");
        }

        public Sonuc KopruKodGuncelle(Kullanici kullanan)
        {
            return ORGBirim.KopruKodGuncelle(null, kullanan);
        }
    }
}

namespace TNS
{
    /// <summary>
    /// 
    /// </summary>
    public class KYMListe : YetkiTipiListesi
    {
        /// <summary>
        /// Organizasyons the listesi.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <returns></returns>
        public static ListeSatir[] OrganizasyonListesi(Kullanici kullanan)
        {
            IORGServis servisORG = TNS.ORG.Arac.Tanimla();

            OrganizasyonBirim kriter = new OrganizasyonBirim();
            ObjectArray dler = servisORG.OrganizasyonBirimListele(kriter, true, false);
            ListeSatir[] donusDeger = new ListeSatir[dler.ObjeSayisi];

            if (dler.sonuc.islemSonuc)
            {
                int sayac = 0;
                foreach (OrganizasyonBirim d in dler.objeler)
                {
                    ListeSatir l = new ListeSatir();
                    l.ad = d.adi;
                    l.kod = d.kod.ToString();
                    donusDeger[sayac] = l;
                    sayac++;
                }
            }

            return donusDeger;
        }

        /// <summary>
        /// Yetkis the tipi liste elemanlari.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="ekParamCagri">The ek param cagri.</param>
        /// <param name="ekParamTanim">The ek param tanim.</param>
        /// <returns></returns>
        public ListeSatir[] YetkiTipiListeElemanlari(Kullanici kullanan, int yil, string ekParamCagri, string ekParamTanim)
        {
            if (ekParamTanim.Equals("12"))
                return OrganizasyonListesi(kullanan);
            throw new NotImplementedException("Bilinmeyen parametre!");
        }
    }
}