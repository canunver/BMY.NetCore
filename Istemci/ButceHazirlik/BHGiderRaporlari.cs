using System;
using System.Collections;
using System.Collections.Generic;
using OrtakClass;
using TNS;
using TNS.BHF;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;
using TNS.UZY;

namespace ButceHazirlikv2
{
    /// <summary>Kullanıcıya gösterilebilecek rapor hatası (eski kodun attığı iletiler ve sunucunun döndürdüğü hata metni).</summary>
    public class RaporHatasi : Exception
    {
        public RaporHatasi(string ileti) : base(ileti) { }
    }

    /// <summary>
    /// Eski BHMv2Ortak.BHGiderRaporlariEkle "Rapor Al" menüsündeki raporlar (excelDahil = false). Eski sayfa bunları
    /// BHMv2Ortak.RaporAl -> RaporlamaServis.TekRaporAl ile Rapor.xml'deki tanıma göre alıyordu; burada tanımlar sabittir ve
    /// rapor kodları eski kodun aynısıdır (ButceHazirlikv2Raporlari.cs, ButceFormRaporlari.cs, ProgramButceRaporlari.cs).
    /// </summary>
    public static class BHGiderRaporlari
    {
        private class RaporTanim
        {
            public string kod;
            public string sablon;
            public string sinif;
            public string ekBilgi;

            public RaporTanim(string kod, string sablon, string sinif, string ekBilgi)
            {
                this.kod = kod;
                this.sablon = sablon;
                this.sinif = sinif;
                this.ekBilgi = ekBilgi;
            }
        }

        // Rapor.xml'deki tanımlar (grup-no, SABLON, NESNE, EKBILGI); sinif "EXCEL": RAPORTIP="EXCEL" (OrtakClass.ExcelRapor ile doldurulur).
        private static readonly RaporTanim[] Tanimlar =
        {
            new RaporTanim("0-1", "BHM/Form02.xlt", "EXCEL", ""),
            new RaporTanim("0-2", "BHM/Form03.xlt", "EXCEL", ""),
            new RaporTanim("0-3", "BHM/Form04.xlt", "EXCEL", ""),
            new RaporTanim("0-4", "BHM/Form05.xlt", "EXCEL", ""),
            new RaporTanim("0-11", "BHM/Form16.xlt", "EXCEL", ""),
            new RaporTanim("0-12", "BHM/Form17.xlt", "BH", "F17"),
            new RaporTanim("0-13", "BHM/Form18.xlt", "BH", "F18"),
            new RaporTanim("0-26", "BHM/Form04_2.xltx", "BH", "F4_2"),
            new RaporTanim("0-5", "BHM/Form08.xlt", "BH", "F8"),
            new RaporTanim("0-6", "BHM/Form09.xlt", "BH", "F9"),
            new RaporTanim("0-25", "BHF/TavanUstuOdenek.xlt", "BF", "F25d"),
            new RaporTanim("997-1", "BHMv2/EkonomikDetay.xlt", "BH", "EkonomikDetay"),
            new RaporTanim("997-2", "BHMv2/FaaliyetEkonomikCapraz.xlt", "BH", "FaaliyetEkonomikCapraz"),
            new RaporTanim("997-3", "BHMv2/GiderButceBirlestirme.xlt", "BH", "GiderButceBirlestirme"),
            new RaporTanim("997-4", "BHMv2/EkoPebUcgen.xlt", "BH", "GiderUcgenEko"),
            new RaporTanim("997-5", "BHMv2/EkoPebUcgen.xlt", "BH", "GelirUcgenEko"),
            new RaporTanim("997-7", "BHMv2/PBListe.xltx", "PB", "Liste"),
        };

        // Eski BHMv2Ortak.BHGiderRaporlariEkle (excelDahil = false / true) ve BHGelirRaporlariEkle menü sıraları
        private static readonly string[] GiderKodlari = { "0-26", "0-5", "0-6", "0-25", "997-1", "997-2", "997-3", "997-4", "997-5", "997-7" };
        private static readonly string[] GiderExcelKodlari = { "0-1", "0-2", "0-3", "0-26", "0-4", "0-5", "0-6", "0-11", "0-25", "997-1", "997-2", "997-3", "997-4", "997-5", "997-7" };
        private static readonly string[] GelirKodlari = { "0-12", "997-5" };

        /// <summary>Menüde gösterilecek raporlar: kod ("grup-no") ve ad (eski BHMv2Ortak.RaporAdBul: OrtakClassRes REP_grup_no).</summary>
        public static List<object> Liste()
        {
            return Liste(GiderKodlari);
        }

        /// <summary>Gider raporları (eski BHGiderRaporlariEkle(menu, excelDahil)).</summary>
        public static List<object> ListeGider(bool excelDahil)
        {
            return Liste(excelDahil ? GiderExcelKodlari : GiderKodlari);
        }

        /// <summary>Gelir raporları (eski BHGelirRaporlariEkle).</summary>
        public static List<object> ListeGelir()
        {
            return Liste(GelirKodlari);
        }

        private static List<object> Liste(string[] kodlar)
        {
            List<object> liste = new List<object>();
            foreach (string kod in kodlar)
                liste.Add(new { kod = kod, ad = RaporAd(kod) });
            return liste;
        }

        /// <summary>Kod, gider (excelDahil) ya da gelir menüsünde var mı?</summary>
        public static bool Tanimli(string kod, bool gelir, bool excelDahil)
        {
            return Array.IndexOf(gelir ? GelirKodlari : excelDahil ? GiderExcelKodlari : GiderKodlari, kod) >= 0;
        }

        public static string RaporAd(string kod)
        {
            return Istemci.Metin.Ortak("REP_" + (kod ?? "").Replace("-", "_"), kod);
        }

        public static bool Tanimli(string kod)
        {
            return Tanimli(kod, false, false);
        }

        private static RaporTanim Bul(string kod)
        {
            foreach (RaporTanim t in Tanimlar)
                if (t.kod == kod) return t;
            return null;
        }

        /// <summary>
        /// Eski BHMv2Ortak.RaporAl: rapor parametrelerini hazırlar, raporu üretir ve üretilen dosyanın yolunu döndürür.
        /// </summary>
        public static string RaporAl(Kullanici kullanan, string raporKod, int yil, string senaryo, string kur, string fon, string fin, string eko, string peb, string raporSablonYol)
        {
            RaporTanim t = Bul(raporKod);
            if (t == null) throw new RaporHatasi("Rapor seçimi hatalı!");

            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            Hashtable raporParam = new Hashtable();
            raporParam.Add("ButceYil", yil.ToString());
            raporParam.Add("Senaryo", senaryo);
            raporParam.Add("senaryoKod", senaryo);
            if (string.IsNullOrEmpty(kur))
            {
                kur = TNS.DEG.Arac.VarsayilanKurumBul();
                kur = TNS.BKD.Arac.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            }

            raporParam.Add("KurKod", kur);
            raporParam.Add("FonKod", fon);
            raporParam.Add("FinKod", fin);
            raporParam.Add("EkoKod", eko);
            raporParam.Add("PebKod", peb);

            raporParam.Add("AyNo", "6");
            raporParam.Add("AyAd", GenelIslemler.AyAdDondur(6));
            raporParam.Add("TahminSenaryo", "00.00");

            IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
            raporParam.Add("SenaryoAd", servis.ButceSenaryoAdBul(kullanan.aktifKultur, senaryo));
            raporParam.Add("KurAd", TNS.UZY.Arac.Tanimla().UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kur, true));

            string sablonAd = raporSablonYol + "/" + t.sablon;
            string sonucDosyaAd = DosyaIslem.DosyaAdUret();

