using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using TNS.BKD;
using System.Collections;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Stratejik amaçlarla ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class STRGenelHavuz
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, SpGenelHavuz sp)
        {
            Sonuc sonuc = YetkiSTR.TanimlamaYazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (sp.tur == ((int)EnumSPGenelHavuzTur.Tema).ToString().PadLeft(2, '0') && string.IsNullOrEmpty(sp.onerilenAltMetin))
                hata = PerformansButceResI.STR134 + "<br>";
            if (sp.tur == ((int)EnumSPGenelHavuzTur.SorunAlan).ToString().PadLeft(2, '0') && string.IsNullOrEmpty(sp.onerilenAltMetin))
                hata += PerformansButceResI.STR135 + "<br>";

            int iliskiKod = (int)(EnumSPGenelHavuzIliski)System.Enum.Parse(typeof(EnumSPGenelHavuzIliski), System.Enum.GetName(typeof(EnumSPGenelHavuzTur), OrtakFonksiyonlar.ConvertToInt(sp.tur, 0)));
            if (iliskiKod > 0)
            {
                if (sp.bagListe == null || sp.bagListe.Length <= 0)
                    hata += string.Format(PerformansButceResI.STR133, PerformansButceResI.ResourceManager.GetString("STR107_" + iliskiKod.ToString().PadLeft(2, '0')));
                else
                {
                    if (sp.tur == ((int)EnumSPGenelHavuzTur.Faaliyet).ToString().PadLeft(2, '0'))
                    {
                        bool zorunluBagVar = false;
                        foreach (SpGenelHavuzBag bag in sp.bagListe)
                        {
                            if (bag.bagliKod.kod.StartsWith(iliskiKod.ToString().PadLeft(2, '0')))
                            {
                                zorunluBagVar = true;
                                break;
                            }
                        }
                        if (!zorunluBagVar)
                            hata += string.Format(PerformansButceResI.STR133, PerformansButceResI.ResourceManager.GetString("STR107_" + iliskiKod.ToString().PadLeft(2, '0')));
                    }
                }
            }

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            sonuc = BagliUstKatmanBirimTurUyumluMu(ve, kullanan, sp.bagListe, (SpGenelHavuzOrganizasyon[])sp.organizasyonListe.objeler.ToArray(typeof(SpGenelHavuzOrganizasyon)));
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABSpGenelHavuz to = new TABSpGenelHavuz(ve);

            ve.BeginTransaction();

            for (int i = 0; i < TNS.BKD.Arac.DillerLength; i++)
            {
                string dilKod = TNS.BKD.Arac.Diller(i);
                string ad = sp.absKod.Ad(dilKod);
                bool var = to.AyniAdVarmi(sp.kod, ad, i + 1, sp.tur);
                if (var)
                {
                    ve.VeriErisimKapat();

                    sonuc.islemSonuc = false;
                    sonuc.hataStr = PerformansButceResI.STR103;
                    return sonuc;
                }
            }

            bool kayitliMi = to.HavuzVarMi(sp.kod, sp.tur);
            if (kayitliMi)
            {
                if (sp.durum == 0)
                {
                    TABBirimGenelHavuz to2 = new TABBirimGenelHavuz(ve);
                    if (to2.KayitVarMi(sp.kod, DateTime.Now.Year))
                    {
                        ve.VeriErisimKapat();
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = PerformansButceResI.STR104;
                        return sonuc;
                    }


                    sp.bitisTarih = new TNSDateTime(DateTime.Now);
                }
                sonuc = to.Guncelle(sp);
            }
            else
            {
                sp.baslangicTarih = new TNSDateTime(DateTime.Now);
                sonuc = to.Kaydet(sp);
                sp.kod = sonuc.anahtar;
            }

            if (sonuc.islemSonuc)
                sonuc = KaydetOrg(ve, sp.kod, sp.tur, sp.organizasyonListe);

            if (sonuc.islemSonuc)
                sonuc = KaydetBag(ve, sp.kod, sp.bagListe);

            sonuc.anahtar = sp.kod;

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc BagliUstKatmanBirimTurUyumluMu(VeriErisim veriErisim, Kullanici kullanan, SpGenelHavuzBag[] ustKatmanlar, SpGenelHavuzOrganizasyon[] birimTurler)
        {
            Sonuc sonuc = new Sonuc();
            if (ustKatmanlar == null || ustKatmanlar.Length <= 0 || birimTurler == null || birimTurler.Length <= 0)
                return sonuc;

            string bt = string.Empty;
            foreach (SpGenelHavuzOrganizasyon birimTur in birimTurler)
            {
                if (!string.IsNullOrEmpty(bt))
                    bt += ";";
                bt += birimTur.orgKod;
            }

            SpGenelHavuz[] kiyasUstKatmanlar = Listele(veriErisim, kullanan, ustKatmanlar[0].bagliKod.kod.Substring(0, 2), EnumSPGenelHavuzDurum.Hepsi, false, bt.Split(';'));
            if (kiyasUstKatmanlar == null || kiyasUstKatmanlar.Length <= 0)
                sonuc.islemSonuc = false;

            if (sonuc.islemSonuc)
                foreach (SpGenelHavuzBag uk in ustKatmanlar)
                    foreach (SpGenelHavuz ukKiyas in kiyasUstKatmanlar)
                        if (uk.bagliKod.kod == ukKiyas.kod)
                            return sonuc;

            sonuc.islemSonuc = false;
            sonuc.hataStr = string.Format(PerformansButceResI.STR106, PerformansButceResI.ResourceManager.GetString("STR107_" + ustKatmanlar[0].bagliKod.kod.Substring(0, 2)));
            return sonuc;
        }

        private static Sonuc KaydetOrg(VeriErisim ve, string kod, string tur, ObjectArray liste)
        {
            TABSpGenelHavuz to = new TABSpGenelHavuz(ve);

            Sonuc sonuc = to.SilOrganizasyon(kod, tur);
            if (sonuc.islemSonuc)
                sonuc = to.KaydetOrganizasyon(kod, tur, liste);
            return sonuc;
        }

        private static Sonuc KaydetBag(VeriErisim ve, string kod, SpGenelHavuzBag[] liste)
        {
            TABSpGenelHavuz to = new TABSpGenelHavuz(ve);

            Sonuc sonuc = to.SilBag(kod);
            if (sonuc.islemSonuc && liste != null)
            {
                foreach (SpGenelHavuzBag bag in liste)
                {
                    sonuc = to.KaydetBag(kod, bag.bagliKod.kod);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            return sonuc;
        }

        public static SpGenelHavuz Listele(VeriErisim veriErisim, Kullanici kullanan, SpGenelHavuz sp)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSpGenelHavuz to = new TABSpGenelHavuz(ve);

            IDataReader dr = to.Listele(sp);
            SpGenelHavuz s = new SpGenelHavuz();
            if (dr.Read())
            {
                s.kod = VeriErisim.DegerAl(dr, 0, string.Empty);
                s.tur = VeriErisim.DegerAl(dr, 1, string.Empty);
                s.durum = VeriErisim.DegerAl(dr, 2, 0);
                s.baslangicTarih = VeriErisim.DegerAl(dr, 3);
                s.bitisTarih = VeriErisim.DegerAl(dr, 4);
                s.hizmetMevzuat = VeriErisim.DegerAl(dr, 5, string.Empty);
                s.hizmetFaaliyet = VeriErisim.DegerAl(dr, 6, string.Empty);
                s.projeTip = VeriErisim.DegerAl(dr, 7, string.Empty);
                s.sorunKategori = VeriErisim.DegerAl(dr, 8, string.Empty);
                s.sorunTip = VeriErisim.DegerAl(dr, 9, string.Empty);
                s.ustBelgeKategori = VeriErisim.DegerAl(dr, 10, string.Empty);
                s.onerilenAltMetin = VeriErisim.DegerAl(dr, 11, string.Empty);
                s.kurKod = PEBGenel.KurKodNoktala(VeriErisim.DegerAl(dr, 12, string.Empty));
                s.fonKod = PEBGenel.FonKodNoktala(VeriErisim.DegerAl(dr, 13, string.Empty));
                s.finKod = VeriErisim.DegerAl(dr, 14, string.Empty);
                s.ekoKod = PEBGenel.EkoKodNoktala(VeriErisim.DegerAl(dr, 15, string.Empty));
                s.pebKod = PEBGenel.PebKodNoktala(VeriErisim.DegerAl(dr, 16, string.Empty));
            }
            dr.Close();

            if (!string.IsNullOrEmpty(s.kod))
            {
                s.organizasyonListe = ListeleOrganizasyon(ve, s.kod, s.tur);
                s.bagListe = ListeleBag(ve, kullanan, s.kod);
            }

            ve.VeriErisimKapat();
            return s;
        }

        public static SpGenelHavuzBag[] ListeleBag(VeriErisim veriErisim, Kullanici kullanan, string kod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSpGenelHavuz to = new TABSpGenelHavuz(ve);
            ArrayList liste = new ArrayList();
            IDataReader dr = to.ListeleBag(kod);
            if (to.Hata() == "")
            {
                while (dr.Read())
                {
                    SpGenelHavuzBag s = new SpGenelHavuzBag();
                    s.kod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    s.bagliKod.kod = VeriErisim.DegerAl(dr, 1, string.Empty);
                    liste.Add(s);
                }
                dr.Close();

                SpGenelHavuzBag[] havuzDizi = (SpGenelHavuzBag[])liste.ToArray(typeof(SpGenelHavuzBag));

                string ustKod = ((int)(EnumSPGenelHavuzIliski)System.Enum.Parse(typeof(EnumSPGenelHavuzIliski), System.Enum.GetName(typeof(EnumSPGenelHavuzTur), OrtakFonksiyonlar.ConvertToInt(kod.Substring(1, 1), 0)))).ToString().PadLeft(2, '0');
                AbsKodAdGetir(havuzDizi, ustKod);

                if (kod.StartsWith(((int)EnumSPGenelHavuzTur.Faaliyet).ToString().PadLeft(2, '0')))
                    AbsKodAdGetir(havuzDizi, ((int)EnumSPGenelHavuzTur.Hizmet).ToString().PadLeft(2, '0'));
            }
            ve.VeriErisimKapat();

            return (SpGenelHavuzBag[])liste.ToArray(typeof(SpGenelHavuzBag));
        }

        private static ObjectArray ListeleOrganizasyon(VeriErisim veriErisim, string kod, string tur)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(null));
            TABSpGenelHavuz to = new TABSpGenelHavuz(ve);
            ObjectArray liste = new ObjectArray();
            IDataReader dr = to.ListeleOrganizasyon(kod, tur);
            if (to.Hata() == "")
            {
                while (dr.Read())
                {
                    SpGenelHavuzOrganizasyon s = new SpGenelHavuzOrganizasyon();
                    s.kod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    s.tur = VeriErisim.DegerAl(dr, 1, string.Empty);
                    s.orgKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                    liste.objeler.Add(s);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return liste;
        }

        public static SpGenelHavuz[] Listele(VeriErisim veriErisim, Kullanici kullanan, int donemNo, int orgBirimKod, string spGenelHavuzKod, string[] ustSpGenelHavuzKod, EnumSPGenelHavuzDurum durum)
        {
            string birimTur = PEBServis.UzayYaziDegerGetir("ORGBIRIM", orgBirimKod.ToString(), true, "TURKOD");
            return Listele(veriErisim, kullanan, donemNo, orgBirimKod, spGenelHavuzKod, ustSpGenelHavuzKod, durum, birimTur);
        }

        public static SpGenelHavuz[] Listele(VeriErisim veriErisim, Kullanici kullanan, string spGenelHavuzKod, EnumSPGenelHavuzDurum durum, bool altBagiOlanlar, params string[] birimTur)
        {
            return Listele(veriErisim, kullanan, 0, 0, spGenelHavuzKod, null, durum, false, altBagiOlanlar, birimTur);
        }

        public static SpGenelHavuz[] Listele(VeriErisim veriErisim, Kullanici kullanan, string spGenelHavuzKod, EnumSPGenelHavuzDurum durum, bool bagVer, bool altBagiOlanlar, params string[] birimTur)
        {
            return Listele(veriErisim, kullanan, 0, 0, spGenelHavuzKod, null, durum, bagVer, altBagiOlanlar, birimTur);
        }

        public static SpGenelHavuz[] Listele(VeriErisim veriErisim, Kullanici kullanan, string spGenelHavuzKod, string[] ustSpGenelHavuzKod, EnumSPGenelHavuzDurum durum, bool altBagiOlanlar, params string[] birimTur)
        {
            return Listele(veriErisim, kullanan, 0, 0, spGenelHavuzKod, ustSpGenelHavuzKod, durum, false, altBagiOlanlar, birimTur);
        }

        public static SpGenelHavuz[] Listele(VeriErisim veriErisim, Kullanici kullanan, int donemNo, int orgBirimKod, string spGenelHavuzKod, string[] ustSpGenelHavuzKod, EnumSPGenelHavuzDurum durum, params string[] birimTur)
        {
            return Listele(veriErisim, kullanan, donemNo, orgBirimKod, spGenelHavuzKod, ustSpGenelHavuzKod, durum, false, false, birimTur);
        }

        private static SpGenelHavuz[] Listele(VeriErisim veriErisim, Kullanici kullanan, int donemNo, int orgBirimKod, string spGenelHavuzKod, string[] ustSpGenelHavuzKod, EnumSPGenelHavuzDurum durum, bool bagVer, bool altBagiOlanlar, params string[] birimTur)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSpGenelHavuz tSPHavuz = new TABSpGenelHavuz(ve);

            IDataReader dr = null;
            try
            {
                dr = tSPHavuz.Listele(donemNo, orgBirimKod, spGenelHavuzKod, ustSpGenelHavuzKod, durum, birimTur);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }
            if (dr == null) { ve.VeriErisimKapat(); return null; }
            VeriErisim ve2 = null;
            TABSpGenelHavuz tSPHavuz2 = null;
            TABSTRGosterge tGosterge = null;
            if (bagVer || altBagiOlanlar)
            {
                ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
                tSPHavuz2 = new TABSpGenelHavuz(ve2);
                tGosterge = new TABSTRGosterge(ve2);
            }

            TNSCollection spHavuzlar = new TNSCollection();
            while (dr.Read())
            {
                SpGenelHavuz gelen = new SpGenelHavuz();
                gelen.kod = VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlar.KOD, string.Empty);
                gelen.tur = VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlar.TUR, string.Empty);

                if (altBagiOlanlar)
                {
                    int altIliski = (int)(EnumSPGenelHavuzAltIliski)System.Enum.Parse(typeof(EnumSPGenelHavuzAltIliski), System.Enum.GetName(typeof(EnumSPGenelHavuzTur), OrtakFonksiyonlar.ConvertToInt(gelen.tur, 0)));
                    if (altIliski > 0 && !tSPHavuz2.BagVarMi(altIliski.ToString().PadLeft(2, '0'), gelen.kod))
                        continue;
                    if (OrtakFonksiyonlar.ConvertToInt(gelen.tur, 0) == (int)EnumSPGenelHavuzTur.SorunAlan && !tGosterge.BagVarMi(0, gelen.kod))
                        continue;
                }

                gelen.durum = VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlar.DURUM, 0);
                gelen.baslangicTarih = VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlar.BASLANGICTARIH);
                gelen.bitisTarih = VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlar.BITISTARIH);
                gelen.hizmetMevzuat = VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlar.HIZMETMEVZUAT, string.Empty);
                gelen.hizmetFaaliyet = VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlar.HIZMETFAALIYET, string.Empty);
                gelen.projeTip = VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlar.PROJETIP, string.Empty);
                gelen.sorunKategori = VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlar.SORUNKATEGORI, string.Empty);
                gelen.sorunTip = VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlar.SORUNTIP, string.Empty);
                gelen.ustBelgeKategori = VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlar.USTBELGEKATEGORI, string.Empty);
                gelen.onerilenAltMetin = VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlar.ONERILENALTMETIN, string.Empty);
                gelen.kurKod = PEBGenel.KurKodNoktala(VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlar.KURKOD, string.Empty));
                gelen.fonKod = PEBGenel.FonKodNoktala(VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlar.FONKOD, string.Empty));
                gelen.finKod = VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlar.FINKOD, string.Empty);
                gelen.ekoKod = PEBGenel.EkoKodNoktala(VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlar.EKOKOD, string.Empty));
                gelen.pebKod = PEBGenel.PebKodNoktala(VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlar.PEBKOD, string.Empty));

                if (bagVer)
                    gelen.bagListe = ListeleBag(ve2, kullanan, gelen.kod);

                spHavuzlar.Add(gelen);
            }
            dr.Close();

            SpGenelHavuz[] havuzDizi = (SpGenelHavuz[])spHavuzlar.ToArray(typeof(SpGenelHavuz));
            AbsKodAdGetir(ref havuzDizi, spGenelHavuzKod);

            ve.VeriErisimKapat();
            if (bagVer || altBagiOlanlar)
                ve2.VeriErisimKapat();
            return havuzDizi;
        }

        private static void AbsKodAdGetir(ref SpGenelHavuz[] spHavuzlar, string ustKod)
        {
            TNSCollection siraliListe = new TNSCollection();

            IBKDServis servisBKD = TNS.BKD.Arac.Tanimla();
            ABSKodlarVAN kodAdlar = servisBKD.KodListeleSeviye(0, "22", ustKod, 2, false, true);
            if (kodAdlar.sonuc.islemSonuc)
            {
                foreach (ABSKod kodAd in kodAdlar.absKodlar)
                {
                    foreach (SpGenelHavuz spHavuz in spHavuzlar)
                    {
                        if (kodAd.kod.Replace(".", "").Equals(spHavuz.kod.Replace(".", "")))
                        {
                            spHavuz.absKod = kodAd;
                            siraliListe.Add(spHavuz);
                            break;
                        }
                    }
                }
            }

            if (siraliListe.Count > 0)
                spHavuzlar = (SpGenelHavuz[])siraliListe.ToArray(typeof(SpGenelHavuz));
        }

        private static void AbsKodAdGetir(SpGenelHavuzBag[] spHavuzlar, string ustKod)
        {
            IBKDServis servisBKD = TNS.BKD.Arac.Tanimla();
            ABSKodlarVAN kodAdlar = servisBKD.KodListeleSeviye(0, "22", ustKod, 2);
            if (kodAdlar.sonuc.islemSonuc)
                foreach (ABSKod kodAd in kodAdlar.absKodlar)
                    foreach (SpGenelHavuzBag spHavuz in spHavuzlar)
                        if (kodAd.kod.Replace(".", "").Equals(spHavuz.bagliKod.kod.Replace(".", "")))
                            spHavuz.bagliKod = kodAd;
        }

        public static SpGenelHavuzBag[] BirimdeKullanilmislarinBaglariniListele(VeriErisim veriErisim, Kullanici kullanan, int donemNo, int orgBirimKod, string spGenelHavuzKod, params string[] ustSpGenelHavuzKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSpGenelHavuz to = new TABSpGenelHavuz(ve);

            IDataReader dr = null;
            try
            {
                dr = to.BirimdeKullanilmislarinBaglariniListele(donemNo, orgBirimKod, spGenelHavuzKod, ustSpGenelHavuzKod);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            ArrayList liste = new ArrayList();
            while (dr.Read())
            {
                SpGenelHavuzBag s = new SpGenelHavuzBag();
                s.bagliKod.kod = VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlarBag.KOD, string.Empty);
                s.kod = VeriErisim.DegerAl(dr, (int)TABSpGenelHavuz.ENUMAlanlarBag.BAGLIKOD, string.Empty);
                liste.Add(s);
            }
            dr.Close();

            SpGenelHavuzBag[] havuzDizi = (SpGenelHavuzBag[])liste.ToArray(typeof(SpGenelHavuzBag));
            AbsKodAdGetir(havuzDizi, string.Empty);

            ve.VeriErisimKapat();
            return (SpGenelHavuzBag[])liste.ToArray(typeof(SpGenelHavuzBag));
        }
    }
}