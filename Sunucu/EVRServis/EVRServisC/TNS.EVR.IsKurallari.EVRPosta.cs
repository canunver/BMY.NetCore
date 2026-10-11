using System;
using System.Collections.Generic;
using System.Text;
using TNS.KYM;
using EVRServisI;
using TNS.Veri;
using System.Threading;
using TNS.EVR.IsKurallari.Veri;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Evrak posta gönderme işlemleriyle ilgili yordamlarını tutan sınıf. 
    /// </summary>
    public class EVRPosta
    {
        /// <summary>
        /// Parametre olarak verilen bilgilere göre listelenen evrakları ilgili kullanıcılara e-posta gönderen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="evrEvrakNo">Evrak evrEvrakNo bilgisini içeren nesne</param>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisini içeren nesne</param>
        /// <param name="bulunduguYer">Evrak'ın bulunduğu yer bilgisini içeren nesne</param>
        /// <param name="postaTcKimlikNo">Posta gönderilecek kullanıcının TcKimlikNo bilgisi içeren nesne</param>
        /// <param name="postaBirim">Posta gönderilecek birim bilgisi içeren nense</param>
        /// <param name="postaTipi">Posta işlem tipini içeren nesne. 1 : Havale, 2 : Haberci</param>
        public static void YeniEvrakGeldiPostasiHazirla(VeriErisim veriErisim, Kullanici kullanan, string evrEvrakNo, string prEvrakNo, string postaTcKimlikNo, string postaBirim, int postaTipi)
        {
            if (!string.IsNullOrWhiteSpace(postaTcKimlikNo))
            {
                EvrakSinif bilgi = new EvrakSinif();
                //Evrak bilgilerini al                
                bilgi.evrEvrakNo = evrEvrakNo;
                bilgi.prEvrakNo = prEvrakNo;
                ObjectArray evraklar = EVREvrak.Listele(veriErisim, kullanan, bilgi);

                if (evraklar.objeler.Count > 0)
                    YeniEvrakGeldiPostasiHazirla(veriErisim, kullanan, (EvrakSinif)evraklar[0], postaTcKimlikNo, postaBirim, postaTipi);
                else
                    OrtakFonksiyonlar.HataStrYaz("{73D749DB-9203-497C-A87A-C82EEC241662} - YeniEvrakGeldiPostasiHazirla; postaTcKimlikNo: " + postaTcKimlikNo + ", postaBirim: " + postaBirim + ", postaTipi:" + postaTipi + ", evrEvrakNo:" + evrEvrakNo);
            }
            else
                OrtakFonksiyonlar.HataStrYaz("{A8E595AA-2744-4886-8930-443D8C4AD796} - YeniEvrakGeldiPostasiHazirla; postaTcKimlikNo: " + postaTcKimlikNo + ", postaBirim: " + postaBirim + ", postaTipi:" + postaTipi + ", evrEvrakNo:" + evrEvrakNo);
        }

        public static void YeniEvrakGeldiPostasiHazirla(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif evrak, string postaTcKimlikNo, string postaBirim, int postaTipi)
        {
            try
            {
                OrtakFonksiyonlar.HataStrYaz("{6E8C571E-EE91-4879-9024-17531B2C116B} - YeniEvrakGeldiPostasiHazirla; postaTcKimlikNo: " + postaTcKimlikNo + ", postaBirim: " + postaBirim + ", postaTipi:" + postaTipi + ", evrEvrakNo:" + evrak.evrEvrakNo);

                if (!string.IsNullOrWhiteSpace(postaTcKimlikNo))
                {
                    bool ePostaGonderilsin = EVREvrakAyarlar.Oku(veriErisim, postaTcKimlikNo, ENUMEvrakAyarlarAnahtar.GELENEVRAKHAVALEPOSTASIAL).deger == "1" ? true : false;
                    if (ePostaGonderilsin)
                    {
                        string kime = "";
                        string adSoyad = "";
                        if (postaTcKimlikNo != "")
                            EpostaAdresiBulKisi(postaTcKimlikNo, ref kime, ref adSoyad);
                        if (postaBirim != "")
                            EpostaAdresiBulBirim(veriErisim, postaBirim, ref kime, ref adSoyad);

                        string[] kimeIc = kime.Split(';');

                        OrtakFonksiyonlar.HataStrYaz("{2D77CFF3-BBA0-46DB-BF25-A3CAA01CE904} - YeniEvrakGeldiPostasiHazirla; kime: " + kime);


                        if (kimeIc.Length > 1)//Çok adrese gidiyor ise 
                            adSoyad = EvrakI.EVRI115;

                        for (int i = 0; i < kimeIc.Length; i++)
                        {
                            if (kimeIc[i].Trim() != "")
                                YeniEvrakGeldiPostasiGonder(kimeIc[i].Trim(), adSoyad, evrak, postaTipi);
                        }
                    }
                    else
                        OrtakFonksiyonlar.HataStrYaz("{34CAC767-2154-44FD-9C0A-1068CDEC795D} - YeniEvrakGeldiPostasiHazirla; postaTcKimlikNo: " + postaTcKimlikNo + ", postaBirim: " + postaBirim + ", postaTipi:" + postaTipi + ", evrEvrakNo:" + evrak.evrEvrakNo);

                }

            }
            catch (ThreadAbortException ex)
            {
                OrtakFonksiyonlar.HataStrYaz("{C92DDB4D-71E4-40A7-81F8-FD00D214B634} - Mail Gönderme Hatası,  Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre listelenen evrakları ilgili kullanıcılara e-posta gönderen yordam.
        /// </summary>
        /// <param name="kime">E-posta gönderielecek kullanıcının e-posta adresi</param>
        /// <param name="adSoyad">E-posta gönderielecek kullanıcının adı ve soyadı bilgisi</param>
        /// <param name="evrak">Evrak bilgisini içeren nesne</param>
        /// <param name="postaTipi">Posta işlem tipini içeren nesne. 1 : Havale, 2 : Haberci</param>
        public static void YeniEvrakGeldiPostasiGonder(string kime, string adSoyad, EvrakSinif evrak, int postaTipi)
        {

            OrtakFonksiyonlar.HataStrYaz("YeniEvrakGeldiPostasiGonder");

            string kimden = System.Configuration.ConfigurationManager.AppSettings.Get("SMTPKullanici");
            if (string.IsNullOrEmpty(kimden)) return;

            string yazilimURL = System.Configuration.ConfigurationManager.AppSettings.Get("yazilimURL");

            if (!string.IsNullOrEmpty(yazilimURL))
                yazilimURL = string.Format("<br><a href='{0}'>{1}</a>", yazilimURL, yazilimURL);


            string havaleKonu = EvrakI.EVRI126;
            string haberciKonu = EvrakI.EVRI129;
            string haberciIcerik = EvrakI.EVRI130;
            string postaKonusu = EvrakI.EVRI131;


            string postaIcerik = "<html><head><meta http-equiv='Content-Type' content='text/html; charset=windows-1254'> <style type='text/css'> body { font-family: 'Tahoma' , 'sans-serif'; } p.MsoNormal { margin-top: 0cm; margin-right: 0cm; margin-bottom: 10.0pt; margin-left: 0cm; line-height: 115%; font-size: 11.0pt; font-family: 'Calibri' , 'sans-serif'; } </style></head><body> <table border='0' cellpadding='0' cellspacing='0' > <tr> <td style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm;' valign='top' width='240px'> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt;'> <img src='http://uygulama.sgb.gov.tr/app_themes/images/evrakPosta.png' /> </p> </td> <td style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm;' width='370'> <p align='center' class='MsoNormal' style='text-align: left; line-height: normal; margin-bottom: 0pt;'> <b><span style='font-size: 16pt'>{PostaKonu}</span></b></p> </td> </tr> <tr> <td colspan='2' style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm;' valign='top' width='610px'> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt;'> <b>" + string.Format(EvrakI.EVRI116, "{AdSoyad}") + "</b></p> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt;'> <span>{PostaIcerik}</span></p> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt;'> <span>&nbsp; </span> </p> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt;'> <span>" + EvrakI.EVRI117 + "</span></p> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt;'> <b>&nbsp; </b> </p> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt;'> </p>";
            postaIcerik += "<table border='0' cellpadding='0' cellspacing='0' width='610px'>";
            postaIcerik += "<tr> <td style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm' valign='top' width='150'> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span><b>" + EvrakI.EVRI118 + "</b></span></p> </td> <td style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm' valign='top' width='150'> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span>: {EvrakNo}</span></p> </td> <td style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm' valign='top' width='150'> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span><b>" + EvrakI.EVRI119 + "</b></span></p> </td> <td style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm' valign='top' width='150'> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span>: {EvrakTarihi}</span></p> </td> </tr> <tr> <td style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm' valign='top'> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span><b>{Geldiği-Gideceği Yer}</b></span></p> </td> <td colspan='3' style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm' valign='top'> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span>: {GeldigiYer}</span></p> </td> </tr> <tr> <td style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm' valign='top' > <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span><b>" + EvrakI.EVRI121 + "</b></span></p> </td> <td style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm' valign='top' colspan='3' > <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span>: {Konusu}</span></p> </td> <td style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm' valign='top' > <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span>&nbsp; </span> </p> </td> <td style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm' valign='top'> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span>&nbsp; </span> </p> </td> </tr> ";

            if (postaTipi == (int)ENUMPOSTATIP.HAVALE || postaTipi == (int)ENUMPOSTATIP.HABERCI)
                postaIcerik += "<tr> <td style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm' valign='top' > <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span><b>" + EvrakI.EVRI122 + "</b></span></p> </td> <td style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm' valign='top' > <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span>: {GelenEvrakNo}</span></p> </td> <td style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm' valign='top' > <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span><b>" + EvrakI.EVRI123 + "</b></span></p> </td> <td style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm' valign='top' > <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span>: {GelenEvrakTarih}</span></p> </td> </tr>";

            postaIcerik += "<tr> <td style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm' valign='top'> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span><b>" + EvrakI.EVRI124 + "</b></span></p> </td> <td style='padding: 0cm 5.4pt;' valign='top' colspan='3'> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span>: {DosyaKodu}</span></p> </td> </tr>";

            if (postaTipi == (int)ENUMPOSTATIP.HAVALE || postaTipi == (int)ENUMPOSTATIP.HABERCI)
                postaIcerik += "<tr> <td style='padding-bottom: 0cm; padding-left: 5.4pt; padding-right: 5.4pt; padding-top: 0cm' valign='top' > <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span><b>" + EvrakI.EVRI125 + "</b></span></p> </td> <td style='padding: 0cm 5.4pt;' valign='top' colspan='3'> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt'> <span>: {Sure}</span></p> </td> </tr>";

            postaIcerik += "</table> <p class='MsoNormal' style='line-height: normal; margin-bottom: 0pt;'> " + yazilimURL + " </p> </td> </tr> </table></body></html>";


            string havaleIcerik = "";
            if (postaTipi == (int)ENUMPOSTATIP.HAVALEGIDEN)
                havaleIcerik = string.Format(EvrakI.EVRI128, "{HavaleTarihi}");
            else if (postaTipi == (int)ENUMPOSTATIP.IMZA)
                havaleIcerik = string.Format(EvrakI.EVRI178, "{HavaleTarihi}");
            else
                havaleIcerik = string.Format(EvrakI.EVRI127, "{HavaleTarihi}", "{Gonderim}");


            int evrakNo = 0;
            TNSDateTime evrakTarih = new TNSDateTime();
            string geldigiYer = "";
            string konusu = "";
            string gelenEvrakNo = "";
            TNSDateTime gelenEvrakTarih = new TNSDateTime();
            TNSDateTime havaleTarih = new TNSDateTime();
            string dosyaKodu = "";
            string gonderim = "";
            string sure = "";
            evrakNo = evrak.evrakNo;
            evrakTarih = evrak.evrakTarih;
            geldigiYer = evrak.geldigiYer;
            konusu = evrak.konu;
            gelenEvrakNo = evrak.gelenEvrakNo;
            gelenEvrakTarih = evrak.gelenEvrakTarih;
            havaleTarih = evrak.hareket.islemTarih;
            sure = evrak.cevapSuresi.ToString();
            if (OrtakFonksiyonlar.ConvertToInt(sure, 0) == 0)
                sure = "-";
            else
                sure += " " + EvrakI.EVRI132;

            if (evrak.sicil.asil == 1)
                gonderim = EvrakI.EVRI133;
            else
                gonderim = EvrakI.EVRI134;

            if (evrak.dosyaKodlari.Count > 0)
                dosyaKodu = ((DosyaPlani)evrak.dosyaKodlari[0]).kod + "-" + ((DosyaPlani)evrak.dosyaKodlari[0]).ad;

            /////////////////////////////////////////////////////////////////////////////////////////////
            if (postaTipi == (int)ENUMPOSTATIP.HAVALE || postaTipi == (int)ENUMPOSTATIP.HAVALEGIDEN || postaTipi == (int)ENUMPOSTATIP.IMZA)         //Havale - İmza
            {
                postaIcerik = postaIcerik.Replace("{PostaKonu}", havaleKonu);
                postaIcerik = postaIcerik.Replace("{PostaIcerik}", havaleIcerik);
                postaKonusu = postaKonusu.Replace("{PostaTur}", EvrakI.EVRI135);
            }
            else if (postaTipi == (int)ENUMPOSTATIP.HABERCI)    //Haberci
            {
                postaIcerik = postaIcerik.Replace("{PostaKonu}", haberciKonu);
                postaIcerik = postaIcerik.Replace("{PostaIcerik}", haberciIcerik);
                postaKonusu = postaKonusu.Replace("{PostaTur}", EvrakI.EVRI136);
            }


            postaIcerik = postaIcerik.Replace("{EvrakNo}", evrakNo.ToString());
            postaIcerik = postaIcerik.Replace("{GeldigiYer}", geldigiYer);
            postaIcerik = postaIcerik.Replace("{Konusu}", konusu);
            postaIcerik = postaIcerik.Replace("{DosyaKodu}", dosyaKodu);
            postaIcerik = postaIcerik.Replace("{EvrakTarihi}", evrakTarih.ToString());
            if (postaTipi == (int)ENUMPOSTATIP.HAVALE || postaTipi == (int)ENUMPOSTATIP.HABERCI)
            {
                postaIcerik = postaIcerik.Replace("{Geldiği-Gideceği Yer}", EvrakI.EVRI120);
                postaIcerik = postaIcerik.Replace("{GelenEvrakNo}", gelenEvrakNo);
                postaIcerik = postaIcerik.Replace("{GelenEvrakTarih}", gelenEvrakTarih.ToString());
                postaIcerik = postaIcerik.Replace("{Sure}", sure);
            }
            else
                postaIcerik = postaIcerik.Replace("{Geldiği-Gideceği Yer}", "Gideceği Yer");


            postaIcerik = postaIcerik.Replace("{HavaleTarihi}", havaleTarih.ToString());
            postaIcerik = postaIcerik.Replace("{AdSoyad}", adSoyad);
            postaIcerik = postaIcerik.Replace("{Gonderim}", gonderim);

            postaKonusu = postaKonusu.Replace("{Konusu}", konusu);

            if (kimden.Trim() != "" && kime.Trim() != "")
            {
                if (kime.IndexOf('@') != -1)
                {
                    try
                    {
                        ParameterizedThreadStart paraThread = new ParameterizedThreadStart(MailAtThread1);
                        Thread th = new Thread(paraThread);
                        th.Name = "MailAtThread1";
                        Object obj = new object[] { kimden, kime, postaKonusu, postaIcerik, true, false, new TNSCollection() };
                        th.Start(obj);
                        //OrtakFonksiyonlar.MailAt(kimden, kime, postaKonusu, postaIcerik, true, false, new TNSCollection());
                    }
                    catch (Exception ex)
                    {
                        OrtakFonksiyonlar.HataStrYaz("{7895D27E-C6C2-440C-99D3-77155CE531BB} - Mail Gönderme Hatası; Kimden: " + kimden + ", Kime: " + kime + ", evrakNo:" + evrak.evrakNo + ", evrEvrakNo:" + evrak.evrEvrakNo + ", Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// E-posta gönderilecek kullanıcının e-posta adresini bulan yordam.
        /// </summary>
        /// <param name="tcKimlikNo">E-posta gönderielecek kullanıcının TcKimlikNo bilgisini içeren nesne</param>
        /// <param name="kime">E-posta gönderielecek kullanıcının e-posta adresi</param>
        /// <param name="adSoyad">E-posta gönderielecek kullanıcının adı ve soyadı bilgisi</param>
        public static void EpostaAdresiBulKisi(string tcKimlikNo, ref string kime, ref string adSoyad)
        {
            IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();
            //Kişinin mail adresini al
            Kullanici aramaBilgisi = new Kullanici();
            aramaBilgisi.mernis = tcKimlikNo;
            ObjectArray lVAN = servisKYM.KullaniciAra(aramaBilgisi, 1, 1);
            if (lVAN.sonuc.islemSonuc)
            {
                foreach (Kullanici k in lVAN.objeler)
                {
                    kime = k.emailAdresi;
                    adSoyad = k.ad + " " + k.soyad;
                }
            }
        }

        /// <summary>
        /// E-posta gönderilecek birimin e-posta adresini bulan yordam.
        /// </summary>
        /// <param name="postaBirim">>E-posta gönderielecek posta birim bilgisini içeren nesne</param>
        /// <param name="kime">E-posta gönderielecek kullanıcının e-posta adresi</param>
        /// <param name="adSoyad">E-posta gönderielecek kullanıcının adı ve soyadı bilgisi</param>
        public static void EpostaAdresiBulBirim(VeriErisim veriErisim, string postaBirim, ref string kime, ref string adSoyad)
        {
            //Kişinin mail adresini al
            Birim birim = new Birim();
            birim.kod = postaBirim;
            birim = EVRBirim.Oku(veriErisim, birim);
            kime = birim.ePosta;
            adSoyad = EvrakI.EVRI137;
        }

        /// <summary>
        /// E-Posta Gönderme işi
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="cc">The cc.</param>
        /// <param name="bcc">The BCC.</param>
        /// <param name="konu">The konu.</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <param name="html">if set to <c>true</c> [HTML].</param>
        /// <param name="replyVar">if set to <c>true</c> [reply variable].</param>
        /// <param name="ekDosyaIsimleri">The ek dosya isimleri.</param>
        /// <returns></returns>
        public static string EPostaGonder(Kullanici kullanan, string evrEvrakNo, string from, string to, string cc, string bcc, string konu, string bilgi, bool html, bool replyVar, TNSCollection ekDosyaIsimleri)
        {
            ParameterizedThreadStart paraThread = new ParameterizedThreadStart(MailAtThread2);
            Thread th = new Thread(paraThread);
            th.Name = "MailAtThread2";
            Object obj = new object[] { from, to, cc, bcc, konu, bilgi, true, false, ekDosyaIsimleri };
            th.Start(obj);

            string sonuc = "1";
            //string sonuc = OrtakFonksiyonlar.MailAt(from, to, cc, bcc, konu, bilgi, true, false, ekDosyaIsimleri);

            if (sonuc == "1")
            {
                string dosyalar = "";
                if (ekDosyaIsimleri == null) ekDosyaIsimleri = new TNSCollection();
                foreach (EPostaEki ek in ekDosyaIsimleri)
                {
                    if (dosyalar != "") dosyalar += "; ";
                    dosyalar += ek.ad;
                }

                if (dosyalar == "") dosyalar = "Ekli doküman gönderilmemiş.";
                //Tarihçesini Kaydet
                EvrakTarihce tarihce = new EvrakTarihce();
                tarihce.evrEvrakNo = evrEvrakNo;
                tarihce.islem = "E-Posta Gönderildi. Kime:" + to + "; İlgi: " + cc + "; Gizli İlgi:" + bcc + "; Gönderilen dosyalar:" + dosyalar;

                Sonuc s = EVREvrakTarihce.Kaydet(null, kullanan, tarihce);
            }
            return sonuc;
        }


        public static void HaberciMailGonder(VeriErisim veriErisim, EvrakSinif evrak)
        {
            try
            {
                ObjectArray haberciList = EVRHaberci.Listele(veriErisim, null, null, evrak.evrakBirimKodu, true);
                ObjectArray tcKimilikList = HaberciKontrol(evrak, haberciList);
                if (tcKimilikList.ObjeSayisi > 0)
                {
                    string kime = "";
                    string adSoyad = "";
                    foreach (string tc in tcKimilikList.objeler)
                    {
                        if (!string.IsNullOrEmpty(tc))
                        {
                            bool ePostaGonderilsin = EVREvrakAyarlar.Oku(veriErisim, tc, ENUMEvrakAyarlarAnahtar.GELENEVRAKHAVALEPOSTASIAL).deger == "1" ? true : false;
                            if (ePostaGonderilsin)
                            {
                                EVRPosta.EpostaAdresiBulKisi(tc, ref kime, ref adSoyad);
                                EVRPosta.YeniEvrakGeldiPostasiGonder(kime, adSoyad, evrak, (int)ENUMPOSTATIP.HABERCI);
                            }
                        }
                    }
                }

            }
            catch (ThreadAbortException ex)
            {
                OrtakFonksiyonlar.HataStrYaz("{3306C153-C8B5-4609-B466-0CA825C4E44D} - Mail Gönderme Hatası,  Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, haberci listesinin kontrolünü yaparak kullanıcı TcKimlikNo bilgilerini getiren yordam.
        /// </summary>
        /// <param name="evrak">Evrak bilgisini içeren nesne</param>
        /// <param name="haberciList">Haberci listesini içeren nesne</param>
        /// <returns>Tckimlik numaraları string nesne koleksiyonu olarak döndürülür.</returns>
        private static ObjectArray HaberciKontrol(EvrakSinif evrak, ObjectArray haberciList)
        {
            TNSDateTime bugun = new TNSDateTime(DateTime.Now.ToShortDateString());

            bool evrakVar;
            ObjectArray tcKimilikList = new ObjectArray();

            foreach (Haberci h in haberciList.objeler)
            {
                evrakVar = false;
                if (bugun.CompareTo(h.baslangicTarih) >= 0 && (bugun.CompareTo(h.bitisTarih) <= 0 || h.bitisTarih.isNull))
                {
                    if (!tcKimilikList.objeler.Contains(h.kullanici))
                    {
                        foreach (HaberciDetay d in h.detaylar)
                        {
                            switch (d.alan)
                            {
                                case (int)ENUMHaberciAlan.GELDIGIYER:
                                    switch (d.islec)
                                    {
                                        case (int)ENUMHaberciIslec.ESIT:
                                            if (evrak.geldigiYer.ToUpper() == d.deger.ToUpper()) { evrakVar = true; }
                                            break;
                                        case (int)ENUMHaberciIslec.ILEBASLAYAN:
                                            if (evrak.geldigiYer.ToUpper().StartsWith(d.deger.ToUpper())) { evrakVar = true; }
                                            break;
                                        case (int)ENUMHaberciIslec.ICINDEGECEN:
                                            if (evrak.geldigiYer.IndexOf(d.deger.ToUpper()) > -1) { evrakVar = true; }
                                            break;
                                    }
                                    break;
                                case (int)ENUMHaberciAlan.KONUSU:
                                    switch (d.islec)
                                    {
                                        case (int)ENUMHaberciIslec.ESIT:
                                            if (evrak.konu.ToUpper() == d.deger.ToUpper()) { evrakVar = true; }
                                            break;
                                        case (int)ENUMHaberciIslec.ILEBASLAYAN:
                                            if (evrak.konu.ToUpper().StartsWith(d.deger.ToUpper())) { evrakVar = true; }
                                            break;
                                        case (int)ENUMHaberciIslec.ICINDEGECEN:
                                            if (evrak.konu.ToUpper().IndexOf(d.deger.ToUpper()) > -1) { evrakVar = true; }
                                            break;
                                    }
                                    break;
                                case (int)ENUMHaberciAlan.GELISSEKLI:
                                    switch (d.islec)
                                    {
                                        case (int)ENUMHaberciIslec.ESIT:
                                            if (evrak.gelisSekli.ToString() == d.deger) { evrakVar = true; }
                                            break;
                                    }
                                    break;
                                case (int)ENUMHaberciAlan.YAZIDURUMU:
                                    switch (d.islec)
                                    {
                                        case (int)ENUMHaberciIslec.ESIT:
                                            if (evrak.yaziDurum.ToString() == d.deger) { evrakVar = true; }
                                            break;
                                    }
                                    break;
                                case (int)ENUMHaberciAlan.GIZLILIKDERECESI:
                                    switch (d.islec)
                                    {
                                        case (int)ENUMHaberciIslec.ESIT:
                                            if (evrak.gizlilik.ToString() == d.deger) { evrakVar = true; }
                                            break;
                                    }
                                    break;
                                case (int)ENUMHaberciAlan.SURELI:
                                    switch (d.islec)
                                    {
                                        case (int)ENUMHaberciIslec.ESIT:
                                            if (evrak.cevapSuresi.ToString() == d.deger) { evrakVar = true; }
                                            break;
                                    }
                                    break;
                                case (int)ENUMHaberciAlan.DOSYAKODU:
                                    foreach (DosyaPlani dp in evrak.dosyaKodlari)
                                        if (!string.IsNullOrEmpty(dp.kod) && d.deger.IndexOf(dp.kod) > -1) { evrakVar = true; break; }
                                    break;
                            }

                            if (evrakVar) { break; }
                        }
                    }
                    if (evrakVar) { tcKimilikList.Ekle(h.kullanici); }

                }
            }
            return tcKimilikList;
        }


        public static void MailAtThread1(object obj)
        {

            try
            {
                Object[] liste = (Object[])obj;

                string kimden = (string)liste[0];
                string kime = (string)liste[1];
                string postaKonusu = (string)liste[2];
                string postaIcerik = (string)liste[3];
                bool html = (bool)liste[4];
                bool replyVar = (bool)liste[5];
                TNSCollection ekDosyaIsimleri = (TNSCollection)liste[6];

                OrtakFonksiyonlar.HataStrYaz("{AF9E89C2-1545-4BD1-B15F-2FDE483B06CC} - MailAtThread1 posta gönderiliyor");

                OrtakFonksiyonlar.MailAt(kimden, kime, postaKonusu, postaIcerik, html, replyVar, ekDosyaIsimleri);
            }
            catch (ThreadAbortException ex)
            {
                OrtakFonksiyonlar.HataStrYaz("{AF9E89C2-1545-4BD1-B15F-2FDE483B06CC} - MailAtThread1 Gönderme Hatası,  Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

        }

        public static void MailAtThread2(object obj)
        {
            try
            {
                Object[] liste = (Object[])obj;

                string from = (string)liste[0];
                string to = (string)liste[1];
                string cc = (string)liste[2];
                string bcc = (string)liste[3];
                string konu = (string)liste[4];
                string bilgi = (string)liste[5];
                bool html = (bool)liste[6];
                bool replyVar = (bool)liste[7];
                TNSCollection ekDosyaIsimleri = (TNSCollection)liste[8];

                string sonuc = OrtakFonksiyonlar.MailAt(from, to, cc, bcc, konu, bilgi, true, false, ekDosyaIsimleri);
            }
            catch (ThreadAbortException ex)
            {
                OrtakFonksiyonlar.HataStrYaz("{ABF67A81-4C26-4A6C-9C18-592FB2C2228A} - MailAtThread2 Gönderme Hatası,  Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

        }
    }
}
