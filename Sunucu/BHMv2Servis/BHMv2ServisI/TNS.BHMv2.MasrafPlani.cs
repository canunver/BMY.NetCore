using TNS.KYM;
using System.Collections;
using System;
using System.Data;
using TNS.BKD;
using System.Text;
using TNS.UZY;
using TNS.BHMv2;

namespace TNS.BHMv2
{
    public enum ENUMMasrafPlanAsama
    {
        URETIM = 1,
        TALEP = 2,
        SGB = 3,
        MERKEZ = 4,
        AHP = 5,
        SONDURUMONAYLI = 9,
        SONDURUMHEPSI = 10,
    }

    [System.Serializable()]
    public class MPRevizeTalep
    {
        public int yil;
        public KurumsalYapi kurumsalYapi;
        public int revizeTalepNo;
        public TNSDateTime evrakTarih;
        public string evrakNo;
        public string evrakIlgi;
        public ENUMRevizeFormDurum formDurum = ENUMRevizeFormDurum.TANIMSIZ;
        public string talepAciklama;
        public string cevapAciklama;

        public string kaydedenKullaniciKodu;
        public TNSDateTime kayitTarihi;
        public string onaylayanKullaniciKodu;
        public TNSDateTime onaylamaTarihi;

        public double talepToplam;
        public double karsilananToplam;

        public MPRevizeSatir[] satirlar;

        public string DurumAciklama()
        {
            return "";
        }

        public bool DegistirilebilirMi()
        {
            if (formDurum == ENUMRevizeFormDurum.Musvette || formDurum == ENUMRevizeFormDurum.Taslak || formDurum == ENUMRevizeFormDurum.Iletildi)
                return true;
            else
                return false;
        }

        public bool AlanlarGecerliMi(Sonuc s)
        {
            int enAzUzunluk = 10;
            if (evrakTarih == null || evrakTarih.isNull)
                s.DegerAta("Tarih alanı boş bırakılamaz!");
            if (talepAciklama == null || talepAciklama.Length < enAzUzunluk)
                s.DegerAta(string.Format("Açıklama alanına en az {0} karakter girmelisiniz!", enAzUzunluk));
            return s.islemSonuc;
        }

        public bool detayGetir = false;
    }

    [System.Serializable()]
    public class MPEkonomik
    {
        public int kod;
        public string ad;
        public string ekoKod;
        public MPEkonomik(int kod, string ad, string ekoKod)
        {
            this.kod = kod;
            this.ad = ad;
            this.ekoKod = ekoKod;
        }
    }

    [System.Serializable()]
    public class MPBilgi
    {
        public ABSKod mpEkonomik;
        public double odenek;
        public double gonderme;
        public double harcama;
        public double toplamBirim;
        public double toplamMerkez;
        public double toplamTasra;
        public double toplam;
    }


    [System.Serializable()]
    public class GenelOdenekBilgi
    {
        public double[] ayDizi;

        public GenelOdenekBilgi()
        {
            ayDizi = new double[12];
            for (int i = 0; i < 12; i++)
            {
                ayDizi[i] = 0;
            }
        }

        public void AyDegerAta(int ay, double deger)
        {
            ayDizi[ay] = deger;
        }

        public string ToplamStr()
        {
            return Donem(-1).ToString("#,###");
        }

        public double Toplam()
        {
            return Donem(-1);
        }

        public double Donem(int donemNo)
        {
            int basla = 0;
            int bit = 12;
            double icToplam = 0;
            if (donemNo >= 0 && donemNo <= 3)
            {
                basla = donemNo * 3;
                bit = (donemNo + 1) * 3;
            }

            for (int i = basla; i < bit; i++)
            {
                icToplam += ayDizi[i];
            }
            return icToplam;
        }


        public string DonemStr(int donem)
        {
            return Donem(donem).ToString("#,###");
        }
    }


    [System.Serializable()]
    public class MPTertipBilgi : IComparable
    {
        public TertipOdenek tertipOdenek;
        public GenelOdenekBilgi genelOdenekBilgi = new GenelOdenekBilgi();

        public MPTertipBilgi(TertipOdenek tertipOdenek)
        {
            this.tertipOdenek = tertipOdenek;
        }

        public MPTertipBilgi(OdenekBilgi item)
        {
            this.tertipOdenek = new TertipOdenek(item);
            for (int i = 0; i < 12; i++)
            {
                this.genelOdenekBilgi.ayDizi[i] = item.EkRakam(i + 1);
            }
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj == null)
                return -1;
            if (obj.GetType() == typeof(MPTertipBilgi))
            {
                MPTertipBilgi ic = (MPTertipBilgi)obj;
                return this.tertipOdenek.tertip.kodlar[3].CompareTo(ic.tertipOdenek.tertip.kodlar[3]);
            }
            else
                return 0;
        }

