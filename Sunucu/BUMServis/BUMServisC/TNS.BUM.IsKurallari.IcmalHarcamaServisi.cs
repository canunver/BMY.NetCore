// Static Model
using System.Data;
using TNS.Veri;
using TNS.BUM.IsKurallari.Veri;
using TNS.BKD;
using TNS.KYM;
using System;
using TNS.BUM;


namespace TNS.BUM.IsKurallari
{
    /// <summary>
    /// Bütçe Uygulama İcmal Harcama Servisi Sınıfı
    /// </summary>
    public class IcmalHarcamaServisi
    {

        /// <summary>
        /// Parametre olarak verilen Kullanıcı Yıl, Aşama, Tertip bilgilerini kullanarak,
        /// İcmal Harcama Programı bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, İcmal Harcama Programı bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        public static ObjectArray IHPListele(VeriErisim veriErisim, Kullanici kullanan, int yil, int asama, Tertip tertip)
        {
            ObjectArray donusDeger = new ObjectArray();
            string kurum;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));

            kurum = (string)tertip.kodlar[0];

            if (kullanan.kullaniciKodu == "")
            {
                donusDeger.sonuc.hataStr = ButceUygulamaResI.BUM228;
                donusDeger.sonuc.islemSonuc = false;
            }
            else if (BUMYetkiServisi.IHPKurumGormeYetkisiVarMi(kullanan, yil, kurum, asama))
            {

                if (donusDeger.sonuc.islemSonuc)
                {
                    TABButce toplamKbo = new TABButceGider(ve);
                    TABISLIHP aylikIHP = new TABISLIHP(ve);

                    IBKDServis servisBKD = BUMServis.TanimlaBKD();
                    ABSKodlarVAN akv = new ABSKodlarVAN();
                    akv = servisBKD.KodListeleSeviye(yil, "4", tertip.kodlar[1], 2, true);

                    for (int i = 0; i < akv.absKodlar.Count; i++)
                    {
                        ABSKod ak = (ABSKod)akv.absKodlar[i];

                        TertipBilgi ta = new TertipBilgi();
                        ta.tertip = new Tertip(yil, ENUMTabloKod.TABGiderIcmal, kurum, ak.kod);
                        ta.tertip.adlar[1] = ak.Ad();

                        Tertip t = new Tertip(yil, ENUMTabloKod.TABButce, kurum, "", "", ak.kod);
                        ta.kbo = toplamKbo.KBOBul(yil, t.kodlar);
                        ta.serbest = toplamKbo.SerbestBul(yil, t.kodlar);
                        IDataReader drAyliklar = aylikIHP.IHPAylikOdenek(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.IHP), asama, ak.kod, ENUMFormDurumGrup.HEPSI);
                        bool girdi = false;
                        if (drAyliklar != null)
                        {
                            while (drAyliklar.Read())
                            {
                                girdi = true;
                                int ay = VeriErisim.DegerAl(drAyliklar, 0, 0);
                                if (ay >= 1 && ay <= 12)
                                    ta.ayDegerler[ay - 1] = VeriErisim.DegerAl(drAyliklar, 1, 0.0);
                                else
                                {
                                    donusDeger.sonuc.hataStr = ButceUygulamaResI.BUM229;
                                    donusDeger.sonuc.islemSonuc = false;
                                    break;
                                }
                            }
                            drAyliklar.Close();
                        }
                        else
                        {
                            donusDeger.sonuc.hataStr = ButceUygulamaResI.BUM230+":\r\n" + ve.hata;
                            donusDeger.sonuc.islemSonuc = false;
                        }

                        if (!donusDeger.sonuc.islemSonuc)
                            break;
                        if (girdi || ta.kbo > 0)
                            donusDeger.Ekle(ta);
                    }
                }
            }
            else
            {
                donusDeger.sonuc.hataStr = kurum + " "+ButceUygulamaResI.BUM231+":\r\n";
                donusDeger.sonuc.islemSonuc = false;
            }

            ve.VeriErisimKapat();

            return donusDeger;

        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama, Tertip bilgilerine göre,
        /// İcmal Harcama Programı KBÖ ve Serbest bilgilerini toplamını hesaplayarak listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, İcmal Harcama Programı bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        public static ObjectArray IHPListeleToplam(VeriErisim veriErisim, Kullanici kullanan, int yil, int asama, Tertip tertip)
        {
            ObjectArray donusDeger = new ObjectArray();
            string kurum;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));

            kurum = (string)tertip.kodlar[0];

            if (kullanan == null)
            {
                donusDeger.sonuc.DegerAta(ButceUygulamaResI.BUM232, "");
            }
            else if (BUMYetkiServisi.IHPKurumGormeYetkisiVarMi(kullanan, yil, kurum, asama))
            {
                TABButce toplamKbo = new TABButceGider(ve);
                TABISLIHP aylikIHP = new TABISLIHP(ve);

                TertipBilgi ta = new TertipBilgi();
                string[] giderKodlar ={ tertip.kodlar[0], "", "", tertip.kodlar[1] };
                ta.kbo = toplamKbo.KBOBul(yil, giderKodlar);
                ta.serbest = toplamKbo.SerbestBul(yil, giderKodlar);
                IDataReader drAyliklar = aylikIHP.IHPAylikOdenek(yil, tertip.kodlar[0], BISabitler.IslemKod(ENUMIslemKod.IHP), asama, tertip.kodlar[1], ENUMFormDurumGrup.HEPSI);
                bool girdi = false;
                if (drAyliklar != null)
                {
                    while (drAyliklar.Read())
                    {
                        girdi = true;
                        int ay = VeriErisim.DegerAl(drAyliklar, 0, 0);
                        if (ay >= 1 && ay <= 12)
                            ta.ayDegerler[ay - 1] = VeriErisim.DegerAl(drAyliklar, 1, 0.0);
                        else
                        {
                            donusDeger.sonuc.DegerAta(ButceUygulamaResI.BUM233, "");
                            break;
                        }
                    }
                    drAyliklar.Close();
                }
                else
                {
                    donusDeger.sonuc.DegerAta(ButceUygulamaResI.BUM234+":\r\n" + ve.hata, "");
                }

                if (donusDeger.sonuc.islemSonuc)
                    if (girdi || ta.kbo > 0)
                        donusDeger.Ekle(ta);
            }
            else
                donusDeger.sonuc.DegerAta(kurum + " "+ButceUygulamaResI.BUM235+":\r\n", "");

            ve.VeriErisimKapat();

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama, İcmal Satırları bilgilerine göre,
        /// İcmal Harcama Programını kaydeden fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="icmalSatirlari">İcmal satırlarını tutan, TNSCollection türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, kaydetme işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc IHPKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, int asama, TNSCollection icmalSatirlari)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (icmalSatirlari == null)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ButceUygulamaResI.BUM236;
                return sonuc;
            }
            if (icmalSatirlari.Count == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ButceUygulamaResI.BUM237;
                return sonuc;
            }

            string kurum = ((TertipBilgi)icmalSatirlari[0]).tertip.kodlar[0];

            if (!TNS.BKD.Arac.KurumMu(kurum))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = kurum + " "+ButceUygulamaResI.BUM238;
                return sonuc;
            }

            if (kullanan == null || kullanan.kullaniciKodu == "")
            {
                sonuc.hataStr = ButceUygulamaResI.BUM239;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (!BUMYetkiServisi.YazmaYetkisiVarMi(kullanan, yil, kurum, asama))
            {
                sonuc.hataStr = ButceUygulamaResI.BUM240;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));

            TABISLIHP tabIHP = new TABISLIHP(ve);
            TABISLForm tabForm = new TABISLForm(ve);
            TABButce tabButceGider = new TABButceGider(ve);
            Tertip t = new Tertip(yil, ENUMTabloKod.TABButce, kurum, "", "", "");

            if ((sonuc.kayitSay = (int)ButceIslemServisi.ButceIslemiOnayliMi(ve, yil, kurum, BISabitler.IslemKod(ENUMIslemKod.IHP), asama)) == (int)ENUMFormDurumGrup.ONAYLI)
            {
                sonuc.hataStr = ButceUygulamaResI.BUM241;
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();

            TNSDateTime tarih = new TNSDateTime(DateTime.Now);
            if (sonuc.kayitSay == (int)ENUMFormDurumGrup.HATALI)
                sonuc = tabForm.Kaydet(yil, kurum, (string)BISabitler.IslemKod(ENUMIslemKod.IHP), asama, tarih, "", "", "", (int)ENUMKayitDurumu.YENI, "", 0, 1, kullanan.kullaniciKodu, tarih, "", -1);
            else
                sonuc = tabForm.Guncelle(yil, kurum, (string)BISabitler.IslemKod(ENUMIslemKod.IHP), asama, tarih, "", "", "", (int)ENUMKayitDurumu.YENI, "", 0, 1, kullanan.kullaniciKodu, tarih, "", -1);

            for (int i = 0; i < icmalSatirlari.Count && sonuc.islemSonuc; i++)
            {
                TertipBilgi ta = (TertipBilgi)icmalSatirlari[i];

                string icKurum = ta.tertip.kodlar[0];
                string icEko = ta.tertip.kodlar[1];

                if (icKurum != kurum)
                {
                    sonuc.hataStr = ButceUygulamaResI.BUM242+" (" + icKurum + ", " + kurum + ")";
                    sonuc.islemSonuc = false;
                }

                if (String.IsNullOrEmpty(icEko) || icEko.Length != 4)
                {
                    sonuc.hataStr = ButceUygulamaResI.BUM243+" (" + icEko + ")";
                    sonuc.islemSonuc = false;
                }
                if (sonuc.islemSonuc)
                {
                    t.kodlar[3] = icEko;
                    double kbo = tabButceGider.KBOBul(yil, t.kodlar);

                    double toplamKbo = 0.0;

                    for (int j = 0; j < 12; j++)
                        toplamKbo += (double)ta.ayDegerler[j];

                    if ((toplamKbo > kbo))
                    {
                        sonuc.hataStr = ButceUygulamaResI.BUM244;
                        sonuc.islemSonuc = false;
                    }
                }
                if (sonuc.islemSonuc)
                {
                    sonuc = tabIHP.IHPSil(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.IHP), asama, icEko);

                    if (sonuc.islemSonuc)
                    {
                        for (int k = 0; k < 12; k++)
                        {
                            if (ta.ayDegerler[k] != 0.0)
                            {
                                sonuc = tabIHP.IHPKaydet(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.IHP), asama, icEko, k + 1, (double)ta.ayDegerler[k], (int)ENUMKayitDurumu.YENI);
                                if (!sonuc.islemSonuc)
                                    break;
                            }
                        }
                    }
                }
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        
        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Tertip bilgilerine göre
        /// Harcama bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasTertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Harcama bilgisini tutan Object türünde nesne döndürür</returns>
        public static Object HarcamaListeleAy(VeriErisim veriErisim, Kullanici kullanan, int yil, Tertip kistasTertip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABButce toplamKbo = new TABButceGider(ve);
            TABHarcamaAyIci aylikHarcama = new TABHarcamaAyIci(ve);

            TertipBilgi tb = new TertipBilgi();

            for (int i = 0; i < 12; i++)
                tb.ayDegerler[i] = 0.0;
            tb.kbo = toplamKbo.KBOBul(yil, kistasTertip.kodlar);

            IDataReader drAyliklar = aylikHarcama.HarcamaAylikOdenek(yil, kistasTertip.kodlar);

            if (drAyliklar != null)
            {
                while (drAyliklar.Read())
                {
                    int ay = VeriErisim.DegerAl(drAyliklar, 0, 0);
                    if (ay >= 1 && ay <= 12)
                        tb.ayDegerler[ay - 1] = VeriErisim.DegerAl(drAyliklar, 1, 0.0);
                }

                drAyliklar.Close();
            }


            ve.VeriErisimKapat();
            return tb;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama ve Tertip bilgilerine göre,
        /// İcmal Harcama Programı bilgisini okuyan fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasTertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, İcmal Harcama Programı bilgisini tutan Object türünde nesne döndürür</returns>
        public static Object IHPOku(VeriErisim veriErisim, Kullanici kullanan, int yil, int asama, Tertip kistasTertip)
        {
            string kurum = (string)kistasTertip.kodlar[0];

            if (kullanan.kullaniciKodu == "")
            {
                return new Sonuc(false, ButceUygulamaResI.BUM245, "");
            }
            else if (BUMYetkiServisi.IHPKurumGormeYetkisiVarMi(kullanan, yil, kurum, asama))
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                TABButce tab = new TABButceGider(ve); ;
                TABISLIHP tahp = new TABISLIHP(ve);

                TertipBilgi tb = new TertipBilgi();
                Tertip t = new Tertip(yil, ENUMTabloKod.TABButce, kistasTertip.kodlar[0], "", "", kistasTertip.kodlar[1]);
                tb.kbo = tab.KBOBul(yil, t.kodlar);

                for (int j = 0; j < 12; j++)
                    tb.ayDegerler[j] = 0;
                Sonuc sonuc = new Sonuc(true, "", "");
                IDataReader degerler = tahp.IHPAylikOdenek(yil, kistasTertip.kodlar[0], BISabitler.IslemKod(ENUMIslemKod.IHP), asama, kistasTertip.kodlar[1], ENUMFormDurumGrup.HEPSI);
                if (degerler != null)
                {
                    while (degerler.Read())
                    {
                        int ay = VeriErisim.DegerAl(degerler, 0, 0);
                        if (ay >= 1 && ay <= 12)
                            tb.ayDegerler[ay - 1] = VeriErisim.DegerAl(degerler, 1, 0.0);
                        else
                        {
                            sonuc.hataStr = ButceUygulamaResI.BUM246;
                            sonuc.islemSonuc = false;
                            break;
                        }
                    }
                    degerler.Close();
                    ve.VeriErisimKapat();
                    if (sonuc.islemSonuc)
                        return tb;
                    else
                        return sonuc;
                }
                else
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ButceUygulamaResI.BUM247+":\r\n" + ve.hata;
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }
            else
            {
                return new Sonuc(false, ButceUygulamaResI.BUM248, "");
            }
        }


        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama ve Kurum Birim bilgilerine göre,
        /// İcmal Harcama Programını Onaylayan fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurumBirim">Kurum Birim bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, onaylama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc IHPOnayla(VeriErisim veriErisim, Kullanici kullanan, int yil, int asama, string kurumBirim)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = TNS.BKD.Arac.KurumsaldanKuruma(kurumBirim, tUzBul.Kirilimlar(0));

            if (!BUMYetkiServisi.IHPOnaylamaYetkisiVarMi(kullanan, yil, kurumBirim, asama))
            {
                sonuc.DegerAta(ButceUygulamaResI.BUM249, "");
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));


            string saklaKurum = kurum;


            TABISLForm form = new TABISLForm(ve);
            TABISLAPGelir gelir = new TABISLAPGelir(ve);
            TABISLAPFinansman finans = new TABISLAPFinansman(ve);
            TABISLIHP icmal = new TABISLIHP(ve);

            int onayKod;
            if (asama == 3)
                onayKod = (int)ENUMFormDurum.ONAYLI;
            else
                onayKod = (int)ENUMFormDurum.BUTCEISLEMIPTAL;

            ve.BeginTransaction();
            //if (sonuc.islemSonuc && !BIForm.GenelButceliMi(saklaKurum))
            //    sonuc = HazineYardimlariniYansit(ve, yil, kurum, asama);
            if (sonuc.islemSonuc)
                sonuc = form.OnayKaydet(yil, kurumBirim, BISabitler.IslemKod(ENUMIslemKod.IHP), asama, onayKod, -1, kullanan.kullaniciKodu, new TNSDateTime(DateTime.Now));
            if (sonuc.islemSonuc)
                sonuc = icmal.OnayKaydet(yil, kurumBirim, BISabitler.IslemKod(ENUMIslemKod.IHP), asama, onayKod);
            if (sonuc.islemSonuc)
                sonuc = gelir.OnayKaydet(yil, kurumBirim, BISabitler.IslemKod(ENUMIslemKod.IHP), asama, onayKod);
            if (sonuc.islemSonuc)
                sonuc = finans.OnayKaydet(yil, kurumBirim, BISabitler.IslemKod(ENUMIslemKod.IHP), asama, onayKod);
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }


        /// <summary>
        /// Parametre olarak verilen Yıl ve Tertip bilgilerine göre,
        /// Harcama Programının Son Durumuna ilişkin bilgileri döndüren fonksiyon.
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Harcama bilgilerine ilişkin Tertiplerin tutulduğu ObjectArray türünde nesne döndürür</returns>
        public static ObjectArray HPSonDurumGoster(VeriErisim veriErisim, int yil, Tertip tertip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));
            ObjectArray ahv = new ObjectArray();
            TABISLIHP tabAHP = new TABISLIHP(ve);
            IDataReader degerler = tabAHP.HPTertipDurum(yil, tertip.kodlar);
            if (degerler != null)
            {
                TertipBilgi tb = new TertipBilgi();
                tb.ayDegerler = new double[12];

                for (int j = 0; j < 12; j++)
                    tb.ayDegerler[j] = 0;

                while (degerler.Read())
                {
                    int ay = VeriErisim.DegerAl(degerler, 0, 0);
                    if (ay >= 1 && ay <= 12)
                        tb.ayDegerler[ay - 1] = VeriErisim.DegerAl(degerler, 1, 0.0);
                    else
                    {
                        ahv.sonuc.hataStr = ButceUygulamaResI.BUM250;
                        break;
                    }
                }
                degerler.Close();

                if (ahv.sonuc.hataStr == "")
                    ahv.Ekle(tb);
            }
            else
                ahv.sonuc.DegerAta(ButceUygulamaResI.BUM251+":\r\n" + ve.hata, "");

            ve.VeriErisimKapat();
            return ahv;
        }

        /*

        /// <summary>
        ///  VeriErisim nesnesi gelmediği için, verierişim olarak null geçirilecektir.		
        ///  ISLIHP Tablosundan verilen yil, kurum, asama ve ekonomik kodun birinci 
        ///  seviyesindeki ekonomik kodlar ve bunların aylık değerleri TNSCollection içerisinde 12 adet double 
        ///  değer olarak döndürür.				
        ///	 Eğer yetki yoksa veya hata oluşursa, sadece Sonuc yapısında bir nesne Collection'a eklenir.	
        ///	 Collection döndürülür.  
        /// </summary>
        /// <param name="kullanan">Yetki kontrolu için kullanıcının bilgileri alınır.</param>
        /// <param name="veriErisim">Veri tabanına bağlanmak için kullanılır. NULL ise yeni bağlantı kurulur, değilse var olan kullanılır.</param>
        /// <param name="yil">Icmal Rakamlarını listelemek için gerekli yıl bilgisi.</param>		
        /// <param name="asama">Icmali bulmak için gerekli sırano bilgisi.</param>
        /// <param name="tertip">Icmalin listelenebilmesi için gerekli kurum kodunu,
        ///  ekonomik kodu içinde tutan Tertip tipindeki collection.</param>
        /// <returns>İşlem başarılı olmuşsa ObjectArray collectionunda degerler döner. 
        /// Hata oluşmuşsa Sonuc yapısı döndürülür.</returns>			
		
        public ObjectArray IHPListeleAy(VeriErisim veriErisim,Kullanici kullanan, int yil, int asama,  Tertip tertip)
        {
            ObjectArray donusDeger=new ObjectArray();
            string kurum;
            string ekoKod;
	
            VeriErisim ve=VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABButce toplamKbo=new TABButce(ve);

            kurum=(string)tertip.kodlar[0];
            ekoKod= (string)tertip.kodlar[3];
			
                if (kullanan.kullaniciKodu=="")
                {
                    donusDeger.sonuc.hataStr="Kullanıcı alanı boş geliyor. ";
                    donusDeger.sonuc.islemSonuc=false;
                }
                else if (!TNS.BKD.Arac.KurumMu(kurum))
                {
                    donusDeger.sonuc.hataStr=kurum + " bir kurum kodu değil!";
                    donusDeger.sonuc.islemSonuc=false;
                }
                else if (BUMYetkiServisi.IHPKurumGormeYetkisiVarMi(kullanan, yil,  kurum,asama))				
                {
                    if (donusDeger.sonuc.islemSonuc)
                    {
                        TABISLIHP aylikIHP=new TABISLIHP(ve);
                        TertipBilgi ta=new TertipBilgi();
                        ta.ayDegerler=new TNSCollection();
                        ta.serbest=toplamKbo.SerbestBul(yil,kurum,"","",ekoKod);

                        for(int i=0;i<12;i++)
                            ta.ayDegerler.Add((double)0.0);

                        IDataReader drAyliklar=aylikIHP.IHPAylikOdenek(yil,kurum,BISabitler.IslemKod[(int)ENUMIslemKod.IHP],asama,ekoKod);
						
                        if (drAyliklar!=null)
                        {
                            while(drAyliklar.Read())
                            {							
                                int ay=VeriErisim.DegerAl(drAyliklar,0,0);
								
                                if(ay>=1 && ay<=12)
                                    ta.ayDegerler[ay-1]=VeriErisim.DegerAl(drAyliklar,1,0.0);
                                else
                                {
                                    donusDeger.sonuc.hataStr="Veritabanından yanlış ay geldi";
                                    donusDeger.sonuc.islemSonuc=false;
                                    break;
                                }
                            }													
                        }
                        else
                        {
                            donusDeger.sonuc.hataStr="Veritabanından okuma yapılamadı:\r\n"+ve.hata;
                            donusDeger.sonuc.islemSonuc=false;
                        }

                        drAyliklar.Close();	

                        ta.kbo=toplamKbo.kboBul(yil,kurum,"","",ekoKod);

                        if (donusDeger.sonuc.islemSonuc)
                            donusDeger.IHPListe.Add(ta);	
                    }

                }		
            else 
            {
                donusDeger.sonuc.hataStr=kurum +"için işlem yapma yetkiniz yoktur:\r\n";
                donusDeger.sonuc.islemSonuc=false;
            }

            ve.VeriErisimKapat();
			
            return donusDeger;
        }
        /// <summary>
        /// Icmal onayı kaldırılırken kullanıcının yetkisi kontrol edilir.
        /// ISLFORM,ISLIHP,ISLGELIRAP,ISLFINANSAP tablosundaki FormDurum alanında yer alan "onaylı" değeri "yenikayıt" değeri ile değiştirilir.		
        /// </summary>
        /// <param name="veriErisim">Veri tabanına bağlanmak için kullanılır. NULL ise yeni bağlantı kurulur, değilse var olan kullanılır.</param>
        /// <param name="kullanan">Yetki kontrolu için kullanıcının bilgileri alınır.</param>
        /// <param name="yil">Icmal onayını kaldırmak için gerekli yıl bilgisi.</param>
        /// <param name="asama">Icmali onayı için gerekli sırano bilgisi.</param>
        /// <param name="tertip">Icmal için gerekli kurum kodunu, ekonomik kodu içinde tutan Tertip tipindeki collection.</param>
        /// <returns>İşlemin sonucu (başarılı olmuş yada hata oluşmuş) Sonuc yapısında döndürülür.</returns>
        public Sonuc IHPOnayKaldir(VeriErisim veriErisim, Kullanici kullanan, int yil, int asama, Tertip tertip)
        {
            Sonuc sonuc=new Sonuc();	

            string kurum=(string)tertip.kodlar[0];

            if (kullanan.kullaniciKodu=="")
            {
                sonuc.hataStr="Kullanıcı alanı boş geliyor. ";
                sonuc.islemSonuc=false;
            }
            else if (!TNS.BKD.Arac.KurumMu(kurum))
            {
                sonuc.hataStr=kurum + " bir kurum kodu değil!";
                sonuc.islemSonuc=false;
            }
            else 
            {
                kurum= kurum.ToString().Replace(".","");
				
                if (BUMYetkiServisi.IHPOnaylamaYetkisiVarMi(kullanan,yil,kurum,asama))
                {					
                    VeriErisim ve=VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                    TABISLForm onayliMi= new TABISLForm(ve);
                    TABISLGELIRAP gelir= new TABISLGELIRAP(ve);
                    TABISLFINANSAP finans= new TABISLFINANSAP(ve);
                    TABISLIHP icmal= new TABISLIHP(ve);

                    ve.BeginTransaction();
                    int onayKod=(int)ENUMFormDurum.YENIKAYIT;
                    if (sonuc.islemSonuc)
                        sonuc=onayliMi.OnayKaldir(yil, kurum, BISabitler.IslemKod[(int)ENUMIslemKod.IHP], asama, onayKod, kullanan.kullaniciTipi.kod, kullanan.kullaniciKodu, new TNSDateTime( DateTime.Now));	
                    if (sonuc.islemSonuc)
                        sonuc=icmal.OnayKaydet(yil, kurum, BISabitler.IslemKod[(int)ENUMIslemKod.AHP], asama, onayKod);
                    if (sonuc.islemSonuc)
                        sonuc=gelir.OnayKaydet(yil, kurum, BISabitler.IslemKod[(int)ENUMIslemKod.AHP], asama, onayKod);	
                    if (sonuc.islemSonuc)
                        sonuc=finans.OnayKaydet(yil, kurum, BISabitler.IslemKod[(int)ENUMIslemKod.AHP], asama, onayKod);
                    if (sonuc.islemSonuc)
                        ve.Commit();
                    else
                        ve.Rollback();
                    ve.VeriErisimKapat();
                }
                else
                {
                    sonuc.hataStr="Onay Kaldırmaya yetkiniz yok";
                    sonuc.islemSonuc=false;
                }
            }
            return sonuc;
        }

        /// <summary>
        /// Kurumun hazine yardımı alan tertipleri varsa TNScollection da listelenir.
        /// TNScollection içersinde iki eleman oluşacaktır.
        /// Birinci eleman cari hazine yardımı gider tertibi, ikinci eleman içersinde de sermaye hazine yardımı gider tertibi listelenir.
        /// Cari ve Sermaye için girilen döngüde gelir hazine yardımı tertiplerinin aylık değerleri bulunur.
        /// Bulunan değerler gider hazine yardımı tertiplerine atanır.
        /// Atamadan önce silinme işlemi yapılır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanına bağlanmak için kullanılır. NULL ise yeni bağlantı kurulur, değilse var olan kullanılır.</param>
        /// <param name="yil">Hazine Yardımını yansıtmak için gerekli yıl bilgisi.</param>
        /// <param name="kurum">Hazine yardımının yansıtılacağı kuruma ait bilgi.</param>
        /// <param name="asama">Hazine yardımının yansıtmak için gerekli sırano bilgisi.</param>
        /// <returns>İşlemin sonucu (başarılı olmuş yada hata oluşmuş) Sonuc yapısında döndürülür.</returns>
        private Sonuc HazineYardimlariniYansit(VeriErisim veriErisim, int yil, string kurum, int asama)
        {
            VeriErisim ve=VeriErisim.VeriErisimBul(null);
            Sonuc sonuc=new Sonuc(true, "", "");
            IBHMServis bhmServis=ButceIslemServisi.BHMServisAl();
            TNSCollection hazineYardimlari=bhmServis.HazineYardimiTertipleriBul(kurum);
            string []GelirHYKodlari={"04.2.1.02","04.2.2.02"};
            double []rakamlar=new double[12];
            TABISLGELIRAP islGelirAP=new TABISLGELIRAP(ve);
            TABISLGIDERAHP islGiderAHP=new TABISLGIDERAHP(veriErisim);

            for (int i=0; i<hazineYardimlari.Count; i++)
            {
                Tertip t=(Tertip) hazineYardimlari[i];
                if(t.kodlar.Count==4)
                {
                    t.kodlar[3]=((string)t.kodlar[3]).Substring(0,4);
                
                    TNSCollection kodlar=new TNSCollection();
                    kodlar.Add(kurum);
                    kodlar.Add(GelirHYKodlari[i]);

                    sonuc=islGiderAHP.AHPSil(yil, TNS.BKD.Arac.KurumsaldanKuruma((string)t.kodlar[0]), BISabitler.IslemKod[(int)ENUMIslemKod.AHP], asama, t.kodlar);
                    if(sonuc.islemSonuc)
                    {
                        IDataReader dr=islGelirAP.AGPAylikOdenek(yil, kurum, BISabitler.IslemKod[(int)ENUMIslemKod.AHP], asama, kodlar);
                        if(dr!=null)
                        {
                            while(dr.Read() && sonuc.islemSonuc)
                            {
                                sonuc=islGiderAHP.AHPKaydet(yil, TNS.BKD.Arac.KurumsaldanKuruma((string)t.kodlar[0]), BISabitler.IslemKod[(int)ENUMIslemKod.AHP], asama, t.kodlar, VeriErisim.DegerAl(dr, 0, (int)0), VeriErisim.DegerAl(dr, 1, 0.0), (int)ENUMFormDurum.YENIKAYIT);
                            }
                            dr.Close();
                        }
                        else
                        {
                            sonuc.hataStr=ve.hata;
                            sonuc.islemSonuc=false;
                        }
                    }
                    if(sonuc.islemSonuc)
                    {
                        sonuc=DetaydanIcmalHesapla(veriErisim, yil, TNS.BKD.Arac.KurumsaldanKuruma((string)t.kodlar[0]), asama, (string)t.kodlar[3]);
                    }
                    if(!sonuc.islemSonuc)
                        break;
                }
            }

            return sonuc;
        }

        /// <summary>
        /// ISLGIDERAHP den verilen yil, kurum, aşama ve ekonomik koda ait aylık değerler bulunur.
        /// Bulunan aylık değerler ISLIHP tablosuna atanır.
        /// Atamadan önce silinme işlemi yapılır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanına bağlanmak için kullanılır. NULL ise yeni bağlantı kurulur, değilse var olan kullanılır.</param>
        /// <param name="yil">Detaydan Icmal hesaplanması için gerekli yıl bilgisi.</param>
        /// <param name="kurum">Detaydan Icmal hesaplanması için kuruma ait bilgi.</param>
        /// <param name="asama">Detaydan Icmal hesaplanması için gerekli sırano bilgisi.</param>
        /// <param name="ekoKod">Detaydan Icmal hesaplanması için gerekli ekonomik kod bilgisi.</param>
        /// <returns>İşlemin sonucu (başarılı olmuş yada hata oluşmuş) Sonuc yapısında döndürülür.</returns>
        private Sonuc DetaydanIcmalHesapla(VeriErisim veriErisim, int yil, string kurum, int asama, string ekoKod)
        {
            Sonuc sonuc=new Sonuc(true, "", "");
            TNSCollection kodlar=new TNSCollection();

            kodlar.Add(kurum);
            kodlar.Add("");
            kodlar.Add("");
            kodlar.Add(ekoKod);
            TABISLGIDERAHP islGiderAHP=new TABISLGIDERAHP(veriErisim);

            IDataReader dr=islGiderAHP.AHPAylikOdenek(yil, kurum, BISabitler.IslemKod[(int)ENUMIslemKod.AHP], asama, kodlar);
            if(dr!=null)
            {
                double []dizi=new double[12];
                for(int j=0; j<12; j++)
                    dizi[j]=0;
                while(dr.Read())
                {
                    int ay=VeriErisim.DegerAl(dr, 0, -1);
                    if(ay>=1 && ay<=12)
                        dizi[ay-1]=VeriErisim.DegerAl(dr, 1, 0.0);
                }
                dr.Close();
                TABISLIHP islIHP=new TABISLIHP(veriErisim);
                sonuc=islIHP.IHPSil(yil, kurum, BISabitler.IslemKod[(int)ENUMIslemKod.IHP], asama, ekoKod); 
                for(int j=0; j<12 && sonuc.islemSonuc; j++)
                {
                    if(dizi[j]!=0)
                    {
                        sonuc=islIHP.IHPKaydet(yil, kurum, BISabitler.IslemKod[(int)ENUMIslemKod.IHP], asama, ekoKod, j+1, dizi[j], (int)ENUMFormDurum.YENIKAYIT); 
                    }
                }
            }
            else
            {
                sonuc.hataStr="Veri tabanından okuma yapılamadı!\r\n"+ veriErisim.hata;
                sonuc.islemSonuc=false;
            }
            return sonuc;
        }		

        /// <summary>
        /// Ekonomik kodların listesi 2. kırılımda elde edilir. (birinci kırılım parametre olarak gelebilir veya boş olabilir.)	
        /// Ekonomik kod "01" ve "02" için ISLIHP tablosundaki aylık değerler bulunur.
        /// Butce tablosundan o kuruma ait serbest rakamlar toplamı bulunur.
        /// Elde edilen toplam ISLIHP tablosundaki aylık değerlerin ilk üç aylık toplamından küçük yada eşit olmalıdır.	
        /// </summary>
        /// <param name="veriErisim">Veri tabanına bağlanmak için kullanılır. NULL ise yeni bağlantı kurulur, değilse var olan kullanılır.</param>
        /// <param name="yil">Serbest Kontrolü için gerekli yıl bilgisi.</param>
        /// <param name="kurum">Serbest Kontrolü için kuruma ait bilgi.</param>
        /// <param name="asama">Serbest Kontrolü için gerekli sırano bilgisi.</param>
        /// <returns>İşlemin sonucu (başarılı olmuş yada hata oluşmuş) Sonuc yapısında döndürülür.</returns>
        private Sonuc SerbestKontrol(VeriErisim veriErisim, int yil, string kurum, int asama)
        {
            Sonuc sonuc=new Sonuc();
            VeriErisim ve=VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
          
            TABISLIHP tabIHP=new TABISLIHP(ve);
            TABButce tabButce=new TABButce(ve);
            IBHMServis servisBHM=TanimlaBHM(); 

            ABSKodlarVAN akv=servisBHM.KodListeleSeviye(yil,"4","",2,true);
            for(int i=0; i<akv.absKodlar.Count;i++)
            {//5
                if(sonuc.islemSonuc)
                {
                    ABSKod ak=(ABSKod)akv.absKodlar[i];
                    TNSCollection  ayDegerlerIHP=new TNSCollection();
                    for(int j=0;j<12;j++)                   
                        ayDegerlerIHP.Add((double)0.0);
                    
                                        
                    if(!( ak.kod.StartsWith("01") || ak.kod.StartsWith("02") ))
                    {
                        IDataReader IHPdegerler=tabIHP.IHPAylikOdenek(yil,kurum,BISabitler.IslemKod[(int)ENUMIslemKod.IHP],asama,ak.kod);
            
                        if (IHPdegerler!=null)
                        {
                            while(IHPdegerler.Read())
                            {							
                                int ay=VeriErisim.DegerAl(IHPdegerler,0,0);
                                if(ay>=1 && ay<=12)
                                    ayDegerlerIHP[ay-1]=VeriErisim.DegerAl(IHPdegerler,1,0.0);                                
                            }
                                
                        }
                        else
                            sonuc.hataStr="Veritabanından okuma yapılamadı:\r\n"+ve.hata;
                            
                        IHPdegerler.Close();
                        double toplamIHP=0.0;
                        if(sonuc.islemSonuc)                              
                        {
                            double toplamSerbest=0.0;
                            toplamIHP=0.0;
                            for(int j=0;j<3;j++)
                                toplamIHP+=(double)ayDegerlerIHP[j];
                            toplamSerbest=tabButce.SerbestBul(yil, kurum, "", "", ak.kod);
                            if (toplamIHP<toplamSerbest)
                            {
                                sonuc.islemSonuc=false;
                                sonuc.hataStr=ak.kod + " Ekonomik kod için ilk 3 Aylık IHP Toplamınız ilk 3 aylık Serbest toplamından büyük olmalıdır. Lütfen Kontrol Ediniz. İlk 3 aylık Serbest Toplamınız:"+ toplamSerbest.ToString();
                                break;
                            }
                        } 
                    }
                }
            }
            return sonuc;
        }
        /// <summary>
        ///VeriErisim nesnesi gelmediği için, verierişim olarak null geçirilecektir. 
        ///Kullanıcının yetkisi olup olmadığı kontrol edilir.	
        ///*IHP taslak oluşturma işlemi yapılır. Kullanıcının yetkisine ve seçtiği kriterlere göre taslak oluşturulur. 
        ///*Yetkisi olmayan kurum ve aşama için oluşturulmaz. Ödenek üstü olan tertipler, son aylardan düşme yapılır ve
        ///ödenek üstü durumu ortadan kaldırılır ve bilgi için kullanıcıya döndürülmek üzere dönecek değere eklenir.
        ///*Eğer oluşturulacak ekonomik kodun önceki yıllar değeri yoksa, bu yılın da oluşturulamayacaktır. Bu durumdaki tertiplerde
        ///kullanıcıya bilgi olarak döndürülmek üzere, dönen değere eklenir.
        ///Taslak Kontrolsuz fonksiyonu * ile belirtilen işlemleri yapmak için kullanılır.
        ///Eğer hak yoksa, sonuc yapısında bir değişken oluşturulur ve TNSCOlleciton'a eklenir.
        ///Dönen değer kullanılırken, Collection'daki ilk elemanın tipi kontrol edilmelidir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanına bağlanmak için kullanılır. NULL ise yeni bağlantı kurulur, değilse var olan kullanılır.</param>
        /// <param name="kullanan">Yetki kontrolu için kullanıcının bilgileri alınır.</param>
        /// <param name="yil">Icmali otomatik oluşturmak için gerekli yıl bilgisi.</param>
        /// <param name="asama">Icmali otomatik oluşturmak için gerekli sırano bilgisi.</param>		
        /// <param name="tertip">Icmali otomatik oluşturmak için gerekli kurum kodunu, ekonomik kodu içinde tutan Tertip tipindeki collection.</param>
        /// <param name="yontem">Icmali hangi değerleri kullanarak hesaplama yapacağımızı belirten bilgi. </param>
        /// <returns>İşlem başarılı olmuşsa TertiplerVAN collectionunda degerler döner. Hata oluşmuşsa Sonuc yapısı döndürülür.</returns> 						
        public TertiplerVAN IHPOlustur(VeriErisim veriErisim,Kullanici kullanan, int yil,int asama,Tertip tertip,int yontem)
        {
            TertiplerVAN donusDeger = new TertiplerVAN();	
            VeriErisim ve=VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));			
				
            string kurum=(string)tertip.kodlar[0];
            string ekoKod=tertip.kodlar[3].ToString().Replace(".","");
		
            if(kullanan.kullaniciKodu=="")
            {	
                donusDeger.sonuc.hataStr="Kullanıcı alanı bos geliyor.";
                donusDeger.sonuc.islemSonuc=false;
            }
            else
            if(!TNS.BKD.Arac.KurumMu(kurum))
            {
                donusDeger.sonuc.hataStr=kurum + " bir kurum degil.";
                donusDeger.sonuc.islemSonuc=false;
            }
            else 
            {
                kurum= kurum.ToString().Replace(".","");
                int tKod=kullanan.kullaniciTipi.kod;				
                if (BUMYetkiServisi.IHPYazmaYetkisiVarMi(kullanan,yil,kurum,asama))
                    if (yontem!=7)
                        donusDeger= TaslakKontrolsuz(ve, yil, asama,  tertip,(int) ENUMFormDurum.YENIKAYIT, yontem);
                    else                    
                        donusDeger.sonuc= TaslakKontrolsuzAhpToplam(ve,yil,asama,kurum,ekoKod);  
            }

            if (donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.bilgiStr="Taslak işlemi başarıyla oluşturulmuştur." ;
                ve.Commit ();
            }
            else
            {
                ve.Rollback();
                donusDeger.sonuc.bilgiStr="İşleminiz başarısız olmuştur.\r\n	";
            }

            ve.VeriErisimKapat();
			
            return donusDeger;

        }
	
        /// <summary>		
        /// Icmal rakamları(ISLIHP) ile ahp rakamlarını(ISLGIDERAHP) karşılastırarak 
        /// ahp deki rakamlar icmale kopyalanır. 	
        /// Kopyalama yapılabilmesi için ISLIHP tablosunda o ekonomik kod için değer olmaması gerekir.		
        /// Yapılan işlem yeni kayıt olarak ISLFORM tablosuna da kayıt eklenir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanına bağlanmak için kullanılır. NULL ise yeni bağlantı kurulur, değilse var olan kullanılır.</param>
        /// <param name="yil">Ahp den İcmale kopyalama yapabilmek için gerekli yıl bilgisi.</param>
        /// <param name="asama">Ahp den İcmale kopyalama yapabilmek için gerekli sırano bilgisi.</param>		
        /// <param name="kurum">Ahp den İcmale kopyalama yapabilmek için gerekli kurum bilgisi.</param>
        /// <param name="ekoKod">Ahp den İcmale kopyalama yapabilmek için gerekli ekonomik kod bilgisi.</param>		
        /// <returns>İşlemin sonucu (başarılı olmuş yada hata oluşmuş) Sonuc yapısında döndürülür.</returns>        
        private Sonuc TaslakKontrolsuzAhpToplam(VeriErisim veriErisim, int yil, int asama,  string kurum,string ekoKod)
        {   
            Sonuc sonuc=new Sonuc();
            VeriErisim ve=VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim veyaz=VeriErisim.VeriErisimBul(null);
            TertiplerVAN donusDeger=new TertiplerVAN();
            TABISLGIDERAHP  tabAhp=new TABISLGIDERAHP(ve);

            TABISLIHP tabIHP=new TABISLIHP(veyaz);
            TABISLForm tabForm=new TABISLForm(veyaz);
            if ((tabIHP.IHPKontrol(yil,kurum,BISabitler.IslemKod[(int)ENUMIslemKod.IHP],asama,ekoKod)!=0.0))
            {							
                sonuc.hataStr=ekoKod+" Tertibinde Daha önceden veri  oluşturulduğu için bu işleminiz gerçekleştirilemedi.İlgili ekonomik koda ait verileri silerek tekrar deneyin.";
                sonuc.islemSonuc=false;         							
            }
            if (!sonuc.islemSonuc)
            {
                veyaz.VeriErisimKapat();
                ve.VeriErisimKapat();
                return sonuc;
            }
            
            IDataReader r=tabAhp.AHPAylikOdenekListe(yil,kurum,BISabitler.IslemKod[(int)ENUMIslemKod.AHP],asama,ekoKod);
            if (r!=null)
            {
                veyaz.BeginTransaction();
                
                while(r.Read()&& sonuc.islemSonuc)
                {	
                    string ekoKodDetay=VeriErisim.DegerAl(r,0,"");
                    int ay=VeriErisim.DegerAl(r,1,0);
                    double deger=VeriErisim.DegerAl(r,2,(double)0);

                    sonuc=tabIHP.IHPKaydet(yil,kurum,BISabitler.IslemKod[(int)ENUMIslemKod.IHP],asama,ekoKodDetay,ay,deger,(int)ENUMKayitDurumu.YENI);
                }
                r.Close();

                if (sonuc.islemSonuc )
                {
                    TNSDateTime  tarih=new TNSDateTime( DateTime.Now);
                    sonuc=tabForm.Sil(yil,kurum,(string)BISabitler.IslemKod[(int)ENUMIslemKod.IHP],asama);
                    if (sonuc.islemSonuc)
                        sonuc=tabForm.Kaydet(yil,kurum,(string)BISabitler.IslemKod[(int)ENUMIslemKod.IHP],asama,tarih,"","1",ENUMKayitDurumu.YENI.ToString(),1,"",1,1,"",tarih);
                }

                if (sonuc.islemSonuc)
                    veyaz.Commit();
                else
                    veyaz.Rollback();
            }
            
            veyaz.VeriErisimKapat();
            ve.VeriErisimKapat();

            return sonuc;
        }
        /// <summary>
        ///VeriErisim nesnesi gelmediği için, verierişim olarak null geçirilecektir. 
        ///Taslak oluşturma işlemi gerçekleşir. Bunun için verilen yöntemdeki seçeneğe uygun olan
        ///değerler bulunur. Bu değerler kbö ye bulunarak oran hesaplanır.(OnIkıOranBul())
        ///Bulunan oran hesaplanacak yılın kbo su ile carpılarak icmal rakamları elde edilir ve ISLFORM tablosuna eklenir.
        ///Yapılan işlem yeni kayıt olarak ISLFORM tablosuna da kayıt eklenir.
        ///Ödenek üstü olan tertipler, son aylardan düşme yapılır ve
        ///ödenek üstü durumu ortadan kaldırılır ve bilgi için kullanıcıya döndürülmek üzere dönecek değere eklenir.
        ///Eğer oluşturulacak ekonomik kodun önceki yıllar değeri yoksa, bu yılın da oluşturulamayacaktır. 
        ///Bu durumdaki tertiplerde kullanıcıya bilgi olarak döndürülmek üzere, dönen değere eklenir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanına bağlanmak için kullanılır. NULL ise yeni bağlantı kurulur, değilse var olan kullanılır.</param>
        /// <param name="yil">Taslak oluşturmak için gerekli yıl bilgisi.</param>
        /// <param name="asama">Taslak oluşturmak için gerekli sırano bilgisi.</param>
        /// <param name="tertip">Taslak oluşturmak için gerekli kurum kodunu, ekonomik kodu içinde tutan Tertip tipindeki collection.</param>
        /// <param name="durum">Taslak oluşturmak için gerekli durum bilgisi(Yeni_Kayıt).</param>
        /// <param name="yontem">Taslak oluşturmak için gerekli olan Icmal rakamlarını hangi değerleri kullanarak hesaplama yapacağımızı belirten bilgi. </param>
        /// <returns>İşlem başarılı olmuşsa TertiplerVAN collectionunda degerler döner. Hata oluşmuşsa Sonuc yapısı döndürülür.</returns>
        private TertiplerVAN TaslakKontrolsuz(VeriErisim veriErisim, int yil, int asama,  Tertip tertip, int durum, int yontem)
        {   
            TertiplerVAN donusDeger=new TertiplerVAN();
            TABButce tabButce=new TABButce(veriErisim);
            TABISLIHP tabIHP=new TABISLIHP(veriErisim);
            TABISLForm tabForm=new TABISLForm(veriErisim);
			
            IBHMServis servisBHM=TanimlaBHM();				
            ABSKodlarVAN akv=servisBHM.KodListele(yil,"4", (string)tertip.kodlar[3].ToString().Replace(".",""));
            veriErisim.BeginTransaction();
            string kurum="";
            bool KayitYok=false;
		
            for(int i=0; i<akv.absKodlar.Count;i++)
            {	
                KayitYok=true;
                ABSKod ak=(ABSKod)akv.absKodlar[i];
                kurum=tertip.kodlar[0].ToString().Replace(".","");
                double kboBuYilDeger=tabButce.kboBul(yil,kurum , "", "", ak.kod.ToString());

                if (kboBuYilDeger!=0.0)
                {
                    TNSCollection OnIkıDeger=new TNSCollection();
                    double oranKBO;				
                    OnIkıDeger=OnIkıOranBul(yil,asama, kurum,ak.kod.ToString(),yontem,out oranKBO);				
				
                    if(oranKBO!=0.0)
                    {
                        double[]ay=new double[12];
                        double toplam=0.0;
						
                        for(int k=0;k<12;k++)
                        {	
                            if ((double)OnIkıDeger[k]>0.0)
                            {
                                ay[k]=Math.Floor(kboBuYilDeger*(double)OnIkıDeger[k]);
                                toplam+=ay[k];
                            }
                        }

                        if(toplam>kboBuYilDeger)
                        {							
                            double fark=toplam-kboBuYilDeger;
                            int l=11;
                            while (fark>0 && l>=0)
                            {
                                if (ay[l]>fark)
                                    ay[l]=ay[l]-fark;
                                else
                                    ay[l]=0.0;

                                fark=fark-ay[l];
                                l--;
                            }                               
                            donusDeger.odenekUstuTertipler.Add(ak.kod);
                            donusDeger.sonuc.islemSonuc=true;
                        }
                        else if(toplam<kboBuYilDeger)
                            ay[11]+=kboBuYilDeger-toplam;

												
                        if ((tabIHP.IHPKontrol(yil,kurum,BISabitler.IslemKod[(int)ENUMIslemKod.IHP],asama,ak.kod.ToString().Replace(".",""))!=0.0))
                        {							
                            donusDeger.sonuc.hataStr=ak.kod.ToString()+" Tertibinde Daha önceden veri  oluşturulduğu için bu işleminiz gerçekleştirilemedi.İlgili ekonomik koda ait verileri silerek tekrar deneyin.";
                            donusDeger.sonuc.islemSonuc=false;
                            break;							
                        }
                        else
                        {
                            for(int j=0;j<12;j++)
                            {
                                if((double)ay[j]!=0.0)
                                {
                                    if (donusDeger.sonuc.islemSonuc)
                                        donusDeger.sonuc=tabIHP.IHPKaydet(yil,kurum,BISabitler.IslemKod[(int)ENUMIslemKod.IHP],asama,ak.kod.ToString().Replace(".",""),j+1,(double)ay[j],durum);
                                    else
                                        break;
                                }
                            }
                        }					
                    }
                    else
                    {
                        donusDeger.dagitilamiyanTertipler.Add(ak.kod);
                        donusDeger.sonuc.islemSonuc=true;
                    }					
                }		

            }
            if(!KayitYok)
                donusDeger.sonuc.bilgiStr="Seçtiğiniz ekonomik koda ait bilgi bulunamamıştır.";

            if (donusDeger.sonuc.islemSonuc )
            {
                TNSDateTime  tarih=new TNSDateTime( DateTime.Now);
                donusDeger.sonuc=tabForm.Sil(yil,kurum,(string)BISabitler.IslemKod[(int)ENUMIslemKod.IHP],asama);
                if (donusDeger.sonuc.islemSonuc)
                    donusDeger.sonuc=tabForm.Kaydet(yil,kurum,(string)BISabitler.IslemKod[(int)ENUMIslemKod.IHP],asama,tarih,"","1",ENUMKayitDurumu.YENI.ToString(),1,"",1,1,"",tarih);
            }

            if (donusDeger.sonuc.islemSonuc)
                veriErisim.Commit();
            else
                veriErisim.Rollback();

            return donusDeger;
        }

        /// <summary>
        /// İcmal hesaplaması yapabilmek için  verilen yöntem seçeneğini kullanarak gerekli oranları bulmamızı sağlar.
        /// </summary>
        /// <param name="yil">Oran bulmak için gerekli yıl bilgisi.</param>
        /// <param name="asama">Oran bulmak için gerekli sırano bilgisi.</param>
        /// <param name="kurum">Oran bulmak için gerekli kurum bilgisi.</param>
        /// <param name="ekoKod">Oran bulmak için gerekli ekokod bilgisi.</param>
        /// <param name="yontem">Oran bulmak için gerekli hesaplamanın yapılmasını sağlayan bilgisi.</param>
        /// <param name="oranKBO">Oran bulmak için gerekli kbö bilgisi.</param>
        /// <returns>İşlem başarılı olmuşsa TNSCollection collectionunda degerler döner. Hata oluşmuşsa Sonuc yapısı döndürülür.</returns>
        private TNSCollection  OnIkıOranBul(int yil,int asama,string kurum,string ekoKod,int yontem, out double oranKBO)
        {
            TNSCollection donusDeger=new TNSCollection();
            oranKBO=0.0;
					
            switch (yontem)
            {	case 1:
                    donusDeger=OranBulIHP(null,yil-1,asama,kurum, ekoKod,out oranKBO,1 );
                    break;
                case 2:
                    donusDeger=OranBulIHP(null,yil-1,asama,kurum, ekoKod,out oranKBO ,1);
                    break;
                case 3:
                    donusDeger=OranBulIHPOrtalama(yil, asama, kurum, ekoKod,out oranKBO,1);					
                    break;
                case 4:
                    donusDeger= OranBulIHP(null,yil-1,asama,kurum, ekoKod,out oranKBO,2 );	
                    break;
                case 5:
                    donusDeger= OranBulIHP(null,yil-2,asama,kurum,ekoKod,out oranKBO,2);
                    break;
                case 6:
                    donusDeger=OranBulIHPOrtalama(yil, asama, kurum, ekoKod,out oranKBO,2);
                    break;
                case 7:
                    donusDeger= OranBulIHP(null,yil,asama,kurum,ekoKod,out oranKBO,3);
                    break;
                default:
                    break;
            }
            return donusDeger;						
        }
        /// <summary>
        ///  Ortalamaya göre işlem yapmak için kullanılır.
        ///  OranBulIHP fonksiyonu iki kez çağrılarak  on ikilik iki değer bulunur.
        ///  Bulunan değerlerin ortalaması alınır. 
        ///  Elde edilen on iki oran TNScollectionda döndürülür.
        /// </summary>
        /// <param name="yil">Oran bulmak için gerekli yıl bilgisi.</param>
        /// <param name="asama">Oran bulmak için gerekli sırano bilgisi.</param>
        /// <param name="kurum">Oran bulmak için gerekli kurum bilgisi.</param>
        /// <param name="ekoKod">Oran bulmak için gerekli ekokod bilgisi.</param>
        /// <param name="oranKBO">Oran bulmak için gerekli kbö bilgisi.</param>
        /// <param name="tur">Yapılacak işlemi belirlememizi sağlar.</param>
        /// <returns>İşlem başarılı olmuşsa TNSCollection collectionunda degerler döner. Hata oluşmuşsa Sonuc yapısı döndürülür.</returns>
        private TNSCollection  OranBulIHPOrtalama(int yil,int asama,string kurum,string ekoKod,out double oranKBO,int tur)
        {
            TNSCollection donusDeger=new TNSCollection();						
            double[]ay=new double[12];
            double kbo,toplamAy1,toplamAy2;
            kbo=0.0;
            toplamAy1=0.0;
            toplamAy2=0.0;

           
            donusDeger= OranBulIHP(null,yil-1,asama,kurum, ekoKod,out oranKBO,tur);					
            kbo=oranKBO;
            for(int i=0;i<12;i++)
            {
                ay[i]=(double)donusDeger[i];
                toplamAy1+=ay[i];
            }						
            donusDeger.Clear();			
		
            donusDeger= OranBulIHP(null,yil-2,asama,kurum, ekoKod,out oranKBO,tur);	
            kbo+=oranKBO;

            for(int i=0;i<12;i++)  
                toplamAy2+=(double)donusDeger[i];
          
            if (toplamAy1>0.0)
            {
                if (toplamAy2>0.0)
                    for(int j=0;j<12;j++)                   
                        ay[j]= (((double)ay[j])+((double)donusDeger[j]))/2; 
                else
                    for(int j=0;j<12;j++)                   
                        ay[j]= (double)ay[j];                     
            }
            else if (toplamAy2>0.0)        
                for(int j=0;j<12;j++)                   
                    ay[j]= (double)donusDeger[j]; 

            donusDeger.Clear();

            for(int i=0;i<12;i++)
                donusDeger.Add(ay[i]);
            oranKBO=kbo;
													
            return donusDeger;			
        }

        /// <summary>
        /// Oranı bulmak için belirtilen tur e göre o ekokod için belirtilen degerler ve kbö
        /// veritabanından alınır. Değerler kbö ye bölünür bulunan değerler TNScollectiona döndürülür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanına bağlanmak için kullanılır. NULL ise yeni bağlantı kurulur, değilse var olan kullanılır.</param>
        /// <param name="yil">Oranı yapılacak işlem için gerekli olan yıl bilgisi.</param>
        /// <param name="asama">Oranı yapılacak işlem için gerekli sırano bilgisi.</param>
        /// <param name="kurum">Oranı yapılacak işlem için gerekli kurum bilgisi.</param>
        /// <param name="ekoKod">Oranı yapılacak işlem için gerekli ekonomik kod bilgisi.</param>
        /// <param name="oranKBO">Oran bulmak için gerekli kbö bilgisi.</param>
        /// <param name="tur">Yapılacak işlemi belirlememizi sağlar.</param>
        /// <returns>İşlem başarılı olmuşsa TNSCollection collectionunda degerler döner. Hata oluşmuşsa Sonuc yapısı döndürülür.</returns>
        private TNSCollection OranBulIHP(VeriErisim veriErisim, int yil,int asama, string kurum,string ekoKod,out double oranKBO,int tur)
        {	
            TNSCollection donusDeger=new TNSCollection();		
            VeriErisim veSorgu=VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABButce tabButce=new TABButce(veSorgu);
            Tertip tertip=new Tertip();
            tertip.kodlar.Add(kurum);
            tertip.kodlar.Add("");
            tertip.kodlar.Add("");
            tertip.kodlar.Add(ekoKod);
						
            double kbo=0.0;				
			
            IDataReader aylikOdenekler=null;
			
            if (tur == 1)// harcama
            {	
                kbo=tabButce.YSOTBul(yil,tertip.kodlar[0].ToString(),tertip.kodlar[1].ToString(),tertip.kodlar[2].ToString(),tertip.kodlar[3].ToString(),false);			
                TABHarcama tabHarcama=new TABHarcama(veSorgu);
                aylikOdenekler=tabHarcama.HarcamaAylikOdenek(yil,tertip.kodlar);                                       
            }
            else if(tur==2)   // ıhp
            {  
                kbo=tabButce.kboBul(yil,kurum,"","",ekoKod);
                TABISLIHP tabIHP=new TABISLIHP(veSorgu);
                aylikOdenekler=tabIHP.IHPAylikOdenek(yil,kurum,BISabitler.IslemKod[(int)ENUMIslemKod.IHP],asama,ekoKod);   
            }
           
            double[]ayDeger=new double[12];
            double[]ayOran=new double[12];

            for(int i=0;i<12;i++)
            {
                ayDeger[i]=0.0;
                ayOran[i]=0.0;
            }
		         
            if ((double)kbo>0.0)
            {
                if (aylikOdenekler!=null)
                {
                    while(aylikOdenekler.Read())
                    {							
                        int ay=VeriErisim.DegerAl(aylikOdenekler,0,0);
                        if(ay>=1 && ay<=12) 
                            if (tur!=1)
                                ayOran[ay-1]=((double)VeriErisim.DegerAl(aylikOdenekler,1,0.0)/kbo);
                            else
                            {
                                ayDeger[ay-1]=(double)VeriErisim.DegerAl(aylikOdenekler,1,0.0);
                                if (ay!=1)
                                {
                                    ayOran[ay-1]=(((double)ayDeger[ay-1]-(double)ayDeger[ay-2])/(kbo));
                                }
                                else
                                    ayOran[ay-1]=((double)ayDeger[ay-1]/(kbo));                              
                            }	                                            
                    }
                    aylikOdenekler.Close();	
                    for(int i=0;i<12;i++)
                        donusDeger.Add(ayOran[i]); 
                }  
                oranKBO=kbo; 
            }                
            else
                oranKBO=0.0;			

            return donusDeger;       
        }   

        */
    }// END CLASS DEFINITION ButceIslemServisi

} // ButceUygulamaServis
