using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using TNS.BKD;
using System.Collections;

namespace TNS.PEB.IsKurallari
{
    public class STRBirimGostergeGerceklesen
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, BirimGostergeGerceklesen[] gostergeler, string formId)
        {
            Sonuc sonuc = new Sonuc();

            if (gostergeler == null || gostergeler.Length <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR138;
                return sonuc;
            }

            foreach (BirimGostergeGerceklesen gosterge in gostergeler)
            {
                string hata = string.Empty;
                if (gosterge.donem == null || gosterge.donem.donemNo <= 0)
                    hata = PerformansButceResI.STR032 + "<br>";
                if (gosterge.orgBirimKod <= 0)
                    hata += PerformansButceResI.STR033 + "<br>";
                if (gosterge.gostergeKod <= 0)
                    hata += PerformansButceResI.STR140 + "<br>";
                if (gosterge.yil <= 0)
                    hata += PerformansButceResI.STR119 + "<br>";
                if (gosterge.ay <= 0)
                    hata += PerformansButceResI.STR143 + "<br>";
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, gostergeler[0].orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, gostergeler[0].donem.donemNo, gostergeler[0].orgBirimKod, ENUMSurec.PerformansProgrami, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABSTRGosterge tBirimGostergeGercek = new TABSTRGosterge(ve);
            ve.BeginTransaction();

            foreach (BirimGostergeGerceklesen gosterge in gostergeler)
            {
                sonuc = tBirimGostergeGercek.GerceklesmeSil(gosterge.orgBirimKod, gosterge.gostergeKod, gosterge.yil, gosterge.ay);
                if (!sonuc.islemSonuc)
                    break;

                sonuc = tBirimGostergeGercek.GerceklesmeKaydet(gosterge.orgBirimKod, gosterge.gostergeKod, gosterge.yil, gosterge.ay, gosterge.gerceklesenDeger);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, gostergeler[0].donem.donemNo, gostergeler[0].orgBirimKod, ENUMSurec.PerformansProgrami);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, BirimGostergeGerceklesen gosterge, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (gosterge.donem == null || gosterge.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (gosterge.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (gosterge.yil <= 0)
                hata += PerformansButceResI.STR119 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, gosterge.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, gosterge.donem.donemNo, gosterge.orgBirimKod, ENUMSurec.PerformansProgrami, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABSTRGosterge tBirimGostergeGercek = new TABSTRGosterge(ve);
            ve.BeginTransaction();

            sonuc = tBirimGostergeGercek.GerceklesmeSil(gosterge.orgBirimKod, gosterge.gostergeKod, gosterge.yil, gosterge.ay);

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

        public static BirimGostergeGerceklesen[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimGostergeGerceklesen gosterge)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, gosterge.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSTRGosterge tBirimGostergeGercek = new TABSTRGosterge(ve);

            IDataReader dr = null;
            try
            {
                dr = tBirimGostergeGercek.GerceklesmeListele(gosterge.orgBirimKod, gosterge.yil, gosterge.ay);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection gostergeler = new TNSCollection();
            while (dr.Read())
            {
                BirimGostergeGerceklesen gelen = new BirimGostergeGerceklesen();
                gelen.donem = gosterge.donem;
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlarGERCEKLESME.BIRIMKOD, 0);
                gelen.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                gelen.gostergeKod = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlarGERCEKLESME.GOSTERGEKOD, 0);
                gelen.gostergeAd = PEBServis.UzayYaziDegerGetir("STRGOSTERGE", gelen.gostergeKod.ToString(), true, string.Empty);
                gelen.yil = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlarGERCEKLESME.YIL, 0);
                gelen.ay = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlarGERCEKLESME.AY, 0);
                gelen.gerceklesenDeger = VeriErisim.DegerAl(dr, (int)TABSTRGosterge.ENUMAlanlarGERCEKLESME.DEGER, (decimal)0);

                gostergeler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (BirimGostergeGerceklesen[])gostergeler.ToArray(typeof(BirimGostergeGerceklesen));
        }

        public static BirimGostergeGerceklesmeDurum GerceklesmeDurumBul(VeriErisim veriErisim, Kullanici kullanan, int orgBirimKod, int yil, int ay)
        {
            BirimGostergeGerceklesmeDurum gerceklesmeDurum = new BirimGostergeGerceklesmeDurum();
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, orgBirimKod);
            if (!sonuc.islemSonuc)
                return gerceklesmeDurum;

            VeriErisim ve = null;
            try
            {
                ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

                BirimHedefGostergeYillik hedeflenenKriter = new BirimHedefGostergeYillik();
                hedeflenenKriter.donem = new Donem();
                hedeflenenKriter.donem.donemNo = STRDonem.AktifDonemBul(ve, yil);
                hedeflenenKriter.orgBirimKod = orgBirimKod;
                hedeflenenKriter.yil = yil;
                BirimHedefGostergeYillik[] hedeflenenler = STRBirimHedefGostergeYillik.Listele(ve, kullanan, hedeflenenKriter);
                if (hedeflenenler == null || hedeflenenler.Length <= 0)
                {
                    ve.VeriErisimKapat();
                    return gerceklesmeDurum;
                }

                BirimGostergeGerceklesen gerceklesenKriter = new BirimGostergeGerceklesen();
                gerceklesenKriter.orgBirimKod = orgBirimKod;
                gerceklesenKriter.yil = yil;
                gerceklesenKriter.ay = ay;
                BirimGostergeGerceklesen[] gerceklesenler = Listele(ve, kullanan, gerceklesenKriter);

                Hashtable ht = new Hashtable();
                TNSCollection gerceklesmeDetayList = new TNSCollection();
                foreach (BirimHedefGostergeYillik hedeflenen in hedeflenenler)
                {
                    BirimGostergeGerceklesmeDurumDetay gerceklesmeDetay = new BirimGostergeGerceklesmeDurumDetay();
                    gerceklesmeDetay.gostergeKod = hedeflenen.gostergeKod;
                    gerceklesmeDetay.gostergeAd = hedeflenen.gostergeAd;
                    gerceklesmeDetay.hedefKod = hedeflenen.hedefKod;
                    gerceklesmeDetay.hedefAd = hedeflenen.hedefAd;
                    gerceklesmeDetay.hedeflenen = hedeflenen.hedeflenenDeger;
                    gerceklesmeDetayList.Add(gerceklesmeDetay);

                    bool gerceklesmeVar = false;

                    if (gerceklesenler != null && gerceklesenler.Length > 0)
                    {
                        foreach (BirimGostergeGerceklesen gerceklesen in gerceklesenler)
                        {
                            if (!hedeflenen.gostergeKod.Equals(gerceklesen.gostergeKod))
                                continue;

                            gerceklesmeVar = true;

                            //Artan için 1, azalan için 2 geliyor.
                            if (!ht.Contains(hedeflenen.gostergeKod))
                                ht.Add(hedeflenen.gostergeKod, PEBServis.UzaySayiDegerGetir("STRGOSTERGE", gerceklesen.gostergeKod.ToString(), true, TABSTRGosterge.ALANLAR[(int)TABSTRGosterge.ENUMAlanlar.BASARIMTURU]));

                            gerceklesmeDetay.gerceklesen = gerceklesen.gerceklesenDeger;
                            gerceklesmeDetay.basarili = GostergeBasariliMi(OrtakFonksiyonlar.ConvertToInt(ht[hedeflenen.gostergeKod], 0), hedeflenen.hedeflenenDeger, gerceklesen.gerceklesenDeger);

                            if (gerceklesmeDetay.basarili)
                                gerceklesmeDurum.basariliGerceklesmeSayi++;
                            else
                                gerceklesmeDurum.basarisizGerceklesmeSayi++;

                            break;
                        }
                    }

                    if (!gerceklesmeVar)
                    {
                        gerceklesmeDetay.basarili = false;
                        gerceklesmeDurum.basarisizGerceklesmeSayi++;
                    }
                }
                gerceklesmeDurum.detay = (BirimGostergeGerceklesmeDurumDetay[])gerceklesmeDetayList.ToArray(typeof(BirimGostergeGerceklesmeDurumDetay));
            }
            catch
            {
            }

            if (ve != null)
                ve.VeriErisimKapat();
            return gerceklesmeDurum;
        }

        public static bool GostergeBasariliMi(ENUMGostergeIlerlemeYonu basariYonu, decimal hedeflenen, decimal gerceklesen)
        {
            return GostergeBasariliMi((int)basariYonu, hedeflenen, gerceklesen);
        }

        public static bool GostergeBasariliMi(int basariYonu, decimal hedeflenen, decimal gerceklesen)
        {
            if (basariYonu == (int)ENUMGostergeIlerlemeYonu.Artan)
                return ArtanGostergeBasariliMi(hedeflenen, gerceklesen);
            else if (basariYonu == (int)ENUMGostergeIlerlemeYonu.Azalan)
                return AzalanGostergeBasariliMi(hedeflenen, gerceklesen);
            else
                return false;
        }

        private static bool ArtanGostergeBasariliMi(decimal hedeflenen, decimal gerceklesen)
        {
            return gerceklesen >= hedeflenen;
        }

        private static bool AzalanGostergeBasariliMi(decimal hedeflenen, decimal gerceklesen)
        {
            return gerceklesen <= hedeflenen;
        }
    }
}