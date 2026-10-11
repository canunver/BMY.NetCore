using System;
using TNS.KYM;
using TNS.UZY;

namespace TNS.ATM.IsKurallari
{
    /// <summary>
    /// Alacak Takip Servis tanımlaması
    /// </summary>
    [Serializable()]
    public class ATMServis : MarshalByRefObject, IATMServis, IDuyuruListesi
    {
        /// <summary>
        /// Uzay Servis tanımlaması
        /// </summary>
        public static IUZYServis servisUZY = null;
        public static bool tabloDegisimYap = true;

        static ATMServis()
        {
            lock ("OncekiIslemiBiteneKadarBekle")
            {
                if (tabloDegisimYap)
                {
                    tabloDegisimYap = false;
                    TNS.ATM.IsKurallari.ATMTabloDegisiklikleri.Kaydet();
                }
            }
        }
        
        //static bool tabloGuncelle = false;

        /// <summary>
        /// Alacak Takip Servis sınıfının oluşturucu(Constructer) fonksiyonu
        /// </summary>
        public ATMServis()
        {
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
            {
                servisUZY = TNS.Api.ServisIstemci.Olustur<IUZYServis>(System.Configuration.ConfigurationManager.AppSettings.Get("UZYServisURL"));
            }

            string deger;
            if (uzayAlan == "")
                deger = servisUZY.UzayDegeriStr(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle);
            else
                deger = servisUZY.UzayDegeriStr(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle, uzayAlan);

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
            {
                servisUZY = TNS.Api.ServisIstemci.Olustur<IUZYServis>(System.Configuration.ConfigurationManager.AppSettings.Get("UZYServisURL"));
            }

            double deger;
            if (uzayAlan == "")
                deger = servisUZY.UzayDegeriDbl(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle);
            else
                deger = servisUZY.UzayDegeriDbl(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle, uzayAlan);

            return deger;
        }


        /// <summary>
        /// Uzay servisine bağlanarak verilen koşul ve değerlerine göre servisi çağıran ve sonucu döndüren fonksiyon
        /// </summary>
        /// <param name="uzayAd">Uzay adını belirten string tipinde sınıf</param>
        /// <param name="kosul">Koşulu belirten string tipinde sınıf</param>
        /// <param name="esitle">Koşulun eşitlenip eşitlenmeyeceğini belirten Bool tipinde sınıf</param>
        /// <param name="uzayAlan">Uzay Alanı belirten string tipinde sınıf</param>
        /// <returns>TNSDateTime tipinde sınıf döndürür</returns>
        public static TNSDateTime UzayTarihDegerGetir(string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            if (servisUZY == null)
            {
                servisUZY = TNS.Api.ServisIstemci.Olustur<IUZYServis>(System.Configuration.ConfigurationManager.AppSettings.Get("UZYServisURL"));
            }

            TNSDateTime deger;
            if (uzayAlan == "")
                deger = servisUZY.UzayDegeriTarih(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle);
            else
                deger = servisUZY.UzayDegeriTarih(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle, uzayAlan);

            return deger;
        }

