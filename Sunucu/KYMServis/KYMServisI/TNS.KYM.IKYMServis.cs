using System;
using TNS;
using KYMServisI;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using TNS.DEG;

namespace TNS.KYM
{
    public enum ENUMIzleme : int
    {
        HATALIGIRIS = 1,
        BASARILIGIRIS = 2,
        PAROLADEGISIM = 3,
    }

    public enum ENUMZamanlayiciTur
    {
        HAFTALIK = 1,
        GUNLUK = 2,
        SAATLIK = 3,
        DAKIKALIK = 4,
    }

    public enum ENUMIslemTuru
    {
        TANIMSIZ = 0,
        KAYIT = 1,
        SILME = 2,
        GORME  = 3,
        GUNCELLEME = 4,
        ONAYLAMA = 5,
        ONAYKALDIRMA = 6
    }

    public enum ENUMLogoTuru
    {
        YAZILIM = 1,
        KURUM = 2,
        //MENU_LOGO = 3
    }

    public enum ENUMYetkiTarihceTur
    {
        KULLANICITIPI = 1,
        YETKITIPI = 2,
        YETKIISLEMTIPI = 3
        //MENU_LOGO = 3
    }

    /// <summary>
    /// Kullanıcı yetki değeri girerken, eğer listeden seçim işaretlenmiş ise, birden fazla seçimin yapılıp yapılmayacağını tutan değer listesi.
    /// </summary>
    public enum ENUMDegerSayisi : int
    {
        /// <summary>
        /// Listeden tek değer seçilsin.
        /// </summary>
        TEK = 1,

        /// <summary>
        /// Listeden birden fazla değer seçilebilsin.
        /// </summary>
        COK = 2,

        /// <summary>
        /// Seçim sayısı değeri tanımsız.
        /// </summary>
        TANIMSIZ = -1
    }

    /// <summary>
    /// Anket sorusu seçenekleri
    /// </summary>
    public enum ENUMSecenekTur
    {
        COKTANTEKSECMELI = 1,
        COKTANCOKSECMELI = 2,
        COKTANTEKSECMELIACIK = 3,
        COKTANCOKSECMELIACIK = 4,
        METINCEVAP = 5,
        COKTANTEKSECMELISECIMLIK = 6,
        COKTANCOKSECMELISECIMLIK = 7,
        TANIMSIZ = -1
    }

    /// <summary>
    /// Sistem tarafından tanınan kullanıcı tipleri. Menüler ve bazı işlemler kullanıcı tipine göre ayarlanır.
    /// </summary>
    public enum ENUMKullaniciTipi : int
    {
        /// <summary>
        /// Kullanıcı tipi tanımsız.
        /// </summary>
        TANIMSIZ = -1,

        /// <summary>
        /// Misafir kullanıcı tipi
        /// </summary>
        MISAFIR = 0,

        /// <summary>
        /// Sistem yöneticisi kullanıcı tipi.
        /// </summary>
        SISTEMYONETICISI = 1,

        /// <summary>
        /// Kullanıcı yöneticisi kullanıcı tipi.
        /// </summary>
        KULLANICIYONETICISI = 2,

        /// <summary>
        /// Personel uygulaması kullanıcı tipi.
        /// </summary>
        PERSONEL = 11,

        /// <summary>
        /// Kadro uygulaması kullanıcı tipi.
        /// </summary>
        KADRO = 12,

        /// <summary>
        /// İç kontrol uygulaması kullanıcı tipi.
        /// </summary>
        ICKONTROL = 13,

        /// <summary>
        /// Ön mali kontrol uygulaması kullanıcı tipi.
        /// </summary>
        ONMALIKONTROL = 14,

        /// <summary>
        /// Taşınır mal işlem fişi giren kullanıcı tipi.
        /// </summary>
        TASINIRMALKAYDI = 15,

        /// <summary>
        /// Kütüphane uygulaması kullanıcı tipi.
        /// </summary>
        KUTUPHANE = 16,

        /// <summary>
        /// Ödenek gönderebilen kullanıcı tipi.
        /// </summary>
        ODENEKGONDERME = 17,

        /// <summary>
        /// Ödeme emri mif kesebilen/görüntüleyebilen kullanıcı tipi.
        /// </summary>
        ODEMEEMRI_MIF = 18,

        /// <summary>
        /// MUhasebe birimi yöneticisi kullanıcı tipi.
        /// </summary>
        MUHASEBEYONETICISI = 19,

        /// <summary>
        /// Bütçe hazırlık modülü kullanıcı tipi.
        /// </summary>
        BUTCEHAZIRLIK = 20,

        /// <summary>
        /// Bütçe uygulama modülü kullanıcı tipi.
        /// </summary>
        BUTCEUYGULAMA = 21,

        /// <summary>
        /// DPT Yatırım projeleri uygulaması kullanıcı tipi.
        /// </summary>
        YATIRIMPROJELERI = 22,

        /// <summary>
        /// Alacak takip uygulaması kullanıcı tipi.
        /// </summary>
        ALACAKTAKIP = 23,

        /// <summary>
        /// Ekonomik gösterge uygulaması kullanıcı tipi.
        /// </summary>
        EKONOMIKGOSTERGE = 24,

        /// <summary>
        /// Kesin hesap uygulaması kullanıcı tipi.
        /// </summary>
        KESINHESAP = 25,

        /// <summary>
        /// Stratejik plan modülü kullanıcı tipi.
        /// </summary>
        STRATEJIKPLAN = 26,

        /// <summary>
        /// Performans bütçe modülü kullanıcı tipi.
        /// </summary>
        PERFORMANSBUTCE = 27,

        /// <summary>
        /// Taşınır mal yöneticisi kullanıcı tipi.
        /// </summary>
        TASINIRMALYONETICISI = 28,

        /// <summary>
        /// Taşınır mal konsolide yetkilisi kullanıcı tipi.
        /// </summary>
        TASINIRMALKONSOLIDEYETKILISI = 29,

        /// <summary>
        /// Performans bütçe tedbir girişi kullanıcı tipi.
        /// </summary>
        PERFORMANSBUTCETEDBIR = 30,

        /// <summary>
        /// Makbuz kullanıcısı kullanıcı tipi.
        /// </summary>
        MAKBUZKULLANICISI = 31,

        /// <summary>
        /// Bütçe yöneticisi kullanıcı tipi.
        /// </summary>
        BUTCEYONETICISI = 32,

        /// <summary>
        /// Kimlik paylaşımı kullanıcı tipi.
        /// </summary>
        KIMLIKPAYLASIMI = 33,

        /// <summary>
        /// Kalite kullanıcısı kullanıcı tipi.
        /// </summary>
        //KALITEKULLANICISI = 34,

        /// <summary>
        /// Proje yönetimi kullanıcı tipi.
        /// </summary>
        //PROJEYONETIMI = 35,

        /// <summary>
        /// Evrak kullanıcısı kullanıcı tipi.
        /// </summary>
        EVRAK = 36,

        /// <summary>
        /// Taşınır Mal Kullanıcı Birimi kullanıcı tipi
        /// </summary>
        TASINIRKULLANICIBIRIM = 38,

        /// <summary>
        /// Tarım bakanlığı Destek Başvuru işlemleri Kullanıcısı (Yonetici)
        /// </summary>
        DESTEKBASVURUYONETICI = 39,

        /// <summary>
        /// Tarım bakanlığı Destek Başvuru işlemleri Kullanıcısı (Uzman)
        /// </summary>
        DESTEKBASVURUUZMAN = 40,

        /// <summary>
        /// Tarım bakanlığı Destek Başvuru işlemleri Kullanıcısı
        /// </summary>
        DESTEKBASVURUTASRA = 41,
        //********Diğer destek kullanıcı tipleri aşağıda kısımda tanımlı*************

        /// <summary>
        /// Sınav işlemleri için
        /// ekleyen:ht
        /// </summary>
        SINAVISLEMLERI = 46,

        /// <summary>
        /// Tarım bakanlığı Görevlendirme Sistemi Kullanıcısı (Yonetici)
        /// </summary>
        GOREVLENDIRMEYONETICI = 47,

        /// <summary>
        /// Tarım bakanlığı Görevlendirme Kullanıcısı 
        /// </summary>
        GOREVLENDIRMEKULLANICI = 48,

        /// <summary>
        /// Taşınmaz işlemleri kullanıcısı
        /// </summary>
        GELIRYONETIMIKULLANICI = 49,

        /// <summary>
        /// Tarım bakanlığı Destek Başvuru sadece raporlama yapmak için
        /// </summary>
        DESTEKBASVURURAPORLAMA = 50,

        /// <summary>
        /// Tarım bakanlığı Destek Başvuru bakanlık merkez kullanıcısı
        /// </summary>
        DESTEKBASVURUMERKEZ = 51,
        //********Diğer destek kullanıcı tipleri aşağıda kısımda tanımlı*************

        /// <summary>
        /// Teminat mektubu ile ilgili
        /// </summary>
        TEMINATMEKTUBU = 52,

        /// <summary>
        /// Evrak belge yöneticisi ile ilgili
        /// </summary>
        EVRAKKURUMBELGEYONETICISI = 53,

        /// <summary>
        /// Evrak belge yöneticisi ile ilgili
        /// </summary>
        EVRAKBELGEYONETICISI = 54,

        /// <summary>
        /// Evrak Arşivci ile ilgili
        /// </summary>
        EVRAKARSIVCI = 55,

        /// <summary>
        /// Evrak Misafir ile ilgili
        /// </summary>
        EVRAKMISAFIR = 56,

        /// <summary>
        /// Teftiş Modülü
        /// </summary>
        TEFTISBASKAN = 57,

        /// <summary>
        /// Teftiş Modülü
        /// </summary>
        TEFTISBASKANYARDIMCISI = 58,

        /// <summary>
        /// Teftiş Modülü
        /// </summary>
        TEFTISMUFETTIS = 59,

        /// <summary>
        /// Teftiş Modülü
        /// </summary>
        TEFTISBURO = 60,

        /// <summary>
        /// Teftiş Modülü
        /// </summary>
        TEFTISKALITE = 61,

        /// <summary>
        /// Teftiş Modülü
        /// </summary>
        TEFTISYONETICI = 62,

        ///
        ///Kütüphane etiket basım yetkisi
        ///
        ETIKETYETKISI = 63,

        ///
        ///Harcama Süreç işlemleri yetkisi
        ///
        HARCAMAISLEMLERI = 64,

        ///
        ///Taşınmaz işlemleri yetkisi
        ///
        TASINMAZISLEMLERIYETKISI = 65,

        ///
        ///Taşınmaz işlemleri yetkisi
        ///
        TASINMAZYONETICIYETKISI = 66,

        /// <summary>
        /// Taşınır mal modülünde yıl devri giriş ve çıkışlara 
        /// müdahale etme yetkisidir
        /// </summary>
        TASINIR_MAL_YIL_DEVRI_MUDAHALE_YETKISI = 70,

        /// <summary>
        /// Evrak Sürec Tasarımcısı ile ilgili
        /// </summary>
        EVRAKSURECTASARIMCISI = 71,

        /// <summary>
        /// Tarım bakanlığı Destek Başvuru DEĞERLENDİRME KOMİSYONU BAŞKANI
        /// </summary>
        DESTEKBASVURUDEGERLENDIRMEBASKAN = 76,

        /// <summary>
        /// Tarım bakanlığı Destek Başvuru DEĞERLENDİRME VERİ GİRİŞ PERSONELİ
        /// </summary>
        DESTEKBASVURUDEGERLENDIRMEVERIGIRIS = 77,

        /// <summary>
        /// Tarım bakanlığı Destek Başvuru Web Servisinden bilgi çekmesi için tanımlanacak kullanıcı
        /// </summary>
        DESTEKWEBSERVISKULLANICISI = 78,

        KALITEDOKUMANTASYONKULLANICISI = 79,

        /// <summary>
        /// Performans bütçe yöneticisi kullanıcı tipi.
        /// </summary>
        PERFORMANSBUTCEYONETICISI = 82,

        EVRAKBAKIM = 83,

        EVRAKKEPGONDER = 84,
        EVRAKKEPAL = 85,

        GOREVYONETIMIKULLANICISI = 86,

        SATINALMAGOREVLISI_MANAS = 87,
        PERSONEL_YONETICISI = 88,
        MUHASEBE_BIRIM_YONETICISI = 89,

        /// <summary>
        /// Taahhüt Kartı ile ilgili
        /// </summary>
        TAAHHUTKARTI = 90,

        ODENEKTALEPKARSILAMA = 91,

        KURUMPROJELERI = 92,

        DUNYABANKASIPROJEYONETICISI = 93,
        DUNYABANKASIBIRIMKULLANICISI = 94,
        SAYISTAYBULGUYONETICISI = 95,
        SAYISTAYBULGUKULLANICISI = 96,
    }

    /// <summary>
    /// Kullanıcının yetki tipi verisi girilirken, listeden mi seçeceği, elle mi gireceği bilgisi.
    /// </summary>
    public enum ENUMGirisSekli : int
    {
        /// <summary>
        /// Yetki tipi değeri listeden seçilecek.
        /// </summary>
        LISTEDEN = 1,

        /// <summary>
        /// Yetki tipi değeri elle giriliecek. ; karakteri girilirse, çoklu değer girilmiş varsayılır.
        /// </summary>
        YAZIYLA = 2,

        /// <summary>
        /// Yetki tipi agaç yapısındaki bir görünümden seçilecek. Birden fazla seçim yapılırsa ; ayracı ile ayrılacak.
        /// </summary>
        AGACTAN = 3,

        /// <summary>
        /// Yetki tipi değeri girişi tanımsız.
        /// </summary>
        TANIMSIZ = -1
    }

    /// <summary>
    /// Yetki tipi değerinin girişi zorunlu mu? değil mi?
    /// </summary>
    public enum ENUMZorunluluk : int
    {
        /// <summary>
        /// Yetki tipi değeri girişi zorunlu.
        /// </summary>
        ZORUNLU = 1,

        /// <summary>
        /// Yetki tipi değeri girişi zorunlu değil.
        /// </summary>
        ZORUNLU_DEGIL = 0,

        /// <summary>
        /// Yetki tipi değeri girişi zorunluluğu tanımsız.
        /// </summary>
        TANIMSIZ = -1
    }

    /// <summary>
    /// Kullanıcı sistemi kullanabilir mi?
    /// </summary>
    public enum ENUMAktiflik : int
    {
        /// <summary>
        /// Aktif ise kullanıcı sistemi kullanabilir.
        /// </summary>
        AKTIF = 1,

        /// <summary>
        /// Pasif ise kullanıcı sistemi kullanamaz.
        /// </summary>
        PASIF = 0,

        /// <summary>
        /// Aktif pasiflik bilgisi tanımsız.
        /// </summary>
        TANIMSIZ = -1
    }

    /// <summary>
    /// Kullanıcı üzerinde yapılan işlemler listesi.
    /// </summary>
    public enum ENUMTarihceIslem : int
    {
        /// <summary>
        /// Kullanıcı yeni oluşturuldu.
        /// </summary>
        YENI = 1,

        /// <summary>
        /// Kullanıcı bilgileri değiştirildi.
        /// </summary>
        GUNCELLEME = 2,

        /// <summary>
        /// Kullanıcı aktif yapıldı.
        /// </summary>
        AKTIF = 3,

        /// <summary>
        /// Kullanıcı pasif yapıldı.
        /// </summary>
        PASIF = 4,

        /// <summary>
        /// Kullanıcının şifresi değiştirildi.
        /// </summary>
        SIFREATA = 5,

        BASARILIGIRIS = 6,
        BASARISIZGIRIS = 7,
    }

    /// <summary>
    /// Kullanıcının aramalarda, gösterilip gösterilmeyeceği ve Duyuruların ana sayfada gösterilip gösterilmeyeceği bilgisi .
    /// </summary>
    public enum ENUMGizle : int
    {
        /// <summary>
        /// Kullanıcı arama sonucu listelerinde görüntülensin / Duyuru ana sayfada görüntülensin.
        /// </summary>
        GORUNUR = 1,

        /// <summary>
        /// Kullanıcı arama sonucu listelerinde görüntülenmesin / Duyuru ana sayfada görüntülenmesin.
        /// </summary>
        GORUNMEZ = 2,

        /// <summary>
        /// Kullanıcı arama sonucu listelerinde görüntülenip görüntülenmeyeceği bilgisi tanımsız / Duyurunun ana sayfada görüntülenip görüntülenmeyeceği bilgisi tanımsız.
        /// </summary>
        TANIMSIZ = 0
    }

    /// <summary>
    /// Duyurunun pencerede gösterilip gösterilmeyeceği bilgisi.
    /// </summary>
    public enum ENUMPencerede : int
    {
        /// <summary>
        /// Duyuru pencerede gösterilsin.
        /// </summary>
        GORUNUR = 1,

        /// <summary>
        /// Duyuru pencerede gösterilmesin.
        /// </summary>
        GORUNMEZ = 2,

        /// <summary>
        /// Duyurunun pencerede gösterilip gösterilmeyeceği bilgisi tanımsız.
        /// </summary>
        TANIMSIZ = 0
    }

    public interface ITekNoktadanGiris
    {
        KullaniciVAN GirisYap(string kullaniciKod, string kullaniciParola, string[] ekBilgiler);
    }

    public class BilgiEkran
    {
        public string sayfaAdres;
        public int yukseklik;
        public string[] modulKodlar;
        public string resId;
        public bool ayarVar;
        public BilgiEkran(string sayfaAdres, int yukseklik, string[] modulKodlar, string resId, bool ayarVar)
        {
            this.ayarVar = ayarVar;
            this.sayfaAdres = sayfaAdres;
            this.yukseklik = yukseklik;
            this.modulKodlar = modulKodlar;
            this.resId = resId;
        }

        public bool AyarVarMi()
        {
            return ayarVar;
        }

