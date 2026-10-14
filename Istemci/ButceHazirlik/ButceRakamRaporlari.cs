using System;
using System.Collections.Generic;
using System.Text.Json;
using ButceHazirlikv2;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;
using TNS.UZY;

namespace Istemci.ButceHazirlik
{
    /// <summary>
    /// Bütçe rakam ekranının "Rapor Al" (gelişmiş rapor) penceresi: eski ButceHazirlikv2/ButceRakamDuzenleYeni.aspx.cs'teki
    /// btnRaporAlGelismis_Click, ExceleYazListe, ExceleYazUcgen ve yardımcıları olduğu gibi alındı.
    /// </summary>
    public class ButceRakamRaporlari
    {
        public class AlanTanim : IComparable
        {
            public int alanKod { get; set; }
            public string alanAd { get; set; }
            public int kirSay { get; set; }
            public int sira { get; set; }
            public int yuzde { get; set; }
            public String yuzdeAd
            {
                get
                {
                    if (yuzde == 1) return "Oran Baş";
                    if (yuzde == 2) return "Oran Son";
                    if (yuzde == 3) return "Fark Baş";
                    if (yuzde == 4) return "Fark Son";
                    return "-";
                }
                set
                {
                    if (value == "Oran Baş") yuzde = 1;
                    else if (value == "Oran Son") yuzde = 2;
                    else if (value == "Fark Baş") yuzde = 3;
                    else if (value == "Fark Son") yuzde = 4;
                    else yuzde = 0;
                }
            }

            public AlanTanim()
            {
                this.alanKod = 0;
                this.alanAd = "";
                this.kirSay = 0;
                this.sira = 0;
                this.yuzde = 0;
            }

            public AlanTanim(int alanKod, string alanAd, AlanTanim at, int kirSay, int sira)
            {
                this.alanKod = alanKod;
                this.alanAd = alanAd;
                if (at != null)
                {
                    this.kirSay = at.kirSay;
                    this.sira = at.sira;
                    this.yuzde = at.yuzde;
                }
                else
                {
                    this.kirSay = kirSay;
                    this.sira = sira;
                    this.yuzde = 0;
                }
            }

            public int CompareTo(object obj)
            {
                if (typeof(AlanTanim) != obj.GetType())
                    return 0;
                AlanTanim gelen = (AlanTanim)obj;
                int fark = this.sira.CompareTo(gelen.sira);
                if (fark == 0)
                    fark = this.alanKod.CompareTo(gelen.alanKod);
                return fark;
            }
        }

        public class GelismisRaporPar
        {
            public GelismisRaporPar()
            {
                this.araToplam = false;
                this.aciklamaOlsun = false;
                this.ucgen = true;
                this.OEYeni = true;
                this.OEOnayli = true;
                this.OEYevmiyeli = true;
            }

            public bool araToplam { get; set; }
            public bool aciklamaOlsun { get; set; }
            public bool ucgen { get; set; }
            public bool OEYeni { get; set; }
            public bool OEOnayli { get; set; }
            public bool OEYevmiyeli { get; set; }

            public AlanTanim[] alanlar { get; set; }
            public AlanTanim[] degerler { get; set; }

            public AlanTanim AlanBul(int alanKod)
            {
                if (alanlar != null)
                    foreach (var item in alanlar)
                        if (item.alanKod == alanKod)
                            return item;
                return null;
            }

            public AlanTanim DegerBul(int alanKod)
            {
                if (degerler != null)
                    foreach (var item in degerler)
                        if (item.alanKod == alanKod)
                            return item;
                return null;
            }
        }

        private static readonly JsonSerializerOptions jsonAyar = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private readonly Kullanici kullanan;
        private readonly ENUMEkKolonButceTipi odenekTur;
        private readonly string raporSablonYol;
        private readonly IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();

        public ButceRakamRaporlari(Kullanici kullanan, ENUMEkKolonButceTipi odenekTur, string raporSablonYol)
        {
            this.kullanan = kullanan;
            this.odenekTur = odenekTur;
            this.raporSablonYol = raporSablonYol;
        }

