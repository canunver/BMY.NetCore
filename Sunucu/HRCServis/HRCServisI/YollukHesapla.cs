using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.BKD;
using TNS.KYM;

namespace TNS.HRC
{
    public enum ENUMYollukHesabiSatirTur
    {
        Genel,
        YolculukBilgisi,
        IkamettenTerminale,
        TerminaldenIkamete,
        GorevGunSayisi,
        EklenecekTutar,
        CikarilacakTutar,
        Konaklama,
        KonaklamaFark,
        HamalBagajUcreti,
        KurFarki
    }

    public class KonaklamaGunu
    {
        public int gunSayisi;
        public decimal gunCarpani;

        public KonaklamaGunu(int gunSayisi, decimal gunCarpani)
        {
            this.gunSayisi = gunSayisi;
            this.gunCarpani = gunCarpani;
        }
    }

    public class KonaklamaGunHesabi
    {
        public List<KonaklamaGunu> konaklamaGunDegerleri = new List<KonaklamaGunu>();

        internal void IslemBilgiBul(int toplamGunSayisi, int seferGunSayisi, out int kullanilacakGun, out decimal carpan, out int kalanGun)
        {
            kullanilacakGun = seferGunSayisi;
            carpan = 0;
            kalanGun = 0;
            decimal toplamGun = 0;
            foreach (KonaklamaGunu kg in konaklamaGunDegerleri)
            {
                if (kg.gunSayisi == -1)
                {
                    carpan = kg.gunCarpani;
                    return;
                }
                else
                {
                    toplamGun += kg.gunSayisi;
                    if (toplamGunSayisi < toplamGun)
                    {
                        if (toplamGunSayisi + seferGunSayisi > kg.gunSayisi)
                            kalanGun = toplamGunSayisi + seferGunSayisi - kg.gunSayisi;
                        else
                            kalanGun = 0;
                        kullanilacakGun = seferGunSayisi - kalanGun;
                        carpan = kg.gunCarpani;
                        return;
                    }
                }
            }
        }
    }

    public class YollukHesabiSatir
    {
        public int satirNo;
        public ENUMYollukHesabiSatirTur satirTur;
        public YollukHesabiSatir(ENUMYollukHesabiSatirTur satirTur, int satirNo)
        {
            this.satirNo = satirNo;
            this.satirTur = satirTur;
        }
        public string YolOturmaTarihi { get; set; }
        public string NeredenNereye { get; set; }
        public string HareketSaatiGidis { get; set; }
        public string HareketSaatiDonus { get; set; }

        public decimal _yolculukGunSayi;
        public decimal yolculukGunSayi { get { return _yolculukGunSayi; } set { _yolculukGunSayi = value; yolculukGunSayiStr = _yolculukGunSayi.ToString(); } }
        public string yolculukGunSayiStr { get; set; }
        public decimal gundelikDeger { get; set; }
        public decimal gundelikToplam { get; set; }
        public string AracTipi { get; set; }
        public decimal yolculuktaHarcanan { get; set; }
        public string yolculuktaHarcananPBK { get; set; }
        public string ParaBirimiSatir { get; set; }
        public decimal kurBilgisi { get; set; }

        public decimal satirToplamTutar { get; set; }
    }

    public class YollukHesabi
    {
        public string daire { get; set; }
        public string ParaBirimi { get; set; }
        public string avans { get; set; }
        public string kadro { get; set; }
        public string unvan { get; set; }
        public string adsoyad { get; set; }
        public string gundelik { get; set; }
        public string belgetarih { get; set; }
        public int yil { get; set; }
        public string surecno { get; set; }
        public ObjectArray detay = new ObjectArray();
        internal void SatirAc(ENUMYollukHesabiSatirTur satirTur, int satirNo)
        {
            aktifSatir = new YollukHesabiSatir(satirTur, satirNo);
            detay.Ekle(aktifSatir);
        }
        private YollukHesabiSatir aktifSatir;
        public decimal genelToplam { get; set; }
        public string gezilenYerler = "";
        public decimal toplamGorevGunSayi { get; set; }
        public int kullArttirimliGunSayi { get; set; }
        public decimal toplamKonaklamaTutari { get; set; }
        public decimal hdn5MaddeKonaklamaToplami { get; set; }

        public int gorevGunSayisi { get; set; }
        public string YolOturmaTarihi { get { return aktifSatir.YolOturmaTarihi; } set { aktifSatir.YolOturmaTarihi = value; } }
        public string NeredenNereye { get { return aktifSatir.NeredenNereye; } set { aktifSatir.NeredenNereye = value; } }
        public string HareketSaatiGidis { get { return aktifSatir.HareketSaatiGidis; } set { aktifSatir.HareketSaatiGidis = value; } }
        public string HareketSaatiDonus { get { return aktifSatir.HareketSaatiDonus; } set { aktifSatir.HareketSaatiDonus = value; } }
        public decimal yolculukGunSayi { get { return aktifSatir.yolculukGunSayi; } set { aktifSatir.yolculukGunSayi = value; } }
        public string yolculukGunSayiStr { get { return aktifSatir.yolculukGunSayiStr; } set { aktifSatir.yolculukGunSayiStr = value; } }
        public decimal gundelikDeger { get { return aktifSatir.gundelikDeger; } set { aktifSatir.gundelikDeger = value; } }
        public decimal gundelikToplam { get { return aktifSatir.gundelikToplam; } set { aktifSatir.gundelikToplam = value; } }
        public string AracTipi { get { return aktifSatir.AracTipi; } set { aktifSatir.AracTipi = value; } }
        public decimal yolculuktaHarcanan { get { return aktifSatir.yolculuktaHarcanan; } set { aktifSatir.yolculuktaHarcanan = value; } }
        public string yolculuktaHarcananPBK { get { return aktifSatir.yolculuktaHarcananPBK; } set { aktifSatir.yolculuktaHarcananPBK = value; } }
        public string ParaBirimiSatir { get { return aktifSatir.ParaBirimiSatir; } set { aktifSatir.ParaBirimiSatir = value; } }
        public decimal kurBilgisi { get { return aktifSatir.kurBilgisi; } set { aktifSatir.kurBilgisi = value; } }
        public decimal satirToplamTutar { get { return aktifSatir.satirToplamTutar; } set { aktifSatir.satirToplamTutar = value; } }

        List<decimal> satirToplamlar = new List<decimal>();
        internal void SatirlariHesapla()
        {
            for (int i = 0; i < detay.objeler.Count; i++)
            {
                YollukHesabiSatir sat = (YollukHesabiSatir)detay.objeler[i];
                SatiraEkle(sat.satirNo, sat.satirToplamTutar);
            }
        }

        private void SatiraEkle(int satirNo, decimal satirToplam)
        {
            for (int i = 0; i < (satirNo - satirToplamlar.Count) + 1; i++)
            {
                satirToplamlar.Add(0);
            }
            satirToplamlar[satirNo] += satirToplam;
        }

        internal decimal SatirToplamTutar(int i)
        {
            if (i < satirToplamlar.Count)
                return satirToplamlar[i];
            return 0;
        }

        internal decimal ToplamBildirimBul()
        {
            decimal top = 0;
            for (int i = 0; i < detay.objeler.Count; i++)
            {
                YollukHesabiSatir sat = (YollukHesabiSatir)detay.objeler[i];
                top += sat.satirToplamTutar;
            }
            return top;
        }
    }

    public class YollukHesapla
    {
        private GeciciGorevYollukBelge belge;
        private Kullanici kullanan;
        private IHRCServis servisHRC;

