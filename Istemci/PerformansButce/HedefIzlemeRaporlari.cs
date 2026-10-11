using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Aspose.Words;
using OrtakClass;
using TNS;
using TNS.BKD;
using TNS.KYM;
using TNS.PEB;
using TNS.UZY;

// Eski PerformansButce\GostergeGerceklesmeRaporlari.cs dosyasından alındı: stratejik plan hedef izleme / değerlendirme
// raporları (Word; şablonlar RaporSablon\PEB\StrHedef*.dotx).
namespace PerformansButce
{
    public class HedefIzlemeRaporlari
    {
        TNS.PEB.IPEBServis servisPEB;
        TNS.UZY.IUZYServis servisUZY;
        int donemBYil, hedefKod;
        string kurumKod;
        Kullanici kullanan;
        int yil;
        int donem;
        string sablonAd;
        string sonucDosyaAd, uzanti;
        public string RaporAl(object o_kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {
            kullanan = (Kullanici)o_kullanan;
            donemBYil = OrtakFonksiyonlar.ConvertToInt(raporParam["donemBYil"], -1);
            hedefKod = OrtakFonksiyonlar.ConvertToInt(raporParam["hedefKod"], -1);
            kurumKod = (string)raporParam["kurumKod"];
            yil = OrtakFonksiyonlar.ConvertToInt(raporParam["yil"], -1);
            donem = OrtakFonksiyonlar.ConvertToInt(raporParam["donem"], -1);
            this.sablonAd = sablonAd;
            this.sonucDosyaAd = sonucDosyaAd;
            this.uzanti = ciktiYeri;
            servisPEB = TNS.PEB.Arac.Tanimla();
            servisUZY = TNS.UZY.Arac.Tanimla();
            if (ekBilgi == "1" || ekBilgi == "2")
                return RaporAl1();
            else
                return RaporAl2(ekBilgi);
        }

        private string RaporAl2(string tur)
        {
            Misyon m = StratejikPlanAna.KurumDonemBul(servisPEB, kullanan, kurumKod, donemBYil);

            TNS.PEB.StratejikHedef hedef = new TNS.PEB.StratejikHedef();
            hedef.donemBYil = donemBYil;
            hedef.dil.kod = 1;
            hedef.kurumKod = kurumKod;

            ObjectArray bilgiler = servisPEB.StratejikHedefListele(kullanan, hedef, true);
            if (!bilgiler.sonuc.islemSonuc)
            {
                throw new Exception("Lütfen hedef seçiniz!");
            }

            if (bilgiler.objeler.Count == 0)
            {
                throw new Exception("Hedef listesi okunamadı!");
            }

            Aspose.Words.Document doc = new Aspose.Words.Document(this.sablonAd);
            Aspose.Words.DocumentBuilder db = new Aspose.Words.DocumentBuilder(doc);

            GenelIslemlerIstemci.AsposeAlanaYaz(db, "kurumAd", servisUZY.UzayDegeriStr(kullanan, "ABSKUR", donemBYil.ToString() + "-" + kurumKod, true));
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "donemBas", m.aralikBasiYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "donemSon", m.aralikSonuYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "raporYil", yil.ToString());

            Aspose.Words.Document docIc = null;
            DocumentBuilder builder = null;
            int ilk = 1;
            PerformansButce.HedefIzlemeRaporlari r = new HedefIzlemeRaporlari();
            Hashtable ht = new Hashtable();
            ht["donemBYil"] = donemBYil.ToString();
            ht["kurumKod"] = kurumKod;
            ht["yil"] = yil;
            ht["donem"] = donem.ToString();

            string sablonYolIc = System.IO.Path.GetDirectoryName(this.sablonAd);
            foreach (TNS.PEB.StratejikHedef item in bilgiler.objeler)
            {
                ht["hedefKod"] = item.strHedefKod.ToString();
                try
                {
                    string dosyaAd = r.RaporAl(kullanan, servisPEB, sablonYolIc + "\\StrHedef" + (tur == "12" ? "v2" : "") + (donem == 4 ? "Degerlendirme" : "Izleme") + ".dotx", sonucDosyaAd, uzanti, ht, "1");
                    if (ilk == 1)
                    {
                        docIc = new Document(dosyaAd);
                        builder = new DocumentBuilder(docIc);
                    }
                    else
                    {
                        builder.MoveToDocumentEnd();
                        //if (ilk == 2)
                        //{
                        //    Aspose.Words.Document srcDoc = new Aspose.Words.Document(dosyaAd);
                        //    doc.AppendDocument(srcDoc, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
                        //    doc.Sections[0].PageSetup.SectionStart = Aspose.Words.SectionStart.NewPage;
                        //    doc.Sections[1].PageSetup.RestartPageNumbering = true;
                        //    doc.Sections[1].PageSetup.PageStartingNumber = 1;
                        //}
                        //else
                        GenelIslemlerIstemci.AsposeBuilderaDocEkle(builder, dosyaAd);
                    }
                    try
                    {
                        System.IO.File.Delete(dosyaAd);
                    }
                    catch { }
                }
                catch { }
                ilk++;
            }
            if (docIc != null)
            {
                string enIc = DosyaIslem.DosyaAdUret("SGG", "") + ".docx";
                docIc.Save(enIc);

                GenelIslemlerIstemci.AsposeAlanaDocEkle(db, "tablolar", enIc);

                try
                {
                    System.IO.File.Delete(enIc);
                }
                catch { }
            }
            else
                GenelIslemlerIstemci.AsposeAlanaYaz(db, "tablolar", "");

            string sonucDosyaYol = sonucDosyaAd + "." + uzanti;

            doc.Save(sonucDosyaYol);
            return sonucDosyaYol;
        }

