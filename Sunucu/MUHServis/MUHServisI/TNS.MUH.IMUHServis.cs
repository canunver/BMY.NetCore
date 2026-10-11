using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using TNS.BKD;
using TNS.DEG;
using TNS.KYM;

namespace TNS.MUH
{
    [System.Serializable()]
    public class ManasGunlukTakip
    {
        public string hesapKod = "";
        public double borc = 0;
        public double alacak = 0;
        public double dovizBorc = 0;
        public double dovizAlacak = 0;
        public double bakiye = 0;
        public double dovizBakiye = 0;
        public double bakiyeBorc = 0;
        public double bakiyeAlacak = 0;
        public double dovizBakiyeBorc = 0;
        public double dovizBakiyeAlacak = 0;

        public TNSDateTime sorguTarih = new TNSDateTime();
    }

    /// <summary>
    /// IstatistikKodDeger: İstatistik Kodu Seçim Değerleri
    /// </summary>
    [System.Serializable()]
    public class IstatistikKodDeger
    {
        public IstatistikKodDeger()
        {
            this.degerKod = 0;
            this.aciklama = "";
            this.ekAlan1 = "";
            this.ekAlan2 = "";
            this.ekAlan3 = "";
        }

        public IstatistikKodDeger(int degerKod, string aciklama, string ekAlan1, string ekAlan2, string ekAlan3)
        {
            this.degerKod = degerKod;
            this.aciklama = aciklama;
            this.ekAlan1 = ekAlan1;
            this.ekAlan2 = ekAlan2;
            this.ekAlan3 = ekAlan3;
        }

        /// <summary>
        /// Değer Kod: İstatistik Değeri
        /// </summary>
        public int degerKod { get; set; }

        /// <summary>
        /// Açıklama: Liste seçim değeri
        /// </summary>
        public string aciklama { get; set; }

        /// <summary>
        /// ekAlan1: Liste seçim değeri
        /// </summary>
        public string ekAlan1 { get; set; }

        /// <summary>
        /// ekAlan2: Liste seçim değeri
        /// </summary>
        public string ekAlan2 { get; set; }

        /// <summary>
        /// ekAlan3: Liste seçim değeri
        /// </summary>
        public string ekAlan3 { get; set; }

        public static string AciklamaBul(IstatistikKodDeger istKod)
        {
            if (istKod == null) return "";
            return istKod.aciklama;
        }
    }


    /// <summary>
    /// IstatistikKod: İstatistik Kodu
    /// </summary>
    [System.Serializable()]
    public class IstatistikKod
    {
        /// <summary>
        /// Kod: İstatistik Kodu
        /// </summary>
        public int kod { get; set; }

        /// <summary>
        /// Ad: İstatistik Adı
        /// </summary>
        public string ad { get; set; }
        public int zorunlu { get; set; }

        public string ekAlan1Ad { get; set; }
        public string ekAlan2Ad { get; set; }
        public string ekAlan3Ad { get; set; }

        /// <summary>
        /// IstatistikKodDeger: İstatistik Kodu Seçim Değerleri
        /// </summary>
        public TNSCollection istatistikKodDeger; //IstatistikKodDeger

        public IstatistikKod()
        {
            istatistikKodDeger = new TNSCollection();
        }

        public void ZorunlulukAta(bool oeZorunlu, bool mifZorunlu)
        {
            zorunlu = 0;
            if (oeZorunlu) zorunlu = zorunlu + 1;
            if (mifZorunlu) zorunlu = zorunlu + 10;
        }
    }


    /// <summary>
    /// GondermeTenkisIcmalKullanici: 
    /// </summary>
    [System.Serializable()]
    public class GondermeTenkisIcmalKullanici
    {
        /// <summary>
        /// Kur Kod: Bütçe birimini giriniz.
        /// </summary>
        public string kulKod { get; set; }

        public GondermeTenkisIcmalKullanici(string kulKod)
        {
            this.kulKod = kulKod;
        }
    }

    /// <summary>
    /// GondermeTenkisIcmal: 
    /// </summary>
    [System.Serializable()]
    public class GondermeTenkisIcmal
    {

        /// <summary>
        /// Yıl: Bütçe yılını giriniz.
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// İcmal No: İcmal numarasını giriniz.
        /// </summary>
        public int icmalNo { get; set; }

        /// <summary>
        /// Kur Kod: Bütçe birimini giriniz.
        /// </summary>
        public string kurKod { get; set; }

        /// <summary>
        /// Form Durum: Form durumunu giriniz.
        /// </summary>
        public int formDurum { get; set; }

        /// <summary>
        /// kullanicilar: İcmal belgesine ödeme emri ekleyebilecek kullanıcılar
        /// </summary>
        public TNSCollection kullanicilar;
        public TNSDateTime araBasTarih;
        public TNSDateTime araBitTarih;

        public GondermeTenkisIcmal(int yil, ENUMBelgeTuru  belgeTur, int icmalNo)
        {
            this.yil = yil;
            this.belgeTur = belgeTur;
            this.icmalNo = icmalNo;
        }

        public GondermeTenkisIcmal()
        {
            this.yil = 0;
            this.belgeTur=ENUMBelgeTuru.TANIMSIZ;
            this.icmalNo = 0;
        }

        public string durum
        {
            get
            {
                if (formDurum < 5)
                    return "Yeni";
                else if (formDurum == 5)
                    return "Onaylı";
                return "Bilinmiyor";
            }
        }

        public int icmalBas { get; set; }

        public int icmalBit { get; set; }

        public bool KullaniciVarMi(string kullaniciKodu)
        {
            if (kullanicilar == null || kullanicilar.Count == 0) return true;
            bool kulBuldu = false;
            foreach (GondermeTenkisIcmalKullanici kulKod in kullanicilar)
            {
                if (kulKod.kulKod == kullaniciKodu)
                {
                    kulBuldu = true;
                }
            }
            return kulBuldu;
        }

        public ENUMBelgeTuru belgeTur { get; set; }
        public TNSDateTime kayitTarih { get; set; } = new TNSDateTime();
        public DateTime dtKayitTarih
        {
            get
            {
                return kayitTarih.Oku();
            }
            set
            {
                kayitTarih.Yaz(value);
            }
        }

        public string belgeTurAd
        {
            get
            {
                if (belgeTur == ENUMBelgeTuru.ODENEKGONDERME) return "Ödenek Gönderme";
                else if (belgeTur == ENUMBelgeTuru.TENKIS) return "Tenkis";
                else return "Bilinmiyor";
            }
        }

        public string kisiler
        {
            get
            {
                string donusDeger = "";
                foreach (GondermeTenkisIcmalKullanici kulKod in kullanicilar)
                {
                    if (donusDeger != "") donusDeger += ", ";
                    donusDeger += kulKod.kulKod;
                }
                return donusDeger;
            }
        }

    }

    /// <summary>
    /// : 
    /// </summary>
    [System.Serializable()]
    public class AvansSorguKriter
    {
        /// <summary>
        /// Yıl: Belge yılını giriniz.
        /// </summary>
        public int yil { get; set; }

        public string kurum { get; set; }
        /// <summary>
        /// Durum: Belge durumunu seçiniz.
        /// </summary>
        public int durum { get; set; }

        public int saymanlikKodu { get; set; }

        public string birim { get; set; }

        public string hesapKod { get; set; }

        public string tcVno { get; set; }

        public bool farklilar { get; set; }

        public bool altHesaptan { get; set; }

        public bool tertipli { get; set; }

        public string tertip { get; set; }

        public bool detay { get; set; }

        public AvansSorguKriter()
        {
            detay = false;
        }
    }

    /// <summary>
    /// AvansSorguCevap: 
    /// </summary>
    [System.Serializable()]
    public class AvansHareket
    {
        public string sira { get; set; }

        /// <summary>
        /// TC Vno: İlgili kişi veya firmanın numarası.
        /// </summary>
        public string tCVno { get; set; }

        /// <summary>
        /// İlgili: İlgili adı soyadı/unvanı.
        /// </summary>
        public string adSoyadUnvan { get; set; }

        /// <summary>
        /// Hesap No: Hesap Numarası.
        /// </summary>
        public string hesapNo { get; set; }

        /// <summary>
        /// Hesap Adı: Hesap Adı.
        /// </summary>
        public string hesapAdi { get; set; }

        private TNSDateTime _sonTarih;
        /// <summary>
        /// Son Tarih: Kişiye ait son işlem tarihi.
        /// </summary>
        public TNSDateTime sonTarih { get { return _sonTarih; } set { _sonTarih = value; } }
        public DateTime sonTarihDT
        {
            get
            {
                if (_sonTarih == null)
                    return DateTime.MinValue;
                else
                    return _sonTarih.Oku();
            }
            set
            {
                if (_sonTarih == null) _sonTarih = new TNSDateTime(value);
                else _sonTarih.Yaz(value);
            }
        }

        /// <summary>
        /// Borç: Borç hareketleri toplamı.
        /// </summary>
        public double borc { get; set; }

        /// <summary>
        /// Alacak: Alacak hareketleri toplamı.
        /// </summary>
        public double alacak { get; set; }

        public string tertip { get; set; }
    }

    public enum ENUMVergiDairesiAktarim
    {
        DAMGAVERGISI = 1,
        KDVTEFKIFATI = 2,
        KDV = 3,
        MUHTASAR = 4,
        BORDRO = 5
    }

    public enum ENUMOdenekKontrolTur
    {
        E2OG_E4OE_E2K = 0,
        E4POG_E4POE_E2PK = 1,
        E4POG_E4POE_E2K = 2,
        E2POG_E4POE_E2PK_E4PTALEP = 3,
        E4POG_E4POE_E4PK = 4,
        TANIMSIZ = 99
    }

    public enum ENUMOdenekGondermeDurum
    {
        OdenenekGondermeEko2 = 0,
        OdenenekGondermeEko4PebMP = 1,
        OdenenekGondermeEko4Peb = 2,
    }

    //public enum ENUMOdenekGondermeTertipYapi
    //{
    //    eko2 = 0,
    //    eko2peb = 1,
    //    eko4peb = 2
    //}

    /// <summary>
    /// Bu sunucu sınıfı için, istemci ve sunucuda ortak kullanılacak statik yordamları içeren sınıf
    /// </summary>
    [Serializable()]
    public static class Arac
    {
        public static void EkoUzunlukOl(OdenekDurumKriter kr, string ekoUz)
        {
            kr.ekoKodKirilim = OrtakFonksiyonlar.ConvertToInt(ekoUz, 0);
            if (kr.ekoKodKirilim == 23)
            {
                kr.ekoKodKirilim = 2;
                kr.eko23 = true;
            }
            else
            {
                kr.eko23 = false;
            }
        }

        public static string SaymanlikTasiIslemAdBul(ENUMSaymanlikTasiIslem item)
        {
            switch (item)
            {
                case ENUMSaymanlikTasiIslem.ODEMEEMRI:
                    return "Ödeme Emri";
                case ENUMSaymanlikTasiIslem.PERSONEL:
                    return "Personeller";
                case ENUMSaymanlikTasiIslem.ABONELIKLER:
                    return "Abonelikler";

                case ENUMSaymanlikTasiIslem.BIRIMBUTCETEKLIFI:
                    return "Birim Bütçe Teklifi";
                case ENUMSaymanlikTasiIslem.BIRIMHIZMETMALIYETI:
                    return "Birim Hizmet Maliyeti";
                case ENUMSaymanlikTasiIslem.DEPREMHARCAMALARI:
                    return "Deprem Harcamaları";
                case ENUMSaymanlikTasiIslem.ARACKIRALAMAPLANI:
                    return "Araç Kiralama Planı";
                case ENUMSaymanlikTasiIslem.SURECTANIMLARI:
                    return "Süreç Tanımları";
                case ENUMSaymanlikTasiIslem.DOGRUDANTEMIN:
                    return "KİK Doğrudan Temin Bilgileri";
                case ENUMSaymanlikTasiIslem.SEYYARGOREVPLANLAMA:
                    return "Seyyar Görev Planlama";
                case ENUMSaymanlikTasiIslem.MEVCUTTASITLAR:
                    return "Mevcut Taşıtlar ve Kira Sözleşmeleri";
                case ENUMSaymanlikTasiIslem.CESITLIODEMELER:
                    return "Harc. Çeşitli Ödemeler (ve Sey. Grv.)";
                case ENUMSaymanlikTasiIslem.ODENEKGONDERME:
                    return "Ödenek Gönderme";
                case ENUMSaymanlikTasiIslem.MASRAFPLANI:
                    return "Masraf Planı";
                case ENUMSaymanlikTasiIslem.MASRAFPLANITAVAN:
                    return "Masraf Planı Tavanı";
                default:
                    return "Tanımsız";
            }
        }

        public static string DegiskenDegerBul(int yil, string cacheID)
        {
            return TNS.DEG.Arac.DegiskenDegerBulCache(yil, cacheID);
        }


        public static void HTDoubleEkle(Hashtable ht, string htKod, double ekleDeger, bool yuvarla)
        {
            object tut = ht[htKod];
            double tutD;
            if (tut != null)
                tutD = (double)tut;
            else
                tutD = 0;

            tutD += ekleDeger;

            if (yuvarla)
                ht[htKod] = Math.Round(tutD, 2);
            else
                ht[htKod] = tutD;
        }

        public static void HTStringEkle(Hashtable ht, string htKod, string deger)
        {
            ht[htKod] = deger;
        }

        public static string OzellikVer(XmlNode node, string ozellikAd)
        {
            try
            {
                return node.Attributes[ozellikAd].Value;
            }
            catch { return string.Empty; }
        }

        public static string BankaHesabindanEmirHesabinaCevir(string hesapKod)
        {
            string retVal = hesapKod.Replace(".", "");
            if (retVal.StartsWith("10201"))
            {
                return "10302" + retVal.Substring(5);
            }
            else if (retVal.StartsWith("105"))
            {
                return "106" + retVal.Substring(3);
            }
            else
                return retVal;
        }

        public static string EmirHesabindanBankaHesabinaCevir(string hesapKod)
        {
            string retVal = hesapKod.Replace(".", "");
            if (retVal.StartsWith("10302"))
            {
                return "10201" + retVal.Substring(5);
            }
            else if (retVal.StartsWith("106"))
            {
                return "105" + retVal.Substring(3);
            }
            else
                return retVal;
        }

        public static string FisTurAdBul(ENUMFISTUR fisTur)
        {
            switch (fisTur)
            {
                case ENUMFISTUR.DONEMICI: return "Dönem İçi";
                case ENUMFISTUR.ACILIS: return "Açılış";
                case ENUMFISTUR.KAPANIS: return "Kapanış";
                case ENUMFISTUR.AMORTISMANKAPANISI: return "Amortisman Kapanış";
                case ENUMFISTUR.SONFIS: return "Son Fiş";
                default: return "";
            }
        }

        public static void YetkiliBirimBul(Kullanici kullanan, out string muhasebeKod, out string birimKod)
        {
            muhasebeKod = "";
            birimKod = "";
            string yetki = kullanan.YetkiTipiDegeriBul("Ödeme Emri/MİF Saymanlık-Birim Yetkisi");

            if (!string.IsNullOrEmpty(yetki))
            {
                string[] yetkiler = yetki.Split('-', ',', ';');

                muhasebeKod = TNS.MUH.Arac.DiziElemanAl(yetkiler, 0, "");
                birimKod = TNS.MUH.Arac.DiziElemanAl(yetkiler, 1, "");
                if (muhasebeKod.StartsWith("*")) muhasebeKod = "";
                if (birimKod.StartsWith("*")) birimKod = "";
            }
        }

        public static string AcilisFisiBul(IMUHServis servis, Kullanici kullanan, int yil)
        {
            TNS.MUH.OdemeEmriMIFForm form = new TNS.MUH.OdemeEmriMIFForm();
            form.yil = yil;
            form.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
            form.fisTurYap(ENUMFISTUR.ACILIS);
            ObjectArray oa = servis.OdemeEmriMIFListele(kullanan, form, false);
            if (oa.sonuc.islemSonuc && oa.ObjeSayisi > 0)
            {
                return ((TNS.MUH.OdemeEmriMIFForm)oa.objeler[0]).belgeNo;
            }
            return "";
        }

        public static string EkonomikKoddanHesapkodBul(string eko, string hesapAna, int ekoKodUzunluk)
        {
            if (string.IsNullOrWhiteSpace(eko)) return "";
            eko = eko.Replace(".", "");

            if (ekoKodUzunluk == 6)
            {
                if (eko.Length != 6) return "";

                string yEko = "";
                if (eko.Length == 8)
                    yEko = hesapAna + "." + eko;
                else
                    yEko = hesapAna + "." + eko.Substring(0, 2) + ".0" + eko.Substring(2, 1) + ".0" + eko.Substring(3, 1) + "." + eko.Substring(4, 2);

                return yEko;

            }
            else if (ekoKodUzunluk == 4)
            {
                if (eko.Length < 4) return "";

                string yEko = "";
                if (eko.Length == 8)
                    yEko = hesapAna + "." + eko.Substring(0, 2) + "." + eko.Substring(2, 2) + "." + eko.Substring(4, 2);
                else
                    yEko = hesapAna + "." + eko.Substring(0, 2) + ".0" + eko.Substring(2, 1) + ".0" + eko.Substring(3, 1);

                return yEko;
            }
            else
                return hesapAna + "." + eko;
        }

        public static string HesapkoddanEkonomikKodBul(int yil, HesapPlaniSatir h, string ekoYapi)
        {
            string[] ekoYapilar = ekoYapi.Split('.');
            string[] hesapParcalar = h.hesapKod.Split('.');
            if (h.ekoBasla == 0) return "";
            if (h.ekoBasla > 0)
            {
                string ekoKod = "";
                for (int i = 0; i < 4; i++)
                {
                    string ekoParca = HesapParcayiEkoParcayaCevir(OrtakFonksiyonlar.DiziElemanAl(hesapParcalar, i + h.ekoBasla, ""), ekoYapilar, i);
                    if (string.IsNullOrEmpty(ekoParca))
                        break;
                    if (ekoKod != "") ekoKod += ".";
                    ekoKod += ekoParca; // OrtakFonksiyonlar.ConvertToInt(ekoParca, 0).ToString(i == 0 || i == 3 ? "00" : "0");
                }
                return ekoKod;
            }
            else
            {
                int ekoBaslaIndeks;
                if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.AVANS))
                    ekoBaslaIndeks = 1;
                else if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.GIDERBUTCESI))
                    ekoBaslaIndeks = 0;
                else if (h.hesapKod.StartsWith("630") || h.hesapKod.StartsWith("830") || h.hesapKod.StartsWith("900"))
                    ekoBaslaIndeks = 0;
                else
                    return "";


                if (hesapParcalar.Length >= 3)
                {
                    string ekoParca = HesapParcayiEkoParcayaCevir(hesapParcalar[1 + ekoBaslaIndeks], ekoYapilar, 0);

                    if (ekoParca[0] != '0')//Eko kod 09 dan büyük olamaz
                    {
                        //if (ekoParca != "30")
                        //    return "";

                        ekoBaslaIndeks++;
                        //630.30.06.01.01 hesap kodu geldiğinde ekokod=06.1 olmalı
                    }

                    string eko = "";
                    for (int i = 1 + ekoBaslaIndeks; i < hesapParcalar.Length; i++)
                    {
                        if (ekoBaslaIndeks > 0 && i == hesapParcalar.Length)//ekoBaslaIndeks 0 dan farklı ise hesap kodunun son basamağı alınmaz
                            break;
                        if (OrtakFonksiyonlar.ConvertToInt(hesapParcalar[i], 0) == 0)//Eko kod 0 ise gerisi alınmaz
                            break;

                        ekoParca = HesapParcayiEkoParcayaCevir(hesapParcalar[i], ekoYapilar, i - (ekoBaslaIndeks + 1));
                        if (string.IsNullOrEmpty(ekoParca)) return eko;
                        if (!string.IsNullOrEmpty(eko)) eko += ".";
                        eko += ekoParca;
                    }
                    return eko;
                }
            }
            return "";
        }

        static string HesapParcayiEkoParcayaCevir(string hesapParca, string[] ekoYapilar, int ekoYapiYer)
        {
            if (ekoYapiYer >= ekoYapilar.Length) return "";
            string ekoParcaYapi = ekoYapilar[ekoYapiYer];
            string fmt = "".PadLeft(ekoParcaYapi.Length, '0');
            return OrtakFonksiyonlar.ConvertToInt(hesapParca, 0).ToString(fmt);
        }

        public static string BirimAdOl(string saymanlikAd, string birimAd)
        {
            return TNS.KYM.Arac.BirimAdOl(saymanlikAd, birimAd);
        }

        public static bool NitelikVarmi(string nitelik, int deger)
        {
            string[] nitelikler = nitelik.Split(';');
            foreach (string n in nitelikler)
            {
                if (string.IsNullOrEmpty(n.Trim())) continue;
                if (n == deger.ToString()) return true;
            }

            return false;
            //return nitelik.IndexOf(";" + deger.ToString() + ";") >= 0;
        }

        /// <summary>
        /// Bu sunucu sınıfının nesnesini .net remoting ile oluşturup döndüren statik fonksiyon
        /// </summary>
        /// <returns>.net remoting ile oluşturulan nesne </returns>
        public static IMUHServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("MUHServisURL");
            IMUHServis servis = TNS.Api.ServisIstemci.Olustur<IMUHServis>(sUrl);
            return servis;
        }

        /// <summary>
        /// Tertips the STR ol.
        /// </summary>
        /// <param name="kur">The kur.</param>
        /// <param name="fon">The fon.</param>
        /// <param name="fin">The fin.</param>
        /// <param name="eko">The eko.</param>
        /// <param name="peb">The peb.</param>
        /// <returns></returns>
        public static string TertipStrOl(string kur, string fon, string fin, string eko, string peb = null)
        {
            string tertip = "";
            if (!string.IsNullOrEmpty(kur + fon + fin + eko))
            {
                tertip = kur + "-" + fon + "-" + fin + "-" + eko;
                if (!string.IsNullOrEmpty(peb))
                    tertip += "-" + peb;
            }
            return tertip;
        }

        /// <summary>
        /// Tertips the STR ol.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static string TertipStrOl(OdemeEmriMIFDetay d)
        {
            string peb = OrtakFonksiyonlar.DiziElemanAl(d.peb.Split('@'), 0, "");
            return TertipStrOl(d.kur.Trim(), d.fon.Trim(), d.fin.Trim(), d.eko.Trim(), peb);
        }

        public static string TertipStrOlPrg(OdemeEmriMIFDetay d)
        {
            string peb = OrtakFonksiyonlar.DiziElemanAl(d.peb.Split('@'), 0, "");
            if (string.IsNullOrWhiteSpace(peb)) return "";
            return TertipStrOl($"{d.prgKod}.{d.aPrgKod}.{d.faalKod}.{d.aFaalKod}", TNS.BKD.Arac.SifirlariAt(d.kur.Trim()), d.fin.Trim(), d.eko.Trim());
        }

        /// <summary>
        /// Tertips the ata.
        /// </summary>
        /// <param name="detay">The detay.</param>
        /// <param name="tertip">The tertip.</param>
        public static void TertipAta(ref OdemeEmriMIFDetay detay, string tertip)
        {
            string[] kodlar = tertip.Split('-');
            if (kodlar.Length > 0) detay.kur = kodlar[0];
            if (kodlar.Length > 1) detay.fon = kodlar[1];
            if (kodlar.Length > 2) detay.fin = kodlar[2];
            if (kodlar.Length > 3) detay.eko = kodlar[3];
            if (kodlar.Length > 4) detay.peb = kodlar[4];
        }

        /// <summary>
        /// Tertips the ata.
        /// </summary>
        /// <param name="detay">The detay.</param>
        /// <param name="kodlar">The kodlar.</param>
        public static void TertipAta(ref OdemeEmriMIFDetay detay, params string[] kodlar)
        {
            if (kodlar.Length > 0) detay.kur = kodlar[0];
            if (kodlar.Length > 1) detay.fon = kodlar[1];
            if (kodlar.Length > 2) detay.fin = kodlar[2];
            if (kodlar.Length > 3) detay.eko = kodlar[3];
            if (kodlar.Length > 4) detay.peb = kodlar[4];
        }

        /// <summary>
        /// Masraf plani uygulayan kurumlar için kullanılacak odenek tablosunun adın döndüren yordam
        /// </summary>
        /// <param name="yil">Yil.</param>
        /// <returns></returns>
        //public static ENUMTabloKod TabloKodBul(int yil)
        //{
        //    if (tabloKod != ENUMTabloKod.TANIMSIZ)
        //        return tabloKod;

        //    if (Arac.OdenekGondermeTertipYapi(null, yil) != ENUMOdenekGondermeTur.eko2)
        //    {
        //        tabloKod = ENUMTabloKod.TABPEBOdenek;
        //    }
        //    else
        //        tabloKod = ENUMTabloKod.TABOdenek;

        //    return tabloKod;
        //}

        /// <summary>
        /// Yazılımın çalıştığı kurumun Kurum kodunu Degisken.xml den getiren fonksiyon.
        /// </summary>
        /// <returns></returns>
        public static string VarsayilanKurumKod()
        {
            return TNS.MUH.Arac.DegiskenDegerBul(0, "/KURUMKOD");
        }

        public static System.Collections.ArrayList MuhasebeListesiDoldur(Kullanici kullanan, string kurum, string kistas, bool bosEkle, out string ilkMuhasebe)
        {
            return MuhasebeListesiDoldur(kullanan, kurum, kistas, bosEkle, out ilkMuhasebe, true);
        }

        public static System.Collections.ArrayList MuhasebeListesiDoldur(Kullanici kullanan, string kurum, string kistas, bool bosEkle, out string ilkMuhasebe, bool kullaniciMuhBirim)
        {
            System.Collections.ArrayList liste = new System.Collections.ArrayList();
            ilkMuhasebe = null;
            TNS.MUH.IMUHServis servis = TNS.MUH.Arac.Tanimla();

            ObjectArray mv = servis.KullaniciMuhasebeBirimleri(kullaniciMuhBirim ? kullanan : null, 0, kurum, kistas);
            if (mv.sonuc.islemSonuc)
            {
                foreach (TNS.MUH.MuhasebeBirim m in mv.objeler)
                {
                    string deger = m.muhasebe.ToString() + "-" + m.muhasebeAd;
                    if (ilkMuhasebe == null)
                        ilkMuhasebe = deger;
                    liste.Add(new { MUHASEBEKODAD = deger });
                }
                if (bosEkle && kullanan != null && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                {
                    liste.Add(new { MUHASEBEKODAD = "0" + "-" + "TÜM BİRİMLER" });
                }
            }
            return liste;
        }

        public static System.Collections.ArrayList MuhasebeBirimListesiDoldur(Kullanici kullanan, string kurum, int muhasebeKod, bool bosEkle)
        {
            System.Collections.ArrayList liste = new System.Collections.ArrayList();
            TNS.MUH.IMUHServis servis = TNS.MUH.Arac.Tanimla();

            MuhasebeBirim[] mv = servis.MuhasebeBirimListele(kullanan, muhasebeKod, kurum);

            if (bosEkle)
            {
                liste.Add(new { MUHASEBEKODAD = "0.000" + "-" + "TÜM BİRİMLER" });
            }
            string ilkMuhasebe = null;
            if (mv != null)
            {
                foreach (TNS.MUH.MuhasebeBirim m in mv)
                {
                    string deger = m.birimKod + "." + m.kurum + "-" + m.birimAd;
                    if (ilkMuhasebe == null)
                        ilkMuhasebe = deger;
                    liste.Add(new { MUHASEBEKODAD = deger });
                }
            }
            return liste;
        }

        public static BankaBilgi HesapIBANBul(string hesapKod)
        {
            hesapKod = hesapKod.Replace(".", "");

            BankaBilgi bb = new BankaBilgi();
            string degiskenAd = "/" + hesapKod + "/Banka";
            bb.BankaAd = TNS.MUH.Arac.DegiskenDegerBul(0, degiskenAd);
            if (string.IsNullOrWhiteSpace(bb.BankaAd)) bb.BankaAd = string.Format("{0} hesap kodu için değişken dosyasında {1} değişkenine değer atayınız.", hesapKod, degiskenAd);

            degiskenAd = "/" + hesapKod + "/Sube";
            bb.SubeAd = TNS.MUH.Arac.DegiskenDegerBul(0, degiskenAd);
            if (string.IsNullOrWhiteSpace(bb.SubeAd)) bb.SubeAd = string.Format("{0} hesap kodu için değişken dosyasında {1} değişkenine değer atayınız.", hesapKod, degiskenAd);

            degiskenAd = "/" + hesapKod + "/IBAN";
            bb.IBAN = TNS.MUH.Arac.DegiskenDegerBul(0, degiskenAd);
            if (string.IsNullOrWhiteSpace(bb.IBAN)) bb.IBAN = string.Format("{0} hesap kodu için değişken dosyasında {1} değişkenine değer atayınız.", hesapKod, degiskenAd);

            degiskenAd = "/" + hesapKod + "/SubeNo";
            bb.MBSubeNo = TNS.MUH.Arac.DegiskenDegerBul(0, degiskenAd);
            if (string.IsNullOrWhiteSpace(bb.MBSubeNo)) bb.MBSubeNo = string.Format("SUBENO");

            degiskenAd = "/" + hesapKod + "/ftpAd";
            bb.ftpAd = TNS.MUH.Arac.DegiskenDegerBul(0, degiskenAd);
            if (string.IsNullOrWhiteSpace(bb.ftpAd)) bb.ftpAd = "halkbankftp";

            degiskenAd = "/" + hesapKod + "/MusteriNo";
            bb.musteriNo = TNS.MUH.Arac.DegiskenDegerBul(0, degiskenAd);
            if (string.IsNullOrWhiteSpace(bb.musteriNo)) bb.musteriNo = "MusteriNo";

            degiskenAd = "/" + hesapKod + "/MusteriAd";
            bb.musteriAd = TNS.MUH.Arac.DegiskenDegerBul(0, degiskenAd);
            if (string.IsNullOrWhiteSpace(bb.musteriAd)) bb.musteriAd = "MusteriAd";

            degiskenAd = "/" + hesapKod + "/HesapNo";
            bb.hesapNo = TNS.MUH.Arac.DegiskenDegerBul(0, degiskenAd);
            if (string.IsNullOrWhiteSpace(bb.hesapNo)) bb.hesapNo = "hesapNo";

            return bb;
        }

        public static string IBANBoslukla(string IBAN)
        {
            if (string.IsNullOrWhiteSpace(IBAN)) return "";
            string tut = IBAN.Trim().Replace(" ", "");
            if (tut.Length != 26) return tut;
            IBAN = tut;
            string donDeger = "";
            int uz = IBAN.Length;
            int yer = 0;
            while (true)
            {
                int icUz = 4;
                if (yer + icUz >= uz) icUz = uz - yer;
                if (donDeger != "") donDeger += " ";
                donDeger += IBAN.Substring(yer, icUz);
                yer = yer + icUz;
                if (yer >= uz) break;
            }
            return donDeger;
        }
         
        public static bool TertipTamMi(Tertip tertip, ENUMOdenekKontrolTur okTur, ENUMBelgeTuru belgeTur, string talep, bool eko23)
        {
            for (int i = 0; i < 3; i++)
            {
                if (!tertip.KodTamMi(i)) return false;
            }

            bool ekoTam = true;

            if (!string.IsNullOrEmpty(talep) && okTur == ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP && belgeTur == ENUMBelgeTuru.ODENEKGONDERME)
            {
            }
            else if (okTur == ENUMOdenekKontrolTur.E2OG_E4OE_E2K || okTur == ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)
            {
                ekoTam = false;
            }
            if (ekoTam)
            {
                if (!tertip.KodTamMi(3)) return false;
            }
            else
            {
                int ekoKirSay;
                if (eko23 && TNS.BKD.Arac.Eko23Mu(tertip.yil, tertip.kodlar[3].Replace(".", "")))
                {
                    ekoKirSay = 3;
                }
                else
                    ekoKirSay = 2;
                if (!tertip.KodTamMi(3, ekoKirSay)) return false;
            }

            if (okTur == ENUMOdenekKontrolTur.E2OG_E4OE_E2K)
            {
                string peb = TNS.MUH.Arac.DiziElemanAl(tertip.kodlar, 4, "");
                if (!string.IsNullOrEmpty(peb)) return false;
            }
            else
                if (!tertip.KodTamMi(4)) return false;
            return true;
        }

        public static ENUMOdenekGondermeTur OdenekGondermeTertipYapi(int yil)
        {
            ENUMOdenekKontrolTur tur = OdenekKontrolTurBul(yil);
            if (tur == ENUMOdenekKontrolTur.E2OG_E4OE_E2K) return ENUMOdenekGondermeTur.eko2;
            else if (tur == ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP) return ENUMOdenekGondermeTur.eko2peb;
            else return ENUMOdenekGondermeTur.eko4peb;
        }

        public static ENUMOdenekKontrolTur OdenekKontrolTurBul(int yil)
        {
            int tut;
            if (yil <= 2010) tut = 0;
            else tut = OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DegiskenDegerBul(yil, "/ODENEKGONDERMEPEB"), 0);

            if (tut == 0) return ENUMOdenekKontrolTur.E2OG_E4OE_E2K;//Standart
            else if (tut == 1) return ENUMOdenekKontrolTur.E4POG_E4POE_E2PK;//Tarım İlk halin çevrilmişi 
            else if (tut == 2) return ENUMOdenekKontrolTur.E4POG_E4POE_E2K; //Maliye Bakanlığı, Manas
            else if (tut == 3) return ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP; //Tarım son hal
            else if (tut == 4) return ENUMOdenekKontrolTur.E4POG_E4POE_E4PK; //Ulusal Ajans
            else return ENUMOdenekKontrolTur.E2OG_E4OE_E2K;
        }

        public static bool IzBedelGerekliBul(int yil)
        {
            int tut;
            if (yil <= 2010) tut = 0;
            else tut = OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DegiskenDegerBul(yil, "/IZBEDELGEREKLI"), 0);
            return tut == 1;
        }

        public static bool OdenekGOndermeSadeceEko2Mi(int yil)
        {
            ENUMOdenekKontrolTur tur = OdenekKontrolTurBul(yil);
            if (tur == ENUMOdenekKontrolTur.E2OG_E4OE_E2K) return true;
            return false;
        }

        public static bool OdenekGondermeEko2liMi(int yil)
        {
            ENUMOdenekKontrolTur tur = OdenekKontrolTurBul(yil);
            if (tur == ENUMOdenekKontrolTur.E2OG_E4OE_E2K || tur == ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP) return true;
            return false;
        }

        private static string prgVar = null;
        public static bool prgBag
        {
            get
            {
                if (prgVar == null)
                    prgVar = TNS.DEG.Arac.DegiskenDegerBul(0, "PRGVAR");
                return prgVar == "1";
            }
        }


        public static bool OdemeEmriPEBMi(int yil)
        {
            ENUMOdenekKontrolTur tur = OdenekKontrolTurBul(yil);
            if (tur != ENUMOdenekKontrolTur.E2OG_E4OE_E2K) return true;
            return false;
        }

        public static bool DortDuzeyDegisimOlacakMi(ENUMOdenekKontrolTur ogKontrolTertipTur)
        {
            if (ogKontrolTertipTur == ENUMOdenekKontrolTur.E4POG_E4POE_E2PK) return true;
            else return false;
        }

        public static string YuzdeOnKodBul(ENUMYuzdeOnTur MalHizmetYapim)
        {
            if (MalHizmetYapim == ENUMYuzdeOnTur.Mal) return "Mal";
            if (MalHizmetYapim == ENUMYuzdeOnTur.Hizmet) return "Hizmet";
            if (MalHizmetYapim == ENUMYuzdeOnTur.Yapim) return "Yapım";
            return "";
        }

        public static string BelgenolarStrYap(OdemeEmriMIFForm[] odemeEmriMIFForm, int yil, bool kurumKoduKoyma)
        {
            string retVal = "";
            if (odemeEmriMIFForm != null)
            {
                for (int i = 0; i < odemeEmriMIFForm.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(odemeEmriMIFForm[i].belgeNo))
                    {
                        string satirStr;
                        if (yil != odemeEmriMIFForm[i].yil) satirStr = odemeEmriMIFForm[i].yil.ToString() + "-";
                        else satirStr = "";
                        string formNo = odemeEmriMIFForm[i].belgeNo.Substring(4);
                        int ilkYer = -1;
                        for (int j = 0; j < formNo.Length; j++)
                        {
                            if (formNo[j] != '0') { ilkYer = j; break; }
                        }
                        if (ilkYer > 0)
                        {
                            formNo = formNo.Substring(ilkYer);
                        }
                        if (retVal != "") retVal += ";";
                        retVal += satirStr + formNo;
                    }
                }
            }
            return retVal;
        }

        public static string[] YuzdeOnAlimTurBul()
        {
            string[] maddeler = { "21f", "22d" };
            if (VarsayilanKurumKod() == "25.75" || VarsayilanKurumKod() == "11.75")
                maddeler = new string[] { "14f", "15d" };
            return maddeler;
        }

        public static int YuzdeOnIhaleKodYerBul(string[] yuzdeOnIhaleKodlar, string ihaleKod)
        {
            for (int i = 0; i < yuzdeOnIhaleKodlar.Length; i++)
            {
                if (yuzdeOnIhaleKodlar[i] == ihaleKod.Replace(".", "")) return i;
            }
            return yuzdeOnIhaleKodlar.Length;
        }

        public static string YuzdeOnAlimTurBul(string yuzdeOnKod)
        {
            string[] maddeler = YuzdeOnAlimTurBul();

            string donDeger = "";
            for (int i = 0; i < maddeler.Length; i++)
            {
                if (i > 0) donDeger += ",";
                donDeger += "'" + maddeler[i] + yuzdeOnKod + "'";
            }
            return donDeger;
        }

        public static string DiziElemanAl(string[] dizi, int elamanNo, string hata)
        {
            if (elamanNo < dizi.Length)
            {
                string tut = dizi[elamanNo];
                if (tut == null) tut = hata;
                return tut;
            }
            return hata;
        }

        public static ENUMOdenekUstuKontrol OdenekUstuKontrolBul()
        {
            string odenekUstuKontrol = System.Configuration.ConfigurationManager.AppSettings.Get("OdenekUstuHarcamaKontrol") + "";
            if (odenekUstuKontrol.ToUpper() == "ODENEK")// veya ODENEKGONDERME
                return ENUMOdenekUstuKontrol.ODENEK;
            else return ENUMOdenekUstuKontrol.ODENEKGONDERME;
        }

        public static string FormDurumAciklama(int durum, ENUMBelgeTuru belgeTur, string yeniBelgeNo = null)
        {
            string islem = "";

            if (durum == (int)ENUMBelgeDurumu.YENI) islem = "Yeni";
            else if (durum == (int)ENUMBelgeDurumu.DEGISTIRILDI) islem = "Değiştirildi";
            else if (durum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER)
            {
                if (belgeTur == ENUMBelgeTuru.ODEMEEMRI || belgeTur == ENUMBelgeTuru.MIF)
                    islem = "Yevmiye No Verildi";
                else
                    islem = "İcmal No Verildi";
            }
            else if (durum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI) islem = "Muhasebata Gönderildi";
            else if (durum == (int)ENUMBelgeDurumu.MUHASEBATALDI) islem = "Muhasebat Aldı";
            else if (durum == (int)ENUMBelgeDurumu.ONAYLI) islem = "Onaylı";
            else if (durum == (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
            {
                if (belgeTur == ENUMBelgeTuru.ODENEKGONDERME || belgeTur == ENUMBelgeTuru.TENKIS)
                    islem = "SGB'ye Gönderildi";
                else
                    islem = "Yevmiye No Verildi";
            }
            else if (durum == (int)ENUMBelgeDurumu.MUHASEBATIADE) islem = "Muhasebat İade Etti";
            else if (durum == (int)ENUMBelgeDurumu.IPTAL) islem = "İptal Edildi";
            else if (durum == (int)ENUMBelgeDurumu.ONAYKALDIR) islem = "Onay Kaldırıldı";
            else if (durum == (int)ENUMBelgeDurumu.YEVMIYEKALDIR) islem = "Yevmiye No Kaldırıldı";
            else if (durum == (int)ENUMBelgeDurumu.TAHSIS) islem = "Tahsis Edildi";
            else if (durum == (int)ENUMBelgeDurumu.YENITALEP) islem = "Talep Hazırlandı";
            else if (durum == (int)ENUMBelgeDurumu.TALEP) islem = "Talep Edildi";
            else if (durum == (int)ENUMBelgeDurumu.DEGISIMONAYLA) islem = "Değişim Formu Onaylandı";
            else if (durum == (int)ENUMBelgeDurumu.TALEPKARSILA) islem = "Talep Karşılandı (Yeni Belge:" + yeniBelgeNo + ")";
            else if (durum == (int)ENUMBelgeDurumu.BELGETURDEGISTIR) islem = "Belge Tür Değiştirildi";
            else if (durum == (int)ENUMBelgeDurumu.CEKNOVER) islem = "Çek/Gönd. No Verildi";
            else
                islem = "Bilinmiyor (" + durum + ")";

            return islem;
        }

        public static int EkonomikIkiDuzeyKontrol(bool ego, string hesapKod)
        {
            bool ikiDuzeyCalis = OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEIKIDUZEYEKOKODCALIS"), 0) > 0;
            if (!ikiDuzeyCalis && !ego) return 0;
            hesapKod = hesapKod.Replace(".", "");
            int ikinciKod = 0;
            if (hesapKod.Length >= 5)
                ikinciKod = OrtakFonksiyonlar.ConvertToInt(hesapKod.Substring(3, 2), 0);

            if (hesapKod.StartsWith("16"))
                return 2;
            else if (hesapKod.StartsWith("15") ||
                hesapKod.StartsWith("25") ||
                (hesapKod.StartsWith("32") && !hesapKod.StartsWith("323")) ||
                hesapKod.StartsWith("90") ||
                hesapKod.StartsWith("26"))
                return 2;
            else if (ikinciKod > 9)
            {
                return 2;
            }
            return 0;
        }

        public static bool MuhasebeIsKolonuGizle()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEISKOLONUGIZLE"), 0) > 0 ? true : false;
        }

        public static bool MuhasebeMahsupKolonuGizle()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEMAHSUPKOLONUGIZLE"), 0) > 0 ? true : false;
        }

        public static bool MuhasebeSatirAciklamaVarMi()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBESATIRACIKLAMAVAR"), 0) > 0 ? true : false;
        }
        public static bool MuhasebeDetayHesapVarMi()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEDETAYHESAPVAR"), 0) > 0 ? true : false;
        }
        public static bool OfmEntegrasyonVarMi()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/OFMENTEGRASYON"), 0) > 0 ? true : false;
        }
        public static bool MuhasebeDovizVarMi()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEDOVIZVAR"), 0) > 0 ? true : false;
        }
        public static bool MuhasebeTertipYokMu()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBUTCEYOK"), 0) > 0 ? true : false;
        }
        public static string MuhasebeKurumIBANTL()
        {
            return OrtakFonksiyonlar.ConvertToStr(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEKURUMIBANTL"));
        }
        public static string MuhasebeKurumIBANEURO()
        {
            return OrtakFonksiyonlar.ConvertToStr(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEKURUMIBANEURO"));
        }
        public static string MuhasebeKurumBankaAdi()
        {
            return OrtakFonksiyonlar.ConvertToStr(TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEKURUMBANKAADI"));
        }

        public static bool DetayBirimVarMi()
        {
            return TNS.MUH.Arac.DegiskenDegerBul(0, "/DETAYBIRIM") == "1";
        }

        public static bool AltHesapGosterme()
        {
            return TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/ALTHESAPKODGOSTERME") == "1";
        }

        public static string yeniZorunluMu = null;
        public static bool YeniZorunluMu()
        {
            if (yeniZorunluMu == null)
            {
                yeniZorunluMu = TNS.MUH.Arac.DegiskenDegerBul(0, "/YENIZORUNLUMU");
            }
            return yeniZorunluMu == "1";
        }

        public static string IhaleKodBul(string ihale)
        {
            if (string.IsNullOrWhiteSpace(ihale)) return "";
            int kes = 0;
            if (ihale.EndsWith("Mal")) kes = 3;
            else if (ihale.EndsWith("Hizmet")) kes = 6;
            else if (ihale.EndsWith("Yapım")) kes = 5;
            if (kes > 0)
                return ihale.Substring(0, ihale.Length - kes);
            return ihale;
        }

        public static string IhaleAlimturBul(string ihale)
        {
            if (ihale.EndsWith("Mal")) return "Mal";
            else if (ihale.EndsWith("Hizmet")) return "Hizmet";
            else if (ihale.EndsWith("Yapım")) return "Yapım";
            return "";
        }

        public static bool GelismisIcmalKontrol(int yil)
        {
            return TNS.MUH.Arac.DegiskenDegerBul(yil, "/GelismisIcmalKontrol") == "1";
        }

        private static string muhasebatTur = null;
        public static bool MuhasebatYok()
        {
            if (muhasebatTur == null)
                muhasebatTur = TNS.KYM.Arac.ServerWebConfigOku("MuhasebatTur", "") + "";
            return !string.IsNullOrEmpty(muhasebatTur);
        }

        public static bool OdenekGondermeYokMu(int yil)
        {
            string kurumKod = TNS.DEG.Arac.VarsayilanKurumBul().Replace(".", "");
            if (TNS.DEG.Arac.TKDKMi() || kurumKod == "460604") return true;

            return TNS.MUH.Arac.DegiskenDegerBul(yil, "/OdenekGondermeKontrol") == "1";
        }

        public static string ProgramTertipHazirla(HesapBilgi detay, Hashtable htPrgIliski)
        {
            //Prog.Altprog.Faal.Altfaal.kkod1.kkod2.Finkod.Ekod1.Ekod2
            if (string.IsNullOrEmpty(detay.peb))
                detay.peb = "PEBYOK";

            string tertip = "";
            try
            {
                tertip = htPrgIliski[detay.peb.Replace(".", "")] + "";
                string kkod1 = detay.kur.Replace(".", "").Substring(0, 2);
                string kkod2 = detay.kur.Replace(".", "").Substring(2, 2);
                string ekod1 = detay.eko.Replace(".", "").Substring(0, 2);
                string ekod2 = detay.eko.Replace(".", "").Substring(2, 2);

                tertip += "." + OrtakFonksiyonlar.ConvertToInt(kkod1, 0);
                tertip += "." + OrtakFonksiyonlar.ConvertToInt(kkod2, 0);
                tertip += "." + OrtakFonksiyonlar.ConvertToInt(detay.fin, 0);
                tertip += "." + OrtakFonksiyonlar.ConvertToInt(ekod1, 0);
                tertip += "." + OrtakFonksiyonlar.ConvertToInt(ekod2, 0);
            }
            catch
            {
                tertip = "";
            }

            return tertip;
        }

    }

    [Serializable]
    public class IlgiliBilgi
    {
        private string _ilgili;
        public string ilgili { get { return _ilgili; } set { _ilgili = value; } }

        private string _ilgBanka;
        public string ilgBanka { get { return _ilgBanka; } set { _ilgBanka = value; } }

        private string _ilgIBAN;
        public string ilgIBAN { get { return _ilgIBAN; } set { _ilgIBAN = value; } }

        private string _bicSwift;
        public string bicSwift { get { return _bicSwift; } set { _bicSwift = value; } }

        public string vergiDairesi { get; set; }
    }

    /// <summary>
    /// Okuma, yazma gibi yetki kontrollerini yapan statik fonksiyonları içeren sınıf
    /// </summary>
    [Serializable()]
    public static class Yetki
    {
        /// <summary>
        /// Makbuz modülünü kullanma yetkisini kontrol eden metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns></returns>
        public static bool MakbuzKullanicisimi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MAKBUZKULLANICISI))
                return true;
            else
                return false;
        }

        public static bool VeznedarMi(Kullanici kullanan)
        {
            string yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Veznedar");
            yetkiTipiYetkiDegeri = yetkiTipiYetkiDegeri.Replace(";", "");
            int yetkiTipiYetkiDegeriInt = OrtakFonksiyonlar.ConvertToInt(yetkiTipiYetkiDegeri, -1);
            if (yetkiTipiYetkiDegeriInt >= 100) yetkiTipiYetkiDegeriInt = 0;

            return yetkiTipiYetkiDegeriInt > 0;
        }

        public static bool VeznedarMakbuzHazirlayan(Kullanici kullanan)
        {
            string yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Veznedar");
            yetkiTipiYetkiDegeri = yetkiTipiYetkiDegeri.Replace(";", "");
            int yetkiTipiYetkiDegeriInt = OrtakFonksiyonlar.ConvertToInt(yetkiTipiYetkiDegeri, -1);

            return yetkiTipiYetkiDegeriInt == 100;
        }

        public static bool MuhasebeBirimYetkiVarMi(Kullanici kullanan, string muhasebe, string birim, bool gorme = false)
        {
            int muhasebeInt = OrtakFonksiyonlar.ConvertToInt(muhasebe, 0);
            if (kullanan.ad == "aktarim")//Sistem dışı aktarımların yapıldığı kullanıcı
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.ODENEKGONDERME))
            {
                return true;
            }

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MUHASEBEYONETICISI))
            {
                return true;
            }

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
            {
                if (OrtakFonksiyonlar.ConvertToInt(kullanan.calistigiKurum, 0) == muhasebeInt && gorme)
                    return true;
                //else if (!gorme)
                //    return false;
            }

            return SaymanliBirimYetkiVarMi(kullanan, muhasebeInt, birim, gorme, "Ödeme Emri/MİF Saymanlık-Birim Yetkisi");
        }

        public static bool SaymanliBirimYetkiVarMi(Kullanici kullanan, int muhasebeInt, string birim, bool gorme, string yetkiTipiAdi)
        {
            string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

            if (yetkiTipiDegeri.Trim() == "")
                return true;

            string[] d = yetkiTipiDegeri.Split(';');

            for (int i = 0; i < d.Length; i++)
            {
                string[] muhasebeBirim = d[i].Split('-');

                if (muhasebeBirim.Length != 2)
                    return false;
                int yetkiMuhasebeInt = OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[0], 0);
                if (yetkiMuhasebeInt == muhasebeInt || muhasebeBirim[0] == "*" || (gorme && IlceyiGorurMu(yetkiMuhasebeInt, muhasebeInt)))
                {
                    if (OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[1], 0) == OrtakFonksiyonlar.ConvertToInt(birim, 0) || muhasebeBirim[1] == "*")
                        return true;
                    if (gorme && muhasebeBirim[1] == "260" && birim == "250")
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Kullanicis the muhasebe birimi getir.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <returns></returns>
        public static string KullaniciMuhasebeBirimiGetir(Kullanici kullanan)
        {
            string muhasebeler = "";
            string birimler = "";

            string kurumlar = OdemeEmriMIFKurumlariVer(kullanan);
            string muhasebeBirimler = OdemeEmriMIFMuhasebeBirimVer(kullanan);
            string[] birim = muhasebeBirimler.Split(';');
            for (int kk = 0; kk < birim.Length; kk++)
            {
                string[] mb = birim[kk].Split('-');
                if (muhasebeler != "") muhasebeler += ";";
                muhasebeler += mb[0];
                if (mb.Length > 1 && OrtakFonksiyonlar.ConvertToInt(mb[1], -1) >= 0)
                {
                    if (birimler != "") birimler += ";";
                    birimler += mb[1];
                }
            }
            return birimler;
        }

        /// <summary>
        /// Ödeme Emri/MİF yetkili olduğu birimlşerin listesini verir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns></returns>
        public static string OdemeEmriMIFKurumlariVer(Kullanici kullanan)
        {
            string yetkiTipiAdi = "";

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                return VarsayilanKurumBul();
            else
            {
                yetkiTipiAdi = "Ödeme Emri/MİF Hazırlayacağı Birimler";
                return OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);
            }
        }

        /// <summary>
        /// Yetkili olduğu Muhasebe birim listesini verir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns></returns>
        public static string OdemeEmriMIFMuhasebeBirimVer(Kullanici kullanan)
        {
            string yetkiTipiAdi = "";

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                return kullanan.calistigiKurum;
            else
            {
                yetkiTipiAdi = "Ödeme Emri/MİF Saymanlık-Birim Yetkisi";
                return OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);
            }
        }

        private static bool IlceyiGorurMu(int yetkiMuhasebeInt, int muhasebeInt)
        {
            int ilBirimKod = yetkiMuhasebeInt % 1000;
            if (ilBirimKod == 100 || ilBirimKod == 150)
            {
                return yetkiMuhasebeInt / 100 == yetkiMuhasebeInt / 100;
            }
            return false;
        }

        /// <summary>
        /// Varsayılan kurum kodu bilgisini DEGServis'e sorar ve döndürür
        /// </summary>
        /// <returns>Varsayılan kurum kodu bilgisini döndürür</returns>
        public static string VarsayilanKurumBul()
        {
            return TNS.MUH.Arac.DegiskenDegerBul(0, "/KURUMKOD");
        }

        /// <summary>
        /// Varsayılan kurum adı bilgisini DEGServis'e sorar ve döndürür
        /// </summary>
        /// <returns>Varsayılan kurum kodu bilgisini döndürür</returns>
        public static string VarsayilanKurumBulAd()
        {
            return TNS.MUH.Arac.DegiskenDegerBul(0, "/KURUMAD");
        }

        /// <summary>
        /// Kullanıcının Muhasebe Yöneticisi yetkisini kontrol eden metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns></returns>
        public static bool MuhasebeYoneticisimi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MUHASEBEYONETICISI))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Kullanıcının Sistem Yöneticisi yetkisini kontrol eden metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns></returns>
        public static bool SistemYoneticisimi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MUHASEBEYONETICISI))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Kullanıcının belirtilen belge türüne göre okuma/görme/yazma yetkisini kontrol eden metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="ogy">Okuma/Görme/Yazma türü</param>
        /// <param name="belgeTur">Belge Türü</param>
        /// <returns></returns>
        public static bool YetkisiVarMi(Kullanici kullanan, int ogy, int belgeTur)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)
                || kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI)
                || kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MUHASEBEYONETICISI))
                return true;

            string yetkiTipiYetkiDegeri = "";

            if (belgeTur == (int)ENUMBelgeTuru.ODEMEEMRI || belgeTur == (int)ENUMBelgeTuru.MIF)
            {
                if (ogy == (int)ENUMOGY.Gorme)
                {
                    if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.ODENEKGONDERME))
                        return true;
                }
                yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Ödeme Emri/MİF Yetkisi");
            }
            else if (belgeTur == (int)ENUMBelgeTuru.ODENEKGONDERME || belgeTur == (int)ENUMBelgeTuru.TENKIS)
                yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Ödenek Gönderme Yetkisi");

            string[] yetkiTipiYetkiDegerleri = yetkiTipiYetkiDegeri.Split(';');
            foreach (string item in yetkiTipiYetkiDegerleri)
            {
                if (string.IsNullOrWhiteSpace(item)) continue;
                int yetkiTipiYetkiDegeriInt = OrtakFonksiyonlar.ConvertToInt(item.Trim(), -1);

                if (ogy == (int)ENUMOGY.Gorme && yetkiTipiYetkiDegeriInt > 0)
                    return true;
                if (ogy == (int)ENUMOGY.Yazma && yetkiTipiYetkiDegeriInt > 1)
                    return true;
                if (ogy == (int)ENUMOGY.Onaylama && yetkiTipiYetkiDegeriInt > 2)
                    return true;
                if (ogy == (int)ENUMOGY.OnayKaldir && yetkiTipiYetkiDegeriInt >= 5)
                    return true;
                if (ogy == (int)ENUMOGY.YevmiyeKaldir && yetkiTipiYetkiDegeriInt == 6)
                    return true;
                if (ogy == (int)ENUMOGY.MuhasebatGonder && yetkiTipiYetkiDegeriInt > 2)
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Yetki bilgilerini tutan enumeration
    /// </summary>
    public enum ENUMOGY
    {
        /// <summary>
        /// Onaylama
        /// </summary>
        Onaylama = 1,
        /// <summary>
        /// Görme
        /// </summary>
        Gorme = 2,
        /// <summary>
        /// Yazma
        /// </summary>
        Yazma = 3,
        /// <summary>
        /// Yevmiye Kaldırma
        /// </summary>
        YevmiyeKaldir = 4,
        /// <summary>
        /// Onay Kaldırma
        /// </summary>
        OnayKaldir = 5,
        /// <summary>
        /// Muhasebata Gönder
        /// </summary>
        MuhasebatGonder = 6,
    }

    /// <summary>
    /// Hesap Planı Yetki bilgilerini tutan enumeration
    /// </summary>
    public enum ENUMHesapPlaniYetki
    {
        /// <summary>
        /// Görme 
        /// </summary>
        Gorme = 1,
        /// <summary>
        /// Yazma
        /// </summary>
        Yazma = 2,
        /// <summary>
        /// Silme
        /// </summary>
        Silme = 3,
    }

    /// <summary>
    /// Fiş Türü bilgilerini tutan enumeration
    /// </summary>
    public enum ENUMFISTUR : int
    {
        /// <summary>
        /// Dönem için girilen fişler
        /// </summary>
        DONEMICI = 0,

        /// <summary>
        /// Açılış fişi
        /// </summary>
        ACILIS = 1,

        /// <summary>
        /// Kapanış fişi
        /// </summary>
        KAPANIS = 2,

        /// <summary>
        /// Amortisman Kayıtlarını Kapatan Fiş.
        /// </summary>
        AMORTISMANKAPANISI = 3,

        /// <summary>
        /// Son Fiş
        /// </summary>
        SONFIS = 9,
    }

    /// <summary>
    /// Belge Durumu bilgilerini tutan enumeration
    /// </summary>
    public enum ENUMBelgeDurumu
    {
        /// <summary>
        /// Yeni
        /// </summary>
        YENI = 1,
        /// <summary>
        /// Değiştirildi
        /// </summary>
        DEGISTIRILDI = 2,
        /// <summary>
        /// Muhasebata Gönderildi
        /// </summary>
        MUHASEBATAGONDERILDI = 3,
        /// <summary>
        /// Muhasebat Aldı
        /// </summary>
        MUHASEBATALDI = 4,
        /// <summary>
        /// Onaylı
        /// </summary>
        ONAYLI = 5,

        /// <summary>
        /// Ödeme Emri için Yevmiye, Ödenek gönderme için SGBYe Gönder durumu (Can ekledi)
        /// </summary>
        YEVMIYE_OESGBYEGONDER = 6,


        /// <summary>
        /// İcmal No/Yevmiye No verme 
        /// </summary>
        ICMALYEVMIYENOVER = 7,
        /// <summary>
        /// Muhasebat İade
        /// </summary>
        MUHASEBATIADE = 8,
        /// <summary>
        /// İptal
        /// </summary>
        IPTAL = 9,
        /// <summary>
        /// Onay Kaldır
        /// </summary>
        ONAYKALDIR = 10,
        /// <summary>
        /// Yevmiye kaldır
        /// </summary>
        YEVMIYEKALDIR = 11,
        /// <summary>
        /// Tahsis
        /// </summary>
        TAHSIS = 12,
        /// <summary>
        /// Belge Tür değiştir
        /// </summary>
        BELGETURDEGISTIR = 13,
        /// <summary>
        /// Belge Durumunu Yeni yap
        /// </summary>
        BELGEDURUMUYENIYAP = 14,
        /// <summary>
        /// Tahsis ve Onaylı
        /// </summary>
        TAHSISVEONAYLI = 17,
        /// <summary>
        /// Çek No verme 
        /// </summary>
        CEKNOVER = 18,

        /// <summary>
        /// Bu Talep karşılandı ve karşılığında bir belge üretildi.
        /// </summary>
        TALEPKARSILA = 96,

        /// <summary>
        /// Talebi yeni belge yap.
        /// </summary>
        TALEPBELGEYAP = 97,

        /// <summary>
        /// Değişim Onayla
        /// </summary>
        DEGISIMONAYLA = 98,

        /// <summary>
        /// Talep
        /// </summary>
        YENI_ONAYLITALEP = 89,

        /// <summary>
        /// Talep
        /// </summary>
        YENITALEP = 90,

        /// <summary>
        /// Talep
        /// </summary>
        KARSILANANTALEP = 96,

        /// <summary>
        /// Talep
        /// </summary>
        TALEP = 99,

        /// <summary>
        /// Talep
        /// </summary>
        TALEPIPTAL = 100,

        /// <summary>
        /// İşlemde
        /// </summary>
        ISLEMDE = 101,

        /// <summary>
        /// Verilen işlem numarasından başka süreçlerde işlem bekleyen ödenek gönderme belgesi var mı?
        /// </summary>
        DIGERSURECISLEMBELGELER = 102,

        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = -1

        //Yevmiyesiz-23
        //Bütün Belgeler-22
        //Yevmiyeli ve onaylı-24
        //Muhasebata Gönderilecekler-21
        //İcmal No Verilmemişler-20
    }

    public enum ENUMSaymanlikTasiIslem : int
    {
        ODEMEEMRI = 1,
        PERSONEL = 2,
        ABONELIKLER = 3,
        BIRIMBUTCETEKLIFI = 4,
        BIRIMHIZMETMALIYETI = 5,
        DEPREMHARCAMALARI = 6,
        ARACKIRALAMAPLANI = 7,
        SURECTANIMLARI = 8,
        DOGRUDANTEMIN = 9,
        SEYYARGOREVPLANLAMA = 10,
        MEVCUTTASITLAR = 11,
        CESITLIODEMELER = 12,
        ODENEKGONDERME = 13,
        MASRAFPLANI = 14,
        MASRAFPLANITAVAN = 15,
    }


    /// <summary>
    /// Belge türü bilgilerini tutan enumeration
    /// </summary>
    public enum ENUMBelgeTuru
    {
        /// <summary>
        /// Ödeme Emri
        /// </summary>
        ODEMEEMRI = 1,
        /// <summary>
        /// Muhasebe İşlem Fişi
        /// </summary>
        MIF = 2,
        /// <summary>
        /// Ödenek Gönderme
        /// </summary>
        ODENEKGONDERME = 3,
        /// <summary>
        /// Tenkis
        /// </summary>
        TENKIS = 4,

        ENTEGRSYON_BELGE = 999,

        /// <summary>
        /// Ödeme Emri Yüksek
        /// </summary>
        ODEMEEMRI_YUKSEK = 11,
        /// <summary>
        /// Muhasebe İşlem Fişi Yüksek
        /// </summary>
        MIF_YUKSEK = 12,
        /// <summary>
        /// Ödenek Gönderme Yüksek
        /// </summary>
        ODENEKGONDERME_YUKSEK = 13,
        /// <summary>
        /// Tenkis Yüksek
        /// </summary>
        TENKIS_YUKSEK = 14,

        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = -1
    }

    /// <summary>
    /// Onay Durumu bilgilerini tutan enumeration
    /// </summary>
    public enum ENUMOnayDurumu
    {
        /// <summary>
        /// Onaylı
        /// </summary>
        ONAYLI = 1,
        /// <summary>
        /// Onaysız
        /// </summary>
        ONAYSIZ = 2,
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = -1
    }

    /// <summary>
    /// Hesap Kodu Nitelik bilgilerini tutan enumeration
    /// </summary>
    public enum ENUMHesapKodNitelik
    {
        /// <summary>
        /// Gider bütçesi hesabı
        /// </summary>
        GIDERBUTCESI = 1,
        /// <summary>
        /// Kasa Hesabı
        /// </summary>
        KASAHESABI = 2,
        /// <summary>
        /// Özel Gider hesabı
        /// </summary>
        OZELGIDER = 3,
        /// <summary>
        /// Emanet hesabı
        /// </summary>
        EMANET = 4,
        /// <summary>
        /// Avans hesabı
        /// </summary>
        AVANS = 5,
        /// <summary>
        /// Gelir bütçesi hesabı
        /// </summary>
        GELIRBUTCESI = 6,
        /// <summary>
        /// Gider yansıtma hesabı
        /// </summary>
        GIDERYANSITMA = 8,
        /// <summary>
        /// Gelir yansıtma hesabı
        /// </summary>
        GELIRYANSITMA = 9,
        /// <summary>
        /// Yansıtma yapılacak hesaplar
        /// </summary>
        YANSITMAYAPILACAK = 10,
        /// <summary>
        /// KDV hesabı
        /// </summary>
        KDV = 11,
        /// <summary>
        /// YANSITMAZORUNLUBORC hesabı
        /// </summary>
        YANSITMAZORUNLUBORC = 12,
        /// <summary>
        /// Mahsup
        /// </summary>
        MAHSUP = 13,
        /// <summary>
        /// Pasif
        /// </summary>
        PASIF = 14,
        /// <summary>
        /// YANSITMAZORUNLUBORC hesabı
        /// </summary>
        YANSITMAZORUNLUALACAK = 15,
        /// <summary>
        /// Saymanlık bazda bakiye kontrolü yap
        /// </summary>
        SAYMANLIKBAZDABAKIYE = 16,
        /// <summary>
        /// MIF Kontrolünde avans kontrolü ilgili hesap kodu için yapılmasın
        /// </summary>
        AVANSKONTROLUYAPMA = 17,
        /// <summary>
        /// Bazı hesaplarda birim 000 kabul edilmesin
        /// </summary>
        BIRIMZORUNLU = 18
    }

    /// <summary>
    /// Kesin Hesap Bütçe (Rapor) bilgilerini tutan enumeration
    /// </summary>
    public enum ENUMKesinHesapButce
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Kurumsal rapor
        /// </summary>
        KURUMSAL = 1,
        /// <summary>
        /// Fonksiyonel rapor
        /// </summary>
        FONKSIYONEL = 2,
        /// <summary>
        /// Finans rapor
        /// </summary>
        FINANS = 3,
        /// <summary>
        /// Ekonomik rapor
        /// </summary>
        EKONOMIK = 4,
        /// <summary>
        /// Gelir ekonomik rapor
        /// </summary>
        GELIREKONOMIK = 5,
        /// <summary>
        /// Bilanço raporu
        /// </summary>
        BILONCO = 6,
        /// <summary>
        /// Gider ekonomik rapor
        /// </summary>
        GIDEREKONOMIK = 7,
        /// <summary>
        /// Ekonomik 2. kırılıma göre rapor
        /// </summary>
        EKONOMIK2 = 8,
        /// <summary>
        /// Fin, Eko 2. kırılım ve PEBe göre rapor
        /// </summary>
        FINEKO2PEB = 9,
    }

    /// <summary>
    /// Ödenek ihtiyaç aşama bilgilerini tutan enumeration
    /// </summary>
    public enum ENUMOdenekIhtiyacAsama
    {
        /// <summary>
        /// Aşama1
        /// </summary>
        ASAMA1 = 1,
        /// <summary>
        /// Aşama2
        /// </summary>
        ASAMA2 = 2,
    }

    /// <summary>
    /// Talep form durum bilgilerini tutan enumeration
    /// </summary>
    public enum ENUMTalepFormDurum
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Yeni
        /// </summary>
        YENI = 1,
        /// <summary>
        /// Değiştirildi
        /// </summary>
        DEGISTIRILDI = 2,
        /// <summary>
        /// Merkeze gönderildi
        /// </summary>
        MERKEZEGONDERILDI = 3,
        /// <summary>
        /// İşleme alındı
        /// </summary>
        ISLEMEALINDI = 4,
        /// <summary>
        /// Onaylandı
        /// </summary>
        ONAYLANDI = 5,
        /// <summary>
        /// İptal
        /// </summary>
        IPTAL = 9,
    }

    public enum ENUMTKDKTalepDurum
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Yeni
        /// </summary>
        YENI = 1,
        /// <summary>
        /// İşleme alındı
        /// </summary>
        ISLEMEALINDI = 2,
        /// <summary>
        /// Onaylandı
        /// </summary>
        TAMAMLANDI = 3,
        /// <summary>
        /// İptal
        /// </summary>
        REDDEDILDI = 9,
    }

    /// <summary>
    /// Taahhut Tarihçe durum bilgileri tutan enumeration
    /// </summary>
    public enum ENUMTaahhutTarihceDurum
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        [Description("Tanımsız")]
        TANIMSIZ = 0,
        /// <summary>
        /// Yeni
        /// </summary>
        [Description("Yeni")]
        YENI = 1,
        /// <summary>
        /// Güncelleme
        /// </summary>
        [Description("Güncelleme")]
        GUNCELLEME = 2,
        /// <summary>
        /// Tahakkuk Oluşturuldu
        /// </summary>
        [Description("Tahakkuk Oluşturuldu")]
        TAHAKKUKOLUSTURULDU = 3,
        /// <summary>
        /// Kapandı
        /// </summary>
        [Description("Kapandı")]
        KAPANDI = 4,
    }

    /// <summary>
    /// Ödeme Emri/MİF Detay bilgilerini tutan nesne
    /// </summary>
    [Serializable()]
    public class OdemeEmriMIFDetay
    {
        /// <summary>
        /// Kesinti
        /// </summary>
        public int kesinti = 0;
        /// <summary>
        /// Hesap Kodu
        /// </summary>
        public string hesapKod = "";
        /// <summary>
        /// 
        /// </summary>
        public string kur = "";
        /// <summary>
        /// 
        /// </summary>
        public string fon = "";
        /// <summary>
        /// 
        /// </summary>
        public string fin = "";
        /// <summary>
        /// 
        /// </summary>
        public string eko = "";
        /// <summary>
        /// 
        /// </summary>
        public string peb = "";
        /// <summary>
        /// Borç
        /// </summary>
        public double borc = 0;
        public double borcYPB { get { if (this.doviz != "") return this.dovizBorc; else return this.borc; } set { if (this.doviz != "") this.dovizBorc = value; else this.borc = value; } }
        /// <summary>
        /// Alacak
        /// </summary>
        public double alacak = 0;
        public double alacakYPB { get { if (this.doviz != "") return this.dovizAlacak; else return this.alacak; } set { if (this.doviz != "") this.dovizAlacak = value; else this.alacak = value; } }
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama = "";
        /// <summary>
        /// Hesap Adı
        /// </summary>
        public string hesapAd = "";
        /// <summary>
        /// Belge No
        /// </summary>
        public string belgeNo = "";
        /// <summary>
        /// Belge Tür
        /// </summary>
        public int belgeTur = 0;
        /// <summary>
        /// Satır No
        /// </summary>
        public int satirNo = 0;
        /// <summary>
        /// Alt Hesap Kodu
        /// </summary>
        public string altHesapKod = "";
        public string altHesapAd = "";
        /// <summary>
        /// Döviz adı
        /// </summary>
        public string doviz = "";
        /// <summary>
        /// Döviz karşılığı merkez bankası dönüşüm türü Alış, Efektif Alış, Satış, Efektif Satışi, Serbest Alış, Serbest Satış
        /// </summary>
        public string dovizKurTur = "";
        /// <summary>
        /// Döviz borç miktarı
        /// </summary>
        public double dovizBorc = 0.0;
        /// <summary>
        /// Döviz alacak miktarı
        /// </summary>
        public double dovizAlacak = 0.0;
        /// <summary>
        /// Döviz kuru
        /// </summary>
        public decimal dovizKur = 0;
        /// <summary>
        /// Satır Açıklaması
        /// </summary>
        public string satirAciklama = "";

        public int yil { get; set; }

        public string DetayBirim
        {
            get { return detayBirim; }
            set { detayBirim = value; }
        }

        public string detayBirim = "";
        public string mahsupBelgeNo = "";
        public int sorguKapatan = 0;
        public int sorguKapatilan = 0;
        public string sorguTertip = "";
        public bool sadeceAltHesaptanKontrol = false;

        /// <summary>
        /// Kapatma bilgileri
        /// </summary>
        public ObjectArray kapatma;

        public HesapPlaniSatir hKod = null;

        /// <summary>
        ///  Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public OdemeEmriMIFDetay(int yil)
        {
            this.yil = yil;
        }

        /// <summary>
        ///  Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public OdemeEmriMIFDetay(Tertip tertip)
        {
            kur = OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 0, "");
            fon = OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 1, "");
            fin = OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 2, "");
            eko = OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 3, "");
            peb = OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 4, "");
            this.hesapKod = "";
            this.borc = 0;
            this.alacak = 0;
            this.mahsupBelgeNo = "";
            this.kapatma = null;
        }

        public OdemeEmriMIFDetay(int tertipYil, string hesapKod, double borc, double alacak, string mahsupBelgeNo = "")
        {
            this.yil = tertipYil;
            this.hesapKod = hesapKod;
            this.borc = borc;
            this.alacak = alacak;
            this.mahsupBelgeNo = mahsupBelgeNo;
            this.kapatma = null;
        }

        public OdemeEmriMIFDetay(OdemeEmriMIFDetay eski)
        {
            this.kesinti = eski.kesinti;
            this.hesapKod = eski.hesapKod;
            this.kur = eski.kur;
            this.fon = eski.fon;
            this.fin = eski.fin;
            this.eko = eski.eko;
            this.peb = eski.peb;
            this.borc = eski.borc;
            this.alacak = eski.alacak;
            this.aciklama = eski.aciklama;
            this.hesapAd = eski.hesapAd;
            this.belgeNo = eski.belgeNo;
            this.belgeTur = eski.belgeTur;
            this.satirNo = eski.satirNo;
            this.altHesapKod = eski.altHesapKod;
            this.doviz = eski.doviz;
            this.dovizKurTur = eski.dovizKurTur;
            this.dovizBorc = eski.dovizBorc;
            this.dovizAlacak = eski.dovizAlacak;
            this.dovizKur = eski.dovizKur;
            this.satirAciklama = eski.satirAciklama;
            this.mahsupBelgeNo = eski.mahsupBelgeNo;
            this.detayBirim = eski.detayBirim;
            this.kapatma = null;
            if (eski.hKod != null)
            {
                this.hKod = new HesapPlaniSatir(eski.hKod);
            }
            this.yil = eski.yil;
        }

        public int Kiyasla(OdemeEmriMIFDetay o2, string yontemKod)
        {
            if (yontemKod == "1")
            {
                if (this.borc > 0 && o2.borc == 0) return -1;
                if (this.borc == 0 && o2.borc > 0) return 1;
                int sonuc = this.hesapKod.CompareTo(o2.hesapKod);
                if (sonuc != 0) return sonuc;
                if (this.borc > 0) return this.borc.CompareTo(o2.borc);
                return this.alacak.CompareTo(o2.alacak);
            }
            else
                return 0;
        }

        public string TertipStr()
        {
            string retVal = kur + "-" + fon + "-" + fin + "-" + eko;
            if (!string.IsNullOrWhiteSpace(peb)) retVal += "-" + peb;
            return retVal;
        }

        public void TertipAta(string tertip)
        {
            if (!string.IsNullOrWhiteSpace(tertip))
            {
                string[] kodlar = tertip.Split('-');
                kur = OrtakFonksiyonlar.DiziElemanAl(kodlar, 0, "");
                fon = OrtakFonksiyonlar.DiziElemanAl(kodlar, 1, "");
                fin = OrtakFonksiyonlar.DiziElemanAl(kodlar, 2, "");
                eko = OrtakFonksiyonlar.DiziElemanAl(kodlar, 3, "");
                peb = OrtakFonksiyonlar.DiziElemanAl(kodlar, 4, "");
            }
        }

        public string TertipStrTiresiz()
        {
            string retVal = TertipStr();
            while (retVal.EndsWith("-"))
                retVal = retVal.Substring(0, retVal.Length - 1);
            return retVal;
        }

        public void BorcAlacakAyarla()
        {
            if (borc > alacak)
            {
                borc = borc - alacak;
                alacak = 0;
            }
            else
            {
                alacak = alacak - borc;
                borc = 0;
            }

            if (dovizBorc > dovizAlacak)
            {
                dovizBorc = dovizBorc - dovizAlacak;
                dovizAlacak = 0;
            }
            else
            {
                dovizAlacak = dovizAlacak - dovizBorc;
                dovizBorc = 0;
            }
        }

        public double BorcAlacakNet
        {
            get { return Math.Round(borc - alacak, 2); }
        }

        override public bool Equals(object obj)
        {
            string p_hesapKod = "";
            string p_altHesapKod = "";
            string p_tertip = "";
            if (obj.GetType() == typeof(string))
            {
                string[] gel = ((string)obj).Split('-');
                p_hesapKod = OrtakFonksiyonlar.DiziElemanAl(gel, 0, "");
                p_altHesapKod = OrtakFonksiyonlar.DiziElemanAl(gel, 1, "");
                p_tertip = OrtakFonksiyonlar.DiziElemanAl(gel, 2, "");
            }
            else if (obj.GetType() == typeof(OdemeEmriMIFDetay))
            {
                OdemeEmriMIFDetay gel = ((OdemeEmriMIFDetay)obj);
                p_hesapKod = gel.hesapKod;
                p_altHesapKod = gel.altHesapKod;
                p_tertip = gel.TertipStrTiresiz();
            }

            if (p_hesapKod == this.hesapKod && p_altHesapKod == this.altHesapKod && p_tertip == this.TertipStrTiresiz())
                return true;
            return false;
        }

        public double dengeBorc = 0;
        public double dengeAlacak = 0;
        public int prgKod;
        public int aPrgKod;
        public int faalKod;
        public int aFaalKod;

        public double kalanBorc { get { return Math.Round(borc - dengeBorc, 2); } }
        public double kalanAlacak { get { return Math.Round(alacak - dengeAlacak, 2); } }

        public int isKod { get; set; }
        public string isAdi { get; set; }
    }

    /// <summary>
    /// Ödeme Emri/MİF bilgilerini tutan nesne
    /// </summary>
    [Serializable()]
    public class OdemeEmriMIFForm
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Belge türü
        /// </summary>
        public ENUMBelgeTuru belgeTur;
        /// <summary>
        /// İşlem tarihi
        /// </summary>
        public TNS.TNSDateTime islemTarih;

        public string belgeTamNo { get { return yil.ToString() + "-" + belgeNo; } }

        /// <summary>
        /// Belge Numarası
        /// </summary>
        public string belgeNo;
        /// <summary>
        /// Kurum kod bilgisi
        /// </summary>
        public string kurum;
        /// <summary>
        /// Kurum adı
        /// </summary>
        public string kurumAd;
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birden fazla muhasebe bilgisi taşımak için. Noktalı virgül ayıracı ile kullanılır
        /// </summary>
        public string muhasebeListe;
        /// <summary>
        /// Muhasebe adı
        /// </summary>
        public string muhasebeAd;
        /// <summary>
        /// Harcama Birim kodu
        /// </summary>
        public string birim;
        /// <summary>
        /// Harcama Birim adı
        /// </summary>
        public string birimAd;
        /// <summary>
        /// İlgili kişi/firma adı
        /// </summary>
        public string ilgiliAd;
        /// <summary>
        /// İlgili kişi/firma kimlik/vergi numarası
        /// </summary>
        public string ilgiliNo;
        /// <summary>
        /// İlgili kişi/firma vergi dairesi
        /// </summary>
        public string ilgiliVD;
        /// <summary>
        /// İlgili kişi/firma banka adı
        /// </summary>
        public string ilgiliBankaAd;
        /// <summary>
        /// İlgili kişi/firma IBAN numarası
        /// </summary>
        public string ilgiliBankaNo;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Ödemeye esas belge Türü
        /// </summary>
        public string OEBTur;
        /// <summary>
        /// Ödemeye esas belge numarası
        /// </summary>
        public string OEBNo;
        /// <summary>
        /// Ödemeye esas belge tarihi
        /// </summary>
        public TNS.TNSDateTime OEBTarih;
        /// <summary>
        /// Ödemeye esas belge tutarı
        /// </summary>
        public double OEBTutar;
        /// <summary>
        /// İade Olunan Emanetler tarih
        /// </summary>
        public TNS.TNSDateTime IOETarih;
        /// <summary>
        /// İade Olunan Emanetler yevmiye no
        /// </summary>
        public string IOEYevNo;
        /// <summary>
        /// İade Olunan Emanetler ilgili
        /// </summary>
        public string IOEKim;
        /// <summary>
        /// Avans ve Kredi Tarih
        /// </summary>
        public TNS.TNSDateTime AVKTarih;
        /// <summary>
        /// Avans ve Kredi yevmiye Numarası
        /// </summary>
        public string AVKYevNo;
        /// <summary>
        /// Avans ve Kredi tutar
        /// </summary>
        public double AVKTutar;
        /// <summary>
        /// Yevmiye tarihi
        /// </summary>
        public TNS.TNSDateTime YEVYevTarih;
        /// <summary>
        /// Yevmiye numarası
        /// </summary>
        public int YEVYevNo;
        /// <summary>
        /// Verile Emri No
        /// </summary>
        public string YEVVerNo;
        /// <summary>
        /// Çek/Gönder.No
        /// </summary>
        public string YEVCekNo;
        /// <summary>
        /// Çek/Gönder.Tarih
        /// </summary>
        public TNSDateTime YEVCekTarih;
        /// <summary>
        /// Form durumu
        /// </summary>
        public int formDurum;
        /// <summary>
        /// Form durum tarihi
        /// </summary>
        public TNS.TNSDateTime formDurumTarih;
        /// <summary>
        /// İşlem Yapan kullanıcı
        /// </summary>
        public string islemYapan;
        /// <summary>
        /// İşlem Yapan kullanıcı Ad
        /// </summary>
        public string islemYapanAd;
        /// <summary>
        /// İptal edilme nedeni
        /// </summary>
        public string iptalNeden;
        /// <summary>
        /// Aktarım işleminde kullanılan alan (1 değerini alır)
        /// </summary>
        public bool aktarim;
        /// <summary>
        /// İşlem Tarihi Sorgulama için kullanılır
        /// </summary>
        public string sorguIslemTarih;
        /// <summary>
        /// Form durum Tarihi Sorgulama için kullanılır
        /// </summary>
        public string sorguFormDurumTarih;
        /// <summary>
        /// Yevmiye Tarihi Sorgulama için kullanılır
        /// </summary>
        public string sorguYEVYevTarih;
        /// <summary>
        /// Yevmiye Numarası Sorgulama için kullanılır
        /// </summary>
        public string sorguYEVYevNo;
        /// <summary>
        /// Tarih sıralı listenin alınması için Sorgulama için kullanılır
        /// </summary>
        public bool sorguTarihSiraliMi;
        /// <summary>
        /// Ekonomik kod sıralı listenin alınması Sorgulama için kullanılır
        /// </summary>
        public bool sorguEkoSiraliMi;
        /// <summary>
        /// Hesap kod sıralı listenin alınması Sorgulama için kullanılır
        /// </summary>
        public bool sorguHesapKodSiraliMi;
        /// <summary>
        /// Belge Tür Sorgulama için kullanılır
        /// </summary>
        public int sorguBelgeTur;
        public string sorguBirim = "";
        /// <summary>
        /// Döviz kuru ile yapılan işlemler listelensin
        /// </summary>
        public bool sorguDovizliIslemler;
        /// <summary>
        /// Borç toplamı
        /// </summary>
        public double borcToplam;
        /// <summary>
        /// Alacak toplamı
        /// </summary>
        public double alacakToplam;
        /// <summary>
        /// Ödeme emri/MİF detay bilgileri
        /// </summary>
        public ObjectArray detay;
        /// <summary>
        /// Proje Numarası
        /// </summary>
        public string projeNo;
        /// <summary>
        /// İhale bilgisi
        /// </summary>
        public string ihale;
        /// <summary>
        /// <see cref="ENUMFISTUR"/> Fiş tür bilgisi
        /// </summary>
        public ENUMFISTUR[] fisTur = null;
        /// <summary>
        /// İlgili belge numarası
        /// </summary>
        public string ilgiliBelgeNo;
        //Maaş entegrasyonu için
        /// <summary>
        /// Ay bilgisi
        /// </summary>
        public int ay;
        /// <summary>
        /// Personel tür bilgisi
        /// </summary>
        public string perTur;
        /// <summary>
        /// İşlendi
        /// </summary>
        public string islendi;
        /// <summary>
        /// Bordro türü
        /// </summary>
        public string bordroTur;
        //****************************
        //Süreç entegrasyonu için
        /// <summary>
        /// Süreç yıl
        /// </summary>
        public int surecYil;
        /// <summary>
        /// Süreç Numarası
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Kurumsal Kod
        /// </summary>
        public string kurKod;
        /// <summary>
        /// Kurumsal Kod
        /// </summary>
        public int organizasyonKodu;
        public bool yilSonuBelgesiMi;
        public bool kurFarkiBelgesiMi;

        public string yevmiyeAciklama;
        public string duzeltilenOdemeEmriNo;
        public int ekliDosyaVarmi = 0;

        public int odenekKontroluYapma;
        public IstatistikKodDeger istKod1;
        public IstatistikKodDeger istKod2;

        public bool sadeceUstVeri = false;
        public bool mukerrerlikKontrolu = false;
        public bool altHesapZorunsuzsaHata = false;

        public TNSDateTime baslamaTarihi;
        public TNSDateTime bitisTarihi;

        public int yevmiyeNo1;
        public int yevmiyeNo2;

        public string ettn;


        public void GiderTertipleriBul(System.Collections.Generic.List<string> tertipler, System.Collections.Generic.List<double> tutarlar)
        {
            tertipler.Clear();
            tutarlar.Clear();
            foreach (OdemeEmriMIFDetay item in detay.objeler)
            {
                if (item.hesapKod.StartsWith("830") && !string.IsNullOrEmpty(item.kur))
                {
                    string tertip = item.kur + "-" + item.fon + "-" + item.fin + "-" + item.eko + "-" + item.peb;
                    int index = tertipler.IndexOf(tertip);
                    if (index >= 0)
                    {
                        tutarlar[index] += (item.borc - item.alacak);
                    }
                    else
                    {
                        tertipler.Add(tertip);
                        tutarlar.Add(item.borc - item.alacak);
                    }
                }
            }
        }

        public static bool OdenekKontroluYapilmayanEkoMu4(string eko)
        {
            eko = eko.Replace(".", "");
            if (
                //eko.StartsWith("034201") ||
                //eko.StartsWith("034204") ||
                eko.StartsWith("039") ||
                eko.StartsWith("033201") ||
                eko.StartsWith("034") ||
                eko.StartsWith("0309") ||
                eko.StartsWith("03032001") ||
                eko.StartsWith("0304")
                ) return true;
            return false;
        }

        public static string _haricKodlar = null;
        public static string haricEkolar
        {
            get
            {
                if (_haricKodlar == null)
                {
                    _haricKodlar = TNS.DEG.Arac.DegiskenDegerBul(0, "OdenekUstuEkolar");
                    if (_haricKodlar == null) _haricKodlar = "";
                }
                return _haricKodlar;
            }
        }

        public void OdenekKontroluTertipleriBul(ENUMOdenekKontrolTur kontrolTur, bool talep, System.Collections.Generic.List<OdemeEmriMIFDetay> tertipler, System.Collections.Generic.List<OdemeEmriMIFDetay> kontrolsuzTertipler)
        {
            Tertip tUzBul = new Tertip(this.yil, ENUMTabloKod.TABPEBOdenek);

            int eko2Uz = tUzBul.KodUzunlukBul(3, 2, true);
            int eko3Uz = tUzBul.KodUzunlukBul(3, 3, true);

            foreach (OdemeEmriMIFDetay item in detay.objeler)
            {
                if (item.hKod == null) continue;
                if (!(item.hKod.nitelik.Contains(";" + ((int)ENUMHesapKodNitelik.GIDERBUTCESI) + ";") || item.hKod.nitelik.Contains(";" + ((int)ENUMHesapKodNitelik.AVANS) + ";"))) continue;
                if (!(item.kur != "" && item.fon != "" && item.fin != "" && item.eko != "")) continue;
                if (item.hesapKod.StartsWith("162")) continue;
                if (!(item.hesapKod.StartsWith("16") || item.hesapKod.StartsWith("830"))) continue;

                string[] ekoParcalar = haricEkolar.Split(',', ';');
                bool haricBulduk = false;
                foreach (var ekoParca in ekoParcalar)
                {
                    if (string.IsNullOrWhiteSpace(ekoParca)) continue;
                    if (item.eko.Replace(".", "").StartsWith(ekoParca.Replace(".", "")))
                    {
                        haricBulduk = true;
                        break;
                    }
                }

                if (!haricBulduk)
                    KontrolListesineEkle(this.yil, item, tertipler, kontrolTur, talep, eko2Uz, eko3Uz);
                else if (kontrolsuzTertipler != null)
                    KontrolListesineEkle(this.yil, item, kontrolsuzTertipler, kontrolTur, talep, eko2Uz, eko3Uz);
            }
            AlacakHesaplariDus(tertipler);
            AlacakHesaplariDus(kontrolsuzTertipler);
        }

        private void AlacakHesaplariDus(List<OdemeEmriMIFDetay> tertipler)
        {
            if (tertipler == null) return;
            for (int i = tertipler.Count - 1; i >= 0; i--)
            {
                OdemeEmriMIFDetay det = (OdemeEmriMIFDetay)tertipler[i];
                if (det.alacak > det.borc)
                    tertipler.RemoveAt(i);
            }
        }

        public static void KontrolListesineEkle(int yil, OdemeEmriMIFDetay item, System.Collections.Generic.List<OdemeEmriMIFDetay> liste, ENUMOdenekKontrolTur kontrolTur, bool talep, int eko2Uz, int eko3Uz)
        {
            string kontrolEko = item.eko;
            string kontrolPeb = item.peb;
            if (kontrolTur != ENUMOdenekKontrolTur.TANIMSIZ)
            {
                if (kontrolTur == ENUMOdenekKontrolTur.E2OG_E4OE_E2K || kontrolTur == ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP || kontrolTur == ENUMOdenekKontrolTur.E4POG_E4POE_E2K || kontrolTur == ENUMOdenekKontrolTur.E4POG_E4POE_E2PK)
                {
                    int ekoUz;
                    if (TNS.BKD.Arac.Eko23Mu(yil, item.eko.Replace(".", "")))
                        ekoUz = eko3Uz;
                    else
                        ekoUz = eko2Uz;


                    kontrolEko = item.eko.Replace(".", "").Substring(0, ekoUz);
                }

                if (kontrolTur == ENUMOdenekKontrolTur.E2OG_E4OE_E2K || kontrolTur == ENUMOdenekKontrolTur.E4POG_E4POE_E2K)
                {
                    kontrolPeb = "";
                }
            }
            OdemeEmriMIFDetay satir = ListedeVarMi(liste, item.kur, item.fon, item.fin, kontrolEko, kontrolPeb);
            if (satir == null)
            {
                satir = new OdemeEmriMIFDetay(yil);
                satir.kur = item.kur;
                satir.fon = item.fon;
                satir.fin = item.fin;
                satir.eko = kontrolEko;
                satir.peb = kontrolPeb;
                satir.aciklama = item.kur + "-" + item.fon + "-" + item.fin + "-";

                if (talep && kontrolTur == ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)
                {
                    satir.aciklama += item.eko + "-" + item.peb;
                }
                else if (kontrolTur == ENUMOdenekKontrolTur.E2OG_E4OE_E2K || kontrolTur == ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)
                {
                    satir.aciklama += item.eko.Replace(".", "").Substring(0, eko2Uz);
                    if (kontrolTur == ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)
                        satir.aciklama += "-" + item.peb;
                }
                else
                {
                    satir.aciklama += item.eko + "-" + item.peb;
                }
                liste.Add(satir);
            }
            satir.borc += item.borc - item.alacak;
        }

        private static OdemeEmriMIFDetay ListedeVarMi(System.Collections.Generic.List<OdemeEmriMIFDetay> liste, string kur, string fon, string fin, string eko, string peb)
        {
            foreach (OdemeEmriMIFDetay item in liste)
            {
                if (item.kur == kur &&
                item.fon == fon &&
                item.fin == fin &&
                item.eko == eko &&
                item.peb == peb) return item;
            }
            return null;
        }

        public System.Collections.Generic.List<OdemeEmriMIFDetay> AvansKapatmaTertipleriBul(int eko2Uz, int eko3Uz)
        {
            System.Collections.Generic.List<OdemeEmriMIFDetay> liste = new System.Collections.Generic.List<OdemeEmriMIFDetay>();
            foreach (OdemeEmriMIFDetay item in detay.objeler)
            {
                if (item.hKod != null && item.hKod.nitelik.Contains(";" + ((int)ENUMHesapKodNitelik.AVANS) + ";"))
                {
                    KontrolListesineEkle(this.yil, item, liste, ENUMOdenekKontrolTur.TANIMSIZ, false, eko2Uz, eko3Uz);
                }
            }
            return liste;
        }

        //****************************
        /// <summary>
        ///  Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public OdemeEmriMIFForm()
        {
            istKod1 = new IstatistikKodDeger();
            istKod2 = new IstatistikKodDeger();
            odenekKontroluYapma = 0;
            yil = 0;
            belgeTur = ENUMBelgeTuru.TANIMSIZ;
            islemTarih = new TNSDateTime();
            belgeNo = "";
            kurum = "";
            muhasebe = 0;
            muhasebeListe = "";
            birim = "";
            ilgiliAd = "";
            ilgiliNo = "";
            ilgiliVD = "";
            ilgiliBankaAd = "";
            ilgiliBankaNo = "";
            aciklama = "";
            OEBTur = "";
            OEBNo = "";
            OEBTarih = new TNSDateTime();
            OEBTutar = 0;
            IOETarih = new TNSDateTime();
            IOEYevNo = "";
            IOEKim = "";
            AVKTarih = new TNSDateTime();
            AVKYevNo = "";
            AVKTutar = 0;
            YEVYevTarih = new TNSDateTime();
            YEVYevNo = 0;
            YEVVerNo = "";
            YEVCekNo = "";
            YEVCekTarih = new TNSDateTime();
            formDurum = -1;
            formDurumTarih = new TNSDateTime();
            islemYapan = "";
            iptalNeden = "";
            aktarim = false;
            sorguIslemTarih = "";
            sorguFormDurumTarih = "";
            sorguYEVYevTarih = "";
            sorguYEVYevNo = "";
            sorguTarihSiraliMi = false;
            sorguEkoSiraliMi = false;
            sorguBelgeTur = -1;
            sorguBirim = "";
            borcToplam = 0;
            alacakToplam = 0;
            detay = new ObjectArray();
            projeNo = "";
            ihale = "";
            fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.DONEMICI, ENUMFISTUR.ACILIS });
            ilgiliBelgeNo = "";
            ay = 0;
            perTur = "";
            islendi = "";
            bordroTur = "";
            surecYil = 0;
            surecNo = "";
            kurKod = "";
            organizasyonKodu = 0;
            yilSonuBelgesiMi = false;
            kurFarkiBelgesiMi = false;
            baslamaTarihi = new TNSDateTime();
            bitisTarihi = new TNSDateTime();
            yevmiyeNo1 = 0;
            yevmiyeNo2 = 0;
            ettn = "";

            //fisTurYap(ENUMFISTUR.DONEMICI);
        }

        public double ToplamAl(string hesapKod, bool borc)
        {
            double toplam = 0;
            if (detay != null && detay.ObjeSayisi > 0)
            {
                foreach (OdemeEmriMIFDetay item in detay.objeler)
                {
                    if (item.hesapKod.StartsWith(hesapKod))
                    {
                        if (borc) toplam += item.borc;
                        else toplam += item.alacak;
                    }
                }
            }
            return toplam;
        }

        public OdemeEmriMIFDetay SatirBul(string hesapKod, string detayBirim, string tertip, ref int satirNo)
        {
            for (int i = satirNo + 1; i < detay.ObjeSayisi; i++)
            {
                OdemeEmriMIFDetay detaySatir = (OdemeEmriMIFDetay)detay.objeler[i];
                if (detaySatir.hesapKod.Replace(".", "") == hesapKod.Replace(".", "") &&
                    detaySatir.detayBirim == detayBirim &&
                    detaySatir.TertipStr().Replace(".", "").StartsWith(tertip.Replace(".", "")))
                {
                    satirNo = i;
                    return detaySatir;
                }
            }
            satirNo = -1;
            return null;
        }

        public void fisTurYap(ENUMFISTUR[] p)
        {
            fisTur = p;
        }

        public void fisTurYap(ENUMFISTUR p)
        {
            fisTur = new ENUMFISTUR[] { p };
        }

    }

    public enum ENUMOdenekGondermeTur
    {
        eko2 = 0,
        eko4peb = 1,
        eko2peb = 2,
    }

    public enum ENUMOdenekUstuKontrol
    {
        ODENEK,
        ODENEKGONDERME
    }

    /// <summary>
    /// Belgelere eklenen dokumanlar
    /// </summary>
    [Serializable()]
    public class Dokuman
    {
        /// <summary>
        /// Yıl anahtarı
        /// </summary>
        public int yil = 0;
        /// <summary>
        /// Belge numarası anahtarı
        /// </summary>
        public string belgeNo = "";
        /// <summary>
        /// Birden fazla belge için belge sırası
        /// </summary>
        public int siraNo = 0;
        /// <summary>
        /// Dokuman adı
        /// </summary>
        public string dokumanAdi;
        /// <summary>
        /// dokuman içeriği
        /// </summary>
        public byte[] dokumanIcerik;
        public string kayitEdilecekDosyaYol = "";
    }

    /// <summary>
    /// Ödenek Gönderme / Tenkis detay bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class GondermeTenkisDetay
    {
        /// <summary>
        /// Bütçe tertibi
        /// </summary>
        public Tertip tertip;
        /// <summary>
        /// Ödenek miktarı
        /// </summary>
        public double miktar;
        /// <summary>
        /// Kullanılabilir ödenek miktar
        /// </summary>
        public double kullanilabilir;
        /// <summary>
        /// Onaysız Gönderilen - Onaylı Tenkis
        /// </summary>
        public double gonderilen;
        /// <summary>
        /// Avans dahil
        /// </summary>
        public double harcanan;
        /// <summary>
        /// İçinde bulunan dönemdeki MP değeri
        /// </summary>
        public double donemMP;
        /// <summary>
        ///Toplam MP
        /// </summary>
        public double toplamMP;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public GondermeTenkisDetay()
        {
            tertip = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABPEBOdenek);
            miktar = 0;
            kullanilabilir = 0;
            gonderilen = 0;
            harcanan = 0;
            donemMP = 0;
            toplamMP = 0;
        }

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl</param>
        public GondermeTenkisDetay(int yil)
        {
            tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            miktar = 0;
            kullanilabilir = 0;
        }
    }

    /// <summary>
    /// Ödenek Gönderme / Tenkis form bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class GondermeTenkisForm
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Belge Tür (Ödenek Gönderme/Tenkis)
        /// </summary>
        public ENUMBelgeTuru belgeTur;
        /// <summary>
        /// Belge No
        /// </summary>
        public string belgeNo;
        /// <summary>
        /// Kurum kodu
        /// </summary>
        public string kurum;
        /// <summary>
        /// Kurum adı
        /// </summary>
        public string kurumAd;
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Muhasebe adı
        /// </summary>
        public string muhasebeAd;
        /// <summary>
        /// Harcama birimn kodu
        /// </summary>
        public string birim;
        /// <summary>
        /// Harcama birim adı
        /// </summary>
        public string birimAd;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Yazı Numarası
        /// </summary>
        public string yaziNo;
        /// <summary>
        /// Yazı tarihi
        /// </summary>
        public TNS.TNSDateTime yaziTarih;
        /// <summary>
        /// İstek yazı Numarası
        /// </summary>
        public string istekYaziNo;
        /// <summary>
        /// İstek yazı tarihi
        /// </summary>
        public TNS.TNSDateTime istekYaziTarih;
        /// <summary>
        /// Forum Durumu
        /// </summary>
        public int formDurum;
        /// <summary>
        /// Form Durum tarihi
        /// </summary>
        public TNS.TNSDateTime formDurumTarih;
        /// <summary>
        /// İşlem yapan Kullanıcı
        /// </summary>
        public string islemYapan;
        /// <summary>
        /// İptal nedeni
        /// </summary>
        public string iptalNeden;
        /// <summary>
        /// icmal Numarası
        /// </summary>
        public int icmalNo;
        /// <summary>
        /// İcmal tarihi
        /// </summary>
        public TNS.TNSDateTime icmalTarih;
        /// <summary>
        /// Onay Numarası
        /// </summary>
        public string onayNo;
        /// <summary>
        /// Onay Tarihi
        /// </summary>
        public TNS.TNSDateTime onayTarih;
        /// <summary>
        /// İşlem tarihi
        /// </summary>
        public TNS.TNSDateTime islemTarih;
        /// <summary>
        /// Aktarım işleminde kullanılacak
        /// </summary>
        public bool aktarim;

        /// <summary>
        /// İşlem tarihi (Sorgulama kullanılan)
        /// </summary>
        public string sorguIslemTarih;
        /// <summary>
        /// Form Durum tarihi (Sorgulama kullanılan)
        /// </summary>
        public string sorguFormDurumTarih;
        /// <summary>
        /// İcmal tarihi (Sorgulama kullanılan)
        /// </summary>
        public string sorguIcmalTarih;
        /// <summary>
        /// Muhasebe Kodu (Sorgulama kullanılan)
        /// </summary>
        public string sorguMuhasebeKod;
        /// <summary>
        /// Belge toplamı
        /// </summary>
        public double belgeToplami;
        /// <summary>
        /// Ödenek Gönderme/Tenkis detay bilgileri
        /// </summary>
        public ObjectArray detay;
        /// <summary>
        /// Kontrol tür
        /// </summary>
        public int kontrolTur;
        /// <summary>
        /// Talep bilgisi
        /// </summary>
        public string talep;
        /// <summary>
        /// Kur Fon Fin Eko; eko2 aynı kalmak üzere farklı bir peb kodu ve eko4 de harcama yapabilmesini sağlıyor. Bu alan eksi (-) yazılır diğer satır ile aynı rakam olmalı
        /// </summary>
        public int DortDuzeyDegisim = 0;
        /// <summary>
        /// Süreç yılı
        /// </summary>
        public int surecYil = 0;
        /// <summary>
        /// Süreç Numarası
        /// </summary>
        public string surecNo = "";

        /// <summary>
        /// Taşra kullanıcılarının bütün belgeler(iptaller ariç) için 
        /// </summary>
        public int tasra;
        public string vergiNo;
        public int isKodu { get; set; }
        public string isAdi { get; set; }
        public string ekDosyaAd1 { get; set; }
        public string ekDosyaAd2 { get; set; }



        /// <summary>
        ///  Sınıfının yeni bir örneğini oluşturur.
        /// </summary>     
        public GondermeTenkisForm()
        {
            yil = 0;
            belgeTur = ENUMBelgeTuru.TANIMSIZ;
            belgeNo = "";
            kurum = "";
            kurumAd = "";
            muhasebe = 0;
            muhasebeAd = "";
            birim = "";
            birimAd = "";
            aciklama = "";
            yaziNo = "";
            yaziTarih = new TNSDateTime();
            istekYaziNo = "";
            istekYaziTarih = new TNSDateTime();
            formDurum = -1;
            formDurumTarih = new TNSDateTime();
            islemYapan = "";
            iptalNeden = "";
            icmalNo = -1;
            icmalTarih = new TNSDateTime();
            onayNo = "";
            onayTarih = new TNSDateTime();
            islemTarih = new TNSDateTime();
            aktarim = false;
            sorguIslemTarih = "";
            sorguFormDurumTarih = "";
            sorguIcmalTarih = "";
            sorguMuhasebeKod = "";
            belgeToplami = 0;
            detay = new ObjectArray();
            kontrolTur = -1;
            talep = "";
            surecYil = 0;
            surecNo = "";
            tasra = 0;
        }

        /// <summary>
        /// Talepler ekonomik 4 seviyede göndermeler ise eko 2 seviyede olduğundan, talep satırlarını birleştirip eko 2 seviye indiren metod.
        /// </summary>
        /// <param name="okTur">Ödenek göndermelerin hangi tertip kırılımlarında olacağını gösteren değişken.</param>
        public void SatirlariTopla(int yil, ENUMOdenekKontrolTur okTur, bool eko23)
        {
            if (okTur != ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)
                return;
            for (int i = 0; i < this.detay.ObjeSayisi; i++)
            {
                GondermeTenkisDetay simdikiSatir = (GondermeTenkisDetay)this.detay.objeler[i];
                string eko = Eko2Yap(yil, simdikiSatir.tertip.kodlar[3], eko23);

                simdikiSatir.tertip.kodlar[3] = eko;
                for (int j = this.detay.ObjeSayisi - 1; j > i; j--)
                {
                    GondermeTenkisDetay ayniMiSatir = (GondermeTenkisDetay)this.detay.objeler[j];
                    if (ayniMiSatir.tertip.Kiyasla(simdikiSatir.tertip))
                    {
                        simdikiSatir.miktar += ayniMiSatir.miktar;
                        this.detay.objeler.RemoveAt(j);
                    }
                }
            }
        }

        private string Eko2Yap(int yil, string eko, bool eko23)
        {
            string[] ekolar = eko.Split('.');
            string yeni = "";

            int uz;
            if (eko23 && TNS.BKD.Arac.Eko23Mu(yil, eko.Replace(".", "")))
                uz = 3;
            else
                uz = 2;

            for (int i = 0; i < ekolar.Length && i < uz; i++)
            {
                if (yeni != "") yeni += ".";
                yeni += ekolar[i];
            }
            return yeni;
        }

        public double tutarBE { get; set; }

        public double tutarKE { get; set; }
        public string surecYilNo
        {
            get
            {
                if (surecYil > 0)
                {
                    return surecYil + "/" + surecNo;
                }
                else
                    return "";
            }
        }

        public string OnayKodBul()
        {
            int kod = 0;
            if (!string.IsNullOrWhiteSpace(this.talep))
                kod += 100;
            if (this.belgeTur == ENUMBelgeTuru.ODENEKGONDERME) kod += 1;
            else if (this.belgeTur == ENUMBelgeTuru.TENKIS) kod += 2;
            else kod += 3;
            return kod.ToString("0000");
        }

        public string BelgeNoYap(string kurumKod, int belgeNo)
        {
            return kurumKod + belgeNo.ToString().PadLeft(5, '0');
        }

        public string DosyaAnahtarOl()
        {
            return yil + "-" + ((int)belgeTur) + "-" + belgeNo;
        }
    }

    /// <summary>
    /// Saymanlık/Harcama Birimi ile birlikte ödenek gönderme / tenkis bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class GondermeTenkisSayBirim
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Kurum kodu
        /// </summary>
        public string kurum;
        /// <summary>
        /// Belge Türü
        /// </summary>
        public ENUMBelgeTuru belgeTur;
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Muhasebe Adı
        /// </summary>
        public string muhasebeAd;
        /// <summary>
        /// Harcama birim kodu
        /// </summary>
        public string birim;
        /// <summary>
        /// Harcama birim adı
        /// </summary>
        public string birimAd;
        /// <summary>
        /// Form durumu
        /// </summary>
        public int formDurum;
        /// <summary>
        /// İşlem yapan kullanıcı
        /// </summary>
        public string islemYapan;
        /// <summary>
        /// Ödenek tutarı
        /// </summary>
        public double odenekToplami;
        /// <summary>
        /// Tenkis tutarı
        /// </summary>
        public double tenkisToplami;
        /// <summary>
        /// Avans tutarı
        /// </summary>
        public double avansToplami;
        /// <summary>
        /// Detay satırlarını taşıyan nesne
        /// </summary>
        public ObjectArray detay;
        /// <summary>
        /// Saymanlık/Birim bilgilerini taşıyan nesne
        /// </summary>
        public ObjectArray saymanlikBirim;
        /// <summary>
        /// Tertip bilgisi
        /// </summary>
        public Tertip tertip;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public GondermeTenkisSayBirim()
        {
            yil = 0;
            kurum = "";
            belgeTur = ENUMBelgeTuru.TANIMSIZ;
            muhasebe = 0;
            muhasebeAd = "";
            birim = "";
            birimAd = "";
            formDurum = -1;
            odenekToplami = 0;
            tenkisToplami = 0;
            avansToplami = 0;
            tertip = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdenek);
            detay = new ObjectArray();
            saymanlikBirim = new ObjectArray();
        }
    }

    /// <summary>
    /// Ödenek gönderme/Tenkis belgelerinin kontrol raporu bilgileri
    /// </summary>
    [Serializable()]
    public class GondermeTenkisKontrolListesi
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Belge Tür
        /// </summary>
        public ENUMBelgeTuru belgeTur;
        /// <summary>
        /// Belge Numarası
        /// </summary>
        public string belgeNo;
        /// <summary>
        /// Kurum kodu
        /// </summary>
        public string kurum;
        /// <summary>
        /// Kurum adı
        /// </summary>
        public string kurumAd;
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Muhasebe adı
        /// </summary>
        public string muhasebeAd;
        /// <summary>
        /// Harcama birim kodu
        /// </summary>
        public string birim;
        /// <summary>
        /// harcama biri adı
        /// </summary>
        public string birimAd;
        /// <summary>
        /// Form durum
        /// </summary>
        public int formDurum;
        /// <summary>
        /// Form durumunun değiştiği tarih
        /// </summary>
        public TNS.TNSDateTime formDurumTarih;
        /// <summary>
        /// İşlem yapan kullanıcı
        /// </summary>
        public string islemYapan;
        /// <summary>
        /// İcmal numarası
        /// </summary>
        public int icmalNo;
        /// <summary>
        /// İcmal tarihi
        /// </summary>
        public TNS.TNSDateTime icmalTarih;
        /// <summary>
        /// Onay numarası
        /// </summary>
        public string onayNo;
        /// <summary>
        /// Onay tarihi
        /// </summary>
        public TNS.TNSDateTime onayTarih;
        /// <summary>
        /// İşlşem tarihi
        /// </summary>
        public TNS.TNSDateTime islemTarih;
        /// <summary>
        /// Tertip bilgisi
        /// </summary>
        public Tertip tertip;
        /// <summary>
        /// Tutar
        /// </summary>
        public double miktar;
        public string vergiNo;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public GondermeTenkisKontrolListesi()
        {
            yil = 0;
            belgeTur = ENUMBelgeTuru.TANIMSIZ;
            belgeNo = "";
            kurum = "";
            kurumAd = "";
            muhasebe = 0;
            muhasebeAd = "";
            birim = "";
            birimAd = "";
            formDurum = -1;
            formDurumTarih = new TNSDateTime();
            islemYapan = "";
            icmalNo = 0;
            icmalTarih = new TNSDateTime();
            onayNo = "";
            onayTarih = new TNSDateTime();
            islemTarih = new TNSDateTime();
            tertip = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdenek);
            miktar = 0;
        }
    }

    /// <summary>
    /// Hesap Planı bilgileri
    /// </summary>
    [Serializable()]
    public class HesapPlaniSatir
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil = 0;

        private string _hesapKod = "";
        /// <summary>
        /// Hesap Plan kodu
        /// </summary>
        public string hesapKod { get { return _hesapKod; } set { _hesapKod = value; } }

        private string _aciklama = "";
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama { get { return _aciklama; } set { _aciklama = value; } }

        /// <summary>
        /// Seviye
        /// </summary>
        public int seviye = 0;
        /// <summary>
        /// Koruma
        /// </summary>
        public int koruma = -1;
        /// <summary>
        /// Bakiye
        /// </summary>
        public int bakiye = -1;
        /// <summary>
        /// Tertip
        /// </summary>
        public int tertip = -1;
        /// <summary>
        /// Kesinti
        /// </summary>
        public int kesinti = -1;
        /// <summary>
        /// Nitelik
        /// </summary>
        public string nitelik = "";
        /// <summary>
        /// Grup
        /// </summary>
        public string grup = "";
        /// <summary>
        /// Detay
        /// </summary>
        public bool detay = false;
        /// <summary>
        /// Alt Hesap Kodu
        /// </summary>
        public int altHesapKod = -1;
        /// <summary>
        /// Kod ve açıklama alanlarından arama yapmak için
        /// </summary>
        public string hesapKodHesapAd = "";
        /// <summary>
        /// Döviz kur kodu
        /// </summary>
        public string dovizKurKod = "";
        /// <summary>
        /// Döviz Kur türü Satış
        /// </summary>
        public string dovizKurTurSatis = "";
        /// <summary>
        /// Döviz Kur türü Alış
        /// </summary>
        public string dovizKurTurAlis = "";
        /// <summary>
        /// Kur farkı alacak hesabı
        /// </summary>
        public string kurFarkiAlacakHesabi = "";
        /// <summary>
        /// kur farkı borç hesabı
        /// </summary>
        public string kurFarkiBorcHesabi = "";

        public HesapPlaniSatir()
        {
        }

        public HesapPlaniSatir(HesapPlaniSatir hesapPlaniSatir)
        {
            this.yil = hesapPlaniSatir.yil;
            this._hesapKod = hesapPlaniSatir._hesapKod;
            this._aciklama = hesapPlaniSatir._aciklama;
            this.seviye = hesapPlaniSatir.seviye;
            this.koruma = hesapPlaniSatir.koruma;
            this.bakiye = hesapPlaniSatir.bakiye;
            this.tertip = hesapPlaniSatir.tertip;
            this.kesinti = hesapPlaniSatir.kesinti;
            this.nitelik = hesapPlaniSatir.nitelik;
            this.grup = hesapPlaniSatir.grup;
            this.detay = hesapPlaniSatir.detay;
            this.altHesapKod = hesapPlaniSatir.altHesapKod;
            this.hesapKodHesapAd = hesapPlaniSatir.hesapKodHesapAd;
            this.dovizKurKod = hesapPlaniSatir.dovizKurKod;
            this.dovizKurTurSatis = hesapPlaniSatir.dovizKurTurSatis;
            this.dovizKurTurAlis = hesapPlaniSatir.dovizKurTurAlis;
            this.kurFarkiAlacakHesabi = hesapPlaniSatir.kurFarkiAlacakHesabi;
            this.kurFarkiBorcHesabi = hesapPlaniSatir.kurFarkiBorcHesabi;
        }

        public int ekoBasla { get; set; }

        public bool BorcAlacakCalisirMi()
        {
            return bakiye == 0;
        }

        public bool BorcCalisirMi()
        {
            return bakiye == 1;
        }

        public bool AlacakCalisirMi()
        {
            return bakiye == 2;
        }

        public bool MahsupYapilacakMi()
        {
            return TNS.MUH.Arac.NitelikVarmi(nitelik, (int)ENUMHesapKodNitelik.MAHSUP);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class HesapPlaniYansitma
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Hesap kodu
        /// </summary>
        public string hesapKod;
        /// <summary>
        /// Yansıtma hesap kodu
        /// </summary>
        public string yansitmaHesapKod;
        /// <summary>
        /// satur numarası
        /// </summary>
        public int satir;
        /// <summary>
        /// Hesap Kodu adı
        /// </summary>
        public string hKodAciklama;
        /// <summary>
        /// Yansıtma hesap kodu adı
        /// </summary>
        public string yKodAciklama;

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (GetType() == obj.GetType())
            {
                return this.hesapKod.Equals(((HesapPlaniYansitma)obj).hesapKod);
            }
            if (obj.GetType() == typeof(string))
            {
                bool sonuc = this.hesapKod.Equals((string)obj);
                return sonuc;
            }
            return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            throw new NotImplementedException();
            return base.GetHashCode();
        }
        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public HesapPlaniYansitma()
        {
            yil = 0;
            hesapKod = "";
            yansitmaHesapKod = "";
            hKodAciklama = "";
            yKodAciklama = "";
            satir = 0;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class MuhasebeBirim
    {
        public string MuhasebeBirimKurum
        {
            get { return muhasebe + "." + birimKod + "." + kurum; }

            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    string[] parcalar = value.Split('.');
                    if (parcalar.Length >= 3)
                    {
                        muhasebe = OrtakFonksiyonlar.ConvertToInt(parcalar[0], 0);
                        birimKod = parcalar[1];
                        kurum = parcalar[2];
                        for (int i = 3; i < parcalar.Length; i++)
                        {
                            kurum += "." + parcalar[i];
                        }
                    }
                }
            }
        }
        public string BirimKurum { get { return birimKod + "." + kurum; } }

        /// <summary>
        /// Harcama Birim kod
        /// </summary>
        public string birimKod { get; set; }

        /// <summary>
        /// Muhasebe kod
        /// </summary>
        public int muhasebe { get; set; }

        /// <summary>
        /// Kurum kod
        /// </summary>
        public string kurum { get; set; }

        /// <summary>
        /// Harcama Birim adı
        /// </summary>
        public string birimAd;

        /// <summary>
        /// Harcama Birim adı
        /// </summary>
        public string MuhasebeBirimAd { get { return TNS.MUH.Arac.BirimAdOl(muhasebeAd, birimAd); } }

        /// <summary>
        /// Harcama Birim adı
        /// </summary>
        public string BirimKodAd { get { return BirimKurum + "-" + MuhasebeBirimAd; } }

        /// <summary>
        /// Harcama Birim adı
        /// </summary>
        public string MuhasebeBirimKodAd { get { return MuhasebeBirimKurum + "-" + MuhasebeBirimAd; } }

        /// <summary>
        /// Muhasebe adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// İlin Plaka Kodu
        /// </summary>
        public int plakaKod;

        public string orgCografyaKod;

        public bool kurumLike = true;
        public int pasif = 0;

        public string vno;

        public string aramaKosul = "";
        public int pasif_0Hepsi_1Aktif_2Pasif = 0;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public MuhasebeBirim()
        {
            birimKod = "";
            muhasebe = 0;
            kurum = "";
            birimAd = "";
            muhasebeAd = "";
            plakaKod = 0;
            vno = "";
        }

        override public string ToString()
        {
            return muhasebeAd;
        }
    }

    [Serializable()]
    public class Mudurluk
    {
        public string kod = "";
        public string ad = "";
        public int aktif = 0;
    }

    /// <summary>
    /// Muhasebe Yetkili bilgileri
    /// </summary>
    [Serializable()]
    public class MuhasebeYetkilisi
    {
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Adı soyadı
        /// </summary>
        public string adsoyad;
        /// <summary>
        /// Ünvanı
        /// </summary>
        public string unvan;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public MuhasebeYetkilisi()
        {
            muhasebe = 0;
            adsoyad = "";
            unvan = "";
        }
    }

    /// <summary>
    /// İmza Bilgisi bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class ImzaBilgisi
    {
        /// <summary>
        /// Kullanıcı kodu
        /// </summary>
        public string kullaniciKodu = "";
        /// <summary>
        /// Belge Türü
        /// </summary>
        public ENUMBelgeTuru belgeTur = ENUMBelgeTuru.TANIMSIZ;
        /// <summary>
        /// Muhasebe Kodu
        /// </summary>
        public int muhasebe = 0;
        /// <summary>
        /// Birim kodu
        /// </summary>
        public string birim = "";
        /// <summary>
        /// İmza yeri
        /// </summary>
        public string imzaYer = "";
        /// <summary>
        /// İmzayalacak kişinin Adı soyadı
        /// </summary>
        public string adSoyad = "";
        /// <summary>
        /// İmzayalacak kişinin Ünvanı
        /// </summary>
        public string unvan = "";
        /// <summary>
        /// İmzayalacak kişinin görevi
        /// </summary>
        public string gorevUnvan = "";
        /// <summary>
        /// Tarih
        /// </summary>
        public int tarih = 0;
        /// <summary>
        /// Kullanılmıyor
        /// </summary>
        public int kullanilmiyor = 0;
    }

    /// <summary>
    /// Muhasebe bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class MuhasebeBilgisi
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod = 0;
        /// <summary>
        /// Ad
        /// </summary>
        public string ad = "";

        public int Kod { get { return kod; } set { kod = value; } }
        public string Ad { get { return ad; } set { ad = value; } }

        public string kistasKodAd { get; set; }

        public string ilgili1 { get; set; }
        public string ilgili2 { get; set; }
        public string ilgili1Unvan { get; set; }
        public string ilgili2Unvan { get; set; }
        public string adres { get; set; }
        public string telefon { get; set; }
        public string vergiNo { get; set; }
        public string faks { get; set; }
        public string ePosta { get; set; }
    }

    /// <summary>
    /// Yanstıma yapılmasına karar verilen satırların yansıtma penceresinde gösterimi sırasında kullanılır
    /// </summary>
    [Serializable()]
    public class YansitmaYapilacakSatirlar
    {
        /// <summary>
        /// 
        /// </summary>
        public string hesapKod = "";
        /// <summary>
        /// 
        /// </summary>
        public string hesapAd = "";
        /// <summary>
        /// 
        /// </summary>
        public string yansitma = "";
        /// <summary>
        /// 
        /// </summary>
        public string tertip = "";
        /// <summary>
        /// 
        /// </summary>
        public double borc = 0.0;
        /// <summary>
        /// 
        /// </summary>
        public double alacak = 0.0;

        public string detayBirim = "";

        public string isKod = "";
    }

    /// <summary>
    /// Form kayıt işlemlerinde log bilgilerini taşımak için kullanılan nesne
    /// </summary>
    [Serializable()]
    public class TarihceBilgisi
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil = 0;
        /// <summary>
        /// Belge türü
        /// </summary>
        public ENUMBelgeTuru belgeTur = ENUMBelgeTuru.TANIMSIZ;
        /// <summary>
        /// Belge Numarası
        /// </summary>
        public string belgeNo = "";
        /// <summary>
        /// İşlem yapan kullanıcı
        /// </summary>
        public string islemYapan = "";
        /// <summary>
        /// İşlem yapan kullanıcı Ad
        /// </summary>
        public string islemYapanAd = "";
        /// <summary>
        /// İşlem tarihi
        /// </summary>
        public TNSDateTime islemTarih = new TNSDateTime();
        /// <summary>
        /// İşlem
        /// </summary>
        public string islem = "";
    }

    /// <summary>
    /// Şablon oluşturma sırasında kullanılan içerik tür bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class SablonIcerikTur
    {
        /// <summary>
        /// Belge türü
        /// </summary>
        public ENUMBelgeTuru belgeTur;
        /// <summary>
        /// Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public SablonIcerikTur()
        {
            belgeTur = ENUMBelgeTuru.TANIMSIZ;
            kod = 0;
            ad = "";
        }
    }

    /// <summary>
    /// Şablon bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class Sablon
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod = 0;
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil = 0;
        /// <summary>
        /// Belge Tür
        /// </summary>
        public ENUMBelgeTuru belgeTur = ENUMBelgeTuru.TANIMSIZ;
        /// <summary>
        /// Bütçe Tür
        /// </summary>
        public string butceTur = "";
        /// <summary>
        /// İçerik tür
        /// </summary>
        public int icerikTur = 0;
        /// <summary>
        /// Adı
        /// </summary>
        public string ad = "";
        /// <summary>
        /// Durum
        /// </summary>
        public int durum = 0;
        /// <summary>
        /// Durum tarih
        /// </summary>
        public TNSDateTime durumTarih;
        /// <summary>
        /// İşlem yapan kullanıcı
        /// </summary>
        public string islemYapan = "";
        /// <summary>
        /// Görme yetkisi var mı?
        /// </summary>
        public int gormeYetkisi = 0;
        /// <summary>
        /// Kurum kodu
        /// </summary>
        public string kurum = "";
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebe = 0;
        /// <summary>
        /// Harcama Birim kodu
        /// </summary>
        public string birim = "";
        /// <summary>
        /// İlgili ad
        /// </summary>
        public string ilgiliAd = "";
        /// <summary>
        /// İlgili kimlik/Vergi No
        /// </summary>
        public string ilgiliNo = "";
        /// <summary>
        /// Vergi dairesi
        /// </summary>
        public string ilgiliVD = "";
        /// <summary>
        /// Banka adı
        /// </summary>
        public string ilgiliBankaAd = "";
        /// <summary>
        /// IBAN numarası
        /// </summary>
        public string ilgiliBankaNo = "";
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama = "";
        /// <summary>
        /// Detay satırları
        /// </summary>
        public ObjectArray detay = new ObjectArray();
        /// <summary>
        /// Kurumsal Kod
        /// </summary>
        public string kurKod = "";
        /// <summary>
        /// Kurumsal Kod
        /// </summary>
        public int organizasyonKodu = 0;
        /// <summary>
        /// Sorgulama sırasında detay kayıtların listelenmesi isteniyor mu?
        /// </summary>
        public bool detayVer = false;
    }

    /// <summary>
    /// Şablon detay bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class SablonDetay
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Sıra 
        /// </summary>
        public int sira;
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Hesap Kodu
        /// </summary>
        public string hesapKod;
        /// <summary>
        /// Tertip
        /// </summary>
        public Tertip tertip;
        /// <summary>
        /// Borç
        /// </summary>
        public double borc;
        /// <summary>
        /// Alacak
        /// </summary>
        public double alacak;
        /// <summary>
        /// Kesinti mi?
        /// </summary>
        public int kesinti;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Alt Hesap Kodu
        /// </summary>
        public string altHesapKod;
        /// <summary>
        /// Döviz adı
        /// </summary>
        public string doviz = "";
        /// <summary>
        /// Döviz karşılığı merkez bankası dönüşüm türü Alış, Efektif Alış, Satış, Efektif Satış
        /// </summary>
        public string dovizKurTur = "";
        /// <summary>
        /// Döviz borç miktarı
        /// </summary>
        public double dovizBorc = 0.0;
        /// <summary>
        /// Döviz alacak miktarı
        /// </summary>
        public double dovizAlacak = 0.0;
        /// <summary>
        /// Döviz kuru
        /// </summary>
        public decimal dovizKur = 0;
        /// <summary>
        /// Satır açıklama
        /// </summary>
        public string satirAciklama = "";
        public string detayBirim = "";

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public SablonDetay()
        {
            kod = 0;
            sira = 0;
            yil = 0;
            hesapKod = "";
            tertip = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABPEBOdenek);
            borc = 0;
            alacak = 0;
            kesinti = 0;
            aciklama = "";
            altHesapKod = "";
        }
    }

    /// <summary>
    /// ödenek durum sorgulamasında detay kriterlerin belirlenemesinde kullanılan liste
    /// </summary>
    public enum ENUMOdenekDurumDetay
    {
        /// <summary>
        /// Tertip bazında
        /// </summary>
        TERTIP = 1,
        /// <summary>
        /// Muhasebe bazında
        /// </summary>
        MUHASEBE = 2,
        /// <summary>
        /// Birim bazında
        /// </summary>
        BIRIM = 3,
        /// <summary>
        /// Konsolide
        /// </summary>
        KONSOLIDE = 4,
        EKO1BASAMAK = 5,
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = -1
    }

    /// <summary>
    /// ödenek durum raporu kriterlerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class OdenekDurumKriter
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Ay
        /// </summary>
        public int ay;
        /// <summary>
        /// Tertip
        /// </summary>
        public Tertip tertip;
        /// <summary>
        /// Muhasebe
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim
        /// </summary>
        public string birim;
        /// <summary>
        /// Konsolide
        /// </summary>
        public bool konsolide;
        /// <summary>
        /// Detay
        /// </summary>
        public ENUMOdenekDurumDetay detay;

        /// <summary>
        /// Harcama gösterilsin mi?
        /// </summary>
        public bool harcamaGoster;

        public bool harcamaPebli = false;

        /// <summary>
        /// Harcama gösterilsin mi?
        /// </summary>
        public bool harcamaBlokeGoster;

        /// <summary>
        /// Ekonomik kod kırılım sayısı
        /// </summary>
        public int ekoKodKirilim;
        public bool eko23 = false;
        /// <summary>
        /// Ödenek gösterilsin mi?
        /// </summary>
        public bool odenekGoster;
        /// <summary>
        /// Ödenek Gönderme gösterilsin mi?
        /// </summary>
        public bool odenekGondermeGoster;
        /// <summary>
        /// Tenkis gösterilsin mi?
        /// </summary>
        public bool tenkisGoster;
        /// <summary>
        ///  Ödeme Emri gösterilsin mi?
        /// </summary>
        public bool odemeEmriGoster;
        /// <summary>
        /// MİF gösterilsin mi?
        /// </summary>
        public bool mifGoster;
        /// <summary>
        /// Gelir gösterilsin mi?
        /// </summary>
        public bool gelirGoster;
        /// <summary>
        /// Faaliyet kod gösterilsin mi? (PebKod)
        /// </summary>
        public bool faaliyetGoster;

        /// <summary>
        /// Avans bilgileri gösterilsin mi?
        /// </summary>
        public bool avansGoster;

        public bool yeniMetot = true;

        public TNSDateTime sorguTarih1 = new TNSDateTime();
        public TNSDateTime sorguTarih2 = new TNSDateTime();
        public int durum;
        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public OdenekDurumKriter()
        {
            yil = 0;
            ay = 0;
            tertip = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABPEBOdenek);
            muhasebe = 0;
            birim = "";
            konsolide = false;
            detay = ENUMOdenekDurumDetay.TANIMSIZ;
            odenekGoster = true;
            harcamaGoster = false;
            harcamaBlokeGoster = false;
            ekoKodKirilim = 2;
            odenekGondermeGoster = false;
            tenkisGoster = false;
            odemeEmriGoster = false;
            mifGoster = false;
            gelirGoster = false;
            faaliyetGoster = false;
            avansGoster = false;
            durum = 0;
        }

        public bool ekoAdBul { get; set; } = false;
    }

    /// <summary>
    /// Ödenek gönderme durum bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class OdenekGondermeDurum
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Belge Tür
        /// </summary>
        public int belgeTur;
        /// <summary>
        /// Form durum
        /// </summary>
        public int formDurum;
        /// <summary>
        /// tertip
        /// </summary>
        public Tertip tertip;
        /// <summary>
        /// Muhasebe Kodu
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Harcama birim Kodu
        /// </summary>
        public string birim;
        /// <summary>
        /// Miktar
        /// </summary>
        public double miktar;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public OdenekGondermeDurum()
        {
            yil = 0;
            belgeTur = 0;
            formDurum = 0;
            tertip = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABPEBOdenek);
            muhasebe = 0;
            birim = "";
            miktar = 0;
        }
    }

    /// <summary>
    /// HarcamaDurum bilgileri taşıyan nesne
    /// </summary>
    [Serializable()]
    public class HarcamaDurum
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Tertip
        /// </summary>
        public Tertip tertip;
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Harcama Birim kodu
        /// </summary>
        public string birim;
        /// <summary>
        /// Harcama rakamı
        /// </summary>
        public double harcama;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public HarcamaDurum()
        {
            yil = 0;
            tertip = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABPEBOdenek);
            muhasebe = 0;
            birim = "";
            harcama = 0;
        }
    }

    /// <summary>
    /// ButceDurum bilgisi taşıyan nesne
    /// </summary>
    [Serializable()]
    public class ButceDurum
    {
        /// <summary>
        /// Bütçe Yılı
        /// </summary>
        public int yil;
        /// <summary>
        /// Tertip
        /// </summary>
        public Tertip tertip;
        /// <summary>
        /// Kanun ile verilen ödenek
        /// </summary>
        public double kanun;
        /// <summary>
        /// Kesintili başlangıç ödeneği
        /// </summary>
        public double kbo;
        /// <summary>
        /// Yıl içinde eklenen miktar
        /// </summary>
        public double eklenen;
        /// <summary>
        /// Yıl içinde Düşülen miktar
        /// </summary>
        public double dusulen;
        /// <summary>
        /// Toplam ödenek
        /// </summary>
        public double toplamOdenek;
        /// <summary>
        /// Serbest
        /// </summary>
        public double serbest;
        /// <summary>
        /// Bloke
        /// </summary>
        public double bloke;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public ButceDurum()
        {
            yil = 0;
            tertip = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABPEBOdenek);
            kanun = 0;
            kbo = 0;
            eklenen = 0;
            dusulen = 0;
            toplamOdenek = 0;
            serbest = 0;
            bloke = 0;
        }
    }

    /// <summary>
    /// OdenekDurum bilgisi taşıyan nesne
    /// </summary>
    [Serializable()]
    public class OdenekDurum : IComparable
    {
        static string serbestMPdenStr = null;
        static bool _serbestMPden = false;
        public static bool SerbestMPden
        {
            get
            {
                if (serbestMPdenStr == null)
                {
                    serbestMPdenStr = TNS.KYM.Arac.Tanimla().WebConfigOku("SerbestMPden", "0");
                    _serbestMPden = serbestMPdenStr == "1";
                }
                return _serbestMPden;
            }
        }


        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Bütçe tertibi
        /// </summary>
        public Tertip tertipButce;
        /// <summary>
        /// Ödeme emri tertibi
        /// </summary>
        public Tertip tertipOdemeEmri;
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Muhasebe Adı
        /// </summary>
        public string muhasebeAd;
        /// <summary>
        /// Birim Kodu
        /// </summary>
        public string birim;
        /// <summary>
        /// Birim Adı
        /// </summary>
        public string birimAd;
        /// <summary>
        /// Kanun ödeneği
        /// </summary>
        public double kanun;
        /// <summary>
        /// KBÖ
        /// </summary>
        public double kbo;
        /// <summary>
        /// Ekelenen ödenek miktarı
        /// </summary>
        public double eklenen;
        /// <summary>
        /// Düşülen ödenek miktarı
        /// </summary>
        public double dusulen;
        /// <summary>
        /// Toplam Ödenek miktarı
        /// </summary>
        public double toplamOdenek;
        /// <summary>
        /// Serbest ödenek miktarı
        /// </summary>
        public double serbest;
        public double ahpSerbest;
        public double serbestBul
        {
            get
            {
                if (SerbestMPden)
                    return ahpSerbest;
                else
                    return serbest;
            }
        }

        /// <summary>
        /// Bloke ödenek miktarı
        /// </summary>
        public double bloke;
        public double ahpBloke;
        public double blokeBul
        {
            get { if (SerbestMPden) return ahpBloke; else return bloke; }
        }

        /// <summary>
        /// Harcama tutarı
        /// </summary>
        public double harcama;
        /// <summary>
        /// Avans miktarı
        /// </summary>
        public double avans;
        /// <summary>
        /// Bloke edilen ödenek
        /// </summary>
        public double blokeEdilen;
        /// <summary>
        /// Onaysız ödenek gönderme miktarı
        /// </summary>
        public double onaysizOdenekGonderme;
        /// <summary>
        /// Onaylı ödenek gönderme miktarı
        /// </summary>
        public double onayliOdenekGonderme;
        /// <summary>
        /// Onaysız tenkis miktarı
        /// </summary>
        public double onaysizTenkis;
        /// <summary>
        /// Onaylı tenkis miktarı
        /// </summary>
        public double onayliTenkis;
        /// <summary>
        /// Onaysız ödeme emri tutarı
        /// </summary>
        public double onaysizOdemeEmri;
        /// <summary>
        /// Onaysız ödeme emri tutarı
        /// </summary>
        public double onayliOdemeEmri;
        /// <summary>
        /// Onaylı ödeme tutarı
        /// </summary>
        public double onaysizMIF;
        /// <summary>
        /// Onaysız MİF tutarı
        /// </summary>
        public double onayliMIF;

        /// <summary>
        ///  Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public OdenekDurum()
        {
            yil = 0;
            tertipButce = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABPEBOdenek);
            tertipOdemeEmri = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);
            muhasebe = 0;
            muhasebeAd = "";
            birim = "";
            birimAd = "";
            kanun = 0;
            kbo = 0;
            eklenen = 0;
            dusulen = 0;
            toplamOdenek = 0;
            serbest = 0;
            ahpSerbest = 0;
            ahpBloke = 0;
            bloke = 0;
            harcama = 0;
            avans = 0;
            blokeEdilen = 0;
            onaysizOdenekGonderme = 0;
            onayliOdenekGonderme = 0;
            onaysizTenkis = 0;
            onayliTenkis = 0;
            onaysizOdemeEmri = 0;
            onayliOdemeEmri = 0;
            onaysizMIF = 0;
            onayliMIF = 0;
        }

        public static int eko2Uz;
        public static int eko3Uz;

        public string HashKod
        {
            get
            {
                string s = "";
                if (muhasebe != 0)
                {
                    s += muhasebe.ToString("00000") + "-";
                    if (!string.IsNullOrEmpty(birim))
                        s += birim + "-";
                }

                string kod = tertipButce.kodlar[0] + "-" + tertipButce.kodlar[1] + "-" + tertipButce.kodlar[2] + "-" + TNS.BKD.Arac.Eko23Dondur(yil, tertipButce.kodlar[3], eko2Uz, eko3Uz);
                kod += "-" + tertipButce.kodlar[4] + "-" + tertipButce.kodlar[3];

                s += kod;
                return s;
            }
        }

        public int CompareTo(object obj)
        {
            if (obj is OdenekDurum)
            {
                OdenekDurum ic = (OdenekDurum)obj;
                return HashKod.CompareTo(ic.HashKod);
            }
            return 0;
        }

        public string MuhasebeAd(Kullanici kullanan, UZY.IUZYServis uzyServis)
        {
            this.muhasebeAd = uzyServis.UzayDegeriStr(kullanan, "MUHMUHASEBE", this.muhasebe.ToString(), true, "MUHASEBEAD");
            return this.muhasebeAd;
        }

        public string BirimAd(Kullanici kullanan, UZY.IUZYServis uzyServis)
        {
            this.birimAd = uzyServis.UzayDegeriStr(kullanan, "MUHMUHASEBEBIRIM", this.birim + "-" + this.muhasebe.ToString(), true, "BIRIMAD");
            return this.birimAd;
        }
    }

    /// <summary>
    /// ButceDurumu bilgileri taşıyan nesne
    /// </summary>
    [Serializable()]
    public class ButceDurumu
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Kurumsal kod
        /// </summary>
        public string kurKod;
        /// <summary>
        /// Kurumsal ad
        /// </summary>
        public string kurAd;
        /// <summary>
        /// Fonksiyonel kod
        /// </summary>
        public string fonKod;
        /// <summary>
        /// Fonksiyonel ad
        /// </summary>
        public string fonAd;
        /// <summary>
        /// Finans kodu
        /// </summary>
        public string finKod;
        /// <summary>
        /// Finans adı
        /// </summary>
        public string finAd;
        /// <summary>
        /// Ekonomik kod
        /// </summary>
        public string ekoKod;
        /// <summary>
        /// Ekonomik ad
        /// </summary>
        public string ekoAd;
        /// <summary>
        /// Kesintili başlangıç ödeneği
        /// </summary>
        public double kbo;
        /// <summary>
        /// Serbest bırakılan ödenek miktarı
        /// </summary>
        public double serbest;
        /// <summary>
        /// Yıl içinde Eklenen ödenek
        /// </summary>
        public double eklenen;
        /// <summary>
        /// Yıl içinde düşülen ödenek
        /// </summary>
        public double dusulen;
        /// <summary>
        /// Harcama miktarı
        /// </summary>
        public double harcama;
        /// <summary>
        /// Onaylanmış ödenek gönderme miktarı
        /// </summary>
        public double onayliOdenekGonderme;
        /// <summary>
        /// Onaysız ödenek gönderme tutarı
        /// </summary>
        public double onaysizOdenekGonderme;
        /// <summary>
        /// Onaylanmış tenkis miktarı
        /// </summary>
        public double onayliTenkis;
        /// <summary>
        /// Onaysız tenkis miktarı
        /// </summary>
        public double onaysizTenkis;
        /// <summary>
        /// Bütçe muhasebe
        /// </summary>
        public ObjectArray butceMuhasebe;
        /// <summary>
        /// Bütçe işlem
        /// </summary>
        public ObjectArray butceIslem;
        /// <summary>
        /// Bütçe ödenek bilgilerini göster
        /// </summary>
        public bool butceGetir;
        /// <summary>
        /// Harcama bilgilerini göster
        /// </summary>
        public bool harcamaGetir;
        /// <summary>
        /// Ödenek gönderme bilgilerini göster
        /// </summary>
        public bool odenekGodermeGetir;
        /// <summary>
        /// Muhasebe bilgilerini göster
        /// </summary>
        public bool muhasebeGetir;
        /// <summary>
        /// Bütçe işlemlerini göster
        /// </summary>
        public bool butceIslemGetir;
        /// <summary>
        /// İlgili ay bilgisi
        /// </summary>
        public int ay;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public ButceDurumu()
        {
            ay = -1;
            yil = 0;
            kurKod = "";
            kurAd = "";
            fonKod = "";
            fonAd = "";
            finKod = "";
            finAd = "";
            ekoKod = "";
            ekoAd = "";
            kbo = 0.0;
            serbest = 0.0;
            eklenen = 0.0;
            dusulen = 0.0;
            harcama = 0.0;
            butceMuhasebe = new ObjectArray();
            butceIslem = new ObjectArray();
            butceGetir = false;
            harcamaGetir = false;
            odenekGodermeGetir = false;
            muhasebeGetir = false;
            butceIslemGetir = false;
        }
    }

    /// <summary>
    /// Muhasebe bazında işlem yapılan işlemleri taşıyan nesne
    /// </summary>
    [Serializable()]
    public class ButceMuhasebe
    {
        /// <summary>
        /// Yıl bilgisi
        /// </summary>
        public int yil;
        /// <summary>
        /// Ay bilgisi
        /// </summary>
        public int ay;
        /// <summary>
        /// Kurum kodu
        /// </summary>
        public string kurumKod;
        /// <summary>
        /// Kurumsal kod
        /// </summary>
        public string kurKod;
        /// <summary>
        /// Fonksiyonel kod
        /// </summary>
        public string fonKod;
        /// <summary>
        /// Finans kodu
        /// </summary>
        public string finKod;
        /// <summary>
        /// Ekonomik kod
        /// </summary>
        public string ekoKod;
        /// <summary>
        /// Hesap kodu
        /// </summary>
        public string hesapKod;
        /// <summary>
        /// Borç bilgisi
        /// </summary>
        public double borc;
        /// <summary>
        /// Alacak bilgisi
        /// </summary>
        public double alacak;

        /// <summary>
        /// Birim kodu
        /// </summary>
        public string birim;

        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebe;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public ButceMuhasebe()
        {
            yil = 0;
            ay = 0;
            kurumKod = "";
            kurKod = "";
            fonKod = "";
            finKod = "";
            ekoKod = "";
            hesapKod = "";
            borc = 0.0;
            alacak = 0.0;
            birim = "";
            muhasebe = 0;
        }
    }

    /// <summary>
    /// Yıl içinde yapılan bütçe işlem bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class ButceIslem
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Kurumsal kod
        /// </summary>
        public string kurKod;
        /// <summary>
        /// Fonksiyonel kod
        /// </summary>
        public string fonKod;
        /// <summary>
        /// Finans kodu
        /// </summary>
        public string finKod;
        /// <summary>
        /// Ekonomik kod
        /// </summary>
        public string ekoKod;
        /// <summary>
        /// İşlem kodu
        /// </summary>
        public string islemKod;
        /// <summary>
        /// Ekelenen tutar
        /// </summary>
        public double eklenen;
        /// <summary>
        /// Düşülen tutar
        /// </summary>
        public double dusulen;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public ButceIslem()
        {
            yil = 0;
            kurKod = "";
            fonKod = "";
            finKod = "";
            ekoKod = "";
            islemKod = "";
            eklenen = 0.0;
            dusulen = 0.0;
        }
    }

    [Serializable()]
    public class MIFEntegrasyon
    {
        public int yil = 0;
        public string belgeNo = "";
        public string ettn = "";
        public TNSDateTime tarih = new TNSDateTime();
        public string islemYapan = "";
        public int durum = 0;
    }

    /// <summary>
    /// GelirDurumu bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class GelirDurumu
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Harcama birim kodu
        /// </summary>
        public string birim;
        /// <summary>
        /// Kurumsal kod
        /// </summary>
        public string kurKod;
        /// <summary>
        /// Kurumsal kod adı
        /// </summary>
        public string kurAd;
        /// <summary>
        /// Ekonomik kod
        /// </summary>
        public string ekoKod;
        /// <summary>
        /// Ekonomik kod adı
        /// </summary>
        public string ekoAd;
        /// <summary>
        /// Planlanan gelir miktarı
        /// </summary>
        public double planlanan;
        /// <summary>
        /// Gerçekleşen gelir miktarı
        /// </summary>
        public double gerceklesen;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public GelirDurumu()
        {
            yil = 0;
            muhasebe = 0;
            birim = "";
            kurKod = "";
            kurAd = "";
            ekoKod = "";
            ekoAd = "";
            planlanan = 0.0;
            gerceklesen = 0.0;
        }
    }

    /// <summary>
    /// ButceGider bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class ButceGider
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Kurumsal kod
        /// </summary>
        public string kur;
        /// <summary>
        /// Fonksiyonel kod
        /// </summary>
        public string fon;
        /// <summary>
        /// Finans kodu
        /// </summary>
        public string fin;
        /// <summary>
        /// Ekonomik kod
        /// </summary>
        public string eko;
        /// <summary>
        /// Kanun ile verilen ödenek miktarı
        /// </summary>
        public double kanun;
        /// <summary>
        /// Kesintili başlangıç ödeneği
        /// </summary>
        public double kbo;
        /// <summary>
        /// Yıl içinde Eklenen tutar
        /// </summary>
        public double eklenen;
        /// <summary>
        /// Yıl içinde düşülen tutar
        /// </summary>
        public double dusulen;
        /// <summary>
        /// Serbest bırakılan tutar
        /// </summary>
        public double serbest;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public ButceGider()
        {
            yil = 0;
            kur = "";
            fon = "";
            fin = "";
            eko = "";
            kanun = 0.0;
            kbo = 0.0;
            eklenen = 0.0;
            dusulen = 0.0;
            serbest = 0.0;
        }
    }

    /// <summary>
    /// Finansman bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class ButceGelirFinansman
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Kurumsal kod
        /// </summary>
        public string kur;
        /// <summary>
        /// Ekonomik kod
        /// </summary>
        public string eko;
        /// <summary>
        /// Kanun
        /// </summary>
        public double kanun;
        /// <summary>
        /// Kesintili başlangıç ödeneği
        /// </summary>
        public double kbo;
        /// <summary>
        /// Eklenen
        /// </summary>
        public double eklenen;
        /// <summary>
        /// Düşülen
        /// </summary>
        public double dusulen;
        /// <summary>
        /// Serbest
        /// </summary>
        public double serbest;
        /// <summary>
        /// 
        /// </summary>
        public int tur;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public ButceGelirFinansman()
        {
            yil = 0;
            kur = "";
            eko = "";
            kanun = 0.0;
            kbo = 0.0;
            eklenen = 0.0;
            dusulen = 0.0;
            serbest = 0.0;
            tur = 0;
        }
    }

    /// <summary>
    /// Hesap satir bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class HesapSatir
    {
        /// <summary>
        /// Grup
        /// </summary>
        public string grup;
        /// <summary>
        /// Hesap kodu
        /// </summary>
        public string hesapNo;
        /// <summary>
        /// Hesap adı
        /// </summary>
        public string hesapAd;
        /// <summary>
        /// Borç ile alacak arasındaki fark
        /// </summary>
        public double borcAlacakFark;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="p_grup">Grup</param>
        /// <param name="p_hesapNo">Hesap no</param>
        /// <param name="p_hesapAd">hesap Adı</param>
        /// <param name="p_borcAlacakFark">Borç ile Alacak arasındaki fark</param>
        public HesapSatir(string p_grup, string p_hesapNo, string p_hesapAd, double p_borcAlacakFark)
        {
            grup = p_grup;
            hesapNo = p_hesapNo;
            hesapAd = p_hesapAd;
            borcAlacakFark = p_borcAlacakFark;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class HesapBilgi
    {
        /// <summary>
        /// Kalan hesapla.
        /// </summary>
        /// <param name="borc">Borç</param>
        /// <param name="alacak">Alacak</param>
        /// <param name="kalanBorc">Borç kalanı</param>
        /// <param name="kalanAlacak">Alacak kalanı</param>
        public static void KalanHesapla(double borc, double alacak, out double kalanBorc, out double kalanAlacak)
        {
            if (borc > alacak)
            {
                kalanBorc = borc - alacak;
                kalanAlacak = 0;
            }
            else
            {
                kalanAlacak = alacak - borc;
                kalanBorc = 0;
            }
        }

        public void NoktalariAt()
        {
            kur = kur.Replace(".", "");
            fon = fon.Replace(".", "");
            fin = fin.Replace(".", "");
            eko = eko.Replace(".", "");
            peb = peb.Replace(".", "");
        }

        /// <summary>
        /// Hesap kodu
        /// </summary>
        public string hesapKod;
        public string altHesapKod;
        /// <summary>
        /// Hesap Adı
        /// </summary>
        public string hesapAd;
        /// <summary>
        /// Kurumsal kod
        /// </summary>
        public string kur;
        /// <summary>
        /// Fonksiyonel kod
        /// </summary>
        public string fon;
        /// <summary>
        /// Finans kodu
        /// </summary>
        public string fin;
        /// <summary>
        /// Ekonomik kod
        /// </summary>
        public string eko;
        /// <summary>
        /// Ekonomik kod
        /// </summary>
        public string peb;
        /// <summary>
        /// Borç
        /// </summary>
        public double borc;
        /// <summary>
        /// Alacak
        /// </summary>
        public double alacak;
        /// <summary>
        /// Borç Kalanı
        /// </summary>
        public double kalanBorc;
        /// <summary>
        /// Alacak Kalanı
        /// </summary>
        public double kalanAlacak;
        /// <summary>
        /// BirimKod
        /// </summary>
        public string birimKod;

        public bool TamMi
        {
            get
            {
                if (kur.Length == 8 && fon.Length == 6 && fin.Length == 2 && eko.Length == 8) return true;
                return false;
            }
        }

        public string TertipStr
        {
            get
            {
                string tertip = kur + "-" + fon + "-" + fin + "-" + eko;
                if (peb != "")
                    tertip += "-" + peb;

                return tertip;
            }
        }


        public HesapBilgi()
        {
            hesapKod = "";
            hesapAd = "";
            kur = "";
            fon = "";
            fin = "";
            eko = "";
            peb = "";
            borc = 0;
            alacak = 0;
            altHesapKod = "";
            kalanBorc = 0;
            kalanAlacak = 0;
            birimKod = "";
        }

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="p_hesapKod">Hesap Kodu</param>
        /// <param name="p_hesapAd">hesap Adı</param>
        /// <param name="p_borc">Borç</param>
        /// <param name="p_alacak">Alacask</param>
        /// <param name="p_kur">Kurumsal kod</param>
        /// <param name="p_fon">Fonksiyonel kod</param>
        /// <param name="p_fin">Finans kodu</param>
        /// <param name="p_eko">Ekonomik kod</param>
        public HesapBilgi(string p_hesapKod, string p_hesapAd, double p_borc, double p_alacak, string p_kur, string p_fon, string p_fin, string p_eko, string p_peb, string p_altHesapKod, string p_birimKod)
        {
            Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABPEBHarcama);

            hesapKod = p_hesapKod;
            hesapAd = p_hesapAd;
            if (!string.IsNullOrEmpty(p_kur)) t.kodlar[0] = p_kur;
            if (!string.IsNullOrEmpty(p_fon)) t.kodlar[1] = p_fon;
            if (!string.IsNullOrEmpty(p_fin)) t.kodlar[2] = p_fin;
            if (!string.IsNullOrEmpty(p_eko)) t.kodlar[3] = p_eko;
            if (!string.IsNullOrEmpty(p_peb)) t.kodlar[4] = p_peb;
            t.Noktala();

            kur = t.kodlar[0];
            fon = t.kodlar[1];
            fin = t.kodlar[2];
            eko = t.kodlar[3];
            peb = t.kodlar[4];
            borc = p_borc;
            alacak = p_alacak;
            altHesapKod = p_altHesapKod;
            birimKod = p_birimKod;
            HesapBilgi.KalanHesapla(borc, alacak, out kalanBorc, out kalanAlacak);
        }
    }

    /// <summary>
    /// AnaHesap bilgisi taşıyan nesne
    /// </summary>
    [Serializable()]
    public class AnaHesap
    {
        /// <summary>
        /// Hesap kodu
        /// </summary>
        public string hesapKod;
        /// <summary>
        /// Hesap adı
        /// </summary>
        public string hesapAd;
        /// <summary>
        /// Kurumsal kod
        /// </summary>
        public string kur;
        /// <summary>
        /// Fonksiyonel kod
        /// </summary>
        public string fon;
        /// <summary>
        /// Finans kodu
        /// </summary>
        public string fin;
        /// <summary>
        /// Ekonomik kod
        /// </summary>
        public string eko;
        /// <summary>
        /// Peb kod
        /// </summary>
        public string peb;
        /// <summary>
        /// Borç toplamı
        /// </summary>
        public double borcToplam;
        /// <summary>
        /// Alacak toplamı
        /// </summary>
        public double alacakToplam;
        /// <summary>
        /// Borç kalanı
        /// </summary>
        public double kalanBorcToplam;
        /// <summary>
        /// Alacak kalanı
        /// </summary>
        public double kalanAlacakToplam;
        /// <summary>
        /// detay hesap bilgileri
        /// </summary>
        public ObjectArray detayHesaplar;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="p_hesapKod">Hesap kodu</param>
        /// <param name="p_hesapAd">Hesap Adı</param>
        /// <param name="p_borc">Borç</param>
        /// <param name="p_alacak">Alacak</param>
        /// <param name="p_kur">Kurumsal kod</param>
        /// <param name="p_fon">Fonksiyonel kod</param>
        /// <param name="p_fin">Finans kodu</param>
        /// <param name="p_eko">Ekonomik kod</param>
        /// <param name="p_peb">PEB kod</param>
        public AnaHesap(string p_hesapKod, string p_hesapAd, double p_borc, double p_alacak, string p_kur, string p_fon, string p_fin, string p_eko, string p_peb)
        {
            hesapKod = p_hesapKod;
            hesapAd = p_hesapAd;
            kur = p_kur;
            fon = p_fon;
            fin = p_fin;
            eko = p_eko;
            peb = p_peb;
            borcToplam = p_borc;
            alacakToplam = p_alacak;
            HesapBilgi.KalanHesapla(borcToplam, alacakToplam, out kalanBorcToplam, out kalanAlacakToplam);
            detayHesaplar = new ObjectArray();
        }
    }

    /// <summary>
    /// Mizan raporu bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class MizanBilgi
    {
        /// <summary>
        /// Sonraki sayfanın alacak toplamı
        /// </summary>
        public double oncBorc;
        /// <summary>
        /// Önceki sayfanın alacak toplamı
        /// </summary>
        public double oncAlacak;
        /// <summary>
        /// Ana hesap bilgileri taşıyan nesne
        /// </summary>
        public ObjectArray anaHesaplar;
        /// <summary>
        /// <see cref="Sonuc"/> liste sonuç bilgisini tutan özellik
        /// </summary>
        public Sonuc sonuc;
        /// <summary>
        /// 
        /// </summary>
        private AnaHesap simdAnaHesap;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="p_oncBorc">p_onc borc</param>
        /// <param name="p_oncAlacak">p_onc alacak</param>
        public MizanBilgi(double p_oncBorc, double p_oncAlacak)
        {
            oncBorc = p_oncBorc;
            oncAlacak = p_oncAlacak;
            anaHesaplar = new ObjectArray();
            sonuc = new Sonuc();
            simdAnaHesap = null;
        }

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="p_hesapKod">Hesap kod</param>
        /// <param name="p_hesapAd">Hesap Ad</param>
        /// <param name="p_borc">Borç</param>
        /// <param name="p_alacak">Alacak</param>
        /// <param name="p_kur">Kurumsal kod</param>
        /// <param name="p_fon">Fonksiyonel kod</param>
        /// <param name="p_fin">Finans Kod</param>
        /// <param name="p_eko">Ekonomik kod</param>
        public void DetayEkle(string p_hesapKod, string p_hesapAd, double p_borc, double p_alacak, string p_kur, string p_fon, string p_fin, string p_eko, string p_peb, string p_altHesapKod, string p_birimKod)
        {
            if (simdAnaHesap != null)
            {
                simdAnaHesap.borcToplam += p_borc;
                simdAnaHesap.alacakToplam += p_alacak;
                HesapBilgi hb = new HesapBilgi(p_hesapKod, p_hesapAd, p_borc, p_alacak, p_kur, p_fon, p_fin, p_eko, p_peb, p_altHesapKod, p_birimKod);
                simdAnaHesap.detayHesaplar.objeler.Add(hb);

                double bakiye = simdAnaHesap.borcToplam - simdAnaHesap.alacakToplam;
                if (bakiye > 0)
                {
                    simdAnaHesap.kalanBorcToplam = bakiye;
                    simdAnaHesap.kalanAlacakToplam = 0;
                }
                else
                {
                    simdAnaHesap.kalanBorcToplam = 0;
                    simdAnaHesap.kalanAlacakToplam = Math.Abs(bakiye);
                }

            }
        }

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="p_hesapKod">Hesap kodu</param>
        /// <param name="p_hesapAd">Hesap adı</param>
        public void AnaHesapEkle(string p_hesapKod, string p_hesapAd)
        {
            simdAnaHesap = new AnaHesap(p_hesapKod, p_hesapAd, 0, 0, "", "", "", "", "");
            anaHesaplar.objeler.Add(simdAnaHesap);
        }
    }

    /// <summary>
    /// Makbuz açıklama
    /// </summary>
    [Serializable()]
    public class MakbuzAciklama
    {
        public int kod = 0;
        public string birimKod = "";
        public string aciklama = "";
        public int makbuzTipiKod = 0;
    }

    /// <summary>
    /// MakbuzTipi bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class MakbuzTipi
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod { get; set; }
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama { get; set; }
        /// <summary>
        /// Hesap kod
        /// </summary>
        public string hesapKod { get; set; }
        /// <summary>
        /// KDV
        /// </summary>
        public int kdv { get; set; }
        /// <summary>
        /// KDV Hesap kodu
        /// </summary>
        public string kdvHesapKod { get; set; }
        /// <summary>
        /// Kasa hesap kodu
        /// </summary>
        public string kasaHesapKod { get; set; }
        /// <summary>
        /// Yansitma hesap kodu
        /// </summary>
        public string yansitmaHesapKod { get; set; }

        public string krediHesapKod { get; set; }

        public int durum { get; set; }
        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public MakbuzTipi()
        {
            kod = 0;
            aciklama = string.Empty;
            hesapKod = string.Empty;
            kdv = -1;
            kdvHesapKod = string.Empty;
            kasaHesapKod = string.Empty;
            yansitmaHesapKod = string.Empty;
            krediHesapKod = string.Empty;
            durum = 0;
        }

    }

    /// <summary>
    /// Makbuz bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class Makbuz
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil { get; set; }
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebe { get; set; }
        /// <summary>
        /// Muhasebe adı
        /// </summary>
        public string muhasebeAd { get; set; }
        /// <summary>
        /// Birim kodu
        /// </summary>
        public string birim { get; set; }
        /// <summary>
        /// Birim adı
        /// </summary>
        public string birimAd { get; set; }
        /// <summary>
        /// Yevmiye numarası
        /// </summary>
        public string yevmiyeNo { get; set; }
        /// <summary>
        /// Makbuz tipi bilgisi taşıyan nesne
        /// </summary>
        public MakbuzTipi makbuzTipi { get; set; }
        /// <summary>
        /// İlgili kişi adı
        /// </summary>
        public string kisiAd { get; set; }
        /// <summary>
        /// Tutar
        /// </summary>
        public double makbuzTipiTutar { get; set; }
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama { get; set; }
        /// <summary>
        /// Düzenleyen adı
        /// </summary>
        public string duzenleyen { get; set; }
        /// <summary>
        /// Düzenleyen Ünvanı
        /// </summary>
        public string unvan { get; set; }
        /// <summary>
        /// İşlem tarihi
        /// </summary>
        public TNSDateTime islemTarih { get; set; }
        public TNSDateTime odemeTarih { get; set; }
        public DateTime islemTarihDT { get { return islemTarih.Oku(); } }

        public TNSDateTime araBasTarih { get; set; }
        public TNSDateTime araBitTarih { get; set; }

        public bool kisiBilgisiTamBul = false;

        public string durumAd
        {
            get
            {
                if (durum == 1) return "Yeni";
                if (durum == 3) return "Ödendi";
                if (durum == 5) return "Onaylı";
                if (durum == 9) return "İptal";
                return "";
            }
        }
        /// <summary>
        /// Durum
        /// </summary>
        public int durum { get; set; }
        /// <summary>
        /// Ödeme emri no
        /// </summary>
        public string odemeEmriNo { get; set; }
        /// <summary>
        /// Kullanıcı kodu
        /// </summary>
        public string kullaniciKodu { get; set; }
        /// <summary>
        /// Kullanılan hesap kodları taşıyan nesne
        /// </summary>
        public TNSCollection hesaplar { get; set; }

        public string makbuzTipiAciklama { get { return makbuzTipi.aciklama; } }

        public double krediKartiIleOdenen { get; set; }

        public string sorguHesapKod { get; set; }

        public string paraBirimiKod { get; set; }

        public double genelToplam
        {
            get
            {
                double genelToplam = 0;
                for (int i = 0; i < hesaplar.Count; i++)
                {
                    MakbuzHesap mh = (MakbuzHesap)hesaplar[i];
                    if (mh.hesaplananAlan > 0) continue;
                    genelToplam += mh.tutar;
                }
                genelToplam += makbuzTipiTutar;

                return genelToplam;
            }
        }
        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Makbuz()
        {
            Sifirla();
        }

        private void Sifirla()
        {
            yil = 0;
            muhasebe = 0;
            muhasebeAd = string.Empty;
            birim = string.Empty;
            birimAd = string.Empty;
            yevmiyeNo = string.Empty;
            makbuzTipi = new MakbuzTipi();
            kisiAd = string.Empty;
            makbuzTipiTutar = (double)0;
            aciklama = string.Empty;
            duzenleyen = string.Empty;
            unvan = string.Empty;
            islemTarih = new TNSDateTime();
            odemeTarih = new TNSDateTime();
            araBasTarih = new TNSDateTime();
            araBitTarih = new TNSDateTime();
            durum = 0; //int) ENUMBelgeDurumu.YENI;
            odemeEmriNo = string.Empty;
            kullaniciKodu = string.Empty;
            hesaplar = new TNSCollection();
            krediKartiIleOdenen = -1;
            paraBirimiKod = "";
        }

        public Makbuz(int yil, int muhasebe, string makbuzNo)
        {
            Sifirla();
            this.yil = yil;
            this.muhasebe = muhasebe;
            this.yevmiyeNo = makbuzNo;
        }

        public string tcKimlikNoVergiNo { get; set; }

        public void HesapSatirEkle(int toplamSatir)
        {
            for (int i = hesaplar.Count; i < toplamSatir; i++)
            {
                hesaplar.Add(new MakbuzHesap());
            }
        }
    }

    /// <summary>
    /// Makbuzda kullanılan hesap bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class MakbuzHesap
    {
        /// <summary>
        /// Hesap kodu
        /// </summary>
        public string hesapKod { get; set; }
        /// <summary>
        /// Hesap kodu
        /// </summary>
        public string hesapAd { get; set; }
        /// <summary>
        /// Tutar
        /// </summary>
        public double tutar { get; set; }

        /// <summary>
        /// Makbuz üst bilgisinde bulunan kasa, kredi gibi hesaplanan alan mı?
        /// </summary>
        public int hesaplananAlan { get; set; }

        /// <summary>
        /// Makbuz üst bilgisinde alt hesap kodu
        /// </summary>
        public string altHesap { get; set; }

        /// <summary>
        /// Makbuz üst bilgisinde alt hesap adı
        /// </summary>
        public string altHesapAd { get; set; }

        /// <summary>
        /// The tertip.
        /// </summary>
        public string tertip { get; set; }
        //public string paraBirimiKod { get; set; }

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public MakbuzHesap()
        {
            hesapKod = string.Empty;
            hesapAd = string.Empty;
            tutar = (double)0;
            hesaplananAlan = 0;
            altHesap = string.Empty;
            altHesapAd = string.Empty;
            tertip = string.Empty;
            //paraBirimiKod = "";
        }
    }

    /// <summary>
    /// Avans bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class AvansBilgi
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim kodu
        /// </summary>
        public string birim;
        /// <summary>
        /// TC Kimlik Numarası
        /// </summary>
        public string kimlikNo;
        /// <summary>
        /// Adı soyadı
        /// </summary>
        public string adSoyad;
        /// <summary>
        /// Borç tutarı
        /// </summary>
        public double borc;
        /// <summary>
        /// Alacak tutarı
        /// </summary>
        public double alacak;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Belge numarası
        /// </summary>
        public string belgeNo;
        /// <summary>
        /// Muhasebe Adı
        /// </summary>
        public string muhasebeAd;
        /// <summary>
        /// Birim adı
        /// </summary>
        public string birimAd;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public AvansBilgi()
        {
            yil = 0;
            muhasebe = 0;
            birim = "";
            kimlikNo = "";
            adSoyad = "";
            borc = 0.0;
            alacak = 0.0;
            aciklama = "";
            belgeNo = "";
            muhasebeAd = "";
            birimAd = "";
        }
    }

    /// <summary>
    /// Ödenek ihtiyaci bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class OdenekIhtiyac
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Harcama birim kodu
        /// </summary>
        public string birim;
        /// <summary>
        /// Kurumsal kod
        /// </summary>
        public string kurKod;
        /// <summary>
        /// Fonksiyonel kod
        /// </summary>
        public string fonKod;
        /// <summary>
        /// Finans kodu
        /// </summary>
        public string finKod;
        /// <summary>
        /// Ekonomik kod
        /// </summary>
        public string ekoKod;
        /// <summary>
        /// Kurumsal adı
        /// </summary>
        public string kurAd;
        /// <summary>
        /// Ekonomik adı
        /// </summary>
        public string ekoAd;
        /// <summary>
        /// harcama1
        /// </summary>
        public double harcama1;
        /// <summary>
        /// harcama2
        /// </summary>
        public double harcama2;
        /// <summary>
        /// harcama3
        /// </summary>
        public double harcama3;
        /// <summary>
        /// 1.Dönem Ödenek talep rakamı
        /// </summary>
        public double odenek1;
        /// <summary>
        /// 2.Dönem Ödenek talep rakamı
        /// </summary>
        public double odenek2;
        /// <summary>
        /// 3.Dönem Ödenek talep rakamı
        /// </summary>
        public double odenek3;
        /// <summary>
        /// 4.Dönem Ödenek talep rakamı
        /// </summary>
        public double odenek4;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Kesintili Başlangıç ödeneği
        /// </summary>
        public double kbo;
        /// <summary>
        /// Ocak harcaması
        /// </summary>
        public double harcamaAy1;
        /// <summary>
        /// Şubat harcaması
        /// </summary>
        public double harcamaAy2;
        /// <summary>
        /// Mart harcaması
        /// </summary>
        public double harcamaAy3;
        /// <summary>
        /// Nisan harcaması
        /// </summary>
        public double harcamaAy4;
        /// <summary>
        /// Mayıs harcaması
        /// </summary>
        public double harcamaAy5;
        /// <summary>
        /// Haziran Harcaması
        /// </summary>
        public double harcamaAy6;
        /// <summary>
        /// Temmuz harcaması
        /// </summary>
        public double harcamaAy7;
        /// <summary>
        /// Ağustos harcaması
        /// </summary>
        public double harcamaAy8;
        /// <summary>
        /// Eylül harcaması
        /// </summary>
        public double harcamaAy9;
        /// <summary>
        /// Ekim harcaması
        /// </summary>
        public double harcamaAy10;
        /// <summary>
        /// Kasım harcaması
        /// </summary>
        public double harcamaAy11;
        /// <summary>
        /// Aralık harcaması
        /// </summary>
        public double harcamaAy12;
        /// <summary>
        /// Yıl-2 harcaması
        /// </summary>
        public double harcamaYil1;
        /// <summary>
        /// Yıl-1 harcaması
        /// </summary>
        public double harcamaYil2;
        /// <summary>
        /// Yıl harcaması
        /// </summary>
        public double harcamaYil3;
        /// <summary>
        /// İlk kayıt mı?
        /// </summary>
        public bool ilkKayit;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public OdenekIhtiyac()
        {
            yil = 0;
            muhasebe = 0;
            birim = "";
            kurKod = "";
            fonKod = "";
            finKod = "";
            ekoKod = "";
            kurAd = "";
            ekoAd = "";
            harcama1 = 0.0;
            harcama2 = 0.0;
            harcama3 = 0.0;
            odenek1 = 0.0;
            odenek2 = 0.0;
            odenek3 = 0.0;
            odenek4 = 0.0;
            aciklama = "";
            kbo = 0.0;
            harcamaAy1 = 0.0;
            harcamaAy2 = 0.0;
            harcamaAy3 = 0.0;
            harcamaAy4 = 0.0;
            harcamaAy5 = 0.0;
            harcamaAy6 = 0.0;
            harcamaAy7 = 0.0;
            harcamaAy8 = 0.0;
            harcamaAy9 = 0.0;
            harcamaAy10 = 0.0;
            harcamaAy11 = 0.0;
            harcamaAy12 = 0.0;
            harcamaYil1 = 0.0;
            harcamaYil2 = 0.0;
            harcamaYil3 = 0.0;
            ilkKayit = false;
        }
    }

    /// <summary>
    /// Ödenek ihtiyaç onay bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class OdenekIhtiyacOnay
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Aşama
        /// </summary>
        public int asama;
        /// <summary>
        /// Onay
        /// </summary>
        public int onay;
        /// <summary>
        /// Kayıt tarihi
        /// </summary>
        public TNSDateTime kayitTarihi;
        /// <summary>
        /// İşlem yapan kullanıcı
        /// </summary>
        public string kayitEden;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public OdenekIhtiyacOnay()
        {
            yil = 0;
            muhasebe = 0;
            asama = 0;
            onay = 0;
            kayitTarihi = new TNSDateTime();
            kayitEden = "";
        }
    }

    /// <summary>
    /// Hesin Hesap (Bütçe) raporu bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class KesinHesapButce
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Hesap kodu
        /// </summary>
        public string hesapKod;
        /// <summary>
        /// Bütçe kodu
        /// </summary>
        public string kod;
        /// <summary>
        /// Bütçe kod adı
        /// </summary>
        public string kodAd;
        /// <summary>
        /// Yıl içindeki tutar
        /// </summary>
        public double deger1;
        /// <summary>
        /// Yıl-1 içindeki tutar
        /// </summary>
        public double deger2;
        /// <summary>
        /// Yıl-2 içindeki tutar
        /// </summary>
        public double deger3;
        /// <summary>
        /// Yıl açılış tutarı
        /// </summary>
        public double degerAcilis1;
        /// <summary>
        /// Yıl-1 açılış tutarı
        /// </summary>
        public double degerAcilis2;
        /// <summary>
        /// Yıl-2 açılış tutarı
        /// </summary>
        public double degerAcilis3;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public KesinHesapButce()
        {
            yil = 0;
            hesapKod = "";
            kod = "";
            kodAd = "";
            deger1 = 0.0;
            deger2 = 0.0;
            deger3 = 0.0;
            degerAcilis1 = 0.0;
            degerAcilis2 = 0.0;
            degerAcilis3 = 0.0;
        }

        public void DegerAta(int alanNo, double deger)
        {
            if (alanNo == 1)
                this.deger1 = deger;
            else if (alanNo == 2)
                this.deger2 = deger;
            else if (alanNo == 3)
                this.deger3 = deger;
        }
    }

    /// <summary>
    /// Büyük defter raporu bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class BuyukDefter
    {
        /// <summary>
        /// Yil
        /// </summary>
        public int yil = 0;
        /// <summary>
        /// Belge Tür
        /// </summary>
        public int belgeTur = 0;
        /// <summary>
        /// İşlem tarihi
        /// </summary>
        public TNS.TNSDateTime islemTarih = new TNSDateTime();
        /// <summary>
        /// Belge numarası
        /// </summary>
        public string belgeNo = "";
        /// <summary>
        /// Kurum kodu
        /// </summary>
        public string kurum = "";
        /// <summary>
        /// Kurum adı
        /// </summary>
        public string kurumAd = "";
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebe = 0;
        /// <summary>
        /// Muhasebe adı
        /// </summary>
        public string muhasebeAd = "";
        /// <summary>
        /// Harcama birim kodu
        /// </summary>
        public string birim = "";
        /// <summary>
        /// Harcama birim adı
        /// </summary>
        public string birimAd = "";
        /// <summary>
        /// İlgili kişi adı
        /// </summary>
        public string ilgiliAd = "";
        /// <summary>
        /// İlgili kişi adı
        /// </summary>
        public string ilgiliNo = "";
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama = "";
        /// <summary>
        /// Yevmiye tarihi
        /// </summary>
        public TNS.TNSDateTime YEVYevTarih = new TNSDateTime();
        /// <summary>
        /// Yevmiye numarası
        /// </summary>
        public int YEVYevNo = 0;
        /// <summary>
        /// Form durumu
        /// </summary>
        public int formDurum = 0;
        /// <summary>
        /// Form durum tarihi
        /// </summary>
        public TNS.TNSDateTime formDurumTarih = new TNSDateTime();
        /// <summary>
        /// İşlem Yapan kullanıcı
        /// </summary>
        public string islemYapan = "";
        /// <summary>
        /// satır no
        /// </summary>
        public int satirno = 0;
        /// <summary>
        /// hesap kodu
        /// </summary>
        public string hesapKod = "";
        /// <summary>
        /// Kurumsal kod
        /// </summary>
        public string kur = "";
        /// <summary>
        /// Fonksiyonel kod
        /// </summary>
        public string fon = "";
        /// <summary>
        /// Finans kodu
        /// </summary>
        public string fin = "";
        /// <summary>
        /// Ekonomik kod
        /// </summary>
        public string eko = "";
        /// <summary>
        /// Borç tutarı
        /// </summary>
        public double borc = 0.0;
        /// <summary>
        /// Alacak tutarı
        /// </summary>
        public double alacak = 0.0;
        /// <summary>
        /// bakiye
        /// </summary>
        public double bakiye = 0.0;
        /// <summary>
        /// hesap adı
        /// </summary>
        public string hesapAd = "";
        public string altHesap = "";
        public string altHesapAd = "";
        public string detayBirim = "";
        public string kapattigiBelgeNo { get; set; }

        public double borcDoviz = 0.0;
        public double alacakDoviz = 0.0;
        public int prgKod;
        public int aPrgKod;
        public int faalKod;
        public int aFaalKod;
    }

    /// <summary>
    /// Nakit talebi bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class NakitTalebi
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Ay
        /// </summary>
        public int ay;
        /// <summary>
        /// teşkilat
        /// </summary>
        public int teskilat;
        /// <summary>
        /// Harcama birim kodu
        /// </summary>
        public string birim;
        /// <summary>
        /// Aşama
        /// </summary>
        public int asama;
        /// <summary>
        /// Hafta bilgisi
        /// </summary>
        public int hafta;
        /// <summary>
        /// Ekonomik kod
        /// </summary>
        public string ekoKod;
        /// <summary>
        /// Tutar
        /// </summary>
        public double tutar;
        /// <summary>
        /// İşlem yapan kullanıcı
        /// </summary>
        public string kullaniciKodu;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public NakitTalebi()
        {
            yil = 0;
            ay = 0;
            teskilat = 0;
            birim = "";
            asama = 0;
            hafta = 0;
            ekoKod = "";
            tutar = 0.0;
            kullaniciKodu = ""; ;
        }
    }

    /// <summary>
    /// Aylık dağılım bilgisi taşıyan nesne
    /// </summary>
    [Serializable()]
    public class AylikDagilim
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil = 0;
        /// <summary>
        /// Ay
        /// </summary>
        public int ay = 0;
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebe = 0;
        /// <summary>
        /// Harcama birimi
        /// </summary>
        public string birim = "";
        /// <summary>
        /// tertip
        /// </summary>
        public Tertip tertip = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdenek);
        public string hesapKod = "";
        public string hesapAd = "";
        /// <summary>
        /// Tutar
        /// </summary>
        public double tutar = 0.0;
    }

    /// <summary>
    /// Talep bilgilerini taşıyan nesne
    /// </summary>
    [Serializable]
    public class TalepBelge
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// talep numarası
        /// </summary>
        public int talepNo;
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// harcama Birim kodu
        /// </summary>
        public string birimKod;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Talep tarihi
        /// </summary>
        public TNSDateTime talepTarih;
        /// <summary>
        /// Talep eden kullanıcı kodu
        /// </summary>
        public string talepKullaniciKod;
        /// <summary>
        /// talep formu durumu
        /// </summary>
        public int formDurum;
        /// <summary>
        /// Talep formu tarihi
        /// </summary>
        public TNSDateTime formDurumTarih;
        /// <summary>
        /// talep karşılığında oluşturulan ödenek gönderme belge numarası
        /// </summary>
        public string odenekGondermeBelgeNo;
        /// <summary>
        /// Süreç Yılı
        /// </summary>
        public int surecYil;
        /// <summary>
        /// Süreç Numarası
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Karşılayan kullanıcı kodu
        /// </summary>
        public string karsilayanKullaniciKod;
        /// <summary>
        /// Karşılama tarihi
        /// </summary>
        public TNSDateTime karsilananTarih;
        /// <summary>
        /// Detay bilgilerini taşıyan nesne
        /// </summary>
        public ObjectArray detay;
        /// <summary>
        /// tarihçe bilgilerini taşıyan nesne 
        /// </summary>
        public ObjectArray tarihce;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TalepBelge()
        {
            yil = 0;
            talepNo = 0;
            muhasebeKod = 0;
            birimKod = string.Empty;
            aciklama = string.Empty;
            talepTarih = new TNSDateTime();
            talepKullaniciKod = string.Empty;
            formDurum = 0;
            formDurumTarih = new TNSDateTime();
            odenekGondermeBelgeNo = string.Empty;
            surecYil = 0;
            surecNo = string.Empty;
            karsilayanKullaniciKod = string.Empty;
            karsilananTarih = new TNSDateTime();
            detay = new ObjectArray();
            tarihce = new ObjectArray();
        }
    }

    /// <summary>
    /// talepDetay bilgilerini taşıyan nesne
    /// </summary>
    [Serializable]
    public class TalepDetay
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// talep Numarası
        /// </summary>
        public int talepNo;
        /// <summary>
        /// Sıra Numarası
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Kurumsal kod
        /// </summary>
        public string kurKod;
        /// <summary>
        /// Fonksiyonel kod
        /// </summary>
        public string fonKod;
        /// <summary>
        /// Finans kodu
        /// </summary>
        public string finKod;
        /// <summary>
        /// Ekonomik kod
        /// </summary>
        public string ekoKod;
        /// <summary>
        /// Performans kodu
        /// </summary>
        public string pebKod;
        /// <summary>
        /// talep tutarı
        /// </summary>
        public decimal talepTutar;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string talepAciklama;
        /// <summary>
        /// Karşılanan tutar
        /// </summary>
        public decimal karsilananTutar;
        /// <summary>
        /// Karşılama sırasında yazılan açıklama
        /// </summary>
        public string karsilananAciklama;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TalepDetay()
        {
            yil = 0;
            talepNo = 0;
            siraNo = 0;
            kurKod = string.Empty;
            fonKod = string.Empty;
            finKod = string.Empty;
            ekoKod = string.Empty;
            pebKod = string.Empty;
            talepTutar = 0;
            talepAciklama = string.Empty;
            karsilananTutar = 0;
            karsilananAciklama = string.Empty;
        }

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="pYil">p yil</param>
        /// <param name="pTalepNo">p talep no</param>
        public TalepDetay(int pYil, int pTalepNo)
        {
            yil = pYil;
            talepNo = pTalepNo;
            siraNo = 0;
            kurKod = string.Empty;
            fonKod = string.Empty;
            finKod = string.Empty;
            ekoKod = string.Empty;
            pebKod = string.Empty;
            talepTutar = 0;
            talepAciklama = string.Empty;
            karsilananTutar = 0;
            karsilananAciklama = string.Empty;
        }
    }

    /// <summary>
    /// Talep tarihçe bilgilerini taşıyan nesne
    /// </summary>
    [Serializable]
    public class TalepTarihce
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Talep numarası
        /// </summary>
        public int talepNo;
        /// <summary>
        /// Sıra no
        /// </summary>
        public int siraNo;
        /// <summary>
        /// İşlem Tarihi
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// İşlem yapan kullanıcı
        /// </summary>
        public string islemYapan;
        /// <summary>
        /// Yapılan işlem
        /// </summary>
        public int islem;

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TalepTarihce()
        {
            yil = 0;
            talepNo = 0;
            siraNo = 0;
            tarih = new TNSDateTime();
            islemYapan = string.Empty;
            islem = 0;
        }

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="pYil">p yil</param>
        /// <param name="pTalepNo">p talep no</param>
        public TalepTarihce(int pYil, int pTalepNo)
        {
            yil = pYil;
            talepNo = pTalepNo;
            siraNo = 0;
            tarih = new TNSDateTime();
            islemYapan = string.Empty;
            islem = 0;
        }
    }


    /// <summary>
    /// Kapatılmak istenilen hesap bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class KapanisBilgileri
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil = 0;
        /// <summary>
        /// belgeNo
        /// </summary>
        public string belgeNo = "";
        /// <summary>
        /// sirano
        /// </summary>
        public int siraNo = 0;
        /// <summary>
        /// kYil
        /// </summary>
        public int kYil = 0;
        /// <summary>
        /// kBelgeNo
        /// </summary>
        public string kBelgeNo = "";
        /// <summary>
        /// kHesapKod
        /// </summary>
        public string kHesapKod = "";
        /// <summary>
        /// kIlgiliVno
        /// </summary>
        public string kIlgiliVno = "";
        /// <summary>
        /// kTertip
        /// </summary>
        public string kTertip = "";
        /// <summary>
        /// kTutar
        /// </summary>
        public double kTutar = 0.0;
        /// <summary>
        /// borc/alacak
        /// </summary>
        public int borcAlacak = 0;
        public string kBirimKod = "";

    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ISLHarcama
    {
        /// <summary>
        /// 
        /// </summary>
        public int yil = 0;
        /// <summary>
        /// 
        /// </summary>
        public int muhasebeKod = 0;
        /// <summary>
        /// 
        /// </summary>
        public string muhasebeAd = "";
        /// <summary>
        /// 
        /// </summary>
        public string birimKod = "";
        /// <summary>
        /// 
        /// </summary>
        public string birimAd = "";
        /// <summary>
        /// 
        /// </summary>
        public string kurKod = "";
        /// <summary>
        /// 
        /// </summary>
        public string fonKod = "";
        /// <summary>
        /// 
        /// </summary>
        public string finKod = "";
        /// <summary>
        /// 
        /// </summary>
        public string ekoKod = "";
        /// <summary>
        /// 
        /// </summary>
        public ObjectArray aylar = new ObjectArray();
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Ay
    {
        /// <summary>
        /// 
        /// </summary>
        public int id = 0;
        /// <summary>
        /// 
        /// </summary>
        public double miktar = 0.0;
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ButceOdenekBilgi
    {
        /// <summary>
        /// 
        /// </summary>
        public int yil = 0;
        /// <summary>
        /// 
        /// </summary>
        public int muhasebeKod = 0;
        /// <summary>
        /// 
        /// </summary>
        public string muhasebeAd = "";
        /// <summary>
        /// 
        /// </summary>
        public string birimKod = "";
        /// <summary>
        /// 
        /// </summary>
        public string birimAd = "";
        /// <summary>
        /// 
        /// </summary>
        public string hesapKod = "";
        /// <summary>
        /// 
        /// </summary>
        public string kurKod = "";
        /// <summary>
        /// 
        /// </summary>
        public string fonKod = "";
        /// <summary>
        /// 
        /// </summary>
        public string finKod = "";
        /// <summary>
        /// 
        /// </summary>
        public string ekoKod = "";
        /// <summary>
        /// 
        /// </summary>
        public string pebKod = "";
        /// <summary>
        /// 
        /// </summary>
        public double odenek = 0.0;
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class YilSonuIslem
    {
        /// <summary>
        /// 
        /// </summary>
        public int yil = 0;
        /// <summary>
        /// 
        /// </summary>
        public int muhasebe = 0;
        /// <summary>
        /// 
        /// </summary>
        public string birim = "";
        /// <summary>
        /// 
        /// </summary>
        public string belgeNo = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public int tur = 0;
        /// <summary>
        /// 
        /// </summary>
        public string ad = "";
        /// <summary>
        /// 
        /// </summary>
        public string aciklama = "";

    }


    //TJE***************************************
    //[System.Serializable()]
    //public class AccountDetail
    //{
    //    /// <summary>
    //    /// accCode: Account Code
    //    /// </summary>
    //    public string accCode;

    //    /// <summary>
    //    /// bsSeq: Budget Submission Sequence No
    //    /// </summary>
    //    public int bsSeq;

    //    /// <summary>
    //    /// eco: Economic Code
    //    /// </summary>
    //    public string eco;

    //    public AccountDetail(string accCode, int bsSeq, string eco)
    //    {
    //        this.accCode = accCode;
    //        this.bsSeq = bsSeq;
    //        this.eco = eco;
    //    }
    //}

    ///// <summary>
    ///// Journal Entry Line Class
    ///// </summary>
    //[System.Serializable()]
    //public class JournalEntryDC
    //{
    //    /// <summary>
    //    /// debit account codes
    //    /// </summary>
    //    public AccountDetail debitAcc;

    //    /// <summary>
    //    /// credit account codes
    //    /// </summary>
    //    public AccountDetail creditAcc;

    //    /// <summary>
    //    /// ttr: TTR
    //    /// </summary>
    //    public int ttrD;

    //    /// <summary>
    //    /// ttr: TTR
    //    /// </summary>
    //    public int ttrC;

    //    /// <summary>
    //    /// Amount: Amount
    //    /// </summary>
    //    public double amount;

    //    public JournalEntryDC(AccountDetail debitAcc, AccountDetail creditAcc, int ttrD, int ttrC, double amount)
    //    {
    //        this.debitAcc = debitAcc;
    //        this.creditAcc = creditAcc;
    //        this.amount = amount;
    //        this.ttrD = ttrD;
    //        this.ttrC = ttrC;
    //    }

    //    public JournalEntryDC()
    //    {
    //    }
    //}

    ///// <summary>
    ///// MUHMifForm: 
    ///// </summary>
    //[System.Serializable()]
    //public class JournalEntryFormDC
    //{

    //    /// <summary>
    //    /// Yıl: Yılı giriniz
    //    /// </summary>
    //    public int year;

    //    /// <summary>
    //    /// Form No: Form numarası giriniz
    //    /// </summary>
    //    public string formNo;

    //    /// <summary>
    //    /// Birim: Birim
    //    /// </summary>
    //    public string mabaCode;

    //    /// <summary>
    //    /// Account Office Code: Accounting Office
    //    /// </summary>
    //    public int accountOfficeCode;

    //    /// <summary>
    //    /// Expenditure Unit Code: Expenditure Unit Code
    //    /// </summary>
    //    public string expUnitCode;

    //    /// <summary>
    //    /// Description: Description
    //    /// </summary>
    //    public string description;

    //    /// <summary>
    //    /// Created By: Created By
    //    /// </summary>
    //    public string createdBy;

    //    /// <summary>
    //    /// Creation Date: Creation Date
    //    /// </summary>
    //    public TNSDateTime creationDate;

    //    /// <summary>
    //    /// Created By: Created By
    //    /// </summary>
    //    public string changedBy;

    //    /// <summary>
    //    /// Change Date: Change Date
    //    /// </summary>
    //    public TNSDateTime changeDate;

    //    /// <summary>
    //    /// Approve By: Approved By
    //    /// </summary>
    //    public string approvedBy;

    //    /// <summary>
    //    /// Approve Date: Approve Date
    //    /// </summary>
    //    public TNSDateTime approveDate;

    //    /// <summary>
    //    /// Exp Op Year: Exp Op Year
    //    /// </summary>
    //    public int expOpYear;

    //    /// <summary>
    //    /// Exp Op No: Exp Op No
    //    /// </summary>
    //    public string expOpNo;

    //    /// <summary>
    //    /// Oper ID: Oper ID
    //    /// </summary>
    //    public string operID;

    //    /// <summary>
    //    /// Oper Date: Oper Date
    //    /// </summary>
    //    public TNSDateTime operDate;

    //    /// <summary>
    //    /// Doc. Type: Document Type
    //    /// </summary>
    //    public string docType;

    //    /// <summary>
    //    /// Doc. ID: Document ID
    //    /// </summary>
    //    public string docID;

    //    /// <summary>
    //    /// trBo: treasury or Butgert Org
    //    /// </summary>
    //    public int trBo;

    //    /// <summary>
    //    /// MuhMif: Journal Entries
    //    /// </summary>
    //    public JournalEntryDC[] journalLines;
    //}
    //TJE***************************************


    public enum ENUMBankaListesiTur
    {
        OESecim = 1,
        HarcamaSurecUretti = 2
    }

    public enum ENUMBankaListesiDurum
    {
        Yeni = 0,
        Degismis = 1,
        Onayli = 5,
        Iptal = 9
    }

    public enum ENUMBankaListesiDetayDurum
    {
        Yeni = 0,
        Odendi = 1,
        Iptal = 9
    }

    /// <summary>
    /// MUHBankaOdemeDetay: İlgili ödeme satırları
    /// </summary>
    [System.Serializable()]
    public class MUHBankaOdemeDetay
    {

        /// <summary>
        /// #: Order No
        /// </summary>
        public int sira;

        /// <summary>
        /// Muhasebe Belgesi Yılı: Muhasebe Belgesi Yılı
        /// </summary>
        public int mifYil;

        /// <summary>
        /// Muhasebe Belgesi No: Muhasebe Belgesi No
        /// </summary>
        public string mifBelgeNo;

        public int mifYevmiyeNo;
        public TNSDateTime odemeEmriIslemTarih;


        /// <summary>
        /// Muhasebe Belgesi No: Muhasebe Belgesi No
        /// </summary>
        public int mifSira;

        /// <summary>
        /// Hesap Kodu: Hesap Kodu
        /// </summary>
        public string muhHesapKodu;

        /// <summary>
        /// Sözleşme No: Sözleşme No
        /// </summary>
        public string sozlesmeNo;

        /// <summary>
        /// İlgili: İlgili
        /// </summary>
        public string ilgili;

        /// <summary>
        /// TC No/Vergi No: TC No/Vergi No
        /// </summary>
        public string tc_Vno;

        /// <summary>
        /// İlgili Banka: İlgilinin Bankası
        /// </summary>
        public string ilgBanka;

        /// <summary>
        /// İlgili IBAN: İlgilinin IBAN numarası
        /// </summary>
        public string ilgIBAN;

        /// <summary>
        /// Tutar: Ödenecek Tutar
        /// </summary>
        public double tutar;

        /// <summary>
        /// Tutar: Para Birimi
        /// </summary>
        public string doviz;

        /// <summary>
        /// Tutar: Açıklama
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Durum: 0- Yeni, 1-Ödendi, 9-Ödenmedi
        /// </summary>
        public ENUMBankaListesiDetayDurum durum;

        /// <summary>
        /// Dekont No: Ödendi ise bankanın dekont numarası
        /// </summary>
        public string dekontNo;

        /// <summary>
        /// Gelen dekont için yapılan muhasebe işleminin numarası
        /// </summary>
        public string odemeEmriNo;

        public string cekNo { get; set; }

        public bool IlgiliAyniMi(MUHBankaOdemeDetay ilgiliAyni)
        {
            if (this.tc_Vno == ilgiliAyni.tc_Vno && this.ilgIBAN.Replace(" ", "") == ilgiliAyni.ilgIBAN.Replace(" ", ""))
                return true;
            else
                return false;
        }
        private string _yevmiyeNoStr = null;
        public string birimKod;

        public string yevmiyeNoStr
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_yevmiyeNoStr))
                    return this.mifYevmiyeNo.ToString();
                else
                    return _yevmiyeNoStr;
            }
            set
            {
                _yevmiyeNoStr = value;
            }
        }

        public void Birlestir(MUHBankaOdemeDetay ilgiliAyni)
        {
            if (!string.IsNullOrWhiteSpace(this.aciklama)) this.aciklama += ", ";
            this.aciklama += ilgiliAyni.aciklama;
            this.tutar += ilgiliAyni.tutar;
            this.mifBelgeNo += ", " + ilgiliAyni.mifBelgeNo;
            this.yevmiyeNoStr += ", " + ilgiliAyni.yevmiyeNoStr;
        }
    }

    /// <summary>
    /// MUHBankaOdeme: 
    /// </summary>
    [System.Serializable()]
    public class MUHBankaOdeme
    {
        public int saymanlik;

        public string birim;
        /// <summary>
        /// Gün: Günü giriniz
        /// </summary>
        public TNSDateTime gun;

        /// <summary>
        /// Sıra No: GÜn içindeki sıra numarası giriniz
        /// </summary>
        public int siraNo;

        /// <summary>
        /// Açıklama: İşlem hakkında girilecek ek bilgiler.
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Kullanıcı Kodu: Kullanıcı Kodu
        /// </summary>
        public string kullaniciKod;

        /// <summary>
        /// Oluşturulma Zamanı: Oluşturulma Zamanı
        /// </summary>
        public TNSDateTime olusmaZamani;

        /// <summary>
        /// Değ. Kul. Kodu: Değiştiren Kullanıcı Kodu
        /// </summary>
        public string degistirenKod;

        /// <summary>
        /// Değiştirilme Tarihi: Değiştirilme Tarihi
        /// </summary>
        public TNSDateTime degismeZamani;

        /// <summary>
        /// Onaylayan Kul. Kodu: Onaylayan Kulllanıcı Kodu
        /// </summary>
        public string onaylayanKod;

        /// <summary>
        /// Onaylanma Zamanı: Onaylanma Zamanı
        /// </summary>
        public TNSDateTime onayZamani;

        /// <summary>
        /// Hazırlayan: Hazırlayan
        /// </summary>
        public string hazirlayan;

        /// <summary>
        /// Ödeme Tarihi
        /// </summary>
        public TNSDateTime odemeTarih;

        /// <summary>
        /// Muhasebe Yetkilisi: Muhasebe Yetkilisi
        /// </summary>
        public string muhYetkili;

        /// <summary>
        /// Muh. Yet. Unvanı: Muh. Yet. Unvanı
        /// </summary>
        public string muhYetUnvan;

        /// <summary>
        /// Onaylayan: Onaylayan
        /// </summary>
        public string onaylayan;

        /// <summary>
        /// Onaylayan Unvanı: Onaylayan Unvanı
        /// </summary>
        public string onayUnvan;

        /// <summary>
        /// Tür: 1-Elle oluşturuldu; 2-İlgili modül üretti(değiştirilemez)
        /// </summary>
        public ENUMBankaListesiTur tur;

        /// <summary>
        /// Durum: 0-Yeni; 1-Değişmiş; 5-Onaylı; 9-İptal
        /// </summary>
        public ENUMBankaListesiDurum durum;

        /// <summary>
        /// MUHBankaOdemeDetay: İlgili ödeme satırları
        /// </summary>
        public MUHBankaOdemeDetay[] muhBankaOdemeDetay;

        public string cekEmriNo { get; set; }
        public int donemNo { get; set; }

        public string bankaDosyaAd { get; set; }

        public string grupOku { get; set; }

        public TNSDateTime kapatmaTarihi { get; set; }
    }

    [Serializable]
    public class BankaBilgi
    {
        public string BankaAd;
        public string SubeAd;
        public string MBSubeNo;
        public string IBAN;
        public string ftpAd;
        public string musteriNo;
        public string hesapNo;

        public string musteriAd { get; set; }
    }

    public enum ENUMYuzdeOnTur
    {
        Mal = 1,
        Hizmet = 2,
        Yapim = 3,
        Tanimsiz = 0
    }

    [Serializable]
    public class YuzdeOnBilgi
    {
        public ENUMYuzdeOnTur tur;
        public bool birimSiniriMi;
        public double ustSinir;
        public double odenen;
        public TNS.BKD.TertipOdenekBilgi[] liste = null;
        public string saymanlikKodlar;
        public string birimKodlar;

        public string TurAd
        {
            get
            {
                return TNS.MUH.Arac.YuzdeOnKodBul(tur);
            }
        }
    }

    [Serializable]
    public class EksikOdenekBilgi
    {
        /// <summary>
        /// 
        /// </summary>
        public string kur = "";
        /// <summary>
        /// 
        /// </summary>
        public string fon = "";
        /// <summary>
        /// 
        /// </summary>
        public string fin = "";
        /// <summary>
        /// 
        /// </summary>
        public string eko = "";
        /// <summary>
        /// 
        /// </summary>
        public string peb = "";
        /// <summary>
        /// IHTIYAC
        /// </summary>
        public double IHTIYAC = 0;

        public double ODENEK = 0;

        public double HARCAMA = 0;

        public double AVANS = 0;

        public double BLOKE = 0;

        public string TertipStr()
        {
            string tertip = kur + "-" + fon + "-" + fin + "-" + eko;
            if (!string.IsNullOrEmpty(peb))
                tertip += "-" + peb;
            return tertip;
        }

        public double borc { get; set; }

        public string kurKont { get; set; }

        public string fonKont { get; set; }

        public string finKont { get; set; }

        public string ekoKont { get; set; }

        public string pebKont { get; set; }
    }

    [Serializable]
    public class EksikOdenekler
    {
        public Sonuc sonuc = null;
        public EksikOdenekBilgi[] eksikOdenekler = null;

        public string BilgiStr()
        {
            string retVal = "";
            if (eksikOdenekler != null)
            {
                foreach (EksikOdenekBilgi item in eksikOdenekler)
                {
                    if (retVal != "") retVal += "<br>";
                    retVal += item.TertipStr() + ": " + item.IHTIYAC;
                }
            }

            if (retVal != "")
                retVal = "Tertiplerdeki eksik ödenek miktarları:<br>" + retVal;

            return retVal;
        }

        public bool EksikOdenekVarMi()
        {
            return (eksikOdenekler != null && eksikOdenekler.Length > 0);
        }
    }

    /// <summary>
    /// BagliEkoPeb: BagliEkoPeb
    /// </summary>
    [System.Serializable()]
    public class BagliEkoPeb
    {

        /// <summary>
        /// eko: Ekonomik Kodu giriniz giriniz.
        /// </summary>
        public string tertip { get; set; }

        /// <summary>
        /// peb: Faaliyet Kodunu giriniz giriniz.
        /// </summary>
        public string peb { get; set; }
    }

    public class BankaHesapHareket
    {
        public virtual TNS.MUH.BankaHesapBilgi Listele(string musteriNo, string IBAN, string hesapNo, DateTime basla, DateTime bitis, string borcAlacakHepsi)
        {
            return null;
        }

        public static bool HesapNoIBANEsitMi(string hesapNo, string IBAN)
        {
            return Son8(hesapNo.Replace("-", "").Replace(" ", "")) == Son8(IBAN.Replace(" ", ""));
        }

        private static string Son8(string str)
        {
            int uz = str.Length;
            if (uz > 8)
            {
                string tut = str.Substring(uz - 8);
                return tut;
            }
            return str;
        }

        //public static void DebugYaz(string veri)
        //{
        //    OrtakFonksiyonlar.HataDosyaYaz("BankaHareket.txt", DateTime.Now.ToShortDateString() + ": " + veri, false);
        //}
    }

    /// <summary>
    /// BankaHareketAna: 
    /// </summary>
    [System.Serializable()]
    public class BankaHesapBilgi
    {
        private string _hesapAdi;
        /// <summary>
        /// Hesap Adı: Yılı giriniz.
        /// </summary>
        public string hesapAdi { get { return _hesapAdi; } set { _hesapAdi = value; } }

        private string _hesapNo;
        /// <summary>
        /// Hesap No: Yılı giriniz.
        /// </summary>
        public string hesapNo { get { return _hesapNo; } set { _hesapNo = value; } }

        private string _subeNo;
        /// <summary>
        /// Şube No: Yılı giriniz.
        /// </summary>
        public string subeNo { get { return _subeNo; } set { _subeNo = value; } }

        private string _subeAdi;
        /// <summary>
        /// Şube Adı: Yılı giriniz.
        /// </summary>
        public string subeAdi { get { return _subeAdi; } set { _subeAdi = value; } }

        /// <summary>
        /// KullaniciMesaj: Mesajlar
        /// </summary>
        public BankaHareket[] hareketler;

        public string hesapCinsi { get; set; }
    }

    public class BankaHareket
    {
        private TNSDateTime _islemTarihi;
        /// <summary>
        /// İşlem Tarihi: Harcama Birimi Kodunu Giriniz.
        /// </summary>
        public TNSDateTime islemTarihi { get { return _islemTarihi; } set { _islemTarihi = value; } }
        public DateTime islemTarihiDT
        {
            get
            {
                if (_islemTarihi == null)
                    return DateTime.MinValue;
                else
                    return _islemTarihi.Oku();
            }
            set
            {
                if (_islemTarihi == null) _islemTarihi = new TNSDateTime(value);
                else _islemTarihi.Yaz(value);
            }
        }

        private string _dekont_FisNo;
        /// <summary>
        /// Dekont/Fiş No: Yazı giriniz.
        /// </summary>
        public string dekont_FisNo { get { return _dekont_FisNo; } set { _dekont_FisNo = value; } }

        private string _aciklama;
        /// <summary>
        /// Açıklama: Yazı giriniz.
        /// </summary>
        public string aciklama { get { return _aciklama; } set { _aciklama = value; } }

        private double _islemTutari;
        /// <summary>
        /// İşlem Tutarı: Yazı giriniz.
        /// </summary>
        public double islemTutari { get { return _islemTutari; } set { _islemTutari = value; } }

        private string _paraCinsi;
        /// <summary>
        /// Para Cinsi: Yazı giriniz.
        /// </summary>
        public string paraCinsi { get { return _paraCinsi; } set { _paraCinsi = value; } }

        private double _yeniBakiye;
        /// <summary>
        /// Yeni Bakiye: Yazı giriniz.
        /// </summary>
        public double yeniBakiye { get { return _yeniBakiye; } set { _yeniBakiye = value; } }
    }

    /// <summary>
    /// Taahhut kartı bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class TaahhutKartiBilgi
    {
        public enum ENUMSozTur : int
        {
            Hepsi = 0,
            Normal = 1,
            Personel = 2
        }

        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebeKod;
        public string birimKod;
        /// <summary>
        /// Proje Sıra No
        /// </summary>
        public string projeSiraNo;
        /// <summary>
        /// DPT Proje Sıra No
        /// </summary>
        public string DptProjeSiraNo;
        /// <summary>
        ///Müteahhit Kimlik No
        /// </summary>
        public string muteahhitKimlikNo;
        /// <summary>
        /// Müteahhit Ünvan
        /// </summary>
        public string muteahhitUnvan;
        /// <summary>
        /// Müteahhit Adres
        /// </summary>
        public string muteahhitAdres;
        /// <summary>
        /// Konu
        /// </summary>
        public string konu;
        /// <summary>
        /// Keşif Bedeli
        /// </summary>
        public double kesifBeldeli;
        /// <summary>
        /// İndirim Oranı
        /// </summary>
        public double indirimOrani;
        /// <summary>
        /// İhale Bedeli
        /// </summary>
        public double ihaleBedeli;
        /// <summary>
        /// Gerçekleşen Taahhüt Tutarı
        /// </summary>
        public double gerceklesenTaahhutTutar;
        /// <summary>
        /// Damga Vergisi Tarihi
        /// </summary>
        public TNS.TNSDateTime damgaVergisiTarihi;
        /// <summary>
        /// Damga Vergisi Numarası
        /// </summary>
        public string damgaVergisiNo;
        /// <summary>
        /// Damga Vergisi Tutar
        /// </summary>
        public double damgaVergisiTutar;
        /// <summary>
        /// Sözleşme Damga Vergisi Tarihi
        /// </summary>
        public TNS.TNSDateTime sozlesmeDVTarihi;
        /// <summary>
        /// Sözleşme Damga Vergisi Numarası
        /// </summary>
        public string sozlesmeDVNo;
        /// <summary>
        /// SözleşmecDamga Vergisi Tutar
        /// </summary>
        public double sozlesmeDVTutar;
        /// <summary>
        /// Sözleşme Tarihi
        /// </summary>
        public TNS.TNSDateTime sozlesmeTarihi;
        /// <summary>
        /// İşe Başlama Tarihi
        /// </summary>
        public TNS.TNSDateTime iseBaslamaTarihi;
        /// <summary>
        /// Geçici Kabul Tarihi
        /// </summary>
        public TNS.TNSDateTime geciciKabulTarihi;
        /// <summary>
        /// İşi Bitirme Tarihi
        /// </summary>
        public TNS.TNSDateTime isBitirmeTarihi;
        /// <summary>
        /// Süre Uzatımı 1. değer
        /// </summary>
        public string sureUzatimi1;
        /// <summary>
        /// Süre Uzatımı 2. değer
        /// </summary>
        public string sureUzatimi2;
        /// <summary>
        /// Süre Uzatımı 3. değer
        /// </summary>
        public string sureUzatimi3;
        /// <summary>
        /// Ödenek Kurumsal Kodu
        /// </summary>
        public string odenekTertip;
        /// <summary>
        /// Tertip Hesap Kodu
        /// </summary>
        public string tertipHesapKod;
        /// <summary>
        /// Tertip Kurumsal Kodu
        /// </summary>
        public string duranTertip;
        /// <summary>
        /// İşlem Yapan Kodu
        /// </summary>
        public string islemYapanKod;
        /// <summary>
        /// İşlem Yapan Adı
        /// </summary>
        public string islemYapanAd;
        /// <summary>
        /// İşlem Tarihi
        /// </summary>
        public TNS.TNSDateTime islemTarihi;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama;
        public string alimUsuluKod = "";
        public int sgkBildirimi = 0;
        public int odemeEmriYil = 0;
        public string odemeEmriNo = "";
        //kapandı true ise 1 false ise 0
        public int kapandi = 0;
        public string parabirimiKod = "";
        public TNS.TNSDateTime garantiBaslamaTarihi;
        public TNS.TNSDateTime garantiBitisTarihi;

        public int isKod;

        public ObjectArray degerArtisi = new ObjectArray();
        public ENUMSozTur sozTur = ENUMSozTur.Hepsi;
        public int odemeEmriDurum;

        public double sorguTaahhutTutar = 0.0;
        public double sorguOdenenTutar = 0.0;
        public TNSDateTime arananTarih = null;

        public TNSDateTime sorguSozlesmeTarihBas = new TNSDateTime();
        public TNSDateTime sorguSozlesmeTarihBit = new TNSDateTime();

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TaahhutKartiBilgi()
        {
            yil = 0;
            kod = -1;
            muhasebeKod = 0;
            birimKod = "";
            projeSiraNo = "";
            DptProjeSiraNo = "";
            muteahhitKimlikNo = "";
            muteahhitUnvan = "";
            muteahhitAdres = "";
            konu = "";
            kesifBeldeli = 0.0;
            indirimOrani = 0.0;
            ihaleBedeli = 0.0;
            gerceklesenTaahhutTutar = 0.0;
            damgaVergisiTarihi = new TNSDateTime();
            damgaVergisiNo = "";
            damgaVergisiTutar = 0.0;
            sozlesmeDVTarihi = new TNSDateTime();
            sozlesmeDVNo = "";
            sozlesmeDVTutar = 0.0;
            sozlesmeTarihi = new TNSDateTime();
            iseBaslamaTarihi = new TNSDateTime();
            geciciKabulTarihi = new TNSDateTime();
            isBitirmeTarihi = new TNSDateTime();
            sureUzatimi1 = "";
            sureUzatimi2 = "";
            sureUzatimi3 = "";
            odenekTertip = "";
            tertipHesapKod = "";
            duranTertip = "";
            islemYapanAd = "";
            islemYapanKod = "";
            islemTarihi = new TNSDateTime();
            aciklama = "";
            garantiBaslamaTarihi = new TNSDateTime();
            garantiBitisTarihi = new TNSDateTime();
            isKod = 0;
            alimUsuluKod = "";
        }

        public bool OdemeEmriOnayliMi()
        {
            return (odemeEmriDurum > 2 && odemeEmriDurum < 9);
        }

        public bool VerilerTutarliMi(Sonuc sonuc)
        {
            if (this.yil == 0)
                sonuc.DegerAta("Yıl bilgisi boş geçilemez!<br>");

            if (this.muhasebeKod == 0)
                sonuc.DegerAta("Muhasebe kodu girilmelidir!<br>");

            if (string.IsNullOrWhiteSpace(this.birimKod))
                sonuc.DegerAta("Muhasebe birim kodu girilmelidir!<br>");

            if (string.IsNullOrWhiteSpace(this.konu))
                sonuc.DegerAta("Konu girilmelidir!<br>");

            if (this.sozTur != TaahhutKartiBilgi.ENUMSozTur.Personel)
            {
                if (this.sgkBildirimi == 0)
                    sonuc.DegerAta("SGK Bildirim bilgisi boş bırakılamaz<br>");

                if (TNSDateTime.IsNull(this.sozlesmeDVTarihi))
                    sonuc.DegerAta("Sözleşme damga vergisi tarihi boş geçilemez!<br>");
                //if (string.IsNullOrEmpty(this.sozlesmeDVNo))
                //    sonuc.DegerAta("Sözleşme damga vergisi numarası boş geçilemez!<br>");
                //if (this.sozlesmeDVTutar <= 0)
                //    sonuc.DegerAta("Sözleşme damga vergisi tutarı boş geçilemez!<br>");
                if (string.IsNullOrEmpty(this.odenekTertip))
                    sonuc.DegerAta("Ödeneğin tertibi boş geçilemez!<br>");

                if (sonuc.islemSonuc)
                {
                    TNS.MUH.ENUMOdenekKontrolTur kt = TNS.MUH.Arac.OdenekKontrolTurBul(this.yil);

                    ENUMTabloKod tertipTur = ENUMTabloKod.TABPEBHarcama;
                    if (kt == TNS.MUH.ENUMOdenekKontrolTur.E2OG_E4OE_E2K)
                        tertipTur = ENUMTabloKod.TABHarcama;

                    Tertip tertip = new Tertip(DateTime.Now.Year, tertipTur);

                    tertip.TertipAta(this.odenekTertip);
                    bool tertipTamMi = tertip.TamMi();
                    if (!tertipTamMi)
                        sonuc.DegerAta("Ödeneğin tertibi hatalı!<br>");
                }
            }

            if (TNSDateTime.IsNull(this.sozlesmeTarihi))
                sonuc.DegerAta("Sözleşme tarihi girilmelidir!<br>");

            if (this.ihaleBedeli <= 0)
                sonuc.DegerAta("İhale bedeli girilmelidir!<br>");

            if (TNSDateTime.IsNull(this.iseBaslamaTarihi))
                sonuc.DegerAta("İşe başlama tarihi girilmelidir!<br>");
            else if (TNSDateTime.IsNull(this.isBitirmeTarihi))
                sonuc.DegerAta("İşi bitirme tarihi girilmelidir!<br>");
            else if (this.isBitirmeTarihi.Oku() < this.iseBaslamaTarihi.Oku())
                sonuc.DegerAta("İşi bitirme tarihi, başlangıç tarihinden büyük olmalıdır!<br>");

            if (TNS.MUH.Arac.MuhasebeDovizVarMi() && string.IsNullOrWhiteSpace(this.parabirimiKod))
                sonuc.DegerAta("Para birimi boş geçilemez!<br>");

            if (string.IsNullOrWhiteSpace(this.aciklama))
                sonuc.DegerAta("Açıklama alanı girilmelidir!<br>");

            if (string.IsNullOrWhiteSpace(this.muteahhitKimlikNo))
                sonuc.DegerAta("Müteahhit kimlik no girilmelidir!<br>");

            if (string.IsNullOrEmpty(this.muteahhitUnvan))
                sonuc.DegerAta("Müteahhidin/Personelin Unvan alanı boş geçilemez!<br>");

            return sonuc.islemSonuc;
        }
    }

    /// <summary>
    /// Taahhut bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class TaahhutBilgi
    {
        public bool bosMu
        {
            get
            {
                return (kod <= 0 && yil == 0 && ay == 0 && string.IsNullOrWhiteSpace(tertipKod) && toplam == 0);
            }
        }

        /// <summary>
        /// Kod
        /// </summary>
        public int kod { get; set; } = -1;
        /// <summary>
        /// Taahhüt kodu
        /// </summary>
        public int taahhutKod { get; set; } = 0;
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil { get; set; } = 0;
        /// <summary>
        /// Yıl
        /// </summary>
        public int ay { get; set; } = 0;

        public double toplam { get { return ilkTaahhutTutari + fiyatFarki + ilaveTaahhutler + digerTaahhutler; } }
        /// <summary>
        ///İlk Taahhüt Tutarı
        /// </summary>
        public double ilkTaahhutTutari { get; set; } = 0.0;
        /// <summary>
        /// Fiyat Farkı
        /// </summary>
        public double fiyatFarki { get; set; } = 0.0;
        /// <summary>
        /// İlave Taahhütler
        /// </summary>
        public double ilaveTaahhutler { get; set; } = 0.0;
        /// <summary>
        /// Diğer Taahhütler
        /// </summary>
        public double digerTaahhutler { get; set; } = 0.0;
        /// <summary>
        /// İşlem Tarihi
        /// </summary>
        public TNS.TNSDateTime islemTarihi { get; set; } = new TNSDateTime();
        public DateTime islemTarihiDT { get { return islemTarihi.Oku(); } set { islemTarihi.Yaz(value); } }
        /// <summary>
        /// İşlem Yapan TC
        /// </summary>
        public string islemYapanKod { get; set; } = "";
        /// <summary>
        /// İşlem Yapan Adı
        /// </summary>
        public string islemYapanAd { get; set; } = "";
        public string tertipKod { get; set; } = "";
        public string degerArtisKod { get; set; } = null;

        public string muteahhitKimlikNo { get; set; } = null;
        public int MIFYil { get; set; } = 0;
        public string MIFNo { get; set; } = null;
        public int MIFDurum { get; set; } = 0;
    }

    /// <summary>
    /// Taahhut bilgilerini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class TaahhutKartiDegerArtisi
    {
        public string KOD { get { return kod; } set { kod = value; } }
        public int ODEMEEMRIYIL { get { return odemeEmriYil; } set { odemeEmriYil = value; } }
        public string ODEMEEMRINO { get { return odemeEmriNo; } set { odemeEmriNo = value; } }
        public DateTime ONAYTARIHI { get { return onayTarihi.Oku(); } set { onayTarihi.Yaz(value); } }
        public DateTime SOZLESMETARIHI { get { return sozlesmeTarihi.Oku(); } set { sozlesmeTarihi.Yaz(value); } }
        public double SOZLESMETUTAR { get { return sozlesmeTutari; } set { sozlesmeTutari = value; } }
        public DateTime DAMGAVERGISITARIHI { get { return damgaVergisiTarihi.Oku(); } set { damgaVergisiTarihi.Yaz(value); } }
        public string DAMGAVERGISINO { get { return damgaVergisiNo; } set { damgaVergisiNo = value; } }
        public double DAMGAVERGISITUTAR { get { return damgaVergisiTutari; } set { damgaVergisiTutari = value; } }

        public string kod = "";
        public int taahhutKod = 0;
        public TNSDateTime onayTarihi = new TNSDateTime();
        public string onayTarihiStr { set { onayTarihi.Yaz(value); } }
        public TNSDateTime sozlesmeTarihi = new TNSDateTime();
        public string sozlesmeTarihiStr { set { sozlesmeTarihi.Yaz(value); } }
        public double sozlesmeTutari = 0.0;
        public TNSDateTime damgaVergisiTarihi = new TNSDateTime();
        public string damgaVergisiTarihiStr { set { damgaVergisiTarihi.Yaz(value); } }
        public string damgaVergisiNo = "";
        public double damgaVergisiTutari = 0.0;
        public int odemeEmriYil = 0;
        public string odemeEmriNo = "";
        public TNS.TNSDateTime islemTarihi = new TNSDateTime();
        public string islemYapanKod = "";
        public string islemYapanAd = "";
        public int odemeEmriFormDurum;
    }
    /// <summary>
    /// Alınan teminatlar taşıyan nesne
    /// </summary>
    [Serializable()]
    public class TaahhutTeminatlar
    {
        /// <summary>
        /// Taahhüt kodu
        /// </summary>
        public int taahhutKod;
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        ///Dosya No
        /// </summary>
        public string dosyaNo;
        /// <summary>
        /// Tarih
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// Numara
        /// </summary>
        public string numara;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Tutar
        /// </summary>
        public double tutar;
        /// <summary>
        /// İşlem Tarihi
        /// </summary>
        public TNSDateTime islemTarihi;
        /// <summary>
        /// İşlem Yapan TC
        /// </summary>
        public string islemYapanKod;
        /// <summary>
        /// İşlem Yapan Adı
        /// </summary>
        public string islemYapanAd;
        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TaahhutTeminatlar()
        {
            taahhutKod = 0;
            yil = 0;
            dosyaNo = "";
            tarih = new TNSDateTime();
            numara = "";
            aciklama = "";
            tutar = 0.0;
            islemTarihi = new TNSDateTime();
            islemYapanKod = "";
            islemYapanAd = "";
        }

    }

    /// <summary>
    /// VerilenAvanslarVeMahsuplar taşıyan nesne
    /// </summary>
    [Serializable()]
    public class TaahhutAvanslar
    {
        /// <summary>
        /// Taahhüt kodu
        /// </summary>
        public int taahhutKod;
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        ///Belge No
        /// </summary>
        public string belgeNo;
        /// <summary>
        /// Tarih
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// Numara
        /// </summary>
        public string numara;
        /// <summary>
        /// Verilen Avans
        /// </summary>
        public double verilenAvans;
        /// <summary>
        /// Mahsup Edilen Avans
        /// </summary>
        public double mahsupEdilenAvans;
        /// <summary>
        /// İşlem Tarihi
        /// </summary>
        public TNSDateTime islemTarihi;
        /// <summary>
        /// İşlem Yapan TC
        /// </summary>
        public string islemYapanKod;
        /// <summary>
        /// İşlem Yapan Adı
        /// </summary>
        public string islemYapanAd;
        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TaahhutAvanslar()
        {
            taahhutKod = 0;
            yil = 0;
            belgeNo = "";
            tarih = new TNSDateTime();
            numara = "";
            verilenAvans = 0.0;
            mahsupEdilenAvans = 0.0;
            islemTarihi = new TNSDateTime();
            islemYapanKod = "";
            islemYapanAd = "";
        }
    }

    /// <summary>
    /// borçlar ve temlikler bilgisini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class TaahhutBorclarTanim
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// konu
        /// </summary>
        public string konu;
        /// <summary>
        /// ALACAKLI AD SOYAD
        /// </summary>
        public string alacakliAdSoyad;
        /// <summary>
        /// temlik Açan Vergi No
        /// </summary>
        public string temlikAcanVergiNo;
        /// <summary>
        ///temlik veren vergi no
        /// </summary>
        public string temlikVerenVergiNo;
        /// <summary>
        /// temlik tutari
        /// </summary>
        public double temlikTutari;
        /// <summary>
        /// aciklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Noter Onay Tarihi
        /// </summary>
        public TNS.TNSDateTime noterOnayTarihi;
        /// <summary>
        /// Noter Onay Tarihi Sorgu
        /// </summary>
        public TNS.TNSDateTime noterOnayTarihiSorgu;
        /// <summary>
        /// Noter Onay No
        /// </summary>
        public string noterOnayNo;
        /// <summary>
        /// İşlem Yapan Adı
        /// </summary>
        public string islemYapanAd;
        /// <summary>
        /// İşlem Yapan TC
        /// </summary>
        public string islemYapanKod;
        /// <summary>
        /// İşlem Tarihi
        /// </summary>
        public TNS.TNSDateTime islemTarihi;
        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TaahhutBorclarTanim()
        {
            kod = 0;
            konu = "";
            temlikAcanVergiNo = "";
            temlikVerenVergiNo = "";
            temlikTutari = 0.0;
            aciklama = "";
            noterOnayTarihi = new TNSDateTime();
            noterOnayTarihiSorgu = new TNSDateTime();
            noterOnayNo = "";
            islemYapanAd = "";
            islemYapanKod = "";
            islemTarihi = new TNSDateTime();
        }
    }

    /// <summary>
    /// Odeme Emri bilgisini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class TaahhutOdeme
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Taahhüt kodu
        /// </summary>
        public int taahhutKod;
        /// <summary>
        /// Odeme Emri kodu
        /// </summary>
        public string odemeEmriKod;
        /// <summary>
        /// Odeme Emri Tarihi
        /// </summary>
        public TNSDateTime odemeEmriTarihi;
        /// <summary>
        /// aciklama
        /// </summary>
        public string aciklama;
        public string hakedisHesap;
        public string odemeHesap;
        public string yansitmaHesap;
        /// <summary>
        /// hakedisTutari
        /// </summary>
        public double hakedisTutari;
        /// <summary>
        ///Damga Vegisi
        /// </summary>
        public double damgaVergisi;
        /// <summary>
        /// gelir Vergisi Stopaji
        /// </summary>
        public double gelirVergisiStopaji;
        /// <summary>
        /// fonpayı
        /// </summary>
        public double fonPayi;
        /// <summary>
        /// para cezaları
        /// </summary>
        public double paraCezalari;
        /// <summary>
        /// mahsup edilen avans
        /// </summary>
        public double mahsupEdilenAvans;
        /// <summary>
        /// icra kesintisi
        /// </summary>
        public double icraKesintisi;
        /// <summary>
        /// teminat kesintisi
        /// </summary>
        public double teminatKesintisi;
        /// <summary>
        ///ssk primi ve gecikme zammı
        /// </summary>
        public double sskPrimiVeGecikmeZammi;
        /// <summary>
        ///vergi borcu
        /// </summary>
        public double vergiBorcu;
        /// <summary>
        ///kdv tevkifatı
        /// </summary>
        public double kdvTevkifati;
        /// <summary>
        ///temlik
        /// </summary>
        public double temlik;
        /// <summary>
        ///bütçe emaneti
        /// </summary>
        public double butceEmaneti;
        /// <summary>
        ///kdv
        /// </summary>
        public double kdv;
        /// <summary>
        ///kesintiler toplamı
        /// </summary>
        public double kesintilerToplami;
        /// <summary>
        ///yapılan ödeme
        /// </summary>
        public double yapilanOdeme;
        /// <summary>
        /// İşlem Tarihi
        /// </summary>
        public TNS.TNSDateTime islemTarihi;
        /// <summary>
        /// İşlem Yapan TC
        /// </summary>
        public string islemYapanKod;
        /// <summary>
        /// İşlem Yapan Adı
        /// </summary>
        public string islemYapanAd;
        /// <summary>
        /// kdvHesapKodu
        /// </summary>
        public string kdvHesapKodu;
        /// <summary>
        /// damgaVergisiHesapKodu
        /// </summary>
        public string damgaVergisiHesapKodu;
        /// <summary>
        /// gelirVergisiHesapKodu
        /// </summary>
        public string gelirVergisiHesapKodu;
        /// <summary>
        /// fonPayiHesapKodu
        /// </summary>
        public string fonPayiHesapKodu;
        /// <summary>
        /// paraCezalariHesapKodu
        /// </summary>
        public string paraCezalariHesapKodu;
        /// <summary>
        /// mahsupEdileAvansHesapKodu
        /// </summary>
        public string mahsupEdileAvansHesapKodu;
        /// <summary>
        /// icraKesintisiHesapKodu
        /// </summary>
        public string icraKesintisiHesapKodu;
        /// <summary>
        /// teminatKesintisiHesapKodu
        /// </summary>
        public string teminatKesintisiHesapKodu;
        /// <summary>
        /// sskPrimiGecikmeZammiHesapKodu
        /// </summary>
        public string sskPrimiGecikmeZammiHesapKodu;
        /// <summary>
        /// vergiBorcuHesapKodu
        /// </summary>
        public string vergiBorcuHesapKodu;
        /// <summary>
        /// kdvTevkifatiHesapKodu
        /// </summary>
        public string kdvTevkifatiHesapKodu;
        /// <summary>
        /// temlikHesapKodu
        /// </summary>
        public string temlikHesapKodu;
        /// <summary>
        /// butceEmanetiHesapKodu
        /// </summary>
        public string butceEmanetiHesapKodu;
        /// <summary>
        /// kdvHesapKodu2
        /// </summary>
        public string kdvHesapKodu2;
        /// <summary>
        ///kdv2
        /// </summary>
        public double kdv2;
        /// <summary>
        /// kdvHesapKodu3
        /// </summary>
        public string kdvHesapKodu3;
        /// <summary>
        ///kdv3
        /// </summary>
        public double kdv3;
        /// <summary>
        /// damgaVergisiHesapKodu2
        /// </summary>
        public string damgaVergisiHesapKodu2;
        /// <summary>
        ///Damga Vegisi2
        /// </summary>
        public double damgaVergisi2;

        public ObjectArray digerGelirler = new ObjectArray();

        public string kiraGeliriHesapKodu;
        public string kiraGeliriKDVHesapKodu;
        public double kiraGeliri;
        public double kiraGeliriKDV;

        public string cesitliGelirHesapKodu;
        public string cesitliGelirKDVHesapKodu;
        public double cesitliGelir;
        public double cesitliGelirKDV;

        public string digerHizmetGelirHesapKodu;
        public string digerHizmetGelirKDVHesapKodu;
        public double digerHizmetGelir;
        public double digerHizmetGelirKDV;


        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TaahhutOdeme()
        {
            kod = 0;
            taahhutKod = 0;
            odemeEmriKod = "";
            odemeEmriTarihi = new TNSDateTime();
            aciklama = "";
            hakedisHesap = "";
            odemeHesap = "";
            yansitmaHesap = "";
            hakedisTutari = 0.0;
            damgaVergisi = 0.0;
            gelirVergisiStopaji = 0.0;
            fonPayi = 0.0;
            paraCezalari = 0.0;
            mahsupEdilenAvans = 0.0;
            icraKesintisi = 0.0;
            teminatKesintisi = 0.0;
            sskPrimiVeGecikmeZammi = 0.0;
            vergiBorcu = 0.0;
            kdvTevkifati = 0.0;
            temlik = 0.0;
            butceEmaneti = 0.0;
            kdv = 0.0;
            kesintilerToplami = 0.0;
            yapilanOdeme = 0.0;
            islemTarihi = new TNSDateTime();
            islemYapanKod = "";
            islemYapanAd = "";
            kdvHesapKodu = "";
            damgaVergisiHesapKodu = "";
            gelirVergisiHesapKodu = "";
            fonPayiHesapKodu = "";
            paraCezalariHesapKodu = "";
            mahsupEdileAvansHesapKodu = "";
            icraKesintisiHesapKodu = "";
            teminatKesintisiHesapKodu = "";
            sskPrimiGecikmeZammiHesapKodu = "";
            vergiBorcuHesapKodu = "";
            kdvTevkifatiHesapKodu = "";
            temlikHesapKodu = "";
            butceEmanetiHesapKodu = "";
            kdvHesapKodu2 = "";
            kdv2 = 0;
            kdvHesapKodu3 = "";
            kdv3 = 3;
            damgaVergisiHesapKodu2 = "";
            damgaVergisi2 = 0;
            kiraGeliriHesapKodu = "";
            kiraGeliriKDVHesapKodu = "";
            kiraGeliri = 0;
            kiraGeliriKDV = 0;
            cesitliGelirHesapKodu = "";
            cesitliGelirKDVHesapKodu = "";
            cesitliGelir = 0;
            cesitliGelirKDV = 0;
            digerHizmetGelirHesapKodu = "";
            digerHizmetGelirKDVHesapKodu = "";
            digerHizmetGelir = 0;
            digerHizmetGelirKDV = 0;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class TaahhutOdemeDiger
    {
        public int kod = 0;
        public int taahhutKod = 0;
        public int taahhutOdemeKod = 0;
        public string hesapKod = "";
        public double borc = 0.0;
        public double alacak = 0.0;
        public string hesapKodKDV = "";
        public double borcKDV = 0.0;
        public double alacakKDV = 0.0;
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class GIBAktarim
    {
        public string adi = "";
        public string soyadi = "";
        public string vergiNo = "";
        public string islemTuru = "";
        public int oran = 0;
        public string vergiOran = "";
        public double matrah = 0.0;
        public double icerdigiPara = 0.0;
        public double alacak = 0.0;
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class KBSAktarim
    {
        public string kod = "";
        public int yil = 0;
        public int goster = 0;
        public string hesapKod = "";
        public string hesapAd = "";
        public int hesapUzunluk = 0;
        public int fonUzunluk = 0;
        public int finUzunluk = 0;
    }
    [Serializable]
    public class KBSAktarimDeger
    {
        public int yil = 0;
        public string hesapKod = "";
        public string hesap = "";
        public string fon = "";
        public string fin = "";
        public double borc = 0.0;
        public double alacak = 0.0;
    }

    /// <summary>
    /// borçlar ve temlikler bilgisini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class TaahhutBorclar
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// taahhüt kod
        /// </summary>
        public int taahhutKod;
        /// <summary>
        /// ALACAKLI AD SOYAD
        /// </summary>
        public string alacakliAdSoyad;
        /// <summary>
        /// DOSYA NO
        /// </summary>
        public int dosyaNo;
        /// <summary>
        /// temlik Açan Vergi No
        /// </summary>
        public string temlikAcanVergiNo;
        /// <summary>
        ///temlik veren vergi no
        /// </summary>
        public string temlikVerenVergiNo;
        /// <summary>
        /// temlik tutari
        /// </summary>
        public double temlikTutari;
        /// <summary>
        /// islem yapan ad
        /// </summary>
        public string islemYapanAd;
        /// <summary>
        /// İşlem Yapan TC
        /// </summary>
        public string islemYapanKod;
        /// <summary>
        /// İşlem Tarihi
        /// </summary>
        public TNS.TNSDateTime islemTarihi;
        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TaahhutBorclar()
        {
            kod = 0;
            taahhutKod = 0;
            dosyaNo = 0;
            alacakliAdSoyad = "";
            temlikAcanVergiNo = "";
            temlikVerenVergiNo = "";
            temlikTutari = 0.0;
            islemYapanAd = "";
            islemYapanKod = "";
            islemTarihi = new TNSDateTime();
        }
    }

    /// <summary>
    /// borçlar ve temlikler bilgisini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class MutemetTanim
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod = 0;
        /// <summary>
        /// yil
        /// </summary>
        public int yil = 0;
        /// <summary>
        /// tc kimlik no
        /// </summary>
        public string tcKimlikNo = "";
        /// <summary>
        /// muhasebe Kod
        /// </summary>
        public int muhasebeKod = 0;
        public string birimKod = "";
        /// <summary>
        ///hesap Kod
        /// </summary>
        public string hesapKod = "";
        /// <summary>
        /// tutar
        /// </summary>
        public double tutar = 0.0;
        /// <summary>
        /// islem yapan ad
        /// </summary>
        public string islemYapanAd = "";
        /// <summary>
        /// İşlem Yapan TC
        /// </summary>
        public string islemYapanKod = "";
        /// <summary>
        /// İşlem Tarihi
        /// </summary>
        public TNS.TNSDateTime islemTarihi = new TNSDateTime();
        public string kisiAdi = "";
        public string hesapAdi = "";
    }

    /// <summary>
    /// borçlar ve temlikler bilgisini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class HesapPlaniKisit
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod = 0;
        /// <summary>
        /// yil
        /// </summary>
        public int yil = 0;
        /// <summary>
        /// muhasebe Kod
        /// </summary>
        public int muhasebeKod = 0;
        public string birimKod = "";
        /// <summary>
        ///hesap Kod
        /// </summary>
        public string hesapKod = "";
        public string hesapAdi = "";
        /// <summary>
        /// islem yapan ad
        /// </summary>
        public string islemYapanAd = "";
        /// <summary>
        /// İşlem Yapan TC
        /// </summary>
        public string islemYapanKod = "";
        /// <summary>
        /// İşlem Tarihi
        /// </summary>
        public TNS.TNSDateTime islemTarihi = new TNSDateTime();
    }

    /// <summary>
    /// borçlar ve temlikler bilgisini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class VergiDairesiAktarimTurTanim
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod = 0;
        /// <summary>
        /// The altarim tur kod
        /// </summary>
        public int aktarimTurKod = 0;
        /// <summary>
        /// tür kod
        /// </summary>
        public int turKod = 0;
        /// <summary>
        /// adı
        /// </summary>
        public string adi = "";
        /// <summary>
        /// hesap Kod
        /// </summary>
        public string hesapKod = "";
        /// <summary>
        /// oranı
        /// </summary>
        public decimal orani = 0;
        /// <summary>
        /// islem yapan ad
        /// </summary>
        public string islemYapanAd = "";
        /// <summary>
        ///  islem yapan kod
        /// </summary>
        public string islemYapanKod = "";
        /// <summary>
        ///  islem tarihi
        /// </summary>
        public TNS.TNSDateTime islemTarihi = new TNSDateTime();

        public static string HesapKoduBul(ObjectArray tanimlar, int turKod, string adi, bool tek)
        {
            string sonuc = "";
            foreach (VergiDairesiAktarimTurTanim item in tanimlar.objeler)
            {
                if (item.turKod == turKod && item.adi == adi)
                {
                    if (tek) return item.hesapKod;
                    else
                    {
                        if (sonuc != "") sonuc += ";";
                        sonuc += item.hesapKod;
                    }
                }
            }
            return sonuc;
        }

        public static double TutarVer(IMUHServis s, Kullanici kullanan, int yil, int ay, int saymanlikKod, string hesapKod, int borcAlacak, bool yevmiyeli)
        {
            TNSDateTime t0 = new TNSDateTime("01." + "01" + "." + yil);
            TNSDateTime t2 = new TNSDateTime("01." + ay + "." + yil);
            t2.AddMonths(1);
            t2.AddDays(-1);

            MizanBilgi mbIc = s.Mizan(kullanan, yil, "", saymanlikKod, "", t0, t2, 100, hesapKod, hesapKod, yevmiyeli ? 1 : 3, new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI }, 0, "");
            if (mbIc != null && mbIc.anaHesaplar != null)
            {
                foreach (AnaHesap item in mbIc.anaHesaplar.objeler)
                {
                    if (item.detayHesaplar != null)
                    {
                        foreach (HesapBilgi itemDet in item.detayHesaplar.objeler)
                        {
                            if (itemDet.kalanBorc > 0) return OrtakFonksiyonlar.KurusYap(itemDet.kalanBorc);
                            if (itemDet.kalanAlacak > 0) return OrtakFonksiyonlar.KurusYap(itemDet.kalanAlacak);
                        }
                    }
                }
            }
            return 0;
        }
    }

    /// <summary>
    /// TAAHHUT TARİHCE bilgisini taşıyan nesne
    /// </summary>
    [Serializable()]
    public class TaahhutKartiTarihce
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod = 0;
        /// <summary>
        /// taahhut kod
        /// </summary>
        public int taahhutKod = 0;
        /// <summary>
        /// işlem adı
        /// </summary>
        public string islemAdi = "";
        /// <summary>
        /// işlem Kod
        /// </summary>
        public int islemKod = 0;
        /// <summary>
        /// islem yapan ad
        /// </summary>
        public string islemYapanAd = "";
        /// <summary>
        ///  islem yapan kod
        /// </summary>
        public string islemYapanKod = "";
        /// <summary>
        ///  islem tarihi
        /// </summary>
        public TNS.TNSDateTime islemTarihi = new TNSDateTime();
        /// <summary>
        /// açıklama
        /// </summary>
        public string aciklama = "";
    }

    [System.Serializable()]
    public class TasinirRaporuListe
    {
        public int yil = 0;
        public string muhasebeKod = "";
        public string harcamaBirimKod = "";
        public string fisNo = "";
        public TNSDateTime fisTarih = new TNSDateTime();
        public string ambarKod = "";
        public int islemTipi = 0;
        public string ilgili = "";
        public TNSDateTime islemTarih = new TNSDateTime();
        public string hesapKod = "";
        public string hesapAdi = "";
        public double adet = 0.0;
        public double tutar = 0.0;
        public string belgeNo = "";
    }

    [Serializable()]
    public class Fatura
    {
        public int kod = 0;
        public int muhasebeKod = 0;
        public string birimKod = "";
        public string mudurlukKod = "";
        public string musteriKod = "";
        public string faturaNo = "";
        public TNSDateTime faturaTarihi = new TNSDateTime();
        public string irsaliyeNo = "";
        public TNSDateTime irsaliyeTarihi = new TNSDateTime();

        public string faturaParaBirimi = "";
        public int faturaDili = 0;
        public string tahakkukBelgeNo = "";
        public string odemeBelgeNo = "";

        public int tevkifatOran = 0;
        public double tevkifatTutar = 0.0;

        public string aciklama = "";
        public double toplam = 0.0;
        public double kdvToplam = 0;
        public double genelToplam = 0.0;
        public TNSDateTime odemeTarihi = new TNSDateTime();
        public double odenenToplam = 0.0;
        public double odenenTutar = 0.0;
        public double kesintiTutar = 0.0;
        public string odemeParaBirimi = "";
        public ObjectArray detay = new ObjectArray();
        public string musteriAdi = "";
        public string musteriAdiEk = "";
        public TNSDateTime sorguBasTarih = new TNSDateTime();
        public TNSDateTime sorguBitTarih = new TNSDateTime();
        public int durum = 0;
        public string muhBelgeAciklama = "";
        public int[] soruguKod = null;

        public string tahakkukHesapKod = "";
        public string odemeHesapKod = "";
        public string islemYapan = "";
        public TNSDateTime islemTarihi = new TNSDateTime();
    }

    [Serializable()]
    public class FaturaDetay
    {
        public int kod = 0;
        public int faturaKod = 0;
        public string aciklama = "";
        public string hesapKod = "";
        public double miktar = 0.0;
        public double kdvOran = 0.0;
        public double birimFiyat = 0.0;
        public double kdvTutar = 0.0;
        public double tutar = 0.0;
    }

    [Serializable()]
    public class FaturaAciklama
    {
        public int kod = 0;
        public int tur = 0;
        public string aciklama = "";
    }

    public enum ENUMFaturaDurum
    {
        BUTUNFATURALAR = 0,
        YENI = 1,
        TAHAKKUK = 3,
        ODENDI = 5,
        IPTAL = 9,
    }

    [Serializable()]
    public class Plaka
    {
        public int kod = 0;
        public string plaka1 = "";
        public string plaka2 = "";
        public string plaka3 = "";
        public string aciklama = "";
        public ObjectArray plakaSahip = new ObjectArray();
    }

    [Serializable()]
    public class PlakaSahip
    {
        public int kod = 0;
        public int plakaKod = 0;
        public string vergiNo = "";
        public string ad = "";
        public string adres = "";
    }

    [Serializable()]
    public class AciklamaBilgi
    {
        public int kod = 0;
        public int muhasebeKod = 0;
        public string birimKod = "";
        public string adi = "";
        public string aciklama = "";
        public int tipi = 0;
        public string kayitEden = "";
        public int paylasilan = 0;
    }

    [Serializable()]
    public class SayistayVeri
    {
        public string kod = "";
        public int muhasebe = 0;
        public int yil = 0;
        public int ay = 0;
        public string veriler = "";
        public string tur = "";
    }

    [Serializable()]
    public class SGBKBSFark
    {
        [Serializable()]
        public class FaalOdenek : IComparable
        {
            public string faal;
            public double odenek;

            public int CompareTo(object obj)
            {
                if (obj is FaalOdenek)
                {
                    FaalOdenek gelen = (FaalOdenek)obj;
                    return gelen.odenek.CompareTo(this.odenek);
                }
                return 0;
            }
        }

        public int SayKod { get; set; }
        public string SaySorgu { get; set; }
        public string BirimKod { get; set; }
        public string HesapKod { get; set; }
        public string Tertip
        {
            get
            {
                return kurKod + "-" + fonKod + "-" + finKod + "-" + ekoKod + "-" + pebKod;
            }
            set
            {
                string[] alanlar = value.Split('-');
                kurKod = OrtakFonksiyonlar.DiziElemanAl(alanlar, 0, "");
                fonKod = OrtakFonksiyonlar.DiziElemanAl(alanlar, 1, "");
                finKod = OrtakFonksiyonlar.DiziElemanAl(alanlar, 2, "");
                ekoKod = OrtakFonksiyonlar.DiziElemanAl(alanlar, 3, "");
                pebKod = OrtakFonksiyonlar.DiziElemanAl(alanlar, 4, "");
            }
        }
        public double SGBHarcama { get; set; }
        public double KBSHarcama { get; set; }
        public double fark { get { return KBSHarcama - SGBHarcama; } }

        public TNSCollection faalOdenekler = new TNSCollection();
        public bool pebli = false;

        public int sgbKsbDurum;
        public double minFark;
        public int minFarkTur;

        private FaalOdenek FaalOdenekBul(int sira)
        {
            while (sira >= faalOdenekler.Count)
            {
                faalOdenekler.Add(new FaalOdenek());
            }
            return (FaalOdenek)faalOdenekler[sira];
        }

        private void FaalYaz(int sira, string value)
        {
            FaalOdenek fo = FaalOdenekBul(sira);
            fo.faal = value;
        }

        public string FaalBul(int sira)
        {
            FaalOdenek fo = FaalOdenekBul(sira);
            return fo.faal;
        }

        private void OdenekYaz(int sira, double value)
        {
            FaalOdenek fo = FaalOdenekBul(sira);
            fo.odenek = value;
        }

        public double OdenekBul(int sira)
        {
            FaalOdenek fo = FaalOdenekBul(sira);
            return fo.odenek;
        }

        public string Faal0 { get { return FaalBul(0); } set { FaalYaz(0, value); } }
        public double Odenek0 { get { return OdenekBul(0); } set { OdenekYaz(0, value); } }

        public string Faal1 { get { return FaalBul(1); } set { FaalYaz(1, value); } }
        public double Odenek1 { get { return OdenekBul(1); } set { OdenekYaz(1, value); } }

        public string Faal2 { get { return FaalBul(2); } set { FaalYaz(2, value); } }
        public double Odenek2 { get { return OdenekBul(2); } set { OdenekYaz(2, value); } }

        public string Faal3 { get { return FaalBul(3); } set { FaalYaz(3, value); } }
        public double Odenek3 { get { return OdenekBul(2); } set { OdenekYaz(2, value); } }

        public double OdenekT
        {
            get
            {
                double top = 0;
                foreach (FaalOdenek item in faalOdenekler)
                {
                    top += item.odenek;
                }
                return top;
            }
        }

        public int yil { get; set; }

        public int ay { get; set; }

        public string kurKod { get; set; }

        public string fonKod { get; set; }

        public string finKod { get; set; }

        public string ekoKod { get; set; }

        public FaalOdenek Ekle(string pebKod, double odenek)
        {
            FaalOdenek f = new FaalOdenek();
            f.faal = pebKod;
            f.odenek = odenek;
            faalOdenekler.Add(f);
            return f;
        }

        public FaalOdenek Bul(string pebKod)
        {
            foreach (FaalOdenek f in faalOdenekler)
            {
                if (f.faal == pebKod) return f;
            }
            return null;
        }

        public int mode { get; set; }

        public string pebKod { get; set; }

    }

    [Serializable()]
    public class ImzaTanim
    {
        public int kod;
        public string ad;
        public string kullaniciKodu;
        public ENUMBelgeTuru belgeTur;
        public ImzaTanimDetay[] detaylar;

        public ImzaTanim()
        {
            kod = 0;
            ad = "";
            kullaniciKodu = "";
            belgeTur = 0;
        }
    }

    [Serializable()]
    public class ImzaTanimDetay : ImzaBilgisi
    {
        public int imzaTanimKod;
    }

    /// <summary>
    /// CekKocani: Çek Koçanındaki Sayfalar
    /// </summary>
    [System.Serializable()]
    public class CekKocani
    {
        private static string CekDurumAdBul(CekDurum cekDurum)
        {
            switch (cekDurum)
            {
                case CekDurum.Iptalli: return "İptal";
                case CekDurum.Bozduruldu: return "Bozduruldu";
                default: return "Yeni";
            }
        }

        public String cekDurumAd
        {
            get
            {
                return CekDurumAdBul(cekDurum);
            }
        }

        public CekDurum cekDurum
        {
            get
            {
                if (!TNSDateTime.IsNull(iptalTarihi)) return CekDurum.Iptalli;
                if (!String.IsNullOrWhiteSpace(odemeEmriNo)) return CekDurum.Bozduruldu;
                return CekDurum.Acik;
            }
        }

        public enum CekDurum : int
        {
            Iptalli = 9,
            Bozduruldu = 5,
            Acik = 1
        }

        /// <summary>
        /// hesapKod: Hesap Kodu
        /// </summary>
        public string hesapKod { get; set; }

        /// <summary>
        /// CekNo: Çek numarası
        /// </summary>
        public string cekNo { get; set; }

        /// <summary>
        /// Kayıt Tarihi: Çek sayfası kayıt tarihi
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

        public TNSDateTime gun { get; set; }
        public DateTime gunDT
        {
            get
            {
                if (gun == null)
                    return DateTime.MinValue;
                else
                    return gun.Oku();
            }
            set
            {
                if (gun == null) gun = new TNSDateTime(value);
                else gun.Yaz(value);
            }
        }

        /// <summary>
        /// İptal Tarihi: Çekin iptal edildiği tarihi
        /// </summary>
        public TNSDateTime iptalTarihi { get; set; }
        public DateTime iptalTarihiDT
        {
            get
            {
                if (iptalTarihi == null)
                    return DateTime.MinValue;
                else
                    return iptalTarihi.Oku();
            }
            set
            {
                if (iptalTarihi == null) iptalTarihi = new TNSDateTime(value);
                else iptalTarihi.Yaz(value);
            }
        }

        public string iptalNedeni { get; set; }
        public string mifBelgeNo { get; set; }
        public string tcVno { get; set; }
        public string ilgili { get; set; }
        public double tutar { get; set; }
        public string odemeEmriNo { get; set; }

        public TNSDateTime tahsilTarihi { get; set; }
        public DateTime tahsilTarihiDT
        {
            get
            {
                if (tahsilTarihi == null)
                    return DateTime.MinValue;
                else
                    return tahsilTarihi.Oku();
            }
            set
            {
                if (tahsilTarihi == null) tahsilTarihi = new TNSDateTime(value);
                else tahsilTarihi.Yaz(value);
            }
        }
    }

    [Serializable()]
    public class IsTanim
    {
        public string muhasebeBirimKod;
        public int muhasebeKod;

        public int kod { get; set; }
        public int orgBirimKod { get; set; }
        public string orgBirimAd { get; set; }
        public string adi { get; set; }
        public int tip { get; set; }
        public TNSDateTime baslangicTarihi { get; set; }
        public TNSDateTime bitisTarihi { get; set; }
        public string amac { get; set; }
        public string hedef { get; set; }
        public string sorumluKod { get; set; }
        public string sorumluAd { get; set; }
        public string gorevTanimi { get; set; }
        public int durum { get; set; }

        public string islemYapan { get; set; }
        public TNSDateTime islemTarihi { get; set; }

        public string sorguEko { get; set; }
        public int sorguYil { get; set; }

    }

    [Serializable()]
    public class IsOdenek
    {
        public int isKod { get; set; }
        public int yil { get; set; }
        public string kurKod { get; set; }
        public string fonKod { get; set; }
        public string finKod { get; set; }
        public string ekoKod { get; set; }
        public string pebKod { get; set; }
        public double blokeTutar { get; set; }

        public string tertip
        {
            get
            {
                string str = "";

                if (this.yil > 0)
                {
                    Tertip t = new Tertip(this.yil, ENUMTabloKod.TABPEBOdenek);

                    t.TertipAta(kurKod, fonKod, finKod, ekoKod, pebKod);
                    t.Noktala();

                    str = t.TertipStrOl();
                }
                return str;
            }
            set
            {
                string tertip = value;

                if (this.yil > 0 && !string.IsNullOrWhiteSpace(tertip))
                {
                    Tertip t = new Tertip(this.yil, ENUMTabloKod.TABPEBOdenek);
                    t.TertipAta(tertip);
                    t.NoktalariAt();

                    this.kurKod = t.kodlar[0];
                    this.fonKod = t.kodlar[1];
                    this.finKod = t.kodlar[2];
                    this.ekoKod = t.kodlar[3];
                    if (t.kodlar.Length > 4)
                        this.pebKod = t.kodlar[4] + "";
                }


            }
        }

        public IsOdenek()
        {
            this.kurKod = "";
            this.fonKod = "";
            this.finKod = "";
            this.ekoKod = "";
            this.pebKod = "";
        }

    }

    [Serializable()]
    public class IsBlokeOdenek
    {
        public string isAd { get; set; }
        public string birimAd { get; set; }
        public string ilgiliVergiNo { get; set; }
        public string ilgiliAd { get; set; }
        public double toplamOdenek { get; set; }
        public double harcananOdenek { get; set; }

        public double kalanTutar { get; set; }

        public string sorguEko { get; set; }

    }


    [Serializable()]
    public class TaahhutKartiSozlesme
    {
        public int kod { get; set; }
        public int taahhutKartiKod { get; set; }
        public ENUMSozlesmeTur tur { get; set; }
        public int maddeNo { get; set; }
        public string ad { get; set; }
        public string aciklama { get; set; }
    }

    public enum ENUMSozlesmeTur
    {
        SOZLESMEKOSULLARI = 1,
        GARANTISARTLARI = 2
    }

    [Serializable()]
    public class TaahhutKartiBakimTarihce
    {
        public int kod { get; set; }
        public int taahhutKartiKod { get; set; }
        public TNSDateTime girisTarihi { get; set; }
        public string girisAciklama { get; set; }
        public TNSDateTime cikisTarihi { get; set; }
        public string cikisAciklama { get; set; }

    }

    [Serializable()]
    public class OrtakBelge
    {
        public string kod { get; set; }
        public string ilgiKod { get; set; }
        public ENUMOrtakBelgeTip tip { get; set; }
        public string belgeAdi { get; set; }
        public byte[] icerik { get; set; }
        public string kayitEdilecekDosyaYol = "";

        public OrtakBelge() { }
        public OrtakBelge(string belgeAdi, int belgeBoyutu)
        {
            this.belgeAdi = belgeAdi;
            this.icerik = new byte[belgeBoyutu];
        }

    }

    [Serializable()]
    public class CaprazTablo
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod { get; set; }

        public string adi { get; set; }
        /// <summary>
        /// json
        /// </summary>
        public string json { get; set; }
        /// <summary>
        /// kayitEden
        /// </summary>
        public string kayitEden { get; set; }

        public TNSDateTime kayitTarihi { get; set; }

        public int sutunTur { get; set; }

        public string sorguSatir { get; set; }

        public string sorguSutun { get; set; }

        public int sorguYil { get; set; }

        public int sorguDurum { get; set; }

        public string sorguMuhasebeKod { get; set; }

        public string sorguBirimKod { get; set; }

        public TNSDateTime sorguTarih1 { get; set; }

        public TNSDateTime sorguTarih2 { get; set; }

        public string sorguAltHesapKod { get; set; }
        public string sorguIlgiliKod { get; set; }

        public string sorguKurKod { get; set; }

        public string sorguFonkKod { get; set; }

        public string sorguFinansKod { get; set; }

        public string sorguEkoKod { get; set; }

        public string sorguFaaliyetKod { get; set; }
        public TNSDateTime sorguYevmiyeTarih1 { get; set; }

        public TNSDateTime sorguYevmiyeTarih2 { get; set; }

        /// <summary>
        /// Sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public CaprazTablo()
        {
            kod = 0;
            adi = "";
            json = string.Empty;
            kayitEden = string.Empty;
            kayitTarihi = new TNSDateTime();
            sutunTur = 0;
            sorguSatir = "";
            sorguSutun = "";
            sorguYil = 0;
            sorguDurum = 0;
            sorguMuhasebeKod = "";
            sorguBirimKod = "";
            sorguTarih1 = new TNSDateTime();
            sorguTarih2 = new TNSDateTime();
            sorguYevmiyeTarih1 = new TNSDateTime();
            sorguYevmiyeTarih2 = new TNSDateTime();
            sorguAltHesapKod = "";
            sorguIlgiliKod = "";
            sorguKurKod = "";
            sorguFonkKod = "";
            sorguFinansKod = "";
            sorguEkoKod = "";
            sorguFaaliyetKod = "";
        }

    }

    [Serializable()]
    public class CaprazTabloIcerik
    {
        public string belgeNo = "";
        public string hesapKodu = "";
        public string tur = "";
        public int yevmiyeNo = 0;
        public TNSDateTime belgeTarih = new TNSDateTime();
        public TNSDateTime yevmiyeTarih = new TNSDateTime();
        public double bakiye = 0.0;
        public string ilgiliAdi = "";
        public string ilgiliNo = "";
    }

    [Serializable()]
    public class TalepTanimForm
    {
        public string kod { get; set; }
        public int yil { get; set; }
        public int ay { get; set; }
        public int talepTur { get; set; }
        public string kayitEden { get; set; }
        public TNSDateTime kayitTarihi { get; set; }
        public ObjectArray satirlar;
        public bool detayGoster { get; set; }

        public TalepTanimForm()
        {
            kod = "";
            yil = 0;
            ay = 0;
            talepTur = 0;
            kayitEden = "";
            kayitTarihi = new TNSDateTime();
            satirlar = new ObjectArray();
            detayGoster = false;
        }

    }

    [Serializable()]
    public class TalepTanimFormDetay
    {
        public string kod { get; set; }
        public string talepKod { get; set; }
        public string ekoKod { get; set; }
        public string ekoAdi { get; set; }

        public TalepTanimFormDetay()
        {
            kod = "";
            talepKod = "";
            ekoKod = "";
            ekoAdi = "";
        }

    }

    [Serializable()]
    public class TkdkTalepForm
    {
        public string kod { get; set; }
        public string belgeNo { get; set; }
        public int yil { get; set; }
        public int ay { get; set; }
        public int tur { get; set; }
        public int muhasebeKod { get; set; }
        public string birimKod { get; set; }
        public int durum { get; set; }
        public string kayitEden { get; set; }
        public string kayitEdenAdi { get; set; }
        public TNSDateTime kayitTarihi { get; set; }
        public string gerekce { get; set; }
        public string odemeEmriNo1 { get; set; }
        public string odemeEmriNo2 { get; set; }
        public string odemeEmriNo3 { get; set; }
        public string odemeEmriNo4 { get; set; }

        public double mevcutBakiye { get; set; }
        public double gelenNakit { get; set; }

        public ObjectArray satirlar { get; set; }
        public bool detayGoster { get; set; }
        public string birimAdi { get; set; }
        public string birimIban { get; set; }

        public string birimVergiNo { get; set; }
        public string birimVergiAdi { get; set; }

        public string birimKurumsalkod { get; set; }

        public TkdkTalepForm()
        {
            kod = "";
            belgeNo = "";
            yil = 0;
            ay = 0;
            tur = 0;
            muhasebeKod = 0;
            birimKod = "";
            durum = 0;
            kayitEden = "";
            kayitEdenAdi = "";
            kayitTarihi = new TNSDateTime();
            gerekce = "";
            odemeEmriNo1 = "";
            odemeEmriNo2 = "";
            odemeEmriNo3 = "";
            odemeEmriNo4 = "";
            mevcutBakiye = 0.0;
            gelenNakit = 0.0;
            satirlar = new ObjectArray();
            detayGoster = false;
            birimAdi = "";
            birimIban = "";
            birimVergiNo = "";
        }

    }

    [Serializable()]
    public class TkdkTalepDetay
    {
        public string kod { get; set; }
        public string talepKod { get; set; }
        public int siraNo { get; set; }
        public string ekoKod { get; set; }
        public double tutar { get; set; }
        public double tutar1 { get; set; }
        public double tutar2 { get; set; }
        public double tutar3 { get; set; }
        public string gerekce { get; set; }
        public string ekoAdi { get; set; }
        public string tertip { get; set; }

        public TkdkTalepDetay()
        {
            kod = "";
            talepKod = "";
            siraNo = 0;
            ekoKod = "";
            tutar = 0.0;
            tutar1 = 0.0;
            tutar2 = 0.0;
            tutar3 = 0.0;
            gerekce = "";
            ekoAdi = "";
            tertip = "";
        }

    }

    public enum ENUMOrtakBelgeTip
    {
        GENEL = 0,
        TAAHHUTKARTI = 1,
        TEMINATMEKTUBU = 2,
    }

    [Serializable()]
    public class FaturaTakip : OdemeEmriMIFForm
    {
        //public int yil { get; set; }
        //public string belgeNo { get; set; }
        public int takipDurum { get; set; }

        public FaturaTakip()
        {
            //yil = 0;
            //belgeNo = "";
            takipDurum = 0;
        }

    }

    /// <summary>
    /// Muhasebe modulü içerisinde yapılabilecek işlemleri içeren arayüz
    /// </summary>
    public interface IMUHServis
    {
        /// <summary>
        /// Dokumans the kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="dokuman">The dokuman.</param>
        /// <returns></returns>
        Sonuc DokumanKaydet(Kullanici kullanan, Dokuman dokuman);
        /// <summary>
        /// Dokumans the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="dokuman">The dokuman.</param>
        /// <returns></returns>
        ObjectArray DokumanListele(Kullanici kullanan, Dokuman dokuman);
        /// <summary>
        /// Dokumans the sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="dokuman">The dokuman.</param>
        /// <returns></returns>
        Sonuc DokumanSil(Kullanici kullanan, Dokuman dokuman);

        /// <summary>
        /// Ödeme emri / MIF kaydetme işlemi
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        Sonuc OdemeEmriMIFKaydet(Kullanici kullanan, OdemeEmriMIFForm form);

        /// <summary>
        /// Ödeme emri / MIF vergi no güncelleme
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        Sonuc OdemeEmriMIFAltHesapKodGuncelle(Kullanici kullanan, string yeniVergiNo, string eskiVergiNo);

        /// <summary>
        /// firma bilgisi silme
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        Sonuc HRCFirmaBilgisiSilme(Kullanici kullanan, string vergiNo);

        /// <summary>
        /// Alts the hesap liste hazirla.
        /// </summary>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="kriter">The kriter.</param>
        /// <returns></returns>
        ObjectArray AltHesapListeHazirla(Kullanici kullanan, int muhasebe, string kriter);

        /// <summary>
        /// Odemes the emri mif kaydet eksik odenek.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        EksikOdenekler OdemeEmriMIFKaydetEksikOdenek(Kullanici kullanan, OdemeEmriMIFForm form);

        /// <summary>
        /// KDVs the bilgisi bul.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <param name="satirNo">The satir no.</param>
        /// <returns></returns>
        OdemeEmriMIFDetay KDVBilgisiBul(Kullanici kullanan, int yil, string belgeNo, int satirNo, string hesapKod);

        /// <summary>
        /// Odemes the emri mif tek satir oku.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <param name="satirNo">The satir no.</param>
        /// <returns></returns>
        OdemeEmriMIFDetay OdemeEmriMIFTekSatirOku(Kullanici kullanan, int yil, string belgeNo, int satirNo);

        /// <summary>
        /// Tek satır olarak Ödeme emri / MIF kaydetme işlemi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        Sonuc OdemeEmriMIFTekSatirKaydet(Kullanici kullanan, OdemeEmriMIFForm form);
        /// <summary>
        /// Ödeme emri / MIF kaydetme işlemi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <param name="ilgiliBelgeNo">İlgili belge no</param>
        /// <returns></returns>
        Sonuc OdemeEmriMIFIlgiliBelgeKaydet(Kullanici kullanan, int yil, string belgeNo, string ilgiliBelgeNo);

        /// <summary>
        /// Ödeme emri / MIF listeleme işlemi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="detay">Eğer <c>true</c> ise detay bilgilerini de listele</param>
        /// <returns></returns>
        /// ***
        ObjectArray OdemeEmriMIFListele(Kullanici kullanan, OdemeEmriMIFForm form, bool detay);

        /// <summary>
        /// Ödeme emri / MIF listeleme işlemi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="detay">Eğer <c>true</c> ise detay bilgilerini de listele</param>
        /// <returns></returns>
        ObjectArray OdemeEmriMIFListeleTumDetay(Kullanici kullanan, OdemeEmriMIFForm form);

        /// <summary>
        /// Ödeme emri / MIF listeleme işlemi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="detay">Eğer <c>true</c> ise detay bilgilerini de listele</param>
        /// <param name="mahsup">Eğer <c>true</c> ise mahsup bilgilerini de listele</param>
        /// <returns></returns>
        /// ***
        ObjectArray OdemeEmriMIFListele(Kullanici kullanan, OdemeEmriMIFForm form, bool detay, bool mahsup);
        /// <summary>
        /// Bir Ödeme emri / MIF listeleme işlemi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        /// ***
        ObjectArray OdemeEmriMIFTekBelgeListele(Kullanici kullanan, OdemeEmriMIFForm form);

        Sonuc BelgeDurumDegistirODEMIF(Kullanici kullanan, TNSCollection formlar, bool surecBlokeDuzenle);

        /// <summary>
        /// Tertip listele.
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="eko">Ekonomik kod</param>
        /// <returns></returns>
        ObjectArray TertipListele(int yil, string kurum, int muhasebe, string birim, string eko);
        /// <summary>
        /// 4 kırımlı olarak Tertip listele 
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="eko">Ekonomik Kod</param>
        /// <returns></returns>
        ObjectArray TertipListeleEko4(int yil, string kurum, int muhasebe, string birim, string eko);
        /// <summary>
        /// Performans bütçe tertiplerini listele 
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="eko">Ekonomik Kod</param>
        /// <param name="peb">Peb kodu</param>
        /// <returns></returns>
        ObjectArray TertipListeleEko4Peb(int yil, string kurum, int muhasebe, string birim, string eko, string peb);

        /// <summary>
        /// Ödenek Gönderme/tenkis kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="MPRevizeDeYap">4 düzey Ödenek değişimi sırasında, MP revize de üretilecekse true, aksi halde false verilmelidir. </param>
        /// <returns></returns>
        Sonuc GondermeTenkisKaydet(Kullanici kullanan, GondermeTenkisForm form, bool MPRevizeDeYap, bool donemOdenegi, bool sgbKbsKontrolYapma);
        /// <summary>
        /// Ödenek Gönderme/tenkis kontrolü
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        Sonuc GondermeTenkisKontrol(Kullanici kullanan, GondermeTenkisForm form, bool donemOdenegi, bool sgbKbsKontrolYapma);
        /// <summary>
        /// Ödenek Gönderme/tenkis listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="detay">if set to <c>true</c> [detay]</param>
        /// <returns></returns>
        ObjectArray GondermeTenkisListele(Kullanici kullanan, GondermeTenkisForm form, int detay);
        /// <summary>
        /// Ödenek Gönderme/tenkis tek belge listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        ObjectVAN GondermeTenkisKimlikOku(Kullanici kullanan, GondermeTenkisForm form);
        /// <summary>
        /// Ödenek Gönderme/tenkis tek belge listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        ObjectArray GondermeTenkisTekBelgeListele(Kullanici kullanan, GondermeTenkisForm form);
        ///// <summary>
        ///// Ödenek Gönderme/tenkis kontrol listesi
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        ///// <returns></returns>
        //ObjectArray GondermeTenkisKontrolListesi(Kullanici kullanan, GondermeTenkisForm form);
        /// <summary>
        /// Ödenek Gönderme/tenkis tek belge tek detay listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <param name="belgeTur">Belge turü</param>
        /// <param name="tertip">Tertip bilgisi</param>
        /// <returns></returns>
        GondermeTenkisDetay GondermeTenkisTekBelgeTekDetayListele(Kullanici kullanan, int yil, string belgeNo, int belgeTur, string tertip);
        /// <summary>
        /// Belge durum değiştir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="formlar">İşlem yapılacak bilgileri içeren nesne</param>
        /// <returns></returns>
        Sonuc BelgeDurumDegistirGonTen(Kullanici kullanan, TNSCollection formlar);
        /// <summary>
        /// Belge durum bilgisi
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <returns></returns>
        int BelgeDurumBilgisi(int yil, string belgeNo);
        /// <summary>
        /// Belge tür değiştir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="yeniBelgeTur">Yeni belge türü</param>
        /// <returns></returns>
        Sonuc BelgeTurDegistir(Kullanici kullanan, OdemeEmriMIFForm form, int yeniBelgeTur);
        /// <summary>
        /// Hesap planı kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="hesapPlani">Hesap planı bilgisi içeren nesne</param>
        /// <returns></returns>
        Sonuc HesapPlaniKaydet(Kullanici kullanan, ObjectArray hesapPlani);
        /// <summary>
        /// Hesaps plani sil
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="hesapPlani">Hesap planı bilgisi içeren nesne</param>
        /// <returns></returns>
        Sonuc HesapPlaniSil(Kullanici kullanan, ObjectArray hesapPlani);
        /// <summary>
        /// Hesap planı listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="hesapPlaniSatir">Hesap planı bilgisi içeren nesne</param>
        /// <returns></returns>
        ObjectArray HesapPlaniListele(Kullanici kullanan, HesapPlaniSatir hesapPlaniSatir);
        /// <summary>
        /// Hesap planı detay sayıları düzenle
        /// </summary>
        /// <param name="yil">Yıl</param>
        void HesapPlaniDetaySayilariDuzenle(int yil);
        ///// <summary>
        ///// Hesap planı ebutceden aktar
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="yil">Yıl</param>
        ///// <param name="kurum">Kurum kodu</param>
        ///// <param name="hesapKod">Hesap kodu</param>
        ///// <returns></returns>
        //Sonuc HesapPlaniEbutcedenAktar(Kullanici kullanan, string yil, string kurum, string hesapKod);
        /// <summary>
        /// Hesap plani nitelik değiştir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="hesapPlaniSatir">Hesap planı bilgisi içeren nesne</param>
        /// <returns></returns>
        Sonuc HesapPlaniNitelikDegistir(Kullanici kullanan, HesapPlaniSatir hesapPlaniSatir, int nitelikIslemi);
        /// <summary>
        /// Hesap planı aktar
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kaynakYil">Kaynak yıl</param>
        /// <param name="hedefYil">Hedef yıl</param>
        /// <returns></returns>
        Sonuc HesapPlaniAktar(Kullanici kullanan, int kaynakYil, int hedefYil);
        /// <summary>
        /// Hesap kodunun özelliklerini al
        /// </summary>
        /// <param name="hKod">Hesap kodu</param>
        /// <returns></returns>
        HesapPlaniSatir HesapKodOzellikAl(HesapPlaniSatir hKod);
        /// <summary>
        /// Hesap planı yansıtma listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="hesapPlani">Hesap planı bilgisi içeren nesne</param>
        /// <returns></returns>
        ObjectArray HesapPlaniYansitmaListele(Kullanici kullanan, ObjectArray hesapPlani);
        /// <summary>
        /// Ekonomik koddan hesap planı kodlarını getir
        /// </summary>
        /// <param name="ekoKod">eko kod</param>
        /// <returns></returns>
        HesapPlaniYansitma[] EkonomikKoddanHesapKodlariGetir(string ekoKod);

        /// <summary>
        /// Kullanıcının yetkili olduğu muhasebe birimleri
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <returns></returns>
        ObjectArray KullaniciMuhasebeBirimleri(Kullanici kullanan, int muhasebe, string kurum, string kistasKodAd);
        ObjectArray KullaniciMuhasebeBirimleri(Kullanici kullanan, int muhasebe, string kurum);

        /// <summary>
        /// Harcama birimine ait kurumsal kod listesi
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <returns></returns>
        ABSKodlarVAN HarcamaBirimleriKurumsalKod(int yil, string kurum, int muhasebe, string birim);
        /// <summary>
        /// Imza kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="imzaBilgileri">İmza bilgileri</param>
        /// <returns></returns>
        Sonuc ImzaKaydet(Kullanici kullanan, ObjectArray imzaBilgileri);
        /// <summary>
        /// Imza listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belgeTur">Belge türü</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <returns></returns>
        ObjectArray ImzaListele(Kullanici kullanan, ENUMBelgeTuru belgeTur, int muhasebe, string birim);
        /// <summary>
        /// Nakit talebi kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">İşlem yapılacak bilgileri içeren nesne</param>
        /// <param name="kontrolSekli">Kontrol şekli</param>
        /// <returns></returns>
        Sonuc NakitTalebiKaydet(Kullanici kullanan, ObjectArray bilgi, string kontrolSekli);
        /// <summary>
        /// Nakit talebi listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">İşlem yapılacak bilgileri içeren nesne</param>
        /// <param name="kontrolSekli">Kontrol şekli</param>
        /// <returns></returns>
        ObjectArray NakitTalebiListele(Kullanici kullanan, NakitTalebi bilgi, string kontrolSekli);
        /// <summary>
        /// Tarihce listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="belgeTur">Belge tür</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <returns></returns>
        ObjectArray TarihceListele(Kullanici kullanan, int yil, ENUMBelgeTuru belgeTur, string belgeNo);
        /// <summary>
        /// Gider Bütçesi kaydet
        /// </summary>
        /// <param name="bilgi">İşlem yapılacak bilgileri içeren nesne</param>
        /// <returns></returns>
        Sonuc ButceGiderKaydet(ObjectArray bilgi);
        /// <summary>
        /// Gider Bütçesi listele
        /// </summary>
        /// <param name="kriter">Kriter</param>
        /// <returns></returns>
        ObjectArray ButceGiderListele(ButceGider kriter);
        /// <summary>
        /// Gelir Bütçesi finansman kaydet
        /// </summary>
        /// <param name="bilgi">İşlem yapılacak bilgileri içeren nesne</param>
        /// <returns></returns>
        Sonuc ButceGelirFinansmanKaydet(ObjectArray bilgi);

        /// <summary>
        /// Gelir Bütçesi finansman listele
        /// </summary>
        /// <param name="kriter">kriter</param>
        /// <returns></returns>
        ObjectArray ButceGelirFinansmanListele(ButceGelirFinansman kriter);

        /// <summary>
        /// Yansıtma bilgileri kaydet
        /// </summary>
        /// <param name="yansitmalar">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        Sonuc YansitmaKaydet(ObjectArray yansitmalar);

        /// <summary>
        /// Yansıtma bilgileri sil
        /// </summary>
        /// <param name="yansitmalar">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        Sonuc YansitmaSil(ObjectArray yansitmalar);

        /// <summary>
        /// Yansitma bilgilerini listele
        /// </summary>
        /// <param name="yansitma">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        ObjectArray YansitmaListele(HesapPlaniYansitma yansitma);

        ///// <summary>
        ///// Yansitma listesini ebutceden aktar
        ///// </summary>
        ///// <param name="yil">Yıl</param>
        ///// <param name="kurum">Kurum kodu</param>
        ///// <param name="hesapKod">Hesap kodu</param>
        ///// <param name="yansitmaHesapKod">Yansitma hesap kodu</param>
        ///// <returns></returns>
        //Sonuc YansitmaEbutcedenAktar(string yil, string kurum, string hesapKod, string yansitmaHesapKod);

        /// <summary>
        /// Yansitma listele
        /// </summary>
        /// <param name="yansitma">Yansitma</param>
        /// <returns></returns>
        ObjectArray YansitmaEkoKodListele(HesapPlaniYansitma yansitma);
        /// <summary>
        /// Yansitma listesi aktar
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kaynakYil">Kaynak yil</param>
        /// <param name="hedefYil">Hedef yil</param>
        /// <returns></returns>
        Sonuc YansitmaAktar(Kullanici kullanan, int kaynakYil, int hedefYil);

        /// <summary>
        /// Muhasebe bilgilerini kaydet
        /// </summary>
        /// <param name="muhasebeBilgileri">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        Sonuc MuhasebeKaydet(ObjectArray muhasebeBilgileri);

        /// <summary>
        /// Muhasebe bilgisini sil
        /// </summary>
        /// <param name="muhasebeBilgileri">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        Sonuc MuhasebeSil(ObjectArray muhasebeBilgileri);

        /// <summary>
        /// Muhasebe bilgisi listele
        /// </summary>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <returns></returns>
        ObjectArray MuhasebeListele(MuhasebeBilgisi muhasebe);

        /// <summary>
        /// Muhasebe listesini ebutceden aktar.
        /// </summary>
        /// <param name="kod">Muhasebe Kodu</param>
        /// <param name="ad">Muhasebe Adı</param>
        /// <returns></returns>
        Sonuc MuhasebeBirimKaydet(ObjectArray muhasebeBirimler);

        /// <summary>
        /// Muhasebe birimi sil.
        /// </summary>
        /// <param name="muhasebeBirimler">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        Sonuc MuhasebeBirimSil(ObjectArray muhasebeBirimler);

        /// <summary>
        /// Muhasebe birim listele.
        /// </summary>
        /// <param name="muhasebeBirim">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        ObjectArray MuhasebeBirimListele(MuhasebeBirim muhasebeBirim);

        //MuhasebeBirim VNOdanMuhasebeBirimListele(MuhasebeBirim muhasebeBirim);

        ///// <summary>
        ///// Muhasebe birim bilgisini ebutceden aktar.
        ///// </summary>
        ///// <param name="kurum">Kurum kodu</param>
        ///// <param name="muhasebe">Muhasebe kodu</param>
        ///// <param name="kod">Muhasebe birim kodu</param>
        ///// <param name="ad">Muhasebe birim adı</param>
        ///// <returns></returns>
        //Sonuc MuhasebeBirimEbutcedenAktar(string kurum, string muhasebe, string kod, string ad);
        /// <summary>
        /// Sablon icerik tür liste.
        /// </summary>
        /// <returns></returns>
        ObjectArray SablonIcerikTurListe();
        /// <summary>
        /// Sablon listele.
        /// </summary>
        /// <param name="sablon">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        ObjectArray SablonListe(Sablon sablon);
        /// <summary>
        /// Sablon kaydet.
        /// </summary>
        /// <param name="sablon">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        Sonuc SablonKaydet(Sablon sablon);
        /// <summary>
        /// Sablon sil.
        /// </summary>
        /// <param name="sablon">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        Sonuc SablonSil(Sablon sablon);
        /// <summary>
        /// Ödenek durum bilgilerini listele.
        /// </summary>
        /// <param name="kriter">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        ObjectArray OdenekDurum(OdenekDurumKriter kriter);
        /// <summary>
        /// Ödenek durum bilgisi listesi.
        /// </summary>
        /// <param name="kriter">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        ObjectArray OdenekDurumListesi(OdenekDurumKriter kriter);
        /// <summary>
        /// Harcama durum bilgisi listele.
        /// </summary>
        /// <param name="kriter">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        ObjectArray HarcamaDurumListesi(Kullanici kullanan, OdenekDurumKriter kriter);
        /// <summary>
        /// Ödenek üstü harcama listesi.
        /// </summary>
        /// <param name="kriter">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        ObjectArray OdenekUstuHarcamaListesi(OdenekDurumKriter kriter);
        /// <summary>
        /// Gelir durum listesi.
        /// </summary>
        /// <param name="kriter">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        ObjectArray GelirDurumListesi(OdenekDurumKriter kriter);
        /// <summary>
        /// Muhasebattan gelen bilgilerden bütçe gideri fon eko liste.
        /// </summary>
        /// <param name="kriter">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        ObjectArray MuhasebatButceGideriFonEkoListe(OdenekDurumKriter kriter);
        /// <summary>
        /// Bütçe gider rakam olustur.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="serbestMi">Eğer <c>true</c> ise serbest ödenek tutarını ödenek kadar oluştur</param>
        /// <returns></returns>
        Sonuc ButceGiderRakamOlustur(TNS.KYM.Kullanici kullanan, int yil, bool serbestMi);
        /// <summary>
        /// Gelir Bütçesi rakam olustur.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="serbestMi">Eğer <c>true</c> ise serbest ödenek tutarını ödenek kadar oluştur</param>
        /// <returns></returns>
        Sonuc ButceGelirRakamOlustur(TNS.KYM.Kullanici kullanan, int yil, bool serbestMi);
        /// <summary>
        /// Harcama listele.
        /// </summary>
        /// <param name="kriter">kriter</param>
        /// <returns></returns>
        ObjectArray HarcamaListele(OdenekDurumKriter kriter);
        /// <summary>
        /// Butce durumu bilgilendirme.
        /// </summary>
        /// <param name="degerler">degerler</param>
        /// <returns></returns>
        ObjectArray ButceDurumuBilgilendirme(ObjectArray degerler);
        /// <summary>
        /// Verilen kriter bilgilerine göre bilanço raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="tur">Tur</param>
        /// <param name="fisTur">Fis tur</param>
        /// <param name="tarih1">The tarih1.</param>
        /// <param name="tarih2">The tarih2.</param>
        /// <returns></returns>
        ObjectArray Bilanco(TNS.KYM.Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, int tur, int fisTur, TNSDateTime tarih1, TNSDateTime tarih2);
        /// <summary>
        /// Verilen kriter bilgilerine göre mizan raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="tarih1">Başlangıç Tarihi</param>
        /// <param name="tarih2">Bitiş Tarihi</param>
        /// <param name="seviye">hesap kodu Seviyesi</param>
        /// <param name="basHesap">Başlangıç hesap kodu</param>
        /// <param name="bitHesap">Bitiş hesap kodu</param>
        /// <param name="tur">Tur</param>
        /// <param name="fisTur">Fis tur</param>
        /// <param name="listeTur">The liste tur.</param>
        /// <returns></returns>
        MizanBilgi Mizan(TNS.KYM.Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, int seviye, string basHesap, string bitHesap, int tur, ENUMFISTUR[] fisTur, int listeTur, string altHesap, bool ucgen = false);

        MizanBilgi MizanToplamBul(TNS.KYM.Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, int seviye, string basHesap, string bitHesap, int tur, ENUMFISTUR[] fisTur, int listeTur, string altHesap);

        Sonuc MuhasebeSorgu(TNS.KYM.Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, int seviye, string basHesap, string bitHesap, int tur, ENUMFISTUR[] fisTur, int listeTur, string kurKod, string fonKod, string altHesap, int borcAlacak);

        /// <summary>
        /// Verilen kriter bilgilerine göre birim bazında mizan raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="tarih1">Başlangıç Tarihi</param>
        /// <param name="tarih2">Bitiş Tarihi</param>
        /// <param name="seviye">hesap kodu Seviyesi</param>
        /// <param name="basHesap">Başlangıç hesap kodu</param>
        /// <param name="bitHesap">Bitiş hesap kodu</param>
        /// <param name="tur">Tur</param>
        /// <param name="fisTur">Fis tur</param>
        /// <param name="listeTur">The liste tur.</param>
        /// <param name="kurKod">The kur kod.</param>
        /// <param name="fonKod">The fon kod.</param>
        /// <returns></returns>
        MizanBilgi MizanBirim(TNS.KYM.Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, int seviye, string basHesap, string bitHesap, int tur, ENUMFISTUR[] fisTur, int listeTur, string kurKod = "", string fonKod = "", string altHesap = "");
        /// <summary>
        /// Verilen kriter bilgilerine göre tertip bazında mizan raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="tarih1">Başlangıç Tarihi</param>
        /// <param name="tarih2">Bitiş Tarihi</param>
        /// <param name="seviye">Hesap kodu Seviyesi</param>
        /// <param name="basHesap">Başlangıç hesap kodu</param>
        /// <param name="bitHesap">Bitiş hesap kodu</param>
        /// <param name="tur">Tur</param>
        /// <param name="fisTur">Fis tur</param>
        /// <param name="listeTur">The liste tur.</param>
        /// <returns></returns>
        MizanBilgi MizanTertip(TNS.KYM.Kullanici kullanan, int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, int seviye, string basHesap, string bitHesap, int tur, ENUMFISTUR[] fisTur, int listeTur, string altHesap, bool gercekEkoKodVer, int yevmiyeNo1, int yevmiyeNo2);
        /// <summary>
        /// Verilen kriter bilgilerine göre ödeme emri belgelerinde 830-905 hesap kodlarının kullanımını raporlar
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <returns></returns>
        OdemeEmriMIFForm Kontrol830905(TNS.KYM.Kullanici kullanan, int yil, int saymanlikKodu, string birim);
        /// <summary>
        /// Ödenek üstü raporunu hazırlar.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <returns></returns>
        OdemeEmriMIFForm OdenekUstu(TNS.KYM.Kullanici kullanan, int yil, int saymanlikKodu, string birim);
        /// <summary>
        /// Hatalı hesap kodu kullanılan ödeme emri belgelerini raporlar.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="saymanlikKodu">Saymanlık kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <returns></returns>
        OdemeEmriMIFForm UstHesapKodKullanilanlar(TNS.KYM.Kullanici kullanan, int yil, int saymanlikKodu, string birim);
        /// <summary>
        /// Verilen kriter bilgilerine göre büyük defter raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        BuyukDefter[] BuyukDefterListele(Kullanici kullanan, OdemeEmriMIFForm form, int basla, int bitis, string raporTur);
        /// <summary>
        /// Verilen kriter bilgilerine göre büyük defter devreden rakamlar raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        double[] BuyukDefterDevreden(Kullanici kullanan, OdemeEmriMIFForm form);
        /// <summary>
        /// Verilen kriter bilgilerine göre Kesin hesap (Gider tertip) raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        ObjectArray KesinHesapGiderTertipListele(Kullanici kullanan, OdemeEmriMIFForm form);
        /// <summary>
        /// Verilen kriter bilgilerine göre Kesin hesap (Gider) raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="kolonDeger">kolon deger</param>
        /// <returns></returns>
        ObjectArray KesinHesapGiderRakamVer(Kullanici kullanan, OdemeEmriMIFForm form, string[] kolonDeger);
        /// <summary>
        /// Verilen kriter bilgilerine göre Kesin hesap (Gelir tertip) raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        ObjectArray KesinHesapGelirTertipListele(Kullanici kullanan, OdemeEmriMIFForm form);
        /// <summary>
        /// Verilen kriter bilgilerine göre Kesin hesap (Gelir) raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        ObjectArray KesinHesapGelirRakamVer(Kullanici kullanan, OdemeEmriMIFForm form);
        /// <summary>
        /// Verilen kriter bilgilerine göre Kesin hesap (Bütçe) raporunu oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="kodTur">kod tur</param>
        /// <returns></returns>
        ObjectArray KesinHesapButce(Kullanici kullanan, OdemeEmriMIFForm form, int kodTur);
        /// <summary>
        /// Kullanıcıya ait Harcama birimlerini listeler
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yetkiTipiAdi">yetki tipi adi</param>
        /// <returns></returns>
        ObjectArray KullanicininHarcamaBirimleri(Kullanici kullanan, string yetkiTipiAdi);

        Sonuc MuhasebatHarcamaKaydet(Kullanici kullanan, int yil, int ay, int muhasebeKod, string birimKod, string vergiNo, HesapBilgi[] tolar, bool prg);

        ///// <summary>
        ///// Muhasebata ait harcama rakamlarını aktarma işlemi
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="yil">Yıl</param>
        ///// <param name="ay">ay</param>
        ///// <param name="kurum">Kurum kodu</param>
        ///// <returns></returns>
        //Sonuc MuhasebatAktar(Kullanici kullanan, string yil, string ay, string kurum);


        /// <summary>
        /// e-bütçe sisteminden Muhasebata ait harcama rakamlarını aktarma işlemi
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="ay">ay</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <returns></returns>
        Sonuc HarcamaAktar(Kullanici kullanan, string yil, string ay, string kurum);
        ///// <summary>
        ///// e-bütçe sisteminden Ödenek rakamlarını aktarır.
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="yil">Yıl</param>
        ///// <param name="asama">asama</param>
        ///// <param name="kurum">Kurum kodu</param>
        ///// <returns></returns>
        //Sonuc OdenekAktar(Kullanici kullanan, string yil, string asama, string kurum);
        ///// <summary>
        ///// e-bütçe sisteminden bütçe kodlarını aktarır
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="yil">Yıl</param>
        ///// <param name="kodTur">kod tur</param>
        ///// <param name="ustKod">ust kod</param>
        ///// <param name="seviye">seviye</param>
        ///// <returns></returns>
        //Sonuc ButceKodAktar(Kullanici kullanan, string yil, string kodTur, string ustKod, int seviye);
        ///// <summary>
        ///// e-bütçe sisteminden bütçe rakamlarını aktarır
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="yil">Yıl</param>
        ///// <param name="kurum">Kurum kodu</param>
        ///// <returns></returns>
        //Sonuc ButceAktar(Kullanici kullanan, string yil, string kurum);
        /// <summary>
        /// Ebutce kullanıcı bilgilerini ebütçe sistemine bağlantı kurarak test eder.
        /// </summary>
        /// <param name="kullanici">The kullanici.</param>
        /// <param name="parola">The parola.</param>
        /// <returns></returns>
        Sonuc EbutceParolaKontrol(string kullanici, string parola);
        ///// <summary>
        ///// e-bütçe sisteminden bütçe işlem rakamlarını aktarır
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="yil">Yıl</param>
        ///// <param name="kurum">Kurum kodu</param>
        ///// <returns></returns>
        //Sonuc ButceIslemAktar(Kullanici kullanan, int yil, string kurum);
        ///// <summary>
        ///// e-bütçe sisteminden AHP rakamlarını aktarır
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="yil">Yıl</param>
        ///// <param name="kurum">Kurum kodu</param>
        ///// <returns></returns>
        //Sonuc AHPAktar(Kullanici kullanan, int yil, string kurum);
        /// <summary>
        /// e-bütçe sisteminden Ödenek gönderme belgelerinin durumlarını kontrol eder
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="tur">tur</param>
        void OdenekGondermeEButcedenKontrol(Kullanici kullanan, int yil, int tur);
        /// <summary>
        /// Ödenek gönderme belgelerini e-bütçe sistemine kayıt eder
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        void OdenekGondermeBelgeleriGonder(Kullanici kullanan, int yil, ENUMBelgeTuru belgeTur, string belgeNolar);
        void OdenekGondermeEbutceyeGonder(Kullanici kullanan, int yil);
        ///// <summary>
        ///// e-bütçe sisteminde bulunan ödenek gönderme belgelerini listeler
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="yil">Yıl</param>
        ///// <param name="ay">ay</param>
        ///// <param name="tur">tur</param>
        //void OdenekGondermeEbutcedenListele(Kullanici kullanan, int yil, int ay, int tur);
        /// <summary>
        /// Makbuz tipi kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzTipleri">makbuz tipleri</param>
        /// <returns></returns>
        Sonuc MakbuzTipiKaydet(Kullanici kullanan, ObjectArray makbuzTipleri);
        /// <summary>
        /// Makbuz tipi sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzTipleri">makbuz tipleri</param>
        /// <returns></returns>
        Sonuc MakbuzTipiSil(Kullanici kullanan, ObjectArray makbuzTipleri);
        /// <summary>
        /// Makbuz tipi listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzTipi">makbuz tipi</param>
        /// <returns></returns>
        ObjectArray MakbuzTipiListele(Kullanici kullanan, MakbuzTipi makbuzTipi);
        /// <summary>
        /// Makbuz kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzlar">makbuzlar</param>
        /// <returns></returns>
        Sonuc MakbuzKaydet(Kullanici kullanan, Makbuz makbuz);
        /// <summary>
        /// Makbuz durum guncelle.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzlar">makbuzlar</param>
        /// <returns></returns>
        Sonuc MakbuzDurumGuncelle(Kullanici kullanan, TNSCollection makbuzlar);
        /// <summary>
        /// Makbuz odeme emri guncelle.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzlar">makbuzlar</param>
        /// <returns></returns>
        Sonuc MakbuzOdemeEmriGuncelle(Kullanici kullanan, ObjectArray makbuzlar);
        /// <summary>
        /// Makbuz listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuz">makbuz</param>
        /// <returns></returns>
        ObjectArray MakbuzListele(Kullanici kullanan, Makbuz makbuz);
        ObjectArray MakbuzListeleMizan(Kullanici kullanan, Makbuz makbuz);
        ObjectArray MakbuzBirimleriListele(Kullanici kullanan, Makbuz makbuz);
        ObjectArray MakbuzKisileriListele(Kullanici kullanan, Makbuz makbuz);
        /// <summary>
        /// Avans listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="avans">avans</param>
        /// <returns></returns>
        ObjectArray AvansListele(Kullanici kullanan, AvansBilgi avans);
        /// <summary>
        /// Ödenek ihtiyaç kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ihtiyac">ihtiyac</param>
        /// <returns></returns>
        Sonuc OdenekIhtiyacKaydet(Kullanici kullanan, ObjectArray ihtiyac);
        /// <summary>
        /// Ödenek ihtiyaç listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ihtiyac">ihtiyac</param>
        /// <returns></returns>
        ObjectArray OdenekIhtiyacListele(Kullanici kullanan, OdenekIhtiyac ihtiyac);
        /// <summary>
        /// Ödenek ihtiyaç icmal olarak listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ihtiyac">ihtiyac</param>
        /// <returns></returns>
        ObjectArray OdenekIhtiyacListeleIcmal(Kullanici kullanan, OdenekIhtiyac ihtiyac);
        /// <summary>
        /// Ödenek ihtiyaç onay kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="odenekIhtiyacOnay">odenek ihtiyac onay</param>
        /// <returns></returns>
        Sonuc OdenekIhtiyacOnayKaydet(Kullanici kullanan, OdenekIhtiyacOnay odenekIhtiyacOnay);
        /// <summary>
        /// Ödenek ihtiyaç onay listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="odenekIhtiyacOnay">odenek ihtiyac onay</param>
        /// <returns></returns>
        ObjectArray OdenekIhtiyacOnayListele(Kullanici kullanan, OdenekIhtiyacOnay odenekIhtiyacOnay);
        /// <summary>
        /// Muhasebe yetkilisi kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="muhasebeYetkilisi">muhasebe yetkilisi</param>
        /// <returns></returns>
        Sonuc MuhasebeYetkilisiKaydet(Kullanici kullanan, MuhasebeYetkilisi muhasebeYetkilisi);
        /// <summary>
        /// Muhasebe yetkilisi sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="muhasebeYetkilisi">muhasebe yetkilisi</param>
        /// <returns></returns>
        Sonuc MuhasebeYetkilisiSil(Kullanici kullanan, MuhasebeYetkilisi muhasebeYetkilisi);
        /// <summary>
        /// Muhasebe yetkilisi listele.
        /// </summary>
        /// <param name="muhasebeYetkilisi">muhasebe yetkilisi</param>
        /// <returns></returns>
        ObjectArray MuhasebeYetkilisiListele(MuhasebeYetkilisi muhasebeYetkilisi);
        /// <summary>
        /// Maaş bilgileri listele.(EGO için geliştirildi)
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        ObjectArray MaasEntegrasyonuListe(Kullanici kullanan, OdemeEmriMIFForm form);
        /// <summary>
        /// Maas bilgileri islendi.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="fisNo">fis no</param>
        void MaasEntegrasyonuIslendi(Kullanici kullanan, string fisNo);
        /// <summary>
        /// Harcamaların Aylık dağılımları.
        /// </summary>
        /// <param name="kriter">kriter</param>
        /// <returns></returns>
        ObjectArray AylikDagilimHarcama(AylikDagilim kriter);
        /// <summary>
        /// Ödenek gönderme  Aylık dağılımları.
        /// </summary>
        /// <param name="kriter">kriter</param>
        /// <returns></returns>
        ObjectArray AylikDagilimOdenekGonderme(AylikDagilim kriter);
        ObjectArray AylikDagilimHesapKod(Kullanici kullanan, OdemeEmriMIFForm kriter);
        ObjectArray YillikDagilimHesapKod(Kullanici kullanan, OdemeEmriMIFForm kriter);
        ObjectArray YillikAylikDagilimHesapKod(Kullanici kullanan, OdemeEmriMIFForm kriter);
        ObjectArray YillikAylikTarihselDagilimHesapKod(Kullanici kullanan, OdemeEmriMIFForm kriter);
        ObjectArray BirimGelirGider(Kullanici kullanan, OdemeEmriMIFForm kriter);
        ObjectArray GelirGiderListe(Kullanici kullanan, OdemeEmriMIFForm kriter);
        ObjectArray EkoKod1Basamak(Kullanici kullanan, OdemeEmriMIFForm kriter);
        ObjectArray BirimDagilimHesapKod(Kullanici kullanan, OdemeEmriMIFForm kriter);
        ObjectArray TasinirRaporu(Kullanici kullanan, OdemeEmriMIFForm kriter);
        /// <summary>
        /// Talep kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belge">belge</param>
        /// <returns></returns>
        Sonuc TalepKaydet(Kullanici kullanan, TalepBelge belge);

        ///// <summary>
        ///// Talep onay belgesi kaydet.
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="tertipListe">tertip liste</param>
        ///// <param name="talepYil">talep yil</param>
        ///// <param name="talepNo">talep no</param>
        ///// <param name="muhasebe">Muhasebe kodu</param>
        ///// <param name="birim">Birim kodu</param>
        ///// <param name="surecNo">Sürec no</param>
        ///// <returns></returns>
        //Sonuc TalepKaydetOnayBelgesi(Kullanici kullanan, ObjectArray tertipListe, int talepYil, int talepNo, int muhasebe, string birim, string surecNo);

        /// <summary>
        /// Talep tarihçe kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tarihce">tarihce</param>
        /// <returns></returns>
        Sonuc TalepTarihceKaydet(Kullanici kullanan, TalepTarihce tarihce);
        /// <summary>
        /// Talep listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belge">belge</param>
        /// <returns></returns>
        ObjectArray TalepListele(Kullanici kullanan, TalepBelge belge);
        /// <summary>
        /// Talep tek belge listele .
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="talepNo">talep no</param>
        /// <returns></returns>
        TalepBelge TalepListeleTekBelge(Kullanici kullanan, int yil, int talepNo);
        /// <summary>
        /// Talep durum değiştir.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="talepNo">talep no</param>
        /// <param name="talepFormDurum">talep form durum</param>
        /// <returns></returns>
        Sonuc TalepDurumDegistir(Kullanici kullanan, int yil, int talepNo, int talepFormDurum);
        /// <summary>
        /// Talep Ödenek gönderme belgesi kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="talepDetay">talep detay</param>
        /// <param name="yil">Yıl</param>
        /// <param name="talepNo">talep no</param>
        /// <returns></returns>
        Sonuc TalepOdenekGondermeKaydet(Kullanici kullanan, ObjectArray talepDetay, int yil, int talepNo);
        /// <summary>
        /// Kapanis bilgilerini kaydet
        /// </summary>
        /// <param name="kullanan">Kullanan.</param>
        /// <param name="liste">KapanisBilgileri türünde nesneleri içeren Liste.</param>
        /// <returns></returns>
        Sonuc KapanisBilgisiKaydet(Kullanici kullanan, ObjectArray liste);
        /// <summary>
        /// Kapanis bilgilerini iptal eder
        /// </summary>
        /// <param name="kullanan">Kullanan.</param>
        /// <param name="liste">KapanisBilgileri türünde nesneleri içeren Liste.</param>
        /// <returns></returns>
        Sonuc KapanisBilgisiIptalEt(Kullanici kullanan, ObjectArray liste);
        Sonuc KapanisBilgisiIptalEtHazirla(Kullanici kullanan, ObjectArray liste);
        ObjectArray KapanisBilgisiListele(KapanisBilgileri kriter);

        /// <summary>
        /// Kur farkı olan hesaplar için belge üretir
        /// </summary>
        /// <param name="kullanan">Kullanan.</param>
        /// <param name="belge">The belge.</param>
        /// <param name="muhasebeBirimineIsle">if set to <c>true</c> [muhasebe birimine isle].</param>
        /// <returns></returns>
        Sonuc KurFarkiBelgeUret(Kullanici kullanan, OdemeEmriMIFForm belge, bool muhasebeBirimineIsle);
        /// <summary>
        /// Oybses the kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <param name="oybsStr">The oybs STR.</param>
        /// <returns></returns>
        Sonuc OybsKaydet(Kullanici kullanan, int yil, string belgeNo, string oybsStr);
        /// <summary>
        /// Oybses the sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <returns></returns>
        Sonuc OybsSil(Kullanici kullanan, int yil, string belgeNo);
        /// <summary>
        /// Oybses the listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <returns></returns>
        ObjectArray OybsListele(Kullanici kullanan, int yil, string belgeNo);
        /// <summary>
        /// ISLs the harcama listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        ObjectArray ISLHarcamaListele(Kullanici kullanan, ISLHarcama bilgi);
        /// <summary>
        /// Odeneks the miktar getir.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="birim">The birim.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="kur">The kur.</param>
        /// <param name="fon">The fon.</param>
        /// <param name="fin">The fin.</param>
        /// <param name="eko">The eko.</param>
        /// <param name="peb">The peb.</param>
        /// <returns></returns>
        double OdenekMiktarGetir(Kullanici kullanan, int muhasebe, string birim, int yil, string kur, string fon, string fin, string eko, string peb = "");

        /// <summary>
        /// Odeneks the talep yapilacak mi.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="birim">The birim.</param>
        /// <param name="tertipler">The tertipler.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="surecYil">The surec yil.</param>
        /// <param name="surecNo">The surec no.</param>
        /// <param name="odemeEmriBelgeNo">The odeme emri belge no.</param>
        /// <returns></returns>
        EksikOdenekler OdenekTalepYapilacakMi(Kullanici kullanan, int muhasebe, string birim, TertipOdenekBilgi[] tertipler, int yil, int surecYil, string surecNo, string odemeEmriBelgeNo, bool odemeEmriMi, bool dogrudanTeminMi);

        /// <summary>
        /// Butces the odenek kapat.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="birim">The birim.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <param name="belgeTarih">The belge tarih.</param>
        /// <returns></returns>
        ObjectArray ButceOdenekKapat(Kullanici kullanan, int yil, int muhasebe, string birim, string belgeNo, TNSDateTime belgeTarih);
        /// <summary>
        /// Yevmiyes the defteri listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        //DataTable YevmiyeDefteriListele(Kullanici kullanan, OdemeEmriMIFForm form);
        ObjectArray OdemeEmriMIFTarihOncesiToplamGetir(Kullanici kullanan, OdemeEmriMIFForm form);
        ObjectArray YilSonuBelgeUret(Kullanici kullanan, OdemeEmriMIFForm form, int tur, string islem);
        ObjectArray YilSonuIslemleriListele(Kullanici kullanan, YilSonuIslem yilSonuIslem);

        //TJE***************************************
        ///// <summary>
        ///// MUHMifForm nesnesini veri tabanından okuyan fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">MUHMifForm yapısında MUHMifForm: bilgilerini tutan değişken</param>
        //JournalEntryFormDC ReadJournalEntry(Kullanici kullanan, JournalEntryFormDC form);
        ///// <summary>
        ///// MUHMifForm nesnesini veri tabanına yazan fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">MUHMifForm yapısında MUHMifForm: bilgilerini tutan değişken</param>
        ///// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        //Sonuc SaveJournalEntry(Kullanici kullanan, JournalEntryFormDC form);
        //TJE***************************************

        Sonuc BankaListesiDetayliKaydet(Kullanici kullanan, MUHBankaOdeme form);
        Sonuc BankaListesiEkle(Kullanici kullanan, int saymanlik, string birim, TNSDateTime dt, string aciklama, string muhYetkili, string muhYetUnvan, string onaylayan, string onayUnvan);
        Sonuc BankaListesiGuncelle(Kullanici kullanan, MUHBankaOdeme bilgi);
        Sonuc BankaListesiSil(Kullanici kullanan, MUHBankaOdeme bilgi);

        Sonuc BankaListesineOEEkle(Kullanici kullanan, MUHBankaOdeme bilgi, MUHBankaOdemeDetay detay);
        MUHBankaOdeme BankaListesiOku(Kullanici kullanan, MUHBankaOdeme bilgi);
        ObjectArray BankaListesiListele(Kullanici kullanan, int saymanlikKod, string birim, TNSDateTime basla, TNSDateTime bitis, string araMetin);
        //Sonuc BankaListesiDetayDurumGuncelle(Kullanici kullanan, MUHBankaOdeme bilgi, MUHBankaOdemeDetay detay);
        ObjectArray BankaListesiOdenecekEmirListele(Kullanici kullanan, int yil, int saymanlikKod, string birim, TNSDateTime yevBasTar, TNSDateTime yevBitTar);
        Sonuc BankaListesindenOESil(Kullanici kullanan, MUHBankaOdeme bilgi, MUHBankaOdemeDetay detay);
        Sonuc BankaListesiOnayla(Kullanici kullanan, int saymanlik, string birim, TNSDateTime gun, int siraNo, ENUMBankaListesiDurum onayKod, int donemNo);
        Sonuc BankaListesiDetayOdendi(Kullanici kullanan, int saymanlik, string birim, TNSDateTime gun, int siraNo, int sira, string dekontNo);
        IlgiliBilgi IlgiliBilgiBul(Kullanici kullanan, string tc_Vno, int saymanlikKod);
        YuzdeOnBilgi YuzdeOnBilgiBul(Kullanici kullanan, int yil, ENUMYuzdeOnTur MalHizmetYapim, int saymanlik, string birim, string haricOdemeEmriNo);

        OdemeEmriMIFForm[] OdemeEmriMIFSurecBelgeNoListele(Kullanici kullanan, int surecYil, string surecNo, int mifYil, string mifBelgeNo);

        MuhasebeBirim[] MuhasebeBirimListele(Kullanici kullanan, int saymanlikKod, string birimTurler);

        Sonuc OdemeEmriMIFSurecBagKur(Kullanici kullanan, int yil, string belgeNo, int surecYil, string surecNo, int muhasebeKod, string birimKod);

        Sonuc BirimHarcamaSayOE(Kullanici kullanan, int yil, int ay, int saymanlikKod, string birimKod);

        Sonuc OdemeEmriEvrakNoKaydet(Kullanici kullanan, int yil, string evrakNo, string odemeEmriNo, int durum);
        Sonuc OdemeEmriEvrakNoUret(Kullanici kullanan, int yil, string belgeNo);

        string OdemeEmriEvrakNoOku(int yil, string belgeNo);

        ObjectArray YuzdeOnListele(Kullanici kullanan, int yil);

        bool MuhasebeBirimYetkiVarMi(Kullanici kullanan, string muhasebe, string birim, TNS.MUH.ENUMOGY yetkiTur);

        AvansHareket[] AvansSorgula(Kullanici kullanan, AvansSorguKriter form);

        Sonuc TalepBelgeYapGonTen(Kullanici kullanan, TNSCollection formlar, bool sgbKbsKontrolYapma);

        EksikOdenekBilgi TertipDurumBul(Kullanici kullanan, int yil, int muhasebe, string birim, string tertip, string odemeEmriBelgeNo, int surecYil, string surecNo);

        GondermeTenkisDetay SatirDurumBul(Kullanici kullanan, int form_yil, string muhasebe, string birim, ENUMBelgeTuru belgeTur, GondermeTenkisDetay sd, bool planlananOdenek, bool talep);

        /// <summary>
        /// BagliEkoPeb listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BagliEkoPeb yapısında BagliEkoPeb:  bilgilerini tutan değişken</param>
        BagliEkoPeb[] ListeleBagliEkoPebAna(Kullanici kullanan, BagliEkoPeb form);

        /// <summary>
        /// BagliEkoPeb nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BagliEkoPeb yapısında BagliEkoPeb:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetBagliEkoPebAna(Kullanici kullanan, BagliEkoPeb form);

        Sonuc SilBagliEkoPebAna(Kullanici kullanan, BagliEkoPeb form);

        Sonuc BankaListesiBankayaGonderildi(Kullanici kullanan, MUHBankaOdeme mbO, string sonucDosyaAd);

        //Sonuc BankaListesiCekNoYaz(Kullanici kullanan, MUHBankaOdeme bilgi);

        int BankaDosyaSiraNoBul(Kullanici kullanan, string onAd);

        YuzdeOnBilgi YuzdeOnUstSinirBul(Kullanici kullanan, int yil, YuzdeOnBilgi yob);

        Sonuc BankaListesiCekNoOEEkle(Kullanici kullanan, MUHBankaOdeme bilgi, string p);

        Sonuc BankaListesiDetayOdendiToplu(Kullanici kullanan, MUHBankaOdeme bilgi, MUHBankaOdemeDetay[] mbod);

        void DetayBirimDoldur();

        string SaymanlikBirimKurkodBul(int saymanlik, string birim);

        ObjectArray AvansBelgeleriListele(Kullanici kullanan, OdemeEmriMIFForm kriter, bool mahsuplarDahilVer);
        ObjectArray MahsupEdilecekBelgeListele(OdemeEmriMIFForm oeForm, bool mahsupAlaniBosOlanlar);

        Sonuc BankaAkibetDosyasiIsle(Kullanici kullanan, TNSDateTime islenecekGun);

        /// <summary>
        /// GondermeTenkisIcmal listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">GondermeTenkisIcmal yapısında GondermeTenkisIcmal:  bilgilerini tutan değişken</param>
        ObjectArray ListeleOdenekGondermeIcmal(Kullanici kullanan, GondermeTenkisIcmal form);

        /// <summary>
        /// GondermeTenkisIcmal nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">GondermeTenkisIcmal yapısında GondermeTenkisIcmal:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetOdenekGondermeIcmal(Kullanici kullanan, GondermeTenkisIcmal form);

        /// <summary>
        /// GondermeTenkisIcmal nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="formlar">GondermeTenkisIcmal yapısında GondermeTenkisIcmal:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc DurumAtaOdenekGondermeIcmal(Kullanici kullanan, TNSCollection formlar);

        Sonuc TertiplerUygunMu(Kullanici kullanan, TNSCollection tertipler);

        ObjectArray MuhasebeBirimListeleSablondan(Kullanici kullanan, string sablon);

        Sonuc AltHesapDegistir(Kullanici kullanan, int yil, string belgeNo, int sirano, string altHesapKod);

        ObjectArray KBSAktarimListele(Kullanici kullanan, int yil);
        Sonuc KBSAktarimKaydet(Kullanici kullanan, ObjectArray liste, int yil);

        Sonuc MakbuzAciklamaKaydet(Kullanici kullanan, MakbuzAciklama bilgi);
        ObjectArray MakbuzAciklamaListele(MakbuzAciklama bilgi);
        Sonuc MakbuzAciklamaSil(Kullanici kullanan, MakbuzAciklama bilgi);

        Sonuc MakbuzOdendi(Kullanici kullanan, Makbuz makbuz);

        Sonuc TaahhutKartiKaydet(Kullanici kullanan, TaahhutKartiBilgi taahhütListesi, TNSCollection taahhutBilgiler);
        ObjectArray TaahhutKartiListele(Kullanici kullanan, TaahhutKartiBilgi taahhut, int personel1Nrml0Yok2Hepsi);
        Sonuc TaahhutKartiSil(Kullanici kullanan, int kod);
        Sonuc TaahhutKartiKapat(Kullanici kullanan, TNSCollection kodlar, int durum);
        Sonuc TaahhutKartiOdemeEmriKaydet(Kullanici kullanan, int kartYil, int kartKod, int odemeEmriYil, string odemeEmriNo);
        Sonuc TaahhutKartiDegerArtisiOdemeEmriKaydet(Kullanici kullanan, string kod, int taahhukKod, int odemeEmriYil, string odemeEmriNo);
        Sonuc TaahhutKartiDegerArtisiKaydet(Kullanici kullanan, TaahhutKartiDegerArtisi da, TNSCollection taahhutBilgiler);
        ObjectVAN TaahhutKartiDegerArtisiAcilabilirMi(Kullanici kullanan, TaahhutKartiDegerArtisi da);
        Sonuc TaahhutKaydet(Kullanici kullanan, int taahhutKod, string degerArtisiKod, TNSCollection taahhutListesi);
        ObjectArray TaahhutListele(Kullanici kullanan, TaahhutBilgi taahhut);
        ObjectArray TaahhutToplamListele(Kullanici kullanan, int taahhutKod);
        Sonuc TaahhutSil(Kullanici kullanan, int kod);
        Sonuc TaahhutTeminatlarKaydet(Kullanici kullanan, ObjectArray teminatListesi);
        ObjectArray TaahhutTeminatlarListele(Kullanici kullanan, TaahhutTeminatlar teminat);
        Sonuc TaahhutTeminatlarSil(Kullanici kullanan, ObjectArray teminatListesi);
        Sonuc TaahhutAvanslarKaydet(Kullanici kullanan, ObjectArray liste);
        ObjectArray TaahhutAvanslarListele(Kullanici kullanan, TaahhutAvanslar form);
        Sonuc TaahhutAvanslarSil(Kullanici kullanan, ObjectArray avansListesi);
        Sonuc TaahhutBorclarTanimKaydet(Kullanici kullanan, TaahhutBorclarTanim borclar);
        ObjectArray TaahhutBorclarTanimListele(Kullanici kullanan, TaahhutBorclarTanim borclar);
        Sonuc TaahhutBorclarTanimSil(Kullanici kullanan, ObjectArray borcListesi);
        Sonuc TaahhutOdemeEmriKaydet(Kullanici kullanan, ObjectArray odemeEmriListesi);
        ObjectArray TaahhutOdemeEmriListele(Kullanici kullanan, TaahhutOdeme form);
        double TaahhutOdemeEmriHazirlananToplamTutar(Kullanici kullanan, int taahhutKod);
        double TaahhutOdemeEmriHazirlananKapatmaFarki(Kullanici kullanan, int taahhutKod);
        Sonuc TaahhutOdemeEmriSil(Kullanici kullanan, int kod);
        Sonuc MutemetTanimKaydet(Kullanici kullanan, MutemetTanim form);
        ObjectArray MutemetTanimListele(Kullanici kullanan, MutemetTanim form);
        Sonuc MutemetTanimSil(Kullanici kullanan, ObjectArray mutemetListesi);
        Sonuc MutemetAktar(Kullanici kullanan, int kaynakYil, int hedefYil);

        Sonuc HesapPlaniKisitKaydet(Kullanici kullanan, HesapPlaniKisit form);
        ObjectArray HesapPlaniKisitListele(Kullanici kullanan, HesapPlaniKisit form);
        Sonuc HesapPlaniKisitSil(Kullanici kullanan, ObjectArray liste);
        Sonuc HesapPlaniKisitAktar(Kullanici kullanan, int kaynakYil, int hedefYil);

        /// <summary>
        /// Belges the istatistik kod ata odemif.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="fler">The fler.</param>
        /// <param name="ist1">The ist1.</param>
        /// <param name="ist2">The ist2.</param>
        /// <returns></returns>
        Sonuc BelgeIstatistikKodAtaODEMIF(Kullanici kullanan, TNSCollection fler, int ist1, int ist2);
        /// <summary>
        /// IstatistikKod listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IstatistikKod yapısında IstatistikKod: İstatistik Kodu bilgilerini tutan değişken</param>
        /// <param name="sifirEkle">if set to <c>true</c> [sifir ekle].</param>
        /// <param name="eksibirEkle">if set to <c>true</c> [eksibir ekle].</param>
        /// <returns></returns>
        ObjectArray ListeleIstatistikTanim(Kullanici kullanan, IstatistikKod form, bool sifirEkle, bool eksibirEkle, bool detaylarOlsun);

        /// <summary>
        /// IstatistikKod nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IstatistikKod yapısında IstatistikKod: İstatistik Kodu bilgilerini tutan değişken</param>
        /// <param name="sifirEkle">if set to <c>true</c> [sifir ekle].</param>
        /// <param name="eksibirEkle">if set to <c>true</c> [eksibir ekle].</param>
        /// <returns></returns>
        IstatistikKod OkuIstatistikKodGiris(Kullanici kullanan, IstatistikKod form, bool sifirEkle, bool eksibirEkle);

        /// <summary>
        /// IstatistikKod nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IstatistikKod yapısında IstatistikKod: İstatistik Kodu bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetIstatistikKodGiris(Kullanici kullanan, IstatistikKod form);
        Sonuc KaydetIstatistikKodTanim(Kullanici kullanan, IstatistikKod form);

        Sonuc TaahhutBorclarKaydet(Kullanici kullanan, ObjectArray borclarListe);
        ObjectArray TaahhutBorclarListele(Kullanici kullanan, TaahhutBorclar borclar);
        Sonuc TaahhutBorclarSil(Kullanici kullanan, ObjectArray borcListesi);

        Sonuc DamgaVergisiAktarimTipiKaydet(Kullanici kullanan, ObjectArray aktarimTipleri);
        Sonuc DamgaVergisiAktarimTipiSil(Kullanici kullanan, ObjectArray aktarimTipleri);
        ObjectArray DamgaVergisiAktarimTipiListele(Kullanici kullanan, VergiDairesiAktarimTurTanim aktarimTipi);

        Sonuc KBSAktarimDetayKaydet(Kullanici kullanan, ObjectArray liste, int yil);
        ObjectArray KBSAktarimDetayListele(Kullanici kullanan, int yil);
        Sonuc MahsupYapilmamisBelgeBul(Kullanici kullanan, int yil, string hesapKod, string altHesapKod, bool borclu, string belgeNo, double miktar);
        ObjectVAN MahsupSatiriBul(Kullanici kullanan, int yil, string mahsupBelgeNo, string hesapKod, string altHesapKod, bool borclu, double miktar);

        ObjectArray KapaliBirimListele(Kullanici kullanan, MuhasebeBirim form);
        Sonuc BirimAcKapat(Kullanici kullanan, MuhasebeBirim form, bool kapat);
        ObjectArray TaahhutListele(Kullanici kullanan, OdemeEmriMIFForm form);

        Sonuc FaturaKaydet(Kullanici kullanan, Fatura bilgi);
        ObjectArray FaturaListele(Kullanici kullanan, Fatura bilgi);
        Sonuc FaturaDurumGuncelle(Kullanici kullanan, Fatura bilgi);
        Sonuc FaturaMIFBelgeNoGuncelle(Kullanici kullanan, Fatura bilgi);
        Sonuc FaturaMuhasebeAciklamaKaydet(Kullanici kullanan, Fatura bilgi);
        Sonuc FaturaAciklamaKaydet(Kullanici kullanan, FaturaAciklama bilgi);
        ObjectArray FaturaAciklamaListele(Kullanici kullanan, FaturaAciklama bilgi);
        Sonuc FaturaAciklamaSil(Kullanici kullanan, FaturaAciklama bilgi);
        //Sonuc FaturaAciklamaKontrol(string tip, Kullanici kullanan, FaturaAciklama bilgi);
        //Sonuc FaturaOdenenTakipGuncelle(Kullanici kullanan, Fatura bilgi);

        Sonuc MudurlukKaydet(Kullanici kullanan, Mudurluk bilgi);
        ObjectArray MudurlukListele(Kullanici kullanan, Mudurluk bilgi);
        Sonuc MudurlukSil(Kullanici kullanan, Mudurluk bilgi);

        Sonuc PlakaKaydet(Kullanici kullanan, Plaka bilgi);
        ObjectArray PlakaListele(Kullanici kullanan, Plaka bilgi);
        Sonuc PlakaSil(Kullanici kullanan, Plaka bilgi);

        string OdenekGondermeSorgu(int yil, int muhasebe, string birim, int durumAralik, ENUMBelgeTuru belgeTur, bool faaliyetli, string kur, string fon, string fin, string eko);
        bool BirimKapaliMi(Kullanici kullanan, int saymanlik, string birimKod, string kurKod);

        Sonuc BankaListesiBasilanCekNoAta(Kullanici kullanan, MUHBankaOdeme bilgi, MUHBankaOdemeDetay detay);

        ObjectArray IstisnaRaporlari(Kullanici kullanan, SGBKBSFark sgbKbsFark);

        Sonuc ImzaTanimKaydet(Kullanici kullanan, ImzaTanim form);
        Sonuc ImzaTanimSil(Kullanici kullanan, ImzaTanim form);
        ImzaTanim[] ImzaTanimListele(Kullanici kullanan, ImzaTanim form);
        ImzaTanim ImzaTanimOku(Kullanici kullanan, ImzaTanim form);
        Sonuc ImzaTanimGecerliImzaYap(Kullanici kullanan, int imzaTanimKod, string birimKod);

        Sonuc AciklamaKaydet(Kullanici kullanan, AciklamaBilgi bilgi);
        ObjectArray AciklamaListele(Kullanici kullanan, AciklamaBilgi bilgi);
        Sonuc AciklamaSil(Kullanici kullanan, AciklamaBilgi bilgi);

        /// <summary>
        /// CekKocani listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CekKocani yapısında CekKocani: Çek Koçanındaki Sayfalar bilgilerini tutan değişken</param>
        ObjectArray ListeleCekKocani(Kullanici kullanan, CekKocani form, bool acik, bool bozdurulmus, bool iptalli, TNSDateTime baslaTarih, TNSDateTime bitisTarih);

        /// <summary>
        /// CekKocani nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CekKocani yapısında CekKocani: Çek Koçanındaki Sayfalar bilgilerini tutan değişken</param>
        //ObjectVAN OkuCekKocani(Kullanici kullanan, CekKocani form);

        /// <summary>
        /// CekKocani nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CekKocani yapısında CekKocani: Çek Koçanındaki Sayfalar bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetCekKocani(Kullanici kullanan, CekKocani form);
        Sonuc SilCekKocani(Kullanici kullanan, CekKocani form);

        /// <summary>
        /// Sayistays the veri kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        Sonuc SayistayVeriKaydet(Kullanici kullanan, SayistayVeri form);
        /// <summary>
        /// Sayistays the veri listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        ObjectArray SayistayVeriListele(Kullanici kullanan, SayistayVeri form);
        /// <summary>
        /// Sayistays the veri sil.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="veri">The veri.</param>
        /// <returns></returns>
        Sonuc SayistayVeriSil(Kullanici kullanan, SayistayVeri veri);

        ObjectArray KapatmaListele(Kullanici kullanan, int yil, string belgeNo);
        Sonuc OdenekGondermeAktarimIslemleri(Kullanici kullanici, int yil);

        Sonuc IsKaydet(Kullanici kullanan, IsTanim form);
        Sonuc IsSil(Kullanici kullanan, IsTanim form);
        ObjectArray IsListele(Kullanici kullanan, IsTanim form);

        Sonuc IsOdenekKaydet(Kullanici kullanan, IsOdenek[] liste);
        Sonuc IsOdenekSil(Kullanici kullanan, IsOdenek form);
        ObjectArray IsOdenekListele(Kullanici kullanan, IsOdenek form);

        ObjectArray IsOdenekDurumListele(Kullanici kullanan, int yil, int muhasebe, string birim, int isKod);

        ObjectArray IsBlokeOdenekListele(Kullanici kullanan, IsTanim form);

        Sonuc TaahhutKartiSozlesmeKaydet(Kullanici kullanan, TaahhutKartiSozlesme form);
        Sonuc TaahhutKartiSozlesmeSil(Kullanici kullanan, TaahhutKartiSozlesme form);
        ObjectArray TaahhutKartiSozlesmeListele(Kullanici kullanan, TaahhutKartiSozlesme form);

        Sonuc TaahhutKartiBakimTarihceKaydet(Kullanici kullanan, TaahhutKartiBakimTarihce form);
        Sonuc TaahhutKartiBakimTarihceSil(Kullanici kullanan, TaahhutKartiBakimTarihce form);
        ObjectArray TaahhutKartiBakimTarihceListele(Kullanici kullanan, TaahhutKartiBakimTarihce form);

        Sonuc OrtakBelgeKaydet(Kullanici islemiYapan, OrtakBelge form);
        ObjectArray OrtakBelgeListele(Kullanici islemiYapan, OrtakBelge form);
        OrtakBelge OrtakBelgeOku(Kullanici islemiYapan, OrtakBelge form);
        Sonuc OrtakBelgeSil(Kullanici islemiYapan, OrtakBelge form);
        Sonuc TaahhutKartiOdemeEmriUret(Kullanici kullanan, TaahhutKartiBilgi bilgi);
        Sonuc TaahhutKartiOdemeEmriUret(Kullanici kullanan, int sozKod, string degerArtisiKod);

        Sonuc EntegrasyonMIFKaydet(Kullanici islemiYapan, OdemeEmriMIFForm form);
        Sonuc EntegrasyonMIFGuncelleDurum(Kullanici islemiYapan, OdemeEmriMIFForm form);
        ObjectArray EntegrasyonMIFListele(Kullanici islemiYapan, OdemeEmriMIFForm form, bool getirDetay);
        Sonuc EntegrasyonMIFSil(Kullanici islemiYapan, OdemeEmriMIFForm form);

        Sonuc MIFEntegrasyonGonder(Kullanici kullanan, ObjectArray MIFListe);

        ObjectArray ManasGunlukTakipGetir(Kullanici kullanan, ManasGunlukTakip kriter);

        Sonuc CaprazTabloKaydet(Kullanici kullanan, CaprazTablo bilgi);

        Sonuc CaprazTabloSil(Kullanici kullanan, CaprazTablo bilgi);

        ObjectArray CaprazTabloListele(Kullanici kullanan, CaprazTablo bilgi);

        ObjectArray CaprazTabloRaporListele(Kullanici kullanan, CaprazTablo bilgi);

        ObjectArray CaprazTabloRaporHesapKodListele(Kullanici kullanan, CaprazTablo bilgi);

        ObjectArray CaprazTabloRaporHesapKodListele2(Kullanici kullanan, CaprazTablo bilgi);

        Sonuc TalepTanimKaydet(Kullanici kullanan, TalepTanimForm bilgi);

        Sonuc TalepTanimSil(Kullanici kullanan, TalepTanimForm bilgi);

        ObjectArray TalepTanimListele(Kullanici kullanan, TalepTanimForm bilgi);


        Sonuc TkdkTalepKaydet(Kullanici kullanan, TkdkTalepForm bilgi);
        Sonuc TkdkTalepTertipKaydet(Kullanici kullanan, ObjectArray liste);
        Sonuc TkdkTalepGuncelleDurum(Kullanici kullanan, ObjectArray bilgi);

        Sonuc TkdkTalepSil(Kullanici kullanan, TkdkTalepForm bilgi);

        ObjectArray TkdkTalepListele(Kullanici kullanan, TkdkTalepForm bilgi);

        Sonuc TkdkTalepOdemeEmriGuncelle(Kullanici kullanan, TkdkTalepForm bilgi);

        Sonuc FaturaTakipKaydet(Kullanici kullanan, FaturaTakip form);
        ObjectArray FaturaTakipListele(Kullanici kullanan, FaturaTakip form);
        Sonuc BIBagKopart(Kullanici kullanan, int revizeYil, int revizeNo);
        Sonuc BIDuzelt(Kullanici kullanan, TNS.BKD.MPRevizeForm f);
        Sonuc MuhasebeBirimiTasi(Kullanici kullanan, int eskiSaymanlikKod, string eskiBirimKod, int yeniSaymanlikKod, string yeniBirimKod, TNSCollection islemListesi);
        Sonuc EbutceAktarimBilgiOku();
    }
}
