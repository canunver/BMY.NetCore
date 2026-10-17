using System;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KHM;
using TNS.KYM;
using TNS.UZY;

// Eski Istemci/KesinHesap/ExcelRaporKesinHesapGelir.cs'ten alındı (Rapor.xml NESNE adları aynen korunmuştur); değişenler:
// - TNS.Raporlama.IRapor arayüzü Core'da yok: sınıflardan kaldırıldı (RaporAl imzası aynı, yansıma ile çağrılır).
// - Resources.KesinHesap.FRM017 yerine KesinHesap.Genel.FRM017 sabiti; throw new Exception yerine ButceHazirlikv2.RaporHatasi.
// - return "Hata" yerine "HATA: ..." metni; o_servis null olabileceği için "o_servis is IUZYServis" denetimi.
// - raporParam.Add("raporTur", ..) yerine raporParam["raporTur"] = .. (aynı Hashtable ile tekrar çağrılırsa hata vermesin).
// - Sunucu (KHM) metotları Core'da tipli TNSCollection<T> döndürür; tipsiz TNSCollection'a örtük dönüşümle eski kod aynen çalışır.
namespace KesinHesapGelir
{
    /// <summary>
    /// ExcelRaporGelirCetveliDinamik sınıfı
    /// </summary>
    public class ExcelRaporGelirCetveliDinamik
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="sablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="sonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            string raporTur = "dinamik";
            raporParam["raporTur"] = raporTur;
            return GelirCetveli.RaporAl(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);
        }
    }

    /// <summary>
    /// ExcelRaporGelirCetveliStatik sınıfı
    /// </summary>
    public class ExcelRaporGelirCetveliStatik
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="sablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="sonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            string raporTur = "Statik";
            raporParam["raporTur"] = raporTur;
            return GelirCetveli.RaporAl(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);
        }
    }

    /// <summary>
    /// ExcelRaporGelirForm43 sınıfı
    /// </summary>
    public class ExcelRaporGelirForm43
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="sablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="sonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            IKHMServis servisKHM = TNS.KHM.Arac.Tanimla();

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kirilim = 2;
            bool dinamik = true;
            Kullanici kullanan = (Kullanici)o_kullanan;

            IUZYServis uzyServis;
            if (o_servis is IUZYServis)
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            string ayAd = (string)raporParam["AyAd"];
            string kur = (string)raporParam["Kur"] + "";
            string eko = (string)raporParam["Eko"] + "";
            string tur = (string)raporParam["raporTur"] + "";
            if (tur == "dinamik")
                dinamik = true;

            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            //asama=4;
            int sutunSay = 18;

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABGelir);

            kistas.kodlar[0] = kur;
            kistas.kodlar[1] = eko;

            if (KesinHesap.Genel.GormeYetkisiVarMiKesinHesap(kullanan, kurum))
            {

                TNSCollection cevap = servisKHM.KesinHesapGelirCetveliHazirlaCokYilli(kullanan, yil, ay, kistas, dinamik, 2);

                if (cevap != null && cevap.Count > 0)
                {
                    if (cevap[0].GetType() == typeof(Sonuc))
                        throw new ButceHazirlikv2.RaporHatasi(((Sonuc)cevap[0]).hataStr);
                }

                if (kurum != "")
                {
                    string kurumEk = "";
                    if (kurum.Length == 2) kurumEk = "00";
                    XLS.HucreAdBulYaz("KurumAd", kurum + kurumEk + " - " + uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurum, true));
                }

                XLS.HucreAdBulYaz("ButceYil", yil);
                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                int ilkSutun = sutun;
                int[] koyuAcikYer = { satir - 2, satir - 1 };
                int ilkSatir = satir;

                string[] kodlar = new string[11];
                TNSCollection toplamlar = new TNSCollection();
                for (int i = 0; i < 15; i++)
                    toplamlar.Add((double)0);

                for (int k = 0; k < cevap.Count; k++)
                {
                    if (cevap[k].GetType() != typeof(UcgenSatir))
                        return "HATA: Sunucudan beklenmeyen kayıt tipi geldi.";

                    UcgenSatir us = (UcgenSatir)cevap[k];

                    sutun = ilkSutun;
                    kodlar[us.yer] = us.kodParca;

                    int koyuAcik;
                    if (us.yer == 0)
                        koyuAcik = 0;
                    else
                        koyuAcik = 1;

                    if (koyuAcikYer[koyuAcik] != satir)
                        XLS.HucreKopyala(koyuAcikYer[koyuAcik], sutun, koyuAcikYer[koyuAcik], sutun + sutunSay, satir, sutun);

                    for (int i = sutun; i < sutun + sutunSay; i++)
                        XLS.HucreDegerYaz(satir, i, "");

                    bool yazma = false;
                    if ((us.kodParca == "00") || (us.yer > 1))
                    {
                        yazma = true;
                        satir--;
                    }
                    // TertipGelirBilgi tertipBilgi = new TertipGelirBilgi();
                    int f = 0;
                    if (!yazma)
                    {

                        XLS.HucreDegerYaz(satir, us.yer, us.kodParca);
                        XLS.HucreDegerYaz(satir, sutun + kirilim, us.aciklama);

                        for (int i = 0; i < us.ekDegerler.Count; i++)
                        {
                            TertipGelirBilgi tertipBilgi = (TertipGelirBilgi)us.ekDegerler[i];
                            XLS.HucreDegerYaz(satir, sutun + kirilim + 1, tertipBilgi.tahmin);
                            XLS.HucreDegerYaz(satir, sutun + kirilim + 2, tertipBilgi.nettahakkuk);
                            XLS.HucreDegerYaz(satir, sutun + kirilim + 3, tertipBilgi.netTahsilat);
                            if (tertipBilgi.tahmin > 0)
                                XLS.HucreDegerYaz(satir, sutun + kirilim + 4, (tertipBilgi.netTahsilat / tertipBilgi.tahmin) * 100);
                            if (tertipBilgi.nettahakkuk > 0)
                                XLS.HucreDegerYaz(satir, sutun + kirilim + 5, (tertipBilgi.netTahsilat / tertipBilgi.nettahakkuk) * 100);
                            if (us.yer == 0)
                            {

                                toplamlar[f] = tertipBilgi.tahmin + (double)toplamlar[f];
                                toplamlar[f + 1] = tertipBilgi.nettahakkuk + (double)toplamlar[f + 1];
                                toplamlar[f + 2] = tertipBilgi.netTahsilat + (double)toplamlar[f + 2];
                                if ((double)toplamlar[f] > 0)
                                    toplamlar[f + 3] = ((double)toplamlar[f + 2] / (double)toplamlar[f]) * 100;
                                if ((double)toplamlar[f + 1] > 0)
                                    toplamlar[f + 4] = ((double)toplamlar[f + 2] / (double)toplamlar[f + 1]) * 100;

                            }
                            f = f + 5;
                            sutun = sutun + 5;
                        }


                        if ((satir - ilkSatir + 1) % 40 == 1 && (satir - ilkSatir + 1) > 10)
                        {
                            XLS.YatayCizgiCiz(satir - 1, ilkSutun, ilkSutun + sutunSay - 1, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK, false);
                            XLS.SayfaSonuKoy(satir);
                        }

                    }
                    satir++;
                }
                XLS.HucreKopyala(ilkSatir - 2, ilkSutun, ilkSatir - 2, ilkSutun + sutunSay, satir, ilkSutun);
                XLS.HucreBirlestir(satir, ilkSutun, satir, ilkSutun + 2);
                XLS.HucreDegerYaz(satir, ilkSutun, "GENEL TOPLAM");
                int l = 0;
                foreach (double d in toplamlar)
                    XLS.HucreDegerYaz(satir, ilkSutun + 3 + l++, d);



                XLS.YatayCizgiCiz(satir, ilkSutun, ilkSutun + sutunSay - 1, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK);
                XLS.YatayCizgiCiz(satir + 1, ilkSutun, ilkSutun + sutunSay - 1, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK);
                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new ButceHazirlikv2.RaporHatasi(KesinHesap.Genel.FRM017);
        }
    }

    /// <summary>
    /// ExcelRaporGelirForm42 sınıfı
    /// </summary>
    public class ExcelRaporGelirForm42
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="sablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="sonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            IKHMServis servisKHM = TNS.KHM.Arac.Tanimla();

            Tablo XLS = GenelIslemler.NewTablo();
            //int satir = 0;
            //int sutun = 0;
            //int kirilim = 2;
            //bool dinamik = true;
            Kullanici kullanan = (Kullanici)o_kullanan;

            IUZYServis uzyServis;
            if (o_servis is IUZYServis)
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            string ayAd = (string)raporParam["AyAd"];
            string kur = (string)raporParam["Kur"] + "";
            string eko = (string)raporParam["Eko"] + "";
            string tur = (string)raporParam["raporTur"] + "";
            //if (tur == "dinamik")
            //    dinamik = true;
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            if (kurum != "")
            {
                string kurumEk = "";
                if (kurum.Length == 2) kurumEk = "00";
                XLS.HucreAdBulYaz("KurumAd", kurum + kurumEk + " - " + uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurum, true));
            }
            XLS.HucreAdBulYaz("ButceYil", yil);

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABGelir);

            kistas.kodlar[0] = kur;
            kistas.kodlar[1] = eko;

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    /// <summary>
    /// GelirCetveli sınıfı
    /// </summary>
    public class GelirCetveli
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="sablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="sonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public static string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam)
        {
            IKHMServis servisKHM = TNS.KHM.Arac.Tanimla();

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int satir2 = 0;
            int sutun2 = 0;
            int kirilim = 4;
            bool dinamik = false;
            Kullanici kullanan = (Kullanici)o_kullanan;

            IUZYServis uzyServis;
            if (o_servis is IUZYServis)
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            string ayAd = (string)raporParam["AyAd"];
            string kur = (string)raporParam["Kur"] + "";
            string eko = (string)raporParam["Eko"] + "";
            string tur = (string)raporParam["raporTur"] + "";
            if (tur == "dinamik")
                dinamik = true;

            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            //asama=4;
            int sutunSay = 13;
            double oran = 0;

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABGelir);

            kistas.kodlar[0] = kur;
            kistas.kodlar[1] = eko;

            if (KesinHesap.Genel.GormeYetkisiVarMiKesinHesap(kullanan, kurum))
            {

                TNSCollection cevap = servisKHM.KesinHesapGelirCetveliHazirla(kullanan, yil, ay, kistas, dinamik);

                if (cevap != null && cevap.Count > 0)
                {
                    if (cevap[0].GetType() == typeof(Sonuc))
                        throw new ButceHazirlikv2.RaporHatasi(((Sonuc)cevap[0]).hataStr);
                }

                if (kurum != "")
                {
                    string kurumEk = "";
                    if (kurum.Length == 2) kurumEk = "00";
                    XLS.HucreAdBulYaz("KurumAd", kurum + kurumEk + " - " + uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurum, true));
                }

                XLS.HucreAdBulYaz("ButceYil", yil);


                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                satir2 = satir;
                sutun2 = sutun;
                int[] koyuAcikYer = { satir - 2, satir - 1 };
                int ilkSatir = satir;

                string[] kodlar = new string[11];
                TNSCollection toplamlar = new TNSCollection();
                toplamlar.Add((double)0);
                toplamlar.Add((double)0);
                toplamlar.Add((double)0);
                toplamlar.Add((double)0);
                toplamlar.Add((double)0);
                toplamlar.Add((double)0);
                toplamlar.Add((double)0);

                for (int k = 0; k < cevap.Count; k++)
                {
                    if (cevap[k].GetType() != typeof(UcgenSatir))
                        return "HATA: Sunucudan beklenmeyen kayıt tipi geldi.";

                    UcgenSatir us = (UcgenSatir)cevap[k];
                    TertipGelirBilgi tertipBilgi = (TertipGelirBilgi)us.ekDegerler[0];
                    kodlar[us.yer] = us.kodParca;

                    int koyuAcik;
                    if (us.yer == 0)
                    {
                        koyuAcik = 0;
                        toplamlar[0] = tertipBilgi.tahmin + (double)toplamlar[0];
                        toplamlar[1] = tertipBilgi.artik + (double)toplamlar[0];
                        toplamlar[2] = tertipBilgi.yiliTahakkuk + (double)toplamlar[0];
                        toplamlar[3] = tertipBilgi.rediade + (double)toplamlar[0];
                        toplamlar[4] = tertipBilgi.nettahakkuk + (double)toplamlar[0];
                        toplamlar[5] = tertipBilgi.netTahsilat + (double)toplamlar[0];
                        toplamlar[6] = tertipBilgi.devirArtik + (double)toplamlar[0];

                    }
                    else
                        koyuAcik = 1;

                    if (koyuAcikYer[koyuAcik] != satir)
                        XLS.HucreKopyala(koyuAcikYer[koyuAcik], sutun, koyuAcikYer[koyuAcik], sutun + sutunSay, satir, sutun);

                    for (int i = sutun; i < sutun + sutunSay; i++)
                        XLS.HucreDegerYaz(satir, i, "");


                    bool yazma = false;


                    if (us.yer != 3 && us.kodParca == "00")
                    {
                        yazma = true;
                        satir--;
                    }

                    if (!yazma)
                    {
                        XLS.HucreDegerYaz(satir, us.yer, us.kodParca);
                        XLS.HucreDegerYaz(satir, sutun + kirilim, us.aciklama);
                        XLS.HucreDegerYaz(satir, sutun + kirilim + 1, tertipBilgi.tahmin);
                        XLS.HucreDegerYaz(satir, sutun + kirilim + 2, tertipBilgi.artik);
                        XLS.HucreDegerYaz(satir, sutun + kirilim + 3, tertipBilgi.yiliTahakkuk);
                        XLS.HucreDegerYaz(satir, sutun + kirilim + 4, tertipBilgi.rediade);
                        XLS.HucreDegerYaz(satir, sutun + kirilim + 5, tertipBilgi.nettahakkuk);
                        XLS.HucreDegerYaz(satir, sutun + kirilim + 6, tertipBilgi.netTahsilat);
                        XLS.HucreDegerYaz(satir, sutun + kirilim + 7, tertipBilgi.devirArtik);
                        if (tertipBilgi.tahmin > 0)
                            oran = (tertipBilgi.netTahsilat / tertipBilgi.tahmin) * 100;
                        XLS.HucreDegerYaz(satir, sutun + kirilim + 8, oran);
                        if (us.yer < 2)
                        {
                            XLS.AktifSheetDegistir(1);
                            XLS.HucreKopyala(ilkSatir - 1, sutun, ilkSatir - 1, sutun + sutunSay - 6, satir2, sutun);
                            XLS.HucreDegerYaz(satir2, us.yer, us.kodParca);
                            XLS.HucreDegerYaz(satir2, sutun + kirilim, us.aciklama);
                            XLS.HucreDegerYaz(satir2, sutun + kirilim + 1, tertipBilgi.tahmin);
                            XLS.HucreDegerYaz(satir2, sutun + kirilim + 2, tertipBilgi.nettahakkuk);
                            XLS.HucreDegerYaz(satir2, sutun + kirilim + 3, tertipBilgi.netTahsilat);


                            if ((satir2 - ilkSatir + 1) % 40 == 1 && (satir2 - ilkSatir + 1) > 10)
                            {
                                XLS.YatayCizgiCiz(satir2 - 1, sutun, sutun + sutunSay - 6, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK, false);
                                XLS.SayfaSonuKoy(satir2);
                            }
                            XLS.AktifSheetDegistir(0);

                            satir2++;
                        }

                        if ((satir - ilkSatir + 1) % 40 == 1 && (satir - ilkSatir + 1) > 10)
                        {
                            XLS.YatayCizgiCiz(satir - 1, sutun, sutun + sutunSay - 1, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK, false);
                            XLS.SayfaSonuKoy(satir);
                        }

                    }
                    satir++;
                }
                XLS.HucreKopyala(satir - 1, sutun, satir - 1, sutun + sutunSay, satir, sutun);
                XLS.HucreBirlestir(satir, sutun, satir, sutun + 4);
                XLS.HucreDegerYaz(satir, sutun, "GENEL TOPLAM");
                int j = 0;
                foreach (double d in toplamlar)
                    XLS.HucreDegerYaz(satir, sutun + 5 + j++, d);

                XLS.YatayCizgiCiz(satir, sutun, sutun + sutunSay - 1, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK);
                XLS.YatayCizgiCiz(satir + 1, sutun, sutun + sutunSay - 1, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK);


                XLS.AktifSheetDegistir(1);

                XLS.HucreKopyala(ilkSatir, sutun, ilkSatir, sutun + sutunSay - 6, satir2, sutun);
                XLS.HucreBirlestir(satir2, sutun, satir2, sutun + 4);
                XLS.HucreDegerYaz(satir2, sutun, "GENEL TOPLAM");
                XLS.HucreDegerYaz(satir2, sutun + 5, (double)toplamlar[0]);
                XLS.HucreDegerYaz(satir2, sutun + 6, (double)toplamlar[4]);
                XLS.HucreDegerYaz(satir2, sutun + 7, (double)toplamlar[5]);

                XLS.YatayCizgiCiz(satir2, sutun, sutun + sutunSay - 6, OrtakClass.LineStyle.THIN, OrtakClass.TabloRenk.BLACK);
                XLS.YatayCizgiCiz(satir2 + 1, sutun, sutun + sutunSay - 6, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK);


                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new ButceHazirlikv2.RaporHatasi(KesinHesap.Genel.FRM017);
        }
    }

    /// <summary>
    /// ExcelRaporKesinMizan sınıfı
    /// </summary>
    public class ExcelRaporKesinMizan
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="sablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="sonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            string raporTur = "kesin";
            raporParam["raporTur"] = raporTur;
            return ExcelRaporMizan.RaporAl(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);
        }
    }

    /// <summary>
    /// ExcelRaporGeciciMizan sınıfı
    /// </summary>
    public class ExcelRaporGeciciMizan
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="sablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="sonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            string raporTur = "gecici";
            raporParam["raporTur"] = raporTur;
            return ExcelRaporMizan.RaporAl(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);
        }
    }

    /// <summary>
    /// ExcelRaporMizan sınıfı
    /// </summary>
    public class ExcelRaporMizan
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o_kullanan">Kullanıcı değerini tutan Object türünde nesne</param>
        /// <param name="o_servis">Raporlarda kullanılacak Servis değerini tutan Object türünde nesne</param>
        /// <param name="sablonAd">Rapor için kullanılacak Excel Şablon adını tutan String türünde nesne</param>
        /// <param name="sonucDosyaAd">Raporun üretildikten sonra oluşturulacak Excel dosya adını tutan String türünde nesne</param>
        /// <param name="ciktiYeri">Raporun kaydedileceği dizin yerini tutan String türünde nesne</param>
        /// <param name="raporParam">Raporda kullanılacak Parametre değerini tutan HashTable türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public static string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam)
        {

            IKHMServis servisKHM = TNS.KHM.Arac.Tanimla();

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kirilim = 2;

            Kullanici kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            string ayAd = (string)raporParam["AyAd"];
            string kur = (string)raporParam["Kur"] + "";
            string raporTur = (string)raporParam["raporTur"];

            bool gecici = false;
            if (raporTur == "gecici")
                gecici = true;

            IUZYServis uzyServis;
            if (o_servis is IUZYServis)
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            //asama=4;
            int sutunSay = 8;

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            if (KesinHesap.Genel.GormeYetkisiVarMiKesinHesap(kullanan, kurum))
            {

                TNSCollection cevap = servisKHM.KesinHesapMizanHazirla(kullanan, yil, kurum, gecici);

                if (cevap != null && cevap.Count > 0)
                {
                    if (cevap[0].GetType() == typeof(Sonuc))
                        throw new ButceHazirlikv2.RaporHatasi(((Sonuc)cevap[0]).hataStr);
                }

                if (kurum != "")
                {
                    string kurumEk = "";
                    if (kurum.Length == 2) kurumEk = "00";
                    XLS.HucreAdBulYaz("KurumAd", kurum + kurumEk + " - " + uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurum, true));
                }

                XLS.HucreAdBulYaz("ButceYil", yil);
                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                int[] koyuAcikYer = { satir - 2, satir - 1 };

                int ilkSatir = satir;

                for (int k = 0; k < cevap.Count; k++)
                {
                    if (cevap[k].GetType() != typeof(UcgenSatir))
                        return "HATA: Sunucudan beklenmeyen kayıt tipi geldi.";

                    UcgenSatir us = (UcgenSatir)cevap[k];
                    MizanBilgi mizan = (MizanBilgi)us.ekDegerler[0];

                    int koyuAcik;
                    if (us.yer == 0)
                        koyuAcik = 0;
                    else
                        koyuAcik = 1;

                    if (koyuAcikYer[koyuAcik] != satir)
                        XLS.HucreKopyala(koyuAcikYer[koyuAcik], sutun, koyuAcikYer[koyuAcik], sutun + sutunSay, satir, sutun);

                    for (int i = sutun; i < sutun + sutunSay; i++)
                        XLS.HucreDegerYaz(satir, i, "");

                    bool yazma = false;

                    if (us.yer == 0 && us.aciklama == "")
                    {
                        yazma = true;
                        satir--;
                    }


                    if (!yazma)
                    {
                        XLS.HucreDegerYaz(satir, us.yer, us.kodParca);
                        XLS.HucreDegerYaz(satir, kirilim, us.aciklama);
                        XLS.HucreDegerYaz(satir, kirilim + 1, mizan.borc);
                        XLS.HucreDegerYaz(satir, kirilim + 2, mizan.alacak);
                        XLS.HucreDegerYaz(satir, kirilim + 3, mizan.borc_artik);
                        XLS.HucreDegerYaz(satir, kirilim + 4, mizan.alacak_artik);

                        if ((satir - ilkSatir + 1) % 46 == 1 && (satir - ilkSatir + 1) > 10)
                        {
                            XLS.YatayCizgiCiz(satir - 1, sutun, sutun + sutunSay - 1, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK, false);
                            XLS.SayfaSonuKoy(satir);
                        }
                    }
                    satir++;
                }
                for (int i = sutun; i < sutun + sutunSay; i++)
                    XLS.HucreDegerYaz(satir, i, "");

                XLS.YatayCizgiCiz(satir, sutun, sutun + sutunSay - 1, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK);


                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new ButceHazirlikv2.RaporHatasi(KesinHesap.Genel.FRM017);

        }
    }
}
