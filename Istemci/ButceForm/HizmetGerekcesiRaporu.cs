using System;
using OrtakClass;
using TNS;
using TNS.BHF;
using TNS.KYM;
using TNS.UZY;

namespace ButceForm
{
    /// <summary>
    /// Faaliyet gerekçesi raporu; eski ButceFormRaporlari.HizmetGerekcesiHazirla kodunun aynısıdır (Word'ün açtığı HTML, .doc).
    /// Yalnızca parametreler Hashtable yerine doğrudan alınır ve üretilen dosyanın yolu döndürülür; DocxeCevir ile .docx'e çevrilip gönderilir.
    /// </summary>
    public static class HizmetGerekcesiRaporu
    {
        /// <param name="faalKod">0 ise birimin bütün faaliyetleri yazılır.</param>
        public static string HizmetGerekcesiHazirla(IBHFServis servis, Kullanici kullanan, int yil, string kurKod, int faalKod, string kurumKod, string p_SonucDosyaAd)
        {
            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            string sonucDosyaAd = p_SonucDosyaAd + ".doc";

            System.IO.StreamWriter sw = new System.IO.StreamWriter(sonucDosyaAd, false);
            DosyaBasligiAyarla(sw);
            string birimKodAd = kurKod + " - " + uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurKod, true);
            bool kurumMu = (kurKod.Replace(".", "") == kurumKod.Replace(".", ""));

            ObjectArray faaliyetGerekceleri = servis.FaaliyetGerekceListele(kullanan, yil, kurKod, faalKod);

            if (faaliyetGerekceleri.sonuc.islemSonuc)
            {
                foreach (FaaliyetGerekce fg in faaliyetGerekceleri.objeler)
                {
                    if (!string.IsNullOrEmpty(fg.gerekce))
                    {
                        Yazdir(sw, FaaliyetAdBul(kullanan, yil, fg), fg.gerekce, fg.dayanak, fg.maliyet, yil.ToString(), birimKodAd, kurumMu);
                        sw.Write("<br clear=all style='page-break-before:always'>");
                    }
                }
            }
            else
            {
                sw.Close();
                throw new Exception(faaliyetGerekceleri.sonuc.hataStr);
            }

            sw.Write("</body>");
            sw.Write("</html>");
            sw.Flush();
            sw.Close();

            return sonucDosyaAd;
        }

        /// <summary>
        /// Eski kodun ürettiği HTML (.doc) dosyasını Aspose.Words ile gerçek Word belgesine (.docx) çevirir; sayfalar eskisi gibi yatay.
        /// HTML dosyası silinir, .docx dosyasının yolu döndürülür.
        /// </summary>
        public static string DocxeCevir(string htmlDosyaAd)
        {
            AsposeLisans.Yukle();
            Aspose.Words.LoadOptions yukle = new Aspose.Words.LoadOptions();
            yukle.LoadFormat = Aspose.Words.LoadFormat.Html;
            yukle.Encoding = System.Text.Encoding.UTF8;
            Aspose.Words.Document belge = new Aspose.Words.Document(htmlDosyaAd, yukle);
            foreach (Aspose.Words.Section bolum in belge.Sections)
            {
                bolum.PageSetup.Orientation = Aspose.Words.Orientation.Landscape;
                bolum.PageSetup.PaperSize = Aspose.Words.PaperSize.A4;
            }
            string docx = System.IO.Path.ChangeExtension(htmlDosyaAd, ".docx");
            belge.Save(docx, Aspose.Words.SaveFormat.Docx);
            try { System.IO.File.Delete(htmlDosyaAd); } catch { }
            return docx;
        }

        static void DosyaBasligiAyarla(System.IO.StreamWriter fs)
        {
            fs.Write("<html>");
            fs.Write("<head>");
            fs.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            fs.Write("<meta http-equiv=\"Content-Language\" content=\"tr\">");
            GenelIslemler.HTMLWordLandScapeSytle(fs);
            fs.Write("</head>");
            fs.Write("<body>");
            fs.Write("<div class=Section1>");
            fs.Write("</div>");
        }

