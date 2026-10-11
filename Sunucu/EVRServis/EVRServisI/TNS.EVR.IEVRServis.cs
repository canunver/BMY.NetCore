using System;
using TNS;
using TNS.KYM;
using System.Configuration;
using WebServisler.MobilImzaEvrak;
using EVRServisI;
using System.Collections;
using System.Collections.Generic;
using WebServisler.Evrak.SgbnetWCFServisi;
using System.IO;
using TNS.IKY;

namespace TNS.EVR
{
    /// <summary>
    /// Evrak sınıf için istemci ve sunucuda ortak kullanılacak statik fonksiyonlar tutan sınıf.
    /// </summary>
    [Serializable()]
    public static class Arac
    {
        /// <summary>
        /// Evrak sunucu nesnesi uyandıran yordam.
        /// </summary>
        /// <returns>Evrak sunucu nesnesi döndürülür.</returns>
        public static IEVRServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("EVRServisURL");
            IEVRServis servis = TNS.Api.ServisIstemci.Olustur<IEVRServis>(sUrl);
            return servis;
        }

        public static bool ManasKullaniyor()
        {
            return TNS.DEG.Arac.VarsayilanKurumBul().Replace(".", "") == "1399";
        }

        public static string PinKoduGetir(Kullanici kullanan, string mernis)
        {
            string pinKod = "";

            if (mernis.Length <= 11)
            {
                IIKYServis servis = TNS.IKY.Arac.Tanimla();
                TNS.IKY.PersonelMaasParam maasParam = servis.MaasParamListele(kullanan, mernis, 0);

                pinKod = maasParam.ParamDegerBulStr("pinKod");
            }

            return !string.IsNullOrWhiteSpace(pinKod) ? pinKod : mernis;
        }

        public static string OrganizationInnGetir(string mernis)
        {
            if (mernis.Length <= 11)
                return "02712199510190";
            return "";
        }

        public static void HataStrYaz(string hata)
        {
            try
            {
                string hataYaz = System.Configuration.ConfigurationManager.AppSettings.Get(ENUMWebConfigEvrakAyarlarSunucu.EvrakHataStrYaz.ToString()) + "";
                if (hataYaz != "0")
                    OrtakFonksiyonlar.HataStrYaz(hata);
            }
            catch (Exception ex)
            {
                OrtakFonksiyonlar.HataStrYaz(string.Format("Arac.HataStrYaz metodunda hata oluştu. Mesaj:{0} - StackTrace:{1}", ex.Message, ex.StackTrace));
            }
        }

        public static UserInfo LoginCentraCube(string workflowName, string userID, string password, string token)
        {
            if (workflowName == "")
            {
                string config = System.Configuration.ConfigurationManager.AppSettings.Get(ENUMWebConfigEvrakAyarlarSunucu.EvrakLoginCentraCubeWorkflowName.ToString()) + "";
                workflowName = string.IsNullOrWhiteSpace(config) ? "Login" : config;
            }

            if (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(password))
            {
                string girisBilgi = GirisBilgiOku();
                string[] bilgi = girisBilgi.Split('|');
                userID = bilgi[0];
                password = bilgi[1];
                token = bilgi[2];
            }

            Arac.HataStrYaz("TNS.EVR.IEVRServis.cs - LoginCentraCube - WCFClient Başla - {B4437C70-5EB6-4D0E-B2FA-5652A72D0EB4} - " + string.Format("workflowName:{0} - userID:{1} - password:{2} - token:{3}", workflowName, userID, password, token));
            WCFClient wcf = new WCFClient();

            Arac.HataStrYaz("TNS.EVR.IEVRServis.cs - LoginCentraCube - LoginByCheckingToken - {192F65D8-6481-43D2-9455-5487160773E0} - " + string.Format("workflowName:{0} - userID:{1} - password:{2} - token:{3}", workflowName, userID, password, token));
            UserInfo userInfo = wcf.LoginByCheckingToken(workflowName, userID, password, token);
            //UserInfo userInfo = wcf.Login(workflowName, userID, password);

            if (userInfo != null)
            {
                string bilgi = userID + "|" + password + "|" + userInfo.TokenId;
                GirisBilgiYaz(bilgi);
            }
            else
                Arac.HataStrYaz("TNS.EVR.IEVRServis.cs - LoginCentraCube - LoginByCheckingToken - Sonuç - {C5ABDEF2-E854-4E70-B1DF-0AF5F6B35240} - userInfo degeri null");


            return userInfo;
        }

