using System;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KHM;
using TNS.KYM;
using TNS.UZY;

// Eski Istemci/KesinHesap/ExcelRaporKesinHesapGider.cs'ten alındı (Rapor.xml NESNE adları aynen korunmuştur); değişenler:
// - TNS.Raporlama.IRapor arayüzü Core'da yok: sınıflardan kaldırıldı (RaporAl imzası aynı, yansıma ile çağrılır).
// - Resources.KesinHesap.FRM017 yerine KesinHesap.Genel.FRM017 sabiti; throw new Exception yerine ButceHazirlikv2.RaporHatasi.
// - return "Hata" yerine "HATA: ..." metni; o_servis null olabileceği için "o_servis is IUZYServis" denetimi.
// - raporParam.Add("raporTur", ..) yerine raporParam["raporTur"] = .. (aynı Hashtable ile tekrar çağrılırsa hata vermesin).
// - Sunucu (KHM) metotları Core'da tipli TNSCollection<T> döndürür; tipsiz TNSCollection'a örtük dönüşümle eski kod aynen çalışır.
namespace KesinHesapGider
{
    /// <summary>
    /// ExcelRaporGiderForm3 sınıfı
    /// </summary>
    public class ExcelRaporGiderForm3
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
            Kullanici kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            string ayAd = (string)raporParam["AyAd"];
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";

