using System;
using TNS.DEG;
using TNS.KYM;

namespace TNS.TMM
{
    /// <summary>
    /// Remoting yöntemi ile TMMServis nesnesi oluşturmak için kullanılan sınıf
    /// Ayrıca, istemcide ve sunucuda ihtiyaç duyulan ortak
    /// kullanılacak statik fonksiyonlar da bu sınıfta yer alabilir.
    /// </summary>
    [Serializable()]
    public static class Arac
    {
        /// <summary>
        /// TMMServis nesnesini, konfigürasyon dosyasından okuduğu remoting
        /// dizgisini kullanarak .net remoting ile oluşturup döndüren statik yordam
        /// </summary>
        /// <returns>.net remoting ile oluşturulan ITMMServis arayüzünden türemiş nesne</returns>
        public static ITMMServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("TMMServisURL");
            ITMMServis servisTMM = TNS.Api.ServisIstemci.Olustur<ITMMServis>(sUrl);
            return servisTMM;
        }

        public static string DegiskenDegerBul(int yil, string cacheID)
        {
            return TNS.DEG.Arac.DegiskenDegerBulCache(yil, cacheID);
        }

        public static bool DemirbasCesidiMi(string hesapKod)
        {
            if (hesapKod.Length < 3) return false;

            string hesapKodIlkUc = hesapKod.Substring(0, 3);
            if (hesapKodIlkUc == ((int)ENUMTasinirHesapKodu.DEMIRBAS).ToString() || hesapKodIlkUc == ((int)ENUMTasinirHesapKodu.MAKINECIHAZ).ToString() || hesapKodIlkUc == ((int)ENUMTasinirHesapKodu.TASIT).ToString())
                return true;
            else if (MerkezBankasiKullaniyor())
                return true;

            return false;
        }

        public static bool DemirbasMi(string hesapKod)
        {
            if (hesapKod.Length < 3) return false;

            string hesapKodIlkUc = hesapKod.Substring(0, 3);
            if (hesapKodIlkUc == ((int)ENUMTasinirHesapKodu.DEMIRBAS).ToString())
                return true;
            else if (MerkezBankasiKullaniyor())
                return true;

            return false;
        }

        public static bool TasitMi(string hesapKod)
        {
            if (hesapKod.Length < 3) return false;

            string hesapKodIlkUc = hesapKod.Substring(0, 3);
            if (hesapKodIlkUc == ((int)ENUMTasinirHesapKodu.TASIT).ToString())
                return true;

            return false;
        }

        public static bool MakineCihazMi(string hesapKod)
        {
            if (hesapKod.Length < 3) return false;

            string hesapKodIlkUc = hesapKod.Substring(0, 3);
            if (hesapKodIlkUc == ((int)ENUMTasinirHesapKodu.MAKINECIHAZ).ToString())
                return true;

            return false;
        }

        public static bool MerkezBankasiKullaniyor()
        {
            return OrtakFonksiyonlar.WebConfigOku("VTTip", "") == "TCMB";
        }

