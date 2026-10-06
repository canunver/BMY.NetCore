using System;
using System.Collections.Generic;
using TNS.MUH;

namespace TNS.IKY
{
    public enum ENUMPERSONELTEL
    {
        TANIMSIZ = 0,
        CEP = 1,
        IS = 2,
        EV = 3,
    }

    public enum EnumKadroKontenjanIslemTur
    {
        Ihdas = 1,
        Tahsis = 2,
        Tenkis = 3,
        Degisiklik = 4
    }

    enum ENUMMEDENIHAL
    {
        EVLI = 1,
        BEKAR = 2,
        TANIMSIZ = 0,
    }

    public enum ENUMCINSIYET
    {
        ERKEK = 2,
        KADIN = 3,
        TANIMSIZ = 0,
    }

    enum ENUMASKERLIK
    {
        TAMAMLADI = 1,
        TECILLI = 2,
        MUAF = 3,
        TANIMSIZ = 0,
    }

    public enum ENUMCOCUKDURUM
    {
        OZ = 1,
        UVEY = 2,
        EVLATEDINILMIS = 3,
        TANIMSIZ = 0,
    }

    public enum ENUMKartRaporTuru
    {
        Hepsi = 1,
        TeslimEdilmeyen = 2,
        Kayiplar = 3,
    }

    public enum ENUMGOREVLENDIRMETUR
    {
        TANIMSIZ = 0,
        EGITIMVEREN = 1,
        EGITIMALAN = 2,
        SINAV = 3,
        VEKALET = 4,
        GOREV = 5,
        KESIF = 6,
        DENETIM = 7,
        DIGER = 8,
    }

    //public enum ENUMIZINTIP
    //{
    //    BELIRSIZ = 0,
    //    YILLIK = 1,
    //    SIHHI = 6,
    //    MAZERET = 10,
    //}

    public enum ENUMTARIHCE
    {
        KISISEL = 1,
        ADRES = 2,
        AILE = 3,
        OGRENIM = 4,
        YABANCIDIL = 5,
        IZINHAREKET = 6,
        //MEMURIYET = 7,
        KADRO = 8,
        EGITIM = 9,
        CEZAODUL = 10,
        MALBILDIRIMI = 11,
        LOJMAN = 12,
        IZINDEVIR = 13,
        HIZLIBILGI = 14,
        KART = 15,
        ASKERLIK = 16
    }

    public enum ENUMTARIHCEISLEMTIP
    {
        KAYDET = 1,
        GUNCELLE = 2,
        SIL = 3,
        YAZDIR = 4,
        LISTELE = 5
    }

    public enum ENUMGOREVDURUM
    {
        TANIMSIZ = 0,
        YENI = 1,
        EVRAKNOALDI = 2,
        ONAY = 5,
        IPTAL = 9,
    }

    public enum ENUMBELGEFORMTIP
    {
        PERSONELAILE = 1,
        PERSONELKURS = 2,
        PERSONELOGRENIM = 3,
        PERSONELDIL = 4,
        PERSONELKADRO = 5,
        PERSONELMEMURIYET = 6,
        PERSONELASKERLIK = 7,
        PERSONELIZIN = 8,
        PERSONELKISISEL = 9,
        PERSONELGOREV = 10,
    }

    /// <summary>
    /// Personel Aile bilgisi.
    /// </summary>
    [Serializable()]
    public class PersonelAile
    {
        public int kod = 0;
        public int personelKod = 0;
        public string tcKimlikNo = "";
        public string ad = "";
        public string soyad = "";
        public TNSDateTime dogumTarihi = new TNSDateTime();
        public ENUMCINSIYET cinsiyet = ENUMCINSIYET.TANIMSIZ;
        public ENUMYakinlikDerecesi yakinlik = 0;
        public int durum = 0;
        public int bakmaklaYukumlu = 0;
        public int sakatlikDerecesi = 0;
        public int agi = 0;
        public TNSDateTime yukumlulukBitisTarihi = new TNSDateTime();
        public string yakinlikDerecesiAd { get { return TNS.IKY.Arac.YakinlikAd(yakinlik); } }
        public int ogrenimTipi = 0;
        public string meslek = "";
        public string cinsiyetAd = "";
        public string ogrenimAd = "";
        public string personelAdi = "";
        public string unvan = "";
        public string durumKayitKod = "";
        public string durumAd = "";
        public int cocukSayisi = 0;
        public int yas = 0;
    }

    /// <summary>
    /// Personel Askerlik bilgisi.
    /// </summary>
    [Serializable()]
    public class PAskerlik
    {
        public int kod { get; set; }
        public int askerlikDurumu { get; set; }
        public string askerlikDurumHitapID { get; set; }
        public string askerlikDurumuKayitKod { get; set; }
        public string askerlikDurumuAd { get; set; }
        public int sayilmayanGunSayisi { get; set; }
        public int rutbesi { get; set; }
        public int sinifi { get; set; }
        public int borclanmaVarMi { get; set; }
        public TNSDateTime tecilTarihi { get; set; }
        public TNSDateTime askerlikBaslamaTarihi { get; set; }
        public TNSDateTime terhisTarihi { get; set; }
        public TNSDateTime belgeTarihi { get; set; }
        public TNSDateTime yedekSubayOkuluGirisTarihi { get; set; }
        public TNSDateTime astegmenNaspTarihi { get; set; }
        public TNSDateTime tegmenNaspTarihi { get; set; }
        public TNSDateTime erYedekSubayGecisTarihi { get; set; }
        public TNSDateTime erYedekSubayKitaHizmetiBaslamaTarihi { get; set; }
        public TNSDateTime erYedekSubayKitaHizmetiBitisTarihi { get; set; }
        public TNSDateTime ibrazIntikalTarihi { get; set; }
        public string askerlikCuzdanNo { get; set; }
        public string askerlikMuafNeden { get; set; }
        public string belgeNo { get; set; }
        public string askerlikSubesi { get; set; }
        public string sinifDonemSicilNo { get; set; }
        public string erYedekSubayGorevYeri { get; set; }
        public string aciklama { get; set; }
        public string sinifAd { get; set; }
        public string askerlikBaslamaTarihiST { get { return askerlikBaslamaTarihi.ToString(); } }
        public string terhisTarihiST { get { return terhisTarihi.ToString(); } }
        public string ibrazIntikalTarihiST { get { return ibrazIntikalTarihi.ToString(); } }
        public string belgeTarihiST { get { return belgeTarihi.ToString(); } }
        public string astegmenNaspTarihiST { get { return astegmenNaspTarihi.ToString(); } }
        public Sonuc sonuc { get; set; }

        public PAskerlik()
        {
            kod = 0;
            askerlikDurumu = 0;
            sayilmayanGunSayisi = 0;
            rutbesi = 0;
            sinifi = 0;
            borclanmaVarMi = 0;
            tecilTarihi = new TNSDateTime();
            askerlikBaslamaTarihi = new TNSDateTime();
            terhisTarihi = new TNSDateTime();
            belgeTarihi = new TNSDateTime();
            yedekSubayOkuluGirisTarihi = new TNSDateTime();
            astegmenNaspTarihi = new TNSDateTime();
            tegmenNaspTarihi = new TNSDateTime();
            erYedekSubayGecisTarihi = new TNSDateTime();
            erYedekSubayKitaHizmetiBaslamaTarihi = new TNSDateTime();
            erYedekSubayKitaHizmetiBitisTarihi = new TNSDateTime();
            ibrazIntikalTarihi = new TNSDateTime();
            askerlikDurumHitapID = "";
            askerlikDurumuKayitKod = "";
            askerlikDurumuAd = "";
            askerlikCuzdanNo = "";
            askerlikMuafNeden = "";
            belgeNo = "";
            askerlikSubesi = "";
            sinifDonemSicilNo = "";
            erYedekSubayGorevYeri = "";
            aciklama = "";
            sinifAd = "";
            sonuc = new Sonuc();
        }
    }

    /// <summary>
    /// Personel Epitim(Kurs) Sınıfı
    /// </summary>
    [Serializable()]
    public class PersonelKurs
    {
        public int kod = 0;
        public int personelKod = 0;
        /// <summary>
        /// Personelin T.C. Kimlik numarası
        /// </summary>
        public string tcKimlikNo = "";
        /// <summary>
        /// Kurs Numarası
        /// </summary>
        public int kursKod = 0;
        public string kursKodHitapID = "";
        public TNSDateTime tarih = new TNSDateTime();
        /// <summary>
        /// Kursu aldığı yer
        /// </summary>
        public string aldigiYer = "";
        /// <summary>
        /// Başlama tarihi
        /// </summary>
        public TNSDateTime baslamaTarihi = new TNSDateTime();
        /// <summary>
        /// Bitirme tarihi
        /// </summary>
        public TNSDateTime bitirmeTarihi = new TNSDateTime();
        /// <summary>
        /// Puan
        /// </summary>
        public double puan = 0;
        /// <summary>
        /// Derece
        /// </summary>
        public string derece = "";
        /// <summary>
        /// Kurs adı
        /// </summary>
        public string kursAdi = "";
        /// <summary>
        /// Kurs-Ders saati
        /// </summary>
        public int dersSaati = 0;
        /// <summary>
        /// Eğitim yeri
        /// </summary>
        public int egitimYeri = 0; //yutiçi,yurtdışı
        public int ogrenimYeri = 0;
        public string ogrenimYeriHitapID = "";
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama = "";
        /// <summary>
        /// Dönem
        /// </summary>
        public string donem = "";
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil = 0;
        /// <summary>
        /// Dosya Numarası
        /// </summary>
        public string dosyaNo = "";
        /// <summary>
        /// Gün
        /// </summary>
        public int gun = 0;
        public TNSDateTime kurumaIbrazTarihi = new TNSDateTime();
    }

    /// <summary>
    /// Personel İletişim bilgisi.
    /// </summary>
    [Serializable()]
    public class PersonelIletisim
    {
        public int kod { get; set; }
        public string isTelefon { get; set; }
        public string cepTelefon { get; set; }
        public string evTelefon { get; set; }
        public string faks { get; set; }
        public string eposta { get; set; }
        public string adres { get; set; }
        public string url { get; set; }
        public string mobilImzaTelefon { get; set; }
        public string tarabyaTel { get; set; }
        public string lojmanTel { get; set; }
        public string telsiz { get; set; }
        public string goruntulu { get; set; }
        public int rehberdeGozuksun { get; set; }
        public string eposta2 { get; set; }
        public string cepTelefon2 { get; set; }
        public Sonuc sonuc { get; set; }
        public string personelAd { get; set; }

        public string mailLink
        {
            get
            {
                return "<a href='#' onclick='GoMail(\"" + eposta + "\"); return false;' target='_blank'>" + eposta + "</a>";
            }
        }

        public string mailLink2
        {
            get
            {
                return "<a href='#' onclick='GoMail(\"" + eposta2 + "\"); return false;' target='_blank'>" + eposta + "</a>";
            }
        }

        public PersonelIletisim()
        {
            kod = 0;
            isTelefon = "";
            cepTelefon = "";
            evTelefon = "";
            faks = "";
            eposta = "";
            eposta2 = "";
            adres = "";
            url = "";
            mobilImzaTelefon = "";
            tarabyaTel = "";
            lojmanTel = "";
            telsiz = "";
            goruntulu = "";
            rehberdeGozuksun = 0;
            cepTelefon2 = "";
            sonuc = new Sonuc();
        }
    }

