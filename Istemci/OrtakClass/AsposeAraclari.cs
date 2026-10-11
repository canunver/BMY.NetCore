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

        /// <summary>Eski sürümde xls için TabloJ (JExcel) de vardı; ExcelTur hep xlsx olduğundan yalnızca TabloAspose taşındı.</summary>
        public static Tablo NewTablo()
        {
            return new TabloAspose();
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
                       : uzanti == "pdf" ? "application/pdf" : "application/octet-stream";
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