        private int muhasebe { get { return belge.muhasebeKod; } }
        private string birim { get { return belge.birimKod; } }
        private int yil { get { return belge.yil; } }
        private string surecNo { get { return belge.surecNo; } }
        private string tckimlikno { get { return belge.tcKimlikNo; } }
        private ENUMYollukGundelikTuru tur { get { return (ENUMYollukGundelikTuru)belge.belgeTur; } }

        private IdareBilgi IdareBilgiGetir()
        {
            IdareBilgi idare = new IdareBilgi();
            idare.muhasebe = muhasebe;
            idare.birim = birim;
            idare = servisHRC.IdareBilgiListele(kullanan, idare);
            return idare;
        }

        private OnayForm HarcamaTalimatiGetir()
        {
            OnayForm of = new OnayForm(yil, muhasebe, birim, surecNo);
            of.avansVarmi = (int)ENUMEvetHayir.EVET;
            of.hrcTalimati = (int)ENUMEvetHayir.EVET;
            ObjectArray onayFormBilgi = new ObjectArray();
            onayFormBilgi = servisHRC.OnayFormListele(kullanan, of);
            if (onayFormBilgi.objeler.Count > 0)
                of = (OnayForm)onayFormBilgi.objeler[0];
            else
            {
                GeciciGorevYollukTalepBaslik form = new GeciciGorevYollukTalepBaslik();
                form.formKod = of.surecNo;// hdnFormKod.Text;
                form.yil = of.yil;

                ObjectArray liste = servisHRC.GeciciGorevYollukTalepBaslikListele(kullanan, form);
                if (liste.ObjeSayisi == 1)
                {
                    form = (GeciciGorevYollukTalepBaslik)liste.objeler[0];
                    if (!string.IsNullOrEmpty(form.avansOdemeEmri))
                    {
                        of.yaklasikMaliyet = form.avansTutari;
                        of.belgeTarihi = form.tarih;
                    }
                }
            }
            return of;
        }

        private static ParaBirimi _pb = null;
        private ParaBirimi pb
        {
            get
            {
                if (_pb == null)
                {
                    ParaBirimi kriter = new ParaBirimi();
                    kriter.kod = 1;
                    ObjectArray oa = servisHRC.ParaBirimiListele(kullanan, kriter);
                    if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 1)
                        _pb = (ParaBirimi)oa.objeler[0];
                }
                return _pb;
            }
        }

        private string ParaLiraBirimiBul()
        {
            ParaBirimi pbIc = pb;
            if (pbIc == null)
                return "TL";
            return pbIc.ParaBirimiKod;
        }

        private String ParaBirimiDondur(int paraKod)
        {
            TNS.BKD.ParaBirimi para = new TNS.BKD.ParaBirimi();
            para.kod = paraKod;
            ObjectArray bilgi = new ObjectArray();
            bilgi = servisHRC.ParaBirimiListele(kullanan, para);

            return ((TNS.BKD.ParaBirimi)bilgi.objeler[0]).kisaltma;
        }

        private void YazdirUstBilgi(YollukHesabi yollukHesabi, GeciciGorevYollukBelge belge)
        {
            IdareBilgi idare = IdareBilgiGetir();
            yollukHesabi.daire = idare.adi;
            yollukHesabi.ParaBirimi = ParaLiraBirimiBul();

            OnayForm of = HarcamaTalimatiGetir();
            if (of.yaklasikMaliyet > 0.0)
                yollukHesabi.avans = of.belgeTarihi.ToString() + " - " + of.yaklasikMaliyet.ToString("#,###.00") + " " + ParaLiraBirimiBul();
            else
                yollukHesabi.avans = "Almadı";

            GeciciGorevYollukDetay detay = (GeciciGorevYollukDetay)belge.detay.objeler[0];
            string yevmiyeTutar = detay.yevmiyeTutar.ToString("#,###.00");
            string yevmiyeParaBirimi = ParaBirimiDondur(detay.yevmiyeParaBirimi.kod);
            string arttirimliYevmiyeTutar = "";
            if (belge.arttirimliHesapla)
            {
                decimal arttirimliYevmiye = detay.yevmiyeTutar * (decimal)1.5;
                string artirimliYevmiyeParaBirimi = ParaBirimiDondur(detay.yevmiyeParaBirimi.kod);
                arttirimliYevmiyeTutar = arttirimliYevmiye.ToString("#,###.00") + " " + artirimliYevmiyeParaBirimi;
            }

            yollukHesabi.adsoyad = belge.adSoyad;
            yollukHesabi.unvan = belge.unvanAdi;
            yollukHesabi.kadro = belge.kadroEk;
            if (arttirimliYevmiyeTutar == "")
                yollukHesabi.gundelik = yevmiyeTutar + " " + yevmiyeParaBirimi;
            else
                yollukHesabi.gundelik = yevmiyeTutar + " " + yevmiyeParaBirimi + " (" + arttirimliYevmiyeTutar + ")";
            yollukHesabi.belgetarih = DateTime.Now.ToShortDateString();
            yollukHesabi.yil = belge.yil;
            if (belge.surecNo.Trim() == "-1")
                yollukHesabi.surecno = "Şifresiz Giriş";
            else
                yollukHesabi.surecno = belge.surecNo;
        }

        private void KurBilgisiDegerler(KurBilgisi ff, bool havuzdan)
        {
            ff.yil = yil;
            if (havuzdan)
            {
                ff.muhasebe = -1;
                ff.birim = "-1";
                ff.surecNo = "-1";
            }
            else
            {
                ff.muhasebe = muhasebe;
                ff.birim = birim;
                ff.surecNo = surecNo;
            }
        }

        private decimal KurBilgisiDondur(int kod)
        {
            if (kod == 1 || kod == 0) //Ana Para Birimi
                return 1;
            else
            {
                KurBilgisi kur = new KurBilgisi();
                KurBilgisiDegerler(kur, false);
                kur.paraBirimi.kod = kod;
                kur.tckimlikno = tckimlikno;

                ObjectArray liste = servisHRC.KurBilgisiListele(kullanan, kur);

                if (liste.objeler.Count > 0)
                    return ((KurBilgisi)liste.objeler[0]).kur;
                else
                    return 0;
            }
        }

        private String TarihDondur(TNSDateTime tarih, int eklenecekGun)
        {
            TNSDateTime t = new TNSDateTime(tarih.Oku());
            t.AddDays(eklenecekGun);
            return t.ToString();
        }

        private decimal YevmiyeDondur(GeciciGorevYollukBelge belge, GeciciGorevYollukDetay detay, bool arttirimliMi, bool odemeOraniUygula, bool gunSiniriAsildiMi = false)
        {
            decimal gundelikDeger = 0;
            if (belge.belgeTur == ENUMYollukGundelikTuru.GECICIYURTDISI && arttirimliMi)
                gundelikDeger = detay.yevmiyeTutar * (decimal)1.5;
            else
                gundelikDeger = detay.yevmiyeTutar;

            if (gunSiniriAsildiMi)
                gundelikDeger = (gundelikDeger / 3) * 2;

            if (odemeOraniUygula)
            {
                if (belge.odemeOrani != "")
                {
                    if (belge.odemeOrani.IndexOf('/') != -1)
                    {
                        string[] kesir = belge.odemeOrani.Split('/');
                        decimal pay = OrtakFonksiyonlar.ConvertToDecimal(kesir[0], 1);
                        decimal payda = OrtakFonksiyonlar.ConvertToDecimal(kesir[1], 1);
                        gundelikDeger = (pay * gundelikDeger) / payda;
                        gundelikDeger = Math.Round(gundelikDeger, 2);
                    }
                }
            }
            return gundelikDeger;
        }

