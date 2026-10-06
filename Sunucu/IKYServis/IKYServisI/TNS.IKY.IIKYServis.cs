using System;
using TNS.KYM;
using TNS.DEG;

namespace TNS.IKY
{

    /// <summary>
    /// SendikaBilgi: 
    /// </summary>
    [System.Serializable()]
    public class SendikaBilgi
    {
        /// <summary>
        /// Sendika No: Sendika no giriniz.
        /// </summary>
        public int sendikaNo { get; set; } = 0;

        /// <summary>
        /// Sendika Ad: Sendika adını giriniz.
        /// </summary>
        public string sendikaAd { get; set; }

        /// <summary>
        /// Adres: Sendika adresini giriniz.
        /// </summary>
        public string adres { get; set; }

        /// <summary>
        /// IBAN: Sendika IBANı giriniz.
        /// </summary>
        public string iBAN { get; set; }

        /// <summary>
        /// HesapNo: Sendika hesap no giriniz
        /// </summary>
        public string hesapNo { get; set; }

        /// <summary>
        /// HesapNo: Sendika hesap no giriniz
        /// </summary>
        public int topluGorusmePuani { get; set; }

        /// <summary>
        /// SendikaPersonel: 
        /// </summary>
        public TNSCollection sendikaPersonel;

        /// <summary>
        /// HesapNo: Sendika hesap no giriniz
        /// </summary>
        public double kesintiOrani { get; set; } = 0;

    }

    /// <summary>
    /// SendikaBilgi: 
    /// </summary>
    [System.Serializable()]
    public class PersonelSendika
    {
        public int kod { get; set; }
        public string tcKimlikNo { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public string sicilNo { get; set; }
        public double kesintiOrani { get; set; }
    }

    /// <summary>
    /// PersonelTipi: Personel Tipi
    /// </summary>
    [System.Serializable()]
    public class PersonelTipi
    {

        public PersonelTipi()
        {
            kod = 0;
            ad = "";
        }
        /// <summary>
        /// Kod: Kod
        /// </summary>
        public int kod { get; set; }

        /// <summary>
        /// Ad: Ad
        /// </summary>
        public string ad { get; set; }
        public string kodAd { get { return kod + "-" + ad; } }

        /// <summary>
        /// Kurum Dışı: Kurum dışı mı?
        /// </summary>
        public int kurumDisi { get; set; }

        /// <summary>
        /// Sıra: Öncelik sırası
        /// </summary>
        public int sira { get; set; }

        /// <summary>
        /// Kadrolu: Kadrolu bir personel tipi mi?
        /// </summary>
        public int kadrolu { get; set; }

        /// <summary>
        /// Goster: Listelerde gösterilsin mi?
        /// </summary>
        public int goster { get; set; }

        /// <summary>
        /// Grup Ad: Grup Adı
        /// </summary>
        public string grupAd { get; set; }

        /// <summary>
        /// Maaş Grup Ad: Maaş Grup Adı
        /// </summary>
        public string maasGrupAd { get; set; }

        /// <summary>
        /// Aktif: Aktif mi?
        /// </summary>
        public int aktif { get; set; }

        /// <summary>
        /// Terfiye Dahil: Terfiye Dahil mi?
        /// </summary>
        public int terfiyeDahil { get; set; }

        /// <summary>
        /// Maaş Dahil: Maaşa Dahil mi?
        /// </summary>
        public int maasaDahil { get; set; }

        /// <summary>
        /// Hitap Mı: HITAP'a gönderilecek mi?
        /// </summary>
        public int hitapMi { get; set; }

        public int pesinMi { get; set; }

        //Bizim kurumda ise kadrolu bir tür ise kadro işlemi yapılır.
        public bool KadroIslemiYapilacakMi()
        {
            return this.BizimKurumMu() && this.KadroluMu();
        }

        public bool KadroluMu()
        {
            if (kadrolu == 1) return true;
            return false;
        }

        public bool BizimKurumMu()
        {
            if (kurumDisi == 0) return true;
            return false;
        }

        public string[] tipGruplar = null;

        public int H1 { get; set; }
        public int H2 { get; set; }
        public int H3 { get; set; }
        public int H4 { get; set; }
        public int H5 { get; set; }
        public int H6 { get; set; }
        public int H7 { get; set; }
        public TNS.IKY.ENUMHizmetTablolari HITAP { get; set; }
        public int maasDonemGun { get; set; }
        public int maasOdemeGun { get; set; }

        public bool sadeceKullanilanlar = false;
    }

    /// <summary>
    /// Personelle ilgili görev tanımı, bilgi birikimi, beceri gibi bilgileri tutan sınıf
    /// </summary>
    [Serializable()]
    public class PersonelBilgiBirikimi
    {
        /// <summary>
        /// Personelin T.C. kimlik numarası
        /// </summary>
        public string tcKimlikNo;

        /// <summary>
        /// Personelin görev tanımı
        /// </summary>
        public string gorev;

        /// <summary>
        /// Personelin bilgi birikimi
        /// </summary>
        public string bilgi;

        /// <summary>
        /// Personelin sahip olduğu beceriler
        /// </summary>
        public string beceri;

        public string ad;
        public string soyad;
        public string resim;

        /// <summary>
        /// Parametresiz Personel nesnesi oluşturucu yordam
        /// </summary>
        public PersonelBilgiBirikimi()
        {
            tcKimlikNo = string.Empty;
            gorev = string.Empty;
            bilgi = string.Empty;
            beceri = string.Empty;
        }
    }


    public static class IzinArac
    {
        public enum IzinYontem : int
        {
            TC = 1,
            ManasSozlesmeli = 2,
            Kibris = 3,
            Tanimsiz = 99
        }

        public static int IzinHesapla(int izinTurKod, DateTime baslangicTarihi, DateTime bitisTarihi)
        {
            TimeSpan tarih = bitisTarihi - baslangicTarihi;
            //if (izinTurKod == 1 && izinTur == IzinYontem.TCIsci)
            //{
            //    int tatilSay = 0;  // Tatil gunu miktari

            //    // İki tarih arasındaki Pazar günü sayılarının bulunması
            //    for (DateTime dt = baslangicTarihi; dt <= bitisTarihi; dt = dt.AddDays(1.0))
            //    {
            //        if (dt.DayOfWeek == DayOfWeek.Sunday)  // Bu tarih Pazar gününe mi denk geliyor?
            //            tatilSay++;// Evet, tatil gunu sayisini bir arttiralim

            //        // Uzerinde olunan gun resmi tatil mi ?
            //        if (TNS.ResmiTatil.ResmiTatilMi(dt, 1))
            //            tatilSay++;// Evet, resmi tatil, tatil gunu miktarini 1 arttiralim
            //    }

            //    // İzin baslangic ve bitimi gunlerinden tatil gunu sayisini cikartalim.
            //    return tarih.Days - tatilSay;// +1;
            //}
            //else
            //{
            return tarih.Days;// +1;
            //}
        }