            IUZYServis uzyServis;
            if (o_servis is IUZYServis)
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);
            //asama=4;
            int sutunSay = 29;

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            Tertip kistas = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABButce);

            kistas.kodlar[0] = kur;
            kistas.kodlar[1] = fon;
            kistas.kodlar[2] = fin;
            kistas.kodlar[3] = eko;

            if (KesinHesap.Genel.GormeYetkisiVarMiKesinHesap(kullanan, kurum))
            {

                TNSCollection cevap = servisKHM.KesinHesapGiderCetveliHazirla(kullanan, yil, ay, kistas);

                if (cevap != null && cevap.Count > 0)
                {
                    if (cevap[0].GetType() == typeof(Sonuc))
                        throw new ButceHazirlikv2.RaporHatasi(((Sonuc)cevap[0]).hataStr);
                }

                if (kurum != "")
                {
                    string kurumEk = "";
                    if (kurum.Length == 2) kurumEk = "00";
                    XLS.HucreAdBulYaz("KurumAd", kurum + kurumEk + "  " + uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurum, true));
                }

                XLS.HucreAdBulYaz("ButceYil", yil);
                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                int[] koyuAcikYer = { satir - 2, satir - 1 };
                int ilkSatir = satir;
                double ysot = 0.0, eklenen = 0.0, dusulen = 0.0, devir1 = 0.0, devir2 = 0.0;
                string[] kodlar = new string[11];
                string[,] oncekiBilgileri = new string[11, 21];

                bool sayfaBasi = false;
                for (int k = 0; k < cevap.Count; k++)
                {
                    if (cevap[k].GetType() != typeof(UcgenSatir))
                        return "HATA: Sunucudan beklenmeyen kayıt tipi geldi.";

                    UcgenSatir us = (UcgenSatir)cevap[k];
                    KHTertipBilgi tertipBilgi = (KHTertipBilgi)us.ekDegerler[0];

                    bool yazma = false;

                    //if (us.kodParca == "00")
                    //{
                    //    yazma = true;
                    //    satir--;
                    //}

                    eklenen = tertipBilgi.kiciEk + tertipBilgi.karaEk + tertipBilgi.yedekEk + tertipBilgi.odenekEkle + tertipBilgi.ozelOdenek;
                    dusulen = tertipBilgi.kiciDus + tertipBilgi.karaDus + tertipBilgi.yedekDus + tertipBilgi.yiliciIptal;
                    devir1 = tertipBilgi.akreditifdevir + tertipBilgi.taahhutdevir + tertipBilgi.disprojedevir + tertipBilgi.digerdevir;
                    devir2 = tertipBilgi.bagisdevir + tertipBilgi.ozeldevir;
                    ysot = tertipBilgi.kbo + eklenen - dusulen + devir1 + devir2;

                    double yazmaKontrol = eklenen + dusulen + devir1 + devir2 + ysot + tertipBilgi.devir + tertipBilgi.kbo + tertipBilgi.yilSonuIptal + tertipBilgi.odenekUstu + tertipBilgi.harcama;
                    if (yazmaKontrol == 0)
                        yazma = true;

                    int koyuAcik;
                    if (!yazma)
                    {
                        if (sayfaBasi)
                        {
                            sayfaBasi = false;
                            for (int eskiSatir = 0; eskiSatir < us.yer; eskiSatir++)
                            {
                                if (eskiSatir <= 9)
                                    koyuAcik = 0;
                                else
                                    koyuAcik = 1;

                                if (koyuAcikYer[koyuAcik] != satir)
                                    XLS.HucreKopyala(koyuAcikYer[koyuAcik], sutun, koyuAcikYer[koyuAcik], sutun + sutunSay, satir, sutun);

                                for (int i = sutun; i < sutun + sutunSay; i++)
                                    XLS.HucreDegerYaz(satir, i, "");

                                for (int eskiSutun = 0; eskiSutun < oncekiBilgileri.GetUpperBound(1); eskiSutun++)
                                {
                                    if (eskiSutun == 0)
                                        XLS.HucreDegerYaz(satir, eskiSatir, oncekiBilgileri[eskiSatir, eskiSutun]);
                                    else if (eskiSutun == 1)
                                        XLS.HucreDegerYaz(satir, eskiSutun + 8 + kirilim, oncekiBilgileri[eskiSatir, eskiSutun]);
                                    else if (eskiSutun > 1)
                                        XLS.HucreDegerYaz(satir, eskiSutun + 8 + kirilim, OrtakFonksiyonlar.ConvertToDbl(oncekiBilgileri[eskiSatir, eskiSutun]));
                                }
                                satir++;
                            }

                        }

                        kodlar[us.yer] = us.kodParca;
                        if (us.yer <= 9)
                            koyuAcik = 0;
                        else
                            koyuAcik = 1;

                        if (koyuAcikYer[koyuAcik] != satir)
                            XLS.HucreKopyala(koyuAcikYer[koyuAcik], sutun, koyuAcikYer[koyuAcik], sutun + sutunSay, satir, sutun);

                        for (int i = sutun; i < sutun + sutunSay; i++)
                            XLS.HucreDegerYaz(satir, i, "");

                        if (us.yer == 1)
                        {
                            if (Convert.ToInt16(kodlar[0]) >= 30 || Convert.ToInt16(us.kodParca) >= 75)
                                satir--;
                        }

                        XLS.HucreDegerYaz(satir, us.yer, us.kodParca);
                        XLS.HucreDegerYaz(satir, 9 + kirilim, us.aciklama);

                        XLS.HucreDegerYaz(satir, 10 + kirilim, tertipBilgi.bo);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 1, tertipBilgi.bo - tertipBilgi.kbo);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 2, devir1);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 3, tertipBilgi.odenekEkle);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 4, tertipBilgi.karaEk);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 5, tertipBilgi.karaDus);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 6, tertipBilgi.kiciEk);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 7, tertipBilgi.kiciDus);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 8, tertipBilgi.yedekEk);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 9, tertipBilgi.yedekDus);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 10, devir2);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 11, tertipBilgi.ozelOdenek);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 12, tertipBilgi.yiliciIptal);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 13, ysot);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 14, tertipBilgi.harcama);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 15, tertipBilgi.odenekUstu);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 16, tertipBilgi.yilSonuIptal);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 17, tertipBilgi.devir);
                        if ((satir - ilkSatir + 1) % 42 == 0 && (satir - ilkSatir + 1) > 10)
                        {
                            XLS.YatayCizgiCiz(satir, sutun, sutun + sutunSay, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK, false);
                            XLS.SayfaSonuKoy(satir + 1);
                            sayfaBasi = true;
                        }

                        oncekiBilgileri[us.yer, 0] = us.kodParca;
                        oncekiBilgileri[us.yer, 1] = us.aciklama;
                        oncekiBilgileri[us.yer, 2] = tertipBilgi.bo.ToString();
                        oncekiBilgileri[us.yer, 3] = (tertipBilgi.bo - tertipBilgi.kbo).ToString();
                        oncekiBilgileri[us.yer, 4] = devir1.ToString();
                        oncekiBilgileri[us.yer, 5] = tertipBilgi.odenekEkle.ToString();
                        oncekiBilgileri[us.yer, 6] = tertipBilgi.karaEk.ToString();
                        oncekiBilgileri[us.yer, 7] = tertipBilgi.karaDus.ToString();
                        oncekiBilgileri[us.yer, 8] = tertipBilgi.kiciEk.ToString();
                        oncekiBilgileri[us.yer, 9] = tertipBilgi.kiciDus.ToString();
                        oncekiBilgileri[us.yer, 10] = tertipBilgi.yedekEk.ToString();
                        oncekiBilgileri[us.yer, 11] = tertipBilgi.yedekDus.ToString();
                        oncekiBilgileri[us.yer, 12] = devir2.ToString();
                        oncekiBilgileri[us.yer, 13] = tertipBilgi.ozelOdenek.ToString();
                        oncekiBilgileri[us.yer, 14] = tertipBilgi.yiliciIptal.ToString();
                        oncekiBilgileri[us.yer, 15] = ysot.ToString();
                        oncekiBilgileri[us.yer, 16] = tertipBilgi.harcama.ToString();
                        oncekiBilgileri[us.yer, 17] = tertipBilgi.odenekUstu.ToString();
                        oncekiBilgileri[us.yer, 18] = tertipBilgi.yilSonuIptal.ToString();
                        oncekiBilgileri[us.yer, 19] = tertipBilgi.devir.ToString();

                        satir++;
                    }
                }
                for (int i = sutun; i < sutun + sutunSay; i++)
                    XLS.HucreDegerYaz(satir, i, "");

                XLS.YatayCizgiCiz(satir, sutun, sutun + sutunSay, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK);


                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new ButceHazirlikv2.RaporHatasi(KesinHesap.Genel.FRM017);
        }
    }

    /// <summary>
    /// ExcelRaporGiderForm31 sınıfı
    /// </summary>
    public class ExcelRaporGiderForm31
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
            raporParam["raporTur"] = "FON";
            return ExcelRaporEkoFonKod.RaporAl(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);
        }
    }

    /// <summary>
    /// ExcelRaporGiderForm33A sınıfı
    /// </summary>
    public class ExcelRaporGiderForm33A
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
            raporParam["raporTur"] = "FIN";
            return ExcelRaporEkoFonKod.RaporAl(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);
        }
    }

    /// <summary>
    /// ExcelRaporGiderForm33C sınıfı
    /// </summary>
    public class ExcelRaporGiderForm33C
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
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;

            Kullanici kullanan = (Kullanici)o_kullanan;

            IKHMServis servisKHM = TNS.KHM.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            string ayAd = (string)raporParam["AyAd"];
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";
            string raporTur = (string)raporParam["raporTur"];

            IUZYServis uzyServis;
            if (o_servis is IUZYServis)
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABButce);

            kistas.kodlar[0] = kur;
            kistas.kodlar[1] = fon;
            kistas.kodlar[2] = fin;
            kistas.kodlar[3] = eko;

            if (KesinHesap.Genel.GormeYetkisiVarMiKesinHesap(kullanan, kurum))
            {
                TNSCollection cevap = new TNSCollection();
                cevap = servisKHM.KesinHesapFinansalCetvelHazirla(kullanan, yil, ay, kistas);

                if (cevap != null && cevap.Count > 0)
                {
                    if (cevap[0].GetType() == typeof(Sonuc))
                        throw new ButceHazirlikv2.RaporHatasi(((Sonuc)cevap[0]).hataStr);
                }

                if (kurum != "")
                {
                    string kurumEk = "";
                    if (kurum.Length == 2) kurumEk = "00";
                    XLS.HucreAdBulYaz("KurumAd", kurum + kurumEk + "  " + uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurum, true));
                }

                XLS.HucreAdBulYaz("ButceYil", yil);
                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

                double ysot = 0.0, eklenen = 0.0, dusulen = 0.0, devir1 = 0.0, devir2 = 0.0;
                string[] kodlar = new string[11];
                double[] toplam = new double[18];

                for (int k = 0; k < cevap.Count; k++)
                {
                    if (cevap[k].GetType() != typeof(KHTertipBilgi))
                        return "HATA: Sunucudan beklenmeyen kayıt tipi geldi.";

                    KHTertipBilgi tertipBilgi = (KHTertipBilgi)cevap[k];

                    eklenen = tertipBilgi.kiciEk + tertipBilgi.karaEk + tertipBilgi.yedekEk + tertipBilgi.odenekEkle + tertipBilgi.ozelOdenek;
                    dusulen = tertipBilgi.kiciDus + tertipBilgi.karaDus + tertipBilgi.yedekDus + tertipBilgi.yiliciIptal;
                    devir1 = tertipBilgi.akreditifdevir + tertipBilgi.taahhutdevir + tertipBilgi.disprojedevir + tertipBilgi.digerdevir;
                    devir2 = tertipBilgi.bagisdevir + tertipBilgi.ozeldevir;
                    ysot = tertipBilgi.kbo + eklenen - dusulen + devir1 + devir2;

                    bool yazma = false;
                    double yazmaKontrol = eklenen + dusulen + devir1 + devir2 + ysot + tertipBilgi.devir + tertipBilgi.kbo + tertipBilgi.yilSonuIptal + tertipBilgi.odenekUstu + tertipBilgi.harcama;
                    if (yazmaKontrol == 0)
                        yazma = true;

                    if (!yazma)
                    {
                        XLS.HucreDegerYaz(satir, 0, k.ToString());
                        XLS.HucreDegerYaz(satir, 4, uzyServis.UzayDegeriStr(kullanan, "ABSFIN", yil.ToString() + "-" + k.ToString(), true).ToUpper());

                        XLS.HucreDegerYaz(satir, 5, tertipBilgi.bo);
                        XLS.HucreDegerYaz(satir, 6, tertipBilgi.bo - tertipBilgi.kbo);
                        XLS.HucreDegerYaz(satir, 7, devir1);
                        XLS.HucreDegerYaz(satir, 8, tertipBilgi.odenekEkle);
                        XLS.HucreDegerYaz(satir, 9, tertipBilgi.karaEk);
                        XLS.HucreDegerYaz(satir, 10, tertipBilgi.karaDus);
                        XLS.HucreDegerYaz(satir, 11, tertipBilgi.kiciEk);
                        XLS.HucreDegerYaz(satir, 12, tertipBilgi.kiciDus);
                        XLS.HucreDegerYaz(satir, 13, tertipBilgi.yedekEk);
                        XLS.HucreDegerYaz(satir, 14, tertipBilgi.yedekDus);
                        XLS.HucreDegerYaz(satir, 15, devir2);
                        XLS.HucreDegerYaz(satir, 16, tertipBilgi.ozelOdenek);
                        XLS.HucreDegerYaz(satir, 17, tertipBilgi.yiliciIptal);
                        XLS.HucreDegerYaz(satir, 18, ysot);
                        XLS.HucreDegerYaz(satir, 19, tertipBilgi.harcama);
                        XLS.HucreDegerYaz(satir, 20, tertipBilgi.odenekUstu);
                        XLS.HucreDegerYaz(satir, 21, tertipBilgi.yilSonuIptal);
                        XLS.HucreDegerYaz(satir, 22, tertipBilgi.devir);

                        toplam[0] += tertipBilgi.bo;
                        toplam[1] += tertipBilgi.bo - tertipBilgi.kbo;
                        toplam[2] += devir1;
                        toplam[3] += tertipBilgi.odenekEkle;
                        toplam[4] += tertipBilgi.karaEk;
                        toplam[5] += tertipBilgi.karaDus;
                        toplam[6] += tertipBilgi.kiciEk;
                        toplam[7] += tertipBilgi.kiciDus;
                        toplam[8] += tertipBilgi.yedekEk;
                        toplam[9] += tertipBilgi.yedekDus;
                        toplam[10] += devir2;
                        toplam[11] += tertipBilgi.ozelOdenek;
                        toplam[12] += tertipBilgi.yiliciIptal;
                        toplam[13] += ysot;
                        toplam[14] += tertipBilgi.harcama;
                        toplam[15] += tertipBilgi.odenekUstu;
                        toplam[16] += tertipBilgi.yilSonuIptal;
                        toplam[17] += tertipBilgi.devir;

                        satir++;
                    }
                }

                XLS.HucreDegerYaz(satir, 4, "GENEL TOPLAM");
                for (int i = 0; i < toplam.Length; i++)
                {
                    XLS.HucreDegerYaz(satir, 5 + i, toplam[i]);
                }
                XLS.KoyuYap(satir, 0, satir, 24, true);
                XLS.DuseyHizala(satir, 4, 2);


                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new ButceHazirlikv2.RaporHatasi(KesinHesap.Genel.FRM017);
        }
    }

    /// <summary>
    /// ExcelRaporEkoFonKod sınıfı
    /// </summary>
    public class ExcelRaporEkoFonKod
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
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;

            Kullanici kullanan = (Kullanici)o_kullanan;

            IKHMServis servisKHM = TNS.KHM.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            string ayAd = (string)raporParam["AyAd"];
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";
            string raporTur = (string)raporParam["raporTur"];

            IUZYServis uzyServis;
            if (o_servis is IUZYServis)
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABButce);

            kistas.kodlar[0] = kur;
            kistas.kodlar[1] = fon;
            kistas.kodlar[2] = fin;
            kistas.kodlar[3] = eko;

            if (KesinHesap.Genel.GormeYetkisiVarMiKesinHesap(kullanan, kurum))
            {
                TNSCollection cevap = new TNSCollection();
                int satirsayisi = 11;
                if (raporTur == "EKO")
                    cevap = servisKHM.KesinHesapEkonomikCetvelHazirla(kullanan, yil, ay, kistas);
                else if (raporTur == "FIN")
                    cevap = servisKHM.KesinHesapFinansalCetvelHazirla(kullanan, yil, ay, kistas);
                else
                    cevap = servisKHM.KesinHesapFonksiyonelCetvelHazirla(kullanan, yil, ay, kistas);

                if (cevap != null && cevap.Count > 0)
                {

                    if (cevap[0].GetType() == typeof(Sonuc))
                        throw new ButceHazirlikv2.RaporHatasi(((Sonuc)cevap[0]).hataStr);
                }

                if (kurum != "")
                {
                    string kurumEk = "";
                    if (kurum.Length == 2) kurumEk = "00";
                    XLS.HucreAdBulYaz("KurumAd", kurum + kurumEk + "  " + uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurum, true));
                }

                XLS.HucreAdBulYaz("ButceYil", yil);

                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                satir++;//İlk Görünen satıra set etmek için
                double ysot = 0.0, eklenen = 0.0, dusulen = 0.0, devir1 = 0.0, devir2 = 0.0;
                string[] kodlar = new string[11];
                double sutunTop1 = 0, sutunTop2 = 0, sutunTop3 = 0, sutunTop4 = 0, sutunTop5 = 0, sutunTop6 = 0;

                for (int k = 0; k <= satirsayisi; k++)
                {
                    if (cevap[k].GetType() != typeof(KHTertipBilgi))
                        return "HATA: Sunucudan beklenmeyen kayıt tipi geldi.";
                    KHTertipBilgi tertipBilgi = (KHTertipBilgi)cevap[k];

                    eklenen = tertipBilgi.kiciEk + tertipBilgi.karaEk + tertipBilgi.yedekEk + tertipBilgi.odenekEkle + tertipBilgi.ozelOdenek;
                    dusulen = tertipBilgi.kiciDus + tertipBilgi.karaDus + tertipBilgi.yedekDus + tertipBilgi.yiliciIptal;
                    devir1 = tertipBilgi.akreditifdevir + tertipBilgi.taahhutdevir + tertipBilgi.disprojedevir + tertipBilgi.digerdevir;
                    devir2 = tertipBilgi.bagisdevir + tertipBilgi.ozeldevir;
                    ysot = tertipBilgi.kbo + eklenen - dusulen + devir1 + devir2;

                    bool yazma = false;
                    if (ysot == 0 && tertipBilgi.harcama == 0) yazma = true;
                    if (!yazma)
                    {
                        if (raporTur == "FIN")
                        {
                            XLS.HucreDegerYaz(satir, 0, k.ToString());
                            XLS.HucreDegerYaz(satir, 4, uzyServis.UzayDegeriStr(kullanan, "ABSFIN", yil.ToString() + "-" + k.ToString(), true).ToUpper());

                            XLS.HucreDegerYaz(satir, sutun, devir1);
                            XLS.HucreDegerYaz(satir, sutun + 1, ysot);
                            XLS.HucreDegerYaz(satir, sutun + 2, tertipBilgi.harcama);
                            XLS.HucreDegerYaz(satir, sutun + 3, tertipBilgi.odenekUstu);
                            XLS.HucreDegerYaz(satir, sutun + 4, tertipBilgi.yilSonuIptal);
                            XLS.HucreDegerYaz(satir, sutun + 5, tertipBilgi.devir);
                            sutunTop1 += devir1;
                            sutunTop2 += ysot;
                            sutunTop3 += tertipBilgi.harcama;
                            sutunTop4 += tertipBilgi.odenekUstu;
                            sutunTop5 += tertipBilgi.yilSonuIptal;
                            sutunTop6 += tertipBilgi.devir;
                        }
                        else
                        {
                            XLS.HucreDegerYaz(satir, 0, k.ToString("00"));
                            XLS.HucreDegerYaz(satir, 4, uzyServis.UzayDegeriStr(kullanan, "ABSFON", yil.ToString() + "-" + k.ToString("00"), true));

                            XLS.HucreDegerYaz(satir, sutun, ysot);
                            XLS.HucreDegerYaz(satir, sutun + 1, tertipBilgi.gonderilenodenek);
                            XLS.HucreDegerYaz(satir, sutun + 2, tertipBilgi.harcama);
                            XLS.HucreDegerYaz(satir, sutun + 3, tertipBilgi.odenekUstu);
                            XLS.HucreDegerYaz(satir, sutun + 4, tertipBilgi.yilSonuIptal);
                            XLS.HucreDegerYaz(satir, sutun + 5, tertipBilgi.devir);
                            sutunTop1 += ysot;
                            sutunTop2 += tertipBilgi.gonderilenodenek;
                            sutunTop3 += tertipBilgi.harcama;
                            sutunTop4 += tertipBilgi.odenekUstu;
                            sutunTop5 += tertipBilgi.yilSonuIptal;
                            sutunTop6 += tertipBilgi.devir;
                        }
                        satir++;
                    }
                }

                XLS.HucreDegerYaz(satir, 4, "GENEL TOPLAM");
                XLS.HucreDegerYaz(satir, sutun, sutunTop1);
                XLS.HucreDegerYaz(satir, sutun + 1, sutunTop2);
                XLS.HucreDegerYaz(satir, sutun + 2, sutunTop3);
                XLS.HucreDegerYaz(satir, sutun + 3, sutunTop4);
                XLS.HucreDegerYaz(satir, sutun + 4, sutunTop5);
                XLS.HucreDegerYaz(satir, sutun + 5, sutunTop6);
                XLS.KoyuYap(satir, 0, satir, 20, true);
                XLS.DuseyHizala(satir, 4, 2);

                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new ButceHazirlikv2.RaporHatasi(KesinHesap.Genel.FRM017);
        }
    }

    /// <summary>
    /// ExcelRaporGiderForm32 sınıfı
    /// </summary>
    public class ExcelRaporGiderForm32
    {
        IKHMServis servisKHM = TNS.KHM.Arac.Tanimla();

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
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;

            Kullanici kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            string ayAd = (string)raporParam["AyAd"];
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";

            IUZYServis uzyServis;
            if (o_servis is IUZYServis)
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);


            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABButce);

            kistas.kodlar[0] = kur;
            kistas.kodlar[1] = fon;
            kistas.kodlar[2] = fin;
            kistas.kodlar[3] = eko;

            if (KesinHesap.Genel.GormeYetkisiVarMiKesinHesap(kullanan, kurum))
            {
                TNSCollection cevap = new TNSCollection();


                cevap = servisKHM.KesinHesapKodCetvelHazirla(kullanan, yil, ay, kistas, "4", 2);

                if (cevap != null && cevap.Count > 0)
                {

                    if (cevap[0].GetType() == typeof(Sonuc))
                        throw new ButceHazirlikv2.RaporHatasi(((Sonuc)cevap[0]).hataStr);
                }

                if (kurum != "")
                {
                    string kurumEk = "";
                    if (kurum.Length == 2) kurumEk = "00";
                    XLS.HucreAdBulYaz("KurumAd", kurum + kurumEk + "  " + uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurum, true));
                }

                XLS.HucreAdBulYaz("ButceYil", yil);


                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                int[] koyuAcikYer = { satir - 2, satir - 1 };
                int koyuAcik = 0;

                int ilkSatir = satir;
                double ysot = 0.0, eklenen = 0.0, dusulen = 0.0, devir1 = 0.0, devir2 = 0.0;
                string[] kodlar = new string[11];

                for (int k = 0; k < cevap.Count; k++)
                {
                    if (cevap[k].GetType() != typeof(TertipBilgi))
                        return "HATA: Sunucudan beklenmeyen kayıt tipi geldi.";

                    ObjectArray dizi = new ObjectArray();
                    TertipBilgi tb = (TertipBilgi)cevap[k];
                    string ekoKod = "";
                    KHTertipBilgi tertipBilgi = (KHTertipBilgi)tb.tertipBilgi;
                    if (k != cevap.Count - 1)
                    {
                        if ((string)tb.tertip.kodlar[3] != "")
                        {
                            int j = OrtakFonksiyonlar.StringiParcala((string)tb.tertip.kodlar[3], ".", dizi);
                            //if (j == 1) XLS.HucreDegerYaz(satir, sutun, (string)dizi.str[0]);
                            //if (j > 1) XLS.HucreDegerYaz(satir, sutun + 1, (string)dizi.str[1]);
                            if (j == 1)
                            {
                                ekoKod = (string)dizi.objeler[0];
                                koyuAcik = 0;
                            }
                            if (j > 1)
                            {
                                ekoKod = (string)dizi.objeler[1];
                                koyuAcik = 1;
                            }
                        }

                        XLS.HucreKopyala(koyuAcikYer[koyuAcik], sutun, koyuAcikYer[koyuAcik], sutun + 8, satir, sutun);
                        XLS.HucreDegerYaz(satir, sutun + koyuAcik, ekoKod);
                    }
                    else
                        XLS.HucreKopyala(koyuAcikYer[0], sutun, koyuAcikYer[0], sutun + 8, satir, sutun);

                    XLS.SatirYukseklikAyarla(satir + 1, satir + 1, XLS.SatirYukseklikAl(satir));
                    eklenen = tertipBilgi.kiciEk + tertipBilgi.karaEk + tertipBilgi.yedekEk + tertipBilgi.odenekEkle + tertipBilgi.ozelOdenek;
                    dusulen = tertipBilgi.kiciDus + tertipBilgi.karaDus + tertipBilgi.yedekDus + tertipBilgi.yiliciIptal;
                    devir1 = tertipBilgi.akreditifdevir + tertipBilgi.taahhutdevir + tertipBilgi.disprojedevir + tertipBilgi.digerdevir;
                    devir2 = tertipBilgi.bagisdevir + tertipBilgi.ozeldevir;
                    ysot = tertipBilgi.kbo + eklenen - dusulen + devir1 + devir2;

                    double yazmaKontrol = ysot + tertipBilgi.gonderilenodenek + tertipBilgi.harcama + tertipBilgi.odenekUstu + tertipBilgi.yilSonuIptal + tertipBilgi.devir;
                    if (yazmaKontrol == 0)
                        continue;

                    XLS.HucreDegerYaz(satir, sutun + 2, tb.aciklama);
                    XLS.HucreDegerYaz(satir, sutun + 3, ysot);
                    XLS.HucreDegerYaz(satir, sutun + 4, tertipBilgi.gonderilenodenek);
                    XLS.HucreDegerYaz(satir, sutun + 5, tertipBilgi.harcama);
                    XLS.HucreDegerYaz(satir, sutun + 6, tertipBilgi.odenekUstu);
                    XLS.HucreDegerYaz(satir, sutun + 7, tertipBilgi.yilSonuIptal);
                    XLS.HucreDegerYaz(satir, sutun + 8, tertipBilgi.devir);

                    satir++;
                }

                for (int sut = 0; sut < 20; sut++)
                    XLS.KoyuYap(satir - 1, sut, true);

                XLS.YatayCizgiCiz(satir - 1, sutun, sutun + 8, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK, false);
                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new ButceHazirlikv2.RaporHatasi(KesinHesap.Genel.FRM017);
        }
    }

    /// <summary>
    /// ExcelRaporGiderForm33 sınıfı
    /// </summary>
    public class ExcelRaporGiderForm33
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
            int kirilim = 11;
            Kullanici kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            string ayAd = (string)raporParam["AyAd"];
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";

            IUZYServis uzyServis;
            if (o_servis is IUZYServis)
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);
            //asama=4;
            int sutunSay = 18;

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABButce);

            kistas.kodlar[0] = kur;
            kistas.kodlar[1] = fon;
            kistas.kodlar[2] = fin;
            kistas.kodlar[3] = eko;

            if (KesinHesap.Genel.GormeYetkisiVarMiKesinHesap(kullanan, kurum))
            {

                TNSCollection cevap = servisKHM.KesinHesapGiderCetveliHazirlaKurumsal(kullanan, yil, ay, kistas, true);

                if (cevap != null && cevap.Count > 0)
                {
                    if (cevap[0].GetType() == typeof(Sonuc))
                        throw new ButceHazirlikv2.RaporHatasi(((Sonuc)cevap[0]).hataStr);
                }

                if (kurum != "")
                {
                    string kurumEk = "";
                    if (kurum.Length == 2) kurumEk = "00";
                    XLS.HucreAdBulYaz("KurumAd", kurum + kurumEk + "  " + uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurum, true));
                }

                XLS.HucreAdBulYaz("ButceYil", yil);

                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                int[] koyuAcikYer = { satir - 2, satir - 1 };
                int ilkSatir = satir;
                double ysot = 0.0, eklenen = 0.0, dusulen = 0.0, devir1 = 0.0, devir2 = 0.0;
                string[] kodlar = new string[11];

                for (int k = 0; k < cevap.Count; k++)
                {
                    if (cevap[k].GetType() != typeof(UcgenSatir))
                        return "HATA: Sunucudan beklenmeyen kayıt tipi geldi.";

                    UcgenSatir us = (UcgenSatir)cevap[k];
                    KHTertipBilgi tertipBilgi = (KHTertipBilgi)us.ekDegerler[0];
                    kodlar[us.yer] = us.kodParca;
                    eklenen = tertipBilgi.kiciEk + tertipBilgi.karaEk + tertipBilgi.yedekEk + tertipBilgi.odenekEkle + tertipBilgi.ozelOdenek;
                    dusulen = tertipBilgi.kiciDus + tertipBilgi.karaDus + tertipBilgi.yedekDus + tertipBilgi.yiliciIptal;
                    devir1 = tertipBilgi.akreditifdevir + tertipBilgi.taahhutdevir + tertipBilgi.disprojedevir + tertipBilgi.digerdevir;
                    devir2 = tertipBilgi.bagisdevir + tertipBilgi.ozeldevir;
                    ysot = tertipBilgi.kbo + eklenen - dusulen + devir1 + devir2;

                    int koyuAcik;
                    if (us.yer == 0 || us.yer == 1 || us.yer == 2)
                        koyuAcik = 1;
                    else
                        koyuAcik = 0;

                    if (koyuAcikYer[koyuAcik] != satir)
                        XLS.HucreKopyala(koyuAcikYer[koyuAcik], sutun, koyuAcikYer[koyuAcik], sutun + sutunSay, satir, sutun);

                    for (int i = sutun; i < sutun + sutunSay; i++)
                        XLS.HucreDegerYaz(satir, i, "");

                    if (us.yer == 1)
                    {
                        if (Convert.ToInt16(kodlar[0]) >= 30 || Convert.ToInt16(us.kodParca) >= 75)
                            satir--;
                    }

                    bool yazma = false;

                    //if (us.yer != 3 && us.kodParca == "00")
                    //{
                    //    yazma = true;
                    //    satir--;
                    //}

                    if (!yazma)
                    {
                        XLS.HucreDegerYaz(satir, us.yer, us.kodParca);
                        XLS.HucreDegerYaz(satir, sutun + kirilim, us.aciklama);
                        XLS.HucreDegerYaz(satir, sutun + kirilim + 1, ysot);
                        XLS.HucreDegerYaz(satir, sutun + kirilim + 2, tertipBilgi.gonderilenodenek);
                        XLS.HucreDegerYaz(satir, sutun + kirilim + 3, tertipBilgi.harcama);
                        XLS.HucreDegerYaz(satir, sutun + kirilim + 4, tertipBilgi.odenekUstu);
                        XLS.HucreDegerYaz(satir, sutun + kirilim + 5, tertipBilgi.yilSonuIptal);
                        XLS.HucreDegerYaz(satir, sutun + kirilim + 6, tertipBilgi.devir);
                    }
                    satir++;
                }
                for (int i = sutun; i < sutun + sutunSay; i++)
                    XLS.HucreDegerYaz(satir, i, "");

                XLS.KoyuYap(satir - 1, 0, satir, 20, true);


                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new ButceHazirlikv2.RaporHatasi(KesinHesap.Genel.FRM017);
        }
    }

    /// <summary>
    /// ExcelRaporGiderForm34 sınıfı
    /// </summary>
    public class ExcelRaporGiderForm34
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
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0, satir2 = 0, ekokod = 0, fonkod = 0;
            int sutun = 0, sutun2 = 0;

            Kullanici kullanan = (Kullanici)o_kullanan;

            IKHMServis servisKHM = TNS.KHM.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            string ayAd = (string)raporParam["AyAd"];
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";

            IUZYServis uzyServis;
            if (o_servis is IUZYServis)
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABButce);

            kistas.kodlar[0] = kur;
            kistas.kodlar[1] = fon;
            kistas.kodlar[2] = fin;
            kistas.kodlar[3] = eko;

            if (KesinHesap.Genel.GormeYetkisiVarMiKesinHesap(kullanan, kurum))
            {
                TNSCollection cevap = new TNSCollection();

                cevap = servisKHM.KesinHesapCaprazCetvelHazirla(kullanan, yil, ay, kistas);


                if (cevap != null && cevap.Count > 0)
                {

                    if (cevap[0].GetType() == typeof(Sonuc))
                        throw new ButceHazirlikv2.RaporHatasi(((Sonuc)cevap[0]).hataStr);
                }

                if (kurum != "")
                {
                    string kurumEk = "";
                    if (kurum.Length == 2) kurumEk = "00";
                    XLS.HucreAdBulYaz("KurumAd", kurum + kurumEk + "  " + uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurum, true));
                }

                XLS.HucreAdBulYaz("ButceYil", yil);

                XLS.HucreAdAdresCoz("BaslaSatir1", ref satir, ref sutun);
                XLS.HucreAdAdresCoz("BaslaSatir2", ref satir2, ref sutun2);
                double ysot = 0.0, eklenen = 0.0, dusulen = 0.0, devir1 = 0.0, devir2 = 0.0;
                string[] kodlar = new string[11];
                double[] toplam1 = new double[20];
                double[] toplam2 = new double[20];
                string[,] bilgiler = new string[13, 24];

                for (int k = 0; k < cevap.Count; k++)
                {
                    if (cevap[k].GetType() != typeof(TertipBilgi))
                        return "HATA: Sunucudan beklenmeyen kayıt tipi geldi.";
                    TertipBilgi tertipBilgi = (TertipBilgi)cevap[k];

                    eklenen = tertipBilgi.tertipBilgi.kiciEk + tertipBilgi.tertipBilgi.karaEk + tertipBilgi.tertipBilgi.yedekEk + tertipBilgi.tertipBilgi.odenekEkle + tertipBilgi.tertipBilgi.ozelOdenek;
                    dusulen = tertipBilgi.tertipBilgi.kiciDus + tertipBilgi.tertipBilgi.karaDus + tertipBilgi.tertipBilgi.yedekDus + tertipBilgi.tertipBilgi.yiliciIptal;
                    devir1 = tertipBilgi.tertipBilgi.akreditifdevir + tertipBilgi.tertipBilgi.taahhutdevir + tertipBilgi.tertipBilgi.disprojedevir + tertipBilgi.tertipBilgi.digerdevir;
                    devir2 = tertipBilgi.tertipBilgi.bagisdevir + tertipBilgi.tertipBilgi.ozeldevir;
                    ysot = tertipBilgi.tertipBilgi.kbo + eklenen - dusulen + devir1 + devir2;
                    ekokod = Convert.ToInt16((string)tertipBilgi.tertip.kodlar[3]);
                    fonkod = Convert.ToInt16((string)tertipBilgi.tertip.kodlar[1]);
                    string fonAd = uzyServis.UzayDegeriStr(kullanan, "ABSFON", yil.ToString() + "-" + fonkod.ToString("00"), true).ToUpper();

                    if (ysot > 0 || tertipBilgi.tertipBilgi.harcama > 0)
                    {
                        bilgiler[fonkod, 0] = fonkod.ToString("00");
                        bilgiler[fonkod, 1] = fonAd;

                        bilgiler[fonkod, (2 * ekokod)] = ysot.ToString();
                        bilgiler[fonkod, (2 * ekokod) + 1] = tertipBilgi.tertipBilgi.harcama.ToString();

                        bilgiler[fonkod, 22] = (OrtakFonksiyonlar.ConvertToDbl(bilgiler[fonkod, 22]) + ysot).ToString();
                        bilgiler[fonkod, 23] = (OrtakFonksiyonlar.ConvertToDbl(bilgiler[fonkod, 23]) + tertipBilgi.tertipBilgi.harcama).ToString();
                    }
                }

                int sayac = 0;

                for (int i = 0; i < 11; i++)
                {
                    double t1 = OrtakFonksiyonlar.ConvertToDbl(bilgiler[i, 22]);
                    double t2 = OrtakFonksiyonlar.ConvertToDbl(bilgiler[i, 23]);
                    if (t1 > 0 || t2 > 0)
                    {
                        for (int j = 1; j < 10; j++)
                        {
                            XLS.HucreDegerYaz(satir + sayac, sutun, bilgiler[i, 1]);
                            XLS.HucreDegerYaz(satir2 + sayac, sutun, bilgiler[i, 1]);
                            XLS.HucreDegerYaz(satir + sayac, sutun - 1, bilgiler[i, 0]);
                            XLS.HucreDegerYaz(satir2 + sayac, sutun - 1, bilgiler[i, 0]);
                            double ys = OrtakFonksiyonlar.ConvertToDbl(bilgiler[i, (2 * j)]);
                            double gh = OrtakFonksiyonlar.ConvertToDbl(bilgiler[i, (2 * j) + 1]);

                            if (j < 6)
                            {
                                XLS.HucreDegerYaz(satir + sayac, sutun + (2 * j) - 1, ys);
                                XLS.HucreDegerYaz(satir + sayac, sutun + (2 * j), gh);
                            }
                            else
                            {
                                XLS.HucreDegerYaz(satir2 + sayac, sutun2 + (2 * (j - 5)) - 1, ys);
                                XLS.HucreDegerYaz(satir2 + sayac, sutun2 + (2 * (j - 5)), gh);
                            }
                            toplam1[j] += ys;
                            toplam2[j] += gh;

                        }
                        sayac++;
                    }
                }

                XLS.HucreDegerYaz(satir + sayac, sutun, "TOPLAM");
                XLS.HucreDegerYaz(satir2 + sayac, sutun, "TOPLAM");

                for (int i = 1; i < 10; i++)
                {
                    toplam1[19] += toplam1[i];
                    toplam2[19] += toplam2[i];
                    if (i < 6)
                    {
                        XLS.HucreDegerYaz(satir + sayac, sutun + (2 * i) - 1, toplam1[i]);
                        XLS.HucreDegerYaz(satir + sayac, sutun + (2 * i), toplam2[i]);
                    }
                    else
                    {
                        XLS.HucreDegerYaz(satir2 + sayac, sutun + (2 * (i - 5)) - 1, toplam1[i]);
                        XLS.HucreDegerYaz(satir2 + sayac, sutun + (2 * (i - 5)), toplam2[i]);
                    }
                }
                XLS.HucreDegerYaz(satir2 + sayac, sutun + (2 * (10 - 5)) - 1, toplam1[19]);
                XLS.HucreDegerYaz(satir2 + sayac, sutun + (2 * (10 - 5)), toplam2[19]);

                for (int i = 0; i < 10; i++)
                {
                    if (XLS.HucreDegerAl(satir + i, sutun) == "")
                    {
                        XLS.KoyuYap(satir + i - 1, sutun, satir + i - 1, 20, true);
                        XLS.KoyuYap(satir2 + i - 1, sutun, satir2 + i - 1, 20, true);
                        break;
                    }
                }

                sayac++;

                for (int i = sayac; i < 11; i++)
                {
                    XLS.HucreDegerYaz(satir2 + i, sutun + (2 * (10 - 5)) - 1, "");
                    XLS.HucreDegerYaz(satir2 + i, sutun + (2 * (10 - 5)), "");
                }

                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new ButceHazirlikv2.RaporHatasi(KesinHesap.Genel.FRM017);
        }
    }

    /// <summary>
    /// ExcelRaporGiderForm38 sınıfı
    /// </summary>
    public class ExcelRaporGiderForm38
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
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0, satir2 = 0, ekokod = 0, fonkod = 0;
            int sutun = 0, sutun2 = 0;

            Kullanici kullanan = (Kullanici)o_kullanan;

            IKHMServis servisKHM = TNS.KHM.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            string ayAd = (string)raporParam["AyAd"];
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";

            IUZYServis uzyServis;
            if (o_servis is IUZYServis)
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABButce);

            kistas.kodlar[0] = kur;
            kistas.kodlar[1] = fon;
            kistas.kodlar[2] = fin;
            kistas.kodlar[3] = eko;

            if (KesinHesap.Genel.GormeYetkisiVarMiKesinHesap(kullanan, kurum))
            {
                TNSCollection cevap = new TNSCollection();

                cevap = servisKHM.KesinHesapCaprazCetvelHazirla(kullanan, yil, ay, kistas);


                if (cevap != null && cevap.Count > 0)
                {

                    if (cevap[0].GetType() == typeof(Sonuc))
                        throw new ButceHazirlikv2.RaporHatasi(((Sonuc)cevap[0]).hataStr);
                }

                if (kurum != "")
                {
                    string kurumEk = "";
                    if (kurum.Length == 2) kurumEk = "00";
                    XLS.HucreAdBulYaz("KurumAd", kurum + kurumEk + "  " + uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurum, true));
                }

                XLS.HucreAdBulYaz("ButceYil", yil);


                XLS.HucreAdAdresCoz("BaslaSatir1", ref satir, ref sutun);
                XLS.HucreAdAdresCoz("BaslaSatir2", ref satir2, ref sutun2);
                double ysot = 0.0, eklenen = 0.0, dusulen = 0.0, devir1 = 0.0, devir2 = 0.0;
                string[] kodlar = new string[11];
                double[] toplam = new double[40];

                for (int k = 0; k < cevap.Count; k++)
                {
                    if (cevap[k].GetType() != typeof(TertipBilgi))
                        return "HATA: Sunucudan beklenmeyen kayıt tipi geldi.";
                    TertipBilgi tertipBilgi = (TertipBilgi)cevap[k];

                    eklenen = tertipBilgi.tertipBilgi.kiciEk + tertipBilgi.tertipBilgi.karaEk + tertipBilgi.tertipBilgi.yedekEk + tertipBilgi.tertipBilgi.odenekEkle + tertipBilgi.tertipBilgi.ozelOdenek;
                    dusulen = tertipBilgi.tertipBilgi.kiciDus + tertipBilgi.tertipBilgi.karaDus + tertipBilgi.tertipBilgi.yedekDus + tertipBilgi.tertipBilgi.yiliciIptal;
                    devir1 = tertipBilgi.tertipBilgi.akreditifdevir + tertipBilgi.tertipBilgi.taahhutdevir + tertipBilgi.tertipBilgi.disprojedevir + tertipBilgi.tertipBilgi.digerdevir;
                    devir2 = tertipBilgi.tertipBilgi.bagisdevir + tertipBilgi.tertipBilgi.ozeldevir;
                    ysot = tertipBilgi.tertipBilgi.kbo + eklenen - dusulen + devir1 + devir2;
                    ekokod = Convert.ToInt16((string)tertipBilgi.tertip.kodlar[3]);
                    fonkod = Convert.ToInt16((string)tertipBilgi.tertip.kodlar[1]);
                    string fonAd = uzyServis.UzayDegeriStr(kullanan, "ABSFON", yil.ToString() + "-" + fonkod.ToString("00"), true).ToUpper();

                    if (ysot > 0 || tertipBilgi.tertipBilgi.harcama > 0)
                    {
                        XLS.HucreDegerYaz(satir + fonkod - 1, sutun, fonAd);
                        XLS.HucreDegerYaz(satir2 + fonkod - 1, sutun, fonAd);
                        XLS.HucreDegerYaz(satir + fonkod - 1, sutun - 1, fonkod.ToString("00"));
                        XLS.HucreDegerYaz(satir2 + fonkod - 1, sutun - 1, fonkod.ToString("00"));

                        if (ekokod < 6)
                            XLS.HucreDegerYaz(satir + fonkod - 1, sutun + ekokod, tertipBilgi.tertipBilgi.odenekUstu);
                        else
                            XLS.HucreDegerYaz(satir2 + fonkod - 1, sutun2 + ekokod - 5, tertipBilgi.tertipBilgi.odenekUstu);
                    }

                    toplam[ekokod] += tertipBilgi.tertipBilgi.odenekUstu;
                }
                XLS.HucreDegerYaz(satir + 10, sutun, "TOPLAM");
                XLS.HucreDegerYaz(satir2 + 10, sutun, "TOPLAM");

                for (int i = 1; i < 10; i++)
                {
                    toplam[19] += toplam[i];
                    if (i < 6)
                        XLS.HucreDegerYaz(satir + 10, sutun + i, toplam[i]);
                    else
                        XLS.HucreDegerYaz(satir2 + 10, sutun + i - 5, toplam[i]);
                }
                XLS.HucreDegerYaz(satir2 + 10, sutun + 5, toplam[19]);

                //Rakamları 0 olan satırı sil aşağıdakini yukarı taşı 
                for (int i = 0; i < 10; i++)
                {
                    if (XLS.HucreDegerAl(satir + i, sutun) == "")
                        YukariTasi(XLS, i, satir, sutun, satir2);

                    //Boş olan satırlara 0 yaz
                    if (XLS.HucreDegerAl(satir + i, sutun) != "")
                    {
                        for (int j = 2; j < 7; j++)
                        {
                            if (XLS.HucreDegerAl(satir + i, j) == "")
                                XLS.HucreDegerYaz(satir + i, j, 0);

                            if (XLS.HucreDegerAl(satir2 + i, j) == "")
                                XLS.HucreDegerYaz(satir2 + i, j, 0);
                        }
                    }
                }

                for (int i = 0; i < 11; i++)
                {
                    if (XLS.HucreDegerAl(satir2 + i, sutun) == "")
                        XLS.HucreDegerYaz(satir2 + i, 6, "");
                }

                for (int i = 0; i < 10; i++)
                {
                    if (XLS.HucreDegerAl(satir + i, sutun) == "")
                    {
                        XLS.KoyuYap(satir + i - 1, sutun, satir + i - 1, 20, true);
                        XLS.KoyuYap(satir2 + i - 1, sutun, satir2 + i - 1, 20, true);
                        break;
                    }
                }

                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new ButceHazirlikv2.RaporHatasi(KesinHesap.Genel.FRM017);
        }

        void YukariTasi(Tablo XLS, int hedef, int satir, int sutun, int satir2)
        {
            for (int i = hedef + 1; i < 11; i++)
            {
                if (XLS.HucreDegerAl(satir + i, sutun) != "")
                {
                    for (int j = 0; j < 7; j++)
                    {
                        string deger = XLS.HucreDegerAl(satir + i, j);
                        string deger2 = XLS.HucreDegerAl(satir2 + i, j);

                        if (j > 1)
                        {
                            XLS.HucreDegerYaz(satir + hedef, j, OrtakFonksiyonlar.ConvertToDbl(deger));
                            if (j < 6)
                                XLS.HucreDegerYaz(satir2 + hedef, j, OrtakFonksiyonlar.ConvertToDbl(deger));
                        }
                        else
                        {
                            XLS.HucreDegerYaz(satir + hedef, j, deger);
                            if (j < 6)
                                XLS.HucreDegerYaz(satir2 + hedef, j, deger);
                        }

                        XLS.HucreDegerYaz(satir + i, j, "");
                        if (j < 6)
                            XLS.HucreDegerYaz(satir2 + i, j, "");
                    }
                    break;
                }
            }

        }

    }

    /// <summary>
    /// ExcelRaporGiderForm35 sınıfı
    /// </summary>
    public class ExcelRaporGiderForm35
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
            raporParam["raporTur"] = "ONCEKI";
            return ExcelRaporGiderDevir.RaporAl(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);
        }
    }

    /// <summary>
    /// ExcelRaporGiderForm36 sınıfı
    /// </summary>
    public class ExcelRaporGiderForm36
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
            raporParam["raporTur"] = "SONRAKI";
            return ExcelRaporGiderDevir.RaporAl(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);
        }
    }

    /// <summary>
    /// ExcelRaporGiderDevir sınıfı
    /// </summary>
    public class ExcelRaporGiderDevir
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
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";
            string raporTur = (string)raporParam["raporTur"];

            IUZYServis uzyServis;
            if (o_servis is IUZYServis)
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            //asama=4;
            int sutunSay = 21;

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABButce);

            kistas.kodlar[0] = kur;
            kistas.kodlar[1] = fon;
            kistas.kodlar[2] = fin;
            kistas.kodlar[3] = eko;

            if (KesinHesap.Genel.GormeYetkisiVarMiKesinHesap(kullanan, kurum))
            {
                TNSCollection cevap = null;
                if (raporTur == "ONCEKI")
                    cevap = servisKHM.KesinHesapGiderCetveliHazirlaRaporTur(kullanan, yil, ay, kistas, false, ENUMIslemKod.ONCEKIDEVIR);
                else
                    cevap = servisKHM.KesinHesapDevirCetveliHazirla(kullanan, yil, ay, kistas);

                if (cevap != null && cevap.Count > 0)
                {
                    if (cevap[0].GetType() == typeof(Sonuc))
                        throw new ButceHazirlikv2.RaporHatasi(((Sonuc)cevap[0]).hataStr);
                }

                if (kurum != "")
                {
                    string kurumEk = "";
                    if (kurum.Length == 2) kurumEk = "00";
                    XLS.HucreAdBulYaz("KurumAd", kurum + kurumEk + "  " + uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurum, true));
                }

                XLS.HucreAdBulYaz("ButceYil", yil);


                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                int[] koyuAcikYer = { satir - 2, satir - 1 };
                int ilkSatir = satir;
                double devir1 = 0.0, devir2 = 0.0, toplamdevir = 0.0;
                string[] kodlar = new string[11];

                for (int k = 0; k < cevap.Count; k++)
                {
                    if (cevap[k].GetType() != typeof(UcgenSatir))
                        return "HATA: Sunucudan beklenmeyen kayıt tipi geldi.";

                    UcgenSatir us = (UcgenSatir)cevap[k];
                    KHTertipBilgi tertipBilgi = (KHTertipBilgi)us.ekDegerler[0];
                    kodlar[us.yer] = us.kodParca;

                    int koyuAcik;
                    if (us.yer <= 9)
                        koyuAcik = 0;
                    else
                        koyuAcik = 1;

                    if (koyuAcikYer[koyuAcik] != satir)
                        XLS.HucreKopyala(koyuAcikYer[koyuAcik], sutun, koyuAcikYer[koyuAcik], sutun + sutunSay, satir, sutun);

                    for (int i = sutun; i < sutun + sutunSay; i++)
                        XLS.HucreDegerYaz(satir, i, "");

                    if (us.yer == 1)
                    {
                        if (Convert.ToInt16(kodlar[0]) >= 30 || Convert.ToInt16(us.kodParca) >= 75)
                            satir--;
                    }

                    bool yazma = false;

                    //if (us.kodParca == "00")
                    //{
                    //    yazma = true;
                    //    satir--;
                    //}

                    if (!yazma)
                    {
                        XLS.HucreDegerYaz(satir, us.yer, us.kodParca);
                        XLS.HucreDegerYaz(satir, 9 + kirilim, us.aciklama);
                        devir1 = tertipBilgi.akreditifdevir + tertipBilgi.taahhutdevir + tertipBilgi.disprojedevir + tertipBilgi.digerdevir;
                        devir2 = tertipBilgi.bagisdevir + tertipBilgi.ozeldevir;
                        toplamdevir = devir1 + devir2;
                        XLS.HucreDegerYaz(satir, 10 + kirilim, tertipBilgi.digerdevir);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 1, tertipBilgi.akreditifdevir);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 2, tertipBilgi.taahhutdevir);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 3, tertipBilgi.disprojedevir);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 4, devir1);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 5, tertipBilgi.bagisdevir);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 6, tertipBilgi.ozeldevir);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 7, devir2);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 8, toplamdevir);
                    }
                    satir++;
                }

                if (raporTur == "ONCEKI")
                {
                    XLS.HucreDegerYaz(satir - 1, 9 + kirilim, "          TOPLAM");
                }

                if (60 - satir > 0)
                {
                    for (int i = satir; i < satir + (60 - satir); i++)
                    {
                        XLS.HucreKopyala(koyuAcikYer[0], sutun, koyuAcikYer[0], sutun + sutunSay, i, sutun);
                        for (int j = sutun; j < sutun + sutunSay; j++)
                            XLS.HucreDegerYaz(i, j, "");
                    }
                    satir = 60;
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

    /// <summary>
    /// ExcelRaporGiderForm37 sınıfı
    /// </summary>
    public class ExcelRaporGiderForm37
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
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0, fonkod = 0;
            int sutun = 0;

            Kullanici kullanan = (Kullanici)o_kullanan;

            IKHMServis servisKHM = TNS.KHM.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            string ayAd = (string)raporParam["AyAd"];
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";

            IUZYServis uzyServis;
            if (o_servis is IUZYServis)
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABButce);
            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, kistas.Kirilimlar(0));
            string kurumAd = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);


            kistas.kodlar[0] = kur;
            kistas.kodlar[1] = fon;
            kistas.kodlar[2] = fin;
            kistas.kodlar[3] = eko;

            if (KesinHesap.Genel.GormeYetkisiVarMiKesinHesap(kullanan, kurum))
            {
                TNSCollection cevap = new TNSCollection();

                cevap = servisKHM.KesinHesapMahsupCetveliHazirla(kullanan, yil, ay, kistas, "2", 1);


                if (cevap != null && cevap.Count > 0)
                {

                    if (cevap[0].GetType() == typeof(Sonuc))
                        throw new ButceHazirlikv2.RaporHatasi(((Sonuc)cevap[0]).hataStr);
                }

                if (kurum != "")
                {
                    string kurumEk = "";
                    if (kurum.Length == 2) kurumEk = "00";
                    XLS.HucreAdBulYaz("KurumAd", kurum + kurumEk + "  " + uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurum, true));
                }

                XLS.HucreAdBulYaz("ButceYil", yil);


                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

                string[] kodlar = new string[11];
                double sutunTop1 = 0, sutunTop2 = 0, sutunTop3 = 0;

                for (int k = 0; k < cevap.Count; k++)
                {
                    if (cevap[k].GetType() != typeof(TertipHarcamaBilgi))
                        return "HATA: Sunucudan beklenmeyen kayıt tipi geldi.";
                    TertipHarcamaBilgi tertipBilgi = (TertipHarcamaBilgi)cevap[k];

                    fonkod = Convert.ToInt16((string)tertipBilgi.tertip.kodlar[1]);

                    if (tertipBilgi.harcama > 0 || tertipBilgi.mahsup > 0)
                    {
                        XLS.HucreDegerYaz(satir, 0, k.ToString("00"));
                        XLS.HucreDegerYaz(satir, 4, uzyServis.UzayDegeriStr(kullanan, "ABSFON", yil.ToString() + "-" + k.ToString("00"), true));

                        XLS.HucreDegerYaz(satir, sutun, (string)tertipBilgi.tertip.kodlar[1]);
                        XLS.HucreDegerYaz(satir, sutun + 4, (string)tertipBilgi.aciklama);
                        XLS.HucreDegerYaz(satir, sutun + 5, tertipBilgi.harcama - tertipBilgi.mahsup);
                        XLS.HucreDegerYaz(satir, sutun + 6, tertipBilgi.mahsup);
                        XLS.HucreDegerYaz(satir, sutun + 7, tertipBilgi.harcama);

                        sutunTop1 += tertipBilgi.harcama - tertipBilgi.mahsup;
                        sutunTop2 += tertipBilgi.mahsup;
                        sutunTop3 += tertipBilgi.harcama;

                        satir++;
                    }
                }

                XLS.HucreDegerYaz(satir, sutun + 4, "GENEL TOPLAM");
                XLS.HucreDegerYaz(satir, sutun + 5, sutunTop1);
                XLS.HucreDegerYaz(satir, sutun + 6, sutunTop2);
                XLS.HucreDegerYaz(satir, sutun + 7, sutunTop3);
                XLS.KoyuYap(satir, 0, satir, 7, true);
                XLS.DuseyHizala(satir, sutun + 4, 2);


                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new ButceHazirlikv2.RaporHatasi(KesinHesap.Genel.FRM017);
        }
    }

    /// <summary>
    /// ExcelRaporGiderForm39 sınıfı
    /// </summary>
    public class ExcelRaporGiderForm39
    {
        IKHMServis servisKHM = TNS.KHM.Arac.Tanimla();

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
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;

            Kullanici kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            string ayAd = (string)raporParam["AyAd"];
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";

            IUZYServis uzyServis;
            if (o_servis is IUZYServis)
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABButce);

            kistas.kodlar[0] = kur;
            kistas.kodlar[1] = fon;
            kistas.kodlar[2] = fin;
            kistas.kodlar[3] = eko;

            if (KesinHesap.Genel.GormeYetkisiVarMiKesinHesap(kullanan, kurum))
            {
                TNSCollection cevap = new TNSCollection();


                cevap = servisKHM.KesinHesapGiderCokYilli(kullanan, yil, ay, kistas, "4", 2, 2);

                if (cevap != null && cevap.Count > 0)
                {

                    if (cevap[0].GetType() == typeof(Sonuc))
                        throw new ButceHazirlikv2.RaporHatasi(((Sonuc)cevap[0]).hataStr);
                }

                if (kurum != "")
                {
                    string kurumEk = "";
                    if (kurum.Length == 2) kurumEk = "00";
                    XLS.HucreAdBulYaz("KurumAd", kurum + kurumEk + "  " + uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + kurum, true));
                }

                XLS.HucreAdBulYaz("ButceYil", yil);


                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                int ilkSatir = satir;
                int ilkSutun = sutun;

                //string[] kodlar = new string[11];

                for (int k = 0; k < cevap.Count; k++)
                {
                    sutun = ilkSutun;
                    if (cevap[k].GetType() != typeof(TertipBilgiler))
                        return "HATA: Sunucudan beklenmeyen kayıt tipi geldi.";

                    ObjectArray dizi = new ObjectArray();
                    TertipBilgiler tb = (TertipBilgiler)cevap[k];

                    if (k != cevap.Count - 1)
                        XLS.HucreKopyala(satir, sutun, satir, sutun + 14, satir + 1, sutun);
                    XLS.SatirYukseklikAyarla(satir + 1, satir + 1, XLS.SatirYukseklikAl(satir));
                    int j = 0;
                    if ((string)tb.tertip.kodlar[3] != "")
                    {
                        j = OrtakFonksiyonlar.StringiParcala((string)tb.tertip.kodlar[3], ".", dizi);
                        if (j == 1) XLS.HucreDegerYaz(satir, sutun, (string)dizi.objeler[0]);
                        if (j > 1) XLS.HucreDegerYaz(satir, sutun + 1, (string)dizi.objeler[1]);
                    }
                    double oran = 0;
                    if (j == 1)
                    {
                        for (int sut = 0; sut < 20; sut++)
                            XLS.KoyuYap(satir, sut, true);
                    }

                    XLS.HucreDegerYaz(satir, sutun + 2, tb.aciklama);
                    double yazmaKontrol = 0;
                    for (int i = 0; i < tb.degerler.Count; i++)
                    {
                        //sutun = sutun + i;
                        KHOdenekBilgi tertipBilgi = (KHOdenekBilgi)tb.degerler[i];
                        if (tertipBilgi.ysot > 0)
                            oran = (tertipBilgi.harcama / tertipBilgi.ysot) * 100;
                        else
                            oran = 0;

                        yazmaKontrol += tertipBilgi.kbo + tertipBilgi.ysot + tertipBilgi.harcama;

                        XLS.HucreDegerYaz(satir, sutun + 3, tertipBilgi.kbo / 1000);
                        XLS.HucreDegerYaz(satir, sutun + 4, tertipBilgi.ysot / 1000);
                        XLS.HucreDegerYaz(satir, sutun + 5, tertipBilgi.harcama / 1000);
                        XLS.HucreDegerYaz(satir, sutun + 6, oran);
                        sutun = sutun + 4;
                    }

                    if (yazmaKontrol == 0)
                    {
                        for (int sut = 0; sut < 20; sut++)
                        {
                            XLS.HucreDegerYaz(satir, sut, "");
                            XLS.KoyuYap(satir, sut, false);
                        }
                        continue;
                    }

                    satir++;
                }

                for (int sut = 0; sut < 20; sut++)
                    XLS.KoyuYap(satir - 1, sut, true);

                XLS.YatayCizgiCiz(satir - 1, ilkSutun, ilkSutun + 14, OrtakClass.LineStyle.MEDIUM, OrtakClass.TabloRenk.BLACK, false);
                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new ButceHazirlikv2.RaporHatasi(KesinHesap.Genel.FRM017);
        }
    }
}
