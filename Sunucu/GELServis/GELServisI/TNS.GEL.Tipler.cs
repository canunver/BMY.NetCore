using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS;
using TNS.HRC;

namespace TNS.GEL
{
    [Serializable]
    public class Dosya
    {
        public int sirano;
        public int yil;
        public string anahtar;
        public string dosyayolu;
        public string dosyaadi;
        public int tur;
        public string aciklama;
        public Dosya()
        {
            sirano = 0;
            yil = 0;
            anahtar = string.Empty;
            dosyayolu = string.Empty;
            dosyaadi = string.Empty;
            tur = 0;
            aciklama = string.Empty;
        }


    }

    [Serializable]
    public class MuhasebeIslemForm : TNS.HRC.OdemeEmriMIFForm
    {

        public FirmaBilgisi firma;
        public Tasinmaz tasinmaz;
        public int taksitno;
        public int prmakbuzno;

        public MuhasebeIslemForm()
        {

            firma = new FirmaBilgisi();
            tasinmaz = new Tasinmaz();
            taksitno = 0;
            prmakbuzno = 0;
        }
    }

    [Serializable]
    public class MuhasebeIslemFormDetay : TNS.HRC.OdemeEmriMIFDetay
    {
        public FirmaBilgisi firma;
        public Tasinmaz tasinmaz;
        public int taksitno;
        public int sirano;
        public MuhasebeIslemFormDetay()
        {
            firma = new FirmaBilgisi();
            tasinmaz = new Tasinmaz();
            taksitno = 0;
            sirano = 0;
        }
    }

    [Serializable]
    public class Makbuz : TNS.HRC.FaturaForm
    {
        public int prmakbuzno;
        public string makbuzno;
        public TNSDateTime makbuztarihi;
        public decimal gelirtevkifati;

        public int taksitno;
        public Tasinmaz tasinmaz;
        public Makbuz()
        {
            prmakbuzno = 0;
            makbuzno = string.Empty;
            makbuztarihi = null;

            taksitno = 0;
            tasinmaz = new Tasinmaz();
        }
    }

