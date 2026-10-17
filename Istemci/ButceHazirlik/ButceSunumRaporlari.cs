using System;
using System.Collections.Generic;
using System.Linq;
using TNS.Raporlama;
using OrtakClass;
using TNS.KYM;
using TNS.BHMv2;
using TNS.UZY;
using TNS;
using TNS.BKD;
using System.Data;
using Aspose.Words;
using System.Collections;
using Aspose.Words.Tables;
using System.Globalization;

namespace ButceHazirlikv2
{
    /// <summary>
    /// Bütçe sunum raporları (Rapor.xml NESNE "ButceHazirlikv2.ButceSunumRaporlari"; EKBILGI: Yil, YilTklf, BrmTklf, Prg, Sektor,
    /// Birim, Talep, Yeni, TarimDestek, DigerCari, CariEk, Destek, DestekPrg, GercEko1, Toplu).
    /// Eski ButceHazirlikv2/ButceSunumRaporlari.cs'ten alındı; değişenler: System.Web ve IRapor arayüzü kaldırıldı,
    /// çıktı dosyası sonucDosyaAd + ".docx" (boşsa eskisi gibi DosyaIslem.DosyaAdUret ile geçici klasörde) olarak üretilir,
    /// tanınmayan ek bilgide "HATA: ..." döner.
    /// </summary>
    public class ButceSunumRaporlari
    {
        private Kullanici kullanan;

        private int butceYil;
        private string senaryoKod;
        private string senaryoKodOnc;
        private string EkoKod;
        private string EkoAd;
        private string Eko;
        private string KurSec;
        private Hashtable raporParam;
        string kurumKod;
        Document doc;
        DocumentBuilder builder;
        int ay;
        string ciktiAd;
        IUZYServis uzyServis;
        string tasraKod = "0067";
        private string StrYap(double dbl, int kusurat = 0)
        {
            string fmt = "#,##0";
            if (kusurat > 0)
            {
                fmt += ".";
                for (int i = 0; i < kusurat; i++)
                {
                    fmt += "0";
                }
            }
            return dbl.ToString(fmt);
        }

        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            kullanan = (Kullanici)o_kullanan;
            uzyServis = TNS.UZY.Arac.Tanimla();
            kurumKod = TNS.DEG.Arac.VarsayilanKurumBul();
            this.butceYil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            this.senaryoKod = (string)raporParam["Senaryo"];
            this.senaryoKodOnc = (string)raporParam["SenaryoOnc"];
            this.EkoKod = (string)raporParam["EkoKod"];
            this.EkoAd = EkoAdBul(this.EkoKod, (string)raporParam["EkoAd"]);
            this.Eko = (string)raporParam["Eko"];
            this.KurSec = (string)raporParam["KurSec"];
            this.raporParam = raporParam;

            doc = new Document(sablonAd);
            builder = new DocumentBuilder(doc);

            if (!string.IsNullOrEmpty(sonucDosyaAd))
                ciktiAd = sonucDosyaAd + ".docx";
            else
                ciktiAd = DosyaIslem.DosyaAdUret(System.IO.Path.GetFileNameWithoutExtension(sablonAd), "") + ".docx";
            if (butceYil == DateTime.Now.Year)
            {
                ay = DateTime.Now.Month;
                if (DateTime.Now.Day == 1 && ay > 1)
                    ay--;
            }
            else
                ay = 12;

            if (ekBilgi == "Birim")
            {
                BirimRapor();
            }
            else if (ekBilgi == "Talep")
            {
                TalepRapor();
            }
            else if (ekBilgi == "Yeni")
            {
                YeniRapor();
            }
            else if (ekBilgi == "Yil")
            {
                YillaraGoreRapor();
            }
            else if (ekBilgi == "YilTklf")
            {
                YilTeklifiRapor();
            }
            else if (ekBilgi == "BrmTklf")
            {
                BirimTeklifiRapor();
            }
            else if (ekBilgi == "Prg")
            {
                ProgramYapisiRapor();
            }
            else if (ekBilgi == "Sektor")
            {
                SektorRapor();
            }
            else if (ekBilgi == "TarimDestek")
            {
                TarimDestekRapor();
            }
            else if (ekBilgi == "DigerCari")
            {
                DigerCariRapor();
            }
            else if (ekBilgi == "CariEk")
            {
                CariEkRapor();
            }
            else if (ekBilgi == "Destek")
            {
                DestekRapor(true);
            }
            else if (ekBilgi == "DestekPrg")
            {
                DestekRapor(false);
            }
            else if (ekBilgi == "Toplu")
            {
                TopluRapor();
            }
            else if (ekBilgi == "GercEko1")
            {
                GercEko1();
            }
            else
            {
                return "HATA: Tanınmayan ek bilgi: " + ekBilgi;
            }

            return ciktiAd;
        }