        /// <summary>Eski GelismisRaporAyarla: pencerenin alan ve değer listeleri, kullanıcının son seçimleriyle.</summary>
        public static object SecimOku(Kullanici kullanan, ENUMEkKolonButceTipi tur)
        {
            GelismisRaporPar grp = SecimOkuPar(kullanan, tur);
            List<AlanTanim> al = new List<AlanTanim>();
            if (tur == ENUMEkKolonButceTipi.GELIR)
            {
                al.Add(new AlanTanim(1, "Kurumsal", grp.AlanBul(1), 0, 1));
                al.Add(new AlanTanim(5, "Ekonomik", grp.AlanBul(5), 4, 1));
                al.Add(new AlanTanim(14, "Alt Faaliyet", grp.AlanBul(14), 0, 1));
            }
            else
            {
                al.Add(new AlanTanim(1, "Kurumsal", grp.AlanBul(1), 0, 1));
                al.Add(new AlanTanim(2, "Fonksiyonel", grp.AlanBul(2), 0, 1));
                al.Add(new AlanTanim(3, "Fin. Tipi", grp.AlanBul(3), 0, 1));
                al.Add(new AlanTanim(4, "Ekonomik", grp.AlanBul(4), 4, 1));
                al.Add(new AlanTanim(14, "Alt Faaliyet", grp.AlanBul(14), 0, 1));
            }
            List<object> degerler = new List<object>();
            for (int i = 0; i <= 12; i++)
            {
                AlanTanim d = grp.DegerBul(i);
                degerler.Add(new { alanKod = i, secili = d != null, yuzde = d != null ? d.yuzde : 0 });
            }
            return new
            {
                araToplam = grp.araToplam,
                aciklamaOlsun = grp.aciklamaOlsun,
                ucgen = grp.ucgen,
                OEYeni = grp.OEYeni,
                OEOnayli = grp.OEOnayli,
                OEYevmiyeli = grp.OEYevmiyeli,
                alanlar = al.ConvertAll(a => new { a.alanKod, a.alanAd, a.kirSay, a.sira }),
                degerler
            };
        }

        private static GelismisRaporPar SecimOkuPar(Kullanici kullanan, ENUMEkKolonButceTipi tur)
        {
            string tut = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ButceRaporAl" + ((int)tur).ToString());
            GelismisRaporPar gpr = null;
            if (!string.IsNullOrWhiteSpace(tut))
            {
                try { gpr = JsonSerializer.Deserialize<GelismisRaporPar>(tut, jsonAyar); }
                catch (Exception) { gpr = null; }
            }
            return gpr ?? new GelismisRaporPar();
        }

        private void SecimSakla(ENUMEkKolonButceTipi tur, GelismisRaporPar grp)
        {
            string tut = JsonSerializer.Serialize(grp);
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "ButceRaporAl" + ((int)tur).ToString(), tut);
        }

