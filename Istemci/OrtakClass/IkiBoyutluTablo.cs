using System;
using System.Collections;
using OrtakClass;
using TNS;

// Eski OrtakClass\ExcelRapor.cs dosyasından alındı: iki boyutlu (satırda ve sütunda boyut olan) Excel tablosu üretici.
namespace OrtakClass
{
    [Serializable()]
    public class IkiBoyutluTablo
    {
        public class SutunBilgi
        {
            public int KolNo;
            public object[] veriler = null;
            internal void Ekle(object veri, int diziSayisi)
            {
                if (veriler == null) veriler = new object[diziSayisi];
                double gelenVeri = 0;
                veriler[diziSayisi - 1] = veri;

                if (veri is double)
                {
                    gelenVeri = (double)veri;
                }
                else if (veri is int)
                {
                    gelenVeri = (int)veri;
                }
                else if (veri is decimal)
                {
                    gelenVeri = Convert.ToDouble((decimal)veri);
                }
                if (gelenVeri > 0)
                {
                    for (int i = 0; i < diziSayisi - 1; i++)
                    {
                        double tut;
                        if (veriler[i] == null)
                            tut = 0;
                        else
                            tut = (double)veriler[i];
                        veriler[i] = tut + gelenVeri;
                    }
                }
            }
        }

        public class SutunToplam
        {
            private int alanNo;
            public int toplamKol;
            private int baslaSut;
            private int bitisSut;
            public double toplam = 0;

            public SutunToplam(int alanNo, int toplamKol, int baslaSut, int bitisSut)
            {
                // TODO: Complete member initialization
                this.alanNo = alanNo;
                this.toplamKol = toplamKol;
                this.baslaSut = baslaSut;
                this.bitisSut = bitisSut;
            }

            internal bool KolonAraliktaMi(int kolon)
            {
                if (kolon >= baslaSut && kolon <= bitisSut || baslaSut == -1) return true;
                return false;
            }
        }

        public int soldaAlanSayisi { get; set; }
        public int ustteAlanSayisi { get; set; }
        public int ToplamAlanSayisi { get { return soldaAlanSayisi + ustteAlanSayisi; } }
        public string ustBaslik1 { get; set; }
        public string ustBaslik2 { get; set; }
        public string sutunlarBaslik { get; set; }
        public string satirlarBaslik { get; set; }
        public string hucreFormat { get; set; }
        public int veriGenislik { get; set; }
        public int satirYukseklik { get; set; }
        public string kosegenYazi { get; set; }
        public SayfaYonu sayfaYonu { get; set; }
        public bool ustBasliklarTekrarlasin { get; set; }
        public bool solBasliklarTekrarlasin { get; set; }
        public int yanyanaSayfaSayisi { get; set; }
        public int altaltaSayfaSayisi { get; set; }
        public System.Drawing.Color sutunBaslikRenk = System.Drawing.Color.Empty;
        public int veriYanasik = -1;
        public bool sonSutunSigdir = true;
        public string dipNot = null;
        public bool toplamGizle = false;

        public ArrayList veriler;
        private int sutunGenislik(int kolon)
        {
            if (soldaAlanSayisi == 1)
                return 37;
            return 19;
        }

        public IkiBoyutluTablo()
        {
            this.hucreFormat = "#,##0.00;[Red]-#,##0.00;#";
            veriGenislik = 11;
            satirYukseklik = 17;
            sayfaYonu = SayfaYonu.DUSEY;
            ustBasliklarTekrarlasin = true;
            solBasliklarTekrarlasin = true;
            yanyanaSayfaSayisi = 1;
            altaltaSayfaSayisi = 0;
        }

