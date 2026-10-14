using System;
using System.Collections;
using System.Data;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.YAT;

namespace ButceHazirlikv2
{
    /// <summary>
    /// Proje Gerekçe Girişi sayfasının raporları; eski ProjeGerekceAnaForm.aspx.cs kodunun aynısıdır, yalnızca dosyayı göndermek
    /// yerine üretilen dosyanın yolunu döndürür:
    ///   ListeYazdir:      Yazdır / Yazdır (Ek) (Word; şablon YAT\YATGerekce.docx)
    ///   ListeYazdirTablo: Yazdır (Liste) / Yazdır (Ek Liste) (Excel; şablon YAT\YATProjeListe.xltx)
    ///   OzetYazdir:       Yazdır (Özet) (Excel; şablon YAT\YATProjeOzet.xltx)
    /// Hata olursa iletisi Exception ile döner.
    /// </summary>
    public static class ProjeGerekceRaporlari
    {
        /// <summary>Eski GenelIslemlerIstemci.VarsayilanKurumBulAd.</summary>
        private static string KurumAd()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMAD");
        }

        public static string OzetYazdir(Kullanici kullanan, int yil, string kurKod, string sektorKod, string senaryoKod, string senaryoAd, string raporSablonYol)
        {
            string[] sql = new string[10];
            string[] bag = new string[10];

            if (string.IsNullOrWhiteSpace(kurKod)) kurKod = TNS.DEG.Arac.VarsayilanKurumBul();
            string yeniProje = "SELECT '{0}' AS KURKOD, SUM(O.ODENEK) FROM YATPROJE AS P INNER JOIN ";
            yeniProje += "YATALTPROJE AS AP ON P.YIL = AP.YIL AND P.SENARYOKOD = AP.SENARYOKOD AND P.PROJENO = AP.PROJENO INNER JOIN ";
            yeniProje += "YATBUTCEKAYNAK AS KAY ON KAY.YIL = AP.YIL AND KAY.SENARYOKOD = AP.SENARYOKOD AND KAY.ALTPROJENO = AP.ALTPROJENO INNER JOIN ";
            yeniProje += "YATBUTCEKAYNAKPEBKOD AS K ON K.YIL = KAY.YIL AND K.SENARYOKOD = KAY.SENARYOKOD AND K.KAYNAKNO = KAY.KAYNAKNO INNER JOIN ";
            yeniProje += "ButceHazirlikV2.dbo.BHMODENEK AS O ON K.YIL = O.YIL AND O.SENARYOKOD = K.SENARYOKOD AND K.PEBKOD = O.PEBKOD ";
            yeniProje += "WHERE (P.BASLANGICYILI = {1}) AND (K.YIL = {1}) AND P.CARI <> 1 ";
            yeniProje += "AND O.SENARYOKOD='{2}' AND  O.TEKLIFYIL = 0 AND O.KURKOD LIKE '{0}%'";
            sql[0] = string.Format(yeniProje, kurKod.Replace(".", ""), yil, senaryoKod.Replace(".", ""));

            string yeniProjeEk = "SELECT '{0}' AS KURKOD, SUM(O.ODENEK) FROM YATPROJE AS P INNER JOIN ";
            yeniProjeEk += "YATALTPROJE AS AP ON P.YIL = AP.YIL AND P.SENARYOKOD = AP.SENARYOKOD AND P.PROJENO = AP.PROJENO INNER JOIN ";
            yeniProjeEk += "YATBUTCEKAYNAK AS KAY ON KAY.YIL = AP.YIL AND KAY.SENARYOKOD = AP.SENARYOKOD AND KAY.ALTPROJENO = AP.ALTPROJENO INNER JOIN ";
            yeniProjeEk += "YATBUTCEKAYNAKPEBKOD AS K ON K.YIL = KAY.YIL AND K.SENARYOKOD = KAY.SENARYOKOD AND K.KAYNAKNO = KAY.KAYNAKNO INNER JOIN ";
            yeniProjeEk += "ButceHazirlikV2.dbo.BHMTAVANUSTU AS O ON K.YIL = O.YIL AND O.SENARYOKOD = K.SENARYOKOD AND K.PEBKOD = O.PEBKOD ";
            yeniProjeEk += "WHERE (P.BASLANGICYILI = {1}) AND (K.YIL = {1}) AND P.CARI <> 1 ";
            yeniProjeEk += "AND O.SENARYOKOD='{2}' AND O.KURKOD LIKE '{0}%'";
            sql[1] = string.Format(yeniProjeEk, kurKod.Replace(".", ""), yil, senaryoKod.Replace(".", ""));

            string tavanDisiProje = "SELECT '{0}', SUM(YILITEKLIFI0) FROM YATALTPROJETEKLIF WHERE YIL={1} AND BIRIM LIKE '{0}%'";
            sql[2] = string.Format(tavanDisiProje, kurKod.Replace(".", ""), yil);

            string yeniEskiProje = "SELECT '{0}' AS KURKOD, SUM(O.ODENEK) FROM YATPROJE AS P INNER JOIN ";
            yeniEskiProje += "YATALTPROJE AS AP ON P.YIL = AP.YIL AND P.SENARYOKOD = AP.SENARYOKOD AND P.PROJENO = AP.PROJENO INNER JOIN ";
            yeniEskiProje += "YATBUTCEKAYNAK AS KAY ON KAY.YIL = AP.YIL AND KAY.SENARYOKOD = AP.SENARYOKOD AND KAY.ALTPROJENO = AP.ALTPROJENO INNER JOIN ";
            yeniEskiProje += "YATBUTCEKAYNAKPEBKOD AS K ON K.YIL = KAY.YIL AND K.SENARYOKOD = KAY.SENARYOKOD AND K.KAYNAKNO = KAY.KAYNAKNO INNER JOIN ";
            yeniEskiProje += "ButceHazirlikV2.dbo.BHMODENEK AS O ON K.YIL = O.YIL AND O.SENARYOKOD = K.SENARYOKOD AND K.PEBKOD = O.PEBKOD ";
            yeniEskiProje += "WHERE (P.BASLANGICYILI < {1}) AND (P.ESKIYENI = 1) AND (K.YIL = {1}) AND P.CARI <> 1 ";
            yeniEskiProje += "AND O.SENARYOKOD='{2}' AND  O.TEKLIFYIL = 0 AND O.KURKOD LIKE '{0}%'";
            sql[3] = string.Format(yeniEskiProje, kurKod.Replace(".", ""), yil, senaryoKod.Replace(".", ""));

            string yeniEskiProjeEk = "SELECT '{0}' AS KURKOD, SUM(O.ODENEK) FROM YATPROJE AS P INNER JOIN ";
            yeniEskiProjeEk += "YATALTPROJE AS AP ON P.YIL = AP.YIL AND P.SENARYOKOD = AP.SENARYOKOD AND P.PROJENO = AP.PROJENO INNER JOIN ";
            yeniEskiProjeEk += "YATBUTCEKAYNAK AS KAY ON KAY.YIL = AP.YIL AND KAY.SENARYOKOD = AP.SENARYOKOD AND KAY.ALTPROJENO = AP.ALTPROJENO INNER JOIN ";
            yeniEskiProjeEk += "YATBUTCEKAYNAKPEBKOD AS K ON K.YIL = KAY.YIL AND K.SENARYOKOD = KAY.SENARYOKOD AND K.KAYNAKNO = KAY.KAYNAKNO INNER JOIN ";
            yeniEskiProjeEk += "ButceHazirlikV2.dbo.BHMTAVANUSTU AS O ON K.YIL = O.YIL AND O.SENARYOKOD = K.SENARYOKOD AND K.PEBKOD = O.PEBKOD ";
            yeniEskiProjeEk += "WHERE (P.BASLANGICYILI < {1}) AND (P.ESKIYENI = 1) AND (K.YIL = {1}) AND P.CARI <> 1 ";
            yeniEskiProjeEk += "AND O.SENARYOKOD='{2}' AND O.KURKOD LIKE '{0}%'";
            sql[4] = string.Format(yeniEskiProjeEk, kurKod.Replace(".", ""), yil, senaryoKod.Replace(".", ""));

            string devamEdenProje = "SELECT '{0}' AS KURKOD, SUM(O.ODENEK) FROM YATPROJE AS P INNER JOIN ";
            devamEdenProje += "YATALTPROJE AS AP ON P.YIL = AP.YIL AND P.SENARYOKOD = AP.SENARYOKOD AND P.PROJENO = AP.PROJENO INNER JOIN ";
            devamEdenProje += "YATBUTCEKAYNAK AS KAY ON KAY.YIL = AP.YIL AND KAY.SENARYOKOD = AP.SENARYOKOD AND KAY.ALTPROJENO = AP.ALTPROJENO INNER JOIN ";
            devamEdenProje += "YATBUTCEKAYNAKPEBKOD AS K ON K.YIL = KAY.YIL AND K.SENARYOKOD = KAY.SENARYOKOD AND K.KAYNAKNO = KAY.KAYNAKNO INNER JOIN ";
            devamEdenProje += "ButceHazirlikV2.dbo.BHMODENEK AS O ON K.YIL = O.YIL AND O.SENARYOKOD = K.SENARYOKOD AND K.PEBKOD = O.PEBKOD ";
            devamEdenProje += "WHERE (P.BASLANGICYILI < {1}) AND (P.ESKIYENI = 0) AND (K.YIL = {1}) AND P.CARI <> 1 ";
            devamEdenProje += "AND O.SENARYOKOD={2} AND  O.TEKLIFYIL = 0 AND O.KURKOD LIKE '{0}%'";
            sql[5] = string.Format(devamEdenProje, kurKod.Replace(".", ""), yil, senaryoKod.Replace(".", ""));

            string devamEdenProjeEk = "SELECT '{0}' AS KURKOD, SUM(O.ODENEK) FROM YATPROJE AS P INNER JOIN ";
            devamEdenProjeEk += "YATALTPROJE AS AP ON P.YIL = AP.YIL AND P.SENARYOKOD = AP.SENARYOKOD AND P.PROJENO = AP.PROJENO INNER JOIN ";
            devamEdenProjeEk += "YATBUTCEKAYNAK AS KAY ON KAY.YIL = AP.YIL AND KAY.SENARYOKOD = AP.SENARYOKOD AND KAY.ALTPROJENO = AP.ALTPROJENO INNER JOIN ";
            devamEdenProjeEk += "YATBUTCEKAYNAKPEBKOD AS K ON K.YIL = KAY.YIL AND K.SENARYOKOD = KAY.SENARYOKOD AND K.KAYNAKNO = KAY.KAYNAKNO INNER JOIN ";
            devamEdenProjeEk += "ButceHazirlikV2.dbo.BHMTAVANUSTU AS O ON K.YIL = O.YIL AND O.SENARYOKOD = K.SENARYOKOD AND K.PEBKOD = O.PEBKOD ";
            devamEdenProjeEk += "WHERE (P.BASLANGICYILI < {1}) AND (P.ESKIYENI = 0) AND (K.YIL = {1}) AND P.CARI <> 1 ";
            devamEdenProjeEk += "AND O.SENARYOKOD={2} AND O.KURKOD LIKE '{0}%'";
            sql[6] = string.Format(devamEdenProjeEk, kurKod.Replace(".", ""), yil, senaryoKod.Replace(".", ""));

            string odenekToplam = "SELECT '{0}', SUM(ODENEK) FROM ButceHazirlikV2.dbo.BHMODENEK WHERE YIL={1} AND SENARYOKOD='{2}' AND TEKLIFYIL=0 AND KURKOD LIKE '{0}%'";
            sql[7] = string.Format(odenekToplam, kurKod.Replace(".", ""), yil, senaryoKod.Replace(".", ""));

            string odenekToplamEk = "SELECT '{0}', SUM(ODENEK) FROM ButceHazirlikV2.dbo.BHMTAVANUSTU WHERE YIL={1} AND SENARYOKOD='{2}' AND KURKOD LIKE '{0}%'";
            sql[8] = string.Format(odenekToplamEk, kurKod.Replace(".", ""), yil, senaryoKod.Replace(".", ""));

            sql[9] = string.Format(odenekToplam, kurKod.Replace(".", ""), yil - 1, "0600");

            for (int i = 0; i < 10; i++)
                bag[i] = "BaglantiSatiriYAT";

            TNS.UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

            Tablo XLS = GenelIslemler.NewTablo();
            string sonucDosyaAd = DosyaIslem.DosyaAdUret("", "." + XLS.UzantiBul());
            string sablonAd = raporSablonYol + @"/YAT/YATProjeOzet.xltx";
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("ButceYil", yil);
            XLS.HucreAdBulYaz("AsamaAd", senaryoAd);
            XLS.HucreAdBulYaz("BirimAd", kurKod + " - " + servisUZY.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurKod, true));
            int satir = 5;
            int sutun = 2;
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

