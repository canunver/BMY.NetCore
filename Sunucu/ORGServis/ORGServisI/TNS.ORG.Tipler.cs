using System;
using System.Collections;

namespace TNS.ORG
{
    /// <summary>
    /// Personel Sınıfı
    /// </summary>
    [Serializable()]
    public class Personel
    {
        /// <summary>
        /// T.C. Kimlik numarası
        /// </summary>
        public string tcKimlikNo = "";
        /// <summary>
        /// Sicil numarası
        /// </summary>
        public string sicilno = "";
        /// <summary>
        /// Personelin Adı
        /// </summary>
        public string ad = "";
        /// <summary>
        /// Personelin Soyadı
        /// </summary>
        public string soyad = "";
        /// <summary>
        /// Cinsiyeti
        /// </summary>
        public int cinsiyet = 0;
        /// <summary>
        /// İlk soyadı
        /// </summary>
        public string ilksoyad = "";
        /// <summary>
        /// Ana adı
        /// </summary>
        public string anaad = "";
        /// <summary>
        /// Baba adı
        /// </summary>
        public string babaad = "";
        /// <summary>
        /// Doğum yeri
        /// </summary>
        public string dogumyeri = "";
        /// <summary>
        /// Doğum Tarihi
        /// </summary>
        public TNSDateTime dogumtarihi = new TNSDateTime();
        /// <summary>
        /// Medeni Hali
        /// </summary>
        public int medenihal = 0;
        /// <summary>
        /// Kan Grubu
        /// </summary>
        public int kangrubu = 0;
        /// <summary>
        /// Nüfusun bağlı olduğu ilçe kodu
        /// </summary>
        public int nufusilcekod = 0;
        /// <summary>
        /// Nüfus cüzdanı seri numarası
        /// </summary>
        public string cuzdanserino = "";
        /// <summary>
        /// Nüfus cüzdanı kayıt numarası
        /// </summary>
        public string cuzdankayitno = "";
        /// <summary>
        /// Nüfusun bağlı olduğu mahalle/köy
        /// </summary>
        public string mahallekoy = "";
        /// <summary>
        /// Cilt numarası
        /// </summary>
        public int ciltno = 0;
        /// <summary>
        /// Sıra numarası
        /// </summary>
        public int sirano = 0;
        /// <summary>
        /// Aile sıra numarası
        /// </summary>
        public int ailesirano = 0;
        /// <summary>
        /// Nüfus cüzdanının Veriliş nedeni
        /// </summary>
        public string verilisnedeni = "";
        /// <summary>
        /// Nüfus cüzdanının verildiği yer
        /// </summary>
        public string verildigiyer = "";
        /// <summary>
        /// Nüfus cüzdanı veriliş tarihi
        /// </summary>
        public TNSDateTime verilistarihi = new TNSDateTime();
        /// <summary>
        /// Emekli sicil numarası
        /// </summary>
        public string emeklisicilno = "";
        /// <summary>
        /// Bakanlık kart numarası
        /// </summary>
        public string bakanlikkartno = "";
        /// <summary>
        /// Sağlık karne numarası
        /// </summary>
        public string saglikkarneno = "";
        /// <summary>
        /// SSK sicil numarası
        /// </summary>
        public string ssksicilno = "";
        /// <summary>
        /// BAĞKUR sicil numarası
        /// </summary>
        public string bagkursicilno = "";
        /// <summary>
        /// Sürücü belge numarası-Ehliyet Numarası
        /// </summary>
        public string surucubelgeno = "";
        /// <summary>
        /// Sürücü belge tarihi
        /// </summary>
        public TNSDateTime surucubelgetarihi = new TNSDateTime();
        /// <summary>
        /// Yaş tashih-düzeltme tarihi
        /// </summary>
        public TNSDateTime yastashihtarihi = new TNSDateTime();
        /// <summary>
        /// Bağlı olduğu muhasebe kodu
        /// </summary>
        public int muhasebekodu = 0;
        /// <summary>
        /// Bağlı olduğu birim kodu
        /// </summary>
        public string birimkodu = "";
        /// <summary>
        /// Çalıştığı oda kodu
        /// </summary>
        public string odakodu = "";
        /// <summary>
        /// Bağlı olduğu muhasebe kodu
        /// </summary>
        public string muhasebekod = "";
        /// <summary>
        /// Bağlı olduğu harcama birim kodu
        /// </summary>
        public string harcamabirimkod = "";
        /// <summary>
        /// Organizasyonel Kodu
        /// </summary>
        public int organizasyonkod = 0;
        /// <summary>
        /// Persponel resim bilgisi
        /// </summary>
    }

