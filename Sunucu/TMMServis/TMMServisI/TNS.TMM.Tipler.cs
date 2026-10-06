using System;
using System.Collections.Generic;

namespace TNS.TMM
{
    /// <summary>
    ///  Taşınır hesap planı bazında miktar ve ortalama fiyat bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class HesapPlaniOrtalamaFiyatMiktar
    {
        /// <summary>
        /// Taşınır hesap planı bilgilerini tutan nesne
        /// </summary>
        public HesapPlaniSatir hesapPlan;

        /// <summary>
        /// İlgili malzemenin toplam miktarı
        /// </summary>
        public double miktar;

        /// <summary>
        /// İlgili malzemenin ortalama fiyatı
        /// </summary>
        public decimal ortFiyat;

        /// <summary>
        /// Parametresiz HesapPlaniOrtalamaFiyatMiktar nesnesi oluşturucu yordam
        /// </summary>
        public HesapPlaniOrtalamaFiyatMiktar()
        {
            hesapPlan = new HesapPlaniSatir();
            miktar = 0;
            ortFiyat = 0;
        }

        /// <summary>
        /// Hesap planı bilgilerini tutan hps parametresini alan HesapPlaniOrtalamaFiyatMiktar nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="hps">Taşınır hesap planı bilgilerini tutan nesne</param>
        public HesapPlaniOrtalamaFiyatMiktar(HesapPlaniSatir hps)
        {
            hesapPlan = hps;
            miktar = 0;
            ortFiyat = 0;
        }
    }

    /// <summary>
    /// Taşınır malzemelerinin amortisman raporuna ait amortisman ve değerleme bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class AmortismanRapor
    {
        public int yil { get; set; }
        public int prSicilNo { get; set; }
        public int amortismanTur { get; set; }
        /// <summary>
        /// Malzemenin amortisman işlemlerinin başlama yılı
        /// </summary>
        public int amortismanBaslamaYil { get; set; }

        /// <summary>
        /// Malzemenin taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod { get; set; }

        /// <summary>
        /// Malzemenin taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd { get; set; }

        public int amortismanSuresi { get; set; }
        /// <summary>
        /// Malzemenin sicil numarası
        /// </summary>
        public string gorSicilNo { get; set; }

        /// <summary>
        /// BİS
        /// </summary>
        public string bisNo { get; set; }

        /// <summary>
        /// Eski Sici lNo
        /// </summary>
        public string eskiSicilNo { get; set; }

        /// <summary>
        /// Eski BİS No
        /// </summary>
        public string eskiBisNo { get; set; }

        public string ozellik { get; set; }

        /// <summary>
        /// Malzemenin birim fiyatı
        /// </summary>
        public decimal maliyetTutar { get; set; }

        /// <summary>
        /// Malzemeye değerleme işleminden kaynaklı eklenen fiyat
        /// </summary>
        public decimal degerlemeTutar { get { return degerArtisTutar; } }

        /// <summary>
        /// Malzemenin birim fiyatı üzerinden uygulanan amortisman miktarları toplamı
        /// </summary>
        public decimal maliyetAmortismanBirikmisTutar { get; set; }

        /// <summary>
        /// Malzemenin değerleme fiyatı üzerinden uygulanan amortisman miktarları toplamı
        /// </summary>
        public decimal degerlemeAmortismanBirikmisTutar { get; set; }

        /// <summary>
        /// Cari yıla ait malzemenin birim fiyatı üzerinden uygulanan amortisman miktarı
        /// </summary>
        public decimal cariMaliyetAmortismanTutar { get; set; }

        public decimal cariMaliyetAmortismanTutarTTK { get; set; }

        /// <summary>
        /// Cari yıla ait malzemenin değerleme fiyatı üzerinden uygulanan amortisman miktarı
        /// </summary>
        public decimal cariDegerlemeAmortismanTutar { get; set; }

        public decimal cariToplamAmortismanTutar { get { return cariMaliyetAmortismanTutar + cariDegerlemeAmortismanTutar; } }

        /// <summary>
        /// Malzemeye ait amortisman uygulanacak kalan tutar
        /// </summary>
        public decimal kalanMaliyetTutar { get { return maliyetTutar - maliyetAmortismanBirikmisTutar; } }

        /// <summary>
        /// Malzemeye ait amortisman uygulanacak kalan tutar
        /// </summary>
        public decimal kalanDegerlemeTutar { get { return degerlemeTutar - degerlemeAmortismanBirikmisTutar; } }

        /// <summary>
        /// Malzemenin birim fiyatına ait amortisman tutarı ile değerleme amortisman tutarının toplamı
        /// </summary>
        public decimal toplamAmortismanBirikmisTutar { get { return maliyetAmortismanBirikmisTutar + degerlemeAmortismanBirikmisTutar; } }

        /// <summary>
        /// Malzemenin birim fiyatı ile değerleme işleminden kaynaklı eklenen fiyat toplamı
        /// </summary>
        public decimal toplamAmortismanTutar { get { return toplamAmortismanBirikmisTutar + cariToplamAmortismanTutar; } }

        /// <summary>
        /// Malzemenin birim fiyatı ile değerleme işleminden kaynaklı eklenen fiyat toplamı
        /// </summary>
        public decimal toplamTutar { get { return maliyetTutar + degerlemeTutar; } }

        /// <summary>
        /// Malzemeye ait amortisman uygulanacak kalan tutar
        /// </summary>
        public decimal kalanTutar { get { return toplamTutar - toplamAmortismanTutar; } }

        public decimal kalanTutarEnflasyon { get { return (maliyetTutar + enflasyonDegerArtisTutar) - maliyetAmortismanBirikmisTutar; } }

        /// <summary>
        /// Malzemeye ait kalan amortisman süresi
        /// </summary>
        public int kalanAmortismanSure = 0;

        public int kdvOran = 0;

        public ObjectArray degerArtisAmortisman = new ObjectArray();

        public int kistAmortisman = 0;
        public int amortismanDurum = 0;
        public string islemDurumAciklama;
        public string girisFisNo;
        public string cikisFisNo;
        public string disSicilNo;
        public string disSicilNo2;
        public string markaAd;
        public string modelAd;
        public int hesapPlanAdet = 0;
        public IslemTip islemTip = new IslemTip();

        /// <summary>
        /// Parametresiz AmortismanRapor nesnesi oluşturucu yordam
        /// </summary>
        public AmortismanRapor()
        {
        }

        public TNSDateTime girisTarih { get; set; }

        public int islemTipiKod { get; set; }
        public int islemTuruKod { get; set; }

        public TNSDateTime sonGirisTarih { get; set; }

        public TNSDateTime cikisTarih { get; set; }

        public bool SonYilMi { get { if (this.amortismanBaslamaYil + this.amortismanSuresi - 1 <= this.yil) return true; else return false; } }

        public string harcamaBirimiKod { get; set; }

        public string harcamaBirimiAdi { get; set; }

        public string birimKod { get { return this.harcamaBirimiKod.Substring(8, 3); } }

        public string kurKod { get { return this.harcamaBirimiKod.Substring(0, 8); } }

        public string birimKurkod { get { return birimKod + "." + kurKod; } }

        public string saymanlikKod { get; set; }

        public string ambarKod { get; set; }

        public int sonGirisNo { get; set; }

        public int cikisNo { get; set; }

        public string kod { get; set; }

        public int donem { get; set; }

        public string degerArtisKod { get; set; }

        public decimal degerArtisTutar
        {
            get
            {
                decimal toplam = 0;
                foreach (AmortismanRaporDegerArtis ar in degerArtisAmortisman.objeler)
                    toplam += ar.maliyetTutar;
                return toplam;
            }
        }

        public decimal enflasyonDegerArtisTutar
        {
            get
            {
                decimal toplam = 0;
                foreach (AmortismanRaporDegerArtis ar in degerArtisAmortisman.objeler)
                {
                    if (ar.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE)
                        toplam += ar.maliyetTutar;
                }
                return toplam;
            }
        }

        //public decimal degerArtisKalanTutar
        //{
        //    get
        //    {
        //        decimal toplam = 0;
        //        foreach (AmortismanRaporDegerArtis ar in degerArtisAmortisman.objeler)
        //            toplam += ar.kalanTutar;
        //        return toplam;
        //    }
        //}

        public decimal degerArtisAmortismanTutar { get; set; }
        public decimal enflasyonDuzAmortismanTutar { get; set; }

        public string nereyeGitti { get; set; }
        public string nereyeGittiAd { get; set; }

        public string faturaNo { get; set; }
        public TNSDateTime faturaTarih { get; set; }
        public string neredenGeldi { get; set; }


        //EnflasyonDuzeltmesi
        public decimal enfKatsayisi { get; set; }
        public decimal kiymetBedeli { get; set; }
        public decimal kiymetBedeliCariEdf { get; set; }
        public decimal kiymetBedeliBirikmisEdf { get; set; }

        public decimal kiymetBedeliToplamEdf { get { return kiymetBedeliCariEdf + kiymetBedeliBirikmisEdf; } }
        public decimal kiymetBedeliToplam { get { return kiymetBedeli + kiymetBedeliToplamEdf; } }


        public decimal amortisman { get; set; }
        public decimal amortismanCariEdf { get; set; }
        public decimal amortismanBirikmisEdf { get; set; }

        public decimal amortismanToplamEdf { get { return amortismanCariEdf + amortismanBirikmisEdf; } }
        public decimal amortismanToplam { get { return amortisman + amortismanToplamEdf; } }



        public decimal edTutar { get; set; }
        public decimal edTutarEnflasyonDuzeltmesi { get; set; }
        public decimal edTutarEnflasyonDuzeltmesiSon { get; set; }
        public decimal edTutarToplam { get { return edTutar + edTutarEnflasyonDuzeltmesi; } }
        public decimal edTutarToplamSon { get { return edTutar + edTutarEnflasyonDuzeltmesi + edTutarEnflasyonDuzeltmesiSon; } }
        public decimal edAmortisman { get; set; }
        public decimal edAmortismanEnflasyonDuzeltmesi { get; set; }
        public decimal edAmortismanEnflasyonDuzeltmesiSon { get; set; }
        public decimal edAmortismanToplam { get { return edAmortisman + edAmortismanEnflasyonDuzeltmesi; } }
        public decimal edAmortismanToplamSon { get { return edAmortisman + edAmortismanEnflasyonDuzeltmesi + edAmortismanEnflasyonDuzeltmesiSon; } }

        public string girisTarihStr { get { return !girisTarih.isNull ? girisTarih.Oku().ToString("ddMMyyyy") : ""; } }
    }


    [Serializable()]
    public class AmortismanRaporDegerArtis
    {
        public decimal maliyetTutar { get; set; }
        public int amortismanTur { get; set; }
        public decimal cariDegerlemeAmortismanTutar { get; set; }
        public int prSicilNo { get; set; }
        public string gorSicilNo { get; set; }
        public string degerArtisKod { get; set; }
    }


    [Serializable()]
    public class AmortismanRapor3
    {
        public ObjectArray degerDuzeltmeAmortismanX = new ObjectArray();

        public string saymanlikKod { get; set; }
        public string harcamaBirimiKod { get; set; }
        public string harcamaBirimiAdi { get; set; }
        public string ambarKod { get; set; }
        public string hesapPlanKod { get; set; }
        public string hesapPlanAd { get; set; }
        public int yil { get; set; }
        public int donem { get; set; }
        public int prSicilNo { get; set; }
        public int amortismanTur { get; set; }
        public int amortismanBaslamaYil { get; set; }
        public int amortismanBaslamaDonem { get; set; }
        public int amortismanSuresi { get; set; }
        public bool SonYilMi { get { return (this.amortismanBaslamaYil + this.amortismanSuresi - 1 <= this.yil); } }
        public bool SonDonemMi { get { return (this.SonYilMi && this.donem == 12 && this.amortismanKaydet); } }


        public bool amortismanSiniriAltinda { get; set; }
        public bool degerArtisAmortismanYapilacak { get; set; }
        public int donemAyIslemYapilan { get; set; }


        public string gorSicilNo { get; set; }
        public int islemTipiKod { get; set; }
        public int islemTuruKod { get; set; }
        public int kistAmortisman = 0;
        public TNSDateTime girisTarih { get; set; }
        public TNSDateTime sonGirisTarih { get; set; }
        public TNSDateTime cikisTarih { get; set; }
        public int sonGirisNo { get; set; }
        public int cikisNo { get; set; }
        public string girisFisNo;
        public string cikisFisNo;
        public string islemDurumAciklama;
        public string nereyeGitti { get; set; }
        public string nereyeGittiAd { get; set; }
        public string maliyetMerkezi { get; set; }

        public int amortismanDurum = 0;
        public string bisNo { get; set; }
        public string eskiSicilNo { get; set; }
        public string eskiBisNo { get; set; }
        public string degerArtisKod { get; set; }
        public bool amortismanKaydet { get; set; }


        public decimal ilkBedel { get; set; }

        public decimal maliyetTutar { get; set; }
        public decimal maliyetCariAmortisman { get; set; }
        public decimal maliyetCariAmortismanEDFli { get; set; }
        public decimal maliyetCariAmortismanTest
        {
            get
            {

                //decimal maliyetCariAmortisman = 0;
                //if (this.amortismanTur == (int)ENUMAmortismanTur.NORMAL)
                //{

                //    decimal maliyetKalanAmortismanEnflasyon = 0;
                //    decimal maliyetTutarEnflasyon = 0;

                //    foreach (AmortismanRapor3 ar in degerDuzeltmeAmortisman.objeler)
                //    {
                //        if (ar.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE)
                //        {
                //            maliyetTutarEnflasyon += ar.maliyetTutar;
                //            maliyetKalanAmortismanEnflasyon += ar.maliyetKalanAmortisman;
                //        }
                //    }

                //    if (this.SonDonemMi || this.amortismanSiniriAltinda)
                //        maliyetCariAmortisman += (this.maliyetKalanAmortisman + maliyetKalanAmortismanEnflasyon);
                //    else
                //    {
                //        this.maliyetCariAmortismanKuruslu = ((this.maliyetTutar + maliyetTutarEnflasyon) * this.donemAyIslemYapilan) / (this.amortismanSuresi * 12);
                //        maliyetCariAmortisman = (decimal)OrtakFonksiyonlar.KurusYap((double)this.maliyetCariAmortismanKuruslu);
                //    }

                //}
                //else if (this.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME)
                //{
                //    if (amortismanKaydet && this.degerArtisAmortismanYapilacak)
                //    {
                //        if (this.SonDonemMi)
                //            maliyetCariAmortisman = this.maliyetKalanAmortisman;
                //        else
                //            maliyetCariAmortisman = (decimal)OrtakFonksiyonlar.KurusYap(((double)this.maliyetTutar * this.donemAyIslemYapilan) / (this.amortismanSuresi * 12));
                //    }
                //}

                return maliyetCariAmortisman;
            }
        }

        public decimal maliyetBirikmisAmortisman { get; set; }
        public decimal maliyetToplamAmortisman { get { return this.amortismanKaydet ? maliyetBirikmisAmortisman : (maliyetCariAmortisman + maliyetBirikmisAmortisman); } }
        public decimal maliyetKalanAmortisman { get { return OrtakFonksiyonlar.KurusYap(maliyetTutar - maliyetToplamAmortisman); } }

        //19.09.2024 tarihinde Merkez bankasından gelen talep üzerine EDF tutarları ayrı amortisman yapılarak kayıt alında alınacak. H.Ö.
        //public decimal maliyetKalanAmortisman { get { return OrtakFonksiyonlar.KurusYap(maliyetTutar - maliyetToplamAmortisman + (this.enflasyonKalanAmortisman)); } }


        public decimal degerArtisToplamTutar
        {
            get
            {
                decimal toplam = 0;
                foreach (AmortismanRapor3DegerArtis ar in degerDuzeltmeAmortismanX.objeler)
                {
                    if (ar.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME)
                        toplam += ar.maliyetTutar;
                }
                return toplam;
            }
        }
        public decimal degerArtisCariAmortisman
        {
            get
            {
                decimal toplam = 0;
                foreach (AmortismanRapor3DegerArtis ar in degerDuzeltmeAmortismanX.objeler)
                {
                    if (ar.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME)
                        toplam += ar.maliyetCariAmortisman;
                }
                return toplam;
            }
        }
        public decimal degerArtisBirikmisAmortisman
        {
            get
            {
                decimal toplam = 0;
                foreach (AmortismanRapor3DegerArtis ar in degerDuzeltmeAmortismanX.objeler)
                {
                    if (ar.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME)
                        toplam += ar.maliyetBirikmisAmortisman;
                }
                return toplam;
            }
        }
        public decimal degerArtisToplamAmortisman { get { return this.amortismanKaydet ? degerArtisBirikmisAmortisman : (degerArtisCariAmortisman + degerArtisBirikmisAmortisman); } }
        public decimal degerArtisKalanAmortisman { get { return degerArtisToplamTutar - degerArtisToplamAmortisman; } }


        public decimal enflasyonToplamTutar
        {
            get
            {
                decimal toplam = 0;
                foreach (AmortismanRapor3DegerArtis ar in degerDuzeltmeAmortismanX.objeler)
                {
                    if (ar.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE)
                        toplam += ar.maliyetTutar;
                }
                return toplam;
            }
        }

        //19.09.2024 tarihinde Merkez bankasından gelen talep üzerine EDF tutarları ayrı amortisman yapılarak kayıt alında alınacak. H.Ö.
        //public decimal enflasyonCariAmortisman = 0;
        public decimal enflasyonCariAmortisman
        {
            get
            {
                decimal toplam = 0;
                foreach (AmortismanRapor3DegerArtis ar in degerDuzeltmeAmortismanX.objeler)
                {
                    if (ar.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE)
                        toplam += ar.maliyetCariAmortisman;
                }
                return toplam;
            }
        }
        public decimal enflasyonBirikmisAmortisman
        {
            get
            {
                decimal toplam = 0;
                foreach (AmortismanRapor3DegerArtis ar in degerDuzeltmeAmortismanX.objeler)
                {
                    if (ar.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE)
                        toplam += ar.maliyetBirikmisAmortisman;
                }
                return toplam;
            }
        }
        public decimal enflasyonToplamAmortisman { get { return this.amortismanKaydet ? enflasyonBirikmisAmortisman : (enflasyonCariAmortisman + enflasyonBirikmisAmortisman); } }
        public decimal enflasyonKalanAmortisman { get { return enflasyonToplamTutar - enflasyonToplamAmortisman; } }


        public decimal degerDuzeltmeToplamTutar { get { return degerArtisToplamTutar + enflasyonToplamTutar; } }
        public decimal degerDuzeltmeCariAmortisman { get { return degerArtisCariAmortisman + enflasyonCariAmortisman; } }
        public decimal degerDuzeltmeBirikmisAmortisman { get { return degerArtisBirikmisAmortisman + enflasyonBirikmisAmortisman; } }
        public decimal degerDuzeltmeToplamAmortisman { get { return this.amortismanKaydet ? degerDuzeltmeBirikmisAmortisman : (degerDuzeltmeCariAmortisman + degerDuzeltmeBirikmisAmortisman); } }
        public decimal degerDuzeltmeKalanAmortisman { get { return degerDuzeltmeToplamTutar - degerDuzeltmeToplamAmortisman; } }


        public decimal toplamTutar { get { return maliyetTutar + degerDuzeltmeToplamTutar; } }
        public decimal toplamCariAmortisman { get { return maliyetCariAmortisman + degerDuzeltmeCariAmortisman; } }
        public decimal toplamBirikmisAmortisman { get { return maliyetBirikmisAmortisman + degerDuzeltmeBirikmisAmortisman; } }
        public decimal toplamAmortisman { get { return this.amortismanKaydet ? toplamBirikmisAmortisman : (toplamCariAmortisman + toplamBirikmisAmortisman); } }
        public decimal toplamKalanAmortisman { get { return OrtakFonksiyonlar.KurusYap(toplamTutar - toplamAmortisman); } }


        public decimal amortismanYuzdesi { get { return this.amortismanSuresi > 0 ? Math.Truncate(((decimal)100 / (decimal)this.amortismanSuresi) * 100) / 100 : 0; } }
        public decimal amortismanYuzdesiTamamlanan { get { return this.toplamTutar > 0 ? Math.Truncate((decimal)(toplamAmortisman / this.toplamTutar) * 100 * 100) / 100 : 0; } }
        public decimal sonDonemAmortisman { get; set; }
        public bool amortismanSuresiBittiMi
        {
            get
            {
                if (this.amortismanBaslamaYil == 0 || this.amortismanSuresi == 0)
                    return false;

                bool bitti = false;
                int yilKontrol = this.amortismanBaslamaYil + this.amortismanSuresi - 1 - this.yil;
                if (yilKontrol < 0 ||
                    yilKontrol == 0 && toplamKalanAmortisman == 0 ||
                    toplamAmortisman > 0 && toplamKalanAmortisman == 0 ||
                    yilKontrol >= this.amortismanSuresi)
                    bitti = true;

                return bitti;
            }
        }

        public decimal maliyetCariAmortismanKuruslu = 0;

        public decimal degerArtisBirikmisAmortismanTTK = 0;
        public decimal degerArtisCariAmortismanTTK = 0;

        public decimal enflasyonBirikmisAmortismanTTK = 0;
        public decimal enflasyonCariAmortismanTTK = 0;


        public bool gayriMenkulMu = false;
        public bool banknotGayriMenkulMu = false;
        public bool cariYilEldenCikti = false;

        public int sonAmortismanYil { get; set; }
        public int sonAmortismanDonem { get; set; }

        //public TNSCollection listeSicilNoHareket = null;
        //public TNSCollection listeDegerArtis = null;
        //public TNSCollection listeAmortisman = null;
        //public TNSCollection listeOzellik = null;


        //ilk Bedel - ilkBedel
        //Bedel Düzeltme Farkı - degerDuzeltmeToplamTutar
        //Son Bedel - toplamTutar
        //Ayrılan Son Dönem Amortisman - sonDonemAmortisman

        //Biriken Amortisman - toplamAmortisman
        //Amortisman Düzeltme Farkı - degerDuzeltmeBirikmisAmortisman
        //Düz Biriken Amort - degerDuzeltmeToplamAmortisman

        //Önceki Yıllar Amortsiman - toplamBirikmisAmortisman
        //Son dönem ayrılan amortsiman - sonDonemAmortisman
        //Amortisman Yüzdesi - amortismanYuzdesiTamamlanan


    }


    [Serializable()]
    public class AmortismanRapor3DegerArtis
    {
        public decimal maliyetTutar { get; set; }
        public int amortismanTur { get; set; }
        public decimal cariDegerlemeAmortismanTutar { get; set; }
        public int prSicilNo { get; set; }
        public string gorSicilNo { get; set; }
        public string degerArtisKod { get; set; }

        public decimal maliyetCariAmortisman { get; set; }
        public decimal maliyetBirikmisAmortisman { get; set; }

    }


    [Serializable()]

    public class AmortismanRaporBA
    {
        public int adetA = 0;
        public decimal ilkBedel = 0;
        public decimal bedelDuzeltmeFarki = 0;
        public decimal sonBedelA = 0;
        public decimal birikenAmortisman = 0;
        public decimal amortismanDuzeltmeFarki = 0;
        public decimal duzeltilmisBirikenAmortisman = 0;
        public decimal sonBedelB = 0;

        public int adet2A = 0;
        public decimal ilkBedel2 = 0;
        public decimal bedelDuzeltmeFarki2 = 0;
        public decimal sonBedel2A = 0;
        public decimal birikenAmortisman2 = 0;
        public decimal amortismanDuzeltmeFarki2 = 0;
        public decimal duzeltilmisBirikenAmortisman2 = 0;
        public decimal sonBedel2B = 0;
    }


    /// <summary>
    /// Taşınır malzemelerine uygulanan değerleme işlemlerine ait bilgileri tutan sınıf
    /// </summary>
    [Serializable()]
    public class AmortismanDegerleme
    {
        /// <summary>
        /// Değerleme yapılan yıl
        /// </summary>
        public int yil = 0;

        /// <summary>
        /// Değerleme yapılan malzemenin programatik sicil numarası
        /// </summary>
        public int prSicilNo = 0;

        /// <summary>
        /// Uygulanan değerleme tutarı
        /// </summary>
        public decimal degerlemeMiktar = (decimal)0;

        /// <summary>
        /// Uygulanan değerleme oranı
        /// </summary>
        public double degerlemeOran = (double)0;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod = string.Empty;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd = string.Empty;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod = string.Empty;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd = string.Empty;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod = string.Empty;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd = string.Empty;

        /// <summary>
        /// Değerleme yapılan malzemenin sicil numarası
        /// </summary>
        public string gorSicilNo = string.Empty;

