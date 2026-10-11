using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BKD;
using TNS.UZY;

namespace TNS.BHMv2.IsKurallari
{
    public class MasrafPlani
    {
        static string VERIERISIMAD = "BaglantiSatiriBUM";
        static TNS.UZY.IUZYServis servisUZY = UZY.Arac.Tanimla();

        public static bool CokluDegerIntIceriyorMu(string yetkiTipiDegeriStr, int[] yetkiTipiDegeriDizi)
        {
            string[] yetkiTipiDegeriStrDizi = yetkiTipiDegeriStr.Split(';');
            for (int s = 0; s < yetkiTipiDegeriStrDizi.Length; s++)
            {
                int deger = OrtakFonksiyonlar.ConvertToInt(yetkiTipiDegeriStrDizi[s], -1);
                for (int i = 0; i < yetkiTipiDegeriDizi.Length; i++)
                {
                    if (deger == yetkiTipiDegeriDizi[i])
                        return true;
                }
            }
            return false;
        }

        public static bool KurumsalKodVarMi(Kullanici kullanan, string kurKod)
        {
            string birimlerStr = kullanan.YetkiTipiDegeriBul("Ödenek Gönderme Hazırlayacağı Birimler");
            string[] birimler = birimlerStr.Split(';');
            for (int i = 0; i < birimler.Length; i++)
            {
                if (!string.IsNullOrEmpty(birimler[i]))
                    if (kurKod.Replace(".", "").StartsWith(birimler[i].Replace(".", "")))
                    {
                        return true;
                    }
            }
            return false;
        }

        private static bool BirimYetkiTipindeVarMi(string ytDegeri, KurumsalYapi ky)
        {
            string[] ytDegerleri = ytDegeri.Split(';');
            for (int i = 0; i < ytDegerleri.Length; i++)
            {
                string[] ytIc = ytDegerleri[i].Split('-');
                if (ytIc.Length > 0)
                {
                    if ((ytIc[0] == "*") || (OrtakFonksiyonlar.ConvertToInt(ytIc[0], -1) == OrtakFonksiyonlar.ConvertToInt(ky.saymanlikKod, 0)))
                    {
                        if (ytIc.Length > 1)
                        {
                            if (string.IsNullOrEmpty(ytIc[1]) || ytIc[1].Equals("*") || ytIc[1].Equals(ky.birimKod))
                                return true;
                        }
                        else
                            return true;
                    }
                }
            }
            return false;
        }

        private static bool SaymanlikYoneticisiMi(int kullaniciSaymanlik, int birimSaymanlik)
        {
            if (kullaniciSaymanlik == birimSaymanlik)
                return true;
            if (kullaniciSaymanlik / 1000 == birimSaymanlik / 1000)
            {
                int merkezKod = kullaniciSaymanlik % 1000;
                if (((merkezKod == 100) || (merkezKod == 150)) && (birimSaymanlik % 1000 == 0))
                    return true;
            }
            return false;
        }

        private static bool SaymanligiGorurMu(int kullaniciSaymanlik, int birimSaymanlik)
        {
            if (kullaniciSaymanlik == birimSaymanlik)
                return true;
            if (kullaniciSaymanlik / 1000 == birimSaymanlik / 1000)
            {
                int merkezKod = kullaniciSaymanlik % 1000;
                if (((merkezKod == 100) || (merkezKod == 150)))
                    return true;
            }
            return false;
        }

        private static bool GormeYetkisiVarMi(Kullanici kullanan, ENUMMasrafPlanAsama asama, Tertip tertip, KurumsalYapi ky)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            if (asama == ENUMMasrafPlanAsama.SGB)
            {
                return false;
            }
            else if (asama == ENUMMasrafPlanAsama.MERKEZ)
            {
                if (kullanan.KullaniciTipiIceriyorMu(17))
                {
                    bool ekle = KurumsalKodVarMi(kullanan, tertip.kodlar[0]);
                    if (ekle)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                {
                    if (SaymanligiGorurMu(OrtakFonksiyonlar.ConvertToInt(kullanan.calistigiKurum, 0), OrtakFonksiyonlar.ConvertToInt(ky.saymanlikKod, -1)))
                        return true;
                }

                if (YetkiServis.OGYYetkisiVarMi(kullanan, null, tertip, ENUMOGY.Gorme))
                    return true;
                if (kullanan.KullaniciTipiIceriyorMu(18))
                {
                    string tut = kullanan.YetkiTipiDegeriBul("Ödeme Emri/MİF Saymanlık-Birim Yetkisi");
                    if (BirimYetkiTipindeVarMi(tut, ky))
                        return true;
                }
            }

            return false;
        }

        private static bool OnaylamaYetkisiVarMi(Kullanici kullanan, int yil, KurumsalYapi ky, ENUMMasrafPlanAsama asama, string kurKod)
        {
            int[] dizi = { 4 };
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            if (asama == ENUMMasrafPlanAsama.URETIM || asama == ENUMMasrafPlanAsama.TALEP)
            {

                if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                {
                    if (SaymanlikYoneticisiMi(OrtakFonksiyonlar.ConvertToInt(kullanan.calistigiKurum, 0), ky.saymanlikKod))
                        return true;
                }

                if (OrtakFonksiyonlar.ConvertToInt(ky.saymanlikKod, -1) > 0 && !string.IsNullOrEmpty(ky.birimKod))
                {
                    if (kullanan.KullaniciTipiIceriyorMu(18))
                    {
                        string tut = kullanan.YetkiTipiDegeriBul("Ödeme Emri/MİF Saymanlık-Birim Yetkisi");
                        if (BirimYetkiTipindeVarMi(tut, ky))
                        {
                            if (CokluDegerIntIceriyorMu(kullanan.YetkiTipiDegeriBul("Ödeme Emri/MİF Yetkisi"), dizi))
                                return true;
                        }
                    }
                }
            }
            else if (asama == ENUMMasrafPlanAsama.MERKEZ || asama == ENUMMasrafPlanAsama.AHP)
            {
                if (kullanan.KullaniciTipiIceriyorMu(17))
                {
                    bool ekle = KurumsalKodVarMi(kullanan, kurKod);
                    if (ekle)
                    {
                        if (CokluDegerIntIceriyorMu(kullanan.YetkiTipiDegeriBul("Ödenek Gönderme Yetkisi"), dizi))
                            return true;
                    }
                }
            }
            return false;
        }

        public static bool YazmaYetkisiVarMi(Kullanici kullanan, int yil, KurumsalYapi ky, ENUMMasrafPlanAsama asama, string kurKod)
        {
            int[] dizi = { 2, 4 };
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            //return false;
            if (asama == ENUMMasrafPlanAsama.URETIM || asama == ENUMMasrafPlanAsama.TALEP)
            {
                if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                {
                    if (SaymanlikYoneticisiMi(OrtakFonksiyonlar.ConvertToInt(kullanan.calistigiKurum, 0), ky.saymanlikKod))
                        return true;
                }
                if (kullanan.KullaniciTipiIceriyorMu(18))
                {
                    string tut = kullanan.YetkiTipiDegeriBul("Ödeme Emri/MİF Saymanlık-Birim Yetkisi");
                    if (BirimYetkiTipindeVarMi(tut, ky))
                    {
                        if (CokluDegerIntIceriyorMu(kullanan.YetkiTipiDegeriBul("Ödeme Emri/MİF Yetkisi"), dizi))
                            return true;
                    }
                }
            }
            else if (asama == ENUMMasrafPlanAsama.MERKEZ || asama == ENUMMasrafPlanAsama.AHP)
            {
                if (kullanan.KullaniciTipiIceriyorMu(17))
                {
                    bool ekle = KurumsalKodVarMi(kullanan, kurKod);
                    if (ekle)
                    {
                        if (CokluDegerIntIceriyorMu(kullanan.YetkiTipiDegeriBul("Ödenek Gönderme Yetkisi"), dizi))
                            return true;
                    }
                }
            }
            return false;
        }

