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
using System.Drawing;
using Aspose.Words.Tables;
using Aspose.Words.Reporting;
//using Aspose.Words.MailMerging;

namespace ButceHazirlikv2
{
    /// <summary>
    /// DPT yatırım projeleri raporlarını üreten yordamları tutan sınıf.
    /// Sınıfın giriş yordamı olan RaporAl yordamı gelen ek param bilgisine göre ilgili üretim yordamını çağırır.
    /// </summary>
    public class ProjeDosyasiRaporlari
    {
        IYATServis servisYAT = TNS.YAT.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
        Kullanici kullanan = null;
        string sablonAd = string.Empty;
        string sonucDosyaAd = string.Empty;
        string sonucDosyaAdWrd = string.Empty;
        private string sablonYol;
        private int butceYil;
        private string senaryoKod;
        private int projeNo;
        private int altProjeNo;
        private AltProje altProje;
        private Proje proje;
        //        private BilgiVAN altProjeler;
        TNS.UZY.IUZYServis uzyServis = null;
        private string format;
        public Hashtable raporParam { get; set; }

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
        ///
        /// </param>
        /// <returns>Rapor başarı ile üretilmişse üretilen dosya adı, aksi takdirde boşluk döndürür.</returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            this.kullanan = (Kullanici)kullanan;
            this.sablonAd = sablonAd;
            this.sablonYol = System.IO.Path.GetDirectoryName(sablonAd);
            this.sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            this.raporParam = raporParam;
            this.format = null;
            if (raporParam["Format"] != null)
                this.format = raporParam["Format"].ToString();
            if (string.IsNullOrWhiteSpace(this.format))
                this.format = "pdf";
            this.sonucDosyaAdWrd = sonucDosyaAd + "." + this.format;

            this.butceYil = OrtakFonksiyonlar.ConvertToInt(raporParam["ButceYil"], 0);
            this.senaryoKod = raporParam["Senaryo"].ToString();
            //string senaryoAd = raporParam["SenaryoAd"].ToString();

            this.projeNo = OrtakFonksiyonlar.ConvertToInt(raporParam["ProjeNo"], 0);
            this.altProjeNo = OrtakFonksiyonlar.ConvertToInt(raporParam["AltProjeNo"], 0);
            //            this.altProjeler = null;
            try
            {
                if (ekBilgi.Equals("Kapak"))
                    return KapakYazdir();
                else if (ekBilgi.Equals("Ozet"))
                    return OzetYazdir();
                else if (ekBilgi.Equals("Tutar"))
                    return TutarYazdir();
                else if (ekBilgi.Equals("Harc"))
                    return HarcamaYazdir();
                else if (ekBilgi.Equals("Teklif0"))
                    return TeklifYazdir(0);
                else if (ekBilgi.Equals("Teklif1"))
                    return TeklifYazdir(1);
                else if (ekBilgi.Equals("Teklif2"))
                    return TeklifYazdir(2);
                else if (ekBilgi.Equals("TeklifTam"))
                    return TeklifYazdirTam();
                else if (ekBilgi.Equals("Bilgi"))
                    return BilgiYazdir();
                else if (ekBilgi.Equals("KrediHibe1"))
                    return ProjeAltProjeKrediHibeYazdir(1);
                else if (ekBilgi.Equals("KrediHibe2"))
                    return ProjeAltProjeKrediHibeYazdir(2);
                else if (ekBilgi.Equals("KrediHibe"))
                    return KrediHibeYazdir(OrtakFonksiyonlar.ConvertToInt(raporParam["KaynakNo"], 0));
                else if (ekBilgi.Equals("DetayP"))
                    return DetayProgramYazdir();
                else
                    throw new RaporHatasi("Proje dosya raporlarına hatalı ek bilgi kodu geldi!");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string DetayProgramYazdir()
        {
            AltProjeDetayProgrami form = new AltProjeDetayProgrami();
            form.yil = butceYil;
            form.senaryoKod = senaryoKod;
            form.altProjeNo = altProjeNo;
            form.projeNo = projeNo;
            ObjectArray oa = servisYAT.ListeleAltProjeDetayProgrami(kullanan, form);

            DataTable tabHeader = new DataTable();
            tabHeader.TableName = "Tab";

            tabHeader.Columns.Add("sira");
            tabHeader.Columns.Add("yer");
            tabHeader.Columns.Add("birimi");
            tabHeader.Columns.Add("odenek");
            tabHeader.Columns.Add("aciklama");

            double top = 0;
            foreach (AltProjeDetayProgrami item in oa.objeler)
            {
                tabHeader.Rows.Add(item.siraNo.ToString(), item.yer, item.birimi, item.odenek.ToString("#,##0"), item.aciklama);
                top += item.odenek;
            }
            if (top == 0)
                tabHeader.Rows.Add("", "", "", "", "");
            Aspose.Words.Document doc = new Aspose.Words.Document(sablonAd);
            DocumentBuilder builder = new DocumentBuilder(doc);

            string baslik = butceYil.ToString() + " YILI DETAY PROGRAMI";
            if (this.raporParam["baslik"] == null || (string)this.raporParam["baslik"] == "1")
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "baslik", baslik);
            else
            {
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "baslik", "");
            }
            doc.MailMerge.ExecuteWithRegions(tabHeader);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "t_odenek", top.ToString("#,##0"));

