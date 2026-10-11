using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using System.Collections.Generic;

namespace TNS.PEB.IsKurallari
{
    public class STRGorevlendirmeEkip
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, GorevlendirmeEkip[] ekipler, string formId)
        {
            Sonuc sonuc = new Sonuc();

            if (ekipler == null || ekipler.Length <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR082;
                return sonuc;
            }

            foreach (GorevlendirmeEkip ekip in ekipler)
            {
                string hata = string.Empty;
                if (ekip.donem == null || ekip.donem.donemNo <= 0)
                    hata = PerformansButceResI.STR032 + "<br>";
                if (ekip.orgBirimKod <= 0)
                    hata += PerformansButceResI.STR033 + "<br>";
                if (string.IsNullOrEmpty(ekip.ad))
                    hata += PerformansButceResI.STR083;
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, ekipler[0].orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, ekipler[0].donem.donemNo, ekipler[0].orgBirimKod, ENUMSurec.SpHazirlik, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABGorevlendirmeEkip tEkip = new TABGorevlendirmeEkip(ve);
            ve.BeginTransaction();

            Gorevlendirme g = new Gorevlendirme();
            g.donem = ekipler[0].donem;
            g.orgBirimKod = ekipler[0].orgBirimKod;

            sonuc = STRGorevlendirme.Sil(ve, kullanan, g);

            if (sonuc.islemSonuc)
            {
                tEkip.Sil(ekipler[0]);
                int ekipKod = 0;
                foreach (GorevlendirmeEkip ekip in ekipler)
                {
                    ekipKod++;
                    ekip.kod = ekipKod;

                    sonuc = tEkip.Kaydet(ekip);
                    if (!sonuc.islemSonuc)
                        break;

                    foreach (Gorevlendirme gorev in ekip.gorevler)
                    {
                        gorev.donem = ekip.donem;
                        gorev.orgBirimKod = ekip.orgBirimKod;
                        gorev.ekipKod = ekip.kod;
                    }

                    sonuc = STRGorevlendirme.Kaydet(ve, kullanan, ekip.gorevler);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, ekipler[0].donem.donemNo, ekipler[0].orgBirimKod, ENUMSurec.SpHazirlik);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, GorevlendirmeEkip ekip, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (ekip.donem == null || ekip.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (ekip.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, ekip.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, ekip.donem.donemNo, ekip.orgBirimKod, ENUMSurec.SpHazirlik, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABGorevlendirmeEkip tEkip = new TABGorevlendirmeEkip(ve);
            ve.BeginTransaction();

            Gorevlendirme g = new Gorevlendirme();
            g.donem = ekip.donem;
            g.orgBirimKod = ekip.orgBirimKod;

            sonuc = STRGorevlendirme.Sil(ve, kullanan, g);

            if (sonuc.islemSonuc)
                sonuc = tEkip.Sil(ekip);

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

        public static GorevlendirmeEkip[] Listele(VeriErisim veriErisim, Kullanici kullanan, GorevlendirmeEkip ekip, bool detay)
        {
            return Listele(veriErisim, kullanan, ekip, detay, false);
        }

        public static GorevlendirmeEkip[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, GorevlendirmeEkip ekip)
        {
            string hata = string.Empty;
            if (ekip.donem == null || ekip.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (ekip.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033;
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            if (ekip.gorevler != null && ekip.gorevler.Length == 1)
            {
                ekip.gorevler[0].donem = ekip.donem;
                ekip.gorevler[0].orgBirimKod = ekip.orgBirimKod;
            }

            return Listele(veriErisim, kullanan, ekip, false, true);
        }

        private static GorevlendirmeEkip[] Listele(VeriErisim veriErisim, Kullanici kullanan, GorevlendirmeEkip ekip, bool detay, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, ekip.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGorevlendirmeEkip tEkip = new TABGorevlendirmeEkip(ve);

            IDataReader dr =  tEkip.Listele(ekip, sorgu);

            VeriErisim ve2 = null;
            if (detay || (sorgu && ekip.gorevler == null))
                ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            TNSCollection ekipler = new TNSCollection();
            if (dr != null)
            {
                while (dr.Read())
                {
                    GorevlendirmeEkip gelen = new GorevlendirmeEkip();
                    gelen.donem = new Donem();
                    gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABGorevlendirmeEkip.ENUMAlanlar.DONEMNO, 0);
                    gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABGorevlendirmeEkip.ENUMAlanlar.ORGBIRIMKOD, 0);
                    gelen.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                    gelen.kod = VeriErisim.DegerAl(dr, (int)TABGorevlendirmeEkip.ENUMAlanlar.KOD, 0);
                    gelen.ad = VeriErisim.DegerAl(dr, (int)TABGorevlendirmeEkip.ENUMAlanlar.AD, string.Empty);

                    if (detay || (sorgu && ekip.gorevler == null))
                    {
                        try
                        {
                            Gorevlendirme g = new Gorevlendirme();
                            g.donem = gelen.donem;
                            g.orgBirimKod = gelen.orgBirimKod;
                            g.ekipKod = gelen.kod;

                            gelen.gorevler = STRGorevlendirme.Listele(ve2, kullanan, g, sorgu);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    ekipler.Add(gelen);
                }
                dr.Close();
            }

            if (sorgu && ekip.gorevler != null && ekip.gorevler.Length == 1)
            {
                Gorevlendirme[] gorevler = STRGorevlendirme.Listele(ve, kullanan, ekip.gorevler[0], sorgu);
                ekipler = SorguFormVeDetayBirlestir(ekipler, gorevler);
            }

            ve.VeriErisimKapat();
            if (ve2 != null)
                ve2.VeriErisimKapat();
            return (GorevlendirmeEkip[])ekipler.ToArray(typeof(GorevlendirmeEkip));
        }

        private static TNSCollection SorguFormVeDetayBirlestir(TNSCollection formlar, Gorevlendirme[] detaylar)
        {
            TNSCollection bulunanFormlar = new TNSCollection();

            foreach (GorevlendirmeEkip form in formlar)
            {
                bool bulundu = false;
                TNSCollection bulunanDetaylar = new TNSCollection();

                foreach (Gorevlendirme detay in detaylar)
                {
                    if (form.donem.donemNo == detay.donem.donemNo && form.orgBirimKod == detay.orgBirimKod && form.kod == detay.ekipKod)
                    {
                        bulundu = true;
                        bulunanDetaylar.Add(detay);
                    }
                }

                if (bulundu)
                {
                    form.gorevler = (Gorevlendirme[])bulunanDetaylar.ToArray(typeof(Gorevlendirme));
                    bulunanFormlar.Add(form);
                }
            }

            return bulunanFormlar;
        }
    }
}