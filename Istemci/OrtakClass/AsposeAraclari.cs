using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using TNS;

// Eski OrtakClass'tan alındı: Aspose.Words yardımcıları (Genel.cs) ve geçici dosya adı üretme (DosyaIslem.cs).
namespace OrtakClass
{
    public static class GenelIslemlerIstemci
    {
        public static void AsposeAlanaDocEkle(Aspose.Words.DocumentBuilder builder, string alanAd, string dosyaAd)
        {
            if (builder.MoveToMergeField(alanAd))
            {
                AsposeBuilderaDocEkle(builder, dosyaAd);
                // The paragraph that contained the merge field might be empty now and you probably want to delete it.
                if (!builder.CurrentParagraph.HasChildNodes)
                    builder.CurrentParagraph.Remove();
            }
        }

        public static void AsposeBuilderaDocEkle(Aspose.Words.DocumentBuilder builder, string dosyaAd)
        {
            Aspose.Words.Document srcDoc = new Aspose.Words.Document(dosyaAd);

            Aspose.Words.Node insertAfterNode = builder.CurrentParagraph;
            // We will be inserting into the parent of the destination paragraph.
            Aspose.Words.CompositeNode dstStory = insertAfterNode.ParentNode;

            // This object will be translating styles and lists during the import.
            Aspose.Words.NodeImporter importer = new Aspose.Words.NodeImporter(srcDoc, insertAfterNode.Document, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            // Loop through all sections in the source document.
            foreach (Aspose.Words.Section srcSection in srcDoc.Sections)
            {
                // Loop through all block level nodes (paragraphs and tables) in the body of the section.
                foreach (Aspose.Words.Node srcNode in srcSection.Body)
                {
                    // Let's skip the node if it is a last empty paragraph in a section.
                    if (srcNode.NodeType.Equals(Aspose.Words.NodeType.Paragraph))
                    {
                        Aspose.Words.Paragraph para = (Aspose.Words.Paragraph)srcNode;
                        if (para.IsEndOfSection && !para.HasChildNodes)
                            continue;
                    }

                    // This creates a clone of the node, suitable for insertion into the destination document.
                    Aspose.Words.Node newNode = importer.ImportNode(srcNode, true);

                    // Insert new node after the reference node.
                    dstStory.InsertAfter(newNode, insertAfterNode);
                    insertAfterNode = newNode;
                }
            }
        }

        public static void AsposeAlanaYaz(Aspose.Words.DocumentBuilder builder, string alanAd, string deger)
        {
            if (deger == null) deger = "";
            while (builder.MoveToMergeField(alanAd))
                builder.Write(deger);
        }

        /// <summary>Varsayılan kurum kodu (eski GenelIslemlerIstemci.VarsayilanKurumBul: /KURUMKOD).</summary>
        public static string VarsayilanKurumBul()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMKOD");
        }

        /// <summary>Varsayılan kurum adı (eski GenelIslemlerIstemci.VarsayilanKurumBulAd: /KURUMAD).</summary>
        public static string VarsayilanKurumBulAd()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMAD");
        }