        /// <summary>Eski btnRaporAlGelismis_Click; üretilen Excel dosyasının yolunu döndürür.</summary>
        public string RaporAl(GelismisRaporPar grp, int yil, int ay, string senaryoKod, string kur, string fon, string fin, string eko, string peb)
        {
            Tertip kistasTertip = new Tertip(yil, odenekTur == ENUMEkKolonButceTipi.GELIR ? ENUMTabloKod.TABPEBGelir : ENUMTabloKod.TABPEBOdenek);
            if (odenekTur == ENUMEkKolonButceTipi.GELIR)
                kistasTertip.TertipAta(TNS.BHMv2.Arac.TiredenKodBul(kur) + "-" + TNS.BHMv2.Arac.TiredenKodBul(eko) + "-" + TNS.BHMv2.Arac.TiredenKodBul(peb));
            else
                kistasTertip.TertipAta(TNS.BHMv2.Arac.TiredenKodBul(kur) + "-" + TNS.BHMv2.Arac.TiredenKodBul(fon) + "-" + TNS.BHMv2.Arac.TiredenKodBul(fin) + "-" + TNS.BHMv2.Arac.TiredenKodBul(eko) + "-" + TNS.BHMv2.Arac.TiredenKodBul(peb));
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();

            string harcamaOnay = "";
            harcamaOnay = StrEkle(harcamaOnay, ",", grp.OEYeni ? "1,2" : "");
            harcamaOnay = StrEkle(harcamaOnay, ",", grp.OEOnayli ? "5" : "");
            harcamaOnay = StrEkle(harcamaOnay, ",", grp.OEYevmiyeli ? "6" : "");

            if (grp.alanlar == null) grp.alanlar = new AlanTanim[0];
            if (grp.degerler == null) grp.degerler = new AlanTanim[0];
            Array.Sort(grp.alanlar);
            Array.Sort(grp.degerler);
            for (int i = 0; i < grp.alanlar.Length; i++)
            {
                if (grp.alanlar[i].kirSay > 0)
                    kodBilgileri.Add(new UcgenKodBilgi(grp.alanlar[i].alanKod, grp.alanlar[i].kirSay));
            }

            List<EkDegerParam> al = new List<EkDegerParam>();
            int[] kolonlar = new int[13];
            int[] yuzdeler = new int[13];
            for (int i = 0; i < yuzdeler.Length; i++)
            {
                kolonlar[i] = -1;
                yuzdeler[i] = -1;
            }

            int aciklamaKolon = -1;
            for (int i = 0; i < grp.degerler.Length; i++)
            {
                EkDegerParam ep = null;
                int ak = grp.degerler[i].alanKod;
                if (ak == 0)
                    ep = new EkDegerParam(TabloKodOl(ENUMTabloKod.TABPEBOdenek), -3, 0, "06.00");
                else if (ak == 1)
                    ep = new EkDegerParam(TabloKodOl(ENUMTabloKod.TABButce), -3, 0, "06.00");
                else if (ak == 2)
                    ep = new EkDegerParam(TabloKodOl(ENUMTabloKod.TABPEBHarcama), -3, 0, "12;" + harcamaOnay);
                else if (ak == 3)
                    ep = new EkDegerParam(TabloKodOl(ENUMTabloKod.TABPEBOdenek), -2, 0, "06.00");
                else if (ak == 4)
                    ep = new EkDegerParam(TabloKodOl(ENUMTabloKod.TABButce), -2, 0, "06.00");
                else if (ak == 5)
                    ep = new EkDegerParam(TabloKodOl(ENUMTabloKod.TABPEBHarcama), -2, 0, "12;" + harcamaOnay);
                else if (ak == 6)
                    ep = new EkDegerParam(TabloKodOl(ENUMTabloKod.TABPEBOdenek), -1, 0, "06.00");
                else if (ak == 7)
                    ep = new EkDegerParam(TabloKodOl(ENUMTabloKod.TABButce), -1, 0, "06.00");
                else if (ak == 8)
                    ep = new EkDegerParam(TabloKodOl(ENUMTabloKod.TABPEBHarcama), -1, 0, ay.ToString() + ";" + harcamaOnay);
                else if (ak == 9)
                    ep = new EkDegerParam(TabloKodOl(ENUMTabloKod.TABHarcamaTahmin), -1, 0, senaryoKod);
                else if (ak == 10)
                {
                    ep = new EkDegerParam(TabloKodOl(ENUMTabloKod.TABPEBOdenek), 0, 0, senaryoKod);
                    if (grp.aciklamaOlsun)
                        aciklamaKolon = i;
                }
                else if (ak == 11)
                    ep = new EkDegerParam(TabloKodOl(ENUMTabloKod.TABPEBOdenek), 0, 1, senaryoKod);
                else if (ak == 12)
                    ep = new EkDegerParam(TabloKodOl(ENUMTabloKod.TABPEBOdenek), 0, 2, senaryoKod);

                if (ep != null)
                {
                    kolonlar[al.Count] = ak;
                    yuzdeler[al.Count] = grp.degerler[i].yuzde;
                    al.Add(ep);
                }
            }

            EkDegerParam[] ekBilgiler = al.ToArray();
            SecimSakla(odenekTur, grp);
            if (grp.ucgen)
                return ExceleYazUcgen(kistasTertip, kodBilgileri, ekBilgiler, ay, senaryoKod, grp.araToplam, aciklamaKolon, kolonlar, yuzdeler);
            else
                return ExceleYazListe(kistasTertip, kodBilgileri, ekBilgiler, ay, senaryoKod, grp.araToplam, aciklamaKolon, kolonlar, yuzdeler);
        }

        private static string StrEkle(string on, string ayrac, string ek)
        {
            if (string.IsNullOrEmpty(ek)) return on;
            if (string.IsNullOrEmpty(on)) return ek;
            return on + ayrac + ek;
        }

