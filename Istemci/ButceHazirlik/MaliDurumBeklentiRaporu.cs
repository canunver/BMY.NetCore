using System;
using System.Data;
using System.IO;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;
using TNS.UZY;

namespace Istemci.ButceHazirlik
{
    /// <summary>
    /// Kurumsal Mali Durum ve Beklentiler Raporu (eski ButceHazirlikv2/BHMMaliDurumBeklentiForm.aspx btnYazdirYeni_Click ve
    /// btnYazdir_Click). Eski Aspose.Words adres-mektup kodu ve eski .docx şablonları aynen kullanılır; şablon önce kurumun
    /// RaporSablonYol\BHMv2 klasöründe, yoksa uygulamayla gelen SGB.Net\RaporSablon\BHMv2 klasöründe aranır.
    /// </summary>
    public static class MaliDurumBeklentiRaporu
    {
        /// <summary>Raporu üretir, geçici .docx dosyasının yolunu döndürür; hata olursa null ve hata metni.</summary>
        public static string Yazdir(Kullanici kullanan, int yil, string faaliyetKod, bool eskiBicim, string uygulamaKlasoru, out string hata)
        {
            hata = null;
            string sablonAd = eskiBicim ? "MaliDurumBeklenti.docx" : "MaliDurumBeklentiYeni.docx";
            string sablon = SablonBul(uygulamaKlasoru, sablonAd);
            if (sablon == null)
            {
                hata = "Rapor şablonu bulunamadı (BHMv2\\" + sablonAd + ").";
                return null;
            }

            IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
            MaliDurumBeklenti form = new MaliDurumBeklenti { yil = yil, faaliyetKod = faaliyetKod ?? "" };
            MaliDurumBeklenti[] formlar = servis.ListeleBHMMaliDurumBeklenti(kullanan, form);
            if (eskiBicim)
            {
                if (formlar == null)
                {
                    hata = "Yazdırılacak kayıt bulunamadı!";
                    return null;
                }
                return EskiRapor(kullanan, form, formlar, sablon);
            }
            return YeniRapor(kullanan, servis, form, formlar ?? new MaliDurumBeklenti[0], sablon);
        }

        private static string SablonBul(string uygulamaKlasoru, string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "BHMv2", sablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "BHMv2", sablonAd);
            return File.Exists(u) ? u : null;
        }

        /// <summary>Eski btnYazdir_Click (eski sayfada gizliydi): faaliyet başına dört başlık ve metin.</summary>
        private static string EskiRapor(Kullanici kullanan, MaliDurumBeklenti form, MaliDurumBeklenti[] formlar, string sablon)
        {
            string[] basliklar = {
                    string.Format("I – Ocak - Haziran {0} Dönemi Bütçe Uygulama Sonuçları", form.yil),
                    string.Format("II – Ocak - Haziran {0} Döneminde Yürütülen Faaliyetler", form.yil),
                    string.Format("III – Temmuz - Aralık {0} Dönemine İlişkin Beklentiler ve Hedefler", form.yil),
                    string.Format("IV – Temmuz - Aralık {0} Döneminde Yürütülecek Faaliyetler", form.yil) };

            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            Aspose.Words.Document doc = new Aspose.Words.Document(sablon);
            DataTable tabHeader = new DataTable();
            tabHeader.TableName = "Kapak";
            tabHeader.Columns.Add("Yil");
            tabHeader.Rows.Add(form.yil.ToString());
            doc.MailMerge.ExecuteWithRegions(tabHeader);

            DataTable tabBaslik = new DataTable();
            tabBaslik.TableName = "Satirlar";
            tabBaslik.Columns.Add("FaalAd");
            tabBaslik.Columns.Add("Baslik1");
            tabBaslik.Columns.Add("Icerik1");
            tabBaslik.Columns.Add("Baslik2");
            tabBaslik.Columns.Add("Icerik2");
            tabBaslik.Columns.Add("Baslik3");
            tabBaslik.Columns.Add("Icerik3");
            tabBaslik.Columns.Add("Baslik4");
            tabBaslik.Columns.Add("Icerik4");

            for (int i = 0; i < formlar.Length; i++)
            {
                MaliDurumBeklenti f = formlar[i];
                tabBaslik.Rows.Add(uzyServis.UzayDegeriStr(kullanan, "ABSPEBKOD", f.yil + "-" + f.faaliyetKod, true), basliklar[0], f.butceUygulamaSonucu, basliklar[1], f.yurutulenFaaliyetler, basliklar[2], f.beklentilerveHedefler, basliklar[3], f.yurutulecekFaaliyetler);
            }
            doc.MailMerge.ExecuteWithRegions(tabBaslik);
            string ciktiAd = DosyaIslem.DosyaAdUret("MaliDurumBeklenti", "") + ".docx";
            doc.Save(ciktiAd);
            return ciktiAd;
        }

