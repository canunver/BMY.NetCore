using System;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;

namespace ButceHazirlikv2
{
    /// <summary>
    /// Ek kolonlar gruplarının bütçe tipleri (eski EkKolon.cs). Bütçe Teklifi sayfasının odenekTur parametresi bu değerlerdir.
    /// </summary>
    public enum ENUMEkKolonButceTipi : int
    {
        /// <summary>Gider bütçesi için hazırlanmış kolon grubu</summary>
        GIDER = 1,

        /// <summary>Gider bütçesi (ekonomik eko 2) için hazırlanmış kolon grubu</summary>
        GIDEREKO2 = 3,

        /// <summary>Gider bütçesi (ekonomik eko 4) için hazırlanmış kolon grubu</summary>
        GIDEREKO4 = 4,

        /// <summary>Gelir bütçesi için hazırlanmış kolon grubu</summary>
        GELIR = 2,
    }

    /// <summary>Bütçe veri girişlerinde, kullanıcıya gösterilecek ek kolonların türleri</summary>
    public enum ENUMFormulTip : int
    {
        /// <summary>Veri tabanından ek değer bulup gösteren kolon tipi</summary>
        EkDeger = 1,

        /// <summary>Ek kolonlar arasındaki iki kolonun değişim oranı tipi</summary>
        DegisimOran = 2,
    }

    /// <summary>
    /// Bütçe veri giriş ekranlarında, gösterilecek olan ek kolonları tutan sınıf. Eski EkKolon.cs'in aynısıdır; Ext ve FarPoint
    /// tablosuna ekleme yordamları yerine sayfaya başlıklar gönderilir, kaynak dosyasındaki (ButceHazirlikv2.resx BHM006..BHM027)
    /// metinler yerinde yazılıdır.
    /// </summary>
    public class EkKolonlar
    {
        /// <summary>Bu kolon grubunun seçim ekranılarında gösterilecek olan adı.</summary>
        public string kolonGrupAdi;

        /// <summary>Bir grupta bulunan kolonların listesi</summary>
        private TNSCollection ekKolonlar;

        public EkKolonlar(string kolonGrupAdi)
        {
            this.kolonGrupAdi = kolonGrupAdi;
            ekKolonlar = new TNSCollection();
        }

        /// <summary>
        /// Kolon başlıklarında geçen yil-1, yil+1, yil, ay gibi alanları hesaplayıp sonucu döndüren fonksiyon
        /// </summary>
        private static string IfadeYerineKoy(string ifade, int yil, int ay)
        {
            int carpan;
            int rakam;
            int uzun = 0;
            if (ifade.StartsWith("ay"))
            {
                rakam = ay;
                uzun = 2;
            }
            else if (ifade.StartsWith("yil"))
            {
                rakam = yil;
                uzun = 3;
            }
            else
                return "";

            if (ifade.Length <= uzun) return rakam.ToString();
            if (ifade[uzun] == '-')
                carpan = -1;
            else
                carpan = 1;
            return (rakam + carpan * OrtakFonksiyonlar.ConvertToInt(ifade.Substring(uzun + 1), 0)).ToString();
        }

        /// <summary>Kolon başlığında @@formul@@ şeklinde yer alan tüm ifadeleri hesaplayıp değiştiren fonksiyon</summary>
        private string IfadeDegistir(string fmtString, int yil, int ay)
        {
            int ayracyer = fmtString.IndexOf("@@");
            if (ayracyer < 0) return fmtString;
            int ayrackapayer = fmtString.IndexOf("@@", ayracyer + 2);
            if (ayrackapayer < 0) return fmtString;
            return fmtString.Substring(0, ayracyer) + IfadeYerineKoy(fmtString.Substring(ayracyer + 2, ayrackapayer - ayracyer - 2), yil, ay) + IfadeDegistir(fmtString.Substring(ayrackapayer + 2), yil, ay);
        }

        /// <summary>Kolon grubuna, kolon ekleyen metod.</summary>
        public void Ekle(EkKolon ekkolon)
        {
            ekKolonlar.Add(ekkolon);
        }