        public static BirimMasrafPlaniGenel MasrafPlaniBulTavanYeni(VeriErisim veriErisim, Kullanici kullanan, ENUMMasrafPlanAsama asama, Tertip kistasTertip, UcgenKodBilgileri ukb, string saymanlikBirim)
        {
            KurumsalYapi ky = BHMv2.IsKurallari.BHMv2KurumsalYapi.KurumsalYapiDondur(veriErisim, kistasTertip.kodlar[5]);
            if (ky == null)
                throw new Exception("Kurumsal Yapı Çözümlenemedi");
            if (string.IsNullOrEmpty(kistasTertip.kodlar[0]) && !string.IsNullOrEmpty(ky.kurKod))
                kistasTertip.kodlar[0] = ky.kurKod;

            if (!GormeYetkisiVarMi(kullanan, asama, kistasTertip, ky))
                throw new Exception("Bu birimi görme yetkiniz yok!");

            BirimMasrafPlaniGenel bmpg = new BirimMasrafPlaniGenel();
            bmpg.bmpTertip = new BirimMasrafPlaniTertip();

            EkDegerParam[] ekBilgiler = new EkDegerParam[13];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABMPTavan, 0, 0, "1");
            for (int i = 1; i <= 12; i++)
            {
                ekBilgiler[i] = new EkDegerParam(ENUMTabloKod.TABMasrafPlani, 0, 0, ((int)asama).ToString() + ";" + (i).ToString());
            }
            OdenekBilgi[] ob = BHMv2.IsKurallari.UcgenServisi.ListeTertipOl(null, kullanan, kistasTertip, ukb, ekBilgiler, true, false, true, -1);
            System.Collections.Generic.List<MPTertipBilgi> l = new System.Collections.Generic.List<MPTertipBilgi>();
            foreach (var item in ob)
            {
                l.Add(new MPTertipBilgi(item));
            }

            bmpg.bmpTertip.tertipBilgiListe = l.ToArray();
            return bmpg;
        }

        public static BirimMasrafPlaniGenel MasrafPlaniBulTavan(VeriErisim veriErisim, Kullanici kullanan, ENUMMasrafPlanAsama asama, Tertip kistasTertip, UcgenKodBilgileri ukb, string saymanlikBirim)
        {
            KurumsalYapi ky = BHMv2.IsKurallari.BHMv2KurumsalYapi.KurumsalYapiDondur(veriErisim, kistasTertip.kodlar[5]);
            if (ky == null)
                throw new Exception("Kurumsal Yapı Çözümlenemedi");
            if (string.IsNullOrEmpty(kistasTertip.kodlar[0]) && !string.IsNullOrEmpty(ky.kurKod))
                kistasTertip.kodlar[0] = ky.kurKod;

            if (!GormeYetkisiVarMi(kullanan, asama, kistasTertip, ky))
                throw new Exception("Bu birimi görme yetkiniz yok!");

            BirimMasrafPlaniGenel bmpg = new BirimMasrafPlaniGenel();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            bmpg.bmpTertip = MPTertipOkuTavan(ve, kullanan, asama, kistasTertip, ky, ukb, saymanlikBirim);
            ve.VeriErisimKapat();
            return bmpg;
        }

