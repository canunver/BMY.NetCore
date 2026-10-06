using System;
using System.Collections.Generic;
using System.Text;
using Mernis.KPS.Types.KPSWse;
using Mernis.KPSv2.Types.KimlikNoSorgulaAdresServis;
using Mernis.KPSv2.Types.BilesikKutukSorgulaKimlikNoServis;

namespace TNS.KPM
{
    [Serializable]
    public class Il
    {
        public int kod = 0;
        public string ad = string.Empty;

        public Il()
        {
        }

        public Il(IlBilgisi ilB)
        {
            this.kod = ilB.Kod;
            this.ad = ilB.Ad;
        }
    }

    [Serializable]
    public class Ilce
    {
        public Il il = new Il();
        public int kod = 0;
        public string ad = string.Empty;

        public Ilce()
        {
        }

        public Ilce(IlceBilgisi ilceB)
        {
            this.il.kod = ilceB.IlKod;
            this.kod = ilceB.Kod;
            this.ad = ilceB.Ad;
        }
    }

    //[Serializable]
    //public class YAG_Tarih
    //{
    //    int yil = 0;
    //    int ay = 0;
    //    int gun = 0;

    //    public TNSDateTime TNSTarih
    //    {
    //        get
    //        {
    //            try
    //            {
    //                return new TNSDateTime(new DateTime(yil, ay, gun));
    //            }
    //            catch
    //            {
    //                return new TNSDateTime();
    //            }
    //        }
    //    }

    //    public void Yaz(int? yil, int? ay, int? gun)
    //    {
    //        Ata(ref this.yil, yil);
    //        Ata(ref this.ay, ay);
    //        Ata(ref this.gun, gun);
    //    }

    //    private void Ata(ref int a, int? b)
    //    {
    //        if (b == null) a = 0;
    //        else a = b.Value;
    //    }

    //    internal void Yaz(TNSDateTime value)
    //    {
    //        if (value == null)
    //        {
    //            yil = 0;
    //            ay = 0;
    //            gun = 0;
    //        }
    //        else
    //        {
    //            yil = value.Yil;
    //            ay = value.Ay;
    //            gun = value.Gun;
    //        }
    //    }
    //}

    [Serializable]
    public class Kisi
    {
        //Kişisel Bilgileri
        public string tcKimlikNo = string.Empty;
        public string ad = string.Empty;
        public string soyad = string.Empty;
        public string babaAd = string.Empty;
        public string anaAd = string.Empty;
        public string dogumYeri = string.Empty;
        //public YAG_Tarih dogumTarihiMer = new YAG_Tarih();
        public TNSDateTime dogumTarihi = new TNSDateTime(); //  { get { return dogumTarihiMer.TNSTarih; } }
        public int cinsiyet = 0;
        public int medeniHal = 0;
        public int din = 0;
        public string cinsiyetAciklama = "";
        public string medeniHalAciklama = "";
        public string dinAciklama = "";
        public string kanGrubu = string.Empty;
        public string kizlikSoyad = string.Empty;
        public KayitliOlduguYer kayitliOlduguYer = new KayitliOlduguYer();
        public NufusCuzdani nufusCuzdani = new NufusCuzdani();
        public Adres adres = new Adres();
        public ObjectArray aileFertleri = new ObjectArray();

        public Kisi()
        {
        }

        public Kisi(BilesikKutukBilgileri kb)
        {
            this.tcKimlikNo = kb.KimlikNo.ToString();
            KisiTemelBilgileriniAta(kb.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi);
            KisiDigerBilgileriniAta(kb);

            if (kb.TCVatandasiKisiKutukleri.NufusCuzdaniBilgisi != null)
            {
                nufusCuzdani.NufusCuzdaniBilgileriniAta(kb.TCVatandasiKisiKutukleri.NufusCuzdaniBilgisi);
            }
        }

        public Kisi(BilesikKutukBilgileri kb, KimlikNoileKisiAdresBilgileri kab)
        {
            if (kb != null)
            {
                this.tcKimlikNo = kb.KimlikNo.ToString();
                KisiTemelBilgileriniAta(kb.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi);
                KisiDigerBilgileriniAta(kb);
            }

            if (kab != null)
            {
                this.tcKimlikNo = kab.KimlikNo.ToString();
                adres.AdresBilgileriniAta(kab);
            }
        }

        private void KisiTemelBilgileriniAta(TCKisiTemelBilgisi ktb)
        {
            this.ad = ktb.Ad;
            this.soyad = ktb.Soyad;
            this.babaAd = ktb.BabaAd;
            this.anaAd = ktb.AnneAd;
            this.dogumYeri = ktb.DogumYer;
            Arac.TarihYaz(ktb.DogumTarih, this.dogumTarihi);
        }

