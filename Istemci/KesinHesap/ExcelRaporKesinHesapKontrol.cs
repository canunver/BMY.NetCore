using System;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KHM;
using TNS.KYM;
using TNS.UZY;

// Eski Istemci/KesinHesap/ExcelRaporKesinHesapKontrol.cs'ten alındı (Rapor.xml NESNE adları aynen korunmuştur); değişenler:
// - TNS.Raporlama.IRapor arayüzü Core'da yok: sınıflardan kaldırıldı (RaporAl imzası aynı, yansıma ile çağrılır).
// - Resources.KesinHesap.FRM017 yerine KesinHesap.Genel.FRM017 sabiti; throw new Exception yerine ButceHazirlikv2.RaporHatasi.
// - return "Hata" yerine "HATA: ..." metni; o_servis null olabileceği için "o_servis is IUZYServis" denetimi.
// - raporParam.Add("raporTur", ..) yerine raporParam["raporTur"] = .. (aynı Hashtable ile tekrar çağrılırsa hata vermesin).
// - Sunucu (KHM) metotları Core'da tipli TNSCollection<T> döndürür; tipsiz TNSCollection'a örtük dönüşümle eski kod aynen çalışır.
namespace KesinHesapKontrol
{
    /// <summary>
    /// ExcelRaporGiderEkonomik sınıfı
    /// </summary>
    public class ExcelRaporGiderEkonomik
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
            raporParam["raporTur"] = "EKO";
            return KesinHesapGider.ExcelRaporEkoFonKod.RaporAl(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);
        }
    }

    /// <summary>
    /// ExcelRaporGiderEkonomikDetay sınıfı
    /// </summary>
    public class ExcelRaporGiderEkonomikDetay
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
            raporParam["raporTur"] = "EKO";
            return ExcelRaporEkoFonDetay.RaporAl(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);
        }
    }

    /// <summary>
    /// ExcelRaporGiderFonksiyonelDetay sınıfı
    /// </summary>
    public class ExcelRaporGiderFonksiyonelDetay
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
            return ExcelRaporEkoFonDetay.RaporAl(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);
        }
    }

    /// <summary>
    /// ExcelRaporEkoFonDetay sınıfı
    /// </summary>
    public class ExcelRaporEkoFonDetay
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

            IUZYServis uzyServis;
            if (o_servis is IUZYServis)
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();



            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            string ayAd = (string)raporParam["AyAd"];
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";
            string raporTur = (string)raporParam["raporTur"];
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
                int satirsayisi = 10;
                if (raporTur == "EKO")
                {
                    satirsayisi = 9;
                    cevap = servisKHM.KesinHesapEkonomikCetvelHazirla(kullanan, yil, ay, kistas);
                }
                else
                {
                    cevap = servisKHM.KesinHesapFonksiyonelCetvelHazirla(kullanan, yil, ay, kistas);

                }

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
                double ysot = 0.0, eklenen = 0.0, dusulen = 0.0, devir = 0.0, ozeldevir = 0.0;
                string[] kodlar = new string[11];

                for (int k = 0; k <= satirsayisi; k++)
                {
                    if (cevap[k].GetType() != typeof(KHTertipBilgi))
                        return "HATA: Sunucudan beklenmeyen kayıt tipi geldi.";
                    KHTertipBilgi tertipBilgi = (KHTertipBilgi)cevap[k];

                    eklenen = tertipBilgi.kiciEk + tertipBilgi.karaEk + tertipBilgi.yedekEk + tertipBilgi.odenekEkle + tertipBilgi.ozelOdenek;
                    dusulen = tertipBilgi.kiciDus + tertipBilgi.karaDus + tertipBilgi.yedekDus + tertipBilgi.yiliciIptal;


                    XLS.HucreDegerYaz(satir, sutun, tertipBilgi.bo);
                    XLS.HucreDegerYaz(satir, sutun + 1, tertipBilgi.bo - tertipBilgi.kbo);

                    devir = tertipBilgi.taahhutdevir + tertipBilgi.disprojedevir + tertipBilgi.akreditifdevir + tertipBilgi.digerdevir;
                    ozeldevir = tertipBilgi.bagisdevir + tertipBilgi.digerdevir;
                    ysot = tertipBilgi.kbo + eklenen - dusulen + devir + ozeldevir;
                    XLS.HucreDegerYaz(satir, sutun + 2, devir);
                    XLS.HucreDegerYaz(satir, sutun + 3, tertipBilgi.odenekEkle);
                    XLS.HucreDegerYaz(satir, sutun + 4, tertipBilgi.karaEk);
                    XLS.HucreDegerYaz(satir, sutun + 5, tertipBilgi.karaDus);
                    XLS.HucreDegerYaz(satir, sutun + 6, tertipBilgi.kiciEk);
                    XLS.HucreDegerYaz(satir, sutun + 7, tertipBilgi.kiciDus);
                    XLS.HucreDegerYaz(satir, sutun + 8, tertipBilgi.yedekEk);
                    XLS.HucreDegerYaz(satir, sutun + 9, tertipBilgi.yedekDus);
                    XLS.HucreDegerYaz(satir, sutun + 10, ozeldevir);
                    XLS.HucreDegerYaz(satir, sutun + 11, tertipBilgi.odenekEkle);
                    XLS.HucreDegerYaz(satir, sutun + 12, tertipBilgi.yiliciIptal);
                    XLS.HucreDegerYaz(satir, sutun + 13, ysot);
                    XLS.HucreDegerYaz(satir, sutun + 14, tertipBilgi.harcama);
                    XLS.HucreDegerYaz(satir, sutun + 15, tertipBilgi.odenekUstu);
                    XLS.HucreDegerYaz(satir, sutun + 16, tertipBilgi.yilSonuIptal);
                    XLS.HucreDegerYaz(satir, sutun + 17, tertipBilgi.devir);
                    satir++;
                }
                /*
                for (int i = sutun; i < sutun + sutunSay; i++)
                    XLS.HucreDegerYaz(satir, i, "");
                 * */

                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new ButceHazirlikv2.RaporHatasi(KesinHesap.Genel.FRM017);
        }
    }

    /// <summary>
    /// ExcelRaporGiderOdenekUstu sınıfı
    /// </summary>
    public class ExcelRaporGiderOdenekUstu
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
            string raporTur = ((int)ENUMIslemKod.ODENEKUSTU).ToString();
            raporParam["raporTur"] = raporTur;
            return KontrolRaporlari.KontrolRaporlariAl(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);

        }
    }

    /// <summary>
    /// ExcelRaporGiderIptal sınıfı
    /// </summary>
    public class ExcelRaporGiderIptal
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
            string raporTur = ((int)ENUMIslemKod.YSIPTAL).ToString();
            raporParam["raporTur"] = raporTur;
            return KontrolRaporlari.KontrolRaporlariAl(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);

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
        /// <param name="ekBilgi">Ek Bilgiler değerini tutan String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String türünde nesne döndürür</returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            string raporTur = ((int)ENUMIslemKod.DEVIR).ToString();
            raporParam["raporTur"] = raporTur;
            return KontrolRaporlari.KontrolRaporlariAl(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);

        }
    }

    /// <summary>
    /// ExcelRaporGiderAkreditif sınıfı
    /// </summary>
    public class ExcelRaporGiderAkreditif
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
            string raporTur = ((int)ENUMIslemKod.AKREDITIF).ToString();
            raporParam["raporTur"] = raporTur;
            return KontrolRaporlari.KontrolRaporlariAl(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);

        }
    }

    /// <summary>
    /// ExcelRaporGiderOzellikli sınıfı
    /// </summary>
    public class ExcelRaporGiderOzellikli
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
            string raporTur = ((int)ENUMIslemKod.OZELLIKLIGIDERLER).ToString();
            raporParam["raporTur"] = raporTur;
            return KontrolRaporlari.KontrolRaporlariAl(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);

        }
    }

    /// <summary>
    /// ExcelRaporGiderYedek Sınıfı
    /// </summary>
    public class ExcelRaporGiderYedek
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
            string raporTur = ((int)ENUMIslemKod.YEDEK).ToString();
            raporParam["raporTur"] = raporTur;
            return KontrolRaporlari.KontrolRaporlariAl(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam);

        }
    }

    /// <summary>
    /// Kontrol Raporları Sınıfı
    /// </summary>
    public class KontrolRaporlari
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
        public static string KontrolRaporlariAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam)
        {

            IKHMServis servisKHM = TNS.KHM.Arac.Tanimla();

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kirilim = 2;
            ENUMIslemKod islem = ENUMIslemKod.TANIMSIZ;
            Kullanici kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            string ayAd = (string)raporParam["AyAd"];
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";
            int raporTur = OrtakFonksiyonlar.ConvertToInt((string)raporParam["raporTur"], 0);


            if (raporTur == 1)
                islem = ENUMIslemKod.YIIPTAL;
            else if (raporTur == 2)
                islem = ENUMIslemKod.DEVIR;
            else if (raporTur == 4)
                islem = ENUMIslemKod.ODENEKUSTU;
            else if (raporTur == 5)
                islem = ENUMIslemKod.YEDEK;
            else if (raporTur == 6)
                islem = ENUMIslemKod.YSIPTAL;


            IUZYServis uzyServis;
            if (o_servis is IUZYServis)
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = uzyServis.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);
            //asama=4;
            int sutunSay = 30;

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABButce);

            kistas.kodlar[0] = kur;
            kistas.kodlar[1] = fon;
            kistas.kodlar[2] = fin;
            kistas.kodlar[3] = eko;

            if (KesinHesap.Genel.GormeYetkisiVarMiKesinHesap(kullanan, kurum))
            {

                TNSCollection cevap = servisKHM.KesinHesapGiderCetveliHazirlaRaporTur(kullanan, yil, ay, kistas, false, islem);

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
                double ysot = 0.0, eklenen = 0.0, dusulen = 0.0, devir = 0.0, ozeldevir = 0.0;
                string[] kodlar = new string[11];

                for (int k = 0; k < cevap.Count; k++)
                {
                    if (cevap[k].GetType() != typeof(UcgenSatir))
                        return "HATA: Sunucudan beklenmeyen kayıt tipi geldi.";

                    UcgenSatir us = (UcgenSatir)cevap[k];
                    KHTertipBilgi tertipBilgi = (KHTertipBilgi)us.ekDegerler[0];
                    kodlar[us.yer] = us.kodParca;

                    int koyuAcik;
                    if (us.yer == 0 || us.yer == 1 || us.yer == 4 || us.yer == 9)
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
                    /*
                    if (us.yer==3 || us.yer==7)
                    {
                        if (us.kodParca=="00")
                        {
                            UcgenSatir usEski =(UcgenSatir)cevap[k-1];
                            KHTertipBilgi eskiTertipBilgi=(KHTertipBilgi)usEski.ekDegerler[0];
                            bool farkli=false;

                            if (eskiTertipBilgi.bo != tertipBilgi.bo)
                                farkli=true;

                            if (!farkli)
                                satir--;
                        }
                    }
                     */
                    if (us.kodParca == "00")
                    {
                        yazma = true;
                        satir--;
                    }

                    if (!yazma)
                    {
                        XLS.HucreDegerYaz(satir, us.yer, us.kodParca);
                        XLS.HucreDegerYaz(satir, 9 + kirilim, us.aciklama);

                        eklenen = tertipBilgi.kiciEk + tertipBilgi.karaEk + tertipBilgi.yedekEk + tertipBilgi.odenekEkle + tertipBilgi.ozelOdenek;
                        dusulen = tertipBilgi.kiciDus + tertipBilgi.karaDus + tertipBilgi.yedekDus + tertipBilgi.yiliciIptal;


                        devir = tertipBilgi.taahhutdevir + tertipBilgi.disprojedevir + tertipBilgi.akreditifdevir + tertipBilgi.digerdevir;
                        ozeldevir = tertipBilgi.bagisdevir + tertipBilgi.digerdevir;
                        ysot = tertipBilgi.kbo + eklenen - dusulen + devir + ozeldevir;
                        XLS.HucreDegerYaz(satir, 10 + kirilim, tertipBilgi.bo);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 1, tertipBilgi.bo - tertipBilgi.kbo);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 2, devir);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 3, tertipBilgi.odenekEkle);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 4, tertipBilgi.karaEk);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 5, tertipBilgi.karaDus);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 6, tertipBilgi.kiciEk);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 7, tertipBilgi.kiciDus);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 8, tertipBilgi.yedekEk);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 9, tertipBilgi.yedekDus);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 10, ozeldevir);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 11, tertipBilgi.odenekEkle);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 12, tertipBilgi.yiliciIptal);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 13, ysot);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 14, tertipBilgi.harcama);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 15, tertipBilgi.odenekUstu);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 16, tertipBilgi.yilSonuIptal);
                        XLS.HucreDegerYaz(satir, 10 + kirilim + 17, tertipBilgi.devir);

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
