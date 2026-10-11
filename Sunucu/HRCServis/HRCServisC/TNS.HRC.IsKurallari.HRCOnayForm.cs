using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.MUH;
using TNS.UZY;
using System.Collections.Generic;
using TNS.BKD;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Onay form belge bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCOnayForm
    {
        /// <summary>
        /// Bütçe Muhasebe Modul servisi
        /// </summary>
        public static IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

        /// <summary>
        /// Arguman olarak verilen, onay form bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public TNS.MUH.EksikOdenekler Kaydet(Kullanici kullanan, OnayForm form, int islemKod, int parcaKod)
        {
            TNS.MUH.EksikOdenekler eo = new EksikOdenekler();
            eo.sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                eo.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                eo.sonuc.islemSonuc = false;
                return eo;
            }

            string hataStr = "";
            if (form.yil == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";
            if (form.muhasebe == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";
            if (form.birim.Trim() == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";
            if (form.surecNo == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00067 + "<br>";
            if (form.isTanimi.Trim() == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00241 + "<br>";
            if (form.hrcTalimati != (int)ENUMEvetHayir.EVET)
            {
                if (form.isMiktari.Trim() == "")
                    hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00242 + "<br>";
            }
            if (form.isNiteligi.Trim() == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00243 + "<br>";
            if (form.yaklasikMaliyet == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00244 + "<br>";
            if (form.avansVarmi == 1 && form.avansSartlari.Trim() == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00245 + "<br>";
            if ((form.ilanVerilecekmi == 1) && (form.ilanSekli.Trim() == ""))
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00246 + "<br>";
            if (form.yaklasikMaliyet == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00247 + "<br>";
            if (form.ihaleMakami.Trim() == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00248 + "<br>";
            if (form.ihaleYetkili.Trim() == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00249 + "<br>";
            if (form.ihaleYetkiliUnvan.Trim() == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00250 + "<br>";
            if (form.gercekGorevli.Trim() == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00251 + "<br>";
            if (form.gercekGorevliUnvan.Trim() == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00252 + "<br>";
            if (form.belgeTarihi.isNull)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00253 + "<br>";

            int odenekYil = DateTime.Now.Year;

            TNSCollection tertipler = new TNSCollection();
            for (int i = 0; i < form.tertip.objeler.Count; i++)
            {
                OnayFormTertip t1 = ((OnayFormTertip)form.tertip.objeler[i]);
                string tertipIc = t1.kurKod + "-" + t1.fonKod + "-" + t1.finKod + "-" + t1.ekoKod;
                if (t1.pebKod != "")
                    tertipIc += "-" + t1.pebKod;

                string tertip1 = t1.tertipYil + "-" + tertipIc;
                for (int j = i + 1; j < form.tertip.objeler.Count; j++)
                {
                    OnayFormTertip t2 = ((OnayFormTertip)form.tertip.objeler[j]);
                    string tertip2 = t2.tertipYil + "-" + t2.kurKod + "-" + t2.fonKod + "-" + t2.finKod + "-" + t2.ekoKod;
                    if (t2.pebKod != "")
                        tertip2 += "-" + t2.pebKod;
                    if (tertip1.Replace(".", "") == tertip2.Replace(".", ""))
                    {
                        hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00254 + "<br>";
                        i = form.tertip.objeler.Count;
                        break;
                    }
                }

                if (t1.tertipYil == odenekYil)
                {
                    TertipOdenekBilgi tob = new TertipOdenekBilgi(tertipIc, t1.kullanilacakTutar + t1.kdvTutar);
                    tertipler.Add(tob);
                }
            }

            Sonuc stu = servisMUH.TertiplerUygunMu(kullanan, tertipler);
            if (!stu.islemSonuc)
                hataStr += stu.hataStr + "<br>";

            Surec surec = new Surec(form.yil, form.surecNo);
            surec = TNS.HRC.Arac.IlkSurec(HRCSurec.Listele(null, kullanan, surec));
            if (surec == null)
                hataStr += "Süreç bulunamadı" + "<br>";

            if (surec.surecDurum > 5)
                hataStr += "Süreç arşivde yada iptal olduğundan değişiklik yapılamaz!" + "<br>";

            stu = HRCYuzdeOnKapat.YuzdeOnAcikMi(null, kullanan, form.yil, form.ihaleMaddeAdi, surec.MalHizmetYapimAd(), tertipler, form.muhasebe, form.birim);
            if (!stu.islemSonuc)
                hataStr += stu.hataStr + "<br>";

            if (hataStr != "")
            {
                eo.sonuc.islemSonuc = false;
                eo.sonuc.hataStr = hataStr;
                return eo;
            }

            //Parca Adım tamamlandı mı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            int yil = form.yil;
            string surecNo = form.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                eo.sonuc.islemSonuc = false;
                eo.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return eo;
            }
            ///////////////////////////////////////////////////////////////////////////

            bool sifirla = false;
            eo = servisMUH.OdenekTalepYapilacakMi(kullanan, form.muhasebe, form.birim, (TertipOdenekBilgi[])tertipler.ToArray(typeof(TertipOdenekBilgi)), odenekYil, form.yil, form.surecNo, string.Empty, false, form.DogrudanTeminMi());
            if (eo.EksikOdenekVarMi())
            {
                sifirla = true;
            }
            else if (!eo.sonuc.islemSonuc)
            {
                return eo;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOnayForm to = new TABOnayForm(ve);
            ve.BeginTransaction();

            eo.sonuc = to.Kaydet(form);

            if (eo.sonuc.islemSonuc)
            {
                //Tertipleri kaydet
                eo.sonuc = to.TertipSil(form.yil, form.surecNo);
                if (eo.sonuc.islemSonuc)
                {
                    foreach (OnayFormTertip tertip in form.tertip.objeler)
                    {
                        if (sifirla)
                        {
                            tertip.kullanilacakTutar = 0;
                            tertip.kdvTutar = 0;
                            tertip.blokeTutari = 0;
                            tertip.sozlesmeKdvliTutar = 0;
                        }
                        eo.sonuc = to.TertipKaydet(tertip);
                        if (!eo.sonuc.islemSonuc)
                            break;
                    }
                }
            }

            if (eo.sonuc.islemSonuc)
            {
                //KmL
                //tertip sayısı 1'den fazla ise hangi tertip için ödenekGönderme Talebi hazırlanacak
                //
                //string tertipKod = string.Empty;
                //foreach (OnayFormTertip tertip in form.tertip.objeler)
                //{
                //    if (tertip.kullanilacakTutar > tertip.odenekTutari)
                //    {
                //        tertipKod = tertip.kurKod + "-" + tertip.fonKod + "-" + tertip.finKod + "-" + tertip.ekoKod;
                //        if (!string.IsNullOrEmpty(tertip.pebKod))
                //            tertipKod += "-" + tertip.pebKod;
                //
                //        //Sonuc sonucOdenekTalep = servisMUH.OdenekTalepIslemleri(kullanan, form.yil, "", form.yil, form.surecNo, form.muhasebe, form.birim, tertipKod, tertip.kullanilacakTutar);
                //        //burdayım
                //        //TalepBelge belge = new TalepBelge();
                //        //belge.yil = form.yil;
                //
                //        //belge.
                //        //Sonuc sonucOdenekTalep = servisMUH.TalepOdenekGondermeKaydet(kullanan, belge);
                //        //if (!sonucOdenekTalep.islemSonuc)
                //        //{
                //        //    sonuc.hataStr += "<br>" + sonucOdenekTalep.hataStr;
                //        //    break;
                //        //}
                //        //sonuc.bilgiStr += "<br>" + sonucOdenekTalep.bilgiStr;
                //    }
                //}

                //if (form.tertip.ObjeSayisi > 0 && TNS.DEG.Arac.VarsayilanKurumBul() == "12")
                //    sonuc = servisMUH.TalepKaydetOnayBelgesi(kullanan, form.tertip, yil, 0, form.muhasebe, form.birim, form.surecNo);


                if (eo.sonuc.islemSonuc)
                {
                    //Süreç devam ediyor olarak işaretle.
                    ///////////////////////////////////////////////////////////////////////////
                    new HRCSurec().SurecDurumKaydet(null, kullanan, yil, surecNo, ENUMSurecDurum.DEVAMEDIYOR);
                    ///////////////////////////////////////////////////////////////////////////

                    //Parca Adım devam ediyor olarak işaretle.
                    ///////////////////////////////////////////////////////////////////////////
                    new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                    ///////////////////////////////////////////////////////////////////////////

                    if (string.IsNullOrEmpty(eo.sonuc.hataStr))
                        eo.sonuc.bilgiStr += "<br>" + TNS.HRC.HarcamaSurecleriResI.HRC00010;
                    else
                        eo.sonuc.bilgiStr += "<br>" + eo.sonuc.hataStr;

                    if (sifirla)
                        eo.sonuc.bilgiStr += "<br>Ödenek eksikliği yüzünden, rakamlarınınız sıfırlanarak kayıt işlemi yapılmıştır. Ödenek talebiniz karşılandıktan sonra rakamlarınızı tekrar yazıp kaydediniz.";

                    ve.Commit();
                    BlokeTertipleriOtomatikGuncelle(null, kullanan, form.yil, form.surecNo, DateTime.Now.Year, ENUMKontrolDurum.KontrolEtme);
                }
                else
                {
                    eo.sonuc.hataStr += "<br>" + TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + eo.sonuc.hataStr;
                    ve.Rollback();
                }
            }
            else
            {
                eo.sonuc.hataStr += "<br>" + TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + eo.sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return eo;
        }


        /// <summary>
        /// Arguman olarak verilen, onay form evrak sayı bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc EvrakSayiKaydet(Kullanici kullanan, OnayForm form)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hataStr = "";
            if (form.yil == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";
            if (form.muhasebe == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";
            if (form.birim.Trim() == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";
            if (form.surecNo == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00067 + "<br>";

            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOnayForm to = new TABOnayForm(ve);
            ve.BeginTransaction();

            sonuc = to.EvrakSayiKaydet(form);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, onay form belge bilgisini veritabanından okur 
        /// ve OnayForm nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, onay form belge listesini OnayForm nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, OnayForm kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABOnayForm to = new TABOnayForm(ve);
            TABOnayForm to2 = new TABOnayForm(ve2);
            IDataReader r = to.Listele(kriter);

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
                    if (kriter.harcGorevliVar)
                    {
                        bilgi.harcGorevAd = VeriErisim.DegerAl(r, 55, "");
                        bilgi.harcGorevUnvan = VeriErisim.DegerAl(r, 56, "");
                    }
                    bilgi.ihaleMaddeAdi = new HRCIhaleTurMadde().IhaleUsulMaddeAl(kullanan, bilgi.ihaleUsulu, bilgi.ihaleMaddesi).adi;

                    Surec surec = new Surec();
                    surec.yil = bilgi.yil;
                    //surec.muhasebe = bilgi.muhasebe;
                    //surec.birim = bilgi.birim;
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
                    //tertipKriter.tertipYil = DateTime.Now.Year;
                    IDataReader rT = to2.TertipListele(tertipKriter);
                    while (rT.Read())
                    {
                        OnayFormTertip tertip = DRdenOnayFormTertipOl(rT);

                        bilgi.tertip.objeler.Add(tertip);
                    }
                    rT.Close();

                    if (bilgi.tertip.ObjeSayisi == 0)
                    {
                        tertipKriter.tertipYil = 0;
                        IDataReader rT2 = to2.TertipListele(tertipKriter);
                        while (rT2.Read())
                        {
                            OnayFormTertip tertip = DRdenOnayFormTertipOl(rT2);
                            bilgi.tertip.objeler.Add(tertip);
                        }
                        rT2.Close();
                    }

                    liste.objeler.Add(bilgi);
                }
                r.Close();
            }
            else
                liste.sonuc.DegerAta(to.Hata());


            ve2.VeriErisimKapat();
            ve.VeriErisimKapat();

            return liste;
        }

        public static OnayFormTertip DRdenOnayFormTertipOl(IDataReader rT, bool surecDurumVar = false)
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
            tertip.tertipYil = VeriErisim.DegerAl(rT, 11, 0);
            tertip.kdvTutar = VeriErisim.DegerAl(rT, 12, 0.0);
            tertip.sozlesmeKdvliTutar = VeriErisim.DegerAl(rT, 13, 0.0);
            tertip.muhasebe = VeriErisim.DegerAl(rT, 14, 0);
            tertip.birim = VeriErisim.DegerAl(rT, 15, "");
            tertip.isinAdi = VeriErisim.DegerAl(rT, 16, "");
            if (surecDurumVar)
                tertip.surecDurum = VeriErisim.DegerAl(rT, 17, 0);

            if (tertip.tertipYil == 0) tertip.tertipYil = tertip.yil;
            TNS.BKD.Tertip tt = new TNS.BKD.Tertip(DateTime.Now.Year, TNS.BKD.ENUMTabloKod.TABPEBHarcama);
            tt.TertipAta(tertip.kurKod + "-" + tertip.fonKod + "-" + tertip.finKod + "-" + tertip.ekoKod + "-" + tertip.pebKod);
            tt.Noktala();
            tertip.kurKod = tt.kodlar[0];
            tertip.fonKod = tt.kodlar[1];
            tertip.finKod = tt.kodlar[2];
            tertip.ekoKod = tt.kodlar[3];
            tertip.pebKod = tt.kodlar[4];
            return tertip;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, onay form belge tertip bilgisini veritabanından okur 
        /// ve OnayFormTertip nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, onay form belge tertip listesini OnayFormTertip nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleTertip(Kullanici kullanan, OnayFormTertip kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOnayForm to = new TABOnayForm(ve);
            ObjectArray bilgi = new ObjectArray();
            IDataReader rT = to.TertipListele(kriter);
            while (rT.Read())
            {
                OnayFormTertip tertip = DRdenOnayFormTertipOl(rT);
                bilgi.objeler.Add(tertip);
            }
            rT.Close();

            if (bilgi.ObjeSayisi == 0)
            {
                kriter.tertipYil = 0;
                IDataReader rT2 = to.TertipListele(kriter);
                while (rT2.Read())
                {
                    OnayFormTertip tertip = new OnayFormTertip();
                    tertip.yil = VeriErisim.DegerAl(rT2, 0, 0);
                    tertip.surecNo = VeriErisim.DegerAl(rT2, 1, "");
                    tertip.siraNo = VeriErisim.DegerAl(rT2, 2, 0);
                    tertip.kurKod = VeriErisim.DegerAl(rT2, 3, "");
                    tertip.fonKod = VeriErisim.DegerAl(rT2, 4, "");
                    tertip.finKod = VeriErisim.DegerAl(rT2, 5, "");
                    tertip.ekoKod = VeriErisim.DegerAl(rT2, 6, "");
                    tertip.odenekTutari = VeriErisim.DegerAl(rT2, 7, 0.0);
                    tertip.blokeTutari = VeriErisim.DegerAl(rT2, 8, 0.0);
                    tertip.pebKod = VeriErisim.DegerAl(rT2, 9, "");
                    tertip.kullanilacakTutar = VeriErisim.DegerAl(rT2, 10, 0.0);
                    tertip.tertipYil = VeriErisim.DegerAl(rT2, 11, 0);
                    tertip.kdvTutar = VeriErisim.DegerAl(rT2, 12, 0.0);
                    tertip.muhasebe = VeriErisim.DegerAl(rT2, 13, 0);
                    tertip.birim = VeriErisim.DegerAl(rT2, 14, "");

                    if (tertip.tertipYil == 0)
                        tertip.tertipYil = tertip.yil;

                    bilgi.objeler.Add(tertip);
                }
                rT2.Close();
            }

            ve.VeriErisimKapat();
            return bilgi;
        }

        public ObjectArray ListeleBlokeTertip(Kullanici kullanan, OnayFormTertip kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOnayForm to = new TABOnayForm(ve);
            ObjectArray bilgi = new ObjectArray();
            IDataReader rT = to.BlokeTertipListele(kriter);
            while (rT.Read())
            {
                OnayFormTertip tertip = DRdenOnayFormTertipOl(rT, true);
                bilgi.objeler.Add(tertip);
            }
            rT.Close();

            if (bilgi.ObjeSayisi == 0)
            {
                kriter.tertipYil = 0;
                IDataReader rT2 = to.BlokeTertipListele(kriter);
                while (rT2.Read())
                {
                    OnayFormTertip tertip = DRdenOnayFormTertipOl(rT2, true);
                    bilgi.objeler.Add(tertip);
                }
                rT2.Close();
            }

            ve.VeriErisimKapat();
            return bilgi;
        }

        internal Sonuc BlokeTertipleriOtomatikGuncelle(VeriErisim veriErisim, Kullanici kullanan, int surecYil, string surecNo, int tertipYil, ENUMKontrolDurum kontrol)
        {
            if (string.IsNullOrEmpty(surecNo) || surecYil == 0) return new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOnayForm to = new TABOnayForm(ve);

            OnayFormTertip kriter = new OnayFormTertip();
            kriter.surecNo = surecNo;
            kriter.yil = surecYil;
            kriter.tertipYil = tertipYil;

            ObjectArray oa = ListeleBlokeTertip(kullanan, kriter);

            string hataStr = "";
            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            foreach (OnayFormTertip item in oa.objeler)
            {
                double yeniBlokeTutari = item.sozlesmeKdvliTutar;
                if (yeniBlokeTutari <= 0)
                    yeniBlokeTutari = item.kullanilacakTutar + item.kdvTutar;
                yeniBlokeTutari -= HarcananTutarBul(kullanan, uzyServis, item);
                if (yeniBlokeTutari < 0)
                {
                    yeniBlokeTutari = 0;
                }
                if (yeniBlokeTutari > item.blokeTutari && kontrol != ENUMKontrolDurum.KontrolEtme)
                {
                    //hataStr += "Kontrol Yazılmadı";
                }
                item.blokeTutari = yeniBlokeTutari;
            }

            Sonuc sonuc = new Sonuc();
            if (!string.IsNullOrEmpty(hataStr))
            {
                if (kontrol == ENUMKontrolDurum.HataVer)
                {
                    sonuc.DegerAta(hataStr);
                }
            }

            ve.BeginTransaction();
            //if (sonuc.islemSonuc)
            //    sonuc = to.TertipSil(surecYil, surecNo);
            if (sonuc.islemSonuc)
            {
                foreach (OnayFormTertip tertip in oa.objeler)
                {
                    sonuc = to.TertipBlokeGuncelle(tertip);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            if (kontrol == ENUMKontrolDurum.UyariVer)
                sonuc.bilgiStr = hataStr;
            ve.VeriErisimKapat();
            return sonuc;
        }

        private double HarcananTutarBul(Kullanici kullanan, IUZYServis servis, OnayFormTertip item)
        {
            string uzayKosul = TNS.UZY.Arac.UzayKosulOl(item.tertipYil.ToString(), "", "", "830,160,161,164,165,166,167", item.kurKod, item.fonKod, item.finKod, item.ekoKod, "1/8", item.pebKod, "", item.yil.ToString(), item.surecNo);
            //string sql = servis.UzayKosulStr(kullanan, "MUHODEMEEMRIMIF", uzayKosul, false, "");
            double d = servis.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", uzayKosul, false);
            return d;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, onay form belge tertip bilgisini veritabanından okur 
        /// ve OnayFormTertip nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, onay form belge tertip listesini OnayFormTertip nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleTertipBenzer(Kullanici kullanan, OnayFormTertip kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOnayForm to = new TABOnayForm(ve);
            ObjectArray bilgi = new ObjectArray();
            IDataReader rT = to.TertipListeleBenzer(kriter);
            while (rT.Read())
            {
                OnayFormTertip tertip = DRdenOnayFormTertipOl(rT);
                bilgi.objeler.Add(tertip);
            }
            rT.Close();

            if (bilgi.ObjeSayisi == 0)
            {
                kriter.tertipYil = 0;
                IDataReader rT2 = to.TertipListeleBenzer(kriter);
                while (rT2.Read())
                {
                    OnayFormTertip tertip = DRdenOnayFormTertipOl(rT2);
                    bilgi.objeler.Add(tertip);
                }
                rT2.Close();
            }

            ve.VeriErisimKapat();
            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen, onay form belge tertip bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="ofTertip">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc GuncelleTertip(Kullanici kullanan, OnayFormTertip ofTertip)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOnayForm to = new TABOnayForm(ve);
            ve.BeginTransaction();

            sonuc = to.TertipGuncelle(ofTertip);
            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetTertip(Kullanici kullanan, OnayFormTertip ofTertip)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOnayForm to = new TABOnayForm(ve);
            ve.BeginTransaction();

            sonuc = to.TertipKaydet(ofTertip);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                ve.hata = string.Empty;
                sonuc = to.TertipOnayGuncelle(ofTertip);
                if (sonuc.islemSonuc)
                {
                    sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                    ve.Commit();
                }
                else
                {
                    ve.hata = string.Empty;
                    sonuc = to.TertipSil(ofTertip.yil, ofTertip.surecNo, ofTertip.tertipYil);
                    if (sonuc.islemSonuc)
                    {
                        ve.hata = string.Empty;
                        sonuc = to.TertipKaydet(ofTertip);
                        if (sonuc.islemSonuc)
                        {
                            sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                            ve.Commit();
                        }
                        else
                        {
                            //sonuc.anahtar = to.sqlUretilen;
                            sonuc.hataStr = "IOF002 " + TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                            ve.Rollback();
                        }
                    }
                    else
                    {
                        //sonuc.anahtar = to.sqlUretilen;
                        sonuc.hataStr = "IOF001 " + TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                        ve.Rollback();
                    }
                }
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, onay form belge tertibine ait bloke edilen tutar bilgisini boş olarak veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="ofTertip">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc BlokeKaldirTertip(Kullanici kullanan, OnayFormTertip ofTertip)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOnayForm to = new TABOnayForm(ve);
            ve.BeginTransaction();

            sonuc = to.TertipBlokeKaldir(ofTertip);
            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc BlokeEkleTertip(Kullanici kullanan, OnayFormTertip ofTertip)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOnayForm to = new TABOnayForm(ve);
            ve.BeginTransaction();

            sonuc = to.TertipBlokeEkle(ofTertip);
            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = ofTertip.kurKod + "-" + ofTertip.fonKod + "-" + ofTertip.finKod + "-" + ofTertip.ekoKod + "-" + ofTertip.pebKod + " tertibi için " + Math.Abs(ofTertip.blokeTutari).ToString("N2") + " tutarında bloke eklenmiştir."; //TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        //public decimal BlokeHesapla(Kullanici kullanan, int yil, string surecNo)
        //{
        //    Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

        //    //if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
        //    //{
        //    //    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
        //    //    sonuc.islemSonuc = false;
        //    //    return decimal.Zero;
        //    //}

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABOnayForm to = new TABOnayForm(ve);
        //    ve.BeginTransaction();
        //    decimal blokeToplam = to.BlokeToplamListele(yil, surecNo);
        //    ve.VeriErisimKapat();

        //    decimal toplamHarcama = new HRCOdemeEmriForm().ListeleToplamHarcama(kullanan, yil, surecNo);
        //    decimal toplamAvans = new HRCOdemeEmriForm().ListeleToplamAvans(kullanan, yil, surecNo);

        //    return blokeToplam - toplamHarcama - toplamAvans;
        //}


        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, onay belgesi harcama bilgisini veritabanından okur 
        /// ve BHMHarcama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılcak ay bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <param name="peb">İşlem yapılacak alt faaliyet kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, onay belgesi harcama bilgisi listesini BHMHarcama nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray BHMOnayBelgesiHarcamaListesi(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));

            TABOnayForm to = new TABOnayForm(ve);
            IDataReader r = to.ListeleButceHazirlikOnayBelgesiHarcama(yil, ay, muhasebe, birim, kur, fon, fin, eko, peb);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    BHMHarcama bilgi = new BHMHarcama();

                    bilgi.islemYil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.islemAy = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.surecYil = VeriErisim.DegerAl(r, 2, 0);
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 4, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 5, "");
                    bilgi.kur = VeriErisim.DegerAl(r, 6, "");
                    bilgi.fon = VeriErisim.DegerAl(r, 7, "");
                    bilgi.fin = VeriErisim.DegerAl(r, 8, "");
                    bilgi.eko = VeriErisim.DegerAl(r, 9, "");
                    bilgi.peb = VeriErisim.DegerAl(r, 10, "");
                    bilgi.toplamTutar = VeriErisim.DegerAl(r, 11, (decimal)0);
                    bilgi.yillaraSariIslemMi = Convert.ToBoolean(VeriErisim.DegerAl(r, 12, 0));

                    if (bilgi.yillaraSariIslemMi)
                        bilgi.yil = bilgi.surecYil + 1;
                    else
                        bilgi.yil = bilgi.surecYil;

                    liste.objeler.Add(bilgi);
                }
            }
            r.Close();
            ve.VeriErisimKapat();
            return liste;
        }

        internal static ObjectArray YuzdeOnListele(VeriErisim veriErisim, Kullanici kullanan, int yil)
        {
            TNS.MUH.IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            YuzdeOnBilgi yob = new YuzdeOnBilgi();
            yob.tur = ENUMYuzdeOnTur.Mal;
            yob = muhServis.YuzdeOnUstSinirBul(kullanan, yil, yob);
            oa.Ekle(yob);

            yob = new YuzdeOnBilgi();
            yob.tur = ENUMYuzdeOnTur.Hizmet;
            yob = muhServis.YuzdeOnUstSinirBul(kullanan, yil, yob);
            oa.Ekle(yob);

            yob = new YuzdeOnBilgi();
            yob.tur = ENUMYuzdeOnTur.Yapim;
            yob = muhServis.YuzdeOnUstSinirBul(kullanan, yil, yob);
            oa.Ekle(yob);

            yob = new YuzdeOnBilgi();
            yob.tur = ENUMYuzdeOnTur.Tanimsiz;
            oa.Ekle(yob);

            string[] yuzdeOnIhaleKodlar = TNS.MUH.Arac.YuzdeOnAlimTurBul();
            foreach (YuzdeOnBilgi yobIc in oa.objeler)
            {
                try
                {
                    yobIc.liste = MalHizmetYapimListele(ve, kullanan, yil, yobIc, yuzdeOnIhaleKodlar);
                }
                catch (Exception ex)
                {
                    oa.sonuc.DegerAta(ex.Message);
                    break;
                }
            }
            ve.VeriErisimKapat();
            return oa;
        }

        public static TNS.BKD.TertipOdenekBilgi[] MalHizmetYapimListele(VeriErisim veriErisim, Kullanici kullanan, int yil, YuzdeOnBilgi yob, string[] yuzdeOnIhaleKodlar)
        {
            if (yuzdeOnIhaleKodlar == null || yuzdeOnIhaleKodlar.Length == 0)
                throw new Exception("Yüzde on limiti içindeki ihale kodları parametre olarak geçirmelisiniz!");

            string ekoKodYapi = TNS.DEG.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD" + 4 + "/KODYAPI");

            List<TNS.BKD.TertipOdenekBilgi> liste = new List<TNS.BKD.TertipOdenekBilgi>();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOnayForm of = new TABOnayForm(ve);
            string MalHizmetYapim = yob.TurAd;
            IDataReader dr = of.MalHizmetYapimListele(yil, (int)(yob.tur), yob.saymanlikKodlar, yob.birimKodlar);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            TNS.BKD.TertipOdenekBilgi to = null;
            string eskiEkoKod = null;
            while (dr.Read())
            {
                string ekoKod = OrtakFonksiyonlar.KodNoktaKoy(ekoKodYapi, VeriErisim.DegerAl(dr, 0, ""));
                if (ekoKod != eskiEkoKod || to == null || eskiEkoKod == null)
                {
                    string ekoAd = VeriErisim.DegerAl(dr, 1, "");
                    to = new TertipOdenekBilgi(ekoKod + "-" + ekoAd, yuzdeOnIhaleKodlar.Length + 1);
                    liste.Add(to);
                    eskiEkoKod = ekoKod;
                }
                string ihaleKodTur = VeriErisim.DegerAl(dr, 2, "");
                int yer = TNS.MUH.Arac.YuzdeOnIhaleKodYerBul(yuzdeOnIhaleKodlar, ihaleKodTur);
                to.odenekler[yer] += VeriErisim.DegerAl(dr, 3, (double)0);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return liste.ToArray();
        }

        /*
        public ObjectArray BHMAlimHarcamaListesiSozlesmeDmo(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABOnayForm to = new TABOnayForm(ve);
            IDataReader r = to.ListeleButceHazirlikOnayBelgesiHarcamaDmo(yil, ay, muhasebe, birim, kur, fon, fin, eko, peb);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    BHMHarcama bilgi = new BHMHarcama();

                    bilgi.islemYil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.islemAy = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.surecYil = VeriErisim.DegerAl(r, 2, 0);
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 4, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 5, "");
                    bilgi.kur = VeriErisim.DegerAl(r, 6, "");
                    bilgi.fon = VeriErisim.DegerAl(r, 7, "");
                    bilgi.fin = VeriErisim.DegerAl(r, 8, "");
                    bilgi.eko = VeriErisim.DegerAl(r, 9, "");
                    bilgi.peb = VeriErisim.DegerAl(r, 10, "");
                    bilgi.toplamTutar = VeriErisim.DegerAl(r, 11, (decimal)0);
                    bilgi.yillaraSariIslemMi = Convert.ToBoolean(VeriErisim.DegerAl(r, 12, 0));

                    if (bilgi.yillaraSariIslemMi)
                        bilgi.yil = bilgi.surecYil + 1;
                    else
                        bilgi.yil = bilgi.surecYil;
                }
            }
            r.Close();
            ve.VeriErisimKapat();
            return liste;
        }
        */
    }
}