        private void KisiDigerBilgileriniAta(BilesikKutukBilgileri kb)
        {
            this.cinsiyet = kb.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet != null ? (kb.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet.Kod.HasValue ? kb.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet.Kod.Value : 0) : 0;
            this.medeniHal = kb.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal != null ? (kb.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal.Kod.HasValue ? kb.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal.Kod.Value : 0) : 0;
            this.din = kb.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.Din != null ? (kb.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.Din.Kod.HasValue ? kb.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.Din.Kod.Value : 0) : 0;

            if (kb.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet != null && kb.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet.Kod.HasValue)
                this.cinsiyetAciklama = kb.TCVatandasiKisiKutukleri.KisiBilgisi.TemelBilgisi.Cinsiyet.Aciklama;
            if (kb.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal != null && kb.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal.Kod.HasValue)
                this.medeniHalAciklama = kb.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.MedeniHal.Aciklama;
            if (kb.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.Din != null && kb.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.Din.Kod.HasValue)
                this.dinAciklama = kb.TCVatandasiKisiKutukleri.KisiBilgisi.DurumBilgisi.Din.Aciklama;

            kayitliOlduguYer.KayitliOlduguYerBilgileriniAta(kb.TCVatandasiKisiKutukleri.KisiBilgisi.KayitYeriBilgisi);
        }
    }

    [Serializable]
    public class Adres
    {
        //Nüfus Cüzdanı Bilgileri
        public string il = string.Empty;
        public string ilce = string.Empty;
        public string bucak = string.Empty;
        public string mahalle = string.Empty;
        public string koy = string.Empty;
        public string sokak = string.Empty;
        public string numara = string.Empty;

        public Adres()
        {
        }

        public void AdresBilgileriniAta(KisiAdresBilgisiC adres)
        {
            if (adres.YerlesimYeriAdresi.Tip == AdresTipi.BeldeAdresi)
                BeldeAdresBilgileriniAta(adres.YerlesimYeriAdresi.BeldeAdresi);
            else if (adres.YerlesimYeriAdresi.Tip == AdresTipi.IlIlceMerkeziAdresi)
                IlIlceMerkeziAdresBilgileriniAta(adres.YerlesimYeriAdresi.IlIlceMerkeziAdresi);
            else if (adres.YerlesimYeriAdresi.Tip == AdresTipi.KoyAdresi)
                KoyAdresBilgileriniAta(adres.YerlesimYeriAdresi.KoyAdresi);
            else if (adres.YerlesimYeriAdresi.Tip == AdresTipi.YurtDisiAdresi)
                YurtDisiAdresBilgileriniAta(adres.YerlesimYeriAdresi.YurtDisiAdresi);
        }

        public void AdresBilgileriniAta(KimlikNoileKisiAdresBilgileri adres)
        {
            if (adres.YerlesimYeriAdresi.AdresTip != null && adres.YerlesimYeriAdresi.AdresTip.Kod.HasValue && adres.YerlesimYeriAdresi.AdresTip.Kod.Value == (int)AdresTipi.BeldeAdresi)
                BeldeAdresBilgileriniAta(adres.YerlesimYeriAdresi.BeldeAdresi);
            if (adres.YerlesimYeriAdresi.AdresTip != null && adres.YerlesimYeriAdresi.AdresTip.Kod.HasValue && adres.YerlesimYeriAdresi.AdresTip.Kod.Value == (int)AdresTipi.IlIlceMerkeziAdresi)
                IlIlceMerkeziAdresBilgileriniAta(adres.YerlesimYeriAdresi.IlIlceMerkezAdresi);
            else if (adres.YerlesimYeriAdresi.AdresTip != null && adres.YerlesimYeriAdresi.AdresTip.Kod.HasValue && adres.YerlesimYeriAdresi.AdresTip.Kod.Value == (int)AdresTipi.KoyAdresi)
                KoyAdresBilgileriniAta(adres.YerlesimYeriAdresi.KoyAdresi);
            else if (adres.YerlesimYeriAdresi.AdresTip != null && adres.YerlesimYeriAdresi.AdresTip.Kod.HasValue && adres.YerlesimYeriAdresi.AdresTip.Kod.Value == (int)AdresTipi.YurtDisiAdresi)
                YurtDisiAdresBilgileriniAta(adres.YerlesimYeriAdresi.YurtDisiAdresi);
        }

