// Eski Istemci\ButceMuhasebe\MizanBilanco.cs dosyasından alındı (Muhasebe Raporları: mizan, muavin, yevmiye, kesin hesap ...).
// İsim alanı eskisi gibi ButceMuhasebe. Değişenler: sınıflardan TNS.Raporlama.IRapor arayüzü kaldırıldı (Core'da yok;
// Muhasebe/MizanBilancoUclari.cs RaporAl'ı yansıma ile çağırır); Resources.ButceMuhasebe MUHGNL186-189 metinleri yerine yazıldı;
// eski ButceMuhasebe\Genel.cs'teki DondurParaBirimi / ParaBirimiDondur dosyanın sonundaki ButceMuhasebeGenel'e alındı.
// Rapor kodu eskisinin aynısıdır.
using System;
using System.Collections.Generic;
using System.Text;
using OrtakClass;
using TNS;
using TNS.MUH;
using TNS.UZY;
using System.Collections;
using TNS.DEG;
using TNS.BKD;
using TNS.BHMv2;

namespace ButceMuhasebe
{
    /// <summary>
    /// Mizan rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class Mizan
    {
        OrtakClass.Tablo XLS;
        int kaynakSatirKK, kaynakSatirAK, kaynakSatirAA, kaynakSatirKA;
        double stBorc, stAlacak, stBakiyeBorc, stBakiyeAlacak;
        int sayfaNo;
        bool yeniSayfa;
        int aktifSatir;
        int seviye;
        int sutun, SayfaSatirSay, yazilanSatir = 0;

        private void ToplamYaz(bool devredenMi, int devirKolon)
        {
            yazilanSatir = 0;
            //double stKalanBorc, stKalanAlacak;
            aktifSatir++;
            XLS.HucreKopyala(kaynakSatirKK, sutun, kaynakSatirKK, sutun + 12 + devirKolon, aktifSatir, sutun);
            if (devredenMi)
            {
                XLS.HucreDegerYaz(aktifSatir, sutun + 0, "D E V R E D E N :");
                sayfaNo++;
            }
            else
                XLS.HucreDegerYaz(aktifSatir, sutun + 0, "T O P L A M :");

            XLS.HucreBirlestir(aktifSatir, sutun + 0, aktifSatir, sutun + 2);
            XLS.HucreDegerYaz(aktifSatir, sutun + 3 + devirKolon, stBorc);
            XLS.HucreDegerYaz(aktifSatir, sutun + 4 + devirKolon, stAlacak);

            if (stBakiyeBorc == 0 && stBakiyeAlacak == 0)
            {
                double kalan = stBorc - stAlacak;
                if (kalan > 0) stBakiyeBorc = kalan;
                if (kalan < 0) stBakiyeAlacak = Math.Abs(kalan);
            }

            XLS.HucreDegerYaz(aktifSatir, sutun + 5 + devirKolon, stBakiyeBorc);
            XLS.HucreDegerYaz(aktifSatir, sutun + 6 + devirKolon, stBakiyeAlacak);

            //HesapBilgi.KalanHesapla(stBorc, stAlacak, out stKalanBorc, out stKalanAlacak);
            //double bakiye = stBakiyeBorc - stBakiyeAlacak;
            //if (bakiye > 0)
            //{
            //    XLS.HucreDegerYaz(aktifSatir, sutun + 5, bakiye);
            //    XLS.HucreDegerYaz(aktifSatir, sutun + 6, 0);
            //}
            //else
            //{
            //    XLS.HucreDegerYaz(aktifSatir, sutun + 5, 0);
            //    XLS.HucreDegerYaz(aktifSatir, sutun + 6, Math.Abs(bakiye));
            //}

            if (devredenMi)
                yeniSayfa = false;
            else
            {
                yeniSayfa = true;
                XLS.SayfaSonuKoy(aktifSatir + 1);
            }
        }

        private void SatirAc(bool anaKayitMi, int devirKolon)
        {
            int kaynakSatir;
            if (yazilanSatir > SayfaSatirSay)
            {
                ToplamYaz(false, devirKolon);
            }

            if (yeniSayfa)
            {
                ToplamYaz(true, devirKolon);
                if (anaKayitMi && seviye > 1)
                    kaynakSatir = kaynakSatirKK;
                else
                    kaynakSatir = kaynakSatirKA;
            }
            else if (anaKayitMi && seviye > 1)
                kaynakSatir = kaynakSatirKK;
            else
                kaynakSatir = kaynakSatirAA;
            aktifSatir++;
            yazilanSatir++;
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, aktifSatir, sutun);
        }

        /// <summary>
        /// Mizan raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);
            seviye = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Seviye"], 0);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            string birimAd = "";
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int listeTur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["ListeTur"], 0);
            int raporTur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["RaporTur"], 0);
            int bakiyeSifir = TNS.OrtakFonksiyonlar.ConvertToInt((bool)raporParam["BakiyeSifirGosterme"], 0);
            string altHesap = "";

            if (raporTur == 47)
            {
                altHesap = (string)raporParam["AltHesap"];
                seviye = 9;
            }

            string basHesap = (string)raporParam["BasHesap"];
            string bitHesap = (string)raporParam["BitHesap"];
            string saymanlikAd = (string)raporParam["SaymanlikAd"] + "";
            SayfaSatirSay = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["SayfaSatirSay"], 65);
            if (SayfaSatirSay < 30)
                throw new Exception("Sayfadaki satır sayısı 30'dan az olamaz!");

            string ayAralik = tarih1.ToString() + " - " + tarih2.ToString();
            MizanBilgi mb = s.Mizan((TNS.KYM.Kullanici)kullanan, yil, kurum, saymanlik, birim, tarih1, tarih2, seviye, basHesap, bitHesap, tur, fisTur, listeTur, altHesap);

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);
            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdBulYaz("Yil", yil.ToString());
            XLS.HucreAdBulYaz("KurumAd", kurum + " - " + kurumAd);
            if (birim.Trim() != "")
                birimAd = " - (" + birim + ") " + OrtakClass.GenelIslemler.KodAd(12, birim + "-" + saymanlik.ToString(), false);

            if (raporTur == 44)
                XLS.HucreAdBulYaz("mizanAdi", "GEÇİCİ MİZAN");
            if (raporTur == 45)
                XLS.HucreAdBulYaz("mizanAdi", "KESİN MİZAN");
            if (raporTur == 47)
                XLS.HucreAdBulYaz("mizanAdi", "MİZAN (ALT HESAPLAR)");

            XLS.HucreAdBulYaz("SaymanlikAd", saymanlikAd + birimAd);
            XLS.HucreAdBulYaz("AyAralik", ayAralik);

            string paraBirimi = ButceMuhasebeGenel.DondurParaBirimi(1).kisaltma;
            if (listeTur == 2) paraBirimi = "DÖVİZ";
            XLS.HucreAdBulYaz("ParaBirimi", paraBirimi);

            int satir = 0;
            sutun = 0;
            XLS.HucreAdAdresCoz("BaslaHucre", ref satir, ref sutun);
            kaynakSatirKK = satir - 1;
            kaynakSatirAK = satir - 2;
            kaynakSatirAA = satir - 3;
            kaynakSatirKA = satir - 4;

            stBorc = mb.oncBorc;
            stAlacak = mb.oncAlacak;
            sayfaNo = 0;
            yeniSayfa = true;
            aktifSatir = satir;

            int devirKolon = 0;
            if (raporTur == 64)//Devreden
            {
                devirKolon = 1;
                stBorc = 0;
                stAlacak = 0;
            }

            double devredenMizanBakiye = 0;

            int i;
            for (i = 0; i < mb.anaHesaplar.objeler.Count; i++)
            {
                AnaHesap ah = (AnaHesap)mb.anaHesaplar[i];

                if (bakiyeSifir > 0)
                {
                    if (ah.kalanBorcToplam == 0 && ah.kalanAlacakToplam == 0)
                        continue;
                }

                SatirAc(true, devirKolon);

                if (seviye == 1)
                {
                    stBorc += ah.borcToplam;
                    stAlacak += ah.alacakToplam;
                }

                XLS.HucreDegerYaz(aktifSatir, sutun, ah.hesapKod);
                XLS.HucreDegerYaz(aktifSatir, sutun + 2, ah.hesapAd);

                if (devirKolon == 1)
                {
                    TNSDateTime tTarih = new TNSDateTime(tarih1);
                    tTarih.AddDays(-1);
                    MizanBilgi mt = s.MizanToplamBul((TNS.KYM.Kullanici)kullanan, yil, kurum, saymanlik, birim, new TNSDateTime("01.01." + tarih1.Yil), tTarih, seviye, ah.hesapKod, ah.hesapKod, tur, fisTur, listeTur, altHesap);
                    devredenMizanBakiye = mt.oncBorc - mt.oncAlacak;
                    XLS.HucreDegerYaz(aktifSatir, sutun + 3, devredenMizanBakiye);
                    double b = ah.borcToplam;
                    double a = ah.alacakToplam;
                    b += devredenMizanBakiye;
                    ah.kalanBorcToplam = 0;
                    ah.kalanAlacakToplam = 0;
                    if (b - a > 0) ah.kalanBorcToplam = b - a;
                    else ah.kalanAlacakToplam = Math.Abs(b - a);
                }

                XLS.HucreDegerYaz(aktifSatir, sutun + 3 + devirKolon, ah.borcToplam);
                XLS.HucreDegerYaz(aktifSatir, sutun + 4 + devirKolon, ah.alacakToplam);
                XLS.HucreDegerYaz(aktifSatir, sutun + 5 + devirKolon, ah.kalanBorcToplam);
                XLS.HucreDegerYaz(aktifSatir, sutun + 6 + devirKolon, ah.kalanAlacakToplam);

                stBakiyeBorc += ah.kalanBorcToplam;
                stBakiyeAlacak += ah.kalanAlacakToplam;
                for (int j = 0; j < ah.detayHesaplar.objeler.Count; j++)
                {
                    HesapBilgi alth = (HesapBilgi)ah.detayHesaplar[j];

                    if (bakiyeSifir > 0)
                    {
                        if (alth.kalanBorc == 0 && alth.kalanAlacak == 0)
                            continue;
                    }

                    SatirAc(false, devirKolon);
                    stBorc += alth.borc;
                    stAlacak += alth.alacak;

                    if (alth.altHesapKod != "")
                        alth.hesapAd = alth.altHesapKod + " / " + alth.hesapAd;

                    XLS.HucreDegerYaz(aktifSatir, sutun + 1, alth.hesapKod);
                    XLS.HucreDegerYaz(aktifSatir, sutun + 2, alth.hesapAd);

                    if (devirKolon == 1)
                    {
                        TNSDateTime tTarih = new TNSDateTime(tarih1);
                        tTarih.AddDays(-1);
                        MizanBilgi mt = s.MizanToplamBul((TNS.KYM.Kullanici)kullanan, yil, kurum, saymanlik, birim, new TNSDateTime("01.01." + tarih1.Yil), tTarih, seviye, alth.hesapKod, alth.hesapKod, tur, fisTur, listeTur, altHesap);
                        devredenMizanBakiye = mt.oncBorc - mt.oncAlacak;
                        XLS.HucreDegerYaz(aktifSatir, sutun + 3, devredenMizanBakiye);
                        double b = alth.borc;
                        double a = alth.alacak;
                        b += devredenMizanBakiye;
                        alth.kalanBorc = 0;
                        alth.kalanAlacak = 0;
                        if (b - a > 0) alth.kalanBorc = b - a;
                        else alth.kalanAlacak = Math.Abs(b - a);
                    }

                    XLS.HucreDegerYaz(aktifSatir, sutun + 3 + devirKolon, alth.borc);
                    XLS.HucreDegerYaz(aktifSatir, sutun + 4 + devirKolon, alth.alacak);
                    XLS.HucreDegerYaz(aktifSatir, sutun + 5 + devirKolon, alth.kalanBorc);
                    XLS.HucreDegerYaz(aktifSatir, sutun + 6 + devirKolon, alth.kalanAlacak);
                }
            }
            if (yazilanSatir > 0)
            {
                ToplamYaz(false, devirKolon);
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;

        }
    }

    /// <summary>
    /// Mizan birim rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class MizanBirim
    {
        OrtakClass.Tablo XLS;
        int kaynakSatirKK, kaynakSatirAK, kaynakSatirAA, kaynakSatirKA;
        double stBorc, stAlacak;//, stBakiyeBorc, stBakiyeAlacak;
        int sayfaNo;
        bool yeniSayfa;
        int aktifSatir;
        int seviye;
        int sutun, SayfaSatirSay, yazilanSatir = 0;

        private void ToplamYaz(bool devredenMi)
        {
            yazilanSatir = 0;
            //double stKalanBorc, stKalanAlacak;
            aktifSatir++;
            XLS.HucreKopyala(kaynakSatirKK, sutun, kaynakSatirKK, sutun + 12, aktifSatir, sutun);
            if (devredenMi)
            {
                XLS.HucreDegerYaz(aktifSatir, sutun + 0, "D E V R E D E N :");
                sayfaNo++;
            }
            else
                XLS.HucreDegerYaz(aktifSatir, sutun + 0, "T O P L A M :");

            XLS.HucreBirlestir(aktifSatir, sutun + 0, aktifSatir, sutun + 2);
            XLS.HucreDegerYaz(aktifSatir, sutun + 4, stBorc);
            XLS.HucreDegerYaz(aktifSatir, sutun + 5, stAlacak);

            //HesapBilgi.KalanHesapla(stBorc, stAlacak, out stKalanBorc, out stKalanAlacak);

            double bakiye = stBorc - stAlacak;
            if (bakiye > 0)
            {
                XLS.HucreDegerYaz(aktifSatir, sutun + 6, bakiye);
                XLS.HucreDegerYaz(aktifSatir, sutun + 7, 0);
            }
            else
            {
                XLS.HucreDegerYaz(aktifSatir, sutun + 6, 0);
                XLS.HucreDegerYaz(aktifSatir, sutun + 7, Math.Abs(bakiye));
            }

            if (devredenMi)
                yeniSayfa = false;
            else
            {
                yeniSayfa = true;
                XLS.SayfaSonuKoy(aktifSatir + 1);
            }
        }

        private void SatirAc(bool anaKayitMi)
        {
            int kaynakSatir;
            if (yazilanSatir > SayfaSatirSay)
            {
                ToplamYaz(false);
            }

            if (yeniSayfa)
            {
                ToplamYaz(true);
                if (anaKayitMi && seviye > 1)
                    kaynakSatir = kaynakSatirKK;
                else
                    kaynakSatir = kaynakSatirKA;
            }
            else if (anaKayitMi && seviye > 1)
                kaynakSatir = kaynakSatirKK;
            else
                kaynakSatir = kaynakSatirAA;
            aktifSatir++;
            yazilanSatir++;
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 12, aktifSatir, sutun);
        }

        /// <summary>
        /// Mizan birim raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);
            seviye = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Seviye"], 0);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            string birimAd = "";
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int listeTur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["ListeTur"], 0);
            string kurKod = (string)raporParam["KurKod"];
            string fonKod = (string)raporParam["FonKod"];

            string basHesap = (string)raporParam["BasHesap"];
            string bitHesap = (string)raporParam["BitHesap"];
            string saymanlikAd = (string)raporParam["SaymanlikAd"] + "";
            int bakiyeSifir = TNS.OrtakFonksiyonlar.ConvertToInt((bool)raporParam["BakiyeSifirGosterme"], 0);
            string altHesap = (string)raporParam["AltHesap"];

            SayfaSatirSay = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["SayfaSatirSay"], 65);
            if (SayfaSatirSay < 30)
                throw new Exception("Sayfadaki satır sayısı 30'dan az olamaz!");

            string ayAralik = tarih1.ToString() + " - " + tarih2.ToString();
            MizanBilgi mb = s.MizanBirim((TNS.KYM.Kullanici)kullanan, yil, kurum, saymanlik, birim, tarih1, tarih2, seviye, basHesap, bitHesap, tur, fisTur, listeTur, kurKod, fonKod, altHesap);

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);
            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdBulYaz("Yil", yil.ToString());
            XLS.HucreAdBulYaz("KurumAd", kurum + " - " + kurumAd);
            if (birim.Trim() != "")
                birimAd = " - (" + birim + ") " + OrtakClass.GenelIslemler.KodAd(12, birim + "-" + saymanlik.ToString(), false);
            XLS.HucreAdBulYaz("SaymanlikAd", saymanlikAd + birimAd);
            XLS.HucreAdBulYaz("AyAralik", ayAralik);

            string paraBirimi = ButceMuhasebeGenel.DondurParaBirimi(1).kisaltma;
            if (listeTur == 2) paraBirimi = "DÖVİZ";
            XLS.HucreAdBulYaz("ParaBirimi", paraBirimi);

            int satir = 0;
            sutun = 0;
            XLS.HucreAdAdresCoz("BaslaHucre", ref satir, ref sutun);
            kaynakSatirKK = satir - 1;
            kaynakSatirAK = satir - 2;
            kaynakSatirAA = satir - 3;
            kaynakSatirKA = satir - 4;

            stBorc = mb.oncBorc;
            stAlacak = mb.oncAlacak;
            sayfaNo = 0;
            yeniSayfa = true;
            aktifSatir = satir;

            int i;
            for (i = 0; i < mb.anaHesaplar.objeler.Count; i++)
            {
                AnaHesap ah = (AnaHesap)mb.anaHesaplar[i];

                if (bakiyeSifir > 0)
                {
                    if (ah.kalanBorcToplam == 0 && ah.kalanAlacakToplam == 0)
                        continue;
                }

                SatirAc(true);
                if (seviye == 1)
                {
                    stBorc += ah.borcToplam;
                    stAlacak += ah.alacakToplam;
                }
                XLS.HucreDegerYaz(aktifSatir, sutun, ah.hesapKod);
                XLS.HucreDegerYaz(aktifSatir, sutun + 2, ah.hesapAd);
                XLS.HucreDegerYaz(aktifSatir, sutun + 3, ah.kur);
                XLS.HucreDegerYaz(aktifSatir, sutun + 4, ah.borcToplam);
                XLS.HucreDegerYaz(aktifSatir, sutun + 5, ah.alacakToplam);
                XLS.HucreDegerYaz(aktifSatir, sutun + 6, ah.kalanBorcToplam);
                XLS.HucreDegerYaz(aktifSatir, sutun + 7, ah.kalanAlacakToplam);
                //stBakiyeBorc += ah.kalanBorcToplam;
                //stBakiyeAlacak += ah.kalanAlacakToplam;

                for (int j = 0; j < ah.detayHesaplar.objeler.Count; j++)
                {
                    HesapBilgi alth = (HesapBilgi)ah.detayHesaplar[j];

                    if (bakiyeSifir > 0)
                    {
                        if (alth.kalanBorc == 0 && alth.kalanAlacak == 0)
                            continue;
                    }

                    SatirAc(false);
                    stBorc += alth.borc;
                    stAlacak += alth.alacak;

                    XLS.HucreDegerYaz(aktifSatir, sutun + 1, alth.hesapKod);
                    XLS.HucreDegerYaz(aktifSatir, sutun + 2, alth.hesapAd);
                    XLS.HucreDegerYaz(aktifSatir, sutun + 3, alth.kur);
                    XLS.HucreDegerYaz(aktifSatir, sutun + 4, alth.borc);
                    XLS.HucreDegerYaz(aktifSatir, sutun + 5, alth.alacak);
                    XLS.HucreDegerYaz(aktifSatir, sutun + 6, alth.kalanBorc);
                    XLS.HucreDegerYaz(aktifSatir, sutun + 7, alth.kalanAlacak);
                }
            }
            if (yazilanSatir > 0)
            {
                ToplamYaz(false);
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    /// <summary>
    /// Mizan tertip rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class MizanTertip
    {
        OrtakClass.Tablo XLS;
        int kaynakSatirKK, kaynakSatirAK, kaynakSatirAA, kaynakSatirKA;
        double stBorc, stAlacak;//, stBakiyeBorc, stBakiyeAlacak;
        int sayfaNo;
        bool yeniSayfa;
        int aktifSatir;
        int seviye;
        int sutun, SayfaSatirSay, yazilanSatir = 0;

        private void ToplamYaz(bool devredenMi)
        {
            yazilanSatir = 0;
            //double stKalanBorc, stKalanAlacak;
            aktifSatir++;
            XLS.HucreKopyala(kaynakSatirKK, 0, kaynakSatirKK, 24, aktifSatir, 0);
            if (devredenMi)
            {
                XLS.HucreDegerYaz(aktifSatir, sutun + 14, "D E V R E D E N :");
                sayfaNo++;
            }
            else
                XLS.HucreDegerYaz(aktifSatir, sutun + 14, "T O P L A M :");

            //XLS.HucreBirlestir(aktifSatir, sutun + 0, aktifSatir, sutun + 14);
            XLS.HucreDegerYaz(aktifSatir, sutun + 15, stBorc);
            XLS.HucreDegerYaz(aktifSatir, sutun + 16, stAlacak);

            //HesapBilgi.KalanHesapla(stBorc, stAlacak, out stKalanBorc, out stKalanAlacak);
            double bakiye = stBorc - stAlacak;
            if (bakiye > 0)
            {
                XLS.HucreDegerYaz(aktifSatir, sutun + 17, bakiye);
                XLS.HucreDegerYaz(aktifSatir, sutun + 18, 0);
            }
            else
            {
                XLS.HucreDegerYaz(aktifSatir, sutun + 17, 0);
                XLS.HucreDegerYaz(aktifSatir, sutun + 18, Math.Abs(bakiye));
            }

            if (devredenMi)
                yeniSayfa = false;
            else
            {
                yeniSayfa = true;
                XLS.SayfaSonuKoy(aktifSatir + 1);
            }
        }

        private void SatirAc(bool anaKayitMi)
        {
            int kaynakSatir;
            if (yazilanSatir > SayfaSatirSay)
            {
                //ToplamYaz(false);
            }

            if (yeniSayfa)
            {
                //ToplamYaz(true);
                if (anaKayitMi && seviye > 1)
                    kaynakSatir = kaynakSatirKK;
                else
                    kaynakSatir = kaynakSatirKA;
            }
            else if (anaKayitMi && seviye > 1)
                kaynakSatir = kaynakSatirKK;
            else
                kaynakSatir = kaynakSatirAA;
            aktifSatir++;
            yazilanSatir++;
            XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 24, aktifSatir, 0);
        }

        /// <summary>
        /// Mizan tertip raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);
            seviye = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Seviye"], 0);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            string birimAd = "";
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int listeTur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["ListeTur"], 0);
            int tasHesapTertipGosterme = TNS.OrtakFonksiyonlar.ConvertToInt((bool)raporParam["TasinirHesapTertipGosterme"], 0);
            string tasHesapTertipGostermeHesaplar = (string)raporParam["TasinirHesapTertipGostermeHesaplar"];

            string basHesap = (string)raporParam["BasHesap"];
            string bitHesap = (string)raporParam["BitHesap"];
            string saymanlikAd = (string)raporParam["SaymanlikAd"] + "";
            int bakiyeSifir = TNS.OrtakFonksiyonlar.ConvertToInt((bool)raporParam["BakiyeSifirGosterme"], 0);
            string altHesap = (string)raporParam["AltHesap"];
            int prgTertipGoster = TNS.OrtakFonksiyonlar.ConvertToInt((bool)raporParam["ProgramButceTertipGoster"], 0);

            Hashtable htPrgIliski = new Hashtable();
            if (prgTertipGoster > 0)
                htPrgIliski = TNS.BKD.Arac.ProgramButceIliskiHazirla((TNS.KYM.Kullanici)kullanan, yil);

            SayfaSatirSay = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["SayfaSatirSay"], 65);
            if (SayfaSatirSay < 30)
                throw new Exception("Sayfadaki satır sayısı 30'dan az olamaz!");

            string ayAralik = tarih1.ToString() + " - " + tarih2.ToString();
            MizanBilgi mb = s.MizanTertip((TNS.KYM.Kullanici)kullanan, yil, kurum, saymanlik, birim, tarih1, tarih2, seviye, basHesap, bitHesap, tur, fisTur, listeTur, altHesap, false, 0, 0);

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);
            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdBulYaz("Yil", yil.ToString());
            XLS.HucreAdBulYaz("KurumAd", kurum + " - " + kurumAd);
            if (birim.Trim() != "")
                birimAd = " - (" + birim + ") " + OrtakClass.GenelIslemler.KodAd(12, birim + "-" + saymanlik.ToString(), false);
            XLS.HucreAdBulYaz("SaymanlikAd", saymanlik + " - " + saymanlikAd);// + birimAd);
            XLS.HucreAdBulYaz("AyAralik", GenelIslemler.AyAdDondur(tarih2.Ay));

            string paraBirimi = ButceMuhasebeGenel.DondurParaBirimi(1).kisaltma;
            if (listeTur == 2) paraBirimi = "DÖVİZ";
            XLS.HucreAdBulYaz("ParaBirimi", paraBirimi);

            int satir = 0;
            sutun = 0;
            XLS.HucreAdAdresCoz("BaslaHucre", ref satir, ref sutun);
            kaynakSatirKK = satir - 1;
            kaynakSatirAK = satir - 2;
            kaynakSatirAA = satir - 3;
            kaynakSatirKA = satir - 4;

            stBorc = mb.oncBorc;
            stAlacak = mb.oncAlacak;
            sayfaNo = 0;
            yeniSayfa = true;
            aktifSatir = satir;

            if (tasHesapTertipGosterme > 0)
            {
                string[] tasHesaplar = tasHesapTertipGostermeHesaplar.Split(';', ',', ' ');
                //Bazı hesaplarda tertip gösterilmesi istenmiyor. Gelen bilgilerden tertipler kaldırılacak ve hesap bazında yeninden borç alacak bilgilerinin hesaplaması yapılacak
                for (int kk = 0; kk < mb.anaHesaplar.objeler.Count; kk++)
                {
                    AnaHesap ah = (AnaHesap)mb.anaHesaplar[kk];

                    bool bulundu = false;
                    foreach (string h in tasHesaplar)
                    {
                        if (ah.hesapKod == h.Trim() || h.Trim() == "*")
                        {
                            bulundu = true;
                            break;
                        }
                    }

                    if (!bulundu) continue;//Tertiplerin gösterilmeyeceği hesaplar için işlem yapılacak. Diğerleri için geç.
                    double borc = 0.0;
                    double alacak = 0.0;
                    double kalanBorc = 0.0;
                    double kalanAlacak = 0.0;
                    for (int j = 0; j < ah.detayHesaplar.objeler.Count; j++)
                    {
                        HesapBilgi alth = (HesapBilgi)ah.detayHesaplar[j];
                        HesapBilgi gelecekalth = new TNS.MUH.HesapBilgi("", "", 0, 0, "", "", "", "", "", "", "");

                        if ((j + 1) < ah.detayHesaplar.objeler.Count)
                            gelecekalth = (HesapBilgi)ah.detayHesaplar[j + 1];

                        borc += alth.borc;
                        alacak += alth.alacak;

                        if (alth.kalanBorc > 0 && alth.kalanAlacak > 0)
                        {
                            double sonucKalan = alth.kalanBorc - alth.kalanAlacak;
                            if (sonucKalan > 0)
                            {
                                alth.kalanBorc = alth.kalanBorc - alth.kalanAlacak;
                                alth.kalanAlacak = 0.0;
                            }
                            else
                            {
                                alth.kalanAlacak = alth.kalanAlacak - alth.kalanBorc;
                                alth.kalanBorc = 0.0;
                            }
                        }

                        kalanBorc += alth.kalanBorc;
                        kalanAlacak += alth.kalanAlacak;

                        if (gelecekalth.hesapKod != alth.hesapKod || gelecekalth.hesapKod == "")//sonraki hesap kodu şimdki hesap kodundan farklı ise toplamı al, şimdiki hesap kod en son ise yine işlem yap
                        {
                            alth.kur = ""; alth.fon = ""; alth.fin = ""; alth.eko = ""; alth.altHesapKod = "";
                            alth.borc = borc;
                            alth.alacak = alacak;
                            alth.kalanBorc = kalanBorc;
                            alth.kalanAlacak = kalanAlacak;
                            borc = 0.0;
                            alacak = 0.0;
                            kalanBorc = 0.0;
                            kalanAlacak = 0.0;
                        }
                        else
                        {
                            alth.hesapKod = "";
                        }

                    }
                }
            }

            int i;
            for (i = 0; i < mb.anaHesaplar.objeler.Count; i++)
            {
                AnaHesap ah = (AnaHesap)mb.anaHesaplar[i];
                if (bakiyeSifir > 0)
                {
                    if (ah.kalanBorcToplam == 0 && ah.kalanAlacakToplam == 0)
                        continue;
                }

                //SatirAc(true);
                //if (seviye == 1)
                //{
                //    stBorc += ah.borcToplam;
                //    stAlacak += ah.alacakToplam;
                //}
                //XLS.HucreDegerYaz(aktifSatir, sutun, ah.hesapKod);
                //XLS.HucreDegerYaz(aktifSatir, sutun + 14, ah.hesapAd);
                //XLS.HucreDegerYaz(aktifSatir, sutun + 15, ah.borcToplam);
                //XLS.HucreDegerYaz(aktifSatir, sutun + 16, ah.alacakToplam);
                //XLS.HucreDegerYaz(aktifSatir, sutun + 17, ah.kalanBorcToplam);
                //XLS.HucreDegerYaz(aktifSatir, sutun + 18, ah.kalanAlacakToplam);

                for (int j = 0; j < ah.detayHesaplar.objeler.Count; j++)
                {
                    HesapBilgi alth = (HesapBilgi)ah.detayHesaplar[j];
                    if (alth.hesapKod == "") continue;

                    if (alth.kalanBorc > 0 && alth.kalanAlacak > 0)
                    {
                        double sonucKalan = alth.kalanBorc - alth.kalanAlacak;
                        if (sonucKalan > 0)
                        {
                            alth.kalanBorc = alth.kalanBorc - alth.kalanAlacak;
                            alth.kalanAlacak = 0.0;
                        }
                        else
                        {
                            alth.kalanAlacak = alth.kalanAlacak - alth.kalanBorc;
                            alth.kalanBorc = 0.0;
                        }
                    }

                    if (bakiyeSifir > 0)
                    {
                        if (alth.kalanBorc == 0 && alth.kalanAlacak == 0)
                            continue;
                    }

                    SatirAc(false);
                    stBorc += alth.borc;
                    stAlacak += alth.alacak;

                    //stBakiyeBorc += alth.kalanBorc;
                    //stBakiyeAlacak += alth.kalanAlacak;

                    XLS.HucreDegerYaz(aktifSatir, sutun - 4, saymanlik);
                    XLS.HucreDegerYaz(aktifSatir, sutun - 3, tarih2.ToString());
                    XLS.HucreDegerYaz(aktifSatir, sutun - 2, yil);
                    XLS.HucreDegerYaz(aktifSatir, sutun - 1, tarih2.Ay);

                    string[] h = alth.hesapKod.Split('.');
                    if (h.Length > 0)
                        XLS.HucreDegerYaz(aktifSatir, sutun, h[0]);
                    if (h.Length > 1)
                        XLS.HucreDegerYaz(aktifSatir, sutun + 10, h[1]);
                    if (h.Length > 2)
                        XLS.HucreDegerYaz(aktifSatir, sutun + 11, h[2]);
                    if (h.Length > 3)
                        XLS.HucreDegerYaz(aktifSatir, sutun + 12, h[3]);
                    if (h.Length > 4)
                        XLS.HucreDegerYaz(aktifSatir, sutun + 13, h[4]);

                    if (prgTertipGoster == 0)
                    {
                        if (!string.IsNullOrEmpty(alth.kur))
                        {
                            string[] k = alth.kur.Split('.');
                            if (k.Length > 0)
                                XLS.HucreDegerYaz(aktifSatir, sutun + 1, k[0]);
                            if (k.Length > 1)
                                XLS.HucreDegerYaz(aktifSatir, sutun + 2, k[1]);
                            if (k.Length > 2)
                                XLS.HucreDegerYaz(aktifSatir, sutun + 3, k[2]);
                            if (k.Length > 3)
                                XLS.HucreDegerYaz(aktifSatir, sutun + 4, k[3]);
                        }

                        if (!string.IsNullOrEmpty(alth.fon))
                        {
                            string[] f = alth.fon.Split('.');
                            if (f.Length > 0)
                                XLS.HucreDegerYaz(aktifSatir, sutun + 5, f[0]);
                            if (f.Length > 1)
                                XLS.HucreDegerYaz(aktifSatir, sutun + 6, f[1]);
                            if (f.Length > 2)
                                XLS.HucreDegerYaz(aktifSatir, sutun + 7, f[2]);
                            if (f.Length > 3)
                                XLS.HucreDegerYaz(aktifSatir, sutun + 8, f[3]);
                        }

                        XLS.HucreDegerYaz(aktifSatir, sutun + 9, alth.fin);
                    }
                    else
                    {
                        XLS.HucreBirlestir(aktifSatir, sutun + 1, aktifSatir, sutun + 9);
                        XLS.YatayHizala(aktifSatir, sutun + 1, 0);

                        if (alth.hesapKod.StartsWith("630") ||
                            alth.hesapKod.StartsWith("830") ||
                            alth.hesapKod.StartsWith("90"))
                        {
                            string trp = TNS.MUH.Arac.ProgramTertipHazirla(alth, htPrgIliski);
                            XLS.HucreDegerYaz(aktifSatir, sutun + 1, trp);
                        }
                    }

                    //string[] e = alth.eko.Split('.');
                    //if (e.Length > 0)
                    //    XLS.HucreDegerYaz(aktifSatir, sutun + 10, e[0]);
                    //if (e.Length > 1)
                    //    XLS.HucreDegerYaz(aktifSatir, sutun + 11, e[1]);
                    //if (e.Length > 2)
                    //    XLS.HucreDegerYaz(aktifSatir, sutun + 12, e[2]);
                    //if (e.Length > 3)
                    //    XLS.HucreDegerYaz(aktifSatir, sutun + 13, e[3]);

                    XLS.HucreDegerYaz(aktifSatir, sutun + 14, alth.hesapAd);
                    XLS.HucreDegerYaz(aktifSatir, sutun + 15, alth.borc);
                    XLS.HucreDegerYaz(aktifSatir, sutun + 16, alth.alacak);
                    XLS.HucreDegerYaz(aktifSatir, sutun + 17, alth.kalanBorc);
                    XLS.HucreDegerYaz(aktifSatir, sutun + 18, alth.kalanAlacak);
                }
            }
            if (yazilanSatir > 0)
            {
                ToplamYaz(false);
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public static ENUMFISTUR[] IntDiziYap(string p)
        {
            string[] dizi = p.Split(',');
            List<ENUMFISTUR> list = new List<ENUMFISTUR>();
            foreach (string item in dizi)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    try
                    {
                        ENUMFISTUR tut = (ENUMFISTUR)Convert.ToInt32(item);
                        list.Add(tut);
                    }
                    catch { }
                }
            }
            return list.ToArray();
        }
    }

    /// <summary>
    /// Kontrol830905 rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class Kontrol830905
    {
        OrtakClass.Tablo XLS;
        int aktifSatir, sutun;

        /// <summary>
        /// Kontrol830905 raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            string saymanlikAd = (string)raporParam["SaymanlikAd"] + "";

            OdemeEmriMIFForm mb = s.Kontrol830905((TNS.KYM.Kullanici)kullanan, yil, saymanlik, birim);

            if (mb.aciklama != "")
                throw new Exception(mb.aciklama);
            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdBulYaz("txtYil", yil.ToString());
            XLS.HucreAdBulYaz("txtMuhasebe", saymanlik + " - " + saymanlikAd);
            XLS.HucreAdBulYaz("txtBirim", birim);
            int satir = 0;
            sutun = 0;
            XLS.HucreAdAdresCoz("BaslaHucre", ref satir, ref sutun);

            aktifSatir = satir;
            foreach (OdemeEmriMIFDetay od in mb.detay.objeler)
            {
                XLS.HucreKopyala(satir, sutun, satir, sutun + 12, aktifSatir, sutun);

                XLS.HucreDegerYaz(aktifSatir, sutun, od.belgeNo);
                XLS.HucreDegerYaz(aktifSatir, sutun + 1, od.hesapKod);
                XLS.HucreDegerYaz(aktifSatir, sutun + 2, TNS.MUH.Arac.TertipStrOl(od));
                XLS.HucreDegerYaz(aktifSatir, sutun + 3, od.borc);
                XLS.HucreDegerYaz(aktifSatir, sutun + 4, od.alacak);
                XLS.HucreDegerYaz(aktifSatir, sutun + 5, od.aciklama);
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    /// <summary>
    /// Kontrol830905 rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class AcikAvanslar
    {
        OrtakClass.Tablo XLS;
        int aktifSatir, sutun;
        int yil;
        int saymanlik;
        string birim;
        string saymanlikAd;
        string mail;
        int muhasebe = 0;
        string muhasebeBirim = "";
        string altHesap;

        public TNSCollection PersonelListesiBul(System.Collections.Hashtable raporParam)
        {
            ParametreleriAl(raporParam);
            string sql = SqlOl(1);
            TNS.UZY.VTSorgula vts = new VTSorgula(sql, "BaglantiSatiriBUM", 0);
            TNS.BKD.Tertip t = new Tertip(yil, ENUMTabloKod.TABOdemeEmri);
            TNSCollection tc = new TNSCollection();
            while (vts.satirOku() != null)
            {
                StrKodStrAd s = new StrKodStrAd(vts.StringAl(0), vts.StringAl(1));
                tc.Add(s);
            }
            return tc;
        }


        public TNSCollection BirimListesiBul(System.Collections.Hashtable raporParam)
        {
            ParametreleriAl(raporParam);
            string sql = SqlOl(2);
            TNS.UZY.VTSorgula vts = new VTSorgula(sql, "BaglantiSatiriBUM", 0);
            TNS.BKD.Tertip t = new Tertip(yil, ENUMTabloKod.TABOdemeEmri);
            TNSCollection tc = new TNSCollection();
            while (vts.satirOku() != null)
            {
                StrKodStrAd s = new StrKodStrAd(vts.IntAl(0) + "-" + vts.StringAl(1), vts.StringAl(2));
                tc.Add(s);
            }
            return tc;
        }

        public void ParametreleriAl(System.Collections.Hashtable raporParam)
        {
            yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            birim = (string)raporParam["Birim"];
            saymanlikAd = (string)raporParam["SaymanlikAd"] + "";
            mail = (string)raporParam["Mail"] + "";
            altHesap = (string)raporParam["AltHesap"] + "";
            muhasebe = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Muhasebe"], 0);
            muhasebeBirim = (string)raporParam["MuhasebeBirim"];
        }

        /// <summary>
        /// Kontrol830905 raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            ParametreleriAl(raporParam);

            XLS = GenelIslemler.NewTablo();
            if (string.IsNullOrWhiteSpace(ciktiYeri)) ciktiYeri = GenelIslemler.ExcelTur();
            sonucDosyaAd = sonucDosyaAd + "." + ciktiYeri;
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdBulYaz("txtYil", yil.ToString());
            XLS.HucreAdBulYaz("txtMuhasebe", saymanlik);
            XLS.HucreAdBulYaz("txtBirimAd", saymanlikAd);
            XLS.HucreAdBulYaz("txtBirim", birim);
            int satir = 0;
            sutun = 0;
            int fsayfa = 0; int fsatir1 = 0; int fsutun1 = 0; int fsatir2 = 0; int fsutun2 = 0;

            XLS.HucreAdAdresCoz("FormatSatir", ref fsayfa, ref fsatir1, ref fsutun1, ref fsatir2, ref fsutun2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            int yuks = XLS.SatirYukseklikAl(satir);
            aktifSatir = satir;
            string sql = SqlOl(0);
            TNS.UZY.VTSorgula vts = new VTSorgula(sql, "BaglantiSatiriBUM", 0);
            TNS.BKD.Tertip t = new Tertip(yil, ENUMTabloKod.TABOdemeEmri);
            while (vts.satirOku() != null)
            {
                double borc = vts.DoubleAl(9);
                double alacak = vts.DoubleAl(10);
                double borcDoviz = vts.DoubleAl(11);
                string doviz = "";
                double yazilacakBorc = borc;
                if (borcDoviz > 0)
                {
                    yazilacakBorc = borcDoviz;//Döviz çalışan hesapların kapatması döviz rakamı geldiği için Manas Melih 21.10.2021
                    doviz = " (" + vts.StringAl(12) + ")";
                }

                XLS.HucreKopyala(fsatir1, fsutun1, fsatir2, fsutun2, aktifSatir, sutun);
                XLS.HucreDegerYaz(aktifSatir, sutun, vts.StringAl(0));
                XLS.HucreDegerYaz(aktifSatir, sutun + 1, vts.DTAl(1));
                XLS.HucreDegerYaz(aktifSatir, sutun + 2, vts.StringAl(2));
                string hKod = vts.StringAl(3);
                XLS.HucreDegerYaz(aktifSatir, sutun + 3, OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(0), hKod));
                XLS.HucreDegerYaz(aktifSatir, sutun + 4, vts.StringAl(4) + doviz);
                XLS.HucreDegerYaz(aktifSatir, sutun + 5, vts.StringAl(5));
                string unvan = vts.StringAl(6);
                if (string.IsNullOrWhiteSpace(unvan)) unvan = vts.StringAl(7);
                XLS.HucreDegerYaz(aktifSatir, sutun + 6, unvan);
                XLS.HucreDegerYaz(aktifSatir, sutun + 7, vts.IntAl(8));
                XLS.HucreDegerYaz(aktifSatir, sutun + 8, yazilacakBorc);
                XLS.HucreDegerYaz(aktifSatir, sutun + 9, alacak);
                XLS.SatirYukseklikAyarla(aktifSatir, aktifSatir, -1, 7, yuks);
                aktifSatir++;
            }
            if (aktifSatir == satir)
                return "";
            XLS.DosyaSaklamaFormatAta(ciktiYeri);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string SqlOl(int kisi)
        {
            string alanlar;
            if (kisi == 2)
                alanlar = "DISTINCT MUHMIFFORM.MUHASEBEKOD, MUHMIFFORM.BIRIMKOD, (SELECT EPOSTA FROM HarcamaYonetimi.dbo.HRCIDAREBILGI IB where IB.MUHASEBEKOD=MUHMIFFORM.MUHASEBEKOD AND IB.BIRIMKOD=MUHMIFFORM.BIRIMKOD)";
            else if (kisi == 1)
                alanlar = "DISTINCT MUHMIF.ALTHESAPKOD, (SELECT EPOSTA FROM Personel.dbo.PERPersonel where TCKIMLIKNO=MUHMIF.ALTHESAPKOD)";
            else
                alanlar = "MUHMIFFORM.BELGENO, MUHMIFFORM.YEVMIYETARIH, MUHMIFFORM.ACIKLAMA, MUHMIF.HESAPKOD, MUHHESAPPLANI.ACIKLAMA, MUHMIF.ALTHESAPKOD, (SELECT MIN(AD) FROM HarcamaYonetimi.dbo.HRCFIRMABILGISI where VERGINO=MUHMIF.ALTHESAPKOD), (SELECT MIN(AD + ' ' + SOYAD) FROM Personel.dbo.PERPersonel where TCKIMLIKNO=MUHMIF.ALTHESAPKOD), DATEDIFF(d, MUHMIFFORM.YEVMIYETARIH, GETDATE()) GUN, MUHMIF.BORC, (SELECT SUM(KTUTAR) FROM MUHKAPATMA WHERE MUHKAPATMA.YIL=MUHMIF.YIL AND MUHKAPATMA.KBELGENO=MUHMIF.BELGENO AND MUHKAPATMA.KILGILIVNO=MUHMIF.ALTHESAPKOD AND MUHKAPATMA.KBIRIMKOD=MUHMIF.DETAYBIRIM AND KHESAPKOD=MUHMIF.HESAPKOD) ALACAK, MUHMIF.DOVIZBORC,MUHMIF.DOVIZ";
            string sql = @"SELECT " + alanlar + " FROM MUHMIF ";
            sql += "INNER JOIN MUHMIFFORM ON MUHMIFFORM.YIL=MUHMIF.YIL AND MUHMIFFORM.BELGENO=MUHMIF.BELGENO ";
            sql += "INNER JOIN MUHHESAPPLANI ON MUHHESAPPLANI.YIL=MUHMIF.YIL AND MUHHESAPPLANI.HESAPKOD=MUHMIF.HESAPKOD where MUHMIF.YIL=" + yil.ToString() + " AND MUHMIFFORM.FORMDURUM=6 AND MUHHESAPPLANI.NITELIK LIKE '%;5;%' and BORC>0 ";
            sql += "AND ((SELECT SUM(KTUTAR) FROM MUHKAPATMA WHERE MUHKAPATMA.YIL=MUHMIF.YIL AND MUHKAPATMA.KBELGENO=MUHMIF.BELGENO AND MUHKAPATMA.KILGILIVNO=MUHMIF.ALTHESAPKOD AND KHESAPKOD=MUHMIF.HESAPKOD AND KBIRIMKOD=MUHMIF.DETAYBIRIM) IS NULL OR (SELECT SUM(KTUTAR) FROM MUHKAPATMA WHERE MUHKAPATMA.YIL=MUHMIF.YIL AND MUHKAPATMA.KBELGENO=MUHMIF.BELGENO AND MUHKAPATMA.KILGILIVNO=MUHMIF.ALTHESAPKOD AND KHESAPKOD=MUHMIF.HESAPKOD) < BORC) ";
            if (!string.IsNullOrWhiteSpace(altHesap))
                sql += "AND MUHMIF.ALTHESAPKOD = '" + altHesap + "'";
            if (muhasebe > 0)
                sql += "AND MUHMIFFORM.MUHASEBEKOD = " + muhasebe.ToString() + "";
            if (!string.IsNullOrWhiteSpace(muhasebeBirim))
                sql += "AND MUHMIFFORM.BIRIMKOD = '" + muhasebeBirim + "'";
            if (mail == "1")
                sql += "AND (DATEDIFF(d, MUHMIFFORM.YEVMIYETARIH, GETDATE())=20 or DATEDIFF(d, MUHMIFFORM.YEVMIYETARIH, GETDATE())=25 or DATEDIFF(d, MUHMIFFORM.YEVMIYETARIH, GETDATE())>=30)";
            if (kisi == 0)
                sql += " order by GUN desc";
            return sql;
        }
    }

    /// <summary>
    /// OdenekUstu rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class OdenekUstu
    {
        OrtakClass.Tablo XLS;
        int aktifSatir, sutun;

        /// <summary>
        /// OdenekUstu raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            string saymanlikAd = (string)raporParam["SaymanlikAd"] + "";

            OdemeEmriMIFForm mb = s.OdenekUstu((TNS.KYM.Kullanici)kullanan, yil, saymanlik, birim);

            if (mb.aciklama != "")
                throw new Exception(mb.aciklama);
            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdBulYaz("txtYil", yil.ToString());
            XLS.HucreAdBulYaz("txtMuhasebe", saymanlik + " - " + saymanlikAd);
            XLS.HucreAdBulYaz("txtBirim", birim);
            int satir = 0;
            sutun = 0;
            XLS.HucreAdAdresCoz("BaslaHucre", ref satir, ref sutun);

            aktifSatir = satir;
            foreach (OdemeEmriMIFDetay od in mb.detay.objeler)
            {
                XLS.HucreKopyala(satir, sutun, satir, sutun + 12, aktifSatir, sutun);

                XLS.HucreDegerYaz(aktifSatir, sutun, od.belgeNo);
                XLS.HucreDegerYaz(aktifSatir, sutun + 1, od.hesapKod);
                XLS.HucreDegerYaz(aktifSatir, sutun + 2, TNS.MUH.Arac.TertipStrOl(od));
                XLS.HucreDegerYaz(aktifSatir, sutun + 3, od.borc);
                XLS.HucreDegerYaz(aktifSatir, sutun + 4, od.alacak);
                XLS.HucreDegerYaz(aktifSatir, sutun + 5, od.aciklama);
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    /// <summary>
    /// UstHesapKodKullanilanlar rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class UstHesapKodKullanilanlar
    {
        OrtakClass.Tablo XLS;
        int aktifSatir, sutun;

        /// <summary>
        /// UstHesapKodKullanilanlar raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            string saymanlikAd = (string)raporParam["SaymanlikAd"] + "";

            OdemeEmriMIFForm mb = s.UstHesapKodKullanilanlar((TNS.KYM.Kullanici)kullanan, yil, saymanlik, birim);

            if (mb.aciklama != "")
                throw new Exception(mb.aciklama);
            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdBulYaz("txtYil", yil.ToString());
            //XLS.HucreAdBulYaz("txtMuhasebe", saymanlik + " - " + saymanlikAd);
            //XLS.HucreAdBulYaz("txtBirim", birim);
            int satir = 0;
            sutun = 0;
            XLS.HucreAdAdresCoz("BaslaHucre", ref satir, ref sutun);

            aktifSatir = satir;
            foreach (OdemeEmriMIFDetay od in mb.detay.objeler)
            {
                XLS.HucreKopyala(satir, sutun, satir, sutun + 12, aktifSatir, sutun);

                XLS.HucreDegerYaz(aktifSatir, sutun, od.belgeNo);
                XLS.HucreDegerYaz(aktifSatir, sutun + 1, od.hesapKod);
                XLS.HucreDegerYaz(aktifSatir, sutun + 2, od.borc);
                XLS.HucreDegerYaz(aktifSatir, sutun + 3, od.alacak);
                XLS.HucreDegerYaz(aktifSatir, sutun + 4, od.aciklama);
                aktifSatir++;
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    /// <summary>
    /// Yevmiye rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class Yevmiye
    {
        OrtakClass.Tablo XLS;
        int sutun;
        int sayfadakiSatir = 66;
        string raporAdi = "YEVMİYE DEFTERİ";
        string kurumAdi = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

        /// <summary>
        /// Yevmiye raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int listeTur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["ListeTur"], 0);
            int sayistay = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Sayistay"], 0);

            //string basHesap = (string)raporParam["BasHesap"];
            //string bitHesap = (string)raporParam["BitHesap"];

            TNS.MUH.OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(yil);
            oEmriDetay.hesapKod = "";// basHesap.Replace(".", "").Trim();
            oEmriDetay.kur = "";
            oEmriDetay.fon = "";
            oEmriDetay.fin = "";
            oEmriDetay.eko = "";
            int RaporSira = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["RaporSira"], 0);

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            oEmri.sorguTarihSiraliMi = true;
            oEmri.detay.objeler.Add(oEmriDetay);
            if (RaporSira == 1)
                oEmri.sorguTarihSiraliMi = false;
            if (tur == 1)
            {
                oEmri.formDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
                oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else if (tur == 2)
            {
                oEmri.formDurum = (int)ENUMBelgeDurumu.ONAYLI;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else
            {
                oEmri.formDurum = 22;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            oEmri.fisTur = fisTur;
            //OrtakFonksiyonlar.HataStrYaz("Rapor Listele Başla" + DateTime.Now.ToString());
            ObjectArray mb = s.OdemeEmriMIFListele((TNS.KYM.Kullanici)kullanan, oEmri, true);
            //OrtakFonksiyonlar.HataStrYaz("Rapor Listele Bitti" + DateTime.Now.ToString());

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            OdemeEmriMIFDetay oncekiTarihToplam = new OdemeEmriMIFDetay(yil);
            if ((tarih1.Ay != 1) && tarih1.Yil == yil)
            {
                TNS.MUH.OdemeEmriMIFForm oEmriOncekiTarih = new OdemeEmriMIFForm();
                oEmriOncekiTarih = oEmri;
                oEmriOncekiTarih.sorguYEVYevTarih = "01.01." + yil + ";" + tarih1.ToString();

                ObjectArray oncekiTarihToplamListe = s.OdemeEmriMIFTarihOncesiToplamGetir((TNS.KYM.Kullanici)kullanan, oEmriOncekiTarih);
                if (oncekiTarihToplamListe.ObjeSayisi > 0)
                {
                    oncekiTarihToplam = (OdemeEmriMIFDetay)oncekiTarihToplamListe.objeler[0];
                }
            }

            if ((string)raporParam["RaporTur"] == "5.1")
            {
                sonucDosyaAd = sonucDosyaAd + ".txt";
                YevmiyeDefteriDosyayaYaz(XLS, kullanan, yil, tur, saymanlik, birim, mb, oEmri, tarih1, tarih2, sayistay, sonucDosyaAd);
            }
            else
            {
                XLS = GenelIslemler.NewTablo();
                sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
                XLS.DosyaAc(sablonAd, sonucDosyaAd);

                YevmiyeDefteriIcerikYaz(XLS, kullanan, yil, tur, saymanlik, birim, mb, oncekiTarihToplam, oEmri, tarih1, tarih2, listeTur);

                //Şablon olrak kullanılan ilk sayfayı sil
                //XLS.SheetSil(0);
                XLS.DosyaSaklaTamYol();
            }
            return sonucDosyaAd;
        }

        void YevmiyeDefteriIcerikYaz(OrtakClass.Tablo XLS, object kullanan, int yil, int tur, int saymanlik, string birim, ObjectArray mb, OdemeEmriMIFDetay oncekiTarihToplam, TNS.MUH.OdemeEmriMIFForm oEmri, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2, int listeTur)
        {
            string saymanlikAd = "";
            string birimAd = "";
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

            //Boş sayfa yarat ve 0. sayfayı şablon olarak kullan
            //*****************************************************
            int sayfadakiSatirSayisi = 500000;
            int aktifSayfa = 0;

            if (mb.objeler.Count > sayfadakiSatirSayisi)//Başlık yazmada problem var. Başlık 0 sheetine yazıyor.
            {
                XLS.YeniSheetEkle(0);
                aktifSayfa++;
                XLS.AktifSheetDegistir(aktifSayfa);
            }
            //*****************************************************

            BaslikYaz(XLS, yil, saymanlik, saymanlikAd, kurum, kurumAd, birim, birimAd, tarih1, tarih2, listeTur);

            int satir = 0;
            sutun = 0;
            XLS.HucreAdAdresCoz("BaslaHucre", ref satir, ref sutun);
            int sablonSatir = satir - 1;


            TNSDateTime belgeTarih = new TNSDateTime();
            TNSDateTime kontrolTarih = new TNSDateTime();
            double gunBorcToplam = 0.0;
            double gunAlacakToplam = 0.0;
            double ayBorcToplam = 0.0;
            double ayAlacakToplam = 0.0;
            double ayKumulatifBorcToplam = oncekiTarihToplam.borc;
            double ayKumulatifAlacakToplam = oncekiTarihToplam.alacak;
            double genelBorcToplam = 0.0;
            double genelAlacakToplam = 0.0;

            double yBorc = 0.00;
            double yAlacak = 0.00;
            foreach (OdemeEmriMIFForm f in mb.objeler)
            {
                //liste satır sayısını aşıyor ise yeni sayfaya geç
                if (satir + 1000 > sayfadakiSatirSayisi)
                {
                    XLS.HucreAdAdresCoz("BaslaHucre", ref satir, ref sutun);
                    XLS.YeniSheetEkle(0);
                    aktifSayfa++;
                    XLS.AktifSheetDegistir(aktifSayfa);
                    BaslikYaz(XLS, yil, saymanlik, saymanlikAd, kurum, kurumAd, birim, birimAd, tarih1, tarih2, listeTur);
                }

                ////---------------->
                if (tur == 1)
                {
                    if (belgeTarih.isNull) belgeTarih = f.YEVYevTarih;
                    kontrolTarih = f.YEVYevTarih;
                }
                else if (tur == 3 || tur == 2)
                {
                    if (belgeTarih.isNull) belgeTarih = f.islemTarih;
                    kontrolTarih = f.islemTarih;
                }

                if (belgeTarih.ToString() != kontrolTarih.ToString())
                {
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);
                    XLS.HucreBirlestir(satir, 0, satir, 7);
                    XLS.HucreDegerYaz(satir, 0, "GÜN TOPLAMI");
                    XLS.HucreDegerYaz(satir, 9, gunBorcToplam);
                    XLS.HucreDegerYaz(satir, 10, gunAlacakToplam);

                    satir++;
                }

                if (belgeTarih.Ay != kontrolTarih.Ay)
                {
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);
                    XLS.HucreBirlestir(satir, 0, satir, 7);
                    XLS.HucreDegerYaz(satir, 0, "AY TOPLAMI");
                    XLS.HucreDegerYaz(satir, 9, ayBorcToplam);
                    XLS.HucreDegerYaz(satir, 10, ayAlacakToplam);

                    satir++;
                }

                if (belgeTarih.Ay != kontrolTarih.Ay)
                {
                    if (ayKumulatifBorcToplam > 0.0 || ayKumulatifAlacakToplam > 0.0)
                    {
                        XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);
                        XLS.HucreBirlestir(satir, 0, satir, 7);
                        XLS.HucreDegerYaz(satir, 0, "ÖNCEKİ AYLARDAN DEVİR");
                        XLS.HucreDegerYaz(satir, 9, ayKumulatifBorcToplam);
                        XLS.HucreDegerYaz(satir, 10, ayKumulatifAlacakToplam);

                        satir++;

                        genelBorcToplam += ayKumulatifBorcToplam;
                        genelAlacakToplam += ayKumulatifAlacakToplam;
                    }

                    ayKumulatifBorcToplam += ayBorcToplam;
                    ayKumulatifAlacakToplam += ayAlacakToplam;
                }
                ////---------------->


                XLS.HucreKopyala(sablonSatir - 1, 0, sablonSatir - 1, 15, satir, 0);
                string bTur = "";
                if (f.belgeTur == ENUMBelgeTuru.ODEMEEMRI)
                    bTur = "ÖDE";
                else
                    bTur = "MİF";

                if (tur == 1)
                {
                    XLS.HucreDegerYaz(satir, 0, f.YEVYevTarih.ToString());
                    XLS.HucreDegerYaz(satir, 1, f.YEVYevNo);
                }
                else if (tur == 3 || tur == 2)
                {
                    XLS.HucreDegerYaz(satir, 0, f.islemTarih.ToString());
                    XLS.HucreDegerYaz(satir, 1, f.belgeNo + "-" + bTur);
                }


                XLS.HucreDegerYaz(satir, 2, f.birim);

                XLS.HucreDegerYaz(satir, 8, f.aciklama);

                yBorc = 0.00;
                yAlacak = 0.00;
                int anaSatir = satir;

                satir++;
                foreach (OdemeEmriMIFDetay d in f.detay.objeler)
                {
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);

                    XLS.HucreDegerYaz(satir, 2, d.DetayBirim);
                    XLS.HucreDegerYaz(satir, 3, d.hesapKod);
                    if (!String.IsNullOrEmpty(d.kur))
                    {
                        XLS.HucreDegerYaz(satir, 4, d.kur);
                        XLS.HucreDegerYaz(satir, 5, d.fon);
                        XLS.HucreDegerYaz(satir, 6, d.fin);
                        XLS.HucreDegerYaz(satir, 7, d.eko);
                        //XLS.HucreDegerYaz(satir, 6, d.hesapKod.Substring(4));
                    }
                    if (!string.IsNullOrWhiteSpace(d.satirAciklama))
                        XLS.HucreDegerYaz(satir, 8, d.satirAciklama);
                    else
                        XLS.HucreDegerYaz(satir, 8, d.hesapAd);
                    if (listeTur == 1)
                    {
                        XLS.HucreDegerYaz(satir, 9, d.borc);
                        XLS.HucreDegerYaz(satir, 10, d.alacak);
                        yBorc += d.borc;
                        yAlacak += d.alacak;
                    }
                    else
                    {
                        XLS.HucreDegerYaz(satir, 9, d.dovizBorc);
                        XLS.HucreDegerYaz(satir, 10, d.dovizAlacak);
                        yBorc += d.dovizBorc;
                        yAlacak += d.dovizAlacak;
                    }

                    satir++;
                }

                if (belgeTarih.Ay == kontrolTarih.Ay)
                {
                    ayBorcToplam += yBorc;
                    ayAlacakToplam += yAlacak;
                }
                else
                {
                    ayBorcToplam = yBorc;
                    ayAlacakToplam = yAlacak;
                }

                if (belgeTarih.ToString() == kontrolTarih.ToString())
                {
                    gunBorcToplam += yBorc;
                    gunAlacakToplam += yAlacak;
                }
                else
                {
                    gunBorcToplam = yBorc;
                    gunAlacakToplam = yAlacak;

                    if (tur == 1)
                    {
                        belgeTarih = f.YEVYevTarih;
                    }
                    else if (tur == 3 || tur == 2)
                    {
                        belgeTarih = f.islemTarih;
                    }
                }

                XLS.HucreDegerYaz(anaSatir, 9, yBorc);
                XLS.HucreDegerYaz(anaSatir, 10, yAlacak);

                genelBorcToplam += yBorc;
                genelAlacakToplam += yAlacak;
            }

            XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);
            XLS.HucreBirlestir(satir, 0, satir, 7);
            XLS.HucreDegerYaz(satir, 0, "GÜN TOPLAMI");
            XLS.HucreDegerYaz(satir, 9, gunBorcToplam);
            XLS.HucreDegerYaz(satir, 10, gunAlacakToplam);
            satir++;

            XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);
            XLS.HucreBirlestir(satir, 0, satir, 7);
            XLS.HucreDegerYaz(satir, 0, "AY TOPLAMI");
            XLS.HucreDegerYaz(satir, 9, ayBorcToplam);
            XLS.HucreDegerYaz(satir, 10, ayAlacakToplam);
            satir++;

            if (ayKumulatifBorcToplam > 0.0 || ayKumulatifAlacakToplam > 0.0)
            {
                XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);
                XLS.HucreBirlestir(satir, 0, satir, 7);
                XLS.HucreDegerYaz(satir, 0, "ÖNCEKİ AYLARDAN DEVİR");
                XLS.HucreDegerYaz(satir, 9, ayKumulatifBorcToplam);
                XLS.HucreDegerYaz(satir, 10, ayKumulatifAlacakToplam);
                satir++;
            }

            XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);
            XLS.HucreBirlestir(satir, 0, satir, 7);
            XLS.HucreDegerYaz(satir, 0, "GENEL TOPLAM");
            XLS.HucreDegerYaz(satir, 9, genelBorcToplam);
            XLS.HucreDegerYaz(satir, 10, genelAlacakToplam);
            satir++;

        }

        void BaslikYaz(OrtakClass.Tablo XLS, int yil, int saymanlik, string saymanlikAd, string kurum, string kurumAd, string birim, string birimAd, TNSDateTime tarih1, TNSDateTime tarih2, int listeTur)
        {
            XLS.HucreAdBulYaz("Yil", " : " + yil.ToString());
            XLS.HucreAdBulYaz("KurumAd", " : " + kurum + " - " + kurumAd);

            if (saymanlik > 0)
            {
                saymanlikAd = OrtakClass.GenelIslemler.KodAd(11, saymanlik.ToString(), true);
                XLS.HucreAdBulYaz("SaymanlikAd", " : " + saymanlik + " - " + saymanlikAd);
            }
            else
                XLS.HucreAdBulYaz("SaymanlikAd", " : Kurum Geneli");

            if (birim.Trim() != "")
            {
                birimAd = OrtakClass.GenelIslemler.KodAd(12, birim + "-" + saymanlik.ToString(), false);
                XLS.HucreAdBulYaz("BirimAdi", " : " + birim + " - " + birimAd);
            }
            else
                XLS.HucreAdBulYaz("BirimAdi", " : Kurum Geneli");

            XLS.HucreAdBulYaz("Bastar", tarih1.ToString());
            XLS.HucreAdBulYaz("Bittar", tarih2.ToString());

            string paraBirimi = ButceMuhasebeGenel.DondurParaBirimi(1).kisaltma;
            if (listeTur == 2) paraBirimi = "DÖVİZ";
            XLS.HucreAdBulYaz("ParaBirimi", paraBirimi);

        }

        void YevmiyeDefteriDosyayaYaz(OrtakClass.Tablo XLS, object kullanan, int yil, int tur, int saymanlik, string birim, ObjectArray mb, TNS.MUH.OdemeEmriMIFForm oEmri, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2, int sayistay, string sonucDosyaAd)
        {
            StringBuilder rapor = new StringBuilder();
            double tBorc = 0;
            double tAlacak = 0;
            int sayfaNo = 0;
            int satir = 1;
            string kolonDeger = "";

            string kolon = "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}";
            string baslik = "Tarih  Numara     Hesap Kodu             Kurumsal    Fonksiyon F Açıklama                   Borç            Alacak\r\n";
            baslik += "------ ---------- ---------------------- ----------- --------- - -------------------------- --------------- ---------------";

            if (sayistay > 0)
            {
                kolon = "{9}|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}";
                baslik = "";
            }

            sayfaNo++;
            if (sayistay == 0)
                BaslikYazDosya(rapor, raporAdi, kurumAdi, sayfaNo, baslik);

            satir = 6;

            foreach (OdemeEmriMIFForm f in mb.objeler)
            {
                string kolon0 = "";
                string kolon1 = "";
                string kolon2 = "";
                string kolon3 = "";
                string kolon4 = "";
                string kolon5 = "";
                string kolon6 = "";
                string kolon7 = "";
                string kolon8 = "";
                string kolon9 = "";

                f.aciklama = f.aciklama.Replace('\r', ' ');
                f.aciklama = f.aciklama.Replace('\n', ' ');
                f.aciklama = f.aciklama.Trim();

                if (sayistay == 0)
                {
                    if (f.aciklama.Length > 26)
                        kolon6 = f.aciklama.Substring(0, 26).PadRight(26);
                    else
                        kolon6 = f.aciklama.PadRight(26);
                }
                else
                    kolon6 = f.aciklama;

                //string bTur = "";
                //if (f.belgeTur == ENUMBelgeTuru.ODEMEEMRI)
                //    bTur = "ÖDE";
                //else
                //    bTur = "MİF";

                if (tur == 1)
                {
                    kolon0 = f.YEVYevTarih.Gun.ToString("00") + f.YEVYevTarih.Ay.ToString("00") + f.YEVYevTarih.Yil.ToString().Substring(2, 2);
                    kolon1 = f.YEVYevNo.ToString().PadRight(10);

                    if (sayistay > 0)
                    {
                        kolon0 = f.YEVYevTarih.Gun.ToString("00") + "." + f.YEVYevTarih.Ay.ToString("00") + "." + f.YEVYevTarih.Yil.ToString();
                        kolon9 = f.formDurumTarih.Gun.ToString("00") + "." + f.formDurumTarih.Ay.ToString("00") + "." + f.formDurumTarih.Yil.ToString("0000") + " " + f.formDurumTarih.Saat.ToString("00") + ":" + f.formDurumTarih.Dakika.ToString("00") + ":" + f.formDurumTarih.Saniye.ToString("00");
                    }

                }
                else
                {
                    kolon0 = f.islemTarih.Gun.ToString("00") + f.islemTarih.Ay.ToString("00") + f.islemTarih.Yil.ToString().Substring(2, 2);
                    kolon1 = f.belgeNo.ToString().PadRight(10);

                }

                double yBorc = 0.00;
                double yAlacak = 0.00;

                if (sayistay == 0)
                    kolonDeger = string.Format(kolon, kolon0, kolon1, "".PadRight(22), "".PadRight(11), "".PadRight(9), "".PadRight(1), kolon6, "", "", kolon9);
                else
                    kolonDeger = string.Format(kolon, kolon0.Trim(), kolon1.Trim(), "", "", "", "", kolon6.Trim(), "", "", kolon9.Trim());

                SatirYazDosya(rapor, kolonDeger, ref satir, ref sayfaNo, baslik, sayistay);

                foreach (OdemeEmriMIFDetay d in f.detay.objeler)
                {
                    kolon2 = d.hesapKod.PadRight(22);
                    if (d.hesapAd.Length > 26)
                        kolon6 = d.hesapAd.Substring(0, 26).PadRight(26);
                    else
                        kolon6 = d.hesapAd.PadRight(26);

                    if (!String.IsNullOrEmpty(d.kur))
                    {
                        kolon3 = d.kur.PadRight(11);
                        kolon4 = d.fon.PadRight(9);
                        kolon5 = d.fin.PadRight(1);
                        //kolon6 = d.eko.PadRight(18);
                    }
                    else
                    {
                        kolon3 = "".PadRight(11);
                        kolon4 = "".PadRight(9);
                        kolon5 = "".PadRight(1);
                        //kolon6 = "".PadRight(18);
                    }

                    tBorc += d.borc;
                    tAlacak += d.alacak;

                    yBorc += d.borc;
                    yAlacak += d.alacak;

                    if (sayistay == 0)
                    {
                        kolon7 = d.borc.ToString("#,###.00").PadLeft(15);
                        kolon8 = d.alacak.ToString("#,###.00").PadLeft(15);
                        kolonDeger = string.Format(kolon, "".PadRight(6), "".PadLeft(10), kolon2, kolon3, kolon4, kolon5, kolon6, kolon7, kolon8, kolon9);
                    }
                    else
                    {
                        kolon7 = d.borc.ToString("###.00");
                        kolon8 = d.alacak.ToString("###.00");
                        kolonDeger = string.Format(kolon, kolon0.Trim(), kolon1.Trim(), kolon2.Trim(), kolon3.Trim(), kolon4.Trim(), kolon5.Trim(), kolon6.Trim(), kolon7.Trim(), kolon8.Trim(), kolon9.Trim());
                    }

                    SatirYazDosya(rapor, kolonDeger, ref satir, ref sayfaNo, baslik, sayistay);
                }
                kolon0 = "";
                kolon1 = "";
                kolon6 = "";

                kolon7 = yBorc.ToString("#,###.00").PadLeft(15);
                kolon8 = yAlacak.ToString("#,###.00").PadLeft(15);

                if (sayistay == 0)
                {
                    kolonDeger = string.Format(kolon, "".PadRight(6), "".PadLeft(10), "".PadLeft(22), "".PadLeft(11), "".PadLeft(9), "".PadLeft(1), "BELGE TOPLAMI : ".PadRight(26), kolon7, kolon8, kolon9);

                    SatirYazDosya(rapor, kolonDeger, ref satir, ref sayfaNo, baslik, sayistay);
                }
            }

            OrtakFonksiyonlar.DosyaYaz(sonucDosyaAd, rapor.ToString(), true);
        }

        void SatirYazDosya(StringBuilder rapor, string kolonDeger, ref int satir, ref int sayfaNo, string baslik, int sayistay)
        {
            if (satir > sayfadakiSatir)
            {
                sayfaNo++;

                if (sayistay == 0)
                    BaslikYazDosya(rapor, raporAdi, kurumAdi, sayfaNo, baslik);

                satir = 6;
            }

            rapor.AppendLine(kolonDeger);
            satir++;
        }

        void BaslikYazDosya(StringBuilder rapor, string raporAdi, string kurumAdi, int sayfaNo, string baslik)
        {
            if (sayfaNo == 1)
            {
                rapor.AppendLine("".PadLeft(62 - (raporAdi.Length / 2)) + raporAdi);
                rapor.AppendLine("".PadLeft(62 - (kurumAdi.Length / 2)) + kurumAdi);
                sayfadakiSatir = 68;
            }
            rapor.AppendLine("Sayfa No   : " + sayfaNo);
            rapor.AppendLine(baslik);
        }
    }

    /// <summary>
    /// Yevmiye rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class YevmiyeAktar
    {
        /// <summary>
        /// Yevmiye raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string sayKurum = SayistayKoduBul(kurum);
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int listeTur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["ListeTur"], 0);
            int sayistay = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Sayistay"], 0);

            string basHesap = (string)raporParam["BasHesap"];
            string bitHesap = (string)raporParam["BitHesap"];

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            oEmri.sorguTarihSiraliMi = true;

            if (tur == 1)
            {
                oEmri.formDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
                oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else if (tur == 2)
            {
                oEmri.formDurum = (int)ENUMBelgeDurumu.ONAYLI;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else
            {
                oEmri.formDurum = 22;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            oEmri.fisTurYap(fisTur);

            ObjectArray mb = s.OdemeEmriMIFListele((TNS.KYM.Kullanici)kullanan, oEmri, true);

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            sonucDosyaAd = sonucDosyaAd + ".csv";
            System.IO.StreamWriter sw = new System.IO.StreamWriter(sonucDosyaAd, false, System.Text.Encoding.Default);
            /*
            sw.WriteLine(string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}|{20}|{21}|{22}|{23}|{24}|{25}|{26}|{27}|{28}|{29}|{30}|{31}|{32}|{33}|{34}|{35}|{36}|{37}|{38}"
    , "kurumAd" //2 KURUM ADI 	STRING	
    , "kurum" //3 KURUM KODU 	STRING
    , "muhasebeAd" //4 MUHASEBE BİRİMİNİN AD
    , "birimAd" //4	BİRİMİNİN ADI	STRING	
    , "belgeTarih" //5	YEVMİYE TARİHİ	DATE	
    , "yevmiyeTarih" //6	YEVMİYE TARİHİ	DATE	
    , "belgeNo" //7	YEVMİYE NUMARASI	STRING	
    , "YevNo" //8	YEVMİYE NUMARASI	STRING	
    , "surec" //9	YEVMİYE NUMARASI	STRING	
    , "aciklama" //10	ÖDEME EMRİ VEYA MUHASEBE İŞLEM FİŞİ AÇIKLAMA	STRING	Belge Düzenleme Nedeni
    , "anahesapKod" //11	HESAP KODU	STRING	
    , "hesapKod" //12	HESAP KODU	STRING	
    , "hesapAd" //13	HESAP ADI	STRING	
    , "kur1" //14	KURUMSAL  KODU 1	STRING	
    , "kur2"//15	KURUMSAL  KODU 2	STRING	
    , "kur3"//16	KURUMSAL  KODU 3	STRING	
    , "kur4"//17	KURUMSAL KODU 4	STRING	
    , "kur5"//18	KURUMSAL  KODU 5	STRING	
    , "fon1"//19	FONKSİYON KOD 1	STRING	
    , "fon2"//20	FONKSİYON KOD 2	STRING	
    , "fon3"//21	FONKSİYON KOD 3	STRING	
    , "fon4"//22	FONKSİYON KOD 4	STRING	
    , "fin1"//23	FİNANS KODU	STRING	
    , "eko1"//24	EKONOMİK /Y. HESAP KODU 1	STRING	
    , "eko2"//25	EKONOMİK /Y. HESAP KODU 2	STRING	
    , "eko3"//26	EKONOMİK /Y. HESAP KODU 3	STRING	
    , "eko4"//27	EKONOMİK /Y. HESAP KODU 4	STRING	
    , "borc" //28	BORÇ	NUMERIC	0,00 formatında  (“0,..” olmayacak )
    , "alacak" //29	ALACAK	NUMERIC	0,00 formatında  (“0,..” olmayacak )
    , "Bos1" //30	BÜYÜK DEFTERDEN AÇIKLAMA	STRING	Hesap adı olabilir Hizmet alımlar, alınan teminatlar, Gider yansıtma hesabı .....
    , "ilgiliAd" //31	MUHASEBE İŞLEM FİŞİ/ÖDEME EMRİ BELGESİ/YARDIMCI DEFTERDEN İLGİLİNİN ADI, SOYADI	STRING	
    , "ilgiliNo" //32	MUHASEBE İŞLEM FİŞİ/ÖDEME EMRİ BELGESİ/YARDIMCI DEFTERDEN  İLGİLİNİN T.C./ VERGİ KİMLİK NO	STRING	
    , "ilgiliBankaAd" //33	MUHASEBE İŞLEM FİŞİ/ÖDEME EMRİ BELGESİ/YARDIMCI DEFTERDEN  İLGİLİNİN BANKA ŞUBE ADI	STRING	İBAN varsa ayrı bir sütun açılacak
    , "ilgiliBankaNo" //34	MUHASEBE İŞLEM FİŞİ/ÖDEME EMRİ BELGESİ/YARDIMCI DEFTERDEN  İLGİLİNİN BANKA HESAP NUMARASI	STRING	İBAN varsa ayrı bir sütun açılacak
    , "Bos2" //35	MUHASEBE İŞLEM FİŞİNDEN İADE OLUNAN EMANETLERDE/ÖN ÖDEMELERDE/ALACAKLARDA VB. KİMİN ADINA KAYITLI OLDUĞU	STRING	
    , "Bos3" //36	MUHASEBE İŞLEM FİŞİNDEN İADE OLUNAN EMANETLERDE/ÖN ÖDEMELERDE/ALACAKLARDA VB. AVANS VE KREDİNİN VERİLDİĞİ TARİH	DATE	
    , "Bos4"//37	MUHASEBE İŞLEM FİŞİNDEN İADE OLUNAN EMANETLERDE/ÖN ÖDEMELERDE/ALACAKLARDA VB. YEVMİYE NUMARASI	STRING	
    , "Bos5" //38	ÇEK / GÖNDERME EMRİ NO.	STRING	
    , "EkonomikKod" //39	EkoKod
    ));
            */

            foreach (OdemeEmriMIFForm f in mb.objeler)
            {
                string aciklama = f.aciklama.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ').Replace(';', ',').Replace('|', ',').Trim();
                TNSDateTime belgeTarih;
                TNSDateTime yevmiyeTarih;
                string yevmiyeTarihddmmyyyy = "";
                string yevmiyeTarihyyyymmdd = "";
                string yevmiyeTarihStr = "";
                if (!f.YEVYevTarih.isNull)
                {
                    yevmiyeTarih = f.YEVYevTarih;
                    yevmiyeTarihddmmyyyy = yevmiyeTarih.Gun.ToString("00") + yevmiyeTarih.Ay.ToString("00") + yevmiyeTarih.Yil.ToString("0000");
                    yevmiyeTarihyyyymmdd = yevmiyeTarih.Yil.ToString("0000") + yevmiyeTarih.Ay.ToString("00") + yevmiyeTarih.Gun.ToString("00");
                    yevmiyeTarihStr = yevmiyeTarih.ToString();
                }

                belgeTarih = f.islemTarih;
                string belgeTarihddmmyyyy = belgeTarih.Gun.ToString("00") + belgeTarih.Ay.ToString("00") + belgeTarih.Yil.ToString("0000");
                string belgeTarihyyyymmdd = belgeTarih.Yil.ToString("0000") + belgeTarih.Ay.ToString("00") + belgeTarih.Gun.ToString("00");
                string formDurumTarihddmmyyyy = f.formDurumTarih.Yil.ToString("0000") + f.formDurumTarih.Ay.ToString("00") + f.formDurumTarih.Gun.ToString("00");

                foreach (OdemeEmriMIFDetay d in f.detay.objeler)
                {
                    string e1;
                    string e2;
                    string e3;
                    string e4;

                    if (string.IsNullOrWhiteSpace(d.eko))
                    {
                        e1 = BolumAl(d.eko, 0);//23
                        e2 = BolumAl(d.eko, 1); //24 EKONOMİK /Y. HESAP KODU 2	STRING	
                        e3 = BolumAl(d.eko, 2); //25 EKONOMİK /Y. HESAP KODU 3	STRING	
                        e4 = BolumAl(d.eko, 3);//26 EKONOMİK /Y. HESAP KODU 4	STRING	
                    }
                    else
                    {
                        e1 = BolumAl(d.hesapKod, 1);//23
                        e2 = BolumAl(d.hesapKod, 2); //24 EKONOMİK /Y. HESAP KODU 2	STRING	
                        e3 = BolumAl(d.hesapKod, 3); //25 EKONOMİK /Y. HESAP KODU 3	STRING	
                        e4 = BolumAl(d.hesapKod, 4);//26 EKONOMİK /Y. HESAP KODU 4	STRING	
                    }

                    string ilgIBAN;
                    string ilgVNo;
                    string ilgBanka;

                    if (f.ilgiliBankaNo.StartsWith("tr", StringComparison.CurrentCultureIgnoreCase))
                    {
                        ilgIBAN = IBANDonustur(f.ilgiliBankaNo);
                        ilgVNo = VergiNoDonustur(f.ilgiliNo);
                        ilgBanka = f.ilgiliBankaAd;
                    }
                    else
                    {
                        ilgIBAN = "";
                        ilgVNo = "";
                        ilgBanka = "";
                    }
                    sw.WriteLine(string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}|{20}|{21}|{22}|{23}|{24}|{25}|{26}|{27}|{28}|{29}|{30}|{31}|{32}|{33}|{34}|{35}|{36}|{37}|{38}|{39}"
                        , belgeTarih.Yil.ToString("0000") // 1- MALİ YIL
                        , kurumAd //2- KURUM ADI 	 
                        , sayKurum //3	KURUM KODU 	 
                        , f.muhasebeAd //4	MUHASEBE BİRİMİNİN ADI	STRING	
                        , f.muhasebe //5	MUHASEBE BİRİMİNİN kodu	STRING	
                        , yevmiyeTarihStr //6	YEVMİYE TARİHİ	DATE	
                        , f.YEVYevNo.ToString() //7	YEVMİYE NUMARASI	STRING	
                        , d.hesapAd //8	HESAP ADI	STRING	
                        , d.hesapKod.Substring(0, 3) //9	HESAP KODU	STRING	
                        , f.YEVCekNo //10 çek gönderme No
                        , aciklama //11	ÖDEME EMRİ VEYA MUHASEBE İŞLEM FİŞİ AÇIKLAMA	STRING	Belge Düzenleme Nedeni
                        , BolumAl(d.kur, 0) //12	KURUMSAL  KODU 1	STRING	
                        , BolumAl(d.kur, 1)//13	KURUMSAL  KODU 2	STRING	
                        , BolumAl(d.kur, 2)//14	KURUMSAL  KODU 3	STRING	
                        , BolumAl(d.kur, 3)//15	KURUMSAL KODU 4	STRING	
                        , BolumAl(d.kur, 4) //16	KURUMSAL  KODU 5	STRING	
                        , f.birimAd //17 Harcama birimi adı
                        , BolumAl(d.fon, 0)//18	FONKSİYON KOD 1	STRING	
                        , BolumAl(d.fon, 1)//19	FONKSİYON KOD 2	STRING	
                        , BolumAl(d.fon, 2)//20	FONKSİYON KOD 3	STRING	
                        , BolumAl(d.fon, 3)//21	FONKSİYON KOD 4	STRING	

                        , BolumAl(d.fin, 0)//22	FİNANS KODU	STRING	

                        , e1//23 EKONOMİK /Y. HESAP KODU 1 STRING	
                        , e2//24 EKONOMİK /Y. HESAP KODU 2 STRING	
                        , e3//25 EKONOMİK /Y. HESAP KODU 3 STRING	
                        , e4//26 EKONOMİK /Y. HESAP KODU 4 STRING	

                        , d.borc.ToString("#,##0.00")  //27	BORÇ	NUMERIC	0,00 formatında  (“0,..” olmayacak )
                        , d.alacak.ToString("#,##0.00") //28	ALACAK	NUMERIC	0,00 formatında  (“0,..” olmayacak )

                        , "" //29 Büyük defterden açıklama
                        , "" //30 Kasa defteri alındı belgesi

                        , f.ilgiliAd //31	MUHASEBE İŞLEM FİŞİ/ÖDEME EMRİ BELGESİ/YARDIMCI DEFTERDEN İLGİLİNİN ADI, SOYADI	STRING	
                        , ilgVNo //32	MUHASEBE İŞLEM FİŞİ/ÖDEME EMRİ BELGESİ/YARDIMCI DEFTERDEN  İLGİLİNİN T.C./ VERGİ KİMLİK NO	STRING	
                        , f.ilgiliBankaAd //33	MUHASEBE İŞLEM FİŞİ/ÖDEME EMRİ BELGESİ/YARDIMCI DEFTERDEN  İLGİLİNİN BANKA ŞUBE ADI	STRING	İBAN varsa ayrı bir sütun açılacak
                        , ilgIBAN //34	MUHASEBE İŞLEM FİŞİ/ÖDEME EMRİ BELGESİ/YARDIMCI DEFTERDEN  İLGİLİNİN BANKA HESAP NUMARASI	STRING	İBAN varsa ayrı bir sütun açılacak
                        , ilgBanka // 35 vergi dairesi

                        , "" //36	MUHASEBE İŞLEM FİŞİNDEN İADE OLUNAN EMANETLERDE/ÖN ÖDEMELERDE/ALACAKLARDA VB. KİMİN ADINA KAYITLI OLDUĞU	STRING	
                        , "" //37	MUHASEBE İŞLEM FİŞİNDEN İADE OLUNAN EMANETLERDE/ÖN ÖDEMELERDE/ALACAKLARDA VB. AVANS VE KREDİNİN VERİLDİĞİ TARİH	DATE	
                        , ""//38	MUHASEBE İŞLEM FİŞİNDEN İADE OLUNAN EMANETLERDE/ÖN ÖDEMELERDE/ALACAKLARDA VB. YEVMİYE NUMARASI	STRING	

                        , f.projeNo // 39 taahhüt kart no
                        , BelgeTurdenSayistayBelgeTure(f.fisTur[0]) // 40 işlem tipi
                        ));
                }
            }
            sw.Flush();
            sw.Close();
            return sonucDosyaAd;
        }

        private string IBANDonustur(string ilgiliBankaNo)
        {
            string tut = ilgiliBankaNo.Replace(" ", "");
            if (tut.Length < 10) tut = "";
            return tut;
        }

        private string VergiNoDonustur(string ilgiliNo)
        {
            if (ilgiliNo.Length == 10 || ilgiliNo.Length == 11)
                return ilgiliNo;
            else
                return "";
        }

        private string SayistayKoduBul(string kurum)
        {
            if (kurum == "11.75")
                return "90.08.01.00"; //Ulusal Ajans.
            if (kurum.Length == 5)
                kurum += ".00.00";
            return kurum;
        }

        private string BelgeTurdenSayistayBelgeTure(ENUMFISTUR fisTur)
        {
            switch (fisTur)
            {
                case ENUMFISTUR.ACILIS: return "1";
                case ENUMFISTUR.DONEMICI: return "2";
                case ENUMFISTUR.KAPANIS: return "3";
                case ENUMFISTUR.AMORTISMANKAPANISI: return "3";
                case ENUMFISTUR.SONFIS: return "4";

            }
            return "2";
        }

        private string BolumAl(string kod, int bolNo)
        {
            return TNS.MUH.Arac.DiziElemanAl(kod.Split('.'), bolNo, "");
        }
    }

    /// <summary>
    /// Buyuk Defterx rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class BuyukDefterx
    {
        OrtakClass.Tablo XLS;
        IMUHServis s = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Buyuk Defterx raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);

            string basHesap = (string)raporParam["BasHesap"];//Büyük defterde 1 hesap olacak
            string bitHesap = (string)raporParam["BitHesap"];

            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            HesapPlaniSatir hs = new HesapPlaniSatir();
            hs.yil = yil;
            hs.seviye = 1;
            ObjectArray hp = s.HesapPlaniListele((TNS.KYM.Kullanici)kullanan, hs);
            List<string> hesaplar = new List<string>();

            foreach (HesapPlaniSatir hs1 in hp.objeler)
            {
                string hss = hs1.hesapKod.Replace(".", "").Substring(0, 3);
                bool bulundu = false;
                for (int i = 0; i < hesaplar.Count; i++)
                {
                    if (hesaplar[i] == hss)
                    {
                        bulundu = true;
                        break;
                    }
                }
                if (!bulundu)
                    hesaplar.Add(hss);
            }

            bool basla = false;
            bool bitir = false;
            for (int i = 0; i < hesaplar.Count; i++)
            {
                if (basHesap.Replace(".", "").StartsWith(hesaplar[i]))
                    basla = true;

                if (bitHesap.Replace(".", "").StartsWith(hesaplar[i]))
                    bitir = true;

                TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
                oEmri.yil = yil;
                //oEmri.kurum = kurum;
                oEmri.muhasebe = saymanlik;
                oEmri.birim = birim;
                if (tur == 1)
                {
                    oEmri.formDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
                    oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
                }
                else if (tur == 2)
                {
                    oEmri.formDurum = (int)ENUMBelgeDurumu.ONAYLI;
                    oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
                }
                else
                {
                    oEmri.formDurum = 22;
                    oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
                }
                oEmri.fisTurYap(fisTur);
                oEmri.sorguTarihSiraliMi = true;

                if (basla)
                {
                    oEmri.detay.objeler.Clear();
                    BuyukDefterIcerikYaz(XLS, kullanan, yil, tur, saymanlik, birim, hesaplar[i], oEmri, tarih1, tarih2);
                }

                if (bitir)
                    break;
            }

            XLS.SheetSil(0);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void BuyukDefterIcerikYaz(OrtakClass.Tablo XLS, object kullanan, int yil, int tur, int saymanlik, string birim, string hesap, TNS.MUH.OdemeEmriMIFForm oEmri, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2)
        {
            string saymanlikAd = "";
            string birimAd = "";
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

            TNS.MUH.OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(yil);
            oEmriDetay.hesapKod = hesap.Replace(".", "").Trim();
            oEmriDetay.kur = "";
            oEmriDetay.fon = "";
            oEmriDetay.fin = "";
            oEmriDetay.eko = "";
            oEmri.detay.objeler.Add(oEmriDetay);

            ObjectArray mb = s.OdemeEmriMIFListele((TNS.KYM.Kullanici)kullanan, oEmri, true);
            if (mb.objeler.Count == 0)
                return;

            XLS.YeniSheetEkle(0);

            //Devreden bulmak için
            bool devrendenYok = false;
            double devredenBorc = 0;
            double devredenAlacak = 0;
            double devrendenBakiye = 0;

            if (tarih1.Oku().AddDays(-1).Year == tarih1.Oku().Year)
            {
                if (tur == 1)
                    oEmri.sorguYEVYevTarih = "01.01." + tarih1.Yil.ToString() + ";" + tarih1.Oku().AddDays(-1).ToString();
                else
                    oEmri.sorguIslemTarih = "01.01." + tarih1.Yil.ToString() + ";" + tarih1.Oku().AddDays(-1).ToString();
                ObjectArray devredenmb = s.OdemeEmriMIFListele((TNS.KYM.Kullanici)kullanan, oEmri, true);

                foreach (OdemeEmriMIFForm f in devredenmb.objeler)
                {
                    foreach (OdemeEmriMIFDetay d in f.detay.objeler)
                    {
                        devredenBorc += d.borc;
                        devredenAlacak += d.alacak;
                        devrendenBakiye += d.borc - d.alacak;
                    }
                }
            }
            else
                devrendenYok = true;
            //**********************************

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            XLS.HucreDegerYaz(6, 1, " : " + kurum + " - " + kurumAd);
            XLS.HucreDegerYaz(9, 1, " : " + hesap);

            if (saymanlik > 0)
            {
                saymanlikAd = OrtakClass.GenelIslemler.KodAd(11, saymanlik.ToString(), true);
                XLS.HucreDegerYaz(7, 1, " : " + saymanlik + " - " + saymanlikAd);
            }
            else
                XLS.HucreDegerYaz(7, 1, " : Kurum Geneli");

            if (birim.Trim() != "")
            {
                birimAd = OrtakClass.GenelIslemler.KodAd(12, birim + "-" + saymanlik.ToString(), false);
                XLS.HucreDegerYaz(8, 1, " : " + birim + " - " + birimAd);
            }
            else
                XLS.HucreDegerYaz(8, 1, " : Kurum Geneli");

            XLS.HucreDegerYaz(6, 9, tarih1.ToString());
            XLS.HucreDegerYaz(7, 9, tarih2.ToString());
            int satir = 14;
            int sablonSatir = satir - 1;
            double tBorc = 0;
            double tAlacak = 0;
            string hesapAd = "";
            double bakiye = 0;
            double tmpBakiye = 0;
            string bakiyeStr = "";

            //Devreden
            if (!devrendenYok)
            {
                XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);
                XLS.HucreDegerYaz(satir, 6, "D E V R E D E N");

                XLS.HucreDegerYaz(satir, 7, devredenBorc);
                XLS.HucreDegerYaz(satir, 8, devredenAlacak);

                tmpBakiye = devrendenBakiye;
                bakiyeStr = "";
                if (tmpBakiye < 0)
                    bakiyeStr += Math.Abs(tmpBakiye).ToString("#,###.00") + " (A)";
                else if (tmpBakiye > 0)
                    bakiyeStr += Math.Abs(tmpBakiye).ToString("#,###.00") + " (B)";

                XLS.HucreDegerYaz(satir, 9, bakiyeStr);
                XLS.KoyuYap(satir, 0, satir, 15, true);
                satir++;
            }
            //************************************************

            bakiye = devrendenBakiye;

            foreach (OdemeEmriMIFForm f in mb.objeler)
            {
                string aciklama = f.aciklama;
                string bTur = "";
                if (f.belgeTur == ENUMBelgeTuru.ODEMEEMRI)
                    bTur = "ÖDE";
                else
                    bTur = "MİF";

                foreach (OdemeEmriMIFDetay d in f.detay.objeler)
                {
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);

                    if (tur == 1)
                    {
                        XLS.HucreDegerYaz(satir, 0, f.YEVYevTarih.ToString());
                        XLS.HucreDegerYaz(satir, 1, f.YEVYevNo);
                    }
                    else
                    {
                        XLS.HucreDegerYaz(satir, 0, f.islemTarih.ToString());
                        XLS.HucreDegerYaz(satir, 1, f.belgeNo + "-" + bTur);
                    }

                    if (!String.IsNullOrEmpty(d.kur))
                    {
                        XLS.HucreDegerYaz(satir, 2, d.kur);
                        XLS.HucreDegerYaz(satir, 3, d.fon);
                        XLS.HucreDegerYaz(satir, 4, d.fin);
                        XLS.HucreDegerYaz(satir, 5, d.eko);
                    }

                    if (d.hesapKod.Length > 3)
                        XLS.HucreDegerYaz(satir, 5, d.hesapKod.Substring(4));

                    XLS.HucreDegerYaz(satir, 6, aciklama);
                    XLS.HucreDegerYaz(satir, 7, d.borc);
                    XLS.HucreDegerYaz(satir, 8, d.alacak);

                    bakiye += (d.borc - d.alacak);

                    tmpBakiye = bakiye;
                    bakiyeStr = "";
                    if (tmpBakiye < 0)
                        bakiyeStr += Math.Abs(tmpBakiye).ToString("#,###.00") + " (A)";
                    else if (tmpBakiye > 0)
                        bakiyeStr += Math.Abs(tmpBakiye).ToString("#,###.00") + " (B)";

                    XLS.HucreDegerYaz(satir, 9, bakiyeStr);

                    tBorc += d.borc;
                    tAlacak += d.alacak;

                    satir++;
                }
            }

            tBorc += devredenBorc;
            tAlacak += devredenAlacak;

            hesapAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", yil.ToString() + "-" + hesap, false);
            XLS.HucreDegerYaz(9, 1, " : " + hesap + " - " + hesapAd);

            XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);
            XLS.HucreDegerYaz(satir, 6, "HESAP TOPLAMI");
            XLS.HucreDegerYaz(satir, 7, tBorc);
            XLS.HucreDegerYaz(satir, 8, tAlacak);

            tmpBakiye = tBorc - tAlacak;
            bakiyeStr = "";
            if (tmpBakiye < 0)
                bakiyeStr += Math.Abs(tmpBakiye).ToString("#,###.00") + " (A)";
            else if (tmpBakiye > 0)
                bakiyeStr += Math.Abs(tmpBakiye).ToString("#,###.00") + " (B)";

            XLS.HucreDegerYaz(satir, 9, bakiyeStr);
            XLS.KoyuYap(satir, 0, satir, 15, true);
        }
    }

    /// <summary>
    /// Buyuk Defter Detayx rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class BuyukDefterDetayx
    {
        OrtakClass.Tablo XLS;
        TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();

        /// <summary>
        /// Buyuk Defter Detayx raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);

            string basHesap = (string)raporParam["BasHesap"];
            string bitHesap = (string)raporParam["BitHesap"];

            if (basHesap.Replace(".", "").Length != bitHesap.Replace(".", "").Length)
                throw new Exception("Verilen hesap kodları aynı uzunlukta değildir.");

            if (basHesap.Replace(".", "").Substring(0, 3) != bitHesap.Replace(".", "").Substring(0, 3))
                throw new Exception("İki hesap koduda aynı hesap kodunun altında olmalıdır.");

            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            HesapPlaniSatir hs = new HesapPlaniSatir();
            hs.yil = yil;
            hs.hesapKod = basHesap.Replace(".", "");

            ObjectArray hp = s.HesapPlaniListele((TNS.KYM.Kullanici)kullanan, hs);
            if (hp.objeler.Count > 0)
            {
                HesapPlaniSatir hsx = (HesapPlaniSatir)hp.objeler[0];

                //if (basHesap.Replace(".", "") != hsx.hesapKod.Replace(".", "") || !hsx.detay)
                //    throw new Exception(basHesap + " detay hesap kodu değildir.");
            }
            else
                throw new Exception(string.Format("{0} hesap kodu bulunamadı.", basHesap));

            hs = new HesapPlaniSatir();
            hs.yil = yil;
            hs.hesapKod = bitHesap.Replace(".", "");
            hp = s.HesapPlaniListele((TNS.KYM.Kullanici)kullanan, hs);

            if (hp.objeler.Count > 0)
            {
                HesapPlaniSatir hsx = (HesapPlaniSatir)hp.objeler[0];

                //if (bitHesap.Replace(".", "") != hsx.hesapKod.Replace(".", "") || !hsx.detay)
                //    throw new Exception(bitHesap + " detay hesap kodu değildir.");
            }
            else
                throw new Exception(string.Format("{0} hesap kodu bulunamadı.", bitHesap));


            hs = new HesapPlaniSatir();
            hs.yil = yil;
            hs.detay = true;
            hs.hesapKod = basHesap.Replace(".", "");//.Substring(0, 3);

            hp = s.HesapPlaniListele((TNS.KYM.Kullanici)kullanan, hs);
            List<string> hesaplar = new List<string>();

            foreach (HesapPlaniSatir hs1 in hp.objeler)
            {
                string hss = hs1.hesapKod;
                bool bulundu = false;
                for (int i = 0; i < hesaplar.Count; i++)
                {
                    if (hesaplar[i] == hss)
                    {
                        bulundu = true;
                        break;
                    }
                }
                if (!bulundu)
                    hesaplar.Add(hss);
            }

            bool basla = false;
            bool bitir = false;
            int sayac = 0;
            for (int i = 0; i < hesaplar.Count; i++)
            {
                if (hesaplar[i].Replace(".", "").StartsWith(basHesap.Replace(".", "")))
                    basla = true;

                if (hesaplar[i].Replace(".", "").StartsWith(bitHesap.Replace(".", "")) &&
                    basHesap.Replace(".", "") != bitHesap.Replace(".", ""))
                {
                    bitir = true;
                    break;
                }

                if (basla)
                    sayac++;

                if (sayac > 200)
                    throw new Exception(string.Format("{0} ile {1} hesapları arasında 200 den fazla hesap bulundu. Lütfen aralığı küçültün.", basHesap, bitHesap));
            }

            basla = false;
            bitir = false;
            for (int i = 0; i < hesaplar.Count; i++)
            {
                if (hesaplar[i].Replace(".", "").StartsWith(basHesap.Replace(".", "")))
                    basla = true;

                if (hesaplar[i].Replace(".", "").StartsWith(bitHesap.Replace(".", "")) &&
                    basHesap.Replace(".", "") != bitHesap.Replace(".", ""))
                    bitir = true;

                TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
                oEmri.yil = yil;
                //oEmri.kurum = kurum;
                oEmri.muhasebe = saymanlik;
                oEmri.birim = birim;
                if (tur == 1)
                {
                    oEmri.formDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
                    oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
                }
                else if (tur == 2)
                {
                    oEmri.formDurum = (int)ENUMBelgeDurumu.ONAYLI;
                    oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
                }
                else
                {
                    oEmri.formDurum = 22;
                    oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
                }
                oEmri.fisTurYap(fisTur);
                oEmri.sorguTarihSiraliMi = true;

                if (basla)
                {
                    oEmri.detay.objeler.Clear();
                    //new BuyukDefter().BuyukDefterIcerikYaz(XLS, kullanan, yil, tur, saymanlik, birim, hesaplar[i], oEmri, tarih1, tarih2);
                }

                if (bitir)
                    break;
            }
            if (XLS.AktifSheet() > 0)
                XLS.SheetSil(0);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    /// <summary>
    /// Buyuk Defter rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class BuyukDefter
    {
        OrtakClass.Tablo XLS;
        TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();

        /// <summary>
        /// Buyuk Defter raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            string raporTur = TNS.OrtakFonksiyonlar.ConvertToStr(raporParam["RaporTur"]);
            int listeTur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["ListeTur"], 0);
            int hesapAdiAyni = TNS.OrtakFonksiyonlar.ConvertToInt((bool)raporParam["HesapAdiAyniOlanlariListele"], 0);
            string kurKod = (string)raporParam["KurKod"];
            string fonKod = (string)raporParam["FonKod"];

            string basHesap = (string)raporParam["BasHesap"];
            string bitHesap = (string)raporParam["BitHesap"];

            if (raporTur.StartsWith("49"))
            {
                bitHesap = basHesap + "z";
            }

            if (hesapAdiAyni > 0)
            {
                //Hesabın adını bul
                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.yil = yil;
                hs.hesapKod = basHesap;
                ObjectArray yler = s.HesapPlaniListele((TNS.KYM.Kullanici)kullanan, hs);
                foreach (HesapPlaniSatir y in yler.objeler)
                {
                    hs.hesapKod = "";
                    hs.aciklama = y.aciklama;
                }

                //Hesabın adını vererek uyan diğer hesap kodlarını bul ve basHesap alanına | ile ekle
                yler = s.HesapPlaniListele((TNS.KYM.Kullanici)kullanan, hs);
                basHesap = "";
                foreach (HesapPlaniSatir y in yler.objeler)
                {
                    if (basHesap != "") basHesap += "|";
                    basHesap += y.hesapKod;
                }
            }

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            if (tur == 1)
            {
                oEmri.formDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
                oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else if (tur == 2)
            {
                oEmri.formDurum = (int)ENUMBelgeDurumu.ONAYLI;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else
            {
                oEmri.formDurum = 22;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            oEmri.fisTurYap(fisTur);
            oEmri.sorguEkoSiraliMi = true;

            if (listeTur == 2)
                oEmri.sorguDovizliIslemler = true;

            TNS.MUH.OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(yil);
            if (hesapAdiAyni == 0)
                oEmriDetay.hesapKod = basHesap.Replace(".", "").Trim() + ";" + bitHesap.Replace(".", "").Trim();
            else
                oEmriDetay.hesapKod = basHesap.Replace(".", "").Trim();

            oEmriDetay.kur = kurKod;
            oEmriDetay.fon = fonKod;
            oEmriDetay.fin = "";
            oEmriDetay.eko = "";
            oEmri.detay.objeler.Add(oEmriDetay);

            int sayac = 1;
            int alinacakBilgi = 30000;

            ObjectArray mbler = new ObjectArray();

            //alinacakBilgi miktarı kadar serverdan bilgi al ve bilgi satır sayısı 0 olduğunda döndüden çık
            for (int i = 0; i < 30; i++)
            {
                TNS.MUH.BuyukDefter[] mb = s.BuyukDefterListele((TNS.KYM.Kullanici)kullanan, oEmri, sayac, sayac + alinacakBilgi, raporTur);
                sayac += alinacakBilgi + 1;

                if (mb.Length == 0)
                    break;

                for (int ii = 0; ii < mb.Length; ii++)
                {
                    TNS.MUH.BuyukDefter d = (TNS.MUH.BuyukDefter)mb[ii];
                    mbler.objeler.Add(d);
                }
            }

            bool detay = false;
            bool herHesapYeniSayfada = false;
            if (raporTur.StartsWith("4"))
                detay = true;
            if (raporTur.StartsWith("49"))
                detay = true;
            if (raporTur.StartsWith("10"))
                herHesapYeniSayfada = true;

            if (yil == 0)
                yil = tarih2.Yil;

            if ((string)raporParam["RaporTur"] == "10.1")
            {
                sonucDosyaAd = sonucDosyaAd + ".txt";

                BuyukDefterDosyayaYaz(XLS, kullanan, yil, tur, saymanlik, birim, mbler, oEmri, tarih1, tarih2, detay, herHesapYeniSayfada, raporTur, sonucDosyaAd);
            }
            else if ((string)raporParam["RaporTur"] == "49")
            {
                XLS = GenelIslemler.NewTablo();
                sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
                XLS.DosyaAc(sablonAd, sonucDosyaAd);

                BuyukDefterListeIcerikYaz(XLS, kullanan, yil, tur, saymanlik, birim, mbler, oEmri, tarih1, tarih2, detay, herHesapYeniSayfada, raporTur, listeTur);

                XLS.DosyaSaklaTamYol();
            }
            else
            {
                XLS = GenelIslemler.NewTablo();
                sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
                XLS.DosyaAc(sablonAd, sonucDosyaAd);

                BuyukDefterIcerikYaz(XLS, kullanan, yil, tur, saymanlik, birim, mbler, oEmri, tarih1, tarih2, detay, herHesapYeniSayfada, raporTur, listeTur);

                XLS.DosyaSaklaTamYol();
            }
            return sonucDosyaAd;
        }

        private void BuyukDefterIcerikYaz(OrtakClass.Tablo XLS, object kullanan, int yil, int tur, int saymanlik, string birim, ObjectArray mb, TNS.MUH.OdemeEmriMIFForm oEmri, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2, bool detay, bool herHesapYeniSayfada, string raporTur, int listeTur)
        {
            IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

            int sablonSatir = 0;
            int hesapSablonSatir = 0;

            int sablonSutun = 0;
            double tBorc = 0;
            double tAlacak = 0;
            string eskiHesap = "";

            XLS.HucreAdAdresCoz("BaslaHucre", ref sablonSatir, ref sablonSutun);
            XLS.HucreAdAdresCoz("HesapAdi", ref hesapSablonSatir, ref sablonSutun);

            int ilkSatir = sablonSatir + 1;
            int satir = ilkSatir;

            if (raporTur.StartsWith("10")) //Defteri kebir
            {
                XLS.HucreDegerYaz(0, 0, "DEFTER-İ KEBİR");
            }

            if (mb.objeler.Count > 0)
            {
                TNS.MUH.BuyukDefter ff = (TNS.MUH.BuyukDefter)mb.objeler[0];
                BaslikYaz(XLS, saymanlik, birim, tarih1, tarih2, listeTur, ff);
            }

            string kontrolHesapKod = "";
            foreach (TNS.MUH.BuyukDefter f in mb.objeler)
            {
                if (raporTur.StartsWith("4"))
                    kontrolHesapKod = f.hesapKod;
                else
                    kontrolHesapKod = f.hesapKod.Substring(0, 3);

                if (eskiHesap != "" && eskiHesap != kontrolHesapKod)
                {
                    if (tBorc > 0 || tAlacak > 0)
                    {
                        XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 16, satir, 0);
                        XLS.HucreDegerYaz(satir, 8, "HESAP TOPLAMI");
                        XLS.HucreDegerYaz(satir, 9, tBorc);
                        XLS.HucreDegerYaz(satir, 10, tAlacak);
                        XLS.HucreDegerYaz(satir, 11, BakiyeStringUret(tBorc - tAlacak));
                        XLS.KoyuYap(satir, 0, satir, 16, true);
                        satir++;
                    }
                }

                if (eskiHesap != kontrolHesapKod)//Rapor ilk başlarken de yazılmalı
                {
                    tBorc = 0;
                    tAlacak = 0;

                    string hesapKodu = kontrolHesapKod;
                    string hesapAdi = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", f.yil.ToString() + "-" + hesapKodu, true);

                    XLS.HucreKopyala(hesapSablonSatir, 0, hesapSablonSatir, 16, satir, 0);
                    XLS.HucreDegerYaz(satir, 0, hesapKodu + " - " + hesapAdi);
                    satir++;

                    //*************
                    #region DEVREDEN
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 16, satir, 0);
                    XLS.HucreDegerYaz(satir, 8, "DEVREDEN");
                    XLS.KoyuYap(satir, 0, satir, 16, true);

                    if (tarih1.Gun == 1 && tarih1.Ay == 1)
                    {
                        tBorc = 0;
                        tAlacak = 0;
                    }
                    else
                    {
                        if (oEmri.formDurum == (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
                            oEmri.sorguYEVYevTarih = "01.01." + yil.ToString() + ";" + tarih1.Oku().AddDays(-1).ToString();
                        else
                            oEmri.sorguIslemTarih = "01.01." + yil.ToString() + ";" + tarih1.Oku().AddDays(-1).ToString();

                        ((TNS.MUH.OdemeEmriMIFDetay)oEmri.detay[0]).hesapKod = kontrolHesapKod;

                        double[] dev = s.BuyukDefterDevreden((TNS.KYM.Kullanici)kullanan, oEmri);
                        if (dev.Length > 1)
                        {
                            tBorc = dev[0];
                            tAlacak = dev[1];
                        }
                        else
                        {
                            tBorc = 0;
                            tAlacak = 0;
                        }
                    }

                    XLS.HucreDegerYaz(satir, 9, tBorc);
                    XLS.HucreDegerYaz(satir, 10, tAlacak);
                    XLS.HucreDegerYaz(satir, 11, BakiyeStringUret(tBorc - tAlacak));
                    satir++;
                    #endregion
                    //*****************
                }

                string aciklama = f.aciklama;
                string bTur = "";
                if (f.belgeTur == (int)ENUMBelgeTuru.ODEMEEMRI)
                    bTur = "ÖDE";
                else
                    bTur = "MİF";

                XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);

                if (tur == 1)
                {
                    XLS.HucreDegerYaz(satir, 0, f.YEVYevTarih.ToString());
                    XLS.HucreDegerYaz(satir, 1, f.YEVYevNo);
                }
                else
                {
                    XLS.HucreDegerYaz(satir, 0, f.islemTarih.ToString());
                    XLS.HucreDegerYaz(satir, 1, f.belgeNo + "-" + bTur);
                }

                XLS.HucreDegerYaz(satir, 2, f.detayBirim);

                if (!String.IsNullOrEmpty(f.kur))
                {
                    XLS.HucreDegerYaz(satir, 3, f.kur);
                    XLS.HucreDegerYaz(satir, 4, f.fon);
                    XLS.HucreDegerYaz(satir, 5, f.fin);
                    XLS.HucreDegerYaz(satir, 6, f.eko);
                }

                XLS.HucreDegerYaz(satir, 6, f.hesapKod);
                XLS.HucreDegerYaz(satir, 7, f.altHesap + "-" + f.altHesapAd);

                tBorc += f.borc;
                tAlacak += f.alacak;
                XLS.HucreDegerYaz(satir, 8, aciklama);
                XLS.HucreDegerYaz(satir, 9, f.borc);
                XLS.HucreDegerYaz(satir, 10, f.alacak);
                XLS.HucreDegerYaz(satir, 11, BakiyeStringUret(tBorc - tAlacak));

                eskiHesap = kontrolHesapKod;
                satir++;
            }

            XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 16, satir, 0);
            XLS.HucreDegerYaz(satir, 8, "HESAP TOPLAMI");
            XLS.HucreDegerYaz(satir, 9, tBorc);
            XLS.HucreDegerYaz(satir, 10, tAlacak);
            XLS.HucreDegerYaz(satir, 11, BakiyeStringUret(tBorc - tAlacak));
            XLS.KoyuYap(satir, 0, satir, 16, true);
        }

        private void BuyukDefterListeIcerikYaz(OrtakClass.Tablo XLS, object kullanan, int yil, int tur, int saymanlik, string birim, ObjectArray mb, TNS.MUH.OdemeEmriMIFForm oEmri, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2, bool detay, bool herHesapYeniSayfada, string raporTur, int listeTur)
        {
            int ilkSatir = 12;
            int satir = ilkSatir;
            int sablonSatir = 0;
            int sablonSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref sablonSatir, ref sablonSutun);

            Hashtable htBirim = new Hashtable();

            bool baslikYaz = true;
            string eskiYevmiye = "";
            TNS.MUH.BuyukDefter satirHesap = new TNS.MUH.BuyukDefter();

            foreach (TNS.MUH.BuyukDefter f in mb.objeler)
            {
                if (baslikYaz)
                {
                    if (htBirim[f.muhasebe.ToString() + f.birim + f.kur] == null)
                    {
                        f.birimAd = OrtakClass.GenelIslemler.KodAd(12, f.birim + "-" + f.muhasebe.ToString() + "-" + f.kur, false);
                        htBirim[f.muhasebe.ToString() + f.birim + f.kur] = f.birimAd;
                    }
                    else
                        f.birimAd = (string)htBirim[f.muhasebe.ToString() + f.birim + f.kur];

                    string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
                    string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

                    XLS.HucreAdBulYaz("KurumAd", " : " + kurum + " - " + kurumAd);

                    if (saymanlik > 0)
                        XLS.HucreAdBulYaz("SaymanlikAd", " : " + saymanlik + " - " + f.muhasebeAd);
                    else
                        XLS.HucreAdBulYaz("SaymanlikAd", " : Kurum Geneli");

                    if (birim.Trim() != "")
                        XLS.HucreAdBulYaz("BirimAdi", " : " + birim + " - " + f.birimAd);
                    else
                        XLS.HucreAdBulYaz("BirimAdi", " : Kurum Geneli");

                    XLS.HucreAdBulYaz("Bastar", tarih1.ToString());
                    XLS.HucreAdBulYaz("Bittar", tarih2.ToString());

                    string paraBirimi = ButceMuhasebeGenel.DondurParaBirimi(1).kisaltma;
                    if (listeTur == 2) paraBirimi = "DÖVİZ";
                    XLS.HucreAdBulYaz("ParaBirimi", paraBirimi);

                    baslikYaz = false;
                }

                if (eskiYevmiye != f.belgeNo && eskiYevmiye != "")
                {
                    BuyukDefterListeSatirYaz(satir, sablonSatir, tur, satirHesap);
                    satirHesap = new TNS.MUH.BuyukDefter();
                    satir++;
                }

                satirHesap.aciklama = f.aciklama;
                satirHesap.belgeTur = f.belgeTur;
                satirHesap.YEVYevTarih = f.YEVYevTarih;
                satirHesap.YEVYevNo = f.YEVYevNo;
                satirHesap.islemTarih = f.islemTarih;
                satirHesap.belgeNo = f.belgeNo;
                satirHesap.hesapKod = f.hesapKod;
                satirHesap.ilgiliAd = f.ilgiliAd;
                satirHesap.ilgiliNo = f.ilgiliNo;
                satirHesap.kur = f.kur;
                satirHesap.fon = f.fon;
                satirHesap.fin = f.fin;
                satirHesap.eko = f.eko;
                satirHesap.borc += f.borc;
                satirHesap.alacak += f.alacak;
                satirHesap.detayBirim = f.detayBirim;

                eskiYevmiye = f.belgeNo;
            }

            BuyukDefterListeSatirYaz(satir, sablonSatir, tur, satirHesap);
        }

        void BuyukDefterListeSatirYaz(int satir, int sablonSatir, int tur, TNS.MUH.BuyukDefter f)
        {
            string aciklama = f.aciklama;
            string bTur = "";
            if (f.belgeTur == (int)ENUMBelgeTuru.ODEMEEMRI)
                bTur = "ÖDE";
            else
                bTur = "MİF";

            XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);

            if (tur == 1)
            {
                XLS.HucreDegerYaz(satir, 0, f.YEVYevTarih.ToString());
                XLS.HucreDegerYaz(satir, 1, f.YEVYevNo);
            }
            else
            {
                XLS.HucreDegerYaz(satir, 0, f.islemTarih.ToString());
                XLS.HucreDegerYaz(satir, 1, f.belgeNo + "-" + bTur);
            }

            XLS.HucreDegerYaz(satir, 2, f.detayBirim);

            XLS.HucreDegerYaz(satir, 3, f.hesapKod);
            XLS.HucreDegerYaz(satir, 4, f.ilgiliNo + "-" + f.ilgiliAd);
            XLS.HucreDegerYaz(satir, 5, aciklama);

            if (!String.IsNullOrEmpty(f.kur))
            {
                string tertip = f.kur;
                if (f.fon != "") tertip += "-" + f.fon;
                if (f.fin != "") tertip += "-" + f.fin;
                if (f.eko != "") tertip += "-" + f.eko;
                XLS.HucreDegerYaz(satir, 6, tertip);
            }

            //tBorc += f.borc;
            //tAlacak += f.alacak;
            XLS.HucreDegerYaz(satir, 7, f.borc);
            XLS.HucreDegerYaz(satir, 8, f.alacak);
            XLS.HucreDegerYaz(satir, 9, BakiyeStringUret(f.borc - f.alacak));
        }

        void BaslikYaz(OrtakClass.Tablo XLS, int saymanlik, string birim, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2, int listeTur, TNS.MUH.BuyukDefter f)
        {
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

            XLS.HucreAdBulYaz("KurumAd", " : " + kurum + " - " + kurumAd);
            if (saymanlik > 0)
                XLS.HucreAdBulYaz("SaymanlikAd", " : " + saymanlik + " - " + f.muhasebeAd);
            else
                XLS.HucreAdBulYaz("SaymanlikAd", " : Kurum Geneli");


            if (birim.Trim() != "")
                XLS.HucreAdBulYaz("BirimAdi", " : " + birim + " - " + f.birimAd);
            else
                XLS.HucreAdBulYaz("BirimAdi", " : Kurum Geneli");

            XLS.HucreAdBulYaz("Bastar", " : " + tarih1.ToString());
            XLS.HucreAdBulYaz("Bittar", " : " + tarih2.ToString());

            string paraBirimi = ButceMuhasebeGenel.DondurParaBirimi(1).kisaltma;
            if (listeTur == 2) paraBirimi = "DÖVİZ";
            XLS.HucreAdBulYaz("ParaBirimi", paraBirimi);
        }

        private string BakiyeStringUret(double tmpBakiye)
        {
            string bakiyeStr = "";
            if (tmpBakiye < 0)
                bakiyeStr += Math.Abs(tmpBakiye).ToString("#,##0.00") + " (A)";
            else if (tmpBakiye > 0)
                bakiyeStr += Math.Abs(tmpBakiye).ToString("#,##0.00") + " (B)";
            else
                bakiyeStr += Math.Abs(tmpBakiye).ToString("0.00");
            return bakiyeStr;
        }

        private void BuyukDefterDosyayaYaz(OrtakClass.Tablo XLS, object kullanan, int yil, int tur, int saymanlik, string birim, ObjectArray mb, TNS.MUH.OdemeEmriMIFForm oEmri, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2, bool detay, bool herHesapYeniSayfada, string raporTur, string sonucDosyaAd)
        {
            StringBuilder rapor = new StringBuilder();
            double tBorc = 0;
            double tAlacak = 0;
            int sayfadakiSatir = 66;
            int sayfaNo = 0;
            int satir = 1;
            string eskiHesap = "";

            string kolon = "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}";
            string baslik = "Tarih  Numara     Kurumsal    Fonksiyon F Ekonomik           Açıklama     Borç            Alacak          Bakiye\r\n";
            baslik += "------ ---------- ----------- --------- - ------------------ ------------ --------------- --------------- -----------------";

            string raporAdi = "DEFTER-İ KEBİR";
            string kurumAdi = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            bool yeniSayfaYap = false;

            //for (int i = 0; i < mb.Length; i++)
            //{
            //    TNS.MUH.BuyukDefter f = mb[i];
            foreach (TNS.MUH.BuyukDefter f in mb.objeler)
            {
                string kolon0 = "";
                string kolon1 = "";
                string kolon2 = "";
                string kolon3 = "";
                string kolon4 = "";
                string kolon5 = "";
                string kolon6 = "";
                string kolon7 = "";
                string kolon8 = "";
                string kolon9 = "";

                string anaHesap = f.hesapKod.Substring(0, 3);
                if (eskiHesap != anaHesap)
                {
                    if (satir + 6 > sayfadakiSatir || eskiHesap == "")
                        yeniSayfaYap = true;
                    else
                    {
                        string anaHesapAdi = OrtakClass.GenelIslemler.KodAd(13, DateTime.Now.Year + "-" + anaHesap, true);
                        rapor.AppendLine("");
                        rapor.AppendLine("Hesap Kodu : " + anaHesap + " - " + anaHesapAdi);
                        rapor.AppendLine(baslik);
                        satir += 4;
                    }
                }
                if (satir > sayfadakiSatir || yeniSayfaYap)
                {
                    yeniSayfaYap = false;
                    if (eskiHesap != "")
                    {
                        for (int ii = satir; ii <= sayfadakiSatir; ii++)
                            rapor.AppendLine("");
                    }
                    sayfaNo++;
                    BaslikYazDosya(rapor, raporAdi, kurumAdi, sayfaNo, anaHesap, baslik);
                    satir = 7;
                }
                f.aciklama = f.aciklama.Replace('\r', ' ');
                f.aciklama = f.aciklama.Replace('\n', ' ');
                f.aciklama = f.aciklama.Trim();
                if (f.aciklama.Length > 12)
                    kolon6 = f.aciklama.Substring(0, 12).PadRight(12);
                else
                    kolon6 = f.aciklama.PadRight(12);

                string bTur = "";
                if (f.belgeTur == (int)ENUMBelgeTuru.ODEMEEMRI)
                    bTur = "ÖDE";
                else
                    bTur = "MİF";

                if (tur == 1)
                {
                    kolon0 = f.YEVYevTarih.Gun.ToString("00") + f.YEVYevTarih.Ay.ToString("00") + f.YEVYevTarih.Yil.ToString().Substring(2, 2);
                    kolon1 = f.YEVYevNo.ToString().PadRight(10);
                }
                else
                {
                    kolon0 = f.islemTarih.Gun.ToString("00") + f.islemTarih.Ay.ToString("00") + f.islemTarih.Yil.ToString().Substring(2, 2);
                    kolon1 = (f.belgeNo.Substring(6) + "-" + bTur).PadRight(10);
                }

                if (!String.IsNullOrEmpty(f.kur))
                {
                    kolon2 = f.kur.PadRight(11);
                    kolon3 = f.fon.PadRight(9);
                    kolon4 = f.fin.PadRight(1);
                    kolon5 = f.eko.PadRight(18);
                }
                else
                {
                    kolon2 = "".PadRight(11);
                    kolon3 = "".PadRight(9);
                    kolon4 = "".PadRight(1);
                    kolon5 = "".PadRight(18);
                }

                if (f.hesapKod.Length > 3)
                    kolon5 = f.hesapKod.Substring(4).PadRight(18);

                tBorc += f.borc;
                tAlacak += f.alacak;

                kolon7 = f.borc.ToString("#,###.00").PadLeft(15);
                kolon8 = f.alacak.ToString("#,###.00").PadLeft(15);
                kolon9 = BakiyeStringUret(tBorc - tAlacak).PadLeft(17);

                string kolonDeger = string.Format(kolon, kolon0, kolon1, kolon2, kolon3, kolon4, kolon5, kolon6, kolon7, kolon8, kolon9);
                rapor.AppendLine(kolonDeger);

                eskiHesap = anaHesap;
                satir++;
            }
            OrtakFonksiyonlar.DosyaYaz(sonucDosyaAd, rapor.ToString(), true);
        }

        void BaslikYazDosya(StringBuilder rapor, string raporAdi, string kurumAdi, int sayfaNo, string anaHesap, string baslik)
        {
            rapor.AppendLine("".PadLeft(62 - (raporAdi.Length / 2)) + raporAdi);
            rapor.AppendLine("".PadLeft(62 - (kurumAdi.Length / 2)) + kurumAdi);
            rapor.AppendLine("Sayfa No   : " + sayfaNo);

            string anaHesapAdi = OrtakClass.GenelIslemler.KodAd(13, DateTime.Now.Year + "-" + anaHesap, true);
            rapor.AppendLine("Hesap Kodu : " + anaHesap + " - " + anaHesapAdi);
            rapor.AppendLine(baslik);
        }
    }

    /// <summary>
    /// Buyuk Defter rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class BuyukDefterAltHesap
    {
        OrtakClass.Tablo XLS;
        TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();

        /// <summary>
        /// Buyuk Defter raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            string raporTur = TNS.OrtakFonksiyonlar.ConvertToStr(raporParam["RaporTur"]);
            int listeTur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["ListeTur"], 0);
            string kurKod = (string)raporParam["KurKod"];
            string fonKod = (string)raporParam["FonKod"];
            string altHesap = (string)raporParam["AltHesap"];

            string basHesap = (string)raporParam["BasHesap"];
            string bitHesap = (string)raporParam["BitHesap"];

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            if (tur == 1)
            {
                oEmri.formDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
                oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else if (tur == 2)
            {
                oEmri.formDurum = (int)ENUMBelgeDurumu.ONAYLI;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else
            {
                oEmri.formDurum = 22;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            oEmri.fisTurYap(fisTur);
            oEmri.sorguEkoSiraliMi = true;

            if (listeTur == 2)
                oEmri.sorguDovizliIslemler = true;

            TNS.MUH.OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(yil);
            oEmriDetay.hesapKod = basHesap.Replace(".", "").Trim() + ";" + bitHesap.Replace(".", "").Trim();

            oEmriDetay.kur = kurKod;
            oEmriDetay.fon = fonKod;
            oEmriDetay.fin = "";
            oEmriDetay.eko = "";
            oEmriDetay.altHesapKod = altHesap;
            oEmriDetay.sadeceAltHesaptanKontrol = true;

            if (oEmriDetay.altHesapKod == "")
                oEmriDetay.altHesapKod = "-999-";

            oEmri.detay.objeler.Add(oEmriDetay);

            int sayac = 1;
            int alinacakBilgi = 30000;

            ObjectArray mbler = new ObjectArray();

            //alinacakBilgi miktarı kadar serverdan bilgi al ve bilgi satır sayısı 0 olduğunda döndüden çık
            for (int i = 0; i < 30; i++)
            {
                TNS.MUH.BuyukDefter[] mb = s.BuyukDefterListele((TNS.KYM.Kullanici)kullanan, oEmri, sayac, sayac + alinacakBilgi, raporTur);
                sayac += alinacakBilgi + 1;

                if (mb.Length == 0)
                    break;

                for (int ii = 0; ii < mb.Length; ii++)
                {
                    TNS.MUH.BuyukDefter d = (TNS.MUH.BuyukDefter)mb[ii];
                    mbler.objeler.Add(d);
                }
            }

            bool detay = false;
            bool herHesapYeniSayfada = false;
            detay = true;

            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            if (yil == 0)
                yil = tarih2.Yil;

            BuyukDefterIcerikYaz(XLS, kullanan, yil, tur, saymanlik, birim, mbler, oEmri, tarih1, tarih2, detay, herHesapYeniSayfada, raporTur, listeTur);

            XLS.DosyaSaklaTamYol();

            return sonucDosyaAd;
        }

        private void BuyukDefterIcerikYaz(OrtakClass.Tablo XLS, object kullanan, int yil, int tur, int saymanlik, string birim, ObjectArray mb, TNS.MUH.OdemeEmriMIFForm oEmri, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2, bool detay, bool herHesapYeniSayfada, string raporTur, int listeTur)
        {
            IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

            int sablonSatir = 0;
            int hesapSablonSatir = 0;
            int altHesapSablonSatir = 0;
            int baslikSablonSatir = 0;

            int sablonSutun = 0;
            double tBorc = 0;
            double tAlacak = 0;
            string eskiHesap = "";

            XLS.HucreAdAdresCoz("BaslaHucre", ref sablonSatir, ref sablonSutun);
            XLS.HucreAdAdresCoz("HesapAdi", ref hesapSablonSatir, ref sablonSutun);
            XLS.HucreAdAdresCoz("AltHesapAdi", ref altHesapSablonSatir, ref sablonSutun);
            XLS.HucreAdAdresCoz("BaslikSatir", ref baslikSablonSatir, ref sablonSutun);

            XLS.SatirYukseklikAyarla(baslikSablonSatir, baslikSablonSatir, 0);
            XLS.SatirYukseklikAyarla(sablonSatir, sablonSatir, 0);

            int ilkSatir = sablonSatir + 1;
            int satir = ilkSatir;

            if (mb.objeler.Count > 0)
            {
                TNS.MUH.BuyukDefter ff = (TNS.MUH.BuyukDefter)mb.objeler[0];
                BaslikYaz(XLS, saymanlik, birim, tarih1, tarih2, listeTur, ff);
            }

            string kontrolHesapKod = "";
            foreach (TNS.MUH.BuyukDefter f in mb.objeler)
            {
                kontrolHesapKod = f.hesapKod + "-" + f.altHesap;

                if (eskiHesap != "" && eskiHesap != kontrolHesapKod)
                {
                    if (tBorc > 0 || tAlacak > 0)
                    {
                        XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 16, satir, 0);
                        XLS.HucreDegerYaz(satir, 8, "HESAP TOPLAMI");
                        XLS.HucreDegerYaz(satir, 9, tBorc);
                        XLS.HucreDegerYaz(satir, 10, tAlacak);
                        XLS.HucreDegerYaz(satir, 11, BakiyeStringUret(tBorc - tAlacak));
                        XLS.KoyuYap(satir, 0, satir, 16, true);
                        satir++;
                        satir++;
                    }
                }

                if (eskiHesap != kontrolHesapKod)//Rapor ilk başlarken de yazılmalı
                {
                    tBorc = 0;
                    tAlacak = 0;

                    string hesapKodu = kontrolHesapKod;
                    string hesapAdi = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", f.yil.ToString() + "-" + hesapKodu, true);

                    XLS.HucreKopyala(hesapSablonSatir, 0, hesapSablonSatir, 16, satir, 0);
                    XLS.HucreDegerYaz(satir, 0, hesapKodu + " - " + hesapAdi);
                    satir++;

                    XLS.HucreKopyala(altHesapSablonSatir, 0, altHesapSablonSatir, 16, satir, 0);
                    XLS.HucreDegerYaz(satir, 0, f.altHesap + " - " + f.altHesapAd);
                    satir++;

                    XLS.HucreKopyala(baslikSablonSatir, 0, baslikSablonSatir, 16, satir, 0);
                    satir++;

                    //*************
                    #region DEVREDEN
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 16, satir, 0);
                    XLS.HucreDegerYaz(satir, 8, "DEVREDEN");
                    XLS.KoyuYap(satir, 0, satir, 16, true);

                    if (tarih1.Gun == 1 && tarih1.Ay == 1)
                    {
                        tBorc = 0;
                        tAlacak = 0;
                    }
                    else
                    {
                        if (oEmri.formDurum == (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
                            oEmri.sorguYEVYevTarih = "01.01." + yil.ToString() + ";" + tarih1.Oku().AddDays(-1).ToString();
                        else
                            oEmri.sorguIslemTarih = "01.01." + yil.ToString() + ";" + tarih1.Oku().AddDays(-1).ToString();

                        string devirHesapKod = kontrolHesapKod.Split('-')[0];
                        ((TNS.MUH.OdemeEmriMIFDetay)oEmri.detay[0]).hesapKod = devirHesapKod;

                        double[] dev = s.BuyukDefterDevreden((TNS.KYM.Kullanici)kullanan, oEmri);
                        if (dev.Length > 1)
                        {
                            tBorc = dev[0];
                            tAlacak = dev[1];
                        }
                        else
                        {
                            tBorc = 0;
                            tAlacak = 0;
                        }
                    }

                    XLS.HucreDegerYaz(satir, 9, tBorc);
                    XLS.HucreDegerYaz(satir, 10, tAlacak);
                    XLS.HucreDegerYaz(satir, 11, BakiyeStringUret(tBorc - tAlacak));
                    satir++;
                    #endregion
                    //*****************
                }

                string aciklama = f.aciklama;
                string bTur = "";
                if (f.belgeTur == (int)ENUMBelgeTuru.ODEMEEMRI)
                    bTur = "ÖDE";
                else
                    bTur = "MİF";

                XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);

                if (tur == 1)
                {
                    XLS.HucreDegerYaz(satir, 0, f.YEVYevTarih.ToString());
                    XLS.HucreDegerYaz(satir, 1, f.YEVYevNo);
                }
                else
                {
                    XLS.HucreDegerYaz(satir, 0, f.islemTarih.ToString());
                    XLS.HucreDegerYaz(satir, 1, f.belgeNo + "-" + bTur);
                }

                XLS.HucreDegerYaz(satir, 2, f.detayBirim);

                if (!String.IsNullOrEmpty(f.kur))
                {
                    XLS.HucreDegerYaz(satir, 3, f.kur);
                    XLS.HucreDegerYaz(satir, 4, f.fon);
                    XLS.HucreDegerYaz(satir, 5, f.fin);
                    XLS.HucreDegerYaz(satir, 6, f.eko);
                }

                XLS.HucreDegerYaz(satir, 7, f.altHesap + "-" + f.altHesapAd);

                tBorc += f.borc;
                tAlacak += f.alacak;
                XLS.HucreDegerYaz(satir, 8, aciklama);
                XLS.HucreDegerYaz(satir, 9, f.borc);
                XLS.HucreDegerYaz(satir, 10, f.alacak);
                XLS.HucreDegerYaz(satir, 11, BakiyeStringUret(tBorc - tAlacak));

                eskiHesap = kontrolHesapKod;
                satir++;
            }

            XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 16, satir, 0);
            XLS.HucreDegerYaz(satir, 8, "HESAP TOPLAMI");
            XLS.HucreDegerYaz(satir, 9, tBorc);
            XLS.HucreDegerYaz(satir, 10, tAlacak);
            XLS.HucreDegerYaz(satir, 11, BakiyeStringUret(tBorc - tAlacak));
            XLS.KoyuYap(satir, 0, satir, 16, true);
        }

        void BaslikYaz(OrtakClass.Tablo XLS, int saymanlik, string birim, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2, int listeTur, TNS.MUH.BuyukDefter f)
        {
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

            XLS.HucreAdBulYaz("KurumAd", " : " + kurum + " - " + kurumAd);
            if (saymanlik > 0)
                XLS.HucreAdBulYaz("SaymanlikAd", " : " + saymanlik + " - " + f.muhasebeAd);
            else
                XLS.HucreAdBulYaz("SaymanlikAd", " : Kurum Geneli");


            if (birim.Trim() != "")
                XLS.HucreAdBulYaz("BirimAdi", " : " + birim + " - " + f.birimAd);
            else
                XLS.HucreAdBulYaz("BirimAdi", " : Kurum Geneli");

            XLS.HucreAdBulYaz("Bastar", " : " + tarih1.ToString());
            XLS.HucreAdBulYaz("Bittar", " : " + tarih2.ToString());

            string paraBirimi = ButceMuhasebeGenel.DondurParaBirimi(1).kisaltma;
            if (listeTur == 2) paraBirimi = "DÖVİZ";
            XLS.HucreAdBulYaz("ParaBirimi", paraBirimi);
        }

        private string BakiyeStringUret(double tmpBakiye)
        {
            string bakiyeStr = "";
            if (tmpBakiye < 0)
                bakiyeStr += Math.Abs(tmpBakiye).ToString("#,##0.00") + " (A)";
            else if (tmpBakiye > 0)
                bakiyeStr += Math.Abs(tmpBakiye).ToString("#,##0.00") + " (B)";
            else
                bakiyeStr += Math.Abs(tmpBakiye).ToString("0.00");
            return bakiyeStr;
        }
    }

    /// <summary>
    /// Kesin Hesap Gider rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class KesinHesapGider
    {
        OrtakClass.Tablo XLS;
        IMUHServis s = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Kesin Hesap Gider raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int raporTur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["RaporTur"], 0);

            string basHesap = (string)raporParam["BasHesap"];
            string bitHesap = (string)raporParam["BitHesap"];

            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdBulYaz("ButceYil", yil.ToString());
            XLS.HucreAdBulYaz("KurumAd", kurum + " - " + kurumAd);
            XLS.HucreAdBulYaz("ParaBirimi", ButceMuhasebeGenel.ParaBirimiDondur());

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            if (tur == 1)
            {
                oEmri.formDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
                oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else if (tur == 2)
            {
                oEmri.formDurum = (int)ENUMBelgeDurumu.ONAYLI;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else
            {
                oEmri.formDurum = 22;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            oEmri.fisTurYap(fisTur);

            oEmri.sorguEkoSiraliMi = true;

            TNS.MUH.OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(yil);
            //oEmriDetay.hesapKod = basHesap.Replace(".", "").Trim() + ";" + bitHesap.Replace(".", "").Trim();
            oEmriDetay.kur = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            oEmriDetay.fon = "";
            oEmriDetay.fin = "";
            oEmriDetay.eko = "";

            oEmriDetay.hesapKod = "90103;90104;90105;90106;90108;90101;90115;90109;90111;90113;90117;90112;90114;90118;830;834;90119;90120;90121;90122;90124;90125";
            oEmri.detay.objeler.Add(oEmriDetay);

            ObjectArray mb = s.KesinHesapGiderTertipListele((TNS.KYM.Kullanici)kullanan, oEmri);

            KesinHesapGiderTertipIcerikYaz(XLS, kullanan, yil, tur, saymanlik, birim, mb, oEmri, tarih1, tarih2, true, true, raporTur);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void KesinHesapGiderTertipIcerikYaz(OrtakClass.Tablo XLS, object kullanan, int yil, int tur, int saymanlik, string birim, ObjectArray mb, TNS.MUH.OdemeEmriMIFForm oEmri, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2, bool detay, bool herHesapYeniSayfada, int raporTur)
        {
            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            int satir = 0;
            int sablonSatir = 0;
            int sablonSutun = 0;

            XLS.HucreAdAdresCoz("BaslaSatir", ref sablonSatir, ref sablonSutun);
            satir = sablonSatir;

            TNS.BKD.Tertip tertip = new TNS.BKD.Tertip(DateTime.Now.Year, TNS.BKD.ENUMTabloKod.TABButce);

            string[] eKodlar = new string[11];
            string kur = "";
            string fon = "";
            string fin = "";
            string eko = "";
            string kodAd = "";
            string[] kolonDeger = new string[11];
            kolonDeger[0] = "90103;90104;90105;90106;90108";
            kolonDeger[1] = "90101";
            kolonDeger[2] = "90115";
            kolonDeger[3] = "90109;90111;90113;90117";
            kolonDeger[4] = "90112;90114;90118";
            kolonDeger[5] = "";
            kolonDeger[6] = "830;834";
            kolonDeger[7] = "";
            kolonDeger[8] = "";
            kolonDeger[9] = "90125";
            kolonDeger[10] = "90119;90120;90121;90122;90124";
            System.Collections.Hashtable kurListe = new System.Collections.Hashtable();
            System.Collections.Hashtable fonListe = new System.Collections.Hashtable();
            System.Collections.Hashtable finListe = new System.Collections.Hashtable();
            System.Collections.Hashtable ekoListe = new System.Collections.Hashtable();


            foreach (string f in mb.objeler)
            {
                tertip.TertipAta(f);
                tertip.Noktala();

                //Kurumsal ve fonksiyonel tammı
                bool tamMi = tertip.KodTamMi(0);
                if (!tamMi) continue;
                tamMi = tertip.KodTamMi(1);
                if (!tamMi) continue;

                string ff = tertip.TertipStrOl().Replace("-", ".");
                string[] kodlar = ff.Split('.');

                int degisen = 0;
                if (eKodlar[0] != "")
                {
                    for (int i = 0; i < kodlar.Length; i++)
                    {
                        if (eKodlar.Length > i)
                        {
                            if (eKodlar[i] != kodlar[i])
                            {
                                degisen = i;
                                break;
                            }
                        }
                        else
                        {
                            degisen = i;
                            break;
                        }
                    }
                }

                if (degisen == 0)
                {
                    kur = kodlar[0];
                    fon = "";
                    fin = "";
                    eko = "";
                    kodAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "ABSKUR", oEmri.yil.ToString() + "-" + kur, true);

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 0, kodlar[0]);
                    XLS.HucreDegerYaz(satir, 11, kodAd);
                    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, kur, fon, fin, eko, oEmri, kolonDeger);
                    XLS.KoyuYap(satir, 0, satir, 30, true);
                }
                if (degisen <= 1 && kodlar.Length > 1)
                {
                    kur = kodlar[0] + kodlar[1];
                    fon = "";
                    fin = "";
                    eko = "";
                    kodAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "ABSKUR", oEmri.yil.ToString() + "-" + kur, true);

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 1, kodlar[1]);
                    XLS.HucreDegerYaz(satir, 11, kodAd);
                    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, kur, fon, fin, eko, oEmri, kolonDeger);
                    XLS.KoyuYap(satir, 0, satir, 30, true);
                }
                if (degisen <= 2 && kodlar.Length > 2)
                {
                    kur = kodlar[0] + kodlar[1] + kodlar[2];
                    fon = "";
                    fin = "";
                    eko = "";
                    kodAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "ABSKUR", oEmri.yil.ToString() + "-" + kur, true);

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 2, kodlar[2]);
                    XLS.HucreDegerYaz(satir, 11, kodAd);
                    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, kur, fon, fin, eko, oEmri, kolonDeger);
                    XLS.KoyuYap(satir, 0, satir, 30, true);
                }
                if (degisen <= 3 && kodlar.Length > 3)
                {
                    kur = kodlar[0] + kodlar[1] + kodlar[2] + kodlar[3];
                    fon = "";
                    fin = "";
                    eko = "";
                    kodAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "ABSKUR", oEmri.yil.ToString() + "-" + kur, true);

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 3, kodlar[3]);
                    XLS.HucreDegerYaz(satir, 11, kodAd);
                    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, kur, fon, fin, eko, oEmri, kolonDeger);
                    XLS.KoyuYap(satir, 0, satir, 30, true);
                    XLS.KoyuYap(satir, 0, satir, 30, true);
                }
                if (degisen <= 4 && kodlar.Length > 4)
                {
                    fon = kodlar[4];
                    fin = "";
                    eko = "";
                    if (fonListe.ContainsKey(fon))
                        kodAd = fonListe[fon].ToString();
                    else
                    {
                        kodAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "ABSFON", oEmri.yil.ToString() + "-" + fon, true);
                        fonListe.Add(fon, kodAd);
                    }

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 4, kodlar[4]);
                    XLS.HucreDegerYaz(satir, 11, kodAd);
                    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, kur, fon, fin, eko, oEmri, kolonDeger);
                    XLS.KoyuYap(satir, 0, satir, 30, true);
                }
                if (degisen <= 5 && kodlar.Length > 5)
                {
                    fon = kodlar[4] + kodlar[5];
                    fin = "";
                    eko = "";
                    if (fonListe.ContainsKey(fon))
                        kodAd = fonListe[fon].ToString();
                    else
                    {
                        kodAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "ABSFON", oEmri.yil.ToString() + "-" + fon, true);
                        fonListe.Add(fon, kodAd);
                    }

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 5, kodlar[5]);
                    XLS.HucreDegerYaz(satir, 11, kodAd);
                    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, kur, fon, fin, eko, oEmri, kolonDeger);
                }
                if (degisen <= 6 && kodlar.Length > 6)
                {
                    fon = kodlar[4] + kodlar[5] + kodlar[6];
                    fin = "";
                    eko = "";
                    if (fonListe.ContainsKey(fon))
                        kodAd = fonListe[fon].ToString();
                    else
                    {
                        kodAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "ABSFON", oEmri.yil.ToString() + "-" + fon, true);
                        fonListe.Add(fon, kodAd);
                    }

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 6, kodlar[6]);
                    XLS.HucreDegerYaz(satir, 11, kodAd);
                    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, kur, fon, fin, eko, oEmri, kolonDeger);
                }
                if (degisen <= 7 && kodlar.Length > 7)
                {
                    fon = kodlar[4] + kodlar[5] + kodlar[6] + kodlar[7];
                    fin = "";
                    eko = "";
                    if (fonListe.ContainsKey(fon))
                        kodAd = fonListe[fon].ToString();
                    else
                    {
                        kodAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "ABSFON", oEmri.yil.ToString() + "-" + fon, true);
                        fonListe.Add(fon, kodAd);
                    }

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 7, kodlar[7]);
                    XLS.HucreDegerYaz(satir, 11, kodAd);
                    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, kur, fon, fin, eko, oEmri, kolonDeger);
                    XLS.KoyuYap(satir, 0, satir, 30, true);
                }
                if (degisen <= 8 && kodlar.Length > 8)
                {
                    fin = kodlar[8];
                    eko = "";
                    if (finListe.ContainsKey(fin))
                        kodAd = finListe[fin].ToString();
                    else
                    {
                        kodAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "ABSFIN", oEmri.yil.ToString() + "-" + fin, true);
                        finListe.Add(fin, kodAd);
                    }

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 8, kodlar[8]);
                    XLS.HucreDegerYaz(satir, 11, kodAd);
                    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, kur, fon, fin, eko, oEmri, kolonDeger);
                }
                if (degisen <= 9 && kodlar.Length > 9)
                {
                    eko = kodlar[9];
                    if (ekoListe.ContainsKey(eko))
                        kodAd = ekoListe[eko].ToString();
                    else
                    {
                        kodAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "ABSEKO", oEmri.yil.ToString() + "-" + eko, true);
                        ekoListe.Add(eko, kodAd);
                    }

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 9, kodlar[9]);
                    XLS.HucreDegerYaz(satir, 11, kodAd);
                    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, kur, fon, fin, eko, oEmri, kolonDeger);
                    XLS.KoyuYap(satir, 0, satir, 30, true);
                }
                if (degisen <= 10 && kodlar.Length > 10)
                {
                    eko = kodlar[9] + kodlar[10];
                    if (ekoListe.ContainsKey(eko))
                        kodAd = ekoListe[eko].ToString();
                    else
                    {
                        kodAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "ABSEKO", oEmri.yil.ToString() + "-" + eko, true);
                        ekoListe.Add(eko, kodAd);
                    }

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 10, kodlar[10]);
                    XLS.HucreDegerYaz(satir, 11, kodAd);
                    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, kur, fon, fin, eko, oEmri, kolonDeger);
                }

                eKodlar = ff.Split('.');
            }
            XLS.YatayCizgiCiz(satir, 0, 23, LineStyle.DOUBLE, TabloRenk.BLACK, false);
        }

        private void SayisalDegerYaz(TNS.KYM.Kullanici kullanan, int satir, string kur, string fon, string fin, string eko, OdemeEmriMIFForm form, string[] kolonDeger)
        {
            ((OdemeEmriMIFDetay)form.detay[0]).kur = kur;
            ((OdemeEmriMIFDetay)form.detay[0]).fon = fon;
            ((OdemeEmriMIFDetay)form.detay[0]).fin = fin;
            ((OdemeEmriMIFDetay)form.detay[0]).eko = eko;
            ObjectArray mb = s.KesinHesapGiderRakamVer(kullanan, form, kolonDeger);
            int kolon = 12;
            for (int i = kolon; i < 23; i++)
                XLS.HucreDegerYaz(satir, i, 0.00);

            double netOdenek = 0;
            double dusulen = 0;
            double giderOdenek = 0;
            double kolonButceGider = 0;
            int kolonSay = 0;
            foreach (double deger in mb.objeler)
            {
                if (kolonSay == 0 || kolonSay == 1 || kolonSay == 2 || kolonSay == 3)
                    netOdenek += deger;
                if (kolonSay == 4)
                    dusulen += OrtakFonksiyonlar.ConvertToDbl(Math.Abs(deger));
                if (kolonSay == 6)
                    giderOdenek += Math.Abs(deger);
                if (kolon == 18)
                    kolonButceGider = Math.Abs(deger);
                XLS.HucreDegerYaz(satir, kolon, Math.Abs(deger));
                kolonSay++;
                kolon++;
            }
            XLS.HucreDegerYaz(satir, 17, netOdenek - dusulen);
            XLS.HucreDegerYaz(satir, 21, (netOdenek - dusulen) - giderOdenek);
            if (kolonButceGider > 0 && (netOdenek - dusulen) > 0)
                XLS.HucreDegerYaz(satir, 23, (kolonButceGider * 100) / (netOdenek - dusulen));
            else
                XLS.HucreDegerYaz(satir, 23, 0);
        }
    }

    /// <summary>
    /// Kesin Hesap Gelir rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class KesinHesapGelir
    {
        OrtakClass.Tablo XLS;
        IMUHServis s = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Kesin Hesap Gelir raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int raporTur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["RaporTur"], 0);

            string basHesap = (string)raporParam["BasHesap"];
            string bitHesap = (string)raporParam["BitHesap"];

            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdBulYaz("ButceYil", yil.ToString());
            XLS.HucreAdBulYaz("KurumAd", kurum + " - " + kurumAd);
            XLS.HucreAdBulYaz("ParaBirimi", ButceMuhasebeGenel.ParaBirimiDondur());

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            if (tur == 1)
            {
                oEmri.formDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
                oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();//önceki yıl sql inde sorgu tarihi kalıyordu
            }
            else if (tur == 2)
            {
                oEmri.formDurum = (int)ENUMBelgeDurumu.ONAYLI;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else
            {
                oEmri.formDurum = 22;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }

            oEmri.fisTurYap(fisTur);
            oEmri.sorguEkoSiraliMi = true;

            oEmri.detay = new ObjectArray();
            TNS.MUH.OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(yil);
            oEmriDetay.hesapKod = "";
            oEmriDetay.kur = "";
            oEmriDetay.fon = "";
            oEmriDetay.fin = "";
            oEmriDetay.eko = "";
            oEmri.detay.objeler.Add(oEmriDetay);

            ObjectArray mb = s.KesinHesapGelirTertipListele((TNS.KYM.Kullanici)kullanan, oEmri);

            KesinHesapGelirTertipIcerikYaz(XLS, kullanan, yil, tur, saymanlik, birim, mb, oEmri, tarih1, tarih2, true, true, raporTur);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void KesinHesapGelirTertipIcerikYaz(OrtakClass.Tablo XLS, object kullanan, int yil, int tur, int saymanlik, string birim, ObjectArray mb, TNS.MUH.OdemeEmriMIFForm oEmri, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2, bool detay, bool herHesapYeniSayfada, int raporTur)
        {
            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            int satir = 0;
            int sablonSatir = 0;
            int sablonSutun = 0;

            XLS.HucreAdAdresCoz("BaslaSatir", ref sablonSatir, ref sablonSutun);
            satir = sablonSatir;

            string[] eKodlar = new string[4];
            string eko = "";
            string ekoAd = "";
            double[] toplamlar = new double[9];
            mb.objeler.Sort();
            foreach (string f in mb.objeler)
            {
                string[] kodlar = f.Split('.');

                int degisen = 0;
                if (eKodlar[0] != "")
                {
                    for (int i = 0; i < kodlar.Length; i++)
                    {
                        if (eKodlar[i] != kodlar[i])
                        {
                            degisen = i;
                            break;
                        }
                    }
                }

                if (kodlar.Length != 4)
                    continue;

                //EGO gösterilmesini istedi 27032020 Melih
                //if (kodlar[0] == "09")//Ret ve iadeler gösterilmeyecek
                //    continue;

                if (degisen == 0)
                {
                    eko = kodlar[0];
                    //ekoAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "ABSGELIRKOD", oEmri.yil.ToString() + "-" + eko, true);
                    ekoAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-800" + eko, true);

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 0, kodlar[0]);
                    XLS.HucreDegerYaz(satir, 4, ekoAd);
                    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, eko, oEmri, toplamlar);
                    XLS.KoyuYap(satir, 0, satir, 13, true);
                }
                if (degisen <= 1 && kodlar.Length > 1)
                {
                    eko = kodlar[0] + kodlar[1];
                    //ekoAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "ABSGELIRKOD", oEmri.yil.ToString() + "-" + kodlar[0] + OrtakFonksiyonlar.ConvertToInt(kodlar[1], 0), true);
                    ekoAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-800" + eko, true);

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 1, kodlar[1]);
                    XLS.HucreDegerYaz(satir, 4, ekoAd);
                    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, eko, oEmri, null);
                }
                if (degisen <= 2 && kodlar.Length > 2)
                {
                    eko = kodlar[0] + kodlar[1] + kodlar[2];
                    //ekoAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "ABSGELIRKOD", oEmri.yil.ToString() + "-" + kodlar[0] + OrtakFonksiyonlar.ConvertToInt(kodlar[1], 0) + OrtakFonksiyonlar.ConvertToInt(kodlar[2], 0), true);
                    ekoAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-800" + eko, true);

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 2, kodlar[2]);
                    XLS.HucreDegerYaz(satir, 4, ekoAd);
                    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, eko, oEmri, null);
                }
                if (degisen <= 3 && kodlar.Length > 3)
                {
                    eko = kodlar[0] + kodlar[1] + kodlar[2] + kodlar[3];
                    //ekoAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "ABSGELIRKOD", oEmri.yil.ToString() + "-" + kodlar[0] + OrtakFonksiyonlar.ConvertToInt(kodlar[1], 0) + OrtakFonksiyonlar.ConvertToInt(kodlar[2], 0) + kodlar[3], true);
                    ekoAd = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-800" + eko, true);

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 3, kodlar[3]);
                    XLS.HucreDegerYaz(satir, 4, ekoAd);
                    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, eko, oEmri, null);
                }

                eKodlar = f.Split('.');
            }

            satir++;
            XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
            XLS.HucreDegerYaz(satir, 3, "");
            XLS.HucreDegerYaz(satir, 4, "TOPLAM");
            int kolon = 5;
            for (int i = kolon; i < 13; i++)
                XLS.HucreDegerYaz(satir, i, toplamlar[i - kolon]);

            XLS.KoyuYap(satir, 0, satir, 13, true);
            XLS.YatayCizgiCiz(satir, 0, 13, LineStyle.DOUBLE, TabloRenk.BLACK, false);
        }

        private void SayisalDegerYaz(TNS.KYM.Kullanici kullanan, int satir, string eko, OdemeEmriMIFForm form, double[] toplamlar)
        {
            form.detay = new ObjectArray();
            OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(form.yil);
            detay.kur = "";
            detay.fon = "";
            detay.fin = "";
            detay.eko = eko;
            form.detay.objeler.Add(detay);

            ObjectArray mb = s.KesinHesapGelirRakamVer(kullanan, form);
            int kolon = 5;
            for (int i = kolon; i < 13; i++)
                XLS.HucreDegerYaz(satir, i, 0.00);

            int j = 0;
            foreach (double deger in mb.objeler)
            {
                if (toplamlar != null)
                {
                    toplamlar[j] += deger;
                    j++;
                }
                XLS.HucreDegerYaz(satir, kolon, deger);
                kolon++;
            }
        }
    }

    /// <summary>
    /// Kesin Hesap Bütçe rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class KesinHesapButce
    {
        OrtakClass.Tablo XLS;
        IMUHServis s = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Kesin Hesap Bütçe raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int raporTur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["RaporTur"], 0);
            int seviye = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Seviye"], 0);

            string basHesap = (string)raporParam["BasHesap"];
            string bitHesap = (string)raporParam["BitHesap"];

            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdBulYaz("ButceYil", yil.ToString());
            XLS.HucreAdBulYaz("KurumAd", kurum + " - " + kurumAd);
            XLS.HucreAdBulYaz("ParaBirimi", ButceMuhasebeGenel.ParaBirimiDondur());

            XLS.HucreAdBulYaz("txtYil", yil);
            if (raporTur == 13)
                XLS.HucreAdBulYaz("txtBaslik", "GİDERLERİN KURUMSAL SINIFLANDIRILMASI TABLOSU");
            else if (raporTur == 14)
                XLS.HucreAdBulYaz("txtBaslik", "GİDERLERİN FONKSİYONEL SINIFLANDIRILMASI TABLOSU");
            else if (raporTur == 15)
                XLS.HucreAdBulYaz("txtBaslik", "GİDERLERİN EKONOMİK SINIFLANDIRILMASI TABLOSU");
            else if (raporTur == 16)
                XLS.HucreAdBulYaz("txtBaslik", "BÜTÇE GİDERLERİNİN KURUMSAL SINIFLANDIRILMASI TABLOSU");
            else if (raporTur == 17)
                XLS.HucreAdBulYaz("txtBaslik", "BÜTÇE GİDERLERİNİN FONKSİYONEL SINIFLANDIRILMASI TABLOSU");
            else if (raporTur == 18)
                XLS.HucreAdBulYaz("txtBaslik", "BÜTÇE GİDERLERİNİN EKONOMİK SINIFLANDIRILMASI TABLOSU");
            else if (raporTur == 19)
                XLS.HucreAdBulYaz("txtBaslik", "BÜTÇE GİDERLERİNİN FİNANS SINIFLANDIRILMASI TABLOSU");
            else if (raporTur == 20)
                XLS.HucreAdBulYaz("txtBaslik", "GELİRLERİN EKONOMİK SINIFLANDIRILMASI TABLOSU");
            else if (raporTur == 21)
                XLS.HucreAdBulYaz("txtBaslik", "BÜTÇE GELİRLERİN EKONOMİK SINIFLANDIRILMASI TABLOSU");
            else if (raporTur == 31)
                XLS.HucreAdBulYaz("txtBaslik", "FAALİYET SONUÇLARI TABLOSU");
            else if (raporTur == 32)
                XLS.HucreAdBulYaz("txtBaslik", "BÜTÇE UYGULAMA SONUÇLARI TABLOSU");
            else if (raporTur == 34)
                XLS.HucreAdBulYaz("txtBaslik", "İÇ BORÇ DEĞİŞİM TABLOSU");
            else if (raporTur == 35)
                XLS.HucreAdBulYaz("txtBaslik", "DIŞ BORÇ DEĞİŞİM TABLOSU");

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            if (tur == 1)
            {
                oEmri.formDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
                oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else if (tur == 2)
            {
                oEmri.formDurum = (int)ENUMBelgeDurumu.ONAYLI;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else
            {
                oEmri.formDurum = 22;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            oEmri.fisTurYap(fisTur);
            oEmri.sorguEkoSiraliMi = true;

            TNS.MUH.OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(yil);

            if (raporTur >= 13 && raporTur <= 15)
                oEmriDetay.hesapKod = "630";
            else if (raporTur >= 16 && raporTur <= 19)
                oEmriDetay.hesapKod = "830";
            else if (raporTur == 20)
                oEmriDetay.hesapKod = "600";
            else if (raporTur == 21)
                oEmriDetay.hesapKod = "800";
            else if (raporTur == 34)
                oEmriDetay.hesapKod = "3;4;-310;-410";
            else if (raporTur == 35)
                oEmriDetay.hesapKod = "310;410";

            oEmriDetay.kur = "";//Kurumsal kod gelir bilgilerini getirmiyor.// OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            oEmriDetay.fon = "";
            oEmriDetay.fin = "";
            oEmriDetay.eko = "";
            oEmri.detay.objeler.Add(oEmriDetay);

            int kodTur = 0;
            if (raporTur == 13 || raporTur == 16)
                kodTur = (int)ENUMKesinHesapButce.KURUMSAL;
            else if (raporTur == 14 || raporTur == 17)
                kodTur = (int)ENUMKesinHesapButce.FONKSIYONEL;
            else if (raporTur == 15 || raporTur == 18)
                kodTur = (int)ENUMKesinHesapButce.GIDEREKONOMIK; //EKONOMIK;
            else if (raporTur == 19)
                kodTur = (int)ENUMKesinHesapButce.FINANS;
            else if (raporTur == 20 || raporTur == 21 || raporTur == 34 || raporTur == 35)
                kodTur = (int)ENUMKesinHesapButce.GELIREKONOMIK;

            if (raporTur < 31)
            {
                ObjectArray mb = s.KesinHesapButce((TNS.KYM.Kullanici)kullanan, oEmri, kodTur);
                KesinHesapButceIcerikYaz(XLS, kullanan, yil, tur, saymanlik, birim, mb, oEmri, tarih1, tarih2, true, true, raporTur, kodTur);
            }
            else if (raporTur == 33)
            {
                ((OdemeEmriMIFDetay)oEmri.detay[0]).hesapKod = "";
                kodTur = (int)ENUMKesinHesapButce.BILONCO;
                ObjectArray mb = s.KesinHesapButce((TNS.KYM.Kullanici)kullanan, oEmri, kodTur);
                BilancoIcerikYaz(XLS, kullanan, yil, tur, saymanlik, birim, mb, oEmri, tarih1, tarih2, true, true, raporTur, kodTur);

            }
            else if (raporTur == 31 || raporTur == 32)
            {
                if (raporTur == 31)
                    ((OdemeEmriMIFDetay)oEmri.detay[0]).hesapKod = "630";
                else
                    ((OdemeEmriMIFDetay)oEmri.detay[0]).hesapKod = "830";
                kodTur = (int)ENUMKesinHesapButce.GIDEREKONOMIK;//EKONOMIK;
                ObjectArray mb = s.KesinHesapButce((TNS.KYM.Kullanici)kullanan, oEmri, kodTur);

                if (raporTur == 31)
                    ((OdemeEmriMIFDetay)oEmri.detay[0]).hesapKod = "600";
                else
                    ((OdemeEmriMIFDetay)oEmri.detay[0]).hesapKod = "800";
                kodTur = (int)ENUMKesinHesapButce.GELIREKONOMIK;
                ObjectArray mb1 = s.KesinHesapButce((TNS.KYM.Kullanici)kullanan, oEmri, kodTur);

                if (raporTur == 31)
                    ((OdemeEmriMIFDetay)oEmri.detay[0]).hesapKod = "610";
                else
                    ((OdemeEmriMIFDetay)oEmri.detay[0]).hesapKod = "810";
                kodTur = (int)ENUMKesinHesapButce.GELIREKONOMIK;
                ObjectArray mb2 = s.KesinHesapButce((TNS.KYM.Kullanici)kullanan, oEmri, kodTur);

                foreach (TNS.MUH.KesinHesapButce f in mb1.objeler)
                    mb.objeler.Add(f);

                foreach (TNS.MUH.KesinHesapButce f in mb2.objeler)
                    mb.objeler.Add(f);

                KesinHesapFaaliyetIcerikYaz(XLS, kullanan, yil, tur, saymanlik, birim, mb, oEmri, tarih1, tarih2, true, true, raporTur, kodTur, seviye);
            }
            else if (raporTur == 34 || raporTur == 35)
            {
                //Açılış
                oEmri.fisTurYap(ENUMFISTUR.ACILIS);
                ObjectArray acilis = s.KesinHesapButce((TNS.KYM.Kullanici)kullanan, oEmri, kodTur);

                //Hepsi
                oEmri.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });
                ObjectArray donemIci = s.KesinHesapButce((TNS.KYM.Kullanici)kullanan, oEmri, kodTur);

                ObjectArray gidecek = new ObjectArray();

                //Sondaki fazladan gelen 00 kodlarını atmak için
                //**********************************************************************
                foreach (TNS.MUH.KesinHesapButce acilisDeger in acilis.objeler)
                {
                    if (acilisDeger.kod.EndsWith(".00.00"))
                    {
                        int i = acilisDeger.kod.LastIndexOf(".00.00");
                        acilisDeger.kod = acilisDeger.kod.Substring(0, acilisDeger.kod.Length - 6);
                    }
                }
                foreach (TNS.MUH.KesinHesapButce donemIciDeger in donemIci.objeler)
                {
                    if (donemIciDeger.kod.EndsWith(".00.00"))
                    {
                        int i = donemIciDeger.kod.LastIndexOf(".00.00");
                        donemIciDeger.kod = donemIciDeger.kod.Substring(0, donemIciDeger.kod.Length - 6);
                    }
                }
                for (int i = 0; i < acilis.objeler.Count; i++)
                {
                    TNS.MUH.KesinHesapButce acilisDeger = (TNS.MUH.KesinHesapButce)acilis.objeler[i];
                    if (acilisDeger.kod.EndsWith(".00") && acilisDeger.kod.Length > 3)
                    {
                        TNS.MUH.KesinHesapButce acilisDeger2 = null;

                        if (acilis.objeler.Count > i + 1)
                            acilisDeger2 = (TNS.MUH.KesinHesapButce)acilis.objeler[i + 1];

                        string acilisDeger2Kod = "";
                        if (acilisDeger2 != null && acilisDeger2.kod.Length - 3 > 0)
                            acilisDeger2Kod = acilisDeger2.kod.Substring(0, acilisDeger2.kod.Length - 3);

                        if (acilisDeger.kod.Substring(0, acilisDeger.kod.Length - 3) != acilisDeger2Kod)
                            acilisDeger.kod = acilisDeger.kod.Substring(0, acilisDeger.kod.Length - 3);
                    }
                }
                for (int i = 0; i < donemIci.objeler.Count; i++)
                {
                    TNS.MUH.KesinHesapButce donemIciDeger = (TNS.MUH.KesinHesapButce)donemIci.objeler[i];
                    if (donemIciDeger.kod.EndsWith(".00") && donemIciDeger.kod.Length > 3)
                    {
                        TNS.MUH.KesinHesapButce donemIciDeger2 = null;

                        if (donemIci.objeler.Count > i + 1)
                            donemIciDeger2 = (TNS.MUH.KesinHesapButce)donemIci.objeler[i + 1];

                        string donemIciDeger2Kod = "";
                        if (donemIciDeger2 != null && donemIciDeger2.kod.Length - 3 > 0)
                            donemIciDeger2Kod = donemIciDeger2.kod.Substring(0, donemIciDeger2.kod.Length - 3);

                        if (donemIciDeger.kod.Substring(0, donemIciDeger.kod.Length - 3) != donemIciDeger2Kod)
                            donemIciDeger.kod = donemIciDeger.kod.Substring(0, donemIciDeger.kod.Length - 3);
                    }
                }
                //**********************************************************************

                foreach (TNS.MUH.KesinHesapButce acilisDeger in acilis.objeler)
                {
                    bool bulundu = false;
                    foreach (TNS.MUH.KesinHesapButce donemIciDeger in donemIci.objeler)
                    {
                        if (donemIciDeger.hesapKod == acilisDeger.hesapKod && donemIciDeger.kod == acilisDeger.kod)
                        {
                            //donemIciDeger.deger1 = donemIciDeger.deger1 - acilisDeger.deger1;
                            //donemIciDeger.deger2 = donemIciDeger.deger2 - acilisDeger.deger2;
                            //donemIciDeger.deger3 = donemIciDeger.deger3 - acilisDeger.deger3;
                            double yilbasi = acilisDeger.deger1;
                            double yilsonu = donemIciDeger.deger1;
                            double degisim = yilsonu - yilbasi;
                            donemIciDeger.deger3 = yilbasi;
                            donemIciDeger.deger2 = yilsonu;
                            donemIciDeger.deger1 = degisim;

                            if (donemIciDeger.deger1 > 0 || donemIciDeger.deger2 > 0 || donemIciDeger.deger3 > 0)
                                gidecek.objeler.Add(donemIciDeger);

                            bulundu = true;
                            break;
                        }
                    }
                    if (!bulundu)
                    {
                        //acilis ta olan hesap yıl içinde kullanılmamış ise - olması için
                        double yilbasi = acilisDeger.deger1;
                        double yilsonu = 0;
                        double degisim = yilsonu - yilbasi;
                        acilisDeger.deger3 = yilbasi;
                        acilisDeger.deger2 = yilsonu;
                        acilisDeger.deger1 = degisim;

                        gidecek.objeler.Add(acilisDeger);
                    }
                }

                foreach (TNS.MUH.KesinHesapButce donemIciDeger in donemIci.objeler)
                {
                    if (donemIciDeger.deger2 == 0 && donemIciDeger.deger1 > 0)// kaldırıldı. EGO Metro zarar göstermesine rağmen değişimde olması için(25.03.2010)
                    {
                        double yilbasi = 0;
                        double yilsonu = donemIciDeger.deger1;
                        double degisim = yilsonu - yilbasi;
                        donemIciDeger.deger3 = yilbasi;
                        donemIciDeger.deger2 = yilsonu;
                        donemIciDeger.deger1 = degisim;
                        gidecek.objeler.Add(donemIciDeger);
                    }
                }
                KesinHesapButceIcerikYaz(XLS, kullanan, yil, tur, saymanlik, birim, gidecek, oEmri, tarih1, tarih2, true, true, raporTur, kodTur);
            }
            else if (raporTur == 36)
            {
                //Varlık-------------------------------------------------------
                kodTur = (int)ENUMKesinHesapButce.BILONCO;
                oEmriDetay.hesapKod = "1;2;-25;-29";
                //Açılış
                oEmri.fisTurYap(ENUMFISTUR.ACILIS);
                ObjectArray acilisVarlik = s.KesinHesapButce((TNS.KYM.Kullanici)kullanan, oEmri, kodTur);

                //Hepsi
                oEmri.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });
                ObjectArray donemIciVarlik = s.KesinHesapButce((TNS.KYM.Kullanici)kullanan, oEmri, kodTur);

                foreach (TNS.MUH.KesinHesapButce acilisDeger in acilisVarlik.objeler)
                {
                    bool bulundu = false;
                    foreach (TNS.MUH.KesinHesapButce donemIciDeger in donemIciVarlik.objeler)
                    {
                        if (donemIciDeger.hesapKod == acilisDeger.hesapKod && donemIciDeger.kod == acilisDeger.kod)
                        {
                            donemIciDeger.degerAcilis1 = acilisDeger.deger1;
                            donemIciDeger.degerAcilis2 = acilisDeger.deger2;
                            donemIciDeger.degerAcilis3 = acilisDeger.deger3;
                            bulundu = true;
                            break;
                        }
                    }
                    if (!bulundu)
                    {
                        //acilis ta olan hesap yıl içinde kullanılmamış ise - olması için
                        acilisDeger.degerAcilis1 = acilisDeger.deger1;
                        acilisDeger.degerAcilis2 = acilisDeger.deger2;
                        acilisDeger.degerAcilis3 = acilisDeger.deger3;
                        donemIciVarlik.objeler.Add(acilisDeger);
                    }
                }
                //Yukumluluk--------------------------------------------------------------------
                kodTur = (int)ENUMKesinHesapButce.BILONCO;
                oEmriDetay.hesapKod = "3;4";
                //Açılış
                oEmri.fisTurYap(ENUMFISTUR.ACILIS);
                ObjectArray acilisYukumluluk = s.KesinHesapButce((TNS.KYM.Kullanici)kullanan, oEmri, kodTur);

                //Hepsi
                oEmri.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });
                ObjectArray donemIciYukumluluk = s.KesinHesapButce((TNS.KYM.Kullanici)kullanan, oEmri, kodTur);

                foreach (TNS.MUH.KesinHesapButce acilisDeger in acilisYukumluluk.objeler)
                {
                    bool bulundu = false;
                    foreach (TNS.MUH.KesinHesapButce donemIciDeger in donemIciYukumluluk.objeler)
                    {
                        if (donemIciDeger.hesapKod == acilisDeger.hesapKod && donemIciDeger.kod == acilisDeger.kod)
                        {
                            donemIciDeger.degerAcilis1 = acilisDeger.deger1;
                            donemIciDeger.degerAcilis2 = acilisDeger.deger2;
                            donemIciDeger.degerAcilis3 = acilisDeger.deger3;
                            bulundu = true;
                            break;
                        }
                    }
                    if (!bulundu)
                    {
                        //acilis ta olan hesap yıl içinde kullanılmamış ise - olması için
                        acilisDeger.degerAcilis1 = acilisDeger.deger1;
                        acilisDeger.degerAcilis2 = acilisDeger.deger2;
                        acilisDeger.degerAcilis3 = acilisDeger.deger3;
                        donemIciYukumluluk.objeler.Add(acilisDeger);
                    }
                }

                foreach (TNS.MUH.KesinHesapButce deger in donemIciYukumluluk.objeler)
                    donemIciVarlik.objeler.Add(deger);

                KesinHesapMaliIcerikYaz(XLS, kullanan, yil, tur, saymanlik, birim, donemIciVarlik, oEmri, tarih1, tarih2, true, true, raporTur, kodTur);
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void KesinHesapButceIcerikYaz(OrtakClass.Tablo XLS, object kullanan, int yil, int tur, int saymanlik, string birim, ObjectArray mb, TNS.MUH.OdemeEmriMIFForm oEmri, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2, bool detay, bool herHesapYeniSayfada, int raporTur, int kodTur)
        {
            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            int satir = 0;
            int sablonSatir = 0;
            int sablonSutun = 0;

            XLS.HucreAdAdresCoz("BaslaSatir", ref sablonSatir, ref sablonSutun);
            satir = sablonSatir;

            TNS.BKD.Tertip tertip = new TNS.BKD.Tertip(DateTime.Now.Year, TNS.BKD.ENUMTabloKod.TABButce);

            string[] eKodlar = new string[4];
            string kod = "";
            string ad = "";
            string kodAdUzay = "";
            string eHesapKod = "";

            if (kodTur == (int)ENUMKesinHesapButce.KURUMSAL)
            {
                kodAdUzay = "ABSKUR";
                XLS.HucreAdBulYaz("txtKodAd", "KURUMSAL");
            }
            else if (kodTur == (int)ENUMKesinHesapButce.FONKSIYONEL)
            {
                kodAdUzay = "ABSFON";
                XLS.HucreAdBulYaz("txtKodAd", "FONKSİYONEL");
            }
            else if (kodTur == (int)ENUMKesinHesapButce.FINANS)
            {
                kodAdUzay = "ABSFIN";
                XLS.HucreAdBulYaz("txtKodAd", "FİN");
                XLS.SutunGizle(2, 4, true);
            }
            else if (kodTur == (int)ENUMKesinHesapButce.EKONOMIK)
            {
                kodAdUzay = "ABSEKO";
                XLS.HucreAdBulYaz("txtKodAd", "EKONOMİK");
            }
            else if (kodTur == (int)ENUMKesinHesapButce.GELIREKONOMIK || kodTur == (int)ENUMKesinHesapButce.GIDEREKONOMIK)
            {
                kodAdUzay = "MUHHESAPPLANI";
                XLS.HucreAdBulYaz("txtKodAd", "EKONOMİK");
            }
            if (raporTur == 34 || raporTur == 35)
                XLS.HucreAdBulYaz("txtKodAd", "YARDIMCI HES.");
            if (raporTur == 34)
                XLS.HucreAdBulYaz("txtAciklamaAd", "İÇ BORCUN NİTELİĞİ");
            if (raporTur == 35)
                XLS.HucreAdBulYaz("txtAciklamaAd", "DIŞ BORCUN NİTELİĞİ");

            double gToplam1 = 0;
            double gToplam2 = 0;
            double gToplam3 = 0;
            foreach (TNS.MUH.KesinHesapButce ff in mb.objeler)
            {
                if (ff.kod.Trim() == "")
                {
                    string ekleKod = "";
                    string ekleKodEski = "";
                    int kontrolYil = 0;
                    if (kodTur == (int)ENUMKesinHesapButce.KURUMSAL)
                    {
                        kontrolYil = 2010;//EGO Bu yılda Muhasebe Kurumsal kodunu değiştirdi. Kurumsal kod bir birime karşılık gelmiyorsa Muhasebe toplamına ekleniyor
                        ekleKod = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIKURUMSALKOD") + "";
                        ekleKodEski = System.Configuration.ConfigurationManager.AppSettings.Get("MuhasebeKurKodEski");
                    }
                    if (kodTur == (int)ENUMKesinHesapButce.FONKSIYONEL)
                    {
                        kontrolYil = 2010;//EGO Bu yılda Muhasebe Fonksiyonl kodunu değiştirdi. Fonksiyonel kod bir birime karşılık gelmiyorsa Muhasebe toplamına ekleniyor
                        ekleKod = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIFONKSIYONELKOD") + "";
                        ekleKodEski = System.Configuration.ConfigurationManager.AppSettings.Get("MuhasebeFonKodEski");
                    }
                    if (kontrolYil > 0)
                    {
                        string kontrolKod = "";
                        foreach (TNS.MUH.KesinHesapButce fff in mb.objeler)
                        {
                            if (yil < kontrolYil)
                                kontrolKod = ekleKodEski;
                            else
                                kontrolKod = ekleKod;
                            if (fff.kod == kontrolKod)
                            {
                                fff.deger1 += ff.deger1;
                                ff.deger1 = 0;
                            }
                        }
                        foreach (TNS.MUH.KesinHesapButce fff in mb.objeler)
                        {
                            if ((yil - 1) < kontrolYil)
                                kontrolKod = ekleKodEski;
                            else
                                kontrolKod = ekleKod;

                            if (fff.kod == kontrolKod)
                            {
                                fff.deger2 += ff.deger2;
                                ff.deger2 = 0;
                            }
                        }
                        foreach (TNS.MUH.KesinHesapButce fff in mb.objeler)
                        {
                            if ((yil - 2) < kontrolYil)
                                kontrolKod = ekleKodEski;
                            else
                                kontrolKod = ekleKod;
                            if (fff.kod == kontrolKod)
                            {
                                fff.deger3 += ff.deger3;
                                ff.deger3 = 0;
                            }
                        }
                    }
                }

                if (ff.kod.Trim() == "") continue;
                Tertip t = new Tertip(yil, ENUMTabloKod.TABButce, ff.kod.Trim(), ff.kod.Trim());

                bool eklemeYap = false;
                if (kodTur == (int)ENUMKesinHesapButce.KURUMSAL)
                    eklemeYap = t.KodTamMi(0);
                if (kodTur == (int)ENUMKesinHesapButce.FONKSIYONEL)
                    eklemeYap = t.KodTamMi(1);

                if (kodTur == (int)ENUMKesinHesapButce.GIDEREKONOMIK ||
                    kodTur == (int)ENUMKesinHesapButce.EKONOMIK ||
                    kodTur == (int)ENUMKesinHesapButce.FINANS ||
                    kodTur == (int)ENUMKesinHesapButce.GELIREKONOMIK) continue;

                if (eklemeYap)
                {
                    gToplam1 += ff.deger1;
                    gToplam2 += ff.deger2;
                    gToplam3 += ff.deger3;
                }
                else
                    ff.kod = "";//Eğer Kurumsal veya Fonksiyonel tam değilse zaten yukarıda muhasebe birimine eklneiyor
            }
            mb.objeler.Sort(new Sirala());

            foreach (TNS.MUH.KesinHesapButce f in mb.objeler)
            {
                string[] kodlar = f.kod.Split('.');

                if (f.kod.Trim() == "")
                    continue;

                int degisen = 0;
                if (eKodlar[0] != "" || eHesapKod != f.hesapKod)
                {
                    if (eHesapKod != f.hesapKod)
                        degisen = 0;
                    else
                    {
                        for (int i = 0; i < kodlar.Length; i++)
                        {
                            degisen = i;

                            if ((eKodlar.Length > i) && eKodlar[i] != kodlar[i])
                                break;
                        }
                    }
                }

                if (degisen == 0 && kodlar.Length > 0 && kodlar[0] != "")
                {
                    kod = kodlar[0];
                    if (kodTur == (int)ENUMKesinHesapButce.GELIREKONOMIK || kodTur == (int)ENUMKesinHesapButce.GIDEREKONOMIK)
                        ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, kodAdUzay, oEmri.yil.ToString() + "-" + f.hesapKod + kod, true);
                    else
                        ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, kodAdUzay, oEmri.yil.ToString() + "-" + kod, true);
                    double tutar1 = 0;
                    double tutar2 = 0;
                    double tutar3 = 0;
                    foreach (TNS.MUH.KesinHesapButce ff in mb.objeler)
                    {
                        if (ff.kod.Replace(".", "").StartsWith(kod) && f.hesapKod == ff.hesapKod)
                        {
                            tutar1 += ff.deger1;
                            tutar2 += ff.deger2;
                            tutar3 += ff.deger3;
                        }
                    }

                    //Genel toplam bilgisi
                    if (kodTur == (int)ENUMKesinHesapButce.GIDEREKONOMIK ||
                    kodTur == (int)ENUMKesinHesapButce.EKONOMIK ||
                    kodTur == (int)ENUMKesinHesapButce.FINANS ||
                    kodTur == (int)ENUMKesinHesapButce.GELIREKONOMIK)
                    {
                        gToplam1 += tutar1;
                        gToplam2 += tutar2;
                        gToplam3 += tutar3;
                    }


                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 0, f.hesapKod);
                    XLS.HucreDegerYaz(satir, 1, kodlar[0]);
                    XLS.HucreDegerYaz(satir, 5, ad);
                    XLS.HucreDegerYaz(satir, 8, tutar1);
                    XLS.HucreDegerYaz(satir, 7, tutar2);
                    XLS.HucreDegerYaz(satir, 6, tutar3);
                    XLS.KoyuYap(satir, 0, satir, 8, true);
                }
                if (degisen <= 1 && kodlar.Length > 1)
                {
                    kod = kodlar[0] + kodlar[1];
                    if (kodTur == (int)ENUMKesinHesapButce.GELIREKONOMIK || kodTur == (int)ENUMKesinHesapButce.GIDEREKONOMIK)
                        ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, kodAdUzay, oEmri.yil.ToString() + "-" + f.hesapKod + kod, true);
                    else
                        ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, kodAdUzay, oEmri.yil.ToString() + "-" + kod, true);
                    double tutar1 = 0;
                    double tutar2 = 0;
                    double tutar3 = 0;
                    foreach (TNS.MUH.KesinHesapButce ff in mb.objeler)
                    {
                        if (ff.kod.Replace(".", "").StartsWith(kod) && f.hesapKod == ff.hesapKod)
                        {
                            tutar1 += ff.deger1;
                            tutar2 += ff.deger2;
                            tutar3 += ff.deger3;
                        }
                    }

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 0, f.hesapKod);
                    XLS.HucreDegerYaz(satir, 2, kodlar[1]);
                    XLS.HucreDegerYaz(satir, 5, ad);
                    XLS.HucreDegerYaz(satir, 8, tutar1);
                    XLS.HucreDegerYaz(satir, 7, tutar2);
                    XLS.HucreDegerYaz(satir, 6, tutar3);
                }
                if (degisen <= 2 && kodlar.Length > 2)
                {
                    kod = kodlar[0] + kodlar[1] + kodlar[2];
                    if (kodTur == (int)ENUMKesinHesapButce.GELIREKONOMIK || kodTur == (int)ENUMKesinHesapButce.GIDEREKONOMIK)
                        ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, kodAdUzay, oEmri.yil.ToString() + "-" + f.hesapKod + kod, true);
                    else
                        ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, kodAdUzay, oEmri.yil.ToString() + "-" + kod, true);
                    double tutar1 = 0;
                    double tutar2 = 0;
                    double tutar3 = 0;
                    foreach (TNS.MUH.KesinHesapButce ff in mb.objeler)
                    {
                        if (ff.kod.Replace(".", "").StartsWith(kod) && f.hesapKod == ff.hesapKod)
                        {
                            tutar1 += ff.deger1;
                            tutar2 += ff.deger2;
                            tutar3 += ff.deger3;
                        }
                    }

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 0, f.hesapKod);
                    XLS.HucreDegerYaz(satir, 3, kodlar[2]);
                    XLS.HucreDegerYaz(satir, 5, ad);
                    XLS.HucreDegerYaz(satir, 8, tutar1);
                    XLS.HucreDegerYaz(satir, 7, tutar2);
                    XLS.HucreDegerYaz(satir, 6, tutar3);
                }
                if (degisen <= 3 && kodlar.Length > 3)
                {
                    kod = kodlar[0] + kodlar[1] + kodlar[2] + kodlar[3];
                    if (kodTur == (int)ENUMKesinHesapButce.GELIREKONOMIK || kodTur == (int)ENUMKesinHesapButce.GIDEREKONOMIK)
                        ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, kodAdUzay, oEmri.yil.ToString() + "-" + f.hesapKod + kod, true);
                    else
                        ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, kodAdUzay, oEmri.yil.ToString() + "-" + kod, true);
                    double tutar1 = f.deger1;
                    double tutar2 = f.deger2;
                    double tutar3 = f.deger3;

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 0, f.hesapKod);
                    XLS.HucreDegerYaz(satir, 4, kodlar[3]);
                    XLS.HucreDegerYaz(satir, 5, ad);
                    XLS.HucreDegerYaz(satir, 8, tutar1);
                    XLS.HucreDegerYaz(satir, 7, tutar2);
                    XLS.HucreDegerYaz(satir, 6, tutar3);
                    XLS.KoyuYap(satir, 0, satir, 8, true);
                }
                eKodlar = f.kod.Split('.');
                eHesapKod = f.hesapKod;
            }

            satir++;
            XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
            XLS.HucreBirlestir(satir, 0, satir, 5);
            XLS.HucreDegerYaz(satir, 0, "TOPLAM");
            XLS.HucreDegerYaz(satir, 8, gToplam1);
            XLS.HucreDegerYaz(satir, 7, gToplam2);
            XLS.HucreDegerYaz(satir, 6, gToplam3);
            XLS.KoyuYap(satir, 0, satir, 8, true);
        }

        private void KesinHesapMaliIcerikYaz(OrtakClass.Tablo XLS, object kullanan, int yil, int tur, int saymanlik, string birim, ObjectArray mb, TNS.MUH.OdemeEmriMIFForm oEmri, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2, bool detay, bool herHesapYeniSayfada, int raporTur, int kodTur)
        {
            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            int satir = 0;
            int sablonSatir = 0;
            int sablonSutun = 0;

            XLS.HucreAdAdresCoz("BaslaSatir", ref sablonSatir, ref sablonSutun);
            satir = sablonSatir;

            TNS.BKD.Tertip tertip = new TNS.BKD.Tertip(DateTime.Now.Year, TNS.BKD.ENUMTabloKod.TABButce);

            string[] eKodlar = new string[4];
            string ad = "";

            double gToplam1 = 0;
            double gToplam2 = 0;
            double gToplam3 = 0;
            double gToplamAcilis1 = 0;
            double gToplamAcilis2 = 0;
            double gToplamAcilis3 = 0;

            double gToplam1Varlik = 0;
            double gToplam2Varlik = 0;
            double gToplam3Varlik = 0;
            double gToplamAcilis1Varlik = 0;
            double gToplamAcilis2Varlik = 0;
            double gToplamAcilis3Varlik = 0;
            foreach (TNS.MUH.KesinHesapButce ff in mb.objeler)
            {
                //Tür Bilanço olduğu için borç-alacak yapılıyor. Fakat alacak-borç olmalı bu nedenle -1 ile çarpılıyor
                int carpan = 1;
                if (ff.hesapKod == "103" || ff.hesapKod == "268")
                    carpan = -1;

                ff.deger1 = carpan * Math.Abs(ff.deger1);
                ff.deger2 = carpan * Math.Abs(ff.deger2);
                ff.deger3 = carpan * Math.Abs(ff.deger3);
                ff.degerAcilis1 = carpan * Math.Abs(ff.degerAcilis1);
                ff.degerAcilis2 = carpan * Math.Abs(ff.degerAcilis2);
                ff.degerAcilis3 = carpan * Math.Abs(ff.degerAcilis3);
            }
            mb.objeler.Sort(new Sirala());

            bool yukumlulukBasla = true;
            foreach (TNS.MUH.KesinHesapButce f in mb.objeler)
            {
                if (yukumlulukBasla && (f.hesapKod.StartsWith("3") || f.hesapKod.StartsWith("4")))
                {
                    gToplam1Varlik = gToplam1;
                    gToplam2Varlik = gToplam2;
                    gToplam3Varlik = gToplam3;
                    gToplamAcilis1Varlik = gToplamAcilis1;
                    gToplamAcilis2Varlik = gToplamAcilis2;
                    gToplamAcilis3Varlik = gToplamAcilis3;

                    yukumlulukBasla = false;
                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreBirlestir(satir, 0, satir, 1);
                    XLS.HucreDegerYaz(satir, 0, "MALİ VARLIKLAR TOPLAMI (A)");
                    if (gToplam1 != 0)
                        XLS.HucreDegerYaz(satir, 10, gToplamAcilis1 / gToplam1);
                    else
                        XLS.HucreDegerYaz(satir, 10, 0.00);
                    XLS.HucreDegerYaz(satir, 9, gToplam1);
                    XLS.HucreDegerYaz(satir, 8, gToplamAcilis1);
                    if (gToplam2 != 0)
                        XLS.HucreDegerYaz(satir, 7, gToplamAcilis2 / gToplam2);
                    else
                        XLS.HucreDegerYaz(satir, 7, 0.00);
                    XLS.HucreDegerYaz(satir, 6, gToplam2);
                    XLS.HucreDegerYaz(satir, 5, gToplamAcilis2);
                    if (gToplam3 != 0)
                        XLS.HucreDegerYaz(satir, 4, gToplamAcilis3 / gToplam3);
                    else
                        XLS.HucreDegerYaz(satir, 4, 0.00);
                    XLS.HucreDegerYaz(satir, 3, gToplam3);
                    XLS.HucreDegerYaz(satir, 2, gToplamAcilis3);
                    XLS.KoyuYap(satir, 0, satir, 15, true);

                    satir = satir + 3;
                    XLS.HucreKopyala(sablonSatir - 2, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 1, "MALİ YÜKÜMLÜLÜĞÜN TÜRÜ");
                    XLS.HucreBirlestir(satir, 0, satir + 1, 0);
                    XLS.HucreBirlestir(satir, 1, satir + 1, 1);
                    XLS.HucreBirlestir(satir, 2, satir, 4);
                    XLS.HucreBirlestir(satir, 5, satir, 7);
                    XLS.HucreBirlestir(satir, 8, satir, 10);

                    satir++;

                    gToplam1 = 0;
                    gToplam2 = 0;
                    gToplam3 = 0;
                    gToplamAcilis1 = 0;
                    gToplamAcilis2 = 0;
                    gToplamAcilis3 = 0;
                }

                ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-" + f.hesapKod, true);

                satir++;
                XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                XLS.HucreDegerYaz(satir, 0, f.hesapKod);
                XLS.HucreDegerYaz(satir, 1, ad);
                if (f.deger1 != 0)
                    XLS.HucreDegerYaz(satir, 10, f.degerAcilis1 / f.deger1);
                else
                    XLS.HucreDegerYaz(satir, 10, 0.00);
                XLS.HucreDegerYaz(satir, 9, f.deger1);
                XLS.HucreDegerYaz(satir, 8, f.degerAcilis1);
                if (f.deger2 != 0)
                    XLS.HucreDegerYaz(satir, 7, f.degerAcilis2 / f.deger2);
                else
                    XLS.HucreDegerYaz(satir, 7, 0.00);
                XLS.HucreDegerYaz(satir, 6, f.deger2);
                XLS.HucreDegerYaz(satir, 5, f.degerAcilis2);
                if (f.deger3 != 0)
                    XLS.HucreDegerYaz(satir, 4, f.degerAcilis3 / f.deger3);
                else
                    XLS.HucreDegerYaz(satir, 4, 0.00);
                XLS.HucreDegerYaz(satir, 3, f.deger3);
                XLS.HucreDegerYaz(satir, 2, f.degerAcilis3);

                gToplam1 += f.deger1;
                gToplam2 += f.deger2;
                gToplam3 += f.deger3;
                gToplamAcilis1 += f.degerAcilis1;
                gToplamAcilis2 += f.degerAcilis2;
                gToplamAcilis3 += f.degerAcilis3;
            }

            satir++;
            XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
            XLS.HucreBirlestir(satir, 0, satir, 1);
            if (yukumlulukBasla)
                XLS.HucreDegerYaz(satir, 0, "MALİ VARLIKLAR TOPLAMI (A)");
            else
                XLS.HucreDegerYaz(satir, 0, "MALİ YÜKÜMLÜLÜKLER TOPLAMI (B)");
            if (gToplam1 != 0)
                XLS.HucreDegerYaz(satir, 10, gToplamAcilis1 / gToplam1);
            else
                XLS.HucreDegerYaz(satir, 10, 0.00);
            XLS.HucreDegerYaz(satir, 9, gToplam1);
            XLS.HucreDegerYaz(satir, 8, gToplamAcilis1);
            if (gToplam2 != 0)
                XLS.HucreDegerYaz(satir, 7, gToplamAcilis2 / gToplam2);
            else
                XLS.HucreDegerYaz(satir, 7, 0.00);
            XLS.HucreDegerYaz(satir, 6, gToplam2);
            XLS.HucreDegerYaz(satir, 5, gToplamAcilis2);
            if (gToplam3 != 0)
                XLS.HucreDegerYaz(satir, 4, gToplamAcilis3 / gToplam3);
            else
                XLS.HucreDegerYaz(satir, 4, 0.00);
            XLS.HucreDegerYaz(satir, 3, gToplam3);
            XLS.HucreDegerYaz(satir, 2, gToplamAcilis3);
            XLS.KoyuYap(satir, 0, satir, 15, true);

            satir++;
            XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
            XLS.HucreBirlestir(satir, 0, satir, 1);
            XLS.HucreDegerYaz(satir, 0, "NET MALİ DEĞER (A-B)");
            if ((gToplam1Varlik - gToplam1) != 0)
                XLS.HucreDegerYaz(satir, 10, (gToplamAcilis1Varlik - gToplamAcilis1) / (gToplam1Varlik - gToplam1));
            else
                XLS.HucreDegerYaz(satir, 10, 0.00);
            XLS.HucreDegerYaz(satir, 9, gToplam1Varlik - gToplam1);
            XLS.HucreDegerYaz(satir, 8, gToplamAcilis1Varlik - gToplamAcilis1);
            if ((gToplam2Varlik - gToplam2) != 0)
                XLS.HucreDegerYaz(satir, 7, (gToplamAcilis2Varlik - gToplamAcilis2) / (gToplam2Varlik - gToplam2));
            else
                XLS.HucreDegerYaz(satir, 7, 0.00);
            XLS.HucreDegerYaz(satir, 6, gToplam2Varlik - gToplam2);
            XLS.HucreDegerYaz(satir, 5, gToplamAcilis2Varlik - gToplamAcilis2);
            if ((gToplam3Varlik - gToplam3) != 0)
                XLS.HucreDegerYaz(satir, 4, (gToplamAcilis3Varlik - gToplamAcilis3) / (gToplam3Varlik - gToplam3));
            else
                XLS.HucreDegerYaz(satir, 4, 0.00);
            XLS.HucreDegerYaz(satir, 3, gToplam3Varlik - gToplam3);
            XLS.HucreDegerYaz(satir, 2, gToplamAcilis3Varlik - gToplamAcilis3);
            XLS.KoyuYap(satir, 0, satir, 15, true);
        }

        class Sirala : System.Collections.IComparer
        {
            public int Compare(object o1, object o2)
            {
                if (o1 == o2) return 0;
                TNS.MUH.KesinHesapButce i_o1 = (TNS.MUH.KesinHesapButce)o1;
                TNS.MUH.KesinHesapButce i_o2 = (TNS.MUH.KesinHesapButce)o2;
                return (i_o1.hesapKod + i_o1.kod).CompareTo((i_o2.hesapKod + i_o2.kod));
            }
        }

        private void KesinHesapFaaliyetIcerikYaz(OrtakClass.Tablo XLS, object kullanan, int yil, int tur, int saymanlik, string birim, ObjectArray mb, TNS.MUH.OdemeEmriMIFForm oEmri, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2, bool detay, bool herHesapYeniSayfada, int raporTur, int kodTur, int seviye)
        {
            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            int satirGider = 0;
            int satirGelir = 0;
            int satir = 0;
            int sablonSatir = 0;
            int sablonSutun = 0;
            bool baslikYaz810 = true;
            double giderToplami1 = 0;
            double giderToplami2 = 0;
            double giderToplami3 = 0;
            double gelirToplami1 = 0;
            double gelirToplami2 = 0;
            double gelirToplami3 = 0;
            double iadeToplami1 = 0;
            double iadeToplami2 = 0;
            double iadeToplami3 = 0;

            XLS.HucreAdAdresCoz("BaslaSatirGider", ref sablonSatir, ref sablonSutun);
            satirGider = sablonSatir;
            satirGelir = sablonSatir;

            if (raporTur == 31)
            {
                XLS.HucreDegerYaz(sablonSatir - 2, 5, "GİDERİN TÜRÜ");
                XLS.HucreDegerYaz(sablonSatir - 2, 15, "GELİRİN TÜRÜ");
            }
            TNS.BKD.Tertip tertip = new TNS.BKD.Tertip(DateTime.Now.Year, TNS.BKD.ENUMTabloKod.TABButce);

            string[] eKodlar = new string[4];
            string kod = "";
            string ad = "";
            int baslaSutun = 0;

            foreach (TNS.MUH.KesinHesapButce f in mb.objeler)
            {
                string[] kodlar = f.kod.Split('.');

                int degisen = 0;
                if (eKodlar[0] != "")
                {
                    for (int i = 0; i < kodlar.Length; i++)
                    {
                        if (eKodlar.Length > i)
                        {
                            if (eKodlar[i] != kodlar[i])
                            {
                                degisen = i;
                                break;
                            }
                        }
                        else
                        {
                            degisen = i;
                            break;
                        }
                    }
                }

                if ((f.hesapKod.StartsWith("630") || f.hesapKod.StartsWith("830")))
                {
                    baslaSutun = 0;
                    satir = satirGider;
                    giderToplami1 += f.deger1;
                    giderToplami2 += f.deger2;
                    giderToplami3 += f.deger3;
                }
                else
                {
                    baslaSutun = 10;
                    satir = satirGelir;
                    if (f.hesapKod.StartsWith("810"))
                    {
                        iadeToplami1 += f.deger1;
                        iadeToplami2 += f.deger2;
                        iadeToplami3 += f.deger3;
                    }
                    else
                    {
                        gelirToplami1 += f.deger1;
                        gelirToplami2 += f.deger2;
                        gelirToplami3 += f.deger3;
                    }
                }

                if (baslikYaz810 && f.hesapKod.StartsWith("810"))
                {
                    satir = satirGelir;
                    satir++;
                    XLS.HucreKopyala(sablonSatir, baslaSutun, sablonSatir, baslaSutun + 8, satir, baslaSutun);
                    XLS.HucreDegerYaz(satir, baslaSutun, "T O P L A M");
                    XLS.HucreBirlestir(satir, baslaSutun, satir, baslaSutun + 5);
                    XLS.KoyuYap(satir, baslaSutun, satir, baslaSutun + 8, true);//Performans bozuyor
                    XLS.HucreDegerYaz(satir, baslaSutun + 8, gelirToplami1);
                    XLS.HucreDegerYaz(satir, baslaSutun + 7, gelirToplami2);
                    XLS.HucreDegerYaz(satir, baslaSutun + 6, gelirToplami3);

                    satir = satir + 3;
                    XLS.HucreKopyala(sablonSatir - 2, baslaSutun + 0, sablonSatir - 1, baslaSutun + 8, satir, baslaSutun);
                    XLS.HucreDegerYaz(satir, baslaSutun + 5, "RED VE İADENİN TÜRÜ");
                    XLS.HucreBirlestir(satir, baslaSutun + 1, satir, baslaSutun + 4);//Ekonomik yazısı
                    XLS.HucreBirlestir(satir, baslaSutun + 5, satir + 1, baslaSutun + 5);//Aciklama yazısı

                    XLS.HucreDegerYaz(satir, baslaSutun + 6, yil - 2 + " YILI");
                    XLS.HucreDegerYaz(satir, baslaSutun + 7, yil - 1 + " YILI");
                    XLS.HucreDegerYaz(satir, baslaSutun + 8, "CARI YIL (" + yil + ")");

                    satir++;
                    baslikYaz810 = false;
                }


                if (degisen == 0 && kodlar.Length > 0)
                {
                    kod = kodlar[0];
                    ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-" + f.hesapKod + kod, true);
                    double tutar1 = 0;
                    double tutar2 = 0;
                    double tutar3 = 0;
                    foreach (TNS.MUH.KesinHesapButce ff in mb.objeler)
                    {
                        if (ff.kod.Replace(".", "").StartsWith(kod) && ff.hesapKod.StartsWith(f.hesapKod))
                        {
                            tutar1 += ff.deger1;
                            tutar2 += ff.deger2;
                            tutar3 += ff.deger3;
                        }
                    }

                    satir++;
                    XLS.HucreKopyala(sablonSatir, baslaSutun + 0, sablonSatir, baslaSutun + 8, satir, baslaSutun);
                    XLS.HucreDegerYaz(satir, baslaSutun + 0, f.hesapKod);
                    XLS.HucreDegerYaz(satir, baslaSutun + 1, kodlar[0]);
                    XLS.HucreDegerYaz(satir, baslaSutun + 5, ad);
                    XLS.HucreDegerYaz(satir, baslaSutun + 8, tutar1);
                    XLS.HucreDegerYaz(satir, baslaSutun + 7, tutar2);
                    XLS.HucreDegerYaz(satir, baslaSutun + 6, tutar3);
                    XLS.KoyuYap(satir, baslaSutun + 0, satir, baslaSutun + 8, true);
                }
                if (degisen <= 1 && kodlar.Length > 1 && seviye >= 2)
                {
                    kod = kodlar[0] + kodlar[1];
                    ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-" + f.hesapKod + kod, true);
                    double tutar1 = 0;
                    double tutar2 = 0;
                    double tutar3 = 0;
                    foreach (TNS.MUH.KesinHesapButce ff in mb.objeler)
                    {
                        if (ff.kod.Replace(".", "").StartsWith(kod) && ff.hesapKod.StartsWith(f.hesapKod))
                        {
                            tutar1 += ff.deger1;
                            tutar2 += ff.deger2;
                            tutar3 += ff.deger3;
                        }
                    }

                    satir++;
                    XLS.HucreKopyala(sablonSatir, baslaSutun + 0, sablonSatir, baslaSutun + 8, satir, baslaSutun);
                    XLS.HucreDegerYaz(satir, baslaSutun + 0, f.hesapKod);
                    XLS.HucreDegerYaz(satir, baslaSutun + 2, kodlar[1]);
                    XLS.HucreDegerYaz(satir, baslaSutun + 5, ad);
                    XLS.HucreDegerYaz(satir, baslaSutun + 8, tutar1);
                    XLS.HucreDegerYaz(satir, baslaSutun + 7, tutar2);
                    XLS.HucreDegerYaz(satir, baslaSutun + 6, tutar3);
                    XLS.KoyuYap(satir, baslaSutun + 0, satir, baslaSutun + 8, true);
                }
                if (degisen <= 2 && kodlar.Length > 2 && seviye >= 3)
                {
                    kod = kodlar[0] + kodlar[1] + kodlar[2];
                    ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-" + f.hesapKod + kod, true);
                    double tutar1 = 0;
                    double tutar2 = 0;
                    double tutar3 = 0;
                    foreach (TNS.MUH.KesinHesapButce ff in mb.objeler)
                    {
                        if (ff.kod.Replace(".", "").StartsWith(kod) && ff.hesapKod.StartsWith(f.hesapKod))
                        {
                            tutar1 += ff.deger1;
                            tutar2 += ff.deger2;
                            tutar3 += ff.deger3;
                        }
                    }

                    satir++;
                    XLS.HucreKopyala(sablonSatir, baslaSutun + 0, sablonSatir, baslaSutun + 8, satir, baslaSutun);
                    XLS.HucreDegerYaz(satir, baslaSutun + 0, f.hesapKod);
                    XLS.HucreDegerYaz(satir, baslaSutun + 3, kodlar[2]);
                    XLS.HucreDegerYaz(satir, baslaSutun + 5, ad);
                    XLS.HucreDegerYaz(satir, baslaSutun + 8, tutar1);
                    XLS.HucreDegerYaz(satir, baslaSutun + 7, tutar2);
                    XLS.HucreDegerYaz(satir, baslaSutun + 6, tutar3);
                }
                if (degisen <= 3 && kodlar.Length > 3 && seviye >= 4)
                {
                    kod = kodlar[0] + kodlar[1] + kodlar[2] + kodlar[3];
                    ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-" + f.hesapKod + kod, true);
                    double tutar1 = f.deger1;
                    double tutar2 = f.deger2;
                    double tutar3 = f.deger3;

                    satir++;
                    XLS.HucreKopyala(sablonSatir, baslaSutun + 0, sablonSatir, baslaSutun + 8, satir, baslaSutun);
                    XLS.HucreDegerYaz(satir, baslaSutun + 0, f.hesapKod);
                    XLS.HucreDegerYaz(satir, baslaSutun + 4, kodlar[3]);
                    XLS.HucreDegerYaz(satir, baslaSutun + 5, ad);
                    XLS.HucreDegerYaz(satir, baslaSutun + 8, tutar1);
                    XLS.HucreDegerYaz(satir, baslaSutun + 7, tutar2);
                    XLS.HucreDegerYaz(satir, baslaSutun + 6, tutar3);
                }

                if (f.hesapKod.StartsWith("630") || f.hesapKod.StartsWith("830"))
                    satirGider = satir;
                else
                    satirGelir = satir;

                eKodlar = f.kod.Split('.');
            }

            //Gider Toplamını yaz
            satir = satirGider;
            satir++;
            XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 8, satir, 0);
            XLS.HucreDegerYaz(satir, 0, "T O P L A M");
            XLS.HucreBirlestir(satir, 0, satir, 5);
            XLS.KoyuYap(satir, 0, satir, 8, true);
            XLS.HucreDegerYaz(satir, 8, giderToplami1);
            XLS.HucreDegerYaz(satir, 7, giderToplami2);
            XLS.HucreDegerYaz(satir, 6, giderToplami3);

            //Gelir yada ret iade toplamını yaz
            satir = satirGelir;
            satir++;
            XLS.HucreKopyala(sablonSatir, 10, sablonSatir, 18, satir, 10);
            XLS.HucreDegerYaz(satir, 10, "T O P L A M");
            XLS.HucreBirlestir(satir, 10, satir, 15);
            XLS.KoyuYap(satir, 10, satir, 18, true);

            XLS.HucreDegerYaz(satir, 18, gelirToplami1 - iadeToplami1);
            XLS.HucreDegerYaz(satir, 17, gelirToplami2 - iadeToplami2);
            XLS.HucreDegerYaz(satir, 16, gelirToplami3 - iadeToplami3);

            satir = satirGider;
            satir = satir + 5;
            XLS.HucreKopyala(sablonSatir - 2, 6, sablonSatir - 2, 8, satir, 6);
            XLS.HucreDegerYaz(satir, 6, yil - 2 + " YILI");
            XLS.HucreDegerYaz(satir, 7, yil - 1 + " YILI");
            XLS.HucreDegerYaz(satir, 8, "CARI YIL (" + yil + ")");
            if (raporTur == 31)
            {
                satir++;
                XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 8, satir, 0);
                XLS.HucreBirlestir(satir, 0, satir, 5);
                XLS.HucreDegerYaz(satir, 0, "GİDER TOPLAMI");
                XLS.HucreDegerYaz(satir, 8, giderToplami1);
                XLS.HucreDegerYaz(satir, 7, giderToplami2);
                XLS.HucreDegerYaz(satir, 6, giderToplami3);

                satir++;
                XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 8, satir, 0);
                XLS.HucreBirlestir(satir, 0, satir, 5);
                XLS.HucreDegerYaz(satir, 0, "GELİR TOPLAMI");
                XLS.HucreDegerYaz(satir, 8, gelirToplami1);
                XLS.HucreDegerYaz(satir, 7, gelirToplami2);
                XLS.HucreDegerYaz(satir, 6, gelirToplami3);

                satir++;
                XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 8, satir, 0);
                XLS.HucreBirlestir(satir, 0, satir, 5);
                XLS.HucreDegerYaz(satir, 0, "FAALİYET SONUCU");
                XLS.HucreDegerYaz(satir, 8, gelirToplami1 - giderToplami1);
                XLS.HucreDegerYaz(satir, 7, gelirToplami2 - giderToplami2);
                XLS.HucreDegerYaz(satir, 6, gelirToplami3 - giderToplami3);
            }
            else
            {
                satir++;
                XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 8, satir, 0);
                XLS.HucreBirlestir(satir, 0, satir, 5);
                XLS.HucreDegerYaz(satir, 0, "BÜTÇE GİDER TOPLAMI");
                XLS.HucreDegerYaz(satir, 8, giderToplami1);
                XLS.HucreDegerYaz(satir, 7, giderToplami2);
                XLS.HucreDegerYaz(satir, 6, giderToplami3);

                satir++;
                XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 8, satir, 0);
                XLS.HucreBirlestir(satir, 0, satir, 5);
                XLS.HucreDegerYaz(satir, 0, "BÜTÇE GELİR TOPLAMI");
                XLS.HucreDegerYaz(satir, 8, gelirToplami1);
                XLS.HucreDegerYaz(satir, 7, gelirToplami2);
                XLS.HucreDegerYaz(satir, 6, gelirToplami3);

                satir++;
                XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 8, satir, 0);
                XLS.HucreBirlestir(satir, 0, satir, 5);
                XLS.HucreDegerYaz(satir, 0, "RED VE İADE TOPLAMI");
                XLS.HucreDegerYaz(satir, 8, iadeToplami1);
                XLS.HucreDegerYaz(satir, 7, iadeToplami2);
                XLS.HucreDegerYaz(satir, 6, iadeToplami3);

                satir++;
                XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 8, satir, 0);
                XLS.HucreBirlestir(satir, 0, satir, 5);
                XLS.HucreDegerYaz(satir, 0, "NET BÜTÇE GELİRİ");
                XLS.HucreDegerYaz(satir, 8, gelirToplami1 - iadeToplami1);
                XLS.HucreDegerYaz(satir, 7, gelirToplami2 - iadeToplami2);
                XLS.HucreDegerYaz(satir, 6, gelirToplami3 - iadeToplami3);

                satir++;
                XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 8, satir, 0);
                XLS.HucreBirlestir(satir, 0, satir, 5);
                XLS.HucreDegerYaz(satir, 0, "BÜTÇE GELİR-GİDER FARKI");
                XLS.HucreDegerYaz(satir, 8, (gelirToplami1 - iadeToplami1) - giderToplami1);
                XLS.HucreDegerYaz(satir, 7, (gelirToplami2 - iadeToplami2) - giderToplami2);
                XLS.HucreDegerYaz(satir, 6, (gelirToplami3 - iadeToplami3) - giderToplami3);
            }
        }

        private void BilancoIcerikYaz(OrtakClass.Tablo XLS, object kullanan, int yil, int tur, int saymanlik, string birim, ObjectArray mb, TNS.MUH.OdemeEmriMIFForm oEmri, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2, bool detay, bool herHesapYeniSayfada, int raporTur, int kodTur)
        {
            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            int satirAktif = 0;
            int satirPasif = 0;
            int satir = 0;
            int sablonSatir = 0;
            int sablonSutun = 0;
            double aktifToplami1 = 0;
            double aktifToplami2 = 0;
            double aktifToplami3 = 0;
            double pasifToplami1 = 0;
            double pasifToplami2 = 0;
            double pasifToplami3 = 0;
            int carpan = 0;

            XLS.HucreAdAdresCoz("BaslaSatirGider", ref sablonSatir, ref sablonSutun);
            satirAktif = sablonSatir + 1;
            satirPasif = sablonSatir + 1;

            string[] eKodlar = new string[4];
            string kod = "";
            string ad = "";
            int baslaSutun = 0;

            //590 hesapla Eğer kapatma kaydı yapılmamış ise 59* lı hesaplar yoktur. O zaman 630-600 farkından hesaplanır. Olumlu (+) ise 590 olumsuz ise (-) 591
            //Kıbrıs için 630-600 farkı 590'a eklenmeli ki geçen yıl karı/zararı artmış olsun.
            //**************************************************
            double tutar1_59 = 0;
            double tutar2_59 = 0;
            double tutar3_59 = 0;
            double tutar1_590 = 0;
            double tutar2_590 = 0;
            double tutar3_590 = 0;
            double tutar1_591 = 0;
            double tutar2_591 = 0;
            double tutar3_591 = 0;

            //EGO 590 veya 591 fişi yaptıysa (yapmasına gerek yok) türü kapanış olmalı. O zaman hesaplamaya katılmıyor. Melih 01.03.2018

            foreach (TNS.MUH.KesinHesapButce ff in mb.objeler)
            {
                if (ff.kod.Replace(".", "").StartsWith("600"))
                {
                    tutar1_59 -= Math.Abs(ff.deger1);
                    tutar2_59 -= Math.Abs(ff.deger2);
                    tutar3_59 -= Math.Abs(ff.deger3);
                }
                if (ff.kod.Replace(".", "").StartsWith("630"))
                {
                    tutar1_59 += Math.Abs(ff.deger1);
                    tutar2_59 += Math.Abs(ff.deger2);
                    tutar3_59 += Math.Abs(ff.deger3);
                }
            }

            if (tutar1_59 < 0) tutar1_590 = tutar1_59;
            if (tutar2_59 < 0) tutar2_590 = tutar2_59;
            if (tutar3_59 < 0) tutar3_590 = tutar3_59;
            if (tutar1_59 > 0) tutar1_591 = tutar1_59;
            if (tutar2_59 > 0) tutar2_591 = tutar2_59;
            if (tutar3_59 > 0) tutar3_591 = tutar3_59;

            bool bulundu590 = false;
            bool bulundu591 = false;

            foreach (TNS.MUH.KesinHesapButce ff in mb.objeler)
            {
                if (ff.kod.Replace(".", "").StartsWith("590"))
                {
                    //Kıbrıs için 630-600 farkı 590'a eklenmeli ki geçen yıl karı artmış olsun.
                    ff.deger1 += tutar1_590;
                    ff.deger2 += tutar2_590;
                    ff.deger3 += tutar3_590;

                    tutar1_590 = ff.deger1;
                    tutar2_590 = ff.deger2;
                    tutar3_590 = ff.deger3;
                    bulundu590 = true;
                }
                if (ff.kod.Replace(".", "").StartsWith("591"))
                {
                    //Kıbrıs için 630-600 farkı 590'a eklenmeli ki geçen yıl zararı artmış olsun.
                    ff.deger1 += tutar1_591;
                    ff.deger2 += tutar2_591;
                    ff.deger3 += tutar3_591;

                    tutar1_591 = ff.deger1;
                    tutar2_591 = ff.deger2;
                    tutar3_591 = ff.deger3;
                    bulundu591 = true;
                }
            }

            if (!bulundu590)
            {
                TNS.MUH.KesinHesapButce ff1 = new TNS.MUH.KesinHesapButce();
                ff1.deger1 = tutar1_590;
                ff1.deger2 = tutar2_590;
                ff1.deger3 = tutar3_590;
                ff1.hesapKod = "590";
                ff1.kod = "590";
                mb.objeler.Add(ff1);
            }

            if (!bulundu591)
            {
                TNS.MUH.KesinHesapButce ff1 = new TNS.MUH.KesinHesapButce();
                ff1.deger1 = tutar1_591;
                ff1.deger2 = tutar2_591;
                ff1.deger3 = tutar3_591;
                ff1.hesapKod = "591";
                ff1.kod = "591";
                mb.objeler.Add(ff1);
            }
            mb.objeler.Sort(new Sirala());
            //**************************************************

            foreach (TNS.MUH.KesinHesapButce f in mb.objeler)
            {
                string kontrolKod = f.kod;
                if (kontrolKod.Length > 0) f.kod = kontrolKod[0] + "";
                if (kontrolKod.Length > 1) f.kod += "." + kontrolKod[1];
                if (kontrolKod.Length > 2) f.kod += "." + kontrolKod[2];

                string[] kodlar = f.kod.Split('.');

                int degisen = 0;

                if (eKodlar[0] != "")
                {
                    for (int i = 0; i < kodlar.Length; i++)
                    {
                        if (eKodlar[i] != kodlar[i])
                        {
                            degisen = i;
                            break;
                        }
                    }
                }

                if (f.hesapKod.StartsWith("1") || f.hesapKod.StartsWith("2"))
                {
                    carpan = 1;
                    baslaSutun = 0;
                    satir = satirAktif;
                    aktifToplami1 += f.deger1 * carpan;
                    aktifToplami2 += f.deger2 * carpan;
                    aktifToplami3 += f.deger3 * carpan;
                }
                else if (f.hesapKod.StartsWith("3") || f.hesapKod.StartsWith("4") || f.hesapKod.StartsWith("5"))
                {
                    carpan = -1;
                    baslaSutun = 6;
                    satir = satirPasif;
                    pasifToplami1 += f.deger1 * carpan;
                    pasifToplami2 += f.deger2 * carpan;
                    pasifToplami3 += f.deger3 * carpan;
                }
                else
                    continue;

                if (degisen == 0 && kodlar.Length > 0)
                {
                    kod = kodlar[0];
                    ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-" + kod, true);
                    double tutar1 = 0;
                    double tutar2 = 0;
                    double tutar3 = 0;

                    string tempKod = kodlar[0] + kodlar[1] + kodlar[2];


                    foreach (TNS.MUH.KesinHesapButce ff in mb.objeler)
                    {
                        if (ff.kod.Replace(".", "").StartsWith(kod))
                        {
                            tutar1 += ff.deger1 * carpan;
                            tutar2 += ff.deger2 * carpan;
                            tutar3 += ff.deger3 * carpan;
                        }
                    }

                    satir++;
                    XLS.HucreKopyala(sablonSatir, baslaSutun + 0, sablonSatir, baslaSutun + 4, satir, baslaSutun);

                    string kodYaz = BilancoKodBul(kodlar[0]);
                    XLS.HucreDegerYaz(satir, baslaSutun + 0, kodYaz);
                    XLS.HucreDegerYaz(satir, baslaSutun + 1, ad);
                    XLS.HucreDegerYaz(satir, baslaSutun + 4, Math.Round(tutar1, 2));
                    XLS.HucreDegerYaz(satir, baslaSutun + 3, Math.Round(tutar2, 2));
                    XLS.HucreDegerYaz(satir, baslaSutun + 2, Math.Round(tutar3, 2));
                    XLS.KoyuYap(satir, baslaSutun + 0, satir, baslaSutun + 4, true);
                }
                if (degisen <= 1 && kodlar.Length > 1)
                {
                    kod = kodlar[0] + kodlar[1];
                    ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-" + kod, true);
                    double tutar1 = 0;
                    double tutar2 = 0;
                    double tutar3 = 0;

                    string tempKod = kodlar[0] + kodlar[1] + kodlar[2];
                    if (kod.StartsWith("59"))
                    {
                        tutar1 += (tutar1_590 + tutar1_591) * -1;
                        tutar2 += (tutar2_590 + tutar2_591) * -1;
                        tutar3 += (tutar3_590 + tutar3_591) * -1;
                    }
                    else
                    {
                        foreach (TNS.MUH.KesinHesapButce ff in mb.objeler)
                        {
                            if (ff.kod.Replace(".", "").StartsWith(kod))
                            {
                                tutar1 += ff.deger1 * carpan;
                                tutar2 += ff.deger2 * carpan;
                                tutar3 += ff.deger3 * carpan;
                            }
                        }
                    }

                    satir++;
                    XLS.HucreKopyala(sablonSatir, baslaSutun + 0, sablonSatir, baslaSutun + 4, satir, baslaSutun);

                    string kodYaz = BilancoKodBul(kod);
                    XLS.HucreDegerYaz(satir, baslaSutun + 0, kodYaz);
                    XLS.HucreDegerYaz(satir, baslaSutun + 1, ad);
                    XLS.HucreDegerYaz(satir, baslaSutun + 4, Math.Round(tutar1, 2));
                    XLS.HucreDegerYaz(satir, baslaSutun + 3, Math.Round(tutar2, 2));
                    XLS.HucreDegerYaz(satir, baslaSutun + 2, Math.Round(tutar3, 2));
                    XLS.KoyuYap(satir, baslaSutun + 0, satir, baslaSutun + 4, true);
                }
                if (degisen <= 2 && kodlar.Length > 2)
                {
                    kod = kodlar[0] + kodlar[1] + kodlar[2];
                    ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-" + kod, true);
                    double tutar1 = 0;
                    double tutar2 = 0;
                    double tutar3 = 0;

                    foreach (TNS.MUH.KesinHesapButce ff in mb.objeler)
                    {
                        if (ff.kod.Replace(".", "").StartsWith(kod))
                        {
                            tutar1 += ff.deger1 * carpan;
                            tutar2 += ff.deger2 * carpan;
                            tutar3 += ff.deger3 * carpan;
                        }
                    }

                    satir++;
                    XLS.HucreKopyala(sablonSatir, baslaSutun + 0, sablonSatir, baslaSutun + 4, satir, baslaSutun);

                    string kodYaz = BilancoKodBul(kod);
                    XLS.HucreDegerYaz(satir, baslaSutun + 0, kodYaz);
                    XLS.HucreDegerYaz(satir, baslaSutun + 1, ad);
                    XLS.HucreDegerYaz(satir, baslaSutun + 4, Math.Round(tutar1, 2));
                    XLS.HucreDegerYaz(satir, baslaSutun + 3, Math.Round(tutar2, 2));
                    XLS.HucreDegerYaz(satir, baslaSutun + 2, Math.Round(tutar3, 2));
                }

                if (f.hesapKod.StartsWith("1") || f.hesapKod.StartsWith("2"))
                    satirAktif = satir;
                else if (f.hesapKod.StartsWith("3") || f.hesapKod.StartsWith("4") || f.hesapKod.StartsWith("5"))
                    satirPasif = satir;

                eKodlar = f.kod.Split('.');
            }

            satir = satirAktif;
            satir++;
            baslaSutun = 0;
            XLS.HucreKopyala(sablonSatir, baslaSutun + 0, sablonSatir, baslaSutun + 4, satir, baslaSutun);
            XLS.HucreBirlestir(satir, baslaSutun + 0, satir, baslaSutun + 1);
            XLS.HucreDegerYaz(satir, baslaSutun + 0, "AKTİF TOPLAMI");
            XLS.HucreDegerYaz(satir, baslaSutun + 4, Math.Round(aktifToplami1, 2));
            XLS.HucreDegerYaz(satir, baslaSutun + 3, Math.Round(aktifToplami2, 2));
            XLS.HucreDegerYaz(satir, baslaSutun + 2, Math.Round(aktifToplami3, 2));
            XLS.KoyuYap(satir, baslaSutun + 0, satir, baslaSutun + 4, true);

            satir = satirPasif;
            satir++;
            baslaSutun = 6;
            XLS.HucreKopyala(sablonSatir, baslaSutun + 0, sablonSatir, baslaSutun + 4, satir, baslaSutun);
            XLS.HucreBirlestir(satir, baslaSutun + 0, satir, baslaSutun + 1);
            XLS.HucreDegerYaz(satir, baslaSutun + 0, "PASİF TOPLAMI");
            XLS.HucreDegerYaz(satir, baslaSutun + 4, Math.Round(pasifToplami1, 2));
            XLS.HucreDegerYaz(satir, baslaSutun + 3, Math.Round(pasifToplami2, 2));
            XLS.HucreDegerYaz(satir, baslaSutun + 2, Math.Round(pasifToplami3, 2));
            XLS.KoyuYap(satir, baslaSutun + 0, satir, baslaSutun + 4, true);

            satir = satirAktif + 3;
            baslaSutun = 0;
            XLS.HucreDegerYaz(satir, baslaSutun + 0, "Bilanço Dipnotları :");
            XLS.KoyuYap(satir, baslaSutun + 0, satir, baslaSutun + 4, true);

            foreach (TNS.MUH.KesinHesapButce f in mb.objeler)
            {
                if (f.kod.StartsWith("9"))
                {
                    satir++;
                    XLS.HucreKopyala(sablonSatir, baslaSutun + 0, sablonSatir, baslaSutun + 4, satir, baslaSutun);
                    XLS.HucreDegerYaz(satir, baslaSutun + 0, f.kod.Replace(".", ""));
                    ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-" + f.kod, true);

                    XLS.HucreDegerYaz(satir, baslaSutun + 1, ad);
                    XLS.HucreDegerYaz(satir, baslaSutun + 4, Math.Abs(Math.Round(f.deger1, 2)));
                    XLS.HucreDegerYaz(satir, baslaSutun + 3, Math.Abs(Math.Round(f.deger2, 2)));
                    XLS.HucreDegerYaz(satir, baslaSutun + 2, Math.Abs(Math.Round(f.deger3, 2)));
                }
            }


        }

        string BilancoKodBul(string kod)
        {
            string[] romaRakam = { "0", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };
            string[] alfabe = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N" };

            string kodYaz = kod;
            if (kodYaz.Length == 1)
                kodYaz = romaRakam[OrtakFonksiyonlar.ConvertToInt(kodYaz, 0)];
            else if (kodYaz.Length == 2)
                kodYaz = alfabe[OrtakFonksiyonlar.ConvertToInt(kodYaz.Substring(1, 1), 0)];
            //else if (kodYaz.Length == 3)
            //    kodYaz = (OrtakFonksiyonlar.ConvertToInt(kodYaz.Substring(2, 1), 0) + 1).ToString();

            return kodYaz;
        }
    }

    /// <summary>
    /// Kesin Hesap Ödenek rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class KesinHesapOdenek
    {
        OrtakClass.Tablo XLS;
        IMUHServis s = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Kesin Hesap Ödenek raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            string raporTur = (string)raporParam["RaporTur"];

            string basHesap = (string)raporParam["BasHesap"];
            string bitHesap = (string)raporParam["BitHesap"];

            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdBulYaz("ButceYil", yil.ToString());
            XLS.HucreAdBulYaz("KurumAd", kurum + " - " + kurumAd);
            XLS.HucreAdBulYaz("ParaBirimi", ButceMuhasebeGenel.ParaBirimiDondur());

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            if (tur == 1)
            {
                oEmri.formDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
                oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else if (tur == 2)
            {
                oEmri.formDurum = (int)ENUMBelgeDurumu.ONAYLI;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else
            {
                oEmri.formDurum = 22;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            oEmri.fisTurYap(fisTur);
            oEmri.sorguEkoSiraliMi = true;

            TNS.MUH.OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(yil);
            oEmriDetay.hesapKod = "901;830;165";
            oEmriDetay.kur = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            oEmriDetay.fon = "";
            oEmriDetay.fin = "";
            oEmriDetay.eko = "";
            oEmri.detay.objeler.Add(oEmriDetay);
            int kodTur = (int)ENUMKesinHesapButce.EKONOMIK2;
            ObjectArray mb = s.KesinHesapButce((TNS.KYM.Kullanici)kullanan, oEmri, kodTur);

            KesinHesapOdenekIcerikYaz(XLS, kullanan, yil, tur, saymanlik, birim, mb, oEmri, tarih1, tarih2, true, true, raporTur);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void KesinHesapOdenekIcerikYaz(OrtakClass.Tablo XLS, object kullanan, int yil, int tur, int saymanlik, string birim, ObjectArray mb, TNS.MUH.OdemeEmriMIFForm oEmri, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2, bool detay, bool herHesapYeniSayfada, string raporTur)
        {
            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            int satir = 0;
            int sablonSatir = 0;
            int sablonSutun = 0;
            string[] kolonDeger = new string[5];
            kolonDeger[0] = "90101;90103;90104;90105;90106;90108";
            kolonDeger[1] = "90109;90111;90113;90115;90117";
            kolonDeger[2] = "90112;90114;90118;90110";
            kolonDeger[3] = "830";
            kolonDeger[4] = "165";

            XLS.HucreAdAdresCoz("BaslaSatir", ref sablonSatir, ref sablonSutun);
            satir = sablonSatir;

            string[] eKodlar = new string[4];
            string kod = "";
            string ad = "";

            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            ad = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

            satir++;
            XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
            XLS.HucreBirlestir(satir, 0, satir, 1);
            XLS.HucreDegerYaz(satir, 0, kurum);
            XLS.HucreDegerYaz(satir, 4, ad);
            SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, kurum, "", "", "", oEmri, kolonDeger);
            XLS.KoyuYap(satir, 0, satir, 14, true);

            foreach (TNS.MUH.KesinHesapButce f in mb.objeler)
            {
                string[] kodlar = f.kod.Split('.');

                if (f.kod.Trim() == "")
                    continue;

                int degisen = 0;
                if (eKodlar[0] != "")
                {
                    for (int i = 0; i < kodlar.Length; i++)
                    {
                        if (eKodlar[i] != kodlar[i])
                        {
                            degisen = i;
                            break;
                        }
                    }
                }

                if (degisen == 0 && kodlar.Length > 0 && kodlar[0] != "")
                {
                    kod = kodlar[0];
                    //string eko = kodlar[0];
                    ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "ABSEKO", oEmri.yil.ToString() + "-" + kod, true);

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 0, kodlar[0]);
                    XLS.HucreDegerYaz(satir, 4, ad);
                    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, "", "", "", kod, oEmri, kolonDeger);
                    XLS.KoyuYap(satir, 0, satir, 14, true);
                }
                if (degisen <= 1 && kodlar.Length > 1)
                {
                    kod = kodlar[0] + kodlar[1];
                    //string eko = kodlar[0] + OrtakFonksiyonlar.ConvertToInt(kodlar[1], 0);
                    ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "ABSEKO", oEmri.yil.ToString() + "-" + kod, true);

                    satir++;
                    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                    XLS.HucreDegerYaz(satir, 1, kodlar[1]);
                    XLS.HucreDegerYaz(satir, 4, ad);
                    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, "", "", "", kod, oEmri, kolonDeger);
                }
                //if (degisen <= 2 && kodlar.Length > 2)
                //{
                //    kod = kodlar[0] + kodlar[1] + kodlar[2];
                //    string eko = kodlar[0] + OrtakFonksiyonlar.ConvertToInt(kodlar[1], 0) + kodlar[2];
                //    ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-" + f.hesapKod + kod, true);

                //    satir++;
                //    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                //    XLS.HucreDegerYaz(satir, 2, kodlar[2]);
                //    XLS.HucreDegerYaz(satir, 4, ad);
                //    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, "", "", "", eko, oEmri, kolonDeger);
                //}
                //if (degisen <= 3 && kodlar.Length > 3)
                //{
                //    kod = kodlar[0] + kodlar[1] + kodlar[2] + kodlar[3];
                //    string eko = kodlar[0] + OrtakFonksiyonlar.ConvertToInt(kodlar[1], 0) + OrtakFonksiyonlar.ConvertToInt(kodlar[2], 0) + kodlar[3];
                //    ad = servisUZY.UzayDegeriStr((TNS.KYM.Kullanici)kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-" + f.hesapKod + kod, true);

                //    satir++;
                //    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 30, satir, 0);
                //    XLS.HucreDegerYaz(satir, 3, kodlar[3]);
                //    XLS.HucreDegerYaz(satir, 4, ad);
                //    SayisalDegerYaz((TNS.KYM.Kullanici)kullanan, satir, "", "", "", eko, oEmri, kolonDeger);
                //    XLS.KoyuYap(satir, 0, satir, 12, true);
                //}
                eKodlar = f.kod.Split('.');
            }
        }

        private void SayisalDegerYaz(TNS.KYM.Kullanici kullanan, int satir, string kur, string fon, string fin, string eko, OdemeEmriMIFForm form, string[] kolonDeger)
        {
            ((OdemeEmriMIFDetay)form.detay[0]).kur = kur;
            ((OdemeEmriMIFDetay)form.detay[0]).fon = fon;
            ((OdemeEmriMIFDetay)form.detay[0]).fin = fin;
            ((OdemeEmriMIFDetay)form.detay[0]).eko = eko;
            ObjectArray mb = s.KesinHesapGiderRakamVer(kullanan, form, kolonDeger);
            for (int i = 5; i < 13; i++)
                XLS.HucreDegerYaz(satir, i, 0.00);

            double odemeEmri = 0.00;
            double eklenen = 0.00;
            double tenkis = 0.00;
            double odeme = 0.00;
            double mahsup = 0.00;
            int kolonSay = 0;
            //Gelen değerler alacak-borç olduğu için -1 ile çarpılıyor
            foreach (double deger in mb.objeler)
            {
                if (kolonSay == 0)
                    odemeEmri = deger;
                if (kolonSay == 1)
                    eklenen = deger;
                if (kolonSay == 2)
                    tenkis = deger * -1;
                if (kolonSay == 3)
                    odeme = deger * -1;
                if (kolonSay == 4)
                    mahsup = deger * -1;

                kolonSay++;
            }
            XLS.HucreDegerYaz(satir, 5, odemeEmri);
            XLS.HucreDegerYaz(satir, 6, eklenen);
            XLS.HucreDegerYaz(satir, 7, tenkis);
            XLS.HucreDegerYaz(satir, 8, (odemeEmri + eklenen - tenkis));
            XLS.HucreDegerYaz(satir, 9, odeme);
            XLS.HucreDegerYaz(satir, 10, mahsup);
            XLS.HucreDegerYaz(satir, 11, (odeme + mahsup));
            double odenekUstu = (odemeEmri + eklenen - tenkis) - (odeme + mahsup);
            if (odenekUstu > 0)
                odenekUstu = 0;
            XLS.HucreDegerYaz(satir, 12, odenekUstu);
            double odenekToplam = odemeEmri + eklenen - tenkis;
            double harcamaToplam = odeme + mahsup;
            XLS.HucreDegerYaz(satir, 13, Math.Round(odenekToplam - harcamaToplam, 2));

            if (odenekToplam > 0)
                XLS.HucreDegerYaz(satir, 14, Math.Round(harcamaToplam / odenekToplam * 100, 2));
            else
                XLS.HucreDegerYaz(satir, 14, 0);
        }
    }

    /// <summary>
    /// Kesin Hesap Ödenek rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class ButceGiderleriOdenekPrg
    {
        OrtakClass.Tablo XLS;
        IMUHServis s = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Kesin Hesap Ödenek raporu üreten fonksiyon
        /// </summary>
        /// <param name="p_kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object p_kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.KYM.Kullanici kullanan = (TNS.KYM.Kullanici)p_kullanan;
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string saymanlikAd = TNS.UZY.Arac.Tanimla().UzayDegeriStr(kullanan, "MUHMUHASEBE", saymanlik.ToString(), true);
            string raporTur = (string)raporParam["RaporTur"];
            string kurKod1 = "", kurKod2 = "";
            OrtakFonksiyonlar.StringiIkiyeBol(kurum, ".", ref kurKod1, ref kurKod2);

            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);

            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdBulYaz("butceYil", yil.ToString());
            XLS.HucreAdBulYaz("kurumAd", kurumAd);
            if (raporTur == "37.1")
            {
                XLS.HucreAdBulYaz("muhKod", saymanlik);
                XLS.HucreAdBulYaz("muhAd", saymanlikAd);

                XLS.HucreAdBulYaz("kurKod1", kurKod1);
                XLS.HucreAdBulYaz("kurKod2", kurKod2);
            }
            TNS.BHMv2.IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();


            TNS.BKD.Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kurum, "", "", "", "");

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(14, kistas.KodNoKirilimSay(4)));
            if (raporTur == "37.1")
            {
                kodBilgileri.Add(new UcgenKodBilgi(3, 1));
                kodBilgileri.Add(new UcgenKodBilgi(4, 2));
            }
            EkDegerParam[] ekBilgiler = new EkDegerParam[3];
            if (raporTur == "37.1")
            {
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABHarcama, 0, 0, "12;90101,90103,90104,90105,90106,90108,90109,90111,90113,90115,90117,90119");
                ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABHarcama, 0, 0, "12;90112,90114,90118,90110");
                ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABHarcama, 0, 0, "12;830");
            }
            else
            {
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABHarcama, 2, 0, "12;830");
                ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABHarcama, 1, 0, "12;830");
                ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABHarcama, 0, 0, "12;830");
            }
            int satir = 0, sutun = 0;
            XLS.HucreAdAdresCoz("baslaSatir", ref satir, ref sutun);
            int satirYuk = XLS.SatirYukseklikAl(satir);
            int fsat1 = 0, fsut1 = 0, fsat2 = 0, fsut2 = 0, fsayfa = 0;
            XLS.HucreAdAdresCoz("formatSatir", ref fsayfa, ref fsat1, ref fsut1, ref fsat2, ref fsut2);

            int rsat1 = 0, rsut1 = 0, rsat2 = 0, rsut2 = 0, rsayfa = 0;
            XLS.HucreAdAdresCoz("sayfaSonu", ref rsayfa, ref rsat1, ref rsut1, ref rsat2, ref rsut2);

            OdenekBilgi[] a = servisBHMv2.ListeTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, true);
            ProgramaCevir(yil, a);
            double[] toplamlar = { 0, 0, 0 };
            int basSatir = satir;
            foreach (OdenekBilgi ob in a)
            {
                int sut = sutun;
                XLS.HucreKopyala(fsat1, fsut1, fsat2, fsut2, satir, sut);

                string[] kodlar;


                if (raporTur == "37.1")
                {
                    kodlar = new string[6];
                    KodYaz(ob.tertip.kodlar[1], kodlar, 0, 3);
                    KodYaz(ob.tertip.kodlar[2], kodlar, 3, 1);
                    KodYaz(ob.tertip.kodlar[3], kodlar, 4, 2);
                }
                else
                {
                    kodlar = new string[3];
                    KodYaz(ob.tertip.kodlar[1], kodlar, 0, 3);
                }

                foreach (string item in kodlar)
                {
                    XLS.HucreDegerYaz(satir, sut++, item);
                }
                if (raporTur == "37.2")
                    XLS.HucreDegerYaz(satir, sut++, ob.tertip.adlar[4]);
                else
                    sut++;
                if (raporTur == "37.2")
                {
                    KurusYaz(XLS, satir, sut, ob.EkRakam(0));
                    sut += 2;
                    KurusYaz(XLS, satir, sut, ob.EkRakam(1));
                    sut += 2;
                    KurusYaz(XLS, satir, sut, ob.EkRakam(2));
                    sut += 2;
                    toplamlar[0] += ob.EkRakam(0);
                    toplamlar[1] += ob.EkRakam(1);
                    toplamlar[2] += ob.EkRakam(2);
                }
                else
                {
                    double alinan = -1 * ob.EkRakam(0);

                    KurusYaz(XLS, satir, sut, alinan);
                    sut += 2;
                    KurusYaz(XLS, satir, sut, ob.EkRakam(1));
                    sut += 2;
                    double kalan = alinan - ob.EkRakam(1);
                    KurusYaz(XLS, satir, sut, kalan);
                    sut += 2;
                    KurusYaz(XLS, satir, sut, ob.EkRakam(2));
                    if (ob.EkRakam(2) > kalan)
                    {
                        sut += 6;
                        KurusYaz(XLS, satir, sut, ob.EkRakam(2) - kalan);
                    }
                }
                satir++;
            }
            XLS.SatirYukseklikAyarla(basSatir, satir - 1, -1, 5, (int)(satirYuk * 1.1));

            XLS.HucreKopyala(rsat1, rsut1, rsat2, rsut2, satir, sutun);
            if (raporTur == "37.2")
            {
                int sut = sutun + 4;
                KurusYaz(XLS, satir, sut, toplamlar[0]);
                sut += 2;
                KurusYaz(XLS, satir, sut, toplamlar[1]);
                sut += 2;
                KurusYaz(XLS, satir, sut, toplamlar[2]);
                sut += 2;
                XLS.SatirYukseklikAyarla(satir, satir, -1, 5, (int)(satirYuk * 1.3));
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void KodYaz(string kod, string[] parcalalar, int baslaInd, int enFazla)
        {
            int yazilan = 0;
            string[] kodParcalar = kod.Split('.');
            for (; yazilan < kodParcalar.Length && yazilan < enFazla; yazilan++)
                parcalalar[baslaInd + yazilan] = kodParcalar[yazilan];
            for (; yazilan < enFazla; yazilan++)
                parcalalar[baslaInd + yazilan] = "";
        }

        private void ProgramaCevir(int yil, OdenekBilgi[] a)
        {
            foreach (OdenekBilgi ob in a)
            {
                string tKod = TNS.BHMv2.Arac.ProgramTertipOl(ob.tertip, false);
                string[] tKodParca = tKod.Split('-');
                string pkod = "";
                if (tKodParca.Length > 0)
                    pkod = tKodParca[0];
                if (string.IsNullOrWhiteSpace(pkod)) pkod = ob.tertip.kodlar[4];
                ob.tertip.kodlar[1] = pkod;
            }
            Array.Sort(a, new FonFinEkoSirala());
        }

        private void KurusYaz(Tablo XLS, int satir, int sut, double miktar)
        {
            XLS.HucreDegerYaz(satir, sut, Math.Truncate(miktar));
            XLS.HucreDegerYaz(satir, sut + 1, GenelIslemler.KurusBul(miktar));
        }

        private class FonFinEkoSirala : IComparer
        {
            public int Compare(object x, object y)
            {
                OdenekBilgi xob = (OdenekBilgi)x;
                OdenekBilgi yob = (OdenekBilgi)y;

                int snc = Kiyasla(xob.tertip.kodlar, yob.tertip.kodlar, 1);
                if (snc != 0) return snc;
                snc = Kiyasla(xob.tertip.kodlar, yob.tertip.kodlar, 2);
                if (snc != 0) return snc;
                return Kiyasla(xob.tertip.kodlar, yob.tertip.kodlar, 3);
            }

            private int Kiyasla(string[] kodlar1, string[] kodlar2, int diziYer)
            {
                string k1 = OrtakFonksiyonlar.DiziElemanAl(kodlar1, diziYer, "");
                string k2 = OrtakFonksiyonlar.DiziElemanAl(kodlar2, diziYer, "");
                return k1.CompareTo(k2);
            }
        }
    }

    /// <summary>
    /// Yevmiye rapor işlemlerini düzenleyen sınıf
    /// </summary>
    public class DamgaVergisiBeyannamesi
    {
        OrtakClass.Tablo XLS;
        string kurumAdi = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

        /// <summary>
        /// Yevmiye raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];

            string basHesap = (string)raporParam["BasHesap"];

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            oEmri.sorguTarihSiraliMi = true;

            oEmri.formDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
            oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();

            oEmri.fisTurYap(0);
            OdemeEmriMIFDetay det = new OdemeEmriMIFDetay(yil);
            det.hesapKod = basHesap + "|";
            oEmri.detay.Ekle(det);
            //OrtakFonksiyonlar.HataStrYaz("Rapor Listele Başla" + DateTime.Now.ToString());
            ObjectArray mb = s.OdemeEmriMIFListeleTumDetay((TNS.KYM.Kullanici)kullanan, oEmri);
            //OrtakFonksiyonlar.HataStrYaz("Rapor Listele Bitti" + DateTime.Now.ToString());

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS = GenelIslemler.NewTablo();
            XLS.BosDosyaAc(sonucDosyaAd);
            int sat = 0;
            foreach (OdemeEmriMIFForm form in mb.objeler)
            {
                int sut = 0;
                XLS.HucreDegerYaz(sat, sut++, sat + 1);
                XLS.HucreDegerYaz(sat, sut++, form.YEVYevTarih.ToString());
                XLS.HucreDegerYaz(sat, sut++, form.belgeNo);
                XLS.HucreDegerYaz(sat, sut++, form.YEVYevNo);
                XLS.HucreDegerYaz(sat, sut++, form.aciklama);
                XLS.HucreDegerYaz(sat, sut++, form.ilgiliAd);
                XLS.HucreDegerYaz(sat, sut++, form.OEBNo);
                XLS.HucreDegerYaz(sat, sut++, form.OEBTarih.ToString());
                XLS.HucreDegerYaz(sat, sut++, form.OEBTur);
                XLS.HucreDegerYaz(sat, sut++, form.OEBTutar);
                double butceGideri = form.ToplamAl("830", true);
                double damgaVergisi = form.ToplamAl(basHesap, false);
                double odenen = form.ToplamAl("103", false);
                XLS.HucreDegerYaz(sat, sut++, butceGideri);
                XLS.HucreDegerYaz(sat, sut++, odenen);
                XLS.HucreDegerYaz(sat, sut++, damgaVergisi);
                sat++;
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        //void YevmiyeDefteriIcerikYaz(OrtakClass.Tablo XLS, object kullanan, int yil, int tur, int saymanlik, string birim, ObjectArray mb, OdemeEmriMIFDetay oncekiTarihToplam, TNS.MUH.OdemeEmriMIFForm oEmri, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2, int listeTur)
        //{
        //    string saymanlikAd = "";
        //    string birimAd = "";
        //    string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
        //    string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

        //    //Boş sayfa yarat ve 0. sayfayı şablon olarak kullan
        //    //*****************************************************
        //    int sayfadakiSatirSayisi = 50000;
        //    int aktifSayfa = 0;

        //    if (mb.objeler.Count > 10000)//Başlık yazmada problem var. Başlık 0 sheetine yazıyor.
        //    {
        //        XLS.YeniSheetEkle(0);
        //        aktifSayfa++;
        //        XLS.AktifSheetDegistir(aktifSayfa);
        //    }
        //    //*****************************************************

        //    BaslikYaz(XLS, yil, saymanlik, saymanlikAd, kurum, kurumAd, birim, birimAd, tarih1, tarih2, listeTur);

        //    int satir = 0;
        //    sutun = 0;
        //    XLS.HucreAdAdresCoz("BaslaHucre", ref satir, ref sutun);
        //    int sablonSatir = satir - 1;


        //    TNSDateTime belgeTarih = new TNSDateTime();
        //    TNSDateTime kontrolTarih = new TNSDateTime();
        //    double gunBorcToplam = 0.0;
        //    double gunAlacakToplam = 0.0;
        //    double ayBorcToplam = 0.0;
        //    double ayAlacakToplam = 0.0;
        //    double ayKumulatifBorcToplam = oncekiTarihToplam.borc;
        //    double ayKumulatifAlacakToplam = oncekiTarihToplam.alacak;
        //    double genelBorcToplam = 0.0;
        //    double genelAlacakToplam = 0.0;

        //    double yBorc = 0.00;
        //    double yAlacak = 0.00;
        //    foreach (OdemeEmriMIFForm f in mb.objeler)
        //    {
        //        //liste satır sayısını aşıyor ise yeni sayfaya geç
        //        if (satir + 1000 > sayfadakiSatirSayisi)
        //        {
        //            XLS.HucreAdAdresCoz("BaslaHucre", ref satir, ref sutun);
        //            XLS.YeniSheetEkle(0);
        //            aktifSayfa++;
        //            XLS.AktifSheetDegistir(aktifSayfa);
        //            BaslikYaz(XLS, yil, saymanlik, saymanlikAd, kurum, kurumAd, birim, birimAd, tarih1, tarih2, listeTur);
        //        }

        //        ////---------------->
        //        if (tur == 1)
        //        {
        //            if (belgeTarih.isNull) belgeTarih = f.YEVYevTarih;
        //            kontrolTarih = f.YEVYevTarih;
        //        }
        //        else if (tur == 3 || tur == 2)
        //        {
        //            if (belgeTarih.isNull) belgeTarih = f.islemTarih;
        //            kontrolTarih = f.islemTarih;
        //        }



        //        if (belgeTarih.ToString() != kontrolTarih.ToString())
        //        {
        //            XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);
        //            XLS.HucreBirlestir(satir, 0, satir, 7);
        //            XLS.HucreDegerYaz(satir, 0, "GÜN TOPLAMI");
        //            XLS.HucreDegerYaz(satir, 8, gunBorcToplam);
        //            XLS.HucreDegerYaz(satir, 9, gunAlacakToplam);

        //            satir++;
        //        }

        //        if (belgeTarih.Ay != kontrolTarih.Ay)
        //        {
        //            XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);
        //            XLS.HucreBirlestir(satir, 0, satir, 7);
        //            XLS.HucreDegerYaz(satir, 0, "AY TOPLAMI");
        //            XLS.HucreDegerYaz(satir, 8, ayBorcToplam);
        //            XLS.HucreDegerYaz(satir, 9, ayAlacakToplam);

        //            satir++;
        //        }

        //        if (belgeTarih.Ay != kontrolTarih.Ay)
        //        {
        //            if (ayKumulatifBorcToplam > 0.0 || ayKumulatifAlacakToplam > 0.0)
        //            {
        //                XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);
        //                XLS.HucreBirlestir(satir, 0, satir, 7);
        //                XLS.HucreDegerYaz(satir, 0, "ÖNCEKİ AYLARDAN DEVİR");
        //                XLS.HucreDegerYaz(satir, 8, ayKumulatifBorcToplam);
        //                XLS.HucreDegerYaz(satir, 9, ayKumulatifAlacakToplam);

        //                satir++;

        //                genelBorcToplam += ayKumulatifBorcToplam;
        //                genelAlacakToplam += ayKumulatifAlacakToplam;
        //            }

        //            ayKumulatifBorcToplam += ayBorcToplam;
        //            ayKumulatifAlacakToplam += ayAlacakToplam;
        //        }
        //        ////---------------->


        //        XLS.HucreKopyala(sablonSatir - 1, 0, sablonSatir - 1, 15, satir, 0);
        //        string bTur = "";
        //        if (f.belgeTur == ENUMBelgeTuru.ODEMEEMRI)
        //            bTur = "ÖDE";
        //        else
        //            bTur = "MİF";

        //        if (tur == 1)
        //        {
        //            XLS.HucreDegerYaz(satir, 0, f.YEVYevTarih.ToString());
        //            XLS.HucreDegerYaz(satir, 1, f.YEVYevNo);
        //        }
        //        else if (tur == 3 || tur == 2)
        //        {
        //            XLS.HucreDegerYaz(satir, 0, f.islemTarih.ToString());
        //            XLS.HucreDegerYaz(satir, 1, f.belgeNo + "-" + bTur);
        //        }


        //        XLS.HucreDegerYaz(satir, 7, f.aciklama);

        //        yBorc = 0.00;
        //        yAlacak = 0.00;
        //        int anaSatir = satir;

        //        satir++;
        //        foreach (OdemeEmriMIFDetay d in f.detay.objeler)
        //        {
        //            XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);

        //            XLS.HucreDegerYaz(satir, 2, d.hesapKod);
        //            if (!String.IsNullOrEmpty(d.kur))
        //            {
        //                XLS.HucreDegerYaz(satir, 3, d.kur);
        //                XLS.HucreDegerYaz(satir, 4, d.fon);
        //                XLS.HucreDegerYaz(satir, 5, d.fin);
        //                XLS.HucreDegerYaz(satir, 6, d.eko);
        //                //XLS.HucreDegerYaz(satir, 6, d.hesapKod.Substring(4));
        //            }
        //            XLS.HucreDegerYaz(satir, 7, d.hesapAd);
        //            if (listeTur == 1)
        //            {
        //                XLS.HucreDegerYaz(satir, 8, d.borc);
        //                XLS.HucreDegerYaz(satir, 9, d.alacak);
        //                yBorc += d.borc;
        //                yAlacak += d.alacak;
        //            }
        //            else
        //            {
        //                XLS.HucreDegerYaz(satir, 8, d.dovizBorc);
        //                XLS.HucreDegerYaz(satir, 9, d.dovizAlacak);
        //                yBorc += d.dovizBorc;
        //                yAlacak += d.dovizAlacak;
        //            }

        //            satir++;
        //        }

        //        if (belgeTarih.Ay == kontrolTarih.Ay)
        //        {
        //            ayBorcToplam += yBorc;
        //            ayAlacakToplam += yAlacak;
        //        }
        //        else
        //        {
        //            ayBorcToplam = yBorc;
        //            ayAlacakToplam = yAlacak;
        //        }

        //        if (belgeTarih.ToString() == kontrolTarih.ToString())
        //        {
        //            gunBorcToplam += yBorc;
        //            gunAlacakToplam += yAlacak;
        //        }
        //        else
        //        {
        //            gunBorcToplam = yBorc;
        //            gunAlacakToplam = yAlacak;

        //            if (tur == 1)
        //            {
        //                belgeTarih = f.YEVYevTarih;
        //            }
        //            else if (tur == 3 || tur == 2)
        //            {
        //                belgeTarih = f.islemTarih;
        //            }
        //        }

        //        XLS.HucreDegerYaz(anaSatir, 8, yBorc);
        //        XLS.HucreDegerYaz(anaSatir, 9, yAlacak);

        //        genelBorcToplam += yBorc;
        //        genelAlacakToplam += yAlacak;
        //    }

        //    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);
        //    XLS.HucreBirlestir(satir, 0, satir, 7);
        //    XLS.HucreDegerYaz(satir, 0, "GÜN TOPLAMI");
        //    XLS.HucreDegerYaz(satir, 8, gunBorcToplam);
        //    XLS.HucreDegerYaz(satir, 9, gunAlacakToplam);
        //    satir++;

        //    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);
        //    XLS.HucreBirlestir(satir, 0, satir, 7);
        //    XLS.HucreDegerYaz(satir, 0, "AY TOPLAMI");
        //    XLS.HucreDegerYaz(satir, 8, ayBorcToplam);
        //    XLS.HucreDegerYaz(satir, 9, ayAlacakToplam);
        //    satir++;

        //    if (ayKumulatifBorcToplam > 0.0 || ayKumulatifAlacakToplam > 0.0)
        //    {
        //        XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);
        //        XLS.HucreBirlestir(satir, 0, satir, 7);
        //        XLS.HucreDegerYaz(satir, 0, "ÖNCEKİ AYLARDAN DEVİR");
        //        XLS.HucreDegerYaz(satir, 8, ayKumulatifBorcToplam);
        //        XLS.HucreDegerYaz(satir, 9, ayKumulatifAlacakToplam);
        //        satir++;
        //    }

        //    XLS.HucreKopyala(sablonSatir, 0, sablonSatir, 15, satir, 0);
        //    XLS.HucreBirlestir(satir, 0, satir, 7);
        //    XLS.HucreDegerYaz(satir, 0, "GENEL TOPLAM");
        //    XLS.HucreDegerYaz(satir, 8, genelBorcToplam);
        //    XLS.HucreDegerYaz(satir, 9, genelAlacakToplam);
        //    satir++;

        //}

        //void BaslikYaz(OrtakClass.Tablo XLS, int yil, int saymanlik, string saymanlikAd, string kurum, string kurumAd, string birim, string birimAd, TNSDateTime tarih1, TNSDateTime tarih2, int listeTur)
        //{
        //    XLS.HucreAdBulYaz("Yil", " : " + yil.ToString());
        //    XLS.HucreAdBulYaz("KurumAd", " : " + kurum + " - " + kurumAd);

        //    if (saymanlik > 0)
        //    {
        //        saymanlikAd = OrtakClass.GenelIslemler.KodAd(11, saymanlik.ToString(), true);
        //        XLS.HucreAdBulYaz("SaymanlikAd", " : " + saymanlik + " - " + saymanlikAd);
        //    }
        //    else
        //        XLS.HucreAdBulYaz("SaymanlikAd", " : Kurum Geneli");

        //    if (birim.Trim() != "")
        //    {
        //        birimAd = OrtakClass.GenelIslemler.KodAd(12, birim + "-" + saymanlik.ToString(), false);
        //        XLS.HucreAdBulYaz("BirimAdi", " : " + birim + " - " + birimAd);
        //    }
        //    else
        //        XLS.HucreAdBulYaz("BirimAdi", " : Kurum Geneli");

        //    XLS.HucreAdBulYaz("Bastar", tarih1.ToString());
        //    XLS.HucreAdBulYaz("Bittar", tarih2.ToString());

        //    string paraBirimi = ButceMuhasebeGenel.DondurParaBirimi(1).kisaltma;
        //    if (listeTur == 2) paraBirimi = "DÖVİZ";
        //    XLS.HucreAdBulYaz("ParaBirimi", paraBirimi);

        //}

        //void YevmiyeDefteriDosyayaYaz(OrtakClass.Tablo XLS, object kullanan, int yil, int tur, int saymanlik, string birim, ObjectArray mb, TNS.MUH.OdemeEmriMIFForm oEmri, TNS.TNSDateTime tarih1, TNS.TNSDateTime tarih2, int sayistay, string sonucDosyaAd)
        //{
        //    StringBuilder rapor = new StringBuilder();
        //    double tBorc = 0;
        //    double tAlacak = 0;
        //    int sayfaNo = 0;
        //    int satir = 1;
        //    string kolonDeger = "";

        //    string kolon = "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}";
        //    string baslik = "Tarih  Numara     Hesap Kodu             Kurumsal    Fonksiyon F Açıklama                   Borç            Alacak\r\n";
        //    baslik += "------ ---------- ---------------------- ----------- --------- - -------------------------- --------------- ---------------";

        //    if (sayistay > 0)
        //    {
        //        kolon = "{9}|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}";
        //        baslik = "";
        //    }

        //    sayfaNo++;
        //    if (sayistay == 0)
        //        BaslikYazDosya(rapor, raporAdi, kurumAdi, sayfaNo, baslik);

        //    satir = 6;

        //    foreach (OdemeEmriMIFForm f in mb.objeler)
        //    {
        //        string kolon0 = "";
        //        string kolon1 = "";
        //        string kolon2 = "";
        //        string kolon3 = "";
        //        string kolon4 = "";
        //        string kolon5 = "";
        //        string kolon6 = "";
        //        string kolon7 = "";
        //        string kolon8 = "";
        //        string kolon9 = "";

        //        f.aciklama = f.aciklama.Replace('\r', ' ');
        //        f.aciklama = f.aciklama.Replace('\n', ' ');
        //        f.aciklama = f.aciklama.Trim();

        //        if (sayistay == 0)
        //        {
        //            if (f.aciklama.Length > 26)
        //                kolon6 = f.aciklama.Substring(0, 26).PadRight(26);
        //            else
        //                kolon6 = f.aciklama.PadRight(26);
        //        }
        //        else
        //            kolon6 = f.aciklama;

        //        //string bTur = "";
        //        //if (f.belgeTur == ENUMBelgeTuru.ODEMEEMRI)
        //        //    bTur = "ÖDE";
        //        //else
        //        //    bTur = "MİF";

        //        if (tur == 1)
        //        {
        //            kolon0 = f.YEVYevTarih.Gun.ToString("00") + f.YEVYevTarih.Ay.ToString("00") + f.YEVYevTarih.Yil.ToString().Substring(2, 2);
        //            kolon1 = f.YEVYevNo.ToString().PadRight(10);

        //            if (sayistay > 0)
        //            {
        //                kolon0 = f.YEVYevTarih.Gun.ToString("00") + "." + f.YEVYevTarih.Ay.ToString("00") + "." + f.YEVYevTarih.Yil.ToString();
        //                kolon9 = f.formDurumTarih.Gun.ToString("00") + "." + f.formDurumTarih.Ay.ToString("00") + "." + f.formDurumTarih.Yil.ToString("0000") + " " + f.formDurumTarih.Saat.ToString("00") + ":" + f.formDurumTarih.Dakika.ToString("00") + ":" + f.formDurumTarih.Saniye.ToString("00");
        //            }

        //        }
        //        else
        //        {
        //            kolon0 = f.islemTarih.Gun.ToString("00") + f.islemTarih.Ay.ToString("00") + f.islemTarih.Yil.ToString().Substring(2, 2);
        //            kolon1 = f.belgeNo.ToString().PadRight(10);

        //        }

        //        double yBorc = 0.00;
        //        double yAlacak = 0.00;

        //        if (sayistay == 0)
        //            kolonDeger = string.Format(kolon, kolon0, kolon1, "".PadRight(22), "".PadRight(11), "".PadRight(9), "".PadRight(1), kolon6, "", "", kolon9);
        //        else
        //            kolonDeger = string.Format(kolon, kolon0.Trim(), kolon1.Trim(), "", "", "", "", kolon6.Trim(), "", "", kolon9.Trim());

        //        SatirYazDosya(rapor, kolonDeger, ref satir, ref sayfaNo, baslik, sayistay);

        //        foreach (OdemeEmriMIFDetay d in f.detay.objeler)
        //        {
        //            kolon2 = d.hesapKod.PadRight(22);
        //            if (d.hesapAd.Length > 26)
        //                kolon6 = d.hesapAd.Substring(0, 26).PadRight(26);
        //            else
        //                kolon6 = d.hesapAd.PadRight(26);

        //            if (!String.IsNullOrEmpty(d.kur))
        //            {
        //                kolon3 = d.kur.PadRight(11);
        //                kolon4 = d.fon.PadRight(9);
        //                kolon5 = d.fin.PadRight(1);
        //                //kolon6 = d.eko.PadRight(18);
        //            }
        //            else
        //            {
        //                kolon3 = "".PadRight(11);
        //                kolon4 = "".PadRight(9);
        //                kolon5 = "".PadRight(1);
        //                //kolon6 = "".PadRight(18);
        //            }

        //            tBorc += d.borc;
        //            tAlacak += d.alacak;

        //            yBorc += d.borc;
        //            yAlacak += d.alacak;

        //            if (sayistay == 0)
        //            {
        //                kolon7 = d.borc.ToString("#,###.00").PadLeft(15);
        //                kolon8 = d.alacak.ToString("#,###.00").PadLeft(15);
        //                kolonDeger = string.Format(kolon, "".PadRight(6), "".PadLeft(10), kolon2, kolon3, kolon4, kolon5, kolon6, kolon7, kolon8, kolon9);
        //            }
        //            else
        //            {
        //                kolon7 = d.borc.ToString("###.00");
        //                kolon8 = d.alacak.ToString("###.00");
        //                kolonDeger = string.Format(kolon, kolon0.Trim(), kolon1.Trim(), kolon2.Trim(), kolon3.Trim(), kolon4.Trim(), kolon5.Trim(), kolon6.Trim(), kolon7.Trim(), kolon8.Trim(), kolon9.Trim());
        //            }

        //            SatirYazDosya(rapor, kolonDeger, ref satir, ref sayfaNo, baslik, sayistay);
        //        }
        //        kolon0 = "";
        //        kolon1 = "";
        //        kolon6 = "";

        //        kolon7 = yBorc.ToString("#,###.00").PadLeft(15);
        //        kolon8 = yAlacak.ToString("#,###.00").PadLeft(15);

        //        if (sayistay == 0)
        //        {
        //            kolonDeger = string.Format(kolon, "".PadRight(6), "".PadLeft(10), "".PadLeft(22), "".PadLeft(11), "".PadLeft(9), "".PadLeft(1), "BELGE TOPLAMI : ".PadRight(26), kolon7, kolon8, kolon9);

        //            SatirYazDosya(rapor, kolonDeger, ref satir, ref sayfaNo, baslik, sayistay);
        //        }
        //    }

        //    OrtakFonksiyonlar.DosyaYaz(sonucDosyaAd, rapor.ToString(), true);
        //}

        //void SatirYazDosya(StringBuilder rapor, string kolonDeger, ref int satir, ref int sayfaNo, string baslik, int sayistay)
        //{
        //    if (satir > sayfadakiSatir)
        //    {
        //        sayfaNo++;

        //        if (sayistay == 0)
        //            BaslikYazDosya(rapor, raporAdi, kurumAdi, sayfaNo, baslik);

        //        satir = 6;
        //    }

        //    rapor.AppendLine(kolonDeger);
        //    satir++;
        //}

        //void BaslikYazDosya(StringBuilder rapor, string raporAdi, string kurumAdi, int sayfaNo, string baslik)
        //{
        //    rapor.AppendLine("".PadLeft(62 - (raporAdi.Length / 2)) + raporAdi);
        //    rapor.AppendLine("".PadLeft(62 - (kurumAdi.Length / 2)) + kurumAdi);
        //    rapor.AppendLine("Sayfa No   : " + sayfaNo);
        //    rapor.AppendLine(baslik);
        //}
    }

    public class AylikBazdaListe
    {
        OrtakClass.Tablo XLS;
        string kurumAdi = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

        /// <summary>
        /// Yevmiye raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            string basHesap = (string)raporParam["BasHesap"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int seviye = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Seviye"], 0);

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            oEmri.fisTurYap(fisTur);
            oEmri.surecYil = seviye;

            if (tur == 2 || tur == 3)
                oEmri.formDurum = 0;
            else
                oEmri.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;

            OdemeEmriMIFDetay det = new OdemeEmriMIFDetay(yil);
            det.hesapKod = basHesap.Replace(".", "");
            oEmri.detay.Ekle(det);

            ObjectArray mb = s.AylikDagilimHesapKod((TNS.KYM.Kullanici)kullanan, oEmri);

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("Baslik", yil + " YILI AYLIK BAZDA HESAPLARIN DURUMU");

            string birimAd = "";
            if (birim.Trim() != "")
                birimAd = OrtakClass.GenelIslemler.KodAd(12, birim + "-" + saymanlik.ToString(), false);

            string saymanlikAd = (string)raporParam["SaymanlikAd"] + "";
            if (saymanlikAd == "")
                saymanlikAd = "Kurum Geneli";
            else
                saymanlikAd = saymanlik + " - " + saymanlikAd;
            XLS.HucreAdBulYaz("muhasebe", saymanlikAd);

            if (birimAd == "") birimAd = "Kurum Geneli";
            else
                birimAd = birim + " - " + birimAd;
            XLS.HucreAdBulYaz("birim", birimAd);

            int baslaSatir = 0, baslaSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);

            Hashtable tablo = new Hashtable();
            List<HesapPlaniSatir> hesapKodlar = new List<HesapPlaniSatir>();
            foreach (AylikDagilim form in mb.objeler)
            {
                bool bulundu = false;
                foreach (HesapPlaniSatir h in hesapKodlar)
                {
                    if (h.hesapKod == form.hesapKod)
                    {
                        bulundu = true;
                        if (form.yil > h.seviye)
                        {
                            h.seviye = form.yil;
                            h.hesapKod = form.hesapKod;
                            h.aciklama = form.hesapAd;
                        }
                    }
                }
                if (!bulundu)
                {
                    HesapPlaniSatir hs = new HesapPlaniSatir();
                    hs.seviye = form.yil;
                    hs.hesapKod = form.hesapKod;
                    hs.aciklama = form.hesapAd;
                    hesapKodlar.Add(hs);
                }
                tablo[form.hesapKod + "*" + form.ay] = form.tutar;// Math.Abs hatalı sonuc veriyor
            }

            int satir = baslaSatir + 1;
            double genelToplam = 0.0;
            double[] toplamAlt = new double[15];

            foreach (HesapPlaniSatir form in hesapKodlar)
            {
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 15, satir, 0);

                int sut = 0;
                XLS.HucreDegerYaz(satir, sut++, form.hesapKod);
                XLS.HucreDegerYaz(satir, sut++, form.aciklama);
                double toplamYan = 0.0;

                for (int i = 0; i < 12; i++)
                {
                    XLS.HucreDegerYaz(satir, (i + 1) + 1, 0);//Eğer değer yoksa 0 yazsın
                    if (tablo[form.hesapKod + "*" + (i + 1)] != null)
                    {
                        XLS.HucreDegerYaz(satir, (i + 1) + 1, (double)tablo[form.hesapKod + "*" + (i + 1)]);
                        toplamYan += (double)tablo[form.hesapKod + "*" + (i + 1)];
                        toplamAlt[i] += (double)tablo[form.hesapKod + "*" + (i + 1)];
                    }
                }
                XLS.HucreDegerYaz(satir, 12 + 2, toplamYan);
                genelToplam += toplamYan;
                satir++;
            }

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 15, satir, 0);
            XLS.HucreDegerYaz(satir, 1, "TOPLAM");

            for (int i = 0; i < 12; i++)
            {
                XLS.HucreDegerYaz(satir, i + 2, toplamAlt[i]);

            }
            XLS.HucreDegerYaz(satir, 12 + 2, genelToplam);
            XLS.KoyuYap(satir, 0, satir, 15, true);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    public class YillikBazdaListe
    {
        OrtakClass.Tablo XLS;
        string kurumAdi = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

        /// <summary>
        /// Yevmiye raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            string basHesap = (string)raporParam["BasHesap"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int seviye = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Seviye"], 0);

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            oEmri.fisTurYap(fisTur);
            oEmri.surecYil = seviye;

            if (tur == 2 || tur == 3)
                oEmri.formDurum = 0;
            else
                oEmri.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;

            OdemeEmriMIFDetay det = new OdemeEmriMIFDetay(yil);
            det.hesapKod = basHesap.Replace(".", "");
            oEmri.detay.Ekle(det);

            ObjectArray mb = s.YillikDagilimHesapKod((TNS.KYM.Kullanici)kullanan, oEmri);

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("Baslik", "SON 5 YILLIK BAZDA HESAPLARIN DURUMU");

            string birimAd = "";
            if (birim.Trim() != "")
                birimAd = OrtakClass.GenelIslemler.KodAd(12, birim + "-" + saymanlik.ToString(), false);

            string saymanlikAd = (string)raporParam["SaymanlikAd"] + "";
            if (saymanlikAd == "")
                saymanlikAd = "Kurum Geneli";
            else
                saymanlikAd = saymanlik + " - " + saymanlikAd;
            XLS.HucreAdBulYaz("muhasebe", saymanlikAd);

            if (birimAd == "") birimAd = "Kurum Geneli";
            else
                birimAd = birim + " - " + birimAd;
            XLS.HucreAdBulYaz("birim", birimAd);

            int baslaSatir = 0, baslaSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);

            Hashtable tablo = new Hashtable();
            List<HesapPlaniSatir> hesapKodlar = new List<HesapPlaniSatir>();
            foreach (AylikDagilim form in mb.objeler)
            {
                bool bulundu = false;
                foreach (HesapPlaniSatir h in hesapKodlar)
                {
                    if (h.hesapKod == form.hesapKod)
                    {
                        bulundu = true;
                        if (form.ay > h.seviye)
                        {
                            h.seviye = form.ay;
                            h.hesapKod = form.hesapKod;
                            h.aciklama = form.hesapAd;
                        }
                    }
                }
                if (!bulundu)
                {
                    HesapPlaniSatir hs = new HesapPlaniSatir();
                    hs.seviye = form.ay;
                    hs.hesapKod = form.hesapKod;
                    hs.aciklama = form.hesapAd;
                    hesapKodlar.Add(hs);
                }
                tablo[form.hesapKod + "*" + (yil - form.ay)] = form.tutar;// Math.Abs hatalı sonuc veriyor
            }

            for (int i = 0; i < 5; i++)
            {
                XLS.HucreDegerYaz(baslaSatir - 1, i + 2, yil - i);
            }

            int satir = baslaSatir + 1;
            double genelToplam = 0.0;
            double[] toplamAlt = new double[6];

            foreach (HesapPlaniSatir form in hesapKodlar)
            {
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 6, satir, 0);

                int sut = 0;
                XLS.HucreDegerYaz(satir, sut++, form.hesapKod);
                XLS.HucreDegerYaz(satir, sut++, form.aciklama);
                double toplamYan = 0.0;

                for (int i = 0; i < 5; i++)
                {
                    XLS.HucreDegerYaz(satir, (i) + 2, 0);//Eğer değer yoksa 0 yazsın
                    if (tablo[form.hesapKod + "*" + (i)] != null)
                    {
                        XLS.HucreDegerYaz(satir, (i) + 2, (double)tablo[form.hesapKod + "*" + (i)]);
                        toplamYan += (double)tablo[form.hesapKod + "*" + (i)];
                        toplamAlt[i] += (double)tablo[form.hesapKod + "*" + (i)];
                    }
                }
                XLS.HucreDegerYaz(satir, 5 + 2, toplamYan);
                genelToplam += toplamYan;
                satir++;
            }

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 15, satir, 0);
            XLS.HucreDegerYaz(satir, 1, "TOPLAM");

            for (int i = 0; i < 5; i++)
            {
                XLS.HucreDegerYaz(satir, i + 2, toplamAlt[i]);

            }
            XLS.HucreDegerYaz(satir, 5 + 2, genelToplam);
            XLS.KoyuYap(satir, 0, satir, 6, true);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    public class YillikAylikBazdaListe
    {
        OrtakClass.Tablo XLS;
        string kurumAdi = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

        /// <summary>
        /// Yevmiye raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            string basHesap = (string)raporParam["BasHesap"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int seviye = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Seviye"], 0);

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            oEmri.fisTurYap(fisTur);
            oEmri.surecYil = seviye;

            if (tur == 2 || tur == 3)
                oEmri.formDurum = 0;
            else
                oEmri.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;

            OdemeEmriMIFDetay det = new OdemeEmriMIFDetay(yil);
            det.hesapKod = basHesap.Replace(".", "");
            oEmri.detay.Ekle(det);

            ObjectArray mb = s.YillikAylikDagilimHesapKod((TNS.KYM.Kullanici)kullanan, oEmri);

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("Baslik", (yil - 2) + " / " + yil + " YILLARI KARŞILAŞTIRMA RAPORU");

            string birimAd = "";
            if (birim.Trim() != "")
                birimAd = OrtakClass.GenelIslemler.KodAd(12, birim + "-" + saymanlik.ToString(), false);

            string saymanlikAd = (string)raporParam["SaymanlikAd"] + "";
            if (saymanlikAd == "")
                saymanlikAd = "Kurum Geneli";
            else
                saymanlikAd = saymanlik + " - " + saymanlikAd;
            XLS.HucreAdBulYaz("muhasebe", saymanlikAd);

            if (birimAd == "") birimAd = "Kurum Geneli";
            else
                birimAd = birim + " - " + birimAd;
            XLS.HucreAdBulYaz("birim", birimAd);

            int baslaSatir = 0, baslaSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);

            Hashtable tablo = new Hashtable();
            List<HesapPlaniSatir> hesapKodlar = new List<HesapPlaniSatir>();
            foreach (AylikDagilim form in mb.objeler)
            {
                bool bulundu = false;
                foreach (HesapPlaniSatir h in hesapKodlar)
                {
                    if (h.hesapKod == form.hesapKod)
                    {
                        bulundu = true;
                        if (form.yil > h.seviye && !string.IsNullOrEmpty(form.hesapAd))
                        {
                            h.seviye = form.yil;
                            h.hesapKod = form.hesapKod;
                            h.aciklama = form.hesapAd;
                        }
                    }
                }
                if (!bulundu)
                {
                    HesapPlaniSatir hs = new HesapPlaniSatir();
                    hs.seviye = form.yil;
                    hs.hesapKod = form.hesapKod;
                    hs.aciklama = form.hesapAd;
                    hesapKodlar.Add(hs);
                }
                tablo[form.hesapKod + "*" + form.yil + "*" + form.ay] = form.tutar;// Math.Abs hatalı sonuc veriyor
            }

            int satir = baslaSatir + 1;
            double[,] toplamAlt = new double[15, 3];
            double[] genelToplam = new double[3];

            foreach (HesapPlaniSatir form in hesapKodlar)
            {
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 52, satir, 0);

                int sut = 0;
                XLS.HucreDegerYaz(satir, sut++, form.hesapKod);
                XLS.HucreDegerYaz(satir, sut++, form.aciklama);
                double[] toplamYan = new double[3];

                for (int i = 0; i < 12; i++)
                {
                    for (int kyil = oEmri.yil - 2; kyil <= oEmri.yil; kyil++)
                    {

                        int yyil = (kyil - oEmri.yil) + 1;
                        yyil = (i + 1) * 4 + yyil;
                        yyil--;

                        //Kolon Başlığı
                        XLS.HucreDegerYaz(baslaSatir - 1, yyil, kyil);

                        XLS.HucreDegerYaz(satir, yyil, 0);//Eğer değer yoksa 0 yazsın
                        if (tablo[form.hesapKod + "*" + kyil + "*" + (i + 1)] != null)
                        {
                            XLS.HucreDegerYaz(satir, yyil, (double)tablo[form.hesapKod + "*" + kyil + "*" + (i + 1)]);
                            toplamYan[oEmri.yil - kyil] += (double)tablo[form.hesapKod + "*" + kyil + "*" + (i + 1)];
                            toplamAlt[i, oEmri.yil - kyil] += (double)tablo[form.hesapKod + "*" + kyil + "*" + (i + 1)];
                        }
                    }
                }

                //Kolon Başlığı
                XLS.HucreDegerYaz(baslaSatir - 1, 50, oEmri.yil - 2);
                XLS.HucreDegerYaz(baslaSatir - 1, 51, oEmri.yil - 1);
                XLS.HucreDegerYaz(baslaSatir - 1, 52, oEmri.yil);

                XLS.HucreDegerYaz(satir, 50, toplamYan[2]);
                XLS.HucreDegerYaz(satir, 51, toplamYan[1]);
                XLS.HucreDegerYaz(satir, 52, toplamYan[0]);
                genelToplam[2] += toplamYan[2];
                genelToplam[1] += toplamYan[1];
                genelToplam[0] += toplamYan[0];
                satir++;
            }

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 52, satir, 0);
            XLS.HucreDegerYaz(satir, 1, "TOPLAM");

            for (int i = 0; i < 12; i++)
            {
                for (int kyil = oEmri.yil - 2; kyil <= oEmri.yil; kyil++)
                {
                    int yyil = (kyil - oEmri.yil) + 1;
                    yyil = (i + 1) * 4 + yyil;
                    yyil--;

                    XLS.HucreDegerYaz(satir, yyil, toplamAlt[i, oEmri.yil - kyil]);
                }
            }
            XLS.HucreDegerYaz(satir, 50, genelToplam[2]);
            XLS.HucreDegerYaz(satir, 51, genelToplam[1]);
            XLS.HucreDegerYaz(satir, 52, genelToplam[0]);
            XLS.KoyuYap(satir, 0, satir, 52, true);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    public class YillikAylikBazdaListe2Yillik
    {
        OrtakClass.Tablo XLS;
        string kurumAdi = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

        /// <summary>
        /// Yevmiye raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            string basHesap = (string)raporParam["BasHesap"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int seviye = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Seviye"], 0);

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            oEmri.fisTurYap(fisTur);
            oEmri.surecYil = seviye;

            if (tur == 2 || tur == 3)
                oEmri.formDurum = 0;
            else
                oEmri.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;

            OdemeEmriMIFDetay det = new OdemeEmriMIFDetay(yil);
            det.hesapKod = basHesap.Replace(".", "");
            oEmri.detay.Ekle(det);

            ObjectArray mb = s.YillikAylikDagilimHesapKod((TNS.KYM.Kullanici)kullanan, oEmri);

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("Baslik", (yil - 1) + " / " + yil + " YILLARI KARŞILAŞTIRMA RAPORU");

            string birimAd = "";
            if (birim.Trim() != "")
                birimAd = OrtakClass.GenelIslemler.KodAd(12, birim + "-" + saymanlik.ToString(), false);

            string saymanlikAd = (string)raporParam["SaymanlikAd"] + "";
            if (saymanlikAd == "")
                saymanlikAd = "Kurum Geneli";
            else
                saymanlikAd = saymanlik + " - " + saymanlikAd;
            XLS.HucreAdBulYaz("muhasebe", saymanlikAd);

            if (birimAd == "") birimAd = "Kurum Geneli";
            else
                birimAd = birim + " - " + birimAd;
            XLS.HucreAdBulYaz("birim", birimAd);

            int baslaSatir = 0, baslaSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);

            Hashtable tablo = new Hashtable();
            List<HesapPlaniSatir> hesapKodlar = new List<HesapPlaniSatir>();
            foreach (AylikDagilim form in mb.objeler)
            {
                bool bulundu = false;
                foreach (HesapPlaniSatir h in hesapKodlar)
                {
                    if (h.hesapKod == form.hesapKod)
                    {
                        bulundu = true;
                        if (form.yil > h.seviye && !string.IsNullOrEmpty(form.hesapAd))
                        {
                            h.seviye = form.yil;
                            h.hesapKod = form.hesapKod;
                            h.aciklama = form.hesapAd;
                        }
                    }
                }
                if (!bulundu)
                {
                    HesapPlaniSatir hs = new HesapPlaniSatir();
                    hs.seviye = form.yil;
                    hs.hesapKod = form.hesapKod;
                    hs.aciklama = form.hesapAd;
                    hesapKodlar.Add(hs);
                }
                tablo[form.hesapKod + "*" + form.yil + "*" + form.ay] = form.tutar;// Math.Abs hatalı sonuc veriyor
            }

            int satir = baslaSatir + 1;
            double[,] toplamAlt = new double[15, 2];
            double[] genelToplam = new double[2];

            foreach (HesapPlaniSatir form in hesapKodlar)
            {
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 40, satir, 0);

                int sut = 0;
                XLS.HucreDegerYaz(satir, sut++, form.hesapKod);
                XLS.HucreDegerYaz(satir, sut++, form.aciklama);
                double[] toplamYan = new double[2];

                for (int i = 0; i < 12; i++)
                {
                    for (int kyil = oEmri.yil - 1; kyil <= oEmri.yil; kyil++)
                    {

                        int yyil = (kyil - oEmri.yil) + 1;
                        yyil = (i + 1) * 3 + yyil;
                        yyil--;

                        //Kolon Başlığı
                        XLS.HucreDegerYaz(baslaSatir - 1, yyil, kyil);

                        XLS.HucreDegerYaz(satir, yyil, 0);//Eğer değer yoksa 0 yazsın
                        if (tablo[form.hesapKod + "*" + kyil + "*" + (i + 1)] != null)
                        {
                            XLS.HucreDegerYaz(satir, yyil, (double)tablo[form.hesapKod + "*" + kyil + "*" + (i + 1)]);
                            toplamYan[oEmri.yil - kyil] += (double)tablo[form.hesapKod + "*" + kyil + "*" + (i + 1)];
                            toplamAlt[i, oEmri.yil - kyil] += (double)tablo[form.hesapKod + "*" + kyil + "*" + (i + 1)];
                        }
                    }
                }

                //Kolon Başlığı
                XLS.HucreDegerYaz(baslaSatir - 1, 38, oEmri.yil - 1);
                XLS.HucreDegerYaz(baslaSatir - 1, 39, oEmri.yil);

                XLS.HucreDegerYaz(satir, 38, toplamYan[1]);
                XLS.HucreDegerYaz(satir, 39, toplamYan[0]);
                genelToplam[1] += toplamYan[1];
                genelToplam[0] += toplamYan[0];
                satir++;
            }

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 40, satir, 0);
            XLS.HucreDegerYaz(satir, 1, "TOPLAM");

            for (int i = 0; i < 12; i++)
            {
                for (int kyil = oEmri.yil - 1; kyil <= oEmri.yil; kyil++)
                {
                    int yyil = (kyil - oEmri.yil) + 1;
                    yyil = (i + 1) * 3 + yyil;
                    yyil--;

                    XLS.HucreDegerYaz(satir, yyil, toplamAlt[i, oEmri.yil - kyil]);
                }
            }
            XLS.HucreDegerYaz(satir, 38, genelToplam[1]);
            XLS.HucreDegerYaz(satir, 39, genelToplam[0]);
            XLS.KoyuYap(satir, 0, satir, 40, true);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    public class YillikAylikTarihselBazdaListe
    {
        OrtakClass.Tablo XLS;
        string kurumAdi = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

        /// <summary>
        /// Yevmiye raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            string basHesap = (string)raporParam["BasHesap"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int seviye = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Seviye"], 0);
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            oEmri.fisTurYap(fisTur);
            oEmri.surecYil = seviye;

            if (tur == 2 || tur == 3)
            {
                oEmri.formDurum = 0;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else
            {
                oEmri.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
                oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }

            OdemeEmriMIFDetay det = new OdemeEmriMIFDetay(yil);
            det.hesapKod = basHesap.Replace(".", "");
            oEmri.detay.Ekle(det);

            ObjectArray mb = s.YillikAylikTarihselDagilimHesapKod((TNS.KYM.Kullanici)kullanan, oEmri);

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("Baslik", "TARİHSEL KARŞILAŞTIRMA RAPORU");

            string birimAd = "";
            if (birim.Trim() != "")
                birimAd = OrtakClass.GenelIslemler.KodAd(12, birim + "-" + saymanlik.ToString(), false);

            string saymanlikAd = (string)raporParam["SaymanlikAd"] + "";
            if (saymanlikAd == "")
                saymanlikAd = "Kurum Geneli";
            else
                saymanlikAd = saymanlik + " - " + saymanlikAd;
            XLS.HucreAdBulYaz("muhasebe", saymanlikAd);

            if (birimAd == "") birimAd = "Kurum Geneli";
            else
                birimAd = birim + " - " + birimAd;
            XLS.HucreAdBulYaz("birim", birimAd);

            TNSDateTime d1 = new TNSDateTime(tarih1.ToString());
            TNSDateTime d2 = new TNSDateTime(tarih2.ToString());
            XLS.HucreAdBulYaz("tarih2", d1.ToString() + "-" + d2.ToString());
            d1.AddYears(-1);
            d2.AddYears(-1);
            XLS.HucreAdBulYaz("tarih1", d1.ToString() + "-" + d2.ToString());

            int baslaSatir = 0, baslaSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);

            Hashtable tablo = new Hashtable();
            List<HesapPlaniSatir> hesapKodlar = new List<HesapPlaniSatir>();
            foreach (AylikDagilim form in mb.objeler)
            {
                bool bulundu = false;
                foreach (HesapPlaniSatir h in hesapKodlar)
                {
                    if (h.hesapKod == form.hesapKod)
                    {
                        bulundu = true;
                        if (form.yil > h.seviye)
                        {
                            h.seviye = form.yil;
                            h.hesapKod = form.hesapKod;
                            h.aciklama = form.hesapAd;
                        }
                    }
                }
                if (!bulundu)
                {
                    HesapPlaniSatir hs = new HesapPlaniSatir();
                    hs.seviye = form.yil;
                    hs.hesapKod = form.hesapKod;
                    hs.aciklama = form.hesapAd;
                    hesapKodlar.Add(hs);
                }
                tablo[form.hesapKod + "*" + form.yil] = form.tutar;// Math.Abs hatalı sonuc veriyor
            }

            int satir = baslaSatir + 1;
            double[] toplamAlt = new double[15];
            double[] genelToplam = new double[2];

            foreach (HesapPlaniSatir form in hesapKodlar)
            {
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 40, satir, 0);

                int sut = 0;
                XLS.HucreDegerYaz(satir, sut++, form.hesapKod);
                XLS.HucreDegerYaz(satir, sut++, form.aciklama);
                double[] toplamYan = new double[2];

                for (int kyil = oEmri.yil - 1; kyil <= oEmri.yil; kyil++)
                {
                    int yyil = (kyil - oEmri.yil) + 3;

                    XLS.HucreDegerYaz(satir, yyil, 0);//Eğer değer yoksa 0 yazsın
                    if (tablo[form.hesapKod + "*" + kyil] != null)
                    {
                        XLS.HucreDegerYaz(satir, yyil, (double)tablo[form.hesapKod + "*" + kyil]);
                        toplamYan[oEmri.yil - kyil] += (double)tablo[form.hesapKod + "*" + kyil];
                        toplamAlt[oEmri.yil - kyil] += (double)tablo[form.hesapKod + "*" + kyil];
                    }
                }

                //Kolon Başlığı
                XLS.HucreDegerYaz(baslaSatir - 1, 38, oEmri.yil - 1);
                XLS.HucreDegerYaz(baslaSatir - 1, 39, oEmri.yil);

                XLS.HucreDegerYaz(satir, 38, toplamYan[1]);
                XLS.HucreDegerYaz(satir, 39, toplamYan[0]);
                genelToplam[1] += toplamYan[1];
                genelToplam[0] += toplamYan[0];
                satir++;
            }

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 40, satir, 0);
            XLS.HucreDegerYaz(satir, 1, "TOPLAM");

            for (int kyil = oEmri.yil - 1; kyil <= oEmri.yil; kyil++)
            {
                int yyil = (kyil - oEmri.yil) + 3;

                XLS.HucreDegerYaz(satir, yyil, toplamAlt[oEmri.yil - kyil]);
            }

            XLS.HucreDegerYaz(satir, 38, genelToplam[1]);
            XLS.HucreDegerYaz(satir, 39, genelToplam[0]);
            XLS.KoyuYap(satir, 0, satir, 40, true);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    public class BirimBazdaListe
    {
        OrtakClass.Tablo XLS;
        string kurumAdi = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

        /// <summary>
        /// Yevmiye raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            string basHesap = (string)raporParam["BasHesap"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int seviye = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Seviye"], 0);
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            oEmri.fisTurYap(fisTur);
            oEmri.surecYil = seviye;

            if (tur == 2 || tur == 3)
            {
                oEmri.formDurum = 0;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else
            {
                oEmri.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
                oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }

            OdemeEmriMIFDetay det = new OdemeEmriMIFDetay(yil);
            det.hesapKod = basHesap.Replace(".", "");
            oEmri.detay.Ekle(det);

            ObjectArray mb = s.BirimDagilimHesapKod((TNS.KYM.Kullanici)kullanan, oEmri);

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("Baslik", "BİRİM BAZDA HESAPLARIN DURUMU");

            string birimAd = "";
            if (birim.Trim() != "")
                birimAd = OrtakClass.GenelIslemler.KodAd(12, birim + "-" + saymanlik.ToString(), false);

            string saymanlikAd = (string)raporParam["SaymanlikAd"] + "";
            if (saymanlikAd == "")
                saymanlikAd = "Kurum Geneli";
            else
                saymanlikAd = saymanlik + " - " + saymanlikAd;
            XLS.HucreAdBulYaz("muhasebe", saymanlikAd);
            XLS.HucreAdBulYaz("yil", yil);

            int baslaSatir = 0, baslaSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);

            Hashtable tablo = new Hashtable();
            List<HesapPlaniSatir> hesapKodlar = new List<HesapPlaniSatir>();
            string birimler = "";
            foreach (AylikDagilim form in mb.objeler)
            {
                bool bulundu = false;
                foreach (HesapPlaniSatir h in hesapKodlar)
                {
                    if (h.hesapKod == form.hesapKod)
                        bulundu = true;
                }
                if (!bulundu)
                {
                    HesapPlaniSatir hs = new HesapPlaniSatir();
                    hs.hesapKod = form.hesapKod;
                    hs.aciklama = form.hesapAd;
                    hesapKodlar.Add(hs);
                }
                if (birimler.IndexOf(form.birim) < 0)
                {
                    if (birimler != "") birimler += ";";
                    birimler += form.birim;
                }

                tablo[form.hesapKod + "*" + form.birim] = form.tutar;// Math.Abs olursa hatalı sonuçlar oluyor
            }
            string[] birimListe = birimler.Split(';');

            XLS.SutunAc(3, birimListe.Length);
            XLS.SutunGenislikAyarla(3, birimListe.Length + 2, 11);
            for (int i = 0; i < birimListe.Length; i++)
            {
                XLS.HucreDegerYaz(baslaSatir - 1, i + 2, birimListe[i]);
            }
            XLS.HucreDegerYaz(baslaSatir - 1, birimListe.Length + 2, "TOPLAM");

            int satir = baslaSatir + 1;
            double genelToplam = 0.0;
            double[] toplamAlt = new double[60];

            foreach (HesapPlaniSatir form in hesapKodlar)
            {
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 6, satir, 0);

                int sut = 0;
                XLS.HucreDegerYaz(satir, sut++, form.hesapKod);
                XLS.HucreDegerYaz(satir, sut++, form.aciklama);
                double toplamYan = 0.0;

                for (int i = 0; i < birimListe.Length; i++)
                {
                    XLS.HucreDegerYaz(satir, (i) + 2, 0);//Eğer değer yoksa 0 yazsın
                    if (tablo[form.hesapKod + "*" + birimListe[i]] != null)
                    {
                        XLS.HucreDegerYaz(satir, (i) + 2, (double)tablo[form.hesapKod + "*" + birimListe[i]]);
                        toplamYan += (double)tablo[form.hesapKod + "*" + birimListe[i]];
                        toplamAlt[i] += (double)tablo[form.hesapKod + "*" + birimListe[i]];
                    }
                }
                XLS.HucreDegerYaz(satir, birimListe.Length + 2, toplamYan);
                genelToplam += toplamYan;
                satir++;
            }

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 15, satir, 0);
            XLS.HucreDegerYaz(satir, 1, "TOPLAM");

            for (int i = 0; i < birimListe.Length; i++)
            {
                XLS.HucreDegerYaz(satir, i + 2, toplamAlt[i]);

            }
            XLS.HucreDegerYaz(satir, birimListe.Length + 2, genelToplam);
            XLS.KoyuYap(satir, 0, satir, birimListe.Length + 2, true);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;

        }
    }

    public class BirimGelirGider
    {
        OrtakClass.Tablo XLS;
        string kurumAdi = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

        /// <summary>
        /// Yevmiye raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            string basHesap = (string)raporParam["BasHesap"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int seviye = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Seviye"], 0);
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            oEmri.fisTurYap(fisTur);
            oEmri.surecYil = seviye;

            if (tur == 2 || tur == 3)
            {
                oEmri.formDurum = 0;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else
            {
                oEmri.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
                oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }

            OdemeEmriMIFDetay det = new OdemeEmriMIFDetay(yil);
            det.hesapKod = basHesap.Replace(".", "");
            oEmri.detay.Ekle(det);

            ObjectArray mb = s.BirimGelirGider((TNS.KYM.Kullanici)kullanan, oEmri);

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("Baslik", "BİRİM BAZINDA GELİR/GİDER DURUMU");

            string birimAd = "";
            if (birim.Trim() != "")
                birimAd = OrtakClass.GenelIslemler.KodAd(12, birim + "-" + saymanlik.ToString(), false);

            string saymanlikAd = (string)raporParam["SaymanlikAd"] + "";
            if (saymanlikAd == "")
                saymanlikAd = "Kurum Geneli";
            else
                saymanlikAd = saymanlik + " - " + saymanlikAd;
            XLS.HucreAdBulYaz("muhasebe", saymanlikAd);
            XLS.HucreAdBulYaz("yil", yil);

            int baslaSatir = 0, baslaSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);

            Hashtable tablo = new Hashtable();
            List<HesapPlaniSatir> hesapKodlar = new List<HesapPlaniSatir>();
            string birimler = "";
            foreach (OdenekDurum form in mb.objeler)
            {
                bool bulundu = false;
                foreach (HesapPlaniSatir h in hesapKodlar)
                {
                    if (h.hesapKod == form.birim)
                        bulundu = true;
                }
                if (!bulundu)
                {
                    HesapPlaniSatir hs = new HesapPlaniSatir();
                    hs.hesapKod = form.birim;
                    hs.aciklama = form.birimAd;
                    hesapKodlar.Add(hs);
                }
                if (birimler.IndexOf(form.birim) < 0)
                {
                    if (birimler != "") birimler += ";";
                    birimler += form.birim;
                }

                tablo[form.birim + "*" + form.muhasebeAd] = form.toplamOdenek;// Math.Abs olursa hatalı sonuçlar oluyor
            }
            string[] birimListe = birimler.Split(';');

            int satir = baslaSatir + 1;

            double gelir = 0.0;
            double gider = 0.0;
            double gelirToplam = 0.0;
            double giderToplam = 0.0;

            foreach (HesapPlaniSatir form in hesapKodlar)
            {
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 6, satir, 0);

                try { gelir = (double)tablo[form.hesapKod + "*" + "800"]; }
                catch { gelir = 0.0; }
                gelir = gelir * -1;

                try { gider = (double)tablo[form.hesapKod + "*" + "830"]; }
                catch { gider = 0.0; }

                XLS.HucreDegerYaz(satir, 0, form.hesapKod + "-" + form.aciklama);
                XLS.HucreDegerYaz(satir, 1, gelir);
                XLS.HucreDegerYaz(satir, 2, gider);
                XLS.HucreDegerYaz(satir, 3, gelir - gider);

                gelirToplam += gelir;
                giderToplam += gider;

                satir++;
            }

            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 6, satir, 0);
            XLS.HucreDegerYaz(satir, 0, "TOPLAM");
            XLS.HucreDegerYaz(satir, 1, gelirToplam);
            XLS.HucreDegerYaz(satir, 2, giderToplam);
            XLS.HucreDegerYaz(satir, 3, gelirToplam - giderToplam);
            XLS.KoyuYap(satir, 0, satir, 15, true);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;

        }
    }

    public class GelirGiderListe
    {
        OrtakClass.Tablo XLS;
        string kurumAdi = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

        /// <summary>
        /// Yevmiye raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            string basHesap = (string)raporParam["BasHesap"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int seviye = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Seviye"], 0);
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            oEmri.fisTurYap(fisTur);
            oEmri.surecYil = seviye;

            if (tur == 2 || tur == 3)
            {
                oEmri.formDurum = 0;
                oEmri.sorguIslemTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }
            else
            {
                oEmri.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
                oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            }

            OdemeEmriMIFDetay det = new OdemeEmriMIFDetay(yil);
            det.hesapKod = "800;830";
            oEmri.detay.Ekle(det);

            ObjectArray mb = s.GelirGiderListe((TNS.KYM.Kullanici)kullanan, oEmri);

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("Baslik", "GELİR/GİDER LİSTESİ");

            string birimAd = "";
            if (birim.Trim() != "")
                birimAd = OrtakClass.GenelIslemler.KodAd(12, birim + "-" + saymanlik.ToString(), false);

            string saymanlikAd = (string)raporParam["SaymanlikAd"] + "";
            if (saymanlikAd == "")
                saymanlikAd = "Kurum Geneli";
            else
            {
                saymanlikAd = saymanlik + " - " + saymanlikAd;
                if (birimAd != "")
                    saymanlikAd += " / " + birimAd;
            }

            XLS.HucreAdBulYaz("muhasebe", saymanlikAd);
            XLS.HucreAdBulYaz("yil", yil);

            int baslaSatir = 0, baslaSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);

            int satir = 0;
            int sutun = 0;
            int satirGelir = baslaSatir;
            int satirGider = baslaSatir;

            //XLS.SatirAc(baslaSatir+1, 5000);

            double gelirToplam = 0.0;
            double giderToplam = 0.0;

            foreach (OdenekDurum form in mb.objeler)
            {
                if (form.muhasebeAd.StartsWith("830"))
                {
                    sutun = 0;
                    satirGider++;
                    satir = satirGider;
                    giderToplam += form.toplamOdenek;
                }
                else
                {
                    sutun = 4;
                    satirGelir++;
                    satir = satirGelir;
                    gelirToplam += form.toplamOdenek;
                }

                XLS.HucreKopyala(baslaSatir, sutun, baslaSatir, sutun + 3, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, form.muhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 1, form.birimAd);
                XLS.HucreDegerYaz(satir, sutun + 2, form.toplamOdenek);
            }

            satirGider++;
            satirGelir++;

            XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 3, satirGider, 0);
            XLS.HucreDegerYaz(satirGider, 1, "TOPLAM");
            XLS.HucreDegerYaz(satirGider, 2, giderToplam);
            XLS.KoyuYap(satirGider, 0, satirGider, 3, true);

            XLS.HucreKopyala(baslaSatir, 4, baslaSatir, 6, satirGelir, 4);
            XLS.HucreDegerYaz(satirGelir, 5, "TOPLAM");
            XLS.HucreDegerYaz(satirGelir, 6, gelirToplam);
            XLS.KoyuYap(satirGelir, 4, satirGelir, 6, true);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;

        }
    }

    public class DevredenHesaplar
    {
        OrtakClass.Tablo XLS;
        string kurumAdi = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

        /// <summary>
        /// Yevmiye raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int listeTur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["ListeTur"], 0);
            int sayistay = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Sayistay"], 0);

            string basHesap = (string)raporParam["BasHesap"];
            string bitHesap = (string)raporParam["BitHesap"];
            int RaporSira = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["RaporSira"], 0);

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            oEmri.sorguTarihSiraliMi = true;
            if (RaporSira == 1)
                oEmri.sorguTarihSiraliMi = false;

            oEmri.formDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
            oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();

            oEmri.fisTurYap(fisTur);

            OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(oEmri.yil);
            oEmriDetay.kur = "";
            oEmriDetay.fon = "";
            oEmriDetay.fin = "";
            oEmriDetay.eko = "";
            oEmriDetay.peb = "";
            oEmriDetay.hesapKod = basHesap.Trim().Replace(".", "").Trim();
            oEmri.detay.objeler.Add(oEmriDetay);

            //OrtakFonksiyonlar.HataStrYaz("Rapor Listele Başla" + DateTime.Now.ToString());
            ObjectArray mb = s.OdemeEmriMIFListele((TNS.KYM.Kullanici)kullanan, oEmri, true, true);
            //OrtakFonksiyonlar.HataStrYaz("Rapor Listele Bitti" + DateTime.Now.ToString());

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            BaslikYaz(XLS, yil, saymanlik, "", kurum, kurumAd, birim, "", tarih1, tarih2, listeTur);

            int baslaSatir = 0, baslaSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
            int satir = baslaSatir + 1;

            Hashtable htKapatmaBelge = new Hashtable();
            Hashtable htKapatmaBelgeDurum = new Hashtable();

            if (mb.sonuc.islemSonuc)
            {
                if (mb.objeler.Count > 0)
                {
                    //Raporlanacak hesapkodlarını bulmak için
                    //*********************************************************
                    string hesapKodlar = "";
                    foreach (OdemeEmriMIFForm oem in mb.objeler)
                    {
                        ObjectArray yeniDetay = new ObjectArray();
                        foreach (OdemeEmriMIFDetay detay in oem.detay.objeler)
                        {
                            if (hesapKodlar != "") hesapKodlar += ";";
                            if (hesapKodlar.IndexOf(detay.hesapKod + ";") > -1) continue;
                            hesapKodlar += detay.hesapKod;
                        }
                    }
                    ObjectArray hesapKodlari = HesapKoduBilgisiAl(s, yil, hesapKodlar);
                    //*********************************************************

                    //İlgili belge no ile yapılan işlemleri ilgili belge no içinde birleştir
                    //*********************************************************
                    //foreach (OdemeEmriMIFForm oem in mb.objeler)
                    //{
                    //    if (string.IsNullOrEmpty(oem.ilgiliBelgeNo)) continue;

                    //    bool eklendi = false;
                    //    foreach (OdemeEmriMIFForm oem2 in mb.objeler)
                    //    {
                    //        if (oem2.belgeNo == oem.ilgiliBelgeNo && oem2.belgeNo != oem.belgeNo)//belgeNoları farklı ama belgeNo ile ilgiliNo aynı olacak
                    //        {
                    //            foreach (OdemeEmriMIFDetay detay in oem.detay.objeler)
                    //            {
                    //                eklendi = true;
                    //                oem2.detay.objeler.Add(detay);
                    //            }
                    //            if (eklendi) break;
                    //        }
                    //    }
                    //    if (eklendi)
                    //        oem.detay.objeler = new TNSCollection();
                    //}
                    //*********************************************************

                    //Bir yevmiyede, aynı hesapkod birden fazla satırda geçiyor ise tek satırda toplama işlemi yapılıyor
                    //Bu bölüm EGODan gelen talep ve Özcan bey ile konuşma üzerine kapatıldı. Her satır ayrı ayrı gelecek. 27.01.2016 Melih
                    //*********************************************************
                    foreach (OdemeEmriMIFForm oem in mb.objeler)
                    {
                        ObjectArray yeniDetay = new ObjectArray();
                        foreach (OdemeEmriMIFDetay detay in oem.detay.objeler)
                        {
                            OdemeEmriMIFDetay icDetay = new OdemeEmriMIFDetay(oem.yil);
                            icDetay = detay;
                            icDetay.belgeNo = oem.belgeNo;

                            bool bulundu = false;
                            foreach (OdemeEmriMIFDetay od in yeniDetay.objeler)
                            {
                                if (od.hesapKod == detay.hesapKod && od.altHesapKod == detay.altHesapKod && od.detayBirim == detay.detayBirim)//Alt hesap kontrolü eklendi. 21.07.2015 Melih Aynı belgede farklı alt hesaplara ait bilgi olabiliyor EGO
                                {
                                    od.borc += Math.Round(detay.borc, 2);
                                    od.alacak += Math.Round(detay.alacak, 2);

                                    foreach (KapanisBilgileri kk in detay.kapatma.objeler)
                                    {
                                        bool ekle = true;
                                        foreach (KapanisBilgileri kk2 in od.kapatma.objeler)
                                        {
                                            if (kk2.belgeNo == kk.belgeNo)
                                                ekle = false;
                                        }

                                        if (ekle)
                                            od.kapatma.objeler.Add(kk);
                                    }
                                    bulundu = true;
                                }
                            }

                            if (!bulundu)
                                yeniDetay.objeler.Add(icDetay);
                        }

                        oem.detay = new ObjectArray();
                        oem.detay = yeniDetay;
                    }
                    //*********************************************************

                    ObjectArray liste = new ObjectArray();
                    foreach (OdemeEmriMIFForm oem in mb.objeler)
                    {
                        foreach (OdemeEmriMIFDetay detay in oem.detay.objeler)
                        {
                            double kapatmaBorc = 0;
                            double kapatmaAlacak = 0;
                            double kapatmaBorcAlacak = 0;

                            double kapatmaTutari = 0.0;
                            double gercekTutar = Math.Round(detay.borc, 2) - Math.Round(detay.alacak, 2);
                            double kalanTutar = 0.0;
                            if (detay.kapatma != null)
                            {
                                foreach (KapanisBilgileri kapatma in detay.kapatma.objeler)
                                {
                                    if (string.IsNullOrEmpty(kapatma.belgeNo)) continue;

                                    if (kapatma.borcAlacak == 1)
                                        kapatmaBorc += Math.Round(kapatma.kTutar, 2);
                                    else if (kapatma.borcAlacak == 2)
                                        kapatmaAlacak += Math.Round(kapatma.kTutar, 2);
                                    else
                                    {
                                        kapatmaBorcAlacak = Math.Round(kapatma.kTutar, 2);
                                        if (detay.borc > 0) kapatmaAlacak += Math.Round(kapatmaBorcAlacak, 2);
                                        if (detay.alacak > 0) kapatmaBorc += Math.Round(kapatmaBorcAlacak, 2);
                                    }

                                    int kapatmaBelgeDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
                                    if (kapatma.belgeNo != "-1")
                                    {
                                        try
                                        {
                                            kapatmaBelgeDurum = (int)htKapatmaBelgeDurum[kapatma.yil + "-" + kapatma.belgeNo];
                                        }
                                        catch { kapatmaBelgeDurum = -99; }

                                        if (kapatmaBelgeDurum == -99)
                                        {
                                            kapatmaBelgeDurum = s.BelgeDurumBilgisi(kapatma.yil, kapatma.belgeNo);
                                            htKapatmaBelgeDurum[kapatma.yil + "-" + kapatma.belgeNo] = kapatmaBelgeDurum;
                                        }
                                    }

                                    if (kapatmaBelgeDurum == (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
                                        kapatmaTutari += Math.Round(kapatma.kTutar, 2);
                                }
                            }

                            kalanTutar = Math.Round(Math.Abs(gercekTutar), 2) - Math.Round(kapatmaTutari, 2);
                            if (kalanTutar == 0) continue;


                            //Borc veya alacak çalışan hesapkodlarında mahsup belgelerini listede gösterme
                            //*********************************************************************************
                            bool islemeAlma = false;
                            foreach (HesapPlaniSatir h in hesapKodlari.objeler)
                            {
                                if (h.hesapKod.Replace(".", "") == detay.hesapKod.Replace(".", ""))
                                {
                                    if (h.bakiye == 1)
                                    {
                                        //kapatmaAlacak = kapatmaTutari;
                                        if (Math.Round(detay.borc, 2) - Math.Round(detay.alacak, 2) < 0 && detay.mahsupBelgeNo != "") islemeAlma = true;
                                    }
                                    else if (h.bakiye == 2)
                                    {
                                        //kapatmaBorc = kapatmaTutari;
                                        if (Math.Round(detay.borc, 2) - Math.Round(detay.alacak, 2) > 0 && detay.mahsupBelgeNo != "") islemeAlma = true;
                                    }
                                    else
                                    {
                                        //Borç-Alacak çalışıyorsa
                                        if (Math.Round(detay.borc, 2) == Math.Round(detay.alacak, 2)) islemeAlma = true;
                                    }
                                }
                            }

                            //Yukarıda yapılan işlemşerden sonra mahsup belgesimi kontrolü (eğer mahsup belgesinde bu hesap kod için mahsup yapılmıyorsa onu rapora katmak için hesap bazında bakmak gerekiyor
                            if (!islemeAlma)
                            {
                                KapanisBilgileri kap = new KapanisBilgileri();
                                kap.yil = oem.yil;
                                kap.belgeNo = oem.belgeNo;
                                kap.kHesapKod = detay.hesapKod;

                                ObjectArray kl = (ObjectArray)htKapatmaBelge[kap.yil + "-" + kap.belgeNo + "-" + kap.kHesapKod];

                                if (kl == null)
                                {
                                    kl = s.KapanisBilgisiListele(kap);
                                    htKapatmaBelge[kap.yil + "-" + kap.belgeNo + "-" + kap.kHesapKod] = kl;
                                }

                                if (kl.objeler.Count > 0) islemeAlma = true;
                            }

                            if (islemeAlma) continue;
                            //*********************************************************************************

                            detay.borc = detay.borc + kapatmaBorc;
                            detay.alacak = detay.alacak + kapatmaAlacak;

                            string tertip = TNS.MUH.Arac.TertipStrOl(detay);
                            if (tertip == "") tertip = oem.kurKod;

                            TNS.MUH.BuyukDefter df = new TNS.MUH.BuyukDefter();
                            df.YEVYevTarih = oem.YEVYevTarih;
                            df.YEVYevNo = oem.YEVYevNo;
                            df.hesapKod = detay.hesapKod;
                            df.ilgiliNo = oem.ilgiliNo;
                            df.ilgiliAd = oem.ilgiliAd;
                            df.altHesap = detay.altHesapKod;
                            df.altHesapAd = detay.altHesapAd;
                            df.kur = detay.kur;
                            df.borc = detay.borc;
                            df.alacak = detay.alacak;
                            df.bakiye = kalanTutar;

                            liste.objeler.Add(df);
                        }
                    }
                    liste.objeler.Sort(new Sirala());

                    double arat1 = 0.0;
                    double arat2 = 0.0;
                    double arat3 = 0.0;
                    double gnlt1 = 0.0;
                    double gnlt2 = 0.0;
                    double gnlt3 = 0.0;
                    string eskiHesap = "";
                    foreach (TNS.MUH.BuyukDefter d in liste.objeler)
                    {
                        if (eskiHesap != d.hesapKod && eskiHesap != "")
                        {
                            XLS.SatirAc(satir, 1);
                            XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 9, satir, 0);
                            XLS.HucreDegerYaz(satir, 5, "ARA TOPLAM");
                            XLS.HucreDegerYaz(satir, 6, arat1);
                            XLS.HucreDegerYaz(satir, 7, arat2);
                            XLS.HucreDegerYaz(satir, 8, arat3);
                            XLS.ArkaPlanRenk(satir, 0, satir, 8, TabloRenk.VERY_LIGHT_YELLOW);
                            arat1 = 0;
                            arat2 = 0;
                            arat3 = 0;
                            satir++;
                        }

                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 9, satir, 0);
                        XLS.HucreDegerYaz(satir, 0, d.YEVYevTarih.ToString());
                        XLS.HucreDegerYaz(satir, 1, d.YEVYevNo);
                        XLS.HucreDegerYaz(satir, 2, d.hesapKod);
                        XLS.HucreDegerYaz(satir, 3, string.IsNullOrWhiteSpace(d.altHesap) ? d.ilgiliNo : d.altHesap);
                        XLS.HucreDegerYaz(satir, 4, string.IsNullOrWhiteSpace(d.altHesapAd) ? d.ilgiliAd : d.altHesapAd);
                        XLS.HucreDegerYaz(satir, 5, d.kur);
                        XLS.HucreDegerYaz(satir, 6, d.borc);
                        XLS.HucreDegerYaz(satir, 7, d.alacak);
                        XLS.HucreDegerYaz(satir, 8, d.bakiye);

                        arat1 += d.borc;
                        arat2 += d.alacak;
                        arat3 += d.bakiye;
                        gnlt1 += d.borc;
                        gnlt2 += d.alacak;
                        gnlt3 += d.bakiye;

                        eskiHesap = d.hesapKod;
                        satir++;
                    }

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 9, satir, 0);
                    XLS.HucreDegerYaz(satir, 5, "ARA TOPLAM");
                    XLS.HucreDegerYaz(satir, 6, arat1);
                    XLS.HucreDegerYaz(satir, 7, arat2);
                    XLS.HucreDegerYaz(satir, 8, arat3);
                    XLS.ArkaPlanRenk(satir, 0, satir, 8, TabloRenk.VERY_LIGHT_YELLOW);
                    satir++;
                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(baslaSatir, 0, baslaSatir, 9, satir, 0);
                    XLS.HucreDegerYaz(satir, 5, "GENEL TOPLAM");
                    XLS.HucreDegerYaz(satir, 6, gnlt1);
                    XLS.HucreDegerYaz(satir, 7, gnlt2);
                    XLS.HucreDegerYaz(satir, 8, gnlt3);
                    XLS.ArkaPlanRenk(satir, 0, satir, 8, TabloRenk.ICE_BLUE);
                    satir++;
                }
            }

            XLS.DosyaSaklaTamYol();

            return sonucDosyaAd;
        }

        void BaslikYaz(OrtakClass.Tablo XLS, int yil, int saymanlik, string saymanlikAd, string kurum, string kurumAd, string birim, string birimAd, TNSDateTime tarih1, TNSDateTime tarih2, int listeTur)
        {
            XLS.HucreAdBulYaz("Yil", " : " + yil.ToString());
            XLS.HucreAdBulYaz("KurumAd", " : " + kurum + " - " + kurumAd);

            if (saymanlik > 0)
            {
                saymanlikAd = OrtakClass.GenelIslemler.KodAd(11, saymanlik.ToString(), true);
                XLS.HucreAdBulYaz("SaymanlikAd", " : " + saymanlik + " - " + saymanlikAd);
            }
            else
                XLS.HucreAdBulYaz("SaymanlikAd", " : Kurum Geneli");

            if (birim.Trim() != "")
            {
                birimAd = OrtakClass.GenelIslemler.KodAd(12, birim + "-" + saymanlik.ToString(), false);
                XLS.HucreAdBulYaz("BirimAdi", " : " + birim + " - " + birimAd);
            }
            else
                XLS.HucreAdBulYaz("BirimAdi", " : Kurum Geneli");

            XLS.HucreAdBulYaz("Bastar", tarih1.ToString());
            XLS.HucreAdBulYaz("Bittar", tarih2.ToString());

            string paraBirimi = ButceMuhasebeGenel.DondurParaBirimi(1).kisaltma;
            if (listeTur == 2) paraBirimi = "DÖVİZ";
            XLS.HucreAdBulYaz("ParaBirimi", paraBirimi);

        }

        ObjectArray HesapKoduBilgisiAl(IMUHServis servisMUH, int yil, string hesapKodlar)
        {
            HesapPlaniSatir hesapKodKriter = new HesapPlaniSatir();
            hesapKodKriter.yil = OrtakFonksiyonlar.ConvertToInt(yil, DateTime.Now.Year);
            hesapKodKriter.hesapKod = hesapKodlar;
            ObjectArray hesapKodBilgileri = servisMUH.HesapPlaniListele(new TNS.KYM.Kullanici(), hesapKodKriter);
            return hesapKodBilgileri;
        }

        class Sirala : System.Collections.IComparer
        {
            public int Compare(object o1, object o2)
            {
                if (o1 == o2) return 0;
                TNS.MUH.BuyukDefter i_o1 = (TNS.MUH.BuyukDefter)o1;
                TNS.MUH.BuyukDefter i_o2 = (TNS.MUH.BuyukDefter)o2;
                return (i_o1.hesapKod + i_o1.YEVYevNo.ToString("0000000")).CompareTo((i_o2.hesapKod + i_o2.YEVYevNo.ToString("0000000")));
            }
        }
    }

    public class EmanetlerDefteri
    {
        OrtakClass.Tablo XLS;
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            int listeTur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["ListeTur"], 0);

            string basHesap = (string)raporParam["BasHesap"];

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            oEmri.sorguTarihSiraliMi = true;

            oEmri.formDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
            oEmri.fisTurYap(fisTur);

            OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(oEmri.yil);
            oEmriDetay.hesapKod = basHesap.Trim().Replace(".", "").Trim();
            oEmri.detay.objeler.Add(oEmriDetay);

            ObjectArray mb = s.TaahhutListele((TNS.KYM.Kullanici)kullanan, oEmri);

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            if (mb.ObjeSayisi != 3)
                throw new Exception("Metot hatalı çalıştı.");


            HesapPlaniSatir hesPlani = (HesapPlaniSatir)mb.objeler[0];
            TNSCollection tcGiris = (TNSCollection)mb.objeler[1];
            TNSCollection tcCikis = (TNSCollection)mb.objeler[2];

            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int baslaSatir = 0, baslaSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
            int yukseklik = XLS.SatirYukseklikAl(baslaSatir);

            int girisSatir = 0, girisSutun = 0;
            XLS.HucreAdAdresCoz("Giris", ref girisSatir, ref girisSutun);

            int cikisSatir = 0, cikisSutun = 0;
            XLS.HucreAdAdresCoz("Cikis", ref cikisSatir, ref cikisSutun);

            int sheetNo = 0, fSatir1 = 0, fSutun1 = 0, fSatir2 = 0, fSutun2 = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref fSatir1, ref fSutun1, ref fSatir2, ref fSutun2);

            int sat = baslaSatir;
            for (int i = 0; i < Math.Max(tcGiris.Count, tcCikis.Count); i++)
            {
                XLS.HucreKopyala(fSatir1, fSutun1, fSatir1, fSutun2, sat, baslaSutun);
                TNS.MUH.BuyukDefter gir = (TNS.MUH.BuyukDefter)DiziElemanAl(tcGiris, i);
                TNS.MUH.BuyukDefter cik = (TNS.MUH.BuyukDefter)DiziElemanAl(tcCikis, i);
                if (gir != null)
                {
                    XLS.HucreDegerYaz(sat, baslaSutun, i + 1);
                    XLS.HucreDegerYaz(sat, baslaSutun + 1, gir.islemTarih.Oku());
                    XLS.HucreDegerYaz(sat, baslaSutun + 2, gir.belgeNo);
                    XLS.HucreDegerYaz(sat, baslaSutun + 3, gir.kapattigiBelgeNo);
                    XLS.HucreDegerYaz(sat, baslaSutun + 4, gir.altHesap + "-" + gir.altHesapAd);
                    XLS.HucreDegerYaz(sat, baslaSutun + 5, Math.Abs(gir.borc - gir.alacak));
                    XLS.HucreDegerYaz(sat, baslaSutun + 6, gir.aciklama);
                }
                if (cik != null)
                {
                    XLS.HucreDegerYaz(sat, baslaSutun + 8, i + 1);
                    XLS.HucreDegerYaz(sat, baslaSutun + 9, cik.islemTarih.Oku());
                    XLS.HucreDegerYaz(sat, baslaSutun + 10, cik.belgeNo);
                    XLS.HucreDegerYaz(sat, baslaSutun + 12, cik.altHesap + "-" + cik.altHesapAd);
                    XLS.HucreDegerYaz(sat, baslaSutun + 15, Math.Abs(cik.borc - cik.alacak));
                    XLS.HucreDegerYaz(sat, baslaSutun + 16, cik.kapattigiBelgeNo);
                }
                sat++;
            }

            XLS.HucreKopyala(fSatir1 + 2, fSutun1, fSatir1 + 2, fSutun2, sat, baslaSutun);
            XLS.SatirYukseklikAyarla(baslaSatir, sat - 1, yukseklik);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private object DiziElemanAl(TNSCollection dizi, int i)
        {
            if (i < dizi.Count) return dizi[i];
            return null;
        }
    }

    public class TasinirRaporu
    {
        OrtakClass.Tablo XLS;
        string kurumAdi = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

        /// <summary>
        /// Yevmiye raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            string basHesap = (string)raporParam["BasHesap"];

            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;

            OdemeEmriMIFDetay det = new OdemeEmriMIFDetay(yil);
            det.hesapKod = basHesap.Replace(".", "");
            oEmri.detay.Ekle(det);

            ObjectArray mb = s.TasinirRaporu((TNS.KYM.Kullanici)kullanan, oEmri);

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            string saymanlikAd = (string)raporParam["SaymanlikAd"] + "";
            if (saymanlikAd == "")
                saymanlikAd = "Kurum Geneli";
            else
                saymanlikAd = saymanlik + " - " + saymanlikAd;
            XLS.HucreAdBulYaz("SaymanlikAd", saymanlikAd);
            XLS.HucreAdBulYaz("Yil", yil);

            int baslaSatir = 0, baslaSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);

            int satir = baslaSatir;
            string eskiKod = "";
            string kod = "";
            foreach (TasinirRaporuListe form in mb.objeler)
            {
                kod = form.harcamaBirimKod + form.fisNo;
                if (eskiKod == "" || eskiKod != kod)
                {
                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(0, 0, 0, 10, satir, 0);

                    XLS.HucreDegerYaz(satir, 0, form.harcamaBirimKod);
                    XLS.HucreDegerYaz(satir, 1, form.fisNo);
                    XLS.HucreDegerYaz(satir, 2, form.fisTarih.ToString());
                    XLS.HucreDegerYaz(satir, 3, form.ilgili);
                    XLS.HucreDegerYaz(satir, 4, form.belgeNo);
                    satir++;
                }

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(1, 0, 1, 10, satir, 0);

                XLS.HucreDegerYaz(satir, 0, form.hesapKod);
                XLS.HucreDegerYaz(satir, 1, form.adet);
                XLS.HucreDegerYaz(satir, 2, form.tutar);
                XLS.HucreDegerYaz(satir, 3, form.hesapAdi);

                eskiKod = form.harcamaBirimKod + form.fisNo;

                satir++;
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    public class GelirStopaj
    {
        OrtakClass.Tablo XLS;
        int kaynakSatirKK;
        int aktifSatir;
        int sutun = 0;

        /// <summary>
        /// Mizan raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            int listeTur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["ListeTur"], 0);
            string birim = (string)raporParam["Birim"];
            string birimAd = "";
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            string altHesap = "";
            string basHesap = (string)raporParam["BasHesap"];
            string bitHesap = (string)raporParam["BitHesap"];
            string saymanlikAd = (string)raporParam["SaymanlikAd"] + "";
            double stopajOran = (double)raporParam["StopajOran"];

            string ayAralik = tarih1.ToString() + " - " + tarih2.ToString();
            Sonuc sonuc = s.MuhasebeSorgu((TNS.KYM.Kullanici)kullanan, yil, kurum, saymanlik, birim, tarih1, tarih2, 100, basHesap, bitHesap, tur, fisTur, listeTur, "", "", altHesap, 2);

            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);
            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdBulYaz("Yil", yil.ToString());
            XLS.HucreAdBulYaz("KurumAd", kurum + " - " + kurumAd);
            if (birim.Trim() != "")
                birimAd = " - (" + birim + ") " + OrtakClass.GenelIslemler.KodAd(12, birim + "-" + saymanlik.ToString(), false);

            XLS.HucreAdBulYaz("SaymanlikAd", saymanlikAd + birimAd);
            XLS.HucreAdBulYaz("AyAralik", ayAralik);

            string paraBirimi = ButceMuhasebeGenel.DondurParaBirimi(1).kisaltma;
            if (listeTur == 2) paraBirimi = "DÖVİZ";
            XLS.HucreAdBulYaz("ParaBirimi", paraBirimi);

            int satir = 0;
            sutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            XLS.HucreAdAdresCoz("FormatSatir", ref kaynakSatirKK, ref sutun);

            aktifSatir = satir;
            string sql = "SELECT MUHMIFFORM.ISLEMTARIH, MUHMIF.HESAPKOD,  MUHHESAPPLANI.ACIKLAMA, MUHMIF.ALTHESAPKOD, MUHMIF.ALACAK, " + sonuc.bilgiStr + " ORDER BY 1,2,4";
            TNS.UZY.VTSorgula vtS = new VTSorgula(sql, "BaglantiSatiriBUM", 0);

            int yuks = XLS.SatirYukseklikAl(aktifSatir);
            double topAlacak = 0;
            double topStopaj = 0;
            while (vtS.satirOku() != null)
            {
                XLS.HucreKopyala(kaynakSatirKK, sutun, kaynakSatirKK, sutun + 12, aktifSatir, sutun);
                XLS.HucreDegerYaz(aktifSatir, sutun, vtS.DTAl(0));
                XLS.HucreDegerYaz(aktifSatir, sutun + 1, OrtakFonksiyonlar.KodNoktaKoy("999.99.99.99.99.99.99.99", vtS.StringAl(1)));
                XLS.HucreDegerYaz(aktifSatir, sutun + 2, vtS.StringAl(2));

                XLS.HucreDegerYaz(aktifSatir, sutun + 3, vtS.StringAl(3));
                XLS.HucreDegerYaz(aktifSatir, sutun + 4, vtS.StringAl(5));
                double alacak = vtS.DoubleAl(4);
                double stopaj = OrtakFonksiyonlar.YuzdeHesapla(alacak, stopajOran, 2);
                XLS.HucreDegerYaz(aktifSatir, sutun + 5, alacak);
                XLS.HucreDegerYaz(aktifSatir, sutun + 6, stopaj);
                topAlacak += alacak;
                topStopaj += stopaj;
                aktifSatir++;
            }
            if (!string.IsNullOrWhiteSpace(vtS.hata))
                throw new Exception(vtS.hata);
            XLS.HucreKopyala(kaynakSatirKK + 1, sutun, kaynakSatirKK + 1, sutun + 12, aktifSatir, sutun);
            XLS.HucreDegerYaz(aktifSatir, sutun + 5, topAlacak);
            XLS.HucreDegerYaz(aktifSatir, sutun + 6, topStopaj);
            XLS.SatirYukseklikAyarla(satir, aktifSatir, yuks);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    public class GelirStopajAktar
    {
        OrtakClass.Tablo XLS;

        /// <summary>
        /// Mizan raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            int listeTur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["ListeTur"], 0);
            string birim = (string)raporParam["Birim"];
            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);
            ENUMFISTUR[] fisTur = MizanTertip.IntDiziYap((string)raporParam["FisTur"]);
            string altHesap = "-999-";
            string basHesap = (string)raporParam["BasHesap"];
            string bitHesap = (string)raporParam["BitHesap"];
            string saymanlikAd = (string)raporParam["SaymanlikAd"] + "";
            double stopajOran = (double)raporParam["StopajOran"];
            int seviye = 2;
            MizanBilgi mb = s.Mizan((TNS.KYM.Kullanici)kullanan, yil, kurum, saymanlik, birim, tarih1, tarih2, seviye, basHesap, bitHesap, tur, fisTur, listeTur, altHesap);

            if (!mb.sonuc.islemSonuc)
                throw new Exception(mb.sonuc.hataStr);
            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.BosDosyaAc(sonucDosyaAd);

            int aktifSatir = 0;
            for (int i = 0; i < mb.anaHesaplar.ObjeSayisi; i++)
            {
                AnaHesap ah = (AnaHesap)mb.anaHesaplar[i];
                for (int j = 0; j < ah.detayHesaplar.objeler.Count; j++)
                {
                    HesapBilgi alth = (HesapBilgi)ah.detayHesaplar[j];

                    XLS.HucreDegerYaz(aktifSatir, 0, "Stopaj Hesap Kodu Yazınız");
                    XLS.HucreDegerYaz(aktifSatir, 1, "");

                    XLS.HucreDegerYaz(aktifSatir, 2, OrtakFonksiyonlar.YuzdeHesapla(alth.alacak, stopajOran, 2));
                    XLS.HucreDegerYaz(aktifSatir, 3, 0);
                    XLS.HucreDegerYaz(aktifSatir, 4, alth.altHesapKod);
                    XLS.HucreDegerYaz(aktifSatir, 5, string.Format("{0} için toplam {1:0,000.00} lira alacak kaydının yüzde {2:0} tutarı", alth.hesapAd, alth.alacak, stopajOran));
                    XLS.HucreDegerYaz(aktifSatir, 6, birim);
                    aktifSatir++;
                }
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    public class FirmalardanAlinan
    {
        OrtakClass.Tablo XLS;
        string kurumAdi = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

        /// <summary>
        /// Yevmiye raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            string altHesap = (string)raporParam["AltHesap"];
            string aciklama = (string)raporParam["Aciklama"];
            string raporTur = (string)raporParam["KesintiRaporTur"];
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);

            TNS.KYM.Kullanici kullananK = (TNS.KYM.Kullanici)kullanan;

            //İdari bilgileri al
            TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
            TNS.HRC.IdareBilgi ib = new TNS.HRC.IdareBilgi();
            ib.muhasebe = saymanlik;
            ib.birim = "000";
            ib = servisHRC.IdareBilgiListele(kullananK, ib);

            //Alt hesap (firma) bilgilerini al
            TNS.HRC.FirmaBilgisi firma = new TNS.HRC.FirmaBilgisi();
            firma.muhasebe = saymanlik;
            firma.vergiNo = altHesap;
            ObjectArray listeFirma = servisHRC.FirmaBilgisiListele(kullananK, firma);
            if (listeFirma.ObjeSayisi > 0)
                firma = (TNS.HRC.FirmaBilgisi)listeFirma.objeler[0];


            //Ödemne Emri
            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            //oEmri.yil = yil;
            //oEmri.kurum = kurum;
            oEmri.muhasebe = saymanlik;
            oEmri.birim = birim;
            //oEmri.ilgiliNo = altHesap;

            OdemeEmriMIFDetay od = new OdemeEmriMIFDetay(DateTime.Now.Year);
            od.altHesapKod = altHesap;
            oEmri.detay.objeler.Add(od);

            oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            oEmri.formDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
            oEmri.sorguTarihSiraliMi = true;
            ObjectArray mb = s.OdemeEmriMIFListele(kullananK, oEmri, true, false);

            List<string> hesaplarListe = new List<string>();
            List<int> yevmiyeListe = new List<int>();
            Hashtable htHesaplarBaslik = new Hashtable();
            Hashtable htHesaplarToplam = new Hashtable();
            Hashtable htYevmiyeHesaplar = new Hashtable();
            Hashtable htYevmiye = new Hashtable();
            Hashtable htOdenen = new Hashtable();

            if (raporTur == "1")
            {
                hesaplarListe.Add("830");
                TNS.MUH.Arac.HTStringEkle(htHesaplarBaslik, "830", "Tahakkuk Toplamı");
            }

            foreach (OdemeEmriMIFForm oe in mb.objeler)
            {
                bool ekleYevmiye = false;
                foreach (OdemeEmriMIFDetay oed in oe.detay.objeler)
                {
                    if (oed.kesinti > 0)
                    {
                        //Daha önce eklenmiş ise tekrar ekleme
                        bool ekle = true;
                        foreach (string hk in hesaplarListe)
                        {
                            if (hk == oed.hesapKod)
                            {
                                ekle = false;
                                break;
                            }
                        }
                        if (ekle)
                            hesaplarListe.Add(oed.hesapKod);

                        TNS.MUH.Arac.HTStringEkle(htHesaplarBaslik, oed.hesapKod, oed.hesapAd);

                        TNS.MUH.Arac.HTDoubleEkle(htYevmiyeHesaplar, oe.YEVYevNo + "-" + oed.hesapKod, Math.Abs(oed.borc - oed.alacak), true);
                        ekleYevmiye = true;
                    }

                    if (oed.hesapKod.StartsWith("10"))
                        TNS.MUH.Arac.HTDoubleEkle(htOdenen, oe.YEVYevNo.ToString(), Math.Abs(oed.borc - oed.alacak), true);

                    if (oed.hesapKod.StartsWith("830"))
                    {
                        TNS.MUH.Arac.HTDoubleEkle(htYevmiyeHesaplar, oe.YEVYevNo + "-" + "830", Math.Abs(oed.borc - oed.alacak), true);
                    }
                }

                if (ekleYevmiye)
                {
                    yevmiyeListe.Add(oe.YEVYevNo);
                    htYevmiye[oe.YEVYevNo] = oe.YEVYevTarih.ToString();
                }
            }

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int baslaSatir = 0, baslaSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);


            //İdare bilgileri yaz
            XLS.HucreAdBulYaz("txtIdareAdi", ib.adi);
            XLS.HucreAdBulYaz("txtMuhasebeKod", saymanlik);
            XLS.HucreAdBulYaz("txtIdareVD", ib.vd);
            XLS.HucreAdBulYaz("txtIdareVNo", ib.vNo);
            //********************************************
            //İlgili yaz
            XLS.HucreAdBulYaz("txtIlgili", firma.ad);
            //********************************************
            //Açıklama yaz
            XLS.HucreAdBulYaz("txtAciklama", aciklama);
            //********************************************
            //İmza yaz
            XLS.HucreAdBulYaz("txtYetkiliAdi", GenelIslemler.KullaniciDegiskenGetir(kullananK.kullaniciKodu, "RAPORIMZAYETKILIADI"));
            XLS.HucreAdBulYaz("txtYetkiliBirim", GenelIslemler.KullaniciDegiskenGetir(kullananK.kullaniciKodu, "RAPORIMZAYETKILIBIRIM"));
            XLS.HucreAdBulYaz("txtYetkiliUnvan", GenelIslemler.KullaniciDegiskenGetir(kullananK.kullaniciKodu, "RAPORIMZAYETKILIUNVAN"));
            //********************************************
            XLS.HucreAdBulYaz("txtSorguTarih", tarih1.ToString() + "-" + tarih2.ToString());
            XLS.HucreAdBulYaz("txtTarih", DateTime.Now.ToShortDateString());

            int satir = baslaSatir;
            int baslikSatir = baslaSatir - 2;
            int anaHesapKodKolon = 3;
            int baslikKolon = anaHesapKodKolon;
            int sablonTutarKolon = 3;
            foreach (string hesapKod in hesaplarListe)
            {
                string hesapBaslik = (string)htHesaplarBaslik[hesapKod];
                XLS.HucreKopyala(0, sablonTutarKolon, 0, sablonTutarKolon, 0, baslikKolon);
                XLS.HucreKopyala(baslikSatir, sablonTutarKolon, baslikSatir, sablonTutarKolon, baslikSatir, baslikKolon);
                XLS.HucreBirlestir(baslikSatir, baslikKolon, baslikSatir + 1, baslikKolon);
                XLS.DuseyCizgiCiz(baslikSatir, baslikSatir + 1, baslikKolon + 1, LineStyle.THIN, TabloRenk.BLACK);
                XLS.HucreDegerYaz(baslikSatir, baslikKolon, hesapBaslik);
                baslikKolon++;
            }
            XLS.HucreKopyala(0, sablonTutarKolon, 0, sablonTutarKolon, 0, baslikKolon);
            XLS.HucreKopyala(baslikSatir, sablonTutarKolon, baslikSatir, sablonTutarKolon, baslikSatir, baslikKolon);
            XLS.HucreBirlestir(baslikSatir, baslikKolon, baslikSatir + 1, baslikKolon);
            XLS.DuseyCizgiCiz(baslikSatir, baslikSatir + 1, baslikKolon + 1, LineStyle.THIN, TabloRenk.BLACK);
            XLS.HucreDegerYaz(baslikSatir, baslikKolon, "Net Ödenen");

            double odenenTutar = 0;
            foreach (int yevNo in yevmiyeListe)
            {
                string yevTarih = (string)htYevmiye[yevNo];

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(0, 0, 0, 40, satir, 0);
                XLS.HucreDegerYaz(satir, 1, yevTarih);
                XLS.HucreDegerYaz(satir, 2, yevNo);

                int hesapKodKolon = anaHesapKodKolon;

                //Kesintiler
                foreach (string hesapKod in hesaplarListe)
                {
                    double tutar = OrtakFonksiyonlar.ConvertToDbl(htYevmiyeHesaplar[yevNo + "-" + hesapKod]);
                    TNS.MUH.Arac.HTDoubleEkle(htHesaplarToplam, hesapKod, tutar, true);
                    XLS.HucreDegerYaz(satir, hesapKodKolon, tutar);

                    hesapKodKolon++;
                }
                //Net Ödenen

                double tahakkuk = OrtakFonksiyonlar.ConvertToDbl(htYevmiyeHesaplar[yevNo + "-" + "830"]);

                double odenen = 0;
                if (tahakkuk > 0)
                {
                    odenen = OrtakFonksiyonlar.ConvertToDbl(htOdenen[yevNo.ToString()]);
                    odenenTutar += odenen;
                }
                XLS.HucreDegerYaz(satir, hesapKodKolon, odenen);

                satir++;
            }

            //Alt toplamları yaz
            XLS.HucreKopyala(0, anaHesapKodKolon, 0, 40, satir, anaHesapKodKolon);
            //XLS.HucreDegerYaz(satir, anaHesapKodKolon - 1, tahakkukTutar);
            foreach (string hesapKod in hesaplarListe)
            {
                double tutar = OrtakFonksiyonlar.ConvertToDbl(htHesaplarToplam[hesapKod]);
                XLS.HucreDegerYaz(satir, anaHesapKodKolon, tutar);
                anaHesapKodKolon++;
            }
            XLS.HucreDegerYaz(satir, anaHesapKodKolon, odenenTutar);
            XLS.ArkaPlanRenk(satir, 1, satir, anaHesapKodKolon, System.Drawing.Color.LightGray);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    public class ManasGunlukTakip
    {
        OrtakClass.Tablo XLS;
        string kurumAdi = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();

        /// <summary>
        /// Yevmiye raporu üreten fonksiyon
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            string hesapKodlari = "";
            for (int i = 0; i < 60; i++)
            {
                if (hesapKodlari != "") hesapKodlari += ",";
                hesapKodlari += XLS.HucreDegerAl(i, 0);
            }

            TNS.MUH.IMUHServis s = TNS.MUH.Arac.Tanimla();
            //int yil = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);
            string kurum = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBulAd();
            int saymanlik = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);

            //Kurlar
            TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
            TNS.HRC.MerkezBankasiKurSinif mbKur = new TNS.HRC.MerkezBankasiKurSinif();
            mbKur.kurTarih = tarih1;
            string[] kurlar = { "TRY", "KGS", "EUR" };
            ObjectArray kurListe = servisHRC.MerkezBankKurDegerleri(mbKur, kurlar);
            //**************


            TNS.KYM.Kullanici kullananK = (TNS.KYM.Kullanici)kullanan;
            TNS.MUH.ManasGunlukTakip kriter = new TNS.MUH.ManasGunlukTakip();
            kriter.sorguTarih = tarih1;
            kriter.hesapKod = hesapKodlari.Replace(".", "");

            ObjectArray liste = s.ManasGunlukTakipGetir(kullananK, kriter);

            foreach (TNS.MUH.ManasGunlukTakip item in liste.objeler)
            {
                for (int i = 0; i < 60; i++)
                {
                    hesapKodlari = XLS.HucreDegerAl(i, 0);
                    hesapKodlari = hesapKodlari.Replace(".", "");

                    if (hesapKodlari == "") continue;

                    if (hesapKodlari == item.hesapKod.Replace(".", ""))
                    {
                        string hucreTur = XLS.HucreDegerAl(i, 2);
                        if (hucreTur == "devir") XLS.HucreDegerYaz(i, 2, item.bakiye);
                        if (hucreTur == "devirD") XLS.HucreDegerYaz(i, 2, item.dovizBakiye);
                        if (hucreTur == "alınanAvans") XLS.HucreDegerYaz(i, 2, item.bakiyeBorc);
                        if (hucreTur == "alınanAvansD") XLS.HucreDegerYaz(i, 2, item.dovizBakiyeBorc);
                        if (hucreTur == "ödenekTahsis") XLS.HucreDegerYaz(i, 2, item.bakiyeBorc);
                        if (hucreTur == "ödenekTahsisD") XLS.HucreDegerYaz(i, 2, item.dovizBakiyeBorc);
                        if (hucreTur == "muhasebe") XLS.HucreDegerYaz(i, 2, item.bakiye);
                        if (hucreTur == "muhasebeD") XLS.HucreDegerYaz(i, 2, item.dovizBakiye);
                        if (hucreTur == "hesapAcma") XLS.HucreDegerYaz(i, 3, item.borc);
                        if (hucreTur == "hesapAcmaD") XLS.HucreDegerYaz(i, 3, item.dovizBorc);

                        hucreTur = XLS.HucreDegerAl(i, 3);
                        if (hucreTur == "tahsil") XLS.HucreDegerYaz(i, 3, item.borc);
                        if (hucreTur == "tahsilD") XLS.HucreDegerYaz(i, 3, item.dovizBorc);
                        if (hucreTur == "tamamlananEvrak") XLS.HucreDegerYaz(i, 3, item.bakiyeAlacak);
                        if (hucreTur == "tamamlananEvrakD") XLS.HucreDegerYaz(i, 3, item.dovizBakiyeAlacak);
                        if (hucreTur == "serbestBırakılan") XLS.HucreDegerYaz(i, 3, item.bakiyeAlacak);
                        if (hucreTur == "serbestBırakılanD") XLS.HucreDegerYaz(i, 3, item.dovizBakiyeAlacak);
                        if (hucreTur == "muhasebe") XLS.HucreDegerYaz(i, 3, item.bakiye);
                        if (hucreTur == "muhasebeD") XLS.HucreDegerYaz(i, 3, item.dovizBakiye);
                        if (hucreTur == "kapama") XLS.HucreDegerYaz(i, 3, item.borc);
                        if (hucreTur == "kapamaD") XLS.HucreDegerYaz(i, 3, item.dovizBorc);

                        hucreTur = XLS.HucreDegerAl(i, 4);
                        if (hucreTur == "tediye") XLS.HucreDegerYaz(i, 4, item.alacak);
                        if (hucreTur == "tediyeD") XLS.HucreDegerYaz(i, 4, item.dovizAlacak);
                        if (hucreTur == "muhasebe") XLS.HucreDegerYaz(i, 4, item.bakiye);
                        if (hucreTur == "muhasebeD") XLS.HucreDegerYaz(i, 4, item.dovizBakiye);

                        break;
                    }
                }
            }

            for (int i = 0; i < 60; i++)
            {
                hesapKodlari = XLS.HucreDegerAl(i, 0);
                if (hesapKodlari == "") continue;

                for (int j = 2; j < 5; j++)
                {
                    string hucreTur = XLS.HucreDegerAl(i, j);
                    if (hucreTur == "devir") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "devirD") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "alınanAvans") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "alınanAvansD") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "ödenekTahsis") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "ödenekTahsisD") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "muhasebe") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "muhasebeD") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "tahsil") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "tahsilD") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "tamamlananEvrak") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "tamamlananEvrakD") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "serbestBırakılan") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "serbestBırakılanD") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "tediye") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "tediyeD") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "hesapAcma") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "hesapAcmaD") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "kapama") XLS.HucreDegerYaz(i, j, 0);
                    if (hucreTur == "kapamaD") XLS.HucreDegerYaz(i, j, 0);
                }
            }

            foreach (TNS.HRC.KurDegerleri kurDegeri in kurListe.objeler)
            {
                if (kurDegeri.paraBirimi.kisaltma == "KGS")
                    XLS.HucreAdBulYaz("dovizCOM", 1 / OrtakFonksiyonlar.ConvertToDbl(kurDegeri.alis));
                if (kurDegeri.paraBirimi.kisaltma == "TRY")
                    XLS.HucreAdBulYaz("dovizTRY", 1 / OrtakFonksiyonlar.ConvertToDbl(kurDegeri.alis));
            }

            XLS.HucreAdBulYaz("tarih", tarih1.ToString());
            XLS.CalculateFormula();
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }


    /// <summary>Eski ButceMuhasebe\Genel.cs ButceMuhasebeGenel'den bu raporların kullandığı iki işlev (aynen).</summary>
    public class ButceMuhasebeGenel
    {
        public static TNS.BKD.ParaBirimi DondurParaBirimi(int paraBirimiKod)
        {
            TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
            TNS.KYM.Kullanici kullanan = new TNS.KYM.Kullanici();

            TNS.BKD.ParaBirimi para = new TNS.BKD.ParaBirimi();
            ObjectArray paralar = servisHRC.ParaBirimiListele(kullanan, para);

            if (paralar.objeler.Count > 0)
            {
                foreach (TNS.BKD.ParaBirimi bilgi in paralar.objeler)
                {
                    if (bilgi.kod == paraBirimiKod)
                        return bilgi;
                }
            }
            return new TNS.BKD.ParaBirimi();
        }

        public static string ParaBirimiDondur()
        {
            string pBirimi = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEPARABIRIMI") + "";
            if (pBirimi.Trim() == "") pBirimi = "TL";
            return pBirimi;
        }
    }
}