        private string RaporAl1()
        {
            TNS.PEB.StratejikHedef hedef = new TNS.PEB.StratejikHedef();
            hedef.donemBYil = donemBYil;
            hedef.strHedefKod = hedefKod;
            hedef.dil.kod = 1;
            hedef.kurumKod = kurumKod;

            if (hedef.strHedefKod < 1)
            {
                throw new Exception("Lütfen hedef seçiniz!");
            }

            ObjectArray bilgiler = servisPEB.StratejikHedefListele(kullanan, hedef, true);
            if (!bilgiler.sonuc.islemSonuc)
            {
                throw new Exception("Lütfen hedef seçiniz!");
            }

            if (bilgiler.objeler.Count == 0)
            {
                throw new Exception("Hedef okunamadı!");
            }

            TNS.PEB.StratejikHedef item = (TNS.PEB.StratejikHedef)bilgiler.objeler[0];

            ObjectArray oa1 = servisPEB.ListeleStrHedefGostergeGerc(kullanan, hedef, yil, donem);
            ObjectArray oa2 = servisPEB.ListeleStrHedefGercBilgi(kullanan, hedef, yil, donem, 1);

            Aspose.Words.Document doc = new Aspose.Words.Document(this.sablonAd);
            Aspose.Words.DocumentBuilder db = new Aspose.Words.DocumentBuilder(doc);

            GenelIslemlerIstemci.AsposeAlanaYaz(db, "aK", item.strAmac.strAmacKisa);
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "amac", item.strAmac.strAmac);
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "hK", item.strHedefKisa);
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "hedef", item.strHedef);

            GenelIslemlerIstemci.AsposeAlanaYaz(db, "ilgPrg", StratejikHedefYeni.MetinBul(item.programBagi, 0));
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "iliskiliAprgHedef", StratejikHedefYeni.MetinBul(item.programBagi, 1));

            GenelIslemlerIstemci.AsposeAlanaYaz(db, "performans", StratejikHedefYeni.StrYap(HedefPerformansHesapla(oa1.objeler))); //Performans Bul "(PG1.1.1 Performansı X Hedefe Etkisi) + (PG1.1.2 Performansı X Hedefe Etkisi)"
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "sbirim", StratejikHedefYeni.BirimBul(item.birimler, 0, 1));

            GenelIslemlerIstemci.AsposeAlanaYaz(db, "sapmaNedeni", DiziElemanAl(oa2, 0, ""));
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "alimacakOnlem", DiziElemanAl(oa2, 1, ""));
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "degerlendirmeler", DiziElemanAl(oa2, 2, ""));

            DataTable dt = new DataTable();
            dt.TableName = "veri";

            dt.Columns.Add("pg");
            dt.Columns.Add("deg0");
            dt.Columns.Add("deg1");
            dt.Columns.Add("deg2");
            dt.Columns.Add("deg3");
            dt.Columns.Add("deg4");

            dt.Columns.Add("aciklama");
            dt.Columns.Add("ilgililik");
            dt.Columns.Add("etkililik");
            dt.Columns.Add("etkinlik");
            dt.Columns.Add("surdurulebilirlik");
            dt.Columns.Add("d4N");

            double deg0;
            double deg1;
            double deg2;
            double deg3;
            double gostergePerf;
            bool limitle = TNS.DEG.Arac.DegiskenDegerBul(0, "TBMM") == "1";
            bool birVar = false;
            bool ikiVar = false;
            foreach (StrHedefGosterge gos in oa1.objeler)
            {
                deg0 = gos.hedefeEtkisi;
                deg1 = gos.donemBaslangic;
                deg2 = gos.hedeflenenBirikimliDeger;
                deg3 = gos.birikimliGerceklesme;
                gostergePerf = gos.birikimliPerformans;
                string gostergePerformansNot;
                double gostergePerfLimitli = SifirYuz(gostergePerf, out gostergePerformansNot);
                if (limitle)
                {
                    gostergePerf = gostergePerfLimitli;
                }
                if (gostergePerformansNot == "1") birVar = true;
                if (gostergePerformansNot == "2") ikiVar = true;
                if (gostergePerformansNot != "") gostergePerformansNot = "*";
                int ondalik = gos.gostergeOndalik; //StratejikHedefYeni.OndalikBul(deg0, deg1, deg2, deg3);

                dt.Rows.Add(gos.gorKod + " " + gos.gostergeAd, "%" + StratejikHedefYeni.StrYap(deg0, 0), StratejikHedefYeni.StrYap(deg1, ondalik), StratejikHedefYeni.StrYap(deg2, ondalik), StratejikHedefYeni.StrYap(deg3, ondalik), StratejikHedefYeni.StrYap(gostergePerf, 1), gos.aciklama, gos.ilgililik, gos.etkililik, gos.etkinlik, gos.surdurulebilirlik, gostergePerformansNot);
            }
            if (dt.Rows.Count == 0)
                dt.Rows.Add("", "", "", "", "", "", "", "", "", "", "");
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "dipNotlar", DipnotOl(birVar, ikiVar));
            doc.MailMerge.CleanupOptions = Aspose.Words.Reporting.MailMergeCleanupOptions.RemoveEmptyParagraphs;
            doc.MailMerge.ExecuteWithRegions(dt);

            //DataTable dt2 = new DataTable();
            //dt2.TableName = "aciklamalar";
            //dt2.Columns.Add("aciklama");

            //foreach (IntKodStrAd aciklama in oa2.objeler)
            //{
            //    dt2.Rows.Add(aciklama.ad);
            //}
            //if (dt2.Rows.Count == 0)
            //    dt2.Rows.Add("");
            //doc.MailMerge.ExecuteWithRegions(dt2);

            //DataTable dt3 = new DataTable();
            //dt3.TableName = "degerlendirmeler";
            //dt3.Columns.Add("degerlendirme");
            //foreach (IntKodStrAd aciklama in oa3.objeler)
            //{
            //    dt3.Rows.Add(aciklama.ad);
            //}
            //if (dt3.Rows.Count == 0)
            //    dt3.Rows.Add("");
            //doc.MailMerge.ExecuteWithRegions(dt3);

            string sonucDosyaYol = sonucDosyaAd + "." + uzanti;

            doc.Save(sonucDosyaYol);
            return sonucDosyaYol;
        }

        private string DipnotOl(bool birVar, bool ikiVar)
        {
            string cevap = "";
            if (birVar || ikiVar)
            {
                cevap = "*:";
                if (birVar)
                    cevap += " Gösterge performansı negatif olan değerler hedef performansının ölçümünde “0” olarak alınmıştır.";
                if (ikiVar)
                {
                    cevap += " Gösterge performansında 100'ü aşan değerler hedef performansının ölçümünde “100” olarak alınmıştır.";
                }
            }
            return cevap;
        }

        private double HedefPerformansHesapla(TNSCollection gerceklesmeler)
        {
            double deg = 0;
            string gostergePerformansNot;
            foreach (StrHedefGosterge gos in gerceklesmeler)
            {
                deg += SifirYuz(gos.birikimliPerformans, out gostergePerformansNot) * gos.hedefeEtkisi;
            }
            return deg / 100;
        }

        private double SifirYuz(double oran, out string gostergePerformansNot)
        {
            gostergePerformansNot = "";
            if (oran < 0) { gostergePerformansNot = "1"; return 0; }
            if (oran > 100) { gostergePerformansNot = "2"; return 100; }
            return oran;
        }

        public static string DiziElemanAl(ObjectArray oa2, int indis, string def)
        {
            if (indis < oa2.objeler.Count)
            {
                IntKodStrAd iksa = (IntKodStrAd)oa2.Al(indis);
                return iksa.ad;
            }
            return def;
        }
    }
}