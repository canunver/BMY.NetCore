using System;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.KYM;
using TNS.BKD;

namespace TNS.YAT
{
    /// <summary>
    /// Remoting yöntemi ile YATServis nesnesi oluşturmak için kullanılan sınıf
    /// Ayrıca, istemcide ve sunucuda ihtiyaç duyulan ortak kullanılacak statik fonksiyonlar da bu sınıfta yer alabilir.
    /// </summary>
    [Serializable()]
    public static class Arac
    {
        /// <summary>
        /// YATServis nesnesini, konfigürasyon dosyasından okuduğu remoting dizgisini kullanarak
        /// .net remoting ile oluşturup döndüren statik yordam
        /// </summary>
        /// <returns>.net remoting ile oluşturulan IYATServis arayüzünden türemiş nesne</returns>
        public static IYATServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("YATServisURL");
            IYATServis servis = TNS.Api.ServisIstemci.Olustur<IYATServis>(sUrl);
            return servis;
        }

        public static string IlIlceAdOl(TNSCollection ilIlceler)
        {
            string donDeger = "";
            if (ilIlceler == null || ilIlceler.Count == 0) return donDeger;
            for (int i = 0; i < ilIlceler.Count; i++)
            {
                ProjeIlIlce ilIce = (ProjeIlIlce)ilIlceler[i];
                string ad = ilIce.ilIlce.ilceAdi;

                if (String.Compare(ad, ilIce.ilAd, true) != 0)
                    ad = ilIce.ilAd + " " + ad;
                if (!string.IsNullOrWhiteSpace(ad))
                {
                    if (donDeger != "") donDeger += ", ";
                    donDeger += ad;
                }
            }
            return donDeger;
        }

        public static string EtudDevamEdenYeniAciklamaABC(string kod)
        {
            if (kod == "0") return "A-Etüd Proje";
            if (kod == "2") return "C-Yeni";
            return "B-Devam Eden";
        }

        public static string EtudDevamEdenYeniAciklama(string kod, bool toplamEkle)
        {
            string ek = "";
            if (toplamEkle)
            {
                if (kod == "0") ek = " İŞLERİ TOPLAMI";
                else ek = "LER TOPLAMI";
            }
            if (kod == "0") return "ETÜD PROJE" + ek;
            if (kod == "2") return "YENİ PROJE" + ek;
            return "DEVAM EDEN PROJE" + ek;


        }

        public static string BuYilBiterMiAciklama(int kod, int yil)
        {
            if (kod == 0) return string.Format("a) {0} Yılında Bitenler", yil);
            return string.Format("b) {0} Yılından Sonraya Kalanlar", yil);
        }

        public static string HTDegerAl(System.Collections.Hashtable ht, string alanAd, int pkod = -1, int apKod = -1)
        {
            string donusDeger = null;
            if (pkod > 0)
            {
                alanAd = alanAd + "_" + pkod + "_" + apKod;
            }
            try
            {
                donusDeger = (string)ht[alanAd];
            }
            catch
            {
            }
            if (donusDeger == null) donusDeger = "";
            return donusDeger;
        }

