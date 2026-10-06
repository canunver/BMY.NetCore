using System;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.KYM;
using System.Data;
using System.Collections;

namespace TNS.BKD
{

    public enum ENUMTavanTur : int
    {
        tavanTurEkoSektorBirim = 1,
        tavanTurEkoFaaliyet = 2,
        tavanTurTertipYatirim = 3,
        tavanTurYatirimVize = 4,
        tavanTurYatirimVizeTertip = 5,
        tavanTurTertipCari = 6,
        tavanTurGorusmeCari = 7,

        tavanTurSeyyarDOSE = 10,
        tavanTurSeyyarIlTavan = 11,

        tavanTurIOIProje = 21,
        tavanTurYIKOBProje = 22,

        YATProjeTanim = 51,
        YATProjeYil = 52,

        yollukPlanlamaForm = 61,

        stratejikPlan = 71,
        stratejikPlanIzleme = 72,
    }
    public enum ENUMRevizeFormTip
    {
        Hepsi = 0,
        MasrafPlaniRevize = 1,
        MasrafPlaniAktarma = 7,
        MasrafPlaniAHP = 9,
        AHPRevize = 2,
        Aktarma = 3,
        KurumAktarma = 8,
        Ekleme = 4,
        Serbest = 5,
        Iptal = 6,
        AktarmaEklemeler = 97,
        MasrafPlaniIslemleri = 98,
        BumkoIslemleri = 99
    }


    public enum ENUMRevizeFormDurum
    {
        Musvette = 6,
        Taslak = 1,
        Iletildi = 2,
        Onayli = 5,
        Iptal = 9,
        EskiAsama = 7,
        IslemleIptal = 8,
        IptalOlmayan = 99,
        TANIMSIZ = 88,
    }


    /// <summary>
    /// Alt faaliyet kimlik bilgileri ile alt faaliyetle faaliyet ve alt proje kaynak arasındaki ilişkileri tutan sınıf.
    /// </summary>
    [System.Serializable()]
    public class PrgKodYapi
    {
        public int yil { get; set; } = 0;

        public int kodTur { get; set; } = 0;

        public int kod { get; set; } = 0;

        public string ad { get; set; } = "";

        public int tur { get; set; } = 0;

        public string fonKod { get; set; } = "";
        public string sorguKriter { get; set; } = "";

        public string ilgiliBirimKod { get; set; } = "";
    }

    [System.Serializable()]
    public class PrgPebIliski
    {
        public int yil { get; set; } = 0;

        public string pebKod { get; set; } = "";

        public int prgKod { get; set; } = 0;
        public string prgAdi { get; set; } = "";
        public int altPrgKod { get; set; } = 0;
        public string altPrgAdi { get; set; } = "";
        public int faalKod { get; set; } = 0;
        public string faalAdi { get; set; } = "";
        public int altFaalKod { get; set; } = 0;
        public string altFaalAdi { get; set; } = "";
        public string araMetin { get; set; } = "";
        public string pebAd { get; set; }

        public string fonKod { get; set; }
        public string faalFonKod { get; set; }

        public bool fonUyumsuz { get; set; }
    }

    public enum EnumPrgKodYapi
    {
        PROGRAM = 1,
        ALTPROGRAM = 2,
        FAALIYET = 3,
        ALTFAALIYET = 4,
    }
    [System.Serializable()]
    public class MPRevizeSatir
    {
        public Tertip tertip;
        public double[] odenek = new double[12];
        public double[] karsilananOdenek = null;

        public static int EklenenSatirBul(int baslaSatir, MPRevizeSatir[] satirlar)
        {
            if (satirlar == null) return -1;
            for (int i = baslaSatir + 1; i < satirlar.Length; i++)
            {
                MPRevizeSatir sat = satirlar[i];
                if (sat.NetToplam() > 0)
                    return i;
            }
            return -1;
        }

        public static int DusulenUyumluSatirBul(int baslaSatir, MPRevizeSatir[] satirlar, MPRevizeSatir mPRevizeSatir)
        {
            if (satirlar == null) return -1;
            for (int i = baslaSatir + 1; i < satirlar.Length; i++)
            {
                MPRevizeSatir sat = satirlar[i];
                if (sat.NetToplam() < 0)
                    if (sat.Eko2UyumluMu(mPRevizeSatir))
                        return i;
            }
            return -1;
        }

        public static int AyaEklenenUyumluSatirBul(int baslaSatir, MPRevizeSatir[] satirlar, MPRevizeSatir mPRevizeSatir, int ay)
        {
            if (satirlar == null) return -1;
            for (int i = baslaSatir + 1; i < satirlar.Length; i++)
            {
                MPRevizeSatir sat = satirlar[i];
                if (sat.odenek[ay] > 0)
                    if (sat.Eko2UyumluMu(mPRevizeSatir))
                        return i;
            }
            return -1;
        }

        private double NetTopla(double[] dizi)
        {
            double toplamEkle = 0;
            double toplamDus = 0;
            if (dizi != null)
            {
                for (int i = 0; i < dizi.Length; i++)
                {
                    if (dizi[i] < 0)
                        toplamDus -= dizi[i];
                    else
                        toplamEkle += dizi[i];
                }
            }
            return toplamEkle - toplamDus;
        }

        private double Topla(double[] dizi)
        {
            double toplamEkle = 0;
            double toplamDus = 0;
            if (dizi != null)
            {
                for (int i = 0; i < dizi.Length; i++)
                {
                    if (dizi[i] < 0)
                        toplamDus -= dizi[i];
                    else
                        toplamEkle += dizi[i];
                }
            }
            return Math.Max(toplamEkle, toplamDus);
        }

        public double Toplam()
        {
            return Topla(odenek);
        }

        public double NetToplam()
        {
            return this.NetTopla(odenek);
        }

        public double ToplamKarsilanan()
        {
            return Topla(karsilananOdenek);
        }

        public double NetToplamKarsilanan()
        {
            return this.NetTopla(karsilananOdenek);
        }

        internal bool Eko2UyumluMu(MPRevizeSatir revizeSatir)
        {
            int eko2Uz = revizeSatir.tertip.KodUzunlukBul(3, 2, true);

            if (revizeSatir.tertip.kodlar.Length == tertip.kodlar.Length && tertip.kodlar.Length > 4)
            {
                if (revizeSatir.tertip.kodlar[0] == tertip.kodlar[0] && revizeSatir.tertip.kodlar[1] == tertip.kodlar[1] && revizeSatir.tertip.kodlar[2] == tertip.kodlar[2])
                {
                    if (revizeSatir.tertip.kodlar[3].Replace(".", "").Substring(0, eko2Uz).Equals(tertip.kodlar[3].Replace(".", "").Substring(0, eko2Uz)))
                        return true;
                }
            }
            return false;
        }

        public double Dusulen
        {
            get
            {
                return EklenenDusulen(false);
            }
        }

        public double Eklenen
        {
            get
            {
                return EklenenDusulen(true);
            }
        }

        public double EklenenDusulen(bool eklenen)
        {
            double sonuc = 0;
            for (int i = 0; i < odenek.Length; i++)
            {
                double deger = odenek[i];
                if (eklenen && deger > 0) sonuc += deger;
                else if (!eklenen && deger < 0) sonuc -= deger;
            }
            return sonuc;
        }

        internal void KodAdBul(IBKDServis bkdServis, Hashtable ht, int kodNo)
        {
            if (!string.IsNullOrWhiteSpace(tertip.kodlar[kodNo]))
            {
                if (!ht.Contains(tertip.kodlar[kodNo]))
                {
                    ht[tertip.kodlar[kodNo]] = bkdServis.KodAdBul(tertip.yil, tertip.KodNolar(kodNo).ToString(), tertip.kodlar[kodNo]);
                }
                tertip.adlar[kodNo] = (string)ht[tertip.kodlar[kodNo]];
            }
        }

        public int grup = 1;

        public string sektorKod { get; set; }

        public string sektorAd { get; set; }

        public string bolgeler { get; set; }

