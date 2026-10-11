using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BKD;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// BHMv2ServisV projesindeki tüm nesneler tarafından ortak kullanılan fonksiyonları tutan sınıf.
    /// </summary>
    public class TABBHMGENEL
    {
        public static string NoktaAt(string noktali)
        {
            if (string.IsNullOrWhiteSpace(noktali)) return "";
            return noktali.Replace(".", "");
        }

        public static string NoktaAt(string[] dizi, int ind)
        {
            return NoktaAt(OrtakFonksiyonlar.DiziElemanAl(dizi, ind, ""));
        }
        
        /// <summary>
        /// Tertip yapısında verilen sıradaki alan için alan sorgusu oluşturmak üzere alan bazındaki alan koşul ol yordamını çağıran yordam.
        /// </summary>
        /// <param name="tabloAd">Alan adının başına eklenmesi isteniyorsa verilen tablo adı</param>
        /// <param name="alanAd">Tertip içindeki verilen sıradaki alanın veri tabanındaki alan adı</param>
        /// <param name="kistasTertip">Verilen sıradaki alanı için, alan koşulu oluşturulacak tertip nesnesi</param>
        /// <param name="siraNo">Alan koşulu oluşturulacak alanın değerinin, tertip içindeki sıra numarası</param>
        /// <returns>Verilen alan için oluşturulan ve sql cümlesindeki where kısmına eklenebilen koşul dizgisi döndürülür.</returns>
        public static string AlanKosulOl(string tabloAd, string alanAd, Tertip kistasTertip, int siraNo)
        {
            if (siraNo >= kistasTertip.kodlar.Length)
                return "";
            return VeriErisim.AlanKosulOl(tabloAd, alanAd, kistasTertip.kodlar[siraNo], " AND ");
        }

        internal static string TertipDurumKosulEkle(string tabloAd, string alanAd, ENUMTertipDurum[] tertipDurumlar)
        {
            if (tertipDurumlar == null || tertipDurumlar.Length == 0) return "";
            string icKosul = "";
            for (int i = 0; i < tertipDurumlar.Length; i++)
            {
                if (!string.IsNullOrEmpty(icKosul)) icKosul += " OR ";
                icKosul += VeriErisim.KosulOl(tabloAd, alanAd, "=", (int)tertipDurumlar[i]);
            }
            return " AND (" + icKosul + ")";
        }

        /// <summary>
        /// Birden N'e sırala
        /// </summary>
        /// <param name="n">Sıralamanın biteceği üst sınır.</param>
        /// <returns>1'den verilen sayısa kadar (1,2,3..,n) şeklinde string döndürülür.</returns>
        internal static string BirdenNeSirala(int n)
        {
            string donusDeger = "";
            for (int i = 1; i <= n; i++)
            {
                donusDeger += i.ToString();
                if (i < n)
                    donusDeger += ", ";
            }
            return donusDeger;
        }
    }
}

/*

        //public static string TABLOADDISBHMPEBFON = "ButceHazirlik.dbo.PEBFON";

        //public static string TABLOADDISBHMABSEKO = "ButceHazirlik.dbo.ABSEKO";

        //public static string VIEWHTSAYFABILGI = "BHMHTSAYFABILGIVIEW";

        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        //public TABBHMGENEL(VeriErisim veriErisim)
        //    : base(veriErisim)
        //{
        //}

        //public string BelgeNoAl(string sayacKod, string sayacFormat)
        //{
        //    string sayacDeger = "";
        //    string sql = "";
        //    bool dosyaYok = true;

        //    DateTime t = DateTime.Now;
        //    while (dosyaYok)
        //    {
        //        try
        //        {
        //            dosyaYok = SayacDosyaYarat(sayacKod);
        //            if (t.AddSeconds(5) < DateTime.Now)//5 saniye dönerse çıksın
        //                dosyaYok = false;
        //        }
        //        catch { }
        //    }

        //    lock (sayacKod)
        //    {
        //        sql = "SELECT SAYAC FROM BHMSAYAC WHERE " + VeriErisim.KosulOl("", "KOD", "=", sayacKod);
        //        IDataReader r = veriErisim.DataReaderDondur(sql);

        //        if (veriErisim.hata == "")
        //        {
        //            if (r.Read())
        //                sayacDeger = VeriErisim.DegerAl(r, 0, "");

        //            r.Close();

        //            if (sayacDeger == "")
        //                sayacDeger = "0";

        //            sayacDeger = (Convert.ToInt32(sayacDeger) + 1).ToString();
        //            sayacDeger = sayacDeger.PadLeft(sayacFormat.Length, sayacFormat[0]);

        //            if (Convert.ToInt32(sayacDeger) == 1)
        //                sql = "INSERT INTO BHMSAYAC (KOD,SAYAC) VALUES (" + VeriErisim.DegerYap(sayacKod) + "," + VeriErisim.DegerYap(sayacDeger) + ")";
        //            else
        //                sql = "UPDATE BHMSAYAC SET SAYAC=" + VeriErisim.DegerYap(sayacDeger) + " WHERE KOD=" + VeriErisim.DegerYap(sayacKod);

        //            if (!KomutCalistir(sql, 0).islemSonuc)
        //            {
        //                SayacDosyaSil(sayacKod);
        //                return "";
        //            }
        //        }
        //        else
        //        {
        //            SayacDosyaSil(sayacKod);
        //            return "";
        //        }

        //        SayacDosyaSil(sayacKod);
        //        return sayacDeger;
        //    }
        //}

        //bool SayacDosyaYarat(string sayacKod)
        //{
        //    if (System.IO.File.Exists(TNS.OrtakFonksiyonlar.GeciciYolBul() + sayacKod))
        //        return true;

        //    try
        //    {
        //        System.IO.FileStream fs = System.IO.File.Create(TNS.OrtakFonksiyonlar.GeciciYolBul() + sayacKod);
        //        fs.Close();
        //        return false;
        //    }
        //    catch
        //    {
        //        return true;
        //    }
        //}

        //bool SayacDosyaSil(string sayacKod)
        //{
        //    try
        //    {
        //        System.IO.File.Delete(TNS.OrtakFonksiyonlar.GeciciYolBul() + sayacKod);
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}




*/