        /// <summary>
        /// Değerleme yapılan malzemenin taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod = string.Empty;

        /// <summary>
        /// Değerleme yapılan malzemenin taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd = string.Empty;

        /// <summary>
        /// Değerleme işleminin yapılış tarihi ve zamanı
        /// </summary>
        public TNSDateTime islemTarih = new TNSDateTime();

        /// <summary>
        /// Değerleme işlemini yapan kullanıcının kodu
        /// </summary>
        public string islemYapan = string.Empty;

        /// <summary>
        /// Değerleme yapılan yılda yapılmış değerleme işlemlerinin toplam tutarı
        /// </summary>
        public decimal yapilanToplamDegerlemeYilIci = (decimal)0;

        /// <summary>
        /// Değerleme yapılan yıldan önceki yıllarda yapılmış değerleme işlemlerinin toplam tutarı
        /// </summary>
        public decimal yapilanToplamDegerlemeEskiYillar = (decimal)0;

        /// <summary>
        /// Parametresiz AmortismanDegerleme nesnesi oluşturucu yordam
        /// </summary>
        public AmortismanDegerleme()
        {
        }
    }

    /// <summary>
    /// Taşınır malzemelerine uygulanan amortisman işlemlerine ait bilgileri tutan sınıf
    /// </summary>
    [Serializable()]
    public class Amortisman
    {
        /// <summary>
        /// Amortisman yapılan yıl
        /// </summary>
        public int yil = 0;

        /// <summary>
        /// Amortisman yapılan malzemenin programatik sicil numarası
        /// </summary>
        public int prSicilNo = 0;

        /// <summary>
        /// Yapılan amortismanın türü
        /// ENUMAmortismanTur listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int amortismanTur = 0;

        /// <summary>
        /// Uygulanan amortisman tutarı
        /// </summary>
        public decimal amortismanMiktar = (decimal)0;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod = string.Empty;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd = string.Empty;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod = string.Empty;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd = string.Empty;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod = string.Empty;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd = string.Empty;

        /// <summary>
        /// Amortisman yapılan malzemenin sicil numarası
        /// </summary>
        public string gorSicilNo = string.Empty;

        /// <summary>
        /// Amortisman yapılan malzemenin taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod = string.Empty;

        /// <summary>
        /// Amortisman yapılan malzemenin taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd = string.Empty;

        /// <summary>
        /// Amortisman işleminin yapılış tarihi ve zamanı
        /// </summary>
        public TNSDateTime islemTarih = new TNSDateTime();

        /// <summary>
        /// Amortisman işlemini yapan kullanıcının kodu
        /// </summary>
        public string islemYapan = string.Empty;

        /// <summary>
        /// Malzemeye kaç kere amortisman uygulanmış sayısı
        /// </summary>
        public int yapilanAmortismanSayisi = 0;

        /// <summary>
        /// Malzemeye uygulanmış amortisman tutarlarının toplamı
        /// </summary>
        public decimal yapilanToplamAmortisman = (decimal)0;

        /// <summary>
        /// Malzemenin birim fiyatı
        /// </summary>
        public decimal maliyetTutar = (decimal)0;

        /// <summary>
        /// Malzemeye ait kalan amortisman süresi
        /// </summary>
        public int kalanAmortismanSure = 0;

        /// <summary>
        /// Parametresiz Amortisman nesnesi oluşturucu yordam
        /// </summary>
        public Amortisman()
        {
        }
    }

    /// <summary>
    /// Amortisman işlemlerinde kullanılan kriter bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class AmortismanKriter
    {
        public string kod = "";

        /// <summary>
        /// Yıl kriteri
        /// </summary>
        public int yil = 0;

        public int donem = 0;

        public int prSicilNo = 0;

        public string degerArtisKod = "";

        public decimal amortismanMiktar = 0;

        public bool sadeceCariYil = false;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod = string.Empty;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd = string.Empty;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod = string.Empty;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd = string.Empty;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod = string.Empty;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd = string.Empty;

        /// <summary>
        /// Belge yılı
        /// </summary>
        public int belgeYil = 0;

        /// <summary>
        /// Belge numarası
        /// </summary>
        public string belgeNo = string.Empty;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod = string.Empty;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd = string.Empty;

        /// <summary>
        /// Sicil numarası
        /// </summary>
        public string gorSicilNo = string.Empty;

        /// <summary>
        /// Yapılan işlemin türü
        /// ENUMAmortismanIslemTur listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int islemTur = 0;

        /// <summary>
        /// Yapılan amortismanın türü
        /// ENUMAmortismanTur listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int amortismanTur = 0;

        /// <summary>
        /// Uygulanan değerleme oranı
        /// </summary>
        public double degerlemeOran = (double)0;

        /// <summary>
        /// Yapılan işlemin uygulama türü
        /// ENUMAmortismanUygulamaTur listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int uygulamaTur = 0;

        /// <summary>
        /// Malzemenin edinme yılı başlangıç kriteri
        /// Aralık belirtilmesi gereken durumlarda edinmeIlkYili aralığın alt sınırını ifade eder.
        /// </summary>
        public int edinmeIlkYili = 0;

        /// <summary>
        /// Malzemenin edinme yılı bitiş kriteri
        /// Aralık belirtilmesi gereken durumlarda edinmeSonYili aralığın üst sınırını ifade eder.
        /// </summary>
        public int edinmeSonYili = 0;

        /// <summary>
        /// Uygulanacak birikmiş amortismans oranı
        /// Bu alan, amortisman işlemi ilk defa yapılıyorsa kullanılır.
        /// </summary>
        public double birikmisAmortismanOran = (double)0;

        /// <summary>
        /// Uygulanacak kalan amortisman süresi
        /// Bu alan, amortisman işlemi ilk defa yapılıyorsa kullanılır.
        /// </summary>
        public int kalanAmortismanSure = 0;

        /// <summary>
        /// Amortisman uygulanacak malzemelerin programatik sicil numaralarını tutan nesne
        /// </summary>
        public TNSCollection prSicilNolar = new TNSCollection();

        public TNSDateTime baslamaTarihi = new TNSDateTime();

        public TNSDateTime bitisTarihi = new TNSDateTime();        public bool altOdalar = false;
        /// <summary>
        /// Parametresiz AmortismanKriter nesnesi oluşturucu yordam
        /// </summary>
        public AmortismanKriter()
        {
        }

        /// <summary>
        /// Yıl parametresi alan AmortismanKriter nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        public AmortismanKriter(int yil)
        {
            this.yil = yil;
        }

        /// <summary>
        /// Yıl ve taşınır hesap planı kodu parametrelerini alan AmortismanKriter nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="hesapPlanKod">Taşınır hesap planı kodu</param>
        public AmortismanKriter(int yil, string hesapPlanKod)
        {
            this.yil = yil;
            this.hesapPlanKod = hesapPlanKod;
        }

        /// <summary>
        /// Yıl, muhasebe birimi, harcama birimi ve belge numarası parametrelerini alan AmortismanKriter nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebeKod">Muhasebe birimi kodu</param>
        /// <param name="harcamaKod">Harcama birimi kodu</param>
        /// <param name="belgeNo">Belge numarası</param>
        public AmortismanKriter(int yil, string muhasebeKod, string harcamaKod, string belgeNo)
        {
            this.yil = yil;
            this.belgeYil = yil;
            this.muhasebeKod = muhasebeKod;
            this.harcamaKod = harcamaKod;
            this.belgeNo = belgeNo;
        }

        public int raporTur { get; set; }

        public string kumulatif = "0";

        public int amortismanSure = 0;
        public string nereyeGitti = "";
        public int hesapPlanIslemDurum = 0;
        public string raporTurEkBilgi = "";

        public bool ihracDahil { get; set; }
        public int sayfaBaslangic = 0;
        public int sayfaAdet = 0;
        public string sayfaCacheKod = "";
    }

    /// <summary>
    /// Amortisman alt sınır tutarını yıl bazında tutan sınıf
    /// </summary>
    [Serializable()]
    public class AmortismanSinir
    {
        /// <summary>
        /// Yıl kriteri
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// Malzeme alt sınır tutarı
        /// </summary>
        public double sinir { get; set; }


        /// <summary>
        /// Taşıt amortismanı için her yıl belirlenen amortisman sınırı
        /// </summary>
        public double sinirTasit { get; set; }

        /// <summary>
        /// Parametresiz AmortismanSinir nesnesi oluşturucu yordam
        /// </summary>
        public AmortismanSinir()
        {
            yil = 0;
            sinir = 0;
            mifBelgeNo = null;
            sinirTasit = 0;
        }

        public string mifBelgeNo { get; set; }
    }

    /// <summary>
    /// Amortisman işlemi uygulanacak malzemelerin amortisman sürelerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class AmortismanSure
    {
        /// <summary>
        /// Yıl kriteri
        /// </summary>
        public int yil = 0;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod = string.Empty;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd = string.Empty;

        /// <summary>
        /// Amortisman süresi
        /// </summary>
        public int sure = 0;

        /// <summary>
        /// Parametresiz AmortismanSure nesnesi oluşturucu yordam
        /// </summary>
        public AmortismanSure()
        {
        }

        /// <summary>
        /// Yıl, taşınır hesap planı ve amortisman süresi alan AmortismanSure nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="hesapPlanKod">Taşınır hesap planı kodu</param>
        /// <param name="sure">Amortisman süresi</param>
        public AmortismanSure(int yil, string hesapPlanKod, int sure)
        {
            this.yil = yil;
            this.hesapPlanKod = hesapPlanKod;
            this.sure = sure;
        }
    }

    /// <summary>
    /// Ölçü birim bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class OlcuBirim
    {
        /// <summary>
        /// Ölçü birimin programatik kodu
        /// </summary>
        public int kod;

        /// <summary>
        /// Ölçü birimin adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Parametresiz OlcuBirim nesnesi oluşturucu yordam
        /// </summary>
        public OlcuBirim()
        {
            kod = 0;
            ad = "";
        }
    }

    /// <summary>
    /// MuhasebeIslemTipi bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class MuhasebeIslemTipi
    {
        public string kod = "";
        public string tip = "";
        public string ad = "";
    }

    /// <summary>
    /// İşlem tipi bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class IslemTip
    {
        /// <summary>
        /// İşlem tipinin programatik kodu
        /// </summary>
        public int kod;

        /// <summary>
        /// İşlem tipinin adı
        /// </summary>
        public string ad;

        /// <summary>
        /// İşlem tipinin türü
        /// ENUMIslemTipi listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int tur;

        /// <summary>
        /// Parametresiz IslemTip nesnesi oluşturucu yordam
        /// </summary>
        public IslemTip()
        {
            kod = 0;
            ad = "";
            tur = 0;
        }
    }

    /// <summary>
    /// Marka bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Marka
    {
        /// <summary>
        /// Markanın programatik kodu
        /// </summary>
        public int kod;

        /// <summary>
        /// Markanın adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Parametresiz Marka nesnesi oluşturucu yordam
        /// </summary>
        public Marka()
        {
            kod = 0;
            ad = string.Empty;
        }
    }

    /// <summary>
    /// Model bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Model
    {
        /// <summary>
        /// Markanın programatik kodu
        /// </summary>
        public int markaKodu;

        /// <summary>
        /// Modelin programatik kodu
        /// </summary>
        public int kod;

        /// <summary>
        /// Modelin adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Markanın adı
        /// </summary>
        public string markaAd;

        /// <summary>
        /// Parametresiz Model nesnesi oluşturucu yordam
        /// </summary>
        public Model()
        {
            markaKodu = 0;
            kod = 0;
            ad = string.Empty;
            markaAd = string.Empty;
        }
    }

    /// <summary>
    /// İl bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Il
    {
        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string kod;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Parametresiz Il nesnesi oluşturucu yordam
        /// </summary>
        public Il()
        {
            kod = string.Empty;
            ad = string.Empty;
        }
    }

    /// <summary>
    /// İlçe bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Ilce
    {
        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKodu;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string ilAd;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string kod;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Parametresiz Ilce nesnesi oluşturucu yordam
        /// </summary>
        public Ilce()
        {
            ilKodu = string.Empty;
            ilAd = string.Empty;
            kod = string.Empty;
            ad = string.Empty;
        }
    }

    /// <summary>
    /// Bölge bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Bolge
    {
        /// <summary>
        /// Bölgenin programatik kodu
        /// </summary>
        public int kod;

        /// <summary>
        /// Bölgenin adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Bölgenin muhasebe birimleri listesini tutan nesne
        /// </summary>
        public TNSCollection muhasebe;

        /// <summary>
        /// Parametresiz Bolge nesnesi oluşturucu yordam
        /// </summary>
        public Bolge()
        {
            kod = 0;
            ad = string.Empty;
            muhasebe = new TNSCollection();
        }
    }

    /// <summary>
    /// Muhasebe birimi bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Muhasebe
    {
        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string kod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string ad;

        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string ilAd;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilceAd;

        /// <summary>
        /// Muhasebe biriminin kapalı olup olmadığı bilgisi
        /// </summary>
        public int kapali;

        /// <summary>
        /// Bölgenin programatik kodu
        /// </summary>
        public int bolgeKod;

        /// <summary>
        /// Muhasebe biriminin KDV mükellefi olup olmadığı bilgisi
        /// </summary>
        public int kdvMukellefi;

        /// <summary>
        /// Muhasebe biriminin icmalden muaf olup olmadığı bilgisi
        /// </summary>
        public int icmalMuaf;

        /// <summary>
        /// Parametresiz Muhasebe nesnesi oluşturucu yordam
        /// </summary>
        public Muhasebe()
        {
            kod = string.Empty;
            ad = string.Empty;
            ilKod = string.Empty;
            ilAd = string.Empty;
            ilceKod = string.Empty;
            ilceAd = string.Empty;
            kapali = -1;
            bolgeKod = 0;
            kdvMukellefi = -1;
            icmalMuaf = -1;
        }
    }

    /// <summary>
    /// Harcama birimi bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class HarcamaBirimi
    {
        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string kod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string il;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilce;

        /// <summary>
        /// Harcama biriminin kapalı olup olmadığı bilgisi
        /// </summary>
        public int kapali;

        /// <summary>
        /// Parametresiz HarcamaBirimi nesnesi oluşturucu yordam
        /// </summary>
        public HarcamaBirimi()
        {
            ilKod = string.Empty;
            ilceKod = string.Empty;
            muhasebeKod = string.Empty;
            kod = string.Empty;
            ad = string.Empty;
            muhasebeAd = string.Empty;
            il = string.Empty;
            ilce = string.Empty;
            kapali = -1;
        }
    }

    /// <summary>
    /// Ambar bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Ambar
    {
        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaBirimKod;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string kod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Ambarın adresi
        /// </summary>
        public string adres;

        /// <summary>
        /// Taşınır kayıt ve kontrol yetkilisinin adı
        /// </summary>
        public string TKKYAd;

        /// <summary>
        /// Taşınır kayıt ve kontrol yetkilisinin unvanı
        /// </summary>
        public string TKKYUnvan;

        public string TKKYAd2;
        public string TKKYUnvan2;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaBirimAd;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string il;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilce;

        /// <summary>
        /// Ambarın kapalı olup olmadığı bilgisi
        /// </summary>
        public int kapali;

        /// <summary>
        /// Ambarın kullanıcı birimi olup olmadığı bilgisi
        /// </summary>
        public int kullaniciBirimi;

        /// <summary>
        /// Ambarın kullanıcı birimi olup olmadığı bilgisi
        /// </summary>
        public int hediye;

        public bool aktarim = false;

        /// <summary>
        /// Parametresiz Ambar nesnesi oluşturucu yordam
        /// </summary>
        public Ambar()
        {
            ilKod = string.Empty;
            ilceKod = string.Empty;
            muhasebeKod = string.Empty;
            harcamaBirimKod = string.Empty;
            kod = string.Empty;
            ad = string.Empty;
            adres = string.Empty;
            TKKYAd = string.Empty;
            TKKYUnvan = string.Empty;
            TKKYAd2 = string.Empty;
            TKKYUnvan2 = string.Empty;
            muhasebeAd = string.Empty;
            harcamaBirimAd = string.Empty;
            il = string.Empty;
            ilce = string.Empty;
            kapali = -1;
            kullaniciBirimi = -1;
            hediye = -1;
            aktarim = false;
        }
    }

    /// <summary>
    /// Firma bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class FirmaBilgisi
    {
        /// <summary>
        /// Firmayı kayıt eden kullanıcının kodu
        /// </summary>
        public string kullaniciKodu;

        /// <summary>
        /// Firmanın programatik kodu
        /// </summary>
        public int kod;

        /// <summary>
        /// Firmanın adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Firmanın vergi numarası
        /// </summary>
        public string vno;

        /// <summary>
        /// Firmanın vergi dairesi
        /// </summary>
        public string vd;

        /// <summary>
        /// Firmanın banka bilgisi
        /// </summary>
        public string banka;

        /// <summary>
        /// Firmanın banka hesap numarası
        /// </summary>
        public string hesapNo;

        /// <summary>
        /// Firmanın telefonu
        /// </summary>
        public string tel;

        /// <summary>
        /// Firmanın faksı
        /// </summary>
        public string fax;

        /// <summary>
        /// Parametresiz FirmaBilgisi nesnesi oluşturucu yordam
        /// </summary>
        public FirmaBilgisi()
        {
            kullaniciKodu = "";
            kod = 0;
            ad = "";
            vno = "";
            vd = "";
            banka = "";
            hesapNo = "";
            tel = "";
            fax = "";
        }
    }

    /// <summary>
    /// Oda bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Oda
    {
        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaBirimKod;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Odanın programatik kodu
        /// </summary>
        public string kod;

        /// <summary>
        /// Odanın görünen kodu
        /// </summary>
        public string kod2;

        /// <summary>
        /// Odanın adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaBirimAd;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string il;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilce;

        /// <summary>
        /// Seviye
        /// </summary>
        public int seviye;

        /// <summary>
        /// Üst Kod
        /// </summary>
        public int ustKod;

        /// <summary>
        /// Durum
        /// </summary>
        public int durum;


        public int siraNo;

        /// <summary>
        /// Parametresiz Oda nesnesi oluşturucu yordam
        /// </summary>
        public Oda()
        {
            ilKod = string.Empty;
            ilceKod = string.Empty;
            muhasebeKod = string.Empty;
            harcamaBirimKod = string.Empty;
            ambarKod = string.Empty;
            kod = string.Empty;
            kod2 = string.Empty;
            ad = string.Empty;
            muhasebeAd = string.Empty;
            harcamaBirimAd = string.Empty;
            ambarAd = string.Empty;
            il = string.Empty;
            ilce = string.Empty;
            seviye = 0;
            ustKod = 0;
            durum = 0;
            siraNo = 0;
        }
    }

    /// <summary>
    /// Personel bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Personel
    {
        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaBirimKod;

        /// <summary>
        /// Personelin T.C. kimlik numarası
        /// </summary>
        public string kod;

        /// <summary>
        /// Personelin adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Personelin unvanı
        /// </summary>
        public string unvan;

        /// <summary>
        /// Personelin görevi
        /// </summary>
        public string gorev;

        /// <summary>
        /// Personelin telefonu
        /// </summary>
        public string telefon;

        /// <summary>
        /// Personele ait fotograf varsa dosya adı
        /// </summary>
        public string resim;

        /// <summary>
        /// Personelin odası
        /// </summary>
        public string oda;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaBirimAd;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string il;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilce;

        /// <summary>
        /// Personelin cep telefonu
        /// </summary>
        public string cepTel;

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilmeyen malzemeler kurumdan
        /// çıkarılırsa personelin cep telefonuna mesaj atılıp atılmayacağı bilgisi
        /// </summary>
        public bool smsAt;

        /// <summary>
        /// Personelin e-posta adresi
        /// </summary>
        public string eposta;

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilmeyen malzemeler
        /// kurumdan çıkarılırsa personele e-posta atılıp atılmayacağı bilgisi
        /// </summary>
        public bool epostaAt;

        public string faks;

        /// <summary>
        /// Parametresiz Personel nesnesi oluşturucu yordam
        /// </summary>
        public Personel()
        {
            ilKod = string.Empty;
            ilceKod = string.Empty;
            muhasebeKod = string.Empty;
            harcamaBirimKod = string.Empty;
            kod = string.Empty;
            ad = string.Empty;
            unvan = string.Empty;
            gorev = string.Empty;
            telefon = string.Empty;
            resim = string.Empty;
            oda = string.Empty;
            muhasebeAd = string.Empty;
            harcamaBirimAd = string.Empty;
            il = string.Empty;
            ilce = string.Empty;
            cepTel = string.Empty;
            smsAt = false;
            eposta = string.Empty;
            epostaAt = false;
            faks = string.Empty;
        }
    }

    /// <summary>
    /// Raporların imza alanlarında kullanılan yetkili bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ImzaBilgisi
    {
        //public string kullaniciKodu;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebe;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string birim;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambar;

        /// <summary>
        /// İmza bilgilerinin hangi yetkiye sahip kişiye ait olduğu
        /// ENUMImzaYer listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int imzaYer;

        /// <summary>
        /// Yetkilinin TCKimlikNosu
        /// </summary>
        public string tcKimlikNo;

        /// <summary>
        /// Yetkilinin adı soyadı
        /// </summary>
        public string adSoyad;

        /// <summary>
        /// Yetkilinin ünvanı
        /// </summary>
        public string unvan;

        /// <summary>
        /// Yetkilinin göreviyle ilgili ünvanı
        /// </summary>
        public string gorevUnvan;

        /// <summary>
        /// Raporlarda yetkiliye ait bilgilerin yanısıra tarih bilgisi de görünsün mü bilgisi
        /// </summary>
        public int tarih;

        /// <summary>
        /// Yetkili imza bilgilerinin raporlarda kullanılıp kullanılmadığı bilgisi
        /// </summary>
        public int kullanilmiyor;

        /// <summary>
        /// Parametresiz ImzaBilgisi nesnesi oluşturucu yordam
        /// </summary>
        public ImzaBilgisi()
        {
            //kullaniciKodu = "";
            muhasebe = "";
            birim = "";
            ambar = "";
            imzaYer = 0;
            adSoyad = "";
            unvan = "";
            gorevUnvan = "";
            tarih = 0;
            kullanilmiyor = 0;
        }
    }

    /// <summary>
    /// Taşınır hesap planı bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class HesapPlaniSatir
    {
        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Taşınır hesap planı kodunun kaç kırılımlı olduğu bilgisi
        /// </summary>
        public int seviye;

        /// <summary>
        /// Taşınır hesap planı kodunun detayının olup olmadığı bilgisi
        /// </summary>
        public bool detay;

        /// <summary>
        /// Ölçü biriminin programatik kodu
        /// </summary>
        public int olcuBirim;

        /// <summary>
        /// KDV oranı
        /// </summary>
        public int kdv;

        /// <summary>
        /// Ölçü biriminin adı
        /// </summary>
        public string olcuBirimAd;

        /// <summary>
        /// Taşınır hesap planı bilgilerinin güncellemelerden etkilenip etkilenmeyeceği bilgisi
        /// </summary>
        public int guncelleme;

        public int vurgula;

        /// <summary>
        /// Taşınır hesap planı bilgilerinin kullanılıp kullanılmadığı bilgisi
        /// </summary>
        public int kullanilmiyor;

        /// <summary>
        /// Taşınır hesap planının numara bilgisi
        /// </summary>
        public string numara;

        /// <summary>
        /// Taşınır malzemesinin resmi varsa resme ait dosya adı
        /// </summary>
        public string resim;

        /// <summary>
        /// Taşınır malzemesinin resmi varsa resme ait dosya adı
        /// </summary>
        public string hesapKodAciklama;

        /// <summary>
        /// Taşınır malzemesinin barkod bilgilerini tutan sınıf
        /// </summary>
        public TasHesapPlaniBarkod hesapKoduBarkod;
        public int amortiYil;

        public int rfidEtiketKod;
        public string rfidEtiketAd;
        public int markaKod;
        public string markaAd;
        public int modelKod;
        public string modelAd;

        public int kistAmortisman;
        public int toplamKayitSayisi = 0;


        /// <summary>
        /// Parametresiz HesapPlaniSatir nesnesi oluşturucu yordam
        /// </summary>
        public HesapPlaniSatir()
        {
            hesapKod = "";
            aciklama = "";
            seviye = 0;
            detay = false;
            olcuBirim = -1;
            kdv = -1;
            olcuBirimAd = "";
            guncelleme = 0;
            vurgula = 0;
            kullanilmiyor = 0;
            numara = "";
            resim = string.Empty;
            hesapKodAciklama = string.Empty;
            hesapKoduBarkod = new TasHesapPlaniBarkod();
            amortiYil = 0;
            rfidEtiketKod = -1;
            rfidEtiketAd = "";
            markaKod = -1;
            markaAd = "";
            modelKod = -1;
            modelAd = "";
            kistAmortisman = 0;
        }
    }

