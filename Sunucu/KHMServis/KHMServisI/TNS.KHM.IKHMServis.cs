using System;
using System.Collections.Generic;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH;

namespace TNS.KHM
{
    /// <summary>
    /// Kesin Hesap sınıfı için; istemci ve sunucuda ortak kullanılacak statik fonksiyonlar içeren sınıf
    /// </summary>
    [Serializable()]
    public static class Arac
    {
        /// <summary>
        /// Bu sunucu sınıfının nesnesini, konfigürasyon dosyasından okuduğu
        /// remoting dizgisini kullanarak, .net remoting ile oluşturup döndüren statik yordam
        /// </summary>
        /// <returns>.net remoting ile oluşturulan nesne</returns>
        public static IKHMServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("KHMServisURL");
            IKHMServis servis = TNS.Api.ServisIstemci.Olustur<IKHMServis>(sUrl);
            return servis;
        }
    }

    public enum enumBulguDurum : int
    {
        Tanimsiz = 0,
        OnTaslakBulgu = 1,
        TaslakBulgu = 2,
        Bulgu = 3,
        TaslakBulguKapandi = 6,
        OnBulguKapandi = 7,
        BulguKapandi = 8,

        Iptal = 9,

        AcikBulgular = 101,
        KapaliBulgular = 102,
        IptalHaric = 103,
    }

    public enum enumBulguButceTur : int
    {
        Tanimsiz = 0,
        GenelButce = 1,
        DonerSermaye = 2
    }

    public enum enumBulguTur : int
    {
        Tanimsiz = 0,
        DenetimGorusu = 1,
        DigerBulgu = 2,
        IdareDonerSermaye = 3,
        DigerDonerSermaye = 4
    }

    [Serializable()]
    public class BirimGirisIzni
    {
        public MuhasebeBirim birim = new MuhasebeBirim();
        public bool girisIzni { get; set; } = false;
    }

    [Serializable()]
    public class SayBulgu
    {
        public static string BulguTurAdBul(enumBulguTur bulguTurKod)
        {
            switch (bulguTurKod)
            {
                case enumBulguTur.DenetimGorusu: return "Denetim Görüşünün Dayanağı Bulgular";
                case enumBulguTur.DigerBulgu: return "Diğer Bulgular";
                case enumBulguTur.IdareDonerSermaye: return "İdare Döner Sermaye İşletmesi";
                case enumBulguTur.DigerDonerSermaye: return "Diğer Döner Sermaye İşletmeleri";
                default: return "";
            }
        }

        public static string ButceTurAdBul(enumBulguButceTur butceTurKod)
        {
            switch (butceTurKod)
            {
                case enumBulguButceTur.GenelButce: return "Genel Bütçe";
                case enumBulguButceTur.DonerSermaye: return "Döner Sermaye";
                default: return "";
            }
        }

        public static string DurumAdBul(enumBulguDurum durumKod)
        {
            switch (durumKod)
            {
                case enumBulguDurum.OnTaslakBulgu:
                    return "Ön Taslak Bulgu";
                case enumBulguDurum.TaslakBulguKapandi:
                    return "Kapanmış Ön Taslak Bulgu";
                case enumBulguDurum.TaslakBulgu:
                    return "Taslak Bulgu";
                case enumBulguDurum.OnBulguKapandi:
                    return "Kapanmış Taslak Bulgu";
                case enumBulguDurum.Bulgu:
                    return "Bulgu";
                case enumBulguDurum.BulguKapandi:
                    return "Kapanmış Bulgu";
                case enumBulguDurum.Iptal:
                    return "İptal";

                case enumBulguDurum.AcikBulgular:
                    return "Açık Bulgular";
                case enumBulguDurum.KapaliBulgular:
                    return "Kapanmış Bulgular";
                case enumBulguDurum.IptalHaric:
                    return "İptal Edilmemişler";

                default:
                    return "";
            }
        }

        public int bulguID { get; set; }
        public int yil { get; set; }
        public int bitisYil { get; set; } = 0;
        public enumBulguButceTur butceTurKod { get; set; } = enumBulguButceTur.GenelButce;
        public string butceTurAd { get { return ButceTurAdBul(butceTurKod); } }

        public enumBulguTur bulguTurKod { get; set; }
        public string bulguTurAd { get { return BulguTurAdBul(bulguTurKod); } }

        public string bulguNo { get; set; }

        public string bulguBasligi { get; set; }
        public string bulguOzeti { get; set; }
        public string bulguDayanagi { get; set; }
        public string idareCevabi { get; set; }
        public string idareCevapOzeti { get; set; }
        public string sayistayOnerisi { get; set; }

        /// <summary> ON: Ön Bulgu, BLG: Bulgu </summary>
        public enumBulguDurum durumKod { get; set; } = enumBulguDurum.TaslakBulgu;
        public string birimKod { get; set; } = "";
        public int muhasebeKod { get; set; } = 0;
        public string kurKod { get; set; } = "";
        public string birimCevap { get; set; } = "";
        public string birimIsler { get; set; } = "";
        public string MuhasebeBirimKurum
        {
            get
            {
                MuhasebeBirim mb = new MuhasebeBirim();
                mb.birimKod = this.birimKod;
                mb.muhasebe = this.muhasebeKod;
                mb.kurum = this.kurKod;
                return mb.MuhasebeBirimKurum;
            }
            set
            {
                MuhasebeBirim mb = new MuhasebeBirim();
                mb.MuhasebeBirimKurum = value;
                this.birimKod = mb.birimKod;
                this.muhasebeKod = mb.muhasebe;
                this.kurKod = mb.kurum;
            }
        }

        public string durumAd
        {
            get
            {
                return DurumAdBul(durumKod);
            }
        }

        /// <summary> Basit iş kuralı doğrulaması (UI öncesi). </summary>
        public void KontrolEt(Sonuc s)
        {
            var hatalar = new List<string>();
            if (yil < 2000 || yil > 2100) s.DegerAta("Yıl 2000–2100 arasında olmalıdır.");
            if (bitisYil != 0)
            {
                if (bitisYil < yil) s.DegerAta("Bitiş yılı, yıldan büyük olmamalıdır.");
            }
            if (this.bulguTurKod == enumBulguTur.Tanimsiz) s.DegerAta("Bulgu türü seçiniz.");
            if (this.butceTurKod == enumBulguButceTur.Tanimsiz) s.DegerAta("Bütçe türü seçiniz.");
            if (string.IsNullOrWhiteSpace(bulguBasligi)) s.DegerAta("Bulgu metni boş olamaz.");
            if (string.IsNullOrWhiteSpace(bulguOzeti)) s.DegerAta("Bulgu özeti boş olamaz.");
        }
        public bool yerineGetirildi { get; set; }
        public bool yerineGetirildiCevap { get; set; } = false;
        public string yapilacakFaaliyetler { get; set; }
        public string ilgiliBirimKurum { get; set; }
        public string gorevOrtagi { get; set; }
        public TNSDateTime ongorulenTamamlanmaTarihi { get; set; } = new TNSDateTime();
        public DateTime ongorulenTamamlanmaTarihiDT { get { return ongorulenTamamlanmaTarihi.Oku(); } set { ongorulenTamamlanmaTarihi.Yaz(value); } }
        public TNSDateTime ongorulenTamamlanmaTarihiCevap { get; set; } = new TNSDateTime();
        public DateTime ongorulenTamamlanmaTarihiCevapDT { get { return ongorulenTamamlanmaTarihiCevap.Oku(); } set { ongorulenTamamlanmaTarihiCevap.Yaz(value); } }
        public string muhasebeBirimAd { get; set; }
    }

    /// <summary>
    /// Yetki kontrollerini düzenleyen fonksiyonları içeren sınıf
    /// </summary>
    [Serializable()]
    public static class Yetki
    {
        static string KesinHesapYT = "Kesin Hesap Yapacağı Birimler";
        static string KesinHesapYetkiYT = "Kesin Hesap Yetkisi";

        /// <summary>
        /// Arguman olarak verilen kullanıcının görme yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <param name="kurum">Yetki kontrolü yapılacak kurum/birim bilgisi.</param>
        /// <returns>Kullanıcının görme yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, string kurum)
        {
            if (kullanan == null) return false;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else
                return kullanan.YetkiTipiDegeriIceriyorMu(KesinHesapYT, kurum, true) && kullanan.YetkiTipiDegeriIceriyorMu(KesinHesapYetkiYT, "1;2;3;4", false);
        }
    }

    /// <summary>
    /// İta Amirleri cetvelinde kullanılan üst bilgileri taşıyan nesne
    /// </summary>
    [Serializable()]
    public class KHItaAmiri
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil = 0;
        /// <summary>
        /// Kurum
        /// </summary>
        public string kurum = "";
        /// <summary>
        /// Derece
        /// </summary>
        public int derece = 0;
        /// <summary>
        /// KHItaBilgileri taşıyan alan
        /// </summary>
        public KHItaBilgileri bilgiler = new KHItaBilgileri();
    }

    /// <summary>
    /// İta Amirleri cetvelinde kullanılan bilgileri taşıyan nesne
    /// </summary>
    [Serializable()]
    public class KHItaBilgileri
    {
        /// <summary>
        /// Harcama birim kodu
        /// </summary>
        public string birim = "";
        /// <summary>
        /// Adı soyadı
        /// </summary>
        public string adSoyad = "";
        /// <summary>
        /// Göreve başlama tarihi
        /// </summary>
        public TNSDateTime baslamaTarihi = new TNSDateTime();
        /// <summary>
        /// Görevden ayrılış tarihi
        /// </summary>
        public TNSDateTime ayrilmaTarihi = new TNSDateTime();
    }

    /// <summary>
    /// Kesin Hesap raporlarında kullanılan tertip bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class KHTertipBilgi
    {
        /// <summary>
        /// Başlangıç Ödeneği
        /// </summary>
        public double bo = 0.0;
        /// <summary>
        /// Kesintili başlangıç ödeneği
        /// </summary>
        public double kbo = 0.0;
        /// <summary>
        /// Kurum içi Eklenen
        /// </summary>
        public double kiciEk = 0.0;
        /// <summary>
        /// Kurum içi düşülen
        /// </summary>
        public double kiciDus = 0.0;
        /// <summary>
        /// Kurumlar arası eklenen
        /// </summary>
        public double karaEk = 0.0;
        /// <summary>
        /// Kurumlar arası düşülen
        /// </summary>
        public double karaDus = 0.0;
        /// <summary>
        /// Yedekten eklenen
        /// </summary>
        public double yedekEk = 0.0;
        /// <summary>
        /// Yedekten düşülen
        /// </summary>
        public double yedekDus = 0.0;
        /// <summary>
        /// Ödenek Ekleme
        /// </summary>
        public double odenekEkle = 0.0;
        /// <summary>
        /// Özel ödenek
        /// </summary>
        public double ozelOdenek = 0.0;
        /// <summary>
        /// Yıl içinde iptal edilen
        /// </summary>
        public double yiliciIptal = 0.0;
        /// <summary>
        /// Harcama
        /// </summary>
        public double harcama = 0.0;
        /// <summary>
        /// Ödenek üstü rakamı
        /// </summary>
        public double odenekUstu = 0.0;
        /// <summary>
        /// Yıl sonunda iptal edilen
        /// </summary>
        public double yilSonuIptal = 0.0;
        /// <summary>
        /// Devir rakamı
        /// </summary>
        public double devir = 0.0;
        /// <summary>
        /// Akreditif
        /// </summary>
        public double akreditif = 0.0;
        /// <summary>
        /// Akreditif devir rakamı
        /// </summary>
        public double akreditifdevir = 0.0;
        /// <summary>
        /// Taahhüt devir rakamı
        /// </summary>
        public double taahhutdevir = 0.0;
        /// <summary>
        /// Dış proje devir rakamı
        /// </summary>
        public double disprojedevir = 0.0;
        /// <summary>
        /// Bağış devir rakamı
        /// </summary>
        public double bagisdevir = 0.0;
        /// <summary>
        /// özel devir rakamı
        /// </summary>
        public double ozeldevir = 0.0;
        /// <summary>
        /// Diğer devir rakamı
        /// </summary>
        public double digerdevir = 0.0;
        /// <summary>
        /// Gönderilen ödenek
        /// </summary>
        public double gonderilenodenek = 0.0;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama = "";
    }

    /// <summary>
    /// Ödenek bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class KHOdenekBilgi
    {
        /// <summary>
        /// Kesintili başlangıç ödeneği
        /// </summary>
        public double kbo = 0.0;
        /// <summary>
        /// Yıl sonu ödenek toplamı
        /// </summary>
        public double ysot = 0.0;
        /// <summary>
        /// Harcama rakamı
        /// </summary>
        public double harcama = 0.0;
    }

    /// <summary>
    /// Ödenek bilgilerini taşıyan veri aktarım nesnesi
    /// </summary>
    [Serializable]
    public class OdenekBilgileriVAN
    {
        /// <summary>
        /// TNS.BHM.OdenekBilgi tipinde bilgi tutan alan
        /// </summary>
        public TNSCollection detaylar = new TNSCollection();
        /// <summary>
        /// <see cref="Sonuc"/> tipinde bilgi tutan alan
        /// </summary>
        public Sonuc sonuc = new Sonuc();
    }

    /// <summary>
    /// Tertip bilgilerini taşıyan veri aktarım nesnesi
    /// </summary>
    [Serializable()]
    public class KHTertipBilgiVAN
    {
        /// <summary>
        /// <see cref="KHTertipBilgi"/> tipinde bilgi tutan alan
        /// </summary>
        public KHTertipBilgi tertipBilgi = new KHTertipBilgi();
        /// <summary>
        /// <see cref="Sonuc"/> tipinde bilgi tutan alan
        /// </summary>
        public Sonuc sonuc = new Sonuc();
    }

    /// <summary>
    /// Tertip bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class TertipBilgi
    {
        /// <summary>
        /// <see cref="Tertip"/> tipinde bilgi tutan alan
        /// </summary>
        public Tertip tertip;
        /// <summary>
        /// <see cref="KHTertipBilgi"/> tipinde bilgi tutan alan
        /// </summary>
        public KHTertipBilgi tertipBilgi = new KHTertipBilgi();
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama;
    }

    /// <summary>
    /// Tertip ve ödenek bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class TertipBilgiler
    {
        /// <summary>
        /// <see cref="Tertip"/> tipinde bilgi tutan alan
        /// </summary>
        public Tertip tertip;
        /// <summary>
        /// <see cref="KHOdenekBilgi"/> tipinde bilgi tutan alan
        /// </summary>
        public TNSCollection degerler = new TNSCollection();
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama = "";
    }

    /// <summary>
    /// Mizan bilgisini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class MizanBilgi
    {
        /// <summary>
        /// Hesap Kodu
        /// </summary>
        public string hesKod = "";
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama = "";
        /// <summary>
        /// Borç
        /// </summary>
        public double borc = 0.0;
        /// <summary>
        /// Alacak
        /// </summary>
        public double alacak = 0.0;
        /// <summary>
        /// Borç kalanı
        /// </summary>
        public double borc_artik = 0.0;
        /// <summary>
        /// Alacak kalanı
        /// </summary>
        public double alacak_artik = 0.0;
    }

    /// <summary>
    /// Tertip ve ödenek bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class TertipOdenekBilgi
    {
        /// <summary>
        /// <see cref="Tertip"/> tipinde bilgi tutan alan
        /// </summary>
        public Tertip tertip;
        /// <summary>
        /// İşlem
        /// </summary>
        public string islem = "";
        /// <summary>
        /// Ödenek
        /// </summary>
        public double odenek = 0.0;

        /// <summary>
        /// <see cref="TertipOdenekBilgi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TertipOdenekBilgi(int yil)
        {
            tertip = new Tertip(yil, ENUMTabloKod.TABButce);
        }
    }

    /// <summary>
    /// Tertip ve Harcama bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class TertipHarcamaBilgi
    {
        /// <summary>
        /// <see cref="Tertip"/> tipinde bilgi tutan alan
        /// </summary>
        public Tertip tertip;
        /// <summary>
        /// Harcama
        /// </summary>
        public double harcama = 0.0;
        /// <summary>
        /// Mahsup
        /// </summary>
        public double mahsup = 0.0;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama = "";

        /// <summary>
        /// <see cref="TertipHarcamaBilgi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TertipHarcamaBilgi(int yil)
        {
            tertip = new Tertip(yil, ENUMTabloKod.TABButce);
        }
    }

    /// <summary>
    /// Gelir bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class TertipGelirBilgi
    {
        /// <summary>
        /// Tahmin
        /// </summary>
        public double tahmin = 0.0;
        /// <summary>
        /// Artık
        /// </summary>
        public double artik = 0.0;
        /// <summary>
        /// Yılı Tahakkuk
        /// </summary>
        public double yiliTahakkuk = 0.0;
        /// <summary>
        /// Red-İade
        /// </summary>
        public double rediade = 0.0;
        /// <summary>
        /// Net tahakkuk
        /// </summary>
        public double nettahakkuk = 0.0;
        /// <summary>
        /// Net Tahsilat
        /// </summary>
        public double netTahsilat = 0.0;
        /// <summary>
        /// Devir Artık
        /// </summary>
        public double devirArtik = 0.0;
    }

    /// <summary>
    /// Gider gerekçe bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class GiderGerekce
    {
        /// <summary>
        /// <see cref="Tertip"/> tipinde bilgi tutan alan
        /// </summary>
        public Tertip tertip;
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil = 0;
        /// <summary>
        /// Gerekçe
        /// </summary>
        public string gerekce = "";

        /// <summary>
        /// <see cref="GiderGerekce"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        [Obsolete("GiderGerekce metodu yerine, GiderGerekce(yil) metodunu kullanınız.")]
        public GiderGerekce()
        {
            yil = 0;
            tertip = new Tertip(yil, ENUMTabloKod.TABButce);
        }

        /// <summary>
        /// <see cref="GiderGerekce"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public GiderGerekce(int yil)
        {
            this.yil = yil;
            tertip = new Tertip(yil, ENUMTabloKod.TABButce);
        }
    }

    /// <summary>
    /// Gider gerekçe bilgilerini taşıyan veri aktarım nesnesi
    /// </summary>
    [Serializable()]
    public class GiderGerekceVAN
    {
        /// <summary>
        /// <see cref="GiderGerekce"/> tipinde bilgi tutan alan
        /// </summary>
        public GiderGerekce gerekce = new GiderGerekce();
        /// <summary>
        /// <see cref="Sonuc"/> tipinde bilgi tutan alan
        /// </summary>
        public Sonuc sonuc = new Sonuc();
    }

    /// <summary>
    /// Gider gerekçe bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class GiderGerekceDetay
    {
        /// <summary>
        /// <see cref="Tertip"/> tipinde bilgi tutan alan
        /// </summary>
        public Tertip tertip;

        /// <summary>
        /// Gerekçe
        /// </summary>
        public string gerekce = "";

        /// <summary>
        /// <see cref="GiderGerekceDetay"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public GiderGerekceDetay(int yil)
        {
            tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
        }

        public int damgaSay { get; set; }

        public int onayKod { get; set; }
    }

    /// <summary>
    /// Gider gerekçe bilgilerini taşıyan veri aktarım nesnesi
    /// </summary>
    [Serializable()]
    public class GiderGerekceDetayVAN
    {
        /// <summary>
        /// <see cref="GiderGerekce"/> tipinde bilgi tutan alan
        /// </summary>
        public GiderGerekceDetay gerekce = null;
        /// <summary>
        /// <see cref="Sonuc"/> tipinde bilgi tutan alan
        /// </summary>
        public Sonuc sonuc = null;

        public GiderGerekceDetayVAN(int yil)
        {
            sonuc = new Sonuc();
            gerekce = new GiderGerekceDetay(yil);
        }
    }

    /// <summary>
    /// Gelir gerekçe bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class GelirGerekce
    {
        /// <summary>
        /// <see cref="Tertip"/> tipinde bilgi tutan alan
        /// </summary>
        public Tertip tertip;
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil = 0;
        /// <summary>
        /// Gerekçe
        /// </summary>
        public string gerekce = "";

        /// <summary>
        /// <see cref="GelirGerekce"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public GelirGerekce()
        {
            tertip = new Tertip(yil, ENUMTabloKod.TABGelir);
        }
    }

    /// <summary>
    /// Gelir gerekçe bilgilerini taşıyan veri aktarım nesnesi
    /// </summary>
    [Serializable()]
    public class GelirGerekceVAN
    {
        /// <summary>
        /// <see cref="GelirGerekce"/> tipinde bilgi tutan alan
        /// </summary>
        public GelirGerekce gerekce = new GelirGerekce();
        /// <summary>
        /// <see cref="Sonuc"/> tipinde bilgi tutan alan
        /// </summary>
        public Sonuc sonuc = new Sonuc();
    }

    /// <summary>
    /// İşlem kodu
    /// </summary>
    public enum ENUMIslemKod
    {
        /// <summary>
        /// Yıl içi iptal
        /// </summary>
        YIIPTAL = 1,
        /// <summary>
        /// Devir
        /// </summary>
        DEVIR = 2,
        /// <summary>
        /// Akreditif
        /// </summary>
        AKREDITIF = 3,
        /// <summary>
        /// Ödenek üstü
        /// </summary>
        ODENEKUSTU = 4,
        /// <summary>
        /// Yedek
        /// </summary>
        YEDEK = 5,
        /// <summary>
        /// Yıl sonu iptal
        /// </summary>
        YSIPTAL = 6,
        /// <summary>
        /// Özellikli gider
        /// </summary>
        OZELLIKLIGIDERLER = 7,
        /// <summary>
        /// Önceki Devir
        /// </summary>
        ONCEKIDEVIR = 8,
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = -1
    }

    //[Serializable()]
    //public class KHSabitler
    //{
    //    /// <summary>
    //    /// İşlem kodları
    //    /// </summary>
    //    public static int[] IslemKod = { 1, 2, 3, 4, 5, 6, 7, 8 };
    //    /// <summary>
    //    /// İşlem adları
    //    /// </summary>
    //    public static string[] IslemAd = { "Yıl İçi İptal", "Devir", "Akreditif", "OdenekUstu", "Yedek", "Yıl Sonu Iptal", "Özellikli Giderler", "Önceki Devir", "Tanımsız" };
    //}

    /// <summary>
    /// Kesin hesap sunucu servisi tarafından sağlanan hizmetlerinin arayüz sınıfı.
    /// </summary>
    public interface IKHMServis
    {
        /// <summary>
        /// Görme yetkisi var mi kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="kurum">Kurum bilgisi.</param>
        /// <returns></returns>
        bool GormeYetkisiVarMi(Kullanici kullanan, string kurum);
        /// <summary>
        /// Kesin hesap gider bilgilerini hesaplayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="khBilgiler">kaydedilecek TertipBilgi nesnelerini içeren koleksiyon.</param>
        /// <returns></returns>
        Sonuc KesinHesapGiderBilgileriKaydet(Kullanici kullanan, int yil, TNSCollection khBilgiler);
        /// <summary>
        /// Kesin hesap gider cetvelini hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        TNSCollection<TNS.BHMv2.UcgenSatir> KesinHesapGiderCetveliHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip);
        /// <summary>
        /// Kesin hesap fonksiyonel cetvelini hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        TNSCollection KesinHesapFonksiyonelCetvelHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip);
        /// <summary>
        /// Kesin hesap finansal cetvelini hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        TNSCollection KesinHesapFinansalCetvelHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip);
        /// <summary>
        /// Kesin hesap ekonomik cetvelini hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        TNSCollection KesinHesapEkonomikCetvelHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip);
        /// <summary>
        /// Kesin hesap kod cetvelini hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="tur">Tür bilgisi.</param>
        /// <param name="seviye">seviye bilgisi.</param>
        /// <returns></returns>
        TNSCollection<TertipBilgi> KesinHesapKodCetvelHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip, string tur, int seviye);
        /// <summary>
        /// Kesin hesap gider cetvelini (kurumsal) hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="kurumsal">Eğer <c>true</c> kurumsal bazda liste alınır.</param>
        /// <returns></returns>
        TNSCollection<TNS.BHMv2.UcgenSatir> KesinHesapGiderCetveliHazirlaKurumsal(Kullanici kullanan, int yil, int ay, Tertip tertip, bool kurumsal);
        /// <summary>
        /// Kesin hesap gider cetvelini hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="kurumsal">Eğer <c>true</c> kurumsal bazda liste alınır.</param>
        /// <param name="raporTur">Tür bilgisi.</param>
        /// <returns></returns>
        TNSCollection<TNS.BHMv2.UcgenSatir> KesinHesapGiderCetveliHazirlaRaporTur(Kullanici kullanan, int yil, int ay, Tertip tertip, bool kurumsal, ENUMIslemKod raporTur);
        /// <summary>
        /// Kesin hesap Çapraz cetvel hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        TNSCollection<TertipBilgi> KesinHesapCaprazCetvelHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip);
        /// <summary>
        /// Kesin hesap mahsup cetvelini hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="tur">Tür bilgisi.</param>
        /// <param name="seviye">Seviye bilgisi.</param>
        /// <returns></returns>
        TNSCollection<TertipHarcamaBilgi> KesinHesapMahsupCetveliHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip, string tur, int seviye);
        /// <summary>
        /// Devir akreditif (Bütçe İşlemleri) listelesini hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="akreditifMi">Eğer <c>true</c> olursa akreditif işlemi yapılır.</param>
        /// <returns></returns>
        TNSCollection<TertipOdenekBilgi> BIDevirAkreditifListele(Kullanici kullanan, int yil, string[] kodlar, bool akreditifMi);
        /// <summary>
        /// Devir akreditif (Kesin hesap) listesini hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="islem">İşlem bilgisi.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        TNSCollection<TertipOdenekBilgi> KHDevirAkreditifListele(Kullanici kullanan, int yil, string[] islem, string[] kodlar);
        /// <summary>
        /// Devir akreditif (Kesin Hesap) kaydetme işlemini yapan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kurum">Kurum bilgisi.</param>
        /// <param name="detaylar">TertipOdenekBilgi tipinde işlem yapılacak bilgileri taşıyan nesne.</param>
        /// <returns></returns>
        //Sonuc KHDevirAkreditifKaydet(Kullanici kullanan, int yil, string kurum, TNSCollection detaylar);
        /// <summary>
        /// Yıl ici iptal bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="detaylar">TertipOdenekBilgi tipinde işlem yapılacak bilgileri taşıyan nesne.</param>
        /// <returns></returns>
        //Sonuc YilIciIptalKaydet(Kullanici kullanan, int yil, TNSCollection detaylar);
        /// <summary>
        /// Gelir bilgilerini hesaplayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        Sonuc KesinHesapGelirBilgileriHesapla(Kullanici kullanan, int yil, int ay, Tertip tertip);
        /// <summary>
        /// Gelir cetveli hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="dinamikMi">Eğer <c>true</c> dinamik olarak üretilir.</param>
        /// <returns></returns>
        TNSCollection<TNS.BHMv2.UcgenSatir> KesinHesapGelirCetveliHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip, bool dinamikMi);
        /// <summary>
        /// Gider gerekcesi getiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        GiderGerekceVAN GiderGerekceGetir(Kullanici kullanan, int yil, Tertip tertip);
        /// <summary>
        /// Gider gerekcesi listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        ObjectArray<GiderGerekce> GiderGerekceListele(Kullanici kullanan, int yil, Tertip tertip);
        /// <summary>
        /// Gelir gerekcesi getiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        GelirGerekceVAN GelirGerekceGetir(Kullanici kullanan, int yil, Tertip tertip);
        /// <summary>
        /// Gelir gerekçesi kayıt eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="gerekce">İşlem yapılacak bilgi.</param>
        /// <returns></returns>
        Sonuc GelirGerekceKaydet(Kullanici kullanan, int yil, GelirGerekce gerekce);
        /// <summary>
        /// Gider gerekçesi kayıt eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="gerekce">İşlem yapılacak bilgi.</param>
        /// <returns></returns>
        Sonuc GiderGerekceKaydet(Kullanici kullanan, int yil, GiderGerekce gerekce);
        /// <summary>
        /// Gider gerekçesi silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        Sonuc GiderGerekceSil(Kullanici kullanan, int yil, Tertip tertip);
        /// <summary>
        /// Gelir gerekçesi silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        Sonuc GelirGerekceSil(Kullanici kullanan, int yil, Tertip tertip);
        /// <summary>
        /// Kesin hesap mizan raporu hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kurum">Kurum bilgisi.</param>
        /// <param name="gecici">if set to <c>true</c> [gecici].</param>
        /// <returns></returns>
        TNSCollection<TNS.BHMv2.UcgenSatir> KesinHesapMizanHazirla(Kullanici kullanan, int yil, string kurum, bool gecici);
        /// <summary>
        /// Kesin hesap Gider Çok yıllı raporu hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="tur">Tür.</param>
        /// <param name="seviye">Seviye bilgisi.</param>
        /// <param name="kacYil">Kaç yıllık işlem yapılacağı bilgisi.</param>
        /// <returns></returns>
        TNSCollection<TertipBilgiler> KesinHesapGiderCokYilli(Kullanici kullanan, int yil, int ay, Tertip tertip, string tur, int seviye, int kacYil);
        /// <summary>
        /// Gelir cetveli (çok yıllı) raporunu hazirlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <param name="dinamikMi">Eğer <c>true</c> dinamik olarak üretilir.</param>
        /// <param name="kacYil">Kaç yıllık işlem yapılacağı bilgisi.</param>
        /// <returns></returns>
        TNSCollection<TNS.BHMv2.UcgenSatir> KesinHesapGelirCetveliHazirlaCokYilli(Kullanici kullanan, int yil, int ay, Tertip tertip, bool dinamikMi, int kacYil);
        /// <summary>
        /// Devir cetveli raporunu hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        TNSCollection<TNS.BHMv2.UcgenSatir> KesinHesapDevirCetveliHazirla(Kullanici kullanan, int yil, int ay, Tertip tertip);
        /// <summary>
        /// Yil içi iptal listelesi raporunu hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        TNSCollection<TertipOdenekBilgi> YilIciIptalListele(Kullanici kullanan, int yil, string[] kodlar);
        /// <summary>
        /// Gider Tertip deger bilgilerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        ObjectArray<TertipBilgi> GiderTertipDegerListele(Kullanici kullanan, int yil, Tertip tertip);
        /// <summary>
        /// Gider tertiplerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu.</param>
        /// <returns></returns>
        ObjectArray<TNS.BKD.Tertip> GiderTertipListele(Kullanici kullanan, int yil, Tertip tertip);

        Sonuc GiderGerekceSilDetay(Kullanici kullanan, int yil, Tertip tertip);
        ObjectArray<GiderGerekceDetay> GiderGerekceListeleDetay(Kullanici kullanan, int yil, Tertip tertip);
        Sonuc GiderGerekceDetayKaydet(Kullanici kullanan, int yil, GiderGerekceDetay gerekce);
        GiderGerekceDetayVAN GiderGerekceDetayGetir(Kullanici kullanan, int yil, Tertip tertip);

        /// <summary>
        /// İta amirlerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kurum">Kurum bilgisi.</param>
        /// <param name="derece">Derece bilgisi.</param>
        /// <returns></returns>
        TNSCollection<KHItaAmiri> ItaAmirleriListele(Kullanici kullanan, int yil, string kurum, int derece);
        /// <summary>
        /// İta amirlerini kayıt eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="kurum">Kurum bilgisi.</param>
        /// <param name="derece">Derece bilgisi.</param>
        /// <param name="detaylar">KHItaBilgileri tipinde işlem yapılacak bilgileri taşıyan nesne.</param>
        /// <returns></returns>
        Sonuc ItaAmirleriKaydet(Kullanici kullanan, int yil, string kurum, int derece, TNSCollection detaylar);

        string FaaliyetAciklamaBul(Kullanici kullanan, int yil, Tertip tertip);

        Sonuc GiderGerekceDetayOnayla(Kullanici kullanan, int yil, GiderGerekceDetay gerekce);

        ObjectArray<GiderGerekceDetay> FaaliyetAciklamaDamgaSayBul(Kullanici kullanan, Tertip tertip);

        Sonuc FaaliyetAciklamaOnayKaldir(Kullanici kullanan, Tertip kistasTertip);

        /// <summary>
        /// OnBulgu listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBulgu yapısında OnBulgu:  bilgilerini tutan değişken</param>
        ObjectArray<SayBulgu> ListeleOnBulgu(Kullanici kullanan, SayBulgu form);

        /// <summary>
        /// OnBulgu nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBulgu yapısında OnBulgu:  bilgilerini tutan değişken</param>
        ObjectVAN<SayBulgu> OkuOnBulgu(Kullanici kullanan, SayBulgu form);

        /// <summary>
        /// OnBulgu nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">OnBulgu yapısında OnBulgu:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetOnBulgu(Kullanici kullanan, SayBulgu form);
        Sonuc BulguBirimBagKur(Kullanici kullanan, int onBulguId, TNS.MUH.MuhasebeBirim muhasebeBirim);
        ObjectArray<TNS.MUH.MuhasebeBirim> BulguIliskiliBirimListele(Kullanici kullanan, int onBulguId);
        Sonuc BulguBirimBagSil(Kullanici kullanan, int onBulguId, MuhasebeBirim mb);
        ObjectArray<SayBulgu> OnBulguCevapListele(Kullanici kullanan, int yil, string birimKod);
        Sonuc OnBulguCevapKaydet(Kullanici kullanan, SayBulgu onBulgu);
        ObjectArray<BirimGirisIzni> BirimGirisIzniListele(Kullanici kullanan, int yil, int tur);
        Sonuc BirimGirisIzniKaydet(Kullanici kullanan, int yil, int tur, bool izin, TNSCollection birimler);
    }
}
