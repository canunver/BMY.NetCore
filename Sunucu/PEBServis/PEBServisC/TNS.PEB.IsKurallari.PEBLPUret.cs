using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.PEB;
using TNS.PEB.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using lpsolve55;

namespace TNS.PEB.IsKurallari
{

    enum GirdiCikti
    {
        Girdi,
        Cikti
    }

    class ModelCozum
    {
        public double[] degerler = null;

        public ModelCozum(int degiskenSay)
        {
            degerler = new double[degiskenSay];
        }

        public double obj;
        public int ornekOkulSay = 0;
    }

    class ModelDegisken
    {
        public string kod;
        public GirdiCikti girdiCikti;
        public string ad;
        public int gostergeKod;

        public ModelDegisken(string kod, string ad, int gostergeKod, GirdiCikti girdiCikti)
        {
            this.kod = kod;
            this.ad = ad;
            this.girdiCikti = girdiCikti;
            this.gostergeKod = gostergeKod;
        }
    }

    class ModelVeri
    {
        public string kurumKod;
        public string kurumAd;
        public double[] degerler = null;

        public ModelCozum cozum = null;

        public double degisimOran(int i)
        {
            double girdi = degerler[i];
            double cikti = cozum.degerler[i];

            if (girdi != 0)
                return (cikti - girdi) * 100 / girdi;
            else if (cikti == 0)
                return 0;
            return 100000;
        }

        public ModelVeri(string kurumAd, string kurumKod, int degerSay)
        {
            this.kurumAd = kurumAd;
            this.kurumKod = kurumKod;
            degerler = new double[degerSay];
            cozum = new ModelCozum(degerSay);
        }

        public double this[int index]
        {
            get
            {
                return degerler[index];
            }
            set
            {
                if (value == 0)
                    degerler[index] = 0; //0.0001;
                else
                    degerler[index] = value;
            }
        }
    }

    public class DataBaseBilgi
    {
        public VeriErisim ve;
        public TABVZATablolar tab;
        public int islemYil;
        public int grupKod;
        public int altKod;
        public string cogKod;
        public Sonuc sonuc;
    }

    class Model
    {
        System.Xml.XmlTextWriter xmlWriter;
        private string modelAd;
        private System.Collections.Generic.List<ModelDegisken> girdiCiktilar = new System.Collections.Generic.List<ModelDegisken>();
        private System.Collections.Generic.List<ModelVeri> modelVeriler = new System.Collections.Generic.List<ModelVeri>();

        public Model(string modelAd)
        {
            // TODO: Complete member initialization
            this.modelAd = modelAd;
        }

        internal void GirdiCiktiEkle(string kod, string ad, int gostergeKod, GirdiCikti gc)
        {
            girdiCiktilar.Add(new ModelDegisken(kod, ad, gostergeKod, gc));
        }

        internal void VeriEkle(ModelVeri modelVeri)
        {
            modelVeriler.Add(modelVeri);
        }

        private /* unsafe  */ static bool ctrlcfunc(IntPtr lp, int userhandle)
        {
            /* 'If set to true, then solve is aborted and returncode will indicate this. */
            return (false);
        }

        private int KurumSay()
        {
            return modelVeriler.Count;
        }

        internal int DegiskenSay()
        {
            return girdiCiktilar.Count;
        }

        internal string DegiskenKod(int i)
        {
            return girdiCiktilar[i].kod;
        }

        private int LPKolonSayBul()
        {
            return 1 + KurumSay() + DegiskenSay();
        }

        private bool AmacMax()
        {
            if (modelAd == "CRS-OO" || modelAd == "VRS-OO") return true;
            else return false;
        }

        private double EpsilonBul()
        {
            if (modelAd == "CRS-OO" || modelAd == "VRS-OO") return 0.0000001;
            else return -0.0000001;
        }

        private lpsolve.lpsolve_constr_types LambdaKisitBul()
        {
            if (modelAd == "VRS-IO" || modelAd == "VRS-OO")
                return lpsolve.lpsolve_constr_types.EQ;
            else
                return lpsolve.lpsolve_constr_types.GE;
        }

        private double LambdaToplamDeger()
        {
            if (modelAd == "VRS-IO" || modelAd == "VRS-OO") return 1;
            else return 0;
        }

        private bool GirdiMi()
        {
            if (modelAd == "CRS-OO" || modelAd == "VRS-OO") return false;
            else return true;
        }