    /// <summary>
    /// Taşınır işlem fişinin üst bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class TasinirIslemForm
    {
        /// <summary>
        /// Belge yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Belge numarası
        /// </summary>
        public string fisNo;

        /// <summary>
        /// Belge tarihi
        /// </summary>
        public TNSDateTime fisTarih;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Belgenin işlem tipinin kodu
        /// </summary>
        public int islemTipKod;

        /// <summary>
        /// Taşınır işlem fişinin muayene kabul belgesinin numarası
        /// </summary>
        public string muayeneNo;

        /// <summary>
        /// Taşınır işlem fişinin muayene kabul belgesinin tarihi
        /// </summary>
        public TNSDateTime muayeneTarih;

        /// <summary>
        /// Taşınır işlem fişine dayanak belgenin numarası
        /// </summary>
        public string dayanakNo;

        /// <summary>
        /// Taşınır işlem fişine dayanak belgenin tarihi
        /// </summary>
        public TNSDateTime dayanakTarih;

        /// <summary>
        /// Belgede geçen malzemelerin alındığı faturanın numarası
        /// </summary>
        public string faturaNo;

        /// <summary>
        /// Belgede geçen malzemelerin alındığı faturanın tarihi
        /// </summary>
        public TNSDateTime faturaTarih;

        /// <summary>
        /// Belgede geçen malzemelerin alındığı yer bilgisi
        /// </summary>
        public string neredenGeldi;

        /// <summary>
        /// Belgede geçen malzemelerin verildiği yer bilgisi
        /// </summary>
        public string nereyeGitti;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği harcama birimi kodu
        /// </summary>
        public string gHarcamaKod;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği ambar kodu
        /// </summary>
        public string gAmbarKod;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği muhasebe birimi kodu
        /// </summary>
        public string gMuhasebeKod;

        /// <summary>
        /// Malzemeleri gönderen belgenin yılı
        /// </summary>
        public int gYil;

        /// <summary>
        /// Malzemeleri gönderen belgenin numarası
        /// </summary>
        public string gFisNo;

        /// <summary>
        /// Tekil Kod
        /// </summary>
        public string kod;

        /// <summary>
        /// ENUMTasinirIslemFormOnayDurumu
        /// </summary>
        public int onayDurum;

        /// <summary>
        /// Belgenin durumu
        /// ENUMBelgeDurumu listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int durum;

        /// <summary>
        /// Belge ile ilgili işlemin yapılış tarihi ve zamanı
        /// </summary>
        public TNSDateTime islemTarih;

        /// <summary>
        /// Belge ile ilgili işlemi yapan kullanıcının kodu
        /// </summary>
        public string islemYapan;

        /// <summary>
        /// Belgede geçen malzemelerin verildiği kişi bilgisi
        /// </summary>
        public string kimeGitti;

        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string ilAd;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilceAd;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği muhasebe birimi adı
        /// </summary>
        public string gMuhasebeAd;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği harcama birimi adı
        /// </summary>
        public string gHarcamaAd;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği ambar adı
        /// </summary>
        public string gAmbarAd;

        /// <summary>
        /// Belgenin işlem tipinin adı
        /// </summary>
        public string islemTipAd;

        /// <summary>
        /// Belgenin işlem tipinin türü
        /// ENUMIslemTipi listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int islemTipTur;

        /// <summary>
        /// Belgenin devir giriş belgesi olup olmadığı bilgisi
        /// </summary>
        public bool devirGirisiMi;

        /// <summary>
        /// Taşınır işlem fişinin detay bilgilerini tutan nesne
        /// </summary>
        public ObjectArray detay;
        public string ipAdresi = "";

        public string aciklama;

        public string ad;

        public int gayrimenkulTur;

        public double fiyati;

        public int dosyaSayisi;

        public string adres;

        public string paftaNo;

        public string adaNo;

        public string parselNo;

        public string alani;

        public string edinmeSekli;

        public new TNSDateTime edinmeTarihi;

        public string kayittanCikisNedeni;

        public new TNSDateTime kayittanCikisTarihi;

        public string tifFisNo;
        public bool kayittanSonraOnayla = false;
        public ENUMTasinirIslemFormOnayDurumu BAOnayAdimi = ENUMTasinirIslemFormOnayDurumu.TANIMSIZ;

        public string onayAciklama;
        public int mutemmim;

        /// <summary>
        /// Veri aktarımı yapılırken, kontrol gerektiren yerlerde kullanılmak üzere eklendi
        /// </summary>
        public bool aktarim = false;

        public bool numaraVer = false;

        /// <summary>
        /// Parametresiz TasinirIslemForm nesnesi oluşturucu yordam
        /// </summary>
        public TasinirIslemForm()
        {
            yil = 0;
            fisNo = string.Empty;
            fisTarih = new TNSDateTime();
            muhasebeKod = string.Empty;
            harcamaKod = string.Empty;
            ambarKod = string.Empty;
            islemTipKod = 0;
            muayeneNo = string.Empty;
            muayeneTarih = new TNSDateTime();
            dayanakNo = string.Empty;
            dayanakTarih = new TNSDateTime();
            faturaNo = string.Empty;
            faturaTarih = new TNSDateTime();
            neredenGeldi = string.Empty;
            nereyeGitti = string.Empty;
            gHarcamaKod = string.Empty;
            gAmbarKod = string.Empty;
            gMuhasebeKod = string.Empty;
            gYil = 0;
            gFisNo = "";
            durum = 0;
            islemTarih = new TNSDateTime();
            islemYapan = string.Empty;
            kimeGitti = string.Empty;

            ilKod = string.Empty;
            ilAd = string.Empty;
            ilceKod = string.Empty;
            ilceAd = string.Empty;
            muhasebeAd = string.Empty;
            harcamaAd = string.Empty;
            ambarAd = string.Empty;
            gMuhasebeAd = string.Empty;
            gHarcamaAd = string.Empty;
            gAmbarAd = string.Empty;
            islemTipAd = string.Empty;
            islemTipTur = 0;

            ad = "";
            gayrimenkulTur = 0;
            fiyati = 0.0;
            dosyaSayisi = 0;
            adres = "";
            paftaNo = "";
            adaNo = "";
            parselNo = "";
            alani = "";
            edinmeSekli = "";
            edinmeTarihi = new TNSDateTime();
            kayittanCikisNedeni = "";
            kayittanCikisTarihi = new TNSDateTime();

            devirGirisiMi = false;

            kod = string.Empty;
            onayDurum = (int)ENUMTasinirIslemFormOnayDurumu.TANIMSIZ;
            aciklama = "";
            onayAciklama = "";
            mutemmim = 0;

            detay = new ObjectArray();
            tifFisNo = "";
        }

        /// <summary>
        /// Belgenin durumuna ilişkin açıklama bilgisini döndüren yordam
        /// </summary>
        /// <param name="durum">ENUMBelgeDurumu listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Belge durumunun açıklaması döndürülür.</returns>
        public static string FormDurumAciklama(int durum)
        {
            if (durum == (int)ENUMBelgeDurumu.YENI) return TasinirMalResI.TMM007;
            else if (durum == (int)ENUMBelgeDurumu.DEGISTIRILDI) return TasinirMalResI.TMM008;
            else if (durum == (int)ENUMBelgeDurumu.ONAYLI) return TasinirMalResI.TMM009;
            else if (durum == (int)ENUMBelgeDurumu.ONAYKALDIR) return TasinirMalResI.TMM010;
            else if (durum == (int)ENUMBelgeDurumu.IPTAL) return TasinirMalResI.TMM011;
            else
                return string.Format(TasinirMalResI.TMM012, durum);
        }
    }

