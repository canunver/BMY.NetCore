using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.MUH;
using TNS.IKY;

namespace TNS.HRC.IsKurallari
{
    public static class BordroYapiIslem
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriHRC";

        internal static BordroYapi[] BordroYapiListele(VeriErisim veriErisim, Kullanici kullanan, int bordroYapiTur, int bordroIslemTur, bool sadeceKendisi, string bordroYapiTurler, string bordroIslemTurler, bool kendisindeVeyaAdinda)
        {
            BordroGenelDegisken.DegiskenleriSifirla();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            //VeriErisim ve2 = VeriErisim.VeriErisimBul(null, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTANIM tab = new TABHRCBORDROTANIM(ve);
            //TABHRCBORDROTANIM tab2 = new TABHRCBORDROTANIM(ve2);

            IDataReader dr = tab.Listele(bordroYapiTur, bordroIslemTur, null, bordroYapiTurler, bordroIslemTurler, kendisindeVeyaAdinda);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            ArrayList alByler = new ArrayList();
            BordroYapi by = null;
            ArrayList alTum = new ArrayList();
            ArrayList alDeg = new ArrayList();
            ArrayList alFor = new ArrayList();
            DegiskenFormul detay = null;
            int oncBordroYapiTur = -1;
            int oncBordroIslemTur = -1;
            while (dr.Read())
            {
                int okuBordroYapiTur = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.BORDROTANIM, 2); ;
                int okuBordroIslemTur = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.BORDROISLEM, 2); ;
                if (by == null || oncBordroYapiTur != okuBordroYapiTur || oncBordroIslemTur != okuBordroIslemTur)
                {
                    if (by != null)
                        by.FormulAta(alTum, alDeg, alFor);
                    alTum.Clear();
                    alDeg.Clear();
                    alFor.Clear();

                    oncBordroYapiTur = okuBordroYapiTur;
                    oncBordroIslemTur = okuBordroIslemTur;
                    by = new BordroYapi();
                    by.bordroTur = okuBordroYapiTur;
                    by.bordroIslemTur = okuBordroIslemTur;

                    by.bordroAd = VeriErisim.DegerAl(dr, 15 + (int)TABHRCBORDROTANIM.ENUMAlanlarANA.BORDROAD, "");
                    by.kaynakBordroTur = VeriErisim.DegerAl(dr, 15 + (int)TABHRCBORDROTANIM.ENUMAlanlarANA.KAYNAKBORDROTUR, 0);
                    by.kaynakBordroIslemTur = VeriErisim.DegerAl(dr, 15 + (int)TABHRCBORDROTANIM.ENUMAlanlarANA.KAYNAKBORDROISLEMTUR, 0);

                    //by = BordroTanimAnaOku(tab2, null, by);
                    alByler.Add(by);
                }

