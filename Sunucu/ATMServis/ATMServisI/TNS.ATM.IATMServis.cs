using System;
using TNS.KYM;
using TNS.MUH;

namespace TNS.ATM
{
    /// <summary>
    /// Bu sunucu sınıfı için, istemci ve sunucuda ortak kullanılacak statik fonksiyonlar içeren sınıf
    /// </summary>
    [Serializable()]
    public static class Arac
    {
        /// <summary>
        /// Bu sunucu sınıfının nesnesini .net remoting ile oluşturup döndüren statik fonksiyon
        /// </summary>
        /// <returns>.net remoting ile oluşturulan nesne</returns>
        public static IATMServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("ATMServisURL");
            IATMServis servis = TNS.Api.ServisIstemci.Olustur<IATMServis>(sUrl);
            return servis;
        }

        public static double BorcBul(Dosya d, string varsayilanKurumKod)
        {
            if (MuhasebedenGelsin(varsayilanKurumKod))
                return d.muhasebeBorcDurumu;
            else
                return d.borc;
        }

        public static bool MuhasebedenGelsin(string varsayilanKurumKod)
        {
            if (varsayilanKurumKod == "460604")
                return true;
            return false;
        }

        public static double FaizOrani(double faizOran, DateTime odemeTarihi, DateTime faizBaslangicTarihi, DateTime taksitBaslangicTarihi, DateTime taksitFaizBulmaTarihi, int tacilFaizNo)
        {
            int ay, gunInt;
            FaizIslemler.FaizDonem[] faizOranlar;
            bool gibYontem = false;
            if (OrtakFonksiyonlar.EsitMi(faizOran, 0) || OrtakFonksiyonlar.EsitMi(faizOran, 99))
            {
                if (taksitBaslangicTarihi > DateTime.MinValue && odemeTarihi >= taksitBaslangicTarihi)
                {
                    faizOranlar = TNS.FaizIslemler.FaizOranAyBul(taksitBaslangicTarihi, tacilFaizNo);
                }
                else
                {
                    faizOranlar = TNS.FaizIslemler.faizOran;
                    gibYontem = true;
                }
            }
            else if (OrtakFonksiyonlar.EsitMi(faizOran, 9))
            {
                faizOranlar = new FaizIslemler.FaizDonem[3];
                faizOranlar[0] = new FaizIslemler.FaizDonem(new DateTime(2024, 6, 1), 9.0 / 1200);
                faizOranlar[1] = new FaizIslemler.FaizDonem(new DateTime(2026, 8, 1), 24.0 / 1200);
                faizOranlar[2] = new FaizIslemler.FaizDonem(DateTime.MaxValue, 31.0 / 1200);
            }
            else
            {
                faizOranlar = new FaizIslemler.FaizDonem[1];
                faizOranlar[0] = new FaizIslemler.FaizDonem(DateTime.MaxValue, faizOran / 1200);
            }
            return TNS.FaizIslemler.FaizOranHesapla(faizBaslangicTarihi, faizBaslangicTarihi, odemeTarihi, DateTime.MinValue, faizOranlar, gibYontem, null, out ay, out gunInt);
        }

