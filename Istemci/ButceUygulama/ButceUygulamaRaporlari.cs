using System;
using System.Collections;
using System.IO;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.BUM;
using TNS.KYM;

namespace Istemci.ButceUygulama
{
    /// <summary>
    /// Ayrıntılı Harcama / Gelir Programı raporları (Ek 1A, Ek 1B, Ek 2, Ek 3C, İHP-AHP kıyas). Eski ButceUygulama/ButceUygulamaRaporlari.cs'ten
    /// alındı; rapor kodu aynıdır. Değişenler: System.Web, TNS.Raporlama.IRapor ve kaynak dosyaları kaldırıldı (metinler yazıldı);
    /// GunlukNakitIhtiyaci raporu GunlukNakitIhtiyaciRaporu.cs'te olduğundan buraya alınmadı; şablon klasörü RaporAl'a verilir
    /// (SablonKlasoru: önce kurumun RaporSablonYol\BUM klasörü, yoksa uygulamayla gelen SGB.Net\RaporSablon\BUM).
    /// </summary>
    public class ButceUygulamaRaporlari
    {
        string eko;
        string peb;
        string raporKurum;
        int raporYil, raporAsama;
        IBKDServis servisBKD;
        IBUMServis servisBUM;
        Kullanici kullanan;
        string raporBirim;
        string raporSablon;
        string raporSonucDosyaAd;
        string raporKurumAd;
        string raporBirimAd;
        string AdSoyadSol = "";
        string UnvanSol = "";
        string AdSoyadSag = "";
        string UnvanSag = "";
        string raporEkBilgi;
        object tertipTam = null;
        ENUMTabloKod tabloKod = ENUMTabloKod.TABPEBOdenek;

        /// <summary>BUM şablon klasörü: kurumun RaporSablonYol\BUM klasöründe istenen şablon varsa o, yoksa SGB.Net\RaporSablon\BUM.</summary>
        public static string SablonBul(string uygulamaKlasoru, string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "BUM", sablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "BUM", sablonAd);
            return File.Exists(u) ? u : null;
        }

        private static double YuzdeBul(double miktar, double tum)
        {
            if (tum == 0) return -1;
            else return ((miktar / tum) * 100);
        }

