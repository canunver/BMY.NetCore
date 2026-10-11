using System;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.BKD;
using TNS.BHMv2;
using System.Collections;

namespace TNS.YAT
{
    public enum ENUMKarakteristikTur : int
    {
        Sade = 0,
        ArkadaZorunlu = 1,
        OndeZorunlu = 2,
        ArkadaSerbest = 3,
        OndeSerbest = 4
    }
    /// <summary>
    /// YollukPlan: Yolluk Plan Detay
    /// </summary>
    [System.Serializable()]
    public class YollukPlan
    {

        /// <summary>
        /// SIRANO: Alt Faaliyet Numarası
        /// </summary>
        public int sIRANO { get; set; }

        /// <summary>
        /// Dairesi: Düzenleyecek dairenin ismi
        /// </summary>
        public string dairesi { get; set; }

        /// <summary>
        /// Konusu: Düzenlenecek eğitim/toplatının konusu
        /// </summary>
        public string konusu { get; set; }

        /// <summary>
        /// Muhtelif Mi: Muhtelif mi
        /// </summary>
        public bool muhtelifMi { get; set; }

        /// <summary>
        /// Süresi: Bir kişinin kalacağı ortalama, yol hariç, süre
        /// </summary>
        public int suresi { get; set; }

        /// <summary>
        /// Kişi Sayısı: Toplam katılacak kişi sayısı
        /// </summary>
        public int kisiSayisi { get; set; }

        /// <summary>
        /// Yapılacağı Ay: Yapılacağı ay
        /// </summary>
        public string yapilacagiAy { get; set; }

        /// <summary>
        /// Yeri: Yeri
        /// </summary>
        public string yeri { get; set; }

        /// <summary>
        /// KURKOD: Kurumsal Kod
        /// </summary>
        public string kURKOD { get; set; }

        /// <summary>
        /// FONKOD: Fonksiyonel Kod
        /// </summary>
        public string fONKOD { get; set; }

        /// <summary>
        /// FINKOD: Finansman Kodu
        /// </summary>
        public string fINKOD { get; set; }

        /// <summary>
        /// EKOKOD: Ekonomik Kod
        /// </summary>
        public string eKOKOD { get; set; }

        /// <summary>
        /// EKOKOD: Ekonomik Kod
        /// </summary>
        public string pebKod { get; set; }

        /// <summary>
        /// MALIYET: Maliyet
        /// </summary>
        public double mALIYET { get; set; }

        public string tertip
        {
            get { return kURKOD + "-" + fONKOD + "-" + fINKOD + "-" + eKOKOD + "-" + pebKod; }
            set
            {
                string[] parcalar = null;
                if (!string.IsNullOrWhiteSpace(value))
                    parcalar = value.Trim().Split('-');
                kURKOD = OrtakFonksiyonlar.DiziElemanAl(parcalar, 0, "");
                fONKOD = OrtakFonksiyonlar.DiziElemanAl(parcalar, 1, "");
                fINKOD = OrtakFonksiyonlar.DiziElemanAl(parcalar, 2, "");
                eKOKOD = OrtakFonksiyonlar.DiziElemanAl(parcalar, 3, "");
                pebKod = OrtakFonksiyonlar.DiziElemanAl(parcalar, 4, "");
            }
        }
        public string altProjeAdi { get; set; }
        public string yurutenBirim { get; set; }

        public string faaliyetAd { get; set; }

        public string faaliyetBirimAd { get; set; }
    }

    [Serializable]
    public class KarakteristikGiris
    {
        public int ekTurKod { get { return (int)ekTur; } set { ekTur = (ENUMKarakteristikTur)value; } }
        public ENUMKarakteristikTur ekTur { get; set; }
        public bool sec { get; set; }
        public string deger { get; set; }
        public string degerKisa { get; set; }
        public string ekDeger { get; set; }
        public string sira { get; set; }

        public KarakteristikGiris()
        {
            this.ekTur = ENUMKarakteristikTur.Sade;
            this.deger = "";
            this.degerKisa = "";
        }

        public KarakteristikGiris(ENUMKarakteristikTur ek, string deger, string degerKisa)
        {
            this.ekTur = ek;
            this.deger = deger;
            this.degerKisa = degerKisa;
        }

        public void EkDegerAta(string veri)
        {
            ekDeger = "";
            if (EsitMi(veri))
            {
                if (ekTur == ENUMKarakteristikTur.ArkadaZorunlu || ekTur == ENUMKarakteristikTur.ArkadaSerbest) //değer önde, ek değer arkada (3 ise zorunlu değil)
                {
                    ekDeger = veri.Substring(deger.Length).Trim();
                }
                else if (ekTur == ENUMKarakteristikTur.OndeZorunlu || ekTur == ENUMKarakteristikTur.OndeSerbest) //ek değer önde, değer arkada (4 ise zorunlu değil)
                {
                    ekDeger = veri.Substring(0, veri.Length - deger.Length).Trim();
                }
            }
        }

        public bool EsitMi(string veri)
        {
            if (ekTur == ENUMKarakteristikTur.Sade) return deger == veri;
            else if (ekTur == ENUMKarakteristikTur.ArkadaZorunlu || ekTur == ENUMKarakteristikTur.ArkadaSerbest) return veri.StartsWith(deger);
            else if (ekTur == ENUMKarakteristikTur.OndeZorunlu || ekTur == ENUMKarakteristikTur.OndeSerbest) return veri.EndsWith(deger);
            return false;
        }
    }

    public enum ENUMProjeMaliYapiKod : int
    {
        harcamaTahmin = -7,
        odenekDusulen = -6,
        odenekEklenen = -5,
        harcamaYilSonu = -4,
        harcama6ay = -3,
        kumulatifHarcama = -2,
        kaynakToplam = -1,
        teklif0 = 0,
        teklif1 = 1,
        teklif2 = 2,
        ekteklif0 = 3,
        ekteklif1 = 4,
        ekteklif2 = 5,
        sonrakiYillar = 6,
        odenekEklenenDusulen = 10,
        TANIMSIZ = -99,
    }

    /// <summary>
    /// Alt projelere eklenebilecek bütçe kaynakların türlerini tutan liste
    /// </summary>
    public enum ENUMButceKaynakTur : int
    {
        /// <summary>
        /// Türü bütçe olan bütçe kaynağı
        /// </summary>
        BUTCE = 1,

        /// <summary>
        /// Bütçede gösterilen kredi bütçe kaynağı türü
        /// </summary>
        KREDIBUTCEICI = 2,

        /// <summary>
        /// Bütçede gösterilmeyen kredi bütçe kaynağı türü
        /// </summary>
        KREDIBUTCEDISI = 3,

        /// <summary>
        /// Hibe bütçe kaynağı türü
        /// </summary>
        HIBE = 4,

        /// <summary>
        /// Döner Sermaye bütçe kaynağı türü
        /// </summary>
        DONERSERMAYE = 5,

        /// <summary>
        /// Yeni alt proje açıldığında detay bilgilerinin girilmesi için
        /// </summary>
        YENIPROJE = 6
    }

    /// <summary>
    /// Proje türlerine göre ödenek toplamı türlerini tutan liste
    /// </summary>
    public enum ENUMTeklifToplamTur : int
    {
        /// <summary>
        /// Etüd proje türü için ödenek toplamı türü
        /// </summary>
        ETUDPROJE = 0,

        /// <summary>
        /// Devam eden proje türü için ödenek toplamı türü
        /// </summary>
        DEVAMEDENPROJE = 1,

        /// <summary>
        /// Yeni proje türü için ödenek toplamı türü
        /// </summary>
        YENIPROJE = 2
    }

    /// <summary>
    /// Sözlük tanımlamalarında bu işlem türlerine göre ilgili işlem yapılıyor. Sözlük işlem türlerini tutan liste
    /// </summary>
    public enum ENUMSozlukIslemTur : int
    {
        /// <summary>
        /// Sözlük kaydetme işlemi türü
        /// </summary>
        KAYDET = 0,

        /// <summary>
        /// Sözlük silme işlemi türü
        /// </summary>
        SIL = 1
    }

    /// <summary>
    /// Proje ve alt projelerin modellerini tutan liste
    /// </summary>
    public enum ENUMModel : int
    {
        /// <summary>
        /// Yap işlet devret modeli
        /// </summary>
        YID = 1,

        /// <summary>
        /// Yap işlet modeli
        /// </summary>
        YI = 2,

        /// <summary>
        /// Normal modeli
        /// </summary>
        NORMAL = 3
    }

    [Serializable()]
    public class KrediDetay
    {
        public int butceYil { get; set; }
        public string senaryoKod { get; set; }
        public int kaynakNo { get; set; }
        public int yil { get; set; }
        public double anlasmaTutari { get; set; }
        public double kullanilanTutar { get; set; }
        public double kullanilacakTutar { get; set; }
        public double anlasmaTutariIc { get; set; }
        public double kullanilanTutarIc { get; set; }
        public double kullanilacakTutarIc { get; set; }
    }

    /// <summary>
    /// Proje Türü, Döküman Türü ve İl sözlük tanımlarını tutan sınıf
    /// </summary>
    [Serializable()]
    public class Sozluk
    {
        // override object.Equals
        public override bool Equals(object obj)
        {

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Sozluk gelen = (Sozluk)obj;
            return gelen.kod.Equals(this.kod);
        }

        /// <summary>
        /// Sözlük kodu
        /// </summary>
        public int kod { get; set; }

        /// <summary>
        /// Sözlük adı
        /// </summary>
        public string ad { get; set; }

        /// <summary>
        /// Sözlük aciklamasi (Sadece düzey 2 bölge kodlu DPT Tablo 2 raporunu almak için il tanımında kullanılıyor.)
        /// </summary>
        public string aciklama { get; set; }