        internal void CozKurum(string yol, ModelVeri kurumVeri)
        {
            lpsolve55.lpsolve.Init(".");
            IntPtr lp;
            int kolonSay = LPKolonSayBul();
            lp = lpsolve.make_lp(0, kolonSay); //Lp Kolon Sayısı
            try
            {
                //OrtakFonksiyonlar.HataDosyaYaz("LPTakip.txt", "gir", false);
                lpsolve.put_abortfunc(lp, new lpsolve.ctrlcfunc(ctrlcfunc), 0);
                lpsolve.set_timeout(lp, 0);

                if (AmacMax())
                    lpsolve.set_maxim(lp);
                else
                    lpsolve.set_minim(lp);

                double e = EpsilonBul();

                double[] objDizi = new double[kolonSay + 1];
                double[] lamdaDizi = new double[kolonSay + 1];

                objDizi[0] = 0; //kullanılmayan boyut;
                lamdaDizi[0] = 0; //kullanılmayan değişken;

                objDizi[1] = 1; //TETA değişkeni;
                lamdaDizi[1] = 0; //TETA değişkeni;

                for (int i = 0; i < KurumSay(); i++) //Lambda değişkeni katsayıları
                {
                    objDizi[2 + i] = 0;
                    lamdaDizi[2 + i] = 1;
                }

                int degiskenBasla = 2 + KurumSay();
                for (int i = 0; i < DegiskenSay(); i++) //S değişkeni katsayıları
                {
                    objDizi[degiskenBasla + i] = e;
                    lamdaDizi[degiskenBasla + i] = 0;
                }

                lpsolve.set_obj_fn(lp, objDizi);
                ModelHedefFonsiyonYaz(objDizi, AmacMax());

                for (int i = 0; i < DegiskenSay(); i++)
                {
                    double[] degiskenKatsayilar = new double[kolonSay + 1];
                    degiskenKatsayilar[0] = 0; //kullanılmayan değişken;
                    bool degiskenKatsayiMi = ((GirdiMi() && girdiCiktilar[i].girdiCikti == GirdiCikti.Girdi) || (!GirdiMi() && girdiCiktilar[i].girdiCikti == GirdiCikti.Cikti));
                    if (degiskenKatsayiMi)
                        degiskenKatsayilar[1] = 0 - kurumVeri[i]; //TETA değişkeni katsayısı
                    else
                        degiskenKatsayilar[1] = 0; //TETA değişkeni katsayısı;

                    int lambdaNo = 0;
                    foreach (ModelVeri v in modelVeriler) //Lambda değişkeni katsayıları
                    {
                        degiskenKatsayilar[2 + lambdaNo++] = v[i];
                    }

                    for (int j = 0; j < DegiskenSay(); j++) //S DEğişkeni katsayıları
                    {
                        if (j != i)
                            degiskenKatsayilar[degiskenBasla + j] = 0;
                        else
                        {
                            if (girdiCiktilar[i].girdiCikti == GirdiCikti.Girdi)
                                degiskenKatsayilar[degiskenBasla + j] = 1;
                            else
                                degiskenKatsayilar[degiskenBasla + j] = -1;
                        }
                    }

                    double hedef;
                    if (degiskenKatsayiMi)
                        hedef = 0;
                    else
                        hedef = kurumVeri[i];

                    lpsolve.add_constraint(lp, degiskenKatsayilar, lpsolve.lpsolve_constr_types.EQ, hedef);
                    ModelKisitYaz(degiskenKatsayilar, lpsolve.lpsolve_constr_types.EQ, hedef);
                }
                ModelKisitYaz(lamdaDizi, LambdaKisitBul(), LambdaToplamDeger());
                lpsolve.add_constraint(lp, lamdaDizi, LambdaKisitBul(), LambdaToplamDeger());

                //OrtakFonksiyonlar.HataDosyaYaz("LPTakip.txt", "b solve", false);
                lpsolve.lpsolve_return ret = lpsolve.lpsolve_return.OPTIMAL;
                ret = lpsolve.solve(lp); //çöz
                if (ret != lpsolve.lpsolve_return.OPTIMAL)
                    throw new Exception("Çözüm optimal değil!");

                //OrtakFonksiyonlar.HataDosyaYaz("LPTakip.txt", "b get_objective", false);
                kurumVeri.cozum.obj = lpsolve.get_objective(lp); //objective sonucunu oku

                if (!GirdiMi() && kurumVeri.cozum.obj > 0) kurumVeri.cozum.obj = 1 / kurumVeri.cozum.obj;
                //OrtakFonksiyonlar.HataDosyaYaz("LPTakip.txt", "b variables", false);
                double[] variables = new double[lpsolve.get_Ncolumns(lp)];//Çözücüden dönen sütun değişkenlerini okumak için dizi değişken
                lpsolve.get_variables(lp, variables);//Sütun değişkenlerinin sonucunu oku
                int lBasla = 1;
                int yer = 0;

                //OrtakFonksiyonlar.HataDosyaYaz("LPTakip.txt", "b dongu", false);
                foreach (ModelVeri v in modelVeriler)
                {
                    for (int sd = 0; sd < DegiskenSay(); sd++)
                    {
                        kurumVeri.cozum.degerler[sd] += v.degerler[sd] * variables[lBasla + yer];
                    }

                    if (variables[lBasla + yer] > 0.00000001)
                        v.cozum.ornekOkulSay += 1;
                    yer++;
                }

                //OrtakFonksiyonlar.HataDosyaYaz("LPTakip.txt", "b const", false);
                double[] constraints = new double[lpsolve.get_Nrows(lp)];//Çözücüden gelen satır değişkenlerini okumak için dizi değişken
                //OrtakFonksiyonlar.HataDosyaYaz("LPTakip.txt", "b get const", false);
                lpsolve.get_constraints(lp, constraints);//Satır değişkenlerinin sonucunu oku

                //OrtakFonksiyonlar.HataDosyaYaz("LPTakip.txt", "b yaz", false);
                ModelSonucYaz(kurumVeri.cozum.obj, variables, constraints);
                lpsolve.delete_lp(lp);
            }
            catch(Exception ex)
            {
                lpsolve.delete_lp(lp);
                throw ex;
            }

            /** Kaldır 

            lpsolve.set_lp_name(lp, kurumVeri.kurumAd + " Performans");

            lpsolve.write_lp(lp, yol + "/" + kurumVeri.kurumKod + ".lp");
            lpsolve.write_mps(lp, yol + "/" + kurumVeri.kurumKod + ".mps");
             Kaldır son **/

            //OrtakFonksiyonlar.HataDosyaYaz("LPTakip.txt", "b sil", false);
            //OrtakFonksiyonlar.HataDosyaYaz("LPTakip.txt", "çık", false);
        }