        public static String AracTipiDondur(int aracTipi)
        {
            switch (aracTipi)
            {
                case 1:
                    return HarcamaSurecleriResI.FRMGGY056;
                case 2:
                    return HarcamaSurecleriResI.FRMGGY057;
                case 3:
                    return HarcamaSurecleriResI.FRMGGY058;
                case 4:
                    return HarcamaSurecleriResI.FRMGGY059;
                case 5:
                    return HarcamaSurecleriResI.FRMGGY060;
                case 6:
                    return HarcamaSurecleriResI.FRMGGY061;
                case 7:
                    return HarcamaSurecleriResI.FRMGGY062;
                case 8:
                    return HarcamaSurecleriResI.FRMGGY063;
                case 9:
                    return HarcamaSurecleriResI.FRMGGY064;
                case 10:
                    return HarcamaSurecleriResI.FRMGGY065;
                default:
                    return string.Empty;
            }
        }

        private void YazdirYolculukBilgisi(int satirNo, YollukHesabi yollukHesabi, GeciciGorevYollukBelge belge, GeciciGorevYollukDetay detay, int gunSiniri)
        {
            if (detay.yolculukTipi != (int)ENUMGeciciGorevYolculukYonu.GIDIS && detay.yolculukTipi != (int)ENUMGeciciGorevYolculukYonu.DONUS) return;
            decimal satirToplamTutar = 0;
            decimal kurBilgisi = KurBilgisiDondur(detay.yolculuktaParaBirimi.kod);
            decimal yolculuktaHarcanan = 0;

            yolculuktaHarcanan = Math.Round(detay.yolculuktaHarcanan * kurBilgisi, 2);

            yollukHesabi.SatirAc(ENUMYollukHesabiSatirTur.YolculukBilgisi, satirNo);

            if (detay.yolculukGunSayi > 1)
                yollukHesabi.YolOturmaTarihi = detay.hareketTarihi.ToString() + " - " + TarihDondur(detay.hareketTarihi, (int)detay.yolculukGunSayi);
            else
                yollukHesabi.YolOturmaTarihi = detay.hareketTarihi.ToString();

            yollukHesabi.NeredenNereye = detay.nereden + " - " + detay.nereye;
            if (detay.yolculukTipi == (int)ENUMGeciciGorevYolculukYonu.GIDIS)
            {
                yollukHesabi.HareketSaatiGidis = detay.hareketSaati;
                if (!yollukHesabi.gezilenYerler.Contains(detay.nereye))
                    yollukHesabi.gezilenYerler += detay.nereye + ",";
            }
            else if (detay.yolculukTipi == (int)ENUMGeciciGorevYolculukYonu.DONUS)
                yollukHesabi.HareketSaatiDonus = detay.hareketSaati;

            if (detay.yolculukGunSayi > 0)
            {
                yollukHesabi.yolculukGunSayi = detay.yolculukGunSayi;
                yollukHesabi.yolculukGunSayiStr = detay.yolculukGunSayiStr;

                decimal gundelikDeger;

                yollukHesabi.toplamGorevGunSayi += detay.yolculukGunSayi;
                if (yollukHesabi.toplamGorevGunSayi > gunSiniri)
                    gundelikDeger = YevmiyeDondur(belge, detay, false, false);
                else if (belge.arttirimliHesapla)
                    gundelikDeger = YevmiyeDondur(belge, detay, true, false);
                else
                    gundelikDeger = YevmiyeDondur(belge, detay, false, false);

                if (yolculuktaHarcanan == 0)
                    kurBilgisi = KurBilgisiDondur(detay.yevmiyeParaBirimi.kod);

                yollukHesabi.gundelikDeger = gundelikDeger; //.ToString("#,###.00"));
                decimal gundelikToplam = detay.yolculukGunSayi * gundelikDeger;
                gundelikToplam = Math.Round(gundelikToplam, 2);
                yollukHesabi.gundelikToplam = gundelikToplam;//.ToString("#,###.00"));

                satirToplamTutar += Math.Round(gundelikToplam * KurBilgisiDondur(detay.yevmiyeParaBirimi.kod), 2);
            }

            yollukHesabi.AracTipi = AracTipiDondur(detay.yolculukAracTipi);
            if (detay.yolculuktaHarcanan > 0)
                yollukHesabi.yolculuktaHarcanan = detay.yolculuktaHarcanan; //.ToString("#,###.00"));
            satirToplamTutar += yolculuktaHarcanan;
            if (detay.yolculuktaHarcanan == 0)
                yollukHesabi.ParaBirimiSatir = ParaBirimiDondur(detay.yevmiyeParaBirimi.kod);
            else
                yollukHesabi.ParaBirimiSatir = ParaBirimiDondur(detay.yolculuktaParaBirimi.kod);
            if (kurBilgisi != 1)
                yollukHesabi.kurBilgisi = kurBilgisi; //.ToString());
            yollukHesabi.satirToplamTutar = satirToplamTutar; //.ToString("#,###.00"));
            yollukHesabi.genelToplam += satirToplamTutar;
        }

        private void YazdirIkamettenTerminale(int satirNo, YollukHesabi yollukHesabi, GeciciGorevYollukBelge belge, GeciciGorevYollukDetay detay)
        {
            if (detay.ikamettenTerminaleAracTipi > 0 && detay.yolculukTipi != (int)ENUMGeciciGorevYolculukYonu.GOREV)
            {
                decimal kurBilgisi = KurBilgisiDondur(detay.ikamettenTerminaleParaBirimi.kod);
                decimal ikamettenTerminaleHarcanan = 0;

                ikamettenTerminaleHarcanan = Math.Round(detay.ikamettenTerminaleHarcanan * kurBilgisi, 2);

                yollukHesabi.SatirAc(ENUMYollukHesabiSatirTur.IkamettenTerminale, satirNo);

                yollukHesabi.YolOturmaTarihi = detay.hareketTarihi.ToString();
                if (detay.yolculukTipi == (int)ENUMGeciciGorevYolculukYonu.GIDIS)
                    yollukHesabi.NeredenNereye = HarcamaSurecleriResI.FRMGGY079;
                else
                    yollukHesabi.NeredenNereye = HarcamaSurecleriResI.FRMGGY080;
                yollukHesabi.AracTipi = AracTipiDondur(detay.ikamettenTerminaleAracTipi);
                yollukHesabi.yolculuktaHarcanan = detay.ikamettenTerminaleHarcanan; //.ToString("#,###.00"));
                yollukHesabi.ParaBirimiSatir = ParaBirimiDondur(detay.ikamettenTerminaleParaBirimi.kod);
                if (kurBilgisi != 1)
                    yollukHesabi.kurBilgisi = kurBilgisi; //.ToString());
                yollukHesabi.satirToplamTutar = ikamettenTerminaleHarcanan; //.ToString("#,###.00"));
                yollukHesabi.genelToplam += ikamettenTerminaleHarcanan;
            }
        }

        private void YazdirTerminaldenIkamete(int satirNo, YollukHesabi yollukHesabi, GeciciGorevYollukBelge belge, GeciciGorevYollukDetay detay)
        {
            if (detay.terminaldenIkameteAracTipi > 0 && detay.yolculukTipi != (int)ENUMGeciciGorevYolculukYonu.GOREV)
            {
                decimal kurBilgisi = KurBilgisiDondur(detay.terminaldenIkameteParaBirimi.kod);
                decimal terminaldenIkameteHarcanan = Math.Round(detay.terminaldenIkameteHarcanan * kurBilgisi, 2);

                yollukHesabi.SatirAc(ENUMYollukHesabiSatirTur.TerminaldenIkamete, satirNo);

                yollukHesabi.YolOturmaTarihi = detay.hareketTarihi.ToString();
                if (detay.yolculukTipi == (int)ENUMGeciciGorevYolculukYonu.GIDIS)
                    yollukHesabi.NeredenNereye = HarcamaSurecleriResI.FRMGGY077;
                else
                    yollukHesabi.NeredenNereye = HarcamaSurecleriResI.FRMGGY078;
                yollukHesabi.AracTipi = AracTipiDondur(detay.terminaldenIkameteAracTipi);
                yollukHesabi.yolculuktaHarcanan = detay.terminaldenIkameteHarcanan; //.ToString("#,###.00"));
                yollukHesabi.ParaBirimiSatir = ParaBirimiDondur(detay.terminaldenIkameteParaBirimi.kod);
                if (kurBilgisi != 1)
                    yollukHesabi.kurBilgisi = kurBilgisi;
                yollukHesabi.satirToplamTutar = terminaldenIkameteHarcanan; //.ToString("#,###.00"));
                yollukHesabi.genelToplam += terminaldenIkameteHarcanan;
            }
        }