        private ENUMTabloKod TabloKodOl(ENUMTabloKod tabloKod)
        {
            if (odenekTur == ENUMEkKolonButceTipi.GELIR)
            {
                if (tabloKod == ENUMTabloKod.TABPEBOdenek) return ENUMTabloKod.TABPEBGelir;
                if (tabloKod == ENUMTabloKod.TABPEBHarcama) return ENUMTabloKod.TABPEBGelirGerceklesmesi;
                if (tabloKod == ENUMTabloKod.TABHarcamaTahmin) return ENUMTabloKod.TABGelirTahmini;
                if (tabloKod == ENUMTabloKod.TABButce) return ENUMTabloKod.TABButceGelir;
            }
            return tabloKod;
        }

        /// <summary>Eski Resources.ButceHazirlikv2 KODADnnn metinleri.</summary>
        private static string KodAd(int kodNo)
        {
            switch (kodNo)
            {
                case 1: return "Kurumsal";
                case 2: return "Fonksiyonel";
                case 3: return "Fin. Tipi";
                case 4: return "Ekonomik";
                case 5: return "Ekonomik";
                case 14: return "Alt Faaliyet";
                default: return null;
            }
        }

        private string ExceleYazListe(Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, int ay, string senaryoKod, bool araToplam, int aciklamaKolon, int[] kolonlar, int[] yuzdeler)
        {
            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            OdenekBilgi[] odenekBilgiler = servis.ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, true, false, true, aciklamaKolon);