        // Eski kodda değer hücreleri "<td>&nbsp;</td>" + değer + "</td>" biçimindeydi; değer tablonun dışına düşüyordu (docx'te
        // yıl, birim ve faaliyet tablonun üstüne çıkıyordu). Burada değer hücrenin içine yazılır.
        private static void Yazdir(System.IO.StreamWriter sw, string faaliyet, string gerekce, string dayanak, string maliyet, string yil, string kurum, bool kurumMu)
        {
            string f;
            if (kurumMu)
                f = "Kurum Adı";
            else
                f = "Birim Adı";

            sw.Write("<p align='center'>&nbsp;</p>");
            sw.Write("<p align='center'><b>FAALİYET GEREKÇESİ VE HEDEFLERİ</b></p>");
            sw.Write("<table border='2' width='100%' id='table1' cellspacing='1' bordercolor='#000000' style='border-collapse: collapse'>");
            sw.Write("<tr>");
            sw.Write("<td style='border-left-style: solid; border-left-width: 1px; border-top-style: solid; border-top-width: 1px' width='105'>");
            sw.Write("Yılı</td>");
            sw.Write("<td style='border-right-style: solid; border-right-width: 1px; border-top-style: solid; border-top-width: 1px'>");
            sw.Write(yil + "</td>");
            sw.Write("</tr>");
            sw.Write("<tr>");
            sw.Write("<td style='border-left-style: solid; border-left-width: 1px; border-bottom-style: solid; border-bottom-width: 1px' width='105'>");
            sw.Write(f + "</td>");
            sw.Write("<td style='border-right-style: solid; border-right-width: 1px; border-bottom-style: solid; border-bottom-width: 1px'>");
            sw.Write(kurum + "</td>");
            sw.Write("</tr>");
            sw.Write("<tr>");
            sw.Write("<td style='border-left-style: solid; border-left-width: 1px' width='105'>");
            sw.Write("Faaliyet</td>");
            sw.Write("<td style='border-right-style: solid; border-right-width: 1px'>");
            sw.Write(faaliyet + "</td>");
            sw.Write("</tr>");
            sw.Write("</table>");
            sw.Write("<p style='margin-top: 0; margin-bottom: 0'>&nbsp;</p>");
            sw.Write("<table border='2' cellSpacing=2 cellPadding=2 width='100%' bordercolor='#000000' id='table2' style='border-collapse: collapse'>");
            sw.Write("<tr>");
            sw.Write("<td>" + gerekce + "</td>");
            sw.Write("</tr>");
            sw.Write("<tr>");
            sw.Write("<td>" + dayanak + "</td>");
            sw.Write("</tr>");
            sw.Write("<tr>");
            sw.Write("<td>" + maliyet + "</td>");
            sw.Write("</tr>");
            sw.Write("</table>");
        }

        private static string FaaliyetAdBul(Kullanici kullanan, int yil, FaaliyetGerekce fg)
        {
            TNS.BKD.IBKDServis muhServis = TNS.BKD.Arac.Tanimla();
            TNS.BKD.PrgKodYapi kriter = new TNS.BKD.PrgKodYapi();
            kriter.yil = yil;
            kriter.kodTur = (int)TNS.BKD.EnumPrgKodYapi.FAALIYET;
            kriter.kod = fg.faalKod;
            ObjectArray bilgiler = muhServis.PrgKodYapiListele(kullanan, kriter);

            if (bilgiler.sonuc.islemSonuc && bilgiler.ObjeSayisi > 0)
            {
                return fg.faalKod + "-" + ((TNS.BKD.PrgKodYapi)bilgiler.objeler[0]).ad;
            }
            return "";
        }
    }
}
