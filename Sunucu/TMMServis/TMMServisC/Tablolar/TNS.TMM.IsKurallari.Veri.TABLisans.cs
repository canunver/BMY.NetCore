//using System;
//using System.Data;
//using System.Web.Security;
//using System.Management;
//using TNS.Veri;
//using TNS.DEG;

//namespace TNS.TMM.IsKurallari.Veri
//{
//    /// <summary>
//    /// Taşınır lisans kontrolünü sağlayan fonksiyonların tutulduğu sınıf
//    /// </summary>
//    public class TABLisans : TABOrtak
//    {

//        /// <summary>
//        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
//        /// </summary>
//        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
//        public TABLisans(VeriErisim veriErisim) : base(veriErisim) { }

//        /// <summary>
//        /// Lisans kontrolü yaparak sonucunu döndüren fonksiyon
//        /// </summary>
//        /// <param name="ve">Veri Erişim bilgisini tutan, VeriErisim türünde nesne</param>
//        /// <param name="harcamaMi">Harcamaya göre kontrolün yapılıp yapılmayacağınu tutan, Bool türünde nesne</param>
//        /// <returns>Fonksiyonun çalışması sonucunda, Lisans Kontrolünden geçilmişse true geçilememişse false bilgisini tutan Bool türünde nesne döndürür</returns>
//        public static bool LisansKontrol(VeriErisim ve, bool harcamaMi)
//        {
//            string kurumLisans = System.Configuration.ConfigurationManager.AppSettings.Get("Stratek");
//            int harcamaBirimiSayisi = new TABHarcamaBirimi(ve).HarcamaBirimiSayisiDondur();
//            string islemciID = IslemciIdAl();
//            string hddID = HDDIdAl();

//            int ekleme = 1;
//            if (harcamaMi)
//                ekleme = 0;

//            int harcamaSayisiLisans = OrtakFonksiyonlar.ConvertToInt(kurumLisans.Substring(kurumLisans.Length - 3), 0);
//            if (kurumLisans == Şifrele(harcamaSayisiLisans.ToString(), islemciID, hddID) && harcamaBirimiSayisi < (harcamaSayisiLisans + ekleme))
//                return true;
//            else if (kurumLisans == Şifrele(harcamaSayisiLisans.ToString(), islemciID, hddID) && harcamaSayisiLisans == 999)
//                return true;
//            if (kurumLisans == ŞifreleKurumAd(harcamaSayisiLisans.ToString()) && harcamaBirimiSayisi < (harcamaSayisiLisans + ekleme))
//                return true;
//            else if (kurumLisans == ŞifreleKurumAd(harcamaSayisiLisans.ToString()) && harcamaSayisiLisans == 999)
//                return true;

//            return false;
//        }

//        /// <summary>
//        /// Parametre olarak verilen Lisans Miktarı, İşlemci ID ve HDD ID bilgilerini kullanarak,
//        /// Şifrelenmiş Lisans bilgisini döndüren fonksiyon
//        /// </summary>
//        /// <param name="lisansMiktar">Lisans Miktar bilgisini tutan, String türünde nesne</param>
//        /// <param name="islemciID">İşlemci ID bilgisini tutan, String türünde nesne</param>
//        /// <param name="hddID">HDD ID bilgisini tutan, String türünde nesne</param>
//        /// <returns>Fonksiyonun çalışması sonucunda, Şifrelenmiş Lisans bilgisini tutan, String türünde nesne döndürür</returns>
//        private static string Şifrele(string lisansMiktar, string islemciID, string hddID)
//        {
//            string lisans = string.Format(TasinirMalResI.TMM002, islemciID, hddID, lisansMiktar);
//            lisans = FormsAuthentication.HashPasswordForStoringInConfigFile(FormsAuthentication.HashPasswordForStoringInConfigFile(lisans, "sha1"), "sha1");
//            if (lisansMiktar.Trim().Length == 1)
//                lisansMiktar = "00" + lisansMiktar;
//            else if (lisansMiktar.Trim().Length == 2)
//                lisansMiktar = "0" + lisansMiktar;
//            lisans += lisansMiktar;
//            return lisans;
//        }

//        /// <summary>
//        /// Parametre olarak verilen Lisans Miktar bilgisine göre, Lisans bilgisini şifreleyip döndüren fonksiyon
//        /// </summary>
//        /// <param name="lisansMiktar">Lisans Miktar bilgisini tutan, String türünde nesne döndürür</param>
//        /// <returns>Fonksiyonun çalışması sonucunda, Şifrelenmiş Lisans bilgisini tutan, String türünde nesne döndürür</returns>
//        private static string ŞifreleKurumAd(string lisansMiktar)
//        {
//            string kurumAdi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KURUMAD");
//            string lisans = string.Format(TasinirMalResI.TMM003, kurumAdi, lisansMiktar);
//            lisans = FormsAuthentication.HashPasswordForStoringInConfigFile(FormsAuthentication.HashPasswordForStoringInConfigFile(lisans, "sha1"), "sha1");
//            if (lisansMiktar.Trim().Length == 1)
//                lisansMiktar = "00" + lisansMiktar;
//            else if (lisansMiktar.Trim().Length == 2)
//                lisansMiktar = "0" + lisansMiktar;
//            lisans += lisansMiktar;
//            return lisans;
//        }