        //private void YaziYaz(string yazi, bool tabEkle, bool enterEkle)
        //{
        //    Console.Write(yazi);
        //    if (tabEkle)
        //        Console.Write("\t");
        //    if (enterEkle)
        //        Console.WriteLine();
        //}

        private void ModelBaslikYaz(ModelVeri kurumVeri)
        {
            xmlWriter.WriteStartElement("MODEL");
            xmlWriter.WriteAttributeString("KURUM", kurumVeri.kurumAd);
            xmlWriter.WriteAttributeString("KURUMKOD", kurumVeri.kurumAd);
            //YaziYaz(kurumVeri.kurumAd, false, true);
        }

        private void ModelSonucYaz(ModelVeri kurumVeri, DataBaseBilgi dbBilgi)
        {
            xmlWriter.WriteStartElement("SONUC");
            xmlWriter.WriteAttributeString("KURUM", kurumVeri.kurumAd);
            xmlWriter.WriteAttributeString("KURUMKOD", kurumVeri.kurumKod);
            AttrDblYaz("ORNEK", kurumVeri.cozum.ornekOkulSay);
            AttrDblYaz("AMAC", kurumVeri.cozum.obj);
            if (dbBilgi != null)
            {
                if (dbBilgi.sonuc.islemSonuc)
                    dbBilgi.sonuc = dbBilgi.tab.SonucEkle(dbBilgi.islemYil, dbBilgi.grupKod, dbBilgi.altKod, dbBilgi.cogKod, OrtakFonksiyonlar.ConvertToInt(kurumVeri.kurumKod, -1), kurumVeri.cozum.obj, kurumVeri.cozum.ornekOkulSay);
            }
            //YaziYaz(kurumVeri.kurumAd, false, true);
        }

        //private void DoubleYaz(double d)
        //{
        //    YaziYaz(d.ToString("0.0000"), true, false);
        //}

        //private void DiziYaz(double[] dizi, bool ilkiGereksiz)
        //{
        //    if (dizi == null) return;
        //    int ilk = 0;
        //    if (ilkiGereksiz)
        //        ilk = 1;
        //    for (int i = ilk; i < dizi.Length; i++)
        //    {
        //        DoubleYaz(dizi[i]);
        //    }
        //}


        private void SonucYaz(double[] dizi, string onEk)
        {
            if (dizi == null) return;

            for (int i = DegiskenSay() - 1; i >= 0; i--)
            {
                AttrDblYaz(onEk + "S" + (i + 1).ToString(), dizi[i]);
            }

        }

        private void DiziAttributeYaz(double[] dizi, bool gereksizVar)
        {
            if (dizi == null) return;

            int ilk = 0;
            if (gereksizVar)
            {
                //AttrDblYaz("G", dizi[0]);
                ilk = 1;
            }

            int kurumBasla = ilk + 1 + KurumSay();
            for (int i = DegiskenSay() - 1; i >= 0; i--)
            {
                AttrDblYaz("S" + (i + 1).ToString(), dizi[i + kurumBasla]);
            }

            for (int i = KurumSay() - 1; i >= 0; i--)
            {
                AttrDblYaz("L" + (i + 1).ToString(), dizi[i + ilk + 1]);
            }

            AttrDblYaz("TETA", dizi[ilk]);
        }

        private void AttrDblYaz(string ad, double deger)
        {
            xmlWriter.WriteAttributeString(ad, deger.ToString("0.0000"));
        }