        private void YurtDisiAdresBilgileriniAta(KimlikNoileYurtDisiAdresi adres)
        {
            this.il = adres.YabanciUlke.Aciklama;
            this.ilce = adres.YabanciSehir;
        }

        private void KoyAdresBilgileriniAta(KimlikNoileKoyAdresi adres)
        {
            this.il = adres.Il;
            this.ilce = adres.Ilce;
            this.bucak = "";// adres.Bucak;
            this.mahalle = adres.Mahalle;
            this.sokak = adres.Csbm;
            this.numara = adres.DisKapiNo + "/" + adres.IcKapiNo;
            this.koy = adres.Koy;
        }

        private void IlIlceMerkeziAdresBilgileriniAta(IlIlceMerkeziAdresBilgisi adres)
        {
            this.il = adres.Il;
            this.ilce = adres.Ilce;
            this.mahalle = adres.Mahalle;
            this.sokak = adres.Csbm;
            this.numara = adres.DisKapiNo + "/" + adres.IcKapiNo;
        }

        private void BeldeAdresBilgileriniAta(KimlikNoileBeldeAdresi adres)
        {
            this.il = adres.Il;
            this.ilce = adres.Ilce;
            this.bucak = "";// adres.Bucak;
            this.mahalle = adres.Mahalle;
            this.sokak = adres.Csbm;
            this.numara = adres.DisKapiNo + "/" + adres.IcKapiNo;
        }

        private void BeldeAdresBilgileriniAta(BeldeAdresBilgisi adres)
        {
            this.il = adres.Il;
            this.ilce = adres.Ilce;
            this.bucak = "";// adres.Bucak;
            try { this.bucak = adres.Bucak; } catch { }
            this.mahalle = adres.Mahalle;
            this.sokak = adres.Csbm;
            this.numara = adres.DisKapiNo + "/" + adres.IcKapiNo;
        }

        private void IlIlceMerkeziAdresBilgileriniAta(KimlikNoileIlIlceMerkezi adres)
        {
            this.il = adres.Il;
            this.ilce = adres.Ilce;
            this.mahalle = adres.Mahalle;
            this.sokak = adres.Csbm;
            this.numara = adres.DisKapiNo + "/" + adres.IcKapiNo;
        }

        private void KoyAdresBilgileriniAta(KoyAdresBilgisi adres)
        {
            this.il = adres.Il;
            this.ilce = adres.Ilce;
            this.bucak = "";// adres.Bucak;
            try { this.bucak = adres.Bucak; } catch { }
            this.mahalle = adres.Mahalle;
            this.sokak = adres.Csbm;
            this.numara = adres.DisKapiNo + "/" + adres.IcKapiNo;
            this.koy = adres.Koy;
        }

        private void YurtDisiAdresBilgileriniAta(YurtDisiAdresBilgisi adres)
        {
            this.il = adres.Ulke;
            this.ilce = adres.Sehir;
        }
    }

    [Serializable]
    public class NufusCuzdani
    {
        //Nüfus Cüzdanı Bilgileri
        public string nufusCuzdanSeri = string.Empty;
        public string nufusCuzdanNo = string.Empty;
        public string verildigiYer = string.Empty;
        public int verilisNedeni = 0;
        public string verilisNedeniAciklama = string.Empty;
        public string kayitNo = string.Empty;
        //public YAG_Tarih verilisTarihiMer = new YAG_Tarih();
        public TNSDateTime verilisTarihi = new TNSDateTime(); //{ get { return verilisTarihiMer.TNSTarih; } set { verilisTarihiMer.Yaz(value); } }

        public NufusCuzdani()
        {
        }

        public void NufusCuzdaniBilgileriniAta(TCCuzdanBilgisi nc)
        {
            this.nufusCuzdanSeri = nc.Seri != null ? nc.Seri : "";
            this.nufusCuzdanNo = nc.No.ToString();
            this.verildigiYer = nc.VerildigiIlce != null ? nc.VerildigiIlce.Aciklama : "";
            this.kayitNo = nc.CuzdanKayitNo != null ? nc.CuzdanKayitNo.ToString() : "";

            Arac.TarihYaz(nc.VerilmeTarih, this.verilisTarihi);

            this.verilisNedeni = nc.CuzdanVerilmeNeden != null ? (nc.CuzdanVerilmeNeden.Kod.HasValue ? nc.CuzdanVerilmeNeden.Kod.Value : 0) : 0;

            if (nc.CuzdanVerilmeNeden != null)
                this.verilisNedeniAciklama = nc.CuzdanVerilmeNeden.Aciklama;
        }
    }

