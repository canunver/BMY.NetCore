using System;
using System.Collections.Generic;
using TNS.Raporlama;
using TNS.KYM;
using OrtakClass;
using TNS;
using Aspose.Words;
using System.Data;
using TNS.BKD;
using TNS.BHMv2;
using Aspose.Words.Tables;
using TNS.UZY;
using System.Collections;

namespace ButceHazirlikv2
{
    /// <summary>Faaliyet raporu metinleri (eski FaaliyetRaporuForm.FaaliyetRaporuBilgi): 0 = bölüm 4, 1 = 4.1, 2 = 4.2, 2 + i = Tablo i notu.</summary>
    public class FaaliyetRaporuBilgi
    {
        public string[] rapor = new string[18];
    }

    /// <summary>
    /// Faaliyet raporu ve tabloları (Rapor.xml grup 1, NESNE="ButceHazirlikv2.FaaliyetRaporuRaporlari"). Eski
    /// ButceHazirlikv2/FaaliyetRaporuRaporlari.cs'in aynısıdır; yalnızca eski FaaliyetRaporuForm.Listele buraya (MetinleriOku) alındı
    /// ve AsposeAlanaYazHtml için YatRaporAraclari kullanıldı.
    /// </summary>
    public class FaaliyetRaporuRaporlari
    {
        /// <summary>BuyukMetin (BHMv2) anahtarları: yil + ek. Sıra FaaliyetRaporuBilgi.rapor ile aynıdır.</summary>
        public static string MetinAnahtarEk(int sira)
        {
            if (sira == 0) return "_0";
            if (sira == 1) return "_01";
            if (sira == 2) return "_02";
            return "_" + (sira - 2).ToString();
        }

        /// <summary>Eski FaaliyetRaporuForm.Listele: yılın bölüm metinleri ve tablo notları.</summary>
        public static FaaliyetRaporuBilgi MetinleriOku(Kullanici kullanan, int yil)
        {
            IKYMServis servis = TNS.KYM.Arac.Tanimla();
            FaaliyetRaporuBilgi frb = new FaaliyetRaporuBilgi();
            for (int i = 0; i <= 17; i++)
                frb.rapor[i] = servis.BuyukMetinOku(kullanan, "BHMv2", yil.ToString() + MetinAnahtarEk(i), "tr");
            return frb;
        }

        public Kullanici kullanan { get; set; }
        public string sablonAd { get; set; }
        public string sablonYol { get; set; }
        public string format { get; set; }
        public string sonucDosyaAd { get; set; }
        public int butceYil { get; set; }
        public string sonucDosyaAdWrd { get; set; }
        public IBHMv2Servis servisBHMv2 { get; set; }

        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            this.kullanan = (Kullanici)kullanan;
            this.sablonAd = sablonAd;
            this.sablonYol = System.IO.Path.GetDirectoryName(sablonAd);
            this.sonucDosyaAd = sonucDosyaAd;
            this.servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
            object o = raporParam["Format"];
            if (o != null && o.GetType() == typeof(string))
                this.format = raporParam["Format"].ToString();
            if (this.format != "pdf")
                this.format = "docx";
            this.sonucDosyaAdWrd = sonucDosyaAd + "." + this.format;
            int grup = OrtakFonksiyonlar.ConvertToInt(raporParam["Grup"], 0);
            int yikob = OrtakFonksiyonlar.ConvertToInt(raporParam["Yikob"], 0);
            this.tabloNo = (string)raporParam["TabloNo"];

            this.butceYil = OrtakFonksiyonlar.ConvertToInt(raporParam["ButceYil"].ToString(), 0);
            if (this.tabloNo == null)
                this.tabloNo = TabloNoBul(OrtakFonksiyonlar.ConvertToInt(ekBilgi, -1));
            int yikobEk; ;
            if (ekBilgi.Equals("9"))
            {
                yikobEk = yikob;
            }
            else 
                yikobEk = -1;
            this.tabloAd = TabloAdBul(OrtakFonksiyonlar.ConvertToInt(ekBilgi, -1), this.butceYil - 1, this.butceYil, yikobEk);
            if (ekBilgi.Equals("1"))
                return YilKarsilastirmasi(4, 1);
            else if (ekBilgi.Equals("2"))
                return GiderSiniflandirmasiRaporuHazirla(4, 1);
            else if (ekBilgi.Equals("3"))
                return GiderSiniflandirmasiRaporuHazirla(4, 2);
            else if (ekBilgi.Equals("15"))
                return GiderSiniflandirmasiProgramRaporuHazirla();
            else if (ekBilgi.Equals("4"))
                return GiderSiniflandirmasiRaporuHazirla(2, 1);
            else if (ekBilgi.Equals("5"))
                return GiderSiniflandirmasiRaporuHazirla(3, 1);
            else if (ekBilgi.Equals("6"))
                return GiderSiniflandirmasiRaporuHazirla(1, 2, "", true);
            else if (ekBilgi.Equals("7"))
                return GiderSiniflandirmasiRaporuHazirla(4, 4, "05.03.10", true);
            else if (ekBilgi.Equals("8"))
                return GiderSiniflandirmasiEkoFinHazirla();
            else if (ekBilgi.Equals("9"))
                return ProjeListesi("06", "", grup, yikob);
            else if (ekBilgi.Equals("10"))
                return ProjeListesi("06", "07", grup, yikob);
            else if (ekBilgi.Equals("11"))
                return ProjeListesi("07", "", grup, yikob);
            else if (ekBilgi.Equals("12"))
                return ProjeListesi("07", "07", grup, yikob);
            else if (ekBilgi.Equals("13"))
                return ProjeListesi("08", "", grup, yikob);
            else if (ekBilgi.Equals("14"))
                return ProjeListesi("05", "", grup, yikob);
            else if (ekBilgi.Equals("99"))
                return FaaliyetRaporu(grup, yikob);