    /// <summary>
    /// Görevlendirme Sınıfı
    /// </summary>
    [Serializable()]
    public class Gorev
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// Muhasebe adı
        /// </summary>
        public string muhasebeAd;
        /// <summary>
        /// Birim kodu
        /// </summary>
        public string birimKod;
        /// <summary>
        /// Birim adı
        /// </summary>
        public string birimAd;
        /// <summary>
        /// Amaç
        /// </summary>
        public string amac;
        /// <summary>
        /// Tür
        /// </summary>
        public int tur;
        /// <summary>
        /// İşlem kodu
        /// </summary>
        public int islemKod;
        /// <summary>
        /// Durum
        /// </summary>
        public int durum;
        /// <summary>
        /// Görev yerleri
        /// </summary>
        public ObjectArray gorevYerleri;
        /// <summary>
        /// Görevli personeller
        /// </summary>
        public ObjectArray gorevliPersoneller;
        /// <summary>
        /// Görev tertipleri
        /// </summary>
        public ObjectArray gorevTertipler;
        /// <summary>
        /// Evrak numarası
        /// </summary>
        public string evrEvrakNo;
        /// <summary>
        /// Program evrak numarası
        /// </summary>
        public string prEvrakNo;
        /// Sınıfın oluşturucu(Constructer) fonksiyonu
        public Gorev()
        {
            kod = 0;
            muhasebeKod = 0;
            birimKod = "";
            amac = "";
            tur = 0;
            islemKod = 0;
            durum = 0;
            gorevYerleri = new ObjectArray();
            gorevliPersoneller = new ObjectArray();
            gorevTertipler = new ObjectArray();
            evrEvrakNo = string.Empty;
            prEvrakNo = string.Empty;
        }
    }

    /// <summary>
    /// Görevlendirme Yer Tarih Sınıfı
    /// </summary>
    [Serializable()]
    public class GorevYerTarih
    {
        /// <summary>
        /// Görevlendirme kodu
        /// </summary>
        public int gorevKod;
        /// <summary>
        /// Sıra numarası
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Bulunduğu ülke kodu
        /// </summary>
        public string bulunduguUlkeKod;
        /// <summary>
        /// Bulunduğu il kodu
        /// </summary>
        public string bulunduguIlKod;
        /// <summary>
        /// Bulunduğu ilçe kodu
        /// </summary>
        public string bulunduguIlceKod;
        /// <summary>
        /// Bulunduğu yer
        /// </summary>
        public string bulunduguYer;
        /// <summary>
        /// Gideceği ülke kodu
        /// </summary>
        public string gidecegiUlkeKod;
        /// <summary>
        /// Gideceği il kodu
        /// </summary>
        public string gidecegiIlKod;
        /// <summary>
        /// Gideceği ilçe kodu
        /// </summary>
        public string gidecegiIlceKod;
        /// <summary>
        /// Gideceği yer
        /// </summary>
        public string gidecegiYer;
        /// <summary>
        /// Başlangıç tarihi
        /// </summary>
        public TNSDateTime baslangicTarihi;
        /// <summary>
        /// Bitiş tarihi
        /// </summary>
        public TNSDateTime bitisTarihi;
        /// <summary>
        /// Vasıta
        /// </summary>
        public int vasita;
        /// <summary>
        /// Yol dahil mi
        /// </summary>
        public int yolDahilMi;
        /// Sınıfın oluşturucu(Constructer) fonksiyonu
        public GorevYerTarih()
        {
            gorevKod = 0;
            siraNo = 0;
            bulunduguUlkeKod = "";
            bulunduguIlKod = "";
            bulunduguIlceKod = "";
            bulunduguYer = "";
            gidecegiUlkeKod = "";
            gidecegiIlKod = "";
            gidecegiIlceKod = "";
            gidecegiYer = "";
            baslangicTarihi = new TNSDateTime();
            bitisTarihi = new TNSDateTime();
            vasita = 0;
            yolDahilMi = 0;
        }
    }

    /// <summary>
    /// Görevlendirme Personel Sınıfı
    /// </summary>
    [Serializable()]
    public class GorevPersonel
    {
        /// <summary>
        /// Görevlendirme kodu
        /// </summary>
        public int gorevKod;
        /// <summary>
        /// T.C. Kimlik numarası
        /// </summary>
        public string tckimlikno;
        /// <summary>
        /// Ad
        /// </summary>
        public string ad;
        /// <summary>
        /// Soyad
        /// </summary>
        public string soyad;
        /// <summary>
        /// Ünvan kodu
        /// </summary>
        public int unvanKod;
        /// <summary>
        /// Ünvan
        /// </summary>
        public string unvan;
        /// <summary>
        /// Kadro derece
        /// </summary>
        public int kadroDerece;
        /// <summary>
        /// Ek gösterge
        /// </summary>
        public int ekGosterge;
        /// <summary>
        /// Avans tutarı
        /// </summary>
        public double avansTutari;
        /// <summary>
        /// Avans para birimi
        /// </summary>
        public int avansParaBirimKod;
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int surecYil;
        /// <summary>
        /// Süreç numarası
        /// </summary>
        public string surecNo;
        /// Sınıfın oluşturucu(Constructer) fonksiyonu
        public GorevPersonel()
        {
            gorevKod = 0;
            tckimlikno = string.Empty;
            ad = string.Empty;
            soyad = string.Empty;
            unvanKod = 0;
            unvan = string.Empty;
            kadroDerece = 0;
            ekGosterge = 0;
            avansTutari = 0.0;
            avansParaBirimKod = 0;
            surecYil = 0;
            surecNo = string.Empty;
        }
    }

    /// <summary>
    /// Görevlendirme Tertip Sınıfı
    /// </summary>
    [Serializable()]
    public class GorevTertip
    {
        /// <summary>
        /// Görevlendirme kodu
        /// </summary>
        public int gorevKod;
        /// <summary>
        /// Sıra numarası
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Tertip
        /// </summary>
        public string tertip;
        /// <summary>
        /// Kullanılacak tutar
        /// </summary>
        public double kullanilacakTutar;
        /// Sınıfın oluşturucu(Constructer) fonksiyonu

        public GorevTertip()
        {
            gorevKod = 0;
            siraNo = 0;
            tertip = string.Empty;
            kullanilacakTutar = 0.0;
        }
    }

    /// <summary>
    /// Nesne Özellik Sınıfı
    /// </summary>
    [Serializable()]
    public class OrganizasyonBirim
    {
        /// <summary>
        /// Nesne özellik kodu
        /// </summary>
        public int kod = 0;
        /// <summary>
        /// Adı
        /// </summary>
        public string adi = "";
        /// <summary>
        /// Kısa adı
        /// </summary>
        public string kisaadi = "";
        /// <summary>
        /// Ana birim kod
        /// </summary>
        public int anaBirimKod = 0;
        /// <summary>
        /// Başlangıç tarihi
        /// </summary>
        public TNSDateTime baslangicTarih = new TNSDateTime();
        /// <summary>
        /// Bitiş tarihi
        /// </summary>
        public TNSDateTime bitisTarih = new TNSDateTime();
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public string muhasebekodu = "";
        /// <summary>
        /// Muhasebe birimi
        /// </summary>
        public string muhasebebirim = "";
        /// <summary>
        /// Evrak birimi
        /// </summary>
        public string evrakbirim = "";
        /// <summary>
        /// Ülke
        /// </summary>
        public string ulke = "";
        public string ulkeAd = "";
        /// <summary>
        /// Bölge
        /// </summary>
        public string bolge = "";
        public string bolgeAd = "";
        /// <summary>
        /// İl
        /// </summary>
        public string il = "";
        public string ilAd = "";
        /// <summary>
        /// İlçe
        /// </summary>
        public string ilce = "";
        public string ilceAd = "";
        /// <summary>
        /// Kurumsal kodu
        /// </summary>
        public string kurumsalkod = "";
        /// <summary>
        /// Vergi numarası
        /// </summary>
        public string vergino = "";
        /// <summary>
        /// Vergi dairesi
        /// </summary>
        public string vergidairesi = "";
        /// <summary>
        /// Adres
        /// </summary>
        public string adres = "";
        /// <summary>
        /// Telefon
        /// </summary>
        public string telefon = "";
        /// <summary>
        /// Faks
        /// </summary>
        public string faks = "";
        /// <summary>
        /// E-Posta
        /// </summary>
        public string eposta = "";
        /// <summary>
        /// Url
        /// </summary>
        public string url = "";
        /// <summary>
        /// Antet
        /// </summary>
        public string antet = "";
        /// <summary>
        /// Yer bilgisi taşıyormu?
        /// </summary>
        public int yerMi = 0;
        /// <summary>
        /// Harcama bilgisi taşıyormu?
        /// </summary>
        public int harcamaMi = 0;
        /// <summary>
        /// Bütçe bilgisi taşıyormu?
        /// </summary>
        public int butceMi = 0;
        /// <summary>
        /// Taşınır bilgisi taşıyormu?
        /// </summary>
        public int tasinirMi = 0;
        /// <summary>
        /// Personel bilgisi taşıyormu?
        /// </summary>
        public int personelMi = 0;
        /// <summary>
        /// Evrak bilgisi taşıyormu?
        /// </summary>
        public int evrakMi = 0;
        /// <summary>
        /// Ülke bilgisi taşıyormu?
        /// </summary>
        public int ulkeMi = 0;
        /// <summary>
        /// Bölge bilgisi taşıyormu?
        /// </summary>
        public int bolgeMi = 0;
        /// <summary>
        /// İl bilgisi taşıyormu?
        /// </summary>
        public int ilMi = 0;
        /// <summary>
        /// İlçe bilgisi taşıyormu?
        /// </summary>
        public int ilceMi = 0;
        /// <summary>
        /// İletişim bilgisi taşıyormu?
        /// </summary>
        public int iletisimMi = 0;
        /// <summary>
        /// Kullanıcının yetkili olduğu muhasebe birim kodları. Sorgulama sırasında kullanılması için
        /// </summary>
        public string yetkiliOlduguMuhasebeBirim = "";

        public string kopruKod = string.Empty;
        public int yoneticiKod = 0;
        public string yoneticiAd = string.Empty;
        public int siraNo = 0;
        public bool sablonMu = false;
        public string fonKod = string.Empty;
        public int merkezKod = 0;
        public string merkezAd = string.Empty;
        public OrgHarcamaKod[] orgHarcamaKod;
        public OrgButceKod[] orgButceKod;
        public int ustYoneticiKod1 = 0;
        public string ustYoneticiAd1 = string.Empty;
        public int ustYoneticiKod2 = 0;
        public string ustYoneticiAd2 = string.Empty;
        public string turKod = string.Empty;
        public string turAd = string.Empty;
        public bool cizimCikisNoktasi = false;
        public bool cizimdeGorunme = false;
        public int kurumMu = 0;
        public string kurumKod = string.Empty;
        public int ambarMi = 0;
        public string ambarKod = string.Empty;
        public bool altDalVar = false;
        public string yoneticiTCKimlik = string.Empty;
        public string yoneticiTCKimlikAd = string.Empty;
        public string ustYoneticiTCKimlik1 = string.Empty;
        public string ustYoneticiTCKimlikAd1 = string.Empty;
        public string ustYoneticiTCKimlik2 = string.Empty;
        public string ustYoneticiTCKimlikAd2 = string.Empty;
        public string entegreKod = string.Empty;
        public int evrGenelEvrakMi;
        public string evrUstBirimKodu = "";
        public int evrKisiyeHavale = 0;
        public int evrSeviye = 0;
        public bool ozellikAra = true;
        public int listeleAktifYil = 0;

        public BirimTip[] YazilimYetkiBirimTipBul()
        {
            ArrayList al = new ArrayList();

            if (kurumMu == 1)
                al.Add(TNS.ORG.BirimTip.ButceKurumu);

            if (harcamaMi == 1)
                al.Add(TNS.ORG.BirimTip.HarcamaBirimi);

            if (butceMi == 1)
                al.Add(TNS.ORG.BirimTip.ButceBirimi);

            if (tasinirMi == 1)
                al.Add(TNS.ORG.BirimTip.TasinirBirimi);

            if (personelMi == 1)
                al.Add(TNS.ORG.BirimTip.PersonelBirimi);

            if (evrakMi == 1)
                al.Add(TNS.ORG.BirimTip.EvrakBirimi);

            if (ambarMi == 1)
                al.Add(TNS.ORG.BirimTip.Ambar);

            //
            //TNS.ORG.BirimTip.ButceKurumu
            //
            //
            //
            //

            return (TNS.ORG.BirimTip[])al.ToArray(typeof(TNS.ORG.BirimTip));
        }

        internal string BirimKodlariOl(bool merkez, bool tasra, bool yurtdisi)
        {
            string retVal = "";
            if (merkez) retVal = StringEkle(retVal, MTYKodBul(1), ";");
            if (tasra) retVal = StringEkle(retVal, MTYKodBul(2), ";");
            if (yurtdisi) retVal = StringEkle(retVal, MTYKodBul(3), ";");
            return retVal;
        }

        private string StringEkle(string onStr, string ekStr, string ek)
        {
            if (string.IsNullOrEmpty(ekStr)) return onStr;
            if (string.IsNullOrEmpty(onStr)) return ekStr;
            return onStr + ek + ekStr;
        }

        private string MTYKodBul(int MTYsiraNo)
        {
            for (int i = 0; i < orgButceKod.Length; i++)
            {
                if (orgButceKod[i].tur == MTYsiraNo)
                    return orgButceKod[i].kurKod;
            }
            return null;
        }

        public bool TutanOzellikVarMi(OrganizasyonBirim ob)
        {
            if (ob == null || !ob.ozellikAra)
                return true;

            if (((ob.ambarMi > 0 && this.ambarMi == ob.ambarMi) || (ob.bolgeMi > 0 && this.bolgeMi == ob.bolgeMi) ||
                (ob.butceMi > 0 && this.butceMi == ob.butceMi) || (ob.evrakMi > 0 && this.evrakMi == ob.evrakMi) ||
                (ob.harcamaMi > 0 && this.harcamaMi == ob.harcamaMi) || (ob.ilceMi > 0 && this.ilceMi == ob.ilceMi) ||
                (ob.iletisimMi > 0 && this.iletisimMi == ob.iletisimMi) || (ob.ilMi > 0 && this.ilMi == ob.ilMi) ||
                (ob.kurumMu > 0 && this.kurumMu == ob.kurumMu) || (ob.personelMi > 0 && this.personelMi == ob.personelMi) ||
                (ob.tasinirMi > 0 && this.tasinirMi == ob.tasinirMi) || (ob.ulkeMi > 0 && this.ulkeMi == ob.ulkeMi) ||
                (ob.yerMi > 0 && this.yerMi == ob.yerMi)) && TutanTurVarMi(ob.turKod.Split(';')))
                return true;

            return false;
        }

        public bool TutanTurVarMi(string[] turler)
        {
            if (turler == null || turler.Length <= 0)
                return true;
            if (string.IsNullOrEmpty(turKod))
                return true;

            foreach (string tur in turler)
                if (string.IsNullOrEmpty(tur) || (!string.IsNullOrEmpty(tur) && turKod.Replace(".", "").StartsWith(tur.Replace(".", ""))))
                    return true;
            return false;
        }
        public string altFaaliyetKodlar { get; set; }
        public bool cokluSecim = false;

        public string adlarSorgu = null;
    }

    [Serializable()]
    public class OrgHarcamaKod
    {
        public int kod = 0;
        public string muhasebeBirimKod = string.Empty;
        public string muhasebeBirimAd = string.Empty;
        public string kurKod = string.Empty;
        public string kurAd = string.Empty;
        public string fonKod = string.Empty;
        public string fonAd = string.Empty;
    }

    [Serializable()]
    public class OrgButceKod
    {
        public int tur = 0;
        public string kurKod = string.Empty;
        public string kurAd = string.Empty;
    }

    [Serializable()]
    public class GrafikOrgDal
    {
        public int kod = 0;
        public string kopruKod = string.Empty;
        public string ad = string.Empty;
        public string kisaAd = string.Empty;
        public string yoneticiAd = string.Empty;
        public ENUMGrafikOrgDalTur tur;
        public GrafikOrgDal[] altDallar;
        public int baglanacakYoneticiKod = 0;

        public GrafikOrgDal()
        {
        }

        public GrafikOrgDal(OrganizasyonBirim ob)
            : this(ob, ENUMGrafikOrgDalTur.BIRIM)
        {
        }

        public GrafikOrgDal(OrganizasyonBirim ob, ENUMGrafikOrgDalTur p_tur)
        {
            kod = ob.kod;
            kopruKod = ob.kopruKod;
            ad = ob.adi;
            kisaAd = ob.kisaadi;
            yoneticiAd = ob.yoneticiAd;
            tur = p_tur;
            baglanacakYoneticiKod = ob.ustYoneticiKod1;
        }

        public bool DalEkle(GrafikOrgDal dal)
        {
            if (dal == null)
                return false;

            if (altDallar != null)
            {
                foreach (GrafikOrgDal d in altDallar)
                {
                    bool eklendi = d.DalEkle(dal);
                    if (eklendi)
                        return true;
                }
            }

            if ((dal.tur == ENUMGrafikOrgDalTur.BIRIM && tur == ENUMGrafikOrgDalTur.BIRIM && dal.kopruKod.Equals(kopruKod)) ||
                (dal.tur == ENUMGrafikOrgDalTur.YONETICI && tur == ENUMGrafikOrgDalTur.YONETICI && dal.kod.Equals(kod) && dal.yoneticiAd.Equals(yoneticiAd)))
                return true;

            if ((tur == ENUMGrafikOrgDalTur.BIRIM && dal.kopruKod.Contains(kopruKod)) ||
                (tur == ENUMGrafikOrgDalTur.YONETICI && dal.baglanacakYoneticiKod.Equals(kod)))
            {
                if (dal.tur == ENUMGrafikOrgDalTur.YONETICI)
                    dal.kopruKod = string.Empty;

                if (altDallar == null)
                    altDallar = new GrafikOrgDal[1];
                else
                {
                    GrafikOrgDal[] altDallarYeni = new GrafikOrgDal[altDallar.Length + 1];
                    altDallar.CopyTo(altDallarYeni, 0);
                    altDallar = altDallarYeni;
                }
                altDallar[altDallar.Length - 1] = dal;
                return true;
            }

            return false;
        }
    }

    [Serializable()]
    public class KisiGorev
    {
        public string tcKimlikNo;
        public int orgKod;
        public string orgAd;
        public int gorevOrgKod;
        public int gorevSiraNo;
        public string gorevAd;
        public KisiGorev()
        {
            orgKod = 0;
            orgAd = String.Empty;
            gorevOrgKod = 0;
            gorevSiraNo = 0;
            gorevAd = string.Empty;
            tcKimlikNo = string.Empty;
        }
    }

    [Serializable()]
    public class CografyaBirimSayisi
    {
        public string cografiKod;
        public string ad;
        public int say;

        public CografyaBirimSayisi(string cografiKod, string ad, int say, bool noktaKoy)
        {
            // TODO: Complete member initialization
            this.cografiKod = cografiKod;
            this.ad = ad;
            this.say = say;
        }
    }
}