    [Serializable]
    public class AileFerdi
    {
        //Nüfus Aile Ferdi Bilgileri
        public int yakinlikDerecesi = 0;
        public string tcKimlikNo = string.Empty;
        public string ad = string.Empty;
        public string soyad = string.Empty;
        public string babaAd = string.Empty;
        public string anaAd = string.Empty;
        public string dogumYeri = string.Empty;
        public int medeniHal = 0;

        //private YAG_Tarih dogumTarihiMer = new YAG_Tarih();
        public TNSDateTime dogumTarihi = new TNSDateTime(); //{ get { return dogumTarihiMer.TNSTarih; } set { dogumTarihiMer.Yaz(value); } }

        //private YAG_Tarih tescilTarihiMer = new YAG_Tarih();
        public TNSDateTime tescilTarihi = new TNSDateTime(); // { get { return tescilTarihiMer.TNSTarih; } set { tescilTarihiMer.Yaz(value); } }

        //private YAG_Tarih olumTarihiMer = new YAG_Tarih();
        public TNSDateTime olumTarihi = new TNSDateTime(); //{ get { return olumTarihiMer.TNSTarih; } set { olumTarihiMer.Yaz(value); } }

        //private YAG_Tarih evlenmeTarihiMer = new YAG_Tarih();
        public TNSDateTime evlenmeTarihi = new TNSDateTime(); //{ get { return evlenmeTarihiMer.TNSTarih; } set { evlenmeTarihiMer.Yaz(value); } }

        //private YAG_Tarih bosanmaTarihiMer = new YAG_Tarih();
        public TNSDateTime bosanmaTarihi = new TNSDateTime(); //{ get { return bosanmaTarihiMer.TNSTarih; } set { bosanmaTarihiMer.Yaz(value); } }

        public AileFerdi()
        {
        }

        public void NufusAileKayitBilgileriniAta(NufusKayitOrnegiC nko)
        {
            if (nko != null)
            {
                //NKOKisiBilgisi kb = nko.KisiListesi[0];

                //YakinlikDerecesiAta(kb.Yakinlik); //this.yakinlikDerecesi
                //this.tcKimlikNo = kb.TCKimlikNo.ToString();
                //this.ad = kb.TemelBilgisi.Ad;
                //this.soyad = kb.TemelBilgisi.Soyad;
                //this.babaAd = kb.TemelBilgisi.BabaAd;
                //this.anaAd = kb.TemelBilgisi.AnaAd;
                //this.dogumYeri = kb.TemelBilgisi.DogumYer;
                //this.dogumTarihi.Yaz(kb.TemelBilgisi.DogumTarih.Gun + "-" + kb.TemelBilgisi.DogumTarih.Ay + "-" + kb.TemelBilgisi.DogumTarih.Yil);
                //MedeniHalAta(kb.DurumBilgisi.MedeniHal); //this.medeniHal = kb.DurumBilgisi.MedeniHal;
                //this.tescilTarihi.Yaz(kb.OlayTarihBilgisi.TescilTarih.Gun + "-" + kb.OlayTarihBilgisi.TescilTarih.Ay + "-" + kb.OlayTarihBilgisi.TescilTarih.Yil);
                //this.olumTarihi.Yaz(kb.DurumBilgisi.OlumTarih.Gun + "-" + kb.DurumBilgisi.OlumTarih.Ay + "-" + kb.DurumBilgisi.OlumTarih.Yil);
                //this.evlenmeTarihi.Yaz(kb.OlayTarihBilgisi.EvlenmeTarih.Gun + "-" + kb.OlayTarihBilgisi.EvlenmeTarih.Ay + "-" + kb.OlayTarihBilgisi.EvlenmeTarih.Yil);
                //this.bosanmaTarihi.Yaz(kb.OlayTarihBilgisi.BosanmaTarih.Gun + "-" + kb.OlayTarihBilgisi.BosanmaTarih.Ay + "-" + kb.OlayTarihBilgisi.BosanmaTarih.Yil);
            }
        }
    }

    [Serializable]
    public class KayitliOlduguYer
    {
        //Kayıtlı Olduğu Yer Bilgileri
        public Ilce ilce = new Ilce();
        public int ciltNo = 0;
        public int aileSiraNo = 0;
        public int bireySiraNo = 0;
        public string mahalleKoy = string.Empty;

        public KayitliOlduguYer()
        {
        }