        private void YazdirGorevGunSayisi(int satirNo, YollukHesabi yollukHesabi, GeciciGorevYollukBelge belge, GeciciGorevYollukDetay detay, int arttirimligunSiniri)
        {
            if (detay.gorevGunSayisi > 0)
            {
                int gunSiniri = 90;

                yollukHesabi.toplamGorevGunSayi += (decimal)detay.gorevGunSayisi;

                decimal kurBilgisi = KurBilgisiDondur(detay.yevmiyeParaBirimi.kod);
                decimal gundelikDeger = YevmiyeDondur(belge, detay, false, true);// detay.yevmiyeTutar;
                decimal gundelikDegerArttirimli = YevmiyeDondur(belge, detay, true, true);  //0;
                decimal gundelikDegerKismi = YevmiyeDondur(belge, detay, false, false, true);  //0;

                int gunSayisi = 0;
                yollukHesabi.gorevGunSayisi = detay.gorevGunSayisi;
                if (belge.belgeTur == ENUMYollukGundelikTuru.GECICIYURTDISI && belge.arttirimliHesapla)
                {
                    yollukHesabi.SatirAc(ENUMYollukHesabiSatirTur.GorevGunSayisi, satirNo);
                    arttirimligunSiniri -= yollukHesabi.kullArttirimliGunSayi;

                    if (yollukHesabi.gorevGunSayisi > (arttirimligunSiniri - 1))
                    {
                        if (arttirimligunSiniri <= 0)
                            gunSayisi = yollukHesabi.gorevGunSayisi;
                        else
                            gunSayisi = arttirimligunSiniri - 1;

                        yollukHesabi.kullArttirimliGunSayi = arttirimligunSiniri;
                    }
                    else
                    {
                        gunSayisi = yollukHesabi.gorevGunSayisi;
                        yollukHesabi.kullArttirimliGunSayi = (yollukHesabi.gorevGunSayisi + 1);
                    }

                    if (arttirimligunSiniri > 0)
                    {
                        int eklenecekGunSayisi = 1;
                        if (detay.yolculukTipi == (int)ENUMGeciciGorevYolculukYonu.GOREV)
                            eklenecekGunSayisi = 0;

                        if (gunSayisi - 1 > 0)
                            yollukHesabi.YolOturmaTarihi = TarihDondur(detay.hareketTarihi, eklenecekGunSayisi) + " - " + TarihDondur(detay.hareketTarihi, gunSayisi + eklenecekGunSayisi - 1);
                        else
                            yollukHesabi.YolOturmaTarihi = TarihDondur(detay.hareketTarihi, eklenecekGunSayisi);
                        string metin = "görev";

                        if (detay.yolculukTipi == (int)ENUMGeciciGorevYolculukYonu.GOREV && detay.yolculukGunSayi != 1)
                        {
                            metin += " (" + detay.yolculukGunSayiStr + ")";
                        }

                        yollukHesabi.NeredenNereye = detay.nereye + " " + metin;
                        yollukHesabi.yolculukGunSayi = gunSayisi;
                        decimal yevmiyeToplamiArttirimli = gundelikDegerArttirimli * gunSayisi;
                        if (detay.yolculukTipi == (int)ENUMGeciciGorevYolculukYonu.GOREV)
                        {
                            yevmiyeToplamiArttirimli = yevmiyeToplamiArttirimli * detay.yolculukGunSayi;
                        }
                        yollukHesabi.gundelikDeger = gundelikDegerArttirimli; //.ToString("#,###.00"));
                        yollukHesabi.gundelikToplam = yevmiyeToplamiArttirimli; //.ToString("#,###.00"));

                        decimal satirToplamTutar = 0;

                        satirToplamTutar = Math.Round(yevmiyeToplamiArttirimli * KurBilgisiDondur(detay.yevmiyeParaBirimi.kod), 2);

                        yollukHesabi.ParaBirimiSatir = ParaBirimiDondur(detay.yevmiyeParaBirimi.kod);
                        if (kurBilgisi != 1)
                            yollukHesabi.kurBilgisi = kurBilgisi; //.ToString());
                        yollukHesabi.satirToplamTutar = satirToplamTutar; //.ToString("#,###.00"));
                        yollukHesabi.genelToplam += satirToplamTutar;
                    }
                }

                if (yollukHesabi.gorevGunSayisi > gunSiniri)
                {
                    gundelikDeger = YevmiyeDondur(belge, detay, false, true, false);// detay.yevmiyeTutar;

                    yollukHesabi.SatirAc(ENUMYollukHesabiSatirTur.GorevGunSayisi, satirNo);

                    int gun = gunSiniri;
                    if (belge.arttirimliHesapla)
                        gun = gunSiniri - arttirimligunSiniri;

                    if (detay.gorevGunSayisi - gunSayisi - 1 > 0)
                        yollukHesabi.YolOturmaTarihi = TarihDondur(detay.hareketTarihi, 1) + " - " + TarihDondur(detay.hareketTarihi, gun + 1 - 1); //detay.gorevGunSayisi - gunSayisi - 1));
                    else
                        yollukHesabi.YolOturmaTarihi = TarihDondur(detay.hareketTarihi, 1);
                    yollukHesabi.NeredenNereye = detay.nereye + " görev";

                    yollukHesabi.yolculukGunSayi = gunSiniri;
                    yollukHesabi.gundelikDeger = gundelikDeger; //.ToString("#,###.00"));
                    yollukHesabi.gundelikToplam = (gun * gundelikDeger); //.ToString("#,###.00"));

                    decimal satirToplamTutar = 0;

                    satirToplamTutar += Math.Round((gun * gundelikDeger) * KurBilgisiDondur(detay.yevmiyeParaBirimi.kod), 2);

                    yollukHesabi.ParaBirimiSatir = ParaBirimiDondur(detay.yevmiyeParaBirimi.kod);
                    if (kurBilgisi != 1)
                        yollukHesabi.kurBilgisi = kurBilgisi; //.ToString());
                    yollukHesabi.satirToplamTutar = satirToplamTutar; //.ToString("#,###.00"));
                    yollukHesabi.genelToplam += satirToplamTutar;
                    satirToplamTutar = 0;

                    gundelikDeger = YevmiyeDondur(belge, detay, false, true, true);// detay.yevmiyeTutar;

                    yollukHesabi.SatirAc(ENUMYollukHesabiSatirTur.GorevGunSayisi, satirNo);

                    if (detay.gorevGunSayisi - gun - 1 > 0)
                        yollukHesabi.YolOturmaTarihi = TarihDondur(detay.hareketTarihi, 1) + " - " + TarihDondur(detay.hareketTarihi, detay.gorevGunSayisi - gun);
                    else
                        yollukHesabi.YolOturmaTarihi = TarihDondur(detay.hareketTarihi, 1);
                    yollukHesabi.NeredenNereye = detay.nereye + " görev";

                    int kalanGunSayisi = yollukHesabi.gorevGunSayisi - gun;
                    if (belge.arttirimliHesapla)
                        kalanGunSayisi -= arttirimligunSiniri;

                    yollukHesabi.yolculukGunSayi = kalanGunSayisi;
                    yollukHesabi.gundelikDeger = gundelikDeger; //.ToString("#,###.00"));
                    yollukHesabi.gundelikToplam = (kalanGunSayisi * gundelikDeger); //.ToString("#,###.00"));

                    satirToplamTutar += Math.Round((kalanGunSayisi * gundelikDeger) * KurBilgisiDondur(detay.yevmiyeParaBirimi.kod), 2);

                    yollukHesabi.ParaBirimiSatir = ParaBirimiDondur(detay.yevmiyeParaBirimi.kod);
                    if (kurBilgisi != 1)
                        yollukHesabi.kurBilgisi = kurBilgisi;
                    yollukHesabi.satirToplamTutar = satirToplamTutar; //.ToString("#,###.00"));
                    yollukHesabi.genelToplam += satirToplamTutar;
                    satirToplamTutar = 0;
                }
                else if (yollukHesabi.gorevGunSayisi - gunSayisi > 0 || arttirimligunSiniri <= 0)
                {
                    int gunSayi = detay.gorevGunSayisi - gunSayisi;
                    if (arttirimligunSiniri <= 0)
                        gunSayi = yollukHesabi.gorevGunSayisi;

                    //belge.arttirimliHesapla = false;
                    gundelikDeger = YevmiyeDondur(belge, detay, false, true);// detay.yevmiyeTutar;

                    yollukHesabi.SatirAc(ENUMYollukHesabiSatirTur.GorevGunSayisi, satirNo);

                    int eklenecekGunSayisi = 1;
                    if (detay.yolculukTipi == (int)ENUMGeciciGorevYolculukYonu.GOREV)
                        eklenecekGunSayisi = 0;

                    if (detay.gorevGunSayisi - gunSayisi - 1 > 0)
                        yollukHesabi.YolOturmaTarihi = TarihDondur(detay.hareketTarihi, eklenecekGunSayisi) + " - " + TarihDondur(detay.hareketTarihi, gunSayi + eklenecekGunSayisi - 1);
                    else
                        yollukHesabi.YolOturmaTarihi = TarihDondur(detay.hareketTarihi, eklenecekGunSayisi);

                    string metin = "görev";
                    if (detay.yolculukTipi == (int)ENUMGeciciGorevYolculukYonu.GOREV && detay.yolculukGunSayi != 1)
                    {
                        metin += " (" + detay.yolculukGunSayiStr + ")";
                    }

                    yollukHesabi.NeredenNereye = detay.nereye + " " + metin;

                    yollukHesabi.yolculukGunSayi = gunSayi;
                    yollukHesabi.gundelikDeger = gundelikDeger; //.ToString("#,###.00"));
                    decimal icGundelikToplam = (gunSayi * gundelikDeger);
                    if (detay.yolculukTipi == (int)ENUMGeciciGorevYolculukYonu.GOREV)
                        icGundelikToplam = icGundelikToplam * detay.yolculukGunSayi;
                    yollukHesabi.gundelikToplam = icGundelikToplam; //.ToString("#,###.00"));
                    decimal satirToplamTutar = 0;

                    satirToplamTutar = Math.Round(icGundelikToplam * KurBilgisiDondur(detay.yevmiyeParaBirimi.kod), 2);
                    yollukHesabi.ParaBirimiSatir = ParaBirimiDondur(detay.yevmiyeParaBirimi.kod);
                    if (kurBilgisi != 1)
                        yollukHesabi.kurBilgisi = kurBilgisi;
                    yollukHesabi.satirToplamTutar = satirToplamTutar; //.ToString("#,###.00"));
                    yollukHesabi.genelToplam += satirToplamTutar;
                }
            }
        }