        private void GercEko1()
        {
            DataTable tabTeklif = new DataTable();
            tabTeklif.TableName = "T";
            tabTeklif.Columns.Add("K");
            tabTeklif.Columns.Add("Aciklama");
            tabTeklif.Columns.Add("KBO");
            tabTeklif.Columns.Add("SonDurum");
            tabTeklif.Columns.Add("Harcama");
            tabTeklif.Columns.Add("P");

            double topKbo = 0;
            double topSonDurum = 0;
            double topHarcama = 0;

            double hTopKbo = 0;
            double hTopSonDurum = 0;
            double hTopHarcama = 0;

            Tertip kistasTertip = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", "");

            Tertip kistasTertipTD = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul() + "/" + TNS.DEG.Arac.VarsayilanKurumBul() + "." + tasraKod, "", "", "05.04");
            Tertip kistasTertipBK05 = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", "05", "23.05");
            Tertip kistasTertipBK07 = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", "07", "23.05");

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 1));

            EkDegerParam[] ekBilgiler = new EkDegerParam[3];

            if (butceYil == DateTime.Now.Year)
            {
                ay = DateTime.Now.Month;
                if (DateTime.Now.Day == 1 && ay > 1)
                    ay--;
            }
            else
                ay = 12;

            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod);
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABMasrafPlaniForm, 0, 0, ";;" + TNS.BHMv2.Arac.EklemeDusmeIslemler() + ";;");
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPEBHarcama, 0, 0, ay.ToString());

            OdenekBilgi[] obler = TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, true);


            OdenekBilgi[] oblerTD = TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, kistasTertipTD, kodBilgileri, ekBilgiler, true);
            OdenekBilgi[] oblerBK05 = TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, kistasTertipBK05, kodBilgileri, ekBilgiler, true);
            OdenekBilgi[] oblerBK07 = TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, kistasTertipBK07, kodBilgileri, ekBilgiler, true);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil", butceYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "AyAd", OrtakFonksiyonlar.AyAdiGetir(ay));

            if (obler.Length == 0)
            {
                tabTeklif.Rows.Add("", "", "", "", "", "", "");
            }
            else
            {
                for (int i = 0; i < obler.Length; i++)
                {
                    OdenekBilgi f = obler[i];
                    double kbo = ((double)f.EkRakam(0));
                    double sonDurum = ((double)f.EkRakam(1)) + kbo;
                    double harcama = ((double)f.EkRakam(2));
                    topKbo += kbo;
                    topSonDurum += sonDurum;
                    topHarcama += harcama;

                    string kod = f.tertip.kodlar[3];
                    TeklifeGercSatirEkle(tabTeklif, kod, f.tertip.adlar[3], kbo, sonDurum, harcama);
                    if (kod == "05")
                    {
                        TeklifeGercSatirEkle(tabTeklif, "Tarımsal Desteklemeler", oblerTD);
                        TeklifeGercSatirEkleDiger(tabTeklif, "Diğer Transferler (Afet Dahil)", f, oblerTD, oblerBK05);
                        TeklifeGercSatirEkle(tabTeklif, "Bağlı Kuruluşlara Transferler", oblerBK05);
                        BagliToplamaEkle(oblerBK05, ref hTopKbo, ref hTopSonDurum, ref hTopHarcama);
                    }
                    else if (kod == "07")
                    {
                        TeklifeGercSatirEkle(tabTeklif, "Bağlı Kuruluşlara Sermaye Transferleri", oblerBK07);
                        TeklifeGercSatirEkleDiger(tabTeklif, "Diğer Sermaye Transferleri", f, oblerBK07);
                        BagliToplamaEkle(oblerBK07, ref hTopKbo, ref hTopSonDurum, ref hTopHarcama);
                    }
                }
            }
            doc.MailMerge.ExecuteWithRegions(tabTeklif);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TKBO", StrYap(topKbo / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TSonDurum", StrYap(topSonDurum / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "THarcama", StrYap(topHarcama / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TP", PayHesapla(topHarcama, topSonDurum));

            topKbo -= hTopKbo;
            topSonDurum -= hTopSonDurum;
            topHarcama -= hTopHarcama;

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HTKBO", StrYap(topKbo / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HTSonDurum", StrYap(topSonDurum / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HTHarcama", StrYap(topHarcama / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HTP", PayHesapla(topHarcama, topSonDurum));

            Aspose.Words.Tables.Table tbl = doc.FirstSection.Body.Tables[0];
            for (int i = 1; i < tbl.Rows.Count - 1; i++)
            {
                Cell simCell = tbl.Rows[i].Cells[0];
                string simdBirim = simCell.GetText();
                if (simdBirim.Length < 2)
                {
                    for (int c = 0; c < 6; c++)
                    {
                        Cell fCell = tbl.Rows[i].Cells[c];
                        if (fCell != null)
                        {
                            NodeCollection runs = fCell.GetChildNodes(NodeType.Run, true);
                            foreach (Run run in runs)
                            {
                                run.Font.Bold = false;
                            }
                        }
                    }
                }
            }
            doc.Save(ciktiAd);
        }

        private void TeklifeGercSatirEkleDiger(DataTable tabTeklif, string aciklama, OdenekBilgi anaSatir, params OdenekBilgi[][] cikacaklar)
        {
            double kbo = 0;
            double eklenen = 0;
            double harcanan = 0;

            DegerleriBulOB(anaSatir, ref kbo, ref eklenen, ref harcanan, 1);
            for (int i = 0; i < cikacaklar.Length; i++)
            {
                DegerleriBul(cikacaklar[i], ref kbo, ref eklenen, ref harcanan, -1);
            }

            TeklifeGercSatirEkle(tabTeklif, "", aciklama, kbo, kbo + eklenen, harcanan);
        }


        private void TeklifeGercSatirEkle(DataTable tabTeklif, string aciklama, OdenekBilgi[] oblerTD)
        {
            double kbo = 0;
            double eklenen = 0;
            double harcama = 0;
            DegerleriBul(oblerTD, ref kbo, ref eklenen, ref harcama, 1);
            TeklifeGercSatirEkle(tabTeklif, "", aciklama, kbo, kbo + eklenen, harcama);
        }

        private void TeklifeGercSatirEkle(DataTable tabTeklif, string kod, string aciklama, double kbo, double sonDurum, double harcama)
        {
            tabTeklif.Rows.Add(kod, aciklama, StrYap(kbo / 1000), StrYap(sonDurum / 1000), StrYap(harcama / 1000), PayHesapla(harcama, sonDurum));
        }

        private string EkoAdBul(string ekoKod, string def)
        {
            if (ekoKod == "06") return "SERMAYE GİDERLERİ (PROJE)";
            else if (ekoKod == "07") return "SERMAYE TRANSFERİ";
            else if (ekoKod == "08") return "BORÇ VERME";
            else return def;
        }

        RaporBilgi[] raporNolar = new RaporBilgi[]
            { new RaporBilgi(4, "")
            , new RaporBilgi(5, "")
            , new RaporBilgi(6,""), new RaporBilgi(7, "")
            , new RaporBilgi(8,"06"), new RaporBilgi(8, "07"), new RaporBilgi(8, "08")
            , new RaporBilgi(1,"06"), new RaporBilgi(1, "07"), new RaporBilgi(1, "08")
            , new RaporBilgi(2,"06"), new RaporBilgi(2, "07"), new RaporBilgi(2, "08")
            , new RaporBilgi(3,"06"), new RaporBilgi(3, "07"), new RaporBilgi(3, "08")
            , new RaporBilgi(9, "")
            , new RaporBilgi(10, "")
            , new RaporBilgi(11, "")
        };

        private void TopluRapor()
        {
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil", butceYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "RaporTarih", DateTime.Now.Date.ToString("dd MMMM yyyy"));

            for (int i = 0; i < raporNolar.Length; i++)
            {
                try
                {
                    raporParam["EkoKod"] = raporNolar[i].ekoKod;
                    string raporAd = RaporlamaServis.TekRaporUret(20001, raporNolar[i].raporNo, kullanan, null, raporParam);
                    GenelIslemlerIstemci.AsposeAlanaDocEkle(builder, "tablo" + raporNolar[i].TabloAd, raporAd);
                    System.IO.File.Delete(raporAd);
                }
                catch (Exception ex)
                {
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "tablo" + raporNolar[i].TabloAd, ex.Message);
                }
            }
            doc.Save(ciktiAd);
        }

        private void CariEkRapor()
        {
            DataTable tabTeklif = new DataTable();
            tabTeklif.TableName = "T";
            tabTeklif.Columns.Add("Birim");
            tabTeklif.Columns.Add("Aciklama");
            tabTeklif.Columns.Add("EkTeklif");

            DataTable tabTasra = new DataTable();
            tabTasra.TableName = "T2";
            tabTasra.Columns.Add("Birim");
            tabTasra.Columns.Add("Aciklama");
            tabTasra.Columns.Add("EkTeklif");

            double topEkTeklif = 0;
            double topEkTeklifTas = 0;
            string tasraAd = "TAŞRA BİRİMLERİ";
            BirimleriEkle(tabTeklif, true, "01,02,03", ref topEkTeklif, ref tasraAd);
            BirimleriEkle(tabTasra, false, "01,02", ref topEkTeklifTas, ref tasraAd);
            BirimleriEkle(tabTasra, false, "03", ref topEkTeklifTas, ref tasraAd);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil", butceYil.ToString());

            if (tabTeklif.Rows.Count == 0)
                tabTeklif.Rows.Add("", "", "");
            doc.MailMerge.ExecuteWithRegions(tabTeklif);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TEkTeklif", StrYap(topEkTeklif / 1000));

            if (tabTasra.Rows.Count == 0)
                tabTasra.Rows.Add("", "", "");
            doc.MailMerge.ExecuteWithRegions(tabTasra);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "T2EkTeklif", StrYap(topEkTeklifTas / 1000));

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TTEkTeklif", StrYap((topEkTeklifTas + topEkTeklif) / 1000));

            doc.Save(ciktiAd);
        }

        private void BirimleriEkle(DataTable tabTeklif, bool merkezMi, string ekoKisit, ref double topEkTeklif, ref string tasraAd)
        {
            string kurKod = TNS.DEG.Arac.VarsayilanKurumBul();
            if (!merkezMi)
                kurKod += "." + tasraKod;
            Tertip kistasTertip = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, kurKod, "", "", ekoKisit);
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(0), 2));

            EkDegerParam[] ekBilgiler = new EkDegerParam[1];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBTavanUstu, 0, 0, senaryoKod);
            OdenekBilgi[] obler = TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, true);

            if (merkezMi)
            {
                for (int i = 0; i < obler.Length; i++)
                {
                    OdenekBilgi f = obler[i];
                    if (!f.tertip.kodlar[0].EndsWith(tasraKod))
                    {
                        double ekTeklif = ((double)f.EkRakam(0));
                        topEkTeklif += ekTeklif;
                        tabTeklif.Rows.Add(KisaAdBul(f.tertip.kodlar[0], f.tertip.adlar[0]), "", StrYap(ekTeklif / 1000));
                    }
                }
            }
            else
            {
                if (obler.Length == 1)
                {
                    OdenekBilgi f = obler[0];
                    double ekTeklif = ((double)f.EkRakam(0));
                    topEkTeklif += ekTeklif;
                    string kisaAd;
                    if (ekoKisit == "03")
                        kisaAd = "Su, elektrik, akaryakıt, iş sağlığı ve güvenliği, koruyucu gıda yardımı, hizmet binası ve araç kiralama, geçici ve seyyar görev yolluğu, bakım onarım, temsil ağırlama giderleri";
                    else
                        kisaAd = "Personel giderleri, sosyal güvenlik primi ödemeleri, stajyer öğrenci";
                    tabTeklif.Rows.Add(tasraAd, kisaAd, StrYap(ekTeklif / 1000));
                    tasraAd = "";
                }
            }
        }

        private string KisaAdBul(string kurKod, string varsayilan)
        {
            string tut = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", butceYil.ToString() + "-" + kurKod, true, "KURAD2");
            if (string.IsNullOrEmpty(tut))
                return varsayilan;
            else
                return tut;
        }

        private void DigerCariRapor()
        {
            DataTable tabTeklif = new DataTable();
            tabTeklif.TableName = "T";
            tabTeklif.Columns.Add("Aciklama");
            tabTeklif.Columns.Add("OncKBO");
            tabTeklif.Columns.Add("BTeklif");
            tabTeklif.Columns.Add("EkTeklif");
            tabTeklif.Columns.Add("TT");
            tabTeklif.Columns.Add("AO");
            tabTeklif.Columns.Add("P");

            double topOncKbo = 0;
            double topTeklif = 0;
            double topEkTeklif = 0;

            KistasYapi kY = new KistasYapi();
            kY.ekleTertipler = new Tertip[1];
            kY.ekleTertipler[0] = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", "05");
            kY.dusTertipler = new Tertip[1];
            kY.dusTertipler[0] = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", "05.04.60/05.04.70");


            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kY.ekleTertipler[0].KodNolar(3), 4));

            EkDegerParam[] ekBilgiler = new EkDegerParam[3];

            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, -1, 0, senaryoKodOnc);
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod);
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPEBTavanUstu, 0, 0, senaryoKod);


            OdenekBilgi[] obler = TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, kY, kodBilgileri, ekBilgiler, true, false, true, -1);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil", butceYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil_1", (butceYil - 1).ToString());

            if (obler.Length == 0)
            {
                tabTeklif.Rows.Add("", "", "", "", "", "", "");
            }
            else
            {
                for (int i = 0; i < obler.Length; i++)
                {
                    OdenekBilgi f = obler[i];
                    topOncKbo += ((double)f.EkRakam(0));
                    topTeklif += ((double)f.EkRakam(1));
                    topEkTeklif += ((double)f.EkRakam(2));
                }
                for (int i = 0; i < obler.Length; i++)
                {
                    OdenekBilgi f = obler[i];
                    double oncKbo = ((double)f.EkRakam(0));
                    double teklif = ((double)f.EkRakam(1));
                    double ekTeklif = ((double)f.EkRakam(2));

                    tabTeklif.Rows.Add(f.tertip.adlar[3], StrYap(oncKbo / 1000), StrYap(teklif / 1000), StrYap(ekTeklif / 1000), StrYap((teklif + ekTeklif) / 1000), ArtisYuzde(teklif + ekTeklif, oncKbo), PayHesapla(teklif + ekTeklif, topTeklif + topEkTeklif));
                }
            }
            doc.MailMerge.ExecuteWithRegions(tabTeklif);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TOncKBO", StrYap(topOncKbo / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TBTeklif", StrYap(topTeklif / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TEkTeklif", StrYap(topEkTeklif / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TTT", StrYap((topTeklif + topEkTeklif) / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TAO", ArtisYuzde(topTeklif + topEkTeklif, topOncKbo));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TP", PayHesapla(topTeklif + topEkTeklif, topTeklif + topEkTeklif));

            CiftlerinArkaPlaniniDegistir(doc.FirstSection.Body.Tables[0]);
            doc.Save(ciktiAd);
        }

        private void TarimDestekRapor()
        {
            DataTable tabTeklif = new DataTable();
            tabTeklif.TableName = "T";
            tabTeklif.Columns.Add("Aciklama");
            tabTeklif.Columns.Add("O2KBO");
            tabTeklif.Columns.Add("OncKBO");
            tabTeklif.Columns.Add("BTeklif");
            tabTeklif.Columns.Add("EkTeklif");
            tabTeklif.Columns.Add("TT");
            tabTeklif.Columns.Add("AO");
            tabTeklif.Columns.Add("P");

            double topO2Kbo = 0;
            double topOncKbo = 0;
            double topTeklif = 0;
            double topEkTeklif = 0;

            Tertip kistasTertip = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", "05.04.60/05.04.70");
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 3));

            EkDegerParam[] ekBilgiler = new EkDegerParam[4];

            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, -2, 0, senaryoKodOnc);
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, -1, 0, senaryoKodOnc);
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod);
            ekBilgiler[3] = new EkDegerParam(ENUMTabloKod.TABPEBTavanUstu, 0, 0, senaryoKod);

            OdenekBilgi[] obler = TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, true);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil", butceYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil_1", (butceYil - 1).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil_2", (butceYil - 2).ToString());

            if (obler.Length == 0)
            {
                tabTeklif.Rows.Add("", "", "", "", "", "", "", "");
            }
            else
            {
                for (int i = 0; i < obler.Length; i++)
                {
                    OdenekBilgi f = obler[i];
                    topO2Kbo += ((double)f.EkRakam(0));
                    topOncKbo += ((double)f.EkRakam(1));
                    topTeklif += ((double)f.EkRakam(2));
                    topEkTeklif += ((double)f.EkRakam(3));
                }
                for (int i = 0; i < obler.Length; i++)
                {
                    OdenekBilgi f = obler[i];
                    double o2Kbo = ((double)f.EkRakam(0));
                    double oncKbo = ((double)f.EkRakam(1));
                    double teklif = ((double)f.EkRakam(2));
                    double ekTeklif = ((double)f.EkRakam(3));

                    tabTeklif.Rows.Add(f.tertip.adlar[3], StrYap(o2Kbo / 1000), StrYap(oncKbo / 1000), StrYap(teklif / 1000), StrYap(ekTeklif / 1000), StrYap((teklif + ekTeklif) / 1000), ArtisYuzde(teklif + ekTeklif, oncKbo), PayHesapla(teklif + ekTeklif, topTeklif + topEkTeklif));
                }
            }
            doc.MailMerge.ExecuteWithRegions(tabTeklif);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TO2KBO", StrYap(topO2Kbo / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TOncKBO", StrYap(topOncKbo / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TBTeklif", StrYap(topTeklif / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TEkTeklif", StrYap(topEkTeklif / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TTT", StrYap((topTeklif + topEkTeklif) / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TAO", ArtisYuzde(topTeklif + topEkTeklif, topOncKbo));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TP", PayHesapla(topTeklif + topEkTeklif, topTeklif + topEkTeklif));

            CiftlerinArkaPlaniniDegistir(doc.FirstSection.Body.Tables[0]);
            doc.Save(ciktiAd);
        }

        private void SektorRapor()
        {
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ekoAd", EkoAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil", butceYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil_1", (butceYil - 1).ToString());

            DataTable tabTeklif = new DataTable();
            tabTeklif.TableName = "T";
            tabTeklif.Columns.Add("Aciklama");
            tabTeklif.Columns.Add("OncKBO");
            tabTeklif.Columns.Add("ButceTeklif");
            tabTeklif.Columns.Add("EkTeklif");
            tabTeklif.Columns.Add("TT");
            tabTeklif.Columns.Add("AO");
            tabTeklif.Columns.Add("P");

            double topOncKbo = 0;
            double topTeklif = 0;
            double topEkTeklif = 0;

            Tertip kistasTertip = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", EkoKod);
            ObjectArray obler = TNS.YAT.Arac.Tanimla().ProjeSunumBilgileri(kullanan, butceYil, senaryoKod, senaryoKodOnc, kistasTertip, 1);
            obler = Grupla(obler, new SektorAd());

            int satirSay = 0;

            for (int i = 0; i < obler.ObjeSayisi; i++)
            {
                ArrayList al = (ArrayList)obler.Al(i);
                OdenekBilgi f = (OdenekBilgi)al[7];

                double oncKbo = f.EkRakam(0);
                double teklif = f.EkRakam(1);
                double ekTeklif = f.EkRakam(2);
                topOncKbo += oncKbo;
                topTeklif += teklif;
                topEkTeklif += ekTeklif;
            }

            for (int i = 0; i < obler.ObjeSayisi; i++)
            {
                ArrayList al = (ArrayList)obler.Al(i);

                //string sektorKod = (string)al[3];
                string sektorAd = (string)al[4];
                OdenekBilgi f = (OdenekBilgi)al[7];

                double oncKbo = f.EkRakam(0);
                double teklif = f.EkRakam(1);
                double ekTeklif = f.EkRakam(2);
                if (OrtakFonksiyonlar.KurusYap(teklif + ekTeklif) != 0)
                {
                    tabTeklif.Rows.Add(sektorAd, StrYap(oncKbo / 1000), StrYap(teklif / 1000), StrYap(ekTeklif / 1000), StrYap((teklif + ekTeklif) / 1000), ArtisYuzde(teklif + ekTeklif, oncKbo), PayHesapla(teklif + ekTeklif, topTeklif + topEkTeklif));
                    satirSay++;
                }
            }

            if (satirSay == 0)
            {
                tabTeklif.Rows.Add("", "", "", "", "", "", "");
            }

            doc.MailMerge.ExecuteWithRegions(tabTeklif);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TKBO", StrYap(topOncKbo / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TBTeklif", StrYap(topTeklif / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TEkTeklif", StrYap(topEkTeklif / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TTT", StrYap((topTeklif + topEkTeklif) / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TAO", ArtisYuzde(topTeklif + topEkTeklif, topOncKbo));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TP", PayHesapla(topTeklif + topEkTeklif, topTeklif + topEkTeklif));

            CiftlerinArkaPlaniniDegistir(doc.FirstSection.Body.Tables[0]);
            doc.Save(ciktiAd);
        }

        private void ProgramYapisiRapor()
        {
            DataTable tabTeklif = new DataTable();
            tabTeklif.TableName = "T";
            tabTeklif.Columns.Add("Aciklama");
            tabTeklif.Columns.Add("OncKBO");
            tabTeklif.Columns.Add("ButceTeklif");
            tabTeklif.Columns.Add("EkTeklif");
            tabTeklif.Columns.Add("TT");
            tabTeklif.Columns.Add("AO");
            tabTeklif.Columns.Add("P");

            double topOncKbo = 0;
            double topTeklif = 0;
            double topEkTeklif = 0;

            Tertip kistasTertip = new Tertip(butceYil, ENUMTabloKod.TABPRGOdenek, "", "", "", "", "", TNS.DEG.Arac.VarsayilanKurumBul());
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(1), 1));

            EkDegerParam[] ekBilgiler = new EkDegerParam[3];

            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPRGOdenek, -1, 0, senaryoKodOnc);
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPRGOdenek, 0, 0, senaryoKod);
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPRGTavanUstu, 0, 0, senaryoKod);

            OdenekBilgi[] obler = TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, true);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil", butceYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil_1", (butceYil - 1).ToString());
            int satSay = 0;
            for (int i = 0; i < obler.Length; i++)
            {
                OdenekBilgi f = obler[i];
                topOncKbo += ((double)f.EkRakam(0));
                topTeklif += ((double)f.EkRakam(1));
                topEkTeklif += ((double)f.EkRakam(2));
            }
            for (int i = 0; i < obler.Length; i++)
            {
                OdenekBilgi f = obler[i];
                double oncKbo = ((double)f.EkRakam(0));
                double teklif = ((double)f.EkRakam(1));
                double ekTeklif = ((double)f.EkRakam(2));
                if (oncKbo > 0 || teklif > 0 || ekTeklif > 0)
                {
                    tabTeklif.Rows.Add(f.tertip.adlar[1], StrYap(oncKbo / 1000), StrYap(teklif / 1000), StrYap(ekTeklif / 1000), StrYap((teklif + ekTeklif) / 1000), ArtisYuzde(teklif + ekTeklif, oncKbo), PayHesapla(teklif + ekTeklif, topTeklif + topEkTeklif));
                    satSay++;
                }
            }

            if (satSay == 0)
            {
                tabTeklif.Rows.Add("", "", "", "", "", "", "");
            }

            doc.MailMerge.ExecuteWithRegions(tabTeklif);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TKBO", StrYap(topOncKbo / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TBTeklif", StrYap(topTeklif / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TEkTeklif", StrYap(topEkTeklif / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TTT", StrYap((topTeklif + topEkTeklif) / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TAO", ArtisYuzde(topTeklif + topEkTeklif, topOncKbo));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TP", PayHesapla(topTeklif + topEkTeklif, topTeklif + topEkTeklif));

            CiftlerinArkaPlaniniDegistir(doc.FirstSection.Body.Tables[0]);
            doc.Save(ciktiAd);
        }

        private void DestekRapor(bool fonKod)
        {
            DataTable tabTeklif = YeniTabloOl("T", "Fonkod");
            DataTable tF = YeniTabloOl("TF", "Fonkod");
            DataTable tK = YeniTabloOl("TK", "Fonkod", "KurKod");
            DataTable tA = YeniTabloOl("TA", "Fonkod", "KurKod", "AFKod");

            tF.PrimaryKey = new DataColumn[] { tF.Columns["FonKod"] };
            tK.PrimaryKey = new DataColumn[] { tK.Columns["FonKod"], tK.Columns["KurKod"] };
            tA.PrimaryKey = new DataColumn[] { tA.Columns["FonKod"], tA.Columns["KurKod"], tA.Columns["AFKod"] };

            DataSet veriler = new DataSet();
            veriler.Tables.Add(tF);
            veriler.Tables.Add(tK);
            veriler.Tables.Add(tA);

            veriler.Relations.Add(new DataColumn[] { tF.Columns["FonKod"] }, new DataColumn[] { tK.Columns["FonKod"] });
            veriler.Relations.Add(new DataColumn[] { tK.Columns["FonKod"], tK.Columns["KurKod"] }, new DataColumn[] { tA.Columns["FonKod"], tA.Columns["KurKod"] });

            double topKanun = 0;
            double topSon = 0;
            double topHarcama = 0;

            string kurKosul = TNS.DEG.Arac.VarsayilanKurumBul();
            if (KurSec == "01") kurKosul += "/" + TNS.DEG.Arac.VarsayilanKurumBul() + ".0060";

            Tertip kistasTertip = new Tertip(butceYil, ENUMTabloKod.TABPRGOdenek, "", "", "", "", "", kurKosul, "", Eko);
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            int fark;
            string tertipTurAd;
            if (fonKod)
            {
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(9), 4));
                tertipTurAd = "Fonksiyonel";
                fark = 0;
            }
            else
            {
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 1));
                tertipTurAd = "Program Bütçe";
                fark = 3;
            }
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(5), 2));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), 1));

            EkDegerParam[] ekBilgiler = new EkDegerParam[3];

            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPRGOdenek, 0, 0, senaryoKod);
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPRGMasrafPlaniForm, 0, 0, ";;" + TNS.BHMv2.Arac.EklemeDusmeIslemler() + ";;");
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPRGHarcama, 0, 0, ay.ToString());

            UcgenSatir[] obler = TNS.BHMv2.Arac.Tanimla().UcgenTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, true);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil", butceYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TertipTurAd", tertipTurAd);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "RaporTarih", DateTime.Now.Date.ToString("dd MMMM yyyy"));

            if (obler.Length == 0)
            {
                tabTeklif.Rows.Add("0", "", "", "", "", "");
                tF.Rows.Add("0", "", "", "", "", "");
                tK.Rows.Add("0", "0", "", "", "", "", "");
                tA.Rows.Add("0", "0", "0", "", "", "", "", "");
            }
            else
            {
                string[] kodlar = new string[7];
                for (int i = 0; i < obler.Length; i++)
                {
                    UcgenSatir f = obler[i];
                    double kanun = ((double)f.ekDegerler[0]);
                    double eklenenDusulen = ((double)f.ekDegerler[1]);
                    double son = kanun + eklenenDusulen;
                    double harcama = ((double)f.ekDegerler[2]);
                    DiziDegerAta(kodlar, f.yer, f.kodParca);
                    if (f.yer == 3 - fark)
                    {
                        topKanun += kanun;
                        topSon += son;
                        topHarcama += harcama;
                        tabTeklif.Rows.Add(KodStrOl(kodlar, 0, 3 - fark), f.aciklama.ToUpper(new CultureInfo("tr-TR", false)), StrYap(kanun / 1000), StrYap(son / 1000), StrYap(harcama / 1000), PayHesapla(harcama, son));
                        tF.Rows.Add(KodStrOl(kodlar, 0, 3 - fark), f.aciklama.ToUpper(new CultureInfo("tr-TR", false)), StrYap(kanun / 1000), StrYap(son / 1000), StrYap(harcama / 1000), PayHesapla(harcama, son));
                    }
                    if (f.yer == 5 - fark)
                        tK.Rows.Add(KodStrOl(kodlar, 0, 3 - fark), KodStrOl(kodlar, 4 - fark, 5 - fark), f.aciklama.ToUpper(new CultureInfo("tr-TR", false)), StrYap(kanun / 1000), StrYap(son / 1000), StrYap(harcama / 1000), PayHesapla(harcama, son));

                    if (f.yer == 6 - fark)
                        tA.Rows.Add(KodStrOl(kodlar, 0, 3 - fark), KodStrOl(kodlar, 4 - fark, 5 - fark), f.kodParca, "   " + OrtakFonksiyonlar.IlkHarflerBuyuk(f.aciklama), StrYap(kanun / 1000), StrYap(son / 1000), StrYap(harcama / 1000), PayHesapla(harcama, son));
                }
            }

            doc.MailMerge.ExecuteWithRegions(tabTeklif);
            doc.MailMerge.CleanupOptions = Aspose.Words.Reporting.MailMergeCleanupOptions.RemoveEmptyParagraphs;
            doc.MailMerge.ExecuteWithRegions(veriler);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TKanun", StrYap(topKanun / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TSon", StrYap(topSon / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "THarcama", StrYap(topHarcama / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TOr", PayHesapla(topHarcama, topSon));

            CiftlerinArkaPlaniniDegistir(doc.LastSection.Body.Tables[0], System.Drawing.Color.FromArgb(248, 203, 173));
            doc.Save(ciktiAd);
        }

        //private void DebugYaz(string v, bool ilk = false)
        //{
        //    OrtakFonksiyonlar.HataDosyaYaz("BSRapor.txt", v, ilk);
        //}

        private DataTable YeniTabloOl(string tabloAd, params string[] alanlar)
        {
            DataTable tabTeklif = new DataTable();
            tabTeklif.TableName = tabloAd;
            for (int i = 0; i < alanlar.Length; i++)
            {
                tabTeklif.Columns.Add(alanlar[i]);
            }

            tabTeklif.Columns.Add("Aciklama");
            tabTeklif.Columns.Add("Kanun");
            tabTeklif.Columns.Add("Son");
            tabTeklif.Columns.Add("Harcama");
            tabTeklif.Columns.Add("Or");
            return tabTeklif;
        }

        private string KodStrOl(string[] kodlar, int bas, int bit)
        {
            string d = "";
            for (int i = bas; i <= bit; i++)
            {
                d += kodlar[i];
            }
            return d;
        }

        private void DiziDegerAta(string[] kodlar, int yer, string kodParca)
        {
            kodlar[yer] = kodParca;
            for (int i = yer + 1; i < 7; i++)
            {
                kodlar[i] = "";
            }
        }

        private UcgenSatir UcgenSatirUret(int yer, string kod, string aciklama, double r1, double r2, double r3)
        {
            UcgenSatir u = new UcgenSatir();
            u.yer = yer;
            u.ekDegerler.Add(r1);
            u.ekDegerler.Add(r2);
            u.ekDegerler.Add(r3);
            u.aciklama = aciklama;
            u.kodParca = kod;
            return u;
        }

        private void BirimTeklifiRapor()
        {
            DataTable tabTeklif = new DataTable();
            tabTeklif.TableName = "T";
            tabTeklif.Columns.Add("Aciklama");
            tabTeklif.Columns.Add("E1");
            tabTeklif.Columns.Add("E2");
            tabTeklif.Columns.Add("E3");
            tabTeklif.Columns.Add("E5");
            tabTeklif.Columns.Add("E6");
            tabTeklif.Columns.Add("E7");
            tabTeklif.Columns.Add("E8");
            tabTeklif.Columns.Add("EE");
            tabTeklif.Columns.Add("ET");

            ToplamAl toplamAl = new ToplamAl(1, 9);
            toplamAl.SatirSifirla(0);
            Tertip kistasTertip = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", "");
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), 1));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 1));

            EkDegerParam[] ekBilgiler = new EkDegerParam[2];

            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod);
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPEBTavanUstu, 0, 0, senaryoKod);

            UcgenSatir[] obler = TNS.BHMv2.Arac.Tanimla().UcgenTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, true);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil", butceYil.ToString());

            if (obler.Length == 0)
            {
                tabTeklif.Rows.Add("", "", "", "", "", "", "", "", "", "");
            }
            else
            {
                DataRow dr = null;
                for (int i = 0; i < obler.Length; i++)
                {
                    UcgenSatir f = obler[i];
                    if (f.yer == 0)
                    {
                        double e = ((double)f.ekDegerler[1]);
                        double t = ((double)f.ekDegerler[0]) + e;
                        dr = tabTeklif.Rows.Add(f.aciklama, "", "", "", "", "", "", "", StrYap(e / 1000), StrYap(t / 1000));
                        toplamAl.DegerArttir(0, 7, e);
                        toplamAl.DegerArttir(0, 8, t);
                    }
                    else
                    {
                        int eKod = OrtakFonksiyonlar.ConvertToInt(f.kodParca, 0);
                        if (eKod > 3) eKod--;
                        if (eKod >= 1 && eKod <= 7)
                        {
                            double t = ((double)f.ekDegerler[0]);
                            dr[eKod] = StrYap(t / 1000);
                            toplamAl.DegerArttir(0, eKod - 1, t);
                        }
                    }
                }
            }
            tabTeklif.DefaultView.Sort = "Aciklama";
            tabTeklif = tabTeklif.DefaultView.ToTable();
            doc.MailMerge.ExecuteWithRegions(tabTeklif);

            for (int i = 0; i < 9; i++)
            {
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TE" + i.ToString(), StrYap(toplamAl.toplam[0, i] / 1000));
            }

            CiftlerinArkaPlaniniDegistir(doc.FirstSection.Body.Tables[0]);

            doc.Save(ciktiAd);
        }

        private void YilTeklifiRapor()
        {
            DataTable tabTeklif = new DataTable();
            tabTeklif.TableName = "T";
            tabTeklif.Columns.Add("K");
            tabTeklif.Columns.Add("Aciklama");
            tabTeklif.Columns.Add("OncKBO");
            tabTeklif.Columns.Add("ButceTeklif");
            tabTeklif.Columns.Add("EkTeklif");
            tabTeklif.Columns.Add("TT");
            tabTeklif.Columns.Add("AO");
            tabTeklif.Columns.Add("P");

            double topOncKbo = 0;
            double topTeklif = 0;
            double topEkTeklif = 0;

            double hTopOncKbo = 0;
            double hTopTeklif = 0;
            double hTopEkTeklif = 0;

            Tertip kistasTertip = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", "");

            Tertip kistasTertipTD = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul() + "/" + TNS.DEG.Arac.VarsayilanKurumBul() + "." + tasraKod, "", "", "05.04");
            Tertip kistasTertipBK05 = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", "05", "23.05");
            Tertip kistasTertipBK07 = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", "07", "23.05");

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 1));

            EkDegerParam[] ekBilgiler = new EkDegerParam[3];

            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, -1, 0, senaryoKodOnc);
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod);
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPEBTavanUstu, 0, 0, senaryoKod);

            OdenekBilgi[] obler = TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, true);

            OdenekBilgi[] oblerTD = TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, kistasTertipTD, kodBilgileri, ekBilgiler, true);
            OdenekBilgi[] oblerBK05 = TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, kistasTertipBK05, kodBilgileri, ekBilgiler, true);
            OdenekBilgi[] oblerBK07 = TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, kistasTertipBK07, kodBilgileri, ekBilgiler, true);


            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil", butceYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil_1", (butceYil - 1).ToString());

            if (obler.Length == 0)
            {
                tabTeklif.Rows.Add("", "", "", "", "", "", "", "");
            }
            else
            {
                for (int i = 0; i < obler.Length; i++)
                {
                    OdenekBilgi f = obler[i];
                    topOncKbo += ((double)f.EkRakam(0));
                    topTeklif += ((double)f.EkRakam(1));
                    topEkTeklif += ((double)f.EkRakam(2));
                }
                for (int i = 0; i < obler.Length; i++)
                {
                    OdenekBilgi f = obler[i];
                    double oncKbo = ((double)f.EkRakam(0));
                    double teklif = ((double)f.EkRakam(1));
                    double ekTeklif = ((double)f.EkRakam(2));
                    string kod = f.tertip.kodlar[3];
                    TeklifeSatirEkle(tabTeklif, kod, f.tertip.adlar[3], oncKbo, teklif, ekTeklif, topTeklif, topEkTeklif);
                    if (kod == "05")
                    {
                        TeklifeEkle(tabTeklif, "Tarımsal Desteklemeler", oblerTD, topTeklif, topEkTeklif);
                        TeklifeDigerEkle(tabTeklif, "Diğer Transferler (Afet Dahil)", f, topTeklif, topEkTeklif, oblerTD, oblerBK05);
                        TeklifeEkle(tabTeklif, "Bağlı Kuruluşlara Transferler", oblerBK05, topTeklif, topEkTeklif);
                        BagliToplamaEkle(oblerBK05, ref hTopOncKbo, ref hTopTeklif, ref hTopEkTeklif);
                    }
                    else if (kod == "07")
                    {
                        TeklifeEkle(tabTeklif, "Bağlı Kuruluşlara Sermaye Transferleri", oblerBK07, topTeklif, topEkTeklif);
                        TeklifeDigerEkle(tabTeklif, "Diğer Sermaye Transferleri", f, topTeklif, topEkTeklif, oblerBK07);
                        BagliToplamaEkle(oblerBK07, ref hTopOncKbo, ref hTopTeklif, ref hTopEkTeklif);
                    }
                }
            }
            doc.MailMerge.ExecuteWithRegions(tabTeklif);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TKBO", StrYap(topOncKbo / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TBTeklif", StrYap(topTeklif / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TEkTeklif", StrYap(topEkTeklif / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TTT", StrYap((topTeklif + topEkTeklif) / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TAO", ArtisYuzde(topTeklif + topEkTeklif, topOncKbo));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TP", PayHesapla(topTeklif + topEkTeklif, topTeklif + topEkTeklif));

            topOncKbo -= hTopOncKbo;
            topTeklif -= hTopTeklif;
            topEkTeklif -= hTopEkTeklif;

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HTKBO", StrYap(topOncKbo / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HTBTeklif", StrYap(topTeklif / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HTEkTeklif", StrYap(topEkTeklif / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HTTT", StrYap((topTeklif + topEkTeklif) / 1000));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HTAO", ArtisYuzde(topTeklif + topEkTeklif, topOncKbo));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HTP", PayHesapla(topTeklif + topEkTeklif, topTeklif + topEkTeklif));

            Aspose.Words.Tables.Table tbl = doc.FirstSection.Body.Tables[0];
            for (int i = 1; i < tbl.Rows.Count - 1; i++)
            {
                Cell simCell = tbl.Rows[i].Cells[0];
                string simdBirim = simCell.GetText();
                if (simdBirim.Length < 2)
                {
                    for (int c = 0; c < 8; c++)
                    {
                        Cell fCell = tbl.Rows[i].Cells[c];
                        NodeCollection runs = fCell.GetChildNodes(NodeType.Run, true);
                        foreach (Run run in runs)
                        {
                            run.Font.Bold = false;
                        }
                    }
                }
            }
            doc.Save(ciktiAd);
        }

        private void TeklifeSatirEkle(DataTable tabTeklif, string kod, string aciklama, double oncKbo, double teklif, double ekTeklif, double topTeklif, double topEkTeklif)
        {
            tabTeklif.Rows.Add(kod, aciklama, StrYap(oncKbo / 1000), StrYap(teklif / 1000), StrYap(ekTeklif / 1000), StrYap((teklif + ekTeklif) / 1000), ArtisYuzde(teklif + ekTeklif, oncKbo), PayHesapla(teklif + ekTeklif, topTeklif + topEkTeklif));
        }

        private void DegerleriBulOB(OdenekBilgi f, ref double oncKbo, ref double teklif, ref double ekTeklif, double carpan)
        {
            oncKbo += (carpan * f.EkRakam(0));
            teklif += (carpan * f.EkRakam(1));
            ekTeklif += (carpan * f.EkRakam(2));
        }

        void DegerleriBul(OdenekBilgi[] oblerTD, ref double oncKbo, ref double teklif, ref double ekTeklif, double carpan)
        {
            if (oblerTD.Length > 0)
            {
                DegerleriBulOB(oblerTD[0], ref oncKbo, ref teklif, ref ekTeklif, carpan);
            }
        }

        private void TeklifeEkle(DataTable tabTeklif, string aciklama, OdenekBilgi[] oblerTD, double topTeklif, double topEkTeklif)
        {
            double oncKbo = 0;
            double teklif = 0;
            double ekTeklif = 0;
            DegerleriBul(oblerTD, ref oncKbo, ref teklif, ref ekTeklif, 1);
            TeklifeSatirEkle(tabTeklif, "", aciklama, oncKbo, teklif, ekTeklif, topTeklif, topEkTeklif);
        }


        private void TeklifeDigerEkle(DataTable tabTeklif, string aciklama, OdenekBilgi anaSatir, double topTeklif, double topEkTeklif, params OdenekBilgi[][] cikacaklar)
        {
            double oncKbo = 0;
            double teklif = 0;
            double ekTeklif = 0;

            DegerleriBulOB(anaSatir, ref oncKbo, ref teklif, ref ekTeklif, 1);
            for (int i = 0; i < cikacaklar.Length; i++)
            {
                DegerleriBul(cikacaklar[i], ref oncKbo, ref teklif, ref ekTeklif, -1);
            }

            TeklifeSatirEkle(tabTeklif, "", aciklama, oncKbo, teklif, ekTeklif, topTeklif, topEkTeklif);
        }

        private void BagliToplamaEkle(OdenekBilgi[] oblerBK05, ref double hTopOncKbo, ref double hTopTeklif, ref double hTopEkTeklif)
        {
            DegerleriBul(oblerBK05, ref hTopOncKbo, ref hTopTeklif, ref hTopEkTeklif, 1);
        }

        private void YillaraGoreRapor()
        {
            DataTable tabTeklif = new DataTable();
            tabTeklif.TableName = "T";
            tabTeklif.Columns.Add("YilAd");
            tabTeklif.Columns.Add("Butce");
            tabTeklif.Columns.Add("O");

            int basYil = butceYil - 8;
            double oncYilDeger = SatirEkle(null, basYil - 1, 0, 0);
            for (int y = basYil; y <= butceYil; y++)
            {
                oncYilDeger = SatirEkle(tabTeklif, y, 0, oncYilDeger);
            }
            oncYilDeger = SatirEkle(tabTeklif, butceYil, 1, oncYilDeger);
            SatirEkle(tabTeklif, butceYil, 2, oncYilDeger);

            doc.MailMerge.ExecuteWithRegions(tabTeklif);

            Aspose.Words.Tables.Table tbl = doc.FirstSection.Body.Tables[0];
            CiftlerinArkaPlaniniDegistir(tbl);
            for (int i = 1; i < tbl.Rows.Count - 1; i++)
            {
                Cell simCell = tbl.Rows[i].Cells[0];
                string simdBirim = simCell.GetText();
                if (simdBirim.Contains("OVP"))
                {
                    for (int c = 0; c < 3; c++)
                    {
                        Cell fCell = tbl.Rows[i].Cells[c];
                        NodeCollection runs = fCell.GetChildNodes(NodeType.Run, true);
                        foreach (Run run in runs)
                        {
                            run.Font.Bold = true;
                        }
                    }
                }
            }

            doc.Save(ciktiAd);
        }

        private void CiftlerinArkaPlaniniDegistir(Table tbl)
        {
            CiftlerinArkaPlaniniDegistir(tbl, System.Drawing.Color.FromArgb(242, 220, 219));
        }

        private void CiftlerinArkaPlaniniDegistir(Table tbl, System.Drawing.Color ikinciRenk)
        {
            if (tbl != null)
            {
                for (int i = 1; i < tbl.Rows.Count - 1; i++)
                {
                    if (i % 2 == 0)
                    {
                        for (int c = 0; c < tbl.Rows[i].Cells.Count; c++)
                        {
                            Cell fCell = tbl.Rows[i].Cells[c];
                            fCell.CellFormat.Shading.BackgroundPatternColor = ikinciRenk;
                        }
                    }
                }
            }
        }

        private double SatirEkle(DataTable tabTeklif, int yil, int yilFark, double oncYilDeger)
        {
            string sKod;
            string yilEk;
            if (yil == butceYil)
            {
                sKod = senaryoKod;
                if (yilFark == 0) yilEk = " (OVP)";
                else yilEk = " (Tahmin)";
            }
            else
            {
                sKod = senaryoKodOnc;
                yilEk = "";
            }
            double simdYilDeger = uzyServis.UzayDegeriDbl(kullanan, "ABSODENEK", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), sKod, yilFark.ToString(), kurumKod), false);
            if (tabTeklif != null)
                tabTeklif.Rows.Add((yil + yilFark).ToString() + yilEk, StrYap(simdYilDeger / 1000), ArtisYuzde(simdYilDeger, oncYilDeger));
            return simdYilDeger;
        }

        private string ArtisYuzde(double simdYilDeger, double oncYilDeger)
        {
            if (oncYilDeger == 0) return " - ";
            return StrYap(((simdYilDeger - oncYilDeger) / oncYilDeger) * 100, 1);
        }

        private string PayHesapla(double pay, double toplam)
        {
            if (toplam == 0) return " - ";
            return StrYap((pay / toplam) * 100, 1);
        }

        private void YeniRapor()
        {
            DataTable tabTeklif = new DataTable();
            tabTeklif.TableName = "T";
            tabTeklif.Columns.Add("BirimAd");
            tabTeklif.Columns.Add("ProjeAd");
            tabTeklif.Columns.Add("TT");

            double topEkTeklif = 0;

            Tertip kistasTertip = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", EkoKod);
            ObjectArray obler = TNS.YAT.Arac.Tanimla().ProjeSunumBilgileri(kullanan, butceYil, senaryoKod, senaryoKodOnc, kistasTertip, 3);
            obler = Grupla(obler, new BirimAdAltProjeAd());

            int satSay = 0;


            string oncPeb2Ad = null;
            double birimToplam = 0;
            for (int i = 0; i < obler.ObjeSayisi; i++)
            {
                ArrayList al = (ArrayList)obler.Al(i);

                string peb2Ad = (string)al[0];
                string altProjeAd = (string)al[6];
                if (string.IsNullOrEmpty(altProjeAd)) continue;
                OdenekBilgi f = (OdenekBilgi)al[7];
                if (oncPeb2Ad != peb2Ad && oncPeb2Ad != null)
                {
                    tabTeklif.Rows.Add("TOPLAM", StrYap(birimToplam), "");
                    birimToplam = 0;
                }
                oncPeb2Ad = peb2Ad;
                double ekTeklif = (f.EkRakam(1) + f.EkRakam(2)) / 1000;
                if (OrtakFonksiyonlar.KurusYap(ekTeklif) != 0)
                {
                    tabTeklif.Rows.Add(peb2Ad, altProjeAd, StrYap(ekTeklif));
                    satSay++;
                    topEkTeklif += ekTeklif;
                    birimToplam += ekTeklif;
                }
            }
            if (oncPeb2Ad != null)
            {
                tabTeklif.Rows.Add("TOPLAM", StrYap(birimToplam), "");
            }

            if (satSay == 0)
            {
                tabTeklif.Rows.Add("", "", "");
            }

            doc.MailMerge.ExecuteWithRegions(tabTeklif);

            List<Cell> l = new List<Cell>();
            string oncBirim = null;
            Aspose.Words.Tables.Table tbl = doc.FirstSection.Body.Tables[0];
            for (int i = 1; i < tbl.Rows.Count - 1; i++)
            {
                Cell simCell = tbl.Rows[i].Cells[0];
                string simdBirim = simCell.GetText();
                if (simdBirim.StartsWith("TOPLAM"))
                {
                    for (int c = 0; c < 3; c++)
                    {
                        Cell fCell = tbl.Rows[i].Cells[c];
                        fCell.CellFormat.Shading.BackgroundPatternColor = System.Drawing.Color.FromArgb(242, 220, 219);
                        if (c == 0)
                            fCell.FirstParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                        else if (c == 1)
                            fCell.FirstParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                    }
                }
                if (oncBirim == null || oncBirim != simdBirim)
                {
                    oncBirim = simdBirim;
                    VerticallyMergeCells(l);
                }
                l.Add(simCell);
            }
            VerticallyMergeCells(l);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ekoAd", EkoAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil", butceYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil_1", (butceYil - 1).ToString());

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TopTopTeklif", StrYap(topEkTeklif));

            doc.Save(ciktiAd);
        }

        private void BirimRapor()
        {
            DataTable tabTeklif = new DataTable();
            tabTeklif.TableName = "T";
            tabTeklif.Columns.Add("BirimAd");
            tabTeklif.Columns.Add("OncKBO");
            tabTeklif.Columns.Add("ButceTeklif");
            tabTeklif.Columns.Add("EkTeklif");
            tabTeklif.Columns.Add("TT");
            tabTeklif.Columns.Add("AO");

            double topOncKbo = 0;
            double topTeklif = 0;
            double topEkTeklif = 0;

            Tertip kistasTertip = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", EkoKod);
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), 1));

            EkDegerParam[] ekBilgiler = new EkDegerParam[3];

            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, -1, 0, senaryoKodOnc);
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod);
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPEBTavanUstu, 0, 0, senaryoKod);

            OdenekBilgi[] obler = TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, true);
            Array.Sort(obler, new PebAdaGoreSirala());
            if (obler.Length == 0)
            {
                tabTeklif.Rows.Add("", "", "", "", "", "");
            }
            else
                for (int i = 0; i < obler.Length; i++)
                {
                    OdenekBilgi f = obler[i];
                    double oncKbo = f.EkRakam(0) / 1000;
                    double teklif = f.EkRakam(1) / 1000;
                    double ekTeklif = f.EkRakam(2) / 1000;
                    tabTeklif.Rows.Add(f.tertip.adlar[4], StrYap(oncKbo), StrYap(teklif), StrYap(ekTeklif), StrYap(teklif + ekTeklif), ArtisYuzde(teklif + ekTeklif, oncKbo));

                    topOncKbo += oncKbo;
                    topTeklif += teklif;
                    topEkTeklif += ekTeklif;
                }
            doc.MailMerge.ExecuteWithRegions(tabTeklif);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ekoAd", EkoAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil", butceYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil_1", (butceYil - 1).ToString());

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TopKBO", StrYap(topOncKbo));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TopButceTeklif", StrYap(topTeklif));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TopEkTeklif", StrYap(topEkTeklif));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TopTopTeklif", StrYap(topTeklif + topEkTeklif));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TAO", ArtisYuzde(topTeklif + topEkTeklif, topOncKbo));

            doc.Save(ciktiAd);
        }

        private void TalepRapor()
        {
            DataTable tabTeklif = new DataTable();
            tabTeklif.TableName = "T";
            tabTeklif.Columns.Add("BirimAd");
            tabTeklif.Columns.Add("ProjeAd");
            tabTeklif.Columns.Add("OncKBO");
            tabTeklif.Columns.Add("ButceTeklif");
            tabTeklif.Columns.Add("EkTeklif");
            tabTeklif.Columns.Add("TT");

            double topOncKbo = 0;
            double topTeklif = 0;
            double topEkTeklif = 0;

            Tertip kistasTertip = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", EkoKod);
            ObjectArray obler = TNS.YAT.Arac.Tanimla().ProjeSunumBilgileri(kullanan, butceYil, senaryoKod, senaryoKodOnc, kistasTertip, 2);
            obler = Grupla(obler, new BirimAdProjeAd());

            if (obler.ObjeSayisi == 0)
            {
                tabTeklif.Rows.Add("", "", "", "", "", "");
            }
            else
                for (int i = 0; i < obler.ObjeSayisi; i++)
                {
                    ArrayList al = (ArrayList)obler.Al(i);

                    string peb2Ad = (string)al[0];
                    string projeAd = (string)al[1];
                    OdenekBilgi f = (OdenekBilgi)al[7];

                    double oncKbo = f.EkRakam(0) / 1000;
                    double teklif = f.EkRakam(1) / 1000;
                    double ekTeklif = f.EkRakam(2) / 1000;
                    if (OrtakFonksiyonlar.KurusYap(teklif + ekTeklif) != 0)
                    {
                        tabTeklif.Rows.Add(peb2Ad, projeAd, StrYap(oncKbo), StrYap(teklif), StrYap(ekTeklif), StrYap(teklif + ekTeklif));

                        topOncKbo += oncKbo;
                        topTeklif += teklif;
                        topEkTeklif += ekTeklif;
                    }
                }
            doc.MailMerge.ExecuteWithRegions(tabTeklif);

            List<Cell> l = new List<Cell>();
            string oncBirim = null;
            Aspose.Words.Tables.Table tbl = doc.FirstSection.Body.Tables[0];
            for (int i = 1; i < tbl.Rows.Count - 1; i++)
            {
                Cell simCell = tbl.Rows[i].Cells[0];
                string simdBirim = simCell.GetText();
                if (oncBirim == null || oncBirim != simdBirim)
                {
                    oncBirim = simdBirim;
                    VerticallyMergeCells(l);
                }
                l.Add(simCell);
            }
            VerticallyMergeCells(l);

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ekoAd", EkoAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil", butceYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Yil_1", (butceYil - 1).ToString());

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TopKBO", StrYap(topOncKbo));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TopButceTeklif", StrYap(topTeklif));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TopEkTeklif", StrYap(topEkTeklif));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TopTopTeklif", StrYap(topTeklif + topEkTeklif));

            doc.Save(ciktiAd);
        }

        private void VerticallyMergeCells(List<Cell> l)
        {
            if (l.Count > 1)
            {
                Cell c1 = l[0];
                c1.CellFormat.VerticalMerge = CellMerge.First;

                for (int i = 1; i < l.Count; i++)
                {
                    Cell c2 = l[i];
                    c2.CellFormat.VerticalMerge = CellMerge.Previous;
                }
            }
            l.Clear();
        }

        private ObjectArray Grupla(ObjectArray obler, IComparer b)
        {
            obler.objeler.Sort(b);
            //ObjectArray yeni = obler;
            int yer = 1;
            while (yer < obler.objeler.Count)
            {
                if (b.Compare(obler[yer - 1], obler[yer]) == 0)
                {
                    RakamlariEkle(obler, yer - 1, yer);
                    obler.objeler.RemoveAt(yer);
                }
                else
                    yer++;
            }

            return obler;
        }

        private void RakamlariEkle(ObjectArray obler, int hedefYer, int kaynakYer)
        {
            ArrayList alHedef = (ArrayList)obler[hedefYer];
            ArrayList alKaynak = (ArrayList)obler[kaynakYer];

            OdenekBilgi hedefOb = (OdenekBilgi)alHedef[7];
            OdenekBilgi kaynakOb = (OdenekBilgi)alKaynak[7];

            for (int i = 0; i < kaynakOb.ekRakamlar.Count; i++)
            {
                hedefOb.ekRakamlar[i] = kaynakOb.EkRakam(i) + hedefOb.EkRakam(i);
            }
        }

        private class PebAdaGoreSirala : IComparer<OdenekBilgi>
        {
            public int Compare(OdenekBilgi x, OdenekBilgi y)
            {
                return x.tertip.adlar[4].CompareTo(y.tertip.adlar[4]);
            }
        }

        private class BirimAdProjeAd : IComparer
        {
            public int Compare(object x, object y)
            {
                if ((x is ArrayList) && (y is ArrayList))
                {
                    ArrayList alX = (ArrayList)x;
                    ArrayList alY = (ArrayList)y;
                    string adX = (string)alX[0];
                    string adY = (string)alY[0];
                    int compSonuc = adX.CompareTo(adY);
                    if (compSonuc == 0)
                    {
                        adX = (string)alX[1];
                        adY = (string)alY[1];
                        compSonuc = adX.CompareTo(adY);
                    }
                    return compSonuc;
                }
                else
                    return 0;
            }
        }
        private class BirimAdAltProjeAd : IComparer
        {
            public int Compare(object x, object y)
            {
                if ((x is ArrayList) && (y is ArrayList))
                {
                    ArrayList alX = (ArrayList)x;
                    ArrayList alY = (ArrayList)y;
                    string adX = (string)alX[0];
                    string adY = (string)alY[0];
                    int compSonuc = adX.CompareTo(adY);
                    if (compSonuc == 0)
                    {
                        adX = (string)alX[6];
                        adY = (string)alY[6];
                        compSonuc = adX.CompareTo(adY);
                    }
                    return compSonuc;
                }
                else
                    return 0;
            }
        }
        private class SektorAd : IComparer
        {
            public int Compare(object x, object y)
            {
                if ((x is ArrayList) && (y is ArrayList))
                {
                    ArrayList alX = (ArrayList)x;
                    ArrayList alY = (ArrayList)y;
                    string adX = (string)alX[3];
                    string adY = (string)alY[3];
                    int compSonuc = adX.CompareTo(adY);
                    return compSonuc;
                }
                else
                    return 0;
            }
        }

        private class RaporBilgi
        {
            public int raporNo;
            public string ekoKod;

            public RaporBilgi(int raporNo, string ekoKod)
            {
                this.raporNo = raporNo;
                this.ekoKod = ekoKod;
            }

            public string TabloAd
            {
                get
                {
                    return raporNo + ekoKod;
                }
            }
        }
    }
}