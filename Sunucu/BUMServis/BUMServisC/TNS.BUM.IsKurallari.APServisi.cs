using System.Data;
using TNS.Veri;
//using TNS.BUM.IsKurallari.Veri;
//using TNS.BHM;
using TNS.KYM;
using System;
using TNS.BUM;
using TNS.BKD;
using TNS.BUM.IsKurallari.Veri;
using TNS.UZY;
using TNS.BHMv2;
using System.Collections;
namespace TNS.BUM.IsKurallari
{
    /// <summary>
    /// Bütçe Uygulama Aylık Harcama Servisi Sınıfı
    /// </summary>
    public class AylikHarcamaServisi
    {
        private static ENUMFormDurumGrup APOnayDurumBul(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumBirim, int asama, ENUMIslemKod islemKod)
        {
            Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            string kurum = TNS.BKD.Arac.KurumsaldanKuruma(kurumBirim, tUzBul.Kirilimlar(0));
            ENUMFormDurumGrup onayDurum = ButceIslemServisi.ButceIslemiOnayliMi(veriErisim, yil, kurum, BISabitler.IslemKod(islemKod), asama);

            if (onayDurum != ENUMFormDurumGrup.YENI && onayDurum != ENUMFormDurumGrup.HATALI)
            {
                return onayDurum;
            }
            if (kurum != kurumBirim)
            {
                return ButceIslemServisi.ButceIslemiOnayliMi(veriErisim, yil, kurumBirim, BISabitler.IslemKod(islemKod), asama);
            }
            return onayDurum;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum, Aşama, İşlem kodu bilgilerine göre,
        /// Ayrıntılı Programla ilgili işlem yapmadan önce kontrolleri gerçekleştirerek sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurumBirim">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, ENUMIslemKod türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, kontrol işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        private static Sonuc KontrolAPKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumBirim, int asama, ENUMIslemKod islemKod, TertipOdenek[] tertipBilgileri, ENUMTabloKod tablokod, string senaryoKod)
        {
            Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            Sonuc sonuc = new Sonuc();
            sonuc.kayitSay = 0;

            string kurum = TNS.BKD.Arac.KurumsaldanKuruma(kurumBirim, tUzBul.Kirilimlar(0));
            if (kurumBirim == "")
            {
                sonuc.DegerAta(ButceUygulamaResI.BUM001);
            }
            else
            {
                if (asama == 1 && kurumBirim == kurum)
                {
                    sonuc.DegerAta("1. aşamalarda birim seçerek kaydedebilirsiniz");
                }
                if (kullanan.kullaniciKodu == "")
                {
                    sonuc.hataStr = ButceUygulamaResI.BUM002;
                    sonuc.islemSonuc = false;
                }
            }
            if (sonuc.islemSonuc)
            {
                ENUMFormDurumGrup onayDurum = ButceIslemServisi.ButceIslemiOnayliMi(veriErisim, yil, kurum, BISabitler.IslemKod(islemKod), asama);
                sonuc.kayitSay = (int)onayDurum;
                if (onayDurum != ENUMFormDurumGrup.YENI && onayDurum != ENUMFormDurumGrup.HATALI)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ButceUygulamaResI.BUM003;
                }
                if (kurum != kurumBirim)
                {
                    onayDurum = ButceIslemServisi.ButceIslemiOnayliMi(veriErisim, yil, kurumBirim, BISabitler.IslemKod(islemKod), asama);
                    sonuc.kayitSay = (int)onayDurum;
                    if (onayDurum != ENUMFormDurumGrup.YENI && onayDurum != ENUMFormDurumGrup.HATALI)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = ButceUygulamaResI.BUM003;
                    }
                }
            }
            if (sonuc.islemSonuc)
            {
                foreach (TertipOdenek tb in tertipBilgileri)
                {
                    if (!(BUMYetkiServisi.AHPYazmaYetkisiVarMi(kullanan, yil, tb.tertip, asama)))
                    {
                        sonuc.DegerAta(ButceUygulamaResI.BUM004);
                    }

                    if (tb.tertip.tabloKod != tablokod)
                    {
                        sonuc.DegerAta(tb.tertip.TertipStrOl() + " " + ButceUygulamaResI.BUM007);
                    }
                    if (!tb.tertip.kodlar[0].Replace(".", "").StartsWith(kurumBirim.Replace(".", "")))
                    {
                        sonuc.DegerAta(tb.tertip.TertipStrOl() + " " + ButceUygulamaResI.BUM008 + " " + kurumBirim + " " + ButceUygulamaResI.BUM009);
                    }

                    double kontrolRakam = UstLimitBul(veriErisim, yil, tb.tertip, senaryoKod);
                    double toplamKbo = 0.0;
                    for (int j = 0; j < 12; j++)
                        toplamKbo += tb.odenek[j];
                    if ((toplamKbo > kontrolRakam))
                    {
                        sonuc.DegerAta(tb.tertip.TertipStrOl() + " tertibinde ay toplamı" + "(" + toplamKbo.ToString("#,###") + ") " + ButceUygulamaResI.BUM011 + " (" + kontrolRakam.ToString("#,###") + ") " + ButceUygulamaResI.BUM012);
                    }
                }
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Tertip bilgisine göre,
        /// Üst limiti bulup döndüren fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, üst limit değerini tutan Double türünde nesne döndürür</returns>
        public static double UstLimitBul(VeriErisim veriErisim, int yil, Tertip tertip, string senaryoKod)
        {
            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            string uzayAd = "";
            if (tertip.tabloKod == ENUMTabloKod.TABButce || tertip.tabloKod == ENUMTabloKod.TABPEBOdenek)
                uzayAd = "ABSODENEK";
            else
                uzayAd = "ABSGELIR";

            double ustLimit = uzyServis.UzayDegeriDbl(null, uzayAd, yil + "-" + senaryoKod + "-0-" + tertip.TertipStrOl(), false);

            if (tertip.tabloKod == ENUMTabloKod.TABButce)
            {
                TABAHPBloke tabBloke = new TABAHPBloke(veriErisim);
                double kontrolRakam = tabBloke.UstLimitBul(yil, tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4]);
                if (kontrolRakam >= 0 && kontrolRakam < ustLimit)
                    ustLimit = kontrolRakam;
            }
            return ustLimit;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum Birim, Aşama ve Tertip bilgilerine göre,
        /// Ayrıntılı Program bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurumBirim">Kurum birim bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertipBilgileri">Tertip bilgilerini tutan, TNSCollection türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, kaydetme işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc APKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumBirim, int asama, TertipOdenek[] tertipBilgileri)
        {
            if (tertipBilgileri.Length < 1)
            {
                return new Sonuc(false, ButceUygulamaResI.BUM005, "");
            }
            ENUMTabloKod tablokod = tertipBilgileri[0].tertip.tabloKod;
            ENUMIslemKod islemKod;
            if (tablokod == ENUMTabloKod.TABButce || tablokod == ENUMTabloKod.TABPEBOdenek)
            {
                islemKod = ENUMIslemKod.AHP;
            }
            else if (tablokod == ENUMTabloKod.TABGelir || tablokod == ENUMTabloKod.TABFinansman || tablokod == ENUMTabloKod.TABPEBGelir)
            {
                islemKod = ENUMIslemKod.IHP;
            }
            else
                return new Sonuc(false, ButceUygulamaResI.BUM006, "");

            IBHMv2Servis servisBHM = BHMv2.Arac.Tanimla();
            string odenekAsama = servisBHM.OnayliAsamaBul(yil);
            if (string.IsNullOrWhiteSpace(odenekAsama)) odenekAsama = "06.00";

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            if (asama > 1) kurumBirim = TNS.DEG.Arac.VarsayilanKurumBul();
            Sonuc sonuc = KontrolAPKaydet(ve, kullanan, yil, kurumBirim, asama, islemKod, tertipBilgileri, tablokod, odenekAsama);
            if (sonuc.islemSonuc)
            {
                TABISLForm tabForm = new TABISLForm(ve);
                TABAyrintiliProgram tabAP;
                if (tablokod == ENUMTabloKod.TABButce || tablokod == ENUMTabloKod.TABPEBOdenek)
                    tabAP = new TABISLAPGider(ve);
                else if (tablokod == ENUMTabloKod.TABGelir || tablokod == ENUMTabloKod.TABPEBGelir)
                    tabAP = new TABISLAPGelir(ve);
                else
                    tabAP = new TABISLAPFinansman(ve);

                ve.BeginTransaction();
                TNSDateTime onaytarih = new TNSDateTime(DateTime.Now);
                if (sonuc.kayitSay == (int)ENUMFormDurumGrup.HATALI)
                    sonuc = tabForm.Kaydet(yil, kurumBirim, (string)BISabitler.IslemKod(islemKod), asama, onaytarih, "", "", "", (int)ENUMKayitDurumu.YENI, "", 0, 1, kullanan.kullaniciKodu, onaytarih, "", -1);
                else
                    sonuc = tabForm.Guncelle(yil, kurumBirim, (string)BISabitler.IslemKod(islemKod), asama, onaytarih, "", "", "", (int)ENUMKayitDurumu.YENI, "", 0, 1, kullanan.kullaniciKodu, onaytarih, "", -1);

                if (sonuc.islemSonuc)
                {
                    foreach (TertipOdenek tb in tertipBilgileri)
                    {
                        if (!sonuc.islemSonuc)
                            break;
                        sonuc = tabAP.Sil(yil, "", BISabitler.IslemKod(islemKod), asama, tb.tertip.kodlar);
                        if (!sonuc.islemSonuc)
                            break;
                        for (int k = 0; k < 12; k++)
                        {
                            if (tb.odenek[k] != 0.0)
                            {
                                sonuc = tabAP.Kaydet(yil, kurumBirim, BISabitler.IslemKod(islemKod), asama, tb.tertip.kodlar, k + 1, Math.Floor(tb.odenek[k]), ENUMFormDurum.YENIKAYIT);
                                if (!sonuc.islemSonuc)
                                    break;
                            }
                        }
                    }
                }

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kullanıcı, Aşama, Tertip, İşlem bilgilerine göre,
        /// Listeleme yapmadan önce gerekli kontrolleri yapıp sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasTertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, kontrol işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        private static Sonuc KontrolEtListele(VeriErisim veriErisim, int yil, Kullanici kullanan, int asama, Tertip kistasTertip, string islem)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (string.IsNullOrEmpty(kistasTertip.kodlar[0]))
            {
                sonuc.hataStr = ButceUygulamaResI.BUM013;
                sonuc.islemSonuc = false;
            }
            else if (kullanan == null || kullanan.kullaniciKodu == "")
            {
                sonuc.hataStr = ButceUygulamaResI.BUM014;
                sonuc.islemSonuc = false;
            }
            else
            {
                if (!BUMYetkiServisi.AHPGormeYetkisiVarMi(kullanan, yil, kistasTertip, asama == 3 ? 1 : asama))
                {
                    sonuc.hataStr = ButceUygulamaResI.BUM015;
                    sonuc.islemSonuc = false;
                }
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama ve Tertip bilgilerine göre,
        /// Ayrıntılı Program bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasTertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Ayrıntılı Program bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        public static TertipOdenek[] APListele(VeriErisim veriErisim, Kullanici kullanan, int asama, Tertip kistasTertip)
        {
            bool eko2 = TNS.MUH.Arac.OdenekGondermeEko2liMi(kistasTertip.yil);
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            for (int i = 0; i < kistasTertip.KodNoSay(); i++)
            {
                bool eko23 = false;
                int tut = kistasTertip.KodNoKirilimSay(i);
                if (i == 3 && eko2)
                {
                    eko23 = TNS.BKD.Arac.Eko2_3Bul(kullanan);
                    tut = 2;
                }
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(i), tut, eko23));
            }
            return APListeleKodBilgi(veriErisim, kullanan, asama, kistasTertip, kodBilgileri);
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama ve Tertip bilgilerini göre,
        /// Ayrıntılı Programı okuyan fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasTertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Ayrıntılı Program bilgisini tutan Object türünde nesne döndürür</returns>
        public static TertipOdenek APOku(VeriErisim veriErisim, Kullanici kullanan, int yil, int asama, Tertip kistasTertip)
        {
            TertipOdenek[] toler = APListele(veriErisim, kullanan, asama, kistasTertip);
            if (toler.Length > 0)
                return toler[0];
            else
                return null;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama, Tertip ve Ekonomik Seviye bilgilerine göre,
        /// Ayrıntılı Program bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasTertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <param name="ukb">Sonuç kümesinde bulunacak tertibin hangi kodlardan oluşacağı bilgisi.</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Ayrıntılı Program bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        public static TertipOdenek[] APListeleKodBilgi(VeriErisim veriErisim, Kullanici kullanan, int asama, Tertip kistasTertip, UcgenKodBilgileri ukb)
        {
            int yil = kistasTertip.yil;
            ENUMTabloKod tablokod = kistasTertip.tabloKod;
            ENUMIslemKod islemKod;
            if (tablokod == ENUMTabloKod.TABButce || tablokod == ENUMTabloKod.TABPEBOdenek)
            {
                islemKod = ENUMIslemKod.AHP;
            }
            else if (tablokod == ENUMTabloKod.TABGelir || tablokod == ENUMTabloKod.TABFinansman || tablokod == ENUMTabloKod.TABPEBGelir)
            {
                islemKod = ENUMIslemKod.IHP;
            }
            else
            {
                throw new Exception(ButceUygulamaResI.BUM016);
            }

            IUZYServis uzyServis = BUMServis.TanimlaUZY();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            Sonuc sonuc = KontrolEtListele(ve, yil, kullanan, asama, kistasTertip, BISabitler.IslemKod(islemKod));
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                throw new Exception(sonuc.hataStr);
            }

            TABAyrintiliProgram tahp;
            IBHMv2Servis servisBHM = BHMv2.Arac.Tanimla();
            string odenekAsama = servisBHM.OnayliAsamaBul(yil);
            if (string.IsNullOrWhiteSpace(odenekAsama)) odenekAsama = "06.00";

            EkDegerParam[] ekBilgiler = new EkDegerParam[14];
            if (tablokod == ENUMTabloKod.TABButce || tablokod == ENUMTabloKod.TABPEBOdenek)
            {
                tahp = new TABISLAPGider(ve);
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, odenekAsama);
                ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABOdenekGonderme, 0, 0, "");
            }
            else if (tablokod == ENUMTabloKod.TABGelir || tablokod == ENUMTabloKod.TABPEBGelir)
            {
                tahp = new TABISLAPGelir(ve);
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBGelir, 0, 0, odenekAsama);
                ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPEBGelirGerceklesmesi, 0, 0, "");
            }
            else
            {
                tahp = new TABISLAPFinansman(ve);
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABFinansman, 0, 0, odenekAsama);
                ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABFinansman, 0, 0, odenekAsama);
            }
            for (int i = 1; i <= 12; i++)
            {
                ekBilgiler[i + 1] = new EkDegerParam(ENUMTabloKod.TABGIDERAP, 0, 0, $"{asama};{i}");
            }
            ArrayList al = new ArrayList();
            OdenekBilgi[] obler = null;
            try
            {
                obler = servisBHM.ListeTertipOl(kullanan, kistasTertip, ukb, ekBilgiler, true);
            }
            catch (Exception ex)
            {
                ve.VeriErisimKapat();
                throw ex;
            }
            foreach (OdenekBilgi ob in obler)
            {
                TertipOdenek tb = new TertipOdenek(14);
                tb.tertip = ob.tertip;
                if (kistasTertip.tabloKod == ENUMTabloKod.TABPEBOdenek)
                    tb.tertip.tertipAdi = tb.tertip.adlar[3] + "(" + tb.tertip.adlar[4] + ")";
                for (int i = 0; i < 14; i++)
                {
                    tb.odenek[i] = (double)ob.ekRakamlar[i];
                }
                al.Add(tb);
                //if (tb.odenek[0] != 0)
                //{
                //    tb.odenek[1] = (double)ob.ekRakamlar[1];

                //    if (string.IsNullOrEmpty(tb.tertip.kodlar[0]))
                //        tb.tertip.kodlar[0] = kistasTertip.kodlar[0];

                //    al.Add(tb);
                //    IDataReader degerler = tahp.AylikOdenek(yil, "", BISabitler.IslemKod(islemKod), asama, tb.tertip.kodlar);
                //    if (degerler != null)
                //    {
                //        while (degerler.Read())
                //        {
                //            int ay = VeriErisim.DegerAl(degerler, 0, 0);
                //            if (ay >= 1 && ay <= 12)
                //                tb.odenek[ay + 1] = VeriErisim.DegerAl(degerler, 1, 0.0);
                //            else
                //            {
                //                degerler.Close();
                //                ve.VeriErisimKapat();
                //                throw new Exception(ButceUygulamaResI.BUM017);
                //            }
                //        }
                //        degerler.Close();
                //    }
                //    else
                //    {
                //        ve.VeriErisimKapat();
                //        throw new Exception(ButceUygulamaResI.BUM018 + ":\r\n" + ve.hata);
                //    }
                //}
            }
            ve.VeriErisimKapat();
            return (TertipOdenek[])al.ToArray(typeof(TertipOdenek));
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Tertip bilgilerine göre,
        /// Ayrıntılı Programın son durumunu gösteren fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Ayrıntılı Program bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        public static ObjectArray APSonDurumGoster(VeriErisim veriErisim, int yil, Tertip tertip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));
            ObjectArray ahv = new ObjectArray();
            TABAyrintiliProgram tabAP;

            if (tertip.tabloKod == ENUMTabloKod.TABButce || tertip.tabloKod == ENUMTabloKod.TABOdenek)
                tabAP = new TABISLAPGider(ve);
            else if (tertip.tabloKod == ENUMTabloKod.TABGelir || tertip.tabloKod == ENUMTabloKod.TABPEBGelir)
                tabAP = new TABISLAPGelir(ve);
            else if (tertip.tabloKod == ENUMTabloKod.TABFinansman)
                tabAP = new TABISLAPFinansman(ve);
            else
            {
                ahv.sonuc.hataStr = ButceUygulamaResI.BUM027 + ":\r\n" + ve.hata;
                ahv.sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return ahv;
            }

            IDataReader degerler = tabAP.TertipDurum(yil, tertip.kodlar);
            ahv.sonuc.islemSonuc = true;
            if (degerler != null)
            {
                TertipBilgi tb = new TertipBilgi();
                while (degerler.Read())
                {
                    int ay = VeriErisim.DegerAl(degerler, 0, 0);
                    if (ay >= 1 && ay <= 12)
                        tb.ayDegerler[ay - 1] = VeriErisim.DegerAl(degerler, 1, 0.0);
                    else
                    {
                        ahv.sonuc.hataStr = ButceUygulamaResI.BUM028;
                        ahv.sonuc.islemSonuc = false;
                        break;
                    }
                }
                degerler.Close();

                if (ahv.sonuc.islemSonuc)
                    ahv.Ekle(tb);
            }
            else
            {
                ahv.sonuc.hataStr = ButceUygulamaResI.BUM029 + ":\r\n" + ve.hata;
                ahv.sonuc.islemSonuc = false;
            }

            ve.VeriErisimKapat();
            return ahv;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama ve Tertip bilgilerine göre,
        /// Taslak oluşturmadan önce gerekli kontrolleri yaparak sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, kontrol işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        private static Sonuc KontrolEtTaslakOlustur(Kullanici kullanan, VeriErisim veriErisim, int yil, int asama, Tertip tertip)
        {
            Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            Sonuc sonuc = new Sonuc(true, "", "");
            string kurum;
            if (asama > 1)
                kurum = TNS.BKD.Arac.KurumsaldanKuruma(tertip.kodlar[0], tUzBul.Kirilimlar(0));
            else
                kurum = tertip.kodlar[0];
            if (kullanan == null)
            {
                sonuc.DegerAta(ButceUygulamaResI.BUM030, "");
            }
            else if (string.IsNullOrEmpty(tertip.kodlar[0]))
            {
                sonuc.DegerAta(ButceUygulamaResI.BUM031, "");
            }
            else if (!(BUMYetkiServisi.AHPYazmaYetkisiVarMi(kullanan, yil, kurum, asama)))
            {
                sonuc.DegerAta(ButceUygulamaResI.BUM032, "");
            }
            else
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));

                TABISLForm onayliMi = new TABISLForm(veriErisim);
                ENUMFormDurumGrup onaydurum = ButceIslemServisi.ButceIslemiOnayliMi(veriErisim, yil, kurum, BISabitler.IslemKod(ENUMIslemKod.AHP), asama);
                if (onaydurum == ENUMFormDurumGrup.ONAYLI)
                {
                    sonuc.DegerAta(ButceUygulamaResI.BUM033, "");
                }
                ve.VeriErisimKapat();
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Aşama, Tertip, Yöntem bilgilerine göre,
        /// Taslak Oluşturan fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <param name="yontem">Yöntem bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Taslak bilgilerini tutan TertiplerVAN türünde nesne döndürür</returns>
        public static TertiplerVAN TaslakOlustur(VeriErisim veriErisim, Kullanici kullanan, int yil, int asama, Tertip tertip, int yontem)
        {
            Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            TertiplerVAN donusDeger = new TertiplerVAN();
            VeriErisim veListe = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim veKaydet = VeriErisim.VeriErisimBul(null, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim veIcmal = VeriErisim.VeriErisimBul(null, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));

            TABButce tabButceListe = new TABButceGider(veListe);

            TABISLForm tabForm = new TABISLForm(veKaydet);
            TABAyrintiliProgram tabAP = new TABISLAPGider(veKaydet);

            donusDeger.sonuc = KontrolEtTaslakOlustur(kullanan, veListe, yil, asama, tertip);

            if (donusDeger.sonuc.islemSonuc)
            {
                string kurum = TNS.BKD.Arac.KurumsaldanKuruma((string)tertip.kodlar[0], tUzBul.Kirilimlar(0)).Replace(".", "");
                IDataReader dr = tabButceListe.ButceTertipListele(yil, tertip.kodlar, true);
                veKaydet.BeginTransaction();
                bool KayitYok = false;
                while (dr.Read())
                {
                    KayitYok = true;
                    Tertip gonderTertip = new Tertip(yil, tertip.tabloKod);
                    gonderTertip.TertibeKodEkle(0, dr.GetString(0), "");
                    gonderTertip.TertibeKodEkle(1, dr.GetString(1), "");
                    gonderTertip.TertibeKodEkle(2, dr.GetString(2), "");
                    gonderTertip.TertibeKodEkle(3, dr.GetString(3), "");

                    double kboBuYilDeger = VeriErisim.DegerAl(dr, 4, (double)0);
                    if (kboBuYilDeger > 0)
                    {
                        double oranKBO = 0.0;
                        double[] AylikOran = AylikOranBul(veIcmal, yil, kurum, BISabitler.IslemKod(ENUMIslemKod.AHP), asama, gonderTertip, out oranKBO, yontem);
                        if (oranKBO != 0)
                        {
                            double[] ayDegerler = new double[12];
                            double toplam = 0.0;
                            for (int i = 0; i < 12; i++)
                            {
                                ayDegerler[i] = Math.Floor(kboBuYilDeger * AylikOran[i]);
                                toplam = toplam + ayDegerler[i];
                            }

                            if (toplam > kboBuYilDeger)
                            {
                                double fark = toplam - kboBuYilDeger;
                                int i = 11;
                                while (fark > 0 && i >= 0)
                                {
                                    if (ayDegerler[i] > fark)
                                        ayDegerler[i] = ayDegerler[i] - fark;
                                    else
                                        ayDegerler[i] = 0.0;

                                    fark = fark - ayDegerler[i];
                                    i--;
                                }
                                donusDeger.odenekUstuTertipler.Add(tertip.TertipStrOl());
                                donusDeger.sonuc.islemSonuc = true;
                            }
                            else if (toplam < kboBuYilDeger)
                                ayDegerler[11] = ayDegerler[11] + kboBuYilDeger - toplam;

                            if ((tabAP.Kontrol(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.AHP), asama, gonderTertip.kodlar) != 0.0))
                            {
                                donusDeger.dagitilamiyanTertipler.Add(gonderTertip.TertipStrOl());
                            }
                            else
                            {
                                for (int j = 0; j < 12; j++)
                                {
                                    if (ayDegerler[j] != 0.0)
                                    {
                                        if (donusDeger.sonuc.islemSonuc)
                                            donusDeger.sonuc = tabAP.Kaydet(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.AHP), asama, gonderTertip.kodlar, j + 1, Math.Floor(ayDegerler[j]), ENUMFormDurum.YENIKAYIT);
                                        else
                                            break;
                                    }
                                }
                            }

                        }// oranKBO!=0
                        else
                            donusDeger.dagitilamiyanTertipler.Add(gonderTertip.TertipStrOl());
                    }//kboBuYilDeger>0                   
                }// tertip listele while sonu
                dr.Close();
                if (!KayitYok)
                    donusDeger.sonuc.bilgiStr = ButceUygulamaResI.BUM034;

                if (donusDeger.sonuc.islemSonuc)
                {
                    TNSDateTime onaytarih = new TNSDateTime(DateTime.Now);
                    donusDeger.sonuc = tabForm.Sil(yil, kurum, (string)BISabitler.IslemKod(ENUMIslemKod.AHP), asama);
                    if (donusDeger.sonuc.islemSonuc)
                        donusDeger.sonuc = tabForm.Kaydet(yil, kurum, (string)BISabitler.IslemKod(ENUMIslemKod.AHP), asama, onaytarih, "", "1", ENUMKayitDurumu.YENI.ToString(), 1, "", 1, 1, "", onaytarih, "", -1);
                }

                if (donusDeger.sonuc.islemSonuc)
                    veKaydet.Commit();
                else
                    veKaydet.Rollback();
            }//kontrol ifi sonu
            veKaydet.VeriErisimKapat();
            veListe.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem, Aşama, Tertip, Oran KBÖ, Yöntem bilgilerine göre,
        /// Aylık Oranları, OranBulAHP rutinini çalıştırarak bulan fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <param name="oranKBO">KBÖ Oran bilgisini tutan, Double türünde nesne</param>
        /// <param name="yontem">Yöntem bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Aylık Oranları tutan Double dizi türünde nesne döndürür</returns>
        public static double[] AylikOranBul(VeriErisim veriErisim, int yil, string kurum, string islem, int asama, Tertip tertip, out double oranKBO, int yontem)
        {
            double[] donusDeger = null;
            oranKBO = 0.0;

            if (yontem == 1)
                donusDeger = OranBulAHP(veriErisim, yil - 2, kurum, islem, 2, tertip, out oranKBO, 3);  //1 ahp,2 ıhp,3 harcama

            else if (yontem == 2)
                donusDeger = OranBulAHP(veriErisim, yil - 1, kurum, islem, 2, tertip, out oranKBO, 3);// harcama için 3

            //else if (yontem == 3)
            //    donusDeger = OranBulAHPOrtalama(veriErisim, yil - 1, kurum, islem, 2, tertip, out oranKBO, 3);  //3 harcama ortalamas 

            else if (yontem == 4)
                donusDeger = OranBulAHP(veriErisim, yil - 2, kurum, islem, 2, tertip, out oranKBO, 1); // ahp 

            else if (yontem == 5)
                donusDeger = OranBulAHP(veriErisim, yil - 1, kurum, islem, 2, tertip, out oranKBO, 1); // ahp

            //else if (yontem == 6)
            //    donusDeger = OranBulAHPOrtalama(veriErisim, yil - 1, kurum, islem, 2, tertip, out oranKBO, 1); // ahp ortalaması

            else if (yontem == 7)
                donusDeger = OranBulAHP(veriErisim, yil, kurum, BISabitler.IslemKod(ENUMIslemKod.IHP), asama, tertip, out oranKBO, 2); // ıhp

            //            else if(yontem=5)
            //                OranBulAHPOrtalama(yil,kurum,islem,asama,gonderTertip); 
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem, Aşama, Tertip, Oran KBÖ ve Tür bilgilerine göre,
        /// Ayrıntılı Harcama Programı Oranlarını bulan fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <param name="oranKBO">KBÖ Oran bilgisini tutan, Double türünde nesne</param>
        /// <param name="tur">Tür bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Ayrıntılı Harcama Programı oran bilgilerini tutan Double dizi türünde nesne döndürür</returns>
        public static double[] OranBulAHP(VeriErisim veriErisim, int yil, string kurum, string islem, int asama, Tertip tertip, out double oranKBO, int tur)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));

            double[] donusDeger = new double[12];
            for (int j = 0; j < 12; j++)
                donusDeger[j] = 0.0;
            TABButce tabButce = new TABButceGider(ve);
            IDataReader aylikOdenekler = null;
            double kbo = 0.0;
            if (tur == 3)// harcama
            {
                kbo = tabButce.YSOTBul(yil, tertip.kodlar, false);
                TABHarcamaAyIci tabHarcama = new TABHarcamaAyIci(veriErisim);
                aylikOdenekler = tabHarcama.HarcamaAylikOdenek(yil, tertip.kodlar);
            }
            else if (tur == 2)   // ıhp
            {
                string[] kodlar = { kurum, "", "", tertip.kodlar[3] };
                kbo = tabButce.KBOBul(yil, kodlar);
                TABISLIHP tabIHP = new TABISLIHP(veriErisim);
                aylikOdenekler = tabIHP.IHPAylikOdenek(yil, kurum, islem, asama, tertip.kodlar[3].ToString(), ENUMFormDurumGrup.HEPSI);
            }
            else if (tur == 1)// ahp
            {
                kbo = tabButce.KBOBul(yil, tertip.kodlar);
                TABISLAPGider tabAHP = new TABISLAPGider(veriErisim);
                aylikOdenekler = tabAHP.AylikOdenek(yil, kurum, islem, asama, tertip.kodlar);
            }

            if (kbo > 0.0 && aylikOdenekler != null)
            {
                while (aylikOdenekler.Read())
                {
                    double toplamOdenek = 0;
                    double toplamOran = 0;
                    int ay = VeriErisim.DegerAl(aylikOdenekler, 0, 0);
                    if (ay >= 1 && ay <= 12)
                    {
                        double odenek = VeriErisim.DegerAl(aylikOdenekler, 1, 0.0);
                        toplamOdenek += odenek;
                        double oran = Math.Round(odenek / kbo, -2);
                        donusDeger[ay - 1] = oran;
                        toplamOran += oran;
                    }
                    if (toplamOdenek == kbo && toplamOran != 100)
                    {
                        donusDeger[11] = donusDeger[11] - (toplamOran - 100);
                    }
                }

                aylikOdenekler.Close();
                oranKBO = kbo;
            }
            else
                oranKBO = 0.0;
            ve.VeriErisimKapat();


            return donusDeger;

        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum, Aşama bilgilerini kullanarak,
        /// Ayrıntılı Programın Onayını kaldıran fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Onay kaldırma işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc OnayKaldirAP(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurum, int asama)
        {//1
            Sonuc sonuc = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));

            if (asama < 3) //Sonraki aşamada işlem varsa onay kaldırma
            {
                ENUMFormDurumGrup dur = APOnayDurumBul(ve, kullanan, yil, kurum, asama + 1, ENUMIslemKod.AHP);
                if (dur != ENUMFormDurumGrup.HATALI)
                    sonuc.DegerAta("Bir sonraki adımın işlemleri başladığından bu aşamanın onayı kaldırılamaz!");
            }
            if (kurum == "")
                sonuc.DegerAta(ButceUygulamaResI.BUM035, "");
            //else if (!TNS.BKD.Arac.KurumMu(kurum))
            //    sonuc.DegerAta(ButceUygulamaResI.BUM036, "");
            else if (kullanan == null)
                sonuc.DegerAta(ButceUygulamaResI.BUM037, "");
            else if (!BUMYetkiServisi.AHPOnaylamaYetkisiVarMi(kullanan, yil, kurum, asama))
                sonuc.DegerAta(ButceUygulamaResI.BUM038, "");

            if (sonuc.islemSonuc)
            {
                TABISLForm tabForm = new TABISLForm(ve);
                TABISLAPGider detay = new TABISLAPGider(ve);
                //TABButce tabButce = new TABButceGider(ve);

                TNSDateTime onaytarih = new TNSDateTime(DateTime.Now);
                ve.BeginTransaction();

                sonuc = tabForm.OnayKaydet(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.AHP), asama, (int)ENUMFormDurum.YENIKAYIT, -1, kullanan.kullaniciKodu, onaytarih);
                if (sonuc.islemSonuc)
                    sonuc = detay.OnayKaydet(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.AHP), asama, (int)ENUMFormDurum.YENIKAYIT);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                }
                else
                    ve.Rollback();
                //if (sonuc.islemSonuc)
                //    ButceIslemServisi.SerbestHesapla(null, null, yil, kurum, ENUMTabloKod.TABButce);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, Aşama bilgilerine göre,
        /// Blokeli Tertipleri kontrol eden ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kontrol işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc BlokeliTertipKontrol(VeriErisim veriErisim, int yil, string kurum, int asama)
        {
            Sonuc s = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));
            VeriErisim veAHP = VeriErisim.VeriErisimBul(null, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));
            TABAHPBloke tbBloke = new TABAHPBloke(ve);
            TABISLAPGider tbAHP = new TABISLAPGider(veAHP);

            IDataReader drBloke = tbBloke.BlokeListele(yil, kurum);
            if (drBloke != null)
            {
                while (drBloke.Read())
                {
                    Tertip trp = new Tertip(yil, ENUMTabloKod.TABButce, VeriErisim.DegerAl(drBloke, 1, ""), VeriErisim.DegerAl(drBloke, 2, ""), VeriErisim.DegerAl(drBloke, 3, ""), VeriErisim.DegerAl(drBloke, 4, ""));
                    double blokeMiktari = VeriErisim.DegerAl(drBloke, 5, 0.0);

                    double toplam = tbAHP.TertipToplamiBul(yil, BISabitler.IslemKod(ENUMIslemKod.AHP), asama, trp.kodlar);

                    if (toplam > blokeMiktari)
                    {
                        s.DegerAta(trp.TertipStrOl() + " " + ButceUygulamaResI.BUM039 + "(" + blokeMiktari.ToString("#,###") + ") " + ButceUygulamaResI.BUM040 + "(" + toplam.ToString("#,###") + ")." + ButceUygulamaResI.BUM041, "");
                        break;
                    }
                }
                drBloke.Close();
            }
            else
                s.DegerAta(ButceUygulamaResI.BUM042 + "\r\n" + ve.hata, "");

            ve.VeriErisimKapat();
            veAHP.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum ve Aşama bilgilerine göre,
        /// İcmal Harcama ve Ayrıntılı Harcama Programı bilgilerini kontrol eden ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kontrol işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc IHPAHPKontrol(VeriErisim veriErisim, int yil, string kurum, int asama)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));

            TABISLIHP tabIHP = new TABISLIHP(ve);
            TABISLAPGider tabAHP = new TABISLAPGider(ve);
            IBKDServis servisBKD = BUMServis.TanimlaBKD();

            ABSKodlarVAN akv = servisBKD.KodListeleSeviye(yil, "4", "", 2, true);

            for (int i = 0; i < akv.absKodlar.Count; i++)
            {
                if (sonuc.islemSonuc)
                {
                    ABSKod ak = (ABSKod)akv.absKodlar[i];
                    if (ak.kod.Length == 4)
                    {
                        double[] ayDegerlerIHP = new double[12];
                        double[] ayDegerlerAHP = new double[12];
                        for (int j = 0; j < 12; j++)
                        {
                            ayDegerlerIHP[j] = 0.0;
                            ayDegerlerAHP[j] = 0.0;
                        }

                        IDataReader IHPdegerler = tabIHP.IHPAylikOdenek(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.IHP), asama, ak.kod, ENUMFormDurumGrup.HEPSI);

                        if (IHPdegerler != null)
                        {
                            while (IHPdegerler.Read())
                            {
                                int ay = VeriErisim.DegerAl(IHPdegerler, 0, 0);
                                if (ay >= 1 && ay <= 12)
                                    ayDegerlerIHP[ay - 1] = VeriErisim.DegerAl(IHPdegerler, 1, 0.0);
                            }
                            IHPdegerler.Close();
                        }
                        else
                            sonuc.DegerAta(ButceUygulamaResI.BUM043 + ":\r\n", "");

                        if (sonuc.islemSonuc)
                        {
                            string[] kodDizi = { kurum, "", "", ak.kod };
                            IDataReader AHPdegerler = tabAHP.AylikOdenek(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.AHP), asama, kodDizi);

                            if (AHPdegerler != null)
                            {
                                while (AHPdegerler.Read())
                                {
                                    int ay = VeriErisim.DegerAl(AHPdegerler, 0, 0);
                                    if (ay >= 1 && ay <= 12)
                                        ayDegerlerAHP[ay - 1] = VeriErisim.DegerAl(AHPdegerler, 1, 0.0);
                                }
                                AHPdegerler.Close();
                            }
                            else
                                sonuc.DegerAta(ButceUygulamaResI.BUM044 + ":\r\n" + ve.hata, "");
                        }

                        double toplamIHP = 0.0;
                        double toplamAHP = 0.0;
                        for (int j = 0; j < 11 && sonuc.islemSonuc; j++)
                        {
                            toplamIHP += ayDegerlerIHP[j];
                            toplamAHP += ayDegerlerAHP[j];
                            if (toplamAHP > toplamIHP)
                            {
                                sonuc.DegerAta(ak.kod + " " + ButceUygulamaResI.BUM045, "");
                            }
                        }
                        //                        if (asama!=1)
                        //                        {
                        //                            if(sonuc.islemSonuc)                              
                        //                            {
                        //                                if((ak.kod.Substring(0,2)!="01") || (ak.kod.Substring(0,2)!="02"))
                        //                                {
                        //                                    double toplamSerbest=0.0;
                        //                                    toplamAHP=0.0;
                        //                                    for(int j=0;j<3;j++)
                        //                                        toplamAHP+=(double)ayDegerlerAHP[j];
                        //                                    toplamSerbest=tabButce.SerbestBul(yil,kurum,"","",ak.kod);
                        //                                    if (toplamAHP<toplamSerbest)
                        //                                    {
                        //                                        sonuc.islemSonuc=false;
                        //                                        sonuc.hataStr=ak.kod + " Ekonomik kod için ilk 3 Aylık  AHP Toplamınız ilk 3 aylık Serbest toplamından büyük olmalıdır. Lütfen Kontrol Ediniz. İlk 3 aylık Serbest Toplamınız:"+ toplamSerbest.ToString();
                        //                                        break;
                        //                                    }
                        //                                }                    
                        //                            }
                        //                        }
                    }
                }
                else
                    break;
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum ve Aşama bilgilerine göre,
        /// Ayrıntılı Programı Onaylayan Fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Onaylama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc OnaylaAP(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurum, int asama)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABISLForm tabForm = new TABISLForm(ve);
            TABISLAPGider tabDetay = new TABISLAPGider(ve);

            int onayliKod = -1;
            if (kurum == "")
                sonuc.DegerAta(ButceUygulamaResI.BUM046, "");
            //else if (!TNS.BKD.Arac.KurumMu(kurum))
            //    sonuc.DegerAta(ButceUygulamaResI.BUM047, "");
            else if (kullanan == null)
                sonuc.DegerAta(ButceUygulamaResI.BUM048, "");
            else if (!(BUMYetkiServisi.AHPOnaylamaYetkisiVarMi(kullanan, yil, kurum, asama)))
                sonuc.DegerAta(ButceUygulamaResI.BUM049, "");
            else if (!(sonuc = BlokeliTertipKontrol(ve, yil, kurum, asama)).islemSonuc)
            {
            }
            //else if (onayliMi.ButceIslemiOnayKoduBul(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.IHP), asama) != (int)ENUMFormDurum.ONAYLI)
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.DegerAta(ButceUygulamaResI.BUM050, "");
            //}
            //            else if(BUMYetkiServisi.OzelButceliMi(kurum))
            //            {
            //                sonuc=ButceDengesiKontrol(ve,yil,kurum,asama);
            //                
            //            }

            //if (sonuc.islemSonuc)
            //    sonuc = IHPAHPKontrol(ve, yil, kurum, asama);

            if (sonuc.islemSonuc)
            {
                TNSDateTime onaytarih = new TNSDateTime(DateTime.Now);
                onayliKod = tabForm.ButceIslemiOnayKoduBul(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.AHP), asama);
                ve.BeginTransaction();
                int onayKod;
                if (asama == 3)
                    onayKod = (int)ENUMFormDurum.ONAYLI;
                else
                    onayKod = (int)ENUMFormDurum.BUTCEISLEMIPTAL;

                if (onayliKod == 0)
                    sonuc = tabForm.Kaydet(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.AHP), asama, onaytarih,
                           "", "", "", onayKod, "", -1, -1, kullanan.kullaniciKodu, onaytarih, kurum, -1);
                else
                    sonuc = tabForm.OnayKaydet(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.AHP), asama, onayKod, -1, kullanan.kullaniciKodu, onaytarih);
                if (sonuc.islemSonuc)
                    sonuc = tabDetay.OnayKaydet(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.AHP), asama, onayKod);
                if (sonuc.islemSonuc)
                    sonuc = tabDetay.EskiOnaylariDegistir(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.AHP), asama, 7);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    if (asama == 3)
                    {
                        //return ButceIslemServisi.ButceleriHazirla(null, kullanan, yil, kurum, ENUMTabloKod.TABButce);
                    }
                }
                else
                    ve.Rollback();
                //ButceIslemServisi.SerbestHesapla(null, null, yil, kurum, ENUMTabloKod.TABButce);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Dizi ve Tertip bilgilerine göre,
        /// Dizi içerisinde ilgili tertibi arayarak, bulduğu bilgiyi döndüren fonksiyon.
        /// </summary>
        /// <param name="dizi">Dizi bilgisini tutan, ObjectArray türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, AHPBloke türünde nesne döndürür</returns>
        private static AHPBloke DizideTertipBul(ObjectArray dizi, Tertip tertip)
        {
            for (int i = 0; i < dizi.ObjeSayisi; i++)
            {
                AHPBloke b = (AHPBloke)dizi[i];
                if (b.tertip.TertipStrOl() == tertip.TertipStrOl())
                    return b;

            }
            return null;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kurum kodu bilgilerine göre,
        /// Blokeleri listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurKod">Kurumsal kod bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Bloke bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        public static ObjectArray BlokeListele(VeriErisim veriErisim, int yil, string kurKod)
        {
            ObjectArray blokeler = new ObjectArray();
            blokeler.sonuc = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));
            VeriErisim veBloke = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));
            TABButce tbButce = new TABButceGider(ve);
            TABAHPBloke tbBloke = new TABAHPBloke(veBloke);

            string[] kodlar = { kurKod, "", "", "" };
            IDataReader drButce = tbButce.ButceTertipListele(yil, kodlar, true);
            if (drButce != null)
            {
                while (drButce.Read())
                {
                    AHPBloke ahpBloke = new AHPBloke(yil);
                    ahpBloke.tertip.TertipAta(VeriErisim.DegerAl(drButce, 0, ""), VeriErisim.DegerAl(drButce, 1, ""), VeriErisim.DegerAl(drButce, 2, ""), VeriErisim.DegerAl(drButce, 3, ""));
                    ahpBloke.tertip.Noktala();
                    ahpBloke.KBO = VeriErisim.DegerAl(drButce, 4, 0.0);
                    ahpBloke.AHPUst = -1;
                    blokeler.Ekle(ahpBloke);
                }
                drButce.Close();
            }
            else
            {
                blokeler.sonuc.hataStr = ButceUygulamaResI.BUM051 + ":\r\n" + ve.hata;
                blokeler.sonuc.islemSonuc = false;
            }

            if (blokeler.sonuc.islemSonuc)
            {
                IDataReader drBloke = tbBloke.BlokeListele(yil, kurKod);
                if (drBloke != null)
                {
                    while (drBloke.Read())
                    {
                        Tertip tut = new Tertip(yil, ENUMTabloKod.TABButce, VeriErisim.DegerAl(drBloke, 1, ""), VeriErisim.DegerAl(drBloke, 2, ""), VeriErisim.DegerAl(drBloke, 3, ""), VeriErisim.DegerAl(drBloke, 4, ""));
                        tut.Noktala();

                        AHPBloke b = DizideTertipBul(blokeler, tut);
                        if (b != null)
                            b.AHPUst = VeriErisim.DegerAl(drBloke, 5, 0.0);
                    }
                    drBloke.Close();
                }
                else
                {
                    blokeler.sonuc.hataStr = ButceUygulamaResI.BUM051 + ":\r\n" + ve.hata;
                    blokeler.sonuc.islemSonuc = false;
                }
            }
            return blokeler;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum ve Tertip bilgilerine göre,
        /// Bloke kaydeden fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="trpler">Tertipler bilgisini tutan, TNSCollection türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kaydetme işlemininin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public static Sonuc BlokeKaydet(VeriErisim veriErisim, int yil, string kurum, TNSCollection trpler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));
            TABAHPBloke tb = new TABAHPBloke(ve);

            ve.BeginTransaction();

            sonuc = tb.BlokeSil(yil, kurum);

            if (sonuc.islemSonuc)
            {
                foreach (AHPBloke trp in trpler)
                {

                    sonuc = tb.BlokeKaydet(yil, trp.tertip.kodlar, trp.AHPUst);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="veriErisim"></param>
        /// <param name="kullanan"></param>
        /// <param name="yil"></param>
        /// <param name="asama"></param>
        /// <param name="tertip"></param>
        /// <returns></returns>
        public AHPVAN AHPListeleAy(VeriErisim veriErisim, Kullanici kullanan, int yil, int asama, Tertip tertip)
        {
            AHPVAN ahv = new AHPVAN();

            string kurum;
            tertip.NoktalariAt();
            kurum = (string)tertip.kodlar[0];
            string ekoKod = (string)tertip.kodlar[3];

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABButce toplamKbo = new TABButce(ve);

            if (kurum == "")
            {
                ahv.sonuc.hataStr = "Kurum Kodu Girilmesi gerekmektedir.";
                ahv.sonuc.islemSonuc = true;
            }
            else if (kullanan.kullaniciKodu == "")
            {
                ahv.sonuc.hataStr = "Kullanıcı Bilgisi Boş Olamaz ";
                ahv.sonuc.islemSonuc = false;
            }
            else if (ekoKod == "")
            {
                ahv.sonuc.hataStr = "Ekonomik Kod Bilgisi Boş olamaz";
                ahv.sonuc.islemSonuc = false;
            }

            else
            {
                if (!BUMYetkiServisi.AHPKurumGormeYetkisiVarMi(kullanan, yil, kurum, asama))
                {
                    ahv.sonuc.hataStr = "Bu Kurumun Bilgilerini Görem Hakkınız Yok.";
                    ahv.sonuc.islemSonuc = false;
                }

            }

            if (ahv.sonuc.islemSonuc)
            {
                TABISLAPGider ayliklar = new TABISLAPGider(ve);
                //kurum=TNS.BKD.Arac.KurumsaldanKuruma(kurum);

                IDataReader degerler = ayliklar.AHPAylikOdenek(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.AHP), asama, tertip.kodlar);
                if (degerler != null)
                {
                    TertipBilgi tb = new TertipBilgi();
                    tb.ayDegerler = new TNSCollection();

                    for (int j = 0; j < 12; j++)
                        tb.ayDegerler.Add((double)0.0);

                    while (degerler.Read())
                    {
                        int ay = VeriErisim.DegerAl(degerler, 0, 0);
                        if (ay >= 1 && ay <= 12)
                            tb.ayDegerler[ay - 1] = VeriErisim.DegerAl(degerler, 1, 0.0);
                        else
                        {
                            ahv.sonuc.hataStr = "Veritabanından yanlış ay geldi";
                            break;
                        }
                    }
                    degerler.Close();

                    tb.kbo = toplamKbo.kboBul(yil, kurum, "", "", ekoKod);

                    if (ahv.sonuc.hataStr == "")
                        ahv.AHPListe.Add(tb);
                }
                else
                    ahv.sonuc.hataStr = "Veritabanından okuma yapılamadı:\r\n" + ve.hata;
            }

            ve.VeriErisimKapat();

            return ahv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="veriErisim"></param>
        /// <param name="yil"></param>
        /// <returns></returns>
        public ABSKodlarVAN EkoKodListele(VeriErisim veriErisim, int yil)
        {
            IBHMServis servisBHM = TanimlaBHM();
            ABSKodlarVAN gonder = new ABSKodlarVAN();
            ABSKodlarVAN akv = servisBHM.KodListeleSeviye(yil, "4", "", 2);
            for (int i = 0; i < akv.absKodlar.Count; i++)
            {
                ABSKod ak = (ABSKod)akv.absKodlar[i];
                if (ak.kod.Length == 3)
                    gonder.absKodlar.Add(ak);
            }
            gonder.sonuc = akv.sonuc;

            return gonder;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="veriErisim"></param>
        /// <param name="yil"></param>
        /// <param name="kurum"></param>
        /// <param name="islem"></param>
        /// <param name="asama"></param>
        /// <param name="tertip"></param>
        /// <param name="oranKBO"></param>
        /// <param name="tur"></param>
        /// <returns></returns>
        private TNSCollection OranBulAHPOrtalama(VeriErisim veriErisim, int yil, string kurum, string islem, int asama, Tertip tertip, out double oranKBO, int tur)
        {
            TNSCollection donusDeger = new TNSCollection();
            oranKBO = 0.0;
            double[] ay = new double[12];
            double toplamAy1 = 0.0;
            double toplamAy2 = 0.0;
            double kbo;
            donusDeger = OranBulAHP(veriErisim, yil, kurum, islem, 2, tertip, out oranKBO, tur);
            kbo = oranKBO;
            for (int i = 0; i < 12; i++)
            {
                ay[i] = (double)donusDeger[i];
                toplamAy1 += ay[i];
            }
            donusDeger.Clear();
            donusDeger = OranBulAHP(veriErisim, yil - 1, kurum, islem, 2, tertip, out oranKBO, tur);
            for (int i = 0; i < 12; i++)
                toplamAy2 += (double)donusDeger[i];

            if (toplamAy1 > 0.0)
            {
                if (toplamAy2 > 0.0)
                    for (int j = 0; j < 12; j++)
                        ay[j] = (((double)ay[j]) + ((double)donusDeger[j])) / 2;
                else
                    for (int j = 0; j < 12; j++)
                        ay[j] = (double)ay[j];
            }
            else if (toplamAy2 > 0.0)
                for (int j = 0; j < 12; j++)
                    ay[j] = (double)donusDeger[j];

            donusDeger.Clear();
            for (int i = 0; i < 12; i++)
                donusDeger.Add(ay[i]);
            oranKBO = kbo;

            return donusDeger;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="veriErisim"></param>
        /// <param name="yil"></param>
        /// <param name="kurum"></param>
        /// <param name="asama"></param>
        /// <returns></returns>
        private Sonuc ButceDengesiKontrol(VeriErisim veriErisim, int yil, string kurum, int asama)
        {

            Sonuc sonuc = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABISLAPGider tabAHP = new TABISLAPGider(ve);
            TABISLGELIRAP tabAGP = new TABISLGELIRAP(ve);
            TABISLFINANSAP tabAFP = new TABISLFINANSAP(ve);


            TNSCollection ayDegerlerAHP = new TNSCollection();
            TNSCollection ayDegerlerAFP = new TNSCollection();
            TNSCollection ayDegerlerAGP = new TNSCollection();

            for (int j = 0; j < 12; j++)
            {
                ayDegerlerAHP.Add((double)0.0);
                ayDegerlerAFP.Add((double)0.0);
                ayDegerlerAGP.Add((double)0.0);
            }
            IDataReader AHPdegerler = tabAHP.AHPAylikOdenek(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.AHP), asama, "");
            if (AHPdegerler != null)
            {
                while (AHPdegerler.Read())
                {
                    int ay = VeriErisim.DegerAl(AHPdegerler, 0, 0);
                    if (ay >= 1 && ay <= 12)
                        ayDegerlerAHP[ay - 1] = VeriErisim.DegerAl(AHPdegerler, 1, 0.0);
                }
            }
            else
                sonuc.hataStr = "Veritabanından okuma yapılamadı:\r\n" + ve.hata;
            AHPdegerler.Close();

            IDataReader AFPdegerler = tabAFP.AFPAylikOdenek(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.AHP), asama, "");
            if (AFPdegerler != null)
            {
                while (AFPdegerler.Read())
                {
                    int ay = VeriErisim.DegerAl(AFPdegerler, 0, 0);
                    if (ay >= 1 && ay <= 12)
                        ayDegerlerAFP[ay - 1] = VeriErisim.DegerAl(AFPdegerler, 1, 0.0);
                }
            }
            else
                sonuc.hataStr = "Veritabanından okuma yapılamadı:\r\n" + ve.hata;
            AFPdegerler.Close();

            IDataReader AGPdegerler = tabAGP.AGPAylikOdenek(yil, kurum, BISabitler.IslemKod(ENUMIslemKod.AHP), asama, "");

            if (AGPdegerler != null)
            {
                while (AGPdegerler.Read())
                {
                    int ay = VeriErisim.DegerAl(AGPdegerler, 0, 0);
                    if (ay >= 1 && ay <= 12)
                        ayDegerlerAGP[ay - 1] = VeriErisim.DegerAl(AGPdegerler, 1, 0.0);
                }
            }
            else
                sonuc.hataStr = "Veritabanından okuma yapılamadı:\r\n" + ve.hata;
            AGPdegerler.Close();
            if (sonuc.hataStr == "")
            {

                double net = 0.0;
                for (int k = 0; k < 12; k++)
                {
                    net = (double)ayDegerlerAGP[k] + (double)ayDegerlerAFP[k] - (double)ayDegerlerAHP[k];
                    if (net < 0)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = k + 1 + ". Ayda Bütce Dengeniz Tutarlı Değil. Lütfen Kontrol Ediniz.";
                        break;
                    }

                }
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="veriErisim"></param>
        /// <param name="kullanan"></param>
        /// <param name="yil"></param>
        /// <param name="tertip"></param>
        /// <returns></returns>
        public AHPVAN HarcamaListeleAy(VeriErisim veriErisim, Kullanici kullanan, int yil, Tertip tertip)
        {
            AHPVAN ahv = new AHPVAN();

            string kurum;
            tertip.NoktalariAt();
            kurum = (string)tertip.kodlar[0];
            string ekoKod = (string)tertip.kodlar[3];


            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            if (kurum == "")
            {
                ahv.sonuc.hataStr = "Kurum Kodu Girilmesi gerekmektedir.";
                ahv.sonuc.islemSonuc = true;
            }
            else if (kullanan.kullaniciKodu == "")
            {
                ahv.sonuc.hataStr = "Kullanıcı Bilgisi Boş Olamaz ";
                ahv.sonuc.islemSonuc = false;
            }
            else if (ekoKod == "")
            {
                ahv.sonuc.hataStr = "Ekonomik Kod Bilgisi Boş olamaz";
                ahv.sonuc.islemSonuc = false;
            }
            else if (kurum.Length < 8)
            {
                ahv.sonuc.hataStr = "Birim Kodu Girmeniz Gerekmektedir";
                ahv.sonuc.islemSonuc = false;
            }
            else if (ekoKod.Length != 3)
            {
                ahv.sonuc.hataStr = "Ekonomiik Kod 2. Düzeyde Olmalıdır";
                ahv.sonuc.islemSonuc = false;
            }
            else
            {
                //                if(BUMYetkiServisi.AHPKurumGormeYetkisiVarMi(kullanan,yil,kurum))
                //                {
                //                    //Genel Bütçeli Kurumlar için 2. aşaması onaylı olmayan bilgiler kurum kullanıcılarına gösterilmez...
                //					
                //                }
                //                else
                //                {
                //                    ahv.sonuc.hataStr="Bu Kurumun Bilgilerini Görem Hakkınız Yok.";
                //                    ahv.sonuc.islemSonuc=false;
                //                }

            }

            if (ahv.sonuc.islemSonuc)
            {

                TABHarcama ayliklar = new TABHarcama(ve);
                kurum = TNS.BKD.Arac.KurumsaldanKuruma(kurum);
                IDataReader degerler = ayliklar.HarcamaAylikOdenek(yil, tertip.kodlar);

                if (degerler != null)
                {
                    TertipBilgi tb = new TertipBilgi();
                    TABButce tabBut = new TABButce(ve);
                    tb.kbo = tabBut.YSOTBul(yil, tertip.kodlar[0].ToString(), tertip.kodlar[1].ToString(), tertip.kodlar[2].ToString(), tertip.kodlar[3].ToString(), false);
                    tb.ayDegerler = new TNSCollection();


                    for (int j = 0; j < 12; j++)
                        tb.ayDegerler.Add((double)0.0);

                    while (degerler.Read())
                    {

                        int ay = VeriErisim.DegerAl(degerler, 0, 0);
                        if (ay >= 1 && ay <= 12)
                            tb.ayDegerler[ay - 1] = VeriErisim.DegerAl(degerler, 1, 0.0);
                        else
                        {
                            ahv.sonuc.hataStr = "Veritabanından yanlış ay geldi";
                            break;
                        }
                    }
                    if (ahv.sonuc.hataStr == "")
                        ahv.AHPListe.Add(tb);

                }
                else
                    ahv.sonuc.hataStr = "Veritabanından okuma yapılamadı:\r\n" + ve.hata;

            }

            ve.VeriErisimKapat();

            return ahv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="veriErisim"></param>
        /// <param name="yil"></param>
        /// <param name="tertip"></param>
        /// <returns></returns>
        public double kboBul(VeriErisim veriErisim, int yil, Tertip tertip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            TABButce tb = new TABButce(ve);
            double kbo = 0.0;
            kbo = tb.kboBul(yil, tertip.kodlar[0].ToString(), tertip.kodlar[1].ToString(), tertip.kodlar[2].ToString(), tertip.kodlar[3].ToString());
            return kbo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="veriErisim"></param>
        /// <param name="yil"></param>
        /// <param name="asama"></param>
        /// <param name="kurKod"></param>
        /// <returns></returns>
        public Sonuc BlokeliTertipMi(VeriErisim veriErisim, int yil, int asama, string kurKod)
        {
            Sonuc s = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim veAHP = VeriErisim.VeriErisimBul(null);
            TABAHPBloke tbBloke = new TABAHPBloke(ve);
            TABISLAPGider tbAHP = new TABISLAPGider(veAHP);

            IDataReader drBloke = tbBloke.BlokeListele(yil, kurKod);
            if (drBloke != null)
            {
                while (drBloke.Read())
                {
                    Tertip trp = new Tertip();
                    trp.kodlar.Add(OrtakFonksiyonlar.KodNoktaKoy((string)trp.kirilimlar[0], VeriErisim.DegerAl(drBloke, 1, "")));
                    trp.kodlar.Add(OrtakFonksiyonlar.KodNoktaKoy((string)trp.kirilimlar[1], VeriErisim.DegerAl(drBloke, 2, "")));
                    trp.kodlar.Add(OrtakFonksiyonlar.KodNoktaKoy((string)trp.kirilimlar[2], VeriErisim.DegerAl(drBloke, 3, "")));
                    trp.kodlar.Add(OrtakFonksiyonlar.KodNoktaKoy((string)trp.kirilimlar[3], VeriErisim.DegerAl(drBloke, 4, "")));

                    double toplam = tbAHP.TertipToplamiBul(yil, "01.01", asama, trp.kodlar);
                    if (toplam != 0)
                    {
                        s.hataStr = trp.TertipStrOl() + " tertibi bloke tutulması gerekirken değer girilmiş.";
                        s.islemSonuc = false;
                        break;
                    }

                }
                drBloke.Close();
            }
            else
            {
                s.hataStr = " Veritabanı okuma hatası\r\n" + ve.hata;
                s.islemSonuc = false;
            }

            ve.VeriErisimKapat();
            veAHP.VeriErisimKapat();
            return s;

        }
         * */
    }//AylikHarcamaServisi sonu
}//name space sonu