        private void KonaklamaBilgiTopla(int satirNo, ObjectArray bilgiKonaklamaDetay, GeciciGorevYollukBelge belge, GeciciGorevYollukDetay detay)
        {
            if (detay.konaklananGunSayi > 0 && (detay.yolculukTipi == (int)ENUMGeciciGorevYolculukYonu.GIDIS || detay.yolculukTipi == (int)ENUMGeciciGorevYolculukYonu.GOREV))
            {
                GeciciGorevYollukDetay konaklamaDetay = new GeciciGorevYollukDetay();
                bilgiKonaklamaDetay.objeler.Add(konaklamaDetay);
                konaklamaDetay.konaklananGunSayi = detay.konaklananGunSayi;
                konaklamaDetay.yolculukTipi = detay.yolculukTipi;
                konaklamaDetay.konaklamaGunlukFatura = detay.konaklamaGunlukFatura; //belge.gundelikDeger;
                konaklamaDetay.nereye = detay.nereye;
                konaklamaDetay.yevmiyeTutar = detay.yevmiyeTutar;
                konaklamaDetay.yevmiyeParaBirimi = detay.yevmiyeParaBirimi;
                konaklamaDetay.konaklamaGunlukFatura = detay.konaklamaGunlukFatura;
                konaklamaDetay.konaklamaGunlukFaturaParaBirimi = detay.konaklamaGunlukFaturaParaBirimi;
                konaklamaDetay.kayitSiraNo = satirNo;
            }
        }

        private void YazdirCikarilacakTutar(int satirNo, YollukHesabi yollukHesabi, GeciciGorevYollukBelge belge)
        {
            if (Math.Abs(belge.cikarilacakTutar) > 0 && belge.cikarilacakAciklama != "")
            {
                yollukHesabi.SatirAc(ENUMYollukHesabiSatirTur.CikarilacakTutar, satirNo);
                yollukHesabi.YolOturmaTarihi = belge.cikarilacakAciklama;

                decimal kurBilgisi = KurBilgisiDondur(belge.cikarilacakTutarParaBirimi.kod);
                if (kurBilgisi != 1)
                {
                    yollukHesabi.yolculuktaHarcanan = belge.cikarilacakTutar;//.ToString("#,###.00"));
                    yollukHesabi.ParaBirimiSatir = ParaBirimiDondur(belge.cikarilacakTutarParaBirimi.kod);
                    yollukHesabi.kurBilgisi = kurBilgisi; //.ToString());
                }
                decimal satirToplamTutar = -1 * Math.Round(belge.cikarilacakTutar * kurBilgisi, 2);
                yollukHesabi.satirToplamTutar = satirToplamTutar; //.ToString("#,###.00"));
                yollukHesabi.genelToplam += satirToplamTutar;
            }
        }

        private void YazdirEklenecekTutar(int satirNo, YollukHesabi yollukHesabi, GeciciGorevYollukBelge belge)
        {
            if (belge.eklenecekTutar > 0 && belge.eklenecekAciklama != "")
            {
                yollukHesabi.SatirAc(ENUMYollukHesabiSatirTur.EklenecekTutar, satirNo);
                yollukHesabi.YolOturmaTarihi = belge.eklenecekAciklama;
                decimal kurBilgisi = KurBilgisiDondur(belge.eklenecekTutarParaBirimi.kod);

                if (kurBilgisi != 1)
                {
                    yollukHesabi.yolculuktaHarcanan = belge.eklenecekTutar;//.ToString("#,###.00"));
                    yollukHesabi.ParaBirimiSatir = ParaBirimiDondur(belge.eklenecekTutarParaBirimi.kod);
                    yollukHesabi.kurBilgisi = kurBilgisi; //.ToString());
                }
                decimal satirToplamTutar = Math.Round(belge.eklenecekTutar * kurBilgisi, 2);
                yollukHesabi.satirToplamTutar = satirToplamTutar; //.ToString("#,###.00"));
                yollukHesabi.genelToplam += satirToplamTutar;
            }
        }