    [Serializable]
    public class MakbuzDetay
    {
        public int yil;
        public int muhasebe;
        public string birim;
        public string surecno;
        public FirmaBilgisi firma;
        public int prmakbuzno;
        public Tasinmaz tasinmaz;
        public int sirano;
        public decimal miktar;
        public decimal kdv;
        public decimal tutar;
        public int taksitno;
        public MakbuzDetay()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecno = string.Empty;
            firma = new FirmaBilgisi();
            prmakbuzno = 0;
            tasinmaz = new Tasinmaz();
            sirano = 0;
            miktar = 0;
            kdv = 0;
            tutar = 0;
            taksitno = 0;
        }
    }

    [Serializable]
    public class Tahsilat : TNS.HRC.Hakedis
    {
        public new int isinSuresi;
        public int isinSuresiTuru;
        public TeklifFirma firma;
        public TeklifTasinmaz tasinmaz;
        public int tekliftipi;
        public int islemkod;
        public int parcakod;
        public Tahsilat()
        {
            isinSuresi = 0;
            isinSuresiTuru = 0;
            firma = new TeklifFirma();
            tasinmaz = new TeklifTasinmaz();
            tekliftipi = 0;
        }
    }

    [Serializable]
    public class TasinmazOdemePlan : TNS.HRC.HakedisOdemePlan
    {
        public int prmakbuzno;
        public string makbuzno;
        public int prmuhfisno;
        public string muhfisno;
        public TeklifFirma firma;
        public TeklifTasinmaz tasinmaz;
        public int tekliftipi;
        public TasinmazOdemePlan()
        {
            prmakbuzno = 0;
            makbuzno = string.Empty;
            prmuhfisno = 0;
            muhfisno = string.Empty;
            firma = new TeklifFirma();
            tasinmaz = new TeklifTasinmaz();
            tekliftipi = 0;
        }
    }

    [Serializable]
    public class TeklifFirma : TNS.HRC.TeklifFirma
    {
        public double tekliftutari;
        public string ad;

        public TeklifFirma()
        {
            tekliftutari = 0.0;
            ad = string.Empty;
        }
    }

    [Serializable]
    public class KomisyonKarar
    {
        public string kararno;
        public int yil;
        public int muhasebe;
        public string birim;
        public string surecNo;
        public Tasinmaz tasinmaz;
        public int tekliftipi;
        public TeklifFirma uygunfirma1;
        public TeklifFirma uygunfirma2;
        public string aciklama;
        public TNSDateTime karartarihi;
        public KomisyonKarar()
        {
            kararno = string.Empty;
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            tasinmaz = new Tasinmaz();
            tekliftipi = 0;
            uygunfirma1 = new TeklifFirma();
            uygunfirma2 = new TeklifFirma();
            aciklama = string.Empty;
            karartarihi = new TNSDateTime();
        }
    }

    [Serializable]
    public class Teklif : HRC.Teklif
    {
        public new TeklifFirma firma;
        public TeklifTasinmaz tasinmaz;
        public int teklifno;//firmanın verdiği teklif numarası
        public double tutari;//verilen teklifin tutarı
        public TNSDateTime tarihi;

        public Teklif()
        {
            teklifno = 0;
            tutari = 0.0;
            tasinmaz = new TeklifTasinmaz();
            firma = new TeklifFirma();
            tarihi = new TNSDateTime();
        }
    }

    [Serializable]
    public class TeklifTasinmaz
    {
        public Tasinmaz tasinmazBilgisi;
        public double kiralanmaSuresi;
        public int kiralanmaTipi;
        public int yil;
        public int muhasebe;
        public string birim;
        public string surecNo;
        public int tekliftipi;
        public TeklifTasinmaz()
        {
            kiralanmaSuresi = 0.0;
            kiralanmaTipi = 0;
            tasinmazBilgisi = new Tasinmaz();
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            tekliftipi = 0;
        }
    }

    [Serializable]
    public class Tasinmaz
    {
        public int yil;
        public string tasinmazSistemKodu;
        public Bolge bolge;
        public bool bolgeAktif;
        public string ilKod;
        public string ilceKod;
        public string mahalle;
        public string koy;
        public string tasinmazKisaKod;
        public string tasinmazAd;
        public TasinmazCins cins;
        public string paftano;
        public string adano;
        public string parselno;
        public string ciltno;
        public string ciltSayfaNo;
        public string ciltSiraNo;
        public double yuzOlcumuMiktari;
        public double yuzOlcumuOrani;
        public int edinmeSekli;
        public TNSDateTime edinmeTarihi;
        public double edinmeSuresi;
        public TNSDateTime tescilTarihi;
        public int kayittanCikisNedeni;
        public TNSDateTime kayittanCikisTarihi;
        public TNSDateTime arsivTarihi;
        public string arsivNo;
        public string aciklama;
        public string mevki;
        public string enlem;
        public string boylam;
        public string malik;
        public int durum;
        public TNSDateTime kayitTarihi;
        public TNSDateTime islemTarihi;
        public int listeSekli;
        public TNSCollection ekDosyalar;
        public int butceTur;
        public int daireSayisi;
        public int daireAlani;
        public string kullanici;
        public Tasinmaz()
        {
            bolgeAktif = false;
            yil = 0;
            bolge = new Bolge();
            tasinmazSistemKodu = ""; ilKod = string.Empty; ilceKod = string.Empty;
            mahalle = string.Empty; koy = string.Empty; tasinmazKisaKod = string.Empty;
            tasinmazAd = string.Empty; cins = new TasinmazCins();
            paftano = string.Empty; adano = string.Empty;
            parselno = string.Empty; ciltno = string.Empty;
            ciltSayfaNo = string.Empty; ciltSiraNo = string.Empty;
            yuzOlcumuMiktari = 0.0; yuzOlcumuOrani = 0.0;
            edinmeSekli = 0; edinmeTarihi = new TNSDateTime();
            edinmeSuresi = 0.0;
            tescilTarihi = new TNSDateTime(); kayittanCikisNedeni = 0;
            kayittanCikisTarihi = new TNSDateTime(); arsivTarihi = new TNSDateTime();
            arsivNo = string.Empty; aciklama = string.Empty; mevki = string.Empty;
            enlem = string.Empty; boylam = string.Empty; malik = string.Empty; durum = 0;
            islemTarihi = new TNSDateTime();
            kayitTarihi = new TNSDateTime();
            listeSekli = 2;//çoklu
            ekDosyalar = new TNSCollection();
            butceTur = 0;
            daireSayisi = 0;
            daireAlani = 0;
            kullanici = "";
        }
        public Tasinmaz(string kod, int yil)
        {
            this.tasinmazSistemKodu = kod;
            this.yil = yil;
        }
    }

    [Serializable]
    public class TasinmazCins
    {
        public string kod;
        public string ad;
        public TNSCollection detaylar;
        public TasinmazCins()
        {
            kod = string.Empty;
            ad = string.Empty;
            detaylar = new TNSCollection();
        }

        public TasinmazCins(string kod, string ad)
        {
            this.kod = kod;
            this.ad = ad;
        }
    }

    [Serializable]
    public class TasinmazTalep
    {
        public int tasinmazYil;
        public string tasinmazKod;
        public string tasinmazAdi;
        public int firmaKodu;
        public string firmaAd;
        public int siraNo;
        public int yaziNo;
        public TNSDateTime yaziTarihi;
        public string aciklama;
        public int sonucKodu;
        public string sonucAciklama;
        public string sonIslemYapanKodu;
        public string sonIslemYapanAdi;
        public TasinmazTalep()
        {
            tasinmazKod = "";
            tasinmazAdi = string.Empty;
            firmaKodu = 0;
            firmaAd = string.Empty;
            siraNo = 0;
            yaziNo = 0;
            TNSDateTime yaziTarihi = new TNSDateTime(DateTime.Now.ToShortDateString());
            aciklama = string.Empty;
            sonucKodu = 0;
            sonucAciklama = string.Empty;
            sonIslemYapanKodu = string.Empty;
            sonIslemYapanAdi = string.Empty;
        }
    }

    [Serializable]
    public class TasinmazTalepEki
    {
        public int tasinmazYil;
        public string tasinmazKod;
        public string tasinmazAdi;
        public int firmaKodu;
        public string firmaAdi;
        public int talepSiraNo;
        public int ekNo;
        public string aciklama;
        public string dosyaYolu;
        public TasinmazTalepEki()
        {
            tasinmazYil = 0;
            tasinmazKod = "";
            tasinmazAdi = string.Empty;
            firmaKodu = 0;
            firmaAdi = string.Empty;
            talepSiraNo = 0;
            ekNo = 0;
            aciklama = string.Empty;
            dosyaYolu = string.Empty;
        }
    }

    [Serializable()]
    public class IhaleTur : HRC.IhaleTur
    {
        public int tur;
        public IhaleTur()
        {
            tur = 0;
        }
    }

    [Serializable()]
    public class IhaleTurMadde : HRC.IhaleTurMadde
    {
        public int tur;
        public IhaleTurMadde()
        {
            tur = 0;
        }
    }

    [Serializable()]
    public class IhaleIlanTanim : HRC.IhaleIlanTanim
    {
        public int tur;
        public IhaleIlanTanim()
        {
            tur = 0;
        }
    }

    [Serializable]
    public class Surec : HRC.Surec
    {
        //public int teklifturu;
    }

    [Serializable()]
    public class Is : HRC.Is
    {

    }

    [Serializable()]
    public class Parca : HRC.Parca
    {

    }

    [Serializable]
    public class IsParca : HRC.IsParca
    {

    }

    [Serializable]
    public class SurecParca : HRC.SurecParca
    {

    }

    [Serializable()]
    public class SurecParcaDurum : HRC.SurecParcaDurum
    {

    }

    [Serializable]
    public class SurecSifre : HRC.SurecSifreSinif
    {

    }

    [Serializable]
    public class OnayForm : HRC.OnayForm
    {
        //public int gelSurecNo;
        public decimal muhammenBedel;
        public int muhammenBedelPBKod;
        public string geciciTeminatMiktari;
        public int sozlesmeYapilacak;
        public string ihaleUsulAdi;

        public OnayForm()
        {
            muhammenBedel = 0;
            muhammenBedelPBKod = 1;//ytl;       
            geciciTeminatMiktari = string.Empty;
            dokumanBedeli = string.Empty;
            sozlesmeYapilacak = 0;
            ihaleUsulAdi = string.Empty;
        }

    }

    [Serializable()]
    public class OnayFormTertip : HRC.OnayFormTertip
    {

    }

    [Serializable]
    public class IdareBilgi : HRC.IdareBilgi
    {

    }

    [Serializable]
    public class MuhammenBedel
    {
        public int yil;
        public int muhasebe;
        public string birim;
        public string surecNo;
        public ObjectArray satirlar;

        public MuhammenBedel()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            satirlar = new ObjectArray();

        }
    }
    [Serializable]
    public class MuhammenBedelSatir
    {
        public int tasinmazyili;
        public string tasinmazKodu;
        public string tasinmazAdi;
        public TNS.HRC.Is islem;
        public int kiralamaTipi;
        public double kiralamaSuresi;
        public double birimFiyat;
        public double tutar;
        public string aciklama;
        public MuhammenBedelSatir()
        {
            tasinmazyili = 0;
            tasinmazKodu = "";
            tasinmazAdi = string.Empty;
            islem = new TNS.HRC.Is();
            kiralamaTipi = 0;
            kiralamaSuresi = 0;
            birimFiyat = 0.0;
            tutar = 0.0;
            aciklama = string.Empty;
        }

    }

    [Serializable()]
    public class KomisyonUye : HRC.KomisyonUye
    {

    }

    [Serializable()]
    public class Uye : HRC.Uye
    {
        public int siraNo;
        public string tcKimlikNo;
        public Uye()
        {
            siraNo = 0;
            tcKimlikNo = string.Empty;
        }


    }

    [Serializable]
    public class OrnekPersonel
    {
        public int id;
        public string ad;
        public string soyad;
        public string cinsiyet;
        public OrnekPersonel()
        {
            id = 0;
            ad = "";
            soyad = "";
            cinsiyet = "T";
        }
    }

    [Serializable()]
    public class TasinmazIcmal
    {
        public int yil;
        public Tasinmaz tasinmaz;
        public TasinmazDevir gecenYildanDevir;
        public TasinmazDevir yilIcindeEdinilen;
        public TasinmazDevir yilIcindeEldenCikan;
        public TasinmazDevir gelecekYilaDevreden;
        public TasinmazIcmal()
        {
            yil = 0;
            tasinmaz = new Tasinmaz();
            gecenYildanDevir = new TasinmazDevir();
            yilIcindeEdinilen = new TasinmazDevir();
            yilIcindeEldenCikan = new TasinmazDevir();
            gelecekYilaDevreden = new TasinmazDevir();
        }
    }

    [Serializable()]
    public class TasinmazDevir
    {
        public double adet;
        public double yuzolcum;
        public TasinmazDevir()
        {
            adet = 0;
            yuzolcum = 0;
        }
    }

    [Serializable()]
    public class Bolge
    {
        public int kod;
        public string ad;
        public string ad2;
        public TNSCollection iller;

        public Bolge()
        {
            kod = 0;
            ad = "";
            ad2 = "";
            iller = new TNSCollection();
        }

        public Bolge(int bolgeKod)
        {
            kod = bolgeKod;
            ad = "";
            ad2 = "";
            iller = new TNSCollection();
        }

    }

    [Serializable()]
    public class BolgeIl
    {
        public string kod;
        public string ad;

        public BolgeIl()
        {
            kod = "";
            ad = "";

        }
    }

    [Serializable()]
    public class TasinmazEkDosya
    {
        public int tasinmazYil;
        public string tasinmazKod;
        public int siraNo;
        public string aciklama;
        public string dosyaYolu;
        public TasinmazEkDosya()
        {
            tasinmazYil = 0;
            tasinmazKod = "";
            siraNo = 0;
            aciklama = "";
            dosyaYolu = "";
        }
    }

}