        /// <summary>
        /// Parametresiz Sözlük nesnesi oluşturucu yordam
        /// </summary>
        public Sozluk()
        {
            kod = 0;
            ad = "";
            aciklama = "";
        }

        /// <summary>
        /// Kod ve ad parametreleri alan Sözlük nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="kod">Sözlük kodu</param>
        /// <param name="ad">Sözlük adı</param>
        public Sozluk(int kod, string ad)
        {
            this.kod = kod;
            this.ad = ad;
        }

        /// <summary>
        /// Kod, ad ve açıklama parametreleri alan Sözlük nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="kod">Sözlük kodu</param>
        /// <param name="ad">Sözlük adı</param>
        /// <param name="aciklama">Sözlük açıklaması</param>
        public Sozluk(int kod, string ad, string aciklama)
        {
            this.kod = kod;
            this.ad = ad;
            this.aciklama = aciklama;
        }

        public int say { get; set; }
    }

    /// <summary>
    /// İlçe sözlük tanımını tutan sınıf
    /// </summary>
    [Serializable()]
    public class IlceSozluk
    {
        /// <summary>
        /// İlçenin ili
        /// </summary>
        public Sozluk il;

        /// <summary>
        /// İlçe kodu
        /// </summary>
        public int ilceKodu { get; set; }

        /// <summary>
        /// İlçe adı
        /// </summary>
        public string ilceAdi { get; set; }

        /// <summary>
        /// Parametresiz İlçe nesnesi oluşturucu yordam
        /// </summary>
        public IlceSozluk()
        {
            il = new Sozluk();
            ilceKodu = 0;
            ilceAdi = "";
        }

        /// <summary>
        /// İl, kod ve ad parametreleri alan İlçe nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="il">İlçenin ili</param>
        /// <param name="ilceKodu">İlçe kodu</param>
        /// <param name="ilceAdi">İlçe adı</param>
        public IlceSozluk(Sozluk il, int ilceKodu, string ilceAdi)
        {
            this.il = il;
            this.ilceKodu = ilceKodu;
            this.ilceAdi = ilceAdi;
        }
    }

    [Serializable()]
    public class AltFaalBilgi
    {
        public string pebKod { get; set; }
        public string pebAd { get; set; }
        public SektorSozluk sektor { get; set; }
        public AltProje altProje { get; set; }

        public Proje proje { get; set; }

        public TNSCollection iller { get; set; }
    }

    /// <summary>
    /// Sektör sözlük tanımını tutan sınıf
    /// </summary>
    [Serializable()]
    public class SektorSozluk
    {
        /// <summary>
        /// Sektör kodu
        /// </summary>
        public string kod;

        /// <summary>
        /// Sektör adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Sektörün alt sektörleri olup olmadığını tutan özellik (Varsa "E", yoksa "H")
        /// </summary>
        public string altSektorVarMi;

        /// <summary>
        /// Parametresiz Sektör nesnesi oluşturucu yordam
        /// </summary>
        public SektorSozluk()
        {
            kod = "";
            ad = "";
            altSektorVarMi = "";
        }

        /// <summary>
        /// Kod, ad ve alt sektör var mı, yok mu parametreleri alan Sektör nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="kod">Sektör kodu</param>
        /// <param name="ad">Sektör adı</param>
        /// <param name="altSektorVarMi">Sektörün alt sektörü var mı, yok mu bilgisi</param>
        public SektorSozluk(string kod, string ad, string altSektorVarMi)
        {
            this.kod = kod;
            this.ad = ad;
            this.altSektorVarMi = altSektorVarMi;
        }
    }

    /// <summary>
    /// İstenilen bir nesne dizisini ve yapılan işlemin sonucunu tutan sınıf
    /// </summary>
    [Serializable()]
    public class BilgiVAN
    {
        /// <summary>
        /// Tutulmak istenen nesne dizisi
        /// </summary>
        public TNSCollection liste;

        /// <summary>
        /// İşlem sonucu
        /// </summary>
        public Sonuc sonuc;

        /// <summary>
        /// Parametresiz BilgiVAN nesnesi oluşturucu yordam
        /// </summary>
        public BilgiVAN()
        {
            liste = new TNSCollection();
            sonuc = new Sonuc();
        }
    }

    /// <summary>
    /// Yatırım projesi bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Proje
    {
        public string yurutecekKurumAd { get { return birim.ad; } set { birim.AdAta(value); } }

        /// <summary>
        /// Projeyi yürütecek kurum
        /// </summary>
        public string yurutecekKurum { get { return birim.kod; } set { birim.kod = value; } }

        /// <summary>
        /// Alt projenin birim bilgisi
        /// </summary>
        public ABSKod birim;

        /// <summary>
        /// Projenin programatik kodu
        /// </summary>
        public int kod { get; set; }

        public string kodAd { get { return DPTProjeNo + "- " + ad; } }

        /// <summary>
        /// Projenin DPT proje numarası
        /// </summary>
        public string DPTProjeNo { get; set; }

        /// <summary>
        /// Projenin sektör bilgisi
        /// </summary>
        public SektorSozluk sektor;

        /// <summary>
        /// Projenin tür bilgisi
        /// </summary>
        public Sozluk tur;

        /// <summary>
        /// Projenin adı
        /// </summary>
        public string ad { get; set; }

        /// <summary>
        /// Projenin karakteristiği
        /// </summary>
        public string karakteristik;

        /// <summary>
        /// Projenin karakteristiğinin özet bilgisi
        /// </summary>
        public string karakteristikOzet;

        /// <summary>
        /// Projenin etüd proje olup olmadığını tutan özellik
        /// </summary>
        public bool etudProjeMi;

        /// <summary>
        /// Projenin başlangıç yılı
        /// </summary>
        public int baslangicYili;

        /// <summary>
        /// Projenin bitiş yılı
        /// </summary>
        public int bitisYili;

        /// <summary>
        /// Projenin modeli
        /// </summary>
        public int model;

        /// <summary>
        /// Projenin neden yapıldığının gerekçesi
        /// </summary>
        public string gerekce;

        /// <summary>
        /// Varsa projeye ait yatırım alt projeleri bilgisi
        /// </summary>
        public TNSCollection altProjeler;

        /// <summary>
        /// Varsa projeye ait il ve ilçe bilgileri
        /// </summary>
        public TNSCollection ilIlceler;

        /// <summary>
        /// Varsa projeye ait ödenek bilgileri
        /// </summary>
        public TNSCollection odenekBilgiler;
        public Hashtable bilgiler = null;
        public TNSCollection kalkinmaBolgeler = null;
        public int siralama = 1;
        public int aktiflik = 2;

        /// <summary>
        /// Parametresiz Proje nesnesi oluşturucu yordam
        /// </summary>
        public Proje()
        {
            birim = new ABSKod();
            kod = 0;
            DPTProjeNo = string.Empty;
            sektor = new SektorSozluk();
            tur = new Sozluk();
            ad = string.Empty;
            karakteristik = string.Empty;
            karakteristikOzet = string.Empty;
            etudProjeMi = true;
            baslangicYili = 0;
            bitisYili = 0;
            model = 0;
            yurutecekKurum = string.Empty;
            yurutecekKurumAd = string.Empty;
            gerekce = string.Empty;
            altProjeler = new TNSCollection();
            ilIlceler = new TNSCollection();
            odenekBilgiler = new TNSCollection();
        }

        /// <summary>
        /// Proje parametresi alan Proje nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="p">Proje oluşturulurken kullanılan proje nesnesi</param>
        public Proje(Proje p)
        {
            kod = p.kod;
            DPTProjeNo = p.DPTProjeNo;
            sektor = new SektorSozluk(p.sektor.kod, p.sektor.ad, p.sektor.altSektorVarMi);
            tur = new Sozluk(p.tur.kod, p.tur.ad);
            ad = p.ad;
            karakteristik = p.karakteristik;
            karakteristikOzet = p.karakteristikOzet;
            etudProjeMi = p.etudProjeMi;
            baslangicYili = p.baslangicYili;
            bitisYili = p.bitisYili;
            model = 0;
            yurutecekKurum = string.Empty;
            yurutecekKurumAd = string.Empty;
            gerekce = string.Empty;
            altProjeler = new TNSCollection();
            if (p.altProjeler.Count > 0)
                altProjeler.AddRange(p.altProjeler);
            ilIlceler = new TNSCollection();
            if (p.ilIlceler.Count > 0)
                ilIlceler.AddRange(p.ilIlceler);
            odenekBilgiler = new TNSCollection();
            if (p.odenekBilgiler.Count > 0)
                odenekBilgiler.AddRange(p.odenekBilgiler);
        }

        public int BuYilBiterMi(int yil)
        {
            if (bitisYili <= yil) return 0;
            return 1;
        }

        public string EtudDevamEdenYeni(int yil)
        {
            if (this.etudProjeMi) return "0";
            if (yil == this.baslangicYili || this.eskiYeni) return "2";
            return "1";
        }

        public bool eskiYeni { get; set; }

        public bool aktifMi { get; set; }

        public int iliskiliAltProje { get; set; }

        public string ProjeYeri()
        {
            return TNS.YAT.Arac.IlIlceAdOl(ilIlceler);
        }

        public string KarakteristikOzet { get { if (string.IsNullOrWhiteSpace(karakteristikOzet)) return karakteristik; else return karakteristikOzet; } }

        public bool cari { get; set; }

        public void ProjeBilgiBul(out double prjTut, out double revPrjTut, out double kBO, out double revizeOdenek, out double harcama, string ekoKod, int butceYil)
        {
            prjTut = 0; revPrjTut = 0; kBO = 0; revizeOdenek = 0; harcama = 0;
            foreach (AltProje ap in this.altProjeler)
            {
                double ic_prjTut; double ic_revPrjTut; double ic_kBO; double ic_revizeOdenek; double ic_harcama;
                ap.AltProjeBilgiBul(out ic_prjTut, out ic_revPrjTut, out ic_kBO, out ic_revizeOdenek, out ic_harcama, ekoKod, butceYil);
                prjTut += ic_prjTut; revPrjTut += ic_revPrjTut; kBO += ic_kBO; revizeOdenek += ic_revizeOdenek; harcama += ic_harcama;
            }
        }
    }