        private void YazdirHamalBagaj(int satirNo, YollukHesabi yollukHesabi, GeciciGorevYollukBelge belge, GeciciGorevYollukDetay detay)
        {
            if (detay.hamalBagaj > 0)
            {
                yollukHesabi.SatirAc(ENUMYollukHesabiSatirTur.HamalBagajUcreti, satirNo);
                yollukHesabi.YolOturmaTarihi = detay.hareketTarihi.ToString();
                yollukHesabi.NeredenNereye = HarcamaSurecleriResI.FRMGGY076;

                decimal kurBilgisi = KurBilgisiDondur(detay.hamalBagajParaBirimi.kod);
                if (kurBilgisi != 1)
                {
                    yollukHesabi.yolculuktaHarcanan = detay.hamalBagaj; //.ToString("#,###.00"));
                    yollukHesabi.ParaBirimiSatir = ParaBirimiDondur(detay.hamalBagajParaBirimi.kod);
                    yollukHesabi.kurBilgisi = kurBilgisi; //.ToString());
                }
                decimal hamalBagajUcreti = Math.Round(detay.hamalBagaj * kurBilgisi, 2);

                yollukHesabi.satirToplamTutar = hamalBagajUcreti; //.ToString("#,###.00"));
                yollukHesabi.genelToplam += hamalBagajUcreti;
            }
        }

        private void YazdirKurFarkiTutar(int satirNo, YollukHesabi yollukHesabi, GeciciGorevYollukBelge belge)
        {
            if (belge.belgeTur == ENUMYollukGundelikTuru.GECICIYURTDISI)
            {
                double avansDbl;
                string avansBilgiStr;
                string avansBelgeNo;
                TNSDateTime avansTarih;
                TNS.HRC.Arac.AvansHesaplaMUH(TNS.MUH.Arac.Tanimla(), kullanan, yil, surecNo, out avansDbl, out avansBilgiStr, out avansBelgeNo, out avansTarih);
                decimal avans = Convert.ToDecimal(avansDbl);
                decimal toplamBildirim = yollukHesabi.ToplamBildirimBul();
                if (avans > 0 && toplamBildirim > avans)
                {
                    if (belge.detay.ObjeSayisi <= 0)
                        throw new Exception("Avans alındığı için, avans farkı hesaplanamabilmesi için en az bir bildirim satırı girilmelidir.");
                    GeciciGorevYollukDetay detay = (GeciciGorevYollukDetay)belge.detay[0];
                    decimal kurBilgisiAvans = KurBilgisiDondur(detay.yevmiyeParaBirimi.kod);
                    if (kurBilgisiAvans == 0)
                        throw new Exception("Avans farkı hesabı için gereken, avans tarihi kur bilgisi bulunamadı!");
                    TNSDateTime btDt = new TNSDateTime(belge.bildirimTarihi);
                    decimal kurBilgisiBildirim = HRC.Arac.KurDegeriDondur(kullanan, detay.yevmiyeParaBirimi, ref btDt, false, 2);
                    if (kurBilgisiBildirim == 0)
                        throw new Exception("Avans farkı hesabı için gereken, bildirim tarihi kur bilgisi bulunamadı!");

                    decimal avansUstu = toplamBildirim - avans;

                    decimal avansUstuYeniTL = OrtakFonksiyonlar.KurusYap(avansUstu / kurBilgisiAvans * kurBilgisiBildirim);
                    decimal kurFarkiTutar = OrtakFonksiyonlar.KurusYap(avansUstuYeniTL - avansUstu);
                    yollukHesabi.SatirAc(ENUMYollukHesabiSatirTur.KurFarki, satirNo);

                    yollukHesabi.YolOturmaTarihi = $"Kur Farkı (Avans Üstü:{avansUstu:#,###.00}, Bildirim Tarihi Karşılığı: {avansUstuYeniTL:#,###.00}) ";
                    yollukHesabi.satirToplamTutar = kurFarkiTutar;
                    yollukHesabi.ParaBirimiSatir = ParaBirimiDondur(detay.yevmiyeParaBirimi.kod);
                    yollukHesabi.kurBilgisi = kurBilgisiBildirim;
                    yollukHesabi.genelToplam += kurFarkiTutar;
                }
            }
        }

        public static IntKodStrAd[] ListeleKonaklamaTur()
        {
            return new IntKodStrAd[] {
                new IntKodStrAd(7, KonaklamaKatsayiDevlet19.Ad),
                new IntKodStrAd(5, KonaklamaKatsayiDevlet16.Ad),
                new IntKodStrAd(6, KonaklamaKatsayiDenetim175.Ad),
                new IntKodStrAd(1, KonaklamaKatsayiDevlet.Ad),
                new IntKodStrAd(2, KonaklamaKatsayiCum3.Ad),
                new IntKodStrAd(3, KonaklamaKatsayiCum5.Ad),
                new IntKodStrAd(4, KonaklamaKatsayiDevlet28.Ad),
            };
        }

        public static KonaklamaKatsayi NesneBulKonaklamaTur(int yurtIciKonaklamaTuru)
        {
            if (yurtIciKonaklamaTuru == 2)
                return new KonaklamaKatsayiCum3();
            else if (yurtIciKonaklamaTuru == 3)
                return new KonaklamaKatsayiCum5();
            else if (yurtIciKonaklamaTuru == 4)
                return new KonaklamaKatsayiDevlet28();
            else if (yurtIciKonaklamaTuru == 5)
                return new KonaklamaKatsayiDevlet16();
            else if (yurtIciKonaklamaTuru == 6)
                return new KonaklamaKatsayiDenetim175();
            else if (yurtIciKonaklamaTuru == 7)
                return new KonaklamaKatsayiDevlet19();
            else
                return new KonaklamaKatsayiDevlet();
        }


        private void YazdirKonaklamaBilgi(ObjectArray bilgiKonaklamaDetay, YollukHesabi yollukHesabi, GeciciGorevYollukBelge belge)
        {
            if (bilgiKonaklamaDetay == null || bilgiKonaklamaDetay.ObjeSayisi <= 0) return;
            yollukHesabi.toplamKonaklamaTutari = 0;
            int toplamGunSayisi = 0;
            int yolIndis = 0;
            KonaklamaKatsayi kk = NesneBulKonaklamaTur(belge.yurtIciKonaklamaTuru);

            KonaklamaGunHesabi kgh = kk.GunleriBul(belge.belgeTur);
            foreach (GeciciGorevYollukDetay konaklamaDetay in bilgiKonaklamaDetay.objeler)
            {
                int seferGunSayisi = Convert.ToInt32(konaklamaDetay.konaklananGunSayi);
                do
                {
                    int kullanilacakGun, kalanGun;
                    decimal carpan;
                    kgh.IslemBilgiBul(toplamGunSayisi, seferGunSayisi, out kullanilacakGun, out carpan, out kalanGun);
                    if (kullanilacakGun > 0 && carpan > 0)
                    {
                        KonaklamaSatirEkle(konaklamaDetay, yollukHesabi, kullanilacakGun, carpan, belge);
                    }
                    toplamGunSayisi += kullanilacakGun;
                    seferGunSayisi = kalanGun;
                } while (seferGunSayisi > 0);
                yolIndis++;
            }

            if (yollukHesabi.hdn5MaddeKonaklamaToplami > decimal.Zero)
            {
                yollukHesabi.SatirAc(ENUMYollukHesabiSatirTur.KonaklamaFark, 0);
                yollukHesabi.YolOturmaTarihi = "5.madde kapsamındaki ödenecek konaklama tutarı farkı";
                yollukHesabi.satirToplamTutar = yollukHesabi.hdn5MaddeKonaklamaToplami;
                yollukHesabi.genelToplam += yollukHesabi.satirToplamTutar;
            }
        }