            SortedList sl = servisUZY.SorguBirlestir(sql, bag, new int[] { 0 });
            double tavanDisiProjeTutar = 0;
            foreach (DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                XLS.HucreDegerYaz(satir, sutun, alVeri.DblAl(0)); //Yeni Proje Tavan İçi
                XLS.HucreDegerYaz(satir++, sutun + 1, alVeri.DblAl(1)); //Yeni Proje Tavan İçi Ek

                XLS.HucreDegerYaz(satir++, sutun + 1, tavanDisiProjeTutar = alVeri.DblAl(2)); //Yeni Proje Tavan Dışı Ek

                XLS.HucreDegerYaz(satir, sutun, alVeri.DblAl(3) + alVeri.DblAl(5)); //Devam Eden Projeler Tavan İçi
                XLS.HucreDegerYaz(satir++, sutun + 1, alVeri.DblAl(4) + alVeri.DblAl(6)); //Devam Eden Projeler Ek

                satir += 2;

                XLS.HucreDegerYaz(satir, sutun, alVeri.DblAl(7)); //Yeni Proje Tavan İçi
                XLS.HucreDegerYaz(satir++, sutun + 1, alVeri.DblAl(8)); //Yeni Proje Tavan İçi Ek

                XLS.HucreDegerYaz(satir, sutun, alVeri.DblAl(9)); //Yeni Proje Tavan İçi
                break;
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public static string ListeYazdirTablo(IYATServis servis, Kullanici kullanan, int yil, string kurKod, string sektorKod, string senaryoKod, string senaryoAd, int ek, string raporSablonYol)
        {
            AltProje apSor = new AltProje();
            apSor.birim_kod = kurKod;

            BilgiVAN projeler = servis.AltProjeListele(kullanan, yil, senaryoKod, apSor, sektorKod, true, true);
            if (!projeler.sonuc.islemSonuc)
                throw new Exception(projeler.sonuc.hataStr);
            AltProjeTeklif form = new AltProjeTeklif();
            form.yil = yil;
            form.senaryo = senaryoKod;
            form.birim = kurKod;
            form.sektorNo = sektorKod;

            ObjectArray yeniler = servis.AltProjeListeleYeni(kullanan, form);
            if (!yeniler.sonuc.islemSonuc)
                throw new Exception(yeniler.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            string sonucDosyaAd = DosyaIslem.DosyaAdUret("", "." + XLS.UzantiBul());
            string sablonAd = raporSablonYol + @"/YAT/YATProjeListe.xltx";
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("KurumAd", KurumAd());
            XLS.HucreAdBulYaz("AsamaAd", senaryoAd);
            XLS.HucreAdBulYaz("ButceYil", yil);

            TNS.UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
            int satir = 0;
            int sutun = 0;
            int fsatir = 0;
            int fsutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            XLS.HucreAdAdresCoz("FormatSatir", ref fsatir, ref fsutun);
            foreach (AltProje item in projeler.liste)
            {
                if ((ek == 0 && (item.projeTutariHibeli > 0 || item.yiliTeklifi > 0 || item.yiliTeklifiEk > 0))
                    || (ek == 1 && item.yiliTeklifiEk > 0))
                {
                    TabloyaSatirYaz(XLS, satir, sutun, fsatir, item.birim.kod, "0", item.birim.ad, item.ad, AltProjeSektorBul(servis, kullanan, yil, item.kod, senaryoKod), item.baslangicYil, item.bitisYil, item.projeTutariHibesiz, item.yiliTeklifi, item.yiliTeklifiEk, item.gerekce);
                    satir++;
                }
            }
            foreach (AltProjeTeklif item in yeniler.objeler)
            {
                TabloyaSatirYaz(XLS, satir, sutun, fsatir, item.birim, "1", BirimAdBul(servisUZY, kullanan, yil, item.birim), item.ad, SektorAdBul(servisUZY, kullanan, yil, item.sektorNo), item.baslangicYil, item.bitisYil, item.projeTutari, 0, item.yiliTeklifi0, item.gerekce);
                satir++;
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private static void TabloyaSatirYaz(Tablo XLS, int satir, int sutun, int fsatir, string BirimKod, string YeniEski,
                string Birim, string Ad, string Sektor, int BasYil, int BitYil, double ProjeTutar, double YiliTeklif, double EkTeklif, string Gerekce)
        {
            XLS.HucreKopyala(fsatir, sutun, fsatir, sutun + 11, satir, sutun);

            XLS.HucreDegerYaz(satir, sutun + 0, YeniEski == "0" ? "Devam E." : "Yeni");
            XLS.HucreDegerYaz(satir, sutun + 1, BirimKod);
            XLS.HucreDegerYaz(satir, sutun + 2, Birim);
            XLS.HucreDegerYaz(satir, sutun + 3, Ad);
            XLS.HucreDegerYaz(satir, sutun + 4, Sektor);
            XLS.HucreDegerYaz(satir, sutun + 5, BasYil);
            XLS.HucreDegerYaz(satir, sutun + 6, BitYil);
            XLS.HucreDegerYaz(satir, sutun + 7, ProjeTutar);
            XLS.HucreDegerYaz(satir, sutun + 8, YiliTeklif);
            XLS.HucreDegerYaz(satir, sutun + 9, EkTeklif);
            XLS.HucreDegerYaz(satir, sutun + 10, Gerekce);
            XLS.SatirYukseklikAyarla(satir, satir, -1, 3, 25);
        }

        public static string ListeYazdir(IYATServis servis, Kullanici kullanan, int yil, string kurKod, string sektorKod, string senaryoKod, string senaryoAd, int ek, string raporSablonYol)
        {
            AltProje apSor = new AltProje();
            apSor.birim_kod = kurKod;

            BilgiVAN projeler = servis.AltProjeListele(kullanan, yil, senaryoKod, apSor, sektorKod, true, true);
            if (!projeler.sonuc.islemSonuc)
                throw new Exception(projeler.sonuc.hataStr);
            AltProjeTeklif form = new AltProjeTeklif();
            form.yil = yil;
            form.senaryo = senaryoKod;
            form.birim = kurKod;
            form.sektorNo = sektorKod;

            ObjectArray yeniler = servis.AltProjeListeleYeni(kullanan, form);
            if (!yeniler.sonuc.islemSonuc)
                throw new Exception(yeniler.sonuc.hataStr);

            Aspose.Words.Document doc = new Aspose.Words.Document(raporSablonYol + @"/YAT/YATGerekce.docx");
            DataTable tabHeader = new DataTable();
            tabHeader.TableName = "Baslik";
            tabHeader.Columns.Add("Kurum");
            tabHeader.Columns.Add("SenAd");

            tabHeader.Rows.Add(KurumAd(), senaryoAd);
            doc.MailMerge.ExecuteWithRegions(tabHeader);

            DataTable tabListe = new DataTable();
            tabListe.TableName = "Liste";
            tabListe.Columns.Add("Yil");
            tabListe.Columns.Add("BirimKod");
            tabListe.Columns.Add("YeniEski");
            tabListe.Columns.Add("Birim");
            tabListe.Columns.Add("Ad");
            tabListe.Columns.Add("Sektor");
            tabListe.Columns.Add("BasYil");
            tabListe.Columns.Add("BitYil");
            tabListe.Columns.Add("ProjeTutar");
            tabListe.Columns.Add("YiliTeklif");
            tabListe.Columns.Add("EkTeklif");
            tabListe.Columns.Add("Gerekce");
            TNS.UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
            foreach (AltProje item in projeler.liste)
            {
                if ((ek == 0 && (item.projeTutariHibeli > 0 || item.yiliTeklifi > 0 || item.yiliTeklifiEk > 0))
                    || (ek == 1 && item.yiliTeklifiEk > 0))
                {
                    tabListe.Rows.Add(yil.ToString(), item.birim.kod, "0", item.birim.ad, item.ad, AltProjeSektorBul(servis, kullanan, yil, item.kod, senaryoKod), item.baslangicYil.ToString(), item.bitisYil.ToString(), item.projeTutariHibesiz.ToString("#,##0"), item.yiliTeklifi.ToString("#,##0"), item.yiliTeklifiEk.ToString("#,##0"), item.gerekce);
                }
            }
            foreach (AltProjeTeklif item in yeniler.objeler)
            {
                tabListe.Rows.Add(yil.ToString(), item.birim, "1", BirimAdBul(servisUZY, kullanan, yil, item.birim), item.ad, SektorAdBul(servisUZY, kullanan, yil, item.sektorNo), item.baslangicYil.ToString(), item.bitisYil.ToString(), item.projeTutari.ToString("#,##0"), "YENİ", item.yiliTeklifi0.ToString("#,##0"), item.gerekce);
            }

            tabListe.DefaultView.Sort = "BirimKod,YeniEski";
            doc.MailMerge.ExecuteWithRegions(tabListe.DefaultView.ToTable());

            string ciktiAd = DosyaIslem.DosyaAdUret("", "") + ".docx";
            doc.Save(ciktiAd);
            return ciktiAd;
        }

        private static string AltProjeSektorBul(IYATServis servis, Kullanici kullanan, int yil, int altProjeNo, string senaryoKod)
        {
            Proje proje = new Proje();
            proje.iliskiliAltProje = altProjeNo;
            BilgiVAN projeler = servis.ProjeListele(kullanan, yil, senaryoKod, proje, "", false);
            if (projeler.sonuc.islemSonuc && projeler.liste.Count == 1)
            {
                Proje p = (Proje)projeler.liste[0];
                return p.sektor.ad;
            }
            return "";
        }

        private static string SektorAdBul(TNS.UZY.IUZYServis servisUZY, Kullanici kullanan, int yil, string sektorNo)
        {
            return servisUZY.UzayDegeriStr(kullanan, "ABSSEKTOR", yil.ToString() + "-" + sektorNo, true);
        }

        private static string BirimAdBul(TNS.UZY.IUZYServis servisUZY, Kullanici kullanan, int yil, string kurKod)
        {
            return servisUZY.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurKod, true);
        }
    }
}