        public static bool MeclisKullaniyor()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.TMM.Arac.DegiskenDegerBul(0, "/TBMM"), 0) > 0 ? true : false;
        }

        public static bool CumhurbaskanligiKullaniyor()
        {
            //return TNS.TMM.Arac.DegiskenDegerBul(0, "/KURUMKOD").Replace(".", "") == "01";
            return TNS.TMM.Arac.DegiskenDegerBul(0, "/KURUMAD").IndexOf("CUMHURBAŞKAN") > -1;
        }

        public static bool CumhurbaskanligiSosyalSgbNetKullaniyor()
        {
            return TNS.TMM.Arac.DegiskenDegerBul(0, "/KURUMKOD").Replace(".", "") == "01";
        }

        public static bool CumhurbaskanligiSgbNetKullaniyor()
        {
            return TNS.TMM.Arac.DegiskenDegerBul(0, "/KURUMKOD").Replace(".", "") == "02";
        }

        public static string EPCOlustur(int prSicilNo, string kurumKod)
        {
            kurumKod = kurumKod.Replace(".", "");
            kurumKod = kurumKod.PadRight(8, '0');

            int padStrSay = 24 - kurumKod.Length;
            string epc = kurumKod + prSicilNo.ToString().PadLeft(padStrSay, '0');

            return epc;
        }

        public static int EPCCoz(Kullanici kullanan, string kurumKod, string epc)
        {
            int prSicilNo = 0;
            kurumKod = kurumKod.Replace(".", "");


            if (epc.Length == 24 && epc.StartsWith(kurumKod))
                prSicilNo = OrtakFonksiyonlar.ConvertToInt(epc.Substring(8), 0);
            else
            {
                ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
                ObjectArray bilgi = servisTMM.SicilRFIDListele(kullanan, epc, 0);

                foreach (SicilNoHareket sh in bilgi.objeler)
                {
                    prSicilNo = sh.prSicilNo;
                    break;
                }
            }

            return prSicilNo;
        }

    }

    //[Serializable()]
    //public static class Lisans
    //{
    //    public static Sonuc LisansKontrol()
    //    {
    //        //ITMMServis servisTMM = TMM.Arac.Tanimla();
    //        //return servisTMM.LisansKontrol();

    //        Sonuc sonuc = new Sonuc(true, "", "");

    //        return sonuc;

    //    }
    //}

    /// <summary>
    /// Taşınır mal modülünün okuma, yazma gibi yetki kontrollerini yapan statik fonksiyonları içeren sınıf
    /// </summary>
    [Serializable()]
    public static class Yetki
    {
        /// <summary>
        /// Parametre olarak gönderilen kullanıcının taşınır mal bilgileri için
        /// ogy parametresindeki yetkiye sahip olup olmadığını kontrol eden yordam
        /// Kullanıcı ilgili yetkiye sahipse true, değilse false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <param name="ogy">ENUMOGY listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>İlgili yetkinin olup olmadığının sonucu</returns>
        public static bool YetkisiVarMi(Kullanici kullanan, int ogy)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRMALYONETICISI))
                return true;

            string yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Taşınır Yetki Şablonu");
            string[] ytler = yetkiTipiYetkiDegeri.Split(';');

            //Görev olarak yetkilendirme yapıldığında birden fazla yetki dönüyor. Yetkinin ilki alınıyor. Melih 
            //if (ytler.Length > 0)
            //    yetkiTipiYetkiDegeri = ytler[0];
            //else
            //    yetkiTipiYetkiDegeri = "";

            for (int i = 0; i < ytler.Length; i++)
            {
                yetkiTipiYetkiDegeri = ytler[i];

                int yetkiTipiYetkiDegeriInt = OrtakFonksiyonlar.ConvertToInt(yetkiTipiYetkiDegeri, -1);
                if (ogy == (int)ENUMOGY.Gorme && yetkiTipiYetkiDegeriInt > 0)
                    return true;
                if (ogy == (int)ENUMOGY.Yazma && (yetkiTipiYetkiDegeriInt == 2 || yetkiTipiYetkiDegeriInt == 4))
                    return true;
                if (ogy == (int)ENUMOGY.Onaylama && yetkiTipiYetkiDegeriInt > 2)
                    return true;
                if (ogy == (int)ENUMOGY.OnayKaldir && yetkiTipiYetkiDegeriInt == 5)
                    return true;
            }


            return false;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının istemci tarafındaki taşınır mal
        /// sayfalarını görmeye yetkili olup olmadığının kontrolünü yapan yordam
        /// Kullanıcının taşınır mal yetkisi varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <returns>Yetkili olup olmadığının sonucu</returns>
        public static bool SayfayaGirisYapabilirMi(Kullanici kullanan)
        {
            return YetkisiVarMi(kullanan, 2);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının istemci tarafındaki taşınır mal
        /// sayfalarını görmeye yetkili olup olmadığının kontrolünü yapan yordam
        /// Kullanıcının taşınır mal yetkisi varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <returns>
        /// Yetkili olup olmadığının sonucu
        /// </returns>
        public static bool SayfayaGirisYapabilirMiBirim(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                return true;

            string yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Taşınır Kullanıcı Birimi Yetkisi");

            return !string.IsNullOrEmpty(yetkiTipiYetkiDegeri);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının taşınır mal işlemleri
        /// yapabileceği muhasebe-harcama-ambar birimlerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">Birimleri istenen kullanıcı</param>
        /// <returns>Taşınır mal işlemleri yapabileceği birim listesi</returns>
        public static string[] YetkiTipiDegerleriDondur(Kullanici kullanan)
        {
            string yetkiDeger = kullanan.yetkiTipiDegerleri["Taşınır Muhasebe Harcama Birim Yetkisi"];
            if (yetkiDeger == null)
            {
                if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI) && !string.IsNullOrEmpty(kullanan.calistigiKurum))
                    yetkiDeger = kullanan.calistigiKurum + "-*-*";
                else
                    yetkiDeger = "";
            }

            return yetkiDeger.Split(';');
        }
    }

    /// <summary>
    /// Taşınır mal modülü sunucu sınıfı tarafından sunulan fonksiyonları belirten arayüz sınıfı
    /// </summary>
    public interface ITMMServis
    {
        /// <summary>
        /// İşlem tipi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen IslemTip nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="islemTip">İşlem tipi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc IslemTipKaydet(Kullanici kullanan, IslemTip islemTip);

        /// <summary>
        /// İşlem tipi, taşınır işlem fişlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen IslemTip nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="islemTip">İşlem tipi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc IslemTipSil(Kullanici kullanan, IslemTip islemTip);

        /// <summary>
        /// Verilen kriterlere uygun olan işlem tipi bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="islemTip">İşlem tipi bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında işlem tipi listesi döndürülür.</returns>
        ObjectArray IslemTipListele(Kullanici kullanan, IslemTip islemTip);

        /// <summary>
        /// Ölçü birim varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen OlcuBirim nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="olcuBirim">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc OlcuBirimKaydet(Kullanici kullanan, OlcuBirim olcuBirim);

        /// <summary>
        /// Ölçü birim, taşınır hesap planında kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen OlcuBirim nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="olcuBirim">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc OlcuBirimSil(Kullanici kullanan, OlcuBirim olcuBirim);

        /// <summary>
        /// Verilen kriterlere uygun olan ölçü birim bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="olcuBirim">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ölçü birim listesi döndürülür.</returns>
        ObjectArray OlcuBirimListele(Kullanici kullanan, OlcuBirim olcuBirim);


        Sonuc MuhasebeIslemTipiKaydet(Kullanici kullanan, MuhasebeIslemTipi muhasebeIslemTip);
        Sonuc MuhasebeIslemTipiSil(Kullanici kullanan, MuhasebeIslemTipi muhasebeIslemTip);
        ObjectArray MuhasebeIslemTipiListele(Kullanici kullanan, MuhasebeIslemTipi muhasebeIslemTip);

        /// <summary>
        /// İl varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Il nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="il">İl bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc IlKaydet(Kullanici kullanan, Il il);

        /// <summary>
        /// Verilen kriterlere uygun olan il bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="il">İl bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında il listesi döndürülür.</returns>
        ObjectArray IlListele(Kullanici kullanan, Il il);

        /// <summary>
        /// İl, ilçelerde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Il nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="il">İl bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc IlSil(Kullanici kullanan, Il il);

        /// <summary>
        /// İlçe varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Ilce nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc IlceKaydet(Kullanici kullanan, Ilce ilce);

        /// <summary>
        /// Verilen kriterlere uygun olan ilçe bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ilçe listesi döndürülür.</returns>
        ObjectArray IlceListele(Kullanici kullanan, Ilce ilce);

        /// <summary>
        /// İlçe, muhasebe birimlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Ilce nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc IlceSil(Kullanici kullanan, Ilce ilce);

        /// <summary>
        /// Bölge varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Bolge nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="bolge">Bölge bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc BolgeKaydet(Kullanici kullanan, Bolge bolge);

        /// <summary>
        /// Verilen kriterlere uygun olan bölge bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="bolge">Bölge bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında bölge listesi döndürülür.</returns>
        ObjectArray BolgeListele(Kullanici kullanan, Bolge bolge);

        /// <summary>
        /// Bölge, muhasebe birimlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Bolge nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="bolge">Bölge bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc BolgeSil(Kullanici kullanan, Bolge bolge);

        /// <summary>
        /// Marka varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Marka nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="marka">Marka bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc MarkaKaydet(Kullanici kullanan, Marka marka);

        /// <summary>
        /// Verilen kriterlere uygun olan marka bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="marka">Marka bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında marka listesi döndürülür.</returns>
        ObjectArray MarkaListele(Kullanici kullanan, Marka marka);

        /// <summary>
        /// Marka, modellerde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Marka nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="marka">Marka bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc MarkaSil(Kullanici kullanan, Marka marka);

        /// <summary>
        /// Model varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Model nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="model">Model bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ModelKaydet(Kullanici kullanan, Model model);

        /// <summary>
        /// Verilen kriterlere uygun olan model bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="model">Model bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında model listesi döndürülür.</returns>
        ObjectArray ModelListele(Kullanici kullanan, Model model);

        /// <summary>
        /// Model, taşınır malzemelerin özelliklerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Model nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="model">Model bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ModelSil(Kullanici kullanan, Model model);

        /// <summary>
        /// Muhasebe birimi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Muhasebe nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebeBilgisi">Muhasebe birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc MuhasebeKaydet(Kullanici kullanan, Muhasebe muhasebeBilgisi);

        /// <summary>
        /// Muhasebe birimi, harcama birimlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Muhasebe nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebeBilgisi">Muhasebe birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc MuhasebeSil(Kullanici kullanan, Muhasebe muhasebeBilgisi);

        /// <summary>
        /// Verilen kriterlere uygun olan muhasebe birimi bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebe">Muhasebe birimi bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında muhasebe birimi listesi döndürülür.</returns>
        ObjectArray MuhasebeListele(Kullanici kullanan, Muhasebe muhasebe);

        /// <summary>
        /// Verilen muhasebe biriminin KDV mükellefi olup olmadığı bilgisini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kod">Muhasebe birimi kodu</param>
        /// <returns>KDV mükellefi olup olmadığı bilgisi döndürülür.</returns>
        bool MuhasebeKdvMukellefiMi(Kullanici kullanan, string kod);

        /// <summary>
        /// Harcama birimi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen HarcamaBirimi nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="harcamaBirimi">Harcama birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc HarcamaBirimiKaydet(Kullanici kullanan, HarcamaBirimi harcamaBirimi);

        /// <summary>
        /// Verilen kriterlere uygun olan harcama birimi bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="harcamaBirimi">Harcama birimi bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında harcama birimi listesi döndürülür.</returns>
        ObjectArray HarcamaBirimiListele(Kullanici kullanan, HarcamaBirimi harcamaBirimi);

        /// <summary>
        /// Harcama birimi, ambarlarda kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen HarcamaBirimi nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="harcamaBirimi">Harcama birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc HarcamaBirimiSil(Kullanici kullanan, HarcamaBirimi harcamaBirimi);

        /// <summary>
        /// Verilen taşınır hesap planlarını teker teker varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen taşınır hesap planlarından
        /// herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hesapPlani">Taşınır hesap planlarını tutan nesne</param>
        /// <param name="muhasebat">İşlemin muhasebat formatında excel dosyasından gelip gelmediği bilgisi</param>
        /// <param name="kaydetZorla">Hata olsa da kayıt işlemi yapılsın mı bilgisi</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc HesapPlaniKaydet(Kullanici kullanan, ObjectArray hesapPlani, bool muhasebat, bool kaydetZorla);

        /// <summary>
        /// Taşınır hesap planlarını teker teker silen yordam
        /// Eğer, taşınır işlem fişlerinde kullanılmışsa ve/veya detayında başka taşınır hesap
        /// planı varsa ya da kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hesapPlani">Taşınır hesap planlarını tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc HesapPlaniSil(Kullanici kullanan, ObjectArray hesapPlani);

        Sonuc HesapPlaniKullanilmisMi(Kullanici kullanan, HesapPlaniSatir hesapPlani);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır hesap planlarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hesapPlaniSatir">Taşınır hesap planı bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır hesap planları listesi döndürülür.</returns>
        ObjectArray HesapPlaniListele(Kullanici kullanan, HesapPlaniSatir hesapPlaniSatir, Sayfalama sayfalama);

        /// <summary>
        /// Taşınır hesap planlarının detay bilgilerini düzenleyen yordam
        /// </summary>
        void HesapPlaniDetaySayilariDuzenle();

        /// <summary>
        /// Ambar varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Ambar nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ambar">Ambar bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc AmbarKaydet(Kullanici kullanan, Ambar ambar);

        /// <summary>
        /// Verilen kriterlere uygun olan ambar bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ambar">Ambar bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ambar listesi döndürülür.</returns>
        ObjectArray AmbarListele(Kullanici kullanan, Ambar ambar);

        /// <summary>
        /// Parametre olarak verilen ambar bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Ambar nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ambar">Ambar bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc AmbarSil(Kullanici kullanan, Ambar ambar);

        /// <summary>
        /// Oda varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Oda nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="oda">Oda bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc OdaKaydet(Kullanici kullanan, Oda oda);

        /// <summary>
        /// Verilen kriterlere uygun olan oda bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="oda">Oda bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında oda listesi döndürülür.</returns>
        ObjectArray OdaListele(Kullanici kullanan, Oda oda);

        /// <summary>
        /// Parametre olarak verilen oda bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Oda nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="oda">Oda bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc OdaSil(Kullanici kullanan, Oda oda);

        /// <summary>
        /// Silinen ancak kullanımda olan oda bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında oda listesi döndürülür.</returns>
        ObjectArray OdaListeleSilinenOdalar(Kullanici kullanan);

        /// <summary>
        /// Silinen ancak kullanımda olan odalar varsa yeniden oda kodu oluşturan yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Oda nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc OdaKaydetSilinenOdalar(Kullanici kullanan);

        /// <summary>
        /// Personel varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc PersonelKaydet(Kullanici kullanan, Personel personel);

        /// <summary>
        /// Verilen kriterlere uygun olan personel bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında personel listesi döndürülür.</returns>
        ObjectArray PersonelListele(Kullanici kullanan, Personel personel);

        /// <summary>
        /// Personel, zimmet fişlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc PersonelSil(Kullanici kullanan, Personel personel);

        /// <summary>
        /// Taşınır işlem fişini varsa güncelleyen, yoksa kaydeden yordam
        /// Taşınır işlem fişi ve detay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc TasinirIslemFisiKaydet(Kullanici kullanan, TasinirIslemForm tf);

        Sonuc TasinirIslemFisiUstBilgiGuncelle(Kullanici kullanan, TasinirIslemForm tf);

        /// <summary>
        /// Taşınır işlem fişinin durumunu güncelleyen yordam
        /// Parametre olarak verilen yeniDurum bilgisine bakarak taşınır işlem fişine ve detay bilgilerine bir takım
        /// kontroller uygular, verilen fiş bilgileri bütün kontrolleri sorunsuz geçerse belgeye yeni durum atanır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <param name="yeniDurum">Belgeye atanması istenen yeni durum bilgisi - İptal, Onay veya OnayKaldir değerlerini alabilir.</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc TasinirIslemFisiDurumDegistir(Kullanici kullanan, TasinirIslemForm tf, string yeniDurum);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem fişini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <param name="siraNoSirali">Taşınır işlem fişinin detay bilgilerinin satır sırasına göre sıralı olup olmaması bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem fişi döndürülür.</returns>
        ObjectArray TasinirIslemFisiAc(Kullanici kullanan, TasinirIslemForm tf, bool siraNoSirali);
        ObjectArray TasinirIslemDetayKaydiGetir(Kullanici kullanan, TasinirIslemForm tf, bool siraNoSirali, int sayfaNo, int kayitSayisi);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem fişlerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <param name="kriter">Sorgulama için ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem fişleri listesi döndürülür.</returns>
        ObjectArray TasinirIslemFisiListele(Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter kriter);

        Sonuc TasinirIslemFisiKHKFisNoGuncelle(Kullanici kullanan, TasinirIslemForm tf);


        /// <summary>
        /// Verilen taşınır işlem fişinin durum bilgisini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <returns>Taşınır işlem fişinin durumu döndürülür.</returns>
        int TasinirIslemFisiDurumGetir(Kullanici kullanan, TasinirIslemForm tf);

        /// <summary>
        /// Taşınır işlem fişinin, muhasebe işlem fişi ile bağını varsa güncelleyen, yoksa kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <param name="belgeNo">Muhasebe işlem fişi belge numarası</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc TasinirMIFNoKaydet(Kullanici kullanan, TasinirIslemForm tf, string belgeNo);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem fişlerinin muhasebe işlem fişleri ile bağlarını listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında TasinirIslemMIF nesneleri listesi döndürülür.</returns>
        ObjectArray TasinirMIFNoListele(Kullanici kullanan, TasinirIslemForm tf);

        Sonuc TasinirSurecNoKaydet(Kullanici kullanan, TasinirIslemForm tf, string belgeNo);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem fişlerinin muhasebe işlem fişleri ile bağlarını listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında TasinirIslemMIF nesneleri listesi döndürülür.</returns>
        ObjectArray TasinirSurecNoListele(TasinirIslemForm tf);

        /// <summary>
        /// Geçici alındı fişini varsa güncelleyen, yoksa kaydeden yordam
        /// Geçici alındı fişi ve detay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Geçici alındı fişi bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc GeciciAlindiFisiKaydet(Kullanici kullanan, TasinirIslemForm tf);

        /// <summary>
        /// Verilen kriterlere uygun olan geçici alındı fişini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Geçici alındı fişi bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında geçici alındı fişi döndürülür.</returns>
        ObjectArray GeciciAlindiFisiAc(Kullanici kullanan, TasinirIslemForm tf);

        /// <summary>
        /// Verilen kriterlere uygun olan geçici alındı fişlerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Geçici alındı fişi bilgilerini tutan nesne</param>
        /// <param name="kriter">Sorgulama için ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında geçici alındı fişleri listesi döndürülür.</returns>
        ObjectArray GeciciAlindiFisiListele(Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter kriter);

        /// <summary>
        /// Zimmet fişini varsa güncelleyen, yoksa kaydeden yordam
        /// Zimmet fişi ve detay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm ve ObjectArray nesneleri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <param name="zimmetDetay">Zimmet fişinin detay bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ZimmetFisiKaydet(Kullanici kullanan, ZimmetForm zimmet, ObjectArray zimmetDetay);


        /// <summary>
        /// Zimmet fişi eonay varsa güncelleyen, yoksa kaydeden yordam
        /// Zimmet fişi eonay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetFormEOnay  nesneleri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi eonay bilgilerini tutan nesne</param>
        /// <param name="zimmetDetay">Zimmet fişi EOnay bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ZimmetFisiEOnayKaydet(Kullanici kullanan, ZimmetFormEOnay zfeonay);

        /// <summary>
        /// Verilen kriterlere uygun olan zimmet fiş eonay bilgisini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetFormEOnay nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zfeonay">Zimmet fişi eonay bilgilerini tutan nesne</param>
        /// <param name="kriter">Sorgulama için ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişleri listesi döndürülür.</returns>
        ObjectArray ZimmetFisiEOnayListele(Kullanici kullanan, ZimmetFormEOnay zfeonay, ZimmetFormEOnayKriter kriter);

        /// <summary>
        /// Verilen kriterlere uygun olan zimmet fişini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişi döndürülür.</returns>
        ObjectArray ZimmetFisiAc(Kullanici kullanan, ZimmetForm zimmet);

        /// <summary>
        /// Verilen kriterlere uygun olan zimmet fişlerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <param name="kriter">Sorgulama için ek kriter bilgilerini tutan nesne</param>
        /// <param name="eOnayDurum"> Zimmet belgelerinin eposta ile gönderilip onaylanması için eklenildi. null ise kullanılmıyor</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişleri listesi döndürülür.</returns>
        ObjectArray ZimmetFisiListele(Kullanici kullanan, ZimmetForm zimmet, ZimmetFormKriter kriter, string eOnayDurum);

        /// <summary>
        /// Verilen kriterlere uygun olan zimmet fişlerini eonay bilgileri ile beraber döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <param name="kriter">Sorgulama için ek kriter bilgilerini tutan nesne</param>
        /// <param name="eonaykriter"> Sorgulama için eonay kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişleri listesini eonay bilgileri ile döndürülür.</returns>
        ObjectArray ZimmetFisiListeleEOnayBilgisiyle(Kullanici kullanan, ZimmetForm zimmet, ZimmetFormKriter kriter, ZimmetFormEOnayKriter eonaykriter);

        /// <summary>
        /// Verilen kriterlere uygun olan kisi/ortak alan zimmet fişlerini eonay bilgileri ile beraber döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter"> Sorgulama için eonay kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişleri listesini eonay bilgileri ile döndürülür.</returns>
        ObjectArray ZimmetFisiListeleKisiOrtakEOnayBilgisiyle(ZimmetFormEOnayKriter kriter);

        /// <summary>
        /// Verilen zimmet fişinin detay bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişinin detay bilgileri döndürülür.</returns>
        ObjectArray ZimmetFisiDetayListele(Kullanici kullanan, ZimmetForm zimmet);

        /// <summary>
        /// Zimmet fişinin durumunu güncelleyen yordam
        /// Parametre olarak verilen yeniDurum bilgisine bakarak zimmet fişine ve detay bilgilerine bir takım
        /// kontroller uygular, verilen fiş bilgileri bütün kontrolleri sorunsuz geçerse belgeye yeni durum atanır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zf">Zimmet fişi bilgilerini tutan nesne</param>
        /// <param name="yeniDurum">Belgeye atanması istenen yeni durum bilgisi - İptal, Onay veya OnayKaldir değerlerini alabilir.</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ZimmetFisiDurumDegistir(Kullanici kullanan, ZimmetForm zf, string yeniDurum);

        /// <summary>
        /// ZimmetFoemEOnay  durumunu güncelleyen yordam
        /// Parametre olarak verilen yeniDurum bilgisine bakarak zimmet fişi eonay bilgilerine bir takım
        /// kontroller uygular, verilen eonay bilgileri bütün kontrolleri sorunsuz geçerse belgeye yeni durum atanır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetFormEOnay nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zf">Zimmet fişi EOnay bilgilerini tutan nesne</param>
        /// <param name="yeniDurum">Belgeye atanması istenen yeni durum bilgisi - Gönderildi, Cevaplandı KABUL, Cevaplandı RED değerlerini alabilir.</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ZimmetFisiEOnayDurumDegistir(Kullanici kullanan, ZimmetFormEOnay zfeonay, string yeniDurum, string personelKod);

        ZimmetForm ZimmetVarMi(Kullanici kullanan, string gorSicilNo);

        Sonuc ZimmetDusVer(Kullanici kullanan, ZimmetForm zimmet, TNSCollection zimmetDetay, string verilecekPersonelKod, string verilecekOdaKod, ENUMZimmetBelgeTur verilecekBelgeTur);

        /// <summary>
        /// Aktif olarak ambarda olan ve verilen kriterlere uygun olan demirbaşları listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNoHareket">Demirbaş kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        ObjectArray SicilNoListele(Kullanici kullanan, SicilNoHareket sicilNoHareket);
        ObjectArray SicilNoListele(Kullanici kullanan, SicilNoHareket sicilNoHareket, int ilkKayit, int kayitSay);

        /// <summary>
        /// Ambarda veya zimmette ayırdetmeksizin kriterlere uygun olan bütün demirbaşları listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNoHareket">Demirbaş kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        ObjectArray ButunSicilNoListele(Kullanici kullanan, SicilNoHareket sicilNoHareket);
        ObjectArray ButunSicilNoListele(Kullanici kullanan, SicilNoHareket sicilNoHareket, Sayfalama sayfa);

        /// <summary>
        /// Verilen kriterlere uygun olan ve aktif olarak ambarda olan henüz tüketilmemiş sarf malzemelerini listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="shBilgi">Tüketim malzemeleri kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tüketim malzemeleri listesi döndürülür.</returns>
        ObjectArray TuketimListele(Kullanici kullanan, StokHareketBilgi shBilgi);

        /// <summary>
        /// Zimmette olan ve verilen kriterlere uygun olan demirbaşları listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNoHareket">Demirbaş kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        ObjectArray ZimmetliSicilNoListele(Kullanici kullanan, SicilNoHareket sicilNoHareket);

        /// <summary>
        /// Ambarda veya zimmette ayırdetmeksizin kriterlere uygun olan bütün demirbaşları listeleyen yordam
        /// Kriterlerde personel kriteri de varsa sadece zimmetleri listeler.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNoHareket">Demirbaş kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        ObjectArray BarkodSicilNoListele(Kullanici kullanan, SicilNoHareket sicilNoHareket, Sayfalama sayfa);

        /// <summary>
        /// Sayım tutanağı formunu varsa güncelleyen, yoksa kaydeden yordam
        /// Sayım tutanağı formu ve detay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen SayimForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sf">Sayım tutanağı formunun bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc SayimKaydet(Kullanici kullanan, SayimForm sf);

        /// <summary>
        /// Verilen kriterlere uygun olan sayım tutanağı formlarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen SayimForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sf">Sayım tutanağı formu bilgilerini tutan nesne</param>
        /// <param name="detayVer">Sayım tutanağı formlarının detay bilgilerinin istenip istenmediği bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sayım tutanağı formları listesi döndürülür.</returns>
        ObjectArray SayimListele(Kullanici kullanan, SayimForm sf, bool detayVer);

        /// <summary>
        /// Verilen kriterlere uygun olan sayım tutanağı formlarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen SayimForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sf">Sayım tutanağı formu bilgilerini tutan nesne</param>
        /// <param name="detayVer">Sayım tutanağı formlarının detay bilgilerinin istenip istenmediği bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sayım tutanağı formları listesi döndürülür.</returns>
        ObjectArray SayimRaporListele(Kullanici kullanan, SayimForm sf, bool detayVer);

        /// <summary>
        /// Parametre olarak verilen sayım tutanağı formunu ve detayını silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen SayimForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sf">Sayım tutanağı formunun bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc SayimSil(Kullanici kullanan, SayimForm sf);

        /// <summary>
        /// Verilen kriterlere uygun olan ambardaki tüketim malzemelerini,
        /// demirbaşları ve zimmetli demirbaşları döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sf">Sayım tutanağı kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ambardaki malzemelerin detay bilgileri listesi döndürülür.</returns>
        ObjectArray SayimAmbardakileriGetir(Kullanici kullanan, SayimForm sf);

        /// <summary>
        /// Taşınır istek formunu varsa güncelleyen, yoksa kaydeden yordam
        /// Taşınır istek formu ve detay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi
        /// yapılmaktadır, aksi takdirde işlem yapılmaz ve hata döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="iForm">Taşınır istek formu bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc IstekKaydet(Kullanici kullanan, IstekForm iForm);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır istek formlarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen IstekForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="iForm">Taşınır istek formu bilgilerini tutan nesne</param>
        /// <param name="detayVer">Taşınır istek formlarının detay bilgilerinin istenip istenmediği bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır istek formları listesi döndürülür.</returns>
        ObjectArray IstekListele(Kullanici kullanan, IstekForm iForm, bool detayVer);

        /// <summary>
        /// Kayıttan düşme teklif ve onay tutanağını varsa güncelleyen, yoksa kaydeden yordam
        /// Kayıttan düşme teklif ve onay tutanağı ve detay bilgileri ile ilgili bir takım kontroller
        /// yapılmakta, sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen KayittanDusmeForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kf">Kayıttan düşme teklif ve onay tutanağı bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc KayittanDusmeKaydet(Kullanici kullanan, KayittanDusmeForm kf);

        /// <summary>
        /// Verilen kriterlere uygun olan kayıttan düşme teklif ve onay tutanaklarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen KayittanDusmeForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kf">Kayıttan düşme teklif ve onay tutanağı bilgilerini tutan nesne</param>
        /// <param name="detayVer">Kayıttan düşme teklif ve onay tutanaklarının detay bilgilerinin istenip istenmediği bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kayıttan düşme teklif ve onay tutanakları listesi döndürülür.</returns>
        ObjectArray KayittanDusmeListele(Kullanici kullanan, KayittanDusmeForm kf, bool detayVer);

        /// <summary>
        /// Parametre olarak verilen kayıttan düşme teklif ve onay tutanağını ve detaylarını silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen KayittanDusmeForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kf">Kayıttan düşme teklif ve onay tutanağının bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc KayittanDusmeSil(Kullanici kullanan, KayittanDusmeForm kf);

        /// <summary>
        /// Verilen kriterlere uygun olan dayanıklı taşınır defteri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen DefterKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Dayanıklı taşınır defteri kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında dayanıklı taşınır defteri listesi döndürülür.</returns>
        ObjectArray DayanikliTasinirlarDefteriOlustur(Kullanici kullanan, DefterKriter kriter);

        /// <summary>
        /// Verilen kriterlere uygun olan tüketim malzemeleri defteri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen DefterKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Tüketim malzemeleri defteri kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tüketim malzemeleri defteri listesi döndürülür.</returns>
        ObjectArray TuketimMalzemeDefteriOlustur(Kullanici kullanan, DefterKriter kriter);

        /// <summary>
        /// Verilen kriterlere uygun olan müze defteri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen DefterKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Müze defteri kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında müze defteri listesi döndürülür.</returns>
        ObjectArray MuzeDefteriOlustur(Kullanici kullanan, DefterKriter kriter);

        /// <summary>
        /// Verilen kriterlere uygun olan kütüphane defteri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen DefterKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Kütüphane defteri kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kütüphane defteri listesi döndürülür.</returns>
        ObjectArray KutuphaneDefteriOlustur(Kullanici kullanan, DefterKriter kriter);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır sayım ve döküm cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır sayım ve döküm cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır sayım ve döküm cetveli listesi döndürülür.</returns>
        ObjectArray TasinirSayimCetveli(Kullanici kullanan, Cetvel kriter);

        /// <summary>
        /// Verilen kriterlere uygun olan harcama birimi taşınır yönetim hesabı cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Harcama birimi taşınır yönetim hesabı cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında harcama birimi taşınır yönetim hesabı cetveli listesi döndürülür.</returns>
        ObjectArray TasinirYonetimHesabiCetveli(Kullanici kullanan, Cetvel kriter);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır hesap cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır hesap cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır hesap cetveli listesi döndürülür.</returns>
        ObjectArray TasinirHesapCetveli(Kullanici kullanan, Cetvel kriter);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır hesap cetveli (il) bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır hesap cetveli (il) kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır hesap cetveli (il) listesi döndürülür.</returns>
        ObjectArray TasinirHesapCetveliIl(Kullanici kullanan, Cetvel kriter);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır kesin hesap cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır kesin hesap cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır kesin hesap cetveli listesi döndürülür.</returns>
        ObjectArray TasinirKesinHesapCetveli(Kullanici kullanan, Cetvel kriter, bool muhasebeEkle);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır kesin hesap icmal cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır kesin hesap icmal cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır kesin hesap icmal cetveli listesi döndürülür.</returns>
        ObjectArray TasinirKesinHesapIcmalCetveli(Kullanici kullanan, Cetvel kriter, bool muhasebeEkle);

        /// <summary>
        /// Verilen kriterlere uygun olan müze/kütüphane yönetim hesabı cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Müze/Kütüphane yönetim hesabı cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında müze/kütüphane yönetim hesabı cetveli listesi döndürülür.</returns>
        ObjectArray MuzeKutuphaneCetveli(Kullanici kullanan, Cetvel kriter);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır hesap cetveli (bölge) bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır hesap cetveli (bölge) kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır hesap cetveli (bölge) listesi döndürülür.</returns>
        ObjectArray TasinirHesapCetveliBolge(Kullanici kullanan, Cetvel kriter);

        /// <summary>
        /// Yetkili imza bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ImzaBilgisi nesnelerinden
        /// herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="imzaBilgileri">Yetkili imza bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ImzaKaydet(Kullanici kullanan, ObjectArray imzaBilgileri);

        /// <summary>
        /// Verilen kriterlere uygun olan yetkili imza bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebe">Muhasebe birimi kodu</param>
        /// <param name="birim">Harcama birimi kodu</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <param name="imzaYer">ENUMImzaYer listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yetkili imza bilgileri listesi döndürülür.</returns>
        ObjectArray ImzaListele(Kullanici kullanan, string muhasebe, string birim, string ambar, int imzaYer);

        /// <summary>
        /// Parametre olarak verilen firmaları varsa güncelleyen, yoksa kaydeden yordam
        /// </summary>
        /// <param name="firmaBilgileri">Firma bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc FirmaKaydet(ObjectArray firmaBilgileri);

        /// <summary>
        /// Parametre olarak verilen firmaları silen yordam
        /// </summary>
        /// <param name="firmaBilgileri">Firma bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc FirmaSil(ObjectArray firmaBilgileri);

        /// <summary>
        /// Verilen kriterlere uygun olan firma bilgilerini döndüren yordam
        /// </summary>
        /// <param name="firma">Firma bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında firma listesi döndürülür.</returns>
        ObjectArray FirmaListele(FirmaBilgisi firma);

        /// <summary>
        /// Parametre olarak verilen kriterleri kullanarak ilgili demirbaşa ait hareketleri bulan yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNoHareket">Demirbaş kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaşa ait hareket bilgileri listesi döndürülür.</returns>
        ObjectArray SicilNoTarihceListele(Kullanici kullanan, SicilNoHareket sicilNoHareket);

        /// <summary>
        /// Parametre olarak verilen taşınır işlem fişine ait işlem tarihçesi bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebe">Muhasebe birimi kodu</param>
        /// <param name="harcamaBirimi">Harcama birimi kodu</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem fişine ait işlem tarihçesi listesi döndürülür.</returns>
        ObjectArray IslemFormTarihceListele(Kullanici kullanan, int yil, string muhasebe, string harcamaBirimi, string belgeNo);

        /// <summary>
        /// Parametre olarak verilen zimmet fişine ait işlem tarihçesi bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebe">Muhasebe birimi kodu</param>
        /// <param name="harcamaBirimi">Harcama birimi kodu</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <param name="belgeTur">ENUMZimmetBelgeTur listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişine ait işlem tarihçesi listesi döndürülür.</returns>
        ObjectArray ZimmetTarihceListele(Kullanici kullanan, int yil, string muhasebe, string harcamaBirimi, string belgeNo, int belgeTur);

        /// <summary>
        /// Parametre olarak verilen taşınır işlem fişine ait işlem tarihçesi bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebe">Muhasebe birimi kodu</param>
        /// <param name="harcamaBirimi">Harcama birimi kodu</param>
        /// <param name="baslamaTarihi">Başlama Tarihi</param>
        /// <param name="bitisTarihi">Bitiş Tarihi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem fişine ait işlem tarihçesi listesi döndürülür.</returns>
        ObjectArray IslemFormTarihceRapor(Kullanici kullanan, string muhasebe, string harcamaBirimi, TNSDateTime baslamaTarihi, TNSDateTime bitisTarihi);

        /// <summary>
        /// Parametre olarak verilen demirbaşlara ait özellik bilgileri varsa güncelleyen, yoksa kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebe">Muhasebe birimi kodu</param>
        /// <param name="harcamaBirimi">Harcama birimi kodu</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <param name="sicilNoOzellik">Demirbaşlara ait özellik bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc SicilNoOzellikKaydet(Kullanici kullanan, int yil, string muhasebe, string harcamaBirimi, string ambar, ObjectArray sicilNoOzellik);

        /// <summary>
        /// Parametre olarak verilen demirbaşlara ait özellik bilgilerini silen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebe">Muhasebe birimi kodu</param>
        /// <param name="harcamaBirimi">Harcama birimi kodu</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <param name="prSicilNo">Demirbaş listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc SicilNoOzellikSil(Kullanici kullanan, int yil, string muhasebe, string harcamaBirimi, string ambar, int[] prSicilNo);

        /// <summary>
        /// Parametre olarak verilen demirbaşa ait özellik bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="prSicilNo">Demirbaşın programatik sicil numarası</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaşın özellik bilgileri döndürülür.</returns>
        ObjectArray SicilNoOzellikListele(Kullanici kullanan, int prSicilNo);

        /// <summary>
        /// Verilen kriterlere uygun olan malzemelerin ortalama birim fiyat ve
        /// toplam miktar bilgilerini taşınır hesap planı bazında döndüren yordam
        /// hesapKod parametresinde "," ayracı kullanılarak birden fazla taşınır hesap kodu verilebilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="harcamaBirimKod">Harcama birimi kodu</param>
        /// <param name="hesapKod">Taşınır hesap planı kodu/kodları bilgisi</param>
        /// <param name="sadeceRakamOlanlar">Taşınır hesap planı listesinin tamamı mı yoksa sadece rakam olanlar mı listelensin bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır hesap planları, ortalama birim fiyatları ve toplam miktarları listesi döndürülür.</returns>
        ObjectArray HesapPlaniOrtalamaFiyatMiktarListele(Kullanici kullanan, int yil, string harcamaBirimKod, string hesapKod, bool sadeceRakamOlanlar);

        /// <summary>
        /// Parametre olarak verilen yılda harcama birimlerinin kesmiş
        /// oldukları son taşınır işlem fişi numaralarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında harcama birimi bazında taşınır işlem fişi numaraları listesi döndürülür.</returns>
        ObjectArray HarcamaBirimiSonTIFListele(Kullanici kullanan, int yil, TNSDateTime islemTarihIlk, TNSDateTime islemTarihSon);

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan taşınır malzemelerin giren,
        /// çıkan, zimmet miktarları/tutarları gibi bilgileri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin miktar, tutar bilgileri listesi döndürülür.</returns>
        ObjectArray TasinirDepoDurumu(Kullanici kullanan, DepoDurum kriter);

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan taşınır malzemelerin giren,
        /// çıkan, zimmet miktarları/tutarları gibi bilgileri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin miktar, tutar bilgileri listesi döndürülür.</returns>
        //ObjectArray TasinirDepoDurumuKismi(Kullanici kullanan, DepoDurum kriter);

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan taşınır malzemelerin giren,
        /// çıkan, zimmet miktarları/tutarları gibi bilgileri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin miktar, tutar bilgileri listesi döndürülür.</returns>
        ObjectArray KHKRaporu(Kullanici kullanan, DepoDurum kriter);

        /// <summary>
        /// Verilen TuketimCikis kriterlerine uygun olan tüketim malzemelerinin
        /// dönemsel kullanım miktarları bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">TuketimCikis listeleme kriterleri</param>
        /// <param name="fark">Tüketim fark raporu mu, yoksa tüketim raporu mu hazırlansın bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tüketim malzemelerinin dönemsel kullanım bilgileri listesi döndürülür.</returns>
        ObjectArray TuketimCikis(Kullanici kullanan, TuketimCikis kriter, bool fark);

        /// <summary>
        /// Verilen kriterlere uygun olan ortak alan zimmet bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Zimmet listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ortak alan zimmet bilgileri listesi döndürülür.</returns>
        ObjectArray ZimmetOrtakAlan(Kullanici kullanan, ZimmetOrtakAlanVeKisi kriter);

        /// <summary>
        /// Verilen kriterlere uygun olan kişi zimmet bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Zimmet listeleme kriterleri</param>
        /// <param name="kontrolYap">Yetki kontrolü yapılsın mı yapılmasın mı bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kişi zimmet bilgileri listesi döndürülür.</returns>
        ObjectArray ZimmetKisi(Kullanici kullanan, ZimmetOrtakAlanVeKisi kriter, bool kontrolYap);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem bilgileri listesi döndürülür.</returns>
        ObjectArray TasinirIslemMalzemeTarihceRaporu(Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter tfKriter);

        /// <summary>
        /// Verilen kriterlere uygun olan demirbaş bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Demirbaş listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş bilgileri listesi döndürülür.</returns>
        ObjectArray SicilRaporu(Kullanici kullanan, SicilNoHareket kriter);

        /// <summary>
        /// Verilen kriterlere uygun olan girişi yapılmamış devir çıkış belgeleri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında girişi yapılmamış devir çıkış belgeleri listesi döndürülür.</returns>
        ObjectArray GirisiYapilmamisDevirCikislari(Kullanici kullanan, TasinirIslemForm kriter);

        /// <summary>
        /// Verilen KurusFarki kriterlerine uygun olan taşınır malzemelerinin
        /// kuruş fark tutarları bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">KurusFarki listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin kuruş fark tutarları listesi döndürülür.</returns>
        ObjectArray KurusFarklariRaporu(Kullanici kullanan, KurusFarki kriter);

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan taşınır malzemelerin giren, çıkan, önceki
        /// yılın tüketim, dönemsel tüketim miktarları/tutarları gibi bilgileri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin miktar, tutar bilgileri listesi döndürülür.</returns>
        ObjectArray EnCokKullanilanTasinirlar(Kullanici kullanan, DepoDurum kriter);

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan personel sayılarını istenen bazda döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dp">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında personel sayıları listesi döndürülür.</returns>
        ObjectArray PersonelSayilari(Kullanici kullanan, DepoDurum dp);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır istek ve karşılanan miktarlarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır istek listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır istek bilgileri listesi döndürülür.</returns>
        ObjectArray IstekKarsilananRaporu(Kullanici kullanan, IstekForm kriter);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır malzemelerinin giren, çıkan, kalan tutarlarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin tutar bilgileri listesi döndürülür.</returns>
        ObjectArray MuhasebatKontrolRaporu(Kullanici kullanan, MuhasebatKontrol kriter);

        /// <summary>
        /// Parametre olarak verilen SQL cümlesini çalıştırıp sonucunu döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sql">Çalıştırılacak SQL cümlesi</param>
        /// <returns>Çalıştırılan SQL cümlesi sonucunda gelen bilgi formatlanarak döndürülür.</returns>
        string SQLCalistir(Kullanici kullanan, string baglanti, string sql);

        //Sonuc LisansKontrol();

        //Sonuc ScriptCalistir();

        /// <summary>
        /// Harcama birimi değişikliğini yapan yordam
        /// Bu yordam, bütün veri tabanındaki kayıtları gezer ve harcama birimi geçen verileri günceller.
        /// Gönderilen bilgiler eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="harcamaBirimiEski">Eski harcama birimi bilgisini tutan nesne</param>
        /// <param name="harcamaBirimiYeni">Yeni harcama birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc HarcamaBirimDegistir(Kullanici kullanan, HarcamaBirimi harcamaBirimiEski, HarcamaBirimi harcamaBirimiYeni);

        /// <summary>
        /// İhtiyaç fazlası taşınır form ve detay bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AcikPazarForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="apf">İhtiyaç fazlası taşınır bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc AcikPazarKaydet(Kullanici kullanan, AcikPazarForm apf);

        /// <summary>
        /// Verilen kriterlere uygun olan ihtiyaç fazlası taşınır bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="apf">İhtiyaç fazlası taşınır bilgilerini tutan nesne</param>
        /// <param name="sorgudan">İsteğin sorgu ekranından mı yoksa form ekranından mı yapıldığı bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ihtiyaç fazlası taşınır bilgileri listesi döndürülür.</returns>
        ObjectArray AcikPazarListele(Kullanici kullanan, AcikPazarForm apf, bool sorgudan);

        /// <summary>
        /// İhtiyaç fazlası taşınır form, detay ve istek bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AcikPazarForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="apf">İhtiyaç fazlası taşınır bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc AcikPazarSil(Kullanici kullanan, AcikPazarForm apf);

        /// <summary>
        /// İhtiyaç fazlası taşınır malzemelerine yapılan istek bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen bilgiler eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="apf">İhtiyaç fazlası taşınır bilgilerini tutan nesne</param>
        /// <param name="istekler">İhtiyaç fazlası taşınır malzemelerine yapılan istek listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc AcikPazarIstekKaydet(Kullanici kullanan, AcikPazarForm apf, TNSCollection istekler);

        /// <summary>
        /// Verilen kriterlere uygun olan ihtiyaç fazlası taşınır istek bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="istek">İhtiyaç fazlası taşınır istek bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ihtiyaç fazlası taşınır istek bilgileri listesi döndürülür.</returns>
        ObjectArray AcikPazarIstekListele(Kullanici kullanan, AcikPazarIstek istek);

        /// <summary>
        /// Amortisman sınırı varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanSinir nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sinir">Amortisman sınır bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc AmortismanSinirKaydet(Kullanici kullanan, AmortismanSinir sinir);

        /// <summary>
        /// Parametre olarak verilen yılın amortisman sınırını döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında amortisman sınır bilgileri döndürülür.</returns>
        ObjectArray AmortismanSinirListele(Kullanici kullanan, int yil);

        /// <summary>
        /// Verilen demirbaş sicil ve RFID eşleşme bilgilerini teker teker varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen eşleşmelerden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="rfIdler">Demirbaş sicil ve RFID eşleşme bilgileri listesini tutan nesne</param>
        /// <param name="webServis">Eşleşme bilgileri web servisten mi geliyor yoksa başka bir arayüzden mi bilgisi</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc SicilRFIDKaydet(Kullanici kullanan, TNSCollection rfIdler, bool webServis);
        Sonuc SicilRFIDKaydet(Kullanici kullanan, ObjectArray rfIdler);
        ObjectArray SicilRFIDListele(Kullanici kullanan, string rfIdNo, int prSicilNo);

        /// <summary>
        /// Demirbaş sicil ve RFID eşleşme bilgilerini teker teker silen yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="rfIdler">Demirbaş sicil ve RFID eşleşme bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc SicilRFIDSil(Kullanici kullanan, TNSCollection rfIdler);

        /// <summary>
        /// Verilen kriterlere uygun olan demirbaş sicil ve RFID eşleşme bilgilerini döndüren yordam
        /// Eşleşme işlemi daha önceden yapılmış olsun veya olmasın bütün demirbaşlar listelenir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNo">Listeleme kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        ObjectArray SicilListeleRFIDHepsi(Kullanici kullanan, SicilNoHareket sicilNo, int ilkKayit = 0, int kayitSay = 0);

        /// <summary>
        /// Verilen kriterlere uygun olan demirbaş sicil ve RFID eşleşme bilgilerini döndüren yordam
        /// Eşleşme işlemi daha önceden yapılmış demirbaşlar listelenir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNo">Listeleme kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        ObjectArray SicilListeleRFIDEslesen(Kullanici kullanan, SicilNoHareket sicilNo, int ilkKayit = 0, int kayitSay = 0);

        /// <summary>
        /// Verilen kriterlere uygun olan demirbaş sicil ve RFID eşleşme bilgilerini döndüren yordam
        /// Eşleşme işlemi daha önceden yapılmamış demirbaşlar listelenir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNo">Listeleme kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        ObjectArray SicilListeleRFIDEslesmeyen(Kullanici kullanan, SicilNoHareket sicilNo, int ilkKayit = 0, int kayitSay = 0);

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilmeyen malzemeler kurumdan çıkarılacak
        /// olursa bilgilendirilecek personele ait iletişim bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Bilgi verilecek personele ait iletişim bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc KapiGecisSmsKaydet(Kullanici kullanan, Personel personel);

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilmeyen malzemeler kurumdan çıkarılacak
        /// olursa bilgilendirilecek personele ait iletişim bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">İletişim bilgileri silinecek personele ait bilgileri tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc KapiGecisSmsSil(Kullanici kullanan, Personel personel);

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilmeyen malzemeler kurumdan çıkarılacak
        /// olursa bilgilendirilecek personele ait iletişim bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında personel iletişim bilgileri listesi döndürülür.</returns>
        ObjectArray KapiGecisSmsListele(Kullanici kullanan, Personel personel);

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilen malzemeleri kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen demirbaşlardan herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="siciller">Geçişine izin verilen demirbaşların listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc KapiGecisSicilKaydet(Kullanici kullanan, TNSCollection siciller);

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilen malzemelerin izinlerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen demirbaşlardan herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="siciller">Geçiş izni silinen  demirbaşların listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc KapiGecisSicilSil(Kullanici kullanan, TNSCollection siciller);

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilen malzemelerin verilen kriterlere uyanlarını listeleyen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen SicilNoHareket nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicil">Geçişine izin verilen demirbaşları listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında geçişine izin verilen malzeme bilgileri listesi döndürülür.</returns>
        ObjectArray KapiGecisSicilListele(Kullanici kullanan, SicilNoHareket sicil);

        /// <summary>
        /// Parametre olarak verilen demirbaşın RFID entegrasyonunda kurumdan
        /// çıkışına izin verilip verilmediği bilgisini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="prSicilNo">Demirbaşın programatik sicil numarası</param>
        /// <param name="rfIdNo">Demirbaşın RFID numarası</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc KapiGecisSicilGecebilirMi(Kullanici kullanan, int prSicilNo, int rfIdNo);

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilmeyen malzemeler
        /// kurumdan çıkarılacak olursa bilgilendirme loglarını kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kgller">Bilgilendirme logları listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc KapiGecisLogKaydet(Kullanici kullanan, TNSCollection kgller);

        /// <summary>
        /// Verilen kriterlere uygun olan bilgilendirme loglarını döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kgl">Bilgilendirme logları listeleme kriterleri</param>
        /// <param name="tarih1">Tarih aralığının alt sınırı</param>
        /// <param name="tarih2">Tarih aralığının üst sınırı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında bilgilendirme logları listesi döndürülür.</returns>
        ObjectArray KapiGecisLogListele(Kullanici kullanan, KapiGecisLog kgl, TNSDateTime tarih1, TNSDateTime tarih2);

        Sonuc RFIDSayimBaslat(Kullanici kullanan, SayimRFID srfid);

        /// <summary>
        /// RFID sayım bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen SayimRFID nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="srfid">RFID sayım bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc RFIDSayimKaydet(Kullanici kullanan, SayimRFID srfid, bool detaySifirla);

        /// <summary>
        /// Verilen kriterlere uygun olan RFID sayım bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="srfid">RFID sayım kriterlerini tutan nesne</param>
        /// <param name="detay">RFID sayımının detay bilgileri listelensin mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında RFID sayım bilgileri listesi döndürülür.</returns>
        ObjectArray RFIDSayimListele(Kullanici kullanan, SayimRFID srfid, bool detay);

        /// <summary>
        /// Verilen yıl, muhasebe birimi ve harcama birimi bazında son taşınır işlem
        /// fişi iptal edilmişse belge numarasını, değilse boşluk döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcamaKod">Harcama birimi</param>
        /// <returns>Belge numarası döndürülür.</returns>
        string HarcamaYonetimiIcinTIFNoVer(int yil, string muhasebe, string harcamaKod);

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının muhasebe-harcama-ambar üçlüsü
        /// üzerinde yazma yetkisine sahip olup olmadığının kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcama">Harcama birimi</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <returns>Yetki kontrolünün sonucu döndürülür.</returns>
        bool YazmaYetkisiVarMi(Kullanici kullanan, string muhasebe, string harcama, string ambar);

        ObjectArray TekrarEdenSicilleriBul(Kullanici kullanan, SicilNoHareket sicilNo);
        Sonuc SicilNoGuncelle(Kullanici kullanan, SicilNoHareket[] sicilNolar);

        Sonuc TalepKarsilamaKaydet(Kullanici kullanan, TalepKarsilama tk);
        Sonuc TalepKarsilamaCevapKaydet(Kullanici kullanan, TalepKarsilamaCevap tk);
        Sonuc TalepKarsilamaDurumGuncelle(Kullanici kullanan, string refId, int durumKod);
        Sonuc TalepDetayIptal(Kullanici kullanan, string urunRefId);
        Sonuc TalepKarsilamaCevapSil(Kullanici kullanan, TalepKarsilamaCevap tk);
        Sonuc TalepKarsilamaCevapTamamla(Kullanici kullanan, TalepKarsilamaCevap tk);

        ObjectArray TalepKarsilamaListele(Kullanici kullanan, TalepKarsilama tk, bool detayVer);
        Sonuc TalepKarsilamaDurumDegistir(Kullanici kullanan, TalepKarsilama tk);

        ObjectArray TalepKarsilamaCevapListeleSatinAlma(Kullanici kullanan, TalepKarsilamaCevap tk);
        ObjectArray TalepKarsilamaCevapListele(Kullanici kullanan, TalepKarsilamaCevap tk, bool detayVer);

        ObjectArray TalepKarsilamaListele(Kullanici kullanan, TalepKarsilama tk, bool detayVer, bool harcamaModulu);

        Sonuc TalepKarsilamaTuketimTIFOlustur(Kullanici kullanan, TalepKarsilama tk);

        Sonuc TuketimUretimKaydet(Kullanici kullanan, TuketimUretim tu);

        ObjectArray TuketimUretimListele(Kullanici kullanan, TuketimUretim tu, bool detayVer);

        Sonuc GiyecekTechizatKaydet(Kullanici kullanan, GiyecekTechizat tu);

        ObjectArray GiyecekTechizatListele(Kullanici kullanan, GiyecekTechizat tu, bool detayVer);

        Sonuc DevirGirisTIFKaydetOnayla(Kullanici kullanan, TasinirIslemForm giris, TNSCollection cikislar);

        //ObjectArray BirimFiyatOrtalamalari(Kullanici kullanan, BirimFiyatOrtalamasi kriter);

        //int PrSicilNoRFIDDondur(int prSicilNo);

        ObjectArray TasinirResimGetir(int prSicilNo, string resimID);

        Sonuc TasinirResimSil(TasResim bilgi);

        Sonuc TasinirResimKaydet(TasResim bilgi);

        ObjectArray TopluIslemListele(Kullanici kullanan, TopluIslem t);
        Sonuc TopluIslemGuncelle(Kullanici kullanan, TopluIslem t);
        ObjectArray TopluIslemDetayGrupKodListele(Kullanici kullanan);
        ObjectArray TopluIslemFormListele(Kullanici kullanan, TopluIslemForm t);
        Sonuc TopluIslemFormKaydet(Kullanici kullanan, TopluIslemForm t);
        Sonuc TopluIslemFormSil(Kullanici kullanan, int grupKod);

        ObjectArray HesapPlaniBarkodGetir(TasHesapPlaniBarkod barkod);
        Sonuc HesapPlaniBarkodSil(Kullanici kullanan, string barkodListe);
        Sonuc HesapPlaniBarkodKaydet(Kullanici kullanan, TasHesapPlaniBarkod barkod);

        ObjectArray AmortismanRaporla(Kullanici kullanan, AmortismanKriter form, bool bitenlerDahil);
        ObjectArray AmortismanRaporla2(Kullanici kullanan, AmortismanKriter form);
        ObjectArray AmortismanRaporla3(Kullanici kullanan, AmortismanKriter form);
        AmortismanRaporBA AmortismanRaporlaBA(Kullanici kullanan, AmortismanKriter form);

        Sonuc AmortismanVerisiOlustur(Kullanici kullanan, AmortismanKriter ak, int islemSiraNo);
        Sonuc AmortismanVerisiOlustur2(Kullanici kullanan, AmortismanKriter ak);
        Sonuc AmortismanVerisiOlustur3(Kullanici kullanan, AmortismanKriter ak);
        Sonuc AmortismanRakamKaydet(Kullanici kullanan, string kod, int yil, int donem, int prSicilNo, ENUMAmortismanTur amortismanTur, decimal amortismanMiktar, string saymanlikKod, string birimKod, string ambarKod);
        //ObjectArray AmortismanDTDOlustur(Kullanici kullanan, DefterKriter kriter);

        /// <summary>
        /// Taşınır malzemelerinin amortisman sürelerini varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanSure nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sure">Amortisman süre bilgisini tutan nesne</param>
        /// <returns>
        /// İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.
        /// </returns>
        Sonuc AmortismanSureKaydet(Kullanici kullanan, HesapPlaniSatir sure);

        /// <summary>
        /// SayimKisiForm bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ImzaBilgisi nesnelerinden
        /// herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="form">The form.</param>
        /// <returns>
        /// İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.
        /// </returns>
        Sonuc SayimKisiFormBaslat(Kullanici kullanan, SayimKisiForm form);

        /// <summary>
        /// SayimKisiForm bilgilerini listeleyen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ImzaBilgisi nesnelerinden
        /// herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="imzaBilgileri">Yetkili imza bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        ObjectArray SayimKisiFormListele(Kullanici kullanan, SayimKisiForm form);

        /// <summary>
        /// SayimKisiForm bilgilerini güncelleyen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ImzaBilgisi nesnelerinden
        /// herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="form">The form.</param>
        /// <returns>
        /// İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.
        /// </returns>
        Sonuc SayimKisiFormSonlandir(Kullanici kullanan, SayimKisiForm form);

        /// <summary>
        /// kisi bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ImzaBilgisi nesnelerinden
        /// herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="form">The form.</param>
        /// <returns>
        /// İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.
        /// </returns>
        Sonuc SayimKisiYeniKisiSay(Kullanici kullanan, Sayimkisi form);

        /// <summary>
        /// kisi detay bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ImzaBilgisi nesnelerinden
        /// herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="liste">The liste.</param>
        /// <returns>
        /// İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.
        /// </returns>
        Sonuc SayimKisiDetayKaydet(Kullanici kullanan, ObjectArray liste);

        /// <summary>
        /// Sayims the kisi kisi listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        ObjectArray SayimKisiKisiListele(Kullanici kullanan, Sayimkisi form);

        /// <summary>
        /// Sayims the kisi kisi detay listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form"> The form.</param>
        /// <returns></returns>
        ObjectArray SayimKisiKisiDetayListele(Kullanici kullanan, SayimKisiDetay form);

        /// <summary>
        /// Sayims the kisi kisi detay listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="sayim">The sayim.</param>
        /// <returns></returns>
        Sonuc SayimKisiDetayOkundu(Kullanici kullanan, SayimKisiDetay sayim);

        /// <summary>
        /// Sayims the kisi birime yetkilimi.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="sayim">The sayim.</param>
        /// <returns></returns>
        Sonuc SayimKisiBirimeYetkilimi(Kullanici kullanan, SayimKisiForm sayim);

        Sonuc SayimKisiFazlaSil(Kullanici kullanan, SayimKisiDetay sayim);

        /// <summary>
        /// SayimAmbarForm bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ImzaBilgisi nesnelerinden
        /// herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="form">The form.</param>
        /// <returns>
        /// İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.
        /// </returns>
        Sonuc SayimAmbarFormBaslat(Kullanici kullanan, SayimAmbarForm form);

        /// <summary>
        /// SayimAmbarForm bilgilerini listeleyen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ImzaBilgisi nesnelerinden
        /// herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="imzaBilgileri">Yetkili imza bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        ObjectArray SayimAmbarFormListele(Kullanici kullanan, SayimAmbarForm form);

        /// <summary>
        /// SayimAmbarForm bilgilerini güncelleyen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ImzaBilgisi nesnelerinden
        /// herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="form">The form.</param>
        /// <returns>
        /// İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.
        /// </returns>
        Sonuc SayimAmbarFormSonlandir(Kullanici kullanan, SayimAmbarForm form);

        /// <summary>
        /// ambar bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ImzaBilgisi nesnelerinden
        /// herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="form">The form.</param>
        /// <returns>
        /// İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.
        /// </returns>
        Sonuc SayimAmbarYeniAmbarSay(Kullanici kullanan, Sayimambar form);

        /// <summary>
        /// ambar detay bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ImzaBilgisi nesnelerinden
        /// herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="liste">The liste.</param>
        /// <returns>
        /// İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.
        /// </returns>
        Sonuc SayimAmbarDetayKaydet(Kullanici kullanan, ObjectArray liste);

        /// <summary>
        /// Sayims the ambar ambar listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        ObjectArray SayimAmbarAmbarListele(Kullanici kullanan, Sayimambar form);

        /// <summary>
        /// Sayims the ambar ambar detay listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form"> The form.</param>
        /// <returns></returns>
        ObjectArray SayimAmbarAmbarDetayListele(Kullanici kullanan, SayimAmbarDetay form);

        /// <summary>
        /// Sayims the ambar ambar detay listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="sayim">The sayim.</param>
        /// <returns></returns>
        Sonuc SayimAmbarDetayOkundu(Kullanici kullanan, SayimAmbarDetay sayim);

        /// <summary>
        /// Sayims the ambar birime yetkilimi.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="sayim">The sayim.</param>
        /// <returns></returns>
        Sonuc SayimAmbarBirimeYetkilimi(Kullanici kullanan, SayimAmbarForm sayim);

        /// <summary>
        /// Sayims the ambarda fazlayı siler.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="sayim">The sayim.</param>
        /// <returns></returns>
        Sonuc SayimAmbarFazlaSil(Kullanici kullanan, SayimAmbarDetay sayim);

        /// <summary>
        /// Sayims the ambar ambar detay listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form"> The form.</param>
        /// <returns></returns>
        ObjectArray SayimKisiRapor(Kullanici kullanan, Sayimkisi form);

        /// <summary>
        /// Sayims the ambar rapor indir.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form"> The form.</param>
        /// <returns></returns>
        ObjectArray SayimAmbarRapor(Kullanici kullanan, Sayimambar form);

        /// <summary>
        /// Sayims the ambar rapor indir.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form"> The form.</param>
        /// <returns></returns>
        ObjectArray SayimKisiRaporRapor(Kullanici kullanan, Sayimkisi form);

        Sonuc AmortismanSinirMIFKaydet(Kullanici kullanan, int yil, string mifNo);

        Sonuc TesisBilesenleriKaydet(Kullanici kullanan, TesisBilesenleri tk);
        Sonuc TesisBilesenleriDurumGuncelle(Kullanici kullanan, string refId, int durumKod);
        ObjectArray TesisBilesenleriListele(Kullanici kullanan, TesisBilesenleri tk);

        Sonuc UretimFormuKaydet(Kullanici kullanan, UretimFormu tk);
        Sonuc UretimFormuDurumGuncelle(Kullanici kullanan, string kod, int durumKod);
        ObjectArray UretimFormuListele(Kullanici kullanan, UretimFormu tk);

        Sonuc UretimRecetesiKaydet(Kullanici kullanan, ObjectArray liste);
        ObjectArray UretimRecetesiListele(Kullanici kullanan, UretimRecetesi tk);


        Sonuc AktarimKaydet(Kullanici kullanan, Aktarim tk);
        Sonuc AktarimDevamEt(Kullanici kullanan, Aktarim tk);
        Sonuc AktarimDurumGuncelle(Kullanici kullanan, string refId, int durumKod);
        ObjectArray AktarimListele(Kullanici kullanan, Aktarim tk);
        ObjectArray AktarimListeleTarihce(Kullanici kullanan, string ilgiKod);
        Sonuc TasinirIslemFisiOnayDurumDegistir(Kullanici kullanan, TasinirIslemForm tf, ENUMTasinirIslemFormOnayDurumu yeniDurum);

        ObjectArray TasinirIslemFisiOnayYetkiListele(Kullanici kullanan, TasinirIslemOnayYetki form);
        Sonuc TasinirIslemFisiOnayYetkiKaydet(Kullanici kullanan, string muhasebeKod, string harcamaBirimKod, TasinirIslemOnayYetki[] liste);

        Sonuc BisKaydet(Kullanici kullanan, int yil, string muhasebe, string harcamaBirimi, string ambar, string sicilNo, int prSicilNo, SicilNoOzellik[] formListe);
        ObjectArray BisSicilListele(Kullanici kullanan, SicilNoHareket sicilNo);
        ObjectArray IliskiliMalzemeListele(Kullanici kullanan, int[] prSicilNoListe, bool sadeceSatirSayisi = false);
        int IliskiliMalzemeSayisi(Kullanici kullanan, int[] prSicilNoListe);

        ObjectArray DosyaSicilNoListele(Kullanici kullanan, SicilNoHareket sicilNo);

        Sonuc RFIDEtiketTuruKaydet(Kullanici kullanan, RFIDEtiketTuru form);
        ObjectArray RFIDEtiketTuruListele(Kullanici kullanan, RFIDEtiketTuru form);
        Sonuc RFIDEtiketTuruSil(Kullanici kullanan, RFIDEtiketTuru form);

        ObjectArray IslemDosyaGetir(IslemDosya bilgi);

        Sonuc IslemDosyaSil(IslemDosya bilgi);

        Sonuc IslemDosyaKaydet(IslemDosya bilgi);

        bool GormeYetkisiVarMi(Kullanici kullanan, string muhasebe, string harcama, string ambar);

        Sonuc GeciciAlindiFisiSicilOlustur(Kullanici kullanan, TasinirIslemForm tf, string islem);

        Sonuc GeciciAlindiFisiTifFisNoKaydet(Kullanici kullanan, TasinirIslemForm tf);

        ObjectArray DevirCikis(Kullanici kullanan, DevirCikis kriter);

        Sonuc SicilNoVer(Kullanici kullanan, ObjectArray sicilNolar);

        Sonuc SicilNoDegerArtisKaydet(Kullanici kullanan, SicilNoDegerArtis tk);
        Sonuc SicilNoDegerArtisKaydet(Kullanici kullanan, ObjectArray bilgiler);
        Sonuc SicilNoDegerArtisSil(Kullanici kullanan, SicilNoDegerArtis tk);
        ObjectArray SicilNoDegerArtisListele(Kullanici kullanan, SicilNoDegerArtis tk);
        Sonuc SicilNoDegerArtisDurumGuncelle(Kullanici kullanan, SicilNoDegerArtis tk, string durumTur);

        ObjectArray BAOnayiYapilacaklarListele(Kullanici kullanan, string mernis, BAOnay kriter);
        ObjectArray BAOnayiSorgulama(Kullanici kullanan, BAOnay tf);
        Sonuc AmortismanIslemFisiOnayDurumDegistir(Kullanici kullanan, AmortismanIslemForm form, ENUMTasinirIslemFormOnayDurumu yeniDurum);
        Sonuc AmortismanIslemFisiDurumDegistir(Kullanici kullanan, AmortismanIslemForm form, ENUMAmortismanFormDurumu yeniDurum);
        Sonuc AmortismanIslemFisiKaydet(Kullanici kullanan, AmortismanIslemForm form);
        ObjectArray AmortismanIslemFisiListele(Kullanici kullanan, AmortismanIslemForm form);

        ObjectArray MuhasebeBilgiListele(Kullanici kullanan, MuhasebeIslemi form);
        ObjectArray MuhasebeBilgiOnIzlemeOlustur(Kullanici kullanan, MuhasebeIslemiKriter form);
        Sonuc MuhasebeServisineGonder(Kullanici kullanan, MuhasebeIslemi[] liste);
        Sonuc MuhasebeBilgiDurumSorgulaVeGuncelle(Kullanici kullanan, MuhasebeIslemi[] liste);

        ObjectArray AmortismanRaporuMB(Kullanici kullanan, AmortismanKriter ak);
        ObjectArray BAOnayiTarihceListele(MuhasebeIslemiKriter form);

        decimal SonBedelGetir(Kullanici kullanan, int prSicilNo);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem bilgileri listesi döndürülür.</returns>
        ObjectArray TasinirKodGrupluTasinirHurdaRaporu(Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter tfKriter);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem bilgileri listesi döndürülür.</returns>
        ObjectArray IlBazliTasinirHurdaRaporu(Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter tfKriter);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem bilgileri listesi döndürülür.</returns>
        ObjectArray BakanlikDuzeyiTasinirHurdaRaporu(Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter tfKriter);

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem bilgileri listesi döndürülür.</returns>
        ObjectArray BirimItibariylaTasinirHurdaRaporu(Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter tfKriter);

        /// <summary>
        /// Verilen kriterlere uygun olan ABSKuraGoreHarcamaBirimleri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl bilgisini tutan nesne</param>     
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem bilgileri listesi döndürülür.</returns>

        ObjectArray ABSKuraGoreHarcamaBirimleriListele(Kullanici kullanan, int yil);

        ObjectArray ToplamYillikTuketimRaporu(Kullanici kullanan, DepoDurum kriter);
        ObjectArray HesapPlaniBazindaYillikTuketimRaporu(Kullanici kullanan, DepoDurum kriter);

        //ObjectArray ZimmetKisiBirimAmbarBazli(Kullanici kullanan, ZimmetForm kriter);

        //ObjectArray ZimmetOrtakAlanBirimAmbarBazli(Kullanici kullanan, ZimmetForm kriter);
        //ObjectArray ZimmetKisideki(Kullanici kullanan, ZimmetForm kriter);

        ObjectArray AyarlarMBListele(Kullanici kullanan, AyarlarMB form);
        Sonuc AyarlarMBKaydet(Kullanici kullanan, AyarlarMB form);
        Sonuc AyarlarMBSil(Kullanici kullanan, AyarlarMB form);

        bool MutemmimAktifMi(Kullanici kullanan, AyarlarMB form);

        Sonuc TestYap(Kullanici kullanan, int islem, object form);

        int AmortismanDonemiBul(string harcamaKod, TNSDateTime tarih);

        Sonuc HarmoniKaydet(Kullanici kullanan, Harmoni form);
        ObjectArray HarmoniListele(Kullanici kullanan, Harmoni form);

        ObjectArray BirimlereGoreDemirbasRaporu(Kullanici kullanan, TasinirIslemDetay form);

        Sonuc HesapPlanKodDegistir(Kullanici kullanan, string muhasebe, string harcamaBirimi, string ambar, int prSicilNo, HesapPlaniSatir form);

        Sonuc EnflasyonDuzeltmesiKaydet(Kullanici kullanan, EnflasyonDuzeltmesi enflasyonDuzeltmesi);
        Sonuc EnflasyonDuzeltmesiSil(Kullanici kullanan, EnflasyonDuzeltmesi enflasyonDuzeltmesi);
        ObjectArray EnflasyonDuzeltmesiListele(Kullanici kullanan, EnflasyonDuzeltmesi enflasyonDuzeltmesi);
        Sonuc EnflasyonDuzeltmesiAmortismanKaydet(Kullanici kullanan, AmortismanKriter ar);
        Sonuc EnflasyonDuzeltmesiTifKaydet(Kullanici kullanan, AmortismanKriter ar);

        ObjectArray KontrolRaporu(Kullanici kullanan, KontrolRaporu form);

        Sonuc EBYSCumhurbaskanligiKaydet(Kullanici kullanan, EBYSCumhurbaskanligi form);
        ObjectArray EBYSCumhurbaskanligiListele(Kullanici kullanan, EBYSCumhurbaskanligi form);

        Sonuc EBYSCumhurbaskanligiBirimKaydet(Kullanici kullanan, EBYSCumhurbaskanligiBirim form);
        ObjectArray EBYSCumhurbaskanligiBirimListele(Kullanici kullanan, EBYSCumhurbaskanligiBirim form);

        Sonuc RFIDHareketKaydet(Kullanici kullanan, RFIDHareket form);
        ObjectArray RFIDHareketListele(Kullanici kullanan, RFIDHareket form);
        Sonuc RFIDEtiketTarihiGuncelle(Kullanici kullanan, int prSicilNo, TNSDateTime tarih);
        int PrSicilNoGetir(Kullanici kullanan, string sicilNo);
        ObjectArray PrSicilNoListele(Kullanici kullanan, SicilNoHareket form);

        Sonuc RFIDYaziciKaydet(Kullanici kullanan, RFIDYazici form);
        ObjectArray RFIDYaziciListele(Kullanici kullanan, RFIDYazici form);
        Sonuc RFIDYaziciSil(Kullanici kullanan, RFIDYazici form);

        Sonuc SayimAmbarFormSil(Kullanici kullanan, SayimAmbarForm form);

        Sonuc SayimKisiFormSil(Kullanici kullanan, SayimKisiForm form);
    }
}