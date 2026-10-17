using System;
using System.Collections.Generic;
using System.Text;
using TNS.KYM;
using OrtakClass;
using TNS.BKD;
using TNS;
using TNS.UZY;
using TNS.BHF;
using System.Collections;

namespace ButceForm
{
    /// <summary>
    /// Bütçe Formlar Sınıfı
    /// </summary>
    public class BHFFormlar
    {
        /// <summary>
        /// Yıl ve Form Adı anahtar alanlarına göre Şablon adını bulan fonksiyon
        /// </summary>
        /// <param name="yil">Yıl değerini tutan Int türünde nesne</param>
        /// <param name="formAd">Form Adı değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalıştırılması sonucundan String türünde nesne döndürür</returns>
        public static string SablonAdBul(int yil, string formAd)
        {
            return formAd;
        }

        /// <summary>
        /// Yıl ve Form Adına göre Form numarasını bulan fonksiyon
        /// </summary>
        /// <param name="yil">Yıl değerini tutan Int türünde nesne</param>
        /// <param name="formAd">Form adı değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalıştırılması sonucunda String türünde nesne döndürür</returns>
        public static string FormNoBul(int yil, string formAd)
        {
            if (formAd == "BirimHizmetMaliyeti")
            {
                if (yil >= 2009)
                    return "10";
                else
                    return "11";
            }
            else if (formAd == "FizikselDegerler")
            {
                if (yil >= 2009)
                    return "11";
                else
                    return "13";
            }
            else if (formAd == "UluslararasiKuruluslar")
            {
                if (yil >= 2009)
                    return "17";
                else
                    return "22";
            }
            else if (formAd == "EdinilecekTasitlar")
            {
                if (yil >= 2009)
                    return "18";
                else
                    return "23";
            }
            else if (formAd == "MevcutTasitlar")
            {
                if (yil >= 2009)
                    return "19";
                else
                    return "24";
            }
            else if (formAd == "OzelGuvenlik")
            {
                if (yil >= 2009)
                    return "21";
                else
                    return "25";
            }
            else if (formAd == "DonerSermaye")
            {
                if (yil >= 2009)
                    return "23";
                else
                    return "26";
            }
            else if (formAd == "FonGelirGider")
            {
                if (yil >= 2009)
                    return "24";
                else
                    return "27";
            }
            else if (formAd == "TavanUstuOdenek")
            {
                if (yil >= 2009)
                    return "25";
                else
                    return "28";
            }
            else if (formAd == "HizmetAlimiTasitBilgi")
            {
                return "20";
            }
            else if (formAd == "TemizlikHizmeti")
            {
                return "20";
            }
            else
                return "00";
        }

    }

    /// <summary>
    /// Ek form raporları; eski ButceForm/ButceFormRaporlari.cs kodunun aynısıdır (EkFormAna sayfasındaki formların Yazdır tuşları).
    /// Değişenler: şablon yolu RaporSablonYol ayarından, kurum kodu/adı TNS.DEG'den okunur; HTML gerekçe raporları
    /// (HizmetGerekcesiRaporu.cs'e alındı) ve Response'a yazan yordamlar çıkarıldı.
    /// </summary>
    public class ButceFormRaporlari
    {
        /// <summary>
        /// Yıl değişkeni
        /// </summary>
        private int yil;
        /// <summary>
        /// Kurum Kod değişkeni
        /// </summary>
        private string kurumKod;
        /// <summary>
        /// Kurum Ad değişkeni
        /// </summary>
        private string kurumAd;
        /// <summary>
        /// Kurumsal Kod değişkeni
        /// </summary>
        private string kurKod;
        /// <summary>
        /// Kurumsal Ad değişkeni
        /// </summary>
        private string kurAd;
        /// <summary>
        /// Birim Kod değişkeni
        /// </summary>
        private string birimKod;
        /// <summary>
        /// Birim Ad değişkeni
        /// </summary>
        private string birimAd;
        /// <summary>
        /// Kullanan değişkeni
        /// </summary>
        private Kullanici kullanan;
        /// <summary>
        /// Servis değişkeni
        /// </summary>
        private IBHFServis servis;
        /// <summary>
        /// Excel raporları için kullanılan Sonuç dosya ad değişkeni
        /// </summary>
        private string sonucDosyaAd;
        /// <summary>
        /// Excel raporları için kullanılan Şablon ad değişkeni
        /// </summary>
        private string sablonAd;

        /// <summary>
        /// Senaryo değerini tutan değişken
        /// </summary>
        private string senaryo;
        /// <summary>
        /// Aşama ad değişkeni
        /// </summary>
        private string asamaAd;

        /// <summary>
        /// Bütçe Hazırlık Form ve Uzay Servisine bağlanarak, raporlarda kullanılacak parametreleri (yıl, kurkod, aşama v.b.) hazırlayan ve konfigürasyon dosyasından raporlarda kullanılacak şablonun dizin değerini alan fonksiyon 
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="p_SonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="p_SablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="p_SablonDefault">Rapor için kullanılacak Excel Şablon ön tanımlama adını tutan String türünde nesne</param>
        /// <param name="sonucDosyaEk">Raporun üretildikten sonra oluşturulacak dosya adının uzantısını tutan String türünde nesne</param>
        private void DegerAl(object o_kullanan, object o_servis, System.Collections.Hashtable raporParam, string p_SonucDosyaAd, string p_SablonAd, string p_SablonDefault, string sonucDosyaEk)
        {
            kullanan = (Kullanici)o_kullanan;
            if (o_servis.GetType() != typeof(IBHFServis))
                servis = TNS.BHF.Arac.Tanimla();
            else
                servis = (IBHFServis)o_servis;

            IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

            yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            kurKod = (string)raporParam["KurKod"] + "";
            kurAd = servisUZY.UzayDegeriStr(kullanan, "ABSKUR", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), kurKod), true);
            kurumKod = TNS.DEG.Arac.VarsayilanKurumBul();
            kurumAd = ButceHazirlikv2.BHGiderRaporlari.KurumAd();