        /// <summary>
        /// Kolonlar arasında EkDeğer türünde olanlar veri tabanında hesaplanır ve sadece bu kolonların karşılığını tutan bir dizi elde
        /// edilir. Bu dizi içerisinde yer alan bir kolonun, gerçek kolonlarda kaçıncı sırada olduğunu bulan fonksiyon
        /// </summary>
        private int GercekDiziDegeriniBul(int sanalDiziDegeri, int ekBilgiBasla)
        {
            int ekDegerKolonSay = 0;

            for (int i = 0; i < ekKolonlar.Count; i++)
            {
                if (((EkKolon)ekKolonlar[i]).formulTip == ENUMFormulTip.EkDeger)
                {
                    ekDegerKolonSay++;
                    if (ekDegerKolonSay == sanalDiziDegeri)
                        return i + ekBilgiBasla;
                }
            }

            return ekDegerKolonSay + ekBilgiBasla;
        }

        /// <summary>Kolonlardaki EkDeger türünde olan kolonları, bir dizi içerisinde döndüren metod.</summary>
        public EkDegerParam[] EkdegerParamDondur(ENUMEkKolonButceTipi butceTip, int ay, string senaryoKod)
        {
            int i = 0;
            int formultipsay = 0;

            for (i = 0; i < ekKolonlar.Count; i++)
            {
                if (((EkKolon)ekKolonlar[i]).formulTip == ENUMFormulTip.EkDeger) formultipsay++;
            }

            EkDegerParam[] ekdegerparamlar = new EkDegerParam[formultipsay];

            formultipsay = 0;
            for (i = 0; i < ekKolonlar.Count; i++)
            {
                EkKolon ekKolon = (EkKolon)ekKolonlar[i];
                if (ekKolon.formulTip == ENUMFormulTip.EkDeger)
                {
                    EkDegerParam ekP = ekKolon.ekDegerParam;
                    ENUMTabloKod tabloKod = ekP.TabloKod();
                    string senaryoKodVeyaAy = ekP.senaryoKodVeyaAy;
                    if (string.IsNullOrEmpty(senaryoKodVeyaAy))
                    {
                        if (tabloKod == ENUMTabloKod.TABHAZHarcama)
                        {
                            senaryoKodVeyaAy = ay.ToString();
                        }
                        else
                        {
                            senaryoKodVeyaAy = senaryoKod;
                        }
                    }
                    if (butceTip == ENUMEkKolonButceTipi.GELIR)
                    {
                        if (tabloKod == ENUMTabloKod.TABPEBOdenek) tabloKod = ENUMTabloKod.TABPEBGelir;
                        else if (tabloKod == ENUMTabloKod.TABHAZHarcama) tabloKod = ENUMTabloKod.TABGelirGerceklesmesi;
                        else if (tabloKod == ENUMTabloKod.TABHarcamaTahmin) tabloKod = ENUMTabloKod.TABGelirTahmini;
                    }
                    if (butceTip == ENUMEkKolonButceTipi.GIDER)
                    {
                        if (tabloKod == ENUMTabloKod.TABHAZHarcama) tabloKod = ENUMTabloKod.TABPEBHarcama;
                    }

                    ekdegerparamlar[formultipsay] = new EkDegerParam(tabloKod, ekP.butceYilFark, ekP.teklifYil, senaryoKodVeyaAy);
                    formultipsay++;
                }
            }

            return ekdegerparamlar;
        }

        /// <summary>Kolon oran (değişim) kolonu mu? Sayfa oranları yüzde, diğerlerini tutar olarak gösterir.</summary>
        public bool OranMi(int kolonNo)
        {
            return ((EkKolon)ekKolonlar[kolonNo]).formulTip != ENUMFormulTip.EkDeger;
        }

        internal string FormatBul(int kolonNo)
        {
            if (((EkKolon)ekKolonlar[kolonNo]).formulTip == ENUMFormulTip.EkDeger)
                return "#,##0";
            else
                return "%##0";
        }

        internal string StringeCevir(double deger, int kolonNo)
        {
            if (double.IsNaN(deger)) return "-";
            return deger.ToString(FormatBul(kolonNo));
        }

