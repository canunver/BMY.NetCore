using HRCServisI;
using System;
using System.Collections.Generic;
using System.Configuration;
using TNS;
using TNS.BKD;
using TNS.DEG;
using TNS.HRC;
using TNS.IKY;
using TNS.KYM;
using TNS.UZY;

namespace TNS.HRC
{
    /// <summary>
    /// Harcama Yönetimi sınıfı için; istemci ve sunucuda ortak kullanılacak statik fonksiyonlar içeren sınıf
    /// </summary>
    [Serializable()]
    public static class Arac
    {

        public static bool BoolYap(string value)
        {
            return (value == "True" || value == "true");
        }


        public static TNS.HRC.IHRCServis _servisHRC = null;
        public static TNS.HRC.IHRCServis servisHRC
        {
            get
            {
                if (_servisHRC == null)
                {
                    _servisHRC = Tanimla();
                }
                return _servisHRC;
            }
        }

        private static ObjectArray paraBirimleri = null;
        public static ObjectArray ParaBirimleri
        {
            get
            {
                if (paraBirimleri == null)
                {
                    paraBirimleri = servisHRC.ParaBirimiListele(null, new ParaBirimi());
                }
                return paraBirimleri;
            }
        }

        public static TNS.BKD.ParaBirimi ParaBirimiBul(IHRCServis servisHRC, Kullanici kullanan, int paraBirimiKod, string paraBirimiKisaltma)
        {
            TNS.BKD.ParaBirimi donenDeger = new TNS.BKD.ParaBirimi();

            TNS.BKD.ParaBirimi para = new TNS.BKD.ParaBirimi();
            para.kod = paraBirimiKod;
            para.kisaltma = paraBirimiKisaltma;
            ObjectArray paralar = servisHRC.ParaBirimiListele(kullanan, para);

            foreach (TNS.BKD.ParaBirimi bilgi in paralar.objeler)
            {
                donenDeger = bilgi;
            }

            return donenDeger;
        }

        public static decimal KurDegeriDondur(Kullanici kullanan, ParaBirimi pb, ref TNSDateTime dt, bool yakinGunDondur, int tur = 1)
        {
            if (pb == null || pb.kod == 1 || pb.kod == 0) //Ana Para Birimi
                return 1;
            else
            {
                TNS.BKD.ParaBirimi paraBirimi = pb;
                if (string.IsNullOrEmpty(pb.kisaltma))
                {
                    paraBirimi = ParaBirimiBul(servisHRC, kullanan, pb.kod, "");
                }

                MerkezBankasiKurSinif t = new MerkezBankasiKurSinif();
                t.kurTarih = dt;
                t.satirlar = new MerkezBankasiKurDetaySinif[1];
                t.satirlar[0] = new MerkezBankasiKurDetaySinif();
                t.satirlar[0].paraBirim = paraBirimi;
                t.yakinGunBul = yakinGunDondur;
                t = servisHRC.MerkezBankKurOku(kullanan, t);
                if (t.satirlar != null && t.satirlar.Length == 1)
                {
                    dt = t.kurTarih;
                    if (tur == 2)
                        return t.satirlar[0].dovizSatis;
                    return t.satirlar[0].efektifSatis;
                }
                return 0;
            }
        }

        public static void AvansHesaplaMUH(TNS.MUH.IMUHServis servisMUH, Kullanici kullanan, int surecYil, string surecNo, out double avans, out string avansBilgiStr, out string avansBelgeNo, out TNSDateTime avansTarih)
        {
            avansTarih = null;
            TNS.MUH.OdemeEmriMIFForm kriter = new TNS.MUH.OdemeEmriMIFForm();
            kriter.surecYil = surecYil;
            kriter.surecNo = surecNo;
            ObjectArray avansBilgi = servisMUH.AvansBelgeleriListele(kullanan, kriter, false);
            avans = 0;
            avansBilgiStr = "";
            avansBelgeNo = "";

            foreach (TNS.MUH.OdemeEmriMIFForm ff in avansBilgi.objeler)
            {
                avansBelgeNo = ff.belgeNo;
                if (ff.formDurum >= 1 && ff.formDurum <= 8)
                {
                    foreach (TNS.MUH.OdemeEmriMIFDetay fd in ff.detay.objeler)
                    {
                        double satirAvans = (fd.borc - fd.alacak);
                        if (satirAvans > 0)
                        {
                            avans += satirAvans;
                            if (avansBilgiStr != "") avansBilgiStr += ";";
                            avansBilgiStr += fd.hesapKod + "@" + fd.TertipStr() + "@" + satirAvans.ToString() + "@" + ff.belgeNo + "@" + fd.satirNo;
                            if (avansTarih == null)
                                avansTarih = ff.islemTarih;
                        }
                    }
                }
            }
        }

        public static double GelirVergisiHesapla(double d_oncMatrah, double d_buAy, ObjectArray GelirVergisiOranlari)
        {
            decimal oncMatrah = Convert.ToDecimal(d_oncMatrah);
            decimal buAy = Convert.ToDecimal(d_buAy);
            decimal matrah = oncMatrah + buAy;
            decimal donusDeger = 0;
            if (matrah > 0)
            {
                decimal taban = 0;
                foreach (PSLGelirVergisi item in GelirVergisiOranlari.objeler)
                {
                    if (oncMatrah >= taban && oncMatrah < item.miktar || item.miktar == 0)
                    {
                        decimal kullan = 0;
                        if (oncMatrah + buAy <= item.miktar || item.miktar == 0)
                            kullan = buAy;
                        else
                            kullan = item.miktar - oncMatrah;

                        donusDeger += OrtakFonksiyonlar.YuzdeHesapla(kullan, item.yuzde);
                        oncMatrah = item.miktar;
                        buAy = buAy - kullan;
                    }
                    if (buAy == 0) break;
                    taban = item.miktar;
                }
            }
            return Convert.ToDouble(donusDeger);
        }

        private static ParaBirimi _pb = null;

        public static ParaBirimi pb(TNS.HRC.IHRCServis servisHRC, Kullanici kullanan)
        {
            if (_pb == null)
            {
                ParaBirimi kriter = new ParaBirimi();
                kriter.kod = 1;
                ObjectArray oa = servisHRC.ParaBirimiListele(kullanan, kriter);
                if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 1)
                    _pb = (ParaBirimi)oa.objeler[0];
            }
            return _pb;
        }

        public static string ParaLiraBirimiBul(TNS.HRC.IHRCServis servisHRC, Kullanici kullanan)
        {
            ParaBirimi pbIc = pb(servisHRC, kullanan);
            if (pbIc == null)
                return "TL";
            return pbIc.ParaBirimiKod;
        }

        public static string ParaKurusBirimiBul(TNS.HRC.IHRCServis servisHRC, Kullanici kullanan)
        {
            ParaBirimi pbIc = pb(servisHRC, kullanan);
            if (pbIc == null)
                return "Kr";
            return pbIc.KurusBirimiKod;
        }

        private static ParaIsmi _paraIsmi = null;
        public static ParaIsmi ParaIsmiBul(TNS.HRC.IHRCServis servisHRC, Kullanici kullanan)
        {
            if (_paraIsmi == null)
            {
                ParaBirimi pbIc = pb(servisHRC, kullanan);
                if (pbIc == null)
                {
                    _paraIsmi = new ParaIsmi("Lira", true, "Kuruş", true);
                }
                else
                    _paraIsmi = pbIc.ParaIsmiBul;
            }
            return _paraIsmi;
        }

        public static int SEYYARGOREVISLEM = 95;
        public static Surec IlkSurec(ObjectArray oaSurecler)
        {
            if (oaSurecler == null) return null;
            if (oaSurecler.ObjeSayisi == 0) return null;
            return (Surec)oaSurecler[0];
        }

        public static string AboneAdBul(int tip)
        {
            switch (tip)
            {
                case (int)ENUMAboneTip.ELEKTRIK:
                    return "Elektrik";
                case (int)ENUMAboneTip.SU:
                    return "Su";
                case (int)ENUMAboneTip.DOGALGAZ:
                    return "Doğalgaz";
                case (int)ENUMAboneTip.TELEFON:
                    return "Telefon";
                case (int)ENUMAboneTip.ADSL:
                    return "ADSL";
                case (int)ENUMAboneTip.AVEA:
                    return "Avea";
                case (int)ENUMAboneTip.VODAFONE:
                    return "Vodafone";
                case (int)ENUMAboneTip.TURKCELL:
                    return "Turkcell";
                case (int)ENUMAboneTip.OZELSERVISTELLER:
                    return "Özel Telefon Servisleri";
                case (int)ENUMAboneTip.NOKTADANNOKTAYA:
                    return "Noktadan Noktaya";
                case (int)ENUMAboneTip.HATKIRASI:
                    return "Hat Kirası";
                case (int)ENUMAboneTip.KABLOLUTV:
                    return "Kablolu Tv";
                case (int)ENUMAboneTip.JEOTERMALNUKLEERENERJI:
                    return "Jeotermal Enerji - Nükleer Enerji";
                case (int)ENUMAboneTip.DIGER:
                    return "Diğer";
            }
            return "Diğer";
        }

