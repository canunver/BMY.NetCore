using System;
using System.Data;
using TNS.Veri;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using System.Xml;
using System.Net;
using System.Collections.Generic;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// HRCMerkezBankKur formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCMerkezBankKur
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriHRC";

        /// <summary>
        /// MerkezBankasiKurSinif nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tarihBas">The tarih bas.</param>
        /// <param name="tarihBitis">The tarih bitis.</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public static Sonuc Kaydet(Kullanici kullanan, TNSDateTime tarihBas, TNSDateTime tarihBitis)
        {
            // <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
            Sonuc s = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCMERKEZBANKKUR tabHRCMERKEZBANKKUR = new TABHRCMERKEZBANKKUR(ve);

            s = KurBilgiBankasiniOlustur(ve, tarihBas, tarihBitis);

            ve.VeriErisimKapat();
            return s;
        }

        private static Sonuc KurBilgiBankasiniOlustur(VeriErisim veriErisim, TNSDateTime tarihBas, TNSDateTime tarihBitis)
        {

            Sonuc sonuc = new Sonuc(true, "", "");

            if (tarihBas.Oku() > DateTime.Now)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00223;
                return sonuc;
            }

            if (tarihBitis.Oku().AddDays(-1) > DateTime.Now)//Bitis tarihi bugun olarak verildiğinde uyarı vermesin
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00224;
                return sonuc;
            }
            if (tarihBas.Oku() > tarihBitis.Oku())
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00225;
                return sonuc;
            }

            string kurKaynak = (TNS.DEG.Arac.DegiskenDegerBul(0, "/KurKaynak") + "").ToLower();

            if (kurKaynak == "tcmb")
            {
                int deneme = 0;
                while (deneme < 10)
                {
                    try
                    {
                        MerkezBankasiKurSinif merkezKur = KurBilgisiniAl(tarihBas);
                        sonuc = GunlukKurlariKaydet(veriErisim, merkezKur);
                        deneme = 0;
                    }
                    catch (Exception exc)
                    {
                        deneme++;

                        if (exc.Message.IndexOf("(404)") < 0)
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = exc.Message;
                        }
                    }
                    tarihBas.AddDays(1);
                    if (!sonuc.islemSonuc || tarihBas.Oku() > tarihBitis.Oku())
                        break;
                }
            }
            if (kurKaynak == "ab")
            {
                while (true)
                {
                    try
                    {
                        MerkezBankasiKurSinif merkezKur = KurBilgisiniAlAB(tarihBas);
                        sonuc = GunlukKurlariKaydet(veriErisim, merkezKur);
                    }
                    catch (Exception exc)
                    {
                        if (exc.Message.IndexOf("(404)") < 0)
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = exc.Message;
                            break;
                        }
                    }
                    tarihBas.AddDays(1);
                    if (!sonuc.islemSonuc || tarihBas.Oku() > tarihBitis.Oku())
                        break;
                }
            }
            if (kurKaynak == "ecb")
            {
                while (true)
                {
                    try
                    {
                        MerkezBankasiKurSinif merkezKur = KurBilgisiniAlECB(tarihBas);
                        sonuc = GunlukKurlariKaydet(veriErisim, merkezKur);
                    }
                    catch (Exception exc)
                    {
                        if (exc.Message.IndexOf("(404)") < 0)
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = exc.Message;
                            break;
                        }
                    }
                    tarihBas.AddDays(1);
                    if (!sonuc.islemSonuc || tarihBas.Oku() > tarihBitis.Oku())
                        break;
                }
            }
            if (kurKaynak == "ktcmb")
            {
                int deneme = 0;
                while (deneme < 10)
                {
                    try
                    {
                        MerkezBankasiKurSinif merkezKur = KurBilgisiniAlKibris(tarihBas);

                        sonuc = GunlukKurlariKaydet(veriErisim, merkezKur);
                        deneme = 0;
                    }
                    catch (Exception exc)
                    {
                        deneme++;
                        if (exc.Message.IndexOf("(404)") < 0)
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = exc.Message;
                        }
                    }

                    tarihBas.AddDays(1);

                    if (!sonuc.islemSonuc || tarihBas.Oku() > tarihBitis.Oku())
                        break;
                }
            }

            if (kurKaynak == "kirgiz")
            {
                ArrayList al = new ArrayList();
                MerkezBankasiKurSinif merkezKurKirgiz = new MerkezBankasiKurSinif();
                MerkezBankasiKurSinif merkezKurTurk = new MerkezBankasiKurSinif();

                TNSDateTime aktarimTarihi = new TNSDateTime(tarihBas.Oku().Date);

                while (true)
                {
                    merkezKurTurk = KurBilgisiniAl(aktarimTarihi);
                    //for (int i = 0; i < 20; i++)
                    //{
                    //    try
                    //    {
                    //        merkezKurTurk = KurBilgisiniAl(aktarimTarihi);
                    //        break;
                    //    }
                    //    catch (Exception)
                    //    {
                    //        aktarimTarihi.AddDays(-1);
                    //    }
                    //}

                    foreach (MerkezBankasiKurDetaySinif d in merkezKurTurk.satirlar)
                    {
                        d.kurTarihi = new TNSDateTime(tarihBas.Oku().Date);
                        //d.kurTarihi.AddDays(1);
                        //if (d.kurTarihi.Oku() > tarihBitis.Oku())
                        //    break;

                        if (d.paraBirim.kisaltma == "USD")
                        {
                            d.paraBirim.kisaltma = "TRY";
                            d.dovizAlis = 1 / d.dovizAlis;
                            d.dovizSatis = 1 / d.dovizSatis;
                            d.efektifAlis = 1 / d.efektifAlis;
                            d.efektifSatis = 1 / d.efektifSatis;
                            al.Add(d);
                        }

                        if (d.paraBirim.kisaltma == "EUR")
                        {
                            d.paraBirim.kisaltma = "EUR";
                            d.dovizAlis = d.crossRateOther;
                            d.dovizSatis = d.crossRateOther;
                            d.efektifAlis = d.crossRateOther;
                            d.efektifSatis = d.crossRateOther;
                            al.Add(d);
                        }

                        if (d.paraBirim.kisaltma == "CHF")
                        {
                            d.paraBirim.kisaltma = "CHF";
                            d.dovizAlis = 1 / d.crossRateUSD;
                            d.dovizSatis = 1 / d.crossRateUSD;
                            d.efektifAlis = 1 / d.crossRateUSD;
                            d.efektifSatis = 1 / d.crossRateUSD;
                            al.Add(d);
                        }
                    }

                    tarihBas.AddDays(1);
                    aktarimTarihi = new TNSDateTime(tarihBas.Oku().Date);

                    if (!sonuc.islemSonuc || tarihBas.Oku() > tarihBitis.Oku())
                        break;
                }

                //Kırgız bilgilerini al
                try
                {
                    merkezKurKirgiz = KurBilgisiniAlKirgiz(tarihBitis);

                    foreach (MerkezBankasiKurDetaySinif d in merkezKurKirgiz.satirlar)
                    {
                        al.Add(d);
                    }
                }
                catch { }

                MerkezBankasiKurSinif merkezKur = new MerkezBankasiKurSinif();
                merkezKur.satirlar = (MerkezBankasiKurDetaySinif[])al.ToArray(typeof(MerkezBankasiKurDetaySinif));

                sonuc = GunlukKurlariKaydet(veriErisim, merkezKur);
            }

            return sonuc;
        }

        public static Sonuc GunlukKurlariKaydet(VeriErisim veriErisim, MerkezBankasiKurSinif form)
        {
            Sonuc s = new Sonuc(true, "", "");

            if (form.satirlar.Length == 0) return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, "Sistem");
            TABHRCMERKEZBANKKUR tabHRCMERKEZBANKKUR = new TABHRCMERKEZBANKKUR(ve);

            ve.BeginTransaction();

            if (form.satirlar.Length > 0)
                s = tabHRCMERKEZBANKKUR.Sil(form);

            for (int i = 0; i < form.satirlar.Length && s.islemSonuc; i++)
            {
                //Tek gün aktarımı yapıldığında satırlar ile form.kurtarih aynı olabilir sorun olmaz
                //Çoklu aktarım yapıldığında form kurtarihi değil satırlardaki kur tarihi kullanılmalı. Çünkü satırlarda 10 günlük kur bilgisi oluyor
                form.kurTarih = new TNSDateTime(form.satirlar[i].kurTarihi.ToString());

                s = tabHRCMERKEZBANKKUR.Ekle(form, i);
            }

            if (s.islemSonuc)
            {
                //OrtakFonksiyonlar.HataDosyaYaz("TCCBKurCek.txt", "commit", false);
                ve.Commit();
            }
            else
            {
                OrtakFonksiyonlar.HataDosyaYaz("TCCBKurCek.txt", "rollback", false);
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return s;

        }

        private static MerkezBankasiKurSinif KurBilgisiniAl(TNSDateTime okunacakGun)
        {
            XmlDocument myXmlDocument = new XmlDocument();
            TNSDateTime islemGun = new TNSDateTime(okunacakGun.Oku());
            int deneme = 0;
            while (true)
            {
                islemGun.AddDays(-1);
                try
                {
                    deneme++;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    string filePath = string.Format("https://www.tcmb.gov.tr/kurlar/{0}{1}/{2}{1}{0}.xml", islemGun.Yil.ToString("0000"), islemGun.Ay.ToString("00"), islemGun.Gun.ToString("00"));
                    myXmlDocument.Load(filePath);
                    break;
                }
                catch (Exception exc)
                {
                    if (exc.Message.IndexOf("(404)") > 0 && deneme < 20) continue;
                    throw exc;
                }
            }

            XmlNode node = myXmlDocument.DocumentElement;

            MerkezBankasiKurSinif kur = new MerkezBankasiKurSinif();
            kur.kurTarih = new TNSDateTime(okunacakGun.Oku());

            ArrayList al = new ArrayList();
            foreach (XmlNode node1 in node.ChildNodes)
            {
                if (node1.Name == "Currency")
                {
                    if (node1.ChildNodes.Count > 3)
                    {
                        MerkezBankasiKurDetaySinif kurSatir = new MerkezBankasiKurDetaySinif();

                        kurSatir.paraBirim.kisaltma = node1.Attributes["Kod"].InnerText;
                        foreach (XmlNode node2 in node1.ChildNodes)
                        {
                            //if (node2.Name == "Unit")             
                            //if (node2.Name == "Isim")  
                            //if (node2.Name == "CurrencyName")

                            if (node2.Name == "ForexBuying")
                                kurSatir.dovizAlis = OrtakFonksiyonlar.ConvertToDecimal(node2.InnerText.Replace('.', ','), 0);
                            if (node2.Name == "ForexSelling")
                                kurSatir.dovizSatis = OrtakFonksiyonlar.ConvertToDecimal(node2.InnerText.Replace('.', ','), 0);
                            if (node2.Name == "BanknoteBuying")
                                kurSatir.efektifAlis = OrtakFonksiyonlar.ConvertToDecimal(node2.InnerText.Replace('.', ','), 0);
                            if (node2.Name == "BanknoteSelling")
                                kurSatir.efektifSatis = OrtakFonksiyonlar.ConvertToDecimal(node2.InnerText.Replace('.', ','), 0);
                            if (node2.Name == "CrossRateOther")
                                kurSatir.crossRateOther = OrtakFonksiyonlar.ConvertToDecimal(node2.InnerText.Replace('.', ','), 0);
                            if (node2.Name == "CrossRateUSD")
                                kurSatir.crossRateUSD = OrtakFonksiyonlar.ConvertToDecimal(node2.InnerText.Replace('.', ','), 0);

                            //if (node2.Name == "CrossRateOther")   

                        }

                        kurSatir.kurTarihi = kur.kurTarih;
                        al.Add(kurSatir);
                    }
                }
                kur.satirlar = (MerkezBankasiKurDetaySinif[])al.ToArray(typeof(MerkezBankasiKurDetaySinif));
            }
            return kur;
        }

        private static MerkezBankasiKurSinif KurBilgisiniAlAB(TNSDateTime okunacakGun)
        {
            XmlDocument myXmlDocument = new XmlDocument();
            TNSDateTime islemGun = new TNSDateTime(okunacakGun.Oku());
            int deneme = 0;
            string jsonData = "";
            while (true)
            {
                try
                {
                    deneme++;
                    string urlPath = string.Format("https://ec.europa.eu/budg/inforeuro/api/public/monthly-rates?year={0}&month={1}&lang=en", islemGun.Yil.ToString("0000"), islemGun.Ay.ToString("00"));
                    jsonData = OrtakFonksiyonlar.WebdenOku(urlPath);
                    break;
                    //jsonData = "[{\"country\":\"Thailand\",\"currency\":\"Thai baht\",\"isoA3Code\":\"THB\",\"isoA2Code\":\"TH\",\"value\":38.598,\"comment\":null},{ \"country\":\"Turkey\",\"currency\":\"Turkish lira\",\"isoA3Code\":\"TRY\",\"isoA2Code\":\"TR\",\"value\":4.3851,\"comment\":null}]";
                }
                catch (Exception exc)
                {
                    if (exc.Message.IndexOf("(404)") > 0 && deneme < 20) continue;
                    if (exc is WebException)
                    {
                        OrtakFonksiyonlar.HataDosyaYaz("ABKurCek.txt", exc.ToString(), false);
                        WebException wexc = (WebException)exc;
                        if (wexc != null && wexc.Response != null)
                        {
                            System.IO.StreamReader sr = new System.IO.StreamReader(wexc.Response.GetResponseStream());
                            OrtakFonksiyonlar.HataDosyaYaz("ABKurCek.txt", sr.ReadToEnd(), false);
                        }
                    }
                    throw exc;
                }
            }

            MerkezBankasiKurSinif kur = new MerkezBankasiKurSinif();
            kur.kurTarih = new TNSDateTime(okunacakGun.Oku());
            if (!string.IsNullOrWhiteSpace(jsonData))
            {
                AB_KurYapi[] gelenler = TNS.JsonIslem.Oku<AB_KurYapi[]>(jsonData);
                foreach (AB_KurYapi node1 in gelenler)
                {
                    if (node1.isoA3Code == "TRY")
                    {
                        MerkezBankasiKurDetaySinif kurSatir = new MerkezBankasiKurDetaySinif();

                        kurSatir.paraBirim.kisaltma = "EUR";
                        kurSatir.dovizAlis = node1.value;
                        kurSatir.dovizSatis = node1.value;
                        kurSatir.efektifAlis = node1.value;
                        kurSatir.efektifSatis = node1.value;


                        kurSatir.kurTarihi = kur.kurTarih;
                        kur.satirlar = new MerkezBankasiKurDetaySinif[1];
                        kur.satirlar[0] = kurSatir;
                        break;
                    }
                }
            }

            return kur;
        }

        private static MerkezBankasiKurSinif KurBilgisiniAlECB(TNSDateTime okunacakGun)
        {
            XmlDocument myXmlDocument = new XmlDocument();
            TNSDateTime islemGun = new TNSDateTime(okunacakGun.Oku());
            int deneme = 0;
            string jsonData = "";
            while (true)
            {
                try
                {
                    deneme++;
                    string urlPath = string.Format("https://api.exchangeratesapi.io/{0}-{1}-{2}", islemGun.Yil.ToString("0000"), islemGun.Ay.ToString("00"), islemGun.Gun.ToString("00"));
                    jsonData = OrtakFonksiyonlar.WebdenOku(urlPath);
                    break;
                    //jsonData = "[{\"country\":\"Thailand\",\"currency\":\"Thai baht\",\"isoA3Code\":\"THB\",\"isoA2Code\":\"TH\",\"value\":38.598,\"comment\":null},{ \"country\":\"Turkey\",\"currency\":\"Turkish lira\",\"isoA3Code\":\"TRY\",\"isoA2Code\":\"TR\",\"value\":4.3851,\"comment\":null}]";
                }
                catch (Exception exc)
                {
                    if (exc.Message.IndexOf("(404)") > 0 && deneme < 20) continue;
                    if (exc is WebException)
                    {
                        OrtakFonksiyonlar.HataDosyaYaz("ECBKurCek.txt", exc.ToString(), false);
                        WebException wexc = (WebException)exc;
                        if (wexc != null && wexc.Response != null)
                        {
                            System.IO.StreamReader sr = new System.IO.StreamReader(wexc.Response.GetResponseStream());
                            OrtakFonksiyonlar.HataDosyaYaz("CBKurCek.txt", sr.ReadToEnd(), false);
                        }
                    }
                    throw exc;
                }
            }

            MerkezBankasiKurSinif kur = new MerkezBankasiKurSinif();
            kur.kurTarih = new TNSDateTime(okunacakGun.Oku());
            if (!string.IsNullOrWhiteSpace(jsonData))
            {
                ECB_KurYapi gelen = TNS.JsonIslem.Oku<ECB_KurYapi>(jsonData);
                List<MerkezBankasiKurDetaySinif> l = new List<MerkezBankasiKurDetaySinif>();
                kur.kurTarih = new TNSDateTime(gelen.date);
                DovizDetayEkle(kur, l, "TRY", gelen.rates.TRY);
                DovizDetayEkle(kur, l, "USD", gelen.rates.USD);
                DovizDetayEkle(kur, l, "GBP", gelen.rates.GBP);

                if (l.Count > 0)
                {
                    kur.satirlar = l.ToArray();
                }
            }

            return kur;
        }

        private static void DovizDetayEkle(MerkezBankasiKurSinif kur, List<MerkezBankasiKurDetaySinif> l, string kisaltma, decimal deger)
        {
            MerkezBankasiKurDetaySinif kurSatir = new MerkezBankasiKurDetaySinif();
            if (deger == 0) return;
            kurSatir.paraBirim.kisaltma = kisaltma;
            kurSatir.dovizAlis = 1m / deger;
            kurSatir.dovizSatis = kurSatir.dovizAlis;
            kurSatir.efektifAlis = kurSatir.dovizAlis;
            kurSatir.efektifSatis = kurSatir.dovizAlis;
            kurSatir.kurTarihi = kur.kurTarih;
            l.Add(kurSatir);
        }

        private static MerkezBankasiKurSinif KurBilgisiniAlKibris(TNSDateTime okunacakGun)
        {
            MerkezBankasiKurSinif kur = new MerkezBankasiKurSinif();
            ArrayList al = new ArrayList();
            try
            {
                string filePath = "https://www.mb.gov.ct.tr/kur/tarih/";
                filePath += okunacakGun.Yil + "" + okunacakGun.Ay.ToString("00") + okunacakGun.Gun.ToString("00");

                //OrtakFonksiyonlar.HataStrYaz("Kıbrıs Kur Bilgisi İşleniyor");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    delegate { return true; };
                XmlDocument myXmlDocument = new XmlDocument();
                myXmlDocument.Load(filePath);
                XmlNode node;
                node = myXmlDocument.DocumentElement;

                //kur.kurTarih = new TNSDateTime(node.Attributes["Kur_Tarihi"].InnerText);

                foreach (XmlNode node1 in node.ChildNodes)
                {
                    if (node1.Name == "Kur_Tarihi")
                    {
                        kur.kurTarih = new TNSDateTime(node1.InnerText);
                        break;
                    }
                }

                foreach (XmlNode nodeAna in node.ChildNodes)
                {
                    if (nodeAna.Name == "Resmi_Kurlar")
                    {
                        foreach (XmlNode node1 in nodeAna.ChildNodes)
                        {
                            if (node1.Name == "Resmi_Kur")
                            {
                                MerkezBankasiKurDetaySinif kurSatir = new MerkezBankasiKurDetaySinif();

                                foreach (XmlNode node2 in node1.ChildNodes)
                                {
                                    if (node2.Name == "Sembol")
                                        kurSatir.paraBirim.kisaltma = node2.InnerText;
                                    if (node2.Name == "Doviz_Alis")
                                        kurSatir.dovizAlis = OrtakFonksiyonlar.ConvertToDecimal(node2.InnerText.Replace('.', ','), 0);
                                    if (node2.Name == "Doviz_Satis")
                                        kurSatir.dovizSatis = OrtakFonksiyonlar.ConvertToDecimal(node2.InnerText.Replace('.', ','), 0);
                                    if (node2.Name == "Efektif_Alis")
                                        kurSatir.efektifAlis = OrtakFonksiyonlar.ConvertToDecimal(node2.InnerText.Replace('.', ','), 0);
                                    if (node2.Name == "Efektif_Satis")
                                        kurSatir.efektifSatis = OrtakFonksiyonlar.ConvertToDecimal(node2.InnerText.Replace('.', ','), 0);
                                }

                                kurSatir.kurTarihi = kur.kurTarih;
                                al.Add(kurSatir);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                OrtakFonksiyonlar.HataDosyaYaz("KIBRISKurCek.txt", exc.ToString(), false);
                throw exc;
            }
            kur.satirlar = (MerkezBankasiKurDetaySinif[])al.ToArray(typeof(MerkezBankasiKurDetaySinif));
            return kur;
        }

        private static MerkezBankasiKurSinif KurBilgisiniAlKirgiz(TNSDateTime okunacakGun)
        {
            //OrtakFonksiyonlar.HataStrYaz("Kırgızistan Kur Bilgisi İşleniyor");
            string filePath = "https://www.nbkr.kg/XML/daily.xml";

            XmlDocument myXmlDocument = new XmlDocument();
            myXmlDocument.Load(filePath);
            XmlNode node;
            node = myXmlDocument.DocumentElement;

            MerkezBankasiKurSinif kur = new MerkezBankasiKurSinif();
            kur.kurTarih = new TNSDateTime(node.Attributes["Date"].Value);

            ArrayList al = new ArrayList();
            foreach (XmlNode node1 in node.ChildNodes)
            {
                if (node1.Name == "Currency")
                {
                    if (node1.ChildNodes.Count > 1)
                    {
                        MerkezBankasiKurDetaySinif kurSatir = new MerkezBankasiKurDetaySinif();

                        kurSatir.paraBirim.kisaltma = node1.Attributes["ISOCode"].Value;

                        if (kurSatir.paraBirim.kisaltma != "USD") continue;

                        foreach (XmlNode node2 in node1.ChildNodes)
                        {
                            if (node2.Name == "Value")
                            {
                                kurSatir.paraBirim.kisaltma = "KGS";
                                kurSatir.dovizAlis = 1 / OrtakFonksiyonlar.ConvertToDecimal(node2.InnerText.Replace('.', ','), 0);
                                kurSatir.dovizSatis = 1 / OrtakFonksiyonlar.ConvertToDecimal(node2.InnerText.Replace('.', ','), 0);
                                kurSatir.efektifAlis = 1 / OrtakFonksiyonlar.ConvertToDecimal(node2.InnerText.Replace('.', ','), 0);
                                kurSatir.efektifSatis = 1 / OrtakFonksiyonlar.ConvertToDecimal(node2.InnerText.Replace('.', ','), 0);
                            }
                        }

                        kurSatir.kurTarihi = kur.kurTarih;
                        al.Add(kurSatir);
                    }
                }
                kur.satirlar = (MerkezBankasiKurDetaySinif[])al.ToArray(typeof(MerkezBankasiKurDetaySinif));
            }
            return kur;


        }

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, MerkezBankasiKurSinif form)
        {
            return true;
        }

        /// <summary>
        /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AnahtarAlanlarGecerliMi(Kullanici kullanan, MerkezBankasiKurSinif form)
        {

            if (form.kurTarih.isNull)
                return new Sonuc(false, "Geçersiz bir tarih girdiniz.", "");
            return new Sonuc(true, "", "");
        }


        /// <summary>
        /// MerkezBankasiKurSinif nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MerkezBankasiKurSinif yapısında MerkezBankasiKurSinif: bilgilerini tutan değişken</param>
        /// <returns></returns>
        public static MerkezBankasiKurSinif Oku(VeriErisim veriErisim, Kullanici kullanan, MerkezBankasiKurSinif form)
        {
            MerkezBankasiKurSinif formIc = new MerkezBankasiKurSinif();
            formIc.sonuc = AnahtarAlanlarGecerliMi(kullanan, form);
            if (!formIc.sonuc.islemSonuc)
            {
                return formIc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCMERKEZBANKKUR tabHRCMERKEZBANKKUR = new TABHRCMERKEZBANKKUR(ve);
            IDataReader dr = null;
            if (form.yakinGunBul)
            {
                dr = tabHRCMERKEZBANKKUR.YakinGunBul(form);

                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    formIc.sonuc.DegerAta(ve.hata);
                    return formIc;
                }

                bool bulundu = false;
                if (dr.Read())
                {
                    bulundu = true;
                    form.kurTarih = VeriErisim.DegerAl(dr, 0);
                    formIc.kurTarih = VeriErisim.DegerAl(dr, 0);
                }
                dr.Close();
                if (!bulundu)
                {
                    formIc.sonuc.DegerAta(TNS.HRC.HarcamaSurecleriResI.HRC00227);
                    ve.VeriErisimKapat();
                    return formIc;
                }
            }

            ArrayList al = new ArrayList();
            formIc.kurTarih = form.kurTarih;
            dr = tabHRCMERKEZBANKKUR.Oku(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    MerkezBankasiKurDetaySinif detay = new MerkezBankasiKurDetaySinif();
                    detay.kurTarihi = VeriErisim.DegerAl(dr, (int)TABHRCMERKEZBANKKUR.ENUMAlanlar.KURTARIH);
                    detay.paraBirim.kisaltma = VeriErisim.DegerAl(dr, (int)TABHRCMERKEZBANKKUR.ENUMAlanlar.PARABIRIMI, "");
                    detay.dovizAlis = (decimal)VeriErisim.DegerAl(dr, (int)TABHRCMERKEZBANKKUR.ENUMAlanlar.DOVIZALIS, (decimal)0);
                    detay.dovizSatis = (decimal)VeriErisim.DegerAl(dr, (int)TABHRCMERKEZBANKKUR.ENUMAlanlar.DOVIZSATIS, (decimal)0);
                    detay.efektifAlis = (decimal)VeriErisim.DegerAl(dr, (int)TABHRCMERKEZBANKKUR.ENUMAlanlar.EFEKTIFALIS, (decimal)0);
                    detay.efektifSatis = (decimal)VeriErisim.DegerAl(dr, (int)TABHRCMERKEZBANKKUR.ENUMAlanlar.EFEKTIFSATIS, (decimal)0);
                    detay.serbestAlis = (decimal)VeriErisim.DegerAl(dr, (int)TABHRCMERKEZBANKKUR.ENUMAlanlar.SERBESTALIS, (decimal)0);
                    detay.serbestSatis = (decimal)VeriErisim.DegerAl(dr, (int)TABHRCMERKEZBANKKUR.ENUMAlanlar.SERBESTSATIS, (decimal)0);
                    detay.paraBirim.kod = VeriErisim.DegerAl(dr, 8, 0);
                    detay.paraBirim.adi = VeriErisim.DegerAl(dr, 9, "");
                    al.Add(detay);
                }
                dr.Close();
            }

            if (form.tumParalariDondur)
            {
                BKD.ParaBirimi kriter = new BKD.ParaBirimi();
                ObjectArray bilgiler = HRCParaBirimi.Listele(ve, kullanan, kriter);

                foreach (BKD.ParaBirimi item in bilgiler.objeler)
                {
                    if (KurBul(al, item.kisaltma) < 0)
                    {
                        MerkezBankasiKurDetaySinif detay = new MerkezBankasiKurDetaySinif();
                        detay.kurTarihi = form.kurTarih;
                        detay.paraBirim.kisaltma = item.kisaltma;
                        detay.dovizAlis = 0;
                        detay.dovizSatis = 0;
                        detay.efektifAlis = 0;
                        detay.efektifSatis = 0;
                        detay.serbestAlis = 0;
                        detay.serbestSatis = 0;
                        detay.paraBirim.kod = 0;
                        detay.paraBirim.adi = item.adi;
                        al.Add(detay);
                    }
                }
            }

            formIc.satirlar = (MerkezBankasiKurDetaySinif[])al.ToArray(typeof(MerkezBankasiKurDetaySinif));
            ve.VeriErisimKapat();
            return formIc;
        }

        private static int KurBul(ArrayList al, string kisaltma)
        {
            for (int i = 0; i < al.Count; i++)
            {
                MerkezBankasiKurDetaySinif item = (MerkezBankasiKurDetaySinif)al[i];
                if (item.paraBirim.kisaltma == kisaltma)
                    return i;

            }
            return -1;
        }

        /// <summary>
        /// Verilen tarih ve önceki günündeki kur bilgilerini döndürür
        /// </summary>
        /// <param name="form">İstenilen tarih bilgisini içerir</param>
        /// <param name="kurlar">string[] İstenilen döviz kodlarını içerir</param>
        /// <returns>KurDegerleri tipinde nesneler içeren ObjectArray döndürür</returns>
        public static ObjectArray KurDegerleri(MerkezBankasiKurSinif form, string[] kurlar)
        {
            form.kurTarih = new TNSDateTime(form.kurTarih.ToString());
            form.satirlar = new MerkezBankasiKurDetaySinif[kurlar.Length];
            for (int i = 0; i < kurlar.Length; i++)
            {
                form.satirlar[i] = new MerkezBankasiKurDetaySinif();
                form.satirlar[i].paraBirim.kisaltma = kurlar[i];
            }
            ObjectArray liste = new ObjectArray();
            MerkezBankasiKurSinif degerler = Oku(null, new Kullanici(), form);

            for (int j = 0; j < kurlar.Length; j++)
            {
                for (int i = 0; i < degerler.satirlar.Length; i++)
                {
                    if (degerler.satirlar[i].paraBirim.kisaltma == kurlar[j])
                    {
                        form.kurTarih = degerler.satirlar[i].kurTarihi;
                        KurDegerleri kur = new KurDegerleri();
                        kur.paraBirimi.adi = degerler.satirlar[i].paraBirim.adi;
                        kur.paraBirimi.kisaltma = degerler.satirlar[i].paraBirim.kisaltma;
                        kur.alis = degerler.satirlar[i].dovizAlis;
                        kur.satis = degerler.satirlar[i].dovizSatis;
                        kur.efektifAlis = degerler.satirlar[i].efektifAlis;
                        kur.efektifSatis = degerler.satirlar[i].efektifSatis;
                        kur.serbestAlis = degerler.satirlar[i].serbestAlis;
                        kur.serbestSatis = degerler.satirlar[i].serbestSatis;

                        liste.Ekle(kur);
                    }
                }
            }
            string anahtar = form.kurTarih.ToString();

            form.kurTarih.AddDays(-1);
            degerler = Oku(null, new Kullanici(), form);
            if (degerler.satirlar != null)
            {
                anahtar += ";" + degerler.kurTarih.ToString();
                for (int i = 0; i < degerler.satirlar.Length; i++)
                {
                    foreach (KurDegerleri kur in liste.objeler)
                    {
                        if (kur.paraBirimi.kisaltma == degerler.satirlar[i].paraBirim.kisaltma)
                        {
                            kur.oncekiSatis = degerler.satirlar[i].efektifSatis;
                            break;
                        }
                    }
                }
            }
            liste.sonuc.anahtar = anahtar;
            return liste;
        }

        internal static Sonuc SerbestKurGuncelle(VeriErisim veriErisim, Kullanici kullanan, MerkezBankasiKurSinif form)
        {
            Sonuc s = new Sonuc(true, "", "");

            if (form.kurTarih.Oku() > DateTime.Now)
            {
                s.islemSonuc = false;
                s.hataStr = "Lütfen ileriye dönük bir tarih girmeyiniz.";
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHRCMERKEZBANKKUR tabHRCMERKEZBANKKUR = new TABHRCMERKEZBANKKUR(ve);

            ve.BeginTransaction();

            for (int i = 0; i < form.satirlar.Length && s.islemSonuc; i++)
            {
                s = tabHRCMERKEZBANKKUR.Guncelle(form, i);
            }

            if (s.islemSonuc)
            {
                ve.Commit();
                s.bilgiStr = "Serbest Kur Bilgileri başarıyla güncellendi.";
            }
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }

        private class AB_KurYapi
        {
            public string isoA3Code { get; set; }
            public decimal value { get; set; }
        }

        private class ECB_KurDetay
        {
            public decimal TRY;
            public decimal USD;
            public decimal GBP;
        }
        private class ECB_KurYapi
        {
            public ECB_KurDetay rates { get; set; } = new ECB_KurDetay();
            public string date { get; set; }
        }
    }
}
