using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    public class STRStratejiBelirleme
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, StratejiBelirleme[] stratejiler, string amacKod, string formId)
        {
            Sonuc sonuc = new Sonuc();

            if (stratejiler == null || stratejiler.Length <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR080;
                return sonuc;
            }

            foreach (StratejiBelirleme sb in stratejiler)
            {
                string hata = string.Empty;
                if (sb.donem == null || sb.donem.donemNo <= 0)
                    hata = PerformansButceResI.STR032 + "<br>";
                if (sb.orgBirimKod <= 0)
                    hata += PerformansButceResI.STR033 + "<br>";
                if (string.IsNullOrEmpty(sb.hedefKod))
                    hata += PerformansButceResI.STR077 + "<br>";
                if (string.IsNullOrEmpty(sb.stratejiAd))
                    hata += PerformansButceResI.STR078;// +"<br>";
                //if (string.IsNullOrEmpty(sb.stratejiAciklama))
                //    hata += PerformansButceResI.STR079;
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, stratejiler[0].orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, stratejiler[0].donem.donemNo, stratejiler[0].orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABStratejiBelirleme tStratejiBelirleme = new TABStratejiBelirleme(ve);
            ve.BeginTransaction();

            int siraNo = 0;
            tStratejiBelirleme.Sil(stratejiler[0], amacKod);
            foreach (StratejiBelirleme sb in stratejiler)
            {
                sb.siraNo = ++siraNo;

                sonuc = tStratejiBelirleme.Kaydet(sb);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, stratejiler[0].donem.donemNo, stratejiler[0].orgBirimKod, ENUMSurec.StratejikModelleme);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, StratejiBelirleme strateji, string amacKod, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (strateji.donem == null || strateji.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (strateji.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, strateji.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, strateji.donem.donemNo, strateji.orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABStratejiBelirleme tStrateji = new TABStratejiBelirleme(ve);
            ve.BeginTransaction();

            sonuc = tStrateji.Sil(strateji, amacKod);
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

        public static StratejiBelirleme[] Listele(VeriErisim veriErisim, Kullanici kullanan, StratejiBelirleme strateji)
        {
            return Listele(veriErisim, kullanan, strateji, false);
        }

        public static StratejiBelirleme[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, StratejiBelirleme strateji)
        {
            string hata = string.Empty;
            if (strateji.donem == null || strateji.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (strateji.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033;
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            return Listele(veriErisim, kullanan, strateji, true);
        }

        private static StratejiBelirleme[] Listele(VeriErisim veriErisim, Kullanici kullanan, StratejiBelirleme strateji, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, strateji.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABStratejiBelirleme tStrateji = new TABStratejiBelirleme(ve);

            IDataReader dr = null;
            try
            {
                dr = tStrateji.Listele(strateji, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection stratejiler = new TNSCollection();
            while (dr.Read())
            {
                StratejiBelirleme gelen = new StratejiBelirleme();
                gelen.donem = strateji.donem;
                gelen.orgBirimKod = strateji.orgBirimKod;
                gelen.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                gelen.hedefKod = VeriErisim.DegerAl(dr, (int)TABStratejiBelirleme.ENUMAlanlar.HEDEFKOD, string.Empty);
                gelen.hedefAd = PEBServis.UzayYaziDegerGetir("STRBIRIMAMACHEDEF", gelen.donem.donemNo + "-" + gelen.orgBirimKod + "-" + gelen.hedefKod, true, string.Empty);
                gelen.siraNo = VeriErisim.DegerAl(dr, (int)TABStratejiBelirleme.ENUMAlanlar.SIRANO, 0);
                gelen.stratejiAd = VeriErisim.DegerAl(dr, (int)TABStratejiBelirleme.ENUMAlanlar.STRATEJIAD, string.Empty);
                gelen.stratejiAciklama = VeriErisim.DegerAl(dr, (int)TABStratejiBelirleme.ENUMAlanlar.STRATEJIACIKLAMA, string.Empty);

                stratejiler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (StratejiBelirleme[])stratejiler.ToArray(typeof(StratejiBelirleme));
        }
    }
}