        /// <summary>
        /// Bu sunucu sınıfının nesnesini, konfigürasyon dosyasından okuduğu
        /// remoting dizgisini kullanarak, .net remoting ile oluşturup döndüren statik yordam 
        /// </summary>
        /// <returns>.net remoting ile oluşturulan nesne</returns>
        public static IHRCServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("HRCServisURL");
            IHRCServis servis = TNS.Api.ServisIstemci.Olustur<IHRCServis>(sUrl);
            return servis;
        }

        public static string DogrudanTeminKontrol(double yaklasikMaliyet, double yuzdeSinirOnceHarcanan, double yuzdeSinirToplam, int kontrolTuru, bool buyukSehirMi, double buyukSehirSinir, double kucukSehirSinir)
        {
            string hataStr = string.Empty;
            if (yaklasikMaliyet + yuzdeSinirOnceHarcanan > yuzdeSinirToplam)
            {
                string ekMesaj = string.Format("Doğrudan temin %10 üst sınırı: {0}, bugüne kadar harcanan: {1}, kullanılmak istenen: {2}", yuzdeSinirToplam, yuzdeSinirOnceHarcanan, yaklasikMaliyet);
                if (kontrolTuru == 1)
                    hataStr += HarcamaSurecleriResI.FRMIOF069 + "<br>" + ekMesaj;
                else
                    hataStr += HarcamaSurecleriResI.FRMIOF070 + "<br>" + ekMesaj;
            }

            if (buyukSehirMi)
            {
                if (yaklasikMaliyet > buyukSehirSinir)
                    hataStr += string.Format(HarcamaSurecleriResI.FRMIOF071, buyukSehirSinir.ToString("#,###.00")) + "<br>";
            }
            else
            {
                if (yaklasikMaliyet > kucukSehirSinir)
                    hataStr += string.Format(HarcamaSurecleriResI.FRMIOF072, kucukSehirSinir.ToString("#,###.00")) + "<br>";
            }

            return hataStr;
        }

        public static string PazarlikUsuluKontrol(double yaklasikMaliyet, double yuzdeSinirOnceHarcanan, double yuzdeSinirToplam, double pazarlikFUstSinir)
        {
            string hataStr = string.Empty;
            if (yaklasikMaliyet + yuzdeSinirOnceHarcanan > yuzdeSinirToplam)
            {
                string ekMesaj = string.Format("Doğrudan temin %10 üst sınırı: {0}, bugüne kadar harcanan: {1}, kullanılmak istenen: {2}", yuzdeSinirToplam, yuzdeSinirOnceHarcanan, yaklasikMaliyet);
                hataStr += HarcamaSurecleriResI.FRMIOF073 + "<br>" + ekMesaj;
            }
            if (yaklasikMaliyet > pazarlikFUstSinir)
                hataStr += string.Format(HarcamaSurecleriResI.FRMIOF074, pazarlikFUstSinir.ToString("#,###.00")) + "<br>";

            return hataStr;
        }

        public static string BordroAsamaOl(int ay, int bordroTur, int bordroIslem)
        {
            return ay.ToString() + "_" + bordroTur.ToString() + "_" + bordroIslem.ToString();
        }

        public static string KYYapiIlKosul(bool harcamaBirimi = true)
        {
            string alanAd;
            if (harcamaBirimi)
                alanAd = "HARCAMABIRIMI";
            else
                alanAd = "KOD";
            return $"case SUBSTRING({alanAd},1,3) when '100' then '06' when '300' then '06' when '900' then '99' ELSE SUBSTRING({alanAd},2,2) END";
        }

        public static string KoduIlYap(string kod)
        {
            string ilKod;
            if (string.IsNullOrWhiteSpace(kod)) return "00";
            kod = kod.Replace(".", "");

            if (kod.Length < 3) return "00";
            if (kod.StartsWith("900")) ilKod = "99";
            else
            {
                ilKod = kod.Substring(1, 2);
                if (ilKod == "00") ilKod = "06";
            }
            return ilKod;
        }

        private static System.Collections.Hashtable tabloDeger = null;
        public static void TabloDegerSifirla()
        {
            if (tabloDeger != null)
                tabloDeger.Clear();
        }

        public static double TabloDegerYil(IHRCServis servis, Kullanici kullanan, int tabloKod, int unvanKod, int derece, int yil)
        {
            if (tabloDeger == null) tabloDeger = new System.Collections.Hashtable();
            string anahtar = tabloKod + "_" + unvanKod + "_" + derece + "_" + yil;
            object donDeger = tabloDeger[anahtar];
            if (donDeger == null)
            {
                donDeger = Convert.ToDouble(servis.OkuBordroTablo(null, tabloKod, unvanKod, derece, yil));
                tabloDeger[anahtar] = donDeger;
            }
            return (double)donDeger;
        }

        public static bool CumhurbaskanligiMi()
        {
            bool deger = false;
            string kurumKod = TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMKOD");

            if (kurumKod.Replace(".", "") == "02")
                deger = true;

            //Cumhurbaşkanlığı ile TBMM nin kurumsalkodları aynı (TBMM bakım anlaşması olmadığı için değişmedi) Melih 07.08.2023
            string kurAd = TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMAD");
            if (kurAd.Contains("TBMM"))
                deger = false;

            return deger;
        }

        public static double ParaCevir(double miktar, double kur, bool ters)
        {
            if (ters) return OrtakFonksiyonlar.KurusYap(miktar * kur);
            if (kur > 0)
                return OrtakFonksiyonlar.KurusYap(miktar / kur);
            else
                return 0;
        }
    }

    /// <summary>
    /// Harcama Yönetimi sınıfı için; istemci ve sunucuda ortak kullanılacak statik fonksiyonlar içeren sınıf
    /// </summary>
    [Serializable()]
    public static class OrtakServis
    {
        public static string SatirAciklamaOl(TNS.MUH.IMUHServis servisMUH, Kullanici kullanan, string kimlikNo, int saymanlikKod)
        {
            TNS.MUH.IlgiliBilgi ib = servisMUH.IlgiliBilgiBul(kullanan, kimlikNo, saymanlikKod);
            if (ib == null)
                return kimlikNo;
            else
                return ib.ilgili + "(" + kimlikNo + ")";
        }

        public static TNS.HRC.Surec SurecBilgisiAl(IHRCServis servisHRC, Kullanici kullanan, int yil, string surecNo)
        {
            TNS.HRC.Surec surec = new TNS.HRC.Surec();
            surec.yil = yil;
            surec.surecNo = surecNo;
            ObjectArray surecler = servisHRC.SurecListele(kullanan, surec);
            if (surecler.objeler.Count > 0)
                return (Surec)surecler.objeler[0];
            else
                return null;
        }

        /// <summary>
        /// Web configdeki DamgaVergisiHK değerine göre damga vergisi hesap kodu döndürür
        /// </summary>
        /// <returns>Damga vergisi hesap kodu döndürür</returns>
        //[Obsolete("Damga vergisi türü olan metodu kullanın")]
        //public static string DamgaVergisiHesapKodu()
        //{
        //    string hesapKod = ConfigurationManager.AppSettings.Get("DamgaVergisiHK") + "";
        //    if (string.IsNullOrEmpty(hesapKod))
        //    {
        //        if (OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
        //            hesapKod = "600.01.05.01.01";
        //        else
        //            hesapKod = "360.03.09";
        //    }
        //    return hesapKod;
        //}

        /// <summary>
        /// Web configdeki DamgaVergisiHK değerine göre damga vergisi hesap kodu döndürür
        /// </summary>
        /// <param name="tur">Hesap kodu istenilen damga vergisi türü</param>
        /// <returns>Damga vergisi hesap kodu döndürür</returns>
        public static string DamgaVergisiHesapKodu(ENUMDamgaVergisiTur tur)
        {
            string hesapKod = ConfigurationManager.AppSettings.Get("DamgaVergisiHK" + ((int)tur).ToString()) + "";
            if (!string.IsNullOrEmpty(hesapKod)) return hesapKod;

            hesapKod = ConfigurationManager.AppSettings.Get("DamgaVergisiHK") + "";
            if (string.IsNullOrEmpty(hesapKod))
            {
                if (OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                    hesapKod = "600.01.05.01.01";
                else
                {
                    if (tur == ENUMDamgaVergisiTur.KISI)
                        hesapKod = "360.03.01";
                    else
                        hesapKod = "360.03.09";
                }
            }
            return hesapKod;
        }

        public static string DamgaVergisiHesapKodu(string vergiNo, bool yolluk)
        {
            if (yolluk)
                return DamgaVergisiHesapKodu(ENUMDamgaVergisiTur.YOLLUK);
            else if (vergiNo.Length == 11)
                return DamgaVergisiHesapKodu(ENUMDamgaVergisiTur.KISI);
            else
                return DamgaVergisiHesapKodu(ENUMDamgaVergisiTur.DIGER);
        }

        /// <summary>
        /// Web configdeki DamgaVergisiYK değerine göre damga vergisi yansıtma hesap kodu döndürür
        /// </summary>
        /// <returns>Damga vergisi yansıtma hesap kodu döndürür</returns>
        public static string DamgaVergisiYansitmaHesapKodu()
        {
            string damgaVergisi = ConfigurationManager.AppSettings.Get("DamgaVergisiYK") + "";
            if (string.IsNullOrEmpty(damgaVergisi))
            {
                if (OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                    damgaVergisi = "800.01.05.01.01";
            }
            return damgaVergisi;
        }

        /// <summary>
        /// Web configdeki GelirVergisiHK değerine göre gelir vergisi hesap kodu döndürür
        /// </summary>
        /// <param name="ayrinti">Gelir vergisi hesap kodu ayrıntısı</param>
        /// <returns>Gelir vergisi hesap kodu döndürür</returns>
        public static string GelirVergisiHesapKodu(string ayrinti)
        {
            string hesapKod = ConfigurationManager.AppSettings.Get("GelirVergisiHK") + "";
            if (string.IsNullOrEmpty(hesapKod))
            {
                if (OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                    hesapKod = "600.01.01.01.03";
                else
                    hesapKod = "360.01." + ayrinti;
            }
            return hesapKod;
        }


        public static string AsgariGecimIndirimiHesapKodu()
        {
            string hesapKod = string.Empty;

            if (OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                hesapKod = "630.12.01.01.09";
            else
                hesapKod = "360.01.01.02"; //+ayrinti;

            return hesapKod;
        }

        /// <summary>
        /// Web configdeki DamgaVergisiHK değerine göre gelir vergisi hesap kodu döndürür
        /// </summary>
        /// <param name="tur">Hesap kodu istenilen damga vergisi türü</param>
        /// <returns>Gelir vergisi hesap kodu döndürür</returns>
        public static string GelirVergisiHesapKodu(ENUMDamgaVergisiTur tur)
        {
            string hesapKod = ConfigurationManager.AppSettings.Get("GelirVergisiHK") + "";
            if (string.IsNullOrEmpty(hesapKod))
            {
                if (OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                    hesapKod = "600.01.01.01.03";
                else
                {
                    if (tur == ENUMDamgaVergisiTur.KISI)
                        hesapKod = "360.03.01";
                }
            }
            return hesapKod;
        }

        /// <summary>
        /// Web configdeki GelirVergisiYK değerine göre gelir vergisi yansıtma hesap kodu döndürür
        /// </summary>
        /// <returns>Gelir vergisi yansıtma hesap kodu döndürür</returns>
        public static string GelirVergisiYansitmaHesapKodu()
        {
            string hesapKod = ConfigurationManager.AppSettings.Get("GelirVergisiYK") + "";
            if (string.IsNullOrEmpty(hesapKod))
            {
                if (OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                    hesapKod = "800.01.01.01.03";
            }
            return hesapKod;
        }

        /// <summary>
        /// Web configdeki GelirVergisiHK değerine göre KDV tevkifatı hesap kodu döndürür
        /// </summary>
        /// <returns>KDV tevkifatı hesap kodu döndürür</returns>
        public static string KDVTevkifatiHesapKodu()
        {
            string hesapKod = ConfigurationManager.AppSettings.Get("GelirVergisiHK") + "";
            if (string.IsNullOrEmpty(hesapKod))
            {
                if (OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                    hesapKod = "600.01.03.01.02";
                else
                    hesapKod = "360.04.02";
            }
            return hesapKod;
        }

        /// <summary>
        /// Web configdeki GelirVergisiYK değerine göre KDV tevkifatı yansıtma hesap kodu döndürür
        /// </summary>
        /// <returns>KDV tevkifatı yansıtma hesap kodu döndürür</returns>
        public static string KDVTevkifatiYansitmaHesapKodu()
        {
            string hesapKod = ConfigurationManager.AppSettings.Get("GelirVergisiYK") + "";
            if (string.IsNullOrEmpty(hesapKod))
            {
                if (OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                    hesapKod = "800.01.03.01.02";
            }
            return hesapKod;
        }

        /// <summary>
        /// Kurum bütçe türünü, varsayılan kurum koduna göre bulur ve döndürür
        /// </summary>
        /// <returns>Kurum bütçe türünü döndürür</returns>
        public static ENUMKurumButceTur KurumButceTur()
        {
            string kurumButceTur = TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMBUTCETUR");
            if (kurumButceTur == "1")
                return ENUMKurumButceTur.GENELBUTCE;
            else if (kurumButceTur == "2")
                return ENUMKurumButceTur.OZELBUTCE;
            else
            {
                string kurumKod = VarsayilanKurumBul();
                if (OrtakFonksiyonlar.ConvertToInt(kurumKod.Substring(0, 2), 0) < 38)
                    return ENUMKurumButceTur.GENELBUTCE;
                else
                    return ENUMKurumButceTur.OZELBUTCE;
            }
        }

        public static string VarsayilanKurumBul()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMKOD");
        }

        public static string VarsayilanKurumBulAd()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMAD");
        }

        public static bool UlusalAjansMi()
        {
            return VarsayilanKurumBul() == "25.75" || VarsayilanKurumBul() == "11.75";
        }

        /// <summary>
        /// Ödeme Emri Belgesi detayındaki satır bilgisini oluşturur ve TNS.MUH.OdemeEmriMIFDetay nesnesi olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılan yıl bilgisi.</param>
        /// <param name="hesapKod">İşlem yapılan hesap kod bilgisi.</param>
        /// <param name="kesinti">İşlem yapılan kesinti bilgisi.</param>
        /// <param name="tertip">İşlem yapılan tertip bilgisi.</param>
        /// <param name="borc">İşlem yapılan borc bilgisi.</param>
        /// <param name="alacak">İşlem yapılan alacak bilgisi.</param>
        /// <param name="avans">İşlem yapılan avans bilgisi.</param>
        /// <returns>Verilen argumanlara göre ödeme emri belgesi detay satır bilgisini oluşturur ve TNS.MUH.OdemeEmriMIFDetay nesnesi olarak döner</returns>
        public static TNS.MUH.OdemeEmriMIFDetay OdemeEmriDetaySatirOlustur(int yil, string hesapKod, int kesinti, string tertip, double borc, double alacak, double avans)
        {
            TNS.MUH.OdemeEmriMIFDetay td = new TNS.MUH.OdemeEmriMIFDetay(yil);
            td.hesapKod = hesapKod;
            td.kesinti = kesinti;

            if (tertip != "")
                TNS.MUH.Arac.TertipAta(ref td, tertip);

            td.borc = borc;
            if (avans > 0)
            {
                td.borc = 0;
                if (alacak < 0)
                    td.alacak = borc;
                else
                    td.alacak = avans;
            }
            else
                td.alacak = alacak;

            return td;
        }

        /// <summary>
        /// Verilen argumanlara bağlı olarak, ödeme emri belgesi başlık bilgilerini oluşturur
        /// </summary>
        /// <param name="tf">Üzerinde işlem yapılan TNS.MUH.OdemeEmriMIFForm nesnesi </param>
        /// <param name="yil">İşlem yapılan yıl bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılan muhasebe kodu bilgisi.</param>
        /// <param name="birim">İşlem yapılan birim kodu bilgisi.</param>
        /// <param name="odemeEmriNo">İşlem yapılan odeme emri no bilgisi.</param>
        /// <param name="aciklama">Başlık açıklama bilgisi.</param>
        /// <param name="islemTarih">İşlem yapılan işlem tarih bilgisi.</param>
        /// <param name="kullaniciKodu">İşlem yapan kullanıcı kodu bilgisi.</param>
        /// <param name="surecNo">İşlem yapılan sureç no bilgisi.</param>
        public static void OdemeEmriBaslikOlustur(TNS.MUH.OdemeEmriMIFForm tf, int yil, int muhasebe, string birim, string odemeEmriNo, string aciklama, TNSDateTime islemTarih, string kullaniciKodu, string surecNo)
        {
            tf.yil = yil;
            tf.muhasebe = muhasebe;
            tf.birim = birim;
            tf.belgeNo = odemeEmriNo.Trim();
            tf.aciklama = aciklama;
            tf.islemTarih = islemTarih;
            tf.formDurumTarih = new TNSDateTime(DateTime.Now.ToString());
            tf.kurum = OrtakServis.VarsayilanKurumBul();
            tf.islemYapan = kullaniciKodu;
            tf.surecYil = yil;
            tf.surecNo = surecNo;
        }

        /// <summary>
        /// Verilen argumanlara bağlı olarak, ödeme emri belgesi detay bilgilerini oluşturur
        /// </summary>
        /// <param name="tf">Üzerinde işlem yapılan TNS.MUH.OdemeEmriMIFForm nesnesi </param>
        /// <param name="odemeHesap">İşlem yapılacak ödeme hesap bilgisi.</param>
        /// <param name="tertip">İşlem yapılacak tertip bilgisi.</param>
        /// <param name="borcTutar">İşlem yapılacak borç tutar bilgisi.</param>
        /// <param name="damgaVergisiTutar">İşlem yapılacak damga vergisi tutar bilgisi.</param>
        /// <param name="gelirVergisiTutar">İşlem yapılacak gelir vergisi tutar bilgisi.</param>
        /// <param name="avansTutar">İşlem yapılacak avans tutar bilgisi.</param>
        /// <param name="avansHesap">İşlem yapılacak avans hesap bilgisi.</param>
        /// <param name="avansTertip">İşlem yapılacak avans tertip bilgisi.</param>
        /// <param name="emanetOrani">İşlem yapılacak emanet orani bilgisi.</param>
        public static string OdemeEmriDetayOlustur(TNS.MUH.OdemeEmriMIFForm tf, string odemeHesap, string tertip, double borcTutar, string damgaVergisiHesapKod, double damgaVergisiTutar, double gelirVergisiTutar, double avansTutar, string avansHesap, string avansTertip, double emanetOrani)
        {
            double emanetTutar = (borcTutar - damgaVergisiTutar) * emanetOrani / 100;
            double alacakTutar = borcTutar - (damgaVergisiTutar + gelirVergisiTutar + avansTutar + emanetTutar);

            string hata = TertipDizisiEkle(tf, tertip, "630", borcTutar, odemeHesap);
            if (!string.IsNullOrWhiteSpace(hata)) return hata;

            //****************************
            if (avansTutar > 0)
            {
                if (avansTertip != null)
                    tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, avansHesap, 1, avansTertip, borcTutar, alacakTutar, avansTutar));
                else
                {
                    string[] avansBilgiler = avansHesap.Split(';');
                    for (int i = 0; i < avansBilgiler.Length; i++)
                    {
                        string[] avansSatirBilgiler = avansBilgiler[i].Split('@');
                        tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, TNS.MUH.Arac.DiziElemanAl(avansSatirBilgiler, 0, ""), 1, TNS.MUH.Arac.DiziElemanAl(avansSatirBilgiler, 1, ""), 0, 0, OrtakFonksiyonlar.ConvertToDouble(TNS.MUH.Arac.DiziElemanAl(avansSatirBilgiler, 2, ""))));
                    }
                }
            }

            //****************************
            if (damgaVergisiTutar > 0)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, damgaVergisiHesapKod, 0, "", 0, damgaVergisiTutar, 0));

            //****************************
            if (gelirVergisiTutar > 0)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, OrtakServis.GelirVergisiHesapKodu("03"), 0, "", 0, gelirVergisiTutar, 0));

            //****************************
            if (emanetTutar > 0)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, "333.99.19", 0, "", 0, emanetTutar, 0));

            //****************************
            string tertipOdeme = "";
            if (odemeHesap.StartsWith("323") || odemeHesap.StartsWith("16"))
                tertipOdeme = tertip;
            else
                tertipOdeme = "";

            if (borcTutar > avansTutar)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, odemeHesap, 0, tertipOdeme, 0, alacakTutar, 0));

            hata = TertipDizisiEkle(tf, tertip, "830", borcTutar, odemeHesap);
            if (!string.IsNullOrWhiteSpace(hata)) return hata;

            //****************************
            if (damgaVergisiTutar > 0 && OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, OrtakServis.DamgaVergisiYansitmaHesapKodu(), 0, "", 0, damgaVergisiTutar, 0));

            //****************************
            if (gelirVergisiTutar > 0 && OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, OrtakServis.GelirVergisiYansitmaHesapKodu(), 0, "", 0, gelirVergisiTutar, 0));

            //****************************
            tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, "835", 0, "", 0, borcTutar, 0));

            //****************************
            if ((damgaVergisiTutar > 0 || gelirVergisiTutar > 0) && OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, "805", 0, "", damgaVergisiTutar + gelirVergisiTutar, 0, 0));
            return "";
        }

        /// <summary>
        /// Verilen argumanlara bağlı olarak, ödeme emri belgesi detay bilgilerini oluşturur
        /// </summary>
        /// <param name="tf">Üzerinde işlem yapılan TNS.MUH.OdemeEmriMIFForm nesnesi </param>
        /// <param name="odemeHesap">İşlem yapılacak ödeme hesap bilgisi.</param>
        /// <param name="tertip">İşlem yapılacak tertip bilgisi.</param>
        /// <param name="borcTutar">İşlem yapılacak borç tutar bilgisi.</param>
        /// <param name="damgaVergisiTutar">İşlem yapılacak damga vergisi tutar bilgisi.</param>
        /// <param name="gelirVergisiTutar">İşlem yapılacak gelir vergisi tutar bilgisi.</param>
        /// <param name="avansTutar">İşlem yapılacak avans tutar bilgisi.</param>
        /// <param name="avansHesap">İşlem yapılacak avans hesap bilgisi.</param>
        /// <param name="avansTertip">İşlem yapılacak avans tertip bilgisi.</param>
        /// <param name="emanetOrani">İşlem yapılacak emanet orani bilgisi.</param>
        /// <param name="pttTertip">İşlem yapılacak PTT tertip bilgisi.</param>
        /// <param name="pttTutar">İşlem yapılacak PTT tutar bilgisi.</param>
        public static void OdemeEmriDetayOlustur(TNS.MUH.OdemeEmriMIFForm tf, string odemeHesap, string tertip, double borcTutar, string damgaVergisiHesapKodu, double damgaVergisiTutar, double gelirVergisiTutar, double avansTutar, string avansHesap, string avansTertip, double emanetOrani, string pttTertip, double pttTutar)
        {
            string borcHesap = "";
            string yansitHesap = "";
            double emanetTutar = (borcTutar + pttTutar - damgaVergisiTutar) * emanetOrani / 100;
            double alacakTutar = borcTutar + pttTutar - (damgaVergisiTutar + gelirVergisiTutar + avansTutar + emanetTutar);//avans

            string[] kodlar = tertip.Trim().Split('-');
            if (tertip.Length > 3)
            {
                borcHesap = TNS.HRC.OrtakServis.HesapKodOlustur(kodlar[3], "630");
                yansitHesap = TNS.HRC.OrtakServis.HesapKodOlustur(kodlar[3], "830");
            }
            else
                return;

            if (odemeHesap.StartsWith("323") && OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                yansitHesap = yansitHesap.Replace("830", "831");

            //****************************
            tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, borcHesap, 0, tertip, borcTutar, 0, 0));

            //****************************
            if (pttTutar > 0)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, "630.03.05.02.01", 0, pttTertip, pttTutar, 0, 0));

            //****************************
            if (avansTutar > 0)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, avansHesap, 1, avansTertip, borcTutar + pttTutar, alacakTutar, avansTutar));

            //****************************
            if (damgaVergisiTutar > 0)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, damgaVergisiHesapKodu, 0, "", 0, damgaVergisiTutar, 0));

            //****************************
            if (gelirVergisiTutar > 0)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, OrtakServis.GelirVergisiHesapKodu("03"), 0, "", 0, gelirVergisiTutar, 0));

            //****************************
            if (emanetTutar > 0)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, "333.99.19", 0, "", 0, emanetTutar, 0));

            //****************************
            string tertipOdeme = "";
            if (odemeHesap.StartsWith("323") || odemeHesap.StartsWith("16"))
                tertipOdeme = tertip;
            else
                tertipOdeme = "";
            if (borcTutar + pttTutar > avansTutar)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, odemeHesap, 0, tertipOdeme, 0, alacakTutar, 0));

            //****************************
            tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, yansitHesap, 0, tertip, borcTutar, 0, 0));

            if (pttTutar > 0 && OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, "830.03.05.02.01", 0, pttTertip, pttTutar, 0, 0));

            //****************************
            if (damgaVergisiTutar > 0 && OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, OrtakServis.DamgaVergisiYansitmaHesapKodu(), 0, "", 0, damgaVergisiTutar, 0));

            //****************************
            if (gelirVergisiTutar > 0 && OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, OrtakServis.GelirVergisiYansitmaHesapKodu(), 0, "", 0, gelirVergisiTutar, 0));

            //****************************
            tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, "835", 0, "", 0, borcTutar + pttTutar, 0));

            //****************************
            if ((damgaVergisiTutar > 0 || gelirVergisiTutar > 0) && OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, "805", 0, "", damgaVergisiTutar + gelirVergisiTutar, 0, 0));
        }

        /// <summary>
        /// Verilen argumanlara bağlı olarak, ödeme emri belgesi detay bilgilerini oluşturur
        /// </summary>
        /// <param name="tf">Üzerinde işlem yapılan TNS.MUH.OdemeEmriMIFForm nesnesi </param>
        /// <param name="odemeHesap">İşlem yapılacak ödeme hesap bilgisi.</param>
        /// <param name="tertip">İşlem yapılacak tertip bilgisi.</param>
        /// <param name="borcTutar">İşlem yapılacak borç tutar bilgisi.</param>
        /// <param name="damgaVergisiTutar">İşlem yapılacak damga vergisi tutar bilgisi.</param>
        /// <param name="gelirVergisiTutar">İşlem yapılacak gelir vergisi tutar bilgisi.</param>
        /// <param name="avansTutar">İşlem yapılacak avans tutar bilgisi.</param>
        /// <param name="avansHesap">İşlem yapılacak avans hesap bilgisi.</param>
        /// <param name="avansTertip">İşlem yapılacak avans tertip bilgisi.</param>
        /// <param name="emanetOrani">İşlem yapılacak emanet orani bilgisi.</param>
        /// <param name="faizTutar">İşlem yapılacak faiz tutar bilgisi.</param>
        /// <param name="faizHesap">İşlem yapılacak faiz hesap bilgisi.</param>
        public static string OdemeEmriDetayOlustur(TNS.MUH.OdemeEmriMIFForm tf, string odemeHesap, string tertip, double borcTutar, string damgaVergisiHesapKodu, double damgaVergisiTutar, double gelirVergisiTutar, double avansTutar, string avansHesap, string avansTertip, double emanetOrani, double faizTutar, string faizHesap, string faizTertip, string avansIlgili, double digerKesinti)
        {
            double emanetTutar = (borcTutar - damgaVergisiTutar) * emanetOrani / 100;
            double alacakTutar = borcTutar + faizTutar - (damgaVergisiTutar + gelirVergisiTutar + avansTutar + emanetTutar);//avans

            string hata = TertipDizisiEkle(tf, tertip, "630", borcTutar, odemeHesap);
            if (!string.IsNullOrWhiteSpace(hata)) return hata;

            hata = TertipDizisiEkle(tf, faizTertip, faizHesap, faizTutar, odemeHesap);
            if (!string.IsNullOrWhiteSpace(hata)) return hata;

            //****************************
            if (avansTutar > 0)
            {
                if (!string.IsNullOrWhiteSpace(avansTertip))
                {
                    TNS.MUH.OdemeEmriMIFDetay dt = TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, avansHesap, 1, avansTertip, borcTutar, alacakTutar, avansTutar);
                    dt.altHesapKod = avansIlgili;
                    tf.detay.objeler.Add(dt);
                }
                else
                {
                    string[] avansBilgiler = avansHesap.Split(';');
                    for (int i = 0; i < avansBilgiler.Length; i++)
                    {
                        string[] avansSatirBilgiler = avansBilgiler[i].Split('@');
                        TNS.MUH.OdemeEmriMIFDetay dt = TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, TNS.MUH.Arac.DiziElemanAl(avansSatirBilgiler, 0, ""), 1, TNS.MUH.Arac.DiziElemanAl(avansSatirBilgiler, 1, ""), 0, 0, OrtakFonksiyonlar.ConvertToDouble(TNS.MUH.Arac.DiziElemanAl(avansSatirBilgiler, 2, "")));
                        dt.altHesapKod = avansIlgili;
                        tf.detay.objeler.Add(dt);
                    }
                }
            }

            //****************************
            if (damgaVergisiTutar > 0)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, damgaVergisiHesapKodu, 0, "", 0, damgaVergisiTutar, 0));

            //****************************
            if (gelirVergisiTutar > 0)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, OrtakServis.GelirVergisiHesapKodu("03"), 0, "", 0, gelirVergisiTutar, 0));

            //****************************
            if (emanetTutar > 0)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, "333.99.19", 0, "", 0, emanetTutar, 0));

            //****************************
            string tertipOdeme = "";
            if (odemeHesap.StartsWith("323") || odemeHesap.StartsWith("16"))
                tertipOdeme = tertip;
            else
                tertipOdeme = "";

            if ((borcTutar > avansTutar) || (borcTutar == 0 && faizTutar > avansTutar))
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, odemeHesap, 0, tertipOdeme, 0, alacakTutar - digerKesinti, 0));

            hata = TertipDizisiEkle(tf, tertip, "830", borcTutar, odemeHesap);
            if (!string.IsNullOrWhiteSpace(hata)) return hata;

            hata = TertipDizisiEkle(tf, faizTertip, "830", faizTutar, odemeHesap);
            if (!string.IsNullOrWhiteSpace(hata)) return hata;

            //****************************
            if (damgaVergisiTutar > 0 && OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, OrtakServis.DamgaVergisiYansitmaHesapKodu(), 0, "", 0, damgaVergisiTutar, 0));

            //****************************
            if (gelirVergisiTutar > 0 && OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, OrtakServis.GelirVergisiYansitmaHesapKodu(), 0, "", 0, gelirVergisiTutar, 0));

            //****************************
            tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, "835", 0, "", 0, borcTutar + faizTutar, 0));

            //****************************
            if ((damgaVergisiTutar > 0 || gelirVergisiTutar > 0) && OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, "805", 0, "", damgaVergisiTutar + gelirVergisiTutar, 0, 0));
            return "";
        }

        public static string TertipDizisiEkle(MUH.OdemeEmriMIFForm tf, string tertip, string hesapKod, double borcTutar, string odemeHesap)
        {
            if (borcTutar == 0) return "";
            string[] tertipDizi = tertip.Split(';');
            int tertipDiziBoy = tertipDizi.Length;
            double toplamTutar = 0;
            for (int i = 0; i < tertipDiziBoy; i++)
            {
                string borcHesap = "";
                string[] tertipOdenek = tertipDizi[i].Split('@');
                string icTertip = TNS.MUH.Arac.DiziElemanAl(tertipOdenek, 0, "");
                double icOdenek = OrtakFonksiyonlar.ConvertToDouble(TNS.MUH.Arac.DiziElemanAl(tertipOdenek, 1, ""));

                if (hesapKod == "630" || hesapKod == "830")
                {
                    string[] kodlar = icTertip.Trim().Split('-');
                    if (tertip.Length > 3)
                    {
                        borcHesap = TNS.HRC.OrtakServis.HesapKodOlustur(kodlar[3], hesapKod);
                    }
                    else
                        return "Ekonomik kod bulunamadığında işlem iptal edildi";

                    if (odemeHesap.StartsWith("323") && OrtakServis.KurumButceTur() == ENUMKurumButceTur.GENELBUTCE)
                        borcHesap = borcHesap.Replace("830", "831");
                }
                else borcHesap = hesapKod;
                if (tertipDiziBoy == 1 && icOdenek == 0) icOdenek = borcTutar;
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, borcHesap, 0, icTertip, icOdenek, 0, 0));
                //****************************
                toplamTutar += icOdenek;
            }
            double fark = Math.Round(toplamTutar - borcTutar, 2);
            if (fark != 0)
            {
                return string.Format("Ödenmesi gereken tutar {0} iken, girilen rakamların toplamı {1} olduğundan ödenek işlem iptal edildi.", borcTutar, toplamTutar);
            }
            return "";
        }

        public static MUH.OdemeEmriMIFDetay OdemeEmriDetayOlustur(MUH.OdemeEmriMIFForm tf, string hesapKod, double borc, double alacak)
        {
            MUH.OdemeEmriMIFDetay d = TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, hesapKod, 0, "", borc, alacak, 0);
            tf.detay.objeler.Add(d);
            return d;
        }
        /// <summary>
        /// Verilen argumanlara bağlı olarak, ödeme emri belgesi ilgili bilgilerini oluşturur
        /// </summary>
        /// <param name="tf">Üzerinde işlem yapılan TNS.MUH.OdemeEmriMIFForm nesnesi </param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kodu bilgisi.</param>
        /// <param name="birim">İşlem yapılaacak birim kodu bilgisi.</param>
        /// <param name="aboneTip">İşlem yapılacak abone tip bilgisi.</param>
        /// <param name="tcKimlik">İlgili kişiye ait TC Kimlik No bilgisi.</param>
        /// <param name="firmaKod">İlgili firma kodu bilgisi</param>

        public static void OdemeEmriIlgiliOlustur(Kullanici kullanan, TNS.MUH.OdemeEmriMIFForm tf, int muhasebe, string birim, int aboneTip, string tcKimlik, int firmaKod, string aboneNo)
        {
            TNS.HRC.IHRCServis servisHRC = HRC.Arac.Tanimla();
            if (aboneTip > 0)
            {
                TNS.HRC.AboneNo abone = new TNS.HRC.AboneNo();
                abone.muhasebe = muhasebe;
                abone.birim = birim;
                abone.aboneTip = aboneTip;
                abone.aboneNo = aboneNo;
                abone.sadeceIlgili = true;
                ObjectArray listeAbone = new ObjectArray();
                listeAbone = servisHRC.AboneNoListele(new Kullanici(), abone);
                foreach (TNS.HRC.AboneNo p in listeAbone.objeler)
                {
                    OrtakServis.OdemeEmriIlgiliOlustur(tf, p.firma.ad, p.firma.banka, p.firma.hesapNo, p.firma.vergiNo, p.firma.vergiDairesi);
                }
                return;
            }

            if (firmaKod > 0)
            {
                TNS.HRC.FirmaBilgisi firma = servisHRC.FirmaBilgisiListele(new Kullanici(), firmaKod);
                if (firma != null)
                {
                    OrtakServis.OdemeEmriIlgiliOlustur(tf, firma.ad, firma.banka, firma.hesapNo, firma.vergiNo, firma.vergiDairesi);
                }
                return;
            }

            TNS.MUH.IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
            MUH.IlgiliBilgi ib = muhServis.IlgiliBilgiBul(kullanan, tcKimlik, muhasebe);
            if (ib != null)
                OrtakServis.OdemeEmriIlgiliOlustur(tf, ib.ilgili, ib.ilgBanka, ib.ilgIBAN, tcKimlik, ib.vergiDairesi);
            else
                //if (tcKimlik != "")
                //{
                //    PersonelBilgi personel = new PersonelBilgi();
                //    personel = servisHRC.PersonelBilgiListele(new Kullanici(), tcKimlik, 0, "");
                //    if (personel != null)
                //    {
                //        OrtakServis.OdemeEmriIlgiliOlustur(ref tf, personel.adSoyad, personel.banka + " " + personel.sube, personel.hesapNo, personel.tcKimlikNo, personel.vergiDairesi);
                //        return;
                //    }
                //    else
                //    {
                //        TNS.HRC.FirmaBilgisi firma = servisHRC.FirmaBilgisiListele(new Kullanici(), tcKimlik);
                //        if (firma != null)
                //        {
                //            OrtakServis.OdemeEmriIlgiliOlustur(ref tf, firma.ad, firma.banka, firma.hesapNo, firma.vergiNo, firma.vergiDairesi);
                //            return;
                //        }
                //    }
                //}
                OrtakServis.OdemeEmriIlgiliOlustur(tf, tcKimlik, "", "", tcKimlik, "");
        }

        /// <summary>
        /// Verilen argumanlara bağlı olarak, ödeme emri belgesi ilgili bilgilerini oluşturur
        /// </summary>
        /// <param name="tf">Üzerinde işlem yapılan TNS.MUH.OdemeEmriMIFForm nesnesi </param>
        /// <param name="ad">İşlem yapılacak ilgili adı bilgisi</param>
        /// <param name="banka">İşlem yapılacak banka adı bilgisi</param>
        /// <param name="hesapNo">İşlem yapılacak hesap no bilgisi</param>
        /// <param name="vergiNo">İşlem yapılacak vergi no bilgisi.</param>
        /// <param name="vergiDairesi">İşlem yapılacak vergi dairesi bilgisi.</param>
        public static void OdemeEmriIlgiliOlustur(TNS.MUH.OdemeEmriMIFForm tf, string ad, string banka, string hesapNo, string vergiNo, string vergiDairesi)
        {
            tf.ilgiliAd = ad;
            tf.ilgiliBankaAd = banka;
            tf.ilgiliBankaNo = hesapNo;
            tf.ilgiliNo = vergiNo;
            tf.ilgiliVD = vergiDairesi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hesap kodu oluşturur ve string olarak döner
        /// </summary>
        /// <param name="ekoKod">İşlem yapılacak ekonomik kod bilgisi.</param>
        /// <param name="hesapKod">İşlem yapılacak hesap kod bilgisi.</param>
        /// <returns>Hesap kodu oluşturur ve string olarak döner</returns>
        public static string HesapKodOlustur(string ekoKod, string hesapKod)
        {
            string donusDeger = "";
            if (ekoKod.Trim() == "")
                return donusDeger;

            string[] eko = ekoKod.Split('.');

            if (hesapKod == "630" && eko[0] == "06")
            {
                donusDeger = "30";
                eko[3] = "";
            }
            for (int i = 0; i < eko.Length; i++)
            {
                if (eko[i] == "")
                    break;
                else if (eko[i].Length < 2)
                    eko[i] = eko[i].PadLeft(2, '0');

                if (donusDeger != "") donusDeger += ".";

                donusDeger += eko[i];
            }

            donusDeger = hesapKod + "." + donusDeger;

            return donusDeger;
        }

        //public static ObjectArray AvansBelgeleriListele(IHRCServis servisHRC, Kullanici kullanan, int yil, string surecNo)
        //{
        //    TNS.HRC.OdemeEmriMIFForm ff = new OdemeEmriMIFForm();
        //    ff.yil = yil;
        //    ff.surecNo = surecNo;
        //    ff.belgeTur = (int)ENUMOdemeBelgeTur.MIF;
        //    return servisHRC.OdemeEmriFormListele(kullanan, ff);
        //}

        public static string AvansKodYap(string hesapKod, string ekoKod)
        {
            string donusDeger = "";
            if (ekoKod.Trim() == "")
                return donusDeger;

            string[] eko = ekoKod.Split('.');

            hesapKod = hesapKod.Replace(".", "");
            if (hesapKod.StartsWith("322"))
            {
                hesapKod = "322" + eko[0] + "0" + eko[1] + "0" + eko[2] + eko[3];
            }
            else
            {
                if (hesapKod.Length > 5) hesapKod = hesapKod.Substring(0, 5);
                hesapKod = hesapKod + eko[0] + "0" + eko[1];
            }
            return OrtakFonksiyonlar.KodNoktaKoy("999.99.99.99.99", hesapKod);
        }

        public static string KullaniciYetkisindenKurKodBul(Kullanici kullanan, IHRCServis servisHRC, bool noktala = true, int yil = 0)
        {
            string kurKod = string.Empty;

            int muhasebe = 0;
            string birim = "";
            TNS.HRC.Yetki.KullanicininYetkiliOlduguMuhasebeBirim(kullanan, ref muhasebe, ref birim);

            if (muhasebe > 0 || !string.IsNullOrWhiteSpace(birim))
            {
                string kurumsalKodlar = servisHRC.MuhasebeBirimKurumsalKodAl(muhasebe, birim);
                if (!string.IsNullOrWhiteSpace(kurumsalKodlar))
                {
                    string[] kurumsalListe = kurumsalKodlar.Split(',');
                    if (kurumsalListe.Length == 1)
                        kurKod = kurumsalListe[0].Replace(".", "");
                    else if (kurumsalListe.Length > 1)
                    {
                        string tempKurKod = string.Empty;
                        int say = 0;
                        for (int i = 0; i < kurumsalListe.Length; i++)
                        {
                            tempKurKod = kurumsalListe[i].Replace(".", "");
                            if (tempKurKod.Substring(6, 2) != "00")
                            {
                                kurKod = tempKurKod;
                                say++;
                            }
                        }
                        if (say > 1)
                            kurKod = string.Empty;
                        //if (string.IsNullOrWhiteSpace(kurKod))
                        //    kurKod = kurumsalListe[0];
                    }
                }
            }

            if (noktala && !string.IsNullOrWhiteSpace(kurKod))
            {
                return KurKodNoktala(kurKod);
            }

            return kurKod;
        }

        public static string KurKodNoktala(string kurKod, int yil = 0)
        {
            kurKod = kurKod.Replace(".", "");
            if (yil == 0)
                yil = DateTime.Now.Year;
            string kodyapi = TNS.DEG.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD1/KODYAPI");
            return OrtakFonksiyonlar.KodNoktaKoy(kodyapi, kurKod);
        }

        public static string KurKoddanKurAdBul(Kullanici kullanan, IUZYServis servisUZY, string kurKod, int yil = 0)
        {
            kurKod = kurKod.Replace(".", "");
            if (yil == 0)
                yil = DateTime.Now.Year;
            var kurumAd = servisUZY.UzayDegeriStr(kullanan, "ABSKUR", string.Format("{0}-{1}", yil, kurKod), false);

            return kurumAd;
        }


        public static bool TalepMi(IHRCServis servis, int surecYil, string surecNo, ref string talepNo)
        {
            if (talepNo == null)
            {
                talepNo = servis.TalepNoBul(surecYil, surecNo);

            }
            return !string.IsNullOrEmpty(talepNo);
        }

        public static int YURTDISIGGISLEMKODBul(int yil)
        {
            int kod = OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(yil, "YURTDISIGGISLEMKOD"), 0);
            if (kod == 0)
                kod = 213;
            return kod;
        }

        public static double VarsayilanDamgaVergisi(ENUMDamgaVergisiTur dvTur, int yil)
        {
            string kod = TNS.DEG.Arac.DegiskenDegerBul(yil, "DAMGAVERGISIORANI" + ((int)dvTur).ToString());
            if (string.IsNullOrWhiteSpace(kod))
                kod = TNS.DEG.Arac.DegiskenDegerBul(yil, "DAMGAVERGISIORANI");
            if (!string.IsNullOrWhiteSpace(kod))
                return OrtakFonksiyonlar.ConvertToDbl(kod);
            return 0;
        }

        public static decimal KesirDeger(string kesir, out decimal pay, out decimal payda)
        {
            decimal donusDeger = 0;
            pay = 0;
            payda = 0;

            if (kesir == "0")
            {
                donusDeger = 0;
                pay = 0;
                payda = 1;
            }
            else if (kesir == "1/3")
            {
                donusDeger = (decimal)1 / 3;
                pay = 1;
                payda = 3;
            }
            else if (kesir == "1/2")
            {
                donusDeger = (decimal)1 / 2;
                pay = 1;
                payda = 2;
            }
            else if (kesir == "2/3")
            {
                donusDeger = (decimal)2 / 3;
                pay = 2;
                payda = 3;
            }
            else if (kesir == "1")
            {
                donusDeger = 1;
                pay = 1;
                payda = 1;
            }
            else if (kesir == "2")
            {
                donusDeger = 2;
                pay = 2;
                payda = 1;
            }
            return donusDeger;
        }

        public static decimal KurBilgisiDondur(Kullanici kullanan, IHRCServis servisHRC, int kod, TNSDateTime gun)
        {
            if (kod == 1 || kod == 0) //TL
                return 1;
            else
            {
                MerkezBankasiKurSinif kur = new MerkezBankasiKurSinif();
                kur.kurTarih = gun;

                MerkezBankasiKurSinif liste = servisHRC.MerkezBankKurOku(kullanan, kur);
                if (liste == null || liste.sonuc == null || !liste.sonuc.islemSonuc || liste.satirlar == null) return 0;
                foreach (MerkezBankasiKurDetaySinif item in liste.satirlar)
                {
                    if (item.paraBirim.kod == kod)
                        return item.serbestSatis;
                }
            }
            return 1;
        }

        public static double DamgaVergisiHesapla(double mik, double oran)
        {
            return Math.Round(mik * oran, 2);
        }

        public static string AvansArtigiDus(string avansBilgi, double avansArtigiMiktar)
        {
            if (avansArtigiMiktar <= 0) return avansBilgi;
            string artiksizAvans = "";
            string[] avansBilgiler = avansBilgi.Split(';');
            for (int i = 0; i < avansBilgiler.Length && avansArtigiMiktar > 0; i++)
            {
                string[] avansSatirBilgiler = avansBilgiler[i].Split('@');
                string avansHesapKod = TNS.MUH.Arac.DiziElemanAl(avansSatirBilgiler, 0, "");
                string avansTertip = TNS.MUH.Arac.DiziElemanAl(avansSatirBilgiler, 1, "");
                double satirAvans = OrtakFonksiyonlar.ConvertToDouble(TNS.MUH.Arac.DiziElemanAl(avansSatirBilgiler, 2, ""));
                string belgeNo = TNS.MUH.Arac.DiziElemanAl(avansSatirBilgiler, 3, "");
                string belgeSatir = TNS.MUH.Arac.DiziElemanAl(avansSatirBilgiler, 4, "");

                if (satirAvans > avansArtigiMiktar)
                {
                    satirAvans -= avansArtigiMiktar;
                    avansArtigiMiktar = 0;
                }
                else
                {
                    avansArtigiMiktar -= satirAvans;
                    satirAvans = 0;
                }
                if (satirAvans > 0)
                {
                    if (artiksizAvans != "") artiksizAvans += ";";
                    artiksizAvans += avansHesapKod + "@" + avansTertip + "@" + satirAvans.ToString() + "@" + belgeNo + "@" + belgeSatir;
                }
            }
            if (avansArtigiMiktar > 0) return "";
            return artiksizAvans;
        }
    }

    /// <summary>
    /// Fatura: Tüm Faturalar
    /// </summary>
    [System.Serializable()]
    public class FaturaTum
    {
        public int muhasebe { get; set; }
        public string birim { get; set; }
        /// <summary>
        /// Yıl: Fatura yılını giriniz.
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// Firma Kodu: Firma Kodu
        /// </summary>
        public string firmaKodu { get; set; }

        /// <summary>
        /// Firma Adı: Firma Adı
        /// </summary>
        public string firmaAdi { get; set; }

        /// <summary>
        /// Fatura No: Fatura No
        /// </summary>
        public string faturaNo { get; set; }

        /// <summary>
        /// Fatura Tarihi: Fatura Tarihi
        /// </summary>
        public TNSDateTime faturaTarihi { get; set; }
        public DateTime faturaTarihiDT
        {
            get
            {
                if (faturaTarihi == null)
                    return DateTime.MinValue;
                else
                    return faturaTarihi.Oku();
            }
            set
            {
                if (faturaTarihi == null) faturaTarihi = new TNSDateTime(value);
                else faturaTarihi.Yaz(value);
            }
        }

        /// <summary>
        /// Açıklama: Açıklama
        /// </summary>
        public string aciklama { get; set; }

        /// <summary>
        /// Fatura Tutarı: Fatura Tutarı
        /// </summary>
        public double faturaTutari { get; set; }
        public double faturaTutariKDVLI { get; set; }

        /// <summary>
        /// Süreç Yıl: Süreç Yılı
        /// </summary>
        public int surecYil { get; set; }

        /// <summary>
        /// Süreç No: Süreç No
        /// </summary>
        public string surecNo { get; set; }

        /// <summary>
        /// MİF Yıl: MİF Belge Yılı
        /// </summary>
        public int mIFYil { get; set; }

        /// <summary>
        /// MİF No: MİF Belge No
        /// </summary>
        public string mIFNo { get; set; }

        /// <summary>
        /// MİF No: MİF Belge No
        /// </summary>
        public int tip { get; set; }

        public FaturaTum()
        {
            tip = -1;
        }

        public TNSDateTime yevmiyeTarih { get; set; }

        public int yevmiyeNo { get; set; }

        public int mifDurum { get; set; }
        public bool sorguMukerrer = false;
        public string sorguKontrol = "";
    }


    /// <summary>
    /// Yetki kontrollerini düzenleyen fonksiyonları içeren sınıf
    /// </summary>
    [Serializable()]
    public static class Yetki
    {

        public static bool SistemYoneticisiMi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Arguman olarak verilen kriterler için yetki kontrolü yapar ve bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <param name="ogy">Yetki kontrolü yapılacak okuma görme yazma bilgisi.</param>
        /// <param name="belgeTur">Yetki kontrolü yapılacak belge tür bilgisi.</param>
        /// <returns>Yetki kontrolü yapar ve bool olarak döner</returns>
        public static bool YetkisiVarMi(Kullanici kullanan, int ogy, int belgeTur)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                return true;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.HARCAMAISLEMLERI))
                return true;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MUHASEBEYONETICISI))
                return true;

            string yetkiTipiYetkiDegeri = "";
            if (belgeTur == (int)TNS.MUH.ENUMBelgeTuru.ODEMEEMRI || belgeTur == (int)TNS.MUH.ENUMBelgeTuru.MIF)
                yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Ödeme Emri/MİF Yetkisi");

            yetkiTipiYetkiDegeri = yetkiTipiYetkiDegeri.Replace(";", "");
            int yetkiTipiYetkiDegeriInt = OrtakFonksiyonlar.ConvertToInt(yetkiTipiYetkiDegeri, -1);
            if (ogy == (int)ENUMOGY.Gorme && yetkiTipiYetkiDegeriInt > 0)
                return true;
            if (ogy == (int)ENUMOGY.Yazma && yetkiTipiYetkiDegeriInt > 1)
                return true;
            if (ogy == (int)ENUMOGY.Onaylama && yetkiTipiYetkiDegeriInt > 2)
                return true;
            if (ogy == (int)ENUMOGY.OnayKaldir && yetkiTipiYetkiDegeriInt == 5)
                return true;

            return false;
        }

        public static bool YetkisiVarMi(Kullanici kullanan, ENUMYetkiTipi yetkiTipi, ENUMKullaniciYetkiTipi kullaniciYetkiTipi)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                return true;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MUHASEBEYONETICISI))
                return true; 

            string yetkiTipiYetkiDegeri = "";
            if (yetkiTipi == ENUMYetkiTipi.YAPABILIR)
                yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Yapabileceği İşlemler");
            else if (yetkiTipi == ENUMYetkiTipi.GOREBILIR)
                yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Görebileceği İşlemler");
            else if (yetkiTipi == ENUMYetkiTipi.ONAYLAYABILIR)
                yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Onaylayabileceği İşlemler");


            string[] yetkiTipiYetkiDegeriListe = yetkiTipiYetkiDegeri.Split(';');
            for (int i = 0; i < yetkiTipiYetkiDegeriListe.Length; i++)
            {
                if (OrtakFonksiyonlar.ConvertToInt(yetkiTipiYetkiDegeriListe[i], 0) == (int)kullaniciYetkiTipi)
                    return true;
            }
            return false;
        }

        public static bool YetkisiVarMi(Kullanici kullanan, int ogy, int belgeTur, string muhasebe, string birim)
        {
            if (MuhasebeBirimYetkiVarMi(kullanan, muhasebe, birim))
                return YetkisiVarMi(kullanan, ogy, belgeTur);
            return false;
        }

        public static bool YetkisiVarMi(Kullanici kullanan, ENUMYetkiTipi yetkiTipi, ENUMKullaniciYetkiTipi kullaniciYetkiTipi, string muhasebe, string birim)
        {
            return YetkisiVarMi(kullanan, yetkiTipi, kullaniciYetkiTipi) && MuhasebeBirimYetkiVarMi(kullanan, muhasebe, birim);
        }

        /// <summary>
        /// İşlem yapılmak istenilen muhasebe/birim üzerinde yetkisinin olup olmadığını kontrol eden metod
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Harcama birim kodu</param>
        /// <returns></returns>
        public static bool MuhasebeBirimYetkiVarMi(Kullanici kullanan, string muhasebe, string birim)
        {
            if (kullanan.ad == "aktarim")//Sistem dışı aktarımların yapıldığı kullanıcı
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MUHASEBEYONETICISI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
            {
                if (OrtakFonksiyonlar.ConvertToInt(kullanan.calistigiKurum, 0) == OrtakFonksiyonlar.ConvertToInt(muhasebe, 0))
                    return true;
                else
                    return false;
            }

            string yetkiTipiAdi = "";

            yetkiTipiAdi = "Ödeme Emri/MİF Saymanlık-Birim Yetkisi";

            string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

            if (yetkiTipiDegeri.Trim() == "")
                return true;

            string[] d = yetkiTipiDegeri.Split(';');

            for (int i = 0; i < d.Length; i++)
            {
                string[] muhasebeBirim = d[i].Split('-');

                if (muhasebeBirim.Length != 2)
                    return false;

                if (OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[0], 0) == OrtakFonksiyonlar.ConvertToInt(muhasebe, 0) || muhasebeBirim[0] == "*")
                {
                    if (birim.Trim() == "")
                        return true;
                    else if (OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[1], 0) == OrtakFonksiyonlar.ConvertToInt(birim, 0) || muhasebeBirim[1] == "*")
                        return true;
                }
            }
            return false;
        }

        public static bool MuhasebeBirimYetkiVarMiSTF(Kullanici kullanan, string muhasebe, string birim, ENUMOGY ogy, string stfIslem)
        {
            if (MuhasebeBirimYetkiVarMi(kullanan, muhasebe, birim))
            {
                if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                {
                    return true;
                }
                if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MUHASEBEYONETICISI))
                {
                    return true;
                }

                if (string.IsNullOrWhiteSpace(stfIslem)) return true;
                string[] stfIslemParcalar = stfIslem.Split(',', ';');
                int yetkiKod;
                if (ogy == ENUMOGY.Onaylama)
                    yetkiKod = 86;
                else if (ogy == ENUMOGY.Yazma)
                    yetkiKod = 84;
                else yetkiKod = 85;

                string yetki = kullanan.YetkiTipiDegeriBul(yetkiKod);
                if (string.IsNullOrWhiteSpace(yetki) && stfIslem == "01") return true;
                foreach (string item in stfIslemParcalar)
                {
                    if (ParcayaYetkiVar(yetki, item)) return true;
                }
            }
            return false;
        }

        private static bool ParcayaYetkiVar(string yetki, string item)
        {
            string[] yetkiParcalar = yetki.Split(',', ';');
            foreach (string icItem in yetkiParcalar)
            {
                if (item == icItem.Trim()) return true;
            }
            return false;
        }

        /// <summary>
        /// Arguman olarak verilen kullanıcının sayfaya giriş izni olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <returns>Kullanıcının sayfaya giriş izni olup olmadığını bool olarak döner</returns>
        public static bool SayfayaGirisYapabilirMi(Kullanici kullanan)
        {
            return YetkisiVarMi(kullanan, (int)ENUMOGY.Gorme, (int)TNS.MUH.ENUMBelgeTuru.ODEMEEMRI);
        }

        public static bool SayfayaGirisYapabilirMi(Kullanici kullanan, ENUMKullaniciYetkiTipi kullaniciYetkiTipi)
        {
            if (OrtakServis.UlusalAjansMi())
                return YetkisiVarMi(kullanan, ENUMYetkiTipi.GOREBILIR, kullaniciYetkiTipi);
            else
                return SayfayaGirisYapabilirMi(kullanan);
        }

        public static bool SayfayaGirisYapabilirMi(Kullanici kullanan, ENUMKullaniciYetkiTipi kullaniciYetkiTipi, string muhasebe, string birim)
        {
            return YetkisiVarMi(kullanan, ENUMYetkiTipi.GOREBILIR, kullaniciYetkiTipi, muhasebe, birim);
        }

        /// <summary>
        /// Arguman olarak verilen kullanıcının sayfaya giriş izni olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <returns>Kullanıcının sayfaya giriş izni olup olmadığını bool olarak döner</returns>
        public static bool SayfayaGirisYapabilirMi(Kullanici kullanan, string muhasebe, string birim)
        {
            return YetkisiVarMi(kullanan, (int)ENUMOGY.Gorme, (int)TNS.MUH.ENUMBelgeTuru.ODEMEEMRI, muhasebe, birim);
        }

        /// <summary>
        /// Arguman olarak verilen kullanıcının görme yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <returns>Kullanıcının görme yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan)
        {
            return YetkisiVarMi(kullanan, (int)ENUMOGY.Gorme, (int)TNS.MUH.ENUMBelgeTuru.ODEMEEMRI);
        }

        /// <summary>
        /// Arguman olarak verilen kullanıcının görme yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <returns>Kullanıcının görme yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, string muhasebe, string birim)
        {
            return YetkisiVarMi(kullanan, (int)ENUMOGY.Gorme, (int)TNS.MUH.ENUMBelgeTuru.ODEMEEMRI, muhasebe, birim);
        }

        public static bool GormeYetkisiVarMi(Kullanici kullanan, ENUMKullaniciYetkiTipi kullaniciYetkiTipi)
        {
            return YetkisiVarMi(kullanan, ENUMYetkiTipi.GOREBILIR, kullaniciYetkiTipi);
        }

        public static bool GormeYetkisiVarMi(Kullanici kullanan, ENUMKullaniciYetkiTipi kullaniciYetkiTipi, string muhasebe, string birim)
        {
            return YetkisiVarMi(kullanan, ENUMYetkiTipi.GOREBILIR, kullaniciYetkiTipi, muhasebe, birim);
        }

        /// <summary>
        /// Arguman olarak verilen kullanıcının yazma yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <returns>Kullanıcının yazma yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            return YetkisiVarMi(kullanan, (int)ENUMOGY.Yazma, (int)TNS.MUH.ENUMBelgeTuru.ODEMEEMRI);
        }

        /// <summary>
        /// Arguman olarak verilen kullanıcının yazma yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <returns>Kullanıcının yazma yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, string muhasebe, string birim)
        {
            return YetkisiVarMi(kullanan, (int)ENUMOGY.Yazma, (int)TNS.MUH.ENUMBelgeTuru.ODEMEEMRI, muhasebe, birim);
        }

        public static bool YazmaYetkisiVarMi(Kullanici kullanan, ENUMKullaniciYetkiTipi kullaniciYetkiTipi)
        {
            return YetkisiVarMi(kullanan, ENUMYetkiTipi.YAPABILIR, kullaniciYetkiTipi);
        }

        public static bool YazmaYetkisiVarMi(Kullanici kullanan, ENUMKullaniciYetkiTipi kullaniciYetkiTipi, string muhasebe, string birim)
        {
            return YetkisiVarMi(kullanan, ENUMYetkiTipi.YAPABILIR, kullaniciYetkiTipi, muhasebe, birim);
        }

        public static void KullanicininYetkiliOlduguMuhasebeBirim(Kullanici kullanan, ref int muhasebe, ref string gelenBirim)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MUHASEBEYONETICISI))
                return;

            int calistigiMuhasebe = OrtakFonksiyonlar.ConvertToInt(kullanan.calistigiKurum, 0);

            //Kullanıcı yöneticisi ise sadece muhasebe set edilicek
            //Milli emlak gazi bey kullanıcı yöneticisi ve harcama süreçlerinde taşra birimlerinin süreçlerini görmek istiyor
            //if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
            //    return;

            string birimListe = "";
            string muhasebeListe = "";

            string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Ödeme Emri/MİF Saymanlık-Birim Yetkisi");
            if (yetkiTipiDegeri != "")
            {
                ObjectArray d = new ObjectArray();
                int parca = OrtakFonksiyonlar.StringiParcala(yetkiTipiDegeri, ";", d);
                foreach (string yetkiTipiDegeriParca in d.objeler)
                {
                    string[] muhasebeBirim = yetkiTipiDegeriParca.Split('-');
                    if (muhasebeBirim.Length == 2)
                    {
                        if (muhasebeBirim[0].Trim() != "*")
                        {
                            if (muhasebeListe != "")
                                muhasebeListe += ";";
                            muhasebeListe += OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[0], 0).ToString();
                        }

                        if (muhasebeBirim[1].Trim() != "*")
                        {
                            if (gelenBirim.Trim() == "")
                            {
                                //kullanicinin birden fazla birime yetkisi varsa ve bu birimlerden sadece biri için listeleme yapmak istiyorsa birim değerini direk alıyoruz.
                                //eğer kullanicinin yazdığı birim için yetkisi yok ise listele bölümündeki HRCYetkiKontrol.MuhasebeBirimYetkiVarMi kodu ile izni olmayan birimlerin süreçlerini göremeyecek.
                                if (birimListe != "") birimListe += ";";
                                birimListe += muhasebeBirim[1];
                            }
                        }
                        else
                        {
                            if (gelenBirim.Trim() != "")
                            {
                                if (birimListe != "") birimListe += ";";
                                birimListe += gelenBirim;
                            }
                        }
                    }
                }
            }
            else if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
            {
                yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Taşınır Muhasebe Harcama Birim Yetkisi");
                if (yetkiTipiDegeri != "")
                {
                    ObjectArray d = new ObjectArray();
                    int parca = OrtakFonksiyonlar.StringiParcala(yetkiTipiDegeri, ";", d);
                    foreach (string yetkiTipiDegeriParca in d.objeler)
                    {
                        string[] muhasebeBirim = yetkiTipiDegeriParca.Split('-');
                        if (muhasebeBirim.Length == 3)
                        {
                            if (muhasebeBirim[0].Trim() != "*")
                            {
                                if (muhasebeListe != "")
                                    muhasebeListe += ";";
                                muhasebeListe += OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[0], 0).ToString();
                            }

                            if (muhasebeBirim[1].Trim() != "*")
                            {
                                if (gelenBirim.Trim() == "")
                                {
                                    //kullanicinin birden fazla birime yetkisi varsa ve bu birimlerden sadece biri için listeleme yapmak istiyorsa birim değerini direk alıyoruz.
                                    //eğer kullanicinin yazdığı birim için yetkisi yok ise listele bölümündeki HRCYetkiKontrol.MuhasebeBirimYetkiVarMi kodu ile izni olmayan birimlerin süreçlerini göremeyecek.
                                    if (birimListe != "") birimListe += ";";
                                    if (muhasebeBirim[1].Length > 10)
                                        birimListe += muhasebeBirim[1].Replace(".", "").Substring(8, 3);//Son 3 hanesi
                                    else
                                        birimListe += "999";
                                }
                            }
                            else
                            {
                                if (gelenBirim.Trim() != "")
                                {
                                    if (birimListe != "") birimListe += ";";
                                    birimListe += gelenBirim;
                                }
                            }
                        }
                    }
                }
            }
            if (birimListe != "")
                gelenBirim = birimListe;
            if (birimListe == "")
                muhasebe = calistigiMuhasebe;
        }

        public static bool ParcaYetkisiVarmi(Kullanici kullanan, ENUMOGY ogy, int parcaKod)
        {
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.HARCAMAISLEMLERI))
                return true;
            else if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MUHASEBEYONETICISI))
                return true;
            else if (OrtakServis.UlusalAjansMi())
                return true;

            string yetkiKod;
            if (ogy == ENUMOGY.Onaylama)
                yetkiKod = "Onaylayabileceği Adımlar";
            else if (ogy == ENUMOGY.Yazma)
                yetkiKod = "Yapabileceği Adımlar";
            else yetkiKod = "Görebileceği Adımlar";

            string yetki = kullanan.YetkiTipiDegeriBul(yetkiKod);

            if (ParcayaYetkiVar(yetki, parcaKod.ToString())) return true;

            return false;
        }
    }

    /// <summary>
    /// BordroTabloDetay: Bordro Tablo Değerleri
    /// </summary>
    [System.Serializable()]
    public class BordroTabloAna
    {
        public enum ENUMBordroTabloTur : int
        {
            UnvanDerece = 1,
            DereceKademe = 2
        }

        public BordroTabloAna()
        {
        }
        /// <summary>
        /// Kod: Tablo Kodu
        /// </summary>
        public int kod { get; set; }

        /// <summary>
        /// Ad: Tablo Adı
        /// </summary>
        public string ad { get; set; }

        /// <summary>
        /// Tur: Tablo Turu
        /// </summary>
        public ENUMBordroTabloTur tur { get; set; }
    }

    /// <summary>
    /// BordroTabloDetay: Bordro Tablo Değerleri
    /// </summary>
    [System.Serializable()]
    public class BordroTabloDetay
    {
        public int yil;

        public BordroTabloDetay()
        {
        }

        public BordroTabloDetay(BordroTabloDetay onceki)
        {
            if (onceki != null)
            {
                kod = onceki.kod;
                ad = onceki.ad;
                unvanKod = onceki.unvanKod;
                unvanAd = onceki.unvanAd;
                derece = onceki.derece;
                deger = onceki.deger;
            }
        }

        /// <summary>
        /// Kod: Tablo Kodu
        /// </summary>
        public int kod { get; set; }

        /// <summary>
        /// Ad: Tablo Adı
        /// </summary>
        public string ad { get; set; }

        /// <summary>
        /// Unvan Kod: Unvan Kod
        /// </summary>
        public int unvanKod { get; set; }

        /// <summary>
        /// Ad: Tablo Adı
        /// </summary>
        public string unvanAd { get; set; }

        /// <summary>
        /// Derece: Derece
        /// </summary>
        public int derece { get; set; }

        /// <summary>
        /// Deger: Değer
        /// </summary>
        public decimal deger { get; set; }

        public decimal carpan { get; set; }
    }

    [Serializable()]
    public class BordroMail
    {
        public int surecYil;
        public string surecNo;
        public int bordroTur;
        public int bordroIslemTur;
        public string tc;
        public string ePosta;
        public string kullaniciKodu;
        public TNSDateTime eklemeZamani;
        public TNSDateTime sonucZamani;
        public string sonucStr;
        public string ad;
        public string soyad;
        public string isinAdi;
    }

    /// <summary>
    /// Harcama Yönetimi modulü içerisinde yapılabilecek işlemleri içeren arayüz
    /// </summary>
    public interface IHRCServis
    {
        /// <summary>
        /// Harcama Yönetiminde kullanılacak İşlem bilgisini veritabanına kaydeder
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek olan Is nesnesi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IsKaydet(Kullanici kullanan, Is bilgi);

        /// <summary>
        /// Harcama Yönetiminde kullanılan İşlem bilgisini veritabanından okur 
        /// ve <see cref="Is"/> nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan"><see cref="Kullanici"/> bilgilerini içeren nesne</param>
        /// <param name="bilgi">Listelemede kriter olarak kullanılacak değerleri içeren <see cref="Is"/> nesnesi</param>
        /// <returns>İstenilen kriterdeki işlem listesini <see cref="Is"/> nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray IsListele(Kullanici kullanan, Is bilgi);
        /// <summary>
        /// Harcama Yönetiminde kullanılan İşlem bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IsSil(Kullanici kullanan, Is bilgi);
        /// <summary>
        /// Harcama Yönetiminde kullanılan parça bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc ParcaKaydet(Kullanici kullanan, Parca bilgi);
        /// <summary>
        /// Harcama Yönetiminde kullanılan parça bilgisini veritabanından okur 
        /// ve Parca nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki parça listesini Parca nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray ParcaListele(Kullanici kullanan, Parca kriter);
        /// <summary>
        /// Harcama Yönetiminde kullanılan parça bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc ParcaSil(Kullanici kullanan, Parca bilgi);
        /// <summary>
        /// İlişkilendirilmiş işlem ve parça bilgilerini veritabanına kaydeder ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek işlem ve parça bilgilerini tutan IsParca nesnesi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IsParcaKaydet(Kullanici kullanan, IsParca bilgi);
        /// <summary>
        /// İlişkilendirilmiş işlem ve parça bilgilerini veritabanından okur ve IsParca nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="isKod">Parça bilgileri listelenecek olan işlem kodu bilgisi</param>
        /// <returns>Verilen işlem koduna ait parça bilgilerini tutan nesne</returns>
        IsParca IsParcaListele(Kullanici kullanan, int isKod);
        /// <summary>
        /// İlişkilendirilmiş ihale ve parça bilgilerini veritabanından okur ve IsParca nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="isKod">İhaleye türüne göre parça bilgileri listelenecek olan işlem kodu</param>
        /// <returns>Verilen işlem koduna ait ihale parça bilgilerini tutan nesne</returns>
        IsParca IsIhaleParcaListele(Kullanici kullanan, int isKod);
        /// <summary>
        /// İlişkilendirilmiş ihale ve parça bilgilerini veritabanına kaydeder ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek parça bilgilerini tutan IhaleParca nesnesi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleParcaKaydet(Kullanici kullanan, IhaleParca bilgi);
        /// <summary>
        /// İlişkilendirilmiş ihale ve parça bilgilerini veritabanına kaydeder ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek parça bilgilerini IhaleParcaBag nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleParcaGuncelle(Kullanici kullanan, ObjectArray bilgi);
        /// <summary>
        /// Verilen kriterlere bağlı ilişkili parçaları veritabanından okur ve IhaleParca nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ihaleUsul">Listelemede kriter olarak kullanılacak ihalu usul kodu</param>
        /// <param name="ihaleMadde">Listelemede kriter olarak kullanılacak ihale madde kodu</param>
        /// <param name="komisyon">Listelemede kriter olarak kullanılacak komisyon var mı yok mu bilgisi</param>
        /// <param name="tur">Listelemede kriter olarak kullanılacak işlem türü bilgisi</param>
        /// <returns>Verilen kriterlere ait ihale parça bilgilerini tutan nesne</returns>
        IhaleParca IhaleParcaListele(Kullanici kullanan, int ihaleUsul, int ihaleMadde, int komisyon, int tur);
        /// <summary>
        /// Güncelleme servisinde kullanılmak üzere ilişkilendirilmiş tüm ihale ve parça listesini veritabanından okur
        /// ve IhaleParcaBag nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns>İlişkilendirilmiş tüm ihale ve parça listesini IhaleParcaBag nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray IhaleParcaTumListe(Kullanici kullanan);
        /// <summary>
        /// İhalede kullanılacak sözleşme metnini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek sözleşme metnine ait bilgileri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleSozlesmeTanimKaydet(Kullanici kullanan, IhaleSozlesmeTanim bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihalede kullanılacak sözleşme metnine ait bilgileri veritabanından okur 
        /// ve IhaleSozlesmeTanim nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihalede kullanılacak sözleşme metnine ait bilgiyi IhaleSozlesmeTanim nesnesi olarak tutan nesne</returns>
        IhaleSozlesmeTanim IhaleSozlesmeTanimListele(Kullanici kullanan, IhaleSozlesmeTanim bilgi);
        /// <summary>
        /// Arguman olarak verilen, ihalede kullanılacak sözleşme metnine ait bilgileri veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner 
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgileri tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleSozlesmeKaydet(Kullanici kullanan, IhaleSozlesme bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihalede kullanılacak sözleşme metnine ait bilgileri veritabanından okur 
        /// ve IhaleSozlesme nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale sözleşme bilgisini IhaleSozlesme nesnesi olarak tutan nesne</returns>
        IhaleSozlesme IhaleSozlesmeListele(Kullanici kullanan, IhaleSozlesme kriter);
        /// <summary>
        /// Arguman olarak verilen, öğrenci eğitmen bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc OgrenciEgitmenKaydet(Kullanici kullanan, OgrenciEgitmen bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, Öğrenci ve Eğitmen bilgilerini veritabanından okur 
        /// ve OgrenciEgitmen nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren OgrenciEgitmen nesnesi</param>
        /// <returns>İstenilen kriterdeki Öğrenci ve Eğitmen bilgileri listesini OgrenciEgitmen nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray OgrenciEgitmenListele(Kullanici kullanan, OgrenciEgitmen kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, Ödenek Gönderme Belgesi gönderilecek mailleri veritabanından okur
        /// ve AboneOdenekGondermeMail nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="mail">Listelemede kriter olarak kullanılacak değerleri içeren AboneOdenekGondermeMail nesnesi</param>
        /// <returns>İstenilen kriterdeki Ödenek Gönderme Mail listesini AboneOdenekGondermeMail nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray AboneOdenekGondermeMailListele(AboneOdenekGondermeMail mail);
        /// <summary>
        /// Arguman olarak verilen, Ödenek Gönderme Mail bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="mail">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc AboneOdenekGondermeMailKaydet(Kullanici kullanan, AboneOdenekGondermeMail mail);
        /// <summary>
        /// Arguman olarak verilen, Ödenek Gönderme Mail bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="mail">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc AboneOdenekGondermeMailSil(Kullanici kullanan, AboneOdenekGondermeMail mail);
        /// <summary>
        /// Arguman olarak verilen kriterlere ait mail adreslerine Ödenek Gönderme Belgesini mail olarak gönderir
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi</param>
        /// <param name="aboneTip">İşlem yapılacak abone tipi bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc AboneOdenekGondermeMailAt(Kullanici kullanan, int yil, int ay, int aboneTip);
        /// <summary>
        /// Arguman olarak verilen, abone fatura bilgisini içeren AboneFatura nesne koleksiyonunu veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Kaydedilecek bilgileri AboneFatura nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc AboneFaturaKaydet(Kullanici kullanan, ObjectArray liste);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, abone fatura bilgisini veritabanından okur 
        /// ve AboneFatura nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, abone fatura listesini AboneFatura nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray AboneFaturaListele(Kullanici kullanan, AboneFatura kriter);
        /// <summary>
        /// Arguman olarak verilen, abone fatura bilgisine ait Ödeme Emri Belge Numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc AboneFaturaOdemeEmriNoKaydet(Kullanici kullanan, AboneFatura kriter);
        /// <summary>
        /// Arguman olarak verilen, abone fatura bilgisine ait gsm ödeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc AboneFaturaGsmBilgiKaydet(Kullanici kullanan, AboneFatura kriter); /// <summary>
                                                                                 /// Arguman olarak verilen kriterlere bağlı olarak, abone fatura istatistik bilgisini veritabanından okur 
                                                                                 /// ve AboneFaturaIstatistikSinif nesne koleksiyonu olarak döner
                                                                                 /// </summary>
                                                                                 /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
                                                                                 /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
                                                                                 /// <returns>İstenilen kriterdeki, abone fatura istatistik bilgisi listesini AboneFaturaIstatistikSinif nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray AboneFaturaListeleIstatistik(Kullanici kullanan, AboneFaturaIstatistikSinif kriter);

        /// <summary>
        /// Arguman olarak verilen, süreç bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SurecKaydet(Kullanici kullanan, Surec bilgi);
        Sonuc SurecKaydet(Kullanici kullanan, Surec bilgi, SurecParca spBilgi);

        /// <summary>
        /// Arguman olarak verilen sürecin açıklama bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SurecKaydetAciklama(Kullanici kullanan, Surec bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak süreç bilgisini veritabanından okur 
        /// ve Surec nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, süreç listesini Surec nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray SurecListele(Kullanici kullanan, Surec kriter);
        ObjectArray SurecDurumListele(Kullanici kullanan, SurecSorguKriter kriter);
        /// <summary>
        /// Arguman olarak verilen sürecin durum bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SurecDurumDegistir(Kullanici kullanan, Surec bilgi);
        Sonuc SurecIslemDegistir(Kullanici kullanan, Surec bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç bilgisini veritabanından okur 
        /// ve Surec nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, süreç listesini Surec nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray SurecSorgula(Kullanici kullanan, SurecSorguKriter kriter);
        ObjectArray SurecMuhMifBaglantiListele(Kullanici kullanan, SurecMuhMifBaglanti kriter, Tertip tertip);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak devam eden süreç bilgisini veritabanından okur 
        /// ve Surec nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, devam eden süreç listesini Surec nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray SurecDevamEdenler(Kullanici kullanan, SurecSorguKriter kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürece ait parçaların durum bilgisini veritabanından okur 
        /// ve SurecParcaDurum nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürece ait parça durum listesini SurecParcaDurum nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray SurecParcaDurumListele(Kullanici kullanan, SurecParcaDurum kriter);
        /// <summary>
        /// Arguman olarak verilen, süreç parçasının durum bilgisini onaylandı olarak veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SurecParcaDurumOnayVer(Kullanici kullanan, SurecParcaDurum bilgi);
        /// <summary>
        /// Arguman olarak verilen, süreç parçasının durum bilgisini devam ediyor ya da tanımsız olarak veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SurecParcaDurumOnayKaldir(Kullanici kullanan, SurecParcaDurum bilgi);
        /// <summary>
        /// Arguman olarak verilen, teklife ait malzeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgileri TeklifMalzeme nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılan sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılan sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TeklifMalzemeKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, teklife ait malzeme bilgisini veritabanından okur 
        /// ve Teklif Malzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, teklife ait malzeme listesini TeklifMalzeme nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TeklifMalzemeListele(Kullanici kullanan, TeklifMalzeme kriter);
        /// <summary>
        /// Sistemde bulunan tüm ölçü birimi bilgisini veritabanından okur 
        /// ve OlcuBirim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns>Sistemde bulunan ölçü birimi bilgisini OlcuBirim nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray OlcuBirimListele(Kullanici kullanan);


        string OlcuBirimAdBul(Kullanici kullanan, int birimKod);

        /// <summary>
        /// Arguman olarak verilen, teklif yapacak firma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgileri TeklifFirma nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılan sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılan sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TeklifFirmaKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, teklif yapacak firma bilgisini veritabanından okur 
        /// ve TeklifFirma nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki teklif yapacak firma listesini TeklifFirma nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TeklifFirmaListele(Kullanici kullanan, TeklifFirma kriter);
        /// <summary>
        /// Arguman olarak verilen, dönem teklif bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Kaydedilecek bilgileri TeklifDonem nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılan sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılan sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TeklifDonemKaydet(Kullanici kullanan, ObjectArray liste, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, dönem teklif bilgisini veritabanından okur 
        /// ve TeklifDonem nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, dönem teklif listesini TeklifDonem nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TeklifDonemListele(Kullanici kullanan, TeklifDonem kriter);
        /// <summary>
        /// Arguman olarak verilen, firmalar tarafından malzemelere yapılan teklif bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Kaydedilecek bilgileri Teklif nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılan sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılan sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TeklifKaydet(Kullanici kullanan, ObjectArray liste, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, firmalara ait malzeme teklif bilgisini veritabanından okur 
        /// ve Teklif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki firmalara ait malzeme teklif bilgisi listesini Teklif nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TeklifListele(Kullanici kullanan, Teklif kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, Kısmi Teklif ile ihale kazanan 1. ve 2. firma bilgisini veritabanından okur 
        /// ve Teklif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki kısmi teklif kazanan firma bilgisi listesini Teklif nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TeklifKismiKazananListele(Kullanici kullanan, Teklif kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, firmalar tarafından teklif verilen malzemelere ait ortalama birim fiyat bilgisini veritabanından okur 
        /// ve Teklif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, firmalar tarafından teklif verilen malzemelere ait ortalama birim fiyat bilgisi listesini Teklif nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TeklifListeleYMOrtBirimFiyat(Kullanici kullanan, Teklif kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, firmanın ihalesini kazandığı malzeme bilgisini veritabanından okur 
        /// ve TeklifMalzeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <param name="firmaNo">İşlem yapılacak firma no bilgisi</param>
        /// <returns>İstenilen kriterdeki, firmanın ihalesini kazandığı malzeme bilgisi listesini TeklifMalzeme nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TeklifKazananFirmaMalzemeListele(Kullanici kullanan, int yil, string surecNo, int firmaNo);

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kazanan firma bilgisini veritabanından okur 
        /// ve TeklifFirma nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <returns>İstenilen kriterdeki, kazanan firma bilgi listesini TeklifFirma nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TeklifKazananFirmaListele(Kullanici kullanan, int yil, string surecNo);

        /// <summary>
        /// Verilen sürecin kazananını Kısmi kazanan yada Kazanan firma olarak bulur.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <param name="malzemeNo">Bu malzemeyi kazanan firma</param>
        /// <returns>İstenilen kriterdeki, kazanan firma bilgi listesini TeklifFirma nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TeklifKazananFirmaListeleGenel(Kullanici kullanan, int yil, string surecNo, int malzemeNo);

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreçte tanımlı ihale için kazanan firma sayısını veritabanından okur ve döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <returns>İstenilen kriterdeki, süreçte tanımlı ihale için kazanan firma sayısı</returns>

        int TeklifKazananFirmaSayisiGetir(Kullanici kullanan, int yil, string surecNo);
        /// <summary>
        /// Arguman olarak verilen, teklif genel toplam bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TeklifGenelToplamKaydet(Kullanici kullanan, TeklifGenelToplam kriter);
        Sonuc TeklifGenelToplamSil(Kullanici kullanan, TeklifGenelToplam kriter, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, teklife ait genel toplam bilgisini veritabanından okur 
        /// ve TeklifGenelToplam nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, teklife ait genel toplam bilgisi listesini TeklifGenelToplam nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TeklifGenelToplamListele(Kullanici kullanan, TeklifGenelToplam kriter);
        /// <summary>
        /// Arguman olarak verilen, Üretici fiyatları Endeksi bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Kaydedilecek bilgileri Ufe nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc UfeKaydet(Kullanici kullanan, ObjectArray liste);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, Üretici Fiyatları Endeksi bilgisini veritabanından okur 
        /// ve Ufe nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, Üretici Fiyatları Endeksi bilgisi listesini Ufe nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray UfeListele(Kullanici kullanan, Ufe kriter);
        /// <summary>
        /// Arguman olarak verilen yıl ve ay kriterlerinden bugune kadarki Üretici Fiyatları Endeksi oranlarını kümülatif toplam olarak veritabanından okur ve döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi</param>
        /// <returns>İstenilen kriterdeki, yıl ve ay kriterlerinden bugune kadarki Üretici Fiyatları Endeksi oranlarının kümülatif toplam bilgisi</returns>
        double UfeToplami(Kullanici kullanan, int yil, int ay);
        /// <summary>
        /// Arguman olarak verilen, komisyon üyeleri bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KomisyonUyeKaydet(Kullanici kullanan, KomisyonUye liste);
        /// <summary>
        /// Arguman olarak verilen, komisyon üyeleri bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KomisyonUyeKaydet(Kullanici kullanan, KomisyonUye liste, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, komisyon üye bilgisini veritabanından okur 
        /// ve KomisyonUye nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, komisyon üye bilgisi listesini tutan KomisyonUye nesnesi</returns>
        KomisyonUye KomisyonUyeListele(Kullanici kullanan, KomisyonUye kriter);
        /// <summary>
        /// Arguman olarak verilen, fatura bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FaturaFormKaydet(Kullanici kullanan, FaturaForm kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fatura bilgisini veritabanından okur 
        /// ve FaturaForm nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, fatura listesini FaturaForm nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray FaturaFormListele(Kullanici kullanan, FaturaForm kriter);

        ObjectArray FaturaFormListeleServis(Kullanici kullanan, FaturaForm kriter);

        ObjectArray FaturaFormListeleIhaleMalzemeli(Kullanici kullanan, FaturaForm kriter);
        /// <summary>
        /// Arguman olarak verilen, faturanın durum bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FaturaFormDurumDegistir(Kullanici kullanan, FaturaForm kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fatura form kontrol bilgisini veritabanından okur 
        /// ve gerekli kontrolleri yaptıktan sonra işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FaturaFormListeleKontrol(Kullanici kullanan, FaturaForm kriter);
        /// <summary>
        /// Arguman olarak verilen, faturaya ait ödeme emri belge numarasını veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FaturaFormOdemeEmriNoKaydet(Kullanici kullanan, FaturaForm kriter);
        /// <summary>
        /// Arguman olarak verilen, taşınır form bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TasinirFormKaydet(Kullanici kullanan, TasinirForm kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, taşınır form bilgisini veritabanından okur 
        /// ve TasinirForm nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, taşınır form listesini TasinirForm nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TasinirFormListele(Kullanici kullanan, TasinirForm kriter);
        /// <summary>
        /// Arguman olarak verilen, taşınır form bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TasinirFormSil(Kullanici kullanan, TasinirForm kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, ödeme emri belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc OdemeEmriFormKaydet(Kullanici kullanan, OdemeEmriMIFForm kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ödeme emri belge bilgisini veritabanından okur 
        /// ve OdemeEmriMIFForm nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ödeme emri belge listesini OdemeEmriMIFForm nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray OdemeEmriFormListele(Kullanici kullanan, OdemeEmriMIFForm kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ödeme emri belgesi miktar toplam bilgisini veritabanından okur 
        /// ve OdemeEmriMIFDetay nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="hesapKod">İşlem yapılacak hesap kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, ödeme emri belgesi miktar toplam bilgisi listesini OdemeEmriMIFDetay nesne koleksiyonu olarak tutan nesne</returns>
        //ObjectArray OdemeEmriMiktarListele(Kullanici kullanan, OdemeEmriMIFForm kriter, string hesapKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ödeme emri belgesi miktar toplam bilgisini veritabanından okur 
        /// ve OdemeEmriMIFDetay nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="hesapKod">İşlem yapılacak hesap kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, ödeme emri belgesi miktar toplam bilgisi listesini OdemeEmriMIFDetay nesne koleksiyonu olarak tutan nesne</returns>
        //ObjectArray OdemeEmriMiktarListele(Kullanici kullanan, OdemeEmriMIFForm kriter, string hesapKod, string tertip);
        /// <summary>
        /// Arguman olarak verilen, ödeme emri belgesinin durumunu iptal olarak veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc OdemeEmriFormIptalEt(Kullanici kullanan, OdemeEmriMIFForm kriter);
        /// <summary>
        /// Arguman olarak verilen, ödeme emri belge bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc OdemeEmriFormSil(Kullanici kullanan, OdemeEmriMIFForm kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, taahhutname bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="tcKimlikNo">TC Kimlik Numarası</param>
        /// <param name="tur">Tür</param>
        /// <param name="tarih">Tarih</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TaahhutnameKaydet(Kullanici kullanan, string tcKimlikNo, int tur, TNSDateTime tarih);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, taahhutname bilgisini veritabanından okur 
        /// ve [tcKimlikNo|tur|tarih] string nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="tcKimlikNo">TC Kimlik Numarası</param>
        /// <param name="tur">Tür</param>
        /// <param name="tarih">Tarih</param>
        /// <returns></returns>
        ObjectArray TaahhutnameListele(string tcKimlikNo, int tur, TNSDateTime tarih);
        /// <summary>
        /// Arguman olarak verilen, firma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FirmaBilgisiKaydet(Kullanici kullanan, FirmaBilgisi kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, firma bilgisini veritabanından okur 
        /// ve FirmaBilgisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, firma bilgisi listesini FirmaBilgisi nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray FirmaBilgisiListele(Kullanici kullanan, FirmaBilgisi kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, firma bilgisini veritabanından okur 
        /// ve FirmaBilgisi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="firmaKod">İşlem yapılacak firma kodu bilgisi</param>
        /// <returns>İstenilen kriterdeki, firma bilgisini FirmaBilgisi nesnesi olarak tutan nesne</returns>
        FirmaBilgisi FirmaBilgisiListele(Kullanici kullanan, int firmaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, firma bilgisini veritabanından okur
        /// ve FirmaBilgisi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="muhasebe">İşlem yapılacak firmanın kayıtlı olduğu muhasebe</param>
        /// <param name="vNo">İşlem yapılacak firma vergi numarası</param>
        /// <returns>
        /// İstenilen kriterdeki, firma bilgisini FirmaBilgisi nesnesi olarak tutan nesne
        /// </returns>
        FirmaBilgisi FirmaBilgisiListele(Kullanici kullanan, int muhasebe, string vNo);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, firma bilgisini veritabanından okur 
        /// ve FirmaBilgisi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, firma bilgisini FirmaBilgisi nesnesi olarak tutan nesne</returns>
        FirmaBilgisi FirmaGirisListele(Kullanici kullanan, FirmaBilgisi kriter);
        /// <summary>
        /// Arguman olarak verilen, firmanın sistemi giriş bilgisini veritabanına kaydeder,
        /// eğer firma sisteme kayıtlı ve parola bilgisi yoksa ya da yeni parola talebi varsa 
        /// firma yetkilisine mail olarak parola gönderir
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="yeniParola">Eğer <c>true</c> verilirse firma yetkilisine mail yoluyla parola gönderilir</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FirmaGirisKaydet(Kullanici kullanan, FirmaBilgisi kriter, bool yeniParola);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ilgili firmanın sisteme giriş bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="vergiNo">İşlem yapılacak firmaya ait vergi numarası</param>
        /// <param name="parola">İşlem yapılacak firmaya ait parola</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FirmaGirisYap(string vergiNo, string parola);
        /// <summary>
        /// Arguman olarak verilen, firma parola bilgisinin geçerliliğini kontrol eder, geçerliyse veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="eskiParola">İşlem yapılacak firmaya ait eski parola</param>
        /// <param name="yeniParola1">İşlem yapılacak firmaya ait yeni parola</param>
        /// <param name="yeniParola2">İşlem yapılacak firmaya ait yeni parola tekrarı</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FirmaParolaDegistir(Kullanici kullanan, string eskiParola, string yeniParola1, string yeniParola2);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, firmanın yaptığı işleri veritabanından okur 
        /// ve Teklif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="vNo">İşlem yapılacak firmaya ait vergi numarası</param>
        /// <param name="sadeceDevamEdenler">Eğer <c>true</c> verilirse firmaya ait devam işler listelenir</param>
        /// <returns>İstenilen kriterdeki, firma işleri listesini Teklif nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray FirmaninIsleri(Kullanici kullanan, int yil, string vNo, bool sadeceDevamEdenler);
        /// <summary>
        /// Arguman olarak verilen, kullanıcı koduna ait firmanın sistem son giriş tarihi bilgisini veritabanına kaydeder
        /// </summary>
        /// <param name="kullaniciKodu">İşlem yapılacak firmaya ait kullanıcı kodu</param>
        void FirmaGirisTarihKaydet(string kullaniciKodu);

        /// <summary>
        /// Arguman olarak verilen, onay form bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        TNS.MUH.EksikOdenekler OnayFormKaydet(Kullanici kullanan, OnayForm kriter, int islemKod, int parcaKod);

        /// <summary>
        /// Arguman olarak verilen, onay form evrak sayı bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns></returns>
        Sonuc OnayFormEvrakSayiKaydet(Kullanici kullanan, OnayForm kriter);

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, onay form belge bilgisini veritabanından okur 
        /// ve OnayForm nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, onay form belge listesini OnayForm nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray OnayFormListele(Kullanici kullanan, OnayForm kriter);

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, onay form belge tertip bilgisini veritabanından okur 
        /// ve OnayFormTertip nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, onay form belge tertip listesini OnayFormTertip nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray OnayFormTertipListele(Kullanici kullanan, OnayFormTertip kriter);

        Sonuc BlokeTertipleriOtomatikGuncelle(Kullanici kullanan, int surecYil, string surecNo, int tertipYil, ENUMKontrolDurum kontrol);

        ObjectArray BlokeTertipListele(Kullanici kullanan, OnayFormTertip kriter);

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, onay form belge tertip bilgisini veritabanından okur 
        /// ve OnayFormTertip nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, onay form belge tertip listesini OnayFormTertip nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray OnayFormTertipBenzerListele(Kullanici kullanan, OnayFormTertip kriter);

        /// <summary>
        /// Arguman olarak verilen, onay form belge tertip bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc OnayFormTertipGuncelle(Kullanici kullanan, OnayFormTertip kriter);

        Sonuc OnayFormTertipKaydet(Kullanici kullanan, OnayFormTertip kriter);

        /// <summary>
        /// Arguman olarak verilen, onay form belge tertibine ait bloke edilen tutar bilgisini boş olarak veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc BlokeKaldirTertip(Kullanici kullanan, OnayFormTertip kriter);

        Sonuc BlokeEkleTertip(Kullanici kullanan, OnayFormTertip kriter);
        /// <summary>
        /// Arguman olarak verilen, kiralama bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>

        Sonuc KiralamaKaydet(Kullanici kullanan, Kiralama kriter, int islemKod, int parcaKod);

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kiralama bilgisini veritabanından okur 
        /// ve Kiralama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, kiralama listesini Kiralama nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray KiralamaListele(Kullanici kullanan, Kiralama kriter);

        /// <summary>
        /// Arguman olarak verilen, tek kaynaktan temin bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TekKaynaktanTeminKaydet(Kullanici kullanan, TekKaynaktanTemin kriter);

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, tek kaynaktan temin bilgisini veritabanından okur 
        /// ve TekKaynaktanTemin nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, tek kaynaktan temin bilgi listesini TekKaynaktanTemin nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TekKaynaktanTeminListele(Kullanici kullanan, TekKaynaktanTemin kriter);

        /// <summary>
        /// Arguman olarak verilen, para birimi bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc ParaBirimiKaydet(Kullanici kullanan, ParaBirimi bilgi);

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, para birimi bilgisini veritabanından okur 
        /// ve ParaBirimi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, para birimi listesini ParaBirimi nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray ParaBirimiListele(Kullanici kullanan, ParaBirimi kriter);

        /// <summary>
        /// Arguman olarak verilen, para birimi bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc ParaBirimiSil(Kullanici kullanan, ParaBirimi bilgi);

        /// <summary>
        /// Arguman olarak verilen, ihale tür tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Kaydedilecek bilgileri IhaleTur nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>

        Sonuc IhaleTurKaydet(Kullanici kullanan, ObjectArray liste);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale tür tanım bilgisini veritabanından okur 
        /// ve IhaleTur nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale tür tanım bilgisi listesini IhaleTur nesne koleksiyonu olarak tutan nesne</returns>

        ObjectArray IhaleTurListele(Kullanici kullanan, IhaleTur kriter);
        /// <summary>
        /// Arguman olarak verilen, ihale türüne ait madde tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Kaydedilecek bilgileri IhaleTurMadde nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>

        Sonuc IhaleTurMaddeKaydet(Kullanici kullanan, ObjectArray liste);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale türüne ait madde tanım bilgisini veritabanından okur 
        /// ve IhaleTurMadde nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ihaleKod">İşlem yapılacak ihale tür kodu</param>
        /// <returns>İstenilen kriterdeki, ihale türüne ait madde tanım bilgisi listesini IhaleTurMadde nesne koleksiyonu olarak tutan nesne</returns>

        ObjectArray IhaleTurMaddeListele(Kullanici kullanan, int ihaleKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale türüne ait madde tanım bilgisini veritabanından okur 
        /// ve IhaleTurMadde nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ihaleKod">İşlem yapılacak ihale tür kodu</param>
        /// <param name="maddeKod">İşlem yapılacak ihale madde kodu</param>
        /// <returns>İstenilen kriterdeki, ihale türüne ait madde tanım bilgisini IhaleTurMadde nesnesi olarak tutan nesne</returns>
        IhaleTurMadde IhaleUsulMaddeAl(Kullanici kullanan, int ihaleKod, int maddeKod);
        /// <summary>
        /// Arguman olarak verilen, ihale türüne ait ilan tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Kaydedilecek bilgileri IhaleIlanTanim nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleIlanKaydet(Kullanici kullanan, ObjectArray liste);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale türüne ait ilan tanım bilgisini veritabanından okur 
        /// ve IhaleIlanTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ihaleKod">İşlem yapılacak ihale tür kodu</param>
        /// <param name="ihaleMaddeSira">İşlem yapılacak ihale türüne ait madde kodu</param>
        /// <returns>İstenilen kriterdeki, ihale türüne ait ilan tanım bilgisi listesini IhaleIlanTanim nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray IhaleIlanListele(Kullanici kullanan, int ihaleKod, int ihaleMaddeSira);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale türüne ait ilan tanım bilgisini veritabanından okur 
        /// ve IhaleIlanTanim nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ihaleKod">İşlem yapılacak ihale tür kodu</param>
        /// <param name="fiyat">İşlem yapılacak fiyat bilgisi</param>
        /// <param name="isTur">İşlem yapılacak süreç işlem kodu</param>
        /// <returns>İstenilen kriterdeki, ihale türüne ait ilan tanım bilgisini IhaleIlanTanim nesnesi olarak tutan nesne</returns>
        IhaleIlanTanim IhaleIlanSekliAl(Kullanici kullanan, int ihaleKod, double fiyat, int isTur);
        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleIlanBilgisiKaydet(Kullanici kullanan, IhaleIlanBilgisi bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale ilan bilgisini veritabanından okur 
        /// ve IhaleIlanBilgisi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale ilan bilgisini IhaleIlanBilgisi nesnesi olarak tutan nesne</returns>
        IhaleIlanBilgisi IhaleIlanBilgisiListele(Kullanici kullanan, IhaleIlanBilgisi kriter);
        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisine ait belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgileri IhaleIlanBilgisiBelge nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleIlanBilgisiBelgeKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisine ait belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belge">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleIlanBilgisiBelgeKaydet(Kullanici kullanan, IhaleIlanBilgisiBelge belge, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisine ait belge bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belge">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleIlanBilgisiBelgeSil(Kullanici kullanan, IhaleIlanBilgisiBelge belge, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisine ait belge bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek bilgileri IhaleIlanBilgisiBelge nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleIlanBilgisiBelgeSil(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale ilan bilgisine ait belge bilgisini veritabanından okur 
        /// ve IhaleIlanBilgisiBelge nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale ilan bilgisine ait belge bilgisi listesini IhaleIlanBilgisiBelge nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray IhaleIlanBilgisiBelgeListele(Kullanici kullanan, IhaleIlanBilgisiBelge kriter);
        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisine ait belgenin sıra no bilgisini, belge listesinde bir üst sırada görünecek şekilde veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belge">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleIlanBilgisiBelgeYukariTasi(Kullanici kullanan, IhaleIlanBilgisiBelge belge, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisine ait belgenin sıra no bilgisini, belge listesinde bir alt sırada görünecek şekilde veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belge">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleIlanBilgisiBelgeAsagiTasi(Kullanici kullanan, IhaleIlanBilgisiBelge belge, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, sayfa yardım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yardim">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc YardimKaydet(Kullanici kullanan, Yardim yardim);
        /// <summary>
        /// Arguman olarak verilen, sayfa yardım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="list">Kaydedilecek bilgileri Yardim nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc YardimGuncelleme(Kullanici kullanan, ObjectArray list);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sayfa yardım bilgisini veritabanından okur 
        /// ve Yardım nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yardim">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sayfa yardım bilgisini Yardim nesnesi olarak tutan nesne</returns>
        Yardim YardimGoster(Kullanici kullanan, Yardim yardim);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sayfa yardım bilgisini veritabanından okur 
        /// ve Yardim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yardim">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sayfa yardım listesini Yardim nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray YardimListele(Kullanici kullanan, Yardim yardim);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, tertibe ait bütçe gider bilgisini veritabanından okur 
        /// ve ButceGider nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu</param>
        /// <returns>İstenilen kriterdeki, tertibe ait bütçe gider bilgisi listesini ButceGider nesne koleksiyonu olarak tutan nesne</returns>
        ///ObjectArray YuzdeOnRakamiVer(int yil, ObjectArray tertip);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, tertip bilgisini veritabanından okur 
        /// ve ButceGider nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, tertip bilgi listesini ButceGider nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TertipListele(int yil, string kur, string fon, string fin, string eko);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, harcama bilgisini veritabanından okur 
        /// ve ButceHarcama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, harcama bilgi listesini ButceHarcama nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray HarcamaListele(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ödenek gönderme belge bilgisini veritabanından okur 
        /// ve ButceGider nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <param name="durum">İşlem yapılacak ödenek gönderme belgesinin durum bilgisi</param>
        /// <returns>İstenilen kriterdeki, ödenek gönderme belge bilgi listesini ButceGider nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray OdenekGondermeListele(int yil, int muhasebe, string birim, string kur, string fon, string fin, string eko, int durum);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kullanılabilir ödenek bilgisini veritabanından okur 
        /// ve ButceGider nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="kontrolTur">İşlem yapılacak kontrol tür bilgisi (Ödenek Gönderme, Ödenek)</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <returns></returns>
        ObjectArray KullanilabilirOdenek(int yil, int kontrolTur, int muhasebe, string birim, string kur, string fon, string fin, string eko);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kurumsal kod bilgisini veritabanından okur 
        /// ve string olarak döner
        /// </summary>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <returns></returns>
        string MuhasebeBirimKurumsalKodAl(int muhasebe, string birim);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, muhasebe ve birim bilgisini veritabanından okur 
        /// ve MuhasebeBirim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <param name="kurKod">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, muhasebe ve birim bilgisi listesini MuhasebeBirim nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray MuhasebeBirimListele(int muhasebe, string birim, string kurKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç için geçerli olan ekonomik kod bilgisini veritabanından okur 
        /// ve string olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <returns>İstenilen kriterdeki, süreç için geçerli olan ekonomik kod bilgisini string olarak döner</returns>
        string SurecEkonomikKodAl(Kullanici kullanan, int yil, string surecNo);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, muhasebe ve birime ait tertip bilgisini veritabanından okur 
        /// ve MuhasebeBirimTertip nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, muhasebe ve birime ait tertip bilgisi listesini MuhasebeBirimTertip nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray MuhasebeBirimTertipListele(int muhasebe, string birim, string eko);
        /// <summary>
        /// Arguman olarak verilen, muhasebe ve birime ait tertip bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tertip">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc MuhasebeBirimTertipKaydet(Kullanici kullanan, MuhasebeBirimTertip tertip);
        /// <summary>
        /// Arguman olarak verilen, muhasebe ve birime ait tertip bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tertip">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc MuhasebeBirimTertipSil(Kullanici kullanan, MuhasebeBirimTertip tertip);
        /// <summary>
        /// Arguman olarak verilen, idare bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IdareBilgiKaydet(Kullanici kullanan, IdareBilgi bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, idare bilgisini veritabanından okur 
        /// ve IdareBilgi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, idare bilgisini IdareBilgi nesnesi olarak tutan nesne</returns>
        IdareBilgi IdareBilgiListele(Kullanici kullanan, IdareBilgi kriter);
        IdareBilgi KisiIdareBilgiBul(Kullanici kullanan);

        /// <summary>
        /// Arguman olarak verilen, harcama pusulası bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HarcamaPusulasiSil(Kullanici kullanan, FaturaForm bilgi);
        /// <summary>
        /// Arguman olarak verilen, ihale numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleNumarasiKaydet(Kullanici kullanan, IhaleNumarasi kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale numarası bilgisini veritabanından okur 
        /// ve IhaleNumarasi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale numarası bilgisini IhaleNumarası nesnesi olarak tutan nesne</returns>
        IhaleNumarasi IhaleNumarasiListele(Kullanici kullanan, IhaleNumarasi kriter);
        /// <summary>
        /// Arguman olarak verilen, harcama talimatı bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HarcamaTalimatiKaydet(Kullanici kullanan, HarcamaTalimati kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, harcama talimatı bilgisini veritabanından okur 
        /// ve HarcamaTalimatı nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, harcama talimatı bilgisi listesini HarcamaTalimati nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray HarcamaTalimatiListele(Kullanici kullanan, HarcamaTalimati kriter);
        /// <summary>
        /// Arguman olarak verilen, hesap cetveli bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HesapCetveliKaydet(Kullanici kullanan, HesapCetveli liste, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hesap cetveli bilgisini veritabanından okur 
        /// ve HesapCetveli nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, hesap cetveli bilgisini HesapCetveli nesnesi olarak tutan nesne</returns>
        HesapCetveli HesapCetveliListele(Kullanici kullanan, HesapCetveli kriter);
        /// <summary>
        /// Arguman olarak verilen, ihale dosyası içerik tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleDosyasiIcerikKaydet(Kullanici kullanan, ObjectArray kriter);
        /// <summary>
        /// Arguman olarak verilen, ihale dosyası içerik tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgileri IhaleDosyasiIcerik nesne koleksiyonu olarak tutan nesne</param>
        /// <returns></returns>
        Sonuc IhaleDosyasiIcerikListeKaydet(Kullanici kullanan, ObjectArray kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale dosyası içerik tanım bilgisini veritabanından okur 
        /// ve IhaleDosyasiIcerik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale dosyası içerik tanım bilgisi listesini IhaleDosyasiIcerik nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray IhaleDosyasiIcerikListele(Kullanici kullanan, IhaleDosyasiIcerik kriter);
        /// <summary>
        /// Arguman olarak verilen, sürece bağlı ihale dosyası içerik bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleDosyasiIcerikListeKaydet(Kullanici kullanan, IhaleDosyasiIcerikListe liste, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürece bağlı ihale dosyası içerik bilgisini veritabanından okur 
        /// ve IhaleDosyasiIcerikListe nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürece bağlı ihale dosyası içerik bilgisini IhaleDosyasiIcerikListe nesnesi olarak tutan nesne</returns>
        IhaleDosyasiIcerikListe IhaleDosyasiIcerikListeAl(Kullanici kullanan, IhaleDosyasiIcerikListe kriter);
        /// <summary>
        /// Arguman olarak verilen, ihale dosyası satın alma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgileri IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleDosyasiAlanKaydet(Kullanici kullanan, ObjectArray kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale dosyası satın alma bilgisini veritabanından okur 
        /// ve IhaleTeklifDosyasi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale dosyası satın alma bilgisi listesini IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray IhaleDosyasiAlanListele(Kullanici kullanan, IhaleTeklifDosyasi kriter);
        /// <summary>
        /// Arguman olarak verilen, ihale dosyası teslim etme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgileri IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TeklifDosyasiVerenKaydet(Kullanici kullanan, ObjectArray kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale dosyası teslim etme bilgisini veritabanından okur 
        /// ve IhaleTeklifDosyasi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale dosyası teslim etme bilgisi listesini IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TeklifDosyasiVerenListele(Kullanici kullanan, IhaleTeklifDosyasi kriter);
        /// <summary>
        /// Arguman olarak verilen, firmanın ihale devam edip etmeme bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Kaydedilecek bilgileri IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleFirmaDevamGuncelle(Kullanici kullanan, ObjectArray liste);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihaleye devem eden firma bilgisini veritabanından okur 
        /// ve IhaleTeklifDosyasi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihaleye devam eden firma bilgisi listesini IhaleTeklifDosyasi nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray DevamEdenFirmaListele(Kullanici kullanan, IhaleTeklifDosyasi kriter);
        /// <summary>
        /// Arguman olarak verilen, ihale teklif dosyası komisyon tutanağı bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TeklifDosyasiKomisyonTutanagiKaydet(Kullanici kullanan, TeklifDosyasiKomisyonTutanagi kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale teklif dosyası komisyon tutanağı bilgisini veritabanından okur 
        /// ve TeklifDosyasiKomisyonTutanagi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale teklif dosyası komisyon tutanağı bilgisini TeklifDosyasiKomisyonTutanagi nesnesi olarak tutan nesne</returns>
        TeklifDosyasiKomisyonTutanagi TeklifDosyasiKomisyonTutanagiListele(Kullanici kullanan, TeklifDosyasiKomisyonTutanagi kriter);
        /// <summary>
        /// Arguman olarak verilen, ihale teklif dosyası içerik tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgileri TeklifDosyasiIcerik nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TeklifDosyasiIcerikKaydet(Kullanici kullanan, ObjectArray kriter);
        /// <summary>
        /// Arguman olarak verilen, ihale teklif dosyası içerik tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// Güncelleme servisinde kullanılır.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgileri TeklifDosyasiIcerik nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TeklifDosyasiIcerikListeKaydet(Kullanici kullanan, ObjectArray kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale teklif dosyası içerik bilgisini veritabanından okur 
        /// ve TeklifDosyasiIcerik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale teklif dosyası içerik bilgisi listesini TeklifDosyasiIcerik nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TeklifDosyasiIcerikListele(Kullanici kullanan, TeklifDosyasiIcerik kriter);
        /// <summary>
        /// Arguman olarak verilen, ihale teklif dosyası içerik bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TeklifDosyasiIcerikListeKaydet(Kullanici kullanan, TeklifDosyasiIcerikListe liste, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale teklif dosyası içerik bilgisini veritabanından okur 
        /// ve TeklifDosyasiIcerikListe nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale teklif dosyası içerik bilgisini TeklifDosyasiIcerikListe nesnesi olarak tutan nesne</returns>
        TeklifDosyasiIcerikListe TeklifDosyasiIcerikListeAl(Kullanici kullanan, TeklifDosyasiIcerikListe kriter);
        /// <summary>
        /// Arguman olarak verilen, ihale teklif zarfı biçim tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TeklifZarfBicimiKaydet(Kullanici kullanan, TeklifZarfBicimi bilgi);
        /// <summary>
        /// Arguman olarak verilen, ihale teklif zarfı biçim tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// Güncelleme servisinde kullanılır.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgileri TeklifZarfBicimi nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TeklifZarfBicimiListeKaydet(Kullanici kullanan, ObjectArray bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale teklif zarfı biçim tanım bilgisini veritabanından okur 
        /// ve TeklifZarfBicimi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale teklif zarfı biçim tanım bilgisi listesini TeklifZarfBicimi nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TeklifZarfBicimiListele(Kullanici kullanan, TeklifZarfBicimi kriter);
        /// <summary>
        /// Arguman olarak verilen, ihale teklif zarfı biçim tanım bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TeklifZarfBicimiSil(Kullanici kullanan, TeklifZarfBicimi bilgi);
        /// <summary>
        /// Arguman olarak verilen, teklif veren firmaların, teklif zarf kontrol bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TeklifZarfKontroluKaydet(Kullanici kullanan, TeklifZarfKontroluBaslik bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, teklif veren firmanın teklif zarfı kontrol bilgisini veritabanından okur 
        /// ve TeklifZarfKontroluBaslik nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, teklif zarf kontrol bilgisini TeklifZarfKontroluBaslik nesnesi olarak tutan nesne</returns>
        TeklifZarfKontroluBaslik TeklifZarfKontroluListele(Kullanici kullanan, TeklifZarfKontroluBaslik kriter);
        /// <summary>
        /// Arguman olarak verilen, teklif veren firmanın teklif zarfındaki eksik olan belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgileri TeklifDosyasiEksikBelge nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TeklifDosyasiEksikBelgeKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, teklif veren firmanın teklif zarfındaki eksik olan belge bilgisini veritabanından okur 
        /// ve TeklifDosyasiEksikBelge nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, teklif veren firmanın teklif zarfındaki eksik olan belge bilgisi listesini TeklifDosyasiEksikBelge nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TeklifDosyasiEksikBelgeListele(Kullanici kullanan, TeklifDosyasiEksikBelge kriter);
        /// <summary>
        /// Arguman olarak verilen, teklif veren firmanın teklif zarfındaki uygun olmayan belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgileri TeklifUygunOlmayanBelge nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TeklifUygunOlmayanBelgeKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, teklif veren firmanın teklif zarfındaki uygun olmayan belge bilgisini veritabanından okur 
        /// ve TeklifUygunOlmayanBelge nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, teklif veren firmanın teklif zarfındaki uygun olmayan belge bilgisi listesini TeklifUygunOlmayanBelge nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TeklifUygunOlmayanBelgeListele(Kullanici kullanan, TeklifUygunOlmayanBelge kriter);
        /// <summary>
        /// Arguman olarak verilen, ihale için firma tarafından teklif edilen fiyat bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgileri TeklifEdilenFiyat nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TeklifEdilenFiyatKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale için firma tarafından teklif edilen fiyat bilgisini veritabanından okur 
        /// ve TeklifEdilenFiyat nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale için firma tarafından teklif edilen fiyat bilgisi listesini TeklifEdilenFiyat nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TeklifEdilenFiyatListele(Kullanici kullanan, TeklifEdilenFiyat kriter);
        /// <summary>
        /// Arguman olarak verilen, ihaleyi kazanan firma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KazananFirmaKaydet(Kullanici kullanan, KazananFirma bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, ihaleyi kazanan firma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Güncellenecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KazananFirmaGuncelle(Kullanici kullanan, KazananFirma bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihaleyi kazanan firma bilgisini veritabanından okur 
        /// ve KazananFirma nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihaleyi kazanan firma bilgisini KazananFirma nesnesi olarak tutan nesne</returns>
        KazananFirma KazananFirmaListele(Kullanici kullanan, KazananFirma kriter);
        /// <summary>
        /// Arguman olarak verilen, ihale kararı bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleKarariKaydet(Kullanici kullanan, IhaleKarari bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale kararı bilgisini veritabanından okur 
        /// ve IhaleKarari nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale kararı bilgisini IhaleKarari nesnesi olarak tutan nesne</returns>
        IhaleKarari IhaleKarariListele(Kullanici kullanan, IhaleKarari kriter);
        /// <summary>
        /// Arguman olarak verilen, ihale firma sonucu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgileri IhaleFirmaSonucu nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleFirmaSonucuKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale firma sonucu bilgisini veritabanından okur 
        /// ve IhaleFirmaSonucu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale firma sonucu listesini IhaleFirmaSonucu nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray IhaleFirmaSonucuListele(Kullanici kullanan, IhaleFirmaSonucu kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kısmi teklif verilebilen ihale için ihale firma sonucu bilgisini veritabanından okur 
        /// ve IhaleFirmaSonucu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, kısmi teklif verilebilen ihale için ihale firma sonucu bilgisi listesini IhaleFirmaSonucu nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray IhaleFirmaSonucuListeleKismi(Kullanici kullanan, IhaleFirmaSonucu kriter);
        /// <summary>
        /// Arguman olarak verilen, ihaleyi kazanan firmalar için düzenelenen sözleşmeye davet bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgileri IhaleSozlesmeyeDavet nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleSozlesmeyeDavetKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihaleyi kazanan firmalar için düzenelenen sözleşmeye davet bilgisini veritabanından okur 
        /// ve IhaleSozlesmeyeDavet nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihaleyi kazanan firmalar için düzenelenen sözleşmeye davet bilgisi listesini IhaleSozlesmeyeDavet nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray IhaleSozlesmeyeDavetListele(Kullanici kullanan, IhaleSozlesmeyeDavet kriter);
        /// <summary>
        /// Arguman olarak verilen, sözleşme sonucunda ödnemesi gereken ücret bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleSozlesmeSonuUcretKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sözleşme sonucunda ödnemesi gereken ücret bilgisini veritabanından okur 
        /// ve IhaleSozlesmeSonuUcret nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sözleşme sonucunda ödnemesi gereken ücret bilgisi listesini IhaleSozlesmeSonuUcret nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray IhaleSozlesmeSonuUcretListele(Kullanici kullanan, IhaleSozlesmeSonuUcret kriter);
        /// <summary>
        /// Arguman olarak verilen, ihaleye firma daveti bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgileri FirmaTeklifDaveti nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleFirmaTeklifDavetiKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihaleye firma daveti bilgisini veritabanından okur 
        /// ve FirmaTeklifDaveti nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihaleye firma daveti bilgisi listesini FirmaTeklifDaveti nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray IhaleFirmaTeklifDavetiListele(Kullanici kullanan, FirmaTeklifDaveti kriter);
        /// <summary>
        /// Arguman olarak verilen, ihale sonucu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleSonucuKaydet(Kullanici kullanan, IhaleSonucu bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale sonucu bilgisini veritabanından okur 
        /// ve IhaleSonucu nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale sonucu bilgisini IhaleSonucu nesnesi olarak tutan nesne</returns>
        IhaleSonucu IhaleSonucuListele(Kullanici kullanan, IhaleSonucu kriter);
        /// <summary>
        /// Arguman olarak verilen, hakediş bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HakedisKaydet(Kullanici kullanan, Hakedis kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, ihalenin teklif değerlendirme işlemleri bittikten sonra girilen hakediş ön bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HakedisOnBilgiKaydet(Kullanici kullanan, Hakedis kriter, int islemKod, int parcaKod, TNSCollection tertipBilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hakediş bilgisini veritabanından okur 
        /// ve Hakedis nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, hakediş bilgisini Hakedis nesnesi olarak tutan nesne</returns>
        Hakedis HakedisListele(Kullanici kullanan, Hakedis kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hakediş ön bilgisini veritabanından okur 
        /// ve Hakedis nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, hakediş ön bilgisini Hakedis nesnesi olarak tutan nesne</returns>
        Hakedis HakedisListeleOnBilgi(Kullanici kullanan, Hakedis kriter);
        /// <summary>
        /// Arguman olarak verilen, hakediş ödeme planına fatura numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <param name="hakedisSiraNo">İşlem yapılacak hakediş sıra numarası bilgisi</param>
        /// <param name="faturaNo">İşlem yapılacak fatura numarası bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HakedisFaturaNoKaydet(Kullanici kullanan, int yil, string surecNo, int hakedisSiraNo, int faturaNo);
        /// <summary>
        /// Arguman olarak verilen, hakediş ödeme planı bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc HakedisOdemePlaniSil(Kullanici kullanan, HakedisOdemePlan kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, sözleşmeli maaş bordro bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgileri PSLSozlesmeliMaasBordro nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc PSLSozlesmeliMaasBordroKaydet(Kullanici kullanan, ObjectArray kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, sözleşmeli maaş bordro bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc PSLSozlesmeliMaasBordroSil(Kullanici kullanan, PSLSozlesmeliMaasBordro kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sözleşmeli maaş bordro bilgisini veritabanından okur 
        /// ve PSLSozlesmeliMaasBordro nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sözleşmeli maaş bordro bilgisi listesini PSLSozlesmeliMaasBordro nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray PSLSozlesmeliMaasBordroListele(Kullanici kullanan, PSLSozlesmeliMaasBordro kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sözleşmeli personel asgari geçim bordro bilgisini veritabanından okur 
        /// ve PSLSozlesmeliMaasBordro nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sözleşmeli personel asgari geçim bordro bilgisi listesini PSLSozlesmeliMaasBordro nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray PSLSozlesmeliAsgariGecimBordroListele(Kullanici kullanan, TNS.HRC.PSLSozlesmeliMaasBordro kriter);
        /// <summary>
        /// Arguman olarak verilen, sözleşmeli maaş ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bordro">Kaydederken kullanılacak kriter bilgisini tutan nesne</param>
        /// <param name="belgeNo">Kaydedilecek ödeme emri belge numarası bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc PSLSozlesmeliMaasMIFNoKaydet(Kullanici islemiYapan, PSLSozlesmeliMaasBordro bordro, string belgeNo);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sözleşme maaş ödeme emri belge numarası bilgisini veritabanından okur 
        /// ve Sonuc nesnesi olarak döner.
        /// Ödeme Emri Belge Numarası bilgisi Sonuc nesnesinin anahtar özelliğinde bulunmaktadır.
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc PSLSozlesmeliMaasMIFNoListele(Kullanici islemiYapan, PSLSozlesmeliMaasBordro kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sözleşmeli maaş bordrosu için fark bordrosu hazırlanıp hazırlanmadığı bilgisini veritabanından okur 
        /// ve boolean olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sözleşmeli maaş bordrosu için fark bordrosu hazırlanıp hazırlanmadığı bilgisini boolean olarak döner</returns>
        bool PSLSozlesmeliFarkBordrosuVarMi(Kullanici kullanan, TNS.HRC.PSLSozlesmeliMaasBordro kriter);
        /// <summary>
        /// Arguman olarak verilen, öğrenci maaş bordro bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgileri PSLOgrenciMaasBordro nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc PSLOgrenciMaasBordroKaydet(Kullanici kullanan, ObjectArray kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, öğrenci maaş bordro bilgisini veritabanından okur 
        /// ve PSLOgrenciMaasBordro nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, öğrenci maaş bordro listesini PSLOgrenciMaasBordro nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray PSLOgrenciMaasBordroListele(Kullanici kullanan, PSLOgrenciMaasBordro kriter);
        /// <summary>
        /// Arguman olarak verilen, öğrenci maaş ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="kriter">Kaydederken kullanılacak kriter bilgisini tutan nesne</param>
        /// <param name="belgeNo">Kaydedilecek ödeme emri belge numarası bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc PSLOgrenciMaasMIFNoKaydet(Kullanici islemiYapan, PSLOgrenciMaasBordro kriter, string belgeNo);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, öğrenci maaş ödeme emri belge numarası bilgisini veritabanından okur 
        /// ve Sonuc nesnesi olarak döner.
        /// Ödeme Emri Belge Numarası bilgisi Sonuc nesnesinin anahtar özelliğinde bulunmaktadır.
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns> 
        Sonuc PSLOgrenciMaasMIFNoListele(Kullanici islemiYapan, PSLOgrenciMaasBordro kriter);
        /// <summary>
        /// Arguman olarak verilen, maaş temel tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgileri PSLMaasTemelBilgisi nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns> 
        Sonuc PSLMaasTemelBilgisiKaydet(Kullanici kullanan, ObjectArray kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, maaş temel tanım bilgisini veritabanından okur 
        /// ve PSLMaasTemelBilgisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, maaş temel tanım bilgisi listesini PSLMaasTemelBilgisi nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray PSLMaasTemelBilgisiListele(Kullanici kullanan, TNS.HRC.PSLMaasTemelBilgisi kriter);
        /// <summary>
        /// Arguman olarak verilen, personel hastalık izni bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgileri PSLHastalikIzni nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc PSLHastalikIzniKaydet(Kullanici kullanan, ObjectArray kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, personel hastalık izni bilgisini veritabanından okur 
        /// ve PSLHastalikIzni nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, personel hastalık izni listesini PSLHastalikIzni nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray PSLHastalikIzniListele(Kullanici kullanan, TNS.HRC.PSLHastalikIzni kriter);
        /// <summary>
        /// Arguman olarak verilen, birime gelen birimden giden personel bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgileri PSLGelenGidenPersonel nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc PSLGelenGidenPersonelKaydet(Kullanici kullanan, ObjectArray kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, birime gelen birimden giden personel bilgisini veritabanından okur 
        /// ve PSLGelenGidenPersonel nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, birime gelen birimden giden personel bilgisi listesini PSLGelenGidenPersonel nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray PSLGelenGidenPersonelListele(Kullanici kullanan, TNS.HRC.PSLGelenGidenPersonel kriter);
        /// <summary>
        /// Arguman olarak verilen, gelir vergisi tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgileri PSLGelirVergisi nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc PSLGelirVergisiKaydet(Kullanici kullanan, ObjectArray kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, gelir vergisi tanım bilgisini veritabanından okur 
        /// ve PSLGelirVergisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, gelir vergisi tanım listesini PSLGelirVergisi nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray PSLGelirVergisiListele(Kullanici kullanan, TNS.HRC.PSLGelirVergisi kriter);
        /// <summary>
        /// Arguman olarak verilen, SSK Hizmet Belgesi bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc PSLSSKHizmetBelgesiKaydet(Kullanici kullanan, PSLSSKHizmetBelgesi kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, * bilgisini veritabanından okur 
        /// ve * nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">The kriter.</param>
        /// <returns></returns>
        PSLSSKHizmetBelgesi PSLSSKHizmetBelgesiListele(Kullanici kullanan, PSLSSKHizmetBelgesi kriter);
        /// <summary>
        /// Arguman olarak verilen, personel sosyal yardım belgesi bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgileri PSLSosyalYardimi nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc PSLSosyalYardimiKaydet(Kullanici kullanan, ObjectArray kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, personel sosyal yardım belgesi bilgisini veritabanından okur 
        /// ve PSLSosyalYardimi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, personel sosyal yardım belgesi bilgisi listesini PSLSosyalYardimi nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray PSLSosyalYardimiListele(Kullanici kullanan, TNS.HRC.PSLSosyalYardimi kriter);
        /// <summary>
        /// Arguman olarak verilen, personel sosyal yardım belgesine ait ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc PSLSosyalYardimiOdemeEmriNoKaydet(Kullanici kullanan, PSLSosyalYardimi kriter);
        /// <summary>
        /// Arguman olarak verilen, sosyal yardım tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgileri PSLSosyalYardimTemelBilgisi nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc PSLSosYardTemelBilgisiKaydet(Kullanici kullanan, ObjectArray kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sosyal yardım tanım bilgisini veritabanından okur 
        /// ve PSLSosyalYardimTemelBilgisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sosyal yardım tanım bilgisi listesini PSLSosyalYardimTemelBilgisi nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray PSLSosYardTemelBilgisiListele(Kullanici kullanan, TNS.HRC.PSLSosyalYardimTemelBilgisi kriter);
        /// <summary>
        /// Arguman olarak verilen, aile yardım bildirim belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc PSLAileYardimBildirimiKaydet(Kullanici kullanan, PSLAileYardimBildirimiBaslik kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, aile yardım bildirim belge bilgisini veritabanından okur 
        /// ve PSLAileYardimBildirimiBaslik nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, aile yardım bildirim belge bilgisini PSLAileYardimBildirimiBaslik nesnesi olarak tutan nesne</returns>
        PSLAileYardimBildirimiBaslik PSLAileYardimBildirimiListele(Kullanici kullanan, PSLAileYardimBildirimiBaslik kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale takvim bilgisini veritabanından okur 
        /// ve IhaleTakvim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale takvim bilgisi listesini IhaleTakvim nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray IhaleTakvimListele(Kullanici kullanan, TNS.HRC.Surec kriter);
        /// <summary>
        /// Arguman olarak verilen, sözleşmeli personel bordro tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgileri SozlesmeliBordroTanim nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc PSLSozlesmeliBordroTanimKaydet(Kullanici kullanan, ObjectArray kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sözleşmeli personel bordro hesaplamasında kullanılacak tanım bilgisini veritabanından okur 
        /// ve SozlesmeliBordroTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sözleşmeli personel bordro hesaplamasında kullanılacak tanım bilgisi listesini SozlesmeliBordroTanim nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray PSLSozlesmeliBordroTanimListele(Kullanici kullanan, TNS.HRC.SozlesmeliBordroTanim kriter);
        /// <summary>
        /// Arguman olarak verilen, fazla mesai bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FazlaMesaiKaydet(Kullanici islemiYapan, FazlaMesai bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, fazla mesai bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FazlaMesaiSil(Kullanici islemiYapan, FazlaMesai bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai bilgisini veritabanından okur 
        /// ve FazlaMesai nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The kullanici.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, fazla mesai bilgisini FazlaMesai nesnesi olarak tutan nesne</returns>
        FazlaMesai FazlaMesaiListele(Kullanici kullanici, FazlaMesai kriter);
        /// <summary>
        /// Arguman olarak verilen, fazla mesai bilgisi için gerekli hesaplamaları yapar, fazla mesai bordro bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FazlaMesaiBordroHazirla(Kullanici islemiYapan, FazlaMesai bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, fazla mesai bordro bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FazlaMesaiBordroSil(Kullanici islemiYapan, FazlaMesaiBordro bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai bordro bilgisini veritabanından okur 
        /// ve FazlaMesaiBordrolar nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="bankaListesi">Eğer <c>true</c> verilirse banka bilgilerini de getirir</param>
        /// <returns>İstenilen kriterdeki, fazla mesai bordro bilgisini FazlaMesaiBordrolar nesnesi olarak tutan nesne</returns>
        FazlaMesaiBordrolar FazlaMesaiBordroListele(Kullanici islemiYapan, FazlaMesaiBordro kriter, bool bankaListesi);
        /// <summary>
        /// Arguman olarak verilen, fazla mesai bordro kesinti bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FazlaMesaiBordroKesintiKaydet(Kullanici islemiYapan, FazlaMesaiBordrolar bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, fazla mesai bordro ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Ödeme emri belge numarasının kaydedileceği belge bilgisini tutan nesne</param>
        /// <param name="belgeNo">Kaydedilecek ödeme emri belge numarası bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FazlaMesaiMIFNoKaydet(Kullanici islemiYapan, FazlaMesaiBordro bilgi, string belgeNo);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai bordro ödeme emri belge numarası bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// Ödeme emri belge numarası Sonuc nesnesinin anahtar özelliğinde bulunur
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FazlaMesaiMIFNoListele(Kullanici islemiYapan, FazlaMesaiBordro kriter);
        /// <summary>
        /// Arguman olarak verilen, fazla mesai düzenelenen personel bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FazlaMesaiPersonelKaydet(Kullanici islemiYapan, FazlaMesai bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, fazla mesai düzenelenen personel bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FazlaMesaiPersonelSil(Kullanici islemiYapan, FazlaMesai bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai düzenlenen personel bilgisini veritabanından okur 
        /// ve FazlaMesai nesnesi olarak döner
        /// </summary>
        /// <param name="islemYapan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, fazla mesai düzenlenen personel bilgisini FazlaMesai nesnesi olarak tutan nesne</returns>
        FazlaMesai FazlaMesaiPersonelListele(Kullanici islemYapan, FazlaMesai kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai bilgisini veritabanından okur 
        /// ve FazlaMesai nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, fazla mesai bilgisi listesini FazlaMesai nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray FazlaMesaiSorgula(Kullanici islemiYapan, FazlaMesai kriter);
        /// <summary>
        /// Arguman olarak verilen, fazla mesai ücret hesaplamasında kullanılacak puantaj tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FazlaMesaiPuantajKaydet(Kullanici islemiYapan, FazlaMesaiPuantaj bilgi);
        /// <summary>
        /// Arguman olarak verilen, fazla mesai ücret hesaplamasında kullanılacak puantaj tanım bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc FazlaMesaiPuantajSil(Kullanici islemiYapan, FazlaMesaiPuantaj bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fazla mesai ücret hesaplamasında kullanılacak puantaj tanım bilgisini veritabanından okur 
        /// ve FazlaMesaiPuantajlar nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, fazla mesai ücret hesaplamasında kullanılacak puantaj tanım bilgisini FazlaMesaiPuantaj nesnesi olarak tutan nesne</returns>
        FazlaMesaiPuantajlar FazlaMesaiPuantajListele(Kullanici islemiYapan, FazlaMesaiPuantaj kriter);
        /// <summary>
        /// Arguman olarak verilen, ek ders bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc EkDersKaydet(Kullanici islemiYapan, EkDers bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, ek ders bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc EkDersSil(Kullanici islemiYapan, EkDers bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders bilgisini veritabanından okur 
        /// ve EkDers nesnesi olarak döner
        /// </summary>
        /// <param name="kullanici">The kullanici.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns></returns>
        EkDers EkDersListele(Kullanici kullanici, EkDers kriter);
        /// <summary>
        /// Arguman olarak verilen, ek ders bilgisi için gerekli hesaplamaları yapar, ek ders bordro bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Bordro hazırlananacak bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc EkDersBordroHazirla(Kullanici islemiYapan, EkDers bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, ek ders bordro bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Bordro hazırlananacak bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc EkDersBordroSil(Kullanici islemiYapan, EkDersBordro bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders bordro bilgisini veritabanından okur 
        /// ve EkDersBordrolar nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="bankaListesi">Eğer <c>true</c> verilirse banka bilgilerini de getirir</param>
        /// <returns>İstenilen kriterdeki, ek ders bordro bilgisini EkDersBordrolar nesnesi olarak tutan nesne</returns>
        EkDersBordrolar EkDersBordroListele(Kullanici islemiYapan, EkDersBordro kriter, bool bankaListesi);
        /// <summary>
        /// Arguman olarak verilen, ek ders bordro kesinti bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Bordro hazırlananacak bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc EkDersBordroKesintiKaydet(Kullanici islemiYapan, EkDersBordrolar bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, ek ders bordro ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Ödeme emri belge numarasının kaydedileceği belge bilgisini tutan nesne</param>
        /// <param name="belgeNo">Kaydedilecek ödeme emri belge numarası bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc EkDersMIFNoKaydet(Kullanici islemiYapan, EkDersBordro bilgi, string belgeNo);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders bordro ödeme emri belge numarası bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// Ödeme emri belge numarası Sonuc nesnesinin anahtar özelliğinde bulunur
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns></returns>
        Sonuc EkDersMIFNoListele(Kullanici islemiYapan, EkDersBordro kriter);
        /// <summary>
        /// Arguman olarak verilen, ek ders düzenelenen personel bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc EkDersPersonelKaydet(Kullanici islemiYapan, EkDers bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, ek ders düzenelenen personel bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc EkDersPersonelSil(Kullanici islemiYapan, EkDers bilgi, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders düzenlenen personel bilgisini veritabanından okur 
        /// ve EkDers nesnesi olarak döner
        /// </summary>
        /// <param name="islemYapan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ek ders düzenlenen personel bilgisini EkDers nesnesi olarak tutan nesne</returns>
        EkDers EkDersPersonelListele(Kullanici islemYapan, EkDers kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders bilgisini veritabanından okur 
        /// ve EkDers nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ek ders listesini EkDers nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray EkDersSorgula(Kullanici islemiYapan, EkDers kriter);
        /// <summary>
        /// Arguman olarak verilen, ek ders ücret hesaplamasında kullanılacak puantaj tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc EkDersPuantajKaydet(Kullanici islemiYapan, EkDersPuantaj bilgi);
        /// <summary>
        /// Arguman olarak verilen, ek ders ücret hesaplamasında kullanılacak puantaj tanım bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc EkDersPuantajSil(Kullanici islemiYapan, EkDersPuantaj bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders ücret hesaplamasında kullanılacak puantaj tanım bilgisini veritabanından okur 
        /// ve EkDersPuantajlar nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ek ders ücret hesaplamasında kullanılacak puantaj tanım bilgisini EkDersPuantajlar nesnesi olarak tutan nesne</returns>
        EkDersPuantajlar EkDersPuantajListele(Kullanici islemiYapan, EkDersPuantaj kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ek ders ücret hesaplamasında kullanılacak puantaj tanımının açıklama bilgisini veritabanından okur 
        /// ve EkDersPuantajlar nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ek ders ücret hesaplamasında kullanılacak puantaj tanımının açıklama bilgisini EkDersPuantajlar nesnesi olarak tutan nesne</returns>
        EkDersPuantajlar EkDersPuantajAciklamaListele(Kullanici islemiYapan, EkDersPuantaj kriter);

        /// <summary>
        /// Arguman olarak verilen, çeşitli ödemeler bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgileri CesitliOdeme nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc MahkemeHarcKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod);

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, çeşitli ödemeler bilgisini veritabanından okur 
        /// ve CesitliOdeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, çeşitli ödemeler bilgisi listesini CesitliOdeme nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray MahkemeHarcListele(Kullanici kullanan, MahkemeHarc kriter);

        /// <summary>
        /// Arguman olarak verilen, çeşitli ödemeler bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgileri CesitliOdeme nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc CesitliOdemeKaydet(Kullanici kullanan, ObjectArray bilgi, int islemKod, int parcaKod);

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, çeşitli ödemeler bilgisini veritabanından okur 
        /// ve CesitliOdeme nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, çeşitli ödemeler bilgisi listesini CesitliOdeme nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray CesitliOdemeListele(Kullanici kullanan, CesitliOdeme kriter);
        /// <summary>
        /// Arguman olarak verilen, çeşitli ödemeler ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns></returns>
        Sonuc CesitliOdemeOdemeEmriNoKaydet(Kullanici kullanan, CesitliOdeme bilgi);
        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="sb">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc GeciciGorevYollukBelgeKaydet(Kullanici islemiYapan, GeciciGorevYollukBelge sb, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk belge bilgisini veritabanından okur 
        /// ve GeciciGorevYollukBelge nesnesi olarak döner
        /// </summary>
        /// <param name="islemYapan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns></returns>
        GeciciGorevYollukBelge GeciciGorevYollukBelgeListele(Kullanici islemYapan, GeciciGorevYollukBelge kriter);
        ObjectArray GeciciGorevYollukBelgeAyrintiliListele(Kullanici kullanan, GeciciGorevYollukBelge kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk belge bilgisini veritabanından okur 
        /// ve GeciciGorevYollukBelge nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="islemYapan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk belge bilgisi listesini GeciciGorevYollukBelge nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray GeciciGorevYollukBelgeTumListe(Kullanici islemYapan, GeciciGorevYollukBelge kriter);
        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk belgesini, geçici görev yolluk belge havuzundan siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="sb">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc GeciciGorevYollukHavuzSil(Kullanici islemiYapan, GeciciGorevYollukBelge sb);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk belge bilgisini veritabanından okur 
        /// ve GeciciGorevYollukBelge nesnesi olarak döner
        /// </summary>
        /// <param name="islemYapan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk belge bilgisini GeciciGorevYollukBelge nesnesi olarak tutan nesne</returns>
        GeciciGorevYollukBelge OncedenKayitliMi(Kullanici islemYapan, GeciciGorevYollukBelge kriter);
        /// <summary>
        /// Arguman olarak verilen, toplu geçici görev yolluk belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="liste">Kaydedilecek bilgileri GeciciGorevYollukToplu nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc GeciciGorevYollukTopluKaydet(Kullanici islemiYapan, ObjectArray liste, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, toplu geçici görev yolluk belge bilgisini veritabanından okur 
        /// ve GeciciGorevYollukToplu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="islemYapan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk belge bilgisi listesini GeciciGorevYollukBelge nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray GeciciGorevYollukTopluListele(Kullanici islemYapan, GeciciGorevYollukToplu kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, toplu geçici görev yolluk belge banka listesi bilgisini veritabanından okur 
        /// ve GeciciGorevYollukToplu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="islemYapan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk belge bilgisi listesini GeciciGorevYollukBelge nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray GeciciGorevYollukTopluBankaListesiListele(Kullanici islemYapan, GeciciGorevYollukToplu kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk istatistik bilgisini veritabanından okur 
        /// ve PSLYollukIstatistikSinif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk istatistik bilgisi listesini PSLYollukIstatistikSinif nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray GeciciGorevYollukIstatistik(Kullanici kullanan, PSLYollukIstatistikSinif kriter);
        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemYapan">The islem yapan.</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc GeciciGundelikTanimKaydet(Kullanici islemYapan, YollukGundelikTanim liste, bool zorunluYeni);
        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemYapan">The islem yapan.</param>
        /// <param name="liste">Kaydedilecek bilgileri YollukGundelikTanim nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc GeciciGundelikTanimListeKaydet(Kullanici islemYapan, int yil, ENUMYollukGundelikTuru tur, ObjectArray liste);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk tanım bilgisini veritabanından okur 
        /// ve YollukGundelikTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="islemYapan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk tanım bilgisi listesini YollukGundelikTanim nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray GeciciGundelikTanimListele(Kullanici islemYapan, YollukGundelikTanim kriter);
        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk tanım bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemYapan">The islem yapan.</param>
        /// <param name="gt">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc GeciciGundelikTanimSil(Kullanici islemYapan, YollukGundelikTanim gt);
        /// <summary>
        /// Arguman olarak verilen, sürekli görev yolluk belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="sb">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SurekliGorevYollukBelgeKaydet(Kullanici islemiYapan, SurekliGorevYollukBelge sb, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk belge bilgisini veritabanından okur 
        /// ve SurekliGorevYollukBelge nesnesi olarak döner
        /// </summary>
        /// <param name="islemYapan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk belge bilgisini SurekliGorevYollukBelge nesnesi olarak tutan nesne</returns>
        SurekliGorevYollukBelge SurekliGorevYollukBelgeListele(Kullanici islemYapan, SurekliGorevYollukBelge kriter);
        /// <summary>
        /// Arguman olarak verilen, sürekli görev yolluk belgesini, sürekli görev yolluk belge havuzundan siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="islemiYapan">The islemi yapan.</param>
        /// <param name="sb">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SurekliGorevYollukHavuzSil(Kullanici islemiYapan, SurekliGorevYollukBelge sb);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk istatistik bilgisini veritabanından okur 
        /// ve PSLYollukIstatistikSinif nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk istatistik bilgisi listesini PSLYollukIstatistikSinif nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray SurekliGorevYollukIstatistik(Kullanici kullanan, PSLYollukIstatistikSinif kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk belge bilgisini veritabanından okur 
        /// ve SurekliGorevYollukBelge nesnesi olarak döner
        /// </summary>
        /// <param name="islemYapan">The islem yapan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk belge bilgisini SurekliGorevYollukBelge nesnesi olarak tutan nesne</returns>
        SurekliGorevYollukBelge OncedenKayitliMi(Kullanici islemYapan, SurekliGorevYollukBelge kriter);
        /// <summary>
        /// Arguman olarak verilen, işçi maaş bordro hesaplamasında kullanılacak tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgileri IsciBordroTanim nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IsciBordroTanimKaydet(Kullanici kullanan, ObjectArray kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, işçi maaş bordro hesaplamasında kullanılacak tanım bilgisini veritabanından okur 
        /// ve IsciBordroTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, işçi maaş bordro hesaplamasında kullanılacak tanım bilgisi listesini IsciBordroTanim nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray IsciBordroTanimListele(Kullanici kullanan, IsciBordroTanim kriter);
        /// <summary>
        /// Arguman olarak verilen, işçi maaş bordro bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgileri IsciMaasBordro nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IsciMaasBordroKaydet(Kullanici kullanan, ObjectArray kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen, işçi maaş bordro bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IsciMaasBordroSil(Kullanici kullanan, IsciMaasBordro kriter, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, işçi maaş bordro bilgisini veritabanından okur 
        /// ve IsciMaasBordro nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, işçi maaş bordro bilgisi listesini IsciMaasBordro nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray IsciMaasBordroListele(Kullanici kullanan, IsciMaasBordro kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, rapor bilgisini veritabanından okur 
        /// ve ilgili nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, rapor bilgisi listesini ilgili nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray RaporAl(Kullanici kullanan, RaporKriter kriter);
        /// <summary>
        /// Arguman olarak verilen, fatura yıl ve fatura ay bilgisini kullanarak telefon abone fatura bilgisini Telekom'un gönderdiği xml dosyasından okur
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TelekomFaturaOlustur(Kullanici kullanan, AboneFaturaTelefon kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, telekom fatura bilgisini veritabanından okur 
        /// ve fatura bilgisi bulunan kullanıcıları listeler ve bu kullanıcılara fatura bilgisini içeren 
        /// mail gönderir.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        void TelekomFaturaMailAt(Kullanici kullanan, AboneFaturaTelefon kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, telekom fatura bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// Telekom fatura bilgisi varsa Sonuc nesnesinin islemSonuc özelliği true olarak gelir.
        /// </summary>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TelekomFaturaKayitVarMi(AboneFaturaTelefon kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, telekom fatura bilgisini veritabanından okur 
        /// ve AboneFaturaTelefon nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="sadeceYeniler">Eğer <c>true</c> verilirse süreç oluşturulmamış telekom fatura bilgisi listelenir.</param>
        /// <returns>İstenilen kriterdeki, telekom fatura bilgisi listesini AboneFaturaTelefon nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TelekomFaturaListele(Kullanici kullanan, AboneFaturaTelefon kriter, bool sadeceYeniler);
        /// <summary>
        /// Arguman olarak verilen kriterlere göre fatura dosya ismi üretir ve string olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="faturaYil">İşlem yapılacak fatura yıl bilgisi.</param>
        /// <param name="faturaAy">İşlem yapılacak fatura ay bilgisi.</param>
        /// <param name="faturaNolar">İşlem yapılacak fatura numaraları. ";" ile birbirinden ayrılır</param>
        /// <returns>Kriterlere uygun fatura dosya isimlerini ";" ile ayrılmış string olarak döner</returns>
        string TelekomFaturaDosyaIsimVer(Kullanici kullanan, int faturaYil, int faturaAy, string faturaNolar);
        /// <summary>
        /// Arguman olarak verilen dosyayı okur ve byte dizisi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="faturaYil">İşlem yapılacak fatura yil bilgisi.</param>
        /// <param name="faturaAy">İşlem yapılacak fatura ay bilgisi.</param>
        /// <param name="dosyaAd">İşlem yapılacak dosya adı bilgisi.</param>
        /// <returns>Okunan dosyayı byte dizisi olarak döner</returns>
        byte[] TelekomFaturaDosyaVer(Kullanici kullanan, int faturaYil, int faturaAy, string dosyaAd);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini veritabanından okur 
        /// ve ilgili kullanıcılara fatura bilgisini mail olarak gönderir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        void ElektrikFaturaMailAt(Kullanici kullanan, AboneFaturaElektrik kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// Elektrik fatura bilgisi varsa Sonuc nesnesinin islemSonuc özelliği true olarak gelir.
        /// </summary>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc ElektrikFaturaKayitVarMi(AboneFaturaElektrik kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini veritabanından okur 
        /// ve AboneFaturaElektrik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="sadeceYeniler">Eğer <c>true</c> süreç oluşturulmayan faturalar gelir</param>
        /// <returns>İstenilen kriterdeki, elektrik fatura bilgisi listesini AboneFaturaElektrik nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray ElektrikFaturaListele(Kullanici kullanan, AboneFaturaElektrik kriter, bool sadeceYeniler);
        /// <summary>
        /// Arguman olarak verilen kriterlere göre fatura dosya ismi üretir ve string olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="faturaYil">İşlem yapılacak fatura yıl bilgisi.</param>
        /// <param name="faturaAy">İşlem yapılacak fatura ay bilgisi.</param>
        /// <param name="faturaNolar">İşlem yapılacak fatura numaraları. ";" ile birbirinden ayrılır</param>
        /// <returns>Kriterlere uygun fatura dosya isimlerini ";" ile ayrılmış string olarak döner</returns>
        string ElektrikFaturaDosyaIsimVer(Kullanici kullanan, int faturaYil, int faturaAy, string faturaNolar);
        /// <summary>
        /// Arguman olarak verilen dosyayı okur ve byte dizisi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="faturaYil">İşlem yapılacak fatura yil bilgisi.</param>
        /// <param name="faturaAy">İşlem yapılacak fatura ay bilgisi.</param>
        /// <param name="dosyaAd">İşlem yapılacak dosya adı bilgisi.</param>
        /// <returns>Okunan dosyayı byte dizisi olarak döner</returns>
        byte[] ElektrikFaturaDosyaVer(Kullanici kullanan, int faturaYil, int faturaAy, string dosyaAd);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini veritabanından okur 
        /// ve ilgili dosyaya fatura çıktısını kaydeder
        /// ve sonucu string olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yil bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <returns>Fatura çıktısı oluşturma işlem sonucu string olarak döner</returns>
        string ElektrikEskiFaturaCiktiOlustur(Kullanici kullanan, int yil, int ay);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini veritabanından okur 
        /// ve ilgili kullanıcılara fatura bilgisini mail olarak gönderir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        void SuFaturaMailAt(Kullanici kullanan, AboneFaturaSu kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, su fatura bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// Su fatura bilgisi varsa Sonuc nesnesinin islemSonuc özelliği true olarak gelir.
        /// </summary>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SuFaturaKayitVarMi(AboneFaturaSu kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, su fatura bilgisini veritabanından okur 
        /// ve AboneFaturaSu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <param name="sadeceYeniler">Eğer <c>true</c> süreç oluşturulmayan faturalar gelir</param>
        /// <returns>İstenilen kriterdeki, su fatura bilgisi listesini AboneFaturaSu nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray SuFaturaListele(Kullanici kullanan, AboneFaturaSu kriter, bool sadeceYeniler);
        /// <summary>
        /// Arguman olarak verilen kriterlere göre fatura dosya ismi üretir ve string olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="faturaYil">İşlem yapılacak fatura yıl bilgisi.</param>
        /// <param name="faturaAy">İşlem yapılacak fatura ay bilgisi.</param>
        /// <param name="faturaNolar">İşlem yapılacak fatura numaraları. ";" ile birbirinden ayrılır</param>
        /// <returns>Kriterlere uygun fatura dosya isimlerini ";" ile ayrılmış string olarak döner</returns>
        string SuFaturaDosyaIsimVer(Kullanici kullanan, int faturaYil, int faturaAy, string faturaNolar);
        /// <summary>
        /// Arguman olarak verilen dosyayı okur ve byte dizisi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="faturaYil">İşlem yapılacak fatura yıl bilgisi.</param>
        /// <param name="faturaAy">İşlem yapılacak fatura ay bilgisi.</param>
        /// <param name="dosyaAd">İşlem yapılacak dosya adı bilgisi.</param>
        /// <returns>Okunan dosyayı byte dizisi olarak döner</returns>
        byte[] SuFaturaDosyaVer(Kullanici kullanan, int faturaYil, int faturaAy, string dosyaAd);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç hatırlatma bilgisini veritabanından okur 
        /// ve string (kod|aciklama) nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns>İstenilen kriterdeki, süreç hatırlatma bilgisi listesini string (kod|aciklama) nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray SurecHatirlatmaSorgula(Kullanici kullanan);
        /// <summary>
        /// Arguman olarak verilen, süreç hatırlatma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kod">İşlem yapılacak hatırlatma kodu bilgisi</param>
        /// <param name="aciklama">İşlem yapılacak hatırlatma açıklama bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SurecHatirlatmaKaydet(Kullanici kullanan, int kod, string aciklama);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç tertip bilgisini veritabanından okur 
        /// ve string nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan"> The kullanan.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kodu yıl bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kodu bilgisi</param>
        /// <returns>İstenilen kriterdeki, süreç tertip bilgisi listesini string nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray SurecTertipSorgula(Kullanici kullanan, int muhasebe, string birim);
        /// <summary>
        /// Arguman olarak verilen, süreç tertip bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tertip">İşlem yapılacak tertip bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SurecTertipKaydet(Kullanici kullanan, string tertip);
        /// <summary>
        /// Arguman olarak verilen, süreç tertip bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tertip">İşlem yapılacak tertip bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SurecTertipSil(Kullanici kullanan, string tertip);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, mal hizmet yapım alım tutar bilgisini veritabanından okur 
        /// ve MHYAlimTutari nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, mal hizmet yapım alım tutar bilgisini MHYAlimTutari nesnesi olarak tutan nesne</returns>
        MHYAlimTutari MHYAlimTutariListele(Kullanici kullanan, MHYAlimTutari kriter);
        /// <summary>
        /// Arguman olarak verilen, kamu ihale kurumu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KIKBilgileriKaydet(Kullanici kullanan, ObjectArray bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kamu ihale kurumu bilgisini veritabanından okur 
        /// ve KIKBilgileri nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, kamu ihale kurumu bilgisi listesini KIKBilgileri nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray KIKBilgileriListele(Kullanici kullanan, KIKBilgileri kriter);

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, il bilgisini veritabanından okur 
        /// ve Il nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns>İstenilen kriterdeki, il billgisi listesini Il nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray IlListeleKY(Kullanici kullanan, string ilkod = null);

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, il bilgisini veritabanından okur 
        /// ve Il nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns>İstenilen kriterdeki, il billgisi listesini Il nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray IlListele(Kullanici kullanan);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, il bilgisini veritabanından okur 
        /// ve Il nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ilKod">İşlem yapılacak il kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, il bilgisini Il nesnesi olarak tutan nesne</returns>
        Il IlListele(Kullanici kullanan, string ilKod);
        /// <summary>
        /// Arguman olarak verilen, il bilgisi listesini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgileri Il nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IlListesiKaydet(Kullanici kullanan, ObjectArray bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ülke bilgisini veritabanından okur 
        /// ve Ulke nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ulke billgisi listesini Ulke nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray UlkeListele(Kullanici kullanan);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ulke bilgisini veritabanından okur 
        /// ve Ulke nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="ulkeKod">İşlem yapılacak ulke kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, ulke bilgisini Ulke nesnesi olarak tutan nesne</returns>
        Ulke UlkeListele(Kullanici kullanan, string ulkeKod);
        /// <summary>
        /// Arguman olarak verilen, ulke bilgisi listesini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgileri Ulke nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc UlkeListesiKaydet(Kullanici kullanan, ObjectArray bilgi);
        /// <summary>
        /// Arguman olarak verilen, abone no bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc AboneNoKaydet(Kullanici kullanan, AboneNo bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, abone no bilgisini veritabanından okur 
        /// ve AboneNo nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, abone no listesini AboneNo nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray AboneNoListele(Kullanici kullanan, AboneNo kriter);
        /// <summary>
        /// Arguman olarak verilen, tedavi bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TedaviKaydet(Kullanici kullanan, Tedavi liste, int islemKod, int parcaKod);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, tedavi bilgisini veritabanından okur 
        /// ve Tedavi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, tedavi bilgisini Tedavi nesnesi olarak tutan nesne</returns>
        Tedavi TedaviListele(Kullanici kullanan, Tedavi kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yıllık tedavi harcama ortalama bilgisini veritabanından okur 
        /// ve TedaviIstatistik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, yıllık tedavi harcama ortalama bilgisi listesini TedaviIstatistik nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray TedaviListeleIstatistik(Kullanici kullanan, TedaviIstatistik kriter);
        /// <summary>
        /// Arguman olarak verilen, tedavi bilgisine ait ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TedaviOdemeEmriNoKaydet(Kullanici kullanan, Tedavi kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, personel bilgisini veritabanından okur 
        /// ve PersonelBilgi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tcKimlikNo">İşlem yapılacak TC Kimlik No bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kodu bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kodu bilgisi.</param>
        /// <returns>İstenilen kriterdeki, personel bilgisini PersonelBilgi nesnesi olarak tutan nesne</returns>
        PersonelBilgi PersonelBilgiListele(Kullanici kullanan, string tcKimlikNo, int muhasebe, string birim);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, öğrenci eğitmen bilgisini veritabanından okur 
        /// ve PersonelBilgi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tcKimlikNo">İşlem yapılacak TC Kimlik No bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kodu bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kodu bilgisi.</param>
        /// <returns>İstenilen kriterdeki, personel bilgisini PersonelBilgi nesnesi olarak tutan nesne</returns>
        PersonelBilgi PersonelOgrenciEgitmenListele(Kullanici kullanan, string tcKimlikNo, int muhasebe, string birim);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, personel aile bilgisini veritabanından okur 
        /// ve PersonelAileBilgi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, personel aile bilgisi listesini PersonelAileBilgi nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray PersonelAileListele(PersonelAileBilgi kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, personel sayısı bilgisini veritabanından okur 
        /// ve PersonelSayisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, personel sayısı bilgisi listesini PersonelSayisi nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray PersonelSayisi(int muhasebe, string birim);
        /// <summary>
        /// Arguman olarak verilen, geçici görev yolluk bilgisine ait ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc GeciciGorevYollukOdemeEmriNoKaydet(Kullanici kullanan, GeciciGorevYollukBelge kriter);
        /// <summary>
        /// Arguman olarak verilen, toplu geçici görev yolluk bilgisine ait ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc GeciciGorevYollukOdemeEmriNoKaydet(Kullanici kullanan, GeciciGorevYollukToplu kriter);
        /// <summary>
        /// Arguman olarak verilen, sürekli görev yolluk bilgisine ait ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SurekliGorevYollukOdemeEmriNoKaydet(Kullanici kullanan, SurekliGorevYollukBelge kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk toplam yolculuk gün sayısı bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk toplam yolculuk gün sayısı bilgisini double olarak döner</returns>
        double GGYToplamYolculukGunSayisi(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk toplam görev gün sayısı bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk toplam görev gün sayısı bilgisini double olarak döner</returns>
        double GGYToplamGorevGunSayisi(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk toplam gün sayısı bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk toplam gün sayısı bilgisini double olarak döner</returns>
        double GGYToplamGunSayisi(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk toplam yevmiye tutar bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk toplam yevmiye tutar bilgisini double olarak döner</returns>
        double GGYToplamYevmiyeTutar(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, geçici görev yolluk toplam adam sayısı bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, geçici görev yolluk toplam adam sayısı bilgisini double olarak döner</returns>
        double GGYToplamAdamSayisi(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl);
        /// <summary>
        /// Belirtilen parametrelere ait Harcama Yönetimi sisteminde gerçekleşen verileri döndürür.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Gerçekleşmenin yılı</param>
        /// <param name="muhasebeKod">Gerçekleşmenin yapıldığı muhasebeKod</param>
        /// <param name="birimKod">Gerçekleşmenin yapıldığı birimKod</param>
        /// <param name="belgeTur">Gerçekleşmenin türü (Yurt içi, Yurt dışı)</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İşlem sonucunu döndüren, ObjectArray sınıfında nesne içinde ObjectArrayler içerir ve her bir ObjectArray içerisinde sırasıyla Ülke Adı, Görev Sayısı, Toplam Görev Gün Sayısı ve Toplam Tutar bilgileri bulunur.</returns>
        ObjectArray GGYToplamGerceklesmeListele(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMYollukGundelikTuru belgeTur, bool sadeceIl);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, denetim elemanları için geçici görev yolluk toplam gerçekleşme bilgisini veritabanından okur 
        /// ve (gündelik adı, görev sayısı, toplam görev gün sayısı, toplam tutar) dizisi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, denetim elemanları için geçici görev yolluk toplam gerçekleşme bilgisini, 
        /// (gündelik adı, görev sayısı, toplam görev gün sayısı, toplam tutar) dizisi olarak tutan nesne</returns>
        ObjectArray GGYToplamGerceklesmeDenetimListele(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, bool sadeceIl);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk toplam yevmiye tutar bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk toplam yevmiye tutar bilgisini double olarak döner</returns>
        double SGYToplamYevmiyeTutar(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk ortalama mesafe bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk ortalama mesafe bilgisini double olarak döner</returns>
        double SGYOrtalamaMesafe(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürekli görev yolluk toplam adam sayısı bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, sürekli görev yolluk toplam adam sayısı bilgisini double olarak döner</returns>
        double SGYToplamAdamSayisi(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, tayin ve emeklilik için düzenlenen sürekli görev yolluk gerçekleşme bilgisini veritabanından okur 
        /// ve (tür adı, kişi sayısı, ödenen tutar) nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="belgeTur">İşlem yapılacak belge tur bilgisi (Yurt içi, Yurt dışı).</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İstenilen kriterdeki, tayin ve emeklilik için düzenlenen sürekli görev yolluk gerçekleşme bilgisini
        /// (tür adı, kişi sayısı, ödenen tutar) nesne koleksiyonu olarak döner</returns>
        ObjectArray SGYToplamGerceklesmeTayinEmekliListele(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl);
        /// <summary>
        /// Belirtilen parametrelere ait Harcama Yönetimi sisteminde gerçekleşen verileri döndürür.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Gerçekleşmenin yılı</param>
        /// <param name="muhasebeKod">Gerçekleşmenin yapıldığı muhasebeKod</param>
        /// <param name="birimKod">Gerçekleşmenin yapıldığı birimKod</param>
        /// <param name="belgeTur">Gerçekleşmenin türü (Yurt içi, Yurt dışı)</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse bağlı bulunduğu ile göre sonuç üretilir</param>
        /// <returns>İşlem sonucunu döndüren, ObjectArray sınıfında nesne içinde ObjectArrayler içerir ve her bir ObjectArray içerisinde sırasıyla Gerçekleşme türü (TAYIN,EMEKLI), Adam Sayısı ve Ödeme Tutarı bilgileri bulunur.</returns>
        ObjectArray SGYToplamGerceklesmeListele(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, tertiplere ait aylık harcama bilgisini veritabanından okur 
        /// ve AylikDagilim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="kurKod">İşlem yapılacak kurumsal kod bilgisi.</param>
        /// <param name="islemKodListesi">İşlem yapılacak işlem kod listesi.</param>
        /// <returns>İstenilen kriterdeki, tertiplere ait aylık harcama bilgisi listesini AylikDagilim nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray HarcamaTertipAylikDagilim(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, string kurKod, int[] islemKodListesi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, toplam tertip harcama bilgisini veritabanından okur 
        /// ve double olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebeKod">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birimKod">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="kurKod">İşlem yapılacak kurumsal kod bilgisi.</param>
        /// <param name="islemKodListesi">İşlem yapılacak işlem kod listesi.</param>
        /// <returns>İstenilen kriterdeki, toplam tertip harcama bilgisini double olarak döner</returns>
        double HarcamaTertipToplam(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, string kurKod, int[] islemKodListesi);
        /// <summary>
        /// Arguman olarak verilen, kur bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Kaydedilecek bilgileri KurBilgisi nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KurBilgisiKaydet(Kullanici kullanan, ObjectArray liste);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kur bilgisini veritabanından okur 
        /// ve KurBilgisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, kur bilgisi listesini KurBilgisi nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray KurBilgisiListele(Kullanici kullanan, KurBilgisi kriter);
        /// <summary>
        /// İhaleye ait belge bilgisini veritabanına kaydeder ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc IhaleBelgeKaydet(Kullanici kullanan, IhaleBelge bilgi);
        /// <summary>
        /// Verilen kriterlere bağlı ihale belge bilgisini veritabanından okur ve IhaleBelge nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>Verilen kriterlere ait ihale belge bilgisini tutan nesne</returns>
        IhaleBelge IhaleBelgeListele(Kullanici kullanan, IhaleBelge bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, borcu yok belgesi durumuna göre borcu yok belgelerini bir sonraki duruma taşır
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="belge">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc BorcuYokBelgeleriGonder(BorcuYokBelgesi belge);
        /// <summary>
        /// Arguman olarak verilen, borcu yok belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belge">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc BorcuYokKaydet(Kullanici kullanan, BorcuYokBelgesi belge);

        /// <summary>
        /// Sırada bekleyen belgeler olduğu zaman dilekçe bitiş tarihi bugünden farklı ise
        /// dilekçe tekrar sorgulanamıyor.Bu nedenle dilekçenin silinip tekrar sorgulanması gerekmektedir
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="belgeler"></param>
        /// <returns></returns>
        Sonuc BorcuYokSil(Kullanici kullanan, TNSCollection belgeler);
        /// <summary>
        /// Arguman olarak verilen, borcu belge durum bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc BorcuYokBelgeDurumDegistir(Kullanici kullanan, BorcuYokBelgesi bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, borcu yok belge bilgisini veritabanından okur 
        /// ve BorcuYokBelgesi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, borcu yok belge bilgisi listesini BorcuYokBelgesi nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray BorcuYokListele(Kullanici kullanan, BorcuYokBelgesi kriter);

        /// <summary>
        /// Arguman olarak verilen, borcu yok belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belge">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc BorcuYokSGKKaydet(Kullanici kullanan, BorcuYokBelgesiSGK belge);
        /// <summary>
        /// Arguman olarak verilen, borcu belge durum bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc BorcuYokSGKBelgeDurumDegistir(Kullanici kullanan, BorcuYokBelgesiSGK bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, borcu yok belge bilgisini veritabanından okur 
        /// ve BorcuYokBelgesi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, borcu yok belge bilgisi listesini BorcuYokBelgesi nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray BorcuYokSGKListele(Kullanici kullanan, BorcuYokBelgesiSGK kriter);

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, borcu yok belge bilgisini veritabanından okur 
        /// ve BorcuYokBelgesi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, borcu yok belge bilgisini BorcuYokBelgesi nesnesi olarak tutan nesne</returns>
        BorcuYokBelgesi BorcuYokAc(Kullanici kullanan, BorcuYokBelgesi kriter);
        /// <summary>
        /// Gelirler İdaresi Başkanlığından alınan servislerini kullanma bilgilerini veritabanına kaydeder
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="sorgulanan">İşlem yapılacak sorgulanan bilgisi.</param>
        /// <param name="islemYapan">İşlem yapılacak işlem yapan kullanıcı adı bilgisi.</param>
        /// <param name="aciklama">İşlem yapılacak aciklama bilgisi.</param>
        /// <param name="kurum">İşlem yapılacak kurum bilgisi.</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc GIBLogKaydet(string sorgulanan, string islemYapan, string aciklama, string kurum);
        /// <summary>
        /// Arguman olarak verilen, avans tutarı hesaplamasında kullanılan avans hesap bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc AvansHesapKaydet(Kullanici kullanan, ObjectArray liste);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, avans tutarı hesaplamasında kullanılan avans hesap bilgisini veritabanından okur 
        /// ve AvansHesap nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, avans tutarı hesaplamasında kullanılan avans hesap bilgisi listesini AvansHesap nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray AvansHesapListele(Kullanici kullanan, AvansHesap kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura bilgisini ilgili web servisden okur
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc ElektrikFaturaAktar(Kullanici kullanan, int yil, int ay);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, su fatura bilgisini ilgili web servisden okur
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SuFaturaAktar(Kullanici kullanan, int yil, int ay);
        /// <summary>
        /// Arguman olarak verilen, abone fatura bilgisine ait süreç oluşturma işlemlerini yapar
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="aboneTip">İşlem yapılacak abone tip bilgisi.</param>
        void AboneServisiSurecOlustur(Kullanici kullanan, int yil, int ay, int aboneTip);
        /// <summary>
        /// Arguman olarak verilen, abone fatura bilgisine ait ödeme emri belgesi oluşturma işlemlerini yapar
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="aboneTip">İşlem yapılacak abone tip bilgisi.</param>
        void AboneServisiOdemeEmriOlustur(Kullanici kullanan, int yil, int ay, int aboneTip);
        /// <summary>
        /// Arguman olarak verilen, abone fatura bilgisine ait ödenek gönderme belgesi oluşturma işlemlerini yapar
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="aboneTip">İşlem yapılacak abone tip bilgisi.</param>
        void AboneServisiOdenekGonderme(Kullanici kullanan, int yil, int ay, int aboneTip);
        /// <summary>
        /// Arguman olarak verilen, abone fatura bilgisine ait mail gönderme işlemlerini yapar
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi.</param>
        /// <param name="aboneTip">İşlem yapılacak abone tip bilgisi.</param>
        void AboneServisiMailGonder(Kullanici kullanan, int yil, int ay, int aboneTip);
        /// <summary>
        /// Arguman olarak verilen, avans artığı bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc AvansArtigiKaydet(Kullanici kullanan, AvansArtigi bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, avans artığı bilgisini veritabanından okur 
        /// ve AvansArtigi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, avans artığı listesini AvansArtigi nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray AvansArtigiListele(Kullanici kullanan, AvansArtigi kriter);
        /// <summary>
        /// Arguman olarak verilen, avans artığı bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc AvansArtigiSil(Kullanici kullanan, AvansArtigi kriter);
        /// <summary>
        /// Arguman olarak verilen, süreç parça bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SurecParcaKaydet(Kullanici kullanan, SurecParca bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç parça bilgisini veritabanından okur 
        /// ve SurecParca nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, süreç parça bilgisini SurecParca nesnesi olarak tutan nesne</returns>
        SurecParca SurecParcaListele(Kullanici kullanan, SurecParca kriter);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç parça bilgisini veritabanından okur 
        /// ve SurecParca nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, süreç parça listesini SurecParca nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray SurecParcaListelee(Kullanici kullanan, SurecParca kriter);
        /// <summary>
        /// Arguman olarak verilen, süreç parça bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SurecParcaSil(Kullanici kullanan, SurecParca bilgi);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sürecin yıllara sari olup olmadığı bilgisini veritabanından okur 
        /// ve bool olarak döner
        /// </summary>
        /// <param name="surecYil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <returns>İstenilen kriterdeki, sürecin yıllara sari olup olmadığı bilgisini bool olarak döner</returns>
        bool YillaraSariIslemMi(int surecYil, string surecNo);
        /// <summary>
        /// Arguman olarak verilen, şartname ihale ilan bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SartnameIhaleIlanBilgisiKaydet(Kullanici kullanan, IhaleIlanBilgisi bilgi);
        /// <summary>
        /// MerkezBankasiKurSinif nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MerkezBankasiKurSinif yapısında MerkezBankasiKurSinif: bilgilerini tutan değişken</param>
        MerkezBankasiKurSinif MerkezBankKurOku(Kullanici kullanan, MerkezBankasiKurSinif form);
        /// <summary>
        /// Verilen tarih aralığındaki baslangıç ve bitiş günleride dahil bütün günlerin kur
        /// değerleri Merkez bankasından alınarak sisteme yazılır
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tarihBas">Başlangıç tarihi</param>
        /// <param name="tarihBit">Bitiş tarihi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc MerkezBankKurKaydet(Kullanici kullanan, TNSDateTime tarihBas, TNSDateTime tarihBit);
        /// <summary>
        /// MerkezBankasiKurSinif nesnesini veri tabanından okuyan fonksiyon ve içinde MerkezBankasiKurSinif nesleri tutan ObjectArray döndürür
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="form">MerkezBankasiKurSinif yapısında MerkezBankasiKurSinif: bilgilerini tutan değişken</param>
        /// <param name="kurlar">Hangi kurların gösterileceği bilgisi</param>
        ObjectArray MerkezBankKurDegerleri(MerkezBankasiKurSinif form, string[] kurlar);
        /// <summary>
        /// SurecSifreSinif nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SurecSifreSinif yapısında SurecSifreSinif: bilgilerini tutan değişken</param>
        SurecSifreSinif SurecSifreOku(Kullanici kullanan, SurecSifreSinif form);
        /// <summary>
        /// Arguman olarak verilen, süreç şifre bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SurecSifreSil(Kullanici kullanan, SurecSifreSinif form);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, süreç şifre bilgisini veritabanından okur 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kriterSinif">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SurecSifreDogrula(SurecSifreSinif kriterSinif);
        /// <summary>
        /// SurecSifreSinif nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SurecSifreSinif yapısında SurecSifreSinif: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SurecSifreKaydet(Kullanici kullanan, SurecSifreSinif form);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yurt içi geçici görev yolluk istatistik bilgisini veritabanından okur 
        /// ve YIGGYIstatistik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tur">İşlem yapılacak tür bilgisi.</param>
        /// <returns>İstenilen kriterdeki, yurt içi geçici görev yolluk istatistik bilgisi listesini YIGGYIstatistik nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray ListeleYIGGYIstatistik(int yil, ENUMYIGGYTur tur);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yurt dışı geçici görev yolluk istatistik bilgisini veritabanından okur 
        /// ve YDGGYIstatistik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tur">İşlem yapılacak tür bilgisi.</param>
        /// <returns>İstenilen kriterdeki, yurt dışı geçici görev yolluk istatistik bilgisi listesini YDGGYIstatistik nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray ListeleYDGGYIstatistik(int yil, ENUMYDGGYTur tur);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yurt içi sürekli görev yolluk istatistik bilgisini veritabanından okur 
        /// ve YISGYIstatistik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tur">İşlem yapılacak tür bilgisi.</param>
        /// <returns>İstenilen kriterdeki, yurt içi geçici görev yolluk istatistik bilgisi listesini YISGYIstatistik nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray ListeleYISGYIstatistik(int yil, ENUMYISGYTur tur);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yurt dışı sürekli görev yolluk istatistik bilgisini veritabanından okur 
        /// ve YDSGYIstatistik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="tur">İşlem yapılacak tür bilgisi.</param>
        /// <returns>İstenilen kriterdeki, yurt dışı sürekli görev yolluk istatistik bilgisi listesini YDSGYIstatistik nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray ListeleYDSGYIstatistik(int yil, ENUMYDSGYTur tur);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, elektrik fatura istatistik bilgisini veritabanından okur 
        /// ve ElektrikIstatistik nesnesi olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, elektrik fatura istatistik bilgisini ElektrikIstatistik nesnesi olarak tutan nesne</returns>
        ElektrikIstatistik ListeleElektrikIstatistik(int yil, int muhasebe, string birim);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, su fatura istatistik bilgisini veritabanından okur 
        /// ve SuIstatistik nesnesi olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, su fatura istatistik bilgisini SuIstatistik nesnesi olarak tutan nesne</returns>
        SuIstatistik ListeleSuIstatistik(int yil, int muhasebe, string birim);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, telekom fatura istatistik bilgisini veritabanından okur 
        /// ve TelekomIstatistik nesnesi olarak döner
        /// </summary>
        /// <param name="tip">İşlem yapılacak abone tip bilgisi.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, telekom fatura istatistik bilgisini TelekomIstatistik nesnesi olarak tutan nesne</returns>
        TelekomIstatistik ListeleTelekomIstatistik(ENUMAboneTip tip, int yil, int muhasebe, string birim);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, onay belgesi harcama bilgisini veritabanından okur 
        /// ve BHMHarcama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılcak ay bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <param name="peb">İşlem yapılacak alt faaliyet kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, onay belgesi harcama bilgisi listesini BHMHarcama nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray BHMOnayBelgesiHarcamaListesi(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, alım harcama bilgisini veritabanından okur 
        /// ve BHMHarcama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılcak ay bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <param name="peb">İşlem yapılacak alt faaliyet kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, alım harcama bilgisi listesini BHMHarcama nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray BHMAlimHarcamaListesi(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, fatura harcama bilgisini veritabanından okur 
        /// ve BHMHarcama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılcak ay bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <param name="peb">İşlem yapılacak alt faaliyet kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, fatura harcama bilgisi listesini BHMHarcama nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray BHMFaturaHarcamaListesi(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb);
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, taşınır harcama bilgisini veritabanından okur 
        /// ve BHMHarcama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi.</param>
        /// <param name="ay">İşlem yapılcak ay bilgisi.</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi.</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <param name="peb">İşlem yapılacak alt faaliyet kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, taşınır harcama bilgisi listesini BHMHarcama nesne koleksiyonu olarak tutan nesne</returns>
        ObjectArray BHMTeslimHarcamaListesi(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb);

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
        //ObjectArray BHMOdemeHarcamaListesi(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb);

        ObjectArray SurecBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter);
        ObjectArray OnayBelgesiBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter);
        ObjectArray OnayBelgesiTertipBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter);
        ObjectArray AvansFormBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter);
        ObjectArray TeklifEdilenFiyatBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter);
        ObjectArray PiyasaFiyatBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter);
        ObjectArray PiyasaFiyatBilgiToplamListesi(Kullanici kullanan, SurecSorguKriter kriter);
        ObjectArray KazananFirmaBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter);
        ObjectArray HakedisBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter);
        ObjectArray FaturaBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter);
        ObjectArray TasinirBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter);
        ObjectArray IhaleIlanBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter);
        ObjectArray IhaleSozlesmeSonuUcretBilgiListesi(Kullanici kullanan, SurecSorguKriter kriter);
        ObjectArray MufYaklasikMaliyetListesi(Kullanici kullanan, SurecSorguKriter kriter);

        Sonuc MPRevizeNoTalepNoSil(Kullanici kullanan, MPRevizeNoTalepNo bilgi);
        Sonuc MPRevizeNoTalepNoKaydet(Kullanici kullanan, MPRevizeNoTalepNo bilgi);
        MPRevizeNoTalepNo MPRevizeNoTalepNoListele(Kullanici kullanan, MPRevizeNoTalepNo kriter);

        //decimal BlokeHesapla(Kullanici kullanan, int yil, string surecNo);

        SeyyarGorevDurumAna SeyyarGorevDurumBul(Kullanici kullanan, int yil, int saymanlik, string birim, string kurKod, int islemKod, int surecYil, string surecNo, string oeNo);
        Sonuc SeyyarGorevKaydet(Kullanici kullanan, int yil, int asama, SeyyarGorevI[] sg);
        Sonuc SeyyarGorevOnayKaydet(Kullanici kullanan, SeyyarGorevI sg);
        Sonuc SeyyarGorevOnayKaldir(Kullanici kullanan, SeyyarGorevI sg);
        ObjectArray SeyyarGorevListele(Kullanici kullanan, int yil, string harcamabirimi, int asama);
        ObjectArray HarcamaBirimiPersonelListesi(Kullanici kullanan, string harcamaBirmi);
        // ObjectArray SeyyarGorevUnvanListele(Kullanici kullanan, string harcamabirimi);
        ObjectArray SeyyarGorevAdamGunTespitListele(Kullanici kullanan, int yil, string harcamabirimi, int asama, bool harcamaOlsun, bool perSayOlsun, bool unvanlarOlsun);
        Sonuc SeyyarGorevAdamGunTespitKaydet(Kullanici kullanan, SeyyarGorevI sg);
        ObjectArray SeyyarGorevUnvanTespitListele(Kullanici kullanan, int yil, int istihdamSekli, int unvanKod, int asama, string harcamaBirimi);
        Sonuc SeyyarGorevUnvanTespitKaydet(Kullanici kullanan, SeyyarGorevI sg);
        ObjectArray PersonelHarcamaBirimiListele(Kullanici kullanan, int isthidamSekli, int unvanKodu);

        ObjectArray RaporIhaleFirmaListele(Kullanici kullanan, RaporIhaleFirma kriter);

        Sonuc TurkcellLogKaydet(Kullanici kullanan, TurkcellLog bilgi);
        ObjectArray TurkcellLogListele(Kullanici kullanan, TurkcellLog kriter);
        string TurkcellLogDosyaAdiAl(Kullanici kullanan);

        ObjectArray FazlaMesaiPuantajUnvanListele(Kullanici kullanan);
        Sonuc FazlaMesaiPuantajUnvanKaydet(Kullanici kullanan, ObjectArray liste);

        ObjectArray FazlaMesaiBordroUnvanListele(Kullanici kullanan, FazlaMesaiBordroUnvan kriter);
        Sonuc FazlaMesaiBordroUnvanKaydet(Kullanici kullanan, ObjectArray liste);

        Sonuc GunlukKurKaydet(Kullanici kullanan, MerkezBankasiKurSinif form);

        Sonuc GeciciGorevYollukTopluBelgeKaydet(Kullanici kullanan, ObjectArray liste, int islemKod, int parcaKod);

        Sonuc MerkezBankSerbestKurGuncelle(Kullanici kullanan, MerkezBankasiKurSinif form);

        Sonuc TeminatMektubuKaydet(Kullanici kullanan, TeminatMektubu bilgi);
        ObjectArray TeminatMektubuListele(Kullanici kullanan, TeminatMektubu bilgi);
        Sonuc TeminatMektubuSil(Kullanici kullanan, TeminatMektubu bilgi);
        ObjectArray TeminatMektubuCikisListele(Kullanici kullanan, TeminatMektubuCikisBilgileri bilgi);
        Sonuc TeminatMektubuGirisOdemeEmriGuncelle(Kullanici kullanan, int dosyaYil, string dosyaNo, int belgeYil, string belgeNo, TNSDateTime belgeTarihi);
        Sonuc TeminatMektubuCikisOdemeEmriGuncelle(Kullanici kullanan, int cikisKod, int belgeYil, string belgeNo, TNSDateTime belgeTarihi);

        Sonuc UAAvansTalepKaydet(Kullanici kullanan, UAAvansTalep bilgi);
        UAAvansTalep UAAvansTalepOku(Kullanici kullanan, UAAvansTalep kriter);
        double UAAvansFaturaTopla(Kullanici kullanan, UAAvansTalep kriter);
        Sonuc UAAvansTalepKapat(Kullanici kullanan, UAAvansKapatma bilgi);
        UAAvansKapatma UAAvansKapatmaOku(Kullanici kullanan, UAAvansKapatma kriter);

        Sonuc SatinAlmaTalepFormKaydet(Kullanici kullanan, SatinAlmaTalepForm bilgi);

        ObjectArray SatinAlmaTalepFormListele(Kullanici kullanan, SatinAlmaTalepForm kriter);
        Sonuc SatinAlmaTalepFormDurumGuncelle(Kullanici kullanan, int yil, string formKod, ENUMSatinalmaTalepFormDurum durum);
        Sonuc SatinAlmaTalepFormSurecBilgiGuncelle(Kullanici kullanan, int yil, string formKod, int surecYil, string surecNo);
        ObjectArray SatinAlmaTalepFormSorgula(Kullanici kullanan, SatinAlmaTalepFormSorgu kriter, bool SureceBaglanacakAvansliAlimMi, string surecNo);

        TNSCollection SatinAlmaTalepFormIhtiyacDetayListele(Kullanici kullanan, int yil, string formKod, int siraNo = 0);
        TNSCollection SatinAlmaTalepFormIhtiyacDetayListeleSurec(Kullanici kullanan, int yil, string surecNo, int siraNo = 0);
        Sonuc SatinAlmaTalepFormTeklifKaydet(Kullanici kullanan, int yil, string surecNo, int firmaKod, int tip, TNSCollection bilgi);
        ObjectArray SatinAlmaTalepFormTeklifListele(Kullanici kullanan, int yil, string surecNo, int firmaKod, int tip, int siraNo);

        Sonuc GeciciGorevYollukTalepBaslikKaydet(Kullanici kullanan, GeciciGorevYollukTalepBaslik bilgi);

        ObjectArray GeciciGorevYollukTalepBaslikListele(Kullanici kullanan, GeciciGorevYollukTalepBaslik kriter);

        Sonuc GeciciGorevYollukTalepOnaylandi(Kullanici kullanan, GeciciGorevYollukTalepBaslik form);

        Sonuc IhtiyacPlaniKaydet(Kullanici kullanan, ObjectArray ihtiyacPlani);

        Sonuc IhtiyacPlaniKaydet(Kullanici kullanan, IhtiyacPlaniSatir ihtiyacPlani);

        Sonuc IhtiyacPlaniSil(Kullanici kullanan, IhtiyacPlaniSatir ihtiyacPlani);

        Sonuc IhtiyacPlaniSil(Kullanici kullanan, ObjectArray ihtiyacPlani);

        ObjectArray IhtiyacPlaniListele(Kullanici kullanan, IhtiyacPlaniSatir ihtiyacPlaniSatir);

        string TalepNoBul(int yil, string surecNo);

        int HizmetAlimTurBul(int isKod);

        string IsEkoKodBul(int isKod);

        SatinAlmaTalepIhtiyacBaslik SatinAlmaTalepIhtiyacBaslikAc(Kullanici kullanan, int yil, string formKod);

        void IhtiyacSatiriAdDegistir(Kullanici kullanan, int yil, string formKod, int siraNo, string yeniDeger);

        Sonuc IhtiyacSatiriDetayKaydet(Kullanici kullanan, int yil, string formKod, int siraNo, TNSCollection ihtiyacListesi);

        Sonuc IhtiyacSatiriSil(Kullanici kullanan, int yil, string formKod, int siraNo);

        Sonuc UAAvansTalepOEIliskiKur(Kullanici kullanan, int surecYil, string surecNo, string odemeEmriNo);

        Sonuc UAAvansKapatmaOEIliskiKur(Kullanici kullanan, int surecYil, string surecNo, string odemeEmriNo);

        Sonuc UAAvansKalanKapatmaOEIliskiKur(Kullanici kullanan, int surecYil, string surecNo, string kalanOdemeEmriNo);

        OnMaliKontrolAna OnMaliKontrolListele(Kullanici kullanan, int surecYil, string surecNo, ENUMOnMaliKontrolTur onMaliKontrolTur, string odemeEmriNo);

        Sonuc OnMaliKontrolKaydet(Kullanici kullanan, OnMaliKontrolAna omk);

        ObjectArray AlimlariListele(Kullanici kullanan, HizmetAlimi bilgi);

        ObjectArray SatinAlmaTalepListeleBaslik(Kullanici kullanan, int yil, string formKod, int siraNo, bool detayVer);

        Sonuc CesitliOdemeBankaListesiKaydet(Kullanici kullanan, CesitliOdeme co, TNSDateTime bankaOdemeGun, int bankaOdemeSiraNo);

        /// <summary>
        /// KismiGeciciKabul nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KismiGeciciKabul yapısında KismiGeciciKabul: bilgilerini tutan değişken</param>
        KismiGeciciKabul KismiGeciciKabulOku(Kullanici kullanan, KismiGeciciKabul form);

        /// <summary>
        /// KismiGeciciKabul nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KismiGeciciKabul yapısında KismiGeciciKabul: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KismiGeciciKabulKaydet(Kullanici kullanan, KismiGeciciKabul form);

        //ObjectArray KazananGetir(Kullanici kullanan, int yil, string surecNo, int malzemeNo);

        /// <summary>
        /// IhaleKabul nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IhaleKabul yapısında IhaleKabul: bilgilerini tutan değişken</param>
        IhaleKabul KabulFormOku(Kullanici kullanan, IhaleKabul form);

        /// <summary>
        /// IhaleKabul nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IhaleKabul yapısında IhaleKabul: bilgilerini tutan değişken</param>
        IhaleKabul[] KabulFormListele(Kullanici kullanan, IhaleKabul form);

        /// <summary>
        /// IhaleKabul nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IhaleKabul yapısında IhaleKabul: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KabulFormKaydet(Kullanici kullanan, IhaleKabul form);

        Sonuc KabulFormMalzemeKaydet(Kullanici kullanan, int surecYil, string surecNo, int tutanakNo, IhaleKabulMalzeme[] malListesi);

        IhaleKabulMalzeme[] KabulFormMalzemeListele(Kullanici kullanan, int surecYil, string surecNo, int tutanakNo);

        TeklifKazananFirma KazananGetir(Kullanici kullanan, int yil, string surecNo, int malzemeNo);

        /// <summary>
        /// CalisanBordroAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CalisanBordroAna yapısında CalisanBordroAna: bilgilerini tutan değişken</param>
        CalisanBordroAna CalisanBordroOku(Kullanici kullanan, CalisanBordroAna form);

        /// <summary>
        /// CalisanBordroAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CalisanBordroAna yapısında CalisanBordroAna: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc CalisanBordroKaydet(Kullanici kullanan, CalisanBordroAna form);

        /// <summary>
        /// CalisanBordroGelirKesinti nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CalisanBordroGelirKesinti yapısında CalisanBordroGelirKesinti: bilgilerini tutan değişken</param>
        CalisanBordroGelirKesinti[] CalisanBordroGelirKesintiListele(Kullanici kullanan);

        /// <summary>
        /// CalisanBordroGelirKesinti nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">CalisanBordroGelirKesinti yapısında CalisanBordroGelirKesinti: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc CalisanBordroGelirKesintiKaydet(Kullanici kullanan, CalisanBordroGelirKesinti form);

        /// <summary>
        /// Verilen kullanıcının ilgili sürece yetkisinin olup olmadığını döndürür.
        /// </summary>
        /// <param name="kullanan">Yetkisi kontrol edilecek kişi</param>
        /// <param name="surecYil">Süreci bulmak için sürecin yılı</param>
        /// <param name="surecNo">Süreci bulmak için sürecin numarası</param>
        /// <param name="yetkiTipi">Kontrol edilecek yetki türü</param>
        /// <returns>Yetkisi vrsa Sonuc değişkeninde true yoksa false döndürür.</returns>
        Sonuc SurecYetkisiVarMi(Kullanici kullanan, int surecYil, string surecNo, ENUMKullaniciYetkiTipi yetkiTipi);

        CalisanBordroAna BordroViewOku(Kullanici kullanan, string tarih, string kume);

        Sonuc BordroOdemeEmriKaydet(Kullanici kullanan, string surecNo, int surecYil, string olusanOdemeEmriNo);

        /// <summary>
        /// PSLGeciciGorevUcak listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PSLGeciciGorevUcak yapısında PSLGeciciGorevUcak:  bilgilerini tutan değişken</param>
        PSLGeciciGorevUcak[] ListelePSLGeciciGorevUcak(Kullanici kullanan, PSLGeciciGorevUcak form);

        /// <summary>
        /// PSLGeciciGorevUcak nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PSLGeciciGorevUcak yapısında PSLGeciciGorevUcak:  bilgilerini tutan değişken</param>
        PSLGeciciGorevUcak OkuPSLGeciciGorevUcak(Kullanici kullanan, PSLGeciciGorevUcak form);

        /// <summary>
        /// PSLGeciciGorevUcak nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">PSLGeciciGorevUcak yapısında PSLGeciciGorevUcak:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetPSLGeciciGorevUcak(Kullanici kullanan, PSLGeciciGorevUcak form);

        Sonuc BordroBankaListesiKaydet(Kullanici kullanan, string surecNo, int surecYil, TNSDateTime bankaOdemeGun, int bankaOdemeSiraNo);

        Sonuc CesitliOdemeAvansOdemeEmriNoKaydet(Kullanici kullanan, CesitliOdeme co);

        Sonuc CesitliOdemeAvansBankaListesiKaydet(Kullanici kullanan, CesitliOdeme co, TNSDateTime avansBankaOdemeGun, int avansBankaOdemeSiraNo);

        /// <summary>
        /// DovizOdeme listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DovizOdeme yapısında DovizOdeme:  bilgilerini tutan değişken</param>
        DovizOdeme[] ListDovizOdeme(Kullanici kullanan, DovizOdeme form);

        /// <summary>
        /// DovizOdeme nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DovizOdeme yapısında DovizOdeme:  bilgilerini tutan değişken</param>
        DovizOdeme GetDovizOdeme(Kullanici kullanan, DovizOdeme form);

        /// <summary>
        /// DovizOdeme nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DovizOdeme yapısında DovizOdeme:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SaveDovizOdeme(Kullanici kullanan, DovizOdeme form);

        Sonuc DovizOdemeOdemeEmriNoKaydet(Kullanici kullanan, DovizOdeme form);

        /// <summary>
        /// AvukatOdemesi listesini veri tabanýndan okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluþturur
        /// </summary>
        /// <param name="kullanan">Kullanýcý bilgilerini içeren nesne</param>
        /// <param name="form">AvukatOdemesi yapýsýnda AvukatOdemesi:  bilgilerini tutan deðiþken</param>
        AvukatOdemesi[] ListeleAvukatOdemesi(Kullanici kullanan, AvukatOdemesi form);

        /// <summary>
        /// AvukatOdemesi nesnesini veri tabanýndan okuyan fonksiyon
        /// Hata olursa Exception oluþturur
        /// </summary>
        /// <param name="kullanan">Kullanýcý bilgilerini içeren nesne</param>
        /// <param name="form">AvukatOdemesi yapýsýnda AvukatOdemesi:  bilgilerini tutan deðiþken</param>
        AvukatOdemesi OkuAvukatOdemesi(Kullanici kullanan, AvukatOdemesi form);

        /// <summary>
        /// AvukatOdemesi nesnesini veri tabanýna yazan fonksiyon
        /// Hata olursa Exception oluþturur
        /// </summary>
        /// <param name="kullanan">Kullanýcý bilgilerini içeren nesne</param>
        /// <param name="form">AvukatOdemesi yapýsýnda AvukatOdemesi:  bilgilerini tutan deðiþken</param>
        /// <returns>Ýþlem sonucunu döndüren, Sonuc sýnýfýnda nesne</returns>
        Sonuc KaydetAvukatOdemesi(Kullanici kullanan, AvukatOdemesi form);

        Sonuc AvukatOdemesiOdemeEmriNoKaydet(Kullanici kullanan, AvukatOdemesi form);

        SurecOzetBilgi SurecOzetBilgiBul(Kullanici kullanan, int surecYil, string surecNo);

        Sonuc YollukBelgeSil(Kullanici kullanan, int yil, string surecNo);

        Sonuc TeklifMalzemeDetaySil(Kullanici kullanan, int yil, string surecNo, int firmaKod, int tip, int siraNo);

        Sonuc BordroYapiSakla(Kullanici kullanan, BordroYapi bordroYapi);

        BordroYapi BordroYapiOku(Kullanici kullanan, int bordroYapi, int islemTur, bool sadeceKedisi);
        ObjectArray BordroYapiDegiskenListele(Kullanici kullanan, int islemTur);
        ObjectArray BordroYapiDegiskenOku(Kullanici kullanan, int islemTur, string formulAd);
        CalisanDinamikBordro CalisanDinamikBordroOku(Kullanici kullanan, BordroYapi bordroYapi, BordroKistas bordroKistas);
        CalisanDinamikBordroDonem OncekiDonemBul(int surecYil, string surecNo);
        Sonuc CalisanDinamikBordroKaydet(Kullanici kullanan, int yil, string surecNo, BordroYapi bordroYapi, System.Collections.ArrayList satirlar);
        Sonuc CalisanDinamikBordroOnayla(Kullanici kullanan, int yil, string surecNo, BordroYapi bordroYapi, int onayKod);
        Sonuc CalisanDinamikBordroOnayliMi(Kullanici kullanan, int yil, string surecNo, BordroYapi bordroYapi);

        /// <summary>
        /// Verilen yıldaki yapılan sözleşmeler ve onay belgeleri kullanılarak yüzde on bilgisi bulunur.
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="yil"></param>
        /// <returns></returns>
        ObjectArray YuzdeOnListele(Kullanici kullanan, int yil);

        /// <summary>
        /// FaturaAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">FaturaTum yapısında aranacak fatura  bilgilerini tutan değişken</param>
        FaturaTum[] ListeleFaturaTum(Kullanici kullanan, FaturaTum form);

        /// <summary>
        /// DogrudanTeminPlan listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DogrudanTeminPlan yapısında DogrudanTeminPlan:  bilgilerini tutan değişken</param>
        ObjectArray ListeleDogrudanTeminPlan(Kullanici kullanan, DogrudanTeminPlan form);

        ObjectArray IstatistikDogrudanTeminPlan(Kullanici kullanan, int yil, string senaryoKod);

        /// <summary>
        /// DogrudanTeminPlan nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DogrudanTeminPlan yapısında DogrudanTeminPlan:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetDogrudanTeminPlan(Kullanici kullanan, int yil, string senaryoKod, int saymanlik, string birim, string harcamaTuru, DogrudanTeminPlan[] form);

        /// <summary>
        /// YuzdeOnKapatAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YuzdeOnKapatAna yapısında YuzdeOnKapatAna:  bilgilerini tutan değişken</param>
        ObjectArray ListeleYuzdeOnKapat(Kullanici kullanan, YuzdeOnKapat form);

        /// <summary>
        /// YuzdeOnKapatAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">YuzdeOnKapatAna yapısında YuzdeOnKapatAna:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetYuzdeOnKapat(Kullanici kullanan, YuzdeOnKapat form);

        Sonuc YuzdeOnAcikMi(Kullanici kullanan, int yil, string ihaleMaddeAdi, string malHizmetYapim, TNSCollection tertipler, int saymanlikKodu, string birimKodu);

        ObjectArray ListeleSeyyarGorevIlTavan(Kullanici kullanan, int yil, string senaryoKod, TNS.BKD.TavanGenel kistas);

        /// <summary>
        /// SeyyarGorevRevize nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Okunacak SeyyarGorevRevize formunun yıl değeri</param>
        /// <param name="belgeTur">Okunacak SeyyarGorevRevize formunun belge türü</param>
        /// <param name="belgeNo">Okunacak SeyyarGorevRevize formunun belge numarası</param>
        SeyyarGorevRevize OkuSeyyarGorevRevize(Kullanici kullanan, int yil, ENUMRevizeFormBelgeTur belgeTur, int belgeNo, bool tumIl, bool onaylilar);

        /// <summary>
        /// SeyyarGorevRevize nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">SeyyarGorevRevize yapısında SeyyarGorevRevize:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetSeyyarGorevRevize(Kullanici kullanan, SeyyarGorevRevize form);

        BKD.TavanGenel SeyyarGorevBilgiBul(Kullanici kullanan, int yil, string ilKod, int haricBelgeNoDagitilan);

        //ObjectArray SeyyarGorevBilgiListele(Kullanici kullanan, int yil, string ilKod, string belgeNo, int haricBelgeNoDagitilan);

        ObjectArray SeyyarGorevIslemListele(Kullanici kullanan, SeyyarGorevRevize kistasForm, bool onaylilar);

        Sonuc DurumAtaSeyyarGorevRevize(Kullanici kullanan, SeyyarGorevRevize revForm);

        ObjectArray SeyyarGorevBilgiBulBirim(Kullanici kullanan, int yil, TNSCollection birimListesi, int belgeNo, bool onaylilar);

        //ObjectArray HarcamaBirimiListele(Kullanici kullanan, MuhasebeBirim muhasebeBirim, bool kurumsalYapili);

        ObjectArray ListeleSeyyarGorevRevize(Kullanici kullanan, SeyyarGorevRevize kistasForm);

        Sonuc DurumDegistirSeyyarGorevRevize(Kullanici kullanan, TNSCollection belgeler);

        Sonuc BordroSablonSakla(Kullanici kullanan, BordroSablonDosya bsd, bool yeni);

        ObjectArray BordroSablonListele(Kullanici kullanan, int bordroTur, int islemTur, ENUMUCDURUM sayfa, ENUMUCDURUM ortakRapor);

        ObjectVAN CalisanDinamikBordroDonemOku(Kullanici kullanan, CalisanDinamikBordroDonem calisanDinamikBordroDonem);

        Sonuc CalisanDinamikBordroDonemKaydet(Kullanici kullanan, CalisanDinamikBordroDonem calisanDinamikBordroDonem);

        BordroMuhasebeKayitlari CalisanDinamikBordroHesapOku(Kullanici kullanan, int yil, string surecNo, TNSCollection bordroTurler, int bordroIslemTur, CalisanDinamikBordroDonem bordroDonem, int avansIslemKod, string odemeEmriHesapKod);

        Sonuc CalisanDinamikBordroVeriAktar(Kullanici kullanan, int yil, string surecNo, TNSCollection bordroTurler, int bordroIslemTur, int aktar);

        Sonuc TeminatMektubuNeviKaydet(Kullanici kullanan, TeminatMektubuNevi bilgi);
        ObjectArray TeminatMektubuNeviListele(Kullanici kullanan, TeminatMektubuNevi bilgi);
        Sonuc TeminatMektubuNeviSil(Kullanici kullanan, TeminatMektubuNevi bilgi);

        ObjectArray ListeleBordroTabloAna(Kullanici kullanan);

        /// <summary>
        /// BordroTabloTanim listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroTabloTanim yapısında BordroTabloTanim: Bordro Tablo Tanım bilgilerini tutan değişken</param>
        ObjectArray ListeleBordroTablo(Kullanici kullanan, BordroTabloDetay form);

        /// <summary>
        /// BordroTabloTanim nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroTabloTanim yapısında BordroTabloTanim: Bordro Tablo Tanım bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetBordroTablo(Kullanici kullanan, int tabloKod, int yil, int unvanKod, TNSCollection degerler);

        /// <summary>
        /// BordroTabloTanim nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroTabloTanim yapısında BordroTabloTanim: Bordro Tablo Tanım bilgilerini tutan değişken</param>
        decimal OkuBordroTablo(Kullanici kullanan, int tabloKod, int unvanKod, int derece, int yil);
        /// <summary>
        /// BordroParamAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroParamAna yapısında BordroParamAna: Bordro Param Ana bilgilerini tutan değişken</param>
        ObjectArray ListelePersonelBordroParam(Kullanici kullanan);

        /// <summary>
        /// BordroParamAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroParamAna yapısında BordroParamAna: Bordro Param Ana bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetPersonelBordroParam(Kullanici kullanan, TNSCollection degiskenler);

        /// <summary>
        /// BordroIslemTur nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroIslemTur yapısında BordroIslemTur: Bordro İşlem Tür bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetBordroIslemTur(Kullanici kullanan, BordroIslemTur form);

        /// <summary>
        /// BordroIslemTur nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroIslemTur yapısında BordroIslemTur: Bordro İşlem Tür bilgilerini tutan değişken</param>
        ObjectVAN OkuBordroIslemTur(Kullanici kullanan, BordroIslemTur form);

        /// <summary>
        /// BordroIslemTur nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BordroIslemTur yapısında BordroIslemTur: Bordro İşlem Tür bilgilerini tutan değişken</param>
        ObjectArray BordroIslemTurListele(Kullanici kullanan, BordroIslemTur form);

        Sonuc BordroTabloArttir(Kullanici kullanan, int tabloKod, int yil, double artisKatsayisi);

        Sonuc KaydetBordroTabloTanim(Kullanici kullanan, BordroTabloAna tabloTanim);
        Sonuc KopyalaBordroTablo(Kullanici kullanan, int yil, int hedefKod, int kaynakKod, int unvanKod, bool onceSil);

        Sonuc KaydetButceBildirim(Kullanici kullanan, ButceBildirim form);
        ButceBildirim[] ListeleButceBildirim(Kullanici kullanan, ButceBildirim form);
        Sonuc SilButceBildirim(Kullanici kullanan, ButceBildirim form);

        Sonuc ManasSatinAlmaIsTanimKaydet(Kullanici kullanan, ManasSatinAlmaIsTanim form);
        Sonuc ManasSatinAlmaIsTanimSil(Kullanici kullanan, ManasSatinAlmaIsTanim form);
        ManasSatinAlmaIsTanim[] ManasSatinAlmaIsTanimListele(Kullanici kullanan, ManasSatinAlmaIsTanim form);
        ManasSatinAlmaIsTanim ManasSatinAlmaIsTanimOku(Kullanici kullanan, ManasSatinAlmaIsTanim form);
        Sonuc ManasSatinAlmaOnayKaydet(Kullanici kullanan, ManasSatinAlmaIsTanim form);

        Sonuc ManasSatinAlmaGorevliKaydet(Kullanici kullanan, ManasSatinAlmaGorevli[] formList, int satinAlmaKod);
        ManasSatinAlmaGorevli[] ManasSatinAlmaGorevliListele(Kullanici kullanan, ManasSatinAlmaGorevli form);

        Sonuc ManasSatinAlmaUrunKaydet(Kullanici kullanan, ManasSatinAlmaUrun[] formList, int satinAlmaKod);
        ManasSatinAlmaUrun[] ManasSatinAlmaUrunListele(Kullanici kullanan, ManasSatinAlmaUrun form);

        Sonuc ManasSatinAlmaFirmaKaydet(Kullanici kullanan, ManasSatinAlmaFirma[] formList, int satinAlmaKod, int tur);
        ManasSatinAlmaFirma[] ManasSatinAlmaFirmaListele(Kullanici kullanan, ManasSatinAlmaFirma form);

        Sonuc ManasSatinAlmaTeklifKaydet(Kullanici kullanan, ManasSatinAlmaTeklif[] formList, ManasSatinAlmaTeklifForm tf);
        ManasSatinAlmaTeklifForm ManasSatinAlmaTeklifListele(Kullanici kullanan, ManasSatinAlmaTeklif form);

        Sonuc BordroMailiGonderEkle(Kullanici kullanan, int surecYil, string surecNo, BordroYapi bordroYapi, TNSCollection tcListe);
        ObjectArray BordroMailiBekleyenListele(Kullanici kullanan);

        Sonuc BordroMailBirak(Kullanici kullanan, BordroMail bm);

        Sonuc CalisanDinamikBordroSil(Kullanici kullanan, int surecYil, string surecNo, BordroYapi by, TNSCollection tcListe);

        Sonuc TeminatMektubuUyariGonder(Kullanici kullanan);
        ObjectArray CalisanDinamikBordroListele(Kullanici kullanan, BordroKistas bordroKistas);

        Sonuc KomisyonUyeSablonKaydet(Kullanici kullanan, KomisyonUyeSablon sablon);
        Sonuc KomisyonUyeSablonSil(Kullanici kullanan, KomisyonUyeSablon sablon);
        ObjectArray KomisyonUyeSablonListele(Kullanici kullanan, KomisyonUyeSablon sablon);
        Sonuc BordroKontrolEt(Kullanici kullanan, int yil, int ay, int muhasebe, string birim, int bordroTur, int bordroIslem);

        Sonuc GundelikKaydet(Kullanici kullanan, Gundelik bilgi);
        ObjectArray GundelikListele(Kullanici kullanan, Gundelik kriter);
        Sonuc GundelikSil(Kullanici kullanan, Gundelik bilgi);
        Sonuc GundelikSonrakiYilaKopyala(Kullanici kullanan, Gundelik bilgi);
        ObjectVAN GeciciGorevYollukManasOku(Kullanici kullanan, GeciciGorevYollukManas form);
        Sonuc GeciciGorevYollukManasOEIliskilendir(Kullanici kullanan, GeciciGorevYollukManas form, bool avans);
        Sonuc GeciciGorevYollukManasKaydet(Kullanici kullanan, GeciciGorevYollukManas form);
        GeciciGorevYollukManas GeciciGorevYollukManasHesapla(Kullanici kullanan, GeciciGorevYollukManas form);

        Sonuc BordroRaporSablonKaydet(Kullanici kullanan, BordroRaporSablon form);
        ObjectArray BordroRaporSablonListele(Kullanici kullanan, BordroRaporSablon sablon);

        ObjectArray PersonelMaliyetHesapla(Kullanici kullanan, string tcKimlikNo, TNSDateTime basTarih, TNSDateTime bitTarih, int ekParam);
        Sonuc OncekiAsamadanAl(Kullanici kullanan, int hedefYil, int hedefAsama);
        Sonuc DetayHesapla(Kullanici kullanan, int yil, int asama);

        ObjectArray BordroKisiBorcuBilgiBul(Kullanici kullanan, int personelTip);
        Sonuc BordroSablonSil(Kullanici kullanan, BordroSablonDosya bsd);

        /// <summary>
        /// DBIkraz listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBIkraz yapısında DBIkraz:  bilgilerini tutan değişken</param>
        ObjectArray ListeleDBIkrazlar(Kullanici kullanan, DBIkraz form, bool detayOku);

        /// <summary>
        /// DBIkraz nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBIkraz yapısında DBIkraz:  bilgilerini tutan değişken</param>
        ObjectVAN OkuDBIkraz(Kullanici kullanan, DBIkraz form);

        /// <summary>
        /// DBIkraz nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBIkraz yapısında DBIkraz:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetDBIkraz(Kullanici kullanan, DBIkraz form);

        /// <summary>
        /// DBBilesen listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBBilesen yapısında DBBilesen:  bilgilerini tutan değişken</param>
        ObjectArray ListeleDBBilesenler(Kullanici kullanan, DBBilesen form);

        /// <summary>
        /// DBBilesen nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBBilesen yapısında DBBilesen:  bilgilerini tutan değişken</param>
        ObjectVAN OkuDBBilesen(Kullanici kullanan, DBBilesen form);

        /// <summary>
        /// DBBilesen nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBBilesen yapısında DBBilesen:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetDBBilesen(Kullanici kullanan, DBBilesen form);

        /// <summary>
        /// DBAltBilesen listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBAltBilesen yapısında DBAltBilesen:  bilgilerini tutan değişken</param>
        ObjectArray ListeleDBAltBilesenler(Kullanici kullanan, DBAltBilesen form);

        /// <summary>
        /// DBAltBilesen nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBAltBilesen yapısında DBAltBilesen:  bilgilerini tutan değişken</param>
        ObjectVAN OkuDBAltBilesen(Kullanici kullanan, DBAltBilesen form);

        /// <summary>
        /// DBAltBilesen nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBAltBilesen yapısında DBAltBilesen:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetDBAltBilesen(Kullanici kullanan, DBAltBilesen form);

        /// <summary>
        /// DBButceRef nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="butceRef">DBButceRef yapısında DBButceRef:  bilgilerini tutan değişken</param>
        ObjectArray ListeleDBButceRef(Kullanici kullanan, DBButceRef butceRef);

        /// <summary>
        /// DBButceRef nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="butceRefID">butceRefID:  lotları okunacak bütçe ref</param>
        ObjectArray ListeleDBButceRefLot(Kullanici kullanan, int butceRefID);
        /// <summary>
        /// DBButceRef nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="butceRef">DBButceRef yapısında DBButceRef:  bilgilerini tutan değişken</param>
        ObjectVAN OkuDBButceRef(Kullanici kullanan, DBButceRef butceRef);

        /// <summary>
        /// DBButceRef nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="butceRef">DBButceRef yapısında DBButceRef:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetDBButceRef(Kullanici kullanan, DBButceRef butceRef);

        ObjectArray ListeleDBSatinAlma(Kullanici kullanan, DBSatinAlma satinAlma, int siralama = 1);
        ObjectVAN OkuDBSatinAlma(Kullanici kullanan, DBSatinAlma satinAlma);
        Sonuc KaydetDBSatinAlma(Kullanici kullanan, DBSatinAlma satinAlma);
        ObjectArray ListeleDBDosyaBilgileri(Kullanici kullanan, int kod, string modulKod);
        Sonuc KaydetDBHakedis(Kullanici kullanan, DBHakedis form);
        Sonuc SilDBHakedis(Kullanici kullanan, int hakedisID); 
        Sonuc SilDBButceRef(Kullanici kullanan, int butceRefID);
        Sonuc SilDBSatinAlmaPlani(Kullanici kullanan, int satinAlmaID);
        ObjectVAN OkuDBHakedis(Kullanici kullanan, int hakedisID);
        ObjectArray ListeleDBHakedis(Kullanici kullanan, DBHakedis dBHakedis, bool detayOku, int siralaYontem);
        Sonuc KaydetDBDosyaBilgisi(Kullanici kullanan, DBYuklenebilecekDosyalar dosyaBilgi);
        Sonuc DBHakedisDurumDegistir(Kullanici kullanan, int hakedisID, int yeniDurumKod, string gerekce);
        ObjectArray ListeleMerkezNumaratorler(Kullanici kullanan, DBMerkezBankasiNumarator dBMerkezBankasiNumarator);
        ObjectVAN OkuMerkezNumarator(Kullanici kullanan, int merkezNumaratorID);
        ObjectArray MerkezeGonderilmeyenHakSahipleri(Kullanici kullanan, int altBilesenID);
        Sonuc YeniMerkezNumarasiAc(Kullanici kullanan, int altBilesenID);
        Sonuc MerkezNumaratoreEkle(Kullanici kullanan, int hakedisID, int numarator, int merkezNumaratorID);
        Sonuc MerkezNumaratordenCikart(Kullanici kullanan, int hakedisID, int numarator, int merkezNumaratorID);
        ObjectArray ListeleDBSozlesmeler(Kullanici kullanan, DBSozlesme sozlesme, int siralama = 1);
        ObjectArray ListeleSozlesmesizOdemeler(Kullanici kullanan, DBSozlesme sozlesme, int siralama = 1);
        ObjectArray ListeleDBEkstreler(Kullanici kullanan, TNSDateTime baslangicTarih, TNSDateTime bitisTarih, int IkrazID);
        Sonuc DBEkstrelerVeriEkle(Kullanici kullanan, DBEkstre ekstre);
        Sonuc KaydetDBSozlesme(Kullanici kullanan, DBSozlesme sozlesme);

        /// <summary>
        /// DBImza listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBImza yapısında DBImza:  bilgilerini tutan değişken</param>
        ObjectArray ListeleDBImzalar(Kullanici kullanan, DBImza form);

        /// <summary>
        /// DBImza nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DBImza yapısında DBImza:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetDBImzalar(Kullanici kullanan, DBImza form);
        Sonuc MerkezNumaratorImzaKaydet(Kullanici kullanan, DBMerkezBankasiNumarator mn);
        Sonuc MerkezNumarasiDurumDegistir(Kullanici kullanan, DBMerkezBankasiNumarator mn);
        ObjectVAN GetOdemePlani(Kullanici kullanan, int sozlesmeID, int odemePlanID);
        ObjectArray ListeleOdemePlani(Kullanici kullanan, int sozlesmeID);
        Sonuc KaydetDBOdemePlani(Kullanici kullanan, DBOdemePlani odemePlani);
        Sonuc DurumDegistirDBOdemePlani(Kullanici kullanan, DBOdemePlani odemePlani);
        ObjectVAN DBSozlesmeNoAl(Kullanici kullanan, int sozlesmeID);
        ObjectVAN DBSozlesmeKapat(Kullanici kullanan, int sozlesmeID, int durum, string gerekce);
        Sonuc DBSilSozlesme(Kullanici kullanan, int sozlesmeID); 
        Sonuc KaydetDBSatinAlmaGerceklesmePlani(Kullanici kullanan, DBSatinAlmaGerceklesmePlan gercPlan);
        Sonuc KaydetDBSGBNumarator(Kullanici kullanan, DBEkstre[] liste);
        Sonuc DurumDegistirDBSatinAlmaGerceklesmePlani(Kullanici kullanan, int satinAlmaID, int dagilimID, int durumKod);
    }
}