            birimKod = (string)raporParam["BirimKod"];
            if (string.IsNullOrWhiteSpace(birimKod))
                birimKod = (string)raporParam["Birim"];
            birimAd = (string)raporParam["BirimAd"] + "";
            if (string.IsNullOrWhiteSpace(birimAd) && !string.IsNullOrWhiteSpace(birimKod))
                birimAd = servisUZY.UzayDegeriStr(kullanan, "ABSKUR", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), birimKod), true);

            if (String.IsNullOrEmpty(sonucDosyaEk))
                sonucDosyaEk = GenelIslemler.ExcelTur();
            sonucDosyaAd = p_SonucDosyaAd + "." + sonucDosyaEk;
            if (String.IsNullOrEmpty(p_SablonAd))
                sablonAd = Istemci.Ayar.Oku("RaporSablonYol") + "/BHF/" + BHFFormlar.SablonAdBul(yil, p_SablonDefault) + ".xlt";
            else
                sablonAd = p_SablonAd;
            asamaAd = (string)raporParam["AsamaAd"];
            senaryo = (string)raporParam["Senaryo"];
        }

        /// <summary>
        /// Uluslararası kuruluşlara üyelik ve üyeliğin maliyeti raporunu hazırlayan fonksiyon
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="p_SablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="p_SonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string UluslararasiKuruluslarHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "UluslararasiKuruluslar", null);
            string kurumBirim = kurKod;

            try
            {
                UluslarArasiKurulus[] uluslarArasiKuruluslar = servis.UluslarArasiKuruluslarListele(kullanan, yil, kurumBirim);
                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;
                int sutun = 0;

                XLS.DosyaAc(sablonAd, sonucDosyaAd);

                string ekoKod = "0722,0562";

                XLS.HucreAdBulYaz("ButceYil", yil);
                XLS.HucreAdBulYaz("KurumAd", kurumAd);
                XLS.HucreAdBulYaz("BirimAd", kurAd);

                IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
                XLS.HucreAdBulYaz("OncYilOdenek", uzyServis.UzayDegeriDbl(kullanan, "ABSODENEK", (yil - 2).ToString() + "-06.00-0-" + kurumBirim + "---" + ekoKod, false));
                XLS.HucreAdBulYaz("OncYilHarcama", uzyServis.UzayDegeriDbl(kullanan, "ABSHAZMUHASEBAT", (yil - 2).ToString() + "-12-" + kurumBirim + "---" + ekoKod, false));
                XLS.HucreAdBulYaz("YilOdenek", uzyServis.UzayDegeriDbl(kullanan, "ABSODENEK", (yil - 1).ToString() + "-06.00-0-" + kurumBirim + "---" + ekoKod, false));
                XLS.HucreAdBulYaz("YilHazHarcama", uzyServis.UzayDegeriDbl(kullanan, "ABSHAZMUHASEBAT", (yil - 1).ToString() + "-6-" + kurumBirim + "---" + ekoKod, false));
                XLS.HucreAdBulYaz("YilSHTah", uzyServis.UzayDegeriDbl(kullanan, "ABSHARCAMATAHMIN", (yil - 1).ToString() + "-" + senaryo + "-" + kurumBirim + "---" + ekoKod, false));
                XLS.HucreAdBulYaz("Yil1Teklif", uzyServis.UzayDegeriDbl(kullanan, "ABSODENEK", yil.ToString() + "-" + senaryo + "-0-" + kurumBirim + "---" + ekoKod, false));
                XLS.HucreAdBulYaz("Yil2Teklif", uzyServis.UzayDegeriDbl(kullanan, "ABSODENEK", yil.ToString() + "-" + senaryo + "-1-" + kurumBirim + "---" + ekoKod, false));
                XLS.HucreAdBulYaz("Yil3Teklif", uzyServis.UzayDegeriDbl(kullanan, "ABSODENEK", yil.ToString() + "-" + senaryo + "-2-" + kurumBirim + "---" + ekoKod, false));

                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

                int i;
                double[] dizi = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (i = 0; i < uluslarArasiKuruluslar.Length; i++)
                {
                    XLS.HucreKopyala(satir, sutun, satir, sutun + 16, satir + 1, sutun);
                    UluslarArasiKurulus uk = uluslarArasiKuruluslar[i];
                    XLS.HucreDegerYaz(satir, sutun, Convert.ToDouble(uk.siraNo));
                    XLS.HucreDegerYaz(satir, sutun + 1, uk.kurulusAd);
                    XLS.HucreDegerYaz(satir, sutun + 2, uk.yasalDayanak);
                    XLS.HucreDegerYaz(satir, sutun + 3, Convert.ToDouble(uk.toplantiSayisi));

                    XLS.HucreDegerYaz(satir, sutun + 4, Convert.ToDouble(uk.toplantiKatsayisi));
                    XLS.HucreDegerYaz(satir, sutun + 5, uk.dovizCinsi);
                    XLS.HucreDegerYaz(satir, sutun + 6, Convert.ToDouble(uk.oncekiyilDoviz));
                    XLS.HucreDegerYaz(satir, sutun + 7, Convert.ToDouble(uk.oncekiyilAidat));
                    XLS.HucreDegerYaz(satir, sutun + 8, Convert.ToDouble(uk.hazirlikYilDoviz));
                    XLS.HucreDegerYaz(satir, sutun + 9, Convert.ToDouble(uk.hazirlikYilAidat));
                    XLS.HucreDegerYaz(satir, sutun + 10, Convert.ToDouble(uk.ertesiYilDoviz));
                    XLS.HucreDegerYaz(satir, sutun + 11, Convert.ToDouble(uk.ertesiYilAidat));
                    XLS.HucreDegerYaz(satir, sutun + 12, Convert.ToDouble(uk.ertesiYil2Doviz));
                    XLS.HucreDegerYaz(satir, sutun + 13, Convert.ToDouble(uk.ertesiYil2Aidat));
                    XLS.HucreDegerYaz(satir, sutun + 14, uk.aciklama);

                    dizi[0] += Convert.ToDouble(uk.toplantiSayisi);
                    dizi[1] += Convert.ToDouble(uk.toplantiKatsayisi);
                    dizi[2] += Convert.ToDouble(uk.oncekiyilDoviz);
                    dizi[3] += Convert.ToDouble(uk.oncekiyilAidat);
                    dizi[4] += Convert.ToDouble(uk.hazirlikYilDoviz);
                    dizi[5] += Convert.ToDouble(uk.hazirlikYilAidat);
                    dizi[6] += Convert.ToDouble(uk.ertesiYilDoviz);
                    dizi[7] += Convert.ToDouble(uk.ertesiYilAidat);
                    dizi[8] += Convert.ToDouble(uk.ertesiYil2Doviz);
                    dizi[9] += Convert.ToDouble(uk.ertesiYil2Aidat);


                    satir++;
                }

                XLS.HucreDegerYaz(satir, sutun, "TOPLAM");
                XLS.HucreDegerYaz(satir, sutun + 3, dizi[0]);
                XLS.HucreDegerYaz(satir, sutun + 4, dizi[1]);
                XLS.HucreDegerYaz(satir, sutun + 6, dizi[2]);
                XLS.HucreDegerYaz(satir, sutun + 7, dizi[3]);
                XLS.HucreDegerYaz(satir, sutun + 8, dizi[4]);
                XLS.HucreDegerYaz(satir, sutun + 9, dizi[5]);
                XLS.HucreDegerYaz(satir, sutun + 10, dizi[6]);
                XLS.HucreDegerYaz(satir, sutun + 11, dizi[7]);
                XLS.HucreDegerYaz(satir, sutun + 12, dizi[8]);
                XLS.HucreDegerYaz(satir, sutun + 13, dizi[9]);

                XLS.HucreBirlestir(satir, sutun, satir, sutun + 1);
                XLS.HucreDegerYaz(satir, sutun + 14, " ");
                XLS.YatayCizgiCiz(satir, sutun, sutun + 14, LineStyle.MEDIUM, TabloRenk.BLACK, false);

                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Hizmet alımı yoluyla edinilecek taşıt raporunu hazırlayan fonksiyon
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="p_SablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="p_SonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string EdinilecekTasitHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "EdinilecekTasitlar", null);
            try
            {
                TNS.BHF.EdinilecekTasitSatiri[] EdinilecekTasitSatirlari = servis.EdinilecekTasitlarListele(kullanan, yil, kurKod);
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaAc(sablonAd, sonucDosyaAd);
                int satir = 0;
                int sutun = 0;
                XLS.HucreAdBulYaz("ButceYil", yil);
                XLS.HucreAdBulYaz("KurumAd", kurumAd);
                XLS.HucreAdBulYaz("BirimAd", kurAd);

                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

                int kaynakSatir = satir;
                for (int i = 0; i < EdinilecekTasitSatirlari.Length; i++)
                {
                    TNS.BHF.EdinilecekTasitSatiri ets = EdinilecekTasitSatirlari[i];

                    XLS.HucreDegerYaz(satir + i, sutun + 0, "T" + ets.tasitSozluk.siraNo);
                    XLS.HucreDegerYaz(satir + i, sutun + 1, ets.tasitSozluk.cinsi);
                    XLS.HucreDegerYaz(satir + i, sutun + 2, ets.tasitSozluk.diffarensiyel);
                    XLS.HucreDegerYaz(satir + i, sutun + 3, ets.adet);
                    XLS.HucreDegerYaz(satir + i, sutun + 4, ets.kullanimYeri);
                    XLS.HucreDegerYaz(satir + i, sutun + 5, ets.finkaynak);

                    if (i != EdinilecekTasitSatirlari.Length - 1)
                    {
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, satir + i + 1, sutun);
                        XLS.SatirYukseklikAyarla(satir + i + 1, satir + i + 1, XLS.SatirYukseklikAl(kaynakSatir));
                        for (int j = 0; j < 7; j++)
                            XLS.HucreDegerYaz(satir + i + 1, sutun + j, "");
                    }
                }

                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Taşıt Sıra numarasına göre verilen Excel tablosunda değerleri arayarak, taşıt sıra numarası değerine sahip excel satır numarasını döndüren fonksiyon
        /// </summary>
        /// <param name="XLS">Excel dosyasını tutan Tablo türünde nesne</param>
        /// <param name="baslaSatir">Excel dosyasında raporlama için kullanılan Başlangıç satır değerini tutan Int türünde nesne</param>
        /// <param name="baslaSutun">Excel dosyasında raporlama için kullanılan Başlangıç sütun değerini tutan Int türünde nesne</param>
        /// <param name="tasitSiraNo">Taşıt sıra numarası değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalıştırılması Int türünde nesne döndürür</returns>
        private int HATSatirBul(Tablo XLS, int baslaSatir, int baslaSutun, string tasitSiraNo)
        {
            while (true)
            {
                string deger = XLS.HucreDegerAl(baslaSatir, baslaSutun);
                if (string.IsNullOrEmpty(deger))
                    return -1;
                if (deger.Equals(tasitSiraNo))
                    return baslaSatir;
                baslaSatir++;
            }
        }

        /// <summary>
        /// Taşıt sıra numarasınına göre Taşıt Genel Bilgisini (Mevcut taşıtlar, tasfiye edilecek taşıtlar, hizmet alımı ile kullanılacak taşıt sayısı ve birim maliyetinin gibi) döndüren fonksiyon
        /// </summary>
        /// <param name="tasitSiraNo">Taşıt Sıra Numarası değerini tutan String türünde nesne.</param>
        /// <param name="tasitBilgiDizi">Taşıt genel bilgileri dizisini tutan Object Array türünde nesne.</param>
        /// <returns>Verilen Taşıt Sıra numarasına göre Taşıt Genel Bilgi türünde nesne döndürür, taşıt sıra numarası bulunmazsa null döndürür</returns>
        private TasitGenelBilgi TasitGenelBilgiBul(string tasitSiraNo, ObjectArray tasitBilgiDizi)
        {
            for (int i = 0; i < tasitBilgiDizi.ObjeSayisi; i++)
            {
                TasitGenelBilgi tgb = (TasitGenelBilgi)tasitBilgiDizi[i];
                if (tgb.tasitSozluk.siraNo.Equals(tasitSiraNo))
                    return tgb;
            }
            return null;
        }

        /// <summary>
        /// Taşıt Sıra Numarasına göre Hizmet alımı ile edinilecek taşıtların detay bilgilerini (şoförlü/şoförsüz, giderli/gidersiz, üç yıllık bilgiler) döndüren fonksiyon 
        /// </summary>
        /// <param name="tasitSiraNo">Taşıt Sıra Numarası değerini tutan String türünde nesne</param>
        /// <param name="hat">Hizmet alımı ile edinelecek taşıtların detay bilgileri dizisini tutan Object Array türünde nesne</param>
        /// <returns>Verilen Taşıt Sıra numarasına göre Hizmet Alimi Taşıt Bilgisi Detay türünde nesne döndürür, taşıt sıra numarası bulunmazsa null döndürür</returns>
        private HizmetAlimiTasitBilgisiDetay HATHizmetAlimiTasitBilgiBulDetay(string tasitSiraNo, ObjectArray hat)
        {
            for (int i = 0; i < hat.ObjeSayisi; i++)
            {
                HizmetAlimiTasitBilgisiDetay hatb = (HizmetAlimiTasitBilgisiDetay)hat[i];
                if (hatb.tasitSozluk.siraNo.Equals(tasitSiraNo))
                    return hatb;
            }
            return null;
        }

        /// <summary>
        /// Taşıt Sıra Numarasına göre Hizmet alımı yoluyla kullanılan görevli bilgisini döndüren fonksiyon
        /// </summary>
        /// <param name="tasitSiraNo">Taşıt Sıra Numarası değerini tutan String türünde nesne</param>
        /// <param name="hat">Hizmet alımı yoluyla kullanılacak görevliler dizisini tutan Object Array türünde nesne</param>
        /// <returns>Verilen Taşıt Sıra numarasına göre Hizmet Alimi Görevli türünde nesne döndürür, taşıt sıra numarası bulunmazsa null döndürür</returns>
        private HizmetAlimiGorevli HATHizmetAlimiGorevliBul(string tasitSiraNo, ObjectArray hat)
        {
            for (int i = 0; i < hat.ObjeSayisi; i++)
            {
                HizmetAlimiGorevli hatb = (HizmetAlimiGorevli)hat[i];
                if (hatb.SiraNo.Equals(tasitSiraNo))
                    return hatb;
            }
            return null;
        }

        /// <summary>
        /// Temizlik hizmeti ihale bilgileri raporunu hazırlayan fonksiyon
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="p_SablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="p_SonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string TemizlikHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "Temizlik", null);

            TemizlikBilgi[] liste = servis.TemizlikMaliyetOku(kullanan, yil, kurKod, 0);

            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            int satir = 0;
            int sutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

            int butceYilSatir = 0;
            int butceYilSutun = 0;
            XLS.HucreAdAdresCoz("ButceYil", ref butceYilSatir, ref butceYilSutun);

            int kurAdSatir = 0;
            int kurAdSutun = 0;
            XLS.HucreAdAdresCoz("KurumAd", ref kurAdSatir, ref kurAdSutun);

            int tertipAdSatir = 0;
            int tertipAdSutun = 0;
            XLS.HucreAdAdresCoz("TertipAd", ref tertipAdSatir, ref tertipAdSutun);

            for (int i = 0; i < liste.Length - 1; i++)
            {
                XLS.YeniSheetEkle(0);
            }

            for (int i = 0; i < liste.Length; i++)
            {
                XLS.AktifSheetDegistir(i);

                XLS.HucreDegerYaz(kurAdSatir, kurAdSutun, kurAd);
                XLS.HucreDegerYaz(butceYilSatir, butceYilSutun, yil.ToString());

                XLS.HucreDegerYaz(satir, sutun, liste[i].hzmAlanOnceki);
                XLS.HucreDegerYaz(satir, sutun + 1, liste[i].hzmAlanBuYil);
                XLS.HucreDegerYaz(satir, sutun + 2, liste[i].hzmAlanSonraki);
                XLS.HucreDegerYaz(satir + 1, sutun, liste[i].gorSayisiOnceki);
                XLS.HucreDegerYaz(satir + 1, sutun + 1, liste[i].gorSayisiBuYil);
                XLS.HucreDegerYaz(satir + 1, sutun + 2, liste[i].gorSayisiSonraki);
                string basbitisOnceki = liste[i].basOnceki + "-" + liste[i].bitisOnceki;
                string basbitisBuYil = liste[i].basBuYil + "-" + liste[i].bitisBuYil;
                string basbitisSonraki = liste[i].basSonraki + "-" + liste[i].bitisSonraki;
                XLS.HucreDegerYaz(satir + 2, sutun, basbitisOnceki);
                XLS.HucreDegerYaz(satir + 2, sutun + 1, basbitisBuYil);
                XLS.HucreDegerYaz(satir + 2, sutun + 2, basbitisSonraki);

                TNSDateTime basOncekiTarih = new TNSDateTime(liste[i].basOnceki);
                TNSDateTime bitisOncekiTarih = new TNSDateTime(liste[i].bitisOnceki);

                TNSDateTime basBuYilTarih = new TNSDateTime(liste[i].basBuYil);
                TNSDateTime bitisBuYilTarih = new TNSDateTime(liste[i].bitisBuYil);

                TNSDateTime basSonrakiTarih = new TNSDateTime(liste[i].basSonraki);
                TNSDateTime bitisSonrakiTarih = new TNSDateTime(liste[i].bitisSonraki);

                XLS.HucreDegerYaz(satir + 3, sutun, bitisOncekiTarih.Ay - basOncekiTarih.Ay);
                XLS.HucreDegerYaz(satir + 3, sutun + 1, bitisBuYilTarih.Ay - basBuYilTarih.Ay);
                XLS.HucreDegerYaz(satir + 3, sutun + 2, bitisSonrakiTarih.Ay - basSonrakiTarih.Ay);

                for (int k = 1; k <= 10; k++)
                {
                    double harcama = 0;
                    double tahmin = 0;
                    double teklif = 0;
                    for (int j = 0; j < liste[i].maliyetler.Length; j++)
                    {

                        int iKod = OrtakFonksiyonlar.ConvertToInt(liste[i].maliyetler[j].kod, 0);
                        if (iKod == k)
                        {
                            harcama = liste[i].maliyetler[j].Harcama;
                            tahmin = liste[i].maliyetler[j].Tahmin;
                            teklif = liste[i].maliyetler[j].Teklif;
                            break;
                        }

                    }
                    int ekSatir = 0;
                    if (k == 8)
                        ekSatir = 1;
                    else if (k > 8)
                        ekSatir = 2;

                    XLS.HucreDegerYaz(satir + 5 + ekSatir + k, sutun, harcama);
                    XLS.HucreDegerYaz(satir + 5 + ekSatir + k, sutun + 1, tahmin);
                    XLS.HucreDegerYaz(satir + 5 + ekSatir + k, sutun + 2, teklif);
                }

                string s = "";
                Tertip t = new Tertip(yil, ENUMTabloKod.TABOdenek);
                for (int j = 0; j < liste[i].tertipler.Length; j++)
                {
                    GuvenlikTemizlikIhaleTertip tertip = liste[i].tertipler[j];

                    if (s != "")
                        s += ", ";
                    s += OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(0), tertip.kurKod) + "-";
                    s += OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(1), tertip.fonKod) + "-";
                    s += OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(2), tertip.finKod) + "-";
                    s += OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(3), tertip.ekoKod);
                }
                XLS.HucreDegerYaz(tertipAdSatir, tertipAdSutun, s);
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        /// <summary>
        /// Mevcut taşıtlar, tasfiye edilecek taşıtlar, hizmet alımı ile kullanılacak taşıt sayısı ve birim maliyeti raporunu hazırlayan fonksiyon
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="p_SablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="p_SonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string KullanilanTasitlarYazdir(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "MevcutTasitlar", null);
            string kurumKod = kurKod;
            TasitSozluk[] tasitSozlukler = servis.TasitSozlukListele(yil);
            ObjectArray hat = servis.KullanilanTasitlarListele(kullanan, yil, kurumKod, 3);

            if (hat.sonuc.islemSonuc)
            {
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaAc(sablonAd, sonucDosyaAd);
                int baslaSatir = 0;
                int baslaSutun = 0;
                XLS.HucreAdBulYaz("ButceYil", yil);
                XLS.HucreAdBulYaz("KurumAd", kurumAd);

                XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);

                for (int i = 0; i < tasitSozlukler.Length; i++)
                {
                    TasitSozluk tsf = tasitSozlukler[i];
                    int satir = HATSatirBul(XLS, baslaSatir, baslaSutun, tsf.siraNo);
                    if (satir > 0)
                    {

                        TasitGenelBilgi hatb = TasitGenelBilgiBul(tsf.siraNo, hat);
                        if (hatb != null)
                        {
                            XLS.HucreDegerYaz(satir, baslaSutun + 3, hatb.adet);
                            XLS.HucreDegerYaz(satir, baslaSutun + 4, hatb.adet);
                            XLS.HucreDegerYaz(satir, baslaSutun + 5, hatb.adet);
                            //XLS.HucreDegerYaz(satir, baslaSutun + 6, hatb.tasfiye2);
                            //XLS.HucreDegerYaz(satir, baslaSutun + 7, hatb.tasfiye1);
                            //XLS.HucreDegerYaz(satir, baslaSutun + 8, hatb.tasfiye0);
                            XLS.HucreDegerYaz(satir, baslaSutun + 7, hatb.adet);
                            XLS.HucreDegerYaz(satir, baslaSutun + 8, hatb.adet);
                            XLS.HucreDegerYaz(satir, baslaSutun + 9, hatb.adet);
                            //XLS.HucreDegerYaz(satir, baslaSutun + 12, hatb.birim2);
                            //XLS.HucreDegerYaz(satir, baslaSutun + 13, hatb.birim1);
                            //XLS.HucreDegerYaz(satir, baslaSutun + 14, hatb.birim0);
                        }
                        else
                        {
                            for (int j = 6; j <= 14; j++)
                            {
                                XLS.HucreDegerYaz(satir, baslaSutun + j, 0);
                            }
                        }
                    }
                }
                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                return "";
        }

        public string HizmetAlimiTasitlariDetayYaz(ObjectArray hat, string formAd)
        {
            if (hat.sonuc.islemSonuc)
            {
                TasitSozluk[] tasitSozlukler = servis.TasitSozlukListele(yil);
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaAc(sablonAd, sonucDosyaAd);
                int baslaSatir = 0;
                int baslaSutun = 0;
                XLS.HucreAdBulYaz("ButceYil", yil);
                XLS.HucreAdBulYaz("KurumAd", kurumAd);

                XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);

                XLS.HucreAdBulYaz("FormAd", formAd);
                for (int i = 0; i < tasitSozlukler.Length; i++)
                {
                    TasitSozluk tsf = tasitSozlukler[i];
                    int satir = HATSatirBul(XLS, baslaSatir, baslaSutun, tsf.siraNo);
                    if (satir > 0)
                    {

                        HizmetAlimiTasitBilgisiDetay hatb = HATHizmetAlimiTasitBilgiBulDetay(tsf.siraNo, hat);
                        if (hatb != null)
                        {
                            for (int basla = 0; basla <= 13; basla += 13)
                            {
                                int j = 0;
                                if (basla == 13) j = 1;
                                XLS.HucreDegerYaz(satir, baslaSutun + 3 + basla, hatb.soforluYakitGiderDahil[j].adet);
                                XLS.HucreDegerYaz(satir, baslaSutun + 4 + basla, hatb.soforluYakitGiderDahil[j].sure);
                                XLS.HucreDegerYaz(satir, baslaSutun + 5 + basla, hatb.soforluYakitGiderDahil[j].maliyet);

                                XLS.HucreDegerYaz(satir, baslaSutun + 6 + basla, hatb.soforluYakitGiderHaric[j].adet);
                                XLS.HucreDegerYaz(satir, baslaSutun + 7 + basla, hatb.soforluYakitGiderHaric[j].sure);
                                XLS.HucreDegerYaz(satir, baslaSutun + 8 + basla, hatb.soforluYakitGiderHaric[j].maliyet);

                                XLS.HucreDegerYaz(satir, baslaSutun + 9 + basla, hatb.soforsuzYakitGiderDahil[j].adet);
                                XLS.HucreDegerYaz(satir, baslaSutun + 10 + basla, hatb.soforsuzYakitGiderDahil[j].sure);
                                XLS.HucreDegerYaz(satir, baslaSutun + 11 + basla, hatb.soforsuzYakitGiderDahil[j].maliyet);

                                XLS.HucreDegerYaz(satir, baslaSutun + 12 + basla, hatb.soforsuzYakitGiderHaric[j].adet);
                                XLS.HucreDegerYaz(satir, baslaSutun + 13 + basla, hatb.soforsuzYakitGiderHaric[j].sure);
                                XLS.HucreDegerYaz(satir, baslaSutun + 14 + basla, hatb.soforsuzYakitGiderHaric[j].maliyet);
                            }
                        }
                        else
                        {
                            for (int j = 3; j <= 28; j++)
                            {
                                XLS.HucreDegerYaz(satir, baslaSutun + j, 0);
                            }
                        }
                    }
                }
                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new Exception(hat.sonuc.hataStr);
        }
        internal string KiralikAraclarListele(Kullanici o_kullanan, IBHFServis o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "HizmetAlimiTasitBilgi", null);
            KiralikAracSozlesmesi form = new KiralikAracSozlesmesi();
            form.yil = yil;
            form.saymanlikNo = OrtakFonksiyonlar.ConvertToInt((string)raporParam["saymanlikNo"], 0);
            form.birimKod = (string)raporParam["birimKod"];

            ObjectArray oa = servis.ListeleKiralikAracSozlesme(kullanan, form);
            if (oa.sonuc.islemSonuc)
            {
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.BosDosyaAc(sonucDosyaAd);
                int sat = 0;
                XLS.HucreDegerYaz(sat, 0, "Say Kod");
                XLS.HucreDegerYaz(sat, 1, "Say Ad");
                XLS.HucreDegerYaz(sat, 2, "Bir Kod");
                XLS.HucreDegerYaz(sat, 3, "Bir Ad");
                XLS.HucreDegerYaz(sat, 4, "Firma Kod");
                XLS.HucreDegerYaz(sat, 5, "Firma Ad");
                XLS.HucreDegerYaz(sat, 6, "Baş Tar.");
                XLS.HucreDegerYaz(sat, 7, "Bit Tar.");

                XLS.HucreDegerYaz(sat, 8, "T Kod");
                XLS.HucreDegerYaz(sat, 9, "Cinsi");
                XLS.HucreDegerYaz(sat, 10, "Tipi");
                XLS.HucreDegerYaz(sat, 11, "Şof. Dhl");
                XLS.HucreDegerYaz(sat, 12, "Yak. Dhl");
                XLS.HucreDegerYaz(sat, 13, "pebKod");
                XLS.HucreDegerYaz(sat, 14, "DÖSE");
                XLS.HucreDegerYaz(sat, 15, "adet");
                XLS.HucreDegerYaz(sat, 16, "Süre");
                XLS.HucreDegerYaz(sat, 17, "Birim");
                XLS.HucreDegerYaz(sat, 18, "Birim Fiyat");
                XLS.HucreDegerYaz(sat, 19, "Tutar");

                XLS.HucreDegerYaz(sat, 20, "Kaç Ay");
                XLS.HucreDegerYaz(sat, 21, "Ay Bedeli");
                sat++;
                foreach (KiralikAracSozlesmesi sozlesme in oa.objeler)
                {
                    foreach (KiralikArac arac in sozlesme.kiralikArac)
                    {
                        XLS.HucreDegerYaz(sat, 0, sozlesme.saymanlikNo);
                        XLS.HucreDegerYaz(sat, 1, sozlesme.muhasebeAd);
                        XLS.HucreDegerYaz(sat, 2, sozlesme.birimKod);
                        XLS.HucreDegerYaz(sat, 3, sozlesme.muhBirimAd);
                        XLS.HucreDegerYaz(sat, 4, sozlesme.firmaKod);
                        XLS.HucreDegerYaz(sat, 5, sozlesme.firmaAd);
                        XLS.HucreDegerYaz(sat, 6, sozlesme.isBaslangicTarihiDT);
                        XLS.HucreDegerYaz(sat, 7, sozlesme.isBitisTarihiDT);

                        XLS.HucreDegerYaz(sat, 8, arac.tasitTKod);
                        XLS.HucreDegerYaz(sat, 9, arac.tasitCinsiAd);
                        XLS.HucreDegerYaz(sat, 10, arac.tasitTipAd);
                        XLS.HucreDegerYaz(sat, 11, arac.soforDahilAd);
                        XLS.HucreDegerYaz(sat, 12, arac.yakitDahilAd);
                        XLS.HucreDegerYaz(sat, 13, arac.pebKod);
                        if (arac.dsMi)
                            XLS.HucreDegerYaz(sat, 14, "Evet");
                        XLS.HucreDegerYaz(sat, 15, arac.adet);
                        XLS.HucreDegerYaz(sat, 16, arac.sure);
                        XLS.HucreDegerYaz(sat, 17, arac.sureBirimiAd);
                        XLS.HucreDegerYaz(sat, 18, arac.birimFiyat);
                        XLS.HucreDegerYaz(sat, 19, arac.Tutar);

                        XLS.HucreDegerYaz(sat, 20, arac.tasitAy);
                        XLS.HucreDegerYaz(sat, 21, arac.birimFiyatAy);
                        sat++;
                    }
                }
                XLS.KoyuYap(0, 0, 0, 21, true);
                XLS.HucreFormatla(1, 6, sat, 7, "dd/mm/yyyy");
                XLS.HucreFormatla(1, 18, sat, 19, "#,##0.00");
                XLS.HucreFormatla(1, 21, sat, 21, "#,##0.00");

                XLS.SutunGenislikAyarla(0, 21, -1, 3, 100);

                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new Exception(oa.sonuc.hataStr);
        }

        public string HizmetAlimiTasitlariDetayHazirlaKiralik(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "HizmetAlimiTasitBilgi", null);
            int saymanlikNo = OrtakFonksiyonlar.ConvertToInt((string)raporParam["saymanlikNo"], 0);
            string birimKod = (string)raporParam["birimKod"];

            ObjectArray hat = servis.KiralikAracRaporla(kullanan, yil, saymanlikNo, birimKod);
            return HizmetAlimiTasitlariDetayYaz(hat, "Hizmet Alımı Suretiyle Kullanılan Taşıtlara İlişkin Bilgi Formu");
        }

        /// <summary>
        /// Hizmet alımı ile edinilecek taşıtların şoförlü/şoförsüz, yakıtlı/yakıtsız miktar ve maliyetleri raporunu hazırlayan fonksiyon
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="p_SablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="p_SonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string HizmetAlimiTasitlariDetayHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "HizmetAlimiTasitBilgi", null);
            string kullanilacak = (string)raporParam["Kullanilacak"];
            string kurumKod = kurKod;
            ObjectArray hat = servis.HizmetAlimiTasitlariDetayListele(kullanan, yil, kurumKod, kullanilacak == "0");
            return HizmetAlimiTasitlariDetayYaz(hat, kullanilacak == "0" ? "Hizmet Alımı Suretiyle Kullanılan Taşıtlara İlişkin Bilgi Formu" : "Hizmet Alımı Suretiyle Kullanılacak Taşıtlara İlişkin Bilgi Formu");
        }

        /// <summary>
        /// Hizmet alımı ile edinilecek görevlilerin hizmet türleri itibariyle miktar ve maliyetleri raporunu hazırlayan fonksiyon
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="p_SablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="p_SonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string HizmetAlimiGorevliHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "HizmetAlimiGorevli", null);
            string kurumKod = kurKod;
            ObjectArray hat = servis.HizmetAlimiGorevliListele(kullanan, yil, kurumKod);

            if (hat.sonuc.islemSonuc)
            {
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaAc(sablonAd, sonucDosyaAd);
                int baslaSatir = 0;
                int baslaSutun = 0;
                XLS.HucreAdBulYaz("ButceYil", yil);
                XLS.HucreAdBulYaz("KurumAd", kurumAd);

                XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);

                for (int i = 1; i < 16; i++)
                {
                    int satir = HATSatirBul(XLS, baslaSatir, baslaSutun, i.ToString());
                    if (satir > 0)
                    {

                        HizmetAlimiGorevli hatb = HATHizmetAlimiGorevliBul(i.ToString(), hat);
                        if (hatb != null)
                        {
                            XLS.HucreDegerYaz(satir, baslaSutun + 2, hatb.ozelGorevliTemizlikAdet2);
                            XLS.HucreDegerYaz(satir, baslaSutun + 3, hatb.ozelGorevliTemizlikSure2);
                            XLS.HucreDegerYaz(satir, baslaSutun + 4, hatb.ozelGorevliTemizlikMaliyet2);
                            XLS.HucreDegerYaz(satir, baslaSutun + 5, hatb.ozelGorevliGuvenlikAdet2);
                            XLS.HucreDegerYaz(satir, baslaSutun + 6, hatb.ozelGorevliGuvenlikSure2);
                            XLS.HucreDegerYaz(satir, baslaSutun + 7, hatb.ozelGorevliGuvenlikMaliyet2);
                            XLS.HucreDegerYaz(satir, baslaSutun + 8, hatb.ozelGorevliSoforAdet2);
                            XLS.HucreDegerYaz(satir, baslaSutun + 9, hatb.ozelGorevliSoforSure2);
                            XLS.HucreDegerYaz(satir, baslaSutun + 10, hatb.ozelGorevliSoforMaliyet2);
                            XLS.HucreDegerYaz(satir, baslaSutun + 11, hatb.ozelGorevliDigerAdet2);
                            XLS.HucreDegerYaz(satir, baslaSutun + 12, hatb.ozelGorevliDigerSure2);
                            XLS.HucreDegerYaz(satir, baslaSutun + 13, hatb.ozelGorevliDigerMaliyet2);

                            XLS.HucreDegerYaz(satir, baslaSutun + 16, hatb.ozelGorevliTemizlikAdet1);
                            XLS.HucreDegerYaz(satir, baslaSutun + 17, hatb.ozelGorevliTemizlikSure1);
                            XLS.HucreDegerYaz(satir, baslaSutun + 18, hatb.ozelGorevliTemizlikMaliyet1);
                            XLS.HucreDegerYaz(satir, baslaSutun + 19, hatb.ozelGorevliGuvenlikAdet1);
                            XLS.HucreDegerYaz(satir, baslaSutun + 20, hatb.ozelGorevliGuvenlikSure1);
                            XLS.HucreDegerYaz(satir, baslaSutun + 21, hatb.ozelGorevliGuvenlikMaliyet1);
                            XLS.HucreDegerYaz(satir, baslaSutun + 22, hatb.ozelGorevliSoforAdet1);
                            XLS.HucreDegerYaz(satir, baslaSutun + 23, hatb.ozelGorevliSoforSure1);
                            XLS.HucreDegerYaz(satir, baslaSutun + 24, hatb.ozelGorevliSoforMaliyet1);
                            XLS.HucreDegerYaz(satir, baslaSutun + 25, hatb.ozelGorevliDigerAdet1);
                            XLS.HucreDegerYaz(satir, baslaSutun + 26, hatb.ozelGorevliDigerSure1);
                            XLS.HucreDegerYaz(satir, baslaSutun + 27, hatb.ozelGorevliDigerMaliyet1);
                        }
                        else
                        {
                            for (int j = 2; j <= 27; j++)
                            {
                                XLS.HucreDegerYaz(satir, baslaSutun + j, 0);
                            }
                        }
                    }
                }
                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                return "";
        }

        /// <summary>
        /// Bir birimin sahip olduğu fiziksel değerlerin raporunu hazırlayan fonksiyon
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="p_SablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="p_SonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string FizikselDegerlerHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "FizikselDegerler", null);

            ObjectArray fdv = servis.FizikselDegerlerListele(kullanan, yil, kurKod);
            if (fdv.sonuc.islemSonuc)
            {
                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;
                int sutun = 0;
                XLS.DosyaAc(sablonAd, sonucDosyaAd);

                XLS.HucreAdBulYaz("ButceYil", yil);
                XLS.HucreAdBulYaz("KurumAd", kurAd);

                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                int kaynakSatir = satir;

                foreach (FizikselDeger fd in fdv.objeler)
                {
                    if (fd.kod.Length == 5 || fd.kod.Substring(0, 1) != "1")
                    {
                        foreach (FizikselDegerYilBilgi y in fd.degerler)
                        {
                            if (y.yil == yil - 3)
                                XLS.HucreDegerYaz(satir, sutun, y.deger);

                            if (y.yil == yil - 2)
                                XLS.HucreDegerYaz(satir, sutun + 1, y.deger);

                            if (y.yil == yil - 1)
                                XLS.HucreDegerYaz(satir, sutun + 2, y.deger);

                            if (y.yil == yil)
                                XLS.HucreDegerYaz(satir, sutun + 3, y.deger);
                        }
                    }
                    satir++;
                }

                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new Exception(fdv.sonuc.hataStr);
        }

        /// <summary>
        /// Döner sermaye ve fon bilgileri raporunu hazırlayan fonksiyon
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="p_SablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="p_SonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string FonGelirGiderHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            int dSermayeNo = OrtakFonksiyonlar.ConvertToInt((string)raporParam["DSermayeNo"] + "", 0);
            string defSablon;
            if (dSermayeNo == 0)
                defSablon = "FonGelirGider";
            else
                defSablon = "DonerSermaye";

            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, defSablon, null);

            DonerSermaye dsv = servis.DonerSermayeListele(kullanan, yil, kurKod, dSermayeNo);

            if (dsv.sonuc.islemSonuc)
            {
                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;
                int sutun = 0;
                XLS.DosyaAc(sablonAd, sonucDosyaAd);

                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

                int kaynakSatir = satir;

                XLS.HucreAdBulYaz("ButceYil", yil);
                XLS.HucreAdBulYaz("KurumAd", kurumAd);
                XLS.HucreAdBulYaz("DSermayeAdi", dsv.donerSermayeAd);

                for (int i = 0; i < dsv.donerSermayeSatirlari.Length; i++)
                {
                    DonerSermayeSatir dss = (DonerSermayeSatir)dsv.donerSermayeSatirlari[i];

                    foreach (DonerSermayeYil dsy in dss.degerler)
                    {
                        if (dsy.yil == yil - 2)
                            XLS.HucreDegerYaz(satir, sutun, dsy.deger);

                        if (dsy.yil == yil - 1)
                            if (dsy.odenekMi)
                                XLS.HucreDegerYaz(satir, sutun + 1, dsy.deger);
                            else
                                XLS.HucreDegerYaz(satir, sutun + 2, dsy.deger);

                        if (dsy.yil == yil)
                            XLS.HucreDegerYaz(satir, sutun + 3, dsy.deger);

                        if (dsy.yil == yil + 1)
                            XLS.HucreDegerYaz(satir, sutun + 4, dsy.deger);

                        if (dsy.yil == yil + 2)
                            XLS.HucreDegerYaz(satir, sutun + 5, dsy.deger);
                    }
                    satir++;
                }

                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new Exception(dsv.sonuc.hataStr);
        }


        /// <summary>
        /// Döner Sermaye/Fon raporunu hazırlayan fonksiyon
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="p_SablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="p_SonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string DSFonGelirGiderHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            string defSablon;
            defSablon = "DonerSermaye";
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, defSablon, null);

            int sheetsay = 0;

            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            for (int i = 1; i < 7; i++)
            {
                DonerSermaye donersermaye = servis.DonerSermayeListele(kullanan, yil, kurKod, i);

                if (donersermaye.donerSermayeAd.Trim() != "")
                {
                    XLS.AktifSheetDegistir(sheetsay);

                    int satir = 0;
                    int sutun = 0;
                    XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

                    int butceYilSatir = 0;
                    int butceYilSutun = 0;
                    XLS.HucreAdAdresCoz("ButceYil", ref butceYilSatir, ref butceYilSutun);

                    int kurumAdSatir = 0;
                    int kurumAdSutun = 0;
                    XLS.HucreAdAdresCoz("KurumAd", ref kurumAdSatir, ref kurumAdSutun);

                    int donerAdSatir = 0;
                    int donerAdSutun = 0;
                    XLS.HucreAdAdresCoz("DSermayeAdi", ref donerAdSatir, ref donerAdSutun);

                    XLS.HucreDegerYaz(butceYilSatir, butceYilSutun, yil.ToString());
                    XLS.HucreDegerYaz(kurumAdSatir, kurumAdSutun, kurAd);
                    XLS.HucreDegerYaz(donerAdSatir, donerAdSutun, donersermaye.donerSermayeAd);

                    int kaynakSatir = satir;
                    for (int k = 0; k < donersermaye.donerSermayeSatirlari.Length; k++)
                    {
                        DonerSermayeSatir dss = (DonerSermayeSatir)donersermaye.donerSermayeSatirlari[k];

                        foreach (DonerSermayeYil dsy in dss.degerler)
                        {
                            if (dsy.yil == yil - 2)
                                XLS.HucreDegerYaz(satir, sutun, dsy.deger);

                            if (dsy.yil == yil - 1)
                                if (dsy.odenekMi)
                                    XLS.HucreDegerYaz(satir, sutun + 1, dsy.deger);
                                else
                                    XLS.HucreDegerYaz(satir, sutun + 2, dsy.deger);

                            if (dsy.yil == yil)
                                XLS.HucreDegerYaz(satir, sutun + 3, dsy.deger);

                            if (dsy.yil == yil + 1)
                                XLS.HucreDegerYaz(satir, sutun + 4, dsy.deger);

                            if (dsy.yil == yil + 2)
                                XLS.HucreDegerYaz(satir, sutun + 5, dsy.deger);
                        }
                        satir++;
                    }
                    if (i < 6)
                    {
                        donersermaye = servis.DonerSermayeListele(kullanan, yil, kurKod, i + 1);
                        if (donersermaye.donerSermayeAd.Trim() != "")
                        {
                            XLS.YeniSheetEkle(0);
                            sheetsay++;
                            XLS.AktifSheetDegistir(sheetsay);
                        }
                    }
                }
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }


        /// <summary>
        /// Yeni yapılacak yada tamamlanacak hizmetler için, ekonomik kod ve miktarları raporunu hazırlayan fonksiyon
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="p_SablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="p_SonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string HizmetBilgisiEkoHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "form07", null);
            int tamamMiilkMi = 2;
            if (ekBilgi.Equals("F7"))
                tamamMiilkMi = 1;

            int satir = 0;
            int sutun = 0;
            int yilSatir = 0;
            int yilSutun = 0;
            int baslikSatir = 0;
            int baslikSutun = 0;
            int formIDSatir = 0;
            int formIDSutun = 0;

            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            XLS.HucreAdAdresCoz("ButceYil", ref yilSatir, ref yilSutun);
            XLS.HucreAdAdresCoz("Baslik2", ref baslikSatir, ref baslikSutun);
            XLS.HucreAdAdresCoz("FormID", ref formIDSatir, ref formIDSutun);

            int kaynakSatir = satir;
            XLS.AktifSheetDegistir(0);
            for (int i = 0; i < 2; i++)
            {
                XLS.YeniSheetEkle(0);
            }

            for (int icYil = 0; icYil < 3; icYil++)
            {
                XLS.AktifSheetDegistir(icYil);

                XLS.HucreDegerYaz(yilSatir, yilSutun, yil);
                XLS.HucreDegerYaz(yilSatir + 1, yilSutun, yil + icYil);
                XLS.HucreDegerYaz(yilSatir + 2, yilSutun, kurAd);


                if (tamamMiilkMi == 2)
                {
                    XLS.HucreDegerYaz(baslikSatir, baslikSutun, (yil + icYil).ToString() + " YILINDA İLK DEFA YAPILMASI PLANLANAN");
                    XLS.HucreDegerYaz(baslikSatir + 1, baslikSutun, "HİZMETLERİN GEREKTİRDİĞİ ÖDENEK TEKLİFLERİ");
                    XLS.HucreDegerYaz(formIDSatir, formIDSutun, "FORM : 6-" + (icYil + 1));
                }
                else
                {
                    XLS.HucreDegerYaz(baslikSatir, baslikSutun, (yil + icYil).ToString() + " YILINDA TAMAMLANMASI PLANLANAN");
                    XLS.HucreDegerYaz(baslikSatir + 1, baslikSutun, "HİZMETLERE İLİŞKİN BİLGİ FORMU");
                    XLS.HucreDegerYaz(formIDSatir, formIDSutun, "FORM : 7-" + (icYil + 1));
                }

                ObjectArray hbv = servis.HizmetBilgisiListele(kullanan, yil, kurKod, tamamMiilkMi, yil + icYil);

                if (hbv.sonuc.islemSonuc)
                {
                    double[] toplameko = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    double genelToplam = 0;
                    int siraNo = 1;

                    satir = kaynakSatir;

                    for (int i = 0; i < hbv.ObjeSayisi; i++)
                    {
                        XLS.HucreKopyala(satir, sutun, satir, sutun + 18, satir + 1, sutun);
                        double toplam = 0;

                        HizmetBilgisiSatiri hbs = (HizmetBilgisiSatiri)hbv[i];

                        XLS.HucreDegerYaz(satir, sutun, (siraNo++).ToString());
                        XLS.HucreDegerYaz(satir, sutun + 1, hbs.birimKod);
                        XLS.HucreDegerYaz(satir, sutun + 2, hbs.birimAd);
                        XLS.HucreDegerYaz(satir, sutun + 3, Convert.ToString(hbs.baslangicTarihi));
                        XLS.HucreDegerYaz(satir, sutun + 4, Convert.ToString(hbs.bitisTarihi));
                        XLS.HucreDegerYaz(satir, sutun + 5, hbs.hizmetAdi);
                        XLS.HucreDegerYaz(satir, sutun + 6, hbs.aciklama);

                        foreach (HizmetBilgiEko hbe in hbs.miktarlar)
                        {
                            int k = Convert.ToInt16(hbe.ekoKod);
                            if (hbe.miktar != 0)
                            {
                                XLS.HucreDegerYaz(satir, sutun + k + 6, hbe.miktar);
                                toplameko[k - 1] = toplameko[k - 1] + hbe.miktar;
                                toplam += hbe.miktar;
                            }
                            XLS.HucreDegerYaz(satir, sutun + 16, toplam);
                        }

                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 17, satir + 1, sutun);
                        XLS.SatirYukseklikAyarla(satir + 1, satir + 1, XLS.SatirYukseklikAl(satir));
                        for (int j = 0; j < 16; j++)
                            XLS.HucreDegerYaz(satir + 1, sutun + j, "");

                        satir++;
                    }

                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 6);
                    XLS.HucreDegerYaz(satir, sutun, "TOPLAM");
                    XLS.KoyuYap(satir, sutun, true);
                    XLS.YaziTipBuyuklugu(satir, sutun, 12);

                    for (int j = 0; j < 9; j++)
                    {
                        XLS.HucreDegerYaz(satir, sutun + j + 7, toplameko[j]);
                        genelToplam += toplameko[j];
                        XLS.KoyuYap(satir, sutun + j + 7, true);
                        XLS.YaziTipBuyuklugu(satir, sutun + j + 7, 11);
                    }

                    XLS.HucreDegerYaz(satir, sutun + 16, genelToplam);
                    XLS.KoyuYap(satir, sutun + 16, true);
                    XLS.YaziTipBuyuklugu(satir, sutun + 16, 11);
                }
                else
                    throw new Exception(hbv.sonuc.hataStr);
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        /// <summary>
        /// Birimlerin hizmetlerini gerçekleştirmek amacıyla yaptıkları işlemlerin raporunu hazırlayan fonksiyon
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="p_SablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="p_SonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string HizmetMaliyetiTespitiHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "BirimHizmetMaliyeti", null);

            ObjectArray hmv = servis.HizmetMaliyetiTesbitiListele(kullanan, yil, kurKod);

            if (hmv.sonuc.islemSonuc)
            {
                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;
                int sutun = 0;
                XLS.DosyaAc(sablonAd, sonucDosyaAd);

                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                int sayacDiger = 1;

                XLS.HucreAdBulYaz("ButceYil", yil);
                XLS.HucreAdBulYaz("KurumAd", kurumAd);
                XLS.HucreAdBulYaz("BirimAd", kurAd);

                int kaynakSatir = satir;

                for (int i = 0; i < hmv.ObjeSayisi; i++)
                {

                    HizmetMaliyetiTesbitiSatir hms = (HizmetMaliyetiTesbitiSatir)hmv[i];
                    if (hms.degerGirilsinMi)
                    {
                        if (hms.gercek > 0)
                            XLS.HucreDegerYaz(satir, sutun, hms.gercek);
                        if (hms.haziran > 0)
                            XLS.HucreDegerYaz(satir, sutun + 1, hms.haziran);
                        if (hms.tahmin > 0)
                            XLS.HucreDegerYaz(satir, sutun + 2, hms.tahmin);
                        if (hms.tahmin1 > 0)
                            XLS.HucreDegerYaz(satir, sutun + 3, hms.tahmin1);
                        if (hms.tahmin2 > 0)
                            XLS.HucreDegerYaz(satir, sutun + 4, hms.tahmin2);

                        if (hms.kod.StartsWith("6") && hms.kod.Length > 1)
                        {
                            XLS.HucreDegerYaz(satir, sutun - 1, hms.aciklama);
                            XLS.HucreDegerYaz(satir, sutun - 3, "      " + (sayacDiger++).ToString() + ".");
                        }
                    }
                    satir++;
                }

                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new Exception(hmv.sonuc.hataStr);
        }

        /// <summary>
        /// Taşranın hizmetlerini gerçekleştirmek amacıyla yaptıkları işlemlerin raporunu hazırlayan fonksiyon
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="p_SablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="p_SonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string HizmetMaliyetiTespitiHazirlaTasra(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            int saymanlik = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);
            string birim = (string)raporParam["Birim"];
            bool KurToplam = (string)raporParam["KurToplam"] == "1";
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "BirimHizmetMaliyeti", null);

            if (KurToplam)
            {
                saymanlik = 0;
                birim = "";
                birimAd = kurAd;
            }
            else
            {
                TNS.MUH.IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
                TNS.MUH.MuhasebeBirim mb = new TNS.MUH.MuhasebeBirim();
                mb.muhasebe = saymanlik;
                mb.birimKod = birim;

                ObjectArray oa = muhServis.MuhasebeBirimListele(mb);
                if (oa.sonuc.islemSonuc && oa.ObjeSayisi > 0)
                {
                    mb = (TNS.MUH.MuhasebeBirim)oa.objeler[0];
                    birimAd = mb.MuhasebeBirimAd;
                }
                else
                    birimAd = "";
            }
            ObjectArray hmv = servis.HizmetMaliyetiTasraListele(kullanan, yil, saymanlik, birim, kurKod, KurToplam);

            if (hmv.sonuc.islemSonuc)
            {
                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;
                int sutun = 0;
                XLS.DosyaAc(sablonAd, sonucDosyaAd);

                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                int sayacDiger = 1;

                XLS.HucreAdBulYaz("ButceYil", yil);
                XLS.HucreAdBulYaz("KurumAd", kurumAd);
                XLS.HucreAdBulYaz("BirimAd", birimAd);

                int kaynakSatir = satir;

                for (int i = 0; i < hmv.ObjeSayisi; i++)
                {
                    HizmetMaliyetiTesbitiSatir hms = (HizmetMaliyetiTesbitiSatir)hmv[i];
                    if (hms.degerGirilsinMi)
                    {
                        if (hms.gercek > 0)
                            XLS.HucreDegerYaz(satir, sutun, hms.gercek);
                        if (hms.haziran > 0)
                            XLS.HucreDegerYaz(satir, sutun + 1, hms.haziran);
                        if (hms.tahmin > 0)
                            XLS.HucreDegerYaz(satir, sutun + 2, hms.tahmin);
                        if (hms.tahmin1 > 0)
                            XLS.HucreDegerYaz(satir, sutun + 3, hms.tahmin1);
                        if (hms.tahmin2 > 0)
                            XLS.HucreDegerYaz(satir, sutun + 4, hms.tahmin2);

                        if (hms.kod.StartsWith("6") && hms.kod.Length > 1)
                        {
                            XLS.HucreDegerYaz(satir, sutun - 1, hms.aciklama);
                            XLS.HucreDegerYaz(satir, sutun - 3, "      " + (sayacDiger++).ToString() + ".");
                        }
                    }
                    satir++;
                }

                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new Exception(hmv.sonuc.hataStr);
        }

        /// <summary>
        /// Özel Güvenlik ihaleleriyle ilgili raporu hazırlayan fonksiyon
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="p_SablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="p_SonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string OzelGuvenlikHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "OzelGuvenlik", null);

            OzelGuvenlikBilgi[] liste = servis.OzelGuvenlikMaliyetOku(kullanan, yil, kurKod, 0);

            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            int satir = 0;
            int sutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

            int butceYilSatir = 0;
            int butceYilSutun = 0;
            XLS.HucreAdAdresCoz("ButceYil", ref butceYilSatir, ref butceYilSutun);

            int kurAdSatir = 0;
            int kurAdSutun = 0;
            XLS.HucreAdAdresCoz("KurumAd", ref kurAdSatir, ref kurAdSutun);

            int tertipAdSatir = 0;
            int tertipAdSutun = 0;
            XLS.HucreAdAdresCoz("TertipAd", ref tertipAdSatir, ref tertipAdSutun);

            for (int i = 0; i < liste.Length - 1; i++)
            {
                XLS.YeniSheetEkle(0);
            }

            for (int i = 0; i < liste.Length; i++)
            {
                XLS.AktifSheetDegistir(i);

                XLS.HucreDegerYaz(kurAdSatir, kurAdSutun, kurAd);
                XLS.HucreDegerYaz(butceYilSatir, butceYilSutun, yil.ToString());

                XLS.HucreDegerYaz(satir, sutun, liste[i].hzmAlanOnceki);
                XLS.HucreDegerYaz(satir, sutun + 1, liste[i].hzmAlanBuYil);
                XLS.HucreDegerYaz(satir, sutun + 2, liste[i].hzmAlanSonraki);
                XLS.HucreDegerYaz(satir + 1, sutun, liste[i].gorSayisiOnceki);
                XLS.HucreDegerYaz(satir + 1, sutun + 1, liste[i].gorSayisiBuYil);
                XLS.HucreDegerYaz(satir + 1, sutun + 2, liste[i].gorSayisiSonraki);
                string basbitisOnceki = liste[i].basOnceki + "-" + liste[i].bitisOnceki;
                string basbitisBuYil = liste[i].basBuYil + "-" + liste[i].bitisBuYil;
                string basbitisSonraki = liste[i].basSonraki + "-" + liste[i].bitisSonraki;
                XLS.HucreDegerYaz(satir + 2, sutun, basbitisOnceki);
                XLS.HucreDegerYaz(satir + 2, sutun + 1, basbitisBuYil);
                XLS.HucreDegerYaz(satir + 2, sutun + 2, basbitisSonraki);

                TNSDateTime basOncekiTarih = new TNSDateTime(liste[i].basOnceki);
                TNSDateTime bitisOncekiTarih = new TNSDateTime(liste[i].bitisOnceki);

                TNSDateTime basBuYilTarih = new TNSDateTime(liste[i].basBuYil);
                TNSDateTime bitisBuYilTarih = new TNSDateTime(liste[i].bitisBuYil);

                TNSDateTime basSonrakiTarih = new TNSDateTime(liste[i].basSonraki);
                TNSDateTime bitisSonrakiTarih = new TNSDateTime(liste[i].bitisSonraki);

                XLS.HucreDegerYaz(satir + 3, sutun, bitisOncekiTarih.Ay - basOncekiTarih.Ay);
                XLS.HucreDegerYaz(satir + 3, sutun + 1, bitisBuYilTarih.Ay - basBuYilTarih.Ay);
                XLS.HucreDegerYaz(satir + 3, sutun + 2, bitisSonrakiTarih.Ay - basSonrakiTarih.Ay);

                for (int k = 1; k <= 10; k++)
                {
                    double harcama = 0;
                    double tahmin = 0;
                    double teklif = 0;
                    for (int j = 0; j < liste[i].maliyetler.Length; j++)
                    {

                        int iKod = OrtakFonksiyonlar.ConvertToInt(liste[i].maliyetler[j].kod, 0);
                        if (iKod == k)
                        {
                            harcama = liste[i].maliyetler[j].Harcama;
                            tahmin = liste[i].maliyetler[j].Tahmin;
                            teklif = liste[i].maliyetler[j].Teklif;
                            break;
                        }

                    }
                    int ekSatir = 0;
                    if (k == 8)
                        ekSatir = 1;
                    else if (k > 8)
                        ekSatir = 2;

                    XLS.HucreDegerYaz(satir + 5 + ekSatir + k, sutun, harcama);
                    XLS.HucreDegerYaz(satir + 5 + ekSatir + k, sutun + 1, tahmin);
                    XLS.HucreDegerYaz(satir + 5 + ekSatir + k, sutun + 2, teklif);
                }

                string s = "";
                Tertip t = new Tertip(yil, ENUMTabloKod.TABOdenek);
                for (int j = 0; j < liste[i].tertipler.Length; j++)
                {
                    GuvenlikTemizlikIhaleTertip tertip = liste[i].tertipler[j];

                    if (s != "")
                        s += ", ";
                    s += OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(0), tertip.kurKod) + "-";
                    s += OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(1), tertip.fonKod) + "-";
                    s += OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(2), tertip.finKod) + "-";
                    s += OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(3), tertip.ekoKod);
                }
                XLS.HucreDegerYaz(tertipAdSatir, tertipAdSutun, s);
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        /// <summary>
        /// Tavan üstüne sığmayan ödenekler raporunu hazırlayan fonksiyon
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="p_SablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="p_SonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string TavanUstuOdenekHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "TavanUstuOdenek", null);
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
                throw new Exception(tsv.sonuc.hataStr);
        }

        /// <summary>
        /// Tavan üstüne sığmayan ödenekler raporunu hazırlayan fonksiyon
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="p_SablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="p_SonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string TavanUstuOdenek2Hazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "TavanUstuOdenek2", null);

            ObjectArray tsv;
            if (ekBilgi == "F25d")
                tsv = servis.TavanUstuListeleDetaydan(kullanan, yil, kurKod, senaryo, true);
            else
                tsv = servis.TavanUstuListeleDetay(kullanan, yil, kurKod);

            if (tsv.sonuc.islemSonuc)
            {
                Tablo XLS = GenelIslemler.NewTablo();
                int fsatir = 0;
                int fsutun = 0;
                int satir = 0;
                int sutun = 0;
                XLS.DosyaAc(sablonAd, sonucDosyaAd);
                XLS.HucreAdAdresCoz("FormatSatir", ref fsatir, ref fsutun);
                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

                XLS.HucreAdBulYaz("Yil", yil);
                XLS.HucreAdBulYaz("KurumAd", kurumAd);
                XLS.HucreAdBulYaz("KurumKod", kurumKod);
                IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
                int kaynakSatir = fsatir;
                ToplamAl toplamAl = new ToplamAl(2, 7);
                for (int i = 0; i < tsv.ObjeSayisi; i++)
                {
                    TNS.BHF.TavanUstuOdenek tss = (TNS.BHF.TavanUstuOdenek)tsv[i];
                    string[] ekoParca = OrtakFonksiyonlar.KodNoktaKoy("99.9.9.99", tss.ekokod).Split('.');
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun + 0, OrtakFonksiyonlar.DiziElemanAl(ekoParca, 0, ""));
                    XLS.HucreDegerYaz(satir, sutun + 1, OrtakFonksiyonlar.DiziElemanAl(ekoParca, 1, ""));
                    XLS.HucreDegerYaz(satir, sutun + 2, OrtakFonksiyonlar.DiziElemanAl(ekoParca, 2, ""));
                    XLS.HucreDegerYaz(satir, sutun + 3, OrtakFonksiyonlar.DiziElemanAl(ekoParca, 3, ""));
                    XLS.HucreDegerYaz(satir, sutun + 4, uzyServis.UzayDegeriStr(kullanan, "ABSEKO", yil.ToString() + "-" + tss.ekokod, true));
                    XLS.HucreDegerYaz(satir, sutun + 13, tss.aciklama);

                    toplamAl.DegerAta(1, 0, uzyServis.UzayDegeriDbl(kullanan, "ABSODENEK", TNS.UZY.Arac.UzayKosulOl((yil - 2).ToString(), "06.00", "0", kurKod, "", "", tss.ekokod), false));
                    toplamAl.DegerAta(1, 1, uzyServis.UzayDegeriDbl(kullanan, "ABSHARCAMA", TNS.UZY.Arac.UzayKosulOl((yil - 2).ToString(), "12", kurKod, "", "", tss.ekokod), false));
                    toplamAl.DegerAta(1, 2, uzyServis.UzayDegeriDbl(kullanan, "ABSODENEK", TNS.UZY.Arac.UzayKosulOl((yil - 1).ToString(), "06.00", "0", kurKod, "", "", tss.ekokod), false));
                    toplamAl.DegerAta(1, 3, uzyServis.UzayDegeriDbl(kullanan, "ABSHARCAMA", TNS.UZY.Arac.UzayKosulOl((yil - 1).ToString(), "6", kurKod, "", "", tss.ekokod), false));
                    toplamAl.DegerAta(1, 4, uzyServis.UzayDegeriDbl(kullanan, "ABSHARCAMATAHMIN", TNS.UZY.Arac.UzayKosulOl((yil - 1).ToString(), "00.00", kurKod, "", "", tss.ekokod), false));
                    toplamAl.DegerAta(1, 5, uzyServis.UzayDegeriDbl(kullanan, "ABSODENEK", TNS.UZY.Arac.UzayKosulOl((yil).ToString(), senaryo, "0", kurKod, "", "", tss.ekokod), false));
                    toplamAl.DegerAta(1, 6, tss.odenek);
                    toplamAl.ToplamEkle(0, 1);
                    TavanUstuOdenek2SatirYaz(XLS, satir, sutun, toplamAl, 1);
                    satir++;
                }

                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 14, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 4, "T O P L A M");
                TavanUstuOdenek2SatirYaz(XLS, satir, sutun, toplamAl, 0);

                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new Exception(tsv.sonuc.hataStr);
        }

        private void TavanUstuOdenek2SatirYaz(Tablo XLS, int satir, int sutun, ToplamAl toplamAl, int topSat)
        {
            XLS.HucreDegerYaz(satir, sutun + 5, toplamAl.toplam[topSat, 0]);
            XLS.HucreDegerYaz(satir, sutun + 6, toplamAl.toplam[topSat, 1]);

            XLS.HucreDegerYaz(satir, sutun + 7, toplamAl.toplam[topSat, 2]);
            XLS.HucreDegerYaz(satir, sutun + 8, toplamAl.toplam[topSat, 3]);
            XLS.HucreDegerYaz(satir, sutun + 9, toplamAl.toplam[topSat, 4]);

            XLS.HucreDegerYaz(satir, sutun + 10, toplamAl.toplam[topSat, 5]);
            XLS.HucreDegerYaz(satir, sutun + 11, toplamAl.toplam[topSat, 6]);
        }

        /// <summary>
        /// Hizmet (faaliyet) gerekçesi raporu (F1); eski HizmetGerekcesiHazirla'nın yerine geçer.
        /// Üretim kodu HizmetGerekcesiRaporu.cs'e taşınmıştır; burada yalnızca eskisi gibi parametreler raporParam'dan okunup oraya verilir.
        /// Eskisi gibi Word'ün açtığı HTML (.doc) dosyası üretilir.
        /// </summary>
        public string HizmetGerekcesiHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgiler)
        {
            DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "", "doc");
            int faalKod = OrtakFonksiyonlar.ConvertToInt(raporParam["FaaliyetKod"] + "", 0);
            return HizmetGerekcesiRaporu.HizmetGerekcesiHazirla(servis, kullanan, yil, kurKod, faalKod, kurumKod, p_SonucDosyaAd);
        }

        /// <summary>
        /// Ek bilgiler anahtar alanına göre ilgili rapor fonksiyonunu çağıran fonksiyon
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="sablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="sonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgiler">Ek Bilgiler değerini tutan String türünde nesne </param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgiler)
        {
            if (ekBilgiler == "F25" || ekBilgiler == "F25d")
            {
                return TavanUstuOdenekHazirla(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgiler);
            }
            else if (ekBilgiler == "F6" || ekBilgiler == "F7")
            {
                return HizmetBilgisiEkoHazirla(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgiler);
            }
            else if (ekBilgiler == "F22")
            {
                return TemizlikHazirla(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgiler);
            }
            else if (ekBilgiler == "F1")
            {
                return HizmetGerekcesiHazirla(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgiler);
            }
            else if (ekBilgiler == "F10")
            {
                return HizmetMaliyetiTespitiHazirla(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgiler);
            }
            else if (ekBilgiler == "F11")
            {
                return FizikselDegerlerHazirla(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgiler);
            }
            else if (ekBilgiler == "F17")
            {
                return UluslararasiKuruluslarHazirla(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgiler);
            }
            else if (ekBilgiler == "F18")
            {
                return EdinilecekTasitHazirla(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgiler);
            }
            else if (ekBilgiler == "F21")
            {
                return OzelGuvenlikHazirla(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgiler);
            }
            else if (ekBilgiler == "F23")
            {
                return DSFonGelirGiderHazirla(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgiler);
            }
            else if (ekBilgiler == "F24")
            {
                return FonGelirGiderHazirla(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgiler);
            }

            return "";
        }

        //public string IsciSayiUcretHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgiler)
        //{
        //    DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "form20", null);
        //    string isciGrup = (string)raporParam["IsciGrup"];
        //    IsciSayiUcretleriVAN isv = servis.IsciSayiUcretListele(kullanan, yil, kurKod, isciGrup);
        //    if (isciGrup.Equals("G"))
        //        isciGrup = "GEÇİCİ";
        //    else if (isciGrup.Equals("S"))
        //        isciGrup = "SÜREKLİ";
        //    else
        //        isciGrup = "SKS";
        //    if (isv.sonuc.islemSonuc)
        //    {
        //        Tablo XLS = GenelIslemler.NewTablo();
        //        int satir = 0;
        //        int sutun = 0;
        //        XLS.DosyaAc(sablonAd, sonucDosyaAd);

        //        XLS.HucreAdBulYaz("ButceYil", yil);
        //        XLS.HucreAdBulYaz("KurumAd", kurumAd);
        //        XLS.HucreAdBulYaz("TurAd", isciGrup);

        //        XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
        //        int kaynakSatir = satir;

        //        for (int i = 0; i < isv.isciSayiUcretleri.Count; i++)
        //        {
        //            IsciSayiUcreti isc = (IsciSayiUcreti)isv.isciSayiUcretleri[i];

        //            XLS.HucreDegerYaz(satir + i, sutun + 0, isc.grupAd);
        //            XLS.HucreDegerYaz(satir + i, sutun + 1, isc.grubdakiIsci);
        //            XLS.HucreDegerYaz(satir + i, sutun + 2, isc.ortalamaCiplak);
        //            XLS.HucreDegerYaz(satir + i, sutun + 3, isc.ortalamaGiyinik);
        //        }
        //        XLS.DosyaSaklaTamYol();
        //        return sonucDosyaAd;
        //    }
        //    else
        //        throw new Exception(isv.sonuc.hataStr);
        //}

        //public string InsanKaynaklariHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        //{
        //    DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "form12", null);
        //    IUZYServis uzyServis;
        //    if (o_servis.GetType() == typeof(IUZYServis))
        //        uzyServis = (IUZYServis)o_servis;
        //    else
        //        uzyServis = TNS.UZY.Arac.Tanimla();


        //    string kurumBirim = "";

        //    if (birimKod != "")
        //    {
        //        kurumBirim = birimKod;
        //        birimAd = birimAd + " - " + uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurumBirim, true);
        //    }
        //    else
        //    {
        //        kurumBirim = kurumKod;
        //        birimAd = "KURUM GENELİ";
        //    }

        //    InsanKaynaklariVAN ikv = servis.InsanKaynaklariListele(kullanan, yil, kurumBirim);

        //    if (ikv.sonuc.islemSonuc)
        //    {
        //        Tablo XLS = GenelIslemler.NewTablo();
        //        int satir = 0;
        //        int sutun = 0;
        //        XLS.DosyaAc(sablonAd, sonucDosyaAd);

        //        XLS.HucreAdBulYaz("ButceYil", yil);
        //        XLS.HucreAdBulYaz("KurumAd", kurumAd);
        //        XLS.HucreAdBulYaz("BirimAd", birimAd);

        //        XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
        //        int kaynakSatir = satir;

        //        foreach (InsanKaynak ik in ikv.insanKaynaklari)
        //        {
        //            if (!((ik.kod == "9") & (ik.kod.Length == 1)))
        //            {
        //                foreach (InsanKaynakDeger y in ik.degerler)
        //                {
        //                    if ((y.yil == yil - 3))
        //                        XLS.HucreDegerYaz(satir, sutun, y.deger);

        //                    if (y.yil == yil - 2)
        //                        XLS.HucreDegerYaz(satir, sutun + 1, y.deger);

        //                    if (y.yil == yil - 1)
        //                        XLS.HucreDegerYaz(satir, sutun + 2, y.deger);

        //                    if (y.yil == yil)
        //                        XLS.HucreDegerYaz(satir, sutun + 3, y.deger);

        //                    if (y.yil == yil + 1)
        //                        XLS.HucreDegerYaz(satir, sutun + 4, y.deger);

        //                    if (y.yil == yil + 2)
        //                        XLS.HucreDegerYaz(satir, sutun + 5, y.deger);
        //                }
        //            }
        //            satir++;
        //        }

        //        XLS.DosyaSaklaTamYol();
        //        return sonucDosyaAd;
        //    }
        //    else
        //        return "";
        //}

        //public string TopluIsSozlesmesiHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        //{
        //    DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "form21", null);

        //    ObjectArray tisv = servis.TopluIsSozlesmesiListele(kullanan, yil, kurumKod);

        //    if (tisv.sonuc.islemSonuc)
        //    {
        //        Tablo XLS = GenelIslemler.NewTablo();
        //        int satir = 0;
        //        int sutun = 0;
        //        XLS.DosyaAc(sablonAd, sonucDosyaAd);

        //        XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

        //        XLS.HucreAdBulYaz("ButceYil", yil);
        //        XLS.HucreAdBulYaz("KurumAd", kurumAd);
        //        XLS.HucreAdBulYaz("KurumKod", kurumKod);

        //        int kaynakSatir = satir;
        //        for (int i = 0; i < tisv.ObjeSayisi; i++)
        //        {
        //            TopluIsSozlesmesi tss = (TopluIsSozlesmesi)tisv[i];

        //            XLS.HucreDegerYaz(satir, sutun, tss.surekli);
        //            XLS.HucreDegerYaz(satir + 1, sutun, tss.gecici);

        //            XLS.HucreDegerYaz(satir, sutun + 1, tss.yururlukTarihi.ToString());
        //            XLS.HucreDegerYaz(satir, sutun + 2, "6772 (BKK)");
        //            XLS.HucreDegerYaz(satir, sutun + 3, tss.tarihNot30.ToString());
        //            XLS.HucreDegerYaz(satir, sutun + 4, "13 gün");

        //            XLS.HucreDegerYaz(satir + 1, sutun + 2, "Toplu İş Sözleşmesi");
        //            XLS.HucreDegerYaz(satir + 1, sutun + 3, tss.tarihNot13.ToString());
        //            XLS.HucreDegerYaz(satir + 1, sutun + 4, "30 gün");

        //            if (i != tisv.ObjeSayisi - 1)
        //            {
        //                XLS.HucreKopyala(kaynakSatir, sutun - 3, kaynakSatir + 1, sutun + 8, satir + 2, sutun - 3);
        //                XLS.SatirYukseklikAyarla(satir + 2, satir + 3, XLS.SatirYukseklikAl(satir));
        //            }

        //            XLS.HucreBirlestir(satir, sutun + 1, satir + 1, sutun + 1);
        //            satir += 2;
        //        }

        //        XLS.DosyaSaklaTamYol();
        //        return sonucDosyaAd;
        //    }
        //    else
        //        throw new Exception(tisv.sonuc.hataStr);
        //}

        //public string MevcutTasitHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        //{
        //    DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "form24", null);
        //    IBHMServis servisBHM = TNS.BHM.Arac.Tanimla();
        //    TNS.BHF.MevcutTasitlarVAN mtv = servis.MevcutTasitlarListele(kullanan, yil, kurumKod, Convert.ToInt16(birimKod));

        //    if (mtv.sonuc.islemSonuc)
        //    {
        //        Tablo XLS = GenelIslemler.NewTablo();
        //        int satir = 0;
        //        int sutun = 0;
        //        XLS.DosyaAc(sablonAd, sonucDosyaAd);


        //        XLS.HucreAdBulYaz("ButceYil", yil);
        //        XLS.HucreAdBulYaz("KurumAd", kurumAd);
        //        XLS.HucreAdBulYaz("BirimAdi", birimAd);

        //        XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

        //        int kaynakSatir = satir;
        //        for (int i = 0; i < mtv.mevcutTasit.Count; i++)
        //        {
        //            MevcutTasit mt = (MevcutTasit)mtv.mevcutTasit[i];
        //            satir = -1;
        //            for (int j = kaynakSatir; j < 32; j++)
        //            {
        //                if (XLS.HucreDegerAl(j, sutun).Equals("T" + mt.tasitSozluk.siraNo))
        //                {
        //                    satir = j;
        //                    break;
        //                }
        //            }
        //            if (satir > -1)
        //                XLS.HucreDegerYaz(satir, sutun + mt.finKayKod + 1, mt.deger);
        //        }

        //        XLS.DosyaSaklaTamYol();
        //        return sonucDosyaAd;
        //    }
        //    else
        //        return "";
        //}


        //public string HizmetAlimiTasitBilgiHazirla(object o_kullanan, object o_servis, string p_SablonAd, string p_SonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        //{
        //    string tur = (string)raporParam["Tur"] + "";

        //    DegerAl(o_kullanan, o_servis, raporParam, p_SonucDosyaAd, p_SablonAd, "HizmetAlimiTasitBilgi" + tur, null);

        //    Tablo XLS = GenelIslemler.NewTablo();
        //    XLS.DosyaAc(sablonAd, sonucDosyaAd);
        //    XLS.HucreAdBulYaz("ButceYil", yil);
        //    XLS.HucreAdBulYaz("KurumAd", kurumAd);
        //    int kodBaslaSatir = 0;
        //    int kodBaslaSutun = 0;
        //    XLS.HucreAdAdresCoz("KodBasla", ref kodBaslaSatir, ref kodBaslaSutun);
        //    if (kodBaslaSatir > 0)
        //    {
        //        for (int i = 0; i < 3; i++)
        //        {
        //            int yilBaslaSatir = 0;
        //            int yilBaslaSutun = 0;
        //            XLS.HucreAdAdresCoz("BaslaYil" + i.ToString(), ref yilBaslaSatir, ref yilBaslaSutun);
        //            if (yilBaslaSutun > 0)
        //            {
        //                HizmetAlimiTasitBilgi form = new HizmetAlimiTasitBilgi();
        //                form.butceYili = yil;
        //                form.kurKod = kurumKod;
        //                form.hizmetYili = yil - i;
        //                try
        //                {
        //                    form = servis.HizmetAlimiTasitBilgiOku(kullanan, form);
        //                    for (int j = 0; j < form.detaylar.Length; j++)
        //                    {
        //                        int yazSatir = -1;
        //                        for (int icSatir = kodBaslaSatir; true; icSatir++)
        //                        {
        //                            string tut = XLS.HucreDegerAl(icSatir, kodBaslaSutun);
        //                            if (tut == form.detaylar[j].tasitSiraNo)
        //                            {
        //                                yazSatir = icSatir;
        //                                break;
        //                            }
        //                            else if (String.IsNullOrEmpty(tut))
        //                                break;
        //                        }

        //                        if (yazSatir > 0)
        //                        {
        //                            if (form.detaylar[j].soforluYakitliSureAy != 0 || form.detaylar[j].soforluYakitliSureTutar != 0)
        //                            {
        //                                XLS.HucreDegerYaz(yazSatir, yilBaslaSutun + 0, form.detaylar[j].soforluYakitliSureAy);
        //                                XLS.HucreDegerYaz(yazSatir, yilBaslaSutun + 1, 1);
        //                                XLS.HucreDegerYaz(yazSatir, yilBaslaSutun + 2, form.detaylar[j].soforluYakitliSureTutar);
        //                            }

        //                            if (form.detaylar[j].soforluYakitsizSureAy != 0 || form.detaylar[j].soforluYakitsizSureTutar != 0)
        //                            {
        //                                XLS.HucreDegerYaz(yazSatir, yilBaslaSutun + 3, form.detaylar[j].soforluYakitsizSureAy);
        //                                XLS.HucreDegerYaz(yazSatir, yilBaslaSutun + 4, 1);
        //                                XLS.HucreDegerYaz(yazSatir, yilBaslaSutun + 5, form.detaylar[j].soforluYakitsizSureTutar);
        //                            }

        //                            if (form.detaylar[j].soforsuzYakitliSureAy != 0 || form.detaylar[j].soforsuzYakitliSureTutar != 0)
        //                            {
        //                                XLS.HucreDegerYaz(yazSatir, yilBaslaSutun + 6, form.detaylar[j].soforsuzYakitliSureAy);
        //                                XLS.HucreDegerYaz(yazSatir, yilBaslaSutun + 7, 1);
        //                                XLS.HucreDegerYaz(yazSatir, yilBaslaSutun + 8, form.detaylar[j].soforsuzYakitliSureTutar);
        //                            }

        //                            if (form.detaylar[j].soforsuzYakitsizSureAy != 0 || form.detaylar[j].soforsuzYakitsizSureTutar != 0)
        //                            {
        //                                XLS.HucreDegerYaz(yazSatir, yilBaslaSutun + 9, form.detaylar[j].soforsuzYakitsizSureAy);
        //                                XLS.HucreDegerYaz(yazSatir, yilBaslaSutun + 10, 1);
        //                                XLS.HucreDegerYaz(yazSatir, yilBaslaSutun + 11, form.detaylar[j].soforsuzYakitsizSureTutar);
        //                            }
        //                        }
        //                    }
        //                }
        //                catch (Exception exc)
        //                {
        //                    XLS.HucreDegerYaz(1, 1, exc.Message);
        //                }
        //            }
        //        }
        //    }
        //    XLS.DosyaSaklaTamYol();
        //    return sonucDosyaAd;
        //}

        //private int HATMevcutDegerBul(string tasitSiraNo, MevcutTasitlarVAN mtv, int butceTur)
        //{
        //    for (int i = 0; i < mtv.mevcutTasit.Count; i++)
        //    {
        //        MevcutTasit mt = (MevcutTasit)mtv.mevcutTasit[i];
        //        if (mt.tasitSozluk.siraNo.Equals(tasitSiraNo) && mt.finKayKod == butceTur)
        //            return mt.deger;
        //    }
        //    return 0;
        //}



    }
}