        /// <summary>
        /// Eski btnYazdirYeni_Click: dönem (II, III) / birim / faaliyet kırılımında ödenekler ve metinler; bütçe uygulama sonucu
        /// metni ödenek, yıl içi işlemler ve ilk altı ay harcamasından üretilir.
        /// </summary>
        private static string YeniRapor(Kullanici kullanan, IBHMv2Servis servis, MaliDurumBeklenti form, MaliDurumBeklenti[] formlar, string sablon)
        {
            string[] basliklar = {
                        $"{form.yil} Yılı Ocak-Haziran Dönemi Bütçe Uygulama Sonuçları",
                        $"{form.yil} Yılı Ocak-Haziran Döneminde Yürütülen Faaliyetler",
                        $"{form.yil} Yılı Temmuz-Aralık Dönemine İlişkin Beklentiler ve Hedefler",
                        $"{form.yil} Yılı Temmuz-Aralık Döneminde Yürütülecek Faaliyetler" };

            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            Aspose.Words.Document doc = new Aspose.Words.Document(sablon);

            DataTable tabHeader = new DataTable();
            tabHeader.TableName = "Kapak";
            tabHeader.Columns.Add("Yil");
            tabHeader.Rows.Add(form.yil.ToString());
            doc.MailMerge.ExecuteWithRegions(tabHeader);

            DataTable tabDonemler = YeniTabloOl("Donemler", 1, "Donem", "DonemAd");
            DataTable tabBirimler = YeniTabloOl("Birimler", 2, "Donem", "Birim", "BirimAd");
            DataTable tabOdenekBolum = YeniTabloOl("OdenekBolum", 2, "Donem", "Birim", "BirimAd", "Yil", "Odenek");
            DataTable tabOdenekler = YeniTabloOl("FaalOdenekler", 3, "Donem", "Birim", "FaaliyetKod", "FaalAd", "Odenek");
            DataTable tabSatirlar = YeniTabloOl("Satirlar", 3, "Donem", "Birim", "FaaliyetKod", "FaalAd", "Baslik1", "Icerik1", "Baslik2", "Icerik2");

            DataSet veriler = new DataSet();
            veriler.Tables.Add(tabDonemler);
            veriler.Tables.Add(tabBirimler);
            veriler.Tables.Add(tabOdenekBolum);
            veriler.Tables.Add(tabOdenekler);
            veriler.Tables.Add(tabSatirlar);

            veriler.Relations.Add(new DataColumn[] { tabDonemler.Columns["Donem"] }, new DataColumn[] { tabBirimler.Columns["Donem"] });
            veriler.Relations.Add(new DataColumn[] { tabBirimler.Columns["Donem"], tabBirimler.Columns["Birim"] }, new DataColumn[] { tabOdenekBolum.Columns["Donem"], tabOdenekBolum.Columns["Birim"] });
            veriler.Relations.Add(new DataColumn[] { tabOdenekBolum.Columns["Donem"], tabOdenekBolum.Columns["Birim"] }, new DataColumn[] { tabOdenekler.Columns["Donem"], tabOdenekler.Columns["Birim"] });
            veriler.Relations.Add(new DataColumn[] { tabBirimler.Columns["Donem"], tabBirimler.Columns["Birim"] }, new DataColumn[] { tabSatirlar.Columns["Donem"], tabSatirlar.Columns["Birim"] });

            tabDonemler.Rows.Add("1", $"II- {form.yil} YILI OCAK-HAZİRAN DÖNEMİNDE YÜRÜTÜLEN FAALİYETLER");
            tabDonemler.Rows.Add("2", $"III- {form.yil} YILI TEMMUZ-ARALIK DÖNEMİ BEKLENTİ VE HEDEFLERİ İLE YÜRÜTÜLMESİ PLANLANAN FAALİYETLER");

            Tertip kistas = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", "", form.faaliyetKod);
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(14, kistas.KodNoKirilimSay(4)));
            EkDegerParam[] ekBilgiler = new EkDegerParam[3];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, "06.00");
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABMasrafPlaniForm, 0, 0, ";;" + TNS.BHMv2.Arac.EklemeDusmeIslemler() + ";;");
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABHarcama, 0, 0, "6");

            OdenekBilgi[] a = servis.ListeTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, true) ?? new OdenekBilgi[0];

            string oncBirimKod = "";
            int birimSay = 0;
            DataRow baslikSatir = null;
            double toplam = 0;
            foreach (OdenekBilgi ob in a)
            {
                string birimKod = ob.tertip.kodlar[4].Substring(0, 2);
                if (oncBirimKod != birimKod)
                {
                    if (baslikSatir != null)
                        baslikSatir.SetField(4, StrYap(toplam));
                    birimSay++;
                    string birimAd = uzyServis.UzayDegeriStr(kullanan, "ABSPEBKOD", form.yil + "-" + birimKod, true);
                    if (string.IsNullOrWhiteSpace(birimAd)) birimAd = birimKod;
                    birimAd = birimAd.ToUpper();
                    tabBirimler.Rows.Add("1", birimKod, "II." + birimSay + ". " + birimAd);
                    tabBirimler.Rows.Add("2", birimKod, "III." + birimSay + ". " + birimAd);
                    baslikSatir = tabOdenekBolum.Rows.Add("1", birimKod, birimAd, form.yil.ToString(), "");
                    toplam = 0;
                    oncBirimKod = birimKod;
                }

                double odenek = (double)ob.ekRakamlar[0];
                string faaliyetAd = ob.tertip.adlar[4];
                string faaliyetKod = ob.tertip.kodlar[4];
                tabOdenekler.Rows.Add("1", birimKod, faaliyetKod, faaliyetAd, StrYap(odenek));
                MaliDurumBeklenti f = FaaliyetBul(formlar, faaliyetKod.Replace(".", ""));
                if (f == null) f = new MaliDurumBeklenti();
                f.butceUygulamaSonucu = ButceUygulamaSonucuOl(ob, faaliyetAd, form.yil);
                if (DoluMu(f.butceUygulamaSonucu) || DoluMu(f.yurutulenFaaliyetler))
                    tabSatirlar.Rows.Add("1", birimKod, faaliyetKod, faaliyetAd, MetinVeyaBos(basliklar[0], f.butceUygulamaSonucu), f.butceUygulamaSonucu, MetinVeyaBos(basliklar[1], f.yurutulenFaaliyetler), f.yurutulenFaaliyetler);
                if (DoluMu(f.beklentilerveHedefler) || DoluMu(f.yurutulecekFaaliyetler))
                    tabSatirlar.Rows.Add("2", birimKod, faaliyetKod, faaliyetAd, MetinVeyaBos(basliklar[2], f.beklentilerveHedefler), f.beklentilerveHedefler, MetinVeyaBos(basliklar[3], f.yurutulecekFaaliyetler), f.yurutulecekFaaliyetler);
                toplam += odenek;
            }
            if (baslikSatir != null)
                baslikSatir.SetField(4, StrYap(toplam));

            doc.MailMerge.CleanupOptions = Aspose.Words.Reporting.MailMergeCleanupOptions.RemoveEmptyParagraphs | Aspose.Words.Reporting.MailMergeCleanupOptions.RemoveUnusedRegions;
            doc.MailMerge.ExecuteWithRegions(veriler);
            string ciktiAd = DosyaIslem.DosyaAdUret("MaliDurumBeklenti", "") + ".docx";
            doc.UpdateFields();
            doc.Save(ciktiAd);
            return ciktiAd;
        }

        private static string ButceUygulamaSonucuOl(OdenekBilgi ob, string faaliyetAd, int yil)
        {
            double odenek = (double)ob.ekRakamlar[0];
            double islem = (double)ob.ekRakamlar[1];
            double harcanan = (double)ob.ekRakamlar[2];

            if (islem == 0)
                return $"{yil} yılında {OrtakFonksiyonlar.IlkHarflerBuyuk(faaliyetAd)} faaliyeti için kanun ile {StrYap(odenek)} TL ödenek tahsis edilmiş olup, ilk altı ay itibari ile tahsis edilen ödenekten {HarcanmistirBul(harcanan)}";
            else
                return $"{yil} yılında {OrtakFonksiyonlar.IlkHarflerBuyuk(faaliyetAd)} faaliyeti için kanun ile {StrYap(odenek)} TL ödenek tahsis edilmiş olup, yıl içi işlemlerle {StrYap(odenek + islem)} TL olmuştur. İlk altı ay itibari ile tahsis edilen ödenekten {HarcanmistirBul(harcanan)}";
        }

        private static string HarcanmistirBul(double harcanan)
        {
            if (harcanan <= 0)
                return "harcama yapılmamıştır.";
            else
                return $"{StrYap(harcanan)} TL harcanmıştır.";
        }

        private static MaliDurumBeklenti FaaliyetBul(MaliDurumBeklenti[] formlar, string arananKod)
        {
            foreach (var item in formlar)
            {
                if (item.faaliyetKod == arananKod)
                    return item;
            }
            return null;
        }

        private static string StrYap(double rakam)
        {
            return rakam.ToString("#,##0");
        }

        private static object MetinVeyaBos(string baslik, string veri)
        {
            if (DoluMu(veri))
                return baslik;
            else
                return null;
        }

        private static bool DoluMu(string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        private static DataTable YeniTabloOl(string tabloAd, int pkSay, params string[] alanlar)
        {
            DataTable tab = new DataTable();
            tab.TableName = tabloAd;
            for (int i = 0; i < alanlar.Length; i++)
            {
                tab.Columns.Add(alanlar[i]);
            }

            DataColumn[] pkColums = new DataColumn[pkSay];
            for (int i = 0; i < pkSay; i++)
            {
                pkColums[i] = tab.Columns[alanlar[i]];
            }
            tab.PrimaryKey = pkColums;
            return tab;
        }
    }
}
