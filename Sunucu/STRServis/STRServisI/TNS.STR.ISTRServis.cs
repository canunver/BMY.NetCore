using System;
using TNS.KYM;

namespace TNS.STR
{
    /// <summary>
    /// Remoting yöntemi ile STRServis nesnesi oluşturmak için kullanılan sınıf
    /// Ayrıca, istemcide ve sunucuda ihtiyaç duyulan ortak kullanılacak statik fonksiyonlar da bu sınıfta yer alabilir.
    /// </summary>
    [Serializable()]
    public static class Arac
    {
        /// <summary>
        /// STRServis nesnesini, konfigürasyon dosyasından okuduğu remoting dizgisini kullanarak
        /// .net remoting ile oluşturup döndüren statik yordam
        /// </summary>
        /// <returns>.net remoting ile oluşturulan ISTRServis arayüzünden türemiş nesne</returns>
        public static ISTRServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("STRServisURL");
            ISTRServis servis = TNS.Api.ServisIstemci.Olustur<ISTRServis>(sUrl);
            return servis;
        }
    }

    /// <summary>
    /// Stratejik plan modülünün okuma, yazma gibi yetki kontrollerini yapan statik fonksiyonları içeren sınıf
    /// </summary>
    [Serializable()]
    public static class Yetki
    {
        /// <summary>
        /// Parametre olarak gönderilen kullanıcının istemci tarafındaki stratejik plan
        /// sayfalarını görmeye yetkili olup olmadığının kontrolünü yapan yordam
        /// Kullanıcının stratejik plan yetkisi varsa true, yoksa false döndürür.
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
        private static bool YoneticiMi(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının stratejik plan
        /// bilgilerini görme yetkisinin olup olmadığını kontrol eden yordam
        /// Kullanıcının stratejik plan bilgilerini görme yetkisi varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">İstekte bulunan kullanıcı</param>
        /// <returns>Görme yetkisi olup olmadığının sonucu</returns>
        private static bool GormeYetkisiVarMi(Kullanici kullanan)
        {
            return
                kullanan.YetkiTipiDegeriIceriyorMu("Stratejik Plan Yetkisi", "1", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Stratejik Plan Yetkisi", "2", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Stratejik Plan Yetkisi", "3", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Stratejik Plan Yetkisi", "4", false);
            //|| TNS.PEB.Yetki.OkumaHakkiVarMi(kullanan).islemSonuc;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının stratejik plan
        /// bilgilerini değiştirme yetkisinin olup olmadığını kontrol eden yordam
        /// Kullanıcının stratejik plan bilgilerini değiştirme yetkisi varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">İstekte bulunan kullanıcı</param>
        /// <returns>Yazma yetkisi olup olmadığının sonucu</returns>
        private static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            return
                kullanan.YetkiTipiDegeriIceriyorMu("Stratejik Plan Yetkisi", "2", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Stratejik Plan Yetkisi", "4", false);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının stratejik plan
        /// bilgilerini onaylama yetkisinin olup olmadığını kontrol eden yordam
        /// Kullanıcının stratejik plan bilgilerini onaylama yetkisi varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">İstekte bulunan kullanıcı</param>
        /// <returns>Onaylama yetkisi olup olmadığının sonucu</returns>
        private static bool OnaylamaYetkisiVarMi(Kullanici kullanan)
        {
            return
                kullanan.YetkiTipiDegeriIceriyorMu("Stratejik Plan Yetkisi", "3", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Stratejik Plan Yetkisi", "4", false);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının stratejik plan
        /// bilgilerini değiştirme hakkının olup olmadığını kontrol eden yordam
        /// Kullanıcının stratejik plan bilgilerini değiştirme hakkı varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <returns>Yazma hakkı olup olmadığının sonucu</returns>
        public static Sonuc YazmaHakkiVarMi(Kullanici kullanan)
        {
            Sonuc donusDeger = new Sonuc(false, StratejikPlanResI.STR001, "");

            if (YoneticiMi(kullanan) || YazmaYetkisiVarMi(kullanan))
            {
                donusDeger.islemSonuc = true;
                donusDeger.hataStr = "";
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının stratejik plan sözlüklerini
        /// tanımlamaya hakkı var mı yok mu kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Sözlük kaydedecek kullanıcı</param>
        /// <returns>Yetkili olup olmadığının sonucu</returns>
        public static Sonuc TanimlamaYazmaHakkiVarMi(Kullanici kullanan)
        {
            Sonuc donusDeger = new Sonuc(false, StratejikPlanResI.STR001, "");

            if (YoneticiMi(kullanan))
            {
                donusDeger.islemSonuc = true;
                donusDeger.hataStr = "";
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının stratejik plan
        /// bilgilerini okuma hakkının olup olmadığını kontrol eden yordam
        /// Kullanıcının stratejik plan bilgilerini okuma hakkı varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">İstekte bulunan kullanıcı</param>
        /// <returns>Okuma hakkı olup olmadığının sonucu</returns>
        public static Sonuc OkumaHakkiVarMi(Kullanici kullanan)
        {
            Sonuc donusDeger = new Sonuc(false, StratejikPlanResI.STR002, "");

            if (YoneticiMi(kullanan) || GormeYetkisiVarMi(kullanan))
            {
                donusDeger.islemSonuc = true;
                donusDeger.hataStr = "";
            }

            return donusDeger;
        }
    }

    /// <summary>
    /// Stratejik plan modülü sunucu sınıfı tarafından sunulan fonksiyonları belirten arayüz sınıfı
    /// </summary>
    public interface ISTRServis
    {
        /// <summary>
        /// Dil varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Dil nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dil">Dil bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc DilKaydet(Kullanici kullanan, Dil dil);

        /// <summary>
        /// Verilen kriterlere uygun olan dil bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dil">Dil bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında dil listesi döndürülür.</returns>
        ObjectArray DilListele(Kullanici kullanan, Dil dil);

        /// <summary>
        /// Dil, stratejik planda kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Dil nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dil">Dil bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc DilSil(Kullanici kullanan, Dil dil);

        /// <summary>
        /// Misyon varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Misyon nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="misyon">Misyon bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc MisyonKaydet(Kullanici kullanan, Misyon misyon);

        /// <summary>
        /// Verilen kriterlere uygun olan misyon bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="misyon">Misyon bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında misyon nesnesi döndürülür.</returns>
        ObjectArray MisyonListele(Kullanici kullanan, Misyon misyon);

        /// <summary>
        /// Vizyon varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Vizyon nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="vizyon">Vizyon bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc VizyonKaydet(Kullanici kullanan, Vizyon vizyon);

        /// <summary>
        /// Verilen kriterlere uygun olan vizyon bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="vizyon">Vizyon bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında vizyon nesnesi döndürülür.</returns>
        ObjectArray VizyonListele(Kullanici kullanan, Vizyon vizyon);

        /// <summary>
        /// Tema varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Tema nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tema">Tema bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc TemaKaydet(Kullanici kullanan, Tema tema);

        /// <summary>
        /// Verilen kriterlere uygun olan tema bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tema">Tema bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tema listesi döndürülür.</returns>
        ObjectArray TemaListele(Kullanici kullanan, Tema tema);

        /// <summary>
        /// Verilen dönem başlangıç yılına ait tema özet/kısa bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tema listesi döndürülür.</returns>
        ObjectArray TemaKisaListele(Kullanici kullanan, int donemBYil);

        /// <summary>
        /// Tema, stratejik amaçta kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// Tema nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="temalar">Tema nesnelerini tutan liste</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc TemaSil(Kullanici kullanan, TNSCollection temalar);

        /// <summary>
        /// Stratejik amaç varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Stratejik Amaç nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="strAmac">Stratejik amaç bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc StratejikAmacKaydet(Kullanici kullanan, StratejikAmac strAmac);

        /// <summary>
        /// Verilen kriterlere uygun olan stratejik amaç bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="strAmac">Stratejik amaç bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik amaç listesi döndürülür.</returns>
        ObjectArray StratejikAmacListele(Kullanici kullanan, StratejikAmac strAmac);

        /// <summary>
        /// Verilen dönem başlangıç yılına ait stratejik amaç özet/kısa bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik amaç listesi döndürülür.</returns>
        ObjectArray StratejikAmacKisaListele(Kullanici kullanan, int donemBYil);

        /// <summary>
        /// Stratejik amaç, stratejik hedefte kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// Stratejik Amaç nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="amaclar">Stratejik Amaç nesnelerini tutan liste</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc StratejikAmacSil(Kullanici kullanan, TNSCollection amaclar);

        /// <summary>
        /// Stratejik hedef varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Stratejik Hedef nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="strHedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc StratejikHedefKaydet(Kullanici kullanan, StratejikHedef strHedef);

        /// <summary>
        /// Verilen kriterlere uygun olan stratejik hedef bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="strHedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik hedef listesi döndürülür.</returns>
        ObjectArray StratejikHedefListele(Kullanici kullanan, StratejikHedef strHedef);

        /// <summary>
        /// Verilen dönem başlangıç yılına ait stratejik hedef özet/kısa bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik hedef listesi döndürülür.</returns>
        ObjectArray StratejikHedefKisaListele(Kullanici kullanan, int donemBYil);

        /// <summary>
        /// Stratejik hedef, performans hedefte kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// Stratejik Hedef nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hedefler">Stratejik Hedef nesnelerini tutan liste</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc StratejikHedefSil(Kullanici kullanan, TNSCollection hedefler);
    }
}