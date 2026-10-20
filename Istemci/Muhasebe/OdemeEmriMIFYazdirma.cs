using System;
using System.Collections;
using System.IO;
using Istemci;
using OrtakClass;
using TNS;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>
    /// Ödeme emri / MİF belge yazdırma (eski ButceMuhasebe/OdemeEmriMIFYazdirma.cs, aynı şablon ve yerleşimle).
    /// Eski koddan farkı: kullanıcı parametreden gelir; dosya burada gönderilmez, üretilen dosyanın yolu "dosya" ile döner
    /// (uç DosyaIslem.DosyaGonder ile gönderir). EBYS'ye gönderimde dosya üretilir ama gönderilmez, sonuç "bilgi" ile döner.
    /// </summary>
    public static class OdemeEmriMIFYazdirma
    {
        [ThreadStatic] private static string uretilenDosya;
        [ThreadStatic] private static string uretilenUzanti;
        [ThreadStatic] private static string evrakBilgi;

        /// <summary>Tek belge (eski Listele: OdemeEmriMIFTekBelgeListele).</summary>
        public static ObjectArray Listele(Kullanici kullanan, IMUHServis servisMUH, int yil, string belgeNo, string yevmiyeNo = "", bool sadeceUstVeri = false)
        {
            OdemeEmriMIFForm f = new OdemeEmriMIFForm();
            f.yil = yil;
            f.belgeNo = belgeNo;
            f.sadeceUstVeri = sadeceUstVeri;

            if (belgeNo.Trim() == "")
                f.YEVYevNo = OrtakFonksiyonlar.ConvertToInt(yevmiyeNo, 0);

            return servisMUH.OdemeEmriMIFTekBelgeListele(kullanan, f);
        }

        /// <summary>
        /// Belge çıktısı (eski Yazdir). excelYazYer boşsa geçici dosyaya yazılır ve yolu "dosya" ile döner (uzanti: dosya türü);
        /// excelYazYer verilmişse oraya yazılır (toplu yazdırma), dosya null kalır. Hata varsa metni döner.
        /// evrakaGonder: eski "EBYS'ye Gönder"; sonuç metni "bilgi" ile döner.
        /// </summary>
        public static string Yazdir(Kullanici kullanan, int yil, string belgeNo, string excelYazYer, ENUMBelgeTuru belgeTur, out string dosya, out string uzanti, out string bilgi,
            bool evrakaGonder = false, bool omk = false, bool prgTertip = false)
        {
            dosya = null; uzanti = null; bilgi = null;
            uretilenDosya = null; uretilenUzanti = null; evrakBilgi = null;

            //belge tur 0 geldiği için rapor üretmiyor.
            if ((int)belgeTur == 0)
                belgeTur = ENUMBelgeTuru.TANIMSIZ;

            string hataStr = "";

            IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

            ObjectArray fler = Listele(kullanan, servisMUH, yil, belgeNo);

            if (fler.sonuc.islemSonuc)
            {
                if (fler.objeler.Count == 0)
                    hataStr = string.Format("Görmek istediğiniz {0} nolu belge bulunamadı", belgeNo);
                else
                {
                    OdemeEmriMIFForm form = (OdemeEmriMIFForm)fler.objeler[0];
                    if (form.belgeNo != "")
                    {
                        if (belgeTur != ENUMBelgeTuru.TANIMSIZ)
                        {
                            Sonuc sonuc = servisMUH.BelgeTurDegistir(kullanan, ((OdemeEmriMIFForm)fler.objeler[0]), (int)belgeTur);
                            ((OdemeEmriMIFForm)fler.objeler[0]).belgeTur = belgeTur;
                        }
                        hataStr = ExceleYaz(kullanan, servisMUH, (OdemeEmriMIFForm)fler.objeler[0], excelYazYer, evrakaGonder, omk, prgTertip);
                    }
                    else
                        hataStr = "İstenilen belge bulunamadığı için gösterilemiyor.";
                }
            }
            else
                hataStr = fler.sonuc.hataStr;

            dosya = uretilenDosya; uzanti = uretilenUzanti; bilgi = evrakBilgi;
            return hataStr ?? "";
        }

        /// <summary>Eski GenelIslemler.KurusBul.</summary>
        private static int KurusBul(double para)
        {
            return (int)(((long)Math.Round(para * 100)) % 100);
        }

        /// <summary>Eski ButceMuhasebeGenel.DondurParaBirimi.</summary>
        private static ParaBirimi DondurParaBirimi(int paraBirimiKod)
        {
            TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
            ObjectArray paralar = servisHRC.ParaBirimiListele(new Kullanici(), new ParaBirimi());
            foreach (ParaBirimi bilgi in paralar.objeler)
            {
                if (bilgi.kod == paraBirimiKod)
                    return bilgi;
            }
            return new ParaBirimi();
        }

        /// <summary>Eski OrtakClass.DosyaIslem.SablonAdOl: imzalı (eimza) şablon varsa o, yoksa kurum şablonu; ikisi de yoksa uygulama klasörü.</summary>
        private static string SablonAdOl(string raporYol, string modulYol, string sablonAd, bool imza)
        {
            string dosyaAd;
            if (imza)
            {
                dosyaAd = Path.Combine(raporYol, modulYol, "eimza" + sablonAd);
                if (File.Exists(dosyaAd)) return dosyaAd;
            }
            string dosyaOrjAd = Path.Combine(raporYol, modulYol, sablonAd);
            if (File.Exists(dosyaOrjAd)) return dosyaOrjAd;
            dosyaAd = Path.Combine(AppContext.BaseDirectory, "RaporSablon", modulYol, sablonAd);
            if (File.Exists(dosyaAd)) return dosyaAd;
            return dosyaOrjAd;
        }

        private static string ExceleYaz(Kullanici kullanan, IMUHServis servisMUH, OdemeEmriMIFForm form, string excelYazYer, bool evrakaGonder, bool omk = false, bool prgTertip = false)
        {
            TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int tSatir = 0;
            bool tutarTekHucrede = false;
            bool satirAc = true;
            bool parafBoslukBirak = false;

            //GenelIslemlerIstemci.VarsayilanKurumBul();
            string sablonAd = "";

            string dosyaAdiEk = "";
            if (prgTertip)
                dosyaAdiEk = "PEB";

            if (form.kurum == "25.75" || form.kurum == "11.75")
                return ExceleYazUA(kullanan, servisMUH, form, excelYazYer, evrakaGonder, omk);
            else if (form.belgeTur == ENUMBelgeTuru.ODEMEEMRI)
            {
                if (TNS.HRC.Arac.CumhurbaskanligiMi())
                    sablonAd = "MUH-OdemeEmriBelgeCumhur" + dosyaAdiEk + ".XLT";
                else if (form.kurum == "06")
                    sablonAd = "MUH-OdemeEmriBelgeSayistay.XLT";
                else if (form.kurum.Replace(".", "") == "1399")
                {
                    sablonAd = "MUH-OdemeEmriBelge1399.XLTX";
                    tutarTekHucrede = true;
                    parafBoslukBirak = true;

                    int imzaKod = OrtakFonksiyonlar.ConvertToInt(GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEIMZATANIMKOD"), 0);
                    ImzaTanim imzaForm = new ImzaTanim();
                    if (imzaKod > 0)
                        imzaForm.kod = imzaKod;
                    else
                        imzaForm = servisMUH.ImzaTanimOku(kullanan, new ImzaTanim() { belgeTur = ENUMBelgeTuru.ODEMEEMRI });

                    if (imzaForm.kod > 0)
                    {
                        Sonuc sonuc = servisMUH.ImzaTanimGecerliImzaYap(kullanan, imzaForm.kod, "999");
                    }
                }
                else if (form.kurum.Replace(".", "") == "460604")
                {
                    sablonAd = "MUH-OdemeEmriBelgeEGO.XLT";
                    tutarTekHucrede = true;
                }
                else
                    sablonAd = "MUH-OdemeEmriBelge" + dosyaAdiEk + ".XLT";
            }
            else if (form.belgeTur == ENUMBelgeTuru.MIF)
            {
                if (TNS.HRC.Arac.CumhurbaskanligiMi())
                    sablonAd = "MUH-MIFBelgeCumhur" + dosyaAdiEk + ".XLT";
                else if (form.kurum == "06")
                    sablonAd = "MUH-MIFBelgeSayistay.XLT";
                else if (form.kurum.Replace(".", "") == "1399")
                {
                    sablonAd = "MUH-MIFBelge1399.XLTX";
                    tutarTekHucrede = true;
                    parafBoslukBirak = true;

                    int imzaKod = OrtakFonksiyonlar.ConvertToInt(GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEIMZATANIMKOD"), 0);
                    ImzaTanim imzaForm = new ImzaTanim();
                    if (imzaKod > 0)
                        imzaForm.kod = imzaKod;
                    else
                        imzaForm = servisMUH.ImzaTanimOku(kullanan, new ImzaTanim() { belgeTur = ENUMBelgeTuru.ODEMEEMRI });

                    if (imzaForm.kod > 0)
                    {
                        Sonuc sonuc = servisMUH.ImzaTanimGecerliImzaYap(kullanan, imzaForm.kod, "999");
                    }
                }
                else if (form.kurum.Replace(".", "") == "460604")
                {
                    sablonAd = "MUH-MIFBelgeEGO.XLT";
                    tutarTekHucrede = true;
                }
                else
                    sablonAd = "MUH-MIFBelge" + dosyaAdiEk + ".XLT";
            }

            bool gavurYaz = false;
            if (GenelIslemlerIstemci.VarsayilanKurumBulAd().StartsWith("VAKIFLAR İD"))
            {
                gavurYaz = true;
                tutarTekHucrede = true;
            }

            string sonucDosyaAd = "";
            bool gonder = false;
            if (excelYazYer == "")
            {
                sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
                gonder = true;
            }
            else
                sonucDosyaAd = excelYazYer;

            XLS.DosyaAc(SablonAdOl(Ayar.Oku("RaporSablonYol"), "MUH", sablonAd, false), sonucDosyaAd);

            string[] dizi;
            dizi = form.kurum.Split('.');
            if (dizi.Length > 0) XLS.HucreAdBulYaz("txtKurum1", dizi[0]);
            if (dizi.Length > 1) XLS.HucreAdBulYaz("txtKurum2", dizi[1]);
            if (dizi.Length > 2) XLS.HucreAdBulYaz("txtKurum3", dizi[2]);
            XLS.HucreAdBulYaz("txtBelgeNo", form.belgeNo);
            XLS.HucreAdBulYaz("txtYil", form.yil);
            XLS.HucreAdBulYaz("txtMuhasebe", form.muhasebe);
            XLS.HucreAdBulYaz("txtMuhasebeAd", form.muhasebeAd);
            XLS.HucreAdBulYaz("txtKurumAdBaslik", form.kurumAd);
            XLS.HucreAdBulYaz("txtKurumAd", form.kurumAd);
            XLS.HucreAdBulYaz("txtBirim", form.birim);
            XLS.HucreAdBulYaz("txtBirimAd", form.birimAd);
            XLS.HucreAdBulYaz("txtIlgiliAd", form.ilgiliAd);
            XLS.HucreAdBulYaz("txtIlgiliNo", form.ilgiliNo);
            XLS.HucreAdBulYaz("txtIlgiliBankaAd", form.ilgiliBankaAd);
            XLS.HucreAdBulYaz("txtIlgiliBankaNo", form.ilgiliBankaNo);
            XLS.HucreAdBulYaz("txtIlgiliVD", form.ilgiliVD);
            XLS.HucreAdBulYaz("txtCekNo", form.YEVCekNo);
            XLS.HucreAdBulYaz("IOEKim", form.IOEKim);
            XLS.HucreAdBulYaz("IOETarih", form.IOETarih.ToString());
            XLS.HucreAdBulYaz("IOEYevNo", form.IOEYevNo);
            XLS.HucreAdBulYaz("AVKTarih", form.AVKTarih.ToString());
            XLS.HucreAdBulYaz("AVKYevNo", form.AVKYevNo);
            XLS.HucreAdBulYaz("AVKTutar", form.AVKTutar.ToString("#,###.00"));

            XLS.HucreAdBulYaz("txtTaahhutBelgeNo", form.projeNo);

            XLS.HucreAdBulYaz("txtAciklamaEkler", form.aciklama.Replace('\r', ' '));

            XLS.HucreAdBulYaz("txtOEBTur", form.OEBTur);
            XLS.HucreAdBulYaz("txtOEBTarih", form.OEBTarih.ToString());
            XLS.HucreAdBulYaz("txtOEBNo", form.OEBNo);
            XLS.HucreAdBulYaz("txtOEBTutar", Math.Truncate(form.OEBTutar));
            XLS.HucreAdBulYaz("txtOEBTutarYKR", KurusBul(form.OEBTutar));

            if (form.YEVYevTarih.isNull)
            {
                XLS.HucreAdBulYaz("txtYevmiyeTarih", form.islemTarih.ToString());
            }
            else
            {
                XLS.HucreAdBulYaz("txtYevmiyeTarih", form.YEVYevTarih.ToString());
                XLS.HucreAdBulYaz("txtYevmiyeNo", form.YEVYevNo);
            }

            XLS.HucreAdBulYaz("txtVerileNo", form.YEVVerNo);
            XLS.HucreAdBulYaz("txtCekNo", form.YEVCekNo);

            //txtIOETarih.Text=form.IOETarih.ToString();
            //txtIOEYevNo.Text=form.IOEYevNo;
            //txtIOEKim.Text=form.IOEKim;
            //txtAVKTarih.Text=form.AVKTarih.ToString();
            //txtAVKYevNo.Text=form.AVKYevNo;
            //txtAVKTutar.Text=form.AVKTutar.ToString();

            XLS.HucreAdAdresCoz("txtHesapNo", ref satir, ref sutun);
            if (satir == -1)
                XLS.HucreAdAdresCoz("txtHesapNoTam", ref satir, ref sutun);

            string altHesapAciklamaYaz = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/ODEMIFALTHESAPYAZMATURU");

            double borcToplami = 0;
            double alacakToplami = 0;
            double butceGiderToplami = 0;
            double ozelGiderToplami = 0;
            double kesintiToplami = 0;
            double odenmesiToplami = 0;
            double avansToplami = 0;
            double emanetToplami = 0;
            Hashtable yeniOdenecek = new Hashtable();

            Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);
            string hesapYapi = (string)t.Kirilimlar(0);

            ParaIsmi paraIsmi = TNS.HRC.Arac.ParaIsmiBul(servisHRC, kullanan);

            //Faaliyet bazında girilen Ödeme Emri belgeleri yazdırma sırasında faaliyetleri olmadığı için aynı satırların tekrarı olarak gözüküyor. Onun için aynı saturlar tek satıra toplanıyor
            foreach (OdemeEmriMIFDetay detay in form.detay.objeler)
            {
                if (TNS.DEG.Arac.VarsayilanKurumBul().Replace(".", "") == "460604" ||
                    TNS.DEG.Arac.VarsayilanKurumBul().Replace(".", "") == "4035" ||
                    TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/ODEMIFYAZDIRSATIRTEKRARLI") == "1")// EGO satırlar aynı olsa bile farklı yazılmasını istiyor
                    break;

                if (detay.hesapKod == "XXX") continue;

                string tertip = TNS.MUH.Arac.TertipStrOl(detay);
                string kontrol = detay.hesapKod + tertip;

                foreach (OdemeEmriMIFDetay kd in form.detay.objeler)
                {
                    if (kd.hesapKod == "XXX") continue;

                    if (kd.satirNo > detay.satirNo && ((kd.borc > 0 && detay.borc > 0) || (kd.alacak > 0 && detay.alacak > 0)))//iki tarafda aynı tutar türünde olmalı
                    {
                        string tertipKd = TNS.MUH.Arac.TertipStrOl(kd);
                        string kontrolKd = kd.hesapKod + tertipKd;
                        if (kontrolKd == kontrol)
                        {
                            detay.borc = Math.Round(detay.borc + kd.borc, 4);
                            detay.alacak = Math.Round(detay.alacak + kd.alacak, 4);
                            kd.hesapKod = "XXX";
                        }
                    }
                }
            }

            int sablonSatir = 0;
            int sablonSutun = 0;
            XLS.HucreAdAdresCoz("txtSablon", ref sablonSatir, ref sablonSutun);

            int acilanSatirSay = 0;
            ParaBirimi pb = DondurParaBirimi(1);
            string kurum = form.kurum.Replace(".", "");

            foreach (OdemeEmriMIFDetay detay in form.detay.objeler)
            {
                if (detay.hesapKod == "XXX") continue;

                dizi.Initialize();

                if (detay.borc == 0 && detay.alacak == 0 && detay.dovizBorc == 0 && detay.dovizAlacak == 0)
                    continue;

                //Satır açılıyor
                //******************************************************************
                if (satirAc)
                {
                    acilanSatirSay++;
                    XLS.SatirAc(satir + 1, 1);
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir + 1, 0);
                    SatirDuzenle(XLS, form, satir);
                }
                //******************************************************************

                string hKod = detay.hesapKod;
                if (hKod.Length != hesapYapi.Length)
                {
                    hKod = hKod.Replace(".", "");
                    hKod = hKod.PadRight(hesapYapi.Replace(".", "").Length, '0');
                    hKod = OrtakFonksiyonlar.KodNoktaKoy(hesapYapi, hKod);
                }

                XLS.HucreAdAdresCoz("txtHesapNoTam", ref tSatir, ref sutun);
                if (tSatir > 0)
                {
                    XLS.HucreDegerYaz(satir, sutun, detay.hesapKod);
                    if (TNS.HRC.Arac.CumhurbaskanligiMi())
                        XLS.HucreBirlestir(satir, sutun, satir, sutun + 5);
                }
                else
                {
                    dizi = hKod.Split('.');
                    if (dizi.Length > 0)
                    {
                        XLS.HucreAdAdresCoz("txtHesapNo", ref tSatir, ref sutun);
                        XLS.HucreDegerYaz(satir, sutun, (string)dizi[0]);
                    }
                    for (int i = 1; i < dizi.Length; i++)
                    {
                        XLS.HucreAdAdresCoz("txtEko" + i.ToString(), ref tSatir, ref sutun);
                        if (sutun > -1)
                            XLS.HucreDegerYaz(satir, sutun, (string)dizi[i]);
                    }
                }

                XLS.HucreAdAdresCoz("txtTertipPEB", ref tSatir, ref sutun);
                if (tSatir > 0)
                {
                    XLS.HucreDegerYaz(satir, sutun, TNS.MUH.Arac.TertipStrOlPrg(detay));
                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 9);
                }
                else
                {
                    string araKod = "";

                    araKod = "txtKur";
                    dizi.Initialize();
                    dizi = detay.kur.Split('.');
                    for (int i = 0; i < dizi.Length; i++)
                    {
                        XLS.HucreAdAdresCoz(araKod + (i + 1).ToString(), ref tSatir, ref sutun);
                        if (sutun > -1)
                            XLS.HucreDegerYaz(satir, sutun, dizi[i]);
                    }

                    araKod = "txtFon";
                    dizi.Initialize();
                    dizi = detay.fon.Split('.');
                    for (int i = 0; i < dizi.Length; i++)
                    {
                        XLS.HucreAdAdresCoz(araKod + (i + 1).ToString(), ref tSatir, ref sutun);
                        if (sutun > -1)
                            XLS.HucreDegerYaz(satir, sutun, dizi[i]);
                    }

                    araKod = "txtFin";
                    dizi.Initialize();
                    dizi = detay.fin.Split('.');
                    for (int i = 0; i < dizi.Length; i++)
                    {
                        XLS.HucreAdAdresCoz(araKod + (i + 1).ToString(), ref tSatir, ref sutun);
                        if (sutun > -1)
                            XLS.HucreDegerYaz(satir, sutun, dizi[i]);
                    }
                }

                //if (hesapKoduTamYaz)
                //{
                //    araKod = "txtEko";
                //    dizi.Initialize();
                //    dizi = detay.eko.Split('.');
                //    for (int i = 0; i < dizi.Length; i++)
                //    {
                //        XLS.HucreAdAdresCoz(araKod + (i + 1).ToString(), ref tSatir, ref sutun);
                //        if (sutun > -1)
                //            XLS.HucreDegerYaz(satir, sutun, dizi[i]);
                //    }
                //}

                if (detay.borc > 0 || detay.dovizBorc > 0)
                {
                    if (tutarTekHucrede)
                    {
                        XLS.HucreAdAdresCoz("txtBorc", ref tSatir, ref sutun);
                        XLS.HucreDegerYaz(satir, sutun, detay.borc);

                        if (detay.dovizBorc > 0)
                        {
                            XLS.HucreAdAdresCoz("txtBorcDoviz", ref tSatir, ref sutun);
                            XLS.HucreDegerYaz(satir, sutun, detay.dovizBorc.ToString("#,###.00") + " " + detay.doviz);
                        }
                    }
                    else
                    {
                        XLS.HucreAdAdresCoz("txtBorc", ref tSatir, ref sutun);
                        XLS.HucreDegerYaz(satir, sutun, Math.Truncate(detay.borc));
                        XLS.HucreAdAdresCoz("txtBorcYKR", ref tSatir, ref sutun);
                        XLS.HucreDegerYaz(satir, sutun, KurusBul(detay.borc));
                    }
                }

                if (detay.alacak > 0 || detay.dovizAlacak > 0)
                {
                    if (tutarTekHucrede)
                    {
                        XLS.HucreAdAdresCoz("txtAlacak", ref tSatir, ref sutun);
                        XLS.HucreDegerYaz(satir, sutun, detay.alacak);

                        if (detay.dovizAlacak > 0)
                        {
                            XLS.HucreAdAdresCoz("txtAlacakDoviz", ref tSatir, ref sutun);
                            XLS.HucreDegerYaz(satir, sutun, detay.dovizAlacak.ToString("#,###.00") + " " + detay.doviz);
                        }
                    }
                    else
                    {
                        XLS.HucreAdAdresCoz("txtAlacak", ref tSatir, ref sutun);
                        XLS.HucreDegerYaz(satir, sutun, Math.Truncate(detay.alacak));
                        XLS.HucreAdAdresCoz("txtAlacakYKR", ref tSatir, ref sutun);
                        XLS.HucreDegerYaz(satir, sutun, KurusBul(detay.alacak));
                    }
                }

                string aciklama = detay.aciklama;
                if (altHesapAciklamaYaz == "1")
                {
                    if (!string.IsNullOrEmpty(detay.altHesapAd)) aciklama = detay.altHesapAd;
                }
                if (altHesapAciklamaYaz == "2")
                {
                    if (!string.IsNullOrEmpty(detay.altHesapAd)) aciklama += " (" + detay.altHesapAd + ")";
                }

                XLS.HucreAdAdresCoz("txtAciklama", ref tSatir, ref sutun);
                XLS.HucreDegerYaz(satir, sutun, aciklama);


                XLS.HucreAdAdresCoz("txtDetayBirim", ref tSatir, ref sutun);
                XLS.HucreDegerYaz(satir, sutun, SubStr(detay.DetayBirim, 0, 3));

                string satirAciklama = detay.altHesapAd;
                if (satirAciklama != "" && detay.satirAciklama != "") satirAciklama += " / ";
                satirAciklama += detay.satirAciklama;

                XLS.HucreAdAdresCoz("txtSatirAciklama", ref tSatir, ref sutun);
                XLS.HucreDegerYaz(satir, sutun, satirAciklama);

                XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(330));
                satir++;

                borcToplami += Math.Round(detay.borc, 2);
                alacakToplami += Math.Round(detay.alacak, 2);

                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = form.yil;
                h.hesapKod = detay.hesapKod;

                h = servisMUH.HesapKodOzellikAl(h);
                if (h != null && detay.kesinti < 1)
                {
                    //Odenecek tutarın kasa hesabından bulunması için 13.01.2017
                    if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.KASAHESABI) && detay.alacak > 0 &&
                        !(TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.AVANS) ||
                        TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.GELIRBUTCESI) ||
                        TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.GIDERBUTCESI) ||
                        TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.OZELGIDER) ||
                        TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.EMANET)))
                        OdenecekEkle(yeniOdenecek, pb.kisaltma, Math.Round(detay.alacak, 2), h, Math.Round(detay.dovizAlacak, 2));

                    if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.KASAHESABI) && form.belgeTur == ENUMBelgeTuru.MIF)//if ((belgeTür==2) && (hesapNo=="103" || hesapNo=="100"))
                        odenmesiToplami += Math.Round(detay.alacak - detay.borc, 2);

                    if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.GIDERBUTCESI))//if(Number(borç)>0 && (hesapNo=="702" || hesapNo=="704"))
                        butceGiderToplami += Math.Round(detay.borc - detay.alacak, 2);//Bütçe Gideri

                    if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.AVANS))//if (Number(borç)>0 && hesapNo=="172")
                        avansToplami += Math.Abs(Math.Round(detay.alacak, 2) - Math.Round(detay.borc, 2));
                }
                if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.OZELGIDER))//if(Number(borç)>0 && ((eko=="01010103" && hesapNo=="603") || (eko=="01010000" && hesapNo=="370")))
                    ozelGiderToplami += Math.Round(detay.borc, 2);//Özel Gider İndirimi

                if (detay.kesinti > 0 && detay.alacak > 0 && !(TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.OZELGIDER) && detay.borc > 0) && !(TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.EMANET) && detay.borc > 0))
                    kesintiToplami += Math.Round(detay.alacak - detay.borc, 2);

                if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.EMANET) && detay.borc > 0)// Emanet hesabı sadece borç ise yapılır 10.09.2013 Melih
                    emanetToplami += Math.Round(detay.borc - detay.alacak, 2);
            }

            if (satirAc)
            {
                int toplamSatir = 26;

                if (parafBoslukBirak) toplamSatir = 18;

                int acilacakSatirSayisi = toplamSatir - acilanSatirSay;
                for (int i = 0; i < acilacakSatirSayisi; i++)
                {
                    XLS.SatirAc(satir + 1, 1);
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir + 1, 0);
                    SatirDuzenle(XLS, form, satir);
                    satir++;
                }
            }

            borcToplami = Math.Round(borcToplami, 2);
            alacakToplami = Math.Round(alacakToplami, 2);

            odenmesiToplami = (Math.Round(butceGiderToplami, 2) + Math.Round(ozelGiderToplami, 2) + Math.Round(avansToplami, 2) + Math.Round(emanetToplami, 2)) - Math.Round(kesintiToplami, 2);

            butceGiderToplami = Math.Round(butceGiderToplami, 2);
            odenmesiToplami = Math.Round(odenmesiToplami, 2);
            ozelGiderToplami = Math.Round(ozelGiderToplami, 2);
            emanetToplami = Math.Round(emanetToplami, 2);
            avansToplami = Math.Round(avansToplami, 2);
            kesintiToplami = Math.Round(kesintiToplami, 2);

            if (tutarTekHucrede)
            {
                XLS.HucreAdBulYaz("txtToplamBorc", borcToplami);
                XLS.HucreAdBulYaz("txtToplamAlacak", alacakToplami);
                XLS.HucreAdBulYaz("txtButceGideriBorcToplami", butceGiderToplami);
                XLS.HucreAdBulYaz("txtButceGideri", butceGiderToplami);

                XLS.HucreAdBulYaz("txtOzelGider", ozelGiderToplami);
                XLS.HucreAdBulYaz("txtKesinti", kesintiToplami);
                XLS.HucreAdBulYaz("txtKesinti", kesintiToplami);

                //Ödenmesi gereken tutarın, Kasa hesabından üretilenden yazmak için 13.01.2017
                //*****************************************************************************
                if (kurum == "1399" || TNS.HRC.Arac.CumhurbaskanligiMi() || gavurYaz)//|| kurum == "30"
                {
                    XLS.HucreAdBulYaz("txtOdenmesi", OdenmesiBul(yeniOdenecek));
                }
                else
                    XLS.HucreAdBulYaz("txtOdenmesi", odenmesiToplami.ToString("#,###.00"));
                //*****************************************************************************
            }
            else
            {
                XLS.HucreAdBulYaz("txtToplamBorc", Math.Truncate(borcToplami));
                XLS.HucreAdBulYaz("txtToplamBorcYKR", KurusBul(borcToplami));
                XLS.HucreAdBulYaz("txtToplamAlacak", Math.Truncate(alacakToplami));
                XLS.HucreAdBulYaz("txtToplamAlacakYKR", KurusBul(alacakToplami));

                XLS.HucreAdBulYaz("txtButceGideriBorcToplami", Math.Truncate(butceGiderToplami));
                XLS.HucreAdBulYaz("txtButceGideriBorcToplamiYKR", KurusBul(butceGiderToplami));

                XLS.HucreAdBulYaz("txtButceGideri", Math.Truncate(butceGiderToplami));
                XLS.HucreAdBulYaz("txtButceGideriYKR", KurusBul(butceGiderToplami));

                XLS.HucreAdBulYaz("txtOzelGider", Math.Truncate(ozelGiderToplami));
                XLS.HucreAdBulYaz("txtOzelGiderYKR", KurusBul(ozelGiderToplami));

                XLS.HucreAdBulYaz("txtKesinti", Math.Truncate(kesintiToplami));
                XLS.HucreAdBulYaz("txtKesintiYKR", KurusBul(kesintiToplami));

                XLS.HucreAdBulYaz("txtKesinti", Math.Truncate(kesintiToplami));
                XLS.HucreAdBulYaz("txtKesintiYKR", KurusBul(kesintiToplami));

                XLS.HucreAdBulYaz("txtOdenmesi", Math.Truncate(odenmesiToplami));
                XLS.HucreAdBulYaz("txtOdenmesiYKR", KurusBul(odenmesiToplami));
            }

            if (butceGiderToplami > 0)
                XLS.HucreAdBulYaz("txtButceGideriYaziyla", OrtakFonksiyonlar.Yaziyla(butceGiderToplami, paraIsmi));


            //Ödenmesi gereken tutarın, Kasa hesabından üretilenden yazmak için 13.01.2017
            //*****************************************************************************
            if (kurum == "1399" || TNS.HRC.Arac.CumhurbaskanligiMi() || gavurYaz)//|| kurum == "30"
                XLS.HucreAdBulYaz("txtOdenmesiYaziyla", OdenmesiBulYaziyla(yeniOdenecek));
            else
                XLS.HucreAdBulYaz("txtOdenmesiYaziyla", OrtakFonksiyonlar.Yaziyla(odenmesiToplami, paraIsmi));
            //*****************************************************************************

            OdenekGondermeYazdirma.ImzaBilgileriniYaz(servisMUH, XLS, kullanan, form.belgeTur, 0, form.birim, form.islemTarih);

            string parola = TNS.DEG.Arac.DegiskenDegerBul(0, "/MUHASEBE/CIKTIPAROLA");
            if (!string.IsNullOrWhiteSpace(parola) && !evrakaGonder)
                XLS.KorumayaAl(parola);

            XLS.DosyaSaklaTamYol();

            string ciktiTur = GenelIslemler.ExcelTur();

            if (gonder)
            {
                if (evrakaGonder)
                    return EvrakaGonder(servisMUH, sonucDosyaAd, form, kullanan, omk);
                else
                {
                    uretilenDosya = sonucDosyaAd;
                    uretilenUzanti = ciktiTur;
                }
            }
            return "";
        }

        private static string SubStr(string str, int bas, int uz)
        {
            if (string.IsNullOrWhiteSpace(str)) return "";
            int len = str.Length;
            if (bas >= len) return "";
            if (bas + uz >= len) uz = len - bas;
            return str.Substring(bas, uz);
        }

        private static string OdenmesiBul(Hashtable yeniOdenecek)
        {
            string yazi = "";

            foreach (DictionaryEntry item in yeniOdenecek)
            {
                string kur = (string)item.Key;
                double tutar = (double)item.Value;

                if (yazi != "") yazi += "\n";
                yazi += tutar.ToString("#,###.00") + " " + kur;
            }

            return yazi;
        }

        private static string OdenmesiBulYaziyla(Hashtable yeniOdenecek)
        {
            string yazi = "";

            foreach (DictionaryEntry item in yeniOdenecek)
            {
                string kur = (string)item.Key;
                double tutar = (double)item.Value;
                ParaIsmi pi = null;

                TNS.BKD.ParaBirimi pb = new ParaBirimi();
                pb.kisaltma = kur;
                pi = pb.ParaIsmiBul;

                string yaziyla = OrtakFonksiyonlar.Yaziyla(tutar, pi);

                if (yazi != "") yazi += " ve ";
                yazi += yaziyla;
            }

            return yazi;
        }

        private static void OdenecekEkle(Hashtable yeniOdenecek, string kisaltma, double tutar, HesapPlaniSatir h, double tutarDoviz)
        {
            if (h.dovizKurKod != "")
                HashEkle(yeniOdenecek, h.dovizKurKod, tutarDoviz);
            else
                HashEkle(yeniOdenecek, kisaltma, tutar);
        }

        private static void HashEkle(Hashtable yeniOdenecek, string p, double tutar)
        {
            double tut = OrtakFonksiyonlar.ConvertToDbl(yeniOdenecek[p]);
            yeniOdenecek[p] = tut + tutar;
        }

        private static void SatirDuzenle(Tablo XLS, OdemeEmriMIFForm form, int satir)
        {
            if (form.belgeTur == ENUMBelgeTuru.ODEMEEMRI)
            {
                if (TNS.HRC.Arac.CumhurbaskanligiMi())
                {
                    XLS.HucreBirlestir(satir + 1, 1, satir + 1, 2);
                    XLS.HucreBirlestir(satir + 1, 11, satir + 1, 12);
                    XLS.HucreBirlestir(satir + 1, 20, satir + 1, 21);
                }
                else if (form.kurum == "06")
                {
                    XLS.HucreBirlestir(satir + 1, 1, satir + 1, 2);
                    XLS.HucreBirlestir(satir + 1, 11, satir + 1, 12);
                    XLS.HucreBirlestir(satir + 1, 20, satir + 1, 21);
                }
            }
            else if (form.belgeTur == ENUMBelgeTuru.MIF)
            {
                if (TNS.HRC.Arac.CumhurbaskanligiMi())
                {
                    XLS.HucreBirlestir(satir + 1, 1, satir + 1, 2);
                    XLS.HucreBirlestir(satir + 1, 11, satir + 1, 12);
                    XLS.HucreBirlestir(satir + 1, 20, satir + 1, 21);
                }
                else if (form.kurum == "06")
                {
                    XLS.HucreBirlestir(satir + 1, 1, satir + 1, 2);
                    XLS.HucreBirlestir(satir + 1, 11, satir + 1, 12);
                    XLS.HucreBirlestir(satir + 1, 20, satir + 1, 21);
                }
            }
        }

        private static string ExceleYazUA(Kullanici kullanan, IMUHServis servisMUH, OdemeEmriMIFForm form, string excelYazYer, bool evrakaGonder, bool omk = false)
        {
            TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int tSatir = 0;

            //GenelIslemlerIstemci.VarsayilanKurumBul();
            string sablonAd;
            if (form.belgeTur == ENUMBelgeTuru.ODEMEEMRI)
            {
                sablonAd = "MUH-OdemeEmriBelgeUA.xltx";
            }
            else //if (form.belgeTur == ENUMBelgeTuru.MIF)
            {
                sablonAd = "MUH-MIFBelgeUA.xltx";
            }

            string sonucDosyaAd = "";
            bool gonder = false;
            string ciktiTur = GenelIslemler.ExcelTur();
            if (excelYazYer == "")
            {
                sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
                gonder = true;
                if (evrakaGonder)
                {
                    try { System.IO.File.Delete(sonucDosyaAd); }
                    catch { }
                    ciktiTur = "html";
                    sonucDosyaAd += "." + ciktiTur;
                }
            }
            else
                sonucDosyaAd = excelYazYer;

            if (omk)
            {
                sablonAd = "OMK" + sablonAd;
            }

            XLS.DosyaAc(SablonAdOl(Ayar.Oku("RaporSablonYol"), "MUH", sablonAd, evrakaGonder), sonucDosyaAd);

            string[] dizi;
            dizi = form.kurum.Split('.');
            XLS.HucreAdBulYaz("txtYil", form.yil);

            XLS.HucreAdBulYaz("ParaBirimiTL", TNS.HRC.Arac.ParaLiraBirimiBul(servisHRC, kullanan));
            XLS.HucreAdBulYaz("ParaBirimiKr", TNS.HRC.Arac.ParaKurusBirimiBul(servisHRC, kullanan));

            XLS.HucreAdBulYaz("txtBirimAd", form.birimAd);
            XLS.HucreAdBulYaz("txtIlgiliAd", form.ilgiliAd);
            XLS.HucreAdBulYaz("txtIlgiliNo", form.ilgiliNo);
            XLS.HucreAdBulYaz("txtIlgiliBankaAd", form.ilgiliBankaAd);
            XLS.HucreAdBulYaz("txtIlgiliBankaNo", form.ilgiliBankaNo);
            XLS.HucreAdBulYaz("txtIlgiliVD", form.ilgiliVD);

            XLS.HucreAdBulYaz("txtAciklamaEkler", form.aciklama.Replace('\r', ' '));

            XLS.HucreAdBulYaz("txtBelgeNo", form.belgeNo);
            if (!form.YEVYevTarih.isNull)
            {
                XLS.HucreAdBulYaz("txtYevmiyeTarih", form.YEVYevTarih.ToString());
                XLS.HucreAdBulYaz("txtYevmiyeNo", form.YEVYevNo);
                XLS.HucreAdBulYaz("txtYevmiyeNoTarih", form.YEVYevNo + "/" + form.YEVYevTarih.ToString());
            }

            XLS.HucreAdAdresCoz("txtHesapNo", ref satir, ref sutun);

            double borcToplami = 0;
            double alacakToplami = 0;
            double butceGiderToplami = 0;
            double ozelGiderToplami = 0;
            double kesintiToplami = 0;
            double odenmesiToplami = 0;
            double avansToplami = 0;
            double emanetToplami = 0;
            Hashtable yeniOdenecek = new Hashtable();

            Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);
            string hesapYapi = (string)t.Kirilimlar(0);

            ParaIsmi paraIsmi = TNS.HRC.Arac.ParaIsmiBul(servisHRC, kullanan);

            //Faaliyet bazında girilen Ödeme Emri belgeleri yazdırma sırasında faaliyetleri olmadığı için aynı satırların tekrarı olarak gözüküyor. Onun için aynı satırlar tek satıra toplanıyor
            foreach (OdemeEmriMIFDetay detay in form.detay.objeler)
            {
                if (TNS.DEG.Arac.VarsayilanKurumBul().Replace(".", "") == "460604" ||
                    TNS.DEG.Arac.VarsayilanKurumBul().Replace(".", "") == "4035" ||
                    TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/ODEMIFYAZDIRSATIRTEKRARLI") == "1")// EGO satırlar aynı olsa bile farklı yazılmasını istiyor
                    break;

                if (detay.hesapKod == "XXX") continue;

                string tertip = TNS.MUH.Arac.TertipStrOl(detay);
                string kontrol = detay.hesapKod + tertip;

                foreach (OdemeEmriMIFDetay kd in form.detay.objeler)
                {
                    if (kd.hesapKod == "XXX") continue;

                    if (kd.satirNo > detay.satirNo && ((kd.borc > 0 && detay.borc > 0) || (kd.alacak > 0 && detay.alacak > 0)))//iki tarafda aynı tutar türünde olmalı
                    {
                        string tertipKd = TNS.MUH.Arac.TertipStrOl(kd);
                        string kontrolKd = kd.hesapKod + tertipKd;
                        if (kontrolKd == kontrol)
                        {
                            string tut = kontrol + "\t" + detay.borc + "\t" + detay.alacak + "\t" + kd.borc + "\t" + kd.alacak + "\t";
                            detay.borc += kd.borc;
                            detay.alacak += kd.alacak;
                            kd.hesapKod = "XXX";
                            //tut = detay.borc + "\t" + detay.alacak;
                            //OrtakFonksiyonlar.HataDosyaYaz("OE_Dbg_" + form.belgeNo + ".txt", tut, false);
                        }
                    }
                }
            }

            //GV:36001;OdeDV:3600301,3600309;SozDV:3600302;KarDV:3600303;KDV:36004
            string gosterKesintiStr = XLS.HucreAdDegerAl("KesintiKodlar");
            string[] gosterKesintiAdlar = null;
            string[] gosterKesintiKodlar = null;
            double[] gosterKesintiToplamlar = null;
            if (!string.IsNullOrWhiteSpace(gosterKesintiStr))
            {
                string[] gosterKesintiler = null;
                gosterKesintiler = gosterKesintiStr.Split(';');
                gosterKesintiToplamlar = new double[gosterKesintiler.Length];
                gosterKesintiAdlar = new string[gosterKesintiler.Length];
                gosterKesintiKodlar = new string[gosterKesintiler.Length];
                for (int i = 0; i < gosterKesintiToplamlar.Length; i++)
                {
                    string[] ic = gosterKesintiler[i].Split(':');
                    gosterKesintiAdlar[i] = ic[0];
                    gosterKesintiKodlar[i] = ic[1];
                    gosterKesintiToplamlar[i] = 0;
                }
            }

            ParaBirimi pb = DondurParaBirimi(1);
            foreach (OdemeEmriMIFDetay detay in form.detay.objeler)
            {
                if (detay.hesapKod == "XXX") continue;

                dizi.Initialize();

                if (detay.borc == 0 && detay.alacak == 0 && detay.dovizBorc == 0 && detay.dovizAlacak == 0)
                    continue;

                string hKod = detay.hesapKod;
                if (hKod.Length != hesapYapi.Length)
                {
                    hKod = hKod.Replace(".", "");
                    hKod = hKod.PadRight(hesapYapi.Replace(".", "").Length, '0');
                    hKod = OrtakFonksiyonlar.KodNoktaKoy(hesapYapi, hKod);
                }

                dizi = hKod.Split('.');

                if (dizi.Length > 0)
                {
                    XLS.HucreAdAdresCoz("txtHesapNo", ref tSatir, ref sutun);
                    XLS.HucreDegerYaz(satir, sutun, (string)dizi[0]);
                }
                for (int i = 1; i < dizi.Length; i++)
                {
                    XLS.HucreAdAdresCoz("txtEko" + i.ToString(), ref tSatir, ref sutun);
                    if (sutun > -1)
                        XLS.HucreDegerYaz(satir, sutun, (string)dizi[i]);
                }

                string araKod = "";

                araKod = "txtKur";
                dizi.Initialize();
                dizi = detay.kur.Split('.');
                for (int i = 0; i < dizi.Length; i++)
                {
                    XLS.HucreAdAdresCoz(araKod + (i + 1).ToString(), ref tSatir, ref sutun);
                    if (sutun > -1)
                        XLS.HucreDegerYaz(satir, sutun, dizi[i]);
                }

                araKod = "txtFon";
                dizi.Initialize();
                dizi = detay.fon.Split('.');
                for (int i = 0; i < dizi.Length; i++)
                {
                    XLS.HucreAdAdresCoz(araKod + (i + 1).ToString(), ref tSatir, ref sutun);
                    if (sutun > -1)
                        XLS.HucreDegerYaz(satir, sutun, dizi[i]);
                }

                araKod = "txtFin";
                dizi.Initialize();
                dizi = detay.fin.Split('.');
                for (int i = 0; i < dizi.Length; i++)
                {
                    XLS.HucreAdAdresCoz(araKod + (i + 1).ToString(), ref tSatir, ref sutun);
                    if (sutun > -1)
                        XLS.HucreDegerYaz(satir, sutun, dizi[i]);
                }

                //araKod = "txtEko";
                //dizi.Initialize();
                //dizi = detay.eko.Split('.');
                //for (int i = 0; i < dizi.Length; i++)
                //{
                //    XLS.HucreAdAdresCoz(araKod + (i + 1).ToString(), ref tSatir, ref sutun);
                //    if (sutun > -1)
                //        XLS.HucreDegerYaz(satir, sutun, dizi[i]);
                //}
                string noktasizHesapKod = detay.hesapKod.Replace(".", "");
                if (detay.borc > 0 || detay.dovizBorc > 0)
                {
                    XLS.HucreAdAdresCoz("txtBorc", ref tSatir, ref sutun);
                    XLS.HucreDegerYaz(satir, sutun, Math.Truncate(detay.borc));
                    XLS.HucreAdAdresCoz("txtBorcYKR", ref tSatir, ref sutun);
                    XLS.HucreDegerYaz(satir, sutun, KurusBul(detay.borc));
                }

                if (detay.alacak > 0 || detay.dovizAlacak > 0)
                {
                    XLS.HucreAdAdresCoz("txtAlacak", ref tSatir, ref sutun);
                    XLS.HucreDegerYaz(satir, sutun, Math.Truncate(detay.alacak));
                    XLS.HucreAdAdresCoz("txtAlacakYKR", ref tSatir, ref sutun);
                    XLS.HucreDegerYaz(satir, sutun, KurusBul(detay.alacak));
                    GosterKodToplamAl(detay, gosterKesintiAdlar, gosterKesintiKodlar, gosterKesintiToplamlar);
                }

                if (noktasizHesapKod.StartsWith("103") || noktasizHesapKod.StartsWith("106"))
                {
                    BankaBilgi bb = TNS.MUH.Arac.HesapIBANBul(noktasizHesapKod);
                    XLS.HucreAdBulYaz("KurumIBAN", TNS.MUH.Arac.IBANBoslukla(bb.IBAN));
                }
                XLS.HucreAdAdresCoz("txtAciklama", ref tSatir, ref sutun);
                XLS.HucreDegerYaz(satir, sutun, detay.aciklama);
                XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(330));
                satir++;

                borcToplami += Math.Round(detay.borc, 2);
                alacakToplami += Math.Round(detay.alacak, 2);

                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = form.yil;
                h.hesapKod = detay.hesapKod;

                h = servisMUH.HesapKodOzellikAl(h);
                if (h != null && detay.kesinti < 1)
                {
                    //Odenecek tutarın kasa hesabından bulunması için 13.01.2017
                    if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.KASAHESABI) && detay.alacak > 0 &&
                        !(TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.AVANS) ||
                        TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.GELIRBUTCESI) ||
                        TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.GIDERBUTCESI) ||
                        TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.OZELGIDER) ||
                        TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.EMANET)))
                        OdenecekEkle(yeniOdenecek, pb.kisaltma, Math.Round(detay.alacak, 2), h, Math.Round(detay.dovizAlacak, 2));

                    if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.GIDERBUTCESI))//if(Number(borç)>0 && (hesapNo=="702" || hesapNo=="704"))
                        butceGiderToplami += Math.Round(detay.borc - detay.alacak, 2);//Bütçe Gideri

                    if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.KASAHESABI) && form.belgeTur == ENUMBelgeTuru.MIF)//if ((belgeTür==2) && (hesapNo=="103" || hesapNo=="100"))
                        odenmesiToplami += Math.Round(detay.alacak - detay.borc, 2);

                    if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.AVANS))//if (Number(borç)>0 && hesapNo=="172")
                        avansToplami += Math.Abs(Math.Round(detay.alacak, 2) - Math.Round(detay.borc, 2));
                }
                if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.OZELGIDER))//if(Number(borç)>0 && ((eko=="01010103" && hesapNo=="603") || (eko=="01010000" && hesapNo=="370")))
                    ozelGiderToplami += Math.Round(detay.borc, 2);//Özel Gider İndirimi

                if (detay.kesinti > 0 && detay.alacak > 0 && !(TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.OZELGIDER) && detay.borc > 0) && !(TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.EMANET) && detay.borc > 0))
                    kesintiToplami += Math.Round(detay.alacak - detay.borc, 2);

                if (TNS.MUH.Arac.NitelikVarmi(h.nitelik, (int)ENUMHesapKodNitelik.EMANET) && detay.borc > 0)
                    emanetToplami += Math.Round(detay.borc - detay.alacak, 2);
                //                OrtakFonksiyonlar.HataDosyaYaz("OdeMifYaz.txt", string.Format("detay.kesinti:{0}, h.hesapKod:{1}, h.nitelik:{2}, butceGiderToplami:{3}, odenmesiToplami:{4}, avansToplami:{5}, ozelGiderToplami:{6}, kesintiToplami:{7}, emanetToplami:{8}", detay.kesinti, h.hesapKod, h.nitelik, butceGiderToplami, odenmesiToplami, avansToplami, ozelGiderToplami, kesintiToplami, emanetToplami), false);
            }
            borcToplami = Math.Round(borcToplami, 2);
            alacakToplami = Math.Round(alacakToplami, 2);

            odenmesiToplami = (Math.Round(butceGiderToplami, 2) + Math.Round(ozelGiderToplami, 2) + Math.Round(avansToplami, 2) + Math.Round(emanetToplami, 2)) - Math.Round(kesintiToplami, 2);

            bool oebYazildi = false;
            if (!string.IsNullOrWhiteSpace(form.surecNo))
            {
                TNS.HRC.OnayForm onayForm = new TNS.HRC.OnayForm();
                onayForm.yil = form.surecYil;
                onayForm.surecNo = form.surecNo;
                ObjectArray oa = servisHRC.OnayFormListele(kullanan, onayForm);
                if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 1)
                {
                    onayForm = (TNS.HRC.OnayForm)oa.objeler[0];
                    if (onayForm.ihaleUsulu != 0 && onayForm.ihaleUsulu != 4)
                    {
                        if (OnMaliKontrolYapilmisMi(TNS.HRC.Arac.Tanimla(), form))
                            XLS.HucreAdBulYaz("OnMaliKontrol", "Ön Mali Kontrol Yapılmıştır.");
                        else
                            XLS.HucreAdBulYaz("OnMaliKontrol", "Ön Mali Kontrole Tabidir.");
                    }
                    XLS.HucreAdBulYaz("YaklasikMaliyetTL", onayForm.yaklasikMaliyet);
                }
                int[] oebYer = new int[10];
                XLS.HucreAdAdresCoz("txtOEBTur", ref oebYer[0], ref oebYer[1]);
                XLS.HucreAdAdresCoz("txtOEBTarih", ref oebYer[2], ref oebYer[3]);
                XLS.HucreAdAdresCoz("txtOEBNo", ref oebYer[4], ref oebYer[5]);
                XLS.HucreAdAdresCoz("txtOEBTutar", ref oebYer[6], ref oebYer[7]);
                XLS.HucreAdAdresCoz("txtOEBTutarYKR", ref oebYer[8], ref oebYer[9]);
                decimal tahakkukToplam = 0;

                if (oebYer[0] > 0)
                {
                    TNS.HRC.FaturaForm kriter = new TNS.HRC.FaturaForm();
                    kriter.yil = form.surecYil;
                    kriter.surecNo = form.surecNo;
                    kriter.durum = (int)ENUMAktiflik.AKTIF;
                    ObjectArray oaFaturalar = servisHRC.FaturaFormListele(kullanan, kriter);
                    if (oaFaturalar.sonuc.islemSonuc && oaFaturalar.ObjeSayisi > 0)
                    {
                        int yaz = 0;
                        string faturaTarihler = "";
                        string faturaNolar = "";
                        decimal digerToplam = 0;
                        for (int i = 0; i < oaFaturalar.ObjeSayisi; i++)
                        {
                            TNS.HRC.FaturaForm ff = (TNS.HRC.FaturaForm)oaFaturalar[i];
                            if (ff.faturaTarih.Oku() <= form.islemTarih.Oku())
                            {
                                oebYazildi = true;
                                tahakkukToplam += ff.genelToplam;
                                if (yaz < 2)
                                {
                                    XLS.HucreDegerYaz(oebYer[0] + yaz, oebYer[1], "Fatura");
                                    XLS.HucreDegerYaz(oebYer[2] + yaz, oebYer[3], ff.faturaTarih.ToString());
                                    XLS.HucreDegerYaz(oebYer[4] + yaz, oebYer[5], ff.faturaNo);
                                    LiraKurusYaz(XLS, oebYer[6] + yaz, oebYer[7], oebYer[8] + yaz, oebYer[9], Convert.ToDouble(ff.genelToplam));
                                }
                                else
                                {
                                    string ekle = "";
                                    if (yaz > 2) ekle = ",";
                                    faturaTarihler += ekle + ff.faturaTarih.ToString();
                                    faturaNolar += ekle + ff.faturaNo;
                                    digerToplam += ff.genelToplam;
                                }
                                yaz++;
                            }
                        }
                        if (yaz >= 2)
                        {
                            if (yaz > 2)
                            {
                                faturaTarihler = "Muhtelif";
                                faturaNolar = "Muhtelif";
                            }
                            yaz = 2;
                            XLS.HucreDegerYaz(oebYer[0] + yaz, oebYer[1], "Fatura");
                            XLS.HucreDegerYaz(oebYer[2] + yaz, oebYer[3], faturaTarihler);
                            XLS.HucreDegerYaz(oebYer[4] + yaz, oebYer[5], faturaNolar);
                            LiraKurusYaz(XLS, oebYer[6] + yaz, oebYer[7], oebYer[8] + yaz, oebYer[9], Convert.ToDouble(digerToplam));
                        }
                        LiraKurusYaz(XLS, oebYer[6] + 3, oebYer[7], oebYer[8] + 3, oebYer[9], Convert.ToDouble(tahakkukToplam));
                    }
                }
                TNS.HRC.Hakedis hakedis = null;
                hakedis = new TNS.HRC.Hakedis();
                XLS.HucreAdBulYaz("SurecYilNo", form.surecYil + "/" + form.surecNo);

                hakedis.yil = form.surecYil;
                hakedis.surecNo = form.surecNo;

                OdemeEmriMIFForm[] ilgiliBelgeler = servisMUH.OdemeEmriMIFSurecBelgeNoListele(kullanan, form.surecYil, form.surecNo, form.yil, form.belgeNo);
                double oncedenOdenen = 0;
                for (int i = 0; i < ilgiliBelgeler.Length; i++)
                {
                    oncedenOdenen += ilgiliBelgeler[i].alacakToplam;
                }
                string ilgiliBelgeNolar = TNS.MUH.Arac.BelgenolarStrYap(ilgiliBelgeler, form.yil, true);

                hakedis = servisHRC.HakedisListele(kullanan, hakedis);
                if (hakedis.muhasebe != 0)
                {
                    LiraKurusYaz(XLS, "SozlesmeTL", "", hakedis.sozlesmeBedeli);
                    LiraKurusYaz(XLS, "SozlesmeKDVTL", "", hakedis.sozlesmeKDVBedeli);
                    LiraKurusYaz(XLS, "SozlesmeArtisTL", "", hakedis.sozlesmeArtisBedeli);
                    LiraKurusYaz(XLS, "TahakkukTL", "", Convert.ToDouble(tahakkukToplam));
                    LiraKurusYaz(XLS, "OnceOdenenTL", "", oncedenOdenen);
                }

                XLS.HucreAdBulYaz("IliskiliOdeEmri", ilgiliBelgeNolar);
            }

            if (!oebYazildi)
            {
                XLS.HucreAdBulYaz("txtOEBTur", form.OEBTur);
                XLS.HucreAdBulYaz("txtOEBTarih", form.OEBTarih.ToString());
                XLS.HucreAdBulYaz("txtOEBNo", form.OEBNo);
                if (form.OEBTutar > 0)
                {
                    LiraKurusYaz(XLS, "txtOEBTutar", "txtOEBTutarYKR", form.OEBTutar);
                }
            }

            butceGiderToplami = Math.Round(butceGiderToplami, 2);
            odenmesiToplami = Math.Round(odenmesiToplami, 2);
            ozelGiderToplami = Math.Round(ozelGiderToplami, 2);
            emanetToplami = Math.Round(emanetToplami, 2);
            avansToplami = Math.Round(avansToplami, 2);
            kesintiToplami = Math.Round(kesintiToplami, 2);

            XLS.HucreAdBulYaz("txtToplamBorc", Math.Truncate(borcToplami));
            int kk = KurusBul(borcToplami);
            XLS.HucreAdBulYaz("txtToplamBorcYKR", kk);
            XLS.HucreAdBulYaz("txtToplamAlacak", Math.Truncate(alacakToplami));
            XLS.HucreAdBulYaz("txtToplamAlacakYKR", KurusBul(alacakToplami));
            if (butceGiderToplami > 0)
            {
                XLS.HucreAdBulYaz("txtButceGideriBorcToplami", Math.Truncate(butceGiderToplami));
                XLS.HucreAdBulYaz("txtButceGideriBorcToplamiYKR", KurusBul(butceGiderToplami));
            }
            XLS.HucreAdBulYaz("txtButceGideri", Math.Truncate(butceGiderToplami));
            XLS.HucreAdBulYaz("txtButceGideriYKR", KurusBul(butceGiderToplami));

            XLS.HucreAdBulYaz("txtOzelGider", Math.Truncate(ozelGiderToplami));
            XLS.HucreAdBulYaz("txtOzelGiderYKR", KurusBul(ozelGiderToplami));

            XLS.HucreAdBulYaz("txtKesinti", Math.Truncate(kesintiToplami));
            XLS.HucreAdBulYaz("txtKesintiYKR", KurusBul(kesintiToplami));


            if (butceGiderToplami > 0)
                XLS.HucreAdBulYaz("txtButceGideriYaziyla", OrtakFonksiyonlar.Yaziyla(butceGiderToplami, paraIsmi));

            if (odenmesiToplami > 0)
            {
                XLS.HucreAdBulYaz("txtOdenmesiYaziyla", OrtakFonksiyonlar.Yaziyla(odenmesiToplami, paraIsmi));
                XLS.HucreAdBulYaz("txtOdenmesi", odenmesiToplami);
            }
            //XLS.HucreAdBulYaz("txtOdenmesiYaziyla", OdenmesiBulYaziyla(yeniOdenecek));
            //XLS.HucreAdBulYaz("txtOdenmesi", OdenmesiBul(yeniOdenecek));

            double toplamYazilanKesinti = 0;
            if (gosterKesintiAdlar != null)
            {
                for (int i = 0; i < gosterKesintiAdlar.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(gosterKesintiAdlar[i]))
                    {
                        LiraKurusYaz(XLS, gosterKesintiAdlar[i] + "TL", gosterKesintiAdlar[i] + "Kr", gosterKesintiToplamlar[i]);
                        toplamYazilanKesinti += gosterKesintiToplamlar[i];
                    }
                }
            }
            LiraKurusYaz(XLS, "DigerKesTL", "DigerKesKr", Math.Round(kesintiToplami - toplamYazilanKesinti, 2));

            OdenekGondermeYazdirma.ImzaBilgileriniYaz(servisMUH, XLS, kullanan, form.belgeTur, 0, form.birim, form.islemTarih, YuksekMi(form.yil, odenmesiToplami));
            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            if (gonder)
            {
                if (evrakaGonder)
                    return EvrakaGonderUA(servisMUH, sonucDosyaAd, form, kullanan, omk);
                else
                {
                    uretilenDosya = sonucDosyaAd;
                    uretilenUzanti = ciktiTur;
                }
            }
            return "";
        }

        private static bool YuksekMi(int yil, double odenmesiToplami)
        {
            string tut = TNS.DEG.Arac.DegiskenDegerBul(yil, "/OdemeEmriImzaSiniri");
            if (string.IsNullOrWhiteSpace(tut)) return false;
            double deger = OrtakFonksiyonlar.ConvertToDbl(tut);
            if (deger <= 0) return false;
            if (odenmesiToplami >= deger) return true;
            return false;
        }

        /// <summary>
        /// Eski EvrakaGonderUA: belge UAEvrakAktar eklentisiyle (TNS.EvrakAktar) evrak sistemine gönderilirdi. Bu eklenti eski
        /// .NET Framework uygulamasına bağlı bir kitaplıktır ve Core'da yüklenemez; eski kodun eklenti bulunamayınca verdiği hata döner.
        /// </summary>
        private static string EvrakaGonderUA(IMUHServis servisMUH, string sonucDosyaAd, OdemeEmriMIFForm form, Kullanici kullanan, bool omk)
        {
            try { File.Delete(sonucDosyaAd); } catch { }
            return "UAEvrakAktar dosyası bulunamadı!";
        }

        /// <summary>
        /// Eski EvrakaGonder: belge PDF'e çevrilir, evrak servisine (yazilimURL + Servis/EvrakIslemServis.asmx, GidenEvrakKaydet) giden evrak
        /// olarak kaydedilir; dönen 11 haneli evrak no belgeye yazılır. Sonuç metni "bilgi" ile döner (eski MesajKutusu).
        /// </summary>
        private static string EvrakaGonder(IMUHServis servisMUH, string sonucDosyaAd, OdemeEmriMIFForm form, Kullanici kullanan, bool omk)
        {
            try
            {
                Aspose.Cells.Workbook workBook = new Aspose.Cells.Workbook(sonucDosyaAd);
                workBook.Save(sonucDosyaAd, Aspose.Cells.SaveFormat.Pdf);

                byte[] pdfBytes = File.ReadAllBytes(sonucDosyaAd);
                string pdfBase64 = Convert.ToBase64String(pdfBytes);

                string dosyaOnEk = "MIF";
                string dosyaTuru = "Muhasebe İşlem Fişi";
                if (form.belgeTur == ENUMBelgeTuru.ODEMEEMRI)
                {
                    dosyaOnEk = "OE";
                    dosyaTuru = "Ödeme Emri belgesi";
                }

                string konu = form.yil + "/" + form.belgeNo + " sayılı " + dosyaTuru;
                if (!string.IsNullOrEmpty(form.ilgiliAd.Trim()))
                    konu += "(" + form.ilgiliAd + ")";

                string prEvrakNo = "";
                bool kayitliMi = false;
                string evrakNo = servisMUH.OdemeEmriEvrakNoOku(form.yil, form.belgeNo);
                if (!string.IsNullOrWhiteSpace(evrakNo))
                {
                    prEvrakNo = evrakNo;
                    kayitliMi = true;
                }

                string ad = dosyaOnEk + form.yil + form.belgeNo + ".pdf";
                string xml = OdemeEmriWebServisleri.EvrakXMLOlustur("", prEvrakNo, konu, 2, form.ilgiliAd, "000", form.aciklama, ad, pdfBase64);

                prEvrakNo = OdemeEmriWebServisleri.GidenEvrakKaydet(kullanan.kullaniciKodu, kullanan.parola, xml);

                if (OrtakFonksiyonlar.ConvertToLong(prEvrakNo) > 0 && prEvrakNo.Length == 11)
                {
                    if (kayitliMi == false)
                    {
                        OdemeEmriEvrakNoKaydet(servisMUH, form.yil, prEvrakNo, form.belgeNo, 0);
                    }

                    evrakBilgi = "Belgeniz, evrak sistemine aktarılmıştır.";
                    return "";
                }
                return string.IsNullOrWhiteSpace(prEvrakNo) ? "Evrak sistemi yanıt vermedi." : prEvrakNo; //Evrak kayıt işlemi hata döndürdü ise
            }
            catch (Exception ex)
            {
                return "Evrak sistemine gönderilemedi: " + ex.Message;
            }
            finally
            {
                try { File.Delete(sonucDosyaAd); } catch { }
            }
        }

        private static void OdemeEmriEvrakNoOku()
        {
            throw new NotImplementedException();
        }

        private static void OdemeEmriEvrakNoKaydet(IMUHServis servisMUH, int yil, string evrakNo, string odemeEmriNo, int durum)
        {
            servisMUH.OdemeEmriEvrakNoKaydet(null, yil, evrakNo, odemeEmriNo, durum);
        }

        private static void GosterKodToplamAl(OdemeEmriMIFDetay detay, string[] gosterKesintiAdlar, string[] gosterKesintiKodlar, double[] gosterKesintiToplamlar)
        {
            if (gosterKesintiAdlar == null) return;
            string noktasizKod = detay.hesapKod.Replace(".", "");
            for (int i = 0; i < gosterKesintiAdlar.Length; i++)
            {
                if (KodListedeVarMi(noktasizKod, gosterKesintiKodlar[i]))
                {
                    gosterKesintiToplamlar[i] += (detay.alacak - detay.borc);
                }
            }
        }

        private static bool KodListedeVarMi(string hesapKodNoktasiz, string kodlarstr)
        {
            if (string.IsNullOrWhiteSpace(kodlarstr)) return false;
            string[] kodlar = kodlarstr.Split(',');
            for (int i = 0; i < kodlar.Length; i++)
            {
                if (hesapKodNoktasiz.StartsWith(kodlar[i])) return true;
            }
            return false;
        }

        private static bool OnMaliKontrolYapilmisMi(TNS.HRC.IHRCServis servisHRC, OdemeEmriMIFForm form)
        {
            if (!string.IsNullOrEmpty(form.surecNo))
            {
                return false; // servisHRC.OnMaliKontrolYapilmisMi(form.surecYil, form.surecNo);
            }
            else
                return false;
        }

        private static double OnMaliKontrolSinirBul(int yil)
        {
            double val;
            string OnMaliKontrolSinir = TNS.MUH.Arac.DegiskenDegerBul(yil, "/OnMaliKontrolSinir");
            if (!string.IsNullOrEmpty(OnMaliKontrolSinir))
                val = OrtakFonksiyonlar.ConvertToDouble(OnMaliKontrolSinir);
            else
                val = 50000;
            return val;
        }

        private static void LiraKurusYaz(Tablo XLS, string LiraAlanAd, string KurusAlanAd, double tutar)
        {
            if (string.IsNullOrWhiteSpace(KurusAlanAd))
            {
                XLS.HucreAdBulYaz(LiraAlanAd, tutar);
            }
            else
            {
                XLS.HucreAdBulYaz(LiraAlanAd, Math.Truncate(tutar));
                XLS.HucreAdBulYaz(KurusAlanAd, KurusBul(tutar));
            }
        }

        private static void LiraKurusYaz(Tablo XLS, int satirL, int sutunL, int satirK, int sutunK, double tutar)
        {
            XLS.HucreDegerYaz(satirL, sutunL, Math.Truncate(tutar));
            XLS.HucreDegerYaz(satirK, sutunK, KurusBul(tutar));
        }
    }
}