            Tablo XLS = GenelIslemler.NewTablo();
            string sonucDosyaAd = DosyaIslem.DosyaAdUret("TertipListe", "") + ".xlsx";
            XLS.DosyaAc(this.raporSablonYol + "/BHMv2/ButceHazirlikListeRapor.xltx", sonucDosyaAd);
            GerceklesmeBaslikYaz(XLS, kistasTertip);
            int yil = kistasTertip.yil;
            XLS.HucreAdBulYaz("ButceYil", kistasTertip.yil);
            XLS.HucreAdBulYaz("KurumAd", uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kistasTertip.kodlar[0], true));
            XLS.HucreAdBulYaz("Asama", senaryoKod);
            XLS.HucreAdBulYaz("AyNo", ay);
            int baslaSatir = 0;
            int satir = 0;
            int sutun = 0;
            int fSatir = 0;
            int fSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            XLS.HucreAdAdresCoz("FormatSatir", ref fSatir, ref fSutun);
            baslaSatir = satir;
            int yukseklik = XLS.SatirYukseklikAl(satir);
            ToplamAl toplamAl = new ToplamAl(3, kolonlar.Length);
            for (int i = 0; i < 3; i++)
            {
                toplamAl.SatirSifirla(i);
            }

            bool araToplamOlabilir = false;
            int[] cevrim = new int[kodBilgileri.BoyutSayisi];
            for (int i = 0; i < kodBilgileri.BoyutSayisi; i++)
            {
                XLS.HucreDegerYaz(satir - 1, sutun + i, KodAd(kodBilgileri[i].kodNo));
                cevrim[i] = -1;

                if (kodBilgileri[i].kodNo == 4 && kodBilgileri[i].kirilimSay > 2)
                    araToplamOlabilir = true;
                for (int j = 0; j < kistasTertip.kodlar.Length; j++)
                {
                    if (kistasTertip.KodNolar(j) == kodBilgileri[i].kodNo)
                        cevrim[i] = j;
                }
                if (cevrim[i] == -1)
                    throw new Exception("Kod numarası bulunamadı!");
            }
            if (!araToplamOlabilir && araToplam) araToplam = false;

            SutunGenislikleriAyarla(XLS, kolonlar, yuzdeler, sutun + 6, 3, baslaSatir - 1);

            XLS.SutunGenislikAyarla(sutun + kodBilgileri.BoyutSayisi, sutun + 4, 0);

            Tertip geciciTertip = new Tertip(kistasTertip.yil, kistasTertip.tabloKod);

            string eskiGelen = null;
            int eskiSatir = -1;
            for (int i = 0; i < odenekBilgiler.Length; i++)
            {
                string gelen = "";
                bool ekle = true;
                OdenekBilgi ob = odenekBilgiler[i];
                string[] t = new string[cevrim.Length];
                for (int j = 0; j < cevrim.Length; j++)
                {
                    t[j] = ob.tertip.kodlar[cevrim[j]];
                    if (araToplam && ekle)
                    {
                        if (gelen != "") gelen += "-";
                        if (cevrim[j] == 3)
                        {
                            if (!string.IsNullOrWhiteSpace(t[j]) && t[j].Length > 4)
                                gelen += t[j].Substring(0, 4);
                            else
                                gelen += t[j];
                            ekle = false;
                        }
                        else
                        {
                            gelen += t[j];
                        }
                    }
                }
                if (araToplam)
                {
                    if (eskiGelen != gelen)
                    {
                        if (eskiSatir > -1)
                        {
                            AraYaz(uzyServis, XLS, eskiSatir, sutun, toplamAl, 1, eskiGelen, geciciTertip, fSatir, fSutun, cevrim);
                            toplamAl.SatirSifirla(1);
                        }
                        eskiSatir = satir;
                        satir++;
                        eskiGelen = gelen;
                    }
                }
                XLS.HucreKopyala(fSatir, fSutun, fSatir, fSutun + 40, satir, sutun);
                for (int j = 0; j < cevrim.Length; j++)
                {
                    XLS.HucreDegerYaz(satir, sutun + j, ob.tertip.kodlar[cevrim[j]]);
                }
                if (aciklamaKolon > -1)
                {
                    if (!string.IsNullOrWhiteSpace(ob.aciklama)) ob.aciklama = " (" + ob.aciklama + ")";
                }
                else
                    ob.aciklama = "";
                XLS.HucreDegerYaz(satir, sutun + 5, ob.tertip.TertipAdiKodAddan + ob.aciklama);
                for (int j = 0; j < ob.ekRakamlar.Count; j++)
                {
                    toplamAl.DegerAta(2, kolonlar[j], (double)ob.ekRakamlar[j]);
                }
                toplamAl.ToplamEkle(0, 2);
                toplamAl.ToplamEkle(1, 2);
                RakamlariYaz(XLS, satir, sutun + 6, toplamAl, 2);
                satir++;
            }
            if (eskiSatir > -1)
            {
                AraYaz(uzyServis, XLS, eskiSatir, sutun, toplamAl, 1, eskiGelen, geciciTertip, fSatir, fSutun, cevrim);
                toplamAl.SatirSifirla(1);
            }
            XLS.HucreKopyala(fSatir + 2, fSutun, fSatir + 2, fSutun + 40, satir, sutun);
            RakamlariYaz(XLS, satir, sutun + 6, toplamAl, 0);
            satir += 2;

            XLS.SatirYukseklikAyarla(baslaSatir, satir, -1);

            XLS.HucreKopyala(fSatir + 4, fSutun, fSatir + 5, fSutun + 40, satir, sutun);
            XLS.HucreDegerHtmlYaz(0, satir + 1, sutun, (HizmetGerekcesiOku(kistasTertip) ?? "").Replace("\r", "").Replace("\n", ""));
            XLS.SatirYukseklikAyarla(satir + 1, satir + 1, yukseklik * 6);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void GerceklesmeBaslikYaz(Tablo XLS, Tertip kistasTertip)
        {
            string baslik = "";
            if (kistasTertip.tabloKod == ENUMTabloKod.TABPEBOdenek)
                baslik = "Harcama";
            else
                baslik = "Gerçekleşme";
            XLS.HucreAdBulYaz("GerceklesmeAd", baslik);
        }

        private void SutunGenislikleriAyarla(Tablo XLS, int[] kolonlar, int[] yuzdeler, int basla, int kopyaSatirSay, int baslikSatir)
        {
            for (int i = 0; i < 13; i++)
            {
                int kolYer = Array.IndexOf(kolonlar, i);
                bool rakamVar = false;
                bool yuzdeVar = false;

                if (kolYer >= 0)
                {
                    rakamVar = true;
                    if (yuzdeler[kolYer] > 0) yuzdeVar = true;
                    if (i <= 1) yuzdeVar = false;
                }
                if (!rakamVar)
                    XLS.SutunGenislikAyarla(basla + 2 * i, basla + 2 * i, 0);
                if (!yuzdeVar)
                    XLS.SutunGenislikAyarla(basla + 2 * i + 1, basla + 2 * i + 1, 0);
                else
                {
                    string[] formuller = new string[] { "if({0}>0, {1}*100/{0},\"\")", "if({0}>0, ({1}-{0})*100/{0},\"\")", "{1}-{0}", "{0}-{1}" };
                    int adres2Sut = basla + 2 * i;
                    int adres1Sut = basla + 2 * i;
                    int formulNo;
                    int bilgiTur = yuzdeler[kolYer];
                    bool harcamaMi = HarcamaMi(i);
                    bool butceSonMu = ButceSonMu(i);

                    string baslik;
                    if (butceSonMu)
                    {
                        if (bilgiTur > 2) formulNo = 2;
                        else formulNo = 1;
                        adres1Sut -= 2;
                        if (bilgiTur <= 2)
                            baslik = "Artış %";
                        else
                            baslik = "Artış";
                    }
                    else if (harcamaMi)
                    {
                        if (bilgiTur > 2) formulNo = 3;
                        else formulNo = 0;
                        if (bilgiTur == 1 || bilgiTur == 3)
                            adres1Sut -= 4;
                        else
                            adres1Sut -= 2;
                        if (i == 9)
                            adres1Sut -= 2;
                        if (bilgiTur <= 2)
                            baslik = "Kullanım %";
                        else
                            baslik = "Kalan";
                    }
                    else
                    {
                        if (bilgiTur > 2) formulNo = 2;
                        else formulNo = 1;

                        if (i > 10)
                        {
                            adres1Sut -= 2;
                        }
                        else
                        {
                            if (i == 10) adres1Sut -= 2;
                            if (bilgiTur == 1 || bilgiTur == 3)
                                adres1Sut -= 6;
                            else
                                adres1Sut -= 4;
                        }
                        if (bilgiTur <= 2)
                            baslik = "Artış %";
                        else
                            baslik = "Artış veya Azalış";
                    }
                    XLS.HucreDegerYaz(baslikSatir, adres2Sut + 1, baslik);

                    int sutGen = XLS.SutunGenislikAl(adres2Sut);
                    for (int sat = 0; sat < kopyaSatirSay; sat++)
                    {
                        if (bilgiTur > 2)
                        {
                            XLS.HucreKopyala(sat, adres2Sut, sat, adres2Sut, sat, adres2Sut + 1);
                            XLS.SutunGenislikAyarla(adres2Sut + 1, adres2Sut + 1, sutGen, 0, sutGen);
                        }
                        string frml = string.Format(formuller[formulNo], GenelIslemler.ExcelAdres(adres1Sut, sat), GenelIslemler.ExcelAdres(adres2Sut, sat));
                        XLS.HucreFormulYaz(sat, adres2Sut + 1, "=" + frml);
                    }
                }
            }
        }

        private bool ButceSonMu(int i)
        {
            return (i == 1 || i == 4 || i == 7);
        }

        private bool HarcamaMi(int i)
        {
            return (i == 2 || i == 5 || i == 8 || i == 9);
        }

        private string ExceleYazUcgen(Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, int ay, string senaryoKod, bool araToplam, int aciklamaKolon, int[] kolonlar, int[] yuzdeler)
        {
            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            UcgenSatir[] odenekBilgiler = servis.UcgenTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, true, aciklamaKolon);

            Tablo XLS = GenelIslemler.NewTablo();
            string sonucDosyaAd = DosyaIslem.DosyaAdUret("TertipListe", "") + ".xlsx";
            XLS.DosyaAc(this.raporSablonYol + "/BHMv2/ButceHazirlikUcgenRapor.xltx", sonucDosyaAd);
            GerceklesmeBaslikYaz(XLS, kistasTertip);
            int yil = kistasTertip.yil;
            XLS.HucreAdBulYaz("ButceYil", kistasTertip.yil);
            XLS.HucreAdBulYaz("KurumAd", uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kistasTertip.kodlar[0], true));
            XLS.HucreAdBulYaz("Asama", senaryoKod);
            XLS.HucreAdBulYaz("AyNo", ay);
            int baslaSatir;
            int satir = 0;
            int sutun = 0;
            int fSatir = 0;
            int fSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            XLS.HucreAdAdresCoz("FormatSatir", ref fSatir, ref fSutun);
            baslaSatir = satir;

            ToplamAl toplamAl = new ToplamAl(3, kolonlar.Length);
            for (int i = 0; i < 3; i++)
            {
                toplamAl.SatirSifirla(i);
            }

            int[] cevrim = new int[kodBilgileri.KirilimSayisi];
            int sutunIslem = sutun;
            int toplamSay = 0;
            for (int i = 0; i < kodBilgileri.BoyutSayisi; i++)
            {
                XLS.HucreBirlestir(satir - 2, sutunIslem, satir - 2, sutunIslem + kodBilgileri[i].kirilimSay - 1);
                XLS.HucreDegerYaz(satir - 2, sutunIslem, KodAd(kodBilgileri[i].kodNo));
                for (int j = 0; j < kodBilgileri[i].kirilimSay; j++)
                {
                    int kaynak = 0;
                    if (kodBilgileri[i].kirilimSay == 1)
                        kaynak = 18;
                    else if (j == 0)
                        kaynak = 19;
                    else if (j == kodBilgileri[i].kirilimSay - 1)
                        kaynak = 21;
                    else
                        kaynak = 20;

                    for (int fsat = 0; fsat < 6; fsat++)
                    {
                        XLS.HucreKopyala(fSatir + fsat, kaynak, fSatir + fsat, kaynak, fSatir + fsat, sutunIslem);
                    }

                    if (j != 0)
                        cevrim[sutunIslem] = 0;
                    else
                        cevrim[sutunIslem] = i + 1;

                    XLS.HucreKopyala(fSatir + 0, kaynak, fSatir + 0, kaynak, satir - 1, sutunIslem);
                    XLS.HucreDegerYaz(satir - 1, sutunIslem++, RomaRakami(j + 1));
                }
            }

            XLS.SutunGenislikAyarla(sutunIslem, sutun + 21, 0);

            SutunGenislikleriAyarla(XLS, kolonlar, yuzdeler, sutun + 23, 7, baslaSatir - 2);

            for (int i = 0; i < odenekBilgiler.Length; i++)
            {
                UcgenSatir ob = odenekBilgiler[i];
                XLS.HucreKopyala(fSatir + cevrim[ob.yer], fSutun, fSatir + cevrim[ob.yer], fSutun + 55, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun + ob.yer, ob.kodParca);

                XLS.HucreDegerYaz(satir, sutun + 22, ob.aciklama);
                for (int j = 0; j < ob.ekDegerler.Count; j++)
                {
                    toplamAl.DegerAta(2, kolonlar[j], (double)ob.ekDegerler[j]);
                }
                if (ob.yer == 0)
                {
                    toplamSay++;
                    toplamAl.ToplamEkle(1, 2);
                }
                RakamlariYaz(XLS, satir, sutun + 23, toplamAl, 2);
                satir++;
            }

            if (toplamSay > 1)
            {
                XLS.HucreKopyala(fSatir + 6, fSutun, fSatir + 6, fSutun + 55, satir, sutun);
                RakamlariYaz(XLS, satir, sutun + 23, toplamAl, 1);
                satir += 2;
            }
            XLS.SatirYukseklikAyarla(baslaSatir, satir, -1);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string RomaRakami(int p)
        {
            if (p == 1) return "i";
            if (p == 2) return "ii";
            if (p == 3) return "iii";
            if (p == 4) return "iv";
            if (p == 5) return "v";
            if (p == 6) return "vi";
            return "";
        }

        private string HizmetGerekcesiOku(Tertip kistasTertip)
        {
            TNS.BHF.IBHFServis bhfServis = TNS.BHF.Arac.Tanimla();
            return bhfServis.FaaliyetGerekceOku(kullanan, kistasTertip.yil, kistasTertip.kodlar[0], 0).gerekce;
        }

        private void AraYaz(IUZYServis uzyServis, Tablo XLS, int satir, int sutun, ToplamAl toplamAl, int toplamAlSatir, string eskiGelen, Tertip t, int fSatir, int fSutun, int[] cevrim)
        {
            string[] kodlar = eskiGelen.Split('-');
            for (int i = 0; i < kodlar.Length; i++)
            {
                t.kodlar[cevrim[i]] = kodlar[i];
            }
            t.AdlariBul(uzyServis);
            XLS.HucreKopyala(fSatir + 1, fSutun, fSatir + 1, fSutun + 40, satir, sutun);
            for (int j = 0; j < cevrim.Length; j++)
            {
                XLS.HucreDegerYaz(satir, sutun + j, t.kodlar[cevrim[j]]);
            }

            XLS.HucreDegerYaz(satir, sutun + 5, t.TertipAdiKodAddan);
            RakamlariYaz(XLS, satir, sutun + 6, toplamAl, toplamAlSatir);
        }

        private void RakamlariYaz(Tablo XLS, int satir, int sutun, ToplamAl toplamAl, int toplamSatir)
        {
            for (int i = 0; i < toplamAl.SutunSay(); i++)
            {
                XLS.HucreDegerYaz(satir, sutun + i * 2, toplamAl.toplam[toplamSatir, i]);
            }
        }
    }
}