            throw new Exception("Rapor hatalı parametrelerle çağrıldı!");
        }

        private string YilKarsilastirmasi(int kodNo, int kirilimSay)
        {
            Tertip kistas = null;
            UcgenKodBilgileri kodBilgileri = null;
            EkDegerParam[] ekBilgiler = null;

            kistas = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, GenelIslemlerIstemci.VarsayilanKurumBul(), "", "", "", "");
            kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(kodNo - 1), kirilimSay));

            ekBilgiler = new EkDegerParam[2];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, -1, 0, "06.00");
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, "06.00");

            UcgenSatir[] odnler = servisBHMv2.UcgenTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, true);

            Aspose.Words.Document doc = new Aspose.Words.Document(sablonAd);
            DocumentBuilder builder = new DocumentBuilder(doc);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYil", butceYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYilO", (butceYil - 1).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TabloNo", this.tabloNo);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TabloAd", this.tabloAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KodAd", KodAdBul(kodNo));
            if (kodNo == 1)
            {
                Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
                Row row = table.Rows[0];
                //row.Cells[1].CellFormat.PreferredWidth = PreferredWidth.FromPoints(row.Cells[1].CellFormat.PreferredWidth.Value - 38);
                row.Cells[0].CellFormat.PreferredWidth = PreferredWidth.FromPoints(row.Cells[0].CellFormat.PreferredWidth.Value + 38);
            }
            DataTable dt = new DataTable();
            dt.TableName = "Veri";
            dt.Columns.Add("EkoKod", typeof(String));
            dt.Columns.Add("EkoAd", typeof(String));
            dt.Columns.Add("KBO_O", typeof(String));
            dt.Columns.Add("KBO_O_Or", typeof(String));
            dt.Columns.Add("KBO", typeof(String));
            dt.Columns.Add("KBO_Or", typeof(String));
            dt.Columns.Add("Oran", typeof(String));

            double tKbo_O = 0;
            double tKbo = 0;

            foreach (UcgenSatir us in odnler)
            {
                if (us.yer == 0)
                {
                    tKbo_O += (double)us.ekDegerler[0];
                    tKbo += (double)us.ekDegerler[1];
                }
            }

            string[] kodParcala = new string[4];
            foreach (UcgenSatir us in odnler)
            {
                bool yaz;
                double Kbo_O = (double)us.ekDegerler[0];
                double Kbo = (double)us.ekDegerler[1];

                kodParcala[us.yer] = us.kodParca;

                if (us.yer < 3 && kodNo == 1)
                    yaz = false;
                else
                    yaz = true;
                string yazKod;
                if (kodNo == 1 && yaz)
                    yazKod = String.Join(".", kodParcala, 0, us.yer + 1);
                else
                    yazKod = us.kodParca;
                if (yaz)
                    dt.Rows.Add(yazKod, us.aciklama, StringYap(Kbo_O), StringYapOran(Kbo_O, tKbo_O, 1), StringYap(Kbo), StringYapOran(Kbo, tKbo, 1), StringAYapOran(Kbo_O, Kbo, 1));
            }
            if (tKbo == 0)
            {
                dt.Rows.Add("", "", "", "", "", "", "");
            }
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TKBO_O", StringYap(tKbo_O));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TKBO", StringYap(tKbo));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "AOran", StringAYapOran(tKbo_O, tKbo, 1));
            doc.MailMerge.ExecuteWithRegions(dt);
            if (kirilimSay > 1)
            {
                Table tbl = doc.FirstSection.Body.Tables[0];

                for (int i = 1; i < tbl.Rows.Count - 1; i++)
                {
                    Cell aktifHucre = tbl.Rows[i].Cells[0];
                    string aktifHucreVeri = aktifHucre.GetText();
                    if (!string.IsNullOrWhiteSpace(aktifHucreVeri))
                    {
                        if (aktifHucreVeri.Length == 3) //satırı koyu yap
                        {
                            for (int colIdx = 0; colIdx < tbl.Rows[i].Cells.Count; colIdx++)
                            {
                                Cell formatHucre = tbl.Rows[i].Cells[colIdx];
                                NodeCollection runs = formatHucre.GetChildNodes(NodeType.Run, true);
                                foreach (Run run in runs)
                                {
                                    run.Font.Bold = true;
                                }
                            }
                        }
                        else if (aktifHucreVeri.Length == 2) //ilk hücreyi sağa daya
                        {
                            foreach (Paragraph p in aktifHucre.Paragraphs)
                            {
                                p.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                            }
                        }
                    }
                }
            }
            doc.Save(this.sonucDosyaAdWrd);
            return this.sonucDosyaAdWrd;
        }

        private static string[] tabloAdlar = {
                     "{0}-{1} Yılları Bakanlığımız Bütçesi:"
                    ,"{1} Yılı Bütçe Uygulama Sonuçları, Ekonomik Sınıflandırma:"
                    ,"{1} Yılı Bütçe Uygulama Sonuçları, Ekonomik İkinci Düzey Sınıflandırma:"
                    ,"{1} Yılı Bütçe Uygulama Sonuçları, Fonksiyonel Sınıflandırma:"
                    ,"{1} Yılı Bütçe Uygulama Sonuçları, Finansal Sınıflandırma:"
                    ,"{1} Yılı Bütçe Uygulama Sonuçları, Kurumsal Sınıflandırma:"
                    ,"{1} Yılı Bütçe Uygulama Sonuçları, Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler:"
                    ,"{1} Yılı Yatırım Ödenekleri:"
                    ,"{1} Yılı Sermaye Giderleri ({2}):"
                    ,"{1} Yılı Sermaye Giderleri, Dış Proje Kredileri:"
                    ,"{1} Yılı Sermaye Transferleri:"
                    ,"{1} Yılı Sermaye Transferleri, Dış Proje Kredileri:"
                    ,"{1} Yılı Borç Verme:"
                    ,"{1} Yılı Cari Transferler:"
                    ,"{1} Yılı Bütçe Uygulama Sonuçları, Program Sınıflandırma:"
                                             };
        private string TabloNoBul(int tabloNo)
        {
            if (tabloNo == 15) return "4";
            return tabloNo.ToString();
        }

        public static string TabloAdBul(int tabloNo, int oncekiYil, int butceYil, int yikobEk)
        {
            if (tabloNo >= 1 && tabloNo <= tabloAdlar.Length)
            {
                string fmt = tabloAdlar[tabloNo - 1];
                string yikobEkStr;
                if (yikobEk == 0)
                    yikobEkStr = "YİKOB Dahil";
                else if (yikobEk == 1)
                    yikobEkStr = "YİKOB Hariç";
                else if (yikobEk == 2)
                    yikobEkStr = "YİKOB Ödenekleri";
                else
                    yikobEkStr = "";

                return string.Format(fmt, oncekiYil, butceYil, yikobEkStr);
            }
            else
                return "";
        }

        class YazRaporBilgi
        {
            internal int raporNo;
            internal int yikob;
            internal string tabloNo;
            internal bool raporNotYaz;
            internal YazRaporBilgi(int raporNo, string tabloNo)
            {
                this.raporNo = raporNo;
                this.tabloNo = tabloNo;
                this.yikob = -1;
                raporNotYaz = true;
            }
            internal YazRaporBilgi(int raporNo, string tabloNo, int yikob, bool raporNotYaz)
            {
                this.raporNo = raporNo;
                this.tabloNo = tabloNo;
                this.yikob = yikob;
                this.raporNotYaz = raporNotYaz;
            }
        }

        private string FaaliyetRaporu(int grup, int yikob)
        {

            YazRaporBilgi[] yazRaporBilgi = new YazRaporBilgi[] {
                new YazRaporBilgi(1, "1"),
                new YazRaporBilgi(2, "2"),
                new YazRaporBilgi(3, "3"),
                new YazRaporBilgi(15, "4"),
                new YazRaporBilgi(5, "5"),
                new YazRaporBilgi(6, "6"),
                new YazRaporBilgi(7, "7"),
                new YazRaporBilgi(8, "8"),
                new YazRaporBilgi(9, "9-a", 1, false),
                new YazRaporBilgi(9, "9-b", 0, true),
                new YazRaporBilgi(9, "9-c", 2, false),
                new YazRaporBilgi(10, "10"),
                new YazRaporBilgi(11, "11"),
                new YazRaporBilgi(12, "12"),
                new YazRaporBilgi(13, "13"),
            };
            Aspose.Words.Document doc = new Aspose.Words.Document(sablonAd);
            DocumentBuilder builder = new DocumentBuilder(doc);

            FaaliyetRaporuBilgi frb = MetinleriOku(kullanan, butceYil);
            YatRaporAraclari.AsposeAlanaYazHtml(builder, "bolum4", frb.rapor[0]);
            YatRaporAraclari.AsposeAlanaYazHtml(builder, "bolum4_1", frb.rapor[1]);

            Hashtable raporParam = new Hashtable();
            raporParam["ButceYil"] = this.butceYil.ToString();
            raporParam["Grup"] = grup.ToString();
            raporParam["Yikob"] = yikob.ToString();
            for (int rNo = 0; rNo < yazRaporBilgi.Length; rNo++)
            {
                int i = rNo + 1;
                try
                {
                    if (yazRaporBilgi[rNo].yikob > -1)
                        raporParam["Yikob"] = yazRaporBilgi[rNo].yikob.ToString();
                    else
                        raporParam["Yikob"] = yikob.ToString();

                    raporParam["TabloNo"] = yazRaporBilgi[rNo].tabloNo;

                    string raporAd = RaporlamaServis.TekRaporUret(1, yazRaporBilgi[rNo].raporNo, kullanan, null, raporParam);
                    GenelIslemlerIstemci.AsposeAlanaDocEkle(builder, "tablo" + i.ToString(), raporAd);
                    System.IO.File.Delete(raporAd);
                }
                catch (Exception ex)
                {
                    GenelIslemlerIstemci.AsposeAlanaYaz(builder, "tablo" + i.ToString(), ex.Message);
                }
                string dipNot;
                if (yazRaporBilgi[rNo].raporNotYaz)
                    dipNot = frb.rapor[yazRaporBilgi[rNo].raporNo + 2];
                else
                    dipNot = "";
                YatRaporAraclari.AsposeAlanaYazHtml(builder, "tablo" + i.ToString() + "Not", dipNot);
            }
            YatRaporAraclari.AsposeAlanaYazHtml(builder, "bolum4_2", frb.rapor[2]);
            doc.Save(this.sonucDosyaAdWrd);
            return this.sonucDosyaAdWrd;
        }

        public class GeciciProjeBilgi
        {
            public string DPTPROJENO { get; set; }
            public int NO { get; set; }
            public string AD { get; set; }
            public int BASLANGICYILI { get; set; }

            public int AP_DPTDEVAR { get; set; }

            public double Kbo { get; set; }
            public double eklenenDusulen { get; set; }
            public double Harcama { get; set; }

            public List<GeciciProjeBilgi> altlar = null;

            public GeciciProjeBilgi()
            {
                Kbo = 0;
                eklenenDusulen = 0;
                Harcama = 0;
            }

            internal bool DoluMu()
            {
                return Kbo != 0 || eklenenDusulen != 0 || Harcama != 0;
            }

            internal bool YazilacakAltProjeVar(out string adEk)
            {
                adEk = "";
                if (altlar == null) return false;
                for (int i = altlar.Count - 1; i >= 0; i--)
                {
                    GeciciProjeBilgi alt = altlar[i];
                    if (!alt.DoluMu())
                        altlar.RemoveAt(i);
                }
                if (altlar.Count == 1)
                {
                    GeciciProjeBilgi alt = altlar[0];
                    if (alt.Kbo == this.Kbo && alt.eklenenDusulen == this.eklenenDusulen && alt.Harcama == this.Harcama)
                    {
                        if (String.Compare(alt.AD, this.AD, true) != 0)
                            adEk = alt.AD;
                        return false;
                    }
                }
                return true;
            }

            public string SEKTORNO { get; set; }

            public string SEKTORAD { get; set; }

            public string YURUTECEKKURUM { get; set; }

            public string KURAD { get; set; }
        }

        private string ProjeListesi(string ekoKod, string finKod, int grup, int yikob)
        {
            Tertip kistas = null;
            UcgenKodBilgileri kodBilgileri = null;
            EkDegerParam[] ekBilgiler = null;

            kistas = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, GenelIslemlerIstemci.VarsayilanKurumBul(), "", finKod, ekoKod, "");
            kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(4), kistas.KodNoKirilimSay(4)));

            ekBilgiler = new EkDegerParam[3];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, "06.00");
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABMasrafPlaniForm, 0, 0, ";;" + TNS.BHMv2.Arac.EklemeDusmeIslemler() + ";;");
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPEBHarcama, 0, 0, "12");

            OdenekBilgi[] odnler = servisBHMv2.ListeTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, true);

            string sql = @"SELECT P.DPTPROJENO, P.PROJENO, P.AD, P.BASLANGICYILI, AP.ALTPROJENO, AP.AD, AP.BASLANGICYILI, AP.DPTDEVAR, PK.PEBKOD, K.TOPLAMADAHIL, P.SEKTORNO, SEK.AD, P.YURUTECEKKURUM, KUR.KURAD
                            FROM YATBUTCEKAYNAKPEBKOD AS PK
                            INNER JOIN YATBUTCEKAYNAK AS K ON PK.YIL = K.YIL AND PK.SENARYOKOD = K.SENARYOKOD AND PK.KAYNAKNO = K.KAYNAKNO
                            INNER JOIN YATALTPROJE AS AP ON K.YIL = AP.YIL AND K.SENARYOKOD = AP.SENARYOKOD AND K.ALTPROJENO = AP.ALTPROJENO
                            INNER JOIN YATPROJE AS P ON AP.YIL = P.YIL AND AP.SENARYOKOD = P.SENARYOKOD AND AP.PROJENO = P.PROJENO
                            INNER JOIN YATSEKTOR AS SEK ON SEK.YIL = P.YIL AND SEK.SEKTORNO = P.SEKTORNO
                            INNER JOIN ButceHazirlik.dbo.ABSKUR AS KUR ON KUR.YIL = P.YIL AND KUR.KURKOD = P.YURUTECEKKURUM
                            WHERE (PK.YIL = " + butceYil.ToString() + ") AND (PK.SENARYOKOD = '0600')";
            if (yikob == 1)
                sql += " AND K.YIKOBMU <> 1";
            else if (yikob == 2)
                sql += " AND K.YIKOBMU = 1";

            sql += " order by ";

            string sira = "P.DPTPROJENO, AP.BASLANGICYILI, AP.ALTPROJENO";
            if (grup == 1)
                sira = "P.SEKTORNO, " + sira;
            else if (grup == 2)
                sira = "P.YURUTECEKKURUM, " + sira;

            sql += sira;

            VTSorgula sorgu = new VTSorgula(sql, "BaglantiSatiriYAT", 0);

            List<GeciciProjeBilgi> projeler = new List<GeciciProjeBilgi>();
            int AP_ALTPROJENOeski = -1;
            int PROJENOeski = -1;
            GeciciProjeBilgi pb = null;
            GeciciProjeBilgi apb = null;
            while (true)
            {
                DataRow dr = sorgu.satirOku();
                if (dr == null) break;
                int PROJENO = sorgu.IntAl(1);
                int AP_ALTPROJENO = sorgu.IntAl(4);

                if (pb == null || PROJENOeski != PROJENO)
                {
                    PROJENOeski = PROJENO;
                    pb = new GeciciProjeBilgi();
                    pb.DPTPROJENO = sorgu.StringAl(0);
                    pb.NO = PROJENO;
                    pb.AD = sorgu.StringAl(2);
                    pb.BASLANGICYILI = sorgu.IntAl(3);

                    pb.SEKTORNO = sorgu.StringAl(10);
                    pb.SEKTORAD = sorgu.StringAl(11);
                    pb.YURUTECEKKURUM = sorgu.StringAl(12);
                    pb.KURAD = sorgu.StringAl(13);

                    pb.altlar = new List<GeciciProjeBilgi>();
                    projeler.Add(pb);
                    apb = null;
                }

                if (apb == null || AP_ALTPROJENOeski != AP_ALTPROJENO)
                {
                    AP_ALTPROJENOeski = AP_ALTPROJENO;
                    apb = new GeciciProjeBilgi();

                    apb.NO = AP_ALTPROJENO;
                    apb.AD = sorgu.StringAl(5);
                    apb.BASLANGICYILI = sorgu.IntAl(6);
                    apb.AP_DPTDEVAR = sorgu.IntAl(7);
                    if (apb.AP_DPTDEVAR == 1)
                        pb.altlar.Add(apb);
                }

                string PK_PEBKOD = sorgu.StringAl(8);
                int K_TOPLAMADAHIL = sorgu.IntAl(9);

                OdenekBilgi ob = PebKodBul(odnler, PK_PEBKOD);
                if (ob != null)
                {
                    pb.Kbo += (double)ob.ekRakamlar[0];
                    pb.eklenenDusulen += (double)ob.ekRakamlar[1];
                    pb.Harcama += (double)ob.ekRakamlar[2];
                    apb.Kbo += (double)ob.ekRakamlar[0];
                    apb.eklenenDusulen += (double)ob.ekRakamlar[1];
                    apb.Harcama += (double)ob.ekRakamlar[2];
                }
            }

            Aspose.Words.Document doc = new Aspose.Words.Document(sablonAd);
            DocumentBuilder builder = new DocumentBuilder(doc);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYil", butceYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TabloNo", this.tabloNo);
            if (!string.IsNullOrWhiteSpace(sorgu.hata))
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TabloAd", sorgu.hata);
            else
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TabloAd", this.tabloAd);

            DataTable dt = new DataTable();
            dt.TableName = "Veri";
            dt.Columns.Add("ProjeAd", typeof(String));
            dt.Columns.Add("KBO", typeof(String));
            dt.Columns.Add("Revize", typeof(String));
            dt.Columns.Add("Harcama", typeof(String));
            dt.Columns.Add("Oran", typeof(String));

            double tKbo = 0;
            double tRevize = 0;
            double tHarcama = 0;
            string eskiGrup = "";
            DataRow grupSat = null;
            double top1 = 0, top2 = 0, top3 = 0;
            foreach (GeciciProjeBilgi us in projeler)
            {
                if (us.DoluMu())
                {
                    string adEk = "";
                    bool altProjeVar = us.YazilacakAltProjeVar(out adEk);
                    if (YeniGrup(grup, ref eskiGrup, us))
                    {
                        GrupDegerGuncelle(grupSat, top1, top2, top3);
                        top1 = 0;
                        top2 = 0;
                        top3 = 0;
                        grupSat = ProjeGrupEkle(dt, eskiGrup);
                    }
                    ProjeDosyaEkle(dt, us, false, adEk, grup);
                    if (altProjeVar)
                    {
                        foreach (GeciciProjeBilgi item in us.altlar)
                        {
                            if (item.DoluMu())
                                ProjeDosyaEkle(dt, item, true, "", grup);
                        }
                    }
                    top1 += us.Kbo;
                    top2 += us.Kbo + us.eklenenDusulen;
                    top3 += us.Harcama;
                    tKbo += us.Kbo;
                    tRevize += us.Kbo + us.eklenenDusulen;
                    tHarcama += us.Harcama;
                }
            }
            GrupDegerGuncelle(grupSat, top1, top2, top3);

            if (tKbo + tRevize != 0)
            {
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TKBO", StringYap(tKbo / Bolum));
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TRevize", StringYap(tRevize / Bolum));
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "THarcama", StringYap(tHarcama / Bolum));
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TOran", StringYapOran(tHarcama, tRevize));
                doc.MailMerge.ExecuteWithRegions(dt);

                Table tbl = doc.FirstSection.Body.Tables[0];
                //string onBos = "";
                //if (grup != 0) onBos = "   ";

                for (int i = 2; i < tbl.Rows.Count - 1; i++)
                {
                    Cell aktifHucre = tbl.Rows[i].Cells[0];
                    string aktifHucreVeri = aktifHucre.GetText();
                    bool koyuYap = false;
                    bool arkaAlan = false;
                    int tabSay = 0;
                    if (aktifHucreVeri.StartsWith("\t\t\t")) tabSay = 3;
                    else if (aktifHucreVeri.StartsWith("\t\t")) tabSay = 2;
                    else if (aktifHucreVeri.StartsWith("\t")) tabSay = 1;
                    if (tabSay > 0)
                    {
                        aktifHucreVeri = aktifHucreVeri.Substring(tabSay);
                        //aktifHucre.RemoveAllChildren();
                        Run pr = (Run)aktifHucre.FirstParagraph.Runs[0].Clone(true);
                        pr.Text = aktifHucreVeri;

                        aktifHucre.RemoveAllChildren();
                        aktifHucre.EnsureMinimum();

                        aktifHucre.Paragraphs[0].AppendChild(pr);

                        aktifHucre.FirstParagraph.ParagraphFormat.LeftIndent = (tabSay - 1) * 6;
                    }

                    if (tabSay == 3 || (tabSay == 2 && grup == 0))
                    {
                        koyuYap = false;
                        arkaAlan = false;
                    }
                    else
                    {
                        koyuYap = true;
                        if (grup != 0)
                        {
                            if (tabSay == 1) arkaAlan = true;
                        }
                    }

                    if (arkaAlan || koyuYap)//satırı koyu yap
                    {
                        for (int colIdx = 0; colIdx < tbl.Rows[i].Cells.Count; colIdx++)
                        {
                            Cell formatHucre = tbl.Rows[i].Cells[colIdx];
                            if (arkaAlan)
                                formatHucre.CellFormat.Shading.BackgroundPatternColor = System.Drawing.Color.FromArgb(253, 245, 230);  //System.Drawing.Color.LightGray;//Color.FromName("#cccccc"); 
                            NodeCollection runs = formatHucre.GetChildNodes(NodeType.Run, true);
                            foreach (Run run in runs)
                            {
                                run.Font.Bold = true;
                            }
                        }
                    }
                }
            }
            else
            {
                // Remove paragraphs with no text.
                Node[] paragraphs = doc.GetChildNodes(NodeType.Table, true).ToArray();
                foreach (Node tab in paragraphs)
                {
                    tab.Remove();
                }

                //paragraphs = doc.GetChildNodes(NodeType.Paragraph, true).ToArray();
                //foreach (Node paragraph in paragraphs)
                //{
                //    paragraph.Remove();
                //}
            }
            doc.Save(this.sonucDosyaAdWrd);
            return this.sonucDosyaAdWrd;
        }

        private void GrupDegerGuncelle(DataRow grupSat, double Kbo, double revizeOdenek, double Harcama)
        {
            if (grupSat != null)
            {
                grupSat[1] = StringYap(Kbo / Bolum);
                grupSat[2] = StringYap(revizeOdenek / Bolum);
                grupSat[3] = StringYap(Harcama / Bolum);
                grupSat[4] = StringYapOran(Harcama, revizeOdenek);
            }
        }

        private DataRow ProjeGrupEkle(DataTable dt, string eskiGrup)
        {
            return dt.Rows.Add("\t" + eskiGrup, "", "", "", "");
        }

        private bool YeniGrup(int grup, ref string eskiGrup, GeciciProjeBilgi us)
        {
            if (grup == 0) return false;
            else
            {
                string grupStr;
                if (grup == 1) grupStr = us.SEKTORAD;
                else grupStr = us.KURAD;
                if (string.IsNullOrWhiteSpace(eskiGrup) || eskiGrup != grupStr)
                {
                    eskiGrup = grupStr;
                    return true;
                }
            }
            return false;
        }

        private void ProjeDosyaEkle(DataTable dt, GeciciProjeBilgi us, bool altProje, string adEk, int grup)
        {
            string onBos = "\t";
            if (grup != 0) onBos = "\t\t";
            if (!string.IsNullOrWhiteSpace(adEk)) adEk = " (" + adEk + ")";
            dt.Rows.Add((altProje ? (onBos + "\t") : (onBos + "")) + us.AD + adEk, StringYap(us.Kbo / Bolum), StringYap((us.Kbo + us.eklenenDusulen) / Bolum), StringYap(us.Harcama / Bolum), StringYapOran(us.Harcama, us.Kbo + us.eklenenDusulen));
        }

        private OdenekBilgi PebKodBul(OdenekBilgi[] odnler, string PEBKOD)
        {
            foreach (OdenekBilgi item in odnler)
            {
                if (item.tertip.kodlar[4].Replace(".", "") == PEBKOD)
                    return item;
            }
            return null;
        }

        //private void ODNLERYaz(OdenekBilgi[] odnler)
        //{
        //    foreach (OdenekBilgi item in odnler)
        //    {
        //        BilgiYaz(string.Format("{0}: {1} {2} {3}", item.tertip.TertipStrOl(), item.ekRakamlar[0], item.ekRakamlar[1], item.ekRakamlar[2]));
        //    }
        //}

        //private void BilgiYaz(string v)
        //{
        //    OrtakFonksiyonlar.HataDosyaYaz("RaporDebug.txt", v, false);
        //}

        private string GiderSiniflandirmasiRaporuHazirla(int kodNo, int kirilimSayCagiran, Tertip kistas, UcgenKodBilgileri kodBilgileri, bool sadeceSonAsama, bool prg = false) //int kirilimSay, int kodNo, string kriter = "", bool sadeceSonAsama = false)
        {
            EkDegerParam[] ekBilgiler = null;
            ekBilgiler = new EkDegerParam[3];
            if (prg)
            {
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPRGOdenek, 0, 0, "06.00");
                ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPRGMasrafPlaniForm, 0, 0, ";;" + TNS.BHMv2.Arac.EklemeDusmeIslemler() + ";;");
                ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPRGHarcama, 0, 0, "12;830");
            }
            else
            {
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, "06.00");
                ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABMasrafPlaniForm, 0, 0, ";;" + TNS.BHMv2.Arac.EklemeDusmeIslemler() + ";;");
                ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPEBHarcama, 0, 0, "12");
            }
            int kirilimSay = kodBilgileri.KirilimSayisi;

            UcgenSatir[] odnler = servisBHMv2.UcgenTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, true);

            Aspose.Words.Document doc = new Aspose.Words.Document(sablonAd);
            DocumentBuilder builder = new DocumentBuilder(doc);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ButceYil", butceYil.ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TabloNo", this.tabloNo);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TabloAd", this.tabloAd);

            if (!prg)
            {
                string kodAd = "";
                for (int i = 0; i < kodBilgileri.BoyutSayisi; i++)
                {
                    if (kodAd != "") kodAd += "/";
                    kodAd += KodAdBul(kodBilgileri[i].kodNo);
                }
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KodAd", kodAd);
            }

            if (sadeceSonAsama)
            {
                Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
                Row row = table.Rows[0];
                //row.Cells[1].CellFormat.PreferredWidth = PreferredWidth.FromPoints(row.Cells[1].CellFormat.PreferredWidth.Value - 38);
                row.Cells[0].CellFormat.PreferredWidth = PreferredWidth.FromPoints(row.Cells[0].CellFormat.PreferredWidth.Value + 38);
            }
            DataTable dt = new DataTable();
            dt.TableName = "Veri";
            if (prg)
            {
                dt.Columns.Add("KodAd", typeof(String));
            }
            else
            {
                dt.Columns.Add("EkoKod", typeof(String));
                dt.Columns.Add("EkoAd", typeof(String));
            }
            dt.Columns.Add("KBO", typeof(String));
            dt.Columns.Add("Revize", typeof(String));
            dt.Columns.Add("Harcama", typeof(String));
            dt.Columns.Add("Oran", typeof(String));

            double tKbo = 0;
            double tRevize = 0;
            double tHarcama = 0;
            string[] kodParcala = new string[4];
            foreach (UcgenSatir us in odnler)
            {
                bool yaz;
                double Kbo = (double)us.ekDegerler[0];
                double Revize = Kbo + (double)us.ekDegerler[1];
                double Harcama = (double)us.ekDegerler[2];

                kodParcala[us.yer] = us.kodParca;

                if (us.yer < (kirilimSayCagiran - 1) && sadeceSonAsama)
                    yaz = false;
                else
                    yaz = true;
                string yazKod;
                if (sadeceSonAsama && yaz && !prg)
                    yazKod = String.Join(".", kodParcala, 0, us.yer + 1);
                else
                {
                    yazKod = us.kodParca;
                    if (kirilimSayCagiran == 2 && kodNo == 4 && us.yer == 1)
                    {
                        yazKod = "*" + yazKod;
                    }
                }
                if (yaz)
                {
                    if (prg)
                        dt.Rows.Add("".PadLeft(us.yer, ' ') + yazKod + "- " + us.aciklama, StringYap(Kbo / Bolum), StringYap(Revize / Bolum), StringYap(Harcama / Bolum), StringYapOran(Harcama, Revize));
                    else
                        dt.Rows.Add(yazKod, us.aciklama, StringYap(Kbo / Bolum), StringYap(Revize / Bolum), StringYap(Harcama / Bolum), StringYapOran(Harcama, Revize));
                }
                if (us.yer == 0)
                {
                    tKbo += Kbo;
                    tRevize += Revize;
                    tHarcama += Harcama;
                }
            }
            if (tKbo == 0)
            {
                if (prg)
                    dt.Rows.Add("", "", "", "", "");
                else
                    dt.Rows.Add("", "", "", "", "", "");
            }
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TKBO", StringYap(tKbo / Bolum));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TRevize", StringYap(tRevize / Bolum));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "THarcama", StringYap(tHarcama / Bolum));
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TOran", StringYapOran(tHarcama, tRevize));
            doc.MailMerge.ExecuteWithRegions(dt);
            if (kirilimSay > 1)
            {
                Table tbl = doc.FirstSection.Body.Tables[0];

                for (int i = 1; i < tbl.Rows.Count - 1; i++)
                {
                    Cell aktifHucre = tbl.Rows[i].Cells[0];
                    string aktifHucreVeri = aktifHucre.GetText();
                    if (!string.IsNullOrWhiteSpace(aktifHucreVeri))
                    {
                        if (!prg)
                        {
                            if (aktifHucreVeri.StartsWith("*")) //ilk hücreyi sağa daya
                            {
                                aktifHucre.FirstParagraph.Runs.Clear();
                                aktifHucre.FirstParagraph.AppendChild(new Run(doc, aktifHucreVeri.Substring(1)));
                                foreach (Paragraph p in aktifHucre.Paragraphs)
                                {
                                    p.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                                }
                            }
                            else if (aktifHucreVeri.Length == 3) //satırı koyu yap
                            {
                                for (int colIdx = 0; colIdx < tbl.Rows[i].Cells.Count; colIdx++)
                                {
                                    Cell formatHucre = tbl.Rows[i].Cells[colIdx];
                                    NodeCollection runs = formatHucre.GetChildNodes(NodeType.Run, true);
                                    foreach (Run run in runs)
                                    {
                                        run.Font.Bold = true;
                                    }
                                }
                            }
                        }
                        else if (aktifHucreVeri.Length > 3)
                        {
                            string ilk2 = aktifHucreVeri.Substring(0, 2);
                            string ilk1 = aktifHucreVeri.Substring(0, 1);
                            aktifHucre.FirstParagraph.Runs.Clear();
                            aktifHucre.FirstParagraph.AppendChild(new Run(doc, aktifHucreVeri.TrimStart()));
                            if (ilk2 == "  ")
                            {
                                aktifHucre.FirstParagraph.ParagraphFormat.LeftIndent = 20;
                            }
                            else
                            {
                                NodeCollection runs = aktifHucre.GetChildNodes(NodeType.Run, true);
                                foreach (Run run in runs)
                                {
                                    run.Font.Bold = true;
                                }
                                if (ilk1 == " ")
                                    aktifHucre.FirstParagraph.ParagraphFormat.LeftIndent = 10;
                            }
                        }
                    }
                }
            }
            doc.Save(this.sonucDosyaAdWrd);
            return this.sonucDosyaAdWrd;
        }

        private string GiderSiniflandirmasiRaporuHazirla(int kodNo, int kirilimSay, string kriter = "", bool sadeceSonAsama = false)
        {
            Tertip kistas = null;
            UcgenKodBilgileri kodBilgileri = null;
            kistas = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, GenelIslemlerIstemci.VarsayilanKurumBul(), "", "", "", "");
            if (!string.IsNullOrWhiteSpace(kriter))
                kistas.kodlar[kodNo - 1] = kriter;
            kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(kodNo - 1), kirilimSay));

            return GiderSiniflandirmasiRaporuHazirla(kodNo, kirilimSay, kistas, kodBilgileri, sadeceSonAsama);
        }

        private string GiderSiniflandirmasiProgramRaporuHazirla()
        {
            Tertip kistas = null;
            UcgenKodBilgileri kodBilgileri = null;
            kistas = new Tertip(butceYil, ENUMTabloKod.TABPRGOdenek, "", "", "", "", "", GenelIslemlerIstemci.VarsayilanKurumBul());
            kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(1), 1));
            kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(2), 1));
            kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(3), 1));

            return GiderSiniflandirmasiRaporuHazirla(33, 3, kistas, kodBilgileri, false, true);
        }

        private string GiderSiniflandirmasiEkoFinHazirla()
        {
            Tertip kistas = null;
            UcgenKodBilgileri kodBilgileri = null;
            kistas = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek, GenelIslemlerIstemci.VarsayilanKurumBul(), "", "", "06,07,08", "");
            kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(3), 1));
            kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(2), 1));

            return GiderSiniflandirmasiRaporuHazirla(4, 2, kistas, kodBilgileri, false);
        }

        private string KodAdBul(int kodNo)
        {
            if (kodNo == 1)
                return "Kurumsal Kod";
            else if (kodNo == 2)
                return "Fonk. Kod";
            else if (kodNo == 3)
                return "Fin. Kod";
            else if (kodNo == 4)
                return "Eko. Kod";
            return kodNo.ToString() + ". kod";
        }

        private string StringYap(double p, int ondalik = 0)
        {
            string format = "#,##0";
            if (ondalik > 0)
            {
                format = format + "." + "0".PadLeft(ondalik, '0');
            }
            return p.ToString(format);
        }

        private string StringYapOran(double bolunen, double bolen, int ondalik = 0)
        {
            if (bolen == 0) return "";
            return StringYap(bolunen * 100 / bolen, ondalik);
        }

        private string StringAYapOran(double onceki, double sonraki, int ondalik)
        {
            return StringYapOran(sonraki - onceki, onceki, ondalik);
        }

        public double Bolum { get { return 1; } }

        public string tabloNo { get; set; }

        public string tabloAd { get; set; }
    }
}