        internal static double ABOranDondur(int ipardDonemi)
        {
            if (ipardDonemi == 3) return 0.5;
            else return 0.75;
        }
    }

    /// <summary>
    /// Bu sunucu sınıfı için, istemci ve sunucuda ortak kullanılacak yetki ile ilgili statik fonksiyonlar içeren sınıf
    /// </summary>
    [Serializable()]
    public static class Yetki
    {
        /// <summary>
        /// Sayfaya kullanıcının giriş yapabilmesi durumunu kontrol eden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <returns>Bool tipinde sınıf döndürür</returns>
        public static bool SayfayaGirisYapabilirMi(Kullanici kullanan)
        {
            return GormeYetkisiVarMi(kullanan) || YoneticiMi(kullanan) || KullaniciYoneticiMi(kullanan);
        }

        /// <summary>
        /// Kullanıcının sistem yönetici olması durumunu kontrol eden fonksiyon
        /// </summary>
        /// <param name="kullanan">Yetki kontrolünden geçecek kullanici bilgileri, Kullanici tipinde sınıf</param>
        /// <returns>Bool tipinde sınıf döndürür</returns>
        public static bool YoneticiMi(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        /// <summary>
        /// Kullanıcının kullanıcı yönetici olması durumunu kontrol eden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanıcı tipinde sınıf</param>
        /// <returns>Bool tipinde sınıf döndürür</returns>
        public static bool KullaniciYoneticiMi(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI);
        }

        /// <summary>
        /// Kullanıcının Alacak Takibi modülünü görme yetkisi durumunu kontrol eden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanıcı tipinde sınıf</param>
        /// <returns>Bool tipinde sınıf döndürür</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan)
        {
            return
                kullanan.YetkiTipiDegeriIceriyorMu("Alacak Takibi Yetkisi", "1", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Alacak Takibi Yetkisi", "2", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Alacak Takibi Yetkisi", "3", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Alacak Takibi Yetkisi", "4", false);
        }

        /// <summary>
        /// Kullanıcının Alacak Takibi modülüne yazma yetkisi durumunu kontrol eden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanıcı tipinde sınıf</param>
        /// <returns>Bool tipinde sınıf döndürür</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            return
                kullanan.YetkiTipiDegeriIceriyorMu("Alacak Takibi Yetkisi", "2", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Alacak Takibi Yetkisi", "4", false);
        }

        /// <summary>
        /// Kullanıcının Alacak Takibi modülünü onaylama yetkisi durumunu kontrol eden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanıcı tipinde sınıf</param>
        /// <returns>Bool tipinde sınıf döndürür</returns>
        public static bool OnaylamaYetkisiVarMi(Kullanici kullanan)
        {
            return
                kullanan.YetkiTipiDegeriIceriyorMu("Alacak Takibi Yetkisi", "3", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Alacak Takibi Yetkisi", "4", false);
        }

        /// <summary>
        /// Kullanıcının dosya yazma hakkı durumunu kontrol eden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="saymanlikKodu">Saymanlık kodu bilgisi, Int tipinde sınıf</param>
        /// <param name="birimKodu">Birim kodu bilgisi, String tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public static Sonuc DosyaYazmaOnaylamaHakkiVarMi(Kullanici kullanan, int saymanlikKodu, string birimKodu, ENUMDosyaIslemTur islemTur)
        {
            Sonuc donusDeger = new Sonuc();
            if (YoneticiMi(kullanan))
            {
                return donusDeger;
            }

            if (!SaymanlikVeBirimHakkiMi(kullanan, saymanlikKodu, birimKodu))
            {
                if (!(KullaniciYoneticiMi(kullanan) && OrtakFonksiyonlar.ConvertToInt(kullanan.calistigiKurum, 0) == saymanlikKodu))
                {
                    donusDeger.DegerAta(AlacakTakipResI.ATA051);
                    return donusDeger;
                }
            }

            if (islemTur == ENUMDosyaIslemTur.Kaydet || islemTur == ENUMDosyaIslemTur.IptalEt)
            {
                if (!YazmaYetkisiVarMi(kullanan))
                {
                    donusDeger.DegerAta("Kaydetme yetkiniz bulunmamaktadır!");
                }
            }
            else
            {
                if (!OnaylamaYetkisiVarMi(kullanan))
                {
                    donusDeger.DegerAta("Onaylama yetkiniz bulunmamaktadır!");
                }
            }

            return donusDeger;
        }

        /// <summary>
        /// Kullanıcının faiz tür yazma hakkı durumunu kontrol eden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public static Sonuc FaizTurYazmaHakkiVarMi(Kullanici kullanan)
        {
            Sonuc donusDeger = new Sonuc(false, AlacakTakipResI.ATA052, "");

            if (YoneticiMi(kullanan))
            {
                donusDeger.islemSonuc = true;
                donusDeger.hataStr = "";
            }

            return donusDeger;
        }

        /// <summary>
        /// Kullanıcının yazma hakkı durumunu kontrol eden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public static Sonuc YazmaHakkiVarMi(Kullanici kullanan)
        {
            Sonuc donusDeger = new Sonuc(false, AlacakTakipResI.ATA053, "");

            if (YoneticiMi(kullanan) || YazmaYetkisiVarMi(kullanan))
            {
                donusDeger.islemSonuc = true;
                donusDeger.hataStr = "";
            }

            return donusDeger;
        }

        /// <summary>
        /// Kullanıcının saymanlık ve birim koduna göre dosya okuma hakkı durumunu kontrol eden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanıcı tipinde sınıf</param>
        /// <param name="saymanlikKodu">Saymanlık kodu bilgisini tutan, Int tipinde sınıf</param>
        /// <param name="birimKodu">Birim bilgisini tutan, String tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public static Sonuc DosyaOkumaHakkiVarMi(Kullanici kullanan, int saymanlikKodu, string birimKodu)
        {
            Sonuc donusDeger = new Sonuc(false, AlacakTakipResI.ATA054, "");

            if (YoneticiMi(kullanan) || (GormeYetkisiVarMi(kullanan) && SaymanlikVeBirimHakkiMi(kullanan, saymanlikKodu, birimKodu)))
            {
                donusDeger.islemSonuc = true;
                donusDeger.hataStr = "";
            }

            if (KullaniciYoneticiMi(kullanan))
            {
                if (OrtakFonksiyonlar.ConvertToInt(kullanan.calistigiKurum, 0) == saymanlikKodu)
                {
                    donusDeger.islemSonuc = true;
                    donusDeger.hataStr = "";
                }
            }

            return donusDeger;
        }

        /// <summary>
        /// Kullanıcının okuma hakkı durumunu kontrol eden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public static Sonuc OkumaHakkiVarMi(Kullanici kullanan)
        {
            Sonuc donusDeger = new Sonuc(false, AlacakTakipResI.ATA055, "");

            if (YoneticiMi(kullanan) || GormeYetkisiVarMi(kullanan))
            {
                donusDeger.islemSonuc = true;
                donusDeger.hataStr = "";
            }

            return donusDeger;
        }

        /// <summary>
        /// Kullanıcının saymanlık ve birim koduna göre Saymanlık ve Birim hakkı durumunu kontrol eden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanıcı tipinde sınıf</param>
        /// <param name="saymanlik">Saymanlık bilgisini tutan, Int tipinde sınıf</param>
        /// <param name="birimKodu">Birim bilgisini tutan, String tipinde sınıf</param>
        /// <returns>Bool türünde sınıf döndürür</returns>
        private static bool SaymanlikVeBirimHakkiMi(Kullanici kullanan, int saymanlik, string birimKodu)
        {
            if (KullaniciYoneticiMi(kullanan))
            {
                if (OrtakFonksiyonlar.ConvertToInt(kullanan.calistigiKurum, 0) == saymanlik)
                    return true;
            }

            //bool donusDeger = false;
            //if (!string.IsNullOrWhiteSpace(birimKodu) && birimKodu != "*")
            //{
            //    ObjectArray mbVAN = KurumdanSaymanlikVeBirimKodlariDondur(kullanan);

            //    if (!mbVAN.sonuc.islemSonuc)
            //        return false;
            //    foreach (MuhasebeBirim mb in mbVAN.objeler)
            //    {
            //        if (mb.muhasebe == saymanlik && mb.birimKod == birimKodu)
            //            donusDeger = true;
            //        else if (mb.muhasebe == saymanlik && mb.birimKod == "*")
            //            donusDeger = true;
            //        else if (mb.muhasebe == 0 && mb.birimKod == birimKodu)
            //            donusDeger = true;
            //        else if (mb.muhasebe == 0 && mb.birimKod == "*")
            //            donusDeger = true;
            //    }

            //    if (!donusDeger)
            //        return false;
            //}
            string[] parca = SaymanlikVeBirimKodlariDondur(kullanan).Split(';');
            foreach (string s in parca)
            {
                string[] saymanlikBirim = s.Split('-');
                if (OrtakFonksiyonlar.ConvertToInt(saymanlikBirim[0], 0) == saymanlik && saymanlikBirim[1] == birimKodu)
                    return true;
                else if (OrtakFonksiyonlar.ConvertToInt(saymanlikBirim[0], 0) == saymanlik && saymanlikBirim[1] == "*")
                    return true;
                else if (saymanlikBirim[0] == "*" && saymanlikBirim[1] == birimKodu)
                    return true;
                else if (saymanlikBirim[0] == "*" && saymanlikBirim[1] == "*")
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Konfigürasyonda yazan Muhasebe servisine bağlanarak Kullanıcının Alacak Takibi yapacağı harcama birimlerini döndüren fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan Kullanıcı tipinde sınıf</param>
        /// <returns>MuhasebeBirimlerVan tipinde sınıf döndürür</returns>
        private static ObjectArray KurumdanSaymanlikVeBirimKodlariDondur(Kullanici kullanan)
        {
            IMUHServis servisMUH = TNS.Api.ServisIstemci.Olustur<IMUHServis>(System.Configuration.ConfigurationManager.AppSettings.Get("MUHServisURL"));
            return servisMUH.KullanicininHarcamaBirimleri(kullanan, "Alacak Takibi Yapacağı Birimler");
        }

        /// <summary>
        /// Kullanıcının Alacak Takibi yapacağı Saymanlık-Birim yetki değerlerini döndüren fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan Kullanıcı tipinde sınıf</param>
        /// <returns>String tipinde sınıf döndürür</returns>
        private static string SaymanlikVeBirimKodlariDondur(Kullanici kullanan)
        {
            return (string)kullanan.yetkiTipiDegerleri["Alacak Takibi Yapacağı Saymanlık-Birim"];
        }

        /// <summary>
        /// Kullanıcıya göre Saymanlık ve Birim kodlarını dizi şeklinde döndüren fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanıcı tipinde sınıf</param>
        /// <returns>String dizi tipinde sınıf döndürür</returns>
        public static string[] SaymanlikVeBirimKodlariArrayDondur(Kullanici kullanan)
        {
            string saymanlikVeBirimKodlari = SaymanlikVeBirimKodlariDondur(kullanan);
            if (saymanlikVeBirimKodlari == null)
                saymanlikVeBirimKodlari = "";

            return saymanlikVeBirimKodlari.Split(';');
        }
    }

    /// <summary>
    /// Bu sunucu sınıfı tarafından sunulan fonksiyonları belirten arayüz sınıfı
    /// </summary>
    public interface IATMServis
    {
        /// <summary>
        /// Faiz tür bilgisini kaydeder
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="faizTur">Faiz tür bilgisi, FaizTur tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        Sonuc FaizTurKaydet(Kullanici kullanan, FaizTur faizTur);

        /// <summary>
        /// Faiz türlerini listeler
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="ad">Ad bilgisi, String tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        ObjectArray FaizTurListele(Kullanici kullanan, int yil, string ad);

        /// <summary>
        /// Alacak Takip dosyasını kaydeder
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="alacakTakip">Alacak takip dosya bilgisi, Dosya tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        Sonuc DosyaKaydet(Kullanici kullanan, Dosya alacakTakip);

        /// <summary>
        /// Alacak takip dosyalarını listeler
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="alacakTakip">Alacak takip dosya bilgisi, Dosya tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        ObjectArray DosyaListele(Kullanici kullanan, Dosya alacakTakip);


        /// <summary>
        /// Tebliğ tarihi geçmiş dosyaları listeler
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        ObjectArray TebligTarihiGecmisDosyaListele(Kullanici kullanan, int yil);

        /// <summary>
        /// Alacak Takip dosyası durum tarihçesini kaydeder
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="durumTarihce">Dosya Durum tarihçe bilgisi, TNSCollection tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        Sonuc DosyaDurumTarihceKaydet(Kullanici kullanan, TNSCollection durumTarihce);

        /// <summary>
        /// Alacak takip dosyası durum tarihçesini listeler
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="dosyaNo">Dosya numarası bilgisi, Int tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        ObjectArray DosyaDurumTarihceListele(Kullanici kullanan, int yil, int dosyaNo);

        /// <summary>
        /// Ödeme Planı hazırlar
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="odemePlani">Ödeme planı bilgisi, OdemePlanı tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        Sonuc OdemePlaniHazirla(Kullanici kullanan, OdemePlani odemePlani);

        /// <summary>
        /// Ödeme planına göre taksit ekler
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="odemePlani">Ödeme planı bilgisi, OdemePlani tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        Sonuc TaksitEkle(Kullanici kullanan, OdemePlani odemePlani);

        /// <summary>
        /// Taksitlerin ödenme durumunu döndürür
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="taksit">Taksit bilgisi, TNSCollection tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        Sonuc TaksitOdendi(Kullanici kullanan, Dosya taksit);

        /// <summary>
        /// Yıl ve dosya numarasına göre Taksitleri listeler
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="dosyaNo">Dosya numarası bilgisi, Int tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        ObjectArray TaksitListele(Kullanici kullanan, int yil, int dosyaNo);
        double DovizKuruBulOncAy(Kullanici kullanan, TNSDateTime kurTarihi);

        /// <summary>
        /// Alacak takip dosya durumlarını listeler
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="hepsiMi">Hepsimi bilgisi, Bool tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        ObjectArray DurumListele(Kullanici kullanan, bool hepsiMi);

        /// <summary>
        /// Döviz birimi ve Alış/Satış tipine göre Döviz kurunu getirir
        /// </summary>
        /// <param name="pb">Para birimi bilgisi, ENUMParabirimi  tipinde sınıf</param>
        /// <param name="alis">Alış bilgisi, Bool tipinde sınıf</param>
        /// <returns>Double tipinde sınıf döndürür</returns>
        double DovizKurGetir(ENUMParaBirimi pb, bool alis);

        /// <summary>
        /// Odemes the emri kaydet dosya.
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <param name="dosyaNo">The dosya no.</param>
        /// <param name="belgeYil">The belge yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <returns></returns>
        Sonuc OdemeEmriKaydetDosya(int yil, int dosyaNo, int belgeYil, string belgeNo);
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
        Sonuc OdemeEmriKaydetTaksit(int yil, int dosyaNo, int siraNo, int belgeYil, string belgeNo, bool faizMi);

        /// <summary>
        /// Dosyas the hesap hareket ana kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="hesapHareketAna">The hesap hareket ana.</param>
        /// <returns></returns>
        Sonuc DosyaHesapHareketAnaKaydet(Kullanici kullanan, DosyaHesapHareketAna hesapHareketAna);
        /// <summary>
        /// Dosyas the hesap hareket ana odeme emri no kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="hesapHareketAnalist">The hesap hareket analist.</param>
        /// <returns></returns>
        Sonuc DosyaHesapHareketAnaOdemeEmriNoKaydet(Kullanici kullanan, ObjectArray hesapHareketAnalist);
        /// <summary>
        /// Dosyas the hesap hareket ana listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="hesapHareketAna">The hesap hareket ana.</param>
        /// <returns></returns>
        ObjectArray DosyaHesapHareketAnaListele(Kullanici kullanan, DosyaHesapHareketAna hesapHareketAna);
        /// <summary>
        /// Dosyas the hesap hareket ana dosya listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="hesapHareketAna">The hesap hareket ana.</param>
        /// <returns></returns>
        ObjectArray DosyaHesapHareketAnaDosyaListele(Kullanici kullanan, DosyaHesapHareketAna hesapHareketAna);
        /// <summary>
        /// Dosyas the hesap hareket ana sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="hesapHareketAna">The hesap hareket ana.</param>
        /// <returns></returns>
        Sonuc DosyaHesapHareketAnaSil(Kullanici kullanan, DosyaHesapHareketAna hesapHareketAna);
        /// <summary>
        /// Dosyas the hesap hareket tasarim kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="listeHesapHareketTasarim">The liste hesap hareket tasarim.</param>
        /// <returns></returns>
        Sonuc DosyaHesapHareketTasarimKaydet(Kullanici kullanan, ObjectArray listeHesapHareketTasarim);
        /// <summary>
        /// Dosyas the hesap hareket tasarim listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="hesapHareketTasarim">The hesap hareket tasarim.</param>
        /// <returns></returns>
        ObjectArray DosyaHesapHareketTasarimListele(Kullanici kullanan, DosyaHesapHareketTasarim hesapHareketTasarim);
        /// <summary>
        /// Dosyas the hesap hareket tasarim sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="hesapHareketTasarim">The hesap hareket tasarim.</param>
        /// <returns></returns>
        Sonuc DosyaHesapHareketTasarimSil(Kullanici kullanan, DosyaHesapHareketTasarim hesapHareketTasarim);
        /// <summary>
        /// Dosyas the hesap hareket cocuk kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="listeHesapHareketCocuk">The liste hesap hareket cocuk.</param>
        /// <returns></returns>
        Sonuc DosyaHesapHareketCocukKaydet(Kullanici kullanan, ObjectArray listeHesapHareketCocuk);
        /// <summary>
        /// Dosyas the hesap hareket cocuk listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="hesapHareketCocuk">The hesap hareket cocuk.</param>
        /// <returns></returns>
        ObjectArray DosyaHesapHareketCocukListele(Kullanici kullanan, DosyaHesapHareketCocuk hesapHareketCocuk);
        /// <summary>
        /// Dosyas the hesap hareket cocuk sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="hesapHareketCocuk">The hesap hareket cocuk.</param>
        /// <returns></returns>
        Sonuc DosyaHesapHareketCocukSil(Kullanici kullanan, DosyaHesapHareketCocuk hesapHareketCocuk);
        Sonuc DosyaHesapHareketTasarimAnaKaydet(Kullanici kullanan, DosyaHesapHareketTasarimAna hesapHareketTasarimAna);
        ObjectArray DosyaHesapHareketTasarimAnaListele(Kullanici kullanan, DosyaHesapHareketTasarimAna hesapHareketTasarimAna);
        Sonuc DosyaHesapHareketTasarimAnaSil(Kullanici kullanan, DosyaHesapHareketTasarimAna hesapHareketTasarimAna);

        /// <summary>
        /// DosyaAB listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DosyaAB yapısında DosyaAB: Borç Dosyası bilgilerini tutan değişken</param>
        ObjectArray ListeleDosyaAB(Kullanici kullanan, DosyaAB form);

        /// <summary>
        /// DosyaAB nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DosyaAB yapısında DosyaAB: Borç Dosyası bilgilerini tutan değişken</param>
        ObjectVAN OkuDosyaAB(Kullanici kullanan, DosyaAB form);

        ///// <summary>
        ///// DosyaAB nesnesini veri tabanına yazan fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">DosyaAB yapısında DosyaAB: Borç Dosyası bilgilerini tutan değişken</param>
        ///// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        //Sonuc KaydetDosyaAB(Kullanici kullanan, DosyaAB form);

        /// <summary>
        /// Alacak Takip dosyası durum tarihçesini kaydeder
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="durumTarihce">Dosya Durum tarihçe bilgisi, TNSCollection tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        //Sonuc DosyaDurumDegistirAB(Kullanici kullanan, TNSCollection durumTarihce);

        /// <summary>
        /// Alacak takip dosyası durum tarihçesini listeler
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="dosyaNo">Dosya numarası bilgisi, Int tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        ObjectArray DosyaTarihceListeleAB(Kullanici kullanan, int iPARDDonemi, int dosyaAnahtar);
        Sonuc KaydetDosyaABBorcHareket(Kullanici kullanan, int iPARDDonemi, int dosyaAnahtar, BorcHareket bh, ENUMDosyaIslemTur islemTur);
        ObjectArray ListeleDosyaABBorcHareket(Kullanici kullanan, int iPARDDonemi, int dosyaAnahtar, BorcHareket bh);
        Sonuc DosyalarinFaiziniHesapla(Kullanici kullanan, TNSDateTime hesapTarihi, DosyaAB form, int hesaplamaTur);
        //Sonuc TaksitlendirDosyaAB(Kullanici kullanan, DosyaAB alacakTakip, int taksitSay);
        //Sonuc TaksitKaldirDosyaAB(Kullanici kullanan, DosyaAB alacakTakip);
        Sonuc SilDosyaABBorcHareket(Kullanici kullanan, int iPARDDonemi, int dosyaAnahtar, int siraNo);

        /// <summary>
        /// OnBorc listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        ObjectArray ListeleOnBorcForm(Kullanici kullanan, OnBorc form);

        /// <summary>
        /// OnBorc nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        ObjectVAN OkuOnBorcForm(Kullanici kullanan, OnBorc form);

        /// <summary>
        /// OnBorc nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBorc yapısında OnBorc:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetOnBorcForm(Kullanici kullanan, OnBorc form, ENUMDosyaIslemTur islemTur);

        ObjectVAN OkuGAKForm(Kullanici kullanan, GeriAlimKarari form);
        Sonuc KaydetGAKForm(Kullanici kullanan, GeriAlimKarari form, ENUMDosyaIslemTur islemTur);

        ObjectVAN OkuDurumForm(Kullanici kullanan, DosyaABDurum form);
        Sonuc KaydetDurumForm(Kullanici kullanan, DosyaABDurum form, ENUMDosyaIslemTur islemTur);

        ObjectVAN OkuTaksitForm(Kullanici kullanan, DosyaABTaksit form);
        Sonuc KaydetTaksitForm(Kullanici kullanan, DosyaABTaksit form, ENUMDosyaIslemTur islemTur);
        Sonuc KaydetBorcluForm(Kullanici kullanan, ABBorclu form, ENUMDosyaIslemTur islemTur);
        ObjectVAN OkuBorcluForm(Kullanici kullanan, ABBorclu obForm);
    }
}