        public bool KullaniliyorMu()
        {
            if (modulKodlar == null || modulKodlar.Length == 0)
            {
                return true;
            }
            for (int i = 0; i < modulKodlar.Length; i++)
            {
                if (TNS.KYM.Arac.ModulKullaniliyorMu(modulKodlar[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public string BaslikBul()
        {
            return KYM.KullaniciResI.ResourceManager.GetString(resId);
        }
    }

    public class BilgiEkranYerlesim
    {

        public int ekranIndex;
        public int ekranCol;
        public BilgiEkranYerlesim(int ekranIndex, int ekranCol)
        {

            this.ekranIndex = ekranIndex;
            this.ekranCol = ekranCol;
        }
    }
    public interface IBilgiEkran
    {
        string BilgiUret(Kullanici kullanan);
    }

    [Serializable()]
    public class ZamanlayiciTip
    {
        public string kod = "";
        public string islemAdi = "";
        public string tur = "";
        public TNSDateTime zaman = new TNSDateTime();
    }
    /// <summary>
    /// Kullanıcı modülünde olup da, diğer modüller tarafından da kullanılması gereken yordamları tutan sınıf.
    /// </summary>
    public static class Arac
    {
        public static string BirimAdOl(string saymanlikAd, string birimAd)
        {
            if (string.IsNullOrWhiteSpace(saymanlikAd)) return birimAd;
            if (string.IsNullOrWhiteSpace(birimAd)) return saymanlikAd;
            string IlkAd = SaymanlikAdTemizle(saymanlikAd);
            if (birimAd.IndexOf(IlkAd, StringComparison.CurrentCultureIgnoreCase) >= 0) return birimAd;
            else return IlkAd + " " + birimAd;
        }

        private static string[] saymanlikAdlar = { "Merkez Say. Müd", "Malmüdürlüğü", "Defterdarlık Muhasebe Müdürlüğü", "MAL MÜDÜRLÜĞÜ", "Muhasebe Birimi", "MERKEZ SAYMANLIĞI", "Saymanlık Müdürlüğü", "Saymanlık Müd.", "Say. Müd.", "Muh. Birimi", "Saymanlığı", "Defterdarlığı", "Defterdarlık Muhasebe Müd.", "Say. Müdürlüğü" };
        private static string SaymanlikAdTemizle(string saymanlikAd)
        {
            for (int i = 0; i < saymanlikAdlar.Length; i++)
            {
                int yer = saymanlikAd.IndexOf(saymanlikAdlar[i], StringComparison.CurrentCultureIgnoreCase);
                if (yer == 0) return "";
                else if (yer > 0)
                {
                    return saymanlikAd.Substring(0, yer).Trim();
                }
            }
            return saymanlikAd;
            //string[] BirimAdParcalar = saymanlikAd.Split(' ');
            //return BirimAdParcalar[0];
        }

        public static string ServerWebConfigOku(string degiskenAd, string defDeger)
        {
            return Tanimla().WebConfigOku(degiskenAd, defDeger);
        }

        /// <summary>
        /// Remove HTML tags from string using char array.
        /// </summary>
        public static string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }


        public static BilgiEkran[] bilgiEkranlar = {
                                                            new BilgiEkran( "../Personelv2/PersonelBilgisi.aspx?menuyok=1", 100, new string[] { "PER" },"BE001",false),
                                                            new BilgiEkran( "../Duyuru/Duyuru.aspx", 320, null,"BE002",false),
                                                            new BilgiEkran( "../Dashboard/Genel/SikKullanilan.ascx", 210, null,"BE003",false),
                                                            new BilgiEkran( "../Evrak/EvrakSayi.aspx?menuyok=1", 210,  new string[]{"EVR"},"BE004",false),
                                                            //new BilgiEkran( "../Dashboard/Genel/HavaDurumu.aspx?menuYok=1", 210, null,"BE005", true),
                                                            //new BilgiEkran( "../Dashboard/Genel/Anket.aspx?menuYok=1", 210, null,"BE007", false),
                                                            new BilgiEkran( "../HarcamaSurecleri/KurBilgi.aspx?menuYok=1", 210, new string[]{"MUH","HRC"},"BE006",false),
                                                            new BilgiEkran( "../KYM/MesajPanosuForm.aspx?menuYok=1", 210, new string[]{"MUH"},"BE008", false)
                                                   };
        public static string[] moduller = { "ORG", "BHM", "BUM", "MUH", "HRC", "TMM", "PER", "EVR", "KTP", "YAT", "ATM", "KHM", "PEB", "STRKur", "STRBir", "GEL", "KPS", "MVZ", "EKG", "TRM", "GRV", "SB", "TFT", "TMZ", "IKY", "KDS", "GIT", "DBP", "SBY" };
        public static BilgiEkranYerlesim[] ModulYerlesimBul(Kullanici kullanan)
        {
            return new BilgiEkranYerlesim[] {
                        new BilgiEkranYerlesim(0,0),
                        new BilgiEkranYerlesim(1,0),
                        new BilgiEkranYerlesim(2,1),
                        new BilgiEkranYerlesim(3,1),
                        new BilgiEkranYerlesim(4,2),
                        new BilgiEkranYerlesim(5,2)
                        };
        }

        public static string ModulAdBul(string modulKod)
        {
            string tut = TNS.KYM.KullaniciResI.ResourceManager.GetString("MDL_" + modulKod);
            if (string.IsNullOrEmpty(tut)) return modulKod;
            return tut;
        }

        public static bool ModulKullaniliyorMu_GRV()
        {
            return ModulKullaniliyorMu("GRV");
        }

        public static bool ModulKullaniliyorMu_STRKur()
        {
            return ModulKullaniliyorMu("STRKur");
        }

        public static bool ModulKullaniliyorMu_MUH()
        {
            return ModulKullaniliyorMu("MUH");
        }

        public static bool ModulKullaniliyorMu_TMM()
        {
            return ModulKullaniliyorMu("TMM");
        }

        public static bool ModulKullaniliyorMu_PER()
        {
            return ModulKullaniliyorMu("PER");
        }

        public static bool ModulKullaniliyorMu_IKY()
        {
            return ModulKullaniliyorMu("IKY");
        }

        public static bool ModulKullaniliyorMu_EVR()
        {
            return ModulKullaniliyorMu("EVR");
        }

        public static bool ModulKullaniliyorMu_MVZ()
        {
            return ModulKullaniliyorMu("MVZ");
        }

        public static bool ModulKullaniliyorMu_HRC()
        {
            return ModulKullaniliyorMu("HRC");
        }

        public static bool ModulKullaniliyorMu_KDS()
        {
            return ModulKullaniliyorMu("KDS");
        }

        public static bool ModulKullaniliyorMu_GIT()
        {
            return ModulKullaniliyorMu("GIT");
        }

        public static bool ModulKullaniliyorMu(string modulKod)
        {
            return TNS.DEG.Arac.DegiskenDegerBulCache(0, "/MODUL/" + modulKod) == "0" ? false : true;
        }

        public static KullaniciTipiModul[] kullaniciTipiModul =
        {
            new KullaniciTipiModul(ENUMKullaniciTipi.MISAFIR, null),
            new KullaniciTipiModul(ENUMKullaniciTipi.SISTEMYONETICISI, null),
            new KullaniciTipiModul(ENUMKullaniciTipi.KULLANICIYONETICISI, null),
            new KullaniciTipiModul(ENUMKullaniciTipi.PERSONEL, new string[]{"PER", "IKY"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.KADRO, new string[]{"PER", "KPS", "EVR"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.ICKONTROL, new string[]{"X"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.ONMALIKONTROL, new string[]{"X"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.TASINIRMALKAYDI, new string[]{"TMM"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.KUTUPHANE, new string[]{"KTP"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.ODENEKGONDERME, new string[]{"BUM"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.ODEMEEMRI_MIF, new string[]{"MUH", "HRC"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.MUHASEBEYONETICISI, new string[]{"MUH", "HRC"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.MUHASEBE_BIRIM_YONETICISI, new string[]{"MUH", "HRC"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.BUTCEHAZIRLIK, new string[]{"BHM"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.BUTCEUYGULAMA, new string[]{"BUM"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.YATIRIMPROJELERI, new string[]{"YAT"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.ALACAKTAKIP, new string[]{"ATM"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.EKONOMIKGOSTERGE, new string[]{"X"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.KESINHESAP, new string[]{"KHM"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.STRATEJIKPLAN, new string[]{"STRKur", "STRBir"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.PERFORMANSBUTCE, new string[]{"PEB"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.PERFORMANSBUTCEYONETICISI, new string[]{"PEB", "STRKur", "STRBir"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.TASINIRMALYONETICISI, new string[]{"TMM"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.TASINIRMALKONSOLIDEYETKILISI, new string[]{"TMM"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.PERFORMANSBUTCETEDBIR, new string[]{"PEB"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.MAKBUZKULLANICISI, new string[]{"MUH", "HRC"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.BUTCEYONETICISI, new string[]{"BHM", "BUM"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.KIMLIKPAYLASIMI, new string[]{"KPS"}),
            //new KullaniciTipiModul(ENUMKullaniciTipi.KALITEKULLANICISI, new string[]{"X"}),
            //new KullaniciTipiModul(ENUMKullaniciTipi.PROJEYONETIMI, new string[]{"X"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.EVRAK, new string[]{"EVR"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.TASINIRKULLANICIBIRIM, new string[]{"TMM"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.DESTEKBASVURUYONETICI, new string[]{"TRM"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.DESTEKBASVURUUZMAN, new string[]{"TRM"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.DESTEKBASVURUTASRA, new string[]{"TRM"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.DESTEKBASVURUMERKEZ, new string[]{"TRM"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.DESTEKBASVURURAPORLAMA, new string[]{"TRM"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.DESTEKBASVURUDEGERLENDIRMEBASKAN, new string[]{"TRM"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.DESTEKBASVURUDEGERLENDIRMEVERIGIRIS, new string[]{"TRM"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.DESTEKWEBSERVISKULLANICISI, new string[]{"TRM"}),

            new KullaniciTipiModul(ENUMKullaniciTipi.SINAVISLEMLERI, new string[]{"SB"}),

            new KullaniciTipiModul(ENUMKullaniciTipi.GOREVLENDIRMEYONETICI, new string[]{"GRV"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.GOREVLENDIRMEKULLANICI, new string[]{"GRV"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.GELIRYONETIMIKULLANICI, new string[]{"GEL"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.TEMINATMEKTUBU, new string[]{"HRC"}),

            new KullaniciTipiModul(ENUMKullaniciTipi.EVRAKKURUMBELGEYONETICISI, new string[]{"EVR"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.EVRAKBELGEYONETICISI, new string[]{"EVR"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.EVRAKARSIVCI, new string[]{"EVR"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.EVRAKMISAFIR, new string[]{"EVR"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.EVRAKSURECTASARIMCISI, new string[]{"EVR"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.EVRAKBAKIM, new string[]{"EVR"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.EVRAKKEPGONDER, new string[]{"EVR"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.EVRAKKEPAL, new string[]{"EVR"}),

            new KullaniciTipiModul(ENUMKullaniciTipi.TEFTISBASKAN, new string[]{"TFT"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.TEFTISBASKANYARDIMCISI, new string[]{"TFT"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.TEFTISMUFETTIS, new string[]{"TFT"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.TEFTISBURO, new string[]{"TFT"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.TEFTISKALITE, new string[]{"TFT"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.TEFTISYONETICI, new string[]{"TFT"}),

            new KullaniciTipiModul(ENUMKullaniciTipi.ETIKETYETKISI, new string[]{"KTP"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.HARCAMAISLEMLERI, new string[]{"HRC", "TJE"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.TASINMAZISLEMLERIYETKISI, new string[]{"TMZ"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.TASINMAZYONETICIYETKISI, new string[]{"TMZ"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.TASINIR_MAL_YIL_DEVRI_MUDAHALE_YETKISI, new string[]{"TMM"}),


            new KullaniciTipiModul(ENUMKullaniciTipi.KALITEDOKUMANTASYONKULLANICISI, new string[]{"KDS"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.GOREVYONETIMIKULLANICISI, new string[]{"GIT"}),

            new KullaniciTipiModul(ENUMKullaniciTipi.SATINALMAGOREVLISI_MANAS, new string[]{"HRC"}),

            new KullaniciTipiModul(ENUMKullaniciTipi.ODENEKTALEPKARSILAMA, new string[]{"BUM"}),

            new KullaniciTipiModul(ENUMKullaniciTipi.KURUMPROJELERI, new string[]{"YAT"}),

            new KullaniciTipiModul(ENUMKullaniciTipi.DUNYABANKASIPROJEYONETICISI, new string[]{"DBP"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.DUNYABANKASIBIRIMKULLANICISI, new string[]{"DBP"}),

            new KullaniciTipiModul(ENUMKullaniciTipi.SAYISTAYBULGUYONETICISI, new string[]{"SBY"}),
            new KullaniciTipiModul(ENUMKullaniciTipi.SAYISTAYBULGUKULLANICISI, new string[]{"SBY"}),

        };

        public static bool KTKullaniliyorMu(ENUMKullaniciTipi kt)
        {
            foreach (KullaniciTipiModul ktm in kullaniciTipiModul)
            {
                if (ktm.kullaniciTipi == kt)
                {
                    return ktm.KTKullaniliyorMu();
                }
            }
            return false;
        }

        /// <summary>
        /// Tüm modüller tarafından kullanılan yetki tipleri listesi. Bu listede her bir yetki tipi için yetki tipinin kodu, adı ve giriş şekli ile, eğer listeden giriş yapılacaksa, listenin alınacağı kütüphane ismi ismi ile kütüphanedeki sınıfı ismi tutulur.
        /// </summary>
        public static YetkiTipi[] yetkiTipiListesi =
        {
            new YetkiTipi(0, "Kurum", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, "KYMServisC", "TNS.KYMListe", "2"),
            new YetkiTipi(4, "Bütçe İşlem Yapacağı Birimler", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "3"),
            new YetkiTipi(5, "Bütçe İşlem Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "MUHServisC", "TNS.KYMListe", "6"),
            new YetkiTipi(6, "Ödenek Gönderme Hazırlayacağı Birimler", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "3", ENUMKullaniciTipi.ODENEKGONDERME),
            new YetkiTipi(7, "Ödenek Gönderme Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "KYMServisC", "TNS.KYMListe", "4", ENUMKullaniciTipi.ODENEKGONDERME),
            new YetkiTipi(8, "Kütüphane Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "KYMServisC", "TNS.KYMListe", "5", ENUMKullaniciTipi.KUTUPHANE),
            new YetkiTipi(9, "Yatırım Hazırlığı Yapacağı Birimler", ENUMDegerSayisi.TEK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU, "BKDServisC", "TNS.KYMListe", "3", ENUMKullaniciTipi.YATIRIMPROJELERI),
            new YetkiTipi(10, "Yatırım Hazırlığı Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "KYMServisC", "TNS.KYMListe", "4", ENUMKullaniciTipi.YATIRIMPROJELERI),
            new YetkiTipi(11, "Alacak Takibi Yapacağı Birimler", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU, "BKDServisC", "TNS.KYMListe", "3", ENUMKullaniciTipi.ALACAKTAKIP),
            new YetkiTipi(12, "Alacak Takibi Yapacağı Saymanlık-Birim", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.ALACAKTAKIP),
            new YetkiTipi(13, "Alacak Takibi Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "KYMServisC", "TNS.KYMListe", "4", ENUMKullaniciTipi.ALACAKTAKIP),
            new YetkiTipi(14, "Personel Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "KYMServisC", "TNS.KYMListe", "5", ENUMKullaniciTipi.PERSONEL),
            new YetkiTipi(15, "Kadro Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "KYMServisC", "TNS.KYMListe", "5", ENUMKullaniciTipi.KADRO),
            new YetkiTipi(16, "İç Kontrol Yapacağı Birimler", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU, "BKDServisC", "TNS.KYMListe", "3",ENUMKullaniciTipi.ICKONTROL),
            new YetkiTipi(17, "İç Kontrol Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "KYMServisC", "TNS.KYMListe", "4", ENUMKullaniciTipi.ICKONTROL),
            new YetkiTipi(18, "Ön Mali Kontrol Yapacağı Birimler", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU, "BKDServisC", "TNS.KYMListe", "3", ENUMKullaniciTipi.ONMALIKONTROL),
            new YetkiTipi(19, "Ön Mali Kontrol Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "KYMServisC", "TNS.KYMListe", "4", ENUMKullaniciTipi.ONMALIKONTROL),
            new YetkiTipi(20, "Ödeme Emri/MİF Hazırlayacağı Birimler", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "3", ENUMKullaniciTipi.ODEMEEMRI_MIF),
            new YetkiTipi(21, "Ödeme Emri/MİF Saymanlık-Birim Yetkisi", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.ODEMEEMRI_MIF, ENUMKullaniciTipi.HARCAMAISLEMLERI),
            new YetkiTipi(22, "Ödeme Emri/MİF Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "KYMServisC", "TNS.KYMListe", "6", ENUMKullaniciTipi.ODEMEEMRI_MIF),
            new YetkiTipi(23, "Personel Saymanlık-Birim Yetkisi", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.PERSONEL),
            new YetkiTipi(24, "Bütçe Uygulama Görme Yetki Şablonu", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "3", ENUMKullaniciTipi.BUTCEUYGULAMA),
            new YetkiTipi(25, "Bütçe Uygulama Göreceği Aşamalar", ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "7", ENUMKullaniciTipi.BUTCEUYGULAMA),
            new YetkiTipi(26, "Bütçe Uygulama Kayıt Yetki Şablonu", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "3", ENUMKullaniciTipi.BUTCEUYGULAMA),
            new YetkiTipi(27, "Bütçe Uygulama Kaydedeceği Aşamalar", ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "7", ENUMKullaniciTipi.BUTCEUYGULAMA),
            new YetkiTipi(28, "Bütçe Uygulama Onay Yetki Şablonu", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "3", ENUMKullaniciTipi.BUTCEUYGULAMA),
            new YetkiTipi(29, "Bütçe Uygulama Onaylayacağı Aşamalar", ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "7", ENUMKullaniciTipi.BUTCEUYGULAMA),
            new YetkiTipi(30, "Kesin Hesap Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "KYMServisC", "TNS.KYMListe", "4", ENUMKullaniciTipi.KESINHESAP),
            new YetkiTipi(31, "Kesin Hesap Yapacağı Birimler", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "3", ENUMKullaniciTipi.KESINHESAP),
            new YetkiTipi(32, "Taşınır Yetki Şablonu", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "KYMServisC", "TNS.KYMListe", "4", ENUMKullaniciTipi.TASINIRMALKAYDI),
            new YetkiTipi(33, "Taşınır Muhasebe Harcama Birim Yetkisi", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.TASINIRMALKAYDI),
            new YetkiTipi(34, "Ödeme Emri/MİF Ekonomik Kod", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.ODEMEEMRI_MIF),
            new YetkiTipi(35, "Ödenek Gönderme Ekonomik Kod", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "6", ENUMKullaniciTipi.ODENEKGONDERME),

            new YetkiTipi(36, "Bütçe Hazırlığı Görebileceği Birimler", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "3", ENUMKullaniciTipi.BUTCEHAZIRLIK),
            new YetkiTipi(37, "Bütçe Hazırlığı Kaydedebileceği Birimler", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "3", ENUMKullaniciTipi.BUTCEHAZIRLIK),
            new YetkiTipi(38, "Bütçe Hazırlığı Onaylayabileceği Birimler", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "3", ENUMKullaniciTipi.BUTCEHAZIRLIK),
            new YetkiTipi(39, "Bütçe Hazırlığı Görebileceği Aşamalar", ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "7", ENUMKullaniciTipi.BUTCEHAZIRLIK),
            new YetkiTipi(40, "Bütçe Hazırlığı Kaydedebileceği Aşamalar", ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "7", ENUMKullaniciTipi.BUTCEHAZIRLIK),
            new YetkiTipi(41, "Bütçe Hazırlığı Onaylayabileceği Aşamalar", ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "7", ENUMKullaniciTipi.BUTCEHAZIRLIK),

            new YetkiTipi(42, "Stratejik Plan Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "KYMServisC", "TNS.KYMListe", "4", ENUMKullaniciTipi.STRATEJIKPLAN),
            new YetkiTipi(43, "Performans Bütçe Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "KYMServisC", "TNS.KYMListe", "4", ENUMKullaniciTipi.PERFORMANSBUTCE, ENUMKullaniciTipi.PERFORMANSBUTCETEDBIR),
            new YetkiTipi(44, "Performans Bütçe Yapacağı Birimler", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU, "BKDServisC", "TNS.KYMListe", "3", ENUMKullaniciTipi.PERFORMANSBUTCE),
            new YetkiTipi(45, "Performans Bütçe Tedbir Yapacağı Birimler", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.PERFORMANSBUTCETEDBIR),
            new YetkiTipi(47, "Kimlik Paylaşım Saymanlık-Birim Yetkisi", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.KIMLIKPAYLASIMI),
            //new YetkiTipi(48, "Kalite Yönetimi Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU, "KYMServisC", "TNS.KYMListe", "4", ENUMKullaniciTipi.KALITEKULLANICISI),
            //new YetkiTipi(49, "Proje Yönetimi Yetkisi", ENUMDegerSayisi.COK, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU, "KYMServisC", "TNS.KYMListe", "4", ENUMKullaniciTipi.PROJEYONETIMI),
            new YetkiTipi(50, "Kütüphane Listesi", ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "KTPServisC", "TNS.KYMListe", "8", ENUMKullaniciTipi.KUTUPHANE),

            new YetkiTipi(51, "Evrak Yetki Şablonu", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "EVRServisC", "TNS.KYMListe", "9", ENUMKullaniciTipi.EVRAK),
            new YetkiTipi(52, "Yetkili Olduğu Evrak Birimleri", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU, "EVRServisC", "TNS.KYMListe", "10", ENUMKullaniciTipi.EVRAK, ENUMKullaniciTipi.EVRAKSURECTASARIMCISI),
            new YetkiTipi(53, "Evrak Onay Yetki Şablonu", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "EVRServisC", "TNS.KYMListe", "11", ENUMKullaniciTipi.EVRAK),
            new YetkiTipi(102, "Dosya Tasnif Planı", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "EVRServisC", "TNS.KYMListe", "12", ENUMKullaniciTipi.EVRAK),
            new YetkiTipi(103, "Gizlilik Derecesi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "EVRServisC", "TNS.KYMListe", "13", ENUMKullaniciTipi.EVRAK),

            new YetkiTipi(55, "Bütçe Hazırlığı Görebileceği Altfaaliyetler", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.BUTCEHAZIRLIK),
            new YetkiTipi(56, "Bütçe Hazırlığı Kaydedebileceği Altfaaliyetler", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.BUTCEHAZIRLIK),
            new YetkiTipi(57, "Bütçe Hazırlığı Onaylayabileceği Altfaaliyetler", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.BUTCEHAZIRLIK),

            new YetkiTipi(61, "Bütçe Hazırlığı Görebileceği Fonksiyonlar", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.BUTCEHAZIRLIK),
            new YetkiTipi(62, "Bütçe Hazırlığı Kaydedebileceği Fonksiyonlar", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.BUTCEHAZIRLIK),
            new YetkiTipi(63, "Bütçe Hazırlığı Onaylayabileceği Fonksiyonlar", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.BUTCEHAZIRLIK),

            new YetkiTipi(64, "Stratejik Plan Yapacağı Birimler", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU, "BKDServisC", "TNS.KYMListe", "3", ENUMKullaniciTipi.STRATEJIKPLAN),
            new YetkiTipi(65, "Kadro Yapacağı Kurum", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.KADRO),
            new YetkiTipi(66, "Taşınır Kullanıcı Birimi Yetkisi", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.TASINIRKULLANICIBIRIM),
            new YetkiTipi(67, "Stratejik Plan Kopru Kod", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.STRATEJIKPLAN),
            new YetkiTipi(68, "Destek Basvuru Yetkili Oldugu Iller", ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "TRMServisC", "TNS.KYMListe", "12", ENUMKullaniciTipi.DESTEKBASVURUTASRA,ENUMKullaniciTipi.DESTEKBASVURUDEGERLENDIRMEBASKAN,ENUMKullaniciTipi.DESTEKBASVURUDEGERLENDIRMEVERIGIRIS),
            new YetkiTipi(69, "Destek Basvuru Yetkilisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "TRMServisC", "TNS.KYMListe", "13", ENUMKullaniciTipi.DESTEKBASVURUTASRA,ENUMKullaniciTipi.DESTEKBASVURUDEGERLENDIRMEBASKAN,ENUMKullaniciTipi.DESTEKBASVURUDEGERLENDIRMEVERIGIRIS),
            new YetkiTipi(70, "Destek Basvuru Islem Yetkisi", ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "TRMServisC", "TNS.KYMListe", "14", ENUMKullaniciTipi.DESTEKBASVURUTASRA,ENUMKullaniciTipi.DESTEKBASVURUMERKEZ,ENUMKullaniciTipi.DESTEKBASVURURAPORLAMA,ENUMKullaniciTipi.DESTEKBASVURUUZMAN,ENUMKullaniciTipi.DESTEKBASVURUYONETICI,ENUMKullaniciTipi.DESTEKBASVURUDEGERLENDIRMEBASKAN,ENUMKullaniciTipi.DESTEKBASVURUDEGERLENDIRMEVERIGIRIS,ENUMKullaniciTipi.DESTEKWEBSERVISKULLANICISI),


            new YetkiTipi(73, "Görevlendirme Çalıştığı Birim", ENUMDegerSayisi.TEK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU, "GRVServisC", "TNS.KYMListe", "15", ENUMKullaniciTipi.GOREVLENDIRMEKULLANICI),
            new YetkiTipi(74, "Görevlendirme Faaliyet ve Projede Görebileceği Birimler", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "GRVServisC", "TNS.KYMListe", "15", ENUMKullaniciTipi.GOREVLENDIRMEKULLANICI),
            new YetkiTipi(75, "Görevlendirme Faaliyet ve Projede Kaydedebileceği Birimler", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "GRVServisC", "TNS.KYMListe", "15", ENUMKullaniciTipi.GOREVLENDIRMEKULLANICI),
            new YetkiTipi(76, "Görevlendirme Görev Atamada Görebileceği Birimler", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "GRVServisC", "TNS.KYMListe", "15", ENUMKullaniciTipi.GOREVLENDIRMEKULLANICI),
            new YetkiTipi(77, "Görevlendirme Görev Atamada Kaydedebileceği Birimler", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "GRVServisC", "TNS.KYMListe", "15", ENUMKullaniciTipi.GOREVLENDIRMEKULLANICI),
            new YetkiTipi(78, "Görev Yönetimi Menü Yetkisi", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.GOREVLENDIRMEKULLANICI),
            new YetkiTipi(79, "Personel Organizasyon Yetkisi", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "PERServisC", "TNS.KYMListe", "10", ENUMKullaniciTipi.PERSONEL),

            new YetkiTipi(80, "Yetkili Olduğu Evrak Birimleri (Belge Yöneticisi)", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU, "EVRServisC", "TNS.KYMListe", "10", ENUMKullaniciTipi.EVRAKBELGEYONETICISI),
            new YetkiTipi(81, "Yetkili Olduğu Evrak Birimleri (Arşivci)", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU, "EVRServisC", "TNS.KYMListe", "10", ENUMKullaniciTipi.EVRAKARSIVCI),
            new YetkiTipi(82, "Yetkili Olduğu Evrak Birimleri (Misafir)", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU, "EVRServisC", "TNS.KYMListe", "10", ENUMKullaniciTipi.EVRAKMISAFIR),
            new YetkiTipi(83, "Teminat Mektubu Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "KYMServisC", "TNS.KYMListe", "5", ENUMKullaniciTipi.TEMINATMEKTUBU),

            new YetkiTipi(84, "Yapabileceği İşlemler", ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "HRCServisC", "TNS.HRCListe", "1", ENUMKullaniciTipi.HARCAMAISLEMLERI),
            new YetkiTipi(85, "Görebileceği İşlemler", ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "HRCServisC", "TNS.HRCListe", "1", ENUMKullaniciTipi.HARCAMAISLEMLERI),
            new YetkiTipi(86, "Onaylayabileceği İşlemler", ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "HRCServisC", "TNS.HRCListe", "1", ENUMKullaniciTipi.HARCAMAISLEMLERI),

            new YetkiTipi(87, "Taşınmaz Yetki Şablonu", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "KYMServisC", "TNS.KYMListe", "7", ENUMKullaniciTipi.TASINMAZISLEMLERIYETKISI),
            new YetkiTipi(88, "Taşınmaz Bölge,İl,İlçe İşlem Yetkisi", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.TASINMAZISLEMLERIYETKISI),
            new YetkiTipi(89, "Kişisel İşler", ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "HRCServisC", "TNS.HRCListe", "2", ENUMKullaniciTipi.HARCAMAISLEMLERI),
            new YetkiTipi(90, "Yevmiye Veremez", ENUMDegerSayisi.TEK, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, "", "", "", ENUMKullaniciTipi.ODEMEEMRI_MIF),

            new YetkiTipi(97, "Ödenek Gönderme Faaliyet Kod",          ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "14", ENUMKullaniciTipi.ODENEKGONDERME),
            new YetkiTipi(98, "Banka Hareketleri Görür",               ENUMDegerSayisi.TEK, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, "", "", "", ENUMKullaniciTipi.ODEMEEMRI_MIF),
            new YetkiTipi(99, "Ödeme Emri/MİF Faaliyet Kod",           ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "14", ENUMKullaniciTipi.ODEMEEMRI_MIF),
            new YetkiTipi(101, "Personel Kullanıcı Yönetir",           ENUMDegerSayisi.TEK, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, "", "", "", ENUMKullaniciTipi.PERSONEL),

            new YetkiTipi(104, "Banka Hareketleri Göreceği Hesaplar",  ENUMDegerSayisi.TEK, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, "", "", "", ENUMKullaniciTipi.ODEMEEMRI_MIF),
            new YetkiTipi(105, "İzin-Devir Bilgisi Girebilir",         ENUMDegerSayisi.TEK, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, "", "", "", ENUMKullaniciTipi.PERSONEL),

            new YetkiTipi(106, "Kalite Sistemi Rolü",                  ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "KYMServisC", "TNS.KYMListe", "81", ENUMKullaniciTipi.KALITEDOKUMANTASYONKULLANICISI),
            new YetkiTipi(107, "Kalite Sistemi Birimi",                ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "KYMServisC", "TNS.KYMListe", "82", ENUMKullaniciTipi.KALITEDOKUMANTASYONKULLANICISI),

            new YetkiTipi(108, "Veznedar",                             ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "MUHServisC", "TNS.KYMListe", "7", ENUMKullaniciTipi.MAKBUZKULLANICISI),

            new YetkiTipi(109, "Yetkili Olduğu Evrak Birimleri (Bakım)", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU, "EVRServisC", "TNS.KYMListe", "10", ENUMKullaniciTipi.EVRAKBAKIM),

            new YetkiTipi(110, "Yetkili Olduğu Evrak Birimleri (Kep Alma)", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU, "EVRServisC", "TNS.KYMListe", "10", ENUMKullaniciTipi.EVRAKKEPAL),
            new YetkiTipi(111, "Yetkili Olduğu Evrak Birimleri (Kep Gönderme)", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU, "EVRServisC", "TNS.KYMListe", "10", ENUMKullaniciTipi.EVRAKKEPGONDER),

            new YetkiTipi(112, "Görev Yönetimi Sistemi Rolü",      ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "KYMServisC", "TNS.KYMListe", "91", ENUMKullaniciTipi.GOREVYONETIMIKULLANICISI),
            new YetkiTipi(113, "Görev Yönetimi Sistemi Birimi",    ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU, "KYMServisC", "TNS.KYMListe", "92", ENUMKullaniciTipi.GOREVYONETIMIKULLANICISI),

            new YetkiTipi(114, "OkuIs",                            ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "IKYServisC", "TNS.IKYYetkiTipiListe", "1", ENUMKullaniciTipi.PERSONEL),
            new YetkiTipi(115, "YazIs",                            ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "IKYServisC", "TNS.IKYYetkiTipiListe", "1", ENUMKullaniciTipi.PERSONEL),
            new YetkiTipi(116, "OnayIs",                           ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "IKYServisC", "TNS.IKYYetkiTipiListe", "1", ENUMKullaniciTipi.PERSONEL),
            new YetkiTipi(117, "117",                              ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "SEK", ENUMKullaniciTipi.TANIMSIZ),
            new YetkiTipi(118, "GORPERTIP",                        ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "IKYServisC", "TNS.IKYYetkiTipiListe", "2", ENUMKullaniciTipi.PERSONEL),
            new YetkiTipi(119, "YAZPERTIP",                        ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "IKYServisC", "TNS.IKYYetkiTipiListe", "2", ENUMKullaniciTipi.PERSONEL),
            new YetkiTipi(120, "ONAPERTIP",                        ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "IKYServisC", "TNS.IKYYetkiTipiListe", "2", ENUMKullaniciTipi.PERSONEL),

            new YetkiTipi(121, "Okuma İşlemleri",                  ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "EVRServisC", "TNS.EVRYetkiTipiListe", "1", ENUMKullaniciTipi.EVRAK),
            new YetkiTipi(122, "Yazma İşlemleri",                  ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "EVRServisC", "TNS.EVRYetkiTipiListe", "1", ENUMKullaniciTipi.EVRAK),
            new YetkiTipi(123, "Silme İşlemleri",                  ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "EVRServisC", "TNS.EVRYetkiTipiListe", "1", ENUMKullaniciTipi.EVRAK),

            new YetkiTipi(124, "Giriş Sayfası (Masaüstü)",         ENUMDegerSayisi.TEK, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, "EVRServisC", "TNS.EVRYetkiTipiListe", "1"),
            new YetkiTipi(125, "Giriş Sayfası (Tablet)",           ENUMDegerSayisi.TEK, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, "", "", ""),

            new YetkiTipi(126, "Yapabileceği Adımlar",             ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "HRCServisC", "TNS.HRCListe", "3", ENUMKullaniciTipi.HARCAMAISLEMLERI),
            new YetkiTipi(127, "Görebileceği Adımlar",             ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "HRCServisC", "TNS.HRCListe", "3", ENUMKullaniciTipi.HARCAMAISLEMLERI),
            new YetkiTipi(128, "Onaylayabileceği Adımlar",         ENUMDegerSayisi.COK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "HRCServisC", "TNS.HRCListe", "3", ENUMKullaniciTipi.HARCAMAISLEMLERI),
            new YetkiTipi(129, "Personel Kullanıcı Görev Listesi", ENUMDegerSayisi.TEK, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, "", "", "", ENUMKullaniciTipi.PERSONEL),
            new YetkiTipi(130, "Taşınır Hesap Kodu Değiştirme Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.TASINIRMALKAYDI),
            new YetkiTipi(131, "Başvuruyu Online Durumuna Getir",  ENUMDegerSayisi.TEK, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, "", "", "", ENUMKullaniciTipi.DESTEKBASVURUTASRA),

            new YetkiTipi(132, "Proje Kaydedebileceği Birimler",   ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.KURUMPROJELERI),
            new YetkiTipi(133, "Proje Görebileceği Birimler",      ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.KURUMPROJELERI),
            new YetkiTipi(134, "Proje Onaylayabileceği Birimler",  ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.KURUMPROJELERI),
            new YetkiTipi(135, "Başvuru Üye İşlemi Yapabilir",     ENUMDegerSayisi.TEK, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, "", "", "", ENUMKullaniciTipi.DESTEKBASVURUTASRA),
            new YetkiTipi(136, "Destek Başvuru Şube Müdürü",       ENUMDegerSayisi.TEK, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, "", "", "", ENUMKullaniciTipi.DESTEKBASVURUTASRA),
            new YetkiTipi(137, "Merkez Kabul Yatirim",             ENUMDegerSayisi.TEK, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, "", "", "", ENUMKullaniciTipi.DESTEKBASVURUMERKEZ),
            new YetkiTipi(138, "Merkez Kabul Altyapi",             ENUMDegerSayisi.TEK, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, "", "", "", ENUMKullaniciTipi.DESTEKBASVURUMERKEZ),

            new YetkiTipi(139, "Dünya Bankası Proje Birimi", ENUMDegerSayisi.COK, ENUMGirisSekli.AGACTAN, ENUMZorunluluk.ZORUNLU_DEGIL, "BKDServisC", "TNS.KYMListe", "3", ENUMKullaniciTipi.DUNYABANKASIBIRIMKULLANICISI),
            new YetkiTipi(140, "Dünya Bankası İşlem Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "KYMServisC", "TNS.KYMListe", "4", ENUMKullaniciTipi.DUNYABANKASIBIRIMKULLANICISI),

            new YetkiTipi(141, "Sayıştay Bulgu Saymanlık-Birim Yetkisi", ENUMDegerSayisi.TANIMSIZ, ENUMGirisSekli.YAZIYLA, ENUMZorunluluk.ZORUNLU_DEGIL, string.Empty, string.Empty, string.Empty, ENUMKullaniciTipi.SAYISTAYBULGUKULLANICISI),
            new YetkiTipi(142, "Sayıştay Bulgu İşlem Yetkisi", ENUMDegerSayisi.TEK, ENUMGirisSekli.LISTEDEN, ENUMZorunluluk.ZORUNLU_DEGIL, "KYMServisC", "TNS.KYMListe", "4", ENUMKullaniciTipi.SAYISTAYBULGUKULLANICISI),
        };

        /// <summary>
        /// Sistemde tanımlı tüm kullanıcı tiplerinin, listesini bulan ve döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <returns>Sistemde tanımlı tüm kullanıcı tipleri döndürülür.</returns>
        public static ENUMKullaniciTipi[] KTListesi(Kullanici kullanan)
        {
            Array a = System.Enum.GetValues(typeof(ENUMKullaniciTipi));
            ENUMKullaniciTipi[] kullaniciTipiListesi = new ENUMKullaniciTipi[a.Length];
            int i = 0;
            foreach (ENUMKullaniciTipi e in a)
            {
                kullaniciTipiListesi[i++] = e;
            }
            return kullaniciTipiListesi;
        }

        /// <summary>
        /// Verilen kullanıcı tipinin, açıklamasını döndüren yordam.
        /// </summary>
        /// <param name="kullaniciTipi">Açıklaması döndürülecek kullanıcı tipi.</param>
        /// <returns>Verilen kullanıcı tipinin açıklaması döndürülür.</returns>
        public static string KullaniciTipiAdi(ENUMKullaniciTipi kullaniciTipi)
        {
            return TNS.KYM.KullaniciResI.ResourceManager.GetString("KYMKT_" + KullaniciTipiKodu(kullaniciTipi).ToString().Replace("-", "_"));
        }

        /// <summary>
        /// Verilen kullanıcı tipinin, sıra numarasını döndüren yordam.
        /// </summary>
        /// <param name="kullaniciTipi">Sıra numarası döndürülecek kullanıcı tipi.</param>
        /// <returns>Verilen kullanıcı tipinin sıra nu döndürülür.</returns>
        public static int KullaniciTipiKodu(ENUMKullaniciTipi kullaniciTipi)
        {
            return (int)kullaniciTipi;
        }

        /// <summary>
        /// Kullanıcı modülünün sunucu bölümüne remoting ile bağlanan ve kullanıcı arayüzü olarak bağlantıyı döndüren yordam.
        /// </summary>
        /// <returns>Kullanıcı modülünün sunucu bağlantısı döndürülür.</returns>
        public static IKYMServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("KYMServisURL");
            IKYMServis servis = TNS.Api.ServisIstemci.Olustur<IKYMServis>(sUrl);
            return servis;
        }

        /// <summary>
        /// Kullanıcının kültür kodu kullanılarak, kod parçasını çalıştıran uygulamanın (thread) dilini değiştiren yordam.
        /// </summary>
        /// <param name="kullanan">Aktif kültürü thread'in aktif kültürü yapılacak kullanıcı nesnesi.</param>
        public static string KimlikBul(Kullanici kullanan)
        {
            if (kullanan != null)
                return kullanan.KimlikBul();
            return null;
        }

        /// <summary>
        /// Kullanıcının kültür kodu kullanılarak, kod parçasını çalıştıran uygulamanın (thread) dilini değiştiren yordam.
        /// </summary>
        /// <param name="kullanan">Aktif kültürü thread'in aktif kültürü yapılacak kullanıcı nesnesi.</param>
        public static void KulturDegistir(Kullanici kullanan)
        {
            if (kullanan != null)
                KulturDegistir(kullanan.aktifKultur);
        }

        /// <summary>
        /// kültür kodu kullanılarak, kod parçasını çalıştıran uygulamanın (thread) dilini değiştiren yordam.
        /// </summary>
        /// <param name="kultur">Thread'in aktif kültürü yapılacak dizgi.</param>
        public static void KulturDegistir(string kultur)
        {
            if (!string.IsNullOrEmpty(kultur))
            {
                //TNS.BHMv2.ButceHazirlikResI.Culture = new System.Globalization.CultureInfo("en");
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(kultur);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(kultur);
            }
        }

        /// <summary>
        /// Sistemde kayıtlı, tüm yetki tiplerini liste halinde döndüren yordam.
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>Sistemde kayıtlı, tüm yetki tiplerin kodunu ve adını liste halinde döndürür.</returns>
        public static YetkiTipi[] YTListesi(Kullanici kullanan)
        {
            YetkiTipiListesiGuncelle();
            return yetkiTipiListesi;
        }

        /// <summary>
        /// Parametre olarak geçirilen yetki tipi kodunu kullanarak, 
        /// yetki tipi bilgisini bulan ve döndüren yordam.
        /// </summary>
        /// <param name="kod">Bilgileri bulunacak yetki tipinin kodu.</param>
        /// <returns>Kodu verilen yetki tipinin bilgileri döndürülür.</returns>
        public static YetkiTipi YetkiTipiBul(int kod)
        {
            foreach (YetkiTipi yt in TNS.KYM.Arac.yetkiTipiListesi)
            {
                if (yt.kod == kod) return yt;
            }
            throw new Exception(KullaniciResI.KYM001);
        }

        /// <summary>
        /// Parametre olarak geçirilen yetki tipi adını kullanarak, 
        /// yetki tipi bilgisini bulan ve döndüren yordam.
        /// </summary>
        /// <param name="ad">Bilgileri bulunacak yetki tipinin adı.</param>
        /// <returns>Adı verilen yetki tipinin bilgileri döndürülür.</returns>
        public static YetkiTipi YetkiTipiBul(string ad)
        {
            foreach (YetkiTipi yt in TNS.KYM.Arac.yetkiTipiListesi)
            {
                if (yt.ad == ad) return yt;
            }
            return null;
        }

        /// <summary>
        /// Verilen kullanıcı tipiyle ilişkilendirilmiş yetki tiplerinin dizisini döndüren yordam.
        /// </summary>
        /// <param name="kullaniciTipi">Kendisiyle ilgili yetki tipi tanımları döndürülecek kullanıcı tipi.</param>
        /// <returns>Verilen kullanıcı tipiyle ilişkilendirilmiş yetki tiplerinin dizisi döndürülür.</returns>
        public static YetkiTipi[] KullaniciTipiYetkiTipleriBul(ENUMKullaniciTipi kullaniciTipi)
        {
            YetkiTipiListesiGuncelle();
            ArrayList al = new ArrayList();

            foreach (YetkiTipi yt in TNS.KYM.Arac.yetkiTipiListesi)
            {
                if (yt.KullaniciTipiVarMi(kullaniciTipi)) al.Add(yt);
            }
            return (YetkiTipi[])al.ToArray(typeof(YetkiTipi));
        }

        private static void YetkiTipiListesiGuncelle()
        {
            //Tacikistan
            //string tut = degServis.DegiskenDegerBul(0, "/YetkiBirimElleGir");
            //if (!string.IsNullOrEmpty(tut) && tut != "0")
            //{
            //    //0. yetkini satır numarası 524'ü çıkart
            //    yetkiTipiListesi[525 - 524].girisSekli = ENUMGirisSekli.YAZIYLA;
            //    yetkiTipiListesi[527 - 524].girisSekli = ENUMGirisSekli.YAZIYLA;
            //    yetkiTipiListesi[530 - 524].girisSekli = ENUMGirisSekli.YAZIYLA;
            //    yetkiTipiListesi[532 - 524].girisSekli = ENUMGirisSekli.YAZIYLA;
            //    yetkiTipiListesi[537 - 524].girisSekli = ENUMGirisSekli.YAZIYLA;
            //    yetkiTipiListesi[539 - 524].girisSekli = ENUMGirisSekli.YAZIYLA;
            //    yetkiTipiListesi[541 - 524].girisSekli = ENUMGirisSekli.YAZIYLA;
            //    yetkiTipiListesi[545 - 524].girisSekli = ENUMGirisSekli.YAZIYLA;
            //    yetkiTipiListesi[547 - 524].girisSekli = ENUMGirisSekli.YAZIYLA;
            //    yetkiTipiListesi[549 - 524].girisSekli = ENUMGirisSekli.YAZIYLA;
            //    yetkiTipiListesi[552 - 524].girisSekli = ENUMGirisSekli.YAZIYLA;
            //    yetkiTipiListesi[557 - 524].girisSekli = ENUMGirisSekli.YAZIYLA;
            //    yetkiTipiListesi[558 - 524].girisSekli = ENUMGirisSekli.YAZIYLA;
            //    yetkiTipiListesi[559 - 524].girisSekli = ENUMGirisSekli.YAZIYLA;
            //    yetkiTipiListesi[565 - 524].girisSekli = ENUMGirisSekli.YAZIYLA;
            //    yetkiTipiListesi[583 - 524].girisSekli = ENUMGirisSekli.YAZIYLA;
            //}
        }

        /// <summary>
        /// Yetki tipi girişi sırasında kullanılacak liste elemanlarını verilen kriterlere göre bulup döndüren yordam.
        /// Bu yordam, yetki tipi ile ilişkilendirilmiş olan kütüphanedeki liste işlemlerini tutan sınıfı çağırır.
        /// </summary>
        /// <param name="kullanan">Listeyi talep eden kullanıcı, bazı listeler kullanıcılara göre değişebilir.</param>
        /// <param name="yetkiTipi">Kendisiyle ilişkilendirilmiş liste döndürülülecek olan yetki tipi.</param>
        /// <param name="yil">Döndürülecek listedeki elemanların yıl kıstası. (Bazı listelerde)</param>
        /// <param name="ekParam">Döndürülecek listede kullanılacak ek bilgi. Bu bilgi yetki tipinden, tipine farklılık gösterebilir. Yetki tipi listenin sınırlandırılması için ek bilgi isteiyorsa kullanılır.</param>
        /// <returns>Verilen kriterlere uyan liste elemenları döndürülür.</returns>
        public static ListeSatir[] YetkiTipiGirisListesiDoldur(Kullanici kullanan, YetkiTipi yetkiTipi, int yil, string ekParam)
        {
            ArrayList bilgiListesi = new ArrayList();

            IKYMServis servis = TNS.KYM.Arac.Tanimla();

            return servis.YetkiTipiGirisListesi(kullanan, yetkiTipi, yil, ekParam);
        }

        public static bool DestekKullaniciTipindenMi(int kullaniciTipiKodu)
        {
            if (kullaniciTipiKodu == (int)ENUMKullaniciTipi.DESTEKBASVURURAPORLAMA ||
                kullaniciTipiKodu == (int)ENUMKullaniciTipi.DESTEKBASVURUMERKEZ ||
                kullaniciTipiKodu == (int)ENUMKullaniciTipi.DESTEKBASVURUTASRA ||
                kullaniciTipiKodu == (int)ENUMKullaniciTipi.DESTEKBASVURUUZMAN ||
                kullaniciTipiKodu == (int)ENUMKullaniciTipi.DESTEKBASVURUDEGERLENDIRMEBASKAN ||
                kullaniciTipiKodu == (int)ENUMKullaniciTipi.DESTEKBASVURUDEGERLENDIRMEVERIGIRIS ||
                kullaniciTipiKodu == (int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Kullanıcı Yönetim Servisine bağlanarak, Veri tabanı, Kullanıcı adı ve Şifre bilgilerini alan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici tipinde sınıf</param>
        /// <param name="userID">Kullanıcı adı bilgisini tutan, String tipinde sınıf</param>
        /// <param name="password">Şifre bilgisini tutan, String tipinde sınıf</param>
        /// <param name="dataSource">Veri kaynağı bilgisini tutan, String tipinde sınıf</param>
        public static void VeriTabaniBaglantiBilgileriniAl(Kullanici kullanan, string baglantiSatiri, out string userID, out string password, out string dataSource, out string veriTabani)
        {
            userID = null;
            password = null;
            dataSource = null;
            veriTabani = null;
            TNS.KYM.IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();

            string[] degerler = servisKYM.VeriErisimDegeriVer(kullanan, baglantiSatiri).Split(';');

            for (int i = 0; i < degerler.Length; i++)
            {
                degerler[i] = degerler[i].Trim();

                if (degerler[i].ToUpper().StartsWith("DATA SOURCE="))
                {
                    int yer = degerler[i].IndexOf("=");
                    dataSource = degerler[i].Substring(yer + 1).Trim();
                }
                else if (degerler[i].ToUpper().IndexOf("USER") > -1)
                {
                    int yer = degerler[i].IndexOf("=");
                    userID = degerler[i].Substring(yer + 1).Trim();
                }
                else if (degerler[i].ToUpper().StartsWith("PASSWORD="))
                {
                    int yer = degerler[i].IndexOf("=");
                    password = degerler[i].Substring(yer + 1).Trim();
                }
                else if (degerler[i].ToUpper().IndexOf("CATALOG") > -1)
                {
                    int yer = degerler[i].IndexOf("=");
                    veriTabani = degerler[i].Substring(yer + 1).Trim();
                }
            }
        }

        public static string YoneticiBildirimDurumAdBul(ENUMYoneticiBildirimFormuDurum durum)
        {
            if (durum == ENUMYoneticiBildirimFormuDurum.Yeni) return "Yeni";
            else if (durum == ENUMYoneticiBildirimFormuDurum.Gonderildi) return "Gönderildi";
            else if (durum == ENUMYoneticiBildirimFormuDurum.Onaylandi) return "Onaylandı";
            else if (durum == ENUMYoneticiBildirimFormuDurum.Reddedildi) return "Reddedildi";
            else if (durum == ENUMYoneticiBildirimFormuDurum.IptalEdildi) return "İptal";
            else return "Tanimsiz";
        }

        public static string EvrakAlanBul(string evrakAlan, ObjectArray oa)
        {
            foreach (EvrakAktarimMeta item in oa.objeler)
            {
                if (item.alanKodu == evrakAlan)
                    return item.alanDeger;
            }
            return null;
        }
    }

    public class KullaniciTipiModul
    {
        public ENUMKullaniciTipi kullaniciTipi;
        string[] modulKodlari;
        public KullaniciTipiModul(ENUMKullaniciTipi kullaniciTipi, string[] modulKodlari)
        {
            this.kullaniciTipi = kullaniciTipi;
            this.modulKodlari = modulKodlari;
        }

        internal bool KTKullaniliyorMu()
        {
            if (modulKodlari == null || modulKodlari.Length == 0) return true;
            for (int i = 0; i < modulKodlari.Length; i++)
            {
                if (modulKodlari[i] == "X") return false;
                if (TNS.KYM.Arac.ModulKullaniliyorMu(modulKodlari[i])) return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Kullanıcı modülünde kullanılan yetki olup da, diğer modüller tarafından da kullanılması gereken yordamları tutan sınıf.
    /// </summary>
    public static class Yetki
    {
        /// <summary>
        /// Verilen dizginin sha1 yöntemi ile şifrelenmiş halini döndüren yordam.
        /// </summary>
        /// <param name="bilgi">sha1 yöntemi ile şifrelenecek dizgi.</param>
        /// <returns>Verilen dizginin sha1 yöntemi ile şifrelenmiş hali döndürülür.</returns>
        public static string Sifrele(string bilgi)
        {
            string sifrelenmis = "";

            byte[] bytes = Encoding.UTF8.GetBytes(bilgi);
            HashAlgorithm hash = new SHA1Managed();
            byte[] hashBytes = hash.ComputeHash(bytes);
            string hashValue = Convert.ToBase64String(hashBytes);
            byte[] pp = Convert.FromBase64String(hashValue);

            for (int i = 0; i < pp.Length; i++)
                sifrelenmis += Convert.ToInt32(pp[i]).ToString("X2");

            return sifrelenmis;

            //return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(bilgi, "sha1");
        }

        /// <summary>
        /// Kullanan parametresi ile bilgileri verilen kullanıcı, sistem yöneticisi ise, Sonuc yapısının islemSonuc alanı true, değilse false yapan yordam.
        /// </summary>
        /// <param name="kullanan">Sistem kullanısı olup olmadığı kontrol edilecek kullanıcının bilgilerini içeren nesne.</param>
        /// <returns>Sonuc yapısının islemSonuc alanında, kullanıcı sistem yöneticisi ise doğru, değilse yanlış döndürülür.</returns>
        public static Sonuc YoneticiMi(Kullanici kullanan)
        {
            Sonuc sonuc = new Sonuc(false, string.Empty, string.Empty);
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                sonuc.islemSonuc = true;
            sonuc.anahtar = kullanan.kullaniciKodu;

            return sonuc;
        }

        /// <summary>
        /// Kullanan parametresi ile bilgileri verilen kullanıcı, destek yöneticisi ise, Sonuc yapısının islemSonuc alanı true, değilse false yapan yordam.
        /// </summary>
        /// <param name="kullanan">Sistem kullanısı olup olmadığı kontrol edilecek kullanıcının bilgilerini içeren nesne.</param>
        /// <returns>Sonuc yapısının islemSonuc alanında, kullanıcı sistem yöneticisi ise doğru, değilse yanlış döndürülür.</returns>
        public static Sonuc DestekYoneticisiMi(Kullanici kullanan)
        {
            Sonuc sonuc = new Sonuc(false, string.Empty, string.Empty);
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI))
                sonuc.islemSonuc = true;
            sonuc.anahtar = kullanan.kullaniciKodu;

            return sonuc;
        }
        /// <summary>
        /// Sadece sistem yöneticileri ile, kullanıcı yöneticilerinin yapabileceği işlemleri kontrol etmek için kullanılacak yordam. Kullanıcı bu iki tipden birine sahipse doğru, değilse yanlış döndürülür.
        /// </summary>
        /// <param name="kullanan">Sistem yöneticisi veya kullanıcı yöneticisi olup olmadığı kontrol edilecek kullanıcı nesnesi.</param>
        /// <returns>Parametre olarak verilen kullanıcı, Sistem yöneticisi veya kullanıcı yöneticisi ise doğru, değilse yanlış döndürülür.</returns>
        public static bool SistemveyaKullaniciYoneticisiMi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI) || kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.PERSONEL))
            {
                return true;
            }
            else
                return false;
        }
    }

    /// <summary>
    /// Sistemde belirli kullanıcı tiplerine mail atmak için kullanılan sınıf.
    /// Bu sınıf iki amaçla kullanılmaktadır:
    ///    1- Duyuru postası: Bu durumda posta nesnesi, her email için tekrarlanmaktadır. 
    ///    2- Duyuru özeti: Bu durumda ise email adresleri olmadan, toplamGonderilen ve toplamBasarili alanları geçerli olur.
    ///    Not: Doğrusu email'lerin liste halinde tutulmasıdır. Fakat zaman içinde bu şekilde gelişmiştir. Düzeltilirse daha iyi olur.
    /// </summary>
    [Serializable()]
    public class Posta
    {
        /// <summary>
        /// Bir kullanıcı tipindeki kullanıcılara gönderilen her mesaj bir kayıt numarası alır. Mesajın kayıt numarası.
        /// </summary>
        public int mesajID = 0;

        /// <summary>
        /// Mesajın konu alanında görüntülenecek metin.
        /// </summary>
        public string konu = string.Empty;

        /// <summary>
        /// Mail olarak gönderilecek metin.
        /// </summary>
        public string mesaj = string.Empty;

        /// <summary>
        /// Mesajın gönderileceği kullanıcı tipi
        /// </summary>
        public ENUMKullaniciTipi ktKod = 0;

        /// <summary>
        /// Mesajı gönderen kişinin email adresi.
        /// </summary>
        public string gonderen = string.Empty;

        /// <summary>
        /// Mesajı gönderen kişinin kullanıcı bilgileri.
        /// </summary>
        public string kullaniciKodu = string.Empty;

        /// <summary>
        /// Mesajı alacak kişi. Not: bu alanın dizi olması daha olur.
        /// </summary>
        public string email = string.Empty;

        /// <summary>
        /// Mesajın hazırlandığı tarih.
        /// </summary>
        public TNSDateTime tarih = new TNSDateTime();

        /// <summary>
        /// Bu mesaj kaç kişiye gönderilecek bilgisi. Kullanıcı tipindeki aktif kullanıcıların sayısı.
        /// </summary>
        public int toplamGonderilen = 0;

        /// <summary>
        /// Bu mesaj kaç kişiye gönderildi.
        /// </summary>
        public int toplamBasarili = 0;

        /// <summary>
        /// Mesaj email adresindeki kişiye gönderilemedi ise, hata mesajı. Not: bu alanın dizi olması daha olur.
        /// </summary>
        public string hataMesaji = string.Empty;

        /// <summary>
        /// Mesaj email adresindeki kişiye gönderilip gönderilmediği. Not: bu alanın dizi olması daha olur.
        /// </summary>
        public int gonderildiMi = 0;
    }

    [Serializable()]
    public class Muhasebe
    {
        public string kod = string.Empty;
        public string ad = string.Empty;

    }

    [Serializable()]
    public class MuhasebeVAN
    {
        public Sonuc sonuc;
        public Muhasebe muhasebe;

        public MuhasebeVAN()
        {
            sonuc = new Sonuc();
            muhasebe = new Muhasebe();
        }
    }

    /// <summary>
    /// Yetki tipi listeleri, bu arayüzü gerçekleştiren sınıflardan alınacaktır.
    /// </summary>
    public interface YetkiTipiListesi
    {
        /// <summary>
        /// Yetki tipi listesi döndüren yordam. Bu arayüzü gerçekleştiren sınıflar bu yordamı yazarak, liste sağlarlar.
        /// </summary>
        /// <param name="kullanan">Listeyi isteyen kullanıcının bilgisi.</param>
        /// <param name="yil">Sonuc listedeki elemanların kıstas yılı.</param>
        /// <param name="ekParamCagri">Sonuc listedeki elemanlar için verilen ek kıstas bilgisi. Bu alanın biçimi ve anlamı yordamdan yordama değişir.</param>
        /// <param name="ekParamTanim">Bir yordam, birden fazla liste döndürebilir. Bu alan döndürülecek listenin bilgisini tutar. Yetki tipi tanımlanırken, statik olarak verilir.</param>
        /// <returns>Verilen kıstaslara uyan yetki listesi döndürülür.</returns>
        ListeSatir[] YetkiTipiListeElemanlari(Kullanici kullanan, int yil, string ekParamCagri, string ekParamTanim);
    }

    /// <summary>
    /// Yetki tipi listeleri, bu arayüzü gerçekleştiren sınıflardan alınacaktır.
    /// </summary>
    public interface IDuyuruListesi
    {
        /// <summary>
        /// Yetki tipi listesi döndüren yordam. Bu arayüzü gerçekleştiren sınıflar bu yordamı yazarak, liste sağlarlar.
        /// </summary>
        /// <param name="kullanan">Listeyi isteyen kullanıcının bilgisi.</param>
        /// <param name="yil">Sonuc listedeki elemanların kıstas yılı.</param>
        /// <param name="ekParamCagri">Sonuc listedeki elemanlar için verilen ek kıstas bilgisi. Bu alanın biçimi ve anlamı yordamdan yordama değişir.</param>
        /// <param name="ekParamTanim">Bir yordam, birden fazla liste döndürebilir. Bu alan döndürülecek listenin bilgisini tutar. Yetki tipi tanımlanırken, statik olarak verilir.</param>
        /// <returns>Verilen kıstaslara uyan yetki listesi döndürülür.</returns>
        ObjectArray DuyuruListesiAl(Kullanici kullanan, string ekParamTanim);
    }

    /// <summary>
    /// Yetki tipi listeleri, bu arayüzü gerçekleştiren sınıflardan alınacaktır.
    /// </summary>
    public interface YetkiTipiAgaci
    {
        /// <summary>
        /// Yetki tipi listesi döndüren yordam. Bu arayüzü gerçekleştiren sınıflar bu yordamı yazarak, liste sağlarlar.
        /// </summary>
        /// <param name="kullanan">Listeyi isteyen kullanıcının bilgisi.</param>
        /// <param name="yil">Sonuc listedeki elemanların kıstas yılı.</param>
        /// <param name="ustKod">Döndürülecek listenin ait olduğu dalın kodu.</param>
        /// <param name="ekParamCagri">Sonuc listedeki elemanlar için verilen ek kıstas bilgisi. Bu alanın biçimi ve anlamı yordamdan yordama değişir.</param>
        /// <param name="ekParamTanim">Bir yordam, birden fazla liste döndürebilir. Bu alan döndürülecek listenin bilgisini tutar. Yetki tipi tanımlanırken, statik olarak verilir.</param>
        /// <returns>Verilen kıstaslara uyan yetki listesi döndürülür.</returns>
        ListeSatir[] YetkiTipiAgacElemanlari(Kullanici kullanan, int yil, string ustKod, string ekParamCagri, string ekParamTanim);

        /// <summary>
        /// Verilen kodun adını bulup döndüren yordam.
        /// </summary>
        /// <param name="kullanan">Çağrıyı yapan kullanıcının bilgisi.</param>
        /// <param name="yil">Döndürülecek ad için kıstas yıl.</param>
        /// <param name="kod">Adı döndürülecek kod.</param>
        /// <param name="ekParamCagri">Döndürülecek ad için verilen ek kıstas bilgisi. Bu alanın biçimi ve anlamı yordamdan yordama değişir.</param>
        /// <param name="ekParamTanim">Bir yordam, birden fazla ad değeri döndürebilir. Bu alan döndürülecek listenin bilgisini tutar. Yetki tipi tanımlanırken, statik olarak verilir.</param>
        /// <returns></returns>
        string YetkiTipiKodAd(Kullanici kullanan, int yil, string kod, string ekParamCagri, string ekParamTanim);

    }

    /// <summary>
    /// Değer girilerek kullanıcının yapabileceği işlemleri kısıtlayan özellilere yetki tipi ismi verilmiştir.
    /// Yetki tiplerine değer girilmesi zorunlu olabildiği gibi, değer bir listeden de seçilebilir.
    /// Bu sınıf yetki tipi kimlik bilgileri ve veri giriş şeklini tutar.
    /// </summary>
    [Serializable()]
    public class YetkiTipi
    {
        /// <summary>
        /// Yetki tipinin kodu. Yetki tipi tanımlanırken, statik olarak verilir, değiştirilmemelidir.
        /// </summary>
        public int kod = -1;

        /// <summary>
        /// Yetki tipinin adı. Yetki tipi tanımlanırken, statik olarak verilir, değiştirilmemelidir.
        /// </summary>
        public string ad = string.Empty;

        /// <summary>
        /// Kullanıcının yetki tipi değeri girerken, yazıyla mı gireceği, listeden mi seçeceği bilgisi.
        /// </summary>
        public ENUMGirisSekli girisSekli = ENUMGirisSekli.TANIMSIZ;

        /// <summary>
        /// Eğer listeden seçilecekse, listeden birden fazla değer seçilip seçilemeyeceğini tutan değişken.
        /// </summary>
        public ENUMDegerSayisi degerSayisi = ENUMDegerSayisi.TANIMSIZ;

        /// <summary>
        /// Yetki tip kaydedilirken bu değer girilmesinin  zorunlu olup olmadığı bilgisi.
        /// </summary>
        public ENUMZorunluluk zorunluMu = ENUMZorunluluk.TANIMSIZ;

        /// <summary>
        /// Eğer yetki değerleri listeden seçilecekse, listeyi sağlayan ve YetkiTipiListesi arayüzünü gerçekleştiren sınıfın bulunduğu dll'in dosya ismi. (.dll olmadan)
        /// </summary>
        public string dllAd;

        /// <summary>
        /// Eğer yetki değerleri listeden seçilecekse, listeyi sağlayan ve YetkiTipiListesi arayüzünü gerçekleştiren sınıfın tam ismi. (Package isimleri dahil edilerek.)
        /// </summary>
        public string nesneAd;

        /// <summary>
        /// Listeyi sağlayan ve YetkiTipiListesi arayüzünü gerçekleştiren sınıfın, birden fazla liste döndürüyorsa, hangi listenin döndürüleceğini gösteren ek bilgi.
        /// </summary>
        public string ekParam;

        /// <summary>
        /// Bu yetki tipinin geçerli olduğu kullanıcı tipleri.
        /// </summary>
        ENUMKullaniciTipi[] kullaniciTipleri = null;

        /// <summary>
        /// Boş yetki tipi nesnesi oluşturan yordam.
        /// </summary>
        public YetkiTipi()
        {
        }

        /// <summary>
        /// Verilen parametrelerle yetki tipi nesnesi oluşturan yordam. Uygulama başlarken, tüm yetki tipleri oluşturulurken çağrılır. 
        /// </summary>
        /// <param name="pKod">Yetki tipinin kodu. Yetki tipi tanımlanırken, statik olarak verilir, değiştirilmemelidir.</param>
        /// <param name="pAd">Yetki tipinin adı. Yetki tipi tanımlanırken, statik olarak verilir, değiştirilmemelidir.</param>
        /// <param name="pDegerSayisi">Eğer listeden seçilecekse, listeden birden fazla değer seçilip seçilemeyeceğini tutan değişken.</param>
        /// <param name="pGirisSekli">Kullanıcının yetki tipi değeri girerken, yazıyla mı gireceği, listeden mi seçeceği bilgisi.</param>
        /// <param name="pZorunluMu">Yetki tip kaydedilirken bu değer girilmesinin  zorunlu olup olmadığı bilgisi.</param>
        /// <param name="pDLLAd">Eğer yetki değerleri listeden seçilecekse, listeyi sağlayan ve YetkiTipiListesi arayüzünü gerçekleştiren sınıfın bulunduğu dll'in dosya ismi. (.dll olmadan)</param>
        /// <param name="pNesneAd">Eğer yetki değerleri listeden seçilecekse, listeyi sağlayan ve YetkiTipiListesi arayüzünü gerçekleştiren sınıfın tam ismi. (Package isimleri dahil edilerek.)</param>
        /// <param name="pEkParam">Listeyi sağlayan ve YetkiTipiListesi arayüzünü gerçekleştiren sınıfın, birden fazla liste döndürüyorsa, hangi listenin döndürüleceğini gösteren ek bilgi.</param>
        /// <param name="pKullaniciTipleri">Bu yetki tipinin geçerli olduğu kullanıcı tipleri parametrik dizisi.</param>
        public YetkiTipi(int pKod, string pAd, ENUMDegerSayisi pDegerSayisi, ENUMGirisSekli pGirisSekli, ENUMZorunluluk pZorunluMu, string pDLLAd, string pNesneAd, string pEkParam, params ENUMKullaniciTipi[] pKullaniciTipleri)
        {
            kod = pKod;
            ad = pAd;
            degerSayisi = pDegerSayisi;
            girisSekli = pGirisSekli;
            zorunluMu = pZorunluMu;
            ekParam = pEkParam;

            nesneAd = pNesneAd;
            dllAd = pDLLAd;
            ekParam = pEkParam;
            kullaniciTipleri = pKullaniciTipleri;
        }

        /// <summary>
        /// Verilen kullanıcı tipinin, yetki tipleri ile ilişkilendirilmiş kullanıcı tiplerinden birisi olup olmadığını bulur.
        /// </summary>
        /// <param name="kullaniciTipi">Yetki tipi ile ilişkili olduğu aranacak kullanıcı tipi.</param>
        /// <returns>Verilen kullanıcı tipi, yetki tipi ile ilişkili ise, doğru değilse yanlış döndürülür.</returns>
        internal bool KullaniciTipiVarMi(ENUMKullaniciTipi kullaniciTipi)
        {
            if (kullaniciTipleri == null) return false;
            foreach (ENUMKullaniciTipi e in kullaniciTipleri)
                if (e == kullaniciTipi) return true;
            return false;
        }

        /// <summary>
        /// Veri giriş ekranlarında bu yetki tipinin kullanıcılara etiketi gösterilirken kullanılacak adını bulup döndüren yordam.
        /// </summary>
        /// <returns>Yetki tipinin etiketi olarak kullanılacak adı döndürülür.</returns>
        public string GosterilenAd()
        {
            return KullaniciResI.ResourceManager.GetString("KYMYT_" + kod.ToString());
        }
    }

    /// <summary>
    /// Kullanıcının bir yetki tipi için girilmiş değerini tutan sınıf.
    /// </summary>
    [Serializable()]
    public class YetkiTipiDegeri
    {
        /// <summary>
        /// Kullanıcı için değeri tutulan yetki tipi nesnesi
        /// </summary>
        public YetkiTipi yetkiTipi = new YetkiTipi();

        /// <summary>
        /// Yetki tipi nesnesinin değeri.
        /// </summary>
        public string deger = string.Empty;
    }

    /// <summary>
    /// Sisteme kayıtlı ve sistemi kullanabilecek kişilere ilişkin bilgileri tutan sınıf.
    /// </summary>
    [Serializable()]
    public class Kullanici
    {
        /// <summary>
        /// Kullanıcının sisteme giriş için kullandığı kod.
        /// </summary>
        public string kullaniciKodu = string.Empty;

        /// <summary>
        /// Kullanıcının sisteme giriş için kullandığı parola. Şifrelenmiş şekilde tutulur.
        /// </summary>
        public string parola = string.Empty;

        /// <summary>
        /// Kullanıcının TC Kimlik numarası.
        /// </summary>
        public string mernis = string.Empty;

        /// <summary>
        /// Kullanıcının çalıştığı kurum. 0 numaralı yetki tipi listesinden gelen kod tutulur.
        /// </summary>
        public string calistigiKurum = string.Empty;

        /// <summary>
        /// Kullanıcının adı.
        /// </summary>
        public string ad = string.Empty;

        /// <summary>
        /// Kullanıcının soyadı.
        /// </summary>
        public string soyad = string.Empty;

        /// <summary>
        /// Kullanıcının doğum tarihi.
        /// </summary>
        public TNSDateTime dogumTarihi = new TNSDateTime();

        /// <summary>
        /// Kullanıcının elektronik posta adresi.
        /// </summary>
        public string emailAdresi = string.Empty;

        /// <summary>
        /// Kullanıcının telefon numarası.
        /// </summary>
        public string telefon = string.Empty;

        /// <summary>
        /// Kullanıcının sisteme son giriş tarihi, giriş esnasında okunur ve değiştirilir.
        /// </summary>
        public TNSDateTime sonGirisTarihi = new TNSDateTime();

        /// <summary>
        /// Kullanıcının ilişkilendirildiği kullanıcı tipleri.
        /// </summary>
        public ENUMKullaniciTipi[] kullaniciTipi = null;

        /// <summary>
        /// Kullanıcının yetki tipleri için girilmiş değerler. Bu değerlerle bazı işlemler kısıtlanır yada izin verilir. Hash table yapısının anahtarı ve değeri stringdir.
        /// </summary>
        public HashTableArrayList yetkiTipiDegerleri = new HashTableArrayList();

        /// <summary>
        /// Kullanıcı aktif mi pasif mi? Pasif kullanıcılar sisteme giriş yapamaz. Pasif kullanıcılar tüm kullanıcı yöneticileri tarafından görüntülenir ve bilgileri değiştirilebilir.
        /// </summary>
        public ENUMAktiflik aktifMi = ENUMAktiflik.TANIMSIZ;

        /// <summary>
        /// Kullanıcının açıklama olarak girilen, kurum içindeki yeri.
        /// </summary>
        public string calistigiBirim = string.Empty;

        /// <summary>
        /// Kullanıcının çalıştığı kurumdaki sicil numarası.
        /// </summary>
        public string sicilNo = string.Empty;

        /// <summary>
        /// Kullanıcının unvanı.
        /// </summary>
        public string unvan = string.Empty;

        /// <summary>
        /// Kullanıcının parolasının son kullanım tarihi. Bu tarihden sonra, kullanıcı yeni bir parola girmeye zorlanır.
        /// </summary>
        public TNSDateTime parolaBitisTarihi = new TNSDateTime();

        /// <summary>
        /// Kullanıcı sisteme giriş yaptığında, sistem tarafından verilen session numarası.
        /// </summary>
        public string sessionId = string.Empty;

        /// <summary>
        /// Sistem yöneticileri haricindeki kullanıcı tipindeki kullanıcılar tarafından yapılan aramalarda bu kullanıcıyı göster/gösterme seçeneği.
        /// </summary>
        public int aramadaGosterme = -1;

        /// <summary>
        /// Kullanıcının annesinin ismi.
        /// </summary>
        public string anaAdi = string.Empty;

        /// <summary>
        /// Kullanıcının babasının ismi.
        /// </summary>
        public string babaAdi = string.Empty;

        /// <summary>
        /// Kullanıcının cinsiyeti.
        /// </summary>
        public string cinsiyet = string.Empty;

        /// <summary>
        /// Kullanıcının cinsiyeti.
        /// </summary>
        public string sertNo = string.Empty;

        /// <summary>
        /// Kullanıcı sisteme girdiğinde seçmiş olduğu aktif dil kodu.
        /// </summary>
        public string aktifKultur = null;

        /// <summary>
        /// Kullanıcının akademik unvanı.
        /// </summary>
        public string akademikUnvan = string.Empty;

        /// <summary>
        /// Kullanıcının rumuzu.
        /// </summary>
        public string rumuz = string.Empty;
        public string ipAddress { get; set; }

        public string KullaniciKodu { get { return kullaniciKodu; } set { kullaniciKodu = value; } }
        public string TCKIMLIKNO { get { return mernis; } set { mernis = value; } }
        public string AD { get { return ad; } set { ad = value; } }
        public string SOYAD { get { return soyad; } set { soyad = value; } }
        public string UNVANAD { get { return unvan; } set { unvan = value; } }
        public string EPOSTA { get { return emailAdresi; } set { emailAdresi = value; } }
        public string ADSOYAD
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ad)) return soyad;
                if (string.IsNullOrWhiteSpace(soyad)) return ad;
                return ad + " " + soyad;
            }
        }
        public string Telefon { get { return telefon; } set { telefon = value; } }

        /// <summary>
        /// Verilen parametrelerle yeni kullanıcı nesnesi oluşturan yordam.
        /// </summary>
        /// <param name="pKullaniciKodu">Kullanıcının sisteme giriş için kullandığı kod.</param>
        /// <param name="pParola">Kullanıcının sisteme giriş için kullandığı parola. Şifrelenmiş şekilde tutulur.</param>
        /// <param name="pMernis">Kullanıcının TC Kimlik numarası.</param>
        /// <param name="pCalistigiKurum">Kullanıcının çalıştığı kurum. 0 numaralı yetki tipi listesinden gelen kod tutulur.</param>
        /// <param name="pAd">Kullanıcının adı.</param>
        /// <param name="pSoyad">Kullanıcının soyadı.</param>
        /// <param name="pDogumTarihi">Kullanıcının doğum tarihi.</param>
        /// <param name="pEmailAdresi">Kullanıcının elektronik posta adresi.</param>
        /// <param name="pTelefon">Kullanıcının telefon numarası.</param>
        /// <param name="pSonGirisTarihi">Kullanıcının sisteme son giriş tarihi, giriş esnasında okunur ve değiştirilir.</param>
        /// <param name="pAktifMi">Kullanıcı aktif mi pasif mi? Pasif kullanıcılar sisteme giriş yapamaz. Pasif kullanıcılar tüm kullanıcı yöneticileri tarafından görüntülenir ve bilgileri değiştirilebilir.</param>
        /// <param name="pCalistigiBirim">Kullanıcının açıklama olarak girilen, kurum içindeki yeri.</param>
        /// <param name="pSicilNo">Kullanıcının çalıştığı kurumdaki sicil numarası.</param>
        /// <param name="pUnvan">Kullanıcının ünvanı.</param>
        /// <param name="pParolaBitisTarihi">Kullanıcının parolasının son kullanım tarihi. Bu tarihden sonra, kullanıcı yeni bir parola girmeye zorlanır.</param>
        /// <param name="akademikUnvan">Kullanıcının akademik unvanı.</param>
        /// <param name="rumuz">Kullanıcının rumuzu. Kullanıcı sisteme giriş yaparken bu bilgiyi kullanabilir.</param>
        public Kullanici(
            string pKullaniciKodu,
            string pParola,
            string pMernis,
            string pCalistigiKurum,
            string pAd,
            string pSoyad,
            TNSDateTime pDogumTarihi,
            string pEmailAdresi,
            string pTelefon,
            TNSDateTime pSonGirisTarihi,
            ENUMAktiflik pAktifMi,
            string pCalistigiBirim,
            string pSicilNo,
            string pUnvan,
            TNSDateTime pParolaBitisTarihi, string akademikUnvan, string rumuz)
        {
            kullaniciKodu = pKullaniciKodu;
            parola = pParola;
            mernis = pMernis;
            calistigiKurum = pCalistigiKurum;
            ad = pAd;
            soyad = pSoyad;
            dogumTarihi = pDogumTarihi;
            emailAdresi = pEmailAdresi;
            telefon = pTelefon;
            sonGirisTarihi = pSonGirisTarihi;
            kullaniciTipi = null;
            aktifMi = pAktifMi;
            calistigiBirim = pCalistigiBirim;
            sicilNo = pSicilNo;
            unvan = pUnvan;
            parolaBitisTarihi = pParolaBitisTarihi;
            sessionId = string.Empty;
            this.akademikUnvan = akademikUnvan;
            this.rumuz = rumuz;
        }

        /// <summary>
        /// Boş kullanıcı nesnesi oluşturan yordam.
        /// </summary>
        public Kullanici()
        {
        }

        /// <summary>
        /// Kullanıcı verilen tiplerden birine sahipse doğru, değilse yanlış döndüren yordam.
        /// </summary>
        /// <param name="tipler">Kontrol edilecek kullanıcı tipleri.</param>
        /// <returns>Kullanıcı verilen kullanıcı tiplerine sahipse doğru, değilse yanlış döndürülür.</returns>
        public bool KullaniciTipiIceriyorMu(params int[] tipler)
        {
            if (this.kullaniciTipi == null) return false;
            for (int i = 0; i < this.kullaniciTipi.Length; i++)
            {
                for (int j = 0; j < tipler.Length; j++)
                {
                    if (this.kullaniciTipi[i] == (ENUMKullaniciTipi)tipler[j])
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Verilen değişken ve değerlerini veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kymServis">Kaydetme yordamının çağrılacağı sunucu nesnesi.</param>
        /// <param name="degiskenAd">Değerleri veri tabanına saklanacak değişken.</param>
        /// <param name="degerler">Değişken için verilen değerler.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc DegiskenleriSakla(IKYMServis kymServis, string degiskenAd, TNSCollection degerler)
        {
            return kymServis.DegiskenKaydet(kullaniciKodu, degiskenAd, degerler);
        }

        public bool DamgaDegerIceriyorMu(int damgaKod, string gelenDeger, bool yildizHepsi)
        {
            string kullaniciYetkiDeger = this.YetkiTipiDegeriBul(damgaKod);
            if (string.IsNullOrEmpty(kullaniciYetkiDeger))
            {
                if (yildizHepsi) return false;
                return true;
            }

            if (kullaniciYetkiDeger == "*") return true;

            string[] kullaniciYetkiDegerler = kullaniciYetkiDeger.Split(';', ',');
            gelenDeger = gelenDeger.Replace(".", string.Empty);
            string[] gelenDegerler = gelenDeger.Split(';', ',');

            foreach (string kullaniciDegerParca in kullaniciYetkiDegerler)
            {
                string kullaniciDegerParcaNoktasiz = kullaniciDegerParca.Replace(".", string.Empty);
                if (!string.IsNullOrEmpty(kullaniciDegerParcaNoktasiz))
                {
                    foreach (string gelenDegerParca in gelenDegerler)
                    {
                        if (gelenDegerParca.StartsWith(kullaniciDegerParcaNoktasiz))
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Kullanıcının verilen yetki tipindeki değerini döndüren yordam. Yetki tipi değeri bilgisine göre bazı işlemler yapılır yada yapılmaz.
        /// </summary>
        /// <param name="yetkiTipiNo">Değeri döndürülecek yetki tipinin numarası.</param>
        /// <returns>Kullanıcının verilen yetki tipindeki değeri döndürülür.</returns>
        public string YetkiTipiDegeriBul(int yetkiTipiNo)
        {
            for (int i = 0; i < Arac.yetkiTipiListesi.Length; i++)
            {
                if (Arac.yetkiTipiListesi[i].kod == yetkiTipiNo)
                    return YetkiTipiDegeriBul(Arac.yetkiTipiListesi[i].ad);
            }
            return string.Empty;
        }

        /// <summary>
        /// Kullanıcının verilen yetki tipindeki değerini döndüren yordam. Yetki tipi değeri bilgisine göre bazı işlemler yapılır yada yapılmaz.
        /// </summary>
        /// <param name="yetkiTipiAdi">Değeri döndürülecek yetki tipinin adı.</param>
        /// <returns>Kullanıcının verilen yetki tipindeki değeri döndürülür.</returns>
        public string YetkiTipiDegeriBul(string yetkiTipiAdi)
        {
            string donusDeger = (string)yetkiTipiDegerleri[yetkiTipiAdi];
            if (donusDeger == null)
                return string.Empty;
            else
                return donusDeger;
        }

        /// <summary>
        /// Yetki tipi verilen değeri kıyaslanarak içeriyorsa doğru, içermiyorsa yanlış döndüren yordam.
        /// </summary>
        /// <param name="yetkiTipiAdi">Değeri kontrol edilecek yetki tipinin adı.</param>
        /// <param name="bakilacakDeger">Kontrol edilecek değer. Yetki tipi değeri ile bakilacak değer uyumlu ise doğru döndürülür.</param>
        /// <param name="kurumKod">Eğer kurum kod verilmişse ve değerler tertip ise, tertibin kurumsal kod alanı, kurum kodu ile değiştirilir.</param>
        /// <param name="tertipMi">Kontrol edilecek verilerin tertip olduğu bilgisi. Eğer değerler tertip ise, tertibin her boyutu ve boyutun her bir parçası kıyaslanır.</param>
        /// <returns>Yetki tipi değeri ile bakılacak değer uyumlu ise doğru, değilse yanlış döndürülür.</returns>
        public static bool YetkiTipiDegerUygunMu(string yetkiTipiDegeri, string bakilacakDeger, string kurumKod, bool tertipMi)
        {
            if (yetkiTipiDegeri == null) return false;
            if (yetkiTipiDegeri == "*") return true;
            if (yetkiTipiDegeri == bakilacakDeger) return true;
            string[] d = yetkiTipiDegeri.Split(';', ',');
            foreach (string yetkiTipiDegeriParca in d)
            {
                if (!string.IsNullOrEmpty(yetkiTipiDegeriParca))
                {
                    if (tertipMi)
                    {
                        string kurumKurumsal = yetkiTipiDegeriParca;
                        if (!string.IsNullOrEmpty(kurumKod))
                        {
                            string[] parcalar = yetkiTipiDegeriParca.Split('-');
                            if (parcalar.Length > 0)
                            {
                                if (parcalar[0].Length > 0)
                                {
                                    kurumKurumsal = kurumKod;
                                    for (int i = 1; i < parcalar.Length; i++)
                                    {
                                        kurumKurumsal = kurumKurumsal + "-" + parcalar[i];
                                    }
                                }
                            }
                        }

                        if (OrtakFonksiyonlar.TertipKiyaslaCoklu(kurumKurumsal, bakilacakDeger))
                            return true;
                    }
                    else
                    {
                        if (yetkiTipiDegeriParca == "*") return true;
                        string[] e = bakilacakDeger.Split(',', ';');
                        foreach (string bakilacakDegerParca in e)
                        {
                            if (yetkiTipiDegeriParca == bakilacakDegerParca)
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Yetki tipi verilen değeri kıyaslanarak içeriyorsa doğru, içermiyorsa yanlış döndüren yordam.
        /// </summary>
        /// <param name="yetkiTipiAdi">Değeri kontrol edilecek yetki tipinin adı.</param>
        /// <param name="bakilacakDeger">Kontrol edilecek değer. Yetki tipi değeri ile bakilacak değer uyumlu ise doğru döndürülür.</param>
        /// <param name="kurumKod">Eğer kurum kod verilmişse ve değerler tertip ise, tertibin kurumsal kod alanı, kurum kodu ile değiştirilir.</param>
        /// <param name="tertipMi">Kontrol edilecek verilerin tertip olduğu bilgisi. Eğer değerler tertip ise, tertibin her boyutu ve boyutun her bir parçası kıyaslanır.</param>
        /// <returns>Yetki tipi değeri ile bakılacak değer uyumlu ise doğru, değilse yanlış döndürülür.</returns>
        public bool YetkiTipiDegeriIceriyorMu(string yetkiTipiAdi, string bakilacakDeger, string kurumKod, bool tertipMi)
        {
            if (yetkiTipiAdi == null || bakilacakDeger == null) return false;

            string yetkiTipiDegeri = YetkiTipiDegeriBul(yetkiTipiAdi);
            return YetkiTipiDegerUygunMu(yetkiTipiDegeri, bakilacakDeger, kurumKod, tertipMi);
        }

        /// <summary>
        /// Yetki tipi verilen değeri kıyaslanarak içeriyorsa doğru, içermiyorsa yanlış döndüren yordam.
        /// </summary>
        /// <param name="yetkiTipiAdi">Değeri kontrol edilecek yetki tipinin adı.</param>
        /// <param name="bakilacakDeger">Kontrol edilecek değer. Yetki tipi değeri ile bakilacak değer uyumlu ise doğru döndürülür.</param>
        /// <param name="tertipMi">Kontrol edilecek verilerin tertip olduğu bilgisi. Eğer değerler tertip ise, tertibin her boyutu ve boyutun her bir parçası kıyaslanır.</param>
        /// <returns>Yetki tipi değeri ile bakılacak değer uyumlu ise doğru, değilse yanlış döndürülür.</returns>
        public bool YetkiTipiDegeriIceriyorMu(string yetkiTipiAdi, string bakilacakDeger, bool tertipMi)
        {
            return YetkiTipiDegeriIceriyorMu(yetkiTipiAdi, bakilacakDeger, string.Empty, tertipMi);
        }

        public void KullaniciTipiEkle(ENUMKullaniciTipi kulTip)
        {
            if (!KullaniciTipiIceriyorMu(new int[1] { (int)kulTip }))
            {
                if (kullaniciTipi == null)
                {
                    kullaniciTipi = new ENUMKullaniciTipi[1];
                }
                else
                {
                    ENUMKullaniciTipi[] yeniKullaniciTipi = new ENUMKullaniciTipi[kullaniciTipi.Length + 1];
                    kullaniciTipi.CopyTo(yeniKullaniciTipi, 0);
                    kullaniciTipi = yeniKullaniciTipi;
                }
                kullaniciTipi[kullaniciTipi.Length - 1] = kulTip;
            }
        }

        public string KimlikBul()
        {
            return kullaniciKodu + "," + mernis + "," + ad + " " + soyad;
        }
    }

    /// <summary>
    /// Okuma işlemi sonucunda sunucudan istemciye kullanıcı nesnesi ile birlikte sonuc yapısını da döndüren veri aktarım nesnesi.
    /// </summary>
    [Serializable()]
    public class KullaniciVAN
    {
        /// <summary>
        /// Sunucuda yapılan işlemin sonucunu tutan nesne.
        /// </summary>
        public Sonuc sonuc;

        /// <summary>
        /// Sunucuda yapılan işlemin sonucunda oluşan Kullanıcı nesnesi.
        /// </summary>
        public Kullanici kullanici;

        /// <summary>
        /// Verilen kullanıcı nesnesi ile kullanıcı veri aktarım nesnesini oluşturan yordam.
        /// </summary>
        /// <param name="pKullanici">Veri aktarım nesnesinin kullanıcı bilgisi.</param>
        public KullaniciVAN(Kullanici pKullanici)
        {
            sonuc = new Sonuc();
            kullanici = pKullanici;
        }

        /// <summary>
        /// Boş kullanıcı veri aktarım nesnesini oluşturan yordam.
        /// </summary>
        public KullaniciVAN()
        {
            sonuc = new Sonuc();
            kullanici = new Kullanici();
        }
    }

    /// <summary>
    /// Kullanıcılara sisteme girdiklerinde, giriş bilgi ekranında gösterilen duyuruların metnini ve özelliklerini tutan sınıf.
    /// </summary>
    [Serializable()]
    public class Duyuru
    {
        public Duyuru()
        {
            kod = 0;
            onem = 0;
            konu = "";
        }
        /// <summary>
        /// Duyurunun veri tabanındaki sıra numarası
        /// </summary>
        public int kod { get; set; }

        /// <summary>
        /// Duyurular listelenirken, listenin üstünde gelecek olan duyuruların onem numarası düşük olan duyurular, daha önemli olarak düşünülür.
        /// </summary>
        public int onem { get; set; }

        /// <summary>
        /// Duyurunun sisteme kaydedildiği yada güncellendiği tarih.
        /// </summary>
        public TNSDateTime tarih = new TNSDateTime();
        public DateTime tarihDt { get { return tarih.Oku(); } }

        /// <summary>
        /// Duyuru metni.
        /// </summary>
        public string duyuru = string.Empty;

        /// <summary>
        /// GORUNMEZ ise duyuru açılış ekranında gösterilmez, aksi halde gösterilir.
        /// </summary>
        public ENUMGizle gizle = ENUMGizle.TANIMSIZ;
        public string gizleAd
        {
            get
            {
                if (gizle == ENUMGizle.GORUNMEZ) return "Gizli";
                if (gizle == ENUMGizle.GORUNUR) return "Açık";
                return "";
            }
        }

        /// <summary>
        /// Bu duyurunun açılış ekranında gösterileceği kullanıcı tipi. Bu alan dolu ise, kullanıcı bu alanda tutulan değerdeki kullanıcı tipine sahipse, duyuruyu görür.
        /// </summary>
        public ENUMKullaniciTipi[] kullaniciTipleri = null;

        /// <summary>
        /// Kullanıcı tipleri dizisindeki kullanıcı kodlarını ; ayıracı ile birleştirerek döndüren yordam.
        /// </summary>
        /// <returns></returns>
        public string KullaniciTipleriStr
        {
            get
            {
                if (kullaniciTipleri == null) return string.Empty;
                string donusDeger = string.Empty;
                for (int i = 0; i < kullaniciTipleri.Length; i++)
                {
                    if (!string.IsNullOrEmpty(donusDeger)) donusDeger += ";";
                    donusDeger += ((int)kullaniciTipleri[i]).ToString();
                }
                if (!string.IsNullOrEmpty(donusDeger)) donusDeger += ";";
                return donusDeger;
            }
        }

        /// <summary>
        /// Verile dizgi içindeki, ; ayıracı ile ayrılmış kullanıcı tiplerinin kodlarını ayırıp, nesnenin kullanıcı tipi dizisi özelliğini oluşturan yordam.
        /// </summary>
        /// <param name="kullaniciTipleriStr">; ayıracı ile ayrılmış kullanıcı tipi kodlarını tutan dizgi.</param>
        public void KullaniciTipleriOl(string kullaniciTipleriStr)
        {
            if (string.IsNullOrEmpty(kullaniciTipleriStr)) kullaniciTipleri = null;
            else
            {
                string[] ktKodlar = kullaniciTipleriStr.Split(';');
                kullaniciTipleri = new ENUMKullaniciTipi[ktKodlar.Length];
                for (int i = 0; i < ktKodlar.Length; i++)
                {
                    kullaniciTipleri[i] = ENUMKullaniciTipi.TANIMSIZ;

                    if (!string.IsNullOrEmpty(ktKodlar[i].Trim()))
                        kullaniciTipleri[i] = (ENUMKullaniciTipi)OrtakFonksiyonlar.ConvertToInt(ktKodlar[i], 0);
                }
            }
        }

        public static string penceredeGoster = "<!PEN!>";
        public TNSDateTime baslaTarih = null;
        public TNSDateTime bitisTarih = null;
        public ENUMPencerede pencerede;

        public string konu { get; set; }

        public string duyuruTemiz
        {
            get
            {
                string dy_duyuru = duyuru.Replace(penceredeGoster, "");//Pencerede gösterme kaydı;
                dy_duyuru = TNS.OrtakFonksiyonlar.RemoveHTML(Arac.StripTagsCharArray(dy_duyuru));
                int kes = dy_duyuru.Length;
                if (kes > 100) kes = 100;
                return dy_duyuru.Substring(0, kes);
            }
        }
    }

    /// <summary>
    /// Kullanıcılar sistemde yeni bir sayfaya giriş yaptıklarında sisteme yazılan bilgilerden elde edilen listelerde kullanılan sınıf.
    /// İstatistiki yada liste elde etmek için kullanılır. İstatistik bilgisinde şu anda tarih ve adet; kullanıcı listesi alındığında ise; kullanıcı adı, işlem tarihi, işlem yeri, sayfa ve bilgi1 alanları kullanılmaktadır.
    /// </summary>
    [Serializable()]
    public class KullaniciIslemYapan
    {
        /// <summary>
        /// İşlem listesi raporlarında kullanıcı adı bilgisi.
        /// </summary>
        public string kullaniciAd = string.Empty;

        /// <summary>
        /// İşlem listesi raporlarında kullanıcının son giriş yaptığı saat, istatistik raporlarında toplam sayının, zaman dilimi.
        /// </summary>
        public TNSDateTime islemTarih = new TNSDateTime();

        /// <summary>
        /// Kullanıcının sistemde son işlem yaptığı yerin ip adresi.
        /// </summary>
        public string islemYeri = string.Empty;

        /// <summary>
        /// Kullanıcının sistemde son işlem yaptığı sayfa.
        /// </summary>
        public string islemSayfa = string.Empty;

        /// <summary>
        /// Kullanıcının adı ve soyadını tutan değişken.
        /// </summary>
        public string bilgi1 = string.Empty;

        /// <summary>
        /// Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.
        /// </summary>
        public string bilgi2 = string.Empty;

        /// <summary>
        /// Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.
        /// </summary>
        public string bilgi3 = string.Empty;

        /// <summary>
        /// Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.
        /// </summary>
        public string bilgi4 = string.Empty;

        /// <summary>
        /// Şu anda kullanılmamaktadır. İleride kullanılmak üzere açılmıştır.
        /// </summary>
        public string bilgi5 = string.Empty;

        /// <summary>
        /// İstatistik bilgilerinde, raporlanan zamanda kaç adet sayfa çağrısı yapıldığı bilgisi.
        /// </summary>
        public int adet = 0;
    }

    /// <summary>
    /// Kullanım grafiği için veriler hazırlanırken, erişim sayısının bulunacağı zaman aralığı listesi.
    /// </summary>
    public enum ENUMKullanimBilgiZaman
    {
        /// <summary>
        /// Kullanım grafiği 15 dakikalık aralıklarla raporlanacak.
        /// </summary>
        ZAMAN_15DK = 1,

        /// <summary>
        /// Kullanım grafiği 1 saatlik aralıklarla raporlanacak.
        /// </summary>
        ZAMAN_1SAAT = 2,

        /// <summary>
        /// Kullanım grafiği 1 günlük aralıklarla raporlanacak.
        /// </summary>
        ZAMAN_GUN = 3,

        /// <summary>
        /// Kullanım grafiği 1 haftalık aralıklarla raporlanacak.
        /// </summary>
        ZAMAN_HAFTA = 4,

        /// <summary>
        /// Kullanım grafiği 1 aylık aralıklarla raporlanacak.
        /// </summary>
        ZAMAN_AY = 5,
    }

    /// <summary>
    /// Ekranlarda girilen ve gelen kayıtların filtrelenmesinde kullanılan alanların 
    /// kullanıcı bazında tutulduğu yapı.
    /// </summary>
    [Serializable()]
    public class Filtre
    {
        /// <summary>
        /// Filtre bilgisinin ait olduğu kullanıcı
        /// </summary>
        public Kullanici kullanan = null;

        /// <summary>
        /// Filtre alanlarının gruplanmasında kullanılan alan. Program içerisinde statik olarak değer verilir. 
        /// </summary>
        public string grup = string.Empty;

        /// <summary>
        /// Yazılım tarafından, kullanıcı bazında üretilen indis.
        /// </summary>
        public int siraNo = 0;

        /// <summary>
        /// Filtre grubu içerindeki alanın adı. Program içerisinde statik olarak değer verilir. 
        /// </summary>
        public string ad = string.Empty;

        /// <summary>
        /// Kullanıcının girdiği ve kayıtların filtrelenmesinde kullanılacak değer.
        /// Bu değer veri tabanında yukarıda anahtar alanlarla birlikte saklanır.
        /// </summary>
        public string kriter = string.Empty;

        /// <summary>
        /// Filtre alanındaki değerler geerli mi diye kontrol edilir. Hata varsa hata mesajı döndürülür.
        /// </summary>
        /// <param name="siraNoKontrolEt">Doğru ise sıra no alanı sıfırdan büyük mü diye kontrol edilir.</param>
        /// <returns>Hata varsa hata mesajı döndürülür.</returns>
        public string KontrolEt(bool siraNoKontrolEt)
        {
            string hata = string.Empty;
            if (!siraNoKontrolEt && string.IsNullOrEmpty(ad.Trim()))
                hata += KullaniciResI.KYMF001 + "<br>";
            if (!siraNoKontrolEt && string.IsNullOrEmpty(kriter.Trim()))
                hata += KullaniciResI.KYMF002 + "<br>";
            if (kullanan == null || string.IsNullOrEmpty(kullanan.kullaniciKodu) || string.IsNullOrEmpty(kullanan.kullaniciKodu.Trim()))
                hata += KullaniciResI.KYMF003 + "<br>";
            if (siraNoKontrolEt && siraNo <= 0)
                hata += KullaniciResI.KYMF004 + "<br>";

            return hata;
        }
    }

    [Serializable()]
    public class Mesaj
    {
        public TNSDateTime tarih;
        public string kullanici = "";
        public string alici = "";
        public string mesaj = "";
        public int okundu = -1;

        public Mesaj()
        {
            tarih = new TNSDateTime();
            kullanici = "";
            alici = "";
            mesaj = "";
            okundu = -1;
        }
    }

    [Serializable()]
    public class Mevzuat
    {
        public string kod;
        public string anaKod;
        public string klonKod;
        public int sira;
        public string ad;
        public string aciklama;
        public TNSDateTime baslangicTarih;
        public TNSDateTime bitisTarih;
        public int turKod;
        public int guncelleme;
        public int sil;
        public string bilgiKod;
        public int bilgiNo;

        public Mevzuat()
        {
            kod = "";
            anaKod = "";
            klonKod = "";
            sira = 0;
            ad = "";
            aciklama = "";
            baslangicTarih = new TNSDateTime();
            bitisTarih = new TNSDateTime();
            turKod = 0;
            guncelleme = 0;
            sil = 0;
            bilgiKod = "";
            bilgiNo = 0;
        }
    }

    public enum ENUMMevzuatTur
    {
        MEVZUAT = 0,
        KANUN = 1,
        KISIM = 2,
        BOLUM = 3,
        MADDE = 4,
        FIKRA = 5,
        YONETMELIK = 6,
        TEBLIG = 7
    }

    [Serializable()]
    public class MevzuatDurum
    {
        public string kod;
        public int kilit;
        public TNSDateTime islemTarih;

        public MevzuatDurum()
        {
            kod = "";
            kilit = 0;
            islemTarih = new TNSDateTime();
        }
    }



    [Serializable()]
    public class MevzuatSayfa
    {
        public string sayfaKod;
        public string mevzuatKod;
        public int sira;
        public Mevzuat[] mevzuat;

        public MevzuatSayfa()
        {
            sayfaKod = "";
            mevzuatKod = "";
            sira = 0;
            mevzuat = new Mevzuat[0];
        }
    }

    /// <summary>
    /// CevapSecenekler: 
    /// </summary>
    [System.Serializable()]
    public class CevapSecenekler
    {
        /// <summary>
        /// Sıra No: Seçenek sıra numarası.
        /// </summary>
        public int secenekSiraNo;

        /// <summary>
        /// secenek: Seçenek metninin seçenek kodu (a,b,c veya 1,2,3 gibi).
        /// </summary>
        public string secenek;

        /// <summary>
        /// Seçenek Metni: Seçenek metni.
        /// </summary>
        public string secenekMetin;
        public int cevapSay;
        public double puan = 0.0;
    }

    [System.Serializable()]
    public class AnketCaprazBilgi
    {
        public AnketSoru[] sorular = new AnketSoru[2];
        public int[,] toplamlar = null;
        public string anketAd;

        public bool tasti;
    }

    [System.Serializable()]
    public class AnketBilgi
    {
        public string modul;
        public string anketAnahtar;
        public string anketAd;
        public TNSDateTime baslaTarih;
        public TNSDateTime bitisTarih;
        public AnketSoru[] sorular;
        public int orgBirimKod;
        public string cevapAnahtar;
        public string cevapIP;
        public TNSDateTime cevapTarih;
        public string kullaniciKod;
        public string mailAdres;
        public string onayKod;
        public int altBirimDahil;
        public bool tekGirislik;
        public string anketiYapan;

        public void AlanlarGecerliMi(Sonuc s)
        {
            if (string.IsNullOrEmpty(anketAd))
                s.DegerAta("Anket adı boş olamaz!");
            if (string.IsNullOrEmpty(modul))
                s.DegerAta("Modül adı boş olamaz!");
            if (string.IsNullOrEmpty(anketAnahtar))
                s.DegerAta("Anket anahtarı boş olamaz!");
            if (baslaTarih == null || baslaTarih.isNull)
                s.DegerAta("Anket başlama tarihi dolu olmalıdır!");
            if (bitisTarih == null || bitisTarih.isNull)
                s.DegerAta("Anket bitiş tarihi dolu olmalıdır!");
            //if (sorular == null || sorular.Length == 0)
            //    s.DegerAta("Ankette en az bir soru olmalıdır!");
        }
    }

    [System.Serializable()]
    public class AnketCevap
    {
        public int cevapNo;
        public string cevapYazi;
        public double puan;

        public AnketCevap(int cevapNo, string cevapYazi, double puan)
        {
            this.cevapNo = cevapNo;
            this.cevapYazi = cevapYazi;
            this.puan = puan;
        }
    }


    /// <summary>
    /// AnketSoru: 
    /// </summary>
    [System.Serializable()]
    public class AnketSoru
    {
        public int soruSira;
        public int soruSeviye;

        /// <summary>
        /// Sıra No: Soru numarasını giriniz.
        /// </summary>
        public int prSiraNo;

        /// <summary>
        /// Soru Metni: Soru metnini giriniz.
        /// </summary>
        public string soruMetin;

        /// <summary>
        /// Seçenek Tür: Seçenek türü giriniz.
        /// </summary>
        public ENUMSecenekTur secenekTur;

        /// <summary>
        /// Anantar 1: Arama anahtarı giriniz.
        /// </summary>
        public string anahtar1;

        /// <summary>
        /// Anantar 2: Arama anahtarı giriniz.
        /// </summary>
        public string anahtar2;

        /// <summary>
        /// Cevap Seçenek: Doğru cevap seçeneği.
        /// </summary>
        public int dogruCevapSecenek;

        /// <summary>
        /// CevapSecenekler: 
        /// </summary>
        public CevapSecenekler[] cevapSecenekler;

        public void AlanlarTutarliMi(Sonuc s)
        {
            if (string.IsNullOrEmpty(soruMetin)) s.DegerAta("Sorunun metin kısmı boş olamaz!");
            else if (soruMetin.Length > 700) s.DegerAta("Soru metni en fazla 700 harf olabilir!");

            if (string.IsNullOrEmpty(anahtar1)) s.DegerAta("İlk anahtar alan boş olamaz!");
            else if (anahtar1.Length > 50) s.DegerAta("İlk anahtar alan en fazla 50 harf olabilir!");

            if (!string.IsNullOrEmpty(anahtar2) && (anahtar2.Length > 50)) s.DegerAta("İkinci anahtar alan en fazla 50 harf olabilir!");

            if (secenekTur == ENUMSecenekTur.TANIMSIZ) s.DegerAta("Seçenek türü girilmelidir.");

            if (secenekTur != ENUMSecenekTur.METINCEVAP && secenekTur != ENUMSecenekTur.TANIMSIZ)
            {
                //Ekonomik yatırım hibe başvurularında tek seçenekli bir soru var :)
                if (cevapSecenekler == null || cevapSecenekler.Length < 1) s.DegerAta("En az bir seçenek girilmelidir!");
                else
                    for (int i = 0; i < cevapSecenekler.Length; i++)
                    {
                        if (string.IsNullOrEmpty(cevapSecenekler[i].secenek)) s.DegerAta(string.Format("{0}. seçeneğin şık alanı boş olamaz!", i + 1));
                        else if (cevapSecenekler[i].secenek.Length > 500) s.DegerAta(string.Format("{0}. seçeneğin şık alanı 500 harften fazla olamaz!", i + 1));

                        if (string.IsNullOrEmpty(cevapSecenekler[i].secenekMetin)) s.DegerAta(string.Format("{0}. seçeneğin metin alanı boş olamaz!", i + 1));
                        else if (cevapSecenekler[i].secenekMetin.Length > 500) s.DegerAta(string.Format("{0}. seçeneğin metin alanı 500 harften fazla olamaz!", i + 1));
                    }
                if (s.islemSonuc)
                {
                    for (int i = 0; i < cevapSecenekler.Length - 1; i++)
                    {
                        for (int j = i + 1; j < cevapSecenekler.Length; j++)
                        {
                            if (cevapSecenekler[i].secenek == cevapSecenekler[j].secenek) s.DegerAta(string.Format("{0}. seçenek ile {1}. seçeneğin şık alanları aynı olamaz!", i + 1, j + 1));
                        }
                    }
                }
            }
        }

        public int durum;

        public string DurumAciklama()
        {
            if (durum == 1)
                return "Anket Kullanımına Açık";
            else if (durum == 9)
                return "Anket Kullanımına kapalı";
            else
                return "Yeni";
        }

        private AnketCevap[] anketCevap = null;
        public void CevapEkle(int cevapNo, string cevapYazi, double puan)
        {
            if (cevapNo == -1 && string.IsNullOrEmpty(cevapYazi))
                throw new Exception(soruSira.ToString() + ". soru için, lütfen değer giriniz!");
            if (anketCevap == null)
            {
                if (secenekTur == ENUMSecenekTur.COKTANCOKSECMELIACIK || secenekTur == ENUMSecenekTur.COKTANCOKSECMELI || secenekTur == ENUMSecenekTur.COKTANCOKSECMELISECIMLIK)
                {
                    if (cevapNo >= 0)
                        anketCevap = new AnketCevap[4];
                    else
                        anketCevap = new AnketCevap[1];
                }
                else
                    anketCevap = new AnketCevap[1];
                for (int i = 0; i < anketCevap.Length; i++)
                {
                    anketCevap[i] = null;
                }
            }
            int yer;
            if (secenekTur == ENUMSecenekTur.COKTANCOKSECMELIACIK || secenekTur == ENUMSecenekTur.COKTANCOKSECMELI || secenekTur == ENUMSecenekTur.COKTANCOKSECMELISECIMLIK)
            {
                int tut = CevapYerBul();
                if (tut < 0) return;
                if (tut == anketCevap.Length)
                {
                    AnketCevap[] dizi = new AnketCevap[tut + 4];
                    anketCevap.CopyTo(dizi, 0);
                    for (int i = tut; i < dizi.Length; i++)
                    {
                        dizi[i] = null;
                    }
                    anketCevap = dizi;
                }
                yer = tut;
            }
            else
            {
                yer = 0;
            }
            anketCevap[yer] = new AnketCevap(cevapNo, cevapYazi, puan);
        }

        public int CevapSay()
        {
            if (anketCevap == null) return 0;
            return CevapYerBul();
        }

        private int CevapYerBul()
        {
            if (anketCevap == null) return -1;
            for (int i = 0; i < anketCevap.Length; i++)
            {
                if (anketCevap[i] == null) return i;
            }
            return anketCevap.Length;
        }

        public AnketCevap Cevap(int siraNo)
        {
            if (anketCevap == null) return null;
            return anketCevap[siraNo];
        }

        public AnketCevap CevapBul(int secenekSiraNo)
        {
            if (anketCevap == null) return null;
            for (int i = 0; i < CevapSay(); i++)
            {
                if (anketCevap[i].cevapNo == secenekSiraNo) return anketCevap[i];
            }
            return null;
        }
    }

    /// <summary>
    /// Logo: 
    /// </summary>
    [System.Serializable()]
    public class Logo
    {
        /// <summary>
        /// tip : Logo tipi
        /// </summary>
        public int turkod;

        /// <summary>
        /// tip : Logo tipi
        /// </summary>
        public string turad;

        /// <summary>
        /// data: Logo byte array
        /// </summary>
        public byte[] data;

        /// <summary>
        /// Constructer
        /// </summary>
        public Logo()
        {
            turkod = 0;
            turad = string.Empty;
            data = null;
        }
        public Logo(int tur)
        {
            this.turkod = tur;
        }
    }

    /// <summary>
    /// Cografya: 
    /// </summary>
    [System.Serializable()]
    public class Cografya
    {
        private string _kod;
        /// <summary>
        /// kod: 
        /// </summary>
        public string kod { get { return _kod; } set { _kod = value; } }

        private string _ad;
        /// <summary>
        /// ad: 
        /// </summary>
        public string ad { get { return _ad; } set { _ad = value; } }

        private string _ad2;
        /// <summary>
        /// ad2: 
        /// </summary>
        public string ad2 { get { return _ad2; } set { _ad2 = value; } }

        private string _kisaAd;
        /// <summary>
        /// kisaAd: 
        /// </summary>
        public string kisaAd { get { return _kisaAd; } set { _kisaAd = value; } }

        private string _plakaKod;
        /// <summary>
        /// plakaKod: 
        /// </summary>
        public string plakaKod { get { return _plakaKod; } set { _plakaKod = value; } }

        private int _nufus;
        /// <summary>
        /// nufus: 
        /// </summary>
        public int nufus { get { return _nufus; } set { _nufus = value; } }

        private int _muhasebe;
        /// <summary>
        /// muhasebe: 
        /// </summary>
        public int muhasebe { get { return _muhasebe; } set { _muhasebe = value; } }

        private int _mernisKod;
        /// <summary>
        /// mernisKod: 
        /// </summary>
        public int mernisKod { get { return _mernisKod; } set { _mernisKod = value; } }

        /// <summary>
        /// ilceMi: 
        /// </summary>
        public bool ilceMi { get; set; }

        public bool ilMi { get; set; }
        public bool ulkeMi { get; set; }

        public Cografya()
        {
            ilceMi = false;
            ilMi = false;
            ulkeMi = false;
        }

        public string kodAd
        {
            get
            {
                return kod + "-" + ad;
            }
            set
            {
                string a1 = "";
                string a2 = "";
                OrtakFonksiyonlar.StringiIkiyeBol(value, "-", ref a1, ref a2);
                kod = a1;
                ad = a2;
            }
        }
    }

    public enum ENUMYoneticiBildirimFormuDurum
    {
        Yeni = 1,
        Gonderildi = 2,
        Onaylandi = 3,
        IptalEdildi = 8,
        Reddedildi = 9,
        Tanimsiz = 0,
        YeniGonderildi = 99
    }

    /// <summary>
    /// KullaniciMesaj: 
    /// </summary>
    [System.Serializable()]
    public class KullaniciMesaj
    {
        private string _kullaniciKodu;
        /// <summary>
        /// Kullanıcı Kodu: Yılı giriniz.
        /// </summary>
        public string kullaniciKodu { get { return _kullaniciKodu; } set { _kullaniciKodu = value; } }

        private int _mesajNo;
        /// <summary>
        /// Mesaj No: Saymanlık kodunu giriniz.
        /// </summary>
        public int mesajNo { get { return _mesajNo; } set { _mesajNo = value; } }

        private TNSDateTime _tarih;
        /// <summary>
        /// Tarih: Harcama Birimi Kodunu Giriniz.
        /// </summary>
        public TNSDateTime tarih { get { return _tarih; } set { _tarih = value; } }
        public DateTime tarihDT
        {
            get
            {
                if (_tarih == null)
                    return DateTime.MinValue;
                else
                    return _tarih.Oku();
            }
            set
            {
                if (_tarih == null) _tarih = new TNSDateTime(value);
                else _tarih.Yaz(value);
            }
        }

        private string _mesaj;
        /// <summary>
        /// Mesaj: Yazı giriniz.
        /// </summary>
        public string mesaj { get { return _mesaj; } set { _mesaj = value; } }

        public KullaniciMesaj(string kullaniciKodu, string mesaj)
        {
            this._kullaniciKodu = kullaniciKodu;
            this._mesaj = mesaj;
        }

        public KullaniciMesaj()
        {
        }
    }

    /// <summary>
    /// YoneticiBildirimFormu: Yonetici Bildirim Formu
    /// </summary>
    [System.Serializable()]
    public class YoneticiBildirimFormu
    {
        private int _saymanlik;
        /// <summary>
        /// Saymanlık: Lütfen Saymanlık'ı seçin.
        /// </summary>
        public int saymanlik { get { return _saymanlik; } set { _saymanlik = value; } }

        private string _birim;
        /// <summary>
        /// Birim: Lütfen Birimi seçin.
        /// </summary>
        public string birim { get { return _birim; } set { _birim = value; } }

        private int _talepNo;
        /// <summary>
        /// Talep No: Lütfen Talep No yazınız.
        /// </summary>
        public int talepNo { get { return _talepNo; } set { _talepNo = value; } }

        private TNSDateTime _talepTarihi;
        /// <summary>
        /// Talep Tarihi: Lütfen Talep Tarihi seçiniz.
        /// </summary>
        public TNSDateTime talepTarihi { get { return _talepTarihi; } set { _talepTarihi = value; } }
        public DateTime talepTarihiDT
        {
            get
            {
                if (_talepTarihi == null)
                    return DateTime.MinValue;
                else
                    return _talepTarihi.Oku();
            }
            set
            {
                if (_talepTarihi == null) _talepTarihi = new TNSDateTime(value);
                else _talepTarihi.Yaz(value);
            }
        }

        private string _yoneticiTCKimlikNo;
        /// <summary>
        /// Yönetici TC Kimlik No: Lütfen Yönetici olacak kişinin TC Kimlik Nosunu giriniz.
        /// </summary>
        public string yoneticiTCKimlikNo { get { return _yoneticiTCKimlikNo; } set { _yoneticiTCKimlikNo = value; } }

        private string _yoneticiAdSoyad;
        /// <summary>
        /// Yönetici Adı Soyadı: Lütfen Yönetici olacak kişinin TC Kimlik Nosunu giriniz.
        /// </summary>
        public string yoneticiAdSoyad { get { return _yoneticiAdSoyad; } set { _yoneticiAdSoyad = value; } }

        /// <summary>
        /// Durum: Lütfen Durumu seçiniz.
        /// </summary>
        public ENUMYoneticiBildirimFormuDurum durum { get; set; }

        public string durumAd
        {
            get
            {
                return TNS.KYM.Arac.YoneticiBildirimDurumAdBul(durum);
            }
        }

        private string _islemYapan;
        /// <summary>
        /// İşlem Yapan: 
        /// </summary>
        public string islemYapan { get { return _islemYapan; } set { _islemYapan = value; } }

        private string _islemOnaylayan;
        /// <summary>
        /// İşlem Onaylayan: 
        /// </summary>
        public string islemOnaylayan { get { return _islemOnaylayan; } set { _islemOnaylayan = value; } }

        public string talepNolar { get; set; }
    }

    /// <summary>
    /// EvrakAktarimDetay: 
    /// </summary>
    [System.Serializable()]
    public class EvrakAktarimMeta
    {
        public EvrakAktarimMeta()
        {
            this.alanKodu = "";
            this.alanDeger = "";
        }

        public EvrakAktarimMeta(string alanKodu, string alanDeger)
        {
            this.alanKodu = alanKodu;
            this.alanDeger = alanDeger;
        }

        /// <summary>
        /// AlanKodu: Alan Kodu
        /// </summary>
        public string alanKodu { get; set; }

        /// <summary>
        /// AlanDeger: Alan Değer
        /// </summary>
        public string alanDeger { get; set; }
    }

    public class MenuSayfa : IComparable
    {
        public string kod { get; set; }
        public string ad { get; set; }
        public string sayfa { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is MenuSayfa)
            {
                return ad.CompareTo(((MenuSayfa)obj).ad);
            }
            return 0;
        }
    }

    /// <summary>
    /// BirimMesaj: Birim Mesajlari
    /// </summary>
    [System.Serializable()]
    public class BirimMesaj
    {
        public enum MesajDurum : int
        {
            Yeni = 1,
            Yayinda = 2,
            Yapildi = 3,
            Arsivlendi = 4,
            Iptal = 9,
        }

        public static SortedList sayfaListe = null;
        public TNSDateTime kayitTarihBuyuk;
        public TNSDateTime kayitTarihKucuk;

        public string id
        {
            get
            {
                return saymanlikKod + ";" + birimKod + ";" + kurKod + ";" + siraNo;
            }
            set
            {
                string[] parcalar = value.Split(';');
                saymanlikKod = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(parcalar, 0, ""), 0);
                birimKod = OrtakFonksiyonlar.DiziElemanAl(parcalar, 1, "");
                kurKod = OrtakFonksiyonlar.DiziElemanAl(parcalar, 2, "");
                siraNo = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(parcalar, 3, ""), 0);
            }
        }
        /// <summary>
        /// Saymanlık Kod: Saymanlık Kod
        /// </summary>
        public int saymanlikKod { get; set; }

        /// <summary>
        /// Birim Kod: Birim Kod
        /// </summary>
        public string birimKod { get; set; }

        /// <summary>
        /// Kur Kod: Kurum Kod
        /// </summary>
        public string kurKod { get; set; }

        /// <summary>
        /// Sıra No: Sıra No
        /// </summary>
        public int siraNo { get; set; }

        /// <summary>
        /// mesaj: Mesaj
        /// </summary>
        public string mesaj { get; set; }

        /// <summary>
        /// kaydeden: Kaydeden Kişi
        /// </summary>
        public string kaydeden { get; set; }

        /// <summary>
        /// Kayıt Tarihi: Kayıt Tarihi
        /// </summary>
        public TNSDateTime kayitTarihi { get; set; }
        public DateTime kayitTarihiDT
        {
            get
            {
                if (kayitTarihi == null)
                    return DateTime.MinValue;
                else
                    return kayitTarihi.Oku();
            }
            set
            {
                if (kayitTarihi == null) kayitTarihi = new TNSDateTime(value);
                else kayitTarihi.Yaz(value);
            }
        }

        /// <summary>
        /// Karşılayan: Karşılayan
        /// </summary>
        public string karsilayan { get; set; }

        /// <summary>
        /// Karşılama Tarihi: Karşılama Tarihi
        /// </summary>
        public TNSDateTime karsilamaTarihi { get; set; }
        public DateTime karsilamaTarihiDT
        {
            get
            {
                if (karsilamaTarihi == null)
                    return DateTime.MinValue;
                else
                    return karsilamaTarihi.Oku();
            }
            set
            {
                if (karsilamaTarihi == null) karsilamaTarihi = new TNSDateTime(value);
                else karsilamaTarihi.Yaz(value);
            }
        }

        /// <summary>
        /// sayfa: İlgili Sayfa
        /// </summary>
        public string sayfa { get; set; }

        private string _sayfaAd = null;
        private string _sayfaAdres = null;
        public string durumlar;

        /// <summary>
        /// sayfa: İlgili Sayfa Adı
        /// </summary>
        public string sayfaAd
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_sayfaAd))
                {
                    if (sayfaListe != null)
                    {
                        MenuSayfa ms = (MenuSayfa)sayfaListe[sayfa];
                        if (ms != null)
                        {
                            _sayfaAd = ms.ad;
                            _sayfaAdres = ms.sayfa;
                        }
                    }
                }
                return _sayfaAd;
            }
        }

        /// <summary>
        /// sayfa: İlgili Sayfa Adı
        /// </summary>
        public string sayfaAdres
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_sayfaAdres))
                {
                    if (sayfaListe != null)
                    {
                        MenuSayfa ms = (MenuSayfa)sayfaListe[sayfa];
                        if (ms != null)
                        {
                            _sayfaAd = ms.ad;
                            _sayfaAdres = ms.sayfa;
                        }
                    }
                }
                return _sayfaAdres;
            }
        }

        /// <summary>
        /// durum: Form durumu
        /// </summary>
        public MesajDurum durum { get; set; }

        public string durumAd
        {
            get
            {
                return DurumAdBul(durum);
            }
        }

        public static string DurumAdBul(MesajDurum durum)
        {
            switch (durum)
            {
                case MesajDurum.Yeni: return "Yeni";
                case MesajDurum.Yayinda: return "Yayında";
                case MesajDurum.Yapildi: return "Birim Tamamladı";
                case MesajDurum.Arsivlendi: return "Arşiv";
                case MesajDurum.Iptal: return "İptal";
                default: return "";
            }
        }

        public string saymanlikAd { get; set; }

        public string birimAd { get; set; }

        public string saymanlikBirimAd
        {
            get
            {
                return TNS.KYM.Arac.BirimAdOl(saymanlikAd, birimAd);
            }
        }
    }

    /// <summary>
    /// EkliDosyalar: Sistemde tutulan dosya bilgileri
    /// </summary>
    [System.Serializable()]
    public class EkliDosyalar
    {
        public EkliDosyalar()
        {

        }

        public EkliDosyalar(string modul, string form, string dosyaAdi, int dosyaBoy, System.IO.Stream stream)
        {
            this.modul = modul;
            this.form = form;
            this.dosyaAdi = dosyaAdi;
            this.ekliDosyaIcerik = new byte[dosyaBoy];
            this.aciklama = "";
            stream.Read(this.ekliDosyaIcerik, 0, dosyaBoy);
        }

        public EkliDosyalar(string modul, string form, int dosyaNo)
        {
            this.modul = modul;
            this.form = form;
            this.dosyaNo = dosyaNo;
            this.aciklama = "";
        }

        /// <summary>
        /// Modül: Modül Kodu
        /// </summary>
        public string modul { get; set; }

        /// <summary>
        /// Form: Form Kodu
        /// </summary>
        public string form { get; set; }

        /// <summary>
        /// Dosya No: Dosya Numarası
        /// </summary>
        public int dosyaNo { get; set; }

        /// <summary>
        /// Dosya Adı: Dosyanın Adı
        /// </summary>
        public string dosyaAdi { get; set; }
        public string dosyaAdiSade
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(dosyaAdi); }
        }

        public string uzanti
        {
            get { return System.IO.Path.GetExtension(dosyaAdi).Replace(".", ""); }
        }

        /// <summary>
        /// Buyukluk: Dosyanın Byte Cinsinden Büyüklüğü
        /// </summary>
        public long buyukluk { get; set; }
        public string buyuklukStr
        {
            get
            {
                long si = 0;
                string t;
                if (buyukluk < 10000) { si = buyukluk; t = "B"; }
                else if (buyukluk < 10000000) { si = buyukluk / 1024; t = "KB"; }
                else { si = buyukluk / (1024 * 1024); t = "MB"; }
                return si.ToString("#,##0") + " " + t;
            }
        }

        /// <summary>
        /// Yukleme Tarihi: Dosyanın Yükleme Tarihi
        /// </summary>
        public TNSDateTime yuklemeTarihi { get; set; }
        public DateTime yuklemeTarihiDT
        {
            get
            {
                if (yuklemeTarihi == null)
                    return DateTime.MinValue;
                else
                    return yuklemeTarihi.Oku();
            }
            set
            {
                if (yuklemeTarihi == null) yuklemeTarihi = new TNSDateTime(value);
                else yuklemeTarihi.Yaz(value);
            }
        }

        /// <summary>
        /// Yukleyen: Dosyayı Yükleyen Kullanıcı
        /// </summary>
        public string yukleyen { get; set; }

        public string onaylayan { get; set; }

        /// <summary>
        /// EkliDosyalarParcalari: Sistemde tutulan dosyanın içeriği
        /// </summary>
        public byte[] ekliDosyaIcerik;
        public TNSDateTime onayZaman;
        public string aciklama { get; set; }
        public string birimAd { get; set; }
    }

    [System.Serializable()]
    public class YerleskeOda
    {
        public int kod { get; set; }
        public string ad { get; set; }
        //public string gorKod { get; set; }
        public string tarif { get; set; }
        public int saymanlikKod { get; set; }
        public string birimKod { get; set; }
        public bool altDalVar { get; set; }
        public int ustKod { get; set; }
        public string ustGorKod { get; set; }
        public string gorKodKisa { get; set; }
        public string gorTamKod { get { return ustGorKod + gorKodKisa; } }
        public string gorTamKodNoktali { get { return OrtakFonksiyonlar.KodNoktaKoy("99.99.99.99.99.99.99.99.99.99.99", gorTamKod); } }

        public static string SonIki(string gorKod)
        {
            if (string.IsNullOrWhiteSpace(gorKod)) return "";
            int len = gorKod.Length;
            if (len < 2) return "";
            else return gorKod.Substring(len - 2, 2);
        }

        public static string SonUc(string gorKod)
        {
            if (string.IsNullOrWhiteSpace(gorKod)) return "";
            int len = gorKod.Length;
            if (len < 3) return "";
            else return gorKod.Substring(len - 3, 3);
        }

        public string ustAd { get; set; }
        public string ilKod { get; set; }

        public string UstBilgiOl()
        {
            if (ustKod <= 0) return "";
            return ustGorKod + " - " + ustAd;
        }

        public int yerTurKod { get; set; }

        public int odaTurKod { get; set; }

        public int orgKod { get; set; }

        public string ustTarif { get; set; }

        public string ilAd { get; set; }

        public string orgAd { get; set; }

        public int durum { get; set; }

        public int maliyetMerkezi { get; set; }
        public string gorKodEski { get; set; }
        public int siraNo { get; set; }

    }

    [System.Serializable()]
    public class IISLogSatir
    {
        public string date { get; set; }
        public string time { get; set; }
        public string s_ip { get; set; }
        public string cs_method { get; set; }
        public string cs_uri_stem { get; set; }
        public string cs_uri_query { get; set; }
        public string s_port { get; set; }
        public string cs_username { get; set; }
        public string c_ip { get; set; }
        public string cs_User_Agent_ { get; set; }
        public string cs_Referer_ { get; set; }
        public string sc_status { get; set; }
        public string sc_substatus { get; set; }
        public string sc_win32_status { get; set; }
        public string time_taken { get; set; }
        public TNSDateTime IslemSaat { get { return new TNSDateTime(date + " " + time); } }

        public static void DegerAra(IISLogSatir hedef, string[] alanlar, string[] degerler)
        {
            Type t = hedef.GetType();
            for (int i = 0; i < alanlar.Length; i++)
            {
                System.Reflection.PropertyInfo pi = t.GetProperty(alanlar[i]);
                if (pi != null)
                    pi.SetValue(hedef, degerler[i], null);
                else
                    throw new Exception(alanlar[i] + " bulunamadı");
            }
        }

        public override string ToString()
        {
            return
            "date" + date +
            ", time" + time +
            ", s_ip" + s_ip +
            ", cs_method" + cs_method +
            ", cs_uri_stem" + cs_uri_stem +
            ", cs_uri_query" + cs_uri_query +
            ", s_port" + s_port +
            ", cs_username" + cs_username +
            ", c_ip" + c_ip +
            ", cs_User_Agent_" + cs_User_Agent_ +
            ", cs_Referer_" + cs_Referer_ +
            ", sc_status" + sc_status +
            ", sc_substatus" + sc_substatus +
            ", sc_win32_status" + sc_win32_status +
            ", time_taken" + time_taken;
        }
    }

    /// <summary>
    /// Kullanıcı sunucusu tarafından sunulan işlemlerin tanımını tutan arayüz sınıf
    /// </summary>
    public interface IKYMServis
    {
        string YetkiTipiAgacKodAd(Kullanici kullanan, YetkiTipi yetkiTipi, int yil, string kod, string ekParamCagri);

        /// <summary>
        /// Kullanıcı tipleri için, değer girilerek kullanıcının yapabileceği işlemleri kısıtlayan yetki tiplerinin listesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">Yetki tipi listesi döndürülürken, eğer sadece bu kullanıcıya ait olanlar döndürülecekse kullanılan ve işlemi yapan kullanıcının bilgilerini tutan parametre.</param>
        /// <param name="yetkiTipi">Veri giriş listesi döndürülecek yetki tipi parametresi.</param>
        /// <param name="yil">Yetki tipi listesi döndürülürken, eğer sadece belirli bir yıla ait olanlar döndürülecekse kullanılan parametre.</param>
        /// <param name="ustKod">Döndürülecek listenin ait olduğu dalın kodu.</param>
        /// <param name="ekParamCagri">Yetki tipi listesi döndürülürken, her çağrı için verilen ve çağrıdan çağrıya anlamı ve biçimi değişen ek parametre. Anlam ve biçim için ilgili çağrıyı gerçekleştiren yordama bakmak gerekir.</param>
        /// <returns>Verilen kıstaslara göre elde edilen ListeSatir dizisi döndürülür.</returns>
        ListeSatir[] YetkiTipiAgacListesi(Kullanici kullanan, YetkiTipi yetkiTipi, int yil, string ustKod, string ekParamCagri);

        /// <summary>
        /// Kullanıcı tipleri için, değer girilerek kullanıcının yapabileceği işlemleri kısıtlayan yetki tiplerinin listesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">Yetki tipi listesi döndürülürken, eğer sadece bu kullanıcıya ait olanlar döndürülecekse kullanılan ve işlemi yapan kullanıcının bilgilerini tutan parametre.</param>
        /// <param name="yetkiTipi">Veri giriş listesi döndürülecek yetki tipi parametresi.</param>
        /// <param name="yil">Yetki tipi listesi döndürülürken, eğer sadece belirli bir yıla ait olanlar döndürülecekse kullanılan parametre.</param>
        /// <param name="ekParamCagri">Yetki tipi listesi döndürülürken, her çağrı için verilen ve çağrıdan çağrıya anlamı ve biçimi değişen ek parametre. Anlam ve biçim için ilgili çağrıyı gerçekleştiren yordama bakmak gerekir.</param>
        /// <returns>Verilen kıstaslara göre elde edilen ListeSatir dizisi döndürülür.</returns>
        ListeSatir[] YetkiTipiGirisListesi(Kullanici kullanan, YetkiTipi yetkiTipi, int yil, string ekParamCagri);

        /// <summary>
        /// Verilen kıstaslara uyan kullanıcı listesini döndüren yordam.
        /// </summary>
        /// <param name="aramaBilgisi">Listelenecek kullanıcıların bilgileri tutan Kullanıcı nesnesi.</param>
        /// <param name="basla">Sonuç kümesinde bu parametre ile belirtilen sıradaki kayıttan itibaren olanlar alınır.</param>
        /// <param name="say">Sonuç kümesinde bu parametre ile verilen sayıdaki kadar eleman yer alır. 0 ise tüm elemanlar sonuç kümesinde yer alır.</param>
        /// <returns>Verilen kıstaslara uyan ve basla parametresiyle belirtilen sıradan başlayarak, say parametresi ile belirtilen kadar kayıt döndürülür.</returns>
        ObjectArray<Kullanici> KullaniciAra(Kullanici aramaBilgisi, int basla, int say);

        /// <summary>
        /// Verilen kıstaslara uyan kullanıcı listesini döndüren yordam.
        /// </summary>
        /// <param name="aramaBilgisi">Listelenecek kullanıcıların bilgileri tutan Kullanıcı nesnesi.</param>
        /// <returns>Verilen kıstaslara uyan ve basla parametresiyle belirtilen sıradan başlayarak, say parametresi ile belirtilen kadar kayıt döndürülür.</returns>
        ObjectArray<Kullanici> KullaniciRaporla(Kullanici aramaBilgisi);

        /// <summary>
        /// Sisteme giriş kodu veya TC kimlik numarası verilen kullanıcının bilgilerini okuyup döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="kullaniciKoduVeyaTCKimlikNo">Bilgileri döndürülecek kullanıcının sisteme giriş kodu veya TC kimlik numarası.</param>
        /// <returns>Sisteme giriş kodu veya TC kimlik numarası verilen kullanıcının bilgileri döndürülür.</returns>
        KullaniciVAN KullaniciOku(Kullanici kullanan, string kullaniciKoduVeyaTCKimlikNo);

        /// <summary>
        /// "kullanici" parametresi ile verilen kullanıcı bilgilerini veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="kullanici">Kaydedilecek kullanıcının bilgilerini tutan parametre.</param>
        /// <param name="yeniMi">Doğru ise daha önce sistemde kayıtlı olmayan bir kullanıcının verilerinin kaydedildiği yordama söylenmiş olur.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc KullaniciKaydet(Kullanici kullanan, Kullanici kullanici, bool yeniMi);

        /// <summary>
        /// "kullanici" parametresi ile verilen kullanıcıyı pasif yapan yordam. Kullanıcıyı pasif yapan kullanıcı, pasif yapılan kullanıcıyı yönetebilmelidir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="kullanici">Pasif yapılacak kullanıcının bilgilerini tutan parametre.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc KullaniciyiPasifYap(Kullanici kullanan, Kullanici kullanici);

        /// <summary>
        /// "kullanici" parametresi ile verilen kullanıcıyı aktif yapan yordam. Kullanıcıyı aktif yapan kullanıcı, aktif yapılan kullanıcıyı yönetebilmelidir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="kullanici">Aktif yapılacak kullanıcının bilgilerini tutan parametre.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc KullaniciyiAktifYap(Kullanici kullanan, Kullanici kullanici);

        /// <summary>
        /// Kullanıcı için, yetki kontrolleri yapıldıktan sonra yeni parola üretilen ve kullanıcıya mail atan yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="kullanici">Sistem tarafından üretilmiş parola atanacak kullanıcı.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ParolaAta(Kullanici kullanan, Kullanici kullanici);

        /// <summary>
        /// 6 basamaklı yeni parola üreten ve ürettiği parolayı sonuc yapısının anahtar alanında döndüren yordam.
        /// </summary>
        /// <returns>Üretilen parola sonuc yapısının anahtar alanında döndürülür.</returns>
        Sonuc ParolaUret();

        /// <summary>
        /// Kullanıcı nesnesinde bilgileri verilen kullanıcı için ad ve soyad alanlarını kullanarak, olası kullancı adları üreten ve döndüren yordam.
        /// </summary>
        /// <param name="kullanici">Kullanıcı adı üretilecek kullanıcıya ait bilgileri tutan parametre.</param>
        /// <returns>Üretilen kullanıcı adları liste olarak döndürülür.</returns>
        ObjectArray<Kullanici> KullaniciAdOner(Kullanici kullanici);

        /// <summary>
        /// Sisteme giriş kullanıcı kodu ve parola ile, eğer verilen kod ve parola geçerli ise ve kullanıcı aktif ise kullanıcı sisteme girişine izin verilir ve bilgileri döndürülür.
        /// </summary>
        /// <param name="kullaniciKodu">Sisteme giriş kullanıcı kodu.</param>
        /// <param name="parola">Sisteme giriş parolası.</param>
        /// <returns>Kullanıcı kodu ve şifre geçerli ise ve kullanıcı aktif ise, aktarım nesnesinin kullanıcı alanı kullanıcı bilgileri doldurulur ve sonuc yapısının islemSonuc alanına Doğru atanır. Aksi takdirde sonuc yapısının islemSonuc alanına Yanlış atanır.</returns>
        KullaniciVAN GirisYap(string kullaniciKodu, string parola);

        /// <summary>
        /// Sisteme giriş kullanıcı kodu ve parolası ile, eğer verilen kod ve parola geçerli ise ve kullanıcı aktif ise kullanıcının bilgileri döndürülür.
        /// </summary>
        /// <param name="kullaniciKodu">Sisteme giriş kullanıcı kodu.</param>
        /// <param name="parola">Sisteme giriş parolası.</param>
        /// <param name="sifreli">Gönderilen parola açık mı şifreli mi bilgisi.</param>
        /// <returns>Kullanıcı kodu ve şifre geçerli ise ve kullanıcı aktif ise, aktarım nesnesinin kullanıcı alanı kullanıcı bilgileri doldurulur ve sonuc yapısının islemSonuc alanına Doğru atanır. Aksi takdirde sonuc yapısının islemSonuc alanına Yanlış atanır.</returns>
        KullaniciVAN GirisYapabilirMi(string kullaniciKodu, string parola, bool sifreli);

        /// <summary>
        /// Verilen parola doğruysa, yeni parolalar aynıysa ve yeni parolaların uzunluğu 4 karakterden fazla ise, kullanıcının parolası değiştirilir ve yeni parola değiştirme zamanı atanır.
        /// </summary>
        /// <param name="kullanan">Parolası değiştirilecek kullanıcı.</param>
        /// <param name="eskiParola">Kullanıcının şu andaki parolası.</param>
        /// <param name="yeniParola1">Kullanıcının olması istenen parolası.</param>
        /// <param name="yeniParola2">Kullanıcının olması istenen parolası.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ParolaDegistir(Kullanici kullanan, string eskiParola, string yeniParola1, string yeniParola2);

        /// <summary>
        /// "kullanan" parametresi ile verilen kullanıcının, "kullanici" parametresi ile verilen kullanıcıyı yönetip yönetemediğini bulup, döndüren yordam.
        /// </summary>
        /// <param name="kullanan">Yönetme hakkına sahip mi diye kontrol edilecek kullanıcı parametresi.</param>
        /// <param name="kullanici">Yönetilip yönetilmediği bulunacak kullanıcı parametresi.</param>
        /// <returns>"kullanan", "kullanici"yi yönetiyorsa Doğru, değilse Yanlış döndürülür.</returns>
        bool YonetiyorMu(Kullanici kullanan, Kullanici kullanici);

        /// <summary>
        /// TC kimlik numarası verilen kullanıcı varsa adı ve soyadı, yoksa boş dizgi döndüren yordam. 
        /// </summary>
        /// <param name="tcKimlikNo">Kullanıcı listesinde var olup olmadığı aranacak kullanıcının TC kimlik numarası.</param>
        /// <returns>TC kimlik numarası verilen kişi kayıtlı kullanıcı ise, adı ve soyadı dizgi olarak yoksa boş dizgi döndürülür..</returns>
        string KullaniciVarMi(string tcKimlikNo);

        /// <summary>
        /// TC Kimlik numarası verilen kullanıcı bulunur ve Parola ata yöntemi çağrılarak, kullanıcıya yeni üretilmiş parola atınır.
        /// </summary>
        /// <param name="tcKimlikNo">Yeni üretilmiş parola atanacak kullanıcının TC kimlik numarası.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc KullaniciyaYeniParolaAta(string tcKimlikNo);

        /// <summary>
        /// TC Kimlik numarası verilen kullanıcının bilgileri mernis bağlantısı ile alan ve kullanıcı veri aktarım nesnesi ile döndüren yordam.
        /// </summary>
        /// <param name="tcKimlikNo">Mernis bağlantısı ile bilgileri bulunacak kişinin TC kimlik numarası.</param>
        /// <returns>TC kimlik numarası geçerli ise  aktarım nesnesinin kullanıcı alanına kullanıcı bilgileri doldurulur ve sonuc yapısının islemSonuc alanına Doğru atanır. Aksi takdirde sonuc yapısının islemSonuc alanına Yanlış atanır.</returns>
        KullaniciVAN MernisBilgiOku(Kullanici kullanan, string tcKimlikNo, TNSDateTime dogumTarih);
        Sonuc TCKimlikNoKontrolu(Kullanici kullanan, string tcKimlikNo, TNSDateTime dogumTarih);
        bool MerniseBagliMi();

        /// <summary>
        /// Mernis sistemine kayıtlı olan ilçe bilgilerini alma işlemini yapar
        /// </summary>
        //void MernisIlIlceOku();

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
        Sonuc KullaniciSessionAta(string kullaniciKodu, TNSDateTime tarih, string hostName, string sayfaAd, string bilgi1, string bilgi2, string bilgi3, string bilgi4, string bilgi5);

        /// <summary>
        /// Web uygulamasının oturum verisini KYMOTURUM tablosuna yazar (varsa günceller).
        /// </summary>
        /// <param name="oturumNo">Oturumun rastgele üretilmiş numarası</param>
        /// <param name="kullaniciKodu">Oturumun sahibi kullanıcı</param>
        /// <param name="veri">Oturum verisi (JSON)</param>
        /// <param name="sureDakika">Oturumun, işlem yapılmazsa kaç dakika sonra düşeceği</param>
        Sonuc OturumYaz(string oturumNo, string kullaniciKodu, string veri, int sureDakika);

        /// <summary>
        /// Oturum verisini okur ve oturumun süresini sureDakika kadar uzatır. Oturum yoksa ya da süresi dolmuşsa null döner.
        /// </summary>
        string OturumOku(string oturumNo, int sureDakika);

        /// <summary>
        /// Oturumu siler (çıkış).
        /// </summary>
        Sonuc OturumSil(string oturumNo);

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
        ObjectArray<KullaniciIslemYapan> KullaniciSessionListele(string kullaniciKodu, int sure, string hostName, string sayfaAd, string bilgi1, string bilgi2, string bilgi3, string bilgi4, string bilgi5);

        /// <summary>
        /// Sisteme erişim bilgilerinin verilen zaman aralığındaki sayılarını, verilen tarih aralığında listeleyen yordam. 
        /// </summary>
        /// <param name="kullaniciKodu">Verile bir kullanıcıya ait veriler listelenir.</param>
        /// <param name="grafikZamanAraligi">Erişim sayılarının hangi sıklıkta raporlanacağı</param>
        /// <param name="tarih1">Erişim sayılarının raporlanacağı zaman diliminin başlangıcı.</param>
        /// <param name="tarih2">Erişim sayılarının raporlanacağı zaman diliminin bitişi.</param>
        /// <param name="sayfaAd">Raporlanacak sayfaların içerdiği dizgi.</param>
        /// <returns>Verilen zaman aralığındaki her bir zaman için oluşan erişim sayısı listesi KullaniciIslemYapan sınıfının islem tarih ve adet alanlarında döndürülür.</returns>
        ObjectArray<KullaniciIslemYapan> KullaniciSessionGrafik(string kullaniciKodu, ENUMKullanimBilgiZaman grafikZamanAraligi, TNSDateTime tarih1, TNSDateTime tarih2, string sayfaAd);

        /// <summary>
        /// Bir kullanıcının, bir değişkeninin değerlerini döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Değişken değerleri bulunacak kullanıcının sisteme giriş kodu.</param>
        /// <param name="degisken">Kullanıcının değerleri bulunacak değişkeninin ismi.</param>
        /// <returns>Kullanıcı değişkeninin değerleri liste olarak döndürülür.</returns>
        TNSCollection<string> DegiskenleriListele(string kullaniciKodu, string degisken);

        /// <summary>
        /// Kullanıcı değişkenininin değerleri, verilen değerler yapan yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Değişken değerleri saklanacak kullanıcının kodu.</param>
        /// <param name="degisken">Değerleri saklanacak değişkenin ismi.</param>
        /// <param name="degerler">Saklanacak değişkenin değerleri.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc DegiskenKaydet(string kullaniciKodu, string degisken, TNSCollection degerler);

        /// <summary>
        /// İçişleri bakanlığı ile entegrasyon için eklenen yordam. Giriş ekranına iç işleri uygulaması tarafından verilen session numarası kullanılarak içişleri bakanlığı web servisi çağıran ve servisten gelen kullanıcı bilgilerini döndürüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="kullaniciKodu">Bilgileri döndürülecek kullanıcının sisteme giriş kodu.</param>
        /// <param name="sessionID">Giriş ekranına iç işleri uygulaması tarafından verilen oturum (session) numarası</param>
        /// <param name="giris">Doğru ise yordam sisteme giriş yapılırken çağrılıyordur. Yanlış ise kullanıcı bilgilerini görüntülemek için çağrılıyordur.</param>
        /// <returns>Bilgileri verilen kullanıcı okunmuşsa aktarım nesnesinin kullanıcı alanında döndürülür. Okunamamışsa aktarım nesnesinin sonuc alanında okuyamama nedeni döndürülür.</returns>
        KullaniciVAN KullaniciOkuWebServisten(Kullanici kullanan, string kullaniciKodu, string sessionID, bool giris);

        /// <summary>
        /// İçişleri bakanlığı entegrasyonu ile sisteme giriş yapılmış ise, giriş yapan kullanıcının son giriş tarihini güncellemek için çağrılan yordam.
        /// Ayrıca kullanıcı pasif ise, hata döndürülür ve giriş yapılmasına izin verilmez.
        /// </summary>
        /// <param name="kullanici">Son giriş tarihi güncellenecek ve aktifliği kontrol edilecek kullanıcı parametresi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc GirisYapSession(Kullanici kullanici);

        /// <summary>
        /// Kullanıcı giriş yaptığında, giriş sayfasında gösterilecek olan dururuların listesini verilen kıstaslara göre döndüren yordam.
        /// </summary>
        /// <param name="aramaBilgisi">Döndürülecek duyuruların uyacağı kriter bilgisini tutan duyuru nesnesi.</param>
        /// <returns>Verilen kriterlere uyan duyurular döndürülür.</returns>
        ObjectArray<Duyuru> DUYListesi(Kullanici kullanan, Duyuru aramaBilgisi, bool modulDuyurulari);

        /// <summary>
        /// Kodu verilen duyurunun bilgilerini döndüren yordam.
        /// </summary>
        /// <param name="duyuruKodu">Döndürülecek duyurunun kodu.</param>
        /// <returns>Kodu verilen duyuru döndürülür.</returns>
        Duyuru DUYOku(int duyuruKodu);

        /// <summary>
        /// Bilgileri verilen duyuru nesnesini veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="duyuru">Duyuru bilgilerini tutan ve kaydedilecek nesne.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc DUYKaydet(Kullanici kullanan, Duyuru duyuru);

        Sonuc MesajKaydet(Mesaj mesaj);

        Mesaj[] MesajListele(Mesaj mesaj);

        /// <summary>
        /// Kodu verilen duyuru bilgilerini veri tabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="duyuruKodu">Silinecek duyurunun kodu.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc DUYSil(Kullanici kullanan, int duyuruKodu);

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
        Sonuc EPostaKaydet(Kullanici kullanan, string konu, string mesaj, int ktKod);

        /// <summary>
        /// Sisteme kaydedilmiş mesajları ve bu mesajların kaç kişiye gönderileceğini ve şu ana kadar kaç kişiye gönderildiğini listeleyen yordam.
        /// </summary>
        /// <returns>Sisteme kaydedilmiş mesajlar ve bu mesajların kaç kişiye gönderileceği ve şu ana kadar kaç kişiye gönderildiği bilgisi Posta nesnesi listesi olarak döndürülür.</returns>
        ObjectArray<Posta> EPostaGonderOncekiGonderiler();

        /// <summary>
        /// "gonderildiMi" parametresinin değerine göre bir mesajla ilgili tüm kullanıcıları, gönderilenleri veya gönderilmeyenleri listeleyen yordam. 
        /// </summary>
        /// <param name="mesajId">Kullanıcıları döndürülecek elektronik postanın numarası.</param>
        /// <param name="gonderildiMi">-1 ise tüm kullanıcılar, 1 ise gönderilenler, 0 ise gönderilmeyenler listenir.</param>
        /// <returns>Bir mesajın gönderildiği/gönderilmediği/ilişkili tüm kullanıcıların bilgisi Posta nesnesi listesi olarak döndürülür.</returns>
        ObjectArray<Posta> EPostaGonderilenlerListele(int mesajId, int gonderildiMi);

        /// <summary>
        /// Bir elektronik postayı kullanıcılara göndermek için başlatılmış thread, tüm kullanıcılara mail atamadan sonlanmışsa, 
        /// verilen elektronik posta numarası için thread'i tekrar başlatan yordam. Daha önceki çalışmada posta atılamayan kullanıcılara posta atılır.
        /// </summary>
        /// <param name="mesajId">Posta atma işlemi tekrar başlatılacak yordam.</param>
        void EPostaGonderTekrar(int mesajId);

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
        string EPostaGonder(string kimden, string kime, string konu, string icerik, bool html, bool reply, TNS.TNSCollection ekDosyaIsimleri);

        /// <summary>
        /// Konfigürasyon dosyasında tanımlanmış olan "HataDosyasi" klasöründeki dosya bilgilerini ';' karakteri ile birleştirip döndüren yordam.
        /// Dosya bilgisi ise '|' karekteri ile ayrılmış değiştirilme zamanı, dosya büyüklüğü ve dosya adı bilgilerinden oluşmaktadır.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <returns>"HataDosyasi" klasöründeki dosya bilgileri ';' ayracı ile döndürülür.</returns>
        ObjectArray<EkliDosyalar> HataDosyaListele(Kullanici kullanan);

        /// <summary>
        /// Konfigürasyon dosyasında tanımlanmış olan "HataDosyasi" klasör yoluna, verilen dosya adı eklenerek tam yolu bulunan dosyayı okuyan ve byte dizisi olarak döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="dosyaAd">İçeriği döndürülecek dosyanın ismi.</param>
        /// <returns>Verilen dosyanın içeriği byte dizisi olarak döndürülür.</returns>
        byte[] HataDosyaVer(Kullanici kullanan, string dosyaAd);

        /// <summary>
        /// Konfigürasyon dosyasında tanımlanmış olan "HataDosyasi" klasör yoluna, verilen dosya adı eklenerek tam yolu bulunan dosyayı silen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="dosyaAd">Silinecek dosyanın ismi.</param>
        void HataDosyaSil(Kullanici kullanan, string dosyaAd);

        /// <summary>
        /// Filtre nesnesi ile geçirilen bilgileri veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="filtre">Kaydedilecek filtre nesnesi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc FiltreKaydet(Filtre filtre);

        /// <summary>
        /// Verilen filtre bilgilesi veri tabanından silinir.
        /// </summary>
        /// <param name="filtre">Veri tabanından silinecek filtre nesnesi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc FiltreSil(Filtre filtre);

        /// <summary>
        /// Verilen kıstasa uyan filtrelerin listesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler..</param>
        /// <param name="grupAd">Listelenecek filtrelerin filtre grubu.</param>
        /// <param name="siraNo">0 ise grupdaki tüm kayıtlı filtreler, 0'dan farklı ise verilen sıradaki filtre kaydı okunur.</param>
        /// <returns>Verilen filtre kıstasına uyan filtrelerin listesi</returns>
        Filtre[] FiltreListele(Kullanici kullanan, string grupAd, int siraNo);

        Hashtable RaporListele();

        /// <summary>
        /// Veri tabanı güncelleme işleminde veri tabanı bağlantı bilgileri verir.
        /// </summary>
        /// <param name="kullanan">Kullananıcı bilgisi.</param>
        /// <param name="veriErisimAd">veri tabanı erişim bilgisi.</param>
        /// <returns></returns>
        string VeriErisimDegeriVer(Kullanici kullanan, string veriErisimAd);

        Mevzuat[] MevzuatListele(Mevzuat mevzuat, TNSDateTime islemTarih);
        Sonuc MevzuatKaydet(Mevzuat[] mevzuat);
        Sonuc MevzuatSil(Mevzuat[] mevzuat);
        Sonuc MevzuatKilitIslem(Mevzuat mevzuat, TNSDateTime islemTarih, bool kilit);

        MevzuatDurum MevzuatListeleSonDurum(string mevzuatKod);

        MevzuatSayfa MevzuatListeleSayfa(string msKod);
        Sonuc MevzuatKaydetSayfa(MevzuatSayfa[] ms);
        Sonuc MevzuatSilSayfa(MevzuatSayfa ms);

        /// <summary>
        /// AnketSoru nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AnketSoru yapısında AnketSoru: bilgilerini tutan değişken</param>
        AnketSoru[] AnketSoruListele(Kullanici kullanan, AnketSoru form);

        /// <summary>
        /// AnketSoru nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AnketSoru yapısında AnketSoru: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc AnketSoruKaydet(Kullanici kullanan, AnketSoru form);

        string[] AnketSoruAnahtarKelimeler(int anahtarNo, string kistas);

        Sonuc AnketSoruDurumDegistir(Kullanici kullanan, AnketSoru soru);

        Sonuc AnketBilgiKaydet(Kullanici kullanan, AnketBilgi anketBilgi);

        AnketBilgi AnketBilgiOku(Kullanici kullanan, AnketBilgi anketKistasBilgi, bool seceneklerDahil, bool cevaplariOku);

        AnketBilgi[] AnketBilgiListele(Kullanici kullanan, AnketBilgi anketKistasBilgi);

        Sonuc AnketBilgisineSoruKaydet(Kullanici kullanan, AnketBilgi anketBilgi);

        Sonuc AnketCevaplariKaydet(Kullanici kullanan, AnketBilgi anketBilgi);

        Sonuc AnketCevaplariSifirla(Kullanici kullanan, int kod);

        Sonuc AnketDavetiGonder(Kullanici kullanan, string modul, string anketAnahtar, int orgBirimKod, string cevapAnahtar, string mailAdres);

        Sonuc AnketDavetiGecerliMi(string modul, string anahtar, int orgBirimKod, string cevapAnahtar, string mailAdres, string onayKod);

        AnketBilgi AnketIstatistikBul(Kullanici kullanan, AnketBilgi anketKistasBilgi);

        AnketCaprazBilgi AnketCaprazIstatistikBul(Kullanici kullanan, AnketBilgi anketKistasBilgi);

        Sonuc LogoKaydet(Kullanici kullanan, Logo bilgi);

        Sonuc LogoSil(Kullanici kullanan, Logo[] bilgiler);

        ObjectArray LogoListele(Kullanici kullanan, Logo bilgi);

        Sonuc LogoTabloOlustur(Kullanici kullanan);

        Kullanici[] KullaniciYetkiTipleriOku(string[] kullaniciKod, int yetkiTipKod);

        Sonuc BuyukMetinListeSakla(Kullanici kullanan, string modulKod, string dilKod, TNSCollection liste);
        Sonuc BuyukMetinSakla(Kullanici kullanan, string modulKod, string anahtar, string dilKod, string metin);

        string BuyukMetinOku(Kullanici kullanan, string modulKod, string anahtar, string dilKod);

        /// <summary>
        /// Anahtar ile başlayan tüm metinler okunur.
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="modulKod"></param>
        /// <param name="anahtar"></param>
        /// <param name="dilKod"></param>
        /// <returns></returns>
        ObjectArray<TNS.StrKodStrAd> BuyukMetinListele(Kullanici kullanan, string modulKod, string anahtar, string dilKod);

        KullaniciVAN KullaniciOku(string kullaniciKoduVeyaTCKimlikNo);

        /// <summary>
        /// Cografya listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Cografya yapısında Cografya:  bilgilerini tutan değişken</param>
        Cografya[] ListeleCografya(Kullanici kullanan, Cografya form);

        /// <summary>
        /// Cografya nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Cografya yapısında Cografya:  bilgilerini tutan değişken</param>
        Cografya OkuCografya(Kullanici kullanan, Cografya form);

        /// <summary>
        /// Cografya nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Cografya yapısında Cografya:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetCografya(Kullanici kullanan, Cografya form);

        /// <summary>
        /// YoneticiBildirimFormu listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YoneticiBildirimFormu yapısında YoneticiBildirimFormu: Yonetici Bildirim Formu bilgilerini tutan değişken</param>
        YoneticiBildirimFormu[] ListYoneticiBildirimFormu(Kullanici kullanan, YoneticiBildirimFormu form);

        /// <summary>
        /// YoneticiBildirimFormu nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YoneticiBildirimFormu yapısında YoneticiBildirimFormu: Yonetici Bildirim Formu bilgilerini tutan değişken</param>
        YoneticiBildirimFormu GetYoneticiBildirimFormu(Kullanici kullanan, YoneticiBildirimFormu form);

        /// <summary>
        /// YoneticiBildirimFormu nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YoneticiBildirimFormu yapısında YoneticiBildirimFormu: Yonetici Bildirim Formu bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SaveYoneticiBildirimFormu(Kullanici kullanan, YoneticiBildirimFormu[] formlar, ENUMYoneticiBildirimFormuDurum durum);

        Sonuc DurumGuncelleYoneticiBildirimFormu(Kullanici kullanan, YoneticiBildirimFormu[] form, ENUMYoneticiBildirimFormuDurum durum, int genelGorevKod);

        /// <summary>
        /// KullaniciMesaj listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KullaniciMesaj yapısında KullaniciMesaj:  bilgilerini tutan değişken</param>
        KullaniciMesaj[] ListeleMesajPanosu(Kullanici kullanan, KullaniciMesaj form);

        /// <summary>
        /// KullaniciMesaj nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="mesajlar">KullaniciMesaj yapısında KullaniciMesaj:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetMesajPanosu(Kullanici kullanan, KullaniciMesaj[] mesajlar);

        ListeVAN MuhasebeListesi();
        MuhasebeVAN MuhasebeOku(string kod);
        MuhasebeVAN MuhasebeOkuAd(string ad);
        Sonuc MuhasebeKaydet(Kullanici kullanan, Muhasebe muhasebe);
        Sonuc MuhasebeSil(Kullanici kullanan, string kod);
        bool MernisKontroluYapma();

        /// <summary>
        /// EvrakAktarim listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EvrakAktarim yapısında EvrakAktarim:  bilgilerini tutan değişken</param>
        ObjectArray<EvrakAktarimMeta> ListeleEvrakAktarim(Kullanici kullanan, string evrakKodu);

        /// <summary>
        /// EvrakAktarim nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="evrakKodu">Evrak kodu bilgisini tutan değişken</param>
        /// <param name="degerler">EvrakAktarim yapısında bilgileri tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetEvrakAktarim(Kullanici kullanan, string evrakKodu, TNSCollection degerler);

        /// <summary>
        /// BirimMesajAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BirimMesajAna yapısında BirimMesajAna: Birim Mesaj Ana bilgilerini tutan değişken</param>
        ObjectArray<BirimMesaj> ListeleBirimMesajlari(Kullanici kullanan, BirimMesaj form);

        /// <summary>
        /// BirimMesajAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BirimMesajAna yapısında BirimMesajAna: Birim Mesaj Ana bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetBirimMesaji(Kullanici kullanan, BirimMesaj degerler);

        string WebConfigOku(string degiskenAd, string defDeger);

        Sonuc DurumDegistirBirimMesaji(Kullanici kullanan, TNSCollection mesajlar);

        int BirimMesajiSayisiBul(Kullanici kullanan, BirimMesaj form);

        /// <summary>
        /// EkliDosyalar listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EkliDosyalar yapısında EkliDosyalar: Sistemde tutulan dosya bilgileri bilgilerini tutan değişken</param>
        ObjectArray<EkliDosyalar> ListeleEkliDosyalar(Kullanici kullanan, EkliDosyalar form, bool birimAdOlsun);

        /// <summary>
        /// EkliDosyalar nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EkliDosyalar yapısında EkliDosyalar: Sistemde tutulan dosya bilgileri bilgilerini tutan değişken</param>
        ObjectVAN<EkliDosyalar> OkuEkliDosyaIcerik(Kullanici kullanan, EkliDosyalar form);

        /// <summary>
        /// EkliDosyalar nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EkliDosyalar yapısında EkliDosyalar: Sistemde tutulan dosya bilgileri bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetEkliDosya(Kullanici kullanan, EkliDosyalar form);
        Sonuc SilEkliDosyalar(Kullanici kullanan, EkliDosyalar form);

        ObjectArray<YerleskeOda> YerleskeOdaListele(Kullanici kullanan, YerleskeOda yerleskeOda);
        Sonuc YerleskeOdaKaydet(Kullanici kullanan, YerleskeOda yerleskeOda);
        Sonuc YerleskeOdaUstKodGuncelle(Kullanici kullanan, YerleskeOda yerleskeOda);
        YerleskeOda YerleskeOdaOku(Kullanici kullanan, int odaKod);

        Sonuc DBHataKaydet(string kullaniciKodu, string tur, string hata, string ipAdresi);

        ObjectArray<Kullanici> KullaniciIslemTarihceListele(Kullanici kullanan, TNSDateTime tarih1, TNSDateTime tarih2, string islemYapilanTCNo, string islemYapilanAd, string islemiYapanKullaniciAdi, int kullaniciTipi);
        Sonuc IISLogYaz(Kullanici kullanan, int sayac, TNSCollection logSatir);
        string Ping(Kullanici kullanan, int tur);
        Sonuc OnaylaOnayKaldir(Kullanici kullanan, EkliDosyalar form, bool onayla);
        void IzlemeKaydet(string ipAddress, string kullaniciKodu, ENUMIzleme izlemeKod, string modulKod, string sayfaAd, string ekBilgi);
    }

    public interface IYetki
    {
        Sonuc YetkiKontrolu(Kullanici kullanan, ENUMIslemTuru islemTur, string oncekiKaydeden, int saymanlikKod, string birimKod, string kurumsalKod, string ekBilgi);
    }
}