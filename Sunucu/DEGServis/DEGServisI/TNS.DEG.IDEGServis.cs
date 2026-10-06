using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using TNS;
using System.Xml;
using System.Collections;

namespace TNS.DEG
{
    /// <summary>
    /// Bu sunucu sınıfı için, istemci ve sunucuda ortak kullanılacak statik fonksiyonlar içeren sınıf
    /// </summary>
    public class Arac
    {
        public static string MS(DateTime dt)
        {
            return string.Format("{0:00}:{1:00}:{2:00}.{3:000}", dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
        }
        public static string MS()
        {
            return MS(DateTime.Now);
        }

        /// <summary>
        /// Bu sunucu sınıfının nesnesini .net remoting ile oluşturup döndüren statik fonksiyon
        /// </summary>
        /// <returns>.net remoting ile oluşturulan nesne </returns>
        public static IDEGServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("DEGServisURL") + "";
            IDEGServis servisDEG = TNS.Api.ServisIstemci.Olustur<IDEGServis>(sUrl);
            return servisDEG;
        }

        public static string DegiskenDegerBul(int yil, string degisken)
        {
            return DegiskenDegerBulCache(yil, degisken);
        }

        public static string KodYapiBul(int yil, int kodNo)
        {
            string degisken = DegiskenDegerBulCache(yil, "/KODYAPI/KOD" + kodNo.ToString() + "/KODYAPI");
            return degisken;
        }


        public static string VarsayilanKurumBul()
        {
            string degisken = DegiskenDegerBulCache(0, "/KURUMKOD");
            return degisken;
        }

        public static bool OdenekGondermeYasakliMi(string ekoKod)
        {
            string[] OdGondYasakEkolar = null; //12 = { "032", "033", "035201", "035202", "035401", "035910", "037190", "037301", "037302" };

            string yasaklar = DegiskenDegerBulCache(0, "/ODENEKGONDERMEYASAK");
            if (!string.IsNullOrEmpty(yasaklar))
                OdGondYasakEkolar = yasaklar.Split(',');

            ekoKod = ekoKod.Replace(".", "");
            if (OdGondYasakEkolar == null || OdGondYasakEkolar.Length == 0) return false;
            for (int i = 0; i < OdGondYasakEkolar.Length; i++)
            {
                if (ekoKod.StartsWith(OdGondYasakEkolar[i].Replace(".", "")))
                    return true;
            }
            return false;
        }

        public static bool ManasKullaniyor()
        {
            string degisken = DegiskenDegerBulCache(0, "/MANAS");
            return OrtakFonksiyonlar.ConvertToInt(degisken, 0) > 0 ? true : false;
        }

        public static bool MerkezBankasiKullaniyor()
        {
            return OrtakFonksiyonlar.ConvertToInt(DegiskenDegerBulCache(0, "/MERKEZBANKASI"), 0) > 0;
        }

        public static string DegiskenDegerBulCache(int yil, string cacheID)
        {
            //CACHE**************************
            object cachedObjectData = TNS.Onbellek.Get("DEG_" + yil + "_" + cacheID);
            if (cachedObjectData != null)
            {
                //OrtakFonksiyonlar.HataDosyaYaz("cacheDEGI.txt", "Cache den Okundu:" + cacheID, false);
                return (string)(cachedObjectData + "");
            }
            //*******************************

            IDEGServis servisDEG = TNS.DEG.Arac.Tanimla();

            string deger = servisDEG.DegiskenDegerBul(yil, cacheID) + "";
            TNS.Onbellek.Remove("DEG_" + yil + "_" + cacheID);
            TNS.Onbellek.Insert("DEG_" + yil + "_" + cacheID, deger);
            //OrtakFonksiyonlar.HataDosyaYaz("cacheDEGI.txt", "Cache e yazıldı:" + cacheID + "*" + deger + "*", false);
            return deger;
        }

        public static void DegiskenDegerAta(int yil, string degiskenYol, string deger)
        {
            IDEGServis servisDEG = TNS.DEG.Arac.Tanimla();

            servisDEG.DegiskenDegerAta(yil, degiskenYol, deger);
            TNS.Onbellek.Remove("DEG_" + yil + "_" + degiskenYol);
        }
    }

    [Serializable()]
    public class Degisken
    {
        public int yil;
        public string yol;
        public string ad;
        public string deger;
        public string aciklama;

        public Degisken()
        {
            yil = 0;
            yol = "/";
            ad = "";
            deger = "";
            aciklama = "";
        }
    } // END CLASS DEFINITION Degisken

    [Serializable()]
    public class Degiskenler
    {
        public TNSCollection degisken;

        public Degiskenler()
        {
            degisken = new TNSCollection();
        }
    }

    [Serializable()]
    public class DegiskenVAN
    {
        public Degisken degisken;
        public Sonuc sonuc;

        public DegiskenVAN()
        {
            sonuc = new Sonuc();
            degisken = new Degisken();
        }
    }

    [Serializable()]
    public class DegiskenlerVAN
    {
        public TNSCollection degiskenler; //degisken
        public Sonuc sonuc;
        public TNSCollection yollar; //string

        public DegiskenlerVAN()
        {
            sonuc = new Sonuc();
            degiskenler = new TNSCollection();
            yollar = new TNSCollection();
        }
    }	// END CLASS DEFINITION DegiskenlerVAN}

    public interface IDEGServis
    {

        string DegiskenDegerBul(int yil, string degiskenYol);
        void DegiskenDegerAta(int yil, string degiskenYol, string deger);
    }
}