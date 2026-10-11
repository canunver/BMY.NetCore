using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    public class STRAsamaKontrol
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, AsamaKontrol askon, ENUMAsamaKontrolIslemTur islem)
        {
            return Kaydet(veriErisim, kullanan, askon, islem, true);
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, AsamaKontrol askon, ENUMAsamaKontrolIslemTur islem, bool detayVar)
        {
            Sonuc sonuc = new Sonuc();

            int kaydetSayac = 0;
            int reddetSayac = 0;

            string hata = string.Empty;
            if (askon.donem == null || askon.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (askon.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (askon.konuNo <= 0)
                hata += PerformansButceResI.STR067 + "<br>";
            if (detayVar && (askon.askonKonuListe == null || askon.askonKonuListe.Length <= 0))
                hata += PerformansButceResI.STR074 + "<br>";
            else if (detayVar)
            {
                foreach (AsamaKontrolDetay askondetay in askon.askonKonuListe)
                {
                    if (islem == ENUMAsamaKontrolIslemTur.Kaydet && askondetay.birimOnayi)
                        kaydetSayac++;
                    else if (islem == ENUMAsamaKontrolIslemTur.BirimOnay && !askondetay.birimOnayi)
                    {
                        hata += PerformansButceResI.STR068 + "<br>";
                        break;
                    }
                    else if (islem == ENUMAsamaKontrolIslemTur.BirimRed && !askondetay.birimOnayi)
                    {
                        if (askondetay.birimAciklama == "")
                        {
                            hata += PerformansButceResI.STR069 + "<br>";
                            break;
                        }
                        reddetSayac++;
                    }
                    else if (islem == ENUMAsamaKontrolIslemTur.UstBirimOnay && !askondetay.ustBirimOnayi)
                    {
                        hata += PerformansButceResI.STR068 + "<br>";
                        break;
                    }
                    else if (islem == ENUMAsamaKontrolIslemTur.UstBirimRed && !askondetay.ustBirimOnayi)
                    {
                        if (askondetay.ustBirimAciklama == "")
                        {
                            hata += PerformansButceResI.STR069 + "<br>";
                            break;
                        }
                        askondetay.birimOnayi = askondetay.ustBirimOnayi;
                        reddetSayac++;
                    }
                }
            }
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            askon.statu = AsamaKontrol.StatuBul(islem);
            if (islem == ENUMAsamaKontrolIslemTur.Kaydet)
                askon.baslamaTarihi = new TNSDateTime(DateTime.Now);
            else if (islem == ENUMAsamaKontrolIslemTur.UstBirimOnay)
                askon.tamamlanmaTarihi = new TNSDateTime(DateTime.Now);

            if (detayVar)
            {
                if ((islem == ENUMAsamaKontrolIslemTur.BirimRed || islem == ENUMAsamaKontrolIslemTur.UstBirimRed) && reddetSayac == 0)
                    hata += PerformansButceResI.STR070 + "<br>";

                if (kaydetSayac == askon.askonKonuListe.Length)
                    askon.statu = ENUMAsamaKontrolStatu.Onayda;
            }
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            if (islem == ENUMAsamaKontrolIslemTur.Kaydet)
                sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, askon.orgBirimKod);
            else if (islem == ENUMAsamaKontrolIslemTur.BirimOnay || islem == ENUMAsamaKontrolIslemTur.BirimRed)
                sonuc = YetkiSTR.OnaylamaHakkiVarMi(kullanan, askon.orgBirimKod, false);
            else if (islem == ENUMAsamaKontrolIslemTur.UstBirimOnay || islem == ENUMAsamaKontrolIslemTur.UstBirimRed)
                sonuc = YetkiSTR.UstOnayaHakkiVarMi(kullanan, askon.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            if (askon.islemYapanBirim <= 0)
                askon.islemYapanBirim = askon.orgBirimKod;
            askon.islemTarihi = new TNSDateTime(DateTime.Now);
            askon.islemYapan = kullanan.kullaniciKodu;

            sonuc = Kaydet(ve, kullanan, islem, askon, detayVar);
            if (sonuc.islemSonuc)
            {
                sonuc = KaydetGecmis(ve, kullanan, new AsamaKontrolGecmis(askon), islem);
                sonuc.anahtar = ((int)askon.statu).ToString();
            }

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

        private static Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, ENUMAsamaKontrolIslemTur islem, AsamaKontrol askon, bool detayVar)
        {
            TABAsamaKontrol to = new TABAsamaKontrol(ve);

            Sonuc sonuc;
            if (to.AsamaKontrolKayitVarmi(askon))
                sonuc = to.Guncelle(askon);
            else
                sonuc = to.Kaydet(askon);

            if (!sonuc.islemSonuc || !detayVar)
                return sonuc;

            to.SilDetay(askon);
            foreach (AsamaKontrolDetay askonDetay in askon.askonKonuListe)
            {
                sonuc = to.KaydetDetay(askonDetay);
                if (!sonuc.islemSonuc)
                    break;
            }

            return sonuc;
        }

        public static Sonuc KaydetGecmis(VeriErisim veriErisim, Kullanici kullanan, AsamaKontrolGecmis asamaKontrolGecmis, ENUMAsamaKontrolIslemTur islem)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (asamaKontrolGecmis.donem == null || asamaKontrolGecmis.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (asamaKontrolGecmis.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (asamaKontrolGecmis.konuNo <= 0)
                hata += PerformansButceResI.STR067 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, asamaKontrolGecmis.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABAsamaKontrol to = new TABAsamaKontrol(ve);

            asamaKontrolGecmis.islem = PerformansButceResI.ResourceManager.GetString("STR073_" + (int)islem);
            asamaKontrolGecmis.islemiYapan = kullanan.kullaniciKodu;
            asamaKontrolGecmis.tarih = new TNSDateTime(DateTime.Now);

            sonuc = to.KaydetGecmis(asamaKontrolGecmis);

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

        public static AsamaKontrol[] Listele(VeriErisim veriErisim, Kullanici kullanan, AsamaKontrol askon)
        {
            return Listele(veriErisim, kullanan, askon, false);
        }

        public static AsamaKontrol[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, AsamaKontrol askon)
        {
            string hata = string.Empty;
            if (askon.donem == null || askon.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (askon.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (askon.konuNo <= 0)
                hata += PerformansButceResI.STR067;
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            if (askon.askonKonuListe != null && askon.askonKonuListe.Length == 1)
            {
                askon.askonKonuListe[0].donem = askon.donem;
                askon.askonKonuListe[0].orgBirimKod = askon.orgBirimKod;
                askon.askonKonuListe[0].konuNo = askon.konuNo;
            }

            return Listele(veriErisim, kullanan, askon, true);
        }

        private static AsamaKontrol[] Listele(VeriErisim veriErisim, Kullanici kullanan, AsamaKontrol askon, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, askon.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAsamaKontrol to = new TABAsamaKontrol(ve);

            IDataReader dr = null;
            try
            {
                dr = to.Listele(askon, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection askonListe = new TNSCollection();
            if (dr != null)
            {
                VeriErisim ve2 = null;
                if (!sorgu || (sorgu && askon.askonKonuListe == null))
                    ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

                while (dr.Read())
                {
                    AsamaKontrol gelen = new AsamaKontrol();
                    gelen.donem = new Donem();
                    gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlar.DONEMNO, 0);
                    gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlar.ORGBIRIMKOD, 0);
                    gelen.konuNo = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlar.KONUNO, 0);
                    gelen.statu = AsamaKontrol.StatuBul(VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlar.STATU, 0));
                    gelen.baslamaTarihi = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlar.BASLAMATARIHI);
                    gelen.tamamlanmaTarihi = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlar.TAMAMLANMATARIHI);
                    gelen.islemYapanBirim = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlar.ISLEMYAPANBIRIM, 0);
                    gelen.islemTarihi = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlar.ISLEMTARIHI);
                    gelen.islemYapan = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlar.ISLEMYAPAN, string.Empty);

                    if (!sorgu || (sorgu && askon.askonKonuListe == null))
                    {
                        try
                        {
                            AsamaKontrolDetay d = new AsamaKontrolDetay();
                            d.donem = gelen.donem;
                            d.orgBirimKod = gelen.orgBirimKod;
                            d.konuNo = gelen.konuNo;

                            gelen.askonKonuListe = ListeleDetay(ve2, d, sorgu);
                        }
                        catch (Exception e)
                        {
                            ve.VeriErisimKapat();
                            ve2.VeriErisimKapat();
                            throw e;
                        }
                    }

                    askonListe.Add(gelen);
                }
                dr.Close();

                if (ve2 != null)
                    ve2.VeriErisimKapat();

                if (sorgu && askon.askonKonuListe != null && askon.askonKonuListe.Length == 1)
                {
                    AsamaKontrolDetay[] askonDetay = ListeleDetay(ve, askon.askonKonuListe[0], sorgu);
                    askonListe = SorguFormVeDetayBirlestir(askonListe, askonDetay);
                }
            }

            ve.VeriErisimKapat();
            return (AsamaKontrol[])askonListe.ToArray(typeof(AsamaKontrol));
        }

        private static TNSCollection SorguFormVeDetayBirlestir(TNSCollection formlar, AsamaKontrolDetay[] detaylar)
        {
            TNSCollection bulunanFormlar = new TNSCollection();

            foreach (AsamaKontrol form in formlar)
            {
                bool bulundu = false;
                TNSCollection bulunanDetaylar = new TNSCollection();

                foreach (AsamaKontrolDetay detay in detaylar)
                {
                    if (form.donem.donemNo == detay.donem.donemNo && form.orgBirimKod == detay.orgBirimKod && form.konuNo == detay.konuNo)
                    {
                        bulundu = true;
                        bulunanDetaylar.Add(detay);
                    }
                }

                if (bulundu)
                {
                    form.askonKonuListe = (AsamaKontrolDetay[])bulunanDetaylar.ToArray(typeof(AsamaKontrolDetay));
                    bulunanFormlar.Add(form);
                }
            }

            return bulunanFormlar;
        }

        private static AsamaKontrolDetay[] ListeleDetay(VeriErisim ve, AsamaKontrolDetay askonDetay, bool sorgu)
        {
            TABAsamaKontrol to = new TABAsamaKontrol(ve);

            IDataReader dr = null;
            try
            {
                dr = to.ListeleDetay(askonDetay, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(PerformansButceResI.STR076);
            }

            TNSCollection liste = new TNSCollection();
            while (dr.Read())
            {
                AsamaKontrolDetay detay = new AsamaKontrolDetay();
                detay.donem = new Donem();
                detay.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlarDetay.DONEMNO, 0);
                detay.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlarDetay.ORGBIRIMKOD, 0);
                detay.konuNo = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlarDetay.KONUNO, 0);
                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlarDetay.SIRANO, 0);
                detay.kontrolListesi = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlarDetay.KONTROLLISTESI, string.Empty);
                detay.birimOnayi = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlarDetay.BIRIMONAYI, 0) == 1 ? true : false;
                detay.birimAciklama = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlarDetay.BIRIMACIKLAMA, string.Empty);
                detay.ustBirimOnayi = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlarDetay.USTBIRIMONAYI, 0) == 1 ? true : false;
                detay.ustBirimAciklama = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlarDetay.USTBIRIMACIKLAMA, string.Empty);

                liste.Add(detay);
            }
            dr.Close();

            return (AsamaKontrolDetay[])liste.ToArray(typeof(AsamaKontrolDetay));
        }

        public static AsamaKontrolGecmis[] ListeleGecmis(VeriErisim ve, Kullanici kullanan, AsamaKontrolGecmis askongecmis)
        {
            string hata = string.Empty;
            if (askongecmis.donem == null || askongecmis.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (askongecmis.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (askongecmis.konuNo <= 0)
                hata += PerformansButceResI.STR067 + "<br>";
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, askongecmis.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim veriErisim = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAsamaKontrol to = new TABAsamaKontrol(veriErisim);

            IDataReader dr = null;
            try
            {
                dr = to.ListeleGecmis(askongecmis);
            }
            catch (Exception e)
            {
                throw e;
            }

            TNSCollection liste = new TNSCollection();

            if (dr == null)
                throw new Exception(PerformansButceResI.STR076);

            while (dr.Read())
            {
                AsamaKontrolGecmis detay = new AsamaKontrolGecmis();
                detay.donem = new Donem();
                detay.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlarGecmis.DONEMNO, 0);
                detay.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlarGecmis.ORGBIRIMKOD, 0);
                detay.konuNo = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlarGecmis.KONUNO, 0);
                detay.islem = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlarGecmis.ISLEM, string.Empty);
                detay.islemiYapan = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlarGecmis.ISLEMIYAPAN, string.Empty);
                detay.tarih = VeriErisim.DegerAl(dr, (int)TABAsamaKontrol.ENUMAlanlarGecmis.TARIH);

                liste.Add(detay);
            }
            dr.Close();

            return (AsamaKontrolGecmis[])liste.ToArray(typeof(AsamaKontrolGecmis));
        }
    }
}