using System;
using System.Collections;
using System.Data;
using TNS.DEG;
using TNS.KYM;
using TNS.MUH;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Taşınır işlem fişinin üst bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMTasinirIslemForm
    {
        private static Sonuc YilDevriKontroluYap(TasinirIslemForm tf, string hesapPlanKod)
        {
            Sonuc sonuc = new Sonuc();

            int yilDevriKontrolYil = 2012;//OrtakFonksiyonlar.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings.Get("YilDevriKontrolYil"), 0);
            if (tf.yil < yilDevriKontrolYil || yilDevriKontrolYil == 0)
                return sonuc;

            string oncekiYilKosul = tf.yil - 1 + "--" + tf.muhasebeKod + "-" + tf.harcamaKod + "-" + tf.ambarKod + "-" + hesapPlanKod + "-";
            string mevcutYilKosul = tf.yil + "--" + tf.muhasebeKod + "-" + tf.harcamaKod + "-" + tf.ambarKod + "-" + hesapPlanKod + "-";

            double girenMiktar = TMMServis.UzaySayiDegerGetir("TASTASINIRGIRENMIKTAR", oncekiYilKosul, false, string.Empty);
            double cikanMiktar = TMMServis.UzaySayiDegerGetir("TASTASINIRCIKANMIKTAR", oncekiYilKosul, false, string.Empty);
            if (girenMiktar != cikanMiktar)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = string.Format(TasinirMalResI.TMM320, tf.muhasebeKod + "-" + tf.harcamaKod + "-" + tf.ambarKod, tf.yil - 1, tf.yil - 1, (girenMiktar - cikanMiktar).ToString("#,###"));
                return sonuc;
            }

            if (tf.islemTipTur == (int)ENUMIslemTipi.YILDEVIRGIRIS)
                return sonuc;

            double girenMiktarYilDevri = TMMServis.UzaySayiDegerGetir("TASTASINIRGIRENMIKTAR", mevcutYilKosul + (int)ENUMIslemTipi.YILDEVIRGIRIS, false, string.Empty);
            double cikanMiktarYilDevri = TMMServis.UzaySayiDegerGetir("TASTASINIRCIKANMIKTAR", oncekiYilKosul + (int)ENUMIslemTipi.YILDEVIRCIKIS, false, string.Empty);
            if (girenMiktarYilDevri != cikanMiktarYilDevri)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = string.Format(TasinirMalResI.TMM321, tf.muhasebeKod + "-" + tf.harcamaKod + "-" + tf.ambarKod, tf.yil, tf.yil, (girenMiktarYilDevri - cikanMiktarYilDevri).ToString("#,###"));
                return sonuc;
            }

            return sonuc;
        }

        private static Sonuc DevirGirisCikisMalzemeleriAyniMi(VeriErisim veriErisim, TasinirIslemForm tf)
        {
            if (tf.islemTipTur != (int)ENUMIslemTipi.DEVIRGIRIS && tf.islemTipTur != (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                return new Sonuc();

            TasinirIslemForm tfDevir = new TasinirIslemForm();
            tfDevir.yil = tf.gYil;
            tfDevir.muhasebeKod = tf.gMuhasebeKod;
            tfDevir.harcamaKod = tf.gHarcamaKod;
            tfDevir.fisNo = tf.gFisNo;

            Kullanici kullanan = new Kullanici();
            kullanan.kullaniciKodu = "stratek";
            kullanan.kullaniciTipi = new ENUMKullaniciTipi[1];
            kullanan.kullaniciTipi[0] = ENUMKullaniciTipi.SISTEMYONETICISI;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectArray devirCikisFis = FisAc(ve, kullanan, tfDevir, true);
            ve.VeriErisimKapat();

            if (!devirCikisFis.sonuc.islemSonuc || devirCikisFis.objeler.Count <= 0)
                return devirCikisFis.sonuc;

            tfDevir = (TasinirIslemForm)devirCikisFis.objeler[0];
            if (tf.detay.objeler.Count != tfDevir.detay.objeler.Count)
                return new Sonuc(false, TasinirMalResI.TMM324, string.Empty);

            for (int i = 0; i < tf.detay.objeler.Count; i++)
            {
                TasinirIslemDetay girisDetay = (TasinirIslemDetay)tf.detay.objeler[i];
                TasinirIslemDetay cikisDetay = (TasinirIslemDetay)tfDevir.detay.objeler[i];

                if (girisDetay.hesapPlanKod.Replace(".", "") != cikisDetay.hesapPlanKod.Replace(".", "") ||
                    girisDetay.miktar != cikisDetay.miktar ||
                    girisDetay.kdvOran != cikisDetay.kdvOran ||
                    System.Math.Round(girisDetay.birimFiyat, 10) != System.Math.Round(cikisDetay.birimFiyat, 10) ||
                    girisDetay.eAciklama != cikisDetay.eAciklama)
                {
                    //if (girisDetay.miktar != cikisDetay.miktar)
                    //    OrtakFonksiyonlar.HataDosyaYaz("DevirCikisGiris.txt", "Miktar farklı:" + girisDetay.miktar + "," + cikisDetay.miktar, false);
                    //if (girisDetay.kdvOran != cikisDetay.kdvOran)
                    //    OrtakFonksiyonlar.HataDosyaYaz("DevirCikisGiris.txt", "KDV farklı:" + girisDetay.kdvOran + "," + cikisDetay.kdvOran, false);
                    //if (girisDetay.birimFiyat != cikisDetay.birimFiyat)
                    //    OrtakFonksiyonlar.HataDosyaYaz("DevirCikisGiris.txt", "BirimFiyat farklı:" + girisDetay.birimFiyat + "," + cikisDetay.birimFiyat, false);
                    //if (girisDetay.eAciklama != cikisDetay.eAciklama)
                    //    OrtakFonksiyonlar.HataDosyaYaz("DevirCikisGiris.txt", "EAciklama farklı:" + girisDetay.eAciklama + "," + cikisDetay.eAciklama, false);
                    //if (girisDetay.hesapPlanKod.Replace(".", "") != cikisDetay.hesapPlanKod.Replace(".", ""))
                    //    OrtakFonksiyonlar.HataDosyaYaz("DevirCikisGiris.txt", "HesapPlanKod farklı:" + girisDetay.hesapPlanKod + "," + cikisDetay.hesapPlanKod, false);

                    return new Sonuc(false, TasinirMalResI.TMM324, string.Empty);
                }
            }

            return new Sonuc(true, string.Empty, string.Empty);
        }

        /// <summary>
        /// Amortisman entegrasyonunun olup olmadığını tutan değişken
        /// </summary>
        private static string amortismanDeger = System.Configuration.ConfigurationManager.AppSettings.Get("Amortisman");

        /// <summary>
        /// Parametre olarak verilen taşınır işlem formu bilgilerinin diğer
        /// parametrelerle birlikte doğruluğunu kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem formu bilgilerini tutan nesne</param>
        /// <param name="durumKontrol">Belge durumuyla ilgili kontrollerin yapılıp yapılmayacağı bilgisi</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc Kontrol(VeriErisim ve, Kullanici kullanan, TasinirIslemForm tf, bool durumKontrol)
        {
            Sonuc sonuc = new Sonuc();
            Sonuc sonucDetay = new Sonuc();
            int islemTur = 0;
            IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            if (TNS.TMM.Arac.MerkezBankasiKullaniyor() && tf.islemTipKod == (int)ENUMIslemTipi.GECICIIHRAC)
            {
                islemTur = (int)ENUMIslemTipi.DEVIRCIKISKURUM;
                tf.islemTipTur = islemTur;
            }
            else
                islemTur = (int)TMMServis.UzaySayiDegerGetir("TASISLEMTIPTUR", tf.islemTipKod.ToString(), true, "");

            if (tf.yil == 0)
                sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";
            if (tf.muhasebeKod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";
            if (tf.harcamaKod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";
            if (tf.ambarKod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM031 + "<br>";
            if (tf.fisTarih.isNull)
                sonuc.hataStr += TasinirMalResI.TMM077 + "<br>";
            if (tf.islemTipKod == 0)
                sonuc.hataStr += TasinirMalResI.TMM249 + "<br>";

            if (tf.fisTarih.Oku().Year != tf.yil && !durumKontrol)
                sonuc.hataStr += string.Format(TasinirMalResI.TMM250, tf.yil, tf.fisTarih.ToString()) + "<br>";

            if (tf.muhasebeKod != String.Empty)
            {
                if (TMMServis.UzayYaziDegerGetir("TASMUHASEBE", tf.muhasebeKod, true, "") == String.Empty)
                    sonuc.hataStr += string.Format(TasinirMalResI.TMM096, tf.muhasebeKod) + "<br>";
            }
            if (tf.muhasebeKod != String.Empty && tf.harcamaKod != String.Empty)
            {
                if (TMMServis.UzayYaziDegerGetir("TASHARCAMABIRIM", tf.muhasebeKod + "-" + tf.harcamaKod, true, "") == String.Empty)
                    sonuc.hataStr += string.Format(TasinirMalResI.TMM097, tf.muhasebeKod, tf.harcamaKod) + "<br>";
            }
            if (tf.muhasebeKod != String.Empty && tf.harcamaKod != String.Empty && tf.ambarKod != String.Empty)
            {
                if (TMMServis.UzayYaziDegerGetir("TASAMBAR", tf.muhasebeKod + "-" + tf.harcamaKod + "-" + tf.ambarKod, true, "") == String.Empty)
                {
                    if (tf.islemTipTur != (int)ENUMIslemTipi.ENFLASYONARTISI)
                        sonuc.hataStr += string.Format(TasinirMalResI.TMM098, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod) + "<br>";
                }
            }

            if (sonuc.hataStr == "")
            {
                TNSCollection kDeg = null;
                string degiskenAd = "KAPAT" + "_" + tf.yil + "_" + tf.muhasebeKod + "_" + tf.harcamaKod.Replace(".", "") + "_" + tf.ambarKod;
                string degisken = "";

                kDeg = servisKYM.DegiskenleriListele("stratek", degiskenAd);

                if (kDeg != null && kDeg.Count > 0)
                    degisken = kDeg[0].ToString();

                if (degisken != "")
                    sonuc.hataStr = string.Format(TasinirMalResI.TMM251, tf.yil) + "<br>";
            }



            if (Arac.MerkezBankasiKullaniyor())
            {
                if (sonuc.hataStr == "")
                {
                    int amortismanDonem = TMMAmortisman.AmortismanDonemiBul(tf.harcamaKod, tf.fisTarih);

                    //Geçerli dönem için amortisman yapılmış ise yeni fiş kaydı, ihraç gibi işlemler yapılamasın.
                    bool amortismanYapilmis = TMMAmortismanIslemForm.AmortismanYapilmisMi(ve, new AmortismanKriter()
                    {
                        yil = tf.yil,
                        donem = amortismanDonem,
                        muhasebeKod = tf.muhasebeKod,
                        harcamaKod = tf.harcamaKod,
                        ambarKod = tf.ambarKod
                    });

                    if (amortismanYapilmis)
                        sonuc.hataStr = string.Format("{0} Birimi - {1} Ambarı - {2} yılı {3}.dönem'e ait amortisman kaydı oluşturulduğu için bu döneme ait kayıt yapılamaz. <br><br><b>Not:</b> Amortismanın B/A onayı tamamlanmadan amortismanı iptal edip yeni kayıt girişi yapabilirsiniz.", tf.harcamaKod, tf.ambarKod, tf.yil, amortismanDonem) + "<br>";
                }

                if (tf.fisTarih.Oku().Date > DateTime.Now.Date)
                    sonuc.hataStr += "İleri tarihli fiş girişi yapılamaz.<br>";



                //Banknot matbaasi için devir giriş/çıkış işleminde %100 amortisman olmamış kayıtlar için aşağıdaki kural çalıştırılacak.
                if (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS || tf.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRISKURUM)
                {
                    if (MerkezBankasiGenel.Ayarlar.BanknotBirimiMi(tf.harcamaKod) || MerkezBankasiGenel.Ayarlar.BanknotBirimiMi(tf.gHarcamaKod))
                    {
                        //Devir giriş/çıkış birimi için son amortisman nezaman yapılmış
                        AmortismanIslemForm sonAmortisman = TMMAmortismanIslemForm.SonAmortisman(ve, kullanan, new AmortismanIslemForm()
                        {
                            muhasebeKod = tf.gMuhasebeKod,
                            harcamaKod = tf.gHarcamaKod,
                            ambarKod = tf.gAmbarKod
                        });


                        bool farkliDonemlereAitAmortismanlarVar = false;

                        foreach (TasinirIslemDetay detay in tf.detay.objeler)
                        {
                            var prSicilNo = TMM.IsKurallari.Genel.PrSicilNoGetir(detay.yil, tf.muhasebeKod, tf.harcamaKod.Replace(".", ""), detay.gorSicilNo);

                            if (prSicilNo > 0)
                            {
                                var ab = TMMAmortisman.AmortismanBilgileri(ve, kullanan, prSicilNo, tf.muhasebeKod, tf.harcamaKod, 0);

                                if (ab.buAmartismanYuzdesi > 0 && ab.buAmartismanYuzdesi != 100.00)
                                {
                                    if (ab.sonAmortismanYil == DateTime.Now.Year && !(ab.sonAmortismanDonem == 3 || ab.sonAmortismanDonem == 6 || ab.sonAmortismanDonem == 9 || ab.sonAmortismanDonem == 12))
                                    {
                                        sonuc.hataStr += "<b>" + prSicilNo + "</b> nolu demirbaşa ait son amortisman <b>" + ab.sonAmortismanDonem + ".</b> dönemde yapılmıştır. <br>";
                                        farkliDonemlereAitAmortismanlarVar = true;
                                    }
                                    else if (sonAmortisman.yil != ab.sonAmortismanYil || sonAmortisman.donem != ab.sonAmortismanDonem)
                                    {
                                        sonuc.hataStr += "<b>" + tf.gHarcamaKod + "</b> nolu birimde <b>" + sonAmortisman.donem + ".</b> döneme ait amortisman yapıldığı için devir giriş/çıkış yapılamaz. <br>";
                                        farkliDonemlereAitAmortismanlarVar = true;
                                    }
                                }
                            }
                        }

                        if (farkliDonemlereAitAmortismanlarVar)
                        {
                            sonuc.hataStr += "Banknot matbaası ile yukarıdaki demirbaşların amortisman dönemlerindeki uyumsuzluktan dolayı <br><b>devir giriş/çıkış</b> işlemi yapılamaz.<br>";

                            if (!(tf.fisTarih.Oku().Date.Month == 1 || tf.fisTarih.Oku().Date.Month == 4 || tf.fisTarih.Oku().Date.Month == 7 || tf.fisTarih.Oku().Date.Month == 10))
                                sonuc.hataStr += "<br><b>Not:</b><br>Banknot matbaası ile amortismanı tamamlanmamış demirbaşlar için <b>1, 4, 7 ve 10.</b> aylarda <b>devir giriş/çıkış</b> işlemi yapılmalıdır.<br>";
                        }
                    }
                }
                //---------------------------------------------------------------------------------


                bool fisGirisiYapilabilir = true;
                bool mutemmimGirisYapilabilir = true;

                //********
                AyarlarMB form = new AyarlarMB
                {
                    muhasebeKod = tf.muhasebeKod,
                    harcamaKod = tf.harcamaKod,
                    ambarKod = tf.ambarKod
                };

                ObjectArray liste = TMMAyarlarMB.Listele(ve, kullanan, form);
                if (liste.sonuc.islemSonuc && liste.objeler.Count == 1)
                {
                    AyarlarMB a = (AyarlarMB)liste.objeler[0];
                    if (!a.mutemmimBitisTarihi.isNull && DateTime.Now.Date > a.mutemmimBitisTarihi.Oku())
                        mutemmimGirisYapilabilir = false;

                    if (!a.fisGirisKapanisTarihi.isNull && DateTime.Now.Date > a.fisGirisKapanisTarihi.Oku())
                        fisGirisiYapilabilir = false;
                }

                if (!fisGirisiYapilabilir)
                    sonuc.hataStr += "Fiş girişi sistem yöneticisi tarafından kapatılmıştır.<br>";

                if (!mutemmimGirisYapilabilir)
                    sonuc.hataStr += "Mütemmim girişi açık değildir.<br>Lütfen sistem yöneticisine başvurunuz.<br>";

                //if (tf.mutemmim == 1 && !mutemmimGirisYapilabilir)
                //sonuc.hataStr += "Mütemmim girişi " + tf.ambarKod + " birimi için açık değildir.<br>Lütfen sistem yöneticisine başvurunuz.<br>";

                //********
            }


            TABTasinirIslemForm tabTasinirIslemForm = new TABTasinirIslemForm(ve);


            //Meclis, Yıl devri yapılmadan yeni fiş kaydı yapılmasını istemiyor.
            if (Arac.MeclisKullaniyor() && islemTur != (int)ENUMIslemTipi.YILDEVIRGIRIS && tf.yil == DateTime.Now.Year)
            {
                bool yilDevriYapilmis = false;

                TasinirIslemForm tfKriter = new TasinirIslemForm();
                tfKriter.yil = tf.yil;
                tfKriter.muhasebeKod = tf.muhasebeKod;
                tfKriter.harcamaKod = tf.harcamaKod;
                tfKriter.ambarKod = tf.ambarKod;

                IDataReader r = tabTasinirIslemForm.YilDevriGirisKaydiYapilmismi(tfKriter);
                if (r.Read())
                {
                    yilDevriYapilmis = true;
                }
                r.Close();

                if (!yilDevriYapilmis)
                {
                    DepoDurum dpkriter = new DepoDurum();
                    dpkriter.yil = tf.yil - 1;
                    dpkriter.muhasebeKod = tf.muhasebeKod;
                    dpkriter.harcamaKod = tf.harcamaKod;
                    dpkriter.ambarKod = tf.ambarKod;
                    dpkriter.yilDevri = true;
                    dpkriter.hesapPlanKod = "150";
                    ObjectArray liste = TMMRaporlar.TasinirDepoDurumu(ve, kullanan, dpkriter);

                    yilDevriYapilmis = true;

                    foreach (DepoDurum dpi in liste.objeler)
                    {
                        if (dpi.kalanMiktar > 0)
                        {
                            yilDevriYapilmis = false;
                            break;
                        }
                    }
                }


                if (!yilDevriYapilmis)
                {
                    sonuc.hataStr += "Önceki yıldaki tüketim malzemelerinin Yıl devri işlemi yapılmadan yeni yıla TİF kaydı yapılamaz" + "<br>";
                }
            }


            if (islemTur == (int)ENUMIslemTipi.SATINALMAGIRIS ||
                islemTur == (int)ENUMIslemTipi.BAGISGIRIS ||
                islemTur == (int)ENUMIslemTipi.IADEGIRIS ||
                islemTur == (int)ENUMIslemTipi.DEVIRGIRISKURUM)
            {
                if (tf.neredenGeldi == string.Empty)
                    sonuc.hataStr += TasinirMalResI.TMM252 + "<br>";

                if (islemTur == (int)ENUMIslemTipi.SATINALMAGIRIS &&
                    tf.faturaNo.Trim() != "" && !tf.faturaTarih.isNull &&
                    !durumKontrol)
                {
                    IDataReader r = tabTasinirIslemForm.SatinAlmaKaydiYapilmismi(tf);
                    if (r.Read())
                    {
                        int yil = VeriErisim.DegerAl(r, 0, 0);
                        string fisNo = VeriErisim.DegerAl(r, 1, "");
                        sonuc.hataStr += string.Format(TasinirMalResI.TMM253, yil, fisNo) + "<br>";
                    }
                    r.Close();
                }
            }
            else if (islemTur == (int)ENUMIslemTipi.TUKETIMCIKIS ||
                     islemTur == (int)ENUMIslemTipi.BAGISCIKIS ||
                     islemTur == (int)ENUMIslemTipi.SATISCIKIS ||
                     islemTur == (int)ENUMIslemTipi.KULLANILMAZCIKIS ||
                     islemTur == (int)ENUMIslemTipi.HURDACIKIS ||
                     islemTur == (int)ENUMIslemTipi.DEVIRCIKISKURUM)// ||
            //islemTur == (int)ENUMIslemTipi.SAYIMNOKSANICIKIS)
            {
                if (tf.nereyeGitti == string.Empty && tf.kimeGitti == string.Empty)
                    sonuc.hataStr += TasinirMalResI.TMM254 + "<br>";
            }
            else if (islemTur == (int)ENUMIslemTipi.DEVIRCIKIS ||
                     islemTur == (int)ENUMIslemTipi.DEVIRGIRIS ||
                     islemTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS ||
                     islemTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
            {
                if (tf.gMuhasebeKod == string.Empty)
                    sonuc.hataStr += TasinirMalResI.TMM255 + "<br>";
                if (tf.gHarcamaKod == string.Empty)
                    sonuc.hataStr += TasinirMalResI.TMM256 + "<br>";
                if (tf.gAmbarKod == string.Empty && (islemTur == (int)ENUMIslemTipi.DEVIRGIRIS || islemTur == (int)ENUMIslemTipi.DEVIRCIKIS))
                {
                    //***Kullanıcı birimi şeklinde çalışıyor ise devir ambarını gösterme*****************************
                    int devirSekli = OrtakFonksiyonlar.ConvertToInt(TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRKULLANICIBIRIMI"), 0);
                    if (devirSekli > 0)
                        tf.gAmbarKod = "";//Kullanıcı birimi olarak çalışıyor ise gönderilecek ambar gösterilmiyor.
                    else
                        sonuc.hataStr += TasinirMalResI.TMM257 + "<br>";
                }

                if (tf.gMuhasebeKod != String.Empty)
                {
                    if (TMMServis.UzayYaziDegerGetir("TASMUHASEBE", tf.gMuhasebeKod, true, "") == String.Empty)
                        sonuc.hataStr += string.Format(TasinirMalResI.TMM258, tf.gMuhasebeKod) + "<br>";
                }
                if (tf.gMuhasebeKod != String.Empty && tf.gHarcamaKod != String.Empty)
                {
                    if (TMMServis.UzayYaziDegerGetir("TASHARCAMABIRIM", tf.gMuhasebeKod + "-" + tf.gHarcamaKod, true, "") == String.Empty)
                        sonuc.hataStr += string.Format(TasinirMalResI.TMM259, tf.gMuhasebeKod, tf.gHarcamaKod) + "<br>";
                }
                if (tf.gMuhasebeKod != String.Empty && tf.gHarcamaKod != String.Empty && tf.gAmbarKod != String.Empty)
                {
                    if (TMMServis.UzayYaziDegerGetir("TASAMBAR", tf.gMuhasebeKod + "-" + tf.gHarcamaKod + "-" + tf.gAmbarKod, true, "") == String.Empty)
                        sonuc.hataStr += string.Format(TasinirMalResI.TMM260, tf.gMuhasebeKod, tf.gHarcamaKod, tf.gAmbarKod) + "<br>";
                }

                if (tf.muhasebeKod == tf.gMuhasebeKod)
                    if (tf.harcamaKod == tf.gHarcamaKod)
                        if (tf.ambarKod == tf.gAmbarKod)
                            sonuc.hataStr += TasinirMalResI.TMM261 + "<br>";

                if (islemTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS || islemTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS)
                {
                    //İşlem yapılan ambarlardan en az bir tanesi normal ambar olmalı. 2 ambarda kullanıcı birimi olamaz.
                    double ambarTip1 = TMMServis.UzaySayiDegerGetir("TASAMBAR", tf.muhasebeKod + "-" + tf.harcamaKod + "-" + tf.ambarKod, true, "KULLANICIBIRIMI");
                    double ambarTip2 = TMMServis.UzaySayiDegerGetir("TASAMBAR", tf.gMuhasebeKod + "-" + tf.gHarcamaKod + "-" + tf.gAmbarKod, true, "KULLANICIBIRIMI");
                    if (ambarTip1 > 0 && ambarTip2 > 0)
                        sonuc.hataStr += TasinirMalResI.TMM328 + "<br>";
                }

                if ((islemTur == (int)ENUMIslemTipi.DEVIRGIRIS || islemTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS) && sonuc.hataStr == "" && !durumKontrol)
                {
                    tf.gFisNo = tf.gFisNo.PadLeft(6, '0');
                    IDataReader r = tabTasinirIslemForm.DevirGirisKaydiYapilmismi(tf);
                    if (r.Read())
                    {
                        int yil = VeriErisim.DegerAl(r, 0, 0);
                        string fisNo = VeriErisim.DegerAl(r, 1, "");
                        sonuc.hataStr += string.Format(TasinirMalResI.TMM262, yil, fisNo) + "<br>";
                    }
                    r.Close();

                    if (sonuc.hataStr == "")
                    {
                        TasinirIslemForm tDevir = new TasinirIslemForm();
                        tDevir.yil = tf.gYil;
                        tDevir.muhasebeKod = tf.gMuhasebeKod;
                        tDevir.harcamaKod = tf.gHarcamaKod;

                        //***Kullanıcı birimi şeklinde çalışıyor ise devir ambarını gösterme*****************************
                        int devirSekli = OrtakFonksiyonlar.ConvertToInt(TNS.TMM.Arac.DegiskenDegerBul(0, "/TASINIRKULLANICIBIRIMI"), 0);
                        if (devirSekli > 0)
                            tDevir.ambarKod = "";
                        else
                            tDevir.ambarKod = tf.gAmbarKod;

                        tDevir.fisNo = tf.gFisNo.PadLeft(6, '0');

                        r = tabTasinirIslemForm.FisAc(tDevir);
                        if (r.Read())
                        {
                            tDevir.gMuhasebeKod = VeriErisim.DegerAl(r, 13, string.Empty);
                            tDevir.gHarcamaKod = VeriErisim.DegerAl(r, 14, string.Empty);
                            tDevir.gAmbarKod = VeriErisim.DegerAl(r, 15, string.Empty);

                            if (tDevir.gMuhasebeKod != tf.muhasebeKod ||
                                tDevir.gHarcamaKod != tf.harcamaKod.Replace(".", "") ||
                                (!string.IsNullOrEmpty(tDevir.gAmbarKod) && tDevir.gAmbarKod != tf.ambarKod))
                                sonuc.hataStr += TasinirMalResI.TMM263 + "<br>";

                            //Onaylanmış mı?
                            tDevir.durum = VeriErisim.DegerAl(r, 16, 0);
                            if (tDevir.durum != (int)ENUMBelgeDurumu.ONAYLI)
                                sonuc.hataStr += TasinirMalResI.TMM330 + "<br>";
                        }
                        else
                            sonuc.hataStr += TasinirMalResI.TMM264 + "<br>";

                        r.Close();
                    }
                }
            }
            else if (islemTur == (int)ENUMIslemTipi.YILDEVIRGIRIS && sonuc.hataStr == "" && !durumKontrol)
            {
                IDataReader r = tabTasinirIslemForm.YilDevriGirisKaydiYapilmismi(tf);
                if (r.Read())
                {
                    int yil = VeriErisim.DegerAl(r, 0, 0);
                    string fisNo = VeriErisim.DegerAl(r, 1, "");
                    sonuc.hataStr += string.Format(TasinirMalResI.TMM262, yil, fisNo) + "<br>";
                }
                r.Close();

                if (sonuc.hataStr == "")
                {
                    TasinirIslemForm tDevir = new TasinirIslemForm();
                    tDevir.yil = tf.gYil;
                    tDevir.muhasebeKod = tf.muhasebeKod;
                    tDevir.harcamaKod = tf.harcamaKod;
                    tDevir.ambarKod = tf.ambarKod;
                    tDevir.fisNo = tf.gFisNo.PadLeft(6, '0');

                    r = tabTasinirIslemForm.FisAc(tDevir);
                    if (r.Read())
                    {
                        int durum = VeriErisim.DegerAl(r, 16, 0);
                        int islemTipKod = VeriErisim.DegerAl(r, 6, 0);
                        islemTipKod = OrtakFonksiyonlar.ConvertToInt(TMMServis.UzaySayiDegerGetir("TASISLEMTIPTUR", islemTipKod.ToString(), true, ""), 0);
                        if (islemTipKod != (int)ENUMIslemTipi.YILDEVIRCIKIS)
                            sonuc.hataStr += TasinirMalResI.TMM265 + "<br>";
                        if (durum != (int)ENUMBelgeDurumu.ONAYLI)
                            sonuc.hataStr += TasinirMalResI.TMM266 + "<br>";
                    }
                    else
                        sonuc.hataStr += TasinirMalResI.TMM267 + "<br>";
                    r.Close();
                }
            }

            if (tf.dayanakTarih.CompareTo(tf.fisTarih) == 1)
                sonuc.hataStr += TasinirMalResI.TMM268 + "<br>";

            //Detay kontrol
            if (tf.detay.ObjeSayisi < 1)
                sonuc.hataStr += TasinirMalResI.TMM269 + "<br>";
            if (!durumKontrol)
                sonucDetay = TMMTasinirIslemDetay.Kontrol(kullanan, tf.detay, islemTur);

            //Onay kontrol
            if (tf.fisNo != string.Empty && !durumKontrol) //Update
            {
                IDataReader dr = tabTasinirIslemForm.BelgeDurumGetir(tf);
                if (!dr.Read())
                    sonuc.hataStr += string.Format(TasinirMalResI.TMM270, tf.fisNo) + "<br>";
                else
                {
                    int durum = VeriErisim.DegerAl(dr, 0, 0);
                    if (durum == (int)ENUMBelgeDurumu.ONAYLI)
                        sonuc.hataStr += string.Format(TasinirMalResI.TMM271, tf.fisNo) + "<br>";
                }
                dr.Close();
                tf.durum = (int)ENUMBelgeDurumu.DEGISTIRILDI;

                //B-A Onay Kontrolü
                if (Arac.MerkezBankasiKullaniyor())
                {
                    ENUMTasinirIslemFormOnayDurumu onayDurum = (ENUMTasinirIslemFormOnayDurumu)tabTasinirIslemForm.OnayDurumGetirTekKolon(tf);
                    if (onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIB)
                        sonuc.hataStr += string.Format("{0} no\'lu belge B Onayınını beklediği için değişiklik yapılamaz.", tf.fisNo) + "<br>";
                    else if (onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIA)
                        sonuc.hataStr += string.Format("{0} no\'lu belge A Onayını beklediği için değişiklik yapılamaz.", tf.fisNo) + "<br>";
                }
            }
            else if (!durumKontrol)
                tf.durum = (int)ENUMBelgeDurumu.YENI;

            sonuc.hataStr += sonucDetay.hataStr;

            if (sonuc.hataStr != String.Empty)
                sonuc.islemSonuc = false;

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Taşınır işlem fişini varsa güncelleyen, yoksa kaydeden yordam
        /// Taşınır işlem fişi ve detay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf)
        {
            Sonuc sonuc = VTAlanUzunlukKontrolEt(tf);
            if (!sonuc.islemSonuc)
                return sonuc;

            if (tf.parselNo == "YetkiyeBakmadanKaydet")
                tf.parselNo = "";
            else
            {
                sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);

                //KUllanıcı biriminde yetkili olan kişi
                if (!sonuc.islemSonuc && (tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS))
                    sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiKullaniciBirimi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);

                //ht:
                //yıl devri giriş ve çıkış fişlerine müdahale etme yetkisi sadece
                //yetkinin verildiği kişilerde olsun 
                if (tf.islemTipTur == (int)ENUMIslemTipi.YILDEVIRGIRIS || tf.islemTipTur == (int)ENUMIslemTipi.YILDEVIRCIKIS)
                    sonuc.islemSonuc = TMMYetkiKontrol.YilDevriGirisCikisYetkisiVarmi(kullanan);

                //if (!sonuc.islemSonuc && tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)//DağıtımGiriş ise kontrol etme
                //    sonuc.islemSonuc = true;

            }

            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM050;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve3 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            sonuc = Kontrol(ve, kullanan, tf, false);
            if (sonuc.hataStr != String.Empty)
            {
                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
                ve3.VeriErisimKapat();
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Sadece kontrol et ve çık
            //ve.VeriErisimKapat();
            //ve2.VeriErisimKapat();
            //ve3.VeriErisimKapat();
            //sonuc.islemSonuc = false;
            //return sonuc;

            sonuc = DevirGirisCikisMalzemeleriAyniMi(ve3, tf);
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
                ve3.VeriErisimKapat();
                return sonuc;
            }

            TABTasinirIslemForm tabTasinirIslemForm = new TABTasinirIslemForm(ve);
            TABGenel tabGenel = new TABGenel(ve2);

            ve.BeginTransaction();

            if (tf.fisNo == string.Empty)
            {
                string sayacKod = "TIF" + tf.yil.ToString() + tf.muhasebeKod + tf.harcamaKod.Replace(".", "");

                if (tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                {
                    sayacKod = "DAG" + tf.yil.ToString() + tf.muhasebeKod + tf.harcamaKod.Replace(".", "");
                    tf.fisNo = tabGenel.BelgeNoAl(sayacKod, "000000");
                    tf.fisNo = "D" + tf.fisNo;
                }
                else
                    tf.fisNo = tabGenel.BelgeNoAl(sayacKod, "000000");

                if (tf.fisNo == string.Empty)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TasinirMalResI.TMM083;
                    ve.VeriErisimKapat();
                    ve2.VeriErisimKapat();
                    ve3.VeriErisimKapat();
                    return sonuc;
                }
            }
            else
                tf.durum = (int)ENUMBelgeDurumu.DEGISTIRILDI;

            if (string.IsNullOrEmpty(tf.kod))
                tf.kod = Genel.KodAl();

            tf.ipAdresi = kullanan.telefon;
            sonuc = tabTasinirIslemForm.Kaydet(tf);

            if (!sonuc.islemSonuc)
            {
                OrtakFonksiyonlar.HataDosyaYaz("TifHata.txt", "*********************************");
                OrtakFonksiyonlar.HataDosyaYaz("TifHata.txt", ve.hata);
                OrtakFonksiyonlar.HataDosyaYaz("TifHata.txt", "*********************************");

                ve.Rollback();
                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
                ve3.VeriErisimKapat();
                sonuc.vtHatasi = true;
                return sonuc;
            }

            if (tf.durum != (int)ENUMBelgeDurumu.YENI)
            {
                //Bu fişe ait satırları sil
                sonuc = TMMTasinirIslemDetay.Sil(ve, kullanan, tf);

                if (!sonuc.islemSonuc)
                {
                    OrtakFonksiyonlar.HataDosyaYaz("TifHata.txt", "*********************************");
                    OrtakFonksiyonlar.HataDosyaYaz("TifHata.txt", ve.hata);
                    OrtakFonksiyonlar.HataDosyaYaz("TifHata.txt", "*********************************");

                    ve.Rollback();
                    ve.VeriErisimKapat();
                    ve2.VeriErisimKapat();
                    ve3.VeriErisimKapat();
                    sonuc.vtHatasi = true;
                    return sonuc;
                }
            }

            //Sicil Durum bilgisi yazmak için 
            //if (Arac.MerkezBankasiKullaniyor())
            //    sonuc = TMMSicilNoDurum.Kaydet(ve, kullanan, tf, "Kayıt");

            if (sonuc.islemSonuc)
                sonuc = TMMTasinirIslemDetay.Kaydet(ve, kullanan, tf.fisNo, tf.detay.objeler);

            if (!sonuc.islemSonuc)
            {
                OrtakFonksiyonlar.HataDosyaYaz("TifHata.txt", "*********************************");
                OrtakFonksiyonlar.HataDosyaYaz("TifHata.txt", ve.hata);
                OrtakFonksiyonlar.HataDosyaYaz("TifHata.txt", "*********************************");

                sonuc.hataStr = TasinirMalResI.TMM200 + "" + sonuc.hataStr;
                ve.Rollback();
                sonuc.vtHatasi = true;
            }
            else if (sonuc.islemSonuc)
            {
                string anahtar = tf.fisNo + "-" + tf.kod;

                sonuc.bilgiStr = string.Format(TasinirMalResI.TMM199, tf.fisNo);
                ve.Commit();

                SicilNoHareket sc = new SicilNoHareket();
                sc.yil = tf.yil;
                sc.muhasebeKod = tf.muhasebeKod;
                sc.harcamaBirimKod = tf.harcamaKod;
                sc.ambarKod = tf.ambarKod;
                sc.fisNo = tf.fisNo;

                ObjectArray sicilNoNumaralar = TMMSicilNoHareket.ListeleSicilNoNumara(null, kullanan, sc);

                if (sicilNoNumaralar.objeler.Count > 0 && (tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS || tf.islemTipTur == (int)ENUMIslemTipi.BAGISGIRIS))
                    tf.numaraVer = true;

                if (sonuc.islemSonuc && (tf.kayittanSonraOnayla || sicilNoNumaralar.objeler.Count > 0))
                    sonuc = BelgeDurumDegistir(null, kullanan, tf, "Onay");

                sonuc.anahtar = anahtar;
            }

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            ve3.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc GuncelleUstBilgi(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf)
        {
            Sonuc sonuc = VTAlanUzunlukKontrolEt(tf);
            if (!sonuc.islemSonuc)
                return sonuc;

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);

            //KUllanıcı biriminde yetkili olan kişi
            if (!sonuc.islemSonuc && (tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS))
                sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiKullaniciBirimi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);

            //ht:
            //yıl devri giriş ve çıkış fişlerine müdahale etme yetkisi sadece
            //yetkinin verildiği kişilerde olsun 
            if (tf.islemTipTur == (int)ENUMIslemTipi.YILDEVIRGIRIS || tf.islemTipTur == (int)ENUMIslemTipi.YILDEVIRCIKIS)
                sonuc.islemSonuc = TMMYetkiKontrol.YilDevriGirisCikisYetkisiVarmi(kullanan);

            //if (!sonuc.islemSonuc && tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)//DağıtımGiriş ise kontrol etme
            //    sonuc.islemSonuc = true;

            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM050;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTasinirIslemForm tabTasinirIslemForm = new TABTasinirIslemForm(ve);
            ve.BeginTransaction();

            tf.ipAdresi = kullanan.telefon;
            tf.islemYapan = kullanan.kullaniciKodu;

            sonuc = tabTasinirIslemForm.Kaydet(tf);

            if (!sonuc.islemSonuc)
            {
                OrtakFonksiyonlar.HataDosyaYaz("TifHata.txt", "*********************************");
                OrtakFonksiyonlar.HataDosyaYaz("TifHata.txt", ve.hata);
                OrtakFonksiyonlar.HataDosyaYaz("TifHata.txt", "*********************************");

                ve.Rollback();
                ve.VeriErisimKapat();
                sonuc.vtHatasi = true;
                return sonuc;
            }
            else if (sonuc.islemSonuc)
            {
                string anahtar = tf.fisNo + "-" + tf.kod;

                sonuc.bilgiStr = string.Format("Belge No'lu fişin " + tf.fisNo + " üst bilgisi başarıyla güncellendi.");
                ve.Commit();

                sonuc.anahtar = anahtar;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem fişini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <param name="siraNoSirali">Taşınır işlem fişinin detay bilgilerinin satır sırasına göre sıralı olup olmaması bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem fişi döndürülür.</returns>
        public static ObjectArray FisAc(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf, bool siraNoSirali)
        {
            string kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI");

            bool kullaniciBirimiYetkisiIleAcildi = false;
            ObjectArray bilgi = new ObjectArray();

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)))
            {
                if (!tf.devirGirisiMi)
                {
                    bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);

                    //KUllanıcı biriminde yetkili olan kişi
                    if (!bilgi.sonuc.islemSonuc)
                    {
                        bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiKullaniciBirimi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
                        kullaniciBirimiYetkisiIleAcildi = true;
                    }
                    if (!bilgi.sonuc.islemSonuc)
                    {
                        bilgi.sonuc.hataStr = TasinirMalResI.TMM060;
                        return bilgi;
                    }
                }

                if (tf.yil == 0)
                    bilgi.sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";
                if (tf.muhasebeKod == string.Empty)
                    bilgi.sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";
                if (tf.harcamaKod == string.Empty)
                    bilgi.sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";
                if (tf.fisNo == string.Empty)
                    bilgi.sonuc.hataStr += TasinirMalResI.TMM085 + "<br>";

                if (bilgi.sonuc.hataStr != String.Empty)
                {
                    bilgi.sonuc.islemSonuc = false;
                    return bilgi;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTasinirIslemForm tabTasinirIslemForm = new TABTasinirIslemForm(ve);

            tf.fisNo = tf.fisNo.PadLeft(6, '0');
            IDataReader dr = tabTasinirIslemForm.FisAc(tf);

            int sonSiraNo = (int)TABTasinirIslemForm.ENUMAlanlar.MUTEMMIM;
            while (dr.Read())
            {
                tf.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.MUHASEBEKOD, string.Empty);
                tf.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.HARCAMABIRIMKOD, string.Empty));
                tf.fisTarih = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.FISTARIH);
                tf.ambarKod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.AMBARKOD, string.Empty);
                tf.islemTipKod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ISLEMTIPKOD, 0);
                tf.muayeneNo = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.MUAYENENO, string.Empty);
                tf.muayeneTarih = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.MUAYENETARIH);
                tf.dayanakNo = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.DAYANAKNO, string.Empty);
                tf.dayanakTarih = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.DAYANAKTARIH);
                tf.neredenGeldi = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.NEREDENGELDI, string.Empty);
                tf.faturaNo = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.FATURANO, string.Empty);
                tf.faturaTarih = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.FATURATARIH);
                tf.nereyeGitti = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.NEREYEGITTI, string.Empty);
                tf.kimeGitti = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.KIMEGITTI, string.Empty);
                tf.gMuhasebeKod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.GMUHASEBEKOD, string.Empty);
                tf.gHarcamaKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.GHARCAMAKOD, string.Empty));
                tf.gAmbarKod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.GAMBARKOD, string.Empty);
                tf.gYil = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.GYIL, 0);
                tf.gFisNo = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.GFISNO, string.Empty);
                tf.kod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.KOD, string.Empty);
                tf.onayDurum = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ONAYDURUM, 0);
                tf.aciklama = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ACIKLAMA, "");
                tf.onayAciklama = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ONAYACIKLAMA, "");
                tf.mutemmim = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.MUTEMMIM, 0);


                if (tf.gMuhasebeKod != string.Empty)
                {
                    TMMMuhasebe tmmMuh = new TMMMuhasebe();
                    Muhasebe muh = new Muhasebe();
                    muh.kod = tf.gMuhasebeKod;

                    ObjectArray gonMuh = tmmMuh.Listele(kullanan, muh);
                    tf.gMuhasebeAd = gonMuh.ObjeSayisi > 0 ? ((Muhasebe)gonMuh[0]).ad : string.Empty;

                    HarcamaBirimi hb = new HarcamaBirimi();
                    hb.kod = tf.gHarcamaKod.Replace(".", "");
                    hb.muhasebeKod = tf.gMuhasebeKod;

                    ObjectArray gonHarcama = TMMHarcamaBirimi.Listele(kullanan, hb);
                    tf.gHarcamaAd = gonHarcama.ObjeSayisi > 0 ? ((HarcamaBirimi)gonHarcama[0]).ad : string.Empty;

                    Ambar amb = new Ambar();
                    amb.kod = tf.gAmbarKod;
                    amb.harcamaBirimKod = tf.gHarcamaKod.Replace(".", "");
                    amb.muhasebeKod = tf.gMuhasebeKod;

                    ObjectArray gonAmbar = TMMAmbar.Listele(null, kullanan, amb);
                    tf.gAmbarAd = gonAmbar.ObjeSayisi > 0 ? ((Ambar)gonAmbar[0]).ad : string.Empty;
                }

                tf.muhasebeAd = VeriErisim.DegerAl(dr, sonSiraNo + 1, string.Empty);
                tf.ilKod = VeriErisim.DegerAl(dr, sonSiraNo + 2, string.Empty);
                tf.ilAd = VeriErisim.DegerAl(dr, sonSiraNo + 3, string.Empty);
                tf.ilceKod = VeriErisim.DegerAl(dr, sonSiraNo + 4, string.Empty);
                tf.ilceAd = VeriErisim.DegerAl(dr, sonSiraNo + 5, string.Empty);
                tf.harcamaAd = VeriErisim.DegerAl(dr, sonSiraNo + 6, string.Empty);
                tf.ambarAd = VeriErisim.DegerAl(dr, sonSiraNo + 7, string.Empty);
                tf.durum = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.DURUM, 0);
                tf.islemTarih = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ISLEMTARIH);
                tf.islemYapan = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ISLEMYAPAN, string.Empty);
                if (tf.islemTipKod == (int)ENUMIslemTipi.GECICIIHRAC)
                    tf.islemTipTur = (int)ENUMIslemTipi.DEVIRCIKISKURUM;
                else
                    tf.islemTipTur = (int)TMMServis.UzaySayiDegerGetir("TASISLEMTIPTUR", tf.islemTipKod.ToString(), true, "");

                //tf.detay = TMMTasinirIslemDetay.Listele(tf, siraNoSirali, 1, 1100);
                tf.detay = TMMTasinirIslemDetay.Listele(tf, siraNoSirali, 0, 0);

                bilgi.Ekle(tf);
            }
            dr.Close();
            ve.VeriErisimKapat();

            if (bilgi.ObjeSayisi == 0)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TasinirMalResI.TMM086;
            }
            else if (bilgi.ObjeSayisi > 1) //Buraya hiç bir zaman girmezsen sevinirim ;)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TasinirMalResI.TMM272;
            }
            else
                bilgi.sonuc.islemSonuc = true;

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) && bilgi.sonuc.islemSonuc)
            {
                if (kullaniciBirimiYetkisiIleAcildi)
                {
                    if (tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                        bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiKullaniciBirimi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
                    else
                        bilgi.sonuc.islemSonuc = false;
                }
                else if (!tf.devirGirisiMi)
                    bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);

                if (!bilgi.sonuc.islemSonuc)
                {
                    bilgi.objeler.Clear();
                    bilgi.sonuc.hataStr = TasinirMalResI.TMM060;
                    return bilgi;
                }
            }

            if (!bilgi.sonuc.islemSonuc)
            {
                bilgi.objeler.Clear();
                return bilgi;
            }

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem fişlerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <param name="kriter">Sorgulama için ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem fişleri listesi döndürülür.</returns>
        public static ObjectArray Listele(Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
            //KUllanıcı biriminde yetkili olan kişi

            if (!bilgi.sonuc.islemSonuc)
            {
                tf.islemTipTur = (int)TMMServis.UzaySayiDegerGetir("TASISLEMTIPTUR", kriter.islemTipi.ToString(), true, "");

                if (tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS || kriter.islemTipi == -999)
                    bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiKullaniciBirimi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
            }


            if (!bilgi.sonuc.islemSonuc)
            {
                bilgi.sonuc.hataStr = TasinirMalResI.TMM060;
                return bilgi;
            }

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)))
            {
                if (tf.yil == 0)
                    bilgi.sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";
                //Yetkisi var ise boş bırakabilir
                //if (tf.muhasebeKod == string.Empty)
                //    bilgi.sonuc.hataStr += TasinirMalResI.TMM142 + "<br>";
                //if (tf.harcamaKod == string.Empty)
                //    bilgi.sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";
                //if (tf.ambarKod == string.Empty)
                //    bilgi.sonuc.hataStr += TasinirMalResI.TMM031 + "<br>";

                if (bilgi.sonuc.hataStr != string.Empty)
                {
                    bilgi.sonuc.islemSonuc = false;
                    return bilgi;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTasinirIslemForm tabTasinirIslemForm = new TABTasinirIslemForm(ve);

            IDataReader dr = tabTasinirIslemForm.Listele(tf, kriter);

            if (ve.hata != "")
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr += ve.hata;
                bilgi.sonuc.vtHatasi = true;

                if (dr != null)
                    dr.Close();

                ve.VeriErisimKapat();

                return bilgi;
            }

            int sonSiraNo = (int)TABTasinirIslemForm.ENUMAlanlar.MUTEMMIM;
            while (dr.Read())
            {
                TasinirIslemForm tasForm = new TasinirIslemForm();

                tasForm.yil = tf.yil;
                if (tf.onayDurum == -100)
                    tasForm.yil = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.YIL, 0);
                //tasForm.muhasebeKod = tf.muhasebeKod;
                tasForm.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.MUHASEBEKOD, string.Empty);
                tasForm.harcamaKod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.HARCAMABIRIMKOD, string.Empty);
                tasForm.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), tasForm.harcamaKod);
                tasForm.fisNo = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.FISNO, string.Empty);

                tasForm.fisTarih = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.FISTARIH);
                tasForm.ambarKod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.AMBARKOD, "");
                tasForm.islemTipKod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ISLEMTIPKOD, 0);
                tasForm.islemTipAd = TMMServis.UzayYaziDegerGetir("TASISLEMTIPAD", tasForm.islemTipKod.ToString(), true, "");

                tasForm.durum = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.DURUM, 0);
                tasForm.islemTarih = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ISLEMTARIH);
                tasForm.islemYapan = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ISLEMYAPAN, "");
                tasForm.muhasebeAd = VeriErisim.DegerAl(dr, sonSiraNo + 1, "");
                tasForm.harcamaAd = VeriErisim.DegerAl(dr, sonSiraNo + 2, "");
                tasForm.ambarAd = VeriErisim.DegerAl(dr, sonSiraNo + 3, "");
                tasForm.islemTipTur = (int)TMMServis.UzaySayiDegerGetir("TASISLEMTIPTUR", tasForm.islemTipKod.ToString(), true, "");
                tasForm.onayDurum = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ONAYDURUM, 0);
                tasForm.kod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.KOD, string.Empty);
                tasForm.aciklama = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ACIKLAMA, "");
                tasForm.onayAciklama = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ONAYACIKLAMA, "");
                tasForm.mutemmim = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.MUTEMMIM, 0);
                tasForm.faturaTarih = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.FATURATARIH);
                tasForm.faturaNo = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.FATURANO, "");
                tasForm.fiyati = OrtakFonksiyonlar.Yuvarla(VeriErisim.DegerAl(dr, sonSiraNo + 5, 0.0), 2);
                tasForm.dosyaSayisi = VeriErisim.DegerAl(dr, sonSiraNo + 6, 0);

                bilgi.Ekle(tasForm);
            }

            dr.Close();
            ve.VeriErisimKapat();
            return bilgi;
        }

        /// <summary>
        /// Verilen taşınır işlem fişinin durum bilgisini döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <returns>Taşınır işlem fişinin durumu döndürülür.</returns>
        public static int BelgeDurumGetir(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf)
        {
            if (tf.yil == 0)
                return (int)ENUMBelgeDurumu.TANIMSIZ;
            if (string.IsNullOrEmpty(tf.muhasebeKod))
                return (int)ENUMBelgeDurumu.TANIMSIZ;
            if (string.IsNullOrEmpty(tf.harcamaKod))
                return (int)ENUMBelgeDurumu.TANIMSIZ;
            if (string.IsNullOrEmpty(tf.fisNo))
                return (int)ENUMBelgeDurumu.TANIMSIZ;

            tf.fisNo.PadLeft(6, '0');

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTasinirIslemForm tTif = new TABTasinirIslemForm(ve);
            return tTif.BelgeDurumGetirTekKolon(tf);
        }

        /// <summary>
        /// Taşınır işlem fişinin durumunu güncelleyen yordam
        /// Parametre olarak verilen yeniDurum bilgisine bakarak taşınır işlem fişine ve detay bilgilerine bir takım
        /// kontroller uygular, verilen fiş bilgileri bütün kontrolleri sorunsuz geçerse belgeye yeni durum atanır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <param name="yeniDurum">Belgeye atanması istenen yeni durum bilgisi - İptal, Onay veya OnayKaldir değerlerini alabilir.</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc BelgeDurumDegistir(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf, string yeniDurum)
        {
            Sonuc sonuc = new Sonuc();
            ///////////////////////////////////////////////////////////
            bool muhasebeKdvMukellefiMi = false;
            TMMMuhasebe tmmMuhasebe = new TMMMuhasebe();
            muhasebeKdvMukellefiMi = tmmMuhasebe.KdvMukellefiMi(tf.muhasebeKod);
            ///////////////////////////////////////////////////////////

            string TCCBEntegrasyon = OrtakFonksiyonlar.WebConfigOku("TasinirTCCBEntegrasyon", "");

            bool numaraVer = tf.numaraVer; //Merkez Bankası için
            string sonrakiHareketler = "";
            int islemTur = 0;

            tf.islemYapan = kullanan.kullaniciKodu;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTasinirIslemForm tabTasinirIslemForm = new TABTasinirIslemForm(ve);

            IDataReader dr = tabTasinirIslemForm.BelgeDurumGetir(tf);
            if (!dr.Read())
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM086;
                dr.Close();
                ve.VeriErisimKapat();
                return sonuc;
            }
            int durum = VeriErisim.DegerAl(dr, 0, 0);
            dr.Close();

            //B-A onay kontrolü
            if (Arac.MerkezBankasiKullaniyor())
            {
                ENUMTasinirIslemFormOnayDurumu onayDurum = (ENUMTasinirIslemFormOnayDurumu)tabTasinirIslemForm.OnayDurumGetirTekKolon(tf);


                if (yeniDurum == "Onay" && tf.BAOnayAdimi == ENUMTasinirIslemFormOnayDurumu.TANIMSIZ &&//BA Onay sürecinden gelmiyorsa
                    (onayDurum == ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI ||
                    onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIA ||
                    onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIB))
                {
                    sonuc.hataStr = "B/A Onay sürecinde veya Tamamlandığı için numara verme işlemi yapılamaz.";
                }

                if (yeniDurum == "OnayKaldir" && tf.BAOnayAdimi == ENUMTasinirIslemFormOnayDurumu.TANIMSIZ &&//BA Onay sürecinden gelmiyorsa
                    (onayDurum == ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI ||
                    onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIA ||
                    onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIB))
                {
                    sonuc.hataStr = "B/A Onay sürecinde veya Tamamlandığı için ön onay kaldırma işlemi yapılamaz.";
                }

                if (onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIB || onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIA)
                {
                    if (yeniDurum == "İptal")
                        sonuc.hataStr = "Belgenin durumu <b>İptal</b> için uygun değil!<br><br><b>Not:</b> Belge B/A Onayına gönderildi. İptal etmek için B/A onay yetklisi tarafından geri gönderilmesi gerekir.";
                }

                if (sonuc.hataStr != "")
                {
                    sonuc.islemSonuc = false;
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            //Onay durum kontrolü
            if (durum == (int)ENUMBelgeDurumu.ONAYLI && yeniDurum == "İptal")
                sonuc.hataStr = string.Format(TasinirMalResI.TMM273, tf.fisNo) + "<br>";
            else if (durum == (int)ENUMBelgeDurumu.ONAYLI && yeniDurum == "Onay")
                sonuc.hataStr = string.Format(TasinirMalResI.TMM274, tf.fisNo) + "<br>";
            else if (durum != (int)ENUMBelgeDurumu.ONAYLI && yeniDurum == "OnayKaldir")
                sonuc.hataStr = string.Format(TasinirMalResI.TMM416, tf.fisNo) + "<br>";
            if (durum == (int)ENUMBelgeDurumu.IPTAL && yeniDurum == "Onay")
                sonuc.hataStr = TasinirMalResI.TMM416 + "<br>";

            if (sonuc.hataStr != "")
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ObjectArray listeFis = FisAc(null, kullanan, tf, true);
            if (listeFis.ObjeSayisi < 1)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM276;
                ve.VeriErisimKapat();
                return sonuc;
            }

            TasinirIslemForm tfIslem = (TasinirIslemForm)listeFis[0];

            //KHK kapsamında fiş onayı kaldırılamaz Maliye Melih 13.12.2016
            //if (durum == (int)ENUMBelgeDurumu.ONAYLI && yeniDurum == "OnayKaldir")
            //{
            //    if ((tfIslem.islemTipKod == 26 || tfIslem.islemTipKod == 27) && tf.harcamaKod.StartsWith("12"))
            //    {
            //        sonuc.islemSonuc = false;
            //        sonuc.hataStr = "KHK kapsamında hazırlanan ve onaylanan TİF belgesinin onayı kaldırılamaz" + "<br>";
            //        ve.VeriErisimKapat();
            //        return sonuc;
            //    }
            //}

            sonuc = Kontrol(null, kullanan, tfIslem, true);
            if (sonuc.hataStr != "")
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Eskiden metodun üstünde idi. Fakat ambar bilgisi olmayınca farklı ambara yetkili bile olsa onaylama yapabiliyordu.
            if (yeniDurum == "Onay" || yeniDurum == "OnayKaldir")
            {
                if (numaraVer) //Numara verme işlemini onay yetkisi olmasada taşınır kullanıcısı yapabilsin. H.Ö 09.10.2018
                    sonuc.islemSonuc = true;
                else
                {
                    if (yeniDurum == "OnayKaldir" && Arac.CumhurbaskanligiSgbNetKullaniyor())
                        sonuc.islemSonuc = TMMYetkiKontrol.OnayKaldirmaYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
                    else
                        sonuc.islemSonuc = TMMYetkiKontrol.OnaylamaYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);

                    if (!sonuc.islemSonuc && (((tfIslem).islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || (tfIslem).islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)))
                        sonuc.islemSonuc = TMMYetkiKontrol.OnaylamaYetkisiVarMiKullaniciBirimi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
                }

                //ht:
                //yıl devri giriş ve çıkış fişlerine müdahale etme yetkisi sadece
                //yetkinin verildiği kişilerde olsun 
                if ((tfIslem).islemTipTur == (int)ENUMIslemTipi.YILDEVIRGIRIS || (tfIslem).islemTipTur == (int)ENUMIslemTipi.YILDEVIRCIKIS)
                    sonuc.islemSonuc = TMMYetkiKontrol.YilDevriGirisCikisYetkisiVarmi(kullanan);


                //if (!sonuc.islemSonuc && (tfIslem).islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)//DağıtımGiriş ise kontrol etme
                //    sonuc.islemSonuc = true;

                if (!sonuc.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    sonuc.hataStr = TasinirMalResI.TMM277;
                    return sonuc;
                }
            }
            else
            {
                if (tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS || tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                    sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiKullaniciBirimi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
                //ht:
                //yıl devri giriş ve çıkış fişlerine müdahale etme yetkisi sadece
                //yetkinin verildiği kişilerde olsun 
                else if ((tfIslem).islemTipTur == (int)ENUMIslemTipi.YILDEVIRGIRIS || (tfIslem).islemTipTur == (int)ENUMIslemTipi.YILDEVIRCIKIS)
                    sonuc.islemSonuc = TMMYetkiKontrol.YilDevriGirisCikisYetkisiVarmi(kullanan);
                else
                    sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, tf.muhasebeKod, tf.harcamaKod, tf.ambarKod);
                if (!sonuc.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    sonuc.hataStr = TasinirMalResI.TMM278;
                    return sonuc;
                }
            }

            //int islemTipiKod = (int)TMMServis.UzaySayiDegerGetir("TASFORMTIPKOD", tf.yil.ToString() + "-" + tf.muhasebeKod + "-" + tf.harcamaKod + "-" + tf.fisNo, true, "");
            islemTur = (int)TMMServis.UzaySayiDegerGetir("TASISLEMTIPTUR", tf.islemTipKod.ToString(), true, "");

            tf = tfIslem;

            bool zimmetIslemiYap = false;

            ObjectArray sicilNolar = new ObjectArray();

            ve.BeginTransaction();

            if (tf.islemTipKod != (int)ENUMIslemTipi.GECICIIHRAC)
            {
                if (yeniDurum == "Onay" || yeniDurum == "OnayKaldir")
                {
                    if (tf.detay.sonuc.islemSonuc)
                    {
                        //Değer arttırma kontolü
                        //************************
                        decimal degerArttir = 0;
                        if (islemTur == (int)ENUMIslemTipi.DEGERARTTIR)
                        {
                            decimal dayanikliSay = 0;
                            foreach (TasinirIslemDetay td in tf.detay.objeler)
                            {
                                //string hesapKoduBir = td.hesapPlanKod.Substring(0, 3);
                                //if (OrtakFonksiyonlar.ConvertToInt(hesapKoduBir, 0) == (int)ENUMTasinirHesapKodu.TUKETIM)
                                //    degerArttir += ((decimal)td.miktar * td.birimFiyatKDVLi);

                                if (TNS.TMM.Arac.DemirbasCesidiMi(td.hesapPlanKod))
                                {
                                    dayanikliSay += td.miktar;
                                }
                            }

                            //if (degerArttir == 0)
                            //    sonuc.hataStr = TasinirMalResI.TMM279;
                            if (dayanikliSay != 1)
                                sonuc.hataStr = TasinirMalResI.TMM280;
                            if (sonuc.hataStr != "")
                            {
                                sonuc.islemSonuc = false;
                                ve.Rollback();
                                ve.VeriErisimKapat();
                                return sonuc;
                            }
                        }
                        //Demirbaş için değer artış yapılsın. Tüketim kapatıldı. 2024.11.07 HÖ




                        //Devir giriş yapılmış ise onay kaldırılamaz
                        if (yeniDurum == "OnayKaldir" && (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS || tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS))
                        {
                            TasinirIslemForm tfDevir = new TasinirIslemForm();
                            tfDevir.gYil = tf.yil;
                            tfDevir.muhasebeKod = tf.gMuhasebeKod;
                            tfDevir.harcamaKod = tf.gHarcamaKod;
                            tfDevir.gFisNo = tf.fisNo;
                            tfDevir.gMuhasebeKod = tf.muhasebeKod;
                            tfDevir.gHarcamaKod = tf.harcamaKod;

                            IDataReader r = tabTasinirIslemForm.DevirGirisKaydiYapilmismi(tfDevir);
                            if (r.Read())
                            {
                                int gDurum = VeriErisim.DegerAl(r, 2, 0);
                                if (gDurum == (int)ENUMBelgeDurumu.ONAYLI)
                                    sonuc.hataStr += TasinirMalResI.TMM329 + "<br>";
                            }
                            r.Close();
                        }


                        if (yeniDurum == "Onay" && (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS || tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS || tf.islemTipTur == (int)ENUMIslemTipi.YILDEVIRGIRIS))
                        {
                            TNS.TMM.TasinirIslemForm tDevir = new TNS.TMM.TasinirIslemForm();
                            if (tf.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS)
                            {
                                tDevir.yil = tf.gYil;
                                tDevir.harcamaKod = tf.gHarcamaKod;
                                tDevir.muhasebeKod = tf.gMuhasebeKod;
                                tDevir.fisNo = tf.gFisNo;
                                tDevir.devirGirisiMi = true;
                            }
                            else if (tf.islemTipTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                            {
                                tDevir.yil = tf.gYil;
                                tDevir.harcamaKod = tf.harcamaKod;
                                tDevir.muhasebeKod = tf.muhasebeKod;
                                tDevir.fisNo = tf.gFisNo;
                                tDevir.devirGirisiMi = true;
                            }
                            else if (tf.islemTipTur == (int)ENUMIslemTipi.YILDEVIRGIRIS)
                            {
                                tDevir.yil = tf.gYil;
                                tDevir.harcamaKod = tf.harcamaKod;
                                tDevir.muhasebeKod = tf.muhasebeKod;
                                tDevir.fisNo = tf.gFisNo;
                            }

                            IDataReader r = tabTasinirIslemForm.FisAc(tDevir);
                            if (r.Read())
                            {
                                int gDurum = VeriErisim.DegerAl(r, 16, 0);
                                if (gDurum != (int)ENUMBelgeDurumu.ONAYLI)
                                    sonuc.hataStr += TasinirMalResI.TMM330 + "<br>";
                            }
                            r.Close();


                            Sonuc devirSonuc = DevirGirisCikisMalzemeleriAyniMi(ve, tf);
                            if (!devirSonuc.islemSonuc)
                                sonuc.hataStr += TasinirMalResI.TMM324 + "<br>";
                        }

                        //A Onayı sonrasında fiş tarihinin değiştirilmesi.
                        if (tf.BAOnayAdimi == ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI && TNS.TMM.Arac.MerkezBankasiKullaniyor())
                        {
                            if (!(tfIslem.fisTarih.Ay == 12 && tfIslem.fisTarih.Gun == 31) && (tfIslem.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS || tfIslem.islemTipTur == (int)ENUMIslemTipi.BAGISGIRIS))
                                tf.fisTarih = new TNSDateTime(DateTime.Now.Date);
                        }


                        //KHK Çıkış ilk giren kontrolü olmasın
                        int kontrolIslemTipKod = OrtakFonksiyonlar.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings.Get("TasinirKHKIslemKoduCikis") + "", 0);
                        bool yilDevriIslemi = false;
                        //************************
                        foreach (TasinirIslemDetay td in tf.detay.objeler)
                        {
                            //Dayanıklı bölümü
                            if (TNS.TMM.Arac.DemirbasCesidiMi(td.hesapPlanKod))
                            {
                                SicilNoHareket sicilNo = new SicilNoHareket();

                                sicilNo.yil = tf.yil;
                                sicilNo.muhasebeKod = tf.muhasebeKod;
                                sicilNo.harcamaBirimKod = tf.harcamaKod;
                                sicilNo.hesapPlanKod = td.hesapPlanKod.Replace(".", "");
                                sicilNo.hesapPlanAd = td.hesapPlanAd;
                                sicilNo.fisNo = td.fisNo;
                                sicilNo.fisSiraNo = td.siraNo;
                                sicilNo.sicilNo = td.gorSicilNo;
                                sicilNo.prSicilNo = td.sicilNo;
                                sicilNo.ambarKod = tf.ambarKod;
                                sicilNo.fiyat = td.birimFiyat;
                                sicilNo.kdvOran = td.kdvOran;
                                sicilNo.islemTipKod = tf.islemTipKod;
                                sicilNo.islemTurKod = islemTur;
                                sicilNo.islemYapan = kullanan.kullaniciKodu;
                                sicilNo.islemTarih = new TNSDateTime(DateTime.Now);
                                sicilNo.eSicilNo = td.eSicilNo;
                                sicilNo.eMuhasebeKod = tf.gMuhasebeKod;
                                sicilNo.eHarcamaBirimKod = tf.gHarcamaKod;
                                sicilNo.fisTarih = new TNSDateTime(tf.fisTarih.ToString());
                                sicilNo.kimeGitti = tf.kimeGitti;
                                sicilNo.nereyeGitti = tf.nereyeGitti;
                                sicilNo.neredenGeldi = tf.neredenGeldi;

                                if (muhasebeKdvMukellefiMi)
                                    sicilNo.kdvliBirimFiyat = sicilNo.fiyat;
                                else
                                    sicilNo.kdvliBirimFiyat = (sicilNo.fiyat * (1 + (OrtakFonksiyonlar.ConvertToDecimal(sicilNo.kdvOran.ToString(), 0) / 100))); ;

                                sicilNo.gonMuhasebeKod = tf.gMuhasebeKod;
                                sicilNo.gonHarcamaBirimKod = tf.gHarcamaKod;
                                sicilNo.gonAmbarKod = tf.gAmbarKod;

                                if (islemTur == (int)ENUMIslemTipi.ENFLASYONARTISI)
                                    degerArttir = td.satisBedeli;

                                if (islemTur == (int)ENUMIslemTipi.DEGERARTTIR && !Arac.MerkezBankasiKullaniyor())
                                    degerArttir = td.satisBedeli;

                                sicilNo.degerArtisi = degerArttir;
                                sicilNo.miktar = OrtakFonksiyonlar.ConvertToInt(td.miktar, 0);
                                sicilNo.ozellik = td.ozellik;

                                if (islemTur == (int)ENUMIslemTipi.SATINALMAGIRIS ||
                                    islemTur == (int)ENUMIslemTipi.ACILIS)
                                {
                                    if (sicilNo.ozellik.disSicilNo == "") sicilNo.ozellik.disSicilNo = td.eSicilNo;
                                    if (sicilNo.ozellik.neredenGeldi == "") sicilNo.ozellik.neredenGeldi = td.eTedarikSekli;
                                    if (sicilNo.ozellik.yeriKonusu == "") sicilNo.ozellik.yeriKonusu = td.eAciklama;
                                    if (sicilNo.ozellik.cagi == "") sicilNo.ozellik.cagi = td.ozellik.cagi;
                                    if (sicilNo.ozellik.cesidi == "") sicilNo.ozellik.cesidi = td.ozellik.cesidi;
                                    if (sicilNo.ozellik.bulunduguYer == "") sicilNo.ozellik.bulunduguYer = td.yerleskeYeri;
                                }
                                sicilNolar.objeler.Add(sicilNo);

                                if (islemTur == (int)ENUMIslemTipi.KULLANILMAZCIKIS)
                                {
                                    sonuc.hataStr = string.Format(TasinirMalResI.TMM242, td.siraNo.ToString()) + "<br>";
                                    break;
                                }

                                if (islemTur == (int)ENUMIslemTipi.YILDEVIRCIKIS ||
                                    islemTur == (int)ENUMIslemTipi.YILDEVIRGIRIS)
                                {
                                    yilDevriIslemi = true;
                                    if (yeniDurum == "Onay")
                                    {
                                        sonuc.hataStr = TasinirMalResI.TMM232;
                                        sonuc.islemSonuc = false;
                                        break;
                                    }
                                }

                                if (islemTur == (int)ENUMIslemTipi.DEVIRGIRIS || islemTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                                {
                                    if (yeniDurum == "Onay")
                                    {
                                        //Devir girişi yapılabilirmi. Devir giriş kaydı yapıldıktan sonra Devir çıkışı yapan formun onayını kaldırdı ise onaylanamaz.
                                        sonuc = TMMSicilNoHareket.DevirGirisiYapilabilirmi(null, kullanan, sicilNo, sicilNo.miktar);
                                        if (!sonuc.islemSonuc)
                                        {
                                            sonuc.hataStr = TasinirMalResI.TMM281;
                                            break;
                                        }
                                    }
                                }
                                else if (islemTur == (int)ENUMIslemTipi.BAGISCIKIS ||
                                        islemTur == (int)ENUMIslemTipi.DEVIRCIKIS ||
                                        islemTur == (int)ENUMIslemTipi.HURDACIKIS ||
                                        islemTur == (int)ENUMIslemTipi.SATISCIKIS ||
                                        islemTur == (int)ENUMIslemTipi.DEGERARTTIR ||
                                        islemTur == (int)ENUMIslemTipi.ENFLASYONARTISI ||
                                        islemTur == (int)ENUMIslemTipi.KULLANILMAZCIKIS ||
                                        islemTur == (int)ENUMIslemTipi.DEVIRCIKISKURUM ||
                                        islemTur == (int)ENUMIslemTipi.SAYIMNOKSANICIKIS ||
                                        islemTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS)
                                {
                                    if (yeniDurum == "Onay")
                                    {
                                        SicilNoHareket sicilNoAra = new SicilNoHareket();
                                        sicilNoAra.yil = sicilNo.yil;
                                        sicilNoAra.muhasebeKod = sicilNo.muhasebeKod;
                                        sicilNoAra.harcamaBirimKod = sicilNo.harcamaBirimKod;
                                        sicilNoAra.ambarKod = sicilNo.ambarKod;
                                        sicilNoAra.prSicilNo = sicilNo.prSicilNo;

                                        ObjectArray o = TMMSicilNoHareket.AmbardakiSicilNolar(ve, kullanan, sicilNoAra);

                                        if (o.objeler.Count <= 0)
                                            sonuc.hataStr += string.Format(TasinirMalResI.TMM021, sicilNo.sicilNo) + "<br>";
                                        else
                                        {
                                            if (islemTur != (int)ENUMIslemTipi.ENFLASYONARTISI)
                                            {
                                                if (sicilNo.fiyat.ToString("#,###.##") != ((SicilNoHareket)o.objeler[0]).fiyat.ToString("#,###.##"))//Giriş fiyatı 14,324567 gibi ise 14,32 den işlem görüyor
                                                    sonuc.hataStr += string.Format(TasinirMalResI.TMM282, sicilNo.sicilNo, ((SicilNoHareket)o.objeler[0]).fiyat.ToString("#,###.##")) + "<br>";
                                            }
                                        }
                                        if (sonuc.hataStr != "")
                                        {
                                            sonuc.islemSonuc = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (OrtakFonksiyonlar.ConvertToInt(td.hesapPlanKod.Substring(0, 3), 0) == (int)ENUMTasinirHesapKodu.TUKETIM && islemTur != kontrolIslemTipKod)
                            {
                                TABStokHareket tabStokHareketView = new TABStokHareket(ve);

                                if (yeniDurum == "Onay")
                                {
                                    if (islemTur == (int)ENUMIslemTipi.TUKETIMCIKIS ||
                                        islemTur == (int)ENUMIslemTipi.DEVIRCIKIS ||
                                        islemTur == (int)ENUMIslemTipi.BAGISCIKIS ||
                                        islemTur == (int)ENUMIslemTipi.SATISCIKIS ||
                                        islemTur == (int)ENUMIslemTipi.KULLANILMAZCIKIS ||
                                        islemTur == (int)ENUMIslemTipi.DEGERARTTIR ||
                                        islemTur == (int)ENUMIslemTipi.ENFLASYONARTISI ||
                                        islemTur == (int)ENUMIslemTipi.DEVIRCIKISKURUM ||
                                        islemTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS)
                                    {
                                        StokHareketBilgi shBilgi = new StokHareketBilgi();
                                        shBilgi.yil = td.yil;
                                        shBilgi.hesapPlanKod = td.hesapPlanKod.Replace(".", "");
                                        shBilgi.muhasebeKod = td.muhasebeKod;
                                        shBilgi.harcamaKod = td.harcamaKod;
                                        shBilgi.ambarKod = tf.ambarKod;
                                        //shBilgi.birimFiyat = td.birimFiyat;
                                        //decimal miktar = td.miktar;

                                        ObjectArray bilgi = TMMStokHareket.AmbardakiStoklariGetir(ve, kullanan, shBilgi);

                                        if (bilgi.objeler.Count <= 0)
                                            sonuc.hataStr += string.Format(TasinirMalResI.TMM243, td.siraNo.ToString(), td.hesapPlanKod) + "<br>";
                                        else
                                        {
                                            string tuketimHata = "";
                                            int sonKontrolEdilenSira = -1;
                                            foreach (StokHareketBilgi stoktaki in bilgi.objeler)
                                            {
                                                foreach (TasinirIslemDetay tifdeki in tf.detay.objeler)
                                                {
                                                    if (tuketimHata != "")
                                                        break;

                                                    if (stoktaki.hesapPlanKod.Replace(".", "").Trim() == tifdeki.hesapPlanKod.Replace(".", "").Trim() && sonKontrolEdilenSira < tifdeki.siraNo)
                                                    {
                                                        sonKontrolEdilenSira = tifdeki.siraNo;

                                                        if ((stoktaki.birimFiyat != tifdeki.birimFiyat || stoktaki.kdvOran != tifdeki.kdvOran) && !yilDevriIslemi)
                                                        {
                                                            tuketimHata += string.Format(TasinirMalResI.TMM244, tifdeki.siraNo, stoktaki.miktar, stoktaki.birimFiyat, stoktaki.kdvOran);
                                                            //OrtakFonksiyonlar.HataDosyaYaz("YilDevriHata.txt", "tuketimHata1", false);
                                                        }

                                                        if (tuketimHata == "")
                                                        {
                                                            if (stoktaki.miktar < (decimal)tifdeki.miktar)
                                                                tuketimHata += string.Format(TasinirMalResI.TMM245, tifdeki.siraNo, tifdeki.hesapPlanKod, stoktaki.miktar) + "<br>";
                                                            //İlk satırdaki malzemenin hepsi kullanılmamış ve tifde aynı hesap kodlu başka bir satır var ise
                                                            else if (stoktaki.miktar > (decimal)tifdeki.miktar)
                                                            {
                                                                foreach (TasinirIslemDetay tifkontrol in tf.detay.objeler)
                                                                {
                                                                    if (stoktaki.hesapPlanKod.Replace(".", "").Trim() == tifkontrol.hesapPlanKod.Replace(".", "").Trim() &&
                                                                        tifkontrol.siraNo > tifdeki.siraNo && !yilDevriIslemi)
                                                                    {
                                                                        tuketimHata += string.Format(TasinirMalResI.TMM246, tifkontrol.siraNo, tifdeki.siraNo, stoktaki.miktar) + "<br>";
                                                                        //OrtakFonksiyonlar.HataDosyaYaz("YilDevriHata.txt", "tuketimHata2", false);
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                                break;
                                                        }
                                                    }
                                                }
                                            }

                                            sonuc.hataStr += tuketimHata;
                                            decimal miktarKontrol = 0;
                                            decimal miktarKontrol2 = 0;
                                            //TİFde 2 farklı satır giriş yapıldı fakat stokta tek malzeme var ise aşağıdaki kontrolde gerekli
                                            foreach (TasinirIslemDetay td2 in tf.detay.objeler)
                                            {
                                                if (td2.hesapPlanKod.Replace(".", "").Trim() == td.hesapPlanKod.Replace(".", "").Trim() &&
                                                    td2.birimFiyat == td.birimFiyat &&
                                                    td2.kdvOran == td.kdvOran)
                                                    miktarKontrol += (decimal)td2.miktar;
                                            }
                                            foreach (StokHareketBilgi td2 in bilgi.objeler)
                                            {
                                                if (td2.hesapPlanKod.Replace(".", "").Trim() == td.hesapPlanKod.Replace(".", "").Trim() &&
                                                    td2.birimFiyat == td.birimFiyat &&
                                                    td2.kdvOran == td.kdvOran)
                                                    miktarKontrol2 += td2.miktar;
                                            }
                                            if (Math.Round(miktarKontrol, 4) > Math.Round(miktarKontrol2, 4))
                                                sonuc.hataStr += string.Format(TasinirMalResI.TMM247, td.siraNo, td.hesapPlanKod, miktarKontrol2.ToString(), miktarKontrol.ToString()) + "<br>";
                                        }
                                    }
                                }
                                else if (yeniDurum == "OnayKaldir")
                                {
                                    StokHareketBilgi bilgi = new StokHareketBilgi();
                                    bilgi.yil = tf.yil;
                                    bilgi.harcamaKod = tf.harcamaKod;
                                    bilgi.muhasebeKod = tf.muhasebeKod;
                                    bilgi.ambarKod = tf.ambarKod;
                                    bilgi.hesapPlanKod = td.hesapPlanKod.Replace(".", "");
                                    bilgi.birimFiyat = td.birimFiyat;
                                    bilgi.kdvOran = td.kdvOran;

                                    decimal kalanMiktar = 0;
                                    decimal miktarKontrol = 0;
                                    //Tifdeki birden fazla satırda aynı malzeme giriş yapmış olabilir.
                                    //miktar alanları topla
                                    foreach (TasinirIslemDetay td2 in tf.detay.objeler)
                                    {
                                        if (td2.hesapPlanKod.Replace(".", "").Trim() == td.hesapPlanKod.Replace(".", "").Trim() &&
                                            td2.birimFiyat == td.birimFiyat &&
                                            td2.kdvOran == td.kdvOran)
                                            miktarKontrol += (decimal)td2.miktar;
                                    }

                                    //Stokta kaç tane var
                                    ObjectArray stok = TMMStokHareket.AmbardakiStoklariGetir(ve, kullanan, bilgi);
                                    foreach (StokHareketBilgi stoktaki in stok.objeler)
                                        kalanMiktar += stoktaki.miktar;

                                    bool kontrolEt = false;
                                    if (islemTur < (int)ENUMIslemTipi.TUKETIMCIKIS)
                                    {
                                        kalanMiktar = kalanMiktar - miktarKontrol;
                                        if (kalanMiktar < 0)
                                            kontrolEt = true;
                                    }
                                    else
                                        kontrolEt = true;

                                    if (kontrolEt)
                                    {
                                        IDataReader drSonrasi = tabStokHareketView.SonrasiVarMi(bilgi, tf.fisNo);
                                        string geciciBilgi = "";
                                        while (drSonrasi.Read())
                                        {
                                            int yil = VeriErisim.DegerAl(drSonrasi, 0, 0);
                                            string muhasebe = VeriErisim.DegerAl(drSonrasi, 1, "");
                                            string hb = VeriErisim.DegerAl(drSonrasi, 2, "");
                                            string ambar = VeriErisim.DegerAl(drSonrasi, 11, "");
                                            string belgeNo = VeriErisim.DegerAl(drSonrasi, 3, "");

                                            geciciBilgi = string.Format(TasinirMalResI.TMM283, TasinirMalResI.TMM063, yil, muhasebe, hb, ambar, belgeNo) + "<br>";
                                            if (sonrakiHareketler.IndexOf(geciciBilgi) < 0)
                                                sonrakiHareketler += geciciBilgi;
                                        }

                                        drSonrasi.Close();
                                    }
                                }
                            }

                            if (!sonuc.islemSonuc || sonuc.hataStr != "")
                            {
                                sonuc.islemSonuc = false;
                                break;
                            }
                        }
                    }
                    else
                        sonuc.hataStr = string.Format(TasinirMalResI.TMM284, tf.fisNo) + "<br>";
                }

                if (sonrakiHareketler != "")
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TasinirMalResI.TMM285 + "<br>";
                    sonuc.hataStr += TasinirMalResI.TMM286 + "<br>";
                    sonuc.hataStr += sonrakiHareketler;
                }


                if (sicilNolar.objeler.Count > 0 && sonuc.hataStr == "" && sonuc.islemSonuc)
                {
                    if (!string.IsNullOrEmpty(amortismanDeger) && amortismanDeger == "1")
                    {
                        if (TMMAmortisman.AmortismanYapilmisMi(ve, tf.yil, tf.muhasebeKod, tf.harcamaKod, tf.fisNo))
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = TMMAmortisman.amortismanHata + "<br>";
                        }

                        if (TMMAmortismanDegerleme.DegerlemeYapilmisMi(ve, tf.yil, tf.muhasebeKod, tf.harcamaKod, tf.fisNo))
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr += TMMAmortismanDegerleme.degerlemeHata;
                        }
                    }

                    if (sonuc.islemSonuc)
                    {
                        if (yeniDurum == "Onay")
                        {
                            if (islemTur == (int)ENUMIslemTipi.SATINALMAGIRIS ||
                                islemTur == (int)ENUMIslemTipi.BAGISGIRIS ||
                                islemTur == (int)ENUMIslemTipi.SAYIMFAZLASIGIRIS ||
                                islemTur == (int)ENUMIslemTipi.IADEGIRIS ||
                                ((islemTur == (int)ENUMIslemTipi.DEVIRGIRIS || islemTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS) && (tf.muhasebeKod != tf.gMuhasebeKod || tf.harcamaKod.Replace(".", "") != tf.gHarcamaKod.Replace(".", ""))) ||
                                islemTur == (int)ENUMIslemTipi.URETILENGIRIS ||
                                islemTur == (int)ENUMIslemTipi.ACILIS ||
                                islemTur == (int)ENUMIslemTipi.DEVIRGIRISKURUM)
                            {


                                //**************************************************
                                //SicilNoHareket sicilNoAra = new SicilNoHareket();
                                //sicilNoAra.yil = tf.yil;
                                //sicilNoAra.muhasebeKod = tf.muhasebeKod;
                                //sicilNoAra.harcamaBirimKod = tf.harcamaKod;
                                //sicilNoAra.ambarKod = tf.ambarKod;
                                //ObjectArray kayitliSicilNolar = TMMSicilNoHareket.ListeleSicilNoNumara(null, kullanan, sicilNoAra);

                                //if (kayitliSicilNolar.objeler.Count > 0)
                                //{
                                //    foreach (SicilNoHareket item in kayitliSicilNolar.objeler)
                                //    {
                                //        if (item.prSicilNo)
                                //        {

                                //        }
                                //    }
                                //}
                                //**************************************************

                                sonuc = TMMSicilNoHareket.SicilNoVer(null, kullanan, sicilNolar, numaraVer);
                                if (sonuc.islemSonuc && Arac.MerkezBankasiKullaniyor() && !tf.aktarim)
                                    zimmetIslemiYap = true;

                                if (sonuc.islemSonuc && !string.IsNullOrWhiteSpace(TCCBEntegrasyon))
                                {
                                    try
                                    {
                                        TMMTCCBEntegrasyon.Kaydet255(kullanan, sicilNolar, "numaraVer");
                                    }
                                    catch (Exception ex)
                                    {
                                        OrtakFonksiyonlar.HataDosyaYaz("TCCBEntegrasyonHataCatch.txt", "numaraVer>" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + ">" + tf.yil + ">" + tf.muhasebeKod + ">" + tf.harcamaKod + ">" + tf.fisNo + ">>> ex.Message:" + ex.Message, false);
                                    }

                                }

                            }

                            if (islemTur == (int)ENUMIslemTipi.BAGISCIKIS ||
                                islemTur == (int)ENUMIslemTipi.DEVIRCIKIS ||
                                islemTur == (int)ENUMIslemTipi.HURDACIKIS ||
                                islemTur == (int)ENUMIslemTipi.SATISCIKIS ||
                                islemTur == (int)ENUMIslemTipi.DEGERARTTIR ||
                                islemTur == (int)ENUMIslemTipi.ENFLASYONARTISI ||
                                islemTur == (int)ENUMIslemTipi.KULLANILMAZCIKIS ||
                                islemTur == (int)ENUMIslemTipi.DEVIRCIKISKURUM ||
                                islemTur == (int)ENUMIslemTipi.SAYIMNOKSANICIKIS ||
                                islemTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS ||
                                ((islemTur == (int)ENUMIslemTipi.DEVIRGIRIS || islemTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS) && tf.muhasebeKod == tf.gMuhasebeKod && tf.harcamaKod.Replace(".", "") == tf.gHarcamaKod.Replace(".", "")))
                            {
                                sonuc = TMMSicilNoHareket.HareketKaydet(null, kullanan, sicilNolar);
                                if (sonuc.islemSonuc && Arac.MerkezBankasiKullaniyor())
                                    sonuc = TMMSicilNoOzellik.BisKayitDus(null, kullanan, sicilNolar);

                                if (sonuc.islemSonuc && !string.IsNullOrWhiteSpace(TCCBEntegrasyon))
                                {
                                    try
                                    {
                                        TMMTCCBEntegrasyon.Kaydet255(kullanan, sicilNolar, "çıkış");
                                    }
                                    catch (Exception ex)
                                    {
                                        OrtakFonksiyonlar.HataDosyaYaz("TCCBEntegrasyonHataCatch.txt", "çıkış>" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + ">" + tf.yil + ">" + tf.muhasebeKod + ">" + tf.harcamaKod + ">" + tf.fisNo + ">>> ex.Message:" + ex.Message, false);
                                    }
                                }
                            }

                            //Sicil Durum bilgisi yazmak için
                            if (sonuc.islemSonuc && Arac.MerkezBankasiKullaniyor())
                                sonuc = TMMSicilNoDurum.Kaydet(ve, kullanan, tf, "Onay");

                        }
                        else if (yeniDurum == "OnayKaldir")
                        {
                            if (Genel.rfIdVarMi)
                            {
                                TABSicilRFID tabRFID = new TABSicilRFID(ve);
                                sonuc = tabRFID.Guncelle(tf.yil, tf.muhasebeKod, tf.harcamaKod, tf.fisNo);
                            }

                            if (sonuc.islemSonuc)
                            {
                                //SicilNoHareket fisBilgi = new SicilNoHareket();
                                //fisBilgi.yil = ((SicilNoHareket)sicilNolar.objeler[0]).yil;
                                //fisBilgi.muhasebeKod = ((SicilNoHareket)sicilNolar.objeler[0]).muhasebeKod;
                                //fisBilgi.harcamaBirimKod = ((SicilNoHareket)sicilNolar.objeler[0]).harcamaBirimKod;
                                //fisBilgi.fisNo = ((SicilNoHareket)sicilNolar.objeler[0]).fisNo;
                                //fisBilgi.islemTurKod = ((SicilNoHareket)sicilNolar.objeler[0]).islemTurKod;

                                //string[] zimmetFisNo = TMMSicilNoHareket.SonrasindakiHareketler(fisBilgi, true).Split(',');

                                //if (zimmetFisNo.Length > 0)
                                //{
                                //    for (int i = 0; i < zimmetFisNo.Length; i++)
                                //    {
                                //        TNS.TMM.ZimmetForm zf = new TNS.TMM.ZimmetForm();

                                //        zf.yil = fisBilgi.yil;
                                //        zf.muhasebeKod = fisBilgi.muhasebeKod;
                                //        zf.harcamaBirimKod = fisBilgi.harcamaBirimKod;
                                //        zf.fisNo = zimmetFisNo[i];
                                //        zf.belgeTur = (int)ENUMZimmetBelgeTur.ZIMMETFISI;
                                //        sonuc = TMMZimmetForm.BelgeDurumDegistir(ve, kullanan, zf, "OnayKaldir");
                                //        sonuc = TMMZimmetForm.BelgeDurumDegistir(ve, kullanan, zf, "İptal");
                                //    }
                                //    if (sonuc.islemSonuc)
                                //        ve.Commit();
                                //}


                                sonuc = TMMSicilNoHareket.SicilNoIptalEt(null, kullanan, sicilNolar);
                                if (sonuc.islemSonuc)
                                {
                                    try
                                    {
                                        sonuc = MIFNoIptal(kullanan, sicilNolar);
                                    }
                                    catch { }

                                    if (sonuc.islemSonuc && !string.IsNullOrWhiteSpace(TCCBEntegrasyon))
                                    {
                                        try
                                        {
                                            TMMTCCBEntegrasyon.Kaydet255(kullanan, sicilNolar, "iptal");
                                        }
                                        catch (Exception ex)
                                        {
                                            OrtakFonksiyonlar.HataDosyaYaz("TCCBEntegrasyonHataCatch.txt", "iptal>" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + ">" + tf.yil + ">" + tf.muhasebeKod + ">" + tf.harcamaKod + ">" + tf.fisNo + ">>> ex.Message:" + ex.Message, false);
                                        }
                                    }

                                }
                            }

                            //Sicil Durum bilgisi yazmak için
                            if (sonuc.islemSonuc && Arac.MerkezBankasiKullaniyor())
                                sonuc = TMMSicilNoDurum.Kaydet(ve, kullanan, tf, "OnayKaldir");
                        }
                    }
                }
            }



            if (sonuc.hataStr == "" && sonuc.islemSonuc)
            {
                tf.islemYapan = kullanan.kullaniciKodu;
                sonuc = tabTasinirIslemForm.BelgeDurumDegistir(tf, yeniDurum);

                if (sonuc.islemSonuc)
                {

                    //Geçici İhraç bilgisi yazmak için
                    if (tf.islemTipKod == (int)ENUMIslemTipi.GECICIIHRAC && yeniDurum == "Onay")
                        sonuc = TMMGeciciIhrac.Kaydet(ve, kullanan, tf);

                    string tarihceAciklama = TasinirIslemForm.FormDurumAciklama(TABGenel.FormDurumNoVer(yeniDurum));

                    if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    {
                        MuhasebeIslemiKriter mk = new MuhasebeIslemiKriter();
                        mk.yil = tf.yil;
                        mk.muhasebeKod = tf.muhasebeKod;
                        mk.harcamaKod = tf.harcamaKod;
                        mk.fisNo = tf.fisNo;

                        ObjectArray listeTarihce = TMMMuhasebeBilgi.ListeleTarihce(null, kullanan, mk);

                        if (listeFis.objeler.Count > 0)
                        {
                            foreach (TarihceBilgisi item in listeTarihce.objeler)
                            {
                                if (item.islem == "Onaylandı" && yeniDurum == "Onay")
                                {
                                    tarihceAciklama = "";
                                    break;
                                }
                            }
                        }
                    }

                    if (tarihceAciklama.Contains("Bilinmiyor"))
                    {
                        sonuc.hataStr = TasinirMalResI.TMM287;
                        sonuc.islemSonuc = false;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(tarihceAciklama) && tf.BAOnayAdimi != ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI)
                        {
                            tf.ipAdresi = kullanan.telefon;
                            tabTasinirIslemForm.KaydetTarihce(tf.yil, tf.muhasebeKod, tf.harcamaKod, tf.fisNo, kullanan.kullaniciKodu, tarihceAciklama, tf.kod, tf.ipAdresi);
                            sonuc.bilgiStr = string.Format(TasinirMalResI.TMM288, tf.fisNo) + "<br>";
                        }

                    }
                }

                if (TNS.TMM.Arac.MerkezBankasiKullaniyor() && yeniDurum == "Onay" && tf.BAOnayAdimi != ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI)
                {
                    sonuc = tabTasinirIslemForm.BelgeDurumDegistir(tf, "OnayKaldir");
                    sonuc.bilgiStr = string.Format(TasinirMalResI.TMM288, tf.fisNo) + "<br>";
                }

                //Eğer Cumhurbaşkanlığı ise ve Tüketim giriş fişi ise //eskiden tüketim çıkıştı 18.12.2023 Melih
                if (!string.IsNullOrWhiteSpace(TCCBEntegrasyon) &&
                    tf.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS &&
                    (yeniDurum == "Onay" || yeniDurum == "OnayKaldir"))
                {
                    try
                    {
                        //Burası nedeniyle sicil no çiftliyor. Sanırım hata oluşuyor ve sicilno rollback olmuyor.
                        //Bu nedenle sonuca bakmadan işlem yapılıyor Melih 18.12.2023 Cumhur

                        //Sonuc yedekSonuc = sonuc;
                        //sonuc = TMMTCCBEntegrasyon.Kaydet(kullanan, tf, yeniDurum);
                        //if (sonuc.islemSonuc)
                        //    sonuc = yedekSonuc;

                        TMMTCCBEntegrasyon.Kaydet(kullanan, tf, yeniDurum);
                    }
                    catch (Exception ex)
                    {
                        //sonuc.islemSonuc = false;
                        OrtakFonksiyonlar.HataDosyaYaz("TCCBEntegrasyonHataCatch.txt", yeniDurum + ">" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + ">" + tf.yil + ">" + tf.muhasebeKod + ">" + tf.harcamaKod + ">" + tf.fisNo + ">>> ex.Message:" + ex.Message, false);
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                if (zimmetIslemiYap && tf.BAOnayAdimi == TNS.TMM.ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI)
                    BelgedekileriZimmetle(ve, kullanan, tf);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            //if (sonuc.islemSonuc)
            //{
            //    if (islemTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS)
            //        DagitimIadeGirisFisiKaydet(kullanan, tf);
            //}

            return sonuc;
        }

        private static void BelgedekileriZimmetle(VeriErisim ve, Kullanici kullanan, TasinirIslemForm tf)
        {
            Sonuc sonuc = new Sonuc();
            Hashtable htZimmet = new Hashtable();
            ObjectArray detay = new ObjectArray();

            SicilNoHareket snh = new SicilNoHareket();
            snh.yil = tf.yil;
            snh.muhasebeKod = tf.muhasebeKod;
            snh.harcamaBirimKod = tf.harcamaKod.Replace(".", "");
            snh.sorguFisNoTif = tf.fisNo;
            ObjectArray bilgi = TMMSicilNoHareket.AmbardakiButunSicilNolar(ve, kullanan, snh);

            foreach (TasinirIslemDetay item in tf.detay.objeler)
            {
                foreach (SicilNoHareket td in bilgi.objeler)
                {
                    if (item.hesapPlanKod == td.hesapPlanKod && item.siraNo == td.fisSiraNo && item.yerleskeYeri != "")
                    {
                        td.ozellik.bulunduguYer = item.yerleskeYeri;

                        detay = (ObjectArray)htZimmet[td.ozellik.bulunduguYer];
                        if (detay == null) detay = new ObjectArray();
                        detay.objeler.Add(td);
                        htZimmet[td.ozellik.bulunduguYer] = detay;
                    }
                }
            }

            foreach (DictionaryEntry entry in htZimmet)
            {
                detay = (ObjectArray)htZimmet[entry.Key];

                TNS.TMM.ZimmetForm zimmet = new TNS.TMM.ZimmetForm();
                zimmet.yil = tf.yil;
                zimmet.fisTarih = tf.fisTarih;
                zimmet.fisNo = "";
                zimmet.muhasebeKod = tf.muhasebeKod;
                zimmet.harcamaBirimKod = tf.harcamaKod;
                zimmet.ambarKod = tf.ambarKod;
                zimmet.vermeDusme = (int)ENUMZimmetVermeDusme.ZIMMETVERME;
                zimmet.kimeGitti = "11111111111";/////////////////////////////////////////////
                zimmet.nereyeGitti = (string)entry.Key;
                zimmet.belgeTur = (int)ENUMZimmetBelgeTur.ZIMMETFISI;
                zimmet.islemTarih = new TNSDateTime(DateTime.Now);
                zimmet.islemYapan = tf.islemYapan;
                zimmet.tip = (int)ENUMZimmetTipi.DEMIRBASCIHAZ;

                ObjectArray zimmetDetayArray = new ObjectArray();

                int siraNo = 1;
                foreach (SicilNoHareket td in detay.objeler)
                {
                    ZimmetDetay zimmetDetay = new ZimmetDetay();
                    zimmetDetay.yil = zimmet.yil;
                    zimmetDetay.muhasebeKod = zimmet.muhasebeKod;
                    zimmetDetay.harcamaBirimKod = zimmet.harcamaBirimKod.Replace(".", "");
                    zimmetDetay.belgeTur = zimmet.belgeTur;

                    zimmetDetay.hesapPlanKod = td.hesapPlanKod;
                    zimmetDetay.gorSicilNo = td.sicilNo;
                    zimmetDetay.ozellik = "";
                    zimmetDetay.siraNo = siraNo;
                    zimmetDetay.kdvOran = td.kdvOran;
                    zimmetDetay.birimFiyat = td.kdvliBirimFiyat;
                    zimmetDetay.teslimDurum = "";
                    zimmetDetay.prSicilNo = td.prSicilNo;

                    siraNo++;
                    zimmetDetayArray.Ekle(zimmetDetay);
                }
                sonuc = TMMZimmetForm.Kaydet(ve, kullanan, zimmet, zimmetDetayArray);
                if (sonuc.islemSonuc)
                {
                    zimmet.fisNo = sonuc.anahtar;
                    sonuc = TMMZimmetForm.BelgeDurumDegistir(ve, kullanan, zimmet, "Onay");
                }
            }
        }

        /// <summary>
        /// Taşınır işlem fişinin, muhasebe işlem fişi ile bağını varsa güncelleyen, yoksa kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <param name="belgeNo">Muhasebe işlem fişi belge numarası</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc MIFNoKaydet(Kullanici kullanan, TasinirIslemForm tf, string belgeNo)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABTasinirIslemForm tab = new TABTasinirIslemForm(ve);

            sonuc = tab.MIFNoKaydet(tf, belgeNo);

            if (sonuc.hataStr == "")
            {
                ve.Commit();
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = TasinirMalResI.TMM289;
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM290 + "<br>" + sonuc.hataStr;
                sonuc.islemSonuc = false;
                ve.Rollback();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen taşınır işlem fişlerine bağlı olan
        /// muhasebe işlem fişlerinin belge durumunu iptal yapan yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNolar">Taşınır işlem fişleri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc MIFNoIptal(Kullanici kullanan, ObjectArray sicilNolar)
        {
            Sonuc sonuc = new Sonuc();

            IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABTasinirIslemForm tab = new TABTasinirIslemForm(ve);

            string eskiFisNo = "";
            foreach (SicilNoHareket f in sicilNolar.objeler)
            {
                if (eskiFisNo != f.fisNo)
                {
                    int MIFDurum = 0;
                    string belgeNo = "";
                    if (servisMUH != null)
                    {
                        belgeNo = tab.MIFNover(f.yil, f.muhasebeKod, f.harcamaBirimKod, f.fisNo);
                        MIFDurum = servisMUH.BelgeDurumBilgisi(f.yil, belgeNo);
                    }

                    if (MIFDurum < 5 || MIFDurum == 9)
                    {
                        if (MIFDurum > 0)
                        {
                            f.durum = (int)ENUMBelgeDurumu.IPTAL;
                            tab.MIFNoIptal(f);
                        }
                    }
                    else
                    {
                        sonuc.hataStr = string.Format(TasinirMalResI.TMM291, f.fisNo, belgeNo);
                        sonuc.islemSonuc = false;
                        break;
                    }
                }
                eskiFisNo = f.fisNo;
            }

            if (sonuc.hataStr == "")
            {
                ve.Commit();
                sonuc.islemSonuc = true;
            }
            else
            {
                sonuc.islemSonuc = false;
                ve.Rollback();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem fişlerinin muhasebe işlem fişleri ile bağlarını listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında TasinirIslemMIF nesneleri listesi döndürülür.</returns>
        public static ObjectArray MIFNoListele(Kullanici kullanan, TasinirIslemForm tf)
        {
            ObjectArray liste = new ObjectArray();
            string hata = "";

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)))
            {
                if (tf.muhasebeKod != "")
                    hata += TasinirMalResI.TMM013 + "<br>";

                if (tf.harcamaKod != "")
                    hata += TasinirMalResI.TMM014;

                if (hata != "")
                {
                    liste.sonuc.islemSonuc = false;
                    liste.sonuc.hataStr = hata;
                    return liste;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTasinirIslemForm tab = new TABTasinirIslemForm(ve);

            IDataReader r = null;

            try
            {
                r = tab.MIFNoListele(tf);
            }
            catch (Exception e)
            {
                r.Close();
                ve.VeriErisimKapat();
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;

                return liste;
            }

            while (r.Read())
            {
                TasinirIslemMIF mif = new TasinirIslemMIF();
                mif.yil = VeriErisim.DegerAl(r, 0, 0);
                mif.muhasebeKod = VeriErisim.DegerAl(r, 1, "");
                mif.harcamaKod = VeriErisim.DegerAl(r, 2, "");
                mif.tifBelgeNo = VeriErisim.DegerAl(r, 3, "");
                mif.mifBelgeNo = VeriErisim.DegerAl(r, 4, "");
                mif.durum = VeriErisim.DegerAl(r, 5, 0);
                mif.mifBelgeTur = VeriErisim.DegerAl(r, 6, 0);

                mif.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), mif.harcamaKod);
                liste.objeler.Add(mif);
            }

            if (liste.objeler.Count > 0)
            {
                liste.sonuc.anahtar = ((TasinirIslemMIF)liste.objeler[0]).mifBelgeNo;
                liste.sonuc.islemSonuc = true;
            }
            else
            {
                liste.sonuc.hataStr = TasinirMalResI.TMM029;
                liste.sonuc.islemSonuc = false;
            }
            r.Close();
            ve.VeriErisimKapat();

            return liste;
        }

        /// <summary>
        /// Taşınır işlem fişinin, muhasebe işlem fişi ile bağını varsa güncelleyen, yoksa kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <param name="belgeNo">Muhasebe işlem fişi belge numarası</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc SurecNoKaydet(Kullanici kullanan, TasinirIslemForm tf, string belgeNo)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABTasinirIslemForm tab = new TABTasinirIslemForm(ve);

            sonuc = tab.SurecNoKaydet(tf, belgeNo);

            if (sonuc.hataStr == "")
            {
                ve.Commit();
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = TasinirMalResI.TMM289;
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM290 + "<br>" + sonuc.hataStr;
                sonuc.islemSonuc = false;
                ve.Rollback();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem fişlerinin muhasebe işlem fişleri ile bağlarını listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında TasinirIslemMIF nesneleri listesi döndürülür.</returns>
        public static ObjectArray SurecNoListele(TasinirIslemForm tf)
        {
            ObjectArray liste = new ObjectArray();
            string hata = "";

            if (tf.muhasebeKod == "")
                hata += TasinirMalResI.TMM013 + "<br>";

            if (tf.harcamaKod == "")
                hata += TasinirMalResI.TMM014;

            if (hata != "")
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = hata;
                return liste;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM");

            TABTasinirIslemForm tab = new TABTasinirIslemForm(ve);

            IDataReader r = null;

            try
            {
                r = tab.SurecNoListele(tf);
            }
            catch (Exception e)
            {
                r.Close();
                ve.VeriErisimKapat();
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;

                return liste;
            }

            while (r.Read())
            {
                TasinirIslemMIF mif = new TasinirIslemMIF();
                mif.yil = VeriErisim.DegerAl(r, 0, 0);
                mif.muhasebeKod = VeriErisim.DegerAl(r, 1, "");
                mif.harcamaKod = VeriErisim.DegerAl(r, 2, "");
                mif.tifBelgeNo = VeriErisim.DegerAl(r, 3, "");
                mif.mifBelgeNo = VeriErisim.DegerAl(r, 4, "");

                mif.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), mif.harcamaKod);
                liste.objeler.Add(mif);
            }

            if (liste.objeler.Count > 0)
            {
                liste.sonuc.anahtar = ((TasinirIslemMIF)liste.objeler[0]).mifBelgeNo;
                liste.sonuc.islemSonuc = true;
            }
            else
            {
                liste.sonuc.hataStr = TasinirMalResI.TMM029;
                liste.sonuc.islemSonuc = false;
            }
            r.Close();
            ve.VeriErisimKapat();

            return liste;
        }

        /// <summary>
        /// Verilen yıl, muhasebe birimi ve harcama birimi bazında son taşınır işlem
        /// fişi iptal edilmişse belge numarasını, değilse boşluk döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcamaKod">Harcama birimi</param>
        /// <returns>Belge numarası döndürülür.</returns>
        public static string HarcamaYonetimiIcinTIFNoVer(int yil, string muhasebe, string harcamaKod)
        {
            if (yil <= 0 || muhasebe.Trim() == "" || harcamaKod.Trim() == "")
                return "";

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABTasinirIslemForm tab = new TABTasinirIslemForm(ve);
            IDataReader r = tab.HarcamaYonetimiIcinTIFNoVer(yil, muhasebe, harcamaKod);

            string belgeNo = "";
            int durum = 0;
            if (r.Read())
            {
                //En son TİF'in durumuna bak iptal edilmiş ise kullan
                belgeNo = VeriErisim.DegerAl(r, 0, "");
                durum = VeriErisim.DegerAl(r, 1, 0);
            }

            if (durum != (int)ENUMBelgeDurumu.IPTAL)
                belgeNo = "";

            r.Close();
            ve.VeriErisimKapat();

            return belgeNo;
        }

        /// <summary>
        /// Dagitims the iade giris fisi kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tf">The tf.</param>
        /// <returns></returns>
        public static Sonuc DagitimIadeGirisFisiKaydet(Kullanici kullanan, TasinirIslemForm tf)
        {
            Sonuc sonuc = new Sonuc();

            ObjectArray listeFis = FisAc(null, kullanan, tf, true);

            if (!listeFis.sonuc.islemSonuc || listeFis.objeler.Count <= 0)
                return listeFis.sonuc;

            tf = (TasinirIslemForm)listeFis[0];

            if (tf.islemTipTur != (int)ENUMIslemTipi.DAGITIMIADECIKIS)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = tf.fisNo + " numaralı fiş dağıtım/iade çıkış fişi değil";
                return sonuc;
            }

            int yil = tf.yil;
            string m = tf.muhasebeKod;
            string h = tf.harcamaKod;
            string a = tf.ambarKod;
            string mg = tf.gMuhasebeKod;
            string hg = tf.gHarcamaKod;
            string ag = tf.gAmbarKod;
            string fisNo = tf.fisNo;

            tf.muhasebeKod = mg;
            tf.harcamaKod = hg;
            tf.ambarKod = ag;
            tf.gYil = yil;
            tf.gMuhasebeKod = m;
            tf.gHarcamaKod = h;
            tf.gAmbarKod = a;
            tf.fisNo = "";
            tf.dayanakNo = fisNo;
            tf.gFisNo = fisNo;
            tf.islemTipTur = (int)ENUMIslemTipi.DAGITIMIADEGIRIS;

            sonuc = Kaydet(null, kullanan, tf);
            if (sonuc.islemSonuc)
                sonuc = BelgeDurumDegistir(null, kullanan, tf, "Onay");

            if (sonuc.islemSonuc)
                DagitimIadeGirisNumaraKaydet(kullanan, yil, m, h, fisNo, fisNo);

            return sonuc;
        }

        /// <summary>
        /// Dagitims the iade giris numara kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="harcamaKod">The harcama kod.</param>
        /// <param name="fisNo">The fis no.</param>
        /// <param name="yeniFisNo">The yeni fis no.</param>
        /// <returns></returns>
        public static Sonuc DagitimIadeGirisNumaraKaydet(Kullanici kullanan, int yil, string muhasebe, string harcamaKod, string fisNo, string yeniFisNo)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABTasinirIslemForm tab = new TABTasinirIslemForm(ve);

            sonuc = tab.DagitimIadeNoKaydet(yil, muhasebe, harcamaKod, fisNo, yeniFisNo);

            if (sonuc.hataStr == "")
            {
                ve.Commit();
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = TasinirMalResI.TMM289;
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM290 + "<br>" + sonuc.hataStr;
                sonuc.islemSonuc = false;
                ve.Rollback();
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc DevirGirisTIFKaydetOnayla(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm giris, TNSCollection cikislar)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            string basariMesaji = TasinirMalResI.TMM335;
            for (int i = 0; i < cikislar.Count; i++)
            {
                TasinirIslemForm cikis = (TasinirIslemForm)cikislar[i];

                Kullanici kullananGiris = new Kullanici();
                kullananGiris.kullaniciKodu = "stratek";
                kullananGiris.kullaniciTipi = new ENUMKullaniciTipi[1];
                kullananGiris.kullaniciTipi[0] = ENUMKullaniciTipi.SISTEMYONETICISI;

                ObjectArray devirGirisFisi = FisAc(ve, kullananGiris, cikis, true);
                if (!devirGirisFisi.sonuc.islemSonuc || devirGirisFisi.objeler.Count != 1)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = devirGirisFisi.sonuc.hataStr;
                    break;
                }

                cikis = (TasinirIslemForm)devirGirisFisi.objeler[0];
                giris.gYil = cikis.yil;
                giris.gMuhasebeKod = cikis.muhasebeKod;
                giris.gHarcamaKod = cikis.harcamaKod;
                giris.gAmbarKod = cikis.ambarKod;
                giris.gFisNo = cikis.fisNo;
                giris.detay = cikis.detay;
                foreach (TasinirIslemDetay detay in cikis.detay.objeler)
                {
                    detay.yil = giris.yil;
                    detay.muhasebeKod = giris.muhasebeKod;
                    detay.harcamaKod = giris.harcamaKod;
                    detay.ambarKod = giris.ambarKod;
                    detay.gonMuhasebeKod = giris.gMuhasebeKod;
                    detay.gonHarcamaKod = giris.gHarcamaKod;

                }

                giris.fisNo = string.Empty;

                if (Arac.MerkezBankasiKullaniyor())
                    giris.parselNo = "YetkiyeBakmadanKaydet"; //Diğer birime yetkisi olmasa da devir giriş kaydı yapılabilsin. 19.11.2019 - Huseyin

                sonuc = Kaydet(ve, kullanan, giris);
                if (!sonuc.islemSonuc)
                    break;
                giris.fisNo = sonuc.anahtar.Split('-')[0];


                if (Arac.MerkezBankasiKullaniyor())
                {
                    //B-A Onayına gönerilmesi istenmiyor. 19.11.2019 - Huseyin

                    //B-A onayına gönder.
                    //TNS.TMM.TasinirIslemForm tf = new TNS.TMM.TasinirIslemForm();

                    //tf.kod = giris.kod;
                    //tf.yil = giris.yil;
                    //tf.fisNo = giris.fisNo;
                    //tf.muhasebeKod = giris.muhasebeKod;
                    //tf.harcamaKod = giris.harcamaKod;
                    //tf.onayAciklama = "";

                    //sonuc = TMMTasinirIslemForm.OnayDurumDegistir(ve, kullanan, tf, ENUMTasinirIslemFormOnayDurumu.GONDERILDIB);
                    //if (!sonuc.islemSonuc)
                    //    break;
                }
                else
                {
                    sonuc = BelgeDurumDegistir(ve, kullanan, giris, "Onay");
                    if (!sonuc.islemSonuc)
                        break;
                }
                sonuc.anahtar = giris.fisNo;

                if (!string.IsNullOrEmpty(basariMesaji))
                    basariMesaji += "<br />";
                basariMesaji += TasinirMalResI.TMM333 + giris.yil + "-" + giris.muhasebeKod + "-" + giris.harcamaKod + "-" + giris.ambarKod + "-" + giris.fisNo;
                basariMesaji += " / " + TasinirMalResI.TMM334 + cikis.yil + "-" + cikis.muhasebeKod + "-" + cikis.harcamaKod + "-" + cikis.ambarKod + "-" + cikis.fisNo;
            }

            if (sonuc.islemSonuc)
                sonuc.bilgiStr = TasinirMalResI.TMM017 + "<br />" + basariMesaji;
            else
                sonuc.hataStr = TasinirMalResI.TMM018 + "<br />" + basariMesaji + "<br />" + sonuc.hataStr;

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Gümrük Bakanlığı kayıtlarını aktarma için
        /// </summary>
        /// <returns></returns>
        public static Sonuc GumrukKaydet()
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABTasinirIslemForm tabTasinirIslemForm = new TABTasinirIslemForm(ve);

            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABTasinirIslemForm tabTasinirIslemForm2 = new TABTasinirIslemForm(ve2);

            ve.BeginTransaction();
            ve2.BeginTransaction();
            IDataReader dr = tabTasinirIslemForm.GumrukTIFAl();
            while (dr.Read())
            {
                string muhasebe = VeriErisim.DegerAl(dr, 0, "");
                string birim = VeriErisim.DegerAl(dr, 1, "");
                string eskiFisNo = VeriErisim.DegerAl(dr, 2, "");

                string kod = "TIF2011" + muhasebe + birim;
                string yeniFisNo = tabTasinirIslemForm2.GumrukNoAl(kod);
                yeniFisNo = (OrtakFonksiyonlar.ConvertToInt(yeniFisNo, 0) + OrtakFonksiyonlar.ConvertToInt(eskiFisNo, -1)).ToString();
                yeniFisNo = yeniFisNo.PadLeft(6, '0');

                sonuc = tabTasinirIslemForm2.GumrukGuncelle(muhasebe, birim, yeniFisNo, eskiFisNo);
                if (!sonuc.islemSonuc)
                    break;
            }

            dr.Close();

            if (sonuc.islemSonuc)
            {

                ve.Commit();
                ve2.Commit();
            }
            else
            {
                ve.Rollback();
                ve2.Rollback();
            }

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc VTAlanUzunlukKontrolEt(TasinirIslemForm tif)
        {
            int muhasebeUzunluk = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD12/KODYAPI").Length;
            int harcamaUzunluk = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI").Replace(".", "").Length;
            int ambarUzunluk = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD11/KODYAPI").Length;

            Sonuc sonuc = new Sonuc();

            //TASISLEMFORM tablosu alanları kontrolü
            if (tif.muhasebeKod.Length > muhasebeUzunluk)
                sonuc.hataStr = TasinirMalResI.TMM337 + "<br>";
            if (tif.harcamaKod.Replace(".", "").Length > harcamaUzunluk)
                sonuc.hataStr += TasinirMalResI.TMM338 + "<br>";
            if (tif.fisNo.Length > 10)
                sonuc.hataStr += TasinirMalResI.TMM339 + "<br>";
            if (tif.ambarKod.Length > ambarUzunluk)
                sonuc.hataStr += TasinirMalResI.TMM371 + "<br>";
            if (tif.muayeneNo.Length > 20)
                sonuc.hataStr += TasinirMalResI.TMM372 + "<br>";
            if (tif.dayanakNo.Length > 250)
                sonuc.hataStr += TasinirMalResI.TMM373 + "<br>";
            if (tif.neredenGeldi.Length > 255)
                sonuc.hataStr += TasinirMalResI.TMM353 + "<br>";
            if (tif.nereyeGitti.Length > 255)
                sonuc.hataStr += TasinirMalResI.TMM374 + "<br>";
            if (tif.gMuhasebeKod.Length > muhasebeUzunluk)
                sonuc.hataStr = TasinirMalResI.TMM375 + "<br>";
            if (tif.gHarcamaKod.Replace(".", "").Length > harcamaUzunluk)
                sonuc.hataStr += TasinirMalResI.TMM376 + "<br>";
            if (tif.gAmbarKod.Length > ambarUzunluk)
                sonuc.hataStr += TasinirMalResI.TMM377 + "<br>";
            if (tif.islemYapan.Length > 50)
                sonuc.hataStr += TasinirMalResI.TMM378 + "<br>";
            if (tif.faturaNo.Length > 250)
                sonuc.hataStr += TasinirMalResI.TMM379 + "<br>";
            if (tif.kimeGitti.Length > 11)
                sonuc.hataStr += TasinirMalResI.TMM380 + "<br>";
            if (tif.gFisNo.Length > 10)
                sonuc.hataStr += TasinirMalResI.TMM381 + "<br>";

            Sonuc detaySonuc = TMMTasinirIslemDetay.VTAlanUzunlukKontrolEt(tif.detay.objeler);
            if (!detaySonuc.islemSonuc)
                sonuc.hataStr += "<br>" + detaySonuc.hataStr;

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;

            return sonuc;
        }

        public static Sonuc KHKFisNoGuncelle(Kullanici kullanan, TasinirIslemForm tf)
        {
            Sonuc sonuc = new Sonuc();

            if (tf.yil == 0 || tf.muhasebeKod == "" || tf.harcamaKod == "" || tf.fisNo == "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Bilgilerde hata var";
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTasinirIslemForm tabTasinirIslemForm = new TABTasinirIslemForm(ve);

            sonuc = tabTasinirIslemForm.KHKFisNoGuncelle(tf);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc OnayDurumDegistir(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf, ENUMTasinirIslemFormOnayDurumu yeniDurum)
        {
            Sonuc sonuc = new Sonuc();

            if (string.IsNullOrEmpty(tf.kod))
            {
                sonuc.DegerAta("Sistem kod bilgisi boş. Lütfen sistem yöneticinize başvurunuz!");
                return sonuc;
            }

            tf.islemYapan = kullanan.kullaniciKodu;
            ObjectArray listeTarihce = null;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTasinirIslemForm tabTasinirIslemForm = new TABTasinirIslemForm(ve);

            //**************Onay Durum Kontrolü****************************************

            string bilgi = "";

            ENUMTasinirIslemFormOnayDurumu onayDurum = (ENUMTasinirIslemFormOnayDurumu)tabTasinirIslemForm.OnayDurumGetirTekKolon(tf);

            int belgeDurum = tabTasinirIslemForm.BelgeDurumGetirTekKolon(tf);

            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
            {
                listeTarihce = new TMMTarihce().IslemFormListele(kullanan, tf.yil, tf.muhasebeKod, tf.harcamaKod, tf.fisNo);

                if (onayDurum == ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI)
                    sonuc.DegerAta("Belge onaylı olduğu için onaya gönderilemez!<br>");
                else if (onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIA && yeniDurum != ENUMTasinirIslemFormOnayDurumu.GERIGONDERILDI)
                {
                    //B onayına yapan kişi A onayını yapamaz. Kontrolü eklenecek
                    string bOnayiVerenKisi = "";
                    foreach (TarihceBilgisi tarihce in listeTarihce.objeler)
                    {
                        if (tarihce.islem == "A Onayına Gönderildi")
                            bOnayiVerenKisi = tarihce.islemYapan;
                    }

                    if (bOnayiVerenKisi == kullanan.kullaniciKodu)
                        sonuc.DegerAta("B onayını veren kullanıcı A onayını veremez!<br>");
                }

                if (sonuc.hataStr == "")
                {      //B veya A Onay yetkisi varmı kontrol edilecek.
                    ObjectArray baYetki = TMMTasinirIslemOnayYetki.Listele(kullanan, new TasinirIslemOnayYetki() { muhasebeKod = tf.muhasebeKod, harcamaBirimKod = tf.harcamaKod, mernis = kullanan.mernis });
                    bool aOnayiYetksisiVar = false;
                    bool bOnayiYetksisiVar = false;
                    foreach (TNS.TMM.TasinirIslemOnayYetki yetki in baYetki.objeler)
                    {
                        if (yetki.onayTur == (int)ENUMTasinirIslemFormOnayDurumu.GONDERILDIA)
                        {
                            aOnayiYetksisiVar = true;
                            bOnayiYetksisiVar = true;
                        }
                        else
                        {
                            bOnayiYetksisiVar = true;
                        }
                    }

                    if (onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIA && !aOnayiYetksisiVar)
                        sonuc.DegerAta(tf.harcamaKod + " birimi için A Onay yetkiniz bulunmamaktarıdr!<br>");
                    else if (onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIB && !bOnayiYetksisiVar)
                        sonuc.DegerAta(tf.harcamaKod + " birimi için B Onay yetkiniz bulunmamaktarıdr!<br>");
                    //********************************************************************************************
                }

            }
            else
            {
                if (belgeDurum == (int)ENUMBelgeDurumu.ONAYLI)
                    sonuc.DegerAta("Belge onaylı olduğu için onaya gönderilemez!<br>");
            }

            if (belgeDurum == (int)ENUMBelgeDurumu.IPTAL)
                sonuc.DegerAta("Belge iptal edildiği için onaya gönderilemez!<br>");

            if (sonuc.hataStr == "")
            {
                if ((onayDurum == ENUMTasinirIslemFormOnayDurumu.TANIMSIZ || onayDurum == ENUMTasinirIslemFormOnayDurumu.GERIGONDERILDI) && yeniDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIB)//B-Onayına gönderildi
                    bilgi = "B Onayına Gönderildi";
                else if (onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIB && yeniDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIA)//A-Onayına gönderildi
                    bilgi = "A Onayına Gönderildi";
                else if (onayDurum == ENUMTasinirIslemFormOnayDurumu.GONDERILDIA && yeniDurum == ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI)//A-Onaylandı
                    bilgi = TasinirMalResI.TMM009;
                else if (yeniDurum == ENUMTasinirIslemFormOnayDurumu.GERIGONDERILDI)
                    bilgi = "B-A Onayından Geri Gonderildi";
                else
                    sonuc.DegerAta("Onay durumu uygun değil!<br>");
                //******************************************************
            }

            if (sonuc.hataStr != "")
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string onayAciklama = tf.onayAciklama;

            ObjectArray listeFis = FisAc(null, kullanan, tf, true);
            if (listeFis.ObjeSayisi < 1)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM276;
                ve.VeriErisimKapat();
                return sonuc;
            }

            TasinirIslemForm tfIslem = (TasinirIslemForm)listeFis[0];


            //Nuvara verme işlemi yapılmadan B/A onayına gönderilmesin
            if (TNS.TMM.Arac.MerkezBankasiKullaniyor() && listeTarihce != null && tfIslem.ambarKod != "50")
            {
                bool sicilNoVarMi = false;

                if (tfIslem.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS || tfIslem.islemTipTur == (int)ENUMIslemTipi.BAGISGIRIS)
                {
                    foreach (TarihceBilgisi tarihce in listeTarihce.objeler)
                    {
                        if (tarihce.islem == "Onaylandı")
                            sicilNoVarMi = true;
                    }
                }
                else
                    sicilNoVarMi = true;

                if (!sicilNoVarMi)
                {
                    ve.VeriErisimKapat();
                    sonuc.hataStr += "Numara verme işlemi yapılmadan B/A onayına gönderilemez.<br>";
                    sonuc.islemSonuc = false;
                    return sonuc;
                }

            }
            //********************************



            //İhraç işleminde bütün siciller için geçici ihraçta onay bekleyen açık fiş var mı? Varsa öncelikle geçici ihtaç B/A onayı tamamlanmalı. Daha sonra ihraç B/A onayı tamamlanmalı.
            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
            {
                bool sicilNoVarMi = false;

                if (tfIslem.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKISKURUM && tfIslem.islemTipKod == 23)
                {
                    string sicilNo = "";

                    foreach (TasinirIslemDetay detay in tfIslem.detay.objeler)
                    {
                        if (sicilNo != "")
                            sicilNo += ",";

                        sicilNo += detay.sicilNo;
                    }

                    if (sicilNo != "")
                        sicilNoVarMi = TMMTasinirIslemForm.OnaylanmamisGeciciIhracKaydiVarMi(ve, sicilNo);
                }


                if (sicilNoVarMi)
                {
                    ve.VeriErisimKapat();
                    sonuc.hataStr += "İHRAÇ kayıtlarınızda geçici ihraç işlemi yapılmış ancak henüz onaylanmamış kayıtlar mevcut. <br>Öncelikle GEÇİCİ İHRAÇ işleminin B/A onayını tamamlayınız.<br>";
                    sonuc.islemSonuc = false;
                    return sonuc;
                }

            }
            //********************************




            sonuc = Kontrol(null, kullanan, tfIslem, true);
            if (sonuc.hataStr != "")
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                return sonuc;
            }

            bool muhasebeKaydiOlustur = false;

            if (sonuc.hataStr == "" && sonuc.islemSonuc)
            {
                ve.BeginTransaction();

                bool fisTarihiDegistir = false;
                if (yeniDurum == ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI && TNS.TMM.Arac.MerkezBankasiKullaniyor())
                {
                    if (!(tfIslem.fisTarih.Ay == 12 && tfIslem.fisTarih.Gun == 31) && (tfIslem.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS || tfIslem.islemTipTur == (int)ENUMIslemTipi.BAGISGIRIS))
                    {
                        tf.fisTarih = new TNSDateTime(DateTime.Now.Date);
                        fisTarihiDegistir = true;
                    }

                    tf.BAOnayAdimi = yeniDurum;
                    if (tf.durum != (int)ENUMBelgeDurumu.ONAYLI)
                        sonuc = BelgeDurumDegistir(ve, kullanan, tf, "Onay");

                    //Merkez Bankası Muhasebe bilgi kaydı oluştur.
                    if (sonuc.islemSonuc)
                        muhasebeKaydiOlustur = true;

                }

                if (sonuc.islemSonuc)
                {
                    if (fisTarihiDegistir)
                        tf.fisTarih = new TNSDateTime(DateTime.Now.Date);
                    sonuc = tabTasinirIslemForm.OnayDurumDegistir(tf, yeniDurum, onayAciklama);
                }

                if (sonuc.islemSonuc)//&& yeniDurum != ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI)
                {
                    tf.islemYapan = kullanan.kullaniciKodu;
                    tf.ipAdresi = kullanan.telefon;
                    tabTasinirIslemForm.KaydetTarihce(tf.yil, tf.muhasebeKod, tf.harcamaKod, tf.fisNo, kullanan.kullaniciKodu, bilgi, tf.kod, tf.ipAdresi);
                    sonuc.bilgiStr = string.Format("{0} belgenin onay durumu değiştirildi.", tf.fisNo) + "<br>";
                }
            }


            bool commit = false;

            if (sonuc.islemSonuc)
            {
                if (Arac.MerkezBankasiKullaniyor())
                {
                    if (muhasebeKaydiOlustur)
                    {
                        bool muhasebeKaydiZorunlu = false;

                        AyarlarMB ayarlar = new AyarlarMB
                        {
                            muhasebeKod = tf.muhasebeKod,
                            harcamaKod = tf.harcamaKod,
                            ambarKod = tf.ambarKod
                        };

                        ObjectArray liste = TMMAyarlarMB.Listele(ve, kullanan, ayarlar);
                        if (liste.sonuc.islemSonuc && liste.objeler.Count == 1)
                        {
                            AyarlarMB a = (AyarlarMB)liste.objeler[0];
                            muhasebeKaydiZorunlu = a.aOnayiMuhasebeKontrolu == 1 ? true : false;
                        }

                        if (!muhasebeKaydiZorunlu)
                            commit = true;


                        //Merkez Bankası Muhasebe bilgi kaydı oluştur.
                        MuhasebeIslemiKriter mk = new MuhasebeIslemiKriter()
                        {
                            yil = tf.yil,
                            muhasebeKod = tf.muhasebeKod,
                            harcamaKod = tf.harcamaKod,
                            ambarKod = tf.ambarKod,
                            fisNo = tf.fisNo,
                            muhasebeIslemTur = ENUMMuhasebeIslemTur.TASINIRISLEMFISI,
                            islemTipTur = tfIslem.islemTipTur,
                            islemTipKod = tfIslem.islemTipKod
                        };

                        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();
                        mk.mutemmim = servisTMM.MutemmimAktifMi(kullanan, new AyarlarMB { muhasebeKod = tf.muhasebeKod, harcamaKod = tf.harcamaKod, ambarKod = tf.ambarKod }) ? 1 : 0;

                        Sonuc sonuc2 = TMMMuhasebeBilgi.Kaydet(ve, kullanan, mk);
                        if (!sonuc2.islemSonuc)
                            TMMMuhasebeBilgi.HataDosyaYaz("ENUMMuhasebeIslemTur:" + mk.muhasebeIslemTur + " - ", sonuc.hataStr);
                        else if (muhasebeKaydiZorunlu)
                            commit = true;

                    }
                    else
                        commit = true;
                }
                else
                    commit = true;
            }

            if (commit)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();


            if (commit && Arac.MerkezBankasiKullaniyor())
            {
                if (yeniDurum == ENUMTasinirIslemFormOnayDurumu.TAMAMLANDI && tfIslem.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS)
                {
                    Sonuc devirGirisSonuc = DevirGirisYap(null, kullanan, tfIslem);
                    if (!devirGirisSonuc.islemSonuc)
                    {
                        //Buraya giriyorsa bilinmeyen bir hata var.
                        sonuc.hataStr = string.Format("{0} belge için devir çıkış işlemi onayladı. Ancak devir giriş alacak birimde kayıt sırasında bir hata oluştu.<br><br>Detay:<br>{1}", tf.fisNo, devirGirisSonuc.hataStr) + "<br>";
                        sonuc.islemSonuc = false;
                    }
                    DevirCikisYapildiMailAt(kullanan, tfIslem);
                }
                else
                {
                    if (bilgi == "B Onayına Gönderildi" || bilgi == "A Onayına Gönderildi")
                        BAOnayinaGonderildiMailAt(kullanan, tfIslem, bilgi);
                }
            }

            return sonuc;
        }

        public static Sonuc DevirGirisYap(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tfIslem)
        {

            TNS.TMM.TasinirIslemForm giris = new TNS.TMM.TasinirIslemForm();
            giris.yil = tfIslem.gYil;
            giris.muhasebeKod = tfIslem.gMuhasebeKod;
            giris.harcamaKod = tfIslem.gHarcamaKod.Replace(".", "");
            giris.ambarKod = tfIslem.gAmbarKod.Replace(".", "");

            giris.devirGirisiMi = true;
            giris.islemTipTur = (int)ENUMIslemTipi.DEVIRGIRIS;
            //giris.islemTipKod = 0;
            //if (giris.islemTipKod == 0)
            //{
            UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
            giris.islemTipKod = (int)servisUZY.UzayDegeriDbl(kullanan, "TASISLEMTIPKOD", ((int)ENUMIslemTipi.DEVIRGIRIS).ToString(), true, "");
            //}

            giris.fisTarih.Yaz(DateTime.Now.Date);

            if (giris.fisTarih.Yil > giris.yil)
                giris.fisTarih = new TNSDateTime("31.12." + giris.yil);

            giris.islemTarih = new TNSDateTime(DateTime.Now);
            giris.islemYapan = kullanan.kullaniciKodu;

            TNSCollection cikislar = new TNSCollection();

            TNS.TMM.TasinirIslemForm cikis = new TNS.TMM.TasinirIslemForm();
            cikis.yil = giris.yil;
            cikis.muhasebeKod = tfIslem.muhasebeKod;
            cikis.harcamaKod = tfIslem.harcamaKod;
            cikis.ambarKod = tfIslem.ambarKod;
            cikis.fisNo = tfIslem.fisNo;

            cikislar.Add(cikis);

            Sonuc sonuc = DevirGirisTIFKaydetOnayla(veriErisim, kullanan, giris, cikislar);

            return sonuc;


            //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            //string basariMesaji = TasinirMalResI.TMM335;
            //for (int i = 0; i < cikislar.Count; i++)
            //{
            //    TasinirIslemForm cikis = (TasinirIslemForm)cikislar[i];

            //    Kullanici kullananGiris = new Kullanici();
            //    kullananGiris.kullaniciKodu = "stratek";
            //    kullananGiris.kullaniciTipi = new ENUMKullaniciTipi[1];
            //    kullananGiris.kullaniciTipi[0] = ENUMKullaniciTipi.SISTEMYONETICISI;

            //    ObjectArray devirGirisFisi = FisAc(ve, kullananGiris, cikis, true);
            //    if (!devirGirisFisi.sonuc.islemSonuc || devirGirisFisi.objeler.Count != 1)
            //    {
            //        sonuc.islemSonuc = false;
            //        sonuc.hataStr = devirGirisFisi.sonuc.hataStr;
            //        break;
            //    }

            //    cikis = (TasinirIslemForm)devirGirisFisi.objeler[0];
            //    giris.gYil = cikis.yil;
            //    giris.gMuhasebeKod = cikis.muhasebeKod;
            //    giris.gHarcamaKod = cikis.harcamaKod;
            //    giris.gAmbarKod = cikis.ambarKod;
            //    giris.gFisNo = cikis.fisNo;
            //    giris.detay = cikis.detay;
            //    foreach (TasinirIslemDetay detay in cikis.detay.objeler)
            //    {
            //        detay.yil = giris.yil;
            //        detay.muhasebeKod = giris.muhasebeKod;
            //        detay.harcamaKod = giris.harcamaKod;
            //        detay.ambarKod = giris.ambarKod;
            //        detay.gonMuhasebeKod = giris.gMuhasebeKod;
            //        detay.gonHarcamaKod = giris.gHarcamaKod;
            //    }

            //    giris.fisNo = string.Empty;
            //    sonuc = Kaydet(ve, kullanan, giris);
            //    if (!sonuc.islemSonuc)
            //        break;
            //    giris.fisNo = sonuc.anahtar.Split('-')[0];

            //    sonuc = BelgeDurumDegistir(ve, kullanan, giris, "Onay");
            //    if (!sonuc.islemSonuc)
            //        break;

            //    sonuc.anahtar = giris.fisNo;

            //    if (!string.IsNullOrEmpty(basariMesaji))
            //        basariMesaji += "<br />";
            //    basariMesaji += TasinirMalResI.TMM333 + giris.yil + "-" + giris.muhasebeKod + "-" + giris.harcamaKod + "-" + giris.ambarKod + "-" + giris.fisNo;
            //    basariMesaji += " / " + TasinirMalResI.TMM334 + cikis.yil + "-" + cikis.muhasebeKod + "-" + cikis.harcamaKod + "-" + cikis.ambarKod + "-" + cikis.fisNo;
            //}

            //if (sonuc.islemSonuc)
            //    sonuc.bilgiStr = TasinirMalResI.TMM017 + "<br />" + basariMesaji;
            //else
            //    sonuc.hataStr = TasinirMalResI.TMM018 + "<br />" + basariMesaji + "<br />" + sonuc.hataStr;

            //ve.VeriErisimKapat();

            //return sonuc;



        }

        public static void BAOnayinaGonderildiMailAt(Kullanici kullanan, TasinirIslemForm tfIslem, string onayTur)
        {
            string eposta = "";

            //Fişin ait olduğu birime yetkili kullanıcıları bul
            TasinirIslemOnayYetki ty = new TasinirIslemOnayYetki();
            ty.muhasebeKod = tfIslem.muhasebeKod;
            ty.harcamaBirimKod = tfIslem.harcamaKod;
            if (onayTur.StartsWith("A"))
                ty.onayTur = (int)ENUMTasinirIslemFormOnayTur.AONAYI;
            else if (onayTur.StartsWith("B"))
                ty.onayTur = (int)ENUMTasinirIslemFormOnayTur.BONAYI;

            ObjectArray bilgi = TMMTasinirIslemOnayYetki.Listele(kullanan, ty);
            foreach (TNS.TMM.TasinirIslemOnayYetki yetki in bilgi.objeler)
            {
                if (eposta != "") eposta += ";";
                eposta += yetki.eposta;
            }

            string yazilimAd = System.Configuration.ConfigurationManager.AppSettings.Get("YazilimAd");
            string yazilimMailAdresi = System.Configuration.ConfigurationManager.AppSettings.Get("YazilimMailAdresi");

            string icerik = "Sayın ilgili,<br />Aşağıda bilgisi bulunan evrak onaylamanız için gönderilmiştir.<br /><br />Belge No:" + tfIslem.fisNo + "<br />Belge Tarihi:" + tfIslem.fisTarih + "<br /> Harcama Birimi:" + tfIslem.harcamaAd + "<br />İşlem Tipi:" + tfIslem.islemTipAd + "<br />Onay Türü:" + onayTur + "<br/><br/>İyi çalışmalar.<br>" + yazilimAd;

            OrtakFonksiyonlar.MailAtThread(yazilimMailAdresi, eposta, "", "", "Evrak Onayı", icerik, true, false, null);

            //OrtakFonksiyonlar.MailAt(yazilimMailAdresi, eposta, "Evrak Onayı", icerik, true, false, new TNSCollection());
        }

        public static ObjectArray BAOnayiYapilacaklarListele(Kullanici kullanan, string mernis, BAOnay kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            if (kriter.onayDurum != (int)ENUMTasinirIslemFormOnayDurumu.ONAY)
            {
                bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kriter.muhasebeKod, kriter.harcamaKod, kriter.ambarKod);

                if (!bilgi.sonuc.islemSonuc)
                {
                    bilgi.sonuc.hataStr = TasinirMalResI.TMM060;
                    return bilgi;
                }
            }
            else
            {
                string yetkiliBirimler = TMMYetkiKontrol.YetkisiOlanBirimlerMB(kullanan, kriter.muhasebeKod);
                if (!string.IsNullOrEmpty(yetkiliBirimler))
                    kriter.harcamaKod = yetkiliBirimler;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTasinirIslemForm tabTasinirIslemForm = new TABTasinirIslemForm(ve);


            IDataReader dr = tabTasinirIslemForm.BAOnayiYapilacaklarListele(mernis, kriter);

            if (ve.hata != "")
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr += ve.hata;
                bilgi.sonuc.vtHatasi = true;

                if (dr != null)
                    dr.Close();

                ve.VeriErisimKapat();

                return bilgi;
            }

            Hashtable tipKodList = new Hashtable();

            int sonSiraNo = (int)TABTasinirIslemForm.ENUMAlanlar.ONAYACIKLAMA;
            while (dr.Read())
            {
                TasinirIslemForm tasForm = new TasinirIslemForm();

                tasForm.yil = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.YIL, 0);
                tasForm.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.MUHASEBEKOD, string.Empty);
                tasForm.harcamaKod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.HARCAMABIRIMKOD, string.Empty);
                tasForm.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), tasForm.harcamaKod);
                tasForm.fisNo = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.FISNO, string.Empty);

                tasForm.fisTarih = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.FISTARIH);
                tasForm.ambarKod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.AMBARKOD, "");
                tasForm.islemTipKod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ISLEMTIPKOD, 0);
                tasForm.durum = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.DURUM, 0);
                tasForm.islemTarih = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ISLEMTARIH);
                tasForm.islemYapan = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ISLEMYAPAN, "");
                tasForm.muhasebeAd = VeriErisim.DegerAl(dr, sonSiraNo + 1, "");
                tasForm.harcamaAd = VeriErisim.DegerAl(dr, sonSiraNo + 2, "");
                tasForm.ambarAd = VeriErisim.DegerAl(dr, sonSiraNo + 3, "");

                if (tasForm.islemTipKod > 0)
                {
                    if (tipKodList.ContainsKey(tasForm.islemTipKod))
                        tasForm.islemTipTur = (int)tipKodList[tasForm.islemTipKod];
                    else
                    {
                        tasForm.islemTipTur = (int)TMMServis.UzaySayiDegerGetir("TASISLEMTIPTUR", tasForm.islemTipKod.ToString(), true, "");
                        tipKodList.Add(tasForm.islemTipKod, tasForm.islemTipTur);
                    }
                }

                tasForm.kod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.KOD, string.Empty);
                tasForm.onayDurum = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ONAYDURUM, 0);
                tasForm.aciklama = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ACIKLAMA, "");
                tasForm.gYil = VeriErisim.DegerAl(dr, 33, 0); //Amortisman için dönem - geçici
                tasForm.onayAciklama = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.ONAYACIKLAMA, "");
                tasForm.mutemmim = VeriErisim.DegerAl(dr, (int)TABTasinirIslemForm.ENUMAlanlar.MUTEMMIM, 0);


                bilgi.Ekle(tasForm);
            }

            dr.Close();
            ve.VeriErisimKapat();
            return bilgi;
        }

        public static ObjectArray BAOnayiSorgulama(Kullanici kullanan, BAOnay kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTasinirIslemForm tabTasinirIslemForm = new TABTasinirIslemForm(ve);

            IDataReader dr = tabTasinirIslemForm.BAOnayiSorgulama(kriter);

            if (ve.hata != "")
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr += ve.hata;
                bilgi.sonuc.vtHatasi = true;

                if (dr != null)
                    dr.Close();

                ve.VeriErisimKapat();

                return bilgi;
            }

            while (dr.Read())
            {
                BAOnay onay = new BAOnay();

                onay.yil = VeriErisim.DegerAl(dr, 0, 0);
                onay.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                onay.harcamaKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                onay.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), onay.harcamaKod);
                onay.fisNo = VeriErisim.DegerAl(dr, 3, string.Empty);

                onay.gonderimTarih = VeriErisim.DegerAl(dr, 4);
                onay.gonderenKisi = VeriErisim.DegerAl(dr, 5, string.Empty);

                onay.bOnayTarih = VeriErisim.DegerAl(dr, 6);
                onay.bOnaylayan = VeriErisim.DegerAl(dr, 7, string.Empty);

                onay.aOnayTarih = VeriErisim.DegerAl(dr, 8);
                onay.aOnaylayan = VeriErisim.DegerAl(dr, 9, string.Empty);

                bilgi.Ekle(onay);
            }

            dr.Close();
            ve.VeriErisimKapat();
            return bilgi;
        }

        public static bool OnaylanmamisBelgeVarMi(VeriErisim veriErisim, TasinirIslemForm form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABTasinirIslemForm tabTasinirIslemForm = new TABTasinirIslemForm(ve);
            bool sonuc = tabTasinirIslemForm.OnaylanmamisBelgeVarMi(form);
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static void DevirCikisYapildiMailAt(Kullanici kullanan, TasinirIslemForm tfIslem)
        {
            string eposta = "";

            //Fişin ait olduğu birime yetkili kullanıcıları bul
            TasinirIslemOnayYetki ty = new TasinirIslemOnayYetki();
            ty.muhasebeKod = tfIslem.gMuhasebeKod;
            ty.harcamaBirimKod = tfIslem.gHarcamaKod;
            //if (onayTur.StartsWith("A"))
            //    ty.onayTur = (int)ENUMTasinirIslemFormOnayTur.AONAYI;
            //else if (onayTur.StartsWith("B"))
            //    ty.onayTur = (int)ENUMTasinirIslemFormOnayTur.BONAYI;

            ObjectArray bilgi = TMMTasinirIslemOnayYetki.Listele(kullanan, ty);
            foreach (TNS.TMM.TasinirIslemOnayYetki yetki in bilgi.objeler)
            {
                if (eposta != "") eposta += ";";
                eposta += yetki.eposta;
            }

            string yazilimAd = System.Configuration.ConfigurationManager.AppSettings.Get("YazilimAd");
            string yazilimMailAdresi = System.Configuration.ConfigurationManager.AppSettings.Get("YazilimMailAdresi");

            string icerik = "Sayın ilgili,<br />Devir giriş işlemi yapılması gereken fişiniz bulunmaktadır.<br /><br />Gönderen Belge No: " + tfIslem.fisNo + "<br />Gönderen Belge Tarihi: " + tfIslem.fisTarih + "<br /> Gönderen Harcama Birimi: " + tfIslem.harcamaAd + "<br /> Gönderilen Harcama Birimi: " + tfIslem.gHarcamaAd + "<br />Gönderen İşlem Tipi: " + tfIslem.islemTipAd + "<br/><br/>İyi çalışmalar.<br>" + yazilimAd;

            OrtakFonksiyonlar.MailAtThread(yazilimMailAdresi, eposta, "", "", "Devir Giriş", icerik, true, false, null);
        }

        public static bool OnaylanmamisGeciciIhracKaydiVarMi(VeriErisim veriErisim, string sicilNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABTasinirIslemForm tabTasinirIslemForm = new TABTasinirIslemForm(ve);
            bool sonuc = tabTasinirIslemForm.OnaylanmamisGeciciIhracKaydiVarMi(sicilNo);
            ve.VeriErisimKapat();
            return sonuc;
        }

    }
}