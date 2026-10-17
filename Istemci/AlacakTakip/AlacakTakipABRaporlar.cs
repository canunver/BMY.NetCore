using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Aspose.Words;
using OrtakClass;
using TNS;
using TNS.ATM;
using TNS.KYM;
using TNS.Raporlama;

namespace AlacakTakip
{
    /// <summary>
    /// AB (IPARD) Takip İşlemleri raporları (Rapor.xml grup 11000, NESNE="AlacakTakip.AlacakTakipABRaporlar"; ad alanı ve sınıf adı
    /// Rapor.xml değişmesin diye eskisiyle aynıdır). Eski AlacakTakip/AlacakTakipABRaporlar.cs'ten alındı; rapor kodu (Tablo / TabloAspose,
    /// Aspose.Words) aynıdır. Değişenler:
    ///   - TNS.Raporlama.IRapor arayüzü Core'da yok; RaporlamaServis RaporAl(object, object, string, string, string, Hashtable, string)
    ///     metodunu yansımayla (reflection) bulur. System.Web kaldırıldı.
    ///   - Eski AlacakTakipListeAB.DovizKuruBul (sayfa sınıfı) buraya alındı (servis.DovizKuruBulOncAy).
    ///   - raporParam değerleri (string) dönüşümü yerine P() ile okunur: uçlar sayı / bool koysa da hata vermez (değer aynı).
    ///   - Şablon RaporlamaServis'in verdiği yolda (kurum Ayar "RaporSablonYol" + Rapor.xml ŞABLON, ör. ATM\...) yoksa uygulamayla
    ///     gelen SGB.Net\RaporSablon\ATM klasörüne bakılır (SablonBul).
    /// </summary>
    public class AlacakTakipABRaporlar
    {
        public class HucreAdres
        {
            public string hucreAd;
            public int tip;

            public HucreAdres(string hucreAd, int tip)
            {
                this.hucreAd = hucreAd;
                this.tip = tip;
            }
        }
        Tablo XLS;
        Aspose.Words.Document doc;
        DocumentBuilder builder;

        public static HucreAdres[] HucreAdresler = new HucreAdres[] {
                new HucreAdres("defterSirasi", 4),      new HucreAdres("borcluAd", 4),             new HucreAdres("kimlikNo", 4),             new HucreAdres("basvuruKayitNumarasi", 4), new HucreAdres("sozlesmeNumarasi", 4),
                new HucreAdres("borcunKoduAd", 4),      new HucreAdres("borcunKodu", 4),           new HucreAdres("vakaNumarasi", 4),
                new HucreAdres("onBorcTarihi", 3),      new HucreAdres("onBorcDovizKuru", 2),
                new HucreAdres("bildirimTarihi", 3),    new HucreAdres("tesellumTarihi", 3),       new HucreAdres("temerrutTarihi", 3),       new HucreAdres("faizHesabiSonTarihi", 3),  new HucreAdres("islemTarihi", 3),
                new HucreAdres("geriAlimTarihi", 3),    new HucreAdres("kalanMiktarAB", 2),        new HucreAdres("kalanMiktarUlusal", 2),
                new HucreAdres("geriAlimMiktarAB", 2),  new HucreAdres("geriAlimMiktarUlusal", 2), new HucreAdres("geriAlimMiktarABEUR", 2),  new HucreAdres("geriAlimMiktarUlusalEUR", 2), new HucreAdres("geriAlimDovizKuru", 2),
                new HucreAdres("aySayisi", 1),          new HucreAdres("gunSayisi", 1),
                new HucreAdres("kalanMiktarAB", 2),     new HucreAdres("kalanFaizMiktarAB", 2),    new HucreAdres("kalanMiktarUlusal", 2),    new HucreAdres("kalanFaizMiktarUlusal",2), new HucreAdres("kalanMiktarToplam",2), new HucreAdres("kalanFaizMiktarToplam", 2),
                new HucreAdres("kalanMiktarABSil", 2),  new HucreAdres("kalanFaizMiktarABSil", 2), new HucreAdres("kalanMiktarUlusalSil", 2), new HucreAdres("kalanFaizMiktarUlusalSil",2), new HucreAdres("kalanToplamToplam",2),   new HucreAdres("duzeltimTarihi", 3), new HucreAdres("faizOranAyToplam", 5), new HucreAdres("faizOranGunToplam", 5) };

        Kullanici kullanan;
        TNS.ATM.IATMServis servis;
        //TNSCollection hucreAdresler = new TNSCollection();
        DosyaAB dab = new DosyaAB();
        TNSDateTime onayTarihi = new TNSDateTime();
        private string sablonAd;
        private string sonucDosyaAd;
        private Hashtable raporParam;
        private string ekBilgi;
        private int baslaSatir = -1;
        private int baslaSutun = -1;
        private int formatSatir = -1;
        private int formatSutun = -1;
        private int sat = 0;
        private int formatSayfa = 0;
        private int formatSatir2 = 0;
        private int formatSutun2 = 0;
        private int satirYuk = 0;
        private int paraBirimi = 1;
        private string ciktiYeri;
        bool pdf = false;
        int islemNo;
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {
            kullanan = (Kullanici)o_kullanan;
            servis = TNS.ATM.Arac.Tanimla();
            this.sablonAd = SablonBul(sablonAd);
            this.sonucDosyaAd = sonucDosyaAd;
            this.raporParam = raporParam ?? new Hashtable();
            this.ekBilgi = ekBilgi;
            dab.avans = Convert.ToBoolean(P("avans"));
            dab.iPARDDonemi = OrtakFonksiyonlar.ConvertToInt(P("iPARDDonemi"), -1);
            dab.islemTarihAralik = P("islemTarihAralik");
            dab.onBorcTarihAralik = P("onBorcTarihAralik");
            dab.geriAlimKarariTarihAralik = P("geriAlimTarihAralik");
            dab.geriAlimSurecAsamalari = P("geriAlimSurecAsamalari");
            dab.saymanlikKod = OrtakFonksiyonlar.ConvertToInt(P("saymanlikKod"), 0);
            dab.birimKod = P("birimKod");
            dab.aramaMetni = P("aramaMetni");
            dab.borcTurleri = P("borcTurleri");
            dab.islemliste = P("islemliste");
            TNSDateTime basTarih = new TNSDateTime(P("basTarih"));

            dab.dosyaAnahtar = OrtakFonksiyonlar.ConvertToInt(P("dosyaAnahtar"), 0);
            islemNo = OrtakFonksiyonlar.ConvertToInt(P("islemNo"), -1);
            paraBirimi = OrtakFonksiyonlar.ConvertToInt(P("pb"), 1);
            dab.borcDurumu = OrtakFonksiyonlar.ConvertToInt(P("borcDurumu"), 0);
            dab.islemTarihi.Yaz(P("islemTarihi"));
            if (TNSDateTime.IsNull(dab.islemTarihi)) dab.islemTarihi.Yaz(DateTime.Now);
            else dab.islemTarihi.AddMinutes(24 * 60 - 1);

            dab.onayTarihi.Yaz(P("onayTarihi"));
            if (TNSDateTime.IsNull(dab.onayTarihi)) dab.onayTarihi.Yaz(DateTime.Now);
            else dab.onayTarihi.AddMinutes(24 * 60 - 1);

            onayTarihi.Yaz(dab.onayTarihi);

            dab.onBorcTarihi.Yaz(P("onBorcTarihi"));

            this.ciktiYeri = P("CiktiYeri") + "";
            if (this.ciktiYeri.Equals("pdf", StringComparison.CurrentCultureIgnoreCase))
                pdf = true;
            if (ekBilgi == "2" || ekBilgi == "9")
                return WordRaporAl();
            else
                return ExcelRaporAl(dab.islemTarihi, basTarih);
        }

        /// <summary>raporParam değerini metin olarak okur (eski (string)raporParam["..."]); yoksa null.</summary>
        private string P(string ad)
        {
            object o = raporParam[ad];
            return o == null ? null : o.ToString();
        }

        /// <summary>Eski AlacakTakipListeAB.DovizKuruBul: verilen tarihten önceki ayın döviz (EUR) kuru.</summary>
        public static double DovizKuruBul(IATMServis servis, Kullanici kullanan, TNSDateTime kurTarihi)
        {
            return servis.DovizKuruBulOncAy(kullanan, kurTarihi);
        }

        /// <summary>
        /// RaporlamaServis'in verdiği şablon yolu (kurum RaporSablonYol + "ATM\ad") yoksa uygulamayla gelen RaporSablon\ATM\ad
        /// (çalışma klasörü = içerik kökü, sonra uygulama klasörü) denenir; hiçbiri yoksa gelen yol döner (eski davranış: Aspose hata verir).
        /// </summary>
        private static string SablonBul(string sablonAd)
        {
            if (string.IsNullOrWhiteSpace(sablonAd) || File.Exists(sablonAd)) return sablonAd;
            string ad = sablonAd.Replace('\\', '/');
            int i = ad.LastIndexOf("/ATM/", StringComparison.OrdinalIgnoreCase);
            string goreli = i >= 0 ? ad.Substring(i + 1) : "ATM/" + Path.GetFileName(ad);
            foreach (string kok in new string[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
            {
                string s = Path.Combine(kok, "RaporSablon", goreli.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(s)) return s;
            }
            return sablonAd;
        }

        private void OrtakBilgiYazWord()
        {
            foreach (HucreAdres item in HucreAdresler)
            {
                string str = "";

                if (item.tip == 1)
                    str = DegerBulInt(item.hucreAd).ToString();
                else if (item.tip == 2 || item.tip == 5)
                    str = DegerBulDbl(item.hucreAd).ToString(item.tip == 2 ? "#,##0.00" : "0.000000");
                else if (item.tip == 3)
                    str = new TNSDateTime(DegerBulTrh(item.hucreAd)).ToString();
                else if (item.tip == 4)
                    str = DegerBulStr(item.hucreAd);
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, item.hucreAd, str);
            }
        }

        private string WordRaporAl()
        {
            string uz = (pdf ? "pdf" : "docx");

            if (dab.iPARDDonemi == 3 && ekBilgi == "2")
            {
                sablonAd = Path.Combine(Path.GetDirectoryName(sablonAd), $"{Path.GetFileNameWithoutExtension(sablonAd)} I3{Path.GetExtension(sablonAd)}");
            }

            sonucDosyaAd = DosyaIslem.DosyaAdUret(System.IO.Path.GetFileNameWithoutExtension(sablonAd), "") + "." + uz;

            doc = new Aspose.Words.Document(sablonAd);
            builder = new DocumentBuilder(doc);
            ObjectArray oa = servis.ListeleDosyaAB(kullanan, dab);
            if (oa.sonuc.islemSonuc)
            {
                if (oa.ObjeSayisi >= 1)
                {
                    dab = (DosyaAB)oa.objeler[0];
                }
                if (ekBilgi == "2")
                    RaporAl6309();
                else
                    RaporAl6806(oa);

                OrtakBilgiYazWord();
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "kurumAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "hazirlayan", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzHazirlayan"));
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "kontrolEden", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzKontrolEden"));
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "gozdenGeciren", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzGozdenGeciren"));
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "onaylayan", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzOnaylayan"));
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "uygunBulan", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzUygunBulan"));

                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "hazirlayanUnv", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzHazirlayanUnv"));
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "kontrolEdenUnv", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzKontrolEdenUnv"));
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "gozdenGecirenUnv", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzGozdenGecirenUnv"));
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "onaylayanUnv", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzOnaylayanUnv"));
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "uygunBulanUnv", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzUygunBulanUnv"));
            }
            doc.Save(sonucDosyaAd, pdf ? SaveFormat.Pdf : SaveFormat.Docx);
            return sonucDosyaAd;
        }