        /// <summary>
        /// Ekkolon listesindeki, ekParam türü olan kolonlar için bir "degerler" dizisinde verilen değerler kullanılarak, tüm kolonların
        /// değerleri hesaplanır.
        /// </summary>
        /// <param name="degerler">EkDeger türündeki kolonların değerleri</param>
        /// <param name="ekBilgiBasla">Gelen dizinin hangi elemanından itibaren ek bilgilerin başladığını gösterir. Dizinin önceki elemanları veri girişinde kullanılan değerlerdir.</param>
        public double[] Hesapla(double[] degerler, int ekBilgiBasla)
        {
            double[] hesaplananlar = new double[ekKolonlar.Count];

            int i = 0;
            int formultipsay = 0;

            for (i = 0; i < ekKolonlar.Count; i++)
            {
                if (((EkKolon)ekKolonlar[i]).formulTip == ENUMFormulTip.EkDeger) formultipsay++;
            }

            int sayac = 0;
            if (degerler.Length - ekBilgiBasla == formultipsay)
            {
                for (i = 0; i < ekKolonlar.Count; i++)
                {
                    if (((EkKolon)ekKolonlar[i]).formulTip == ENUMFormulTip.EkDeger)
                    {
                        hesaplananlar[i] = degerler[sayac + ekBilgiBasla];
                        sayac = sayac + 1;
                    }

                    if (((EkKolon)ekKolonlar[i]).formulTip == ENUMFormulTip.DegisimOran)
                    {
                        double x = degerler[GercekDiziDegeriniBul(((EkKolon)ekKolonlar[i]).bolen, ekBilgiBasla)];
                        double y = degerler[GercekDiziDegeriniBul(((EkKolon)ekKolonlar[i]).bolunen, ekBilgiBasla)];
                        if (y == 0) hesaplananlar[i] = double.NaN;
                        else
                        {
                            x = x - y;
                            hesaplananlar[i] = ((double)x / y);
                        }
                    }
                }
            }
            return hesaplananlar;
        }

        /// <summary>Kolon grubundaki kolon sayısını döndürür</summary>
        public int KolonSayisi()
        {
            return ekKolonlar.Count;
        }

        /// <summary>
        /// Verilen kolonun, verilen satırdaki başlık değerini döndürür. Başlıkta formül varsa yıl ve ay ile hesaplanır.
        /// </summary>
        public string KolonBasligiGetir(int kolonNo, int satirNo, int yil, int ay)
        {
            EkKolon ekKolon = (EkKolon)ekKolonlar[kolonNo];
            if (satirNo < 0)
                return IfadeDegistir(ekKolon.sutunBaslik[0], yil, ay) + "\r\n" + IfadeDegistir(ekKolon.sutunBaslik[1], yil, ay);
            else
                return IfadeDegistir(ekKolon.sutunBaslik[satirNo], yil, ay);
        }

        /// <summary>
        /// Excel'e ek kolon başlıklarını yazar (eski ExceleEkle). Birinci satır başlığı boş olan kolon solundakiyle birleştirilir.
        /// </summary>
        internal void ExceleEkle(Tablo XLS, int satir, int sutun, int yil, int ay)
        {
            int kolSay = KolonSayisi();
            int birlestirSay = 0;
            for (int i = 0; i < kolSay; i++)
            {
                string baslik = KolonBasligiGetir(i, 0, yil, ay);
                if (!string.IsNullOrEmpty(baslik))
                {
                    if (birlestirSay > 0) XLS.HucreBirlestir(satir, sutun - birlestirSay - 1, satir, sutun - 1);
                    XLS.HucreDegerYaz(satir, sutun, baslik);
                    birlestirSay = 0;
                }
                else
                    birlestirSay++;

                XLS.HucreDegerYaz(satir + 1, sutun++, KolonBasligiGetir(i, 1, yil, ay));
            }
            if (birlestirSay > 0) XLS.HucreBirlestir(satir, sutun - birlestirSay - 1, satir, sutun - 1);
        }

        /// <summary>Uygulamanın tanıdığı ek kolonların listesini tutan dizi.</summary>
        private static EkKolonlar[] _staticEkKolonlar = null;
        private static readonly object kilit = new object();