            doc.Save(this.sonucDosyaAdWrd);
            return this.sonucDosyaAdWrd;
        }

        private string ProjeAltProjeKrediHibeYazdir(int krediHibe)
        {
            ENUMButceKaynakTur[] kaynakTurler;
            string harf;
            string krediHibeStr;
            string krediHibeStr1;
            if (krediHibe == 1)
            {
                kaynakTurler = new ENUMButceKaynakTur[] { ENUMButceKaynakTur.KREDIBUTCEDISI, ENUMButceKaynakTur.KREDIBUTCEICI };
                harf = "D";
                krediHibeStr = "DIŞ KREDİSİNE";
                krediHibeStr1 = "Kredi";
            }
            else
            {
                kaynakTurler = new ENUMButceKaynakTur[] { ENUMButceKaynakTur.HIBE };
                harf = "E";
                krediHibeStr = "HİBESİNE";
                krediHibeStr1 = "Hibe";
            }
            ObjectArray oa = servisYAT.KrediliKaynaklariListele(kullanan, butceYil, senaryoKod, projeNo, altProjeNo, kaynakTurler);
            string sakla = this.sonucDosyaAdWrd;

            //GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Harf", harf);
            //GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KrediHibe", krediHibeStr);

            int say = 0;
            if (oa.ObjeSayisi > 0)
            {
                Aspose.Words.Document ilkDoc = null;
                string ilkAd = null;
                for (int i = 0; i < oa.ObjeSayisi; i++)
                {
                    int kaynakNo = (int)oa.objeler[i];
                    System.Collections.Hashtable raporParam = new System.Collections.Hashtable();
                    raporParam["Senaryo"] = senaryoKod;
                    raporParam["ButceYil"] = butceYil.ToString();
                    raporParam["KaynakNo"] = kaynakNo.ToString();
                    raporParam["ProjeNo"] = "-";
                    raporParam["AltProjeNo"] = "-";
                    raporParam["Format"] = "docx";
                    if (i == 0)
                        raporParam["Baslik"] = harf;
                    else
                        raporParam["Baslik"] = "";

                    string uretilenDosya = YatProjeRaporlari.TekRaporUret(2004, 12, kullanan, null, raporParam);
                    if (i == 0)
                    {
                        ilkDoc = new Aspose.Words.Document(uretilenDosya);
                        ilkAd = uretilenDosya;
                    }
                    else
                    {
                        // Open the document to join.
                        Aspose.Words.Document srcDoc = new Aspose.Words.Document(uretilenDosya);
                        // Append the source document at the end of the destination document.
                        ilkDoc.AppendDocument(srcDoc, Aspose.Words.ImportFormatMode.UseDestinationStyles);
                        ilkDoc.Sections[say + 1].PageSetup.SectionStart = Aspose.Words.SectionStart.Continuous;
                        srcDoc = null;
                        //if (say < 2)
                        //else
                        //{
                        //    doc.Sections[say].PageSetup.SectionStart = Aspose.Words.SectionStart.NewPage;
                        //    doc.Sections[say].PageSetup.RestartPageNumbering = true;
                        //    doc.Sections[say].PageSetup.PageStartingNumber = 1;
                        //}
                        say++;
                    }
                    try
                    {
                        System.IO.File.Delete(uretilenDosya);
                    }
                    catch { }
                }
                ilkDoc.Save(sonucDosyaAdWrd);
            }
            else
            {
                Aspose.Words.Document doc = new Aspose.Words.Document(sablonAd);
                DocumentBuilder builder = new DocumentBuilder(doc);

                builder.Font.Bold = true;
                builder.Writeln(harf + "- PROJE " + krediHibeStr + " AİT BİLGİLER");

                builder.Font.Bold = false;
                builder.ParagraphFormat.FirstLineIndent = 16;
                builder.Writeln("Bu projeye ait " + krediHibeStr1 + " bulunmamaktadır.");
                doc.Save(sonucDosyaAdWrd);
            }
            return sonucDosyaAdWrd;
        }

        public class KrediPlanSirala : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x is KrediDetay && y is KrediDetay)
                {
                    return ((KrediDetay)x).yil.CompareTo(((KrediDetay)y).yil);
                }
                return 0;
            }
        }

        private string KrediHibeYazdir(int kaynakNo)
        {
            if (kaynakNo <= 0)
            {
                throw new RaporHatasi("Bütçe Kaynak No boş geliyor.");
            }

            BilgiVAN bilgi = servisYAT.ButceKaynakBilgiOku(kullanan, kaynakNo, butceYil, senaryoKod, string.Empty, true, true, false, null);
            if (!bilgi.sonuc.islemSonuc)
            {
                throw new RaporHatasi(bilgi.sonuc.hataStr);
            }

            if (bilgi.liste.Count == 0)
            {
                throw new RaporHatasi(bilgi.sonuc.bilgiStr);
            }

            ButceKaynak kaynak = (ButceKaynak)bilgi.liste[0];

            Aspose.Words.Document doc = new Aspose.Words.Document(sablonAd);
            DocumentBuilder builder = new DocumentBuilder(doc);
            string baslik = (string)this.raporParam["Baslik"];
            string krediHibe = "Kredi";
            if (!string.IsNullOrWhiteSpace(baslik))
            {
                string krediHibeStr;
                if (baslik == "D")
                    krediHibeStr = "DIŞ KREDİSİNE";
                else
                {
                    krediHibeStr = "HİBESİNE";
                    krediHibe = "Hibe";
                }
                builder.Font.Bold = true;
                builder.ParagraphFormat.KeepWithNext = true;
                builder.Writeln(baslik + "- PROJE " + krediHibeStr + " AİT BİLGİLER");
            }

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Ad", kaynak.kaynakAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Parabirimi", kaynak.pbKisaltma);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Dipnot", kaynak.dipnot);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KrediHibe", krediHibe);

            DataTable tabHeader = new DataTable();
            tabHeader.TableName = "Tab";

            tabHeader.Columns.Add("Aciklama");
            tabHeader.Columns.Add("DK1");
            tabHeader.Columns.Add("IK1");
            tabHeader.Columns.Add("T1");

            tabHeader.Columns.Add("DK2");
            tabHeader.Columns.Add("IK2");
            tabHeader.Columns.Add("T2");

            tabHeader.Columns.Add("DK3");
            tabHeader.Columns.Add("IK3");
            tabHeader.Columns.Add("T3");

            tabHeader.Columns.Add("Yuzde");

            ToplamAl toplamAl = new ToplamAl(2, 6);
            toplamAl.SatirSifirla(0);
            kaynak.krediPlan.Sort(new KrediPlanSirala());
            for (int i = 0; i < kaynak.krediPlan.Count; i++)
            {
                KrediDetay kd = (KrediDetay)kaynak.krediPlan[i];
                toplamAl.DegerAta(1, 0, kd.anlasmaTutari);
                toplamAl.DegerAta(1, 1, kd.anlasmaTutariIc);
                toplamAl.DegerAta(1, 2, kd.kullanilanTutar);
                toplamAl.DegerAta(1, 3, kd.kullanilanTutarIc);
                toplamAl.DegerAta(1, 4, kd.kullanilacakTutar);
                toplamAl.DegerAta(1, 5, kd.kullanilacakTutarIc);
                TabloyaEkle(tabHeader, kd.yil.ToString(), toplamAl, 1);
                toplamAl.ToplamEkle(0, 1);
            }
            //TabloyaEkle(tabHeader, "Toplam", toplamAl, 0);
            doc.MailMerge.ExecuteWithRegions(tabHeader);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "tDK1", Noktala(toplamAl.toplam[0, 0] / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "tIK1", Noktala(toplamAl.toplam[0, 1] / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "tT1", Noktala((toplamAl.toplam[0, 0] + toplamAl.toplam[0, 1]) / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "tDK2", Noktala(toplamAl.toplam[0, 2] / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "tIK2", Noktala(toplamAl.toplam[0, 3] / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "tT2", Noktala((toplamAl.toplam[0, 2] + toplamAl.toplam[0, 3]) / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "tDK3", Noktala(toplamAl.toplam[0, 4] / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "tIK3", Noktala(toplamAl.toplam[0, 5] / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "tT3", Noktala((toplamAl.toplam[0, 4] + toplamAl.toplam[0, 5]) / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "tYuzde", Oranla(toplamAl.toplam[0, 2], toplamAl.toplam[0, 0]));

            doc.Save(this.sonucDosyaAdWrd);
            return this.sonucDosyaAdWrd;
        }

        private void TabloyaEkle(DataTable tabHeader, string aciklama, OrtakClass.ToplamAl toplamAl, int sat)
        {
            tabHeader.Rows.Add(aciklama, Noktala(toplamAl.toplam[sat, 0] / 1000), Noktala(toplamAl.toplam[sat, 1] / 1000), Noktala((toplamAl.toplam[sat, 0] + toplamAl.toplam[sat, 1]) / 1000)
                                       , Noktala(toplamAl.toplam[sat, 2] / 1000), Noktala(toplamAl.toplam[sat, 3] / 1000), Noktala((toplamAl.toplam[sat, 2] + toplamAl.toplam[sat, 3]) / 1000)
                                       , Noktala(toplamAl.toplam[sat, 4] / 1000), Noktala(toplamAl.toplam[sat, 5] / 1000), Noktala((toplamAl.toplam[sat, 4] + toplamAl.toplam[sat, 5]) / 1000)
                                       , Oranla(toplamAl.toplam[sat, 2], toplamAl.toplam[sat, 0])
                );
        }

        private string BilgiYazdir()
        {
            ProjeAltProjeOku();
            Hashtable[] ht;

            if (altProje != null)
            {
                ht = new Hashtable[1];
                ht[0] = servisYAT.AltProjeBilgiListele(kullanan, butceYil, senaryoKod, projeNo, altProjeNo, null);
            }
            else
            {
                ht = new Hashtable[1];
                ht[0] = servisYAT.AltProjeBilgiListele(kullanan, butceYil, senaryoKod, projeNo, 0, null);
                //int say = 0;
                //ht = new Hashtable[altProjeler.liste.Count];
                //foreach (AltProje item in altProjeler.liste)
                //{
                //    ht[say++] = servisYAT.AltProjeBilgiListele(kullanan, butceYil, item.proje.kod, item.kod, null);
                //}
            }

            Aspose.Words.Document doc = new Aspose.Words.Document(sablonAd);
            DocumentBuilder builder = new DocumentBuilder(doc);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYil_2", (butceYil - 2).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYil_1", (butceYil - 1).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYil", butceYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYil1", (butceYil + 1).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYil2", (butceYil + 2).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYil3", (butceYil + 3).ToString());

            YatRaporAraclari.AsposeAlanaYazHtml(builder, "amac", TNS.YAT.Arac.HTDegerAl(ht, "amac"));
            YatRaporAraclari.AsposeAlanaYazHtml(builder, "kapsam", TNS.YAT.Arac.HTDegerAl(ht, "kapsam"));
            YatRaporAraclari.AsposeAlanaYazHtml(builder, "gerekce", TNS.YAT.Arac.HTDegerAl(ht, "gerekce"));
            YatRaporAraclari.AsposeAlanaYazHtml(builder, "gerceklesmeDegerlendirme", TNS.YAT.Arac.HTDegerAl(ht, "gerceklesmeDegerlendirme"));
            YatRaporAraclari.AsposeAlanaYazHtml(builder, "progFaal", TNS.YAT.Arac.HTDegerAl(ht, "progFaal"));
            YatRaporAraclari.AsposeAlanaYazHtml(builder, "yapilanFaal", TNS.YAT.Arac.HTDegerAl(ht, "yapilanFaal"));
            YatRaporAraclari.AsposeAlanaYazHtml(builder, "plan0", TNS.YAT.Arac.HTDegerAl(ht, "plan0"));
            YatRaporAraclari.AsposeAlanaYazHtml(builder, "plan1", TNS.YAT.Arac.HTDegerAl(ht, "plan1"));
            YatRaporAraclari.AsposeAlanaYazHtml(builder, "plan2", TNS.YAT.Arac.HTDegerAl(ht, "plan2"));
            YatRaporAraclari.AsposeAlanaYazHtml(builder, "plan3", TNS.YAT.Arac.HTDegerAl(ht, "plan3"));
            this.raporParam["baslik"] = "0";
            string dosyaAd = YatProjeRaporlari.TekRaporUret(2004, 13, kullanan, null, this.raporParam);
            GenelIslemlerIstemci.AsposeAlanaDocEkle(builder, "detayProg", dosyaAd);
            try
            {
                System.IO.File.Delete(dosyaAd);
            }
            catch { }
            YatRaporAraclari.AsposeAlanaYazHtml(builder, "sorunCozum", TNS.YAT.Arac.HTDegerAl(ht, "sorunCozum"));

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HazAdSoy", TNS.YAT.Arac.HTDegerAl(ht, "HazAdSoy"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HazUnvan", TNS.YAT.Arac.HTDegerAl(ht, "HazUnvan"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HazBirim", TNS.YAT.Arac.HTDegerAl(ht, "HazBirim"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HazIsTel", TNS.YAT.Arac.HTDegerAl(ht, "HazIsTel"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HazCepTel", TNS.YAT.Arac.HTDegerAl(ht, "HazCepTel"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HazFaks", TNS.YAT.Arac.HTDegerAl(ht, "HazFaks"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HazEmail", TNS.YAT.Arac.HTDegerAl(ht, "HazEmail"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "YurAdSoy", TNS.YAT.Arac.HTDegerAl(ht, "YurAdSoy"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "YurUnvan", TNS.YAT.Arac.HTDegerAl(ht, "YurUnvan"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "YurBirim", TNS.YAT.Arac.HTDegerAl(ht, "YurBirim"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "YurIsTel", TNS.YAT.Arac.HTDegerAl(ht, "YurIsTel"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "YurCepTel", TNS.YAT.Arac.HTDegerAl(ht, "YurCepTel"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "YurFaks", TNS.YAT.Arac.HTDegerAl(ht, "YurFaks"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "YurEmail", TNS.YAT.Arac.HTDegerAl(ht, "YurEmail"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KoorAdSoy", TNS.YAT.Arac.HTDegerAl(ht, "KoorAdSoy"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KoorUnvan", TNS.YAT.Arac.HTDegerAl(ht, "KoorUnvan"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KoorBirim", TNS.YAT.Arac.HTDegerAl(ht, "KoorBirim"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KoorIsTel", TNS.YAT.Arac.HTDegerAl(ht, "KoorIsTel"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KoorCepTel", TNS.YAT.Arac.HTDegerAl(ht, "KoorCepTel"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KoorFaks", TNS.YAT.Arac.HTDegerAl(ht, "KoorFaks"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KoorEmail", TNS.YAT.Arac.HTDegerAl(ht, "KoorEmail"));

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "kurumAd", YatRaporAraclari.VarsayilanKurumBul() + "- " + YatRaporAraclari.VarsayilanKurumBulAd());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "birimKodAd", BirimAdBul(true));

            YatRaporAraclari.AsposeAlanaYazHtml(builder, "hizmetGerekcesi", TNS.YAT.Arac.HTDegerAl(ht, "hizmetGerekcesi"));

            doc.Save(this.sonucDosyaAdWrd);
            return this.sonucDosyaAdWrd;
        }

        private string TeklifYazdir(int teklifYil)
        {
            Aspose.Words.Document doc = new Aspose.Words.Document(sablonAd);
            DocumentBuilder builder = new DocumentBuilder(doc);
            string bolum = "";
            if (teklifYil == 0) bolum = "F";
            else if (teklifYil == 1) bolum = "G";
            else if (teklifYil == 2) bolum = "H";

            ObjectArray odenekListesi = servisYAT.OdenekListesiBul(kullanan, butceYil, senaryoKod, teklifYil, projeNo, altProjeNo);
            if (!odenekListesi.sonuc.islemSonuc)
                throw new RaporHatasi(odenekListesi.sonuc.hataStr);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "BolumHarf", bolum);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYil", (butceYil + teklifYil).ToString());

            DataTable tabHeader = new DataTable();
            tabHeader.TableName = "Tab";

            tabHeader.Columns.Add("Aciklama");
            tabHeader.Columns.Add("DK");
            tabHeader.Columns.Add("IK");
            tabHeader.Columns.Add("T");
            tabHeader.Columns.Add("DKE");
            tabHeader.Columns.Add("TE");

            string oncEko = "";
            double t0 = 0;
            double t1 = 0;
            double t2 = 0;
            double t3 = 0;
            DataRow r = null;
            for (int i = 0; i < odenekListesi.ObjeSayisi; i++)
            {
                OdenekBilgi ob = (OdenekBilgi)odenekListesi[i];
                string eko2 = ob.tertip.kodlar[3].Substring(0, 2);
                if (eko2 != oncEko || r == null)
                {
                    if (r != null)
                    {
                        r.SetField(0, oncEko + "- " + EkoAd(oncEko));
                        r.SetField(1, Noktala(t1 / 1000));
                        r.SetField(2, Noktala((t0 - t1) / 1000));
                        r.SetField(3, Noktala(t0 / 1000));
                        r.SetField(4, Noktala(t2 / 1000));
                        r.SetField(5, Noktala(t3 / 1000));
                    }
                    oncEko = eko2;
                    t0 = 0;
                    t1 = 0;
                    t2 = 0;
                    t3 = 0;
                    oncEko = eko2;
                    r = tabHeader.NewRow();
                    tabHeader.Rows.Add(r);
                }
                double it0 = ob.EkRakam(0);
                double it1 = ob.EkRakam(1);
                double it2 = ob.EkRakam(3);
                double it3 = ob.EkRakam(2);

                t0 += it0;
                t1 += it1;
                t2 += it2;
                t3 += it3;

                tabHeader.Rows.Add("    " + ob.tertip.kodlar[3] + "- " + ob.tertip.adlar[3], Noktala(it1 / 1000), Noktala((it0 - it1) / 1000), Noktala(it0 / 1000), Noktala(it2 / 1000), Noktala(it3 / 1000));
            }
            if (r != null)
            {
                r.SetField(0, oncEko + "- " + EkoAd(oncEko));
                r.SetField(1, Noktala(t1 / 1000));
                r.SetField(2, Noktala((t0 - t1) / 1000));
                r.SetField(3, Noktala(t0 / 1000));
                r.SetField(4, Noktala(t2 / 1000));
                r.SetField(5, Noktala(t3 / 1000));
            }
            else
                tabHeader.Rows.Add("", "", "", "");
            //doc.MailMerge.FieldMergingCallback = new HandleMergeFieldAlternatingRows(builder);
            doc.MailMerge.ExecuteWithRegions(tabHeader);

            Node[] table = doc.GetChildNodes(NodeType.Table, true).ToArray();

            foreach (Row row in ((Table)table[0]).Rows)
            {
                Cell ilkCell = row.Cells[0];
                string s = ilkCell.GetText();
                string[] kodlar = s.Split('-');
                if (kodlar.Length > 1 && kodlar[0].Length == 2)
                {
                    foreach (Cell cell in row.Cells)
                    {
                        NodeCollection runs = cell.GetChildNodes(NodeType.Run, true);
                        foreach (Run run in runs)
                        {
                            run.Font.Bold = true;
                        }
                    }
                }
            }

            doc.Save(this.sonucDosyaAdWrd);
            return this.sonucDosyaAdWrd;
        }

        private string TeklifYazdirTam()
        {
            ProjeAltProjeOku();

            Aspose.Words.Document doc = new Aspose.Words.Document(sablonAd);
            DocumentBuilder builder = new DocumentBuilder(doc);

            TNS.BHMv2.UcgenSatir[] odenekListesi = servisYAT.OdenekListesiBulTam(kullanan, butceYil, senaryoKod, projeNo, altProjeNo);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYil", (butceYil).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYil2", (butceYil + 2).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYil1", (butceYil + 1).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ProjeAd", proje.ad + " " + AltProjeAdBul(true, false));

            DataTable tabHeader = new DataTable();
            tabHeader.TableName = "Tab";

            tabHeader.Columns.Add("K0");
            tabHeader.Columns.Add("K1");

            tabHeader.Columns.Add("K2");
            tabHeader.Columns.Add("K3");
            tabHeader.Columns.Add("K4");
            tabHeader.Columns.Add("K5");

            tabHeader.Columns.Add("K6");

            tabHeader.Columns.Add("K7");
            tabHeader.Columns.Add("K8");
            tabHeader.Columns.Add("K9");
            tabHeader.Columns.Add("K10");

            tabHeader.Columns.Add("Aciklama");
            tabHeader.Columns.Add("T0");
            tabHeader.Columns.Add("T1");
            tabHeader.Columns.Add("T2");

            for (int i = 0; i < odenekListesi.Length; i++)
            {
                UcgenSatir ob = odenekListesi[i];
                //if (ob.aciklama.Length > 1)
                {
                    string[] degerler = new string[15];
                    double t0 = (double)ob.ekDegerler[0];
                    double t1 = (double)ob.ekDegerler[1];
                    double t2 = (double)ob.ekDegerler[2];

                    degerler[ob.yer] = ob.kodParca;
                    degerler[11] = ob.aciklama;
                    degerler[12] = Noktala(t0 / 1000);
                    degerler[13] = Noktala(t1 / 1000);
                    degerler[14] = Noktala(t2 / 1000);
                    tabHeader.Rows.Add(degerler);
                }
            }
            //doc.MailMerge.FieldMergingCallback = new HandleMergeFieldAlternatingRows(builder);
            doc.MailMerge.ExecuteWithRegions(tabHeader);

            Node[] table = doc.GetChildNodes(NodeType.Table, true).ToArray();

            //foreach (Row row in ((Table)table[0]).Rows)
            //{
            //    Cell ilkCell = row.Cells[0];
            //    string s = ilkCell.GetText();
            //    string[] kodlar = s.Split('-');
            //    if (kodlar.Length > 1 && kodlar[0].Length == 2)
            //    {
            //        foreach (Cell cell in row.Cells)
            //        {
            //            NodeCollection runs = cell.GetChildNodes(NodeType.Run, true);
            //            foreach (Run run in runs)
            //            {
            //                run.Font.Bold = true;
            //            }
            //        }
            //    }
            //}

            doc.Save(this.sonucDosyaAdWrd);
            return this.sonucDosyaAdWrd;
        }

        private string EkoAd(string eko)
        {
            if (uzyServis == null)
                uzyServis = TNS.UZY.Arac.Tanimla();
            return uzyServis.UzayDegeriStr(kullanan, "ABSEKO", butceYil.ToString() + "-" + eko, true);
        }

        private class HandleMergeFieldAlternatingRows : IFieldMergingCallback
        {
            private DocumentBuilder mBuilder;

            public HandleMergeFieldAlternatingRows(DocumentBuilder builder)
            {
                this.mBuilder = builder;
            }

            public void FieldMerging(FieldMergingArgs e)
            {
                if (mBuilder == null)
                    return;

                if (e.FieldName.Equals("Aciklama"))
                {
                    string s = (string)e.FieldValue;

                    string[] kodlar = s.Split('-');
                    if (kodlar.Length > 1 && kodlar[0].Length == 2)
                    {
                        for (int colIdx = 0; colIdx < 4; colIdx++)
                        {
                            mBuilder.MoveToCell(0, e.RecordIndex + 2, colIdx, 0);
                        }
                    }
                }
            }

            public void ImageFieldMerging(ImageFieldMergingArgs args)
            {
            }
        }

        private string Noktala(double d)
        {
            return d.ToString("#,##0");
        }

        private string Oranla(double d1, double d2)
        {
            if (d2 == 0) return "";
            return (d1 * 100 / d2).ToString("##0.00");
        }

        private string HarcamaYazdir()
        {
            Aspose.Words.Document doc = new Aspose.Words.Document(sablonAd);
            DocumentBuilder builder = new DocumentBuilder(doc);

            DataTable tabHeader = new DataTable();
            tabHeader.TableName = "Tab";

            tabHeader.Columns.Add("Aciklama");
            tabHeader.Columns.Add("DK1");
            tabHeader.Columns.Add("T1");
            tabHeader.Columns.Add("H1");

            tabHeader.Columns.Add("DK2");
            tabHeader.Columns.Add("T2");
            tabHeader.Columns.Add("H2");

            tabHeader.Columns.Add("DKH");
            tabHeader.Columns.Add("TH");
            tabHeader.Columns.Add("HH");

            tabHeader.Columns.Add("O");

            ObjectArray programOd;
            ObjectArray eklenenOd;
            ObjectArray dusulenOd;
            ObjectArray revizeOd;
            ObjectArray harcamalar;

            for (int icYil = butceYil - 13; icYil <= butceYil - 2; icYil++)
            {
                int siraNo = icYil - (butceYil - 13) + 1;
                string format;
                if (siraNo == 1)
                    format = "{0}-Programa giriş yılından (başlama tarihinden) itibaren {1} sonuna kadar kümülatif tutar.";
                else
                    format = "{0}-{1} Yılı Yatırım Tutarı";

                programOd = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, icYil, "06.00", ENUMProjeMaliYapiKod.teklif0, projeNo, altProjeNo, siraNo == 1, false);
                if (!programOd.sonuc.islemSonuc)
                    throw new RaporHatasi(programOd.sonuc.hataStr);

                eklenenOd = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, icYil, "06.00", ENUMProjeMaliYapiKod.odenekEklenen, projeNo, altProjeNo, siraNo == 1, false);
                if (!programOd.sonuc.islemSonuc)
                    throw new RaporHatasi(programOd.sonuc.hataStr);

                dusulenOd = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, icYil, "06.00", ENUMProjeMaliYapiKod.odenekDusulen, projeNo, altProjeNo, siraNo == 1, false);
                if (!programOd.sonuc.islemSonuc)
                    throw new RaporHatasi(programOd.sonuc.hataStr);

                revizeOd = Topla(1, programOd, eklenenOd, dusulenOd);
                if (!revizeOd.sonuc.islemSonuc)
                    throw new RaporHatasi(revizeOd.sonuc.hataStr);

                harcamalar = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, icYil, "06.00", ENUMProjeMaliYapiKod.harcamaYilSonu, projeNo, altProjeNo, siraNo == 1, false);
                if (!harcamalar.sonuc.islemSonuc)
                    throw new RaporHatasi(harcamalar.sonuc.hataStr);

                tabHeader.Rows.Add(String.Format(format, siraNo, icYil), StringeCevir(programOd, 1), StringeCevir(programOd, 3), StringeCevir(programOd, 4), StringeCevir(revizeOd, 1), StringeCevir(revizeOd, 3), StringeCevir(revizeOd, 4), StringeCevir(harcamalar, 1), StringeCevir(harcamalar, 3), StringeCevir(harcamalar, 4), YuzdeBul(ToplamAl(harcamalar, 3), ToplamAl(revizeOd, 3)));
            }

            programOd = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil - 1, "06.00", ENUMProjeMaliYapiKod.teklif0, projeNo, altProjeNo, false, false);
            if (!programOd.sonuc.islemSonuc)
                throw new RaporHatasi(programOd.sonuc.hataStr);

            eklenenOd = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil - 1, senaryoKod, ENUMProjeMaliYapiKod.odenekEklenen, projeNo, altProjeNo, false, false);
            if (!programOd.sonuc.islemSonuc)
                throw new RaporHatasi(programOd.sonuc.hataStr);

            dusulenOd = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil - 1, senaryoKod, ENUMProjeMaliYapiKod.odenekDusulen, projeNo, altProjeNo, false, false);
            if (!programOd.sonuc.islemSonuc)
                throw new RaporHatasi(programOd.sonuc.hataStr);

            revizeOd = Topla(1, programOd, eklenenOd, dusulenOd);
            if (!revizeOd.sonuc.islemSonuc)
                throw new RaporHatasi(revizeOd.sonuc.hataStr);

            harcamalar = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil - 1, senaryoKod, ENUMProjeMaliYapiKod.harcamaYilSonu, projeNo, altProjeNo, false, false);
            if (!harcamalar.sonuc.islemSonuc)
                throw new RaporHatasi(harcamalar.sonuc.hataStr);

            ObjectArray harcama6 = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil - 1, senaryoKod, ENUMProjeMaliYapiKod.harcama6ay, projeNo, altProjeNo, false, false);
            if (!harcama6.sonuc.islemSonuc)
                throw new RaporHatasi(harcama6.sonuc.hataStr);

            ObjectArray harcamaTah = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil - 1, senaryoKod, ENUMProjeMaliYapiKod.harcamaTahmin, projeNo, altProjeNo, false, false);
            if (!harcamaTah.sonuc.islemSonuc)
                throw new RaporHatasi(harcamaTah.sonuc.hataStr);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYil_1", (butceYil - 1).ToString());

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "DK0", StringeCevir(programOd, 1));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "T0", StringeCevir(programOd, 3));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "H0", StringeCevir(programOd, 4));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "DK0R", StringeCevir(revizeOd, 1));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "T0R", StringeCevir(revizeOd, 3));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "H0R", StringeCevir(revizeOd, 1));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "DK0H", StringeCevir(harcamalar, 1));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "T0H", StringeCevir(harcamalar, 3));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "H0H", StringeCevir(harcamalar, 4));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "YTO", YuzdeBul(ToplamAl(harcamalar, 3), ToplamAl(revizeOd, 3)));

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "DK2H", StringeCevir(harcama6, 1));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "T2H", StringeCevir(harcama6, 3));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "H2H", StringeCevir(harcama6, 4));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "YTO2", YuzdeBul(ToplamAl(harcama6, 3), ToplamAl(revizeOd, 3)));

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "DK3H", StringeCevir(harcamaTah, 1));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "T3H", StringeCevir(harcamaTah, 3));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "H3H", StringeCevir(harcamaTah, 6));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "YTO3", YuzdeBul(ToplamAl(harcamaTah, 3), ToplamAl(revizeOd, 3)));

            doc.MailMerge.ExecuteWithRegions(tabHeader);
            doc.Save(this.sonucDosyaAdWrd);
            return this.sonucDosyaAdWrd;
        }

        private string YuzdeBul(double bolunen, double bolen)
        {
            if (bolen == 0)
            {
                if (bolunen == 0) return "";
                return "∞";
            }
            return (bolunen * 100 / bolen).ToString("0.00");
        }

        public class DovizOdenek
        {
            public double kredi;
            public double icKaynak;
            public string paraBirimi;

            public DovizOdenek(double kredi, double icKaynak, string paraBirimi)
            {
                this.kredi = kredi;
                this.icKaynak = icKaynak;
                this.paraBirimi = paraBirimi;
            }
        }

        public class DovizOdenekler
        {
            public List<DovizOdenek> dovizler = new List<DovizOdenek>();
            internal void Ekle(double kredi, double icKaynak, string paraBirimi)
            {
                DovizOdenek d_o = DovizBul(paraBirimi);
                if (d_o == null)
                {
                    d_o = new DovizOdenek(kredi, icKaynak, paraBirimi);
                    dovizler.Add(d_o);
                }
                else
                {
                    d_o.kredi += kredi;
                    d_o.icKaynak += icKaynak;
                }
            }

            private DovizOdenek DovizBul(string paraBirimi)
            {
                foreach (var item in dovizler)
                {
                    if (item.paraBirimi == paraBirimi) return item;
                }
                return null;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="tur">0 kredi, 1 iç kaynak, 2 toplam</param>
            /// <param name="pbEkle"></param>
            /// <returns></returns>
            public string StringOl(int tur, bool pbEkle)
            {
                string sonuc = "";
                foreach (var item in dovizler)
                {
                    double deger = 0;
                    if (tur == 0) deger = item.kredi;
                    else if (tur == 1) deger = item.icKaynak;
                    else if (tur == 2) deger = item.kredi + item.icKaynak;
                    if (deger != 0)
                    {
                        string ic = (deger / 1000).ToString("#,###");
                        if (pbEkle) ic += " " + item.paraBirimi;
                        if (sonuc != "") sonuc += ", ";
                        sonuc += ic;
                    }
                }
                return sonuc;
            }
        }

        private string TutarYazdir()
        {
            BilgiVAN bvKredi = servisYAT.ButceKaynakListele(kullanan, projeNo, altProjeNo, 0, butceYil, senaryoKod, new ENUMButceKaynakTur[] { ENUMButceKaynakTur.KREDIBUTCEICI, ENUMButceKaynakTur.KREDIBUTCEDISI }, true);
            BilgiVAN bvHibe = servisYAT.ButceKaynakListele(kullanan, projeNo, altProjeNo, 0, butceYil, senaryoKod, new ENUMButceKaynakTur[] { ENUMButceKaynakTur.HIBE }, true);
            TNSCollection tc = new TNSCollection();
            ParaBirimleriniListeyeEkle(tc, bvKredi);
            ParaBirimleriniListeyeEkle(tc, bvKredi);

            DovizOdenekler[,] dovizOdenek = new DovizOdenekler[13, 2];

            dovizOdenek[0, 0] = DovizOdenekOl(bvKredi, -1, butceYil + 300, 0);
            dovizOdenek[0, 1] = DovizOdenekOl(bvHibe, -1, butceYil + 300, 0);

            dovizOdenek[1, 0] = DovizOdenekOl(bvKredi, -1, butceYil - 2, 1);
            dovizOdenek[1, 1] = DovizOdenekOl(bvHibe, -1, butceYil - 2, 1);

            dovizOdenek[2, 0] = DovizOdenekOl(bvKredi, butceYil - 1, butceYil - 1, 2);
            dovizOdenek[2, 1] = DovizOdenekOl(bvHibe, butceYil - 1, butceYil - 1, 2);

            dovizOdenek[3, 0] = new DovizOdenekler();
            dovizOdenek[3, 1] = new DovizOdenekler();

            dovizOdenek[4, 0] = DovizOdenekOl(bvKredi, butceYil - 1, butceYil - 1, 2);
            dovizOdenek[4, 1] = DovizOdenekOl(bvHibe, butceYil - 1, butceYil - 1, 2);

            dovizOdenek[5, 0] = DovizOdenekTopla(dovizOdenek[4, 0], dovizOdenek[1, 0]);
            dovizOdenek[5, 1] = DovizOdenekTopla(dovizOdenek[4, 1], dovizOdenek[1, 1]);

            dovizOdenek[6, 0] = DovizOdenekOl(bvKredi, butceYil, butceYil, 2);
            dovizOdenek[6, 1] = DovizOdenekOl(bvHibe, butceYil, butceYil, 2);

            dovizOdenek[7, 0] = DovizOdenekOl(bvKredi, butceYil + 1, butceYil + 1, 2);
            dovizOdenek[7, 1] = DovizOdenekOl(bvHibe, butceYil + 1, butceYil + 1, 2);

            dovizOdenek[8, 0] = DovizOdenekOl(bvKredi, butceYil + 2, butceYil + 2, 2);
            dovizOdenek[8, 1] = DovizOdenekOl(bvHibe, butceYil + 2, butceYil + 2, 2);

            dovizOdenek[9, 0] = DovizOdenekOl(bvKredi, butceYil + 3, butceYil + 300, 2);
            dovizOdenek[9, 1] = DovizOdenekOl(bvHibe, butceYil + 3, butceYil + 300, 2);

            Aspose.Words.Document doc = new Aspose.Words.Document(sablonAd);
            DocumentBuilder builder = new DocumentBuilder(doc);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYil", butceYil.ToString());
            string dovizAdlar = "";
            foreach (string item in tc)
            {
                if (dovizAdlar != "") dovizAdlar += ", ";
                dovizAdlar += item;
            }

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Doviz", dovizAdlar);

            DataTable tabHeader = new DataTable();
            tabHeader.TableName = "Tab";

            tabHeader.Columns.Add("Aciklama");
            tabHeader.Columns.Add("DK1");
            tabHeader.Columns.Add("IK1");
            tabHeader.Columns.Add("T1");
            tabHeader.Columns.Add("H1");

            tabHeader.Columns.Add("DK2");
            tabHeader.Columns.Add("IK2");
            tabHeader.Columns.Add("T2");
            tabHeader.Columns.Add("H2");

            ObjectArray projeTutari = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil, senaryoKod, ENUMProjeMaliYapiKod.kaynakToplam, projeNo, altProjeNo, false, true);
            if (!projeTutari.sonuc.islemSonuc)
                throw new RaporHatasi(projeTutari.sonuc.hataStr);

            ObjectArray kumulatifHarcamaBirOnce = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil - 1, senaryoKod, ENUMProjeMaliYapiKod.kumulatifHarcama, projeNo, altProjeNo, false, true);
            if (!kumulatifHarcamaBirOnce.sonuc.islemSonuc)
                throw new RaporHatasi(kumulatifHarcamaBirOnce.sonuc.hataStr);

            ObjectArray kumulatifHarcamaIkiOnce = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil - 2, senaryoKod, ENUMProjeMaliYapiKod.kumulatifHarcama, projeNo, altProjeNo, false, true);
            if (!kumulatifHarcamaIkiOnce.sonuc.islemSonuc)
                throw new RaporHatasi(kumulatifHarcamaIkiOnce.sonuc.hataStr);

            ObjectArray yatirim_1 = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil - 1, "06.00", ENUMProjeMaliYapiKod.teklif0, projeNo, altProjeNo, false, true);
            if (!yatirim_1.sonuc.islemSonuc)
                throw new RaporHatasi(yatirim_1.sonuc.hataStr);

            ObjectArray harc6_1 = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil - 1, senaryoKod, ENUMProjeMaliYapiKod.harcama6ay, projeNo, altProjeNo, false, true);
            if (!harc6_1.sonuc.islemSonuc)
                throw new RaporHatasi(harc6_1.sonuc.hataStr);

            ObjectArray harcTahmin = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil - 1, senaryoKod, ENUMProjeMaliYapiKod.harcamaTahmin, projeNo, altProjeNo, false, true);
            if (!harcTahmin.sonuc.islemSonuc)
                throw new RaporHatasi(harcTahmin.sonuc.hataStr);

            ObjectArray teklif0 = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil, senaryoKod, ENUMProjeMaliYapiKod.teklif0, projeNo, altProjeNo, false, true);
            if (!teklif0.sonuc.islemSonuc)
                throw new RaporHatasi(teklif0.sonuc.hataStr);

            ObjectArray teklif1 = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil, senaryoKod, ENUMProjeMaliYapiKod.teklif1, projeNo, altProjeNo, false, true);
            if (!teklif1.sonuc.islemSonuc)
                throw new RaporHatasi(teklif1.sonuc.hataStr);

            ObjectArray teklif2 = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil, senaryoKod, ENUMProjeMaliYapiKod.teklif2, projeNo, altProjeNo, false, true);
            if (!teklif2.sonuc.islemSonuc)
                throw new RaporHatasi(teklif2.sonuc.hataStr);

            ObjectArray ek0 = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil, senaryoKod, ENUMProjeMaliYapiKod.ekteklif0, projeNo, altProjeNo, false, true);
            if (!ek0.sonuc.islemSonuc)
                throw new RaporHatasi(ek0.sonuc.hataStr);

            ObjectArray ek1 = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil, senaryoKod, ENUMProjeMaliYapiKod.ekteklif1, projeNo, altProjeNo, false, true);
            if (!ek1.sonuc.islemSonuc)
                throw new RaporHatasi(ek1.sonuc.hataStr);

            ObjectArray ek2 = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil, senaryoKod, ENUMProjeMaliYapiKod.ekteklif2, projeNo, altProjeNo, false, true);
            if (!ek2.sonuc.islemSonuc)
                throw new RaporHatasi(ek2.sonuc.hataStr);

            ObjectArray projeKalan = servisYAT.MaliYapiBul(kullanan, butceYil, senaryoKod, butceYil, senaryoKod, ENUMProjeMaliYapiKod.sonrakiYillar, projeNo, altProjeNo, false, true);
            if (!projeKalan.sonuc.islemSonuc)
                throw new RaporHatasi(projeKalan.sonuc.hataStr);
            bool pbEkle0 = tc.Count > 1;
            int sat = 0;
            tabHeader.Rows.Add("1- Projenin Toplam Tutarı=(6+7+8+9+10)", StringeCevir(projeTutari, 1), StringeCevir(projeTutari, 2), StringeCevir(projeTutari, 3), StringeCevir(projeTutari, 4), dovizOdenek[sat, 0].StringOl(0, pbEkle0), dovizOdenek[sat, 0].StringOl(1, pbEkle0), dovizOdenek[sat, 0].StringOl(2, pbEkle0), dovizOdenek[sat++, 1].StringOl(0, pbEkle0));
            tabHeader.Rows.Add(String.Format("2- {0} Yılı Sonuna Kadar Tahmini Kümülatif Harcama (Resmi Gazeteden alınacaktır.)", butceYil - 2), StringeCevir(kumulatifHarcamaIkiOnce, 1), StringeCevir(kumulatifHarcamaIkiOnce, 2), StringeCevir(kumulatifHarcamaIkiOnce, 3), StringeCevir(kumulatifHarcamaIkiOnce, 4), dovizOdenek[sat, 0].StringOl(0, pbEkle0), dovizOdenek[sat, 0].StringOl(1, pbEkle0), dovizOdenek[sat, 0].StringOl(2, pbEkle0), dovizOdenek[sat++, 1].StringOl(0, pbEkle0));
            tabHeader.Rows.Add(String.Format("3- {0} Yılı Yatırım Tutarı", butceYil - 1), StringeCevir(yatirim_1, 1), StringeCevir(yatirim_1, 2), StringeCevir(yatirim_1, 3), StringeCevir(yatirim_1, 4), dovizOdenek[sat, 0].StringOl(0, pbEkle0), dovizOdenek[sat, 0].StringOl(1, pbEkle0), dovizOdenek[sat, 0].StringOl(2, pbEkle0), dovizOdenek[sat++, 1].StringOl(0, pbEkle0));
            tabHeader.Rows.Add(String.Format("4- {0} Yılı Haziran Sonu Harcama (İlk 6 ay)", butceYil - 1), StringeCevir(harc6_1, 1), StringeCevir(harc6_1, 2), StringeCevir(harc6_1, 3), StringeCevir(harc6_1, 4), dovizOdenek[sat, 0].StringOl(0, pbEkle0), dovizOdenek[sat, 0].StringOl(1, pbEkle0), dovizOdenek[sat, 0].StringOl(2, pbEkle0), dovizOdenek[sat++, 1].StringOl(0, pbEkle0));
            tabHeader.Rows.Add(String.Format("5- {0} Yılı Aralık Sonu Tahmini Harcama (12 ay)", butceYil - 1), StringeCevir(harcTahmin, 1), StringeCevir(harcTahmin, 2), StringeCevir(harcTahmin, 3), StringeCevir(harcTahmin, 4), dovizOdenek[sat, 0].StringOl(0, pbEkle0), dovizOdenek[sat, 0].StringOl(1, pbEkle0), dovizOdenek[sat, 0].StringOl(2, pbEkle0), dovizOdenek[sat++, 1].StringOl(0, pbEkle0));
            tabHeader.Rows.Add(String.Format("6- {0} Yılı Sonuna Kadar Tahmini Kümülatif Harcama = (2+5) x {1} yılı deflatör yüzdesi", butceYil - 1, butceYil), StringeCevir(kumulatifHarcamaBirOnce, 1), StringeCevir(kumulatifHarcamaBirOnce, 2), StringeCevir(kumulatifHarcamaBirOnce, 3), StringeCevir(kumulatifHarcamaBirOnce, 4), dovizOdenek[sat, 0].StringOl(0, pbEkle0), dovizOdenek[sat, 0].StringOl(1, pbEkle0), dovizOdenek[sat, 0].StringOl(2, pbEkle0), dovizOdenek[sat++, 1].StringOl(0, pbEkle0));
            tabHeader.Rows.Add(String.Format("7- {0} Yılı Yatırım Teklifi", butceYil), StringeCevir(teklif0, 1), StringeCevir(teklif0, 2), StringeCevir(teklif0, 3), StringeCevir(teklif0, 4), dovizOdenek[sat, 0].StringOl(0, pbEkle0), dovizOdenek[sat, 0].StringOl(1, pbEkle0), dovizOdenek[sat, 0].StringOl(2, pbEkle0), dovizOdenek[sat++, 1].StringOl(0, pbEkle0));
            tabHeader.Rows.Add(String.Format("8- {0} Yılı Yatırım Teklifi", butceYil + 1), StringeCevir(teklif1, 1), StringeCevir(teklif1, 2), StringeCevir(teklif1, 3), StringeCevir(teklif1, 4), dovizOdenek[sat, 0].StringOl(0, pbEkle0), dovizOdenek[sat, 0].StringOl(1, pbEkle0), dovizOdenek[sat, 0].StringOl(2, pbEkle0), dovizOdenek[sat++, 1].StringOl(0, pbEkle0));
            tabHeader.Rows.Add(String.Format("9- {0} Yılı Yatırım Teklifi", butceYil + 2), StringeCevir(teklif2, 1), StringeCevir(teklif2, 2), StringeCevir(teklif2, 3), StringeCevir(teklif2, 4), dovizOdenek[sat, 0].StringOl(0, pbEkle0), dovizOdenek[sat, 0].StringOl(1, pbEkle0), dovizOdenek[sat, 0].StringOl(2, pbEkle0), dovizOdenek[sat++, 1].StringOl(0, pbEkle0));
            tabHeader.Rows.Add(String.Format("10- {0} Dahil Bitiş Yılına Kadar Yatırım Teklifleri Toplamı", butceYil + 3), StringeCevir(projeKalan, 1), StringeCevir(projeKalan, 2), StringeCevir(projeKalan, 3), StringeCevir(projeKalan, 4), dovizOdenek[sat, 0].StringOl(0, pbEkle0), dovizOdenek[sat, 0].StringOl(1, pbEkle0), dovizOdenek[sat, 0].StringOl(2, pbEkle0), dovizOdenek[sat++, 1].StringOl(0, pbEkle0));
            tabHeader.Rows.Add(String.Format("11- Varsa {0} Yılı için ek ödenek teklifi (OVMP ödenek tavanı dışında) (*)", butceYil), StringeCevir(ek0, 1), StringeCevir(ek0, 2), StringeCevir(ek0, 3), StringeCevir(ek0, 4), "", "", "", "");
            tabHeader.Rows.Add(String.Format("12- Varsa {0} Yılı için ek ödenek teklifi (OVMP ödenek tavanı dışında) (*)", butceYil + 1), StringeCevir(ek1, 1), StringeCevir(ek1, 2), StringeCevir(ek1, 3), StringeCevir(ek1, 4), "", "", "", "");
            tabHeader.Rows.Add(String.Format("13- Varsa {0} Yılı için ek ödenek teklifi (OVMP ödenek tavanı dışında) (*)", butceYil + 2), StringeCevir(ek2, 1), StringeCevir(ek2, 2), StringeCevir(ek2, 3), StringeCevir(ek2, 4), "", "", "", "");
            doc.MailMerge.ExecuteWithRegions(tabHeader);
            doc.Save(this.sonucDosyaAdWrd);
            return this.sonucDosyaAdWrd;
        }

        private void ParaBirimleriniListeyeEkle(TNSCollection tc, BilgiVAN liste)
        {
            DovizOdenekler d_o = new DovizOdenekler();
            if (liste != null && liste.liste != null)
            {
                foreach (ButceKaynak kaynak in liste.liste)
                {
                    if (tc.IndexOf(kaynak.pbKisaltma) < 0)
                        tc.Add(kaynak.pbKisaltma);
                }
            }
        }

        private DovizOdenekler DovizOdenekTopla(params DovizOdenekler[] dovizOdenekler)
        {
            DovizOdenekler dovler = new DovizOdenekler();
            foreach (DovizOdenekler item in dovizOdenekler)
            {
                foreach (var ic in item.dovizler)
                {
                    dovler.Ekle(ic.kredi, ic.icKaynak, ic.paraBirimi);
                }
            }
            return dovler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bvKredi"></param>
        /// <param name="baslaYil"></param>
        /// <param name="bitisYil"></param>
        /// <param name="tur">0 ise anlaşma tutarı, 1 kullanılan tutar, 2 kullanılacak tutar</param>
        /// <returns></returns>
        private DovizOdenekler DovizOdenekOl(BilgiVAN liste, int baslaYil, int bitisYil, int tur)
        {
            DovizOdenekler d_o = new DovizOdenekler();
            if (liste != null && liste.liste != null)
            {
                foreach (ButceKaynak kaynak in liste.liste)
                {
                    if (kaynak.krediPlan != null)
                    {
                        foreach (KrediDetay kd in kaynak.krediPlan)
                        {
                            if (kd.yil >= baslaYil && kd.yil <= bitisYil)
                            {
                                if (tur == 0) d_o.Ekle(kd.anlasmaTutari, kd.anlasmaTutariIc, kaynak.pbKisaltma);
                                else if (tur == 1) d_o.Ekle(kd.kullanilanTutar, kd.kullanilanTutarIc, kaynak.pbKisaltma);
                                else if (tur == 2) d_o.Ekle(kd.kullanilacakTutar, kd.kullanilacakTutarIc, kaynak.pbKisaltma);
                            }
                        }
                    }
                }
            }
            return d_o;
        }

        private decimal DeflatorYuzdesi(Kullanici kullanan, IYATServis servis, int butceYil, string senaryoKod, string sektorKod)
        {
            return servis.DeflatorBul(kullanan, butceYil, senaryoKod, sektorKod);
            //return 1.05;
        }

        private ObjectArray Carp(ObjectArray gelen, decimal katsayi)
        {
            ObjectArray donen = new ObjectArray();
            for (int j = 0; j < gelen.ObjeSayisi; j++)
            {
                ProjeMaliYapisi pmy = (ProjeMaliYapisi)gelen.Al(j);
                donen.Ekle(new ProjeMaliYapisi(pmy, katsayi));
            }
            return donen;
        }

        private ObjectArray Topla(decimal katSayi, params ObjectArray[] gelenler)
        {
            ObjectArray donen = new ObjectArray();
            if (gelenler != null && gelenler.Length > 0)
            {
                for (int i = 0; i < gelenler.Length; i++)
                {
                    for (int j = 0; j < gelenler[i].ObjeSayisi; j++)
                    {
                        ProjeMaliYapisi pmy = (ProjeMaliYapisi)gelenler[i].Al(j);

                        ProjeMaliYapisi bulunan = PMYBul(donen, pmy);
                        if (bulunan == null)
                        {
                            donen.Ekle(new ProjeMaliYapisi(pmy, katSayi));
                        }
                        else
                        {
                            bulunan.Ekle(pmy, katSayi);
                        }
                    }
                }
            }
            return donen;
        }

        private ProjeMaliYapisi PMYBul(ObjectArray donen, ProjeMaliYapisi pmy)
        {
            if (donen == null) return null;
            for (int i = 0; i < donen.ObjeSayisi; i++)
            {
                ProjeMaliYapisi dizideki = (ProjeMaliYapisi)donen[i];
                if (dizideki.projeNo == pmy.projeNo && dizideki.altProjeNo == pmy.altProjeNo)
                    return dizideki;
            }
            return null;
        }

        private double ToplamAl(ObjectArray gelen, int cevrilecekAlan)
        {
            double donen = 0;
            for (int j = 0; j < gelen.ObjeSayisi; j++)
            {
                ProjeMaliYapisi hedef = (ProjeMaliYapisi)gelen[j];
                donen += hedef.DegerAl(cevrilecekAlan);
            }
            return donen;
        }

        private string StringeCevir(ObjectArray gelen, int cevrilecekAlan)
        {
            return (ToplamAl(gelen, cevrilecekAlan) / 1000).ToString("#,###");
            //;
            //string donen = "";
            //for (int j = 0; j < gelen.ObjeSayisi; j++)
            //{
            //    ProjeMaliYapisi hedef = (ProjeMaliYapisi)gelen[j];
            //    if (j > 0) donen += "\r\n";
            //    donen += (hedef.DegerAl(cevrilecekAlan) / 1000).ToString("#,###");
            //}
            //return donen;
        }

        private string OzetYazdir()
        {
            ProjeAltProjeOku();
            Aspose.Words.Document doc = new Aspose.Words.Document(sablonAd);
            DocumentBuilder builder = new DocumentBuilder(doc);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KurumAd", YatRaporAraclari.VarsayilanKurumBulAd());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYil", butceYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "EA", EABul(butceYil).ToLower());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYilSon", (butceYil + 2).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "SektorAd", proje.sektor.ad);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "BirimAd", BirimAdBul(false));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ProjeAd", proje.ad);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "AltProjeAd", AltProjeAdBul(false, true));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "DPTProjeNo", proje.DPTProjeNo);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "DevamYeni", DevamYeni());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "BitisDonemi", ProjeDonemi());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yeri", ProjeYeri());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ProjeBasYil", ProjeBasYil().ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ProjeBitYil", ProjeBitYil().ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KarakteristikOzet", KarakteristikOzet());
            TNSDateTime tnsDT = new TNSDateTime(DateTime.Now);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "sol", tnsDT.ToString() + " " + tnsDT.Saat.ToString("00") + ":" + tnsDT.Dakika.ToString("00"));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "sag", "SGB.net/" + senaryoKod);


            doc.Save(this.sonucDosyaAdWrd);
            return this.sonucDosyaAdWrd;
        }

        private string DevamYeni()
        {
            if (altProje != null)
                return TNS.YAT.Arac.EtudDevamEdenYeniAciklamaABC(altProje.EtudDevamEdenYeni(proje, butceYil));
            else
                return TNS.YAT.Arac.EtudDevamEdenYeniAciklamaABC(proje.EtudDevamEdenYeni(butceYil));
        }

        private string ProjeDonemi()
        {
            int bitYil = ProjeBitYil();
            if (bitYil == butceYil)
                return string.Format("{0}’D{1} BİTEN", butceYil, EABul(butceYil));
            else
                return string.Format("{0}’D{1}N SONRAYA KALAN", butceYil, EABul(butceYil));
        }

        private string EABul(int butceYil)
        {
            int sonBasamak = butceYil % 10;
            if (sonBasamak == 6 || sonBasamak == 9)
                return "A";
            else if (sonBasamak == 0)
            {
                sonBasamak = butceYil % 100;
                if (sonBasamak == 30 || sonBasamak == 40 || sonBasamak == 60 || sonBasamak == 90) return "A";
            }
            return "E";
        }

        private string ProjeYeri()
        {
            if (altProje != null)
                return altProje.ProjeYeri();
            else
                return proje.ProjeYeri();
        }

        private string KarakteristikOzet()
        {
            if (altProje != null)
                return altProje.KarakteristikOzet;
            else
                return proje.KarakteristikOzet;
        }

        private int ProjeBitYil()
        {
            if (altProje != null)
                return altProje.bitisYil;
            else
                return proje.bitisYili;

        }

        private int ProjeBasYil()
        {
            if (altProje != null)
                return altProje.baslangicYil;
            else
                return proje.baslangicYili;
        }

        //private void AlanaYaz(DocumentBuilder builder, string alanAd, string deger)
        //{
        //    if (deger == null) deger = "";
        //    while (builder.MoveToMergeField(alanAd))
        //        builder.Write(deger);

        //}

        private string KapakYazdir()
        {
            ProjeAltProjeOku();
            Aspose.Words.Document doc = new Aspose.Words.Document(sablonAd);
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToMergeField("ResimYer");

            builder.InsertImage(System.IO.Path.Combine(sablonYol, "YATD-KapakResim.png"), 195.8, 195.8);

            DataTable tabHeader = new DataTable();
            tabHeader.TableName = "Kapak";

            tabHeader.Columns.Add("KurumAd");
            tabHeader.Columns.Add("BirimAd");
            tabHeader.Columns.Add("SektorAd");
            tabHeader.Columns.Add("ProjeNo");
            tabHeader.Columns.Add("ProjeAd");
            tabHeader.Columns.Add("AltProjeAd");
            tabHeader.Columns.Add("ButceYil");
            tabHeader.Columns.Add("ButceYilSon");
            tabHeader.Columns.Add("HazirAyad");
            tabHeader.Columns.Add("HazirYil");
            string altProjeAd = "";
            if (altProje != null)
                altProjeAd = AltProjeAdBul(true, true);
            tabHeader.Rows.Add(YatRaporAraclari.VarsayilanKurumBulAd(), BirimAdBul(false), proje.sektor.ad, proje.DPTProjeNo, proje.ad, altProjeAd, butceYil.ToString(), (butceYil + 2).ToString(), GenelIslemler.AyAdDondur(DateTime.Now.Month), DateTime.Now.Year.ToString());

            doc.MailMerge.ExecuteWithRegions(tabHeader);

            doc.Save(this.sonucDosyaAdWrd);
            return this.sonucDosyaAdWrd;
        }

        private string AltProjeAdBul(bool parantez, bool altProjeAdlariDahil)
        {
            string donusDeger = "";
            string ac = "";
            string kapat = "";
            if (parantez)
            {
                ac = "(";
                kapat = ")";
            }
            if (altProje != null)
            {
                donusDeger = altProje.ad;
            }
            else if (proje != null)
            {
                if (altProjeAdlariDahil)
                {
                    foreach (TNS.YAT.AltProje item in proje.altProjeler)
                    {
                        if (!item.DPTDeVar) continue;
                        if (donusDeger != "") donusDeger += "\r\n";
                        donusDeger += (item.ad);
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(donusDeger))
                return ac + donusDeger + kapat;
            return "";
        }

        private string BirimAdBul(bool kodDahil)
        {
            string donusDeger = "";
            if (altProje != null)
            {
                string kod = "";
                if (kodDahil)
                    kod = altProje.birim.kod + "- ";
                donusDeger = kod + altProje.birim.ad;
            }
            //else if (altProjeler != null)
            //{
            //    foreach (TNS.YAT.AltProje item in altProjeler.liste)
            //    {
            //        string kod = "";
            //        if (kodDahil)
            //            kod = item.birim.kod + "- ";
            //        string tut = kod + item.birim.ad;
            //        if (donusDeger.IndexOf(tut) < 0)
            //        {
            //            if (donusDeger != "") donusDeger += "\r\n";
            //            donusDeger += tut;
            //        }
            //    }
            //}
            if (donusDeger == "" && proje != null)
            {
                string kod = "";
                if (kodDahil)
                    kod = proje.birim.kod + "- ";
                donusDeger = kod + proje.birim.ad;
            }
            return donusDeger;
        }

        //private void ProjeAltProjeleriOku()
        //{
        //    TNS.YAT.AltProje altProjeS = new TNS.YAT.AltProje();
        //    altProjeS.proje.kod = projeNo;
        //    altProjeler = servisYAT.AltProjeListele(kullanan, butceYil, senaryoKod, altProje, null, true, true);
        //    if (!altProjeler.sonuc.islemSonuc)
        //        throw new RaporHatasi(altProjeler.sonuc.hataStr);
        //}

        private void ProjeAltProjeOku()
        {
            ProjeAltProjeOku(servisYAT, kullanan, butceYil, senaryoKod, projeNo, altProjeNo, out altProje, out proje);
        }

        public static void ProjeAltProjeOku(IYATServis servisYAT, Kullanici kullanan, int butceYil, string senaryoKod, int projeNo, int altProjeNo, out TNS.YAT.AltProje altProje, out TNS.YAT.Proje proje)
        {
            if (altProjeNo > 0)
            {
                altProje = new TNS.YAT.AltProje();
                altProje.kod = altProjeNo;

                BilgiVAN bv = servisYAT.AltProjeListele(kullanan, butceYil, senaryoKod, altProje, null, true, true);
                if (!bv.sonuc.islemSonuc)
                    throw new RaporHatasi(bv.sonuc.hataStr);
                if (bv.liste == null || bv.liste.Count != 1)
                    throw new RaporHatasi("Aranan alt proje bulunamadı");
                altProje = (TNS.YAT.AltProje)bv.liste[0];
                projeNo = altProje.proje.kod;
            }
            else
            {
                altProje = null;
            }

            if (altProje == null && projeNo == 0)
                throw new RaporHatasi("Aranan proje kodu bulunamadı");

            if (projeNo > 0)
            {
                proje = new TNS.YAT.Proje();
                proje.kod = projeNo;
                BilgiVAN bv = servisYAT.ProjeListele(kullanan, butceYil, senaryoKod, proje, null, true);
                if (!bv.sonuc.islemSonuc)
                    throw new RaporHatasi(bv.sonuc.hataStr);
                if (bv.liste == null || bv.liste.Count != 1)
                    throw new RaporHatasi("Aranan proje bulunamadı");
                proje = (TNS.YAT.Proje)bv.liste[0];
            }
            else
                throw new RaporHatasi("Aranan proje kodu bulunamadı");
        }
    }

    public class ProjeIzlemeDegerlendirme
    {
        IYATServis servisYAT = TNS.YAT.Arac.Tanimla();
        IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();
        IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
        //IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
        //TNS.UZY.IUZYServis uzyServis = null;
        Kullanici kullanan = null;
        string sablonAd = string.Empty;
        string sonucDosyaAd = string.Empty;
        string sonucDosyaAdWrd = string.Empty;
        private int butceYil;
        private string senaryoKod;
        private int projeNo;
        private int altProjeNo;
        private AltProje altProje;
        private Proje proje;
        private string format;
        public Hashtable raporParam { get; set; }
        Aspose.Words.Document doc;
        DocumentBuilder builder;
        private int yidAsama;

        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {
            if (string.IsNullOrWhiteSpace(sablonAd))
                doc = new Aspose.Words.Document();
            else
                doc = new Aspose.Words.Document(sablonAd);
            builder = new DocumentBuilder(doc);
            this.kullanan = (Kullanici)kullanan;
            this.sablonAd = sablonAd;
            this.sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            this.raporParam = raporParam;
            this.format = null;
            if (raporParam["Format"] != null)
                this.format = raporParam["Format"].ToString();
            if (string.IsNullOrWhiteSpace(this.format))
                this.format = "pdf";
            this.sonucDosyaAdWrd = sonucDosyaAd + "." + this.format;

            this.butceYil = OrtakFonksiyonlar.ConvertToInt(raporParam["ButceYil"], 0);
            this.senaryoKod = "06.00";

            this.projeNo = OrtakFonksiyonlar.ConvertToInt(raporParam["ProjeNo"], 0);
            this.altProjeNo = OrtakFonksiyonlar.ConvertToInt(raporParam["AltProjeNo"], 0);
            this.yidAsama = OrtakFonksiyonlar.ConvertToInt(raporParam["YIDAsama"], 1);
            if (ekBilgi.Equals("ProjeDurum"))
                return ProjeDurumYazdir();
            else if (ekBilgi.Equals("ProjeDurumBirim"))
            {
                ProjeAltProjeOku();
                return ProjeDurumBirimYazdir(proje.yurutecekKurum);
            }
            else if (ekBilgi.Equals("ProjeIzleme"))
                return ProjeIzlemeYazdir();
            else if (ekBilgi.Equals("SektoreGore"))
                return SektoreGoreYazdir();
            else if (ekBilgi.Equals("EkoKodaGore"))
                return SektoreGoreYazdir();
            else if (ekBilgi.Equals("SektorEkoKodaGore"))
                return SektoreGoreYazdir();

            return "";
        }

        enum ENUMProjeGrupla
        {
            Sektor, SektorEko, Eko
        }

        class ProjeToplam
        {
            public string SektorKod;
            public string SektorAd;
            public string EkoKod;
            public string EkoAd;
            public int PSay;
            public double PrjTut;
            public double RevPrjTut;
            public double KBO;
            public double Revize;
            public double Harc;
            public double Oran
            {
                get
                {
                    return OrtakFonksiyonlar.YuzdeBul(Harc, Revize, 2);
                }
            }

            public string EkoKodAd { get { return EkoKod + "-" + EkoAd; } }

            public ProjeToplam()
            {

            }

            public ProjeToplam(string SektorKod, string SektorAd, string EkoKod, string EkoAd, int PSay, double PrjTut, double RevPrjTut, double KBO, double Revize, double Harc)
            {
                this.SektorKod = SektorKod;
                this.SektorAd = SektorAd;
                this.EkoKod = EkoKod;
                this.EkoAd = EkoAd;
                this.PSay = PSay;
                this.PrjTut = PrjTut;
                this.RevPrjTut = RevPrjTut;
                this.KBO = KBO;
                this.Revize = Revize;
                this.Harc = Harc;
            }

            internal void Ekle(ProjeToplam pt)
            {
                this.PSay += pt.PSay;
                this.PrjTut += pt.PrjTut;
                this.RevPrjTut += pt.RevPrjTut;
                this.KBO += pt.KBO;
                this.Revize += pt.Revize;
                this.Harc += pt.Harc;
            }
        }

        private string SektoreGoreYazdir()
        {
            Proje proje = new Proje();
            proje.siralama = 2;
            proje.aktiflik = 1;
            proje.sektor.kod = "";
            proje.birim.kod = "";
            ObjectArray oa = servisYAT.ProjeListele(kullanan, butceYil, senaryoKod, proje, false, true, 2);
            if (oa.sonuc.islemSonuc)
            {
                Tab1Yaz(oa);
                Tab2Yaz(oa);
                Tab3Yaz(oa);
            }
            else
            {
                OrtakFonksiyonlar.HataDosyaYaz("ListeleTum.txt", oa.sonuc.hataStr, false);
            }

            string sonucDosyaAd = DosyaIslem.DosyaAdUret() + ".docx";
            doc.Save(sonucDosyaAd);
            return sonucDosyaAd;
        }

        private void Tab1Yaz(ObjectArray oa)
        {
            SortedDictionary<string, ProjeToplam> ht = Grupla(oa, ENUMProjeGrupla.Sektor);

            DataTable tabHeader = new DataTable();
            tabHeader.TableName = "Tab";

            tabHeader.Columns.Add("Sektor");
            tabHeader.Columns.Add("PSay");
            tabHeader.Columns.Add("PrjTut");
            tabHeader.Columns.Add("RevPrjTut");
            tabHeader.Columns.Add("KBO");
            tabHeader.Columns.Add("Revize");
            tabHeader.Columns.Add("Harc");
            tabHeader.Columns.Add("Oran");

            foreach (KeyValuePair<string, ProjeToplam> item in ht)
            {
                ProjeToplam pt = item.Value;
                tabHeader.Rows.Add(pt.SektorAd, StringeCevir(pt.PSay), StringeCevir1000(pt.PrjTut), StringeCevir1000(pt.RevPrjTut), StringeCevir1000(pt.KBO), StringeCevir1000(pt.Revize), StringeCevir1000(pt.Harc), StringeCevir(pt.Oran));
            }
            doc.MailMerge.ExecuteWithRegions(tabHeader);
        }

        private void Tab2Yaz(ObjectArray oa)
        {
            DataTable tabHeader = new DataTable();
            tabHeader.TableName = "Tab2";

            tabHeader.Columns.Add("Eko");
            tabHeader.Columns.Add("KBO");
            tabHeader.Columns.Add("Revize");
            tabHeader.Columns.Add("Harc");
            tabHeader.Columns.Add("Oran");

            SortedDictionary<string, ProjeToplam> ht = Grupla(oa, ENUMProjeGrupla.Eko);
            foreach (KeyValuePair<string, ProjeToplam> item in ht)
            {
                ProjeToplam pt = item.Value;
                tabHeader.Rows.Add(pt.EkoKodAd, StringeCevir1000(pt.KBO), StringeCevir1000(pt.Revize), StringeCevir1000(pt.Harc), StringeCevir(pt.Oran));
            }
            doc.MailMerge.ExecuteWithRegions(tabHeader);
        }

        private void Tab3Yaz(ObjectArray oa)
        {
            DataTable tabHeader = new DataTable();
            tabHeader.TableName = "Tab3";

            tabHeader.Columns.Add("Sektor");
            tabHeader.Columns.Add("Eko");
            tabHeader.Columns.Add("KBO");
            tabHeader.Columns.Add("Revize");
            tabHeader.Columns.Add("Harc");
            tabHeader.Columns.Add("Oran");

            SortedDictionary<string, ProjeToplam> ht = Grupla(oa, ENUMProjeGrupla.SektorEko);
            foreach (KeyValuePair<string, ProjeToplam> item in ht)
            {
                ProjeToplam pt = item.Value;
                tabHeader.Rows.Add(pt.SektorAd, pt.EkoKodAd, StringeCevir1000(pt.KBO), StringeCevir1000(pt.Revize), StringeCevir1000(pt.Harc), StringeCevir(pt.Oran));
            }
            doc.MailMerge.ExecuteWithRegions(tabHeader);
            Table tbl = doc.FirstSection.Body.Tables[2];

            Cell anaHucre = null;

            string oncekiVeri = "";
            for (int i = 2; i < tabHeader.Rows.Count + 2; i++)
            {
                Cell aktifHucre = tbl.Rows[i].Cells[0];
                string aktifHucreVeri = aktifHucre.GetText();
                if (anaHucre != null && oncekiVeri == aktifHucreVeri)
                {
                    VerticallyMergeCells(anaHucre, aktifHucre);
                }
                else
                {
                    anaHucre = aktifHucre;
                    oncekiVeri = aktifHucreVeri;
                }
            }
        }

        private static void VerticallyMergeCells(Cell c1, Cell c2)
        {
            if (c1 == null || c2 == null) return;
            c1.CellFormat.VerticalMerge = CellMerge.First;
            c2.CellFormat.VerticalMerge = CellMerge.Previous;
        }


        public static string StringeCevir(double deger)
        {
            return StringeCevirBol(deger, 1, "#,##0");
        }

        public static string StringeCevirBol(double deger, double bolum, string format)
        {
            return (deger / bolum).ToString(format);
        }

        public static string StringeCevir1000(double deger)
        {
            return StringeCevirBol(deger, 1000, "#,##0");
        }

        public static string StringeCevir(int deger)
        {
            return deger.ToString();
        }

        string[] ekoAdlar = { "PERSONEL GİDERLERİ", "SOSYAL GÜVENLİK KURUMLARINA DEVLET PRİMİ GİDERLERİ", "MAL VE HİZMET ALIM GİDERLERİ", "FAİZ  GİDERLERİ", "CARİ TRANSFERLER", "SERMAYE GİDERLERİ", "SERMAYE TRANSFERLERİ", "BORÇ VERME", "YEDEK ÖDENEKLER" };

        private SortedDictionary<string, ProjeToplam> Grupla(ObjectArray oa, ENUMProjeGrupla grupKod)
        {
            SortedDictionary<string, ProjeToplam> sd = new SortedDictionary<string, ProjeToplam>();
            double prjTut, revPrjTut, KBO, revizeOdenek, harcama;
            foreach (Proje p in oa.objeler)
            {
                if (grupKod == ENUMProjeGrupla.Sektor)
                {
                    p.ProjeBilgiBul(out prjTut, out revPrjTut, out KBO, out revizeOdenek, out harcama, "", butceYil);
                    SDEkle(sd, p.sektor.kod, new ProjeToplam(p.sektor.kod, p.sektor.ad, "", "", 1, prjTut, revPrjTut, KBO, revizeOdenek, harcama));
                }
                else
                {
                    bool ilk = true;
                    for (int i = 1; i < 10; i++)
                    {
                        string ekoKod = i.ToString("00");
                        p.ProjeBilgiBul(out prjTut, out revPrjTut, out KBO, out revizeOdenek, out harcama, ekoKod, butceYil);
                        if (KBO > 0 || revizeOdenek > 0 || harcama > 0)
                        {
                            SDEkle(sd, grupKod == ENUMProjeGrupla.SektorEko ? p.sektor.kod + "-" + ekoKod : ekoKod, new ProjeToplam(p.sektor.kod, p.sektor.ad, ekoKod, ekoAdlar[i - 1], ilk ? 1 : 0, prjTut, revPrjTut, KBO, revizeOdenek, harcama));
                            ilk = false;
                        }
                    }
                }
            }
            return sd;
        }

        private void SDEkle(SortedDictionary<string, ProjeToplam> sd, string hashKod, ProjeToplam pt)
        {
            if (hashKod != null && pt != null)
            {
                ProjeToplam eski;
                try
                {
                    eski = sd[hashKod];
                    eski.Ekle(pt);
                }
                catch
                {
                    sd[hashKod] = pt;
                }
            }
        }

        private string ProjeDurumBirimYazdir(string birimKod)
        {
            TNS.YAT.Proje projeYen = new TNS.YAT.Proje();
            projeYen.kod = 0;
            BilgiVAN bv = servisYAT.ProjeListele(kullanan, butceYil, senaryoKod, projeYen, birimKod, false);
            int say = 0;
            doc = null;
            foreach (Proje item in bv.liste)
            {
                System.Collections.Hashtable raporParam = new System.Collections.Hashtable();
                raporParam["Senaryo"] = senaryoKod;
                raporParam["ButceYil"] = butceYil.ToString();
                raporParam["ProjeNo"] = item.kod.ToString();
                raporParam["AltProjeNo"] = "0";
                raporParam["Format"] = "docx";
                raporParam["YIDAsama"] = this.yidAsama.ToString();

                string uretilenDosya = YatProjeRaporlari.TekRaporUret(2004, 14, kullanan, null, raporParam);
                if (doc == null)
                    doc = new Aspose.Words.Document(uretilenDosya);
                else
                {
                    // Open the document to join.
                    Aspose.Words.Document srcDoc = new Aspose.Words.Document(uretilenDosya);
                    // Append the source document at the end of the destination document.
                    doc.AppendDocument(srcDoc, Aspose.Words.ImportFormatMode.UseDestinationStyles);
                    doc.Sections[say + 1].PageSetup.SectionStart = Aspose.Words.SectionStart.Continuous;
                    srcDoc = null;
                    say++;
                }
                try
                {
                    System.IO.File.Delete(uretilenDosya);
                }
                catch { }
            }

            doc.UpdateFields();
            string sonucDosyaAd = DosyaIslem.DosyaAdUret() + ".docx";
            doc.Save(sonucDosyaAd);
            return sonucDosyaAd;
        }

        private string ProjeIzlemeYazdir()
        {
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KurumAd", YatRaporAraclari.VarsayilanKurumBulAd());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYil", butceYil.ToString());
            ////doc.CustomDocumentProperties.Add("Yil", butceYil.ToString());

            ObjectArray oa = servisKYM.BuyukMetinListele(kullanan, "YAT", YatIzlemeAraclari.AnahtarBulGenel(butceYil, yidAsama, ""), "tr");

            YatRaporAraclari.AsposeAlanaYazHtml(builder, "FinansKaynak", ProjeIzlemeDegerlendirme.MetinBul(oa, YatIzlemeAraclari.AnahtarBulGenel(butceYil, yidAsama, "FinansKaynak")));
            YatRaporAraclari.AsposeAlanaYazHtml(builder, "HizmetKapasitesi", ProjeIzlemeDegerlendirme.MetinBul(oa, YatIzlemeAraclari.AnahtarBulGenel(butceYil, yidAsama, "HizmetKapasitesi")));
            YatRaporAraclari.AsposeAlanaYazHtml(builder, "KapasiteArtisi", ProjeIzlemeDegerlendirme.MetinBul(oa, YatIzlemeAraclari.AnahtarBulGenel(butceYil, yidAsama, "KapasiteArtisi")));
            YatRaporAraclari.AsposeAlanaYazHtml(builder, "YatirimUygulamalari", ProjeIzlemeDegerlendirme.MetinBul(oa, YatIzlemeAraclari.AnahtarBulGenel(butceYil, yidAsama, "YatirimUygulamalari")));
            YatRaporAraclari.AsposeAlanaYazHtml(builder, "Notlar", ProjeIzlemeDegerlendirme.MetinBul(oa, YatIzlemeAraclari.AnahtarBulGenel(butceYil, yidAsama, "Notlar")));

            YatRaporAraclari.AsposeAlanaYazHtml(builder, "SorunCozum", ProjeIzlemeDegerlendirme.MetinBul(oa, YatIzlemeAraclari.AnahtarBulGenel(butceYil, yidAsama, "SorunCozum")));

            proje = new TNS.YAT.Proje();
            proje.kod = 0;

            BilgiVAN bv = servisYAT.ProjeListele(kullanan, butceYil, senaryoKod, proje, null, false);
            int say = 0;
            foreach (Proje item in bv.liste)
            {
                System.Collections.Hashtable raporParam = new System.Collections.Hashtable();
                raporParam["Senaryo"] = senaryoKod;
                raporParam["ButceYil"] = butceYil.ToString();
                raporParam["ProjeNo"] = item.kod.ToString();
                raporParam["AltProjeNo"] = "0";
                raporParam["Format"] = "docx";
                raporParam["YIDAsama"] = this.yidAsama.ToString();

                try
                {
                    string uretilenDosya = YatProjeRaporlari.TekRaporUret(2004, 14, kullanan, null, raporParam);
                    // Open the document to join.
                    Aspose.Words.Document srcDoc = new Aspose.Words.Document(uretilenDosya);
                    // Append the source document at the end of the destination document.
                    doc.AppendDocument(srcDoc, Aspose.Words.ImportFormatMode.UseDestinationStyles);
                    doc.Sections[say + 1].PageSetup.SectionStart = Aspose.Words.SectionStart.Continuous;
                    srcDoc = null;
                    say++;
                    try
                    {
                        System.IO.File.Delete(uretilenDosya);
                    }
                    catch { }
                }
                catch {
                    builder.Writeln($"******{item.kod} kodlu proje yazdırlamadı ********");
                }
            }

            doc.UpdateFields();
            string sonucDosyaAd = DosyaIslem.DosyaAdUret() + ".docx";
            doc.Save(sonucDosyaAd);
            return sonucDosyaAd;
        }

        private string ProjeDurumYazdir()
        {
            ProjeAltProjeOku();

            if (proje != null && proje.altProjeler != null)
            {
                ObjectArray oa = servisKYM.BuyukMetinListele(kullanan, "YAT", "YID" + butceYil.ToString() + "_" + projeNo.ToString() + "_", "tr");
                ProjeDurumProjeYaz(proje, oa);
                foreach (AltProje item in proje.altProjeler)
                {
                    AltProjeDurumProjeYaz(proje, item, oa);
                }
            }
            string sonucDosyaAd = DosyaIslem.DosyaAdUret() + ".docx";
            try
            {
                doc.Save(sonucDosyaAd);
            }
            catch {

                doc.RemoveAllChildren();

                // Yeni bölüm ve paragraf ekle
                Section section = new Section(doc);
                doc.AppendChild(section);

                Body body = new Body(doc);
                section.AppendChild(body);

                Paragraph para = new Paragraph(doc);
                body.AppendChild(para);

                // Builder ile yazı ekle
                DocumentBuilder builder = new DocumentBuilder(doc);
                builder.Writeln($"*********** {butceYil.ToString()}-{projeNo.ToString()} projesine ait verilerde hata olduğundan kaydedilemedi. **********");

                // Tekrar kaydet
                doc.Save(sonucDosyaAd);
            }
            return sonucDosyaAd;
        }

        //private void StilParagrafDuzenle(Style proje, bool IsQuickStyle, OutlineLevel outlineLevel, ParagraphAlignment paragraphAlignment)
        //{
        //    proje.IsQuickStyle = IsQuickStyle;
        //    proje.ParagraphFormat.OutlineLevel = outlineLevel;
        //    proje.ParagraphFormat.KeepTogether = true;
        //    proje.ParagraphFormat.KeepWithNext = true;
        //    proje.ParagraphFormat.SpaceBefore = 12;
        //    proje.ParagraphFormat.SpaceAfter = 6;
        //    proje.ParagraphFormat.Alignment = paragraphAlignment;
        //}

        //private void StilDuzenle(Style style, string fontName, int fontBuyukluk, Color color)
        //{
        //    style.Font.ClearFormatting();
        //    if (!string.IsNullOrWhiteSpace(fontName))
        //        style.Font.Name = fontName;
        //    style.Font.Size = fontBuyukluk;
        //    if (color != Color.Empty)
        //        style.Font.Color = color;
        //    style.Font.Bold = true;
        //}

        private void AltProjeDurumProjeYaz(Proje proje, AltProje altProje, ObjectArray oa)
        {
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading2;
            builder.Writeln(altProje.ad);
            builder.ParagraphFormat.StyleName = "IcBaslikAP";
            builder.Writeln("a) Ödenek ve Harcama Durumu:");
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;
            builder_InsertHtml(builder, YatIzlemeAraclari.OdenekTabloOl(servisBHMv2, kullanan, butceYil, YatIzlemeAraclari.AyOl(this.yidAsama), proje.kod, altProje.kod, false));

            builder.ParagraphFormat.StyleName = "IcBaslikAP";
            builder.Writeln("b) Program ve Gerçekleşme Durumu:");
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;
            builder_InsertHtml(builder, MetinBul(oa, YatIzlemeAraclari.AnahtarBul(butceYil, proje.kod, altProje.kod, this.yidAsama, "Gerekce")));
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;
            builder.Writeln("");
        }

        private void ProjeDurumProjeYaz(Proje proje, ObjectArray oa)
        {
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
            builder.Writeln(proje.DPTProjeNo + " - " + proje.ad);
            builder.ParagraphFormat.StyleName = "IcBaslik";
            builder.ParagraphFormat.Alignment = Aspose.Words.ParagraphAlignment.Left;
            builder.Writeln("a) Ödenek ve Harcama Durumu:");
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;
            builder_InsertHtml(builder, YatIzlemeAraclari.OdenekTabloOl(servisBHMv2, kullanan, butceYil, YatIzlemeAraclari.AyOl(this.yidAsama), proje.kod, 0, false));

            builder.ParagraphFormat.StyleName = "IcBaslik";
            builder.Writeln("b) Program ve Gerçekleşme Durumu:");
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;
            builder_InsertHtml(builder, MetinBul(oa, YatIzlemeAraclari.AnahtarBul(butceYil, proje.kod, 0, this.yidAsama, "Gerekce")));
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;
            builder.Writeln("");
        }

        private void builder_InsertHtml(DocumentBuilder builder, string html)
        {
            try
            {
                builder.InsertHtml(html);
            }
            catch 
            {
                builder.Writeln(html);
            }
        }

        public static string MetinBul(ObjectArray oa, string kod)
        {
            int tut = oa.objeler.IndexOf(kod);
            if (tut >= 0)
                return ((StrKodStrAd)oa.objeler[tut]).ad;
            else
                return "";
        }

        private void ProjeAltProjeOku()
        {
            ProjeDosyasiRaporlari.ProjeAltProjeOku(servisYAT, kullanan, butceYil, senaryoKod, projeNo, altProjeNo, out altProje, out proje);
        }
    }
}