        /// <summary>Personel organizasyon şemasından mı izleniyor (eski GenelIslemlerIstemci.OrgSemaPersonelKullaniyorMu: /OrgSemaPersonel).</summary>
        public static bool OrgSemaPersonelKullaniyorMu()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "/OrgSemaPersonel"), 0) > 0;
        }
    }

    /// <summary>Eski OrtakClass\GenelIslemler.cs'ten: Excel tablosu oluşturma ve birim çevirme yardımcıları.</summary>
    public static class GenelIslemler
    {
        public static int JexcelBirimtoExcelBirimCol(int uzunluk)
        {
            if (ExcelTur() == "xlsx")
                return uzunluk * 900;
            else
                return uzunluk;
        }

        public static int JexcelBirimtoExcelBirim(int uzunluk)
        {
            if (ExcelTur() == "xlsx")
                return uzunluk * 53;
            else
                return uzunluk;
        }

        /// <summary>Ayın adı (1-12); eski OrtakClassRes kaynağındaki ORTAYnn metinleri.</summary>
        public static string AyAdDondur(int ay)
        {
            if (ay < 1 || ay > 12)
                return "";
            else
                return Istemci.Metin.Ortak("ORTAY" + ay.ToString("00"), "");
        }

        public static string ExcelTur()
        {
            return "xlsx";
        }

        /// <summary>Word'ün açtığı HTML raporlarda (.doc) yatay sayfa biçimi (eski GenelIslemler.HTMLWordLandScapeSytle).</summary>
        public static void HTMLWordLandScapeSytle(TextWriter s)
        {
            s.Write("\r\n<style>\r\n");
            s.Write("<!--\r\n");
            s.Write("p.MsoNormal, li.MsoNormal, div.MsoNormal\r\n");
            s.Write("{mso-style-parent:'';\r\n");
            s.Write("margin:0cm;\r\n");
            s.Write("margin-bottom:.0001pt;\r\n");
            s.Write("mso-pagination:widow-orphan;\r\n");
            s.Write("font-size:12.0pt;\r\n");
            s.Write("font-family:'Times New Roman';\r\n");
            s.Write("mso-fareast-font-family:'Times New Roman';}\r\n");
            s.Write("@page Section1\r\n");
            s.Write("{size:841.9pt 595.3pt;\r\n");
            s.Write("mso-page-orientation:landscape;\r\n");
            s.Write("margin:70.9pt 70.9pt 70.9pt 70.9pt;\r\n");
            s.Write("mso-header-margin:35.45pt;\r\n");
            s.Write("mso-footer-margin:35.45pt;\r\n");
            s.Write("mso-paper-source:0;}\r\n");
            s.Write("div.Section1\r\n");
            s.Write("{page:Section1;}\r\n");
            s.Write("-->\r\n");
            s.Write("</style>\r\n");
        }

        /// <summary>Eski sürümde xls için TabloJ (JExcel) de vardı; ExcelTur hep xlsx olduğundan yalnızca TabloAspose taşındı.</summary>
        public static Tablo NewTablo()
        {
            return new TabloAspose();
        }

        /// <summary>Sütun ve satırdan (0'dan başlayan) Excel hücre adresi, ör. (1, 11) -> "B12" (eski jxl.CellReferenceHelper).</summary>
        public static string ExcelAdres(int sutun, int satir)
        {
            return Aspose.Cells.CellsHelper.CellIndexToName(satir, sutun);
        }

        /// <summary>
        /// Bütçe kodunun adı (eski GenelIslemler.KodAd; burada bütçe kod türleri 1-6, 53 kurumsal yapı ve 58 alt faaliyet var).
        /// kosul "yıl-kod" biçimindedir; 53'te yalnızca kurumsal yapı kodu.
        /// </summary>
        public static string KodAd(int kodTur, string kosul, bool esitle)
        {
            string uzayAd;
            if (kodTur == 53)
            {
                // Eski: dört parçalı kodda birimin adına üst birimin adı parantez içinde eklenir.
                TNS.UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
                string[] kosulList = (kosul ?? "").Split('.');
                if (kosulList.Length == 4)
                    return servisUZY.UzayDegeriStr(new TNS.KYM.Kullanici(), "BHMKURUMSALYAPI", kosul, esitle, null) + "(" + servisUZY.UzayDegeriStr(new TNS.KYM.Kullanici(), "BHMKURUMSALYAPI", kosulList[0] + "." + kosulList[1] + "." + kosulList[2], esitle, null) + ")";
                return servisUZY.UzayDegeriStr(new TNS.KYM.Kullanici(), "BHMKURUMSALYAPI", kosul, esitle, null);
            }
            if (kodTur == 1) uzayAd = "ABSKUR";
            else if (kodTur == 2) uzayAd = "ABSFON";
            else if (kodTur == 3) uzayAd = "ABSFIN";
            else if (kodTur == 4) uzayAd = "ABSEKO";
            else if (kodTur == 5) uzayAd = "ABSGELIRKOD";
            else if (kodTur == 6) uzayAd = "ABSFINANSMANKOD";
            else if (kodTur == 58) uzayAd = "ABSPEBKOD";
            else return "";
            string ad = TNS.UZY.Arac.Tanimla().UzayDegeriStr(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle, null);
            return (ad ?? "").Replace(",", " ");
        }

        /// <summary>Değişim oranı (yüzde; eski GenelIslemler.DegisimOranBul): eski 0 ise -1.</summary>
        public static double DegisimOranBul(double yeni, double eski)
        {
            return YuzdeBul(yeni - eski, eski);
        }

        /// <summary>Miktarın tümdeki yüzdesi (eski GenelIslemler.YuzdeBul): tüm 0 ise -1.</summary>
        public static double YuzdeBul(double miktar, double tum)
        {
            if (tum == 0) return -1;
            else return ((miktar / tum) * 100);
        }

        /// <summary>Kullanıcı değişkeninin ilk değeri (eski GenelIslemler.KullaniciDegiskenGetir); yoksa boş.</summary>
        public static string KullaniciDegiskenGetir(string kullaniciKodu, string degiskenAd)
        {
            if (string.IsNullOrEmpty(kullaniciKodu)) return "";
            try
            {
                TNSCollection kDeg = TNS.KYM.Arac.Tanimla().DegiskenleriListele(kullaniciKodu, degiskenAd);
                if (kDeg != null && kDeg.Count > 0 && kDeg[0] != null) return kDeg[0].ToString();
            }
            catch (Exception)
            {
            }
            return "";
        }

        /// <summary>Kullanıcı değişkenini tek değerle saklar (eski GenelIslemler.KullaniciDegiskenSakla).</summary>
        public static Sonuc KullaniciDegiskenSakla(TNS.KYM.Kullanici kullanici, string degiskenAd, string deger)
        {
            if (kullanici == null) return new Sonuc(false, "Kullanıcı yok", "");
            TNSCollection degListesi = new TNSCollection();
            degListesi.Add(deger);
            return kullanici.DegiskenleriSakla(TNS.KYM.Arac.Tanimla(), degiskenAd, degListesi);
        }
    }

    public static class DosyaIslem
    {
        /// <summary>
        /// Sunucuda üretilen dosyayı indirilecek yanıt olarak döndürür (eski DosyaGonder / DosyaGonderX). gidenAd uzantısızsa
        /// uzanti eklenir; sil true ise sunucudaki geçici dosya silinir.
        /// </summary>
        public static IResult DosyaGonder(string dosyaAd, string gidenAd, bool sil, string uzanti)
        {
            byte[] icerik = File.ReadAllBytes(dosyaAd);
            if (sil)
            {
                try { File.Delete(dosyaAd); } catch { }
            }
            uzanti = (uzanti ?? "").TrimStart('.');
            if (uzanti != "" && !gidenAd.EndsWith("." + uzanti, StringComparison.OrdinalIgnoreCase)) gidenAd += "." + uzanti;
            string tur = uzanti == "xlsx" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                       : uzanti == "docx" ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                       : uzanti == "pdf" ? "application/pdf"
                       : uzanti == "doc" ? "application/msword" : "application/octet-stream";
            return Results.File(icerik, tur, gidenAd);
        }

        public static string DosyaAdUret()
        {
            return DosyaAdUret("", "");
        }

        public static string DosyaAdUret(string dosyaAdOnEk, string dosyaAdArkaEk)
        {
            return Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), dosyaAdOnEk + DosyaAdUretSade() + dosyaAdArkaEk);
        }

        public static string DosyaAdUretSade()
        {
            DateTime simdi = System.DateTime.Now;
            return OrtakFonksiyonlar.MetinYap(simdi.Year, 4) + OrtakFonksiyonlar.MetinYap(simdi.Month, 2) + OrtakFonksiyonlar.MetinYap(simdi.Day, 2) + OrtakFonksiyonlar.MetinYap(simdi.Hour, 2) + OrtakFonksiyonlar.MetinYap(simdi.Minute, 2) + OrtakFonksiyonlar.MetinYap(simdi.Second, 2) + OrtakFonksiyonlar.MetinYap(System.DateTime.Now.Millisecond, 4);
        }
    }

    /// <summary>Aspose lisansı (eski Global.asax): Istemci derlemesine gömülü Aspose.Total.lic bir kez yüklenir.</summary>
    public static class AsposeLisans
    {
        private static bool yuklendi;
        private static readonly object kilit = new object();

        public static void Yukle()
        {
            if (yuklendi) return;
            lock (kilit)
            {
                if (yuklendi) return;
                foreach (Action<Stream> ata in new Action<Stream>[] { s => new Aspose.Words.License().SetLicense(s), s => new Aspose.Cells.License().SetLicense(s) })
                    using (Stream s = typeof(AsposeLisans).Assembly.GetManifestResourceStream("Istemci.Properties.Aspose.Total.lic"))
                        if (s != null) ata(s);
                yuklendi = true;
            }
        }
    }
}