    /// <summary>
    /// Taşınır işlem fişinin detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class TasinirIslemDetay
    {
        /// <summary>
        /// Belge yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Belge numarası
        /// </summary>
        public string fisNo;

        /// <summary>
        /// Malzemenin detayda yer aldığı satır numarası
        /// </summary>
        public int siraNo;

        /// <summary>
        /// Malzemenin taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod;

        /// <summary>
        /// Malzemenin sicil numarası
        /// </summary>
        public string gorSicilNo;

        /// <summary>
        /// Malzemenin programatik sicil numarası
        /// </summary>
        public int sicilNo;

        /// <summary>
        /// Malzemenin adedi
        /// </summary>
        public decimal miktar;

        /// <summary>
        /// KDV oranı
        /// </summary>
        public int kdvOran;

        /// <summary>
        /// Malzemenin birim fiyatı
        /// </summary>
        public decimal birimFiyat;

        /// <summary>
        /// Malzemenin KDV'li birim fiyatı
        /// </summary>
        public decimal birimFiyatKDVLi;

        /// <summary>
        /// Malzemenin eski sicil numarası
        /// </summary>
        public string eSicilNo;

        /// <summary>
        /// Malzemenin alım tarihi
        /// </summary>
        public TNSDateTime eAlimTarihi;

        /// <summary>
        /// Malzemenin tedarik şekli
        /// </summary>
        public string eTedarikSekli;

        /// <summary>
        /// Malzemeye ait açıklama
        /// </summary>
        public string eAciklama;

        /// <summary>
        /// Ölçü biriminin adı
        /// </summary>
        public string olcuBirimAd;

        /// <summary>
        /// Malzemenin taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği muhasebe birimi kodu
        /// </summary>
        public string gonMuhasebeKod;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği harcama birimi kodu
        /// </summary>
        public string gonHarcamaKod;

        /// <summary>
        /// Malzemenin özellik bilgilerini tutan nesne
        /// </summary>
        public SicilNoOzellik ozellik;

        /// <summary>
        /// Malzemenin son kullanma tarihi
        /// </summary>
        public TNSDateTime sonTarih;

        public string yerleskeYeri = "";
        public string yerleskeYeriAd = "";

        public decimal satisBedeli;

        /// <summary>
        /// Malzemenin tutar= miktar*birimFiyatKDVLi
        /// </summary>
        public decimal tutar;

        /// <summary>
        /// Parametresiz TasinirIslemDetay nesnesi oluşturucu yordam
        /// </summary>
        public TasinirIslemDetay()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            harcamaKod = string.Empty;
            ambarKod = string.Empty;
            fisNo = string.Empty;
            siraNo = 0;
            hesapPlanKod = string.Empty;
            gorSicilNo = string.Empty;
            sicilNo = 0;
            miktar = (decimal)0;
            kdvOran = 0;
            birimFiyat = 0;
            birimFiyatKDVLi = 0;
            eSicilNo = string.Empty;
            eAlimTarihi = new TNSDateTime();
            eTedarikSekli = string.Empty;
            eAciklama = string.Empty;
            olcuBirimAd = string.Empty;
            hesapPlanAd = string.Empty;
            gonMuhasebeKod = string.Empty;
            gonHarcamaKod = string.Empty;
            ozellik = new SicilNoOzellik();
            sonTarih = new TNSDateTime();
            yerleskeYeri = "";
            satisBedeli = 0;
            tutar = (decimal)0;
        }
    }

    /// <summary>
    /// Taşınır işlem fişlerinin sorgulanmasında kullanılan kriter bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class TasinirFormKriter
    {
        /// <summary>
        /// Başlangıç belge numarası
        /// Aralık belirtilmesi gereken durumlarda belgeNoBasla aralığın alt sınırını ifade eder.
        /// </summary>
        public string belgeNoBasla;

        /// <summary>
        /// Bitiş belge numarası
        /// Aralık belirtilmesi gereken durumlarda belgeNoBit aralığın üst sınırını ifade eder.
        /// </summary>
        public string belgeNoBit;

        /// <summary>
        /// Başlangıç belge tarihi
        /// Aralık belirtilmesi gereken durumlarda belgeTarihBasla aralığın alt sınırını ifade eder.
        /// </summary>
        public TNSDateTime belgeTarihBasla;

        /// <summary>
        /// Bitiş belge tarihi
        /// Aralık belirtilmesi gereken durumlarda belgeTarihBit aralığın üst sınırını ifade eder.
        /// </summary>
        public TNSDateTime belgeTarihBit;

        /// <summary>
        /// Başlangıç durum tarihi
        /// Aralık belirtilmesi gereken durumlarda durumTarihBasla aralığın alt sınırını ifade eder.
        /// </summary>
        public TNSDateTime durumTarihBasla;

        /// <summary>
        /// Bitiş durum tarihi
        /// Aralık belirtilmesi gereken durumlarda durumTarihBit aralığın üst sınırını ifade eder.
        /// </summary>
        public TNSDateTime durumTarihBit;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapKodu;

        /// <summary>
        /// İşlem tipinin kodu
        /// </summary>
        public int islemTipi;

        /// <summary>
        /// İşlemi yapan kullanıcının kodu
        /// </summary>
        public string islemYapan;

        /// <summary>
        /// Malzemelerin verildiği yer bilgisi
        /// </summary>
        public string nereyeVeridi;

        /// <summary>
        /// Malzemelerin verildiği kişi bilgisi
        /// </summary>
        public string kimeVeridi;

        /// <summary>
        /// Malzemelerin alındığı yer bilgisi
        /// </summary>
        public string neredenGeldi;

        /// <summary>
        /// İşlem tiplerinin ";" ayracı ile birleştirilmiş hali
        /// </summary>
        public string islemTipleri;

        public string mernis;
        public string degilHesapKodu;
        public string faturaNo;
        public string aciklama;


        /// <summary>
        /// Parametresiz TasinirFormKriter nesnesi oluşturucu yordam
        /// </summary>
        public TasinirFormKriter()
        {
            belgeNoBasla = string.Empty;
            belgeNoBit = string.Empty;
            belgeTarihBasla = new TNSDateTime();
            belgeTarihBit = new TNSDateTime();
            durumTarihBasla = new TNSDateTime();
            durumTarihBit = new TNSDateTime();
            hesapKodu = string.Empty;
            islemTipi = 0;
            islemYapan = string.Empty;
            nereyeVeridi = string.Empty;
            kimeVeridi = string.Empty;
            neredenGeldi = string.Empty;
            islemTipleri = string.Empty;
            mernis = string.Empty;
            degilHesapKodu = string.Empty;
            faturaNo = string.Empty;
            aciklama = string.Empty;
        }
    }

    /// <summary>
    /// Taşınır işlem fişinin bağlandığı muhasebe işlem fişinin üst bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class TasinirIslemMIF
    {
        /// <summary>
        /// Belge yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Taşınır işlem fişinin belge numarası
        /// </summary>
        public string tifBelgeNo;

        /// <summary>
        /// Muhasebe işlem fişinin belge numarası
        /// </summary>
        public string mifBelgeNo;

        /// <summary>
        /// Muhasebe işlem fişinin durumu
        /// ENUMBelgeDurumu listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int durum;

        /// <summary>
        /// Muhasebe işlem fişinin türü
        /// </summary>
        public int mifBelgeTur;

        /// <summary>
        /// Parametresiz TasinirIslemMIF nesnesi oluşturucu yordam
        /// </summary>
        public TasinirIslemMIF()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            harcamaKod = string.Empty;
            ambarKod = string.Empty;
            tifBelgeNo = string.Empty;
            mifBelgeNo = string.Empty;
            mifBelgeTur = 0;
        }
    }

    /// <summary>
    /// Taşınır işlem fişinin bağlandığı muhasebe işlem fişinin detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class TasinirIslemMIFDetay
    {
        /// <summary>
        /// Hesap planı kodu
        /// </summary>
        public string hesapKod;

        /// <summary>
        /// Hesap planının KDV'li borç tutarı
        /// </summary>
        public double borcKDVDahil;

        /// <summary>
        /// Hesap planının KDV'li alacak tutarı
        /// </summary>
        public double alacakKDVDahil;

        /// <summary>
        /// Hesap planının KDV'siz borç tutarı
        /// </summary>
        public double borcKDVHaric;

        /// <summary>
        /// Hesap planının KDV'siz alacak tutarı
        /// </summary>
        public double alacakKDVHaric;

        /// <summary>
        /// KDV oranı
        /// </summary>
        public string kdvYuzdesi;

        /// <summary>
        /// Hesap planı adı
        /// </summary>
        public string hesapAd;

        /// <summary>
        /// Parametresiz TasinirIslemMIFDetay nesnesi oluşturucu yordam
        /// </summary>
        public TasinirIslemMIFDetay()
        {
            hesapKod = "";
            borcKDVDahil = 0;
            alacakKDVDahil = 0;
            borcKDVHaric = 0;
            alacakKDVHaric = 0;
            kdvYuzdesi = "";
            hesapAd = "";
        }
    }

    /// <summary>
    /// Sayım formunun üst bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class SayimForm
    {
        /// <summary>
        /// Sayım yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Sayımın programatik numarası
        /// </summary>
        public string sayimNo;

        /// <summary>
        /// Sayım tarihi
        /// </summary>
        public TNSDateTime sayimTarih;

        /// <summary>
        /// Sayımın detay bilgileri listesini tutan nesne
        /// </summary>
        public TNSCollection detay;

        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string ilAd;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilceAd;

        /// <summary>
        /// Parametresiz SayimForm nesnesi oluşturucu yordam
        /// </summary>
        public SayimForm()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            sayimNo = string.Empty;
            sayimTarih = new TNSDateTime();
            detay = new TNSCollection();
            ilKod = string.Empty;
            ilAd = string.Empty;
            ilceKod = string.Empty;
            ilceAd = string.Empty;
        }
    }

    /// <summary>
    /// Sayım formunun detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class SayimDetay
    {
        /// <summary>
        /// Sayım yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Sayımın programatik numarası
        /// </summary>
        public string sayimNo;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi kodu
        /// </summary>
        public int olcuBirimKod;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi adı
        /// </summary>
        public string olcuBirimAd;

        /// <summary>
        /// Kayıtlara göre ambarda bulunan malzeme adedi
        /// </summary>
        public decimal kayitAmbarMiktar;

        /// <summary>
        /// Fiziksel olarak ambarda bulunan malzeme adedi
        /// </summary>
        public decimal ambarMiktar;

        /// <summary>
        /// Kayıtlara göre ortak alan zimmette bulunan malzeme adedi
        /// </summary>
        public decimal kayitOrtakMiktar;

        /// <summary>
        /// Fiziksel olarak ortak alan zimmette bulunan malzeme adedi
        /// </summary>
        public decimal ortakMiktar;

        /// <summary>
        /// Kayıtlara göre kişi zimmette bulunan malzeme adedi
        /// </summary>
        public decimal kayitKisiMiktar;

        /// <summary>
        /// Fiziksel olarak bulunan malzemelerin toplam adedinin kayıtlardaki malzemelerin toplam adedinden fazla miktarı
        /// </summary>
        public decimal fazlaMiktar;

        /// <summary>
        /// Fiziksel olarak bulunan malzemelerin toplam adedinin kayıtlardaki malzemelerin toplam adedinden eksik miktarı
        /// </summary>
        public decimal noksanMiktar;

        /// <summary>
        /// Sayımla ilgili taşınır hesap planı bazında açıklama
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Parametresiz SayimDetay nesnesi oluşturucu yordam
        /// </summary>
        public SayimDetay()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            sayimNo = string.Empty;
            hesapPlanKod = string.Empty;
            hesapPlanAd = string.Empty;
            olcuBirimKod = 0;
            olcuBirimAd = string.Empty;
            kayitAmbarMiktar = (decimal)0;
            ambarMiktar = (decimal)0;
            kayitOrtakMiktar = (decimal)0;
            ortakMiktar = (decimal)0;
            kayitKisiMiktar = (decimal)0;
            fazlaMiktar = (decimal)0;
            noksanMiktar = (decimal)0;
            aciklama = string.Empty;
        }
    }

    /// <summary>
    /// RFID entegrasyonu kullanılarak yapılan sayımın üst bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class SayimRFID
    {
        /// <summary>
        /// Sayım yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Sayımın programatik numarası
        /// </summary>
        public string sayimNo;

        /// <summary>
        /// Sayımın adı/açıklaması
        /// </summary>
        public string sayimAdi;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        public string tcKimlikNo;
        public string kisiAd;
        public string odaKod;
        public string odaAd;
        public ENUMSayimRFIDDurum durum;

        /// <summary>
        /// Sayım tarihi
        /// </summary>
        public TNSDateTime sayimTarih;

        /// <summary>
        /// Sayımın detay bilgileri listesini tutan nesne
        /// </summary>
        public TNSCollection detay;

        /// <summary>
        /// Parametresiz SayimRFID nesnesi oluşturucu yordam
        /// </summary>
        public SayimRFID()
        {
            yil = 0;
            sayimNo = string.Empty;
            sayimAdi = string.Empty;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            sayimTarih = new TNSDateTime();
            tcKimlikNo = string.Empty;
            kisiAd = string.Empty;
            odaKod = string.Empty;
            odaAd = string.Empty;
            detay = new TNSCollection();
            durum = (int)ENUMSayimRFIDDurum.TANIMSIZ;
        }
    }

    /// <summary>
    /// RFID entegrasyonu kullanılarak yapılan sayımın detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class SayimRFIDDetay
    {
        /// <summary>
        /// Sayım yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Sayımın programatik numarası
        /// </summary>
        public string sayimNo;

        /// <summary>
        /// Malzemenin programatik sicil numarası
        /// </summary>
        public int prSicilNo;

        /// <summary>
        /// Malzemenin sicil numarası
        /// </summary>
        public string sicilNo;

        /// <summary>
        /// Malzemenin taşınır hesap planı adı
        /// </summary>
        public string sicilAd;

        /// <summary>
        /// Malzemenin sayımla ilgili durumu
        /// ENUMSayimRFIDDurum listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public ENUMSayimRFIDDemirbasDurum durum;

        public int terminalKod;
        public string bulunanOdaKod;
        public string bulunanOdaAd;
        public string beklenenOdaKod;
        public string beklenenOdaAd;
        public string bulunanKisiKod;
        public string bulunanKisiAd;
        public string beklenenKisiKod;
        public string beklenenKisiAd;
        public DateTime bulunanTarih;
        public string beklenenMuhasebeKod;
        public string beklenenMuhasebeAd;
        public string beklenenHarcamaBirimKod;
        public string beklenenHarcamaBirimAd;
        public string beklenenAmbarKod;
        public string beklenenAmbarAd;
        public int rfid;
        public string tckimlikno;
        public string oda;

        /// <summary>
        /// Parametresiz SayimRFIDDetay nesnesi oluşturucu yordam
        /// </summary>
        public SayimRFIDDetay()
        {
            yil = 0;
            sayimNo = string.Empty;
            prSicilNo = 0;
            sicilNo = string.Empty;
            sicilAd = string.Empty;
            durum = 0;
            terminalKod = 0;
            bulunanOdaKod = string.Empty;
            bulunanOdaAd = string.Empty;
            beklenenOdaKod = string.Empty;
            beklenenOdaAd = string.Empty;
            bulunanKisiKod = string.Empty;
            bulunanKisiAd = string.Empty;
            beklenenKisiKod = string.Empty;
            beklenenKisiAd = string.Empty;
            bulunanTarih = new DateTime();
            beklenenMuhasebeKod = string.Empty;
            beklenenMuhasebeAd = string.Empty;
            beklenenHarcamaBirimKod = string.Empty;
            beklenenHarcamaBirimAd = string.Empty;
            beklenenAmbarKod = string.Empty;
            beklenenAmbarAd = string.Empty;
            rfid = 0;
            tckimlikno = string.Empty;
            oda = string.Empty;
        }
    }

    /// <summary>
    /// RFID entegrasyonu kullanılarak yapılan sayımın detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class SayimRFIDDetay2
    {
        /// <summary>
        /// Sayım yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Sayımın programatik numarası
        /// </summary>
        public string sayimNo;

        /// <summary>
        /// Sayımın rfid
        /// </summary>
        public int rfid;

        /// <summary>
        /// Malzemenin programatik sicil numarası
        /// </summary>
        public int prSicilNo;

        /// <summary>
        /// Malzemenin sahibi tckimlikno
        /// </summary>
        public string tckimlikno;

        /// <summary>
        /// Malzemenin oda numarası
        /// </summary>
        public string oda;

        /// <summary>
        /// Malzemenin sayımla ilgili durumu
        /// ENUMSayimRFIDDurum listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public ENUMSayimRFIDDemirbasDurum durum;


        /// <summary>
        /// Parametresiz SayimRFIDDetay nesnesi oluşturucu yordam
        /// </summary>
        public SayimRFIDDetay2()
        {
            yil = 0;
            sayimNo = string.Empty;
            rfid = 0;
            prSicilNo = 0;
            tckimlikno = string.Empty;
            oda = string.Empty;
            durum = 0;

        }
    }
    /// <summary>
    /// Taşınır istek formunun üst bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class IstekForm
    {
        /// <summary>
        /// Belge yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string ilAd;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilceAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Belge numarası
        /// </summary>
        public string belgeNo;

        /// <summary>
        /// Belge tarihi
        /// </summary>
        public TNSDateTime belgeTarihi;

        /// <summary>
        /// İstekte bulunan personelin T.C. kimlik numarası
        /// </summary>
        public string istekYapanKod;

        /// <summary>
        /// İstekte bulunan personelin adı
        /// </summary>
        public string istekYapanAd;

        /// <summary>
        /// Taşınır istek formunun detay bilgilerini tutan nesne
        /// </summary>
        public TNSCollection detay;

        /// <summary>
        /// Başlangıç belge tarihi
        /// Aralık belirtilmesi gereken durumlarda baslaTarih aralığın alt sınırını ifade eder.
        /// </summary>
        public TNSDateTime baslaTarih;

        /// <summary>
        /// Bitiş belge tarihi
        /// Aralık belirtilmesi gereken durumlarda bitTarih aralığın üst sınırını ifade eder.
        /// </summary>
        public TNSDateTime bitTarih;

        public int hediyelik;

        /// <summary>
        /// Parametresiz IstekForm nesnesi oluşturucu yordam
        /// </summary>
        public IstekForm()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            ilKod = string.Empty;
            ilAd = string.Empty;
            ilceKod = string.Empty;
            ilceAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            belgeNo = string.Empty;
            belgeTarihi = new TNSDateTime();
            istekYapanKod = string.Empty;
            istekYapanAd = string.Empty;
            detay = new TNSCollection();

            baslaTarih = new TNSDateTime();
            bitTarih = new TNSDateTime();

            hediyelik = 0;
        }
    }

    /// <summary>
    /// Taşınır istek formunun detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class IstekDetay
    {
        /// <summary>
        /// Belge yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Belge numarası
        /// </summary>
        public string belgeNo;

        /// <summary>
        /// Malzemenin taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod;

        /// <summary>
        /// Malzemenin taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi kodu
        /// </summary>
        public int olcuBirimKod;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi adı
        /// </summary>
        public string olcuBirimAd;

        /// <summary>
        /// Malzemenin istenilen, ihtiyaç duyulan adedi
        /// </summary>
        public decimal istenilenMiktar;

        /// <summary>
        /// Malzemenin karşılanan, alınan adedi
        /// </summary>
        public decimal karsilananMiktar;

        public int kdvOran;
        public decimal birimFiyat;
        public string aciklama;

        /// <summary>
        /// Malzemenin tüketilen, kullanılan adedi
        /// </summary>
        public decimal tuketilenMiktar;

        /// <summary>
        /// İstekte bulunan personelin T.C. kimlik numarası
        /// </summary>
        public string istekYapanKod;

        /// <summary>
        /// İstekte bulunan personelin adı
        /// </summary>
        public string istekYapanAd;

        /// <summary>
        /// Parametresiz IstekDetay nesnesi oluşturucu yordam
        /// </summary>
        public IstekDetay()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            belgeNo = string.Empty;
            hesapPlanKod = string.Empty;
            hesapPlanAd = string.Empty;
            olcuBirimKod = 0;
            olcuBirimAd = string.Empty;
            istenilenMiktar = (decimal)0;
            karsilananMiktar = (decimal)0;
            kdvOran = 0;
            birimFiyat = 0;
            aciklama = string.Empty;
            tuketilenMiktar = (decimal)0;

            istekYapanKod = string.Empty;
            istekYapanAd = string.Empty;
        }
    }

    /// <summary>
    /// Taşınır cetvellerinin üst bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Cetvel
    {
        /// <summary>
        /// Yıl kriteri
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd;

        /// <summary>
        /// Bölgenin programatik kodu
        /// </summary>
        public int bolgeKod;

        /// <summary>
        /// Bölgenin adı
        /// </summary>
        public string bolgeAd;

        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string ilAd;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilceAd;

        /// <summary>
        /// Taşınır cetvellerinin detay bilgilerini tutan nesne
        /// </summary>
        public TNSCollection detay;

        /// <summary>
        /// Başlangıç belge tarihi
        /// Aralık belirtilmesi gereken durumlarda tarih1 aralığın alt sınırını ifade eder.
        /// </summary>
        public TNSDateTime tarih1;

        /// <summary>
        /// Bitiş belge tarihi
        /// Aralık belirtilmesi gereken durumlarda tarih2 aralığın üst sınırını ifade eder.
        /// </summary>
        public TNSDateTime tarih2;

        public int seviye;

        public bool harcamaZorunluDegil;

        /// <summary>
        /// Parametresiz Cetvel nesnesi oluşturucu yordam
        /// </summary>
        public Cetvel()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            hesapPlanKod = string.Empty;
            hesapPlanAd = string.Empty;
            bolgeKod = 0;
            bolgeAd = string.Empty;
            ilKod = string.Empty;
            ilAd = string.Empty;
            ilceKod = string.Empty;
            ilceAd = string.Empty;
            detay = new TNSCollection();
            tarih1 = new TNSDateTime();
            tarih2 = new TNSDateTime();
            seviye = 0;
            harcamaZorunluDegil = false;
        }
    }

    /// <summary>
    /// Taşınır cetvellerinin detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class CetvelDetay
    {
        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi kodu
        /// </summary>
        public int olcuBirimKod;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi adı
        /// </summary>
        public string olcuBirimAd;

        /// <summary>
        /// Önceki yıldan devreden toplam malzeme adedini ve tutarını tutan nesne
        /// </summary>
        public CetvelDetayRakam gecenYildanDevreden;

        /// <summary>
        /// Yıl içinde alınan, gelen toplam malzeme adedini ve tutarını tutan nesne
        /// </summary>
        public CetvelDetayRakam yilIcindeGiren;

        /// <summary>
        /// Önceki yıldan devreden ve yıl içinde alınan, gelen toplam malzeme adedini ve tutarını tutan nesne
        /// </summary>
        public CetvelDetayRakam toplam;

        /// <summary>
        /// Yıl içinde verilen, giden toplam malzeme adedini ve tutarını tutan nesne
        /// </summary>
        public CetvelDetayRakam yilIcindeCikan;

        /// <summary>
        /// Gelecek yıla devreden, kalan toplam malzeme adedini ve tutarını tutan nesne
        /// </summary>
        public CetvelDetayRakam gelecekYilaDevir;

        /// <summary>
        /// Muhasebe birimi kodu (Sayıştay raporlaması için)
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı (Sayıştay raporlaması için)
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu 
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı 
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Parametresiz CetvelDetay nesnesi oluşturucu yordam
        /// </summary>
        public CetvelDetay()
        {
            hesapPlanKod = string.Empty;
            hesapPlanAd = string.Empty;
            olcuBirimKod = 0;
            olcuBirimAd = string.Empty;
            gecenYildanDevreden = new CetvelDetayRakam();
            yilIcindeGiren = new CetvelDetayRakam();
            toplam = new CetvelDetayRakam();
            yilIcindeCikan = new CetvelDetayRakam();
            gelecekYilaDevir = new CetvelDetayRakam();
            muhasebeKod = "";
            muhasebeAd = "";
            harcamaKod = "";
            harcamaAd = "";
        }
    }

    /// <summary>
    /// Taşınır cetvellerinin detay bilgilerinin malzeme adedini ve tutarını tutan sınıf
    /// </summary>
    [Serializable()]
    public class CetvelDetayRakam
    {
        /// <summary>
        /// Malzeme adedi
        /// </summary>
        public decimal miktar;

        /// <summary>
        /// Malzeme tutarı
        /// </summary>
        public decimal tutar;

        /// <summary>
        /// Parametresiz CetvelDetayRakam nesnesi oluşturucu yordam
        /// </summary>
        public CetvelDetayRakam()
        {
            miktar = (decimal)0;
            tutar = (decimal)0;
        }
    }

    /// <summary>
    /// Kayıttan düşme teklif ve onay tutanağının üst bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class KayittanDusmeForm
    {
        /// <summary>
        /// Tutanak yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Tutanağın programatik numarası
        /// </summary>
        public string tutanakNo;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string ilAd;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilceAd;

        /// <summary>
        /// Tutanak tarihi
        /// </summary>
        public TNSDateTime tutanakTarihi;

        /// <summary>
        /// Kayıttan düşme teklif ve onay tutanağının detay bilgilerini tutan nesne
        /// </summary>
        public TNSCollection detay;

        /// <summary>
        /// Parametresiz KayittanDusmeForm nesnesi oluşturucu yordam
        /// </summary>
        public KayittanDusmeForm()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            tutanakNo = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            ilKod = string.Empty;
            ilAd = string.Empty;
            ilceKod = string.Empty;
            ilceAd = string.Empty;
            tutanakTarihi = new TNSDateTime();
            detay = new TNSCollection();
        }
    }

    /// <summary>
    /// Kayıttan düşme teklif ve onay tutanağının detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class KayittanDusmeDetay
    {
        /// <summary>
        /// Tutanak yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Tutanağın programatik numarası
        /// </summary>
        public string tutanakNo;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd;

        /// <summary>
        /// Malzemenin programatik sicil numarası
        /// </summary>
        public int prSicilNo;

        /// <summary>
        /// Malzemenin sicil numarası
        /// </summary>
        public string gorSicilNo;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi kodu
        /// </summary>
        public int olcuBirimKod;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi adı
        /// </summary>
        public string olcuBirimAd;

        /// <summary>
        /// Malzemenin adedi
        /// </summary>
        public decimal miktar;

        /// <summary>
        /// KDV oranı
        /// </summary>
        public int kdvOran;

        /// <summary>
        /// Malzemenin birim fiyatı
        /// </summary>
        public decimal birimFiyat;

        /// <summary>
        /// Malzemenin toplam tutarı
        /// </summary>
        public decimal tutar;

        /// <summary>
        /// Malzemenin satır numarası
        /// </summary>
        public int siraNo;

        /// <summary>
        /// Parametresiz KayittanDusmeDetay nesnesi oluşturucu yordam
        /// </summary>
        public KayittanDusmeDetay()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            tutanakNo = string.Empty;
            hesapPlanKod = string.Empty;
            hesapPlanAd = string.Empty;
            prSicilNo = 0;
            gorSicilNo = string.Empty;
            olcuBirimKod = 0;
            olcuBirimAd = string.Empty;
            miktar = (decimal)0;
            kdvOran = 0;
            birimFiyat = (decimal)0;
            tutar = (decimal)0;
            siraNo = 0;
        }
    }

    /// <summary>
    /// Zimmet fişinin üst bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ZimmetForm
    {
        /// <summary>
        /// Belge yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string ilAd;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilceAd;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaBirimKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaBirimAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Belge numarası
        /// </summary>
        public string fisNo;

        /// <summary>
        /// Belge tarihi
        /// </summary>
        public TNSDateTime fisTarih;

        /// <summary>
        /// Zimmet verilecek malzemelerin tipi
        /// ENUMZimmetTipi listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int tip;

        /// <summary>
        /// Belgede geçen malzemelerin verildiği personelin T.C. kimlik numarası
        /// </summary>
        public string kimeGitti;

        /// <summary>
        /// Belgede geçen malzemelerin verildiği personelin adı
        /// </summary>
        public string kisiAdi;

        /// <summary>
        /// Belgede geçen malzemelerin verildiği odanın kodu
        /// </summary>
        public string nereyeGitti;

        /// <summary>
        /// Malzemelerin verildiği mi alındığı mı bilgisi
        /// ENUMZimmetVermeDusme listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int vermeDusme;

        /// <summary>
        /// Malzemelerin zimmetten düşme tarihi
        /// </summary>
        public TNSDateTime dusmeTarih;

        /// <summary>
        /// Belge ile ilgili işlemi yapan kullanıcının kodu
        /// </summary>
        public string islemYapan;

        /// <summary>
        /// Belge ile ilgili işlemin yapılış tarihi ve zamanı
        /// </summary>
        public TNSDateTime islemTarih;

        /// <summary>
        /// Belgenin türü
        /// ENUMZimmetBelgeTur listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int belgeTur;

        /// <summary>
        /// Belgenin durumu
        /// ENUMBelgeDurumu listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int durum;
        public string ipAdresi = "";

        public bool aktarim = false;

        public ObjectArray sorguDetay = new ObjectArray();

        /// <summary>
        /// Parametresiz ZimmetForm nesnesi oluşturucu yordam
        /// </summary>
        public ZimmetForm()
        {
            yil = 0;
            ilKod = string.Empty;
            ilAd = string.Empty;
            ilceKod = string.Empty;
            ilceAd = string.Empty;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaBirimKod = string.Empty;
            harcamaBirimAd = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            fisNo = string.Empty;
            fisTarih = new TNSDateTime();
            tip = 0;
            kimeGitti = string.Empty;
            kisiAdi = string.Empty;
            nereyeGitti = string.Empty;
            vermeDusme = 0;
            dusmeTarih = new TNSDateTime();
            islemYapan = string.Empty;
            islemTarih = new TNSDateTime();
            belgeTur = 0;
            durum = 0;
            aktarim = false;
        }

        /// <summary>
        /// Belgenin durumuna ilişkin açıklama bilgisini döndüren yordam
        /// </summary>
        /// <param name="durum">ENUMBelgeDurumu listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Belge durumunun açıklaması döndürülür.</returns>
        public static string FormDurumAciklama(int durum)
        {
            if (durum == (int)ENUMBelgeDurumu.YENI) return TasinirMalResI.TMM007;
            else if (durum == (int)ENUMBelgeDurumu.DEGISTIRILDI) return TasinirMalResI.TMM008;
            else if (durum == (int)ENUMBelgeDurumu.ONAYLI) return TasinirMalResI.TMM009;
            else if (durum == (int)ENUMBelgeDurumu.ONAYKALDIR) return TasinirMalResI.TMM010;
            else if (durum == (int)ENUMBelgeDurumu.IPTAL) return TasinirMalResI.TMM011;
            else
                return string.Format(TasinirMalResI.TMM012, durum);
        }
    }

    /// <summary>
    /// Zimmet fişinin detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ZimmetDetay
    {
        /// <summary>
        /// Belge yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaBirimKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaBirimAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Belge numarası
        /// </summary>
        public string fisNo;

        /// <summary>
        /// Belgenin türü
        /// ENUMZimmetBelgeTur listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int belgeTur;

        /// <summary>
        /// Malzemenin detayda yer aldığı satır numarası
        /// </summary>
        public int siraNo;

        /// <summary>
        /// Malzemenin programatik sicil numarası
        /// </summary>
        public int prSicilNo;

        /// <summary>
        /// Malzemenin sicil numarası
        /// </summary>
        public string gorSicilNo;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd;

        /// <summary>
        /// Malzemeyle ilgili zimmet açıklaması
        /// </summary>
        public string ozellik;

        /// <summary>
        /// KDV oranı
        /// </summary>
        public int kdvOran;

        /// <summary>
        /// Malzemenin birim fiyatı
        /// </summary>
        public decimal birimFiyat;

        /// <summary>
        /// Malzemenin zimmet edilirken ki durumu
        /// </summary>
        public string teslimDurum;

        /// <summary>
        /// Malzemenin özellik bilgilerini tutan nesne
        /// Barkod yazdırmasında gerekli
        /// </summary>
        public SicilNoOzellik ozellikSicil;

        /// <summary>
        /// Malzemenin özellik bilgilerini tutan nesne
        /// Eski zimmet bilgisi yazdırmasında gerekli
        /// </summary>
        public SicilNoOzellik ozellikSicilEski;

        /// <summary>
        /// Malzemenin RFID numarası
        /// </summary>
        public int rfIdNo;

        /// <summary>
        /// Parametresiz ZimmetDetay nesnesi oluşturucu yordam
        /// </summary>
        public ZimmetDetay()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaBirimKod = string.Empty;
            harcamaBirimAd = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            fisNo = string.Empty;
            belgeTur = 0;
            siraNo = 0;
            prSicilNo = 0;
            gorSicilNo = string.Empty;
            hesapPlanKod = string.Empty;
            hesapPlanAd = string.Empty;
            ozellik = string.Empty;
            kdvOran = 0;
            birimFiyat = (decimal)0;
            teslimDurum = string.Empty;
            ozellikSicil = new SicilNoOzellik();
            ozellikSicilEski = new SicilNoOzellik();
            rfIdNo = 0;
        }
    }


    /// <summary>
    /// Zimmet fişi EOnay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ZimmetFormEOnay
    {
        /// <summary>
        /// Belge yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaBirimKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaBirimAd;

        /// <summary>
        /// Belge numarası
        /// </summary>
        public string fisNo;

        /// <summary>
        /// Belge tarihi
        /// </summary>
        public TNSDateTime fisTarih;

        /// <summary>
        /// Belgenin türü
        /// ENUMZimmetBelgeTur listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int belgeTur;

        /// <summary>
        /// EOnay için mail gönderen personelin T.C. kimlik numarası
        /// </summary>
        public string emailKimden;

        /// <summary>
        /// EOnay için mail gönderen personelin Ad bilgisi
        /// </summary>
        public string emailKimdenAd;

        /// <summary>
        /// EOnay için mail gönderilen personelin T.C. kimlik numarası
        /// </summary>
        public string emailKime;

        /// <summary>
        /// EOnay için mail gönderilen personelin Ad bilgisi
        /// </summary>
        public string emailKimeAd;

        /// <summary>
        /// EOnay için mail gönderme tarihi
        /// </summary>
        public TNSDateTime emailGondermeTarih;

        /// <summary>
        /// EOnay için gönderilen maile verilan cevap tarihi
        /// </summary>
        public TNSDateTime emailCevapTarih;

        /// <summary>
        /// Belgenin durumu
        /// ENUMEOnayDurumu listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int durum;

        /// <summary>
        /// Parametresiz ZimmetForm nesnesi oluşturucu yordam
        /// </summary>
        public ZimmetFormEOnay()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaBirimKod = string.Empty;
            harcamaBirimAd = string.Empty;
            fisNo = string.Empty;
            fisTarih = new TNSDateTime();
            belgeTur = 0;
            emailKimden = string.Empty;
            emailKimdenAd = string.Empty;
            emailKime = string.Empty;
            emailKimeAd = string.Empty;
            emailGondermeTarih = new TNSDateTime();
            emailCevapTarih = new TNSDateTime();
            durum = 0;
        }

        /// <summary>
        /// Belgenin eonay durumuna ilişkin açıklama bilgisini döndüren yordam
        /// </summary>
        /// <param name="durum">ENUMEOnayDurumu listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Belge eonay durumunun açıklaması döndürülür.</returns>
        public static string FormEOnayDurumAciklama(int durum)
        {
            if (durum == (int)ENUMEOnayDurumu.GONDERILMEDI) return TasinirMalResI.TMM403;
            else if (durum == (int)ENUMEOnayDurumu.GONDERILDI) return TasinirMalResI.TMM404;
            else if (durum == (int)ENUMEOnayDurumu.CEVAPLADIKABUL) return TasinirMalResI.TMM405;
            else if (durum == (int)ENUMEOnayDurumu.CEVAPLADIRED) return TasinirMalResI.TMM406;
            else
                return string.Format(TasinirMalResI.TMM012, durum);
        }
    }

    /// <summary>
    /// Zimmet fişinin üst bilgilerini ve EOnay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ZimmetFormWithEonay
    {
        /// <summary>
        /// Belge yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string ilAd;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilceAd;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaBirimKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaBirimAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Belge numarası
        /// </summary>
        public string fisNo;

        /// <summary>
        /// Belge tarihi
        /// </summary>
        public TNSDateTime fisTarih;

        /// <summary>
        /// Zimmet verilecek malzemelerin tipi
        /// ENUMZimmetTipi listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int tip;

        /// <summary>
        /// Belgede geçen malzemelerin verildiği personelin T.C. kimlik numarası
        /// </summary>
        public string kimeGitti;

        /// <summary>
        /// Belgede geçen malzemelerin verildiği personelin adı
        /// </summary>
        public string kisiAdi;

        /// <summary>
        /// Belgede geçen malzemelerin verildiği odanın kodu
        /// </summary>
        public string nereyeGitti;

        /// <summary>
        /// Malzemelerin verildiği mi alındığı mı bilgisi
        /// ENUMZimmetVermeDusme listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int vermeDusme;

        /// <summary>
        /// Malzemelerin zimmetten düşme tarihi
        /// </summary>
        public TNSDateTime dusmeTarih;

        /// <summary>
        /// Belge ile ilgili işlemi yapan kullanıcının kodu
        /// </summary>
        public string islemYapan;

        /// <summary>
        /// Belge ile ilgili işlemin yapılış tarihi ve zamanı
        /// </summary>
        public TNSDateTime islemTarih;

        /// <summary>
        /// Belgenin türü
        /// ENUMZimmetBelgeTur listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int belgeTur;

        /// <summary>
        /// Belgenin durumu
        /// ENUMBelgeDurumu listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int durum;

        /// <summary>
        /// EOnay için mail gönderen personelin T.C. kimlik numarası
        /// </summary>
        public string emailKimden;

        /// <summary>
        /// EOnay için mail gönderen personelin Ad bilgisi
        /// </summary>
        public string emailKimdenAd;

        /// <summary>
        /// EOnay için mail gönderilen personelin T.C. kimlik numarası
        /// </summary>
        public string emailKime;

        /// <summary>
        /// EOnay için mail gönderilen personelin Ad bilgisi
        /// </summary>
        public string emailKimeAd;

        /// <summary>
        /// EOnay için mail gönderme tarihi
        /// </summary>
        public TNSDateTime emailGondermeTarih;

        /// <summary>
        /// EOnay için gönderilen maile verilan cevap tarihi
        /// </summary>
        public TNSDateTime emailCevapTarih;

        /// <summary>
        /// Belgenin durumu
        /// ENUMEOnayDurumu listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int emaildurum;

        /// <summary>
        /// Parametresiz ZimmetForm nesnesi oluşturucu yordam
        /// </summary>
        public ZimmetFormWithEonay()
        {
            yil = 0;
            ilKod = string.Empty;
            ilAd = string.Empty;
            ilceKod = string.Empty;
            ilceAd = string.Empty;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaBirimKod = string.Empty;
            harcamaBirimAd = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            fisNo = string.Empty;
            fisTarih = new TNSDateTime();
            tip = 0;
            kimeGitti = string.Empty;
            kisiAdi = string.Empty;
            nereyeGitti = string.Empty;
            vermeDusme = 0;
            dusmeTarih = new TNSDateTime();
            islemYapan = string.Empty;
            islemTarih = new TNSDateTime();
            belgeTur = 0;
            durum = 0;
            emailKimden = string.Empty;
            emailKimdenAd = string.Empty;
            emailKime = string.Empty;
            emailKimeAd = string.Empty;
            emailGondermeTarih = new TNSDateTime();
            emailCevapTarih = new TNSDateTime();
            emaildurum = 0;
        }

        /// <summary>
        /// Belgenin durumuna ilişkin açıklama bilgisini döndüren yordam
        /// </summary>
        /// <param name="durum">ENUMBelgeDurumu listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Belge durumunun açıklaması döndürülür.</returns>
        public static string FormDurumAciklama(int durum)
        {
            if (durum == (int)ENUMBelgeDurumu.YENI) return TasinirMalResI.TMM007;
            else if (durum == (int)ENUMBelgeDurumu.DEGISTIRILDI) return TasinirMalResI.TMM008;
            else if (durum == (int)ENUMBelgeDurumu.ONAYLI) return TasinirMalResI.TMM009;
            else if (durum == (int)ENUMBelgeDurumu.ONAYKALDIR) return TasinirMalResI.TMM010;
            else if (durum == (int)ENUMBelgeDurumu.IPTAL) return TasinirMalResI.TMM011;
            else
                return string.Format(TasinirMalResI.TMM012, durum);
        }

        /// <summary>
        /// Belgenin eonay durumuna ilişkin açıklama bilgisini döndüren yordam
        /// </summary>
        /// <param name="durum">ENUMEOnayDurumu listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Belge eonay durumunun açıklaması döndürülür.</returns>
        public static string FormEOnayDurumAciklama(int emaildurum)
        {
            if (emaildurum == (int)ENUMEOnayDurumu.GONDERILMEDI) return TasinirMalResI.TMM403;
            else if (emaildurum == (int)ENUMEOnayDurumu.GONDERILDI) return TasinirMalResI.TMM404;
            else if (emaildurum == (int)ENUMEOnayDurumu.CEVAPLADIKABUL) return TasinirMalResI.TMM405;
            else if (emaildurum == (int)ENUMEOnayDurumu.CEVAPLADIRED) return TasinirMalResI.TMM406;
            else
                return string.Format(TasinirMalResI.TMM012, emaildurum);
        }
    }

    /// <summary>
    /// Taşınır demirbaşının hareket bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class SicilNoHareket
    {
        /// <summary>
        /// Demirbaşın programatik sicil numarası
        /// </summary>
        public int prSicilNo;

        /// <summary>
        /// Hareketin sıra numarası
        /// </summary>
        public int siraNo;

        /// <summary>
        /// Hareketin yapıldığı yıl
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaBirimKod;

        /// <summary>
        /// Taşınır işlem fişi veya zimmet fişi belge numarası
        /// </summary>
        public string fisNo;

        /// <summary>
        /// Demirbaşın fişte yer aldığı satır numarası
        /// </summary>
        public int fisSiraNo;

        /// <summary>
        /// Demirbaşın sicil numarası
        /// </summary>
        public string sicilNo;

        /// <summary>
        /// Demirbaşın sicil sıra numarası
        /// </summary>
        public string sicilSiraNo;

        /// <summary>
        /// Demirbaşın taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod;

        /// <summary>
        /// Demirbaşın taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd;
        public double amortismanYil = 0;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Demirbaşın birim fiyatı
        /// </summary>
        public decimal fiyat;

        public decimal sorguFiyat1;

        public decimal sorguFiyat2;

        /// <summary>
        /// KDV oranı
        /// </summary>
        public int kdvOran;

        /// <summary>
        /// Hareketin işlem tipinin kodu
        /// </summary>
        public int islemTipKod;

        /// <summary>
        /// Hareketin işlem türü
        /// ENUMIslemTipi listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int islemTurKod;

        /// <summary>
        /// Hareket ile ilgili işlemin yapılış tarihi ve zamanı
        /// </summary>
        public TNSDateTime islemTarih;

        /// <summary>
        /// Hareket ile ilgili işlemi yapan kullanıcının kodu
        /// </summary>
        public string islemYapan;

        /// <summary>
        /// Demirbaşın durumu
        /// ENUMSicilDurum listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int durum;

        /// <summary>
        /// Demirbaşın eski sicil numarası
        /// </summary>
        public string eSicilNo;

        /// <summary>
        /// Eski muhasebe birimi kodu
        /// </summary>
        public string eMuhasebeKod;

        /// <summary>
        /// Eski harcama birimi kodu
        /// </summary>
        public string eHarcamaBirimKod;

        /// <summary>
        /// Hareketin işlem tipinin adı
        /// </summary>
        public string islemTipiAd;

        /// <summary>
        /// Demirbaşın özellik bilgilerini tutan nesne
        /// </summary>
        public SicilNoOzellik ozellik;

        /// <summary>
        /// Demirbaşın zimmet verildiği personelin T.C. kimlik numarası
        /// </summary>
        public string personelKod;

        /// <summary>
        /// Demirbaşın zimmet verildiği odanın kodu
        /// </summary>
        public string odaKod;

        /// <summary>
        /// Hareketin yapıldığı belgenin tarihi
        /// </summary>
        public TNSDateTime fisTarih;

        /// <summary>
        /// Demirbaşın alındığı yer bilgisi
        /// </summary>
        public string neredenGeldi;

        /// <summary>
        /// Demirbaşın verildiği yer bilgisi
        /// </summary>
        public string nereyeGitti;

        /// <summary>
        /// Demirbaşın verildiği kişi bilgisi
        /// </summary>
        public string kimeGitti;

        /// <summary>
        /// Harekete ait zimmet açıklaması
        /// </summary>
        public string zimmetOzellik;

        /// <summary>
        /// Demirbaşın KDV'li birim fiyatı
        /// </summary>
        public decimal kdvliBirimFiyat;

        /// <summary>
        /// Demirbaşa uygulanmış değer artışı tutarı
        /// </summary>
        public decimal degerArtisi;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaBirimAd;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Demirbaşın zimmet verildiği personelin adı
        /// </summary>
        public string kisiAd;

        /// <summary>
        /// Demirbaşın zimmet verildiği odanın adı
        /// </summary>
        public string odaAd;

        /// <summary>
        /// Demirbaşı gönderen/Demirbaşın gönderildiği muhasebe birimi kodu
        /// </summary>
        public string gonMuhasebeKod;

        /// <summary>
        /// Demirbaşı gönderen/Demirbaşın gönderildiği harcama birimi kodu
        /// </summary>
        public string gonHarcamaBirimKod;

        /// <summary>
        /// Demirbaşı gönderen/Demirbaşın gönderildiği ambar kodu
        /// </summary>
        public string gonAmbarKod;

        /// <summary>
        /// Demirbaşı gönderen/Demirbaşın gönderildiği muhasebe birimi adı
        /// </summary>
        public string gonMuhasebeAd;

        /// <summary>
        /// Demirbaşı gönderen/Demirbaşın gönderildiği harcama birimi adı
        /// </summary>
        public string gonHarcamaBirimAd;

        /// <summary>
        /// Demirbaşı gönderen/Demirbaşın gönderildiği ambar adı
        /// </summary>
        public string gonAmbarAd;

        /// <summary>
        /// Demirbaş adedi
        /// </summary>
        public int miktar;

        /// <summary>
        /// Demirbaşın ilk hareket tarihi
        /// </summary>
        public TNSDateTime ilkGirisTarih;

        /// <summary>
        /// Demirbaşın ilk hareketinin işlem tipi bilgilerini tutan nesne
        /// </summary>
        public IslemTip ilkGirisIslemTip;

        /// <summary>
        /// Demirbaşın RFID numarası
        /// </summary>
        public int rfIdNo;
        public string epc;//önceden tanımlanmış (İMKB) olan rfIdNo alanı int olduğu için string alana ihtiyaç var.

        /// <summary>
        /// Demirbaşın RFID numarasını kill edecek numara
        /// </summary>
        public int killNo;

        /// <summary>
        /// Demirbaşın RFID numarasına erişim anahtarı
        /// </summary>
        public int accessNo;

        /// <summary>
        /// Demirbaş kapılardan çıkabilir mi çıkamaz mı bilgisi
        /// </summary>
        public bool kapiGecis;

        /// <summary>
        /// Demirbaşın RFID etiket tipi
        /// </summary>
        public int tagType;

        public string islemDurumAciklama = "";

        public string sorguFisNoTif;
        public string sorguFisNoZimmet;
        public string sorguPrSicilNoListe = "";
        public string sorguHaricPrSicilNoListe = "";
        public bool sorguSicilNoIcindeGecen = false;
        public string sorguButunSicilNolardaAra = "";
        public bool sorguEskiSicilNoAramaLike = true;

        public string listeYazdir = "";

        public string sicilNoAciklama;

        public int[] sicilNolar;

        /// <summary>
        /// Veri aktarımı yapılırken, kontrol gerektiren yerlerde kullanılmak üzere eklendi
        /// </summary>
        public bool aktarim = false;
        public bool khkHaric = false;
        public bool sadeceBirimdeOlanlar = false;
        public bool altOdalar = false;

        /// <summary>
        /// Parametresiz SicilNoHareket nesnesi oluşturucu yordam
        /// </summary>
        public SicilNoHareket()
        {
            prSicilNo = 0;
            siraNo = 0;
            yil = 0;
            muhasebeKod = string.Empty;
            harcamaBirimKod = string.Empty;
            fisNo = string.Empty;
            fisSiraNo = 0;
            sicilNo = string.Empty;
            sicilSiraNo = string.Empty;
            hesapPlanKod = string.Empty;
            hesapPlanAd = string.Empty;
            ambarKod = string.Empty;
            fiyat = (decimal)0;
            kdvOran = 0;
            islemTipKod = 0;
            islemTurKod = 0;
            islemTarih = new TNSDateTime();
            islemYapan = string.Empty;
            durum = 0;
            eSicilNo = "";
            eMuhasebeKod = "";
            eHarcamaBirimKod = "";
            islemTipiAd = "";
            ozellik = new SicilNoOzellik();
            personelKod = string.Empty;
            odaKod = string.Empty;
            fisTarih = new TNSDateTime();
            neredenGeldi = string.Empty;
            nereyeGitti = string.Empty;
            kimeGitti = string.Empty;
            zimmetOzellik = string.Empty;
            kdvliBirimFiyat = (decimal)0;
            degerArtisi = (decimal)0;
            muhasebeAd = "";
            harcamaBirimAd = "";
            ambarAd = "";
            kisiAd = "";
            odaAd = "";
            gonMuhasebeKod = "";
            gonHarcamaBirimKod = "";
            gonAmbarKod = "";
            gonMuhasebeAd = "";
            gonHarcamaBirimAd = "";
            gonAmbarAd = "";
            miktar = 0;
            ilkGirisTarih = new TNSDateTime();
            ilkGirisIslemTip = new IslemTip();

            rfIdNo = 0;
            epc = "";
            killNo = 0;
            accessNo = 0;
            kapiGecis = false;
            tagType = 0;
            sorguFisNoTif = "";
            sorguFisNoZimmet = "";

            sorguFiyat1 = 0;
            sorguFiyat2 = 0;

            sicilNoAciklama = "";

            sicilNolar = new int[0];
            khkHaric = false;
        }
    }


    /// <summary>
    /// Zimmet fişlerinin sorgulanmasında kullanılan kriter bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ZimmetFormKriter
    {
        /// <summary>
        /// Başlangıç belge numarası
        /// Aralık belirtilmesi gereken durumlarda belgeNoBasla aralığın alt sınırını ifade eder.
        /// </summary>
        public string belgeNoBasla;

        /// <summary>
        /// Bitiş belge numarası
        /// Aralık belirtilmesi gereken durumlarda belgeNoBit aralığın üst sınırını ifade eder.
        /// </summary>
        public string belgeNoBit;

        /// <summary>
        /// Başlangıç belge tarihi
        /// Aralık belirtilmesi gereken durumlarda belgeTarihBasla aralığın alt sınırını ifade eder.
        /// </summary>
        public TNSDateTime belgeTarihBasla;

        /// <summary>
        /// Bitiş belge tarihi
        /// Aralık belirtilmesi gereken durumlarda belgeTarihBit aralığın üst sınırını ifade eder.
        /// </summary>
        public TNSDateTime belgeTarihBit;

        /// <summary>
        /// Malzemenin sicil numarası
        /// </summary>
        public string sicilNo;

        /// <summary>
        /// Belge ile ilgili işlemi yapan kullanıcının kodu
        /// </summary>
        public string islemYapan;

        /// <summary>
        /// Belgenin durumu
        /// ENUMBelgeDurumu listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int durum;

        /// <summary>
        /// Belgede geçen malzemelerin zimmet verildiği personelin T.C. kimlik numarası
        /// </summary>
        public string kime;

        /// <summary>
        /// Belgede geçen malzemelerin zimmet verildiği odanın kodu
        /// </summary>
        public string nereye;

        /// <summary>
        /// Parametresiz ZimmetFormKriter nesnesi oluşturucu yordam
        /// </summary>
        public ZimmetFormKriter()
        {
            belgeNoBasla = string.Empty;
            belgeNoBit = string.Empty;
            belgeTarihBasla = null;
            belgeTarihBit = null;
            sicilNo = string.Empty;
            islemYapan = string.Empty;
            durum = 0;
            kime = "";
            nereye = "";
        }
    }


    /// <summary>
    /// Zimmet Form EOnay sorgulanmasında kullanılan kriter bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ZimmetFormEOnayKriter
    {
        /// <summary>
        /// Belge yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaBirimKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaBirimAd;

        /// <summary>
        /// Belge numarası
        /// </summary>
        public string fisNo;

        /// <summary>
        /// Belge tarihi
        /// </summary>
        public TNSDateTime fisTarih;

        /// <summary>
        /// Belgenin türü
        /// ENUMEOnayTur listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int belgeTur;

        /// <summary>
        /// Başlangıç eonay gönderme tarihi
        /// Aralık belirtilmesi gereken durumlarda belgeTarihBasla aralığın alt sınırını ifade eder.
        /// </summary>
        public TNSDateTime eonayGondermeTarihBasla;

        /// <summary>
        /// Bitiş eonay gönderme tarihi
        /// Aralık belirtilmesi gereken durumlarda belgeTarihBit aralığın üst sınırını ifade eder.
        /// </summary>
        public TNSDateTime eonayGondermeTarihBitis;

        /// <summary>
        /// Başlangıç eonay cevap tarihi
        /// Aralık belirtilmesi gereken durumlarda belgeTarihBasla aralığın alt sınırını ifade eder.
        /// </summary>
        public TNSDateTime eonayCevapTarihBasla;

        /// <summary>
        /// Bitiş eonay cevap tarihi
        /// Aralık belirtilmesi gereken durumlarda belgeTarihBit aralığın üst sınırını ifade eder.
        /// </summary>
        public TNSDateTime eonayCevapTarihBitis;

        /// <summary>
        /// Belge eonay için gönderen kullanıcının kodu
        /// </summary>
        public string emailKimden;

        /// <summary>
        /// Belge eonay cevap veren kullanıcının kodu
        /// </summary>
        public string emailKime;

        /// <summary>
        /// Belgenin durumu
        /// ENUMBelgeDurumu listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int durum;

        /// <summary>
        /// Parametresiz ZimmetFormEOnayKriter nesnesi oluşturucu yordam
        /// </summary>
        public ZimmetFormEOnayKriter()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaBirimKod = string.Empty;
            harcamaBirimAd = string.Empty;
            fisNo = string.Empty;
            fisTarih = new TNSDateTime();
            belgeTur = 0;
            eonayGondermeTarihBasla = null;
            eonayGondermeTarihBitis = null;
            eonayCevapTarihBasla = null;
            eonayCevapTarihBitis = null;
            emailKimden = string.Empty;
            emailKime = string.Empty;
            durum = 0;
        }
    }

    /// <summary>
    /// Tüketim malzemeleri defterinin üst bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class TMDBilgi
    {
        /// <summary>
        /// İlin adı
        /// </summary>
        public string ilAd;

        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilceAd;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapAd;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi kodu
        /// </summary>
        public int olcuBirimKod;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi adı
        /// </summary>
        public string olcuBirimAd;

        /// <summary>
        /// Tüketim malzemeleri defterinin detay bilgilerini tutan nesne
        /// </summary>
        public TNSCollection detaylar;

        /// <summary>
        /// Parametresiz TMDBilgi nesnesi oluşturucu yordam
        /// </summary>
        public TMDBilgi()
        {
            ilKod = string.Empty;
            ilAd = string.Empty;
            ilceKod = string.Empty;
            ilceAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            hesapKod = string.Empty;
            hesapAd = string.Empty;
            olcuBirimKod = 0;
            olcuBirimAd = string.Empty;
            detaylar = new TNSCollection();
        }
    }

    /// <summary>
    /// Tüketim malzemeleri defterinin detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class TMDBilgiDetay
    {
        /// <summary>
        /// Detay bilgisinin sıra numarası
        /// </summary>
        public int siraNo;

        /// <summary>
        /// Taşınır işlem fişinin belge tarihi
        /// </summary>
        public TNSDateTime tifTarih;

        /// <summary>
        /// Taşınır işlem fişinin belge numarası
        /// </summary>
        public string tifNo;

        /// <summary>
        /// İşlem tipi bilgilerini tutan nesne
        /// </summary>
        public IslemTip islemTipi;

        /// <summary>
        /// Malzemenin adedi
        /// </summary>
        public decimal miktar;

        /// <summary>
        /// Malzemenin KDV'li birim fiyatı
        /// </summary>
        public decimal birimFiyatKDVLi;

        /// <summary>
        /// Malzemenin toplam tutarı
        /// </summary>
        public decimal tutar;

        /// <summary>
        /// Malzemenin verildiği yer bilgisi
        /// </summary>
        public string nereyeGitti;

        /// <summary>
        /// Malzemenin alındığı yer bilgisi
        /// </summary>
        public string neredenGeldi;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği muhasebe birimi kodu
        /// </summary>
        public string gonMuhasebeKod;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği muhasebe birimi adı
        /// </summary>
        public string gonMuhasebeAd;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği harcama birimi kodu
        /// </summary>
        public string gonHarcamaKod;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği harcama birimi adı
        /// </summary>
        public string gonHarcamaAd;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği ambar kodu
        /// </summary>
        public string gonAmbarKod;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği ambar adı
        /// </summary>
        public string gonAmbarAd;

        /// <summary>
        /// Malzemenin verildiği kişi bilgisi
        /// </summary>
        public string kimeGitti;

        /// <summary>
        /// Parametresiz TMDBilgiDetay nesnesi oluşturucu yordam
        /// </summary>
        public TMDBilgiDetay()
        {
            siraNo = 0;
            tifTarih = new TNSDateTime();
            tifNo = string.Empty;
            islemTipi = new IslemTip();
            miktar = 0;
            birimFiyatKDVLi = (decimal)0;
            tutar = (decimal)0;
            nereyeGitti = string.Empty;
            neredenGeldi = string.Empty;
            gonMuhasebeKod = string.Empty;
            gonMuhasebeAd = string.Empty;
            gonHarcamaKod = string.Empty;
            gonHarcamaAd = string.Empty;
            gonAmbarKod = string.Empty;
            gonAmbarAd = string.Empty;
            kimeGitti = string.Empty;
        }
    }

    /// <summary>
    /// Dayanıklı taşınırlar defterinin üst bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class DTDBilgi
    {
        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string ilAd;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilceAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapAd;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi kodu
        /// </summary>
        public int olcuBirimKod;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi adı
        /// </summary>
        public string olcuBirimAd;

        /// <summary>
        /// Dayanıklı taşınırlar defterinin detay bilgilerini tutan nesne
        /// </summary>
        public TNSCollection detaylar;

        /// <summary>
        /// Parametresiz DTDBilgi nesnesi oluşturucu yordam
        /// </summary>
        public DTDBilgi()
        {
            ilKod = string.Empty;
            ilAd = string.Empty;
            ilceKod = string.Empty;
            ilceAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            hesapKod = string.Empty;
            hesapAd = string.Empty;
            olcuBirimKod = 0;
            olcuBirimAd = string.Empty;
            detaylar = new TNSCollection();
        }
    }

    /// <summary>
    /// Dayanıklı taşınırlar defterinin detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class DTDBilgiDetay
    {
        /// <summary>
        /// Taşınır işlem veya zimmet fişinin belge tarihi
        /// </summary>
        public TNSDateTime belgeTarih;

        /// <summary>
        /// Taşınır işlem veya zimmet fişinin belge numarası
        /// </summary>
        public string belgeNo;

        /// <summary>
        /// Demirbaşın programatik sicil numarası
        /// </summary>
        public int prSicilNo;

        /// <summary>
        /// Demirbaşın sicil numarası
        /// </summary>
        public string sicilNo;

        /// <summary>
        /// İşlem tipi bilgilerini tutan nesne
        /// </summary>
        public IslemTip islemTipi;

        /// <summary>
        /// Demirbaşın alındığı yer bilgisi
        /// </summary>
        public string neredenGeldi;

        /// <summary>
        /// Demirbaşın KDV'li birim fiyatı
        /// </summary>
        public decimal birimFiyatKDVLi;

        /// <summary>
        /// Malzemenin adedi
        /// Demirbaş olduğu için her zaman 1 olmalıdır.
        /// </summary>
        public decimal miktar;

        /// <summary>
        /// Demirbaşın toplam tutarı
        /// </summary>
        public decimal toplamDeger;

        /// <summary>
        /// İşlemin taşınır işlem mi yoksa zimmet mi olduğu bilgisi
        /// </summary>
        public string cikisCinsi;

        /// <summary>
        /// Demirbaşın zimmet verildiği personelin T.C. kimlik numarası
        /// </summary>
        public string kimeVerildi;

        /// <summary>
        /// Demirbaşın zimmet verildiği odanın kodu
        /// </summary>
        public string nereyeVerildi;

        /// <summary>
        /// Demirbaşın zimmet verildiği odanın kodu
        /// </summary>
        public string nereyeGonderildi;

        /// <summary>
        /// Demirbaşın özellik bilgileri
        /// </summary>
        public string ozellikler;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği muhasebe birimi kodu
        /// </summary>
        public string gonMuhasebeKod;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği muhasebe birimi adı
        /// </summary>
        public string gonMuhasebeAd;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği harcama birimi kodu
        /// </summary>
        public string gonHarcamaKod;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği harcama birimi adı
        /// </summary>
        public string gonHarcamaAd;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği ambar kodu
        /// </summary>
        public string gonAmbarKod;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği ambar adı
        /// </summary>
        public string gonAmbarAd;

        /// <summary>
        /// Demirbaşa uygulanmış değer artışı tutarı
        /// </summary>
        public decimal degerArtisi;

        /// <summary>
        /// Demirbaşa uygulanmış amortisman tutarı
        /// </summary>
        public decimal amortismanTutar;

        /// <summary>
        /// Demirbaşın marka adı
        /// </summary>
        public string markaAd;

        /// <summary>
        /// Demirbaşın model adı
        /// </summary>
        public string modelAd;

        /// <summary>
        /// Demirbaşın seri no
        /// </summary>
        public string seriNo;


        /// <summary>
        /// Parametresiz DTDBilgiDetay nesnesi oluşturucu yordam
        /// </summary>
        public DTDBilgiDetay()
        {
            belgeTarih = new TNSDateTime();
            belgeNo = string.Empty;
            prSicilNo = 0;
            sicilNo = string.Empty;
            islemTipi = new IslemTip();
            neredenGeldi = string.Empty;
            birimFiyatKDVLi = (decimal)0;
            miktar = 0;
            //degerArtis = 0;
            toplamDeger = (decimal)0;
            cikisCinsi = string.Empty;
            kimeVerildi = string.Empty;
            nereyeVerildi = string.Empty;
            nereyeGonderildi = string.Empty;
            ozellikler = string.Empty;
            //cikisDeger = 0;
            //vermeDusme = 0;
            gonMuhasebeKod = string.Empty;
            gonMuhasebeAd = string.Empty;
            gonHarcamaKod = string.Empty;
            gonHarcamaAd = string.Empty;
            gonAmbarKod = string.Empty;
            gonAmbarAd = string.Empty;
            degerArtisi = (decimal)0;
            amortismanTutar = 0;
            markaAd = string.Empty;
            modelAd = string.Empty;
            seriNo = string.Empty;
        }
    }

    /// <summary>
    /// Müze defterinin üst bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class MuzeBilgi
    {
        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string ilAd;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilceAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapAd;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi kodu
        /// </summary>
        public int olcuBirimKod;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi adı
        /// </summary>
        public string olcuBirimAd;

        /// <summary>
        /// Müze defterinin detay bilgilerini tutan nesne
        /// </summary>
        public TNSCollection detaylar;

        /// <summary>
        /// Parametresiz MuzeBilgi nesnesi oluşturucu yordam
        /// </summary>
        public MuzeBilgi()
        {
            ilKod = string.Empty;
            ilAd = string.Empty;
            ilceKod = string.Empty;
            ilceAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            hesapKod = string.Empty;
            hesapAd = string.Empty;
            olcuBirimKod = 0;
            olcuBirimAd = string.Empty;
            detaylar = new TNSCollection();
        }
    }

    /// <summary>
    /// Müze defterinin detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class MuzeBilgiDetay
    {
        /// <summary>
        /// Taşınır işlem veya zimmet fişinin belge tarihi
        /// </summary>
        public TNSDateTime belgeTarih;

        /// <summary>
        /// Taşınır işlem veya zimmet fişinin belge numarası
        /// </summary>
        public string belgeNo;

        /// <summary>
        /// İşlem tipi bilgilerini tutan nesne
        /// </summary>
        public IslemTip islemTipi;

        /// <summary>
        /// Demirbaşın sicil numarası
        /// </summary>
        public string sicilNo;

        /// <summary>
        /// Demirbaşın adı
        /// </summary>
        public string adi;

        /// <summary>
        /// Demirbaşın geliş tarihi
        /// </summary>
        public string gelisTarihi;

        /// <summary>
        /// Demirbaşın alındığı yer bilgisi
        /// </summary>
        public string neredenGeldi;

        /// <summary>
        /// Demirbaşın nereden bulunduğu bilgisi
        /// </summary>
        public string neredeBulundu;

        /// <summary>
        /// Demirbaşın çağıyla ilgili bilgisi
        /// </summary>
        public string cagi;

        /// <summary>
        /// Demirbaşın fiziksel boyutları
        /// </summary>
        public string boyutlari;

        /// <summary>
        /// Demirbaşın durum bilgisi
        /// </summary>
        public string durumuMaddesi;

        /// <summary>
        /// Demirbaşın ön yüzüyle ilgili tanımlayıcı bilgi
        /// </summary>
        public string onYuz;

        /// <summary>
        /// Demirbaşın arka yüzüyle ilgili tanımlayıcı bilgi
        /// </summary>
        public string arkaYuz;

        /// <summary>
        /// Demirbaşın fotograf bilgisi
        /// </summary>
        public string fotograf;

        /// <summary>
        /// Demirbaşa verilmiş puan bilgisi
        /// </summary>
        public string puan;

        /// <summary>
        /// Demirbaşın KDV'li birim fiyatı
        /// </summary>
        public decimal birimFiyatKDVLi;

        /// <summary>
        /// Demirbaşın yeri ve konusuyla ilgili bilgi
        /// </summary>
        public string yeriKonusu;

        /// <summary>
        /// İşlemin taşınır işlem mi yoksa zimmet mi olduğu bilgisi
        /// </summary>
        public string cikisCinsi;

        /// <summary>
        /// Demirbaşın zimmet verildiği odanın kodu
        /// </summary>
        public string nereyeVerildi;

        /// <summary>
        /// Demirbaşın zimmet verildiği personelin T.C. kimlik numarası
        /// </summary>
        public string kimeVerildi;

        /// <summary>
        /// Demirbaşın zimmet verildiği odanın kodu
        /// </summary>
        public string nereyeGonderildi;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği muhasebe birimi kodu
        /// </summary>
        public string gonMuhasebeKod;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği muhasebe birimi adı
        /// </summary>
        public string gonMuhasebeAd;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği harcama birimi kodu
        /// </summary>
        public string gonHarcamaKod;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği harcama birimi adı
        /// </summary>
        public string gonHarcamaAd;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği ambar kodu
        /// </summary>
        public string gonAmbarKod;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği ambar adı
        /// </summary>
        public string gonAmbarAd;

        /// <summary>
        /// Parametresiz MuzeBilgiDetay nesnesi oluşturucu yordam
        /// </summary>
        public MuzeBilgiDetay()
        {
            belgeTarih = new TNSDateTime();
            belgeNo = string.Empty;
            islemTipi = new IslemTip();
            sicilNo = string.Empty;
            adi = string.Empty;
            gelisTarihi = string.Empty;
            neredenGeldi = string.Empty;
            neredeBulundu = string.Empty;
            cagi = string.Empty;
            boyutlari = string.Empty;
            durumuMaddesi = string.Empty;
            onYuz = string.Empty;
            arkaYuz = string.Empty;
            fotograf = string.Empty;
            puan = string.Empty;
            birimFiyatKDVLi = (decimal)0;
            yeriKonusu = string.Empty;
            cikisCinsi = string.Empty;
            nereyeVerildi = string.Empty;
            kimeVerildi = string.Empty;
            nereyeGonderildi = string.Empty;
            gonMuhasebeKod = string.Empty;
            gonMuhasebeAd = string.Empty;
            gonHarcamaKod = string.Empty;
            gonHarcamaAd = string.Empty;
            gonAmbarKod = string.Empty;
            gonAmbarAd = string.Empty;
        }
    }

    /// <summary>
    /// Kütüphane defterinin üst bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class KutuphaneBilgi
    {
        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string ilAd;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilceAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapAd;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi kodu
        /// </summary>
        public int olcuBirimKod;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi adı
        /// </summary>
        public string olcuBirimAd;

        /// <summary>
        /// Kütüphane defterinin detay bilgilerini tutan nesne
        /// </summary>
        public TNSCollection detaylar;

        /// <summary>
        /// Parametresiz KutuphaneBilgi nesnesi oluşturucu yordam
        /// </summary>
        public KutuphaneBilgi()
        {
            ilKod = string.Empty;
            ilAd = string.Empty;
            ilceKod = string.Empty;
            ilceAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            hesapKod = string.Empty;
            hesapAd = string.Empty;
            olcuBirimKod = 0;
            olcuBirimAd = string.Empty;
            detaylar = new TNSCollection();
        }
    }

    /// <summary>
    /// Kütüphane defterinin detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class KutuphaneBilgiDetay
    {
        /// <summary>
        /// Taşınır işlem veya zimmet fişinin belge tarihi
        /// </summary>
        public TNSDateTime belgeTarih;

        /// <summary>
        /// Taşınır işlem veya zimmet fişinin belge numarası
        /// </summary>
        public string belgeNo;

        /// <summary>
        /// İşlem tipi bilgilerini tutan nesne
        /// </summary>
        public IslemTip islemTipi;

        /// <summary>
        /// Demirbaşın sicil numarası
        /// </summary>
        public string sicilNo;

        /// <summary>
        /// Demirbaşın cilt numarası
        /// </summary>
        public string ciltNo;

        /// <summary>
        /// Demirbaşın yazılmış olduğu dil bilgisi
        /// </summary>
        public string dil;

        /// <summary>
        /// Demirbaşın yazarının adı
        /// </summary>
        public string yazarAdi;

        /// <summary>
        /// Demirbaşın adı
        /// </summary>
        public string adi;

        /// <summary>
        /// Demirbaşın yayın yeri
        /// </summary>
        public string yayinYeri;

        /// <summary>
        /// Demirbaşın yayın tarihi
        /// </summary>
        public string yayinTarihi;

        /// <summary>
        /// Demirbaşın alındığı yer bilgisi
        /// </summary>
        public string neredenGeldi;

        /// <summary>
        /// Demirbaşın fiziksel boyutları
        /// </summary>
        public string boyutlari;

        /// <summary>
        /// Demirbaştaki satır sayısı
        /// </summary>
        public string satirSayisi;

        /// <summary>
        /// Demirbaştaki yaprak sayısı
        /// </summary>
        public string yaprakSayisi;

        /// <summary>
        /// Demirbaştaki sayfa sayısı
        /// </summary>
        public string sayfaSayisi;

        /// <summary>
        /// Demirbaşın cildinin türü
        /// </summary>
        public string ciltTuru;

        /// <summary>
        /// Demirbaşın fotograf bilgisi
        /// </summary>
        public string fotograf;

        /// <summary>
        /// Demirbaşın KDV'li birim fiyatı
        /// </summary>
        public decimal birimFiyatKDVLi;

        /// <summary>
        /// Demirbaşın yeri ve konusuyla ilgili bilgi
        /// </summary>
        public string yeriKonusu;

        /// <summary>
        /// İşlemin taşınır işlem mi yoksa zimmet mi olduğu bilgisi
        /// </summary>
        public string cikisCinsi;

        /// <summary>
        /// Demirbaşın zimmet verildiği odanın kodu
        /// </summary>
        public string nereyeVerildi;

        /// <summary>
        /// Demirbaşın zimmet verildiği personelin T.C. kimlik numarası
        /// </summary>
        public string kimeVerildi;

        /// <summary>
        /// Demirbaşın zimmet verildiği odanın kodu
        /// </summary>
        public string nereyeGonderildi;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği muhasebe birimi kodu
        /// </summary>
        public string gonMuhasebeKod;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği muhasebe birimi adı
        /// </summary>
        public string gonMuhasebeAd;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği harcama birimi kodu
        /// </summary>
        public string gonHarcamaKod;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği harcama birimi adı
        /// </summary>
        public string gonHarcamaAd;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği ambar kodu
        /// </summary>
        public string gonAmbarKod;

        /// <summary>
        /// Malzemeleri gönderen/Malzemelerin gönderildiği ambar adı
        /// </summary>
        public string gonAmbarAd;

        /// <summary>
        /// Parametresiz KutuphaneBilgiDetay nesnesi oluşturucu yordam
        /// </summary>
        public KutuphaneBilgiDetay()
        {
            belgeTarih = new TNSDateTime();
            belgeNo = string.Empty;
            islemTipi = new IslemTip();
            sicilNo = string.Empty;
            ciltNo = string.Empty;
            dil = string.Empty;
            yazarAdi = string.Empty;
            adi = string.Empty;
            yayinYeri = string.Empty;
            yayinTarihi = string.Empty;
            neredenGeldi = string.Empty;
            boyutlari = string.Empty;
            satirSayisi = string.Empty;
            yaprakSayisi = string.Empty;
            sayfaSayisi = string.Empty;
            ciltTuru = string.Empty;
            fotograf = string.Empty;
            birimFiyatKDVLi = (decimal)0;
            yeriKonusu = string.Empty;
            cikisCinsi = string.Empty;
            nereyeVerildi = string.Empty;
            kimeVerildi = string.Empty;
            nereyeGonderildi = string.Empty;
            gonMuhasebeKod = string.Empty;
            gonMuhasebeAd = string.Empty;
            gonHarcamaKod = string.Empty;
            gonHarcamaAd = string.Empty;
            gonAmbarKod = string.Empty;
            gonAmbarAd = string.Empty;
        }
    }

    /// <summary>
    /// Defterlerin sorgulanmasında kullanılan kriter bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class DefterKriter
    {
        /// <summary>
        /// Yıl kriteri
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd;

        /// <summary>
        /// Defter kayıtlarının listelenmeye başlanacağı ilk kayıt
        /// </summary>
        public int ilkKayit;

        /// <summary>
        /// Defterde gösterilmesi istenen kayıt sayısı
        /// </summary>
        public int kayitSayisi;

        /// <summary>
        /// Kitabın adı
        /// </summary>
        public string kitapAd;

        /// <summary>
        /// Yazarın adı
        /// </summary>
        public string yazarAd;

        /// <summary>
        /// Yer bilgisi
        /// </summary>
        public string yer;

        /// <summary>
        /// Parametresiz DefterKriter nesnesi oluşturucu yordam
        /// </summary>
        public DefterKriter()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            hesapPlanKod = string.Empty;
            hesapPlanAd = string.Empty;
            ilkKayit = 0;
            kayitSayisi = 0;
            kitapAd = string.Empty;
            yazarAd = string.Empty;
            yer = string.Empty;
        }

        public DefterKriter(AmortismanKriter ak)
        {
            yil = ak.yil;
            muhasebeKod = ak.muhasebeKod;
            muhasebeAd = string.Empty;
            harcamaKod = ak.harcamaKod;
            harcamaAd = string.Empty;
            ambarKod = ak.ambarKod;
            ambarAd = string.Empty;
            hesapPlanKod = ak.hesapPlanKod;
            hesapPlanAd = string.Empty;
            ilkKayit = 0;
            kayitSayisi = 0;
            kitapAd = string.Empty;
            yazarAd = string.Empty;
            yer = string.Empty;
        }
    }

    /// <summary>
    /// Taşınır tüketim malzemelerinin hareket bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class StokHareketBilgi
    {
        /// <summary>
        /// Hareketin yapıldığı yıl
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Tüketim malzemesinin taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod;

        /// <summary>
        /// Tüketim malzemesinin taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;
        public string fisNo;
        public string olcuBirimAdi;

        /// <summary>
        /// Hareketin işlem türü
        /// ENUMIslemTipi listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int islemTur;

        /// <summary>
        /// Tüketim malzemesinin adedi
        /// </summary>
        public decimal miktar;

        /// <summary>
        /// KDV oranı
        /// </summary>
        public int kdvOran;

        /// <summary>
        /// Tüketim malzemesinin KDV'siz birim fiyatı
        /// </summary>
        public decimal birimFiyat;

        /// <summary>
        /// Tüketim malzemesinin son kullanma tarihi
        /// </summary>
        public TNSDateTime sonTarih;
        public bool khkHaric;
        public bool stokSifirIseGetir = false;

        /// <summary>
        /// Parametresiz StokHareketBilgi nesnesi oluşturucu yordam
        /// </summary>
        public StokHareketBilgi()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            harcamaKod = string.Empty;
            hesapPlanAd = string.Empty;
            hesapPlanKod = string.Empty;
            ambarKod = string.Empty;
            fisNo = string.Empty;
            olcuBirimAdi = string.Empty;
            islemTur = 0;
            miktar = (decimal)0;
            kdvOran = 0;
            birimFiyat = (decimal)0;
            sonTarih = new TNSDateTime();
            khkHaric = false;
        }
    }

    /// <summary>
    /// Taşınır işlem fişinin veya zimmet fişinin tarihçe bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class TarihceBilgisi
    {
        /// <summary>
        /// Belge yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebe;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaBirimi;

        /// <summary>
        /// Belge numarası
        /// </summary>
        public string belgeNo;

        /// <summary>
        /// İşlem yapan kullanıcının kodu
        /// </summary>
        public string islemYapan;

        public string islemYapanAd;

        /// <summary>
        /// İşlemin yapılış tarihi ve zamanı
        /// </summary>
        public TNSDateTime islemTarih;

        /// <summary>
        /// İşlemin açıklaması
        /// </summary>
        public string islem;

        public string tasinirIslemFormKod;

        public string ipAdresi;

        public int onayDurum;

        /// <summary>
        /// Parametresiz TarihceBilgisi nesnesi oluşturucu yordam
        /// </summary>
        public TarihceBilgisi()
        {
            yil = 0;
            muhasebe = "";
            harcamaBirimi = "";
            belgeNo = "";
            islemYapan = "";
            islemYapanAd = "";
            islemTarih = new TNSDateTime();
            islem = "";
            tasinirIslemFormKod = "";
            ipAdresi = "";
            onayDurum = 0;
        }
    }

    /// <summary>
    /// Taşınır demirbaşının özellik bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class SicilNoOzellik
    {
        /// <summary>
        /// Demirbaşın programatik sicil numarası
        /// </summary>
        public int prSicilno;

        /// <summary>
        /// Demirbaşın markasının programatik kodu
        /// </summary>
        public int markaKod;

        /// <summary>
        /// Demirbaşın markasının adı
        /// </summary>
        public string markaAd;

        /// <summary>
        /// Demirbaşın modelinin programatik kodu
        /// </summary>
        public int modelKod;

        /// <summary>
        /// Demirbaşın modelinin adı
        /// </summary>
        public string modelAd;

        /// <summary>
        /// Demirbaşın şase numarası
        /// </summary>
        public string saseNo;

        /// <summary>
        /// Demirbaşın motor numarası
        /// </summary>
        public string motorNo;

        /// <summary>
        /// Demirbaşın plakası
        /// </summary>
        public string plaka;

        /// <summary>
        /// Demirbaşın cilt numarası
        /// </summary>
        public string ciltNo;

        /// <summary>
        /// Demirbaşın yazılmış olduğu dil bilgisi
        /// </summary>
        public string dil;

        /// <summary>
        /// Demirbaşın yazarının adı
        /// </summary>
        public string yazarAdi;

        /// <summary>
        /// Demirbaşın adı
        /// </summary>
        public string adi;

        /// <summary>
        /// Demirbaşın yayın yeri
        /// </summary>
        public string yayinYeri;

        /// <summary>
        /// Demirbaşın yayın tarihi
        /// </summary>
        public string yayinTarihi;

        /// <summary>
        /// Demirbaşın alındığı yer bilgisi
        /// </summary>
        public string neredenGeldi;

        /// <summary>
        /// Demirbaşın fiziksel boyutları
        /// </summary>
        public string boyutlari;

        /// <summary>
        /// Demirbaştaki satır sayısı
        /// </summary>
        public string satirSayisi;

        /// <summary>
        /// Demirbaştaki yaprak sayısı
        /// </summary>
        public string yaprakSayisi;

        /// <summary>
        /// Demirbaştaki sayfa sayısı
        /// </summary>
        public string sayfaSayisi;

        /// <summary>
        /// Demirbaşın cildinin türü
        /// </summary>
        public string ciltTuru;

        /// <summary>
        /// Demirbaşın çeşidi
        /// </summary>
        public string cesidi;

        /// <summary>
        /// Demirbaşın yeri ve konusuyla ilgili bilgi
        /// </summary>
        public string yeriKonusu;

        /// <summary>
        /// Demirbaşın geliş tarihi
        /// </summary>
        public string gelisTarihi;

        /// <summary>
        /// Demirbaşın nereden bulunduğu bilgisi
        /// </summary>
        public string neredeBulundu;

        /// <summary>
        /// Demirbaşın çağıyla ilgili bilgisi
        /// </summary>
        public string cagi;

        /// <summary>
        /// Demirbaşın durum bilgisi
        /// </summary>
        public string durumuMaddesi;

        /// <summary>
        /// Demirbaşın ön yüzüyle ilgili tanımlayıcı bilgi
        /// </summary>
        public string onYuz;

        /// <summary>
        /// Demirbaşın arka yüzüyle ilgili tanımlayıcı bilgi
        /// </summary>
        public string arkaYuz;

        /// <summary>
        /// Demirbaşın fotograf bilgisi
        /// </summary>
        public string fotograf;

        /// <summary>
        /// Demirbaşa verilmiş puan bilgisi
        /// </summary>
        public string puan;

        /// <summary>
        /// Demirbaşın eski sicil numarası
        /// </summary>
        public string disSicilNo;

        public string disSicilNo2;
        public string bisNo;
        public string eskiBisNo1;
        public string eskiBisNo2;
        public int rfidEtiketTurKod;
        public string rfidEtiketTurAd;
        public TNSDateTime garantiBitisTarihi;
        public string giai;
        public string ekNo;
        public string hesapTarih;
        public string butceKodu;
        public string blokajKodu;
        public string geldigiSube;

        public int amortismanYil;
        public int amortismanBitti;
        public string bulunduguYer;
        public string bulunduguYerAd;

        public TNSDateTime rfidEtiketTarihi;


        public int kayitTur; // 1: Yapıştır işlemi

        public TNSDateTime sorguAlimTarihi1 = new TNSDateTime();
        public TNSDateTime sorguAlimTarihi2 = new TNSDateTime();

        /// <summary>
        /// Parametresiz SicilNoOzellik nesnesi oluşturucu yordam
        /// </summary>
        public SicilNoOzellik()
        {
            prSicilno = 0;
            markaKod = 0;
            modelKod = 0;
            saseNo = "";
            motorNo = "";
            plaka = "";
            ciltNo = "";
            dil = "";
            yazarAdi = "";
            adi = "";
            yayinYeri = "";
            yayinTarihi = "";
            neredenGeldi = "";
            boyutlari = "";
            satirSayisi = "";
            yaprakSayisi = "";
            sayfaSayisi = "";
            ciltTuru = "";
            cesidi = "";
            yeriKonusu = "";
            gelisTarihi = "";
            neredeBulundu = "";
            cagi = "";
            durumuMaddesi = "";
            onYuz = "";
            arkaYuz = "";
            fotograf = "";
            puan = "";
            disSicilNo = "";
            modelAd = "";
            markaAd = "";
            disSicilNo2 = "";
            bisNo = "";
            eskiBisNo1 = "";
            eskiBisNo2 = "";
            rfidEtiketTurKod = 0;
            rfidEtiketTurAd = "";
            garantiBitisTarihi = new TNSDateTime();
            giai = "";
            hesapTarih = "";
            butceKodu = "";
            blokajKodu = "";
            geldigiSube = "";
            amortismanYil = 0;
            amortismanBitti = -1;
            bulunduguYer = "";
            bulunduguYerAd = "";
            rfidEtiketTarihi = new TNSDateTime();

            kayitTur = 0;
        }
    }

    /// <summary>
    /// Harcama birimlerine ait son taşınır işlem fişi bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class HarcamaSonTIF
    {
        /// <summary>
        /// Belge yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string ilAd;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilceAd;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Belge numarası
        /// </summary>
        public string tifNo;

        /// <summary>
        /// Parametresiz HarcamaSonTIF nesnesi oluşturucu yordam
        /// </summary>
        public HarcamaSonTIF()
        {
            yil = 0;
            ilKod = string.Empty;
            ilAd = string.Empty;
            ilceKod = string.Empty;
            ilceAd = string.Empty;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            tifNo = string.Empty;
        }
    }

    /// <summary>
    /// Taşınır depo durum raporunun bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class DepoDurum
    {
        /// <summary>
        /// Yıl kriteri
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string ilAd;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilceAd;

        /// <summary>
        /// Rapor türü
        /// ENUMDepoDurumRaporTur listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int raporTur;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi kodu
        /// </summary>
        public int olcuBirimKod;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi adı
        /// </summary>
        public string olcuBirimAd;

        /// <summary>
        /// Malzemenin gelen miktarı
        /// </summary>
        public decimal girenMiktar;

        /// <summary>
        /// Malzemenin gelen tutarı
        /// </summary>
        public decimal girenTutar;

        /// <summary>
        /// Malzemenin giden miktarı
        /// </summary>
        public decimal cikanMiktar;

        /// <summary>
        /// Malzemenin giden tutarı
        /// </summary>
        public decimal cikanTutar;

        /// <summary>
        /// Kişilere zimmet verilmiş malzeme miktarı
        /// </summary>
        public decimal zimmetKisiMiktar;

        /// <summary>
        /// Kişilere zimmet verilmiş malzeme tutarı
        /// </summary>
        public decimal zimmetKisiTutar;

        /// <summary>
        /// Ortak alana zimmet verilmiş malzeme miktarı
        /// </summary>
        public decimal zimmetOrtakMiktar;

        /// <summary>
        /// Ortak alana zimmet verilmiş malzeme tutarı
        /// </summary>
        public decimal zimmetOrtakTutar;

        /// <summary>
        /// Aktif olarak depoda kalan malzeme miktarı
        /// </summary>
        public decimal kalanMiktar;

        /// <summary>
        /// Aktif olarak depoda kalan malzeme tutarı
        /// </summary>
        public decimal kalanTutar;

        /// <summary>
        /// Aktif olarak depoda kalan malzeme miktarı
        /// </summary>
        public decimal devredenMiktar;

        /// <summary>
        /// Aktif olarak depoda kalan malzeme tutarı
        /// </summary>
        public decimal devredenTutar;

        /// <summary>
        /// Sadece yıl içinde yapılan işlemlerin gösterilip gösterilmeyeceği bilgisi
        /// </summary>
        public bool yilIci;

        /// <summary>
        /// Yıl devri işlemlerinin gösterilip gösterilmeyeceği bilgisi
        /// </summary>
        public bool yilDevri;

        /// <summary>
        /// Dönem kriteri
        /// </summary>
        public int donem;

        /// <summary>
        /// Önceki yılın malzeme tüketim miktarı
        /// </summary>
        public decimal gecenYilTuketimMiktar;

        /// <summary>
        /// Önceki yılın malzeme tüketim tutarı
        /// </summary>
        public decimal gecenYilTuketimTutar;

        /// <summary>
        /// Dönemsel malzeme tüketim miktarı
        /// </summary>
        public decimal donemselTuketimMiktar;

        /// <summary>
        /// Dönemsel malzeme tüketim tutarı
        /// </summary>
        public decimal donemselTuketimTutar;

        /// <summary>
        /// KHK Durumu
        /// </summary>
        public int khkDurumu;

        /// <summary>
        /// Nereden geldi/Nereye gitti bilgisi
        /// </summary>
        public string firma;

        /// <summary>
        /// TC kimlik no veya Vergi Numarası
        /// </summary>
        public string TcVNo;

        public int tip;

        public TNSDateTime baslamaTarih;

        public TNSDateTime bitisTarih;

        /// <summary>
        /// Parametresiz DepoDurum nesnesi oluşturucu yordam
        /// </summary>
        public DepoDurum()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            ilKod = string.Empty;
            ilAd = string.Empty;
            ilceKod = string.Empty;
            ilceAd = string.Empty;
            raporTur = 0;
            hesapPlanKod = string.Empty;
            hesapPlanAd = string.Empty;
            olcuBirimKod = 0;
            olcuBirimAd = string.Empty;
            girenMiktar = (decimal)0;
            girenTutar = (decimal)0;
            cikanMiktar = (decimal)0;
            cikanTutar = (decimal)0;
            zimmetKisiMiktar = (decimal)0;
            zimmetKisiTutar = (decimal)0;
            zimmetOrtakMiktar = (decimal)0;
            zimmetOrtakTutar = (decimal)0;
            kalanMiktar = (decimal)0;
            kalanTutar = (decimal)0;
            yilIci = false;
            yilDevri = false;
            donem = 0;
            gecenYilTuketimMiktar = (decimal)0;
            gecenYilTuketimTutar = (decimal)0;
            donemselTuketimMiktar = (decimal)0;
            donemselTuketimTutar = (decimal)0;

            khkDurumu = 0;
            TcVNo = string.Empty;
            firma = string.Empty;
            tip = 0;

            baslamaTarih = new TNSDateTime();
            bitisTarih = new TNSDateTime();
        }
    }

    /// <summary>
    /// Tüketim çıkış raporunun üst bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class TuketimCikis
    {
        /// <summary>
        /// Yıl kriteri
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd;

        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string ilAd;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilceAd;

        /// <summary>
        /// Tüketim çıkış raporunun detay bilgilerini tutan nesne
        /// </summary>
        public TNSCollection detay;

        /// <summary>
        /// Başlangıç tüketim tarihi
        /// Aralık belirtilmesi gereken durumlarda tarih1 aralığın alt sınırını ifade eder.
        /// </summary>
        public TNSDateTime tarih1;

        /// <summary>
        /// Bitiş tüketim tarihi
        /// Aralık belirtilmesi gereken durumlarda tarih2 aralığın üst sınırını ifade eder.
        /// </summary>
        public TNSDateTime tarih2;

        /// <summary>
        /// Parametresiz TuketimCikis nesnesi oluşturucu yordam
        /// </summary>
        public TuketimCikis()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            hesapPlanKod = string.Empty;
            hesapPlanAd = string.Empty;
            ilKod = string.Empty;
            ilAd = string.Empty;
            ilceKod = string.Empty;
            ilceAd = string.Empty;
            detay = new TNSCollection();
            tarih1 = new TNSDateTime();
            tarih2 = new TNSDateTime();
        }
    }

    /// <summary>
    /// Tüketim çıkış raporunun detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class TuketimCikisDetay
    {
        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi kodu
        /// </summary>
        public int olcuBirimKod;

        /// <summary>
        /// Taşınır hesap planının ölçü birimi adı
        /// </summary>
        public string olcuBirimAd;

        /// <summary>
        /// Malzemenin adedi
        /// </summary>
        public decimal miktar;

        /// <summary>
        /// Dönemsel malzeme tutarları
        /// </summary>
        public decimal[] tutar;

        /// <summary>
        /// Parametresiz TuketimCikisDetay nesnesi oluşturucu yordam
        /// </summary>
        public TuketimCikisDetay()
        {
            hesapPlanKod = string.Empty;
            hesapPlanAd = string.Empty;
            olcuBirimKod = 0;
            olcuBirimAd = string.Empty;
            miktar = (decimal)0;
            tutar = new decimal[4];
        }
    }

    /// <summary>
    /// Zimmet raporlarının üst bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ZimmetOrtakAlanVeKisi
    {
        /// <summary>
        /// Yıl kriteri
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Zimmet verilmiş odanın kodu
        /// </summary>
        public string odaKod;

        /// <summary>
        /// Zimmet verilmiş odanın adı
        /// </summary>
        public string odaAd;

        /// <summary>
        /// Zimmet raporlarının detay bilgilerini tutan nesne
        /// </summary>
        public TNSCollection detay;

        /// <summary>
        /// Kişiye verilmiş zimmetlerin de dahil edilip edilmeyeceği bilgisi
        /// </summary>
        public int kisiDahilEt;

        /// <summary>
        /// Zimmet verilmiş personelin T.C. kimlik numarası
        /// </summary>
        public string tcKimlikNo;

        /// <summary>
        /// Zimmet verilmiş personelin adı
        /// </summary>
        public string kisiAd;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapPlanKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapPlanAd;

        public string sicilNo;

        /// <summary>
        /// Parametresiz ZimmetOrtakAlanVeKisi nesnesi oluşturucu yordam
        /// </summary>
        public ZimmetOrtakAlanVeKisi()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            odaKod = string.Empty;
            odaAd = string.Empty;
            detay = new TNSCollection();
            kisiDahilEt = 0;
            tcKimlikNo = string.Empty;
            kisiAd = string.Empty;
            hesapPlanKod = string.Empty;
            hesapPlanAd = string.Empty;
            sicilNo = string.Empty;

        }
    }

    /// <summary>
    /// Zimmet raporlarının detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ZimmetOrtakAlanVeKisiDetay
    {
        /// <summary>
        /// Malzemenin programatik sicil numarası
        /// </summary>
        public int prSicilNo;

        /// <summary>
        /// Malzemenin sicil numarası
        /// </summary>
        public string gorSicilNo;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string sicilAd;

        /// <summary>
        /// Zimmet fişinin belge numarası
        /// </summary>
        public string fisNo;

        /// <summary>
        /// Zimmet fişinin belge tarihi
        /// </summary>
        public TNSDateTime fisTarih;

        /// <summary>
        /// Zimmet verilmiş veya sorumlu personelin T.C. kimlik numarası
        /// </summary>
        public string sorumluKisiTC;

        /// <summary>
        /// Zimmet verilmiş veya sorumlu personelin adı
        /// </summary>
        public string sorumluKisiAd;

        /// <summary>
        /// Malzemeyle ilgili zimmet açıklaması
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Malzemenin RFID numarası
        /// </summary>
        public int rfIdNo;

        public string odaKod;
        public string odaAd;
        public string saseNo;

        /// <summary>
        /// Parametresiz ZimmetOrtakAlanVeKisiDetay nesnesi oluşturucu yordam
        /// </summary>
        public ZimmetOrtakAlanVeKisiDetay()
        {
            prSicilNo = 0;
            gorSicilNo = string.Empty;
            sicilAd = string.Empty;
            fisNo = string.Empty;
            fisTarih = new TNSDateTime();
            sorumluKisiTC = string.Empty;
            sorumluKisiAd = string.Empty;
            aciklama = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            rfIdNo = 0;
            odaKod = "";
            odaAd = "";
            saseNo = "";
        }
    }

    /// <summary>
    /// Taşınır kuruş farkları raporunun üst bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class KurusFarki : TuketimCikis
    {
        /// <summary>
        /// Parametresiz KurusFarki nesnesi oluşturucu yordam
        /// </summary>
        public KurusFarki()
        {
        }
    }

    /// <summary>
    /// Taşınır kuruş farkları raporunun detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class KurusFarkiDetay : TuketimCikisDetay
    {
        /// <summary>
        /// Taşınır işlem fişi belge numarası
        /// </summary>
        public string fisNo;

        /// <summary>
        /// Taşınır işlem fişinin işlem tipinin türü
        /// ENUMIslemTipi listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int islemTipiTur;

        /// <summary>
        /// Parametresiz KurusFarkiDetay nesnesi oluşturucu yordam
        /// </summary>
        public KurusFarkiDetay()
        {
            fisNo = string.Empty;
            islemTipiTur = 0;
        }
    }

    /// <summary>
    /// Harcama birimi değişimi yapılırken kullanılan tablo, kolon gibi bilgileri tutan sınıf
    /// </summary>
    public class TabloSinif
    {
        /// <summary>
        /// Değişiklik yapılacak tablonun bağlı olduğu tablonun adı
        /// </summary>
        public string tabloAdAna;

        /// <summary>
        /// Değişiklik yapılacak tablonun adı
        /// </summary>
        public string tabloAdCocuk;

        /// <summary>
        /// Değişiklik yapılacak tablonun muhasebe birimi kolonunun adı
        /// </summary>
        public string muhasebeKolonAdi;

        /// <summary>
        /// Değişiklik yapılacak tablonun harcama birimi kolonunun adı
        /// </summary>
        public string harcamaBirimKolonAdi;

        /// <summary>
        /// Değişiklik yapılacak tablonun kolonlarının adları
        /// </summary>
        public string[] alanAdlari;

        /// <summary>
        /// Parametresiz TabloSinif nesnesi oluşturucu yordam
        /// </summary>
        public TabloSinif()
        {
            tabloAdAna = string.Empty;
            tabloAdCocuk = string.Empty;
            muhasebeKolonAdi = string.Empty;
            harcamaBirimKolonAdi = string.Empty;
            //yilKolonuVarMi = false;
            alanAdlari = null;
        }
    }

    /// <summary>
    /// İhtiyaç fazlası taşınır formunun üst bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class AcikPazarForm
    {
        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// İstek yapan muhasebe birimi kodu
        /// </summary>
        public string istekYapanMuhasebeKod;

        /// <summary>
        /// İstek yapan muhasebe birimi adı
        /// </summary>
        public string istekYapanMuhasebeAd;

        /// <summary>
        /// İstek yapan harcama birimi kodu
        /// </summary>
        public string istekYapanHarcamaKod;

        /// <summary>
        /// İstek yapan harcama birimi adı
        /// </summary>
        public string istekYapanHarcamaAd;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapAd;

        /// <summary>
        /// İlin plaka kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İlin adı
        /// </summary>
        public string ilAd;

        /// <summary>
        /// İlçenin programatik kodu
        /// </summary>
        public string ilceKod;

        /// <summary>
        /// İlçenin adı
        /// </summary>
        public string ilceAd;

        /// <summary>
        /// İhtiyaç fazlası taşınır formunun detay bilgileri listesini tutan nesne
        /// </summary>
        public TNSCollection detaylar;

        /// <summary>
        /// Parametresiz AcikPazarForm nesnesi oluşturucu yordam
        /// </summary>
        public AcikPazarForm()
        {
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            istekYapanMuhasebeKod = string.Empty;
            istekYapanMuhasebeAd = string.Empty;
            istekYapanHarcamaKod = string.Empty;
            istekYapanHarcamaAd = string.Empty;
            hesapKod = string.Empty;
            hesapAd = string.Empty;
            ilKod = string.Empty;
            ilAd = string.Empty;
            ilceKod = string.Empty;
            ilceAd = string.Empty;
            detaylar = new TNSCollection();
        }
    }

    /// <summary>
    /// İhtiyaç fazlası taşınır formunun detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class AcikPazarDetay
    {
        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Malzemenin detayda yer aldığı satır numarası
        /// </summary>
        public int siraNo;

        /// <summary>
        /// Malzemenin programatik sicil numarası
        /// </summary>
        public int prSicilNo;

        /// <summary>
        /// Malzemenin sicil numarası
        /// </summary>
        public string gorSicilNo;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapAd;

        /// <summary>
        /// KDV oranı
        /// </summary>
        public int kdvOran;

        /// <summary>
        /// Malzemenin birim fiyatı
        /// </summary>
        public decimal birimFiyat;

        /// <summary>
        /// Malzemeyle ilgili açıklama
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Forma ekleniş tarihi
        /// </summary>
        public TNSDateTime eklenisTarihi;

        /// <summary>
        /// Malzemeye istekte bulunan birim var mı bilgisi
        /// </summary>
        public bool istekVar;

        /// <summary>
        /// Parametresiz AcikPazarDetay nesnesi oluşturucu yordam
        /// </summary>
        public AcikPazarDetay()
        {
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            siraNo = 0;
            prSicilNo = 0;
            gorSicilNo = string.Empty;
            hesapKod = string.Empty;
            hesapAd = string.Empty;
            kdvOran = 0;
            birimFiyat = (decimal)0;
            aciklama = string.Empty;
            eklenisTarihi = new TNSDateTime();
            istekVar = false;
        }
    }

    /// <summary>
    /// İhtiyaç fazlası taşınır isteklerinin bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class AcikPazarIstek
    {
        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Malzemenin programatik sicil numarası
        /// </summary>
        public int prSicilNo;

        /// <summary>
        /// Malzemenin sicil numarası
        /// </summary>
        public string gorSicilNo;

        /// <summary>
        /// İstek yapan muhasebe birimi kodu
        /// </summary>
        public string istekYapanMuhasebeKod;

        /// <summary>
        /// İstek yapan muhasebe birimi adı
        /// </summary>
        public string istekYapanMuhasebeAd;

        /// <summary>
        /// İstek yapan harcama birimi kodu
        /// </summary>
        public string istekYapanHarcamaKod;

        /// <summary>
        /// İstek yapan harcama birimi adı
        /// </summary>
        public string istekYapanHarcamaAd;

        /// <summary>
        /// İsteğin yapıldığı tarih
        /// </summary>
        public TNSDateTime istekTarihi;

        /// <summary>
        /// Parametresiz AcikPazarIstek nesnesi oluşturucu yordam
        /// </summary>
        public AcikPazarIstek()
        {
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            prSicilNo = 0;
            gorSicilNo = string.Empty;
            istekYapanMuhasebeKod = string.Empty;
            istekYapanMuhasebeAd = string.Empty;
            istekYapanHarcamaKod = string.Empty;
            istekYapanHarcamaAd = string.Empty;
            istekTarihi = new TNSDateTime();
        }
    }

    /// <summary>
    /// Muhasebat kontrol raporunun bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class MuhasebatKontrol
    {
        /// <summary>
        /// Yıl kriteri
        /// </summary>
        public int yil;

        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapAd;

        /// <summary>
        /// Raporun hazırlanacağı taşınır hesap kodu düzeyi
        /// </summary>
        public int hesapKodUzunluk;

        /// <summary>
        /// Raporun türü
        /// ENUMDepoDurumRaporTur listesindeki MUHASEBE, HARCAMA ve MUHASEBE_HARCAMA değerlerinden biri olmalıdır.
        /// </summary>
        public int raporTur;

        /// <summary>
        /// Tüketim malzemeleri için yıl içinde girenlerin tutarı
        /// Diğer taşınırlar için ise o ana kadar ki bütün girenlerin tutarı
        /// </summary>
        public decimal girenTutar;

        /// <summary>
        /// Tüketim malzemeleri için yıl içinde çıkanların tutarı
        /// Diğer taşınırlar için ise o ana kadar ki bütün çıkanların tutarı
        /// </summary>
        public decimal cikanTutar;

        /// <summary>
        /// Tüketim malzemeleri için yıl başında girenlerin tutarı
        /// </summary>
        public decimal yilBasindaGirenTutar;

        /// <summary>
        /// Parametresiz MuhasebatKontrol nesnesi oluşturucu yordam
        /// </summary>
        public MuhasebatKontrol()
        {
            yil = 0;
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            hesapKod = string.Empty;
            hesapAd = string.Empty;
            hesapKodUzunluk = 0;
            girenTutar = (decimal)0;
            cikanTutar = (decimal)0;
            yilBasindaGirenTutar = (decimal)0;
            raporTur = -1;
        }

        /// <summary>
        /// Parametre olarak verilen işlem türüne göre
        /// verilen tutarı ilgili özelliğe ekleyen yordam
        /// </summary>
        /// <param name="islemTur">İşlem türü</param>
        /// <param name="tutar">Eklenecek tutar</param>
        public void TutarEkle(int islemTur, decimal tutar)
        {
            if (islemTur == (int)ENUMIslemTipi.YILDEVIRGIRIS)
                yilBasindaGirenTutar += tutar;
            else if (islemTur >= (int)ENUMIslemTipi.SATINALMAGIRIS && islemTur <= (int)ENUMIslemTipi.ACILIS)
                girenTutar += tutar;
            else if (islemTur > (int)ENUMIslemTipi.ACILIS)
                cikanTutar += tutar;
        }
    }

    /// <summary>
    /// RFID entegrasyonu kapsamında taşınır demirbaşlarının bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class SicilRFID
    {
        /// <summary>
        /// Muhasebe birimi kodu
        /// </summary>
        public string muhasebeKod;

        /// <summary>
        /// Muhasebe birimi adı
        /// </summary>
        public string muhasebeAd;

        /// <summary>
        /// Harcama birimi kodu
        /// </summary>
        public string harcamaKod;

        /// <summary>
        /// Harcama birimi adı
        /// </summary>
        public string harcamaAd;

        /// <summary>
        /// Ambar kodu
        /// </summary>
        public string ambarKod;

        /// <summary>
        /// Ambar adı
        /// </summary>
        public string ambarAd;

        /// <summary>
        /// Demirbaşın programatik sicil numarası
        /// </summary>
        public int prSicilNo;

        /// <summary>
        /// Demirbaşın RFID numarası
        /// </summary>
        public int rfIdNo;

        /// <summary>
        /// Demirbaşın RFID numarasını kill edecek numara
        /// </summary>
        public int killNo;

        /// <summary>
        /// Demirbaşın RFID numarasına erişim anahtarı
        /// </summary>
        public int accessNo;

        /// <summary>
        /// Demirbaşın RFID etiket tipi
        /// </summary>
        public int tagType;

        /// <summary>
        /// Parametresiz SicilRFID nesnesi oluşturucu yordam
        /// </summary>
        public SicilRFID()
        {
            muhasebeKod = string.Empty;
            muhasebeAd = string.Empty;
            harcamaKod = string.Empty;
            harcamaAd = string.Empty;
            ambarKod = string.Empty;
            ambarAd = string.Empty;
            prSicilNo = 0;
            rfIdNo = 0;
            killNo = 0;
            accessNo = 0;
            tagType = 0;
        }
    }

    /// <summary>
    /// RFID etiketi olan taşınır demirbaşlarının kapılardan geçişi ile ilgili log bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class KapiGecisLog
    {
        /// <summary>
        /// Demirbaşın programatik sicil numarası
        /// </summary>
        public int prSicilNo;

        /// <summary>
        /// Demirbaşın RFID numarası
        /// </summary>
        public int rfIdNo;

        /// <summary>
        /// Demirbaşın kapılardan geçmeye izni yoksa haber verilecek personelin T.C. kimlik numarası
        /// </summary>
        public string tcKimlikNo;

        /// <summary>
        /// Demirbaşın kapılardan geçmeye izni yoksa haber verilecek personelin adı
        /// </summary>
        public string personelAd;

        /// <summary>
        /// Kapıdan geçiş tarihi ve zamanı
        /// </summary>
        public TNSDateTime tarih;

        /// <summary>
        /// Demirbaş ile ilgili ek bilgileri tutan nesne
        /// </summary>
        public SicilNoHareket snh;

        /// <summary>
        /// Parametresiz KapiGecisLog nesnesi oluşturucu yordam
        /// </summary>
        public KapiGecisLog()
        {
            prSicilNo = 0;
            rfIdNo = 0;
            tcKimlikNo = string.Empty;
            personelAd = string.Empty;
            tarih = new TNSDateTime();
            snh = new SicilNoHareket();
        }
    }

    /// <summary>
    /// Taşınır malzemelerinin ortalama birim fiyat bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class BirimFiyatOrtalamasi
    {
        /// <summary>
        /// Taşınır hesap planı kodu
        /// </summary>
        public string hesapKod;

        /// <summary>
        /// Taşınır hesap planı adı
        /// </summary>
        public string hesapAd;

        /// <summary>
        /// Ortalama birim fiyat tutarı
        /// </summary>
        public double birimFiyat;

        /// <summary>
        /// Yıl kriteri
        /// </summary>
        public int yil;

        /// <summary>
        /// Parametresiz BirimFiyatOrtalamasi nesnesi oluşturucu yordam
        /// </summary>
        public BirimFiyatOrtalamasi()
        {
            hesapKod = string.Empty;
            hesapAd = string.Empty;
            birimFiyat = 0.0;
            yil = 0;
        }
    }

    [Serializable()]
    public class TalepKarsilama
    {
        public int yil = 0;
        public Muhasebe muhasebe = new Muhasebe();
        public HarcamaBirimi harcamaBirimi = new HarcamaBirimi();
        public string belgeNo = "";
        public TNSDateTime belgeTarihi = new TNSDateTime();
        public string talepEdenTCNo = "";
        public string talepedenAdSoyad = "";
        public string talepedenUnvan = "";
        public int durum = 0;
        public string gerekce = "";
        public string refId = "";
        public int tur = 0;
        public int amac = 0;

        public TNSCollection detay;

        public TNSDateTime ilkTarih = new TNSDateTime();
        public TNSDateTime sonTarih = new TNSDateTime();
        public bool karsilamaIslemi = false;

        public static string DurumAdBul(int durum)
        {
            if (durum == (int)ENUMTalepKarsilamaDurum.BEKLEMEDE)
                return TasinirMalResI.TMM387;
            else if (durum == (int)ENUMTalepKarsilamaDurum.KAPATILDI)
                return TasinirMalResI.TMM388;
            else if (durum == (int)ENUMTalepKarsilamaDurum.SATINALMADA)
                return TasinirMalResI.TMM389;
            else if (durum == (int)ENUMTalepKarsilamaDurum.IPTAL)
                return TasinirMalResI.TMM414;
            else
                return TasinirMalResI.TMM390;
        }
    }

    [Serializable()]
    public class TalepKarsilamaCevap
    {
        public string refID = "";
        public string refIDAna = "";
        public TNSDateTime tarih = new TNSDateTime();
        public TNSDateTime tarihSorgu = new TNSDateTime();
        public string aciklama = "";
        public int durum = 0;
        public TNSCollection detay = new TNSCollection();
        public string karsilayanAdSoyad = "";
        public string karsilayanUnvan = "";
        public Muhasebe muhasebe = new Muhasebe();
        public HarcamaBirimi harcamaBirimi = new HarcamaBirimi();
        public string talepBelgeNo = "";
    }

    [Serializable()]
    public class TalepKarsilamaCevapDetay
    {
        public string refID = "";
        public string refIDAna = "";
        public string refIDTalep = "";
        public string tur = "";
        public HesapPlaniSatir hesapPlani = new HesapPlaniSatir();
        public int satinAlmaDurum = 0;
        public decimal miktar = 0;
        public string talepAciklama = "";
    }

    [Serializable()]
    public class TalepKarsilamaDetay
    {
        public int siraNo;
        public HesapPlaniSatir hesapPlani = new HesapPlaniSatir();
        public Ambar ambar = new Ambar();
        public decimal depoDurumu = 0;
        public decimal istenilenMiktar = 0;
        public decimal karsilananMiktar = 0;
        public decimal satinAlinanMiktar = 0;
        public string aciklama = "";
        public int tifYil = 0;
        public string tifFisNo = "";
        public double birimFiyat = 0.0;
        public string refId = "";
        public string refIdAna = "";
        public int iptal = 0;
        public string paraBirimiKod = "";
    }

    [Serializable()]
    public class TuketimUretim
    {
        public int yil;
        public Muhasebe muhasebe;
        public HarcamaBirimi harcamaBirimi;
        public string belgeNo;
        public int tur;
        public Ambar ambar;
        public TNSDateTime belgeTarihi;
        public TNSCollection detay;
    }

    [Serializable()]
    public class TuketimUretimDetay
    {
        public HesapPlaniSatir hesapPlani = new HesapPlaniSatir();
        public decimal miktar;
    }

    [Serializable()]
    public class GiyecekTechizat
    {
        public int yil;
        public Muhasebe muhasebe;
        public HarcamaBirimi harcamaBirimi;
        public string belgeNo;
        public int tur;
        public Ambar ambar;
        public TNSDateTime belgeTarihi;
        public TNSCollection detay;
    }

    [Serializable()]
    public class GiyecekTechizatDetay
    {
        public HesapPlaniSatir hesapPlani;
        public decimal miktar;
        public string perUnvan;
        public string perAdSoyad;
        public string perSicilNo;
        public TNSDateTime teslimTarih = new TNSDateTime();
    }

    [Serializable()]
    public class TasResim
    {
        public int resimID = 0;
        public int prSicilNo = 0;
        public int siraNo = 0;
        public byte[] resim;
        public string adi = "";
        public double boyutu = 0.0;
        public string ekleyenKisiAdi = "";
        public string ekleyenKisiKod = "";
        public TNSDateTime eklemeTarihi = new TNSDateTime();
        public string kayitEdilecekDosyaYol = "";
    }

    [Serializable()]
    public class IslemDosya
    {
        public int dosyaKod = 0;
        public int yil = 0;
        public string muhasebeKod = "";
        public string harcamaBirimKod = "";
        public string fisNo = "";
        public int siraNo = 0;
        public byte[] resim;
        public string adi = "";
        public double boyutu = 0.0;
        public string ekleyenKisiAdi = "";
        public string ekleyenKisiKod = "";
        public new TNSDateTime eklemeTarihi = new TNSDateTime();
        public string kayitEdilecekDosyaYol = "";
    }

    [Serializable()]
    public class TasHesapPlaniBarkod
    {
        public int tasBarkodID = 0;
        public string tasHesapNo = "";
        public string barkod = "";
        public string muhasebeKod = "";
        public string harcamaBirimKod = "";
    }

    [Serializable()]
    public class TopluIslemForm
    {
        public int grupKod = 0;
        public string aciklama = "";
        public bool onceliKayitlarinAktarimi = false;
        public ObjectArray detay = new ObjectArray();
    }

    [Serializable()]
    public class TopluIslem
    {
        public int id = 0;
        public int grupKod = 0;
        public string sicilNo = "";
        public string kaynakMuh = "";
        public string kaynakHrc = "";
        public string kaynakAmb = "";
        public string kaynakOda = "";
        public string kaynakTC = "";
        public string hedefMuh = "";
        public string hedefHrc = "";
        public string hedefAmb = "";
        public string hedefOda = "";
        public string hedefTC = "";
        public string zimmetDusBelgeNo = "";
        public string zimmetVerBelgeNo = "";
        public string devirVerBelgeNo = "";
        public string devirAlBelgeNo = "";
    }

    [Serializable()]
    public class KodAd
    {
        public string kod = string.Empty;
        public string ad = string.Empty;
    }

    [Serializable()]
    public class SayimKisiForm
    {
        public string kod = "";
        public string muhasebe = "";
        public string harcamaBirimi = "";
        public TNSDateTime baslamaTarih = new TNSDateTime();
        public TNSDateTime bitisTarih = new TNSDateTime();
        public string adi = "";
        public string islemYapanAd = "";
        public string islemYapanKod = "";
        public TNSDateTime islemTarihi = new TNSDateTime();
    }

    [Serializable()]
    public class Sayimkisi
    {
        public string kod = "";
        public string sayimKod = "";
        public string kisiKod = "";
        public string kisiAd = "";
        public string odaKod = "";
        public string odaAd = "";
        public string islemYapanAd = "";
        public string islemYapanKod = "";
        public TNSDateTime islemTarihi = new TNSDateTime();
        public int fazla = 0;
        public int sayilan = 0;
        public int zimmetli = 0;
        public string sicilNo = "";
        public string malzemeAd = "";
        public int okundu = 0;
        public string fazlaKisiKod = "";
        public string fazlaKisiAd = "";
        public TNSDateTime baslamaTarihi = new TNSDateTime();
        public TNSDateTime bitisTarihi = new TNSDateTime();
        public string sayimAd = "";
        public string muhasebeKod = "";
        public string harcamabirimKod = "";
        public string kimeGitti = "";
    }

    [Serializable()]
    public class SayimKisiDetay
    {
        public string kod = "";
        public string sayimKod = "";
        public string sayimDetayKod = "";
        public string kisiKod = "";
        public int prSicilNo = 0;
        public string sicilNo = "";
        public string epc = "";
        public string malzemeAd = "";
        public int okundu = 0;
        public int fazla = 0;
        public string fazlaKisiKod = "";
        public string fazlaKisiAd = "";
        public string islemYapanAd = "";
        public string islemYapanKod = "";
        public TNSDateTime islemTarihi = new TNSDateTime();
    }

    [Serializable()]
    public class SayimAmbarForm
    {
        public string kod = "";
        public string muhasebe = "";
        public string harcamaBirimi = "";
        public TNSDateTime baslamaTarih = new TNSDateTime();
        public TNSDateTime bitisTarih = new TNSDateTime();
        public string adi = "";
        public string islemYapanAd = "";
        public string islemYapanKod = "";
        public TNSDateTime islemTarihi = new TNSDateTime();
    }

    [Serializable()]
    public class Sayimambar
    {
        public string kod = "";
        public string sayimKod = "";
        public string ambarKod = "";
        public string ambarAd = "";
        public string odaKod = "";
        public string odaAd = "";
        public string islemYapanAd = "";
        public string islemYapanKod = "";
        public TNSDateTime islemTarihi = new TNSDateTime();
        public int fazla = 0;
        public int sayilan = 0;
        public int zimmetli = 0;
        public string sicilNo = "";
        public string malzemeAd = "";
        public int okundu = 0;
        public string fazlaAmbarKod = "";
        public string fazlaAmbarAd = "";
        public TNSDateTime baslamaTarihi = new TNSDateTime();
        public TNSDateTime bitisTarihi = new TNSDateTime();
        public string sayimAd = "";
        public string harcamabirimKod = "";
    }

    [Serializable()]
    public class SayimAmbarDetay
    {
        public string kod = "";
        public string sayimKod = "";
        public string sayimDetayKod = "";
        public string ambarKod = "";
        public int prSicilNo = 0;
        public string sicilNo = "";
        public string epc = "";
        public string malzemeAd = "";
        public int okundu = 0;
        public int fazla = 0;
        public string fazlaAmbarKod = "";
        public string fazlaAmbarAd = "";
        public string islemYapanAd = "";
        public string islemYapanKod = "";
        public TNSDateTime islemTarihi = new TNSDateTime();
    }

    [Serializable()]
    public class TesisBilesenleri
    {
        public string refId = "";
        public Muhasebe muhasebe = new Muhasebe();
        public HarcamaBirimi harcamaBirimi = new HarcamaBirimi();
        public Ambar ambar = new Ambar();
        public int belgeNo = 0;
        public TNSDateTime belgeTarihi = new TNSDateTime();
        public int durum = 0;
        public string sicilNo = "";
        public string hesapPlanKod = "";
        public string hesapPlanAd = "";
        public string bulunduguYer = "";
        public Il il = new Il();
        public Ilce ilce = new Ilce();

        public ObjectArray detay = new ObjectArray();
    }

    [Serializable()]
    public class TesisBilesenleriDetay
    {
        public string refID = "";
        public string refIDAna = "";
        public int siraNo = 0;
        public string adi = "";
        public double miktar = 0;
        public string ozellik = "";
    }

    [Serializable()]
    public class UretimFormu
    {
        public string kod = "";
        public HesapPlaniSatir anaUrun = new HesapPlaniSatir();
        public int miktar = 0;

        public Muhasebe cikanMuhasebe = new Muhasebe();
        public HarcamaBirimi cikanHarcamaBirimi = new HarcamaBirimi();
        public Ambar cikanAmbar = new Ambar();

        public Muhasebe girenMuhasebe = new Muhasebe();
        public HarcamaBirimi girenHarcamaBirimi = new HarcamaBirimi();
        public Ambar girenAmbar = new Ambar();

        public string cikanFisNo = "";
        public string girenFisNo = "";

        public int fisNo = 0;
        public TNSDateTime fisTarihi = new TNSDateTime();
        public int durum = 0;
        public string islemYapan = "";

        public int sorguYil = 0;
        public int sorguBelgeNoBasla = 0;
        public int sorguBelgeNoBitis = 0;
        public TNSDateTime sorguBelgeTarihBasla = new TNSDateTime();
        public TNSDateTime sorguBelgeTarihBitis = new TNSDateTime();

    }

    [Serializable()]
    public class UretimRecetesi
    {
        public string kod = "";
        public HesapPlaniSatir anaUrun = new HesapPlaniSatir();
        public HesapPlaniSatir altUrun = new HesapPlaniSatir();
        public double miktar = 0.0;
    }

    [Serializable()]
    public class Aktarim
    {
        public string kod = "";
        public Muhasebe muhasebe = new Muhasebe();
        public HarcamaBirimi harcamaBirimi = new HarcamaBirimi();
        public Ambar ambar = new Ambar();
        public TNSDateTime belgeTarihi = new TNSDateTime();
        public int islemTipiKod = 0;
        public string islemTipiAd = "";
        public int belgeTur = 0;
        public string belgeTurAd = "";
        public string neredenGeldi = "";
        public string aciklama = "";
        public TNSDateTime islemTarih = new TNSDateTime();
        public string islemYapanKod = "";
        public string islemYapanAd = "";
        public int durum = 0;
        public string yuklenenDosyaAdi = "";
        public ObjectArray detay = new ObjectArray();

        public int toplamKayit = 0;
        public int islenenKayit = 0;
        public string olusanBelgeNo = "";
    }

    [Serializable()]
    public class AktarimDetay
    {
        public string kod = "";
        public string kodAna = "";
        public int siraNo = 0;
        public string hesapPlanKod = "";
        public double miktar = 0;
        public int kdv = 0;
        public double birimFiyat = 0.0;
        public string aciklama = "";
        public string eserAdi = "";
        public string yazarAdi = "";
        public string sicilNo = "";
        public int durum = 0;
        public string belgeNo = "";
    }

    [Serializable()]
    public class TasinirIslemOnayYetki
    {
        public string muhasebeKod = "";
        public string harcamaBirimKod = "";
        public string mernis = "";
        public string adSoyad = "";
        public string eposta = "";
        public int onayTur = (int)ENUMTasinirIslemFormOnayTur.TANIMSIZ;
    }

    [Serializable()]
    public class RFIDEtiketTuru
    {
        public int kod = 0;
        public string ad = "";
        public string fabAd = "";
    }

    [Serializable()]
    public class Unvan
    {
        public int kod = 0;
        public string adi = "";
        public string kisaAdi = "";
        public int siraNo = 0;
        public int yonetici = 0;
    }

    [Serializable()]
    public class Sayfalama
    {
        public int sayfaNo = 0;
        public int kayitSayisi = 0;
        public string siralamaAlani = "";
        public string siralamaYon = "";
    }

    [Serializable()]
    public class SicilNoDurum
    {
        public int prSicilNo = 0;
        public string gorSicilNo = "";
        public string tifKod = "";
        public int durum = 0;
        public int durumIslem = 0;
        public string durumAciklama = "";
        public TNSDateTime islemTarihi = new TNSDateTime();
    }


    /// <summary>
    /// Devir çıkış raporunun üst bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class DevirCikis
    {
        public int yil = 0;
        public string muhasebeKod = "";
        public string harcamaKod = "";
        public string gMuhasebeKod = "";
        public string gHarcamaKod = "";
        public TNSDateTime tarih1 = new TNSDateTime();
        public TNSDateTime tarih2 = new TNSDateTime();
        public string hesapPlanKod = "";
        public decimal miktar = (decimal)0;
        public decimal tutar = (decimal)0;
        public decimal tutarKDVli = (decimal)0;
        public string fisNo = "";
    }

    /// <summary>
    /// Sicil no değer artış bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class SicilNoDegerArtis
    {
        private double _tutar = 0.0;

        public string kod = "";
        public string muhasebeKod = "";
        public string harcamaKod = "";
        public string belgeNo = "";
        public TNSDateTime belgeTarihi = new TNSDateTime();
        public int prSicilNo = 0;
        public string gorSicilNo = "";
        public string ambarKod = "";
        public int kdvOran = 0;
        public double katsayi = 0;

        public double tutar
        {
            get
            {
                double t = Math.Abs(_tutar);
                if (tur == 3) //Değer azalışı
                    t *= -1;
                else if (tur == 1) //Enflasyon
                    t = _tutar;
                return t;
            }
            set { _tutar = value; }
        }
        public string gerekce = "";
        public int durum = 0;
        public int tur = 0;
        public string muhasebeAd = "";
        public string harcamaAd = "";
        public double toplamTutar = 0.0;
        public string malzemeKod = "";
        public string malzemeAdi = "";
        /// <summary>
        /// Enflasyon Katsayısı için
        /// </summary>
        public decimal amotismanTutar = 0;
        public string hesapPlanKod = "";
        public double eskiTutar = 0;
        public string tifNo = "";


        public string sorguEskiSicilNo = "";
        public string sorguBelgeNoBas = "";
        public string sorguBelgeNoBit = "";
        public TNSDateTime sorguBelgeBasTarihi = new TNSDateTime();
        public TNSDateTime sorguBelgeBitTarihi = new TNSDateTime();
    }

    [Serializable()]
    public class AmortismanBilgi
    {
        public double ilkAmartismanYuzdesi = 0;
        public double buAmartismanYuzdesi = 0;
        public double ilkBedel = 0;
        public double birikenAmortisman = 0;
        public double bedelDuzeltmeFarki = 0;
        public double amortiDuzeltmeFarki = 0;
        public double sonBedel = 0;
        public double duzBirikenAmortisman = 0;
        public double ayrilanSonAmortisman = 0;
        public double oncekiYillarAmortisman = 0;
        public string bulunduguYer = "";
        public string bulunduguYerAd = "";
        public int sonAmortismanYil = 0;
        public int sonAmortismanDonem = 0;

        public double maliyetToplamTutar = 0;
        public double maliyetToplamAmortisman = 0;
        public double degerArtisToplamTutar = 0;
        public double degerArtisToplamAmortisman = 0;
        public double enflasyonToplamTutar = 0;
        public double enflasyonToplamAmortisman = 0;

        public static string AmartismanYuzdesiGetir(int amortismanSuresi)
        {
            return Math.Truncate((100 / (double)amortismanSuresi) * 100) / 100 + "";
        }
    }

    [Serializable()]
    public class AmortismanIslemForm
    {
        public string kod = "";
        public int yil = 0;
        public int donem = 0;
        public string muhasebeKod = "";
        public string harcamaKod = "";
        public string fisNo = "";
        public TNSDateTime fisTarih = new TNSDateTime();
        public string ambarKod = "";
        public int durum = 0;
        public TNSDateTime islemTarih = new TNSDateTime();
        public string islemYapan = "";
        public int onayDurum = 0;

        public string muhasebeAd = "";
        public string harcamaAd = "";
        public string ambarAd = "";

        public string hesapKodu = "";
        public string mernis = "";
        public string tip = "";
        public string fisAciklama = "";


        public ObjectArray detay = new ObjectArray();

        public static string FormDurumAciklama(ENUMAmortismanFormDurumu durum)
        {
            if (durum == ENUMAmortismanFormDurumu.YENI) return TasinirMalResI.TMM007;
            else if (durum == ENUMAmortismanFormDurumu.DEGISTIRILDI) return TasinirMalResI.TMM008;
            else if (durum == ENUMAmortismanFormDurumu.ONAYLI) return TasinirMalResI.TMM009;
            else if (durum == ENUMAmortismanFormDurumu.ONAYKALDIR) return TasinirMalResI.TMM010;
            else if (durum == ENUMAmortismanFormDurumu.IPTAL) return TasinirMalResI.TMM011;
            else
                return string.Format(TasinirMalResI.TMM012, durum);
        }
    }

    [Serializable()]
    public class MuhasebeIslemi
    {
        public string islemRefNo = "";
        public string islemCinsi = "";
        public string servis = "";
        public int durum = 0;
        public string islemYapan = "";
        public TNSDateTime islemTarih = new TNSDateTime();
        public string json = "";

        public decimal kiymetBedeli = 0;
        public decimal ilkBedeli = 0;
        public decimal satisBedeli = 0;
        public decimal amortismanTutari = 0;
        public decimal amortismanTutariEdf = 0;
        public string tur = "";
        public string yer = "";
        public string banknot = "";
        public decimal kiymetBedeliEdf = 0;
        public decimal amortismanBedeli = 0;
        public decimal amortismanBedeliEdf = 0;

        public decimal birikmisAmortisman = 0;
        public decimal birikmisAmortismanEdf = 0;

        public string turEdf = "";
        public string turEdfa = "";
        public string tura = "";
        public string turKedf = "";
        public int islemCinsiKod = 0;
        public string aciklama = "";
        public bool mutemmim = false;
        public decimal amortisman1 = 0;
        public decimal amortisman2 = 0;
        public decimal amortismanSos = 0;
        public decimal amortismanOzdr = 0;


        public TNSDateTime sorguTarih1 = new TNSDateTime();
        public TNSDateTime sorguTarih2 = new TNSDateTime();

        public MuhasebeIslemiKriter kriter = new MuhasebeIslemiKriter();
        public ObjectArray detay = new ObjectArray();

        public virtual bool Equals(Object obj)
        {
            if (obj is MuhasebeIslemi)
            {
                MuhasebeIslemi m2 = (MuhasebeIslemi)obj;
                if (this.islemCinsiKod == m2.islemCinsiKod && this.tur == m2.tur) return true;
            }
            return false;
        }

        public void Sifirla()
        {
            this.amortismanBedeli = 0;
            this.amortismanBedeliEdf = 0;
            this.amortismanTutari = 0;
            this.amortismanTutariEdf = 0;
            this.kiymetBedeli = 0;
            this.kiymetBedeliEdf = 0;
            this.satisBedeli = 0;
        }

        public bool SifirMi()
        {
            if (this.amortismanBedeli == 0 &&
            this.amortismanBedeliEdf == 0 &&
            this.amortismanTutari == 0 &&
            this.amortismanTutariEdf == 0 &&
            this.kiymetBedeli == 0 &&
            this.kiymetBedeliEdf == 0 &&
            this.satisBedeli == 0) return true;

            return false;
        }

        public void UstuneTop(MuhasebeIslemi m2)
        {
            this.amortismanBedeli += m2.amortismanBedeli;
            this.amortismanBedeliEdf += m2.amortismanBedeliEdf;
            this.amortismanTutari += m2.amortismanTutari;
            this.amortismanTutariEdf += m2.amortismanTutariEdf;
            this.kiymetBedeli += m2.kiymetBedeli;
            this.kiymetBedeliEdf += m2.kiymetBedeliEdf;
            this.satisBedeli += m2.satisBedeli;
        }
    }

    [Serializable()]
    public class MuhasebeIslemiDetay
    {
        public string refKod = "";
        public int prSicilNo = 0;
        public string islemRefNo = "";
    }

    [Serializable()]
    public class MuhasebeIslemiKriter
    {
        public int yil = 0;
        public int donem = 0;
        public string muhasebeKod = "";
        public string muhasebeAd = "";
        public string harcamaKod = "";
        public string harcamaAd = "";
        public string ambarKod = "";
        public string ambarAd = "";
        public string fisNo = "";
        public int prSicilNo = 0;
        public string hesapPlanKod = "";
        public string hesapPlanAd = "";
        public int kdvOran = 0;
        public decimal kdvliBirimFiyat = 0;
        //public decimal fiyat = 0;
        public string nereyeGitti = "";
        public int islemTurKod = 0;
        public string gayriMenkulTurKod = "";
        public int islemTipTur = 0;
        public int islemTipKod = 0;
        public decimal satisBedeli = 0;
        public ENUMMuhasebeIslemTur muhasebeIslemTur = 0;
        public string aciklama = "";
        public decimal ilkBedel = 0;
        public int geciciIhrac = 0;
        public int mutemmim = 0;
        public int durum = 0;
        public decimal tasitAmortismanSiniri = 0;

        public TNSDateTime edfTarih = new TNSDateTime();

        public string valor = "";
        public TNSDateTime girisZaman = new TNSDateTime();
        public string girisSicil = "";
        public TNSDateTime bOnayZaman = new TNSDateTime();
        public string bOnaySicil = "";
        public TNSDateTime aOnayZaman = new TNSDateTime();
        public string aOnaySicil = "";

        public TNSCollection listeDegerArtis = new TNSCollection();
        public TNSCollection listeAmortisman = new TNSCollection();
    }
    [Serializable()]
    public class BAOnay
    {
        public int yil = 0;
        public string muhasebeKod = "";
        public string muhasebeAd = "";
        public string harcamaKod = "";
        public string harcamaAd = "";
        public string ambarKod = "";
        public string ambarAd = "";
        public string fisNo = "";
        public TNSDateTime fisTarih = new TNSDateTime();

        public string gonderenKisi = "";
        public TNSDateTime gonderimTarih = new TNSDateTime();

        public string bOnaylayan = "";
        public TNSDateTime bOnayTarih = new TNSDateTime();

        public string aOnaylayan = "";
        public TNSDateTime aOnayTarih = new TNSDateTime();

        public int onayDurum = 0;
    }

    [Serializable()]
    public class GeciciIhrac
    {
        public int prSicilNo = 0;
        public int yil = 0;
        public string muhasebeKod = "";
        public string harcamaKod = "";
        public string ambarKod = "";
        public string fisNo = "";
    }

    /// <summary>
    /// Hurda raporlarının bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class HurdaRaporu
    {
        /// <summary>
        /// İl kodu kodu
        /// </summary>
        public string ilKod;

        /// <summary>
        /// İl adı 
        /// </summary>
        public string ilAd;

        /// <summary>
        /// hesap plan kodu
        /// </summary>
        public string hesapPlanKod;

        /// <summary>
        /// hesap plan kodu
        /// </summary>
        public string hBirimKod;

        /// <summary>
        /// hesap plan Adı
        /// </summary>
        public string hesapPlanAd;

        /// <summary>
        /// miktar
        /// </summary>
        public decimal miktar;

        /// <summary>
        /// tutar = miktar*birimTutarKDV
        /// </summary>
        public decimal tutar;

        /// <summary>
        /// seviye
        /// </summary>
        public int seviye;

        /// <summary>
        /// Parametresiz TasinirIslemMIF nesnesi oluşturucu yordam
        /// </summary>
        public HurdaRaporu()
        {
            ilKod = string.Empty;
            ilAd = string.Empty;
            hesapPlanKod = string.Empty;
            hesapPlanAd = string.Empty;
            miktar = (decimal)0;
            tutar = (decimal)0;
            seviye = 0;
        }
    }

    [Serializable()]
    public class AyarlarMB
    {
        public string muhasebeKod = "";
        public string muhasebeAd = "";
        public string harcamaKod = "";
        public string harcamaAd = "";
        public string ambarKod = "";
        public string ambarAd = "";
        public string islemYapan = "";
        public TNSDateTime islemTarihi = new TNSDateTime();
        public TNSDateTime fisGirisKapanisTarihi = new TNSDateTime();
        public TNSDateTime mutemmimBitisTarihi = new TNSDateTime();
        public int aOnayiMuhasebeKontrolu = 0;

    }

    [Serializable()]
    public class Harmoni
    {
        public string sicilNo = "";
        public ENUMHarmoniTip tip = 0;
        public string aciklama = "";
        public TNSDateTime islemTarih = new TNSDateTime();
    }

    [Serializable()]
    public class DuranHareketTablosuMB
    {
        public int yil = 0;
        public string islemAd = "";
        public decimal gayrimenkul = 0;
        public decimal demirbas = 0;
        public decimal yazilim = 0;
        public decimal toplam { get { return gayrimenkul + demirbas + yazilim; } }

        public DuranHareketTablosuMB() { }
        public DuranHareketTablosuMB(string islemAd)
        {
            this.islemAd = islemAd;
        }
        public DuranHareketTablosuMB(string islemAd, int yil)
        {
            this.islemAd = islemAd;
            this.yil = yil;
        }

    }

    [Serializable()]
    public class MuhasebeBakiyeKarsilastirmaMB
    {
        public MuhasebeBakiyeKarsilastirmaDetayMB demirbas = new MuhasebeBakiyeKarsilastirmaDetayMB();
        public MuhasebeBakiyeKarsilastirmaDetayMB yazilim = new MuhasebeBakiyeKarsilastirmaDetayMB();
        public MuhasebeBakiyeKarsilastirmaDetayMB gayrimenkul = new MuhasebeBakiyeKarsilastirmaDetayMB();
    }

    [Serializable()]
    public class MuhasebeBakiyeKarsilastirmaDetayMB
    {
        public decimal bedel = 0;
        public decimal enflasyonDuzeltmeFarki = 0;
        public decimal sonBedeli = 0;
        public decimal amortisman = 0;
        public decimal birikmisAmortismanFarki = 0;
        public decimal duzelmisBirikenAmortisman = 0;
    }


    [Serializable()]
    public class TCCBEntegrasyon
    {
        public string kod = "";
        public int yil = 0;
        public string muhasebeKod = "";
        public string harcamaKod = "";
        public string fisNo = "";
        public string referansNo = "";
        public int silindi = 0;
    }

    [Serializable()]
    public class EnflasyonDuzeltmesi
    {
        public string kod = "";
        public int yil = 0;
        public int ay = 0;
        public double yiUfe = 0;
        public double sinir = 0;
    }

    [Serializable()]
    public class KontrolRaporu
    {
        public int raporTur = 0;
        public int prSicilNo = 0;
        public int yil = 0;
        public string muhasebeKod = "";
        public string muhasebeAd = "";
        public string harcamaKod = "";
        public string harcamaAd = "";
        public string fisNo = "";
        public double kdvliBirimFiyatTif = 0;
        public double kdvliBirimFiyatIlk = 0;
        public double kdvliBirimFiyatSon = 0;
        public string sicilNo = "";
        public string sicilNoAd = "";
    }


    [Serializable()]
    public class EBYSCumhurbaskanligi
    {
        public int evrakId = 0;
        public int yil = 0;
        public string muhasebeKod = "";
        public string harcamaKod = "";
        public string fisNo = "";
        public int belgeTur = 0;
        public string islemYapan = "";
        public TNSDateTime islemTarih = new TNSDateTime();
        public int durum = 0;
        public string dosyaUzantisi = "";
        public string dosyaAdi = "";
        public Byte[] dosyaByteArray = null;
    }

    [Serializable()]
    public class EBYSCumhurbaskanligiBirim
    {
        public int kod = 0;
        public string ad = "";
        public string muhasebeKod = "";
        public string harcamaKod = "";
    }

    [Serializable()]
    public class RFIDHareket
    {
        public string kod = "";
        public int prSicilNo = 0;
        public int etiketTurKod = 0;
        public TNSDateTime islemTarih = new TNSDateTime();
        public string islemYapan = "";
        public int islemTur = 0;
        public int durum = 0;
    }


    [Serializable()]
    public class RFIDYazici
    {
        public int kod = 0;
        public string muhasebeKod = "";
        public string harcamaBirimKod = "";
        public string ip = "";
        public string ad = "";
    }
}