        private void RaporAl6806(ObjectArray oa)
        {
            bool tl = this.paraBirimi != 2;
            DataTable dt = new DataTable();
            dt.TableName = "Borclular";
            dt.Columns.Add(new DataColumn("BorcluAd", typeof(string)));
            dt.Columns.Add(new DataColumn("DefterNo", typeof(string)));
            dt.Columns.Add(new DataColumn("KayitNo", typeof(string)));
            dt.Columns.Add(new DataColumn("Tedbir", typeof(string)));
            dt.Columns.Add(new DataColumn("Anapara", typeof(string)));
            dt.Columns.Add(new DataColumn("Faiz", typeof(string)));
            dt.Columns.Add(new DataColumn("Toplam", typeof(string)));
            dt.Columns.Add(new DataColumn("SilTar", typeof(string)));

            foreach (DosyaAB item in oa.objeler)
            {
                if (!OrtakFonksiyonlar.EsitMi(item.kalanMiktarAB, 0))
                {
                    item.IslemToplamBul(null, -1, true, null, onayTarihi, -1);
                    dt.Rows.Add(item.borcluAd, item.defterSirasi, item.basvuruKayitNumarasi, item.tertip, StrYap(item.kalanMiktarABKur(tl)), StrYap(item.kalanFaizMiktarABKur(tl)), StrYap(item.kalanToplamABKur(tl)), StrYap(item.ongorulenSilinmeTarihiDT));
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Kur", item.geriAlimDovizKuru.ToString("#.###0"));
                }
            }
            if (dt.Rows.Count == 0)
            {
                dt.Rows.Add("", "", "", "", "", "", "", "");
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Kur", "");
            }
            doc.MailMerge.ExecuteWithRegions(dt);
            string kurAd;
            if (tl) kurAd = "TL";
            else kurAd = "EUR";
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "PBAd", kurAd);
        }

        private object StrYap(DateTime tarih)
        {
            return tarih.ToShortDateString();
        }

        private object StrYap(double rakam)
        {
            return rakam.ToString("#,##0.00");
        }

        private void RaporAl6309()
        {
            BorcHareket bh = new BorcHareket(dab);
            bh.siraNo = OrtakFonksiyonlar.ConvertToInt(P("siraNo"), -1);
            bool olduguGibi = OrtakFonksiyonlar.ConvertToInt(P("olduguGibi"), 0) == 1;
            BorcHareket bhAsil = dab.HareketBul(bh.siraNo);
            if (bhAsil != null)
            {
                bh.yerNo = bhAsil.yerNo;
                bh.dovizKuru = bhAsil.dovizKuru;
                bh.tur = bhAsil.tur;
                bh.borcunIlgiliOlduguTaksit = bhAsil.borcunIlgiliOlduguTaksit;
                bh.tarih = new TNSDateTime(bhAsil.tarih.Oku());
                bh.duzeltimTarihi = new TNSDateTime(bhAsil.duzeltimTarihi.Oku());
                bh.yatirilanAB = bhAsil.yatirilanAB;
                bh.yatirilanUlusal = bhAsil.yatirilanUlusal;
            }
            else
            {
                bh.dovizKuru = DovizKuruBul(servis, kullanan, bh.tarih);
                bh.tur = BorcHareket.ENUMHareketTur.Odeme;
                bh.borcunIlgiliOlduguTaksit = P("borcunIlgiliOlduguTaksit");
                bh.tarih = new TNSDateTime(P("tarih"));
                bh.duzeltimTarihi = new TNSDateTime(P("duzeltimTarihi"));
                bh.yatirilanAB = OrtakFonksiyonlar.ConvertToDouble(P("yatirilanAB"));
                bh.yatirilanUlusal = OrtakFonksiyonlar.ConvertToDouble(P("yatirilanUlusal"));
            }
            dab.IslemToplamBul(bh.tarih, bh.yerNo - 1, true, null, onayTarihi, -1);
            bh.OdenendenMiktarHesapla(olduguGibi);

            string versiyon;

            if (dab.iPARDDonemi == 3)
                versiyon = "IPARD III VERSİYON 1.0.01";
            else
                versiyon = "VERSİYON 10.5.01";

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "versiyon", versiyon);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "borcunIlgiliOlduguTaksit", bh.borcunIlgiliOlduguTaksit);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "geriAlimTarihi", bh.tarih.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "duzeltimTarihi", bh.duzeltimTarihi.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "yatirilanAB", bh.yatirilanAB.ToString("#,##0.00"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "yatirilanUlusal", bh.yatirilanUlusal.ToString("#,##0.00"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "yatirilanToplam", bh.yatirilanToplam.ToString("#,##0.00"));

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "miktarAB", bh.miktarABSil.ToString("#,##0.00"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "faizAB", bh.faizABSil.ToString("#,##0.00"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "toplamAB", bh.toplamABSil.ToString("#,##0.00"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "miktarUlusal", bh.miktarUlusalSil.ToString("#,##0.00"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "faizUlusal", bh.faizUlusalSil.ToString("#,##0.00"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "toplamUlusal", bh.toplamUlusalSil.ToString("#,##0.00"));

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "miktarABEUR", bh.miktarABEURSil.ToString("#,##0.00"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "faizABEUR", bh.faizABEURSil.ToString("#,##0.00"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "miktarUlusalEUR", bh.miktarUlusalEURSil.ToString("#,##0.00"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "faizUlusalEUR", bh.faizUlusalEURSil.ToString("#,##0.00"));

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "dovizKuru", bh.dovizKuruIslem.ToString("0.0000"));

            if (dab.iPARDDonemi == 3)
            {
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarABye", (bh.toplamABSil).ToString("#,##0.00"));
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarAByeEUR", (bh.toplamABEURSil).ToString("#,##0.00"));
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarUlusala", (bh.toplamUlusalSil).ToString("#,##0.00"));
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarUlusalaEUR", (bh.toplamUlusalEURSil).ToString("#,##0.00"));
            }
            else
            {
                if (bh.aktarAB < 0)
                {
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarABDen", (0 - bh.aktarAB).ToString("#,##0.00"));
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarABDenEUR", (0 - bh.aktarABEUR).ToString("#,##0.00"));
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarABye", "");
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarAByeEUR", "");
                }
                else
                {
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarABDen", "");
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarABDenEUR", "");
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarABye", (bh.aktarAB).ToString("#,##0.00"));
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarAByeEUR", (bh.aktarABEUR).ToString("#,##0.00"));
                }

                if (bh.aktarUlusal < 0)
                {
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarUlusaldan", (0 - bh.aktarUlusal).ToString("#,##0.00"));
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarUlusaldanEUR", (0 - bh.aktarUlusalEUR).ToString("#,##0.00"));
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarUlusala", "");
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarUlusalaEUR", "");
                }
                else
                {
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarUlusaldan", "");
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarUlusaldanEUR", "");
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarUlusala", (bh.aktarUlusal).ToString("#,##0.00"));
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarUlusalaEUR", (bh.aktarUlusalEUR).ToString("#,##0.00"));
                }
            }
            if (bh.aktarKisiye > 0)
            {
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarKisiye", (bh.aktarKisiye).ToString("#,##0.00"));
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarKisiyeEUR", (bh.aktarKisiyeEUR).ToString("#,##0.00"));
            }
            else
            {
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarKisiye", "");
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "aktarKisiyeEUR", "");
            }
            Aspose.Words.Tables.Table table = (Aspose.Words.Tables.Table)doc.GetChild(NodeType.Table, 2, true);
            if (table != null)
            {
                int[] bolumSatirBas;
                if (dab.iPARDDonemi == 3)
                    bolumSatirBas = new int[] { 20, 28, 36, 40 };
                else
                    bolumSatirBas = new int[] { 22, 30, 34, 38, 46, 50, 54, 58, 62, 66 };
                int kalanBolum = bh.OdemeDurum;
                for (int i = bolumSatirBas.Length - 2; i >= 0; i--)
                {
                    if (i != kalanBolum - 1)
                    {
                        sat = bolumSatirBas[i];
                        for (int j = sat; j < bolumSatirBas[i + 1]; j++)
                        {
                            try
                            {
                                table.Rows.RemoveAt(sat);
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        private string ExcelRaporAl(TNSDateTime islemTarihi, TNSDateTime basTarih)
        {
            string uz = (pdf ? "pdf" : "xlsx");
            XLS = GenelIslemler.NewTablo();
            if (!string.IsNullOrWhiteSpace(sablonAd))
            {
                sonucDosyaAd = DosyaIslem.DosyaAdUret(System.IO.Path.GetFileNameWithoutExtension(sablonAd), "") + "." + uz;
                XLS.DosyaAc(sablonAd, sonucDosyaAd);
            }
            else
            {
                sonucDosyaAd = DosyaIslem.DosyaAdUret("Liste", "") + "." + uz;
                XLS.BosDosyaAc(sonucDosyaAd);
            }

            XLS.HucreAdAdresCoz("FormatSatir", ref formatSayfa, ref formatSatir, ref formatSutun, ref formatSatir2, ref formatSutun2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
            if (baslaSatir > 0)
                satirYuk = XLS.SatirYukseklikAl(baslaSatir);
            sat = baslaSatir;
            XLS.HucreAdBulYaz("HazirlamaTarihi", DateTime.Now.ToShortDateString());
            XLS.HucreAdBulYaz("kurumAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());

            XLS.HucreAdBulYaz("hazirlayan", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzHazirlayan"));
            XLS.HucreAdBulYaz("hazirlayanUnv", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzHazirlayanUnv"));

            XLS.HucreAdBulYaz("kontrolEden", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzKontrolEden"));
            XLS.HucreAdBulYaz("kontrolEdenUnv", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzKontrolEdenUnv"));

            XLS.HucreAdBulYaz("gozdenGeciren", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzGozdenGeciren"));
            XLS.HucreAdBulYaz("gozdenGecirenUnv", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzGozdenGecirenUnv"));

            XLS.HucreAdBulYaz("onaylayan", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzOnaylayan"));
            XLS.HucreAdBulYaz("onaylayanUnv", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzOnaylayanUnv"));

            XLS.HucreAdBulYaz("uygunBulan", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzUygunBulan"));
            XLS.HucreAdBulYaz("uygunBulanUnv", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzUygunBulanUnv"));

            XLS.HucreAdBulYaz("islemGunu", DateTime.Now.ToShortDateString());

            if (ekBilgi == "1" || ekBilgi == "7" || ekBilgi == "10" || ekBilgi == "16" || ekBilgi == "17" || ekBilgi == "21")
            {
                if (dab.iPARDDonemi > 0 && dab.dosyaAnahtar > 0)
                {
                    ObjectArray oa = servis.ListeleDosyaAB(kullanan, dab);
                    if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 1)
                    {
                        dab = (DosyaAB)oa.objeler[0];
                        string fhst = P("islemTarihi");
                        if (!string.IsNullOrWhiteSpace(fhst))
                        {
                            dab.IslemToplamBul(new TNSDateTime(fhst), -1, true, null, onayTarihi, -1);
                        }
                        if (ekBilgi == "1")
                            RaporAl6307(this.paraBirimi != 2);
                        else if (ekBilgi == "7")
                            RaporAl6310();
                        else if (ekBilgi == "10")
                            RaporAl6807();
                        else if (ekBilgi == "17")
                        {
                            dab.oncekiGunHesapla = true;
                            RaporAlTaksit(islemTarihi);
                        }
                        else if (ekBilgi == "21")
                        {
                            dab.oncekiGunHesapla = true;
                            RaporAlIslemler(islemTarihi);
                        }
                        OrtakBilgiYazExcel();
                        dab.oncekiGunHesapla = false;
                    }
                    else
                        XLS.HucreDegerYaz(1, 1, oa.sonuc.hataStr);
                }
                else
                    XLS.HucreDegerYaz(0, 0, "Lütfen dosya bilgisini giriniz!");
            }
            else
            {
                TNSDateTime saklaIslemTarih = null;
                dab.dosyaAnahtar = 0;
                if (ekBilgi == "5" || ekBilgi == "6" || ekBilgi == "19" || ekBilgi == "20")
                {
                    saklaIslemTarih = new TNSDateTime(dab.islemTarihi.Oku());
                    if (TNSDateTime.IsNull(basTarih))
                        basTarih = new TNSDateTime(new DateTime(dab.islemTarihi.Yil, 1, 1));
                    dab.islemTarihi.Yaz(basTarih.Oku());
                }
                ObjectArray oa = servis.ListeleDosyaAB(kullanan, dab);
                if (oa.sonuc.islemSonuc)
                {
                    if (ekBilgi == "3" || ekBilgi == "4")
                        RaporAl6305(oa, ekBilgi == "3", 1, dab.islemTarihi, basTarih);
                    else if (ekBilgi == "5" || ekBilgi == "6")
                        RaporAl6305(oa, ekBilgi == "5", 2, dab.islemTarihi, saklaIslemTarih);
                    else if (ekBilgi == "19" || ekBilgi == "20")
                        RaporAl6305(oa, ekBilgi == "19", 3, dab.islemTarihi, saklaIslemTarih);
                    else if (ekBilgi == "8")
                        RaporAl6307Toplu(oa, dab.islemTarihi);
                    else if (ekBilgi == "11")
                        RaporAl6403(oa, dab.islemTarihi);
                    else if (ekBilgi == "12")
                        RaporAl6404(oa, dab.islemTarihi);
                    else if (ekBilgi == "13")
                        RaporAl6405(oa, dab.islemTarihi);
                    else if (ekBilgi == "14")
                        RaporAl6406(oa, dab.islemTarihi);
                    else if (ekBilgi == "15")
                        RaporAl6804(oa, dab.islemTarihi);
                    else if (ekBilgi == "18")
                        RaporAlTumHareketler(oa, dab.islemTarihi, basTarih);
                }
                else
                    XLS.HucreDegerYaz(baslaSatir, baslaSutun, oa.sonuc.hataStr);
            }
            XLS.DosyaSaklamaFormatAta(uz);
            XLS.DosyaSaklaTamYol();
            return XLS.SonucDosyaAd();
        }

        private void RaporAlIslemler(TNSDateTime islemTarihi)
        {
            List<List<double>> l = new List<List<double>>();

            for (int i = 0; i < dab.borcHareket.Count; i++)
            {
                BorcHareket bh = ((BorcHareket)dab.borcHareket[i]);
                if (bh.FaizIslemi || bh.tur == BorcHareket.ENUMHareketTur.Taksit || bh.tur == BorcHareket.ENUMHareketTur.FazlaOdemeAktarim || bh.tur == BorcHareket.ENUMHareketTur.OdeneniAktarma) continue;
                XLS.HucreKopyala(formatSatir, formatSutun, formatSatir2, formatSutun2, sat, baslaSutun);
                XLS.HucreDegerYaz(sat, baslaSutun, i + 1);
                XLS.HucreDegerYaz(sat, baslaSutun + 1, bh.tarihDT);
                XLS.HucreDegerYaz(sat, baslaSutun + 2, bh.turAd);

                List<double> degerler = new List<double>();
                if (bh.tur == BorcHareket.ENUMHareketTur.Silme)
                {
                    bh.miktarAB = 0;
                    bh.miktarUlusal = 0;
                    bh.faizAB = 0;
                    bh.faizUlusal = 0;
                }


                degerler.Add(bh.yatirilanToplam);
                degerler.Add(bh.miktarAB);
                degerler.Add(bh.miktarUlusal);
                degerler.Add(bh.faizAB);
                degerler.Add(bh.faizUlusal);
                l.Add(degerler);
            }

            ToplamAl ta = new ToplamAl(2, 9);
            ta.SatirSifirla(0);
            ta.SatirSifirla(1);
            TNSDateTime bugun = new TNSDateTime(DateTime.Now);
            int yer = 0;
            for (int i = 0; i < dab.borcHareket.Count; i++)
            {
                BorcHareket bh = ((BorcHareket)dab.borcHareket[i]);
                if (bh.FaizIslemi || bh.tur == BorcHareket.ENUMHareketTur.Taksit || bh.tur == BorcHareket.ENUMHareketTur.FazlaOdemeAktarim || bh.tur == BorcHareket.ENUMHareketTur.OdeneniAktarma) continue;

                if (bh.tur == BorcHareket.ENUMHareketTur.Duzeltme || bh.tur == BorcHareket.ENUMHareketTur.Silme)
                {
                    bh.miktarAB = 0;
                    bh.miktarUlusal = 0;
                    bh.faizAB = 0;
                    bh.faizUlusal = 0;
                }

                if (bh.tur == BorcHareket.ENUMHareketTur.Odeme || bh.tur == BorcHareket.ENUMHareketTur.TaksitOdeme)
                {
                    dab.IslemToplamBul(bh.tarih, bh.yerNo - 1, true, null, bugun, bh.ilgiliIslemSiraNo);
                    //OrtakFonksiyonlar.HataDosyaYaz("TaksitHesap.txt", $"{bh.tarih} :: KTAB:{dab.kalanToplamAB} :: KTU:{dab.kalanToplamUlusal} :: MAB:{bh.miktarAB} :: MU:{bh.miktarUlusal}", false);
                    bh.OdenendenMiktarHesapla(false, true);
                    //OrtakFonksiyonlar.HataDosyaYaz("TaksitHesap.txt", $"{bh.tarih} :: KTAB:{dab.kalanToplamAB} :: KTU:{dab.kalanToplamUlusal} :: MAB:{bh.miktarAB} :: MU:{bh.miktarUlusal}", false);
                    //OrtakFonksiyonlar.HataDosyaYaz("TaksitHesap.txt", $"==================", false);
                }
                XLS.HucreKopyala(formatSatir, formatSutun, formatSatir2, formatSutun2, sat, baslaSutun);
                XLS.HucreDegerYaz(sat, baslaSutun, yer + 1);
                XLS.HucreDegerYaz(sat, baslaSutun + 1, bh.tarihDT);
                XLS.HucreDegerYaz(sat, baslaSutun + 2, bh.turAd);

                List<double> degerler = l[yer++];
                ta.DegerAta(0, 0, degerler[0]);
                ta.DegerAta(0, 1, degerler[1]);
                ta.DegerAta(0, 2, degerler[2]);
                ta.DegerAta(0, 3, degerler[3]);
                ta.DegerAta(0, 4, degerler[4]);

                ta.DegerAta(0, 5, bh.miktarAB);
                ta.DegerAta(0, 6, bh.miktarUlusal);
                ta.DegerAta(0, 7, bh.faizAB);
                ta.DegerAta(0, 8, bh.faizUlusal);
                ta.ToplamEkle(1, 0);

                IslemRakamYaz(sat, ta, 0);
                sat++;
            }

            XLS.HucreKopyala(formatSatir + 1, formatSutun, formatSatir2 + 2, formatSutun2, sat, baslaSutun);
            IslemRakamYaz(sat, ta, 1);
        }

        private void IslemRakamYaz(int sat, ToplamAl ta, int diziSat)
        {
            int sut = 4;
            for (int i = 0; i < 9; i++)
            {
                if (i == 5) sut += 3;
                XLS.HucreDegerYaz(sat, sut, ta.toplam[diziSat, i]);
                sut++;
            }
        }

        private void RaporAlTaksit(TNSDateTime islemTarihi)
        {
            DosyaABTaksit dosyaTaksit = new DosyaABTaksit();

            dosyaTaksit.taksitTalepTarihi.Yaz(P("taksitTalepTarih"));
            dosyaTaksit.taksitTalepSayi = P("taksitTalepSayi");
            dosyaTaksit.taksitKararTarihi.Yaz(P("taksitKararTarih"));
            dosyaTaksit.taksitKararSayi = P("taksitKararSayi");
            dosyaTaksit.taksitIlkTarih.Yaz(P("taksitIlkTarih"));
            dosyaTaksit.taksitSayisi = OrtakFonksiyonlar.ConvertToInt(P("taksitSayisi"), 0);
            dosyaTaksit.tecilFaizNo = OrtakFonksiyonlar.ConvertToInt(P("tecilFaizNo"), 0);
            dab.tecilFaizNo = dosyaTaksit.tecilFaizNo;
            if (!TNS.TNSDateTime.IsNull(dosyaTaksit.taksitTalepTarihi))
            {
                dab.YeniTaksitYap(OrtakFonksiyonlar.ConvertToInt(P("taksitSayisi"), 1), dosyaTaksit);
            }
            else
            {
                dab.IslemToplamBul(new TNSDateTime(dab.taksitTalepTarih.Oku()), -1, true, null, onayTarihi, -1, true);
                dosyaTaksit = null;
            }

            XLS.HucreAdBulYaz("taksitTalepSayi", dab.taksitTalepTarih.ToString() + "/" + dab.taksitTalepSayi);
            XLS.HucreAdBulYaz("taksitKararSayi", dab.taksitKararTarih.ToString() + "/" + dab.taksitKararSayi);

            XLS.HucreAdBulYaz("tecilBasTar", dab.taksitTalepTarih.ToString());

            int taksitSay = 0;
            int sat = baslaSatir;
            ToplamAl ta = new ToplamAl(2, 6);
            ta.SatirSifirla(0);
            ta.SatirSifirla(1);
            for (int i = 0; i < dab.borcHareket.Count; i++)
            {
                BorcHareket bh = ((BorcHareket)dab.borcHareket[i]);
                if (bh.tur != BorcHareket.ENUMHareketTur.Taksit) continue;
                taksitSay++;
                bh.anaDosya = dab;
                XLS.HucreKopyala(formatSatir, formatSutun, formatSatir2, formatSutun2, sat, baslaSutun);
                XLS.HucreDegerYaz(sat, baslaSutun, taksitSay);
                XLS.HucreDegerYaz(sat, baslaSutun + 1, bh.duzeltimTarihiDT);
                XLS.HucreDegerYaz(sat, baslaSutun + 2, bh.taksitGun);
                ta.DegerAta(0, 0, bh.miktarToplam);
                ta.DegerAta(0, 1, bh.gecikmeZammi);
                ta.DegerAta(0, 2, bh.tecilFaizi);
                ta.DegerAta(0, 3, bh.toplamToplam);
                ta.DegerAta(0, 4, bh.toplamAB);
                ta.DegerAta(0, 5, bh.toplamUlusal);

                TaksitRakamYaz(sat, ta, 0);
                ta.ToplamEkle(1, 0);
                sat++;
            }

            XLS.HucreKopyala(formatSatir + 1, formatSutun, formatSatir2 + 1, formatSutun2, sat, baslaSutun);
            TaksitRakamYaz(sat, ta, 1);

            XLS.SatirYukseklikAyarla(baslaSatir, sat, satirYuk);
            XLS.HucreAdBulYaz("taksitFaiz", ta.toplam[1, 2]);
            FaizIslemler.FaizDonem[] tut = FaizIslemler.FaizOranAyBul(dab.taksitTalepTarihDT, dab.tecilFaizNo);
            XLS.HucreAdBulYaz("taksitFaizOran", tut[0].faizOran * 12);
            XLS.HucreAdBulYaz("taksitToplamBorc", ta.toplam[1, 3]);
            XLS.HucreAdBulYaz("taksitSay", taksitSay);

            //Ödemeleri yaz
            taksitSay = 0;
            sat = baslaSatir;
            ta.SatirSifirla(0);
            ta.SatirSifirla(1);
            baslaSutun += 10;
            ToplamAl t2 = new ToplamAl(1, 4);
            t2.SatirSifirla(0);
            for (int i = 0; i < dab.borcHareket.Count; i++)
            {
                BorcHareket bh = ((BorcHareket)dab.borcHareket[i]);
                if (!bh.OdemeTuruIslem) continue;
                if (bh.tarihDT < dab.taksitKararTarihDT) continue;

                taksitSay++;
                bh.anaDosya = dab;
                XLS.HucreKopyala(formatSatir - 2, formatSutun, formatSatir2 - 2, formatSutun2, sat, baslaSutun);
                XLS.HucreDegerYaz(sat, baslaSutun, taksitSay);
                bh.duzeltimTarihiDT = bh.tarihDT;
                XLS.HucreDegerYaz(sat, baslaSutun + 1, bh.duzeltimTarihiDT);
                XLS.HucreDegerYaz(sat, baslaSutun + 2, bh.taksitGun);
                ta.DegerAta(0, 0, bh.miktarToplam);
                ta.DegerAta(0, 1, bh.gecikmeZammi);
                ta.DegerAta(0, 2, bh.tecilFaizi);
                ta.DegerAta(0, 3, bh.toplamToplam);
                ta.DegerAta(0, 4, bh.toplamAB);
                ta.DegerAta(0, 5, bh.toplamUlusal);

                t2.DegerArttir(0, 0, bh.miktarAB);
                t2.DegerArttir(0, 1, bh.miktarUlusal);
                t2.DegerArttir(0, 2, bh.ertelemeAB);
                t2.DegerArttir(0, 3, bh.ertelemeUl);

                TaksitRakamYaz(sat, ta, 0);
                ta.ToplamEkle(1, 0);
                sat++;
            }
            DateTime bugun = islemTarihi.Oku();

            dab.IslemToplamBul(islemTarihi, -1, true, dosyaTaksit, onayTarihi, -1);

            XLS.HucreAdBulYaz("kalanAnaPara", dab.kalanMiktarToplam);
            XLS.HucreAdBulYaz("kalanGecikmeFaizi", dab.kalanErtelemeToplam);
            XLS.HucreAdBulYaz("kalanToplamBorc", dab.kalanToplamToplam);
            XLS.HucreAdBulYaz("RaporTarihi", islemTarihi.ToString() + "/" + dab.gun2 + " Gün");

            dab.TaksitAyarla(bugun, true);
            //Bugün ve bugünden sonra olan taksitleri yaz;
            for (int i = 0; i < dab.borcHareket.Count; i++)
            {
                BorcHareket bh = ((BorcHareket)dab.borcHareket[i]);
                if (bh.tur != BorcHareket.ENUMHareketTur.Taksit) continue;
                if (bh.duzeltimTarihiDT < bugun) continue;
                if (bh.duzeltimTarihiDT == bugun && bh.miktarToplam == 0 && bh.ertelemeAB == 0 && bh.ertelemeUl == 0) continue;
                int farkSatir = 0;
                if (bh.duzeltimTarihiDT == bugun) farkSatir = -1;
                taksitSay++;
                bh.anaDosya = dab;
                XLS.HucreKopyala(formatSatir + farkSatir, formatSutun, formatSatir2 + farkSatir, formatSutun2, sat, baslaSutun);
                XLS.HucreDegerYaz(sat, baslaSutun, taksitSay);
                XLS.HucreDegerYaz(sat, baslaSutun + 1, bh.duzeltimTarihiDT);
                XLS.HucreDegerYaz(sat, baslaSutun + 2, bh.taksitGun);
                ta.DegerAta(0, 0, bh.miktarToplam);
                ta.DegerAta(0, 1, bh.gecikmeZammi);
                ta.DegerAta(0, 2, bh.tecilFaizi);
                ta.DegerAta(0, 3, bh.toplamToplam);
                ta.DegerAta(0, 4, bh.toplamAB);
                ta.DegerAta(0, 5, bh.toplamUlusal);

                TaksitRakamYaz(sat, ta, 0);
                ta.ToplamEkle(1, 0);
                sat++;
            }

            XLS.HucreKopyala(formatSatir + 1, formatSutun, formatSatir2 + 1, formatSutun2, sat, baslaSutun);
            TaksitRakamYaz(sat, ta, 1);
            TNSDateTime dttaksitTalepTarih = new TNSDateTime(dab.taksitTalepTarih.Oku());
            dab.IslemToplamBul(dttaksitTalepTarih, -1, true, dosyaTaksit, onayTarihi, -1, true);

            return;
        }

        private void TaksitRakamYaz(int sat, ToplamAl ta, int taSat)
        {
            for (int i = 0; i <= 5; i++)
            {
                XLS.HucreDegerYaz(sat, baslaSutun + 3 + i, ta.toplam[taSat, i]);
            }
        }

        private void RaporAl6804(ObjectArray oa, TNSDateTime faizHesabiSonTarihi)
        {
            XLS.HucreAdBulYaz("HazirlamaTarihi", DateTime.Now.ToShortDateString());
            DateTime dtBas, dtSon;
            string tut = YilTarihleriBul(faizHesabiSonTarihi, out dtBas, out dtSon, true);
            XLS.HucreAdBulYaz("Donem", tut);
            YilTarihleriBul(faizHesabiSonTarihi, out dtBas, out dtSon, false);
            XLS.HucreAdBulYaz("KayitSayisi1", oa.ObjeSayisi);

            int fshNo = 0;
            int fsat1 = 0;
            int fsut1 = 0;
            int fsat2 = 0;
            int fsut2 = 0;
            int bsat1 = 0;
            int bsut1 = 0;

            XLS.HucreAdAdresCoz("FormatSatir", ref fshNo, ref fsat1, ref fsut1, ref fsat2, ref fsut2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref bsat1, ref bsut1);

            int yuks = XLS.SatirYukseklikAl(bsat1);
            int sat = bsat1;
            ToplamAl t = new ToplamAl(2, 12);
            foreach (DosyaAB gelenDosya in oa.objeler)
            {
                BorcHareket bhHar = gelenDosya.HareketBul(new BorcHareket.ENUMHareketTur[] { BorcHareket.ENUMHareketTur.Silme, BorcHareket.ENUMHareketTur.Odeme, BorcHareket.ENUMHareketTur.TaksitOdeme }, dtBas, dtSon);

                if (bhHar != null)
                {
                    XLS.SatirAc(sat, 1);
                    XLS.HucreKopyala(fsat1, fsut1, fsat2, fsut2, sat, bsut1);
                    XLS.HucreDegerYaz(sat, bsut1 + 0, "(" + ((int)gelenDosya.geriAlimSurecAsamasi).ToString() + ") " + gelenDosya.geriAlimSurecAsamasiAdEn);
                    XLS.HucreDegerYaz(sat, bsut1 + 1, gelenDosya.onBorcTarihi.Oku());
                    XLS.HucreDegerYaz(sat, bsut1 + 2, gelenDosya.tedbir + "-" + gelenDosya.altTedbir + "-" + gelenDosya.butceKalemi); // + "/" + gelenDosya.tedbirAdEn
                    XLS.HucreDegerYaz(sat, bsut1 + 3, gelenDosya.defterSirasi);
                    XLS.HucreDegerYaz(sat, bsut1 + 4, gelenDosya.borcunKodu);
                    XLS.HucreDegerYaz(sat, bsut1 + 5, gelenDosya.vakaNumarasi);

                    t.DegerAta(0, 0, gelenDosya.geriAlimMiktarABKur(false));
                    t.DegerAta(0, 1, gelenDosya.geriAlimMiktarUlusalKur(false));

                    MiktarIslem toplamBul = gelenDosya.HareketToplamBul(new BorcHareket.ENUMHareketTur[] { BorcHareket.ENUMHareketTur.Silme, BorcHareket.ENUMHareketTur.Odeme, BorcHareket.ENUMHareketTur.TaksitOdeme }, dtBas, dtSon, false);

                    t.DegerAta(0, 2, toplamBul.miktarAB);
                    t.DegerAta(0, 3, toplamBul.faizAB);
                    t.DegerAta(0, 4, toplamBul.miktarUlusal);
                    t.DegerAta(0, 5, toplamBul.faizUlusal);
                    t.DegerAta(0, 6, toplamBul.toplamToplam);
                    t.DegerAta(0, 7, gelenDosya.kalanMiktarABEURSil);
                    t.DegerAta(0, 8, gelenDosya.kalanFaizMiktarABEURSil);
                    t.DegerAta(0, 9, gelenDosya.kalanMiktarUlusalEURSil);
                    t.DegerAta(0, 10, gelenDosya.kalanFaizMiktarUlusalEURSil);

                    BorcHareket bhSil = gelenDosya.HareketBul(BorcHareket.ENUMHareketTur.Silme, DateTime.MinValue, DateTime.MaxValue);
                    if (bhSil != null)
                    {
                        t.DegerAta(0, 11, bhSil.miktarABEUR);
                        XLS.HucreDegerYaz(sat, bsut1 + 20, bhSil.tarihDT);
                    }
                    else
                        t.DegerAta(0, 11, 0);

                    R6804Yaz(sat, bsut1, t, 0);
                    sat++;
                }
            }
            if (sat > bsat1)
            {
                XLS.SatirAc(sat, 1);
                XLS.HucreKopyala(fsat1, fsut1, fsat2, fsut2, sat, bsut1);
                XLS.HucreBirlestir(sat, bsut1 + 0, sat, bsut1 + 5);
                XLS.KoyuYap(sat, bsut1 + 0, sat, bsut1 + 20, true);
                XLS.HucreDegerYaz(sat, bsut1 + 0, "T O T A L");
                R6804Yaz(sat, bsut1, t, 1);
                sat++;
            }
            XLS.SatirYukseklikAyarla(bsat1, sat, -1, 3, yuks);
        }

        private void R6804Yaz(int sat, int bsut1, ToplamAl t, int tSat)
        {
            XLS.HucreDegerYaz(sat, bsut1 + 6, t.toplam[tSat, 0]);
            XLS.HucreDegerYaz(sat, bsut1 + 7, t.toplam[tSat, 1]);
            XLS.HucreDegerYaz(sat, bsut1 + 9, t.toplam[tSat, 2]);
            XLS.HucreDegerYaz(sat, bsut1 + 10, t.toplam[tSat, 3]);
            XLS.HucreDegerYaz(sat, bsut1 + 11, t.toplam[tSat, 4]);
            XLS.HucreDegerYaz(sat, bsut1 + 12, t.toplam[tSat, 5]);
            XLS.HucreDegerYaz(sat, bsut1 + 13, t.toplam[tSat, 6]);
            XLS.HucreDegerYaz(sat, bsut1 + 14, t.toplam[tSat, 7]);
            XLS.HucreDegerYaz(sat, bsut1 + 15, t.toplam[tSat, 8]);
            XLS.HucreDegerYaz(sat, bsut1 + 16, t.toplam[tSat, 9]);
            XLS.HucreDegerYaz(sat, bsut1 + 17, t.toplam[tSat, 10]);
            XLS.HucreDegerYaz(sat, bsut1 + 19, t.toplam[tSat, 11]);
            if (tSat == 0)
                t.ToplamEkle(1, 0);
        }

        private void RaporAl6406(ObjectArray oa, TNSDateTime faizHesabiSonTarihi)
        {
            //tabloları ters sırada yazdırmak lazım.
            // tablo 3 Silinmiş borç tutarları
            #region tablo3
            XLS.HucreAdBulYaz("HazirlamaTarihi", faizHesabiSonTarihi.ToString());
            DateTime dtBas, dtSon;
            string tut = AyTarihleriBul(faizHesabiSonTarihi, out dtBas, out dtSon, true);
            XLS.HucreAdBulYaz("Donem", tut);
            AyTarihleriBul(faizHesabiSonTarihi, out dtBas, out dtSon, false);
            XLS.HucreAdBulYaz("KayitSayisi1", oa.ObjeSayisi);

            int fshNo = 0;
            int fsat1 = 0;
            int fsut1 = 0;
            int fsat2 = 0;
            int fsut2 = 0;
            int bsat1 = 0;
            int bsut1 = 0;

            XLS.HucreAdAdresCoz("FormatSatir3", ref fshNo, ref fsat1, ref fsut1, ref fsat2, ref fsut2);
            XLS.HucreAdAdresCoz("BaslaSatir3", ref bsat1, ref bsut1);

            int yuks = XLS.SatirYukseklikAl(bsat1);
            int sat = bsat1;
            foreach (DosyaAB gelenDosya in oa.objeler)
            {
                BorcHareket bh = gelenDosya.HareketBul(BorcHareket.ENUMHareketTur.Silme, dtBas, dtSon);
                if (bh != null)
                {
                    XLS.SatirAc(sat, 1);
                    XLS.HucreKopyala(fsat1, fsut1, fsat2, fsut2, sat, bsut1);
                    XLS.HucreDegerYaz(sat, bsut1 + 0, gelenDosya.borcluAd);
                    XLS.HucreDegerYaz(sat, bsut1 + 1, gelenDosya.borcluKod);
                    XLS.HucreDegerYaz(sat, bsut1 + 2, gelenDosya.basvuruKayitNumarasi);

                    XLS.HucreDegerYaz(sat, bsut1 + 5, bh.tarihDT);
                    XLS.HucreDegerYaz(sat, bsut1 + 6, bh.miktarAB);
                    sat++;
                }
            }
            XLS.SatirYukseklikAyarla(bsat1, sat, -1, 3, yuks);
            #endregion

            //Tablo 2 yeni borçlar ve yeni ödemeler
            #region tablo2
            XLS.HucreAdAdresCoz("FormatSatir2", ref fshNo, ref fsat1, ref fsut1, ref fsat2, ref fsut2);
            XLS.HucreAdAdresCoz("BaslaSatir2", ref bsat1, ref bsut1);

            sat = bsat1;
            dab.dosyaAnahtar = 0;
            dab.islemTarihAralik = null;
            dab.islemliste = null;
            oa = servis.ListeleDosyaAB(kullanan, dab);
            foreach (DosyaAB gelenDosya in oa.objeler)
            {
                gelenDosya.IslemToplamBul(dab.islemTarihi, -1, true, null, onayTarihi, -1);
                bool tarihVar = DosyaAB.TarihIcindeMi(gelenDosya.geriAlimKarariTarihi, dtBas, dtSon);
                if (tarihVar)
                {
                    XLS.SatirAc(sat, 1);
                    XLS.HucreKopyala(fsat1, fsut1, fsat2, fsut2, sat, bsut1);
                    XLS.HucreDegerYaz(sat, bsut1 + 0, gelenDosya.borcluAd);
                    XLS.HucreDegerYaz(sat, bsut1 + 1, gelenDosya.borcluKod);
                    XLS.HucreDegerYaz(sat, bsut1 + 2, gelenDosya.basvuruKayitNumarasi);
                    if (gelenDosya.borcTuru == DosyaAB.BorcTurleri.UsulsuzluklerToptan)
                    {
                        XLS.HucreDegerYaz(sat, bsut1 + 3, gelenDosya.vakaNumarasi);
                        XLS.HucreDegerYaz(sat, bsut1 + 4, gelenDosya.borcunKodu);
                        XLS.HucreDegerYaz(sat, bsut1 + 8, gelenDosya.vakaNumarasi);
                        XLS.HucreDegerYaz(sat, bsut1 + 9, gelenDosya.borcunKodu);
                    }
                    XLS.HucreDegerYaz(sat, bsut1 + 5, gelenDosya.geriAlimKarariTarihi.ToString() + "/" + gelenDosya.onBorcSayi);
                    XLS.HucreDegerYaz(sat, bsut1 + 6, gelenDosya.geriAlimMiktarToplam);

                    XLS.HucreDegerYaz(sat, bsut1 + 10, gelenDosya.geriAlimKarariTarihi.ToString() + "/" + gelenDosya.onBorcSayi);
                    XLS.HucreDegerYaz(sat, bsut1 + 11, gelenDosya.geriAlimMiktarToplam);
                    sat++;
                }
            }
            foreach (DosyaAB gelenDosya in oa.objeler)
            {
                foreach (BorcHareket bhItem in gelenDosya.borcHareket)
                {
                    if (bhItem.kayitDurum != DosyaABParca.ENUMKayitDurum.ONAYLI) continue;

                    if (DosyaAB.TurListedeVarMi(bhItem.tur, new BorcHareket.ENUMHareketTur[] { BorcHareket.ENUMHareketTur.Odeme, BorcHareket.ENUMHareketTur.TaksitOdeme, BorcHareket.ENUMHareketTur.Duzeltme, BorcHareket.ENUMHareketTur.Iade, BorcHareket.ENUMHareketTur.Mahsup, BorcHareket.ENUMHareketTur.Terkin })
                        && DosyaAB.TarihIcindeMi(bhItem.tarih, dtBas, dtSon))
                    {
                        XLS.SatirAc(sat, 1);
                        XLS.HucreKopyala(fsat1, fsut1, fsat2, fsut2, sat, bsut1);
                        XLS.HucreDegerYaz(sat, bsut1 + 0, gelenDosya.borcluAd);
                        XLS.HucreDegerYaz(sat, bsut1 + 1, gelenDosya.borcluKod);
                        XLS.HucreDegerYaz(sat, bsut1 + 2, gelenDosya.basvuruKayitNumarasi);
                        if (gelenDosya.borcTuru == DosyaAB.BorcTurleri.UsulsuzluklerToptan)
                        {
                            XLS.HucreDegerYaz(sat, bsut1 + 3, gelenDosya.vakaNumarasi);
                            XLS.HucreDegerYaz(sat, bsut1 + 4, gelenDosya.borcunKodu);
                            XLS.HucreDegerYaz(sat, bsut1 + 8, gelenDosya.vakaNumarasi);
                            XLS.HucreDegerYaz(sat, bsut1 + 9, gelenDosya.borcunKodu);
                        }
                        XLS.HucreDegerYaz(sat, bsut1 + 5, bhItem.tarih.ToString());
                        XLS.HucreDegerYaz(sat, bsut1 + 6, gelenDosya.geriAlimMiktarToplam);
                        XLS.HucreDegerYaz(sat, bsut1 + 7, bhItem.toplamToplam);

                        XLS.HucreDegerYaz(sat, bsut1 + 10, bhItem.tarih.ToString());
                        XLS.HucreDegerYaz(sat, bsut1 + 11, gelenDosya.geriAlimMiktarToplam);
                        XLS.HucreDegerYaz(sat, bsut1 + 12, bhItem.toplamToplam);
                        sat++;
                    }
                }
            }
            XLS.SatirYukseklikAyarla(bsat1, sat, -1, 3, yuks);
            #endregion

            //tablo 1 yeni ön borçlar
            #region tablo 1
            XLS.HucreAdAdresCoz("FormatSatir1", ref fshNo, ref fsat1, ref fsut1, ref fsat2, ref fsut2);
            XLS.HucreAdAdresCoz("BaslaSatir1", ref bsat1, ref bsut1);
            sat = bsat1;
            dab.dosyaAnahtar = 0;
            dab.onBorcTarihAralik = tut;
            dab.islemliste = null;
            oa = servis.ListeleDosyaAB(kullanan, dab);

            foreach (DosyaAB gelenDosya in oa.objeler)
            {
                XLS.SatirAc(sat, 1);
                XLS.HucreKopyala(fsat1, fsut1, fsat2, fsut2, sat, bsut1);
                XLS.HucreDegerYaz(sat, bsut1 + 0, gelenDosya.borcluAd);
                XLS.HucreDegerYaz(sat, bsut1 + 1, gelenDosya.borcluKod);
                XLS.HucreDegerYaz(sat, bsut1 + 2, gelenDosya.basvuruKayitNumarasi);
                if (gelenDosya.borcTuru == DosyaAB.BorcTurleri.UsulsuzluklerToptan)
                {
                    XLS.HucreDegerYaz(sat, bsut1 + 3, gelenDosya.vakaNumarasi);
                    XLS.HucreDegerYaz(sat, bsut1 + 4, gelenDosya.borcunKodu);
                    XLS.HucreDegerYaz(sat, bsut1 + 8, gelenDosya.vakaNumarasi);
                    XLS.HucreDegerYaz(sat, bsut1 + 9, gelenDosya.borcunKodu);
                }
                XLS.HucreDegerYaz(sat, bsut1 + 5, gelenDosya.onBorcTarihi.ToString() + "/" + gelenDosya.onBorcSayi);

                XLS.HucreDegerYaz(sat, bsut1 + 6, gelenDosya.onBorcMiktarAB);
                XLS.HucreDegerYaz(sat, bsut1 + 7, gelenDosya.onBorcMiktarUlusal);

                XLS.HucreDegerYaz(sat, bsut1 + 10, gelenDosya.onBorcTarihi.ToString() + "/" + gelenDosya.onBorcSayi);
                XLS.HucreDegerYaz(sat, bsut1 + 11, gelenDosya.onBorcMiktarAB);
                XLS.HucreDegerYaz(sat, bsut1 + 12, gelenDosya.onBorcMiktarUlusal);
                sat++;
            }
            XLS.SatirYukseklikAyarla(bsat1, sat, -1, 3, yuks);
            #endregion
        }

        private void RaporAlTumHareketler(ObjectArray oa, TNSDateTime faizHesabiSonTarihi, TNSDateTime basTarih)
        {
            XLS.HucreAdBulYaz("HazirlamaTarihi", faizHesabiSonTarihi.ToString());
            DateTime dtBas, dtSon;
            dtBas = basTarih.Oku();
            dtSon = faizHesabiSonTarihi.Oku();
            //XLS.HucreAdBulYaz("Donem", DonemTarihleriBul(faizHesabiSonTarihi, out dtBas, out dtSon, true));
            //DonemTarihleriBul(faizHesabiSonTarihi, out dtBas, out dtSon, false);
            XLS.HucreAdBulYaz("KayitSayisi1", oa.ObjeSayisi);

            oa = servis.ListeleDosyaAB(kullanan, dab);
            XLS.HucreAdBulYaz("KayitSayisi2", oa.ObjeSayisi);
            int sat = 0;
            int kol = 0;

            XLS.HucreDegerYaz(sat, kol++, "No");
            XLS.HucreDegerYaz(sat, kol++, "Borçlu Ad");
            XLS.HucreDegerYaz(sat, kol++, "Tedbir");
            XLS.HucreDegerYaz(sat, kol++, "Başvuru Kayıt Numarası");
            XLS.HucreDegerYaz(sat, kol++, "Sözleşme Numarası");
            XLS.HucreDegerYaz(sat, kol++, "Borç Türü Ad");
            XLS.HucreDegerYaz(sat, kol++, "Geri Alım Kararı Tarihi");

            XLS.HucreDegerYaz(sat, kol++, "İşlem Türü");
            XLS.HucreDegerYaz(sat, kol++, "İşlem Tarihi");
            XLS.HucreDegerYaz(sat, kol++, "Aktarım Tarihi");

            XLS.HucreDegerYaz(sat, kol++, "Miktar AB (TL)");
            XLS.HucreDegerYaz(sat, kol++, "Faiz AB (TL)");
            XLS.HucreDegerYaz(sat, kol++, "Toplam AB (TL)");
            XLS.HucreDegerYaz(sat, kol++, "Miktar Ulusal (TL)");
            XLS.HucreDegerYaz(sat, kol++, "Faiz Ulusal (TL)");
            XLS.HucreDegerYaz(sat, kol++, "Toplam Ulusal (TL)");
            XLS.HucreDegerYaz(sat, kol++, "Toplam (TL)");

            XLS.HucreDegerYaz(sat, kol++, "Miktar AB (EUR)");
            XLS.HucreDegerYaz(sat, kol++, "Faiz AB (EUR)");
            XLS.HucreDegerYaz(sat, kol++, "Toplam AB (EUR)");
            XLS.HucreDegerYaz(sat, kol++, "Miktar Ulusal (EUR)");
            XLS.HucreDegerYaz(sat, kol++, "Faiz Ulusal (EUR)");
            XLS.HucreDegerYaz(sat, kol++, "Toplam Ulusal (EUR)");
            XLS.HucreDegerYaz(sat, kol++, "Toplam (EUR)");
            sat++;
            foreach (DosyaAB item in oa.objeler)
            {
                foreach (BorcHareket bh in item.borcHareket)
                {
                    if (bh.kayitDurum != DosyaABParca.ENUMKayitDurum.ONAYLI) continue;
                    if (BorcHareket.FaizIslemiMi(bh.tur)) continue;

                    if (DosyaAB.TarihIcindeMi(bh.tarih, dtBas, dtSon) || DosyaAB.TarihIcindeMi(bh.aktarimTarihi, dtBas, dtSon))
                    {
                        kol = 0;
                        XLS.HucreDegerYaz(sat, kol++, item.defterSirasi);
                        XLS.HucreDegerYaz(sat, kol++, item.borcluAd);
                        XLS.HucreDegerYaz(sat, kol++, item.tedbir);
                        XLS.HucreDegerYaz(sat, kol++, item.basvuruKayitNumarasi);
                        XLS.HucreDegerYaz(sat, kol++, item.sozlesmeNumarasi);
                        XLS.HucreDegerYaz(sat, kol++, item.borcTuruAd);
                        XLS.HucreDegerYaz(sat, kol++, item.geriAlimKarariTarihiDT);

                        XLS.HucreDegerYaz(sat, kol++, bh.turAd);
                        XLS.HucreDegerYaz(sat, kol++, bh.tarihDT);
                        if ((bh.tur == BorcHareket.ENUMHareketTur.Silme || bh.tur == BorcHareket.ENUMHareketTur.Odeme || bh.tur == BorcHareket.ENUMHareketTur.TaksitOdeme) && !TNSDateTime.IsNull(bh.aktarimTarihi))
                            XLS.HucreDegerYaz(sat, kol++, bh.aktarimTarihiDT);
                        else
                            kol++;

                        XLS.HucreDegerYaz(sat, kol++, bh.miktarAB);
                        XLS.HucreDegerYaz(sat, kol++, bh.faizAB);
                        XLS.HucreDegerYaz(sat, kol++, bh.toplamAB);

                        XLS.HucreDegerYaz(sat, kol++, bh.miktarUlusal);
                        XLS.HucreDegerYaz(sat, kol++, bh.faizUlusal);
                        XLS.HucreDegerYaz(sat, kol++, bh.toplamUlusal);
                        XLS.HucreDegerYaz(sat, kol++, bh.toplamAB + bh.toplamUlusal);

                        XLS.HucreDegerYaz(sat, kol++, bh.miktarABEUR);
                        XLS.HucreDegerYaz(sat, kol++, bh.faizABEUR);
                        XLS.HucreDegerYaz(sat, kol++, bh.miktarABEUR + bh.faizABEUR);

                        XLS.HucreDegerYaz(sat, kol++, bh.miktarUlusalEUR);
                        XLS.HucreDegerYaz(sat, kol++, bh.faizUlusalEUR);
                        XLS.HucreDegerYaz(sat, kol++, bh.miktarUlusalEUR + bh.faizUlusalEUR);
                        XLS.HucreDegerYaz(sat, kol++, bh.miktarABEUR + bh.faizABEUR + bh.miktarUlusalEUR + bh.faizUlusalEUR);

                        sat++;
                    }
                }
            }

            XLS.HucreFormatla(1, 6, sat, 6, "dd/mm/yyyy");
            XLS.HucreFormatla(1, 8, sat, 9, "dd/mm/yyyy");

            XLS.HucreFormatla(1, 10, sat, 23, "#,##0.00");
        }

        private void RaporAl6404(ObjectArray oa, TNSDateTime faizHesabiSonTarihi)
        {
            XLS.HucreAdBulYaz("HazirlamaTarihi", faizHesabiSonTarihi.ToString());
            DateTime dtBas, dtSon;
            XLS.HucreAdBulYaz("Donem", YilTarihleriBul(faizHesabiSonTarihi, out dtBas, out dtSon, true));
            YilTarihleriBul(faizHesabiSonTarihi, out dtBas, out dtSon, false);
            XLS.HucreAdBulYaz("KayitSayisi1", oa.ObjeSayisi);
            ToplamAl toplamAl = new ToplamAl(7, 5);
            DonemleriToplaDetay(oa, dtBas, dtSon, toplamAl, 2, 3, false, true, true, true);
            for (int i = 0; i < tedbirler.Length; i++)
            {
                for (int j = 4; j < 5; j++)
                {
                    XLS.HucreAdBulYaz("DO_" + (j + 1).ToString() + "_" + tedbirler[i], toplamAl.toplam[i, j]);
                }
            }
        }

        private void RaporAl6403(ObjectArray oa, TNSDateTime faizHesabiSonTarihi)
        {
            XLS.HucreAdBulYaz("HazirlamaTarihi", faizHesabiSonTarihi.ToString());
            DateTime dtBas, dtSon;
            XLS.HucreAdBulYaz("Donem", DonemTarihleriBul(faizHesabiSonTarihi, out dtBas, out dtSon, true));
            DonemTarihleriBul(faizHesabiSonTarihi, out dtBas, out dtSon, false);
            XLS.HucreAdBulYaz("KayitSayisi1", oa.ObjeSayisi);
            ToplamAl toplamAl = new ToplamAl(7, 5);
            //DonemleriToplaDetay(oa, dtBas, dtSon, toplamAl, 3, 3, false);
            //dab.dosyaAnahtar = 0;
            //dab.geriAlimKarariTarihAralik = dab.islemTarihAralik;
            //dab.islemTarihAralik = null;
            //dab.islemliste = null;
            //for (int i = 0; i < tedbirler.Length; i++)
            //{
            //    XLS.HucreAdBulYaz("DO_" + tedbirler[i], toplamAl.toplam[i, 4]);
            //}

            oa = servis.ListeleDosyaAB(kullanan, dab);
            XLS.HucreAdBulYaz("KayitSayisi2", oa.ObjeSayisi);
            ToplamAl toplamAl1 = new ToplamAl(7, 5);
            ToplamAl toplamAl2 = new ToplamAl(7, 5);
            DonemleriToplaDetay(oa, dtBas, dtSon, toplamAl1, 3, 3, false, true, true, true);
            DonemleriToplaDetay(oa, dtBas, dtSon, toplamAl2, 2, 3, false, true, true, true);
            for (int i = 0; i < tedbirler.Length; i++)
            {
                XLS.HucreAdBulYaz("GA_T_" + tedbirler[i], toplamAl1.toplam[i, 4]);
                XLS.HucreAdBulYaz("GA_A_" + tedbirler[i], toplamAl2.toplam[i, 4]);
            }
        }

        private void RaporAl6405(ObjectArray oa, TNSDateTime faizHesabiSonTarihi)
        {
            XLS.HucreAdBulYaz("HazirlamaTarihi", faizHesabiSonTarihi.ToString());
            DateTime dtBas, dtSon;
            XLS.HucreAdBulYaz("Donem", AyTarihleriBul(faizHesabiSonTarihi, out dtBas, out dtSon, true));
            AyTarihleriBul(faizHesabiSonTarihi, out dtBas, out dtSon, false);
            XLS.HucreAdBulYaz("KayitSayisi1", oa.ObjeSayisi);
            ToplamAl toplamAl1 = new ToplamAl(7, 5);
            ToplamAl toplamAl2 = new ToplamAl(7, 5);
            DonemleriToplaDetay(oa, dtBas, dtSon, toplamAl1, 1, 3, true, true, false, false);
            DonemleriToplaDetay(oa, dtBas, dtSon, toplamAl2, 2, 3, true, true, false, false);
            for (int i = 0; i < tedbirler.Length; i++)
            {
                XLS.HucreAdBulYaz("DO_" + tedbirler[i] + "_U", toplamAl1.toplam[i, 4]);
                XLS.HucreAdBulYaz("DO_" + tedbirler[i] + "_AB", toplamAl2.toplam[i, 4]);
            }

            dab.dosyaAnahtar = 0;
            dab.geriAlimKarariTarihAralik = null;
            dab.islemTarihAralik = null;
            dab.islemliste = null;
            oa = servis.ListeleDosyaAB(kullanan, dab);
            XLS.HucreAdBulYaz("KayitSayisi2", oa.ObjeSayisi);
            toplamAl1 = new ToplamAl(7, 5);
            toplamAl2 = new ToplamAl(7, 5);
            DonemleriToplaAna(oa, DateTime.MinValue, dtSon, toplamAl1, 1, 3, true, 2);
            DonemleriToplaAna(oa, DateTime.MinValue, dtSon, toplamAl2, 2, 3, true, 2);
            for (int i = 0; i < tedbirler.Length; i++)
            {
                XLS.HucreAdBulYaz("KALAN_" + tedbirler[i] + "_U", toplamAl1.toplam[i, 4]);
                XLS.HucreAdBulYaz("KALAN_" + tedbirler[i] + "_AB", toplamAl2.toplam[i, 4]);
            }

            dab.dosyaAnahtar = 0;
            XLS.HucreAdBulYaz("Donem2", YilTarihleriBul(faizHesabiSonTarihi, out dtBas, out dtSon, true));
            dab.onBorcTarihAralik = YilTarihleriBul(faizHesabiSonTarihi, out dtBas, out dtSon, false);
            dab.islemTarihAralik = null;
            dab.islemliste = null;
            oa = servis.ListeleDosyaAB(kullanan, dab);
            XLS.HucreAdBulYaz("KayitSayisi3", oa.ObjeSayisi);
            toplamAl1 = new ToplamAl(7, 5);
            toplamAl2 = new ToplamAl(7, 5);
            DonemleriToplaAna(oa, dtBas, dtSon, toplamAl1, 1, 3, true, 2);
            DonemleriToplaAna(oa, dtBas, dtSon, toplamAl2, 2, 3, true, 2);
            for (int i = 0; i < tedbirler.Length; i++)
            {
                XLS.HucreAdBulYaz("OB_" + tedbirler[i] + "_U", toplamAl1.toplam[i, 4]);
                XLS.HucreAdBulYaz("OB_" + tedbirler[i] + "_AB", toplamAl2.toplam[i, 4]);
            }
        }


        private void DonemleriToplaAna(ObjectArray oa, DateTime dtBas, DateTime dtSon, ToplamAl toplamAl, int ulAbT, int ana_Faiz_Top, bool tl, int G_K_O)
        {
            foreach (DosyaAB item in oa.objeler)
            {
                if (G_K_O == 2)
                {
                    TNSDateTime tnsdtSon = new TNSDateTime(dtSon.AddDays(-1));
                    item.IslemToplamBul(tnsdtSon, -1, true, null, onayTarihi, -1);
                }
                int tedbirYer = TedbirYerBul(item.tedbir);
                if (tedbirYer < 0) continue;
                if (DosyaAB.TarihIcindeMi(item.geriAlimKarariTarihi, dtBas, dtSon))
                {
                    double tut = 0;
                    if (G_K_O == 1)
                        tut = item.MiktarBul(ulAbT, ana_Faiz_Top, tl);
                    else if (G_K_O == 2)
                        tut = item.KalanBul(ulAbT, ana_Faiz_Top, tl);
                    else if (G_K_O == 3)
                        tut = item.OnBorcBul(ulAbT, ana_Faiz_Top, tl);
                    if (tut > 0)
                    {
                        toplamAl.DegerArttir(tedbirYer, 4, tut);
                        int donem = DonemBul(item.geriAlimKarariTarihi, true);
                        if (donem >= 1 && donem <= 4)
                            toplamAl.DegerArttir(tedbirYer, donem - 1, tut);
                    }
                }
            }
        }

        private void DonemleriToplaDetay(ObjectArray oa, DateTime dtBas, DateTime dtSon, ToplamAl toplamAl, int UlAbT, int ana_Faiz_Top, bool tl, bool odemeTuruIslem, bool silmeTuruIslem, bool aktarmaTarihiKullan)
        {
            foreach (DosyaAB item in oa.objeler)
            {
                int tedbirYer = TedbirYerBul(item.tedbir);
                if (tedbirYer < 0) continue;
                foreach (BorcHareket bh in item.borcHareket)
                {
                    if (bh.kayitDurum != DosyaABParca.ENUMKayitDurum.ONAYLI) continue;
                    bool arananIslem = false;
                    bool silmeVar = false;
                    TNSDateTime trh;
                    if (aktarmaTarihiKullan && (bh.OdemeTuruIslem || bh.tur == BorcHareket.ENUMHareketTur.FazlaOdemeAktarim || bh.tur == BorcHareket.ENUMHareketTur.Silme))
                    {
                        if (bh.OdemeTuruIslem && TNSDateTime.IsNull(bh.aktarimTarihi))
                        {
                            BorcHareket bhSil;
                            silmeVar = item.SilmeVarMi(bh.tarih, out bhSil);
                            if (silmeVar)
                            {
                                trh = bh.tarih;
                            }
                            else
                                trh = bh.aktarimTarihi;
                        }
                        else
                            trh = bh.aktarimTarihi;
                    }
                    else
                        trh = bh.tarih;

                    if ((odemeTuruIslem && bh.OdemeTuruIslem) || (silmeTuruIslem && (bh.tur == BorcHareket.ENUMHareketTur.FazlaOdemeAktarim || bh.tur == BorcHareket.ENUMHareketTur.Silme)))
                        arananIslem = DosyaAB.TarihIcindeMi(trh, dtBas, dtSon);

                    if (arananIslem)
                    {
                        int donem = DonemBul(trh, true);
                        if (donem >= 1 && donem <= 4)
                        {
                            double tut;
                            if (bh.tur == BorcHareket.ENUMHareketTur.FazlaOdemeAktarim)
                            {
                                if (tl)
                                    tut = bh.aktarimTutari;
                                else
                                    tut = bh.aktarimTutariEUR;
                            }
                            else
                            {
                                int ana_Faiz_TopIc;
                                int UlAbTIc;
                                if (bh.tur == BorcHareket.ENUMHareketTur.Silme)
                                {
                                    ana_Faiz_TopIc = 1;
                                    UlAbTIc = 2;
                                }
                                else
                                {
                                    ana_Faiz_TopIc = ana_Faiz_Top;
                                    UlAbTIc = UlAbT;
                                }
                                if (silmeVar)
                                {
                                    if (UlAbTIc == 2)
                                        tut = 0;
                                    else
                                    {
                                        int bol = 1;
                                        if (item.borcTuru == DosyaAB.BorcTurleri.FazlaOdeme)
                                        {
                                            bol = 4;
                                        }
                                        tut = OrtakFonksiyonlar.KurusYap(bh.MiktarBul(1, ana_Faiz_TopIc, tl) / bol);
                                    }
                                }
                                else
                                    tut = bh.MiktarBul(UlAbTIc, ana_Faiz_TopIc, tl);
                            }
                            toplamAl.DegerArttir(tedbirYer, donem - 1, tut);
                            toplamAl.DegerArttir(tedbirYer, 4, tut);
                        }
                    }
                }
            }
        }

        private int TedbirYerBul(string tedbir)
        {
            return Array.IndexOf(tedbirler, tedbir);
        }

        static string[] tedbirler = { "101", "102", "103", "201", "202", "302", "501" };
        public static string DonemTarihleriBul(TNSDateTime faizHesabiSonTarihi, out DateTime dtBas, out DateTime dtSon, bool birGunDus)
        {
            int ay = faizHesabiSonTarihi.Ay;
            int donemBasi = DonemBul(faizHesabiSonTarihi, false);

            try
            {
                dtBas = new DateTime(faizHesabiSonTarihi.Yil, donemBasi, 1);
                dtSon = dtBas.AddMonths(3);
                if (birGunDus) dtSon = dtSon.AddDays(-1);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + string.Format(" (Gelen tarih:{0}, bulunan ay:{1})", faizHesabiSonTarihi.ToString(), donemBasi));
            }
            return dtBas.ToShortDateString() + " - " + dtSon.ToShortDateString();
        }

        public static string AyTarihleriBul(TNSDateTime faizHesabiSonTarihi, out DateTime dtBas, out DateTime dtSon, bool birGunDus)
        {
            int ay = faizHesabiSonTarihi.Ay;
            try
            {
                dtBas = new DateTime(faizHesabiSonTarihi.Yil, ay, 1);
                dtSon = dtBas.AddMonths(1);
                if (birGunDus) dtSon = dtSon.AddDays(-1);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + string.Format(" (Gelen tarih:{0}, bulunan ay:{1})", faizHesabiSonTarihi.ToString(), ay));
            }
            return dtBas.ToShortDateString() + " - " + dtSon.ToShortDateString();
        }

        public static string YilTarihleriBul(TNSDateTime faizHesabiSonTarihi, out DateTime dtBas, out DateTime dtSon, bool birGunDus)
        {
            try
            {
                dtBas = new DateTime(faizHesabiSonTarihi.Yil, 1, 1);
                dtSon = dtBas.AddYears(1);
                if (birGunDus) dtSon = dtSon.AddDays(-1);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + string.Format(" (Gelen tarih:{0})", faizHesabiSonTarihi.ToString()));
            }
            return dtBas.ToShortDateString() + " - " + dtSon.ToShortDateString();
        }

        public static int DonemBul(TNSDateTime faizHesabiSonTarihi, bool donem)
        {
            int ay = faizHesabiSonTarihi.Ay;
            if (donem)
                return ((ay - 1) / 3) + 1;
            else
                return (((ay - 1) / 3) * 3) + 1;
        }

        private void RaporAl6807()
        {
            XLS.HucreAdBulYaz("IpardVer", dab.iPARDDonemiAd);
            //XLS.HucreAdBulYaz("geriAlimTarihi", dab.islemTarihiDT.ToString());
            int sayfaNo1 = 0, sat1 = 0, sut1 = 0, sat2 = 0, sut2 = 0;
            int islemSatir = baslaSatir;
            int odeSay = 0;
            int odeSatSay = 0;
            List<int> satirNolar = new List<int>();
            foreach (BorcHareket odemeHar in dab.borcHareket)
            {
                if (odemeHar.kayitDurum != DosyaABParca.ENUMKayitDurum.ONAYLI) continue;
                if (!odemeHar.OdemeTuruIslem) continue;
                if (odemeHar.tarihDT > dab.islemTarihiDT) continue;
                if (odeSay == 0)
                {
                    XLS.HucreAdAdresCoz("FormatSatirOdeme", ref sayfaNo1, ref sat1, ref sut1, ref sat2, ref sut2);
                    int silSatSay = sat2 - sat1 + 1;
                    XLS.SatirAc(islemSatir, silSatSay);
                    XLS.HucreKopyala(sat1, sut1, sat2, sut2, islemSatir, sut1);
                    islemSatir += silSatSay;
                    XLS.HucreAdAdresCoz("FormatSatirOdemeBilgi", ref sayfaNo1, ref sat1, ref sut1, ref sat2, ref sut2);
                    odeSatSay = sat2 - sat1 + 1;
                }
                XLS.SatirAc(islemSatir, odeSatSay);
                XLS.HucreKopyala(sat1, sut1, sat2, sut2, islemSatir, sut1);
                odeSay++;
                XLS.HucreDegerYaz(islemSatir, 0, "'" + odeSay + ".");
                XLS.HucreDegerYaz(islemSatir + 2, 2, DovizKuruBul(servis, kullanan, new TNSDateTime(odemeHar.tarihDT.AddMonths(1))));
                XLS.HucreDegerYaz(islemSatir + 2, 4, odemeHar.miktarAB);
                XLS.HucreDegerYaz(islemSatir + 2, 5, odemeHar.faizAB);
                XLS.HucreDegerYaz(islemSatir + 2, 7, odemeHar.miktarUlusal);
                XLS.HucreDegerYaz(islemSatir + 2, 8, odemeHar.faizUlusal);
                satirNolar.Add(islemSatir + 5);
                XLS.HucreDegerYaz(islemSatir + 4, 2, odemeHar.dovizKuru);
                XLS.HucreDegerYaz(islemSatir + 7, 1, "Date: " + odemeHar.tarih.ToString());
                if (dab.iPARDDonemi == 3)
                {
                    XLS.HucreDegerYaz(islemSatir + 6, 1, "To: AB IPARD III Recovery-Refund account");
                    XLS.HucreKopyala(islemSatir + 6, 4, islemSatir + 6, 5, islemSatir + 6, 7);
                    XLS.SatirGizle(islemSatir + 8, islemSatir + 9, true);
                    int yuk = XLS.SatirYukseklikAl(islemSatir + 6);
                    XLS.SatirYukseklikAyarla(islemSatir + 6, islemSatir + 7, yuk * 2);
                }

                islemSatir += odeSatSay;
            }

            if (odeSay != 0)
            {
                XLS.HucreAdAdresCoz("FormatSatirSonDurum", ref sayfaNo1, ref sat1, ref sut1, ref sat2, ref sut2);
                int silSatSay = sat2 - sat1 + 1;
                XLS.SatirAc(islemSatir, silSatSay);
                XLS.HucreKopyala(sat1, sut1, sat2, sut2, islemSatir, sut1);
                XLS.HucreDegerYaz(islemSatir + 3, 4, dab.islemYapilanMiktarAB);
                XLS.HucreDegerYaz(islemSatir + 3, 5, dab.islemYapilanFaizMiktarAB);
                XLS.HucreDegerYaz(islemSatir + 3, 7, dab.islemYapilanMiktarUlusal);
                XLS.HucreDegerYaz(islemSatir + 3, 8, dab.islemYapilanFaizMiktarUlusal);

                XLS.HucreFormulYaz(islemSatir + 4, 4, ToplamOl(satirNolar, HarfOl(4)));
                XLS.HucreFormulYaz(islemSatir + 4, 5, ToplamOl(satirNolar, HarfOl(5)));
                XLS.HucreFormulYaz(islemSatir + 4, 7, ToplamOl(satirNolar, HarfOl(7)));
                XLS.HucreFormulYaz(islemSatir + 4, 8, ToplamOl(satirNolar, HarfOl(8)));

                XLS.HucreDegerYaz(islemSatir + 5, 4, dab.kalanMiktarAB);
                XLS.HucreDegerYaz(islemSatir + 5, 5, dab.kalanFaizMiktarAB);
                XLS.HucreDegerYaz(islemSatir + 5, 7, dab.kalanMiktarUlusal);
                XLS.HucreDegerYaz(islemSatir + 5, 8, dab.kalanFaizMiktarUlusal);
                XLS.HucreDegerYaz(islemSatir + 6, 2, dab.geriAlimDovizKuru);
                islemSatir += silSatSay;
            }

            BorcHareket silmeHareket;
            dab.SilmeVarMi(dab.islemTarihi, out silmeHareket);
            if (silmeHareket != null)
            {
                XLS.HucreAdAdresCoz("FormatSatirSil", ref sayfaNo1, ref sat1, ref sut1, ref sat2, ref sut2);
                int silSatSay = sat2 - sat1 + 1;
                XLS.SatirAc(islemSatir, silSatSay);
                XLS.HucreKopyala(sat1, sut1, sat2, sut2, islemSatir, sut1);
                XLS.HucreDegerYaz(islemSatir + 0, 9, dab.ongorulenSilinmeTarihiDT);
                XLS.HucreDegerYaz(islemSatir + 3, 4, silmeHareket.miktarAB);
                XLS.HucreDegerYaz(islemSatir + 3, 5, silmeHareket.faizAB);
                XLS.HucreDegerYaz(islemSatir + 4, 1, "on:" + silmeHareket.aktarimTarihi.ToString());
                XLS.HucreDegerYaz(islemSatir + 4, 2, dab.geriAlimDovizKuru);
                islemSatir += silSatSay;
            }
        }

        private string ToplamOl(List<int> satirNolar, string kolAd)
        {
            string cevap = "";
            foreach (int item in satirNolar)
            {
                if (cevap == "") cevap = "=";
                else cevap += "+";
                cevap += kolAd + (item + 1).ToString();
            }
            return cevap;
        }

        private string HarfOl(int kolonNo)
        {
            return Convert.ToString((char)(65 + kolonNo));
        }

        private void RaporAl6305(ObjectArray oa, bool tl, int detayli1Ozet2Ozet3, TNSDateTime islemTarihi, TNSDateTime sonTarih)
        {
            string cins;
            if (tl) cins = "TL";
            else cins = "EUR";
            XLS.HucreAdBulYaz("DovizCinsi", cins + " " + "cinsinde");
            XLS.HucreAdBulYaz("RaporTarihi", islemTarihi.ToString() + " " + "itibariyle");
            ToplamAl toplamAl = new ToplamAl(2, 100);
            toplamAl.SatirSifirla(0);
            if (detayli1Ozet2Ozet3 != 1)
            {
                TarihYaz("baslaTar", islemTarihi.Oku());
                TarihYaz("bitisTar", sonTarih.Oku());
            }
            foreach (DosyaAB item in oa.objeler)
            {
                if (detayli1Ozet2Ozet3 == 1)
                {
                    if (item.OnayliSay(item.OnBorcBilgileri) == 0) continue;
                    //item.IslemToplamBul(islemTarihi, -1, true, null, onayTarihi, -1);
                    //if (TNSDateTime.IsNull(item.onBorcTarihi)) continue;
                    if (item.onBorcTarihiDT > islemTarihi.Oku()) continue;
                    bool yazildi = false;
                    BorcHareket faizHareket = null;
                    BorcHareket silmeHareket = null;
                    double kalan = item.kalanToplamToplam;
                    TNSDateTime sonDt = null;
                    TNSDateTime yazilanOnayTarih = new TNSDateTime();
                    for (int i = 0; i < item.borcHareket.Count; i++)
                    {
                        BorcHareket aktif = (BorcHareket)item.borcHareket[i];
                        if (!TNSDateTime.IsNull(aktif.onayTarihi) && !TNSDateTime.IsNull(onayTarihi) && aktif.onayTarihi.Oku() > onayTarihi.Oku()) continue;

                        if (aktif.tur == BorcHareket.ENUMHareketTur.Taksit || aktif.tur == BorcHareket.ENUMHareketTur.OdeneniAktarma)
                            continue;
                        if (aktif.tarihDT > islemTarihi.Oku()) continue;

                        if (aktif.kayitDurum != DosyaABParca.ENUMKayitDurum.ONAYLI) continue;

                        sonDt = aktif.tarih;

                        if (aktif.FaizIslemi)
                        {
                            faizHareket = aktif;
                        }
                        else if (aktif.tur == BorcHareket.ENUMHareketTur.Silme || aktif.tur == BorcHareket.ENUMHareketTur.FazlaOdemeAktarim)
                        {
                            silmeHareket = aktif;
                        }
                        else
                        {
                            if (SatirYaz(item, false, faizHareket, silmeHareket, aktif, tl, true, kalan, islemTarihi, null, yazilanOnayTarih))
                            {
                                silmeHareket = null;
                                if (aktif.tur == BorcHareket.ENUMHareketTur.Duzeltme)
                                    faizHareket = null;
                                yazildi = true;
                            }
                        }
                    }
                    if (TNSDateTime.IsNull(sonDt))
                    {
                        if (sonDt == null) sonDt = new TNSDateTime();
                        sonDt.Yaz(item.geriAlimKarariTarihi);
                        if (TNSDateTime.IsNull(sonDt))
                            sonDt.Yaz(item.onBorcTarihi);
                    }

                    if (yazildi)
                    {
                        foreach (GeriAlimKarari gak in item.GAKBilgileri)
                        {
                            if (gak.onayTarihiDT > islemTarihi.Oku()) continue;
                            if (item.geriAlimKarariOnayTarihi.Oku() > gak.onayTarihiDT && item.FarkliMi(gak))
                            {
                                yazildi = false;
                                break;
                            }
                        }
                    }

                    if (!yazildi || kalan > 0)
                    {
                        SatirYaz(item, true, faizHareket, silmeHareket, null, tl, true, kalan, islemTarihi, sonDt, yazilanOnayTarih);
                    }
                }
                else if (detayli1Ozet2Ozet3 == 2)
                    SatirYazOzet(item, tl, islemTarihi, sonTarih);
                else if (detayli1Ozet2Ozet3 == 3)
                {
                    SatirYazBorcRapor(item, tl, islemTarihi, sonTarih, toplamAl);
                }
            }
            XLS.SatirYukseklikAyarla(baslaSatir, sat, -1, 7, satirYuk);
            if (!tl && detayli1Ozet2Ozet3 == 1)
            {
                XLS.SutunGenislikAyarla(baslaSutun + 10, baslaSutun + 10, 10);
                XLS.SutunGenislikAyarla(baslaSutun + 18, baslaSutun + 18, 10);
                XLS.SutunGenislikAyarla(baslaSutun + 28, baslaSutun + 28, 10);
                XLS.SutunGenislikAyarla(baslaSutun + 36, baslaSutun + 36, 10);
                XLS.SutunGenislikAyarla(baslaSutun + 43, baslaSutun + 43, 10);
                XLS.SutunGenislikAyarla(baslaSutun + 49, baslaSutun + 49, 10);
            }

            if (detayli1Ozet2Ozet3 == 2 && dab.iPARDDonemi == 1)
            {
                int[] silKolon = { 10, 11, 13, 14, 16, 17, 18, 20, 21, 23, 24, 25, 27, 28, 30, 31, 32, 34, 35, 37, 38, 39, 40, 42, 43, 45, 46, 47, 48, 50, 51, 53, 54, 55, 57, 58, 60, 61, 62, 64, 65, 67 };
                for (int i = silKolon.Length - 1; i >= 0; i--)
                {
                    XLS.SutunGenislikAyarla(baslaSutun + silKolon[i], baslaSutun + silKolon[i], 0);
                }
            }

            if (detayli1Ozet2Ozet3 == 3)
            {
                XLS.HucreKopyala(formatSatir, formatSutun, formatSatir2, formatSutun2, sat, baslaSutun);
                XLS.HucreBirlestir(sat, 0, sat, 8);
                XLS.DuseyHizala(sat, 0, 2);
                XLS.KoyuYap(sat, 0, sat, 24, true);
                XLS.HucreDegerYaz(sat, 0, "T O T A L");
                SatirYazBorcRaporToplamAl(XLS, sat, baslaSutun + 9, toplamAl, 0);
            }
        }

        private static DateTime FaizGondermemeKararTarihi = new DateTime(2018, 3, 12);

        private bool SatirYaz(DosyaAB item, bool son, BorcHareket bhFaiz, BorcHareket bhSilme, BorcHareket bhIslem, bool tl, bool detayli, double kalan, TNSDateTime islemTarihi, TNSDateTime sonDt, TNSDateTime onayTarihiIc)
        {
            string tarihler;

            OnayTarihBul(onayTarihiIc, bhFaiz, bhSilme, bhIslem);
            if (TNSDateTime.IsNull(onayTarihiIc))
                onayTarihiIc.Yaz(onayTarihi);

            item.IslemToplamBul(islemTarihi, -1, true, null, onayTarihiIc, -1);

            TNSDateTime sonIslemTarih = new TNSDateTime();
            int fark = 1;
            if (!(item.geriAlimSurecAsamasi == DosyaAB.ENUMDosyaDurum.KESINBORCIPTALI || item.geriAlimSurecAsamasi == DosyaAB.ENUMDosyaDurum.ONBORCIPTALI))
            {
                if ((item.geriAlimSurecAsamasi == DosyaAB.ENUMDosyaDurum.ONBORC || kalan > 0 || (item.GAKBilgileri.Count == 0 && item.OnBorcBilgileri.Count > 1)) && ((detayli && son) || !detayli))
                {
                    fark = 0;
                }
            }
            XLS.HucreKopyala(formatSatir + fark, formatSutun, formatSatir2 + fark, formatSutun2, sat, baslaSutun);
            XLS.HucreDegerYaz(sat, baslaSutun + 0, item.defterSirasi);
            XLS.HucreDegerYaz(sat, baslaSutun + 1, item.tertip);
            XLS.HucreDegerYaz(sat, baslaSutun + 2, item.borcluAd);
            XLS.HucreDegerYaz(sat, baslaSutun + 3, item.kimlikNo);
            XLS.HucreDegerYaz(sat, baslaSutun + 4, item.basvuruKayitNumarasi);
            XLS.HucreDegerYaz(sat, baslaSutun + 5, item.sozlesmeNumarasi);
            XLS.HucreDegerYaz(sat, baslaSutun + 6, item.borcTuruAd);
            XLS.HucreDegerYaz(sat, baslaSutun + 7, item.borcunKodu);
            XLS.HucreDegerYaz(sat, baslaSutun + 8, item.vakaNumarasi);
            TarihYaz(sat, baslaSutun + 9, item.onBorcTarihiDT);
            XLS.HucreDegerYaz(sat, baslaSutun + 10, item.onBorcSayi);

            XLS.HucreDegerYaz(sat, baslaSutun + 11, item.onBorcDovizKuru);

            XLS.HucreDegerYaz(sat, baslaSutun + 12, item.onBorcMiktarABKur(tl));
            XLS.HucreDegerYaz(sat, baslaSutun + 13, item.onBorcMiktarUlusalKur(tl));
            if (item.geriAlimKarariTarihiDT <= islemTarihi.Oku())
            {
                TarihYaz(sat, baslaSutun + 14, item.geriAlimKarariTarihiDT);
                XLS.HucreDegerYaz(sat, baslaSutun + 15, item.geriAlimSayi);
                TarihYaz(sat, baslaSutun + 16, item.bildirimTarihiDT);
                TarihYaz(sat, baslaSutun + 17, item.tesellumTarihiDT);
                TarihYaz(sat, baslaSutun + 18, item.odemeSonTarihiDT);
                XLS.HucreDegerYaz(sat, baslaSutun + 19, item.geriAlimDovizKuru);
                TarihYaz(sat, baslaSutun + 23, item.temerrutTarihiDT);
                if (item.borcTuru != DosyaAB.BorcTurleri.FazlaOdeme)
                    TarihYaz(sat, baslaSutun + 42, item.ongorulenSilinmeTarihiDT);
                if (detayli)
                {
                    if (bhFaiz != null)
                    {
                        item.IslemToplamBul(bhFaiz.tarih, son ? -1 : bhFaiz.yerNo, true, null, onayTarihi, -1);
                        TarihYaz(sat, baslaSutun + 24, item.islemTarihiDT);
                        XLS.HucreDegerYaz(sat, baslaSutun + 25, item.kalanFaizMiktarToplamKur(tl));
                        XLS.HucreDegerYaz(sat, baslaSutun + 26, item.kalanFaizMiktarABKur(tl));
                        XLS.HucreDegerYaz(sat, baslaSutun + 27, item.kalanFaizMiktarUlusalKur(tl));
                    }

                    if (bhSilme != null)
                    {
                        if (bhSilme.tur == BorcHareket.ENUMHareketTur.Silme)
                        {
                            int baslaKol = baslaSutun + 43;
                            XLS.HucreDegerYaz(sat, baslaKol++, bhSilme.karar);
                            XLS.HucreDegerYaz(sat, baslaKol++, bhSilme.dovizKuruIslem);
                            XLS.HucreDegerYaz(sat, baslaKol++, bhSilme.tarihDT);
                            XLS.HucreDegerYaz(sat, baslaKol++, bhSilme.miktarABKur(tl));
                            if (bhSilme.tarihDT < FaizGondermemeKararTarihi)
                                XLS.HucreDegerYaz(sat, baslaKol++, bhSilme.faizABKur(tl));
                        }
                        if (bhSilme.aktarimTarihiDT != DateTime.MinValue)
                        {
                            TarihYaz(sat, baslaSutun + 48, bhSilme.aktarimTarihiDT);
                            XLS.HucreDegerYaz(sat, baslaSutun + 49, bhSilme.aktarimTutariKur(tl));
                        }
                    }

                    if (bhIslem != null)
                    {
                        item.IslemToplamBul(bhIslem.tarih, bhIslem.yerNo, true, null, onayTarihiIc, -1);
                    }
                    XLS.HucreDegerYaz(sat, baslaSutun + 20, item.kalanMiktarToplamKur(tl));
                    XLS.HucreDegerYaz(sat, baslaSutun + 21, item.kalanMiktarABKur(tl));
                    XLS.HucreDegerYaz(sat, baslaSutun + 22, item.kalanMiktarUlusalKur(tl));
                }
                else
                {
                    XLS.HucreDegerYaz(sat, baslaSutun + 20, item.geriAlimMiktarToplamKur(tl));
                    XLS.HucreDegerYaz(sat, baslaSutun + 21, item.geriAlimMiktarABKur(tl));
                    XLS.HucreDegerYaz(sat, baslaSutun + 22, item.geriAlimMiktarUlusalKur(tl));
                    TarihYaz(sat, baslaSutun + 24, item.islemTarihiDT);
                    XLS.HucreDegerYaz(sat, baslaSutun + 25, item.faizMiktarToplamKur(tl));
                    XLS.HucreDegerYaz(sat, baslaSutun + 26, item.faizMiktarABKur(tl));
                    XLS.HucreDegerYaz(sat, baslaSutun + 27, item.faizMiktarUlusalKur(tl));
                }

                if (bhIslem != null)
                {
                    int baslaKol = -1;
                    if (bhIslem.OdemeTuruIslem) baslaKol = baslaSutun + 28;
                    else if (bhIslem.tur == BorcHareket.ENUMHareketTur.Duzeltme) baslaKol = baslaSutun + 35;
                    if (baslaKol > 0)
                    {
                        TarihYaz(sat, baslaKol++, bhIslem.tarihDT);
                        if (bhIslem.tur == BorcHareket.ENUMHareketTur.Duzeltme)
                            XLS.HucreDegerYaz(sat, baslaKol++, bhIslem.karar);
                        XLS.HucreDegerYaz(sat, baslaKol++, bhIslem.dovizKuruIslem);
                        if (bhIslem.OdemeTuruIslem)
                            XLS.HucreDegerYaz(sat, baslaKol++, bhIslem.toplamToplamKur(tl));
                        XLS.HucreDegerYaz(sat, baslaKol++, bhIslem.miktarABKurSil(tl));
                        XLS.HucreDegerYaz(sat, baslaKol++, bhIslem.faizABKurSil(tl));
                        XLS.HucreDegerYaz(sat, baslaKol++, bhIslem.miktarUlusalKurSil(tl));
                        XLS.HucreDegerYaz(sat, baslaKol++, bhIslem.faizUlusalKurSil(tl));
                        if (bhIslem.aktarimTarihiDT != DateTime.MinValue)
                        {
                            TarihYaz(sat, baslaSutun + 48, bhIslem.aktarimTarihiDT);
                            XLS.HucreDegerYaz(sat, baslaSutun + 49, bhIslem.aktarimTutariKur(tl));
                        }
                    }
                }
                else
                {
                    if (!detayli)
                    {
                        MiktarIslem odemeMik = item.ToplamAl(new BorcHareket.ENUMHareketTur[] { BorcHareket.ENUMHareketTur.Odeme, BorcHareket.ENUMHareketTur.Mahsup, BorcHareket.ENUMHareketTur.TaksitOdeme, BorcHareket.ENUMHareketTur.Terkin, BorcHareket.ENUMHareketTur.Iade }, DateTime.MinValue, DateTime.MaxValue, false, out tarihler, sonIslemTarih, false, false);

                        XLS.HucreDegerYaz(sat, 29, odemeMik.kur);
                        XLS.HucreDegerYaz(sat, 30, odemeMik.toplamToplamKur(tl));
                        XLS.HucreDegerYaz(sat, 31, odemeMik.miktarABKur(tl));
                        XLS.HucreDegerYaz(sat, 32, odemeMik.faizABKur(tl));
                        XLS.HucreDegerYaz(sat, 33, odemeMik.miktarUlusalKur(tl));
                        XLS.HucreDegerYaz(sat, 34, odemeMik.faizUlusalKur(tl));

                        odemeMik = item.ToplamAl(new BorcHareket.ENUMHareketTur[] { BorcHareket.ENUMHareketTur.Duzeltme }, DateTime.MinValue, DateTime.MaxValue, false, out tarihler, sonIslemTarih, false, false);
                        XLS.HucreDegerYaz(sat, 37, odemeMik.kur);
                        XLS.HucreDegerYaz(sat, 38, odemeMik.miktarABKur(tl));
                        XLS.HucreDegerYaz(sat, 39, odemeMik.faizABKur(tl));
                        XLS.HucreDegerYaz(sat, 40, odemeMik.miktarUlusalKur(tl));
                        XLS.HucreDegerYaz(sat, 41, odemeMik.faizUlusalKur(tl));

                        odemeMik = item.ToplamAl(new BorcHareket.ENUMHareketTur[] { BorcHareket.ENUMHareketTur.Silme }, DateTime.MinValue, DateTime.MaxValue, false, out tarihler, sonIslemTarih, false, false);
                        XLS.HucreDegerYaz(sat, 44, odemeMik.kur);
                        XLS.HucreDegerYaz(sat, 46, odemeMik.miktarABKur(tl));
                        XLS.HucreDegerYaz(sat, 47, odemeMik.faizABKur(tl));
                    }
                }

                if (detayli)
                {
                    BorcHareket sonIslem = BuyukBul(bhFaiz, bhSilme);
                    sonIslem = BuyukBul(sonIslem, bhIslem);
                    if (sonIslem != null)
                        item.IslemToplamBul(sonIslem.tarih, son ? -1 : sonIslem.yerNo + 1, true, null, son ? onayTarihi : onayTarihiIc, -1);
                    else
                    {
                        if (TNSDateTime.IsNull(sonDt))
                            sonDt = item.geriAlimKarariTarihi;
                        if (TNSDateTime.IsNull(sonDt))
                            sonDt = item.onBorcTarihi;
                        item.IslemToplamBul(sonDt, -1, true, null, son ? onayTarihi : onayTarihiIc, -1);
                    }
                }

                XLS.HucreDegerYaz(sat, baslaSutun + 50, item.geriAlimDovizKuru);
                XLS.HucreDegerYaz(sat, baslaSutun + 51, item.kalanToplamToplamKur(tl));

                XLS.HucreDegerYaz(sat, baslaSutun + 52, item.kalanMiktarABKurSil(tl));
                XLS.HucreDegerYaz(sat, baslaSutun + 53, item.kalanFaizMiktarABKurSil(tl));
                XLS.HucreDegerYaz(sat, baslaSutun + 54, item.kalanMiktarUlusalKurSil(tl));
                XLS.HucreDegerYaz(sat, baslaSutun + 55, item.kalanFaizMiktarUlusalKurSil(tl));
                TarihYaz(sat, baslaSutun + 56, item.yasalTakipTarihiDT);
                XLS.HucreDegerYaz(sat, baslaSutun + 57, (int)item.geriAlimSurecAsamasi);
                XLS.HucreDegerYaz(sat, baslaSutun + 58, item.D1Donem);

                if (!detayli || son)
                    XLS.HucreDegerYaz(sat, baslaSutun + 59, item.yorumlarTR);
            }
            sat++;
            return true;
        }

        private void OnayTarihBul(TNSDateTime ic, BorcHareket bhFaiz, BorcHareket bhSilme, BorcHareket bhIslem)
        {
            //ic.Yaz(DateTime.MinValue);
            TarihAta(ic, bhFaiz);
            TarihAta(ic, bhSilme);
            TarihAta(ic, bhIslem);
        }

        private void TarihAta(TNSDateTime ic, BorcHareket bhFaiz)
        {
            if (bhFaiz != null && !TNSDateTime.IsNull(bhFaiz.onayTarihi))
                if (ic.Oku() < bhFaiz.onayTarihi.Oku())
                    ic.Yaz(bhFaiz.onayTarihi);
        }

        private bool SatirYazOzet(DosyaAB item, bool tl, TNSDateTime islemTarih, TNSDateTime sonTarih)
        {
            string tarihler;
            TNSDateTime sonIslemTarih = new TNSDateTime();
            TNSDateTime sonGun = new TNSDateTime(islemTarih.Oku().AddDays(-1));
            item.IslemToplamBul(sonGun, -1, true, null, onayTarihi, -1);
            bool yaz = false;
            if (item.kalanMiktarToplamKur(true) > 0)
                yaz = true;
            else if (item.geriAlimKarariTarihiDT >= islemTarih.Oku() && item.geriAlimKarariTarihiDT <= sonTarih.Oku())
                yaz = true;
            else if (item.VerilenAraliktaIslemVarMi(islemTarih, sonTarih))
                yaz = true;

            if (!yaz)
                return false;

            XLS.HucreKopyala(formatSatir, formatSutun, formatSatir2, formatSutun2, sat, baslaSutun);
            int sut = 0;
            XLS.HucreDegerYaz(sat, baslaSutun + sut++, item.borcluAd);
            XLS.HucreDegerYaz(sat, baslaSutun + sut++, item.basvuruKayitNumarasi);
            XLS.HucreDegerYaz(sat, baslaSutun + sut++, item.iPARDDonemiAd);
            XLS.HucreDegerYaz(sat, baslaSutun + sut++, tl ? "TL" : "EUR");
            XLS.HucreDegerYaz(sat, baslaSutun + sut++, item.sozlesmeNumarasi);
            XLS.HucreDegerYaz(sat, baslaSutun + sut++, item.defterSirasi);
            XLS.HucreDegerYaz(sat, baslaSutun + sut++, item.vakaNumarasi);
            TarihYaz(sat, baslaSutun + sut++, item.onBorcTarihiDT);
            TarihYaz(sat, baslaSutun + sut++, item.geriAlimKarariTarihiDT);
            XLS.HucreDegerYaz(sat, baslaSutun + sut++, item.geriAlimMiktarToplamKur(tl));

            double m = item.kalanFaizMiktarABKurSil(true);
            HerseyYaz(XLS, sat, baslaSutun + sut, item, tl);
            sut += 21; //Aradaki toplamlar

            DateTime toplamaTarih = sonTarih.Oku().AddDays(1);
            MiktarIslem odemeMik;
            if (item.geriAlimKarariTarihiDT >= item.islemTarihiDT && item.geriAlimKarariTarihiDT <= sonTarih.Oku())
            {
                XLS.HucreDegerYaz(sat, sut + 2, item.geriAlimMiktarABKur(tl));
                XLS.HucreDegerYaz(sat, sut + 3, item.geriAlimMiktarUlusalKur(tl));
            }
            else
            {
                XLS.HucreDegerYaz(sat, sut + 2, 0);
                XLS.HucreDegerYaz(sat, sut + 3, 0);
            }

            //odemeMik = item.ToplamAl(new BorcHareket.ENUMHareketTur[] { BorcHareket.ENUMHareketTur.FaizHesabi, BorcHareket.ENUMHareketTur.FaizHesabiIslem }, islemTarih.Oku(), toplamaTarih, false, out tarihler, sonIslemTarih, true);

            //XLS.HucreDegerYaz(sat, sut + 5, odemeMik.faizABKur(tl));
            //XLS.HucreDegerYaz(sat, sut + 6, odemeMik.faizUlusalKur(tl));
            sut += 7;

            odemeMik = item.ToplamAl(new BorcHareket.ENUMHareketTur[] { BorcHareket.ENUMHareketTur.Odeme, BorcHareket.ENUMHareketTur.Mahsup, BorcHareket.ENUMHareketTur.TaksitOdeme, BorcHareket.ENUMHareketTur.Terkin, BorcHareket.ENUMHareketTur.Iade }, islemTarih.Oku(), toplamaTarih, !tl, out tarihler, sonIslemTarih, true, false);
            HerseyYaz(XLS, sat, baslaSutun + sut, odemeMik, tl, true, sonIslemTarih, -1);
            sut += 8;

            odemeMik = item.ToplamAl(new BorcHareket.ENUMHareketTur[] { BorcHareket.ENUMHareketTur.Silme }, islemTarih.Oku(), sonTarih.Oku().AddDays(1), false, out tarihler, sonIslemTarih, false, false);
            if (!TNSDateTime.IsNull(sonIslemTarih))
            {
                TarihYaz(sat, sut, sonIslemTarih.Oku());
                XLS.HucreDegerYaz(sat, sut + 3, odemeMik.miktarABKur(tl) * -1);
                XLS.HucreDegerYaz(sat, sut + 4, odemeMik.miktarABKur(tl));
                XLS.HucreDegerYaz(sat, sut + 6, odemeMik.faizABKur(tl) * -1);
                XLS.HucreDegerYaz(sat, sut + 7, odemeMik.faizABKur(tl));
            }
            sut += 8;

            odemeMik = item.ToplamAl(new BorcHareket.ENUMHareketTur[] { BorcHareket.ENUMHareketTur.Duzeltme }, islemTarih.Oku(), sonTarih.Oku().AddDays(1), false, out tarihler, sonIslemTarih, true, false);
            HerseyYaz(XLS, sat, baslaSutun + sut, odemeMik, tl, false, null, -1);
            sut += 7;

            item.IslemToplamBul(sonTarih, -1, true, null, onayTarihi, -1);
            HerseyYaz(XLS, sat, baslaSutun + sut, item, tl);

            sat++;
            return true;
        }

        private bool SatirYazBorcRapor(DosyaAB item, bool tl, TNSDateTime islemTarih, TNSDateTime sonTarih, ToplamAl toplamAl)
        {
            toplamAl.SatirSifirla(1);
            string tarihler;
            TNSDateTime sonIslemTarih = new TNSDateTime();
            TNSDateTime sonGun = new TNSDateTime(islemTarih.Oku().AddDays(-1));
            item.IslemToplamBul(sonGun, -1, true, null, onayTarihi, -1);
            bool yaz = false;
            if (item.kalanMiktarToplamKur(true) > 0)
                yaz = true;
            else if (item.geriAlimKarariTarihiDT >= islemTarih.Oku() && item.geriAlimKarariTarihiDT <= sonTarih.Oku())
                yaz = true;
            else if (item.VerilenAraliktaIslemVarMi(islemTarih, sonTarih))
                yaz = true;

            if (!yaz)
                return false;

            XLS.HucreKopyala(formatSatir, formatSutun, formatSatir2, formatSutun2, sat, baslaSutun);
            int sut = 0;
            XLS.HucreDegerYaz(sat, baslaSutun + sut++, item.borcluAd);
            XLS.HucreDegerYaz(sat, baslaSutun + sut++, item.basvuruKayitNumarasi);
            XLS.HucreDegerYaz(sat, baslaSutun + sut++, item.iPARDDonemiAd);
            XLS.HucreDegerYaz(sat, baslaSutun + sut++, tl ? "TL" : "EUR");
            XLS.HucreDegerYaz(sat, baslaSutun + sut++, item.sozlesmeNumarasi);
            XLS.HucreDegerYaz(sat, baslaSutun + sut++, item.defterSirasi);
            XLS.HucreDegerYaz(sat, baslaSutun + sut++, item.vakaNumarasi);
            TarihYaz(sat, baslaSutun + sut++, item.onBorcTarihiDT);
            TarihYaz(sat, baslaSutun + sut++, item.geriAlimKarariTarihiDT);

            toplamAl.DegerAta(1, 0, item.kalanMiktarABKur(tl));
            toplamAl.DegerAta(1, 1, item.kalanFaizMiktarABKur(tl));
            toplamAl.DegerAta(1, 2, item.kalanMiktarABKur(tl));
            toplamAl.DegerAta(1, 3, item.kalanFaizMiktarABKur(tl));

            DateTime toplamaTarih = sonTarih.Oku().AddDays(1);
            MiktarIslem odemeMik;
            if (item.geriAlimKarariTarihiDT >= item.islemTarihiDT && item.geriAlimKarariTarihiDT <= sonTarih.Oku())
            {
                toplamAl.DegerAta(1, 6, item.geriAlimMiktarABKur(tl));
            }

            odemeMik = item.ToplamAl(new BorcHareket.ENUMHareketTur[] { BorcHareket.ENUMHareketTur.FaizHesabi, BorcHareket.ENUMHareketTur.FaizHesabiIslem }, islemTarih.Oku(), toplamaTarih, !tl, out tarihler, sonIslemTarih, true, false);
            toplamAl.DegerAta(1, 7, odemeMik.faizABKur(tl));

            odemeMik = item.ToplamAl(new BorcHareket.ENUMHareketTur[] { BorcHareket.ENUMHareketTur.Duzeltme }, islemTarih.Oku(), sonTarih.Oku().AddDays(1), false, out tarihler, sonIslemTarih, true, false);
            toplamAl.DegerAta(1, 8, odemeMik.miktarABKur(tl));
            toplamAl.DegerAta(1, 9, odemeMik.faizABKur(tl));

            odemeMik = item.ToplamAl(new BorcHareket.ENUMHareketTur[] { BorcHareket.ENUMHareketTur.Odeme, BorcHareket.ENUMHareketTur.Mahsup, BorcHareket.ENUMHareketTur.TaksitOdeme, BorcHareket.ENUMHareketTur.Terkin, BorcHareket.ENUMHareketTur.Iade }, islemTarih.Oku(), toplamaTarih, !tl, out tarihler, sonIslemTarih, true, false);
            toplamAl.DegerAta(1, 10, odemeMik.miktarABKur(tl));
            toplamAl.DegerAta(1, 11, odemeMik.faizABKur(tl));

            odemeMik = item.ToplamAl(new BorcHareket.ENUMHareketTur[] { BorcHareket.ENUMHareketTur.Silme }, islemTarih.Oku(), sonTarih.Oku().AddDays(1), false, out tarihler, sonIslemTarih, false, false);
            if (!TNSDateTime.IsNull(sonIslemTarih))
            {
                toplamAl.DegerAta(1, 12, odemeMik.miktarABKur(tl));
            }

            item.IslemToplamBul(sonTarih, -1, true, null, onayTarihi, -1);
            toplamAl.DegerAta(1, 13, item.kalanMiktarABKurSil(tl));
            toplamAl.DegerAta(1, 14, item.kalanFaizMiktarABKurSil(tl));
            SatirYazBorcRaporToplamAl(XLS, sat, sut, toplamAl, 1);
            toplamAl.ToplamEkle(0, 1);
            sat++;
            return true;
        }

        private void SatirYazBorcRaporToplamAl(Tablo XLS, int sat, int sut, ToplamAl toplam, int toplamSatir)
        {
            for (int i = 0; i < 15; i++)
            {
                XLS.HucreDegerYaz(sat, sut + i, toplam.toplam[toplamSatir, i]);
            }
        }

        private void HerseyYaz(Tablo XLS, int sat, int sut, MiktarIslem islemToplam, bool tl, bool tarihVar, TNSDateTime tarih, int carpan)
        {
            if (tarihVar)
            {
                TarihYaz(sat, sut, tarih.Oku());
                sut++;
            }
            XLS.HucreDegerYaz(sat, sut + 2, islemToplam.miktarABKur(tl) * carpan);
            XLS.HucreDegerYaz(sat, sut + 3, islemToplam.miktarUlusalKur(tl) * carpan);
            XLS.HucreDegerYaz(sat, sut + 5, islemToplam.faizABKur(tl) * carpan);
            XLS.HucreDegerYaz(sat, sut + 6, islemToplam.faizUlusalKur(tl) * carpan);
        }

        private void HerseyYaz(Tablo XLS, int sat, int sut, DosyaAB item, bool tl)
        {
            XLS.HucreDegerYaz(sat, sut + 2, item.kalanMiktarABKurSil(tl));
            XLS.HucreDegerYaz(sat, sut + 3, item.kalanMiktarUlusalKurSil(tl));
            XLS.HucreDegerYaz(sat, sut + 5, item.kalanFaizMiktarABKurSil(tl));
            XLS.HucreDegerYaz(sat, sut + 6, item.kalanFaizMiktarUlusalKurSil(tl));
        }

        private DateTime MinDate(DateTime silmeTarih, DateTime sonTarih)
        {
            if (silmeTarih > DateTime.MinValue && silmeTarih < sonTarih)
                return silmeTarih;
            else
                return sonTarih;
        }

        private BorcHareket BuyukBul(BorcHareket bh1, BorcHareket bh2)
        {
            if (bh2 == null)
                return bh1;
            else if (bh1 == null)
                return bh2;
            else
            {
                if (bh2.yerNo > bh1.yerNo) return bh2;
                else return bh1;
            }
        }

        private bool BendenSonraYazilacakVarMi(DosyaAB item, int satir)
        {
            if (item != null)
            {
                for (int i = satir + 1; i < item.borcHareket.Count; i++)
                {
                    BorcHareket bh = (BorcHareket)item.borcHareket[i];
                    if (!bh.FaizIslemi && bh.tur != BorcHareket.ENUMHareketTur.Silme)
                        return true;
                }
            }
            return false;
        }

        private void OrtakBilgiYazExcel()
        {
            foreach (HucreAdres item in HucreAdresler)
            {
                int shNo = 0, satIc = 0, sutIc = 0, satIc2 = 0, sutIc2 = 0;
                XLS.HucreAdAdresCoz(item.hucreAd, ref shNo, ref satIc, ref sutIc, ref satIc2, ref sutIc2);
                if (satIc >= 0 && sutIc >= 0)
                {
                    int aS = XLS.AktifSheet();
                    if (aS != shNo)
                    {
                        XLS.AktifSheetDegistir(shNo);
                    }
                    if (item.tip == 1)
                        XLS.HucreDegerYaz(satIc, sutIc, DegerBulInt(item.hucreAd));
                    else if (item.tip == 2 || item.tip == 5)
                        XLS.HucreDegerYaz(satIc, sutIc, DegerBulDbl(item.hucreAd));
                    else if (item.tip == 3)
                        TarihYaz(satIc, sutIc, DegerBulTrh(item.hucreAd));
                    else if (item.tip == 4)
                        XLS.HucreDegerYaz(satIc, sutIc, DegerBulStr(item.hucreAd));
                    if (aS != shNo)
                    {
                        XLS.AktifSheetDegistir(aS);
                    }
                }
            }
        }

        private void RaporAl6310()
        {
            if (dab.iPARDDonemi == 1)
            {
                XLS.HucreDegerYaz(25, 1, "TR66 0001 5001 5800 7301 6391 25");
                XLS.HucreDegerYaz(25, 0, "TR93 0001 5001 5800 7301 6392 21");
            }
            else if (dab.iPARDDonemi == 2)
            {
                XLS.HucreDegerYaz(25, 1, "TR09 0001 5001 5800 7305 8853 56");
                XLS.HucreDegerYaz(25, 0, "TR24 0001 5001 5800 7305 8853 77");
            }
            else
            {
                XLS.HucreDegerYaz(25, 0, "TR75 0001 5001 5800 7332 4478 12");
                XLS.HucreDegerYaz(25, 1, "-");

                XLS.HucreDegerYaz(28, 0, "IPARD III Programı Geri Alım Hesabı");
                XLS.HucreFormulYaz(28, 4, "=kalanMiktarABSil+kalanFaizMiktarABSil+kalanMiktarUlusalSil+kalanFaizMiktarUlusalSil");
                XLS.HucreDegerYaz(29, 0, "");
                XLS.HucreDegerYaz(29, 1, "");
                XLS.HucreDegerYaz(29, 4, "");
            }
        }

        private void RaporAl6307Toplu(ObjectArray oa, TNSDateTime faizHesabiSonTarihi)
        {
            XLS = GenelIslemler.NewTablo();
            XLS.BosDosyaAc(sonucDosyaAd);
            int say = 0;
            foreach (DosyaAB item in oa.objeler)
            {
                Hashtable ht = new Hashtable();
                ht["iPARDDonemi"] = item.iPARDDonemi.ToString();
                ht["dosyaAnahtar"] = item.dosyaAnahtar.ToString();
                ht["CiktiYeri"] = "xlsx";
                ht["islemTarihi"] = dab.islemTarihi.ToString();
                ht["onayTarihi"] = dab.onayTarihi.ToString();
                string uretilenDosya = TNS.Raporlama.RaporlamaServis.TekRaporUret(11000, 1, kullanan, null, ht);
                if (!string.IsNullOrWhiteSpace(uretilenDosya))
                {
                    XLS.YeniSheetEkle(uretilenDosya, item.defterSirasi + " H", 0);
                    XLS.YeniSheetEkle(uretilenDosya, item.defterSirasi + " C", 1);
                    say += 2;
                }
            }
            if (say > 0)
            {
                XLS.SheetSil(0);
                for (int i = 0; i < say; i++)
                {
                    XLS.SelectSheet(i);
                }
            }
        }

        private void RaporAl6307(bool tl)
        {
            double oncDegerAB = 0;
            double oncDegerUlusal = 0;
            double topAB = 0;
            double topUlusal = 0;

            BorcHareket yazHar = null;
            if (islemNo > 0)
                yazHar = dab.HareketBul(islemNo);
            if (yazHar == null && !TNSDateTime.IsNull(dab.islemTarihi))
            {
                yazHar = dab.HareketBulIslemTarih();
                if (yazHar == null)
                {
                    yazHar = new BorcHareket(dab);
                    yazHar.tur = BorcHareket.ENUMHareketTur.FaizHesabi;
                    yazHar.tarihDT = dab.islemTarihi.Oku();
                    dab.IslemToplamBul(yazHar.tarih, -1, false, null, onayTarihi, -1);
                    yazHar.faizAB = dab.kalanFaizMiktarAB;
                    yazHar.faizUlusal = dab.kalanFaizMiktarUlusal;
                    yazHar.kayitDurum = DosyaABParca.ENUMKayitDurum.ONAYLI;
                    dab.borcHareket.Add(yazHar);
                }
            }
            XLS.HucreAdBulYaz("silinmisBorcOngurulenTarih", dab.ongorulenSilinmeTarihiDT.ToShortDateString());

            if (yazHar != null)
            {
                if (yazHar.OdemeTuruIslem)
                {
                    XLS.HucreAdBulYaz("geriAlinanBorcTarihi", yazHar.tarih.ToString());
                    XLS.HucreAdBulYaz("geriAlinanBorcAB", yazHar.miktarABKur(tl));
                    XLS.HucreAdBulYaz("geriAlinanBorcFaizAB", yazHar.faizABKur(tl));
                    XLS.HucreAdBulYaz("geriAlinanBorcUlusal", yazHar.miktarUlusalKur(tl));
                    XLS.HucreAdBulYaz("geriAlinanBorcFaizUlusal", yazHar.faizUlusalKur(tl));
                }
                else if (yazHar.tur == BorcHareket.ENUMHareketTur.Duzeltme)
                {
                    XLS.HucreAdBulYaz("duzeltmeTarihi", yazHar.tarih.ToString() + "/" + yazHar.karar);
                    XLS.HucreAdBulYaz("duzeltmeAB", yazHar.miktarABKur(tl));
                    XLS.HucreAdBulYaz("duzeltmeFaizAB", yazHar.faizABKur(tl));
                    XLS.HucreAdBulYaz("duzeltmeUlusal", yazHar.miktarUlusalKur(tl));
                    XLS.HucreAdBulYaz("duzeltmeFaizUlusal", yazHar.faizUlusalKur(tl));
                }
                else if (yazHar.tur == BorcHareket.ENUMHareketTur.Silme)
                {
                    XLS.HucreAdBulYaz("silinmisBorcKarar", yazHar.karar);
                    XLS.HucreAdBulYaz("silinmisBorcTarih", yazHar.tarih.ToString());
                    XLS.HucreAdBulYaz("silinmeAB", yazHar.miktarABKur(tl));
                    XLS.HucreAdBulYaz("silinmeFaizAB", yazHar.faizABKur(tl));
                }
            }

            if (!DosyaAB.TaksitVarMi(dab.taksitKararTarihDT, dab.islemTarihiDT))
            {
                XLS.HucreAdBulYaz("kalanMiktarAB1", dab.kalanMiktarAB);
                XLS.HucreAdBulYaz("kalanMiktarAB2", dab.kalanMiktarAB);

                XLS.HucreAdBulYaz("kalanMiktarUlusal1", dab.kalanMiktarUlusal);
                XLS.HucreAdBulYaz("kalanMiktarUlusal2", dab.kalanMiktarUlusal);
                XLS.HucreAdBulYaz("aySayisi1", dab.aySayisi);
                XLS.HucreAdBulYaz("gunSayisi1", dab.gunSayisi);
                XLS.HucreAdBulYaz("faizOranAyToplam1", dab.faizOranAyToplam);
                XLS.HucreAdBulYaz("faizOranGunToplam1", dab.faizOranGunToplam);

                XLS.HucreAdBulYaz("faizTutarAB1", OrtakFonksiyonlar.KurusYap(dab.kalanMiktarAB * dab.faizOranAyToplam));
                XLS.HucreAdBulYaz("faizTutarUlusal1", OrtakFonksiyonlar.KurusYap(dab.kalanMiktarUlusal * dab.faizOranAyToplam));

                XLS.HucreAdBulYaz("faizTutarAB2", OrtakFonksiyonlar.KurusYap(dab.kalanMiktarAB * dab.faizOranGunToplam));
                XLS.HucreAdBulYaz("faizTutarUlusal2", OrtakFonksiyonlar.KurusYap(dab.kalanMiktarUlusal * dab.faizOranGunToplam));
            }
            else
            {
                XLS.HucreAdBulYaz("baslik1", "TAKSİTLEDİRME TARİHİNE KADAR FAİZ HESAPLAMA TABLOSU");
                XLS.HucreAdBulYaz("baslik2", "TAKSİTLEDİRME TARİHİNDEN SONRAKİ FAİZ HESAPLAMA TABLOSU");
                XLS.HucreAdBulYaz("baslik3", "TOPLAM FAİZ MİKTARI");
                XLS.HucreAdBulYaz("kalanMiktarAB1", dab.kalanMiktarAB);
                XLS.HucreAdBulYaz("kalanMiktarUlusal1", dab.kalanMiktarUlusal);
                XLS.HucreAdBulYaz("faizOranAyToplam1", dab.oran1);
                XLS.HucreAdBulYaz("faizTutarAB1", dab.kalanErtelemeAB);
                XLS.HucreAdBulYaz("faizTutarUlusal1", dab.kalanErtelemeUlusal);

                XLS.HucreAdBulYaz("kalanMiktarAB2", dab.kalanMiktarAB + dab.kalanErtelemeAB);
                XLS.HucreAdBulYaz("kalanMiktarUlusal2", dab.kalanMiktarUlusal + dab.kalanErtelemeUlusal);
                XLS.HucreAdBulYaz("faizOranGunToplam1", dab.oran2);
                XLS.HucreAdBulYaz("faizTutarAB2", dab.kalanFaizMiktarAB - dab.kalanErtelemeAB);
                XLS.HucreAdBulYaz("faizTutarUlusal2", dab.kalanFaizMiktarUlusal - dab.kalanErtelemeUlusal);

                XLS.HucreAdBulYaz("aySayisi1", dab.ay1 + " ay " + dab.gun1 + " gün");
                XLS.HucreAdBulYaz("gunSayisi1", dab.gun2);
            }

            XLS.AktifSheetDegistir(formatSayfa);
            BorcHareket oncItem = null;
            if (dab.silmeVar)
            {
                XLS.HucreDegerYaz(5, 3, "SİLİNMİŞ");
            }
            foreach (BorcHareket item in dab.borcHareket)
            {
                if (item.kayitDurum != DosyaABParca.ENUMKayitDurum.ONAYLI) continue;
                if (item.tur == BorcHareket.ENUMHareketTur.Silme || item.tur == BorcHareket.ENUMHareketTur.Taksit || item.tur == BorcHareket.ENUMHareketTur.FazlaOdemeAktarim || item.tur == BorcHareket.ENUMHareketTur.OdeneniAktarma) continue;
                if (oncDegerAB == item.faizAB && oncDegerUlusal == item.faizUlusal) continue;

                XLS.SatirAc(sat, 1);
                if (item.tur == BorcHareket.ENUMHareketTur.FaizHesabi)
                    XLS.HucreKopyala(formatSatir, formatSutun, formatSatir, formatSutun2, sat, baslaSutun);
                else
                    XLS.HucreKopyala(formatSatir + 1, formatSutun, formatSatir + 1, formatSutun2, sat, baslaSutun);

                item.silmeDuzelt();

                if ((item.OdemeTuruIslem) && oncItem != null)
                {
                    item.faizAB = oncDegerAB - item.faizAB;
                    item.faizUlusal = oncDegerUlusal - item.faizUlusal;
                }
                XLS.HucreDegerYaz(sat, baslaSutun, item.tarihDT);
                XLS.HucreDegerYaz(sat, baslaSutun + 1, item.faizAB);
                XLS.HucreDegerYaz(sat, baslaSutun + 2, item.faizUlusal);

                double farkAB = item.faizAB - oncDegerAB;
                double farkUlusal = item.faizUlusal - oncDegerUlusal;

                topAB += farkAB;
                topUlusal += farkUlusal;

                XLS.HucreDegerYaz(sat, baslaSutun + 4, farkAB);
                XLS.HucreDegerYaz(sat, baslaSutun + 5, farkUlusal);

                oncDegerAB = item.faizAB;
                oncDegerUlusal = item.faizUlusal;
                oncItem = item;
                sat++;
            }

            XLS.SatirYukseklikAyarla(baslaSatir, sat - 1, -1, 3, satirYuk);
            XLS.HucreDegerYaz(sat + 1, baslaSutun + 4, topAB);
            XLS.HucreDegerYaz(sat + 1, baslaSutun + 5, topUlusal);

            XLS.HucreAdBulYaz("hazirlayan2", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzHazirlayan"));
            XLS.HucreAdBulYaz("kontrolEden2", GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ImzKontrolEden"));

            XLS.HucreDegerYaz(sat + 3, baslaSutun + 6, dab.geriAlimKarariTarihiDT);
            XLS.AktifSheetDegistir(0);
            XLS.SelectSheet(0);
            XLS.SelectSheet(1);
        }

        private void TarihYaz(int sat, int sut, DateTime dateTime)
        {
            if (dateTime != DateTime.MinValue)
                XLS.HucreDegerYaz(sat, sut, dateTime);
        }

        private void TarihYaz(string hucreAd, DateTime dateTime)
        {
            if (dateTime != DateTime.MinValue)
            {
                int sat = -1;
                int sut = -1;
                XLS.HucreAdAdresCoz(hucreAd, ref sat, ref sut);
                if (sat >= 0)
                    XLS.HucreDegerYaz(sat, sut, dateTime);
            }
        }

        private string DegerBulStr(string item)
        {
            try
            {
                return (string)dab.GetType().GetProperty(item).GetValue(dab, null);
            }
            catch
            {
                return "";
            }
        }

        private double DegerBulDbl(string item)
        {
            try
            {
                return (double)dab.GetType().GetProperty(item).GetValue(dab, null);
            }
            catch { return 0; }
        }

        private DateTime DegerBulTrh(string item)
        {
            try
            {
                TNSDateTime dt = (TNSDateTime)dab.GetType().GetProperty(item).GetValue(dab, null);
                return dt.Oku();
            }
            catch { return DateTime.MinValue; }
        }

        private int DegerBulInt(string item)
        {
            try
            {
                return (int)dab.GetType().GetProperty(item).GetValue(dab, null);
            }
            catch { return 0; }
        }
    }
}