        /// <summary>Uygulamanın tanıdığı ek kolonların listesini tutan dizi.</summary>
        public static EkKolonlar[] staticEkKolonlar
        {
            get
            {
                if (_staticEkKolonlar == null)
                    lock (kilit)
                        if (_staticEkKolonlar == null) EkKolonlariHazirla();
                return _staticEkKolonlar;
            }
        }

        // Eski ButceHazirlikv2.resx metinleri
        private const string BHM006 = "Birim Teklifine Göre";
        private const string BHM007 = "Birim Teklifi";
        private const string BHM008 = "@@yil-3@@";
        private const string BHM009 = "@@yil-2@@";
        private const string BHM010 = "@@yil-1@@";
        private const string BHM011 = "Artış Oranı";
        private const string BHM012 = "(@@yil-3@@/@@yil-2@@)";
        private const string BHM013 = "(@@yil-2@@/@@yil-1@@)";
        private const string BHM014 = "Harcamaya Göre";
        private const string BHM015 = "Yıl Sonu Harcama";
        private const string BHM016 = "@@ay@@. Ay Harcama";
        private const string BHM017 = "Yıl Sonu Harcamaya Göre (@@yil-3@@/@@yil-2@@)";
        private const string BHM018 = "@@ay@@. Ay Harcamaya Göre (@@yil-2@@/@@yil-1@@)";
        private const string BHM019 = "Yıl Sonu Harcamaya Göre (@@yil-2@@/@@yil-1@@)";
        private const string BHM020 = "KBÖ'ye Göre";
        private const string BHM021 = "KBÖ";
        private const string BHM022 = "KBÖ/Yılsonu Harcama";
        private const string BHM023 = "Yıl Sonu Harcama Tahmini";
        private const string BHM024 = "KBÖ'ye Göre (@@yil-2@@/@@yil-1@@)";
        private const string BHM025 = "Yıl Sonu Harcamaya Göre (@@yil-3@@/@@yil-2@@)";
        private const string BHM026 = "Yıl Sonu Harcamaya Göre (@@yil-2@@/@@yil-1@@)";
        private const string BHM027 = "Detay Tablo";

