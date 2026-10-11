using System;
using System.Data;
using TNS.KYM;
using TNS.PEB.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.PEB.IsKurallari
{
    public class STRTarihselGelisim
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TarihselGelisim tg, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (tg.donem == null || tg.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (tg.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (string.IsNullOrEmpty(tg.aciklama))
                hata += PerformansButceResI.STR052 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, tg.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, tg.donem.donemNo, tg.orgBirimKod, ENUMSurec.MevcutDurumAnalizi, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABTarihselGelisim tabTG = new TABTarihselGelisim(ve);
            ve.BeginTransaction();

            tabTG.Sil(tg);

            int karakterSayi = 1000;
            string aciklama = tg.aciklama;
            while (!string.IsNullOrEmpty(aciklama))
            {
                tg.siraNo++;

                if (aciklama.Length > karakterSayi)
                {
                    tg.aciklama = aciklama.Substring(0, karakterSayi);
                    aciklama = aciklama.Substring(karakterSayi);
                }
                else
                {
                    tg.aciklama = aciklama;
                    aciklama = string.Empty;
                }

                sonuc = tabTG.Kaydet(tg);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, tg.donem.donemNo, tg.orgBirimKod, ENUMSurec.MevcutDurumAnalizi);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TarihselGelisim tg, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (tg.donem == null || tg.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (tg.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, tg.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, tg.donem.donemNo, tg.orgBirimKod, ENUMSurec.MevcutDurumAnalizi, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABTarihselGelisim tabTG = new TABTarihselGelisim(ve);
            ve.BeginTransaction();

            sonuc = tabTG.Sil(tg);
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

        public static TarihselGelisim Listele(VeriErisim veriErisim, Kullanici kullanan, TarihselGelisim tg)
        {
            TarihselGelisim[] tgler = Listele(veriErisim, kullanan, tg, false);
            if (tgler.Length > 0)
                return tgler[0];
            return null;

        }

        public static TarihselGelisim[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, TarihselGelisim tg)
        {
            return Listele(veriErisim, kullanan, tg, true);
        }

        private static TarihselGelisim[] Listele(VeriErisim veriErisim, Kullanici kullanan, TarihselGelisim tg, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, tg.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            string hata = string.Empty;
            if (tg.donem == null || tg.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (tg.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTarihselGelisim tabTG = new TABTarihselGelisim(ve);

            IDataReader dr = null;
            try
            {
                dr = tabTG.Listele(tg, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection tgler = new TNSCollection();
            while (dr.Read())
            {
                int donemNo = VeriErisim.DegerAl(dr, (int)TABTarihselGelisim.ENUMAlanlar.DONEMNO, 0);
                int orgBirimKod = VeriErisim.DegerAl(dr, (int)TABTarihselGelisim.ENUMAlanlar.ORGBIRIMKOD, 0);

                TarihselGelisim gelen = TarihselGelisimBul(tgler, donemNo, orgBirimKod);
                if (gelen == null)
                {
                    gelen = new TarihselGelisim();
                    gelen.donem = new Donem();
                    gelen.donem.donemNo = donemNo;
                    gelen.orgBirimKod = orgBirimKod;

                    tgler.Add(gelen);
                }

                gelen.aciklama += VeriErisim.DegerAl(dr, (int)TABTarihselGelisim.ENUMAlanlar.ACIKLAMA, string.Empty);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (TarihselGelisim[])tgler.ToArray(typeof(TarihselGelisim));
        }

        private static TarihselGelisim TarihselGelisimBul(TNSCollection tgler, int donemNo, int orgBirimKod)
        {
            foreach (TarihselGelisim tg in tgler)
                if (tg.donem.donemNo == donemNo && tg.orgBirimKod == orgBirimKod)
                    return tg;
            return null;
        }
    }
}