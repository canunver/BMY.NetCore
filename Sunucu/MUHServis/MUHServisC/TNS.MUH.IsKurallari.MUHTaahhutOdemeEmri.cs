using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using System;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// OdemeEmri işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHTaahhutOdemeEmri
    {
        /// <summary>
        /// OdemeEmri kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="odemeEmri">The odeme emri.</param>
        /// <returns></returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray odemeEmri)
        {
            Sonuc sonuc = new Sonuc(true, ButceMuhasebeResI.MUH001, "");
            string bilgiKod = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutOdemeEmri to = new TABTaahhutOdemeEmri(ve);
            TABTaahhutKarti tk = new TABTaahhutKarti(ve);
            bool kapali = false;

            foreach (TaahhutOdeme bilgi in odemeEmri.objeler)
            {
                kapali = tk.TaahhutKapaliMi(bilgi.taahhutKod);
                if (kapali == true)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ButceMuhasebeResI.MUH204;
                    break;
                }
                else
                {
                    if (bilgi.kod == 0)
                    {
                        //kod alınıyor
                        //**********************************************************
                        TABGenel tg = new TABGenel(ve);
                        string sayacKod = "TAAHHUTODEMEEMRI";
                        bilgi.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl(sayacKod, "0000"), 0);
                        //**********************************************************
                    }
                }

                sonuc = to.Kaydet(bilgi);
                if (!sonuc.islemSonuc)
                    break;
                else
                {
                    bilgiKod = bilgi.kod.ToString();

                    sonuc = to.SilDiger(bilgi.kod, 0);

                    foreach (TaahhutOdemeDiger od in bilgi.digerGelirler.objeler)
                    {
                        TABGenel tg = new TABGenel(ve);
                        string sayacKod = "TAAHHUTODEMEEMRIDIGER";
                        od.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl(sayacKod, "0000"), 0);
                        od.taahhutOdemeKod = bilgi.kod;
                        od.hesapKod = od.hesapKod.Replace(".", "");
                        od.hesapKodKDV = od.hesapKodKDV.Replace(".", "");
                        to.KaydetDiger(od);
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.anahtar = bilgiKod;
                sonuc.bilgiStr = ButceMuhasebeResI.MUH201;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// OdemeEmri
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public static ObjectArray Listele(Kullanici kullanan, TaahhutOdeme bilgi)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTaahhutOdemeEmri to = new TABTaahhutOdemeEmri(ve);

            IDataReader r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TaahhutOdeme odeme = new TaahhutOdeme();

                    odeme.kod = VeriErisim.DegerAl(r, 0, 0);
                    odeme.taahhutKod = VeriErisim.DegerAl(r, 1, 0);
                    odeme.odemeEmriKod = VeriErisim.DegerAl(r, 2, "");
                    odeme.odemeEmriTarihi = VeriErisim.DegerAl(r, 3);
                    odeme.aciklama = VeriErisim.DegerAl(r, 4, "");
                    odeme.hakedisTutari = VeriErisim.DegerAl(r, 5, 0.0);
                    odeme.damgaVergisi = VeriErisim.DegerAl(r, 6, 0.0);
                    odeme.gelirVergisiStopaji = VeriErisim.DegerAl(r, 7, 0.0);
                    odeme.fonPayi = VeriErisim.DegerAl(r, 8, 0.0);
                    odeme.paraCezalari = VeriErisim.DegerAl(r, 9, 0.0);
                    odeme.mahsupEdilenAvans = VeriErisim.DegerAl(r, 10, 0.0);
                    odeme.icraKesintisi = VeriErisim.DegerAl(r, 11, 0.0);
                    odeme.teminatKesintisi = VeriErisim.DegerAl(r, 12, 0.0);
                    odeme.sskPrimiVeGecikmeZammi = VeriErisim.DegerAl(r, 13, 0.0);
                    odeme.vergiBorcu = VeriErisim.DegerAl(r, 14, 0.0);
                    odeme.kdvTevkifati = VeriErisim.DegerAl(r, 15, 0.0);
                    odeme.temlik = VeriErisim.DegerAl(r, 16, 0.0);
                    odeme.butceEmaneti = VeriErisim.DegerAl(r, 17, 0.0);
                    odeme.kdv = VeriErisim.DegerAl(r, 18, 0.0);
                    odeme.kesintilerToplami = VeriErisim.DegerAl(r, 19, 0.0);
                    odeme.yapilanOdeme = VeriErisim.DegerAl(r, 20, 0.0);
                    odeme.hakedisHesap = VeriErisim.DegerAl(r, 21, "");
                    odeme.odemeHesap = VeriErisim.DegerAl(r, 22, "");
                    odeme.yansitmaHesap = VeriErisim.DegerAl(r, 23, "");
                    odeme.islemTarihi = VeriErisim.DegerAl(r, 24);
                    odeme.islemYapanKod = VeriErisim.DegerAl(r, 25, "");
                    odeme.islemYapanAd = VeriErisim.DegerAl(r, 26, "");
                    odeme.kdvHesapKodu = VeriErisim.DegerAl(r, 27, "");
                    odeme.damgaVergisiHesapKodu = VeriErisim.DegerAl(r, 28, "");
                    odeme.gelirVergisiHesapKodu = VeriErisim.DegerAl(r, 29, "");
                    odeme.fonPayiHesapKodu = VeriErisim.DegerAl(r, 30, "");
                    odeme.paraCezalariHesapKodu = VeriErisim.DegerAl(r, 31, "");
                    odeme.mahsupEdileAvansHesapKodu = VeriErisim.DegerAl(r, 32, "");
                    odeme.icraKesintisiHesapKodu = VeriErisim.DegerAl(r, 33, "");
                    odeme.teminatKesintisiHesapKodu = VeriErisim.DegerAl(r, 34, "");
                    odeme.sskPrimiGecikmeZammiHesapKodu = VeriErisim.DegerAl(r, 35, "");
                    odeme.vergiBorcuHesapKodu = VeriErisim.DegerAl(r, 36, "");
                    odeme.kdvTevkifatiHesapKodu = VeriErisim.DegerAl(r, 37, "");
                    odeme.temlikHesapKodu = VeriErisim.DegerAl(r, 38, "");
                    odeme.butceEmanetiHesapKodu = VeriErisim.DegerAl(r, 39, "");
                    odeme.kdvHesapKodu2 = VeriErisim.DegerAl(r, 40, "");
                    odeme.kdv2 = VeriErisim.DegerAl(r, 41, 0);
                    odeme.kdvHesapKodu3 = VeriErisim.DegerAl(r, 42, "");
                    odeme.kdv3 = VeriErisim.DegerAl(r, 43, 0);
                    odeme.damgaVergisiHesapKodu2 = VeriErisim.DegerAl(r, 44, "");
                    odeme.damgaVergisi2 = VeriErisim.DegerAl(r, 45, 0);
                    odeme.kiraGeliriHesapKodu = VeriErisim.DegerAl(r, 46, "");
                    odeme.kiraGeliri = VeriErisim.DegerAl(r, 47, 0);
                    odeme.kiraGeliriKDVHesapKodu = VeriErisim.DegerAl(r, 48, "");
                    odeme.kiraGeliriKDV = VeriErisim.DegerAl(r, 49, 0);
                    odeme.cesitliGelirHesapKodu = VeriErisim.DegerAl(r, 50, "");
                    odeme.cesitliGelir = VeriErisim.DegerAl(r, 51, 0);
                    odeme.cesitliGelirKDVHesapKodu = VeriErisim.DegerAl(r, 52, "");
                    odeme.cesitliGelirKDV = VeriErisim.DegerAl(r, 53, 0);
                    odeme.digerHizmetGelirHesapKodu = VeriErisim.DegerAl(r, 54, "");
                    odeme.digerHizmetGelir = VeriErisim.DegerAl(r, 55, 0);
                    odeme.digerHizmetGelirKDVHesapKodu = VeriErisim.DegerAl(r, 56, "");
                    odeme.digerHizmetGelirKDV = VeriErisim.DegerAl(r, 57, 0);
                    iVAN.objeler.Add(odeme);
                }
                r.Close();
            }

            if (bilgi.kod == 0)
                OdemeEmriBelgesindenGelenleriEkle(ref iVAN, kullanan, bilgi.taahhutKod);
            else
            {
                r = to.ListeleDiger(bilgi.kod);
                if (to.Hata() == "")
                {
                    while (r.Read())
                    {
                        TaahhutOdemeDiger odeme = new TaahhutOdemeDiger();
                        odeme.kod = VeriErisim.DegerAl(r, 0, 0);
                        odeme.taahhutKod = VeriErisim.DegerAl(r, 1, 0);
                        odeme.taahhutOdemeKod = VeriErisim.DegerAl(r, 2, 0);
                        odeme.hesapKod = VeriErisim.DegerAl(r, 3, "");
                        odeme.borc = VeriErisim.DegerAl(r, 4, 0.0);
                        odeme.alacak = VeriErisim.DegerAl(r, 5, 0.0);
                        odeme.hesapKodKDV = VeriErisim.DegerAl(r, 6, "");
                        odeme.borcKDV = VeriErisim.DegerAl(r, 7, 0.0);
                        odeme.alacakKDV = VeriErisim.DegerAl(r, 8, 0.0);

                        if (iVAN.objeler.Count > 0)
                            ((TaahhutOdeme)iVAN.objeler[0]).digerGelirler.objeler.Add(odeme);
                    }
                    r.Close();
                }
            }

            ve.VeriErisimKapat();

            return iVAN;
        }

        public static void OdemeEmriBelgesindenGelenleriEkle(ref ObjectArray iVAN, Kullanici kullanan, int taahhutKod)
        {
            string projeNo = "";
            string altHesapKod = "";

            TaahhutKartiBilgi taahhut = new TaahhutKartiBilgi();
            taahhut.kod = taahhutKod;
            ObjectArray donusDeger = MUHTaahhutKarti.Listele(null, kullanan, taahhut, 0);
            if (donusDeger.objeler.Count > 0)
            {
                projeNo = ((TaahhutKartiBilgi)donusDeger.objeler[0]).projeSiraNo;
                altHesapKod = ((TaahhutKartiBilgi)donusDeger.objeler[0]).muteahhitKimlikNo;
            }
            else
                return;


            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime(new DateTime(2008, 1, 1));
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime(new DateTime(DateTime.Now.Year, 12, 31));

            TNS.MUH.OdemeEmriMIFForm oEmri = new TNS.MUH.OdemeEmriMIFForm();
            oEmri.yil = 0;
            oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            oEmri.formDurum = 22;
            oEmri.sorguEkoSiraliMi = true;
            oEmri.projeNo = projeNo;

            TNS.MUH.OdemeEmriMIFDetay oEmriDetay = new TNS.MUH.OdemeEmriMIFDetay(DateTime.Now.Year);
            oEmriDetay.hesapKod = "920";
            oEmriDetay.altHesapKod = altHesapKod;
            oEmri.detay.objeler.Add(oEmriDetay);

            ObjectArray mbler = new ObjectArray();

            TNS.MUH.BuyukDefter[] mb = new MUHMizanBilanco().BuyukDefterListele(kullanan, oEmri, 0, 3000, "10");

            for (int i = 0; i < mb.Length; i++)
            {
                TaahhutOdeme odeme = new TaahhutOdeme();
                odeme.odemeEmriKod = mb[i].belgeNo;
                odeme.odemeEmriTarihi = mb[i].islemTarih;
                odeme.hakedisTutari = mb[i].alacak;
                odeme.aciklama = mb[i].aciklama;

                if (mb[i].alacakDoviz > 0)
                    odeme.hakedisTutari = mb[i].alacakDoviz;

                bool bulundu = false;
                foreach (TaahhutOdeme ko in iVAN.objeler)
                {
                    if (ko.odemeEmriKod == odeme.odemeEmriKod && ko.odemeEmriTarihi.Yil == odeme.odemeEmriTarihi.Yil)
                    {
                        bulundu = true;
                        break;
                    }
                }

                if (odeme.aciklama.IndexOf("DÜZELTME") > -1) continue;//Eğer düzeltme kaydı yapıldı ise ödeme yapılmış gibi algılanıyor 16.02.2018 Melih EGO
                if (bulundu) continue;
                if (mb[i].alacak == 0) continue;//Açılış taha

                iVAN.objeler.Add(odeme);
            }
        }

        /// <summary>
        /// odeme emri sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kod">The kod.</param>
        /// <returns></returns>
        public Sonuc Sil(Kullanici kullanan, int kod)
        {
            Sonuc sonuc = new Sonuc(true, ButceMuhasebeResI.MUH001, "");

            if (kod == 0)
            {
                sonuc.DegerAta(ButceMuhasebeResI.MUH133);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTaahhutOdemeEmri to = new TABTaahhutOdemeEmri(ve);

            sonuc = to.Sil(kod);
            sonuc = to.SilDiger(kod, 0);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kayıt silindi";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public double HazirlananOdemeEmriTutari(Kullanici kullanan, int taahhutKod)
        {
            double toplamOdemeEmri = 0.0;

            //Taahhuk bedelini bulmak için
            TaahhutOdeme odeme = new TaahhutOdeme();
            odeme.taahhutKod = taahhutKod;
            ObjectArray liste = MUHTaahhutOdemeEmri.Listele(kullanan, odeme);

            foreach (TaahhutOdeme t in liste.objeler)
            {
                toplamOdemeEmri += t.hakedisTutari;
            }

            return toplamOdemeEmri;
        }

        public double HazirlananOdemeEmriTutariKapatmaFarki(Kullanici kullanan, int taahhutKod)
        {
            string projeNo = "";
            string altHesapKod = "";

            TaahhutKartiBilgi taahhut = new TaahhutKartiBilgi();
            taahhut.kod = taahhutKod;
            ObjectArray donusDeger = MUHTaahhutKarti.Listele(null, kullanan, taahhut, 0);
            if (donusDeger.objeler.Count > 0)
            {
                projeNo = ((TaahhutKartiBilgi)donusDeger.objeler[0]).projeSiraNo;
                altHesapKod = ((TaahhutKartiBilgi)donusDeger.objeler[0]).muteahhitKimlikNo;
            }
            else
                return -1;


            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime(new DateTime(2008, 1, 1));
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime(new DateTime(DateTime.Now.Year, 12, 31));

            TNS.MUH.OdemeEmriMIFForm oEmri = new TNS.MUH.OdemeEmriMIFForm();
            oEmri.yil = 0;
            oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            oEmri.formDurum = 22;
            oEmri.sorguEkoSiraliMi = true;
            oEmri.projeNo = projeNo;

            TNS.MUH.OdemeEmriMIFDetay oEmriDetay = new TNS.MUH.OdemeEmriMIFDetay(DateTime.Now.Year);
            oEmriDetay.hesapKod = "920";
            oEmriDetay.altHesapKod = altHesapKod;
            oEmri.detay.objeler.Add(oEmriDetay);

            ObjectArray mbler = new ObjectArray();

            TNS.MUH.BuyukDefter[] mb = new MUHMizanBilanco().BuyukDefterListele(kullanan, oEmri, 0, 3000, "10");

            double borc = 0;
            double alacak = 0;
            for (int i = 0; i < mb.Length; i++)
            {
                double satirBorc = mb[i].borc;
                double satirAlacak = mb[i].alacak;

                if (mb[i].borcDoviz > 0)
                    satirBorc = mb[i].borcDoviz;

                if (mb[i].alacakDoviz > 0)
                    satirAlacak = mb[i].alacakDoviz;

                borc += satirBorc;
                alacak += satirAlacak;
            }

            return borc - alacak;
        }
    }
}
