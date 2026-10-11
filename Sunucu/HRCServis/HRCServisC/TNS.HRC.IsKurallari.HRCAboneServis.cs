using System;
using System.Data;
using System.Collections;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.MUH;
using TNS.Veri;
using TNS.BKD;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Abone fatura bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCAboneServis
    {
        /// <summary>
        /// Oluşan hataların yazılacağı dosya adını tutan değişken
        /// </summary>
        public string hataDosyaAd = "AboneDosyaAktarimi" + DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + ".txt";

        /// <summary>
        /// Bütçe Muhasebe Modülü servisine ulaşmak için kullanılan değişken
        /// </summary>
        IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

        /// <summary>
        /// Arguman olarak verilen, abone fatura bilgisine ait süreç oluşturma işlemlerini yapar
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="aboneTur">İşlem yapılacak abone tip bilgisi.</param>
        public void SurecOlustur(Kullanici kullanan, int yil, int ay, int aboneTur)
        {
            ObjectArray listeFatura = new ObjectArray();
            if (aboneTur == (int)ENUMAboneTip.TELEFON)
            {
                AboneFaturaTelefon kriter = new AboneFaturaTelefon();
                kriter.faturaYil = yil;
                kriter.faturaAy = ay;
                listeFatura = new HRCAboneFaturaTelekom().Listele(kriter, true);
            }
            else if (aboneTur == (int)ENUMAboneTip.ELEKTRIK)
            {
                AboneFaturaElektrik kriter = new AboneFaturaElektrik();
                kriter.faturaYil = yil;
                kriter.faturaAy = ay;
                listeFatura = new HRCAboneFaturaElektrik().Listele(kriter, true);
            }
            else if (aboneTur == (int)ENUMAboneTip.SU)
            {
                AboneFaturaSu kriter = new AboneFaturaSu();
                kriter.faturaYil = yil;
                kriter.faturaAy = ay;
                listeFatura = new HRCAboneFaturaSu().Listele(kriter, true);
            }
            string eskiMuhasebeBirim = "";
            string surecNo = "";
            ObjectArray surecFatura = new ObjectArray();
            Sonuc sonuc = new Sonuc();
            int muhasebe = 0;
            string birim = "";
            double faturaTutar = 0;
            string aboneNo = "";
            string isletmeKodu = "";
            string faturaNo = "";
            decimal odemeTutar = 0;
            TNSDateTime sonOdemeTarih = new TNSDateTime();
            int kontrolMuhasebe = 0;
            string kontrolBirim = "";

            foreach (Object fNesne in listeFatura.objeler)
            {
                if (aboneTur == (int)ENUMAboneTip.TELEFON)
                {
                    AboneFaturaTelefon f = (AboneFaturaTelefon)fNesne;
                    aboneNo = f.hizmetNo;
                    isletmeKodu = "";
                    faturaNo = f.faturaNo;
                    odemeTutar = f.odnTutar;
                    sonOdemeTarih = f.sonOdemeTarih;
                    kontrolMuhasebe = f.muhasebe;
                    kontrolBirim = f.birim;
                }
                else if (aboneTur == (int)ENUMAboneTip.ELEKTRIK)
                {
                    AboneFaturaElektrik f = (AboneFaturaElektrik)fNesne;
                    aboneNo = f.hizmetNo;
                    isletmeKodu = f.isletmeKodu;
                    faturaNo = "";
                    odemeTutar = f.fatTopTutar;
                    sonOdemeTarih = f.sonOdemeTarih;
                    kontrolMuhasebe = f.muhasebe;
                    kontrolBirim = f.birim;
                }
                else if (aboneTur == (int)ENUMAboneTip.SU)
                {
                    AboneFaturaSu f = (AboneFaturaSu)fNesne;
                    aboneNo = f.hizmetNo;
                    isletmeKodu = f.isletmeKodu;
                    faturaNo = f.faturaNo;
                    odemeTutar = OrtakFonksiyonlar.ConvertToDecimal(f.fatTopTutar);
                    sonOdemeTarih = f.sonOdemeTarih;
                    kontrolMuhasebe = f.muhasebe;
                    kontrolBirim = f.birim;
                }

                if (eskiMuhasebeBirim != kontrolMuhasebe.ToString() + kontrolBirim && kontrolMuhasebe > 0)
                {
                    //Önceki sürecin faturalarını kaydet ve surecFatura değişkenini sıfırla
                    if (surecFatura.objeler.Count > 0)
                    {
                        sonuc = new HRCAboneFatura().Kaydet(null, kullanan, surecFatura);
                        if (!sonuc.islemSonuc)
                            OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - Süreç Oluştur. Muhasebe:" + muhasebe + " Birim:" + birim + " Süreç No:" + surecNo + " Hata:" + sonuc.hataStr.Replace("<b>", "").Replace("</b>", "").Replace("<br>", ""), true);
                    }
                    surecFatura = new ObjectArray();

                    TNS.HRC.Surec sr = new TNS.HRC.Surec();

                    sr.yil = DateTime.Now.Year;
                    sr.surecTarih = new TNSDateTime(DateTime.Now.ToShortDateString());
                    sr.surecNo = "";
                    if (aboneTur == (int)ENUMAboneTip.TELEFON)
                    {
                        sr.islem.kod = 20;//TELEFON ÖDEMESİ HRCIS tablosundaki kod ABONELİK GEREKTİREN ÖDEMELER (TELEFON)
                        sr.isinAdi = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00050, ay.ToString("00"), yil.ToString());
                    }
                    else if (aboneTur == (int)ENUMAboneTip.ELEKTRIK)
                    {
                        sr.islem.kod = 22;//ELEKTRİK ÖDEMESİ HRCIS tablosundaki kod ABONELİK GEREKTİREN ÖDEMELER (ELEKTRİK)
                        sr.isinAdi = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00051, ay.ToString("00"), yil.ToString());
                    }
                    else if (aboneTur == (int)ENUMAboneTip.SU)
                    {
                        sr.islem.kod = 21;//SU ÖDEMESİ HRCIS tablosundaki kod ABONELİK GEREKTİREN ÖDEMELER (SU)
                        sr.isinAdi = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00052, ay.ToString("00"), yil.ToString());
                    }
                    sr.aciklama = "";
                    sr.surecDurum = (int)ENUMSurecDurum.YENI;

                    muhasebe = kontrolMuhasebe;
                    birim = kontrolBirim;

                    sr.muhasebe = muhasebe;
                    sr.birim = birim;
                    sonuc = new HRCSurec().Kaydet(kullanan, sr, null);
                    faturaTutar = 0;

                    if (sonuc.islemSonuc)
                        surecNo = sonuc.anahtar;
                    else
                    {
                        surecNo = "";
                        OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - Süreç Oluştur. Muhasebe:" + muhasebe + " Birim:" + birim + " Hata:" + sonuc.hataStr.Replace("<b>", "").Replace("</b>", "").Replace("<br>", ""), false);
                    }
                }

                if (surecNo != "")
                {
                    TNS.HRC.AboneFatura fatura = new TNS.HRC.AboneFatura();

                    fatura.yil = DateTime.Now.Year;
                    fatura.muhasebe = muhasebe;
                    fatura.birim = birim;
                    fatura.surecNo = surecNo;
                    fatura.faturaAy = ay;
                    fatura.faturaYil = yil;
                    fatura.aboneTip = aboneTur;

                    fatura.aboneNo = aboneNo;
                    fatura.faturaNo = faturaNo;
                    fatura.faturaTutar = OrtakFonksiyonlar.ConvertToDbl(odemeTutar);
                    fatura.faizMiktari = 0;
                    fatura.ozelKullanim = 0;
                    fatura.odemeTarihi = new TNSDateTime(sonOdemeTarih.ToString());
                    fatura.odemeEmriNo = "";
                    fatura.isletmeKodu = isletmeKodu;

                    faturaTutar += OrtakFonksiyonlar.ConvertToDouble(odemeTutar);
                    surecFatura.objeler.Add(fatura);
                }

                eskiMuhasebeBirim = muhasebe.ToString() + birim;
            }
            //Önceki sürecin faturalarını kaydet
            if (surecFatura.objeler.Count > 0)
            {
                sonuc = new HRCAboneFatura().Kaydet(null, kullanan, surecFatura);
                if (!sonuc.islemSonuc)
                    OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - Süreç Oluştur. Muhasebe:" + muhasebe + " Birim:" + birim + " Süreç No:" + surecNo + " Hata:" + sonuc.hataStr.Replace("<b>", "").Replace("</b>", "").Replace("<br>", ""), false);
            }
        }

        /// <summary>
        /// Arguman olarak verilen, abone fatura bilgisine ait ödeme emri belgesi oluşturma işlemlerini yapar
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="aboneTur">İşlem yapılacak abone tip bilgisi.</param>
        public void OdemeEmriOlustur(Kullanici kullanan, int yil, int ay, int aboneTur)
        {
            TNS.HRC.AboneFatura fatura = new TNS.HRC.AboneFatura();
            fatura.aboneTip = aboneTur;
            fatura.faturaYil = yil;
            fatura.faturaAy = ay;
            fatura.odemeEmriNo = "HAZIRLANMAMIŞ";

            ObjectArray liste = new ObjectArray();
            liste = new HRCAboneFatura().Listele(kullanan, fatura, new TNSDateTime(DateTime.Now), true);

            string eskiSurec = "";
            double faturaTutar = 0;
            int muhasebe = 0;
            string birim = "";

            foreach (TNS.HRC.AboneFatura p in liste.objeler)
            {
                if (eskiSurec != p.surecNo && eskiSurec != "" && faturaTutar > 0)
                {
                    OdemeEmriOlustur(kullanan, yil, ay, faturaTutar, eskiSurec, muhasebe, birim, aboneTur);

                    faturaTutar = 0;
                }
                faturaTutar += p.faturaTutar - (p.faizMiktari + p.ozelKullanim);

                eskiSurec = p.surecNo;
                muhasebe = p.muhasebe;
                birim = p.birim;
            }
            if (eskiSurec != "" && faturaTutar > 0)
                OdemeEmriOlustur(kullanan, yil, ay, faturaTutar, eskiSurec, muhasebe, birim, aboneTur);
        }

        /// <summary>
        /// Arguman olarak verilen, abone fatura bilgisine ait ödenek gönderme belgesi oluşturma işlemlerini yapar
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="aboneTur">İşlem yapılacak abone tip bilgisi.</param>
        public void OdenekGonderme(Kullanici kullanan, int yil, int ay, int aboneTur)
        {
            Sonuc sonuc = new Sonuc();
            int muhasebe = 0;
            string birim = "";
            string ekoKod4 = "";
            string ekoKod2 = "";

            if (aboneTur == (int)ENUMAboneTip.TELEFON)
            {
                ekoKod4 = "03.5.2.02";
                ekoKod2 = "03.5";
            }
            else if (aboneTur == (int)ENUMAboneTip.ELEKTRIK)
            {
                ekoKod4 = "03.2.3.03";
                ekoKod2 = "03.2";
            }
            else if (aboneTur == (int)ENUMAboneTip.SU)
            {
                ekoKod4 = "03.2.2.01";
                ekoKod2 = "03.2";
            }

            if (TNS.MUH.Arac.OdenekGondermeTertipYapi(yil) != ENUMOdenekGondermeTur.eko2)
                ekoKod2 = ekoKod4;

            //MuhasebeBirim listesi
            ObjectArray muhasebeBirimListe = new HRCButce().MuhasebeBirimListele(muhasebe, birim, "");
            //ButceHarcama
            ObjectArray harcamaListe = new HRCButce().HarcamaListele(DateTime.Now.Year, ay, muhasebe, birim, "", "", "", ekoKod4);
            //ButceGider
            ObjectArray odenekListe = new HRCButce().OdenekGondermeListele(DateTime.Now.Year, muhasebe, birim, "", "", "", ekoKod2, 0);
            //MuhasebeBirimTertip
            ObjectArray tertipListe = new HRCButce().MuhasebeBirimTertipListele(muhasebe, birim, ekoKod4);
            //Abone Fatura
            TNS.HRC.AboneFatura aboneFatura = new TNS.HRC.AboneFatura();
            aboneFatura.faturaAy = ay;
            aboneFatura.faturaYil = yil;
            aboneFatura.muhasebe = muhasebe;
            aboneFatura.birim = birim;
            aboneFatura.aboneTip = aboneTur;
            ObjectArray aboneListe = new HRCAboneFatura().Listele(kullanan, aboneFatura, new TNSDateTime(DateTime.Now), true);

            Hashtable htHarcama = HarcamaListe(harcamaListe);
            Hashtable htOdenek = OdenekListe(odenekListe);
            Hashtable htFaturaToplam = FaturaToplamListe(aboneListe);
            Hashtable htTertip = TertipListe(tertipListe);

            string odenekAciklama = "";

            if (aboneTur == (int)ENUMAboneTip.TELEFON)
                odenekAciklama = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00053, ay.ToString("00"), yil);
            if (aboneTur == (int)ENUMAboneTip.ELEKTRIK)
                odenekAciklama = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00054, ay.ToString("00"), yil);
            if (aboneTur == (int)ENUMAboneTip.SU)
                odenekAciklama = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00055, ay.ToString("00"), yil);

            if (aboneListe.ObjeSayisi > 0)
            {
                foreach (TNS.HRC.MuhasebeBirim muhBirim in muhasebeBirimListe.objeler)
                {
                    muhasebe = muhBirim.muhasebeKod;
                    birim = muhBirim.birimKod;
                    string key = muhasebe + "-" + birim;
                    double faturaToplam = OrtakFonksiyonlar.ConvertToDbl(htFaturaToplam[key]);
                    double odenek = OrtakFonksiyonlar.ConvertToDbl(htOdenek[key]);
                    double harcama = OrtakFonksiyonlar.ConvertToDbl(htHarcama[key]);
                    double ihtiyac = Math.Round(odenek - harcama - faturaToplam, 2);
                    string tertip = OrtakFonksiyonlar.ConvertToStr(htTertip[key]);

                    if (ihtiyac < 0 && faturaToplam > 0)
                    {
                        ihtiyac = Math.Abs(ihtiyac);
                        ihtiyac = Math.Ceiling(ihtiyac);//Yukarı yuvarla

                        if (tertip == "")
                            OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - Ödenek Gönderme Oluştur. Muhasebe:" + muhasebe + "-" + birim + " biriminde kullanılacak tertip bilgisi bulunamadı.", false);
                        else
                            sonuc = OdenekGondermeHazirla(kullanan, DateTime.Now.Year, muhasebe, birim, ihtiyac, odenekAciklama, tertip);

                        if (!sonuc.islemSonuc)
                            OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - Ödenek Gönderme Oluştur. Muhasebe:" + muhasebe + "-" + birim + " / " + sonuc.hataStr, false);
                    }
                }
            }
        }

        /// <summary>
        /// Arguman olarak verilen, abone fatura bilgisine ait mail gönderme işlemlerini yapar
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="aboneTur">İşlem yapılacak abone tip bilgisi.</param>
        public void MailGonder(Kullanici kullanan, int yil, int ay, int aboneTur)
        {
            if (aboneTur == (int)ENUMAboneTip.TELEFON)
            {
                AboneFaturaTelefon fatura = new AboneFaturaTelefon();
                fatura.faturaYil = yil;
                fatura.faturaAy = ay;
                new HRCAboneFaturaTelekom().MailGonder(kullanan, fatura);
            }
            else if (aboneTur == (int)ENUMAboneTip.ELEKTRIK)
            {
                AboneFaturaElektrik fatura = new AboneFaturaElektrik();
                fatura.faturaYil = yil;
                fatura.faturaAy = ay;
                new HRCAboneFaturaElektrik().MailGonder(kullanan, fatura);
            }
            else if (aboneTur == (int)ENUMAboneTip.SU)
            {
                AboneFaturaSu fatura = new AboneFaturaSu();
                fatura.faturaYil = yil;
                fatura.faturaAy = ay;
                new HRCAboneFaturaSu().MailGonder(kullanan, fatura);
            }
        }

        private Sonuc OdenekGondermeHazirla(Kullanici kullanan, int yil, int muhasebe, string birim, double ihtiyac, string odenekAciklama, string tertip)
        {
            throw new Exception("Bu metod kullanılmıyor. (Abone Servis)");

            //Sonuc sonuc = new Sonuc();
            //GondermeTenkisForm form = new GondermeTenkisForm();
            //form.belgeTur = ENUMBelgeTuru.ODENEKGONDERME;
            //form.yil = yil;
            //form.islemTarih = new TNSDateTime(DateTime.Now.ToShortDateString());
            //form.belgeNo = "";
            //form.kurum = OrtakServis.VarsayilanKurumBul();
            //form.muhasebe = muhasebe;
            //form.birim = birim;
            //form.aciklama = odenekAciklama;
            //form.islemYapan = kullanan.kullaniciKodu;
            //form.yaziNo = "";
            //form.yaziTarih = new TNSDateTime();
            //form.istekYaziNo = "";
            //form.istekYaziTarih = new TNSDateTime();
            //if (form.belgeNo == "")
            //    form.icmalNo = 0;

            //GondermeTenkisDetay detay = new GondermeTenkisDetay();
            //string[] tt = tertip.Split('-');
            //if (tt.Length > 4)
            //    detay.tertip = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);
            //else if (tt.Length > 3 && tt[3].Length > 3)//Ekokod 4 kırılım ise 2 ye düşür
            //    tertip = tertip.Substring(0, tertip.Length - 3);

            //detay.tertip.TertipAta(tertip);
            //detay.miktar = ihtiyac;
            //form.detay.objeler.Add(detay);

            //sonuc = servisMUH.GondermeTenkisKaydet(kullanan, form, false);
            //return sonuc;
        }

        private void OdemeEmriOlustur(Kullanici kullanan, int yil, int ay, double faturaTutar, string surecNo, int muhasebe, string birim, int aboneTur)
        {
            Sonuc sonuc = new Sonuc();

            TNS.MUH.OdemeEmriMIFForm tf = new TNS.MUH.OdemeEmriMIFForm();
            tf.belgeNo = "";
            tf.muhasebe = muhasebe;
            tf.birim = birim;
            tf.yil = DateTime.Now.Year;
            tf.islemTarih = new TNSDateTime(DateTime.Now.ToShortDateString());
            tf.formDurumTarih = new TNSDateTime(DateTime.Now.ToString());
            tf.kurum = OrtakServis.VarsayilanKurumBul();
            tf.islemYapan = kullanan.kullaniciKodu;
            tf.surecYil = DateTime.Now.Year;
            tf.surecNo = surecNo;

            if (aboneTur == (int)ENUMAboneTip.TELEFON)
                tf.aciklama = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00053, ay.ToString("00"), yil);
            if (aboneTur == (int)ENUMAboneTip.ELEKTRIK)
                tf.aciklama = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00054, ay.ToString("00"), yil);
            if (aboneTur == (int)ENUMAboneTip.SU)
                tf.aciklama = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00055, ay.ToString("00"), yil);

            //İlgili///////////////////////////////////////////////////////////////
            TNS.HRC.AboneNo abone = new TNS.HRC.AboneNo();
            abone.muhasebe = muhasebe;
            abone.birim = birim;
            abone.aboneTip = aboneTur;

            ObjectArray listeAbone = new ObjectArray();
            listeAbone = new HRCAboneNo().Listele(kullanan, abone);
            foreach (TNS.HRC.AboneNo p in listeAbone.objeler)
            {
                tf.ilgiliAd = p.firma.ad;
                tf.ilgiliBankaAd = p.firma.banka;
                tf.ilgiliBankaNo = p.firma.hesapNo;
                tf.ilgiliNo = p.firma.vergiNo;
                tf.ilgiliVD = p.firma.vergiDairesi;
            }
            ///////////////////////////////////////////////////////////////

            string borcHesap = "";
            string yansitHesap = "";
            double damgaVergisi = 0;
            double kasa = 0;
            string ekoKod4 = "";

            if (aboneTur == (int)ENUMAboneTip.TELEFON)
            {
                borcHesap = "630.03.05.02.02";
                yansitHesap = "830.03.05.02.02";
                ekoKod4 = "03.5.2.02";
                damgaVergisi = ((faturaTutar / 1.33) * 1.15) * 0.00825;
            }
            else if (aboneTur == (int)ENUMAboneTip.ELEKTRIK)
            {
                borcHesap = "630.03.02.03.03";
                yansitHesap = "830.03.02.03.03";
                ekoKod4 = "03.2.3.03";
                damgaVergisi = (faturaTutar / 1.18) * 0.00825;
            }
            else if (aboneTur == (int)ENUMAboneTip.SU)
            {
                borcHesap = "630.03.02.02.01";
                yansitHesap = "830.03.02.02.01";
                ekoKod4 = "03.2.2.01";
                damgaVergisi = 0;
            }

            string tertip = OdemeEmriTertipVer(DateTime.Now.Year, muhasebe, birim, ekoKod4);
            damgaVergisi = Math.Round(damgaVergisi, 2);
            kasa = faturaTutar - damgaVergisi;

            TNS.MUH.OdemeEmriMIFDetay td = new TNS.MUH.OdemeEmriMIFDetay(tf.yil);
            string[] tt = tertip.Split('-');

            td.hesapKod = borcHesap;
            TNS.MUH.Arac.TertipAta(ref td, tertip);
            td.borc = faturaTutar;
            td.alacak = 0;
            tf.detay.objeler.Add(td);

            if (damgaVergisi > 0)
            {
                td = new TNS.MUH.OdemeEmriMIFDetay(tf.yil);
                td.hesapKod = OrtakServis.DamgaVergisiHesapKodu(tf.ilgiliNo, false);
                td.borc = 0;
                td.alacak = damgaVergisi;
                tf.detay.objeler.Add(td);
            }

            td = new TNS.MUH.OdemeEmriMIFDetay(tf.yil);
            td.hesapKod = "325.01.13";
            if (OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                td.hesapKod = "325.01.13";
            else
                td.hesapKod = "103.02";
            td.borc = 0;
            td.alacak = kasa;
            tf.detay.objeler.Add(td);

            td = new TNS.MUH.OdemeEmriMIFDetay(tf.yil);
            td.hesapKod = yansitHesap;
            TNS.MUH.Arac.TertipAta(ref td, tertip);
            td.borc = faturaTutar;
            td.alacak = 0;
            tf.detay.objeler.Add(td);

            if (damgaVergisi > 0 && OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
            {
                td = new TNS.MUH.OdemeEmriMIFDetay(tf.yil);
                td.hesapKod = OrtakServis.DamgaVergisiYansitmaHesapKodu();
                td.borc = 0;
                td.alacak = damgaVergisi;
                tf.detay.objeler.Add(td);
            }

            td = new TNS.MUH.OdemeEmriMIFDetay(tf.yil);
            td.hesapKod = "835";
            td.borc = 0;
            td.alacak = faturaTutar;
            tf.detay.objeler.Add(td);

            if (damgaVergisi > 0 && OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
            {
                td = new TNS.MUH.OdemeEmriMIFDetay(tf.yil);
                td.hesapKod = "805";
                td.borc = damgaVergisi;
                td.alacak = 0;
                tf.detay.objeler.Add(td);
            }

            sonuc = servisMUH.OdemeEmriMIFKaydet(kullanan, tf);

            if (sonuc.islemSonuc)
            {
                TNS.HRC.AboneFatura bilgi = new TNS.HRC.AboneFatura();
                bilgi.yil = DateTime.Now.Year;
                bilgi.muhasebe = muhasebe;
                bilgi.birim = birim;
                bilgi.surecNo = surecNo;
                bilgi.aboneTip = aboneTur;
                bilgi.odemeEmriNo = sonuc.anahtar;
                sonuc = new HRCAboneFatura().OdemeEmriNoKaydet(kullanan, bilgi);

                if (sonuc.islemSonuc)
                {
                    //******************************************************************
                    //Ödeme emri tablosuna oluşturulan ödeme emri bilgisni kaydet
                    TNS.HRC.OdemeEmriMIFForm f = new TNS.HRC.OdemeEmriMIFForm();
                    f.yil = DateTime.Now.Year;
                    f.surecNo = surecNo;
                    f.muhasebe = muhasebe;
                    f.birim = birim;
                    f.odemeEmriNo = bilgi.odemeEmriNo;
                    f.prFaturaNo = 0;
                    f.prOdemeEmriNo = -1;
                    f.aciklama = "";
                    f.belgeTur = 0;
                    f.detay = new ObjectArray();
                    f.firmaNo = 0;
                    f.odemeEmriTarih = new TNSDateTime(System.DateTime.Now.ToString());
                    f.toplamOdeme = 0;
                    new HRCOdemeEmriForm().Kaydet(kullanan, f, 0, 0);
                    //******************************************************************
                }
                else
                    OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - Ödeme Emri Oluştur. Muhasebe:" + muhasebe + " Birim:" + birim + " Süreç No:" + surecNo + " Hata:" + sonuc.hataStr.Replace("<b>", "").Replace("</b>", "").Replace("<br>", ""), false);
            }
            else
                OrtakFonksiyonlar.HataDosyaYaz(hataDosyaAd, DateTime.Now.ToString() + " - Ödeme Emri Oluştur. Muhasebe:" + muhasebe + " Birim:" + birim + " Süreç No:" + surecNo + " Hata:" + sonuc.hataStr.Replace("<b>", "").Replace("</b>", "").Replace("<br>", ""), false);
        }

        private string OdemeEmriTertipVer(int yil, int muhasebe, string birim, string eko)
        {
            ObjectArray tVAN = new ObjectArray();
            string donusDeger = "";

            tVAN = new HRCButce().MuhasebeBirimTertipListele(muhasebe, birim, eko);
            TNS.MUH.ENUMOdenekGondermeTur ogTur = TNS.MUH.Arac.OdenekGondermeTertipYapi(yil);

            if (tVAN.sonuc.islemSonuc)
            {
                foreach (MuhasebeBirimTertip t in tVAN.objeler)
                {
                    donusDeger = t.kurKod + "-" + t.fonKod + "-" + t.finKod + "-" + t.ekoKod;
                    if (ogTur != ENUMOdenekGondermeTur.eko2)
                        donusDeger += "-" + t.pebEkoKod;

                    string kur = t.kurKod;
                    if (kur.EndsWith("62") || kur.EndsWith("63"))
                        return donusDeger;
                }
            }

            return donusDeger;
        }

        private static Hashtable HarcamaListe(ObjectArray harcamaListe)
        {
            Hashtable htHarcama = new Hashtable();
            foreach (ButceHarcama butceHarcama in harcamaListe.objeler)
            {
                string key = butceHarcama.muhasebeKod + "-" + butceHarcama.birimKod;
                if (htHarcama.Contains(key))
                    htHarcama[key] = OrtakFonksiyonlar.ConvertToDouble(htHarcama[key]) + butceHarcama.harcama;
                else
                    htHarcama.Add(key, butceHarcama.harcama);
            }
            return htHarcama;
        }

        private static Hashtable OdenekListe(ObjectArray butceGiderListe)
        {
            Hashtable htOdenek = new Hashtable();
            foreach (TNS.HRC.ButceGider butceGider in butceGiderListe.objeler)
            {
                string key = butceGider.muhasebe + "-" + butceGider.birim;
                if (htOdenek.Contains(key))
                    htOdenek[key] = OrtakFonksiyonlar.ConvertToDouble(htOdenek[key]) + butceGider.odenekGonderme;
                else
                    htOdenek.Add(butceGider.muhasebe + "-" + butceGider.birim, butceGider.odenekGonderme);
            }
            return htOdenek;
        }

        private static Hashtable TertipListe(ObjectArray tertipListe)
        {
            Hashtable htTertip = new Hashtable();
            foreach (MuhasebeBirimTertip p in tertipListe.objeler)
                htTertip.Add(p.muhasebeKod + "-" + p.birimKod, p.kurKod + "-" + p.fonKod + "-" + p.finKod + "-" + p.ekoKod);

            return htTertip;
        }

        private static Hashtable FaturaToplamListe(ObjectArray faturaListe)
        {
            Hashtable htFaturaToplam = new Hashtable();
            double faturaToplam = 0;
            int eskiMuhasebe = 0;
            string eskiBirim = "";
            foreach (TNS.HRC.AboneFatura fatura in faturaListe.objeler)
            {
                if (eskiMuhasebe != fatura.muhasebe || eskiBirim != fatura.birim)
                {
                    if (faturaToplam > 0)
                    {
                        string key = eskiMuhasebe + "-" + eskiBirim;
                        if (htFaturaToplam.Contains(key))
                            htFaturaToplam[key] = OrtakFonksiyonlar.ConvertToDouble(htFaturaToplam[key]) + faturaToplam;
                        else
                            htFaturaToplam.Add(eskiMuhasebe + "-" + eskiBirim, faturaToplam);
                    }

                    faturaToplam = 0;
                }
                faturaToplam += fatura.faturaTutar;
                eskiMuhasebe = fatura.muhasebe;
                eskiBirim = fatura.birim;
            }
            if (faturaToplam > 0)
            {
                string key = eskiMuhasebe + "-" + eskiBirim;
                if (htFaturaToplam.Contains(key))
                    htFaturaToplam[key] = OrtakFonksiyonlar.ConvertToDouble(htFaturaToplam[key]) + faturaToplam;
                else
                    htFaturaToplam.Add(eskiMuhasebe + "-" + eskiBirim, faturaToplam);
            }

            return htFaturaToplam;
        }
    }
}