        public void KayitliOlduguYerBilgileriniAta(TCKisiKayitYeriBilgisi kkyb)
        {
            if (kkyb.Il != null && kkyb.Il.Kod.HasValue)
            {
                this.ilce.il.kod = kkyb.Il.Kod.Value;
                this.ilce.il.ad = kkyb.Il.Aciklama;
            }
            if (kkyb.Ilce != null && kkyb.Ilce.Kod.HasValue)
            {
                this.ilce.kod = kkyb.Ilce.Kod.Value;
                this.ilce.ad = kkyb.Ilce.Aciklama;
            }
            if (kkyb.Cilt != null && kkyb.Cilt.Kod.HasValue)
            {
                this.mahalleKoy = kkyb.Cilt.Aciklama;
                this.ciltNo = kkyb.Cilt.Kod.Value;
            }
            if (kkyb.AileSiraNo != null && kkyb.AileSiraNo.HasValue)
                this.aileSiraNo = kkyb.AileSiraNo.Value;
            if (kkyb.BireySiraNo != null && kkyb.BireySiraNo.HasValue)
                this.bireySiraNo = kkyb.BireySiraNo.Value;
        }
    }

    public enum ENUMIstekTur : int
    {
        TANIMSIZ = 0,
        IKAMETGAH = 1,
        NUFUSCUZDANORNEK = 2,
        VUKUATLINUFUSKAYITORNEK = 3,
        SADECEADRES = 4,
    }

    [Serializable]
    public class Istek
    {
        public int istekNo;
        public int istekTur;
        public string istekTurAd;
        public string tcKimlikNo;
        public string adSoyad;
        public string islemYapan;
        public TNSDateTime dogumTarih;
        public TNSDateTime islemTarih;
        public TNSDateTime islemTarih2;
        public string sorguIslemYapan;
        public bool entegrasyon;
        public string ipAdres;

        public Istek()
        {
            istekNo = 0;
            istekTur = (int)ENUMIstekTur.TANIMSIZ;
            istekTurAd = string.Empty;
            tcKimlikNo = string.Empty;
            adSoyad = string.Empty;
            islemYapan = string.Empty;
            islemTarih = new TNSDateTime();
            islemTarih2 = new TNSDateTime();
            dogumTarih = new TNSDateTime();
            sorguIslemYapan = string.Empty;
            entegrasyon = false;
            ipAdres = "";
        }

        public static int IstekTurDondur(string istek)
        {
            int donusDeger = (int)ENUMIstekTur.TANIMSIZ;
            if (istek == IstekTurAdiDondur((int)ENUMIstekTur.IKAMETGAH))
                donusDeger = (int)ENUMIstekTur.IKAMETGAH;
            else if (istek == IstekTurAdiDondur((int)ENUMIstekTur.NUFUSCUZDANORNEK))
                donusDeger = (int)ENUMIstekTur.NUFUSCUZDANORNEK;
            else if (istek == IstekTurAdiDondur((int)ENUMIstekTur.VUKUATLINUFUSKAYITORNEK))
                donusDeger = (int)ENUMIstekTur.VUKUATLINUFUSKAYITORNEK;
            else if (istek == IstekTurAdiDondur((int)ENUMIstekTur.SADECEADRES))
                donusDeger = (int)ENUMIstekTur.SADECEADRES;
            return donusDeger;
        }

        public static string IstekTurAdiDondur(int i)
        {
            string donusDeger = string.Empty;
            if (i == (int)ENUMIstekTur.IKAMETGAH)
                donusDeger = "İkametgah İl Muhaberi";
            else if (i == (int)ENUMIstekTur.NUFUSCUZDANORNEK)
                donusDeger = "Nüfus Cüzdanı Örneği";
            else if (i == (int)ENUMIstekTur.VUKUATLINUFUSKAYITORNEK)
                donusDeger = "Vukuatlı Nüfus Kağıt Örneği";
            else if (i == (int)ENUMIstekTur.SADECEADRES)
                donusDeger = "Sadece Adres";
            return donusDeger;
        }

        public void GecerliMi(Sonuc sonuc)
        {
            if (istekTur <= 0)
                sonuc.DegerAta("İstek türü boş bırakılamaz.<br>");

            string kontrol = "";

            if (!DEG.Arac.ManasKullaniyor())
            {
                kontrol = KPM.Arac.TCNoDogruMu(tcKimlikNo);
                if (!string.IsNullOrWhiteSpace(kontrol))
                    sonuc.DegerAta(kontrol);
            }

            if (string.IsNullOrEmpty(islemYapan))
                sonuc.DegerAta("İşlem yapan boş bırakılamaz.<br>");
        }
    }
}