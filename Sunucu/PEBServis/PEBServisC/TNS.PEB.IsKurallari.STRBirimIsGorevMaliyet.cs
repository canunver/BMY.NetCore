using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    public class STRBirimIsGorevMaliyet
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, BirimIsGorevMaliyet[] maliyetler, string formId)
        {
            Sonuc sonuc = new Sonuc();

            if (maliyetler == null || maliyetler.Length <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR086;
                return sonuc;
            }

            foreach (BirimIsGorevMaliyet bigm in maliyetler)
            {
                string hata = string.Empty;
                if (bigm.donem == null || bigm.donem.donemNo <= 0)
                    hata = PerformansButceResI.STR032 + "<br>";
                if (bigm.orgBirimKod <= 0)
                    hata += PerformansButceResI.STR033 + "<br>";
                if (string.IsNullOrEmpty(bigm.isGorevKod))
                    hata += PerformansButceResI.STR137 + "<br>";
                if (bigm.yil <= 0)
                    hata += PerformansButceResI.STR119 + "<br>";
                if (string.IsNullOrEmpty(bigm.faaliyetKod))
                    hata += PerformansButceResI.STR141 + "<br>";
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

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, maliyetler[0].donem.donemNo, maliyetler[0].orgBirimKod, ENUMSurec.PerformansProgrami, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimIsGorevMaliyet tabMaliyet = new TABBirimIsGorevMaliyet(ve);
            ve.BeginTransaction();

            tabMaliyet.Sil(maliyetler[0]);

            foreach (BirimIsGorevMaliyet bigm in maliyetler)
            {
                if (bigm.maliyet <= 0)
                    continue;

                sonuc = tabMaliyet.Kaydet(bigm);
                if (!sonuc.islemSonuc)
                    break;
            }

            //Maliyeti girilmiş iş/görevlerden birimin iş/görevleri bulunup kayıt ediliyor.
            if (sonuc.islemSonuc)
                sonuc = BirimIsGorevKaydet(ve, kullanan, maliyetler, formId, true);

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, maliyetler[0].donem.donemNo, maliyetler[0].orgBirimKod, ENUMSurec.PerformansProgrami);

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

        private static int BirimIsGorevIndexDondur(TNSCollection isGorevler, string isGorevKod)
        {
            for (int i = 0; i < isGorevler.Count; i++)
            {
                Birim_SpGenelHavuz isGorev = (Birim_SpGenelHavuz)isGorevler[i];
                if (isGorev.spGenelHavuz.kod.Replace(".", "").Equals(isGorevKod.Replace(".", "")))
                    return i;
            }
            return -1;
        }

        private static Sonuc BirimIsGorevKaydet(VeriErisim ve, Kullanici kullanan, BirimIsGorevMaliyet[] maliyetler, string formId, bool kaydetMi)
        {
            //Birimin iş/görevleri bulunuyor.
            Birim_SpGenelHavuz bigKriter = new Birim_SpGenelHavuz();
            bigKriter.donem = maliyetler[0].donem;
            bigKriter.orgBirimKod = maliyetler[0].orgBirimKod;
            bigKriter.spGenelHavuz = new SpGenelHavuz();
            bigKriter.spGenelHavuz.kod = ((int)EnumSPGenelHavuzTur.IsGorev).ToString().PadLeft(2, '0');
            Birim_SpGenelHavuz[] bigler = BirimGenelHavuz.Listele(ve, kullanan, bigKriter);

            TNSCollection isGorevler = new TNSCollection(bigler);
            foreach (BirimIsGorevMaliyet maliyet in maliyetler)
            {
                int index = BirimIsGorevIndexDondur(isGorevler, maliyet.isGorevKod);
                if (!kaydetMi)
                {
                    if (index > -1)
                        isGorevler.RemoveAt(index);
                    continue;
                }
                else
                {
                    if (index > -1 && maliyet.maliyet == 0)
                    {
                        isGorevler.RemoveAt(index);
                        continue;
                    }
                    else if (index > -1 || maliyet.maliyet == 0)
                        continue;
                }

                Birim_SpGenelHavuz bspgh = new Birim_SpGenelHavuz();
                bspgh.donem = maliyet.donem;
                bspgh.orgBirimKod = maliyet.orgBirimKod;
                bspgh.spGenelHavuz = new SpGenelHavuz();
                bspgh.spGenelHavuz.kod = maliyet.isGorevKod;
                bspgh.spGenelHavuz.tur = maliyet.isGorevKod.Substring(0, 2);

                isGorevler.Add(bspgh);
            }

            if (isGorevler.Count > 0)
                return BirimGenelHavuz.Kaydet(ve, kullanan, (Birim_SpGenelHavuz[])isGorevler.ToArray(typeof(Birim_SpGenelHavuz)), formId);
            else
                return BirimIsGorevSil(ve, kullanan, maliyetler[0], formId);
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, BirimIsGorevMaliyet maliyet, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (maliyet.donem == null || maliyet.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (maliyet.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (maliyet.yil <= 0)
                hata += PerformansButceResI.STR119 + "<br>";
            if (string.IsNullOrEmpty(maliyet.faaliyetKod))
                hata += PerformansButceResI.STR141 + "<br>";
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

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, maliyet.donem.donemNo, maliyet.orgBirimKod, ENUMSurec.PerformansProgrami, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimIsGorevMaliyet tabMaliyet = new TABBirimIsGorevMaliyet(ve);
            ve.BeginTransaction();

            BirimIsGorevMaliyet[] maliyetler = Listele(ve, kullanan, maliyet);

            sonuc = tabMaliyet.Sil(maliyet);

            //Maliyeti silinmiş iş/görevlerden birimin diğer iş/görevleri ayrıştırılıp kayıt ediliyor.
            if (sonuc.islemSonuc)
                sonuc = BirimIsGorevKaydet(ve, kullanan, maliyetler, formId, false);

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

        private static Sonuc BirimIsGorevSil(VeriErisim ve, Kullanici kullanan, BirimIsGorevMaliyet maliyet, string formId)
        {
            Birim_SpGenelHavuz silKriter = new Birim_SpGenelHavuz();
            silKriter.donem = maliyet.donem;
            silKriter.orgBirimKod = maliyet.orgBirimKod;
            silKriter.spGenelHavuz = new SpGenelHavuz();
            silKriter.spGenelHavuz.kod = ((int)EnumSPGenelHavuzTur.IsGorev).ToString().PadLeft(2, '0');
            return BirimGenelHavuz.Sil(ve, kullanan, silKriter, formId);
        }

        public static BirimIsGorevMaliyet[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimIsGorevMaliyet maliyet)
        {
            return Listele(veriErisim, kullanan, maliyet, false);
        }

        public static BirimIsGorevMaliyet[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, BirimIsGorevMaliyet maliyet)
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

        private static BirimIsGorevMaliyet[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimIsGorevMaliyet maliyet, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, maliyet.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimIsGorevMaliyet tabMaliyet = new TABBirimIsGorevMaliyet(ve);

            IDataReader dr = null;
            try
            {
                dr = tabMaliyet.Listele(maliyet, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection maliyetler = new TNSCollection();
            while (dr.Read())
            {
                BirimIsGorevMaliyet gelen = new BirimIsGorevMaliyet();
                gelen.donem = maliyet.donem;
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimIsGorevMaliyet.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                gelen.isGorevKod = VeriErisim.DegerAl(dr, (int)TABBirimIsGorevMaliyet.ENUMAlanlar.ISGOREVKOD, string.Empty);
                gelen.isGorevAd = PEBServis.UzayYaziDegerGetir("STRGENELHAVUZ", gelen.isGorevKod, true, string.Empty);
                gelen.yil = VeriErisim.DegerAl(dr, (int)TABBirimIsGorevMaliyet.ENUMAlanlar.YIL, 0);
                gelen.maliyet = VeriErisim.DegerAl(dr, (int)TABBirimIsGorevMaliyet.ENUMAlanlar.MALIYET, (double)0);
                gelen.faaliyetKod = VeriErisim.DegerAl(dr, (int)TABBirimIsGorevMaliyet.ENUMAlanlar.FAALIYETKOD, string.Empty);
                gelen.faaliyetAd = PEBServis.UzayYaziDegerGetir("STRGENELHAVUZ", gelen.faaliyetKod, true, string.Empty);

                maliyetler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (BirimIsGorevMaliyet[])maliyetler.ToArray(typeof(BirimIsGorevMaliyet));
        }
    }
}