        public static string TabloUret(IkiBoyutluTablo tabloYapi, string uzanti, string sonucDosyaAd)
        {
            if (tabloYapi == null || tabloYapi.veriler == null)
                throw new Exception("Veri yapısı doldurulmamış!");
            if (tabloYapi.soldaAlanSayisi <= 0 || tabloYapi.ustteAlanSayisi <= 0)
                throw new Exception("Matrisin sol ve üst alan sayıları verilmemiş!");
            if (string.IsNullOrWhiteSpace(sonucDosyaAd))
                sonucDosyaAd = DosyaIslem.DosyaAdUret("TU", "") + "." + uzanti;
            Tablo XLS = new TabloAspose();
            XLS.BosDosyaAc(sonucDosyaAd);

            int formatSatir = 0;

            int satir = tabloYapi.soldaAlanSayisi + 5;
            int sutun = 0;
            XLS.DosyaSaklamaFormatAta(uzanti);
            XLS.HucreDegerYaz(satir++, sutun, tabloYapi.ustBaslik1);
            XLS.HucreDegerYaz(satir++, sutun, tabloYapi.ustBaslik2);

            XLS.KoyuYap(satir - 1, sutun, true);
            XLS.YatayHizala(satir - 1, sutun, 2);
            XLS.DuseyHizala(satir - 1, sutun, 2);
            XLS.YaziTipBuyuklugu(satir - 1, sutun, 10);
            XLS.HucreMetniKaydir(satir - 1, sutun, true);

            XLS.KoyuYap(satir - 2, sutun, true);
            XLS.YatayHizala(satir - 2, sutun, 2);
            XLS.DuseyHizala(satir - 2, sutun, 2);
            XLS.YaziTipBuyuklugu(satir - 2, sutun, 12);
            XLS.HucreMetniKaydir(satir - 2, sutun, true);

            satir++;

            int yuks = tabloYapi.satirYukseklik * 175 / 100;
            int sonBaslikSatir = satir + tabloYapi.ustteAlanSayisi;
            XLS.SatirYukseklikAyarla(satir, sonBaslikSatir, yuks);

            satir++;
            int[] sonYazilan = new int[tabloYapi.ustteAlanSayisi];
            ArrayList sutunToplamlar = new ArrayList();
            int farkliAlan = 0;
            int col = tabloYapi.soldaAlanSayisi;
            SortedList sutunlar = SutunlariBul(tabloYapi);
            Boyutlar onceki = null;
            foreach (DictionaryEntry de in sutunlar)
            {
                Boyutlar boyut = (Boyutlar)de.Key;
                SutunBilgi sb = (SutunBilgi)de.Value;
                farkliAlan = DegisimIslem(onceki, boyut, XLS, satir, sutun, ref col, sonYazilan, sutunToplamlar, formatSatir, tabloYapi);

                if (farkliAlan >= 0) //değişen var...
                {
                    sb.KolNo = sutun + col;
                    for (int i = farkliAlan; i < boyut.Count; i++)
                    {
                        sonYazilan[i] = sb.KolNo;
                        SutunFormatla(XLS, tabloYapi, satir + i, sb.KolNo, ((StrKodStrAd)boyut[i]).ad, -1, i);
                    }
                    ToplamHucreFormat(XLS, formatSatir, tabloYapi, col, -1);
                    col++;
                }
                onceki = boyut;
            }
            farkliAlan = DegisimIslem(onceki, null, XLS, satir, sutun, ref col, sonYazilan, sutunToplamlar, formatSatir, tabloYapi);
            ToplamYaz(XLS, 0, onceki.Count, satir, col, sonYazilan, sutunToplamlar, formatSatir, tabloYapi);
            for (int i = 0; i < tabloYapi.soldaAlanSayisi + 1; i++)
            {
                for (int j = 0; j < tabloYapi.soldaAlanSayisi; j++)
                {
                    SolBaslikAyarla(XLS, tabloYapi, formatSatir + i, sutun + j, i, j);
                }
            }
            XLS.SatirYukseklikAyarla(sonBaslikSatir, sonBaslikSatir, -1, 7, yuks);

            if (tabloYapi.ustBasliklarTekrarlasin)
                XLS.TekrarlanacakSatirlar(satir - 4, satir + tabloYapi.ustteAlanSayisi - 1);

            if (tabloYapi.solBasliklarTekrarlasin && tabloYapi.yanyanaSayfaSayisi != 1)
                XLS.TekrarlanacakSutunlar(sutun, sutun + tabloYapi.soldaAlanSayisi - 1);

            if (tabloYapi.yanyanaSayfaSayisi >= 0)
                XLS.YanyanaSayfaSayisi(tabloYapi.yanyanaSayfaSayisi);

            if (tabloYapi.altaltaSayfaSayisi >= 0)
                XLS.AltaltaSayfaSayisi(tabloYapi.altaltaSayfaSayisi);

            int sonKolon = col;

            XLS.HucreBirlestir(satir - 4, sutun, satir - 4, sonKolon);
            XLS.HucreBirlestir(satir - 3, sutun, satir - 3, sonKolon);
            XLS.SatirYukseklikAyarla(satir - 4, satir - 4, -1, 7, tabloYapi.satirYukseklik * 150 / 100);
            XLS.SatirYukseklikAyarla(satir - 3, satir - 3, -1, 7, tabloYapi.satirYukseklik * 125 / 100);

            XLS.CerceveCiz(satir - 1, satir, sutun + tabloYapi.soldaAlanSayisi, sonKolon + 1, LineStyle.THIN, TabloRenk.BLACK);
            XLS.DuseyCizgiCiz(satir - 1, satir, sonKolon + 1, LineStyle.THIN, TabloRenk.BLACK);
            XLS.SutunGenislikAyarla(sonKolon + 1, sonKolon + 1, 1);
            XLS.HucreBirlestir(satir - 1, sutun + tabloYapi.soldaAlanSayisi, satir - 1, sonKolon);
            XLS.YatayHizala(satir - 1, sutun + tabloYapi.soldaAlanSayisi, 2);
            XLS.DuseyHizala(satir - 1, sutun + tabloYapi.soldaAlanSayisi, 2);
            XLS.YaziTipBuyuklugu(satir - 1, sutun + tabloYapi.soldaAlanSayisi, 10);
            XLS.HucreMetniKaydir(satir - 1, sutun + tabloYapi.soldaAlanSayisi, true);
            XLS.HucreDegerYaz(satir - 1, sutun + tabloYapi.soldaAlanSayisi, tabloYapi.sutunlarBaslik);
            if (tabloYapi.sutunBaslikRenk != System.Drawing.Color.Empty)
            {
                XLS.ArkaPlanRenk(satir - 1, sutun, satir + tabloYapi.ustteAlanSayisi, sonKolon, System.Drawing.Color.FromArgb(247, 150, 70));
            }
            XLS.KoyuYap(satir - 1, sutun + tabloYapi.soldaAlanSayisi, true);

            XLS.HucreBirlestir(satir - 1, sutun, satir + tabloYapi.ustteAlanSayisi - 1, sutun + tabloYapi.soldaAlanSayisi - 1);

            XLS.CerceveCiz(satir - 1, satir + tabloYapi.ustteAlanSayisi, sutun, sutun + tabloYapi.soldaAlanSayisi, LineStyle.THIN, TabloRenk.BLACK);
            XLS.KoyuYap(satir - 1, sutun, true);
            XLS.YatayHizala(satir - 1, sutun, 2);
            XLS.DuseyHizala(satir - 1, sutun, 2);
            XLS.YaziTipBuyuklugu(satir - 1, sutun, 10);
            XLS.HucreMetniKaydir(satir - 1, sutun, true);
            XLS.HucreDegerYaz(satir - 1, sutun, tabloYapi.kosegenYazi);
            XLS.SatirGizle(0, satir - 6, true);

            satir += tabloYapi.ustteAlanSayisi;
            Boyutlar oncekiAnahtar = null;
            int[] satirYerler = new int[tabloYapi.soldaAlanSayisi];
            foreach (IkiBoyutluTabloVeri item in tabloYapi.veriler)
            {
                Boyutlar solAnahtar = new Boyutlar();
                Boyutlar ustAnahtar = new Boyutlar();
                for (int i = 0; i < tabloYapi.soldaAlanSayisi; i++)
                {
                    solAnahtar.Add(item.boyutlar[i]);
                }
                for (int i = tabloYapi.soldaAlanSayisi; i < item.boyutlar.Count; i++)
                {
                    ustAnahtar.Add(item.boyutlar[i]);
                }

                farkliAlan = FarkliAlanBul(oncekiAnahtar, solAnahtar);
                if (farkliAlan >= 0)
                {
                    SatirBilgiYaz(XLS, tabloYapi, item, farkliAlan, ref satir, sutun, solAnahtar, oncekiAnahtar, sutunlar, sutunToplamlar, satirYerler, formatSatir, sonKolon);
                }

                int sutunIndex = sutunlar.IndexOfKey(ustAnahtar);
                if (sutunIndex >= 0)
                {
                    SutunBilgi sb = (SutunBilgi)sutunlar.GetByIndex(sutunIndex);
                    sb.Ekle(item.veri, tabloYapi.soldaAlanSayisi + 1);
                }
                oncekiAnahtar = solAnahtar;
            }
            SatirBilgiYaz(XLS, tabloYapi, null, -1, ref satir, sutun, null, oncekiAnahtar, sutunlar, sutunToplamlar, satirYerler, formatSatir, sonKolon);
            if (!string.IsNullOrWhiteSpace(tabloYapi.dipNot))
            {
                satir++;
                XLS.HucreDegerYaz(satir, sutun, tabloYapi.dipNot);
                XLS.YaziTipBuyuklugu(satir, sutun, 7);
            }
            //foreach (IkiBoyutluTabloVeri item in tabloYapi.veriler)
            //{
            //    int kolNo = 0;
            //    for (int i = 0; i < item.boyutlar.Count; i++)
            //    {
            //        StrKodStrAd icitem = (StrKodStrAd)item.boyutlar[i];
            //        XLS.HucreDegerYaz(satir, sutun + kolNo++, icitem.kodAd);
            //    }
            //    if (item.veri is double)
            //    {
            //        XLS.HucreDegerYaz(satir, sutun + kolNo, (double)item.veri);
            //    }
            //    else if (item.veri is int)
            //    {
            //        XLS.HucreDegerYaz(satir, sutun + kolNo, (int)item.veri);
            //    }
            //    else if (item.veri is decimal)
            //    {
            //        XLS.HucreDegerYaz(satir, sutun + kolNo, (decimal)item.veri);
            //    }
            //    else if (item.veri is DateTime)
            //        XLS.HucreDegerYaz(satir, sutun + kolNo, (DateTime)item.veri);
            //    else if (item.veri is string)
            //        XLS.HucreDegerYaz(satir, sutun + kolNo, (string)item.veri);
            //    else if (item.veri != null)
            //        XLS.HucreDegerYaz(satir, sutun + kolNo, item.veri.ToString());
            //    kolNo++;
            //    satir++;
            //}
            XLS.SayfaYonuAta(tabloYapi.sayfaYonu);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private static void SolBaslikAyarla(Tablo XLS, IkiBoyutluTablo tabloYapi, int satir, int sutun, int toplamSatir, int kolon)
        {
            XLS.CerceveCiz(satir, satir + 1, sutun, sutun + 1, LineStyle.THIN, TabloRenk.BLACK);
            XLS.KoyuYap(satir, sutun, true);
            XLS.YaziTipBuyuklugu(satir, sutun, 9);
            XLS.YatayHizala(satir, sutun, 2);
            XLS.DuseyHizala(satir, sutun, 0);
            int gen = tabloYapi.sutunGenislik(kolon);
            XLS.SutunGenislikAyarla(sutun, sutun, gen);
            if (toplamSatir > 0)
            {
                XLS.ArkaPlanRenk(satir, sutun, RenkBul(toplamSatir));
            }
            if (kolon == tabloYapi.soldaAlanSayisi - 1 && tabloYapi.sonSutunSigdir)
            {
                XLS.HucreMetniSigdir(satir, sutun, true);
            }
            else
                XLS.HucreMetniKaydir(satir, sutun, true);
        }

        private static void SutunFormatla(Tablo XLS, IkiBoyutluTablo tabloYapi, int satir, int sutun, string metin, int toplamAlanNo, int farkliAlan)
        {
            if (farkliAlan == tabloYapi.ustteAlanSayisi - 1)
            {
                for (int j = 0; j < tabloYapi.ustteAlanSayisi; j++)
                {
                    XLS.CerceveCiz(satir - j, satir - j + 1, sutun, sutun + 1, LineStyle.THIN, TabloRenk.BLACK);
                }
            }
            else if (farkliAlan == -1)
            {
                for (int j = 0; j < tabloYapi.ustteAlanSayisi; j++)
                {
                    XLS.CerceveCiz((satir - toplamAlanNo) + j, (satir - toplamAlanNo) + j + 1, sutun, sutun + 1, LineStyle.THIN, TabloRenk.BLACK);
                }
            }
            XLS.HucreDegerYaz(satir, sutun, metin);
            XLS.KoyuYap(satir, sutun, true);
            XLS.YaziTipBuyuklugu(satir, sutun, 9);
            XLS.YatayHizala(satir, sutun, 2);
            XLS.DuseyHizala(satir, sutun, 2);
            XLS.HucreMetniKaydir(satir, sutun, true);
            int gen = tabloYapi.veriGenislik;
            if (toplamAlanNo > -1)
            {
                gen = gen * 125 / 100;
                if (tabloYapi.toplamGizle) gen = 0;
            }
            XLS.SutunGenislikAyarla(sutun, sutun, gen);
            if (toplamAlanNo >= 0)
            {
                int renkNo = (tabloYapi.ustteAlanSayisi - toplamAlanNo);
                XLS.ArkaPlanRenk(satir, sutun, RenkBul(renkNo));
            }
        }

        private static void SolBaslikYaz(Tablo XLS, IkiBoyutluTablo tabloYapi, int satir, int sutun, string metin, int kolon, bool toplamMi)
        {
            XLS.HucreDegerYaz(satir, sutun, metin);
            if (kolon == tabloYapi.soldaAlanSayisi - 1 || toplamMi)
            {
                //
                int yuk = tabloYapi.satirYukseklik;
                if (toplamMi)
                {
                    yuk = yuk * 125 / 100;
                    if (tabloYapi.toplamGizle) yuk = 0;
                }
                int minYuk = yuk;
                if (!toplamMi && !tabloYapi.sonSutunSigdir)
                {
                    yuk = -1;
                }
                XLS.SatirYukseklikAyarla(satir, satir, yuk, 7, minYuk);
            }
        }

        private static void ToplamHucreFormat(Tablo XLS, int formatSatir, IkiBoyutluTablo tabloYapi, int col, int toplamAlanNo)
        {
            for (int i = 0; i < tabloYapi.soldaAlanSayisi + 1; i++)
            {
                XLS.CerceveCiz(formatSatir + i, formatSatir + i + 1, col, col + 1, LineStyle.THIN, TabloRenk.BLACK);
                XLS.HucreFormatla(formatSatir + i, col, formatSatir + i, col, tabloYapi.hucreFormat);
                int renkNo = i;
                if (toplamAlanNo >= 0)
                {
                    renkNo += (tabloYapi.ustteAlanSayisi - toplamAlanNo);
                }
                XLS.ArkaPlanRenk(formatSatir + i, col, RenkBul(renkNo));
                if (toplamAlanNo >= 0 || i > 0)
                {
                    XLS.KoyuYap(formatSatir + i, col, true);
                }
                XLS.YaziTipBuyuklugu(formatSatir + i, col, 9);
                XLS.YatayHizala(formatSatir + i, col, 2);
                if (tabloYapi.veriYanasik != -1)
                    XLS.DuseyHizala(formatSatir + i, col, tabloYapi.veriYanasik);
                XLS.HucreMetniKaydir(formatSatir + i, col, true);
            }
        }

        private static System.Drawing.Color RenkBul(int i)
        {
            return System.Drawing.Color.FromArgb(255 - i * 10, 255 - i * 10, 255 - i * 10);
        }

        private static void SatirBilgiYaz(Tablo XLS, IkiBoyutluTablo tabloYapi, IkiBoyutluTabloVeri item, int farkliAlan, ref int satir, int sutun, Boyutlar solAnahtar, Boyutlar oncekiAnahtar, SortedList sutunlar, ArrayList sutunToplamlar, int[] satirYerler, int formatSatir, int sonKolon)
        {
            if (oncekiAnahtar != null)
            {
                for (int i = tabloYapi.soldaAlanSayisi - 1; i >= farkliAlan; i--)
                {
                    if (i != tabloYapi.soldaAlanSayisi - 1)
                    {
                        XLS.HucreKopyala(formatSatir + (tabloYapi.soldaAlanSayisi - i - 1), 0, formatSatir + (tabloYapi.soldaAlanSayisi - i - 1), sonKolon, satir, 0);
                        SolBaslikYaz(XLS, tabloYapi, satir, sutun + i + 1, "Toplam", i, true);
                        if (sutun + i + 1 < sutun + tabloYapi.soldaAlanSayisi - 1)
                            XLS.HucreBirlestir(satir, sutun + i + 1, satir, sutun + tabloYapi.soldaAlanSayisi - 1);
                        if (i >= 0)
                            XLS.HucreBirlestir(satirYerler[i], sutun + i, satir, sutun + i);
                    }
                    SatirYaz(XLS, ref satir, sutun, solAnahtar, i, sutunlar, sutunToplamlar);
                }
            }
            if (item != null)
            {
                XLS.HucreKopyala(formatSatir, 0, formatSatir, sonKolon, satir, 0);
                for (int i = tabloYapi.soldaAlanSayisi - 1; i >= farkliAlan; i--)
                {
                    satirYerler[i] = satir;
                    SolBaslikYaz(XLS, tabloYapi, satir, sutun + i, ((StrKodStrAd)item.boyutlar[i]).ad, i, false);
                }
            }
        }

        private static void SatirYaz(Tablo XLS, ref int satir, int sutun, Boyutlar solAnahtar, int degisenSutun, SortedList sutunlar, ArrayList sutunToplamlar)
        {
            ToplamlariSifirla(sutunToplamlar);
            foreach (DictionaryEntry de in sutunlar)
            {
                SutunBilgi sb = (SutunBilgi)de.Value;
                int kolNo = sb.KolNo;
                double gelenVeri = 0;
                object veri = (string)null;
                if (sb.veriler != null && sb.veriler.Length > degisenSutun + 1)
                {
                    veri = sb.veriler[degisenSutun + 1];
                    sb.veriler[degisenSutun + 1] = null;
                }
                if (veri is double)
                {
                    gelenVeri = (double)veri;
                    XLS.HucreDegerYaz(satir, sutun + kolNo, (double)veri);
                }
                else if (veri is int)
                {
                    gelenVeri = (int)veri;
                    XLS.HucreDegerYaz(satir, sutun + kolNo, (int)veri);
                }
                else if (veri is decimal)
                {
                    gelenVeri = Convert.ToDouble((decimal)veri);
                    XLS.HucreDegerYaz(satir, sutun + kolNo, (decimal)veri);
                }
                else if (veri is DateTime)
                    XLS.HucreDegerYaz(satir, sutun + kolNo, (DateTime)veri);
                else if (veri is string)
                    XLS.HucreDegerYaz(satir, sutun + kolNo, (string)veri);
                else if (veri != null)
                    XLS.HucreDegerYaz(satir, sutun + kolNo, veri.ToString());
                if (gelenVeri > 0)
                {
                    ToplamlaraEkle(sutunToplamlar, sutun + kolNo, gelenVeri, XLS, satir);
                }
            }
            //int kolNo = 0;
            //for (int i = 0; i < tabloYapi.soldaAlanSayisi; i++)
            //{
            //    StrKodStrAd icitem = (StrKodStrAd)item.boyutlar[i];
            //    XLS.HucreDegerYaz(satir, sutun + kolNo++, icitem.kodAd);
            //}

            //int sutunIndex = sutunlar.IndexOfKey(ustAnahtar);
            //if (sutunIndex >= 0)
            //{
            //}
            satir++;
        }

        private static void ToplamlariSifirla(ArrayList sutunToplamlar)
        {
            foreach (SutunToplam item in sutunToplamlar)
            {
                item.toplam = 0;
            }
        }

        private static void ToplamlaraEkle(ArrayList sutunToplamlar, int kolon, double gelenVeri, Tablo XLS, int satir)
        {
            foreach (SutunToplam item in sutunToplamlar)
            {
                if (item.KolonAraliktaMi(kolon))
                {
                    item.toplam += gelenVeri;
                    XLS.HucreDegerYaz(satir, item.toplamKol, item.toplam);
                }
            }
        }

        private static int DegisimIslem(Boyutlar onceki, Boyutlar boyut, Tablo XLS, int satir, int sutun, ref int col, int[] sonYazilan, ArrayList sutunToplamlar, int formatSatir, IkiBoyutluTablo tabloYapi)
        {
            if (onceki == null && boyut == null) return 0;
            int farkliAlan = FarkliAlanBul(onceki, boyut);

            if (farkliAlan >= 0) //değişen var...
            {
                if (onceki != null)
                {
                    if (farkliAlan < onceki.Count - 1)
                    {
                        for (int i = onceki.Count - 1; i > farkliAlan; i--)
                        {
                            int toplamKol = sutun + col++;
                            ToplamYaz(XLS, i, onceki.Count, satir, toplamKol, sonYazilan, sutunToplamlar, formatSatir, tabloYapi);
                        }
                    }
                }
            }
            return farkliAlan;
        }

        private static void ToplamYaz(Tablo XLS, int alanNo, int alanSay, int satir, int toplamKol, int[] sonYazilan, ArrayList sutunToplamlar, int formatSatir, IkiBoyutluTablo tabloYapi)
        {
            ToplamHucreFormat(XLS, formatSatir, tabloYapi, toplamKol, alanNo);
            SutunFormatla(XLS, tabloYapi, satir + alanNo, toplamKol, "Toplam", alanNo, -1);

            if (alanNo > 0)
            {
                sutunToplamlar.Add(new SutunToplam(alanNo, toplamKol, sonYazilan[alanNo - 1], toplamKol));
                XLS.HucreBirlestir(satir + alanNo - 1, sonYazilan[alanNo - 1], satir + alanNo - 1, toplamKol);
            }
            else
                sutunToplamlar.Add(new SutunToplam(alanNo, toplamKol, -1, -1));
            if ((alanSay - alanNo) > 1)
            {
                XLS.HucreBirlestir(satir + alanNo, toplamKol, satir + alanSay - 1, toplamKol);
            }
        }

        private static int FarkliAlanBul(Boyutlar onceki, Boyutlar simdiki)
        {
            if (onceki == null || simdiki == null) return 0;
            if (onceki.Count != simdiki.Count) return 0;
            for (int i = 0; i < simdiki.Count; i++)
            {
                StrKodStrAd ss = (StrKodStrAd)simdiki[i];
                StrKodStrAd os = (StrKodStrAd)onceki[i];
                if (ss.kod != os.kod) return i;
            }
            return -1;
        }

        private static System.Collections.SortedList SutunlariBul(IkiBoyutluTablo tabloYapi)
        {
            System.Collections.SortedList al = new System.Collections.SortedList();
            foreach (IkiBoyutluTabloVeri item in tabloYapi.veriler)
            {
                int veriSay = item.boyutlar.Count;
                if (veriSay != tabloYapi.ToplamAlanSayisi)
                    throw new Exception("Veri sayısı ile kolon sayısı uyumsuz!");
                Boyutlar ic = new Boyutlar();
                for (int i = tabloYapi.soldaAlanSayisi; i < veriSay; i++)
                {
                    ic.Add(item.boyutlar[i]);
                }
                YoksaEkle(al, ic);
            }
            return al;
        }

        private static void YoksaEkle(System.Collections.SortedList al, Boyutlar ic)
        {
            if (!al.ContainsKey(ic))
            {
                al.Add(ic, new SutunBilgi());
            }
        }
    }
}