        private static void GirisBilgiYaz(string bilgi)
        {
            //string dosyaAd = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string dosyaAd = System.AppDomain.CurrentDomain.BaseDirectory;
            dosyaAd = Path.Combine(dosyaAd, "GirisBilgi.dat");

            if (dosyaAd != null && dosyaAd != "")
            {
                System.Threading.Monitor.Enter(dosyaAd);
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(dosyaAd, false);
                    sw.Write(bilgi);
                    sw.Flush();
                    sw.Close();
                    sw = null;
                }
                catch (Exception)
                {
                    if (sw != null)
                        sw.Close();
                }

                System.Threading.Monitor.Exit(dosyaAd);
            }
        }
        private static string GirisBilgiOku()
        {
            string bilgi = "";
            //string dosyaAd = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string dosyaAd = System.AppDomain.CurrentDomain.BaseDirectory;

            dosyaAd = Path.Combine(dosyaAd, "GirisBilgi.dat");

            if (dosyaAd != null && dosyaAd != "")
            {
                System.Threading.Monitor.Enter(dosyaAd);
                StreamReader sw = null;
                try
                {
                    sw = new StreamReader(dosyaAd, false);
                    bilgi = sw.ReadLine();
                    sw.Close();
                    sw = null;
                }
                catch (Exception)
                {
                    if (sw != null)
                        sw.Close();
                }

                System.Threading.Monitor.Exit(dosyaAd);
            }
            return bilgi;
        }

        public static Dictionary<String, Object> SurecParametrelerineDegerAta(WCFClient wcf, string surecAd, Kullanici kullanan, string evrEvrakNo, string prEvrakNo, bool karar, string token)
        {
            Dictionary<String, Object> girdi = new Dictionary<string, object>();

            var parametreler = wcf.GetInArgumentsByName(surecAd, token);

            foreach (var p in parametreler)
            {
                if (p.Key.ToUpper().Replace("İ", "I") == "_USERCODE")
                    girdi.Add(p.Key, kullanan.kullaniciKodu);
                else if (p.Key.ToUpper() == "_PASSWORD")
                    girdi.Add(p.Key, kullanan.parola);
                else if (p.Key.ToUpper() == "_TCKN")
                    girdi.Add(p.Key, kullanan.mernis);
                else if (p.Key.ToUpper() == "_PREVRAKNO")
                    girdi.Add(p.Key, prEvrakNo);
                else if (p.Key.ToUpper() == "_EVREVRAKNO")
                    girdi.Add(p.Key, evrEvrakNo);
                else if (p.Key.ToUpper() == "_XMLEVRAK")
                    girdi.Add(p.Key, "");
                else if (p.Key.ToUpper() == "REASON")
                    girdi.Add(p.Key, karar ? "true" : "false");
                else if (p.Key.ToUpper() == "RESULT")
                    girdi.Add(p.Key, karar);
            }


            //foreach (var item in girdi)
            //{
            //    Arac.HataStrYaz(item.Key.ToString() + ":" + OrtakFonksiyonlar.ConvertToStr(item.Value));
            //}


            return girdi;
        }
    }

    /// <summary>
    /// Yetki kontrollerini düzenleyen fonksiyonları içeren sınıf
    /// </summary>
    [Serializable()]
    public static class Yetki
    {
        /// <summary>
        /// Evrak Servis Tanımı
        /// </summary>
        public static IEVRServis servis = Arac.Tanimla();

        public static Sonuc YetkiKontrolu(Kullanici kullanan, ENUMYetkiler yetki, ENUMYetkiIslem yetkiIslem)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            //if (kullanan.yetkiSablonlari != null)
            //    foreach (YetkiSablon y in kullanan.yetkiSablonlari)
            //    {
            //        if (y.marinaKod == kullanan.calistigiMarina.kod)
            //        {
            //            foreach (YetkiSablonDetay yd in y.detay)
            //            {
            //                //Eğer sistem yöneticisi ise her işlemi yapabilir
            //                if (yd.yetkiKod == (int)ENUMSayfalar.SISTEMYONETICISI && yd.yetkiDeger == (int)ENUMYetkiSekli.OKUMA)
            //                {
            //                    sonuc.islemSonuc = true;
            //                    return sonuc;
            //                }

            //                if ((int)sayfa == yd.yetkiKod)
            //                {
            //                    if ((yd.yetkiDeger & (int)yetkiSekli) == (int)yetkiSekli)
            //                        sonuc.islemSonuc = true;
            //                }
            //            }
            //        }
            //    }

            if (!sonuc.islemSonuc)
                sonuc.hataStr = "Yetki Bulunamadı";

            return sonuc;
        }

        public static bool EvrakUstDereceYetkisiVarmi(Kullanici kullanan, string islemYapilanBirimKod)
        {
            if (TNS.EVR.Yetki.KullaniciEvrakYetkiSablonu(kullanan, islemYapilanBirimKod) == (int)ENUMEvrakYetkiSablonu.EVRAKYETKILISI ||
                TNS.EVR.Yetki.KullaniciEvrakYetkiSablonu(kullanan, islemYapilanBirimKod) == (int)ENUMEvrakYetkiSablonu.EVRAKMEMURU ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.EVRAKBELGEYONETICISI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.EVRAKKURUMBELGEYONETICISI))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Kullanıcının yetkili olduğu evrak birimlerini döndüren yordam. 
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgilerini içeren nesne</param>
        /// <returns>Yetkili evrak birimleri string nesnesi tipinde döndürülür.</returns>
        public static string KullaniciEvrakYetkiliBirimleri(Kullanici kullanan, ENUMOkumaYazma tur)
        {
            string degerler = "";
            foreach (System.Collections.DictionaryEntry de in kullanan.yetkiTipiDegerleri)
            {
                string ytAd = (string)de.Key;
                for (int i = 1; i < 5; i++)
                {
                    if (tur == ENUMOkumaYazma.YAZMA && (i == 3 || i == 4)) //Yazmak amaçlı kontrol ediliyorsa, Yetkili Olduğu Evrak Birimleri (Arşivci) ve (Misafir) yetkileri kontrol edilmesin. Bir kişi hem Misafir hemde başka birimde evrak kullanıcısı olabilir
                        continue;

                    if (ytAd.Equals(EvrakI.ResourceManager.GetString("EVRI001_" + i)))
                    {
                        string d = TNS.HashTableArrayList.ArrayListToStr(de.Value);
                        if (d.Trim() != "" && OrtakFonksiyonlar.ConvertToInt(d, -1) == -1) // OrtakFonksiyonlar.ConvertToInt(d, -1) == -1 sonradan eklendi. Melih 14.08.2017 Evrak birimi bul 11 gibi bir değer buluyor
                        {
                            if (degerler != "" && !degerler.EndsWith(";")) degerler += ";";
                            degerler += d;
                        }
                    }
                }
            }
            return degerler;
        }

        /// <summary>
        ///  Kullanıcının evrak yetki şablonlarını döndüren yordam. 
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgilerini içeren nesne</param>
        /// <returns>Evrak yetki şablonu int nesnesi tipinde döndürülür.</returns>
        public static int KullaniciEvrakYetkiSablonu(Kullanici kullanan, string islemYapilanBirimKod)
        {
            bool birimeGoreYetki = !string.IsNullOrWhiteSpace(islemYapilanBirimKod) && servis.YapilabilirMi(ENUMWebConfigEvrakAyarlarSunucu.EvrakYetkiKontroluBirimeGoreYapilsin);

            string yd = "";
            foreach (System.Collections.DictionaryEntry de in kullanan.yetkiTipiDegerleri)
            {
                string ytAd = (string)de.Key;
                if (ytAd.Equals(EvrakI.EVRI002))
                {
                    string d = TNS.HashTableArrayList.ArrayListToStr(de.Value);
                    if (d.Trim() != "")
                        yd = d;


                    if (birimeGoreYetki)
                    {
                        Hashtable y1 = (Hashtable)de.Value;

                        foreach (System.Collections.DictionaryEntry de2 in kullanan.yetkiTipiDegerleri)
                        {
                            if (((string)de2.Key).Equals(EvrakI.EVRI001_1))
                            {
                                Hashtable y2 = (Hashtable)de2.Value;
                                foreach (DictionaryEntry item in y2)
                                    if (item.Value + "" == islemYapilanBirimKod)
                                        return OrtakFonksiyonlar.ConvertToInt(y1[item.Key], 0);
                            }
                        }
                    }


                    if (yd.IndexOf((int)ENUMEvrakYetkiSablonu.EVRAKYETKILISI + "") > -1)
                        return (int)ENUMEvrakYetkiSablonu.EVRAKYETKILISI;
                    else if (yd.IndexOf((int)ENUMEvrakYetkiSablonu.EVRAKMEMURU + "") > -1)
                        return (int)ENUMEvrakYetkiSablonu.EVRAKMEMURU;
                    else if (yd.IndexOf((int)ENUMEvrakYetkiSablonu.EVRAKKULLANICISI + "") > -1)
                        return (int)ENUMEvrakYetkiSablonu.EVRAKKULLANICISI;

                    break;
                }
            }

            return OrtakFonksiyonlar.ConvertToInt(yd.TrimEnd(';'), 0);
        }

        /// <summary>
        ///  Kullanıcının evrak onay yetki şablonlarını döndüren yordam. 
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgilerini içeren nesne</param>
        /// <returns>Evrak onay yetki şablonu int nesnesi tipinde döndürülür.</returns>
        public static int KullaniciEvrakOnayYetkiSablonu(Kullanici kullanan)
        {
            string degerler = "";
            foreach (System.Collections.DictionaryEntry de in kullanan.yetkiTipiDegerleri)
            {
                string ytAd = (string)de.Key;
                if (ytAd.Equals(EvrakI.EVRI003))
                {
                    string d = TNS.HashTableArrayList.ArrayListToStr(de.Value);
                    if (d.Trim() != "")
                        degerler = d;
                }
            }
            degerler = degerler.Replace(";", "");
            return OrtakFonksiyonlar.ConvertToInt(degerler, 0);
        }

        /// <summary>
        /// Kullanıcının ilgili birime gelen evrak kayıt yetkisinin olup olmadığını kontrol eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="birimKod1">Evrak'ın bulunduğu yerin birim kodunu içeren nesne.</param>
        /// <param name="birimKod2">Evrak birim kodunu içeren nesne</param>
        /// <returns>Kullanıcının gelen evrak kaydetme yetkisi varsa doğru, yoksa yanlış döndürülür.</returns>
        public static bool GelenEvrakKayitEdebilirMi(Kullanici kullanan, string birimKod1, string birimKod2)
        {
            //Eğer birim başka birimler adına evrak kayıt ediyorsa (Kurum Gelen Evrak Birimi) Bu birimlerin listelenmesi gerekli

            if (!TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, birimKod1))
                return false;

            if (!KullaniciBirimeYetkiliMi(kullanan, birimKod1, ENUMOkumaYazma.YAZMA))
                return false;

            //Aşağı kısım tüm evrak birimlerinin gelen evrak kayıdını yapabilmesi için kapatıldı. 21.06.2011

            ////bu birim kayıt yapabilir mi
            //Birim birimKoduKriter = new Birim();
            //birimKoduKriter.kod = birimKod1;
            //Birim birimKodu = servis.BirimOku(birimKoduKriter);

            //if (birimKodu != null)
            //{
            //    if (birimKodu.genelEvrakMi != (int)ENUMGenelEvrakBirimi.EVET)
            //        return false;
            //}
            //else
            //    return false;

            //Birim yetkiKriter = new Birim();
            //yetkiKriter.ustBirim = birimKod1;

            //ObjectArray secilmis = servis.BirimListele(yetkiKriter);
            //if (secilmis.objeler.Count > 0)
            //{
            //    foreach (Birim y in secilmis.objeler)
            //    {
            //        if (y.kod == birimKod2)
            //            return true;
            //    }
            //}
            //else
            //{
            //    if (birimKod1 == birimKod2)
            //        return true;
            //    else
            //        return false;
            //}

            return true;
        }

        /// <summary>
        /// Kullanıcının ilgili birime giden evrak kayıt yetkisinin olup olmadığını kontrol eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="birimKod">Giden evrak kayıt kontrolü yapılacak evrak birim kodunu içeren nesne</param>
        /// <returns>Kullanıcının giden evrak kaydetme yetkisi varsa doğru, yoksa yanlış döndürülür.</returns>
        public static bool GidenEvrakKayitEdebilirMi(Kullanici kullanan, string birimKod)
        {
            return KullaniciBirimeYetkiliMi(kullanan, birimKod, ENUMOkumaYazma.YAZMA);
        }

        /// <summary>
        /// Kullanıcının ilgili birime form/dokuman kayıt yetkisinin olup olmadığını kontrol eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="birimKod">kayıt kontrolü yapılacak evrak birim kodunu içeren nesne</param>
        /// <returns>Kullanıcının kaydetme yetkisi varsa doğru, yoksa yanlış döndürülür.</returns>
        public static bool FormKayitEdebilirMi(Kullanici kullanan, string birimKod)
        {
            return KullaniciBirimeYetkiliMi(kullanan, birimKod, ENUMOkumaYazma.YAZMA);
        }

        /// <summary>
        /// Kullanıcının birime erişim yetkisinin olup olmadığını kontrol eden yordam.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgilerini içeren nesne</param>
        /// <param name="birimKod">Yetki kontrolü yapılacak evrak birim kodunu içeren nesne</param>
        /// <returns>Kullanıcının birime yetkisi varsa doğru, yoksa yanlış döndürülür.</returns>
        public static bool KullaniciBirimeYetkiliMi(Kullanici kullanan, string birimKod, ENUMOkumaYazma tur)
        {
            string[] yetkiliBirimler = TNS.EVR.Yetki.KullaniciEvrakYetkiliBirimleri(kullanan, tur).Split(';');
            bool bulundu = false;
            for (int i = 0; i < yetkiliBirimler.Length; i++)
            {
                if (yetkiliBirimler[i] == birimKod)
                    bulundu = true;
            }

            if (!bulundu)
            {
                if (TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, birimKod))
                {
                    for (int i = 0; i < yetkiliBirimler.Length; i++)
                    {
                        if (birimKod.StartsWith(yetkiliBirimler[i]))
                            bulundu = true;
                    }

                }
            }
            return bulundu;
        }

        /// <summary>
        /// Kullanıcının evrak kayıt yetkisinin olup olmadığını kontrol eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Evrak bilgilerini içeren nesne</param>
        /// <returns>Kullanıcının yazma yetkisi varsa doğru, yoksa yanlış döndürülür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, EvrakSinif form)
        {
            bool yetkili = false;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.EVRAKKURUMBELGEYONETICISI))
                return true;

            if (!TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, form.bulunduguYer))
                form.hareket.kisi1 = kullanan.mernis;

            if (form.evrEvrakNo == "" && form.evrakTipi == (int)ENUMEvrakTipi.GELEN)
                yetkili = Yetki.GelenEvrakKayitEdebilirMi(kullanan, form.bulunduguYer, form.evrakBirimKodu);
            else if (form.prEvrakNo != "" && form.evrakTipi == (int)ENUMEvrakTipi.GELEN)
                yetkili = servis.EvrakKendiBirimindeMi(kullanan, form, false);
            else if (form.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
            {
                yetkili = Yetki.GidenEvrakKayitEdebilirMi(kullanan, form.bulunduguYer);
                if (yetkili && form.evrEvrakNo != "")
                    yetkili = servis.EvrakKendiBirimindeMi(kullanan, form, false);
            }
            else if (form.evrakTipi == (int)ENUMEvrakTipi.FORMLAR)
            {
                yetkili = Yetki.FormKayitEdebilirMi(kullanan, form.bulunduguYer);
                if (yetkili && form.evrEvrakNo != "")
                    yetkili = servis.EvrakKendiBirimindeMi(kullanan, form, false);
            }

            return yetkili;
        }

        public static bool EvrakIptalYetkisiVarMi(Kullanici kullanan, string islemYapilanBirimKod)
        {
            if (TNS.EVR.Yetki.KullaniciEvrakYetkiSablonu(kullanan, islemYapilanBirimKod) == (int)ENUMEvrakYetkiSablonu.EVRAKYETKILISI ||
               kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.EVRAKBELGEYONETICISI) ||
               kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.EVRAKKURUMBELGEYONETICISI))
                return true;
            else
                return false;
        }

        public static string KullaniciEvrakYetkiliBirimleri(Kullanici kullanan, params int[] kullaniciTipleri)
        {
            string degerler = "";

            foreach (int kt in kullaniciTipleri)
            {
                foreach (System.Collections.DictionaryEntry de in kullanan.yetkiTipiDegerleri)
                {
                    string ytAd = (string)de.Key;

                    if (kt == (int)ENUMKullaniciTipi.EVRAKBAKIM && ytAd.Equals(EvrakI.ResourceManager.GetString("EVRI001_6")) ||
                        kt == (int)ENUMKullaniciTipi.EVRAKKEPAL && ytAd.Equals(EvrakI.ResourceManager.GetString("EVRI001_7")) ||
                        kt == (int)ENUMKullaniciTipi.EVRAKKEPGONDER && ytAd.Equals(EvrakI.ResourceManager.GetString("EVRI001_8")) ||
                        kt == (int)ENUMKullaniciTipi.EVRAKBELGEYONETICISI && ytAd.Equals(EvrakI.ResourceManager.GetString("EVRI001_2")))
                    {
                        string d = TNS.HashTableArrayList.ArrayListToStr(de.Value);
                        if (d.Trim() != "")
                        {
                            if (degerler != "" && !degerler.EndsWith(";")) degerler += ";";
                            degerler += d;
                        }
                    }
                }
            }

            return degerler;
        }

        public static Sonuc YetkiVarMi(Kullanici kullanan, ENUMYetkiler yetki, ENUMYetkiIslem yetkiIslem)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string islemAd = "";
            for (int i = kullanan.yetkiTipiDegerleri.MinIndex(); i <= kullanan.yetkiTipiDegerleri.MaxIndex(); i++)
            {
                string yetkiAd = "";
                switch (yetkiIslem)
                {
                    case ENUMYetkiIslem.OKUMA: yetkiAd = "Okuma İşlemleri"; islemAd = "Okuma"; break;
                    case ENUMYetkiIslem.YAZMA: yetkiAd = "Yazma İşlemleri"; islemAd = "Yazma"; break;
                    case ENUMYetkiIslem.SILME: yetkiAd = "Silme İşlemleri"; islemAd = "Silme"; break;
                }

                string yetkiler = kullanan.yetkiTipiDegerleri[yetkiAd, i];

                //Hiç yetki verilmediyse 
                if (string.IsNullOrWhiteSpace(yetkiler))
                {
                    sonuc.islemSonuc = true;
                    return sonuc;
                }

                string[] yetkiKodlar = yetkiler.Split(';');
                foreach (var kod in yetkiKodlar)
                {
                    if (!string.IsNullOrEmpty(kod) && int.Parse(kod) == (int)yetki)
                    {
                        sonuc.islemSonuc = true;
                        break;
                    }
                }
            }

            if (!sonuc.islemSonuc)
                sonuc.DegerAta(islemAd + " işlemi yapma yetkiniz bulunmamaktadır!");

            return sonuc;
        }


        //public static string AktifBirimBul(Kullanici kullanan)
        //{
        //    IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();
        //    var kDeg = servisKYM.DegiskenleriListele(kullanan.kullaniciKodu, "AKTIFGELENEVRAKBIRIM");

        //    return (kDeg != null && kDeg.Count > 0) ? kDeg[0].ToString() : "";
        //}

    }

    /// <summary>
    /// Sunucu tarafından sunulan fonksiyonları belirten arayüz
    /// </summary>
    public interface IEVRServis
    {
        bool YapilabilirMi(ENUMWebConfigEvrakAyarlarSunucu islem);
        bool YapilabilirMi(ENUMWebConfigEvrakAyarlarSunucu islem, string birimKod);

        /// <summary>
        /// Mobil imza için, sertfikanın gerçerliliğini kontrol eden yordam.
        /// </summary>
        /// <param name="data">Kontrol edilecek veri dizisi nesnesi</param>
        /// <param name="tarih">ontrol edilecek tarih nesnesi</param>
        /// <returns>İşlem sonucu metin olarak döndürülür.</returns>
        string SertifikaGecerliMi(byte[] data, TNSDateTime tarih);

        /// <summary>
        /// Mobil imza için, gönderilen veri dizisindeki imzaları döndüren yordam.
        /// </summary>
        /// <param name="data">Kontrol edilecek veri dizisi nesnesi</param>
        /// <returns>İşlem sonucu imza bilgileri nesne koleksiyonu türünde döndürülür.</returns>
        ObjectArray BelgedekiImzalar(byte[] data);

        /// <summary>
        /// Mobil imza için, belgenin imza durumunu kontrol eden yordam.
        /// </summary>
        /// <param name="data">Kontrol edilecek veri dizisi nesnesi</param>
        /// <returns>İmzalı ise doğru, yoksa yanlış döndürülür.</returns>
        bool ImzaliMi(byte[] data);

        /// <summary>
        /// Mobil imza için, veri dizisini imazaya gönderen yordam.
        /// </summary>
        /// <param name="data">Veri dizisi içeren nesne</param>
        /// <param name="gsmOperator">GSM operatörlerini içeren nesne</param>
        /// <param name="telNo">Telefon numarasını içeren nesne</param>
        /// <returns>İşlem sonucu ImzaSonuc nesnesi türünde döndürülür.</returns>
        ImzaSonuc ImzayaGonder(byte[] data, ENUMGSMOperator gsmOperator, string telNo);

        /// <summary>
        /// Mobil imza için, imzalama işlemini yapan yordam. 
        /// </summary>
        /// <param name="hr">İmzalanacak bilgiyi içeren nesne</param>
        /// <param name="cepMesaj">İmzalamak için cep telefonuna gelen mesajı içeren nesne</param>
        /// <returns>İşlem sonucu ImzaSonuc nesnesi türünde döndürülür.</returns>
        ImzaSonuc MobilImzala(ImzaSonuc hr, string cepMesaj);

        /// <summary>
        /// Parametre olarak verilen evrak birimlerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">>Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak birimlerini içeren nesne koleksiyonu</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc BirimKaydet(Kullanici kullanan, ObjectArray liste);

        /// <summary>
        /// Parametre olarak verilen evrak birimlerini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Evrak birimlerini içeren nesne koleksiyonu</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc BirimSil(Kullanici kullanan, ObjectArray liste);

        /// <summary>
        /// Parametre olarak verilen Birim nesnesine göre evrak birimini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Birim bilgisini içeren nesne</param>
        /// <returns>Evrak birimi Birim nesnesi olarak döndürülür.</returns>
        Birim BirimOku(Birim bilgi);

        /// <summary>
        /// Parametre olarak verilen Birim nesnesine göre evrak birimlerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Birim bilgisini içeren nesne</param>
        /// <returns>Birim nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray BirimListele(Birim bilgi);

        /// <summary>
        /// Parametre olarak verilen Birim nesnesine göre Kurumdaki Ana evrak birimlerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Birim bilgisini içeren nesne</param>
        /// <returns>Birim nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray AnaBirimleriListele(Birim bilgi);

        /// <summary>
        /// Evrak birimlerinin seviye değerlerini düzenleyen yordam.
        /// </summary>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc BirimDetaySayilariDuzenle();

        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">DosyaPlani nesne koleksiyonunu içeren nesne</param>
        /// <param name="aktarim">DosyaPlani nesne koleksiyonunun aktarım ile gelip gelmediğini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc DosyaPlaniKaydet(Kullanici kullanan, ObjectArray liste, bool aktarim, string seciliKod);

        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesne koleksiyonunu içeren dosya planı bilgilerini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">DosyaPlani nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc DosyaPlaniSil(Kullanici kullanan, ObjectArray liste);

        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesnesine göre, DosyaPlani nesne koleksiyonunu veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">DosyaPlani bilgisini içeren nesne</param>
        /// <returns>DosyaPlani nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray DosyaPlaniListele(DosyaPlani bilgi);

        ObjectArray DosyaPlaniTarihceListele(DosyaPlaniTarihce bilgi);

        ObjectArray DosyaPlaniEvrakListele(EvrakSinif bilgi);

        //ObjectArray DosyaPlaniEvrakSayisi(DosyaPlaniTarihce bilgi);

        /// <summary>
        /// Dosya planlarının seviye değerlerini düzenleyen yordam.
        /// </summary>        
        void DosyaPlaniDetaySayilariDuzenle();

        /// <summary>
        /// Parametre olarak verilen DosyaPlaniBirim nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">DosyaPlaniBirim nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc DosyaPlaniBirimKaydet(Kullanici kullanan, ObjectArray liste);

        /// <summary>
        /// Parametre olarak verilen DosyaPlaniBirim nesnesine göre, DosyaPlaniBirim nesne koleksiyonunu veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">DosyaPlaniBirim bilgisini içeren nesne</param>
        /// <returns>DosyaPlaniBirim nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray DosyaPlaniBirimListele(Kullanici kullanan, DosyaPlaniBirim bilgi);

        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">DosyaPlani nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc DosyaPlaniFavoriKaydet(Kullanici kullanan, ObjectArray liste);

        /// <summary>
        /// Dosya planı favori bilgilerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ad">Dosya plan kodunun ad bilgisini içeren nesne</param>
        /// <param name="kod">Dosya plan kodunun kod bilgisini içeren nesne</param>
        /// <returns>DosyaPlani nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray DosyaPlaniFavoriListele(Kullanici kullanan, string ad, string kod);

        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesne koleksiyonunu içeren dosya planı favori bilgilerini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">DosyaPlani nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>        
        Sonuc DosyaPlaniFavoriSil(Kullanici kullanan, ObjectArray liste);

        Sonuc DosyaPlaniBolumTanimKaydet(Kullanici kullanan, DosyaPlaniBolumTanim bilgi);

        ObjectArray DosyaPlaniBolumTanimListele(DosyaPlaniBolumTanim bilgi);

        ObjectArray DosyaPlaniBolumListele(DosyaPlaniBolum bilgi);

        Sonuc DosyaPlaniBolumKaydet(Kullanici kullanan, DosyaPlaniBolum bilgi);
        Sonuc DosyaPlaniKlasorBolumOlustur(Kullanici kullanan);
        ObjectArray DosyaPlaniKlasorBolumListele(DosyaPlaniBolum bilgi);

        Sonuc DosyaPlaniEvrakKaydet(Kullanici kullanan, string prEvrakNo, ArrayList liste, bool eskiKodlariSil, string gerekce);

        Sonuc EvrakGizlikSureUzatimiKaydet(Kullanici kullanan, string[] evrEvrakNolar, TNSDateTime sureSonuTarih);

        Sonuc EvrakGizlikDerecesiKaydet(Kullanici kullanan, string[] evrEvrakNolar, int gizlilikTip);

        //Sonuc GizlilikKaydet(Kullanici kullanan, ObjectArray liste);
        //Sonuc GizlilikSil(Kullanici kullanan, ObjectArray liste);
        //ObjectArray GizlilikListele(Tanimlama bilgi);

        //Sonuc YaziDurumuKaydet(Kullanici kullanan, ObjectArray liste);
        //Sonuc YaziDurumuSil(Kullanici kullanan, ObjectArray liste);
        //ObjectArray YaziDurumuListele(Tanimlama bilgi);

        /// <summary>
        /// OzelAlan bilgisini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">OzelAlan bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc OzelAlanKaydet(Kullanici kullanan, OzelAlan bilgi);

        /// <summary>
        /// OzelAlan bilgilerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgiler">OzelAlan bilgisini içeren nesne</param>
        /// <returns>OzelAlan nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray OzelAlanListele(Kullanici kullanan, OzelAlan bilgiler);

        /// <summary>
        /// OzelAlan bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">OzelAlan bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc OzelAlanSil(Kullanici kullanan, OzelAlan bilgi);

        /// <summary>
        /// Parametre olarak verilen EvrakOzelAlan nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">EvrakOzelAlan nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc EvrakOzelAlanKaydet(Kullanici kullanan, ArrayList bilgi);

        /// <summary>
        /// EvrakOzelAlan bilgilerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <param name="birimKod">Birim kod bilgisini içeren nesne </param>
        /// <param name="liste">EvrakOzelAlan nesne koleksiyonunu içeren nesne</param>
        /// <returns>EvrakOzelAlan nesne koleksiyonunu olarak döndürülür.</returns>
        ArrayList EvrakOzelAlanListele(string prEvrakNo, string birimKod, ArrayList liste);

        //Sonuc GelisSekliKaydet(Kullanici kullanan, ObjectArray liste);
        //Sonuc GelisSekliSil(Kullanici kullanan, ObjectArray liste);
        //ObjectArray GelisSekliListele(Tanimlama bilgi);

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesnesine göre, Evrak bilgilerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Evrak bilgilerini içeren nesne </param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc EvrakKaydet(Kullanici kullanan, EvrakSinif form);

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesnesine göre, Evrak bilgilerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Evrak bilgilerini içeren nesne</param>
        /// <returns>EvrakSinif nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray EvrakListele(Kullanici kullanan, EvrakSinif bilgi, int start, int limit, bool sadeceSatirSayisi = false);
        string[] EvrakListelePrEvrakNo(EvrakSinif form);

        /// <summary>
        /// Paramete olarak verilen evrEvrakNo nesnesine göre, evrak'ın tarihçesini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="evrEvrakNo">evrEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakTarihce nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray EvrakTarihceListele(string evrEvrakNo);

        /// <summary>
        /// Paramete olarak verilen EvrakHareket nesnesine göre, evrak'ın hareket bilgisini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">EvrakHareket bilgisini içeren nesne</param>
        /// <returns>EvrakHareket nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray EvrakHareketListele(EvrakHareket form);

        /// <summary>
        /// Parametre olarak verilen EvrakHareket nesnesine göre, Evrak hareket bilgilerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EvrakHareket bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc EvrakHareketKaydet(Kullanici kullanan, EvrakHareket form);

        /// <summary>
        /// Evrak'ın son hareket durumunu okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakHareket nesnesi olarak döndürülür.</returns>
        EvrakHareket EvrakSonHaraketGetir(string prEvrakNo);

        /// <summary>
        /// Evrak'ın havale sırasında kaydedilen notlarını okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakHareket nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray EvrakHavaleNotuListele(string prEvrakNo);

        /// <summary>
        /// Evrak'ın hareket bilgisine okundu bilgisini kaydeden yordam.
        /// </summary>
        /// <param name="form">EvrakHareket bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc OkunduOlarakIsaretle(EvrakHareket form);

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesnesine göre, evrak sayısını hesaplayan yordam.
        /// </summary>
        /// <param name="form">EvrakSinif bilgisini içeren nesne</param>
        /// <returns>Evrak sayısı double nesene türünde döndürülür.</returns>
        double[] EvrakSayisiVer(Kullanici kullanan, EvrakSinif[] form);

        //EvrakBirimYetkiSinif EvrakBirimYetkiOku(Kullanici kullanan, EvrakBirimYetkiSinif form);
        //Sonuc EvrakBirimYetkiKaydet(Kullanici kullanan, EvrakBirimYetkiSinif form);

        /// <summary>
        /// Paramete olarak verilen Antet nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Antet nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc AntetKaydet(Kullanici kullanan, ObjectArray liste);

        /// <summary>
        /// Parametre olarak verilen Antet nesne koleksiyonunu içeren Antet bilgilerini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Antet nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc AntetSil(Kullanici kullanan, ObjectArray liste);

        /// <summary>
        /// Paramete olarak verilen Antet nesnesine göre, Antet bilgisini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Antet bilgisini içeren nesne</param>
        /// <returns>Antet nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray AntetListele(Kullanici kullanan, Antet bilgi);

        /// <summary>
        /// Paramete olarak verilen Altbilgi nesne koleksiyonunu, kullanıcının evrak altbilgileri olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Altbilgi nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc AltbilgiKaydet(Kullanici kullanan, ObjectArray liste);

        /// <summary>
        /// Parametre olarak verilen Altbilgi nesne koleksiyonunu içeren Altbilgileri veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Altbilgi nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc AltbilgiSil(Kullanici kullanan, ObjectArray liste);

        /// <summary>
        /// Paramete olarak verilen Altbilgi nesnesine göre, kullanıcının evrak altbilgilerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Altbilgi nesne koleksiyonunu içeren nesne</param>
        /// <returns>Altbilgi nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray AltbilgiListele(Kullanici kullanan, Altbilgi bilgi);


        /// <summary>
        /// Parametere olarak verilen bilgilere göre, evrak yer bilgisini EvrakYer nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="birim">Birim bilgisini içeren nesne</param>
        /// <param name="yer">Evrak yer biglisini içeren nesne</param>
        /// <param name="tur">Yer türü bilgisini içeren nesne</param>
        /// <param name="kullanimSayisi">Kullanım sayısını içeren nesne</param>
        /// <returns>EvrakYer nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray EvrakYerIsimleriListele(string birim, string yer, int tur, int kullanimSayisi);

        /// <summary>
        /// Parametere olarak verilen bilgilere göre, evrak yer bilgisini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="birim">Birim bilgisini içeren nesne</param>
        /// <param name="yer">Evrak yer biglisini içeren nesne</param>
        /// <param name="tur">Yer türü bilgisini içeren nesne</param>
        /// <param name="yerBirimKodu">Yer birim kodu bilgisini içeren nesne</param>
        /// <param name="kalici">Kalıcı bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc EvrakYerIsimleriKaydet(string birim, string yer, int tur, string yerBirimKodu, int kalici);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, evrak yer bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="birim">Birim bilgisini içeren nesne</param>
        /// <param name="yer">Evrak yer biglisini içeren nesne</param>
        /// <param name="tur">Yer türü bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc EvrakYerIsimleriSil(string birim, string yer, int tur);

        /// <summary>
        /// Parametere olarak verilen bilgilere göre, evrak konu bilgisini EvrakKonu nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="birim">Birim bilgisini içeren nesne</param>
        /// <param name="konu">Evrak konu biglisini içeren nesne</param>
        /// <param name="kullanimSayisi">Kullanım sayısını içeren nesne</param>
        /// <returns>EvrakKonu nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray EvrakKonuListele(string birim, string konu, int kullanimSayisi);

        /// <summary>
        /// Parametere olarak verilen bilgilere göre, evrak konu bilgisini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="birim">Birim bilgisini içeren nesne</param>
        /// <param name="konu">Evrak konu biglisini içeren nesne</param>
        /// <param name="kalici">Kalıcı bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc EvrakKonuKaydet(string birim, string konu, int kalici);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, evrak konu bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="birim">Birim bilgisini içeren nesne</param>
        /// <param name="konu">Evrak konu biglisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc EvrakKonuSil(string birim, string konu);

        ObjectArray MetaGrupListele(string birim, int kod, string adi);
        Sonuc MetaGrupKaydet(string birim, int kod, string adi);
        Sonuc MetaGrupSil(string birim, int kod);

        /// <summary>
        /// Parametere olarak verilen bilgilere göre, paraf bilgisini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Evrak paraf biglisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc ParafKaydet(Kullanici kullanan, Paraf bilgi);

        /// <summary>
        /// Parametere olarak verilen bilgilere göre, evrak paraf bilgisini Paraf nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kod">Paraf kod bilgisini içeren nesne</param>
        /// <param name="kullaniciKod">Kullanıcı kod bilgisini içeren nesne</param>
        /// <param name="detaylarYuklensin">Paraf detaylarının listelenme durumunu içeren nesne</param>
        /// <returns>Paraf nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray ParafListele(Kullanici kullanan, int kod, string kullaniciKod, bool detaylarYuklensin);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, evrak paraf bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="parafKod">Paraf kod bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc ParafSil(Kullanici kullanan, int parafKod);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, evrak hareket işlemerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="harTur">Evrak hareket durumunu içeren nesne</param>
        /// <param name="formlar">EvrakSinif nesne koleksiyonu tipinde evrak bilgilerini içeren nesne</param>
        /// <param name="yetkiKontrolEt">if set to <c>true</c> [yetki kontrol et].</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        Sonuc EvrakIslem(Kullanici kullanan, ENUMEvrakHareketDurum harTur, ObjectArray formlar, bool yetkiKontrolEt = true);

        /// <summary>
        /// Evrak'ın kullanıcının kendi biriminde olup olmadığını kontrol eden yordam.
        /// </summary>
        /// <param name="form">Evrak bilgilerini içeren nesne</param>
        /// <param name="teslimAlmaIslemi">Evrak teslim alma bilgisini içeren nesne.</param>
        /// <returns>Evrak kullanıcının kendi biriminde ise doğru, yoksa yanlış döndürülür.<</returns>
        bool EvrakKendiBirimindeMi(Kullanici kullanan, EvrakSinif form, bool teslimAlmaIslemi);

        /// <summary>
        /// Parametre olarak verilen kullanıcının evrak üzerinde işlem yapma yetkisini kontrol eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Evrak bilgilerini içeren nesne</param>
        /// <param name="islem">Evrak hareket durumunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc IslemYapmaYetkisiVarMi(Kullanici kullanan, EvrakSinif form, ENUMEvrakHareketDurum islem);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, evrak dağıtım bilgisini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Dağıtım bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc DagitimKaydet(Kullanici kullanan, Dagitim bilgi);

        /// <summary>
        /// Parametere olarak verilen bilgilere göre, dağıtım bilgisini Dagitim nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kodList">Dağıtım kod listesini içeren nesne</param>
        /// <param name="evrakBrimKod">Evrak birim kodunu içeren nesne</param>
        /// <param name="alanlarYuklensin">Dağıtım alanlarının listelenme durumunu içeren nesne</param>
        /// <returns>Dagitim nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray DagitimListele(Kullanici kullanan, string kodList, string evrakBrimKod, bool alanlarYuklensin);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, dağıtım bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="dagitimKod">Dağıtım kod listesini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc DagitimSil(Kullanici kullanan, int dagitimKod);

        /// <summary>
        /// Parametre olarak verilen prEvrakNo bilgisine göre, evrak'ın dağıtım bilgisini EvrakDagitim nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakDagitim nesne koleksiyonunu olarak döndürülür.</returns>
        ArrayList EvrakDagitimListele(string prEvrakNo);

        //Sonuc GidenTurKaydet(Kullanici kullanan, ObjectArray liste);
        //Sonuc GidenTurSil(Kullanici kullanan, ObjectArray liste);
        //ObjectArray GidenTurListele(Kullanici kullanan, Tanimlama bilgi);

        /// <summary>
        /// Parametre olarak verilen evrak birim kodunun evrak kök kodunu getiren yordam.
        /// </summary>
        /// <param name="evrakBirim">Evrek birim kodunu içeren nesne</param>
        /// <returns>Evrak kök kodu string nesesi olarak döndürülür.</returns>
        string EvrakKokBirimGetir(string evrakBirim);
        Birim EvrakKokBirimGetir2(string evrakBirim);

        /// <summary>
        /// Parametre olarak verilen evrak birim kodunun evrak kurum kök kodunu getiren yordam.
        /// </summary>
        /// <param name="evrakBirim">Evrek birim kodunu içeren nesne</param>
        /// <returns>Evrak kurum kök kodu string nesesi olarak döndürülür.</returns>
        string EvrakKurumKokBirimGetir(string evrakBirim);

        /// <summary>
        /// Parametre olarak verilen prEvrakNo bilgisine göre, evrak dosya kod bilgisini DosyaPlani nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="prEvrakNo">Evrak'ın prEvrakNo bilgisini içeren nesne</param>
        /// <returns>DosyaPlani nesne koleksiyonunu olarak döndürülür.</returns>
        ArrayList EvrakDosyaKodOku(Kullanici kullanan, string prEvrakNo);

        /// <summary>
        /// Paramete olarak verilen DosyaPlani nesne koleksiyonunu, kullanıcının evrak dosya kodu bilgileri olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>        
        /// <param name="form">DosyaPlani nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc EvrakDosyaKodKaydet(Kullanici kullanan, string prEvrakNo, ArrayList form);

        /// <summary>
        /// Evraks the dosya kod toplu degistir.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="eskiKod">The eski kod.</param>
        /// <param name="yeniKod">The yeni kod.</param>
        /// <param name="tarih1">The tarih1.</param>
        /// <param name="tarih2">The tarih2.</param>
        /// <param name="aciklama">The aciklama.</param>
        /// <returns></returns>
        Sonuc EvrakDosyaKodTopluDegistir(Kullanici kullanan, string eskiKod, string yeniKod, TNSDateTime tarih1, TNSDateTime tarih2, string aciklama, string bakimdakiler);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, ilişkili evrakların evrEvrakNo ve prEvrakNo bilgileri metin dizisi olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="iliskiprEvrakNo">İlişki prEvrakNo bilgisini içeren nesne</param>
        /// <param name="iliskiTip">Evrak ilişki tipini içeren nese</param>
        /// <returns>Evrak ilişki bilgileri metin dizisi olarak döndürülür.</returns>
        string[] EvrakIliskidenEvrVePrEvrakNoVer(string iliskiprEvrakNo, int iliskiTip);

        /// <summary>
        /// Paramete olarak verilen prEvrakNo nesnesine göre, evrak'ın evrEvrakNo, evrakTarih ve evrakNo bilgisini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakSinifKucuk nesesi olarak döndürülür.</returns>
        EvrakSinifKucuk EvrakNoVeTarihVer(string prEvrakNo);

        /// <summary>
        /// Paramete olarak verilen prEvrakNo nesnesine göre, evrak'ın ilişkili olduğu diğer evrakları EvrakSinif nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <returns>EvrakSinif nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray IliskiliEvraklariListele(string prEvrakNo);

        /// <summary>
        /// Paramete olarak verilen Sablon nesnesini, kullanıcı bilgileri ile birlikte evrak şablonunu olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Şablon bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc SablonKaydet(Kullanici kullanan, Sablon form);

        /// <summary>
        /// Parametre olarak verilen şablon bilgisine göre, evrak'ın şablon bilgisini Sablon nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Şablon bilgilerini içeren nesne</param>
        /// <returns>Sablon nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray SablonListele(Kullanici kullanan, Sablon form);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, şablon bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="sablonId">Şablon ID bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc SablonSil(Kullanici kullanan, int sablonId);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, şablon bilgisini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="sablonId">Şablon ID bilgisini içeren nesne</param>
        Sablon SablonOku(Kullanici kullanan, Sablon form);

        /// <summary>
        /// Parametre olarak verilen evrak nesne koleksiyonuna göre, giden evrak dağıtım listesini gönderilen evraklar bilgisi olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="EvrakListe">EvrakSinif nesne koleksiyonu tipinde evrak bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc EvrakGonderilenKaydet(Kullanici kullanan, ObjectArray EvrakListe);

        /// <summary>
        /// Parametre olarak verilen EvrakGonderilen nesne koleksiyonuna göre, gönderilen evraklar bilgisi olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="Liste">EvrakGonderilen nesne koleksiyonu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc EvrakGonderilenKaydet(ObjectArray Liste);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, Evrak gönderilen bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="gonderilenBirimKodu">Gönderilen birim kodunu içeren nesne</param>
        /// <param name="alindimi">Alındı bilgisini içeren nesne</param>
        /// <returns>EvrakGonderilen nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray EvrakGonderilenListele(string gonderilenBirimKodu, int alindimi);

        /// <summary>
        /// Paramete olarak verilen Haberci nesnesini, evrak habercisi olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">Haberci bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc HaberciKaydet(Haberci form);

        /// <summary>
        /// Parametre olarak verilen bilgisine göre, evrak haberci listesini Haberci nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="tcKimlikNo">TcKimlikNo bilgisini içeren nesne</param>
        /// <param name="kodList">Haberci kod listesini içeren nesne</param>
        /// <param name="evrakBirimKodu">Evrak birim kodunu içeren nesne</param>
        /// <param name="detaylariGetir">Detay bilgilerinin getirilmesini belirten nesne</param>
        /// <returns>Haberci nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray HaberciListele(string tcKimlikNo, string kodList, string evrakBirimKodu, bool detaylariGetir);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, haberci bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="haberciKod">Haberci kod listesini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc HaberciSil(int haberciKod);

        /// <summary>
        /// Parametre olarak verilen HavaleKural nesnesine göre, havale kuralı olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">HavaleKural bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc HavaleKuralKaydet(HavaleKural form);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, evrak havale kural listesini HavaleKural nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kodList">Havale kural kod listesini içeren nesne</param>
        /// <param name="tcKimlikNo">TcKimlikNo bilgisini içeren nesne</param>
        /// <param name="evrakBirimKodu">Evrak birim kodunu içeren nesne</param>
        /// <returns>HavaleKural nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray HavaleKuralListele(string kodList, string tcKimlikNo, string evrakBirimKodu);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, havale kural bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="havaleKuralKodu">Havale kural kod listesini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc HavaleKuralSil(int havaleKuralKodu);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, havale kural bilgilerine bakarak evrak'ı havale eden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Evrak bilgilerini içeren nesne</param>
        /// <param name="kodList">Havale kural kod listesini içeren nesne</param>
        /// <param name="tcKimlikNo">TcKimlikNo bilgisini içeren nesne</param>
        /// <param name="evrakBirimKodu">Evrak birim kodunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc HavaleKuralIleHavale(Kullanici kullanan, EvrakSinif form, string kodList, string tcKimlikNo, string evrakBirimKodu);

        /// <summary>        
        /// Parametre olarak verilen Tanimlama nesne koleksiyonunu, posta tür bilgisi olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Tanimlama nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc PostaIslemTurKaydet(ObjectArray liste);

        /// <summary>
        /// Parametre olarak verilen Tanimlama bilgisine göre, posta tür listesini Tanimlama nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Tanimlama bilgisini içeren nesne</param></param>
        /// <returns>Tanimlama nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray PostaIslemTurListele(Tanimlama bilgi);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, posta tür bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="liste">Tanimlama nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc PostaIslemTurSil(ObjectArray liste);

        /// <summary>
        /// Parametre olarak verilen PostaIslem bilgisine göre, posta işlemini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">PostaIslem bilgisini içeren nesne</param></param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc PostaIslemFormKaydet(PostaIslem form);

        /// <summary>
        /// Parametre olarak verilen PostaIslem bilgisine göre, posta işlem listesini PostaIslem nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">PostaIslem bilgisini içeren nesne</param></param>
        /// <returns>PostaIslem nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray PostaIslemFormListele(PostaIslem form, bool detay);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, posta işlem bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="liste">PostaIslem nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc PostaIslemFormSil(ObjectArray liste);

        /// <summary>
        /// Parametre olarak verilen PostaIslem nesnesine göre, evrak bilgilerini PostaIslemDetay nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">PostaIslem nesne koleksiyonunu içeren nesne</param>
        /// <returns>PostaIslemDetay nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray PostaIslemFormListeleEvrak(PostaIslem form);

        /// <summary>
        /// Parammetre olarak verilen rapor türüne göre, evrak sayılarını içeren rapor bilgisini nesne dizisi olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">Evrak bilgisini içeren nesne</param>
        /// <param name="raporTur">Rapor türünü içeren nesne</param>
        /// <returns>Nesne dizisi olarak döndürülür.</returns>
        ObjectArray RaporListele(EvrakSinif form, ENUMEvrakRaporTuru raporTur);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, personel bilgilerini personel nesne koleksiyonunu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Personel bilgilerini içeren nesne</param>
        /// <returns>Personel nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray PersonelListele(Personel bilgi);

        /// <summary>
        /// Parametre olarak verilen tanımlama tür bilisine göre, tamımlama bilgilerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Tanımlama nesne koleksiyonunu içeren nesne</param>
        /// <param name="tanimlamFormTur">Tanımlama tür bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc TanimlamaKaydet(Kullanici kullanan, ObjectArray liste, ENUMTamimlamaFormTur tanimlamFormTur);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, tanımlama bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Tanımlama nesne koleksiyonunu içeren nesne</param>
        /// <param name="tanimlamFormTur">Tanımlama tür bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc TanimlamaSil(Kullanici kullanan, ObjectArray liste, ENUMTamimlamaFormTur tanimlamFormTur);

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, tanımlama bilgilerini Tanımlama nesne koleksiyonunu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">>Tanımlama bilgisini içeren nesne</param>
        /// <param name="tanimlamFormTur">Tanımlama tür bilgisini içeren nesne</param>
        /// <returns>Tanımlama nesne koleksiyonunu olarak döndürülür.</returns>
        ObjectArray TanimlamaListele(Kullanici kullanan, Tanimlama bilgi, ENUMTamimlamaFormTur tanimlamFormTur);

        /// <summary>
        /// Parametre olarak verilen kullanıcının yetkili olduğu evrak birim kodunu getiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns>Metin nesnesi olarak döndürülür.</returns>
        string EvrakBirimKodListele(Kullanici kullanan);

        /// <summary>
        /// Parametre olarak verilen kaynak ve hedef evrak bilgilerine göre, evrak'ın dosyasını kopyalayan yordam.
        /// </summary>
        /// <param name="kaynakEvrak">Kopyalanacak evrak bilgilerini içeren nesne</param>
        /// <param name="hedefEvrak">Oluşturulacak evrak bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc EvrakEkDosyaKopyala(Kullanici kullanan, EvrakSinif kaynakEvrak, EvrakSinif hedefEvrak);

        /// <summary>
        /// Parametre olarak verilen evrak bilgilerine göre, evrak'ın dosya yolununu metin nesnesi olrak getiren yordam.
        /// </summary>
        /// <param name="evrakBirimi">Evrak birim kodunu içeren nesne</param>
        /// <param name="evrakOlusturmaTarihi">Evrak oluşturma tarihini içeren nesne</param>
        /// <param name="evrEvrakNo">Evrak evrEvrakNo bilgisini içeren nesne</param>
        /// <returns></returns>
        string EvrakEkDosyaYolu(string evrakBirimi, TNSDateTime evrakOlusturmaTarihi, string evrEvrakNo);

        /// <summary>
        /// Parametre olarak verilen evrak EvrakIliski nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>        
        /// <param name="evrakIliskiArr">Evrak ilişki bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc EvrakIliskiKaydet(Kullanici kullanan, ObjectArray evrakIliskiArr);

        /// <summary>
        /// Parametre olarak verilen evrak EvrakIliski nesne koleksiyonunu bilgisini veritabanınından silen yordam.
        /// </summary>        
        /// <param name="evrakIliskiArr">Evrak ilişki bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        Sonuc EvrakIliskiSil(Kullanici kullanan, ObjectArray evrakIliskiArr);

        Sonuc EvrakDegisiklikKaydet(EvrakDegisiklik form);
        ObjectArray EvrakDegisiklikListele(EvrakDegisiklik bilgi);

        //ObjectArray KisiUzerindekileriListele(Kullanici kullanan, string tcKimlikNo);
        //ObjectArray KisiUzerindekileriListele(Kullanici kullanan, EvrakSinif form);

        ObjectArray EvrakHareketleriniRaporla(Kullanici kullanan, EvrakSinif form);

        ObjectArray EvrakEkDosyalariniRaporla(Kullanici kullanan, EvrakSinif form);

        ObjectArray DosyaTasnifPlaniRaporla(Kullanici kullanan, EvrakSinif form);

        Sonuc EvrakVersiyonKaydet(EvrakVersiyon evrVersiyon);
        EvrakVersiyon[] EvrakVersiyonListele(Kullanici kullanan, EvrakVersiyon form);
        EvrakVersiyon EvrakVersiyonOku(Kullanici kullanan, EvrakVersiyon form);
        Sonuc Sil(Kullanici kullanan, int kod);

        Sonuc EvrakVersiyon2Kaydet(EvrakVersiyon2 form);
        EvrakVersiyon2[] EvrakVersiyon2Listele(Kullanici kullanan, EvrakVersiyon2 form);
        EvrakVersiyon2 EvrakVersiyon2Oku(Kullanici kullanan, EvrakVersiyon2 form);

        bool EvrakOlaySorgu(string prEvrakNo, string evrEvrakNo, ENUMOlaySorguTip olaySorgu);

        Sonuc ImzaYoluKaydet(Kullanici kullanan, ImzaYolu form);
        Sonuc ImzaYoluSil(Kullanici kullanan, ImzaYolu form);
        ImzaYolu[] ImzaYoluListele(Kullanici kullanan, ImzaYolu form);
        ImzaYolu ImzaYoluOku(Kullanici kullanan, ImzaYolu form);

        Sonuc EvrakImzaYoluKaydet(Kullanici kullanan, EvrakImzaYolu form);
        Sonuc EvrakImzaYoluKaydet(Kullanici kullanan, EvrakImzaYoluHareket form);
        Sonuc EvrakImzaYoluSil(Kullanici kullanan, EvrakImzaYolu form);
        EvrakImzaYoluHareket[] EvrakImzaYoluListele(Kullanici kullanan, string prEvrakNo);
        EvrakImzaYolu EvrakImzaYoluOku(Kullanici kullanan, EvrakImzaYolu form);
        //string EvrakImzalayacakKisi(Kullanici kullanan, EvrakSinif form);
        Sonuc ImzaYolunaGoreImzaYetkisiVarMi(Kullanici kullanan, ObjectArray evraklar);

        Sonuc ArsivKaydet(Kullanici kullanan, Arsiv form);
        Sonuc ArsivSil(Kullanici kullanan, Arsiv form);
        Arsiv[] ArsivListele(Kullanici kullanan, Arsiv form);
        Arsiv ArsivOku(Kullanici kullanan, Arsiv form, bool metaDetay = false);

        Sonuc EvrakArsivKaydet(Kullanici kullanan, EvrakArsiv form);
        Sonuc EvrakArsivKaydet(Kullanici kullanan, EvrakArsiv[] formList);
        Sonuc EvrakArsivGuncelle(Kullanici kullanan, EvrakArsiv form);
        Sonuc EvrakArsivSil(Kullanici kullanan, EvrakArsiv form);
        EvrakSinif[] EvrakArsivListele(Kullanici kullanan, Arsiv form);
        EvrakArsiv[] EvrakArsivListele(Kullanici kullanan, EvrakArsiv form);
        EvrakArsivMeta[] EvrakArsivListele(Kullanici kullanan, EvrakArsivMeta form);
        EvrakArsivDosya[] EvrakArsivListele(Kullanici kullanan, EvrakArsivDosya form);
        EvrakArsivDosya[] EvrakArsivListele(Kullanici kullanan, int arsivKod, EvrakArsivMeta[] formList);
        EvrakArsiv[] EvrakArsivAra(Kullanici kullanan, int arsivKod, EvrakArsivMeta[] formList);

        string[] OneriListele(string kriter, ENUMOneriTip oneriTip);

        Sonuc MetaKaydet(Kullanici kullanan, Meta form);
        Sonuc MetaSil(Kullanici kullanan, Meta form);
        Meta[] MetaListele(Kullanici kullanan, Meta form, bool detay = false);
        Meta MetaOku(Kullanici kullanan, Meta form);

        Sonuc EvrakBilgiKaydet(Kullanici kullanan, EvrakBilgi form);
        Sonuc EvrakBilgiSil(Kullanici kullanan, EvrakBilgi form);
        EvrakBilgi[] EvrakBilgiListele(Kullanici kullanan, EvrakBilgi form);
        EvrakBilgi[] EvrakBilgiListele(Kullanici kullanan, string[] prEvrakNoListe);
        EvrakBilgi EvrakBilgiOku(Kullanici kullanan, EvrakBilgi form);

        Sonuc EvrakBilgiIliskiKaydet(Kullanici kullanan, EvrakBilgiIliski form);
        Sonuc EvrakBilgiIliskiKaydet(Kullanici kullanan, EvrakBilgiIliski[] formList);
        Sonuc EvrakBilgiIliskiSil(Kullanici kullanan, EvrakBilgiIliski form);
        EvrakBilgiIliski[] EvrakBilgiIliskiListele(Kullanici kullanan, EvrakBilgiIliski form);
        EvrakBilgiIliski EvrakBilgiIliskiOku(Kullanici kullanan, EvrakBilgiIliski form);

        EvrakImha[] EvrakImhaListele(Kullanici kullanan, TNSDateTime baslangicTarihi, TNSDateTime bitisTarihi);
        EvrakTasfiyeImha[] EvrakTasfiyeImhaListele(Kullanici kullanan, TNSDateTime baslangicTarihi, TNSDateTime bitisTarihi, EvrakTasfiyeImha form);
        int EvrakTasfiyeImhaZamaniGelenEvrakSayisi(string evrakBirimKod, TNSDateTime bitisTarihi);

        Sonuc EvrakDosyaKayitEdilebilirMi(Kullanici kullanan, string prEvrakNo, string kullaniciEvrakBirimi);
        bool EvrakGormeYetkisiVarMi(Kullanici kullanan, EvrakSinif form);
        string EvrakBirimAdGetir(string evrakBirimKod);
        string EvrakBirimAdGetir(int idariKimlikKod);
        string EvrakBirimKisaAdGetir(string evrakBirimKod);
        string DosyaPlaniAdGetir(string dosyaKodu);
        string KullaniciAdGetir(string tcKimlikNo);
        string EvrakGelisAdGetir(string kod);

        Sonuc HavaleFavoriKaydet(Kullanici kullanan, HavaleFavori form);
        Sonuc HavaleFavoriKaydet(Kullanici kullanan, HavaleFavori[] list);
        Sonuc HavaleFavoriSil(Kullanici kullanan, string[] kodList);
        HavaleFavori[] HavaleFavoriListele(Kullanici kullanan, HavaleFavori form);
        HavaleFavori HavaleFavoriOku(Kullanici kullanan, HavaleFavori form);

        Sonuc VekaletKaydet(Kullanici kullanan, Vekalet form);
        Sonuc VekaletSil(Kullanici kullanan, Vekalet form);
        Vekalet[] VekaletListele(Kullanici kullanan, Vekalet form);
        Vekalet VekaletOku(Kullanici kullanan, Vekalet form);
        Vekalet[] AktifVekaletleriListele(Kullanici kullanan);
        Vekalet AktifVekaletOku(Kullanici kullanan, string vekaletVerenMernis);

        Sonuc DTVTBirimKaydet(Kullanici kullanan, DTVTBirim form);
        string[] DTVTBirimListele(string query);
        DTVTBirim DTVTKurumOku(string idareKimlikKodu);
        DTVTBirim[] DTVTAltKurumlariGetir(string idareKimlikKodu);
        Sonuc DTVTKepAdresGuncelle(Kullanici kullanan, DTVTBirim form);
        int DtvtDisYazismaYapanKurumSayisi(string idareKimlikKodu, TNSDateTime guncellemeZamani);
        DTVTBirim[] DtvtDisYazismaYapanKurumlariGetir(string idareKimlikKodu, int start, int limit, TNSDateTime guncellemeZamani);
        Sonuc DTVTGuncellemeBilgisiKaydet(Kullanici kullanan, DTVTBirimBilgi form);
        DTVTBirimBilgi DTVTGuncellemeBilgisiGetir(string idareKimlikKodu);

        Sonuc DTVTBirimBilgisiKaydet(Birim form);
        string IdareKimlikKodGetir(string evrakBirimKod);
        string UstBirimKodGetir(string evrakBirimKod);
        string DagitimBirimKodGetir(string evrakBirimKod);

        Sonuc EvrakAraKriterKaydet(Kullanici kullanan, EvrakAraKriter form);
        EvrakAraKriter[] EvrakAraKriterListele(Kullanici kullanan, EvrakAraKriter form);
        EvrakAraKriter EvrakAraKriterOku(Kullanici kullanan, EvrakAraKriter form);

        Sonuc GelenEvrakNumaraVer(Kullanici kullanan, ObjectArray liste);
        ArrayList EvrakEmailAdresleriVer();
        Sonuc MailKaydet(string kullanici, string Uidl, int size, DateTime almaZamani, TNSMailMessage mesaj);
        string BirimKodGetir(string idareKimlikKod);
        bool BirimAktifMi(string kod);

        Sonuc EvrakDosyaBilgisiKaydet(Kullanici kullanan, string evrEvrakNo, bool dosyasiVar);

        string EvrEvrakNoGetir(string prEvrakNo);

        Sonuc EvrakImzaYoluYenile(Kullanici kullanan, EvrakSinif form);
        Sonuc AyniGidenEvrakMevcutMu(Kullanici kullanan, EvrakSinif form);
        bool EvrakNoElleVerilsinMi(Kullanici kullanan, string birimKod);

        EvrakImzaBilgi EvrakImzaBilgiOkuEski(Kullanici kullanan, int imzaYoluKod);
        EvrakImzaBilgi EvrakImzaBilgiOku2(Kullanici kullanan, int imzaYoluKod);

        Sonuc EvrakAyarlarKaydet(EvrakAyarlar form);
        EvrakAyarlar EvrakAyarlarOku(string anahtarSahibi, ENUMEvrakAyarlarAnahtar anahtar);
        EvrakAyarlar[] EvrakAyarlarListele(params EvrakAyarlar[] kriter);

        Sonuc EvrakDosyalariniListele(Kullanici kullanan);
        EvrakArsivRaporEvrakSayisi[] EvrakArsivRaporEvrakSayisi(Kullanici kullanan, TNSDateTime baslangicTarih, TNSDateTime bitisTarih, string islemYapan, int arsivKod);

        EvrakDosya[] EvrakDosyaOCRListele(Kullanici kullanan, EvrakDosya form, int top);
        Sonuc EvrakDosyaOCRGuncelle(Kullanici kullanan, EvrakDosya form);

        string OCRIslenecekDosyalariGonder();
        string OCRIslenenDosyalariAl();
        void OCRIsleminiBaslat();
        byte[] EvrakinDosyasiniGetir(string evrEvrakNo, int siraNo);
        string KriptoIsleminiBaslat();
        string IcerikIsleminiBaslat();

        Sonuc EvrakDosyaKaydet(Kullanici kullanan, EvrakDosya dosya, bool imzaKaldir = true);
        EvrakDosya[] EvrakDosyaListele(Kullanici kullanan, EvrakDosya form, int top, bool dagitimDosyalariniDahilEt = false);
        Sonuc EvrakDosyaDagitimDosyalariniSil(Kullanici kullanan, string evrEvrakNo, bool imzaKaldir = true);

        ObjectArray EvrakDosyaIcerikListele(string kelime, int yil);

        Sonuc FormKaydet(Kullanici kullanan, Form form);
        Form FormOku(Kullanici kullanan, Form form);

        Form[] FormListele(Kullanici kullanan, Form form);
        FormKontrol[] FormKontrolListele(Kullanici kullanan, FormKontrol form);
        FormOzellik[] FormOzellikListele(Kullanici kullanan, FormOzellik form);

        Sonuc EvrakFormKaydet(Kullanici kullanan, string sayfaKod, EvrakForm[] formList);
        EvrakForm[] EvrakFormListele(Kullanici kullanan, EvrakForm form);


        Sonuc TasfiyeKaydet(Kullanici kullanan, ObjectArray liste);
        Sonuc TasfiyeSil(Kullanici kullanan, string prEvrakNo);
        ObjectArray TasfiyeListele(Kullanici kullanan, EvrakSinif bilgi);

        Sonuc SaklamaPlanKaydet(Kullanici kullanan, EvrakSaklama[] liste);
        Sonuc SaklamaSil(Kullanici kullanan, string kod);
        ArrayList SaklamaListele(Kullanici kullanan, string prEvrakNo);

        Sonuc EvrakFaksKaydet(Kullanici kullanan, ObjectArray liste);
        Sonuc EvrakFaksGuncelle(ObjectArray liste, string islem);
        Sonuc EvrakFaksSil(Kullanici kullanan, string prEvrakNo);
        ObjectArray EvrakFaksListele(Kullanici kullanan, EvrakFaks bilgi);

        Sonuc EvrakFaksTarihceKaydet(Kullanici kullanan, ObjectArray liste);
        Sonuc EvrakFaksTarihceSil(Kullanici kullanan, string prEvrakNo);
        ObjectArray EvrakFaksTarihceListele(Kullanici kullanan, EvrakFaksTarihce bilgi);
        Sonuc EvrakFaksKimlikKaydet(EvrakFaksKimlik bilgi);
        String FaksBirimGetir(string kimlik);
        ObjectArray FaksDosyaKoduGetir(string birim);
        String EvrakFaksKimlikBul(string birim);

        Sonuc EtiketKaydet(Kullanici kullanan, ObjectArray liste);
        Sonuc EtiketUstKodDegistir(Kullanici kullanan, ObjectArray liste);
        Sonuc EtiketSil(Kullanici kullanan, ObjectArray liste);
        ObjectArray EtiketListele(Etiket bilgi);
        Sonuc EvrakEtiketKaydet(Kullanici kullanan, ObjectArray liste);
        Sonuc EvrakEtiketSil(Kullanici kullanan, ObjectArray liste);

        Sonuc EvrakParafImzaIslem(Kullanici kullanan, EvrakSinif evrak, string gonderilecekBirim, string gonderilecekKisi, ENUMParafImzaIslemTip parafImzaIslemTip);
        Sonuc EvrakParafImzaGonder(Kullanici kullanan, EvrakSinif evrak, string gonderilecekBirim, string gonderilecekKisi, ENUMParafImzaGonderTip parafImzaGonderTip, bool sonImzaMi);

        EvrakFizikselYeri[] ListeleEvrakFizikselYeri(Kullanici kullanan, EvrakFizikselYeri form);
        EvrakFizikselYeri OkuEvrakFizikselYeri(Kullanici kullanan, EvrakFizikselYeri form);
        Sonuc KaydetEvrakFizikselYeri(Kullanici kullanan, EvrakFizikselYeri form);

        Sonuc EvrakDagitimHareketKaydet(Kullanici kullanan, EvrakDagitimHareket[] formList);
        Sonuc EvrakDagitimHareketSil(Kullanici kullanan, string evrEvrakNo);
        EvrakDagitimHareket[] EvrakDagitimHareketListele(Kullanici kullanan, EvrakDagitimHareket form);

        Sonuc EvrakDagitimaGonder(Kullanici kullanan, EvrakSinif[] evraklar, EvrakDagitimHareket[] evrakDagitimHareketListe, bool dosyayaKaldir);

        Sonuc EVREYazismaBilgiKaydet(Kullanici kullanan, EYazismaBilgi form);
        Sonuc EVREYazismaBilgiSil(Kullanici kullanan, int eYazismaServisTur);
        EYazismaBilgi EVREYazismaBilgiOku(EYazismaBilgi form);
        EYazismaBilgi[] EVREYazismaBilgiListele();

        //Sonuc EYAzismaIsleminiBaslat();

        Sonuc EvrakiHazirlayanaGonder(Kullanici kullanan, string prEvrakNo);

        string EPostaGonder(Kullanici kullanan, string evrEvrakNo, string from, string to, string cc, string bcc, string konu, string bilgi, bool html, bool replyVar, TNSCollection ekDosyaIsimleri);

        bool EvrakImzaliMi(Kullanici kullanan, string evrEvrakNo);
        Sonuc ImzalaParafla2(Kullanici kullanan, string evrEvrakNo, EvrakHareket islemHareket, bool eImzaMi, object surecResultDegeri);
        Sonuc GeriGonder2(Kullanici kullanan, EvrakHareket islemHareket);

        ObjectArray SayacListele(string kod);
        Sonuc SayacKaydet(Kullanici kullanan, ObjectArray kod);
        Sonuc EvrakImzaKaldir(Kullanici kullanan, string evrEvrakNo);
        Sonuc EvrakSaklamaSuresiHesaplaveKaydet(Kullanici kullanan, string prEvrakNo);
        Sonuc EvrakSaklamaPlaniIslemGuncelle(Kullanici kullanan, string prEvrakNoListe, int islem);

        Sonuc EvrakTarihceKaydet(Kullanici kullanan, EvrakTarihce form);

        Sonuc ErisimYetkisiDegistir(Kullanici kullanan, EvrakHareket form);
        Sonuc EvrakSahiplikBilgisiDegistir(Kullanici kullanan, EvrakHareket form);

        ObjectArray DosyayaKaldirilmisEvrakListele(EvrakSinif bilgi);

        Sonuc SistemBilgisiKaydet(string digerUstVeriler);
        string SistemBilgisiOku();
        string GizlilikTipiGetir(int gizlilikTip);

        //******************************************
        Sonuc OrtakDosyalarKaydet(Kullanici kullanan, OrtakDosyalar yk);
        ObjectArray OrtakDosyalarListele(Kullanici kullanan, OrtakDosyalar yk);
        Sonuc OrtakDosyalarBirimKlasoruKontrol(Kullanici kullanan, OrtakDosyalarBirim bilgi);
        ObjectArray OrtakDosyalarDosyaYoluVerUst(Kullanici kullanan, int kod);
        ObjectArray OrtakDosyalarDosyaYoluVerAlt(Kullanici kullanan, int kod);
        Sonuc OrtakDosyalarYapistir(Kullanici kullanan, string islemTur, int nereye, string[] islemYapilacakKayitKodlari, string klasorYolu);
        Sonuc OrtakDosyalarSil(Kullanici kullanan, int[] islemYapilacakKayitKodlari, string klasorYolu);
        Sonuc OrtakDosyalarAdDegistir(Kullanici kullanan, OrtakDosyalar yk);
        Sonuc OrtakDosyalarTarihceKaydet(Kullanici kullanan, OrtakDosyalarTarihce yk);
        Sonuc OrtakDosyalarYetkiKaydet(Kullanici kullanan, ObjectArray liste);
        ObjectArray OrtakDosyalarYetkiListele(Kullanici kullanan, int[] kodlar);
        ObjectArray OrtakDosyalarTarihceListele(Kullanici kullanan, OrtakDosyalarTarihce yk);
        Sonuc OrtakDosyalarNotKaydet(Kullanici kullanan, OrtakDosyalarNot yk);
        ObjectArray OrtakDosyalarNotListele(Kullanici kullanan, OrtakDosyalarNot yk);
        Sonuc OrtakDosyalarNotSil(Kullanici kullanan, OrtakDosyalarNot yk);
        Sonuc OrtakDosyalarSurumKaydet(Kullanici kullanan, OrtakDosyalarSurumler yk);
        ObjectArray OrtakDosyalarSurumListele(Kullanici kullanan, OrtakDosyalarSurumler yk);
        Sonuc OrtakDosyalarSurumSil(Kullanici kullanan, OrtakDosyalarSurumler yk);
        Sonuc OrtakDosyalardanSurumeKaydet(Kullanici kullanan, OrtakDosyalar yk);
        ObjectArray OrtakDosyalarSurumDosyaYoluVerAlt(Kullanici kullanan, int kod);
        OrtakDosyalarKullanimAlani OrtakDosyalarKullanimBilgisi(Kullanici kullanan);
        bool OrtakDosyalarYetkisiVarmi(Kullanici kullanan, int[] dosyaKodlar, int yetkiTuru);
        int OrtakDosyalarBaglantiAdresindenKodBul(string adres);
        //******************************************

        Sonuc DuyuruGrubuKaydet(Kullanici kullanan, DuyuruGrubu form);
        Sonuc DuyuruGrubuSil(Kullanici kullanan, DuyuruGrubu form);
        ObjectArray DuyuruGrubuListele(Kullanici kullanan, DuyuruGrubu bilgi);
        ObjectArray DuyuruGrubuKisiListele(Kullanici kullanan, DuyuruGrubuKisiler bilgi);

        //*********TNB KEB
        //object LoginOl(object loginType, string userName, string password, string result);
        //object MailListele(object filti, string result);
        //string MailGetir(string serviceTip, object uid, string folderName, object result);
        //string MailGoster(string tarih, string kimden, string kime, string cc, string konu, string icerik, string[] ekler);
        //object MailGonder(byte[] mail, string result);

        bool BilgiIcinGelenEvrakGeriGonderilebilsinMi(string birimKod);

        EvrakSicil[] ListeleSicil(Kullanici kullanan, EvrakSicil form);

        Sonuc EvrakAdresKaydet(Kullanici kullanan, EvrakAdresDefteri form);

        Sonuc EvrakAdresSil(Kullanici kullanan, int kod);

        ObjectArray EvrakAdresListele(Kullanici kullanan, EvrakAdresDefteri form, int grupKod);

        Sonuc EvrakAdresGrubuKaydet(Kullanici kullanan, EvrakAdresGrubuAdi form, ObjectArray adresListesi);

        Sonuc EvrakAdresGrubuSil(Kullanici kullanan, int grupKod);

        ObjectArray EvrakAdresGrubuListele(Kullanici kullanan, EvrakAdresGrubuAdi form);

        EvrakAdresDefteri[] GruptakiAdresleriGetir(Kullanici kullanan, string[] gruplar);

        Sonuc SonraHavaleEtKaydet(Kullanici kullanan, EvrakSonraHavaleEt form);
        Sonuc SonraHavaleEtGonderildiOlarakIsaretle(Kullanici kullanan, string prEvrakNo);
        EvrakSonraHavaleEt[] SonraHavaleEtListele(Kullanici kullanan, EvrakSonraHavaleEt form);

        Sonuc KEPGonderiKaydet(Kullanici kullanan, KEPGonderi form);
        ObjectArray KEPGonderiListele(Kullanici kullanan, KEPGonderi form);
        Sonuc KEPGonderiSMIMEKaydet(Kullanici kullanan, KEPGonderi form);
        Sonuc KEPGonderiDurumKaydet(Kullanici kullanan, KEPGonderi form);

        Sonuc SiraNoDegistir(string primaryKey, int siraNo1, int siraNo2, ENUMSiraNoDegistirTablo tabloAd);

        Birim[] BirimListele(string birimKodListesi);
        Birim[] BirimListeleYetkili(Kullanici kullanan, params int[] kullaniciTipleri);

        Sonuc HavaleGrupKaydet(Kullanici kullanan, HavaleGrup form);
        Sonuc HavaleGrupSil(Kullanici kullanan, HavaleGrup form);
        HavaleGrup[] HavaleGrupListele(Kullanici kullanan, HavaleGrup form);
        HavaleGrup HavaleGrupOku(Kullanici kullanan, HavaleGrup form);
        Sonuc HavaleGrupVarsayilanYap(Kullanici kullanan, HavaleGrup form);
        Sonuc HavaleGrupSayacGuncelle(Kullanici kullanan, HavaleGrupDetay[] detaylar);
        Sonuc HavaleGrupPaylasimDurumuBelirle(Kullanici kullanan, HavaleGrup form);

        Sonuc EvrakHareketDegistir(Kullanici kullanan, EvrakHareket[] form);

        EvrakMeta[] EvrakMetaListele(Kullanici kullanan, EvrakMeta form);
        Sonuc EvrakMetaKaydet(Kullanici kullanan, EvrakSinif evrak, EvrakMeta[] formList);


        Sonuc RolKaydet(Kullanici kullanan, Rol form);
        Sonuc RolSil(Kullanici kullanan, Rol form);
        Rol[] RolListele(Kullanici kullanan, Rol form);
        Rol RolOku(Kullanici kullanan, Rol form);
        int RolKullaniciYetkiDegerBul(ENUMYetkiler yetki, string tcKimlikNo);


        SaklamaPlani[] SaklamaPlaniListele(Kullanici kullanan, SaklamaPlani form);
        Sonuc SaklamaPlaniKaydet(Kullanici kullanan, SaklamaPlani form);
        Sonuc SaklamaPlaniSil(Kullanici kullanan, SaklamaPlani form);

        GuncellemeBilgi[] GuncellemeBilgiListele(Kullanici kullanan, string kod);
        Sonuc GuncellemeBilgiKaydet(Kullanici kullanan, string kod, string sql);
        Sonuc GuncellemeBilgiSil(Kullanici kullanan, string kod);
        Sonuc VeritabaniIslemleriSorgula(int islem);
        Sonuc VeritabaniIslemleriSorguCalistir(int islem);
        VeritabaniIndex[] VeritabaniIndexListele(VeritabaniIndex form, string baglantiSatiri);
        Sonuc VeritabaniIndexIslem(VeritabaniIndex[] liste, ENUMVeritabaniIndexIslem islem, string baglantiSatiri);
        Hashtable VeritabaniBaglantiSatiriListele();

        Sonuc KepHesapKaydet(Kullanici kullanan, KepHesap form);
        Sonuc KepHesapSil(Kullanici kullanan, KepHesap form);
        KepHesap[] KepHesapListele(Kullanici kullanan, KepHesap form);
        KepHesap KepHesapOku(Kullanici kullanan, KepHesap form);
        Sonuc KepHesapVarsayilanYap(Kullanici kullanan, KepHesap form);

        Sonuc KepEvrakKaydet(Kullanici kullanan, KepEvrak form);
        Sonuc KepEvrakSil(Kullanici kullanan, KepEvrak form);
        KepEvrak[] KepEvrakListele(Kullanici kullanan, KepEvrak form);
        KepEvrak KepEvrakOku(Kullanici kullanan, KepEvrak form);

        Sonuc KepAdresKaydet(Kullanici kullanan, KepAdres form);
        Sonuc KepAdresSil(Kullanici kullanan, KepAdres form);
        KepAdres[] KepAdresListele(Kullanici kullanan, KepAdres form);

        int EvrakTipGetir(string evrEvrakNo);

        Sonuc BirimDagitimYetkiKaydet(Kullanici kullanan, BirimDagitimYetki[] formList, string evrakBirimKod);
        BirimDagitimYetki[] BirimDagitimYetkiListele(Kullanici kullanan, BirimDagitimYetki form);

        Sonuc BirimHavaleYetkiKaydet(Kullanici kullanan, BirimHavaleYetki[] formList, string evrakBirimKod);
        BirimHavaleYetki[] BirimHavaleYetkiListele(Kullanici kullanan, BirimHavaleYetki form);

        bool EditorKullaniliyorMu(Kullanici kullanan, string evrEvrakNo);

        Sonuc EvrakInfocomDosyaImzala(Kullanici kullanan, InfocomDosyaImza form);
        ObjectArray EvrakInfocomDosyaListele(Kullanici kullanan, InfocomDosyaImza form);

        Sonuc EvrakInfocomApiTarihce(InfocomApiTarihce form);

    }
}