        private void KonaklamaSatirEkle(GeciciGorevYollukDetay konaklamaDetay, YollukHesabi yollukHesabi, int kullanilacakGun, decimal carpan, GeciciGorevYollukBelge belge)
        {
            yollukHesabi.SatirAc(ENUMYollukHesabiSatirTur.Konaklama, konaklamaDetay.kayitSiraNo);
            yollukHesabi.YolOturmaTarihi = string.Format(HarcamaSurecleriResI.FRMGGY072, konaklamaDetay.nereye);
            yollukHesabi.yolculukGunSayi = kullanilacakGun;
            if (belge.belgeTur != ENUMYollukGundelikTuru.GECICIYURTDISI)
            {
                if (!belge.konaklamaTamamiOdenecekMi)
                {
                    decimal gunlukMaksKonaklamaUcreti = konaklamaDetay.yevmiyeTutar * carpan;
                    if (konaklamaDetay.konaklamaGunlukFatura > gunlukMaksKonaklamaUcreti)
                        konaklamaDetay.konaklamaGunlukOdenen = gunlukMaksKonaklamaUcreti;
                    else
                        konaklamaDetay.konaklamaGunlukOdenen = konaklamaDetay.konaklamaGunlukFatura;
                }
                yollukHesabi.yolculuktaHarcanan = konaklamaDetay.konaklamaGunlukOdenen; //.ToString("#,###.00") + " " + 
                yollukHesabi.yolculuktaHarcananPBK = konaklamaDetay.konaklamaGunlukFaturaParaBirimi.kisaltma;
            }
            else
            {
                decimal yurtDisiGundelikDeger = konaklamaDetay.yevmiyeTutar;
                if (belge.arttirimliHesapla)
                {
                    yurtDisiGundelikDeger = konaklamaDetay.yevmiyeTutar * (decimal)1.5;
                }

                decimal odenecekTutar = 0;
                decimal konaklamaGunlukFatura = konaklamaDetay.konaklamaGunlukFatura; //*KurBilgisiAl(kullanan, kur, yolculuk.konaklamaGunlukFaturaParaBirimi.kod);

                decimal asanKisim = decimal.Zero;
                decimal yuzde40i = OrtakFonksiyonlar.KurusYap(yurtDisiGundelikDeger * (decimal)(0.4));
                decimal maksTutar = OrtakFonksiyonlar.KurusYap(yurtDisiGundelikDeger * (decimal)(0.7));
                if (konaklamaGunlukFatura > yuzde40i)
                {
                    asanKisim = konaklamaGunlukFatura - yuzde40i;
                    odenecekTutar = OrtakFonksiyonlar.KurusYap(asanKisim * (decimal)(0.7));
                    if (odenecekTutar > maksTutar)
                        odenecekTutar = maksTutar;
                    asanKisim -= odenecekTutar;
                }
                else
                    odenecekTutar = konaklamaGunlukFatura * 0; //fazla değilse 0

                if (belge.konaklamaTamamiOdenecekMi && asanKisim > 0)
                {
                    yollukHesabi.hdn5MaddeKonaklamaToplami += Math.Round(kullanilacakGun * asanKisim * KurBilgisiDondur(konaklamaDetay.konaklamaGunlukFaturaParaBirimi.kod), 2);
                    //decimal toplamKonaklama = Math.Round(kullanilacakGun * konaklamaGunlukFatura * KurBilgisiDondur(konaklamaDetay.konaklamaGunlukFaturaParaBirimi.kod), 2);
                    //if (odenecekTutar > 0)
                    //{
                    //decimal odenecekAsanKisim = yollukHesabi.hdnKonaklamaFark;
                    //odenecekAsanKisim += toplamKonaklama - (Math.Round(odenecekTutar * kullanilacakGun * KurBilgisiDondur(konaklamaDetay.konaklamaGunlukFaturaParaBirimi.kod), 2));
                    //yollukHesabi.hdnKonaklamaFark = odenecekAsanKisim;
                    //}
                }
                yollukHesabi.yolculuktaHarcanan = konaklamaDetay.konaklamaGunlukFatura; //.ToString("#,###.00") + " " + 
                yollukHesabi.yolculuktaHarcananPBK = konaklamaDetay.konaklamaGunlukFaturaParaBirimi.kisaltma;
                konaklamaDetay.konaklamaGunlukFatura = odenecekTutar;
                konaklamaDetay.konaklamaGunlukOdenen = odenecekTutar;
            }

            konaklamaDetay.toplamKonaklamaOdenen = Math.Round(kullanilacakGun * konaklamaDetay.konaklamaGunlukOdenen * KurBilgisiDondur(konaklamaDetay.konaklamaGunlukFaturaParaBirimi.kod), 2);
            yollukHesabi.toplamKonaklamaTutari = konaklamaDetay.toplamKonaklamaOdenen;
            yollukHesabi.ParaBirimiSatir = HarcamaSurecleriResI.FRMGGY074;
            yollukHesabi.satirToplamTutar = yollukHesabi.toplamKonaklamaTutari; //.ToString("#,###.00"));
            yollukHesabi.genelToplam += yollukHesabi.toplamKonaklamaTutari;
        }


        /// <summary>
        /// Yazdır butonuna basılınca formdaki parametrelere göre geçici görev yolluk formu yazdıran fonksiyon
        /// </summary>
        /// <param name="sender">Çağıran, object tipinde sınıf</param>
        /// <param name="e">Olay argümanı, Event Args tipinde sınıf</param>
        public YollukHesabi Hesapla(bool ustBilgiBul)
        {
            int gunSiniri = 10;
            ObjectArray bilgiKonaklamaDetay = new ObjectArray();
            YollukHesabi yollukHesabi = new YollukHesabi();

            if (belge != null && belge.detay.objeler.Count > 0)
            {
                if (ustBilgiBul)
                    YazdirUstBilgi(yollukHesabi, belge);

                for (int i = 0; i < belge.detay.objeler.Count; i++)
                {
                    GeciciGorevYollukDetay detay = (GeciciGorevYollukDetay)belge.detay.objeler[i];
                    YazdirIkamettenTerminale(i, yollukHesabi, belge, detay);
                    YazdirYolculukBilgisi(i, yollukHesabi, belge, detay, gunSiniri);
                    YazdirTerminaldenIkamete(i, yollukHesabi, belge, detay);
                    YazdirHamalBagaj(i, yollukHesabi, belge, detay);
                    YazdirGorevGunSayisi(i, yollukHesabi, belge, detay, gunSiniri);
                    KonaklamaBilgiTopla(i, bilgiKonaklamaDetay, belge, detay);
                }
                YazdirEklenecekTutar(0, yollukHesabi, belge);
                YazdirCikarilacakTutar(0, yollukHesabi, belge);
                YazdirKonaklamaBilgi(bilgiKonaklamaDetay, yollukHesabi, belge);
                YazdirKurFarkiTutar(0, yollukHesabi, belge);

                yollukHesabi.SatirlariHesapla();
                for (int i = 0; i < belge.detay.objeler.Count; i++)
                {
                    GeciciGorevYollukDetay detay = (GeciciGorevYollukDetay)belge.detay.objeler[i];
                    detay.toplamTutar = yollukHesabi.SatirToplamTutar(i);
                }

            }
            return yollukHesabi;
        }

        public YollukHesapla(GeciciGorevYollukBelge belge, Kullanici kullanan, IHRCServis servisHRC)
        {
            this.kullanan = kullanan;
            this.servisHRC = servisHRC;
            this.belge = belge;
        }

