using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    public class TMMHesapPlaniBarkod
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TasHesapPlaniBarkod hpBarkod)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (string.IsNullOrEmpty(hpBarkod.barkod))
                hata = "Barkod alanı boş bırakılamaz!<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHesapPlaniBarkod tabHPR = new TABHesapPlaniBarkod(ve);
            ve.BeginTransaction();

            try
            {
                if (hpBarkod.tasBarkodID != 0 && hpBarkod.tasBarkodID > 0)
                    sonuc = tabHPR.Guncelle(hpBarkod);
                else
                    sonuc = tabHPR.Kaydet(hpBarkod);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, TasHesapPlaniBarkod hpBarkod)
        {
            ObjectArray listeVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABHesapPlaniBarkod tabHPR = new TABHesapPlaniBarkod(ve);
            IDataReader dr = null;

            try
            {
                dr = tabHPR.Listele(hpBarkod);
            }
            catch (Exception e)
            {
                listeVAN.sonuc.islemSonuc = false;
                listeVAN.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return listeVAN;
            }

            while (dr.Read())
            {
                TasHesapPlaniBarkod gelen = new TasHesapPlaniBarkod();
                gelen.tasBarkodID = VeriErisim.DegerAl(dr, 0, 0);
                gelen.tasHesapNo = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.barkod = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.muhasebeKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.harcamaBirimKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                listeVAN.objeler.Add(gelen);
            }

            if (listeVAN.objeler.Count == 0)
                listeVAN.sonuc.bilgiStr = TasinirMalResI.TMM029;

            dr.Close();
            ve.VeriErisimKapat();
            return listeVAN;
        }

        public static Sonuc Sil(VeriErisim veriErisim, string barkodListe)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABHesapPlaniBarkod tabHPR = new TABHesapPlaniBarkod(ve);

            ve.BeginTransaction();

            try
            {
                foreach (var barkodID in barkodListe.TrimEnd(';').Split(';'))
                {

                    sonuc = tabHPR.Sil(barkodID);
                    if (!sonuc.islemSonuc)
                        break;
                }

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();

            }

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}