    public enum ENUMPersonelSira
    {
        Unvan = 1,
        Birim = 2,
        AdSoyad = 3,
        UnvanAd = 4,
        SicilNo = 5,
    }
    /// <summary>
    /// Personel bilgisi.
    /// </summary>
    [Serializable()]
    public class Personel
    {
        /*
            Kayıt edilmek istenilen personelin TC Kimlik No kontrol eder. 
            Doğru ise true, yoksa false döndürür.
        */
        public static bool TCKimlikNoKontrolu(KYM.Kullanici kullanan, string mernisNo, TNSDateTime dogumTarih)
        {
            TNS.KYM.IKYMServis servis = TNS.KYM.Arac.Tanimla();
            bool mernisKontrolYapma = servis.MernisKontroluYapma();
            if (mernisKontrolYapma)
                return true;

            TNS.KYM.KullaniciVAN kMernisVAN = servis.MernisBilgiOku(kullanan, mernisNo, dogumTarih);
            if (kMernisVAN.sonuc.islemSonuc)
            {
                if (kMernisVAN.kullanici.mernis != mernisNo)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        public string Kontrol(KYM.Kullanici kullanan)
        {
            string hata = "";
            if (string.IsNullOrWhiteSpace(this.tcKimlikNo))
                hata += "TC Kimlik numarası boş bırakılamaz!<br>";

            if (!TCKimlikNoKontrolu(kullanan, this.tcKimlikNo, this.dogumTarihi))
                hata = "T.C Kimlik numarası " + this.tcKimlikNo + " hatalı veya yazılan Ad Soyad ile uyumsuz veya Kimlik doğrulama servisi ile bağlantı kurulamıyor!<br>";

            if (string.IsNullOrWhiteSpace(this.ad))
            {
                hata += "Ad boş bırakılamaz!<br>";
            }
            if (string.IsNullOrWhiteSpace(this.soyad))
            {
                hata += "Soyad boş bırakılamaz!<br>";
            }

            //if (string.IsNullOrWhiteSpace(this.nufus.anaAd))
            //{
            //    hata += "Ana Adı boş bırakılamaz!<br>";
            //}

            //if (string.IsNullOrWhiteSpace(this.nufus.babaAd))
            //{
            //    hata += "Baba Adı boş bırakılamaz!<br>";
            //}

            if (!string.IsNullOrEmpty(this.bankaIbanNo))
            {
                Sonuc ibanSonuc = OrtakFonksiyonlar.IBANKontrol(this.bankaIbanNo, true);
                if (!ibanSonuc.islemSonuc)
                    hata += ibanSonuc.hataStr;
            }

            //if (p.personelTur <= 0)
            //    hata += "<br>Personel Türü boş geçilemez.";

            //if (p.sicilNo.Trim() == "")
            //    hata += "<br>Sicil numarası boş geçilemez.";

            return hata;
        }

        public Personel()
        {
            kod = 0;
            personelTip = 0;
            sakatlikDurumu = 0;
            aktifMi = 0;
            sakatlikDerecesi = 0;
            terorMagduru = 0;
            hukumlu = 0;
            korunmayaMuhtac = 0;
            durum = 0;
            sosyalGuvence = 0;
            kurumu = 0;
            tcKimlikNo = "";
            sicilNo = "";
            personelTipAd = "";
            ad = "";
            soyad = "";
            sakatlikDurumuHitapID = "";
            cinsiyetHitapID = "";
            aciklama = "";
            emekliSicilNo = "";
            sskSicilNo = "";
            bagkurSicilNo = "";
            nufus.ilkSoyad = "";
            bankaIbanNo = "";
            vergiDairesi = "";
            genelSorgu = "";
            hizmetSureleri = new Hizmet[1];
            hizmetSureleri[0] = new Hizmet();
            aile = new PersonelAile[1];
            aile[0] = new PersonelAile();
            ogrenim = new PersonelOgrenim[1];
            ogrenim[0] = new PersonelOgrenim();
            dil = new PersonelDil[1];
            dil[0] = new PersonelDil();
            aktifDurum = 1;
            geldigiKurum = "";
            listeSiralama = ENUMPersonelSira.Unvan;
            tcList = null;
            personelTipleri = "";
            aktifDurumlar = "";
            personelSiteKod = 0;
            hizliBilgi = false;
            tarihce = false;
            arsivNo = "";
            rumuz = "";
            titr = "";
            geldigiKurumSicili = "";
        }

        public PersonelMaasParam personelMaasParam = null;

        public ENUMPersonelSira listeSiralama;
        public int kod { get; set; }
        public string TCKIMLIKNO { get { return tcKimlikNo; } set { tcKimlikNo = value; } }
        public string tcKimlikNo { get; set; }
        public string sicilNo { get; set; }
        public int personelTip { get { return kadro.personelTip; } set { kadro.personelTip = value; } }
        public string personelTipAd { get { return kadro.personelTipAd; } set { kadro.personelTipAd = value; } }

        public string AD { get { return ad; } set { ad = value; } }
        public string ad { get; set; }

        public string SOYAD { get { return soyad; } set { soyad = value; } }
        public string soyad { get; set; }

        public string adSoyad { get { return ad + " " + soyad; } }
        public int sakatlikDurumu { get; set; }
        public string sakatlikDurumuHitapID { get; set; }
        public ENUMCINSIYET cinsiyet { get { return nufus.cinsiyeti; } }
        public string cinsiyetHitapID { get; set; }
        public string aciklama { get; set; }
        public string emekliSicilNo { get; set; }
        public string sskSicilNo { get; set; }
        public string bagkurSicilNo { get; set; }
        public string ilkSoyad { get { return nufus.ilkSoyad; } }
        public string bankaIbanNo { get; set; }
        public string vergiDairesi { get; set; }
        public int aktifMi { get; set; }
        public int sakatlikDerecesi { get; set; }
        public int terorMagduru { get; set; }
        public int hukumlu { get; set; }
        public int korunmayaMuhtac { get; set; }
        public int durum { get; set; }
        public int sosyalGuvence { get; set; }
        public int kurumu { get; set; }
        public string geldigiKurum { get; set; }
        public string personelTipleri { get; set; }
        public string aktifDurumlar { get; set; }
        public int personelSiteKod { get; set; }
        public bool hizliBilgi { get; set; }
        public bool tarihce { get; set; }

        public int unvanSira { get { return kadro.unvanSira; } set { kadro.unvanSira = value; } }
        public int UNVANKOD { get { return kadro.unvan; } set { kadro.unvan = value; } }
        public string UNVANAD { get { return kadro.unvanAd; } set { kadro.unvanAd = value; } }
        public string unvanAd { get { return kadro.unvanAd; } set { kadro.unvanAd = value; } }

        public int KADRODERECE { get { return kadro.kadroDerece; } set { kadro.kadroDerece = value; } }
        public int EKGOSTERGE { get { return kadro.odemeyeEsasEkGosterge; } set { kadro.odemeyeEsasEkGosterge = value; } }
        public string EPOSTA { get { return iletisim.eposta; } set { iletisim.eposta = value; } }
        public string mailLink { get { return iletisim.mailLink; } }
        public string cepTelefon { get { return iletisim.cepTelefon; } set { iletisim.cepTelefon = value; } }
        public string isTelefon { get { return iletisim.isTelefon; } set { iletisim.isTelefon = value; } }

        public string bankaHesapNo { get { return this.bankaIbanNo; } set { this.bankaIbanNo = value; } }

        public int sinifKodu { get { return kadro.sinifi; } set { kadro.sinifi = value; } }

        public string birimAd { get { return kadro.birimAd; } set { kadro.birimAd = value; } }

        public string genelSorgu { get; set; }
        public int aktifDurum { get; set; }
        public string resimYol { get { return ResimBoy(3); } }
        public string resimBase64 { get { return ResimBoy(1); } }
        public string resimBoy2 { get { return ResimBoy(2); } }

        private string ResimBoy(int boy)
        {
            return "ResimGoster.ashx?pKod=" + kod.ToString() + "&kucuk=" + boy.ToString();
        }

        public TNSDateTime dogumTarihi { get { return nufus.dogumTarihi; } }
        public TNSDateTime ilkIseBaslamaTarihi = new TNSDateTime();
        public TNSDateTime asaletTarihi = new TNSDateTime();
        public TNSDateTime kurumaBaslamaTarihi = new TNSDateTime();
        public PersonelNufus nufus = new PersonelNufus();
        public Hizmet[] hizmetSureleri = null;
        public PersonelKadro kadro = new PersonelKadro();
        //public PersonelKadro kadro
        //{
        //    get
        //    {
        //        return kadro;
        //    }
        //}


        public PAskerlik askerlik = new PAskerlik();

        public PersonelOgrenim[] ogrenim;

        public PersonelKurs[] kurs;
        public PersonelIletisim iletisim = new PersonelIletisim();
        public PersonelAile[] aile;
        public PersonelDil[] dil;
        public PersonelIzin[] izin;
        public string kodlar { get; set; }
        public DateTime kadroBaslangicTarihi
        {
            get
            {
                if (hizmetSureleri != null && hizmetSureleri.Length > 0 && hizmetSureleri[0].guncelHizmet != null)
                    return hizmetSureleri[0].guncelHizmet.kadroyaGectigiTarih;
                else
                    return DateTime.MinValue;
            }
        }
        public string unvanVekaletAd { get { return kadro.vekilUnvanAd; } set { kadro.vekilUnvanAd = value; } }
        public string arsivNo { get; set; }
        public string geldigiKurumSicili { get; set; }
        public string medeniDurumuAd { get { return nufus.medeniHaliAd; } }
        public int yas { get { return System.DateTime.Now.Year - nufus.dogumTarihi.Yil; } }

        public int kayitBasla = 0;
        public int kayitToplam = -1;
        public int satir = 0;

        public TNSCollection tcList = null;

        public int odemeEsasDerece { get { return kadro.odemeyeEsasDerece; } set { kadro.odemeyeEsasDerece = value; } }
        public int odemeEsasKademe { get { return kadro.odemeyeEsasKademe; } set { kadro.odemeyeEsasKademe = value; } }
        public int odemeEsasGosterge { get { return kadro.odemeyeEsasGosterge; } set { kadro.odemeyeEsasGosterge = value; } }
        public int odemeEsasEkGosterge { get { return kadro.odemeyeEsasEkGosterge; } set { kadro.odemeyeEsasEkGosterge = value; } }
        public int emekliEsasDerece { get { return kadro.emekliyeEsasDerece; } set { kadro.emekliyeEsasDerece = value; } }
        public int emekliEsasKademe { get { return kadro.emekliyeEsasKademe; } set { kadro.emekliyeEsasKademe = value; } }
        public int emekliEsasGosterge { get { return kadro.emekliyeEsasGosterge; } set { kadro.emekliyeEsasGosterge = value; } }
        public int emekliEsasEkGosterge { get { return kadro.emekliyeEsasEkGosterge; } set { kadro.emekliyeEsasEkGosterge = value; } }
        public int kazanilmisHakDerece { get { return kadro.kazanilmisHakDerece; } set { kadro.kazanilmisHakDerece = value; } }
        public int kazanilmisHakKademe { get { return kadro.kazanilmisHakKademe; } set { kadro.kazanilmisHakKademe = value; } }
        public int kazanilmisGosterge { get { return kadro.kazanilmisGosterge; } set { kadro.kazanilmisGosterge = value; } }
        public int kazanilmisEkGosterge { get { return kadro.kazanilmisEkGosterge; } set { kadro.kazanilmisEkGosterge = value; } }
        public string calistigiBirimTuru { get { return kadro.calistigiBirimTuru; } set { kadro.calistigiBirimTuru = value; } }
        public string birimTuru { get { return kadro.birimTuru; } set { kadro.birimTuru = value; } }
        public string[] tipGruplar = null;
        public string rumuz { get; set; }
        public string titr { get; set; }
        public string kopruKod { get; set; }
        public ObjectArray kisiYakinlik = null;
        public ObjectArray lojmanlar = null;
        public ObjectArray kadrolar = null;

        /// <summary>
        /// 
        ///  0 : -
        ///  10: Aktif
        ///  20: Sonra Başlayanlar
        ///  21: Önce Başlayanlar
        ///  30: Sonra Ayrılanlar
        ///  31: Önce Ayrılanlar
        /// 
        /// </summary>
        public int aktifTarihTur = 0;
        public TNSDateTime aktifTarih = null;

        public bool bordroDisiDahil = true;

        public byte[] resim = null;

        public PersonelOgrenim SonOkulBul()
        {
            if (this.ogrenim != null)
            {
                foreach (PersonelOgrenim item in ogrenim)
                {
                    if (item.sonOkul == 1) return item;
                }
            }
            return null;
        }
        public TNSDateTime aktifTarihSon = null;
        public bool aktifTarihSonHareket = true;
        public bool resimOlsun = false;
        public bool ogrenimListesiOlsun = false;
        public bool lojmanListesiOlsun = false;
        public bool kadrolarOlsun = false;
        public bool maasParamlarOlsun = false;
        public TNSDateTime matrahTarih = null;
        public bool ailelerOlsun = false;
        public bool dillerOlsun = false;
        public bool hizmetSuresiOlsun = false;
        public bool sendikaOlsun = false;

        public int kullanilanIzinGun { get; set; }

        public int gorevYili { get; set; }

        public ObjectArray gorevler { get; set; }

        public TNSDateTime izinDonemBas = null;
        public TNSDateTime izinDonemSon = null;
        public string izinTurler = null;

        public string kopruKodTam
        {
            get
            {
                if (kopruKod == null) return null;
                string kKod = kopruKod;
                if (!string.IsNullOrWhiteSpace(kadro.gitOrganizasyonKopruKodu)) kKod += ";" + kadro.gitOrganizasyonKopruKodu;
                return kKod;
            }
        }

        public TNSDateTime giderBasla = null;
        public TNSDateTime giderBitis = null;
        public ObjectArray giderToplamlar;
        public int cezalarOlsun = 0;
        public ObjectArray cezaOduller = null;
        public TNSCollection sendikalar = null;
    }

    [Serializable()]
    public class PersonelNufus
    {
        public int kod { get; set; }
        public int medeniHali { get; set; }
        public ENUMCINSIYET cinsiyeti { get; set; }
        public string cinsiyetAd
        {
            get
            {
                return TNS.IKY.Arac.CinsiyetAdBul(cinsiyeti);
            }
        }
        public string kanGrubu { get; set; }
        public string medeniHaliAd { get; set; }
        public string babaAd { get; set; }
        public string anaAd { get; set; }
        public string dogumYeri { get; set; }
        public string nufusIlKod { get; set; }
        public string nufusIlceKod { get; set; }
        public string cuzdanSeriNo { get; set; }
        public string cuzdanKayitNo { get; set; }
        public string nufusMahalleKoy { get; set; }
        public string nufusCiltNo { get; set; }
        public string nufusSiraNo { get; set; }
        public string nufusAileSiraNo { get; set; }
        public string cuzdanVerilisNedeni { get; set; }
        public string cuzdanVerildigiYer { get; set; }
        public string dini { get; set; }
        public string ilkSoyad { get; set; }
        public string ilAd { get; set; }
        public string ilceAd { get; set; }
        public TNSDateTime dogumTarihi { get; set; }
        public TNSDateTime cuzdanVerilisTarihi { get; set; }
        public string pasaportNo { get; set; }
        public TNSDateTime pasaportVerilisTarihi { get; set; }
        public string pasaportVerildigiYer { get; set; }
        public string pasaportVerenMakam { get; set; }
        public int pasaportSuresi { get; set; }


        public PersonelNufus()
        {
            kod = 0;
            medeniHali = 0;
            cinsiyeti = ENUMCINSIYET.TANIMSIZ;
            kanGrubu = "";
            medeniHaliAd = "";
            babaAd = "";
            anaAd = "";
            dogumYeri = "";
            nufusIlKod = "";
            nufusIlceKod = "";
            cuzdanSeriNo = "";
            cuzdanKayitNo = "";
            nufusMahalleKoy = "";
            nufusCiltNo = "";
            nufusSiraNo = "";
            nufusAileSiraNo = "";
            cuzdanVerilisNedeni = "";
            cuzdanVerildigiYer = "";
            dini = "";
            ilkSoyad = "";
            ilAd = "";
            ilceAd = "";
            dogumTarihi = new TNSDateTime();
            cuzdanVerilisTarihi = new TNSDateTime();
            pasaportNo = "";
            pasaportVerilisTarihi = new TNSDateTime();
            pasaportVerildigiYer = "";
            pasaportVerenMakam = "";
            pasaportSuresi = 0;
        }
    }

    [Serializable()]
    public class PersonelPersonelRapor
    {
        public string tcKimlikNo = "";
        public string ad = "";
        public string soyad = "";
        public string unvan = "";
        public int cinsiyet = 0;
        public TNSDateTime dogumTarihi = new TNSDateTime();
        public string okulAdi = "";
        public int seviyesi = 0;
        public string seviyesiText = "";
        public TNSDateTime bitirmeTarihi = new TNSDateTime();
        public string LUOkulAdi = "";
        public int LUSeviyesi = 0;
        public string LUSeviyesiText = "";
        public TNSDateTime LUBitirmeTarihi = new TNSDateTime();
        public string dil = "";
        public int notuDil = 0;
        public string duzeyDil = "";
        public int yilDil = 0;
        public int ayDil = 0;
        public int belgeTurDil = 0;
    }

    [Serializable()]
    public class PersonelEgitimRapor
    {
        public string tcKimlikNo = "";
        public string ad = "";
        public string soyad = "";
        public string unvan = "";
        public string egitiminAdi = "";
        public string egitiminTipi = "";
        public TNSDateTime baslangicTarihi = new TNSDateTime();
        public TNSDateTime bitisTarihi = new TNSDateTime();
        public int dersSaati = 0;
        public string egitimiVerenKurulus = "";
        public string egitimYeri = "";
        public string aciklama = "";
        public string donemi = "";
        public int yili = 0;
        public string dosyaNo = "";
    }

    [Serializable()]
    public class PersonelIzinRapor
    {
        public string tcKimlikNo = "";
        public string ad = "";
        public string unvan = "";
        public TNSDateTime baslangicTarihi = new TNSDateTime();
        public TNSDateTime bitisTarihi = new TNSDateTime();
        public int yillikIzin = 0;
        public string aciklama = "";
        public int tip = 0;
        public int kadroDerece = 0;
        public int gunSayisi = 0;
        public string izinTuru;
        public string izinYeri;
        public int siraNo = 0;
        public int personelKod = 0;
    }

    [Serializable()]
    public class PersonelIzinDurum
    {
        public string tcKimlikNo = "";
        public int personelKod = 0;
        public int tip = 0;
        public string ad = "";
        public int toplamGun = 0;
        public int kullanilanGun = 0;
        public int listelemeYili = 0;
    }

    /// <summary>
    /// Personelin Eklediği Belgeler
    /// </summary>
    [Serializable()]
    public class PersonelBelgeler
    {
        public PersonelBelgeler()
        {
            this.tcKimlikNo = "";
            this.tip = 0;
            this.kayitSiraNo = "";
            this.belgeSiraNo = 0;
            this.belgeAdi = "";
        }

        public PersonelBelgeler(string tcKimlikNo = "", int tip = 0, string kayitSiraNo = "", int belgeSiraNo = 0, string belgeAdi = "")
        {
            this.tcKimlikNo = tcKimlikNo;
            this.tip = tip;
            this.kayitSiraNo = kayitSiraNo;
            this.belgeSiraNo = belgeSiraNo;
            this.belgeAdi = belgeAdi;
        }

        public PersonelBelgeler(string tcKimlikNo, int tip, string kayitSiraNo, string belgeAdi, int belgeBoyutu)
        {
            this.tcKimlikNo = tcKimlikNo;
            this.tip = tip;
            this.kayitSiraNo = kayitSiraNo;
            this.belgeSiraNo = 0;
            string dosyaAdi = belgeAdi;
            if (dosyaAdi.LastIndexOf('/') > 0)
                dosyaAdi = dosyaAdi.Substring(dosyaAdi.LastIndexOf('/')).TrimStart('/');
            if (dosyaAdi.LastIndexOf('\\') > 0)
                dosyaAdi = dosyaAdi.Substring(dosyaAdi.LastIndexOf('\\')).TrimStart('\\');
            this.belgeAdi = dosyaAdi;
            this.belge = new byte[belgeBoyutu];
        }

        public PersonelBelgeler(int pkod, int tipKayitKod, int tip, string belgeAdi, int belgeBoyutu)
        {
            //this.kod = kod;
            this.tip = tip;
            this.pKod = pkod;
            this.tipKayitKod = tipKayitKod;
            string dosyaAdi = belgeAdi;
            if (dosyaAdi.LastIndexOf('/') > 0)
                dosyaAdi = dosyaAdi.Substring(dosyaAdi.LastIndexOf('/')).TrimStart('/');
            if (dosyaAdi.LastIndexOf('\\') > 0)
                dosyaAdi = dosyaAdi.Substring(dosyaAdi.LastIndexOf('\\')).TrimStart('\\');
            this.belgeAdi = dosyaAdi;
            this.belge = new byte[belgeBoyutu];
        }

        /// <summary>
        /// T.C. Kimlik numarası
        /// </summary>
        public string tcKimlikNo;
        /// <summary>
        /// Hangi forma eklendiği
        /// </summary>
        public int tip;
        /// <summary>
        /// Formdaki hangi kayda eklendiği
        /// </summary>
        public string kayitSiraNo;
        /// <summary>
        /// Kaydın kaçıncı belgesi olduğu
        /// </summary>
        public int belgeSiraNo;
        /// <summary>
        /// Belge adı
        /// </summary>
        public string belgeAdi;
        /// <summary>
        /// Belge içeriği
        /// </summary>
        public byte[] belge;

        public int kod;
        public int pKod;
        public int tipKayitKod;
    }


    /// <summary>
    /// Personel bilgisi.
    /// </summary>
    [Serializable()]
    public class PersonelUnvanKadro
    {
        public string unvanAd = "";
        public int cinsiyet = 0;
        public int cinsiyetAdet = 0;
    }

    /// <summary>
    /// Personel Resim Bilgisi
    /// </summary>
    [Serializable()]
    public class PersonelResim
    {
        /// <summary>
        /// PersonelKod
        /// </summary>
        public int kod = 0;
        //public string personelAd;
        //public string sicilNo;
        //public string kimlikNo;
        /// <summary>
        /// Resim bilgisi
        /// </summary>
        public byte[] resim;
    }

    /// <summary>
    /// Personel Adres Sınıfı
    /// </summary>
    /// 
    [Serializable()]
    public class PersonelAdres
    {

        public int kod = 0;
        public string tcKimlikNo = "";
        public string ilKod = "";
        public string ilceKod = "";
        public string mahalle = "";
        public string caddeSokak = "";
        public string binaDaireNo = "";
        public string postaKodu = "";
        public string adres = "";
        public string tebligatAdres = "";
        public int mulkDurumu = 0;
        public TNSDateTime tasinmaTarihi = new TNSDateTime();
        public string isTel = "";
        public string evTel = "";
        public string cepTel = "";
        public string faks = "";
        public string eposta = "";
        public string url = "";
        public string mobilImzaTel = "";
        public string acilKisi = "";
        public string acilKisiNumara = "";
    }

    /// <summary>
    /// Personel Ceza Ödül Sınıfı
    /// </summary>
    [Serializable()]
    public class PersonelCezaOdul
    {
        public int kod = 0;
        public int personelKod = 0;
        public int cezaOdulTipi = 0;
        public string verenKurum = "";
        public string verenAdi = "";
        public string verenUnvani = "";
        public string alanUnvani = "";
        public string cinsi = "";
        public string sebebi = "";
        public string belgeCinsi = "";
        public string belgeNo = "";
        public string dayanak = "";
        public string cezaOdulTipiAdi = "";
        public string cezaOdulTipiKayitKodu = "";
        public TNSDateTime belgeTarihi = new TNSDateTime();
        public TNSDateTime tarih = new TNSDateTime();
    }

    /// <summary>
    /// Personel Öğrenim Sınıfı
    /// </summary>
    [Serializable()]
    public class PersonelOgrenim
    {
        public int kod = 0;
        public int personelKod = 0;
        public string tcKimlikNo = "";
        public string personelAdi = "";
        public int ogrenimKod = 0;
        public string ogrenimKodHitapID = "";
        public string ogrenimKodKayitKod = "";
        public string ogrenimAdi = "";
        public int universiteKod = 0;
        public string universiteAdi = "";
        public int fakulteKod = 0;
        public string fakulteAdi = "";
        public int bolumKod = 0;
        public string bolumAdi = "";
        public string okulAdi = "";
        public int belgeTur = 0;
        public int ogrenimSuresi = 0;
        public TNSDateTime belgeTarihi = new TNSDateTime();
        public string belgeNo = "";
        public TNSDateTime baslangicTarihi = new TNSDateTime();
        public TNSDateTime bitirmeTarihi = new TNSDateTime();
        public int derece = 0;
        public int gecerli = 0;
        public int sonOkul = 0;
        public int ogrenimYeri = 0;
        public string ogrenimYeriHitapID = "";
        public int denklik = 0;
        public TNSDateTime denklikTarihi = new TNSDateTime();
        public string denkOkul = "";
        public string denkBolum = "";
        public int hazirlikVarMi = 0;
        public TNSDateTime kurumaIbrazTarihi = new TNSDateTime();
        public int memuriyeteBasladigiOkul = 0;
    }

    /// <summary>
    /// Personel Dil bilgisi.
    /// </summary>
    [Serializable()]
    public class PersonelDil
    {
        public string tcKimlikNo = "";
        public int kod = 0;
        public int personelKod = 0;
        public int dilKod = 0;
        public double puan = 0;
        public TNSDateTime tarih = new TNSDateTime();
        public string duzey = "";
        public int donemAy = 0;
        public int donemYil = 0;
        public int belgeTur = 0;
        public TNSDateTime belgeTarihi = new TNSDateTime();
        public string belgeNo = "";
        public int bordroyaEsas = 0;
        public int kurumFaydalaniyor = 0;
        public string dilSeviyesi = "";
        public string dilAdi = "";
        public string belgeAdi = "";
    }

    ///// <summary>
    ///// Personel Memuriyet Sınıfı
    ///// </summary>
    //[Serializable()]
    //public class PersonelMemuriyet
    //{
    //    public int kod = 0;
    //    public int personelKod = 0;
    //    /// <summary>
    //    /// Personelin T.C. Kimlik numarası
    //    /// </summary>
    //    public string tcKimlikNo = "";
    //    /// <summary>
    //    /// Personelin T.C. Kimlik numarası
    //    /// </summary>
    //    public int siraNo = 0;
    //    /// <summary>
    //    /// Muhasebe kodu
    //    /// </summary>
    //    public int muhasebeKodu = 0;
    //    /// <summary>
    //    /// Muhasebe Birim kodu
    //    /// </summary>
    //    public string birimKodu = "";
    //    /// <summary>
    //    /// Organizasyon birim kodu
    //    /// </summary>
    //    public int organizasyonKodu = 0;
    //    /// <summary>
    //    /// Kurumsal kod
    //    /// </summary>
    //    public string kurum = "";
    //    /// <summary>
    //    /// Birim Adı
    //    /// </summary>
    //    public string birimAd = "";
    //    /// <summary>
    //    /// Görevi
    //    /// </summary>
    //    public string gorev = "";
    //    /// <summary>
    //    /// İstihdam şekli
    //    /// </summary>
    //    public int istihdamSekli = 0;
    //    /// <summary>
    //    /// Açıklama
    //    /// </summary>
    //    public string aciklama = "";
    //    /// <summary>
    //    /// Başlama tarihi
    //    /// </summary>
    //    public TNSDateTime baslamaTarihi = new TNSDateTime();
    //    /// <summary>
    //    /// ayrilma tarihi
    //    /// </summary>
    //    public TNSDateTime ayrilmaTarihi = new TNSDateTime();
    //    /// <summary>
    //    /// Başlama nedeni
    //    /// </summary>
    //    public int baslamaNedeni = 0;
    //    /// <summary>
    //    /// ayrilma nedeni
    //    /// </summary>
    //    public int ayrilmaNedeni = 0;
    //    /// <summary>
    //    /// Başlama nedeni AD
    //    /// </summary>
    //    public string baslamaNedeniAd = "";
    //    /// <summary>
    //    /// ayrilma nedeni ad
    //    /// </summary>
    //    public string ayrilmaNedeniAd = "";
    //    /// <summary>
    //    /// Evrak numarası
    //    /// </summary>
    //    public string evrakNo = "";
    //    /// <summary>
    //    /// Yasal dayanak
    //    /// </summary>
    //    public string yasalDayanak = "";
    //    /// <summary>
    //    /// Onay tarihi
    //    /// </summary>
    //    public TNSDateTime onayTarih = new TNSDateTime();
    //    /// <summary>
    //    /// Kadro derece
    //    /// </summary>
    //    public int kadroDerece = 0;
    //    /// <summary>
    //    /// Kadro tarihi
    //    /// </summary>
    //    public TNSDateTime kadroTarih = new TNSDateTime();
    //    /// <summary>
    //    /// Ödemeye esas derece
    //    /// </summary>
    //    public int odemeyeEsasDerece = 0;
    //    /// <summary>
    //    /// Ödemeye esas kademe
    //    /// </summary>
    //    public int odemeyeEsasKademe = 0;
    //    /// <summary>
    //    /// Ödemeye esas tarih
    //    /// </summary>
    //    public TNSDateTime odemeyeEsasTarih = new TNSDateTime();
    //    /// <summary>
    //    /// Ödemeye esas gösterge
    //    /// </summary>
    //    public int odemeyeEsasGosterge = 0;
    //    /// <summary>
    //    /// Ödemeye esas ek gösterge
    //    /// </summary>
    //    public int odemeyeEsasEkGosterge = 0;
    //    /// <summary>
    //    /// Kazanılmış hak derece
    //    /// </summary>
    //    public int kazanilmisHakDerece = 0;
    //    /// <summary>
    //    /// Kazanılmış hak kademe
    //    /// </summary>
    //    public int kazanilmisHakKademe = 0;
    //    /// <summary>
    //    /// Kazanılmış hak tarih
    //    /// </summary>
    //    public TNSDateTime kazanilmisHakTarih = new TNSDateTime();
    //    /// <summary>
    //    /// Kazanılmış gösterge
    //    /// </summary>
    //    public int kazanilmisGosterge = 0;
    //    /// <summary>
    //    /// Kazanılmış ek gösterge
    //    /// </summary>
    //    public int kazanilmisEkGosterge = 0;
    //    /// <summary>
    //    /// Emekliliğe esas derece
    //    /// </summary>
    //    public int emekliyeEsasDerece = 0;
    //    /// <summary>
    //    /// Emekliliğe esas kademe
    //    /// </summary>
    //    public int emekliyeEsasKademe = 0;
    //    /// <summary>
    //    /// Emekliliğe esas tarih
    //    /// </summary>
    //    public TNSDateTime emekliyeEsasTarih = new TNSDateTime();
    //    /// <summary>
    //    /// Emekliliğe esas gösterge
    //    /// </summary>
    //    public int emekliyeEsasGosterge = 0;
    //    /// <summary>
    //    /// Emekliliğe esas ek gösterge
    //    /// </summary>
    //    public int emekliyeEsasEkGosterge = 0;
    //}

    /// <summary>
    /// Personel Kadro Sınıfı
    /// </summary>
    [Serializable()]
    public class PersonelKadro : ICloneable
    {
        public PersonelKadro()
        {
            kod = 0;
            eskiKod = 0;
            personelKod = 0;
            personelTip = 0;
            ayrilmaGerekcesi = 0;
            ayrilmaGerekcesiAd = "";
            organizasyonKodu = 0;
            personelTipAd = "";
            personelTipCoklu = "";
            birimAd = "";
            unvan = 0;
            vekilUnvanKod = 0;
            unvanAd = "";
            sinifi = 0;
            sinifAd = "";
            sinifKisaAd = "";
            evrakNo = "";
            yasalDayanak = "";
            onayTarih = new TNSDateTime();
            aciklama = "";
            kadroDerece = 0;
            kadroTarih = new TNSDateTime();
            odemeyeEsasDerece = 0;
            odemeyeEsasKademe = 0;
            odemeyeEsasTarih = new TNSDateTime();
            odemeyeEsasGosterge = 0;
            odemeyeEsasEkGosterge = 0;
            kazanilmisHakDerece = 0;
            kazanilmisHakKademe = 0;
            kazanilmisHakTarih = new TNSDateTime();
            kazanilmisGosterge = 0;
            kazanilmisEkGosterge = 0;
            emekliyeEsasDerece = 0;
            emekliyeEsasKademe = 0;
            emekliyeEsasTarih = new TNSDateTime();
            emekliyeEsasGosterge = 0;
            emekliyeEsasEkGosterge = 0;
            atama = new PersonelAtama();
            personelTCKimlikNo = "";
            personelAdSoyad = "";
            gorevYeri = "";
            belgeCinsi = "";
            belgeNo = "";
            hizmetSebebi = "";
            hizmetSebebiAd = "";
            onaylayan = "";
            belgeTarihi = new TNSDateTime();
            atamaTarihi = new TNSDateTime();
            ayrilmaTarihi = new TNSDateTime();

            //eskiOrganizasyonKodu = 0;
            //eskiUnvanKodu = 0;
            //eskiKadroDerece = 0;
            //eskiOdemeyeEsasDerece = 0;
            //eskiOdemeyeEsasKademe = 0;
            //eskiOdemeyeEsasGosterge = 0;
            //eskiOdemeyeEsasEkGosterge = 0;
            //eskiKazanilmisHakDerece = 0;
            //eskiKazanilmisHakKademe = 0;
            //eskiKazanilmisGosterge = 0;
            //eskiKazanilmisEkGosterge = 0;
            //eskiEmekliyeEsasDerece = 0;
            //eskiEmekliyeEsasKademe = 0;
            //eskiEmekliyeEsasGosterge = 0;
            //eskiEmekliyeEsasEkGosterge = 0;
            kadroDurum = ENUMKadroDurumu.TANIMSIZ;
            eskiHizmetTipi = 0;
            eskiHizmetTipiFlag = 0;
            ayrilmaNedeni = "";
            kurumDisi = 0;
            hizmetGunSayisi = 0;
            hizmetSebebiKod = 0;
            //OnSekizYasAltiPrimGünü = 0;
            //primGunu = 0;
            KHADurumu = 0;
            kidemGunu = 0;
            //KHAGunu = 0;
            //EKEAGunu = 0;
            muhasebeBirimi = "";
            muhasebeKodu = 0;
            kurumsalKod = "";
            vekilUnvanAd = "";
            hizmetSebebiTip = ENUMHizmetSebebiNevi.TANIMSIZ;
            ayrilmaGerekcesiTip = ENUMHizmetSebebiNevi.TANIMSIZ;
            derece = new YukselecegiDerece();
            hizmetHesabi = ENUMHizmetTablolari.EmekliSandigi;
            terfiMi = false;
            meslekKodu = "";
        }

        public PersonelKadro(PersonelKadro gelen)
        {
            this.kod = gelen.kod;
            this.eskiKod = gelen.eskiKod;
            this.personelKod = gelen.personelKod;
            this.personelTip = gelen.personelTip;
            this.ayrilmaGerekcesi = gelen.ayrilmaGerekcesi;
            this.ayrilmaGerekcesiAd = gelen.ayrilmaGerekcesiAd;
            this.organizasyonKodu = gelen.organizasyonKodu;
            this.personelTipAd = gelen.personelTipAd;
            this.personelTipCoklu = gelen.personelTipCoklu;
            this.birimAd = gelen.birimAd;
            this.unvan = gelen.unvan;
            this.vekilUnvanKod = gelen.vekilUnvanKod;
            this.unvanAd = gelen.unvanAd;
            this.sinifi = gelen.sinifi;
            this.sinifAd = gelen.sinifAd;
            this.sinifKisaAd = gelen.sinifKisaAd;
            this.evrakNo = gelen.evrakNo;
            this.yasalDayanak = gelen.yasalDayanak;
            this.onayTarih = new TNSDateTime(gelen.onayTarih);
            this.aciklama = gelen.aciklama;
            this.kadroDerece = gelen.kadroDerece;
            this.kadroTarih = new TNSDateTime(gelen.kadroTarih);
            this.odemeyeEsasDerece = gelen.odemeyeEsasDerece;
            this.odemeyeEsasKademe = gelen.odemeyeEsasKademe;
            this.odemeyeEsasTarih = new TNSDateTime(gelen.odemeyeEsasTarih);
            this.odemeyeEsasGosterge = gelen.odemeyeEsasGosterge;
            this.odemeyeEsasEkGosterge = gelen.odemeyeEsasEkGosterge;
            this.kazanilmisHakDerece = gelen.kazanilmisHakDerece;
            this.kazanilmisHakKademe = gelen.kazanilmisHakKademe;
            this.kazanilmisHakTarih = new TNSDateTime(gelen.kazanilmisHakTarih);
            this.kazanilmisGosterge = gelen.kazanilmisGosterge;
            this.kazanilmisEkGosterge = gelen.kazanilmisEkGosterge;
            this.emekliyeEsasDerece = gelen.emekliyeEsasDerece;
            this.emekliyeEsasKademe = gelen.emekliyeEsasKademe;
            this.emekliyeEsasTarih = new TNSDateTime(gelen.emekliyeEsasTarih);
            this.emekliyeEsasGosterge = gelen.emekliyeEsasGosterge;
            this.emekliyeEsasEkGosterge = gelen.emekliyeEsasEkGosterge;
            this.atama = new PersonelAtama();
            this.personelTCKimlikNo = gelen.personelTCKimlikNo;
            this.personelAdSoyad = gelen.personelAdSoyad;
            this.gorevYeri = gelen.gorevYeri;
            this.belgeCinsi = gelen.belgeCinsi;
            this.belgeNo = gelen.belgeNo;
            this.hizmetSebebi = gelen.hizmetSebebi;
            this.hizmetSebebiAd = gelen.hizmetSebebiAd;
            this.onaylayan = gelen.onaylayan;
            this.belgeTarihi = new TNSDateTime(gelen.belgeTarihi);
            this.atamaTarihi = new TNSDateTime(gelen.atamaTarihi);
            this.ayrilmaTarihi = new TNSDateTime(gelen.ayrilmaTarihi);
            this.kadroDurum = gelen.kadroDurum;
            this.eskiHizmetTipi = gelen.eskiHizmetTipi;
            this.eskiHizmetTipiFlag = gelen.eskiHizmetTipiFlag;
            this.ayrilmaNedeni = gelen.ayrilmaNedeni;
            this.kurumDisi = gelen.kurumDisi;
            this.hizmetGunSayisi = gelen.hizmetGunSayisi;
            this.hizmetSebebiKod = gelen.hizmetSebebiKod;
            this.KHADurumu = gelen.KHADurumu;
            this.kidemGunu = gelen.kidemGunu;
            this.muhasebeBirimi = gelen.muhasebeBirimi;
            this.muhasebeKodu = gelen.muhasebeKodu;
            this.kurumsalKod = gelen.kurumsalKod;
            this.vekilUnvanAd = gelen.vekilUnvanAd;
            this.hizmetSebebiTip = gelen.hizmetSebebiTip;
            this.ayrilmaGerekcesiTip = gelen.ayrilmaGerekcesiTip;
            this.derece = new YukselecegiDerece();
            this.hizmetHesabi = gelen.hizmetHesabi;
            this.terfiMi = gelen.terfiMi;
            this.birimTuru = gelen.birimTuru;
            this.calistigiBirimTuru = gelen.calistigiBirimTuru;
            this.meslekKodu = gelen.meslekKodu;
        }
        public int kod { get; set; }
        public int eskiKod { get; set; }
        public int personelKod { get; set; }
        public int kisiHaric = 0;
        public int ayrilmaGerekcesi { get; set; }
        public string ayrilmaGerekcesiAd { get; set; }
        /// <summary>
        /// Organizasyon birim kodu
        /// </summary>
        public int organizasyonKodu { get; set; }

        public int personelTip { get; set; }
        public string personelTipAd { get; set; }
        public string personelTipCoklu { get; set; }

        public string birimAd { get; set; }
        /// <summary>
        /// Unvan
        /// </summary>
        public int unvan { get; set; }
        public int vekilUnvanKod { get; set; }
        /// <summary>
        /// Unvan Ad
        /// </summary>
        public string unvanAd { get; set; }
        /// <summary>
        /// Sınıf
        /// </summary>
        public int sinifi { get; set; }
        /// <summary>
        /// Sinif Ad
        /// </summary>
        public string sinifAd { get; set; }
        public string sinifKisaAd { get; set; }
        /// <summary>
        /// Açıklama
        /// </summary>
        public string evrakNo { get; set; }
        /// <summary>
        /// Açıklama
        /// </summary>
        public string yasalDayanak { get; set; }
        /// <summary>
        /// onayTarihi
        /// </summary>
        public TNSDateTime onayTarih { get; set; }
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama { get; set; }
        /// <summary>
        /// Kadro derece
        /// </summary>
        public int kadroDerece { get; set; }
        /// <summary>
        /// Kadro tarihi
        /// </summary>
        public TNSDateTime kadroTarih { get; set; }
        /// <summary>
        /// Ödemeye esas derece
        /// </summary>
        public int odemeyeEsasDerece { get; set; }
        /// <summary>
        /// Ödemeye esas kademe
        /// </summary>
        public int odemeyeEsasKademe { get; set; }
        /// <summary>
        /// Ödemeye esas tarih
        /// </summary>
        public TNSDateTime odemeyeEsasTarih { get; set; }
        /// <summary>
        /// Ödemeye esas gösterge
        /// </summary>
        public int odemeyeEsasGosterge { get; set; }
        /// <summary>
        /// Ödemeye esas ek gösterge
        /// </summary>
        public int odemeyeEsasEkGosterge { get; set; }
        /// <summary>
        /// Kazanılmış hak derece
        /// </summary>
        public int _kazanilmisHakDerece;
        public int kazanilmisHakDerece { get { return _kazanilmisHakDerece; } set { _kazanilmisHakDerece = value; } }
        /// <summary>
        /// Kazanılmış hak kademe
        /// </summary>
        public int _kazanilmisHakKademe;
        public int kazanilmisHakKademe { get { return _kazanilmisHakKademe; } set { _kazanilmisHakKademe = value; } }
        /// <summary>
        /// Kazanılmış hak tarih
        /// </summary>
        public TNSDateTime kazanilmisHakTarih { get; set; }
        /// <summary>
        /// Kazanılmış gösterge
        /// </summary>
        public int kazanilmisGosterge { get; set; }
        /// <summary>
        /// Kazanılmış ek gösterge
        /// </summary>
        public int kazanilmisEkGosterge { get; set; }
        /// <summary>
        /// Emekliliğe esas derece
        /// </summary>
        public int _emekliyeEsasDerece;
        public int emekliyeEsasDerece { get { return _emekliyeEsasDerece; } set { _emekliyeEsasDerece = value; } }
        /// <summary>
        /// Emekliliğe esas kademe
        /// </summary>
        public int _emekliyeEsasKademe;
        public int emekliyeEsasKademe { get { return _emekliyeEsasKademe; } set { _emekliyeEsasKademe = value; } }
        /// <summary>
        /// Emekliliğe esas tarih
        /// </summary>
        public TNSDateTime emekliyeEsasTarih { get; set; }
        /// <summary>
        /// Emekliliğe esas gösterge
        /// </summary>
        public int emekliyeEsasGosterge { get; set; }
        /// <summary>
        /// Emekliliğe esas ek gösterge
        /// </summary>
        public int emekliyeEsasEkGosterge { get; set; }
        public PersonelAtama atama { get; set; }
        public string personelTCKimlikNo { get; set; }
        public string personelAdSoyad { get; set; }
        public string gorevYeri { get; set; }
        public string belgeCinsi { get; set; }
        public string belgeNo { get; set; }
        public string hizmetSebebi { get; set; }
        public string hizmetSebebiAd { get; set; }
        public string onaylayan { get; set; }
        public TNSDateTime belgeTarihi { get; set; }
        public DateTime belgeTarihiDT { get { return belgeTarihi.Oku(); } }
        public TNSDateTime atamaTarihi { get; set; }
        public TNSDateTime ayrilmaTarihi { get; set; }

        public ENUMKadroDurumu kadroDurum { get; set; }
        public string kadroDurumAd { get { return IKY.Arac.KayitDurumuAdOl(kadroDurum); } }

        //public int hizmetTuru { get; set; }
        public int eskiHizmetTipi { get; set; }
        public int eskiHizmetTipiFlag { get; set; }
        public string ayrilmaNedeni { get; set; }
        public int kurumDisi { get; set; }
        public int hizmetGunSayisi { get; set; }
        public int hizmetSebebiKod { get; set; }
        //public int hizmetKurumu { get; set; }
        //public string hizmetKurumuAd { get; set; }
        //public string hizmetKurumuKayitKod { get; set; }
        //public int OnSekizYasAltiPrimGünü { get; set; }
        //public int primGunu { get; set; }
        public int KHADurumu { get; set; }
        public int kidemGunu { get; set; }
        //public int KHAGunu { get; set; }
        //public int EKEAGunu { get; set; }
        //public int _yedekArtis;
        //public int yedekArtis { get { return _yedekArtis; } set { _yedekArtis = value; } }

        public string muhasebeBirimi { get; set; }
        public int _muhasebeKodu;
        public int muhasebeKodu { get { return _muhasebeKodu; } set { _muhasebeKodu = value; } }
        public string kurumsalKod { get; set; }
        public string vekilUnvanAd { get; set; }
        public ENUMHizmetSebebiNevi hizmetSebebiTip { get; set; }
        public ENUMHizmetSebebiNevi ayrilmaGerekcesiTip { get; set; }
        public YukselecegiDerece derece { get; set; }
        public ENUMHizmetTablolari hizmetHesabi { get; set; }
        public DateTime ayrilmaTarihiDT { get { return ayrilmaTarihi.Oku(); } }
        public DateTime atamaTarihiDT { get { return atamaTarihi.Oku(); } }

        public DateTime emekliyeEsasTarihDT { get { return emekliyeEsasTarih.Oku(); } }
        public DateTime odemeyeEsasTarihDT { get { return odemeyeEsasTarih.Oku(); } }
        public DateTime kazanilmisHakTarihDT { get { return kazanilmisHakTarih.Oku(); } }
        public DateTime kadroTarihDT { get { return kadroTarih.Oku(); } }
        public DateTime onayTarihDT { get { return onayTarih.Oku(); } }

        public bool terfiMi { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public int iliskiliKadroIslemNo { get; set; }

        public int atananacagiKadroUnvanKod { get; set; }

        public int atananacagiKadroDerece { get; set; }

        public void GostergeDuzenle()
        {
            this.odemeyeEsasGosterge = TNS.IKY.Arac.GostergeBul(this.odemeyeEsasDerece, this.odemeyeEsasKademe);
            this.emekliyeEsasGosterge = TNS.IKY.Arac.GostergeBul(this.emekliyeEsasDerece, this.emekliyeEsasKademe);
            this.kazanilmisGosterge = TNS.IKY.Arac.GostergeBul(this.kazanilmisHakDerece, this.kazanilmisHakKademe);
        }

        public bool bosKadrolar = false;
        public string muhasebeAd = "";
        public string muhasebeBirimiAd = "";
        public int gorevOrganizasyonKodu { get; set; }
        public string gorevBirimAd { get; set; }

        public long hitapKod { get; set; }

        public string hitapkhaDurum { get; set; }

        public ENUMSGKNevi hitapsgkNevi { get; set; }

        public string hitapSicilNo { get; set; }

        public string hitapBorcNevi { get; set; }

        public double hitaptoplamTutar { get; set; }

        public double hitapodenenMiktar { get; set; }

        public TNSDateTime hitapodemeTarihi { get; set; }

        public string birimKisaAd { get; set; }

        public string birimGorevAd
        {
            get
            {
                return AdBul(true);
            }
        }

        public string AdBul(bool kisaAd)
        {
            if (kurumDisi == 0)
            {
                if (kisaAd && !string.IsNullOrWhiteSpace(birimKisaAd))
                    return birimKisaAd;
                if (!string.IsNullOrWhiteSpace(birimAd))
                    return birimAd;
            }
            return gorevYeri;
        }

        public string organizasyonKopruKodu { get; set; }

        public int unvanSira { get; set; }

        public int tayinOlacagiBirim { get; set; }
        public string tayinOlacagiBirimAd { get; set; }

        public string gitOrganizasyonKopruKodu { get; set; }

        public string calistigiBirimTuru { get; set; }

        public string birimTuru { get; set; }
        public string meslekKodu { get; set; }
    }

    /// <summary>
    /// Personel Atama bilgisi.
    /// </summary>
    [Serializable()]
    public class PersonelAtama
    {
        public int kod = 0;
        public int pkkod = 0;
        public int pkEskiKod = 0;
        public string atamaNedeni = "";
        public int atamaTuru = 0;
        public string atamaTuruAd = "";
        public int atamaSekli = 0;
        public string atamaSekliAd = "";
        public TNSDateTime atamaTarihi = new TNSDateTime();
        public string onaylayan = "";
        public TNSDateTime onayTarihi = new TNSDateTime();
        public int hareketDurumu = 0;
        public string hareketDurumuAd = "";
        public string hareketDurumuKayitKod = "";
        public bool onayliMi = false;
    }

    /// <summary>
    /// Unvan bilgisi.
    /// </summary>
    [Serializable()]
    public class Unvan
    {
        public ENUMPersonelSira listeSiralama = ENUMPersonelSira.UnvanAd;

        public Unvan()
        {
            kod = 0;
            ad = "";
            sinifAdKisa = "";
            siraNo = 0;
            yonetici = 0;
            sinifKod = 0;
            minYil = 0;
            maxYil = 0;
            hizmetGunu = 0;
            sinifAd = "";
            aktif = false;
        }

        public int kod { get; set; }
        public string ad { get; set; }
        public string sinifAdKisa { get; set; }
        public int siraNo { get; set; }
        public int yonetici { get; set; }
        public int sinifKod { get; set; }
        public int minYil { get; set; }
        public int maxYil { get; set; }
        public int hizmetGunu { get; set; }
        public string sinifAd { get; set; }
        public bool aktif { get; set; }

        public string yoneticiAd { get { return yonetici == 0 ? "Hayır" : "Evet"; } }
        public string aktifAd { get { return aktif ? "Evet" : "Hayır"; } }
        public int UnvanKod { get { return kod; } set { kod = value; } }
        public string UnvanAd { get { return ad + " (" + sinifAdKisa + ")"; } }

        public int hitapKod { get; set; }
    }

    /// <summary>
    /// Sınıf bilgisi.
    /// </summary>
    [Serializable()]
    public class Sinif
    {
        public int kod = 0;
        public string ad = "";
    }

    /// <summary>
    /// Başlama ve Ayrılma nedenleri.
    /// </summary>
    [Serializable()]
    public class Neden
    {
        public int kod = 0;
        public string ad = "";
        public int baslama = 0;
        public int ayrilma = 0;
    }

    /// <summary>
    /// UstBirim bilgisi.
    /// </summary>
    [Serializable()]
    public class UstBirim
    {
        public int kod = 0;
        public string ad = "";
    }

    /// <summary>
    /// Banka bilgisi.
    /// </summary>
    [Serializable()]
    public class Banka
    {
        public string kod = "";
        public string ad = "";
    }

    /// <summary>
    /// Banka Şube bilgisi.
    /// </summary>
    [Serializable()]
    public class BankaSube
    {
        public string kod = "";
        public string bankaKodu = "";
        public string ad = "";
        public string bankaAd = "";
    }

    /// <summary>
    /// Dil bilgisi.
    /// </summary>
    [Serializable()]
    public class Dil
    {
        public int kod = 0;
        public string ad = "";
    }

    /// <summary>
    /// Kurs bilgisi.
    /// </summary>
    [Serializable()]
    public class Kurs
    {
        public int kod = 0;
        public string ad = "";
    }


    /// <summary>
    /// Öğrenim bilgisi.
    /// </summary>
    [Serializable()]
    public class Ogrenim
    {
        public int kod = 0;
        public string ad = "";
        public int siraNo = 0;
        public int tip = 0;
    }

    /// <summary>
    /// İzin Tür Bilgisi.
    /// </summary>
    [Serializable()]
    public class IzinTur
    {
        public int kod { get; set; }
        public string ad { get; set; }
        public string kodVeSure
        {
            get
            {
                return kod.ToString() + ";" + sure.ToString();
            }
        }

        public int sure = 0;
        public int gorulebilirlik = 0;
        public int sira = 0;

        public IzinTur(int kod, string ad, int sure, int gorulebilirlik, int sira)
        {
            this.kod = kod;
            this.ad = ad;
            this.sure = sure;
            this.gorulebilirlik = gorulebilirlik;
            this.sira = sira;
        }
        public IzinTur()
        {
        }
    }

    /// <summary>
    /// kadro bilgisi.
    /// </summary>
    [Serializable()]
    public class Kadro
    {
        public int teskilat = 0;
        public int ustBirim = 0;
        public int sinif = 0;
        public int unvan = 0;
        public int kadro = 0;
        public int sayIl = 0;
        public int sayTip = 0;
        public int sayIlce = 0;
        public int altIl = 0;
        public int altTip = 0;
        public int altIlce = 0;
        public int dolu = 0;
        public int bos = 0;
        public int adet = 0;
        public int oncesiDolu = 0;
        public int oncesiBos = 0;
        public int oncesiAdet = 0;
        public TNSDateTime islemTarih = new TNSDateTime();
        public TNSDateTime vizeTarih = new TNSDateTime();
        public int yil = 0;
        public int teklifNo = 0;
        public int icmalNo = 0;
        public int siraNo = 0;
        public int tahsisTenkis = 0;
        public string ustBirimAd = "";
        public string sinifAd = "";
        public string unvanAd = "";
        public string muhasebeAd = "";
        public string teskilatAd = "";
    }

    [Serializable()]
    public class SozlesmeliBordroTanim
    {
        public string tcKimlikNo = "";
        public int yil = 0;
        public int ay = 0;
        public decimal kumulatifVergiMatrahi = 0;
    }

    [Serializable()]
    public class OgrenimBazliPersonelSayi
    {
        public Ogrenim ogrenim;
        public int personelSayi;
    }

    [Serializable()]
    public class UnvanBazliPersonelSayi
    {
        public Unvan unvan;
        public int personelSayi;
    }

    [Serializable()]
    public class SinifBazliPersonelSayi
    {
        public Sinif sinif;
        public int personelSayi;
    }

    [Serializable()]
    public class UnvanVeKadroDereceBazliPersonelSayi
    {
        public Unvan unvan;
        public int kadroDerece;
        public int personelSayi;
        public int kontenjan;
    }

    [Serializable()]
    public class KadroKontenjan
    {
        public int kod;
        public int organizasyonKod;
        public string organizasyonAd;
        public int islemNo;
        public EnumKadroKontenjanIslemTur islemTur;
        public TNSDateTime islemTarih;
        public string aciklama;
        public KadroKontenjanDetay[] detaylar;
        public TNSDateTime dayanakBelgeTarih;
        public string dayanakBelgeNo;
        public ENUMPersonelTur personelTur;

        public string organizasyonKopruKod { get; set; }

        public string organizasyonKadroKod { get; set; }
    }

    [Serializable()]
    public class KadroKontenjanDetay
    {
        public int kkod;
        public int unvanKod;
        public string unvanAd;
        public int kadroDerece;
        public int kontenjan;
        public string sinifAd;
    }

    [Serializable()]
    public class KadroDoluBos
    {
        public KadroDoluBos()
        {
            unvanKod = 0;
            kadroDerece = 0;
            unvanAd = "";
            sinifAd = "";
            kontenjan = 0;
            dolu = 0;
            siraNo = 0;
        }

        public int unvanKod { get; set; }
        public string unvanAd { get; set; }
        public string sinifAd { get; set; }
        public int kadroDerece { get; set; }
        public int kontenjan { get; set; }
        public int dolu { get; set; }
        public int bos { get { return kontenjan - dolu; } }
        public int siraNo { get; set; }

        public string ID { get { return unvanKod.ToString() + '-' + kadroDerece.ToString(); } }
        public string Aciklama { get { if (unvanKod == 0) return ""; else return unvanAd + " (" + sinifAd + ") " + kadroDerece.ToString() + ". Derece (Boş: " + bos.ToString() + ")"; } }
    }

    [Serializable()]
    public class IzinBilgi
    {
        public int yilIzni { get; set; }
        public int toplam { get { return devreden + yilIci; } }
        public int devreden { get; set; }
        public int yilIci { get; set; }
        public int devirdenKullanilan { get { if (yilIzni < devreden) return yilIzni; else return devreden; } }
        public int yildanKullanilan { get { if (yilIzni > devreden) return yilIzni - devreden; else return 0; } }
        public int kalan { get { return toplam - yilIzni; } }
    }

    [Serializable()]
    public class PersonelIzin
    {
        public int kod { get; set; }
        public int personelKod { get; set; }
        public string tcKimlikNo = "";
        public string personelAd = "";
        public string personelSicilNo = "";
        public string personelBirim = "";

        public int izinTurKod { get; set; }
        public string izinTurAd
        {
            get
            {
                return IzinTipAdGetir(this.izinTurKod);
            }
        }

        public static string IzinTipAdGetir(int izinTurKod)
        {
            if (Arac.izinTipListesi == null)
            {
                TNS.IKY.Arac.izinTipListesi = TNS.IKY.Arac.Tanimla().IzinTurListele(new IzinTur());
            }
            foreach (IzinTur item in Arac.izinTipListesi.objeler)
            {
                if (item.kod == izinTurKod) return item.ad;
            }
            return "";
        }

        public string aciklama { get; set; }

        public TNSDateTime baslangicTarihi = new TNSDateTime();
        public DateTime baslangicTarihiDT
        {
            get
            {
                return baslangicTarihi.Oku();
            }
            set
            {
                baslangicTarihi.Yaz(value);
            }
        }

        public TNSDateTime bitisTarihi = new TNSDateTime();
        public DateTime bitisTarihiDT
        {
            get
            {
                return bitisTarihi.Oku();
            }
            set
            {
                bitisTarihi.Yaz(value);
            }
        }

        public string telefon { get; set; }
        public int kullanilanGun { get; set; }
        public int devreden = 0;
        public string izinYeri { get; set; }
        public int onay { get; set; }
        public string onayAd
        {
            get
            {
                return onay == 1 ? "+" : "-";
            }
        }

        public string evrEvrakNo { get; set; }
        public string donem = string.Empty;
        public string belgeNo { get; set; }
        public TNSDateTime belgeTarihi = new TNSDateTime();
        public DateTime belgeTarihiDT
        {
            get
            {
                return belgeTarihi.Oku();
            }
            set
            {
                belgeTarihi.Yaz(value);
            }
        }

        public int imzalayan { get; set; }
        public int teklifEden { get; set; }
        public int yerineGorevli { get; set; }
        public string imzalayanAd { get; set; }
        public string teklifEdenAd { get; set; }
        public string yerineGorevliAd { get; set; }
        public string saglikKurulusu { get; set; }
        public string memuriyeti { get; set; }
        public string izinTipAd { get; set; }
        public string imzalayanUnvan { get; set; }
        public string teklifEdenUnvan { get; set; }
        public TNSDateTime kayitTarihi = new TNSDateTime();
        public DateTime kayitTarihiDT
        {
            get
            {
                return kayitTarihi.Oku();
            }
            set
            {
                kayitTarihi.Yaz(value);
            }
        }
        public int donemYil { get; set; }

        public TNSDateTime raporTarihi = new TNSDateTime();
        public DateTime raporTarihiDT
        {
            get
            {
                return raporTarihi.Oku();
            }
            set
            {
                raporTarihi.Yaz(value);
            }
        }

        public string raporNo { get; set; }

        public int yatiliGecenSure { get; set; }
        public string yerKod { get; set; }
        public string yerAd { get; set; }

        public string ulkeAd { get; set; }

        public string izinNotu2 { get; set; }

    }

    [Serializable()]
    public class PersonelIzinDevir
    {
        public PersonelIzinDevir()
        {
            kod = 0;
            personelKod = 0;
            yil = 0;
            devreden = 0;
            yilici = 0;
            birimAd = "";
            arama = "";
        }

        public int kod { get; set; }
        public int personelKod { get; set; }
        public int yil { get; set; }
        public string yilAd { get { return TNS.IKY.IzinArac.DonemAdBul(yil); } }
        public int devreden { get; set; }
        public int yilici { get; set; }
        public string arama { get; set; }
        public string birimAd { get; set; }
        public string sicilNo { get; set; }
        public string tcKimlikNo { get; set; }
        public string adSoyad { get; set; }

        public string personelTipleri { get; set; }

        public int kullanilan { get; set; }

        public int kidemGun { get; set; }
    }

    [Serializable()]
    public class PersonelTarihce
    {
        public string personel;
        public int kod;
        public int personelKod;
        public string kullanici;
        public TNSDateTime tarih;
        public string degisiklik;
        public string kullaniciAd;

        //public Object yeni;
        //public Object eski;
        //public ENUMTARIHCE alan;
    }

    /// <summary>
    /// Görevlendirme Konu bilgisi.
    /// </summary>
    [Serializable()]
    public class GorevKonu
    {
        public string kod = "";
        public string ad = "";
    }

    /// <summary>
    /// Sozluk bilgisi.
    /// </summary>
    [Serializable()]
    public class Sozluk
    {
        public int KOD { get { return kod; } set { kod = value; } }
        public string AD { get { return ad; } set { ad = value; } }
        public int kod = 0;
        public int anaKod = 0;
        public string hitapID = "";
        public string anaKayitAd = "";
        public string anaKayit = "";
        public string kayitKod = "";
        public string ad = "";
        public string kisaAd = "";
        public int seviye = 0;
        public bool birebir = false;
        public bool ozel = false; //kayit kod kullanılacağı zaman
    }
    [Serializable]
    public class IKYHitap
    {
        public int kod = 0;
        public int personelKod = 0;
        public int tipKayitKod = 0;
        public int tip = 0;
        public string hitapKayitNo = "";
        public int islemTuru = 0;

    }

    /// <summary>
    /// Personel Kart bilgisi.
    /// </summary>
    [Serializable()]
    public class PersonelKart
    {
        public int kod = 0;
        public int personelKod = 0;
        public int iadeAlanPersonel = 0;
        public int zimmetEdenPersonel = 0;
        public int aktifMi = 0;
        public int kartTipi = 0;
        public string kartNo = "";
        public ENUMYakinlikDerecesi yakinligi;
        public string yakinlikAd = "";
        public string plaka = "";
        public string adiSoyadi = "";
        public string tagCihazNo = "";
        public string aciklama = "";
        public string kartTipiAd = "";
        public string iadeAlanPersonelAd = "";
        public string zimmetEdenPersonelAd = "";
        public TNSDateTime kayipTarihi = new TNSDateTime();
        public TNSDateTime zimmetTarihi = new TNSDateTime();
        public TNSDateTime iadeTarihi = new TNSDateTime();
        public string personelAd = "";
        public string unvan = "";
        public int siraNo = 0;
    }

    public enum ENUMSGKNevi
    {
        SSKKamu = 1,
        SSKOzel = 2,
        BagKur = 3,
        YurtDisi = 4,
        BankaEmekliSandiklari = 5,
        SSKAskerlikBorclanmasi = 6,
        SSKDigerBorclanma = 7,
        BagKurAskerlikBorclanmasi = 8,
        BagKurDigerBorclanma = 9,
        BankaEmekliSandiklariAskerlikBorclanmasi = 10,
        BankaEmekliSandiklariDigerBorclanma = 11,
        SSK_FHZ = 12
    }

    public enum ENUMHizmetSebebiNevi
    {
        TANIMSIZ = 0,
        BASLAMA = 1,
        AYRILMA = 2,
        TERFI = 5,
        NAKILBASLAMA = 6,
        NAKILAYRILMA = 206,

        UCRETSIZIZIN = 7,
        UCRETSIZAYRILMA = 207,

        GOREVDENUZAK = 8,
        GOREVUZAKAYRILMA = 208,

        YURTDISIEGITIM = 9,
        YURTDISIEGITIMEAYRILMA = 209,

        BASLAMAGRUBU = 98,
        AYRILMAGRUBU = 99
    }

    [Serializable()]
    public class HizmetSebebi
    {
        public string tipKod { get { return TipKodOl(tip, kod); } }
        public int kod { get; set; }
        public string sebepAdi { get; set; }
        //public string tarihZorunluluk { get; set; }
        public ENUMHizmetSebebiNevi tip { get; set; }
        public string tipAd { get { return TipAd(tip); } }

        public static string TipKodOl(ENUMHizmetSebebiNevi tip, int kod)
        {
            int t = (int)tip;
            if (t > 0 && kod > 0)
            {
                return t.ToString() + "-" + kod.ToString();
            }
            return "";
        }

        public static string TipAd(ENUMHizmetSebebiNevi tip)
        {
            switch (tip)
            {
                case ENUMHizmetSebebiNevi.TANIMSIZ: return "-";
                case ENUMHizmetSebebiNevi.BASLAMA: return "Kuruma Başlama";
                case ENUMHizmetSebebiNevi.AYRILMA: return "Kurumdan Ayrılma";
                case ENUMHizmetSebebiNevi.TERFI: return "Terfi";
                case ENUMHizmetSebebiNevi.NAKILBASLAMA: return "Nakil ile Başlama";
                case ENUMHizmetSebebiNevi.NAKILAYRILMA: return "Nakil ile Ayrılış";
                case ENUMHizmetSebebiNevi.UCRETSIZIZIN: return "Ücretsiz İzin";
                case ENUMHizmetSebebiNevi.UCRETSIZAYRILMA: return "Ücretsiz İzne Ayrılış";
                case ENUMHizmetSebebiNevi.BASLAMAGRUBU: return "Tüm Başlamalar";
                case ENUMHizmetSebebiNevi.AYRILMAGRUBU: return "Tüm Ayrılmalar";
                case ENUMHizmetSebebiNevi.GOREVDENUZAK: return "Görevden Uzak Dönem";
                case ENUMHizmetSebebiNevi.GOREVUZAKAYRILMA: return "Görevden Uzaklaştırıldı";

                case ENUMHizmetSebebiNevi.YURTDISIEGITIM: return "Yurtdışı Eğitim Dönemi";
                case ENUMHizmetSebebiNevi.YURTDISIEGITIMEAYRILMA: return "Yurtdışı Eğitim İçin Ayrılma";
            }
            return "";
        }

        public static string BaslamaGrubuListe()
        {
            return "1,5,6,7,8,9";
        }

        public static string AyrilmaGrubuListe()
        {
            return "2,206";
        }
    }

    [Serializable()]
    public class EskiHizmetTipi
    {
        public int kod;
        public string hizmetAdi;
        public int toplam;
        public int aylik;
        public int emekli;
        public int izin;
        public int cb;
    }

    [Serializable()]
    public class IzinGunleri
    {
        public int toplam;
        public int kurum;
        public int kadro;
    }

    [Serializable()]
    public class PersonelIzinGunleri
    {
        public int yilIci;
        public int devir;
        public int kullanilan;
        public int kalan;
        public Sonuc sonuc;
    }

    [Serializable()]
    public class Hizmet
    {
        public static int YilDondur(int toplamGun)
        {
            return toplamGun / 360;
        }

        public static string YilAyGunDondur(int toplamGun)
        {
            string yilAyGun = "";
            int yil = toplamGun / 360;
            int ay = (toplamGun % 360) / 30;
            int gun = (toplamGun % 360) % 30;
            yilAyGun = yil + " yıl " + ay + " ay " + gun + " gün";

            return yilAyGun;
        }


        public int siraNo = 0;
        public int kod = 0;
        public int pKod = 0;

        public string hizmetYilAyGun { get { return YilAyGunDondur(hizmetGunu); } }
        public int hizmetGunu = 0;
        public int hizmetYili { get { return YilDondur(hizmetGunu); } }

        public string kidemYilAyGun { get { return YilAyGunDondur(kidemGunu); } }
        public int kidemGunu = 0;
        public int kidemYili { get { return YilDondur(kidemGunu); } }

        public string KHAYilAyGun { get { return YilAyGunDondur(KHAGunu); } }
        public int KHAGunu = 0;

        public string EKEAYilAyGun { get { return YilAyGunDondur(EKEAGunu); } }
        public int EKEAGunu = 0;

        public int izinHizmetGunu = 0;
        public int kurumHizmetGunu = 0;
        public int kadroHizmetGunu = 0;
        public DateTime baslangicTarihi = new DateTime();
        public DateTime bitisTarihi = new DateTime();
        public int hizmetKurumu = 0;
        public string hizmetKurumuKayitKod = "";
        public string personelAd = "";
        public BirimUnvan guncelHizmet = new BirimUnvan();
        public DateTime kadroyaGecmeTarihi = new DateTime();
        public DateTime kurumaBasladigiTarihi = new DateTime();
        public int kurumdakiSure = 0;
        public int kadrodakiSure = 0;
        public IzinGunleri izin = new IzinGunleri();
    }

    [Serializable()]
    public class Terfi
    {
        public int kod = 0;
        public int pKadroKod = 0;
        public int pYeniKadroKod = 0;
        public int pKod = 0;
        public int tip = 0;
        public string kimlikNo = "";
        public string sicilNo = "";
        public string ad = "";
        public string birim = "";
        public string unvan = "";
        public int kadroDerece = 0;
        public TNSDateTime kadroTarih = new TNSDateTime();
        public int odemeyeEsasDerece = 0;
        public int odemeyeEsasKademe = 0;
        public TNSDateTime odemeyeEsasTarih = new TNSDateTime();
        public int kazanilmisHakDerece = 0;
        public int kazanilmisHakKademe = 0;
        public TNSDateTime kazanilmisHakTarih = new TNSDateTime();
        public int emekliyeEsasDerece = 0;
        public int emekliyeEsasKademe = 0;
        public TNSDateTime emekliyeEsasTarih = new TNSDateTime();
        public int durum;
        public int hizmetSebebi;
        public string ogrenim = "";
        public int yukselecegiDerece = 0;
        public int yil = 0;
        public int ay = 0;
        public string sebepAd = "";
        public int YeniOdemeyeEsasDerece = 0;
        public int YeniOdemeyeEsasKademe = 0;
        public int YeniOdemeyeEsasEkGosterge = 0;
        public int YeniKazanilmisHakDerece = 0;
        public int YeniKazanilmisHakKademe = 0;
        public int YeniKazanilmisHakEkGosterge = 0;
        public int YeniEmekliyeEsasDerece = 0;
        public int YeniEmekliyeEsasKademe = 0;
        public int YeniEmekliyeEsasEkGosterge = 0;
        public int kazanilmisHakEkGosterge = 0;
        public int odemeyeEsasEkGosterge = 0;
        public int emekliyeEsasEkGosterge = 0;
        public string statu = "";

        public bool EKEASonMu()
        {
            return SonMu(this.emekliyeEsasDerece, this.emekliyeEsasKademe);
        }

        public bool KHASonMu()
        {
            return SonMu(this.kazanilmisHakDerece, this.kazanilmisHakKademe);
        }

        public static bool SonMu(int kazanilmisHakDerece, int kazanilmisHakKademe)
        {
            if (kazanilmisHakDerece <= 1 && kazanilmisHakDerece >= 4) return true;
            return false;
        }
    }

    [Serializable()]
    public class HarcamaBirimi
    {
        public int birimKod = 0;
        public string muhasebeKodu = "";
        public string muhasebeBirimi = "";
        public string kurumsalKod = "";
    }

    [Serializable()]
    public class PersonelKadroRapor
    {
        public int siraNo = 0;
        public int adet = 0;
        public string birim = "";
        public string unvan = "";
        public string sinif = "";
        public int kadroDerece = 0;
        public int kEkGosterge = 0;
        public int kKademe = 0;
        public int kDerece = 0;
        public int eEkGosterge = 0;
        public int eKademe = 0;
        public int eDerece = 0;
        public string statu = "";
        public string isim = "";
        public int personelKod = 0;
        public YukselecegiDerece derece = new YukselecegiDerece();
    }

    [Serializable()]
    public class MailImzaRapor
    {
        public string isim;
        public string unvan;
        public string birim;
        public string cepTel;
        public string isTel;
        public string fax;
    }

    [Serializable()]
    public class BirimUnvan
    {
        public string unvan { get; set; }
        public string gorevUnvan { get; set; }
        public string birim { get; set; }
        public string personelAd { get; set; }
        public string personelTC { get; set; }
        public string personelSicil { get; set; }
        public string geldigiKurum { get; set; }
        public int kadroDerece { get; set; }
        public int ekGosterge { get; set; }
        public int kDerece { get; set; }
        public int kKademe { get; set; }
        public int eDerece { get; set; }
        public int eKademe { get; set; }
        public int personelKod { get; set; }
        public DateTime kurumaBasladigiTarih { get; set; }
        public DateTime memuriyeteBasladigiTarih { get; set; }
        public DateTime kadroyaGectigiTarih { get; set; }
        public int hizmetGunu_Yil { get; set; }
        public BirimUnvan()
        {
            unvan = "";
            birim = "";
            personelAd = "";
            personelTC = "";
            personelSicil = "";
            geldigiKurum = "";
            kadroDerece = 0;
            ekGosterge = 0;
            kDerece = 0;
            kKademe = 0;
            eDerece = 0;
            eKademe = 0;
            personelKod = 0;
            kurumaBasladigiTarih = new DateTime();
            memuriyeteBasladigiTarih = new DateTime();
            kadroyaGectigiTarih = new DateTime();
        }
    }

    [Serializable()]
    public class KadroAyar
    {
        public int pKod { get; set; }
        public int kTerfiAyi { get; set; }
        public int kidemTerfiAyi { get; set; }
        public int eTerfiAyi { get; set; }
        public int maximumDerece { get; set; }
        public int aktif { get; set; }
        public TNSDateTime eAcilisTarihi { get; set; }
        public TNSDateTime kAcilisTarihi { get; set; }
        public KadroAyar()
        {
            pKod = 0;
            kTerfiAyi = 0;
            kidemTerfiAyi = 0;
            eTerfiAyi = 0;
            maximumDerece = 0;
            aktif = 0;
            kAcilisTarihi = new TNSDateTime();
            eAcilisTarihi = new TNSDateTime();
        }
    }

    [Serializable()]
    public class YukselecegiDerece
    {
        public string ogrenim;
        public int derece;
    }

    [Serializable()]
    public class OgrenimRaporu
    {
        public string personelAd = "";
        public string unvan = "";
        public string birim = "";
        public string okulTipi = "";
        public string okulAdi = "";
        public string statu = "";
        public int siraNo = 0;
        public int yasGunu = 0;
        public int kurumHizmetGunu = 0;
    }

    [Serializable()]
    public class AileRaporu
    {
        public string personelAd = "";
        public string personelUnvan = "";
        public int personelKod = 0;
        public PersonelAile[] yakinlar;
    }

    [Serializable()]
    public class DinamikRapor
    {
        public int personelKod;
        public byte[] resim;
        public string personelAd;
        public string personelSoyad;
        public string personelTC;
        public string sicilNo;
        public string unvan;
        public string birim;
        public string tip;
        public TNSDateTime baslamaTarihi;
        public TNSDateTime ayrilisTarihi;
        public string cepTel;
        public string dahiliTel;
        public string personelBabaAdi;
        public int kadroDerece;
        public int ekGosterge;
        public string dogumYeri;
        public TNSDateTime dogumTarihi;
        public string nufusIlcesi;
        public string ogrenimDurumu;
        public string geldigiKurum;
        public string okulAdi;
    }

    [Serializable()]
    public class MaasParam
    {
        public string degiskenAd { get; set; }
        public string aciklama { get; set; }
        public TNS.VariableType tip { get; set; }

        public object deger
        {
            get
            {
                if (tip == VariableType.Money) return degerDbl;
                else return degerStr;
            }
        }

        public string degerGenel
        {
            get
            {
                if (tip == VariableType.Money) return degerDbl.ToString("#,##0.######");
                else return degerStr;
            }
            set
            {
                degerDbl = OrtakFonksiyonlar.ConvertToDbl(value);
                degerStr = value;
            }
        }

        public double degerDbl { get; set; }
        public string degerStr { get; set; }

        public MaasParam()
        {
            tip = VariableType.Money;
            this.degiskenAd = "";
            this.aciklama = "";
            this.degerDbl = 0;
            this.degerStr = "";
        }

        public MaasParam(string degiskenAd, VariableType vTip, string aciklama)
        {
            this.tip = vTip;
            this.degiskenAd = degiskenAd;
            this.aciklama = aciklama;
            this.degerDbl = 0;
            this.degerStr = "";
        }

        public MaasParam(string degiskenAd, string aciklama, double deger)
        {
            tip = VariableType.Money;
            this.degiskenAd = degiskenAd;
            this.aciklama = aciklama;
            this.degerDbl = deger;
            this.degerStr = "";
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(string))
            {
                if (degiskenAd == (string)obj) return true;
            }
            return base.Equals(obj);
        }
    }

    [Serializable()]
    public class PersonelMaasMatrah : IComparable
    {
        public PersonelMaasMatrah()
        {
            tabloAd = "";
            satirNo = 0;
            matrahTur = 1;
        }

        public string tcKimlikNo { get; set; }
        public int yil { get; set; }
        public string surecNo { get; set; }
        public int matrahTur { get; set; }
        public string tabloAd { get; set; }
        public int satirNo { get; set; }
        public string matrahTurAd
        {
            get
            {
                return MatrahTurAd(matrahTur);
            }
            set
            {
                for (int i = 1; i <= matrahTurAdlar.Length; i++)
                {
                    if (value == matrahTurAdlar[i - 1]) { matrahTur = i; return; }
                }
                matrahTur = 0;
            }
        }
        public static string[] matrahTurAdlar =  {
            "Gelir Ver.",
            "SGK Mat.",
            "SGK Matrah Üstü (Bu Ay)",
            "SGK Matrah Üstü (Önc. Ay)",
            "AGİ Devreden",
            "GV İstisna Devreden",
            "DV İstisna Devreden",
            "Gelir Ver. Hesap",
            "Asgari Ücret Matrah",
            "GV Hes. İstisna Devr.",
            "DV Hes. İstisna Devr.",
        };

        public static string MatrahTurAd(int matrahTur)
        {
            if (matrahTur >= 1 && matrahTur <= matrahTurAdlar.Length)
                return matrahTurAdlar[matrahTur - 1];
            return "";
        }

        public TNSDateTime tarih { get; set; }
        public DateTime tarihDT
        {
            get
            {
                if (tarih == null) tarih = new TNSDateTime();
                return tarih.Oku();
            }
            set
            {
                if (tarih == null) tarih = new TNSDateTime();
                tarih.Yaz(value);
            }
        }
        public double deger { get; set; }
        public int matPerTur
        {
            get
            {
                if (!string.IsNullOrEmpty(tabloAd))
                {
                    string[] tP = tabloAd.Split('_');
                    if (tP.Length == 4)
                    {
                        return OrtakFonksiyonlar.ConvertToInt(tP[2], 0);
                    }
                }
                return 0;
            }
        }

        public int CompareTo(object obj)
        {
            if (obj is PersonelMaasMatrah)
                return this.tcKimlikNo.CompareTo(((PersonelMaasMatrah)obj).tcKimlikNo);
            return 1;
        }
    }

    [Serializable()]
    public class PersonelMaasParam
    {
        public TNSCollection paramlar;
        public TNSCollection matrahlar;

        public double ParamDegerBul(string paramAd)
        {
            if (paramlar == null) return 0;
            int ind = paramlar.IndexOf(paramAd);
            if (ind >= 0)
                return ((MaasParam)paramlar[ind]).degerDbl;
            return 0;
        }

        public string ParamDegerBulStr(string paramAd)
        {
            if (paramlar == null) return "";
            int ind = paramlar.IndexOf(paramAd);
            if (ind >= 0)
                return ((MaasParam)paramlar[ind]).degerStr;
            return "";
        }

        public enum ENUMMatrahKod : int
        {
            GelirVergisi = 1,
            SGKDevreden = 2,
            SGKMtrhBirOnce = 3,
            SGKMtrhIkiOnce = 4,
            AGIDevreden = 5,

            GVIstisnaDevir = 6,
            DVIstisnaDevir = 7,
            GelirVergisiHesap = 8,
            AsgariUcretMatrah = 9,
            GVHesIstisnaDevir = 10,
            DVHesIstisnaDevir = 11,
        }

        public double MatrahDegerBul(ENUMMatrahKod matrahKod, DateTime tarih, string surecNo, int perTur)
        {
            double donusDeger = 0;
            if (matrahlar != null)
            {
                foreach (PersonelMaasMatrah item in matrahlar)
                {
                    if (item.matrahTur == (int)matrahKod && (surecNo != item.surecNo) && (item.tarihDT < tarih || (item.tarihDT == tarih && perTur != item.matPerTur)))
                    {
                        bool ekle = false;
                        if ((matrahKod == ENUMMatrahKod.GelirVergisi || matrahKod == ENUMMatrahKod.GelirVergisiHesap || matrahKod == ENUMMatrahKod.AsgariUcretMatrah) && AyniYilMi(item.tarihDT, tarih))
                            ekle = true;
                        if ((matrahKod == ENUMMatrahKod.DVIstisnaDevir || matrahKod == ENUMMatrahKod.GVIstisnaDevir || matrahKod == ENUMMatrahKod.GVHesIstisnaDevir || matrahKod == ENUMMatrahKod.DVHesIstisnaDevir || matrahKod == ENUMMatrahKod.SGKDevreden || matrahKod == ENUMMatrahKod.AGIDevreden) && AyniAyMi(item.tarihDT, tarih))
                            ekle = true;
                        if ((matrahKod == ENUMMatrahKod.SGKMtrhBirOnce || matrahKod == ENUMMatrahKod.SGKMtrhIkiOnce) && OncekiAyMi(item.tarihDT, tarih))
                            ekle = true;
                        if (ekle)
                            donusDeger += item.deger;
                    }
                }
            }
            return donusDeger;
        }

        private bool AyniYilMi(DateTime tarihDT, DateTime tarih)
        {
            return (tarihDT.Year == tarih.Year);
        }
        private bool AyniAyMi(DateTime tarihDT, DateTime tarih)
        {
            return (tarihDT.Year == tarih.Year && tarihDT.Month == tarih.Month);
        }
        private bool OncekiAyMi(DateTime tarihDT, DateTime tarih)
        {
            return (tarihDT.Year == tarih.Year && tarihDT.Month == tarih.Month - 1) ||
                (tarihDT.Year == tarih.Year - 1 && tarihDT.Month == 12 && tarih.Month == 1);
        }
    }

    /// <summary>
    /// LojmanSite: 
    /// </summary>
    [System.Serializable()]
    public class LojmanSite
    {

        /// <summary>
        /// KOD: Kodu giriniz.
        /// </summary>
        public int kod { get; set; }

        /// <summary>
        /// Site Adi: Site Adını giriniz.
        /// </summary>
        public string siteAdi { get; set; }

        /// <summary>
        /// Mahalle: Mahalle giriniz.
        /// </summary>
        public string mahalle { get; set; }

        /// <summary>
        /// Cadde: Cadde giriniz.
        /// </summary>
        public string cadde { get; set; }

        /// <summary>
        /// Sokak: Sokak giriniz.
        /// </summary>
        public string sokak { get; set; }

        /// <summary>
        /// Semt: Semt giriniz.
        /// </summary>
        public string semt { get; set; }

        /// <summary>
        /// İlçe: İlçe giriniz.
        /// </summary>
        public string ilce { get; set; }

        /// <summary>
        /// İl: İl giriniz.
        /// </summary>
        public string il { get; set; }

        /// <summary>
        /// Yakıt Kesilecek: Yakıt kesilecek ise işaretleyiniz.
        /// </summary>
        public int yakitKesilecek { get; set; }

        /// <summary>
        /// Kesinti Tipi: Kesin Tipini giriniz.
        /// </summary>
        public string kesintiTipi { get; set; }

        /// <summary>
        /// Kesinti Çevre Gideri: Çevre giderini giriniz.
        /// </summary>
        public int kesintiCevreGideri { get; set; }

        /// <summary>
        /// Kesinti Su Gideri: Su giderini giriniz.
        /// </summary>
        public int kesintiSuGideri { get; set; }

        /// <summary>
        /// Kesinti Diğer: Diğer kesintileri giriniz.
        /// </summary>
        public int kesintiDiger { get; set; }

        /// <summary>
        /// Kesinti Diğer Açıklama: Kesinti açıklması giriniz.
        /// </summary>
        public string kesintiDigerAciklama { get; set; }

        /// <summary>
        /// Kesinti Ortak Gider: Ortak giderleri giriniz.
        /// </summary>
        public int kesintiOrtakGider { get; set; }
    }

    /// <summary>
    /// LojmanSiteBlok: 
    /// </summary>
    [System.Serializable()]
    public class LojmanSiteBlok
    {
        /// <summary>
        /// KOD: Kodu giriniz.
        /// </summary>
        public int kod { get; set; }
        public int siteKod { get; set; }

        /// <summary>
        /// Site Adi: Site Adını giriniz.
        /// </summary>
        public string siteAdi { get; set; }

        /// <summary>
        /// blokAdi: blokAdi giriniz.
        /// </summary>
        public string blokAdi { get; set; }

        /// <summary>
        /// blokNo: blokNo giriniz.
        /// </summary>
        public string blokNo { get; set; }

        /// <summary>
        /// daireSayısı: daireSayısı giriniz.
        /// </summary>
        public int daireSayisi { get; set; }

        /// <summary>
        /// siteGideri: Yakıt kesilecek ise işaretleyiniz.
        /// </summary>
        public int siteGideri { get; set; }

        public int bosDaireAdeti { get; set; }

        public string blokNoBosDaireli { get { return blokNo + " (" + bosDaireAdeti + " adet boş daire içerir.)"; } }
    }

    /// <summary>
    /// LojmanSiteBlok: 
    /// </summary>
    [System.Serializable()]
    public class LojmanSiteBlokDaire
    {
        /// <summary>
        /// KOD: Kodu giriniz.
        /// </summary>
        public int kod { get; set; }
        public int siteKod { get; set; }

        /// <summary>
        /// Site Adi: Site Adını giriniz.
        /// </summary>
        public string siteAdi { get; set; }

        /// <summary>
        /// blokKod: blokKod giriniz.
        /// </summary>
        public int blokKod { get; set; }

        /// <summary>
        /// blokNo: blokNo giriniz.
        /// </summary>
        public string blokNo { get; set; }

        /// <summary>
        /// daireTuru: daireTuru giriniz.
        /// </summary>
        public int daireTuru { get; set; }
        public int daireTipi { get; set; }
        public int daireKatNo { get; set; }
        public string daireAciklama { get; set; }

        /// <summary>
        /// daireTuruAd: daireTuruAd giriniz.
        /// </summary>
        public string daireTuruAd { get; set; }

        /// <summary>
        /// daireNo: daireNo giriniz.
        /// </summary>
        public int daireNo { get; set; }

        /// <summary>
        /// metrekare: metrekare giriniz.
        /// </summary>
        public int metrekare { get; set; }

        /// <summary>
        /// elektrikNo: elektrikNo giriniz.
        /// </summary>
        public int elektrikNo { get; set; }

        /// <summary>
        /// suNo: suNo giriniz.
        /// </summary>
        public int suNo { get; set; }

        /// <summary>
        /// dogalgazNo: dogalgazNo giriniz.
        /// </summary>
        public int dogalgazNo { get; set; }

        /// <summary>
        /// cevtemNo: cevtemNo giriniz.
        /// </summary>
        public int cevtemNo { get; set; }

        /// <summary>
        /// dahiliTel: dahiliTel giriniz.
        /// </summary>
        public string dahiliTel { get; set; }

        public string daireGenel { get { return (doluMu == 1 ? "Dolu - " : (rezervMi == 1 ? "Rezervli - " : "Boş - ")) + daireNo + " - " + daireTuruAd + " - M2: " + metrekare; } }

        public string personelAd { get; set; }

        public int siraNo { get; set; }

        public int doluMu { get; set; }

        public int rezervMi { get; set; }
    }

    /// <summary>
    /// PersonelLojmani: 
    /// </summary>
    [System.Serializable()]
    public class PersonelLojmani
    {
        /// <summary>
        /// KOD: Kodu giriniz.
        /// </summary>
        public int kod { get; set; }
        public int daireKod { get; set; }
        public int personelKod { get; set; }
        public int lojmanTipi { get; set; }
        public TNSDateTime basTarihi { get; set; }
        public TNSDateTime bitTarihi { get; set; }
        public string adres { get; set; }
        public int kesintiVar { get; set; }
        public int tazminatiVar { get; set; }
        public int suParasiKesintisi { get; set; }
        public TNSDateTime konutYardimTarihi { get; set; }
        public TNSDateTime ozelTarih { get; set; }
        public int adresTipi { get; set; }
        public int tahakkuk { get; set; }
        public string tahsisTuru { get; set; }
        public string lojmanTipiEtiketi { get; set; }
        public string blokNo { get; set; }
        public string siteAdi { get; set; }
        public int daireNo { get; set; }
        public int blokKod { get; set; }
        public int siteKod { get; set; }
        public DateTime basTarihiDT { get { return basTarihi.Oku(); } }
        public DateTime bitTarihiDT { get { return bitTarihi.Oku(); } }
        public int siraNo { get; set; }
        public string personelAd { get; set; }
        public string personelMail { get; set; }
        public string personelTel { get; set; }
        public string daireTipiAd { get; set; }
        public string unvanAd { get; set; }
        public int yil { get; set; }
        public int ay { get; set; }
        public int yilAyDolu = 1;
        public string yilAy { get { return yil.ToString() + ay.ToString().PadLeft(2, '0'); } }
        public int oturmaGunu { get; set; }
        public ENUMTahsisTipi tashisTip { get; set; }
        public string tahsisTipAd { get { return TNS.IKY.Arac.TahsisTipAdBul(tashisTip); } }
        public TNSDateTime rezervTarih { get; set; }
        public SozlukGider kesintiler { get; set; }
        public bool giderleriOku { get; set; }
        public bool komisyonMu { get; set; }
        public TNSCollection pkesintiler { get; set; }
        public int daireMetreKare { get; set; }
        public int oturduguLojmanSayisi { get; set; }
        public int oncelikDerecesi { get; set; }
        public string aciklama { get; set; }

        public PersonelLojmani()
        {
            this.kod = 0;
            this.komisyonMu = false;
            this.sadeceOturanlar = false;
            this.yakitKesilecek = 0;
        }
        public bool sadeceOturanlar { get; set; }
        public int yakitKesilecek { get; set; }
        public int daireTuru { get; set; }

        public int metrekare { get; set; }

        public bool Bos
        {
            get
            {
                if (string.IsNullOrWhiteSpace(personelAd)) return true;
                if (this.bitTarihi != null && !this.bitTarihi.isNull) return true;
                return false;
            }
        }
    }

    /// <summary>
    /// LojmanTuru: 
    /// </summary>
    [System.Serializable()]
    public class LojmanTuru
    {
        /// <summary>
        /// KOD: Kodu giriniz.
        /// </summary>
        public int kod { get; set; }
        public string etiket { get; set; }
        public int toplamYil { get; set; }
    }

    /// <summary>
    /// SozlukGider: 
    /// </summary>
    [System.Serializable()]
    public class SozlukGider
    {
        /// <summary>
        /// KOD: Kodu giriniz.
        /// </summary>
        public int kod { get; set; }
        public string ad { get; set; }
        public double miktar { get; set; }
        public int otomatik { get; set; }
        public int aktif { get; set; }
        public double endex { get; set; }
        public double elden { get; set; }
        public string aciklama { get; set; }
        public TNSCollection kesintiler { get; set; }
        public int eldenGor { get; set; }
        public int endexGor { get; set; }
        public int aciklamaGor { get; set; }

        public SozlukGider()
        {
            this.kod = 0;
            this.ad = "";
            this.miktar = 0.0;
            this.otomatik = 0;
            this.aktif = 0;
            this.endex = 0.0;
            this.elden = 0.0;
        }
    }

    /// <summary>
    /// SiteGider: 
    /// </summary>
    [System.Serializable()]
    public class SiteGider
    {
        /// <summary>
        /// KOD: Kodu giriniz.
        /// </summary>
        public int kod { get; set; }
        public int siteKod { get; set; }
        public int yil { get; set; }
        public int ay { get; set; }
        public int giderKod { get; set; }
        public bool otomatik { get; set; }
        public double miktar { get; set; }
        public string aciklama { get; set; }
        public int eskiKod { get; set; }
        public SozlukGider[] giderler { get; set; }
        public string giderTurAd { get; set; }
        public string siteAdi { get; set; }
        public bool varMi { get; set; }
    }

    /// <summary>
    /// PersonelSiteKesinti: 
    /// </summary>
    [System.Serializable()]
    public class PersonelSiteKesinti
    {
        /// <summary>
        /// KOD: Kodu giriniz.
        /// </summary>
        public int kod { get; set; }
        public int siteKesTopKod { get; set; }
        public int pLojmanKod { get; set; }
        public int yil { get; set; }
        public int ay { get; set; }
        public int giderKod { get; set; }
        public int aktif { get; set; }
        public double tutar { get; set; }
        public double endex { get; set; }
        public double elden { get; set; }
        public string aciklama { get; set; }
        public int siraNo { get; set; }
        public int personelKod { get; set; }
        public string personelAd { get; set; }
        public string personelMail { get; set; }
        public string unvanAd { get; set; }
        public string giderTurAd { get; set; }
        public string TC { get; set; }
        public string siteAdi { get; set; }
        public int siteKod { get; set; }
        public int daireNo { get; set; }
        public string blokNo { get; set; }
        public bool varMi { get; set; }
        public double toplamTutar { get; set; }
        //public int eldenGor { get; set; }
        //public int endexGor { get; set; }
        //public int aciklamaGor { get; set; }
    }

    /// <summary>
    /// PersonelSiteKesinti: 
    /// </summary>
    [System.Serializable()]
    public class PersonelSiteKesintiToplam
    {
        /// <summary>
        /// KOD: Kodu giriniz.
        /// </summary>
        public int kod { get; set; }
        public int pLojmanKod { get; set; }
        public int yil { get; set; }
        public int ay { get; set; }
        public PersonelSiteKesinti[] kesintiler { get; set; }
        public int siraNo { get; set; }
        public string personelAd { get; set; }
        public string personelMail { get; set; }
        public string unvanAd { get; set; }
        public string giderTurAd { get; set; }
        public string TC { get; set; }
        public string siteAdi { get; set; }
        public int daireNo { get; set; }
        public string blokNo { get; set; }
        public bool varMi { get; set; }
        public double toplamTutar { get; set; }
    }

    [System.Serializable()]
    public class VarMi
    {
        public Sonuc s { get; set; }
        public bool varMi { get; set; }
    }

    /// <summary>
    /// LojmanKomisyon: 
    /// </summary>
    [System.Serializable()]
    public class Komisyon
    {
        /// <summary>
        /// KOD: Kodu giriniz.
        /// </summary>
        public int kod { get; set; }

        /// <summary>
        /// kararTarihi: kararTarihi giriniz.
        /// </summary>
        public TNSDateTime kararTarihi { get; set; }
        public DateTime kararTarihiDT { get { return kararTarihi.Oku(); } }

        /// <summary>
        /// kararSayisi: kararSayisi giriniz.
        /// </summary>
        public string kararSayisi { get; set; }

        /// <summary>
        /// toplantiTarihi: toplantiTarihi giriniz.
        /// </summary>
        public TNSDateTime toplantiTarihi { get; set; }

        /// <summary>
        /// YONERGE: YONERGE giriniz.
        /// </summary>
        public string yonerge { get; set; }

        public IKYImza[] imzalar { get; set; }
    }

    /// <summary>
    /// LojmanKomisyon: 
    /// </summary>
    [System.Serializable()]
    public class IKYImza
    {

        /// <summary>
        /// KOD: Kodu giriniz.
        /// </summary>
        public int kod { get; set; }

        /// <summary>
        /// komisyonKod: komisyonKod giriniz.
        /// </summary>
        public int komisyonKod { get; set; }

        /// <summary>
        /// Tip: Tip giriniz.
        /// </summary>
        public ENUMKomisyonTip tip { get; set; }
        public int tipInt { get { return (int)tip; } }

        /// <summary>
        /// unvan: unvan giriniz.
        /// </summary>
        public string unvan { get; set; }

        /// <summary>
        /// isim: isim giriniz.
        /// </summary>
        public string isim { get; set; }

        /// <summary>
        /// imzaTur: imzaTur giriniz.
        /// </summary>
        public ENUMImzaTur imzaTur { get; set; }
        public int imzaTurInt { get { return (int)imzaTur; } }
        public string imzaTurStr
        {
            get
            {
                return Arac.ImzaTurAdBul(imzaTur);
            }
        }

        /// <summary>
        /// siraNo: siraNo giriniz.
        /// </summary>
        public int siraNo { get; set; }
    }

    [System.Serializable()]
    public class IKYSiteYetki
    {
        public int kod { get; set; }

        public int pkod { get; set; }

        public int yetkiliOlduguSiteKod { get; set; }

        public int aktif { get; set; }

        public string aktifText { get { return aktif == 1 ? "Aktif" : "Pasif"; } }

        public int tip { get; set; }

        public string tipAd { get; set; }

        public string yetkiliOlduguSiteAd { get; set; }

        public string personelAd { get; set; }

        public string oturduguSite { get; set; }

        public string oturduguBlok { get; set; }

        public int oturduguDaire { get; set; }

        public int oturduguPLKod { get; set; }
    }


    [System.Serializable()]
    public class IKYPersonelGider
    {
        public enum ENUMPersonelHarcamaTur
        {
            Yemek = 1,
            Tanimsiz = 0
        }
        public int pKod { get; set; }

        public int yerleskeKod { get; set; }

        public ENUMPersonelHarcamaTur harcamaTur { get; set; }
        public string harcamaTurAdi { get { return HarcamaTurAd(harcamaTur); } }
        public int kriterYil;
        public int kriterAy;

        public TNSDateTime tarih = new TNSDateTime();
        public int durum;
        public string kullaniciKodu;
        public TNSDateTime degisiklikZamani;
        public string tckimlikNo;
        public string ad;
        public string soyad;
        public int saymanlikKod;
        public string birimKod;
        public string kriterOrgKopruKod;

        public IKYPersonelGider(IKYPersonelGider pg)
        {
            this.pKod = pg.pKod;
            this.tarih = new TNSDateTime(pg.tarih);
            this.yerleskeKod = pg.yerleskeKod;
            this.harcamaTur = pg.harcamaTur;
            this.tutar = pg.tutar;
        }

        public IKYPersonelGider()
        {
        }

        public DateTime tarihDT { get { return tarih.Oku(); } set { tarih.Yaz(value); } }

        public int mailGonderildi { get; set; }

        public string yerleskeAdi { get; set; }
        public double tutar { get; set; }

        public static string HarcamaTurAd(ENUMPersonelHarcamaTur yemek)
        {
            switch (yemek)
            {
                case ENUMPersonelHarcamaTur.Yemek: return "Yemek";
            }
            return "";
        }

        public static TNSCollection HarcamaTurListe(bool tanimsizEkle)
        {
            TNSCollection tc = new TNSCollection();
            if (tanimsizEkle)
                tc.Add(new IntKodStrAd((int)IKYPersonelGider.ENUMPersonelHarcamaTur.Tanimsiz, IKYPersonelGider.HarcamaTurAd(IKYPersonelGider.ENUMPersonelHarcamaTur.Tanimsiz)));
            tc.Add(new IntKodStrAd((int)IKYPersonelGider.ENUMPersonelHarcamaTur.Yemek, IKYPersonelGider.HarcamaTurAd(IKYPersonelGider.ENUMPersonelHarcamaTur.Yemek)));
            return tc;
        }
    }

    [System.Serializable()]
    public class IKYYerleske
    {
        public int kod { get; set; }

        public string ad { get; set; }

        public int yemekhaneVarmi { get; set; }
    }

    /// <summary>
    /// Gorevler: Görevler
    /// </summary>
    [System.Serializable()]
    public class Gorevler
    {
        public enum ENUMGorevDurumu
        {
            ALTIAY = 1,
            ASIL = 2,
            TEDVIREN = 3,
            UCRETLIVEKALET = 4,
            UCRETSIZVEKALET = 5
        }

        /// <summary>
        /// PKod: Kod
        /// </summary>
        public int pKod { get; set; }

        /// <summary>
        /// gorevNo: Görev Sıra No
        /// </summary>
        public int gorevNo { get; set; }

        /// <summary>
        /// unvan Kod: Görev
        /// </summary>
        public int unvanKod { get; set; }

        /// <summary>
        /// Görev Durumu: Görev Durumu
        /// </summary>
        public ENUMGorevDurumu gorevDurumu { get; set; }

        /// <summary>
        /// Birimi: Kadro Birimi
        /// </summary>
        public int birimi { get; set; }

        /// <summary>
        /// Başlama Tarihi: Başlama Tarihi
        /// </summary>
        public TNSDateTime baslamaTarihi { get; set; }
        public DateTime baslamaTarihiDT
        {
            get
            {
                if (baslamaTarihi == null)
                    return DateTime.MinValue;
                else
                    return baslamaTarihi.Oku();
            }
            set
            {
                if (baslamaTarihi == null) baslamaTarihi = new TNSDateTime(value);
                else baslamaTarihi.Yaz(value);
            }
        }

        /// <summary>
        /// Bitiş Tarihi: Bitiş Tarihi
        /// </summary>
        public TNSDateTime bitisTarihi { get; set; }
        public DateTime bitisTarihiDT
        {
            get
            {
                if (bitisTarihi == null)
                    return DateTime.MinValue;
                else
                    return bitisTarihi.Oku();
            }
            set
            {
                if (bitisTarihi == null) bitisTarihi = new TNSDateTime(value);
                else bitisTarihi.Yaz(value);
            }
        }

        public DateTime bitisTarihiDTEkle1
        {
            get
            {
                return bitisTarihiDT.AddDays(1);
            }
        }

        public static string GorevDurumAd(ENUMGorevDurumu durum)
        {
            switch (durum)
            {
                case ENUMGorevDurumu.ALTIAY:
                    return "6 Ay Deneme Süreli";
                case ENUMGorevDurumu.ASIL:
                    return "Asil";
                case ENUMGorevDurumu.TEDVIREN:
                    return "Tedviren";
                case ENUMGorevDurumu.UCRETLIVEKALET:
                    return "Ücretli Vekalet";
                case ENUMGorevDurumu.UCRETSIZVEKALET:
                    return "Ücretsiz Vekalet";
                default:
                    return "";
            }
        }

        public string unvanAd { get; set; }
        public string birimiAd { get; set; }
        public int unvanSira { get; set; }
        public string gorevDurumuAd
        {
            get
            {
                return GorevDurumAd(this.gorevDurumu);
            }
        }

        public bool UcretliMi()
        {
            return (this.gorevDurumu == ENUMGorevDurumu.ALTIAY || this.gorevDurumu == ENUMGorevDurumu.ASIL || this.gorevDurumu == ENUMGorevDurumu.UCRETLIVEKALET);
        }

        public string aciklama { get; set; }

        public string onayNo { get; set; }

        public TNSDateTime onayTarihi { get; set; }
        public DateTime onayTarihiDT
        {
            get
            {
                if (onayTarihi == null)
                    return DateTime.MinValue;
                else
                    return onayTarihi.Oku();
            }
            set
            {
                if (onayTarihi == null) onayTarihi = new TNSDateTime(value);
                else onayTarihi.Yaz(value);
            }
        }
    }

    [Serializable]
    public class OnaysizKadroBilgi
    {
        public TNSCollection onaysizlar = null;
        public int onaysizSay = 0;
        public TNSCollection tayinBekleyenler = null;
        public int tayinBekleyenSay = 0;
    }

    [Serializable()]
    public class PersonelSozlesme
    {
        public int kod { get; set; }
        public int yil { get; set; }
        public string tcKimlikNo { get; set; }
        public string adSoyad { get; set; }
        public TNSDateTime basTarih = new TNSDateTime();
        public TNSDateTime bitTarih = new TNSDateTime();
        public double tutar { get; set; }
        public int sozlesmeKod { get; set; }

        public DateTime basTarihDT { get { return basTarih.Oku(); } }
        public DateTime bitTarihDT { get { return bitTarih.Oku(); } }
    }

    /// <summary>
    /// Personel Mal Bildirimi Sınıfı
    /// </summary>
    [Serializable()]
    public class PersonelMalBildirimi
    {
        public int kod { get; set; } = 0;

        public int personelKod { get; set; } = 0;

        /// <summary>
        /// Personelin T.C. Kimlik numarası
        /// </summary>
        public string tcKimlikNo { get; set; } = "";

        /// <summary>
        /// Sıra numarası
        /// </summary>
        public int sirano { get; set; } = 0;

        /// <summary>
        /// Mal bildirim tarihi
        /// </summary>
        public TNSDateTime tarih { get; set; } = new TNSDateTime();
        public DateTime tarihDT { get { return tarih.Oku(); } set { tarih.Yaz(value); } }

        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama { get; set; } = "";

        public TNSCollection kimlikListesi { get; set; } = new TNSCollection();
        public TNSCollection tasinmazListesi { get; set; } = new TNSCollection();
        public TNSCollection kooperatifListesi { get; set; } = new TNSCollection();
        public TNSCollection tasitListesi { get; set; } = new TNSCollection();
        public TNSCollection digerTasinirListesi { get; set; } = new TNSCollection();
        public TNSCollection bankaMenkulListesi { get; set; } = new TNSCollection();
        public TNSCollection altinMucevherListesi { get; set; } = new TNSCollection();
        public TNSCollection borcAlacakListesi { get; set; } = new TNSCollection();
        public TNSCollection digerServetListesi { get; set; } = new TNSCollection();

        public List<MalBilKimlik> kimlikListesiTanimli { set { kimlikListesi.AddRange(value); } }
        public List<MalBilTasinmaz> tasinmazListesiTanimli { set { tasinmazListesi.AddRange(value); } }
        public List<MalBilKooperatif> kooperatifListesiTanimli { set { kooperatifListesi.AddRange(value); } }
        public List<MalBilTasit> tasitListesiTanimli { set { tasitListesi.AddRange(value); } }
        public List<MalBilDigerTasinir> digerTasinirListesiTanimli { set { digerTasinirListesi.AddRange(value); } }
        public List<MalBilBankaMenkul> bankaMenkulListesiTanimli { set { bankaMenkulListesi.AddRange(value); } }
        public List<MalBilAltinMucevher> altinMucevherListesiTanimli { set { altinMucevherListesi.AddRange(value); } }
        public List<MalBilBorcAlacak> borcAlacakListesiTanimli { set { borcAlacakListesi.AddRange(value); } }
        public List<MalBilDigerServet> digerServetListesiTanimli { set { digerServetListesi.AddRange(value); } }
    }

    [Serializable()]
    public class MalBilKimlik
    {
        public int siraNo { get; set; }
        public string adiSoyadi { get; set; }
        public TNSDateTime dogumTarihi { get; set; } = new TNSDateTime();
        public DateTime dogumTarihiDT { get { return dogumTarihi.Oku(); } set { dogumTarihi.Yaz(value); } }
        public string dogumYeri { get; set; }
        public string yakinligi { get; set; }
        public string tcKimlikNo { get; set; }
    }

    [Serializable()]
    public class MalBilTasinmaz
    {
        public int siraNo { get; set; }
        public double degeri { get; set; }
        public string tasinmazCinsi { get; set; }
        public string adres { get; set; }
        public double hisseMiktari { get; set; }
        public TNSDateTime edinmeTarihi { get; set; } = new TNSDateTime();
        public DateTime edinmeTarihiDT { get { return edinmeTarihi.Oku(); } set { edinmeTarihi.Yaz(value); } }
        public string malikTCKN { get; set; }
    }

    [Serializable()]
    public class MalBilKooperatif
    {
        public int siraNo { get; set; }
        public string kooperatifAdiYeri { get; set; }
        public double hisseDegeri { get; set; }
        public TNSDateTime uyelikTarihi { get; set; } = new TNSDateTime();
        public DateTime uyelikTarihiDT { get { return uyelikTarihi.Oku(); } set { uyelikTarihi.Yaz(value); } }
        public string tcKimlikNo { get; set; }
    }

    [Serializable()]
    public class MalBilTasit
    {
        public int siraNo { get; set; }
        public string plakaNo { get; set; }
        public string tasitinCinsi { get; set; }
        public string tasitinMarkasi { get; set; }
        public double edinmeDegeri { get; set; }
        public int modelYili { get; set; }
        public TNSDateTime edinmeTarihi { get; set; } = new TNSDateTime();
        public DateTime edinmeTarihiDT { get { return edinmeTarihi.Oku(); } set { edinmeTarihi.Yaz(value); } }
        public string tcKimlikNo { get; set; }
    }

    [Serializable()]
    public class MalBilDigerTasinir
    {
        public int siraNo { get; set; }
        public string tasinirMalCinsi { get; set; }
        public double edinmeDegeri { get; set; }
        public int modelYili { get; set; }
        public TNSDateTime edinmeTarihi { get; set; } = new TNSDateTime();
        public DateTime edinmeTarihiDT { get { return edinmeTarihi.Oku(); } set { edinmeTarihi.Yaz(value); } }
        public string tcKimlikNo { get; set; }
    }

    [Serializable()]
    public class MalBilBankaMenkul
    {
        public int siraNo { get; set; }
        public string nitelik { get; set; }
        public double miktari { get; set; }
        public string cinsi { get; set; }
        public string tcKimlikNo { get; set; }
    }

    [Serializable()]
    public class MalBilAltinMucevher
    {
        public int siraNo { get; set; }
        public string cinsi { get; set; }
        public double degeri { get; set; }
        public double miktari { get; set; }
        public string tcKimlikNo { get; set; }
    }

    [Serializable()]
    public class MalBilBorcAlacak
    {
        public int siraNo { get; set; }
        public string borcluAdi { get; set; }
        public string alacakliAdi { get; set; }
        public double tutar { get; set; }
    }

    [Serializable()]
    public class MalBilDigerServet
    {
        public int siraNo { get; set; }
        public string servetUnsuru { get; set; }
        public string edinmeSekli { get; set; }
        public string tcKimlikNo { get; set; }
    }
}