using System;
using System.Collections;
using System.Data;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using System.Collections.Generic;
using TNS.DEG;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class MUHFaturaTakip
    {
        /// <summary>
        /// Kaydets the specified ve.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">The bilgi</param>
        /// <returns></returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, FaturaTakip bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABFaturaTakip to = new TABFaturaTakip(ve);

            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH201;
            }
            else
            {
                ve.Rollback();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH001;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Listeles the specified ve.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, FaturaTakip form)
        {
            string hata = "";

            ObjectArray oslar = new ObjectArray();
            Hashtable birimAd = new Hashtable();

            //Listeleme sırasında kurum kullanıcıları kendilerine ait evrakları görsün
            MUHMuhasebeBirimServis.KullanicininYetkiliOlduguBirimler(kullanan, form);
            if (form.aciklama == "YetkisiYok")
                hata = "İşlem yapmaya yetkiniz yok.<br>";
            else
            {
                string hesapKod = "";
                if (form.detay.objeler.Count > 0)
                {
                    OdemeEmriMIFDetay detay = (OdemeEmriMIFDetay)form.detay[0];
                    hesapKod = detay.hesapKod.Replace(".", "");
                }

                if (string.IsNullOrWhiteSpace(hesapKod))
                    hata = "Hesap kodu boş bırakılamaz.<br>";
            }

            if (hata != "")
            {
                oslar.sonuc.DegerAta(hata);
                return oslar;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim veSorgu = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));


            TABFaturaTakip to = new TABFaturaTakip(ve);
            IDataReader r = to.Listele(form);

            if (to.Hata() == "")
            {
                string belgeNo = "";
                string eskiBelgeNo = "";
                FaturaTakip os = null;

                Tertip th = new Tertip(form.yil, ENUMTabloKod.TABOdemeEmri);
                Tertip t = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);
                Hashtable htIst = new Hashtable();
                while (r.Read())
                {
                    belgeNo = VeriErisim.DegerAl(r, 2, "");

                    if (eskiBelgeNo != belgeNo)
                    {
                        if (os != null)
                        {
                            //Eğer belgede kurumsal kod yok ise birim adı hatalı doldurulabilir
                            if (string.IsNullOrEmpty(os.birimAd))
                            {
                                if (birimAd[os.muhasebe.ToString() + os.birim + os.kurKod] == null)
                                {
                                    os.birimAd = MUHServis.UzayYaziDegerGetir(veSorgu, kullanan, "MUHMUHASEBEBIRIM", os.birim + "-" + os.muhasebe.ToString() + "-" + os.kurKod, false, "BIRIMAD");
                                    if (os.birimAd == "")
                                        os.birimAd = MUHServis.UzayYaziDegerGetir(veSorgu, kullanan, "MUHMUHASEBEBIRIM", os.birim + "-" + os.muhasebe.ToString() + "-" + MUH.Yetki.VarsayilanKurumBul(), false, "BIRIMAD");
                                    birimAd[os.muhasebe.ToString() + os.birim + os.kurKod] = os.birimAd;
                                }
                                else
                                    os.birimAd = (string)birimAd[os.muhasebe.ToString() + os.birim + os.kurKod];
                            }
                            oslar.objeler.Add(os);
                        }

                        os = new FaturaTakip();
                        os.yil = VeriErisim.DegerAl(r, 0, 0);
                        os.belgeTur = (ENUMBelgeTuru)VeriErisim.DegerAl(r, 1, 0);
                        os.belgeNo = VeriErisim.DegerAl(r, 2, "");
                        os.kurum = VeriErisim.DegerAl(r, 3, "");
                        os.muhasebe = VeriErisim.DegerAl(r, 4, 0);
                        os.birim = VeriErisim.DegerAl(r, 5, "");
                        os.ilgiliAd = VeriErisim.DegerAl(r, 6, "");
                        os.ilgiliNo = VeriErisim.DegerAl(r, 7, "");
                        os.ilgiliBankaAd = VeriErisim.DegerAl(r, 8, "");
                        os.ilgiliBankaNo = VeriErisim.DegerAl(r, 9, "");
                        os.ilgiliVD = VeriErisim.DegerAl(r, 10, "");
                        os.aciklama = VeriErisim.DegerAl(r, 11, "");
                        os.islemTarih = VeriErisim.DegerAl(r, 12);
                        os.formDurumTarih = VeriErisim.DegerAl(r, 13);
                        os.formDurum = VeriErisim.DegerAl(r, 14, 0);
                        os.iptalNeden = VeriErisim.DegerAl(r, 15, "");
                        os.YEVYevTarih = VeriErisim.DegerAl(r, 16);
                        os.YEVYevNo = VeriErisim.DegerAl(r, 17, 0);
                        os.IOETarih = VeriErisim.DegerAl(r, 18);

                        os.OEBTur = VeriErisim.DegerAl(r, 24, "");
                        os.OEBTarih = VeriErisim.DegerAl(r, 25);
                        os.OEBNo = VeriErisim.DegerAl(r, 26, "");
                        os.YEVCekNo = VeriErisim.DegerAl(r, 29, "");
                        if (os.YEVCekNo != "")
                            os.YEVCekTarih = os.IOETarih;

                        os.kurum = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), os.kurum);
                        os.islemYapan = VeriErisim.DegerAl(r, 30, "");
                        os.projeNo = VeriErisim.DegerAl(r, 31, "");
                        os.ihale = VeriErisim.DegerAl(r, 32, "");
                        os.fisTurYap((ENUMFISTUR)VeriErisim.DegerAl(r, 33, 0));
                        os.ilgiliBelgeNo = VeriErisim.DegerAl(r, 34, "");

                        os.surecYil = VeriErisim.DegerAl(r, 35, 0);
                        os.surecNo = VeriErisim.DegerAl(r, 36, "");
                        os.kurKod = VeriErisim.DegerAl(r, 37, "");
                        os.organizasyonKodu = VeriErisim.DegerAl(r, 38, 0);

                        os.yevmiyeAciklama = VeriErisim.DegerAl(r, 39, "");
                        os.duzeltilenOdemeEmriNo = VeriErisim.DegerAl(r, 40, "");

                        os.istKod1.degerKod = VeriErisim.DegerAl(r, 41, 0);
                        os.istKod2.degerKod = VeriErisim.DegerAl(r, 42, 0);
                        os.istKod1.aciklama = IstatistikKodDeger.AciklamaBul(MUHIstatistikKodGiris.DegerAdBul(veSorgu, kullanan, 1, os.istKod1.degerKod, htIst));
                        os.istKod2.aciklama = IstatistikKodDeger.AciklamaBul(MUHIstatistikKodGiris.DegerAdBul(veSorgu, kullanan, 2, os.istKod2.degerKod, htIst));
                        os.kurumAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler1.KURAD, "");
                        os.muhasebeAd = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler1.MUHASEBEAD, "");
                        {
                            os.borcToplam = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler1.BORC, 0.0);
                            os.alacakToplam = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler1.ALACAK, 0.0);

                            //Bu satır 17.07.2018 tarihinde eklendi. Tuğçe hanım listeleme ekranında ödeme emrine eklenmiş dosya var mı görmek istiyor. Melih
                            os.ekliDosyaVarmi = VeriErisim.DegerAl(r, (int)TABOdemeEmriMIF.ENUMSorguEkler1.ALACAK + 1, 0);
                        }

                        os.takipDurum = VeriErisim.DegerAl(r, 48, 0);
                    }

                    eskiBelgeNo = belgeNo;
                }
                if (os != null)
                {
                    //Eğer belgede kurumsal kod yok ise birim adı doldurulamıyor
                    if (string.IsNullOrEmpty(os.birimAd))
                    {
                        if (birimAd[os.muhasebe.ToString() + os.birim + os.kurKod] == null)
                        {
                            os.birimAd = MUHServis.UzayYaziDegerGetir(veSorgu, kullanan, "MUHMUHASEBEBIRIM", os.birim + "-" + os.muhasebe.ToString() + "-" + os.kurKod, false, "BIRIMAD");
                            if (os.birimAd == "")
                                os.birimAd = MUHServis.UzayYaziDegerGetir(veSorgu, kullanan, "MUHMUHASEBEBIRIM", os.birim + "-" + os.muhasebe.ToString() + "-" + MUH.Yetki.VarsayilanKurumBul(), false, "BIRIMAD");
                        }
                        else
                            os.birimAd = (string)birimAd[os.muhasebe.ToString() + os.birim + os.kurKod];
                    }
                    oslar.objeler.Add(os);
                }
                try
                {
                    r.Close();
                }
                catch { }
            }
            else
            {
                //OdemeEmriMIFForm os = new OdemeEmriMIFForm();
                //os.aciklama = "Listeleme sırasında bir hata oluştu. Lütfen kriter alanlarını kontrol ederek tekrar listeleme işlemi yapınız.<br>" + to.Hata();
                //oslar.formDizi.Add(os);
            }
            ve.VeriErisimKapat();
            veSorgu.VeriErisimKapat();

            return oslar;
        }


    }
}
