using System.Data;
using System.Xml;
using TNS.ATM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.ATM.IsKurallari
{
    /// <summary>
    /// Alacak Takip Genel İşlemler Sınıfı
    /// </summary>
    public class Genel
    {
        /// <summary>
        /// Döviz kurunu getiren fonksiyon
        /// </summary>
        /// <param name="pb">Para birimi bilgisi, ENUMParaBirimi tipinde sınıf</param>
        /// <param name="alis">Alış/Satış bilgisi, Bool tipinde sınıf </param>
        /// <returns>Double tipinde sınıf döndürür</returns>
        public static double DovizKurGetir(ENUMParaBirimi pb, bool alis)
        {
            string paraBirimi = string.Empty;
            if (pb == ENUMParaBirimi.DOLAR)
                paraBirimi = "USD";
            else if (pb == ENUMParaBirimi.EURO)
                paraBirimi = "EUR";
            else if (pb == ENUMParaBirimi.STERLIN)
                paraBirimi = "GBP";

            string tur = "ForexSelling";
            if (alis)
                tur = "ForexBuying";

            XmlTextReader rdr = new XmlTextReader("http://www.tcmb.gov.tr/kurlar/today.xml");
            XmlDocument MyXml = new XmlDocument();
            MyXml.Load(rdr);
            XmlNodeList MyNode = MyXml.SelectNodes("/Tarih_Date/Currency[@Kod ='" + paraBirimi + "']/" + tur);

            string kur = MyNode.Item(0).InnerXml.ToString().Replace(".", ",");
            return OrtakFonksiyonlar.ConvertToDbl(kur);
        }

        /// <summary>
        /// Yıl ve Dosya numarasına göre Alacak takip dosyasının son durumunu döndüren fonksiyon
        /// </summary>
        /// <param name="ve">VeriErişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="dosyaNo">Dosya numarası bilgisi, Int tipinde sınıf</param>
        /// <returns>Int tipinde sınıf döndürür</returns>
        public static int SonDurumDondur(VeriErisim ve, int yil, int dosyaNo)
        {
            TABATMDosya tabDosya = new TABATMDosya(ve);
            return tabDosya.SonDurumDondur(yil, dosyaNo);
        }

        /// <summary>
        /// Yıl, Dosya ve Sıra numarasına göre Taksitin ödenme durumunu dönen fonksiyon
        /// </summary>
        /// <param name="ve">VeriErişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="dosyaNo">Dosya bilgisi, Int tipinde sınıf</param>
        /// <param name="siraNo">Sıra numarası bilgisi Int tipinde sınıf</param>
        /// <returns>Bool tipinde sınıf döndürür</returns>
        public static bool TaksitOdendiMi(VeriErisim ve, int yil, int dosyaNo, int siraNo)
        {
            TABATMDosyaTaksit tabTaksit = new TABATMDosyaTaksit(ve);
            return tabTaksit.TaksitOdendiMi(yil, dosyaNo, siraNo);
        }

        /// <summary>
        /// Yıl ve dosya numarasına göre Taksit Sayısı döndüren fonksiyon
        /// </summary>
        /// <param name="ve">VeriErişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="dosyaNo">Dosya numarası bilgisi, Int tipinde sınıf</param>
        /// <returns>Int tipinde sınıf döndürür</returns>
        public static int TaksitSayisiDondur(VeriErisim ve, int yil, int dosyaNo)
        {
            TABATMDosyaTaksit tabTaksit = new TABATMDosyaTaksit(ve);
            return tabTaksit.TaksitSayisiDondur(yil, dosyaNo);
        }

        /// <summary>
        /// Yıl ve dosya numarasına göre faiz başlangıç tarihi döndüren fonksiyon
        /// </summary>
        /// <param name="ve">VeriErişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="dosyaNo">Dosya numarası bilgisi, Int tipinde sınıf</param>
        /// <returns>TNSDateTime tipinde sınıf döndürür</returns>
        public static TNSDateTime FaizBaslangicTarihiDondur(VeriErisim ve, int yil, int dosyaNo)
        {
            TABATMDosya tabDosya = new TABATMDosya(ve);
            return tabDosya.FaizBaslangicTarihiDondur(yil, dosyaNo);
        }

        /// <summary>
        /// Koda göre durum adı bilgisi döndüren fonksiyon
        /// </summary>
        /// <param name="kod">Kod bilgisi, Int tipinde sınıf</param>
        /// <returns>String tipinde sınıf döndürür</returns>
        public static string DurumAdiDondur(int kod)
        {
            string ad = "";
            if (kod > 0)
                ad = ATMServis.UzayYaziDegerGetir("ATMDURUM", kod.ToString(), true, "");

            return ad;
        }


        /// <summary>
        /// Yıl ve koda göre faiz türünün adının döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="kod">Kod bilgisi, Int tipinde sınıf</param>
        /// <returns>String tipinde sınıf döndürür</returns>
        public static string FaizTurAdiDondur(int yil, int kod)
        {
            string ad = "";
            if (yil > 0 && kod > 0)
                ad = ATMServis.UzayYaziDegerGetir("ATMFAIZTUR", yil.ToString() + "-" + kod.ToString(), true, "");

            return ad;
        }

        /// <summary>
        /// Yıl ve koda göre faiz türünün oranını döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="kod">Kod bilgisi, Int tipinde sınıf</param>
        /// <returns>String tipinde sınıf döndürür</returns>
        public static double FaizTurOranDondur(int yil, int kod)
        {
            double oran = 0.0;
            if (yil > 0 && kod > 0)
                oran = ATMServis.UzaySayiDegerGetir("ATMFAIZTUR", yil.ToString() + "-" + kod.ToString(), true, "ORAN");

            return oran;
        }

        /// <summary>
        /// Saymanlık koduna göre saymanlık adını döndüren fonksiyon
        /// </summary>
        /// <param name="saymanlikKodu">Saymanlık kodu bilgisi, Int tipinde sınıf</param>
        /// <returns>String tipinde sınıf döndürür</returns>
        public static string SaymanlikAdiDondur(int saymanlikKodu)
        {
            string saymanlikAdi = "";
            if (saymanlikKodu > 0)
                saymanlikAdi = ATMServis.UzayYaziDegerGetir("MUHMUHASEBE", saymanlikKodu.ToString(), true, "");

            return saymanlikAdi;
        }

        /// <summary>
        /// Saymanlık koduna göre saymanlık bilgisinin doğruluk durumunu döndüren fonksiyon
        /// </summary>
        /// <param name="saymanlikKodu">Saymanlık kodu bilgisi, Int tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public static Sonuc SaymanlikDogruMu(int saymanlikKodu)
        {
            Sonuc donusDeger = new Sonuc();
            string saymanlikAdi = SaymanlikAdiDondur(saymanlikKodu);
            if (saymanlikKodu <= 0 || saymanlikAdi == "")
            {
                donusDeger.hataStr = AlacakTakipResI.ATA049;
                donusDeger.islemSonuc = false;
                return donusDeger;
            }

            donusDeger.islemSonuc = true;
            donusDeger.hataStr = "";
            return donusDeger;
        }

        /// <summary>
        /// Saymanlık ve birim kodlarına göre birim adını döndüren fonksiyon
        /// </summary>
        /// <param name="saymanlikKodu">Saymanlık kodu bilgisi, Int tipinde sınıf</param>
        /// <param name="birimKodu">Birim kodu bilgisi, String tipinde sınıf</param>
        /// <returns>String tipinde sınıf döndürür</returns>
        public static string BirimAdiDondur(int saymanlikKodu, string birimKodu)
        {
            string birimAdi = "";
            if (saymanlikKodu > 0 && birimKodu != "")
                birimAdi = ATMServis.UzayYaziDegerGetir("MUHMUHASEBEBIRIM", birimKodu + "-" + saymanlikKodu.ToString(), true, "");

            return birimAdi;
        }

        /// <summary>
        /// Saymanlık ve Birim kodlarına göre birimin doğruluk durumunu döndüren fonksiyon
        /// </summary>
        /// <param name="saymanlikKodu">Saymanlık kodu bilgisi, Int tipinde sınıf</param>
        /// <param name="birimKodu">Birim kodu bilgisi, String tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public static Sonuc BirimDogruMu(int saymanlikKodu, string birimKodu)
        {
            Sonuc donusDeger = new Sonuc();
            string birimAdi = BirimAdiDondur(saymanlikKodu, birimKodu);
            if (birimKodu == "" || birimAdi == "")
            {
                donusDeger.hataStr = AlacakTakipResI.ATA050;
                donusDeger.islemSonuc = false;
                return donusDeger;
            }

            donusDeger.islemSonuc = true;
            donusDeger.hataStr = "";
            return donusDeger;
        }

        /// <summary>
        /// Kod bilgisine göre Dosya durumunu döndüren fonksiyon
        /// </summary>
        /// <param name="ve">VeriErişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kod">Kod bilgisi, Int tipinde sınıf</param>
        /// <returns>Bool tipinde sınıf döndürür</returns>
        public static bool DurumVarMi(VeriErisim ve, int kod)
        {
            TABATMDurum tabDurum = new TABATMDurum(ve);
            return tabDurum.DurumVarMi(kod);
        }

        /// <summary>
        /// Yıl ve dosya numarasına göre alacak takip dosyasının olma durumunu döndüren fonksiyon
        /// </summary>
        /// <param name="ve">VeriErişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="dosyaNo">Dosya numarası bilgisi, Int tipinde sınıf</param>
        /// <returns>Bool tipinde sınıf döndürür</returns>
        public static bool DosyaVarMi(VeriErisim ve, int yil, int dosyaNo)
        {
            TABATMDosya tabDosya = new TABATMDosya(ve);
            return tabDosya.DosyaVarMi(yil, dosyaNo);
        }

        /// <summary>
        /// Yıl ve koda göre faiz türü olma durumunu döndüren fonksiyon
        /// </summary>
        /// <param name="ve">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="kod">Kod bilgisi, Int tipinde sınıf</param>
        /// <returns>Bool tipinde sınıf döndürür</returns>
        public static bool FaizTurVarMi(VeriErisim ve, int yil, int kod)
        {
            TABATMFaizTur tabFaizTur = new TABATMFaizTur(ve);
            return tabFaizTur.FaizTurVarMi(yil, kod);
        }

        /// <summary>
        /// Aktif yılı döndüren fonksiyon
        /// </summary>
        /// <returns>Int tipinde sınıf döndürür</returns>
        public static int AktifYilDondur()
        {
            return System.DateTime.Today.Year;
        }
    }
}
