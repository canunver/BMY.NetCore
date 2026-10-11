using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;
using System.Collections;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Xml;
using System.Collections.Generic;

namespace TNS.TMM.IsKurallari
{
    public static class TMMMuhasebeBilgi
    {
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, MuhasebeIslemi form)
        {
            ObjectArray bilgi = new ObjectArray();

            string hata = "";
            //if (form.kriter.yil == 0)
            //    hata += "Yıl giriniz." + "<br>";
            if (form.kriter.muhasebeKod == "")
                hata += "Muhasebe Kodu giriniz." + "<br>";
            //if (form.kriter.harcamaKod == "")
            //    hata += "Harcama Birim Kodu giriniz." + "<br>";

            if (hata != "")
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = hata;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                TABMuhasebeBilgi to = new TABMuhasebeBilgi(ve);

                IDataReader dr = to.Listele(form);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        MuhasebeIslemi mi = new MuhasebeIslemi();
                        mi.islemRefNo = VeriErisim.DegerAl(dr, (int)TABMuhasebeBilgi.ENUMAlanlar.ISLEMREFNO, "");
                        mi.islemCinsi = VeriErisim.DegerAl(dr, (int)TABMuhasebeBilgi.ENUMAlanlar.ISLEMCINSI, "");
                        mi.servis = VeriErisim.DegerAl(dr, (int)TABMuhasebeBilgi.ENUMAlanlar.SERVIS, "");
                        mi.durum = VeriErisim.DegerAl(dr, (int)TABMuhasebeBilgi.ENUMAlanlar.DURUM, 0);
                        mi.islemYapan = VeriErisim.DegerAl(dr, (int)TABMuhasebeBilgi.ENUMAlanlar.ISLEMYAPAN, "");
                        mi.islemTarih = VeriErisim.DegerAl(dr, (int)TABMuhasebeBilgi.ENUMAlanlar.ISLEMTARIH);
                        mi.json = VeriErisim.DegerAl(dr, (int)TABMuhasebeBilgi.ENUMAlanlar.JSON, "");

                        bilgi.Ekle(mi);
                    }
                    dr.Close();
                }
                else
                    bilgi.sonuc.DegerAta(ve.hata);

            }
            catch (Exception ex)
            {
                bilgi.sonuc.DegerAta(ex.Message);
            }

            ve.VeriErisimKapat();

            return bilgi;
        }

        public static ObjectArray ListeleGonderilecek(VeriErisim veriErisim, Kullanici kullanan, MuhasebeIslemiKriter form)
        {
            ObjectArray bilgi = new ObjectArray();

            string hata = "";
            if (form.yil == 0)
                hata += "Yıl giriniz." + "<br>";
            if (form.muhasebeIslemTur == ENUMMuhasebeIslemTur.AMORTISMAN && form.donem == 0)
                hata += "Dönem giriniz." + "<br>";
            if (form.muhasebeKod == "")
                hata += "Muhasebe Kodu giriniz." + "<br>";
            if (form.harcamaKod == "")
                hata += "Harcama Birim Kodu giriniz." + "<br>";
            if (form.fisNo == "")
                hata += "Fiş No giriniz." + "<br>";

            if (hata != "")
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = hata;
                return bilgi;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            try
            {
                TABMuhasebeBilgi to = new TABMuhasebeBilgi(ve);

                IDataReader dr = to.ListeleGonderilecek(form);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        MuhasebeIslemiKriter kriter = new MuhasebeIslemiKriter();
                        kriter.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                        kriter.hesapPlanKod = VeriErisim.DegerAl(dr, 1, ""); //OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHP, VeriErisim.DegerAl(dr, 1, ""));
                        kriter.hesapPlanAd = VeriErisim.DegerAl(dr, 2, "");
                        kriter.muhasebeKod = VeriErisim.DegerAl(dr, 3, "");
                        kriter.harcamaKod = VeriErisim.DegerAl(dr, 4, "");
                        kriter.ambarKod = VeriErisim.DegerAl(dr, 5, "");
                        kriter.kdvOran = VeriErisim.DegerAl(dr, 6, 0);
                        kriter.kdvliBirimFiyat = VeriErisim.DegerAl(dr, 7, (decimal)0);
                        kriter.ilkBedel = VeriErisim.DegerAl(dr, 8, (decimal)0);

                        kriter.muhasebeAd = VeriErisim.DegerAl(dr, 9, "");
                        kriter.harcamaAd = VeriErisim.DegerAl(dr, 10, "");
                        kriter.ambarAd = VeriErisim.DegerAl(dr, 11, "");
                        kriter.islemTurKod = VeriErisim.DegerAl(dr, 12, 0);
                        kriter.nereyeGitti = VeriErisim.DegerAl(dr, 13, "");
                        kriter.gayriMenkulTurKod = VeriErisim.DegerAl(dr, 14, "");
                        kriter.satisBedeli = VeriErisim.DegerAl(dr, 15, (decimal)0);
                        kriter.aciklama = VeriErisim.DegerAl(dr, 16, "");
                        kriter.mutemmim = VeriErisim.DegerAl(dr, 17, 0);

                        kriter.listeDegerArtis = ListeleDetaylar(VeriErisim.DegerAl(dr, 18, ""), 2);
                        kriter.listeAmortisman = ListeleDetaylar(VeriErisim.DegerAl(dr, 19, ""), 3);

                        kriter.geciciIhrac = VeriErisim.DegerAl(dr, 20, 0);
                        kriter.tasitAmortismanSiniri = VeriErisim.DegerAl(dr, 21, (decimal)0);

                        bilgi.Ekle(kriter);
                    }

                    dr.Close();
                }
                else
                    bilgi.sonuc.DegerAta(ve.hata);

            }
            catch (Exception ex)
            {
                bilgi.sonuc.DegerAta(ex.Message);
            }

            ve.VeriErisimKapat();

            return bilgi;
        }

        private static TNSCollection ListeleDetaylar(string okunanXML, int detayTip)
        {
            TNSCollection pVAN = new TNSCollection();
            if (string.IsNullOrWhiteSpace(okunanXML)) return pVAN;

            XMLStringDataReader r = new XMLStringDataReader(okunanXML);
            if (r != null)
            {
                while (r.Read())
                {
                    if (detayTip == 2)
                        pVAN.Add(DetayOkuDegerArtis(r));
                    else if (detayTip == 3)
                        pVAN.Add(DetayOkuAmortisman(r));
                }
                r.Close();
            }
            return pVAN;
        }

        private static SicilNoDegerArtis DetayOkuDegerArtis(IDataReader dr)
        {
            SicilNoDegerArtis detay = new SicilNoDegerArtis();
            detay.kod = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.KOD, "");
            detay.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.MUHASEBEKOD, "");
            detay.harcamaKod = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.HARCAMABIRIMKOD, "");
            detay.belgeNo = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.BELGENO, "");
            detay.belgeTarihi = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.BELGETARIHI);
            detay.prSicilNo = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.PRSICILNO, 0);
            detay.gorSicilNo = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.GORSICILNO, "");
            detay.tutar = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.TUTAR, 0.0);
            detay.gerekce = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.GEREKCE, "");
            detay.durum = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.DURUM, 0);
            detay.tur = VeriErisim.DegerAl(dr, (int)TABSicilNoDegerArtis.ENUMAlanlar.TUR, 0);

            return detay;
        }

        private static AmortismanKriter DetayOkuAmortisman(IDataReader dr)
        {
            AmortismanKriter detay = new AmortismanKriter();
            detay.kod = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.KOD, "");
            detay.yil = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.YIL, 0);
            detay.donem = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.DONEM, 0);
            detay.prSicilNo = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.PRSICILNO, 0);
            detay.amortismanTur = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.AMORTISMANTUR, 0);
            detay.degerArtisKod = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.DEGERARTISKODU, "");
            detay.amortismanMiktar = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.AMORTISMANMIKTAR, (decimal)0.0);
            detay.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.MUHASEBEKOD, "");
            detay.harcamaKod = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.HARCAMABIRIMKOD, "");
            detay.ambarKod = VeriErisim.DegerAl(dr, (int)TABAmortisman.ENUMAlanlar.AMBARKOD, "");

            return detay;
        }


        public static ObjectArray ListeleTarihce(VeriErisim veriErisim, Kullanici kullanan, MuhasebeIslemiKriter form)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABMuhasebeBilgi to = new TABMuhasebeBilgi(ve);
            IDataReader dr = to.ListeleTarihce(form.yil, form.muhasebeKod, form.harcamaKod.Replace(".", ""), form.fisNo);

            if (to.Hata() == "")
            {
                while (dr.Read())
                {
                    TarihceBilgisi tarih = new TarihceBilgisi();

                    tarih.yil = VeriErisim.DegerAl(dr, (int)TABTarihce.ENUMAlanlar.YIL, 0);
                    tarih.muhasebe = VeriErisim.DegerAl(dr, (int)TABTarihce.ENUMAlanlar.MUHASEBEKOD, "");
                    tarih.harcamaBirimi = VeriErisim.DegerAl(dr, (int)TABTarihce.ENUMAlanlar.HARCAMABIRIMKOD, "");
                    tarih.belgeNo = VeriErisim.DegerAl(dr, (int)TABTarihce.ENUMAlanlar.FISNO, "");
                    tarih.islemYapan = VeriErisim.DegerAl(dr, (int)TABTarihce.ENUMAlanlar.ISLEMYAPAN, "");
                    tarih.islemTarih = VeriErisim.DegerAl(dr, (int)TABTarihce.ENUMAlanlar.ISLEMTARIH);
                    tarih.islem = VeriErisim.DegerAl(dr, (int)TABTarihce.ENUMAlanlar.ISLEM, "");
                    tarih.tasinirIslemFormKod = VeriErisim.DegerAl(dr, (int)TABTarihce.ENUMAlanlar.TASISLEMFORMKOD, "");
                    tarih.islemYapan = VeriErisim.DegerAl(dr, 9, "");
                    tarih.islemYapanAd = VeriErisim.DegerAl(dr, 10, "");

                    iVAN.objeler.Add(tarih);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return iVAN;
        }

        public static ObjectArray OnIzlemeOlustur(VeriErisim veriErisim, Kullanici kullanan, MuhasebeIslemiKriter form)
        {
            ObjectArray kaydetSonuc = Kaydet(veriErisim, kullanan, form, true);

            return kaydetSonuc;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, MuhasebeIslemiKriter form)
        {
            ObjectArray kaydetSonuc = Kaydet(veriErisim, kullanan, form, false);

            return kaydetSonuc.sonuc;
        }

        private static ObjectArray Kaydet(VeriErisim veriErisim, Kullanici kullanan, MuhasebeIslemiKriter form, bool onIzlemeOlustur)
        {
            ObjectArray kaydetSonuc = new ObjectArray();


            MerkezBankasi mb = MerkezBankasiGenel.Ayarlar;

            if (mb == null)
            {
                kaydetSonuc.sonuc = new Sonuc(false, "Muhasebe ayarları için gerekli dosyasi bulunamadı!", "");
                return kaydetSonuc;
            }

            //*******************
            bool muhasebeFisiKaydet = false;
            if (form.muhasebeIslemTur == ENUMMuhasebeIslemTur.TASINIRISLEMFISI)
            {
                if (form.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS ||
                    form.islemTipTur == (int)ENUMIslemTipi.BAGISGIRIS ||
                    form.islemTipTur == (int)ENUMIslemTipi.HURDACIKIS || form.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKISKURUM || //IHRAÇ
                    form.islemTipTur == (int)ENUMIslemTipi.SATISCIKIS)
                    muhasebeFisiKaydet = true;
            }
            else
                muhasebeFisiKaydet = true;

            //2018 4. dönem (yıl sonu) gayrimenkul amortismanı manuel muhasebeye göndermiş. Bu nedenle muhasebeye fiş kesilmeyecek
            if (form.muhasebeIslemTur == ENUMMuhasebeIslemTur.AMORTISMAN && form.ambarKod == "50" && form.yil == 2018 && form.donem == 12)
                muhasebeFisiKaydet = false;
            //******************


            if (!muhasebeFisiKaydet)
            {
                kaydetSonuc.sonuc = new Sonuc(true, "", "Muhasebe fişi kesilmeyecek.");
                return kaydetSonuc;
            }
            //*******************


            string servisKod = mb.MuhasebeServisKodGetir(form.harcamaKod, form.ambarKod);
            if (string.IsNullOrWhiteSpace(servisKod))
            {
                kaydetSonuc.sonuc = new Sonuc(false, "Muhasebe servis kodu bulunamadı!", "");
                return kaydetSonuc;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            //SicilNoDegerArtis degerArtis = new SicilNoDegerArtis();

            if (onIzlemeOlustur)
                form.durum = 100; //Ön izlememde onaysız kayıtların listelenmesi için 

            ObjectArray bilgiler = TMMMuhasebeBilgi.ListeleGonderilecek(ve, kullanan, form);

            if (!bilgiler.sonuc.islemSonuc || bilgiler.objeler.Count == 0)
            {
                ve.VeriErisimKapat();

                HataDosyaYaz("Kaydet-Liste-Bos", bilgiler.sonuc.hataStr);

                kaydetSonuc.sonuc = bilgiler.sonuc;
                return kaydetSonuc;
            }

            ////Ortak Listede bu bölüm alındığı için kaldırıldı. 04.10.2018 H.Ö 
            //if (form.muhasebeIslemTur == ENUMMuhasebeIslemTur.DEGERARTIS)
            //{
            //    //Değer artış türünü almak için bu bölüm kaldırılmadı. Ortak listede bu bilgi alındıktan sonra kaldırılacak.

            //    //Bedel Artırma ve azaltma işlemleri yapılacak
            //    degerArtis = new SicilNoDegerArtis
            //    {
            //        muhasebeKod = form.muhasebeKod,
            //        harcamaKod = form.harcamaKod,
            //        belgeNo = form.donem.ToString("000000")
            //    };

            //    ObjectArray dListe = TMMSicilNoDegerArtis.Listele(null, kullanan, degerArtis);
            //    degerArtis = (SicilNoDegerArtis)dListe[0];


            //    if (degerArtis.tur == 3)
            //    {
            //        ve.VeriErisimKapat();
            //        return new Sonuc(false, "Değer artış ve azalış işlemleri için muhasebe işlem fişi oluşturma çalışması devam ediyor!<br>En kısa sürede tamamlanacaktır.", "");
            //    }
            //}

            //Tarihçe bilgisi taşınır işlem fişi, değer atrışı ve amortisman işlemlerinin B/A onay bilgilerini okumak için eklenmiştir.
            ObjectArray listeTarihce = ListeleTarihce(ve, kullanan, form);
            string islemKontrol = "";
            foreach (TarihceBilgisi t in listeTarihce.objeler)
            {
                if (t.islem == "Değiştirildi")
                    continue;

                if (t.islem == "B Onayına Gönderildi")
                {
                    form.girisZaman = t.islemTarih;
                    form.girisSicil = t.islemYapan;
                    islemKontrol = "B Onayına Gönderildi";
                }
                else if (t.islem == "A Onayına Gönderildi" && islemKontrol == "B Onayına Gönderildi")
                {
                    form.bOnayZaman = t.islemTarih;
                    form.bOnaySicil = t.islemYapan;
                    islemKontrol = "A Onayına Gönderildi";
                }
                else if (t.islem == "Onaylandı" && (islemKontrol == "A Onayına Gönderildi" || islemKontrol == "Onaylandı"))
                {
                    form.aOnayZaman = t.islemTarih;
                    form.aOnaySicil = t.islemYapan;
                    islemKontrol = "Onaylandı";
                }
                else
                {
                    form.bOnayZaman = new TNSDateTime();
                    form.bOnaySicil = "";
                    form.aOnayZaman = new TNSDateTime();
                    form.aOnaySicil = "";
                }
            }

            if (form.bOnaySicil != "")
            {
                form.aOnayZaman = new TNSDateTime(DateTime.Now);
                form.aOnaySicil = kullanan.mernis;
            }

            //***************************************************


            //decimal tasitAmortismanSiniri = TMMAmortismanSinir.AmortismanSinirTasitVer(ve, kullanan, form.yil);

            string hata = "";

            MuhasebeIslemi m = null;
            ENUMISLEMCINSI islemCinsi = 0;
            Hashtable listeMuhasebe = new Hashtable();

            foreach (MuhasebeIslemiKriter mk in bilgiler.objeler)
            {
                form.aciklama = mk.aciklama;
                //form.mutemmim = mk.mutemmim;

                islemCinsi = ENUMISLEMCINSI.TANIMSIZ;

                m = new MuhasebeIslemi();

                decimal maliyetCariAmortisman = 0;
                decimal enflasyonFarkiCariAmortisman = 0;
                decimal degerArtisCariAmortisman = 0;

                decimal maliyetToplamAmortisman = 0;
                decimal enflasyonFarkiToplamAmortisman = 0;
                decimal degerArtisToplamAmortisman = 0;

                decimal enflasyonFarkiToplam = 0;
                decimal degerArtisToplam = 0;

                decimal kiymetBedeliEdf = 0;
                decimal amortismanBedeliEdf = 0;
                decimal birikmisAmortisman = 0;
                decimal birikmisAmortismanEdf = 0;

                SicilNoDegerArtis degerArtis = new SicilNoDegerArtis();
                foreach (SicilNoDegerArtis da in mk.listeDegerArtis)
                {
                    bool aktarimEDFMi = false; //Aktarım sırasında edf amortismanları için TASAMORTISMAN tablosu DEGERARTISKODU boş olarak aktarıldı. Kontrol ediliyor
                    foreach (AmortismanKriter a in mk.listeAmortisman)
                    {
                        if (a.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE && string.IsNullOrWhiteSpace(a.degerArtisKod) && a.yil == 2000 && da.belgeNo == "12")
                            aktarimEDFMi = true;
                    }

                    if (!aktarimEDFMi && da.muhasebeKod == form.muhasebeKod && da.harcamaKod == form.harcamaKod && da.belgeTarihi.Yil == form.yil && da.belgeNo == form.donem.ToString("000000"))
                    {
                        degerArtis = da;
                        continue;
                    }

                    if (da.tur == 1) //Enflasyon Değer Farkı
                        enflasyonFarkiToplam += (decimal)da.tutar;
                    else
                        degerArtisToplam += (decimal)da.tutar;
                }

                if (form.muhasebeIslemTur == ENUMMuhasebeIslemTur.DEGERARTIS && degerArtis.tutar == 0)
                {
                    hata += "Değer artış bilgisi okunamadı. Lütfen sistem yönetiniz ile iletişime geçiniz.";
                    break;
                }


                foreach (AmortismanKriter a in mk.listeAmortisman)
                {
                    if (a.yil == form.yil && a.donem == form.donem)
                    {
                        //if (a.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE && string.IsNullOrWhiteSpace(a.degerArtisKod)) //Enflasyon Farkı Amortisman
                        if (a.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE) //Enflasyon Farkı Amortisman
                            enflasyonFarkiCariAmortisman += a.amortismanMiktar;
                        else if (a.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME) //Değer Artış Amortisman
                            degerArtisCariAmortisman += a.amortismanMiktar;
                        else
                            maliyetCariAmortisman += a.amortismanMiktar;
                    }

                    //if (a.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE && string.IsNullOrWhiteSpace(a.degerArtisKod)) //Enflasyon Farkı Amortisman
                    if (a.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE) //Enflasyon Farkı Amortisman
                        enflasyonFarkiToplamAmortisman += a.amortismanMiktar;
                    else if (a.amortismanTur == (int)ENUMAmortismanTur.DEGERLEME) //Değer Artış Amortisman
                        degerArtisToplamAmortisman += a.amortismanMiktar;
                    else
                        maliyetToplamAmortisman += a.amortismanMiktar;
                }

                decimal toplamCariAmortisman = maliyetCariAmortisman + degerArtisCariAmortisman + enflasyonFarkiCariAmortisman;

                decimal toplamDegerDuzetmeAmortisman = enflasyonFarkiToplamAmortisman + degerArtisToplamAmortisman;
                decimal toplamAmortisman = maliyetToplamAmortisman + toplamDegerDuzetmeAmortisman;

                decimal degerDuzeltmeToplam = degerArtisToplam + enflasyonFarkiToplam;

                decimal kiymetBedeli = mk.ilkBedel + degerDuzeltmeToplam;

                decimal amortismanKalanTutar = OrtakFonksiyonlar.KurusYap(kiymetBedeli - toplamAmortisman);


                if (form.muhasebeIslemTur == ENUMMuhasebeIslemTur.DEGERARTIS)
                {
                    kiymetBedeli = 0;
                    kiymetBedeliEdf = 0;
                    birikmisAmortisman = 0;
                    birikmisAmortismanEdf = 0;


                    if (degerArtis.tur == 1) //Enflasyon Değer Farkı
                    {
                        kiymetBedeliEdf = (decimal)degerArtis.tutar;
                        foreach (AmortismanKriter a in mk.listeAmortisman)
                            if (a.degerArtisKod == degerArtis.kod)
                                amortismanBedeliEdf += a.amortismanMiktar;
                    }
                    else if (degerArtis.tur == 2) //Değer artış
                    {
                        //decimal katsayi = (decimal)degerArtis.tutar / (mk.ilkBedel + degerArtisToplam);
                        //decimal katsayi = (decimal)degerArtis.tutar * (mk.ilkBedel + degerDuzeltmeToplam);

                        foreach (AmortismanKriter a in mk.listeAmortisman)
                        {
                            if (a.degerArtisKod != degerArtis.kod)
                            {
                                //if (a.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE && string.IsNullOrWhiteSpace(a.degerArtisKod)) //Enflasyon Farkı Amortisman
                                if (a.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE) //Enflasyon Farkı Amortisman
                                    birikmisAmortismanEdf += a.amortismanMiktar;
                                else
                                    birikmisAmortisman += a.amortismanMiktar;
                            }
                        }

                        kiymetBedeli = (decimal)degerArtis.tutar;
                        kiymetBedeliEdf = 0;
                        birikmisAmortisman = 0;
                        birikmisAmortismanEdf = 0;

                        /*
                        //kiymetBedeli = mk.ilkBedel * katsayi;
                        kiymetBedeliEdf = enflasyonFarkiToplam * katsayi;
                        //kiymetBedeliEdf = (decimal)degerArtis.tutar - kiymetBedeli;
                        birikmisAmortisman = birikmisAmortisman * katsayi;
                        birikmisAmortismanEdf = birikmisAmortismanEdf * katsayi;
                        */
                    }
                    else if (degerArtis.tur == 3) //Değer azalış
                    {
                        //decimal katsayi = ((decimal)degerArtis.tutar * -1) / (mk.ilkBedel + degerArtisToplam);
                        decimal katsayi = ((decimal)degerArtis.tutar * -1) / (mk.ilkBedel);

                        foreach (AmortismanKriter a in mk.listeAmortisman)
                        {
                            if (a.degerArtisKod != degerArtis.kod)
                            {
                                //if (a.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE && string.IsNullOrWhiteSpace(a.degerArtisKod)) //Enflasyon Farkı Amortisman
                                if (a.amortismanTur == (int)ENUMAmortismanTur.FEVKALADE) //Enflasyon Farkı Amortisman
                                    birikmisAmortismanEdf += a.amortismanMiktar;
                                else
                                    birikmisAmortisman += a.amortismanMiktar;
                            }
                        }

                        kiymetBedeli = (decimal)degerArtis.tutar;
                        //kiymetBedeli = mk.ilkBedel * katsayi;
                        kiymetBedeliEdf = enflasyonFarkiToplam * katsayi;
                        //kiymetBedeliEdf = (decimal)degerArtis.tutar - kiymetBedeli;
                        birikmisAmortisman = birikmisAmortisman * katsayi;
                        birikmisAmortismanEdf = birikmisAmortismanEdf * katsayi;
                    }
                }


                if (form.muhasebeIslemTur == ENUMMuhasebeIslemTur.TASINIRISLEMFISI)
                {
                    bool izBedel = form.islemTipKod == (int)ENUMIslemTipi.BAGISGIRIS;

                    bool geciciIhrac = form.islemTipKod == (int)ENUMIslemTipi.GECICIIHRAC;

                    bool sanatEseri = mk.hesapPlanKod.StartsWith("0880106") || mk.hesapPlanAd.StartsWith("SANAT ESERLERİ");


                    //İhraç - Satış ve Geçici İhraç
                    if (geciciIhrac || mk.islemTurKod == (int)ENUMIslemTipi.HURDACIKIS || mk.islemTurKod == (int)ENUMIslemTipi.DEVIRCIKISKURUM || mk.islemTurKod == (int)ENUMIslemTipi.SATISCIKIS) //56 - İhraç
                    {
                        /* Kapatıldı
                            //Satış işleminde fiyat değer artış ile birlikte geliyor.
                            kiymetBedeli = mk.kdvliBirimFiyat;
                            amortismanKalanTutar = kiymetBedeli - toplamAmortisman;
                        */

                        //SABİT KIYMET SATIŞ VE İHRAÇ
                        if (mb.ArsaMi(mk.hesapPlanKod) && !geciciIhrac)    //ARSA
                            islemCinsi = ENUMISLEMCINSI.ARSASATIS;
                        else if (mb.BinaMi(mk.hesapPlanKod) && !geciciIhrac) //GAYRIMENKUL
                        {
                            if (amortismanKalanTutar > 0)
                            {
                                if (mk.islemTurKod == (int)ENUMIslemTipi.SATISCIKIS)
                                    islemCinsi = ENUMISLEMCINSI.GAYRIMENKULTAMAMIAMORTIEDILMEMISSATIS; //Tamamı Amorti Edilmemiş Gayrimenkul Satış
                                else
                                    islemCinsi = ENUMISLEMCINSI.GAYRIMENKULTAMAMIAMORTIEDILMEMISIHRAC; //Tamamı Amorti Edilmemiş Gayrimenkul İhraç

                            }
                            else if (amortismanKalanTutar == 0)
                            {
                                if (mk.islemTurKod == (int)ENUMIslemTipi.SATISCIKIS)
                                    islemCinsi = ENUMISLEMCINSI.GAYRIMENKULTAMAMIAMORTIEDILMISSATIS; //Tamamı Amorti Gayrimenkul Satış
                                else
                                    islemCinsi = ENUMISLEMCINSI.GAYRIMENKULTAMAMIAMORTIEDILMISIHRAC; //Tamamı Amorti Gayrimenkul İhraç

                            }
                        }
                        else if (mb.YazilimMi(mk.hesapPlanKod)) //YAZILIM
                        {
                            if (amortismanKalanTutar > 0)
                            {
                                if (mk.islemTurKod == (int)ENUMIslemTipi.SATISCIKIS)
                                    islemCinsi = ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMEMISSATIS; //Tamamı Amorti Edilmemiş Yazılım Satış
                                else
                                {
                                    //Geçici İhraç Kontrolü
                                    if (geciciIhrac)
                                        islemCinsi = ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMEMISIHRACHESABINAALMA; //Tamamı Amorti Edilmemiş Yazılım İhraç Hesabına Alınması
                                    else if (mk.geciciIhrac == 1)
                                        islemCinsi = ENUMISLEMCINSI.YAZILIMIHRACHESABINDANIHRAC; //Yazılım İhraç Hesabından İhraç
                                    else
                                        islemCinsi = ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMEMISIHRAC; //Tamamı Amorti Edilmemiş Yazılım İhraç
                                }
                            }
                            else if (amortismanKalanTutar == 0)
                            {
                                if (mk.islemTurKod == (int)ENUMIslemTipi.SATISCIKIS)
                                    islemCinsi = ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMISSATIS; //Tamamı Amorti Yazılım Satış
                                else
                                {
                                    //Geçici İhraç Kontrolü
                                    if (geciciIhrac)
                                        islemCinsi = ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMISIHRACHESABINAALMA; //Tamamı Amorti Yazılım İhraç Hesabına Alınması
                                    else if (mk.geciciIhrac == 1)
                                        islemCinsi = ENUMISLEMCINSI.YAZILIMIHRACHESABINDANIHRAC; //Yazılım İhraç Hesabından İhraç
                                    else
                                        islemCinsi = ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMISIHRAC; //Tamamı Amorti Yazılım İhraç
                                }
                            }
                        }
                        else if (sanatEseri && !geciciIhrac)
                        {
                            if (mk.islemTurKod == (int)ENUMIslemTipi.SATISCIKIS)
                            {
                                if (toplamAmortisman > 0)
                                    islemCinsi = ENUMISLEMCINSI.LDHMSNSATISAMORTISMANLI; //Sanat Eseri Amortismanı Olan Satış
                                else
                                    islemCinsi = ENUMISLEMCINSI.LDHMSNSATISAMORTISMANSIZ; //Sanat Eseri Amortismanı Olmayan Satış

                            }
                            else if (mk.islemTurKod == (int)ENUMIslemTipi.DEVIRCIKISKURUM)
                            {
                                if (toplamAmortisman > 0)
                                    islemCinsi = ENUMISLEMCINSI.LDHMSNIHRACAMORTISMANLI; //Sanat Eseri Amortismanı Olan İhraç
                                else
                                    islemCinsi = ENUMISLEMCINSI.LDHMSNIHRACAMORTISMANSIZ; //Sanat Eseri Amortismanı Olmayan İhraç
                            }
                        }
                        else
                        {
                            if (amortismanKalanTutar > 0)
                            {
                                if (mk.islemTurKod == (int)ENUMIslemTipi.SATISCIKIS)
                                    islemCinsi = ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMEMISSATIS; //Tamamı Amorti Edilmemiş Demirbaş Satış
                                else
                                {
                                    //Geçici İhraç Kontrolü
                                    if (geciciIhrac)
                                        islemCinsi = ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMEMISIHRACHESABINAALMA; //Tamamı Amorti Edilmemiş Demirbaş İhraç Hesabına Alınması
                                    else if (mk.geciciIhrac == 1)
                                        islemCinsi = ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMEMISIHRACHESABINDANIHRAC; //Tamamı Amorti Edilmemiş Demirbaş İhraç Hesabından İhraç
                                    else
                                        islemCinsi = ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMEMISIHRAC; //Tamamı Amorti Edilmemiş Demirbaş İhraç
                                }
                            }
                            else
                            {
                                if (mk.islemTurKod == (int)ENUMIslemTipi.SATISCIKIS)
                                    islemCinsi = ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMISSATIS; //Tamamı Amorti Demirbaş Satış
                                else
                                {
                                    //Geçici İhraç Kontrolü
                                    if (geciciIhrac)
                                        islemCinsi = ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMISIHRACHESABINAALMA; //Tamamı Amorti Demirbaş İhraç Hesabına Alınması
                                    else if (mk.geciciIhrac == 1)
                                        //islemCinsi = ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMISIHRACHESABINDANIHRAC; //Tamamı Amorti Demirbaş İhraç Hesabından İhraç
                                        islemCinsi = ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMEMISIHRACHESABINDANIHRAC; //Tamamı Amorti Edilmemiş Demirbaş İhraç Hesabından İhraç
                                    else
                                        islemCinsi = ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMISIHRAC; //Tamamı Amorti Demirbaş İhraç
                                }
                            }
                        }

                        //m.kiymetBedeli = mk.kdvliBirimFiyat;
                        m.kiymetBedeli = mk.ilkBedel + degerArtisToplam;
                        m.ilkBedeli = mk.ilkBedel;
                        m.kiymetBedeliEdf = enflasyonFarkiToplam;
                        m.birikmisAmortisman = toplamAmortisman - enflasyonFarkiToplamAmortisman;
                        m.birikmisAmortismanEdf = enflasyonFarkiToplamAmortisman;
                        m.satisBedeli = mk.satisBedeli;
                    }
                    else if (izBedel)
                    {
                        if (mb.YazilimMi(mk.hesapPlanKod)) //YAZILIM
                            islemCinsi = ENUMISLEMCINSI.YAZILIMIZBEDELIGIRIS;
                        else
                            islemCinsi = ENUMISLEMCINSI.DEMIRBASIZBEDELIGIRIS;
                        m.kiymetBedeli = kiymetBedeli;
                    }
                    else if (sanatEseri)
                    {
                        islemCinsi = ENUMISLEMCINSI.LDHMSANATESERIGIRIS;
                        m.kiymetBedeli = kiymetBedeli;
                    }
                    else
                    {
                        //if (mk.kdvliBirimFiyat == (decimal)0.01)
                        //    islemCinsi = ENUMISLEMCINSI.IZBEDELIGIRIS;
                        //else
                        //{
                        if (mb.BinaMi(mk.hesapPlanKod)) //GAYRIMENKUL
                        {
                            if (mk.islemTurKod == 1 || mk.islemTurKod == 2 || mk.islemTurKod == 49)
                                islemCinsi = ENUMISLEMCINSI.GAYRIMENKULGIRIS;
                        }
                        else if (mb.ArsaMi(mk.hesapPlanKod))    //ARSA
                        {
                            if (mk.islemTurKod == 1 || mk.islemTurKod == 2 || mk.islemTurKod == 49)
                                islemCinsi = ENUMISLEMCINSI.ARSAGIRIS;
                        }
                        else if (mb.YazilimMi(mk.hesapPlanKod)) //YAZILIM
                            islemCinsi = ENUMISLEMCINSI.YAZILIMGIRIS;
                        else
                            islemCinsi = ENUMISLEMCINSI.DEMIRBASGIRIS;
                        //}
                        m.kiymetBedeli = kiymetBedeli;
                    }
                }
                else if (form.muhasebeIslemTur == ENUMMuhasebeIslemTur.DEGERARTIS && (degerArtis.tur == 2 || degerArtis.tur == 3))
                {
                    //degerArtis.tur - 2: Değer Artışı , 3: Değer Azaltma

                    bool artisMi = degerArtis.tutar > 0 ? true : false;

                    bool sanatEseri = mk.hesapPlanKod.StartsWith("0880106") || mk.hesapPlanAd.StartsWith("SANAT ESERLERİ");

                    if (sanatEseri)
                    {
                        if (artisMi)
                            islemCinsi = ENUMISLEMCINSI.LDHMSNARTIR;
                        else
                            islemCinsi = ENUMISLEMCINSI.LDHMSNAZALT;
                    }
                    else if (mb.BinaMi(mk.hesapPlanKod) || mb.ArsaMi(mk.hesapPlanKod)) //GAYRIMENKUL veya ARSA
                    {
                        if (artisMi)
                            islemCinsi = ENUMISLEMCINSI.GAYRIMENKULBEDELARTIRMA;
                        else
                            islemCinsi = ENUMISLEMCINSI.GAYRIMENKULBEDELAZALTMA;
                    }
                    else if (mb.YazilimMi(mk.hesapPlanKod)) //YAZILIM
                    {
                        if (artisMi)
                            islemCinsi = ENUMISLEMCINSI.YAZILIMBEDELARTIRMA;
                        else
                            islemCinsi = ENUMISLEMCINSI.YAZILIMBEDELAZALTMA;
                    }
                    else
                    {
                        if (artisMi)
                            islemCinsi = ENUMISLEMCINSI.DEMIRBASBEDELARTIRMA;
                        else
                            islemCinsi = ENUMISLEMCINSI.DEMIRBASBEDELAZALTMA;
                    }

                    m.kiymetBedeli = kiymetBedeli;
                    m.kiymetBedeliEdf = kiymetBedeliEdf;
                    m.birikmisAmortisman = birikmisAmortisman;
                    m.birikmisAmortismanEdf = birikmisAmortismanEdf;

                    //07.05.2019 Gayrimenkul şablon düzelmesi için gelen mail nedeniyle eklendi. (H.Ö)
                    if (artisMi && islemCinsi == ENUMISLEMCINSI.GAYRIMENKULBEDELARTIRMA)
                    {
                        m.kiymetBedeli = kiymetBedeliEdf + kiymetBedeli;
                        m.kiymetBedeliEdf = 0;
                        m.birikmisAmortisman = 0;
                        m.birikmisAmortismanEdf = 0;
                    }

                }
                else if (form.muhasebeIslemTur == ENUMMuhasebeIslemTur.DEGERARTIS && degerArtis.tur == 1)
                {
                    //degerArtis.tur - 1: Enflasyon Artışı

                    if (mb.BinaMi(mk.hesapPlanKod) || mb.ArsaMi(mk.hesapPlanKod)) //GAYRIMENKUL veya ARSA
                        islemCinsi = ENUMISLEMCINSI.GAYRIMENKULENFLASYONDUZELTMESI;
                    else if (mb.YazilimMi(mk.hesapPlanKod)) //YAZILIM
                        islemCinsi = ENUMISLEMCINSI.YAZILIMENFLASYONDUZELTMESI;
                    else
                        islemCinsi = ENUMISLEMCINSI.DEMIRBASENFLASYONDUZELTMESI;

                    m.kiymetBedeliEdf = kiymetBedeliEdf;
                    m.amortismanBedeliEdf = amortismanBedeliEdf;
                }
                else if (form.muhasebeIslemTur == ENUMMuhasebeIslemTur.ENFLASYONDUZELTMESI)
                {
                    bool sanatEseri = mk.hesapPlanKod.StartsWith("0880106") || mk.hesapPlanAd.StartsWith("SANAT ESERLERİ");
                    bool yazilim = mb.YazilimMi(mk.hesapPlanKod); //YAZILIM
                    bool bina = mb.BinaMi(mk.hesapPlanKod); //GAYRIMENKUL
                    bool arsa = mb.ArsaMi(mk.hesapPlanKod); //ARSA
                    bool tasit = mb.TasitMi(mk.hesapPlanKod); //TAŞIT

                    kiymetBedeliEdf = enflasyonFarkiToplam;
                    amortismanBedeliEdf = enflasyonFarkiToplamAmortisman;

                    if (sanatEseri)
                    {
                        islemCinsi = ENUMISLEMCINSI.LDHMSEENFDUZELT;
                        m.kiymetBedeliEdf = kiymetBedeliEdf;
                        TurDoldur(islemCinsi, m, mk);
                        ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                        m = new MuhasebeIslemi();


                        islemCinsi = ENUMISLEMCINSI.LDHMSEAMORTIENFDUZELT;
                        m.amortismanBedeliEdf = amortismanBedeliEdf;
                        TurDoldur(islemCinsi, m, mk);
                        ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                        m = new MuhasebeIslemi();

                        islemCinsi = ENUMISLEMCINSI.LDHMSEENFDUZELTKZ;
                        m.kiymetBedeliEdf = kiymetBedeliEdf;
                        m.amortismanBedeliEdf = amortismanBedeliEdf;
                        TurDoldur(islemCinsi, m, mk);
                        ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                        m = new MuhasebeIslemi();
                    }
                    else if (bina)
                    {
                        islemCinsi = ENUMISLEMCINSI.LDHMBINAENFDUZELT;
                        m.kiymetBedeliEdf = kiymetBedeliEdf;
                        TurDoldur(islemCinsi, m, mk);
                        ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                        m = new MuhasebeIslemi();


                        islemCinsi = ENUMISLEMCINSI.LDHMBINAAMORTIENFDUZELT;
                        m.amortismanBedeliEdf = amortismanBedeliEdf;
                        TurDoldur(islemCinsi, m, mk);
                        ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                        m = new MuhasebeIslemi();

                        islemCinsi = ENUMISLEMCINSI.LDHMBINAENFDUZELTKZ;
                        m.kiymetBedeliEdf = kiymetBedeliEdf;
                        m.amortismanBedeliEdf = amortismanBedeliEdf;
                        TurDoldur(islemCinsi, m, mk);
                        ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                        m = new MuhasebeIslemi();
                    }
                    else if (arsa)
                    {
                        islemCinsi = ENUMISLEMCINSI.LDHMARSAENFDUZELT;
                        m.kiymetBedeliEdf = kiymetBedeliEdf;
                        TurDoldur(islemCinsi, m, mk);
                        ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                        m = new MuhasebeIslemi();


                        islemCinsi = ENUMISLEMCINSI.LDHMARSAENFDUZELTK;
                        m.kiymetBedeliEdf = kiymetBedeliEdf;
                        TurDoldur(islemCinsi, m, mk);
                        ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                        m = new MuhasebeIslemi();
                    }
                    else if (tasit)
                    {
                        islemCinsi = ENUMISLEMCINSI.LDHMTASITAMORTISMANKKEGZR;
                        m.kiymetBedeliEdf = kiymetBedeliEdf;
                        m.amortismanBedeliEdf = amortismanBedeliEdf;
                        TurDoldur(islemCinsi, m, mk);
                        ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                        m = new MuhasebeIslemi();
                    }
                    else
                    {
                        //Demirbaş

                        islemCinsi = ENUMISLEMCINSI.LDHMDBENFDUZELT;
                        m.kiymetBedeliEdf = kiymetBedeliEdf;
                        TurDoldur(islemCinsi, m, mk);
                        ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                        m = new MuhasebeIslemi();


                        islemCinsi = ENUMISLEMCINSI.LDHMDBAMORTIENFDUZELT;
                        m.amortismanBedeliEdf = amortismanBedeliEdf;
                        TurDoldur(islemCinsi, m, mk);
                        ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                        m = new MuhasebeIslemi();

                        islemCinsi = ENUMISLEMCINSI.LDHMDBENFDUZELTKZ;
                        m.kiymetBedeliEdf = kiymetBedeliEdf;
                        m.amortismanBedeliEdf = amortismanBedeliEdf;
                        TurDoldur(islemCinsi, m, mk);
                        ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                        m = new MuhasebeIslemi();
                    }



                }
                else if (form.muhasebeIslemTur == ENUMMuhasebeIslemTur.AMORTISMAN)
                {
                    bool geciciIhrac = mk.geciciIhrac == 1;

                    if (amortismanKalanTutar == 0)
                    {
                        //TAMAMI AMORTİ EDİLMİŞ HESAPLARA AKTARIM
                        //Tamamı amorti edilmiş kayıtlar için öncelikle bedelAktarma yapılacak sonrasında amortisman aktarma yapılacak.


                        //TurDoldur(islemCinsi, m, mk);
                        //ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                        //m = new MuhasebeIslemi();

                        //1-Bedel Aktarma
                        if (mb.BinaMi(mk.hesapPlanKod)) //Gayrimenkul
                        {
                            //Tamamı Amorti Edilmiş Hesaplara Aktarım - e.	Gayrimenkul Bedel Aktarma
                            islemCinsi = ENUMISLEMCINSI.GAYRIMENKULBEDELAKTARMA;
                        }
                        else if (mb.YazilimMi(mk.hesapPlanKod)) //Yazılım
                        {
                            //Tamamı Amorti Edilmiş Hesaplara Aktarım - c.	Yazılım Bedel Aktarma
                            islemCinsi = ENUMISLEMCINSI.YAZILIMBEDELAKTARMA;
                        }
                        else
                        {
                            //Tamamı Amorti Edilmiş Hesaplara Aktarım - a.	Demirbaş Bedel Aktarma
                            islemCinsi = ENUMISLEMCINSI.DEMIRBASBEDELAKTARMA;
                        }

                        m.kiymetBedeli = kiymetBedeli;
                        m.kiymetBedeliEdf = enflasyonFarkiToplam;

                        TurDoldur(islemCinsi, m, mk);
                        ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                        m = new MuhasebeIslemi();

                        //2-Amortisman Aktarma
                        if (mb.BinaMi(mk.hesapPlanKod)) //Gayrimenkul
                        {
                            //Tamamı Amorti Edilmiş Hesaplara Aktarım - f.	Gayrimenkul Amortisman Aktarma
                            islemCinsi = ENUMISLEMCINSI.GAYRIMENKULAMORTISMANAKTARMA;
                            m.amortismanBedeli = toplamAmortisman - enflasyonFarkiToplamAmortisman;

                        }
                        else if (mb.YazilimMi(mk.hesapPlanKod)) //Yazılım
                        {
                            //Tamamı Amorti Edilmiş Hesaplara Aktarım - d.	Yazılım Amortisman Aktarma
                            islemCinsi = ENUMISLEMCINSI.YAZILIMAMORTISMANAKTARMA;
                            m.amortismanBedeli = toplamAmortisman;
                        }
                        else
                        {
                            //Tamamı Amorti Edilmiş Hesaplara Aktarım - b.	Demirbaş Amortisman Aktarma
                            islemCinsi = ENUMISLEMCINSI.DEMIRBASAMORTISMANAKTARMA;
                            m.amortismanBedeli = toplamAmortisman;
                        }

                        //m.amortismanBedeli = toplamAmortisman; //Gayrimenkul amortisman aktarma işleminde şablona göre amortismanBedeli = amortismanBedeli-amortismanBedeliEdf olacak. HÖ - 2022.06.09
                        m.amortismanBedeliEdf = enflasyonFarkiToplamAmortisman;


                        TurDoldur(islemCinsi, m, mk);
                        ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                        m = new MuhasebeIslemi();
                    }
                    else if (geciciIhrac)
                    {
                        if (mb.YazilimMi(mk.hesapPlanKod)) //YAZILIM
                            islemCinsi = ENUMISLEMCINSI.LDHMYZGECICIAMORTI;
                        else
                            islemCinsi = ENUMISLEMCINSI.LDHMDBGECICIAMORTI;

                        m.amortismanTutari = toplamCariAmortisman;

                        TurDoldur(islemCinsi, m, mk);
                        ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                        m = new MuhasebeIslemi();
                    }

                    bool izBedel = mk.islemTurKod == (int)ENUMIslemTipi.BAGISGIRIS;

                    if (izBedel)
                    {
                        if (mb.YazilimMi(mk.hesapPlanKod)) //YAZILIM
                            islemCinsi = ENUMISLEMCINSI.YAZILIMIZBEDELIAMORTISMAN;
                        else
                            islemCinsi = ENUMISLEMCINSI.DEMIRBASIZBEDELIAMORTISMAN;
                        m.kiymetBedeli = kiymetBedeli;

                    }
                    else if (mb.TasitMi(mk.hesapPlanKod) && mk.tasitAmortismanSiniri > 0) //TAŞIT amortismanına ait şablonda çalışacak
                    {
                        //TurDoldur(islemCinsi, m, mk);
                        //ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                        //m = new MuhasebeIslemi();

                        islemCinsi = ENUMISLEMCINSI.TASITAMORTISMAN;

                        bool idareMerkezi = mb.IdareMerkeziMi(mk.harcamaKod);
                        bool banknotBirimi = mb.BanknotBirimiMi(mk.harcamaKod);
                        bool istMerkezBank = mb.IstMerkezBankasiMi(mk.nereyeGitti);     //İE0016U10 - 1563 - İST.MRK.BANK.AR.EĞ.M
                        bool izmirOzdereMi = mb.OzdereMi(mk.nereyeGitti);               //1519, 1069 -  İzmir Özdere
                        bool ankaraSosyalTesisMi = mb.AnkaraSosyalMi(mk.nereyeGitti);   //1518, 1063  - Ankara Sosyal Tesis
                        //bool izmirOzdereMi = mk.nereyeGitti == "1519";          //İzmir Özdere
                        //bool ankaraSosyalTesisMi = mk.nereyeGitti == "1063";    //Ankara Sosyal Tesis


                        decimal tasitAmortisman = toplamCariAmortisman;
                        decimal tasitKayitBedel = kiymetBedeli;

                        //if (ankaraSosyalTesisMi)
                        //    m.amortismanSos = tasitAmortisman;
                        //else if (izmirOzdereMi)
                        //    m.amortismanOzdr = tasitAmortisman;

                        /*
    İdare Merkezi ve Şubeler İçin
    DB (Demirbaşın kayıtlı bedeli) >= 300.000 ise  AMORTİSMAN TUTARI nın 15.000 TL ye kadar olan kısmı
    DB (Demirbaşın kayıtlı bedeli) <  300.000 ise  AMORTİSMAN TUTARI nın 15.000 TL yi aşan kısmı 

    Banknot Matbaası GM için
    DB (Demirbaşın kayıtlı bedeli) >= 300.000 ise  AMORTİSMAN TUTARI nın 5.000 TL ye kadar olan kısmı
    DB (Demirbaşın kayıtlı bedeli) <  300.000 ise  AMORTİSMAN TUTARI nın 5.000 TL yi aşan kısmı 

 İdare Merkezi ve Şubeler İçin

    AMORTİSMAN1
    DB (Demirbaşın kayıtlı bedeli) >= 300.000 ise  AMORTİSMAN TUTARI nın 15.000 TL ye kadar olan kısmı

    AMORTİSMAN1 *
    DB (Demirbaşın kayıtlı bedeli) >= 300.000 ise  AMORTİSMAN TUTARI nın 5.000 TL ye kadar olan kısmı x (a)

    AMORTİSMAN2 *
    DB (Demirbaşın kayıtlı bedeli) >= 300.000 ise  AMORTİSMAN TUTARI nın 5.000 TL yi aşan kısmı x (a)

    AMORTİSMAN2
    DB (Demirbaşın kayıtlı bedeli) >= 300.000 ise  AMORTİSMAN TUTARI nın 15.000 TL yi aşan kısmı

    AMORTİSMAN1
    DB (Demirbaşın kayıtlı bedeli) < 300.000 ise  AMORTİSMAN TUTARI nın 15.000 TL ye kadar olan kısmı

    SON YIL İLAVE OLARAK

    AMORTİSMAN1 *
    DB (Demirbaşın kayıtlı bedeli) >= 300.000 ise  AMORTİSMAN TUTARI nın 5.000 TL ye kadar olan kısmı x (3-a)

    AMORTİSMAN2 *
    DB (Demirbaşın kayıtlı bedeli) >= 300.000 ise  AMORTİSMAN TUTARI nın 5.000 TL yi aşan kısmı x (3-a)


    * a: Kıst amortismanda Varlığın alındığı yılda ilişkin kıst amortisman hesaplanan ay sayısı. 
         Faydalı ömür bittiğinde ilk yıl için hesaplanmamış olan tüm amortismanlar son yıl amorti edilir. Bu mevcut uygulamada da böyle zaten.


*/
                        //192.202,98 TL - 2018-09-11 08:53:49.527 - BİNEK OTOSU - 155896
                        //9.610,15 TL 3 aylık amortismanı
                        //3.203,38 TL 1 aylık amortismanı

                        //ilk amortisman : 3.203,38
                        //son amortisman : 35.237,20



                        //Kıst amortisman olmayan olması gereken amortisman tutarı
                        decimal aylikNormalCariAmortisman = OrtakFonksiyonlar.KurusYap(tasitKayitBedel / (5 * 12)); //Amortisman ömrü * dönem
                        decimal kontrolDegeri = OrtakFonksiyonlar.KurusYap(mk.tasitAmortismanSiniri / (5 * 12)); //Amortisman ömrü * dönem


                        if (!banknotBirimi)
                        {
                            int tasit_donem = (int)(OrtakFonksiyonlar.KurusYap(tasitAmortisman / aylikNormalCariAmortisman));

                            if (tasit_donem > 3)
                            {
                                int ilk_donem = 3 - tasit_donem % 3;

                                decimal ilkKontrolDegeri = OrtakFonksiyonlar.KurusYap(mk.tasitAmortismanSiniri / (5 * 12)) * ilk_donem;

                                decimal sonKontrolDegeri = (mk.tasitAmortismanSiniri / (5 * 12)) * (tasit_donem + ilk_donem) - ilkKontrolDegeri;

                                kontrolDegeri = sonKontrolDegeri;
                            }
                            else
                                kontrolDegeri *= tasit_donem;

                            string yuvarlaKontrol = kontrolDegeri.ToString();
                            if (yuvarlaKontrol.EndsWith(",99") || yuvarlaKontrol.EndsWith(".99"))
                                kontrolDegeri += (decimal)0.01;
                        }


                        if (tasitKayitBedel >= mk.tasitAmortismanSiniri)
                        {
                            if (tasitAmortisman >= kontrolDegeri)
                            {
                                m.amortisman1 = kontrolDegeri;
                                m.amortisman2 = tasitAmortisman - kontrolDegeri;
                            }
                            else
                            {
                                m.amortisman1 = tasitAmortisman;
                                m.amortisman2 = 0;
                            }
                        }
                        else
                        {
                            m.amortisman1 = tasitAmortisman;
                            m.amortisman2 = 0;
                        }

                    }
                    else
                    {

                        bool kanunenKabulEdilmeyenYer = mb.SosyalTesisVarMi(mk.nereyeGitti);

                        if (mb.BinaMi(mk.hesapPlanKod) || mb.ArsaMi(mk.hesapPlanKod)) //GAYRIMENKUL veya ARSA
                        {
                            if (kanunenKabulEdilmeyenYer)
                            {
                                islemCinsi = ENUMISLEMCINSI.GAYRIMENKULAMORTISMANAYIRMAKKEG;
                                YerDoldur(islemCinsi, m, mk);
                            }
                            else
                                islemCinsi = ENUMISLEMCINSI.GAYRIMENKULAMORTISMANAYIRMA;

                        }
                        else if (mb.YazilimMi(mk.hesapPlanKod)) //YAZILIM
                        {
                            if (kanunenKabulEdilmeyenYer)
                            {
                                islemCinsi = ENUMISLEMCINSI.YAZILIMAMORTISMANAYIRMAKKEG;
                                YerDoldur(islemCinsi, m, mk);
                            }
                            else
                                islemCinsi = ENUMISLEMCINSI.YAZILIMAMORTISMANAYIRMA;
                        }
                        else
                        {
                            if (kanunenKabulEdilmeyenYer)
                            {
                                islemCinsi = ENUMISLEMCINSI.DEMIRBASAMORTISMANAYIRMAKKEG;
                                YerDoldur(islemCinsi, m, mk);
                            }
                            else
                                islemCinsi = ENUMISLEMCINSI.DEMIRBASAMORTISMANAYIRMA;
                        }

                        //Bilgi: toplamCariAmortisman = maliyetCariAmortisman + degerArtisCariAmortisman + enflasyonFarkiCariAmortisman;
                        //m.amortismanTutari = toplamCariAmortisman; //RE: Enflasyon Düzeltmesi mail. 2024.09.16 tarihli mail ile amortisman şablonları değiştirilmiştir.

                        m.amortismanTutari = maliyetCariAmortisman + degerArtisCariAmortisman;
                        m.amortismanTutariEdf = enflasyonFarkiCariAmortisman;
                    }


                }


                if (form.muhasebeIslemTur != ENUMMuhasebeIslemTur.ENFLASYONDUZELTMESI)
                {
                    TurDoldur(islemCinsi, m, mk);
                    ListeEkle(listeMuhasebe, islemCinsi, ve, kullanan, form, servisKod, mk.prSicilNo, m, onIzlemeOlustur);
                }
            }

            Sonuc sonuc = new Sonuc(true, hata, "");

            if (listeMuhasebe.Count > 0)
            {
                if (onIzlemeOlustur)
                {
                    foreach (DictionaryEntry de in listeMuhasebe)
                    {
                        MuhasebeIslemi mi = (MuhasebeIslemi)de.Value;

                        mi.islemRefNo = "Muhasebe kaydı öncesi ön izleme içindir - Referans Numarası A Onayında verilecek";

                        mi.json = JsonOlustur(mi);
                        kaydetSonuc.objeler.Add(mi);
                    }
                }
                else
                {
                    foreach (DictionaryEntry de in listeMuhasebe)
                    {
                        m = (MuhasebeIslemi)de.Value;
                        sonuc = Kaydet(ve, kullanan, m);

                        if (!sonuc.islemSonuc)
                        {
                            HataDosyaYaz("Kayıt-Liste-Hata", sonuc.hataStr);
                            break;
                        }
                    }
                }
            }
            else
                sonuc.DegerAta("Muhasebe kaydı oluşturulacak bilgi bulunamadı.");

            if (!sonuc.islemSonuc)
                HataDosyaYaz("Kayıt-Hata", sonuc.hataStr);

            ve.VeriErisimKapat();

            kaydetSonuc.sonuc = sonuc;

            return kaydetSonuc;
        }

        private static void ListeEkle(Hashtable listeCins, ENUMISLEMCINSI islemCinsi, VeriErisim veriErisim, Kullanici kullanan, MuhasebeIslemiKriter form, string servisKod, int prSicilNo, MuhasebeIslemi bilgi, bool onIzlemeOlustur)
        {
            MuhasebeIslemi m = null;
            Sonuc sonuc = null;

            string key = (int)islemCinsi * 100 + "-" + bilgi.yer + "-" + bilgi.tur + "-" + bilgi.tura + "-" + bilgi.turEdf + "-" + bilgi.turEdfa + "-" + bilgi.turKedf + "-" + bilgi.banknot;

            if (!listeCins.ContainsKey(key))
            {
                m = new MuhasebeIslemi();

                if (!onIzlemeOlustur)
                {
                    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

                    sonuc = IslemRefNoAl(ve, kullanan, form.yil, form.harcamaKod, form.muhasebeIslemTur);
                    if (sonuc.islemSonuc)
                        m.islemRefNo = sonuc.anahtar;

                    ve.VeriErisimKapat();
                }

                m.islemCinsiKod = (int)islemCinsi;
                m.islemCinsi = IslemCinsiAdGetir(m.islemCinsiKod);
                m.servis = servisKod;
                m.durum = (int)ENUMMuhasebeIslemiOnayDurumu.TANIMSIZ;
                m.islemYapan = kullanan.kullaniciKodu;
                m.islemTarih = new TNSDateTime(DateTime.Now);

                m.kriter.girisZaman = form.girisZaman;
                m.kriter.girisSicil = form.girisSicil;
                m.kriter.bOnayZaman = form.bOnayZaman;
                m.kriter.bOnaySicil = form.bOnaySicil;
                m.kriter.aOnayZaman = form.aOnayZaman;
                m.kriter.aOnaySicil = form.aOnaySicil;

                m.tur = bilgi.tur;
                m.yer = bilgi.yer;
                m.turEdf = bilgi.turEdf;
                m.turEdfa = bilgi.turEdfa;
                m.tura = bilgi.tura;
                m.turKedf = bilgi.turKedf;
                m.banknot = bilgi.banknot;

                m.aciklama = form.aciklama;
                m.mutemmim = (form.mutemmim == 1 ? true : false);

                listeCins.Add(key, m);
            }

            m = (MuhasebeIslemi)listeCins[key];

            m.detay.Ekle(new MuhasebeIslemiDetay() { prSicilNo = prSicilNo });

            m.kiymetBedeli += bilgi.kiymetBedeli;
            m.amortismanBedeliEdf += bilgi.amortismanBedeliEdf;
            m.amortismanBedeli += bilgi.amortismanBedeli;
            m.kiymetBedeliEdf += bilgi.kiymetBedeliEdf;
            m.amortismanTutari += bilgi.amortismanTutari;
            m.amortismanTutariEdf += bilgi.amortismanTutariEdf;
            m.satisBedeli += bilgi.satisBedeli;

            m.ilkBedeli += bilgi.ilkBedeli;
            m.birikmisAmortisman += bilgi.birikmisAmortisman;
            m.birikmisAmortismanEdf += bilgi.birikmisAmortismanEdf;

            m.amortisman1 += bilgi.amortisman1;
            m.amortisman2 += bilgi.amortisman2;
            m.amortismanSos += bilgi.amortismanSos;
            m.amortismanOzdr += bilgi.amortismanOzdr;

        }

        private static void TurDoldur(ENUMISLEMCINSI islemCinsi, MuhasebeIslemi m, MuhasebeIslemiKriter mk)
        {
            bool idareMerkezi = MerkezBankasiGenel.Ayarlar.IdareMerkeziMi(mk.harcamaKod);
            bool banknotBirimi = MerkezBankasiGenel.Ayarlar.BanknotBirimiMi(mk.harcamaKod);
            bool istMerkezBank = MerkezBankasiGenel.Ayarlar.IstMerkezBankasiMi(mk.nereyeGitti); //İE0016U10 - 1563 - İST.MRK.BANK.AR.EĞ.M

            bool izmirOzdereMi = MerkezBankasiGenel.Ayarlar.OzdereMi(mk.nereyeGitti);               //1519, 1069 -  İzmir Özdere
            //bool izmirOzdereMi = mk.nereyeGitti == "1519"; // "KİK2400S21"; //İzmir Özdere
            //m.yer = MerkezBankasi.Ayarlar.MuhasebeYerKoduGetir(mk.nereyeGitti);

            switch (islemCinsi)
            {
                case ENUMISLEMCINSI.GAYRIMENKULGIRIS:           //LDHMGMGIRIS
                case ENUMISLEMCINSI.GAYRIMENKULBEDELAZALTMA:
                    m.tur = istMerkezBank ? "3" : (idareMerkezi ? "1" : "0");
                    //3: İM - İstanbul Merkez Bankacılığı Araştırma ve Eğitim Merkezi 1.İM - diğer ,0: Şube ,Hizmet Yılı: Banknot Matbaası (BM’ de hizmet yılı bazlı feri hesap var.)
                    break;
                case ENUMISLEMCINSI.ARSAGIRIS:                  //LDHMARGIRIS
                    m.tur = mk.gayriMenkulTurKod;
                    //tur = "İM: arsa bazlı feriler 1: İstanbul Güzel Yalı Arsası, 2:Bursa Şubesi Yeni Hizmet Binası Arsası, 3:Bursa Şubesi Yeni Lojman Binası Arsası, 4:İstanbul 4.Levent Arsası, 8:İdare Merkezi Sosyal Tesis Eğitim Arsası, 12:Gaziantep Şubesi Yeni Hizmet Binası Arsası, 13:Antalya Şubesi Arsası, 14:İzmit Şubesi Arsası, 15:İstanbul Finans Merkezi Arsası, 16:Diyarbakır Şubesi Arsası";
                    break;
                case ENUMISLEMCINSI.GAYRIMENKULTAMAMIAMORTIEDILMEMISSATIS:  //LDHMGMAMORTIDSATIS
                case ENUMISLEMCINSI.GAYRIMENKULTAMAMIAMORTIEDILMEMISIHRAC:  //LDHMGMAMORTIDIHRAC
                    m.tur = istMerkezBank ? "3" : (izmirOzdereMi ? "1" : "0");
                    //tur = 3: IM - İstanbul Merkez Bankacılığı Araştırma ve Eğitim Merkezi 1:  İM – İzmir Özdere 0: İM – diğer / Şube Hizmet Yılı : Banknot Matbaası
                    m.tura = istMerkezBank ? "3" : (izmirOzdereMi ? "1" : "0");
                    //tura = 3: İM - İstanbul Merkez Bankacılığı Araştırma ve Eğitim Merkezi  / İstanbul Sosyal Tesisi 1: İM - İzmir Özdere Dinlenme Tesisleri / Banknot Matbaası Binası 2003 ve Öncesi Girişler 0: İM – diğer / Şube / Banknot Matbaası
                    break;
                case ENUMISLEMCINSI.GAYRIMENKULTAMAMIAMORTIEDILMISSATIS:    //LDHMGMAMORTISATIS
                case ENUMISLEMCINSI.GAYRIMENKULTAMAMIAMORTIEDILMISIHRAC:    //LDHMGMAMORTIIHRAC  
                    m.tura = istMerkezBank ? "3" : (izmirOzdereMi ? "1" : "0");
                    //tura = 1; İM -  İzmir Özdere Dinlenme Tesisleri / Banknot Matbaası Binası 2003 ve Öncesi Girişler 3; İM - İstanbul Merkez Bankacılığı Araştırma ve Eğitim Merkezi 0; İM – diğer / Şube / Banknot Matbaası
                    m.turEdf = mk.gayriMenkulTurKod;
                    //tuEdf = 1: İstanbul Güzel Yalı Arsası, 2:Bursa Şubesi Yeni Hizmet Binası Arsası, 3:Bursa Şubesi Yeni Lojman Binası Arsası, 4:İstanbul 4.Levent Arsası, 8:İdare Merkezi Sosyal Tesis Eğitim Arsası, 12:Gaziantep Şubesi Yeni Hizmet Binası Arsası, 13:Antalya Şubesi Arsası, 14:İzmit Şubesi Arsası, 15:İstanbul Finans Merkezi Arsası, 16:Diyarbakır Şubesi Arsası
                    break;
                case ENUMISLEMCINSI.ARSASATIS:  //LDHMARSATIS
                    m.tur = mk.gayriMenkulTurKod;
                    //tur = 1: İstanbul Güzel Yalı Arsası, 2:Bursa Şubesi Yeni Hizmet Binası Arsası, 3:Bursa Şubesi Yeni Lojman Binası Arsası, 4:İstanbul 4.Levent Arsası, 8:İdare Merkezi Sosyal Tesis Eğitim Arsası, 12:Gaziantep Şubesi Yeni Hizmet Binası Arsası, 13:Antalya Şubesi Arsası, 14:İzmit Şubesi Arsası, 15:İstanbul Finans Merkezi Arsası, 16:Diyarbakır Şubesi Arsası
                    break;
                case ENUMISLEMCINSI.DEMIRBASAMORTISMANAYIRMA:   //LDHMDBAMORTISMAN
                case ENUMISLEMCINSI.YAZILIMAMORTISMANAYIRMA:    //LDHMYZAMORTISMAN
                    m.banknot = banknotBirimi ? "1" : "0";
                    //banknot = 1 = Banknot Matbaası 0 = Banknot Matbaası DEĞİL
                    break;
                case ENUMISLEMCINSI.GAYRIMENKULAMORTISMANAYIRMA:    //LDHMGMAMORTISMAN
                    m.banknot = banknotBirimi ? "1" : "0";
                    //banknot = 1 = Banknot Matbaası 0 = Banknot Matbaası DEĞİL
                    m.tur = istMerkezBank ? "3" : (izmirOzdereMi ? "1" : "0");
                    //tur = 1: İM için  İzmir Özdere Dinlenme Tesisleri/Banknot Matbaası Binası 2003 ve Öncesi Girişler, 3: İstanbul Merkez Bankacılığı Araştırma ve Eğitim Merkezi 0: İM Diğer / Şube
                    break;
                case ENUMISLEMCINSI.GAYRIMENKULAMORTISMANAYIRMAKKEG:
                    m.tur = istMerkezBank ? "3" : (izmirOzdereMi ? "1" : "0");
                    //tur = 1: İM için  İzmir Özdere Dinlenme Tesisleri/Banknot Matbaası Binası 2003 ve Öncesi Girişler, 3: İstanbul Merkez Bankacılığı Araştırma ve Eğitim Merkezi 0: İM Diğer / Şube
                    break;
                case ENUMISLEMCINSI.GAYRIMENKULBEDELAKTARMA:
                    m.tur = istMerkezBank ? "3" : (izmirOzdereMi ? "1" : "0");
                    //tur = 3: İM - İstanbul Merkez Bankacılığı Araştırma ve Eğitim Merkezi 1.İM - diğer , 0: Şube , Hizmet Yılı: Banknot Matbaası (BM’ de hizmet yılı bazlı feri hesap var.)
                    m.tura = istMerkezBank ? "3" : (izmirOzdereMi ? "1" : "0");
                    //tura = 1: İM -  İzmir Özdere Dinlenme Tesisleri / Banknot Matbaası Binası 2003 ve Öncesi Girişler 3: İM - İstanbul Merkez Bankacılığı Araştırma ve Eğitim Merkezi 0: İM – diğer / Şube / Banknot Matbaası
                    break;
                case ENUMISLEMCINSI.GAYRIMENKULAMORTISMANAKTARMA:
                    m.tura = mk.gayriMenkulTurKod;
                    //tura = 1: İM -  İzmir Özdere Dinlenme Tesisleri / Banknot Matbaası Binası 2003 ve Öncesi Girişler 3: İM - İstanbul Merkez Bankacılığı Araştırma ve Eğitim Merkezi 0: İM – diğer / Şube / Banknot M.
                    break;
                case ENUMISLEMCINSI.GAYRIMENKULBEDELARTIRMA:    //LDHMGMARTIR
                    m.tur = istMerkezBank ? "3" : (izmirOzdereMi ? "1" : "0");
                    m.tura = istMerkezBank ? "3" : (izmirOzdereMi ? "1" : "0");
                    break;

            }


        }

        private static void YerDoldur(ENUMISLEMCINSI islemCinsi, MuhasebeIslemi m, MuhasebeIslemiKriter mk)
        {
            m.yer = MerkezBankasiGenel.Ayarlar.MuhasebeYerKoduGetir(mk.nereyeGitti);
        }

        private static void Grupla(ArrayList mler)
        {
            for (int i = 0; i < mler.Count; i++)
            {
                MuhasebeIslemi m1 = (MuhasebeIslemi)mler[i];
                if (m1.SifirMi()) continue;

                for (int j = i + 1; j < mler.Count; j++)
                {
                    MuhasebeIslemi m2 = (MuhasebeIslemi)mler[j];

                    if (m1.Equals(m2))
                    {
                        m1.UstuneTop(m2);
                        m2.Sifirla();
                    }
                }
            }
        }

        private static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, MuhasebeIslemi form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMuhasebeBilgi to = new TABMuhasebeBilgi(ve);

            form.json = JsonOlustur(form);

            ve.BeginTransaction();

            Sonuc sonuc = to.Kaydet(form);

            if (sonuc.islemSonuc)
            {
                foreach (MuhasebeIslemiDetay detay in form.detay.objeler)
                {
                    detay.islemRefNo = form.islemRefNo;
                    detay.refKod = Guid.NewGuid().ToString();

                    sonuc = to.Kaydet(detay);
                    if (!sonuc.islemSonuc)
                        break;
                }

            }

            if (!sonuc.islemSonuc)
                ve.Rollback();
            else
            {
                sonuc.bilgiStr = "İşlem başarıyla tamamlandı.";
                ve.Commit();
            }
            ve.VeriErisimKapat();


            if (sonuc.islemSonuc)
            {
                bool serviseGonder = true;

                //Muhasebe servisine A onayından sonra gönderiliyor.
                Sonuc sonuc2 = MuhasebeServisineGonder(null, kullanan, new MuhasebeIslemi[] { form }, serviseGonder);
            }

            return sonuc;
        }

        private static Sonuc DurumDegistir(VeriErisim veriErisim, Kullanici kullanan, MuhasebeIslemi form, ENUMMuhasebeIslemiOnayDurumu yeniDurum)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABMuhasebeBilgi to = new TABMuhasebeBilgi(ve);

            ve.BeginTransaction();

            sonuc = to.DurumDegistir(form, yeniDurum);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc MuhasebeServisineGonder(VeriErisim veriErisim, Kullanici kullanan, MuhasebeIslemi[] liste, bool serviseGonder = true)
        {
            Sonuc sonuc = new Sonuc(false, "Muhasebe servisine gönderme işleminde bilinmeyen bir hata oluştu.", "");

            try
            {
                //Zamanlayı tarafından çağırılırsa
                if (kullanan.mernis == "SISTEM")
                {
                    var form = new MuhasebeIslemi();
                    form.kriter.muhasebeKod = "00001";
                    form.sorguTarih1 = new TNSDateTime(DateTime.Now.Date);
                    form.sorguTarih2 = new TNSDateTime(DateTime.Now.Date);

                    var listeSistem = TMMMuhasebeBilgi.Listele(veriErisim, kullanan, form);

                    if (listeSistem.objeler.Count > 0)
                        liste = (MuhasebeIslemi[])listeSistem.objeler.ToArray(typeof(MuhasebeIslemi));
                    else
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = "Sistem tarafından muhasebe servisine gönderilecek kayıt bulunamadı!";
                        return sonuc;
                    }
                }


                string json = "";
                foreach (MuhasebeIslemi mi in liste)
                {
                    if (json != "")
                        json += ",";
                    json += mi.json;
                }

                json = "{ \"muhasebeIslemiList\": [" + json + " ] }";

                if (serviseGonder == true)
                {
                    //Servise Gönder
                    string hata = MerkezBankasiMuhasebeServisGonder(json);
                    if (hata == "")
                    {
                        sonuc.islemSonuc = true;
                        sonuc.bilgiStr = "Muhasebeye bilgiler başarıyla gönderildi.";
                    }
                    else
                    {
                        sonuc.hataStr = hata;
                    }
                }


                if (kullanan.mernis == "SISTEM")
                {
                    sonuc.islemSonuc = true;
                    System.Threading.Thread.Sleep(30 * 1000); //30 saniye
                }
                else
                {
                    System.Threading.Thread.Sleep(3 * 1000); //3 saniye
                }


                if (sonuc.islemSonuc)
                {
                    foreach (MuhasebeIslemi mi in liste)
                    {
                        //Muhasebe durum sorgula
                        //Muhasebeye gönderildiğinen emin olmak için bir sorgu gönderiyoruz. Durum bilgisi gelirse ilk olarak gönderildi yapıyoruz.
                        ENUMMuhasebeIslemiOnayDurumu durum = MerkezBankasiMuhasebeServisDurumSorgula(mi.islemRefNo);
                        if (durum != ENUMMuhasebeIslemiOnayDurumu.TANIMSIZ && durum != ENUMMuhasebeIslemiOnayDurumu.ISLEMBULUNAMADI)
                        {
                            sonuc = DurumDegistir(null, kullanan, mi, ENUMMuhasebeIslemiOnayDurumu.GONDERILDI);
                            if (!sonuc.islemSonuc)
                                break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ex.Message;
            }

            return sonuc;
        }

        public static Sonuc DurumSorgulaveGuncelle(VeriErisim veriErisim, Kullanici kullanan, MuhasebeIslemi[] liste)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            //if (liste == null || liste.Length == 0)
            //    liste = ListeleDurumuIslenmeyenler(ve, kullanan);


            //Zamanlayı tarafından çağırılırsa
            if (kullanan.mernis == "SISTEM")
            {
                var form = new MuhasebeIslemi();
                form.sorguTarih1 = new TNSDateTime(DateTime.Now.Date);
                form.sorguTarih2 = new TNSDateTime(DateTime.Now.Date);

                var listeSistem = TMMMuhasebeBilgi.Listele(veriErisim, kullanan, form);

                if (listeSistem.objeler.Count > 0)
                    liste = (MuhasebeIslemi[])listeSistem.objeler.ToArray(typeof(MuhasebeIslemi));
            }


            if (liste != null)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

                foreach (MuhasebeIslemi mi in liste)
                {
                    if (mi.durum != (int)ENUMMuhasebeIslemiOnayDurumu.ISLENDI)
                    {
                        ENUMMuhasebeIslemiOnayDurumu durum = MerkezBankasiMuhasebeServisDurumSorgula(mi.islemRefNo);
                        if (durum != ENUMMuhasebeIslemiOnayDurumu.TANIMSIZ && durum != ENUMMuhasebeIslemiOnayDurumu.ISLEMBULUNAMADI)
                        {
                            sonuc = DurumDegistir(ve, kullanan, mi, durum);
                            if (!sonuc.islemSonuc)
                                break;
                        }
                    }
                }

                ve.VeriErisimKapat();
            }

            return sonuc;
        }

        private static MuhasebeIslemi[] ListeleDurumuIslenmeyenler(VeriErisim veriErisim, Kullanici kullanan)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABMuhasebeBilgi to = new TABMuhasebeBilgi(ve);

            IDataReader dr = to.ListeleDurumuIslenmeyenReferanslar();

            if (dr != null)
            {
                while (dr.Read())
                {
                    MuhasebeIslemi mi = new MuhasebeIslemi();
                    mi.islemRefNo = VeriErisim.DegerAl(dr, (int)TABMuhasebeBilgi.ENUMAlanlar.ISLEMREFNO, "");

                    liste.Ekle(mi);
                }
                dr.Close();
            }
            else
                liste.sonuc.DegerAta(ve.hata);

            ve.VeriErisimKapat();

            if (!liste.sonuc.islemSonuc)
                HataDosyaYaz("ListeleDurumuIslenmeyenler", liste.sonuc.hataStr);

            if (liste.objeler.Count > 0)
                return (MuhasebeIslemi[])liste.objeler.ToArray(typeof(MuhasebeIslemi));
            return null;
        }

        private static Sonuc IslemRefNoAl(VeriErisim veriErisim, Kullanici kullanan, int yil, string harcamaKod, ENUMMuhasebeIslemTur muhasebeIslemTur)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string islemRefNo = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABGenel tabGenel = new TABGenel(ve);

            ve.BeginTransaction();

            string sayacKod = "DMRBS";
            //if (muhasebeIslemTur == ENUMMuhasebeIslemTur.AMORTISMAN)
            //    sayacKod += "A";
            //else if (muhasebeIslemTur == ENUMMuhasebeIslemTur.DEGERARTIS)
            //    sayacKod += "D";

            sayacKod += yil + harcamaKod.Replace(".", "");

            islemRefNo = sayacKod + tabGenel.BelgeNoAl(sayacKod, "0000000000");

            if (islemRefNo == "")
            {
                ve.Rollback();
                sonuc.islemSonuc = false;
                sonuc.hataStr = "islemRefNo alma sırasında bir hata oluştu.";

                HataDosyaYaz("MerkezBankasiMuhasebeServisGonder", sonuc.hataStr);
            }
            else
            {
                ve.Commit();
                sonuc.anahtar = islemRefNo;
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = "";
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        private static string IslemCinsiAdGetir(int islemCinsiKod)
        {
            switch (islemCinsiKod)
            {
                case (int)ENUMISLEMCINSI.DEMIRBASGIRIS: return "LDHMDBGIRIS";
                case (int)ENUMISLEMCINSI.YAZILIMGIRIS: return "LDHMYZGIRIS";
                case (int)ENUMISLEMCINSI.GAYRIMENKULGIRIS: return "LDHMGMGIRIS";
                case (int)ENUMISLEMCINSI.ARSAGIRIS: return "LDHMARGIRIS";
                case (int)ENUMISLEMCINSI.IZBEDELIGIRIS: return "LDHMIZGIRIS";

                case (int)ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMEMISSATIS: return "LDHMDBAMORTIDSATIS";
                case (int)ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMISSATIS: return "LDHMDBAMORTISATIS";
                case (int)ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMEMISIHRAC: return "LDHMDBAMORTIDIHRAC";
                case (int)ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMISIHRAC: return "LDHMDBAMORTIIHRAC";
                case (int)ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMEMISSATIS: return "LDHMYZAMORTIDSATIS";
                case (int)ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMISSATIS: return "LDHMYZAMORTISATIS";
                case (int)ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMEMISIHRAC: return "LDHMYZAMORTIDIHRAC";
                case (int)ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMISIHRAC: return "LDHMYZAMORTIIHRAC";
                case (int)ENUMISLEMCINSI.GAYRIMENKULTAMAMIAMORTIEDILMEMISSATIS: return "LDHMGMAMORTIDSATIS";
                case (int)ENUMISLEMCINSI.GAYRIMENKULTAMAMIAMORTIEDILMISSATIS: return "LDHMGMAMORTISATIS";
                case (int)ENUMISLEMCINSI.GAYRIMENKULTAMAMIAMORTIEDILMEMISIHRAC: return "LDHMGMAMORTIDIHRAC";
                case (int)ENUMISLEMCINSI.GAYRIMENKULTAMAMIAMORTIEDILMISIHRAC: return "LDHMGMAMORTIIHRAC";
                case (int)ENUMISLEMCINSI.ARSASATIS: return "LDHMARSATIS";
                case (int)ENUMISLEMCINSI.DEMIRBASAMORTISMANAYIRMA: return "LDHMDBAMORTISMAN";
                case (int)ENUMISLEMCINSI.DEMIRBASAMORTISMANAYIRMAKKEG: return "LDHMDBAMORTISMANKKEG";
                case (int)ENUMISLEMCINSI.YAZILIMAMORTISMANAYIRMA: return "LDHMYZAMORTISMAN";
                case (int)ENUMISLEMCINSI.YAZILIMAMORTISMANAYIRMAKKEG: return "LDHMYZAMORTISMANKKEG";
                case (int)ENUMISLEMCINSI.GAYRIMENKULAMORTISMANAYIRMA: return "LDHMGMAMORTISMAN";
                case (int)ENUMISLEMCINSI.GAYRIMENKULAMORTISMANAYIRMAKKEG: return "LDHMGMAMORTISMANKKEG";
                case (int)ENUMISLEMCINSI.DEMIRBASBEDELAKTARMA: return "LDHMDBBEDELAKTAR";
                case (int)ENUMISLEMCINSI.DEMIRBASAMORTISMANAKTARMA: return "LDHMDBAMORTISMANAKTAR";
                case (int)ENUMISLEMCINSI.YAZILIMBEDELAKTARMA: return "LDHMYZBEDELAKTAR";
                case (int)ENUMISLEMCINSI.YAZILIMAMORTISMANAKTARMA: return "LDHMYZAMORTISMANAKTAR";
                case (int)ENUMISLEMCINSI.GAYRIMENKULBEDELAKTARMA: return "LDHMGMBEDELAKTAR";
                case (int)ENUMISLEMCINSI.GAYRIMENKULAMORTISMANAKTARMA: return "LDHMGMAMORTISMANAKTAR";
                case (int)ENUMISLEMCINSI.DEMIRBASBEDELARTIRMA: return "LDHMDBARTIR";
                case (int)ENUMISLEMCINSI.DEMIRBASBEDELAZALTMA: return "LDHMDBAZALT";
                case (int)ENUMISLEMCINSI.YAZILIMBEDELARTIRMA: return "LDHMYZARTIR";
                case (int)ENUMISLEMCINSI.YAZILIMBEDELAZALTMA: return "LDHMYZAZALT";
                case (int)ENUMISLEMCINSI.GAYRIMENKULBEDELARTIRMA: return "LDHMGMARTIR";
                case (int)ENUMISLEMCINSI.GAYRIMENKULBEDELAZALTMA: return "LDHMGMAZALT";

                case (int)ENUMISLEMCINSI.DEMIRBASENFLASYONDUZELTMESI: return "LDHMDBEDF";
                case (int)ENUMISLEMCINSI.YAZILIMENFLASYONDUZELTMESI: return "LDHMYZEDF";
                case (int)ENUMISLEMCINSI.GAYRIMENKULENFLASYONDUZELTMESI: return "LDHMGMEDF";

                case (int)ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMEMISIHRACHESABINAALMA: return "LDHMDBIHRACHESABINAD";
                case (int)ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMEMISIHRACHESABINDANIHRAC: return "LDHMDBIHRACHESABINDAND";
                case (int)ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMISIHRACHESABINAALMA: return "LDHMDBIHRACHESABINA";
                //case (int)ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMISIHRACHESABINDANIHRAC: return "LDHMDBIHRACHESABINDAN"; //02.09.2019 Mail ile gönderilen bilgiye göre sadece "LDHMDBIHRACHESABINDAND" olacak.

                case (int)ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMEMISIHRACHESABINAALMA: return "LDHMYZIHRACHESABINAD";
                case (int)ENUMISLEMCINSI.YAZILIMIHRACHESABINDANIHRAC: return "LDHMYZIHRACHESABINDAN";
                case (int)ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMISIHRACHESABINAALMA: return "LDHMYZIHRACHESABINA";

                case (int)ENUMISLEMCINSI.TASITAMORTISMAN: return "LDHMTASITAMORTISMAN";

                case (int)ENUMISLEMCINSI.DEMIRBASIZBEDELIGIRIS: return "LDHMDBIZBEDELIYLEGIRIS";
                case (int)ENUMISLEMCINSI.YAZILIMIZBEDELIGIRIS: return "LDHMYZIZBEDELIYLEGIRIS";
                case (int)ENUMISLEMCINSI.DEMIRBASIZBEDELIAMORTISMAN: return "LDHMDBIZBEDELIYLEAMORT";
                case (int)ENUMISLEMCINSI.YAZILIMIZBEDELIAMORTISMAN: return "LDHMYZIZBEDELIYLEAMORT";

                case (int)ENUMISLEMCINSI.LDHMSANATESERIGIRIS: return "LDHMSANATESERIGIRIS";
                case (int)ENUMISLEMCINSI.LDHMSNARTIR: return "LDHMSNARTIR";
                case (int)ENUMISLEMCINSI.LDHMSNAZALT: return "LDHMSNAZALT";
                case (int)ENUMISLEMCINSI.LDHMSNSATISAMORTISMANLI: return "LDHMSNSATISAMORTISMANLI";
                case (int)ENUMISLEMCINSI.LDHMSNSATISAMORTISMANSIZ: return "LDHMSNSATISAMORTISMANSIZ";
                case (int)ENUMISLEMCINSI.LDHMSNIHRACAMORTISMANLI: return "LDHMSNIHRACAMORTISMANLI";
                case (int)ENUMISLEMCINSI.LDHMSNIHRACAMORTISMANSIZ: return "LDHMSNIHRACAMORTISMANSIZ";

                case (int)ENUMISLEMCINSI.LDHMDBGECICIAMORTI: return "LDHMDBGECICIAMORTI";
                case (int)ENUMISLEMCINSI.LDHMYZGECICIAMORTI: return "LDHMYZGECICIAMORTI";

                //DEMİRBAŞ ENFLASYON DÜZELTMESİ
                case (int)ENUMISLEMCINSI.LDHMDBENFDUZELT: return "LDHMDBENFDUZELT";
                //DEMİRBAŞ AMORTİSMAN ENFLASYON DÜZELTMESİ
                case (int)ENUMISLEMCINSI.LDHMDBAMORTIENFDUZELT: return "LDHMDBAMORTIENFDUZELT";
                //DEMİRBAŞ ENFLASYON DÜZELTMESİ KARI/ZARARI
                case (int)ENUMISLEMCINSI.LDHMDBENFDUZELTKZ: return "LDHMDBENFDUZELTKZ";

                //YAZILIM ENFLASYON DÜZELTMESİ
                case (int)ENUMISLEMCINSI.LDHMYAZENFDUZELT: return "LDHMYAZENFDUZELT";
                //YAZILIM AMORTİSMAN ENFLASYON DÜZELTMESİ
                case (int)ENUMISLEMCINSI.LDHMYAZAMORTIENFDUZELT: return "LDHMYAZAMORTIENFDUZELT";
                //YAZILIM ENFLASYON DÜZELTMESİ KARI/ZARARI
                case (int)ENUMISLEMCINSI.LDHMYAZENFDUZELTKZ: return "LDHMYAZENFDUZELTKZ";

                //SANAT ESERLERİ ENFLASYON DÜZELTMESİ
                case (int)ENUMISLEMCINSI.LDHMSEENFDUZELT: return "LDHMSEENFDUZELT";
                //SANAT ESERLERİ AMORTİSMAN ENFLASYON DÜZELTMESİ
                case (int)ENUMISLEMCINSI.LDHMSEAMORTIENFDUZELT: return "LDHMSEAMORTIENFDUZELT";
                //SANAT ESERLERİ ENFLASYON DÜZELTMESİ KARI/ZARARI
                case (int)ENUMISLEMCINSI.LDHMSEENFDUZELTKZ: return "LDHMSEENFDUZELTKZ";

                //BİNALAR ENFLASYON DÜZELTMESİ
                case (int)ENUMISLEMCINSI.LDHMBINAENFDUZELT: return "LDHMBINAENFDUZELT";
                //BİNALAR AMORTİSMAN ENFLASYON DÜZELTMESİ
                case (int)ENUMISLEMCINSI.LDHMBINAAMORTIENFDUZELT: return "LDHMBINAAMORTIENFDUZELT";
                //BİNALAR ENFLASYON DÜZELTMESİ KARI/ZARARI
                case (int)ENUMISLEMCINSI.LDHMBINAENFDUZELTKZ: return "LDHMBINAENFDUZELTKZ";

                //ARSALAR ENFLASYON DÜZELTMESİ
                case (int)ENUMISLEMCINSI.LDHMARSAENFDUZELT: return "LDHMARSAENFDUZELT";
                //ARSALAR ENFLASYON DÜZELTMESİ KARI
                case (int)ENUMISLEMCINSI.LDHMARSAENFDUZELTK: return "LDHMARSAENFDUZELTK";

                //TAŞIT ENFLASYON DÜZELTME KKEG ZARAR
                case (int)ENUMISLEMCINSI.LDHMTASITAMORTISMANKKEGZR: return "LDHMTASITAMORTISMANKKEGZR";


            }
            return "TANIMSIZ";
        }

        private static string JsonOlustur(MuhasebeIslemi form)
        {
            MuhasebeIslemiIc mic = new MuhasebeIslemiIc
            {
                islemRefNo = form.islemRefNo,
                islemCinsi = form.islemCinsi,
                valor = form.islemTarih.Oku().ToString("dd'/'MM'/'yyy"),
                servis = form.servis,
                giris = new SicilZaman() { zaman = TarihFormatDegistir(form.kriter.girisZaman.Oku()), sicil = form.kriter.girisSicil }, // "2018-01-03T00:00:00Z"
                bOnay = new SicilZaman() { zaman = TarihFormatDegistir(form.kriter.bOnayZaman.Oku()), sicil = form.kriter.bOnaySicil },
                aOnay = new SicilZaman() { zaman = TarihFormatDegistir(form.kriter.aOnayZaman.Oku()), sicil = form.kriter.aOnaySicil }
            };

            ArrayList pList = new ArrayList();

            switch (form.islemCinsiKod)
            {
                case (int)ENUMISLEMCINSI.DEMIRBASGIRIS:             //LDHMDBGIRIS
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    break;
                case (int)ENUMISLEMCINSI.YAZILIMGIRIS:              //"LDHMYZGIRIS";
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    break;
                case (int)ENUMISLEMCINSI.GAYRIMENKULGIRIS:          //"LDHMGMGIRIS"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    pList.Add(new ParametreList() { ad = "TUR", deger = TurDuzenle(form.tur) });
                    break;
                case (int)ENUMISLEMCINSI.ARSAGIRIS:                 //"LDHMARGIRIS"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    pList.Add(new ParametreList() { ad = "TUR", deger = TurDuzenle(form.tur) });
                    break;
                case (int)ENUMISLEMCINSI.IZBEDELIGIRIS: //"LDHMIZGIRIS"; 
                    //pList.Add(new ParametreList() { ad = "TUR", deger = TurDuzenle(form.tur) });
                    pList.Add(new ParametreList() { ad = "ADET", deger = form.kiymetBedeli / (decimal)0.01 });
                    break;
                case (int)ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMEMISSATIS: //"LDHMDBAMORTIDSATIS"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "SATISBEDELI", deger = FiyatDuzenle(form.satisBedeli) });
                    //pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.ilkBedeli) });
                    pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) }); //16.08.2024 tarihli mail talebi üzerinde değiştirilidi. "hatalı muhasebeleşme (bedel düzeltmesinden kaynaklı)"
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    break;
                case (int)ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMISSATIS: //"LDHMDBAMORTISATIS"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "SATISBEDELI", deger = FiyatDuzenle(form.satisBedeli) });
                    //pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.ilkBedeli) });
                    pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) }); //16.08.2024 tarihli mail talebi üzerinde değiştirilidi. "hatalı muhasebeleşme (bedel düzeltmesinden kaynaklı)"
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    break;
                case (int)ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMEMISIHRAC: //"LDHMDBAMORTIDIHRAC"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    //pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.ilkBedeli) });
                    pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) }); //28.09.2023 tarihli mail talebi üzerinde değiştirilid. "hatalı muhasebeleşme (bedel düzeltmesinden kaynaklı)"
                    break;
                case (int)ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMISIHRAC:  //"LDHMDBAMORTIIHRAC"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    //pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.ilkBedeli) }); //Talep üzerine ilk bedel değeri gönderiliyor. 2024.08.14 tarihli mail bilgisine göre burası kapatıldı.
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    break;
                case (int)ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMEMISSATIS: //"LDHMYZAMORTIDSATIS"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "SATISBEDELI", deger = FiyatDuzenle(form.satisBedeli) });
                    //pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.ilkBedeli) });
                    pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) }); //21.08.2024 tarihli mail talebi üzerinde değiştirilidi. "hatalı muhasebeleşme (bedel düzeltmesinden kaynaklı)"
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    break;
                case (int)ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMISSATIS:   //"LDHMYZAMORTISATIS"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "SATISBEDELI", deger = FiyatDuzenle(form.satisBedeli) });
                    //pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.ilkBedeli) });
                    pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) }); //21.08.2024 tarihli mail talebi üzerinde değiştirilidi. "hatalı muhasebeleşme (bedel düzeltmesinden kaynaklı)"
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    break;
                case (int)ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMEMISIHRAC: //"LDHMYZAMORTIDIHRAC"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    //pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.ilkBedeli) });
                    pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) }); //21.08.2024 tarihli mail talebi üzerinde değiştirilid. "hatalı muhasebeleşme (bedel düzeltmesinden kaynaklı)"
                    break;
                case (int)ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMISIHRAC:  //"LDHMYZAMORTIIHRAC"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    //pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.ilkBedeli) }); //Talep üzerine ilk bedel değeri gönderiliyor. 2024.08.21 tarihli mail bilgisine göre burası kapatıldı.
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    break;
                case (int)ENUMISLEMCINSI.GAYRIMENKULTAMAMIAMORTIEDILMEMISSATIS: //"LDHMGMAMORTIDSATIS"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "SATISBEDELI", deger = FiyatDuzenle(form.satisBedeli) });
                    pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.ilkBedeli) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    pList.Add(new ParametreList() { ad = "TUR", deger = TurDuzenle(form.tur) });
                    pList.Add(new ParametreList() { ad = "TURA", deger = TurDuzenle(form.tura) });
                    break;
                case (int)ENUMISLEMCINSI.GAYRIMENKULTAMAMIAMORTIEDILMISSATIS:   //"LDHMGMAMORTISATIS"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "SATISBEDELI", deger = FiyatDuzenle(form.satisBedeli) });
                    pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.ilkBedeli) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    pList.Add(new ParametreList() { ad = "TURA", deger = TurDuzenle(form.tura) });
                    pList.Add(new ParametreList() { ad = "TUREDF", deger = TurDuzenle(form.turEdf) });
                    break;
                case (int)ENUMISLEMCINSI.GAYRIMENKULTAMAMIAMORTIEDILMEMISIHRAC: //"LDHMGMAMORTIDIHRAC"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.ilkBedeli) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    pList.Add(new ParametreList() { ad = "TUR", deger = TurDuzenle(form.tur) });
                    pList.Add(new ParametreList() { ad = "TURA", deger = TurDuzenle(form.tura) });
                    break;
                case (int)ENUMISLEMCINSI.GAYRIMENKULTAMAMIAMORTIEDILMISIHRAC:   //"LDHMGMAMORTIIHRAC"; 
                    //pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.ilkBedeli) }); //Talep üzerine ilk bedel değeri gönderiliyor.
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    pList.Add(new ParametreList() { ad = "TURA", deger = TurDuzenle(form.tura) });
                    pList.Add(new ParametreList() { ad = "TUREDF", deger = TurDuzenle(form.turEdf) });
                    break;
                case (int)ENUMISLEMCINSI.ARSASATIS: //"LDHMARSATIS"; 
                    pList.Add(new ParametreList() { ad = "SATISBEDELI", deger = FiyatDuzenle(form.satisBedeli) });
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "TUR", deger = TurDuzenle(form.tur) });
                    break;
                case (int)ENUMISLEMCINSI.DEMIRBASAMORTISMANAYIRMA: //"LDHMDBAMORTISMAN"; 
                    //pList.Add(new ParametreList() { ad = "AMORTISMANTUTARI", deger = FiyatDuzenle(form.amortismanTutari) });
                    pList.Add(new ParametreList() { ad = "EDFSIZAMORTISMANTUT", deger = FiyatDuzenle(form.amortismanTutari) });
                    pList.Add(new ParametreList() { ad = "EDFLIAMORTISMANTUT", deger = FiyatDuzenle(form.amortismanTutariEdf) });
                    pList.Add(new ParametreList() { ad = "BANKNOT", deger = TurDuzenle(form.banknot) });
                    break;
                case (int)ENUMISLEMCINSI.DEMIRBASAMORTISMANAYIRMAKKEG: //"LDHMDBAMORTISMANKKEG"; 
                    //pList.Add(new ParametreList() { ad = "AMORTISMANTUTARI", deger = FiyatDuzenle(form.amortismanTutari) });
                    pList.Add(new ParametreList() { ad = "EDFSIZAMORTISMANTUT", deger = FiyatDuzenle(form.amortismanTutari) });
                    pList.Add(new ParametreList() { ad = "EDFLIAMORTISMANTUT", deger = FiyatDuzenle(form.amortismanTutariEdf) });
                    pList.Add(new ParametreList() { ad = "YER", deger = TurDuzenle(form.yer) });
                    break;
                case (int)ENUMISLEMCINSI.YAZILIMAMORTISMANAYIRMA: //"LDHMYZAMORTISMAN"; 
                    //pList.Add(new ParametreList() { ad = "AMORTISMANTUTARI", deger = FiyatDuzenle(form.amortismanTutari) });
                    pList.Add(new ParametreList() { ad = "EDFSIZAMORTISMANTUT", deger = FiyatDuzenle(form.amortismanTutari) });
                    pList.Add(new ParametreList() { ad = "EDFLIAMORTISMANTUT", deger = FiyatDuzenle(form.amortismanTutariEdf) });
                    pList.Add(new ParametreList() { ad = "BANKNOT", deger = TurDuzenle(form.banknot) });
                    break;
                case (int)ENUMISLEMCINSI.YAZILIMAMORTISMANAYIRMAKKEG: //"LDHMYZAMORTISMANKKEG"; 
                    //pList.Add(new ParametreList() { ad = "AMORTISMANTUTARI", deger = FiyatDuzenle(form.amortismanTutari) });
                    pList.Add(new ParametreList() { ad = "EDFSIZAMORTISMANTUT", deger = FiyatDuzenle(form.amortismanTutari) });
                    pList.Add(new ParametreList() { ad = "EDFLIAMORTISMANTUT", deger = FiyatDuzenle(form.amortismanTutariEdf) });
                    pList.Add(new ParametreList() { ad = "YER", deger = TurDuzenle(form.yer) });
                    break;
                case (int)ENUMISLEMCINSI.GAYRIMENKULAMORTISMANAYIRMA: //"LDHMGMAMORTISMAN"; 
                    //pList.Add(new ParametreList() { ad = "AMORTISMANTUTARI", deger = FiyatDuzenle(form.amortismanTutari) });
                    pList.Add(new ParametreList() { ad = "EDFSIZAMORTISMANTUT", deger = FiyatDuzenle(form.amortismanTutari) });
                    pList.Add(new ParametreList() { ad = "EDFLIAMORTISMANTUT", deger = FiyatDuzenle(form.amortismanTutariEdf) });
                    pList.Add(new ParametreList() { ad = "BANKNOT", deger = TurDuzenle(form.banknot) });
                    pList.Add(new ParametreList() { ad = "TUR", deger = TurDuzenle(form.tur) });
                    break;
                case (int)ENUMISLEMCINSI.GAYRIMENKULAMORTISMANAYIRMAKKEG: //"LDHMGMAMORTISMANKKEG"; 
                    //pList.Add(new ParametreList() { ad = "AMORTISMANTUTARI", deger = FiyatDuzenle(form.amortismanTutari) });
                    pList.Add(new ParametreList() { ad = "EDFSIZAMORTISMANTUT", deger = FiyatDuzenle(form.amortismanTutari) });
                    pList.Add(new ParametreList() { ad = "EDFLIAMORTISMANTUT", deger = FiyatDuzenle(form.amortismanTutariEdf) });
                    pList.Add(new ParametreList() { ad = "TUR", deger = TurDuzenle(form.tur) });
                    pList.Add(new ParametreList() { ad = "YER", deger = TurDuzenle(form.yer) });
                    break;
                case (int)ENUMISLEMCINSI.DEMIRBASBEDELAKTARMA: //"LDHMDBBEDELAKTAR"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    break;
                case (int)ENUMISLEMCINSI.DEMIRBASAMORTISMANAKTARMA: //"LDHMDBAMORTISMANAKTAR"; 
                    pList.Add(new ParametreList() { ad = "AMORTISMANBEDELI", deger = FiyatDuzenle(form.amortismanBedeli) });
                    pList.Add(new ParametreList() { ad = "AMORTISMANBEDELIEDF", deger = FiyatDuzenle(form.amortismanBedeliEdf) });
                    break;
                case (int)ENUMISLEMCINSI.YAZILIMBEDELAKTARMA: //"LDHMYZBEDELAKTAR"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    break;
                case (int)ENUMISLEMCINSI.YAZILIMAMORTISMANAKTARMA: //"LDHMYZAMORTISMANAKTAR"; 
                    pList.Add(new ParametreList() { ad = "AMORTISMANBEDELI", deger = FiyatDuzenle(form.amortismanBedeli) });
                    pList.Add(new ParametreList() { ad = "AMORTISMANBEDELIEDF", deger = FiyatDuzenle(form.amortismanBedeliEdf) });
                    break;
                case (int)ENUMISLEMCINSI.GAYRIMENKULBEDELAKTARMA: //"LDHMGMBEDELAKTAR"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "TURA", deger = TurDuzenle(form.tura) });
                    pList.Add(new ParametreList() { ad = "TUR", deger = TurDuzenle(form.tur) });
                    break;
                case (int)ENUMISLEMCINSI.GAYRIMENKULAMORTISMANAKTARMA: //"LDHMGMAMORTISMANAKTAR"; 
                    pList.Add(new ParametreList() { ad = "AMORTISMANBEDELI", deger = FiyatDuzenle(form.amortismanBedeli) });
                    pList.Add(new ParametreList() { ad = "AMORTISMANBEDELIEDF", deger = FiyatDuzenle(form.amortismanBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "TURA", deger = TurDuzenle(form.tura) });
                    break;
                case (int)ENUMISLEMCINSI.DEMIRBASBEDELARTIRMA:      //"LDHMDBARTIR";
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    break;
                case (int)ENUMISLEMCINSI.DEMIRBASBEDELAZALTMA:      //"LDHMDBAZALT";
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    break;
                case (int)ENUMISLEMCINSI.YAZILIMBEDELARTIRMA:       //"LDHMYZARTIR";
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    break;
                case (int)ENUMISLEMCINSI.YAZILIMBEDELAZALTMA: //"LDHMYZAZALT";
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    break;
                case (int)ENUMISLEMCINSI.GAYRIMENKULBEDELARTIRMA:   //"LDHMGMARTIR";
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    //pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    //pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    //pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    pList.Add(new ParametreList() { ad = "TUR", deger = TurDuzenle(form.tur) });
                    //pList.Add(new ParametreList() { ad = "TURA", deger = TurDuzenle(form.tura) });
                    break;
                case (int)ENUMISLEMCINSI.GAYRIMENKULBEDELAZALTMA: //"LDHMGMAZALT";
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = Math.Round(form.kiymetBedeliEdf, 2) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = Math.Round(form.birikmisAmortismanEdf, 2) });
                    pList.Add(new ParametreList() { ad = "TUR", deger = TurDuzenle(form.tur) });
                    break;
                case (int)ENUMISLEMCINSI.DEMIRBASENFLASYONDUZELTMESI: //"LDHMDBEDF";
                case (int)ENUMISLEMCINSI.YAZILIMENFLASYONDUZELTMESI: //"LDHMYZEDF";
                case (int)ENUMISLEMCINSI.GAYRIMENKULENFLASYONDUZELTMESI: //"LDHMGMEDF";
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "AMORTISMANEDF", deger = FiyatDuzenle(form.amortismanBedeliEdf) });
                    break;

                case (int)ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMEMISIHRACHESABINAALMA: //"LDHMDBIHRACHESABINAD"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    pList.Add(new ParametreList() { ad = "SONBEDELEDFSIZ", deger = FiyatDuzenle(form.kiymetBedeli) }); //ILKBEDELI eskisi
                    break;
                case (int)ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMEMISIHRACHESABINDANIHRAC: //"LDHMDBIHRACHESABINDAND"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    pList.Add(new ParametreList() { ad = "SONBEDELEDFSIZ", deger = FiyatDuzenle(form.kiymetBedeli) }); //ILKBEDELI
                    break;
                case (int)ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMISIHRACHESABINAALMA: //"LDHMDBIHRACHESABINA"; 
                    pList.Add(new ParametreList() { ad = "SONBEDELEDFSIZ", deger = FiyatDuzenle(form.kiymetBedeli) }); //form.kiymetBedeli eskisi
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    break;
                //case (int)ENUMISLEMCINSI.DEMIRBASTAMAMIAMORTIEDILMISIHRACHESABINDANIHRAC: //"LDHMDBIHRACHESABINDAN"; 
                //    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                //    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                //    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                //    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                //    break;

                case (int)ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMEMISIHRACHESABINAALMA: //"LDHMYZIHRACHESABINAD"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    pList.Add(new ParametreList() { ad = "SONBEDELEDFSIZ", deger = FiyatDuzenle(form.kiymetBedeli) }); //ILKBEDELI eskisi
                    break;
                case (int)ENUMISLEMCINSI.YAZILIMIHRACHESABINDANIHRAC: //"LDHMYZIHRACHESABINDAN"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    pList.Add(new ParametreList() { ad = "SONBEDELEDFSIZ", deger = FiyatDuzenle(form.kiymetBedeli) }); //ILKBEDELI
                    break;
                case (int)ENUMISLEMCINSI.YAZILIMTAMAMIAMORTIEDILMISIHRACHESABINAALMA: //"LDHMYZIHRACHESABINA"; 
                    pList.Add(new ParametreList() { ad = "SONBEDELEDFSIZ", deger = FiyatDuzenle(form.kiymetBedeli) }); //form.kiymetBedeli eskisi
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    break;
                case (int)ENUMISLEMCINSI.TASITAMORTISMAN: //"LDHMTASITAMORTISMAN"; 
                    pList.Add(new ParametreList() { ad = "AMORTISMAN1", deger = FiyatDuzenle(form.amortisman1) });
                    pList.Add(new ParametreList() { ad = "AMORTISMAN2", deger = FiyatDuzenle(form.amortisman2) });
                    pList.Add(new ParametreList() { ad = "AMORTISMANASOS", deger = FiyatDuzenle(form.amortismanSos) });
                    pList.Add(new ParametreList() { ad = "AMORTISMANOZDR", deger = FiyatDuzenle(form.amortismanOzdr) });
                    break;
                case (int)ENUMISLEMCINSI.DEMIRBASIZBEDELIGIRIS: //"LDHMDBIZBEDELIYLEGIRIS"; 
                case (int)ENUMISLEMCINSI.YAZILIMIZBEDELIGIRIS: //"LDHMYZIZBEDELIYLEGIRIS"; 
                case (int)ENUMISLEMCINSI.DEMIRBASIZBEDELIAMORTISMAN: //"LDHMDBIZBEDELIYLEAMORT"; 
                case (int)ENUMISLEMCINSI.YAZILIMIZBEDELIAMORTISMAN: //"LDHMYZIZBEDELIYLEAMORT"; 
                    pList.Add(new ParametreList() { ad = "IZBEDELI", deger = form.kiymetBedeli });
                    break;
                case (int)ENUMISLEMCINSI.LDHMSANATESERIGIRIS: //"LDHMSANATESERIGIRIS"; 
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    break;
                case (int)ENUMISLEMCINSI.LDHMSNARTIR:       //"LDHMSNARTIR";
                    pList.Add(new ParametreList() { ad = "ARTIRIMTUTARI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    break;
                case (int)ENUMISLEMCINSI.LDHMSNAZALT:       //"LDHMSNAZALT";
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) });
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    break;
                case (int)ENUMISLEMCINSI.LDHMSNSATISAMORTISMANLI:       //"LDHMSNSATISAMORTISMANLI";
                    pList.Add(new ParametreList() { ad = "SATISBEDELI", deger = FiyatDuzenle(form.satisBedeli) });
                    pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) }); //ILKBEDELI : EDF Hariç bedel isteniyor
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    break;
                case (int)ENUMISLEMCINSI.LDHMSNSATISAMORTISMANSIZ:       //"LDHMSNSATISAMORTISMANSIZ";
                    pList.Add(new ParametreList() { ad = "SATISBEDELI", deger = FiyatDuzenle(form.satisBedeli) });
                    pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) }); //ILKBEDELI : EDF Hariç son bedel isteniyor
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    break;
                case (int)ENUMISLEMCINSI.LDHMSNIHRACAMORTISMANLI:       //"LDHMSNIHRACAMORTISMANLI";
                    pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) }); //ILKBEDELI : EDF Hariç son bedel isteniyor
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISMAN", deger = FiyatDuzenle(form.birikmisAmortisman) });
                    pList.Add(new ParametreList() { ad = "BIRIKMISAMORTISEDF", deger = FiyatDuzenle(form.birikmisAmortismanEdf) });
                    break;
                case (int)ENUMISLEMCINSI.LDHMSNIHRACAMORTISMANSIZ:       //"LDHMSNIHRACAMORTISMANSIZ";
                    pList.Add(new ParametreList() { ad = "ILKBEDELI", deger = FiyatDuzenle(form.kiymetBedeli) }); //ILKBEDELI : EDF Hariç son bedel isteniyor
                    pList.Add(new ParametreList() { ad = "KIYMETBEDELIEDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    break;
                case (int)ENUMISLEMCINSI.LDHMDBGECICIAMORTI:       //"LDHMDBGECICIAMORTI";
                case (int)ENUMISLEMCINSI.LDHMYZGECICIAMORTI:       //"LDHMYZGECICIAMORTI";
                    pList.Add(new ParametreList() { ad = "AMORTISMAN", deger = FiyatDuzenle(form.amortismanTutari) });
                    break;
                case (int)ENUMISLEMCINSI.LDHMDBENFDUZELT:       //DEMİRBAŞ ENFLASYON DÜZELTMESİ
                case (int)ENUMISLEMCINSI.LDHMYAZENFDUZELT:      //YAZILIM ENFLASYON DÜZELTMESİ
                case (int)ENUMISLEMCINSI.LDHMBINAENFDUZELT:     //BİNALAR ENFLASYON DÜZELTMESİ
                case (int)ENUMISLEMCINSI.LDHMARSAENFDUZELT:     //ARSALAR ENFLASYON DÜZELTMESİ
                case (int)ENUMISLEMCINSI.LDHMARSAENFDUZELTK:    //ARSALAR ENFLASYON DÜZELTMESİ KARI
                case (int)ENUMISLEMCINSI.LDHMSEENFDUZELT:       //SANAT ESERLERİ ENFLASYON DÜZELTMESİ
                    pList.Add(new ParametreList() { ad = "KIYMETENFDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    break;
                case (int)ENUMISLEMCINSI.LDHMDBAMORTIENFDUZELT:     //DEMİRBAŞ AMORTİSMAN ENFLASYON DÜZELTMESİ
                case (int)ENUMISLEMCINSI.LDHMYAZAMORTIENFDUZELT:    //YAZILIM AMORTİSMAN ENFLASYON DÜZELTMESİ
                case (int)ENUMISLEMCINSI.LDHMBINAAMORTIENFDUZELT:   //BİNALAR AMORTİSMAN ENFLASYON DÜZELTMESİ
                case (int)ENUMISLEMCINSI.LDHMSEAMORTIENFDUZELT:     //SANAT ESERLERİ AMORTİSMAN ENFLASYON DÜZELTMESİ
                    pList.Add(new ParametreList() { ad = "AMORTENFDF", deger = FiyatDuzenle(form.amortismanBedeliEdf) });
                    break;
                case (int)ENUMISLEMCINSI.LDHMDBENFDUZELTKZ:         //DEMİRBAŞ ENFLASYON DÜZELTMESİ KARI/ZARARI
                case (int)ENUMISLEMCINSI.LDHMYAZENFDUZELTKZ:        //YAZILIM ENFLASYON DÜZELTMESİ KARI/ZARARI
                case (int)ENUMISLEMCINSI.LDHMBINAENFDUZELTKZ:       //BİNALAR ENFLASYON DÜZELTMESİ KARI/ZARARI
                case (int)ENUMISLEMCINSI.LDHMSEENFDUZELTKZ:         //SANAT ESERLERİ ENFLASYON DÜZELTMESİ KARI/ZARARI
                case (int)ENUMISLEMCINSI.LDHMTASITAMORTISMANKKEGZR: //TAŞIT ENFLASYON DÜZELTME KKEG ZARAR
                    pList.Add(new ParametreList() { ad = "KIYMETENFDF", deger = FiyatDuzenle(form.kiymetBedeliEdf) });
                    pList.Add(new ParametreList() { ad = "AMORTENFDF", deger = FiyatDuzenle(form.amortismanBedeliEdf) });
                    break;

            }
            pList.Add(new ParametreList() { ad = "ACIKLAMA", deger = AciklamaDuzenle(form.aciklama) });
            pList.Add(new ParametreList() { ad = "MUTEMMIM", deger = form.mutemmim ? "E" : "H" });


            mic.parametreList = (ParametreList[])pList.ToArray(typeof(ParametreList));

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MuhasebeIslemiIc));
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, mic);
            string json = System.Text.Encoding.UTF8.GetString(ms.ToArray());

            return json;
        }

        private static decimal FiyatDuzenle(decimal deger)
        {
            return Math.Round(Math.Abs(deger), 2);
        }

        private static decimal TurDuzenle(string deger)
        {
            return OrtakFonksiyonlar.ConvertToDecimal(deger, 0);
        }

        private static string AciklamaDuzenle(string deger)
        {
            return string.IsNullOrWhiteSpace(deger) ? "" : (deger.Length > 50 ? deger.Substring(0, 50) : deger);
        }

        private static string TarihFormatDegistir(DateTime dateTime)
        {
            //yyyy-mm-dd'T'HH:MM:ss'Z'
            return dateTime.Year.ToString("0000") + "-" + dateTime.Month.ToString("00") + "-" + dateTime.Day.ToString("00") + "T" + dateTime.Hour.ToString("00") + ":" + dateTime.Minute.ToString("00") + ":" + dateTime.Second.ToString("00") + "Z";
        }

        private static string MerkezBankasiMuhasebeServisGonder(string json)
        {
            string hata = "";
            string bilgi = "";
            try
            {
                string servisAdres = OrtakFonksiyonlar.WebConfigOku("TASMBMuhasebeServisAdresi", "");  //https://merwebt.tcmb.gov.tr/MGMMUBSWS/public/muhasebe/veri
                if (servisAdres != "")
                {
                    bilgi += "servisAdres:" + servisAdres + "\r\n";

                    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);

                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

                    HttpWebRequest rq = (HttpWebRequest)WebRequest.Create(servisAdres);
                    rq.Method = WebRequestMethods.Http.Post;
                    rq.ContentType = "application/json; charset=UTF-8";
                    rq.ContentLength = byteArray.Length;

                    using (var sw = rq.GetRequestStream())
                    {
                        sw.Write(byteArray, 0, byteArray.Length);
                        sw.Close();
                    }

                    HttpWebResponse rp = (HttpWebResponse)rq.GetResponse();
                    if (rp != null)
                    {
                        using (var streamReader = new StreamReader(rp.GetResponseStream()))
                        {
                            string result = streamReader.ReadToEnd();

                            bilgi += "result: +" + result + "\r\n";
                        }
                    }

                    bilgi += "StatusCode:" + (int)rp.StatusCode + "\r\n";
                }
                else
                    hata += "TASMBMuhasebeServisAdresi bilgisi bulunamadı!";

            }
            catch (Exception ex)
            {
                hata += bilgi;
                hata += ex.Message;
            }

            if (hata != "")
            {
                hata += bilgi;
                HataDosyaYaz("MerkezBankasiMuhasebeServisGonder", hata);
            }

            return hata;
        }

        private static ENUMMuhasebeIslemiOnayDurumu MerkezBankasiMuhasebeServisDurumSorgula(string islemRefNo)
        {
            ENUMMuhasebeIslemiOnayDurumu durum = ENUMMuhasebeIslemiOnayDurumu.TANIMSIZ;
            string hata = "";
            string bilgi = "";
            try
            {
                string servisAdres = OrtakFonksiyonlar.WebConfigOku("TASMBMuhasebeServisAdresiDurum", ""); //https://merwebt.tcmb.gov.tr/MGMMUBSWS/public/muhasebe/durum/{islemRefNo}
                if (servisAdres != "")
                {
                    servisAdres += islemRefNo;

                    bilgi += "servisAdres:" + servisAdres + "\r\n";

                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

                    HttpWebRequest rq = (HttpWebRequest)WebRequest.Create(servisAdres);
                    rq.Method = WebRequestMethods.Http.Get;
                    rq.ContentType = "application/json; charset=UTF-8";

                    HttpWebResponse rp = (HttpWebResponse)rq.GetResponse();
                    if (rp != null)
                    {
                        using (var streamReader = new StreamReader(rp.GetResponseStream()))
                        {
                            string result = streamReader.ReadToEnd();

                            if (result.IndexOf("Islendi") > -1)
                                durum = ENUMMuhasebeIslemiOnayDurumu.ISLENDI;
                            else if (result.IndexOf("Hatali") > -1)
                                durum = ENUMMuhasebeIslemiOnayDurumu.HATALI;
                            else if (result.IndexOf("Hazir") > -1)
                                durum = ENUMMuhasebeIslemiOnayDurumu.HAZIR;
                            else if (result.IndexOf("Beklemede") > -1)
                                durum = ENUMMuhasebeIslemiOnayDurumu.BEKLEMEDE;
                            else if (result.IndexOf("HenuzIslenmedi") > -1)
                                durum = ENUMMuhasebeIslemiOnayDurumu.HENUZISLENMEDI;
                            else if (result.IndexOf("IslemBulunamadi") > -1)
                                durum = ENUMMuhasebeIslemiOnayDurumu.ISLEMBULUNAMADI;
                            else if (result.IndexOf("BilinmeyenDurum") > -1)
                                durum = ENUMMuhasebeIslemiOnayDurumu.BILINMEYENDURUM;

                            bilgi += "result: +" + result + "\r\n";
                        }
                    }

                    bilgi += "StatusCode:" + (int)rp.StatusCode + "\r\n";
                }
                else
                    hata += "TASMBMuhasebeServisAdresiDurum bilgisi bulunamadı!";

            }
            catch (Exception ex)
            {
                hata += bilgi;
                hata += ex.Message;
            }

            if (hata != "")
            {
                hata += bilgi;
                HataDosyaYaz("MerkezBankasiMuhasebeServisDurumSorgula - " + islemRefNo, hata);
            }

            return durum;
        }

        public static void HataDosyaYaz(string baslik, string mesaj)
        {
            OrtakFonksiyonlar.HataDosyaYaz("MerkezBankasiMuhasebeServisi.txt", string.Format("{0} : {1} \n{2}", baslik, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), mesaj), false);
        }

        public static MerkezBankasiDemirbasHesapToplamBakiye DemirbasHesapToplamBakiyeSorgula(string ambarKod)
        {
            var sonuc = new MerkezBankasiDemirbasHesapToplamBakiye();
            string hata = "";
            string bilgi = "";
            try
            {
                string servisAdres = OrtakFonksiyonlar.WebConfigOku("TASMBDemirbasHesapToplamBakiyeSorgulaServisAdresi", ""); //https://uygulamau.tcmb.gov.tr/MGMMUBSWS/public/muhasebe/getDemirbasHesapToplamBakiye/{ambarkod}
                if (servisAdres != "")
                {
                    servisAdres += ambarKod;

                    bilgi += "servisAdres:" + servisAdres + "\r\n";

                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

                    HttpWebRequest rq = (HttpWebRequest)WebRequest.Create(servisAdres);
                    rq.Method = WebRequestMethods.Http.Get;
                    rq.ContentType = "application/json; charset=UTF-8";

                    HttpWebResponse rp = (HttpWebResponse)rq.GetResponse();
                    if (rp != null)
                    {
                        using (var streamReader = new StreamReader(rp.GetResponseStream()))
                        {
                            string result = streamReader.ReadToEnd();

                            string json = result;
                            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);
                            MemoryStream ms = new MemoryStream(byteArray);

                            var serializer = new DataContractJsonSerializer(typeof(MerkezBankasiDemirbasHesapToplamBakiye));
                            sonuc = serializer.ReadObject(ms) as MerkezBankasiDemirbasHesapToplamBakiye;

                            sonuc.durum = true;

                            bilgi += "result: +" + result + "\r\n";
                        }
                    }

                    bilgi += "StatusCode:" + (int)rp.StatusCode + "\r\n";
                }
                else
                    hata += "TASMBDemirbasHesapToplamBakiyeSorgulaServisAdresi bilgisi bulunamadı!";

            }
            catch (Exception ex)
            {
                hata += bilgi;
                hata += ex.Message;
            }

            if (hata != "")
            {
                hata += bilgi;

                sonuc.durum = false;
                sonuc.mesaj = hata;

                HataDosyaYaz("TASMBDemirbasHesapToplamBakiyeSorgulaServisAdresi - " + ambarKod, hata);
            }

            return sonuc;

            /*
                Demirbaş Bakiye Servisi
                Demirbaş bakiye servisi aşağıda bulunan hesapların ambar kodlarına göre isteğin atıldığı tarihten bir
                önceki işgününe ait bakiye toplamlarını döner. Bakiye toplamları aşağıda belirtildiği gibi “BEDEL”,
                “BEDEL_ENFLASYON_DUZELTME”, “AMORTISMAN” ve “AMORTISMAN_ENFLASYON_DUZELTME”
                gruplandırması ile verilir.
                PROD_URL:
                https://uygulama.tcmb.gov.tr/MGMMUBSWS/public/muhasebe/getDemirbasHesapToplamBakiye/{A
                MBARKOD}
                TEST_URL:
                https://uygulamau.tcmb.gov.tr/MGMMUBSWS/public/muhasebe/getDemirbasHesapToplamBakiye/{
                AMBARKOD}
                Parametre olarak AMBARKOD verilmelidir, AMBARKOD 01, 50 veya 51 olabilir.
                Örnek Request:
                https://uygulamau.tcmb.gov.tr/MGMMUBSWS/public/muhasebe/getDemirbasHesapToplamBakiye/
                51
                Örnek Response:
                {
                 "AMORTISMAN_ENFLASYON_DUZELTME": 976507.76,
                 "AMORTISMAN": 117309499.17,
                 "BEDEL_ENFLASYON_DUZELTME": -1266494.88,
                 "BEDEL": -136769981.58
                }
            */
        }

    }

    internal enum ENUMISLEMCINSI
    {
        TANIMSIZ = 0,

        //Kıymet Girişi
        DEMIRBASGIRIS = 1,
        YAZILIMGIRIS = 2,
        GAYRIMENKULGIRIS = 3,
        ARSAGIRIS = 4,
        IZBEDELIGIRIS = 5,

        //Amortisman Ayırma
        DEMIRBASAMORTISMANAYIRMA = 10,
        DEMIRBASAMORTISMANAYIRMAKKEG = 11,
        YAZILIMAMORTISMANAYIRMA = 12,
        YAZILIMAMORTISMANAYIRMAKKEG = 13,
        GAYRIMENKULAMORTISMANAYIRMA = 14,
        GAYRIMENKULAMORTISMANAYIRMAKKEG = 15,

        //Bedel Güncelleme
        DEMIRBASBEDELARTIRMA = 20,
        DEMIRBASBEDELAZALTMA = 21,
        YAZILIMBEDELARTIRMA = 22,
        YAZILIMBEDELAZALTMA = 23,
        GAYRIMENKULBEDELARTIRMA = 24,
        GAYRIMENKULBEDELAZALTMA = 25,

        //Tamamı Amorti Edilmiş Hesaplara Aktarım
        DEMIRBASBEDELAKTARMA = 30,
        DEMIRBASAMORTISMANAKTARMA = 31,
        YAZILIMBEDELAKTARMA = 32,
        YAZILIMAMORTISMANAKTARMA = 33,
        GAYRIMENKULBEDELAKTARMA = 34,
        GAYRIMENKULAMORTISMANAKTARMA = 35,

        //Sabit Kıymet İhraç –Satış
        DEMIRBASTAMAMIAMORTIEDILMEMISIHRAC = 40,
        DEMIRBASTAMAMIAMORTIEDILMISIHRAC = 41,
        YAZILIMTAMAMIAMORTIEDILMEMISIHRAC = 42,
        YAZILIMTAMAMIAMORTIEDILMISIHRAC = 43,
        GAYRIMENKULTAMAMIAMORTIEDILMEMISIHRAC = 44,
        GAYRIMENKULTAMAMIAMORTIEDILMISIHRAC = 45,
        ARSASATIS = 46,

        //Sabit Kıymet Satış
        DEMIRBASTAMAMIAMORTIEDILMEMISSATIS = 50,
        DEMIRBASTAMAMIAMORTIEDILMISSATIS = 51,
        YAZILIMTAMAMIAMORTIEDILMEMISSATIS = 52,
        YAZILIMTAMAMIAMORTIEDILMISSATIS = 53,
        GAYRIMENKULTAMAMIAMORTIEDILMEMISSATIS = 54,
        GAYRIMENKULTAMAMIAMORTIEDILMISSATIS = 55,

        //Enflasyon Düzenlemesi
        DEMIRBASENFLASYONDUZELTMESI = 60,
        YAZILIMENFLASYONDUZELTMESI = 61,
        GAYRIMENKULENFLASYONDUZELTMESI = 62,

        //Geçici İhraç - Demirbaş
        DEMIRBASTAMAMIAMORTIEDILMEMISIHRACHESABINAALMA = 70,
        DEMIRBASTAMAMIAMORTIEDILMEMISIHRACHESABINDANIHRAC = 71,
        DEMIRBASTAMAMIAMORTIEDILMISIHRACHESABINAALMA = 72,
        //DEMIRBASTAMAMIAMORTIEDILMISIHRACHESABINDANIHRAC = 73,

        //Geçici İhraç - Yazılım
        YAZILIMTAMAMIAMORTIEDILMEMISIHRACHESABINAALMA = 80,
        YAZILIMIHRACHESABINDANIHRAC = 81,
        YAZILIMTAMAMIAMORTIEDILMISIHRACHESABINAALMA = 82,


        TASITAMORTISMAN = 90,

        DEMIRBASIZBEDELIGIRIS = 91,
        YAZILIMIZBEDELIGIRIS = 92,
        DEMIRBASIZBEDELIAMORTISMAN = 93,
        YAZILIMIZBEDELIAMORTISMAN = 94,


        //Sanat Eseri Giriş
        LDHMSANATESERIGIRIS = 100,
        //Sanat Eseri BedelArtirma
        LDHMSNARTIR = 101,
        //Sanat Eseri BedelAzaltma
        LDHMSNAZALT = 102,
        //Sanat Eseri Amortismanı Olan Satış
        LDHMSNSATISAMORTISMANLI = 103,
        //Sanat Eseri Amortismanı Olmayan Satış
        LDHMSNSATISAMORTISMANSIZ = 104,
        //Sanat Eseri Amortismanı Olan İhraç
        LDHMSNIHRACAMORTISMANLI = 105,
        //Sanat Eseri Amortismanı Olmayan İhraç
        LDHMSNIHRACAMORTISMANSIZ = 106,

        //Tamamı amorti edilmemiş geçici ihraca alınan demirbaşların amorti edilmesi
        LDHMDBGECICIAMORTI = 110,
        //Tamamı amorti edilmemiş geçici ihraca alınan yazılımların amorti edilmesi
        LDHMYZGECICIAMORTI = 111,



        //DEMİRBAŞ ENFLASYON DÜZELTMESİ
        LDHMDBENFDUZELT = 200,
        //DEMİRBAŞ AMORTİSMAN ENFLASYON DÜZELTMESİ
        LDHMDBAMORTIENFDUZELT = 201,
        //DEMİRBAŞ ENFLASYON DÜZELTMESİ KARI/ZARARI
        LDHMDBENFDUZELTKZ = 202,

        //YAZILIM ENFLASYON DÜZELTMESİ
        LDHMYAZENFDUZELT = 203,
        //YAZILIM AMORTİSMAN ENFLASYON DÜZELTMESİ
        LDHMYAZAMORTIENFDUZELT = 204,
        //YAZILIM ENFLASYON DÜZELTMESİ KARI/ZARARI
        LDHMYAZENFDUZELTKZ = 205,

        //SANAT ESERLERİ ENFLASYON DÜZELTMESİ
        LDHMSEENFDUZELT = 206,
        //SANAT ESERLERİ AMORTİSMAN ENFLASYON DÜZELTMESİ
        LDHMSEAMORTIENFDUZELT = 207,
        //SANAT ESERLERİ ENFLASYON DÜZELTMESİ KARI/ZARARI
        LDHMSEENFDUZELTKZ = 208,

        //BİNALAR ENFLASYON DÜZELTMESİ
        LDHMBINAENFDUZELT = 209,
        //BİNALAR AMORTİSMAN ENFLASYON DÜZELTMESİ
        LDHMBINAAMORTIENFDUZELT = 210,
        //BİNALAR ENFLASYON DÜZELTMESİ KARI/ZARARI
        LDHMBINAENFDUZELTKZ = 211,

        //ARSALAR ENFLASYON DÜZELTMESİ
        LDHMARSAENFDUZELT = 212,
        //ARSALAR ENFLASYON DÜZELTMESİ KARI
        LDHMARSAENFDUZELTK = 213,

        //TAŞIT ENFLASYON DÜZELTME KKEG ZARAR
        LDHMTASITAMORTISMANKKEGZR = 214


    }

    [DataContract]
    internal class MuhasebeIslemiIc
    {
        [DataMember(Order = 1)]
        public string islemRefNo { get; set; }

        [DataMember(Order = 2)]
        public string islemCinsi { get; set; }

        [DataMember(Order = 3)]
        public string valor { get; set; }

        [DataMember(Order = 4)]
        public string servis { get; set; }

        [DataMember(Order = 5)]
        public SicilZaman giris { get; set; }
        [DataMember(Order = 6)]
        public SicilZaman bOnay { get; set; }
        [DataMember(Order = 7)]
        public SicilZaman aOnay { get; set; }

        [DataMember(Order = 8)]
        public ParametreList[] parametreList { get; set; }

    }

    [DataContract]
    internal class SicilZaman
    {
        [DataMember(Order = 1)]
        public string zaman { get; set; }

        [DataMember(Order = 2)]
        public string sicil { get; set; }
    }

    [DataContract]
    internal class ParametreList
    {
        [DataMember(Order = 1)]
        public string ad { get; set; }

        [DataMember(Order = 2)]
        public object deger { get; set; }
    }


    internal static class MerkezBankasiGenel
    {
        private static MerkezBankasi _ayarlar = new MerkezBankasi();

        private static bool ayarlarOkundu = false;


        //public static MerkezBankasi Ayarlar()
        //{
        //    if (!ayarlarOkundu)
        //        AyarlarOku();

        //    return _ayarlar;
        //}

        public static MerkezBankasi Ayarlar
        {
            get
            {
                AyarlarOku();

                return _ayarlar;
            }
            private set { _ayarlar = value; }
        }

        private static void AyarlarOku()
        {
            if (!MerkezBankasiGenel.ayarlarOkundu)
            {
                MerkezBankasiGenel.ayarlarOkundu = true;

                string dosyaAd = OrtakFonksiyonlar.WebConfigOku("MerkezBankasiAyarlar", "");

                try
                {
                    if (dosyaAd != null)
                    {
                        if (new FileInfo(dosyaAd).Exists)
                        {
                            string json = OrtakFonksiyonlar.DosyaOku(dosyaAd);
                            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);
                            MemoryStream ms = new MemoryStream(byteArray);

                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MerkezBankasi));
                            _ayarlar = serializer.ReadObject(ms) as MerkezBankasi;
                        }
                        else
                            VarsayilanAyarlariOlusur();
                    }
                    else
                    {
                        _ayarlar = new MerkezBankasi();
                        OrtakFonksiyonlar.HataStrYaz(string.Format("{0} dosyasi bulunamadı!", dosyaAd));
                    }
                }
                catch (Exception e)
                {
                    OrtakFonksiyonlar.HataStrYaz(string.Format("{0} dosyasinda okuma hatası olustu! * {1}", dosyaAd, e.Message));
                }
            }
        }

        private static void VarsayilanAyarlariOlusur()
        {
            string dosyaAd = System.Configuration.ConfigurationManager.AppSettings.Get("MerkezBankasiAyarlar");

            try
            {
                if (dosyaAd != null)
                {
                    //İstanbul Merkez : İE0016U10 - 1563
                    //Binek Otosu : 0750101 (Taşıt amortisman için)
                    MerkezBankasi form = new MerkezBankasi("000", "010", "1563", "09800", "09700", "094", "0750101");

                    ArrayList liste = new ArrayList();

                    for (int i = 0; i < 50; i++)
                    {
                        string birimKod = i.ToString().PadLeft(3, '0');
                        liste.Add(new MuhasebeServis() { birim = birimKod, ambar = "01", kod = "104" });
                        liste.Add(new MuhasebeServis() { birim = birimKod, ambar = "50", kod = "108" });
                        liste.Add(new MuhasebeServis() { birim = birimKod, ambar = "51", kod = "104" });
                    }

                    form.muhasebeServisListe = (MuhasebeServis[])liste.ToArray(typeof(MuhasebeServis));

                    liste = new ArrayList
                    {
                        new SosyalTesis() { kod = "1519", ad = "İzmir Özdere Kampı", muhasebeYerKod = "1", ozelYerAdi = "ozdere" },        //KİK2400S21
                        new SosyalTesis() { kod = "1069", ad = "İzmir Özdere Kampı", muhasebeYerKod = "1", ozelYerAdi = "ozdere" },        //
                        new SosyalTesis() { kod = "1063", ad = "Ankara Sosyal Tesisi", muhasebeYerKod = "2", ozelYerAdi = "ankara" },      //D040000
                        new SosyalTesis() { kod = "1518", ad = "Ankara Sosyal Tesisi", muhasebeYerKod = "2", ozelYerAdi = "ankara" },      //
                        new SosyalTesis() { kod = "1906", ad = "Fenerbançe Sosyal Tesisi", muhasebeYerKod = "0", ozelYerAdi = "" }          //2D050200000
                    };
                    form.sosyalTesisListe = (SosyalTesis[])liste.ToArray(typeof(SosyalTesis));


                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MerkezBankasi));
                    MemoryStream ms = new MemoryStream();
                    serializer.WriteObject(ms, form);

                    OrtakFonksiyonlar.DosyaYaz(dosyaAd, ms.ToArray());

                    _ayarlar = form;
                }
                else
                    OrtakFonksiyonlar.HataStrYaz(string.Format("{0} dosyasi bulunamadı!", dosyaAd));

            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataStrYaz(string.Format("{0} dosyasinda yazma hatası olustu! * {1}", dosyaAd, e.Message));
            }
        }

    }


    [DataContract]
    internal class MerkezBankasi
    {
        [DataMember(Order = 1)]
        private string idareMerkezi = "";

        [DataMember(Order = 2)]
        private string banknotBirimi = "";

        [DataMember(Order = 3)]
        private string istMerkezBankasi = "";

        [DataMember(Order = 4)]
        private string gayriMenkulBinaHesapKod = "";

        [DataMember(Order = 5)]
        private string gayriMenkulArsaHesapKod = "";

        [DataMember(Order = 6)]
        private string yazilimHesapKod = "";

        [DataMember(Order = 7)]
        private string tasitHesapKod = "";

        [DataMember(Order = 8)]
        public SosyalTesis[] sosyalTesisListe { get; set; }

        [DataMember(Order = 9)]
        public MuhasebeServis[] muhasebeServisListe { get; set; }

        public MerkezBankasi() { }
        public MerkezBankasi(string idareMerkezi, string banknotBirimi, string istMerkezBankasi, string gayriMenkulBinaHesapKod, string gayriMenkulArsaHesapKod, string yazilimHesapKod, string tasitHesapKod)
        {
            this.idareMerkezi = idareMerkezi;
            this.banknotBirimi = banknotBirimi;
            this.istMerkezBankasi = istMerkezBankasi;
            this.gayriMenkulBinaHesapKod = gayriMenkulBinaHesapKod;
            this.gayriMenkulArsaHesapKod = gayriMenkulArsaHesapKod;
            this.yazilimHesapKod = yazilimHesapKod;
            this.tasitHesapKod = tasitHesapKod;
        }

        public bool BinaMi(string hesapPlanKod)
        {
            return hesapPlanKod.StartsWith(this.gayriMenkulBinaHesapKod);
        }
        public bool ArsaMi(string hesapPlanKod)
        {
            return hesapPlanKod.StartsWith(this.gayriMenkulArsaHesapKod);
        }
        public bool YazilimMi(string hesapPlanKod)
        {
            return hesapPlanKod.StartsWith(this.yazilimHesapKod);
        }
        public bool TasitMi(string hesapPlanKod)
        {
            return hesapPlanKod.StartsWith(this.tasitHesapKod);
        }

        public bool IdareMerkeziMi(string birimKod)
        {
            return this.idareMerkezi == birimKod;
        }
        public bool BanknotBirimiMi(string birimKod)
        {
            return this.banknotBirimi == birimKod;
        }
        public bool IstMerkezBankasiMi(string yerleskeKod)
        {
            return this.istMerkezBankasi == yerleskeKod;
        }

        public bool OzdereMi(string yerleskeKod)
        {
            if (this.sosyalTesisListe != null)
                foreach (var tesis in this.sosyalTesisListe)
                {
                    if (tesis.ozelYerAdi == "ozdere" && tesis.kod == yerleskeKod)
                        return true;
                }
            return false;
        }

        public bool AnkaraSosyalMi(string yerleskeKod)
        {
            if (this.sosyalTesisListe != null)
                foreach (var tesis in this.sosyalTesisListe)
                {
                    if (tesis.ozelYerAdi == "ankara" && tesis.kod == yerleskeKod)
                        return true;
                }
            return false;
        }

        public string MuhasebeServisKodGetir(string birimKod, string ambarKod)
        {
            if (this.muhasebeServisListe == null) return "";

            foreach (var servis in this.muhasebeServisListe)
            {
                if (servis.birim == birimKod && servis.ambar == ambarKod)
                    return servis.kod;
            }
            return "";
        }

        public bool SosyalTesisVarMi(string yerleskeKod)
        {
            if (this.sosyalTesisListe != null)
                foreach (var tesis in this.sosyalTesisListe)
                {
                    if (tesis.kod == yerleskeKod)
                        return true;
                }
            return false;
        }

        public string MuhasebeYerKoduGetir(string yerleskeKod)
        {
            if (this.sosyalTesisListe != null)
                foreach (var tesis in this.sosyalTesisListe)
                    if (tesis.kod == yerleskeKod)
                        return tesis.muhasebeYerKod;
            return "0";
        }

    }

    [DataContract]
    internal class MuhasebeServis
    {
        [DataMember(Order = 1)]
        public string birim { get; set; }

        [DataMember(Order = 2)]
        public string ambar { get; set; }

        [DataMember(Order = 3)]
        public string kod { get; set; }
    }

    [DataContract]
    internal class SosyalTesis
    {
        [DataMember(Order = 1)]
        public string kod { get; set; }

        [DataMember(Order = 2)]
        public string ad { get; set; }

        [DataMember(Order = 3)]
        public string muhasebeYerKod { get; set; }

        [DataMember(Order = 4)]
        public string ozelYerAdi { get; set; }
    }


    [DataContract]
    public class MerkezBankasiDemirbasHesapToplamBakiye
    {
        [DataMember(Order = 1)]
        public decimal AMORTISMAN_ENFLASYON_DUZELTME = 0;

        [DataMember(Order = 2)]
        public decimal AMORTISMAN = 0;

        [DataMember(Order = 3)]
        public decimal BEDEL_ENFLASYON_DUZELTME = 0;

        [DataMember(Order = 4)]
        public decimal BEDEL = 0;

        public bool durum = false;
        public string mesaj = "";

        public MerkezBankasiDemirbasHesapToplamBakiye() { }
    }
}