        public string DPTPROJEALTPROJENO
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DPTPROJENO)) return "";
                return DPTPROJENO + "-" + ALTPROJENO.ToString();
            }
        }
        public int IOIISKOD { get; set; } = 0;
        public string IOIISAD { get; set; }

        public int ALTPROJENO { get; set; }
        public string ALTPROJEAD { get; set; }
        public string DPTPROJENO { get; set; }

        public bool SifirdanBuyukVarMi()
        {
            for (int i = 0; i < odenek.Length; i++)
            {
                double deger = odenek[i];
                if (deger > 0) return true;
            }
            return false;
        }
    }


    [System.Serializable()]
    public class MPRevizeForm
    {
        public int yil { get; set; }
        public int revizeNo { get; set; }
        public ENUMRevizeFormTip revizeFormTip = ENUMRevizeFormTip.Hepsi;
        public ENUMRevizeFormDurum formDurum = ENUMRevizeFormDurum.Musvette;
        public string kaydedenKullaniciKodu { get; set; }
        public TNSDateTime kayitTarihi;
        public string onaylayanKullaniciKodu { get; set; }
        public TNSDateTime onaylamaTarihi;
        public DateTime onayTarihDT { get { if (onaylamaTarihi != null) return onaylamaTarihi.Oku(); else return DateTime.MinValue; } }

        public double odenekToplam { get; set; }

        public MPRevizeSatir[] satirlar;

        public TNSDateTime evrakTarih;
        public DateTime evrakTarihDT { get { if (evrakTarih != null) return evrakTarih.Oku(); else return DateTime.MinValue; } }
        public string evrakNo;
        public string evrakIlgi;
        public string aciklama { get; set; }
        public string bumkoKurumKod;
        public string bumkoIslemNo;
        public string BUMKONo { get { return bumkoKurumKod + "-" + bumkoIslemNo; } }
        public string altIslemKod { get; set; }
        public string yedekTertipStr;
        public int ilgiliRevizeNo;
        public string dayanakKodlar;

        public int odGonYil;
        public int odGonTur;
        public string odGonNo;

        public bool projelerArasi { get; set; }
        public bool sektorlerArasi { get; set; }
        public bool bolgelerArasi { get; set; }

        public string onayYaziNo { get; set; }

        public TNSDateTime onayYaziTarih;
        public DateTime onayYaziTarihDT
        {
            get { if (onayYaziTarih != null) return onayYaziTarih.Oku(); else return DateTime.MinValue; }
            set { if (onayYaziTarih == null) onayYaziTarih = new TNSDateTime(value); else onayYaziTarih.Yaz(value); }
        }

        public string durumAd
        {
            get
            {
                if (formDurum == ENUMRevizeFormDurum.Musvette) return "Müsvette";
                else if (formDurum == ENUMRevizeFormDurum.Iptal) return "İptal";
                else if (formDurum == ENUMRevizeFormDurum.Onayli) return "Onaylı";
                else if (formDurum == ENUMRevizeFormDurum.Taslak) return "Taslak";
                return "";
            }
        }
        public bool DegistirilebilirMi()
        {
            if (formDurum == ENUMRevizeFormDurum.IslemleIptal || formDurum == ENUMRevizeFormDurum.Iptal || formDurum == ENUMRevizeFormDurum.Onayli)
                return false;
            else
                return true;
        }

        public bool DegistirilebilirMi(Kullanici kullanan, ENUMRevizeFormDurum yeniFormDurum)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            if (DegistirilebilirMi())
                return true;
            else if (yeniFormDurum == ENUMRevizeFormDurum.Musvette)
                return true;
            return false;
        }

        public MPRevizeForm()
        {
            altIslemKod = "00.00.00";
        }

        public string formTipAd
        {
            get
            {
                return Arac.RevizeFormTipAl(revizeFormTip);
            }
        }

        public string FaaliyetAdlariBulDusulen
        {
            get
            {
                string donDeger = "";
                for (int i = 0; i < satirlar.Length; i++)
                {
                    if (satirlar[i].NetToplam() < 0)
                    {
                        AdEkle(ref donDeger, satirlar[i].tertip.adlar[4]);
                    }
                }
                return donDeger;
            }
        }

        private void AdEkle(ref string donDeger, string ekle)
        {
            if (donDeger.IndexOf(ekle) < 0)
            {
                if (!string.IsNullOrWhiteSpace(donDeger)) donDeger += " ve ";
                donDeger += "\"" + ekle + "\"";
            }
        }

        public string FaaliyetAdlariBulEklenen
        {
            get
            {
                string donDeger = "";
                for (int i = 0; i < satirlar.Length; i++)
                {
                    if (satirlar[i].NetToplam() > 0)
                    {
                        AdEkle(ref donDeger, satirlar[i].tertip.adlar[4]);
                    }
                }
                return donDeger;
            }
        }

        public double ToplamEklenen
        {
            get
            {
                return ToplamBul(true);
            }
        }

        private double ToplamBul(bool eklenen)
        {
            double toplam = 0;
            for (int i = 0; i < satirlar.Length; i++)
            {
                toplam += satirlar[i].EklenenDusulen(eklenen);
            }
            return toplam;
        }

        public void AdlariBul(IBKDServis bkdServis, int kodNo)
        {
            Hashtable ht = new Hashtable();
            for (int i = 0; i < satirlar.Length; i++)
            {
                satirlar[i].KodAdBul(bkdServis, ht, kodNo);
            }
        }
    }


    /// <summary>
    /// Tavan: 
    /// </summary>
    [System.Serializable()]
    public class TavanGenel : IComparable, IComparer
    {
        /// <summary>
        /// Kodları tutan dizi
        /// </summary>
        public string[] kodlar = new string[6];

        /// <summary>
        /// Adlar: kod karşılığı adları tutan dizi
        /// </summary>
        public string[] adlar = new string[6];

        /// <summary>
        /// tavanları tutan dizi
        /// </summary>
        public double[] tavanlar = new double[3];

        public string kod1 { get { return kodlar[0]; } set { kodlar[0] = value; } }
        public string kod2 { get { return kodlar[1]; } set { kodlar[1] = value; } }
        public string kod3 { get { return kodlar[2]; } set { kodlar[2] = value; } }
        public string kod4 { get { return kodlar[3]; } set { kodlar[3] = value; } }
        public string kod5 { get { return kodlar[4]; } set { kodlar[4] = value; } }
        public string kod6 { get { return kodlar[5]; } set { kodlar[5] = value; } }

        public string ad1 { get { return adlar[0]; } set { adlar[0] = value; } }
        public string ad2 { get { return adlar[1]; } set { adlar[1] = value; } }
        public string ad3 { get { return adlar[2]; } set { adlar[2] = value; } }
        public string ad4 { get { return adlar[3]; } set { adlar[3] = value; } }
        public string ad5 { get { return adlar[4]; } set { adlar[4] = value; } }
        public string ad6 { get { return adlar[5]; } set { adlar[5] = value; } }

        public double yil0 { get { return tavanlar[0]; } set { tavanlar[0] = value; } }
        public double yil1 { get { return tavanlar[1]; } set { tavanlar[1] = value; } }
        public double yil2 { get { return tavanlar[2]; } set { tavanlar[2] = value; } }

        public double odenek_1 { get; set; }
        public double odenek_2 { get; set; }
        public double harcama_2 { get; set; }


        public double talep_0 { get; set; }
        public double ekOdenek { get; set; }
        public double ekOdenek1 { get; set; }
        public double ekOdenek2 { get; set; }
        public double harcama_1 { get; set; }
        public double ysHarcamaTahmin { get; set; }
        //public string ysHarcamaTahminGerekce { get; set; }
        public double teklif0 { get; set; }
        public double teklif1 { get; set; }
        public double teklif2 { get; set; }
        public double ekOdenekTeklif { get; set; }
        public double ekOdenek1Teklif { get; set; }
        public double ekOdenek2Teklif { get; set; }

        public double sgb0 { get; set; }
        public double sgb1 { get; set; }
        public double sgb2 { get; set; }
        public double ekSgb { get; set; }
        public double ekSgb1 { get; set; }
        public double ekSgb2 { get; set; }

        public string ekGerekce { get; set; }
        public string ekGerekceTeklif { get; set; }

        public int projeBitisYili { get; set; }
        public string projeDevYeni { get; set; }
        public int durum { get; set; }

        private bool kod5OzelSirala;
        public TavanGenel(bool kod5OzelSirala)
        {
            this.kod5OzelSirala = kod5OzelSirala;
        }

        public TavanGenel()
        {
            this.kod5OzelSirala = true;
        }

        public string tamKod
        {
            get
            {
                return kod1 + "@" + kod2 + "@" + kod3 + "@" + kod4 + "@" + kod5 + "@" + kod6;
            }

            set
            {
                string[] parcalar = value.Split('@');
                kod1 = OrtakFonksiyonlar.DiziElemanAl(parcalar, 0, "");
                kod2 = OrtakFonksiyonlar.DiziElemanAl(parcalar, 1, "");
                kod3 = OrtakFonksiyonlar.DiziElemanAl(parcalar, 2, "");
                kod4 = OrtakFonksiyonlar.DiziElemanAl(parcalar, 3, "");
                kod5 = OrtakFonksiyonlar.DiziElemanAl(parcalar, 4, "");
                kod6 = OrtakFonksiyonlar.DiziElemanAl(parcalar, 5, "");
            }
        }

        public string ekoSektorAciklama
        {
            get
            {
                string sektorAd;
                if (ad2 == "") sektorAd = "Cari";
                else sektorAd = ad2 + " Sektörü";
                return kod1 + "-" + ad1 + ", " + sektorAd + ": Tavan " + yil0.ToString("#,##0");
            }
        }

        public string kod4Aciklama
        {
            get
            {
                return TertipKod + " " + ad4 + ": Tavan " + yil0.ToString("#,##0");
            }
        }

        public string ekoFinAciklama
        {
            get
            {
                return kod2 + "-" + ad2 + " (" + ad3 + "): " + yil0.ToString("#,##0");
            }
        }

        public string TertipKod
        {
            get
            {
                string retval = "";
                int son = 5;
                for (; son >= 0; son--)
                {
                    if (!string.IsNullOrWhiteSpace(kodlar[son])) break;
                }

                for (int i = 0; i <= son; i++)
                {
                    if (retval != "") retval += "-";
                    retval += kodlar[i];
                }
                return retval;
            }
        }

        public override bool Equals(object obj)
        {
            return CompareTo(obj) == 0;
        }

        public int CompareTo(object obj)
        {
            if (obj is TavanGenel)
            {
                return this.tamKod.CompareTo(((TavanGenel)obj).tamKod);
            }
            return -1;
        }

        public int Compare(object x, object y)
        {
            if (x is TavanGenel && y is TavanGenel)
            {
                TavanGenel xx = (TavanGenel)x;
                TavanGenel yy = (TavanGenel)y;
                int sonuc = -1;
                if (this.kod5OzelSirala)
                {
                    if (string.IsNullOrWhiteSpace(xx.kod5) && string.IsNullOrWhiteSpace(yy.kod5))
                        sonuc = 0;
                    else if (string.IsNullOrWhiteSpace(xx.kod5))
                        sonuc = 1;
                    else if (string.IsNullOrWhiteSpace(yy.kod5))
                        sonuc = -1;
                    else
                        sonuc = xx.kod5.CompareTo(yy.kod5);
                }
                else
                    sonuc = 0;
                if (sonuc == 0)
                    return xx.tamKod.CompareTo(yy.tamKod);
                else
                    return sonuc;
            }
            return -1;
        }

        public void DegerEkle(TavanGenel gelen)
        {
            this.yil0 += gelen.yil0;
            this.yil1 += gelen.yil1;
            this.yil2 += gelen.yil2;
            this.odenek_1 += gelen.odenek_1;
            this.odenek_2 += gelen.odenek_2;
            this.harcama_2 += gelen.harcama_2;
            this.talep_0 += gelen.talep_0;
            this.ekOdenek += gelen.ekOdenek;
            this.ekOdenek1 += gelen.ekOdenek1;
            this.ekOdenek2 += gelen.ekOdenek2;
            this.harcama_1 += gelen.harcama_1;
            this.ysHarcamaTahmin += gelen.ysHarcamaTahmin;
            this.teklif0 += gelen.teklif0;
            this.teklif1 += gelen.teklif1;
            this.teklif2 += gelen.teklif2;
            this.ekOdenekTeklif += gelen.ekOdenekTeklif;
            this.ekOdenek1Teklif += gelen.ekOdenek1Teklif;
            this.ekOdenek2Teklif += gelen.ekOdenek2Teklif;

            this.sgb0 += gelen.sgb0;
            this.sgb1 += gelen.sgb1;
            this.sgb2 += gelen.sgb2;
            this.ekSgb += gelen.ekSgb;
            this.ekSgb1 += gelen.ekSgb1;
            this.ekSgb2 += gelen.ekSgb2;
        }

        public double tavanMemur { get { return sgb0; } set { sgb0 = value; } }
        public double tavanIsci { get { return sgb1; } set { sgb1 = value; } }

        public double tavanSozlesmeli { get { return sgb2; } set { sgb2 = value; } }

        public double dagitilanMemur { get { return ekSgb; } set { ekSgb = value; } }
        public double dagitilanIsci { get { return ekSgb1; } set { ekSgb1 = value; } }
        public double dagitilanSozlesmeli { get { return ekSgb2; } set { ekSgb2 = value; } }

        public double kullanilanMemur { get { return teklif0; } set { teklif0 = value; } }
        public double kullanilanIsci { get { return teklif1; } set { teklif1 = value; } }
        public double kullanilanSozlesmeli { get { return teklif2; } set { teklif2 = value; } }

        public double memurEklenen
        {
            get
            {
                if (yil0 < 0)
                    return 0;
                else
                    return yil0;
            }
            set
            {
                if (value > 0) yil0 = value;
            }
        }

        public double memurDusulen
        {
            get
            {
                if (yil0 > 0)
                    return 0;
                else
                    return 0 - yil0;
            }
            set
            {
                if (value > 0) yil0 = 0 - value;
            }
        }

        public double isciEklenen
        {
            get
            {
                if (yil1 < 0)
                    return 0;
                else
                    return yil1;
            }
            set
            {
                if (value > 0) yil1 = value;
            }
        }

        public double isciDusulen
        {
            get
            {
                if (yil1 > 0)
                    return 0;
                else
                    return 0 - yil1;
            }
            set
            {
                if (value > 0) yil1 = 0 - value;
            }
        }

        public double sozlesmeliEklenen
        {
            get
            {
                if (yil2 < 0)
                    return 0;
                else
                    return yil2;
            }
            set
            {
                if (value > 0) yil2 = value;
            }
        }

        public double sozlesmeliDusulen
        {
            get
            {
                if (yil2 > 0)
                    return 0;
                else
                    return 0 - yil2;
            }
            set
            {
                if (value > 0) yil2 = 0 - value;
            }
        }

        public string senaryoKod { get; set; }

        public int yil { get; set; }

        public double odenek_1_1 { get; set; }

        public double odenek_1_2 { get; set; }
    }


    /// <summary>
    /// Tertip yapılarını tutan tablolar. Her tablo zaman ve senaryo bilgisi ile birlikte tertip yapısından oluşmaktadır.
    /// Tertip yapısı ise bir veya daha fazla kod alanından (boyut) oluşmaktadır. Tertip tanımları yıl bazında değişken.xml'den okunmaktadır.
    /// </summary>
    public enum ENUMTabloKod : int
    {
        /// <summary>
        /// Kur, fon, fin ve eko4'den oluşan ödenekleri tutan tablo
        /// </summary>
        TABOdenek = 0,

        /// <summary>
        /// Kur, fon, fin ve eko4'den oluşan harcamaları tutan tablo
        /// </summary>
        TABHarcama = 1,

        /// <summary>
        /// Kur, fon, fin ve eko4'den oluşan yıl sonu harcama tahminini tutan tablo
        /// </summary>
        TABHarcamaTahmin = 2,

        /// <summary>
        /// Kurum ve gelir eko'dan oluşan gelir ödeneklerini tutan tablo
        /// </summary>
        TABGelir = 3,

        /// <summary>
        /// Kurum ve gelir eko'dan oluşan gelir yıl sonu gerçekleşme tahminini tutan tablo
        /// </summary>
        TABGelirTahmini = 4,

        /// <summary>
        /// Kurum ve gelir eko'dan oluşan gelir gerçekleşmelerini tutan tablo
        /// </summary>
        TABGelirGerceklesmesi = 5,

        /// <summary>
        /// Kurum ve finansman eko'dan oluşan finansman ödeneklerini tutan tablo
        /// </summary>
        TABFinansman = 6,

        /// <summary>
        /// Kur, fon, fin ve eko2'den oluşan ve yıl başı ödenekleri tutan tablo
        /// </summary>
        TABButce = 10,

        /// <summary>
        /// Hesap kodundan oluşan harcama bilgilerini tutan tablo
        /// </summary>
        TABOdemeEmri = 11,

        /// <summary>
        /// AHP için gider icmal bilgilerini tutan tablo
        /// </summary>
        TABGiderIcmal = 12,

        /// <summary>
        /// Taşınır hesap kodundan oluşan tablo
        /// </summary>
        TABTASHesapPlani = 13,


        /// <summary>
        /// Kur, fon, fin, eko4 ve alt faaliyetden oluşan ödenekleri tutan tablo
        /// </summary>
        TABPEBOdenek = 14,

        /// <summary>
        /// Kur, fon, fin, eko4 ve alt faaliyetden oluşan harcamaları tutan tablo
        /// </summary>
        TABPEBHarcama = 15,

        /// <summary>
        /// Kur, fon, fin, eko4, alt faaliyetden ve Kurumsal yapıdan oluşan ödenekleri tutan tablo
        /// </summary>
        TABTasraOdenek = 16,

        /// <summary>
        /// Kur, fon, fin, eko4, alt faaliyetden ve Kurumsal yapıdan oluşan ödenek gönderme bilgilerini tutan tablo
        /// </summary>
        TABOdenekGonderme = 17,

        /// <summary>
        /// Kurum ve gelir eko ve alt faaliyetden oluşan gelir ödeneklerini tutan tablo
        /// </summary>
        TABPEBGelir = 18,

        /// <summary>
        /// Kurum ve gelir eko ve alt faaliyetden oluşan gelir gerçekleşmelerini tutan tablo
        /// </summary>
        TABPEBGelirGerceklesmesi = 19,

        /// <summary>
        /// Birimler tarafından yapılan masraf planında bulunan kodları tutan tablo
        /// </summary>
        TABMasrafPlani = 20,

        TABPEBHAZHarcama = 21,
        TABHAZHarcama = 22,
        TABHYBloke = 23,
        TABOdenekGondermeKY = 24,

        TABTJKBudgetSub = 25,
        TABTJKBudget = 26,

        TABISLHarcama = 27,
        TABISLAvans = 28,

        /// <summary>
        /// Birimler tarafından yapılan masraf planında bulunan kodları tutan tablo
        /// </summary>
        TABMasrafPlaniBirim = 29,

        TABMPTavan = 30,

        TABMasrafPlaniForm = 31,
        TABMIFKY = 32,
        TABPEBTavanUstu = 33,
        TABButceGelir = 34,


        TABPRGOdenek = 35,
        TABPRGHarcamaTahmin = 36,
        TABPRGTavanUstu = 37,
        TABPRGHarcama = 38,
        TABPRGOdemeEmri = 39,
        TABPRGOdemeEmriAvansiz = 40,
        TABPRGMasrafPlaniForm = 43,

        TABGIDERAP = 41,
        TABPRGGIDERAP = 42,

        /// <summary>
        /// Tertip bilgisi bilinmediğinde kullanılacak
        /// </summary>
        TANIMSIZ = -1,
    }

    /// <summary>
    /// KOD bilgilerinin işlenmesi sırasında yapılacak işlemin türü 
    /// </summary>
    public enum ENUMKayitDurumu : int
    {
        /// <summary>
        /// Kayıt yeni, sadece ekleme işlemi yapılacak
        /// </summary>
        YENI = 1,

        /// <summary>
        /// Var olan kaydın bilgisi güncellenecek
        /// </summary>
        GUNCELLE = 2,

        /// <summary>
        /// Var olan kayıt silinecek
        /// </summary>
        SIL = 3,

        /// <summary>
        /// İşlem tipi tanımsız, hiö bir işlem yapılmaz.
        /// </summary>
        TANIMSIZ = -1
    }

    /// <summary>
    /// Tertip Kodlari parametre Nesne Sınıfı
    /// </summary>
    [Serializable()]
    public class TertipKodlari
    {
        public string kurKod;
        public string fonKod;
        public string finKod;
        public string ekoKod;
        public string pebKod;

        public TertipKodlari()
        {
            kurKod = "";
            fonKod = "";
            finKod = "";
            ekoKod = "";
            pebKod = "";
        }
    }

    /// <summary>
    /// ENUMTablokod tanım listesindeki her bir tertip için, tertibi oluşturan kodları, kırılımları gibi bilgilerini tutan sınıf.
    /// </summary>
    [Serializable()]
    public class TertipYapi
    {
        /// <summary>
        /// Tertip yapıları yıldan yıla değiştiği için, tutulan yıl bilgisi
        /// </summary>
        public int yil;

        /// <summary>
        /// Tertibin ilişkili olduğu tablo
        /// </summary>
        public ENUMTabloKod tabloKod;

        /// <summary>
        /// Tanımlanan tetipdeki sınıf sayısı (kaç adet kod olduğu, zaman ve senaryo hariç, boyut sayısı)
        /// </summary>
        public int sinifSay = 0;

        /// <summary>
        /// Eğer tertip kurum kodu içeriyorsa, tüm kurumsal yapı mı içeriyor (false), sadece kurum kodu mu içeriyor (true).
        /// </summary>
        public bool kurumMuKurumsalMi = true;

        /// <summary>
        /// Tertibin her bir kodunun kırılım yapısını tutan dizi.
        /// </summary>
        public string[] kirilimlar;

        /// <summary>
        /// Tertibin her bir kodunun kırılım yapısını tutan dizi.
        /// </summary>
        public string[] uzayAd;

        /// <summary>
        /// Tertibin her bir kodunun kod numarasını tutan dizi.
        /// </summary>
        public string[] kodNolar;

        /// <summary>
        /// Tertibin her bir kodunun, kod adını tutan dizi.
        /// </summary>
        public string[] kodAd;

        public string[] yilYok;
        /// <summary>
        /// Yıl ve numarası verilen kodun, değişken.xml'den kırılım yapısını okuyan yordam.
        /// </summary>
        /// <param name="yil">Bilgisi okunacak kodun yıl bilgisi</param>
        /// <param name="kodNo">Bilgisi okunacak kodun kod numarası</param>
        /// <param name="kisaEko">Okunacak kod ekonomik kod ise, tertip uzun ekonomik koda mı? kısa ekomik koda mı? sahip bilgisi.</param>
        /// <returns>Parametresi verilen kodun kırılım yapısı döndürülür.</returns>
        public static string KodYapiBul(int yil, int kodNo, bool kisaEko)
        {
            string degiskenAd;
            if (kodNo == 4 && kisaEko)
                degiskenAd = "KODYAPIKISA";
            else
                degiskenAd = "KODYAPI";
            return TNS.DEG.Arac.DegiskenDegerBul(yil, "KODYAPI/KOD" + kodNo + "/" + degiskenAd);
        }

        /// <summary>
        /// Yıl ve numarası verilen kodun, değişken.xml'den uzay adını okuyan yordam.
        /// </summary>
        /// <param name="yil">Bilgisi okunacak kodun yıl bilgisi</param>
        /// <param name="kodNo">Bilgisi okunacak kodun kod numarası</param>
        /// <returns>Parametresi verilen kodun uzay adı döndürülür.</returns>
        public static string UzayAdBul(int yil, int kodNo)
        {
            return TNS.DEG.Arac.DegiskenDegerBul(yil, "KODYAPI/KOD" + kodNo + "/VTTABLOADI");
        }

        /// <summary>
        /// Yıl ve numarası verilen kodun, değişken.xml'den adını okuyan yordam.
        /// </summary>
        /// <param name="degServis">Değişken.xml'e erişimde kullanılacak sunucu nesnesi.</param>
        /// <param name="yil">Bilgisi okunacak kodun yıl bilgisi</param>
        /// <param name="kodNo">Bilgisi okunacak kodun kod numarası</param>
        /// <returns>Parametresi verilen kodun adı döndürülür.</returns>
        public static string KodAdBul(int yil, int kodNo)
        {
            return TNS.DEG.Arac.DegiskenDegerBul(yil, "KODYAPI/KOD" + kodNo);
        }

        /// <summary>
        /// Yıl ve numarası verilen kodun, değişken.xml'den adını okuyan yordam.
        /// </summary>
        /// <param name="degServis">Değişken.xml'e erişimde kullanılacak sunucu nesnesi.</param>
        /// <param name="yil">Bilgisi okunacak kodun yıl bilgisi</param>
        /// <param name="kodNo">Bilgisi okunacak kodun kod numarası</param>
        /// <returns>Parametresi verilen kodun adı döndürülür.</returns>
        public static string YilYokBul(int yil, int kodNo)
        {
            return TNS.DEG.Arac.DegiskenDegerBul(yil, "KODYAPI/KOD" + kodNo + "/YILYOK");
        }


        /// <summary>
        /// Verilen yıl ve tablo kodu kullanılarak, tablonun tertip yapısını değişken.xml'den okuyan ve tertip yapı nesnesi oluşturan oluşturucu yordam.
        /// Tertip yapısının hangi kodlardan oluştuğu bulunur ve her bir kodun bilgileri okunarak tertip yapısı oluşturulur.
        /// </summary>
        /// <param name="yil">Tertip yapı bilgisi okunacak tablonun yılı</param>
        /// <param name="tabloKod">Tertip yapı bilgisi okunacak tablonun kodu</param>
        public TertipYapi(int yil, ENUMTabloKod tabloKod)
        {
            string kodNolar = TNS.DEG.Arac.DegiskenDegerBul(yil, "/TERTIPYAPI/TERTIP" + (int)tabloKod + "/YAPI");
            bool kisaEko = TNS.DEG.Arac.DegiskenDegerBul(yil, "/TERTIPYAPI/TERTIP" + (int)tabloKod + "/KISAEKO").Equals("1");
            bool kurumMuKurumsalMi = TNS.DEG.Arac.DegiskenDegerBul(yil, "/TERTIPYAPI/TERTIP" + (int)tabloKod + "/KURUMMU").Equals("1");

            this.yil = yil;
            this.tabloKod = tabloKod;
            this.kodNolar = kodNolar.Split(',');
            this.sinifSay = this.kodNolar.Length;
            this.kurumMuKurumsalMi = kurumMuKurumsalMi;

            kirilimlar = new string[sinifSay];
            uzayAd = new string[sinifSay];
            kodAd = new string[sinifSay];
            yilYok = new string[sinifSay];
            for (int i = 0; i < this.sinifSay; i++)
            {
                kirilimlar[i] = KodYapiBul(this.yil, TNS.OrtakFonksiyonlar.ConvertToInt(this.kodNolar[i], 0), kisaEko);
                uzayAd[i] = UzayAdBul(this.yil, TNS.OrtakFonksiyonlar.ConvertToInt(this.kodNolar[i], 0));
                kodAd[i] = KodAdBul(this.yil, TNS.OrtakFonksiyonlar.ConvertToInt(this.kodNolar[i], 0));
                yilYok[i] = YilYokBul(this.yil, TNS.OrtakFonksiyonlar.ConvertToInt(this.kodNolar[i], 0));
            }
        }
    }

    /// <summary>
    /// Tüm tertiplere ait yapı bilgilerini tutan sınıf.
    /// Sınıfdaki dizide, tertip yapıları tutulur. Bilgisi istenen tertip, dizide yoksa TertipYapı nesnesi oluşturulur.
    /// </summary>
    [Serializable()]
    public class TertipYapiBilgileri
    {
        /// <summary>
        /// TertipYapılarını tutan dizi. Bu dizide bilgisi okunmuş tertiplerin bilgileri durur. Bu sayede bir tertip bir kere okunur.
        /// </summary>
        TNSCollection tertipYapiBilgileri = new TNSCollection();

        /// <summary>
        /// Yıl ve tablokodu verilen tertip bilgisi dizide bulunur ve döndürülür. Eğer bulunamazsa null döndürülür.
        /// </summary>
        /// <param name="yil">Tertip yapı bilgisi bulunacak tablonun yılı</param>
        /// <param name="tabloKod">Tertip yapı bilgisi bulunacak tablonun kodu</param>
        /// <returns>Verilen bilgilerle tertip bilgisi bulunursa, bu bilgiye ait tertip yapı döndürülür. Bulunamazsa null döndürülür.</returns>
        private TertipYapi TertipYapiBulIc(int yil, ENUMTabloKod tabloKod)
        {
            for (int i = 0; i < tertipYapiBilgileri.Count; i++)
            {
                TertipYapi ty = (TertipYapi)tertipYapiBilgileri[i];
                if (ty.yil == yil && ty.tabloKod == tabloKod)
                    return ty;
            }
            return null;
        }

        /// <summary>
        /// Yıl ve tablokodu verilen tertibin sınıf sayısı (kod sayısı) döndürülür. Eğer bulunamazsa 0 döndürülür.
        /// </summary>
        /// <param name="yil">Tertibindeki sınıf sayısı bilgisi bulunacak tablonun yılı</param>
        /// <param name="tabloKod">Tertip yapı bilgisi bulunacak tablonun kodu</param>
        /// <returns>Verilen bilgilerle tertip bilgisi bulunursa, bu bilgiye ait sınıf sayısı (kod sayısı) döndürülür. Bulunamazsa 0 döndürülür.</returns>
        public int SinifSayisi(int yil, ENUMTabloKod tabloKod)
        {
            TertipYapi ty = TertipYapiBul(yil, tabloKod);
            if (ty != null)
            {
                return ty.sinifSay;
            }
            else
                return 0;
        }

        /// <summary>
        /// Yıl ve tablokodu verilen tertibe ait bilgi döndürülür. Eğer bulunamazsa null döndürülür.
        /// Tertip yapısı öncelikle dizide aranır. Dizide bulunamazsa, değişken.xml'den okunarak yeni yapı oluşturulur ve diziye eklenir.
        /// </summary>
        /// <param name="yil">Tertip yapı bilgisi bulunacak tablonun yılı</param>
        /// <param name="tabloKod">Tertip yapı bilgisi bulunacak tablonun kodu</param>
        /// <returns>Yıl ve tablokodu verilen tertibe ait bilgi döndürülür. Bulunamazsa null döndürülür.</returns>
        public TertipYapi TertipYapiBul(int yil, ENUMTabloKod tabloKod)
        {
            TertipYapi ty = TertipYapiBulIc(yil, tabloKod);
            if (ty == null)
            {
                ty = new TertipYapi(yil, tabloKod);
                if (ty != null)
                {
                    tertipYapiBilgileri.Add(ty);
                }
            }
            return ty;
        }
    }

    public enum ENUMTertipDurum
    {
        NORMAL = 1,
        ALIR = 2,
        VERIR = 3,
        ALIR_VERIR = 4,
        IPTAL = 99,
        TANIMSIZ = 88
    }

    public class TertipSiralaEko2Peb : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            if (x.GetType() == typeof(Tertip) && y.GetType() == typeof(Tertip))
            {
                return Arac.TertipKodKiyaslaEko2Peb(((Tertip)x).kodlar, ((Tertip)y).kodlar);
            }
            return 0;
        }
    }

    /// <summary>
    /// Yıl ve kodu verilen bir tertibe ait, tertibin kodlarını, adlarını ve tertip adını tutan sınıf.
    /// </summary>
    [Serializable()]
    public class Tertip
    {
        /// <summary>
        /// Verilen tertibin yılı. Bu sayede tertip yapısı bulunur.
        /// </summary>
        public int yil;

        /// <summary>
        /// Verilen tertibin kodu. Bu sayede tertip yapısı bulunur.
        /// </summary>
        public ENUMTabloKod tabloKod;

        /// <summary>
        /// Tertibin kodlarını tutan dizi.
        /// </summary>
        public string[] kodlar;

        /// <summary>
        /// Verilen kodlara karşılık isimleri tutan dizi
        /// </summary>
        public string[] adlar;

        /// <summary>
        /// Verilen tertibin adını tutan değişken
        /// </summary>
        public string tertipAdi;

        /// <summary>
        /// Tertibin durumu (Giriş, Görünür, Gizli)
        /// </summary>
        public ENUMTertipDurum durum = ENUMTertipDurum.TANIMSIZ;

        /// <summary>
        /// Verilen yıl ve tablokodu kullanılarak, tertip nesnesi oluşturulur.
        /// </summary>
        /// <param name="yil">Tertip nesnesi oluşturulcak tablonun yılı</param>
        /// <param name="tabloKod">Tertip nesnesi oluşturulcak tablonun kodu</param>
        public Tertip(int yil, ENUMTabloKod tabloKod)
        {
            this.yil = yil;
            this.tabloKod = tabloKod;
            TertipYapi ty = Arac.tertipYapiBilgileri.TertipYapiBul(yil, tabloKod);
            kodlar = new string[ty.sinifSay];
            adlar = new string[ty.sinifSay];
        }

        /// <summary>
        /// Verilen yıl ve tablokodu kullanılarak, tertip nesnesi oluşturulur. Verilen kod dizisi, sırayla kod alanlarına atanır. (- ile ayrılmış kodlar kullanılmaz)
        /// </summary>
        /// <param name="yil">Tertip nesnesi oluşturulacak tablonun yılı</param>
        /// <param name="tabloKod">Tertip nesnesi oluşturulacak tablonun kodu</param>
        /// <param name="kodDizisi">Tertip kodlarına atanacak kod dizisi</param>
        public Tertip(int yil, ENUMTabloKod tabloKod, params string[] kodDizisi)
            : this(yil, tabloKod)
        {
            TertipAta(kodDizisi);
        }

        /// <summary>
        /// Verilen tablokodu ve tablokodu için bulunan varsayılan yıl kullanılarak, tertip nesnesi oluşturulur.
        /// </summary>
        /// <param name="tabloKod">Tertip nesnesi oluşturulcak tablonun kodu</param>
        [Obsolete("Lüften yil, tablokod olan constructor'ı kullanınız!")]
        public Tertip(ENUMTabloKod tabloKod)
            : this(VarsayilanYilBul(tabloKod), tabloKod)
        {
        }

        /// <summary>
        /// TABOdenek ve TABODenek için bulunan varsayılan yıl kullanılarak, tertip nesnesi oluşturulur.
        /// </summary>
        [Obsolete("Lüften yil, tablokod olan constructor'ı kullanınız!")]
        public Tertip()
            : this(ENUMTabloKod.TABOdenek)
        {
        }

        /// <summary>
        /// Yıl verilmeden tertip oluşturulursa, tablo için kullanılacak yıl bulan yordam.
        /// </summary>
        /// <param name="tabloKod">Tertip yapısının bulunması için kullanılacak varsayılan yıl bulunacak tablo.</param>
        /// <returns>Verilen tablo için, Tertip yapısının bulunmasında kullanılacak yıl.</returns>
        public static int VarsayilanYilBul(ENUMTabloKod tabloKod)
        {
            return System.DateTime.Now.Year;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == typeof(Tertip))
            {
                Tertip gelTertip = (Tertip)obj;
                if (this.yil == gelTertip.yil && this.tabloKod == gelTertip.tabloKod && this.kodlar.Length == gelTertip.kodlar.Length)
                {
                    for (int i = 0; i < kodlar.Length; i++)
                    {
                        if (!this.kodlar[i].Replace(".", "").Equals(gelTertip.kodlar[i].Replace(".", "")))
                            return false;
                    }
                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// Tertibin verilen sıradaki kodunun kırılım yapısı döndüren yordam.
        /// </summary>
        /// <param name="yer">Kırılım yapısı döndürülecek kodun sırası</param>
        /// <returns>Tertibin verilen sıradaki kodunun kırılım yapısı döndürülür.</returns>
        public string Kirilimlar(int yer)
        {
            TertipYapi ty = Arac.tertipYapiBilgileri.TertipYapiBul(this.yil, this.tabloKod);
            return ty.kirilimlar[yer];
        }

        /// <summary>
        /// Tertibin verilen sıradaki kodunun, kod numarasını döndüren yordam.
        /// </summary>
        /// <param name="yer">Kod numarası döndürülecek kodun sırası</param>
        /// <returns>Tertibin verilen sıradaki kodunun, kod numarası döndürülür.</returns>
        public int KodNolar(int yer)
        {
            TertipYapi ty = Arac.tertipYapiBilgileri.TertipYapiBul(this.yil, this.tabloKod);
            return OrtakFonksiyonlar.ConvertToInt(ty.kodNolar[yer], -1);
        }

        /// <summary>
        /// Tertibin verilen sıradaki kodunun, kırılıml yapısındaki kırılım sayısını döndüren yordam.
        /// </summary>
        /// <param name="yer">Kırılım sayısı döndürülecek kodun sırası.</param>
        /// <returns>Tertibin verilen sıradaki kodunun, kırılım sayısı döndürülür.</returns>
        public int KodNoKirilimSay(int yer)
        {
            TertipYapi ty = Arac.tertipYapiBilgileri.TertipYapiBul(this.yil, this.tabloKod);
            return ty.kirilimlar[yer].Split('.').Length;
        }

        public void Tertip23Ayarla(int yer)
        {
            string[] kodDizi = this.kodlar[yer].Split('.');
            if (kodDizi.Length == 3)
            {
                this.kodlar[yer] = kodDizi[0] + "." + kodDizi[1] + kodDizi[2];
            }
        }

        /// <summary>
        /// Kod dizisindeki eleman sayısını döndüren yordam.
        /// </summary>
        /// <returns>Kod dizisindeki eleman sayısı döndürülür.</returns>
        public int KodNoSay()
        {
            return kodlar.Length;
        }

        /// <summary>
        /// Tertip içi kod ve ad dizileri, tertibin sınıf sayısı (kod sayısı) kadar, boş olmayanları, boş olacak şekilde oluşturan yordam.
        /// </summary>
        public void DiziKontrol()
        {
            for (int i = 0; i < Arac.tertipYapiBilgileri.SinifSayisi(yil, tabloKod); i++)
            {
                if (kodlar[i] == null) kodlar[i] = "";
                if (adlar[i] == null) adlar[i] = "";
            }
        }

        /// <summary>
        /// Tertip içindeki kodların noktaları atılır.
        /// </summary>
        public void NoktalariAt()
        {

            for (int i = 0; i < Arac.tertipYapiBilgileri.SinifSayisi(yil, tabloKod); i++)
            {
                kodlar[i] = Arac.NoktalariAt((string)kodlar[i]);
            }
        }

        /// <summary>
        /// Tertip içindeki kodların içeriği, kodun kırılım yapısına göre yeniden oluşturulur.
        /// </summary>
        public void Noktala()
        {
            TertipYapi ty = Arac.tertipYapiBilgileri.TertipYapiBul(yil, tabloKod);
            if (ty == null) return;

            for (int i = 0; i < ty.sinifSay; i++)
            {
                if (!String.IsNullOrEmpty(kodlar[i]))
                    kodlar[i] = OrtakFonksiyonlar.KodNoktaKoy(ty.kirilimlar[i], kodlar[i].Replace(".", ""));
            }
        }

        /// <summary>
        /// Her kod için o kodun uzay adı ve verilen uzay kullanılarak, tertip içindeki kodların isimleri bulunur ve ad dizisi doldurulur.
        /// </summary>
        /// <param name="uzyServis">Adları bulmak için kullanılacak uzay nesnesi</param>
        public void AdlariBul(TNS.UZY.IUZYServis uzyServis, System.Collections.Hashtable[] kodListesi)
        {
            TertipYapi ty = Arac.tertipYapiBilgileri.TertipYapiBul(yil, tabloKod);
            if (ty == null) return;

            for (int i = 0; i < ty.sinifSay; i++)
            {
                if (string.IsNullOrEmpty(kodlar[i]))
                    adlar[i] = "";
                else
                {
                    string bulunan;
                    if (kodListesi != null && kodListesi.Length > i)
                    {
                        if (kodListesi[i].ContainsKey(kodlar[i]))
                        {
                            bulunan = (string)kodListesi[i][kodlar[i]];
                        }
                        else
                            bulunan = null;
                    }
                    else
                        bulunan = null;

                    if (bulunan == null)
                    {
                        if (string.IsNullOrWhiteSpace(ty.yilYok[i]))
                            bulunan = uzyServis.UzayDegeriStr(null, ty.uzayAd[i], yil.ToString() + "-" + kodlar[i], true);
                        else
                            bulunan = uzyServis.UzayDegeriStr(null, ty.uzayAd[i], kodlar[i], true);
                        if (bulunan == null) bulunan = "";
                        if (kodListesi != null && kodListesi.Length > i)
                            kodListesi[i][kodlar[i]] = bulunan;
                    }
                    adlar[i] = bulunan;
                }
            }
        }

        public void AdlariBul(TNS.UZY.IUZYServis uzyServis)
        {
            AdlariBul(uzyServis, null);
        }

        /// <summary>
        /// Her kod için o kodun uzay adı ve verilen uzay kullanılarak, tertip içindeki kodların isimleri bulunur ve ad dizisi doldurulur.
        /// </summary>
        /// <param name="uzyServis">Adları bulmak için kullanılacak uzay nesnesi</param>
        public string UzayAdBul(int i)
        {
            TertipYapi ty = Arac.tertipYapiBilgileri.TertipYapiBul(yil, tabloKod);
            if (ty == null) return null;
            return ty.uzayAd[i];
        }

        public int KodUzunlukBul(int kodNo, int kirilimSay, bool naktasiz)
        {
            int uz = 0;
            TertipYapi ty = Arac.tertipYapiBilgileri.TertipYapiBul(yil, tabloKod);

            if (ty != null && ty.kodNolar[kodNo] != null)
            {
                string[] kodParcalar = ty.kirilimlar[kodNo].Split('.');
                if (kirilimSay <= 0) kirilimSay = kodParcalar.Length;
                if (kodParcalar.Length >= kirilimSay)
                {
                    for (int i = 0; i < kirilimSay; i++)
                    {
                        uz += kodParcalar[i].Length;
                    }
                    if (!naktasiz)
                        uz += (kirilimSay - 1);
                }
            }
            return uz;
        }

        public bool KodTamMi(int kodNo, int kirilimSay)
        {
            int uz = KodUzunlukBul(kodNo, kirilimSay, true);
            if (Arac.NoktalariAt(kodlar[kodNo]).Length != uz)
                return false;
            return true;
        }

        public bool KodTamMi(int kodNo)
        {
            TertipYapi ty = Arac.tertipYapiBilgileri.TertipYapiBul(yil, tabloKod);
            if (ty.kodNolar[kodNo] == null) return false;
            if (ty.kodNolar[kodNo].Equals("1") && ty.kurumMuKurumsalMi)
            {
                if (!Arac.KurumMu(kodlar[kodNo])) return false;
            }
            else if (Arac.NoktalariAt(kodlar[kodNo]).Length != Arac.NoktalariAt(ty.kirilimlar[kodNo]).Length)
                return false;
            return true;
        }

        public bool TamMi()
        {
            return TamMi(-1, -1);
        }

        public bool TamMi(int istKod, int istLen)
        {
            return TamMi(istKod, istLen, false, 0);
        }

        /// <summary>
        /// Tertip içindeki kodların, kod için belirlenmiş olan kırılım yapısının tamamına eşit olup olmadığı bulunur. Eşitse doğru döndürülür.
        /// </summary>
        /// <returns>Tüm kodların değeri ile kodun kırılımı eşit uzunlukta ise doğru değilse yanlış döndürülür.</returns>
        public bool TamMi(int istKod, int istLen, bool eko23, int eko3len)
        {
            TertipYapi ty = Arac.tertipYapiBilgileri.TertipYapiBul(yil, tabloKod);
            if (ty == null) return false;

            if (kodlar.Length != ty.sinifSay) return false;

            for (int i = 0; i < ty.sinifSay; i++)
            {
                if (ty.kodNolar[i] == null) return false;
                if (ty.kodNolar[i].Equals("1") && ty.kurumMuKurumsalMi)
                {
                    if (!Arac.KurumMu(kodlar[i])) return false;
                }
                else if (i == istKod)
                {
                    string kodTut = Arac.NoktalariAt(kodlar[i]);
                    if (ty.kodNolar[i].Equals("4") && eko23)
                    {
                        if (TNS.BKD.Arac.Eko23Mu(yil, kodTut))
                            istLen = eko3len;
                    }
                    if (kodTut.Length != istLen)
                        return false;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(kodlar[i])) return false;
                    if (Arac.NoktalariAt(kodlar[i]).Length != Arac.NoktalariAt(ty.kirilimlar[i]).Length)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Tertip içindeki kodların, tümünün ad karşıkları varsa doğru döndürülür. Eğer ismi bulunmamış tertip varsa, o kodu kod adı ve kod değeri döndürülür.
        /// </summary>
        /// <param name="kodAd">Kod adı boş olan ilk kodun kod adının döndürüleceği değişken</param>
        /// <param name="kod">Kod adı boş olan ilk kodun kod değerinin döndürüleceği değişken</param>
        /// <returns>Tüm adlar dolu ise doğru, değilse yanlış döndürülür.</returns>
        public bool IsimlerTamMi(out string kodAd, out string kod)
        {
            kodAd = "";
            kod = "";
            TertipYapi ty = Arac.tertipYapiBilgileri.TertipYapiBul(yil, tabloKod);
            if (ty == null) return false;

            if (kodlar.Length != ty.sinifSay) return false;

            for (int i = 0; i < ty.sinifSay; i++)
            {
                if (String.IsNullOrEmpty(adlar[i]))
                {
                    kodAd = ty.kodAd[i];
                    kod = kodlar[i];
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Verilen kod ve ad bilgileri, kod değeri ve kod adı dizilerinin verilen sırasına atanır.
        /// </summary>
        /// <param name="yer">Kod ve adın tertip içindeki yeri (0'dan başlayarak)</param>
        /// <param name="p_kod">Verilen sıradaki tertip kodunun kod değeri</param>
        /// <param name="p_ad">Verilen sıradaki tertip kodunun adı</param>
        public void TertibeKodEkle(int yer, string p_kod, string p_ad)
        {
            kodlar[yer] = p_kod;
            adlar[yer] = p_ad;
        }

        //public void TertibeAdEkle(int yer, string p_ad)
        //{
        //    adlar[yer] = p_ad;
        //}

        /// <summary>
        /// Verilen dizgi, tire (-) karakterinden ayrılarak kod dizisine atanır.
        /// </summary>
        /// <param name="tertipStr"></param>
        public void TertipAta(string tertipStr)
        {
            if (String.IsNullOrWhiteSpace(tertipStr)) return;
            string[] gelenKod = tertipStr.Split('-');
            if (gelenKod.Length <= kodlar.Length)
                gelenKod.CopyTo(kodlar, 0);
        }

        /// <summary>
        /// Verilen parametrik dizi kod dizisine atanır.
        /// </summary>
        /// <param name="kodDizisi">Tertip kodlarına atanacak kod dizisi.</param>
        public void TertipAta(params string[] kodDizisi)
        {
            kodDizisi.CopyTo(kodlar, 0);
        }

        /// <summary>
        /// Verilen tertip nesnesinin kodları, kod dizisine atanır.
        /// </summary>
        /// <param name="p_tertip">Tertip kodlarına atanacak, kodları tutan tertip nesnesi.</param>
        public void TertipAta(Tertip p_tertip)
        {
            if (p_tertip == null) return;
            if (p_tertip.kodlar.Length == kodlar.Length)
            {
                p_tertip.kodlar.CopyTo(kodlar, 0);
                p_tertip.adlar.CopyTo(adlar, 0);
            }
        }

        public string TertipAdi { get { return tertipAdi; } set { tertipAdi = value; } }
        public string TertipKodu
        {
            get
            {
                return TertipStrOl();
            }
            set
            {
                TertipAta(value);
            }
        }
        /// <summary>
        /// Tertip içi tüm kodlar tire (-) karakteri ile birleştirip döndüren yordam. Tertip yapısında son kısımlarda bulunan ve boş olan kodlar listeye eklenmez.
        /// </summary>
        /// <returns>Kodların tire (-) ile birleştirilmiş hali döndürülür.</returns>
        public string TertipStrOl()
        {
            return TertipStrOl(true);
        }

        /// <summary>
        /// Tertip içi tüm kodlar tire (-) karakteri ile birleştirip döndüren yordam.
        /// </summary>
        /// <param name="boslariAt">Tertip yapısında son kısımlarda bulunan ve boş olan kodlar listeye eklensin mi? eklenmesin mi bilgisi.</param>
        /// <returns>Kodların tire (-) ile birleştirilmiş hali döndürülür.</returns>
        public string TertipStrOl(bool boslariAt)
        {
            int j = kodlar.Length - 1;
            if (boslariAt)
            {
                while (j >= 0)
                {
                    if (string.IsNullOrEmpty(kodlar[j]))
                        j--;
                    else
                        break;
                }
            }
            string tertip = "";
            for (int i = 0; i <= j; i++)
            {
                tertip = tertip + kodlar[i];
                if (i < j) tertip += "-";
            }
            return tertip;
        }

        /// <summary>
        /// Tertip içindeki kodların her bir kırılımı, verilen kıyas tertibin kırılımları ile başlıyorsa doğru döndüren yordam.
        /// </summary>
        /// <param name="kiyasTertip"></param>
        /// <returns>Tertip içindeki kodların her bir kırılımı, verilen kıyas tertibin kırılımları ile başlıyorsa doğru döndürlür değilse yanlış döndürülür.</returns>
        public bool Kiyasla(Tertip kiyasTertip)
        {
            for (int i = 0; i < Math.Max(kodlar.Length, kiyasTertip.kodlar.Length); i++)
            {
                string kaynak, kistas;
                if (i < kodlar.Length)
                    kaynak = kodlar[i];
                else
                    kaynak = "";
                if (kaynak == null) kaynak = "";
                if (i < kiyasTertip.kodlar.Length)
                    kistas = kiyasTertip.kodlar[i];
                else
                    kistas = "";
                if (kistas == null) kistas = "";

                if (!Arac.KodKiyasla(kaynak, kistas))
                    return false;
            }
            return true;
        }

        public bool AlirMi()
        {
            if (durum == ENUMTertipDurum.ALIR || durum == ENUMTertipDurum.ALIR_VERIR)
                return true;
            else
                return false;
        }

        public bool VerirMi()
        {
            if (durum == ENUMTertipDurum.VERIR || durum == ENUMTertipDurum.ALIR_VERIR)
                return true;
            else
                return false;
        }

        public string AdBul()
        {
            if (string.IsNullOrWhiteSpace(tertipAdi)) KoddanAdOl();
            return tertipAdi;
        }

        public void KoddanAdOl()
        {
            if (this.tabloKod == ENUMTabloKod.TABPEBGelir || this.tabloKod == ENUMTabloKod.TABPEBGelirGerceklesmesi || this.tabloKod == ENUMTabloKod.TABGelir || this.tabloKod == ENUMTabloKod.TABGelirGerceklesmesi || this.tabloKod == ENUMTabloKod.TABGelirTahmini)
                this.tertipAdi = this.adlar[1];
            else
                this.tertipAdi = this.adlar[3];
        }

        public void TertipAta(object value)
        {
            throw new NotImplementedException();
        }

        public string TertipAdiKodAddan
        {
            get
            {
                string ad;
                if (this.tabloKod == ENUMTabloKod.TABPEBGelir || this.tabloKod == ENUMTabloKod.TABPEBGelirGerceklesmesi || this.tabloKod == ENUMTabloKod.TABGelir || this.tabloKod == ENUMTabloKod.TABGelirGerceklesmesi || this.tabloKod == ENUMTabloKod.TABGelirTahmini)
                    ad = this.adlar[1];
                else
                {
                    ad = this.adlar[3];
                    if (string.IsNullOrEmpty(ad))
                    {
                        ad = this.adlar[0];
                    }
                    if (string.IsNullOrEmpty(ad))
                    {
                        ad = this.adlar[1];
                    }
                    if (string.IsNullOrEmpty(ad))
                    {
                        ad = this.adlar[2];
                    }
                    if (!string.IsNullOrWhiteSpace(this.adlar[4]))
                    {
                        if (string.IsNullOrEmpty(ad))
                            ad = this.adlar[4];
                        else
                            ad = ad + " (" + this.adlar[4] + ")";
                    }
                }
                return ad;
            }
        }

    }

    [Serializable()]
    public class TertipOdenekBilgi
    {
        public string tertip;
        public double[] odenekler;

        public string TertipKod
        {
            get { return tertip; }
        }

        public double Odenek0
        {
            get { return OrtakFonksiyonlar.DiziElemanAl(odenekler, 0, 0.0); }
        }

        public double Odenek1
        {
            get { return OrtakFonksiyonlar.DiziElemanAl(odenekler, 1, 0.0); }
        }

        public TertipOdenekBilgi(string tertip, params double[] odenekler)
        {
            this.tertip = tertip;
            this.odenekler = new double[odenekler.Length];
            for (int i = 0; i < odenekler.Length; i++)
            {
                this.odenekler[i] = odenekler[i];
            }
        }

        public TertipOdenekBilgi(string tertip, int odenekUzunluk)
        {
            this.tertip = tertip;
            this.odenekler = new double[odenekUzunluk];
            for (int i = 0; i < odenekUzunluk; i++)
            {
                this.odenekler[i] = 0;
            }
        }
    }

    [Serializable()]
    public class TertipKisa
    {
        /// <summary>
        /// Verilen tertibin yılı. Bu sayede tertip yapısı bulunur.
        /// </summary>
        public int yil;

        /// <summary>
        /// Verilen tertibin kodu. Bu sayede tertip yapısı bulunur.
        /// </summary>
        public ENUMTabloKod tabloKod;

        /// <summary>
        /// Tertibin kodlarını tutan dizi.
        /// </summary>
        public string[] kodlar;

        /// <summary>
        /// Verilen kodlara karşılık isimleri tutan dizi
        /// </summary>
        public string[] adlar;

        /// <summary>
        /// Verilen tertibin adını tutan değişken
        /// </summary>
        public string tertipAdi;

        /// <summary>
        /// Tertibin durumu (Giriş, Görünür, Gizli)
        /// </summary>
        public ENUMTertipDurum durum = ENUMTertipDurum.TANIMSIZ;

        public TertipKisa(int kirilimSay)
        {
            kodlar = new string[kirilimSay];
            adlar = new string[kirilimSay];
        }

    }
    //[Serializable()]
    //public class KodYapiBilgi
    //{
    //    public int yil;

    //    public int kodNo;

    //    public string KodYapi;

    //    public string VTAlanAdi;

    //    public string VTAlanKodu;

    //    public string VTTabloAdi;

    //    public KodYapiBilgi(int yil, int kodNo, string KodYapi, string VTAlanAdi, string VTAlanKodu, string VTTabloAdi)
    //    {
    //        this.yil = yil;
    //        this.kodNo = kodNo;
    //        this.KodYapi = KodYapi;
    //        this.VTAlanAdi = VTAlanAdi;
    //        this.VTAlanKodu = VTAlanKodu;
    //        this.VTTabloAdi = VTTabloAdi;
    //    }
    //}

    public enum ENUMMaskeEkAlan
    {
        Tanimsiz = 0,
        String = 1,
        Number = 2,
        TNSDateTime = 3
    }

    [Serializable()]
    public class EkAlanDeger
    {
        public EkAlan ekAlan;
        private object deger;

        public EkAlanDeger(EkAlan ekAlan, object deger)
        {
            this.ekAlan = ekAlan;
            this.deger = deger;
        }

        public object Deger
        {
            get
            {
                return deger;
            }
            set
            {
                if (ekAlan.enumMaske == ENUMMaskeEkAlan.String)
                {
                    if (value.GetType() == typeof(string))
                        deger = (string)value;
                }
                else if (ekAlan.enumMaske == ENUMMaskeEkAlan.Number)
                {
                    if (value.GetType() == typeof(string))
                        deger = OrtakFonksiyonlar.ConvertToInt((string)value, 0);
                    else if (value.GetType() == typeof(int))
                        deger = (int)value;
                }
                else if (ekAlan.enumMaske == ENUMMaskeEkAlan.TNSDateTime)
                {
                    if (value.GetType() == typeof(string))
                        deger = new TNSDateTime((string)value);
                    else if (value.GetType() == typeof(DateTime))
                        deger = new TNSDateTime((DateTime)value);
                }
                else
                    deger = value;
            }
        }

        public override string ToString()
        {
            if (deger == null)
                return string.Empty;

            if (ekAlan.enumMaske == ENUMMaskeEkAlan.String)
                return (string)deger;
            else if (ekAlan.enumMaske == ENUMMaskeEkAlan.Number)
                return ((int)deger).ToString();
            else if (ekAlan.enumMaske == ENUMMaskeEkAlan.TNSDateTime)
                return ((TNSDateTime)deger).ToString();
            else
                return deger.ToString();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EkAlanDeger"/> class.
        /// Web service için parametresi olamyan bir construster gerekli.
        /// Oluşan hata --TNS.BKD.EkAlanDeger cannot be serialized because it does not have a parameterless constructor.
        /// </summary>
        public EkAlanDeger() { }
    }

    [Serializable()]
    public class EkAlan
    {
        public ENUMMaskeEkAlan enumMaske;
        public string ad;
        private string basSeviye;
        private string bitSeviye;
        private string maske;
        private string listeUzayAd;
        private string ortakAlan;

        public string OrtakAlan
        {
            get { return ortakAlan; }
            set { ortakAlan = value; }
        }

        public string ListeUzayAd
        {
            get { return listeUzayAd; }
            set { listeUzayAd = value; }
        }

        public string BasSeviye
        {
            get { return basSeviye; }
        }

        public string BitSeviye
        {
            get { return bitSeviye; }
        }

        public string Maske
        {
            get
            {
                return maske;
            }
            set
            {
                maske = value;

                if (maske.ToUpper() == "S")
                    enumMaske = ENUMMaskeEkAlan.String;
                else if (maske.ToUpper() == "N")
                    enumMaske = ENUMMaskeEkAlan.Number;
                else if (maske.ToUpper() == "TDT")
                    enumMaske = ENUMMaskeEkAlan.TNSDateTime;
                else
                    enumMaske = ENUMMaskeEkAlan.Tanimsiz;
            }
        }

        public bool KodUygunMu(string kod)
        {
            int basSeviyeLength = 0;
            int kodLength = 0;

            if (!string.IsNullOrEmpty(basSeviye))
            {
                basSeviyeLength = basSeviye.Replace(".", "").Length;
                kodLength = kod.Replace(".", "").Length;
            }

            if (kodLength == basSeviyeLength)
                return true;
            else
            {
                if (string.IsNullOrEmpty(bitSeviye))
                    return false;

                if (kodLength < basSeviyeLength ||
                    kodLength > bitSeviye.Replace(".", "").Length)
                    return false;

                string[] bitSeviyeParcali = bitSeviye.Split('.');
                int kiyasLength = basSeviyeLength;
                for (int i = basSeviye.Split('.').Length; i < bitSeviyeParcali.Length; i++)
                {
                    kiyasLength += bitSeviyeParcali[i].Length;
                    if (kodLength == kiyasLength)
                        return true;
                }
                return false;
            }
        }

        public void SeviyeBelirle(string seviye)
        {
            string[] parcaliSeviye = seviye.Split('/');
            basSeviye = parcaliSeviye[0];
            if (parcaliSeviye.Length > 1)
                bitSeviye = parcaliSeviye[1];
        }

        public EkAlan()
        {
        }

        public EkAlan(string p_ad)
        {
            ad = p_ad;
        }
    }

    /// <summary>
    /// Kodların veri tabanından nasıl okunacağı bilgilerini, kodun kırılım yapısını, yıl ve kodnoya göre tutan sınıf.
    /// Kod yapı bilgileri değişken.xml'den okunur vy bu sınıf yapısında tutulur.
    /// </summary>
    [Serializable()]
    public class ButceKodYapi
    {
        /// <summary>
        /// Kod yapısı yıldan yıla değiştiğinden, tutulan kodun hangi yılına ait bilgilerinin tutulduğunu gösteren değişken.
        /// </summary>
        public int yil;

        /// <summary>
        /// Verilen kodun değişken.xml'deki sıra numarası. Bu numara aynı zamanda tertip içinde de tutulur.
        /// </summary>
        public int kodNo;

        /// <summary>
        /// Bu kodun kırılım yapısını tutan değişken
        /// </summary>
        public string kodYapi;

        /// <summary>
        /// Bu kodla ilgili veri tabanına erişim yapılırken, Tablo adının veri tabanındaki ismi.
        /// </summary>
        public string tabloAd;

        /// <summary>
        /// Bu kodla ilgili veri tabanına erişim yapılırken, Ad alanı için tablodaki alanın ismi.
        /// </summary>
        public string vtAlanAd;

        /// <summary>
        /// Bu kodla ilgili veri tabanına erişim yapılırken, Kod alanı için tablodaki alanın ismi.
        /// </summary>
        public string vtAlanKod;

        /// <summary>
        /// Kodun değişken.xml'de tanımlı olan adı.
        /// </summary>
        public string butceKodAd;

        /// <summary>
        /// Kod kırılımının her bir parçasını tutan dizi.
        /// </summary>
        public string[] kodBolumler;

        /// <summary>
        /// Kod kırılımının parça sayısı.
        /// </summary>
        public int bolumSay;

        public string vtBaglantiAd;
        public bool yilYok;
        public string ekSayfa;
        public string ekSayfaSeviye;
        public EkAlan[] ekAlanlar;

        /// <summary>
        /// Verilen bilgilerle kod nesnesi oluşturan oluşturucu yordam.
        /// </summary>
        /// <param name="p_yil">Kod yapısı yıldan yıla değiştiğinden, tutulan kodun hangi yılına ait bilgilerinin tutulduğunu gösteren değişken.</param>
        /// <param name="p_kodNo">Verilen kodun değişken.xml'deki sıra numarası. Bu numara aynı zamanda tertip içinde de tutulur.</param>
        /// <param name="p_butceKodAd">Kodun değişken.xml'de tanımlı olan adı.</param>
        /// <param name="p_kodYapi">Bu kodun kırılım yapısını tutan değişken</param>
        /// <param name="p_tabloAd">Bu kodla ilgili veri tabanına erişim yapılırken, Tablo adının veri tabanındaki ismi.</param>
        /// <param name="p_vtAlanAd">Bu kodla ilgili veri tabanına erişim yapılırken, Ad alanı için tablodaki alanın ismi.</param>
        /// <param name="p_vtAlanKod">Bu kodla ilgili veri tabanına erişim yapılırken, Kod alanı için tablodaki alanın ismi.</param>
        public ButceKodYapi(int p_yil, int p_kodNo, string p_butceKodAd, string p_kodYapi, string p_tabloAd, string p_vtAlanAd, string p_vtAlanKod, string p_vtBaglantiAd, string p_yilYok, string p_ekSayfa, string p_ekSayfaSeviye, string p_ekAlanlar)
        {
            yil = p_yil;
            kodNo = p_kodNo;
            kodYapi = p_kodYapi;
            butceKodAd = p_butceKodAd;
            tabloAd = p_tabloAd;
            vtAlanAd = p_vtAlanAd;
            vtAlanKod = p_vtAlanKod;
            kodBolumler = kodYapi.Split('.');
            bolumSay = kodBolumler.Length;

            vtBaglantiAd = p_vtBaglantiAd;
            yilYok = p_yilYok == "1" ? true : false;
            ekSayfa = p_ekSayfa;
            ekSayfaSeviye = p_ekSayfaSeviye;
            if (!string.IsNullOrEmpty(p_ekAlanlar))
            {
                string[] parcaliEkAlanlar = p_ekAlanlar.Split(',');
                ekAlanlar = new EkAlan[parcaliEkAlanlar.Length];
                for (int i = 0; i < parcaliEkAlanlar.Length; i++)
                    ekAlanlar[i] = new EkAlan(parcaliEkAlanlar[i]);
            }
        }

        /// <summary>
        /// Kodun verilen kırılıma kadar olan kısmının toplam uzunluğunu bulup döndüren yordam.
        /// </summary>
        /// <param name="kirilimSay">Toplam kırılım sayısı döndürülecek kırılım yeri.</param>
        /// <returns>Kodun verilen kırılıma kadar olan kısmının toplam uzunluğunu döndürülür.</returns>
        public int KirilimToplamUzunluk(int kirilimSay)
        {
            int donguBoy = Math.Min(bolumSay, kirilimSay);
            int boyut = 0;
            for (int i = 0; i < donguBoy; i++)
            {
                boyut += kodBolumler[i].Length;
            }
            return boyut;
        }
    }

    //[Serializable()]
    //public class ABSKodTekDil
    //{
    //    public string kod;
    //    public string ad;

    //    public ABSKodTekDil(string p_kod, string p_ad)
    //    {
    //        kod = p_kod;
    //        ad = p_ad;
    //    }
    //}

    /// <summary>
    /// Bütçe kodları, birden fazla dilde ad ve açıklama tutulacak şekilde tasarlanmıştır. 
    /// Bu durumda bir kod içinde birden fazla ad ve açıklama tutulur. 
    /// Kod içinde her bir dil için tutulacak ad ve açıklama bilgisini tutan sınıf.
    /// </summary>
    [Serializable()]
    public class ABSKodDil
    {
        /// <summary>
        /// Bir koda ait ad alanı
        /// </summary>
        public string ad;

        /// <summary>
        /// Bir koda ait açıklama alanı
        /// </summary>
        public string aciklama;

        /// <summary>
        /// ABsKodDil nesnesi oluşturan oluşturucu yordam.
        /// </summary>
        /// <param name="ad">Bir koda ait ad alanı</param>
        /// <param name="aciklama">Bir koda ait açıklama alanı</param>
        public ABSKodDil(string ad, string aciklama)
        {
            this.ad = ad;
            this.aciklama = aciklama;
        }

        public ABSKodDil()
        {
            this.ad = null;
            this.aciklama = null;
        }

    }


    [Serializable()]
    public class DilTable
    {
        public int DilYerBul(string dil)
        {
            for (int i = 0; i < dilKod.Length; i++)
            {
                if (dilKod[i] == dil) return i;

            }
            return -1;
        }

        public ABSKodDil this[string dil]
        {
            get
            {
                int i = DilYerBul(dil);
                if (i >= 0) return absKodDil[i];
                return null;
            }
            set
            {
                int i = DilYerBul(dil);
                if (i == -1)
                {
                    i = DilYerBul(null);
                    if (i == -1)
                    {
                        i = Length;
                        Array.Resize<string>(ref dilKod, i + 1);
                        Array.Resize<ABSKodDil>(ref absKodDil, i + 1);
                    }
                }
                absKodDil[i] = value;
                dilKod[i] = dil;
            }
        }

        public ABSKodDil[] absKodDil = new ABSKodDil[4];
        public string[] dilKod = new string[4];

        public int Length { get { return dilKod.Length; } }
    }

    /// <summary>
    /// Bir kodun değerini ve birden fazla dilde, adını ve açıklamasını tutan sınıf.
    /// </summary>
    [Serializable()]
    public class ABSKod : IComparable
    {
        public string kod { get { return _kod; } set { _kod = value; } }
        /// <summary>
        /// Kodun değerini tutan değişken
        /// </summary>
        private string _kod = "";

        /// <summary>
        /// Kodun birden fazla dilde adını ve açıklamasını tutan Hashtable nesnesi.
        /// </summary>
        public DilTable diller = new DilTable();


        public EkAlanDeger[] ekAlanlar;

        /// <summary>
        /// Kayıt için istemciden sunucuya, veri gönderilirken yapılacak işlemin türünü tutan değişken. 
        /// </summary>
        public ENUMKayitDurumu kayitDurumu = ENUMKayitDurumu.TANIMSIZ;

        public void UygunEkAlanlariBul()
        {
            if (ekAlanlar == null)
                return;

            TNSCollection uygunlar = new TNSCollection();

            foreach (EkAlanDeger ekAlan in ekAlanlar)
                if (ekAlan.ekAlan.KodUygunMu(kod))
                    uygunlar.Add(ekAlan);

            ekAlanlar = (EkAlanDeger[])uygunlar.ToArray(typeof(EkAlanDeger));
        }

        /// <summary>
        /// Verilen ad, kodun aktif dildeki adı olarak atanır.
        /// </summary>
        /// <param name="ad">Kodun aktif dildeki adı.</param>
        public void AdAta(string ad)
        {
            DilEkle(System.Threading.Thread.CurrentThread.CurrentCulture.Name.Substring(0, 2), ad, "");
        }

        /// <summary>
        /// Dilin verilen dil kodundaki ABSKodDil nesnesini bulup döndüren yordam.
        /// </summary>
        /// <param name="dilKod">Kodun hangi dildeki adının döndürüleceği parametresi.</param>
        /// <param name="defaultDondur">Verilen dil bulunamazsa, varsayılan dil karşılığı döndürülsün mü?</param>
        /// <returns>Verilen dilin karşılığındaki KodDil nesnesi bulunursa, bulunamazsa ve defaultDondur doğru ise varsayılan dildeki karşılığı varsa döndürülür. Bulunamazsa null döndürülür.</returns>
        public ABSKodDil DilBul(string dilKod, bool defaultDondur)
        {
            ABSKodDil tut = null;

            if (string.IsNullOrEmpty(dilKod))
            {
                return DilBul(BKD.Arac.DefaultDil, false);
            }

            if (dilKod.Length > 2) dilKod = dilKod.Substring(0, 2);
            tut = (ABSKodDil)diller[dilKod];

            if (tut != null && !string.IsNullOrEmpty(tut.ad))
                return tut;

            if (defaultDondur)
                return DilBul(BKD.Arac.DefaultDil, false);

            return null;
        }

        /// <summary>
        /// Kodun, verilen kulturKodundaki ad karşılığını döndüren yordam. 
        /// </summary>
        /// <param name="kulturKod">Kodun ad karşılığı döndürülecek dil kodu.</param>
        /// <returns>Kodun, verilen kulturKodundaki ad karşılığı döndürülür.</returns>
        public string AdKultur(string kulturKod)
        {
            return Ad(kulturKod.Substring(0, 2), true);
        }

        /// <summary>
        /// Kodun, verilen kulturKodundaki açıklama karşılığını döndüren yordam. 
        /// </summary>
        /// <param name="kulturKod">Kodun açıklama karşılığı döndürülecek dil kodu.</param>
        /// <returns>Kodun, verilen kulturKodundaki açıklama karşılığı döndürülür.</returns>
        public string AciklamaKultur(string kulturKod)
        {
            return Aciklama(kulturKod.Substring(0, 2), true);
        }

        /// <summary>
        /// Koda verilen dilde, açıklama atanır.  
        /// </summary>
        /// <param name="dilKod">Kodun açıklama atanacak dili.</param>
        /// <param name="aciklama">Atanacak açıklama</param>
        public void AciklamaAta(string dilKod, string aciklama)
        {
            ABSKodDil tut = DilBul(dilKod, false);

            if (tut != null)
                tut.aciklama = aciklama;
        }

        public string ad { get { return Ad(); } }
        public string kodAd { get { return kod + "-" + Ad(); } }
        /// <summary>
        /// Kodun aktif kültürdeki adını döndüren yordam.
        /// </summary>
        /// <returns>Kodun aktif kültürdeki adı döndürülür.</returns>
        public string Ad()
        {
            return AdKultur(System.Threading.Thread.CurrentThread.CurrentCulture.Name);
        }

        /// <summary>
        /// Kodun verilen dildeki adını döndüren yordam.
        /// </summary>
        /// <param name="dilKod">Kodun açıklama atanacak dili.</param>
        /// <returns>Kodun verilen dildeki adı döndürülür.</returns>
        public string Ad(string dilKod)
        {
            return Ad(dilKod, false);
        }

        /// <summary>
        /// Kodun verilen dildeki açıklamasını döndüren yordam.
        /// </summary>
        /// <param name="dilKod">Kodun açıklama atanacak dili.</param>
        /// <returns>Kodun verilen dildeki açıklaması döndürülür.</returns>
        public string Aciklama(string dilKod)
        {
            return Aciklama(dilKod, false);
        }

        /// <summary>
        /// Kodun verilen dildeki adını döndüren yordam.
        /// </summary>
        /// <param name="dilKod">Kodun adı döndürülecek dil</param>
        /// <param name="defaultDondur">Verilen dil bulunamazsa, varsayılan dil karşılığı döndürülsün mü?</param>
        /// <returns>Kodun verilen dildeki adı bulunamazsa default dildeki adı döndürülür.</returns>
        public string Ad(string dilKod, bool defaultDondur)
        {
            ABSKodDil tut = DilBul(dilKod, defaultDondur);

            if (tut != null)
                return tut.ad;
            return "";
        }

        /// <summary>
        /// Kodun verilen dildeki açıklamasını döndüren yordam.
        /// </summary>
        /// <param name="dilKod">Kodun açıklaması döndürülecek dil</param>
        /// <param name="defaultDondur">Verilen dil bulunamazsa, varsayılan dil karşılığı döndürülsün mü?</param>
        /// <returns>Kodun verilen dildeki açıklaması bulunamazsa default dildeki açıklaması döndürülür.</returns>
        public string Aciklama(string dilKod, bool defaultDondur)
        {
            ABSKodDil tut = DilBul(dilKod, defaultDondur);

            if (tut != null)
                return tut.aciklama;
            return "";
        }

        /// <summary>
        /// Kod adı boş olan bir kod nesnesi oluşturan oluşturucu yordam.
        /// </summary>
        public ABSKod()
        {
        }

        /// <summary>
        /// Kod adı verilen kod ve kayıt durumu verilen kayıt durumu olan kod nesnesi oluşturan oluşturucu yordam.
        /// </summary>
        /// <param name="p_kod">Kod değeri</param>
        /// <param name="p_kayitDurumu">Kodun kayıt durumu</param>
        public ABSKod(string p_kod, ENUMKayitDurumu p_kayitDurumu)
        {
            kod = p_kod;
            kayitDurumu = p_kayitDurumu;
        }

        /// <summary>
        /// Kod adı verilen kod olan kod nesnesi oluşturan oluşturucu yordam.
        /// </summary>
        /// <param name="p_kod">Kod değeri</param>
        public ABSKod(string p_kod)
        {
            kod = p_kod;
        }

        /// <summary>
        /// Koda verilen ülke kodunda, ad ve açıklama ekleyen yordam.
        /// </summary>
        /// <param name="ulkeKod">Ad ve açıklama eklenecek olan dil.</param>
        /// <param name="ad">Kodun verilen dildeki adı.</param>
        /// <param name="aciklama">Kodun verilen dildeki açıklaması.</param>
        public void DilEkle(string ulkeKod, string ad, string aciklama)
        {
            diller[ulkeKod] = new ABSKodDil(ad, aciklama);
        }

        /// <summary>
        /// Kod nesnesindeki ad, Kriter olarak verilen ABSKodun adı ile kıyaslanır ve sonuç döndürülür.
        /// </summary>
        /// <param name="o_kriter">Kriter olan kod nesnesi</param>
        /// <returns>Küçükse -1, eşitse 0 ve büyükse 1 döndürülür.</returns>
        public int CompareTo(object o_kriter)
        {
            if (o_kriter is ABSKod)
            {
                ABSKod kriter = (ABSKod)o_kriter;
                return Ad().CompareTo(kriter.Ad());
            }

            return -1;
        }

        public override bool Equals(Object o_kriter)
        {
            string kriter;
            if (o_kriter is ABSKod)
            {
                kriter = ((ABSKod)o_kriter).kod;
            }
            else if (o_kriter is string)
            {
                kriter = (string)o_kriter;
            }
            else
                return false;
            return kod.Replace(".", "").Equals(kriter.Replace(".", ""));
        }

        public string KodParcaAl(int uzunluk)
        {
            string[] parcalar = kod.Split('.');
            uzunluk = Math.Min(parcalar.Length, uzunluk);
            string donusDeger = "";
            for (int i = 0; i < uzunluk; i++)
            {
                if (donusDeger != "") donusDeger += ".";
                donusDeger += parcalar[i];
            }
            return donusDeger;
        }
    }

    /// <summary>
    /// ABSKod listesini TNSCollection yapısında tutan, ayrıca sonuç yapısında sunucudan istemciye işlem sonucu bilgisini döndüren bilgileri tutan sınıf. 
    /// </summary>
    [Serializable()]
    public class ABSKodlarVAN
    {
        /// <summary>
        /// ABSKod listesi tutan değişken.
        /// </summary>
        public TNSCollection absKodlar = new TNSCollection();

        /// <summary>
        /// Sunucudan liste talebi yapan istemcilere, işlem sonucu bilgisini döndüren değişken.
        /// </summary>
        public Sonuc sonuc = new Sonuc();
        //public string kodYapi = "";
    }

    /// <summary>
    /// Bu sunucu sınıfı için, istemci ve sunucuda ortak kullanılacak statik yordamları içeren sınıf
    /// </summary>
    public class Arac
    {

        public static Hashtable ProgramButceIliskiHazirla(Kullanici kullanan, int yil)
        {
            Hashtable htPrgIliski = new Hashtable();
            TNS.BKD.IBKDServis servisBKD = TNS.BKD.Arac.Tanimla();
            TNS.BKD.PrgPebIliski pi = new TNS.BKD.PrgPebIliski();
            pi.yil = yil;
            ObjectArray piListe = servisBKD.PrgPebIliskiListele(kullanan, pi);
            foreach (TNS.BKD.PrgPebIliski item in piListe.objeler)
            {
                htPrgIliski[item.pebKod.Replace(".", "")] = item.prgKod + "." + item.altPrgKod + "." + item.faalKod + "." + item.altFaalKod;
            }

            return htPrgIliski;
        }
        public static string SifirlariAt(string sifirli)
        {
            if (string.IsNullOrWhiteSpace(sifirli)) return "";
            string[] p = sifirli.Split('.');
            string dd = "";
            for (int i = 0; i < p.Length; i++)
            {
                if (dd != "") dd += ".";
                dd += OrtakFonksiyonlar.ConvertToInt(p[i], 0);
            }
            return dd;
        }

        public static string RevizeFormTipAl(ENUMRevizeFormTip revizeFormTip)
        {
            if (revizeFormTip == ENUMRevizeFormTip.MasrafPlaniRevize) return "Masraf Planı Revize";
            if (revizeFormTip == ENUMRevizeFormTip.MasrafPlaniAktarma) return "Faaliyetler Arası Aktarma";
            if (revizeFormTip == ENUMRevizeFormTip.MasrafPlaniAHP) return "Faaliyetler Arası Serbest Değişimi";
            if (revizeFormTip == ENUMRevizeFormTip.AHPRevize) return "AHP Revize";
            if (revizeFormTip == ENUMRevizeFormTip.Ekleme) return "Bümko Öd. Ekleme";
            if (revizeFormTip == ENUMRevizeFormTip.KurumAktarma) return "Kurum (%20) Aktarma";
            if (revizeFormTip == ENUMRevizeFormTip.Aktarma) return "Maliye Aktarma";
            if (revizeFormTip == ENUMRevizeFormTip.Hepsi) return "Hepsi";
            if (revizeFormTip == ENUMRevizeFormTip.MasrafPlaniIslemleri) return "Masraf Planı İşlemleri";
            if (revizeFormTip == ENUMRevizeFormTip.BumkoIslemleri) return "BÜMKO İşlemleri";
            if (revizeFormTip == ENUMRevizeFormTip.AktarmaEklemeler) return "Ödenek Değişimleri";
            return "";
        }

        public static string Eko23AlanOl(int yil, ButceKodYapi bky, string tabloAd, string alanAd)
        {
            int uz2 = bky.KirilimToplamUzunluk(2);
            int uz3 = bky.KirilimToplamUzunluk(3);

            string eko2Ad = $"SUBSTRING({tabloAd}{alanAd}, 1, {uz2})";
            string eko3Ad = $"SUBSTRING({tabloAd}{alanAd}, 1, {uz3})";

            return $" case when {eko2Ad} IN ({TNS.BKD.Arac.Eko2_3ListeAl(yil)}) then {eko3Ad} else {eko2Ad} end";
        }

        public static string EkoAlanOl(int yil, string tabloAd, string alanAd, int uzunluk, bool eko23, string subStr)
        {
            ButceKodYapi bky = Tanimla().ButceKodBilgiBul(yil, 4, "");
            if (eko23)
            {
                int uz2 = bky.KirilimToplamUzunluk(2);
                int uz3 = bky.KirilimToplamUzunluk(3);

                string eko2Ad = $"{subStr}({tabloAd}{alanAd}, 1, {uz2})";
                string eko3Ad = $"{subStr}({tabloAd}{alanAd}, 1, {uz3})";

                return $" case when {eko2Ad} IN ({TNS.BKD.Arac.Eko2_3ListeAl(yil)}) then {eko3Ad} else {eko2Ad} end";
            }
            else
            {
                int ekoUz = bky.KirilimToplamUzunluk(uzunluk);
                return $"{subStr}({tabloAd}{alanAd}, 1, {ekoUz})";
            }
        }

        private static bool? _eko23 = null;
        public static bool Eko2_3Bul(Kullanici kullanan)
        {
            if (_eko23 == null)
            {
                _eko23 = !string.IsNullOrWhiteSpace(TNS.DEG.Arac.DegiskenDegerBul(0, "EKO23"));
            }

            return _eko23.Value;
        }

        private static Dictionary<int, string> _EkoKod3SeviyeStr = new Dictionary<int, string>();
        private static Dictionary<int, string[]> _EkoKod3Seviye = new Dictionary<int, string[]>();
        private static string[] EkoKod3Seviye(int yil)
        {
            if (_EkoKod3Seviye.TryGetValue(yil, out string[] deger))
                return deger;

            string eko23liste = TNS.DEG.Arac.DegiskenDegerBul(yil, "EKO23");
            if (!string.IsNullOrWhiteSpace(eko23liste))
            {
                _EkoKod3SeviyeStr[yil] = eko23liste;
                _EkoKod3Seviye[yil] = eko23liste.Split(',');
            }
            else
            {
                _EkoKod3SeviyeStr[yil] = "";
                _EkoKod3Seviye[yil] = new string[] { };
            }
            return _EkoKod3Seviye[yil];
        }

        public static bool Eko23Mu(int yil, string ekoKod)
        {
            var e3s =EkoKod3Seviye(yil);
            if (e3s != null)
            {
                for (int i = 0; i < e3s.Length; i++)
                {
                    if (ekoKod.StartsWith(e3s[i])) return true;
                }
            }
            return false;
        }

        public static string Eko23Dondur(int yil, string ekoKod, int eko2Uz, int eko3Uz)
        {
            int uz;
            if (Eko23Mu(yil, ekoKod.Replace(".", "")))
                uz = eko3Uz;
            else
                uz = eko2Uz;
            if (uz > ekoKod.Length)
                uz = ekoKod.Length;
            return ekoKod.Substring(0, uz);
        }

        private static Dictionary<int, string> _eko2_3ListeStr = new Dictionary<int, string>();

        public static string Eko2_3ListeAl(int yil)
        {
            if (_eko2_3ListeStr.TryGetValue(yil, out string deger)) return deger;

            string ic = "";

            foreach (string item in EkoKod3Seviye(yil))
            {
                if (ic != "") ic += ",";
                ic += "'" + item + "'";
            }
            _eko2_3ListeStr[yil] = ic;
            return ic;
        }

        public static bool GelirKurumMu(int yil)
        {
            string deger = TNS.DEG.Arac.DegiskenDegerBul(yil, "/TERTIPYAPI/TERTIP18/KURUMMU");
            return deger.Equals("1");
        }

        /// <summary>
        /// Tertip bilgisi girilirken kullanılan tertip durumu dizisi
        /// </summary>
        public static string[] TertipDurumAciklamalar()
        {
            string[] tertipDurumAciklamalar = new string[TertipDurumKodlar.Length];
            for (int i = 0; i < tertipDurumAciklamalar.Length; i++)
            {
                tertipDurumAciklamalar[i] = TNS.BKD.ButceKodResI.ResourceManager.GetString("BKDTRTP_" + TertipDurumKodlar[i], System.Threading.Thread.CurrentThread.CurrentUICulture);
            }
            tertipDurumAciklamalar[1] = TNS.BKD.ButceKodResI.BKDTRTP_2;
            return tertipDurumAciklamalar;
        }

        /// <summary>
        /// TertipDurumAciklamalar dizisinin, her bir elemanı için, 
        /// dizideki yerinin bir fazlasını kod olarak hesaplayıp kod dizisi olarak oluşturup döndüren yordam.
        /// </summary>
        public static string[] TertipDurumKodlar;

        /// <summary>
        /// ABS Kod için hangi dillerde veri girişi yapılacağı ve veri tabanında saklanacağı bilgisi.
        /// Dil dizisinin 1. elemanı için (indis:0) veri tabanı alanlarına ek yapılmaz, 
        /// 2. elemanı için (indis:1) '2' karakteri ek yapılır. Veri tabanında da bu alanlar oluşturulmalıdır.
        /// </summary>
        public static string Diller(int siraNo)
        {
            if (_Diller == null) DilleriOku();
            if (_Diller != null && siraNo < _Diller.Length)
                return _Diller[siraNo];
            return DefaultDil;
        }

        private static string[] _Diller = null;
        private static void DilleriOku()
        {
            string tanimliDiller = "";// TNS.DEG.Arac.DegiskenDegerBul(0, "/DILLER");
            if (!string.IsNullOrEmpty(tanimliDiller)) _Diller = tanimliDiller.Split(',');
            if (_Diller == null || _Diller.Length == 0)
            {
                _Diller = new string[2];
                _Diller[0] = "tr";
                _Diller[1] = "en";
            }
        }

        /// <summary>
        /// Kod adı yada açıklaması istendiğinde ve dil kodu verilmezse yada olmayan bir dil kodu verildiğinde, hangi dil varsayılan olacak
        /// </summary>
        public static string DefaultDil
        {
            get
            {
                if (_DefaultDil == null)
                {
                    _DefaultDil = "";// TNS.DEG.Arac.DegiskenDegerBul(0, "/VARSAYILANDIL");
                    if (string.IsNullOrEmpty(_DefaultDil)) _DefaultDil = "tr";

                }
                return _DefaultDil;
            }
        }
        private static string _DefaultDil = null;

        public static int DillerLength
        {
            get
            {
                if (_Diller == null) DilleriOku();
                if (_Diller == null) return 0;
                return _Diller.Length;
            }
        }

        static Arac()
        {
            TertipDurumKodlar = new string[5];
            TertipDurumKodlar[0] = ((int)ENUMTertipDurum.NORMAL).ToString();
            TertipDurumKodlar[1] = ((int)ENUMTertipDurum.ALIR).ToString();
            TertipDurumKodlar[2] = ((int)ENUMTertipDurum.VERIR).ToString();
            TertipDurumKodlar[3] = ((int)ENUMTertipDurum.ALIR_VERIR).ToString();
            TertipDurumKodlar[4] = ((int)ENUMTertipDurum.IPTAL).ToString();
        }

        /// <summary>
        /// Kod isimleri veri tabanında her dil kodu için ayrı alan olarak tutulmaktadır.
        /// Bu alanların isimleri alan adının yanına eklenen karakter dizisi ile elde edilmektedir.
        /// Bu fonksiyon verilen dilin, dizideki yerine göre, alan adına eklenecek eki döndürür.
        /// </summary>
        /// <param name="yer">Dilin Diller dizisindeki yeri</param>
        /// <returns>Dizi indisi verilen dil için, alan adına eklenecek eki döndürür.</returns>
        public static string AlanEkBul(int yer)
        {
            if (yer == 0) return "";
            else return (yer + 1).ToString();
        }

        /// <summary>
        /// Kod isimleri veri tabanında her dil kodu için ayrı alan olarak tutulmaktadır.
        /// Bu alanların isimleri alan adının yanına eklenen karakter dizisi ile elde edilmektedir.
        /// Bu fonksiyon verilen dilin, dil koduna göre, alan adına eklenecek eki döndürür.
        /// Dil kodunun dizideki yeri bulunur, yer indisine göre ek döndüren yordam çağrılır.
        /// </summary>
        /// <param name="dilKod">Dilin Dil kodu.</param>
        /// <returns>Dizi kodu verilen dil için, alan adına eklenecek eki döndürür.</returns>
        public static string AlanEkBul(string dilKod)
        {
            for (int i = 0; i < TNS.BKD.Arac.DillerLength; i++)
            {
                if (TNS.BKD.Arac.Diller(i).Equals(dilKod))
                    return AlanEkBul(i);
            }
            return "";
        }

        public static string AlanEkBul()
        {
            string kulturAd = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            if (string.IsNullOrEmpty(kulturAd) || kulturAd.Length < 2) return "";

            return AlanEkBul(kulturAd.Substring(0, 2));
        }

        /// <summary>
        /// Statik bir dizise tüm tertiplerin yıl bazında tanımları tutulur. Bu sayede bir bilgi iki kez okunmaz.
        /// </summary>
        public static TertipYapiBilgileri tertipYapiBilgileri = new TertipYapiBilgileri();

        /// <summary>
        /// Verilen kod dizgisindeki tüm noktalar atılır.
        /// </summary>
        /// <param name="kod">Noktaları atılacak kod</param>
        /// <returns>Verilen kodun noktaları atılmış hali döndürülür.</returns>
        public static string NoktalariAt(string kod)
        {
            if (kod == null)
                return null;
            else
                return kod.Replace(".", "");
        }

        /// <summary>
        /// Verilen kod, ile kiyas kod kiyaslanır. Eğer kod'un tüm kırılım parçaları, kiyasKod değişkenindeki tüm kırılımlardaki değerlerle başlıyorsa doğru, değilse yanlış döndüren yordam.
        /// </summary>
        /// <param name="kod">KıyasKod alanına ile benzerliği kontrol edilecek parametre.</param>
        /// <param name="kiyasKod">Kod ile kıyaslanacak kıyas kod yapısı</param>
        /// <returns>Verilen kod, ile kiyas kod kiyaslanır. Eğer kod'un tüm kırılım parçaları, kiyasKod değişkenindeki tüm kırılımlardaki değerlerle başlıyorsa doğru, değilse yanlış döndürülür.</returns>
        public static bool KodKiyasla(string kod, string kiyasKod)
        {
            if (kiyasKod == null) return true;
            string[] kodParca = kod.Split('.');
            string[] kiyasKodParca = kiyasKod.Split('.');
            for (int i = 0; i < Math.Max(kodParca.Length, kiyasKodParca.Length); i++)
            {
                string kaynak, kistas;
                if (i < kodParca.Length)
                    kaynak = kodParca[i];
                else
                    kaynak = "";

                if (i < kiyasKodParca.Length)
                    kistas = kiyasKodParca[i];
                else
                    kistas = "";

                if (!KodParcaKiyasla(kaynak, kistas))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Kaynak ile verilen dizgi, kistas ile başlıyorsa doğru, değilse yanlış döndüren yordam.
        /// </summary>
        /// <param name="kaynak">Kıstas dizgisi ile başladığı kontrol edilecek parametre.</param>
        /// <param name="kistas">Kaynak dizgisinin benzeştiği kontrol edilecek kıstas.</param>
        /// <returns>Kaynak ile verilen dizgi, kistas ile başlıyorsa doğru, değilse yanlış döndürülür.</returns>
        private static bool KodParcaKiyasla(string kaynak, string kistas)
        {
            if (string.IsNullOrEmpty(kistas))
                return true;
            if (string.IsNullOrEmpty(kaynak))
                return false;
            if (kaynak.StartsWith(kistas))
                return true;
            return false;
        }

        /// <summary>
        /// 5018 sayılı kanunda tanımlı olmayan kurumlar için kullanılacak kurum kodunun başlangıç karakterlerini döndüren yordam.
        /// </summary>
        /// <returns>5018 sayılı kanunda tanımlı olmayan kurumlar için kullanılacak kurum kodunun başlangıç karakterleri döndürülür.</returns>
        public static string DigerKurumlarBasla()
        {
            return "50";
        }

        /// <summary>
        /// 5018 sayılı kanunda belediyeler için kullanılacak kurum kodunun ilk iki karekterinin ilk değerini döndüren yordam.
        /// </summary>
        /// <returns>5018 sayılı kanunda belediyeler için kullanılacak kurum kodunun ilk iki karekterinin ilk değeri döndürülür.</returns>
        public static string BelediyeBasla()
        {
            return "46";
        }

        /// <summary>
        /// 5018 sayılı kanunda belediyeler için kullanılacak kurum kodunun ilk iki karekterinin son değerini döndüren yordam.
        /// </summary>
        /// <returns>5018 sayılı kanunda belediyeler için kullanılacak kurum kodunun ilk iki karekterinin son değeri döndürülür.</returns>
        public static string BelediyeBit()
        {
            return "48";
        }

        /// <summary>
        /// Verilen kod bir kuruma ait ise doğru, bir birime ait ise yanlış döndüren yordam.
        /// </summary>
        /// <param name="kod">Kurum mu yoksa birim mi olduğu kontrol edilecek parametre.</param>
        /// <returns>Verilen kod bir kuruma ait ise doğru, bir birime ait ise yanlış döndürülür.</returns>
        public static bool KurumMu(string kod)
        {
            if (kod.IndexOf('.') == 3)
                return true;
            kod = kod.Replace(".", "");
            if (kod.Length == 6)
                return BelediyeMi(kod);
            if (kod.Length == 2)
                if (kod.CompareTo(TNS.BKD.Arac.GenelButceSonKod()) < 0)
                    return true;
                else
                    return false;
            else if (kod.Length == 4)
                if (kod.Substring(0, 2).CompareTo(TNS.BKD.Arac.GenelButceSonKod()) < 0)
                    if (kod.Substring(2, 2).CompareTo(TNS.BKD.Arac.BagimsizKurumBaslangic()) < 0)
                        return false;
                    else
                        return true;
                else
                    return true;
            else
                return false;

        }

        /// <summary>
        /// Verilen kod bir belediye kodu ise doğru, değilse yanlış döndüren yordam.
        /// </summary>
        /// <param name="kurumsalKod">Belediye kodu mu olduğu kontrol edilecek parametre.</param>
        /// <returns>Verilen kod bir belediye kodu ise doğru, değilse yanlış döndürülür.</returns>
        public static bool BelediyeMi(string kurumsalKod)
        {
            if (kurumsalKod == null) return false;
            string icKurumsalKod = kurumsalKod.Replace(".", "");
            if (icKurumsalKod.Length < 6)
                return false;
            icKurumsalKod = icKurumsalKod.Substring(0, 2);
            if (icKurumsalKod.CompareTo(BelediyeBasla()) >= 0 && icKurumsalKod.CompareTo(BelediyeBit()) < 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Bir kurumla ilgili fakat bağımsız bütçesi olan kurumlar için, kurumsal kodda ikinci kırılımın başlangıç değerini döndüren yordam.
        /// </summary>
        /// <returns>Bir kurumla ilgili fakat bağımsız bütçesi olan kurumlar için, kurumsal kodda ikinci kırılımın başlangıç değeri döndürülür.</returns>
        public static string BagimsizKurumBaslangic()
        {
            return "75";
        }

        /// <summary>
        /// Genel bütçeli kurumlar için, kurum kodları listesinde ayrılmış olan kodların birinci kırılımının son değerini döndüren yordam.
        /// </summary>
        /// <returns>Genel bütçeli kurumlar için, kurum kodları listesinde ayrılmış olan kodların birinci kırılımının son değerini döndürülür.</returns>
        public static string GenelButceSonKod()
        {
            return "37";
        }

        /// <summary>
        /// Bu sunucu sınıfının nesnesini .net remoting ile oluşturup döndüren statik fonksiyon
        /// </summary>
        /// <returns>.net remoting ile oluşturulan nesne </returns>
        public static IBKDServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("BKDServisURL");
            IBKDServis servis = TNS.Api.ServisIstemci.Olustur<IBKDServis>(sUrl);
            return servis;
        }

        /// <summary>
        /// Numarası verilen ayın adını döndüren yordam.
        /// </summary>
        /// <param name="ay">Adı döndürülecek ayın numarası.</param>
        /// <returns>Numarası verilen ayın adı döndürülür.</returns>
        public static string AyAdBul(int ay)
        {
            if (ay == 1) return ButceKodResI.BKD001;
            if (ay == 2) return ButceKodResI.BKD002;
            if (ay == 3) return ButceKodResI.BKD003;
            if (ay == 4) return ButceKodResI.BKD004;
            if (ay == 5) return ButceKodResI.BKD005;
            if (ay == 6) return ButceKodResI.BKD006;
            if (ay == 7) return ButceKodResI.BKD007;
            if (ay == 8) return ButceKodResI.BKD008;
            if (ay == 9) return ButceKodResI.BKD009;
            if (ay == 10) return ButceKodResI.BKD010;
            if (ay == 11) return ButceKodResI.BKD011;
            if (ay == 12) return ButceKodResI.BKD012;
            return "";
        }

        public static string KurumsaldanKurumaM(string kurumsalKod)
        {
            return KurumsaldanKuruma(kurumsalKod, null);
        }

        /// <summary>
        /// Verilen kurumsal kodun ait olduğu kurum kodunu bulup döndüren yordam.
        /// </summary>
        /// <param name="kurumsalKod">Kurum kodu bulunacak kurumsal kod.</param>
        /// <returns>Verilen kurumsal kodun ait olduğu kurum kodu döndürülür.</returns>
        public static string KurumsaldanKuruma(string kurumsalKod, string kurYapi)
        {
            return OrtakFonksiyonlar.KurumsaldanKuruma(kurumsalKod, kurYapi);
        }

        /// <summary>
        /// Kod adayı olarak verilen dizginin kod olup olmadığını bulur.
        /// Eğer verilen dizgi nokta ve rakamlardan oluşuyorsa doğru değilse yanlış döndürülür.
        /// </summary>
        /// <param name="kodAdayi">Kod içeriği taşıyıp taşımadığı bulunacak dizgi.</param>
        /// <returns>Verilen dizgi kod uyumlu ise doğru, değilse yanlış döndürülür.</returns>
        public static bool KodMu(string kodAdayi)
        {
            for (int i = 0; i < kodAdayi.Length; i++)
            {
                char c = kodAdayi[i];
                bool aday = (c == '.') || (Char.IsDigit(c));
                if (!aday) return false;
            }
            return true;
        }

        public static double DiziElemenAl(double[] dizi, int indis)
        {
            return dizi[indis];
        }

        public static void DiziElemenAta(double[] dizi, int indis, double deger)
        {
            dizi[indis] = deger;
        }

        public static Tertip tertipUz = null;
        public static int eko2Uz = 3;
        public static int TertipKodKiyaslaEko2Peb(string[] this_kodlar, string[] gelen_kodlar)
        {
            if (tertipUz == null)
            {
                tertipUz = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABPEBOdenek);
                eko2Uz = tertipUz.KodUzunlukBul(3, 2, true);
            }
            int gelenKodSay = gelen_kodlar.Length;
            int thisKodSay = this_kodlar.Length;
            int say = Math.Min(gelenKodSay, thisKodSay);
            for (int i = 0; i < say; i++)
            {
                int sonuc = ParcaKiyasla(this_kodlar[i], gelen_kodlar[i], i, true, eko2Uz);
                if (sonuc != 0) return sonuc;
                if (i == 4)
                {
                    sonuc = ParcaKiyasla(this_kodlar[3], gelen_kodlar[3], 3, false, eko2Uz);
                    if (sonuc != 0) return sonuc;
                }
            }
            return thisKodSay.CompareTo(gelenKodSay);
        }

        public static int PEBOdenekKodUzunlukBul(int yil, int KodNo, int KirilimYer, bool noktasiz)
        {
            if (tertipUz == null || tertipUz.yil != yil)
            {
                tertipUz = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABPEBOdenek);
            }
            return tertipUz.KodUzunlukBul(KodNo, KirilimYer, true);
        }

        private static int ParcaKiyasla(string kodBu, string kodGelen, int i, bool kendisi, int kesUz)
        {
            if (kodBu == null) kodBu = "";
            if (kodGelen == null) kodGelen = "";
            if (i == 3 && kendisi)
            {
                kodBu = KesEko2(kodBu, kesUz);
                kodGelen = KesEko2(kodGelen, kesUz);
            }
            return kodBu.CompareTo(kodGelen);
        }

        private static string KesEko2(string kod, int kesUz)
        {
            kod = kod.Replace(".", "");
            int uz = kod.Length;
            if (uz > kesUz) return kod.Substring(0, kesUz);
            return kod;
        }

    }

    /// <summary>
    /// Para birimleri tanım bilgisini tutan sınıf.
    /// </summary>
    [Serializable()]
    public class ParaBirimi
    {
        /// <summary>
        /// Para biriminin kodu
        /// </summary>
        public int kod { get { return _kod; } set { _kod = value; } }
        public int _kod = 0;

        /// <summary>
        /// Para biriminin adı
        /// </summary>
        public string adi = string.Empty;

        /// <summary>
        /// Para biriminin kısaltması
        /// </summary>
        public string kisaltma { get { return _kisaltma; } set { _kisaltma = value; } }
        private string _kisaltma = string.Empty;

        public string ParaBirimiKod
        {
            get
            {
                if (kisaltma == "TRY" || kisaltma == "TL") return "TL";
                return kisaltma;
            }
        }

        public string KurusBirimiKod
        {
            get
            {
                if (kisaltma == "TRY" || kisaltma == "TL") return "Kr";
                return "c.";
            }
        }

        public string Lira
        {
            get
            {
                if (kisaltma == "EUR") return "Avro";
                if (kisaltma == "USD") return "Dolar";
                if (kisaltma == "TRY" || kisaltma == "TL") return "Lira";
                if (kisaltma == "KGS") return "Som";
                return kisaltma;
            }
        }

        public string Kurus
        {
            get
            {
                if (kisaltma == "TRY" || kisaltma == "TL") return "Kuruş";
                if (kisaltma == "KGS") return "tyyn";
                return "Sent";
            }
        }

        public ParaIsmi ParaIsmiBul
        {
            get
            {
                return new ParaIsmi(Lira, true, Kurus, true);
            }
        }
    }

    /// <summary>
    /// Bütçe kodları sunucu servisi tarafından sağlanan hizmetlerinin arayüz sınıfı.
    /// </summary>
    public interface IBKDServis
    {
        /// <summary>
        /// Verilen arama kıstası kullanılarak, kod ve addan arama yaparak, uyan kodları listeleyen yordam.
        /// </summary>
        /// <param name="dilKod">Adların aranacağı ve listeleneceği dil.</param>
        /// <param name="yil">Kodların listeleneceği yıl.</param>
        /// <param name="kodTuru">Listelenecek kodun tür kodu (Değişken.xml'den kodların adları ve numaraları bulunabilir.)</param>
        /// <param name="aramaKistas">Listelenecek kodların uyacağı arama dizgisi. Arama dizgisi boşluklardan bölünür. İlk parça hem kod için hem de ad için kullanılır. Diğer parçalar sadece ad için arama da kullanılır.</param>
        /// <returns>Verilen kriterlere uyan kodların listesi döndürülür.</returns>
        ABSKod[] KodAra(string dilKod, int yil, string kodTuru, string aramaKistas);

        ABSKod[] KodAra(string dilKod, int yil, string kodTuru, string aramaKistas, bool adaGoreSirala);

        /// <summary>
        /// Kendisine parametre olarak gecirilen kodTuru'ne ve yil'a ait, ve verilen kod'un bir alt seviyesindeki kodları 
        /// listeleyen yordam. ustKod parametresi boş ise ilk seviye kodlar listelenir.
        /// </summary>
        /// <param name="yil">Kod değerlerinin listeleneceği yıl.</param>
        /// <param name="kodTuru">Listelenecek kodun tür kodu (Değişken.xml'den kodların adları ve numaraları bulunabilir.)</param>
        /// <param name="ustKod">Listelenecek kodun başlayacağı seviye ve kıstas parametresi</param>
        /// <returns>Verilen kriterlere uyan kodların listesi ve sonuc bilgisi döndürülür.</returns>
        ABSKodlarVAN KodListele(int yil, string kodTuru, string ustKod);

        /// <summary>
        /// Kendisine parametre olarak gecirilen kodTuru'ne ve yil'a ait, ve verilen kod'un bir alt seviyesinden verilen seviyeye kadar olan kodları 
        /// listeleyen yordam. ustKod parametresi boş ise listeleme ilk seviyeden başlar.
        /// </summary>
        /// <param name="yil">Kod değerlerinin listeleneceği yıl.</param>
        /// <param name="kodTuru">Listelenecek kodun tür kodu (Değişken.xml'den kodların adları ve numaraları bulunabilir.)</param>
        /// <param name="ustKod">Listelenecek kodun başlayacağı seviye ve kıstas parametresi</param>
        /// <param name="seviye">Listelenecek kodun biteceği seviyeyi gösteren parametre.</param>
        /// <returns>Verilen kriterlere uyan kodların listesi ve sonuc bilgisi döndürülür.</returns>
        ABSKodlarVAN KodListeleSeviye(int yil, string kodTuru, string ustKod, int seviye);

        /// <summary>
        /// Kendisine parametre olarak gecirilen kodTuru'ne ve yil'a ait, ve verilen kod'un bir alt seviyesinden verilen seviyeye kadar olan kodları 
        /// listeleyen yordam. ustKod parametresi boş ise listeleme ilk seviyeden başlar.
        /// </summary>
        /// <param name="yil">Kod değerlerinin listeleneceği yıl.</param>
        /// <param name="kodTuru">Listelenecek kodun tür kodu (Değişken.xml'den kodların adları ve numaraları bulunabilir.)</param>
        /// <param name="ustKod">Listelenecek kodun başlayacağı seviye ve kıstas parametresi</param>
        /// <param name="seviye">Listelenecek kodun biteceği seviyeyi gösteren parametre.</param>
        /// <param name="sadeceDetay">Bu paramtre doğru ise verilen koda ve üst koda uyan, en alt seviye kodlar listelenir ve seviye parametresi kullanımaz. Bu parametre yanlış ise seviye parametresi kullanılır.</param>
        /// <returns>Verilen kriterlere uyan kodların listesi ve sonuc bilgisi döndürülür.</returns>
        ABSKodlarVAN KodListeleSeviye(int yil, string kodTuru, string ustKod, int seviye, bool sadeceDetay);

        ABSKodlarVAN KodListeleSeviye(int yil, string kodTuru, string ustKod, int seviye, bool sadeceDetay, bool adaGoreSirala);

        ABSKodlarVAN KodListeleSeviye(int yil, string kodTuru, string ustKod, int seviye, bool sadeceDetay, bool adaGoreSirala, string filtreAd, string filtreKod, string dilKod);

        ABSKodlarVAN KodListeleSeviye(int yil, string kodTuru, string ustKod, int seviye, bool sadeceDetay, bool adaGoreSirala, string filtreAd, string filtreKod, string dilKod, bool hiyerarsik);
        ABSKodlarVAN KodListeleSeviyeIkiYil(int yil, string kodTuru, string ustKod, int seviye, bool sadeceDetay, bool adaGoreSirala, string filtreAd, string filtreKod, string dilKod, bool hiyerarsik, bool ikiYil, string baslangicFiltreKodAd, bool aciklamaEkle);

        /// <summary>
        /// Verilen bir kuruma ait birimleri listeleyen yordam. Bu yordam verilen kullanıcının görebileceği birimleri listeler.
        /// </summary>
        /// <param name="kullanan">Görebileceği birimleri listelenecek kullanıcı bilgilerini tutan sınıf.</param>
        /// <param name="yil">Birimlerin listeleneceği yıl.</param>
        /// <param name="kurum">Birimlerin listeleneceği kurum.</param>
        /// <param name="altDuzey">En alt düzeydeki birimler listelenir.</param>
        /// <returns>Verilen kriterlere uyan kodların listesi ve sonuc bilgisi döndürülür.</returns>
        ABSKodlarVAN BirimListele(Kullanici kullanan, int yil, string kurum, bool altDuzey);

        ABSKodlarVAN BirimListele(Kullanici kullanan, int yil, string kurum, bool altDuzey, bool adaGoreSirala);

        /// <summary>
        /// Verilen listedeki (TNSCollection) kodları veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Kodların kaydedileceği yıl.</param>
        /// <param name="kodTuru">Kodların ait olduğu kod türü. Bu bilgi kullanılarak veri tabanı bilgileri bulunur.</param>
        /// <param name="kodlar">Kaydedilecek kod değerlerini tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc KodKaydet(Kullanici kullanan, int yil, string kodTuru, TNSCollection kodlar);

        /// <summary>
        /// Verilen kod numarasının veya kod adının, verilen yıldaki yapısı ve veri tabanı erişim bilgilerini döndüren yordam.
        /// </summary>
        /// <param name="yil">Verilen kodun bu yıla ait bilgileri döndürülür.</param>
        /// <param name="kodNo">Döndürülecek kodun numarası. (Degisken.xml'deki kod no yada kd adı verilmelidir.)</param>
        /// <param name="kodAd">Döndürülecek kodun adı. (Degisken.xml'deki kod no yada kd adı verilmelidir.)</param>
        /// <returns>Verilen kod numarasının veya kod adının, verilen yıldaki yapısı ve veri tabanı erişim bilgileri ButceKodYapi nesnesi olarak döndürülür.</returns>
        ButceKodYapi ButceKodBilgiBul(int yil, int kodNo, string kodAd);

        /// <summary>
        /// Verilen para birimi tanım bilgisini veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="paraBirimi">Kaydedilecek para birimi tanım bilgilerini tutan nesne.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ParaBirimiKaydet(Kullanici kullanan, ParaBirimi paraBirimi);

        /// <summary>
        /// Verilen kritere uyan, para birimlerinin listesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="kriterParaBirimi">Döndürülecek para birimlerinin uyacağı kriter bilgilerini tutan yapı.</param>
        /// <returns>Verilen kritere uyan, para birimlerinin dizisi döndürülür.</returns>
        ParaBirimi[] ParaBirimiListele(Kullanici kullanan, ParaBirimi kriterParaBirimi);

        /// <summary>
        /// Verilen kritere uyan, para birimleri veri tabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="kriterParaBirimi">Silinecek para birimlerinin uyacağı kriter bilgilerini tutan yapı.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ParaBirimiSil(Kullanici kullanan, ParaBirimi kriterParaBirimi);

        /// <summary>
        /// Verilen kod türü bir önceki yıldaki değerleri bir sonraki yıla kopyalar.
        /// İşlem sonucu Sonuc yapısında döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Kodların kaydedileceği yıl.</param>
        /// <param name="kodTuru">Kodların ait olduğu kod türü. Bu bilgi kullanılarak veri tabanı bilgileri bulunur.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc OncekiYildanKopyala(Kullanici kullanan, int yil, string kodTuru);

        /// <summary>
        /// Verilen kod türü için verilen yıldaki kayıt sayısını bulup döndüren yordam.
        /// </summary>
        /// <param name="yil">Kod sayısının bulunacağı yıl.</param>
        /// <param name="kodTuru">Kodların ait olduğu kod türü. Bu bilgi kullanılarak veri tabanı bilgileri bulunur.</param>
        /// <returns>İşlem sonucunda bulunan sayı döndürülür.</returns>
        int ButceKodSatirSayisiBul(int yil, string kodTuru);
        System.Collections.ArrayList TertipPerformans();
        string KodAdBul(int yil, string kodTuru, string kod);
        string KodAdBul(Kullanici kullanan, int yil, string kodTuru, string kod);
        ABSKodlarVAN KurumListesiKullanici(Kullanici kullanan, int yil);
        Sonuc PrgKodYapiKaydet(Kullanici kullanan, PrgKodYapi bilgi);
        Sonuc PrgKodYapiAktar(Kullanici kullanan, PrgKodYapi bilgi);
        ObjectArray PrgKodYapiListele(Kullanici kullanan, PrgKodYapi bilgi);
        Sonuc PrgKodYapiSil(Kullanici kullanan, PrgKodYapi bilgi);
        Sonuc PrgPebIliskiKaydet(Kullanici kullanan, PrgPebIliski bilgi);
        ObjectArray PrgPebIliskiListele(Kullanici kullanan, PrgPebIliski bilgi);
        Sonuc PrgPebIliskiSil(Kullanici kullanan, PrgPebIliski bilgi);
        Sonuc PrgPebIliskiKopyala(Kullanici kullanan, int yil);

    }
}

/*
//        DataTable ParaBirimiListele(Kullanici kullanan);

    //public enum ENUMButceTur : int
    //{
    //    GENEL = 1,
    //    UNIVERSITEOZEL = 2,
    //    UNIVERSITE = 3,
    //    OZEL = 4,
    //    DDK = 5,
    //    SOSYALGUVENLIK = 6,
    //    DIGER = 7,
    //    TANIMSIZ = -1
    //}


        //	    ABSKodlarVAN ABSKodListesi(int yil, string kodTuru, string aranacakKod);
        //	    Sonuc ABSKodKaydet(Kullanici kullanan, ABSKodlar absKodlar, string kodTuru);		
        //        ABSKodVAN ABSKodOku(ABSKod absKod, string kodTuru);


        //Parametrik Tuketim Talep

        //ABSKodlarVAN OdenekAsamalariBul();

        //ABSKodlarVAN KurumListesiKullanici(Kullanici kullanici, int yil);

        / *
                    string HazineYardimiTertibindenKurumBul(string fonksiyonelKod);
                    BUMKOCollection HazineYardimiTertipleriBul(string kurumKod);
         * * /

        //Sonuc SonKirilimMi(int yil, string kodTuru, string kod);
        //Sonuc KurumMu(int yil, string kod);
        //string KurumsaldanKuruma(string kurumsalKod);

        //ABSKodlarVAN BirimListele(Kullanici kullanan, int yil);
        //ABSKodlarVAN KurumListesiKullanici(Kullanici kullanici, int yil, bool gelirlilerMi);
        //ABSKodlarVAN KurumListesi(int yil, ENUMButceTur butceTur);

    //public class Asamalar
    //{
    //    public static ABSKodlarVAN OdenekAsamalariBul(bool tahminMi)
    //    {
    //        ABSKodlarVAN kv = new ABSKodlarVAN();
    //        if (tahminMi)
    //        {
    //            kv.absKodlar.Add(new ABSKodTekDil("1", "Kurum Y.S. Harc.Tah."));
    //            kv.absKodlar.Add(new ABSKodTekDil("2", "Uzman Y.S. Harc.Tah."));
    //        }
    //        else
    //        {
    //            kv.absKodlar.Add(new ABSKodTekDil("0", "Birim Teklifi"));
    //            kv.absKodlar.Add(new ABSKodTekDil("1", "SGB Görüşü"));
    //            kv.absKodlar.Add(new ABSKodTekDil("2", "Bakanlık Teklifi"));
    //            kv.absKodlar.Add(new ABSKodTekDil("3", "Tasarı"));
    //            kv.absKodlar.Add(new ABSKodTekDil("4", "Plan Bütçe Komisyonu"));
    //            kv.absKodlar.Add(new ABSKodTekDil("5", "Kanun"));
    //            kv.absKodlar.Add(new ABSKodTekDil("6", "KBÖ"));
    //        }
    //        return kv;
    //    }

    //    public static ABSKodlarVAN GelirAsamalariBul(bool tahminMi)
    //    {
    //        ABSKodlarVAN kv = new ABSKodlarVAN();
    //        if (tahminMi)
    //        {
    //            kv.absKodlar.Add(new ABSKodTekDil("1", "Kurum Y.S. Gel.Tah."));
    //            kv.absKodlar.Add(new ABSKodTekDil("2", "Uzman Y.S. Gel.Tah."));
    //        }
    //        else
    //        {
    //            kv.absKodlar.Add(new ABSKodTekDil("0", "Birim Teklifi (Gelir)"));
    //            kv.absKodlar.Add(new ABSKodTekDil("1", "SGB Görüşü (Gelir)"));
    //            kv.absKodlar.Add(new ABSKodTekDil("2", "Bakanlık Teklifi (Gelir)"));
    //            kv.absKodlar.Add(new ABSKodTekDil("3", "Tasarı (Gelir)"));
    //            kv.absKodlar.Add(new ABSKodTekDil("4", "Plan Bütçe Komisyonu (Gelir)"));
    //            kv.absKodlar.Add(new ABSKodTekDil("5", "Kanun (Gelir)"));
    //            kv.absKodlar.Add(new ABSKodTekDil("6", "Kesintili Gelir "));
    //        }
    //        return kv;
    //    }

    //    public static ABSKodlarVAN FinansmanAsamalariBul(bool tahminMi)
    //    {
    //        ABSKodlarVAN kv = new ABSKodlarVAN();
    //        if (tahminMi)
    //        {
    //            kv.absKodlar.Add(new ABSKodTekDil("1", "Kurum Y.S. Fin.Tah."));
    //            kv.absKodlar.Add(new ABSKodTekDil("2", "Uzman Y.S. Fin.Tah."));
    //        }
    //        else
    //        {
    //            kv.absKodlar.Add(new ABSKodTekDil("0", "Başlangıç (Fin.)"));
    //            kv.absKodlar.Add(new ABSKodTekDil("1", "Kurum Teklifi (Fin.)"));
    //            kv.absKodlar.Add(new ABSKodTekDil("2", "Uzman Görüşü (Fin.)"));
    //            kv.absKodlar.Add(new ABSKodTekDil("3", "Tasarı (Fin.)"));
    //            kv.absKodlar.Add(new ABSKodTekDil("4", "Plan Bütçe Komisyonu (Fin.)"));
    //            kv.absKodlar.Add(new ABSKodTekDil("5", "Kanun (Fin.)"));
    //            kv.absKodlar.Add(new ABSKodTekDil("6", "Kesintili Finansman"));
    //        }
    //        return kv;
    //    }


    //    public static string AsamaAdBul(int tabloNo, int asama)
    //    {
    //        ABSKodlarVAN kv;
    //        if (tabloNo == (int)ENUMTabloKod.TABOdenek || tabloNo == (int)ENUMTabloKod.TABHarcamaTahmin)
    //            kv = OdenekAsamalariBul(tabloNo == (int)ENUMTabloKod.TABHarcamaTahmin);
    //        else if (tabloNo == (int)ENUMTabloKod.TABGelir || tabloNo == (int)ENUMTabloKod.TABGelirTahmini)
    //            kv = GelirAsamalariBul(tabloNo == (int)ENUMTabloKod.TABGelirTahmini);
    //        else if (tabloNo == (int)ENUMTabloKod.TABFinansman || tabloNo == (int)ENUMTabloKod.TABFinansmanTahmini)
    //            kv = FinansmanAsamalariBul(tabloNo == (int)ENUMTabloKod.TABFinansmanTahmini);
    //        else
    //            return "";
    //        for (int i = 0; i < kv.absKodlar.Count; i++)
    //        {
    //            ABSKodTekDil kod = (ABSKodTekDil)kv.absKodlar[i];
    //            if (Convert.ToInt32(kod.kod) == (asama % 10))
    //            {
    //                string ekleStr;
    //                if (asama >= 20) ekleStr = " (+2)";
    //                else if (asama >= 10) ekleStr = " (+1)";
    //                else ekleStr = "";
    //                return kod.ad + ekleStr;
    //            }
    //        }
    //        return "";
    //    }

    //}

    //[Serializable()]
    //public class TertipVAN
    //{

    //    public Sonuc sonuc;
    //    public Tertip tertip;
    //    public TertipVAN(int yil, ENUMTabloKod tabloKod)
    //    {
    //        sonuc = new Sonuc();
    //        tertip = new Tertip(yil, tabloKod);
    //    }
    //}// END CLASS DEFINITION TertipVAN


        //public bool BilgiVarMi(int yil, ENUMTabloKod tabloKod)
        //{
        //    return TertipYapiBul(yil, tabloKod) != null;
        //}

        //public static string HazineYardimiTertibindenKurumBul(string fon)
        //{
        //    return "";
        //}

        //public static TNSCollection HazineYardimiTertipleriBul(string kurumKod)
        //{
        //    return null;
        //}
*/