    /// <summary>
    /// Yatırım alt projesi bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class AltProje
    {
        /// <summary>
        /// Parametresiz Alt Proje nesnesi oluşturucu yordam
        /// </summary>
        public AltProje()
        {
            kod = 0;
            birim = new ABSKod();
            proje = new Sozluk();
            tur = new Sozluk();
            ad = "";
            karakteristik = "";
            karakteristikOzet = "";
            sorumluAd = "";
            sorumluGorev = "";
            sorumluTelefon = "";
            sorumluEPosta = "";
            onerenAd = "";
            onerenAdres = "";
            onerenTelefon = "";
            onerenFaks = "";
            gerekce = "";
            amac = "";
            hedef = "";
            maliyet = 0;
            finansmanKaynak = "";
            baslangicYil = 0;
            bitisYil = 0;
            model = 0;
            ihaleTarihi = new TNSDateTime();
            ihaleBedeli = 0;
            ihaleYasasi = "";
            butceKaynaklari = new TNSCollection();
            krediTalepleri = new TNSCollection();
            dokumanlar = new TNSCollection();
            ilIlceler = new TNSCollection();
            odenekBilgiler = new TNSCollection();
        }

        /// <summary>
        /// Alt projenin programatik kodu
        /// </summary>
        public int kod { get; set; }

        public string birim_kod { get { return birim.kod; } set { birim.kod = value; } }

        /// <summary>
        /// Alt projenin birim bilgisi
        /// </summary>
        public ABSKod birim;


        /// <summary>
        /// Alt projenin bağlı olduğu proje bilgisi
        /// </summary>
        public Sozluk proje;
        public string projeAd { get { return proje.ad; } }
        public string projeAciklama { get { return proje.aciklama; } }

        /// <summary>
        /// Alt projenin tür bilgisi
        /// </summary>
        public Sozluk tur;

        /// <summary>
        /// Alt projenin adı
        /// </summary>
        public string ad { get; set; }

        /// <summary>
        /// Alt projenin karakteristiği
        /// </summary>
        public string karakteristik;

        /// <summary>
        /// Alt projenin karakteristiğinin özet bilgisi
        /// </summary>
        public string karakteristikOzet;

        /// <summary>
        /// Alt projeden sorumlu kişinin adı
        /// </summary>
        public string sorumluAd;

        /// <summary>
        /// Alt projeden sorumlu kişinin görevi
        /// </summary>
        public string sorumluGorev;

        /// <summary>
        /// Alt projeden sorumlu kişinin telefonu
        /// </summary>
        public string sorumluTelefon;

        /// <summary>
        /// Alt projeden sorumlu kişinin e-postası
        /// </summary>
        public string sorumluEPosta;

        /// <summary>
        /// Alt projeyi öneren kişinin adı
        /// </summary>
        public string onerenAd;

        /// <summary>
        /// Alt projeyi öneren kişinin adresi
        /// </summary>
        public string onerenAdres;

        /// <summary>
        /// Alt projeyi öneren kişinin telefonu
        /// </summary>
        public string onerenTelefon;

        /// <summary>
        /// Alt projeyi öneren kişinin faksı
        /// </summary>
        public string onerenFaks;

        /// <summary>
        /// Alt projenin neden yapıldığının gerekçesi
        /// </summary>
        public string gerekce { get; set; }

        /// <summary>
        /// Alt projenin yapılış amacı
        /// </summary>
        public string amac;

        /// <summary>
        /// Alt projenin yapılış hedefi
        /// </summary>
        public string hedef;

        /// <summary>
        /// Alt projenin toplam maliyeti
        /// </summary>
        public double maliyet;

        /// <summary>
        /// Alt projeyi finanse eden kaynak
        /// </summary>
        public string finansmanKaynak;

        /// <summary>
        /// Alt projenin başlangıç yılı
        /// </summary>
        public int baslangicYil { get; set; }

        /// <summary>
        /// Alt projenin bitiş yılı
        /// </summary>
        public int bitisYil { get; set; }

        /// <summary>
        /// Alt projenin modeli
        /// </summary>
        public int model;

        /// <summary>
        /// Alt projenin ihaleye çıkış tarihi
        /// </summary>
        public TNSDateTime ihaleTarihi;

        /// <summary>
        /// Alt projenin ihale bedeli
        /// </summary>
        public double ihaleBedeli;

        /// <summary>
        /// Alt proje ihalesinin dayanak yasası
        /// </summary>
        public string ihaleYasasi;

        /// <summary>
        /// Varsa alt projeye ait bütçe kaynakları bilgisi
        /// </summary>
        public TNSCollection butceKaynaklari;

        /// <summary>
        /// Varsa alt projeye ait kredi talepleri bilgisi
        /// </summary>
        public TNSCollection krediTalepleri;

        /// <summary>
        /// Varsa alt projeye ait doküman bilgileri
        /// </summary>
        public TNSCollection dokumanlar;

        /// <summary>
        /// Varsa alt projeye ait il ve ilçe bilgileri
        /// </summary>
        public TNSCollection ilIlceler;

        /// <summary>
        /// Varsa alt projeye ait ödenek bilgileri
        /// </summary>
        public TNSCollection odenekBilgiler;

        public bool DPTDeVar { get; set; }

        public int aktifYil { get; set; }

        public double projeTutariHibeli
        {
            get
            {
                return projeTutariBul(false);
            }
        }

        public double projeTutariHibesiz
        {
            get
            {
                return projeTutariBul(true);
            }
        }

        public double projeTutariBul(bool hibesiz = true)
        {
            double donusDeger = 0;
            if (butceKaynaklari != null)
            {
                foreach (ButceKaynak bk in butceKaynaklari)
                {
                    if (hibesiz && bk.kaynakTur == ENUMButceKaynakTur.HIBE) continue;
                    foreach (ButceKaynakTeklif bkt in bk.teklifler)
                    {
                        if (bkt.butceYil == aktifYil)
                        {
                            donusDeger += bkt.ProjeTutarToplam;
                        }
                    }
                }
            }
            return donusDeger;
        }

        public double yiliTeklifi
        {
            get
            {
                return YiliTeklifiBul(false);
            }
        }

        public double yiliTeklifiHibesiz
        {
            get
            {
                return YiliTeklifiBul(true);
            }
        }

        public double YiliTeklifiBul(bool hibesiz)
        {
            double donusDeger = 0;
            if (butceKaynaklari != null)
            {
                foreach (ButceKaynak bk in butceKaynaklari)
                {
                    if (hibesiz && bk.kaynakTur == ENUMButceKaynakTur.HIBE) continue;

                    foreach (ButceKaynakTeklif bkt in bk.teklifler)
                    {
                        if (bkt.butceYil == aktifYil)
                        {
                            donusDeger += bkt.Odenek0Toplam;
                        }
                    }
                }
            }
            return donusDeger;
        }

        public double yiliTeklifiEk
        {
            get
            {
                double donusDeger = 0;
                if (butceKaynaklari != null)
                {
                    foreach (ButceKaynak bk in butceKaynaklari)
                    {
                        foreach (ButceKaynakTeklif bkt in bk.teklifler)
                        {
                            if (bkt.butceYil == aktifYil)
                            {
                                donusDeger += bkt.OdenekEk0Toplam;
                            }
                        }
                    }
                }
                return donusDeger;
            }
        }

        public string ProjeYeri()
        {
            return TNS.YAT.Arac.IlIlceAdOl(ilIlceler);
        }

        public string EtudDevamEdenYeni(Proje p, int yil)
        {
            if (p.etudProjeMi) return "0";
            if (yil == this.baslangicYil || p.eskiYeni) return "2";
            return "1";
        }

        internal void AltProjeBilgiBul(out double prjTut, out double revPrjTut, out double kBO, out double revizeOdenek, out double harcama, string ekoKod, int butceYil)
        {
            prjTut = 0; revPrjTut = 0; kBO = 0; revizeOdenek = 0; harcama = 0;
            foreach (ButceKaynak bk in this.butceKaynaklari)
            {
                double ic_prjTut; double ic_revPrjTut; double ic_kBO; double ic_revizeOdenek; double ic_harcama;
                bk.KaynakBilgiBul(out ic_prjTut, out ic_revPrjTut, out ic_kBO, out ic_revizeOdenek, out ic_harcama, ekoKod, butceYil);
                prjTut += ic_prjTut; revPrjTut += ic_revPrjTut; kBO += ic_kBO; revizeOdenek += ic_revizeOdenek; harcama += ic_harcama;
            }
        }

        public string KarakteristikOzet { get { if (string.IsNullOrWhiteSpace(karakteristikOzet)) return karakteristik; else return karakteristikOzet; } }

        public string bolgeListeStr
        {
            get
            {
                string donusDeger = "";
                if (bolgeler != null)
                {
                    foreach (KalkinmaBolge item in bolgeler)
                    {
                        if (donusDeger != "") donusDeger += ", ";
                        donusDeger += item.bolgeKod;
                    }
                }
                return donusDeger;
            }
            set
            {
                if (bolgeler != null) bolgeler.Clear();
                else bolgeler = new TNSCollection();
                if (string.IsNullOrWhiteSpace(value)) return;

                string[] secililer = value.Split(',');
                foreach (string item in secililer)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        KalkinmaBolge kb = new KalkinmaBolge();
                        kb.bolgeKod = item;
                        bolgeler.Add(kb);
                    }
                }
            }
        }

        public TNSCollection bolgeler = null;
        public Hashtable bilgiler = null;
    }

    /// <summary>
    /// Yatırım projesi il ve ilçe bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ProjeIlIlce
    {
        public int ilKod { get { return ilIlce.il.kod; } set { ilIlce.il.kod = value; } }
        public int ilceKod { get { return ilIlce.ilceKodu; } set { ilIlce.ilceKodu = value; } }
        public string ilAd { get { return ilIlce.il.ad; } set { ilIlce.il.ad = value; } }
        public string ilceAd { get { return ilIlce.ilceAdi; } set { ilIlce.ilceAdi = value; } }

        /// <summary>
        /// İl ve ilçe bilgisinin ait olduğu yatırım projesinin programatik kodu
        /// </summary>
        public int projeNo;

        /// <summary>
        /// İl ve ilçe bilgisinin ait olduğu yatırım alt projesinin programatik kodu
        /// </summary>
        public int altProjeNo;

        /// <summary>
        /// Yatırım projesine veya alt projesine ait il ve ilçe bilgisi
        /// </summary>
        public IlceSozluk ilIlce;

        /// <summary>
        /// Parametresiz ProjeIlIlce nesnesi oluşturucu yordam
        /// </summary>
        public ProjeIlIlce()
        {
            projeNo = 0;
            altProjeNo = 0;
            ilIlce = new IlceSozluk();
        }
    }

    /// <summary>
    /// Yatırım alt projelerine ait bütçe kaynak bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ButceKaynak
    {
        /// <summary>
        /// Alt projenin programatik kodu
        /// </summary>
        public int altProjeKodu;

        /// <summary>
        /// Alt proje adı
        /// </summary>
        public string altProjeAdi { get; set; }

        /// <summary>
        /// Bütçe kaynağın programatik kodu
        /// </summary>
        public int kaynakNo { get; set; }

        /// <summary>
        /// Bütçe kaynak adı
        /// </summary>
        public string kaynakAd { get; set; }

        /// <summary>
        /// Bütçe kaynağı (krediyi veya hibeyi) veren kurum/kuruluş
        /// </summary>
        public string krediyiVeren;

        /// <summary>
        /// Bütçe kaynağın (kredi veya hibe) başlama tarihi
        /// </summary>
        public TNSDateTime baslamaTarih;

        /// <summary>
        /// Bütçe kaynağın (kredi veya hibe) yürürlük tarihi
        /// </summary>
        public TNSDateTime yururlukTarih;

        /// <summary>
        /// Bütçe kaynağın (kredi veya hibe) türü
        /// Bu tür raporlarda açıklama alanı olarak kullanılan bir özelliktir, bütçe kaynak türü ile ilgisi yoktur.
        /// </summary>
        public string tur;

        /// <summary>
        /// Bütçe kaynağın (kredi veya hibe) tutarı
        /// </summary>
        public string tutar;

        /// <summary>
        /// Bütçe kaynağın (kredi) vadesi
        /// </summary>
        public string vade;

        /// <summary>
        /// Bütçe kaynağın (kredi) ödemesiz dönemi
        /// </summary>
        public string odemesizDonem;

        /// <summary>
        /// Bütçe kaynağın (kredi) faiz oranı
        /// </summary>
        public string faizOran;

        /// <summary>
        /// Bütçe kaynağın (kredi) içerde kalan kısmının krediye oranı
        /// </summary>
        public string IcParaKrediOran;

        /// <summary>
        /// Bütçe kaynağın türü
        /// Bu özelliğin değeri ENUMButceKaynakTur listesinde yer alan elemanlardan biri olmalı.
        /// Programın akışı bütçe kaynak türüne göre değişir.
        /// Bu nedenle bu özelliğin aldığı değer program için önemlidir.
        /// </summary>
        public ENUMButceKaynakTur kaynakTur;

        public enum ENUMKaynakToplam : int
        {
            ProjeyeGeneleDahil = 1,
            ProjeyeDahilGeneleHaric = 2,
            ProjeyeHaricGeneleDahil = 3,
            ProjeyeGeneleHaric = 4
        }

        /// <summary>
        /// Bu özellik, bütçe kaynağa girilen ödenek, harcama gibi rakamların raporlarda toplama
        /// dahil mi olacak yoksa parantez içinde mi gösterilecek kararının verilmesine
        /// yardımcı olur.
        /// </summary>
        public ENUMKaynakToplam toplamaDahil;

        /// <summary>
        /// Varsa bütçe kaynağın ödenek bilgilerinin yıllara bağlı olarak tutulduğu özellik
        /// </summary>
        public TNSCollection teklifler;
        public ButceKaynakTeklif ilkTeklif = null;
        ///// <summary>
        ///// Varsa bütçe kaynağın harcama bilgilerinin yıllara bağlı olarak tutulduğu özellik
        ///// </summary>
        //public TNSCollection harcamalar;

        public TNSCollection krediPlan;

        /// <summary>
        /// Varsa bütçe kaynağın (kredi) geri ödeme bilgilerinin yıllara bağlı olarak tutulduğu özellik
        /// </summary>
        public TNSCollection geriOdeme;

        public TNSCollection pebKodlar;

        public string dptProjeNo { get; set; }
        public string dptProjeAd { get; set; }

        /// <summary>
        /// Parametresiz Bütçe Kaynak nesnesi oluşturucu yordam
        /// </summary>
        public ButceKaynak()
        {
            altProjeKodu = 0;
            altProjeAdi = string.Empty;
            kaynakNo = 0;
            kaynakAd = string.Empty;
            krediyiVeren = string.Empty;
            baslamaTarih = new TNSDateTime();
            yururlukTarih = new TNSDateTime();
            tur = string.Empty;
            tutar = string.Empty;
            vade = string.Empty;
            odemesizDonem = string.Empty;
            faizOran = string.Empty;
            IcParaKrediOran = string.Empty;
            kaynakTur = 0;
            toplamaDahil = ENUMKaynakToplam.ProjeyeGeneleDahil;
            teklifler = new TNSCollection();
            //harcamalar = new TNSCollection();
            krediPlan = new TNSCollection();
            geriOdeme = new TNSCollection();
            pebKodlar = new TNSCollection();
        }

        private ButceKaynakTeklif IlkTeklifBul(bool okuma)
        {
            if (ilkTeklif == null)
            {
                if (teklifler.Count > 0)
                {
                    ilkTeklif = (ButceKaynakTeklif)teklifler[0];
                }
                else if (!okuma)
                {
                    ilkTeklif = new ButceKaynakTeklif();
                    teklifler.Add(ilkTeklif);
                }
            }
            return ilkTeklif;
        }


        private double TeklifDegerBul(ENUMProjeMaliYapiKod kod, int degerTur)
        {
            ButceKaynakTeklif ilkTeklific = IlkTeklifBul(true);
            if (ilkTeklif != null)
                return ilkTeklific.TeklifDegerBul(kod, degerTur);
            return 0;
        }

        private void TeklifDegerAta(ENUMProjeMaliYapiKod kod, int degerTur, double value)
        {
            ButceKaynakTeklif ilkTeklific = IlkTeklifBul(false);
            ilkTeklific.TeklifDegerAta(kod, degerTur, value);
        }

        public double projeToplam_Dis { get { return TeklifDegerBul(ENUMProjeMaliYapiKod.kaynakToplam, 0); } set { TeklifDegerAta(ENUMProjeMaliYapiKod.kaynakToplam, 0, value); } }
        public double projeToplam_Toplam { get { return TeklifDegerBul(ENUMProjeMaliYapiKod.kaynakToplam, 2); } set { TeklifDegerAta(ENUMProjeMaliYapiKod.kaynakToplam, 2, value); } }

        public double odenek_Dis { get { return TeklifDegerBul(ENUMProjeMaliYapiKod.teklif0, 0); } set { TeklifDegerAta(ENUMProjeMaliYapiKod.teklif0, 0, value); } }
        public double odenek_Toplam { get { return TeklifDegerBul(ENUMProjeMaliYapiKod.teklif0, 2); } set { TeklifDegerAta(ENUMProjeMaliYapiKod.teklif0, 2, value); } }

        public double revizeOdenek_Dis { get { return TeklifDegerBul(ENUMProjeMaliYapiKod.odenekEklenenDusulen, 0); } set { TeklifDegerAta(ENUMProjeMaliYapiKod.odenekEklenenDusulen, 0, value); } }
        public double revizeOdenek_Toplam { get { return TeklifDegerBul(ENUMProjeMaliYapiKod.odenekEklenenDusulen, 2); } set { TeklifDegerAta(ENUMProjeMaliYapiKod.odenekEklenenDusulen, 2, value); } }

        public double harcama_Dis { get { return TeklifDegerBul(ENUMProjeMaliYapiKod.harcamaYilSonu, 0); } set { TeklifDegerAta(ENUMProjeMaliYapiKod.harcamaYilSonu, 0, value); } }
        public double harcama_Toplam { get { return TeklifDegerBul(ENUMProjeMaliYapiKod.harcamaYilSonu, 2); } set { TeklifDegerAta(ENUMProjeMaliYapiKod.harcamaYilSonu, 2, value); } }

        public double kumulatifHarcama_Dis { get { return TeklifDegerBul(ENUMProjeMaliYapiKod.kumulatifHarcama, 0); } set { TeklifDegerAta(ENUMProjeMaliYapiKod.kumulatifHarcama, 0, value); } }
        public double kumulatifHarcama_Toplam { get { return TeklifDegerBul(ENUMProjeMaliYapiKod.kumulatifHarcama, 2); } set { TeklifDegerAta(ENUMProjeMaliYapiKod.kumulatifHarcama, 2, value); } }

        public ProjeMaliYapi ProjeMaliYapiBul(ENUMProjeMaliYapiKod teklifYil)
        {
            ButceKaynakTeklif ilkTeklific = IlkTeklifBul(true);
            if (ilkTeklific != null)
                return ilkTeklific.ProjeMaliYapiBul(teklifYil);
            else
                return null;
        }

        public static ENUMButceKaynakTur TurBul(string turAd)
        {
            for (int i = 1; i <= 6; i++)
                if (TurAdGetir((ENUMButceKaynakTur)i) == turAd)
                    return (ENUMButceKaynakTur)i;
            return ENUMButceKaynakTur.BUTCE;
        }

        /// <summary>
        /// Bütçe kaynak türünün adını döndüren yordam
        /// adEkle parametresi true ise dönen sonuç "ad (tür ad)" şeklinde olur.
        /// adEkle parametresi false ise dönen sonuç "tür ad" şeklinde olur.
        /// </summary>
        /// <param name="adEkle">Bütçe kaynak adının dönen değere eklenip eklenmeyeceği bilgisi</param>
        /// <returns>Bütçe kaynak türünün adı</returns>
        public string TurAdGetir(bool adEkle)
        {
            if (adEkle)
                return TurAdGetir(kaynakTur, kaynakAd);
            else
                return TurAdGetir(kaynakTur);
        }

        /// <summary>
        /// Parametre olarak verilen bütçe kaynak türünün adını döndüren yordam
        /// </summary>
        /// <param name="tur">ENUMButceKaynakTur listesinde yer alan elemanlardan birinin değeri</param>
        /// <returns>Bütçe kaynak türünün adı</returns>
        public static string TurAdGetir(ENUMButceKaynakTur tur)
        {
            return TurAdGetir(tur, string.Empty);
        }

        /// <summary>
        /// Parametre olarak verilen bütçe kaynak türünün adını döndüren yordam
        /// ad parametresi boşluktan farklı ise dönen sonuç "ad (tür ad)" şeklinde olur.
        /// ad parametresi boşluk ise dönen sonuç "tür ad" şeklinde olur.
        /// </summary>
        /// <param name="tur">ENUMButceKaynakTur listesinde yer alan elemanlardan birinin değeri</param>
        /// <param name="ad">Bütçe kaynak adı</param>
        /// <returns>Bütçe kaynak türünün adı</returns>
        public static string TurAdGetir(ENUMButceKaynakTur tur, string ad)
        {
            string donusDeger = string.Empty;

            if (tur == ENUMButceKaynakTur.BUTCE)
                donusDeger = YatirimResI.YAT005;
            else if (tur == ENUMButceKaynakTur.KREDIBUTCEICI)
                donusDeger = YatirimResI.YAT006;
            else if (tur == ENUMButceKaynakTur.KREDIBUTCEDISI)
                donusDeger = YatirimResI.YAT007;
            else if (tur == ENUMButceKaynakTur.HIBE)
                donusDeger = YatirimResI.YAT008;
            else if (tur == ENUMButceKaynakTur.DONERSERMAYE)
                donusDeger = YatirimResI.YAT066;
            else if (tur == ENUMButceKaynakTur.YENIPROJE)
                donusDeger = "Yeni proje";

            if (!string.IsNullOrEmpty(ad))
                donusDeger = ad + " (" + donusDeger + ")";

            return donusDeger;
        }

        internal void KaynakBilgiBul(out double prjTut, out double revPrjTut, out double kBO, out double revizeOdenek, out double harcama, string ekoKod, int butceYil)
        {
            prjTut = this.projeToplam_Toplam;
            revPrjTut = this.projeToplam_Toplam;

            kBO = DizidenTopla(odenekler, ekoKod);
            revizeOdenek = kBO + DizidenTopla(odenekDegisimler, ekoKod);
            harcama = DizidenTopla(harcamalar, ekoKod);
        }

        private double DizidenTopla(TNSCollection dizi, string ekoKod)
        {
            double sonuc = 0;
            foreach (KaynakOdenekBilgi bkt in dizi)
            {
                if (string.IsNullOrWhiteSpace(ekoKod) || bkt.ekoKod.StartsWith(ekoKod))
                    sonuc += bkt.odenek;
            }
            return sonuc;
        }

        public bool DPTRapordaGor { get; set; }
        public YeniProjeButce[] yeniTeklifler = null;
        public TNSCollection odenekDegisimler = null;
        public TNSCollection harcamalar = null;
        public TNSCollection odenekler = null;
        public int yikobMu = 0; //1 ise YIKOB

        public string birim { get; set; }

        public int pbKod { get; set; }

        public string dipnot { get; set; }

        public string pbKisaltma { get; set; }

        public string pbAdi { get; set; }

        public int projeNo { get; set; }

        public string sektorNo { get; set; }

        public string sektorAd { get; set; }
    }

    [Serializable()]
    public class ProjeMaliYapisi
    {
        public int projeNo;
        public int altProjeNo;
        public ProjeMaliYapi butce = null;
        public ProjeMaliYapi hibe = null;
        public ProjeMaliYapi dss = null;
        public ProjeMaliYapi butceYP = null;
        public ProjeMaliYapi hibeYP = null;
        public ProjeMaliYapi dssYP = null;

        public ProjeMaliYapisi()
        {
        }

        public ProjeMaliYapisi(ProjeMaliYapisi projeMaliYapisi, decimal katSayi)
        {
            if (projeMaliYapisi != null)
            {
                this.projeNo = projeMaliYapisi.projeNo;
                this.altProjeNo = projeMaliYapisi.altProjeNo;
                this.butce = new ProjeMaliYapi(projeMaliYapisi.butce, katSayi);
                this.hibe = new ProjeMaliYapi(projeMaliYapisi.hibe, katSayi);
                this.dss = new ProjeMaliYapi(projeMaliYapisi.dss, katSayi);
                this.butceYP = new ProjeMaliYapi(projeMaliYapisi.butceYP, katSayi);
                this.hibeYP = new ProjeMaliYapi(projeMaliYapisi.hibeYP, katSayi);
                this.dssYP = new ProjeMaliYapi(projeMaliYapisi.dssYP, katSayi);
            }
        }

        public void Ekle(ProjeMaliYapisi projeMaliYapisi, decimal katSayi)
        {
            this.butce.Ekle(projeMaliYapisi.butce, katSayi);
        }

        public double DegerAl(int okunacakAlan)
        {
            if (okunacakAlan == 1) return butce == null ? 0 : butce.FinKod7;
            if (okunacakAlan == 2) return butce == null ? 0 : butce.DisOzkaynak;
            if (okunacakAlan == 3) return (butce == null ? 0 : butce.Toplam) + (dss == null ? 0 : dss.Toplam);
            if (okunacakAlan == 4) return hibe == null ? 0 : hibe.Toplam;
            if (okunacakAlan == 5) return butceYP == null ? 0 : butceYP.FinKod7;
            if (okunacakAlan == 6) return butceYP == null ? 0 : butceYP.DisOzkaynak;
            if (okunacakAlan == 7) return (butceYP == null ? 0 : butceYP.Toplam) + (dssYP == null ? 0 : dssYP.Toplam); ;
            if (okunacakAlan == 8) return hibeYP == null ? 0 : hibeYP.Toplam;
            return 0;
        }

        public void DegerAta(ENUMButceKaynakTur butceKaynakTur, double fin7, double kaynakToplam, double disOzkaynak)
        {
            ProjeMaliYapi atanacak = null;
            if (butceKaynakTur == ENUMButceKaynakTur.BUTCE || butceKaynakTur == ENUMButceKaynakTur.KREDIBUTCEICI || butceKaynakTur == ENUMButceKaynakTur.YENIPROJE)
            {
                if (butce == null) butce = new ProjeMaliYapi();
                atanacak = butce;
            }
            else if (butceKaynakTur == ENUMButceKaynakTur.HIBE)
            {
                if (hibe == null) hibe = new ProjeMaliYapi();
                atanacak = hibe;
            }
            else if (butceKaynakTur == ENUMButceKaynakTur.DONERSERMAYE)
            {
                if (dss == null) dss = new ProjeMaliYapi();
                atanacak = dss;
            }
            if (atanacak != null)
            {
                atanacak.DegerAta(fin7, kaynakToplam, disOzkaynak);
            }
        }
    }

    //public enum ENUMProjeMaliYapiTur
    //{
    //    HARC6,
    //    HARC12,
    //    BUTCE0,
    //    BUTCE1,
    //    BUTCE2,
    //    EK0,
    //    EK1,
    //    EK2,
    //    EKLENEN,
    //    DUSULEN,
    //    HARCTAH,
    //    PROJETUTARI
    //}

    /// <summary>
    /// Bütçe kaynağın bütçede yer alan ve almayan bütün ödenek teklifleri bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ProjeMaliYapi
    {
        public ProjeMaliYapi()
        {
        }

        public ProjeMaliYapi(ProjeMaliYapi projeMaliYapi, decimal katSayi)
        {
            if (projeMaliYapi != null)
            {
                this.FinKod7 = Convert.ToDouble(Convert.ToDecimal(projeMaliYapi.FinKod7) * katSayi);
                this.DisOzkaynak = Convert.ToDouble(Convert.ToDecimal(projeMaliYapi.DisOzkaynak) * katSayi);
                this.Toplam = Convert.ToDouble(Convert.ToDecimal(projeMaliYapi.Toplam) * katSayi);
            }
        }

        //public double FinKod1 { get; set; }
        public double FinKod7 { get; set; }
        public double DisOzkaynak { get; set; }
        public double Toplam { get; set; }

        public void DegerAta(double fin7, double kaynakToplam, double disOzkaynak)
        {
            FinKod7 += fin7;
            Toplam += kaynakToplam;
            DisOzkaynak += disOzkaynak;
        }

        internal void Ekle(ProjeMaliYapi projeMaliYapi, decimal katSayi)
        {
            if (projeMaliYapi != null)
            {
                this.FinKod7 += Convert.ToDouble(Convert.ToDecimal(projeMaliYapi.FinKod7) * katSayi);
                this.DisOzkaynak += Convert.ToDouble(Convert.ToDecimal(projeMaliYapi.DisOzkaynak) * katSayi);
                this.Toplam += Convert.ToDouble(Convert.ToDecimal(projeMaliYapi.Toplam) * katSayi);
            }
        }

        internal void DegerAta(int degerTur, double value)
        {
            if (degerTur == 0) FinKod7 = value;
            else if (degerTur == 1) DisOzkaynak = value;
            else Toplam = value;
        }

        internal double DegerBul(int degerTur)
        {
            if (degerTur == 0) return FinKod7;
            else if (degerTur == 1) return DisOzkaynak;
            else return Toplam;
        }
    }

    [Serializable()]
    public class YeniProjeButce
    {
        public string ekoKod { get; set; }
        public string ekoAd { get; set; }
        public double projeTutariDis { get; set; }
        public double projeTutariIc { get; set; }
        public double disKredi0 { get; set; }
        public double icKaynak0 { get; set; }
        public double disKredi1 { get; set; }
        public double icKaynak1 { get; set; }
        public double disKredi2 { get; set; }
        public double icKaynak2 { get; set; }
    }

    [Serializable()]
    public class KaynakOdenekBilgi
    {
        public int ay;
        public string finKod;
        public double odenek;
        public string ekoKod;
    }
    /// <summary>
    /// Bütçe kaynağın bütçede yer alan ve almayan bütün ödenek teklifleri bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ButceKaynakTeklif : IComparable
    {
        public int ButceYil { get { return butceYil; } set { butceYil = value; } }
        public double ProjeTutarDis { get { return kaynakToplam.FinKod7; } set { kaynakToplam.FinKod7 = value; } }
        public double ProjeTutarToplam { get { return kaynakToplam.Toplam; } set { kaynakToplam.Toplam = value; } }

        public double Odenek0Dis { get { return OVTeklif[0].FinKod7; } set { OVTeklif[0].FinKod7 = value; } }
        public double Odenek0DisaGiden { get { return OVTeklif[0].DisOzkaynak; } set { OVTeklif[0].DisOzkaynak = value; } }
        public double Odenek0Toplam { get { return OVTeklif[0].Toplam; } set { OVTeklif[0].Toplam = value; } }

        public double OdenekEk0Dis { get { return OVTeklifEk[0].FinKod7; } set { OVTeklifEk[0].FinKod7 = value; } }
        public double OdenekEk0DisaGiden { get { return OVTeklifEk[0].DisOzkaynak; } set { OVTeklifEk[0].DisOzkaynak = value; } }
        public double OdenekEk0Toplam { get { return OVTeklifEk[0].Toplam; } set { OVTeklifEk[0].Toplam = value; } }


        public double OdenekEk1Dis { get { return OVTeklifEk[1].FinKod7; } set { OVTeklifEk[1].FinKod7 = value; } }
        public double OdenekEk1DisaGiden { get { return OVTeklifEk[1].DisOzkaynak; } set { OVTeklifEk[1].DisOzkaynak = value; } }
        public double OdenekEk1Toplam { get { return OVTeklifEk[1].Toplam; } set { OVTeklifEk[1].Toplam = value; } }

        public double OdenekEk2Dis { get { return OVTeklifEk[2].FinKod7; } set { OVTeklifEk[2].FinKod7 = value; } }
        public double OdenekEk2DisaGiden { get { return OVTeklifEk[2].DisOzkaynak; } set { OVTeklifEk[2].DisOzkaynak = value; } }
        public double OdenekEk2Toplam { get { return OVTeklifEk[2].Toplam; } set { OVTeklifEk[2].Toplam = value; } }

        public double Odenek1Dis { get { return OVTeklif[1].FinKod7; } set { OVTeklif[1].FinKod7 = value; } }
        public double Odenek1DisaGiden { get { return OVTeklif[1].DisOzkaynak; } set { OVTeklif[1].DisOzkaynak = value; } }
        public double Odenek1Toplam { get { return OVTeklif[1].Toplam; } set { OVTeklif[1].Toplam = value; } }

        public double Odenek2Dis { get { return OVTeklif[2].FinKod7; } set { OVTeklif[2].FinKod7 = value; } }
        public double Odenek2DisaGiden { get { return OVTeklif[2].DisOzkaynak; } set { OVTeklif[2].DisOzkaynak = value; } }
        public double Odenek2Toplam { get { return OVTeklif[2].Toplam; } set { OVTeklif[2].Toplam = value; } }

        public double OdenekSDis { get { return sonrakiYillar.FinKod7; } set { sonrakiYillar.FinKod7 = value; } }
        public double OdenekSDisaGiden { get { return OVTeklif[2].DisOzkaynak; } set { sonrakiYillar.DisOzkaynak = value; } }
        public double OdenekSToplam { get { return sonrakiYillar.Toplam; } set { sonrakiYillar.Toplam = value; } }

        public double KumulatifHarcamaDis { get { return kumulatifHarcama.FinKod7; } set { kumulatifHarcama.FinKod7 = value; } }
        public double KumulatifHarcamaDisaGiden { get { return kumulatifHarcama.DisOzkaynak; } set { kumulatifHarcama.DisOzkaynak = value; } }
        public double KumulatifHarcamaToplam { get { return kumulatifHarcama.Toplam; } set { kumulatifHarcama.Toplam = value; } }

        public double HarcamaTahminDis { get { return harcamaTahmin.FinKod7; } set { harcamaTahmin.FinKod7 = value; } }
        public double HarcamaTahminDisaGiden { get { return harcamaTahmin.DisOzkaynak; } set { harcamaTahmin.DisOzkaynak = value; } }
        public double HarcamaTahminToplam { get { return harcamaTahmin.Toplam; } set { harcamaTahmin.Toplam = value; } }

        public double Harcama6ayDis { get { return harcama6ay.FinKod7; } set { harcama6ay.FinKod7 = value; } }
        public double Harcama6ayDisaGiden { get { return harcama6ay.DisOzkaynak; } set { harcama6ay.DisOzkaynak = value; } }
        public double Harcama6ayToplam { get { return harcama6ay.Toplam; } set { harcama6ay.Toplam = value; } }

        public double HarcamaYilSonuDis { get { return harcamaYilSonu.FinKod7; } set { harcamaYilSonu.FinKod7 = value; } }
        public double HarcamaYilSonuDisaGiden { get { return harcamaYilSonu.DisOzkaynak; } set { harcamaYilSonu.DisOzkaynak = value; } }
        public double HarcamaYilSonuToplam { get { return harcamaYilSonu.Toplam; } set { harcamaYilSonu.Toplam = value; } }

        public double EklenenYilSonuToplam { get { return odenekEklenen.Toplam; } set { odenekEklenen.Toplam = value; } }
        public double EklenenYilSonuDis { get { return odenekEklenen.FinKod7; } set { odenekEklenen.FinKod7 = value; } }

        public double DusulenYilSonuToplam { get { return odenekDusulen.Toplam; } set { odenekDusulen.Toplam = value; } }
        public double DusulenYilSonuDis { get { return odenekDusulen.FinKod7; } set { odenekDusulen.FinKod7 = value; } }

        /// <summary>
        /// Bütçe kaynağın bilgilerinin ait olduğu yıl
        /// </summary>
        public int butceYil;

        /// <summary>
        /// Proje kaynağının toplam bilgisi
        /// </summary>
        public ProjeMaliYapi kaynakToplam;

        /// <summary>
        /// Bütçe kaynağın Orta Vadeli bütçe taklifi (3 yıllık teklif)
        /// </summary>
        public ProjeMaliYapi[] OVTeklif;

        /// <summary>
        /// Bütçe kaynağın Orta Vadeli ek bütçe taklifi (3 yıllık teklif)
        /// </summary>
        public ProjeMaliYapi[] OVTeklifEk;

        /// <summary>
        /// Bütçe yıl sonu harcama teklifi
        /// </summary>
        public ProjeMaliYapi harcamaTahmin;
        public ProjeMaliYapi kumulatifHarcama;
        public ProjeMaliYapi sonrakiYillar;

        /// <summary>
        /// Bütçe yıl sonu harcama teklifi
        /// </summary>
        public ProjeMaliYapi harcamaYilSonu;

        public ProjeMaliYapi odenekDusulen;

        public ProjeMaliYapi odenekEklenen;
        /// <summary>
        /// Bütçe yıl sonu harcama teklifi
        /// </summary>
        public ProjeMaliYapi harcama6ay;
        public int kaynakNo;

        /// <summary>
        /// Parametresiz Bütçe Kaynak Teklif nesnesi oluşturucu yordam
        /// </summary>
        public ButceKaynakTeklif()
        {
            butceYil = 0;
            kaynakToplam = new ProjeMaliYapi();
            OVTeklif = new ProjeMaliYapi[3];
            for (int i = 0; i < 3; i++)
            {
                OVTeklif[i] = new ProjeMaliYapi();
            }
            OVTeklifEk = new ProjeMaliYapi[3];
            for (int i = 0; i < 3; i++)
            {
                OVTeklifEk[i] = new ProjeMaliYapi();
            }
            sonrakiYillar = new ProjeMaliYapi();
            kumulatifHarcama = new ProjeMaliYapi();
            harcamaTahmin = new ProjeMaliYapi();
            harcamaYilSonu = new ProjeMaliYapi();
            harcama6ay = new ProjeMaliYapi();
            odenekEklenen = new ProjeMaliYapi();
            odenekDusulen = new ProjeMaliYapi();
        }

        public void DegerAta(ENUMProjeMaliYapiKod indeks, double fin7, double kaynakToplam, double disOzkaynak)
        {
            ProjeMaliYapi pmy = ProjeMaliYapiBul(indeks);
            if (pmy != null)
                pmy.DegerAta(fin7, kaynakToplam, disOzkaynak);
        }

        public int CompareTo(object obj)
        {
            if (obj is ButceKaynakTeklif)
            {
                return ((ButceKaynakTeklif)obj).butceYil.CompareTo(this.butceYil);
            }
            return 0;
        }

        public ProjeMaliYapi ProjeMaliYapiBul(ENUMProjeMaliYapiKod indeks)
        {
            if (indeks == ENUMProjeMaliYapiKod.odenekDusulen) return odenekDusulen;
            else if (indeks == ENUMProjeMaliYapiKod.odenekEklenen) return odenekEklenen;
            else if (indeks == ENUMProjeMaliYapiKod.harcamaYilSonu) return harcamaYilSonu;
            else if (indeks == ENUMProjeMaliYapiKod.harcama6ay) return harcama6ay;
            else if (indeks == ENUMProjeMaliYapiKod.kumulatifHarcama) return kumulatifHarcama;
            else if (indeks == ENUMProjeMaliYapiKod.sonrakiYillar) return sonrakiYillar;
            else if (indeks == ENUMProjeMaliYapiKod.harcamaTahmin) return harcamaTahmin;
            else if (indeks == ENUMProjeMaliYapiKod.kaynakToplam) return kaynakToplam;

            else if (indeks == ENUMProjeMaliYapiKod.teklif0) return OVTeklif[0];
            else if (indeks == ENUMProjeMaliYapiKod.teklif1) return OVTeklif[1];
            else if (indeks == ENUMProjeMaliYapiKod.teklif2) return OVTeklif[2];

            else if (indeks == ENUMProjeMaliYapiKod.ekteklif0) return OVTeklifEk[0];
            else if (indeks == ENUMProjeMaliYapiKod.ekteklif1) return OVTeklifEk[1];
            else if (indeks == ENUMProjeMaliYapiKod.ekteklif2) return OVTeklifEk[2];
            return null;
        }

        internal void TeklifDegerAta(ENUMProjeMaliYapiKod kod, int degerTur, double value)
        {
            if (kod == ENUMProjeMaliYapiKod.odenekEklenenDusulen)
            {
                ProjeMaliYapi pmy = ProjeMaliYapiBul(0);
                double tut1 = pmy.DegerBul(degerTur);
                if (value > tut1)
                {
                    TeklifDegerAta(ENUMProjeMaliYapiKod.odenekEklenen, degerTur, value - tut1);
                }
                else
                    TeklifDegerAta(ENUMProjeMaliYapiKod.odenekDusulen, degerTur, value - tut1);
            }
            else
            {
                ProjeMaliYapi pmy = ProjeMaliYapiBul(kod);
                pmy.DegerAta(degerTur, value);
            }
        }

        internal double TeklifDegerBul(ENUMProjeMaliYapiKod kod, int degerTur)
        {
            if (kod == ENUMProjeMaliYapiKod.odenekEklenenDusulen)
            {
                ProjeMaliYapi pmy = ProjeMaliYapiBul(0);
                double tut1 = pmy.DegerBul(degerTur);
                pmy = ProjeMaliYapiBul(ENUMProjeMaliYapiKod.odenekDusulen);
                tut1 += pmy.DegerBul(degerTur);
                pmy = ProjeMaliYapiBul(ENUMProjeMaliYapiKod.odenekEklenen);
                tut1 += pmy.DegerBul(degerTur);
                return tut1;
            }
            else
            {
                ProjeMaliYapi pmy = ProjeMaliYapiBul(kod);
                return pmy.DegerBul(degerTur);
            }
        }
    }

    /// <summary>
    /// Bütçe kaynağın bütçede yer alan ödenek teklifleri bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ButceKaynakOdenek
    {
        /// <summary>
        /// Bütçe kaynağın programatik kodu
        /// </summary>
        public int kaynakNo;

        /// <summary>
        /// Bütçe kaynağın ödeneklerinin bağlı olduğu yıl
        /// </summary>
        public int yil;

        /// <summary>
        /// Bütçe kaynağın ödeneklerinin bağlı olduğu senaryo
        /// </summary>
        public string senaryoKod;

        /// <summary>
        /// Bütçe kaynağın ödenekleri (3 yıllık teklif)
        /// </summary>
        public double[] odenek;

        /// <summary>
        /// Parametresiz Bütçe Kaynak Ödenek nesnesi oluşturucu yordam
        /// </summary>
        public ButceKaynakOdenek()
        {
            kaynakNo = 0;
            yil = 0;
            senaryoKod = string.Empty;
            odenek = new double[3];
        }
    }

    /// <summary>
    /// Bütçe kaynağın harcama bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ButceKaynakHarcama
    {
        /// <summary>
        /// Bütçe kaynağın programatik kodu
        /// </summary>
        public int kaynakNo;

        /// <summary>
        /// Bütçe kaynağın harcamalarının bağlı olduğu yıl
        /// </summary>
        public int yil;

        /// <summary>
        /// Bütçe kaynağın bütçede yer alan haziran harcaması
        /// </summary>
        public double haziranHarcama;

        /// <summary>
        /// Bütçe kaynağın bütçede yer almayan haziran harcaması
        /// </summary>
        public double haziranHarcamaDis;

        /// <summary>
        /// Bütçe kaynağın bütçede yer alan yıl sonu harcaması
        /// </summary>
        public double yilSonuHarcama;

        /// <summary>
        /// Bütçe kaynağın bütçede yer almayan yıl sonu harcaması
        /// </summary>
        public double yilSonuHarcamaDis;

        /// <summary>
        /// Bütçe kaynağın harcama ile ilgili son durum bilgisi
        /// </summary>
        public string sonDurum;

        /// <summary>
        /// Parametresiz Bütçe Kaynak Harcama nesnesi oluşturucu yordam
        /// </summary>
        public ButceKaynakHarcama()
        {
            kaynakNo = 0;
            yil = 0;
            haziranHarcama = 0;
            haziranHarcamaDis = 0;
            yilSonuHarcama = 0;
            yilSonuHarcamaDis = 0;
            sonDurum = "";
        }
    }

    /// <summary>
    /// Bütçe kaynağın geri ödeme bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ButceKaynakGeriOdeme
    {
        /// <summary>
        /// Bütçe kaynağın programatik kodu
        /// </summary>
        public int kaynakNo { get; set; }

        /// <summary>
        /// Bütçe kaynağın geri ödemesinin bağlı olduğu yıl
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// Bütçe kaynağın geri ödenecek ana para tutarı
        /// </summary>
        public double anaParaGeriOdeme { get; set; }

        /// <summary>
        /// Bütçe kaynağın ödenecek faiz tutarı
        /// </summary>
        public double odenecekFaiz { get; set; }

        /// <summary>
        /// Bütçe kaynağın geri ödeme ile ilgili son durum bilgisi
        /// </summary>
        public string projeninSonDurumu { get; set; }

        /// <summary>
        /// Bütçe kaynağın geri ödeme ile ilgili dar boğaz çözümü bilgisi
        /// </summary>
        public string darBogazCozum { get; set; }

        /// <summary>
        /// Parametresiz Bütçe Kaynak Geri Ödeme nesnesi oluşturucu yordam
        /// </summary>
        public ButceKaynakGeriOdeme()
        {
            kaynakNo = 0;
            yil = 0;
            anaParaGeriOdeme = 0;
            odenecekFaiz = 0;
            projeninSonDurumu = "";
            darBogazCozum = "";
        }
    }

    /// <summary>
    /// Alt projenin kredi talep bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class KrediTalep
    {
        /// <summary>
        /// Alt projenin programatik kodu
        /// </summary>
        public int altProjeKodu;

        /// <summary>
        /// Kredi talebin programatik kodu
        /// </summary>
        public int krediNo;

        /// <summary>
        /// Kredi talebin bağlı olduğu yıl
        /// </summary>
        public int yil;

        /// <summary>
        /// Kredi talep edilirken hedeflenen amaç
        /// </summary>
        public string amac;

        /// <summary>
        /// Toplam kredi ihtiyacı tutarı
        /// </summary>
        public double krediIhtiyaci;

        /// <summary>
        /// Temin edilen kredi tutarı
        /// </summary>
        public double teminEdilen;

        /// <summary>
        /// Talep edilen kredi tutarı
        /// </summary>
        public double talepEdilen;

        /// <summary>
        /// Kredi talebine alternatif düşünceler
        /// </summary>
        public string alternatif;

        /// <summary>
        /// Kredi talebiyle ilgili düşünceler
        /// </summary>
        public string dusunceler;

        /// <summary>
        /// Kredi talebinin adı
        /// </summary>
        public string talepAd;

        /// <summary>
        /// Parametresiz Kredi Talep nesnesi oluşturucu yordam
        /// </summary>
        public KrediTalep()
        {
            altProjeKodu = 0;
            krediNo = 0;
            yil = 0;
            amac = "";
            krediIhtiyaci = 0;
            teminEdilen = 0;
            talepEdilen = 0;
            alternatif = "";
            dusunceler = "";
            talepAd = string.Empty;
        }
    }

    /// <summary>
    /// Alt projenin doküman bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Dokuman
    {
        /// <summary>
        /// Alt projenin programatik kodu
        /// </summary>
        public int altProjeKodu;

        /// <summary>
        /// Dokümanın programatik kodu
        /// </summary>
        public string siraNo;

        /// <summary>
        /// Dokümanın tür bilgisi
        /// </summary>
        public Sozluk tur;

        /// <summary>
        /// Dokümanın adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Dokümana eklenen dosyanın adı
        /// </summary>
        public string dosyaAd;

        /// <summary>
        /// Parametresiz Doküman nesnesi oluşturucu yordam
        /// </summary>
        public Dokuman()
        {
            altProjeKodu = 0;
            siraNo = "";
            tur = new Sozluk();
            ad = "";
            dosyaAd = "";
        }
    }

    /// <summary>
    /// Ödenek teklif bilgilerini etüd proje, devam eden proje, yeni proje türleri bazında tutan sınıf
    /// </summary>
    [Serializable()]
    public class TeklifToplam
    {
        /// <summary>
        /// Ödenek tekliflerinin bağlı olduğu yıl (Bütçe Hazırlık yılı)
        /// </summary>
        public int yil;

        /// <summary>
        /// İlgili ödeneklerin 3 yıllık bütçede bağlı olduğu teklif yılı (Bütçe Hazırlık yılı, +1, +2)
        /// </summary>
        public int teklifYil;

        /// <summary>
        /// Etüd projelerin ödenek teklifi toplamı
        /// </summary>
        public double etudProjeToplami;

        /// <summary>
        /// Devam eden projelerin ödenek teklifi toplamı
        /// </summary>
        public double devamEdenProjeToplami;

        /// <summary>
        /// Yeni projelerin ödenek teklifi toplamı
        /// </summary>
        public double yeniProjeToplami;

        /// <summary>
        /// Ödenek tekliflerinin genel toplamı
        /// </summary>
        public double toplam;

        /// <summary>
        /// Parametresiz Teklif toplam nesnesi oluşturucu yordam
        /// </summary>
        public TeklifToplam()
        {
            yil = 0;
            teklifYil = 0;
            etudProjeToplami = 0;
            devamEdenProjeToplami = 0;
            yeniProjeToplami = 0;
            toplam = 0;
        }
    }

    /// <summary>
    /// DPT Tablo-4 raporu bilgilerini tutan sınıf
    /// Bu sınıf, raporun tek satırını ifade eder.
    /// Proje tutar ve karakteristik bilgilerini içerir.
    /// </summary>
    [Serializable()]
    public class Tablo4
    {
        /// <summary>
        /// Projenin DPT Numarası
        /// </summary>
        public string dptNo;

        /// <summary>
        /// Projenin adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Projenin karekteristiği
        /// </summary>
        public string karakteristik;
        /// <summary>
        /// Projenin toplam tutarı
        /// </summary>
        public double butce;

        /// <summary>
        /// Projenin dışa giden toplam tutarı
        /// </summary>
        public double butceDis;
    }

    /// <summary>
    /// Birim, sektör ve/veya tertip bazında yatırım projeleri ödenek teklif ve harcama bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class TeklifDagilim
    {
        /// <summary>
        /// Birim bilgisi
        /// </summary>
        public ABSKod birim;

        /// <summary>
        /// Birime ait proje nesnelerinin tutulduğu dizi
        /// </summary>
        public TNSCollection projeler;

        /// <summary>
        /// Parametresiz Teklif Dağılım nesnesi oluşturucu yordam
        /// </summary>
        public TeklifDagilim()
        {
            birim = new ABSKod();
            projeler = new TNSCollection();
        }
    }

    /// <summary>
    /// Hizmet alımı suretiyle kullanılan/kullanılacak taşıtlara ilişkin ödenek teklif bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class TasitAlimBedeliBilgisi
    {
        /// <summary>
        /// Taşıt sözlük numarası
        /// </summary>
        public string tasitSiraNo = null;

        /// <summary>
        /// Yıllar itibariyle ödenek teklifleri dizisi
        /// </summary>
        public TNSCollection yillar = new TNSCollection();
    }

    /// <summary>
    /// Taşıt talep bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class TasitTalepBilgi
    {
        /// <summary>
        /// Taşıt sözlük numarası
        /// </summary>
        public string tasitSiraNo = null;

        /// <summary>
        /// Sahip olunan taşıt sayısı
        /// </summary>
        public int mevcutAdet = 0;

        /// <summary>
        /// Sahip olunan taşıtların modeli
        /// </summary>
        public string mevcutModel = null;

        /// <summary>
        /// Talep edilen taşıt sayısı
        /// </summary>
        public int talepAdet = 0;

        /// <summary>
        /// Talebe ilişkin açıklama
        /// </summary>
        public string talepAciklama = null;
    }

    /// <summary>
    /// DPT Vize Cetveli raporu bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class DPTVizeBilgi
    {
        /// <summary>
        /// Tertip bilgisi
        /// </summary>
        public Tertip tertip;

        /// <summary>
        /// Sektör bilgisi
        /// </summary>
        public SektorSozluk sektor;

        /// <summary>
        /// Ödenek teklifleri dizisi
        /// </summary>
        public double[] odenek;

        /// <summary>
        /// Yıl parametresi alan DPT Vize Bilgi nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="yil">tertip değişkeninin hangi yıl için oluşturulacağını belirler.</param>
        public DPTVizeBilgi(int yil)
        {
            tertip = new Tertip(yil, ENUMTabloKod.TABOdenek);
            sektor = new SektorSozluk();
            odenek = new double[3];
        }
    }

    [Serializable()]
    public class AltProjeTeklif
    {
        public AltProjeTeklif()
        {
        }

        public AltProjeTeklif(int yil, string birim, string sektorNo)
        {
            this.yil = yil;
            this.birim = birim;
            this.sektorNo = sektorNo;
        }

        public int yil { get; set; }
        public string senaryo { get; set; }
        public string birim { get; set; }
        public string sektorNo { get; set; }
        public string ad { get; set; }
        public int baslangicYil { get; set; }
        public int bitisYil { get; set; }
        public double projeTutari { get; set; }
        public double yiliTeklifi0 { get; set; }
        public double yiliTeklifi1 { get; set; }
        public double yiliTeklifi2 { get; set; }
        public string gerekce { get; set; }
    }

    [Serializable()]
    public class ProjeEkliDosya
    {
        public string dosyaTur
        {
            get
            {
                string ext = System.IO.Path.GetExtension(dosyaAd).Replace(".", ""); ;

                if (ext.ToLower() == "xls" || ext.ToLower() == "xlsx")
                    ext = "XLS";
                else if (ext.ToLower() == "doc" || ext.ToLower() == "docx")
                    ext = "DOC";
                else if (ext.ToLower() == "ppt" || ext.ToLower() == "pptx")
                    ext = "PPT";
                else if (ext.ToLower() == "pdf")
                    ext = "PDF";
                else if (ext.ToLower() == "zip" || ext.ToLower() == "rar")
                    ext = "ZIP";
                else
                    ext = "XXX";

                return "<img src=../App_themes/images/evrakDosyaTip" + ext + ".gif>";
            }
        }
        public int projeKodu { get; set; }
        public int altProjeKodu { get; set; }
        public string fizikiDosyaAd { get; set; }

        public string siraNo { get; set; }

        public string dosyaAd { get; set; }
        public string aciklama { get; set; }

        public TNSDateTime yuklemeZamani { get; set; }

        public string kullanicikodu { get; set; }
    }

    /// <summary>
    /// Deflator: Deflatör
    /// </summary>
    [System.Serializable()]
    public class Deflator
    {
        /// <summary>
        /// Sektor Kod: Sektör
        /// </summary>
        public string sektorKod { get; set; }

        /// <summary>
        /// Sektor Kod: Sektör
        /// </summary>
        public string sektorAd { get; set; }

        /// <summary>
        /// Deflatör: Deflatör
        /// </summary>
        public double deflator { get; set; }
    }

    /// <summary>
    /// AltProjeDetayProgrami: Alt Proje Detay Programı
    /// </summary>
    [System.Serializable()]
    public class AltProjeDetayProgrami
    {
        /// <summary>
        /// Yıl: Yıl
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// Senaryo Kod: Senaryo Kod
        /// </summary>
        public string senaryoKod { get; set; }

        /// <summary>
        /// Alt Proje No: Alt Proje No
        /// </summary>
        public int altProjeNo { get; set; }

        /// <summary>
        /// Alt Proje No: Alt Proje No
        /// </summary>
        public int projeNo { get; set; }

        /// <summary>
        /// Sıra No: Sıra No
        /// </summary>
        public int siraNo { get; set; }

        /// <summary>
        /// Yer: İl/İlçe/Yer
        /// </summary>
        public string yer { get; set; }

        /// <summary>
        /// Birimi: Birimi
        /// </summary>
        public string birimi { get; set; }

        /// <summary>
        /// Saymanlık Kod: Saymanlık Kod
        /// </summary>
        public int saymanlikKod { get; set; }

        /// <summary>
        /// Birim Kod: Birim Kod
        /// </summary>
        public string birimKod { get; set; }

        /// <summary>
        /// Odenek: Ödenek Teklifi (TL)
        /// </summary>
        public double odenek { get; set; }

        /// <summary>
        /// Açıklama: DETAY KARAKTERİSTİK (Faaliyetin adı ve miktarı kısa ve öz yazılacaktır.)
        /// </summary>
        public string aciklama { get; set; }
    }

    /// <summary>
    /// KalkinmaBolge: Kalkinma Bölgelesi
    /// </summary>
    [System.Serializable()]
    public class KalkinmaBolge
    {
        /// <summary>
        /// Bölge Kod: Bölge Kodu
        /// </summary>
        public string bolgeKod { get; set; }

        /// <summary>
        /// Bölge Ad: Bölge Adı
        /// </summary>
        public string bolgeAd { get; set; }

        /// <summary>
        /// KalkinmaBolgeIl: Bölgede Bulunan İller
        /// </summary>
        public TNSCollection kalkinmaBolgeIl;
    }
    ///// <summary>
    ///// Kredi Kullanım bilgileri sınıfı.
    ///// </summary>
    //[Serializable()]
    //public class KrediKullanim
    //{
    //    public int altProjeKodu;
    //    public int krediNo;
    //    public int yil;
    //    public double anlasmaTutari;
    //    public double kullanilanTutar;
    //    public double kullanilacakTutar;

    //    public KrediKullanim()
    //    {
    //        altProjeKodu = 0;
    //        krediNo = 0;
    //        yil = 0;
    //        anlasmaTutari = 0;
    //        kullanilanTutar = 0;
    //        kullanilacakTutar = 0;
    //    }
    //}
}