        /// <summary>
        /// Sunucu tarafındaki Kod listeleme rutinini çalıştırarak kod listelerini elde eden, daha sonra Tablo oluşturarak,
        /// Rapor parametrelerini (Rapor yılı, Kurum adı v.b.) hücrelere yazan, sonra tertip bilgilerini oluşturarak,
        /// sunucu tarafındaki yordamları çalıştırarak Ayrıntılı Program ve İcmal Harcama Programları bilgileri getiren,
        /// getirdiği bilgilerle Ek 1 A Raporunu hazırlayan ve hazırlamış olduğu dosyayı kaydederek, dosya adını döndüren fonksiyon
        /// </summary>
        /// <returns>Fonksiyonun çalışması sonucunda, Raporun dosya adını tutan String türünde nesne döndürür</returns>
        public string Ek1A()
        {
            int[] indisaylarToplam = new int[5] { 0, 2, 4, 6, 8 };
            double[,] aylarTopDeg = new double[9, 13];
            double[] EnaltToplam = new double[13];

            if (!(raporKurum != "" && raporYil > 0 && raporAsama > 0))
                throw new Exception("Yıl veya Kurum alanlarından biri veya bir kaçı boş bırakılmış. Lütfen ilgili alanları kontrol edip tekrar deneyin.");

            int kodTuru;
            bool eko23;
            if (tabloKod == ENUMTabloKod.TABGelir)
            {
                kodTuru = 5;
                eko23 = false;
            }
            else
            {
                kodTuru = 4;
                eko23 = TNS.BKD.Arac.Eko2_3Bul(kullanan);
            }

            ABSKodlarVAN akv = servisBKD.KodListele(Convert.ToInt16(raporYil), kodTuru.ToString(), "");

            if (!akv.sonuc.islemSonuc)
                throw new Exception(akv.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;

            XLS.DosyaAc(raporSablon, raporSonucDosyaAd + "." + GenelIslemler.ExcelTur());
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            int kaynakSatir = satir;

            XLS.HucreAdBulYaz("ButceYil", raporYil);
            XLS.HucreAdBulYaz("KurumAd", raporKurumAd);
            XLS.HucreAdBulYaz("BirimAd", raporBirimAd);
            XLS.HucreAdBulYaz("AdSoyadSol", AdSoyadSol);
            XLS.HucreAdBulYaz("UnvanSol", UnvanSol);
            XLS.HucreAdBulYaz("AdSoyadSag", AdSoyadSag);
            XLS.HucreAdBulYaz("UnvanSag", UnvanSag);

            int ekoYer = 3;
            if (tabloKod == ENUMTabloKod.TABGelir)
                ekoYer = 1;

            for (int i = 0; i < akv.absKodlar.Count; i++)
            {
                ABSKod ak = (ABSKod)akv.absKodlar[i];
                int ilkKirilimTut = -1;

                TertipOdenek[] degerler = null;
                if (raporEkBilgi.Contains("DETAY"))
                {
                    Tertip tertip = new Tertip(raporYil, tabloKod, raporBirim);
                    tertip.kodlar[ekoYer] = ak.kod;

                    UcgenKodBilgileri ukb = new UcgenKodBilgileri();
                    ukb.Add(new UcgenKodBilgi(kodTuru, 2, eko23));
                    degerler = servisBUM.APListeleKodBilgi(kullanan, raporAsama, tertip, ukb);
                }
                else
                {
                    Tertip tertip = new Tertip(raporYil, ENUMTabloKod.TABGiderIcmal, raporBirim.ToString(), ak.kod);
                }

                int ilkKirilim = 0;
                foreach (TertipOdenek tb in degerler)
                {
                    if (ilkKirilim == 0)
                    {
                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir - 2, sutun, kaynakSatir - 2, sutun + 40, satir, sutun);
                        XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(450));

                        XLS.HucreDegerYaz(satir, sutun, ak.kod);
                        XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 2);
                        XLS.HucreDegerYaz(satir, sutun + 1, ak.Ad());
                        ilkKirilimTut = satir;

                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir - 1, sutun, kaynakSatir - 1, sutun + 40, satir, sutun);
                        XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(450));
                    }
                    else
                    {
                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 40, satir, sutun);
                        XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(450));
                    }

                    ilkKirilim = 1;

                    XLS.HucreDegerYaz(satir, sutun + 1, tb.tertip.kodlar[ekoYer].Substring(3, 1));
                    XLS.HucreDegerYaz(satir, sutun + 2, tb.tertip.adlar[ekoYer]);
                    XLS.HucreDegerYaz(satir, sutun + 3, tb.odenek[0]);

                    aylarTopDeg[i, 12] += (double)tb.odenek[0];

                    for (int k = 0; k < 12; k++)
                    {
                        aylarTopDeg[i, k] += (double)tb.odenek[k + 2];
                        if (k == 2 || k == 5 || k == 8 || k == 11)
                        {
                            double deger = (double)tb.odenek[k + 2] + (double)tb.odenek[k + 1] + (double)tb.odenek[k];
                            int indis = (k - 2) / 3;
                            XLS.HucreDegerYaz(satir, indisaylarToplam[indis] + 4, deger);
                            XLS.HucreDegerYaz(satir, indisaylarToplam[indis] + 5, deger / tb.odenek[0] * 100);
                        }
                        if (k == 11)
                        {
                            double gdeger = 0;
                            for (int f = 0; f < 12; f++)
                                gdeger += (double)tb.odenek[f + 2];
                            XLS.HucreDegerYaz(satir, indisaylarToplam[4] + 4, gdeger);
                            XLS.HucreDegerYaz(satir, indisaylarToplam[4] + 5, gdeger / tb.odenek[0] * 100);
                        }
                    }
                }

                if (ilkKirilimTut != -1)
                {
                    XLS.HucreDegerYaz(ilkKirilimTut, sutun + 3, aylarTopDeg[i, 12]);
                    for (int j = 0; j < 12; j++)
                    {
                        if (j == 2 || j == 5 || j == 8 || j == 11)
                        {
                            double deger = aylarTopDeg[i, j] + aylarTopDeg[i, j - 1] + aylarTopDeg[i, j - 2];
                            int indis = (j - 2) / 3;
                            XLS.HucreDegerYaz(ilkKirilimTut, indisaylarToplam[indis] + 4, deger);
                            XLS.HucreDegerYaz(ilkKirilimTut, indisaylarToplam[indis] + 5, deger / aylarTopDeg[i, 12] * 100);
                        }
                        if (j == 11)
                        {
                            double gdeger = 0;
                            for (int f = 0; f < 12; f++)
                                gdeger += (double)aylarTopDeg[i, f];
                            XLS.HucreDegerYaz(ilkKirilimTut, indisaylarToplam[4] + 4, gdeger);
                            XLS.HucreDegerYaz(ilkKirilimTut, indisaylarToplam[4] + 5, gdeger / aylarTopDeg[i, 12] * 100);
                        }
                    }
                    ilkKirilimTut = -1;
                }
            }

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir - 2, sutun, kaynakSatir - 2, sutun + 40, satir, sutun);
            XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(450));

            XLS.HucreBirlestir(satir, sutun, satir, sutun + 2);
            XLS.HucreDegerYaz(satir, sutun, "TOPLAM BÜTÇE GİDERİ");

            for (int f = 0; f < 9; f++)
            {
                for (int j = 0; j < 13; j++)
                    EnaltToplam[j] += aylarTopDeg[f, j];
            }
            XLS.HucreDegerYaz(satir, sutun + 3, EnaltToplam[12]);
            for (int k = 0; k < 12; k++)
            {
                if (k == 2 || k == 5 || k == 8 || k == 11)
                {
                    double deger = EnaltToplam[k] + EnaltToplam[k - 1] + EnaltToplam[k - 2];
                    int indis = (k - 2) / 3;
                    XLS.HucreDegerYaz(satir, indisaylarToplam[indis] + 4, deger);
                    XLS.HucreDegerYaz(satir, indisaylarToplam[indis] + 5, deger / EnaltToplam[12] * 100);
                }
                if (k == 11)
                {
                    double gdeger = 0;
                    for (int f = 0; f < 12; f++)
                        gdeger += (double)EnaltToplam[f];
                    XLS.HucreDegerYaz(satir, indisaylarToplam[4] + 4, gdeger);
                    XLS.HucreDegerYaz(satir, indisaylarToplam[4] + 5, gdeger / EnaltToplam[12] * 100);
                }
            }
            XLS.DosyaSaklaTamYol();
            return raporSonucDosyaAd + "." + GenelIslemler.ExcelTur();
        }

        /// <summary>
        /// Rapor Kurum, Yıl ve Aşama bilgilerini kontrol ederek,
        /// Rapor parametrelerini (Rapor yılı, Kurum adı v.b.) kullanarak tertip bilgilerini oluşturarak,
        /// sunucu tarafındaki yordamları çalıştırarak Ayrıntılı Program bilgileri getiren,
        /// Getirdiği bilgilerle Tablo oluşturan ve tablonun içeriğini doldurarak Ek 2 Raporunu hazırlayan ve hazırlamış olduğu dosyayı kaydederek, dosya adını döndüren fonksiyon
        /// </summary>
        /// <returns>Fonksiyonun çalışması sonucunda, Raporun dosya adını tutan String türünde nesne döndürür</returns>
        public string Ek2(bool eko2)
        {
            if (raporKurum != "" && raporYil > 0 && raporAsama > 0)
            {
                string birimKod;
                if (raporBirim != "00" && raporBirim != "")
                    birimKod = raporBirim;
                else
                    birimKod = raporKurum;
                Tertip tertip = new Tertip(raporYil, ENUMTabloKod.TABPEBOdenek, birimKod, "", "", eko, peb);
                TertipOdenek[] degerler = null;

                UcgenKodBilgileri ukb = new UcgenKodBilgileri();
                ukb.Add(new UcgenKodBilgi(tertip.KodNolar(0), tertip.KodNoKirilimSay(0)));
                ukb.Add(new UcgenKodBilgi(tertip.KodNolar(1), tertip.KodNoKirilimSay(1)));
                ukb.Add(new UcgenKodBilgi(tertip.KodNolar(2), tertip.KodNoKirilimSay(2)));
                if (eko2)
                    ukb.Add(new UcgenKodBilgi(tertip.KodNolar(3), 2, TNS.BKD.Arac.Eko2_3Bul(kullanan)));
                else
                    ukb.Add(new UcgenKodBilgi(tertip.KodNolar(3), tertip.KodNoKirilimSay(3)));
                if (raporEkBilgi.Contains("DETAY"))
                {
                    ukb.Add(new UcgenKodBilgi(tertip.KodNolar(4), tertip.KodNoKirilimSay(4)));
                }
                int toplamKirilimSay = 0;
                for (int i = 0; i < ukb.BoyutSayisi; i++)
                {
                    toplamKirilimSay += ukb[i].kirilimSay;
                }

                degerler = servisBUM.APListeleKodBilgi(kullanan, raporAsama, tertip, ukb);
                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;
                int sutun = 0;

                XLS.DosyaAc(raporSablon, raporSonucDosyaAd + "." + GenelIslemler.ExcelTur());
                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                int kaynakSatir = satir;
                int kaynakSutun = sutun;
                if (toplamKirilimSay <= 11)
                {
                    XLS.ZoomYazici(40);
                }
                else
                {
                    XLS.ZoomYazici(38);
                }
                XLS.HucreAdBulYaz("ButceYil", raporYil);
                XLS.HucreAdBulYaz("KurumAd", raporKurumAd);
                XLS.HucreAdBulYaz("AdSoyadSol", AdSoyadSol);
                XLS.HucreAdBulYaz("UnvanSol", UnvanSol);
                XLS.HucreAdBulYaz("AdSoyadSag", AdSoyadSag);
                XLS.HucreAdBulYaz("UnvanSag", UnvanSag);
                XLS.SutunGenislikAyarla(sutun + toplamKirilimSay, sutun + 17 - 1, 0);
                int[] baslaYerDizi = { 0, 4, 8, 9, 13 };
                if (eko2) baslaYerDizi[4] = 11;

                string[] EskiParcaliKod = new string[toplamKirilimSay];
                foreach (TertipOdenek tb in degerler)
                {
                    sutun = kaynakSutun;

                    if (kaynakSatir != satir)
                    {
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 54, satir, sutun);
                        XLS.SatirYukseklikAyarla(satir, satir, XLS.SatirYukseklikAl(satir - 1));
                    }
                    tb.tertip.Tertip23Ayarla(3);
                    string[] ParcaliKod = tb.tertip.TertipStrOl().Split('.', '-');
                    int j = 0;
                    if (tertipTam == null)
                    {
                        for (j = 0; ParcaliKod[j] == EskiParcaliKod[j] && j < ParcaliKod.Length && j < EskiParcaliKod.Length; j++)
                        {
                            XLS.HucreDegerYaz(satir, sutun + j, "");
                        }
                    }
                    for (; j < ParcaliKod.Length; j++)
                    {
                        XLS.HucreDegerYaz(satir, sutun + j, ParcaliKod[j]);
                        EskiParcaliKod[j] = ParcaliKod[j];
                    }
                    if (tertipTam != null)
                    {
                        XLS.HucreDegerYaz(satir, sutun + j, tb.tertip.TertipStrOl());
                    }

                    sutun = kaynakSutun + 17;
                    if (toplamKirilimSay == 11)
                        XLS.HucreDegerYaz(satir, sutun - 1, tb.tertip.adlar[3]);
                    else
                        XLS.HucreDegerYaz(satir, sutun - 1, tb.tertip.adlar[4]);

                    XLS.HucreDegerYaz(satir, sutun, tb.odenek[0]);

                    for (int aylar = 0; aylar < 12; aylar++)
                    {
                        XLS.HucreDegerYaz(satir, sutun + 1 + aylar * 2, tb.odenek[aylar + 2]);
                        XLS.HucreDegerYaz(satir, sutun + 2 + aylar * 2, YuzdeBul(tb.odenek[aylar + 2], tb.odenek[0]));
                        if (aylar == 2 || aylar == 8) sutun = sutun + 2;
                        if (aylar == 5) sutun = sutun + 4;
                    }
                    satir++;
                }
                XLS.DosyaSaklaTamYol();
                return raporSonucDosyaAd + "." + GenelIslemler.ExcelTur();
            }
            else
                throw new Exception("Yıl veya Kurum alanlarından biri veya bir kaçı boş bırakılmış. Lütfen ilgili alanları kontrol edip tekrar deneyin.");
        }


        /// <summary>
        /// Rapor Kurum, Yıl ve Aşama bilgilerini kontrol ederek,
        /// Rapor parametrelerini (Rapor yılı, Kurum adı v.b.) kullanarak tertip bilgilerini oluşturarak,
        /// sunucu tarafındaki yordamları çalıştırarak Ayrıntılı Program bilgileri getiren,
        /// Getirdiği bilgilerle Tablo oluşturan ve tablonun içeriğini doldurarak Ek 3C Raporunu hazırlayan ve hazırlamış olduğu dosyayı kaydederek, dosya adını döndüren fonksiyon
        /// </summary>
        /// <returns>Fonksiyonun çalışması sonucunda, Raporun dosya adını tutan String türünde nesne döndürür</returns>
        public string Ek3C()
        {
            if (raporKurum != "" && raporYil > 0 && raporAsama > 0)
            {
                Tertip tertip = new Tertip(raporYil, ENUMTabloKod.TABGelir, raporKurum, "");
                string[] oncTertip = { "", "", "", "" };
                bool yaz = false;
                TertipOdenek[] degerler = null;

                degerler = servisBUM.APListele(kullanan, raporAsama, tertip);
                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;
                int sutun = 0;


                XLS.DosyaAc(raporSablon, raporSonucDosyaAd + "." + GenelIslemler.ExcelTur());
                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                int kaynakSatir = satir;
                int kaynakSutun = sutun;

                XLS.HucreAdBulYaz("ButceYil", raporYil);
                XLS.HucreAdBulYaz("KurumAd", raporKurumAd);
                XLS.HucreAdBulYaz("AdSoyadSol", AdSoyadSol);
                XLS.HucreAdBulYaz("UnvanSol", UnvanSol);
                XLS.HucreAdBulYaz("AdSoyadSag", AdSoyadSag);
                XLS.HucreAdBulYaz("UnvanSag", UnvanSag);

                foreach (TertipOdenek tb in degerler)
                {
                    yaz = false;
                    sutun = kaynakSutun;
                    if (kaynakSatir != satir)
                    {
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 42, satir, sutun);
                        XLS.SatirYukseklikAyarla(satir, satir, XLS.SatirYukseklikAl(satir - 1));
                    }
                    tb.tertip.Tertip23Ayarla(3);
                    string[] ParcaliKod = tb.tertip.kodlar[1].Split('.');
                    for (int i = 0; i < ParcaliKod.Length; i++)
                    {
                        if (oncTertip[i] != ParcaliKod[i].ToString() || yaz == true)
                        {
                            yaz = true;
                            XLS.HucreDegerYaz(satir, sutun, ParcaliKod[i].ToString());
                        }
                        else
                            XLS.HucreDegerYaz(satir, sutun, "");
                        oncTertip[i] = ParcaliKod[i];

                        sutun = sutun + 1;
                    }


                    if (ParcaliKod[0] == "09")
                        tb.odenek[0] = tb.odenek[0] * -1;

                    XLS.HucreDegerYaz(satir, sutun, tb.odenek[0]);

                    for (int aylar = 0; aylar < 12; aylar++)
                    {
                        double ayDeger = tb.odenek[aylar + 2];
                        if (ParcaliKod[0] == "09")
                            ayDeger = ayDeger * (-1);

                        XLS.HucreDegerYaz(satir, sutun + 1 + aylar * 2, ayDeger);
                        XLS.HucreDegerYaz(satir, sutun + 2 + aylar * 2, ayDeger * 100 / tb.odenek[0]);
                        if (aylar == 2 || aylar == 8) sutun = sutun + 2;
                        if (aylar == 5) sutun = sutun + 4;
                    }
                    satir++;
                }
                XLS.DosyaSaklaTamYol();
                return raporSonucDosyaAd + "." + GenelIslemler.ExcelTur();
            }
            else
                throw new Exception("Yıl veya Kurum alanlarından biri veya bir kaçı boş bırakılmış. Lütfen ilgili alanları kontrol edip tekrar deneyin.");
        }

        /// <summary>
        /// Parametre olarak verilen Tablo Satırına, indis sayılar ve Ay değerler dizilerini kullanarak, KBÖ'yü göre,
        /// toplamını bulup hücreye değer olarak yazan fonksiyon
        /// </summary>
        /// <param name="XLS">Raporlama için kullanacak Excel tablosunu tutan, Tablo türünde nesne</param>
        /// <param name="satir">Satır bilgisini tutan, Int türünde nesne</param>
        /// <param name="indisaylar">İndis sayıları tutan, Int dizi türünde nesne</param>
        /// <param name="ayDegerler">Ay değelerini tutan, Int dizi türünde nesne</param>
        /// <param name="kbo">KBÖ değerini tutan, Double türünde nesne</param>
        /// <param name="aylarTopDeg">Ayların toplam değerlerini tutan, Double türünde nesne</param>
        private void SatirYazToplamli(Tablo XLS, int satir, int[] indisaylar, double[] ayDegerler, double kbo, double[] aylarTopDeg)
        {
            double satirToplam = 0;
            for (int k = 0; k < 12; k++)
            {
                XLS.HucreDegerYaz(satir, indisaylar[k] + 4, ayDegerler[k]);
                satirToplam += ayDegerler[k];
                if (kbo > 0)
                    XLS.HucreDegerYaz(satir, indisaylar[k] + 5, ayDegerler[k] / kbo * 100);
                if (aylarTopDeg != null)
                    aylarTopDeg[k] += ayDegerler[k];

                if (k == 0 || k == 3 || k == 6 || k == 9)
                {
                    double deger = (double)ayDegerler[k + 2] + (double)ayDegerler[k + 1] + (double)ayDegerler[k];
                    XLS.HucreDegerYaz(satir, indisaylar[k] + 4 + 2, deger);
                    if (kbo > 0)
                        XLS.HucreDegerYaz(satir, indisaylar[k] + 5 + 2, deger / kbo * 100);
                    if (k == 5 || k == 11)
                    {
                        XLS.HucreDegerYaz(satir, indisaylar[k] + 4 + 4, satirToplam);
                        if (kbo > 0)
                            XLS.HucreDegerYaz(satir, indisaylar[k] + 5 + 4, satirToplam / kbo * 100);
                    }
                }
            }
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Rapor Kurumu ve Ekonomik kod bilgilerini kullanarak Tertip bilgisini oluşturan,
        /// Daha sonra sunucu tarafındaki Ayrıntılı program bilgisini kullanıcı, yıl, aşama ve tertip bilgisini kullanarak okuyan,
        /// Ayrıntılı program bilgilerine göre Detay Rakamları oluşturan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisin tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="raporKurum">Rapor kurumu bilgisini tutan, String türünde nesne</param>
        /// <param name="ekoKod">Ekonomik kod bilgisini tutan, String türünde nesne</param>
        /// <param name="detayRakamlar">Detay rakamlarını tutan, Double dizi türünde nesne</param>
        private void DetayRakamBul(Kullanici kullanan, int yil, int asama, string raporKurum, string ekoKod, double[] detayRakamlar)
        {
            Tertip t = new Tertip(yil, ENUMTabloKod.TABButce, raporKurum, "", "", ekoKod);
            Object o = servisBUM.APOku(kullanan, yil, asama, t);
            for (int i = 0; i < 12; i++)
            {
                detayRakamlar[i] = 0;
            }
            if (o.GetType() == typeof(TertipBilgi))
            {
                TertipBilgi tb = (TertipBilgi)o;
                for (int i = 0; i < 12; i++)
                {
                    detayRakamlar[i] = tb.ayDegerler[i];
                }
            }
        }

        /// <summary>
        /// Rapor Yıl bilgisini kullanarak sunucu tarafındaki yordamı çalıştırarak Kod listesini getiren,
        /// Tablo oluşturarak, hücre bilgilerine Rapor parametrelerini (Yıl, Kurum Adı, Ünvan, Ad Soyad)
        /// Yıl ve Kurum bilgisini kullanarak tertip bilgisini oluşturan,
        /// oluşturduğu tertip bilgisini kullanarak sunucu tarafındaki yordamı çalıştırarak İcmal Harcama Programı bilgilerini listeleyen,
        /// Bilgilere göre Satır toplamı ve detayları bularak Tablodaki hücrelere yazan ve rapor dosyasını kaydeden fonksiyon
        /// </summary>
        /// <returns>Fonksiyonun çalışması sonucunda, Rapor adını tutan String türünde nesne döndürür</returns>
        public string AHPIHPKiyasla()
        {
            int[] indisaylar = new int[12] { 0, 2, 4, 8, 10, 12, 18, 20, 22, 26, 28, 30 };
            double[] birinciSeviyeToplam = new double[12];
            double[] birinciSeviyeDetayToplam = new double[12];
            double[] genelToplam = new double[12];
            double[] genelDetayToplam = new double[12];
            double[] detayRakamlar = new double[12];
            double toplamKBO, toplamSeviye1KBO;


            if (raporKurum != "" && raporYil > 0 && raporAsama > 0)
            {
                ABSKodlarVAN akv = servisBKD.KodListele(Convert.ToInt16(raporYil), "4", "");

                if (akv.sonuc.islemSonuc)
                {
                    Tablo XLS = GenelIslemler.NewTablo();
                    int satir = 0;
                    int sutun = 0;

                    XLS.DosyaAc(raporSablon, raporSonucDosyaAd + "." + GenelIslemler.ExcelTur());
                    XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

                    int kaynakSatir = satir;

                    XLS.HucreAdBulYaz("ButceYil", raporYil);
                    XLS.HucreAdBulYaz("KurumAd", raporKurumAd);
                    XLS.HucreAdBulYaz("AdSoyadSol", AdSoyadSol);
                    XLS.HucreAdBulYaz("UnvanSol", UnvanSol);
                    XLS.HucreAdBulYaz("AdSoyadSag", AdSoyadSag);
                    XLS.HucreAdBulYaz("UnvanSag", UnvanSag);
                    toplamKBO = 0;
                    for (int i = 0; i < akv.absKodlar.Count; i++)
                    {
                        for (int j = 0; j < birinciSeviyeToplam.Length; j++)
                        {
                            birinciSeviyeToplam[j] = 0;
                            birinciSeviyeDetayToplam[j] = 0;
                        }
                        ABSKod ak = (ABSKod)akv.absKodlar[i];


                        int ilkKirilimTut = -1;

                        int sonKirilim = 1;
                        ObjectArray degerler;
                        Tertip tertip = new Tertip(raporYil, ENUMTabloKod.TABGiderIcmal, raporKurum, ak.kod);
                        degerler = servisBUM.IHPListele(kullanan, Convert.ToInt16(raporYil), Convert.ToInt16(raporAsama), tertip);
                        if (degerler.sonuc.islemSonuc)
                        {
                            int ilkKirilim = 0;
                            toplamSeviye1KBO = 0;
                            foreach (TertipBilgi tb in degerler.objeler)
                            {
                                if (ilkKirilim == 0)
                                {
                                    ilkKirilim = 1;
                                    satir++;
                                    XLS.SatirAc(satir, 1);
                                    XLS.HucreKopyala(kaynakSatir - 2, sutun, kaynakSatir - 2, sutun + 40, satir, sutun);
                                    XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(450));
                                    satir++;
                                    XLS.SatirAc(satir, 1);
                                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 40, satir, sutun);
                                    XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(450));

                                    XLS.HucreDegerYaz(satir - 1, sutun, "IHP");
                                    XLS.HucreDegerYaz(satir, sutun, "AHP");

                                    XLS.HucreBirlestir(satir - 1, sutun + 1, satir, sutun + 1);
                                    XLS.HucreBirlestir(satir - 1, sutun + 2, satir, sutun + 2);
                                    XLS.HucreBirlestir(satir - 1, sutun + 3, satir, sutun + 3);

                                    XLS.HucreDegerYaz(satir - 1, sutun + 1, ak.kod);
                                    XLS.HucreDegerYaz(satir - 1, sutun + 2, ak.Ad());

                                    ilkKirilimTut = satir;

                                    satir++;
                                    XLS.SatirAc(satir, 1);
                                    XLS.HucreKopyala(kaynakSatir - 1, sutun, kaynakSatir - 1, sutun + 40, satir, sutun);
                                    XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(450));
                                    satir++;
                                    XLS.SatirAc(satir, 1);
                                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 40, satir, sutun);
                                    XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(450));
                                }
                                else
                                {
                                    satir++;
                                    XLS.SatirAc(satir, 1);
                                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 40, satir, sutun);
                                    XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(450));
                                    satir++;
                                    XLS.SatirAc(satir, 1);
                                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 40, satir, sutun);
                                    XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(450));
                                }

                                XLS.HucreDegerYaz(satir - 1, sutun, "IHP");
                                XLS.HucreDegerYaz(satir, sutun, "AHP");

                                XLS.HucreBirlestir(satir - 1, sutun + 1, satir, sutun + 1);
                                XLS.HucreBirlestir(satir - 1, sutun + 2, satir, sutun + 2);
                                XLS.HucreBirlestir(satir - 1, sutun + 3, satir, sutun + 3);

                                XLS.HucreDegerYaz(satir - 1, sutun + 1, tb.tertip.kodlar[sonKirilim]);
                                XLS.HucreDegerYaz(satir - 1, sutun + 2, tb.tertip.adlar[sonKirilim]);
                                XLS.HucreDegerYaz(satir - 1, sutun + 3, tb.kbo);

                                toplamSeviye1KBO += tb.kbo;
                                SatirYazToplamli(XLS, satir - 1, indisaylar, tb.ayDegerler, tb.kbo, birinciSeviyeToplam);
                                DetayRakamBul(kullanan, Convert.ToInt16(raporYil), Convert.ToInt16(raporAsama), raporKurum, tb.tertip.kodlar[sonKirilim], detayRakamlar);
                                SatirYazToplamli(XLS, satir, indisaylar, detayRakamlar, tb.kbo, birinciSeviyeDetayToplam);
                            }

                            if (ilkKirilimTut != -1)
                            {
                                XLS.HucreDegerYaz(ilkKirilimTut - 1, sutun + 3, toplamSeviye1KBO);
                                toplamKBO += toplamSeviye1KBO;
                                SatirYazToplamli(XLS, ilkKirilimTut - 1, indisaylar, birinciSeviyeToplam, toplamSeviye1KBO, genelToplam);
                                SatirYazToplamli(XLS, ilkKirilimTut, indisaylar, birinciSeviyeDetayToplam, toplamSeviye1KBO, genelDetayToplam); //Değiş tonton
                                ilkKirilimTut = -1;
                            }
                        }
                        else
                            throw new Exception(degerler.sonuc.hataStr);
                    }

                    satir++;
                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir - 2, sutun, kaynakSatir - 2, sutun + 40, satir, sutun);
                    XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(450));

                    satir++;
                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 40, satir, sutun);
                    XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(450));

                    XLS.HucreDegerYaz(satir - 1, sutun, "IHP");
                    XLS.HucreDegerYaz(satir, sutun, "AHP");

                    XLS.HucreBirlestir(satir - 1, sutun + 1, satir, sutun + 2);
                    XLS.HucreDegerYaz(satir - 1, sutun + 1, "TOPLAM BÜTÇE GİDERİ");

                    XLS.HucreBirlestir(satir - 1, sutun + 3, satir, sutun + 3);
                    XLS.HucreDegerYaz(satir - 1, sutun + 3, toplamKBO);
                    SatirYazToplamli(XLS, satir - 1, indisaylar, genelToplam, toplamKBO, null);
                    SatirYazToplamli(XLS, satir, indisaylar, genelDetayToplam, toplamKBO, null);

                    XLS.DosyaSaklaTamYol();
                    return raporSonucDosyaAd + "." + GenelIslemler.ExcelTur();
                }
            }
            else
                throw new Exception("Yıl veya Kurum alanlarından biri veya bir kaçı boş bırakılmış. Lütfen ilgili alanları kontrol edip tekrar deneyin.");
            return "";
        }

        /// <summary>
        /// Rapor Kurum, Yıl ve Aşama bilgilerini kontrol eden,
        /// Eğer kontrolden başarıyla geçilmişse Tablo nesnesi oluşturan,
        /// Sunucu tarafındaki yordamı kullanarak kod listesini getiren,
        /// Tablodaki hücrelere raporlama parametrelerini yazan (Yıl, Kurum, Ad Soyad, Ünvan v.b.)
        /// Tertip bilgilerini oluşturan,
        /// eğer Ek Bilgi değişkeni Detay ise, sunucu tarafındaki yordamı çalıştırarak Ayrıntılı Program bilgilerini getiren,
        /// değilse sunucu tarafındaki yordamı çalıştırarak İcmal Harcama Programı bilgilerini getiren,
        /// getirmiş olduğu bilgileri kullanarak Ek1B raporunu oluşturan,
        /// oluşturduğu rapor dosyasını kaydederek saklayan ve rapor dosya adını döndüren fonksiyon
        /// </summary>
        /// <returns>Fonksiyonun çalışması sonucunda, Rapor dosya adını tutan String türünde nesne döndürür</returns>
        public string Ek1B()
        {
            int[] indisaylar = new int[12] { 0, 2, 4, 8, 10, 12, 18, 20, 22, 26, 28, 30 };
            double[] birinciSeviyeToplam = new double[14];
            double[] genelToplam = new double[14];
            double toplamKBO, toplamSeviye1KBO;


            if (!(raporKurum != "" && raporYil > 0 && raporAsama > 0))
                throw new Exception("Yıl veya Kurum alanlarından biri veya bir kaçı boş bırakılmış. Lütfen ilgili alanları kontrol edip tekrar deneyin.");

            ABSKodlarVAN akv = servisBKD.KodListele(Convert.ToInt16(raporYil), "4", "");

            if (!akv.sonuc.islemSonuc)
                throw new Exception(akv.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;

            XLS.DosyaAc(raporSablon, raporSonucDosyaAd + "." + GenelIslemler.ExcelTur());
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

            int kaynakSatir = satir;

            XLS.HucreAdBulYaz("ButceYil", raporYil);
            XLS.HucreAdBulYaz("KurumAd", raporKurumAd);
            XLS.HucreAdBulYaz("AdSoyadSol", AdSoyadSol);
            XLS.HucreAdBulYaz("UnvanSol", UnvanSol);
            XLS.HucreAdBulYaz("AdSoyadSag", AdSoyadSag);
            XLS.HucreAdBulYaz("UnvanSag", UnvanSag);
            toplamKBO = 0;
            for (int i = 0; i < akv.absKodlar.Count; i++)
            {
                for (int j = 0; j < birinciSeviyeToplam.Length; j++)
                    birinciSeviyeToplam[j] = 0;
                ABSKod ak = (ABSKod)akv.absKodlar[i];

                int ilkKirilimTut = -1;

                int sonKirilim = 1;
                TertipOdenek[] degerler = null;
                if (raporEkBilgi.Contains("DETAY"))
                {
                    Tertip tertip = new Tertip(raporYil, ENUMTabloKod.TABPEBOdenek, raporKurum, "", "", ak.kod, "");
                    UcgenKodBilgileri ukb = new UcgenKodBilgileri();
                    ukb.Add(new UcgenKodBilgi(4, 2, TNS.BKD.Arac.Eko2_3Bul(kullanan)));
                    degerler = servisBUM.APListeleKodBilgi(kullanan, raporAsama, tertip, ukb);
                    sonKirilim = 3;
                }
                else
                {
                    Tertip tertip = new Tertip(raporYil, ENUMTabloKod.TABGiderIcmal, raporKurum, ak.kod);
                    //degerler = servisBUM.IHPListele(kullanan, Convert.ToInt16(raporYil), Convert.ToInt16(raporAsama), tertip);
                }

                int ilkKirilim = 0;
                toplamSeviye1KBO = 0;
                foreach (TertipOdenek tb in degerler)
                {
                    if (ilkKirilim == 0)
                    {
                        ilkKirilim = 1;
                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir - 2, sutun, kaynakSatir - 2, sutun + 40, satir, sutun);
                        XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(450));
                        XLS.HucreDegerYaz(satir, sutun, ak.kod);
                        XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 2);
                        XLS.HucreDegerYaz(satir, sutun + 1, ak.Ad());
                        ilkKirilimTut = satir;

                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir - 1, sutun, kaynakSatir - 1, sutun + 40, satir, sutun);
                        XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(450));
                    }
                    else
                    {
                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 40, satir, sutun);
                        XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(450));
                    }

                    XLS.HucreDegerYaz(satir, sutun + 1, tb.tertip.kodlar[3].Substring(3, 1));
                    XLS.HucreDegerYaz(satir, sutun + 2, tb.tertip.adlar[3]);
                    XLS.HucreDegerYaz(satir, sutun + 3, tb.odenek[0]);
                    toplamSeviye1KBO += tb.odenek[0];
                    SatirYazToplamli(XLS, satir, indisaylar, tb.odenek, tb.odenek[0], birinciSeviyeToplam);
                }

                if (ilkKirilimTut != -1)
                {
                    XLS.HucreDegerYaz(ilkKirilimTut, sutun + 3, toplamSeviye1KBO);
                    toplamKBO += toplamSeviye1KBO;
                    SatirYazToplamli(XLS, ilkKirilimTut, indisaylar, birinciSeviyeToplam, toplamSeviye1KBO, genelToplam);
                    ilkKirilimTut = -1;
                }
            }

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir - 2, sutun, kaynakSatir - 2, sutun + 40, satir, sutun);
            XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(450));

            XLS.HucreBirlestir(satir, sutun, satir, sutun + 2);
            XLS.HucreDegerYaz(satir, sutun, "TOPLAM BÜTÇE GİDERİ");

            XLS.HucreDegerYaz(satir, sutun + 3, toplamKBO);
            SatirYazToplamli(XLS, satir, indisaylar, genelToplam, toplamKBO, null);

            XLS.DosyaSaklaTamYol();
            return raporSonucDosyaAd + "." + GenelIslemler.ExcelTur();
        }
        /// <summary>
        /// Rapor parametrelerini alır (Bütçe Yılı, Aşama, Kurum Kodu, Birim, Ünvan, Ad, Soyad v.b.), ek bilgiye göre şablonu belirler ve
        /// ilgili raporu (Ek1A, Ek1B, Ek2 v.b.) üretir. Üretilen Excel dosyasının tam yolunu döndürür.
        /// </summary>
        /// <param name="uygulamaKlasoru">IWebHostEnvironment.ContentRootPath (uygulamayla gelen şablonlar için).</param>
        public string RaporAl(Kullanici p_kullanan, string uygulamaKlasoru, string sonucDosyaAd, Hashtable raporParam, string ekBilgi)
        {
            servisBKD = TNS.BKD.Arac.Tanimla();
            servisBUM = TNS.BUM.Arac.Tanimla();
            raporYil = Convert.ToInt16((string)raporParam["ButceYil"]);
            raporAsama = Convert.ToInt16((string)raporParam["Asama"]);
            raporKurum = (string)raporParam["KurKod"];
            eko = (string)raporParam["eko"];
            peb = (string)raporParam["peb"];
            raporKurumAd = (string)raporParam["KurAd"];
            raporBirim = (string)raporParam["Birim"];
            raporBirimAd = (string)raporParam["BirimAd"];
            raporEkBilgi = ekBilgi;
            AdSoyadSol = (string)raporParam["AdSoyadSol"];
            UnvanSol = (string)raporParam["UnvanSol"];
            AdSoyadSag = (string)raporParam["AdSoyadSag"];
            UnvanSag = (string)raporParam["UnvanSag"];
            tertipTam = raporParam["TertipTam"];
            tabloKod = (ENUMTabloKod)OrtakFonksiyonlar.ConvertToInt(raporParam["TabloKod"], 1);

            string raporFormat = (string)raporParam["Format"];
            bool eko2 = TNS.MUH.Arac.OdenekGondermeEko2liMi(raporYil);
            string uzanti2 = "";

            if ((ekBilgi.Equals("2") || ekBilgi.Equals("2_DETAY")) && eko2)
            {
                uzanti2 = "-E2";
            }

            raporSonucDosyaAd = sonucDosyaAd + "_" + DateTime.Now.Ticks.ToString();
            kullanan = p_kullanan;
            string uzanti;
            if (raporFormat.Equals("1"))
                uzanti = "Imza" + uzanti2 + ".XLT";
            else
                uzanti = uzanti2 + ".XLT";

            string sablonAd;
            if (ekBilgi.Equals("1A") || ekBilgi.Equals("1A_DETAY"))
                sablonAd = "AP-Ek1a" + uzanti;
            else if (ekBilgi.Equals("1B") || ekBilgi.Equals("1B_DETAY"))
                sablonAd = "AP-Ek1b" + uzanti;
            else if (ekBilgi.Equals("2") || ekBilgi.Equals("2_DETAY"))
                sablonAd = "AP-Ek2" + uzanti;
            else if (ekBilgi.Equals("3B"))
                sablonAd = "AP-Ek3b" + uzanti;
            else if (ekBilgi.Equals("3C"))
                sablonAd = "AP-Ek3c" + uzanti;
            else if (ekBilgi.Equals("KIYAS"))
                sablonAd = "AP-AHPIHPDetay" + ".XLT";
            else
                throw new Exception("Ayrıntılı harcama için hatalı rapor kodu geldi!");

            raporSablon = SablonBul(uygulamaKlasoru, sablonAd);
            if (raporSablon == null) throw new Exception("Rapor şablonu bulunamadı (BUM\\" + sablonAd + ").");

            if (ekBilgi.Equals("1A") || ekBilgi.Equals("1A_DETAY"))
                return Ek1A();
            else if (ekBilgi.Equals("1B") || ekBilgi.Equals("1B_DETAY"))
                return Ek1B();
            else if (ekBilgi.Equals("2") || ekBilgi.Equals("2_DETAY"))
                return Ek2(eko2);
            else if (ekBilgi.Equals("3B"))
                return Ek1B();
            else if (ekBilgi.Equals("3C"))
                return Ek3C();
            else
                return AHPIHPKiyasla();
        }
    }
}