        /// <summary>
        /// Faiz Tür bilgisini kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="faizTur">Faiz Tür bilgisi, FaizTur tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public Sonuc FaizTurKaydet(Kullanici kullanan, FaizTur faizTur)
        {
            try
            {
                return new FaizTurClass().Kaydet(null, kullanan, faizTur);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Faiz Tür bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="ad">Ad bilgisi, String tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        public ObjectArray FaizTurListele(Kullanici kullanan, int yil, string ad)
        {
            try
            {
                return new FaizTurClass().Listele(null, kullanan, yil, ad);
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
        /// Alacak Takip dosyasını kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="alacakTakip">Alacak Takip dosya bilgisi, Dosya tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public Sonuc DosyaKaydet(Kullanici kullanan, Dosya alacakTakip)
        {
            try
            {
                return new DosyaClass().Kaydet(null, kullanan, alacakTakip);
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
        /// Alacak Takip dosyalarını listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="alacakTakip">Alacak Takip Dosya bilgisi, Dosya tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        public ObjectArray DosyaListele(Kullanici kullanan, Dosya alacakTakip)
        {
            try
            {
                return DosyaClass.Listele(null, kullanan, alacakTakip);
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
        /// Yıla göre Tebliğ Tarihi Geçmiş Dosyaları listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        public ObjectArray TebligTarihiGecmisDosyaListele(Kullanici kullanan, int yil)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new DosyaClass().TebligTarihiGecmisDosyaListele(null, kullanan, yil);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }


        /// <summary>
        /// Alacak Takip Dosya durum tarihçesini kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="durumTarihce">Durum tarihçe bilgisi, TNSCollection tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public Sonuc DosyaDurumTarihceKaydet(Kullanici kullanan, TNSCollection durumTarihce)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new DosyaDurumTarihceClass().Kaydet(null, kullanan, durumTarihce);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }


        /// <summary>
        /// Alacak takip dosya durum tarihçe bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="dosyaNo">Dosya numarası bilgisi, Int tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        public ObjectArray DosyaDurumTarihceListele(Kullanici kullanan, int yil, int dosyaNo)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new DosyaDurumTarihceClass().Listele(null, kullanan, yil, dosyaNo);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }


        /// <summary>
        /// Alacak takip durumunu listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="hepsiMi">Hepsimi bilgisi, Bool tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        public ObjectArray DurumListele(Kullanici kullanan, bool hepsiMi)
        {
            try
            {
                return new DurumClass().Listele(null, kullanan, hepsiMi);
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
        /// Ödeme planı hazırlayan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="odemePlani">Ödeme Planı bilgisi, OdemePlani tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public Sonuc OdemePlaniHazirla(Kullanici kullanan, OdemePlani odemePlani)
        {
            try
            {
                return DosyaTaksitClass.OdemePlaniHazirla(null, kullanan, odemePlani);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Ödeme Planına göre taksit ekleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="odemePlani">Ödeme Planı bilgisi, OdemePlani tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public Sonuc TaksitEkle(Kullanici kullanan, OdemePlani odemePlani)
        {
            try
            {
                return DosyaTaksitClass.TaksitEkle(null, kullanan, odemePlani);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Taksitlerin ödenme durumunu döndüren fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="taksit">Taksit bilgisi, TNSCollection tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public Sonuc TaksitOdendi(Kullanici kullanan, Dosya taksit)
        {
            try
            {
                return DosyaTaksitClass.Odendi(null, kullanan, taksit);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }


        /// <summary>
        /// Yıl ve Dosya numarası bilgilerine göre taksitleri listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="dosyaNo">Dosya numarası bilgisi, Int tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        public ObjectArray TaksitListele(Kullanici kullanan, int yil, int dosyaNo)
        {
            try
            {
                return DosyaTaksitClass.Listele(null, kullanan, yil, dosyaNo);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Para Birimi ve Alış/Satış durumuna göre döviz kurunu getiren fonksiyon
        /// </summary>
        /// <param name="pb">Para birimi bilgisi, ENUMParaBirimi tipinde sınıf</param>
        /// <param name="alis">Alış/Satış Bilgisi, Bool tipinde sınıf</param>
        /// <returns>Double tipinde sınıf döndürür</returns>
        public double DovizKurGetir(ENUMParaBirimi pb, bool alis)
        {
            double donusDeger = 0;
            try
            {
                donusDeger = Genel.DovizKurGetir(pb, alis);
            }
            catch (Exception)
            {
                donusDeger = -1;
            }
            return donusDeger;
        }

        /// <summary>
        /// Odemes the emri kaydet dosya.
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <param name="dosyaNo">The dosya no.</param>
        /// <param name="belgeYil">The belge yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <returns></returns>
        public Sonuc OdemeEmriKaydetDosya(int yil, int dosyaNo, int belgeYil, string belgeNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new DosyaClass().OdemeEmriKaydetDosya(null, yil, dosyaNo, belgeYil, belgeNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Odemes the emri kaydet taksit.
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <param name="dosyaNo">The dosya no.</param>
        /// <param name="siraNo">The sira no.</param>
        /// <param name="belgeYil">The belge yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <param name="faizMi">if set to <c>true</c> [faiz mi].</param>
        /// <returns></returns>
        public Sonuc OdemeEmriKaydetTaksit(int yil, int dosyaNo, int siraNo, int belgeYil, string belgeNo, bool faizMi)
        {
            try
            {
                return DosyaTaksitClass.OdemeEmriKaydetTaksit(null, yil, dosyaNo, siraNo, belgeYil, belgeNo, faizMi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// HesapHareket dosyasını kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="hesapHareketAna">The hesap hareket ana.</param>
        /// <returns>
        /// Sonuc tipinde sınıf döndürür
        /// </returns>
        public Sonuc DosyaHesapHareketAnaKaydet(Kullanici kullanan, DosyaHesapHareketAna hesapHareketAna)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new DosyaHesapHareketAnaClass().Kaydet(null, kullanan, hesapHareketAna);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Dosyas the hesap hareket ana odeme emri no kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="hesapHareketAnalist">The hesap hareket analist.</param>
        /// <returns></returns>
        public Sonuc DosyaHesapHareketAnaOdemeEmriNoKaydet(Kullanici kullanan, ObjectArray hesapHareketAnalist)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new DosyaHesapHareketAnaClass().KaydetOdemeEmriNo(null, kullanan, hesapHareketAnalist);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// DosyaHesapHareket dosyalarını listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="hesapHareketAna">The hesap hareket ana.</param>
        /// <returns>
        /// ObjectArray tipinde sınıf döndürür
        /// </returns>
        public ObjectArray DosyaHesapHareketAnaListele(Kullanici kullanan, DosyaHesapHareketAna hesapHareketAna)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new DosyaHesapHareketAnaClass().Listele(null, kullanan, hesapHareketAna);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// DosyaHesapHareket dosyalarını listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="hesapHareketAna">The hesap hareket ana.</param>
        /// <returns>
        /// sonuc tipinde sınıf döndürür
        /// </returns>
        public Sonuc DosyaHesapHareketAnaSil(Kullanici kullanan, DosyaHesapHareketAna hesapHareketAna)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new DosyaHesapHareketAnaClass().Sil(null, kullanan, hesapHareketAna);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// DosyaHesapHareketTasarim dosyasını kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="listeHesapHareketTasarim">The liste hesap hareket tasarim.</param>
        /// <returns>
        /// Sonuc tipinde sınıf döndürür
        /// </returns>
        public Sonuc DosyaHesapHareketTasarimKaydet(Kullanici kullanan, ObjectArray listeHesapHareketTasarim)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new DosyaHesapHareketTasarimClass().Kaydet(null, kullanan, listeHesapHareketTasarim);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// DosyaHesapHareketTasarim dosyalarını listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="hesapHareketTasarim">The hesap hareket tasarim.</param>
        /// <returns>
        /// ObjectArray tipinde sınıf döndürür
        /// </returns>
        public ObjectArray DosyaHesapHareketTasarimListele(Kullanici kullanan, DosyaHesapHareketTasarim hesapHareketTasarim)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new DosyaHesapHareketTasarimClass().Listele(null, kullanan, hesapHareketTasarim);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// DosyaHesapHareketTasarim dosyalarını listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="hesapHareketTasarim">DosyaHesapHareketTasarim tipinde sınıf</param>
        /// <returns>sonuc tipinde sınıf döndürür</returns>
        public Sonuc DosyaHesapHareketTasarimSil(Kullanici kullanan, DosyaHesapHareketTasarim hesapHareketTasarim)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new DosyaHesapHareketTasarimClass().Sil(null, kullanan, hesapHareketTasarim);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }


        /// <summary>
        /// DosyaHesapHareketCocuk dosyasını kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="listeHesapHareketCocuk">The liste hesap hareket cocuk.</param>
        /// <returns>
        /// Sonuc tipinde sınıf döndürür
        /// </returns>
        public Sonuc DosyaHesapHareketCocukKaydet(Kullanici kullanan, ObjectArray listeHesapHareketCocuk)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new DosyaHesapHareketCocukClass().Kaydet(null, kullanan, listeHesapHareketCocuk);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// DosyaHesapHareketCocuk dosyalarını listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="hesapHareketCocuk">The hesap hareket cocuk.</param>
        /// <returns>
        /// ObjectArray tipinde sınıf döndürür
        /// </returns>
        public ObjectArray DosyaHesapHareketCocukListele(Kullanici kullanan, DosyaHesapHareketCocuk hesapHareketCocuk)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new DosyaHesapHareketCocukClass().Listele(null, kullanan, hesapHareketCocuk);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// hesapHareketCocuk dosyalarını listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="hesapHareketCocuk">hesapHareketCocuk tipinde sınıf</param>
        /// <returns>sonuc tipinde sınıf döndürür</returns>
        public Sonuc DosyaHesapHareketCocukSil(Kullanici kullanan, DosyaHesapHareketCocuk hesapHareketCocuk)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new DosyaHesapHareketCocukClass().Sil(null, kullanan, hesapHareketCocuk);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// DosyaHesapHareketAna dosyalarını listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="hesapHareketAna">DosyaHesapHareketAna tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        public ObjectArray DosyaHesapHareketAnaDosyaListele(Kullanici kullanan, DosyaHesapHareketAna hesapHareketAna)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new DosyaHesapHareketAnaClass().ListeleDosya(null, kullanan, hesapHareketAna);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// DosyaHesapHareketTasarimAna dosyasını kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="DosyaHesapHareketTasarimAna">DosyaHesapHareketTasarimAna tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public Sonuc DosyaHesapHareketTasarimAnaKaydet(Kullanici kullanan, DosyaHesapHareketTasarimAna hesapHareketTasarimAna)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new DosyaHesapHareketTasarimAnaClass().Kaydet(null, kullanan, hesapHareketTasarimAna);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// DosyaHesapHareketTasarimAna dosyalarını listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="DosyaHesapHareketTasarimAna">DosyaHesapHareketTasarimAna tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        public ObjectArray DosyaHesapHareketTasarimAnaListele(Kullanici kullanan, DosyaHesapHareketTasarimAna hesapHareketTasarimAna)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new DosyaHesapHareketTasarimAnaClass().Listele(null, kullanan, hesapHareketTasarimAna);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// DosyaHesapHareketTasarimAna dosyalarını listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="hesapHareketTasarimAna">DosyaHesapHareketTasarimAna tipinde sınıf</param>
        /// <returns>sonuc tipinde sınıf döndürür</returns>
        public Sonuc DosyaHesapHareketTasarimAnaSil(Kullanici kullanan, DosyaHesapHareketTasarimAna hesapHareketTasarimAna)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new DosyaHesapHareketTasarimAnaClass().Sil(null, kullanan, hesapHareketTasarimAna);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray DosyaTarihceListeleAB(Kullanici kullanan, int yil, int dosyaNo)
        {
            return ATMDosyaAB.TarihceListele(null, kullanan, yil, dosyaNo);
        }

        /// <summary>
        /// DosyaAB nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DosyaAB yapısında DosyaAB: Borç Dosyası bilgilerini tutan değişken</param>
        public ObjectArray ListeleDosyaAB(Kullanici kullanan, DosyaAB form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.Listele(null, kullanan, form, true);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// DosyaAB nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DosyaAB yapısında DosyaAB: Borç Dosyası bilgilerini tutan değişken</param>
        public ObjectVAN OkuDosyaAB(Kullanici kullanan, DosyaAB form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.Oku(null, kullanan, form, true);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        ///// <summary>
        ///// DosyaAB nesnesini veri tabanına yazan fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">DosyaAB yapısında DosyaAB: Borç Dosyası bilgilerini tutan değişken</param>
        ///// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        //public Sonuc KaydetDosyaAB(Kullanici kullanan, DosyaAB form)
        //{
        //    try
        //    {
        //        TNS.KYM.Arac.KulturDegistir(kullanan);
        //        return ATMDosyaAB.Kaydet(null, kullanan, form);
        //    }
        //    catch (Exception e)
        //    {
        //        return new Sonuc(false, e.ToString(), "");
        //    }
        //}

        public Sonuc KaydetDosyaABBorcHareket(Kullanici kullanan, int iPARDDonemi, int dosyaAnahtar, BorcHareket bh, ENUMDosyaIslemTur islemTur)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.KaydetBorcHareket(null, kullanan, iPARDDonemi, dosyaAnahtar, bh, islemTur);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray ListeleDosyaABBorcHareket(Kullanici kullanan, int iPARDDonemi, int dosyaAnahtar, BorcHareket bh)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                DosyaAB dosyaOkunan;
                return ATMDosyaAB.ListeleBorcHareket(null, kullanan, iPARDDonemi, dosyaAnahtar, bh, ENUMDosyaIslemTur.Listeleme, out dosyaOkunan);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public Sonuc SilDosyaABBorcHareket(Kullanici kullanan, int iPARDDonemi, int dosyaAnahtar, int siraNo)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.SilBorcHareket(null, kullanan, iPARDDonemi, dosyaAnahtar, siraNo);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public Sonuc DosyaFaiziHesapla(Kullanici kullanan, int iPARDDonemi, int dosyaAnahtar, TNSDateTime hesapTarihi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.DosyaFaiziHesapla(null, kullanan, iPARDDonemi, dosyaAnahtar, hesapTarihi);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public Sonuc DosyalarinFaiziniHesapla(Kullanici kullanan, TNSDateTime hesapTarihi, DosyaAB form, int hesaplamaTur)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.DosyalarinFaiziniHesapla(null, kullanan, hesapTarihi, form, hesaplamaTur);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public double DovizKuruBulOncAy(Kullanici kullanan, TNSDateTime kurTarihi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.DovizKuruBulOncAy(kullanan, kurTarihi);
            }
            catch
            {
                return 0;
            }
        }

        //public Sonuc TaksitlendirDosyaAB(Kullanici kullanan, DosyaAB alacakTakip, int taksitSay)
        //{
        //    try
        //    {
        //        TNS.KYM.Arac.KulturDegistir(kullanan);
        //        return ATMDosyaAB.Taksitlendir(null, kullanan, alacakTakip, taksitSay);
        //    }
        //    catch (Exception e)
        //    {
        //        return new Sonuc(e.Message);
        //    }
        //}

        //public Sonuc TaksitKaldirDosyaAB(Kullanici kullanan, DosyaAB alacakTakip)
        //{
        //    try
        //    {
        //        TNS.KYM.Arac.KulturDegistir(kullanan);
        //        return ATMDosyaAB.TaksitKaldir(null, kullanan, alacakTakip);
        //    }
        //    catch (Exception e)
        //    {
        //        return new Sonuc(e.Message);
        //    }
        //}

        public ObjectArray DuyuruListesiAl(Kullanici kullanan, string kullaniciTipleri)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.DuyuruListesiAl(null, kullanan, kullaniciTipleri);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc = new Sonuc(e.Message);
                OrtakFonksiyonlar.HataDosyaYaz("Duyuru.txt", e.Message, false);

                return oa;
            }
        }

        /// <summary>
        /// OnBorc nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        public ObjectArray ListeleOnBorcForm(Kullanici kullanan, OnBorc form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.OnBorcListele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// OnBorc nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        public ObjectVAN OkuOnBorcForm(Kullanici kullanan, OnBorc form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.OnBorcOku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// OnBorc nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetOnBorcForm(Kullanici kullanan, OnBorc form, ENUMDosyaIslemTur islemTur)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.OnBorcKaydet(null, kullanan, form, islemTur);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// OnBorc nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        public ObjectVAN OkuGAKForm(Kullanici kullanan, GeriAlimKarari form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.GAKOku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// OnBorc nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetGAKForm(Kullanici kullanan, GeriAlimKarari form, ENUMDosyaIslemTur islemTur)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.GAKKaydet(null, kullanan, form, islemTur);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// OnBorc nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        public ObjectVAN OkuDurumForm(Kullanici kullanan, DosyaABDurum form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.DurumOku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// OnBorc nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetDurumForm(Kullanici kullanan, DosyaABDurum form, ENUMDosyaIslemTur islemTur)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.DurumKaydet(null, kullanan, form, islemTur);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// OnBorc nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        public ObjectVAN OkuTaksitForm(Kullanici kullanan, DosyaABTaksit form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.ABTaksitOku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// OnBorc nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetTaksitForm(Kullanici kullanan, DosyaABTaksit form, ENUMDosyaIslemTur islemTur)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.TaksitKaydet(null, kullanan, form, islemTur);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc KaydetBorcluForm(Kullanici kullanan, ABBorclu form, ENUMDosyaIslemTur islemTur)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.ABBorcluKaydet(null, kullanan, form, islemTur);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectVAN OkuBorcluForm(Kullanici kullanan, ABBorclu form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ATMDosyaAB.ABBorcluOku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }
    }

    public class ATMYetki : TNS.KYM.IYetki
    {
        public Sonuc YetkiKontrolu(Kullanici kullanan, ENUMIslemTuru islemTur, string oncekiKaydeden, int saymanlikKod, string birimKod, string kurumsalKod, string ekBilgi)
        {
            ENUMDosyaIslemTur dosyaIslemTur;
            if (islemTur == ENUMIslemTuru.KAYIT)
                dosyaIslemTur = ENUMDosyaIslemTur.Kaydet;
            else if (islemTur == ENUMIslemTuru.ONAYLAMA)
            {
                dosyaIslemTur = ENUMDosyaIslemTur.Onayla;
                if (oncekiKaydeden == kullanan.kullaniciKodu)
                    return new Sonuc("Dosyayı yükleyen ve onaylayan aynı kişi olamaz!");
            }
            else if (islemTur == ENUMIslemTuru.ONAYKALDIRMA)
                dosyaIslemTur = ENUMDosyaIslemTur.Onayla;
            else if (islemTur == ENUMIslemTuru.SILME)
                dosyaIslemTur = ENUMDosyaIslemTur.IptalEt;
            else
                dosyaIslemTur = ENUMDosyaIslemTur.Listeleme;

            return Yetki.DosyaYazmaOnaylamaHakkiVarMi(kullanan, saymanlikKod, birimKod, dosyaIslemTur);
        }
    }
}
