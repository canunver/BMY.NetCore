using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using TNS.BKD;

namespace TNS.PEB.IsKurallari
{
    /// <summary> 
    /// Dönem tanımlanması ile ilgili veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma, silme işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class STRBirimHedef
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, BirimHedef birimHedef, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (birimHedef.donem == null || birimHedef.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (birimHedef.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (string.IsNullOrEmpty(birimHedef.hedefKod))
                hata += PerformansButceResI.STR077;
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, birimHedef.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, birimHedef.donem.donemNo, birimHedef.orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimHedef tBirimHedef = new TABBirimHedef(ve);
            ve.BeginTransaction();

            sonuc = tBirimHedef.Guncelle(birimHedef);
            if (!sonuc.islemSonuc)
                sonuc = tBirimHedef.Kaydet(birimHedef);

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, birimHedef.donem.donemNo, birimHedef.orgBirimKod, ENUMSurec.StratejikModelleme);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, BirimHedef birimHedef, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (birimHedef.donem == null || birimHedef.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (birimHedef.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033;
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, birimHedef.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, birimHedef.donem.donemNo, birimHedef.orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimHedef tBirimHedef = new TABBirimHedef(ve);
            ve.BeginTransaction();

            sonuc = tBirimHedef.Sil(birimHedef);
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

        public static BirimHedef[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedef birimHedef)
        {
            return Listele(veriErisim, kullanan, birimHedef, false);
        }

        public static BirimHedef[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, BirimHedef birimHedef)
        {
            string hata = string.Empty;
            if (birimHedef.donem == null || birimHedef.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (birimHedef.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033;
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            bool sorgu = true;
            if (birimHedef.gzftler != null && birimHedef.gzftler.Count == 1)
            {
                BirimHedefGzft detay = (BirimHedefGzft)birimHedef.gzftler[0];
                if (detay.orgBirimKod > 0 && detay.gzftTur > 0 && detay.gzftSiraNo > 0)
                {
                    birimHedef.orgBirimKod = detay.orgBirimKod;
                    sorgu = false;
                }
            }

            TNSCollection formlar = new TNSCollection(Listele(veriErisim, kullanan, birimHedef, sorgu));

            if (birimHedef.gzftler != null && birimHedef.gzftler.Count == 1)
            {
                BirimHedefGzft detay = (BirimHedefGzft)birimHedef.gzftler[0];
                detay.donem = birimHedef.donem;
                detay.orgBirimKod = birimHedef.orgBirimKod;

                formlar = SorguFormVeDetayBirlestir(formlar, new TNSCollection(STRBirimHedefGzft.Listele(veriErisim, kullanan, detay, sorgu)));
            }

            if (birimHedef.koordineBirimler != null && birimHedef.koordineBirimler.Count == 1)
            {
                BirimHedefKoordineBirim detay = (BirimHedefKoordineBirim)birimHedef.koordineBirimler[0];
                detay.donem = birimHedef.donem;
                detay.orgBirimKod = birimHedef.orgBirimKod;

                formlar = SorguFormVeDetayBirlestir(formlar, new TNSCollection(STRBirimHedefKoordineBirim.Listele(veriErisim, kullanan, detay, sorgu)));
            }

            if (birimHedef.ustHedefler != null && birimHedef.ustHedefler.Count == 1)
            {
                BirimHedefUstHedef detay = (BirimHedefUstHedef)birimHedef.ustHedefler[0];
                detay.donem = birimHedef.donem;
                detay.orgBirimKod = birimHedef.orgBirimKod;

                formlar = SorguFormVeDetayBirlestir(formlar, new TNSCollection(STRBirimHedefUstHedef.Listele(veriErisim, kullanan, detay, sorgu)));
            }

            if (birimHedef.sorunAlanlar != null && birimHedef.sorunAlanlar.Count == 1)
            {
                BirimHedefSorunAlan detay = (BirimHedefSorunAlan)birimHedef.sorunAlanlar[0];
                detay.donem = birimHedef.donem;
                detay.orgBirimKod = birimHedef.orgBirimKod;

                formlar = SorguFormVeDetayBirlestir(formlar, new TNSCollection(STRBirimHedefSorunAlan.Listele(veriErisim, kullanan, detay, sorgu)));
            }

            if (birimHedef.ustPolitikalar != null && birimHedef.ustPolitikalar.Count == 1)
            {
                BirimHedefUstPolitika detay = (BirimHedefUstPolitika)birimHedef.ustPolitikalar[0];
                detay.donem = birimHedef.donem;
                detay.orgBirimKod = birimHedef.orgBirimKod;

                formlar = SorguFormVeDetayBirlestir(formlar, new TNSCollection(STRBirimHedefUstPolitika.Listele(veriErisim, kullanan, detay, sorgu)));
            }

            if (birimHedef.faaliyetler != null && birimHedef.faaliyetler.Count == 1)
            {
                BirimHedefFaaliyet detay = (BirimHedefFaaliyet)birimHedef.faaliyetler[0];
                detay.donem = birimHedef.donem;
                detay.orgBirimKod = birimHedef.orgBirimKod;

                formlar = SorguFormVeDetayBirlestir(formlar, new TNSCollection(STRBirimHedefFaaliyet.Listele(veriErisim, kullanan, detay, sorgu)));
            }

            return (BirimHedef[])formlar.ToArray(typeof(BirimHedef));
        }

        private static BirimHedef[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedef birimHedef, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, birimHedef.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimHedef tBirimHedef = new TABBirimHedef(ve);

            IDataReader dr = null;
            try
            {
                dr = tBirimHedef.Listele(birimHedef, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection birimHedefler = new TNSCollection();
            while (dr.Read())
            {
                BirimHedef gelen = new BirimHedef();
                gelen.donem = new Donem();
                gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABBirimHedef.ENUMAlanlar.DONEMNO, 0);
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimHedef.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.hedefKod = VeriErisim.DegerAl(dr, (int)TABBirimHedef.ENUMAlanlar.HEDEFKOD, string.Empty);
                gelen.hedefAd = PEBServis.UzayYaziDegerGetir("STRBIRIMAMACHEDEF", gelen.donem.donemNo + "-" + gelen.orgBirimKod + "-" + gelen.hedefKod, true, string.Empty);
                gelen.sorumluBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimHedef.ENUMAlanlar.SORUMLUBIRIMKOD, 0);
                gelen.sorumluBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.sorumluBirimKod.ToString(), true, string.Empty);
                gelen.aciklama = VeriErisim.DegerAl(dr, (int)TABBirimHedef.ENUMAlanlar.ACIKLAMA, string.Empty);

                birimHedefler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (BirimHedef[])birimHedefler.ToArray(typeof(BirimHedef));
        }

        private static TNSCollection SorguFormVeDetayBirlestir(TNSCollection formlar, TNSCollection detaylar)
        {
            TNSCollection bulunanFormlar = new TNSCollection();

            foreach (BirimHedef form in formlar)
            {
                bool bulundu = false;
                for (int i = 0; i < detaylar.Count; i++)
                {
                    object detay = detaylar[i];
                    if (detay.Equals(form))
                        bulundu = true;
                }
                if (bulundu)
                    bulunanFormlar.Add(form);
            }

            return bulunanFormlar;
        }
    }
}