//        /// <summary>
//        /// İşlemcinin ID'sini alan fonksiyon
//        /// </summary>
//        /// <returns>Fonksiyonun çalışması sonucunda, İşlemcinin ID'sini tutan, String türünde nesne döndürür</returns>
//        private static string IslemciIdAl()
//        {
//            ManagementObjectSearcher objMOS = new ManagementObjectSearcher("Select ProcessorID From Win32_Processor");
//            ManagementObjectCollection objMOC = objMOS.Get();

//            string[] islemciler = new string[objMOC.Count];
//            int i = 0;
//            foreach (ManagementObject objMO in objMOC)
//            {
//                bool farkli = false;
//                if (!string.IsNullOrEmpty((string)objMO["ProcessorID"]))
//                {
//                    farkli = true;
//                    for (int j = 0; j < i; j++)
//                    {
//                        if ((string)objMO["ProcessorID"] == islemciler[j])
//                        {
//                            farkli = false;
//                            break;
//                        }
//                    }
//                }

//                if (farkli)
//                    islemciler[i] = (string)objMO["ProcessorID"];

//                objMO.Dispose();
//                i++;
//            }

//            string islemciID = string.Empty;
//            for (int j = 0; j < islemciler.Length; j++)
//            {
//                if (!string.IsNullOrEmpty(islemciler[j]))
//                    islemciID += islemciler[j];
//            }
//            objMOS.Dispose();
//            objMOS = null;
//            objMOC.Dispose();
//            objMOC = null;
//            return islemciID;
//        }

//        /// <summary>
//        /// Hard Disk'in Id'sini alan fonksiyon
//        /// </summary>
//        /// <returns>Fonksiyonun çalışması sonucunda, HDD ID'sini tutan, String türünde nesne döndürür</returns>
//        private static string HDDIdAl()
//        {
//            ManagementObjectSearcher objMOS = new ManagementObjectSearcher("Select VolumeSerialNumber From Win32_LogicalDisk");
//            ManagementObjectCollection objMOC = objMOS.Get();
//            string hddID = string.Empty;
//            foreach (ManagementObject objMO in objMOC)
//            {
//                if (!string.IsNullOrEmpty((string)objMO["VolumeSerialNumber"]))
//                {
//                    hddID = (string)objMO["VolumeSerialNumber"];
//                    objMO.Dispose();
//                    break;
//                }
//            }
//            objMOS.Dispose();
//            objMOS = null;
//            objMOC.Dispose();
//            objMOC = null;
//            return hddID;
//        }

//        //private static string IslemciIdAl()
//        //{
//        //    ManagementObjectSearcher objMOS = new ManagementObjectSearcher("Select ProcessorID From Win32_Processor");
//        //    ManagementObjectCollection objMOC = objMOS.Get();
//        //    string islemciID = string.Empty;
//        //    foreach (ManagementObject objMO in objMOC)
//        //    {
//        //        if (!string.IsNullOrEmpty((string)objMO["ProcessorID"]))
//        //        {
//        //            islemciID = (string)objMO["ProcessorID"];
//        //            objMO.Dispose();
//        //            break;
//        //        }
//        //    }
//        //    objMOS.Dispose();
//        //    objMOS = null;
//        //    objMOC.Dispose();
//        //    objMOC = null;
//        //    return islemciID;
//        //}

//        //private static string HDDIdAl()
//        //{
//        //    ManagementObjectSearcher objMOS = new ManagementObjectSearcher("Select VolumeSerialNumber From Win32_LogicalDisk");
//        //    ManagementObjectCollection objMOC = objMOS.Get();
//        //    string hddID = string.Empty;
//        //    foreach (ManagementObject objMO in objMOC)
//        //    {
//        //        if (!string.IsNullOrEmpty((string)objMO["VolumeSerialNumber"]))
//        //        {
//        //            hddID = (string)objMO["VolumeSerialNumber"];
//        //            objMO.Dispose();
//        //            break;
//        //        }
//        //    }
//        //    objMOS.Dispose();
//        //    objMOS = null;
//        //    objMOC.Dispose();
//        //    objMOC = null;
//        //    return hddID;
//        //}
//    }
//}