        #endregion
    }

    [System.Serializable()]
    public class BirimMasrafPlaniTertip
    {
        public MPTertipBilgi[] tertipBilgiListe;

        public int KodYerBul(int kodNo, string kod)
        {
            if (tertipBilgiListe == null) return -1; //???

            for (int i = 0; i < tertipBilgiListe.Length; i++)
            {
                if (tertipBilgiListe[i].tertipOdenek.tertip.kodlar[kodNo] == kod)
                    return i;
            }

            return -1;
        }
    }

    [System.Serializable()]
    public class BirimMasrafPlaniGenel
    {
        //public BirimMasrafPlaniHarcamaIs harcamaIs = null;
        //public BirimMasrafPlaniTasinir tasinir = null;
        public BirimMasrafPlaniTertip bmpTertip = null;
    }

    [System.Serializable()]
    public class MPTavanDetay
    {
        public KurumsalYapi kurumsalYapi;
        public double[] odenek;
    }

    [System.Serializable()]
    public class MPTavanTertip
    {
        public TertipOdenek tertipOdenek;
        public MPTavanDetay[] birimTavanlari;
    }



    [System.Serializable()]
    public class PEBProjeBilgi
    {
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
        public int ALTPROJENO { get; set; }
        public string DPTPROJENO { get; set; }
    }

    [System.Serializable()]
    public class MPRevizeFormSorgu
    {
        public int yil;
        public string revizeNoAralik;
        public ENUMRevizeFormTip revizeFormTip = ENUMRevizeFormTip.Hepsi;
        public string formDurumAralik;
        public string kaydedenKullaniciKodu;
        public string kayitTarihiAralik;
        public string onaylayanKullaniciKodu;
        public string onaylamaTarihiAralik;

        public Tertip tertip;
        public bool detayGoster;
    }

    [System.Serializable()]
    public class MPDagitimAna
    {
        public int id { get; set; }
        public int yil { get; set; }
        public string kur { get; set; }
        public string kurAd { get; set; }
        public string fon { get; set; }
        public string fonAd { get; set; }
        public string fin { get; set; }
        public string finAd { get; set; }
        public string eko { get; set; }
        public string ekoAd { get; set; }
        public string peb { get; set; }
        public string pebAd { get; set; }
        public string aciklama { get; set; }
        public double miktar { get; set; }
        public TNSCollection detay { get; set; } = new TNSCollection();
        public string tertip { get { return kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb; } }
    }

    [System.Serializable()]
    public class MPDagitimDetay
    {
        public string birimKod { get; set; }
        public string birimAd { get; set; }
        public double oncYilDeger { get; set; }
        public double buYilDeger { get; set; }
        public double miktar { get; set; }
    }

    /*
            [System.Serializable()]
            public class BirimBilgi
            {
                public string birimKod;
                public string birimAd;
                public double personelSayisi;
                public double harcanan;
                public double talep;
                public double sgb;
                public double merkez;
                public double[] donemler;
            }

            [System.Serializable()]
            public class MerkezMasrafPlaniGenel
            {
                public TertipOdenek[] tertipOdenekler;
                public double[,] aylikDagilimlar;
                public BirimBilgi[] birimBilgileri;
            }

            //[System.Serializable()]
            //public class BirimMasrafPlaniHarcamaIsDetay
            //{
            //    public int islemKod;
            //    public string islemAd;
            //    public double harcanan;
            //    public double talep;
            //}

            //[System.Serializable()]
            //public class IslemKod
            //{
            //    public int islemKod;
            //    public string islemAd;

            //    public IslemKod(int islemKod, string islemAd)
            //    {
            //        this.islemAd = islemAd;
            //        this.islemKod = islemKod;
            //    }
            //}


            //[System.Serializable()]
            //public class BirimMasrafPlaniHarcamaIs
            //{
            //    public BirimMasrafPlaniHarcamaIsDetay[] detay;
            //    public GenelOdenekBilgi genelOdenekBilgi = new GenelOdenekBilgi();
            //}

            //[System.Serializable()]
            //public class TasinirBilgi
            //{
            //    public string tasinirKod;
            //    public string tasinirAd;
            //    public decimal giren;
            //    public decimal cikan;
            //    public decimal mevcut;
            //    public decimal talepEdilen;
            //    public decimal birimFiyat;
            //}

            //[System.Serializable()]
            //public class BirimMasrafPlaniTasinir
            //{
            //    public TasinirBilgi[] tasinirListe;
            //    public GenelOdenekBilgi genelOdenekBilgi = new GenelOdenekBilgi();
            //}
            */

}