                detay = new DegiskenFormul();
                detay.formula = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.FORMUL, "");
                detay.giris = (ENUMBordroDegiskenTur)VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.DEGISKENTUR, 0);
                if (!string.IsNullOrWhiteSpace(detay.formula)) detay.giris = ENUMBordroDegiskenTur.Formul;
                detay.tip = (VariableType)VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.DEGISKENTIP, 0);
                detay.ad = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.DEGISKENFORMULAD, "");
                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.SIRANO, "");
                detay.kisaEtiket = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.KISAETIKET, "");
                detay.uzunEtiket = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.UZUNETIKET, "");
                detay.borcHesap = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.BORCHESAP, "");
                detay.alacakHesap = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.ALACAKHESAP, "");
                detay.genislik = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.GENISLIK, 40);

                detay.noktaSay = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.NOKTASAY, 2);
                detay.ilkDeger = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.ILKDEGER, "");

                alTum.Add(detay);
                if (string.IsNullOrEmpty(detay.formula))
                    alDeg.Add(detay);
                else
                    alFor.Add(detay);
            }
            dr.Close();
            if (by != null)
                by.FormulAta(alTum, alDeg, alFor);
            else if (bordroYapiTur > 0 && bordroIslemTur > 0)
            {
                by = new BordroYapi();
                by.bordroTur = bordroYapiTur;
                by.bordroIslemTur = bordroIslemTur;
                by = BordroTanimAnaOku(tab, null, by);
                alByler.Add(by);
            }
            if (!sadeceKendisi)
            {
                for (int i = 0; i < alByler.Count; i++)
                {
                    by = (BordroYapi)alByler[i];
                    if (by.kaynakBordroTur > 0 && by.kaynakBordroIslemTur > 0)
                    {
                        BordroYapi byAlt = BordroYapiOku(ve, kullanan, by.kaynakBordroTur, by.kaynakBordroIslemTur, sadeceKendisi, null, null, false);
                        BordroBirlestir(by, byAlt);
                    }
                }
            }
            ve.VeriErisimKapat();
            return (BordroYapi[])alByler.ToArray(typeof(BordroYapi));
        }

        private static void BordroBirlestir(BordroYapi by, BordroYapi byAlt)
        {
            ArrayList alTum = new ArrayList();

            foreach (DegiskenFormul bulunan in byAlt.hepsi)
            {
                alTum.Add(bulunan);
            }

            foreach (DegiskenFormul item in by.hepsi)
            {
                int bulunanInd = alTum.IndexOf(item);
                if (bulunanInd < 0)
                    alTum.Add(item);
                else
                    alTum[bulunanInd] = item;
            }

            ArrayList alDeg = new ArrayList();
            ArrayList alFor = new ArrayList();
            alTum.Sort();
            foreach (DegiskenFormul bulunan in alTum)
            {
                if (string.IsNullOrEmpty(bulunan.formula))
                    alDeg.Add(bulunan);
                else
                    alFor.Add(bulunan);
            }
            by.FormulAta(alTum, alDeg, alFor);
        }

        internal static BordroYapi BordroYapiOku(VeriErisim veriErisim, Kullanici kullanan, int bordroYapiTur, int bordroIslemTur, bool sadeceKendisi, string bordroYapiTurler, string bordroIslemTurler, bool kendisindeVeyaAdinda)
        {
            BordroYapi[] byler = BordroYapiListele(veriErisim, kullanan, bordroYapiTur, bordroIslemTur, sadeceKendisi, bordroYapiTurler, bordroIslemTurler, kendisindeVeyaAdinda);
            if (byler == null || byler.Length == 0) return null;
            return byler[0];
        }

        /// <summary>
        /// BordroTanim nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroTanim yapısında BordroTanim:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        internal static Sonuc BordroYapiSakla(VeriErisim veriErisim, Kullanici kullanan, BordroYapi bordroYapi)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan))
                s.DegerAta("Bu işlemi yapmak için sistem yöneticisi olmalısınız!");

            if (bordroYapi.degiskenler == null || bordroYapi.degiskenler.Length == 0 && bordroYapi.kaynakBordroTur == 0 && bordroYapi.kaynakBordroIslemTur == 0)
                s.DegerAta("Bu işlemi yapmak için değişken tanımlamalısınız!");

            //if (bordroYapi.formuller == null || bordroYapi.formuller.Length == 0)
            //    s.DegerAta("Bu işlemi yapmak için formül tanımlamalısınız!");

            for (int i = 0; i < bordroYapi.degiskenler.Length; i++)
            {
                if (string.IsNullOrEmpty(bordroYapi.degiskenler[i].ad))
                    s.DegerAta(string.Format("Adı olmayan değişken olamaz!", bordroYapi.degiskenler[i].ad));
                if (bordroYapi.degiskenler[i].tip == VariableType.Bilinmiyor)
                    s.DegerAta(string.Format("Tipi olmayan değişken olamaz!", bordroYapi.degiskenler[i].ad));
            }

            if (bordroYapi.formuller != null)
            {
                for (int i = 0; i < bordroYapi.formuller.Length; i++)
                {
                    if (string.IsNullOrEmpty(bordroYapi.formuller[i].formula))
                        s.DegerAta(string.Format("{0} formülü için formül girmelisiniz!", bordroYapi.formuller[i].ad));
                    if (string.IsNullOrEmpty(bordroYapi.formuller[i].ad))
                        s.DegerAta(string.Format("Adı olmayan formül olamaz!", bordroYapi.formuller[i].ad));
                    if (bordroYapi.formuller[i].tip == VariableType.Bilinmiyor)
                        s.DegerAta(string.Format("Tipi olmayan formül olamaz!", bordroYapi.formuller[i].ad));
                }
            }

            if (string.IsNullOrWhiteSpace(bordroYapi.bordroAd))
                s.DegerAta("Adı olmayan bordro yapısı olamaz!");
            if (!s.islemSonuc)
                return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTANIM tab = new TABHRCBORDROTANIM(ve);

            BordroYapi byOkunan = BordroTanimAnaOku(tab, s, bordroYapi);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();

            if (byOkunan != null)
                s = tab.GuncelleAna(bordroYapi);
            else
                s = tab.EkleAna(bordroYapi);

            if (s.islemSonuc)
                s = tab.Sil(bordroYapi.bordroTur, bordroYapi.bordroIslemTur);
            bordroYapi.HepsiOl();

            for (int i = 0; i < bordroYapi.hepsi.Length && s.islemSonuc; i++)
            {
                s = tab.Ekle(bordroYapi.bordroTur, bordroYapi.bordroIslemTur, bordroYapi.hepsi[i]);
            }

            if (s.islemSonuc)
            {
                ve.Commit();
                TabloOl(ve, kullanan, bordroYapi);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        private static BordroYapi BordroTanimAnaOku(TABHRCBORDROTANIM tab, Sonuc s, BordroYapi bordroYapi)
        {
            BordroYapi by = null;
            if (s == null)
                by = bordroYapi;
            IDataReader dr = tab.OkuAna(bordroYapi);
            if (dr == null)
            {
                if (s != null)
                    s.DegerAta(tab.Hata());
            }
            else
            {
                if (dr.Read())
                {
                    if (by == null)
                        by = new BordroYapi();
                    by.bordroTur = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarANA.BORDROTUR, 0);
                    by.bordroIslemTur = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarANA.BORDROISLEMTUR, 0);
                    by.bordroAd = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarANA.BORDROAD, "");
                    by.kaynakBordroTur = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarANA.KAYNAKBORDROTUR, 0);
                    by.kaynakBordroIslemTur = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarANA.KAYNAKBORDROISLEMTUR, 0);
                }
                dr.Close();
            }
            return by;
        }

        private static string SQLOl(DegiskenFormul degiskenFormul)
        {
            string tut = degiskenFormul.ad + " " + VariableFunctions.GetSQLTypeName(degiskenFormul.tip, degiskenFormul.noktaSay);
            return tut;
        }

        private static string SQLOl(string tabloAd, DegiskenFormul degiskenFormul)
        {
            return "ALTER TABLE " + tabloAd + " ADD " + SQLOl(degiskenFormul);
        }

        private static void TabloOl(VeriErisim ve, Kullanici kullanan, BordroYapi by)
        {
            string sql = "";
            if (by.kaynakBordroTur > 0 && by.kaynakBordroIslemTur > 0)
            {
                BordroYapi byAlt = BordroYapiOku(ve, kullanan, by.kaynakBordroTur, by.kaynakBordroIslemTur, false, null, null, false);
                BordroBirlestir(by, byAlt);
            }
            if (!TabloVarmi(ve, by.VTTabloAd))
            {
                sql = "CREATE TABLE " + by.VTTabloAd + "(\r\n";
                sql += "YIL INT, SURECNO VARCHAR(6), SATIRNO INT, TCKIMLIKNO VARCHAR(20), ";
                for (int i = 0; i < by.hepsi.Length; i++)
                {
                    sql += SQLOl(by.hepsi[i]) + ",\r\n";
                }
                sql += " PRIMARY KEY (YIL, SURECNO, SATIRNO, TCKIMLIKNO))";
            }
            else
            {
                ArrayList al = AlanListesi(ve, by.VTTabloAd);

                for (int i = 0; i < by.hepsi.Length; i++)
                {
                    if (al.IndexOf(by.hepsi[i].ad.Trim()) < 0)
                    {
                        if (sql != "") sql += ", ";
                        sql += SQLOl(by.hepsi[i]);
                    }
                }

                sql = "ALTER TABLE " + by.VTTabloAd + " ADD " + sql;
            }
            ve.ExecuteNonquery(sql);
        }

        private static ArrayList AlanListesi(VeriErisim ve, string tabloAd)
        {
            ArrayList al = new ArrayList();
            IDataReader dr = ve.DataReaderDondur("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=" + VeriErisim.DegerYap(tabloAd));
            if (dr != null)
            {
                OrtakFonksiyonlar.HataDosyaYaz("BRDTabloOl.txt", "Alan listesi", true);
                while (dr.Read())
                {
                    al.Add(VeriErisim.DegerAl(dr, 0, "").Trim());
                }
                dr.Close();
            }
            return al;
        }

        private static bool TabloVarmi(VeriErisim ve, string tabloAd)
        {
            string sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME=" + VeriErisim.DegerYap(tabloAd);
            int say = ve.TekKolonSQLInt(sql);
            return say == 1;
        }

        private static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            return false;
        }

        internal static CalisanDinamikBordro CalisanDinamikBordroOku(VeriErisim veriErisim, Kullanici kullanan, BordroYapi bordroYapi, BordroKistas bordroKistas, bool ekleEkBilgi)
        {
            CalisanDinamikBordro donusDeger = new CalisanDinamikBordro();
            donusDeger.kisiDegerler = new ArrayList();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTANIM tab = new TABHRCBORDROTANIM(ve);

            IDataReader dr = tab.BordroKisiListele(bordroYapi, bordroYapi.SiralamaBulVT(), bordroKistas, TNS.DEG.Arac.VarsayilanKurumBul());
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            int toplamKayitSay = 0;
            if (bordroYapi.basla > 0)
            {
                while (true)
                {
                    if (toplamKayitSay == bordroYapi.basla) break;
                    if (dr.Read()) toplamKayitSay++;
                    else break;
                }
            }
            if (toplamKayitSay == bordroYapi.basla || bordroYapi.limit <= 0)
            {
                TNSDateTime dtOku;
                int toplamOkunan = 0;
                while (dr.Read())
                {
                    toplamKayitSay++;
                    toplamOkunan++;
                    if (toplamOkunan > bordroYapi.limit && bordroYapi.limit > 0) continue;

                    System.Collections.Generic.List<object> al = new System.Collections.Generic.List<object>();
                    al.Add(VeriErisim.DegerAl(dr, 0, "")); //Tc Kimlik No
                    int alanSay = bordroYapi.hepsi.Length;
                    if (bordroYapi.kolonSay > 0) alanSay = bordroYapi.kolonSay;
                    for (int i = 1; i <= alanSay; i++)
                    {
                        object deger;
                        DegiskenFormul tut = bordroYapi.hepsi[i - 1];

                        if (tut.tip == VariableType.Integer) deger = VeriErisim.DegerAl(dr, i, (int)0);
                        else if (tut.tip == VariableType.Money) deger = VeriErisim.DegerAl(dr, i, (double)0);
                        else if (tut.tip == VariableType.Date)
                        {
                            dtOku = VeriErisim.DegerAl(dr, i);
                            if (dtOku != null && !dtOku.isNull)
                                deger = dtOku.Oku();
                            else
                                deger = DateTime.MinValue;
                        }
                        else deger = VeriErisim.DegerAl(dr, i, "");

                        al.Add(deger);
                    }
                    int satirNo = VeriErisim.DegerAl(dr, bordroYapi.hepsi.Length + 1, (int)0);
                    al.Add(satirNo);
                    KisiSatir kisiSatir = new KisiSatir(al.ToArray());
                    if (ekleEkBilgi)
                    {
                        kisiSatir.kisiSatirBilgi = new KisiSatirBilgi();
                        kisiSatir.kisiSatirBilgi.odemeTarih = VeriErisim.DegerAl(dr, bordroYapi.hepsi.Length + 2);
                        kisiSatir.kisiSatirBilgi.oncBordroTarih = VeriErisim.DegerAl(dr, bordroYapi.hepsi.Length + 3);
                        kisiSatir.kisiSatirBilgi.surecYil = VeriErisim.DegerAl(dr, bordroYapi.hepsi.Length + 4, 0);
                        kisiSatir.kisiSatirBilgi.surecNo = VeriErisim.DegerAl(dr, bordroYapi.hepsi.Length + 5, "");

                        kisiSatir.kisiSatirBilgi.muhasebeKod = VeriErisim.DegerAl(dr, bordroYapi.hepsi.Length + 6, 0);
                        kisiSatir.kisiSatirBilgi.birimKod = VeriErisim.DegerAl(dr, bordroYapi.hepsi.Length + 7, "");
                        kisiSatir.kisiSatirBilgi.muhasebeAd = VeriErisim.DegerAl(dr, bordroYapi.hepsi.Length + 8, "");
                        kisiSatir.kisiSatirBilgi.birimAd = VeriErisim.DegerAl(dr, bordroYapi.hepsi.Length + 9, "");

                        kisiSatir.kisiSatirBilgi.hazirlayan = VeriErisim.DegerAl(dr, bordroYapi.hepsi.Length + 10, "");
                        kisiSatir.kisiSatirBilgi.onayaSunan = VeriErisim.DegerAl(dr, bordroYapi.hepsi.Length + 11, "");
                        kisiSatir.kisiSatirBilgi.onaylayan = VeriErisim.DegerAl(dr, bordroYapi.hepsi.Length + 12, "");
                    }
                    donusDeger.kisiDegerler.Add(kisiSatir);
                }
            }
            dr.Close();
            ve.VeriErisimKapat();
            donusDeger.toplamKayitSay = toplamKayitSay;

            if (bordroYapi.kolonSay == 0 && donusDeger.kisiDegerler != null && donusDeger.kisiDegerler.Count > 0)
            {
                CalisanDinamikBordroSirala sirala = new CalisanDinamikBordroSirala(bordroYapi);
                donusDeger.kisiDegerler.Sort(sirala);
            }
            return donusDeger;
        }

        internal static ObjectArray CalisanDinamikBordroListele(VeriErisim veriErisim, Kullanici kullanan, BordroKistas bordroKistas)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            BordroYapi[] byler = null;
            try
            {
                byler = BordroYapiListele(ve, kullanan, bordroKistas.bordroTur, bordroKistas.bordroIslem, false, bordroKistas.bordroYapiTurler, bordroKistas.bordroIslemTurler, false);
            }
            catch (Exception ex)
            {
                oa.sonuc.DegerAta(ex.Message);
            }
            if (byler == null) oa.sonuc.DegerAta("Bordro yapıları okunamadı!");
            if (oa.sonuc.islemSonuc)
            {
                foreach (BordroYapi item in byler)
                {
                    if (item.bordroIslemTur == bordroKistas.bordroIslem || bordroKistas.bordroIslem <= 0)
                    {
                        CalisanDinamikBordroDonem donem = new CalisanDinamikBordroDonem(0, "");
                        donem.bordroYapi = item;
                        try
                        {
                            ve.hata = "";
                            if (!string.IsNullOrWhiteSpace(bordroKistas.siralama))
                                item.siralamaStr = bordroKistas.siralama;
                            donem.calisanDinamikBordro = CalisanDinamikBordroOku(ve, kullanan, item, bordroKistas, true);
                            item.siralamaStr = null;
                            if (donem.calisanDinamikBordro.kisiDegerler.Count > 0)
                            {
                                oa.Ekle(donem);
                            }
                        }
                        catch
                        {
                            //(Exception ex)
                            //OrtakFonksiyonlar.HataDosyaYaz("BordroListele.txt", ex.Message + ex.StackTrace, false);
                        }
                    }
                }
            }
            ve.VeriErisimKapat();
            return oa;
        }

        public static Sonuc CalisanDinamikBordroOnayla(VeriErisim veriErisim, Kullanici kullanan, int yil, string surecNo, BordroYapi bordroYapi, int onayKod)
        {
            Sonuc s = new Sonuc();
            CalisanDinamikBordroDonem cdbdForm = new CalisanDinamikBordroDonem(yil, surecNo);
            ObjectVAN ov = DonemOku(null, kullanan, cdbdForm, null);
            if (!ov.sonuc.islemSonuc)
            {
                return ov.sonuc;
            }
            else if (ov.o == null)
            {
                s.DegerAta("Bordro ile ilgili dönem bulunamadı!");
                return s;
            }
            else
            {
                cdbdForm = (CalisanDinamikBordroDonem)ov.o;
                Surec surec = HRC.IsKurallari.HRCSurec.SurecOku(null, kullanan, yil, surecNo);

                s = BordroKontrolEt(null, kullanan, cdbdForm.bordroDonemBasTarihDT.Year, cdbdForm.bordroDonemBasTarihDT.Month, surec.muhasebe, surec.birim, bordroYapi.bordroTur, bordroYapi.bordroIslemTur);
                if (s.islemSonuc)
                {
                    TNS.BHMv2.IBHMv2Servis srvs = TNS.BHMv2.Arac.Tanimla();
                    s = srvs.OnayKodAtaToplu(kullanan, cdbdForm.bordroDonemBasTarihDT.Year, surec.muhasebe, surec.birim, TNS.HRC.Arac.BordroAsamaOl(cdbdForm.bordroDonemBasTarihDT.Month, bordroYapi.bordroTur, bordroYapi.bordroIslemTur), TNS.BHMv2.ENUMBirimOnayTur.BORDRO, onayKod);
                }
            }
            return s;
        }

        public static Sonuc CalisanDinamikBordroOnayliMi(VeriErisim veriErisim, Kullanici kullanan, int yil, string surecNo, BordroYapi bordroYapi, out CalisanDinamikBordroDonem cdbdForm)
        {
            Sonuc s = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            cdbdForm = new CalisanDinamikBordroDonem(yil, surecNo);
            Surec surec = HRC.IsKurallari.HRCSurec.SurecOku(ve, kullanan, yil, surecNo);
            int onayKod = 0;
            s.anahtar = "";
            if (surec == null)
                s.DegerAta($"{yil} - {surecNo} okunamadı!");
            else if (surec.surecDurum >= 5)
                s.DegerAta("Süreç onaylandığından yada iptal edildiğinden işleme devam edemezsiniz!");
            else
            {
                ObjectVAN ov = DonemOku(ve, kullanan, cdbdForm, null);
                if (!ov.sonuc.islemSonuc)
                {
                    s = ov.sonuc;
                }
                else if (ov.o == null)
                {
                    s.DegerAta("Bordro ile ilgili dönem bulunamadı!");
                }
                else
                {
                    cdbdForm = (CalisanDinamikBordroDonem)ov.o;
                    TNS.BHMv2.IBHMv2Servis srvs = TNS.BHMv2.Arac.Tanimla();
                    onayKod = srvs.OnayKodOkuTasra(kullanan, cdbdForm.bordroDonemBasTarihDT.Year, surec.muhasebe, surec.birim, TNS.HRC.Arac.BordroAsamaOl(cdbdForm.bordroDonemBasTarihDT.Month, bordroYapi.bordroTur, bordroYapi.bordroIslemTur), TNS.BHMv2.ENUMBirimOnayTur.BORDRO);
                    if (onayKod == 0)
                    {
                        if (!BordoAcisZorunluMu((BordroIslemTur.ENUMBordroIslemTurYapi)bordroYapi.bordroIslemTur)) onayKod = 1;
                    }
                }
            }

            if (s.islemSonuc)
            {
                string donemAd = string.Format("{2}-{3} kodlu biriminin, {0}/{1} dönemine ait, {4}.{5} kodlu bordrosu ", cdbdForm.bordroDonemBasTarihDT.Year, cdbdForm.bordroDonemBasTarihDT.Month, surec.muhasebe, surec.birim, bordroYapi.bordroTur, bordroYapi.bordroIslemTur);
                s.anahtar = onayKod.ToString();
                if (onayKod == 0)
                {
                    s.DegerAta(donemAd + "henüz işleme açılmamış!");
                }
                else if (onayKod == 5 || (onayKod == 4 && !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)))
                {
                    s.DegerAta(donemAd + "veri girişine kapatılmış!");
                }
            }
            ve.VeriErisimKapat();
            return s;
        }

        private static string _BordoAcisZorunlu = null;
        private static bool BordoAcisZorunluMu(BordroIslemTur.ENUMBordroIslemTurYapi tur)
        {
            bool zorunluTur = tur == BordroIslemTur.ENUMBordroIslemTurYapi.Fark || tur == BordroIslemTur.ENUMBordroIslemTurYapi.Normal;
            if (TNS.HRC.Arac.CumhurbaskanligiMi())
                zorunluTur = zorunluTur || (tur == BordroIslemTur.ENUMBordroIslemTurYapi.Ikramiye);
            if (zorunluTur)
            {
                if (_BordoAcisZorunlu == null)
                {
                    _BordoAcisZorunlu = DEG.Arac.DegiskenDegerBul(0, "BORDROACISZORUNLU") + "";
                    if (string.IsNullOrWhiteSpace(_BordoAcisZorunlu)) _BordoAcisZorunlu = "1";
                }
                return _BordoAcisZorunlu == "1";
            }
            return false;
        }

        internal static Sonuc CalisanDinamikBordroKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string surecNo, BordroYapi bordroYapi, ArrayList satirlar)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            CalisanDinamikBordroDonem cdbdForm;

            Sonuc s = CalisanDinamikBordroOnayliMi(ve, kullanan, yil, surecNo, bordroYapi, out cdbdForm);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }
            TNSDateTime hazirlamaTarih = new TNSDateTime(cdbdForm.hazirlamaTarihDT);
            ve.BeginTransaction();
            TABHRCBORDROTANIM tab = new TABHRCBORDROTANIM(ve);
            TNSCollection matrahListesi = new TNSCollection();
            if (!bordroYapi.tekTekSil)
                s = tab.BordroSil(yil, surecNo, bordroYapi.VTTabloAd);
            for (int i = 0; i < satirlar.Count && s.islemSonuc; i++)
            {
                double[] matrahlar = new double[TNS.IKY.PersonelMaasMatrah.matrahTurAdlar.Length];
                KisiSatir degerler = (KisiSatir)satirlar[i];
                int satirNo = i + 1;
                if (bordroYapi.tekTekSil)
                {
                    string tc = (string)degerler.degerler[0];
                    satirNo = (int)degerler.degerler[bordroYapi.hepsi.Length + 1];
                    if (satirNo > 0)
                        s = tab.BordroSil(yil, surecNo, bordroYapi.VTTabloAd, tc, satirNo);
                    else
                        satirNo = tab.BordroSiraBul(yil, surecNo, bordroYapi.VTTabloAd);
                }
                if (s.islemSonuc)
                    s = tab.BordroKaydet(yil, surecNo, satirNo, bordroYapi, degerler.degerler, matrahlar);
                if (s.islemSonuc)
                {
                    for (int m = 0; m < matrahlar.Length; m++)
                    {
                        if (matrahlar[m] > 0)
                        {
                            TNS.IKY.PersonelMaasMatrah pmm = new IKY.PersonelMaasMatrah();
                            pmm.tcKimlikNo = (string)degerler.degerler[0];
                            pmm.yil = yil;
                            pmm.surecNo = surecNo;
                            pmm.tabloAd = bordroYapi.VTTabloAd;
                            pmm.satirNo = i;
                            pmm.matrahTur = m + 1;
                            pmm.tarih = hazirlamaTarih;
                            pmm.deger = matrahlar[m];
                            matrahListesi.Add(pmm);
                            //OrtakFonksiyonlar.HataDosyaYaz("Matrah.txt", "Matrah listesine eklendi" + pmm.surecNo + " " + pmm.tcKimlikNo + " " + m.ToString(), false);
                        }
                    }
                }
            }

            if (s.islemSonuc)
            {
                ve.Commit();
                Sonuc matrahSonuc = new Sonuc();
                if (!bordroYapi.tekTekSil)
                {
                    //OrtakFonksiyonlar.HataDosyaYaz("Matrah.txt", "Matrah silinecek" + yil + " " + surecNo + " " + bordroYapi.VTTabloAd, false);
                    matrahSonuc = TNS.IKY.Arac.Tanimla().MaasMatrahSil(kullanan, yil, surecNo, bordroYapi.VTTabloAd, null);
                }
                if (matrahSonuc.islemSonuc)
                {
                    //OrtakFonksiyonlar.HataDosyaYaz("Matrah.txt", "Matrah kaydedilecek" + yil + " " + surecNo + " " + bordroYapi.VTTabloAd, false);
                    matrahSonuc = TNS.IKY.Arac.Tanimla().MaasMatrahKaydet(kullanan, yil, surecNo, bordroYapi.VTTabloAd, matrahListesi);
                }
                //else
                //    OrtakFonksiyonlar.HataDosyaYaz("Matrah.txt", "Matrah silme hatası" + yil + " " + surecNo + " " + bordroYapi.VTTabloAd + " Hata:" + matrahSonuc.hataStr, false);

                if (!matrahSonuc.islemSonuc)
                {
                    OrtakFonksiyonlar.HataDosyaYaz("Matrah.txt", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " Matrah hatası" + yil + " " + surecNo + " " + bordroYapi.VTTabloAd + " Hata:" + matrahSonuc.hataStr, false);
                    s.bilgiStr = "Bordro kaydı yapıldı fakat matrahlar kaydedilemedi. Hata:" + matrahSonuc.hataStr;
                }
                //else
                //    OrtakFonksiyonlar.HataDosyaYaz("Matrah.txt", "Matrah kaydedildi" + yil + " " + surecNo + " " + bordroYapi.VTTabloAd, false);
            }
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc CalisanDinamikBordroSil(VeriErisim veriErisim, Kullanici kullanan, int surecYil, string surecNo, BordroYapi bordroYapi, TNSCollection tcListe)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            CalisanDinamikBordroDonem cdbdForm;
            Sonuc s = CalisanDinamikBordroOnayliMi(ve, kullanan, surecYil, surecNo, bordroYapi, out cdbdForm);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }
            TNSDateTime odemeTarih = new TNSDateTime(DateTime.Now);
            ve.BeginTransaction();
            TABHRCBORDROTANIM tab = new TABHRCBORDROTANIM(ve);
            if (tcListe == null)
            {
                s = tab.BordroSil(surecYil, surecNo, bordroYapi.VTTabloAd, null, 0);
            }
            else
            {
                for (int i = 0; i < tcListe.Count && s.islemSonuc; i++)
                {
                    IntKodStrAd isAd = (IntKodStrAd)tcListe[i];
                    s = tab.BordroSil(surecYil, surecNo, bordroYapi.VTTabloAd, isAd.ad, 0);
                    if (!s.islemSonuc) break;
                }
            }
            if (s.islemSonuc)
            {
                ve.Commit();
                Sonuc matrahSonuc = TNS.IKY.Arac.Tanimla().MaasMatrahSil(kullanan, surecYil, surecNo, bordroYapi.VTTabloAd, tcListe);
                if (!matrahSonuc.islemSonuc)
                    s.bilgiStr = "Bordro kaydı yapıldı fakat matrahlar silinemedi. Hata:" + matrahSonuc.hataStr;
            }
            else
            {
                ve.Rollback();
            }
            return s;
        }

        internal static Sonuc BordroSablonSil(VeriErisim veriErisim, Kullanici kullanan, BordroSablonDosya bsd)
        {
            Sonuc s = new Sonuc();
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                s.DegerAta("İşlem için yetkiniz bulunmamaktadır!");
            }

            if (bsd.sablonKod <= 0)
                s.DegerAta("Şablon kodunu giriniz!");

            if (bsd.bordroTur < 0)
                s.DegerAta("Bordro tür negatif olamaz!");

            if (bsd.islemTur < 0)
                s.DegerAta("İşlem tür negatif olamaz!");

            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTANIM tab = new TABHRCBORDROTANIM(ve);

            ObjectVAN ov = BordroSablonOku(ve, kullanan, bsd);
            if (!ov.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ov.sonuc;
            }
            BordroSablonDosya okunan = (BordroSablonDosya)ov.o;

            if (okunan == null)
            {
                s.DegerAta("Silinecek kayıt bulunamadı! ");
            }
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();
            s = tab.SablonSil(bsd);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc BordroSablonSakla(VeriErisim veriErisim, Kullanici kullanan, BordroSablonDosya bsd, bool yeni)
        {
            Sonuc s = new Sonuc();
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                s.DegerAta("İşlem için yetkiniz bulunmamaktadır!");
            }

            if (string.IsNullOrWhiteSpace(bsd.sablonAd))
                s.DegerAta("Şablon adı boş olamaz!");
            if (bsd.sablonKod <= 0)
                s.DegerAta("Şablon kodunu giriniz!");

            if (bsd.bordroTur < 0)
                s.DegerAta("Bordro tür negatif olamaz!");

            if (bsd.islemTur < 0)
                s.DegerAta("İşlem tür negatif olamaz!");

            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTANIM tab = new TABHRCBORDROTANIM(ve);

            ObjectVAN ov = BordroSablonOku(ve, kullanan, bsd);
            if (!ov.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ov.sonuc;
            }
            BordroSablonDosya okunan = (BordroSablonDosya)ov.o;

            if (yeni && okunan != null)
            {
                s.DegerAta("Yeni kayıt işlemi yaparken, verilen koda ait kayıt bulundu! ");
            }
            else if (!yeni && okunan == null)
            {
                s.DegerAta("Güncelleme işlemi yaparken, verilen koda ait kayıt bulunamadı! ");
            }
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();
            if (yeni)
                s = tab.SablonEkle(bsd);
            else
                s = tab.SablonGuncelle(bsd);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        internal static ObjectVAN BordroSablonOku(VeriErisim ve, Kullanici kullanan, BordroSablonDosya bsd)
        {
            ObjectVAN ov = new ObjectVAN();
            TABHRCBORDROTANIM tab = new TABHRCBORDROTANIM(ve);
            IDataReader dr = tab.SablonListele(true, bsd.bordroTur, bsd.islemTur, bsd.sablonKod);
            if (dr == null)
            {
                ov.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return ov;
            }
            ov.o = null;
            if (dr.Read())
            {
                ov.o = SablonOl(dr);
            }
            dr.Close();
            return ov;
        }

        internal static ObjectArray BordroSablonListele(VeriErisim veriErisim, Kullanici kullanan, int bordroTur, int islemTur, ENUMUCDURUM sayfa, ENUMUCDURUM ortakRapor)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTANIM tab = new TABHRCBORDROTANIM(ve);

            IDataReader dr = tab.SablonListele(false, -1, -1, -1);

            if (dr == null)
            {
                oa.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return oa;
            }

            while (dr.Read())
            {
                BordroSablonDosya bsd = SablonOl(dr);
                if (bsd.UyumluMu(bordroTur, islemTur, sayfa, ortakRapor))
                    oa.Ekle(bsd);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return oa;
        }

        private static BordroSablonDosya SablonOl(IDataReader dr)
        {
            return new BordroSablonDosya(VeriErisim.DegerAl(dr, 0, 0),
                VeriErisim.DegerAl(dr, 1, 0),
                VeriErisim.DegerAl(dr, 2, 0),
                VeriErisim.DegerAl(dr, 3, ""),
                VeriErisim.DegerAl(dr, 4, ""),
                VeriErisim.DegerAl(dr, 5, 0) == 1,
                VeriErisim.DegerAl(dr, 6, ""),
                VeriErisim.DegerAl(dr, 7, 0) == 1,
                VeriErisim.DegerAl(dr, 8, 0) == 1);
        }

        internal static Sonuc DonemKaydet(VeriErisim veriErisim, Kullanici kullanan, CalisanDinamikBordroDonem form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTANIM tabHRCCALISANDINAMIKBORDRODONEM = new TABHRCBORDROTANIM(ve);

            string[] personelTipDizi = form.bordroTur.Split(';', ',');
            int personelTip = 0;
            if (personelTipDizi.Length > 0)
            {
                personelTip = OrtakFonksiyonlar.ConvertToInt(personelTipDizi[0], 0);
            }

            Sonuc s;
            if (personelTip > 0)
            {
                PersonelTipi ptForm = new PersonelTipi();
                ptForm.kod = personelTip;
                ObjectVAN ovpt = TNS.IKY.Arac.Tanimla().OkuPersonelTip(kullanan, ptForm);
                if (ovpt.o != null)
                {
                    ptForm = (PersonelTipi)ovpt.o;
                    s = form.HataKontrol(ptForm.pesinMi, ptForm.maasDonemGun, ptForm.maasOdemeGun, (BordroIslemTur.ENUMBordroIslemTurYapi)form.bordroIslem);
                }
                else
                    s = new Sonuc("Personel tipi okunamadı!");
            }
            else
                s = new Sonuc("Personel tipi boş geldi!");
            if (s.islemSonuc && (form.bordroIslem == (int)BordroIslemTur.ENUMBordroIslemTurYapi.Normal || form.bordroIslem == (int)BordroIslemTur.ENUMBordroIslemTurYapi.Fark))
            {
                ObjectVAN ov = DonemOku(ve, kullanan, form, form.bordroDonemBasTarih);
                if (ov.o != null)
                {
                    form = (CalisanDinamikBordroDonem)ov.o;
                    s.DegerAta(string.Format("{0}/{1} numaralı süreçte benzer bordro dönemine ait bordro girişi olduğundan kaydedemezsiniz", form.yil, form.surecNo));
                }
            }
            if (s.islemSonuc)
            {
                ve.BeginTransaction();

                if (s.islemSonuc)
                    s = tabHRCCALISANDINAMIKBORDRODONEM.DonemSil(form);

                if (s.islemSonuc)
                    s = tabHRCCALISANDINAMIKBORDRODONEM.DonemEkle(form);

                if (s.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }

        internal static ObjectVAN DonemOku(VeriErisim veriErisim, Kullanici kullanan, CalisanDinamikBordroDonem form, TNSDateTime tarihKisit)
        {
            ObjectVAN ov = new ObjectVAN();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTANIM tabHRCCALISANDINAMIKBORDRODONEM = new TABHRCBORDROTANIM(ve);

            IDataReader dr = tabHRCCALISANDINAMIKBORDRODONEM.DonemOku(form, tarihKisit);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                ov.sonuc.DegerAta(ve.hata);
                return ov;
            }

            CalisanDinamikBordroDonem detay = new CalisanDinamikBordroDonem(form.yil, form.surecNo);
            if (dr.Read())
            {
                DRDenDonemOku(detay, dr);
            }
            else if (!TNSDateTime.IsNull(tarihKisit))
            {
                detay = null;
            }
            dr.Close();
            ve.VeriErisimKapat();
            if (TNSDateTime.IsNull(tarihKisit))
            {
                TNS.MUH.IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
                MUH.OdemeEmriMIFForm mifForm = new MUH.OdemeEmriMIFForm();
                mifForm.surecYil = form.yil;
                mifForm.surecNo = form.surecNo;
                mifForm.formDurum = 0;
                ObjectArray oa = muhServis.OdemeEmriMIFListele(kullanan, mifForm, false);
                detay.MifFormListe = oa.objeler;
                //mifForm.detay.Ekle(new MUH.OdemeEmriMIFDetay(form.yil, "920", 0, 0));
                //ObjectArray oa2 = muhServis.OdemeEmriMIFListele(kullanan, mifForm, false);
                //detay.taahhutMIFListe = oa2.objeler;

                //foreach (MUH.OdemeEmriMIFForm item in detay.taahhutMIFListe)
                //{
                //    MUH.OdemeEmriMIFForm bulunan = ListedeMIFBul(detay.MifFormListe, item);
                //    if (bulunan != null)
                //        detay.MifFormListe.Remove(bulunan);
                //}
                //}
                //else
                //    detay.taahhutMIFListe = new TNSCollection();
            }
            ov.o = detay;
            return ov;
        }

        private static MUH.OdemeEmriMIFForm ListedeMIFBul(TNSCollection mifFormListe, MUH.OdemeEmriMIFForm mifForm)
        {
            foreach (MUH.OdemeEmriMIFForm item in mifFormListe)
            {
                if (item.yil == mifForm.yil && item.belgeNo == mifForm.belgeNo) return item;
            }
            return null;
        }

        private static void DRDenDonemOku(CalisanDinamikBordroDonem detay, IDataReader dr)
        {
            detay.yil = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarDonem.YIL, 0);
            detay.surecNo = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarDonem.SURECNO, "");
            detay.hazirlamaTarih = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarDonem.HAZIRLAMATARIH);
            detay.odemeTarih = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarDonem.ODEMETARIH);
            detay.hazirlayan = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarDonem.HAZIRLAYAN, "");
            detay.onayaSunan = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarDonem.ONAYASUNAN, "");
            detay.onaylayan = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarDonem.ONAYLAYAN, "");
            detay.bordroDonemBasTarih = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarDonem.ONCBORDROTARIH);
            if (detay.bordroDonemBasTarih == null || detay.bordroDonemBasTarih.isNull) detay.bordroDonemBasTarih = new TNSDateTime(detay.odemeTarihDT);
            detay.bordroTur = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarDonem.BORDROTUR, "");
            detay.bordroIslem = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarDonem.BORDROISLEM, 0);

            detay.MifFormListe = null;

            detay.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarDonem.BORDROISLEM + 1, 0);
            detay.birimKod = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarDonem.BORDROISLEM + 2, "");
            detay.muhasebeAd = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarDonem.BORDROISLEM + 3, "");
            detay.birimAd = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlarDonem.BORDROISLEM + 4, "");
        }

        public class KisiBilgi
        {
            public string birimKod { get; set; }

            public int personelTip { get; set; }

            public string tcKimlikNo { get; set; }
        }

        internal static BordroMuhasebeKayitlari HesapOku(VeriErisim veriErisim, Kullanici kullanan, int yil, string surecNo, TNSCollection bordroTurler, int bordroIslemTur, CalisanDinamikBordroDonem bordroDonem, int avansIslemKod, string odemeEmriHesapKod)
        {
            BordroMuhasebeKayitlari bmk = new BordroMuhasebeKayitlari();
            ObjectArray oa = HesapOkuIC(veriErisim, kullanan, yil, surecNo, bordroTurler, bordroIslemTur, bordroDonem, avansIslemKod, odemeEmriHesapKod, 1);
            if (!oa.sonuc.islemSonuc)
                bmk.sonuc = oa.sonuc;
            else
            {
                bmk.OdemeEmri = oa.objeler;
                oa = HesapOkuIC(veriErisim, kullanan, yil, surecNo, bordroTurler, bordroIslemTur, bordroDonem, avansIslemKod, odemeEmriHesapKod, 2);
                if (!oa.sonuc.islemSonuc)
                    bmk.sonuc = oa.sonuc;
                else
                    bmk.TahakkukKayitlari = oa.objeler;
            }

            return bmk;
        }

        internal static ObjectArray HesapOkuIC(VeriErisim veriErisim, Kullanici kullanan, int yil, string surecNo, TNSCollection bordroTurler, int bordroIslemTur, CalisanDinamikBordroDonem bordroDonem, int avansIslemKod, string odemeEmriHesapKod, int odemeEmri1TahA2TahB3)
        {
            TNS.HRC.MerkezBankasiKurSinif kriterKB = new TNS.HRC.MerkezBankasiKurSinif();
            kriterKB.kurTarih = bordroDonem.hazirlamaTarih;
            TNS.HRC.MerkezBankasiKurSinif gunlukKur = HRCMerkezBankKur.Oku(null, kullanan, kriterKB);
            TNS.MUH.IMUHServis muhServis = TNS.MUH.Arac.Tanimla();

            ObjectArray hesaplar = new ObjectArray();
            for (int bordroTurYer = 0; bordroTurYer < bordroTurler.Count; bordroTurYer++)
            {
                int bordroYapiTur = (int)bordroTurler[bordroTurYer];
                BordroYapi by = BordroYapiOku(veriErisim, kullanan, bordroYapiTur, bordroIslemTur, false, null, null, false);
                BordroKistas bordroKistas = new BordroKistas();
                bordroKistas.yil = yil;
                bordroKistas.surecNo = surecNo;
                CalisanDinamikBordro cdb = CalisanDinamikBordroOku(veriErisim, kullanan, by, bordroKistas, false);
                int mahsupAy = bordroDonem.islemAy;
                MuhasebeHesaplariEkle(kullanan, muhServis, gunlukKur, hesaplar, cdb, by, bordroTurYer, bordroDonem, yil, odemeEmriHesapKod, odemeEmri1TahA2TahB3, mahsupAy, null);
            }

            if (odemeEmri1TahA2TahB3 == 1)
            {
                BordroMuhasebe h835 = DizideHesapBul(hesaplar.objeler, "835");

                if (avansIslemKod == 1)
                    YarimAvansIslemAc(muhServis, yil, hesaplar, avansIslemKod == 1, h835);
                else if (avansIslemKod == 11)
                    YarimAvansIslemKapa(muhServis, yil, hesaplar, avansIslemKod == 1, h835);
                else if (avansIslemKod == 2)
                    TamAvansIslemAc(muhServis, yil, hesaplar, avansIslemKod == 1, h835);
                else if (avansIslemKod == 12)
                    TamAvansIslemKapa(muhServis, yil, hesaplar, avansIslemKod == 1, h835);
                else if (avansIslemKod == 10)
                    hesaplar.objeler.Clear();
            }
            else
            {
                TaahhutKapat(muhServis, yil, hesaplar);
            }

            hesaplar.objeler.Sort();
            return hesaplar;
        }

        private static void TaahhutKapat(IMUHServis muhServis, int yil, ObjectArray oa)
        {
            BordroMuhasebe taahhutKapamaHesabi = BordroMuhasebeOl(muhServis, yil, "", "921", "", true);
            foreach (BordroMuhasebe item in oa.objeler)
            {
                for (int ii = 0; ii < item.dizi.Length; ii++)
                {
                    taahhutKapamaHesabi.dizi[ii].borc -= item.dizi[ii].borc;
                    taahhutKapamaHesabi.dizi[ii].borcDoviz -= item.dizi[ii].borcDoviz;
                }
            }
            oa_Ekle(oa, taahhutKapamaHesabi);
        }

        public static void MuhasebeHesaplariEkle(Kullanici kullanan, TNS.MUH.IMUHServis muhServis, TNS.HRC.MerkezBankasiKurSinif gunlukKur, ObjectArray hesaplar, CalisanDinamikBordro cdb, BordroYapi by, int bordroTurYer, CalisanDinamikBordroDonem bordroDonem, int yil, string odemeEmriHesapKod, int odemeEmri1TahA2TahB3, int mahsupAy, Sonuc s)
        {
            KisiBilgi kisiBilgi = new KisiBilgi();
            foreach (KisiSatir kisiSatir in cdb.kisiDegerler)
            {
                System.Object[] al = kisiSatir.degerler;
                kisiBilgi.birimKod = (string)CalisanDinamikBordro.DegerBul(by, kisiSatir, "birimKod") + "";
                kisiBilgi.personelTip = CalisanDinamikBordro.DegerBulInt(by, kisiSatir, "personelTip");
                kisiBilgi.tcKimlikNo = (string)al[0];
                string sendikaHesapKod = "";
                for (int i = 0; i < by.hepsi.Length; i++)
                {
                    if (by.hepsi[i].ad == "sendikaBilgi")
                        sendikaHesapKod = OrtakFonksiyonlar.DiziElemanAl(((string)al[i + 1]).Split('-'), 3, "");
                    DiziyeEkle(kisiBilgi, bordroTurYer, hesaplar, by.hepsi[i], al[i + 1], muhServis, yil, bordroDonem, gunlukKur, odemeEmriHesapKod, odemeEmri1TahA2TahB3, mahsupAy, s, sendikaHesapKod);
                }
            }
        }

        private static void TamAvansIslemKapa(IMUHServis muhServis, int yil, ObjectArray oa, bool p, BordroMuhasebe h835)
        {
            BordroMuhasebe hAvans = BordroMuhasebeOl(muhServis, yil, "", "161.18.01.02", "", true);
            ObjectArray oa2 = new ObjectArray();
            foreach (BordroMuhasebe item in oa.objeler)
            {
                if (OdemeMi(item.hesapKod.Replace(".", "")))
                {
                    item.hesapSatir = hAvans.hesapSatir;
                }
            }
        }

        private static void TamAvansIslemAc(IMUHServis muhServis, int yil, ObjectArray oa, bool p, BordroMuhasebe h835)
        {
            BordroMuhasebe hAvans = BordroMuhasebeOl(muhServis, yil, "", "161.18.01.02", "", true);
            ObjectArray oa2 = new ObjectArray();
            foreach (BordroMuhasebe item in oa.objeler)
            {
                if (OdemeMi(item.hesapKod.Replace(".", "")))
                {
                    oa2.Ekle(item);
                    for (int ii = 0; ii < item.dizi.Length; ii++)
                    {
                        hAvans.dizi[ii].borc -= item.dizi[ii].borc;
                        hAvans.dizi[ii].borcDoviz -= item.dizi[ii].borcDoviz;
                    }
                }
            }
            oa.objeler.Clear();
            oa.objeler.AddRange(oa2.objeler);
            oa_Ekle(oa, hAvans);
        }

        private static void YarimAvansIslemKapa(IMUHServis muhServis, int yil, ObjectArray oa, bool p, BordroMuhasebe h835)
        {
            ObjectArray oa2 = new ObjectArray();
            for (int i = 0; i < h835.dizi.Length; i++)
            {
                h835.dizi[i] = new BordroMuhasebe.BorcAlacak();
            }

            foreach (BordroMuhasebe item in oa.objeler)
            {
                if (ButceGideriMi(item.hesapKod.Replace(".", "")))
                {
                    BordroMuhasebe avans = null;
                    string avansHesapKod = AvansHesapBul(item.hesapKod.Replace(".", ""), true);
                    if (!string.IsNullOrWhiteSpace(avansHesapKod))
                    {
                        avans = BordroMuhasebeOl(muhServis, yil, "", avansHesapKod, "", true);
                    }
                    for (int ii = 0; ii < item.dizi.Length; ii++)
                    {
                        double dus = 0;
                        double dusDoviz = 0;
                        dus = Math.Round(item.dizi[ii].borc / 2, 2, MidpointRounding.AwayFromZero);
                        dusDoviz = Math.Round(item.dizi[ii].borcDoviz / 2, 2, MidpointRounding.AwayFromZero);
                        item.dizi[ii].borc = dus;
                        item.dizi[ii].borcDoviz = dusDoviz;
                        if (avans != null)
                        {
                            avans.dizi[ii].borc = 0 - dus;
                            avans.dizi[ii].borcDoviz = 0 - dusDoviz;
                        }
                        else
                        {
                            h835.dizi[ii].borc -= dus;
                            h835.dizi[ii].borcDoviz -= dusDoviz;
                        }
                    }
                    oa_Ekle(oa2, item);
                    if (avans != null)
                    {
                        oa_Ekle(oa2, avans);
                    }
                }
            }
            oa_Ekle(oa2, h835);
            oa.objeler.Clear();
            oa.objeler.AddRange(oa2.objeler);
        }

        private static void YarimAvansIslemAc(IMUHServis muhServis, int yil, ObjectArray oa, bool acilis, BordroMuhasebe h835)
        {

            ObjectArray oa2 = new ObjectArray();

            foreach (BordroMuhasebe item in oa.objeler)
            {
                if (ButceGideriMi(item.hesapKod.Replace(".", "")))
                {
                    BordroMuhasebe avans = null;
                    string avansHesapKod = AvansHesapBul(item.hesapKod.Replace(".", ""), true);
                    if (!string.IsNullOrWhiteSpace(avansHesapKod))
                    {
                        avans = BordroMuhasebeOl(muhServis, yil, "", avansHesapKod, "", true);
                    }
                    for (int ii = 0; ii < item.dizi.Length; ii++)
                    {
                        double dus = 0;
                        double dusDoviz = 0;
                        dus = OrtakFonksiyonlar.KurusYap(item.dizi[ii].borc / 2);
                        dusDoviz = OrtakFonksiyonlar.KurusYap(item.dizi[ii].borcDoviz / 2);
                        item.dizi[ii].borc = item.dizi[ii].borc - dus;
                        item.dizi[ii].borcDoviz = item.dizi[ii].borcDoviz - dusDoviz;
                        if (avans != null)
                        {
                            avans.dizi[ii].borc = dus;
                            avans.dizi[ii].borcDoviz = dusDoviz;
                        }
                        else
                        {
                            if (h835 != null)
                            {
                                h835.dizi[ii].borc += dus;
                                h835.dizi[ii].borcDoviz += dusDoviz;
                            }
                        }
                    }
                    if (avans != null)
                    {
                        oa_Ekle(oa2, avans);
                    }
                }
            }
            oa.objeler.AddRange(oa2.objeler);
        }

        private static BordroMuhasebe DizideHesapBul(TNSCollection objeler, string hesapKod)
        {
            foreach (BordroMuhasebe item in objeler)
            {
                if (item.hesapKod == hesapKod)
                {
                    return item;
                }
            }
            return null;
        }

        private static bool OdemeMi(string hesapKod)
        {
            if (string.IsNullOrWhiteSpace(hesapKod)) return false;
            if (hesapKod.Length <= 4) return false;
            if (hesapKod.StartsWith("10302")) return true;
            return false;
        }

        private static string AvansHesapBul(string hesapKod, bool yarim)
        {
            if (string.IsNullOrWhiteSpace(hesapKod)) return "";
            if (hesapKod.Length <= 9) return "";
            if (hesapKod.StartsWith("630010201"))
            {
                if (yarim) return "162.05.01";
                else return "161.18.01.02";
            }
            return "";
        }

        private static bool ButceGideriMi(string hesapKod)
        {
            if (string.IsNullOrWhiteSpace(hesapKod)) return false;
            if (hesapKod.Length <= 9) return false;
            if (hesapKod.StartsWith("630010201") || hesapKod.StartsWith("830010201"))
            {
                return true;
            }
            return false;
        }

        private static void oa_Ekle(ObjectArray oa, BordroMuhasebe bm)
        {
            if (bm.toplam.borc != 0)
                oa.Ekle(bm);
        }

        private static void DiziyeEkle(KisiBilgi kisiBilgi, int bordroTurYer, ObjectArray hesaplar, DegiskenFormul degiskenFormul, object deger, TNS.MUH.IMUHServis muhServis, int yil, CalisanDinamikBordroDonem bordroDonem, TNS.HRC.MerkezBankasiKurSinif gunlukKur, string odemeEmriHesapKod, int odemeEmri1TahA2TahB3, int mahsupAy, Sonuc s, string sendikaHesapKod)
        {
            if (odemeEmri1TahA2TahB3 != 1 && degiskenFormul.siraNo.IndexOf("[T]") < 0) return;
            DiziyeEkle(kisiBilgi, bordroTurYer, hesaplar, degiskenFormul, true, deger, muhServis, yil, bordroDonem, gunlukKur, odemeEmriHesapKod, odemeEmri1TahA2TahB3, mahsupAy, s, sendikaHesapKod);
            if (odemeEmri1TahA2TahB3 == 1)
                DiziyeEkle(kisiBilgi, bordroTurYer, hesaplar, degiskenFormul, false, deger, muhServis, yil, bordroDonem, gunlukKur, odemeEmriHesapKod, odemeEmri1TahA2TahB3, mahsupAy, s, sendikaHesapKod);
        }

        private static void DiziyeEkle(KisiBilgi kisiBilgi, int bordroTurYer, ObjectArray hesaplar, DegiskenFormul degiskenFormul, bool borcMu, object o_deger, TNS.MUH.IMUHServis muhServis, int yil, CalisanDinamikBordroDonem bordroDonem, TNS.HRC.MerkezBankasiKurSinif gunlukKur, string odemeEmriHesapKod, int odemeEmri1TahA2TahB3, int mahsupAy, Sonuc s, string sendikaHesapKod)
        {
            double deger;

            if (degiskenFormul.tip == VariableType.Money) deger = (double)o_deger;
            else if (degiskenFormul.tip == VariableType.Integer) deger = (int)o_deger;
            else return;
            deger = Math.Round(deger, 2);
            if (deger == 0) return;

            string hesapkodlarStr;
            if (borcMu)
                hesapkodlarStr = HesapBul(kisiBilgi, degiskenFormul.borcHesap);
            else
                hesapkodlarStr = HesapBul(kisiBilgi, degiskenFormul.alacakHesap);
            if (string.IsNullOrWhiteSpace(hesapkodlarStr))
            {
                return;
            }

            if (!borcMu || (odemeEmri1TahA2TahB3 == 2))
                deger = 0 - deger;

            string[] hesapkodlar = hesapkodlarStr.Split(',', ';');
            foreach (string hesapKod in hesapkodlar)
            {
                HesapEkle(hesapKod, deger, bordroTurYer, hesaplar, muhServis, yil, kisiBilgi, gunlukKur, odemeEmriHesapKod, odemeEmri1TahA2TahB3, mahsupAy, sendikaHesapKod);
            }
        }

        private static string HesapBul(KisiBilgi kisiBilgi, string hesapKod)
        {
            if (string.IsNullOrWhiteSpace(hesapKod)) return "";
            string[] hler = hesapKod.Split('-');
            foreach (var item in hler)
            {
                if (item.StartsWith("["))
                {
                    string sol = "";
                    string sag = "";
                    OrtakFonksiyonlar.StringiIkiyeBol(item, "]", ref sol, ref sag);
                    int gelenKod = OrtakFonksiyonlar.ConvertToInt(sol.Substring(1), -1);
                    if (gelenKod == kisiBilgi.personelTip) return sag;
                }
                else
                    return item;
            }
            return "";
        }

        private static void HesapEkle(string hesapKod, double degerDovizAP, int bordroTurYer, ObjectArray hesaplar, IMUHServis muhServis, int yil, KisiBilgi kisiBilgi, TNS.HRC.MerkezBankasiKurSinif gunlukKur, string odemeEmriHesapKod, int odemeEmri1TahA2TahB3, int mahsupAy, string sendikaHesapKod)
        {
            if (string.IsNullOrWhiteSpace(hesapKod)) return;
            bool ortakAltHesap = false;
            if (hesapKod.EndsWith("*"))
            {
                ortakAltHesap = true;
                hesapKod = SonKarakterSil(hesapKod);
            }
            if (hesapKod == "{OEH}")
            {
                hesapKod = odemeEmriHesapKod;
                if (string.IsNullOrWhiteSpace(hesapKod)) hesapKod = "103.02";
            }
            if (hesapKod == "{SND}")
            {
                hesapKod = sendikaHesapKod;
                if (string.IsNullOrWhiteSpace(hesapKod)) hesapKod = "333.10";
            }
            if (odemeEmri1TahA2TahB3 == 2)
            {
                if (!(hesapKod.StartsWith("830.01") || hesapKod.StartsWith("800.01"))) return;

                PersonelTipi pt = TNS.IKY.Arac.PersonelTipBul(kisiBilgi.personelTip);
                if (pt.HITAP == ENUMHizmetTablolari.Bagkur) return;

                hesapKod = "920.01" + hesapKod.Substring(3) + "." + mahsupAy.ToString("00");
                ortakAltHesap = false;
            }
            BordroMuhasebe oncekiHesap = BordroMuhasebe.HesapBul(hesaplar.objeler, hesapKod);

            double deger, degerDoviz;
            if (oncekiHesap == null)
            {
                BordroMuhasebe bm = BordroMuhasebeOl(muhServis, yil, kisiBilgi.birimKod, hesapKod, kisiBilgi.tcKimlikNo, ortakAltHesap);
                DegerBul(bm.hesapSatir, gunlukKur, degerDovizAP, out deger, out degerDoviz);
                if (hesapKod.StartsWith("800.01"))
                {
                    deger *= -1;
                    degerDoviz *= -1;
                }
                bm.DegerAta(bordroTurYer, deger, degerDoviz);
                hesaplar.Ekle(bm);
            }
            else
            {
                BordroMuhasebe bm = new BordroMuhasebe();
                bm.ortakAltHesap = oncekiHesap.ortakAltHesap;
                bm.hesapSatir = oncekiHesap.hesapSatir;
                DegerBul(bm.hesapSatir, gunlukKur, degerDovizAP, out deger, out degerDoviz);
                bm.birimKod = kisiBilgi.birimKod;
                if (hesapKod.StartsWith("800.01"))
                {
                    deger *= -1;
                    degerDoviz *= -1;
                }
                bm.DegerAta(bordroTurYer, deger, degerDoviz);
                if (oncekiHesap.AltHesapVarMi())
                    bm.altHesapKod = kisiBilgi.tcKimlikNo;
                int yer = hesaplar.objeler.IndexOf(bm);
                if (yer < 0)
                    hesaplar.Ekle(bm);
                else
                {
                    BordroMuhasebe bmYer = (BordroMuhasebe)hesaplar.objeler[yer];
                    bmYer.DegerEkle(bordroTurYer, deger, degerDoviz);
                }
            }
        }


        private static void DegerBul(HesapPlaniSatir hesapSatir, MerkezBankasiKurSinif gunlukKur, double degerDovizAP, out double deger, out double degerDoviz)
        {
            if (string.IsNullOrWhiteSpace(hesapSatir.dovizKurKod))
            {
                deger = degerDovizAP;
                degerDoviz = 0;
            }
            else
            {
                decimal kurDeger;
                if (gunlukKur == null) kurDeger = 1;
                kurDeger = gunlukKur.KurBul(hesapSatir.dovizKurKod, "EALIS", 1, 10);
                degerDoviz = degerDovizAP;
                deger = Convert.ToDouble(Math.Round(Convert.ToDecimal(degerDoviz) * kurDeger, 2, MidpointRounding.AwayFromZero));
            }
        }

        private static string SonKarakterSil(string hesapKod)
        {
            return hesapKod.Substring(0, hesapKod.Length - 1);
        }

        private static BordroMuhasebe BordroMuhasebeOl(TNS.MUH.IMUHServis muhServis, int yil, string birimKod, string hesapKod, string altHesapKod, bool ortakAltHesap)
        {
            BordroMuhasebe bm = new BordroMuhasebe();
            HesapPlaniSatir hs = new HesapPlaniSatir();
            hs.hesapKod = hesapKod;
            hs.yil = yil;
            bm.hesapSatir = muhServis.HesapKodOzellikAl(hs);
            if (string.IsNullOrWhiteSpace(bm.hesapKod)) bm.hesapKod = hesapKod;
            bm.ortakAltHesap = ortakAltHesap;
            if (bm.AltHesapVarMi()) bm.altHesapKod = altHesapKod;
            bm.birimKod = birimKod;
            return bm;
        }

        internal static Sonuc BordroVeriAktar(VeriErisim veriErisim, Kullanici kullanan, int yil, string surecNo, TNSCollection bordroTurler, int bordroIslemTur, int aktar)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            Surec s = TNS.HRC.IsKurallari.HRCSurec.SurecOku(ve, kullanan, yil, surecNo);
            CalisanDinamikBordroDonem form = new CalisanDinamikBordroDonem(yil, surecNo);
            ObjectVAN ov = DonemOku(ve, kullanan, form, null);
            if (!ov.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ov.sonuc;
            }
            CalisanDinamikBordroDonem donem = (CalisanDinamikBordroDonem)ov.o;
            if (donem == null || donem.odemeTarih == null || donem.odemeTarih.isNull)
            {
                ve.VeriErisimKapat();
                return new Sonuc("Dönem bilgisi bulunamadı, önce onay formunu girip kaydediniz!");
            }
            if (donem.OnayliMIFVarMi())
            {
                ve.VeriErisimKapat();
                return new Sonuc("Bu süreçle ilgili onaylı ödeme emri olduğundan aktarma işlemi yapılamaz. Lütfen önce ödeme emirlerini iptal ediniz!");
            }
            for (int i = 0; i < bordroTurler.Count; i++)
            {
                int bordroTur = (int)bordroTurler[i];
                BordroYapi bordroYapi = BordroYapiOku(ve, kullanan, bordroTur, bordroIslemTur, false, null, null, false);
                ObjectArray satirlar;
                if (aktar == 1)
                    satirlar = VeriAl(ve, s, bordroTur, bordroIslemTur, donem.odemeTarih, bordroYapi);
                else if (aktar == 2)
                    satirlar = VeriAlUA(ve, s, bordroTur, bordroIslemTur, donem.odemeTarih, bordroYapi);
                else
                    satirlar = null;
                if (satirlar != null)
                {
                    if (satirlar.sonuc.islemSonuc)
                        ov.sonuc = CalisanDinamikBordroKaydet(ve, kullanan, yil, surecNo, bordroYapi, satirlar.objeler);
                    else
                        ov.sonuc = satirlar.sonuc;
                }
                else
                    ov.sonuc.DegerAta("Yöntem bulunamadı!");
                if (!ov.sonuc.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return ov.sonuc;
                }
            }
            ve.VeriErisimKapat();
            return ov.sonuc;
        }

        private static ObjectArray VeriAlUA(VeriErisim ve, Surec s, int bordroTur, int bordroIslemTur, TNSDateTime bordrotarih, BordroYapi bordroYapi)
        {
            //string[] kodAd = {
            //    "", "sicil", "", "ad", "soyad", "unvan", "kidem", "emkMuk", "gosterge", "ekGosterge",
            //    "agi", "ucret", "emKesD", "yuzArtD", "sagSigD", "toplam", "ozelTel", "netOdenen", "kira", "sendika",
            //    "yemek", "gelirVer", "damgaVer", "emKesK", "yuzArtK", "", "", "", "", "gelToplam",
            //    "", "sagSigK", "icraNafaka", "kesToplam", "nvm", "cvm", "tvm", "gelVerOr", "", "",
            //    "", "", "kesenekFark", "digSozZamTaz", "", "issizSigIsv", "issizSigIsci", "sgkDamgaV", "sskPrimIsc", "sskPrimIsv",
            //    "myos", "besKesinti"};
            /*
             * 
             
            CONVERT(VARCHAR, SICILNO) AS F1,  CONVERT(VARCHAR, TCKIMLIKNO) AS F2,  CONVERT(VARCHAR, AD) AS F3,  CONVERT(VARCHAR, SOYAD) AS F4,  CONVERT(VARCHAR, UNVAN) AS F5,  CONVERT(VARCHAR, KIDEM) AS F6,  CONVERT(VARCHAR, DERECEKADEME) AS F7,  CONVERT(INT, GOSTERGE) AS F8,  CONVERT(INT, EKGOSTERGE) AS F9, 
            CONVERT(money, ASGARIGECIMINDIRIM) AS F10,  CONVERT(money, SOZLESMEUCRET) AS F11, CONVERT(money, EMEKLIKESENEKDEVLET) AS F12, CONVERT(money, YUZDEARTISDEVLET) AS F13, CONVERT(money, SAGLIKSIGORTADEVLET) AS F14,  CONVERT(money, GELIRTOPLAM) AS F15, CONVERT(money, OZELTELEFONGORUSME) AS F16, CONVERT(money, NETODENEN) AS F17, CONVERT(money, KIRAKESINTI) AS F18, CONVERT(money,  SENDIKAAIDAT) AS F19, 
            CONVERT(money, YEMEKUCRET) AS F20, CONVERT(money, GELIRVERGI + ASGARIGECIMINDIRIM) AS F21, CONVERT(money, DAMGAVERGI) AS F22, CONVERT(money, EMEKLIKESENEKKISI) AS F23, CONVERT(money, YUZDEARTISDEVLET) AS F24, CONVERT(money, YUZDEARTIKISI - YUZDEARTISDEVLET) AS F25, CONVERT(money, EMEKLIKESENEKDEVLET) AS F26,  CONVERT(money, SAGLIKSIGORTADEVLET) AS F27, CONVERT(money, OZELTELEFONGORUSME) AS F28, CONVERT(money, GELIRTOPLAM) AS F29, 
            CONVERT(money, OZELTELEFONGORUSME) AS F30,  CONVERT(money, SAGLIKSIGORTAKISI) AS F31, CONVERT(money, ICRANAFAKA) AS F32, CONVERT(money, KESINTITOPLAM) AS F33, CONVERT(money, NAKLEDILENVERGIMATRAH) AS F34, CONVERT(money, CARIVERGIMATRAH) AS F35, CONVERT(money, TOPLAMVERGIMATRAH) AS F36, CONVERT(money, GELIRVERGIORAN) AS F37, CONVERT(money, SOZLESMEUCRET) AS F38, CONVERT(money,  EMEKLIKESENEKDEVLET) AS F39, 
            CONVERT(money, YUZDEARTISDEVLET) AS F40, CONVERT(money, SAGLIKSIGORTADEVLET) AS F41, CONVERT(money, KESENEKFARK) AS F42, CONVERT(money, SENDIKAODEME) AS F43, CONVERT(money, SENDIKAODEME) AS F44, CONVERT(money, BESKESINTI) AS F51
            TARIH, TUR, 


             * */
            ObjectArray oa = new ObjectArray();
            string kume;
            if (bordroTur == 2) kume = "(3,4,5)";
            else if (bordroTur == 1) kume = "(1,2,6)";
            else if (bordroTur == 3) kume = "(101)";
            else if (bordroTur == 4) kume = "(102)";
            else
                kume = "(1,2,3,4,6)";
            string viewAd;
            if (bordroTur == 4 || bordroTur == 3) viewAd = "UA_CALISAN_BORDRO_ISCI";
            else viewAd = "UA_CALISAN_BORDRO";
            string sql = "SELECT * FROM " + viewAd;
            sql += " WHERE TARIH = " + VeriErisim.DegerYap(bordrotarih);
            if (!string.IsNullOrWhiteSpace(kume))
                sql += " AND TUR IN " + kume;

            IDataReader dataReader1 = ve.DataReaderDondur(sql);
            if (dataReader1 == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            while (dataReader1.Read())
            {
                KisiSatir kisiSatir = new KisiSatir(new object[bordroYapi.hepsi.Length + 1]);
                for (int i = 0; i < kisiSatir.degerler.Length; i++)
                {
                    kisiSatir.degerler[i] = null;
                }

                CalisanBordroKisi ks = new CalisanBordroKisi();
                for (int i = 0; i < dataReader1.FieldCount; i++)
                {
                    string fname = dataReader1.GetName(i);
                    string ftype = dataReader1.GetDataTypeName(i);
                    try
                    {
                        if (fname == "TCKIMLIKNO")
                        {
                            kisiSatir.degerler[0] = VeriErisim.DegerAl(dataReader1, i, "");
                        }
                        else
                        {
                            int yer = bordroYapi.YerBul(fname, 0);
                            if (yer > -1)
                            {
                                DegiskenFormul df = bordroYapi.hepsi[yer];
                                object o = null;
                                if (df.tip == VariableType.Money) o = VeriErisim.DegerAl(dataReader1, i, (double)0);
                                else if (df.tip == VariableType.Integer) o = VeriErisim.DegerAl(dataReader1, i, 0);
                                else if (df.tip == VariableType.Date)
                                {
                                    TNSDateTime dt = VeriErisim.DegerAl(dataReader1, i);
                                    if (dt != null && !dt.isNull) o = dt.Oku();
                                }
                                else o = VeriErisim.DegerAl(dataReader1, i, "");
                                kisiSatir.degerler[yer + 1] = o;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        oa.sonuc.DegerAta(fname + " alanı okunamadı:" + ex.Message);
                    }
                }
                if (!oa.sonuc.islemSonuc)
                    break;
                oa.objeler.Add(kisiSatir);
            }
            dataReader1.Close();
            return oa;
        }

        private static ObjectArray VeriAl(VeriErisim ve, Surec s, int bordroTur, int bordroIslemTur, TNSDateTime bordroTarih, BordroYapi bordroYapi)
        {
            ObjectArray oa = new ObjectArray();
            int muhKod = s.muhasebe;
            string birimKod = s.birim;
            string ikramiyeMaas = "M";
            int yil = bordroTarih.Yil;
            int ay = bordroTarih.Ay;
            if (bordroIslemTur == 2)
                ikramiyeMaas = "F";
            else if (bordroIslemTur == 3)
            {
                ay = ay - 1;
                if (ay == 0)
                {
                    ay = 12;
                    yil = yil - 1;
                }
                ikramiyeMaas = "K";
            }
            /*
             *       --,[hesAGIO]
      --,[TGP]
      --,[istisnaAileYardimi]
      --,[istisnaCocukYardimi]
      --,[kesBireyselEmeklilik]
      --,[kesHayatSigortasi]
      --,[kesSakatlik]
      --,[KA]
      --,[TA]
      --,[GA]
      --,[EGA]
      --,[OHT]
      --,[EmekEGA]
      --,[EmekMaas]
      --,[EmsanMat]
      --,[odenenTGP]
      --,[hesNom2]
      --,[]
    
             * 
             * */
            //string sql = "SELECT idtSendika, SUM(hesAGI) AGI, SUM(yardimAileNet) AY, SUM(yardimCocukNet) CY, SUM(hesDamgaVergisi) DV, SUM(EmsanGSSKurum) EmsanGSS, SUM(EmsanGSSSahis) EmsanGSSSahis, SUM(EmsanKisi) EmsanKisi";
            //sql += ", SUM(EmsanKurum) EmsanKurum, SUM(EmsanYYA) EmsanYYA, SUM(hesGelirVergisiAGISonrasi) GV, SUM(kesHizmetBorcu) HizBorcu, SUM(kesIcra) IcraKes, SUM(kesIsgor) IsGor";
            //sql += ", SUM(toplamKesinti) KesintiToplami, SUM(kesKisiBorcu) KisiBorcu, SUM(kesLojman) Lojman, SUM(hesNOM) NOM, SUM(kesSendika) Sen, SUM(kesSSKiKisi) SSKiKisi, SUM(kesSSKiKurum) SSKiKurum";
            //sql += ", SUM(kesSSKKisi) SSKKisi, SUM(kesSSKKurum) SSKKurum, SUM(hesSozlesmeUcreti) SU, SUM(toplamTahakkuk) TahakkukToplami, SUM(kesYemek) Yemek";

            string sql = "SELECT personelTCKN, idtSendika, hesAGI AGI, yardimAileNet AY, yardimCocukNet CY, hesDamgaVergisi DV, EmsanGSSKurum EmsanGSS, EmsanGSSSahis EmsanGSSSahis, EmsanKisi EmsanKisi";
            sql += ", EmsanKurum EmsanKurum, EmsanYYA EmsanYYA, hesGelirVergisiAGISonrasi GV, kesHizmetBorcu HizBorcu, kesIcra IcraKes, kesIsgor IsGor";
            sql += ", toplamKesinti KesintiToplami, kesKisiBorcu KisiBorcu, kesLojman Lojman, hesNOM NOM, kesSendika Sen, kesSSKiKisi SSKiKisi, kesSSKiKurum SSKiKurum";
            sql += ", kesSSKKisi SSKKisi, kesSSKKurum SSKKurum, hesSozlesmeUcreti SU, toplamTahakkuk TahakkukToplami, kesYemek Yemek";
            sql += " FROM vOdemeEmriDetay";
            sql += " where yil=" + yil;
            sql += " and ay=" + ay;
            sql += " and ikramiyeMaas=" + VeriErisim.DegerYap(ikramiyeMaas);
            sql += " and muhasebeKodu=" + muhKod;
            sql += " and odemeEmriBirimKodu=" + VeriErisim.DegerYap(birimKod);
            sql += " and idtPersonelTipi=" + bordroTur;

            //string sql = string.Format("SELECT kod, SUM(Tutar) from vOdemeEmriDetay WHERE yil={0} AND ay={1} and muhasebeKodu={2} and odemeEmriBirimKodu={3} and idtPersonelTipi={4} and ikramiyeMaas={5} group by kod", );
            IDataReader dr = ve.DataReaderDondur(sql);
            if (dr == null)
            {
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            while (dr.Read())
            {
                KisiSatir kisiSatir = new KisiSatir(new object[bordroYapi.degiskenler.Length + 1]);
                for (int i = 1; i < kisiSatir.degerler.Length; i++)
                {
                    kisiSatir.degerler[i] = (double)0;
                }
                int sendikaNo = VeriErisim.DegerAl(dr, 1, 0);
                double tc = VeriErisim.DegerAl(dr, 0, 0.0);
                kisiSatir.degerler[0] = tc.ToString();

                for (int j = 2; j < dr.FieldCount; j++)
                {
                    double tutar = VeriErisim.DegerAl(dr, j, 0.0);
                    if (tutar > 0)
                    {
                        string kod = dr.GetName(j);
                        if (kod == "Sen" && sendikaNo > 0) kod = kod + sendikaNo.ToString();
                        int yer = bordroYapi.YerBul(kod, 0);
                        if (yer >= 0)
                        {
                            kisiSatir.degerler[yer + 1] = (double)kisiSatir.degerler[yer + 1] + tutar;
                        }
                    }
                }
                oa.objeler.Add(kisiSatir);
            }
            dr.Close();
            return oa;
        }

        /// <summary>
        /// BordroIslemTur nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroIslemTur yapısında BordroIslemTur: Bordro İşlem Tür bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc BordroIslemTurKaydet(VeriErisim veriErisim, Kullanici kullanan, BordroIslemTur form)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan))
            {
                s.DegerAta("Bu işlemi sistem yöneticileri yapabilir!");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROISLEMTUR tabHRCBORDROISLEMTUR = new TABHRCBORDROISLEMTUR(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
                s = tabHRCBORDROISLEMTUR.Sil(form);

            if (s.islemSonuc)
                s = tabHRCBORDROISLEMTUR.Ekle(form);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// BordroIslemTur nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroIslemTur yapısında BordroIslemTur: Bordro İşlem Tür bilgilerini tutan değişken</param>
        public static ObjectVAN BordroIslemTurOku(VeriErisim veriErisim, Kullanici kullanan, BordroIslemTur form)
        {
            ObjectVAN ov = new ObjectVAN();
            ObjectArray s = BordroIslemTurListele(veriErisim, kullanan, form);
            ov.sonuc = s.sonuc;
            if (s != null && s.ObjeSayisi > 0) ov.o = s[0];
            return ov;
        }

        internal static ObjectArray BordroIslemTurListele(VeriErisim veriErisim, Kullanici kullanan, BordroIslemTur form)
        {
            ObjectArray oa = new ObjectArray();

            if (form.kod > 0)
            {
                IslemTurEkle(oa, form.kod);
            }
            else
            {
                foreach (int item in Enum.GetValues(typeof(BordroIslemTur.ENUMBordroIslemTurYapi)))
                {
                    IslemTurEkle(oa, item);
                }
            }


            //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            //TABHRCBORDROISLEMTUR tabHRCBORDROISLEMTUR = new TABHRCBORDROISLEMTUR(ve);

            //IDataReader dr;
            //dr = tabHRCBORDROISLEMTUR.Oku(form);
            //if (dr == null)
            //{
            //    ve.VeriErisimKapat();
            //    oa.sonuc.DegerAta(ve.hata);
            //    return oa;
            //}
            //while (dr.Read())
            //{
            //    BordroIslemTur detay = new BordroIslemTur(VeriErisim.DegerAl(dr, (int)TABHRCBORDROISLEMTUR.ENUMAlanlar.KOD, 0), VeriErisim.DegerAl(dr, (int)TABHRCBORDROISLEMTUR.ENUMAlanlar.AD, ""));
            //    detay.entegrasyonKod = VeriErisim.DegerAl(dr, (int)TABHRCBORDROISLEMTUR.ENUMAlanlar.ENTEGRASYONKOD, "");
            //    detay.bordroIslemTurYapi = (TNS.HRC.BordroIslemTur.ENUMBordroIslemTurYapi)VeriErisim.DegerAl(dr, (int)TABHRCBORDROISLEMTUR.ENUMAlanlar.BORDROISLEMTURYAPI, 0);
            //    oa.Ekle(detay);
            //}
            //dr.Close();
            //ve.VeriErisimKapat();
            return oa;
        }

        private static void IslemTurEkle(ObjectArray oa, int kod)
        {
            BordroIslemTur.ENUMBordroIslemTurYapi detay_bordroIslemTurYapi = (BordroIslemTur.ENUMBordroIslemTurYapi)kod;
            BordroIslemTur detay = new BordroIslemTur(kod, BordroIslemTur.TurYapiAdBul(detay_bordroIslemTurYapi));
            detay.bordroIslemTurYapi = detay_bordroIslemTurYapi;
            oa.Ekle(detay);
        }

        internal static ObjectArray BordroYapiDegiskenListele(VeriErisim veriErisim, Kullanici kullanan, int islemTur)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTANIM tab = new TABHRCBORDROTANIM(ve);

            IDataReader dr = tab.ListeleAdlar(islemTur);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                StrKodStrAd detay = new StrKodStrAd();
                detay.kod = VeriErisim.DegerAl(dr, 0, "");
                detay.ad = detay.kod;
                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static ObjectArray BordroYapiDegiskenOku(VeriErisim veriErisim, Kullanici kullanan, int islemTur, string formulAd)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTANIM tab = new TABHRCBORDROTANIM(ve);

            IDataReader dr = tab.Listele(0, islemTur, formulAd, null, null, false);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }
            while (dr.Read())
            {
                DegiskenFormul detay = new DegiskenFormul();

                detay.bordroTanim = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.BORDROTANIM, 0);
                detay.bordroIslem = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.BORDROISLEM, 0);

                detay.formula = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.FORMUL, "");
                detay.giris = (ENUMBordroDegiskenTur)VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.DEGISKENTUR, 0);
                detay.tip = (VariableType)VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.DEGISKENTIP, 0);
                detay.ad = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.DEGISKENFORMULAD, "");
                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.SIRANO, "");
                detay.kisaEtiket = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.KISAETIKET, "");
                detay.uzunEtiket = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.UZUNETIKET, "");
                detay.borcHesap = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.BORCHESAP, "");
                detay.alacakHesap = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.ALACAKHESAP, "");
                detay.genislik = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.GENISLIK, 40);

                detay.noktaSay = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.NOKTASAY, 2);
                detay.ilkDeger = VeriErisim.DegerAl(dr, (int)TABHRCBORDROTANIM.ENUMAlanlar.ILKDEGER, "");

                oa.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc BordroMailiGonderEkle(VeriErisim veriErisim, Kullanici kullanan, int surecYil, string surecNo, BordroYapi bordroYapi, TNSCollection tcListe)
        {
            TNS.IKY.IIKYServis servisIKY = TNS.IKY.Arac.Tanimla();
            IKY.Personel p = new IKY.Personel();
            p.tcList = tcListe;
            ObjectArray oa = servisIKY.PersonelListele(kullanan, p, false, true);
            if (!oa.sonuc.islemSonuc)
                return oa.sonuc;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTANIM tab = new TABHRCBORDROTANIM(ve);
            ve.BeginTransaction();
            Sonuc s = null;
            string bilgiStr = "";
            foreach (IKY.Personel tc in oa.objeler)
            {
                string postaAdres = tc.iletisim.eposta2;
                if (string.IsNullOrWhiteSpace(postaAdres))
                    postaAdres = tc.EPOSTA;
                if (!string.IsNullOrWhiteSpace(postaAdres))
                {
                    s = tab.BordroMailiGonderEkle(surecYil, surecNo, bordroYapi.bordroTur, bordroYapi.bordroIslemTur, tc.TCKIMLIKNO, postaAdres, kullanan.kullaniciKodu);
                    if (!s.islemSonuc) break;
                }
                else
                {
                    if (bilgiStr != "") bilgiStr += ", ";
                    bilgiStr += string.Format("{0} adlı personelin e posta adresi bulunamadı!", tc.adSoyad);
                }
            }
            if (s == null) s = new Sonuc();

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            s.bilgiStr = bilgiStr;
            ve.VeriErisimKapat();
            return s;
        }

        internal static ObjectArray BordroMailiBekleyenListele(VeriErisim veriErisim, Kullanici kullanan)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTANIM tab = new TABHRCBORDROTANIM(ve);

            IDataReader dr = tab.BordroMailBekleyenler();
            if (dr != null)
            {
                BordroMail bm = null;
                if (dr.Read())
                {
                    bm = new BordroMail();
                    bm.surecYil = VeriErisim.DegerAl(dr, 0, 0);
                    bm.surecNo = VeriErisim.DegerAl(dr, 1, "");
                    bm.bordroTur = VeriErisim.DegerAl(dr, 2, 0);
                    bm.bordroIslemTur = VeriErisim.DegerAl(dr, 3, 0);
                    bm.tc = VeriErisim.DegerAl(dr, 4, "");
                    bm.ePosta = VeriErisim.DegerAl(dr, 5, "");
                    bm.eklemeZamani = VeriErisim.DegerAl(dr, 6);
                    bm.ad = VeriErisim.DegerAl(dr, 7, "");
                    bm.soyad = VeriErisim.DegerAl(dr, 8, "");
                    bm.isinAdi = VeriErisim.DegerAl(dr, 9, "");
                    oa.Ekle(bm);
                }
                dr.Close();
                if (bm != null)
                    tab.IslemeAlindi(bm);
            }
            else
                oa.sonuc.DegerAta(ve.hata);
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc BordroMailBirak(VeriErisim veriErisim, Kullanici kullanan, BordroMail bm)
        {
            if (!string.IsNullOrWhiteSpace(bm.sonucStr) && bm.sonucStr.Length > 191)
            {
                bm.sonucStr = bm.sonucStr.Substring(0, 190);
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCBORDROTANIM tab = new TABHRCBORDROTANIM(ve);
            ve.BeginTransaction();
            Sonuc s = tab.BordroMailiKapat(bm);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc BordroKontrolEt(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay, int muhasebe, string birim, int bordroTur, int bordroIslem)
        {
            Sonuc s = new Sonuc();
            return s;
        }

        internal static CalisanDinamikBordroDonem OncekiDonemBul(VeriErisim veriErisim, int surecYil, string surecNo)
        {
            CalisanDinamikBordroDonem cdb = null;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, "SİSTEM");
            TABHRCBORDROTANIM tab = new TABHRCBORDROTANIM(ve);
            IDataReader dr = tab.OncekiDonemBul(surecYil, surecNo);

            if (dr != null)
            {
                if (dr.Read())
                {
                    cdb = new CalisanDinamikBordroDonem(surecYil, surecNo);
                    DRDenDonemOku(cdb, dr);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();
            return cdb;
        }
    }
}
