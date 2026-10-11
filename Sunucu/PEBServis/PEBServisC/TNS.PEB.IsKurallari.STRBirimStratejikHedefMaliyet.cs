using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    public class STRBirimStratejikHedefMaliyet
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, BirimStratejikHedefMaliyet[] maliyetler, string formId)
        {
            Sonuc sonuc = new Sonuc();

            if (maliyetler == null || maliyetler.Length <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR086;
                return sonuc;
            }

            foreach (BirimStratejikHedefMaliyet shm in maliyetler)
            {
                string hata = string.Empty;
                if (shm.donem == null || shm.donem.donemNo <= 0)
                    hata = PerformansButceResI.STR032 + "<br>";
                if (shm.orgBirimKod <= 0)
                    hata += PerformansButceResI.STR033 + "<br>";
                if (string.IsNullOrEmpty(shm.hedefKod))
                    hata += PerformansButceResI.STR077 + "<br>";
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, maliyetler[0].orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, maliyetler[0].donem.donemNo, maliyetler[0].orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimStratejikHedefMaliyet tStratejikHedefMaliyet = new TABBirimStratejikHedefMaliyet(ve);
            ve.BeginTransaction();

            tStratejikHedefMaliyet.Sil(maliyetler[0]);

            foreach (BirimStratejikHedefMaliyet sb in maliyetler)
            {
                if (sb.maliyet <= 0)
                    continue;

                sonuc = tStratejikHedefMaliyet.Kaydet(sb);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, maliyetler[0].donem.donemNo, maliyetler[0].orgBirimKod, ENUMSurec.StratejikModelleme);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, BirimStratejikHedefMaliyet maliyet, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (maliyet.donem == null || maliyet.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (maliyet.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, maliyet.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, maliyet.donem.donemNo, maliyet.orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimStratejikHedefMaliyet tStrateji = new TABBirimStratejikHedefMaliyet(ve);
            ve.BeginTransaction();

            sonuc = tStrateji.Sil(maliyet);
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

        public static BirimStratejikHedefMaliyet[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimStratejikHedefMaliyet maliyet)
        {
            return Listele(veriErisim, kullanan, maliyet, false);
        }

        public static BirimStratejikHedefMaliyet[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, BirimStratejikHedefMaliyet maliyet)
        {
            string hata = string.Empty;
            if (maliyet.donem == null || maliyet.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (maliyet.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033;
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            return Listele(veriErisim, kullanan, maliyet, true);
        }

        private static BirimStratejikHedefMaliyet[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimStratejikHedefMaliyet maliyet, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, maliyet.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimStratejikHedefMaliyet tStrateji = new TABBirimStratejikHedefMaliyet(ve);

            IDataReader dr = null;
            try
            {
                dr = tStrateji.Listele(maliyet, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection stratejiler = new TNSCollection();
            while (dr.Read())
            {
                BirimStratejikHedefMaliyet gelen = new BirimStratejikHedefMaliyet();
                gelen.donem = maliyet.donem;
                gelen.orgBirimKod = maliyet.orgBirimKod;
                gelen.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                gelen.hedefKod = VeriErisim.DegerAl(dr, (int)TABBirimStratejikHedefMaliyet.ENUMAlanlar.HEDEFKOD, string.Empty);

                if (sorgu && StratejikHedefMaliyetVarMi(stratejiler, gelen))
                    continue;

                gelen.hedefAd = PEBServis.UzayYaziDegerGetir("STRBIRIMAMACHEDEF", gelen.donem.donemNo + "-" + gelen.orgBirimKod + "-" + gelen.hedefKod, true, string.Empty);
                gelen.yil = VeriErisim.DegerAl(dr, (int)TABBirimStratejikHedefMaliyet.ENUMAlanlar.YIL, 0);
                gelen.maliyet = VeriErisim.DegerAl(dr, (int)TABBirimStratejikHedefMaliyet.ENUMAlanlar.MALIYET, (double)0);

                stratejiler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (BirimStratejikHedefMaliyet[])stratejiler.ToArray(typeof(BirimStratejikHedefMaliyet));
        }

        public static BirimStratejikHedefMaliyetKatman[] Raporla(VeriErisim veriErisim, Kullanici kullanan, BirimStratejikHedefMaliyet maliyetGelen)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, maliyetGelen.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimStratejikHedefMaliyet tStrateji = new TABBirimStratejikHedefMaliyet(ve);

            IDataReader dr = null;
            try
            {
                dr = tStrateji.Listele(maliyetGelen, false);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            TNSCollection stratejiler = new TNSCollection();
            Birim_AmacHedef bahKriter = new Birim_AmacHedef();
            bahKriter.donem = new Donem();
            Birim_AmacHedef[] hedefBilgi = null;
            Birim_AmacHedef[] amacBilgi = null;
            string hedefKod = string.Empty;
            while (dr.Read())
            {
                BirimStratejikHedefMaliyet gelen = new BirimStratejikHedefMaliyet();
                gelen.donem = maliyetGelen.donem;
                gelen.orgBirimKod = maliyetGelen.orgBirimKod;
                gelen.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                gelen.hedefKod = VeriErisim.DegerAl(dr, (int)TABBirimStratejikHedefMaliyet.ENUMAlanlar.HEDEFKOD, string.Empty);
                gelen.hedefAd = PEBServis.UzayYaziDegerGetir("STRBIRIMAMACHEDEF", gelen.donem.donemNo + "-" + gelen.orgBirimKod + "-" + gelen.hedefKod, true, string.Empty);
                gelen.yil = VeriErisim.DegerAl(dr, (int)TABBirimStratejikHedefMaliyet.ENUMAlanlar.YIL, 0);
                gelen.maliyet = VeriErisim.DegerAl(dr, (int)TABBirimStratejikHedefMaliyet.ENUMAlanlar.MALIYET, (double)0);

                if (hedefKod != gelen.hedefKod)
                {
                    bahKriter.donem.donemNo = gelen.donem.donemNo;
                    bahKriter.orgBirimKod = gelen.orgBirimKod;
                    bahKriter.amacHedefKod = gelen.hedefKod;
                    hedefBilgi = STRBirimAmacHedef.Listele(ve2, kullanan, bahKriter, false, false);
                    if (hedefBilgi.Length > 0)
                    {
                        bahKriter.amacHedefKod = hedefBilgi[0].temaAmacKod;
                        amacBilgi = STRBirimAmacHedef.Listele(ve2, kullanan, bahKriter, false, true);
                    }
                    if (hedefBilgi.Length > 0 && amacBilgi.Length > 0)
                        hedefKod = gelen.hedefKod;
                }

                gelen.amacKod = hedefBilgi[0].temaAmacKod;
                gelen.amacAd = hedefBilgi[0].temaAmacAd;
                gelen.temaKod = amacBilgi[0].temaAmacKod;
                gelen.temaAd = amacBilgi[0].temaAmacAd;

                stratejiler.Add(gelen);
            }
            dr.Close();
            
            ve2.VeriErisimKapat();

            BirimStratejikHedefMaliyet[] maliyetler = (BirimStratejikHedefMaliyet[])stratejiler.ToArray(typeof(BirimStratejikHedefMaliyet));

            string[] temalar = TemalariBul(maliyetler);
            BirimStratejikHedefMaliyetKatman[] donusTemalar = new BirimStratejikHedefMaliyetKatman[temalar.Length];
            for (int i = 0; i < donusTemalar.Length; i++)
            {
                donusTemalar[i] = new BirimStratejikHedefMaliyetKatman();
                donusTemalar[i].katmanKod = temalar[i];
                donusTemalar[i].katmanAd = KoddanAdBul(maliyetler, temalar[i], EnumSPGenelHavuzTur.Tema);
                if (maliyetler.Length <= 0)
                    return null;

                donusTemalar[i].orgBirimAd = maliyetler[0].orgBirimAd;

                string[] amaclar = AmaclariBul(maliyetler, temalar[i]);
                BirimStratejikHedefMaliyetKatman[] donusAmaclar = new BirimStratejikHedefMaliyetKatman[amaclar.Length];
                for (int j = 0; j < amaclar.Length; j++)
                {
                    donusAmaclar[j] = new BirimStratejikHedefMaliyetKatman();
                    donusAmaclar[j].katmanKod = amaclar[j];
                    donusAmaclar[j].katmanAd = KoddanAdBul(maliyetler, amaclar[j], EnumSPGenelHavuzTur.Amac);

                    string[] hedefler = HedefleriBul(maliyetler, temalar[i], amaclar[j]);
                    BirimStratejikHedefMaliyetKatman[] donusHedefler = new BirimStratejikHedefMaliyetKatman[hedefler.Length];
                    for (int k = 0; k < hedefler.Length; k++)
                    {
                        donusHedefler[k] = new BirimStratejikHedefMaliyetKatman();
                        donusHedefler[k].katmanKod = hedefler[k];
                        donusHedefler[k].katmanAd = KoddanAdBul(maliyetler, hedefler[k], EnumSPGenelHavuzTur.Hedef);

                        string[] yilMaliyetler = YilMaliyetBul(ve, kullanan, maliyetler, temalar[i], amaclar[j], hedefler[k]);
                        BirimStratejikHedefMaliyetKatman[] donusYilMaliyetler = new BirimStratejikHedefMaliyetKatman[yilMaliyetler.Length];
                        for (int m = 0; m < yilMaliyetler.Length; m++)
                        {
                            donusYilMaliyetler[m] = new BirimStratejikHedefMaliyetKatman();
                            donusYilMaliyetler[m].yil = OrtakFonksiyonlar.ConvertToInt(yilMaliyetler[m].Split(';')[0], 0);
                            donusYilMaliyetler[m].maliyet = OrtakFonksiyonlar.ConvertToDouble(yilMaliyetler[m].Split(';')[1], (double)0);
                        }
                        donusHedefler[k].altKatman = donusYilMaliyetler;
                    }
                    donusAmaclar[j].altKatman = donusHedefler;
                }
                donusTemalar[i].altKatman = donusAmaclar;
            }

            ve.VeriErisimKapat();
            return donusTemalar;
        }

        private static string[] TemalariBul(BirimStratejikHedefMaliyet[] maliyetler)
        {
            string temalar = string.Empty;
            foreach (BirimStratejikHedefMaliyet m in maliyetler)
            {
                if (!temalar.Contains(m.temaKod))
                    temalar += m.temaKod + ' ';
            }
            return temalar.Trim().Split(' ');
        }

        private static string[] AmaclariBul(BirimStratejikHedefMaliyet[] maliyetler, string temaKod)
        {
            string amaclar = string.Empty;
            foreach (BirimStratejikHedefMaliyet m in maliyetler)
            {
                if (m.temaKod == temaKod && !amaclar.Contains(m.amacKod))
                    amaclar += m.amacKod + ' ';
            }
            return amaclar.Trim().Split(' ');
        }

        private static string[] HedefleriBul(BirimStratejikHedefMaliyet[] maliyetler, string temaKod, string amacKod)
        {
            string hedefler = string.Empty;
            foreach (BirimStratejikHedefMaliyet m in maliyetler)
            {
                if (m.temaKod == temaKod && m.amacKod == amacKod && !hedefler.Contains(m.hedefKod))
                    hedefler += m.hedefKod + ' ';
            }
            return hedefler.Trim().Split(' ');
        }

        private static string[] YilMaliyetBul(VeriErisim veriErisim, Kullanici kullanan, BirimStratejikHedefMaliyet[] maliyetler, string temaKod, string amacKod, string hedefKod)
        {
            Donem donem = STRDonem.Listele(veriErisim, kullanan, maliyetler[0].donem)[0];
            string[] ym = new string[donem.bitisYili - donem.baslamaYili + 1];
            for (int i = 0; i < ym.Length; i++)
                ym[i] = donem.baslamaYili + i + ";0";

            foreach (BirimStratejikHedefMaliyet m in maliyetler)
                if (m.temaKod == temaKod && m.amacKod == amacKod && m.hedefKod == hedefKod)
                    for (int i = 0; i < ym.Length; i++)
                        if (ym[i].Contains(m.yil.ToString()))
                            ym[i] = ym[i].Substring(0, ym[i].Length - 1) + m.maliyet;

            return ym;
        }

        private static string KoddanAdBul(BirimStratejikHedefMaliyet[] maliyetler, string kod, EnumSPGenelHavuzTur kodTur)
        {
            foreach (BirimStratejikHedefMaliyet m in maliyetler)
            {
                if (kodTur == EnumSPGenelHavuzTur.Tema && m.temaKod == kod) return m.temaAd;
                if (kodTur == EnumSPGenelHavuzTur.Amac && m.amacKod == kod) return m.amacAd;
                if (kodTur == EnumSPGenelHavuzTur.Hedef && m.hedefKod == kod) return m.hedefAd;
            }
            return string.Empty;
        }

        private static bool StratejikHedefMaliyetVarMi(TNSCollection maliyetler, BirimStratejikHedefMaliyet aranan)
        {
            foreach (BirimStratejikHedefMaliyet maliyet in maliyetler)
                if (maliyet.donem.donemNo == aranan.donem.donemNo && maliyet.orgBirimKod == aranan.orgBirimKod && maliyet.hedefKod == aranan.hedefKod)
                    return true;
            return false;
        }
    }
}