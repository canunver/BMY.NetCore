using System;
using System.Data;
using TNS.KYM;
using TNS.PEB.IsKurallari.Veri;
using TNS.Veri;
using TNS.UZY;
using System.IO;

namespace TNS.PEB.IsKurallari
{
    public class STRSTRGosterge
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, STRGosterge gosterge)
        {
            Sonuc sonuc = Yetki.TanimlamaYazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (string.IsNullOrEmpty(gosterge.ad))
                hata = PerformansButceResI.STR127 + "<br>";
            if (gosterge.birim <= 0)
                hata += PerformansButceResI.STR128 + "<br>";
            if (gosterge.basarimTuru <= 0)
                hata += PerformansButceResI.STR129 + "<br>";
            switch (gosterge.gostergeTuru)
            {
                case (int)ENUMGostergeTuru.Formul:
                    if (string.IsNullOrEmpty(gosterge.hesaplamaYontemi))
                        hata += PerformansButceResI.STR130 + "<br>";
                    else
                    {
                        string derlemeSonucStr = FormulDerlenebilirMi(gosterge.hesaplamaYontemi).hataStr;
                        if (!string.IsNullOrEmpty(derlemeSonucStr))
                            hata += derlemeSonucStr + "<br>";
                    }
                    break;
                case (int)ENUMGostergeTuru.VeriSistemi:
                    if (string.IsNullOrEmpty(gosterge.veriKaynagi) && string.IsNullOrEmpty(gosterge.veriKaynagiSistemi) && string.IsNullOrEmpty(gosterge.ekParam))
                        hata += PerformansButceResI.STR131 + "<br>";
                    break;
            }
            if (gosterge.gostergeBag == null || gosterge.gostergeBag.Length <= 0)
                hata += PerformansButceResI.STR136;
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTRGosterge tab = new TABSTRGosterge(ve);
            ve.BeginTransaction();

            bool var = tab.AyniAdVarmi(gosterge.kod, gosterge.ad);
            if (var)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR103;
                return sonuc;
            }

            if (gosterge.kod > 0)
            {
                if (gosterge.durum == 0)
                    gosterge.bitisTarihi = new TNSDateTime(DateTime.Now);
                sonuc = tab.Guncelle(gosterge);
            }
            else
            {
                gosterge.baslangicTarihi = new TNSDateTime(DateTime.Now);
                sonuc = tab.Kaydet(gosterge);
                gosterge.kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar.Trim(), 0);
            }

            if (sonuc.islemSonuc)
            {
                if (gosterge.gostergeBag != null && gosterge.gostergeBag.Length > 0)
                {
                    foreach (STRGostergeBag gb in gosterge.gostergeBag)
                        gb.gostergeKod = gosterge.kod;

                    sonuc = KaydetBag(ve, kullanan, gosterge.gostergeBag);
                }
            }

            if (sonuc.islemSonuc && gosterge.gostergeTuru == (int)ENUMGostergeTuru.Formul)
            {
                string derlemeSonucStr = Derle(gosterge, true);
                if (!string.IsNullOrEmpty(derlemeSonucStr))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = derlemeSonucStr;
                }
            }

            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = PerformansButceResI.STR011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }
            else
            {
                sonuc.anahtar = gosterge.kod.ToString();
                sonuc.bilgiStr = PerformansButceResI.STR010;
                ve.Commit();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc FormulDerlenebilirMi(string formul)
        {
            STRGosterge gosterge = new STRGosterge();
            gosterge.hesaplamaYontemi = formul;

            string hata = Derle(gosterge, false);
            if (string.IsNullOrEmpty(hata))
                return new Sonuc();
            else
                return new Sonuc(false, PerformansButceResI.STR147 + "<br>" + hata, string.Empty);
        }

        private static string Derle(STRGosterge gosterge, bool dllKaydet)
        {
            // Formüllü göstergeler (birim bazlı stratejik plan / performans göstergesi) kullanıcı isteğiyle kaldırıldı;
            // formül artık derlenmez. Eski kod açıklama olarak duruyor.
            return "Formüllü gösterge bu sürümde desteklenmiyor.";

//            //string yol = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8);
//            string kaynakKodu = "namespace TNS.PEB.IsKurallari\r\n";
//            kaynakKodu += "{\r\n";
//            kaynakKodu += "   public class STRGosterge\r\n"; //:TNS.PEB.IsKurallari.IGostergeHesapla
//            kaynakKodu += "   {\r\n";
//            kaynakKodu += "      public decimal Hesapla(int yil, int orgBirimKod, decimal[] g)\r\n";
//            kaynakKodu += "      {\r\n";
//            kaynakKodu += gosterge.hesaplamaYontemi;
//            kaynakKodu += "      }\r\n";
//            kaynakKodu += "   }\r\n";
//            kaynakKodu += "}\r\n";

//            // .NET 8: CSharpCodeProvider (CodeDom) derleyemez; Roslyn ile derlenir (TNS.KodDerleyici). dllKaydet ise
//            // derlenen kitaplık eskisi gibi DLLAdBul yoluna yazılır (FormulHesapla oradan okur).
//            string hataSonucu;
//            byte[] ikili = TNS.KodDerleyici.Derle(kaynakKodu, "G_" + gosterge.kod, null, out hataSonucu);
//            if (ikili == null)
//                return hataSonucu + kaynakKodu;

//            if (dllKaydet)
//                File.WriteAllBytes(DLLAdBul(gosterge), ikili);
//            return "";
        }

        private static string DLLAdBul(STRGosterge gosterge)
        {
            string yazYol = System.Configuration.ConfigurationManager.AppSettings.Get("HataDosyasi");
            yazYol = System.IO.Path.GetDirectoryName(yazYol);//hatadosyası yolu
            return yazYol + "\\..\\bin\\G_" + gosterge.kod + ".dll";
        }

        //private static string ServerBin()
        //{
        //    string yazYol = System.Configuration.ConfigurationManager.AppSettings.Get("HataDosyasi");
        //    yazYol = System.IO.Path.GetDirectoryName(yazYol);//hatadosyası yolu
        //    return yazYol + "\\..\\bin";
        //}


        private static decimal FormulHesapla(STRGosterge gosterge, int yil, int orgKod, decimal[] degerler)
        {
            // Formüllü göstergeler kullanıcı isteğiyle kaldırıldı (bkz. Derle). Eski kod açıklama olarak duruyor.
            throw new NotSupportedException("Formüllü gösterge bu sürümde desteklenmiyor.");


//            // .NET 8: AppDomain olusturulamaz; DLL toplanabilir bir AssemblyLoadContext icine yuklenir.
//            //ads.ApplicationName = "GostergeHesap";
//            //ads.ApplicationBase = ServerBin();
//            //ads.PrivateBinPath = ServerBin();
//            //ads.
            
//            //AppDomain.CurrentDomain.SetShadowCopyFiles();
//            System.Runtime.Loader.AssemblyLoadContext newDomain = new System.Runtime.Loader.AssemblyLoadContext("GostergeHesap", true);
//            //newDomain..RelativeSearchPath = "..\bin";

//            //string tut = newDomain.BaseDirectory;
//            //string tut2 = newDomain.DynamicDirectory;

//            string dllAd = DLLAdBul(gosterge);

//            byte[] rawAssembly = loadFile(dllAd);
//            System.Reflection.Assembly assembly = newDomain.LoadFromStream(new System.IO.MemoryStream(rawAssembly));

//            object oh = assembly.CreateInstance("TNS.PEB.IsKurallari.STRGosterge");

//            Type t = oh.GetType();
//            decimal d = (decimal)t.InvokeMember("Hesapla", System.Reflection.BindingFlags.InvokeMethod, null, oh, new object[] { yil, orgKod, degerler });

//            oh = null;
//            assembly = null;
//            newDomain.Unload();
//            newDomain = null;
//            return d;
        }

        private static byte[] loadFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            byte[] buffer = new byte[(int)fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            fs = null;
            return buffer;
        }
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, STRGosterge gosterge)
        {
            Sonuc sonuc = Yetki.TanimlamaYazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            if (gosterge.kod <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR132;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTRGosterge tab = new TABSTRGosterge(ve);
            ve.BeginTransaction();

            sonuc = tab.Sil(gosterge);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PerformansButceResI.STR014;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = PerformansButceResI.STR015 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static STRGosterge[] Listele(VeriErisim veriErisim, Kullanici kullanan, STRGosterge gosterge, bool detay, params string[] bagliKodlar)
        {
            Sonuc sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTRGosterge tab = new TABSTRGosterge(ve);

            IDataReader dr = null;
            try
            {
                dr = tab.Listele(gosterge, bagliKodlar);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            VeriErisim ve2 = null;
            if (detay)
                ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            TNSCollection gostergeler = new TNSCollection();
            while (dr.Read())
            {
                STRGosterge gelen = new STRGosterge();
                gelen.kod = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlar.KOD, 0);
                gelen.ad = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlar.AD, "");
                gelen.birim = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlar.BIRIM, 0);
                gelen.basarimTuru = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlar.BASARIMTURU, 0);
                gelen.gostergeTuru = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlar.GOSTERGETURU, 0);
                gelen.hesaplamaYontemi = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlar.HESAPLAMAYONTEMI, "");
                gelen.veriKaynagi = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlar.VERIKAYNAGI, "");
                gelen.veriKaynagiSistemi = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlar.VERIKAYNAGISISTEMI, "");
                gelen.ekParam = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlar.EKPARAM, "");
                gelen.veriToplamaPeriyodu = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlar.VERITOPLAMAPERIYODU, 0);
                gelen.raporlamaPeriyodu = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlar.RAPORLAMAPERIYODU, 0);
                gelen.aciklama = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlar.ACIKLAMA, "");
                gelen.durum = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlar.DURUM, 0);
                gelen.baslangicTarihi = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlar.BASLANGICTARIHI);
                gelen.bitisTarihi = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlar.BITISTARIHI);

                if (detay)
                    gelen.gostergeBag = ListeleBag(ve2, kullanan, gelen.kod);

                gostergeler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            if (detay)
                ve2.VeriErisimKapat();
            return (STRGosterge[])gostergeler.ToArray(typeof(STRGosterge));
        }

        private static STRGostergeBag[] ListeleBag(VeriErisim veriErisim, Kullanici kullanan, int gostergeKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTRGosterge tab = new TABSTRGosterge(ve);

            IDataReader dr = null;
            try
            {
                dr = tab.ListeleBag(gostergeKod);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection baglar = new TNSCollection();
            while (dr.Read())
            {
                STRGostergeBag gelen = new STRGostergeBag();
                gelen.gostergeKod = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlarBag.GOSTERGEKOD, 0);
                gelen.bagliKod = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlarBag.BAGLIKOD, "");
                gelen.bagliAd = PEBServis.UzayYaziDegerGetir("STRGENELHAVUZ", gelen.bagliKod, true, string.Empty);

                baglar.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (STRGostergeBag[])baglar.ToArray(typeof(STRGostergeBag));
        }

        private static Sonuc KaydetBag(VeriErisim veriErisim, Kullanici kullanan, STRGostergeBag[] bagKodlar)
        {
            Sonuc sonuc = new Sonuc();
            if (bagKodlar == null || bagKodlar.Length <= 0)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTRGosterge tab = new TABSTRGosterge(ve);
            ve.BeginTransaction();

            sonuc = tab.SilBag(bagKodlar[0].gostergeKod);
            if (sonuc.islemSonuc)
            {
                foreach (STRGostergeBag bagKod in bagKodlar)
                {
                    sonuc = tab.KaydetBag(bagKod);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = PerformansButceResI.STR010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PerformansButceResI.STR011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc GostergeGerceklesmeKaydet(VeriErisim veriErisim, Kullanici kullanan, int orgBirimKod, int gostergeKod, int yil, int ay, decimal deger)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSTRGosterge tab = new TABSTRGosterge(ve);
            ve.BeginTransaction();

            sonuc = tab.GerceklesmeSil(orgBirimKod, gostergeKod, yil, ay);
            if (sonuc.islemSonuc)
                sonuc = tab.GerceklesmeKaydet(orgBirimKod, gostergeKod, yil, ay, deger);
            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = PerformansButceResI.STR010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PerformansButceResI.STR011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc GostergeVeriKaynagindanCek(VeriErisim veriErisim, Kullanici kullanan, int orgBirimKod, int yil)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            STRGosterge gosterge = new STRGosterge();
            gosterge.gostergeTuru = 3;
            STRGosterge[] gostergeler = Listele(ve, kullanan, gosterge, false, null);
            IUZYServis uzy = TNS.UZY.Arac.Tanimla();
            if (gostergeler != null)
            {
                ve.BeginTransaction();
                for (int i = 0; i < gostergeler.Length; i++)
                {
                    decimal deger = VeriKaynagiDegerBul(kullanan, ve2, uzy, yil, orgBirimKod, gostergeler[i]);
                    sonuc = GostergeGerceklesmeKaydet(ve, kullanan, orgBirimKod, gostergeler[i].kod, yil, 12, deger);
                    if (!sonuc.islemSonuc) break;
                }
                if (sonuc.islemSonuc)
                {
                    sonuc.bilgiStr = PerformansButceResI.STR010;
                    ve.Commit();
                }
                else
                {
                    sonuc.hataStr = PerformansButceResI.STR011 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static decimal VeriKaynagiDegerBul(Kullanici kullanan, VeriErisim ve, IUZYServis uzy, int yil, int orgKod, STRGosterge gosterge)
        {

            return VeriKaynagiDegerBulCografi(kullanan, ve, uzy, yil, "1", orgKod, gosterge);
        }

        public static decimal VeriKaynagiDegerBulCografi(Kullanici kullanan, VeriErisim ve, IUZYServis uzy, int yil, string cografiTur, int orgOrCogKod, STRGosterge gosterge)
        {
            if (gosterge.veriKaynagi == "-")
            {
                if (string.IsNullOrEmpty(gosterge.ekParam))
                    return -1;
                string[] kosulOran = gosterge.ekParam.Split(';');
                string kosul = kosulOran[0];
                string oran = "";
                if (kosulOran.Length > 1)
                    oran = kosulOran[1];
                string alanAd = "";
                if (oran == "1")
                    alanAd = "ROUND(SUM(BILGI)/SUM(BOLEN),6)";
                else
                    alanAd = "ROUND(SUM(BILGI),6)";
                string[] kp = kosul.Split('-');
                string uzayKosul = TNS.UZY.Arac.UzayKosulOl(yil.ToString(), DiziAl(kp, 0), cografiTur, orgOrCogKod.ToString(), "", DiziAl(kp, 1), DiziAl(kp, 2), DiziAl(kp, 3), DiziAl(kp, 4));
                string sql = uzy.UzayKosulStr(kullanan, "STRPERFORMANSHAMVERI", uzayKosul, false, alanAd);
                if (ve != null)
                {
                    return ve.TekKolonSQLDecimal(sql);
                }
                else
                {
                    double d = uzy.UzayDegeriDbl(kullanan, "STRPERFORMANSHAMVERI", uzayKosul, false, alanAd);
                    return Convert.ToDecimal(d);
                }
            }
            else
                return -1;
        }

        private static string DiziAl(string[] dizi, int indis)
        {
            if (dizi == null || indis < 0 || indis >= dizi.Length) return "";
            return dizi[indis];
        }

        public static Sonuc GostergeHesapla(VeriErisim veriErisim, Kullanici kullanan, int orgBirimKod, int yil)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            STRGosterge gosterge = new STRGosterge();
            gosterge.gostergeTuru = 2;

            STRGosterge[] gostergeler = Listele(ve, kullanan, gosterge, false, null);
            if (gostergeler != null)
            {
                ve.BeginTransaction();
                for (int i = 0; i < gostergeler.Length; i++)
                {
                    decimal deger = FormulHesapla(kullanan, ve, yil, orgBirimKod, gostergeler[i], sonuc);
                    if (sonuc.islemSonuc)
                        sonuc = GostergeGerceklesmeKaydet(ve, kullanan, orgBirimKod, gostergeler[i].kod, yil, 12, deger);
                    if (!sonuc.islemSonuc) break;
                }
                if (sonuc.islemSonuc)
                {
                    sonuc.bilgiStr = PerformansButceResI.STR010;
                    ve.Commit();
                }
                else
                {
                    sonuc.hataStr = PerformansButceResI.STR011 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
            }

            return sonuc;
        }

        private static decimal FormulHesapla(Kullanici kullanan, VeriErisim ve, int yil, int orgBirimKod, STRGosterge gosterge, Sonuc sonuc)
        {
            if (string.IsNullOrEmpty(gosterge.ekParam))
            {
                sonuc.DegerAta("Kaynak gösterge yok!");
                return -1;
            }
            string[] dizi = gosterge.ekParam.Split(',');
            int[] gostergeKodlar = new int[dizi.Length];
            for (int i = 0; i < dizi.Length; i++)
            {
                gostergeKodlar[i] = OrtakFonksiyonlar.ConvertToInt(dizi[i], -1);
                if (gostergeKodlar[i] <= 0)
                {
                    sonuc.DegerAta("Kaynak gösterge kodu hatalı!");
                    return -1;
                }
            }
            decimal[] degerler = GostergeGerceklesmeBul(ve, kullanan, yil, orgBirimKod, gostergeKodlar, 12);
            return FormulHesapla(gosterge, yil, orgBirimKod, degerler);

            ////string yol = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8);
            //string kaynakKodu;
            //kaynakKodu = "namespace TNS.PEB.IsKurallari\r\n";
            //kaynakKodu += "{\r\n";
            //kaynakKodu += "   public class STRGosterge\r\n"; //:TNS.PEB.IsKurallari.IGostergeHesapla
            //kaynakKodu += "   {\r\n";
            //kaynakKodu += "      public decimal Hesapla(int yil, int orgBirimKod, decimal[] g)\r\n";
            //kaynakKodu += "      {\r\n";
            //kaynakKodu += gosterge.hesaplamaYontemi;
            //kaynakKodu += "      }\r\n";
            //kaynakKodu += "   }\r\n";
            //kaynakKodu += "}\r\n";

            //Microsoft.CSharp.CSharpCodeProvider cProv = new Microsoft.CSharp.CSharpCodeProvider();

            //System.CodeDom.Compiler.CompilerParameters derlemeParametreleri = new System.CodeDom.Compiler.CompilerParameters();
            //derlemeParametreleri.ReferencedAssemblies.Add("mscorlib.dll");
            //derlemeParametreleri.ReferencedAssemblies.Add("System.dll");
            ////derlemeParametreleri.ReferencedAssemblies.Add(yol);


            ////cParams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            //derlemeParametreleri.GenerateExecutable = false;
            //derlemeParametreleri.GenerateInMemory = true;

            //System.CodeDom.Compiler.CompilerResults derlemeSonucu = cProv.CompileAssemblyFromSource(derlemeParametreleri, kaynakKodu);

            //// Hatalari varsa, sonuc değişkeninde döndür. 
            //if (derlemeSonucu.Errors.Count != 0)
            //{
            //    string hataSonucu = "";
            //    foreach (var er in derlemeSonucu.Errors)
            //    {
            //        hataSonucu += er.ToString();
            //    }
            //    sonuc.DegerAta(hataSonucu + kaynakKodu);
            //}
            //else
            //{
            //    try
            //    {
            // Derlenmiş yapıyı çalıştır. 
            //object obj = derlemeSonucu.CompiledAssembly.CreateInstance("TNS.PEB.IsKurallari.STRGosterge");
            //Type t = obj.GetType();
            //object x = (decimal)t.InvokeMember("Hesapla", System.Reflection.BindingFlags.InvokeMethod, null, obj, new object[] { yil, orgBirimKod, degerler });
            //if (decimal.IsNaN(x) || decimal.IsInfinity(x)) return 0;
            //OrtakFonksiyonlar.ConvertToDecimal(x);//yukarıdaki ifi karşılar mı bu dönüşüm emin olamadım - Erdal
            //TNS.PEB.IsKurallari.IGostergeHesapla obj = (TNS.PEB.IsKurallari.IGostergeHesapla)derlemeSonucu.CompiledAssembly.CreateInstance("TNS.PEB.IsKurallari.STRGosterge");
            //decimal x = obj.Hesapla(yil, orgBirimKod, degerler);
            //return x;
            //    }
            //    catch (Exception e)
            //    {
            //        sonuc.DegerAta(e.Message + kaynakKodu);
            //    }
            //}

            //return 0;
        }
        static Random r = null;
        public static decimal[] GostergeGerceklesmeBul(VeriErisim veriErisim, Kullanici kullanan, int yil, int orgBirimKod, int[] gostergeKodlar, int ay)
        {
            if (r == null) r = new Random();
            decimal[] donusDeger = new decimal[gostergeKodlar.Length];
            for (int i = 0; i < gostergeKodlar.Length; i++)
            {
                donusDeger[i] = r.Next(40) + 40;
            }
            return donusDeger;
            //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            //TABSTRGosterge tab = new TABSTRGosterge(ve);
            //IDataReader dr = tab.GerceklesmeListele(orgBirimKod, yil, ay, gostergeKodlar);
            //if (dr != null)
            //{
            //    while (dr.Read())
            //    {
            //        int kod = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlarGERCEKLESME.GOSTERGEKOD, 0);
            //        int yer = KodYerBul(gostergeKodlar, kod);
            //        if (yer >= 0)
            //            donusDeger[yer] = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlarGERCEKLESME.DEGER, (decimal)0);
            //    }
            //    dr.Close();
            //}
            //ve.VeriErisimKapat();
            //return donusDeger;
        }

        private static int KodYerBul(int[] gostergeKodlar, int kod)
        {
            for (int i = 0; i < gostergeKodlar.Length; i++)
            {
                if (gostergeKodlar[i] == kod)
                    return i;
            }
            return -1;
        }
    }

    //public interface IGostergeHesapla
    //{
    //    decimal Hesapla(int yil, int orgBirimKod, decimal[] g);
    //}
}