        private static BirimMasrafPlaniTertip MPTertipOkuTavan(VeriErisim veriErisim, Kullanici kullanan, ENUMMasrafPlanAsama asama, Tertip kistasTertip, KurumsalYapi kurumsalYapi, UcgenKodBilgileri ukb, string saymanlikBirim)
        {
            BirimMasrafPlaniTertip mpBilgi = new BirimMasrafPlaniTertip();
            ArrayList al = new ArrayList();

            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(veriErisim);
            TertipOdenek[] tolar = MasrafPlaniTavan.MPListeleRapor(null, kullanan, kistasTertip, ukb, false, saymanlikBirim);
            for (int i = 0; i < tolar.Length; i++)
            {
                MPTertipBilgi tb = new MPTertipBilgi(tolar[i]);
                tb.genelOdenekBilgi = new GenelOdenekBilgi();

                IDataReader dr = tab.ListeleAyOdenek(asama, tb.tertipOdenek.tertip, -1, -1, false);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        int ay = VeriErisim.DegerAl(dr, 0, 0);
                        double talep = VeriErisim.DegerAl(dr, 1, 0.0);
                        tb.genelOdenekBilgi.AyDegerAta(ay - 1, talep);
                    }
                    dr.Close();
                }
                al.Add(tb);
            }
            mpBilgi.tertipBilgiListe = (MPTertipBilgi[])al.ToArray(typeof(MPTertipBilgi));
            return mpBilgi;
        }

        public static BirimMasrafPlaniGenel MasrafPlaniBul(VeriErisim veriErisim, Kullanici kullanan, ENUMMasrafPlanAsama asama, Tertip kistasTertip, UcgenKodBilgileri ukb, EkDegerParam[] ekBilgiler)
        {
            KurumsalYapi ky = BHMv2.IsKurallari.BHMv2KurumsalYapi.KurumsalYapiDondur(veriErisim, kistasTertip.kodlar[5]);
            if (ky == null)
                throw new Exception("Kurumsal Yapı Çözümlenemedi");
            if (string.IsNullOrEmpty(kistasTertip.kodlar[0]) && !string.IsNullOrEmpty(ky.kurKod))
                kistasTertip.kodlar[0] = ky.kurKod;

            if (!GormeYetkisiVarMi(kullanan, asama, kistasTertip, ky))
                throw new Exception("Bu birimi görme yetkiniz yok!");

            BirimMasrafPlaniGenel bmpg = new BirimMasrafPlaniGenel();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            bmpg.bmpTertip = MPTertipOku(ve, kullanan, asama, kistasTertip, ky, ukb, ekBilgiler);
            ve.VeriErisimKapat();
            return bmpg;
        }

        private static BirimMasrafPlaniTertip MPTertipOku(VeriErisim veriErisim, Kullanici kullanan, ENUMMasrafPlanAsama asama, Tertip kistasTertip, KurumsalYapi kurumsalYapi, UcgenKodBilgileri ukb, EkDegerParam[] ekBilgiler)
        {
            BirimMasrafPlaniTertip mpBilgi = new BirimMasrafPlaniTertip();
            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

            int[] tertipKodYer;
            int odenekYer;
            string alanAd = MasrafPlaniTavan.AlanIsimleriBul(kullanan, kistasTertip, ukb, out tertipKodYer, out odenekYer, true);
            alanAd += ",AY";
            if (odenekYer == 0)
                throw new Exception("En az bir alan belirtmelisiniz!");
            int ekBilgiSay = 0;
            if (ekBilgiler != null) ekBilgiSay = ekBilgiler.Length;
            ArrayList al = new ArrayList();
            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(veriErisim);

            IDataReader drDis = tab.ListeleRapor(alanAd, kistasTertip, asama, 0, 0);

            if (drDis == null)
            {
                veriErisim.VeriErisimKapat();
                throw new Exception(veriErisim.hata);
            }

            MPTertipBilgi mpTB = null;
            string oncTertip = "";
            Hashtable[] ht = MasrafPlaniTavan.HashtableDiziOl(kistasTertip.KodNoSay());
            while (drDis.Read())
            {
                Tertip okuTertip = new Tertip(kistasTertip.yil, ENUMTabloKod.TABMasrafPlani);
                for (int i = 0; i < odenekYer; i++)
                {
                    string kod = VeriErisim.DegerAl(drDis, i, "");
                    okuTertip.TertibeKodEkle(tertipKodYer[i], kod, "");
                }
                string simdTertip = okuTertip.TertipStrOl();
                if (oncTertip != simdTertip)
                {
                    TertipOdenek to = new TertipOdenek(ekBilgiSay);
                    Tertip uzaySorguTertip = new Tertip(kistasTertip.yil, ENUMTabloKod.TABMasrafPlani);
                    to.tertip = new Tertip(kistasTertip.yil, ENUMTabloKod.TABMasrafPlani);
                    to.tertip.TertipAta(okuTertip);
                    to.tertip.Noktala();
                    mpTB = new MPTertipBilgi(to);
                    mpTB.genelOdenekBilgi = new GenelOdenekBilgi();

                    for (int i = 0; i < uzaySorguTertip.KodNoSay(); i++)
                    {
                        uzaySorguTertip.kodlar[i] = to.tertip.kodlar[i];
                        if (string.IsNullOrEmpty(uzaySorguTertip.kodlar[i]))
                        {
                            try
                            {
                                uzaySorguTertip.kodlar[i] = kistasTertip.kodlar[i];
                            }
                            catch { }
                        }
                    }

                    MasrafPlaniTavan.TertipAdiBul(uzyServis, to.tertip, ht);
                    for (int i = 0; i < ekBilgiSay; i++)
                    {
                        to.odenek[i] = ekBilgiler[i].EkDegerHesaplaDbl(uzyServis, kullanan, kistasTertip.yil, uzaySorguTertip.TertipStrOl());
                    }
                    al.Add(mpTB);
                    oncTertip = simdTertip;
                }
                int ay = VeriErisim.DegerAl(drDis, odenekYer, 0);
                double talep = VeriErisim.DegerAl(drDis, odenekYer + 1, 0.0);
                mpTB.genelOdenekBilgi.AyDegerAta(ay - 1, talep);
            }
            drDis.Close();
            mpBilgi.tertipBilgiListe = (MPTertipBilgi[])al.ToArray(typeof(MPTertipBilgi));
            return mpBilgi;
        }

        public static double[] MPMasrafPlaniOku(VeriErisim veriErisim, Kullanici kullanan, ENUMMasrafPlanAsama asama, Tertip kistasTertip, int islemNo, int ustIslemNo, bool onaysiz)
        {
            double[] al = new double[12];
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(ve);
            IDataReader dr = tab.ListeleAyOdenek(asama, kistasTertip, islemNo, ustIslemNo, onaysiz);
            if (dr != null)
            {
                while (dr.Read())
                {
                    int ay = VeriErisim.DegerAl(dr, 0, 0);
                    al[ay - 1] = VeriErisim.DegerAl(dr, 1, 0.0);
                }
                dr.Close();
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(veriErisim.hata);
            }
            ve.VeriErisimKapat();
            return al;
        }

        private static double[] AHPOku(VeriErisim veriErisim, Kullanici kullanan, int asama, Tertip kistasTertip)
        {
            double[] al = new double[12];

            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(veriErisim);
            IDataReader dr = tab.ListeleAHPOdenek(asama, kistasTertip, 0);
            if (dr != null)
            {
                while (dr.Read())
                {
                    int ay = VeriErisim.DegerAl(dr, 0, 0);
                    al[ay - 1] = VeriErisim.DegerAl(dr, 1, 0.0);
                }
                dr.Close();
            }
            else
                throw new Exception(veriErisim.hata);
            return al;
        }

        public static TertipOdenek[] BirimMasrafPlaniOzetListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama asama, string kurKod, string ekoKod, int raporTur)
        {
            KurumsalYapi kYon = BHMv2.IsKurallari.BHMv2KurumsalYapi.KurumsalYapiDondur(veriErisim, kurumsalYapiKod);
            if (kYon == null)
                throw new Exception("Kurumsal Yapı Çözümlenemedi");
            Tertip t = new Tertip(yil, ENUMTabloKod.TABTasraOdenek, kurKod);
            if (!GormeYetkisiVarMi(kullanan, asama, t, kYon))
                throw new Exception("Birimi görme yetkiniz yok!");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            KurumsalYapi ky = new KurumsalYapi();
            ky.kod = kurumsalYapiKod;
            KurumsalYapi[] kyler = BHMv2KurumsalYapi.Listele(ve, kullanan, ky, false, false, true);
            ArrayList al = new ArrayList();
            int boy;
            if (raporTur == 1)
                boy = 9;
            else
                boy = 12;
            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(ve);
            foreach (KurumsalYapi k in kyler)
            {
                TertipOdenek to = new TertipOdenek(boy);
                to.tertip = new Tertip(yil, ENUMTabloKod.TABTasraOdenek);
                to.tertip.kodlar[5] = k.kod;
                to.tertip.adlar[5] = k.ad;
                IDataReader dr;
                if (raporTur == 1)
                    dr = tab.EkonomikListele(yil, k.kod, asama, kurKod, ekoKod, 0);
                else
                {
                    Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABMasrafPlani, kurKod, "", "", ekoKod, "", k.kod);
                    dr = tab.ListeleAyOdenek(asama, kistasTertip, -1, -1, false);
                }
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        int yer;
                        if (raporTur == 1)
                        {
                            yer = OrtakFonksiyonlar.ConvertToInt(VeriErisim.DegerAl(dr, 0, ""), 1);
                        }
                        else
                            yer = VeriErisim.DegerAl(dr, 0, 0);

                        to.odenek[yer - 1] = VeriErisim.DegerAl(dr, 1, 0.0);
                    }
                    dr.Close();
                    al.Add(to);
                }
                else
                {
                    ve.VeriErisimKapat();
                    throw new Exception(ve.hata);
                }
            }
            ve.VeriErisimKapat();

            return (TertipOdenek[])al.ToArray(typeof(TertipOdenek));
        }

        public static int OnayDurumBul(VeriErisim ve, int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama asama, string ekoKod2)
        {
            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(ve);
            return tab.DurumBul(yil, kurumsalYapiKod, asama, ekoKod2);
        }

        public static int MasrafPlaniGenelOnayDurumBul(VeriErisim veriErisim, int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama asama)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, null);
            int tut = OnayDurumBul(ve, yil, kurumsalYapiKod, asama, "000");
            ve.VeriErisimKapat();
            return tut;
        }

        public static Sonuc MasrafPlaniGenelOnayla(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama asama, int ay)
        {

            Sonuc s = new Sonuc(true, "", "");
            if (asama == ENUMMasrafPlanAsama.SONDURUMONAYLI)
            {
                s.DegerAta("Son durum aşaması onaylanamaz!");
                return s;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(ve);

            KurumsalYapi kY = BHMv2.IsKurallari.BHMv2KurumsalYapi.KurumsalYapiDondur(ve, kurumsalYapiKod);

            if (!OnaylamaYetkisiVarMi(kullanan, yil, kY, asama, ""))
            {
                s.DegerAta("Bu Birimi onaylama yetkiniz bulunmamaktadır!");
            }

            //AHP'si Ödenekten büyük var Mı?
            bool eko23 = TNS.BKD.Arac.Eko2_3Bul(kullanan);
            TABMPGMASRAFPLANI tabMP = new TABMPGMASRAFPLANI(ve);

            if (s.islemSonuc && asama == ENUMMasrafPlanAsama.AHP)
            {
                OdenektenBuyukAHPler(tabMP, yil, TNS.DEG.Arac.VarsayilanKurumBul(), 3, eko23, s);

                if (s.islemSonuc)
                {
                    AHPFarkliMPler(tabMP, yil, TNS.DEG.Arac.VarsayilanKurumBul(), 3, (int)asama, ay, s);
                }

                if (ay == 12)
                {

                }
                //if (s.islemSonuc)
                //{
                //    if (asama == ENUMMasrafPlanAsama.AHP && string.IsNullOrEmpty(kurumsalYapiKod))
                //    {
                //        tertipOdenekler = DagitilmayanlariHesapla(ve, kullanan, yil, asama, s);
                //    }
                //}
                //if (tertipOdenekler != null)
                //    DongudeTertipEkle(tabMP, asama, tertipOdenekler, s, 5);
            }

            if (s.islemSonuc)
            {
                int durum = OnayDurumBul(ve, yil, kurumsalYapiKod, asama, "000");

                if (durum == 5)
                {
                    s.DegerAta("Daha önce onaylandığı için tekrar onaylayamazsınız!");
                }
            }

            //TertipOdenek[] tertipOdenekler = null;

            //s.DegerAta("Hazır değil");
            if (s.islemSonuc)
            {
                ve.BeginTransaction();
                tabMP = new TABMPGMASRAFPLANI(ve);
                if (s.islemSonuc)
                    s = tab.SilDurum(yil, kurumsalYapiKod, asama, "000");
                if (s.islemSonuc && string.IsNullOrEmpty(kurumsalYapiKod))
                {
                    s = tab.TertipDurumDegistir(yil, asama, 5, 0, true);
                    if (s.islemSonuc)
                        s = tab.TertipDurumDegistir(yil, asama, 7, 0, false);
                }
                if (s.islemSonuc)
                    s = tab.EkleDurum(yil, kurumsalYapiKod, asama, "000", 5, kullanan.kullaniciKodu);
                if (s.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();
            return s;
        }

        private static void AHPFarkliMPler(TABMPGMASRAFPLANI tabMP, int yil, string kurumKod, int ahpAsama, int mpAsama, int ay, Sonuc s)
        {
            IDataReader dr = tabMP.AHPFarkliMPler(yil, ay, kurumKod, ahpAsama, mpAsama);

            if (dr != null)
            {
                while (dr.Read())
                {
                    s.DegerAta($@"{VeriErisim.DegerAl(dr, 0, "")}-{VeriErisim.DegerAl(dr, 1, "")}-{VeriErisim.DegerAl(dr, 2, "")}-{VeriErisim.DegerAl(dr, 3, "")}-{VeriErisim.DegerAl(dr, 4, "")} tertibinde {VeriErisim.DegerAl(dr, 5, 0)}. Ay AHP'si olan ({VeriErisim.DegerAl(dr, 6, 0.0)}) TL, ile MP'si olan ({VeriErisim.DegerAl(dr, 7, 0.0)})'den farklı!<br />");
                }
                dr.Close();
            }
            else
                s.DegerAta(tabMP.Hata());
        }

        private static void OdenektenBuyukAHPler(TABMPGMASRAFPLANI tabMP, int yil, string kurumKod, int ahpAsama, bool eko23, Sonuc s)
        {
            IBHMv2Servis servisBHM = BHMv2.Arac.Tanimla();
            string odenekAsama = ProgramOnayForm.OnayliAsamaBul(null, yil);
            if (string.IsNullOrWhiteSpace(odenekAsama)) odenekAsama = "06.00";


            IDataReader dr = tabMP.OdenektenBuyukAHPBul(yil, kurumKod, ahpAsama, eko23, odenekAsama);

            if (dr != null)
            {
                while (dr.Read())
                {
                    s.DegerAta($"{VeriErisim.DegerAl(dr, 0, "")}-{VeriErisim.DegerAl(dr, 1, "")}-{VeriErisim.DegerAl(dr, 2, "")}-{VeriErisim.DegerAl(dr, 3, "")}-{VeriErisim.DegerAl(dr, 4, "")} tertibinde AHP ({VeriErisim.DegerAl(dr, 5, 0.0)}), Ödenekten ({VeriErisim.DegerAl(dr, 6, 0.0)}) fazla!<br />");
                }
                dr.Close();
            }
            else
                s.DegerAta(tabMP.Hata());
        }

        public static TertipOdenek[] DagitilmayanlariHesapla(VeriErisim veriErisim, Kullanici kullanan, int yil, ENUMMasrafPlanAsama asama, Sonuc sonuc)
        {
            try
            {
                /*
                INSERT INTO MPGMASRAFPLANITERTIP
                SELECT AHP.YIL
            ,5 AS ASAMA
            ,AHP.KURKOD
            ,AHP.FONKOD
            ,AHP.FINKOD
            ,AHP.EKOKOD
            ,AHP.PEBKOD
            ,'90000000000'
            ,AHP.AY
            ,SUM(AHP.ODENEK) - ISNULL(SUM(TAVAN.ODENEK),0) AS ODENEK
            ,0 AS ODENEKGONDERME
            ,1 AS DURUM
            ,0 AS REVIZENO, 1 AS GRUP, 1 AS TERTIPSIRA
			FROM ButceIslemleri.dbo.ISLAPGIDER AS AHP
			LEFT OUTER JOIN 
              MPGMASRAFPLANITERTIP AS TAVAN
			 ON  TAVAN.YIL=AHP.YIL
             AND TAVAN.ASAMA = 5
             AND TAVAN.KURKOD = AHP.KURKOD
             AND TAVAN.FONKOD = AHP.FONKOD
             AND TAVAN.FINKOD = AHP.FINKOD
             AND TAVAN.EKOKOD = AHP.EKOKOD
             AND TAVAN.PEBKOD = AHP.PEBKOD
             AND TAVAN.AY=AHP.AY
			 AND TAVAN.REVIZENO=0 
			 where AHP.YIL=2022 AND AHP.ISLEM='0101' AND AHP.SIRANO=3  AND AHP.EKOKOD LIKE '0303%'
group by AHP.YIL, AHP.KURKOD, AHP.FONKOD, AHP.FINKOD, AHP.EKOKOD, AHP.PEBKOD, AHP.AY
HAVING SUM(AHP.ODENEK) - ISNULL(SUM(TAVAN.ODENEK), 0)>0
                                */

                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
                Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

                UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
                bool eko2 = TNS.MUH.Arac.OdenekGondermeEko2liMi(kistasTertip.yil);
                for (int i = 0; i < kistasTertip.KodNoSay(); i++)
                {
                    int tut = kistasTertip.KodNoKirilimSay(i);
                    if (i == 3 && eko2)
                        tut = 2;
                    kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(i), tut));
                }

                string[] tertipler = TertipTanim.Listele(veriErisim, kullanan, kistasTertip, kodBilgileri);

                if (!sonuc.islemSonuc) return null;
                TertipOdenek[] tertipOdenek = new TertipOdenek[tertipler.Length];
                for (int i = 0; i < tertipler.Length; i++)
                {
                    tertipOdenek[i] = new TertipOdenek(12);
                    tertipOdenek[i].tertip = new Tertip(kistasTertip.yil, ENUMTabloKod.TABMasrafPlani);
                    tertipOdenek[i].tertip.TertipAta(tertipler[i]);
                    double[] mpAyDegerler = MPMasrafPlaniOku(ve, kullanan, asama, tertipOdenek[i].tertip, -1, -1, true);
                    double[] ahpAyDegerler = AHPOku(ve, kullanan, 3, tertipOdenek[i].tertip);
                    tertipOdenek[i].tertip.kodlar[5] = "9.00.000.00000";
                    for (int ay = 0; ay < 12; ay++)
                    {
                        tertipOdenek[i].odenek[ay] = Math.Round(ahpAyDegerler[ay] - mpAyDegerler[ay], 4);
                        if (tertipOdenek[i].odenek[ay] < 0)
                        {
                            sonuc.DegerAta(tertipler[i] + " tertibinde " + (ay + 1).ToString() + ". ay masraf planı ödeneği (" + mpAyDegerler[ay].ToString("#,##0.00") + ") AHP'den(" + ahpAyDegerler[ay].ToString("#,##0.00") + ") büyük!");
                        }
                    }
                }
                return tertipOdenek;
            }
            catch (Exception ex)
            {
                sonuc.DegerAta(ex.Message);
            }
            return null;
        }

        public static Sonuc MasrafPlaniGenelOnayKaldir(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalYapiKod, string[] onayKalkacakBirimler, ENUMMasrafPlanAsama asama)
        {
            Sonuc s = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(ve);

            KurumsalYapi kY = BHMv2.IsKurallari.BHMv2KurumsalYapi.KurumsalYapiDondur(ve, kurumsalYapiKod);

            if (!OnaylamaYetkisiVarMi(kullanan, yil, kY, asama, ""))
            {
                s.DegerAta("Bu Birimde onaylama/onay kaldırma yetkiniz bulunmamaktadır!");
            }

            if (s.islemSonuc)
            {
                ve.BeginTransaction();
                if (onayKalkacakBirimler == null)
                {
                    onayKalkacakBirimler = new string[] { kurumsalYapiKod };
                }
                for (int i = 0; i < onayKalkacakBirimler.Length && s.islemSonuc; i++)
                {
                    s = tab.SilDurum(yil, onayKalkacakBirimler[i], asama, "000");
                    if (s.islemSonuc)
                        s = tab.EkleDurum(yil, onayKalkacakBirimler[i], asama, "000", 2, kullanan.kullaniciKodu);
                }
                if (s.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();
            return s;
        }

        public static Sonuc MasrafPlaniGenelTertipKaydet(VeriErisim veriErisim, Kullanici kullanan, ENUMMasrafPlanAsama asama, TertipOdenek[] tertipOdenekler)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            Sonuc s = new Sonuc(true, "", "");
            if (asama == ENUMMasrafPlanAsama.SONDURUMONAYLI)
                s.DegerAta("Son durum aşaması sadece veri görüntülemek içindir. Kayıt yapılamaz!");
            if (tertipOdenekler.Length == 0)
                s.DegerAta("Kaydedilecek tertip bulunamadı!");
            if (s.islemSonuc)
            {
                TertipOdenek to = tertipOdenekler[0];
                int eko2uz = to.tertip.KodUzunlukBul(3, 2, true);
                int eko3uz = to.tertip.KodUzunlukBul(3, 3, true);
                bool eko23 = TNS.BKD.Arac.Eko2_3Bul(kullanan);
                for (int i = 0; i < tertipOdenekler.Length; i++)
                {
                    to = tertipOdenekler[i];
                    string kurumsalYapiKod = to.tertip.kodlar[5];
                    KurumsalYapi kY = BHMv2.IsKurallari.BHMv2KurumsalYapi.KurumsalYapiDondur(ve, kurumsalYapiKod);
                    if (!YazmaYetkisiVarMi(kullanan, to.tertip.yil, kY, asama, kY.kurKod))
                    {
                        s.DegerAta("Bu birime ait bilgileri değiştirme yetkiniz bulunmamaktadır!");
                    }
                    int durum = OnayDurumBul(ve, to.tertip.yil, kurumsalYapiKod, asama, "000");
                    if (durum == 5)
                    {
                        s.DegerAta("Verileriniz onaylı olduğu için değişiklik yapamazsınız!");
                    }
                    if (!to.tertip.TamMi(3, eko2uz, eko23, eko3uz))
                        s.DegerAta("Hatalı tertip:" + to.tertip.TertipStrOl());
                    for (int ayIc = 0; ayIc < to.odenek.Length; ayIc++)
                    {
                        if (to.odenek[ayIc] < 0)
                            s.DegerAta(to.tertip.TertipStrOl() + " tertibinde ödenek sıfırdan küçük girilmiş!");
                    }
                }
            }

            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();
            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(ve);
            DongudeTertipEkle(tab, asama, tertipOdenekler, s, 6, 0);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        private static void DongudeTertipEkle(TABMPGMASRAFPLANI tab, ENUMMasrafPlanAsama asama, TertipOdenek[] tertipOdenekler, Sonuc s, int durum, int ioiIsKod)
        {
            for (int i = 0; i < tertipOdenekler.Length && s.islemSonuc; i++)
            {
                TertipOdenek to = tertipOdenekler[i];
                s = tab.SilTertip(asama, to.tertip);
                for (int ayIc = 0; ayIc < to.odenek.Length && s.islemSonuc; ayIc++)
                {
                    if (to.odenek[ayIc] > 0)
                        s = tab.EkleTertip(asama, to.tertip, ayIc + 1, to.odenek[ayIc], 0, durum, 1, ioiIsKod, 0);
                }
            }
        }

        public static TertipOdenek[] MPBirimTertipListele(VeriErisim veriErisim, Tertip kistasTertip, int saymanlik, string birim)
        {
            return MPBirimTertipListele(veriErisim, kistasTertip, BHMv2.IsKurallari.BHMv2KurumsalYapi.Listele(veriErisim, saymanlik.ToString(), birim, ""));
        }

        public static TertipOdenek[] MPBirimTertipListele(VeriErisim veriErisim, Tertip kistasTertip, string[] kurumsalYapiKodlar)
        {
            ArrayList al = new ArrayList();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, null);

            if (OnayDurumBul(ve, kistasTertip.yil, "", ENUMMasrafPlanAsama.AHP, "000") == 5)
            {
                TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(ve);

                IDataReader dr = tab.Listele(kistasTertip, kurumsalYapiKodlar);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        TertipOdenek to = new TertipOdenek(1);
                        to.tertip = new Tertip(kistasTertip.yil, ENUMTabloKod.TABPEBOdenek);
                        to.tertip.TertipAta(
                            VeriErisim.DegerAl(dr, 0, ""),
                            VeriErisim.DegerAl(dr, 1, ""),
                            VeriErisim.DegerAl(dr, 2, ""),
                            VeriErisim.DegerAl(dr, 3, ""),
                            VeriErisim.DegerAl(dr, 4, "")
                        );

                        to.tertip.Noktala();
                        to.tertip.AdlariBul(servisUZY);
                        to.odenek[0] = VeriErisim.DegerAl(dr, 5, 0.0);
                        al.Add(to);
                    }
                    dr.Close();
                }
            }
            else
            {
                //EkDegerParam a = new EkDegerParam(ENUMTabloKod.TABOdenekGonderme, 0, 0, "");
                Tertip[] t = TNS.BHMv2.IsKurallari.TertipTanim.Listele(null, null, kistasTertip, 1, null);
                for (int i = 0; i < t.Length; i++)
                {
                    TertipOdenek to = new TertipOdenek(1);
                    to.tertip = t[i];
                    to.odenek[0] = 0;
                    al.Add(to);
                }
            }
            ve.VeriErisimKapat();
            return (TertipOdenek[])al.ToArray(typeof(TertipOdenek));
        }

        public static TertipOdenek[] MPTertipListeleAy(VeriErisim veriErisim, Kullanici kullanan, Tertip kistas, string birim, bool gerceklesme, bool ikili)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(ve);
            VeriErisim veBUM = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", kullanan.KimlikBul());

            ArrayList al = new ArrayList();
            IDataReader dr = tab.ListeleTertipAy(kistas.yil, kistas.kodlar[0], kistas.kodlar[1], kistas.kodlar[2], kistas.kodlar[3], kistas.kodlar[4], birim, "5", -1);

            if (dr != null)
            {
                string oncekiTam = "";
                string oncekiEko2 = "";
                TertipOdenek to = null;
                while (dr.Read())
                {
                    string tertipStr = VeriErisim.DegerAl(dr, 0, "") + "-" + VeriErisim.DegerAl(dr, 1, "") + "-" + VeriErisim.DegerAl(dr, 2, "") + "-" + VeriErisim.DegerAl(dr, 3, "") + "-" + VeriErisim.DegerAl(dr, 4, "");
                    if (tertipStr != oncekiTam)
                    {
                        oncekiTam = tertipStr;
                        if (tertipStr.Substring(0, 21) != oncekiEko2)
                        {
                            oncekiEko2 = tertipStr.Substring(0, 21);
                            TertipOdenek to2 = new TertipOdenek(14);
                            to2.tertip = new Tertip(kistas.yil, ENUMTabloKod.TABButce);
                            to2.tertip.TertipAta(oncekiEko2);
                            to2.tertip.Noktala();
                            for (int ay = 0; ay < 12; ay++)
                            {
                                to2.odenek[ay] = 0; // UzayHesapla(veBUM, kullanan, "APGIDER", TNS.UZY.Arac.UzayKosulOl(kistas.yil.ToString(), "", "", "", oncekiEko2, (ay + 1).ToString(), "0/6"), "");
                            }
                            //to2.odenek[12] = UzayHesapla(veBUM, kullanan, "MUHGONDERMEBELGESITENKIS", TNS.UZY.Arac.UzayKosulOl(kistas.yil.ToString(), "", "", "", "", "", "", "5", oncekiEko2), "");
                            //to2.odenek[13] = UzayHesapla(veBUM, kullanan, "MUHODEMEEMRIMIF", TNS.UZY.Arac.UzayKosulOl(kistas.yil.ToString(), "", "", "830", oncekiEko2, "0/6"), "");
                            if (ikili)
                                al.Add(to2);
                        }
                        to = new TertipOdenek(14);
                        to.tertip = new Tertip(kistas.yil, ENUMTabloKod.TABPEBOdenek);
                        to.tertip.TertipAta(oncekiTam);
                        to.tertip.Noktala();
                        al.Add(to);
                        if (gerceklesme)
                        {
                            to.odenek[12] = UzayHesapla(veBUM, kullanan, "MUHGONDERMEBELGESITENKIS", TNS.UZY.Arac.UzayKosulOl(kistas.yil.ToString(), "", "", "", "", "", "", "5", oncekiTam), "");
                            to.odenek[13] = UzayHesapla(veBUM, kullanan, "MUHODEMEEMRIMIF", TNS.UZY.Arac.UzayKosulOl(kistas.yil.ToString(), "", "", "830", to.tertip.kodlar[0], to.tertip.kodlar[1], to.tertip.kodlar[2], to.tertip.kodlar[3], "0/6", to.tertip.kodlar[4]), "");
                        }
                        else
                        {
                            to.odenek[12] = 0;
                            to.odenek[13] = 0;
                        }
                    }
                    if (to != null)
                    {
                        int ayNo = VeriErisim.DegerAl(dr, 5, 0) - 1;
                        to.odenek[ayNo] = VeriErisim.DegerAl(dr, 6, 0.0);
                    }
                }
                dr.Close();
                veBUM.VeriErisimKapat();
                ve.VeriErisimKapat();
                return (TertipOdenek[])al.ToArray(typeof(TertipOdenek));
            }
            else
            {
                veBUM.VeriErisimKapat();
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
        }

        private static double UzayHesapla(VeriErisim veriErisim, Kullanici kullanan, string uzayAd, string uzayKosul, string alanAd)
        {
            string sql = servisUZY.UzayKosulStr(kullanan, uzayAd, uzayKosul, false, alanAd);
            double sonuc = veriErisim.TekKolonSQLDbl(sql);
            if (!string.IsNullOrEmpty(veriErisim.hata))
                throw new Exception(veriErisim.hata + "(" + sql + ")");
            return sonuc;
        }

        public static MPBilgi[] BirimMasrafPlaniBul(VeriErisim veriErisim, Kullanici kullanan, int yil, ENUMMasrafPlanAsama asama, string kurumsalYapiKod, string ekoKod)
        {
            KurumsalYapi ky = BHMv2.IsKurallari.BHMv2KurumsalYapi.KurumsalYapiDondur(veriErisim, kurumsalYapiKod);
            return BirimMasrafPlaniBulIc(veriErisim, kullanan, yil, asama, ky, ekoKod);
        }

        private static MPBilgi[] BirimMasrafPlaniBulIc(VeriErisim veriErisim, Kullanici kullanan, int yil, ENUMMasrafPlanAsama asama, KurumsalYapi ky, string ekoKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            ArrayList al = new ArrayList();
            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(ve);
            IBKDServis bkdServis = TNS.BKD.Arac.Tanimla();
            ABSKodlarVAN akv = bkdServis.KodListele(yil, "4", ekoKod);
            string SENARYOKOD = ProgramOnayForm.OnayliAsamaBul(null, yil);

            string kurumKod = TNS.DEG.Arac.VarsayilanKurumBul();
            foreach (ABSKod ak in akv.absKodlar)
            {
                MPBilgi mpUB = new MPBilgi();
                mpUB.mpEkonomik = ak;
                mpUB.odenek = Arac.OdenekBul(kullanan, yil, SENARYOKOD, kurumKod, "", "", mpUB.mpEkonomik.kod);
                mpUB.harcama = Arac.HarcamaBul(kullanan, yil - 1, ky.kod, ky.saymanlikKod, ky.birimKod, ky.kurKod, "", "", mpUB.mpEkonomik.kod);
                mpUB.gonderme = Arac.OdenekGondermeBul(kullanan, yil - 1, ky.kod, ky.saymanlikKod, ky.birimKod, ky.kurKod, "", "", mpUB.mpEkonomik.kod);

                mpUB.toplam = tab.DagitimBul(yil, "", asama, kurumKod, "", "", mpUB.mpEkonomik.kod);
                mpUB.toplamMerkez = tab.DagitimBul(yil, "1", asama, kurumKod, "", "", mpUB.mpEkonomik.kod);
                mpUB.toplamBirim = tab.DagitimBul(yil, ky.kod, asama, kurumKod, "", "", mpUB.mpEkonomik.kod);
                mpUB.toplamTasra = mpUB.toplam - mpUB.toplamMerkez;
                al.Add(mpUB);
            }
            ve.VeriErisimKapat();
            return (MPBilgi[])al.ToArray(typeof(MPBilgi));
        }

        internal static Sonuc MasrafPlaniGenelUretAHP(VeriErisim veriErisim, Kullanici kullanan, int yil, ENUMMasrafPlanAsama asama, string kurumsalYapiKod, bool ustuneYaz, int ay, Tertip tertip)
        {
            IBHMv2Servis servisBHM = BHMv2.Arac.Tanimla();
            string odenekAsama = ProgramOnayForm.OnayliAsamaBul(null, yil);
            if (string.IsNullOrWhiteSpace(odenekAsama)) odenekAsama = "06.00";

            Sonuc s = new Sonuc(true, "", "");
            if (asama == ENUMMasrafPlanAsama.SONDURUMONAYLI)
                return new Sonuc(false, "Son durum aşaması üretilemez!", "");

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                return new Sonuc(false, "Bu işlemi yapmaya yetkiniz yok!", "");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            int durum = MasrafPlaniGenelOnayDurumBul(ve, yil, kurumsalYapiKod, asama);
            if (durum >= 5)
            {
                ve.VeriErisimKapat();
                s.DegerAta("Onaylı olduğundan bu işlem yapılamaz!");
                return s;
            }

            durum = MasrafPlaniGenelOnayDurumBul(ve, yil, "", asama);
            if (durum >= 5)
            {
                ve.VeriErisimKapat();
                s.DegerAta("Onaylı olduğundan bu işlem yapılamaz!");
                return s;
            }

            string kurumKod = TNS.DEG.Arac.VarsayilanKurumBul();

            TABMPGMASRAFPLANI tabMP = new TABMPGMASRAFPLANI(ve);
            ve.BeginTransaction();
            if (ustuneYaz)
                s = tabMP.MPSil(yil, asama, kurumsalYapiKod, tertip);
            if (s.islemSonuc)
                s = tabMP.MPUretAHP(yil, asama, kurumsalYapiKod, ay, tertip, odenekAsama);
            if (s.islemSonuc)
                OlusanlariKontrolEt(tabMP, s, yil, asama, kurumKod, ay, tertip);
            if (s.islemSonuc)
                s = tabMP.MerkezMerkezleriOlustur(yil, asama, kurumKod, ay, tertip);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            return s;
        }

        private static void OlusanlariKontrolEt(TABMPGMASRAFPLANI tabMP, Sonuc s, int yil, ENUMMasrafPlanAsama asama, string kurumKod, int ay, Tertip tertip)
        {
            IDataReader dr = tabMP.OlusanlariKontrolEt(yil, asama, kurumKod, ay, tertip);
            if (dr != null)
            {
                while (dr.Read())
                {
                    s.DegerAta($@"{VeriErisim.DegerAl(dr, 2, "")}-{VeriErisim.DegerAl(dr, 3, "")}-{VeriErisim.DegerAl(dr, 4, "")}-{VeriErisim.DegerAl(dr, 5, "")}-{VeriErisim.DegerAl(dr, 6, "")} tertibinde {VeriErisim.DegerAl(dr, 8, 0)}. Ay MP'si AHP'den büyük ({VeriErisim.DegerAl(dr, 9, 0.0)})<br />");
                }
                dr.Close();
            }
            else
                s.DegerAta("Oluşan rakamlar kontrol edilemedi:" + tabMP.Hata());
        }

        internal static TertipOdenek[] MasrafPlaniTertipToplamGonderilen(VeriErisim veriErisim, Kullanici kullanan, ENUMMasrafPlanAsama asama, int ay, Tertip kistasTertip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", kullanan.KimlikBul());
            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(ve);

            IDataReader dr = tab.ListeleTertip(kistasTertip, asama, ay);
            ArrayList al = new ArrayList();
            if (dr != null)
            {
                while (dr.Read())
                {
                    TertipOdenek to = new TertipOdenek(2);
                    to.tertip = new Tertip(kistasTertip.yil, ENUMTabloKod.TABTasraOdenek, VeriErisim.DegerAl(dr, 3, ""), VeriErisim.DegerAl(dr, 4, ""), VeriErisim.DegerAl(dr, 5, ""), VeriErisim.DegerAl(dr, 6, ""), VeriErisim.DegerAl(dr, 7, ""), VeriErisim.DegerAl(dr, 1, "") + "-" + VeriErisim.DegerAl(dr, 2, ""));
                    to.odenek[0] = VeriErisim.DegerAl(dr, 8, (double)0.0);
                    string sql = servisUZY.UzayKosulStr(kullanan, "MUHGONDERMEBELGESITENKISBIRIM", TNS.UZY.Arac.UzayKosulOl(kistasTertip.yil.ToString(), "", VeriErisim.DegerAl(dr, 1, ""), VeriErisim.DegerAl(dr, 2, ""), "0/6", to.tertip.kodlar[0], to.tertip.kodlar[1], to.tertip.kodlar[2], to.tertip.kodlar[3], to.tertip.kodlar[4]), false, "");
                    to.odenek[1] = ve2.TekKolonSQLDbl(sql);
                    sql = servisUZY.UzayKosulStr(kullanan, "MUHMUHASEBE", VeriErisim.DegerAl(dr, 1, ""), false, "");
                    to.tertip.adlar[5] = ve2.TekKolonSQLStr(sql);
                    al.Add(to);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return (TertipOdenek[])al.ToArray(typeof(TertipOdenek));
        }

        public static TertipOdenek[] MPBirimListele(VeriErisim veriErisim, Kullanici kullanan, Tertip kistas)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(ve);
            VeriErisim veBUM = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", kullanan.KimlikBul());

            ArrayList al = new ArrayList();
            IDataReader dr = tab.ListeleBirim(kistas.yil, kistas.kodlar[0], kistas.kodlar[1], kistas.kodlar[2], kistas.kodlar[3], kistas.kodlar[4]);

            if (dr != null)
            {
                string oncekiKYKod = "";
                TertipOdenek to = null;
                while (dr.Read())
                {
                    string kyKod = VeriErisim.DegerAl(dr, 0, "");
                    int saymanlikKod = VeriErisim.DegerAl(dr, 1, 0);
                    string birimKod = VeriErisim.DegerAl(dr, 2, "");
                    int ay = VeriErisim.DegerAl(dr, 3, -1);
                    double odenek = VeriErisim.DegerAl(dr, 4, 0.0);
                    //string tertipStr =  + "-" + VeriErisim.DegerAl(dr, 1, "") + "-" + VeriErisim.DegerAl(dr, 2, "") + "-" + VeriErisim.DegerAl(dr, 3, "") + "-" + VeriErisim.DegerAl(dr, 4, "");
                    if (oncekiKYKod != kyKod)
                    {
                        oncekiKYKod = kyKod;
                        to = new TertipOdenek(13);
                        to.tertip = new Tertip(kistas.yil, ENUMTabloKod.TABPEBOdenek);
                        to.tertip.kodlar[0] = kyKod;
                        to.tertip.kodlar[1] = saymanlikKod.ToString();
                        to.tertip.kodlar[2] = birimKod;
                        al.Add(to);
                        to.odenek[12] = UzayHesapla(veBUM, kullanan, "MUHGONDERMEBELGESITENKIS", TNS.UZY.Arac.UzayKosulOl(kistas.yil.ToString(), "", "", "", saymanlikKod.ToString(), birimKod, "", "0/6", kistas.kodlar[0], kistas.kodlar[1], kistas.kodlar[2], kistas.kodlar[3], kistas.kodlar[4]), "");
                    }
                    if (to != null && ay >= 1 && ay <= 12)
                    {
                        to.odenek[ay - 1] = odenek;
                    }
                }
                dr.Close();
                ve.VeriErisimKapat();
                veBUM.VeriErisimKapat();
                return (TertipOdenek[])al.ToArray(typeof(TertipOdenek));
            }
            else
            {
                ve.VeriErisimKapat();
                veBUM.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
        }

        public static TertipOdenek[] MPTertipListele(VeriErisim veriErisim, Kullanici kullanan, string kurumsalYapi, Tertip kistas)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABMPGMASRAFPLANI tab = new TABMPGMASRAFPLANI(ve);
            VeriErisim veBUM = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", kullanan.KimlikBul());

            int saymanlikKod = 0;
            string birimKod = "";
            KurumsalYapi ky;
            try
            {
                ky = IsKurallari.BHMv2KurumsalYapi.KurumsalYapiDondur(ve, kurumsalYapi);
                if (ky != null)
                {
                    saymanlikKod = ky.saymanlikKod;
                    birimKod = ky.birimKod;
                }
            }
            catch { };

            ArrayList al = new ArrayList();
            IDataReader dr = tab.ListeleBirimTertip(kistas.yil, kurumsalYapi, kistas.kodlar[0], kistas.kodlar[1], kistas.kodlar[2], kistas.kodlar[3], kistas.kodlar[4], kistas.KodUzunlukBul(3, 2, true));

            if (dr != null)
            {
                string oncekiTertipStr = "";
                TertipOdenek to = null;
                while (dr.Read())
                {
                    string kurKod = VeriErisim.DegerAl(dr, 0, "");
                    string fonKod = VeriErisim.DegerAl(dr, 1, "");
                    string finKod = VeriErisim.DegerAl(dr, 2, "");
                    string ekoKod = VeriErisim.DegerAl(dr, 3, "");
                    string pebFonKod = VeriErisim.DegerAl(dr, 4, "");

                    string simdikiTertipStr = kurKod + "-" + fonKod + "-" + finKod + "-" + ekoKod + "-" + pebFonKod;
                    int ay = VeriErisim.DegerAl(dr, 5, -1);
                    double odenek = VeriErisim.DegerAl(dr, 6, 0.0);
                    //string tertipStr =  + "-" + VeriErisim.DegerAl(dr, 1, "") + "-" + VeriErisim.DegerAl(dr, 2, "") + "-" + VeriErisim.DegerAl(dr, 3, "") + "-" + VeriErisim.DegerAl(dr, 4, "");
                    if (oncekiTertipStr != simdikiTertipStr)
                    {
                        oncekiTertipStr = simdikiTertipStr;
                        to = new TertipOdenek(13);
                        to.tertip = new Tertip(kistas.yil, ENUMTabloKod.TABPEBOdenek);
                        to.tertip.TertipAta(simdikiTertipStr);
                        to.tertip.Noktala();
                        al.Add(to);
                        string saymalikKosul = "";
                        if (saymanlikKod > 0) saymalikKosul = saymanlikKod.ToString();
                        to.odenek[12] = UzayHesapla(veBUM, kullanan, "MUHGONDERMEBELGESITENKIS", TNS.UZY.Arac.UzayKosulOl(kistas.yil.ToString(), "", "", "", saymalikKosul, birimKod, "", "0/6", simdikiTertipStr), "");
                    }
                    if (to != null && ay >= 1 && ay <= 12)
                    {
                        to.odenek[ay - 1] = odenek;
                    }
                }
                dr.Close();
                ve.VeriErisimKapat();
                veBUM.VeriErisimKapat();
                return (TertipOdenek[])al.ToArray(typeof(TertipOdenek));
            }
            else
            {
                ve.VeriErisimKapat();
                veBUM.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
        }

        public static double[] MasrafPlaniOdenekBilgiBul(VeriErisim veriErisim, Tertip kistasTertip, bool onaysiz, int haricRevizeNo, bool harcamaVar = true, bool blokeVar = true)
        {
            double[] donusDeger = new double[18];
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, null);
            TABMPGMASRAFPLANI tabMP = new TABMPGMASRAFPLANI(ve);

            int kistasAy = TNS.BHMv2.Arac.SerbestAyBul(kistasTertip.yil);

            IDataReader dr = tabMP.ListeleRapor("AY", kistasTertip, ENUMMasrafPlanAsama.SONDURUMONAYLI, -1, haricRevizeNo);
            donusDeger[0] = 0;
            donusDeger[1] = 0;
            if (dr != null)
            {
                while (dr.Read())
                {
                    int ay = VeriErisim.DegerAl(dr, 0, (int)0);
                    double deger = VeriErisim.DegerAl(dr, 1, (double)0);
                    if (ay >= 1 && ay <= 12)
                    {
                        donusDeger[5 + ay] = deger;
                        donusDeger[0] += deger;
                        if (ay <= kistasAy)
                            donusDeger[1] += deger;
                    }
                }
                dr.Close();
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            ve.VeriErisimKapat();

            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            EkDegerParam ep;
            if (harcamaVar)
            {
                ep = new EkDegerParam(ENUMTabloKod.TABPEBHarcama, 0, 0, "");
                donusDeger[2] = ep.EkDegerHesaplaDbl(uzyServis, null, kistasTertip.yil, kistasTertip.TertipStrOl());
            }

            if (blokeVar)
            {
                ep = new EkDegerParam(ENUMTabloKod.TABHYBloke, 0, 0, "");
                donusDeger[3] = ep.EkDegerHesaplaDbl(uzyServis, null, kistasTertip.yil, kistasTertip.TertipStrOl());
            }
            donusDeger[4] = donusDeger[0] - donusDeger[2] - donusDeger[3];
            donusDeger[5] = donusDeger[1] - donusDeger[2];

            return donusDeger;
        }

        internal static Sonuc MPAHPdenMPye(VeriErisim veriErisim, Kullanici kullanan, int yil, ENUMMasrafPlanAsama asama, TNSCollection tertipler)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (asama == ENUMMasrafPlanAsama.SONDURUMONAYLI)
                return new Sonuc(false, "Son durum aşaması üretilemez!", "");

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                return new Sonuc(false, "Bu işlemi yapmaya yetkiniz yok!", "");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            //int durum = MasrafPlaniGenelOnayDurumBul(ve, yil, kurumsalYapiKod, asama);
            //if (durum >= 5)
            //{
            //    ve.VeriErisimKapat();
            //    s.DegerAta("Onaylı olduğundan bu işlem yapılamaz!");
            //    return s;
            //}

            int durum = MasrafPlaniGenelOnayDurumBul(ve, yil, "", asama);
            if (durum >= 5)
            {
                ve.VeriErisimKapat();
                s.DegerAta("Onaylı olduğundan bu işlem yapılamaz!");
                return s;
            }

            TABMPGMASRAFPLANI tabMP = new TABMPGMASRAFPLANI(ve);
            ve.BeginTransaction();

            foreach (Tertip tertip in tertipler)
            {
                s = tabMP.MPSil(yil, asama, tertip);
                if (s.islemSonuc)
                    s = tabMP.AHPdenMPye(yil, asama, tertip);
                if (!s.islemSonuc) break;
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            return s;
        }

        internal static Sonuc MPdenMPye(VeriErisim veriErisim, Kullanici kullanan, int yil, ENUMMasrafPlanAsama kaynakAsama, ENUMMasrafPlanAsama asama)
        {
            Sonuc s = new Sonuc(true, "", "");

            if ((int)kaynakAsama >= (int)asama)
            {
                return new Sonuc(false, "Hedef aşama kaynak aşamadan büyük olmalı!", "");
            }

            if (asama == ENUMMasrafPlanAsama.SONDURUMONAYLI)
                return new Sonuc(false, "Son durum aşaması üretilemez!", "");

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                return new Sonuc(false, "Bu işlemi yapmaya yetkiniz yok!", "");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());

            int durum = MasrafPlaniGenelOnayDurumBul(ve, yil, "", asama);
            if (durum >= 5)
            {
                ve.VeriErisimKapat();
                s.DegerAta("Onaylı olduğundan bu işlem yapılamaz!");
                return s;
            }

            TABMPGMASRAFPLANI tabMP = new TABMPGMASRAFPLANI(ve);
            ve.BeginTransaction();
            s = tabMP.MPSil(yil, asama, "", null);
            if (s.islemSonuc)
                s = tabMP.MPdenMPye(yil, kaynakAsama, asama);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            return s;
        }
    }
}