        private void ModelHedefFonsiyonYaz(double[] dizi, bool amacMax)
        {
            string kosul = "";
            if (amacMax) kosul = "MAX";
            else kosul = "MIN";

            xmlWriter.WriteStartElement("HEDEF");

            DiziAttributeYaz(dizi, true);
            xmlWriter.WriteAttributeString("KOSUL", kosul);
            xmlWriter.WriteEndElement();
            //DiziYaz(dizi, true);
            //YaziYaz(kosul, false, true);
        }

        private void ModelKisitYaz(double[] dizi, lpsolve.lpsolve_constr_types lpsolve_constr_types, double hedef)
        {
            xmlWriter.WriteStartElement("KISIT");
            DiziAttributeYaz(dizi, true);
            //DiziYaz(dizi, true);
            string kosul = "";
            if (lpsolve_constr_types == lpsolve.lpsolve_constr_types.EQ) kosul = "EQ";
            else if (lpsolve_constr_types == lpsolve.lpsolve_constr_types.GE) kosul = "GE";
            else if (lpsolve_constr_types == lpsolve.lpsolve_constr_types.LE) kosul = "LE";

            //YaziYaz(kosul, true, false);
            xmlWriter.WriteAttributeString("KOSUL", kosul);

            AttrDblYaz("SABIT", hedef);
            //DoubleYaz(hedef);

            //YaziYaz("", false, true);
            xmlWriter.WriteEndElement();
        }

        private void ModelSonucYaz(double obj, double[] variables, double[] constraints)
        {
            xmlWriter.WriteStartElement("SONUC");
            AttrDblYaz("HEDEF", obj);

            DiziAttributeYaz(variables, false);
            //DiziYaz(variables, false);

            //DiziAttributeYaz("C", constraints, false);
            //DiziYaz(constraints);

            //DoubleYaz(obj);
            //YaziYaz("", false, true);
            xmlWriter.WriteEndElement();
        }

