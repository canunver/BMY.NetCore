//using MUHServisC.ebutce.Muhasebe;
using System;
using System.Data;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Hesap Planı işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHHesapPlaniServis
    {
        bool ebutceAktarim = false;

        /// <summary>
        /// Hesap planı kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="hesapPlani">Hesap planı bilgisi içeren nesne</param>
        /// <returns></returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray hesapPlani)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            string gelenHesapAd = "";
            if (hesapPlani.objeler.Count == 1)
                gelenHesapAd = ((HesapPlaniSatir)hesapPlani.objeler[0]).aciklama;

            if (!ebutceAktarim)
                sonuc = KontrolEt(kullanan, hesapPlani, "Yazma");
            else
                sonuc.islemSonuc = true;

            if (sonuc.islemSonuc)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

                ve.BeginTransaction();
                TABHesapPlani to = new TABHesapPlani(ve);

                //2021 Cumhur veritabanına ulaşılamadığı için yapıldı. Silinecek Melih 09.01.2021
                //if (kullanan.kullaniciKodu == "stratek")
                //{
                //    to.YansitmaGuncelle2021();
                //    to.HesapPlaniGuncelle2021();
                //    ve.Commit();
                //    ve.VeriErisimKapat();
                //    return new Sonuc();
                //}

                sonuc = to.HesapKodKaydet(hesapPlani, ebutceAktarim);

                if (sonuc.islemSonuc)
                {
                    //Gelen listenin anası ile kendisinin detay sayilarının güncelle
                    foreach (HesapPlaniSatir hs in hesapPlani.objeler)
                    {
                        Tertip t = new Tertip(hs.yil, ENUMTabloKod.TABOdemeEmri);

                        string hesapKodAna = HesapKoduAnasiniVer(t, hs.hesapKod);
                        HesapPlaniDetaySayilariDuzenle(ve, hs.yil, hesapKodAna);
                        HesapPlaniDetaySayilariDuzenle(ve, hs.yil, hs.hesapKod);
                    }

                    ve.Commit();

                    string kayitBilgi = ButceMuhasebeResI.MUH066;
                    if (hesapPlani.objeler.Count == 1)
                    {
                        string kayitliHesapAd = ((HesapPlaniSatir)hesapPlani.objeler[0]).aciklama;
                        if (kayitliHesapAd != gelenHesapAd)
                            kayitBilgi += "<br><br><b>UYARI:<b> Bu hesap kodu ödeme belgelerinde kullanıldığı için ismi değiştirilemez";
                    }

                    sonuc.bilgiStr = kayitBilgi;
                }
                else
                    ve.Rollback();

                ve.VeriErisimKapat();
            }
            else
                OrtakFonksiyonlar.HataStrYaz("Hesap planları excelden aktarılırken hata oluştu: < br > " + sonuc.hataStr);

            return sonuc;
        }

        /// <summary>
        /// Hesap planı aktar
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kaynakYil">Kaynak yıl</param>
        /// <param name="hedefYil">Hedef yıl</param>
        /// <returns></returns>
        public Sonuc Aktar(Kullanici kullanan, int kaynakYil, int hedefYil)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABHesapPlani to = new TABHesapPlani(ve);
            sonuc = to.HesapKodAktar(kaynakYil, hedefYil);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = string.Format(ButceMuhasebeResI.MUH067, hedefYil);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Hesaps plani sil
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="hesapPlani">Hesap planı bilgisi içeren nesne</param>
        /// <returns></returns>
        public Sonuc Sil(Kullanici kullanan, ObjectArray hesapPlani)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH068, "");

            sonuc = KontrolEt(kullanan, hesapPlani, "Silme");

            if (sonuc.islemSonuc)
            {
                foreach (HesapPlaniSatir hs in hesapPlani.objeler)
                {
                    OrtakFonksiyonlar.HataStrYaz(kullanan.kullaniciKodu + " kişisi " + hs.hesapKod + " siliyor");
                }

                VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

                ve.BeginTransaction();
                TABHesapPlani to = new TABHesapPlani(ve);
                sonuc = to.HesapKodSil(hesapPlani);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = ButceMuhasebeResI.MUH069;
                }
                else
                    ve.Rollback();

                ve.VeriErisimKapat();
            }
            else
                OrtakFonksiyonlar.HataStrYaz("Hesap planı silinirken hata: < br > " + sonuc.hataStr);

            return sonuc;
        }

        /// <summary>
        /// Hesap planı listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="hesKod">Hesap planı bilgisi içeren nesne</param>
        /// <returns></returns>
        public ObjectArray Listele(Kullanici kullanan, HesapPlaniSatir hesKod)
        {
            ObjectArray hVAN = new ObjectArray();
            string hata = "";

            if (hesKod.yil <= 0)
                hata += ButceMuhasebeResI.MUH070;

            if (hata != "")
            {
                hVAN.sonuc.hataStr = hata;
                hVAN.sonuc.islemSonuc = false;
                return hVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABHesapPlani to = new TABHesapPlani(ve);
            IDataReader r = to.HesapKodListele(hesKod);

            if (to.Hata() == "")
            {
                Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);

                while (r.Read())
                {
                    HesapPlaniSatir hKod = new HesapPlaniSatir();

                    hKod.yil = VeriErisim.DegerAl(r, 0, 0);
                    hKod.hesapKod = VeriErisim.DegerAl(r, 1, "");
                    hKod.aciklama = VeriErisim.DegerAl(r, 2, "");
                    hKod.seviye = VeriErisim.DegerAl(r, 3, 0);
                    hKod.koruma = VeriErisim.DegerAl(r, 4, 0);
                    hKod.bakiye = VeriErisim.DegerAl(r, 5, 0);
                    hKod.tertip = VeriErisim.DegerAl(r, 6, 0);
                    hKod.kesinti = VeriErisim.DegerAl(r, 7, 0);
                    hKod.nitelik = VeriErisim.DegerAl(r, 8, "");
                    hKod.grup = VeriErisim.DegerAl(r, 9, "");
                    hKod.detay = VeriErisim.DegerAl(r, 10, 0) == 0;
                    hKod.altHesapKod = VeriErisim.DegerAl(r, 11, 0);
                    hKod.dovizKurKod = VeriErisim.DegerAl(r, 12, "");
                    hKod.dovizKurTurSatis = VeriErisim.DegerAl(r, 13, "");
                    hKod.dovizKurTurAlis = VeriErisim.DegerAl(r, 14, "");
                    hKod.kurFarkiAlacakHesabi = VeriErisim.DegerAl(r, 15, "");
                    hKod.kurFarkiBorcHesabi = VeriErisim.DegerAl(r, 16, "");
                    hKod.ekoBasla = VeriErisim.DegerAl(r, 17, -1);

                    if (hKod.nitelik.Trim().StartsWith(";"))
                        hKod.nitelik = hKod.nitelik.Trim().Substring(1, hKod.nitelik.Trim().Length - 1);
                    if (hKod.nitelik.Trim().EndsWith(";"))
                        hKod.nitelik = hKod.nitelik.Trim().Substring(0, hKod.nitelik.Trim().Length - 1);

                    hKod.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), hKod.hesapKod);

                    hVAN.objeler.Add(hKod);
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            return hVAN;
        }

        Sonuc KontrolEt(Kullanici kullanan, ObjectArray hesapPlani, string yetkiTipi)
        {
            string hata = "";
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH071, "");
            Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);

            bool yetki = false;

            bool sistemYoneticisi = false;
            bool muhasebeYoneticisi = false;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) sistemYoneticisi = true;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MUHASEBEYONETICISI)) muhasebeYoneticisi = true;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            if (yetkiTipi == "Yazma")
                yetki = MUH.IsKurallari.MUHYetkiServis.HesapPlaniYazmaYetkisiVarMi(kullanan);
            else if (yetkiTipi == "Silme")
                yetki = MUH.IsKurallari.MUHYetkiServis.HesapPlaniSilmeYetkisiVarMi(kullanan);

            TABHesapPlani th = new TABHesapPlani(ve);

            if (hesapPlani != null && yetkiTipi == "Yazma")
            {
                foreach (HesapPlaniSatir hs in hesapPlani.objeler)
                {
                    if (hs.yil <= 0)
                        hata += ButceMuhasebeResI.MUH072;
                    if (hs.hesapKod == "")
                        hata += ButceMuhasebeResI.MUH073 + "<br>";
                    if (hs.aciklama == "")
                        hata += hs.hesapKod + " - " + ButceMuhasebeResI.MUH074 + "<br>";

                    //Excel den liste halinde kayıt yapıldığında anasıcocugundan dolayı kayıt yapılamıyor
                    //Kayıt esnasında bakmak lazım

                    if (hs.hesapKod.Length > 2)
                    {
                        string[] h = hs.hesapKod.Split(' ');
                        hs.hesapKod = h[0].Trim();

                        if (!OrtakFonksiyonlar.KodYapiKontrol(t.Kirilimlar(0), hs.hesapKod.Replace(".", ""), false).islemSonuc)
                            hata += string.Format(ButceMuhasebeResI.MUH075, hs.hesapKod, t.Kirilimlar(0)) + "<br>";
                    }

                    if (th.HesapKodAnasiCocuguVarMi(hs.yil, hs.hesapKod, true) && !(muhasebeYoneticisi || sistemYoneticisi))
                        hata += "<br>" + string.Format(ButceMuhasebeResI.MUH199, hs.hesapKod);
                    else
                    {
                        MUHOdemeEmriMIFServis ode = new MUHOdemeEmriMIFServis();
                        OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
                        oEmri.yil = hs.yil;
                        oEmri.formDurum = 22;
                        OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(hs.yil);
                        oEmriDetay.hesapKod = hs.hesapKod;//detay kayıt olduğu için % işle aramaya gerek yok çok yavaşlıyor
                        oEmriDetay.kur = "";
                        oEmriDetay.fon = "";
                        oEmriDetay.fin = "";
                        oEmriDetay.eko = "";
                        oEmri.detay.objeler.Add(oEmriDetay);

                        ObjectArray olar = ode.CokBelgeGetir(kullanan, oEmri, false, false);
                        bool butceYoneticisi = kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.BUTCEYONETICISI);
                        if (TNS.MUH.Arac.VarsayilanKurumKod().Replace(".", "") == "1399")
                        {
                            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                                butceYoneticisi = true;
                        }

                        if (olar.objeler.Count > 0 && !butceYoneticisi)
                        {
                            //Hesap Adını eskisi ile eşitle değişmesin diğer bilgiler değişebilir
                            HesapPlaniSatir hOrj = new HesapPlaniSatir() { yil = hs.yil, hesapKod = hs.hesapKod };
                            hOrj = th.HesapKodOzellikAl(hOrj);
                            hs.aciklama = hOrj.aciklama;
                        }
                    }

                    //Bir üst hesap ödeme emrinde kullanılmış mı?

                    string hesapKodAna = HesapKoduAnasiniVer(t, hs.hesapKod);
                    if (hesapKodAna.Replace(".", "") != hs.hesapKod.Replace(".", ""))//Hesap kodu en üst kırılımsa kontrol etme
                    {
                        OdemeEmriMIFForm oEmriAna = new OdemeEmriMIFForm();
                        oEmriAna.yil = hs.yil;
                        oEmriAna.formDurum = 22;
                        OdemeEmriMIFDetay oEmriDetayAna = new OdemeEmriMIFDetay(hs.yil);
                        oEmriDetayAna.hesapKod = hesapKodAna + "|";//detay kayıt olduğu için % işle aramaya gerek yok çok yavaşlıyor
                        oEmriDetayAna.kur = "";
                        oEmriDetayAna.fon = "";
                        oEmriDetayAna.fin = "";
                        oEmriDetayAna.eko = "";
                        oEmriAna.detay.objeler.Add(oEmriDetayAna);

                        ObjectArray olarAna = new MUHOdemeEmriMIFServis().CokBelgeGetir(kullanan, oEmriAna, false, false);
                        if (olarAna.objeler.Count > 0)
                        {
                            hata += "<br>" + string.Format("{0} nolu hesap kodu belgelerde kullanıldığı için alt hesap açılamaz. İlk önce bu kayıtlar iptal durumuna getirin", hesapKodAna);
                        }
                    }
                }
            }
            else if (hesapPlani != null && yetkiTipi == "Silme")
            {
                foreach (HesapPlaniSatir hs in hesapPlani.objeler)
                {
                    if (th.HesapKodAnasiCocuguVarMi(hs.yil, hs.hesapKod, true) && !(muhasebeYoneticisi || sistemYoneticisi))
                        hata += "<br>" + string.Format(ButceMuhasebeResI.MUH076, hs.hesapKod);
                    else
                    {
                        MUHOdemeEmriMIFServis ode = new MUHOdemeEmriMIFServis();
                        OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
                        oEmri.yil = hs.yil;
                        oEmri.formDurum = 22;
                        OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(hs.yil);
                        oEmriDetay.hesapKod = hs.hesapKod;
                        oEmriDetay.kur = "";
                        oEmriDetay.fon = "";
                        oEmriDetay.fin = "";
                        oEmriDetay.eko = "";
                        oEmri.detay.objeler.Add(oEmriDetay);

                        ObjectArray olar = ode.CokBelgeGetir(kullanan, oEmri, false, false);
                        if (olar.objeler.Count > 0)
                        {
                            hata += "<br>" + string.Format(ButceMuhasebeResI.MUH077, hs.hesapKod);
                            break;
                        }
                    }
                }
            }

            if (hata == "")
                sonuc.islemSonuc = true;

            sonuc.hataStr = hata;

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Hesap kodunun özelliklerini al
        /// </summary>
        /// <param name="hKod">Hesap kodu</param>
        /// <returns></returns>
        public HesapPlaniSatir OzellikAl(VeriErisim veriErisim, HesapPlaniSatir hKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            TABHesapPlani to = new TABHesapPlani(ve);

            hKod = to.HesapKodOzellikAl(hKod);

            ve.VeriErisimKapat();

            return hKod;
        }

        /// <summary>
        /// Hesap planı yansıtma listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="hesapPlani">Hesap planı bilgisi içeren nesne</param>
        /// <returns></returns>
        public ObjectArray YansitmaListele(Kullanici kullanan, ObjectArray hesapPlani)
        {
            ObjectArray hVAN = new ObjectArray();
            string hata = "";

            foreach (HesapPlaniSatir hesKod in hesapPlani.objeler)
            {
                if (hesKod.yil <= 0)
                    hata = ButceMuhasebeResI.MUH070;
                if (hata != "")
                    break;
            }

            if (hata != "")
            {
                hVAN.sonuc.hataStr = hata;
                hVAN.sonuc.islemSonuc = false;
                return hVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABHesapPlani to = new TABHesapPlani(ve);
            IDataReader r = to.YansitmaListele(hesapPlani);

            if (to.Hata() == "")
            {
                Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);

                while (r.Read())
                {
                    HesapPlaniYansitma hKod = new HesapPlaniYansitma();

                    hKod.hesapKod = VeriErisim.DegerAl(r, 1, "");
                    hKod.yansitmaHesapKod = VeriErisim.DegerAl(r, 2, "");
                    hKod.satir = VeriErisim.DegerAl(r, 3, 0);
                    hKod.hKodAciklama = VeriErisim.DegerAl(r, 4, "");
                    hKod.yKodAciklama = VeriErisim.DegerAl(r, 5, "");

                    hKod.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), hKod.hesapKod);
                    hKod.yansitmaHesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), hKod.yansitmaHesapKod);

                    hVAN.objeler.Add(hKod);
                }

                r.Close();
            }

            ve.VeriErisimKapat();

            return hVAN;
        }

        /// <summary>
        /// Hesap planı detay sayıları düzenle
        /// </summary>
        /// <param name="ve">The ve.</param>
        /// <param name="yil">Yıl</param>
        public void HesapPlaniDetaySayilariDuzenle(VeriErisim ve, int yil)
        {
            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            ve.BeginTransaction();

            TABHesapPlani to = new TABHesapPlani(ve);
            to.HesapPlaniDetaySayilariDuzenle(yil, 1);
            to.HesapPlaniDetaySayilariDuzenle(yil, 2);
            to.HesapPlaniDetaySayilariDuzenle(yil, 3);
            to.HesapPlaniDetaySayilariDuzenle(yil, 4);
            to.HesapPlaniDetaySayilariDuzenle(yil, 5);
            to.HesapPlaniDetaySayilariDuzenle(yil, 6);
            to.HesapPlaniDetaySayilariDuzenle(yil, 7);
            to.HesapPlaniDetaySayilariDuzenle(yil, 8);
            to.HesapPlaniDetaySayilariDuzenle(yil, 0);

            for (int i = 0; i < 10; i++)
            {
                to.HesapPlaniDetaySayilariDuzenle(yil, i.ToString());
            }

            ve.Commit();

            ve.VeriErisimKapat();
        }

        /// <summary>
        /// Hesaps the plani detay sayilari duzenle.
        /// </summary>
        /// <param name="ve">The ve.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="hesapKod">The hesap kod.</param>
        public void HesapPlaniDetaySayilariDuzenle(VeriErisim ve, int yil, string hesapKod)
        {
            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            ve.BeginTransaction();
            TABHesapPlani to = new TABHesapPlani(ve);


            Tertip t = new Tertip(yil, ENUMTabloKod.TABOdemeEmri);
            hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), hesapKod.Replace(".", ""));
            string[] kodlar = hesapKod.Split('.');
            int seviye = kodlar.Length;
            if (seviye == 0) return;
            hesapKod = hesapKod.Replace(".", "");

            to.HesapPlaniDetaySayilariDuzenle(yil, seviye);
            if (hesapKod.Length > 3)
                to.HesapPlaniDetaySayilariDuzenle(yil, hesapKod.Substring(0, hesapKod.Length - 2));

            ve.Commit();
            ve.VeriErisimKapat();
        }

        ///// <summary>
        ///// Hesap planı ebutceden aktar
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="yil">Yıl</param>
        ///// <param name="kurum">Kurum kodu</param>
        ///// <param name="hesapKod">Hesap kodu</param>
        ///// <returns></returns>
        //public Sonuc EbutcedenAktar(Kullanici kullanan, string yil, string kurum, string hesapKod)
        //{
        //    Sonuc sonuc = new Sonuc("BÜMKO Muhasebe Hesap Planı aktarımını kapattığından hesap planı aktarımı yapılamamaktadır!");
        //    if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
        //        sonuc.DegerAta("Bu işlemi sistem yöneticileri yapabilirler!");
        //    if (!sonuc.islemSonuc)
        //        return sonuc;
        //    //System.Net.ServicePointManager.CertificatePolicy = new CertificatePolicy();
        //    MUHServisC.ebutce.Muhasebe.Muhasebe mEbutce = new MUHServisC.ebutce.Muhasebe.Muhasebe();
        //    mEbutce.Proxy = OrtakFonksiyonlar.ProxyAyarla();

        //    ebutceAktarim = true;

        //    ObjectArray hesler = new ObjectArray();
        //    HesapPlaniW hW = mEbutce.HesapPlaniListele(yil, kurum, hesapKod);
        //    for (int i = 0; i < hW.hesapPlani.Length; i++)
        //    {
        //        HesapPlaniSatir hes = new HesapPlaniSatir();
        //        hes.aciklama = hW.hesapPlani[i].aciklama.Replace("'", "").Replace('"', ' ');
        //        hes.bakiye = hW.hesapPlani[i].bakiye;
        //        hes.detay = hW.hesapPlani[i].detay;
        //        hes.grup = hW.hesapPlani[i].grup;
        //        hes.hesapKod = hW.hesapPlani[i].hesapKod;
        //        hes.kesinti = hW.hesapPlani[i].kesinti;
        //        hes.koruma = hW.hesapPlani[i].koruma;
        //        hes.nitelik = hW.hesapPlani[i].nitelik;
        //        hes.seviye = hW.hesapPlani[i].seviye;
        //        hes.tertip = hW.hesapPlani[i].tertip;
        //        hes.yil = hW.hesapPlani[i].yil;

        //        hesler.objeler.Add(hes);
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
        //    if (hesler.ObjeSayisi == 0)
        //    {
        //        sonuc.DegerAta("EBütçeden veri alınamadı!");
        //    }

        //    if (sonuc.islemSonuc)
        //    {
        //        ve.BeginTransaction();
        //        TABHesapPlani to = new TABHesapPlani(ve);
        //        if (hesapKod == "")
        //            sonuc = to.SilHepsini(OrtakFonksiyonlar.ConvertToInt(yil, 0));

        //        if (sonuc.islemSonuc)
        //            sonuc = Kaydet(kullanan, hesler);

        //        if (sonuc.islemSonuc)
        //        {
        //            ve.Commit();
        //            HesapPlaniDetaySayilariDuzenle(ve, OrtakFonksiyonlar.ConvertToInt(yil, 0));

        //            sonuc.bilgiStr = ButceMuhasebeResI.MUH078;
        //        }
        //        else
        //            ve.Rollback();
        //    }
        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        private string HesapKoduAnasiniVer(Tertip t, string hesapKod)
        {
            //Bir üst hesap ödeme emrinde kullanılmış mı?
            string hesapKodAna = hesapKod.Replace(".", "");
            hesapKodAna = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), hesapKodAna);
            int kirilimSonNoktaYer = hesapKodAna.LastIndexOf('.');
            if (kirilimSonNoktaYer > 0)
                hesapKodAna = hesapKodAna.Substring(0, kirilimSonNoktaYer);

            return hesapKodAna;
        }

        /// <summary>
        /// Hesap plani nitelik değiştir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="hesapPlaniSatir">Hesap planı bilgisi içeren nesne</param>
        /// <returns></returns>
        public Sonuc NitelikDegistir(Kullanici kullanan, HesapPlaniSatir hesapPlaniSatir, int nitelikIslemi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABHesapPlani to = new TABHesapPlani(ve);

            string[] nitelikler = hesapPlaniSatir.nitelik.Split(';');
            if (nitelikIslemi == 1)
            {
                hesapPlaniSatir.nitelik = "";
            }

            sonuc = to.NitelikDegistir(hesapPlaniSatir);

            if (sonuc.islemSonuc)
            {
                if (nitelikIslemi == 1)
                {
                    foreach (string n in nitelikler)
                    {
                        if (string.IsNullOrEmpty(n)) continue;
                        sonuc = to.NitelikKaldir(hesapPlaniSatir, ";" + n + ";");
                    }
                }

                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}