        /// <summary>Tüm tanınan ek kolonlar, diziye doldurulur.</summary>
        private static void EkKolonlariHazirla()
        {
            EkKolonlar[] d = new EkKolonlar[6];

            d[0] = new EkKolonlar(BHM006);
            d[0].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 3, 0, ""), 0, 0, BHM007, BHM008));
            d[0].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 2, 0, ""), 0, 0, "", BHM009));
            d[0].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 1, 0, ""), 0, 0, "", BHM010));
            d[0].Ekle(new EkKolon(ENUMFormulTip.DegisimOran, null, 2, 1, BHM011, BHM012));
            d[0].Ekle(new EkKolon(ENUMFormulTip.DegisimOran, null, 3, 2, "", BHM013));

            d[1] = new EkKolonlar(BHM014);
            d[1].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABHAZHarcama, 3, 0, "12"), 0, 0, BHM008, BHM015));
            d[1].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABHAZHarcama, 2, 0, ""), 0, 0, BHM009, BHM016));
            d[1].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABHAZHarcama, 2, 0, "12"), 0, 0, "", BHM015));
            d[1].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABHAZHarcama, 1, 0, ""), 0, 0, BHM010, BHM016));
            d[1].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABHarcamaTahmin, 1, 0, ""), 0, 0, "", BHM023));
            d[1].Ekle(new EkKolon(ENUMFormulTip.DegisimOran, null, 3, 1, BHM011, BHM017));
            d[1].Ekle(new EkKolon(ENUMFormulTip.DegisimOran, null, 4, 2, "", BHM018));
            d[1].Ekle(new EkKolon(ENUMFormulTip.DegisimOran, null, 5, 3, "", BHM019));

            d[2] = new EkKolonlar(BHM020);
            d[2].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 3, 0, "06.00"), 0, 0, BHM021, BHM008));
            d[2].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 2, 0, "06.00"), 0, 0, "", BHM009));
            d[2].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 1, 0, "06.00"), 0, 0, "", BHM010));
            d[2].Ekle(new EkKolon(ENUMFormulTip.DegisimOran, null, 2, 1, BHM011, BHM012));
            d[2].Ekle(new EkKolon(ENUMFormulTip.DegisimOran, null, 3, 2, "", BHM013));

            d[3] = new EkKolonlar(BHM022);
            d[3].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 3, 0, "06.00"), 0, 0, BHM008, BHM021));
            d[3].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABHAZHarcama, 3, 0, "12"), 0, 0, "", BHM015));
            d[3].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 2, 0, "06.00"), 0, 0, BHM009, BHM021));
            d[3].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABHAZHarcama, 2, 0, "12"), 0, 0, "", BHM015));
            d[3].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 1, 0, "06.00"), 0, 0, BHM010, BHM021));
            d[3].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABHarcamaTahmin, 1, 0, ""), 0, 0, "", BHM023));
            d[3].Ekle(new EkKolon(ENUMFormulTip.DegisimOran, null, 5, 3, BHM011, BHM024));
            d[3].Ekle(new EkKolon(ENUMFormulTip.DegisimOran, null, 4, 2, "", BHM025));
            d[3].Ekle(new EkKolon(ENUMFormulTip.DegisimOran, null, 6, 4, "", BHM026));

            d[4] = new EkKolonlar(BHM027);
            d[4].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 3, 0, "06.00"), 0, 0, BHM008, BHM021));
            d[4].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABHAZHarcama, 3, 0, "12"), 0, 0, "", BHM015));
            d[4].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 2, 0, "06.00"), 0, 0, BHM009, BHM021));
            d[4].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABHAZHarcama, 2, 0, ""), 0, 0, "", BHM016));
            d[4].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABHAZHarcama, 2, 0, "12"), 0, 0, "", BHM015));
            d[4].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 1, 0, "06.00"), 0, 0, BHM010, BHM021));
            d[4].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABHAZHarcama, 1, 0, ""), 0, 0, "", BHM016));
            d[4].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABHarcamaTahmin, 1, 0, ""), 0, 0, "", BHM023));
            d[4].Ekle(new EkKolon(ENUMFormulTip.DegisimOran, null, 6, 3, BHM011, BHM024));
            d[4].Ekle(new EkKolon(ENUMFormulTip.DegisimOran, null, 7, 4, "", BHM018));
            d[4].Ekle(new EkKolon(ENUMFormulTip.DegisimOran, null, 8, 5, "", BHM019));

            d[5] = new EkKolonlar("Tavan Üstü Ödenekler");
            d[5].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABPEBTavanUstu, 0, 0, ""), 0, 0, "Tavan Üstü", "@@yil@@"));
            d[5].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABPEBTavanUstu, 0, 1, ""), 0, 0, "", "@@yil+1@@"));
            d[5].Ekle(new EkKolon(ENUMFormulTip.EkDeger, new EkDegerParam(ENUMTabloKod.TABPEBTavanUstu, 0, 2, ""), 0, 0, "", "@@yil+2@@"));

            _staticEkKolonlar = d;
        }
    }

    /// <summary>
    /// Bütçe veri giriş ekranlarında gösterilen ek kolonların her biri. İki tür formül tipi desteklenmektedir:
    ///     EkParam: Veri tabanından ilgili bilginin getirildiği kolon türü
    ///     ArtışOran: Gruptaki iki kolon arasındaki artış değerini hesaplar
    /// </summary>
    public class EkKolon
    {
        public ENUMFormulTip formulTip;
        public int bolen;
        public int bolunen;
        public string[] sutunBaslik;
        public EkDegerParam ekDegerParam;

        public EkKolon(ENUMFormulTip formulTip, EkDegerParam ekDegerParam, int bolen, int bolunen, params string[] sutunBaslik)
        {
            this.formulTip = formulTip;
            this.sutunBaslik = sutunBaslik;
            this.ekDegerParam = ekDegerParam;
            this.bolen = bolen;
            this.bolunen = bolunen;
        }
    }
}
