using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.MUH;
using TNS.DEG;
using TNS.BKD;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Ödeme emri belge bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCOdemeEmriForm
    {
        /// <summary>
        /// Bütçe Muhasebe Modülü servisine ulaşmak için kullanılan değişken
        /// </summary>
        public static IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

        /// <summary>
        /// Arguman olarak verilen, ödeme emri belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, OdemeEmriMIFForm kriter, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (kriter.odemeEmriTarih.isNull)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00228 + "<br>";

            int satir = 0;
            foreach (OdemeEmriMIFDetay t in kriter.detay.objeler)
            {
                satir++;
                if (t.hesapPlanKod == "")
                    hata += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00229, satir) + "<br>";
                if (t.tertip.Length > 3 && t.tertip.Substring(0, 3) == "630")
                    kriter.toplamOdeme += t.borc;
            }

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            //bool tamamlandi = true;
            int yil = kriter.yil;
            string surecNo = kriter.surecNo;
            //if (parcaKod > -1)
            //{
            //    tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            //    if (tamamlandi)
            //    {
            //        sonuc.islemSonuc = false;
            //        sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
            //        return sonuc;
            //    }
            //}
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriForm to = new TABOdemeEmriForm(ve);

            if (kriter.prOdemeEmriNo == -1)//Ödeme emri no gelmiştir. Ödeme emrinodan prödemeemrino bul, bulamazsan prOdemeEmriNo=0 olacak
            {
                if (kriter.odemeEmriNo.Trim() != "")
                    kriter.prOdemeEmriNo = to.PrOdemeEmriNoBul(kriter.yil, kriter.surecNo, kriter.odemeEmriNo);
                else
                    kriter.prOdemeEmriNo = 0;
            }

            if (kriter.prOdemeEmriNo == 0)
            {
                //ÖdemeEmri kod alınıyor
                //**********************************************************
                TABGenel tg = new TABGenel(ve);

                string sayacKod = "HRCODEMEEMRINO";

                kriter.prOdemeEmriNo = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl(sayacKod, "0"), 0);
                //**********************************************************
            }

            if (kriter.prFaturaNoEk != "")
            {
                string yeniPrFaturaNoEk = "";
                string[] fler = kriter.prFaturaNoEk.Replace("|", "").Split(';');
                foreach (string fn in fler)
                {
                    if (fn.Trim() == "") continue;

                    string fn2 = "|" + fn.Trim() + "|";
                    if (yeniPrFaturaNoEk != "") yeniPrFaturaNoEk += ";";
                    yeniPrFaturaNoEk += fn2;

                    if (kriter.prFaturaNo == 0)//İlk prfatura bilgisini prFaturaNo alanına yazmak için
                        kriter.prFaturaNo = OrtakFonksiyonlar.ConvertToInt(fn, 0);
                }

                kriter.prFaturaNoEk = yeniPrFaturaNoEk;
            }

            ve.BeginTransaction();

            sonuc = to.Kaydet(kriter);
            if (sonuc.islemSonuc)
            {
                sonuc = to.SilDetay(kriter);
                if (sonuc.islemSonuc && kriter.detay.objeler.Count > 0)
                    sonuc = to.KaydetDetay(kriter);
            }

            if (sonuc.islemSonuc)
            {
                //Parca Adım devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                ///////////////////////////////////////////////////////////////////////////

                sonuc.anahtar = kriter.prOdemeEmriNo.ToString();
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
        /// Arguman olarak verilen kriterlere bağlı olarak, ödeme emri belge bilgisini veritabanından okur 
        /// ve OdemeEmriMIFForm nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ödeme emri belge listesini OdemeEmriMIFForm nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABOdemeEmriForm to = new TABOdemeEmriForm(ve);
            IDataReader r = to.Listele(kriter);

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
                    f.prFaturaNoEk = VeriErisim.DegerAl(r, 10, "");

                    f.odemeEmriYil = f.odemeEmriTarih.Oku().Year;

                    f.detay = ListeleDetay(kullanan, f);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        ///// <summary>
        ///// Arguman olarak verilen kriterlere bağlı olarak, ödeme emri belgesi miktar toplam bilgisini veritabanından okur 
        ///// ve OdemeEmriMIFDetay nesne koleksiyonu olarak döner
        ///// </summary>
        ///// <param name="kullanan">The kullanan.</param>
        ///// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        ///// <param name="hesapKod">İşlem yapılacak hesap kod bilgisi</param>
        ///// <returns>İstenilen kriterdeki, ödeme emri belgesi miktar toplam bilgisi listesini OdemeEmriMIFDetay nesne koleksiyonu olarak tutan nesne</returns>
        //public ObjectArray ListeleMiktar(Kullanici kullanan, OdemeEmriMIFForm kriter, string hesapKod)
        //{
        //    ObjectArray bilgi = new ObjectArray();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

        //    TABOdemeEmriForm to = new TABOdemeEmriForm(ve);
        //    IDataReader r = to.ListeleMiktar(kriter, hesapKod);

        //    if (to.Hata() == "")
        //    {
        //        while (r.Read())
        //        {
        //            OdemeEmriMIFDetay t = new OdemeEmriMIFDetay();

        //            t.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, VeriErisim.DegerAl(r, 0, ""));
        //            t.borc = VeriErisim.DegerAl(r, 1, 0.0);
        //            t.alacak = VeriErisim.DegerAl(r, 2, 0.0);
        //            t.hesapPlanAd = HRCServis.UzayYaziDegerGetir("MUHHESAPPLANI", kriter.yil.ToString() + "-" + t.hesapPlanKod, true, "ACIKLAMA");
        //            t.tertip = VeriErisim.DegerAl(r, 3, "");
        //            bilgi.objeler.Add(t);
        //        }
        //    }

        //    r.Close();

        //    ve.VeriErisimKapat();

        //    return bilgi;
        //}

        ///// <summary>
        ///// Arguman olarak verilen kriterlere bağlı olarak, ödeme emri belgesi miktar toplam bilgisini veritabanından okur 
        ///// ve OdemeEmriMIFDetay nesne koleksiyonu olarak döner
        ///// </summary>
        ///// <param name="kullanan">The kullanan.</param>
        ///// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        ///// <param name="hesapKod">İşlem yapılacak hesap kod bilgisi</param>
        ///// <returns>İstenilen kriterdeki, ödeme emri belgesi miktar toplam bilgisi listesini OdemeEmriMIFDetay nesne koleksiyonu olarak tutan nesne</returns>
        //public ObjectArray ListeleMiktar(Kullanici kullanan, OdemeEmriMIFForm kriter, string hesapKod, string tertip)
        //{
        //    ObjectArray bilgi = new ObjectArray();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

        //    TABOdemeEmriForm to = new TABOdemeEmriForm(ve);
        //    IDataReader r = to.ListeleMiktar(kriter, hesapKod, tertip);

        //    if (to.Hata() == "")
        //    {
        //        while (r.Read())
        //        {
        //            OdemeEmriMIFDetay t = new OdemeEmriMIFDetay();

        //            t.yil = VeriErisim.DegerAl(r, 0, 0);
        //            t.surecNo = VeriErisim.DegerAl(r, 1, string.Empty);
        //            t.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, VeriErisim.DegerAl(r, 2, ""));
        //            t.borc = VeriErisim.DegerAl(r, 3, 0.0);
        //            t.alacak = VeriErisim.DegerAl(r, 4, 0.0);
        //            t.hesapPlanAd = HRCServis.UzayYaziDegerGetir("MUHHESAPPLANI", kriter.yil.ToString() + "-" + t.hesapPlanKod, true, "ACIKLAMA");
        //            t.tertip = VeriErisim.DegerAl(r, 5, "");
        //            bilgi.objeler.Add(t);
        //        }
        //    }

        //    r.Close();

        //    ve.VeriErisimKapat();

        //    return bilgi;
        //}

        /// <summary>
        /// Arguman olarak verilen, muhasebe işlem fişi bilgisinin durum bilgisini iptal olarak veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MifIptal(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            //Sonuc sonuc = new Sonuc(true, "İptal edilecek sonuç bulunamadı veya Sürece bağlı belgeler daha önceden iptal edilmiş.", "");
            Sonuc sonuc = new Sonuc(true, "", "");

            TNSCollection fler = new TNSCollection();

            TNS.MUH.OdemeEmriMIFForm muhOdemeEmriMIFForm = new TNS.MUH.OdemeEmriMIFForm();
            muhOdemeEmriMIFForm.yil = kriter.odemeEmriYil;
            if (muhOdemeEmriMIFForm.yil == 0)
                muhOdemeEmriMIFForm.yil = kriter.yil;

            muhOdemeEmriMIFForm.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.IPTAL;
            muhOdemeEmriMIFForm.belgeNo = kriter.odemeEmriNo;
            muhOdemeEmriMIFForm.kurum = OrtakServis.VarsayilanKurumBul();

            if (muhOdemeEmriMIFForm.belgeNo != "")
            {
                int belgeDurum = servisMUH.BelgeDurumBilgisi(kriter.yil, muhOdemeEmriMIFForm.belgeNo);
                if (belgeDurum != (int)TNS.MUH.ENUMBelgeDurumu.IPTAL)
                {
                    fler.Add(muhOdemeEmriMIFForm);
                }
            }

            if (fler.Count > 0)
                sonuc = servisMUH.BelgeDurumDegistirODEMIF(kullanan, fler, true);

            return sonuc;

        }
        public Sonuc OdemeEmriIptal(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            return OdemeEmriIptal(kullanan, kriter, true);
        }
        /// <summary>
        /// Arguman olarak verilen, ödeme emri belgesinin durumunu iptal olarak veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OdemeEmriIptal(Kullanici kullanan, OdemeEmriMIFForm kriter, bool sureceBagliOElerideIptalEt)
        {
            //Sonuc sonuc = new Sonuc(true, "İptal edilecek sonuç bulunamadı veya Sürece bağlı belgeler daha önceden iptal edilmiş.", "");
            Sonuc sonuc = new Sonuc(true, "", "");
            TNSCollection fler = new TNSCollection();

            if (sureceBagliOElerideIptalEt)
            {
                MUH.OdemeEmriMIFForm form = new MUH.OdemeEmriMIFForm();

                form.surecYil = kriter.yil;
                form.surecNo = kriter.surecNo;
                form.muhasebe = kriter.muhasebe;
                form.belgeNo = kriter.odemeEmriNo;
                form.formDurum = 22;
                ObjectArray oa = servisMUH.OdemeEmriMIFListele(kullanan, form, false);
                if (oa.sonuc.islemSonuc)
                {
                    foreach (MUH.OdemeEmriMIFForm item in oa.objeler)
                    {
                        item.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.IPTAL;
                        fler.Add(item);
                    }
                }
            }
            sonuc = IptalOdenekGondermeTalebi(kullanan, kriter);

            //HRC OdemeEmri tablosu
            IptalOdemeEmri(kullanan, kriter, fler);

            //Geçici Görev Yolluk
            if (fler.Count == 0)
            {
                IptalGeciciGorevYollukOdemeEmri(kriter, fler);
            }

            //Sürekli Görev Yolluk
            if (fler.Count == 0)
            {
                IptalSurekliGorevYollukOdemeEmri(kriter, fler);
            }

            //Abone Fatura
            if (fler.Count == 0)
            {
                IptalAboneFaturaOdemeEmri(kriter, fler);
            }

            //Çeşitli Ödemeler Bordro
            if (fler.Count == 0)
            {
                IptalCesitliOdemelerOdemeEmri(kriter, fler);
            }

            //Fazla Mesai
            if (fler.Count == 0)
            {
                IptalFazlaMesaiOdemeEmri(kullanan, kriter, fler);
            }

            //Ek Ders
            if (fler.Count == 0)
            {
                IptalEkDersOdemeEmri(kullanan, kriter, fler);
            }

            //Sosyal Yardım
            if (fler.Count == 0)
            {
                IptalSosyalYardimOdemeEmri(kriter, fler);
            }

            //Avans Artığı
            IptalAvansArtigi(kriter, fler);

            if (fler.Count > 0)
            {
                for (int i = 0; i < fler.Count - 1; i++)
                {
                    for (int j = i + 1; j < fler.Count;)
                    {
                        if (AyniMi((MUH.OdemeEmriMIFForm)fler[i], (MUH.OdemeEmriMIFForm)fler[j]))
                        {
                            fler.RemoveAt(j);
                        }
                        else j++;
                    }
                }
                sonuc = servisMUH.BelgeDurumDegistirODEMIF(kullanan, fler, false);
            }
            return sonuc;
        }

        private bool AyniMi(MUH.OdemeEmriMIFForm p, MUH.OdemeEmriMIFForm p_2)
        {
            if (p.yil == p_2.yil && p.belgeNo == p_2.belgeNo)
                return true;
            return false;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, tertiplere ait aylık harcama bilgisini veritabanından okur 
        /// ve AylikDagilim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="kurKod">İşlem yapılacak kurumsal kod bilgisi.</param>
        /// <param name="islemKodListesi">İşlem yapılacak işlem kod listesi.</param>
        /// <returns>İstenilen kriterdeki, tertiplere ait aylık harcama bilgisi listesini AylikDagilim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleAylikDagilim(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, string kurKod, int[] islemKodListesi)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABOdemeEmriForm to = new TABOdemeEmriForm(ve);
            IDataReader r = to.ListeleAylikDagilim(yil, muhasebeKod, birimKod, kurKod, islemKodListesi);

            if (to.Hata() == "")
            {
                Tertip t = new Tertip(DateTime.Now.Year, TNS.BKD.ENUMTabloKod.TABOdenek);
                while (r.Read())
                {
                    AylikDagilim o = new AylikDagilim();

                    o.ay = VeriErisim.DegerAl(r, 0, 0);
                    o.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    o.birim = VeriErisim.DegerAl(r, 2, "");

                    o.tertip.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), VeriErisim.DegerAl(r, 3, ""));
                    o.tertip.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(1), VeriErisim.DegerAl(r, 4, ""));
                    o.tertip.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(2), VeriErisim.DegerAl(r, 5, ""));
                    o.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(3), VeriErisim.DegerAl(r, 6, ""));

                    o.tutar = VeriErisim.DegerAl(r, 7, 0.0);

                    bilgi.objeler.Add(o);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, toplam tertip harcama bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="kurKod">İşlem yapılacak kurumsal kod bilgisi.</param>
        /// <param name="islemKodListesi">İşlem yapılacak işlem kod listesi.</param>
        /// <returns>İstenilen kriterdeki, toplam tertip harcama bilgisini double olarak döner</returns>
        public double ToplamAylikDagilim(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, string kurKod, int[] islemKodListesi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdemeEmriForm to = new TABOdemeEmriForm(ve);
            return to.ToplamAylikDagilim(yil, muhasebeKod, birimKod, kurKod, islemKodListesi);
        }

        /// <summary>
        /// Arguman olarak verilen, ödeme emri belge bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, OdemeEmriMIFForm kriter, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00268, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (kriter.yil == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";
            if (kriter.muhasebe == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";
            if (kriter.birim.Trim() == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";
            if (kriter.surecNo.Trim() == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00067 + "<br>";

            //if (kriter.odemeEmriNo.Trim() == "")//Ödeme emri hazırlanmamış olabilir bu nedenlekaldırıldı. 12.06.2017 Melih Manas
            //{
            //    hata += TNS.HRC.HarcamaSurecleriResI.HRC00231 + "<br>";
            //    if (kriter.prOdemeEmriNo == 0)
            //        hata += TNS.HRC.HarcamaSurecleriResI.HRC00232 + "<br>";
            //}

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            int yil = kriter.yil;
            string surecNo = kriter.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00233;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////
            if (kriter.odemeEmriNo.Trim() != "")
            {
                sonuc = MifIptal(kullanan, kriter);

                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABOdemeEmriForm to = new TABOdemeEmriForm(ve);
            ve.BeginTransaction();

            sonuc = to.SilDetay(kriter);
            if (sonuc.islemSonuc)
                sonuc = to.Sil(kriter);

            if (sonuc.islemSonuc && kriter.odemeEmriNo.Trim() != "")
            {
                sonuc = to.OdemeEmriNoTemizle(kriter.yil, kriter.surecNo, kriter.odemeEmriNo.Trim());
            }

            if (sonuc.islemSonuc)
            {
                //Parca Adım devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                ///////////////////////////////////////////////////////////////////////////

                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00018;
                ve.Commit();

                //Faturadan ödeme emri numarasını iptal edince silmemenin nedeni daha sonra aynı no ile evrak üretmek 
                //
                //TABFaturaForm fatura = new TABFaturaForm(ve);
                //FaturaForm bilgi = new FaturaForm();
                //bilgi.yil = kriter.yil;
                //bilgi.muhasebe = kriter.muhasebe;
                //bilgi.birim = kriter.birim;
                //bilgi.surecNo = kriter.surecNo;
                //bilgi.prFaturaNo = kriter.prFaturaNo;
                //bilgi.odemeEmriNo = "";
                //bilgi.odemeEmriYil = 0;

                //fatura.OdemeEmriNoKaydet(bilgi);
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00019 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        ///// <summary>
        ///// Arguman olarak verilen kriterlere bağlı olarak, ödeme yapılan harcama bilgisini veritabanından okur 
        ///// ve BHMHarcama nesne koleksiyonu olarak döner
        ///// </summary>
        ///// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        ///// <param name="ay">İşlem yapılcak ay bilgisi.</param>
        ///// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        ///// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        ///// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        ///// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        ///// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        ///// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        ///// <param name="peb">İşlem yapılacak alt faaliyet kod bilgisi.</param>
        ///// <returns>İstenilen kriterdeki, ödeme yapılan harcama bilgisi listesini BHMHarcama nesne koleksiyonu olarak tutan nesne</returns>
        //public ObjectArray BHMOdemeHarcamaListesi(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        //{
        //    ObjectArray liste = new ObjectArray();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));

        //    TABOdemeEmriForm to = new TABOdemeEmriForm(ve);
        //    IDataReader r = to.ListeleButceHazirlikOdemeHarcama(yil, ay, muhasebe, birim, kur, fon, fin, eko, peb);

        //    if (to.Hata() == "")
        //    {
        //        while (r.Read())
        //        {
        //            BHMHarcama bilgi = new BHMHarcama();

        //            bilgi.islemYil = VeriErisim.DegerAl(r, 0, 0);
        //            bilgi.islemAy = VeriErisim.DegerAl(r, 1, 0);
        //            bilgi.surecYil = VeriErisim.DegerAl(r, 2, 0);
        //            bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
        //            bilgi.muhasebe = VeriErisim.DegerAl(r, 4, 0);
        //            bilgi.birim = VeriErisim.DegerAl(r, 5, "");
        //            bilgi.kur = VeriErisim.DegerAl(r, 6, "");
        //            bilgi.fon = VeriErisim.DegerAl(r, 7, "");
        //            bilgi.fin = VeriErisim.DegerAl(r, 8, "");
        //            bilgi.eko = VeriErisim.DegerAl(r, 9, "");
        //            bilgi.peb = VeriErisim.DegerAl(r, 10, "");
        //            bilgi.toplamTutar = VeriErisim.DegerAl(r, 11, (decimal)0);
        //            bilgi.yillaraSariIslemMi = Convert.ToBoolean(VeriErisim.DegerAl(r, 12, 0));

        //            if (bilgi.yillaraSariIslemMi)
        //                bilgi.yil = bilgi.surecYil + 1;
        //            else
        //                bilgi.yil = bilgi.surecYil;

        //            liste.objeler.Add(bilgi);
        //        }
        //    }
        //    r.Close();
        //    ve.VeriErisimKapat();
        //    return liste;
        //}

        //public decimal ListeleToplamHarcama(Kullanici kullanan, int yil, string surecNo)
        //{
        //    OdemeEmriMIFForm form = new OdemeEmriMIFForm();
        //    form.yil = yil;
        //    form.surecNo = surecNo;

        //    decimal donusDeger = decimal.Zero;
        //    ObjectArray liste = ListeleMiktar(kullanan, form, "830");
        //    foreach (OdemeEmriMIFDetay detay in liste.objeler)
        //    {
        //        donusDeger += (decimal)detay.alacak;
        //    }

        //    return donusDeger;
        //}

        //public decimal ListeleToplamAvans(Kullanici kullanan, int yil, string surecNo)
        //{
        //    OdemeEmriMIFForm form = new OdemeEmriMIFForm();
        //    form.yil = yil;
        //    form.surecNo = surecNo;

        //    decimal donusDeger = decimal.Zero;
        //    ObjectArray liste = ListeleMiktar(kullanan, form, "16");
        //    foreach (OdemeEmriMIFDetay detay in liste.objeler)
        //    {
        //        donusDeger += (decimal)detay.borc;
        //    }

        //    return donusDeger;
        //}

        private ObjectArray ListeleDetay(Kullanici kullanan, OdemeEmriMIFForm kriter)
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

        public Sonuc IptalOdenekGondermeTalebi(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            Sonuc sonuc = new Sonuc(true, "", TNS.HRC.HarcamaSurecleriResI.HRC00230);
            GondermeTenkisForm form = new GondermeTenkisForm();
            form.belgeTur = ENUMBelgeTuru.ODENEKGONDERME;
            form.yil = kriter.yil;
            form.kurum = OrtakServis.VarsayilanKurumBul();
            form.muhasebe = kriter.muhasebe;
            form.birim = kriter.birim;
            form.talep = "1";

            //KmL
            //ÖEB yılı süreç yılı ile aynı olmayabilir.
            form.surecYil = kriter.yil;
            form.surecNo = kriter.surecNo;

            ObjectArray formListe = servisMUH.GondermeTenkisListele(kullanan, form, 0);
            if (formListe.ObjeSayisi > 0)
            {
                TNSCollection formlar = new TNSCollection();
                foreach (GondermeTenkisForm tenkisForm in formListe.objeler)
                {
                    if (tenkisForm.formDurum == (int)TNS.MUH.ENUMBelgeDurumu.YENITALEP || tenkisForm.formDurum == (int)TNS.MUH.ENUMBelgeDurumu.TALEP)
                    {
                        tenkisForm.formDurum = (int)(TNS.MUH.ENUMBelgeDurumu.TALEPIPTAL);
                        tenkisForm.islemYapan = kullanan.kullaniciKodu;
                        formlar.Add(tenkisForm);
                    }
                }
                if (formlar.Count > 0)
                {
                    return servisMUH.BelgeDurumDegistirGonTen(kullanan, formlar);
                }
            }
            return sonuc;
        }

        private static void ToplaOdemeEmri(TNSCollection fler, int yil, string odemeEmriNo)
        {
            //YAPILACAK:Yıllara sari işlemde ödeme emri yılı süreç yılından farklı olabilir
            TNS.MUH.OdemeEmriMIFForm muhOdemeEmriMIFForm = new TNS.MUH.OdemeEmriMIFForm();
            muhOdemeEmriMIFForm.surecYil = yil;
            muhOdemeEmriMIFForm.belgeNo = odemeEmriNo;
            muhOdemeEmriMIFForm.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.IPTAL;
            muhOdemeEmriMIFForm.kurum = OrtakServis.VarsayilanKurumBul();

            if (muhOdemeEmriMIFForm.belgeNo != "")
            {
                int belgeDurum = servisMUH.BelgeDurumBilgisi(yil, odemeEmriNo);
                if (belgeDurum != (int)TNS.MUH.ENUMBelgeDurumu.IPTAL)
                {
                    //fler.Add(muhOdemeEmriMIFForm); geçen yılın belgesini iptal ediyordu, kaldırdım.
                }
            }
        }

        private void IptalSosyalYardimOdemeEmri(OdemeEmriMIFForm kriter, TNSCollection fler)
        {
            ObjectArray liste = (new HRCSosyalYardim()).ListeleOdemeEmriNo(kriter.yil, kriter.surecNo);
            if (liste.ObjeSayisi > 0)
            {
                foreach (PSLSosyalYardimi oeb in liste.objeler)
                {
                    ToplaOdemeEmri(fler, oeb.yil, oeb.odemeEmriNo);
                }
            }
        }

        private void IptalCesitliOdemelerOdemeEmri(OdemeEmriMIFForm kriter, TNSCollection fler)
        {
            ObjectArray liste = (new HRCCesitliOdemeBordro()).ListeleOdemeEmriNo(kriter.yil, kriter.surecNo);
            if (liste.ObjeSayisi > 0)
            {
                foreach (CesitliOdeme oeb in liste.objeler)
                {
                    ToplaOdemeEmri(fler, oeb.surecYil, oeb.odemeEmriNo);
                }
            }
        }

        private void IptalAboneFaturaOdemeEmri(OdemeEmriMIFForm kriter, TNSCollection fler)
        {
            ObjectArray liste = (new HRCAboneFatura()).ListeleOdemeEmriNo(kriter.yil, kriter.surecNo);
            if (liste.ObjeSayisi > 0)
            {
                foreach (AboneFatura oeb in liste.objeler)
                {
                    ToplaOdemeEmri(fler, oeb.yil, oeb.odemeEmriNo);
                }
            }
        }

        private void IptalOdemeEmri(Kullanici kullanan, OdemeEmriMIFForm kriter, TNSCollection fler)
        {
            //Ödeme Emri
            kriter.belgeTur = (int)ENUMBelgeTuru.ODEMEEMRI;
            ObjectArray odemeEmriList = new HRCOdemeEmriForm().Listele(kullanan, kriter);
            if (odemeEmriList.ObjeSayisi > 0)
            {
                foreach (OdemeEmriMIFForm surecOdemeEmri in odemeEmriList.objeler)
                {
                    ToplaOdemeEmri(fler, kriter.yil, surecOdemeEmri.odemeEmriNo);
                }
            }

            //Mif
            kriter.belgeTur = (int)ENUMBelgeTuru.MIF;
            ObjectArray hrcMifFormlist = Listele(kullanan, kriter);

            if (hrcMifFormlist.ObjeSayisi > 0)
            {
                foreach (OdemeEmriMIFForm surecOdemeEmriMIFForm in hrcMifFormlist.objeler)
                {
                    ToplaOdemeEmri(fler, kriter.yil, surecOdemeEmriMIFForm.odemeEmriNo);
                }
            }

        }

        private static void IptalEkDersOdemeEmri(Kullanici kullanan, OdemeEmriMIFForm kriter, TNSCollection fler)
        {
            Sonuc sonucMIF = new Sonuc(true, "", "");
            EkDersBordro ekDersBordro = new EkDersBordro();
            ekDersBordro.yil = kriter.yil;
            ekDersBordro.surecNo = kriter.surecNo;

            sonucMIF = (new HRCEkDers()).MIFNoListele(kullanan, ekDersBordro);

            if (sonucMIF.islemSonuc)
            {
                ToplaOdemeEmri(fler, kriter.yil, sonucMIF.anahtar);
            }
        }

        private static void IptalFazlaMesaiOdemeEmri(Kullanici kullanan, OdemeEmriMIFForm kriter, TNSCollection fler)
        {
            Sonuc sonucMIF = new Sonuc(true, "", "");
            FazlaMesaiBordro fazlaMesaiBordro = new FazlaMesaiBordro();
            fazlaMesaiBordro.yil = kriter.yil;
            fazlaMesaiBordro.surecNo = kriter.surecNo;

            sonucMIF = (new HRCFazlaMesai()).MIFNoListele(kullanan, fazlaMesaiBordro);

            if (sonucMIF.islemSonuc)
            {
                ToplaOdemeEmri(fler, kriter.yil, sonucMIF.anahtar);
            }

        }

        private static void IptalGeciciGorevYollukOdemeEmri(OdemeEmriMIFForm kriter, TNSCollection fler)
        {
            ObjectArray liste = (new PSLGeciciGorevYolluk()).ListeleOdemeEmriNo(kriter.yil, kriter.surecNo);
            if (liste.ObjeSayisi > 0)
            {
                foreach (GeciciGorevYollukBelge oeb in liste.objeler)
                {
                    ToplaOdemeEmri(fler, oeb.yil, oeb.odemeEmriNo);
                }
            }
        }

        private static void IptalSurekliGorevYollukOdemeEmri(OdemeEmriMIFForm kriter, TNSCollection fler)
        {
            ObjectArray liste = (new PSLSurekliGorevYolluk()).ListeleOdemeEmriNo(kriter.yil, kriter.surecNo);
            if (liste.ObjeSayisi > 0)
            {
                foreach (SurekliGorevYollukBelge oeb in liste.objeler)
                {
                    ToplaOdemeEmri(fler, oeb.yil, oeb.odemeEmriNo);
                }
            }
        }

        private static void IptalAvansArtigi(OdemeEmriMIFForm kriter, TNSCollection fler)
        {
            ObjectArray listeMif = (new HRCAvansArtigi()).ListeleMifNo(kriter.yil, kriter.surecNo);
            if (listeMif.ObjeSayisi > 0)
            {
                foreach (AvansArtigi aa in listeMif.objeler)
                {
                    ToplaOdemeEmri(fler, aa.yil, aa.avansArtigiMif);
                }
            }
        }

        /*
        public ObjectArray ListeleOdemeEmriMuhMif(Kullanici kullanan, OdemeEmriMIFForm kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABOdemeEmriForm to = new TABOdemeEmriForm(ve);
            IDataReader r = null;
            try
            {
                r = to.ListeleOdemeEmriMuhMif(kriter);

                if (to.Hata() == "")
                {
                    while (r.Read())
                    {
                        OdemeEmriMIFForm f = new OdemeEmriMIFForm();

                        f.yil = VeriErisim.DegerAl(r, 0, 0);
                        f.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                        f.birim = VeriErisim.DegerAl(r, 2, "");
                        f.surecNo = VeriErisim.DegerAl(r, 3, "");
                        f.odemeEmriNo = VeriErisim.DegerAl(r, 4, "");

                        bilgi.objeler.Add(f);
                    }
                }

                r.Close();

                ve.VeriErisimKapat();

                return bilgi;
            }
            catch
            {
                //r.Close();
                ve.VeriErisimKapat();
                return bilgi;
            }
        }
        */
    }
}
