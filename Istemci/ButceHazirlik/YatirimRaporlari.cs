using System;
using System.Collections.Generic;
using TNS;
using TNS.KYM;
using OrtakClass;
using TNS.YAT;
using TNS.UZY;
using TNS.BKD;
using System.Collections;
using TNS.BHMv2;
using System.Data;
using Aspose.Words;
using Aspose.Words.Reporting;
using TNS.BUM;
using TNS.TMM;

namespace ButceHazirlikv2
{
    /// <summary>
    /// DPT yatırım projeleri raporlarını üreten yordamları tutan sınıf.
    /// Sınıfın giriş yordamı olan RaporAl yordamı gelen ek param bilgisine göre ilgili üretim yordamını çağırır.
    /// </summary>
    public class YatirimRaporlari
    {
        IYATServis servisYAT = TNS.YAT.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
        Kullanici kullanan = null;
        string sablonAd = string.Empty;
        string sonucDosyaAd = string.Empty;
        string sonucDosyaAdWrd = string.Empty;
        int Bolen = 1;
        int projeNo;
        int altProjeNo;
        string sektorAd = "";
        private Hashtable raporParamAna;

        /// <summary>
        /// IRAPOR Interface'ini gerçekleştiren sınıfın genel giriş noktası.
        /// Bu yordam ek bilgi alanındaki değere göre ilgili yordamı çağırır.
        /// </summary>
        /// <param name="kullanan">Kullanıcı nesnesi</param>
        /// <param name="servis">Sunucuya erişim nesnesi</param>
        /// <param name="sablonAd">Şablon dosya adı</param>
        /// <param name="sonucDosyaAd">Üretilecek dosya adı</param>
        /// <param name="ciktiYeri">İlgili yordama geçirilir. Genellikle yordamlar bu değişkenden bağımsız olarak rapor üretirler.</param>
        /// <param name="raporParam">Hash table içerisinde raporda kullanılacak parametreleri içeren nesne. Ek bilgi alanına göre değişiklik gösterir. İlgili yordamda hangi parametrelerin kullanıldığı açıklanmıştır.
        /// </param>
        /// <param name="ekBilgi">Desteklenen ek bilgiler. 
        /// Ekbilgi kodu (iki noktanın solundaki) verildiğinde hangi tablonun üretileceği aşağıda listelenmiştir.
        ///      1:                   Tablo1
        ///      2:                   Tablo2
        ///      2-1:                 Tablo2 (düzey 2)
        ///      3:                   Tablo3
        ///      4:                   Tablo4
        ///      5:                   Tablo5
        ///      6:                   Tablo6
        ///      7:                   Tablo7
        ///      10:                  Tablo10
        ///      12_13:               Tablo12_13
        ///      TeklifDagilim:       TeklifDagilim
        ///      SektorDagilim:       SektorDagilim
        ///      BirimProjeleri:      BirimProjeleri
        ///      KarakterDagilim:     KarakterDagilim
        ///      KonsolideYatirim:    KonsolideYatirim
        ///      ProjeTakip:          ProjeTakip
        ///      ProjelerArasiOdenek: ProjelerArasiOdenekAktarmasi
        ///      TertipDagilim:       TertipDagilim
        ///      2Ozel:               Tablo2Ozel
        ///      DPTVizeCetveli:      DPTVizeCetveli
        ///      15:                  Dış Kredi İzleme Raporu
        /// </param>
        /// <returns>Rapor başarı ile üretilmişse üretilen dosya adı, aksi takdirde boşluk döndürür.</returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            this.kullanan = (Kullanici)kullanan;
            this.sablonAd = sablonAd;

            this.sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            this.sonucDosyaAdWrd = sonucDosyaAd + "." + "docx";

            int yil = OrtakFonksiyonlar.ConvertToInt(raporParam["ButceYil"], 0);
            string senaryoKod = raporParam["Senaryo"] + "";
            string senaryoAd = raporParam["SenaryoAd"] + ""; ;
            string birimKod = raporParam["KurKod"] + "";
            string birimAd = raporParam["KurAd"] + "";
            this.raporParamAna = raporParam;
            if (birimKod == "00")
            {
                birimKod = YatRaporAraclari.VarsayilanKurumBul();
                birimAd = YatRaporAraclari.VarsayilanKurumBulAd();
            }
            projeNo = OrtakFonksiyonlar.ConvertToInt(raporParam["projeNo"] + "", -1);
            altProjeNo = OrtakFonksiyonlar.ConvertToInt(raporParam["altProjeNo"] + "", -1);
            string sektor = raporParam["SektorKod"] + "";
            sektorAd = raporParam["SektorAd"] + "";
            if (string.IsNullOrWhiteSpace(sektorAd)) sektorAd = "Tüm Sektörler";
            if (ekBilgi.Equals("1"))
                return Tablo1Yaz(yil, senaryoKod, senaryoAd, birimKod);
            else if (ekBilgi.Equals("2") || ekBilgi.Equals("13"))
                return Tablo2Yaz(yil, senaryoKod, sektor, senaryoAd, false, ekBilgi.Equals("13"), birimKod, false);
            else if (ekBilgi.Equals("Yeni2"))
                return TabloYeni2Yaz(yil, senaryoKod, sektor, senaryoAd, false, ekBilgi.Equals("13"), birimKod, false);
            else if (ekBilgi.Equals("2-o"))
                return Tablo2Yaz(yil, senaryoKod, sektor, senaryoAd, false, false, birimKod, true);
            else if (ekBilgi.Equals("2-1"))
                return Tablo2Yaz(yil, senaryoKod, sektor, senaryoAd, true, false, birimKod, false);
            else if (ekBilgi.Equals("3"))
                return Tablo3Yaz(yil, senaryoKod, sektor, birimKod);
            else if (ekBilgi.Equals("4"))
                return Tablo4Yaz(yil, senaryoKod, sektor, birimKod);
            else if (ekBilgi.Equals("5"))
                return Tablo5Yaz(yil, senaryoKod, sektor, birimKod);
            else if (ekBilgi.Equals("6"))
                return Tablo6Yaz(yil, senaryoKod, sektor, birimKod);
            else if (ekBilgi.Equals("7"))
                return Tablo7Yaz(yil, senaryoKod);
            else if (ekBilgi.Equals("10"))
                return Tablo10Yaz(yil, senaryoKod, sektor, senaryoAd, birimKod);
            else if (ekBilgi.StartsWith("12_13"))
                return Tablo12_13Yaz(yil, sektor, OrtakFonksiyonlar.ConvertToInt(ekBilgi.Split('-')[1], 0), senaryoKod, birimKod);
            else if (ekBilgi.Equals("TeklifDagilim"))
                return TeklifDagilimYaz(yil, sektor, senaryoKod, birimKod);
            else if (ekBilgi.Equals("SektorDagilim"))
                return SektorDagilimYaz(yil, senaryoKod, senaryoAd, birimKod);
            else if (ekBilgi.Equals("BirimProjeleri"))
                return BirimProjeleriYaz(yil, senaryoKod, birimKod);
            else if (ekBilgi.Equals("KarakterDagilim"))
                return KarakterDagilimYaz(yil, senaryoKod, sektor, birimKod);
            else if (ekBilgi.Equals("KonsolideYatirim"))
                return KonsolideYatirimYaz(yil, birimKod, senaryoKod);
            else if (ekBilgi.Equals("ProjeTakip"))
                return ProjeTakipYaz(yil, birimKod, senaryoKod);
            else if (ekBilgi.Equals("ProjelerArasiOdenek"))
                return ProjelerArasiOdenekAktarmasiYaz(yil, sektor, birimKod, senaryoKod);
            else if (ekBilgi.Equals("TertipDagilim"))
                return TertipDagilimYaz(yil, senaryoKod, sektor, senaryoAd, birimKod);
            else if (ekBilgi.Equals("2Ozel"))
                return Tablo2OzelYaz(yil, senaryoKod, sektor, senaryoAd, birimKod);
            else if (ekBilgi.Equals("DPTVizeCetveli"))
                return DPTVizeCetveliYaz(yil, senaryoKod, new Tertip(yil, ENUMTabloKod.TABPEBOdenek, birimKod));
            else if (ekBilgi.Equals("VizeCetveli"))
                return VizeCetveliYaz(yil, senaryoKod, sektor, senaryoAd, birimKod, birimAd);
            else if (ekBilgi.Equals("ButceSonuc"))
                return ButceSonuc(yil, senaryoKod, sektor, senaryoAd, birimKod, birimAd, raporParam);
            else if (ekBilgi.Equals("ButceSonucHiy"))
                return ButceSonucHiy(yil, senaryoKod, sektor, senaryoAd, birimKod, birimAd, raporParam);
            else if (ekBilgi.Equals("ButceIslem"))
                return ButceIslem(yil, senaryoKod, sektor, senaryoAd, birimKod, birimAd, raporParam);
            else if (ekBilgi.Equals("15"))
            {
                string kaynakNolarStr = (string)raporParam["KaynakNo"];
                if (string.IsNullOrWhiteSpace(kaynakNolarStr))
                {
                    ObjectArray oa = servisYAT.KrediliKaynaklariListele(this.kullanan, yil, senaryoKod, projeNo, altProjeNo, new ENUMButceKaynakTur[] { ENUMButceKaynakTur.HIBE, ENUMButceKaynakTur.KREDIBUTCEDISI, ENUMButceKaynakTur.KREDIBUTCEICI });
                    foreach (int item in oa.objeler)
                    {
                        if (!string.IsNullOrWhiteSpace(kaynakNolarStr)) kaynakNolarStr += ",";
                        kaynakNolarStr += item.ToString();
                    }
                }
                return KrediIzlemeRaporu(yil, senaryoKod, kaynakNolarStr, senaryoAd, birimKod, birimAd, raporParam);
            }
            else if (ekBilgi.Equals("14"))
                return StrPlanRaporu(yil, senaryoKod, senaryoAd, birimKod, birimAd, sektor);
            else if (ekBilgi.Equals("YP"))
                return YollukPlani(yil, senaryoKod, senaryoAd, birimKod, birimAd, sektor);
            else if (ekBilgi.Equals("16"))
                return UygulamaRaporu(yil, senaryoKod, senaryoAd, birimKod, birimAd, sektor);
            else if (ekBilgi.Equals("ProjeBilgiFormu"))
                return ProjeBilgiFormuYazdir(yil, senaryoKod, sektor, senaryoAd, true, false, birimKod, false);
            else if (ekBilgi.Equals("ProjeBilgiListe"))
                return ProjeBilgiListeYazdir(yil, senaryoKod, sektor, senaryoAd, true, false, birimKod, false);

            throw new RaporHatasi("Hatalı rapor kodu geldi!");
        }


        private string ProjeBilgiListeYazdir(int yil, string senaryoKod, string sektor, string senaryoAd, bool duzey2, bool hepsiniGoster, string kurKod, bool odenekleriGoster)
        {
            DataTable tabHeader = DataTableOl(yil, senaryoKod, sektor, kurKod);

            Tablo XLS = GenelIslemler.NewTablo();
            int formatSatir = 0;
            int formatSutun = 0;
            int baslaSatir = 0;
            int baslaSutun = 0;
            XLS.DosyaAc(sablonAd, this.sonucDosyaAd);

            XLS.HucreAdAdresCoz("FormatSatir", ref formatSatir, ref formatSutun);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
            XLS.HucreAdBulYaz("butceYil", yil);
            int satir = baslaSatir;
            foreach (DataRow row in tabHeader.Rows)
            {
                int tut = row.Field<int>("AltProjeNo");
                int ekle;
                if (tut == 0)
                    ekle = 0;
                else
                    ekle = 1;

                XLS.HucreKopyala(formatSatir + ekle, formatSutun, formatSatir + ekle, formatSutun + 15, satir, baslaSutun);

                int kol = baslaSutun;
                XLS.HucreDegerYaz(satir, kol++, row.Field<string>("ProjeAd"));
                XLS.HucreDegerYaz(satir, kol++, row.Field<string>("BirimAd"));

                XLS.HucreDegerYaz(satir, kol++, row.Field<string>("Sektoru"));
                XLS.HucreDegerYaz(satir, kol++, row.Field<string>("UygulamaYeri"));
                XLS.HucreDegerYaz(satir, kol++, row.Field<string>("ProjeSuresi"));
                XLS.HucreDegerYaz(satir, kol++, row.Field<string>("Karakteristik"));
                XLS.HucreDegerYaz(satir, kol++, row.Field<string>("Amac"));

                XLS.HucreDegerYaz(satir, kol++, OrtakFonksiyonlar.ConvertToDbl(row.Field<string>("ProjeTutari")));
                XLS.HucreDegerYaz(satir, kol++, OrtakFonksiyonlar.ConvertToDbl(row.Field<string>("OncOdenek")));
                XLS.HucreDegerYaz(satir, kol++, OrtakFonksiyonlar.ConvertToDbl(row.Field<string>("KumHarcama")));
                XLS.HucreDegerYaz(satir, kol++, OrtakFonksiyonlar.ConvertToDbl(row.Field<string>("Teklif")));
                XLS.HucreDegerYaz(satir, kol++, OrtakFonksiyonlar.ConvertToDbl(row.Field<string>("KBO")));
                XLS.HucreDegerYaz(satir, kol++, OrtakFonksiyonlar.ConvertToDbl(row.Field<string>("YatirimP")));

                XLS.HucreDegerYaz(satir, kol++, row.Field<string>("Aciklama"));
                satir++;
            }
            XLS.SatirYukseklikAyarla(baslaSatir, satir - 1, -1, 7, 19);
            XLS.DosyaSaklaTamYol();
            return this.sonucDosyaAd;
        }

        private string ProjeBilgiFormuYazdir(int yil, string senaryoKod, string sektor, string senaryoAd, bool duzey2, bool hepsiniGoster, string kurKod, bool odenekleriGoster)
        {
            DataTable tabHeader = DataTableOl(yil, senaryoKod, sektor, kurKod);

            Aspose.Words.Document doc = new Aspose.Words.Document(sablonAd);
            DocumentBuilder builder = new DocumentBuilder(doc);
            doc.MailMerge.CleanupOptions = MailMergeCleanupOptions.RemoveUnusedRegions | MailMergeCleanupOptions.RemoveUnusedFields;
            doc.MailMerge.ExecuteWithRegions(tabHeader);

            doc.Save(this.sonucDosyaAdWrd);
            return this.sonucDosyaAdWrd;
        }

        private DataTable DataTableOl(int yil, string senaryoKod, string sektor, string kurKod)
        {
            BilgiVAN projeListesi = servisYAT.ProjeAltProjeListele(kullanan, yil, senaryoKod, sektor, null, 0, -1, null, false, yil, false, kurKod, 0, true, "", false);
            if (!projeListesi.sonuc.islemSonuc)
                throw new RaporHatasi(projeListesi.sonuc.hataStr);

            string[] alanlar = { "kisaAmac", "aciklama" };
            Hashtable ht = servisYAT.AltProjeBilgiListeleTum(kullanan, yil, senaryoKod, alanlar);

            DataTable tabHeader = new DataTable();
            tabHeader.TableName = "Projeler";

            tabHeader.Columns.Add("ProjeNo", typeof(int));
            tabHeader.Columns.Add("AltProjeNo", typeof(int));

            tabHeader.Columns.Add("BirimAd");
            tabHeader.Columns.Add("ProjeAd");
            tabHeader.Columns.Add("Sektoru");
            tabHeader.Columns.Add("UygulamaYeri");
            tabHeader.Columns.Add("ProjeSuresi");
            tabHeader.Columns.Add("Karakteristik");

            tabHeader.Columns.Add("Amac");

            tabHeader.Columns.Add("Yil_1");
            tabHeader.Columns.Add("Yil");

            tabHeader.Columns.Add("ProjeTutari");
            tabHeader.Columns.Add("OncOdenek");
            tabHeader.Columns.Add("KumHarcama");
            tabHeader.Columns.Add("Teklif");
            tabHeader.Columns.Add("KBO");
            tabHeader.Columns.Add("YatirimP");

            tabHeader.Columns.Add("Aciklama");
            ToplamAl toplamAl = new ToplamAl(7, 20);

            Bolen = 1;
            Dictionary<string, double[]> butceHazirlikVeri = OdenekleriBul(yil);
            foreach (Proje p in projeListesi.liste)
            {
                DataRow prjRow = tabHeader.Rows.Add(p.kod, 0, p.yurutecekKurumAd, p.ad, p.sektor.ad, p.ProjeYeri(), p.baslangicYili + "-" + p.bitisYili, p.KarakteristikOzet, TNS.YAT.Arac.HTDegerAl(ht, alanlar[0], p.kod, 0), (yil - 1).ToString(), (yil).ToString(), "0", "0", "0", "0", "0", "0", TNS.YAT.Arac.HTDegerAl(ht, alanlar[1], p.kod, 0));
                foreach (AltProje alt in p.altProjeler)
                {
                    foreach (ButceKaynak bk in alt.butceKaynaklari)
                    {
                        toplamAl.SatirSifirla(2);
                        foreach (ButceKaynakTeklif teklif in bk.teklifler)
                        {
                            if (teklif.butceYil == yil)
                            {
                                toplamAl.DegerAta(2, 0, teklif.ProjeTutarToplam / Bolen);
                            }
                            else if (teklif.butceYil == yil - 1)
                            {
                                toplamAl.DegerAta(2, 2, teklif.KumulatifHarcamaToplam / Bolen);
                            }
                        }
                        toplamAl.ToplamEkle(1, 2);
                    }

                    toplamAl.DegerAta(1, 1, ButceHazirlikDegerBul(butceHazirlikVeri, alt.proje.kod, alt.kod, 0)); //Önc Yıl Ödeneği Yatırım (07.00)
                    toplamAl.DegerAta(1, 3, ButceHazirlikDegerBul(butceHazirlikVeri, alt.proje.kod, alt.kod, 1)); //Yılı teklif Yılı (02.00)
                    toplamAl.DegerAta(1, 4, ButceHazirlikDegerBul(butceHazirlikVeri, alt.proje.kod, alt.kod, 2)); //Yılı KBÖ Yılı (06.00)
                    toplamAl.DegerAta(1, 5, ButceHazirlikDegerBul(butceHazirlikVeri, alt.proje.kod, alt.kod, 3)); //Yılı Yatırım Yılı (07.00)

                    tabHeader.Rows.Add(p.kod, alt.kod, alt.birim.ad, alt.ad, p.sektor.ad, alt.ProjeYeri(), p.baslangicYili + "-" + p.bitisYili, alt.KarakteristikOzet, TNS.YAT.Arac.HTDegerAl(ht, alanlar[0], p.kod, alt.kod), (yil - 1).ToString(), (yil).ToString(), toplamAl.ToplamStr(1, 0, 0), toplamAl.ToplamStr(1, 1, 0), toplamAl.ToplamStr(1, 2, 0), toplamAl.ToplamStr(1, 3, 0), toplamAl.ToplamStr(1, 4, 0), toplamAl.ToplamStr(1, 5, 0), TNS.YAT.Arac.HTDegerAl(ht, alanlar[1], p.kod, alt.kod));
                    toplamAl.ToplamEkle(0, 1);
                    toplamAl.SatirSifirla(1);
                }
                prjRow.SetField(11, toplamAl.ToplamStr(0, 0, 0));
                prjRow.SetField(12, toplamAl.ToplamStr(0, 1, 0));
                prjRow.SetField(13, toplamAl.ToplamStr(0, 2, 0));
                prjRow.SetField(14, toplamAl.ToplamStr(0, 3, 0));
                prjRow.SetField(15, toplamAl.ToplamStr(0, 4, 0));
                prjRow.SetField(16, toplamAl.ToplamStr(0, 5, 0));
                toplamAl.SatirSifirla(0);
            }

            if (tabHeader.Rows.Count == 0)
                tabHeader.Rows.Add(0, 0, "", "", "", "", "", "", "Amaç", (yil - 1).ToString(), (yil).ToString(), "0", "0", "0", "0", "0", "0", "Açıklama");
            return tabHeader;
        }

        private string OdenekSorguOlTek(int yil, string senaryoKod)
        {
            string sql = $@";WITH OdenekOzet AS
(
    SELECT YIL, SENARYOKOD, PEBKOD, SUM(ODENEK) AS ODENEK_TOPLAM
    FROM ButceHazirlikv2.dbo.BHMODENEK
    WHERE TEKLIFYIL = 0 AND YIL = {yil} AND SENARYOKOD = '{senaryoKod}'
    GROUP BY YIL, SENARYOKOD, PEBKOD
),
AltProjeOdenek AS
(
    SELECT
        bk.YIL, bk.SENARYOKOD, bk.ALTPROJENO,
        SUM(ISNULL(oo.ODENEK_TOPLAM,0)) AS OdenekToplami
    FROM YATBUTCEKAYNAK bk
    JOIN YATBUTCEKAYNAKPEBKOD pb
      ON pb.YIL=bk.YIL AND pb.SENARYOKOD=bk.SENARYOKOD AND pb.KAYNAKNO=bk.KAYNAKNO
    LEFT JOIN OdenekOzet oo
      ON oo.YIL=pb.YIL AND oo.SENARYOKOD=pb.SENARYOKOD AND oo.PEBKOD=pb.PEBKOD
    WHERE bk.YIL={yil} AND bk.SENARYOKOD='{senaryoKod}'
    GROUP BY bk.YIL, bk.SENARYOKOD, bk.ALTPROJENO
)
SELECT p.PROJENO, AP.ALTPROJENO, ISNULL(apo.OdenekToplami, 0)
FROM YATPROJE p
JOIN YATALTPROJE ap
  ON ap.YIL=p.YIL AND ap.SENARYOKOD=p.SENARYOKOD AND ap.PROJENO=p.PROJENO
LEFT JOIN AltProjeOdenek apo
  ON apo.YIL=ap.YIL AND apo.SENARYOKOD=ap.SENARYOKOD AND apo.ALTPROJENO=ap.ALTPROJENO
WHERE p.YIL={yil} AND p.SENARYOKOD='{senaryoKod}' ORDER BY 1, 2";
            return sql;
        }

        private Dictionary<string, double[]> OdenekSorguOl(params IntKodStrAd[] filtreler)
        {
            Dictionary<string, double[]> dict = new Dictionary<string, double[]>();

            string[] sql = new string[filtreler.Length];
            string[] baglanti = new string[filtreler.Length];

            for (int i = 0; i < filtreler.Length; i++)
            {
                sql[i] = OdenekSorguOlTek(filtreler[i].kod, filtreler[i].ad);
                baglanti[i] = "BaglantiSatiriYAT";
            }

            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

            SortedList sl = uzyServis.SorguBirlestir(sql, baglanti, new int[] { 0, 1 });
            foreach (DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                int projeNo = alAnahtar.IntAl(0);
                int altProjeNo = alAnahtar.IntAl(1);

                double[] v = new double[filtreler.Length];
                for (int i = 0; i < filtreler.Length; i++)
                {
                    v[i] = alVeri.DblAl(i);
                }
                dict[projeNo + "_" + altProjeNo] = v;
            }
            return dict;
        }

        private Dictionary<string, double[]> OdenekleriBul(int yil)
        {
            return OdenekSorguOl(new IntKodStrAd[] { new IntKodStrAd(yil - 1, "0700"), new IntKodStrAd(yil, "0200"), new IntKodStrAd(yil, "0600"), new IntKodStrAd(yil, "0700") });
        }

        private double ButceHazirlikDegerBul(Dictionary<string, double[]> butceHazirlikVeri, int prjNo, int altPrjNo, int indis)
        {
            if (butceHazirlikVeri.TryGetValue(prjNo + "_" + altPrjNo, out double[] degerler))
            {
                return degerler[indis];
            }
            return 0;
        }