        public abstract class KonaklamaKatsayi
        {
            public abstract KonaklamaGunHesabi GunleriBul(ENUMYollukGundelikTuru belgeTur);
        }

        private class KonaklamaKatsayiDevlet28 : KonaklamaKatsayi
        {
            public override KonaklamaGunHesabi GunleriBul(ENUMYollukGundelikTuru gundelikTuru)
            {
                KonaklamaGunHesabi kgh = new KonaklamaGunHesabi();
                switch (gundelikTuru)
                {
                    case ENUMYollukGundelikTuru.SUREKLIGECICI:
                    case ENUMYollukGundelikTuru.TEDAVIYOLLUGU:
                    case ENUMYollukGundelikTuru.KOVUSTURMA:
                    case ENUMYollukGundelikTuru.ANKETOR:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(10, (decimal)2.8));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(80, (decimal)0.95));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(90, (decimal)0.75));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)0));
                        break;
                    case ENUMYollukGundelikTuru.DENETIMELEMANI:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)2.8));
                        break;
                    case ENUMYollukGundelikTuru.GECICIYURTDISI:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(10, (decimal)1.5));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)0));
                        break;
                }
                return kgh;
            }

            public static string Ad { get { return "10 gün 2,8 kat artırımlı"; } }
        }

        private class KonaklamaKatsayiDevlet19 : KonaklamaKatsayi
        {
            public override KonaklamaGunHesabi GunleriBul(ENUMYollukGundelikTuru gundelikTuru)
            {
                KonaklamaGunHesabi kgh = new KonaklamaGunHesabi();
                switch (gundelikTuru)
                {
                    case ENUMYollukGundelikTuru.SUREKLIGECICI:
                    case ENUMYollukGundelikTuru.TEDAVIYOLLUGU:
                    case ENUMYollukGundelikTuru.KOVUSTURMA:
                    case ENUMYollukGundelikTuru.ANKETOR:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(10, (decimal)1.6));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(80, (decimal)0.5));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(90, (decimal)0.4));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)0));
                        break;
                    case ENUMYollukGundelikTuru.DENETIMELEMANI:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)1.9));
                        break;
                    case ENUMYollukGundelikTuru.GECICIYURTDISI:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(10, (decimal)1.5));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)0));
                        break;
                }
                return kgh;
            }

            public static string Ad { get { return "10 gün 1,9 kat artırımlı"; } }
        }

        private class KonaklamaKatsayiDevlet16 : KonaklamaKatsayi
        {
            public override KonaklamaGunHesabi GunleriBul(ENUMYollukGundelikTuru gundelikTuru)
            {
                KonaklamaGunHesabi kgh = new KonaklamaGunHesabi();
                switch (gundelikTuru)
                {
                    case ENUMYollukGundelikTuru.SUREKLIGECICI:
                    case ENUMYollukGundelikTuru.TEDAVIYOLLUGU:
                    case ENUMYollukGundelikTuru.KOVUSTURMA:
                    case ENUMYollukGundelikTuru.ANKETOR:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(10, (decimal)1.6));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(80, (decimal)0.5));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(90, (decimal)0.4));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)0));
                        break;
                    case ENUMYollukGundelikTuru.DENETIMELEMANI:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)1.6));
                        break;
                    case ENUMYollukGundelikTuru.GECICIYURTDISI:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(10, (decimal)1.5));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)0));
                        break;
                }
                return kgh;
            }

            public static string Ad { get { return "10 gün 1,6 kat artırımlı"; } }
        }

        private class KonaklamaKatsayiDevlet : KonaklamaKatsayi
        {
            public override KonaklamaGunHesabi GunleriBul(ENUMYollukGundelikTuru gundelikTuru)
            {
                KonaklamaGunHesabi kgh = new KonaklamaGunHesabi();
                switch (gundelikTuru)
                {
                    case ENUMYollukGundelikTuru.SUREKLIGECICI:
                    case ENUMYollukGundelikTuru.TEDAVIYOLLUGU:
                    case ENUMYollukGundelikTuru.KOVUSTURMA:
                    case ENUMYollukGundelikTuru.ANKETOR:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(10, (decimal)1.5));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(80, (decimal)0.5));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(90, (decimal)0.4));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)0));
                        break;
                    case ENUMYollukGundelikTuru.DENETIMELEMANI:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)1.5));
                        break;
                    case ENUMYollukGundelikTuru.GECICIYURTDISI:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(10, (decimal)1.5));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)0));
                        break;
                }
                return kgh;
            }

            public static string Ad { get { return "10 gün 1,5 kat artırımlı"; } }
        }

        private class KonaklamaKatsayiCum3 : KonaklamaKatsayi
        {
            public static string Ad { get { return "Tam konaklama, 3 kat artırımlı"; } }

            public override KonaklamaGunHesabi GunleriBul(ENUMYollukGundelikTuru gundelikTuru)
            {
                KonaklamaGunHesabi kgh = new KonaklamaGunHesabi();
                switch (gundelikTuru)
                {
                    case ENUMYollukGundelikTuru.SUREKLIGECICI:
                    case ENUMYollukGundelikTuru.TEDAVIYOLLUGU:
                    case ENUMYollukGundelikTuru.KOVUSTURMA:
                    case ENUMYollukGundelikTuru.ANKETOR:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)3));
                        break;
                    case ENUMYollukGundelikTuru.DENETIMELEMANI:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)1.5));
                        break;
                    case ENUMYollukGundelikTuru.GECICIYURTDISI:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(10, (decimal)1.5));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)0));
                        break;
                }
                return kgh;
            }
        }

        private class KonaklamaKatsayiCum5 : KonaklamaKatsayi
        {
            public static string Ad { get { return "Tam konaklama, 5 kat artırımlı"; } }

            public override KonaklamaGunHesabi GunleriBul(ENUMYollukGundelikTuru gundelikTuru)
            {
                KonaklamaGunHesabi kgh = new KonaklamaGunHesabi();
                switch (gundelikTuru)
                {
                    case ENUMYollukGundelikTuru.SUREKLIGECICI:
                    case ENUMYollukGundelikTuru.TEDAVIYOLLUGU:
                    case ENUMYollukGundelikTuru.KOVUSTURMA:
                    case ENUMYollukGundelikTuru.ANKETOR:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)5));
                        break;
                    case ENUMYollukGundelikTuru.DENETIMELEMANI:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)1.5));
                        break;
                    case ENUMYollukGundelikTuru.GECICIYURTDISI:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(10, (decimal)1.5));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)0));
                        break;
                }
                return kgh;
            }
        }

        private class KonaklamaKatsayiDenetim175 : KonaklamaKatsayi
        {
            public override KonaklamaGunHesabi GunleriBul(ENUMYollukGundelikTuru gundelikTuru)
            {
                KonaklamaGunHesabi kgh = new KonaklamaGunHesabi();
                switch (gundelikTuru)
                {
                    case ENUMYollukGundelikTuru.SUREKLIGECICI:
                    case ENUMYollukGundelikTuru.TEDAVIYOLLUGU:
                    case ENUMYollukGundelikTuru.KOVUSTURMA:
                    case ENUMYollukGundelikTuru.ANKETOR:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(10, (decimal)1.6));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(80, (decimal)0.5));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(90, (decimal)0.4));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)0));
                        break;
                    case ENUMYollukGundelikTuru.DENETIMELEMANI:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)1.75));
                        break;
                    case ENUMYollukGundelikTuru.GECICIYURTDISI:
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(10, (decimal)1.5));
                        kgh.konaklamaGunDegerleri.Add(new KonaklamaGunu(-1, (decimal)0));
                        break;
                }
                return kgh;
            }

            public static string Ad { get { return "Denetim elemanı 1.75"; } }
        }

    }
}