        public static string HTDegerAl(System.Collections.Hashtable[] ht, string alanAd)
        {
            string donusDeger = "";
            if (ht != null)
            {
                for (int i = 0; i < ht.Length; i++)
                {
                    System.Collections.Hashtable item = ht[i];
                    string tut = HTDegerAl(item, alanAd, -1, -1);
                    if (!string.IsNullOrWhiteSpace(tut))
                    {
                        if (donusDeger.IndexOf(tut) < 0)
                        {
                            if (i != 0) donusDeger += "\r\n";
                            donusDeger += tut;
                        }
                    }
                }
            }
            return donusDeger;
        }
    }

    /// <summary>
    /// Yatırım modülünün okuma, yazma gibi yetki kontrollerini yapan statik fonksiyonları içeren sınıf
    /// </summary>
    [Serializable()]
    public static class Yetki
    {
        /// <summary>
        /// Parametre olarak gönderilen kullanıcının istemci tarafındaki yatırım
        /// sayfalarını görmeye yetkili olup olmadığının kontrolünü yapan yordam
        /// Kullanıcının yatırım yetkisi varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">İstekte bulunan kullanıcı</param>
        /// <returns>Yetkili olup olmadığının sonucu</returns>
        public static bool SayfayaGirisYapabilirMi(Kullanici kullanan)
        {
            return GormeYetkisiVarMi(kullanan) || YoneticiMi(kullanan);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının yönetici olup olmadığının kontrolünü yapan yordam
        /// Kullanıcı yönetici ise true, değilse false döndürür.
        /// </summary>
        /// <param name="kullanan">İstekte bulunan kullanıcı</param>
        /// <returns>Yönetici olup olmadığının sonucu</returns>
        public static bool YoneticiMi(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının yatırım bilgilerini
        /// görme yetkisinin olup olmadığını kontrol eden yordam
        /// Kullanıcının yatırım bilgilerini görme yetkisi varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">İstekte bulunan kullanıcı</param>
        /// <returns>Görme yetkisi olup olmadığının sonucu</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan)
        {
            return
                kullanan.YetkiTipiDegeriIceriyorMu("Yatırım Hazırlığı Yetkisi", "1", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Yatırım Hazırlığı Yetkisi", "2", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Yatırım Hazırlığı Yetkisi", "3", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Yatırım Hazırlığı Yetkisi", "4", false);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının yatırım bilgilerini
        /// değiştirme yetkisinin olup olmadığını kontrol eden yordam
        /// Kullanıcının yatırım bilgilerini değiştirme yetkisi varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">İstekte bulunan kullanıcı</param>
        /// <returns>Yazma yetkisi olup olmadığının sonucu</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            return
                kullanan.YetkiTipiDegeriIceriyorMu("Yatırım Hazırlığı Yetkisi", "2", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Yatırım Hazırlığı Yetkisi", "4", false);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının yatırım bilgilerini
        /// onaylama yetkisinin olup olmadığını kontrol eden yordam
        /// Kullanıcının yatırım bilgilerini onaylama yetkisi varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">İstekte bulunan kullanıcı</param>
        /// <returns>Onaylama yetkisi olup olmadığının sonucu</returns>
        public static bool OnaylamaYetkisiVarMi(Kullanici kullanan)
        {
            return
                kullanan.YetkiTipiDegeriIceriyorMu("Yatırım Hazırlığı Yetkisi", "3", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Yatırım Hazırlığı Yetkisi", "4", false);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının
        /// yatırım hazırlığı yapabileceği birimleri döndüren yordam
        /// </summary>
        /// <param name="kullanan">Birimleri istenen kullanıcı</param>
        /// <returns>Yatırım hazırlığı yapabileceği birim listesi</returns>
        public static string[] BirimKodlariDondur(Kullanici kullanan, string kurKod)
        {
            if (!string.IsNullOrWhiteSpace(kurKod))
            {
                string[] kurkodlar = new string[1];
                kurkodlar[0] = kurKod;
                return kurkodlar;
            }
            else
            {

                string birimKodlari = (string)kullanan.yetkiTipiDegerleri["Yatırım Hazırlığı Yapacağı Birimler"];
                if (birimKodlari == null)
                    birimKodlari = "";

                return birimKodlari.Split(';');
            }
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının
        /// gönderilen birime yetkisi var mı yok mu kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Birim kontrolü yapılacak kullanıcı</param>
        /// <param name="birimKodu">Kullanıcının birimlerinde aranacak birim</param>
        /// <returns>Birime yetkili olup olmadığının sonucu</returns>
        private static bool BirimBirimiMi(Kullanici kullanan, string birimKodu, TNS.BHMv2.ENUMOGY ogy)
        {
            if (YoneticiMi(kullanan)) return true;
            //GormeYetkisiVarMi(kullanan) && 
            if (birimKodu == null) return true;
            if (birimKodu == "")
            {
                string[] birimler = BirimKodlariDondur(kullanan, null);
                for (int i = 0; i < birimler.Length; i++)
                    if (TNS.BKD.Arac.KurumMu(birimler[i]))
                        return true;

                return false;
            }

            string[] bakilacakKodlar;
            if (ogy == BHMv2.ENUMOGY.Gorme)
                bakilacakKodlar = new string[] { "1", "2", "3", "4" };
            else if (ogy == BHMv2.ENUMOGY.Yazma)
                bakilacakKodlar = new string[] { "2", "4" };
            else
                bakilacakKodlar = new string[] { "3", "4" };

            for (int i = kullanan.yetkiTipiDegerleri.MinIndex(); i <= kullanan.yetkiTipiDegerleri.MaxIndex(); i++)
            {
                string yetki = kullanan.yetkiTipiDegerleri["Yatırım Hazırlığı Yapacağı Birimler", i];
                if (Kullanici.YetkiTipiDegerUygunMu(yetki, birimKodu, "", true))
                {
                    yetki = kullanan.yetkiTipiDegerleri["Yatırım Hazırlığı Yetkisi", i];
                    for (int j = 0; j < bakilacakKodlar.Length; j++)
                    {
                        if (Kullanici.YetkiTipiDegerUygunMu(yetki, bakilacakKodlar[j], "", false))
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının, gönderilen birime yatırım projesi
        /// ve alt projesi tanımlama hakkı var mı yok mu kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Proje kaydedecek kullanıcı</param>
        /// <param name="birimKodu">Projenin kaydedileceği birim</param>
        /// <returns>Yetkili olup olmadığının sonucu</returns>

        public static Sonuc ProjeYazmaHakkiVarMi(Kullanici kullanan, string birimKodu)
        {
            return ProjeGormeYazmaHakkiVarMi(kullanan, birimKodu, BHMv2.ENUMOGY.Yazma);
        }

        public static Sonuc ProjeGormeYazmaHakkiVarMi(Kullanici kullanan, string birimKodu, BHMv2.ENUMOGY ogy)
        {
            Sonuc donusDeger = new Sonuc();
            if (birimKodu == "")
                donusDeger.DegerAta(YatirimResI.YAT002);

            if (YoneticiMi(kullanan) || (BirimBirimiMi(kullanan, birimKodu, ogy)))
            {
                donusDeger.islemSonuc = true;
                donusDeger.hataStr = "";
            }
            else
                donusDeger.DegerAta(YatirimResI.YAT001);

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının yatırım sözlüklerini
        /// tanımlamaya hakkı var mı yok mu kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Sözlük kaydedecek kullanıcı</param>
        /// <returns>Yetkili olup olmadığının sonucu</returns>
        public static Sonuc SozlukYazmaHakkiVarMi(Kullanici kullanan)
        {
            Sonuc donusDeger = new Sonuc(false, YatirimResI.YAT002, "");

            if (YoneticiMi(kullanan))
            {
                donusDeger.islemSonuc = true;
                donusDeger.hataStr = "";
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının yatırım bilgilerini
        /// değiştirme hakkının olup olmadığını kontrol eden yordam
        /// Kullanıcının yatırım bilgilerini değiştirme hakkı varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <returns>Yazma hakkı olup olmadığının sonucu</returns>
        public static Sonuc YazmaHakkiVarMi(Kullanici kullanan)
        {
            Sonuc donusDeger = new Sonuc(false, YatirimResI.YAT002, "");

            if (YoneticiMi(kullanan) || YazmaYetkisiVarMi(kullanan))
            {
                donusDeger.islemSonuc = true;
                donusDeger.hataStr = "";
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının yatırım bilgilerini
        /// okuma hakkının olup olmadığını kontrol eden yordam
        /// Kullanıcının yatırım bilgilerini okuma hakkı varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">İstekte bulunan kullanıcı</param>
        /// <returns>Okuma hakkı olup olmadığının sonucu</returns>
        public static Sonuc OkumaHakkiVarMi(Kullanici kullanan, string birim = "")
        {
            Sonuc donusDeger = new Sonuc();

            if (!BirimBirimiMi(kullanan, birim, BHMv2.ENUMOGY.Gorme))
            {
                donusDeger.DegerAta(YatirimResI.YAT004);
            }

            return donusDeger;
        }
    }

    /// <summary>
    /// Yatırım modülü sunucu sınıfı tarafından sunulan fonksiyonları belirten arayüz sınıfı
    /// </summary>
    public interface IYATServis
    {
        /// <summary>
        /// Proje tür varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="projeTur">Proje tür sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ProjeTurKaydet(Kullanici kullanan, Sozluk projeTur);

        /// <summary>
        /// Verilen kriterlere uygun olan proje tür bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="projeTur">Proje tür kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında proje türleri listesi döndürülür.</returns>
        BilgiVAN ProjeTurListele(Kullanici kullanan, Sozluk projeTur);

        /// <summary>
        /// Proje tür, yatırım projelerinde ve/veya alt projelerinde kullanılmamışsa silen,
        /// kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="projeTur">Proje tür sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ProjeTurSil(Kullanici kullanan, Sozluk projeTur);

        /// <summary>
        /// Doküman tür varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dokumanTur">Doküman tür sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc DokumanTurKaydet(Kullanici kullanan, Sozluk dokumanTur);

        /// <summary>
        /// Verilen kriterlere uygun olan doküman tür bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dokumanTur">Doküman tür kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında doküman türleri listesi döndürülür.</returns>
        BilgiVAN DokumanTurListele(Kullanici kullanan, Sozluk dokumanTur);

        /// <summary>
        /// Doküman tür, alt projelere doküman eklenirken kullanılmamışsa silen,
        /// kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dokumanTur">Doküman tür sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc DokumanTurSil(Kullanici kullanan, Sozluk dokumanTur);

        /// <summary>
        /// İl varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="il">İl sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc IlKaydet(Kullanici kullanan, Sozluk il);

        /// <summary>
        /// Verilen kriterlere uygun olan il bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="il">İl kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında il listesi döndürülür.</returns>
        BilgiVAN IlListele(Kullanici kullanan, Sozluk il);

        /// <summary>
        /// İl, ilçe tanımlamada kullanılmamışsa silen,
        /// kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="il">İl sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc IlSil(Kullanici kullanan, Sozluk il);

        /// <summary>
        /// İlçe varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ilçe sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc IlceKaydet(Kullanici kullanan, IlceSozluk ilce);

        /// <summary>
        /// Verilen kriterlere uygun olan ilçe bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ilçe sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ilçe listesi döndürülür.</returns>
        BilgiVAN IlceListele(Kullanici kullanan, IlceSozluk ilce);

        /// <summary>
        /// İlçe, yatırım projelerinde ve/veya alt projelerinde kullanılmamışsa silen,
        /// kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe sözlük bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc IlceSil(Kullanici kullanan, IlceSozluk ilce);

        ///// <summary>
        ///// Sektör varsa güncelleyen, yoksa kaydeden yordam
        ///// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="sektor">Sektör sözlük bilgisini tutan nesne</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //Sonuc SektorKaydet(Kullanici kullanan, SektorSozluk sektor);

        ///// <summary>
        ///// Verilen kriterlere uygun olan sektör bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="sektor">Sektör kriteri</param>
        ///// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sektör listesi döndürülür.</returns>
        //BilgiVAN SektorListele(Kullanici kullanan, int yil, SektorSozluk sektor);

        ///// <summary>
        ///// Sektör, yatırım projelerinde kullanılmamışsa silen,
        ///// kullanılmışsa uyarı döndüren yordam
        ///// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="sektor">Sektör sözlük bilgisini tutan nesne</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //Sonuc SektorSil(Kullanici kullanan, SektorSozluk sektor);

        /// <summary>
        /// Verilen kriterlere uygun olan ilçe bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ilçe listesi döndürülür.</returns>
        BilgiVAN IlIlceOneriListele(Kullanici kullanan, IlceSozluk ilce);

        /// <summary>
        /// Yatırım projesi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen proje nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="proje">Yatırım projesi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ProjeKaydet(Kullanici kullanan, int yil, string senaryoKod, Proje proje, bool yeni);

        /// <summary>
        /// Yatırım projesi, yatırım alt projelerinde kullanılmamışsa silen,
        /// kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// proje nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="projeler">Proje nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ProjeSil(Kullanici kullanan, int yil, string senaryoKod, TNSCollection projeler);

        BilgiVAN ProjeListele(Kullanici kullanan, int yil, string senaryoKod, Proje proje, string kurKod, bool altProjelerle);
        BilgiVAN ListeleProjeTanim(Kullanici kullanan, int yil, string senaryoKod, string kurKod, string kriter);
        BilgiVAN ListeleAltProjeTanim(Kullanici kullanan, int yil, string senaryoKod, string kurKod, int projeKod, string kriter);

        /// <summary>
        /// Yatırım alt projesi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen alt proje nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProje">Yatırım alt projesi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc AltProjeKaydet(Kullanici kullanan, int yil, string senaryoKod, AltProje altProje, bool yeni);

        /// <summary>
        /// Yatırım alt projesi; bütçe kaynaklarda, dokümanlarda ve kredi taleplerde kullanılmamışsa silen,
        /// kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// alt proje nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProjeler">Alt proje nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc AltProjeSil(Kullanici kullanan, int yil, string senaryoKod, TNSCollection altProjeler);

        /// <summary>
        /// Verilen kriterlere uygun olan yatırım alt projelerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProje">Yatırım alt projesi kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım alt projeleri döndürülür.</returns>
        BilgiVAN AltProjeListele(Kullanici kullanan, int yil, string senaryoKod, AltProje altProje, string sektorKod, bool detayOku, bool detayRakamOku);

        /// <summary>
        /// Bütçe kaynak varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen bütçe kaynak nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynak">Bütçe kaynak bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceKaynakKaydet(Kullanici kullanan, int yil, string senaryoKod, ButceKaynak kaynak, bool yeni);

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
        Sonuc ButceKaynakDetayKaydet(Kullanici kullanan, ButceKaynak kaynak, int yil, string senaryoKod);

        /// <summary>
        /// Bütçe kaynak, herhangi bir detay bilgiye sahip değilse silen,
        /// detay bilgileri varsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// bütçe kaynak nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynakNo">silinecek kaynağın numarası</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceKaynakSil(Kullanici kullanan, int yil, string senaryoKod, int altProjeNo, int kaynakNo);

        /// <summary>
        /// Bütçe kaynağın detay bilgilerini, ödenek teklif  bilgilerini,
        /// harcama  bilgilerini ve geri ödeme bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// bütçe kaynak nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynakNo">Silinecek kaynağın kodu</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceKaynakDetaySil(Kullanici kullanan, int yil, string senaryoKod, int altProjeKodu, int kaynakNo);

        /// <summary>
        /// Verilen kriterlere uygun olan bütçe kaynak bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynakNo">Listelenmek istenen bütçe kaynağın programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="detayVer">Bütçe kaynağın detay bilgileri listelensin mi listelenmesin mi</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında bütçe kaynaklar döndürülür.</returns>
        BilgiVAN ButceKaynakBilgiOku(Kullanici kullanan, int kaynakNo, int yil, string senaryoKod, string sektorKod, bool detayVer, bool tumYillar, bool tekYil, string projeAd);

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
        BilgiVAN ButceKaynakListele(Kullanici kullanan, int projeKodu, int altProjeKodu, int kaynakNo, int yil, string senaryoKod, ENUMButceKaynakTur[] tur, bool detayVer);
        ObjectArray KrediliKaynaklariListele(Kullanici kullanan, int yil, string senaryoKod, int projeKodu, int altProjeKodu, ENUMButceKaynakTur[] kaynakTurler);

        /// <summary>
        /// Alt faaliyet-yatırım bağı üzerinden bütçe hazırlık ödenek tekliflerini bulup,
        /// bütçe kaynak bazında ödenek toplamlarını hesaplayan ve kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceKaynakOdenekAktar(Kullanici kullanan);

        /// <summary>
        /// Alt faaliyet-yatırım bağı üzerinden bütçe hazırlık ödenek tekliflerini bulup,
        /// bütçe kaynak bazında ödenek toplamlarını hesaplayan ve kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceKaynakOdenekAktar(Kullanici kullanan, int yil);

        /// <summary>
        /// Alt faaliyet-yatırım bağı üzerinden bütçe hazırlık ödenek tekliflerini bulup,
        /// bütçe kaynak bazında ödenek toplamlarını hesaplayan ve kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceKaynakOdenekAktar(Kullanici kullanan, int yil, string senaryoKod);

        /// <summary>
        /// Kredi talep varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen kredi talep nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="krediTalep">Kredi talep bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc KrediTalepKaydet(Kullanici kullanan, int yil, string senaryoKod, KrediTalep krediTalep);

        /// <summary>
        /// Verilen kriterlere uygun olan kredi talepleri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProjeKodu">Yatırım alt projesinin programatik kodu</param>
        /// <param name="krediNo">Kredi talebin programatik kodu</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kredi talepler döndürülür.</returns>
        BilgiVAN KrediTalepListele(Kullanici kullanan, int altProjeKodu, int krediNo);

        /// <summary>
        /// Verilen kriterlere uygun olan kredi talepleri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="altProjeKodu">Yatırım alt projesinin programatik kodu</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kredi talepler döndürülür.</returns>
        BilgiVAN KrediTalepListele(int altProjeKodu, Kullanici kullanan, int yil);

        /// <summary>
        /// Doküman varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen doküman nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dokuman">Doküman bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc DokumanKaydet(Kullanici kullanan, int yil, string senaryoKod, Dokuman dokuman);

        /// <summary>
        /// Verilen kriterlere uygun olan dokümanları döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="altProjeKodu">Yatırım alt projesinin programatik kodu</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında dokümanlar döndürülür.</returns>
        BilgiVAN DokumanListele(Kullanici kullanan, int altProjeKodu);

        /// <summary>
        /// Doküman programatik kodu verilen dokümanı silen yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="siraNo">Doküman programatik kodu</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc DokumanSil(Kullanici kullanan, string siraNo);

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
        /// <param name="projeNo">Listelenecek projenin program kodu, -1 ise hepsi</param>
        /// <param name="rakamlarsiz">Listede rakamlar bulunacak mı?</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        BilgiVAN ProjeAltProjeListele(Kullanici kullanan, int yil, string senaryoKod, string sektorNo, string etudMu, int projeNo, int projeTur, ENUMButceKaynakTur[] bkTur, bool rakamlarsiz, int ilkYil, bool kullanilmiyor, string kurKod, int altProjeNo, bool eskiYillarKBO, string araString, bool yetkiYok);

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
        BilgiVAN Tablo2OzelListele(Kullanici kullanan, int yil, string senaryoKod, string sektorNo, int projeNo, string kurKod);

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
        //BilgiVAN Tablo4Listele(Kullanici kullanan, int yil, string senaryoKod, string sektorNo, int projeTuru);

        ///// <summary>
        ///// DPT Tablo-5 raporunu üreten yordam
        ///// Rapor; istenen projenin yıllar itibariyle proje tutar, ödenek teklif ve harcama bilgilerini içerir.
        ///// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        ///// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında bir tane proje nesnesi döndürülür.</returns>
        //BilgiVAN Tablo5Listele(Kullanici kullanan, int projeKodu);

        /// <summary>
        /// DPT Tablo-6 raporunu üreten yordam
        /// Rapor; istenen kredinin kimlik bilgilerini, projesinin kimlik bilgilerini ve yıllar itibariyle rakamsal bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynak">Kredinin programatik kodu</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında proje nesneleri ve detayında da kredi bilgileri döndürülür.</returns>
        BilgiVAN Tablo6Listele(Kullanici kullanan, int kaynak);

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
        BilgiVAN Tablo7Listele(Kullanici kullanan, int yil, string senaryoKod, int krediTalepNo);

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
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        BilgiVAN Tablo10Listele(Kullanici kullanan, int yil, string senaryoKod, string sektorNo, string kurKod);

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
        BilgiVAN Tablo1213Listele(Kullanici kullanan, int yil, string sektorNo, int model, string senaryoKod, string kurKod);

        /// <summary>
        /// Yatırım teklifleri dağılım raporunu üreten yordam
        /// Rapor; birim, yatırım projesi ve tertip bazında yatırım projeleri ödenek teklif ve harcama bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="sektorKod">Sektor kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        BilgiVAN TeklifDagilimListele(Kullanici kullanan, int yil, string senaryoKod, string sektorKod, string kurKod);

        /// <summary>
        /// Birimlere ve sektörlere göre yatırım teklifleri dağılım raporunu üreten yordam
        /// Rapor; birim ve sektör bazında yatırım projeleri ödenek teklif ve harcama bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında teklif dağılım listesi döndürülür.</returns>
        BilgiVAN SektorDagilimListele(Kullanici kullanan, int yil, string senaryoKod, string kurKod);

        /// <summary>
        /// Birimlerin yatırım projeleri ve ödenek teklifleri raporunu üreten yordam
        /// Rapor; istenen birime ait yatırım projelerini ve ödenek tekliflerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="birimKodu">Birim kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        BilgiVAN BirimProjeleriListele(Kullanici kullanan, int yil, string senaryoKod, string birimKodu, bool altProjelerDahil);

        /// <summary>
        /// Konsolide yatırım bilgileri raporunu üreten yordam
        /// Rapor; yatırım projelerini, alt projelerini, proje maliyetini, ödenek tekliflerini ve harcamalarını içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        BilgiVAN KonsolideYatirimListele(Kullanici kullanan, int yil, string kurKod);

        /// <summary>
        /// Proje takip raporunu üreten yordam
        /// Rapor; yatırım projesinin yıllar itibariyle maliyet, ödenek teklif ve harcama bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        BilgiVAN ProjeTakipTablosu(Kullanici kullanan, int yil, string kurKod);

        /// <summary>
        /// Projeler arası ödenek aktarması raporunu üreten yordam
        /// Rapor; yatırım projelerini, alt projelerini, proje maliyetini, ödenek tekliflerini ve harcamalarını içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="sektorNo">Sektör kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        BilgiVAN ProjelerArasiOdenekAktarmasiTablosu(Kullanici kullanan, int yil, string sektorNo, string kurKod);

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
        BilgiVAN TertipDagilimListele(Kullanici kullanan, int yil, string senaryoKod, string sektor, string kurKod);

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
        BilgiVAN DPTVizeCetveliRaporla(Kullanici kullanan, int yil, string senaryoKod, Tertip tertip);

        /// <summary>
        /// Verilen yıla uygun olan Hizmet alımı suretiyle kullanılan/kullanılacak
        /// taşıtlara ilişkin ödenek teklif bilgilerini döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında TasitAlimBedeliBilgisi nesneleri döndürülür.</returns>
        ObjectArray TasitAlimBedeliListele(int yil);

        /// <summary>
        /// Hizmet alımı suretiyle kullanılan/kullanılacak
        /// taşıtlara ilişkin ödenek teklif bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl bilgisi</param>
        /// <param name="tasitAlimBedelleri">TasitAlimBedeliBilgisi nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc TasitAlimBedeliKaydet(Kullanici kullanan, int yil, TNSCollection tasitAlimBedelleri);

        /// <summary>
        /// Verilen kriterlere uygun olan taşıt talep bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurumKod">Kurum kriteri</param>
        /// <param name="sektorKod">Sektör kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında TasitTalepBilgi nesneleri döndürülür.</returns>
        ObjectArray TasitListesiBul(Kullanici kullanan, int yil, string kurumKod, string sektorKod);

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
        Sonuc TasitListesiKaydet(Kullanici kullanan, int yil, string kurumKod, string sektorKod, TNSCollection tasitListesi);

        BilgiVAN VizeCetveliRaporla(Kullanici kullanan, int yil, string senaryoKod, string sektorNo, string birimKod);

        Sonuc AsamaKopyala(Kullanici kullanan, int kaynakYil, string kaynakAsama, int hedefYil, string hedefAsama, bool[] kopyalanacaklar, TNSCollection projeKodlar);

        ObjectArray ProjeHarcamalari(Kullanici kullanan, int yil, string senaryoKod, Tertip kistasTertip, string kurKod, int avansDurum, bool adlariBul);

        ObjectArray ProjeSunumBilgileri(Kullanici kullanan, int yil, string senaryoKod, string senaryoKodOnc, Tertip kistasTertip, int Hepsi1Eski2Yeni3);

        ObjectArray ProjeIslemleri(Kullanici kullanan, TNS.BHMv2.MPRevizeFormSorgu form);

        Sonuc AltProjeGerekceKaydet(Kullanici kullanan, int yil, string senaryoKod, AltProje[] gerekceler);

        ObjectArray AltProjeListeleYeni(Kullanici kullanan, AltProjeTeklif form);

        Sonuc AltProjeKaydetYeni(Kullanici kullanan, int yil, string kurKod, string sektorKod, AltProjeTeklif[] projeGerekceler);

        ObjectArray MaliYapiBul(Kullanici kullanan, int raporYil, string raporSenaryoKod, int butceYil, string senaryoKod, ENUMProjeMaliYapiKod eNUMProjeMaliYapiTur, int projeNo, int altProjeNo, bool oncekiYillarDahil, bool detayAltProje);

        ObjectArray OdenekListesiBul(Kullanici kullanan, int butceYil, string senaryoKod, int teklifYil, int projeNo, int altProjeNo);

        TNS.BHMv2.UcgenSatir[] OdenekListesiBulTam(Kullanici kullanan, int butceYil, string senaryoKod, int projeNo, int altProjeNo);
        ObjectArray OdenekListesiBulEko2(Kullanici kullanan, int butceYil, string senaryoKod, int projeNo, int altProjeNo);

        System.Collections.Hashtable AltProjeBilgiListele(Kullanici kullanan, int yil, string senaryoKod, int projeKodu, int altProjeKodu, string[] alanlar);
        System.Collections.Hashtable AltProjeBilgiListeleTum(Kullanici kullanan, int yil, string senaryoKod, string[] alanlar);

        Sonuc AltProjeBilgiKaydet(Kullanici kullanan, int yil, string senaryoKod, int projeKodu, int altProjeKodu, System.Collections.Hashtable ht);

        ObjectArray ProjeListeleFaal(Kullanici kullanan, int yil, string senaryoKod, string altFaalKod);

        Sonuc YATKaynakKaydetTopluVeri(Kullanici kullanan, int yil, string senaryoKod, ButceKaynak[] bklar, ENUMProjeMaliYapiKod[] teklifYillar);

        AltFaalBilgi AltFaaliyetBilgiBul(Kullanici kullanan, int yil, string pebKod, string kurKod, string senaryoKod, bool illleriBul);

        ObjectArray TanimSozlukListele(Kullanici kullanan, int sozlukTur, int yil, string senaryoKod);

        Sonuc TanimSozlukEkle(Kullanici kullanan, int sozlukTur, Sozluk sozluk);

        ObjectArray ListeleYATYetkiGiris(Kullanici kullanan, int projeKod, int altProjeKod);

        Sonuc KaydetYATYetkiGiris(Kullanici kullanan, int projeNo, int altProjeNo, TNSCollection kullanicilar, int yil, string senaryoKod);

        decimal DeflatorBul(Kullanici kullanan, int butceYil, string senaryoKod, string sektorKod);

        ObjectArray ProjeDosyalariListele(Kullanici kullanan, ProjeEkliDosya form, int yil, string senaryoKod);

        Sonuc ProjeDosyasiSakla(Kullanici kullanan, ProjeEkliDosya form, int yil, string senaryoKod);

        Sonuc ProjeDosyasiSil(Kullanici kullanan, ProjeEkliDosya form, int yil, string senaryoKod);

        Sonuc AltProjeTasi(Kullanici kullanan, int yil, string senaryoKod, int apKod, int pKod);

        Sonuc KaynakTasi(Kullanici kullanan, int yil, string senaryoKod, int kaynakNo, int apKod);

        /// <summary>
        /// DeflatorAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DeflatorAna yapısında DeflatorAna: Deflatör bilgilerini tutan değişken</param>
        ObjectArray ListeleDeflator(Kullanici kullanan, int yil, string senaryoKod, string sektorKod);

        /// <summary>
        /// DeflatorAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DeflatorAna yapısında DeflatorAna: Deflatör bilgilerini tutan değişken</param>
        ObjectVAN OkuDeflator(Kullanici kullanan, int yil, string senaryoKod, string sektorKod);

        /// <summary>
        /// DeflatorAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DeflatorAna yapısında DeflatorAna: Deflatör bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetDeflator(Kullanici kullanan, int yil, string senaryoKod, TNSCollection deflatorler);
        Sonuc KarekteristikKaydet(Kullanici kullanan, KarakteristikGiris[] form);
        KarakteristikGiris[] KarekteristikListele(Kullanici kullanan);

        /// <summary>
        /// YollukPlanAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YollukPlanAna yapısında YollukPlanAna: Yıl ve Faaliyet Seçimi bilgilerini tutan değişken</param>
        ObjectArray ListeleYollukPlanlama(Kullanici kullanan, int yil, string senaryoKod, Tertip tertip, string pebKodListe);

        /// <summary>
        /// YollukPlanAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YollukPlanAna yapısında YollukPlanAna: Yıl ve Faaliyet Seçimi bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetYollukPlanlama(Kullanici kullanan, int yil, string senaryoKod, Tertip tertip, TNSCollection formlar);

        /// <summary>
        /// AltProje listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AltProje yapısında AltProje: Alt Proje bilgilerini tutan değişken</param>
        ObjectArray ListeleAltProjeDetayProgrami(Kullanici kullanan, AltProjeDetayProgrami form);

        /// <summary>
        /// AltProje nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AltProje yapısında AltProje: Alt Proje bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetAltProjeDetayProgrami(Kullanici kullanan, TNSCollection form);

        /// <summary>
        /// KalkinmaBolge listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KalkinmaBolge yapısında KalkinmaBolge: Kalkinma Bölgelesi bilgilerini tutan değişken</param>
        ObjectArray ListeleKalkinmaBolgeleriTanim(Kullanici kullanan, KalkinmaBolge form);

        /// <summary>
        /// KalkinmaBolge nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KalkinmaBolge yapısında KalkinmaBolge: Kalkinma Bölgelesi bilgilerini tutan değişken</param>
        ObjectVAN OkuKalkinmaBolgeleriTamim(Kullanici kullanan, KalkinmaBolge form);

        /// <summary>
        /// KalkinmaBolge nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KalkinmaBolge yapısında KalkinmaBolge: Kalkinma Bölgelesi bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetKalkinmaBolgeleriTamim(Kullanici kullanan, KalkinmaBolge form);

        Sonuc ProjeAltProjeYazmaGormeHakkiVarMi(Kullanici kullanan, int projeKodu, int altProjeKodu, int yil, string senaryoKod, TNS.BHMv2.ENUMOGY ogy, bool hazirlik);

        ObjectArray ProjeListele(Kullanici kullanan, int yil, string senaryoKod, Proje proje, bool tekYil, bool eskiYillarKBO, int ekoUz);
    }
}