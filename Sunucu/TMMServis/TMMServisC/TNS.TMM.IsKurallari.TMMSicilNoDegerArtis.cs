using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    public class TMMSicilNoDegerArtis
    {
        public static Sonuc Kaydet(Kullanici kullanan, SicilNoDegerArtis bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            //sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, bilgi.muhasebeKod, bilgi.harcamaKod, "");
            //if (!sonuc.islemSonuc)
            //{
            //    sonuc.hataStr = TasinirMalResI.TMM050;
            //    return sonuc;
            //}

            sonuc = Kontrol(kullanan, bilgi);
            if (sonuc.hataStr != String.Empty)
                return sonuc;

            ////////////////////////SICIL NO ELLE GIRILDIYSE KONTROL ICIN ////////////
            SicilNoHareket s = new SicilNoHareket();
            s.sicilNo = bilgi.gorSicilNo;
            s.muhasebeKod = bilgi.muhasebeKod;
            s.harcamaBirimKod = bilgi.harcamaKod;

            ObjectArray sicilNo = TMMSicilNoHareket.KontrolSicilNo(null, kullanan, s);

            if (sicilNo.objeler.Count == 0)
            {
                sonuc.hataStr = "Girilen Sicil No Bulunamadı.";
                sonuc.islemSonuc = false;
                return sonuc;
            }
            else if (sicilNo.objeler.Count > 0)
            {
                SicilNoHareket tf = (SicilNoHareket)sicilNo[0];

                bilgi.prSicilNo = tf.prSicilNo;
            }
            ////////////////////////SICIL NO KONTROL ICIN ////////////


            if (bilgi.tur == 3 && Arac.MerkezBankasiKullaniyor())
            {
                //decimal sonBedel = TMMSicilNoHareket.SonBedelGetir(null, kullanan, OrtakFonksiyonlar.ConvertToInt(bilgi.gorSicilNo, 0));
                //if (Math.Abs(bilgi.tutar) > (double)sonBedel)
                //    sonuc.hataStr += string.Format("Değer azalış fiyatı {0} fazlası olmamaz.", sonBedel.ToString("#,###.0")) + "<br>";
            }

            //Amortisman Bittimi Kontrolü *******************************************************************
            AmortismanKriter ak = new AmortismanKriter
            {
                yil = bilgi.belgeTarihi.Yil,
                prSicilNo = bilgi.prSicilNo,
                muhasebeKod = bilgi.muhasebeKod,
                harcamaKod = bilgi.harcamaKod,
                uygulamaTur = (int)ENUMAmortismanUygulamaTur.KRITERLEREGORE,
                raporTur = 6
            };
            ak.donem = TMMAmortisman.AmortismanDonemiBul(bilgi.harcamaKod, bilgi.belgeTarihi);

            ObjectArray aListe = TMMAmortisman.AmortismanRaporla3(null, kullanan, ak);
            AmortismanRapor3 ar = aListe.objeler.Count > 0 ? (AmortismanRapor3)aListe.objeler[0] : new AmortismanRapor3();
            if (ar.amortismanSuresiBittiMi)
                sonuc.hataStr += "Amortisman tamamlandığı için değer düzelme yapılamaz.<br>";
            //*******************************************************************************************************

            if (sonuc.hataStr != String.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGenel tabGenel = new TABGenel(ve);
            TABSicilNoDegerArtis to = new TABSicilNoDegerArtis(ve);

            if (bilgi.kod != "")
            {
                SicilNoDegerArtis t = new SicilNoDegerArtis();
                t.kod = bilgi.kod;
                ObjectArray liste = Listele(ve, kullanan, t);
                SicilNoDegerArtis tf = (SicilNoDegerArtis)liste[0];

                if (tf.durum == (int)ENUMBelgeDurumu.ONAYLI)
                {
                    sonuc.hataStr = "Belge onaylı işlem yapılamaz.";
                    sonuc.islemSonuc = false;
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }
            else
            {
                SicilNoDegerArtis t = new SicilNoDegerArtis();
                t.prSicilNo = bilgi.prSicilNo;
                ObjectArray liste = Listele(ve, kullanan, t);

                foreach (SicilNoDegerArtis form in liste.objeler)
                {
                    if (form.durum != (int)ENUMBelgeDurumu.ONAYLI)
                    {
                        sonuc.hataStr = "Onaylanmamış kayıtlar var. Lütfen yeni kayıt işleminde önce onaysız belgeyi onaylayınız.";
                        sonuc.islemSonuc = false;
                        ve.VeriErisimKapat();
                        return sonuc;
                    }
                }

            }

            if (bilgi.tur == 1 && bilgi.katsayi != 0)
            {
                double sonBedel = (double)TMMSicilNoHareket.SonBedelGetir(ve, kullanan, bilgi.prSicilNo);
                bilgi.tutar = sonBedel * (bilgi.katsayi / 100);
            }

            ve.BeginTransaction();

            if (bilgi.kod != "")
                sonuc = to.Guncelle(bilgi);
            else
            {
                string sayacKod = "SICILDEGERARTIS" + bilgi.muhasebeKod + bilgi.harcamaKod.Replace(".", "");
                bilgi.kod = Guid.NewGuid().ToString().Replace("-", "");
                bilgi.belgeNo = tabGenel.BelgeNoAl(sayacKod, "000000");
                bilgi.durum = (int)ENUMBelgeDurumu.YENI;

                bilgi.harcamaKod = bilgi.harcamaKod.Replace(".", "");
                sonuc = to.Kaydet(bilgi);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = bilgi.kod + "," + bilgi.belgeNo;
                sonuc.bilgiStr = TasinirMalResI.TMM118;

                ve.Commit();

                ////EĞER ENFLASYON İSE İŞLEM YAP (Not : Değer azalış enflasyon gibi çalışmalı)
                //if ((bilgi.tur == 1 || bilgi.tur == 3) && bilgi.sorguEskiSicilNo != "-1")//Enflasyon Düzeltme Farkı (BEDEL DÜZELTME FARKI) sorguEskiSicilNo -1 ise aktarım içindir.
                //{
                //    AmortismanKriter ak = new AmortismanKriter();
                //    ak.yil = bilgi.belgeTarihi.Yil;
                //    ak.prSicilNo = bilgi.prSicilNo;
                //    ak.muhasebeKod = bilgi.muhasebeKod;
                //    ak.harcamaKod = bilgi.harcamaKod;
                //    ak.uygulamaTur = (int)ENUMAmortismanUygulamaTur.KRITERLEREGORE;
                //    ak.raporTur = 6;
                //    //ak.donem = (bilgi.belgeTarihi.Ay - 1) / 3;
                //    ak.donem = TMMAmortisman.AmortismanDonemiBul(ak.harcamaKod, bilgi.belgeTarihi);

                //    ObjectArray aListe = TMMAmortisman.AmortismanRaporla2(null, kullanan, ak);

                //    double simdikiArtisTutar = bilgi.tutar;
                //    foreach (AmortismanRapor ar in aListe.objeler)
                //    {
                //        decimal sonBedel = ar.maliyetTutar + ar.degerArtisTutar - (decimal)simdikiArtisTutar;

                //        decimal oran = (decimal)simdikiArtisTutar / sonBedel;

                //        decimal simdiyeKadarYapilanAmortisman = ar.toplamAmortismanTutar; //ar.toplamAmortismanBirikmisTutar;

                //        decimal yeniAmortismanTutari = simdiyeKadarYapilanAmortisman * oran;

                //        string amortismanKod = "";//Kayıt edilirlen üretiliyor

                //        if (yeniAmortismanTutari != 0)
                //            TMMAmortisman.RakamKaydet(ve, kullanan, amortismanKod, ak.yil, 0, ar.prSicilNo, ENUMAmortismanTur.FEVKALADE, yeniAmortismanTutari, ar.saymanlikKod, ar.harcamaBirimiKod, ar.ambarKod, bilgi.kod);
                //    }

                //}
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc Kaydet(Kullanici kullanan, ObjectArray bilgiler)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            if (bilgiler.objeler.Count == 0)
            {
                sonuc.hataStr = "Kaydedilecek veri bulunamadı!";
                sonuc.islemSonuc = false;
            }
            else
            {
                foreach (SicilNoDegerArtis bilgi in bilgiler.objeler)
                {
                    sonuc = Kaydet(kullanan, bilgi);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            return sonuc;
        }

        public static Sonuc KaydetEnflasyonDuzeltmesiMB(Kullanici kullanan, ObjectArray bilgiler)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            if (bilgiler.objeler.Count == 0)
            {
                sonuc.hataStr = "Enflasyon düzeltmesi yapılacak veri bulunamadı!";
                sonuc.islemSonuc = false;
            }
            else
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
                TABGenel tabGenel = new TABGenel(ve);
                TABSicilNoDegerArtis to = new TABSicilNoDegerArtis(ve);

                int basariliIslem = 0;

                try
                {
                    foreach (SicilNoDegerArtis bilgi in bilgiler.objeler)
                    {
                        if (to.EnflasyonDuzeltmesiYapildiMiMB(bilgi))
                            continue;

                        ve.BeginTransaction();

                        string sayacKod = "SICILDEGERARTIS" + bilgi.muhasebeKod + bilgi.harcamaKod.Replace(".", "");
                        bilgi.kod = Guid.NewGuid().ToString().Replace("-", "");
                        bilgi.belgeNo = tabGenel.BelgeNoAl(sayacKod, "000000");
                        bilgi.durum = (int)ENUMBelgeDurumu.ONAYLI;

                        bilgi.harcamaKod = bilgi.harcamaKod.Replace(".", "");
                        bilgi.hesapPlanKod = bilgi.hesapPlanKod.Replace(".", "");

                        sonuc = to.Kaydet(bilgi);

                        if (sonuc.islemSonuc)
                        {
                            basariliIslem++;

                            ve.Commit();

                            var amorsimanSonuc = TMMAmortisman.RakamKaydet(ve, kullanan, "", bilgi.belgeTarihi.Yil, bilgi.belgeTarihi.Ay, bilgi.prSicilNo, ENUMAmortismanTur.FEVKALADE, bilgi.amotismanTutar, bilgi.muhasebeKod, bilgi.harcamaKod.Replace(".", ""), bilgi.ambarKod, bilgi.kod);
                        }
                        else
                            ve.Rollback();

                    }
                }
                catch (Exception ex)
                {

                }

                ve.VeriErisimKapat();

                if (basariliIslem == 0)
                    sonuc.hataStr = "Enflasyon düzeltmesi yapılacak veri bulunamadı!";

            }
            return sonuc;
        }


        public static Sonuc KaydetEnflasyonDuzeltmesi(Kullanici kullanan, string birimKodBilgi, ObjectArray bilgiler)
        {
            Sonuc sonuc = new Sonuc();

            if (bilgiler.objeler.Count == 0)
            {
                sonuc.DegerAta($"{birimKodBilgi} biriminde enflasyon düzeltmesi yapılacak veri bulunamadı!");
            }
            else
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
                TABGenel tabGenel = new TABGenel(ve);
                TABSicilNoDegerArtis to = new TABSicilNoDegerArtis(ve);

                int basariliIslem = 0;

                try
                {
                    foreach (SicilNoDegerArtis bilgi in bilgiler.objeler)
                    {
                        if (to.EnflasyonDuzeltmesiYapildiMi(bilgi))
                            continue;

                        ve.BeginTransaction();

                        string sayacKod = "SICILDEGERARTIS" + bilgi.muhasebeKod + bilgi.harcamaKod.Replace(".", "");
                        bilgi.kod = Guid.NewGuid().ToString().Replace("-", "");
                        bilgi.belgeNo = tabGenel.BelgeNoAl(sayacKod, "000000");
                        bilgi.durum = (int)ENUMBelgeDurumu.ONAYLI;

                        bilgi.harcamaKod = bilgi.harcamaKod.Replace(".", "");
                        bilgi.hesapPlanKod = bilgi.hesapPlanKod.Replace(".", "");

                        sonuc = to.Kaydet(bilgi);

                        if (sonuc.islemSonuc)
                        {
                            basariliIslem++;

                            ve.Commit();

                            var amorsimanSonuc = TMMAmortisman.RakamKaydet(ve, kullanan, "", bilgi.belgeTarihi.Yil, 0, bilgi.prSicilNo, ENUMAmortismanTur.FEVKALADE, bilgi.amotismanTutar, bilgi.muhasebeKod, bilgi.harcamaKod.Replace(".", ""), "", bilgi.kod);

                        }
                        else
                            ve.Rollback();
                    }
                }
                catch (Exception ex)
                {
                }

                ve.VeriErisimKapat();
                if (basariliIslem == 0)
                    sonuc.DegerAta($"{birimKodBilgi} biriminde {bilgiler.objeler.Count} kayıt bulundu ama daha önce yapıldığı için enflasyon düzeltmesi yapılmadı!");
                else
                    sonuc.DegerAta($"{birimKodBilgi} biriminde {bilgiler.objeler.Count} kaydın {basariliIslem} adedi için enflasyon düzeltmesi yapıldı.");
            }
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, SicilNoDegerArtis bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan);
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilNoDegerArtis tabSicilO = new TABSicilNoDegerArtis(ve);
            IDataReader dr = null;

            try
            {
                dr = tabSicilO.Listele(bilgi);

                if (dr != null)
                {
                    int sonKolon = (int)TABSicilNoDegerArtis.ENUMAlanlar.KATSAYI;
                    while (dr.Read())
                    {
                        SicilNoDegerArtis gelen = new SicilNoDegerArtis();
                        gelen.kod = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.KOD, "");
                        gelen.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.MUHASEBEKOD, "");
                        gelen.harcamaKod = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.HARCAMABIRIMKOD, "");
                        gelen.belgeNo = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.BELGENO, "");
                        gelen.belgeTarihi = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.BELGETARIHI);
                        gelen.prSicilNo = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.PRSICILNO, 0);
                        gelen.gorSicilNo = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.GORSICILNO, "");
                        gelen.tutar = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.TUTAR, 0.0);
                        gelen.gerekce = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.GEREKCE, "");
                        gelen.durum = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.DURUM, 0);
                        gelen.tur = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.TUR, 0);
                        gelen.hesapPlanKod = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.HESAPPLANKOD, "");
                        gelen.eskiTutar = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.ESKITUTAR, 0.0);
                        gelen.tifNo = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.TIFNO, "");
                        gelen.ambarKod = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.AMBARKOD, "");
                        gelen.kdvOran = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.KDVORAN, 0);
                        gelen.katsayi = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.KATSAYI, 0);

                        gelen.muhasebeAd = VeriErisim.DegerAl(dr, sonKolon + 1, "");
                        gelen.harcamaAd = VeriErisim.DegerAl(dr, sonKolon + 2, "");
                        gelen.malzemeKod = VeriErisim.DegerAl(dr, sonKolon + 3, "");
                        gelen.malzemeAdi = VeriErisim.DegerAl(dr, sonKolon + 4, "");
                        gelen.amotismanTutar = VeriErisim.DegerAl(dr, sonKolon + 5, (decimal)0.0);

                        donusDeger.objeler.Add(gelen);
                    }

                    if (donusDeger.objeler.Count == 0)
                        donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

                    //donusDeger.sonuc.anahtar=
                    donusDeger.sonuc.islemSonuc = true;
                }

            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
            }

            if (dr != null)
                dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        public static ObjectArray ListeleToplamTutar(VeriErisim veriErisim, Kullanici kullanan, SicilNoDegerArtis bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan);
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilNoDegerArtis tabSicilO = new TABSicilNoDegerArtis(ve);
            IDataReader dr = null;

            try
            {
                dr = tabSicilO.ListeleToplamTutar(bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            while (dr.Read())
            {
                SicilNoDegerArtis gelen = new SicilNoDegerArtis();
                gelen.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                gelen.toplamTutar = VeriErisim.DegerAl(dr, 1, 0.0);

                donusDeger.objeler.Add(gelen);
            }

            if (donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            //donusDeger.sonuc.anahtar=
            donusDeger.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        public static Sonuc Sil(Kullanici kullanan, SicilNoDegerArtis bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (bilgi.kod != "")
                sonuc.hataStr = TasinirMalResI.TMM074 + "";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilNoDegerArtis to = new TABSicilNoDegerArtis(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc GuncelleDurum(Kullanici kullanan, SicilNoDegerArtis bilgi, string tur)
        {

            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            SicilNoDegerArtis t = new SicilNoDegerArtis();
            t.kod = bilgi.kod;
            ObjectArray liste = Listele(null, kullanan, t);
            SicilNoDegerArtis tf = (SicilNoDegerArtis)liste[0];

            if (tf.durum == (int)ENUMBelgeDurumu.ONAYLI && tur != "OnayKaldir")
            {
                sonuc.hataStr = "Belge onaylı işlem yapılamaz.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (tf.durum != (int)ENUMBelgeDurumu.YENI && tur == "Iptal")
            {
                sonuc.hataStr = "Belgenin durumdan dolayı İptal edilemez.";
                sonuc.islemSonuc = false;
                return sonuc;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilNoDegerArtis to = new TABSicilNoDegerArtis(ve);
            ve.BeginTransaction();

            if (tur == "Onay")
            {
                bilgi.durum = (int)ENUMBelgeDurumu.ONAYLI;
            }
            else if (tur == "OnayKaldir")
            {
                bilgi.durum = (int)ENUMBelgeDurumu.ONAYKALDIR;
            }
            else if (tur == "OnayaGonder")
            {
                bilgi.durum = 3;// (int)ENUMBelgeDurumu.ONAYAGONDER;
            }
            else if (tur == "Iptal")
            {
                bilgi.durum = (int)ENUMBelgeDurumu.IPTAL;
            }
            else if (tur == "GeriGonderildi")
            {
                bilgi.durum = (int)ENUMBelgeDurumu.YENI;
            }

            if (bilgi.kod != "")
                sonuc = to.GuncelleDurum(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                if (tur != "OnayaGonder")
                {
                    SicilNoDegerArtis degerArtis = new SicilNoDegerArtis();
                    degerArtis.prSicilNo = tf.prSicilNo;
                    degerArtis.durum = (int)ENUMBelgeDurumu.ONAYLI;

                    ObjectArray degerArtisListe = ListeleToplamTutar(ve, kullanan, degerArtis);

                    if (degerArtisListe.objeler.Count > 0)
                        degerArtis = (SicilNoDegerArtis)degerArtisListe[0];
                    else
                        degerArtis = new SicilNoDegerArtis();

                    TNS.TMM.SicilNoHareket sc = new SicilNoHareket();
                    sc.prSicilNo = tf.prSicilNo;
                    //sc.siraNo = 1;//SADECE SIRANO 1 OLANIN DEGER ARTISI GUNCELLENECEK
                    sc.degerArtisi = (decimal)degerArtis.toplamTutar;
                    /*  Zimmet sırasında sizil hareket tablosuna değer artış ile fiyat toplamı yazılıyordu. Bu durum 1.satır kayıtları ile aynı olacak şekilde değiştirildi.
                     *  Bu nedenle sıra numarası 1 olan değil, bütün satırların değer artışları güncellenmeli. H.Ö */

                    sonuc = TMMSicilNoHareket.GuncelleDegerArtis(kullanan, sc);


                    //EĞER ENFLASYON İSE İŞLEM YAP (Not : Değer azalış enflasyon gibi çalışmalı)
                    if (sonuc.islemSonuc && (bilgi.tur == 1 || bilgi.tur == 3))
                    {
                        AmortismanKriter ak = new AmortismanKriter
                        {
                            yil = bilgi.belgeTarihi.Yil,
                            prSicilNo = bilgi.prSicilNo,
                            muhasebeKod = bilgi.muhasebeKod,
                            harcamaKod = bilgi.harcamaKod,
                            uygulamaTur = (int)ENUMAmortismanUygulamaTur.KRITERLEREGORE,
                            raporTur = 6
                        };
                        //ak.donem = (bilgi.belgeTarihi.Ay - 1) / 3;
                        ak.donem = TMMAmortisman.AmortismanDonemiBul(bilgi.harcamaKod, bilgi.belgeTarihi);

                        //ObjectArray aListe = TMMAmortisman.AmortismanRaporla2(ve, kullanan, ak);
                        ObjectArray aListe = TMMAmortisman.AmortismanRaporla3(ve, kullanan, ak);

                        decimal simdikiArtisTutar = (decimal)bilgi.tutar;
                        foreach (AmortismanRapor3 ar in aListe.objeler)
                        {
                            decimal sonBedel = ar.toplamTutar;
                            decimal oran = simdikiArtisTutar / (sonBedel - simdikiArtisTutar);
                            decimal yeniAmortismanTutari = (decimal)OrtakFonksiyonlar.KurusYap((double)((ar.maliyetToplamAmortisman + ar.enflasyonToplamAmortisman) * oran));
                            string amortismanKod = "";//Kayıt edilirlen üretiliyor


                            TABAmortisman tAmortisman = new TABAmortisman(ve);
                            ve.BeginTransaction();

                            //Maliyet Amortismanı için Enflasyon Düzeltme
                            if (yeniAmortismanTutari != 0)
                                sonuc = tAmortisman.Kaydet(amortismanKod, ak.yil, -1, ar.prSicilNo, (int)ENUMAmortismanTur.DEGERLEME, bilgi.kod, yeniAmortismanTutari, ar.saymanlikKod, ar.harcamaBirimiKod, ar.ambarKod);

                            //Değer Artış Amortismanları için Enflasyon Düzeltme
                            yeniAmortismanTutari = (decimal)OrtakFonksiyonlar.KurusYap((double)(ar.degerArtisToplamAmortisman * oran));
                            if (yeniAmortismanTutari != 0)
                                sonuc = tAmortisman.Kaydet(amortismanKod, ak.yil, -3, ar.prSicilNo, (int)ENUMAmortismanTur.FEVKALADE, bilgi.kod, yeniAmortismanTutari, ar.saymanlikKod, ar.harcamaBirimiKod, ar.ambarKod);

                            ////Enflasyon Artış Amortismanları için Enflasyon Düzeltme
                            //yeniAmortismanTutari = (decimal)OrtakFonksiyonlar.KurusYap((double)(ar.enflasyonToplamAmortisman * oran));
                            //if (yeniAmortismanTutari != 0)
                            //    sonuc = tAmortisman.Kaydet(amortismanKod, ak.yil, -2, ar.prSicilNo, (int)ENUMAmortismanTur.FEVKALADE, bilgi.kod, yeniAmortismanTutari, ar.saymanlikKod, ar.harcamaBirimiKod, ar.ambarKod);

                            if (sonuc.islemSonuc)
                                ve.Commit();
                            else
                                ve.Rollback();
                        }

                    }




                }

                sonuc.anahtar = bilgi.kod + "," + bilgi.belgeNo;
                sonuc.bilgiStr = TasinirMalResI.TMM118;
                //ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc GuncelleTifNo(Kullanici kullanan, ObjectArray bilgiler)
        {
            Sonuc sonuc = new Sonuc();

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilNoDegerArtis to = new TABSicilNoDegerArtis(ve);

            ve.BeginTransaction();

            foreach (SicilNoDegerArtis bilgi in bilgiler.objeler)
            {
                sonuc = to.GuncelleTifNo(bilgi);
                if (!sonuc.islemSonuc)
                    break;
            }


            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        private static Sonuc Kontrol(Kullanici kullanan, SicilNoDegerArtis bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (bilgi.muhasebeKod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";
            if (bilgi.harcamaKod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";
            if (bilgi.belgeTarihi.isNull)
                sonuc.hataStr += TasinirMalResI.TMM077 + "<br>";
            if (bilgi.gorSicilNo == string.Empty)
                sonuc.hataStr += "Sicil No boş bırakılamaz.<br>";
            if (bilgi.tur == 0)
                sonuc.hataStr += "Tür bilgisi boş bırakılamaz.<br>";

            if (bilgi.tur == 1 && bilgi.tutar == 0)
            {
                //if (tf.katsayi <= 100)
                //    hata += "Enflasyon düzeltme oranı 100 den büyük olmalıdır.<br>";
            }
            else if (bilgi.tutar == 0)
                sonuc.hataStr += "Tutar bilgisi boş bırakılamaz.<br>";

            if (bilgi.belgeNo != "")
            {
                bilgi.belgeNo = bilgi.belgeNo.Trim().PadLeft(6, '0');
                ObjectArray kListe = Listele(null, kullanan, bilgi);
                if (kListe.objeler.Count > 0)
                {
                    SicilNoDegerArtis tf = (SicilNoDegerArtis)kListe[0];
                    if (tf.durum != 1) sonuc.hataStr += "Belgenin durumundan dolayı değişiklik yapılamaz.<br>";
                }
                else
                    sonuc.hataStr += "Verilen belge numarası hatalı.<br>";
            }

            if (sonuc.hataStr != String.Empty)
                sonuc.islemSonuc = false;

            return sonuc;
        }

    }
}
