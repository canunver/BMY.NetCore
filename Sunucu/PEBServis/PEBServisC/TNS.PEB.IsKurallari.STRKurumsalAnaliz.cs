using System;
using System.Data;
using TNS.KYM;
using TNS.PEB.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.PEB.IsKurallari
{
    public class STRKurumsalAnaliz
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, KurumsalAnaliz[] kalar, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            foreach (KurumsalAnaliz ka in kalar)
            {
                if (ka.donem == null || ka.donem.donemNo <= 0)
                    hata += PerformansButceResI.STR032 + "<br>";
                if (ka.orgBirimKod <= 0)
                    hata += PerformansButceResI.STR033 + "<br>";
                if (ka.tur <= 0)
                    hata += PerformansButceResI.STR085 + "<br>";
            }
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, kalar[0].orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, kalar[0].donem.donemNo, kalar[0].orgBirimKod, ENUMSurec.MevcutDurumAnalizi, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABKurumsalAnaliz tka = new TABKurumsalAnaliz(ve);
            ve.BeginTransaction();

            tka.Sil(kalar[0]);
            foreach (KurumsalAnaliz ka in kalar)
            {
                int karakterSayi = 1000;
                string metin = ka.metin;
                while (!string.IsNullOrEmpty(metin))
                {
                    ka.siraNo++;

                    if (metin.Length > karakterSayi)
                    {
                        ka.metin = metin.Substring(0, karakterSayi);
                        metin = metin.Substring(karakterSayi);
                    }
                    else
                    {
                        ka.metin = metin;
                        metin = string.Empty;
                    }

                    sonuc = tka.Kaydet(ka);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, kalar[0].donem.donemNo, kalar[0].orgBirimKod, ENUMSurec.MevcutDurumAnalizi);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, KurumsalAnaliz ka, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (ka.donem == null || ka.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (ka.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, ka.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, ka.donem.donemNo, ka.orgBirimKod, ENUMSurec.MevcutDurumAnalizi, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABKurumsalAnaliz tabKA = new TABKurumsalAnaliz(ve);
            ve.BeginTransaction();

            sonuc = tabKA.Sil(ka);
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

        public static KurumsalAnaliz[] Listele(VeriErisim veriErisim, Kullanici kullanan, KurumsalAnaliz ka)
        {
            return Listele(veriErisim, kullanan, ka, false);
        }

        public static KurumsalAnaliz[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, KurumsalAnaliz ka)
        {
            return Listele(veriErisim, kullanan, ka, true);
        }

        private static KurumsalAnaliz[] Listele(VeriErisim veriErisim, Kullanici kullanan, KurumsalAnaliz ka, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, ka.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            string hata = string.Empty;
            if (ka.donem == null || ka.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (ka.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKurumsalAnaliz tka = new TABKurumsalAnaliz(ve);

            IDataReader dr = null;
            try
            {
                dr = tka.Listele(ka, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection kalar = new TNSCollection();
            while (dr.Read())
            {
                int donemNo = VeriErisim.DegerAl(dr, (int)TABKurumsalAnaliz.ENUMAlanlar.DONEMNO, 0);
                int orgBirimKod = VeriErisim.DegerAl(dr, (int)TABKurumsalAnaliz.ENUMAlanlar.ORGBIRIMKOD, 0);
                int tur = VeriErisim.DegerAl(dr, (int)TABKurumsalAnaliz.ENUMAlanlar.TUR, 0);

                KurumsalAnaliz gelen = KurumsalAnalizBul(kalar, donemNo, orgBirimKod, tur);
                if (gelen == null)
                {
                    gelen = new KurumsalAnaliz();
                    gelen.donem = new Donem();
                    gelen.donem.donemNo = donemNo;
                    gelen.orgBirimKod = orgBirimKod;
                    gelen.tur = tur;

                    kalar.Add(gelen);
                }

                gelen.metin += VeriErisim.DegerAl(dr, (int)TABKurumsalAnaliz.ENUMAlanlar.METIN, string.Empty);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (KurumsalAnaliz[])kalar.ToArray(typeof(KurumsalAnaliz));
        }

        private static KurumsalAnaliz KurumsalAnalizBul(TNSCollection kalar, int donemNo, int orgBirimKod, int tur)
        {
            foreach (KurumsalAnaliz ka in kalar)
                if (ka.donem.donemNo == donemNo && ka.orgBirimKod == orgBirimKod && ka.tur == tur)
                    return ka;
            return null;
        }

        public static Sonuc KaydetSablon(VeriErisim veriErisim, Kullanici kullanan, KurumsalAnalizSablon sablon)
        {
            Sonuc sonuc = YetkiSTR.TanimlamaYazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (sablon.donem == null || sablon.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (string.IsNullOrEmpty(sablon.ad))
                hata += PerformansButceResI.STR095 + "<br>";
            if (sablon.detay == null || sablon.detay.Length <= 0)
                hata += PerformansButceResI.STR096 + "<br>";
            else
            {
                foreach (KurumsalAnaliz ka in sablon.detay)
                {
                    if (ka.tur <= 0)
                    {
                        hata += PerformansButceResI.STR085 + "<br>";
                        break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKurumsalAnaliz tka = new TABKurumsalAnaliz(ve);
            ve.BeginTransaction();

            if (sablon.kod > 0)
                sonuc = tka.GuncelleSablon(sablon);
            else
                sonuc = tka.KaydetSablon(sablon);

            if (sonuc.islemSonuc)
            {
                int sablonKod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0);

                tka.SilSablonDetay(sablon.detay[0]);
                foreach (KurumsalAnaliz ka in sablon.detay)
                {
                    int karakterSayi = 1000;
                    string metin = ka.metin;
                    while (!string.IsNullOrEmpty(metin))
                    {
                        ka.donem = sablon.donem;
                        ka.orgBirimKod = sablonKod;
                        ka.siraNo++;

                        if (metin.Length > karakterSayi)
                        {
                            ka.metin = metin.Substring(0, karakterSayi);
                            metin = metin.Substring(karakterSayi);
                        }
                        else
                        {
                            ka.metin = metin;
                            metin = string.Empty;
                        }

                        sonuc = tka.KaydetSablonDetay(ka);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }

                sonuc.anahtar = sablonKod.ToString();
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

        public static Sonuc SilSablon(VeriErisim veriErisim, Kullanici kullanan, KurumsalAnalizSablon sablon)
        {
            Sonuc sonuc = YetkiSTR.TanimlamaYazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (sablon.donem == null || sablon.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (sablon.kod <= 0)
                hata += PerformansButceResI.STR094 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKurumsalAnaliz tka = new TABKurumsalAnaliz(ve);
            ve.BeginTransaction();

            KurumsalAnaliz ka = new KurumsalAnaliz();
            ka.donem = sablon.donem;
            ka.orgBirimKod = sablon.kod;

            sonuc = tka.SilSablonDetay(ka);
            if (sonuc.islemSonuc)
                sonuc = tka.SilSablon(sablon);

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

        public static KurumsalAnalizSablon[] ListeleSablon(VeriErisim veriErisim, Kullanici kullanan, KurumsalAnalizSablon sablon)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            string hata = string.Empty;
            if (sablon.donem == null || sablon.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKurumsalAnaliz tka = new TABKurumsalAnaliz(ve);

            IDataReader dr = null;
            try
            {
                dr = tka.ListeleSablon(sablon);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection kaSablonListe = new TNSCollection();
            while (dr.Read())
            {
                KurumsalAnalizSablon gelen = new KurumsalAnalizSablon();
                gelen.donem = new Donem();
                gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABKurumsalAnaliz.ENUMAlanlarSablon.DONEMNO, 0);
                gelen.kod = VeriErisim.DegerAl(dr, (int)TABKurumsalAnaliz.ENUMAlanlarSablon.KOD, 0);
                gelen.ad = VeriErisim.DegerAl(dr, (int)TABKurumsalAnaliz.ENUMAlanlarSablon.AD, string.Empty);

                kaSablonListe.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (KurumsalAnalizSablon[])kaSablonListe.ToArray(typeof(KurumsalAnalizSablon));
        }

        public static KurumsalAnaliz[] ListeleSablonDetay(VeriErisim veriErisim, Kullanici kullanan, KurumsalAnaliz ka)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            string hata = string.Empty;
            if (ka.donem == null || ka.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (ka.orgBirimKod <= 0)
                hata += PerformansButceResI.STR094 + "<br>";
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKurumsalAnaliz tka = new TABKurumsalAnaliz(ve);

            IDataReader dr = null;
            try
            {
                dr = tka.ListeleSablonDetay(ka);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection kalar = new TNSCollection();
            while (dr.Read())
            {
                int donemNo = VeriErisim.DegerAl(dr, (int)TABKurumsalAnaliz.ENUMAlanlarSablonDetay.DONEMNO, 0);
                int orgBirimKod = VeriErisim.DegerAl(dr, (int)TABKurumsalAnaliz.ENUMAlanlarSablonDetay.KOD, 0);
                int tur = VeriErisim.DegerAl(dr, (int)TABKurumsalAnaliz.ENUMAlanlarSablonDetay.TUR, 0);

                KurumsalAnaliz gelen = KurumsalAnalizBul(kalar, donemNo, orgBirimKod, tur);
                if (gelen == null)
                {
                    gelen = new KurumsalAnaliz();
                    gelen.donem = new Donem();
                    gelen.donem.donemNo = donemNo;
                    gelen.orgBirimKod = orgBirimKod;
                    gelen.tur = tur;

                    kalar.Add(gelen);
                }

                gelen.metin += VeriErisim.DegerAl(dr, (int)TABKurumsalAnaliz.ENUMAlanlarSablonDetay.METIN, string.Empty);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (KurumsalAnaliz[])kalar.ToArray(typeof(KurumsalAnaliz));
        }
    }
}