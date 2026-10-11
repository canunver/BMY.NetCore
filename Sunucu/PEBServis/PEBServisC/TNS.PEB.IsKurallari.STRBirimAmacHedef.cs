using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using TNS.BKD;

namespace TNS.PEB.IsKurallari
{
    public class STRBirimAmacHedef
    {
        private static int siraUzunluk = 4;

        public static Sonuc OtomatikUret(VeriErisim veriErisim, Kullanici kullanan, int donemNo, int orgBirimKod, string formId)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimAmacHedef tabAmac = new TABBirimAmacHedef(ve);
            if (tabAmac.KayitVarMi(donemNo, orgBirimKod, true))
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, PerformansButceResI.STR117, string.Empty);
            }

            Birim_SpGenelHavuz kriter = new Birim_SpGenelHavuz();
            kriter.donem = new Donem();
            kriter.donem.donemNo = donemNo;
            kriter.orgBirimKod = orgBirimKod;
            kriter.spGenelHavuz = new SpGenelHavuz();
            kriter.spGenelHavuz.kod = ((int)EnumSPGenelHavuzTur.Tema).ToString().PadLeft(2, '0');

            Birim_SpGenelHavuz[] temalar = BirimGenelHavuz.Listele(ve, kullanan, kriter);
            if (temalar == null || temalar.Length <= 0)
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, PerformansButceResI.STR114, string.Empty);
            }

            Sonuc sonuc = null;
            int siraAmac = 0;
            foreach (Birim_SpGenelHavuz tema in temalar)
            {
                Birim_AmacHedef amac = new Birim_AmacHedef();
                amac.donem = new Donem();
                amac.donem.donemNo = donemNo;
                amac.orgBirimKod = orgBirimKod;
                amac.temaAmacKod = tema.spGenelHavuz.kod;
                amac.aciklama = tema.spGenelHavuz.onerilenAltMetin;
                amac.sira = (++siraAmac).ToString();
                if (string.IsNullOrEmpty(amac.aciklama))
                    amac.aciklama = "-";

                kriter.spGenelHavuz.kod = ((int)EnumSPGenelHavuzTur.SorunAlan).ToString().PadLeft(2, '0');
                kriter.spGenelHavuz.bagListe = new SpGenelHavuzBag[1];
                kriter.spGenelHavuz.bagListe[0] = new SpGenelHavuzBag();
                kriter.spGenelHavuz.bagListe[0].bagliKod.kod = tema.spGenelHavuz.kod;

                Birim_SpGenelHavuz[] sorunAlanlar = BirimGenelHavuz.Listele(ve, kullanan, kriter);
                if (sorunAlanlar != null && sorunAlanlar.Length > 0)
                {
                    TNSCollection hedefler = new TNSCollection();
                    int siraHedef = 0;
                    foreach (Birim_SpGenelHavuz sorunAlan in sorunAlanlar)
                    {
                        Birim_AmacHedef hedef = new Birim_AmacHedef();
                        hedef.sorunAlanKod = sorunAlan.spGenelHavuz.kod;
                        hedef.aciklama = sorunAlan.spGenelHavuz.onerilenAltMetin;
                        hedef.sira = (++siraHedef).ToString();
                        if (string.IsNullOrEmpty(hedef.aciklama))
                            hedef.aciklama = "-";

                        hedefler.Add(hedef);
                    }
                    amac.hedefler = (Birim_AmacHedef[])hedefler.ToArray(typeof(Birim_AmacHedef));
                }

                sonuc = Kaydet(ve, kullanan, amac, formId);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
                sonuc.bilgiStr = PerformansButceResI.STR115;
            else
                sonuc.hataStr = PerformansButceResI.STR116 + "<br>" + sonuc.hataStr;

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc KaydetAlanKontroluYap(Birim_AmacHedef bah, bool amac)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (amac)
            {
                if (bah.donem == null || bah.donem.donemNo <= 0)
                    hata = PerformansButceResI.STR032 + "<br>";
                if (bah.orgBirimKod <= 0)
                    hata += PerformansButceResI.STR033 + "<br>";
                if (string.IsNullOrEmpty(bah.temaAmacKod))
                    hata += PerformansButceResI.STR109 + "<br>";
            }
            if (string.IsNullOrEmpty(bah.aciklama))
                hata += PerformansButceResI.STR110 + "<br>";
            if (!amac && string.IsNullOrEmpty(bah.sorunAlanKod))
                hata += PerformansButceResI.STR111 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
            }

            return sonuc;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Birim_AmacHedef bah, string formId)
        {
            Sonuc sonuc = KaydetAlanKontroluYap(bah, true);
            if (!sonuc.islemSonuc)
                return sonuc;

            if (bah.hedefler != null && bah.hedefler.Length > 0)
            {
                for (int i = 0; i < bah.hedefler.Length; i++)
                {
                    sonuc = KaydetAlanKontroluYap(bah.hedefler[i], false);
                    if (!sonuc.islemSonuc)
                    {
                        sonuc.hataStr = string.Format(PerformansButceResI.STR112, i + 1) + "<br />" + sonuc.hataStr;
                        return sonuc;
                    }
                }
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, bah.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, bah.donem.donemNo, bah.orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            Birim_AmacHedef[] hedeflerVT = null;
            bah.sira = bah.sira.PadLeft(siraUzunluk, '0');

            TABBirimAmacHedef tBirimAmacHedef = new TABBirimAmacHedef(ve);
            if (tBirimAmacHedef.SiraNoKullanilmisMi(bah, true))
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = string.Format(PerformansButceResI.STR148, OrtakFonksiyonlar.ConvertToInt(bah.sira, 0));
                return sonuc;
            }

            ve.BeginTransaction();

            if (!string.IsNullOrEmpty(bah.amacHedefKod))
            {
                //Veri tabanında kayıtlı olan hedefleri buluyorum.
                //HedefSil fonksiyonuna göndereceğim.
                Birim_AmacHedef hedefKriter = new Birim_AmacHedef();
                hedefKriter.donem = bah.donem;
                hedefKriter.orgBirimKod = bah.orgBirimKod;
                hedefKriter.temaAmacKod = bah.amacHedefKod;
                hedeflerVT = Listele(ve, kullanan, hedefKriter, false, false, false);

                sonuc = tBirimAmacHedef.Guncelle(bah);
            }
            else
            {
                sonuc = tBirimAmacHedef.Kaydet(bah, true);
                bah.amacHedefKod = sonuc.anahtar;
            }

            if (sonuc.islemSonuc && bah.hedefler != null && bah.hedefler.Length > 0)
            {
                Gzft gzftKriter = new Gzft();
                gzftKriter.donem = bah.donem;
                gzftKriter.orgBirimKod = bah.orgBirimKod;
                Gzft[] gzftler = STRGzft.Listele(ve, kullanan, gzftKriter, true);

                for (int i = 0; i < bah.hedefler.Length; i++)
                {
                    Birim_AmacHedef hedef = bah.hedefler[i];
                    hedef.donem = bah.donem;
                    hedef.orgBirimKod = bah.orgBirimKod;
                    hedef.temaAmacKod = bah.amacHedefKod;
                    hedef.sira = bah.sira + "." + hedef.sira.PadLeft(siraUzunluk, '0');

                    if (tBirimAmacHedef.SiraNoKullanilmisMi(hedef, false))
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = PerformansButceResI.STR149;
                        break;
                    }

                    if (!string.IsNullOrEmpty(hedef.amacHedefKod))
                        sonuc = tBirimAmacHedef.Guncelle(hedef);
                    else
                    {
                        sonuc = tBirimAmacHedef.Kaydet(hedef, false);
                        hedef.amacHedefKod = sonuc.anahtar;
                    }

                    if (!sonuc.islemSonuc)
                        break;

                    BirimHedefSorunAlan bhsa = new BirimHedefSorunAlan();
                    bhsa.donem = hedef.donem;
                    bhsa.orgBirimKod = hedef.orgBirimKod;
                    bhsa.hedefKod = hedef.amacHedefKod;
                    bhsa.sorunAlanKod = hedef.sorunAlanKod;

                    ObjectArray bhsalar = new ObjectArray();
                    bhsalar.objeler.Add(bhsa);

                    sonuc = STRBirimHedefSorunAlan.Kaydet(ve, kullanan, bhsa.donem.donemNo, bhsa.orgBirimKod, bhsa.hedefKod, bhsalar, formId);
                    if (!sonuc.islemSonuc)
                        break;

                    if (gzftler != null && gzftler.Length > 0)
                    {
                        ObjectArray hedefGzftler = new ObjectArray();
                        foreach (GzftDetay gzftDetay in gzftler[0].gzftKonuListe)
                        {
                            if (gzftDetay.sorunAlanKod.Replace(".", "") == hedef.sorunAlanKod.Replace(".", ""))
                            {
                                BirimHedefGzft hedefGzft = new BirimHedefGzft();
                                hedefGzft.donem = gzftDetay.donem;
                                hedefGzft.orgBirimKod = gzftDetay.orgBirimKod;
                                hedefGzft.gzftTur = gzftDetay.tur;
                                hedefGzft.gzftSiraNo = gzftDetay.siraNo;
                                hedefGzft.hedefKod = hedef.amacHedefKod;

                                hedefGzftler.objeler.Add(hedefGzft);
                            }
                        }
                        if (hedefGzftler.objeler.Count > 0)
                        {
                            sonuc = STRBirimHedefGzft.Kaydet(ve, kullanan, hedef.donem.donemNo, hedef.orgBirimKod, hedef.amacHedefKod, hedefGzftler, formId);
                            if (!sonuc.islemSonuc)
                                break;
                        }
                    }
                }
            }

            if (sonuc.islemSonuc && hedeflerVT != null && hedeflerVT.Length > 0)
                sonuc = HedefSil(ve, kullanan, bah, hedeflerVT, formId);

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, bah.donem.donemNo, bah.orgBirimKod, ENUMSurec.StratejikModelleme);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = PerformansButceResI.STR010;
                sonuc.anahtar = bah.amacHedefKod;
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

        /// <summary>
        /// Kaydet ile gelmeyen fakat veri tabanında olan hedefleri bulup siler.
        /// Hedeflerin sorun alanlarını ve gzft ilişkilerini de siler.
        /// </summary>
        /// <param name="veriErisim"></param>
        /// <param name="kullanan"></param>
        /// <param name="bah"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        private static Sonuc HedefSil(VeriErisim veriErisim, Kullanici kullanan, Birim_AmacHedef bah, Birim_AmacHedef[] hedeflerVT, string formId)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            //Silmek üzere, kaydet ile gelenlerden farklı Veri tabanındaki hedefleri buluyorum.
            Birim_AmacHedef[] hedefler = null;
            if (bah.hedefler == null || bah.hedefler.Length <= 0)
                hedefler = hedeflerVT;
            else
            {
                TNSCollection silinecekHedefler = new TNSCollection();
                foreach (Birim_AmacHedef hedefVT in hedeflerVT)
                {
                    bool kayitEdilecek = false;
                    foreach (Birim_AmacHedef hedefKaydet in bah.hedefler)
                    {
                        if (hedefVT.amacHedefKod.Replace(".", "").Equals(hedefKaydet.amacHedefKod.Replace(".", "")))
                        {
                            kayitEdilecek = true;
                            break;
                        }
                    }
                    if (!kayitEdilecek)
                        silinecekHedefler.Add(hedefVT);
                }
                if (silinecekHedefler.Count <= 0)
                {
                    ve.VeriErisimKapat();
                    return sonuc;
                }
                hedefler = (Birim_AmacHedef[])silinecekHedefler.ToArray(typeof(Birim_AmacHedef));
            }

            TABBirimAmacHedef tBirimAmacHedef = new TABBirimAmacHedef(ve);
            ve.BeginTransaction();

            for (int i = 0; i < hedefler.Length; i++)
            {
                sonuc = tBirimAmacHedef.Sil(hedefler[i]);
                if (!sonuc.islemSonuc)
                    break;

                BirimHedefSorunAlan bhsa = new BirimHedefSorunAlan();
                bhsa.donem = hedefler[i].donem;
                bhsa.orgBirimKod = hedefler[i].orgBirimKod;
                bhsa.hedefKod = hedefler[i].amacHedefKod;
                sonuc = STRBirimHedefSorunAlan.Sil(ve, kullanan, bhsa, formId);
                if (!sonuc.islemSonuc)
                    break;

                BirimHedefGzft bhGzft = new BirimHedefGzft();
                bhGzft.donem = hedefler[i].donem;
                bhGzft.orgBirimKod = hedefler[i].orgBirimKod;
                bhGzft.hedefKod = hedefler[i].amacHedefKod;
                sonuc = STRBirimHedefGzft.Sil(ve, kullanan, bhGzft, formId);
                if (!sonuc.islemSonuc)
                    break;
            }

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Birim_AmacHedef bah, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (bah.donem == null || bah.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (bah.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (string.IsNullOrEmpty(bah.amacHedefKod))
                hata += PerformansButceResI.STR113 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, bah.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            //Hedeflerin sorun alanlarını silerken hedef kodları lazım olacak.
            //Bu nedenle hedefleri çekmem gerekiyor..
            Birim_AmacHedef hedefKriter = new Birim_AmacHedef();
            hedefKriter.donem = bah.donem;
            hedefKriter.orgBirimKod = bah.orgBirimKod;
            hedefKriter.temaAmacKod = bah.amacHedefKod;
            Birim_AmacHedef[] hedefler = Listele(ve, kullanan, hedefKriter, false, false, false);

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, bah.donem.donemNo, bah.orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimAmacHedef tBirimAmacHedef = new TABBirimAmacHedef(ve);
            ve.BeginTransaction();

            sonuc = tBirimAmacHedef.Sil(bah);
            if (sonuc.islemSonuc)
            {
                bah.temaAmacKod = bah.amacHedefKod;
                bah.amacHedefKod = string.Empty;
                sonuc = tBirimAmacHedef.Sil(bah);
            }

            if (sonuc.islemSonuc && hedefler != null && hedefler.Length > 0)
            {
                BirimHedefSorunAlan bhsa = new BirimHedefSorunAlan();
                bhsa.donem = bah.donem;
                bhsa.orgBirimKod = bah.orgBirimKod;

                BirimHedefGzft bhGzft = new BirimHedefGzft();
                bhGzft.donem = bah.donem;
                bhGzft.orgBirimKod = bah.orgBirimKod;

                for (int i = 0; i < hedefler.Length; i++)
                {
                    bhsa.hedefKod = hedefler[i].amacHedefKod;
                    sonuc = STRBirimHedefSorunAlan.Sil(ve, kullanan, bhsa, formId);
                    if (!sonuc.islemSonuc)
                        break;

                    bhGzft.hedefKod = hedefler[i].amacHedefKod;
                    sonuc = STRBirimHedefGzft.Sil(ve, kullanan, bhGzft, formId);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

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

        public static Birim_AmacHedef[] Listele(VeriErisim veriErisim, Kullanici kullanan, Birim_AmacHedef bah, bool detay, bool amac)
        {
            return Listele(veriErisim, kullanan, bah, detay, false, amac);
        }

        public static Birim_AmacHedef[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, Birim_AmacHedef bah, bool amac)
        {
            string hata = string.Empty;
            if (bah.donem == null || bah.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (bah.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033;
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            if (bah.hedefler != null && bah.hedefler.Length == 1)
            {
                bah.hedefler[0].donem = bah.donem;
                bah.hedefler[0].orgBirimKod = bah.orgBirimKod;
            }

            return Listele(veriErisim, kullanan, bah, false, true, amac);
        }

        private static Birim_AmacHedef[] Listele(VeriErisim veriErisim, Kullanici kullanan, Birim_AmacHedef bah, bool detay, bool sorgu, bool amac)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, bah.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimAmacHedef tBirimAmacHedef = new TABBirimAmacHedef(ve);

            IDataReader dr = null;
            dr = tBirimAmacHedef.Listele(bah, sorgu);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            VeriErisim ve2 = null;
            if ((amac && (detay || (sorgu && bah.hedefler == null))) ||
                (!amac && (detay || (sorgu && string.IsNullOrEmpty(bah.sorunAlanKod)))))
                ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            TNSCollection bahlar = new TNSCollection();
            while (dr.Read())
            {
                Birim_AmacHedef gelen = new Birim_AmacHedef();
                gelen.donem = new Donem();
                gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABBirimAmacHedef.ENUMAlanlar.DONEMNO, 0);
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimAmacHedef.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                gelen.amacHedefKod = VeriErisim.DegerAl(dr, (int)TABBirimAmacHedef.ENUMAlanlar.AMACHEDEFKOD, string.Empty);
                gelen.temaAmacKod = VeriErisim.DegerAl(dr, (int)TABBirimAmacHedef.ENUMAlanlar.TEMAAMACKOD, string.Empty);
                if (amac)
                    gelen.temaAmacAd = PEBServis.UzayYaziDegerGetir("STRGENELHAVUZ", gelen.temaAmacKod, true, string.Empty);
                else
                    gelen.temaAmacAd = PEBServis.UzayYaziDegerGetir("STRBIRIMAMACHEDEF", gelen.donem.donemNo + "-" + gelen.orgBirimKod + "-" + gelen.temaAmacKod, true, string.Empty);
                gelen.aciklama = VeriErisim.DegerAl(dr, (int)TABBirimAmacHedef.ENUMAlanlar.ACIKLAMA, string.Empty);
                gelen.sira = VeriErisim.DegerAl(dr, (int)TABBirimAmacHedef.ENUMAlanlar.SIRA, string.Empty);

                if (amac && (detay || (sorgu && bah.hedefler == null)))
                {
                    try
                    {
                        Birim_AmacHedef hedefKriter = new Birim_AmacHedef();
                        hedefKriter.donem = gelen.donem;
                        hedefKriter.orgBirimKod = gelen.orgBirimKod;
                        hedefKriter.temaAmacKod = gelen.amacHedefKod;

                        gelen.hedefler = Listele(ve2, kullanan, hedefKriter, detay, sorgu, false);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (!amac && (detay || (sorgu && string.IsNullOrEmpty(bah.sorunAlanKod))))
                {
                    BirimHedefSorunAlan bhsa = new BirimHedefSorunAlan();
                    bhsa.donem = gelen.donem;
                    bhsa.orgBirimKod = gelen.orgBirimKod;
                    bhsa.hedefKod = gelen.amacHedefKod;

                    BirimHedefSorunAlan[] bhsalar = STRBirimHedefSorunAlan.Listele(ve2, kullanan, bhsa);
                    gelen.sorunAlanKod = bhsalar[0].sorunAlanKod;
                    gelen.sorunAlanAd = bhsalar[0].sorunAlanAd;
                }

                bahlar.Add(gelen);
            }
            dr.Close();

            if (amac && sorgu && bah.hedefler != null && bah.hedefler.Length == 1)
            {
                Birim_AmacHedef[] hedefler = Listele(ve, kullanan, bah.hedefler[0], detay, sorgu, false);
                bahlar = SorguFormVeDetayBirlestir(bahlar, hedefler);
            }

            if (!amac && sorgu && !string.IsNullOrEmpty(bah.sorunAlanKod))
            {
                BirimHedefSorunAlan bhsa = new BirimHedefSorunAlan();
                bhsa.donem = bah.donem;
                bhsa.orgBirimKod = bah.orgBirimKod;
                bhsa.sorunAlanKod = bah.sorunAlanKod;

                BirimHedefSorunAlan[] bhsalar = STRBirimHedefSorunAlan.Listele(ve, kullanan, bhsa, sorgu);
                bahlar = SorguFormVeDetayBirlestir(bahlar, bhsalar);
            }

            ve.VeriErisimKapat();
            if (ve2 != null)
                ve2.VeriErisimKapat();
            return (Birim_AmacHedef[])bahlar.ToArray(typeof(Birim_AmacHedef));
        }

        private static TNSCollection SorguFormVeDetayBirlestir(TNSCollection formlar, Birim_AmacHedef[] detaylar)
        {
            TNSCollection bulunanFormlar = new TNSCollection();

            foreach (Birim_AmacHedef form in formlar)
            {
                bool bulundu = false;
                TNSCollection bulunanDetaylar = new TNSCollection();

                foreach (Birim_AmacHedef detay in detaylar)
                {
                    if (form.donem.donemNo == detay.donem.donemNo && form.orgBirimKod == detay.orgBirimKod && form.amacHedefKod == detay.temaAmacKod)
                    {
                        bulundu = true;
                        bulunanDetaylar.Add(detay);
                    }
                }

                if (bulundu)
                {
                    form.hedefler = (Birim_AmacHedef[])bulunanDetaylar.ToArray(typeof(Birim_AmacHedef));
                    bulunanFormlar.Add(form);
                }
            }

            return bulunanFormlar;
        }

        private static TNSCollection SorguFormVeDetayBirlestir(TNSCollection formlar, BirimHedefSorunAlan[] detaylar)
        {
            TNSCollection bulunanFormlar = new TNSCollection();

            foreach (Birim_AmacHedef form in formlar)
            {
                bool bulundu = false;

                foreach (BirimHedefSorunAlan detay in detaylar)
                {
                    if (form.donem.donemNo == detay.donem.donemNo && form.orgBirimKod == detay.orgBirimKod && form.amacHedefKod == detay.hedefKod)
                    {
                        bulundu = true;
                        form.sorunAlanKod = detay.sorunAlanKod;
                        form.sorunAlanAd = detay.sorunAlanAd;
                        break;
                    }
                }

                if (bulundu)
                    bulunanFormlar.Add(form);
            }

            return bulunanFormlar;
        }
    }
}