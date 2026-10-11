using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.BKD;
using TNS.BHMv2;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Para birimi bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCUAStf
    {
        public Sonuc Kaydet(Kullanici kullanan, SatinAlmaTalepForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            // Tüm kullanıcılar satın alma talep formu üretebileceğinden, sadece muhasebe birim yetkisi kontrolü yapılıyor.
            if (!Yetki.MuhasebeBirimYetkiVarMiSTF(kullanan, bilgi.muhasebe.ToString(), bilgi.birim, ENUMOGY.Yazma, "01"))
            {
                sonuc.hataStr = "Bu birim için STF kaydetmeye yetkiniz bulunmamaktadır!";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = Kontrol(bilgi);

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            if (bilgi.formKod == "")
            {
                bilgi.durum = (int)ENUMSatinalmaTalepFormDurum.YENI;

                //Belgeye numara alınıyor
                //**********************************************************
                TABGenel tg = new TABGenel(ve);

                string sayacKod = bilgi.yil.ToString();
                sayacKod = "HRCSTF" + sayacKod;

                bilgi.formKod = tg.BelgeNoAl(sayacKod, "000000");
                //**********************************************************

                //Eğer belgeno alındı ise aşağıda başına kurum kodu konuluyor
                //sonuc.Çıkışta Sonuç Anahtar alanına konuluyor ki client tarafı yeni kayıt edilen belge numarasını alabilsin
                //**********************************************************
                if (OrtakFonksiyonlar.ConvertToInt(bilgi.formKod, 0) == 0)
                {
                    ve.VeriErisimKapat();
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00259;
                    sonuc.islemSonuc = false;
                    return sonuc;
                }
                //**********************************************************
            }

            TABUAStf to = new TABUAStf(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
                //sonuc.anahtar = bilgi.formKod;
                //if (bilgi.ihtiyacListesi.ObjeSayisi > 0)
                //{
                //    string formKod = bilgi.formKod;
                //    if (string.IsNullOrWhiteSpace(formKod))
                //        formKod = bilgi.formKod;

                // sonuc = to.KaydetBaslik(bilgi.yil, formKod, bilgi.ihtiyacListesi);
                //    if (!sonuc.islemSonuc)
                //    {
                //        sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                //        ve.Rollback();
                //    }
                //}

                sonuc.anahtar = bilgi.formKod;
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

        public Sonuc KaydetTeklif(Kullanici kullanan, int yil, string surecNo, int firmaKod, int tip, TNSCollection bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            // Tüm kullanıcılar satın alma talep formu üretebileceğinden, sadece muhasebe birim yetkisi kontrolü yapılıyor.
            //if (!Yetki.MuhasebeBirimYetkiVarMi(kullanan, bilgi.muhasebe.ToString(), bilgi.birim))
            //{
            //    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            //string hata = Kontrol(bilgi);

            //if (hata != "")
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = hata;
            //    return sonuc;
            //}

            int siraNo = 0;
            if (bilgi.Count > 0)
            {
                SatinAlmaTalepIhtiyacTeklif x = (SatinAlmaTalepIhtiyacTeklif)bilgi[0];
                siraNo = x.siraNo;
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));


            TABUAStf to = new TABUAStf(ve);
            ve.BeginTransaction();

            sonuc = to.KaydetTeklif(yil, surecNo, firmaKod, tip, bilgi, siraNo);

            if (sonuc.islemSonuc)
            {
                ObjectArray teklifListe = new ObjectArray();
                Teklif teklif = new Teklif();
                teklif.yil = yil;
                teklif.surecNo = surecNo;
                teklif.tip = tip;
                teklif.malzeme.malzemeNo = MalzemeNoBul(bilgi);
                teklif.firma.firmaNo = FirmaNoBul(bilgi);
                teklif.birimFiyat = BirimFiyatToplamBul(bilgi);
                teklif.muhasebe = MuhasebeBul(bilgi);
                teklif.birim = BirimBul(bilgi);
                teklifListe.objeler.Add(teklif);
                TABTeklif to2 = new TABTeklif(ve);

                sonuc = to2.Sil(teklif, true);
                if (sonuc.islemSonuc)
                {
                    sonuc = to2.Kaydet(teklifListe);

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
                }
                else
                {
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        //public Sonuc Sil(Kullanici kullanan, SatinAlmaTalepForm bilgi)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    //if (!HRCYetkiKontrol.YoneticiMi(kullanan))
        //    //{
        //    //    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
        //    //    sonuc.islemSonuc = false;
        //    //    return sonuc;
        //    //}

        //    //if (bilgi.kod == 0)
        //    //    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00211;

        //    //if (sonuc.hataStr != "")
        //    //{
        //    //    sonuc.islemSonuc = false;
        //    //    return sonuc;
        //    //}

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABUAStf to = new TABUAStf(ve);

        //    ve.BeginTransaction();

        //    sonuc = to.Sil(bilgi);

        //    if (sonuc.islemSonuc)
        //    {
        //        ve.Commit();
        //        sonuc.bilgiStr = bilgi.formKod + "nolu Satın Alma Talep Formu başarıyla silindi.";
        //    }
        //    else
        //    {
        //        ve.Rollback();
        //        sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00019 + "<br>" + sonuc.hataStr;
        //    }

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        private ObjectVAN Oku(VeriErisim veriErisim, Kullanici kullanan, int yil, string formKod)
        {
            ObjectVAN o = new ObjectVAN();
            SatinAlmaTalepForm form = new SatinAlmaTalepForm();
            form.yil = yil;
            form.formKod = formKod.PadLeft(6, '0');
            if (form.yil == 0)
                o.sonuc.DegerAta("Yılı giriniz");
            if (string.IsNullOrWhiteSpace(formKod))
                o.sonuc.DegerAta("Form kodu giriniz");
            o.o = null;
            if (o.sonuc.islemSonuc)
            {
                ObjectArray liste = Listele(veriErisim, kullanan, form);
                o.sonuc = liste.sonuc;
                if (liste.ObjeSayisi == 1)
                    o.o = liste[0];
            }
            return o;
        }

        public ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, SatinAlmaTalepForm kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            TNS.MUH.ENUMOdenekUstuKontrol ouKontrol = TNS.MUH.Arac.OdenekUstuKontrolBul();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TNS.BHMv2.IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
            bool mpOnayli;

            MUH.ENUMOdenekKontrolTur ogKontrolTur = TNS.MUH.Arac.OdenekKontrolTurBul(kriter.yil);

            TABUAStf to = new TABUAStf(ve);
            Yetki.KullanicininYetkiliOlduguMuhasebeBirim(kullanan, ref kriter.muhasebe, ref kriter.birim);
            IDataReader r = to.Listele(kriter);

            if (r != null)
            {
                while (r.Read())
                {
                    SatinAlmaTalepForm f = new SatinAlmaTalepForm();

                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.formKod = VeriErisim.DegerAl(r, 1, "");
                    f.evrakSayi = VeriErisim.DegerAl(r, 2, "");
                    f.konu = VeriErisim.DegerAl(r, 3, "");
                    f.gerekce = VeriErisim.DegerAl(r, 4, "");
                    f.iseBaslamaTarihi = VeriErisim.DegerAl(r, 5);
                    f.aciklama = VeriErisim.DegerAl(r, 6, "");
                    f.yaklasikMaliyet = VeriErisim.DegerAl(r, 7, decimal.Zero);
                    f.tertipStr = VeriErisim.DegerAl(r, 8, "");
                    f.ihaleTurKod = VeriErisim.DegerAl(r, 9, 0);
                    f.ihaleMaddeKod = VeriErisim.DegerAl(r, 10, 0);
                    f.islemKod = VeriErisim.DegerAl(r, 11, 0);
                    f.durum = VeriErisim.DegerAl(r, 12, 0);
                    f.surecYil = VeriErisim.DegerAl(r, 13, 0);
                    f.surecNo = VeriErisim.DegerAl(r, 14, "");
                    f.muhasebe = VeriErisim.DegerAl(r, 15, 0);
                    f.birim = VeriErisim.DegerAl(r, 16, "");
                    f.kurKod = VeriErisim.DegerAl(r, 17, "");
                    f.alimTuru = (ENUMAlimTuru)VeriErisim.DegerAl(r, 18, (int)0);
                    f.onayaSunanAdSoyad = VeriErisim.DegerAl(r, 19, "");
                    f.onayaSunanUnvan = VeriErisim.DegerAl(r, 20, "");
                    f.onaylayanAdSoyad = VeriErisim.DegerAl(r, 21, "");
                    f.onaylayanUnvan = VeriErisim.DegerAl(r, 22, "");
                    f.olusturulmaTarih = VeriErisim.DegerAl(r, 23);

                    f.islemAd = VeriErisim.DegerAl(r, 25, "");

                    if (f.tertipler != null)
                    {
                        foreach (STFTertipBilgi b in f.tertipler)
                        {
                            string[] tertipler = b.TERTIP.Split('-');
                            OdenekBilgi[] obler = TNS.BHMv2.Arac.TertipListele(kullanan, servisBHMv2, 3, f.yil, f.muhasebe, f.birim
                                , OrtakFonksiyonlar.DiziElemanAl(tertipler, 0, ""), OrtakFonksiyonlar.DiziElemanAl(tertipler, 1, ""), OrtakFonksiyonlar.DiziElemanAl(tertipler, 2, "")
                                , OrtakFonksiyonlar.DiziElemanAl(tertipler, 3, ""), OrtakFonksiyonlar.DiziElemanAl(tertipler, 4, ""), ogKontrolTur, ouKontrol, true, out mpOnayli);
                            if (obler != null && obler.Length == 1)
                            {
                                OdenekBilgi ob = obler[0];
                                b.ODENEK = ob.EkRakam(0) - ob.EkRakam(1) - ob.EkRakam(2);
                                b.AD = ob.tertip.adlar[3];
                            }
                            else
                                b.ODENEK = 0;
                        }
                    }
                    bilgi.objeler.Add(f);
                }
                r.Close();
            }

            foreach (SatinAlmaTalepForm f in bilgi.objeler)
            {
                try
                {
                    f.ihtiyacListesi = ListeleBaslik(ve, kullanan, f.yil, f.formKod, 0, true);
                }
                catch { }
            }

            ve.VeriErisimKapat();
            return bilgi;
        }

        public ObjectArray ListeleBaslik(VeriErisim veriErisim, Kullanici kullanan, int yil, string formKod, int siraNo, bool detayVer)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABUAStf to = new TABUAStf(ve);
            IDataReader r = to.ListeleBaslik(yil, formKod, siraNo);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    SatinAlmaTalepIhtiyacBaslik t = new SatinAlmaTalepIhtiyacBaslik();
                    t.SurecYil = VeriErisim.DegerAl(r, 0, 0);
                    t.FormKod = VeriErisim.DegerAl(r, 1, "");
                    t.MalzemeSiraNo = VeriErisim.DegerAl(r, 2, 0);
                    t.Aciklama = VeriErisim.DegerAl(r, 3, "");
                    bilgi.objeler.Add(t);
                }
                r.Close();

                if (detayVer)
                {
                    foreach (SatinAlmaTalepIhtiyacBaslik t in bilgi.objeler)
                    {
                        t.detay = ListeleDetay(veriErisim, kullanan, t.SurecYil, t.FormKod, t.MalzemeSiraNo);
                    }
                }
            }
            else
                bilgi.sonuc.DegerAta(ve.hata);
            ve.VeriErisimKapat();
            return bilgi;
        }

        public TNSCollection ListeleDetay(VeriErisim veriErisim, Kullanici kullanan, int yil, string formKod, int siraNo)
        {
            TNSCollection bilgi = new TNSCollection();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABUAStf to = new TABUAStf(ve);
            IDataReader r = to.ListeleDetay(yil, formKod, siraNo);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    SatinAlmaTalepIhtiyacDetay t = new SatinAlmaTalepIhtiyacDetay();
                    t.yil = VeriErisim.DegerAl(r, 0, 0);
                    t.formKod = VeriErisim.DegerAl(r, 1, "");
                    t.siraNo = VeriErisim.DegerAl(r, 2, 0);
                    t.detaySatirNo = VeriErisim.DegerAl(r, 3, 0);
                    t.ihtiyacKod = VeriErisim.DegerAl(r, 4, "");
                    t.ihtiyacAd = VeriErisim.DegerAl(r, 5, "");
                    t.miktar1 = VeriErisim.DegerAl(r, 6, (decimal)0);
                    t.olcuBirimiKod1 = VeriErisim.DegerAl(r, 7, 0);
                    t.miktar2 = VeriErisim.DegerAl(r, 8, (decimal)0);
                    t.olcuBirimiKod2 = VeriErisim.DegerAl(r, 9, 0);
                    t.aciklama = VeriErisim.DegerAl(r, 10, "");

                    bilgi.Add(t);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return bilgi;
        }

        public ObjectArray ListeleTeklif(Kullanici kullanan, int yil, string surecNo, int firmaKod, int tip, int siraNo)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABUAStf to = new TABUAStf(ve);
            IDataReader r = to.ListeleTeklif(yil, surecNo, firmaKod, tip, siraNo);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    SatinAlmaTalepIhtiyacTeklif t = new SatinAlmaTalepIhtiyacTeklif();
                    t.yil = VeriErisim.DegerAl(r, 0, 0);
                    t.surecNo = VeriErisim.DegerAl(r, 1, "");
                    t.siraNo = VeriErisim.DegerAl(r, 2, 0);
                    t.detaySatirNo = VeriErisim.DegerAl(r, 3, 0);
                    t.ihtiyacKod = VeriErisim.DegerAl(r, 4, "");
                    t.ihtiyacAd = VeriErisim.DegerAl(r, 5, "");
                    t.miktar1 = VeriErisim.DegerAl(r, 6, (decimal)0);
                    t.olcuBirimiKod1 = VeriErisim.DegerAl(r, 7, 0);
                    t.miktar2 = VeriErisim.DegerAl(r, 8, (decimal)0);
                    t.olcuBirimiKod2 = VeriErisim.DegerAl(r, 9, 0);
                    t.firmaKod = VeriErisim.DegerAl(r, 10, 0);
                    t.birimFiyat = VeriErisim.DegerAl(r, 11, (decimal)0);
                    t.tip = VeriErisim.DegerAl(r, 12, 0);

                    bilgi.objeler.Add(t);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public TNSCollection ListeleDetaySurec(Kullanici kullanan, int yil, string surecNo, int siraNo)
        {
            TNSCollection liste = new TNSCollection();

            var formListe = Listele(null, kullanan, new SatinAlmaTalepForm { yil = yil, surecNo = surecNo });
            if (formListe.ObjeSayisi > 0)
            {
                SatinAlmaTalepForm form = (SatinAlmaTalepForm)formListe.objeler[0];

                if (form.ihtiyacListesi.ObjeSayisi > 0)
                {
                    foreach (SatinAlmaTalepIhtiyacDetay detay in form.ihtiyacListesi.objeler)
                    {
                        if (detay.siraNo == siraNo)
                            liste.Add(detay);
                    }
                }
            }

            return liste;
        }

        public Sonuc DurumGuncelle(Kullanici kullanan, int yil, string formKod, ENUMSatinalmaTalepFormDurum durum)
        {
            Sonuc sonuc = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectVAN o = Oku(ve, kullanan, yil, formKod);
            if (o == null || !o.sonuc.islemSonuc)
            {
                if (o.sonuc.islemSonuc)
                    o.sonuc.DegerAta("Onaylanmak istenilen form bulunamadı!");
                ve.VeriErisimKapat();
                return o.sonuc;
            }
            SatinAlmaTalepForm form = (SatinAlmaTalepForm)o.o;
            string onayKod = "";
            if (durum == ENUMSatinalmaTalepFormDurum.IPTAL || durum == ENUMSatinalmaTalepFormDurum.ARSIV)
                onayKod = "01";
            else if (durum == ENUMSatinalmaTalepFormDurum.MUHASEBEONAYLADI)
                onayKod = "03";
            else if (durum == ENUMSatinalmaTalepFormDurum.SURECBASLADI)
                onayKod = "02";
            else
            {
                if (form.durum == (int)ENUMSatinalmaTalepFormDurum.SURECBASLADI)
                    onayKod = "02";
                else if (form.durum == (int)ENUMSatinalmaTalepFormDurum.MUHASEBEONAYLADI)
                    onayKod = "03";
                else
                    onayKod = "01";
            }

            if (!Yetki.MuhasebeBirimYetkiVarMiSTF(kullanan, form.muhasebe.ToString(), form.birim, ENUMOGY.Onaylama, onayKod))
            {
                ve.VeriErisimKapat();
                sonuc.DegerAta("Verilen belgenin durumunu değiştirmeye yetkiniz bulunmamaktadır!");
                return sonuc;
            }
            if (durum != ENUMSatinalmaTalepFormDurum.IPTAL)
            {
                if (form.ihtiyacListesi.ObjeSayisi == 0)
                    sonuc.DegerAta("İhtiyaç listesi giriniz!");
                else
                {
                    for (int i = 0; i < form.ihtiyacListesi.ObjeSayisi; i++)
                    {
                        SatinAlmaTalepIhtiyacBaslik b = (SatinAlmaTalepIhtiyacBaslik)form.ihtiyacListesi[i];
                        if (b.detay.Count == 0)
                            sonuc.DegerAta("İhtiyaç listesi '" + b.Aciklama + "' için detay bilgi giriniz!");
                    }
                }
            }

            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABUAStf to = new TABUAStf(ve);
            ve.BeginTransaction();

            sonuc = to.DurumGuncelle(yil, formKod, (int)durum);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public Sonuc SurecBilgiGuncelle(Kullanici kullanan, int yil, string formKod, int surecYil, string surecNo)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            //if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            //{
            //    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            //string hata = Kontrol(bilgi); ;


            //if (hata != "")
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = hata;
            //    return sonuc;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABUAStf to = new TABUAStf(ve);
            ve.BeginTransaction();

            sonuc = to.SurecBilgiGuncelle(yil, formKod, surecYil, surecNo);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = formKod + " nolu Satın Alma Talep Formu durumu başarıyla güncellendi.";
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

        public ObjectArray Sorgula(Kullanici kullanan, SatinAlmaTalepFormSorgu kriter, bool SureceBaglanacakAvansliAlimMi, string surecNo)
        {
            ObjectArray bilgi = new ObjectArray();

            // Tüm kullanıcılar satın alma talep formu üretebildiğinden, sorgulamada kullanıcının yetkili olduğu birimi alarak sorgulama yapıyoruz.
            // Böylece yetki kontrolü de yapmış oluyoruz.
            Yetki.KullanicininYetkiliOlduguMuhasebeBirim(kullanan, ref kriter.muhasebe, ref kriter.birim);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABUAStf to = new TABUAStf(ve);
            IDataReader r = to.Sorgula(kriter, SureceBaglanacakAvansliAlimMi, surecNo);
            if (r != null)
            {
                while (r.Read())
                {
                    SatinAlmaTalepForm sr = new SatinAlmaTalepForm();
                    sr.yil = VeriErisim.DegerAl(r, 0, 0);
                    sr.formKod = VeriErisim.DegerAl(r, 1, "");
                    sr.evrakSayi = VeriErisim.DegerAl(r, 2, "");
                    sr.konu = VeriErisim.DegerAl(r, 3, "");
                    sr.gerekce = VeriErisim.DegerAl(r, 4, "");
                    sr.iseBaslamaTarihi = VeriErisim.DegerAl(r, 5);
                    sr.aciklama = VeriErisim.DegerAl(r, 6, "");
                    sr.yaklasikMaliyet = VeriErisim.DegerAl(r, 7, 0);
                    sr.tertipStr = VeriErisim.DegerAl(r, 8, "");
                    sr.ihaleTurKod = VeriErisim.DegerAl(r, 9, 0);
                    sr.ihaleMaddeKod = VeriErisim.DegerAl(r, 10, 0);
                    sr.islemKod = VeriErisim.DegerAl(r, 11, 0);
                    sr.durum = VeriErisim.DegerAl(r, 12, 0);
                    sr.surecYil = VeriErisim.DegerAl(r, 13, 0);
                    sr.surecNo = VeriErisim.DegerAl(r, 14, "");
                    sr.muhasebe = VeriErisim.DegerAl(r, 15, 0);
                    sr.birim = VeriErisim.DegerAl(r, 16, "");
                    sr.kurKod = VeriErisim.DegerAl(r, 17, "");
                    sr.alimTuru = (ENUMAlimTuru)VeriErisim.DegerAl(r, 18, (int)0);
                    sr.onayaSunanAdSoyad = VeriErisim.DegerAl(r, 19, "");
                    sr.onayaSunanUnvan = VeriErisim.DegerAl(r, 20, "");
                    sr.onaylayanAdSoyad = VeriErisim.DegerAl(r, 21, "");
                    sr.onaylayanUnvan = VeriErisim.DegerAl(r, 22, "");
                    sr.olusturulmaTarih = VeriErisim.DegerAl(r, 23);
                    sr.islemAd = VeriErisim.DegerAl(r, 25, "");

                    bilgi.objeler.Add(sr);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return bilgi;
        }

        private string Kontrol(SatinAlmaTalepForm bilgi)
        {
            string hata = string.Empty;
            if (bilgi.muhasebe <= 0 || string.IsNullOrWhiteSpace(bilgi.birim) || string.IsNullOrWhiteSpace(bilgi.kurKod))
                hata = "Lütfen birim seçiniz!" + "<br>";

            if (bilgi.konu == string.Empty)
                hata += "Lütfen konu giriniz!" + "<br>";

            return hata;
        }

        private double BirimFiyatToplamBul(TNSCollection bilgi)
        {
            decimal toplam = 0;
            decimal miktar = 0;
            foreach (SatinAlmaTalepIhtiyacTeklif teklif in bilgi)
            {
                miktar = 0;
                if (teklif.miktar1 > 0)
                    miktar = teklif.miktar1;
                if (teklif.miktar2 > 0)
                    miktar *= teklif.miktar2;
                toplam += teklif.birimFiyat * miktar;
            }

            return OrtakFonksiyonlar.ConvertToDouble(toplam);
        }

        private int FirmaNoBul(TNSCollection bilgi)
        {
            int firmaNo = 0;
            if (bilgi.Count > 0)
            {
                SatinAlmaTalepIhtiyacTeklif teklif = (SatinAlmaTalepIhtiyacTeklif)bilgi[0];
                return teklif.firmaKod;
            }
            return firmaNo;
        }

        private int MalzemeNoBul(TNSCollection bilgi)
        {
            int malzemeNo = 0;
            if (bilgi.Count > 0)
            {
                SatinAlmaTalepIhtiyacTeklif teklif = (SatinAlmaTalepIhtiyacTeklif)bilgi[0];
                return teklif.siraNo;
            }
            return malzemeNo;
        }

        private string BirimBul(TNSCollection bilgi)
        {
            if (bilgi.Count > 0)
            {
                SatinAlmaTalepIhtiyacTeklif teklif = (SatinAlmaTalepIhtiyacTeklif)bilgi[0];
                return teklif.harcamaBirimKod;
            }
            return "";
        }

        private int MuhasebeBul(TNSCollection bilgi)
        {
            if (bilgi.Count > 0)
            {
                SatinAlmaTalepIhtiyacTeklif teklif = (SatinAlmaTalepIhtiyacTeklif)bilgi[0];
                return teklif.muhasebeKod;
            }
            return 0;
        }

        internal string TalepNoBul(int yil, string surecNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABUAStf t = new TABUAStf(ve);
            string tut = t.TalepNoBul(yil, surecNo);

            ve.VeriErisimKapat();
            return tut;
        }

        internal SatinAlmaTalepIhtiyacBaslik IhtiyacBaslikAc(Kullanici kullanan, int yil, string formKod)
        {
            SatinAlmaTalepForm kriter = new SatinAlmaTalepForm();
            kriter.yil = yil;
            kriter.formKod = formKod;
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectArray oa = this.Listele(ve, kullanan, kriter);
            if (!oa.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                throw new Exception(oa.sonuc.hataStr);
            }
            if (oa.ObjeSayisi != 1)
            {
                ve.VeriErisimKapat();
                throw new Exception("1 den farklı sayıda form okundu");
            }
            SatinAlmaTalepForm bilgi = (SatinAlmaTalepForm)oa.objeler[0];

            // Tüm kullanıcılar satın alma talep formu üretebileceğinden, sadece muhasebe birim yetkisi kontrolü yapılıyor.
            if (!Yetki.MuhasebeBirimYetkiVarMi(kullanan, bilgi.muhasebe.ToString(), bilgi.birim))
            {
                ve.VeriErisimKapat();
                throw new Exception(TNS.HRC.HarcamaSurecleriResI.HRC00013);
            }

            SatinAlmaTalepIhtiyacBaslik satib = new SatinAlmaTalepIhtiyacBaslik();
            satib.Aciklama = "YENİ SATIR";

            TABUAStf to = new TABUAStf(ve);

            satib.MalzemeSiraNo = to.MaksIhtiyacSatirNo(yil, formKod) + 1;
            ve.BeginTransaction();
            Sonuc s = to.IhtiyacSatirEkle(yil, formKod, satib.MalzemeSiraNo, satib.Aciklama);

            if (s.islemSonuc)
                s = to.IhtiyacDetayKopyala(yil, formKod, satib.MalzemeSiraNo, satib.MalzemeSiraNo - 1);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            if (s.islemSonuc)
                return satib;
            else
                throw new Exception(s.hataStr);
        }

        internal void IhtiyacSatiriAdDegistir(Kullanici kullanan, int yil, string formKod, int siraNo, string aciklama)
        {
            SatinAlmaTalepForm kriter = new SatinAlmaTalepForm();
            kriter.yil = yil;
            kriter.formKod = formKod;
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectArray oa = this.Listele(ve, kullanan, kriter);
            if (!oa.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                throw new Exception(oa.sonuc.hataStr);
            }
            if (oa.ObjeSayisi != 1)
            {
                ve.VeriErisimKapat();
                throw new Exception("1 den farklı sayıda form okundu");
            }
            SatinAlmaTalepForm bilgi = (SatinAlmaTalepForm)oa.objeler[0];

            // Tüm kullanıcılar satın alma talep formu üretebileceğinden, sadece muhasebe birim yetkisi kontrolü yapılıyor.
            if (!Yetki.MuhasebeBirimYetkiVarMi(kullanan, bilgi.muhasebe.ToString(), bilgi.birim))
            {
                ve.VeriErisimKapat();
                throw new Exception(TNS.HRC.HarcamaSurecleriResI.HRC00013);
            }

            TABUAStf to = new TABUAStf(ve);
            ve.BeginTransaction();
            Sonuc s = to.IhtiyacSatirGuncelle(yil, formKod, siraNo, aciklama);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            if (!s.islemSonuc)
                throw new Exception(s.hataStr);
        }

        internal Sonuc DetayKaydet(Kullanici kullanan, int yil, string formKod, int siraNo, TNSCollection ihtiyacListesi)
        {
            SatinAlmaTalepForm kriter = new SatinAlmaTalepForm();
            kriter.yil = yil;
            kriter.formKod = formKod;
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectArray oa = this.Listele(ve, kullanan, kriter);
            if (!oa.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, oa.sonuc.hataStr, "");
            }

            if (oa.ObjeSayisi != 1)
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, "1 den farklı sayıda form okundu", "");
            }
            SatinAlmaTalepForm bilgi = (SatinAlmaTalepForm)oa.objeler[0];

            // Tüm kullanıcılar satın alma talep formu üretebileceğinden, sadece muhasebe birim yetkisi kontrolü yapılıyor.
            if (!Yetki.MuhasebeBirimYetkiVarMi(kullanan, bilgi.muhasebe.ToString(), bilgi.birim))
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00013, "");
            }

            TABUAStf to = new TABUAStf(ve);
            ve.BeginTransaction();
            Sonuc s = to.SilDetay(yil, formKod, siraNo);
            for (int i = 0; i < ihtiyacListesi.Count && s.islemSonuc; i++)
            {
                s = to.KaydetDetay(yil, formKod, siraNo, i + 1, (SatinAlmaTalepIhtiyacDetay)ihtiyacListesi[i]);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal Sonuc IhtiyacSatiriSil(Kullanici kullanan, int yil, string formKod, int siraNo)
        {
            SatinAlmaTalepForm kriter = new SatinAlmaTalepForm();
            kriter.yil = yil;
            kriter.formKod = formKod;
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectArray oa = this.Listele(ve, kullanan, kriter);
            if (!oa.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, oa.sonuc.hataStr, "");
            }
            if (oa.ObjeSayisi != 1)
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, "1 den farklı sayıda form okundu", "");
            }
            SatinAlmaTalepForm bilgi = (SatinAlmaTalepForm)oa.objeler[0];

            // Tüm kullanıcılar satın alma talep formu üretebileceğinden, sadece muhasebe birim yetkisi kontrolü yapılıyor.
            if (!Yetki.MuhasebeBirimYetkiVarMi(kullanan, bilgi.muhasebe.ToString(), bilgi.birim))
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00013, "");
            }

            TABUAStf to = new TABUAStf(ve);
            ve.BeginTransaction();
            Sonuc s = to.SilDetay(yil, formKod, siraNo);
            if (s.islemSonuc)
                s = to.SilBaslik(yil, formKod, siraNo);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        public ObjectArray AlimlariListele(Kullanici kullanan, HizmetAlimi bilgi)
        {
            ObjectArray hizmetAlimi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));


            TABUAStf to = new TABUAStf(ve);

            IDataReader rdr = to.AlimlariListele(bilgi);

            if (to.Hata() == "")
            {
                while (rdr.Read())
                {
                    HizmetAlimi hbilgi = new HizmetAlimi();
                    hbilgi.SurecYil = VeriErisim.DegerAl(rdr, 16, 0);
                    hbilgi.SurecNo = VeriErisim.DegerAl(rdr, 17, "");
                    hbilgi.Konu = VeriErisim.DegerAl(rdr, 6, "");
                    hbilgi.FormKod = VeriErisim.DegerAl(rdr, 1, "");

                    hizmetAlimi.objeler.Add(hbilgi);
                }
            }

            return hizmetAlimi;
        }

        internal Sonuc TeklifMalzemeDetaySil(VeriErisim veriErisim, Kullanici kullanan, int yil, string surecNo, int firmaKod, int tip, int siraNo)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABUAStf to = new TABUAStf(ve);

            sonuc = to.SilTeklif(yil, surecNo, firmaKod, tip, siraNo);
            if (sonuc.islemSonuc)
            {
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
    }
}