            if (t.sinif == "EXCEL")
                return OrtakClass.ExcelRapor.ExcelRaporHesapla(kullanan, null, sablonAd, sonucDosyaAd, null, raporParam);
            else if (t.sinif == "BH")
                return new ButceHazirlikRaporlari().RaporAl(kullanan, servis, sablonAd, sonucDosyaAd, "", raporParam, t.ekBilgi);
            else if (t.sinif == "BF")
                return new ButceFormRaporlari().RaporAl(kullanan, TNS.BHF.Arac.Tanimla(), sablonAd, sonucDosyaAd, "", raporParam, t.ekBilgi);
            else
                return new ProgramButceRaporlari().RaporAl(kullanan, servis, sablonAd, sonucDosyaAd, "", raporParam, t.ekBilgi);
        }

        /// <summary>Eski GenelIslemlerIstemci.VarsayilanKurumBulAd.</summary>
        internal static string KurumAd()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMAD");
        }

        /// <summary>Eski GenelIslemlerIstemci.HarcamaTeklifAyiBul / HarcamaTasariAyiBul (web.config HarcamaTeklifAyi, HarcamaTasariAyi).</summary>
        internal static void HarcamaAyiBul(string ayarAd, out int harcamaAy, out string harcamaAyAd, int hataAy)
        {
            harcamaAy = OrtakFonksiyonlar.ConvertToInt(Istemci.Ayar.Oku(ayarAd, hataAy.ToString()), hataAy);
            harcamaAyAd = GenelIslemler.AyAdDondur(harcamaAy);
        }
    }

    /// <summary>Eski ButceHazirlikv2Raporlari.cs ButceHazirlikRaporlari sınıfından Rapor Al menüsünün kullandığı raporlar.</summary>
    internal class ButceHazirlikRaporlari
    {
        int eko2uz = 3;
        int kurKirSay = 3;
        Tertip tUz;
        int yil;

        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            eko2uz = tUz.KodUzunlukBul(3, 2, true);
            kurKirSay = tUz.KodNoKirilimSay(0);

            if (ekBilgi.Equals("F8"))
            {
                raporParam.Add("Kirilim", "4");
                return OdenekCetveli(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            }
            else if (ekBilgi.Equals("F9"))
            {
                raporParam.Add("Kirilim", "2");
                return OdenekCetveli(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            }
            else if (ekBilgi.Equals("EkonomikDetay"))
                return EkonomikDetay(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);
            else if (ekBilgi.Equals("FaaliyetEkonomikCapraz"))
                return FaaliyetEkonomikCapraz(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);
            else if (ekBilgi.Equals("GiderButceBirlestirme"))
                return GiderButceBirlestirme(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);
            else if (ekBilgi.Equals("GiderUcgenEko") || ekBilgi.Equals("GelirUcgenEko"))
                return EkoUcgen(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("F4_2"))
                return Form4_2(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("F17"))
                return Form17(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi.Equals("F18"))
                return Form18(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else
                throw new RaporHatasi("Tanımsız Rapor Parametresi");
        }

        /// <summary>Form 18 finansman cetveli (eski Form18; Finansman Teklifi sayfasındaki Yazdır). Eski GormeYetkisiVarMi hep true idi; denetim kaldırıldı.</summary>
        public string Form18(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int fsatir = 0;
            int fsutun = 0;
            Kullanici kullanan = (Kullanici)o_kullanan;
            IBHMv2Servis servis = o_servis as IBHMv2Servis ?? TNS.BHMv2.Arac.Tanimla();

            string asama = (string)raporParam["Senaryo"];
            //string asamaAd = (string)raporParam["AsamaAd"];
            string kurum = TNS.DEG.Arac.VarsayilanKurumBul();
            string kurumAd = BHGiderRaporlari.KurumAd();

            sonucDosyaAd = sonucDosyaAd + "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABFinansman, kurum, "");
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            EkDegerParam[] ekBilgiler = new EkDegerParam[1];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABFinansman, 0, 0, asama);
            kodBilgileri.Add(new UcgenKodBilgi(6, 4));

            //ekBilgiler.Add((int)ENUMTabloKod.TABFinansmanGerceklesmesi * 1000 + 2 * 30 + 12);
            //ekBilgiler.Add((int)ENUMTabloKod.TABFinansmanGerceklesmesi * 1000 + 1 * 30 + 6);
            //ekBilgiler.Add((int)ENUMTabloKod.TABFinansmanTahmini*1000+0*30+asamaTahmin);
            //ekBilgiler.Add((int)ENUMTabloKod.TABFinansman * 1000 + asama + 10);
            //ekBilgiler.Add((int)ENUMTabloKod.TABFinansman * 1000 + asama + 20);

            UcgenSatir[] cevap = servis.UcgenTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, true);

            XLS.HucreAdBulYaz("ButceYil", yil.ToString());

            if (kurum != "")
                XLS.HucreAdBulYaz("KurumAd", kurumAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            int basSatir = satir;
            XLS.HucreAdAdresCoz("FormatSatir", ref fsatir, ref fsutun);
            int yukseklik = XLS.SatirYukseklikAl(satir);
            for (int i = 0; i < cevap.Length; i++)
            {
                UcgenSatir us = cevap[i];
                XLS.HucreKopyala(fsatir, fsutun, fsatir, fsutun + 13, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + us.yer, us.kodParca);
                XLS.HucreDegerYaz(satir, sutun + 4, us.aciklama);
                XLS.HucreDegerYaz(satir, sutun + 7, (double)us.ekDegerler[0]);
                satir++;
            }
            XLS.SatirYukseklikAyarla(basSatir, satir, yukseklik);
            //j = 0;
            //foreach (double d in us.ekDegerler)
            //{
            //}
            //if (us.yer != 3)
            //    for (int sut = 0; sut < 13; sut++)
            //        XLS.KoyuYap(satir, sut, true);

            //if (us.yer == 0 && us.kodParca == "5")
            //    netFinansmanYok = false;


            //if (netFinansmanYok)
            //{
            //    for (int sut = 0; sut < 13; sut++)
            //    {
            //        XLS.HucreDegerYaz(satir, sut, "");
            //        XLS.KoyuYap(satir, sut, true);
            //    }
            //    XLS.HucreKopyala(satir, sutun, satir, sutun + 13, satir + 1, sutun);
            //    XLS.SatirYukseklikAyarla(satir + 1, satir + 1, XLS.SatirYukseklikAl(satir));
            //    XLS.HucreDegerYaz(satir, sutun, "5");
            //    XLS.HucreDegerYaz(satir, sutun + 4, "NET FİNANSMAN");

            //    for (int sut = 5; sut < 10; sut++)
            //        XLS.HucreDegerYaz(satir, sutun + sut, "0");

            //    satir++;
            //}

            //for (int sut = 0; sut < 13; sut++)
            //{
            //    XLS.HucreDegerYaz(satir, sut, "");
            //    XLS.KoyuYap(satir, sut, true);
            //}
            //XLS.HucreBirlestir(satir, sutun, satir, sutun + 4);
            //XLS.HucreDegerYaz(satir, sutun, "TOPLAM");

            //j = 0;

            //foreach (double d in toplamlar)
            //{
            //    XLS.HucreDegerYaz(satir, sutun + 5 + j++, d);
            //}

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        /// <summary>Form 17 gelir cetveli (eski Form17). Eski GormeYetkisiVarMi hep true idi; denetim kaldırıldı.</summary>
        public string Form17(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            Kullanici kullanan = (Kullanici)o_kullanan;
            IBHMv2Servis servis = o_servis as IBHMv2Servis ?? TNS.BHMv2.Arac.Tanimla();
            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

            string senaryoKod = (string)raporParam["Senaryo"];
            string senaryoAd = uzyServis.UzayDegeriStr(kullanan, "ABSSENARYOAD", TNS.UZY.Arac.UzayKosulOl(senaryoKod), true);
            string kurum = TNS.DEG.Arac.VarsayilanKurumBul();
            kurum = TNS.BKD.Arac.KurumsaldanKuruma(kurum, tUz.Kirilimlar(0));

            string kurumAd = BHGiderRaporlari.KurumAd();
            string kurKod = (string)raporParam["KurKod"] + "";
            string kurAd = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), kurKod), true);

            string eko = (string)raporParam["eko"] + "";

            int harcamaAy;
            string harcamaAyAd = "";

            if (senaryoKod.CompareTo("03") == 1)
                BHGiderRaporlari.HarcamaAyiBul("HarcamaTasariAyi", out harcamaAy, out harcamaAyAd, 8);
            else
                BHGiderRaporlari.HarcamaAyiBul("HarcamaTeklifAyi", out harcamaAy, out harcamaAyAd, 6);

            sonucDosyaAd = sonucDosyaAd + "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("AsamaAd", senaryoAd);
            XLS.HucreAdBulYaz("ButceYil", yil.ToString());

            if (!string.IsNullOrEmpty(kurAd))
                XLS.HucreAdBulYaz("KurumAd", kurAd);
            else
                XLS.HucreAdBulYaz("KurumAd", kurumAd);

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBGelir, kurum, eko);
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(5, 4));

            EkDegerParam[] ekBilgiler = new EkDegerParam[6];

            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABGelirGerceklesmesi, 2, 0, "12");
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPEBGelir, 1, 0, "06.00");
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABGelirGerceklesmesi, 1, 0, harcamaAy.ToString());
            ekBilgiler[3] = new EkDegerParam(ENUMTabloKod.TABPEBGelir, 0, 0, senaryoKod);
            ekBilgiler[4] = new EkDegerParam(ENUMTabloKod.TABPEBGelir, 0, 1, senaryoKod);
            ekBilgiler[5] = new EkDegerParam(ENUMTabloKod.TABPEBGelir, 0, 2, senaryoKod);

            double[] toplamlar = new double[6];

            UcgenSatir[] cevap = servis.UcgenTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, true);

            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

            int j = 0;
            for (int i = 0; i < cevap.Length; i++)
            {
                UcgenSatir us = cevap[i];

                XLS.HucreKopyala(satir, sutun, satir, sutun + 13, satir + 1, sutun);

                XLS.HucreDegerYaz(satir, sutun + us.yer, us.kodParca);
                XLS.HucreDegerYaz(satir, sutun + 4, us.aciklama);
                j = 0;
                foreach (double d in us.ekDegerler)
                {
                    if (us.yer == 0)
                        toplamlar[j] = d + toplamlar[j];

                    XLS.HucreDegerYaz(satir, sutun + 5 + j++, d);
                }
                satir++;
            }
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 4);
            XLS.HucreDegerYaz(satir, sutun, "TOPLAM");

            j = 0;

            foreach (double d in toplamlar)
                XLS.HucreDegerYaz(satir, sutun + 5 + j++, d);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public string OdenekCetveli(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            return OdenekCetveli(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi, false);
        }

        /// <summary>
        /// Ödenek cetveli hazırlayan yordam. Eski yordam ürettiği raporu FTP'de saklıyordu ama VeriDegismisMi hep true
        /// döndürdüğünden saklanan dosya hiç kullanılmıyordu; saklama kaldırıldı. Eski GormeYetkisiVarMi de hep true idi.
        /// </summary>
        public string OdenekCetveli(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi, bool PEBMi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            Kullanici kullanan = (Kullanici)o_kullanan;

            IBHMv2Servis servis = o_servis as IBHMv2Servis ?? TNS.BHMv2.Arac.Tanimla();
            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

            string senaryoKod = (string)raporParam["Senaryo"];
            string kurum = TNS.DEG.Arac.VarsayilanKurumBul();
            string kurKod = (string)raporParam["KurKod"] + "";
            if (string.IsNullOrWhiteSpace(kurKod)) kurKod = kurum;

            string fon = (string)raporParam["FonKod"] + "";
            string fin = (string)raporParam["FinKod"] + "";
            string eko = (string)raporParam["EkoKod"] + "";
            string peb = (string)raporParam["PebKod"] + "";
            int kirilim = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Kirilim"], 0);
            if (kirilim == 0)
                kirilim = 2;

            int ekleKolon;
            if (kurKirSay == 2)
                ekleKolon = 2;
            else
                ekleKolon = 0;

            int sutunSay;

            if (kirilim == 4)
                sutunSay = 21;
            else
                sutunSay = 19;

            int harcamaAy;
            string harcamaAyAd = "";

            if (senaryoKod.CompareTo("03") == 1)
                BHGiderRaporlari.HarcamaAyiBul("HarcamaTasariAyi", out harcamaAy, out harcamaAyAd, 8);
            else
                BHGiderRaporlari.HarcamaAyiBul("HarcamaTeklifAyi", out harcamaAy, out harcamaAyAd, 6);

            sonucDosyaAd += "." + GenelIslemler.ExcelTur();

            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            harcamaAyAd = harcamaAyAd.ToUpper();
            XLS.HucreAdBulYaz("HarcamaAyAd", harcamaAyAd);

            int fonAlanKod = 2;
            if ((PEBMi) || ((string)raporParam["PEBFon"] + "" == "1"))
            {
                fonAlanKod = 14;
            }

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kurKod, fon, fin, eko, peb);

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(1, kurKirSay));
            kodBilgileri.Add(new UcgenKodBilgi(fonAlanKod, 4));
            kodBilgileri.Add(new UcgenKodBilgi(3, 1));
            kodBilgileri.Add(new UcgenKodBilgi(4, kirilim));

            EkDegerParam[] ekBilgiler = new EkDegerParam[7];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 2, 0, "06.00");
            if (string.IsNullOrWhiteSpace(peb))
                ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABHarcama, 2, 0, "12;830");
            else
                ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPEBHarcama, 2, 0, "12");

            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 1, 0, "06.00");

            if (string.IsNullOrWhiteSpace(peb))
                ekBilgiler[3] = new EkDegerParam(ENUMTabloKod.TABHarcama, 1, 0, harcamaAy.ToString() + ";830");
            else
                ekBilgiler[3] = new EkDegerParam(ENUMTabloKod.TABPEBHarcama, 1, 0, harcamaAy.ToString());
            ekBilgiler[4] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod);
            ekBilgiler[5] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 1, senaryoKod);
            ekBilgiler[6] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 2, senaryoKod);

            UcgenSatir[] cevap = servis.UcgenTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, true);
            if (kurum != "")
                XLS.HucreAdBulYaz("KurumAd", kurum + " - " + uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurum, true));

            XLS.HucreAdBulYaz("ButceYil", yil);
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            int[] koyuAcikYer = { satir - 2, satir - 1 };
            int ilkSatir = satir;
            string[] kodlar = new string[13 - ekleKolon];

            for (int k = 0; k < cevap.Length; k++)
            {
                UcgenSatir us = cevap[k];
                kodlar[us.yer] = us.kodParca;

                int koyuAcik;
                if (us.yer == 0 || us.yer == 1 || us.yer == 4 - ekleKolon || us.yer == 9 - ekleKolon)
                    koyuAcik = 0;
                else
                    koyuAcik = 1;

                if (koyuAcikYer[koyuAcik] != satir)
                    XLS.HucreKopyala(koyuAcikYer[koyuAcik], sutun, koyuAcikYer[koyuAcik], sutun + sutunSay, satir, sutun);

                for (int i = sutun; i < sutun + sutunSay; i++)
                    XLS.HucreDegerYaz(satir, i, "");

                if (us.yer == 1 && ekleKolon == 0)
                {
                    if (Convert.ToInt16(kodlar[0]) >= 30 || Convert.ToInt16(us.kodParca) >= 75)
                        satir--;
                }

                bool yazma = false;
                if ((us.yer == 3 && kurKirSay == 4) || us.yer == 7 - ekleKolon)
                {
                    if (us.kodParca == "00")
                    {
                        UcgenSatir usEski = (UcgenSatir)cevap[k - 1];

                        bool farkli = false;
                        for (int j = 0; j < us.ekDegerler.Count; j++)
                            if ((double)usEski.ekDegerler[j] != (double)us.ekDegerler[j])
                                farkli = true;

                        if (!farkli)
                            satir--;
                    }
                }
                else if (us.kodParca == "00")
                {
                    yazma = true;
                    satir--;
                }

                if (!yazma)
                {
                    int kolonEkle;
                    if (us.yer >= kurKirSay)
                        kolonEkle = 4 - kurKirSay;
                    else
                        kolonEkle = 0;
                    XLS.HucreDegerYaz(satir, us.yer + kolonEkle, us.kodParca);
                    XLS.HucreDegerYaz(satir, 9 + kirilim, us.aciklama);

                    for (int j = 0; j < us.ekDegerler.Count; j++)
                        XLS.HucreDegerYaz(satir, 10 + kirilim + j, (double)us.ekDegerler[j]);

                    if ((satir - ilkSatir + 1) % 46 == 1 && (satir - ilkSatir + 1) > 10)
                    {
                        XLS.YatayCizgiCiz(satir - 1, sutun, sutun + sutunSay - 1, OrtakClass.LineStyle.DOUBLE, TabloRenk.BLACK, false);
                        XLS.SayfaSonuKoy(satir);
                    }
                }
                satir++;
            }
            for (int i = sutun; i < sutun + sutunSay; i++)
            {
                XLS.HucreDegerYaz(satir, i, "");
            }
            XLS.YatayCizgiCiz(satir, sutun, sutun + sutunSay - 1, OrtakClass.LineStyle.DOUBLE, TabloRenk.BLACK);

            if (kurKirSay < 4)
            {
                int genislikTopla = 0;
                for (int i = kurKirSay; i < 4; i++)
                {
                    genislikTopla += XLS.SutunGenislikAl(i);
                    XLS.SutunGenislikAyarla(i, i, 0);
                }
                genislikTopla = genislikTopla / (4 - kurKirSay);
                for (int i = 0; i < kurKirSay; i++)
                {
                    int mevcutGenislik = XLS.SutunGenislikAl(i);
                    XLS.SutunGenislikAyarlaPixel(i, i, mevcutGenislik + genislikTopla);
                }
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public string EkonomikDetay(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam)
        {
            IBHMv2Servis servisBHMv2 = o_servis as IBHMv2Servis ?? TNS.BHMv2.Arac.Tanimla();
            Kullanici kullanan = (Kullanici)o_kullanan;

            string senaryo = (string)raporParam["Senaryo"];
            string senaryoAd = (string)raporParam["SenaryoAd"];
            string kur = (string)raporParam["KurKod"] + "";
            string fon = (string)raporParam["FonKod"] + "";
            string fin = (string)raporParam["FinKod"] + "";
            string eko = (string)raporParam["EkoKod"] + "";
            string peb = (string)raporParam["PebKod"] + "";

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kur, fon, fin, eko, peb);
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(4, 4));

            EkDegerParam[] ekBilgiler = new EkDegerParam[3];

            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryo);
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 1, senaryo);
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 2, senaryo);

            OdenekBilgi[] odnler = servisBHMv2.ListeTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, false);

            int baslaSatir = 0;
            int baslaSutun = 0;

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);

            int yukseklik = XLS.SatirYukseklikAl(baslaSatir);

            int satir = baslaSatir;

            string kod, ad;

            foreach (OdenekBilgi oBilgi in odnler)
            {
                kod = oBilgi.tertip.kodlar[3];
                ad = oBilgi.tertip.adlar[3];

                satir++;
                XLS.HucreKopyala(baslaSatir, baslaSutun, baslaSatir, baslaSutun + 3, satir, baslaSutun);
                XLS.HucreDegerYaz(satir, baslaSutun, kod + " - " + ad);
                int index = 0;
                foreach (double item in oBilgi.ekRakamlar)
                {
                    index++;
                    XLS.HucreDegerYaz(satir, baslaSutun + index, item);
                }
                XLS.SatirYukseklikAyarla(satir, satir, yukseklik);
            }

            XLS.SatirGizle(baslaSatir, baslaSatir, true);

            kur = string.IsNullOrEmpty(kur) ? "-" : kur;
            fon = string.IsNullOrEmpty(fon) ? "-" : fon;
            fin = string.IsNullOrEmpty(fin) ? "-" : fin;
            eko = string.IsNullOrEmpty(eko) ? "-" : eko;
            peb = string.IsNullOrEmpty(peb) ? "-" : peb;
            string kistasBilgi = string.Format("Kur:{0}, Fon:{1}, Fin:{2}, Eko:{3}, Peb:{4}", kur, fon, fin, eko, peb);
            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("Senaryo", senaryoAd);
            XLS.HucreAdBulYaz("Kistas", kistasBilgi);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public string FaaliyetEkonomikCapraz(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam)
        {
            IBHMv2Servis servisBHMv2 = o_servis as IBHMv2Servis ?? TNS.BHMv2.Arac.Tanimla();
            Kullanici kullanan = (Kullanici)o_kullanan;

            string senaryo = (string)raporParam["Senaryo"];
            string senaryoAd = (string)raporParam["SenaryoAd"];
            string kur = (string)raporParam["KurKod"] + "";
            string fon = (string)raporParam["FonKod"] + "";
            string fin = (string)raporParam["FinKod"] + "";
            string eko = (string)raporParam["EkoKod"] + "";
            string peb = ""; //(string)raporParam["PebKod"] + "";

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kur, fon, fin, eko, peb);
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(4), kistas.KodNoKirilimSay(4)));
            kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(3), 1));

            EkDegerParam[] ekBilgiler = new EkDegerParam[1];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryo);

            OdenekBilgi[] odnler = servisBHMv2.ListeTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, false);

            int baslaSatir = 0, toplamSatir = 0;
            int baslaSutun = 0, toplamSutun = 0;

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
            XLS.HucreAdAdresCoz("ToplamSatir", ref toplamSatir, ref toplamSutun);

            int yukseklik = XLS.SatirYukseklikAl(baslaSatir);
            int satir = toplamSatir;

            string kod4 = "", ad4 = "";
            int kod3 = 0;
            double deger = 0.0;
            Hashtable toplamDegerler = new Hashtable();

            foreach (OdenekBilgi oBilgi in odnler)
            {
                deger = (double)oBilgi.ekRakamlar[0];
                kod3 = OrtakFonksiyonlar.ConvertToInt(oBilgi.tertip.kodlar[3], 0);

                if (kod4 != oBilgi.tertip.kodlar[4])
                {
                    kod4 = oBilgi.tertip.kodlar[4];
                    ad4 = oBilgi.tertip.adlar[4];

                    satir++;
                    XLS.HucreKopyala(baslaSatir, baslaSutun, baslaSatir, baslaSutun + 11, satir, baslaSutun);
                    XLS.SatirYukseklikAyarla(satir, satir, yukseklik);
                    XLS.HucreDegerYaz(satir, baslaSutun, ad4);
                    if (kod3 > 0)
                        XLS.HucreDegerYaz(satir, baslaSutun + kod3, deger);
                }
                else
                {
                    if (kod3 > 0)
                        XLS.HucreDegerYaz(satir, baslaSutun + kod3, deger);
                }

                if (kod3 > 0)
                {
                    if (toplamDegerler.ContainsKey(kod3))
                        toplamDegerler[kod3] = OrtakFonksiyonlar.ConvertToDouble(toplamDegerler[kod3].ToString(), 0) + deger;
                    else
                        toplamDegerler.Add(kod3, deger);
                }
            }

            //Toplam İşlem
            satir++;
            XLS.HucreKopyala(toplamSatir, toplamSutun, toplamSatir, toplamSutun + 11, satir, toplamSutun);
            XLS.SatirYukseklikAyarla(satir, satir, yukseklik);

            int topIndex = 0;
            double topDeger = 0.0;
            foreach (DictionaryEntry item in toplamDegerler)
            {
                topIndex = OrtakFonksiyonlar.ConvertToInt(item.Key, 0);
                topDeger = OrtakFonksiyonlar.ConvertToDouble(item.Value.ToString(), 0);
                if (topIndex > 0)
                    XLS.HucreDegerYaz(satir, baslaSutun + topIndex, topDeger);
            }

            XLS.SatirGizle(baslaSatir, baslaSatir, true);

            kur = string.IsNullOrEmpty(kur) ? "-" : kur;
            fon = string.IsNullOrEmpty(fon) ? "-" : fon;
            fin = string.IsNullOrEmpty(fin) ? "-" : fin;
            eko = string.IsNullOrEmpty(eko) ? "-" : eko;
            string kistasBilgi = string.Format("Kur:{0}, Fon:{1}, Fin:{2}, Eko:{3}", kur, fon, fin, eko);
            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("Senaryo", senaryoAd);
            XLS.HucreAdBulYaz("Kistas", kistasBilgi);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public string GiderButceBirlestirme(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam)
        {
            IBHMv2Servis servisBHMv2 = o_servis as IBHMv2Servis ?? TNS.BHMv2.Arac.Tanimla();
            Kullanici kullanan = (Kullanici)o_kullanan;

            string senaryo = (string)raporParam["Senaryo"];
            string senaryoAd = (string)raporParam["SenaryoAd"];
            string kur = (string)raporParam["KurKod"] + "";
            string kurAd = (string)raporParam["KurAd"] + "";
            string fon = (string)raporParam["FonKod"] + "";
            string fin = (string)raporParam["FinKod"] + "";
            string eko = (string)raporParam["EkoKod"] + "";
            string peb = (string)raporParam["PebKod"] + "";

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kur, fon, fin, eko, peb);
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(3), kistas.KodNoKirilimSay(3)));
            kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(4), kistas.KodNoKirilimSay(4)));

            EkDegerParam[] ekBilgiler = new EkDegerParam[1];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryo);

            OdenekBilgi[] odnler = servisBHMv2.ListeTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, false);

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int baslaSatir = 0, baslaSutun = 0;
            int baslikSatir = 0, baslikSutun = 0;

            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
            XLS.HucreAdAdresCoz("BaslaBaslik", ref baslikSatir, ref baslikSutun);

            int yukseklik = XLS.SatirYukseklikAl(baslaSatir);
            int satir = baslaSatir;
            int sutun = baslikSutun;

            ArrayList faaliyetKodList = new ArrayList();
            string kod4 = "", ad4 = "";
            foreach (OdenekBilgi oBilgi in odnler)
            {
                kod4 = oBilgi.tertip.kodlar[4];
                if (!faaliyetKodList.Contains(kod4))
                {
                    ad4 = oBilgi.tertip.adlar[4];
                    faaliyetKodList.Add(kod4);

                    sutun++;
                    XLS.HucreKopyala(baslikSatir, baslikSutun, baslikSatir + 1, baslikSutun, baslikSatir, sutun);
                    XLS.HucreDegerYaz(baslikSatir, sutun, ad4);
                }
            }

            Hashtable liste3 = new Hashtable();
            string kod3 = "", ad3 = "";
            foreach (OdenekBilgi oBilgi in odnler)
            {
                if (kod3 != oBilgi.tertip.kodlar[3])
                {
                    kod3 = oBilgi.tertip.kodlar[3];
                    ad3 = oBilgi.tertip.adlar[3];

                    satir++;
                    XLS.HucreKopyala(baslaSatir, baslaSutun, baslaSatir, baslaSutun + 2 + faaliyetKodList.Count, satir, baslaSutun);
                    XLS.SatirYukseklikAyarla(satir, satir, yukseklik);
                    XLS.HucreDegerYaz(satir, baslaSutun, kod3);
                    XLS.HucreDegerYaz(satir, baslaSutun + 1, ad3);
                    liste3.Add(kod3, satir);
                }
            }

            double[] toplamSatir = new double[faaliyetKodList.Count];
            double[] toplamSutun = new double[liste3.Count];

            int index = 0;
            double deger = 0.0;
            int indexSatir = 0;
            foreach (string faaliyetKod in faaliyetKodList)
            {
                foreach (OdenekBilgi oBilgi in odnler)
                {
                    if (faaliyetKod == oBilgi.tertip.kodlar[4])
                    {
                        kod3 = oBilgi.tertip.kodlar[3];
                        if (liste3.ContainsKey(kod3))
                        {
                            deger = (double)oBilgi.ekRakamlar[0];
                            indexSatir = (int)liste3[kod3];

                            XLS.HucreDegerYaz(indexSatir, baslaSutun + 3 + index, deger);
                            toplamSatir[index] += deger;
                            toplamSutun[indexSatir - 1 - baslaSatir] += deger;
                        }
                    }
                }
                index++;
            }

            double genelToplam = 0.0;

            satir++;
            //TOPLAM SATIR
            XLS.HucreKopyala(baslaSatir, baslaSutun, baslaSatir, baslaSutun + 2 + faaliyetKodList.Count, satir, baslaSutun);
            XLS.SatirYukseklikAyarla(satir, satir, yukseklik);
            XLS.HucreDegerYaz(satir, baslaSutun, "TOPLAM");
            XLS.KoyuYap(satir, baslaSutun, true);
            XLS.HucreBirlestir(satir, baslaSutun, satir, baslaSutun + 1);

            for (int i = 0; i < toplamSatir.Length; i++)
                XLS.HucreDegerYaz(satir, baslaSutun + 3 + i, (double)toplamSatir[i]);

            sutun++;
            //TOPLAM SUTUN
            XLS.HucreKopyala(baslikSatir, baslikSutun, baslikSatir + 1 + satir, baslikSutun, baslikSatir, sutun);
            XLS.HucreDegerYaz(baslikSatir, sutun, "TOPLAM");
            XLS.HucreDegerYaz(baslikSatir - 1, sutun, "(TL)");
            XLS.KoyuYap(baslikSatir, sutun, true);

            for (int i = 0; i < toplamSutun.Length; i++)
            {
                XLS.HucreDegerYaz(baslikSatir + 2 + i, sutun, (double)toplamSutun[i]);
                genelToplam += (double)toplamSutun[i];
            }

            //GENEL TOPLAM
            XLS.HucreDegerYaz(baslikSatir + 2 + toplamSutun.Length, sutun, genelToplam);

            if (string.IsNullOrEmpty(kurAd) || kurAd == "")
                kurAd = kur;

            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("HarcamaYetkilisi", kurAd);
            XLS.HucreAdBulYaz("senaryoAd", senaryoAd);

            int baslik1Satir = 0, baslik1Sutun = 0;
            XLS.HucreAdAdresCoz("Baslik1", ref baslik1Satir, ref baslik1Sutun);
            XLS.HucreBirlestir(baslik1Satir, baslik1Sutun, baslik1Satir, baslik1Sutun + 2 + toplamSatir.Length);
            XLS.HucreAdAdresCoz("Baslik2", ref baslik1Satir, ref baslik1Satir);
            XLS.HucreBirlestir(baslik1Satir, baslik1Sutun, baslik1Satir, baslik1Sutun + 2 + toplamSatir.Length);

            XLS.SatirGizle(baslaSatir, baslaSatir, true);
            XLS.SutunGizle(baslaSutun + 2, baslaSutun + 2, true);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string EkoUcgen(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {
            IBHMv2Servis servisBHMv2 = o_servis as IBHMv2Servis ?? TNS.BHMv2.Arac.Tanimla();
            Kullanici kullanan = (Kullanici)o_kullanan;

            string senaryo = (string)raporParam["Senaryo"];
            string kur = (string)raporParam["KurKod"] + "";
            string kurAd = (string)raporParam["KurAd"] + "";
            string fon = (string)raporParam["FonKod"] + "";
            string fin = (string)raporParam["FinKod"] + "";
            string eko = (string)raporParam["EkoKod"] + "";
            string peb = (string)raporParam["PebKod"] + "";
            string varsayilanKurum = TNS.DEG.Arac.VarsayilanKurumBul();
            if (string.IsNullOrWhiteSpace(kur)) kur = varsayilanKurum;
            Tertip kistas = null;
            UcgenKodBilgileri kodBilgileri = null;
            EkDegerParam[] ekBilgiler = null;
            int sonKolon = 4;
            if (ekBilgi.StartsWith("Gider"))
            {
                kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kur, fon, fin, eko, peb);
                kodBilgileri = new UcgenKodBilgileri();
                kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(3), kistas.KodNoKirilimSay(3)));
                kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(2), kistas.KodNoKirilimSay(2)));

                ekBilgiler = new EkDegerParam[1];
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryo);
            }
            else
            {
                kistas = new Tertip(yil, ENUMTabloKod.TABPEBGelir, kur, eko, peb);
                kodBilgileri = new UcgenKodBilgileri();
                kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(1), kistas.KodNoKirilimSay(1)));
                kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(2), kistas.KodNoKirilimSay(2)));

                ekBilgiler = new EkDegerParam[1];
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBGelir, 0, 0, senaryo);
                sonKolon = 5;
            }
            UcgenSatir[] odnler = servisBHMv2.UcgenTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, false);

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdBulYaz("Baslik1", BHGiderRaporlari.KurumAd());
            if (kur != varsayilanKurum)
            {
                XLS.HucreAdBulYaz("Baslik2", kurAd);
            }
            else
            {
                XLS.HucreAdBulYaz("Baslik2", "Başkanlık Toplam Bütçesi");
            }
            int baslaSatir = 0, baslaSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);

            int satir = baslaSatir;

            if (ekBilgi.StartsWith("Gider"))
            {
                XLS.HucreAdBulYaz("RaporAd", "İşletme Bütçesi - Gider");
                XLS.HucreDegerYaz(baslaSatir - 3, baslaSutun + 4, "Finans");
            }
            else
            {
                XLS.HucreAdBulYaz("RaporAd", "İşletme Bütçesi - Gelir");
            }

            XLS.HucreAdBulYaz("Yil", yil);
            double toplam = 0;
            int sonyer = -1;
            double sonRakam = -1;
            foreach (UcgenSatir oBilgi in odnler)
            {
                if (oBilgi.yer == 0)
                    toplam += (double)oBilgi.ekDegerler[0];
                if (oBilgi.yer < 4 || oBilgi.yer == sonKolon)
                {
                    int ekle = oBilgi.yer;
                    if (oBilgi.yer == sonKolon) ekle = 4;

                    if (oBilgi.yer == sonKolon && sonRakam == (double)oBilgi.ekDegerler[0] && sonyer != oBilgi.yer)
                    {
                        XLS.HucreDegerYaz(satir, baslaSutun + ekle, oBilgi.kodParca);
                    }
                    else
                    {
                        satir++;
                        XLS.HucreKopyala(baslaSatir, baslaSutun, baslaSatir, baslaSutun + 6, satir, baslaSutun);
                        XLS.HucreDegerYaz(satir, baslaSutun + ekle, oBilgi.kodParca);
                        XLS.HucreDegerYaz(satir, baslaSutun + 5, oBilgi.aciklama);
                        XLS.HucreDegerYaz(satir, baslaSutun + 6, (double)oBilgi.ekDegerler[0]);
                        sonRakam = (double)oBilgi.ekDegerler[0];
                        sonyer = oBilgi.yer;
                    }
                }
            }
            satir++;
            XLS.HucreKopyala(baslaSatir - 1, baslaSutun, baslaSatir - 1, baslaSutun + 6, satir, baslaSutun);
            XLS.HucreBirlestir(satir, baslaSutun, satir, baslaSutun + 5);

            XLS.HucreDegerYaz(satir, baslaSutun + 6, toplam);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string Form4_2(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {
            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            TNS.BHMv2.IBHMv2Servis bhmv2Servis = TNS.BHMv2.Arac.Tanimla();

            string kurKod = (string)raporParam["KurKod"];
            string senaryoKod = (string)raporParam["senaryoKod"];
            if (string.IsNullOrWhiteSpace(senaryoKod)) senaryoKod = "0000";
            senaryoKod = senaryoKod.Replace(".", "");

            string sqlOdenek = $"SELECT LEFT(O.EKOKOD, {eko2uz}), SUM(O.ODENEK), E2.EKOAD, E3.EKOAD FROM ButceHazirlikV2.dbo.BHMODENEK AS O INNER JOIN ";
            sqlOdenek += " ABSEKO AS E2 ON O.YIL = E2.YIL AND LEFT(O.EKOKOD, 2) = E2.EKOKOD INNER JOIN ";
            sqlOdenek += $" ABSEKO AS E3 ON O.YIL = E3.YIL AND LEFT(O.EKOKOD, {eko2uz}) = E3.EKOKOD ";
            sqlOdenek += " WHERE (O.YIL = {0}) AND (O.TEKLIFYIL = {1}) AND (O.SENARYOKOD = '{2}') AND (O.KURKOD LIKE '{3}')";
            sqlOdenek += $" GROUP BY LEFT(O.EKOKOD, {eko2uz}), E2.EKOAD, E3.EKOAD  ";
            sqlOdenek += " ORDER BY 1";

            string sqlHarcama = $"SELECT LEFT(O.EKOKOD, {eko2uz}), SUM(O.HARCAMA) FROM ButceHazirlikV2.dbo.ABSHARCAMA AS O ";
            sqlHarcama += " WHERE (O.YIL = {0}) AND (O.KURKOD LIKE '{1}')";
            sqlHarcama += $" GROUP BY LEFT(O.EKOKOD, {eko2uz}) ";
            sqlHarcama += " ORDER BY 1";

            string sqlYSTahmin = $"SELECT LEFT(O.EKOKOD, {eko2uz}), SUM(O.ODENEK) FROM ButceHazirlikV2.dbo.BHMTAHMINGIDER AS O ";
            sqlYSTahmin += " WHERE (O.YIL = {0}) AND (O.TEKLIFYIL = {1}) AND (O.SENARYOKOD = '{2}') AND (O.KURKOD LIKE '{3}')";
            sqlYSTahmin += $" GROUP BY LEFT(O.EKOKOD, {eko2uz}) ";
            sqlYSTahmin += " ORDER BY 1";

            string sqlEkleDus = $"SELECT LEFT(O.EKOKOD, {eko2uz}), SUM(O.EKLENEN), SUM(O.DUSULEN) FROM ButceIslemleri.dbo.ABSBUTCEGIDER AS O ";
            sqlEkleDus += " WHERE (O.YIL = {0}) AND (O.KURKOD LIKE '{1}')";
            sqlEkleDus += $" GROUP BY LEFT(O.EKOKOD, {eko2uz}) ";
            sqlEkleDus += " ORDER BY 1";

            string[] sorgular = new string[5];
            string[] baglantilar = new string[5];

            sorgular[0] = String.Format(sqlOdenek, yil - 2, 0, "0600", kurKod.Replace(".", "") + "%");
            sorgular[1] = String.Format(sqlOdenek, yil - 1, 0, "0600", kurKod.Replace(".", "") + "%");
            sorgular[2] = String.Format(sqlHarcama, yil - 2, kurKod.Replace(".", "") + "%");
            sorgular[3] = String.Format(sqlYSTahmin, yil - 1, 0, senaryoKod, kurKod.Replace(".", "") + "%");
            sorgular[4] = String.Format(sqlEkleDus, yil - 1, kurKod.Replace(".", "") + "%");

            baglantilar[0] = "BaglantiSatiriBHM";
            baglantilar[1] = "BaglantiSatiriBHM";
            baglantilar[2] = "BaglantiSatiriBHM";
            baglantilar[3] = "BaglantiSatiriBHM";
            baglantilar[4] = "BaglantiSatiriBHM";

            SortedList sl = uzyServis.SorguBirlestir(sorgular, baglantilar, new int[] { 0 });
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int sheetNo = 0;
            int fSat1 = 0;
            int fSat2 = 0;
            int fSut1 = 0;
            int fSut2 = 0;
            int baslaSat = 0;
            int baslaSut = 0;

            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref fSat1, ref fSut1, ref fSat2, ref fSut2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSat, ref baslaSut);
            XLS.HucreAdBulYaz("ButceYil", yil);
            XLS.HucreAdBulYaz("KurumAd", TNS.DEG.Arac.VarsayilanKurumBul() + " - " + BHGiderRaporlari.KurumAd());
            XLS.HucreAdBulYaz("BirimAd", kurKod + " - " + uzyServis.UzayDegeriStr((Kullanici)kullanan, "ABSKUR", yil.ToString() + "-" + kurKod, true));
            int eskiEko2Sat = -1;
            string eskiEko2 = "";

            ToplamAl toplamAl = new ToplamAl(3, 8);
            int satir = baslaSat;

            foreach (DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                string ekoKod = alAnahtar.StrAl(0);
                string eko2 = ekoKod.Substring(0, 2);

                toplamAl.DegerAta(2, 0, alVeri.DblAl(0));   //0. sorgu odenek
                string eko2Ad = alVeri.StrAl(1); //0. sorgu odenek
                string ekoAd = alVeri.StrAl(2); //0. sorgu odenek

                toplamAl.DegerAta(2, 2, alVeri.DblAl(3));   //1. sorgu odenek
                if (string.IsNullOrWhiteSpace(eko2Ad))
                    eko2Ad = alVeri.StrAl(4); //1. sorgu odenek
                if (string.IsNullOrWhiteSpace(ekoAd))
                    ekoAd = alVeri.StrAl(5); //1. sorgu odenek

                toplamAl.DegerAta(2, 1, alVeri.DblAl(6));   //2. sorgu harcama
                toplamAl.DegerAta(2, 6, alVeri.DblAl(7));   //3. sorgu harcama

                toplamAl.DegerAta(2, 3, alVeri.DblAl(8));   //3. sorgu harcama
                toplamAl.DegerAta(2, 4, alVeri.DblAl(9));   //3. sorgu harcama

                if (eskiEko2 != eko2)
                {
                    if (eskiEko2Sat > 0)
                    {
                        RakamYaz4_2(XLS, eskiEko2Sat, baslaSut + 3, toplamAl, 1);
                        toplamAl.SatirSifirla(1);
                    }

                    XLS.HucreKopyala(fSat1, fSut1, fSat1, fSut1 + 15, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut, eko2);
                    XLS.HucreDegerYaz(satir, baslaSut + 2, eko2Ad);
                    eskiEko2 = eko2;
                    eskiEko2Sat = satir;
                    satir++;
                }

                XLS.HucreKopyala(fSat1 + 1, fSut1, fSat1 + 1, fSut1 + 15, satir, baslaSut);
                XLS.HucreDegerYaz(satir, baslaSut + 1, ekoKod.Substring(2));
                XLS.HucreDegerYaz(satir, baslaSut + 2, ekoAd);

                ObjectArray oa = bhmv2Servis.ButceYSTahminListele((Kullanici)kullanan, senaryoKod, new Tertip(yil - 1, ENUMTabloKod.TABHarcamaTahmin, kurKod, "", "", ekoKod, ""));
                string aciklama = "";
                foreach (TertipOdenek odn in oa.objeler)
                {
                    if (!string.IsNullOrWhiteSpace(odn.aciklama))
                    {
                        if (aciklama.IndexOf(odn.aciklama) < 0)
                        {
                            if (aciklama != "") aciklama += "\r\n";
                            aciklama += odn.aciklama;
                        }
                    }
                }
                XLS.HucreDegerYaz(satir, baslaSut + 11, aciklama);

                RakamYaz4_2(XLS, satir, baslaSut + 3, toplamAl, 2);
                toplamAl.ToplamEkle(1, 2);
                toplamAl.ToplamEkle(0, 2);
                satir++;
            }

            if (eskiEko2Sat > 0)
            {
                RakamYaz4_2(XLS, eskiEko2Sat, baslaSut + 3, toplamAl, 1);
                toplamAl.SatirSifirla(1);
            }

            XLS.HucreKopyala(fSat1, fSut1, fSat1, fSut1 + 15, satir, baslaSut);
            XLS.HucreDegerYaz(satir, baslaSut, "T O P L A M");
            XLS.HucreBirlestir(satir, baslaSut, satir, baslaSut + 1);
            RakamYaz4_2(XLS, satir, baslaSut + 3, toplamAl, 0);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void RakamYaz4_2(Tablo XLS, int sat, int sut, ToplamAl toplamAl, int tSat)
        {
            XLS.HucreDegerYaz(sat, sut + 0, toplamAl.toplam[tSat, 0]);
            XLS.HucreDegerYaz(sat, sut + 1, toplamAl.toplam[tSat, 1]);
            XLS.HucreDegerYaz(sat, sut + 2, toplamAl.toplam[tSat, 2]);
            XLS.HucreDegerYaz(sat, sut + 3, toplamAl.toplam[tSat, 3]);
            XLS.HucreDegerYaz(sat, sut + 4, toplamAl.toplam[tSat, 4]);
            double t = toplamAl.toplam[tSat, 2] + toplamAl.toplam[tSat, 3] - toplamAl.toplam[tSat, 4];
            XLS.HucreDegerYaz(sat, sut + 5, t);
            XLS.HucreDegerYaz(sat, sut + 6, toplamAl.toplam[tSat, 6]);
            XLS.HucreDegerYaz(sat, sut + 7, t - toplamAl.toplam[tSat, 6]);
        }
    }

    /// <summary>Eski ButceForm/ButceFormRaporlari.cs'ten Form 25 (Tavan İçerisinde Teklif Edilemeyen Ödenekler, detaydan: F25d).</summary>
    internal class ButceFormRaporlari
    {
        Kullanici kullanan;
        IBHFServis servis;
        int yil;
        string kurKod;
        string kurumKod;
        string kurumAd;
        string sablonAd;
        string sonucDosyaAd;
        string senaryo;

        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgiler)
        {
            if (ekBilgiler == "F25" || ekBilgiler == "F25d")
                return TavanUstuOdenekHazirla(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgiler);
            throw new RaporHatasi("Tanımsız Rapor Parametresi");
        }

        private void DegerAl(object o_kullanan, object o_servis, System.Collections.Hashtable raporParam, string p_SonucDosyaAd, string p_SablonAd)
        {
            kullanan = (Kullanici)o_kullanan;
            servis = o_servis as IBHFServis ?? TNS.BHF.Arac.Tanimla();

            yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            kurKod = (string)raporParam["KurKod"] + "";
            kurumKod = TNS.DEG.Arac.VarsayilanKurumBul();
            kurumAd = BHGiderRaporlari.KurumAd();

            sonucDosyaAd = p_SonucDosyaAd + "." + GenelIslemler.ExcelTur();
            sablonAd = p_SablonAd;
            senaryo = (string)raporParam["Senaryo"];
        }

        public string TavanUstuOdenekHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd);
            ObjectArray tsv;
            if (ekBilgi == "F25d")
                tsv = servis.TavanUstuListeleDetaydan(kullanan, yil, kurKod, senaryo, false);
            else
                tsv = servis.TavanUstuListele(kullanan, yil, kurKod);

            if (tsv.sonuc.islemSonuc)
            {
                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;
                int sutun = 0;
                XLS.DosyaAc(sablonAd, sonucDosyaAd);
                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

                XLS.HucreAdBulYaz("ButceYil", yil);
                XLS.HucreAdBulYaz("KurumAd", kurumAd);
                XLS.HucreAdBulYaz("KurumKod", kurumKod);

                int kaynakSatir = satir;
                int z = 0;
                string eskibirim = "";
                for (int i = 0; i < tsv.ObjeSayisi; i++)
                {
                    TNS.BHF.TavanUstuOdenek tss = (TNS.BHF.TavanUstuOdenek)tsv[i];

                    if (tss.kurumsalKod != eskibirim)
                    {
                        XLS.SatirAc(satir + 1, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun - 1, kaynakSatir, sutun + 21, satir + 1, sutun - 1);
                        satir++;
                        z++;
                        XLS.HucreDegerYaz(satir, sutun - 1, z);
                        XLS.HucreDegerYaz(satir, sutun, tss.kurumsalKod);
                    }

                    if (!string.IsNullOrEmpty(tss.aciklama))
                    {
                        string eskiDeger = XLS.HucreDegerAl(satir, sutun + 1);
                        if (!string.IsNullOrEmpty(eskiDeger)) eskiDeger += "\r\n";
                        XLS.HucreDegerYaz(satir, sutun + 1, eskiDeger + tss.aciklama);
                    }

                    for (int j = 0; j < 14; j++)
                    {
                        if (tss.ekokod.Replace(".", "") == XLS.HucreDegerAl(kaynakSatir - 4, sutun + 2 + j))
                        {
                            XLS.HucreDegerYaz(satir, sutun + j + 2, tss.odenek);
                            break;
                        }
                    }

                    eskibirim = tss.kurumsalKod;
                }

                double toplam = 0;
                for (int j = kaynakSatir + 1; j < satir + 1; j++)
                {
                    toplam = 0;
                    for (int i = 5; i < 14; i++)
                    {
                        toplam += XLS.HucreDegerAlDbl(0, j, i);
                    }
                    XLS.HucreDegerYaz(j, 14, toplam);
                }

                XLS.SatirAc(satir + 1, 1);
                XLS.HucreKopyala(kaynakSatir, sutun - 1, kaynakSatir, sutun + 21, satir + 1, sutun - 1);

                XLS.HucreBirlestir(satir + 1, sutun - 1, satir + 1, sutun + 1);
                XLS.HucreDegerYaz(satir + 1, sutun - 1, "GENEL TOPLAM");

                for (int j = 3; j < 18; j++)
                {
                    toplam = 0;
                    for (int i = kaynakSatir + 1; i < satir + 1; i++)
                    {
                        toplam += XLS.HucreDegerAlDbl(0, i, j);
                    }
                    XLS.HucreDegerYaz(satir + 1, j, toplam);
                }

                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new RaporHatasi(tsv.sonuc.hataStr);
        }
    }

    /// <summary>Eski ProgramButceRaporlari.cs (Program Bütçe Ödenekleri listesi).</summary>
    internal class ProgramButceRaporlari
    {
        private Kullanici kullanan;
        private int yil;
        private Tertip tUz;
        private int eko2uz;
        private int kurKirSay;
        private IBHMv2Servis servis;
        IUZYServis uzyServis;
        private string senaryoKod;
        private string senaryoAd;
        private string kurum;
        private string kurKod;
        private string fon;
        private string fin;
        private string eko;
        private string peb;
        private int kirilim;
        private Tablo XLS;
        private int satir = 0;
        private int sutun = 0;
        private int fsatir1 = 0;
        private int fsutun1 = 0;
        private int fsatir2 = 0;
        private int fsutun2 = 0;
        private int fSayfaNo = 0;

        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {
            this.kullanan = (Kullanici)kullanan;
            this.servis = TNS.BHMv2.Arac.Tanimla();

            yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            tUz = new Tertip(yil, ENUMTabloKod.TABPRGOdenek);
            eko2uz = tUz.KodUzunlukBul(3, 2, true);
            kurKirSay = tUz.KodNoKirilimSay(0);
            uzyServis = TNS.UZY.Arac.Tanimla();

            senaryoKod = (string)raporParam["Senaryo"];
            senaryoAd = this.servis.ButceSenaryoAdBul(this.kullanan.aktifKultur, senaryoKod);
            kurum = TNS.DEG.Arac.VarsayilanKurumBul();
            kurKod = (string)raporParam["KurKod"] + "";
            if (string.IsNullOrWhiteSpace(kurKod)) kurKod = kurum;

            fon = (string)raporParam["FonKod"] + "";
            fin = (string)raporParam["FinKod"] + "";
            eko = (string)raporParam["EkoKod"] + "";
            peb = (string)raporParam["PebKod"] + "";
            kirilim = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Kirilim"], 0);
            if (kirilim == 0)
                kirilim = 2;

            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            if (kurum != "")
                XLS.HucreAdBulYaz("KurumAd", kurum + " - " + uzyServis.UzayDegeriStr(this.kullanan, "ABSKUR", yil.ToString() + "-" + kurum, true));

            XLS.HucreAdBulYaz("ButceYil", yil);
            XLS.HucreAdBulYaz("SenaryoAd", senaryoAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            XLS.HucreAdAdresCoz("FormatSatir", ref fSayfaNo, ref fsatir1, ref fsutun1, ref fsatir2, ref fsutun2);

            if (ekBilgi.Equals("Liste"))
                PBListeAl();
            else
                throw new RaporHatasi("Tanımsız Rapor Parametresi");
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void PBListeAl()
        {
            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPRGOdenek, "", "", "", "", "", kurKod, fin, eko, peb, fon);

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();

            for (int i = 0; i < kistas.KodNoSay(); i++)
            {
                if (i >= 9) continue;
                kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(i), kistas.KodNoKirilimSay(i)));
            }
            XLS.HucreAdBulYaz("AyAd", "Haziran");
            EkDegerParam[] ekBilgiler = new EkDegerParam[9];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPRGOdenek, 0, 0, senaryoKod);
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPRGOdenek, 0, 1, senaryoKod);
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPRGOdenek, 0, 2, senaryoKod);
            ekBilgiler[3] = new EkDegerParam(ENUMTabloKod.TABPRGTavanUstu, 0, 0, senaryoKod);
            ekBilgiler[4] = new EkDegerParam(ENUMTabloKod.TABPRGTavanUstu, 0, 1, senaryoKod);
            ekBilgiler[5] = new EkDegerParam(ENUMTabloKod.TABPRGTavanUstu, 0, 2, senaryoKod);
            ekBilgiler[6] = new EkDegerParam(ENUMTabloKod.TABPRGHarcamaTahmin, -1, 0, senaryoKod);
            ekBilgiler[7] = new EkDegerParam(ENUMTabloKod.TABPRGHarcama, -1, 0, "6");
            ekBilgiler[8] = new EkDegerParam(ENUMTabloKod.TABPRGOdemeEmriAvansiz, -2, 0, "12");

            OdenekBilgi[] cevap = servis.ListeTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, true);

            for (int k = 0; k < cevap.Length; k++)
            {
                XLS.HucreKopyala(fsatir1, fsutun1, fsatir2, fsutun2, satir, sutun);

                XLS.HucreDegerYaz(satir, 0, cevap[k].tertip.TertipStrOl());
                for (int i = 0; i < ekBilgiler.Length; i++)
                {
                    XLS.HucreDegerYaz(satir, 1 + i, cevap[k].EkRakam(i));
                }

                for (int i = 0; i < cevap[k].tertip.kodlar.Length; i++)
                {
                    XLS.HucreDegerYaz(satir, 2 + ekBilgiler.Length + i, cevap[k].tertip.kodlar[i]);
                }
                satir++;
            }
            XLS.SutunGenislikAyarla(11, 23, 0);
        }
    }
}
