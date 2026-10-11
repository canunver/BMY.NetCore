using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using System.Collections;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Rapor bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCRaporlarv2
    {
        public ObjectArray SurecBilgiListele(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            if (!HRCYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebe, kriter.birim))
            {
                bilgi.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00256;
                bilgi.sonuc.islemSonuc = false;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRaporlarv2 to = new TABRaporlarv2(ve);
            IDataReader r = to.SurecBilgiListele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Surec sr = new Surec();

                    sr.yil = VeriErisim.DegerAl(r, 0, 0);
                    sr.surecNo = VeriErisim.DegerAl(r, 1, "");
                    sr.muhasebe = VeriErisim.DegerAl(r, 2, 0);
                    sr.birim = VeriErisim.DegerAl(r, 3, "");
                    sr.islem.kod = VeriErisim.DegerAl(r, 4, 0);
                    sr.surecTarih = VeriErisim.DegerAl(r, 5);
                    sr.surecDurum = VeriErisim.DegerAl(r, 6, 0);
                    sr.aciklama = VeriErisim.DegerAl(r, 7, "");
                    sr.isinAdi = VeriErisim.DegerAl(r, 8, "");
                    sr.yillaraSariIslemMi = VeriErisim.DegerAl(r, 9, 0);
                    //9.kolon islem.kod yukarıda tanımlandı
                    sr.islem.ad = VeriErisim.DegerAl(r, 11, "");
                    sr.islem.ekoKod = VeriErisim.DegerAl(r, 12, "");
                    sr.islem.tur = VeriErisim.DegerAl(r, 13, 0);
                    sr.islem.odemeHesapKod = VeriErisim.DegerAl(r, 14, "");

                    if (HRCYetkiKontrol.MuhasebeBirimYetkiVarMi(kullanan, sr.muhasebe.ToString(), sr.birim))
                    {
                        sr.muhasebeAd = HRCServis.servisUZY.UzayDegeriStr(kullanan, "MUHMUHASEBE", sr.muhasebe.ToString(), true, "");
                        sr.birimAd = HRCServis.servisUZY.UzayDegeriStr(kullanan, "MUHMUHASEBEBIRIM", sr.birim + "-" + sr.muhasebe, false, "");
                        bilgi.objeler.Add(sr);
                    }
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray OnayBelgesiBilgiListele(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRaporlarv2 to = new TABRaporlarv2(ve);
            TABOnayForm to2 = new TABOnayForm(ve2);
            IDataReader r = to.OnayBelgesiBilgiListele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    OnayForm bilgi = new OnayForm();

                    bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.idareAdi = VeriErisim.DegerAl(r, 4, "");
                    bilgi.belgeTarihi = VeriErisim.DegerAl(r, 5);
                    bilgi.evrakSayi = VeriErisim.DegerAl(r, 6, "");
                    bilgi.isTanimi = VeriErisim.DegerAl(r, 7, "");
                    bilgi.isNiteligi = VeriErisim.DegerAl(r, 8, "");
                    bilgi.isMiktari = VeriErisim.DegerAl(r, 9, "");
                    bilgi.odenekYili = VeriErisim.DegerAl(r, 10, 0);
                    bilgi.yaklasikMaliyet = VeriErisim.DegerAl(r, 11, 0.0);
                    bilgi.yMParaBirimi = VeriErisim.DegerAl(r, 12, 0);
                    bilgi.yMTip = VeriErisim.DegerAl(r, 13, 0);
                    bilgi.dokumanBedeli = VeriErisim.DegerAl(r, 14, "");
                    bilgi.projeNo = VeriErisim.DegerAl(r, 19, "");
                    bilgi.avansVarmi = VeriErisim.DegerAl(r, 20, 0);
                    bilgi.avansSartlari = VeriErisim.DegerAl(r, 21, "");
                    bilgi.ihaleUsulu = VeriErisim.DegerAl(r, 22, 0);
                    bilgi.ihaleMaddesi = VeriErisim.DegerAl(r, 23, 0);
                    bilgi.ilanVerilecekmi = VeriErisim.DegerAl(r, 24, 0);
                    bilgi.ilanSekli = VeriErisim.DegerAl(r, 25, "");
                    bilgi.onYeterlilik = VeriErisim.DegerAl(r, 26, 0.0);
                    bilgi.fiyatFarkiVarmi = VeriErisim.DegerAl(r, 27, 0);
                    bilgi.fiyatFarki = VeriErisim.DegerAl(r, 28, "");
                    bilgi.aciklama = VeriErisim.DegerAl(r, 29, "");
                    bilgi.gercekGorevli = VeriErisim.DegerAl(r, 30, "");
                    bilgi.gercekGorevliUnvan = VeriErisim.DegerAl(r, 31, "");
                    bilgi.onayaSunumTarihi = VeriErisim.DegerAl(r, 32);
                    bilgi.ihaleYetkili = VeriErisim.DegerAl(r, 33, "");
                    bilgi.ihaleYetkiliUnvan = VeriErisim.DegerAl(r, 34, "");
                    bilgi.onaylamaTarihi = VeriErisim.DegerAl(r, 35);
                    bilgi.ihaleMakami = VeriErisim.DegerAl(r, 36, "");
                    bilgi.komisyonVarmi = VeriErisim.DegerAl(r, 37, 0);
                    bilgi.hTgerceklestirmeSuresi = VeriErisim.DegerAl(r, 38, "");
                    bilgi.hTgerceklestirmeUsulu = VeriErisim.DegerAl(r, 39, "");
                    bilgi.hTgerceklestirmeGorevli = VeriErisim.DegerAl(r, 40, "");
                    bilgi.hTmihmandarlikVarmi = VeriErisim.DegerAl(r, 41, 0);
                    bilgi.hTperTcNo = VeriErisim.DegerAl(r, 42, "");
                    bilgi.hrcTalimati = VeriErisim.DegerAl(r, 43, 0);
                    bilgi.onIlanYilCeyrek = VeriErisim.DegerAl(r, 44, 0);
                    bilgi.onIlanGercekGorevli = VeriErisim.DegerAl(r, 45, "");
                    bilgi.onIlanGercekGorevliUnvan = VeriErisim.DegerAl(r, 46, "");
                    bilgi.onIlanIhaleYetkili = VeriErisim.DegerAl(r, 47, "");
                    bilgi.onIlanIhaleYetkiliUnvan = VeriErisim.DegerAl(r, 48, "");
                    bilgi.onIlanOnaylamaTarihi = VeriErisim.DegerAl(r, 49);
                    bilgi.onIlanOnayaSunumTarihi = VeriErisim.DegerAl(r, 50);
                    bilgi.onIlanVarMi = VeriErisim.DegerAl(r, 51, 0);
                    bilgi.dokumanPostaSatisVarMi = VeriErisim.DegerAl(r, 52, 0);
                    bilgi.dokumanYurticiBedel = VeriErisim.DegerAl(r, 53, "");
                    bilgi.dokumanYurtdisiBedel = VeriErisim.DegerAl(r, 54, "");
                    bilgi.ihaleMaddeAdi = new HRCIhaleTurMadde().IhaleUsulMaddeAl(kullanan, bilgi.ihaleUsulu, bilgi.ihaleMaddesi).adi;

                    Surec surec = new Surec();
                    surec.yil = bilgi.yil;
                    surec.surecNo = bilgi.surecNo;
                    ObjectArray surecler = HRCSurec.Listele(null, kullanan, surec);
                    if (surecler.objeler.Count > 0)
                        bilgi.surec = (Surec)surecler.objeler[0];

                    IhaleIlanTanim ilanTanim = new IhaleIlanTanim();
                    ilanTanim = new HRCIhaleIlanTanim().IhaleIlanSekliAl(kullanan, bilgi.ihaleUsulu, bilgi.yaklasikMaliyet, ((Surec)surecler.objeler[0]).islem.kod);
                    bilgi.ilanTanimi = ilanTanim;

                    //Onay belgesindeki tertipler
                    OnayFormTertip tertipKriter = new OnayFormTertip();
                    tertipKriter.yil = bilgi.yil;
                    tertipKriter.surecNo = bilgi.surecNo;
                    IDataReader rT = to2.TertipListele(tertipKriter);
                    while (rT.Read())
                    {
                        OnayFormTertip tertip = HRCOnayForm.DRdenOnayFormTertipOl(rT);
                        bilgi.tertip.objeler.Add(tertip);
                    }
                    rT.Close();
                    liste.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve2.VeriErisimKapat();
            ve.VeriErisimKapat();

            return liste;
        }

        public ObjectArray OnayBelgesiTertipBilgiListele(Kullanici kullanan, SurecSorguKriter kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlarv2 to = new TABRaporlarv2(ve);
            ObjectArray bilgi = new ObjectArray();
            IDataReader rT = to.OnayBelgesiTertipBilgiListele(kriter);
            while (rT.Read())
            {
                OnayFormTertip tertip = new OnayFormTertip();
                tertip.yil = VeriErisim.DegerAl(rT, 0, 0);
                tertip.surecNo = VeriErisim.DegerAl(rT, 1, "");
                tertip.siraNo = VeriErisim.DegerAl(rT, 2, 0);
                tertip.kurKod = VeriErisim.DegerAl(rT, 3, "");
                tertip.fonKod = VeriErisim.DegerAl(rT, 4, "");
                tertip.finKod = VeriErisim.DegerAl(rT, 5, "");
                tertip.ekoKod = VeriErisim.DegerAl(rT, 6, "");
                tertip.odenekTutari = VeriErisim.DegerAl(rT, 7, 0.0);
                tertip.blokeTutari = VeriErisim.DegerAl(rT, 8, 0.0);
                tertip.pebKod = VeriErisim.DegerAl(rT, 9, "");
                tertip.kullanilacakTutar = VeriErisim.DegerAl(rT, 10, 0.0);
                //tertip.muhasebe = VeriErisim.DegerAl(rT, 11, 0);
                //tertip.birim = VeriErisim.DegerAl(rT, 12, "");

                bilgi.objeler.Add(tertip);
            }
            rT.Close();
            ve.VeriErisimKapat();
            return bilgi;
        }

        public ObjectArray AvansFormBilgiListele(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlarv2 to = new TABRaporlarv2(ve);
            IDataReader r = to.AvansBelgesiBilgiListele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    OdemeEmriMIFForm f = new OdemeEmriMIFForm();

                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    f.birim = VeriErisim.DegerAl(r, 2, "");
                    f.surecNo = VeriErisim.DegerAl(r, 3, "");
                    f.prOdemeEmriNo = VeriErisim.DegerAl(r, 4, 0);
                    f.prFaturaNo = VeriErisim.DegerAl(r, 5, 0);
                    f.odemeEmriNo = VeriErisim.DegerAl(r, 6, "");
                    f.odemeEmriTarih = VeriErisim.DegerAl(r, 7);
                    f.aciklama = VeriErisim.DegerAl(r, 8, "");
                    f.belgeTur = VeriErisim.DegerAl(r, 9, 0);
                    f.odemeEmriYil = f.odemeEmriTarih.Oku().Year;

                    f.detay = OdemeEmriDetayBilgiListele(kullanan, f);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        private ObjectArray OdemeEmriDetayBilgiListele(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            string kodYapisiHesap = TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD8/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABOdemeEmriForm to = new TABOdemeEmriForm(ve);
            IDataReader r = to.ListeleDetay(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    OdemeEmriMIFDetay t = new OdemeEmriMIFDetay();

                    t.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, VeriErisim.DegerAl(r, 6, ""));
                    t.tertipVeMahsupBelgeNo = VeriErisim.DegerAl(r, 7, "");
                    t.yansitma = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, VeriErisim.DegerAl(r, 8, ""));
                    t.borc = VeriErisim.DegerAl(r, 9, 0.0);
                    t.alacak = VeriErisim.DegerAl(r, 10, 0.0);
                    t.hesapPlanAd = HRCServis.UzayYaziDegerGetir("MUHHESAPPLANI", kriter.yil.ToString() + "-" + t.hesapPlanKod, true, "ACIKLAMA");

                    TNS.BKD.Tertip tt = new TNS.BKD.Tertip(DateTime.Now.Year, TNS.BKD.ENUMTabloKod.TABHarcama);
                    string[] kirilim = t.tertip.Split('-');
                    if (kirilim.Length > 4)
                        tt = new TNS.BKD.Tertip(DateTime.Now.Year, TNS.BKD.ENUMTabloKod.TABPEBOdenek);

                    tt.TertipAta(t.tertip);
                    tt.Noktala();
                    t.tertip = tt.TertipStrOl();

                    bilgi.objeler.Add(t);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray TeklifEdilenFiyatBilgiListele(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRaporlarv2 to = new TABRaporlarv2(ve);
            IDataReader r = to.TeklifEdilenFiyatBilgiListele(kriter);
            TeklifEdilenFiyat ekleObj;
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    ekleObj = new TeklifEdilenFiyat();
                    ekleObj.yil = VeriErisim.DegerAl(r, 0, 0);
                    ekleObj.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    ekleObj.birim = VeriErisim.DegerAl(r, 2, "");
                    ekleObj.surecNo = VeriErisim.DegerAl(r, 3, "");
                    ekleObj.tarih = VeriErisim.DegerAl(r, 4);
                    ekleObj.saat = VeriErisim.DegerAl(r, 5, "");
                    ekleObj.teklifTutari = VeriErisim.DegerAl(r, 6, 0.0);
                    ekleObj.fiyatDisiTeklifTutari = VeriErisim.DegerAl(r, 7, 0.0);
                    ekleObj.yerliMaliTeklifTutari = VeriErisim.DegerAl(r, 8, 0.0);
                    ekleObj.firmaKod = VeriErisim.DegerAl(r, 9, 0);
                    ekleObj.teklifTur = VeriErisim.DegerAl(r, 10, 0);
                    ekleObj.firmaAd = VeriErisim.DegerAl(r, 11, "");
                    ekleObj.kazananFirma.kod = VeriErisim.DegerAl(r, 12, 0);
                    ekleObj.kazananFirma.vergiNo = VeriErisim.DegerAl(r, 13, "");
                    ekleObj.kazananFirma.ad = VeriErisim.DegerAl(r, 14, "");
                    donusDeger.objeler.Add(ekleObj);
                }
            }
            r.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        public ObjectArray PiyasaFiyatBilgiListele(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRaporlarv2 to = new TABRaporlarv2(ve);
            IDataReader r = to.PiyasaFiyatTeklifBilgiListele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Teklif t = new Teklif();

                    t.yil = VeriErisim.DegerAl(r, 0, 0);
                    t.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    t.birim = VeriErisim.DegerAl(r, 2, "");
                    t.surecNo = VeriErisim.DegerAl(r, 3, "");
                    t.malzeme.malzemeNo = VeriErisim.DegerAl(r, 4, 0);
                    t.firma.firmaNo = VeriErisim.DegerAl(r, 5, 0);
                    t.donem.donemNo = VeriErisim.DegerAl(r, 6, 0);
                    t.tip = VeriErisim.DegerAl(r, 7, 0);
                    t.birimFiyat = VeriErisim.DegerAl(r, 8, 0.0);
                    t.kazanan = VeriErisim.DegerAl(r, 9, 0);
                    t.malzeme.malzemeKod = VeriErisim.DegerAl(r, 10, "");
                    t.aciklama = VeriErisim.DegerAl(r, 11, "");
                    t.donem.tarih = VeriErisim.DegerAl(r, 12);
                    t.malzeme.adi = VeriErisim.DegerAl(r, 13, "");
                    t.malzeme.miktar = VeriErisim.DegerAl(r, 14, 0.0);
                    t.malzeme.olcuBirim.kod = VeriErisim.DegerAl(r, 15, 0);
                    t.malzeme.olcuBirim.ad = VeriErisim.DegerAl(r, 16, "");
                    t.malzeme.malzemeKod = VeriErisim.DegerAl(r, 17, "");
                    t.toplamTutar = Math.Round((decimal)t.birimFiyat * (decimal)t.malzeme.miktar, 2);

                    if ((t.tip == (int)ENUMTeklifTuru.YAKLASIKMALIYETFIRMA) || (t.tip == (int)ENUMTeklifTuru.PIYASAFIYATARASTIRMASI)
                        || (t.tip == (int)ENUMTeklifTuru.KISMITEKLIF))
                    {
                        t.firma.firmaBilgisi.ad = VeriErisim.DegerAl(r, 18, "");
                        t.firma.firmaBilgisi.adres = VeriErisim.DegerAl(r, 19, "");
                        t.firma.firmaBilgisi.kod = VeriErisim.DegerAl(r, 20, 0);
                        t.firma.firmaBilgisi.vergiNo = VeriErisim.DegerAl(r, 21, "");
                    }
                    bilgi.objeler.Add(t);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray PiyasaFiyatBilgiToplamListele(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRaporlarv2 to = new TABRaporlarv2(ve);
            IDataReader r = to.PiyasaFiyatToplamTeklifBilgiListele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Teklif t = new Teklif();

                    t.yil = VeriErisim.DegerAl(r, 0, 0);
                    t.surecNo = VeriErisim.DegerAl(r, 1, "");
                    t.firma.firmaBilgisi.vergiNo = VeriErisim.DegerAl(r, 2, "");
                    t.firma.firmaBilgisi.ad = VeriErisim.DegerAl(r, 3, "");
                    t.kazanan = VeriErisim.DegerAl(r, 4, 0);
                    t.toplamTutar = VeriErisim.DegerAl(r, 5, 0);

                    bilgi.objeler.Add(t);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray KazananFirmaBilgiListele(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRaporlarv2 to = new TABRaporlarv2(ve);
            IDataReader r = to.KazananFirmaBilgiListele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    KazananFirma kazanan = new KazananFirma();

                    kazanan.yil = VeriErisim.DegerAl(r, 0, 0);
                    kazanan.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    kazanan.birim = VeriErisim.DegerAl(r, 2, "");
                    kazanan.surecNo = VeriErisim.DegerAl(r, 3, "");
                    kazanan.firmaKod1 = VeriErisim.DegerAl(r, 4, 0);
                    kazanan.firmaKod2 = VeriErisim.DegerAl(r, 5, 0);
                    kazanan.firmaVergiNo1 = VeriErisim.DegerAl(r, 6, "");
                    kazanan.firmaAd1 = VeriErisim.DegerAl(r, 7, "");
                    kazanan.firmaVergiNo2 = VeriErisim.DegerAl(r, 8, "");
                    kazanan.firmaAd2 = VeriErisim.DegerAl(r, 9, "");

                    donusDeger.objeler.Add(kazanan);
                }
            }
            r.Close();

            ve.VeriErisimKapat();

            return donusDeger;
        }

        public ObjectArray HakedisBilgiListele(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRaporlarv2 to = new TABRaporlarv2(ve);
            IDataReader r = to.HakedisBilgiListele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Hakedis hakedis = new Hakedis();

                    hakedis.yil = VeriErisim.DegerAl(r, 0, 0);
                    hakedis.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    hakedis.birim = VeriErisim.DegerAl(r, 2, "");
                    hakedis.surecNo = VeriErisim.DegerAl(r, 3, "");
                    hakedis.sozlesmeBedeli = VeriErisim.DegerAl(r, 4, 0.0);
                    hakedis.ihaleTarihi = VeriErisim.DegerAl(r, 5);
                    hakedis.ihaleKararTarihi = VeriErisim.DegerAl(r, 6);
                    hakedis.ihaleKararNo = VeriErisim.DegerAl(r, 7, "");
                    hakedis.sozlesmeTarihi = VeriErisim.DegerAl(r, 8);
                    hakedis.sozlesmeNo = VeriErisim.DegerAl(r, 9, "");
                    hakedis.teslimTarihi = VeriErisim.DegerAl(r, 10);
                    hakedis.isinSuresi = VeriErisim.DegerAl(r, 11, "");
                    hakedis.bitimTarihi = VeriErisim.DegerAl(r, 12);

                    donusDeger.objeler.Add(hakedis);
                }
            }
            r.Close();

            ve.VeriErisimKapat();

            return donusDeger;

        }

        public ObjectArray FaturaBilgiListele(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRaporlarv2 to = new TABRaporlarv2(ve);
            IDataReader r = to.FaturaBilgiListele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    FaturaForm f = new FaturaForm();

                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    f.birim = VeriErisim.DegerAl(r, 2, "");
                    f.surecNo = VeriErisim.DegerAl(r, 3, "");
                    f.prFaturaNo = VeriErisim.DegerAl(r, 4, 0);
                    f.firmaNo = VeriErisim.DegerAl(r, 5, 0);
                    f.faturaNo = VeriErisim.DegerAl(r, 6, "");
                    f.faturaTarih = VeriErisim.DegerAl(r, 7);
                    f.aciklama = VeriErisim.DegerAl(r, 8, "");
                    f.tasinirNo = VeriErisim.DegerAl(r, 9, "");
                    f.odemeEmriNo = VeriErisim.DegerAl(r, 10, "");
                    f.personelAmiri = VeriErisim.DegerAl(r, 11, "");
                    f.personelAmiriUnvan = VeriErisim.DegerAl(r, 12, "");
                    f.genelToplam = (decimal)VeriErisim.DegerAl(r, 13, 0.0);
                    f.oiv = (decimal)VeriErisim.DegerAl(r, 14, 0.0);
                    f.kdvTevkifati = (decimal)VeriErisim.DegerAl(r, 15, 0.0);
                    f.durum = VeriErisim.DegerAl(r, 16, 0);
                    f.iskonto = (decimal)VeriErisim.DegerAl(r, 17, 0.0);
                    f.iskontoOran = (decimal)VeriErisim.DegerAl(r, 18, 0.0);
                    f.odemeEmriYil = VeriErisim.DegerAl(r, 19, 0);
                    f.paraBirimi = VeriErisim.DegerAl(r, 20, 0);

                    if (f.odemeEmriYil == 0)
                        f.odemeEmriYil = f.yil;
                    if (f.tasinirYil == 0)
                        f.tasinirYil = f.yil;


                    f.firma.kod = VeriErisim.DegerAl(r, 21, 0);
                    f.firma.muhasebe = VeriErisim.DegerAl(r, 22, 0);
                    f.firma.birim = VeriErisim.DegerAl(r, 23, "");
                    f.firma.ad = VeriErisim.DegerAl(r, 24, "");
                    f.firma.vergiNo = VeriErisim.DegerAl(r, 25, "");
                    f.firma.vergiDairesi = VeriErisim.DegerAl(r, 26, "");
                    f.firma.banka = VeriErisim.DegerAl(r, 27, "");
                    f.firma.hesapNo = VeriErisim.DegerAl(r, 28, "");
                    f.firma.eMail = VeriErisim.DegerAl(r, 29, "");
                    f.firma.firmaYetkilisi = VeriErisim.DegerAl(r, 30, "");
                    f.firma.telefon = VeriErisim.DegerAl(r, 31, "");
                    f.firma.faks = VeriErisim.DegerAl(r, 32, "");
                    f.firma.adres = VeriErisim.DegerAl(r, 33, "");
                    f.firma.uyruk = VeriErisim.DegerAl(r, 34, 0);
                    f.firma.gorulmesin = VeriErisim.DegerAl(r, 35, 0);
                    f.firma.yasakli = VeriErisim.DegerAl(r, 36, 0);
                    f.firma.cepTel = VeriErisim.DegerAl(r, 37, "");
                    f.firma.web = VeriErisim.DegerAl(r, 38, "");


                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray IhaleIlanBilgiListele(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRaporlarv2 to = new TABRaporlarv2(ve);
            IDataReader r = to.IhaleIlanBilgiListele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    IhaleIlanBilgisi f = new IhaleIlanBilgisi();

                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    f.birim = VeriErisim.DegerAl(r, 2, "");
                    f.surecNo = VeriErisim.DegerAl(r, 3, "");
                    f.tarih = VeriErisim.DegerAl(r, 4);
                    f.teslimYeri = VeriErisim.DegerAl(r, 5, "");
                    f.teslimTarihi = VeriErisim.DegerAl(r, 6, "");
                    f.ihaleYeri = VeriErisim.DegerAl(r, 7, "");
                    f.ihaleTarihi = VeriErisim.DegerAl(r, 8);
                    f.ihaleSaati = VeriErisim.DegerAl(r, 9, "");
                    f.ihaleTeminYeri = VeriErisim.DegerAl(r, 10, "");
                    f.teklifSuresi = VeriErisim.DegerAl(r, 11, 0);
                    f.isSuresi = VeriErisim.DegerAl(r, 12, 0);
                    f.teklifTuru = VeriErisim.DegerAl(r, 13, 0);
                    f.teklifBelirlemeTuru = VeriErisim.DegerAl(r, 14, 0);
                    f.katilimTuru = VeriErisim.DegerAl(r, 15, 0);
                    f.indirimOrani = VeriErisim.DegerAl(r, 16, 0);
                    f.konsorsiyum = VeriErisim.DegerAl(r, 17, 0);
                    f.ilanAdedi = VeriErisim.DegerAl(r, 18, 0);
                    f.ilanSekli = VeriErisim.DegerAl(r, 19, 0);
                    f.kaynakTuru = VeriErisim.DegerAl(r, 20, 0);
                    f.kismiTeklifeAcikMi = VeriErisim.DegerAl(r, 21, 0);
                    f.sektorKodu = VeriErisim.DegerAl(r, 22, "");
                    f.sektorAdi = VeriErisim.DegerAl(r, 23, "");
                    f.ymaKod = VeriErisim.DegerAl(r, 24, 0);
                    f.ymaBaslangicTarihi = VeriErisim.DegerAl(r, 25);
                    f.ymaBitisTarihi = VeriErisim.DegerAl(r, 26);
                    f.ymaAciklama = VeriErisim.DegerAl(r, 27, "");
                    f.onYeterlikDavetTuru = VeriErisim.DegerAl(r, 28, 0);
                    f.onYeterlikDavetSayi = VeriErisim.DegerAl(r, 29, 0);
                    f.ihaleNo = VeriErisim.DegerAl(r, 30, "");

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray TasinirBilgiListele(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            string kodYapisiHBirim = TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRaporlarv2 to = new TABRaporlarv2(ve);
            IDataReader r = to.TasinirBilgiListele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TasinirForm f = new TasinirForm();

                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    f.birim = VeriErisim.DegerAl(r, 2, "");
                    f.surecNo = VeriErisim.DegerAl(r, 3, "");
                    f.prTasinirNo = VeriErisim.DegerAl(r, 4, 0);
                    f.prFaturaNo = VeriErisim.DegerAl(r, 5, 0);
                    f.tasinirNo = VeriErisim.DegerAl(r, 6, "");
                    f.tasinirTarih = VeriErisim.DegerAl(r, 7);
                    f.harcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHBirim, VeriErisim.DegerAl(r, 8, ""));
                    f.ambarKod = VeriErisim.DegerAl(r, 9, "");
                    f.aciklama = VeriErisim.DegerAl(r, 10, "");
                    f.tasinirYil = f.tasinirTarih.Oku().Year;

                    f.detay = TasinirDetayBilgiListele(kullanan, f);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;

        }
        private ObjectArray TasinirDetayBilgiListele(Kullanici kullanan, TasinirForm kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            string kodYapisiHesap = TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD8/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRaporlarv2 to = new TABRaporlarv2(ve);
            IDataReader r = to.TasinirDetayBilgiListele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TasinirDetay t = new TasinirDetay();

                    t.adi = VeriErisim.DegerAl(r, 6, "");
                    t.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, VeriErisim.DegerAl(r, 7, ""));
                    t.miktar = VeriErisim.DegerAl(r, 8, 0.0);
                    t.olcuBirimKod = VeriErisim.DegerAl(r, 9, 0);
                    t.kdvOran = VeriErisim.DegerAl(r, 10, 0);
                    t.birimFiyat = VeriErisim.DegerAl(r, 11, 0.0);
                    t.olusturuldu = VeriErisim.DegerAl(r, 12, 0);
                    t.faturaDetaySiraNo = VeriErisim.DegerAl(r, 13, 0);
                    t.hesapPlanAd = HRCServis.UzayYaziDegerGetir("TASHESAPPLAN", t.hesapPlanKod, true, "ACIKLAMA");

                    bilgi.objeler.Add(t);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray IhaleSozlesmeSonuUcretBilgiListele(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRaporlarv2 to = new TABRaporlarv2(ve);
            IDataReader r = to.IhaleSozlesmeSonuUcretBilgiListele(kriter);
            IhaleSozlesmeSonuUcret ekleObj;
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    ekleObj = new IhaleSozlesmeSonuUcret();
                    ekleObj.yil = VeriErisim.DegerAl(r, 0, 0);
                    ekleObj.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    ekleObj.birim = VeriErisim.DegerAl(r, 2, "");
                    ekleObj.surecNo = VeriErisim.DegerAl(r, 3, "");
                    ekleObj.siraNo = VeriErisim.DegerAl(r, 4, 0);
                    ekleObj.tutar = VeriErisim.DegerAl(r, 5, 0.0);
                    ekleObj.belgeTarihi = VeriErisim.DegerAl(r, 6);
                    ekleObj.belgeNo = VeriErisim.DegerAl(r, 7, "");
                    donusDeger.objeler.Add(ekleObj);
                }
            }
            r.Close();


            ve.VeriErisimKapat();

            return donusDeger;
        }


        public ObjectArray MufYaklasikMaliyetListele(Kullanici kullanan, SurecSorguKriter kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRaporlarv2 to = new TABRaporlarv2(ve);
            TABOnayForm to2 = new TABOnayForm(ve2);
            IDataReader r = to.MufYaklasikMaliyetListele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Teklif t = new Teklif();

                    t.yil = VeriErisim.DegerAl(r, 0, 0);
                    t.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    t.birim = VeriErisim.DegerAl(r, 2, "");
                    t.surecNo = VeriErisim.DegerAl(r, 3, "");
                    t.malzeme.malzemeNo = VeriErisim.DegerAl(r, 4, 0);

                    t.malzeme.miktar = VeriErisim.DegerAl(r, 5, 0); //toplam firma sayısı (sqlde toplanan firma sayısını geçici olarak buraya yazdım) -selin
                    t.birimFiyat = VeriErisim.DegerAl(r, 6, 0.0); //toplam birim fiyat (sqlde malzemeno'ya göre toplanan malzeme başına birim fiyat toplamı geçici olarak buraya yazdım) -selin

                    liste.objeler.Add(t);
                }
            }

            r.Close();

            ve2.VeriErisimKapat();
            ve.VeriErisimKapat();

            return liste;
        }

    }

}