        internal void Coz(string yol, string cikisDosyaXml, DataBaseBilgi dbBilgi)
        {
            xmlWriter = new System.Xml.XmlTextWriter(cikisDosyaXml, System.Text.Encoding.Default);
            xmlWriter.Formatting = System.Xml.Formatting.Indented;
            xmlWriter.Indentation = 3;
            xmlWriter.IndentChar = ' ';

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("MODEL");
            bool hata = false;
            foreach (ModelVeri v in modelVeriler)
            {
                ModelBaslikYaz(v);
                try
                {
                    CozKurum(yol, v);
                }
                catch (Exception ex)
                {
                    dbBilgi.sonuc.DegerAta(ex.Message);
                    hata = true;
                }
                xmlWriter.WriteEndElement();
                if (hata) break;
            }
            if (!hata)
            {
                xmlWriter.WriteStartElement("COZUM");
                if (dbBilgi != null)
                {
                    dbBilgi.sonuc = new Sonuc(true, "", "");
                    dbBilgi.ve.BeginTransaction();
                }

                foreach (ModelVeri v in modelVeriler)
                {
                    ModelSonucYaz(v, dbBilgi);


                    xmlWriter.WriteStartElement("GIRDILER");
                    for (int i = 0; i < DegiskenSay(); i++)
                    {
                        xmlWriter.WriteStartElement("GIRDI");
                        AttrDblYaz("DEGER", v.degerler[i]);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("CIKTILAR");
                    for (int i = 0; i < DegiskenSay(); i++)
                    {
                        xmlWriter.WriteStartElement("CIKTI");
                        AttrDblYaz("DEGER", v.cozum.degerler[i]);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("ORANLAR");
                    for (int i = 0; i < DegiskenSay(); i++)
                    {
                        xmlWriter.WriteStartElement("ORAN");
                        double oran = v.degisimOran(i);
                        AttrDblYaz("DEGER", oran);
                        xmlWriter.WriteEndElement();
                        if (dbBilgi != null)
                        {
                            int gostergeKod = girdiCiktilar[i].gostergeKod;
                            if (dbBilgi.sonuc.islemSonuc)
                            {
                                dbBilgi.sonuc = dbBilgi.tab.SonucDegiskenEkle(dbBilgi.islemYil, dbBilgi.grupKod, dbBilgi.altKod, dbBilgi.cogKod, OrtakFonksiyonlar.ConvertToInt(v.kurumKod, -1), gostergeKod, v.degerler[i], v.cozum.degerler[i], oran);
                            }
                        }
                    }
                    xmlWriter.WriteEndElement();


                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("DEGISKENLER");
                xmlWriter.WriteAttributeString("SAY", DegiskenSay().ToString());
                for (int i = 0; i < DegiskenSay(); i++)
                {
                    xmlWriter.WriteStartElement("DEGISKEN");
                    xmlWriter.WriteAttributeString("AD", girdiCiktilar[i].ad);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }
            if (dbBilgi != null)
            {
                if (dbBilgi.sonuc.islemSonuc)
                    dbBilgi.ve.Commit();
                else
                    dbBilgi.ve.Rollback();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Close();
            //OrtakFonksiyonlar.HataDosyaYaz("LPTakip.txt", "bXXX", false);
        }
    }

    public class VeriZarflama
    {
        string girisDosyaXml;
        string cikisDosyaXml;
        string yol;
        public VeriZarflama(string yol, string girisDosyaXml, string cikisDosyaXml)
        {
            this.girisDosyaXml = girisDosyaXml;
            this.cikisDosyaXml = cikisDosyaXml;
            this.yol = yol;
        }

        public void Coz(DataBaseBilgi dbBilgi)
        {
            bool ilk = true;
            System.Xml.XmlTextReader x = new System.Xml.XmlTextReader(girisDosyaXml);
            Model model = null;
            ModelVeri modelVeri = null;
            try
            {
                while (x.Read())
                {
                    if (x.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        if (ilk)
                        {
                            if (x.Name.ToUpper() != "ZARFLAMA")
                            {
                                throw new Exception("ZARFLAMA ile başlamalı!");
                            }
                            ilk = false;
                            if (model == null)
                                model = new Model(x.GetAttribute("YONTEM"));
                            else
                                throw new Exception("İki MODEL olamaz!");
                        }
                        else
                        {
                            if (x.Name.ToUpper() == "GIRDI" || x.Name.ToUpper() == "CIKTI")
                            {
                                if (model != null)
                                {
                                    GirdiCikti gc;
                                    if (x.Name.ToUpper() == "GIRDI")
                                        gc = GirdiCikti.Girdi;
                                    else
                                        gc = GirdiCikti.Cikti;

                                    model.GirdiCiktiEkle(x.GetAttribute("KOD"), x.GetAttribute("AD"), OrtakFonksiyonlar.ConvertToInt(x.GetAttribute("GOSTERGEKOD"), 1), gc);
                                }
                                else
                                    throw new Exception("MODEL olmalı!");
                            }
                            else if (x.Name.ToUpper() == "VERI")
                            {
                                if (model != null)
                                {
                                    modelVeri = new ModelVeri(x.GetAttribute("KURUM"), x.GetAttribute("KURUMKOD"), model.DegiskenSay());
                                    for (int i = 0; i < model.DegiskenSay(); i++)
                                    {
                                        modelVeri[i] = OrtakFonksiyonlar.ConvertToDbl(x.GetAttribute(model.DegiskenKod(i)));
                                    }
                                    model.VeriEkle(modelVeri);
                                }
                                else
                                    throw new Exception("MODEL olmalı!");
                            }
                        }
                    }
                }
            }
            catch
            {
                x.Close();
                return;
            }

            x.Close();
            model.Coz(yol, cikisDosyaXml, dbBilgi);
        }
    }

    /// <summary>
    /// PEBLPUret formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class PEBLPUret
    {
        ///// <summary>
        ///// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        ///// </summary>
        //public static string VERIERISIMAD = "BaglantiSatiriPEB";

        ///// <summary>
        ///// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        ///// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        ///// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        ///// </summary>
        ///// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        //public static bool YazmaYetkisiVarMi(Kullanici kullanan)
        //{
        //    if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
        //        return true;
        //    return false;
        //}

        ///// <summary>
        ///// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
        ///// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        ///// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
        ///// </summary>
        ///// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        //public static Sonuc AlanlarGecerliMi(Kullanici kullanan, LPUretim form)
        //{
        //    return new Sonuc(true, "", "");
        //}

        ///// <summary>
        ///// LPUretim nesnesini veri tabanına yazan fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">LPUretim yapısında LPUretim: bilgilerini tutan değişken</param>
        ///// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        //public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, LPUretim form, bool ornekData)
        //{
        //    Sonuc s = new Sonuc(true, "", "");
        //    if (!YazmaYetkisiVarMi(kullanan))
        //    {
        //        s.DegerAta("Yazma yetkiniz yok!");
        //        return s;
        //    }

        //    if (form.siraNo != "")
        //    {
        //        s.DegerAta("Sadece yeni kayıtlar eklenebilir!");
        //        return s;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABPEBLPURETIM tabPEBLPURETIM = new TABPEBLPURETIM(ve);

        //    ve.BeginTransaction();

        //    form.siraNo = tabPEBLPURETIM.SiraNoBul(form.yil).ToString();

        //    if (ornekData)
        //    {
        //        form.durum = 2;
        //        form.orgTur = "";
        //        form.orgKod = 999999;
        //    }
        //    else
        //        form.durum = 0;

        //    if (s.islemSonuc)
        //        s = tabPEBLPURETIM.Ekle(form);

        //    if (!ornekData)
        //    {
        //        for (int i = 0; i < form.degiskenler.Length && s.islemSonuc; i++)
        //        {
        //            s = tabPEBLPURETIM.DetayEkle(form, i);
        //        }
        //    }
        //    if (s.islemSonuc)
        //    {
        //        ve.Commit();
        //        s.anahtar = form.siraNo.ToString();
        //    }
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();
        //    return s;
        //}

        ///// <summary>
        ///// LPUretim nesnesini veri tabanından okuyan fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">LPUretim yapısında LPUretim: bilgilerini tutan değişken</param>
        //public static LPUretim[] Listele(VeriErisim veriErisim, Kullanici kullanan, LPUretim form, bool detayOku)
        //{
        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD);
        //    TABPEBLPURETIM tabAna = new TABPEBLPURETIM(ve);

        //    IDataReader dr = tabAna.Listele(form);
        //    if (dr == null)
        //    {
        //        ve.VeriErisimKapat();
        //        throw new Exception(ve.hata);
        //    }
        //    ArrayList al = new ArrayList();
        //    while (dr.Read())
        //    {
        //        LPUretim formIc = new LPUretim();
        //        formIc.yil = VeriErisim.DegerAl(dr, (int)TABPEBLPURETIM.ENUMAlanlar.YIL, 0);
        //        formIc.siraNo = VeriErisim.DegerAl(dr, (int)TABPEBLPURETIM.ENUMAlanlar.SIRANO, 0).ToString();
        //        formIc.aciklama = VeriErisim.DegerAl(dr, (int)TABPEBLPURETIM.ENUMAlanlar.ACIKLAMA, "");
        //        formIc.orgKod = VeriErisim.DegerAl(dr, (int)TABPEBLPURETIM.ENUMAlanlar.ORGKOD, 0);
        //        formIc.orgTur = VeriErisim.DegerAl(dr, (int)TABPEBLPURETIM.ENUMAlanlar.ORGTUR, "");
        //        formIc.cozumYontem = VeriErisim.DegerAl(dr, (int)TABPEBLPURETIM.ENUMAlanlar.COZUMYONTEM, 0);
        //        formIc.durum = VeriErisim.DegerAl(dr, (int)TABPEBLPURETIM.ENUMAlanlar.DURUM, 0);
        //        al.Add(formIc);
        //    }

        //    dr.Close();
        //    if (detayOku)
        //    {
        //        foreach (LPUretim item in al)
        //        {
        //            item.degiskenler = DegiskenOku(tabAna, item);
        //        }
        //    }
        //    ve.VeriErisimKapat();
        //    return (LPUretim[])al.ToArray(typeof(LPUretim));
        //}

        //private static LPUretimDegisken[] DegiskenOku(TABPEBLPURETIM tabAna, LPUretim form)
        //{
        //    IDataReader dr = tabAna.DetayOku(form);
        //    if (dr == null)
        //    {
        //        return null;
        //    }
        //    ArrayList al = new ArrayList();
        //    int say = 1;
        //    while (dr.Read())
        //    {
        //        LPUretimDegisken degisken = new LPUretimDegisken(say++);
        //        degisken.bilgiTuru = VeriErisim.DegerAl(dr, (int)TABPEBLPURETIM.ENUMAlanlarDegisken.BILGITURU, 0);
        //        degisken.anahtar1 = VeriErisim.DegerAl(dr, (int)TABPEBLPURETIM.ENUMAlanlarDegisken.ANAHTAR1, "");
        //        degisken.anahtar2 = VeriErisim.DegerAl(dr, (int)TABPEBLPURETIM.ENUMAlanlarDegisken.ANAHTAR2, "");
        //        degisken.anahtar3 = VeriErisim.DegerAl(dr, (int)TABPEBLPURETIM.ENUMAlanlarDegisken.ANAHTAR3, "");
        //        degisken.anahtar4 = VeriErisim.DegerAl(dr, (int)TABPEBLPURETIM.ENUMAlanlarDegisken.ANAHTAR4, "");
        //        degisken.girdiCikti = VeriErisim.DegerAl(dr, (int)TABPEBLPURETIM.ENUMAlanlarDegisken.GIRDICIKTI, 0);
        //        al.Add(degisken);
        //    }
        //    dr.Close();

        //    return (LPUretimDegisken[])al.ToArray(typeof(LPUretimDegisken));
        //}

        ///// <summary>
        ///// LPUretim nesnesini veri tabanına yazan fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">LPUretim yapısında LPUretim: bilgilerini tutan değişken</param>
        ///// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        //public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, LPUretim[] form)
        //{
        //    Sonuc s = new Sonuc(true, "", "");

        //    if (!YazmaYetkisiVarMi(kullanan))
        //    {
        //        s.DegerAta("Yazma yetkiniz yok!");
        //        return s;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD);
        //    TABPEBLPURETIM tabPEBLPURETIM = new TABPEBLPURETIM(ve);

        //    ve.BeginTransaction();

        //    for (int i = 0; i < form.Length && s.islemSonuc; i++)
        //    {
        //        s = tabPEBLPURETIM.DetaySil(form[i]);
        //        if (s.islemSonuc)
        //            s = tabPEBLPURETIM.Sil(form[i]);
        //    }

        //    if (s.islemSonuc)
        //    {
        //        ve.Commit();
        //    }
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();
        //    return s;
        //}

        ///// <summary>
        ///// LPUretim nesnesini veri tabanına yazan fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">LPUretim yapısında LPUretim: bilgilerini tutan değişken</param>
        ///// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        //public static Sonuc LPUretimeAl(VeriErisim veriErisim, Kullanici kullanan, LPUretim[] form)
        //{
        //    Sonuc s = new Sonuc(true, "", "");

        //    if (!YazmaYetkisiVarMi(kullanan))
        //    {
        //        s.DegerAta("Yazma yetkiniz yok!");
        //        return s;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD);
        //    TABPEBLPURETIM tabPEBLPURETIM = new TABPEBLPURETIM(ve);

        //    ve.BeginTransaction();

        //    for (int i = 0; i < form.Length && s.islemSonuc; i++)
        //    {
        //        s = tabPEBLPURETIM.DurumAta(form[i].yil, OrtakFonksiyonlar.ConvertToInt(form[i].siraNo, -1), 1);
        //    }

        //    if (s.islemSonuc)
        //    {
        //        ve.Commit();
        //    }
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();
        //    return s;
        //}

        //internal static void LPCalistir(VeriErisim veriErisim, Kullanici kullanan, int yil)
        //{
        //    if (!YazmaYetkisiVarMi(kullanan))
        //        throw new Exception("Yetkiniz yok!");
        //    LPUretim form = new LPUretim();
        //    form.yil = yil;
        //    form.durum = 1;
        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD);

        //    LPUretim[] formlar = Listele(ve, kullanan, form, false);
        //    if (formlar == null || formlar.Length == 0)
        //    {
        //        form.durum = 2;
        //        formlar = Listele(veriErisim, kullanan, form, false);
        //    }
        //    if (formlar != null && formlar.Length > 0)
        //    {
        //        form.siraNo = formlar[0].siraNo;
        //        formlar = Listele(veriErisim, kullanan, form, true);
        //    }
        //    if (formlar != null && formlar.Length == 1)
        //    {
        //        FormModelYadaSonucUret(TNS.UZY.Arac.Tanimla(), kullanan, ve, formlar[0], null);
        //    }
        //    ve.VeriErisimKapat();
        //}

        //private static void FormModelYadaSonucUret(TNS.UZY.IUZYServis uzyServis, Kullanici kullanan, VeriErisim ve, LPUretim lPUretim, DataBaseBilgi dbBilgi)
        //{
        //    if (lPUretim.durum == 1)
        //    {
        //        FormModelUret(uzyServis, kullanan, ve, lPUretim);
        //    }
        //    else
        //    {
        //        FormSonucUret(ve, lPUretim, dbBilgi);
        //    }
        //}

        public static Sonuc FormModelUret(TNS.UZY.IUZYServis uzyServis, Kullanici kullanan, VeriErisim ve, LPUretim lPUretim)
        {
            TNS.UZY.IUZYServis uzy = TNS.UZY.Arac.Tanimla();
            TNS.ORG.IORGServis orgServis = TNS.ORG.Arac.Tanimla();

            //p.kod = lPUretim.orgKod;
            //ObjectArray oa = orgServis.OrganizasyonBirimListele(p, false, false);
            //if (oa.ObjeSayisi == 1)
            //{
            //ORG.OrganizasyonBirim p2 = (ORG.OrganizasyonBirim)oa[0];
            ORG.OrganizasyonBirim p = new ORG.OrganizasyonBirim();
            p.kod = 0;
            p.kopruKod = lPUretim.kopruKod;
            p.turKod = lPUretim.orgTur;
            ObjectArray oa = orgServis.OrganizasyonBirimListele(p, false, false);
            if (!oa.sonuc.islemSonuc) return oa.sonuc;
            System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter(lPUretim.ModelDosyaAd(), System.Text.Encoding.Default);
            xmlWriter.Formatting = System.Xml.Formatting.Indented;
            xmlWriter.Indentation = 3;
            xmlWriter.IndentChar = ' ';

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("ZARFLAMA");
            xmlWriter.WriteAttributeString("YONTEM", lPUretim.YontemAd());
            int girdiSay = 0;
            int ciktiSay = 0;

            for (int i = 0; i < lPUretim.degiskenler.Length; i++)
            {
                if (lPUretim.degiskenler[i].girdiCikti == 1)
                {
                    girdiSay++;
                    xmlWriter.WriteStartElement("GIRDI");
                }
                else
                {
                    ciktiSay++;
                    xmlWriter.WriteStartElement("CIKTI");
                }
                xmlWriter.WriteAttributeString("KOD", "D" + i.ToString());
                xmlWriter.WriteAttributeString("AD", lPUretim.degiskenler[i].DegiskenAciklamaBul(uzy));
                xmlWriter.WriteAttributeString("GOSTERGEKOD", lPUretim.degiskenler[i].kod.ToString());

                xmlWriter.WriteEndElement();
            }

            int[] degiskenKodlar = new int[lPUretim.degiskenler.Length];
            for (int i = 0; i < lPUretim.degiskenler.Length; i++)
            {
                degiskenKodlar[i] = lPUretim.degiskenler[i].kod;
            }

            int eklenen = 0;
            foreach (ORG.OrganizasyonBirim org in oa.objeler)
            {
                decimal[] degiskenDegerler = STRSTRGosterge.GostergeGerceklesmeBul(ve, kullanan, lPUretim.yil, org.kod, degiskenKodlar, 12);
                int say = 0;
                for (int i = lPUretim.degiskenler.Length - 1; i >= 0; i--)
                {
                    if (degiskenDegerler[i] > 0) say++;
                }
                if (say > 0)
                {
                    xmlWriter.WriteStartElement("VERI");
                    for (int i = lPUretim.degiskenler.Length - 1; i >= 0; i--)
                    {
                        if (degiskenDegerler[i] <= 0) degiskenDegerler[i] = (decimal)0.0001;
                        //degiskenDegerler[i] = 0;
                        xmlWriter.WriteAttributeString("D" + i.ToString(), degiskenDegerler[i].ToString("0.0000"));
                    }
                    xmlWriter.WriteAttributeString("KURUM", org.adi);
                    xmlWriter.WriteAttributeString("KURUMKOD", org.kod.ToString());
                    xmlWriter.WriteEndElement();
                    eklenen++;
                }
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Close();

            if (girdiSay == 0 || ciktiSay == 0)
                return new Sonuc(false, "Girdi ve/veya Çıktı değişkeni sıfır olduğundan işlem yapılamadı!", "");
            //if (eklenen < ((girdiSay + ciktiSay) + 1)) //??? 2* olacak
            //    return new Sonuc(false, string.Format("Verisi olan birim sayısı ({0}), değişken sayısının({1}) iki katının bir fazlasından az olduğundan işlem yapılamadı!", eklenen, girdiSay + ciktiSay), "");

            return new Sonuc(true, "", "");

            //TABPEBLPURETIM tab = new TABPEBLPURETIM(ve);
            //ve.BeginTransaction();
            //Sonuc s = tab.DurumAta(lPUretim.yil, lPUretim.siraNo, 2);
            //if (s.islemSonuc)
            //    ve.Commit();
            //else
            //    ve.Rollback();
            //}
            //else
            //    return new Sonuc(false, "Verilen org kodun karşılığı bulunamadı!", "");
        }

        //private static string DegiskenDegerBul(TNS.UZY.IUZYServis uzyServis, Kullanici kullanan, STRGosterge degisken, VeriErisim ve, int yil, int orgKod)
        //{
        //    //string alanAd = null;
        //    //if (degisken.OranMi())
        //    //    alanAd = "ROUND(SUM(BILGI)/SUM(BOLEN),6)";
        //    //else
        //    //    alanAd = "ROUND(SUM(BILGI),6)";
        //    //string sql = uzyServis.UzayKosulStr(kullanan, "STRPERFORMANSHAMVERI", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), ((int)degisken.bilgiTuru).ToString(), "", orgKod.ToString(), "", degisken.anahtar1, degisken.anahtar2, degisken.anahtar3, degisken.anahtar4), false, alanAd);
        //    //return ve.TekKolonSQLDbl(sql).ToString("0.0000");
        //    return "1.0000";
        //}

        public static Sonuc FormSonucUret(VeriErisim ve, LPUretim lPUretim, DataBaseBilgi dbBilgi)
        {
            VeriZarflama tut = new VeriZarflama(lPUretim.ModelYol(), lPUretim.ModelDosyaAd(), lPUretim.SonucDosyaAd());
            try
            {
                tut.Coz(dbBilgi);
            }
            catch (Exception e)
            {
                dbBilgi.sonuc.DegerAta(e.Message);
            }
            return dbBilgi.sonuc;
            //TABPEBLPURETIM tab = new TABPEBLPURETIM(ve);
            //ve.BeginTransaction();
            //Sonuc s = tab.DurumAta(lPUretim.yil, OrtakFonksiyonlar.ConvertToInt(lPUretim.siraNo, -1), 3);
            //if (s.islemSonuc)
            //    ve.Commit();
            //else
            //    ve.Rollback();
            //return s;
        }

        //internal static Sonuc LPKontrolVerisiYukle(Kullanici kullanan, LPUretim form, string xmlVeri)
        //{
        //    Sonuc s = PEBLPUret.Kaydet(null, kullanan, form, true);
        //    if (s.islemSonuc)
        //    {
        //        form.siraNo = s.anahtar; //OrtakFonksiyonlar.ConvertToInt(s.anahtar, 0);
        //        System.IO.StreamWriter sw = new System.IO.StreamWriter(form.ModelDosyaAd(TNS.DEG.Arac.Tanimla()), false, System.Text.Encoding.Default);
        //        sw.Write(xmlVeri);
        //        sw.Close();
        //    }
        //    return s;
        //}
    }
}