        private string Tablo1Yaz(int yil, string senaryoKod, string senaryoAd, string kurKod)
        {
            BilgiVAN projeListesi = servisYAT.ProjeAltProjeListele(kullanan, yil, senaryoKod, "", null, projeNo, -1, null, false, 0, false, kurKod, altProjeNo, true, "", false);
            if (!projeListesi.sonuc.islemSonuc)
                throw new RaporHatasi(projeListesi.sonuc.hataStr);
            //if (projeListesi.liste.Count <= 0)
            //    throw new RaporHatasi(projeListesi.sonuc.bilgiStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            int yuk = XLS.SatirYukseklikAl(kaynakSatir);
            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("Asama", senaryoAd);
            XLS.HucreAdBulYaz("Kurulus", YatRaporAraclari.VarsayilanKurumBulAd());

            satir = kaynakSatir - 1;

            //0=Kurum
            //1=Sektör
            ToplamAl toplamAl = new ToplamAl(2, 12);
            string[] groupKod = { "", "" };

            int[] groupSatir = { 0, 0 };

            foreach (Proje p in projeListesi.liste)
            {
                if (groupKod[1] != p.sektor.kod)
                {
                    if (groupSatir[1] > 0)
                    {
                        ToplamYazT1(XLS, groupSatir, 1, sutun, toplamAl);
                    }
                    groupKod[1] = p.sektor.kod;
                    satir++;
                    XLS.HucreKopyala(kaynakSatir - 2, sutun, kaynakSatir - 2, sutun + 10, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun, p.sektor.ad);
                    groupSatir[1] = satir;
                }
                string etudDevamYeni = p.EtudDevamEdenYeni(yil);
                toplamAl.DegerArttir(1, 0, 1);

                foreach (AltProje alt in p.altProjeler)
                {
                    foreach (ButceKaynak bk in alt.butceKaynaklari)
                    {
                        if (bk.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeGeneleDahil || bk.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeHaricGeneleDahil)
                        {
                            foreach (ButceKaynakTeklif teklif in bk.teklifler)
                            {
                                if (teklif.butceYil == yil)
                                {
                                    toplamAl.DegerArttir(1, 1, teklif.ProjeTutarToplam / Bolen);
                                    toplamAl.DegerArttir(1, 2, teklif.HarcamaTahminToplam / Bolen);

                                    int i = 0;
                                    if (etudDevamYeni == "1") i = 1;
                                    else if (etudDevamYeni == "2") i = 2;
                                    toplamAl.DegerArttir(1, 3 + i, teklif.Odenek0Toplam / Bolen);
                                    toplamAl.DegerArttir(1, 6 + i, teklif.Odenek1Toplam / Bolen);
                                    toplamAl.DegerArttir(1, 9 + i, teklif.Odenek2Toplam / Bolen);

                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (groupSatir[1] > 0)
            {
                ToplamYazT1(XLS, groupSatir, 1, sutun, toplamAl);
            }

            satir++;
            XLS.HucreKopyala(kaynakSatir - 1, sutun, kaynakSatir - 1, sutun + 10, satir, sutun);
            XLS.HucreDegerYaz(satir, sutun, "T O P L A M");
            groupSatir[0] = satir;
            ToplamYazT1(XLS, groupSatir, 0, sutun, toplamAl);
            XLS.SatirYukseklikAyarla(kaynakSatir, satir, yuk);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void ToplamYazT1(Tablo XLS, int[] groupSatir, int satirNo, int sutun, ToplamAl toplamAl)
        {
            if (satirNo == 1)
                toplamAl.ToplamEkle(0, 1);

            XLS.HucreDegerYaz(groupSatir[satirNo], sutun + 1, toplamAl.toplam[satirNo, 0]);
            XLS.HucreDegerYaz(groupSatir[satirNo], sutun + 2, toplamAl.toplam[satirNo, 1]);
            XLS.HucreDegerYaz(groupSatir[satirNo], sutun + 3, toplamAl.toplam[satirNo, 2]);
            XLS.HucreDegerYaz(groupSatir[satirNo], sutun + 4, toplamAl.toplam[satirNo, 3]);
            XLS.HucreDegerYaz(groupSatir[satirNo], sutun + 5, toplamAl.toplam[satirNo, 4]);
            XLS.HucreDegerYaz(groupSatir[satirNo], sutun + 6, toplamAl.toplam[satirNo, 5]);

            XLS.HucreDegerYaz(groupSatir[satirNo], sutun + 7, toplamAl.toplam[satirNo, 3] + toplamAl.toplam[satirNo, 4] + toplamAl.toplam[satirNo, 5]);
            XLS.HucreDegerYaz(groupSatir[satirNo], sutun + 8, toplamAl.toplam[satirNo, 6] + toplamAl.toplam[satirNo, 7] + toplamAl.toplam[satirNo, 8]);
            XLS.HucreDegerYaz(groupSatir[satirNo], sutun + 9, toplamAl.toplam[satirNo, 9] + toplamAl.toplam[satirNo, 10] + toplamAl.toplam[satirNo, 11]);

            for (int i = 0; i < 12; i++)
            {
                toplamAl.DegerAta(satirNo, i, 0);
            }
        }

        private DataTable ProjeAnaDT;
        private DataTable AltProjeAnaDT;
        private DataTable ProjeYilDT;
        private DataTable HarcamaDT;
        private DataTable KaynakDT;
        private DataTable KaynakDTKur;

        private static DataTable CreateProjeAnaDT()
        {
            DataTable dt = new DataTable("ProjeAnaDT");

            dt.Columns.Add("PROJEID", typeof(int));
            dt.Columns.Add("BIRIMKODU", typeof(string));
            dt.Columns.Add("PROJEADI", typeof(string));
            dt.Columns.Add("BASLAMAYILI", typeof(int));
            dt.Columns.Add("PROJESAY", typeof(int));

            dt.PrimaryKey = new[] { dt.Columns["PROJEID"] };

            return dt;
        }

        private static DataTable CreateAltProjeAnaDT()
        {
            DataTable dt = new DataTable("AltProjeAnaDT");

            dt.Columns.Add("PROJEID", typeof(int));
            dt.Columns.Add("ALTPROJEID", typeof(int));
            dt.Columns.Add("ALTPROJEADI", typeof(string));
            dt.Columns.Add("ALTPROJESAY", typeof(int));
            dt.PrimaryKey = new[] { dt.Columns["PROJEID"], dt.Columns["ALTPROJEID"] };

            return dt;
        }

        private static DataTable CreateProjeYilDT()
        {
            DataTable dt = new DataTable("ProjeYilDT");

            dt.Columns.Add("YIL", typeof(int));
            dt.Columns.Add("PROJEID", typeof(int));
            dt.Columns.Add("ALTPROJEID", typeof(int));

            dt.Columns.Add("BIRIMKODU", typeof(string));
            dt.Columns.Add("PROJEADI", typeof(string));
            dt.Columns.Add("YERI", typeof(string));
            dt.Columns.Add("KARAKTERISTIGI", typeof(string));
            dt.Columns.Add("BASLAMAYILI", typeof(int));
            dt.Columns.Add("BITISYILI", typeof(int));

            dt.Columns.Add("PROJETUTARI_DISKREDI", typeof(double));
            dt.Columns.Add("PROJETUTARI_TOPLAM", typeof(double));
            dt.Columns.Add("KUMULATIF_DISKREDI", typeof(double));
            dt.Columns.Add("KUMULATIF_TOPLAM", typeof(double));
            dt.Columns.Add("TOPLAMDISI_HIBE", typeof(double));
            dt.Columns.Add("TOPLAMDISI_DIGER", typeof(double));
            dt.Columns.Add("DISKREDI", typeof(double));
            dt.Columns.Add("TOPLAM", typeof(double));

            dt.PrimaryKey = new[] { dt.Columns["YIL"], dt.Columns["PROJEID"], dt.Columns["ALTPROJEID"] };

            return dt;
        }

        private static DataTable CreateHarcamaDT()
        {
            DataTable dt = new DataTable("HarcamaDT");

            dt.Columns.Add("YIL", typeof(int));
            dt.Columns.Add("PROJEID", typeof(int));
            dt.Columns.Add("ALTPROJEID", typeof(int));
            dt.Columns.Add("HARCAMA_DISKREDI", typeof(double));
            dt.Columns.Add("HARCAMA_TOPLAM", typeof(double));

            dt.PrimaryKey = new[] { dt.Columns["YIL"], dt.Columns["PROJEID"], dt.Columns["ALTPROJEID"] };

            return dt;
        }

        private static DataTable CreateKaynakDT()
        {
            DataTable dt = new DataTable("EkonomikDT");

            dt.Columns.Add("YIL", typeof(int));
            dt.Columns.Add("ALTPROJEID", typeof(int));
            dt.Columns.Add("KURKOD", typeof(string));
            dt.Columns.Add("EKOKOD", typeof(string));

            dt.Columns.Add("EKOAD", typeof(string));


            dt.Columns.Add("PROJETUTARI_DISKREDI", typeof(double));
            dt.Columns.Add("PROJETUTARI_TOPLAM", typeof(double));
            dt.Columns.Add("KUMULATIF_DISKREDI", typeof(double));
            dt.Columns.Add("KUMULATIF_TOPLAM", typeof(double));
            dt.Columns.Add("TOPLAMDISI_HIBE", typeof(double));
            dt.Columns.Add("TOPLAMDISI_DIGER", typeof(double));
            dt.Columns.Add("DISKREDI", typeof(double));
            dt.Columns.Add("TOPLAM", typeof(double));

            dt.PrimaryKey = new[] { dt.Columns["YIL"], dt.Columns["ALTPROJEID"], dt.Columns["KURKOD"], dt.Columns["EKOKOD"] };

            return dt;
        }

        private static DataTable CreateKaynakDTKur()
        {
            DataTable dt = new DataTable("EkonomikDTKur");

            dt.Columns.Add("YIL", typeof(int));
            dt.Columns.Add("KURKOD", typeof(string));
            dt.Columns.Add("EKOKOD", typeof(string));
            dt.Columns.Add("EKOAD", typeof(string));

            dt.Columns.Add("PROJETUTARI_DISKREDI", typeof(double));
            dt.Columns.Add("PROJETUTARI_TOPLAM", typeof(double));
            dt.Columns.Add("KUMULATIF_DISKREDI", typeof(double));
            dt.Columns.Add("KUMULATIF_TOPLAM", typeof(double));
            dt.Columns.Add("TOPLAMDISI_HIBE", typeof(double));
            dt.Columns.Add("TOPLAMDISI_DIGER", typeof(double));
            dt.Columns.Add("DISKREDI", typeof(double));
            dt.Columns.Add("TOPLAM", typeof(double));

            dt.PrimaryKey = new[] { dt.Columns["YIL"], dt.Columns["KURKOD"], dt.Columns["EKOKOD"] };

            return dt;
        }

        private static void DegerArtir(DataRow row, string kolonAdi, double tutar)
        {
            if (row != null)
                row[kolonAdi] = (double)((double)row[kolonAdi] + tutar);
        }

        int projeSay = 0;
        private DataRow ProjeAnaSatiriYoksaEkle(Proje proje, string kurKod)
        {
            DataRow row = ProjeAnaDT.Rows.Find(proje.kod);

            if (row != null)
                return row;

            row = ProjeAnaDT.NewRow();

            row["PROJEID"] = proje.kod;
            row["BIRIMKODU"] = (kurKod.Length > 6) ? kurKod : proje.birim.kod;
            row["PROJEADI"] = proje.ad;
            row["BASLAMAYILI"] = proje.baslangicYili;
            row["PROJESAY"] = ++projeSay;

            ProjeAnaDT.Rows.Add(row);

            return row;
        }

        private DataRow ProjeYilSatiriYoksaEkle(int yil, int projeId, int altProjeId, string birimKodu, string projeAdi, string yeri, string karakteristigi, int baslamaYili, int bitisYili)
        {
            DataRow row = ProjeYilDT.Rows.Find(new object[] { yil, projeId, altProjeId });

            if (row != null || projeAdi == null)
                return row;

            row = ProjeYilDT.NewRow();

            row["YIL"] = yil;
            row["PROJEID"] = projeId;
            row["ALTPROJEID"] = altProjeId;
            row["BIRIMKODU"] = birimKodu;
            row["PROJEADI"] = projeAdi;
            row["YERI"] = yeri;
            row["KARAKTERISTIGI"] = karakteristigi;
            row["BASLAMAYILI"] = baslamaYili;
            row["BITISYILI"] = bitisYili;

            row["PROJETUTARI_DISKREDI"] = (double)0;
            row["PROJETUTARI_TOPLAM"] = (double)0;
            row["KUMULATIF_DISKREDI"] = (double)0;
            row["KUMULATIF_TOPLAM"] = (double)0;
            row["TOPLAMDISI_HIBE"] = (double)0;
            row["TOPLAMDISI_DIGER"] = (double)0;
            row["DISKREDI"] = (double)0;
            row["TOPLAM"] = (double)0;

            ProjeYilDT.Rows.Add(row);

            return row;
        }

        int altProjeSay = 0;
        private DataRow AltProjeAnaSatiriYoksaEkle(int projeId, AltProje alt)
        {
            DataRow row = AltProjeAnaDT.Rows.Find(new object[] { projeId, alt.kod });

            if (row != null)
                return row;

            row = AltProjeAnaDT.NewRow();

            row["PROJEID"] = projeId;
            row["ALTPROJEID"] = alt.kod;
            row["ALTPROJEADI"] = alt.ad;
            row["ALTPROJESAY"] = ++altProjeSay;

            AltProjeAnaDT.Rows.Add(row);

            return row;
        }

        private DataRow HarcamaSatiriYoksaEkle(int yil, int projeId, int altProjeId)
        {
            DataRow row = HarcamaDT.Rows.Find(new object[] { yil, projeId, altProjeId });

            if (row != null)
                return row;

            row = HarcamaDT.NewRow();

            row["YIL"] = yil;
            row["PROJEID"] = projeId;
            row["ALTPROJEID"] = altProjeId;
            row["HARCAMA_DISKREDI"] = (double)0;
            row["HARCAMA_TOPLAM"] = (double)0;

            HarcamaDT.Rows.Add(row);

            return row;
        }

        private DataRow KaynakSatirYoksaEkle(int yil, int altProjeId, string kurKod, string ekoKod, string ekoAd)
        {
            DataRow row = KaynakDT.Rows.Find(new object[] { yil, altProjeId, kurKod, ekoKod });

            if (row != null)
                return row;

            row = KaynakDT.NewRow();

            row["YIL"] = yil;
            row["ALTPROJEID"] = altProjeId;
            row["KURKOD"] = kurKod;
            row["EKOKOD"] = ekoKod;
            row["EKOAD"] = ekoAd;

            row["PROJETUTARI_DISKREDI"] = (double)0;
            row["PROJETUTARI_TOPLAM"] = (double)0;
            row["KUMULATIF_DISKREDI"] = (double)0;
            row["KUMULATIF_TOPLAM"] = (double)0;
            row["TOPLAMDISI_HIBE"] = (double)0;
            row["TOPLAMDISI_DIGER"] = (double)0;
            row["DISKREDI"] = (double)0;
            row["TOPLAM"] = (double)0;
            KaynakDT.Rows.Add(row);

            return row;
        }

        private void EkoBilgiEkle(int yilBolum, int projeKod, ButceKaynak bk, string kurKod, double odenekDis, double odenekToplam)
        {
            DataRow projeYilRow = ProjeYilSatiriYoksaEkle(yilBolum, projeKod, 0, kurKod, null, null, null, 0, 0);
            DataRow altYilRow = ProjeYilSatiriYoksaEkle(yilBolum, projeKod, bk.altProjeKodu, kurKod, null, null, null, 0, 0);
            DataRow kaynakRow = null;
            if (bk.DPTRapordaGor)
            {
                kaynakRow = KaynakSatirYoksaEkle(yilBolum, bk.projeNo, kurKod, bk.kaynakAd, bk.kaynakAd);
            }

            KumulatifOdenekEkle(bk, odenekDis, odenekToplam, odenekDis, odenekToplam, altYilRow, projeYilRow, kaynakRow);
        }

        private void EkonomikDagilimiTekSeferdeYukle(int yil, string senaryoKod, string kurKod)
        {
            string[] sql = new string[1];
            string[] baglanti = new string[1];

            sql[0] = $@"SELECT BK.KAYNAKNO, AP.ALTPROJENO, AP.BIRIM, O.FINKOD, BK.KAYNAKTUR, BK.TOPLAMADAHIL, BK.KAYNAKAD, BK.DPTRAPORDAGOR, AP.PROJENO, SUM(O.ODENEK) FROM ButceIslemleri.dbo.MPGMASRAFPLANIREVIZEFORM F
  INNER JOIN ButceIslemleri.dbo.MPGMASRAFPLANITERTIP O ON O.YIL=F.YIL AND O.REVIZENO=F.REVIZENO
  INNER JOIN Yatirim.dbo.YATBUTCEKAYNAKPEBKOD PK ON PK.YIL=O.YIL AND PK.SENARYOKOD={OrtakFonksiyonlar.Tirnakla(senaryoKod.Replace(".", ""))} AND PK.PEBKOD=O.PEBKOD
  INNER JOIN Yatirim.dbo.YATBUTCEKAYNAK BK ON BK.YIL=PK.YIL AND BK.SENARYOKOD=PK.SENARYOKOD AND BK.KAYNAKNO=PK.KAYNAKNO
  INNER JOIN Yatirim.dbo.YATALTPROJE AP ON AP.YIL=BK.YIL AND AP.SENARYOKOD=BK.SENARYOKOD AND AP.ALTPROJENO=BK.ALTPROJENO
  where  F.YIL={yil} AND F.FORMDURUM=5 AND AP.BIRIM LIKE {OrtakFonksiyonlar.Tirnakla(kurKod.Replace(".", "") + "%")}
group by BK.KAYNAKNO, AP.ALTPROJENO, AP.BIRIM, O.FINKOD, BK.KAYNAKTUR, BK.TOPLAMADAHIL, BK.KAYNAKAD, BK.DPTRAPORDAGOR, AP.PROJENO
order by AP.ALTPROJENO, AP.BIRIM, O.FINKOD";

            baglanti[0] = "BaglantiSatiriBHMv2";
            SortedList sl = TNS.UZY.Arac.Tanimla().SorguBirlestir(sql, baglanti, new int[] { 0, 1, 2, 3 });
            foreach (DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                ButceKaynak bk = new ButceKaynak();

                bk.kaynakNo = alAnahtar.IntAl(0);
                bk.altProjeKodu = alAnahtar.IntAl(1);
                string gKurKod = alAnahtar.StrAl(2);
                string gFinKod = alAnahtar.StrAl(3);

                bk.kaynakTur = (ENUMButceKaynakTur)alVeri.IntAl(0);
                bk.toplamaDahil = (ButceKaynak.ENUMKaynakToplam)alVeri.IntAl(1);
                bk.kaynakAd = alVeri.StrAl(2);
                bk.DPTRapordaGor = alVeri.IntAl(3) == 1;
                int projeNo = alVeri.IntAl(4);
                double odenekToplam = alVeri.DblAl(5);
                double odenekDis;
                if (gFinKod == "07")
                    odenekDis = odenekToplam;
                else
                    odenekDis = 0;

                EkoBilgiEkle(1, projeNo, bk, gKurKod, odenekDis, odenekToplam);
            }
        }

        private void DTHazirla(int yilBolum, BilgiVAN bilgiVAN, string kurKod, int islemYil)
        {
            foreach (Proje proje in bilgiVAN.liste)
            {
                DataRow projeAnaRow = ProjeAnaSatiriYoksaEkle(proje, kurKod);

                DataRow projeYilRow = ProjeYilSatiriYoksaEkle(
                    yilBolum,
                    proje.kod,
                    0,
                    (kurKod.Length > 6) ? kurKod : proje.birim.kod,
                    proje.ad,
                    proje.ProjeYeri(),
                    proje.KarakteristikOzet,
                    proje.baslangicYili,
                    proje.bitisYili
                );
                DataRow projeHarcamaRow = null;

                if (yilBolum == 1)
                {
                    projeHarcamaRow = HarcamaSatiriYoksaEkle(yilBolum, proje.kod, 0);
                }

                foreach (AltProje alt in proje.altProjeler)
                {
                    if (!alt.DPTDeVar) continue;
                    AltProjeAnaSatiriYoksaEkle(proje.kod, alt);
                    DataRow altYilRow = ProjeYilSatiriYoksaEkle(
                        yilBolum,
                        proje.kod,
                        alt.kod,
                        alt.birim.kod,
                        alt.ad,
                        alt.ProjeYeri(),
                        alt.KarakteristikOzet,
                        alt.baslangicYil,
                        alt.bitisYil
                    );
                    DataRow altHarcamaRow = null;
                    if (yilBolum == 1)
                    {
                        altHarcamaRow = HarcamaSatiriYoksaEkle(yilBolum, proje.kod, alt.kod);
                    }

                    foreach (ButceKaynak kaynak in alt.butceKaynaklari)
                    {
                        DataRow kaynakRow = null;
                        DataRow kaynakHarcamaRow = null;
                        if (kaynak.DPTRapordaGor)
                        {
                            kaynakRow = KaynakSatirYoksaEkle(yilBolum, alt.kod, alt.birim.kod, kaynak.kaynakAd, kaynak.kaynakAd);
                            if (yilBolum == 1)
                            {
                                kaynakHarcamaRow = null;
                            }
                        }

                        foreach (ButceKaynakTeklif teklif in kaynak.teklifler)
                        {
                            if (teklif.ButceYil == islemYil - 1)
                            {
                                KumulatifHarcamaEkle(kaynak, teklif.KumulatifHarcamaDis, teklif.KumulatifHarcamaToplam, altYilRow, projeYilRow, kaynakRow);
                            }
                            else if (teklif.ButceYil == islemYil)
                            {
                                KumulatifOdenekEkle(kaynak, teklif.ProjeTutarDis, teklif.ProjeTutarToplam, teklif.Odenek0Dis, teklif.Odenek0Toplam, altYilRow, projeYilRow, kaynakRow);

                                if (yilBolum == 1)
                                {
                                    DegerArtir(altHarcamaRow, "HARCAMA_DISKREDI", teklif.Harcama6ayDis);
                                    DegerArtir(projeHarcamaRow, "HARCAMA_DISKREDI", teklif.Harcama6ayDis);
                                    DegerArtir(kaynakHarcamaRow, "HARCAMA_DISKREDI", teklif.Harcama6ayDis);

                                    DegerArtir(altHarcamaRow, "HARCAMA_TOPLAM", teklif.Harcama6ayToplam);
                                    DegerArtir(projeHarcamaRow, "HARCAMA_TOPLAM", teklif.Harcama6ayToplam);
                                    DegerArtir(kaynakHarcamaRow, "HARCAMA_TOPLAM", teklif.Harcama6ayToplam);
                                }
                            }
                            if (kaynak.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeGeneleHaric)
                            {
                                ToplamDisiEkle(kaynak, teklif.ProjeTutarToplam, altYilRow, projeYilRow, kaynakRow);
                            }
                        }
                    }
                }
            }
        }

        private void ToplamDisiEkle(ButceKaynak kaynak, double projeTutarToplam, DataRow altYilRow, DataRow projeYilRow, DataRow kaynakRow)
        {
            if (kaynak.kaynakTur == ENUMButceKaynakTur.HIBE)
            {
                DegerArtir(altYilRow, "TOPLAMDISI_HIBE", projeTutarToplam);
                DegerArtir(projeYilRow, "TOPLAMDISI_HIBE", projeTutarToplam);
                DegerArtir(kaynakRow, "TOPLAMDISI_HIBE", projeTutarToplam);
            }
            else
            {
                DegerArtir(altYilRow, "TOPLAMDISI_DIGER", projeTutarToplam);
                DegerArtir(projeYilRow, "TOPLAMDISI_DIGER", projeTutarToplam);
                DegerArtir(kaynakRow, "TOPLAMDISI_DIGER", projeTutarToplam);
            }
        }

        private void KumulatifOdenekEkle(ButceKaynak kaynak, double projeTutarDis, double projeTutarToplam, double odenek0Dis, double odenek0Toplam, DataRow altYilRow, DataRow projeYilRow, DataRow kaynakRow)
        {
            if (kaynak.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeDahilGeneleHaric ||
                kaynak.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeGeneleDahil)
            {
                DegerArtir(altYilRow, "PROJETUTARI_DISKREDI", projeTutarDis);
                DegerArtir(altYilRow, "PROJETUTARI_TOPLAM", projeTutarToplam);
                DegerArtir(altYilRow, "DISKREDI", odenek0Dis);
                DegerArtir(altYilRow, "TOPLAM", odenek0Toplam);
            }

            if (kaynak.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeHaricGeneleDahil ||
                kaynak.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeGeneleDahil)
            {
                DegerArtir(projeYilRow, "PROJETUTARI_DISKREDI", projeTutarDis);
                DegerArtir(projeYilRow, "PROJETUTARI_TOPLAM", projeTutarToplam);
                DegerArtir(projeYilRow, "DISKREDI", odenek0Dis);
                DegerArtir(projeYilRow, "TOPLAM", odenek0Toplam);
            }
            DegerArtir(kaynakRow, "PROJETUTARI_DISKREDI", projeTutarDis);
            DegerArtir(kaynakRow, "PROJETUTARI_TOPLAM", projeTutarToplam);
            DegerArtir(kaynakRow, "DISKREDI", odenek0Dis);
            DegerArtir(kaynakRow, "TOPLAM", odenek0Toplam);
        }

        private void KumulatifHarcamaEkle(ButceKaynak kaynak, double kumulatifHarcamaDis, double kumulatifHarcamaToplam, DataRow altYilRow, DataRow projeYilRow, DataRow kaynakRow)
        {
            if (kaynak.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeDahilGeneleHaric ||
                kaynak.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeGeneleDahil)
            {
                DegerArtir(altYilRow, "KUMULATIF_DISKREDI", kumulatifHarcamaDis);
                DegerArtir(altYilRow, "KUMULATIF_TOPLAM", kumulatifHarcamaToplam);
            }
            if (kaynak.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeHaricGeneleDahil ||
                kaynak.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeGeneleDahil)
            {
                DegerArtir(projeYilRow, "KUMULATIF_DISKREDI", kumulatifHarcamaDis);
                DegerArtir(projeYilRow, "KUMULATIF_TOPLAM", kumulatifHarcamaToplam);
            }
            DegerArtir(kaynakRow, "KUMULATIF_DISKREDI", kumulatifHarcamaDis);
            DegerArtir(kaynakRow, "KUMULATIF_TOPLAM", kumulatifHarcamaToplam);
        }

        private void SatirYazYeni(Tablo XLS, ref int satir, int sutun, int fsatir, int sonsutun, DataRow r0, DataRow r1, DataRow h, DataRow r2, bool projeSatiri, string ad)
        {
            int kaynakSatir = fsatir + 2;
            if (!projeSatiri) kaynakSatir += 1;
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sonsutun, satir, sutun);

            int icSutun = sutun;

            XLS.HucreDegerYaz(satir, icSutun++, ad);

            if (r0 != null)
            {
                XLS.HucreDegerYaz(satir, icSutun++, (string)r0["YERI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (string)r0["KARAKTERISTIGI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (int)r0["BASLAMAYILI"] + "-" + (int)r0["BITISYILI"]);

                XLS.HucreDegerYaz(satir, icSutun++, (double)r0["PROJETUTARI_DISKREDI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r0["PROJETUTARI_TOPLAM"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r0["KUMULATIF_DISKREDI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r0["KUMULATIF_TOPLAM"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r0["TOPLAMDISI_HIBE"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r0["TOPLAMDISI_DIGER"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r0["DISKREDI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r0["TOPLAM"]);
            }
            else
                icSutun += 11;

            if (r1 != null)
            {
                XLS.HucreDegerYaz(satir, icSutun++, (string)r1["YERI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (string)r1["KARAKTERISTIGI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (int)r1["BASLAMAYILI"] + "-" + (int)r1["BITISYILI"]);

                XLS.HucreDegerYaz(satir, icSutun++, (double)r1["PROJETUTARI_DISKREDI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r1["PROJETUTARI_TOPLAM"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r1["KUMULATIF_DISKREDI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r1["KUMULATIF_TOPLAM"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r1["TOPLAMDISI_HIBE"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r1["TOPLAMDISI_DIGER"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r1["DISKREDI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r1["TOPLAM"]);
            }
            else
                icSutun += 11;

            if (h != null)
            {
                XLS.HucreDegerYaz(satir, icSutun++, (double)h["HARCAMA_DISKREDI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)h["HARCAMA_TOPLAM"]);
            }
            else
                icSutun += 2;

            if (r2 != null)
            {
                XLS.HucreDegerYaz(satir, icSutun++, (string)r2["YERI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (string)r2["KARAKTERISTIGI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (int)r2["BASLAMAYILI"] + "-" + (int)r2["BITISYILI"]);

                XLS.HucreDegerYaz(satir, icSutun++, (double)r2["PROJETUTARI_DISKREDI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r2["PROJETUTARI_TOPLAM"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r2["KUMULATIF_DISKREDI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r2["KUMULATIF_TOPLAM"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r2["TOPLAMDISI_HIBE"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r2["TOPLAMDISI_DIGER"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r2["DISKREDI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)r2["TOPLAM"]);
            }
            else
                icSutun += 11;

            satir++;
        }

        private void YazProjeSatiri(Tablo XLS, ref int satir, int sutun, int fsatir, int sonsutun, int projeId, DataRowView projeDrv)
        {
            DataRow r2 = ProjeYilDT.Rows.Find(new object[] { 2, projeId, 0 });
            DataRow r1 = ProjeYilDT.Rows.Find(new object[] { 1, projeId, 0 });
            DataRow r0 = ProjeYilDT.Rows.Find(new object[] { 0, projeId, 0 });

            DataRow h = HarcamaDT.Rows.Find(new object[] { 1, projeId, 0 });

            SatirYazYeni(XLS, ref satir, sutun, fsatir, sonsutun, r0, r1, h, r2, true, (string)projeDrv["PROJEADI"]);
        }

        private void YazAltProjeSatiri(Tablo XLS, ref int satir, int sutun, int fsatir, int sonsutun, int projeId, int altProjeId, DataRowView altDrv)
        {
            DataRow r2 = ProjeYilDT.Rows.Find(new object[] { 2, projeId, altProjeId });
            DataRow r1 = ProjeYilDT.Rows.Find(new object[] { 1, projeId, altProjeId });
            DataRow r0 = ProjeYilDT.Rows.Find(new object[] { 0, projeId, altProjeId });

            DataRow h = HarcamaDT.Rows.Find(new object[] { 1, projeId, altProjeId });

            SatirYazYeni(XLS, ref satir, sutun, fsatir, sonsutun, r0, r1, h, r2, false, (string)altDrv["ALTPROJEADI"]);
        }

        private void YazEkonomikDagilimSatirlari(Tablo XLS, ref int satir, int sutun, int fsatir, int sonsutun, int altProjeId, string kurKod, string apAd)
        {
            DataView altView = new DataView(KaynakDT);
            altView.RowFilter = "ALTPROJEID = " + altProjeId + " AND KURKOD=" + OrtakFonksiyonlar.Tirnakla(kurKod.Replace(".", ""));
            altView.Sort = "EKOKOD, YIL";

            string sonEko = "";
            foreach (DataRowView altDrv in altView)
            {
                string gelenEko = (string)altDrv["EKOKOD"];
                if (gelenEko != sonEko)
                {
                    if (sonEko != "")
                        satir++;
                    int kaynakSatir = fsatir + 4;
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sonsutun, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun, (string)altDrv["EKOAD"]);
                    sonEko = gelenEko;
                }
                int icSutun = sutun;
                if ((int)altDrv["YIL"] == 0)
                    icSutun += 4;
                else if ((int)altDrv["YIL"] == 1)
                    icSutun += 15;
                else if ((int)altDrv["YIL"] == 2)
                    icSutun += 28;

                XLS.HucreDegerYaz(satir, icSutun++, (double)altDrv["PROJETUTARI_DISKREDI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)altDrv["PROJETUTARI_TOPLAM"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)altDrv["KUMULATIF_DISKREDI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)altDrv["KUMULATIF_TOPLAM"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)altDrv["TOPLAMDISI_HIBE"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)altDrv["TOPLAMDISI_DIGER"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)altDrv["DISKREDI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)altDrv["TOPLAM"]);
            }
            if (sonEko != "")
                satir++;
            else
            {
                OrtakFonksiyonlar.DebugYaz("stratek", $"Bulunamadı: {altView.RowFilter} - {altView.Sort} - {apAd}");
            }
        }

        private string TabloYeni2Yaz(int yil, string senaryoKod, string sektor, string senaryoAd, bool duzey2, bool hepsiniGoster, string kurKod, bool odenekleriGoster)
        {
            BilgiVAN[] projeListesi = new BilgiVAN[2];

            projeListesi[0] = servisYAT.ProjeAltProjeListele(kullanan, yil, senaryoKod, sektor, null, -1, -1, null, false, yil, false, kurKod, -1, true, "", false);
            if (!projeListesi[0].sonuc.islemSonuc)
                throw new RaporHatasi(projeListesi[0].sonuc.hataStr);

            projeListesi[1] = servisYAT.ProjeAltProjeListele(kullanan, yil - 1, "07.00", sektor, null, -1, -1, null, false, yil, false, kurKod, -1, true, "", false);
            if (!projeListesi[1].sonuc.islemSonuc)
                throw new RaporHatasi(projeListesi[1].sonuc.hataStr);

            ProjeAnaDT = CreateProjeAnaDT();
            AltProjeAnaDT = CreateAltProjeAnaDT();
            ProjeYilDT = CreateProjeYilDT();
            HarcamaDT = CreateHarcamaDT();
            KaynakDT = CreateKaynakDT();
            KaynakDTKur = CreateKaynakDTKur();

            DTHazirla(2, projeListesi[0], kurKod, yil);
            DTHazirla(1, projeListesi[1], kurKod, yil - 1);
            DTHazirla(0, projeListesi[1], kurKod, yil - 1);
            EkonomikDagilimiTekSeferdeYukle(yil - 1, "0700", kurKod);

            Tablo XLS = GenelIslemler.NewTablo();
            int kaynakSatir = 0;
            int sutun = 0;
            int satir = 0;
            int fSutun = 0;
            int fSatir = 0;

            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            XLS.HucreAdAdresCoz("FormatSatir", ref fSatir, ref fSutun);
            satir = kaynakSatir;

            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("Asama", senaryoAd);
            string kurAd = YatRaporAraclari.VarsayilanKurumBulAd();
            XLS.HucreAdBulYaz("Kurulus", kurAd);
            int sonSutun = sutun + 36;

            DataView projeView = ProjeAnaDT.DefaultView;
            projeView.Sort = "PROJESAY, BIRIMKODU, BASLAMAYILI, PROJEADI, PROJEID";
            string oncekiKurkod = "";

            foreach (DataRowView projeDrv in projeView)
            {
                DataRow projeRow = projeDrv.Row;

                int projeId = (int)projeRow["PROJEID"];
                string simdikiKurkod = (string)projeRow["BIRIMKODU"];

                if (oncekiKurkod != simdikiKurkod)
                {
                    if (oncekiKurkod != "")
                        BirimKapatYeni(XLS, ref satir, sutun, fSatir, sonSutun, oncekiKurkod);

                    BirimYazYeni(XLS, ref satir, sutun, fSatir, sonSutun, simdikiKurkod);
                    oncekiKurkod = simdikiKurkod;
                }

                YazProjeSatiri(XLS, ref satir, sutun, fSatir, sonSutun, projeId, projeDrv);

                DataView altView = new DataView(AltProjeAnaDT);
                altView.RowFilter = "PROJEID = " + projeId;
                altView.Sort = "ALTPROJESAY, ALTPROJEADI, ALTPROJEID";

                foreach (DataRowView altDrv in altView)
                {
                    int altProjeId = Convert.ToInt32(altDrv["ALTPROJEID"]);

                    YazAltProjeSatiri(XLS, ref satir, sutun, fSatir, sonSutun, projeId, altProjeId, altDrv);

                    YazEkonomikDagilimSatirlari(XLS, ref satir, sutun, fSatir, sonSutun, altProjeId, oncekiKurkod, (string)altDrv["ALTPROJEADI"]);
                }
            }
            if (oncekiKurkod != "")
                BirimKapatYeni(XLS, ref satir, sutun, fSatir, sonSutun, oncekiKurkod);

            XLS.FormulleriSil();
            XLS.SatirSil(0, 6);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void BirimYazYeni(Tablo XLS, ref int satir, int sutun, int fSatir, int sonSutun, string kurkod)
        {
            XLS.HucreKopyala(fSatir, sutun, fSatir, sonSutun, satir, sutun);
            XLS.HucreDegerYaz(satir, sutun, kurkod);

            satir++;
        }

        private void BirimKapatYeni(Tablo XLS, ref int satir, int sutun, int fsatir, int sonsutun, string kurKod)
        {
            DataView altView = new DataView(KaynakDTKur);
            altView.RowFilter = "KURKOD=" + OrtakFonksiyonlar.Tirnakla(kurKod.Replace(".", ""));
            altView.Sort = "EKOKOD, YIL";

            string sonEko = "";
            foreach (DataRowView altDrv in altView)
            {
                string gelenEko = (string)altDrv["EKOKOD"];
                if (gelenEko != sonEko)
                {
                    if (sonEko != "")
                        satir++;
                    int kaynakSatir = fsatir + 1;
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sonsutun, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun, (string)altDrv["EKOAD"]);
                    sonEko = gelenEko;
                }
                int icSutun = sutun;
                if ((int)altDrv["YIL"] == 0)
                    icSutun += 4;
                else if ((int)altDrv["YIL"] == 1)
                    icSutun += 15;
                else if ((int)altDrv["YIL"] == 2)
                    icSutun += 28;

                XLS.HucreDegerYaz(satir, icSutun++, (double)altDrv["PROJETUTARI_DISKREDI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)altDrv["PROJETUTARI_TOPLAM"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)altDrv["KUMULATIF_DISKREDI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)altDrv["KUMULATIF_TOPLAM"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)altDrv["TOPLAMDISI_HIBE"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)altDrv["TOPLAMDISI_DIGER"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)altDrv["DISKREDI"]);
                XLS.HucreDegerYaz(satir, icSutun++, (double)altDrv["TOPLAM"]);
            }
            satir++;
        }

        private string Tablo2Yaz(int yil, string senaryoKod, string sektor, string senaryoAd, bool duzey2, bool hepsiniGoster, string kurKod, bool odenekleriGoster)
        {
            BilgiVAN projeListesi = servisYAT.ProjeAltProjeListele(kullanan, yil, senaryoKod, sektor, null, projeNo, -1, null, false, yil, false, kurKod, altProjeNo, true, "", false);
            if (!projeListesi.sonuc.islemSonuc)
                throw new RaporHatasi(projeListesi.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int kaynakSatir = 0;
            int sutun = 0;
            int satir = 0;

            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("KaynakSatir", ref kaynakSatir, ref sutun);
            satir = kaynakSatir;

            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("Asama", senaryoAd);
            // XLS.HucreAdBulYaz("Sektor", sektor + " - " + servisUZY.UzayDegeriStr(kullanan, "YATSEKTOR", sektor, true));
            string kurAd = YatRaporAraclari.VarsayilanKurumBulAd();
            XLS.HucreAdBulYaz("Kurulus", kurAd);

            if (!duzey2)
            {
                //XLS.SutunGizle(sutun + 1, sutun + 1, true);
                int sutGen = XLS.SutunGenislikAl(sutun + 0);
                XLS.SutunGenislikAyarla(sutun + 1, sutun + 1, sutGen * 2);
            }

            //0=Kurum
            //1=Sektör
            //2=EDY
            //3=BitDevam
            //4=Proje
            //5=AltProje
            ToplamAl toplamAl = new ToplamAl(7, 23);
            string[] groupKod = { "", "", "", "", "" };

            int[] groupSatir = { 0, 0, 0, 0, 0, 0, 0 };

            int sonSutun = sutun + 30;

            XLS.HucreKopyala(kaynakSatir - 9, sutun, kaynakSatir - 9, sonSutun, satir, sutun);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 5);
            XLS.HucreDegerYaz(satir, sutun, kurAd + " TOPLAMI");
            groupSatir[0] = satir;
            int projeSay = 0;
            foreach (Proje p in projeListesi.liste)
            {
                if (groupKod[1] != p.sektor.kod)
                {
                    if (groupSatir[1] > 0)
                    {
                        ToplamYaz(XLS, groupSatir, 1, sutun, toplamAl);
                    }
                    groupKod[1] = p.sektor.kod;
                    groupKod[2] = "";
                    satir++;
                    XLS.HucreKopyala(kaynakSatir - 8, sutun, kaynakSatir - 8, sonSutun, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 5);
                    XLS.HucreDegerYaz(satir, sutun, p.sektor.ad);
                    XLS.HucreDegerYaz(satir, 50, p.sektor.kod);
                    XLS.HucreDegerYaz(satir, 51, p.sektor.ad);
                    groupSatir[1] = satir;
                }
                string etudDevamYeni = p.EtudDevamEdenYeni(yil);
                if (groupKod[2] != etudDevamYeni)
                {
                    if (groupSatir[2] > 0)
                    {
                        ToplamYaz(XLS, groupSatir, 2, sutun, toplamAl);
                    }
                    groupKod[2] = etudDevamYeni;
                    groupKod[3] = "";
                    satir++;
                    XLS.HucreKopyala(kaynakSatir - 7, sutun, kaynakSatir - 7, sonSutun, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 5);
                    XLS.HucreDegerYaz(satir, sutun, TNS.YAT.Arac.EtudDevamEdenYeniAciklama(etudDevamYeni, true));
                    groupSatir[2] = satir;
                    XLS.HucreDegerYaz(satir, 50, p.sektor.kod);
                    XLS.HucreDegerYaz(satir, 51, p.sektor.ad);
                    XLS.HucreDegerYaz(satir, 52, etudDevamYeni);
                }
                int buYilBiter = p.BuYilBiterMi(yil);
                if (groupKod[3] != buYilBiter.ToString())
                {
                    if (groupSatir[3] > 0)
                    {
                        ToplamYaz(XLS, groupSatir, 3, sutun, toplamAl);
                    }
                    groupKod[3] = buYilBiter.ToString();
                    groupKod[4] = "";
                    satir++;
                    XLS.HucreKopyala(kaynakSatir - 6, sutun, kaynakSatir - 6, sonSutun, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 5);
                    XLS.HucreDegerYaz(satir, sutun, TNS.YAT.Arac.BuYilBiterMiAciklama(buYilBiter, yil));
                    groupSatir[3] = satir;
                    projeSay = 0;
                    XLS.HucreDegerYaz(satir, 50, p.sektor.kod);
                    XLS.HucreDegerYaz(satir, 51, p.sektor.ad);
                    XLS.HucreDegerYaz(satir, 52, etudDevamYeni);
                    XLS.HucreDegerYaz(satir, 53, TNS.YAT.Arac.BuYilBiterMiAciklama(buYilBiter, yil));
                }
                int farkSatir = 0;
                if (projeSay++ % 2 == 0) farkSatir = 2;
                satir++;
                XLS.HucreKopyala(kaynakSatir - 5 + farkSatir, sutun, kaynakSatir - 5 + farkSatir, sonSutun, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, p.DPTProjeNo);

                XLS.HucreDegerYaz(satir, 50, p.sektor.kod);
                XLS.HucreDegerYaz(satir, 51, p.sektor.ad);
                XLS.HucreDegerYaz(satir, 52, etudDevamYeni);
                XLS.HucreDegerYaz(satir, 53, TNS.YAT.Arac.BuYilBiterMiAciklama(buYilBiter, yil));
                XLS.HucreDegerYaz(satir, 54, p.DPTProjeNo);
                XLS.HucreDegerYaz(satir, 58, p.yurutecekKurum);

                if (duzey2)
                {
                    XLS.HucreDegerYaz(satir, sutun + 1, IlAciklamaOl(p.ilIlceler));//Düzey2 Bölge Kodu
                }
                XLS.HucreDegerYaz(satir, sutun + 2, p.ad);
                XLS.HucreDegerYaz(satir, sutun + 3, TNS.YAT.Arac.IlIlceAdOl(p.ilIlceler));//Yer
                if (!string.IsNullOrEmpty(p.karakteristikOzet))
                    XLS.HucreDegerYaz(satir, sutun + 4, p.karakteristikOzet);
                else
                    XLS.HucreDegerYaz(satir, sutun + 4, p.karakteristik);
                XLS.HucreDegerYaz(satir, sutun + 5, p.baslangicYili + " / " + p.bitisYili);
                groupSatir[4] = satir;
                toplamAl.SatirSifirla(4);

                foreach (AltProje alt in p.altProjeler)
                {
                    string toplamaDahil = "";
                    if ((p.altProjeler.Count > 1 && alt.DPTDeVar) || hepsiniGoster || (p.altProjeler.Count == 1 && (!alt.ad.Equals(p.ad, StringComparison.OrdinalIgnoreCase) || odenekleriGoster)))
                    {
                        satir++;
                        XLS.HucreKopyala(kaynakSatir - 4 + farkSatir, sutun, kaynakSatir - 4 + farkSatir, sonSutun, satir, sutun);

                        XLS.HucreDegerYaz(satir, sutun + 1, "");
                        XLS.HucreDegerYaz(satir, sutun + 2, "  - " + alt.ad);
                        XLS.HucreDegerYaz(satir, sutun + 3, TNS.YAT.Arac.IlIlceAdOl(alt.ilIlceler));

                        if (!string.IsNullOrEmpty(alt.karakteristikOzet))
                            XLS.HucreDegerYaz(satir, sutun + 4, alt.karakteristikOzet);
                        else
                            XLS.HucreDegerYaz(satir, sutun + 4, alt.karakteristik);

                        XLS.HucreDegerYaz(satir, sutun + 5, alt.baslangicYil + " / " + alt.bitisYil);
                        groupSatir[5] = satir;
                        XLS.HucreDegerYaz(satir, 50, p.sektor.kod);
                        XLS.HucreDegerYaz(satir, 51, p.sektor.ad);
                        XLS.HucreDegerYaz(satir, 52, etudDevamYeni);
                        XLS.HucreDegerYaz(satir, 53, TNS.YAT.Arac.BuYilBiterMiAciklama(buYilBiter, yil));
                        XLS.HucreDegerYaz(satir, 54, p.DPTProjeNo);
                        XLS.HucreDegerYaz(satir, 55, alt.kod);
                        XLS.HucreDegerYaz(satir, 58, alt.birim_kod);
                        if (odenekleriGoster)
                            AltProjeOdenekleriniYaz(XLS, ref satir, sutun, sonSutun, yil, senaryoKod, alt);
                    }
                    foreach (ButceKaynak bk in alt.butceKaynaklari)
                    {
                        foreach (ButceKaynakTeklif teklif in bk.teklifler)
                        {
                            if (teklif.butceYil == yil)
                            {
                                int i = 0;
                                toplamAl.DegerAta(6, 0, teklif.ProjeTutarDis / Bolen);
                                toplamAl.DegerAta(6, 1, teklif.ProjeTutarToplam / Bolen);
                                i = 5;
                                toplamAl.DegerAta(6, i++, teklif.Odenek0Dis / Bolen);
                                toplamAl.DegerAta(6, i++, teklif.Odenek0DisaGiden / Bolen);
                                toplamAl.DegerAta(6, i++, teklif.Odenek0Toplam / Bolen);
                                toplamAl.DegerAta(6, i++, teklif.Odenek1Dis / Bolen);
                                toplamAl.DegerAta(6, i++, teklif.Odenek1DisaGiden / Bolen);
                                toplamAl.DegerAta(6, i++, teklif.Odenek1Toplam / Bolen);
                                toplamAl.DegerAta(6, i++, teklif.Odenek2Dis / Bolen);
                                toplamAl.DegerAta(6, i++, teklif.Odenek2DisaGiden / Bolen);
                                toplamAl.DegerAta(6, i++, teklif.Odenek2Toplam / Bolen);

                                toplamAl.DegerAta(6, i++, teklif.OdenekEk0Dis / Bolen);
                                toplamAl.DegerAta(6, i++, teklif.OdenekEk0DisaGiden / Bolen);
                                toplamAl.DegerAta(6, i++, teklif.OdenekEk0Toplam / Bolen);

                                toplamAl.DegerAta(6, i++, teklif.OdenekEk1Dis / Bolen);
                                toplamAl.DegerAta(6, i++, teklif.OdenekEk1DisaGiden / Bolen);
                                toplamAl.DegerAta(6, i++, teklif.OdenekEk1Toplam / Bolen);

                                toplamAl.DegerAta(6, i++, teklif.OdenekEk2Dis / Bolen);
                                toplamAl.DegerAta(6, i++, teklif.OdenekEk2DisaGiden / Bolen);
                                toplamAl.DegerAta(6, i++, teklif.OdenekEk2Toplam / Bolen);
                            }
                            else if (teklif.butceYil == yil - 1)
                            {
                                toplamAl.DegerAta(6, 2, teklif.KumulatifHarcamaDis / Bolen);
                                toplamAl.DegerAta(6, 3, teklif.KumulatifHarcamaDisaGiden / Bolen);
                                toplamAl.DegerAta(6, 4, teklif.KumulatifHarcamaToplam / Bolen);
                            }
                        }
                        if (bk.kaynakTur != ENUMButceKaynakTur.HIBE)
                        {
                            toplamAl.ToplamEkle(5, 6);
                            if (bk.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeGeneleDahil || bk.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeDahilGeneleHaric)
                                toplamAl.ToplamEkle(4, 6);

                            if (bk.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeGeneleDahil || bk.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeHaricGeneleDahil)
                            {
                                toplamAl.ToplamEkle(3, 6);
                                toplamAl.ToplamEkle(2, 6);
                                toplamAl.ToplamEkle(1, 6);
                                toplamAl.ToplamEkle(0, 6);
                            }
                        }
                        if (bk.DPTRapordaGor || hepsiniGoster)
                        {
                            satir++;
                            XLS.HucreKopyala(kaynakSatir - 4 + farkSatir, sutun, kaynakSatir - 4 + farkSatir, sonSutun, satir, sutun);
                            XLS.HucreDegerYaz(satir, sutun + 1, "");
                            XLS.HucreDegerYaz(satir, sutun + 2, "    - (" + bk.kaynakAd + ")");
                            groupSatir[6] = satir;
                            ToplamYaz(XLS, groupSatir, 6, sutun, toplamAl);
                            XLS.HucreDegerYaz(satir, 50, p.sektor.kod);
                            XLS.HucreDegerYaz(satir, 51, p.sektor.ad);
                            XLS.HucreDegerYaz(satir, 52, etudDevamYeni);
                            XLS.HucreDegerYaz(satir, 53, TNS.YAT.Arac.BuYilBiterMiAciklama(buYilBiter, yil));
                            XLS.HucreDegerYaz(satir, 54, p.DPTProjeNo);
                            XLS.HucreDegerYaz(satir, 55, alt.kod);
                            XLS.HucreDegerYaz(satir, 56, bk.toplamaDahil.ToString());
                            XLS.HucreDegerYaz(satir, 57, bk.kaynakNo);
                            XLS.HucreDegerYaz(satir, 58, bk.birim);
                        }
                        toplamaDahil = ToplamDahilEkle(toplamaDahil, bk.toplamaDahil.ToString());
                    }

                    if ((p.altProjeler.Count > 1 && alt.DPTDeVar) || hepsiniGoster)
                    {
                        ToplamYaz(XLS, groupSatir, 5, sutun, toplamAl);
                        XLS.HucreDegerYaz(groupSatir[5], 56, toplamaDahil);
                    }
                    else
                        toplamAl.SatirSifirla(5);
                }
                ToplamYaz(XLS, groupSatir, 4, sutun, toplamAl);
            }
            ToplamYaz(XLS, groupSatir, 3, sutun, toplamAl);
            ToplamYaz(XLS, groupSatir, 2, sutun, toplamAl);
            ToplamYaz(XLS, groupSatir, 1, sutun, toplamAl);
            ToplamYaz(XLS, groupSatir, 0, sutun, toplamAl);
            if (!duzey2)
                XLS.SutunSil(2, 2);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void AltProjeOdenekleriniYaz(Tablo XLS, ref int satir, int sutun, int sonSutun, int butceYil, string senaryoKod, AltProje alt)
        {
            ToplamAl toplamAl = new ToplamAl(1, 23);
            int[] groupSatir = { 0 };
            ObjectArray usSatirlar = servisYAT.OdenekListesiBulEko2(kullanan, butceYil, senaryoKod, alt.proje.kod, alt.kod);

            string oncEko = "";
            string oncEkoAd = "";

            foreach (OdenekBilgi us in usSatirlar.objeler)
            {
                string simdEko = us.tertip.kodlar[3];
                if (string.IsNullOrWhiteSpace(simdEko)) continue;
                if (simdEko != oncEko)
                {
                    if (oncEko != "")
                    {
                        XLS.HucreKopyala(0, sutun, 0, sonSutun, groupSatir[0], sutun);
                        ToplamYaz(XLS, groupSatir, 0, sutun, toplamAl);
                        XLS.HucreDegerYaz(groupSatir[0], sutun + 2, oncEko);
                        XLS.HucreDegerYaz(groupSatir[0], sutun + 3, oncEkoAd);
                    }
                    oncEkoAd = us.tertip.adlar[3];
                    oncEko = simdEko;
                    satir++;
                    groupSatir[0] = satir;
                }

                int fark;
                if (us.tertip.kodlar[2] == "07")
                    fark = 0;
                else
                    fark = 1;

                int topSut = 5;
                for (int j = 0; j < 6; j++)
                {
                    toplamAl.DegerArttir(0, topSut + fark, (double)us.ekRakamlar[j]);
                    toplamAl.DegerArttir(0, topSut + 2, (double)us.ekRakamlar[j]);
                    topSut += 3;
                }
            }

            if (oncEko != "")
            {
                XLS.HucreKopyala(0, sutun, 0, sonSutun, groupSatir[0], sutun);
                ToplamYaz(XLS, groupSatir, 0, sutun, toplamAl);
                XLS.HucreDegerYaz(groupSatir[0], sutun + 2, oncEko);
                XLS.HucreDegerYaz(groupSatir[0], sutun + 3, oncEkoAd);
            }
        }

        private string TertipKoduYap(UcgenSatir us, string[] tumKodlar)
        {
            tumKodlar[us.yer] = us.kodParca;
            for (int i = us.yer + 1; i < tumKodlar.Length; i++)
            {
                tumKodlar[i] = "";
            }
            string donusDeger = "";
            for (int i = 0; i < tumKodlar.Length && tumKodlar[i] != ""; i++)
            {
                string ek;
                if (i == 0) ek = "";
                else if (i == 2 || i == 6 || i == 7 || i == 11) ek = "-";
                else ek = ".";
                donusDeger += ek + tumKodlar[i];
            }
            return donusDeger;
        }

        private string ToplamDahilEkle(string toplamaDahil, string td)
        {
            if (string.IsNullOrWhiteSpace(toplamaDahil)) return td;
            else if (toplamaDahil.IndexOf(td) < 0) return toplamaDahil + ", " + td;
            return toplamaDahil;
        }

        private string IlAciklamaOl(TNSCollection ilIlceler)
        {
            string donDeger = "";
            if (ilIlceler == null || ilIlceler.Count == 0) return donDeger;
            for (int i = 0; i < ilIlceler.Count; i++)
            {
                ProjeIlIlce ilIce = (ProjeIlIlce)ilIlceler[i];
                string ad = ilIce.ilIlce.il.aciklama;
                if (!string.IsNullOrEmpty(ad))
                {
                    bool adVar = true;
                    if (donDeger == "") adVar = false;
                    else adVar = donDeger.Contains(ad);
                    if (!adVar)
                    {
                        if (donDeger != "") donDeger += ", ";
                        donDeger += ad;
                    }
                }
            }
            return donDeger;
        }

        private void ToplamYaz(Tablo XLS, int[] groupSatir, int satirNo, int sutun, ToplamAl toplamAl)
        {
            for (int i = 0; i < 23; i++)
            {
                XLS.HucreDegerYaz(groupSatir[satirNo], sutun + 6 + i, toplamAl.toplam[satirNo, i]);
                toplamAl.DegerAta(satirNo, i, 0);
            }
        }

        private string Tablo3Yaz(int yil, string senaryoKod, string sektor, string kurKod)
        {
            BilgiVAN projeListesi = servisYAT.ProjeAltProjeListele(kullanan, yil, senaryoKod, sektor, null, projeNo, -1, null, false, yil - 2, true, kurKod, altProjeNo, true, "", false);
            if (!projeListesi.sonuc.islemSonuc)
                throw new RaporHatasi(projeListesi.sonuc.hataStr);
            //if (projeListesi.liste.Count <= 0)
            //    throw new RaporHatasi(projeListesi.sonuc.bilgiStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            int yuk = XLS.SatirYukseklikAl(kaynakSatir);
            XLS.HucreAdBulYaz("Yil", yil - 1);
            XLS.HucreAdBulYaz("Kurulus", YatRaporAraclari.VarsayilanKurumBulAd());

            satir = kaynakSatir - 1;

            //0=Kurum
            //1=Sektör
            //2=Proje
            ToplamAl toplamAl = new ToplamAl(3, 7);
            string[] groupKod = { "", "", "" };

            int[] groupSatir = { 0, 0, 0 };


            foreach (Proje p in projeListesi.liste)
            {
                if (groupKod[1] != p.sektor.kod)
                {
                    if (groupSatir[1] > 0)
                    {
                        ToplamYazT3(XLS, groupSatir, 1, sutun, toplamAl);
                    }
                    groupKod[1] = p.sektor.kod;
                    groupKod[2] = "";
                    satir++;
                    XLS.HucreKopyala(kaynakSatir - 2, sutun, kaynakSatir - 2, sutun + 10, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 1);
                    XLS.HucreDegerYaz(satir, sutun, p.sektor.kod + " " + p.sektor.ad);
                    groupSatir[1] = satir;
                }

                foreach (AltProje alt in p.altProjeler)
                {
                    foreach (ButceKaynak bk in alt.butceKaynaklari)
                    {
                        if (bk.kaynakTur == ENUMButceKaynakTur.HIBE) continue;
                        foreach (ButceKaynakTeklif teklif in bk.teklifler)
                        {
                            double ode = teklif.Odenek0Toplam;
                            if (teklif.butceYil == yil - 2)
                            {
                                toplamAl.DegerArttir(2, 0, ode / Bolen);
                                toplamAl.DegerArttir(2, 1, (ode + teklif.EklenenYilSonuToplam + teklif.DusulenYilSonuToplam) / Bolen); //Revize Ödenek
                                toplamAl.DegerArttir(2, 2, teklif.HarcamaYilSonuToplam / Bolen);
                            }
                            else if (teklif.butceYil == yil - 1)
                            {
                                toplamAl.DegerArttir(2, 3, ode / Bolen);
                                toplamAl.DegerArttir(2, 4, (ode + teklif.EklenenYilSonuToplam + teklif.DusulenYilSonuToplam) / Bolen); //Revize Ödenek
                                toplamAl.DegerArttir(2, 5, teklif.Harcama6ayToplam / Bolen);
                                toplamAl.DegerArttir(2, 6, teklif.HarcamaTahminToplam / Bolen);
                            }
                        }
                    }
                }

                satir++;
                XLS.HucreKopyala(kaynakSatir - 1, sutun, kaynakSatir - 1, sutun + 10, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun, p.DPTProjeNo);
                XLS.HucreDegerYaz(satir, sutun + 1, p.ad);
                groupSatir[2] = satir;
                ToplamYazT3(XLS, groupSatir, 2, sutun, toplamAl);

            }
            if (groupSatir[1] > 0)
            {
                ToplamYazT3(XLS, groupSatir, 1, sutun, toplamAl);
            }

            //satir++;
            //XLS.HucreKopyala(kaynakSatir - 1, sutun, kaynakSatir - 1, sutun + 10, satir, sutun);
            //XLS.HucreDegerYaz(satir, sutun, "T O P L A M");
            //groupSatir[0] = satir;
            //ToplamYazT1(XLS, groupSatir, 0, sutun, toplamAl);
            XLS.SatirYukseklikAyarla(kaynakSatir, satir, yuk);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void ToplamYazT3(Tablo XLS, int[] groupSatir, int satirNo, int sutun, ToplamAl toplamAl)
        {
            if (satirNo == 2)
            {
                toplamAl.ToplamEkle(0, 2);
                toplamAl.ToplamEkle(1, 2);
            }

            for (int i = 0; i < toplamAl.SutunSay(); i++)
            {
                XLS.HucreDegerYaz(groupSatir[satirNo], sutun + 2 + i, toplamAl.toplam[satirNo, i]);
                toplamAl.DegerAta(satirNo, i, 0);
            }
        }

        private string Tablo4Yaz(int yil, string senaryoKod, string sektor, string kurKod)
        {
            BilgiVAN ptler = servisYAT.ProjeTurListele(kullanan, new Sozluk());
            if (!ptler.sonuc.islemSonuc)
                throw new RaporHatasi(ptler.sonuc.hataStr);
            //if (ptler.liste.Count <= 0)
            //    throw new RaporHatasi(ptler.sonuc.bilgiStr);

            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            int yuk = XLS.SatirYukseklikAl(kaynakSatir);

            string[] adres = { "Yil", "ProjeTuru", "Sektor", "Kurulus", "Baslik" };
            string[] kriter = { yil.ToString(), string.Empty, sektor + " - " + servisUZY.UzayDegeriStr(kullanan, "YATSEKTOR", sektor, true), YatRaporAraclari.VarsayilanKurumBulAd(), string.Empty };
            int[] kriterSatir = new int[adres.Length];
            int[] kriterSutun = new int[adres.Length];
            for (int i = 0; i < adres.Length; i++)
                XLS.HucreAdAdresCoz(adres[i], ref kriterSatir[i], ref kriterSutun[i]);

            for (int i = 0; i < ptler.liste.Count - 1; i++)
                XLS.YeniSheetEkle(0);

            for (int k = 0; k < ptler.liste.Count; k++)
            {
                XLS.AktifSheetDegistir(k);

                Sozluk s = (Sozluk)ptler.liste[k];

                kriter[1] = s.kod.ToString() + " - " + s.ad;
                kriter[4] = "Tablo-6: " + s.ad + " YATIRIMLARI";
                for (int j = 0; j < kriter.Length; j++)
                    XLS.HucreDegerYaz(kriterSatir[j], kriterSutun[j], kriter[j]);

                BilgiVAN bilgi = servisYAT.ProjeAltProjeListele(kullanan, yil, senaryoKod, sektor, null, projeNo, s.kod, null, false, yil - 2, true, kurKod, altProjeNo, true, "", false);

                if (!bilgi.sonuc.islemSonuc)
                    throw new RaporHatasi(bilgi.sonuc.hataStr);
                if (bilgi.liste.Count <= 0)
                    continue;

                satir = kaynakSatir - 1;
                foreach (Proje p in bilgi.liste)
                {
                    satir++;
                    XLS.HucreKopyala(kaynakSatir - 1, sutun, kaynakSatir - 1, sutun + 5, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 1);

                    XLS.HucreDegerYaz(satir, sutun, p.ad);

                    double butceDis = 0;
                    double butce = 0;

                    foreach (AltProje alt in p.altProjeler)
                    {
                        foreach (ButceKaynak bk in alt.butceKaynaklari)
                        {
                            foreach (ButceKaynakTeklif teklif in bk.teklifler)
                            {
                                if (teklif.butceYil == yil)
                                {
                                    butceDis += teklif.Odenek0Dis;
                                    butce += teklif.Odenek0Toplam;
                                }
                            }
                        }
                    }

                    XLS.HucreDegerYaz(satir, sutun + 2, butceDis / Bolen);
                    XLS.HucreDegerYaz(satir, sutun + 3, butce / Bolen);
                    XLS.HucreDegerYaz(satir, sutun + 4, p.karakteristik);
                }
                XLS.SatirYukseklikAyarla(kaynakSatir, satir, yuk);
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public class ButceKaynakTeklifSirala : IComparer
        {
            public int Compare(object x, object y)
            {
                int sonuc = 0;
                if (x is ButceKaynakTeklif && y is ButceKaynakTeklif)
                {
                    ButceKaynakTeklif x1 = (ButceKaynakTeklif)x;
                    ButceKaynakTeklif y1 = (ButceKaynakTeklif)y;
                    sonuc = x1.butceYil.CompareTo(y1.butceYil);
                }
                return sonuc;
            }
        }

        private string Tablo5Yaz(int yil, string senaryoKod, string sektor, string kurKod)
        {
            BilgiVAN projeler = servisYAT.ProjeAltProjeListele(kullanan, yil, senaryoKod, sektor, null, projeNo, -1, null, false, yil - 100, true, kurKod, altProjeNo, true, "", false);

            if (!projeler.sonuc.islemSonuc)
                throw new RaporHatasi(projeler.sonuc.hataStr);
            //if (projeler.liste.Count <= 0)
            //    throw new RaporHatasi(projeler.sonuc.bilgiStr);

            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            int satYukseklik = XLS.SatirYukseklikAl(kaynakSatir);
            string[] adres = { "Sektor", "YurutecekKurum", "ProjeAdi", "ProjeNo", "ProjeYeri", "ProjeTarihi", "ProjeKarakteri" };
            string[] kriter = new string[adres.Length];
            int[] kriterSatir = new int[adres.Length];
            int[] kriterSutun = new int[adres.Length];
            for (int i = 0; i < adres.Length; i++)
                XLS.HucreAdAdresCoz(adres[i], ref kriterSatir[i], ref kriterSutun[i]);

            for (int i = 0; i < projeler.liste.Count - 1; i++)
                XLS.YeniSheetEkle(0);

            for (int k = 0; k < projeler.liste.Count; k++)
            {
                XLS.AktifSheetDegistir(k);

                Proje p = (Proje)projeler.liste[k];
                kriter[0] = p.sektor.kod + " - " + p.sektor.ad;
                kriter[1] = p.yurutecekKurumAd;
                kriter[2] = p.ad;
                kriter[3] = p.DPTProjeNo;
                kriter[4] = p.ProjeYeri();
                kriter[5] = p.baslangicYili + " / " + p.bitisYili;
                if (altProjeNo <= 0)
                    kriter[6] = p.KarakteristikOzet;
                else
                {
                    if (p.altProjeler.Count >= 1)
                    {
                        AltProje ap = (AltProje)p.altProjeler[0];
                        kriter[6] = ap.KarakteristikOzet;
                    }
                    else
                        kriter[6] = "";
                }

                for (int j = 0; j < kriter.Length; j++)
                    XLS.HucreDegerYaz(kriterSatir[j], kriterSutun[j], kriter[j]);

                ArrayList al = new ArrayList();
                foreach (AltProje alt in p.altProjeler)
                {
                    foreach (ButceKaynak bk in alt.butceKaynaklari)
                    {
                        if (bk.kaynakTur == ENUMButceKaynakTur.HIBE) continue;
                        foreach (ButceKaynakTeklif teklif in bk.teklifler)
                        {
                            KaynakBilgiEkle(al, teklif);
                        }
                    }
                }
                al.Sort(new ButceKaynakTeklifSirala());
                int yuks = XLS.SatirYukseklikAl(kaynakSatir);
                //yuks = GenelIslemler.JexcelBirimtoExcelBirim(360);
                satir = kaynakSatir - 1;
                foreach (ButceKaynakTeklif teklif in al)
                {
                    if (teklif.butceYil >= yil) continue;
                    satir++;
                    XLS.HucreKopyala(kaynakSatir - 1, sutun, kaynakSatir - 1, sutun + 9, satir, sutun);

                    XLS.HucreDegerYaz(satir, sutun, teklif.butceYil);
                    XLS.HucreDegerYaz(satir, sutun + 1, teklif.ProjeTutarDis / Bolen);
                    XLS.HucreDegerYaz(satir, sutun + 2, teklif.ProjeTutarToplam / Bolen);
                    XLS.HucreDegerYaz(satir, sutun + 3, teklif.Odenek0Dis / Bolen);
                    XLS.HucreDegerYaz(satir, sutun + 4, teklif.Odenek0Toplam / Bolen);
                    XLS.HucreDegerYaz(satir, sutun + 5, (teklif.Odenek0Dis + teklif.EklenenYilSonuDis + teklif.DusulenYilSonuDis) / Bolen);
                    double yilSonuOdenek = (teklif.Odenek0Toplam + teklif.EklenenYilSonuToplam + teklif.DusulenYilSonuToplam);
                    XLS.HucreDegerYaz(satir, sutun + 6, yilSonuOdenek / Bolen);
                    XLS.HucreDegerYaz(satir, sutun + 7, teklif.HarcamaYilSonuDis / Bolen);
                    XLS.HucreDegerYaz(satir, sutun + 8, teklif.HarcamaYilSonuToplam / Bolen);
                    if (teklif.Odenek0Toplam > 0)
                        XLS.HucreDegerYaz(satir, sutun + 9, (teklif.HarcamaYilSonuToplam * 100 / yilSonuOdenek));
                }
                satir++;
                XLS.HucreDegerYaz(satir, sutun, "(*) GERÇEKLEŞME YÜZDESİ=(HARCAMA/PROGRAM ÖDENEĞİ)*100 olarak verilecektir.");
                XLS.SatirYukseklikAyarla(kaynakSatir, satir, yuks);
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void KaynakBilgiEkle(ArrayList al, ButceKaynakTeklif teklif)
        {
            ButceKaynakTeklif kaynak = null;
            foreach (ButceKaynakTeklif item in al)
            {
                if (item.butceYil == teklif.butceYil)
                {
                    kaynak = item;
                    break;
                }
            }

            if (kaynak == null)
            {
                kaynak = new ButceKaynakTeklif();
                kaynak.butceYil = teklif.butceYil;
                al.Add(kaynak);
            }
            kaynak.ProjeTutarDis += teklif.ProjeTutarDis;
            kaynak.ProjeTutarToplam += teklif.ProjeTutarToplam;

            kaynak.Odenek0Dis += teklif.Odenek0Dis;
            kaynak.Odenek0Toplam += teklif.Odenek0Toplam;

            kaynak.EklenenYilSonuToplam += teklif.EklenenYilSonuToplam;
            kaynak.EklenenYilSonuDis += teklif.EklenenYilSonuDis;

            kaynak.DusulenYilSonuDis += teklif.DusulenYilSonuDis;
            kaynak.DusulenYilSonuToplam += teklif.DusulenYilSonuToplam;

            kaynak.HarcamaYilSonuToplam += teklif.HarcamaYilSonuToplam;
            kaynak.HarcamaYilSonuDis += teklif.HarcamaYilSonuDis;
        }

        private string Tablo6Yaz(int yil, string senaryoKod, string sektor, string kurKod)
        {
            ENUMButceKaynakTur[] bkTur = { ENUMButceKaynakTur.HIBE, ENUMButceKaynakTur.KREDIBUTCEDISI, ENUMButceKaynakTur.KREDIBUTCEICI };
            BilgiVAN projeler = servisYAT.ProjeAltProjeListele(kullanan, yil, senaryoKod, sektor, null, projeNo, -1, bkTur, false, yil - 100, true, kurKod, altProjeNo, true, "", false);


            if (!projeler.sonuc.islemSonuc)
                throw new RaporHatasi(projeler.sonuc.hataStr);
            //if (projeler.liste.Count <= 0)
            //    throw new RaporHatasi(projeler.sonuc.bilgiStr);

            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            string[] adres = { "Sektor", "Kurulus", "Ad", "Numara", "Yer", "ProjeTarih", "Karakteristik",
                                 "TutarDis", "TutarToplam", "KrediyiVeren", "KrediTarih", "Tur", "KrediTutar",
                                 "VadeOdemesiz", "Faiz", "IcPara", "SonYil", "AnaPara", "OdenecekFaiz",
                                 "SonDurum", "DarbogazCozum", "BinYTL" };
            string[] kriter = new string[adres.Length];
            int[] kriterSatir = new int[adres.Length];
            int[] kriterSutun = new int[adres.Length];
            for (int i = 0; i < adres.Length; i++)
                XLS.HucreAdAdresCoz(adres[i], ref kriterSatir[i], ref kriterSutun[i]);

            for (int i = 0; i < projeler.liste.Count - 1; i++)
                XLS.YeniSheetEkle(0);

            for (int k = 0; k < projeler.liste.Count; k++)
            {
                XLS.AktifSheetDegistir(k);
                Proje p = (Proje)projeler.liste[k];

                kriter[0] = p.sektor.kod + " - " + p.sektor.ad;
                kriter[1] = p.yurutecekKurumAd;
                kriter[2] = p.ad;
                kriter[3] = p.DPTProjeNo;
                kriter[4] = ""; // p.il.ad + " - " + p.ilce.ilceAdi;
                kriter[5] = p.baslangicYili + " / " + p.bitisYili;
                kriter[6] = p.karakteristikOzet;

                for (int i = 7; i < 22; i++)
                {
                    kriter[i] = "";
                }
                satir = kaynakSatir;
                foreach (AltProje item in p.altProjeler)
                {
                    foreach (ButceKaynak bk in item.butceKaynaklari)
                    {
                        if (bk.kaynakTur != ENUMButceKaynakTur.BUTCE)
                        {
                            KriterEkle(kriter, 9, bk.krediyiVeren);
                            KriterEkle(kriter, 10, bk.baslamaTarih + " / " + bk.yururlukTarih);
                            KriterEkle(kriter, 11, bk.tur);
                            KriterEkle(kriter, 12, bk.tutar);
                            KriterEkle(kriter, 13, bk.vade + " / " + bk.odemesizDonem);
                            KriterEkle(kriter, 14, bk.faizOran);
                            KriterEkle(kriter, 15, bk.IcParaKrediOran);

                            foreach (ButceKaynakGeriOdeme ko in bk.geriOdeme)
                            {
                                KriterEkle(kriter, 16, ko.yil.ToString());
                                KriterEkle(kriter, 17, (ko.anaParaGeriOdeme / Bolen).ToString("#,###"));
                                KriterEkle(kriter, 18, (ko.odenecekFaiz / Bolen).ToString("#,###"));
                                KriterEkle(kriter, 19, ko.projeninSonDurumu);
                                KriterEkle(kriter, 20, ko.darBogazCozum);
                                KriterEkle(kriter, 21, ko.yil.ToString().Trim() + " Yılı Fiyatlarıyla, Bin YTL");
                            }

                            for (int i = 0; i < bk.teklifler.Count; i++)
                            {
                                ButceKaynakTeklif t = (ButceKaynakTeklif)bk.teklifler[i];
                                KriterEkle(kriter, 7, (t.Odenek0Dis / Bolen).ToString("#,###"));
                                KriterEkle(kriter, 8, (t.Odenek0Toplam / Bolen).ToString("#,###"));

                                satir++;

                                XLS.SatirAc(satir, 1);
                                XLS.HucreKopyala(kaynakSatir, sutun - 3, kaynakSatir, sutun + 6, satir, sutun - 3);
                                XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 2);
                                XLS.HucreBirlestir(satir, sutun + 3, satir, sutun + 4);
                                XLS.HucreBirlestir(satir, sutun + 5, satir, sutun + 6);

                                XLS.HucreDegerYaz(satir, sutun, t.butceYil);
                                XLS.HucreDegerYaz(satir, sutun + 1, t.Odenek0Toplam / Bolen);

                                double harcama = t.HarcamaYilSonuToplam;
                                if (harcama <= 0)
                                    harcama = t.Harcama6ayToplam;
                                XLS.HucreDegerYaz(satir, sutun + 3, harcama / Bolen);
                                XLS.HucreDegerYaz(satir, sutun + 5, (t.Odenek0Toplam - harcama) / Bolen);
                            }
                        }
                    }
                }
                for (int j = 0; j < kriter.Length; j++)
                    XLS.HucreDegerYaz(kriterSatir[j], kriterSutun[j], kriter[j]);

                XLS.YatayCizgiCiz(satir + 2, sutun - 3, sutun - 1, OrtakClass.LineStyle.THIN, TabloRenk.BLACK, true);
                XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(360));
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void KriterEkle(string[] kriter, int indis, string deger)
        {
            if (!string.IsNullOrEmpty(kriter[indis])) kriter[indis] += ",";
            kriter[indis] += deger;
        }

        private string Tablo7Yaz(int yil, string senaryoKod)
        {
            BilgiVAN ktler = servisYAT.KrediTalepListele(0, kullanan, yil);
            if (!ktler.sonuc.islemSonuc)
                throw new RaporHatasi(ktler.sonuc.hataStr);
            //if (ktler.liste.Count <= 0)
            //    throw new RaporHatasi(ktler.sonuc.bilgiStr);

            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            string[] adres = { "Yil", "BinYTL", "Sektor", "Kurulus", "Ad", "Numara", "Yer", "ProjeTarih",
                                 "Karakteristik", "TutarDis", "TutarToplam", "KullanimAmaci", "Ihtiyac",
                                 "TeminEdilen", "TalepEdilen", "Alternatifler", "Dusunceler" };
            string[] kriter = new string[adres.Length];
            int[] kriterSatir = new int[adres.Length];
            int[] kriterSutun = new int[adres.Length];
            for (int i = 0; i < adres.Length; i++)
                XLS.HucreAdAdresCoz(adres[i], ref kriterSatir[i], ref kriterSutun[i]);

            for (int i = 0; i < ktler.liste.Count - 1; i++)
                XLS.YeniSheetEkle(0);

            for (int k = 0; k < ktler.liste.Count; k++)
            {
                KrediTalep kt = (KrediTalep)ktler.liste[k];
                BilgiVAN bilgi = servisYAT.Tablo7Listele(kullanan, yil, senaryoKod, kt.krediNo);
                if (!bilgi.sonuc.islemSonuc)
                    throw new RaporHatasi(bilgi.sonuc.hataStr);
                if (bilgi.liste.Count <= 0)
                    continue;

                XLS.AktifSheetDegistir(k);

                kriter[0] = yil.ToString();
                kriter[1] = yil.ToString().Trim() + " Yılı Fiyatlarıyla, Bin YTL";

                Proje p = (Proje)bilgi.liste[0];
                kriter[2] = p.sektor.kod + " - " + p.sektor.ad;
                kriter[3] = p.yurutecekKurumAd;
                kriter[4] = p.ad;
                kriter[5] = p.DPTProjeNo;
                kriter[6] = ""; // +p.il.ad + " - " + p.ilce.ilceAdi;
                kriter[7] = p.baslangicYili + " / " + p.bitisYili;
                kriter[8] = p.karakteristikOzet;

                if (p.altProjeler.Count <= 0)
                {
                    for (int j = 0; j < kriter.Length; j++)
                        XLS.HucreDegerYaz(kriterSatir[j], kriterSutun[j], kriter[j]);
                    continue;
                }

                if (((AltProje)p.altProjeler[0]).butceKaynaklari.Count > 0)
                {
                    ButceKaynakTeklif teklifBilgi = (ButceKaynakTeklif)((ButceKaynak)((AltProje)p.altProjeler[0]).butceKaynaklari[0]).teklifler[0];
                    kriter[9] = (teklifBilgi.Odenek0Dis / Bolen).ToString("#,###");
                    kriter[10] = (teklifBilgi.Odenek0Toplam / Bolen).ToString("#,###");
                }

                if (((AltProje)p.altProjeler[0]).krediTalepleri.Count > 0)
                {
                    kt = (KrediTalep)((AltProje)p.altProjeler[0]).krediTalepleri[0];
                    kriter[11] = kt.amac;
                    kriter[12] = (kt.krediIhtiyaci / Bolen).ToString("#,###");
                    kriter[13] = (kt.teminEdilen / Bolen).ToString("#,###");
                    kriter[14] = (kt.talepEdilen / Bolen).ToString("#,###");
                    kriter[15] = kt.alternatif;
                    kriter[16] = kt.dusunceler;
                }

                for (int j = 0; j < kriter.Length; j++)
                    XLS.HucreDegerYaz(kriterSatir[j], kriterSutun[j], kriter[j]);

                if (((AltProje)p.altProjeler[0]).butceKaynaklari.Count <= 1 ||
                    ((ButceKaynak)((AltProje)p.altProjeler[0]).butceKaynaklari[1]).teklifler.Count <= 0)
                {
                    continue;
                }

                ButceKaynak bk = (ButceKaynak)((AltProje)p.altProjeler[0]).butceKaynaklari[1];
                ButceKaynakTeklif teklif = (ButceKaynakTeklif)bk.teklifler[0];
                satir = kaynakSatir;
                XLS.HucreDegerYaz(satir, sutun + 0 * 2, teklif.ProjeTutarDis / Bolen);
                XLS.HucreDegerYaz(satir + 1, sutun + 0 * 2, teklif.ProjeTutarDis / Bolen);
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string Tablo10Yaz(int yil, string senaryoKod, string sektor, string senaryoAd, string kurKod)
        {
            BilgiVAN bilgi = servisYAT.Tablo10Listele(kullanan, yil, senaryoKod, sektor, kurKod);
            if (!bilgi.sonuc.islemSonuc)
                throw new RaporHatasi(bilgi.sonuc.hataStr);
            //if (bilgi.liste.Count <= 0)
            //    throw new RaporHatasi(bilgi.sonuc.bilgiStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int[] raporSatir = new int[3];

            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("DevamSatir", ref raporSatir[1], ref sutun);
            XLS.HucreAdAdresCoz("YeniSatir", ref raporSatir[2], ref sutun);
            XLS.HucreAdAdresCoz("KaynakSatir", ref raporSatir[0], ref sutun);

            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("Asama", senaryoKod + " - " + senaryoAd);
            XLS.HucreAdBulYaz("Sektor", sektor + " - " + servisUZY.UzayDegeriStr(kullanan, "YATSEKTOR", sektor, true));
            string kurAd = YatRaporAraclari.VarsayilanKurumBulAd();
            XLS.HucreAdBulYaz("Kurulus", kurAd);
            XLS.HucreAdBulYaz("KurulusT", kurAd + " TOPLAMI");

            double[,] bilgiToplam = new double[raporSatir.Length - 1, 5];

            for (int i = 0; i < bilgi.liste.Count; i++)
            {
                Proje p = (Proje)bilgi.liste[i];

                double[] projeToplam = new double[bilgiToplam.Length / (raporSatir.Length - 1)];

                if (p.baslangicYili < yil)
                {
                    for (int k = 1; k < raporSatir.Length; k++)
                        raporSatir[k]++;
                    satir = raporSatir[1];
                }
                else if (p.baslangicYili == yil)
                {
                    for (int k = 2; k < raporSatir.Length; k++)
                        raporSatir[k]++;
                    satir = raporSatir[2];
                }

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(raporSatir[0] - 1, sutun, raporSatir[0] - 1, sutun + 10, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 2);

                XLS.HucreDegerYaz(satir, sutun, p.DPTProjeNo);
                XLS.HucreDegerYaz(satir, sutun + 1, p.ad);
                XLS.HucreDegerYaz(satir, sutun + 3, ""); //p.il.ad + " - " + p.ilce.ilceAdi
                XLS.HucreDegerYaz(satir, sutun + 4, p.karakteristikOzet);
                XLS.HucreDegerYaz(satir, sutun + 5, p.baslangicYili + " / " + p.bitisYili);

                foreach (AltProje alt in p.altProjeler)
                {
                    if (p.altProjeler.Count > 1)
                    {
                        if (p.baslangicYili < yil)
                        {
                            for (int k = 1; k < raporSatir.Length; k++)
                                raporSatir[k]++;
                            satir = raporSatir[1];
                        }
                        else if (p.baslangicYili == yil)
                        {
                            for (int k = 2; k < raporSatir.Length; k++)
                                raporSatir[k]++;
                            satir = raporSatir[2];
                        }

                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(raporSatir[0], sutun, raporSatir[0], sutun + 10, satir, sutun);
                        XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 2);
                        XLS.YatayCizgiCiz(satir, sutun, sutun + 10, OrtakClass.LineStyle.NONE, TabloRenk.BLACK, false);
                        XLS.YatayCizgiCiz(satir, sutun, sutun + 10, OrtakClass.LineStyle.NONE, TabloRenk.BLACK, true);

                        XLS.HucreDegerYaz(satir, sutun + 1, alt.ad);
                        XLS.HucreDegerYaz(satir, sutun + 3, ""); //alt.il.ad + " - " + alt.ilce.ilceAdi
                        XLS.HucreDegerYaz(satir, sutun + 4, alt.karakteristikOzet);
                        XLS.HucreDegerYaz(satir, sutun + 5, alt.baslangicYil + " / " + alt.bitisYil);
                    }

                    if (alt.odenekBilgiler.Count > 0)
                    {
                        TNSCollection apRakam = ((TNS.BHMv2.OdenekBilgi)alt.odenekBilgiler[0]).ekRakamlar;
                        for (int j = 0; j < apRakam.Count; j++)
                        {
                            XLS.HucreDegerYaz(satir, sutun + 6 + j, (double)apRakam[j] / Bolen);
                            projeToplam[j] += (double)apRakam[j];
                        }
                    }
                }

                if (p.altProjeler.Count > 1)
                    for (int k = 0; k < projeToplam.Length; k++)
                        XLS.HucreDegerYaz(satir - p.altProjeler.Count, (sutun + 6 + k), projeToplam[k] / Bolen);

                for (int k = 0; k < 5; k++)
                {
                    int index = 0;
                    if (p.baslangicYili == yil)
                        index = 1;

                    bilgiToplam[index, k] += projeToplam[k];
                }
            }

            for (int j = 0; j < 2; j++)
            {
                int newSatir = raporSatir[0] + 2;
                if (j == 1)
                    newSatir = raporSatir[1] + 2;

                for (int k = 0; k < 5; k++)
                    XLS.HucreDegerYaz(newSatir, (sutun + 6 + k), bilgiToplam[j, k] / Bolen);
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string Tablo12_13Yaz(int yil, string sektor, int model, string senaryoKod, string kurKod)
        {
            BilgiVAN bilgi = servisYAT.Tablo1213Listele(kullanan, yil, sektor, model, senaryoKod, kurKod);
            if (!bilgi.sonuc.islemSonuc)
                throw new RaporHatasi(bilgi.sonuc.hataStr);
            //if (bilgi.liste.Count <= 0)
            //    throw new RaporHatasi(bilgi.sonuc.bilgiStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            string baslik = " MODELİ İLE YAPILMASI DÜŞÜNÜLEN PROJELER";
            string modelAd = string.Empty;
            if (model == (int)ENUMModel.YID)
            {
                modelAd = "YAP-İŞLET-DEVRET (YİD)";
                baslik = "Tablo-12: " + modelAd + baslik;
            }
            else if (model == (int)ENUMModel.YI)
            {
                modelAd = "YAP-İŞLET (Yİ)";
                baslik = "Tablo-13: " + modelAd + baslik + "(*)";
            }
            else
            {
                modelAd = "NORMAL";
                baslik = modelAd + baslik;
            }

            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("Model", modelAd);
            XLS.HucreAdBulYaz("Sektor", sektor + " - " + servisUZY.UzayDegeriStr(kullanan, "YATSEKTOR", sektor, true));
            XLS.HucreAdBulYaz("Baslik", baslik);

            satir = kaynakSatir;

            for (int i = 0; i < bilgi.liste.Count; i++)
            {
                Proje p = (Proje)bilgi.liste[i];

                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 5, satir, sutun);
                XLS.HucreBirlestir(satir, sutun, satir, sutun + 2);
                XLS.YatayCizgiCiz(satir, sutun, sutun + 5, OrtakClass.LineStyle.NONE, TabloRenk.BLACK, false);
                XLS.YatayCizgiCiz(satir, sutun, sutun + 5, OrtakClass.LineStyle.NONE, TabloRenk.BLACK, true);

                XLS.HucreDegerYaz(satir, sutun, p.ad);
                XLS.HucreDegerYaz(satir, sutun + 3, ""); //p.il.ad + " - " + p.ilce.ilceAdi
                XLS.HucreDegerYaz(satir, sutun + 4, p.karakteristikOzet);
                ButceKaynakTeklif t = (ButceKaynakTeklif)((ButceKaynak)((AltProje)p.altProjeler[0]).butceKaynaklari[0]).teklifler[0];
                XLS.HucreDegerYaz(satir, sutun + 5, t.Odenek0Toplam / Bolen);
            }

            XLS.YatayCizgiCiz(satir, sutun, sutun + 5, OrtakClass.LineStyle.THIN, TabloRenk.BLACK, false);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(360));
            if (model != (int)ENUMModel.YI)
                XLS.SatirGizle(satir + 2, satir + 2, true);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string TeklifDagilimYaz(int yil, string sektor, string senaryoKod, string kurKod)
        {
            BilgiVAN bilgi = servisYAT.TeklifDagilimListele(kullanan, yil, senaryoKod, sektor, kurKod);
            if (!bilgi.sonuc.islemSonuc)
                throw new RaporHatasi(bilgi.sonuc.hataStr);
            //if (bilgi.liste.Count <= 0)
            //    throw new RaporHatasi(bilgi.sonuc.bilgiStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("Sektor", sektor);
            XLS.HucreAdBulYaz("SektorAd", servisUZY.UzayDegeriStr(kullanan, "YATSEKTOR", sektor, true));

            satir = kaynakSatir + 2;

            for (int i = 0; i < bilgi.liste.Count; i++)
            {
                TeklifDagilim td = (TeklifDagilim)bilgi.liste[i];

                satir++;
                int birimSatir = satir;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun, td.birim.Ad());

                double[] birimToplam = new double[7];
                foreach (Proje proje in td.projeler)
                {
                    satir++;
                    int projeSatir = satir;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir + 1, sutun, kaynakSatir + 1, sutun + 7, satir, sutun);
                    XLS.YatayCizgiCiz(satir, sutun, sutun + 7, OrtakClass.LineStyle.NONE, TabloRenk.BLACK, false);
                    XLS.HucreDegerYaz(satir, sutun, proje.ad);

                    double[] projeToplam = new double[birimToplam.Length];
                    foreach (TNS.BHMv2.OdenekBilgi odn in proje.odenekBilgiler)
                    {
                        satir++;

                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir + 2, sutun, kaynakSatir + 2, sutun + 7, satir, sutun);
                        XLS.YatayCizgiCiz(satir, sutun, sutun + 7, OrtakClass.LineStyle.NONE, TabloRenk.BLACK, false);
                        XLS.YatayCizgiCiz(satir, sutun, sutun + 7, OrtakClass.LineStyle.NONE, TabloRenk.BLACK, true);

                        XLS.HucreDegerYaz(satir, sutun, odn.tertip.TertipStrOl());
                        for (int m = 0; m < odn.ekRakamlar.Count; m++)
                        {
                            XLS.HucreDegerYaz(satir, sutun + m + 1, (double)odn.ekRakamlar[m]);
                            projeToplam[m] += (double)odn.ekRakamlar[m];
                        }
                    }

                    for (int k = 0; k < projeToplam.Length; k++)
                    {
                        birimToplam[k] += projeToplam[k];
                        XLS.HucreDegerYaz(projeSatir, sutun + k + 1, projeToplam[k]);
                    }
                }
                for (int k = 0; k < birimToplam.Length; k++)
                    XLS.HucreDegerYaz(birimSatir, sutun + k + 1, birimToplam[k]);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 3, satir, GenelIslemler.JexcelBirimtoExcelBirim(360));

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string SektorDagilimYaz(int yil, string senaryoKod, string senaryoAd, string birimKod)
        {
            BilgiVAN bilgi = servisYAT.SektorDagilimListele(kullanan, yil, senaryoKod, birimKod);
            if (!bilgi.sonuc.islemSonuc)
                throw new RaporHatasi(bilgi.sonuc.hataStr);
            //if (bilgi.liste.Count <= 0)
            //    throw new RaporHatasi(bilgi.sonuc.bilgiStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.YatayCizgiCiz(kaynakSatir - 3, sutun + 8, sutun + 10, OrtakClass.LineStyle.MEDIUM, TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(kaynakSatir - 2, sutun + 3, sutun + 10, OrtakClass.LineStyle.MEDIUM, TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(kaynakSatir, sutun + 3, sutun + 10, OrtakClass.LineStyle.MEDIUM, TabloRenk.BLACK, true);

            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("Asama", senaryoKod + " - " + senaryoAd);
            string kurKod = string.Empty;
            string kurAd = string.Empty;
            YatRaporAraclari.VarsayilanKurumBul(out kurKod, out kurAd);
            XLS.HucreAdBulYaz("Kurulus", kurAd + " TOPLAMI");

            satir = kaynakSatir + 5;
            int toplamSutunSay = 7;

            double[] kurumToplam = new double[toplamSutunSay];

            TNSCollection sektorler = new TNSCollection();
            for (int i = 0; i < bilgi.liste.Count; i++)
            {
                TeklifDagilim td = (TeklifDagilim)bilgi.liste[i];

                satir++;

                XLS.SatirAc(satir, 1);
                if (i % 2 == 0)
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 10, satir, sutun);
                else
                    XLS.HucreKopyala(kaynakSatir + 2, sutun, kaynakSatir + 2, sutun + 10, satir, sutun);
                XLS.HucreBirlestir(satir, sutun, satir, sutun + 3);
                XLS.HucreDegerYaz(satir, sutun, td.birim.Ad());
                XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(600));
                XLS.YatayHizala(satir, sutun, 0);

                double[] birimToplam = new double[toplamSutunSay];
                foreach (Proje proje in td.projeler)
                {
                    satir++;

                    XLS.SatirAc(satir, 1);
                    if (i % 2 == 0)
                        XLS.HucreKopyala(kaynakSatir + 1, sutun, kaynakSatir + 1, sutun + 10, satir, sutun);
                    else
                        XLS.HucreKopyala(kaynakSatir + 3, sutun, kaynakSatir + 3, sutun + 10, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun + 3, proje.sektor.ad);
                    XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(360));

                    bool varMi = false;
                    foreach (SektorSozluk s in sektorler)
                        if (s.kod == proje.sektor.kod)
                        {
                            varMi = true;
                            break;
                        }
                    if (!varMi)
                        sektorler.Add(proje.sektor);

                    ButceKaynak bk = (ButceKaynak)((AltProje)proje.altProjeler[0]).butceKaynaklari[0];
                    double[] projeToplam = new double[kurumToplam.Length];
                    foreach (ButceKaynakTeklif bkh in bk.teklifler)
                    {
                        if (bkh.ButceYil == yil - 2)
                        {
                            projeToplam[0] += bkh.Odenek0Toplam;
                            projeToplam[1] += bkh.HarcamaYilSonuToplam;
                        }
                        else if (bkh.butceYil == yil - 1)
                        {
                            projeToplam[2] += bkh.Odenek0Toplam;
                            projeToplam[3] += bkh.Harcama6ayToplam;
                        }
                        else if (bkh.butceYil == yil)
                        {
                            projeToplam[4] += bkh.Odenek0Toplam;
                            projeToplam[5] += bkh.Odenek1Toplam;
                            projeToplam[6] += bkh.Odenek2Toplam;
                        }
                    }

                    for (int k = 0; k < projeToplam.Length; k++)
                    {
                        birimToplam[k] += projeToplam[k];
                        XLS.HucreDegerYaz(satir, sutun + 4 + k, projeToplam[k] / Bolen);
                    }
                }

                satir++;

                XLS.SatirAc(satir, 1);
                if (i % 2 == 0)
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 10, satir, sutun);
                else
                    XLS.HucreKopyala(kaynakSatir + 2, sutun, kaynakSatir + 2, sutun + 10, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 3, "Toplam");
                XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(360));
                XLS.YatayCizgiCiz(satir, sutun + 3, sutun + 10, OrtakClass.LineStyle.THIN, TabloRenk.BLACK, true);
                XLS.YatayCizgiCiz(satir, sutun + 3, sutun + 10, OrtakClass.LineStyle.MEDIUM, TabloRenk.BLACK, false);

                for (int k = 0; k < birimToplam.Length; k++)
                    XLS.HucreDegerYaz(satir, sutun + k + 4, birimToplam[k] / Bolen);
            }

            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir + 4, sutun, kaynakSatir + 4, sutun + 10, satir, sutun);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 3);
            XLS.HucreDegerYaz(satir, sutun, kurAd + " TOPLAMI");
            XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(600));
            XLS.YatayHizala(satir, sutun, 0);

            foreach (SektorSozluk s in sektorler)
            {
                int sonSatir = satir;
                double[] sektorToplam = new double[kurumToplam.Length];
                for (int j = kaynakSatir; j <= sonSatir; j++)
                {
                    string sektorAd = XLS.HucreDegerAl(j, sutun + 3);
                    if (sektorAd == s.ad)
                        for (int k = 0; k < sektorToplam.Length; k++)
                            sektorToplam[k] += OrtakFonksiyonlar.ConvertToDouble(XLS.HucreDegerAl(j, sutun + k + 4), 0);
                }
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir + 5, sutun, kaynakSatir + 5, sutun + 10, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 3, s.ad);
                XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(360));
                for (int k = 0; k < sektorToplam.Length; k++)
                {
                    kurumToplam[k] += sektorToplam[k];
                    XLS.HucreDegerYaz(satir, sutun + k + 4, sektorToplam[k]);
                }
            }

            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir + 4, sutun, kaynakSatir + 4, sutun + 10, satir, sutun);
            XLS.HucreDegerYaz(satir, sutun + 3, "Toplam");
            XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(360));
            XLS.YatayCizgiCiz(satir, sutun + 3, sutun + 10, OrtakClass.LineStyle.THIN, TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(satir, sutun + 3, sutun + 10, OrtakClass.LineStyle.MEDIUM, TabloRenk.BLACK, false);

            for (int k = 0; k < kurumToplam.Length; k++)
                XLS.HucreDegerYaz(satir, sutun + k + 4, kurumToplam[k]);

            satir++;

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir + 4, sutun, kaynakSatir + 4, sutun + 10, satir, sutun);
            XLS.HucreDegerYaz(satir, sutun + 3, "Tavan");
            XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(360));
            XLS.YatayCizgiCiz(satir, sutun + 3, sutun + 10, OrtakClass.LineStyle.THIN, TabloRenk.BLACK, true);
            XLS.YatayCizgiCiz(satir, sutun, sutun + 10, OrtakClass.LineStyle.MEDIUM, TabloRenk.BLACK, false);

            int tavanSutun = 0;
            for (int yilTavan = yil - 2; yilTavan <= yil + 2; yilTavan++)
            {
                int asamaTavan = 1;
                if (yilTavan == yil + 1)
                    asamaTavan = 11;
                else if (yilTavan == yil + 2)
                    asamaTavan = 21;

                double tavan = 0;
                if (yilTavan == yil + 1 || yilTavan == yil + 2)
                    tavan = servisUZY.UzayDegeriDbl(kullanan, "ABSTAVAN", yil.ToString() + "-" + "EKONOMİK" + "-" + kurKod + "-" + "06" + "-" + asamaTavan.ToString(), true);
                else
                    tavan = servisUZY.UzayDegeriDbl(kullanan, "ABSTAVAN", yilTavan.ToString() + "-" + "EKONOMİK" + "-" + kurKod + "-" + "06" + "-" + asamaTavan.ToString(), true);
                XLS.HucreDegerYaz(satir, sutun + tavanSutun + 4, tavan / Bolen);

                if (yilTavan < yil)
                {
                    tavanSutun++;
                    XLS.HucreDegerYaz(satir, sutun + tavanSutun + 4, (double)0);
                }
                tavanSutun++;
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string BirimProjeleriYaz(int yil, string senaryoKod, string birimKodu)
        {
            BilgiVAN bilgi = servisYAT.BirimProjeleriListele(kullanan, yil, senaryoKod, birimKodu, false);
            if (!bilgi.sonuc.islemSonuc)
                throw new RaporHatasi(bilgi.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("Birim", birimKodu);
            XLS.HucreAdBulYaz("BirimAdi", servisUZY.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + birimKodu, true));

            XLS.HucreAdBulYaz("YilEksi2KBO", servisUZY.UzayDegeriDbl(kullanan, "ABSODENEK", (yil - 2).ToString() + "-06.00-0-" + birimKodu, false));
            XLS.HucreAdBulYaz("YilEksi2Harcama", servisUZY.UzayDegeriDbl(kullanan, "ABSHAZMUHASEBAT", (yil - 2).ToString() + "-12-" + birimKodu, false));
            XLS.HucreAdBulYaz("YilEksi1KBO", servisUZY.UzayDegeriDbl(kullanan, "ABSODENEK", (yil - 1).ToString() + "-06.00-0-" + birimKodu, false));
            XLS.HucreAdBulYaz("YilEksi1Harcama", servisUZY.UzayDegeriDbl(kullanan, "ABSHAZMUHASEBAT", (yil - 1).ToString() + "-7-" + birimKodu, false));
            XLS.HucreAdBulYaz("YilTeklif", servisUZY.UzayDegeriDbl(kullanan, "ABSODENEK", yil.ToString() + "-00.00-0-" + birimKodu, false));
            XLS.HucreAdBulYaz("YilUygun", servisUZY.UzayDegeriDbl(kullanan, "ABSODENEK", yil.ToString() + "-" + senaryoKod + "-0-" + birimKodu, false));

            satir = kaynakSatir + 5;
            int yuks = XLS.SatirYukseklikAl(satir + 4);

            double[] toplam = new double[4];
            for (int i = 0; i < bilgi.liste.Count; i++)
            {
                Proje proje = (Proje)bilgi.liste[i];

                XLS.HucreKopyala(kaynakSatir, sutun - 1, kaynakSatir + 1, sutun + 10, satir, sutun - 1);

                XLS.HucreBirlestir(satir, sutun, satir, sutun + 3);
                XLS.HucreBirlestir(satir, sutun + 4, satir, sutun + 5);

                XLS.HucreBirlestir(satir + 1, sutun, satir + 1, sutun + 3);
                XLS.HucreBirlestir(satir + 1, sutun + 4, satir + 1, sutun + 5);

                XLS.HucreDegerYaz(satir, sutun, proje.ad);
                XLS.HucreDegerYaz(satir + 1, sutun, proje.gerekce);

                XLS.SatirYukseklikAyarla(satir, satir, yuks);
                XLS.SatirYukseklikAyarla(satir + 1, satir + 1, yuks * 2);

                ButceKaynak bk = (ButceKaynak)((AltProje)proje.altProjeler[0]).butceKaynaklari[0];
                foreach (ButceKaynakTeklif bkt in bk.teklifler)
                {
                    if (bkt.butceYil == yil - 1)
                    {
                        toplam[0] += bkt.Odenek0Toplam;
                        XLS.HucreDegerYaz(satir + 1, sutun + 6, bkt.Odenek0Toplam);
                    }
                    else if (bkt.butceYil == yil)
                    {
                        toplam[1] += bkt.Odenek0Toplam;
                        XLS.HucreDegerYaz(satir + 1, sutun + 7, bkt.Odenek0Toplam);
                        toplam[2] += bkt.Odenek1Toplam;
                        XLS.HucreDegerYaz(satir + 1, sutun + 8, bkt.Odenek1Toplam);
                        toplam[3] += bkt.Odenek2Toplam;
                        XLS.HucreDegerYaz(satir + 1, sutun + 9, bkt.Odenek2Toplam);
                    }
                }
                satir += 3;
            }

            for (int i = 0; i < toplam.Length; i++)
                XLS.HucreDegerYaz(kaynakSatir + 4, sutun + 6 + i, toplam[i]);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string KarakterDagilimYaz(int yil, string senaryoKod, string sektor, string kurKod)
        {
            System.Collections.Hashtable ht = new System.Collections.Hashtable();
            ht.Add("Yil", yil.ToString());
            ht.Add("Senaryo", senaryoKod);
            ht.Add("Sektor", sektor);
            ht.Add("KurKod", kurKod);
            ht.Add("SektorAd", servisUZY.UzayDegeriStr(kullanan, "YATSEKTOR", sektor, true));

            sonucDosyaAd = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(sonucDosyaAd), System.IO.Path.GetFileNameWithoutExtension(sonucDosyaAd));
            return OrtakClass.ExcelRapor.ExcelRaporHesapla(kullanan, servisUZY, sablonAd, sonucDosyaAd, GenelIslemler.ExcelTur(), ht);
        }

        private string KonsolideYatirimYaz(int yil, string birimKodu, string senaryoKod)
        {
            BilgiVAN bilgi = servisYAT.BirimProjeleriListele(kullanan, yil - 1, senaryoKod, birimKodu, true);
            if (!bilgi.sonuc.islemSonuc)
                throw new RaporHatasi(bilgi.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.HucreAdBulYaz("Yil", yil - 1);

            satir = kaynakSatir;
            int fark = 0;
            int yuks = XLS.SatirYukseklikAl(kaynakSatir);
            for (int i = 0; i < bilgi.liste.Count; i++)
            {
                Proje p = (Proje)bilgi.liste[i];
                Double[] projeToplam = new Double[6];

                XLS.HucreKopyala(kaynakSatir - 2, sutun, kaynakSatir - 2, sutun + 12, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, p.DPTProjeNo);
                XLS.HucreDegerYaz(satir, sutun + 1, p.ad);
                //XLS.HucreDegerYaz(satir, sutun + 2, p.il.ad + " - " + p.ilce.ilceAdi);
                XLS.HucreDegerYaz(satir, sutun + 3, p.karakteristik);
                //XLS.HucreDegerYaz(satir, sutun + 4, p.baslangicYili + " / " + p.bitisYili);
                int projeSatir = satir;
                satir++;

                foreach (AltProje alt in p.altProjeler)
                {
                    //XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir - 1, sutun, kaynakSatir - 1, sutun + 12, satir, sutun);
                    //XLS.YatayCizgiCiz(satir, sutun, sutun + 12, OrtakClass.LineStyle.NONE, TabloRenk.BLACK, false);
                    //XLS.YatayCizgiCiz(satir, sutun, sutun + 12, OrtakClass.LineStyle.NONE, TabloRenk.BLACK, true);

                    //if (satir % 2 == 0)
                    //    for (int j = sutun; j <= sutun + 12; j++)
                    //        XLS.ArkaPlanRenk(satir, j, TabloRenk.IVORY);

                    XLS.HucreDegerYaz(satir, sutun + 1, alt.ad);
                    //XLS.HucreDegerYaz(satir, sutun + 2, alt.il.ad + " - " + alt.ilce.ilceAdi);
                    //XLS.HucreDegerYaz(satir, sutun + 3, alt.karakteristikOzet);
                    //XLS.HucreDegerYaz(satir, sutun + 4, alt.baslangicYil + " / " + alt.bitisYil);

                    double[] apToplam = new double[projeToplam.Length];
                    foreach (ButceKaynak bk in alt.butceKaynaklari)
                    {
                        foreach (ButceKaynakTeklif bkh in bk.teklifler)
                        {
                            if (yil - 2 == bkh.butceYil)
                            {
                                apToplam[1] += bkh.HarcamaYilSonuToplam;
                            }
                            else if (yil - 1 == bkh.butceYil)
                            {
                                apToplam[0] += bkh.ProjeTutarToplam;
                                apToplam[2] += bkh.Odenek0Toplam;
                                apToplam[3] += bkh.EklenenYilSonuToplam;
                                apToplam[4] += (0 - bkh.DusulenYilSonuToplam);
                                apToplam[5] += bkh.Harcama6ayToplam;
                            }
                        }
                    }
                    fark = 0;
                    for (int k = 0; k < apToplam.Length; k++)
                    {
                        projeToplam[k] += apToplam[k];
                        if (k > 4) fark = 1;
                        XLS.HucreDegerYaz(satir, sutun + 5 + k + fark, apToplam[k]);
                    }
                    satir++;
                }

                for (int k = 0; k < projeToplam.Length; k++)
                {
                    fark = 0;
                    if (k > 4) fark = 1;
                    XLS.HucreDegerYaz(projeSatir, sutun + 5 + k + fark, projeToplam[k]);
                }
            }

            XLS.SatirYukseklikAyarla(kaynakSatir, satir, yuks);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string ProjelerArasiOdenekAktarmasiYaz(int yil, string sektorNo, string birimKod, string senaryoKod)
        {
            BilgiVAN bilgi = servisYAT.ProjeAltProjeListele(kullanan, yil, senaryoKod, null, null, projeNo, 0, null, false, yil - 2, true, birimKod, altProjeNo, true, "", false);
            if (!bilgi.sonuc.islemSonuc)
                throw new RaporHatasi(bilgi.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("Sektor", sektorNo + " - " + servisUZY.UzayDegeriStr(kullanan, "YATSEKTOR", sektorNo, true));
            XLS.HucreAdBulYaz("Kurum", YatRaporAraclari.VarsayilanKurumBulAd());
            int geciciSatir = 0;
            int geciciSutun = 0;
            XLS.HucreAdAdresCoz("YurutecekKurulus", ref geciciSatir, ref geciciSutun);
            XLS.SatirGizle(geciciSatir, geciciSatir, true);

            satir = kaynakSatir + 1;
            for (int i = 0; i < bilgi.liste.Count; i++)
            {
                Proje p = (Proje)bilgi.liste[i];

                satir++;
                Double[] projeToplam = new Double[7];

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 10, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, p.DPTProjeNo);
                XLS.HucreDegerYaz(satir, sutun + 1, p.ad);
                XLS.HucreDegerYaz(satir, sutun + 2, p.baslangicYili + " / " + p.bitisYili);
                XLS.HucreDegerYaz(satir, sutun + 8, p.bitisYili);

                foreach (AltProje alt in p.altProjeler)
                {
                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir + 1, sutun, kaynakSatir + 1, sutun + 10, satir, sutun);

                    XLS.HucreDegerYaz(satir, sutun + 1, alt.ad);
                    XLS.HucreDegerYaz(satir, sutun + 2, alt.baslangicYil + " / " + alt.bitisYil);
                    XLS.HucreDegerYaz(satir, sutun + 8, alt.bitisYil);

                    Double[] apToplam = new Double[projeToplam.Length];
                    foreach (ButceKaynak bk in alt.butceKaynaklari)
                    {
                        foreach (ButceKaynakTeklif bkt in bk.teklifler)
                        {
                            if (bkt.butceYil == yil - 1)
                            {
                                apToplam[1] += bkt.HarcamaYilSonuToplam;
                            }
                            else if (bkt.butceYil == yil)
                            {
                                apToplam[0] += bkt.ProjeTutarToplam;
                                apToplam[2] += bkt.Odenek0Toplam;
                                apToplam[3] += bkt.EklenenYilSonuToplam;
                                apToplam[4] += (0 - bkt.DusulenYilSonuToplam);
                                apToplam[5] += bkt.ProjeTutarToplam + bkt.EklenenYilSonuToplam + bkt.DusulenYilSonuToplam;
                                apToplam[6] += bkt.Odenek0Toplam + bkt.EklenenYilSonuToplam + bkt.DusulenYilSonuToplam;
                            }
                        }
                    }
                    int ekSutun = 0;
                    for (int k = 0; k < apToplam.Length; k++)
                    {
                        if (k >= 5)
                            ekSutun = 1;
                        XLS.HucreDegerYaz(satir, sutun + 3 + k + ekSutun, apToplam[k] / Bolen);
                        projeToplam[k] += apToplam[k];
                    }
                }

                for (int k = 0; k < projeToplam.Length; k++)
                {
                    if (k >= 5)
                        XLS.HucreDegerYaz(satir - p.altProjeler.Count, (sutun + 4 + k), projeToplam[k] / Bolen);
                    else
                        XLS.HucreDegerYaz(satir - p.altProjeler.Count, (sutun + 3 + k), projeToplam[k] / Bolen);
                }
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string Tablo2OzelYaz(int yil, string senaryoKod, string sektor, string senaryoAd, string kurKod)
        {
            BilgiVAN bilgi = servisYAT.Tablo2OzelListele(kullanan, yil, senaryoKod, sektor, 0, kurKod);
            if (!bilgi.sonuc.islemSonuc)
                throw new RaporHatasi(bilgi.sonuc.hataStr);
            //if (bilgi.liste.Count <= 0)
            //    throw new RaporHatasi(bilgi.sonuc.bilgiStr);

            TNSCollection projeler = Tablo2OzelProjeSirala(yil, bilgi.liste);

            Tablo XLS = GenelIslemler.NewTablo();
            int kaynakSatir = 0;
            int sutun = 0;
            int satir = 0;

            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("KaynakSatir", ref kaynakSatir, ref sutun);
            satir = kaynakSatir;

            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("Asama", senaryoKod + " - " + senaryoAd);
            XLS.HucreAdBulYaz("Sektor", sektor + " - " + servisUZY.UzayDegeriStr(kullanan, "YATSEKTOR", sektor, true));
            XLS.HucreAdBulYaz("Kurulus", YatRaporAraclari.VarsayilanKurumBulAd());

            foreach (Proje p in projeler)
            {
                satir += 2;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 6, satir, sutun);

                XLS.ArkaPlanRenk(satir, sutun, satir, sutun + 6, TabloRenk.GRAY_25);
                XLS.KoyuYap(satir, sutun, satir, sutun + 6, true);

                XLS.HucreDegerYaz(satir, sutun, p.DPTProjeNo);
                XLS.HucreDegerYaz(satir, sutun + 1, p.ad);
                XLS.HucreDegerYaz(satir, sutun + 2, ""); //p.il.ad + " - " + p.ilce.ilceAdi
                XLS.HucreDegerYaz(satir, sutun + 3, p.baslangicYili + "-" + p.bitisYili);

                int[] toplamSatir = new int[3];
                double[,] toplam = new double[toplamSatir.Length, 3];
                for (int i = 0; i < toplamSatir.Length; i++)
                {
                    for (int j = 0; j < toplam.Length / toplamSatir.Length; j++)
                        toplam[i, j] = new double();
                    toplamSatir[i] = new int();
                }

                bool bitenIlk = true;
                bool devamIlk = true;
                bool yeniIlk = true;
                int siraNo = 0;
                foreach (AltProje altP in p.altProjeler)
                {
                    if ((altP.baslangicYil != yil && altP.bitisYil == yil && bitenIlk)
                        || (altP.baslangicYil != yil && altP.bitisYil > yil && devamIlk)
                        || (altP.baslangicYil == yil && yeniIlk))
                    {
                        siraNo = 0;
                        string aciklama = yil.ToString();
                        int index = 0;
                        if (altP.baslangicYil == yil)
                        {
                            yeniIlk = false;
                            aciklama += " Yılında Yatırım Programına Teklif Edilen Yeni Projeler";
                            index = 2;
                        }
                        else if (altP.bitisYil == yil)
                        {
                            bitenIlk = false;
                            aciklama += " Yılında Bitecek Projeler";
                            index = 0;
                        }
                        else if (altP.bitisYil > yil)
                        {
                            devamIlk = false;
                            aciklama += " Yılından Sonraya Kalan Projeler";
                            index = 1;
                        }

                        satir += 2;

                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 6, satir, sutun);
                        XLS.HucreBirlestir(satir, sutun, satir, sutun + 3);

                        XLS.KoyuYap(satir, sutun, satir, sutun + 6, true);
                        XLS.ArkaPlanRenk(satir, sutun, satir, sutun + 6, TabloRenk.RED);

                        XLS.HucreDegerYaz(satir, sutun, aciklama);

                        toplamSatir[index] = satir;
                    }

                    satir++;
                    siraNo++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 6, satir, sutun);

                    if (siraNo % 2 == 1)
                        XLS.ArkaPlanRenk(satir, sutun, satir, sutun + 6, TabloRenk.VERY_LIGHT_YELLOW);

                    XLS.HucreDegerYaz(satir, sutun, siraNo);
                    XLS.HucreDegerYaz(satir, sutun + 1, altP.ad);
                    XLS.HucreDegerYaz(satir, sutun + 2, ""); //altP.il.ad + " - " + altP.ilce.ilceAdi
                    XLS.HucreDegerYaz(satir, sutun + 3, altP.baslangicYil + "-" + altP.bitisYil);

                    double[] apToplam = new double[toplam.Length / toplamSatir.Length];
                    foreach (ButceKaynak bk in altP.butceKaynaklari)
                    {
                        foreach (ButceKaynakTeklif teklif in bk.teklifler)
                        {
                            if (teklif.butceYil == yil)
                            {
                                apToplam[0] += teklif.ProjeTutarToplam;
                                apToplam[2] += teklif.Odenek0Toplam;
                            }
                            else if (yil - 1 == teklif.butceYil)
                                apToplam[1] += teklif.HarcamaYilSonuToplam;
                        }
                    }
                    for (int m = 0; m < apToplam.Length; m++)
                    {
                        XLS.HucreDegerYaz(satir, sutun + 4 + m, apToplam[m] / Bolen);

                        if (altP.baslangicYil == yil)
                            toplam[2, m] += apToplam[m];
                        else if (altP.bitisYil == yil)
                            toplam[0, m] += apToplam[m];
                        else if (altP.bitisYil > yil)
                            toplam[1, m] += apToplam[m];
                    }
                }

                int projeSatir = 0;
                if (toplamSatir[0] != 0)
                    projeSatir = toplamSatir[0] - 2;
                else if (toplamSatir[1] != 0)
                    projeSatir = toplamSatir[1] - 2;
                else if (toplamSatir[2] != 0)
                    projeSatir = toplamSatir[2] - 2;
                for (int i = 0; i < toplamSatir.Length; i++)
                {
                    for (int j = 0; j < toplam.Length / toplamSatir.Length; j++)
                    {
                        XLS.HucreDegerYaz(toplamSatir[i], sutun + 4 + j, toplam[i, j] / Bolen);
                        if (i == 0)
                            XLS.HucreDegerYaz(projeSatir, sutun + 4 + j, (toplam[0, j] + toplam[1, j] + toplam[2, j]) / Bolen);
                    }
                }
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(360));

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private TNSCollection Tablo2OzelProjeSirala(int yil, TNSCollection projeler)
        {
            foreach (Proje p in projeler)
            {
                TNSCollection[] altProjeler = new TNSCollection[3];
                for (int i = 0; i < altProjeler.Length; i++)
                    altProjeler[i] = new TNSCollection();

                foreach (AltProje alt in p.altProjeler)
                {
                    if (alt.baslangicYil == yil)
                        altProjeler[2].Add(alt);
                    else if (alt.bitisYil == yil)
                        altProjeler[0].Add(alt);
                    else if (alt.bitisYil > yil)
                        altProjeler[1].Add(alt);
                }

                p.altProjeler.Clear();
                for (int i = 0; i < altProjeler.Length; i++)
                    if (altProjeler[i] != null)
                        if (altProjeler[i].Count > 0)
                            p.altProjeler.AddRange(altProjeler[i]);
            }
            return projeler;
        }

        private string TertipDagilimYaz(int yil, string senaryoKod, string sektor, string senaryoAd, string birimKod)
        {
            BilgiVAN bilgi = servisYAT.TertipDagilimListele(kullanan, yil, senaryoKod, sektor, birimKod);
            if (!bilgi.sonuc.islemSonuc)
                throw new RaporHatasi(bilgi.sonuc.hataStr);
            //if (bilgi.liste.Count <= 0)
            //    throw new RaporHatasi(bilgi.sonuc.bilgiStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("Asama", senaryoAd);
            XLS.HucreAdBulYaz("Kurum", YatRaporAraclari.VarsayilanKurumBulAd());

            satir = kaynakSatir + 1;

            double genelToplam = 0;
            for (int i = 0; i < bilgi.liste.Count; i++)
            {
                Proje p = (Proje)bilgi.liste[i];

                satir++;
                int projeSatir = satir;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 4, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, p.DPTProjeNo);
                XLS.HucreDegerYaz(satir, sutun + 1, p.sektor.ad);

                double projeToplam = 0;
                foreach (TNS.BHMv2.OdenekBilgi odn in p.odenekBilgiler)
                {
                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir + 1, sutun, kaynakSatir + 1, sutun + 4, satir, sutun);
                    XLS.YatayCizgiCiz(satir, sutun, sutun + 4, OrtakClass.LineStyle.NONE, TabloRenk.BLACK, false);
                    XLS.YatayCizgiCiz(satir, sutun, sutun + 4, OrtakClass.LineStyle.NONE, TabloRenk.BLACK, true);

                    XLS.HucreDegerYaz(satir, sutun + 2, odn.tertip.TertipStrOl());
                    XLS.HucreDegerYaz(satir, sutun + 3, (double)odn.ekRakamlar[0] / Bolen);
                    projeToplam += (double)odn.ekRakamlar[0];
                }

                XLS.HucreDegerYaz(projeSatir, sutun + 4, projeToplam / Bolen);
                genelToplam += projeToplam;
            }

            XLS.HucreDegerYaz(satir + 1, sutun + 4, genelToplam / Bolen);
            XLS.YatayCizgiCiz(satir, sutun, sutun + 4, OrtakClass.LineStyle.THIN, TabloRenk.BLACK, false);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string ProjeTakipYaz(int yil, string birimKod, string senaryoKod)
        {
            BilgiVAN bilgi = servisYAT.ProjeAltProjeListele(kullanan, yil, senaryoKod, null, null, projeNo, 0, null, false, yil - 2, true, birimKod, altProjeNo, true, "", false);
            if (!bilgi.sonuc.islemSonuc)
                throw new RaporHatasi(bilgi.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            string[] adres = { "ProjeAd", "ProjeNo", "AltProjeAd", "AltProjeNo", "Karakteristik", "BasBitYil" };
            int[] kriterSatir = new int[adres.Length];
            int[] kriterSutun = new int[adres.Length];
            for (int i = 0; i < adres.Length; i++)
                XLS.HucreAdAdresCoz(adres[i], ref kriterSatir[i], ref kriterSutun[i]);

            for (int i = 0; i < bilgi.liste.Count; i++)
                for (int j = 0; j < ((Proje)bilgi.liste[i]).altProjeler.Count; j++)
                    if (!(i == 0 && j == 0))
                        XLS.YeniSheetEkle(0);

            int aktifSayfa = 0;
            for (int i = 0; i < bilgi.liste.Count; i++)
            {
                Proje p = (Proje)bilgi.liste[i];

                for (int j = 0; j < p.altProjeler.Count; j++)
                {
                    XLS.AktifSheetDegistir(aktifSayfa);
                    aktifSayfa++;

                    AltProje alt = (AltProje)p.altProjeler[j];
                    string[] kriter = { p.ad, p.DPTProjeNo, alt.ad, alt.kod.ToString(), alt.karakteristikOzet, alt.baslangicYil + " / " + alt.bitisYil };
                    for (int m = 0; m < kriter.Length; m++)
                        XLS.HucreDegerYaz(kriterSatir[m], kriterSutun[m], kriter[m]);

                    satir = kaynakSatir;
                    for (int k = alt.baslangicYil; k <= alt.bitisYil; k++)
                    {
                        satir += 5;
                        XLS.SatirAc(satir, 4);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir + 3, sutun + 9, satir, sutun);

                        XLS.HucreBirlestir(satir, sutun, satir, sutun + 9);
                        XLS.HucreBirlestir(satir + 1, sutun, satir + 1, sutun + 1);
                        XLS.HucreBirlestir(satir + 2, sutun, satir + 2, sutun + 1);
                        XLS.HucreBirlestir(satir + 3, sutun, satir + 3, sutun + 9);

                        XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
                        XLS.SatirYukseklikAyarla(satir + 1, satir + 1, GenelIslemler.JexcelBirimtoExcelBirim(600));
                        XLS.SatirYukseklikAyarla(satir + 2, satir + 2, GenelIslemler.JexcelBirimtoExcelBirim(400));
                        XLS.SatirYukseklikAyarla(satir + 3, satir + 3, GenelIslemler.JexcelBirimtoExcelBirim(600));
                        XLS.HucreDegerYaz(satir, sutun, k.ToString() + " YILI");
                        XLS.HucreDegerYaz(satir + 1, sutun + 4, (k - 1).ToString() + " TOPLAM HARCAMA");
                        XLS.HucreDegerYaz(satir + 1, sutun + 9, k.ToString() + " YIL İÇİ HARCAMA");
                        XLS.HucreDegerYaz(satir + 2, sutun, alt.ad);
                        XLS.HucreDegerYaz(satir + 2, sutun + 3, alt.baslangicYil + " / " + alt.bitisYil);
                        //XLS.HucreDegerYaz(satir + 3, sutun, altYil.sonDurum);

                        Double[] apToplam = new Double[7];
                        foreach (ButceKaynak bk in alt.butceKaynaklari)
                        {
                            foreach (ButceKaynakTeklif bkt in bk.teklifler)
                            {
                                if (k - 1 == bkt.butceYil)
                                {
                                    apToplam[1] += bkt.HarcamaYilSonuToplam;
                                }
                                else if (k == bkt.butceYil)
                                {
                                    apToplam[0] += bkt.ProjeTutarToplam;
                                    apToplam[2] += bkt.Odenek0Toplam;
                                    apToplam[3] += bkt.EklenenYilSonuToplam;
                                    apToplam[4] += bkt.DusulenYilSonuToplam;
                                    apToplam[5] += bkt.Odenek0Toplam + bkt.EklenenYilSonuToplam + bkt.DusulenYilSonuToplam;
                                    apToplam[6] += bkt.Harcama6ayToplam;
                                }
                            }
                        }
                        int ekSutun;
                        for (int l = 0; l < apToplam.Length; l++)
                        {
                            ekSutun = 1;
                            if (l == 0)
                                ekSutun = 0;
                            XLS.HucreDegerYaz(satir + 2, sutun + 2 + l + ekSutun, apToplam[l]);
                        }
                    }
                }
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string DPTVizeCetveliYaz(int yil, string senaryoKod, Tertip tertip)
        {
            BilgiVAN bilgi = servisYAT.DPTVizeCetveliRaporla(kullanan, yil, senaryoKod, tertip);
            if (!bilgi.sonuc.islemSonuc)
                throw new RaporHatasi(bilgi.sonuc.hataStr);
            //if (bilgi.liste.Count <= 0)
            //    throw new RaporHatasi(bilgi.sonuc.bilgiStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            XLS.HucreAdBulYaz("Yil", yil);

            satir = kaynakSatir;

            for (int i = 0; i < bilgi.liste.Count; i++)
            {
                satir++;

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 5, satir, sutun);

                DPTVizeBilgi dptVize = (DPTVizeBilgi)bilgi.liste[i];
                XLS.HucreDegerYaz(satir, sutun, dptVize.tertip.TertipStrOl());
                XLS.HucreDegerYaz(satir, sutun + 1, dptVize.sektor.kod + " - " + dptVize.sektor.ad);
                for (int j = 0; j < dptVize.odenek.Length; j++)
                    XLS.HucreDegerYaz(satir, sutun + 2 + j, dptVize.odenek[j]);
                XLS.HucreDegerYaz(satir, sutun + 5, dptVize.tertip.adlar[3]);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string VizeCetveliYaz(int yil, string senaryoKod, string sektor, string senaryoAd, string birimKod, string birimAd)
        {
            BilgiVAN bilgi = servisYAT.VizeCetveliRaporla(kullanan, yil, senaryoKod, sektor, birimKod);
            if (!bilgi.sonuc.islemSonuc)
                throw new RaporHatasi(bilgi.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            int yukseklik = XLS.SatirYukseklikAl(kaynakSatir);
            XLS.HucreAdBulYaz("Yil", yil);

            satir = kaynakSatir;

            //0=Kurum
            //1=Sektör
            //2=Proje
            //3=Alt proje
            //4=PebKod
            //5=EkoKod
            ToplamAl toplamAl = new ToplamAl(6, 6);
            string[] groupKod = { "", "", "", "", "" };

            int[] groupSatir = { 0, 0, 0, 0, 0, 0 };

            XLS.HucreKopyala(kaynakSatir - 6, sutun, kaynakSatir - 6, sutun + 19, satir, sutun);
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 5);
            XLS.HucreDegerYaz(satir, sutun, birimKod);
            XLS.HucreDegerYaz(satir, sutun + 6, birimAd);
            groupSatir[0] = satir;

            int altProjeSay = 0;

            for (int i = 0; i < bilgi.liste.Count; i++)
            {
                object[] satirNesneler = (object[])bilgi.liste[i];

                string SEKTORNO = (string)satirNesneler[0];
                string SEKTORAD = (string)satirNesneler[1];

                int PROJENO = (int)satirNesneler[2];
                string PROJENOSTR = PROJENO.ToString();
                string DPTPROJENO = (string)satirNesneler[3];
                string PROJEAD = (string)satirNesneler[4];

                int ALTPROJENO = (int)satirNesneler[5];
                string ALTPROJENOSTR = ALTPROJENO.ToString();
                string BIRIM = (string)satirNesneler[6];
                string ALTPROJEAD = (string)satirNesneler[7];

                string PEBKOD = (string)satirNesneler[8];
                int TOPLAMADAHIL = (int)satirNesneler[9];
                string PEBAD = (string)satirNesneler[10];

                string EKO1 = (string)satirNesneler[11];


                if (groupKod[1] != SEKTORNO)
                {
                    if (groupSatir[1] > 0)
                    {
                        ToplamYazVize(XLS, groupSatir, 1, sutun, toplamAl);
                    }
                    groupKod[1] = SEKTORNO;
                    groupKod[2] = "";
                    satir++;
                    XLS.HucreKopyala(kaynakSatir - 5, sutun, kaynakSatir - 5, sutun + 19, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 5);
                    XLS.HucreDegerYaz(satir, sutun + 1, SEKTORNO);
                    XLS.HucreDegerYaz(satir, sutun + 6, SEKTORAD);
                    groupSatir[1] = satir;
                }
                if (groupKod[2] != PROJENOSTR)
                {
                    if (groupSatir[2] > 0)
                    {
                        ToplamYazVize(XLS, groupSatir, 2, sutun, toplamAl);
                    }
                    groupKod[2] = PROJENOSTR;
                    groupKod[3] = "";
                    satir++;
                    XLS.HucreKopyala(kaynakSatir - 4, sutun, kaynakSatir - 4, sutun + 19, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 5);
                    XLS.HucreDegerYaz(satir, sutun + 2, DPTPROJENO);
                    XLS.HucreDegerYaz(satir, sutun + 6, PROJEAD);
                    groupSatir[2] = satir;
                    altProjeSay = 0;
                }
                if (groupKod[3] != ALTPROJENOSTR)
                {
                    if (groupSatir[3] > 0)
                    {
                        ToplamYazVize(XLS, groupSatir, 3, sutun, toplamAl);
                    }
                    groupKod[3] = ALTPROJENOSTR;
                    groupKod[4] = "";
                    satir++;
                    altProjeSay++;
                    XLS.HucreKopyala(kaynakSatir - 3, sutun, kaynakSatir - 3, sutun + 19, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun + 3, satir, sutun + 5);
                    XLS.HucreDegerYaz(satir, sutun + 3, BIRIM);
                    XLS.HucreDegerYaz(satir, sutun + 6, ALTPROJEAD);
                    groupSatir[3] = satir;
                }

                if (groupKod[4] != PEBKOD)
                {
                    if (groupSatir[4] > 0)
                    {
                        ToplamYazVize(XLS, groupSatir, 4, sutun, toplamAl);
                    }
                    groupKod[4] = PEBKOD;
                    satir++;
                    XLS.HucreKopyala(kaynakSatir - 2, sutun, kaynakSatir - 2, sutun + 19, satir, sutun);
                    XLS.HucreBirlestir(satir, sutun + 4, satir, sutun + 5);
                    XLS.HucreDegerYaz(satir, sutun + 4, PEBKOD);
                    XLS.HucreDegerYaz(satir, sutun + 6, PEBAD);
                    groupSatir[4] = satir;
                }

                satir++;
                XLS.HucreKopyala(kaynakSatir - 1, sutun, kaynakSatir - 1, sutun + 19, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 5, EKO1);
                XLS.HucreDegerYaz(satir, sutun + 6, "");
                groupSatir[5] = satir;

                toplamAl.DegerAta(5, 0, (double)satirNesneler[12]);
                toplamAl.DegerAta(5, 1, (double)satirNesneler[13]);
                toplamAl.DegerAta(5, 2, (double)satirNesneler[14]);
                toplamAl.DegerAta(5, 3, (double)satirNesneler[15]);
                toplamAl.DegerAta(5, 4, (double)satirNesneler[16]);
                toplamAl.DegerAta(5, 5, (double)satirNesneler[17]);
                ToplamYazVize(XLS, groupSatir, 5, sutun, toplamAl);
            }
            for (int i = 4; i >= 0; i--)
            {
                if (groupSatir[i] > 0)
                    ToplamYazVize(XLS, groupSatir, i, sutun, toplamAl);
            }

            XLS.SatirYukseklikAyarla(kaynakSatir, satir, yukseklik);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void ToplamYazVize(Tablo XLS, int[] groupSatir, int satirNo, int sutun, ToplamAl toplamAl)
        {
            if (satirNo == 5)
            {
                toplamAl.ToplamEkle(0, 5);
                toplamAl.ToplamEkle(1, 5);
                toplamAl.ToplamEkle(2, 5);
                toplamAl.ToplamEkle(3, 5);
                toplamAl.ToplamEkle(4, 5);
            }

            XLS.HucreDegerYaz(groupSatir[satirNo], sutun + 7 + 0, toplamAl.toplam[satirNo, 1]);
            XLS.HucreDegerYaz(groupSatir[satirNo], sutun + 7 + 1, toplamAl.toplam[satirNo, 0] + toplamAl.toplam[satirNo, 1]);

            XLS.HucreDegerYaz(groupSatir[satirNo], sutun + 7 + 4, toplamAl.toplam[satirNo, 3]);
            XLS.HucreDegerYaz(groupSatir[satirNo], sutun + 7 + 5, toplamAl.toplam[satirNo, 2] + toplamAl.toplam[satirNo, 3]);

            XLS.HucreDegerYaz(groupSatir[satirNo], sutun + 7 + 8, toplamAl.toplam[satirNo, 5]);
            XLS.HucreDegerYaz(groupSatir[satirNo], sutun + 7 + 9, toplamAl.toplam[satirNo, 4] + toplamAl.toplam[satirNo, 5]);

            for (int i = 0; i < toplamAl.SutunSay(); i++)
            {
                toplamAl.DegerAta(satirNo, i, 0);
            }
        }

        private string ButceSonucHiy(int yil, string senaryoKod, string sektor, string senaryoAd, string birimKod, string birimAd, Hashtable raporParam)
        {
            int avansDurum = OrtakFonksiyonlar.ConvertToInt(HashtableDegerAl(raporParam, "avansDurum", "1"), 1); //1 Hayır, 2 Evet;
            string kur = HashtableDegerAl(raporParam, "kur", TNS.DEG.Arac.VarsayilanKurumBul());
            string fon = HashtableDegerAl(raporParam, "fon", "");
            string fin = HashtableDegerAl(raporParam, "fin", "");
            string eko = HashtableDegerAl(raporParam, "eko", "");
            string peb = HashtableDegerAl(raporParam, "peb", "");
            if (string.IsNullOrWhiteSpace(fon) && string.IsNullOrWhiteSpace(fin) && string.IsNullOrWhiteSpace(eko) && string.IsNullOrWhiteSpace(peb))
                eko = "05,06,07,08";
            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kur, fon, fin, eko, peb);

            ObjectArray bilgi = servisYAT.ProjeHarcamalari(kullanan, yil, senaryoKod, kistasTertip, birimKod, avansDurum, true);
            bilgi.objeler.Sort(new SektorProjeAPEkoSirala());
            if (!bilgi.sonuc.islemSonuc)
                throw new RaporHatasi(bilgi.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int kaynakSatir = 0;
            int kaynakSutun = 0;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref kaynakSutun);
            kaynakSatir = 0;
            int yukseklik = XLS.SatirYukseklikAl(satir);
            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("SenaryoAd", senaryoAd);

            int oncDPTprojeSatir = -1;
            string oncDPTprojeKod = "";
            int oncSektorSatir = -1;
            string oncSektorKod = "";
            int oncAltProjeNo = -1;
            ToplamAl toplamAl = new ToplamAl(5, 5);
            toplamAl.SatirSifirla(0);
            toplamAl.SatirSifirla(1);
            string oncAltProjeAd = "";
            List<OdenekBilgi> ekoToplam = new List<OdenekBilgi>();
            for (int i = 0; i < bilgi.ObjeSayisi; i++)
            {
                ArrayList al = (ArrayList)bilgi.objeler[i];

                string sektorKod = (string)al[0];
                string sektorAd = (string)al[1];
                string DPTprojeKod = (string)al[2];
                string projeAd = (string)al[3];
                int altProjeNo = (int)al[4];
                string altProjeAd = (string)al[5];
                OdenekBilgi ob = (OdenekBilgi)al[6];
                if (DPTprojeKod != oncDPTprojeKod || oncSektorKod != sektorKod || oncAltProjeNo != altProjeNo)
                {
                    if (oncAltProjeNo > 0)
                        AltProjeYaz(XLS, oncAltProjeNo, oncAltProjeAd, toplamAl, ekoToplam, kaynakSatir, kaynakSutun, ref satir);
                    oncAltProjeNo = altProjeNo;
                    oncAltProjeAd = altProjeAd;
                }

                if (oncSektorKod != sektorKod)
                {
                    XLS.HucreKopyala(kaynakSatir, kaynakSutun, kaynakSatir, kaynakSutun + 10, satir, kaynakSutun);
                    XLS.HucreDegerYaz(satir, kaynakSutun, sektorKod);
                    XLS.HucreDegerYaz(satir, kaynakSutun + 4, sektorAd);
                    SadeceRakamlariYaz(XLS, oncSektorSatir, kaynakSutun + 5, toplamAl, 0);
                    oncSektorKod = sektorKod;
                    oncSektorSatir = satir++;
                }

                if (oncSektorKod != sektorKod || DPTprojeKod != oncDPTprojeKod)
                {
                    XLS.HucreKopyala(kaynakSatir, kaynakSutun, kaynakSatir, kaynakSutun + 10, satir, kaynakSutun);
                    XLS.HucreDegerYaz(satir, kaynakSutun + 1, DPTprojeKod);
                    XLS.HucreDegerYaz(satir, kaynakSutun + 4, projeAd);
                    SadeceRakamlariYaz(XLS, oncDPTprojeSatir, kaynakSutun + 5, toplamAl, 1);
                    oncDPTprojeKod = DPTprojeKod;
                    oncDPTprojeSatir = satir++;
                }

                ListeyeEkle(ekoToplam, ob);
            }
            if (oncAltProjeNo > 0)
                AltProjeYaz(XLS, oncAltProjeNo, oncAltProjeAd, toplamAl, ekoToplam, kaynakSatir, kaynakSutun, ref satir);
            SadeceRakamlariYaz(XLS, oncDPTprojeSatir, kaynakSutun + 5, toplamAl, 1);
            SadeceRakamlariYaz(XLS, oncDPTprojeSatir, kaynakSutun + 5, toplamAl, 0);

            XLS.SatirYukseklikAyarla(kaynakSatir, satir, yukseklik);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void ListeyeEkle(List<OdenekBilgi> ekoToplam, OdenekBilgi ob)
        {
            OdenekBilgi bulunan = ListedeBul(ekoToplam, ob);
            if (bulunan == null)
                ekoToplam.Add(ob);
            else
            {
                for (int i = 0; i < bulunan.ekRakamlar.Count; i++)
                {
                    bulunan.ekRakamlar[i] = bulunan.EkRakam(i) + ob.EkRakam(i);
                }
            }
        }

        private OdenekBilgi ListedeBul(List<OdenekBilgi> ekoToplam, OdenekBilgi ob)
        {
            foreach (var item in ekoToplam)
            {
                if (item.tertip.kodlar[3] == ob.tertip.kodlar[3])
                    return item;
            }
            return null;
        }

        private void SadeceRakamlariYaz(Tablo XLS, int satir, int sutun, ToplamAl toplamAl, int toplamSatir)
        {
            if (satir > 0)
            {
                double kbo = toplamAl.toplam[toplamSatir, 0];
                XLS.HucreDegerYaz(satir, sutun++, kbo);
                XLS.HucreDegerYaz(satir, sutun++, kbo + toplamAl.toplam[toplamSatir, 1]);
                sutun++;
                XLS.HucreDegerYaz(satir, sutun++, toplamAl.toplam[toplamSatir, 2]);
                XLS.HucreDegerYaz(satir, sutun++, toplamAl.toplam[toplamSatir, 3]);
            }
            toplamAl.SatirSifirla(toplamSatir);
        }

        private void AltProjeYaz(Tablo XLS, int altProjeNo, string altProjeAd, ToplamAl toplamAl, List<OdenekBilgi> ekoToplam, int kaynakSatir, int kaynakSutun, ref int satir)
        {
            XLS.HucreKopyala(kaynakSatir, kaynakSutun, kaynakSatir, kaynakSutun + 10, satir, kaynakSutun);
            XLS.HucreDegerYaz(satir, kaynakSutun + 2, altProjeNo);
            XLS.HucreDegerYaz(satir, kaynakSutun + 4, altProjeAd);
            int saklaSatir = satir;
            satir++;
            if (satir > 1 && ekoToplam != null)
            {
                foreach (var ob in ekoToplam)
                {
                    XLS.HucreKopyala(kaynakSatir, kaynakSutun, kaynakSatir, kaynakSutun + 10, satir, kaynakSutun);
                    XLS.HucreDegerYaz(satir, kaynakSutun + 3, ob.tertip.kodlar[3]);
                    XLS.HucreDegerYaz(satir, kaynakSutun + 4, ob.tertip.adlar[3]);
                    for (int j = 0; j < 4; j++)
                    {
                        toplamAl.DegerAta(3, j, ob.EkRakam(j));
                    }
                    toplamAl.ToplamEkle(0, 3);
                    toplamAl.ToplamEkle(1, 3);
                    toplamAl.ToplamEkle(2, 3);
                    SadeceRakamlariYaz(XLS, satir, kaynakSutun + 5, toplamAl, 3);
                    satir++;
                }
            }

            SadeceRakamlariYaz(XLS, saklaSatir, kaynakSutun + 5, toplamAl, 2);
            ekoToplam.Clear();
        }

        private string ButceSonuc(int yil, string senaryoKod, string sektor, string senaryoAd, string birimKod, string birimAd, Hashtable raporParam)
        {
            int avansDurum = OrtakFonksiyonlar.ConvertToInt(HashtableDegerAl(raporParam, "avansDurum", "1"), 1); //1 Hayır, 2 Evet;
            string kur = HashtableDegerAl(raporParam, "kur", TNS.DEG.Arac.VarsayilanKurumBul());
            string fon = HashtableDegerAl(raporParam, "fon", "");
            string fin = HashtableDegerAl(raporParam, "fin", "");
            string eko = HashtableDegerAl(raporParam, "eko", "");
            string peb = HashtableDegerAl(raporParam, "peb", "");
            if (string.IsNullOrWhiteSpace(fon) && string.IsNullOrWhiteSpace(fin) && string.IsNullOrWhiteSpace(eko) && string.IsNullOrWhiteSpace(peb))
                eko = "05,06,07,08";
            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kur, fon, fin, eko, peb);

            ObjectArray bilgi = servisYAT.ProjeHarcamalari(kullanan, yil, senaryoKod, kistasTertip, birimKod, avansDurum, true);
            if (!bilgi.sonuc.islemSonuc)
                throw new RaporHatasi(bilgi.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            int kaynakSutun = 0;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref kaynakSutun);
            int yukseklik = XLS.SatirYukseklikAl(kaynakSatir);
            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("SenaryoAd", senaryoAd);
            satir = kaynakSatir;

            for (int i = 0; i < bilgi.ObjeSayisi; i++)
            {
                ArrayList al = (ArrayList)bilgi.objeler[i];

                string sektorKod = (string)al[0];
                string sektorAd = (string)al[1];
                string DPTprojeKod = (string)al[2];
                string projeAd = (string)al[3];
                int altProjeNo = (int)al[4];
                string altProjeAd = (string)al[5];
                OdenekBilgi ob = (OdenekBilgi)al[6];

                sutun = kaynakSutun;
                XLS.HucreKopyala(kaynakSatir - 1, kaynakSutun, kaynakSatir - 1, kaynakSutun + 22, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun++, sektorKod);
                XLS.HucreDegerYaz(satir, sutun++, sektorAd);
                XLS.HucreDegerYaz(satir, sutun++, DPTprojeKod);
                XLS.HucreDegerYaz(satir, sutun++, projeAd);
                XLS.HucreDegerYaz(satir, sutun++, altProjeNo);
                XLS.HucreDegerYaz(satir, sutun++, altProjeAd);

                for (int j = 0; j < 5; j++)
                {
                    XLS.HucreDegerYaz(satir, sutun++, ob.tertip.kodlar[j]);
                    XLS.HucreDegerYaz(satir, sutun++, ob.tertip.adlar[j]);
                }

                double kbo = ob.EkRakam(0);
                XLS.HucreDegerYaz(satir, sutun++, kbo);
                XLS.HucreDegerYaz(satir, sutun++, kbo + ob.EkRakam(1));
                sutun++;
                XLS.HucreDegerYaz(satir, sutun++, ob.EkRakam(2));
                XLS.HucreDegerYaz(satir, sutun++, ob.EkRakam(3));
                satir++;
            }

            XLS.SatirYukseklikAyarla(kaynakSatir, satir, yukseklik);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string HashtableDegerAl(Hashtable raporParam, string aranan, string defaultDeger)
        {
            string donDeger = null;
            try
            {
                donDeger = (string)raporParam[aranan];
            }
            catch
            {
            }
            if (donDeger == null) donDeger = defaultDeger;
            return donDeger;
        }

        private string ButceIslem(int yil, string senaryoKod, string sektor, string senaryoAd, string birimKod, string birimAd, Hashtable raporParam)
        {
            MPRevizeFormSorgu form = (MPRevizeFormSorgu)raporParam["form"];

            if (form == null)
            {
                form = new MPRevizeFormSorgu();
                form.yil = yil;
                form.tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, birimKod, "", "", "05,06,07,08", "");
                form.formDurumAralik = "5";
                form.revizeFormTip = ENUMRevizeFormTip.AktarmaEklemeler;
            }

            ObjectArray bilgi = servisYAT.ProjeIslemleri(kullanan, form);
            if (!bilgi.sonuc.islemSonuc)
                throw new RaporHatasi(bilgi.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            int kaynakSutun = 0;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref kaynakSutun);
            int yukseklik = XLS.SatirYukseklikAl(kaynakSatir);
            XLS.HucreAdBulYaz("Yil", yil);

            satir = kaynakSatir;
            TNS.BUM.IBUMServis srvBUM = TNS.BUM.Arac.Tanimla();
            ObjectArray oa = srvBUM.ButceIslemListesi("", false, false);

            for (int i = 0; i < bilgi.ObjeSayisi; i++)
            {
                ArrayList al = (ArrayList)bilgi.objeler[i];

                string sektorKod = (string)al[0];
                string sektorAd = (string)al[1];
                string DPTprojeKod = (string)al[2];
                string projeAd = (string)al[3];
                int altProjeNo = (int)al[4];
                string altProjeAd = (string)al[5];

                string kurKod = (string)al[6];
                string fonKod = (string)al[7];
                string finKod = (string)al[8];
                string ekoKod = (string)al[9];
                string pebKod = (string)al[10];
                string kyKod = (string)al[11];

                double miktar = (double)al[12];
                int revizeNo = (int)al[13];
                TNSDateTime kayitTarih = (TNSDateTime)al[14];
                int belgeTur = (int)al[15];
                string aciklama = (string)al[16];
                string bumkoIslem = (string)al[17];
                string altIslemKod = (string)al[18];
                int formDurum = (int)al[19];

                sutun = kaynakSutun;
                XLS.HucreKopyala(kaynakSatir - 2, kaynakSutun, kaynakSatir - 2, kaynakSutun + 23, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun++, sektorKod);
                XLS.HucreDegerYaz(satir, sutun++, sektorAd);
                XLS.HucreDegerYaz(satir, sutun++, DPTprojeKod);
                XLS.HucreDegerYaz(satir, sutun++, projeAd);
                XLS.HucreDegerYaz(satir, sutun++, altProjeNo);
                XLS.HucreDegerYaz(satir, sutun++, altProjeAd);
                XLS.HucreDegerYaz(satir, sutun++, kurKod);
                XLS.HucreDegerYaz(satir, sutun++, fonKod);
                XLS.HucreDegerYaz(satir, sutun++, finKod);
                XLS.HucreDegerYaz(satir, sutun++, ekoKod);
                XLS.HucreDegerYaz(satir, sutun++, pebKod);
                XLS.HucreDegerYaz(satir, sutun++, kyKod);

                XLS.HucreDegerYaz(satir, sutun++, miktar);
                XLS.HucreDegerYaz(satir, sutun++, revizeNo);
                XLS.HucreDegerYaz(satir, sutun++, kayitTarih.ToString());
                XLS.HucreDegerYaz(satir, sutun++, TNS.BKD.Arac.RevizeFormTipAl((ENUMRevizeFormTip)belgeTur));
                XLS.HucreDegerYaz(satir, sutun++, aciklama);
                XLS.HucreDegerYaz(satir, sutun++, bumkoIslem);
                XLS.HucreDegerYaz(satir, sutun++, AltIslemAdBul(oa, altIslemKod));
                XLS.HucreDegerYaz(satir, sutun++, TNS.BHMv2.Arac.RevizeFormDurumAdBul((TNS.BKD.ENUMRevizeFormDurum)formDurum));

                satir++;
            }

            XLS.SatirYukseklikAyarla(kaynakSatir, satir, yukseklik);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string AltIslemAdBul(ObjectArray oa, string altIslemKod)
        {
            foreach (ListeSatir ls in oa.objeler)
            {
                if (ls.kod.Replace(".", "") == altIslemKod.Replace(".", ""))
                    return ls.ad;
            }
            return "";
        }

        private string KrediIzlemeRaporu(int butceYil, string senaryoKod, string kaynakNoStr, string senaryoAd, string birimKod, string birimAd, Hashtable raporParam)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int kaynakSatir = 0;
            int kaynakSutun = 0;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref kaynakSutun);
            XLS.HucreAdBulYaz("ButceYil", butceYil);
            satir = kaynakSatir;

            if (string.IsNullOrWhiteSpace(kaynakNoStr))
            {
                XLS.HucreDegerYaz(satir, kaynakSutun, "Bütçe Kaynak No boş geliyor.");
            }
            else
            {
                string[] kaynakNoDizi = kaynakNoStr.Split(',');
                for (int i = 1; i < kaynakNoDizi.Length; i++)
                {
                    XLS.YeniSheetEkle(0);
                }
                int shNo = 0;
                foreach (string kaynakNoIcStr in kaynakNoDizi)
                {
                    XLS.AktifSheetDegistir(shNo);
                    shNo++;
                    int kaynakNo = OrtakFonksiyonlar.ConvertToInt(kaynakNoIcStr, -1);
                    BilgiVAN bilgi = servisYAT.ButceKaynakBilgiOku(kullanan, kaynakNo, butceYil, senaryoKod, string.Empty, true, true, false, null);

                    if (!bilgi.sonuc.islemSonuc)
                    {
                        XLS.HucreDegerYaz(satir, kaynakSutun, bilgi.sonuc.hataStr);
                    }
                    else if (bilgi.liste.Count == 0)
                    {
                        XLS.HucreDegerYaz(satir, kaynakSutun, bilgi.sonuc.bilgiStr);
                    }
                    else
                    {
                        ButceKaynak kaynak = (ButceKaynak)bilgi.liste[0];
                        XLS.HucreAdBulYaz("Parabirimi", kaynak.pbKisaltma);

                        XLS.HucreDegerYaz(satir, kaynakSutun, kaynak.sektorAd);
                        Proje proje = new Proje { kod = kaynak.projeNo };
                        BilgiVAN projeler = servisYAT.ProjeListele(kullanan, butceYil, senaryoKod, proje, null, false);
                        if (projeler.sonuc.islemSonuc && projeler.liste.Count > 0)
                        {
                            proje = (Proje)projeler.liste[0];
                        }

                        XLS.HucreDegerYaz(satir + 1, kaynakSutun, proje.yurutecekKurumAd);
                        XLS.HucreDegerYaz(satir + 2, kaynakSutun, proje.ad);
                        XLS.HucreDegerYaz(satir + 3, kaynakSutun, proje.DPTProjeNo);
                        XLS.HucreDegerYaz(satir + 4, kaynakSutun, proje.ProjeYeri());
                        XLS.HucreDegerYaz(satir + 5, kaynakSutun, kaynak.projeToplam_Dis / Bolen);
                        XLS.HucreDegerYaz(satir + 6, kaynakSutun, kaynak.projeToplam_Toplam / Bolen);
                        XLS.HucreDegerYaz(satir + 7, kaynakSutun, proje.baslangicYili + "/" + proje.bitisYili);
                        XLS.HucreDegerYaz(satir + 8, kaynakSutun, proje.karakteristik);
                        XLS.HucreDegerYaz(satir + 11, kaynakSutun + 1, kaynak.krediyiVeren);
                        XLS.HucreDegerYaz(satir + 12, kaynakSutun, kaynak.baslamaTarih.ToString() + "/" + kaynak.yururlukTarih.ToString());
                        XLS.HucreDegerYaz(satir + 13, kaynakSutun, kaynak.tur);
                        XLS.HucreDegerYaz(satir + 14, kaynakSutun, kaynak.tutar);
                        XLS.HucreDegerYaz(satir + 15, kaynakSutun, kaynak.vade + "/" + kaynak.odemesizDonem);
                        XLS.HucreDegerYaz(satir + 16, kaynakSutun, kaynak.faizOran);
                        XLS.HucreDegerYaz(satir + 17, kaynakSutun, kaynak.IcParaKrediOran);
                        int sat;
                        int sut;
                        for (int i = 0; i < kaynak.krediPlan.Count; i++)
                        {
                            KrediDetay kd = (KrediDetay)kaynak.krediPlan[i];
                            SatirBul(kd.yil, butceYil, 0, out sat, out sut);
                            if (sat >= 0) XLS.HucreDegerYaz(satir + 19 + sat, kaynakSutun + sut, kd.anlasmaTutari / Bolen);
                            SatirBul(kd.yil, butceYil, 1, out sat, out sut);
                            if (sat >= 0) XLS.HucreDegerYaz(satir + 23 + sat, kaynakSutun + sut, kd.kullanilanTutar / Bolen);
                            SatirBul(kd.yil, butceYil, 2, out sat, out sut);
                            if (sat >= 0) XLS.HucreDegerYaz(satir + 27 + sat, kaynakSutun + sut, kd.kullanilacakTutar / Bolen);
                        }
                        ButceKaynakGeriOdeme go = GeriOdemeBul(kaynak.geriOdeme, butceYil);
                        if (go == null) go = new ButceKaynakGeriOdeme();
                        XLS.HucreDegerYaz(satir + 28, kaynakSutun + 1, go.anaParaGeriOdeme / Bolen);
                        XLS.HucreDegerYaz(satir + 29, kaynakSutun + 1, go.odenecekFaiz / Bolen);
                        XLS.HucreDegerYaz(satir + 30, kaynakSutun, go.projeninSonDurumu + "\r\n" + go.darBogazCozum);
                    }
                }
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private ButceKaynakGeriOdeme GeriOdemeBul(TNSCollection geriOdemeler, int butceYil)
        {
            if (geriOdemeler != null)
            {
                foreach (ButceKaynakGeriOdeme item in geriOdemeler)
                {
                    if (item.yil == butceYil) return item;
                }
            }
            return null;
        }

        private void SatirBul(int krediYil, int butceYil, int tur, out int sat, out int sut)
        {
            int min = -9;
            int max = 4;
            sat = -1;
            sut = -1;
            if (tur == 1) max = -1;
            else if (tur == 2) min = -1;
            int fark = krediYil - butceYil;
            if (fark >= min && fark <= max)
            {
                sat = 0;
                sut = fark - min;
                if (sut >= 7)
                {
                    sat = 2;
                    sut = sut - 7;
                }
            }
        }

        private string StrPlanRaporu(int yil, string senaryoKod, string senaryoAd, string birimKod, string birimAd, string sektor)
        {
            string sql = @"SELECT DISTINCT PRJ.DPTPROJENO, PRJ.AD, ST.SAAD, SHAD, PERH.HEDEFGORKOD, PERH.KPAMAC, PERH.KPHEDEF, PERH.KPPOLITIKA, APRJ.AD FROM 
                           PerformansButce.dbo.PEBSTRAGIRLIKVIEW AS ST 
                           INNER JOIN PerformansButce.dbo.PEBPERHEDEF AS PERH ON PERH.YIL=ST.YIL AND PERH.KURKOD=ST.PHKURKOD AND PERH.HEDEFKOD=ST.PHKOD
                           INNER JOIN PerformansButce.dbo.PEBFAALIYETPROJEPEBFON AS PEB ON ST.FPKOD = PEB.MALIYETKOD AND ST.FPKURKOD = PEB.MALIYETKURKOD AND ST.YIL = PEB.YIL
                           INNER JOIN Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS KAYPEB ON KAYPEB.YIL = PEB.YIL AND KAYPEB.PEBKOD = PEB.PEBKOD 
                           INNER JOIN Yatirim.dbo.YATBUTCEKAYNAK AS KAY ON KAY.YIL = KAYPEB.YIL AND KAY.SENARYOKOD = KAYPEB.SENARYOKOD AND KAY.KAYNAKNO = KAYPEB.KAYNAKNO 
                           INNER JOIN Yatirim.dbo.YATALTPROJE AS APRJ ON APRJ.YIL = KAY.YIL AND APRJ.SENARYOKOD = KAY.SENARYOKOD AND APRJ.ALTPROJENO = KAY.ALTPROJENO
                           INNER JOIN Yatirim.dbo.YATPROJE AS PRJ ON PRJ.YIL = APRJ.YIL AND PRJ.SENARYOKOD = APRJ.SENARYOKOD AND PRJ.PROJENO = APRJ.PROJENO 
                           where CARI = 0 AND PRJ.YIL={0} AND PRJ.SENARYOKOD = '{1}' AND PRJ.SEKTORNO LIKE '{2}%' AND PRJ.YURUTECEKKURUM LIKE '{3}%'";

            sql = String.Format(sql, yil, senaryoKod.Replace(".", ""), sektor.Replace(".", ""), birimKod.Replace(".", ""));
            if (this.altProjeNo > 0)
                sql += " AND APRJ.ALTPROJENO=" + this.altProjeNo.ToString();
            if (this.projeNo > 0)
                sql += " AND APRJ.PROJENO=" + this.projeNo.ToString();

            sql += " order by 1,3,4,5";
            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

            TNS.UZY.VTSorgula vts = new VTSorgula(sql, "BaglantiSatiriYAT", 0);

            Tablo XLS = GenelIslemler.NewTablo();
            int formatSatir = 0;
            int formatSutun = 0;
            int baslaSatir = 0;
            int baslaSutun = 0;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("FormatSatir", ref formatSatir, ref formatSutun);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("SektorAd", sektorAd);
            XLS.HucreAdBulYaz("Kurulus", YatRaporAraclari.VarsayilanKurumBulAd());

            System.Data.DataRow dr;
            string projeNo = "";
            string projeAd = "";
            string sa = "";
            string sh = "";
            string ph = "";

            string kpah = "";
            string kpp = "";

            int say = 0;
            int yuks = XLS.SatirYukseklikAl(baslaSatir);
            if (!string.IsNullOrWhiteSpace(vts.hata))
            {
                XLS.HucreDegerYaz(baslaSatir, baslaSutun + 0, sql);
                XLS.HucreDegerYaz(baslaSatir, baslaSutun + 1, vts.hata);
            }
            while ((dr = vts.satirOku()) != null)
            {
                string DPTPROJENO = vts.StringAl(0);
                string AD = vts.StringAl(1);
                string SAAD = vts.StringAl(2);
                string SHAD = vts.StringAl(3);
                string HEDEFKOD = vts.StringAl(4);

                string KPAMAC = vts.StringAl(5);
                string KPHEDEF = vts.StringAl(6);
                string KPPOLITIKA = vts.StringAl(7);
                string ALTPROJEAD = vts.StringAl(8);
                if (projeNo != DPTPROJENO)
                {
                    if (projeNo != "")
                    {
                        StrHedefPrjYaz(XLS, say, false, baslaSatir, baslaSutun, formatSatir, projeNo, projeAd, sa, sh, ph, kpah, kpp);
                        say++;
                    }
                    projeNo = DPTPROJENO;
                    projeAd = AD;
                    if (this.altProjeNo > 0)
                        projeAd += " (" + ALTPROJEAD + ")";
                    sa = "";
                    sh = "";
                    ph = "";
                    kpah = "";
                    kpp = "";
                }
                sa = StrEkle(sa, SAAD);
                sh = StrEkle(sh, SHAD);
                ph = StrEkle(ph, HEDEFKOD);
                kpah = StrEkle(kpah, KPAMAC);
                kpah = StrEkle(kpah, KPHEDEF);
                kpp = StrEkle(kpp, KPPOLITIKA);
            }
            if (projeNo != "")
            {
                StrHedefPrjYaz(XLS, say, true, baslaSatir, baslaSutun, formatSatir, projeNo, projeAd, sa, sh, ph, kpah, kpp);
                say++;
            }
            XLS.SatirYukseklikAyarla(baslaSatir, baslaSatir + say, yuks);
            XLS.SatirYukseklikAyarla(baslaSatir, baslaSatir + say, -1);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void StrHedefPrjYaz(Tablo XLS, int say, bool sonSatir, int baslaSatir, int baslaSutun, int formatSatir, string projeNo, string projeAd, string sa, string sh, string ph, string kpah, string kpp)
        {
            int ekle = 0;
            if (sonSatir) ekle = 1;
            else if (say == 0) ekle = -1;
            int sat = baslaSatir + say;
            XLS.SatirAc(sat, 1);
            XLS.HucreKopyala(formatSatir + ekle, baslaSutun, formatSatir + ekle, baslaSutun + 7, sat, baslaSutun);
            XLS.HucreDegerYaz(sat, baslaSutun + 0, projeNo);
            XLS.HucreDegerYaz(sat, baslaSutun + 1, projeAd);
            XLS.HucreDegerYaz(sat, baslaSutun + 2, kpah);
            XLS.HucreDegerYaz(sat, baslaSutun + 3, kpp);
            XLS.HucreDegerYaz(sat, baslaSutun + 4, sa);
            XLS.HucreDegerYaz(sat, baslaSutun + 5, sh);
            XLS.HucreDegerYaz(sat, baslaSutun + 6, ph);
        }

        private string StrEkle(string gelen, string eklenecek)
        {
            if (string.IsNullOrWhiteSpace(gelen)) return eklenecek;
            if (string.IsNullOrWhiteSpace(eklenecek)) return gelen;
            if (gelen.IndexOf(eklenecek) > -1) return gelen;
            return gelen + ", " + eklenecek;
        }

        private string YollukPlani(int yil, string senaryoKod, string senaryoAd, string birimKod, string birimAd, string sektor)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int formatSatir = 0;
            int formatSutun = 0;
            int baslaSatir = 0;
            int baslaSutun = 0;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("FormatSatir", ref formatSatir, ref formatSutun);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
            XLS.HucreAdBulYaz("Yil", yil);
            int yuks = XLS.SatirYukseklikAl(baslaSatir);
            IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
            string pebKod = (string)this.raporParamAna["PebKod"];
            if (string.IsNullOrWhiteSpace(pebKod))
            {
                pebKod = YollukPlanlamaRaporu.PebKodlariBul(kullanan, servisBHMv2, yil, senaryoKod, projeNo, altProjeNo);
            }
            ObjectArray oa = null;
            if (string.IsNullOrWhiteSpace(pebKod))
                oa = new ObjectArray();
            else
                oa = servisYAT.ListeleYollukPlanlama(kullanan, yil, senaryoKod, null, pebKod);
            if (!oa.sonuc.islemSonuc)
                throw new RaporHatasi(oa.sonuc.hataStr);
            int satir = baslaSatir;
            double top = 0;
            foreach (YollukPlan yp in oa.objeler)
            {
                XLS.HucreKopyala(formatSatir, formatSutun, formatSatir, formatSutun + 12, satir, baslaSutun);
                XLS.HucreDegerYaz(satir, baslaSutun + 0, satir - baslaSatir + 1);
                string yaz = yp.yurutenBirim;
                if (string.IsNullOrWhiteSpace(yaz)) yaz = yp.faaliyetBirimAd;
                XLS.HucreDegerYaz(satir, baslaSutun + 1, yaz);

                XLS.HucreDegerYaz(satir, baslaSutun + 2, yp.dairesi);

                yaz = yp.altProjeAdi;
                if (string.IsNullOrWhiteSpace(yaz)) yaz = yp.faaliyetAd;
                XLS.HucreDegerYaz(satir, baslaSutun + 3, yaz);
                XLS.HucreDegerYaz(satir, baslaSutun + 4, yp.konusu);
                XLS.HucreDegerYaz(satir, baslaSutun + 5, yp.suresi);
                XLS.HucreDegerYaz(satir, baslaSutun + 6, yp.kisiSayisi);
                XLS.HucreDegerYaz(satir, baslaSutun + 8, yp.yapilacagiAy);
                XLS.HucreDegerYaz(satir, baslaSutun + 9, yp.yeri);
                XLS.HucreDegerYaz(satir, baslaSutun + 10, yp.tertip);
                XLS.HucreDegerYaz(satir, baslaSutun + 11, yp.mALIYET);
                top += yp.mALIYET;
                satir++;
            }
            XLS.HucreKopyala(formatSatir + 1, formatSutun, formatSatir + 1, formatSutun + 12, satir, baslaSutun);
            XLS.HucreDegerYaz(satir, baslaSutun + 11, top);
            XLS.SatirYukseklikAyarla(baslaSatir, satir, yuks);
            XLS.SatirYukseklikAyarla(baslaSatir, satir, -1);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string UygulamaRaporu(int yil, string senaryoKod, string senaryoAd, string birimKod, string birimAd, string sektor)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int formatSatir = 0;
            int formatSutun = 0;
            int baslaSatir = 0;
            int baslaSutun = 0;
            int formatSheet1 = 0;
            int formatSatir2 = 0;
            int formatSutun2 = 0;

            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("FormatSatir", ref formatSheet1, ref formatSatir, ref formatSutun, ref formatSatir2, ref formatSutun2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
            XLS.HucreAdBulYaz("butceYil", yil);
            int yuks = XLS.SatirYukseklikAl(baslaSatir);
            IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
            Proje proje = new Proje();
            proje.siralama = 2;
            proje.aktiflik = 1;
            proje.sektor.kod = sektor;
            proje.birim.kod = birimKod;
            ObjectArray oa = servisYAT.ProjeListele(kullanan, yil, senaryoKod, proje, true, true, 0);
            if (!oa.sonuc.islemSonuc)
                throw new RaporHatasi(oa.sonuc.hataStr);
            int satir = baslaSatir;
            ToplamAl toplam = new ToplamAl(4, 27);
            foreach (Proje p in oa.objeler)
            {
                int pSat = satir;
                XLS.HucreKopyala(formatSatir + 1, formatSutun, formatSatir + 1, formatSutun2, pSat, baslaSutun);
                XLS.HucreDegerYaz(pSat, baslaSutun + 2, p.DPTProjeNo);
                XLS.HucreDegerYaz(pSat, baslaSutun + 3, p.ad);
                XLS.HucreDegerYaz(pSat, baslaSutun + 5, p.bitisYili);
                XLS.HucreDegerYaz(pSat, baslaSutun + 6, p.ProjeYeri());
                satir++;
                toplam.SatirSifirla(1);
                foreach (AltProje altP in p.altProjeler)
                {
                    AltProjeMaliBilgiBul(yil, altP, toplam);
                    XLS.HucreKopyala(formatSatir + 2, formatSutun, formatSatir + 2, formatSutun2, satir, baslaSutun);
                    XLS.HucreDegerYaz(satir, baslaSutun + 3, altP.ad);
                    XLS.HucreDegerYaz(satir, baslaSutun + 5, altP.bitisYil);
                    XLS.HucreDegerYaz(satir, baslaSutun + 6, altP.ProjeYeri());
                    ToplamYazUR(toplam, 2, XLS, satir, baslaSutun + 7);
                    satir++;
                }
                ToplamYazUR(toplam, 1, XLS, pSat, baslaSutun + 7);
            }
            XLS.HucreKopyala(formatSatir + 3, formatSutun, formatSatir + 3, formatSutun2, satir, baslaSutun);
            ToplamYazUR(toplam, 0, XLS, satir, baslaSutun + 7);
            XLS.SatirYukseklikAyarla(baslaSatir, satir, -1, 7, yuks);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void ToplamYazUR(ToplamAl toplam, int toplamSat, Tablo XLS, int xlsSatir, int xlsSutun)
        {
            for (int i = 0; i < toplam.SutunSay(); i++)
            {
                XLS.HucreDegerYaz(xlsSatir, xlsSutun, toplam.toplam[toplamSat, i]);
                xlsSutun++;
                if (i == 6 || i == 10 || i == 22)
                    xlsSutun++;
                if (i == 8 || i == 12 || i == 16 || i == 20 || i == 24)
                    xlsSutun += 2;
                if (i == 14 || i == 18)
                    xlsSutun += 3;
            }
        }

        private void AltProjeMaliBilgiBul(int yil, AltProje alt, ToplamAl toplamAl)
        {
            toplamAl.SatirSifirla(2);
            foreach (ButceKaynak bk in alt.butceKaynaklari)
            {
                toplamAl.SatirSifirla(3);
                foreach (ButceKaynakTeklif teklif in bk.teklifler)
                {
                    if (teklif.butceYil == yil)
                    {
                        toplamAl.DegerAta(3, 0, teklif.ProjeTutarDis / Bolen);
                        toplamAl.DegerAta(3, 1, teklif.ProjeTutarToplam / Bolen);

                        toplamAl.DegerAta(3, 4, teklif.Odenek0Dis / Bolen);
                        toplamAl.DegerAta(3, 5, teklif.Odenek0DisaGiden / Bolen);
                        toplamAl.DegerAta(3, 6, teklif.Odenek0Toplam / Bolen);
                    }
                    else if (teklif.butceYil == yil - 1)
                    {
                        toplamAl.DegerAta(3, 2, teklif.KumulatifHarcamaDis / Bolen);
                        toplamAl.DegerAta(3, 3, teklif.KumulatifHarcamaToplam / Bolen);
                    }
                }

                foreach (KaynakOdenekBilgi ob in bk.odenekDegisimler)
                {
                    int donem = (ob.ay - 1) / 3;
                    int ekKol = donem * 4;
                    if (ob.finKod == "7")
                    {
                        toplamAl.DegerArttir(3, 7 + ekKol, ob.odenek / Bolen);
                        toplamAl.DegerArttir(3, 23, ob.odenek / Bolen);
                    }
                    toplamAl.DegerArttir(3, 8 + ekKol, ob.odenek / Bolen);
                    toplamAl.DegerArttir(3, 24, ob.odenek / Bolen);
                }

                foreach (KaynakOdenekBilgi ob in bk.harcamalar)
                {
                    int donem = (ob.ay - 1) / 3;
                    int ekKol = donem * 4;
                    if (ob.finKod == "7")
                    {
                        toplamAl.DegerArttir(3, 9 + ekKol, ob.odenek / Bolen);
                        toplamAl.DegerArttir(3, 25, ob.odenek / Bolen);
                    }
                    toplamAl.DegerArttir(3, 10 + ekKol, ob.odenek / Bolen);
                    toplamAl.DegerArttir(3, 26, ob.odenek / Bolen);
                }

                if (bk.kaynakTur != ENUMButceKaynakTur.HIBE)
                {
                    toplamAl.ToplamEkle(2, 3);
                    if (bk.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeGeneleDahil || bk.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeDahilGeneleHaric)
                        toplamAl.ToplamEkle(1, 3);

                    if (bk.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeGeneleDahil || bk.toplamaDahil == ButceKaynak.ENUMKaynakToplam.ProjeyeHaricGeneleDahil)
                        toplamAl.ToplamEkle(0, 3);
                }
            }
        }

        private class SektorProjeAPEkoSirala : IComparer
        {
            public int Compare(object x, object y)
            {
                ArrayList alx = (ArrayList)x;
                ArrayList aly = (ArrayList)y;

                string sektorKodx = (string)alx[0];
                string sektorKody = (string)aly[0];
                int siraKod = sektorKodx.CompareTo(sektorKody);
                if (siraKod != 0) return siraKod;

                string DPTprojeKodx = (string)alx[2];
                string DPTprojeKody = (string)aly[2];
                siraKod = DPTprojeKodx.CompareTo(DPTprojeKody);
                if (siraKod != 0) return siraKod;

                int altProjeNox = (int)alx[4];
                int altProjeNoy = (int)aly[4];
                siraKod = altProjeNox.CompareTo(altProjeNoy);

                OdenekBilgi obx = (OdenekBilgi)alx[6];
                OdenekBilgi oby = (OdenekBilgi)aly[6];

                return obx.tertip.kodlar[3].CompareTo(oby.tertip.kodlar[3]);
            }
        }
    }
}