        private static IzinYontem _izinYontem = IzinYontem.Tanimsiz;
        public static IzinYontem izinYontem
        {
            get
            {
                if (_izinYontem == IzinYontem.Tanimsiz)
                {
                    _izinYontem = (IzinYontem)OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "IzinYontem"), 1);
                }
                return _izinYontem;
            }
        }

        public static string DonemAdBul(int yil)
        {
            if (izinYontem == IzinYontem.ManasSozlesmeli)
                return yil.ToString() + "-" + (yil + 1).ToString() + " Dönemi";
            else
                return yil.ToString() + " Yılı";
        }

        public static int DonemYiliBul(int yil, int ay)
        {
            if (izinYontem == IzinYontem.ManasSozlesmeli && ay < AyBul()) return yil - 1;
            else return yil;
        }

        public static DateTime DonemBasiBul(int yil)
        {
            try
            {
                return new DateTime(yil, AyBul(), 1);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime DonemSonuBul(int yil)
        {
            try
            {
                return new DateTime(yil + 1, AyBul(), 1);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static int AyBul()
        {
            if (TNS.IKY.IzinArac.izinYontem == IzinArac.IzinYontem.ManasSozlesmeli)
                return 9;
            return 1;
        }

        public static int YilIciIzinHakki(int izinYili)
        {
            if (izinYontem == IzinYontem.ManasSozlesmeli) return 30;
            if (izinYili >= 10)
                return 30;
            else if (izinYili >= 1)
                return 20;
            else
                return 0;
        }
    }

    [Serializable()]
    public static class Arac
    {
        private static int? hizmetBulmaYontemi = null;
        public static int HizmetBulmaYontemiBul()
        {
            if (hizmetBulmaYontemi == null)
            {
                int tut = OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "HizmetBulmaYontemi"), 0);
                if (tut <= 0) hizmetBulmaYontemi = 2;
                else hizmetBulmaYontemi = tut;
            }
            return hizmetBulmaYontemi.Value;
        }

        public static ObjectArray izinTipListesi = null;

        static int[,] Gostergeler = {
                                {1320, 1380, 1440, 1500, 0, 0, 0, 0, 0},
                                {1155, 1210, 1265, 1320, 1380, 1440, 0, 0, 0},
                                {1020, 1065, 1110, 1155, 1210, 1265, 1320, 1380, 0},
                                {915, 950, 985, 1020, 1065, 1110, 1155, 1210, 1265},
                                {835, 865, 895, 915, 950, 985, 1020, 1065, 1110},
                                {760, 785, 810, 835, 865, 895, 915, 950, 985},
                                {705, 720, 740, 760, 785, 810, 835, 865, 895},
                                {660, 675, 690, 705, 720, 740, 760, 785, 810},
                                {620, 630, 645, 660, 675, 690, 705, 720, 740},
                                {590, 600, 610, 620, 630, 645, 660, 675, 690},
                                {560, 570, 580, 590, 600, 610, 620, 630, 645},
                                {545, 550, 555, 560, 570, 580, 590, 600, 610},
                                {530, 535, 540, 545, 550, 555, 560, 570, 580},
                                {515, 520, 525, 530, 535, 540, 545, 550, 555},
                                {500, 505, 510, 515, 520, 525, 530, 535, 540},
                             };

        public static int GostergeBul(int derece, int kademe)
        {
            if (derece < 1 || derece > 15 || kademe < 1 || kademe > 9) return -1;
            return Gostergeler[derece - 1, kademe - 1];
        }

        private static IIKYServis servisIKY = null;

        public static IIKYServis Tanimla()
        {
            if (servisIKY != null) return servisIKY;
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("IKYServisURL");
            servisIKY = TNS.Api.ServisIstemci.Olustur<IIKYServis>(sUrl);
            return servisIKY;
        }

        public static void PersonelTipSifirla()
        {
            _personelTipListe = null;
            _personelTipGrupListe = null;
        }

        public static ObjectArray _personelTipListe = null;
        public static TNSCollection PersonelTipListe
        {
            get
            {
                if (_personelTipListe == null || !_personelTipListe.sonuc.islemSonuc)
                {
                    _personelTipListe = Tanimla().ListelePersonelTip(null, new PersonelTipi { sadeceKullanilanlar = true });
                    //_personelTipListe.objeler.Insert(0, new PersonelTipi());
                }
                return _personelTipListe.objeler;
            }
        }

        public static ObjectArray _personelTipGrupListe = null;
        public static TNSCollection PersonelTipGrupListe
        {
            get
            {
                if (_personelTipGrupListe == null || !_personelTipGrupListe.sonuc.islemSonuc)
                {
                    _personelTipGrupListe = Tanimla().ListelePersonelTipGrup(null, true);
                    //_personelTipGrupListe.objeler.Insert(0, new PersonelTipi());
                }
                return _personelTipGrupListe.objeler;
            }
        }

        public static PersonelTipi PersonelTipBul(int personelTipKod)
        {
            foreach (PersonelTipi item in PersonelTipListe)
            {
                if (item.kod == personelTipKod)
                    return item;
            }
            return null;
        }

        public static string KayitDurumuAdOl(ENUMKadroDurumu kod)
        {
            if (kod == ENUMKadroDurumu.YENI) return "Yeni";
            if (kod == ENUMKadroDurumu.AKTIFKAYIT) return "Aktif";
            if (kod == ENUMKadroDurumu.AKTIFKAPANIYOR) return "Ayrılış Sürecinde";
            if (kod == ENUMKadroDurumu.AKTIFKAPANDI) return "Aktif (Kapanmış)";
            if (kod == ENUMKadroDurumu.AKTIFUCRETSIZIZIN) return "Ücretsiz İzin";
            if (kod == ENUMKadroDurumu.AKTIFYENI) return "Yeni";
            if (kod == ENUMKadroDurumu.AYRILDI) return "Bitmiş hizmet";
            if (kod == ENUMKadroDurumu.AYRILDIHESAPLAMA) return "Bitmiş hizmet";
            if (kod == ENUMKadroDurumu.REDDEDILDI) return "İptal Edildi";
            if (kod == ENUMKadroDurumu.TANIMSIZ) return "Tanımsız";
            return "";
        }

        public static string YakinlikAd(ENUMYakinlikDerecesi yakinlik)
        {
            switch (yakinlik)
            {
                case ENUMYakinlikDerecesi.Esi: return "Eşi";
                case ENUMYakinlikDerecesi.Kizi: return "Kızı";
                case ENUMYakinlikDerecesi.Oglu: return "Oğlu";
                case ENUMYakinlikDerecesi.Annesi: return "Annesi";
                case ENUMYakinlikDerecesi.Babasi: return "Babası";
                case ENUMYakinlikDerecesi.Kardesi: return "Kardeşi";
                case ENUMYakinlikDerecesi.Diger: return "Diğer";
                case ENUMYakinlikDerecesi.Kendisi: return "Kendisi";
                default: return "";
            }
        }

        private static System.Collections.ArrayList _yakinlikListe = null;
        public static System.Collections.ArrayList YakinlikListe
        {
            get
            {
                if (_yakinlikListe == null)
                {
                    _yakinlikListe = new System.Collections.ArrayList();
                    _yakinlikListe.Add(YakinlikSozlukOl(ENUMYakinlikDerecesi.Esi));
                    _yakinlikListe.Add(YakinlikSozlukOl(ENUMYakinlikDerecesi.Kizi));
                    _yakinlikListe.Add(YakinlikSozlukOl(ENUMYakinlikDerecesi.Oglu));
                    _yakinlikListe.Add(YakinlikSozlukOl(ENUMYakinlikDerecesi.Annesi));
                    _yakinlikListe.Add(YakinlikSozlukOl(ENUMYakinlikDerecesi.Babasi));
                    _yakinlikListe.Add(YakinlikSozlukOl(ENUMYakinlikDerecesi.Kardesi));
                    _yakinlikListe.Add(YakinlikSozlukOl(ENUMYakinlikDerecesi.Diger));
                    _yakinlikListe.Add(YakinlikSozlukOl(ENUMYakinlikDerecesi.Kendisi));
                }
                return _yakinlikListe;
            }
        }

        private static Sozluk YakinlikSozlukOl(ENUMYakinlikDerecesi yakinlik)
        {
            Sozluk s = new Sozluk();
            s.kod = (int)yakinlik;
            s.ad = TNS.IKY.Arac.YakinlikAd(yakinlik);
            return s;
        }

        public static string CinsiyetAdBul(ENUMCINSIYET cinsiyet)
        {
            switch (cinsiyet)
            {
                case ENUMCINSIYET.ERKEK: return "Erkek";
                case ENUMCINSIYET.KADIN: return "Kadın";
                case ENUMCINSIYET.TANIMSIZ:
                default:
                    return "";
            }
        }

        private static System.Collections.ArrayList _cinsiyetListe = null;
        public static System.Collections.ArrayList CinsiyetListe
        {
            get
            {
                if (_cinsiyetListe == null)
                {
                    _cinsiyetListe = new System.Collections.ArrayList();
                    _cinsiyetListe.Add(new Sozluk { kod = (int)ENUMCINSIYET.KADIN, ad = CinsiyetAdBul(ENUMCINSIYET.KADIN) });
                    _cinsiyetListe.Add(new Sozluk { kod = (int)ENUMCINSIYET.ERKEK, ad = CinsiyetAdBul(ENUMCINSIYET.ERKEK) });
                    //_cinsiyetListe.Add(CinsiyetAdBul(ENUMCINSIYET.TANIMSIZ));
                }
                return _cinsiyetListe;
            }
        }

        public static string KomisyonTipAdBul(ENUMKomisyonTip tip)
        {
            switch (tip)
            {
                case ENUMKomisyonTip.Paraf: return "Paraf";
                case ENUMKomisyonTip.Olur: return "Olur";
                case ENUMKomisyonTip.Imza: return "İmza";
                default:
                    return "";
            }
        }

        private static System.Collections.ArrayList _komisyonTipListe = null;
        public static System.Collections.ArrayList KomisyonTipListe
        {
            get
            {
                if (_komisyonTipListe == null)
                {
                    _komisyonTipListe = new System.Collections.ArrayList();
                    _komisyonTipListe.Add(new IntKodStrAd((int)ENUMKomisyonTip.Imza, KomisyonTipAdBul(ENUMKomisyonTip.Imza)));
                    _komisyonTipListe.Add(new IntKodStrAd((int)ENUMKomisyonTip.Olur, KomisyonTipAdBul(ENUMKomisyonTip.Olur)));
                    _komisyonTipListe.Add(new IntKodStrAd((int)ENUMKomisyonTip.Paraf, KomisyonTipAdBul(ENUMKomisyonTip.Paraf)));
                }
                return _komisyonTipListe;
            }
        }

        public static bool DonemAcikMi(int yil, int ay)
        {
            int sYil = System.DateTime.Now.Year;
            int sAy = System.DateTime.Now.Month;
            if (yil == sYil && ay == sAy) return true;
            AyEkleCikart(ref sYil, ref sAy, -1);
            if (yil == sYil && ay == sAy) return true;
            AyEkleCikart(ref sYil, ref sAy, 2);
            if (yil == sYil && ay == sAy) return true;

            return false;
        }

        public static void AyEkleCikart(ref int sYil, ref int sAy, int farkAy)
        {
            sAy = sAy + farkAy;

            while (sAy < 1 || sAy > 12)
            {
                if (sAy < 1)
                {
                    sYil--;
                    sAy += 12;
                }
                else if (sAy > 12)
                {
                    sYil++;
                    sAy -= 12;
                }
            }
        }


        public static string ImzaTurAdBul(ENUMImzaTur tip)
        {
            switch (tip)
            {
                case ENUMImzaTur.Uye: return "Üye";
                case ENUMImzaTur.Baskan: return "Başkan";
                default:
                    return "";
            }
        }

        private static System.Collections.ArrayList _imzaTurListe = null;
        public static System.Collections.ArrayList ImzaTurListe
        {
            get
            {
                if (_imzaTurListe == null)
                {
                    _imzaTurListe = new System.Collections.ArrayList();
                    _imzaTurListe.Add(new IntKodStrAd((int)ENUMImzaTur.Uye, ImzaTurAdBul(ENUMImzaTur.Uye)));
                    _imzaTurListe.Add(new IntKodStrAd((int)ENUMImzaTur.Baskan, ImzaTurAdBul(ENUMImzaTur.Baskan)));
                }
                return _imzaTurListe;
            }
        }

        public static string TahsisTipAdBul(ENUMTahsisTipi tip)
        {
            switch (tip)
            {
                case ENUMTahsisTipi.Rezervasyon: return "Rezervasyon";
                case ENUMTahsisTipi.Tahsis: return "Tahsis";
                default:
                    return "";
            }
        }

        private static System.Collections.ArrayList _tahsisTipListe = null;
        public static System.Collections.ArrayList TahsisTipListe
        {
            get
            {
                if (_tahsisTipListe == null)
                {
                    _tahsisTipListe = new System.Collections.ArrayList();
                    _tahsisTipListe.Add(new IntKodStrAd((int)ENUMTahsisTipi.Tahsis, TahsisTipAdBul(ENUMTahsisTipi.Tahsis)));
                    _tahsisTipListe.Add(new IntKodStrAd((int)ENUMTahsisTipi.Rezervasyon, TahsisTipAdBul(ENUMTahsisTipi.Rezervasyon)));
                }
                return _tahsisTipListe;
            }
        }

        private static TNSCollection _hizmetGrubuListe = null;
        public static TNSCollection HizmetGrubuListe
        {
            get
            {
                if (_hizmetGrubuListe == null)
                {
                    _hizmetGrubuListe = new TNSCollection();
                    _hizmetGrubuListe.Add(new IntKodStrAd((int)ENUMHizmetTablolari.EmekliSandigi, "Emekli Sandığı"));
                    _hizmetGrubuListe.Add(new IntKodStrAd((int)ENUMHizmetTablolari.SSK, "SSK"));
                    _hizmetGrubuListe.Add(new IntKodStrAd((int)ENUMHizmetTablolari.Bagkur, "Bağkur"));
                    _hizmetGrubuListe.Add(new IntKodStrAd((int)ENUMHizmetTablolari.Borclanma, "Borçlanma"));
                }
                return _hizmetGrubuListe;
            }
        }


        private static System.Collections.Hashtable _SendikaListesi { get; set; } = null;
        public static System.Collections.Hashtable SendikaListesi
        {
            get
            {
                if (_SendikaListesi == null)
                {
                    _SendikaListesi = new System.Collections.Hashtable();
                    ObjectArray oa = TNS.IKY.Arac.Tanimla().ListeleSendikaBilgi(null, new SendikaBilgi());
                    if (oa.objeler != null)
                    {
                        foreach (SendikaBilgi item in oa.objeler)
                        {
                            _SendikaListesi[item.sendikaNo] = item;
                        }
                    }
                }
                return _SendikaListesi;
            }
            set
            {
                _SendikaListesi = value;
            }
        }

        public static SendikaBilgi SendikaBilgi(int sendikaNo)
        {
            try
            {
                return (SendikaBilgi)SendikaListesi[sendikaNo];
            }
            catch
            {
                return null;
            }
        }

        public static string ENUMSGKNeviAd(ENUMSGKNevi sgkNevi)
        {
            switch (sgkNevi)
            {
                case ENUMSGKNevi.SSKKamu: return "SSK Kamu";
                case ENUMSGKNevi.SSKOzel: return "SSK Özel";
                case ENUMSGKNevi.BagKur: return "Bağ-Kur";
                case ENUMSGKNevi.YurtDisi: return "Yurt Dışı Hizmetleri";
                case ENUMSGKNevi.BankaEmekliSandiklari: return "Banka Emekli Sandıkları";
                case ENUMSGKNevi.SSKAskerlikBorclanmasi: return "SSK Askerlik Borçlanması";
                case ENUMSGKNevi.SSKDigerBorclanma: return "SSK Diğer Borçlanma";
                case ENUMSGKNevi.BagKurAskerlikBorclanmasi: return "Bağ-Kur Askerlik Borçlanması";
                case ENUMSGKNevi.BagKurDigerBorclanma: return "Bağ-Kur Diğer Borçlanma";
                case ENUMSGKNevi.BankaEmekliSandiklariAskerlikBorclanmasi: return "Banka Emekli Sandık Askerlik Borçlanması";
                case ENUMSGKNevi.BankaEmekliSandiklariDigerBorclanma: return "Banka Emekli Sandık Diğer Borçlanma";
                case ENUMSGKNevi.SSK_FHZ: return "SSK FHZ";
                default: return "";
            }
        }

        public static object HitapBorcNeviAd(string hitapBorcNevi)
        {
            if (string.IsNullOrWhiteSpace(hitapBorcNevi)) return "";
            hitapBorcNevi = hitapBorcNevi.Trim();
            if (hitapBorcNevi == "2") return "Askerlik Borçlanması";
            if (hitapBorcNevi == "22") return "Personel Mevzuatına Göre Aylıksız İzin Süreleri(4c)";
            if (hitapBorcNevi == "28") return "5434 SK geçici 209 borçlanması";
            if (hitapBorcNevi == "30") return "5510 S.K. Geç. 4. MD. 17. Fıkra Borç.(5525 S.K.)";
            if (hitapBorcNevi == "31") return "5510 S.K. Geç. 36. MD. Borç.(1402)";
            if (hitapBorcNevi == "32") return "5510 S.K. Geç. 43. MD. Borç.(1416)";
            if (hitapBorcNevi == "33") return "5510 S.K. Geç. 44. MD. Borç.(5525 S.K.)";
            if (hitapBorcNevi == "34") return "5510 S.K. Geç. 51. MD. Borç.(5525 S.K.)";
            if (hitapBorcNevi == "35") return "5510 S.K. 41 MD. (4/C STATÜLÜ DOĞUM BORÇLANMASI) (22.04.2015 eklendi)";
            if (hitapBorcNevi == "36") return "5510 S.K. Geç. 56. MD. Borç.(5525 S.K.) (01.07.2015 tarihinde eklendi)";
            return "";
        }

        public static object HitapKHADurum(string khaDurum)
        {
            if (string.IsNullOrWhiteSpace(khaDurum)) return "";
            khaDurum = khaDurum.Trim();
            if (khaDurum == "0") return "Değerlendirilmedi";
            if (khaDurum == "1") return "Prim gün sayısının 2/3 oranında değerlendirildi";
            if (khaDurum == "2") return "Prim gün sayısının 3/4 oranında değerlendirildi";
            if (khaDurum == "3") return "Prim gün sayısının 4/4 oranında değerlendirildi";
            if (khaDurum == "4") return "Belirlenemedi";
            if (khaDurum == "5") return "İki tarih arasının tamamı değerlendirildi";
            return "";
        }

        public static byte[] ResimAyarla(byte[] liste, int buyukluk, int en, int boy)
        {
            if (buyukluk == 1)
            {
                en = 60;
                boy = 60;
            }
            else if (buyukluk == 2)
            {
                en = 50;
                boy = 50;
            }
            else if (buyukluk == 3)
            {
                en = 150;
                boy = 150;
            }
            else if (buyukluk == 4)
            {
                en = 800;
                boy = 800;
            }
            if (en > 0 && boy > 0 && liste != null)
            {
                var ms = new System.IO.MemoryStream(liste);
                System.Drawing.Image im = System.Drawing.Image.FromStream(ms);

                double aspRatioW = en * 1.0 / im.Width;
                double aspRatioH = boy * 1.0 / im.Height;
                double aspRatio = Math.Min(aspRatioW, aspRatioH);

                en = (int)(im.Width * aspRatio);
                boy = (int)(im.Height * aspRatio);
                System.Drawing.Bitmap bm = new System.Drawing.Bitmap(im, en, boy);

                var ms2 = new System.IO.MemoryStream();
                bm.Save(ms2, System.Drawing.Imaging.ImageFormat.Jpeg);
                liste = ms2.ToArray();
            }
            return liste;
        }

        public static int GunHesapla360(DateTime kucuk, DateTime buyuk)
        {
            if (buyuk.Equals(DateTime.MinValue) || kucuk.Equals(DateTime.MinValue))
                return 0;

            int kiyas = buyuk.CompareTo(kucuk);
            if (kiyas > 0)
            {
                int gun = buyuk.Day - kucuk.Day;
                if (gun < 0)
                {
                    gun += 30;
                    buyuk = buyuk.AddMonths(-1);
                }
                int ay = buyuk.Month - kucuk.Month;
                if (ay < 0)
                {
                    ay = ay + 12;
                    buyuk = buyuk.AddYears(-1);
                }
                int yil = buyuk.Year - kucuk.Year;
                return yil * 360 + ay * 30 + gun;
            }
            else if (kiyas < 0)
                return -1 * GunHesapla360(buyuk, kucuk);
            return 0;
        }

        public static bool KVKK()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "KVKK") == "1";
        }
    }

    public interface IIKYServis
    {
        //Personel
        Sonuc PersonelKaydet(Kullanici islemiYapan, Personel p);
        Sonuc PersonelTCKimlikNoDegistir(Kullanici islemiYapan, Personel p);

        ObjectArray PersonelListele(Kullanici islemiYapan, Personel p, bool hizmetleriOku, bool ozet);
        ObjectArray PersonelIstatistikListele(Kullanici kullanan, Personel p, int istatistikKod);
        ObjectArray PersonelLojmanliListele(Kullanici kullanan, Personel p);

        //Personel Belgeleri
        Sonuc PersonelBelgeKaydet(Kullanici islemiYapan, PersonelBelgeler belge);
        ObjectArray PersonelBelgeListele(Kullanici islemiYapan, PersonelBelgeler belge);
        Sonuc PersonelBelgeListesiGetir(Kullanici islemiYapan, PersonelBelgeler belge);
        Sonuc PersonelBelgeSil(Kullanici islemiYapan, PersonelBelgeler belge);

        //Personel Adres
        Sonuc PersonelAdresKaydet(Kullanici islemiYapan, PersonelIletisim iletisim);
        PersonelIletisim PersonelAdresListele(Kullanici islemiYapan, int kod);

        PersonelNufus PersonelNufusListele(Kullanici islemiYapan, int kod);

        //Personel Askerlik
        Sonuc PersonelAskerlikKaydet(Kullanici islemiYapan, PAskerlik askerlik);
        PAskerlik PersonelAskerlikListele(Kullanici islemiYapan, int kod);

        //PersonelKurs Bilgisi
        Sonuc PersonelKursKaydet(Kullanici islemiYapan, PersonelKurs p);
        ObjectArray PersonelKursListele(Kullanici islemiYapan, PersonelKurs p);
        Sonuc PersonelKursSil(Kullanici islemiYapan, PersonelKurs p);

        //PersonelAile Bilgisi
        Sonuc PersonelAileKaydet(Kullanici islemiYapan, PersonelAile p);
        ObjectArray PersonelAileListele(Kullanici kullanan, int aileKod, int personelKod, string personelKodlar, string personelTur, string organizasyonKopruKodu, int yukumlu);
        Sonuc PersonelAileSil(Kullanici islemiYapan, PersonelAile p);

        //PersonelMalBildirimi Bilgisi
        Sonuc PersonelMalBildirimiKaydet(Kullanici islemiYapan, PersonelMalBildirimi p);
        ObjectArray PersonelMalBildirimiListele(Kullanici islemiYapan, PersonelMalBildirimi p);
        ObjectVAN PersonelMalBildirimiOku(Kullanici islemiYapan, int malBildirimiKod);
        Sonuc PersonelMalBildirimiSil(Kullanici islemiYapan, PersonelMalBildirimi p);

        //Personel Bilgi Birikimi

        /// <summary>
        /// Personel nitelik bilgilerini varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// Personelin sistemde kayıtlı olması gerekmektedir, eğer kayıtlı değilse hata döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel nitelik bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc PersonelBilgiBirikimiKaydet(Kullanici kullanan, PersonelBilgiBirikimi personel);
        /// <summary>
        /// Verilen kriterlere uygun olan personel nitelik bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel nitelik bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında personel nitelik listesi döndürülür.</returns>
        ObjectArray PersonelBilgiBirikimiListele(Kullanici kullanan, PersonelBilgiBirikimi personel);


        //PersonelCezaÖdül Bilgisi
        Sonuc PersonelCezaOdulKaydet(Kullanici islemiYapan, PersonelCezaOdul p);
        ObjectArray PersonelCezaOdulListele(Kullanici islemiYapan, PersonelCezaOdul p);
        Sonuc PersonelCezaOdulSil(Kullanici islemiYapan, PersonelCezaOdul p);

        ////PersonelÖğrenim Bilgisi
        Sonuc PersonelOgrenimKaydet(Kullanici islemiYapan, PersonelOgrenim p);
        ObjectArray PersonelOgrenimListele(Kullanici islemiYapan, PersonelOgrenim p);
        Sonuc PersonelOgrenimSil(Kullanici islemiYapan, PersonelOgrenim p);

        //PersonelDil Bilgisi
        Sonuc PersonelDilKaydet(Kullanici islemiYapan, PersonelDil p);
        ObjectArray PersonelDilListele(Kullanici islemiYapan, PersonelDil p);
        Sonuc PersonelDilSil(Kullanici islemiYapan, PersonelDil p);

        //PersonelKadro Bilgisi
        Sonuc PersonelKadroKaydet(Kullanici islemiYapan, PersonelKadro p);
        ObjectArray PersonelKadroListele(Kullanici islemiYapan, PersonelKadro p, bool tarihArtan);
        ObjectArray HizmetleriGetir(Kullanici kullanan, string organizasyonKopruKod, string personelKod, string personelTur, TNSDateTime sonTarih, int siralamaListesi, string raporKod, int aktiflik);
        Sonuc PersonelKadroAyarKaydet(Kullanici islemiYapan, KadroAyar p);
        ObjectArray PersonelKadroAyarListele(Kullanici kullanan, KadroAyar p);

        //PersonelKart Bilgisi
        Sonuc PersonelKartKaydet(Kullanici islemiYapan, PersonelKart p);
        ObjectArray PersonelKartListele(Kullanici islemiYapan, PersonelKart p);
        Sonuc PersonelKartSil(Kullanici islemiYapan, PersonelKart p);

        //Personel Resim Bilgisi
        Sonuc PersonelResimKaydet(PersonelResim bilgi);
        Sonuc PersonelResimSil(PersonelResim bilgi);
        byte[] PersonelResimGetir(int pKod, int buyukluk, int en, int boy);

        //İzin Tür Bilgisi
        Sonuc IzinTurKaydet(Kullanici islemiYapan, IzinTur p);
        ObjectArray IzinTurListele(IzinTur p);
        Sonuc IzinTurSil(Kullanici islemiYapan, IzinTur p);

        //Unvan Bilgisi
        Sonuc UnvanKaydet(Kullanici islemiYapan, Unvan p);
        ObjectArray UnvanListele(Unvan p);
        Sonuc UnvanSil(Kullanici islemiYapan, Unvan p);


        Sonuc KadroKontenjanKaydet(Kullanici kullanan, KadroKontenjan kk);
        //Sonuc KadroKontenjanSil(Kullanici kullanan, KadroKontenjan kk);
        KadroKontenjan[] KadroKontenjanListele(Kullanici kullanan, KadroKontenjan kk, bool detay);

        ObjectArray PersonelListeleIzinDurum(Kullanici islemiYapan, PersonelIzinDurum pk);
        Sonuc PersonelIzinKaydet(Kullanici islemiYapan, PersonelIzin p);
        ObjectArray PersonelIzinListele(Kullanici kullanan, PersonelIzin p);
        Sonuc PersonelIzinSil(Kullanici islemiYapan, PersonelIzin p);
        Sonuc PersonelIzinYetkiKontrol(Kullanici islemiYapan, PersonelIzin p, ENUMTARIHCEISLEMTIP islemTip);
        PersonelIzinGunleri PersonelYilBazliYillikIzinBilgileri(Kullanici kullanan, int personelKod, DateTime tarih);
        Sonuc PersonelIzinDevirKaydet(Kullanici islemiYapan, PersonelIzinDevir[] pid);
        ObjectArray PersonelIzinDevirListele(Kullanici kullanan, PersonelIzinDevir pid, int organizasyonKodu);
        Sonuc PersonelIzinDevirSil(Kullanici islemiYapan, PersonelIzinDevir pid);
        Sonuc PersonelIzinDevirleriAktar(Kullanici kullanan, PersonelIzinDevir pid, int organizasyonKodu);
        IzinBilgi PersonelKullanilanIzinHesapla(Kullanici kullanan, int personelKod, int yil, int haricKod);
        int PersonelKendindenKucukKullanilanIzinHesapla(int personelKod, TNSDateTime baslamaTarihi);
        Sonuc PersonelIzinImzaGuncelle(Kullanici islemiYapan, PersonelIzin pi);

        Sonuc PersonelTarihceKaydet(Kullanici kullanan, object yeni, object eski, ENUMTARIHCE alan, int pKod, ENUMTARIHCEISLEMTIP islemTipi);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="personelTCKimlikNo"></param>
        /// <returns>TARIH,KATEGORI,DEGISENALAN,YENIDEGER,ESKIDEGER,DEGISIKLIKYAPAN</returns>
        String[] PersonelTarihceListele(Kullanici kullanan, int personelKod);

        //Sözlük Bilgisi
        ObjectArray SozlukListele(Sozluk p);
        Sonuc SozlukKaydet(Kullanici islemiYapan, Sozluk p);
        Sonuc SozlukSil(Kullanici islemiYapan, Sozluk p);

        ObjectArray DoluBosKadroListele(PersonelKadro pk);

        Sonuc AtamaOnayBilgileriniGuncelle(Kullanici kullanan, TNSCollection atama, int onayIslem);

        Sonuc CBVerileriniAktar(Kullanici kullanan, int tip);
        ObjectArray HizmetSebebiListele(ENUMHizmetSebebiNevi sebepler);
        ObjectArray EskiHizmetTipiListele();
        Personel HizmetBelgesiRaporu(Kullanici kullanan, int pKod);
        ObjectArray TerfisiGelenler(Kullanici kullanan, string yilAy, int tip);
        Sonuc PersonelTerfiKaydet(Kullanici islemiYapan, Terfi p);
        ObjectArray PersonelTerfiListele(Kullanici kullanan, Terfi p);
        Sonuc EKEKHATerfiEt(Kullanici kullanan, Terfi terfi);
        ObjectArray KadroRaporlari(Kullanici kullanan, string personelTuru, string tip, string personelKod, string organizasyonKopruKodu, int siralamListesi);
        ObjectArray BakmaklaYukumluListesi(Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, int yukumlu, int aktiflik);
        ObjectArray OgrenimDurumuListesi(Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, int aktiflik);
        MailImzaRapor MailImzaRaporu(int personelKod);
        ObjectArray IzinListesi(Kullanici kullanan, TNS.IKY.Personel personel);
        ObjectArray KartListesi(Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, string raporTuru);
        ObjectArray UnvanaGoreHizmetGrupla(int pKod);

        Personel NakilBildirimRaporu(int pkod);

        /// <summary>
        /// PersonelTipi listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PersonelTipi yapısında PersonelTipi: Personel Tipi bilgilerini tutan değişken</param>
        ObjectArray ListelePersonelTip(Kullanici kullanan, PersonelTipi form);
        ObjectArray ListelePersonelTipGrup(Kullanici kullanan, bool sadeceKullanilanlar);

        /// <summary>
        /// PersonelTipi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PersonelTipi yapısında PersonelTipi: Personel Tipi bilgilerini tutan değişken</param>
        ObjectVAN OkuPersonelTip(Kullanici kullanan, PersonelTipi form);

        /// <summary>
        /// PersonelTipi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PersonelTipi yapısında PersonelTipi: Personel Tipi bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetPersonelTip(Kullanici kullanan, PersonelTipi form);

        PersonelMaasParam MaasParamListele(Kullanici kullanan, string personelTCKimlikNo, int yil);

        Sonuc MaasParamKaydet(Kullanici kullanan, TNSCollection kisiler, TNSCollection degiskenler);

        Sonuc MaasMatrahKaydet(Kullanici kullanan, int yil, string surecNo, string tabloAd, TNSCollection matrahlar);

        Sonuc MaasMatrahSil(Kullanici kullanan, int yil, string surecNo, string tabloAd, TNSCollection tcListe);

        ObjectArray MaasMatrahAktar(Kullanici kullanan, TNSDateTime matrahTarih, TNSCollection matrahlar, int yontem, int yukle);

        ObjectArray PersonelBulOncekiSonraki(Kullanici kullanan, string sicilNo, bool onceki);

        /// <summary>
        /// LojmanSite listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">LojmanSite yapısında LojmanSite:  bilgilerini tutan değişken</param>
        ObjectArray LojmanSiteListele(Kullanici kullanan, LojmanSite form);

        /// <summary>
        /// LojmanSite nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">LojmanSite yapısında LojmanSite:  bilgilerini tutan değişken</param>
        ObjectVAN LojmanSiteOku(Kullanici kullanan, LojmanSite form);

        /// <summary>
        /// LojmanSite nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">LojmanSite yapısında LojmanSite:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc LojmanSiteKaydet(Kullanici kullanan, LojmanSite form);
        Sonuc LojmanSiteSil(Kullanici islemiYapan, int kod);
        ObjectArray SiteYetkilileriListele(Kullanici kullanan, IKYSiteYetki form);
        Sonuc LojmanSiteYetkiKaydet(Kullanici kullanan, IKYSiteYetki[] pList);
        Sonuc LojmanSiteYetkiAktifPasifYap(Kullanici kullanan, IKYSiteYetki p);

        ObjectArray LojmanSiteBlokListele(Kullanici kullanan, LojmanSiteBlok form);
        ObjectVAN LojmanSiteBlokOku(Kullanici kullanan, LojmanSiteBlok form);
        Sonuc LojmanSiteBlokKaydet(Kullanici kullanan, LojmanSiteBlok form);
        Sonuc LojmanSiteBlokSil(Kullanici islemiYapan, int kod);

        ObjectArray LojmanSiteBlokDaireListele(Kullanici kullanan, LojmanSiteBlokDaire form);
        ObjectVAN LojmanSiteBlokDaireOku(Kullanici kullanan, LojmanSiteBlokDaire form);
        Sonuc LojmanSiteBlokDaireKaydet(Kullanici kullanan, LojmanSiteBlokDaire form);
        Sonuc LojmanSiteBlokDaireSil(Kullanici islemiYapan, int kod);

        Sonuc PersonelLojmanKaydet(Kullanici islemiYapan, PersonelLojmani p);
        ObjectArray PersonelLojmanListele(Kullanici kullanan, PersonelLojmani p);
        Sonuc PersonelLojmanSil(Kullanici islemiYapan, PersonelLojmani p);
        ObjectArray LojmanTurleriniListele(Kullanici kullanan, LojmanTuru p);
        ObjectArray LojmanListesiRaporu(Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, int aktiflik, int siteKod, int tahsisTip, int siralamaListesi, int yil1, int yil2);
        ObjectArray LojmanDaireBazliRaporu(Kullanici kullanan, int siteKod, int daireBosluk);
        ObjectArray PersonelDoluDaireListesi(Kullanici kullanan, PersonelLojmani p);
        ObjectArray BosDaireListesi(Kullanici kullanan, int blokKod);
        Sonuc PersonelLojmanRezervdenTahsisEt(Kullanici islemiYapan, PersonelLojmani p);

        string KullanicininIslemleri(Kullanici kullanan, string kopruKod, MUH.ENUMOGY ogy);

        ObjectArray SozlukGiderListele(Kullanici kullanan, SozlukGider form);
        ObjectVAN SozlukGiderOku(Kullanici kullanan, SozlukGider form);
        Sonuc SozlukGiderKaydet(Kullanici kullanan, SozlukGider form);
        Sonuc SozlukGiderSil(Kullanici islemiYapan, int kod);

        ObjectArray SiteGiderListele(Kullanici kullanan, SiteGider form);
        Sonuc SiteGiderKaydet(Kullanici kullanan, int siteKod, int yil, int ay, TNSCollection giderBilgiler);

        ObjectArray PersonelSiteKesintisiListele(Kullanici kullanan, PersonelSiteKesinti form);
        Sonuc PersonelSiteKesintileriTopluKaydet(Kullanici kullanan, PersonelSiteKesinti[] form);
        ObjectArray PersonelSiteKesintiRaporu(Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, int aktiflik, int yil, int ay, int siteKod);
        Sonuc PersonelSiteKesintisiToplamSil(Kullanici islemiYapan, int toplamKod);
        Sonuc PersonelKesintileriKaydet(Kullanici islemiYapan, TNSCollection col);

        ObjectArray KomisyonListele(Kullanici kullanan, Komisyon form);
        ObjectVAN KomisyonOku(Kullanici kullanan, Komisyon form);
        Sonuc KomisyonKaydet(Kullanici kullanan, Komisyon form);
        Sonuc KomisyonSil(Kullanici kullanan, int kod);

        ObjectArray PersonelGiderListele(Kullanici kullanan, IKYPersonelGider yemek);
        Sonuc PersonelGiderKaydet(Kullanici kullanan, IKYPersonelGider yemek, bool yeni);
        //Sonuc PersonelGiderSil(Kullanici kullanan, IKYPersonelGider yemek);
        //ObjectArray YerleskelereGoreGiderListesi(Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, int aktiflik, string yemekhaneler, DateTime baslamaTarihi, DateTime bitisTarihi);
        //Sonuc PersonelYemekGuncelleMailGonderildi(Kullanici kullanan, IKYPersonelGider yemek);
        //ObjectArray PersonelHesaplanmisGiderListesi(Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, int aktiflik, string yemekhaneler, int yil, int ay);

        /// <summary>
        /// HizmetSebebi listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">HizmetSebebi yapısında HizmetSebebi: Hizmet Sebepleri bilgilerini tutan değişken</param>
        ObjectArray ListeleHizmetSebebi(Kullanici kullanan, HizmetSebebi form);

        /// <summary>
        /// HizmetSebebi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">HizmetSebebi yapısında HizmetSebebi: Hizmet Sebepleri bilgilerini tutan değişken</param>
        ObjectVAN OkuHizmetSebebi(Kullanici kullanan, HizmetSebebi form);

        /// <summary>
        /// HizmetSebebi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">HizmetSebebi yapısında HizmetSebebi: Hizmet Sebepleri bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetHizmetSebebi(Kullanici kullanan, HizmetSebebi form);

        /// <summary>
        /// HizmetSebebi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">HizmetSebebi yapısında HizmetSebebi: Hizmet Sebepleri bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SilHizmetSebebleri(Kullanici kullanan, TNSCollection formlar);

        /// <summary>
        /// Gorevlendirme listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Gorevlendirme yapısında Gorevlendirme: Görevlendirme bilgilerini tutan değişken</param>
        ObjectArray ListeleGorevlendirmeler(Kullanici kullanan, Gorevler form);

        /// <summary>
        /// Gorevlendirme nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Gorevlendirme yapısında Gorevlendirme: Görevlendirme bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetGorevlendirmeler(Kullanici kullanan, Gorevler form);

        Sonuc SilGorevlendirmeler(Kullanici kullanan, Gorevler form);

        /// <summary>
        /// Verilen birimdeki (öncelikle organizasyon kod veya harcama koduna göre) onay bekleyen hizmet hareketlerini listeler.
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="orgKod"></param>
        /// <param name="muhasebeKod"></param>
        /// <param name="muhasebeBirimKod"></param>
        /// <returns></returns>
        ObjectVAN OnaysizKayitlar(Kullanici kullanan, int orgKod, bool sadeceSayilar);

        int SebepKodBul(ENUMHizmetSebebiNevi sebepTur);
        Sonuc PersonelGiderAktar(Kullanici kullanan, IKYPersonelGider gider);

        ////PersonelMemuriyet Bilgisi
        //Sonuc PersonelMemuriyetKaydet(Kullanici islemiYapan, PersonelMemuriyet p);
        //ObjectArray PersonelMemuriyetListele(Kullanici islemiYapan, PersonelMemuriyet p);
        //Sonuc PersonelMemuriyetSil(Kullanici islemiYapan, PersonelMemuriyet p);

        ////Sinif Bilgisi
        //Sonuc SinifKaydet(Kullanici islemiYapan, Sinif p);
        //ObjectArray SinifListele(Sinif p);
        //Sonuc SinifSil(Kullanici islemiYapan, Sinif p);
        ////Banka Bilgisi
        //Sonuc BankaKaydet(Kullanici islemiYapan, Banka p);
        //ObjectArray BankaListele(Banka p);
        //Sonuc BankaSil(Kullanici islemiYapan, Banka p);

        ////Banka Şube Bilgisi
        //Sonuc BankaSubeKaydet(Kullanici islemiYapan, BankaSube p);
        //ObjectArray BankaSubeListele(BankaSube p);
        //Sonuc BankaSubeSil(Kullanici islemiYapan, BankaSube p);

        ////Neden Bilgisi
        //Sonuc NedenKaydet(Kullanici islemiYapan, Neden p);
        //ObjectArray NedenListele(Neden p);
        //Sonuc NedenSil(Kullanici islemiYapan, Neden p);
        //ObjectArray PersonelTarihceListele(Kullanici kullanan, PersonelTarihce p);
        //ObjectArray PersonelListeleUnvanKadro(Kullanici islemiYapan, Personel p);
        //ObjectArray PersonelListelePersonelRapor(Kullanici islemiYapan, Personel p);
        //ObjectArray PersonelListeleEgitimRapor(Kullanici islemiYapan, Personel p, PersonelKurs pk);
        ////UstBirim Bilgisi
        //Sonuc UstBirimKaydet(Kullanici islemiYapan, UstBirim p);
        //ObjectArray UstBirimListele(UstBirim p);
        //Sonuc UstBirimSil(Kullanici islemiYapan, UstBirim p);

        ////Kadro Bilgisi
        //Sonuc KadroKaydet(Kullanici islemiYapan, ObjectArray p);
        //Sonuc KadroSil(Kullanici islemiYapan, TNSDateTime tarih);
        //ObjectArray KadroRapor(Kullanici islemiYapan, Kadro p);
        //ObjectArray KadroVizeListele();

        //Sözleşmeli Personel Maaş Tanım
        //Sonuc SozlesmeliBordroTanimKaydet(Kullanici islemiYapan, SozlesmeliBordroTanim p);
        //Sonuc SozlesmeliBordroTanimSil(Kullanici islemiYapan, SozlesmeliBordroTanim kriter);

        ////ObjectArray OgrenimBazliPersonelSayilariListele(int muhasebeKod, string birimKod);
        //ObjectArray UnvanBazliPersonelSayilariListele(int organizasyonKod);
        //ObjectArray SinifBazliPersonelSayilariListele(int muhasebeKod, string birimKod);
        //ObjectArray UnvanVeKadroDereceBazliPersonelSayilariListele(Kullanici kullanan, KadroKontenjan kk);

        //ObjectArray PersonelDetayliBilgiGetir(Kullanici kullanan, int kod, string kimlikNo, int eskiKadroKod);
        //ObjectArray PersonelAtamaListele(Kullanici kullanan, PersonelKadro p);
        //        HarcamaBirimi BirimHarcamaBul(Kullanici kullanan, int birimKod);
        //Sonuc PersonelTerfiSil(Kullanici islemiYapan, int kod);        //int HizmetGunuDondur(DateTime baslangicT, DateTime bitisT, int kadroKod);
        //Hizmet HizmetSureBilgileriniGetir(int personelKod, DateTime sonTarih);
        //ObjectArray SirtlikListesi(Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu);
        //ObjectArray DinamikRaporListesi(string personelKod, string personelTur, string organizasyonKopruKodu, string siralamalar, int aktiflik);        //Sonuc SiteGiderSil(Kullanici islemiYapan, int kod);
        //ObjectArray YillarArasiLojmandaOturanlarListesiRaporu(Kullanici kullanan, int aktiflik, int siteKod, int yil1, int yil2);
        //ObjectArray LojmanDegisiklikListesiRaporu(Kullanici kullanan, int aktiflik, int siteKod);
        //ObjectVAN SiteGiderOku(Kullanici kullanan, SiteGider form);
        //ObjectVAN PersonelSiteKesintisiOku(Kullanici kullanan, PersonelSiteKesinti form);
        //Sonuc PersonelSiteKesintisiKaydet(Kullanici kullanan, PersonelSiteKesinti form);
        //Sonuc PersonelSiteKesintisiSil(Kullanici islemiYapan, PersonelSiteKesinti form);
        //Sonuc ZamanliIsler(Kullanici kullanan, DateTime zaman);
        /// <summary>
        /// Gorevlendirme nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Gorevlendirme yapısında Gorevlendirme: Görevlendirme bilgilerini tutan değişken</param>
        //ObjectVAN OkuGorevlendirmeler(Kullanici kullanan, Gorevler form);
        //Personel PersonelMemuriyetTarihleriListele(string tcKimlikNo);
        //ObjectArray PersonelYerleskeListele(Kullanici kullanan, IKYYerleske yerleske);

        ObjectArray PersonelSozlesmeListele(Kullanici kullanan, PersonelSozlesme form);

        Sonuc PersonelSozlesmeKaydet(Kullanici kullanan, PersonelSozlesme[] liste);

        Sonuc PersonelSozlesmeSil(Kullanici kullanan, PersonelSozlesme form);
        Sonuc TarimVerileriniAktar(string tcKimlikNo);
        Sonuc PersonelGiderYukle(Kullanici kullanan, TNSCollection liste);
        /// <summary>
        /// SendikaBilgi listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SendikaBilgi yapısında SendikaBilgi:  bilgilerini tutan değişken</param>
        ObjectArray ListeleSendikaBilgi(Kullanici kullanan, SendikaBilgi form);

        /// <summary>
        /// SendikaBilgi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SendikaBilgi yapısında SendikaBilgi:  bilgilerini tutan değişken</param>
        ObjectVAN OkuSendikaBilgi(Kullanici kullanan, SendikaBilgi form);

        /// <summary>
        /// SendikaBilgi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SendikaBilgi yapısında SendikaBilgi:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetSendikaBilgi(Kullanici kullanan, SendikaBilgi form);
        Sonuc PersonelSendikadanCikart(Kullanici kullanan, int pKod);
        Sonuc PersonelSendikaOranDegistir(Kullanici kullanan, int pKod, double kesintiOrani);
        Sonuc PersonelSendikayaEkle(Kullanici kullanan, int sendikaKod, TNSCollection pKodList, double kesintiOran, bool yetkiKontroluYap);
        ObjectVAN OkuKisiSendikaBilgi(Kullanici kullanan, string personelTCKimlikNo);
    }
}