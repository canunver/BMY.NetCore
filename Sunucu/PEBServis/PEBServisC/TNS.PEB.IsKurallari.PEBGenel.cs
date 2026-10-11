using System;
using System.Data;
using TNS.DEG;
using TNS.KYM;
using TNS.Veri;
using TNS.BKD;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Performans bütçe modülü iş kuralları sınıflarının ortak kullandığı yordamları tutan sınıf
    /// </summary>
    public class PEBGenel
    {
        /// <summary>
        /// Parametre olarak verilen kurumsal kodu, kurumsal kod maskesine göre noktalayıp döndüren yordam
        /// </summary>
        /// <param name="kurkod">Noktalanacak kurumsal kod</param>
        /// <returns>Noktalanmış kurumsal kod döndürülür.</returns>
        public static string KurKodNoktala(string kurkod)
        {
            return OrtakFonksiyonlar.KodNoktaKoy(KurKodYapiDondur(), kurkod);
        }

        /// <summary>
        /// Parametre olarak verilen fonksiyonel kodu, fonksiyonel kod maskesine göre noktalayıp döndüren yordam
        /// </summary>
        /// <param name="fonkod">Noktalanacak fonksiyonel kod</param>
        /// <returns>Noktalanmış fonksiyonel kod döndürülür.</returns>
        public static string FonKodNoktala(string fonkod)
        {
            return OrtakFonksiyonlar.KodNoktaKoy(FonKodYapiDondur(), fonkod);
        }

        /// <summary>
        /// Parametre olarak verilen ekonomik kodu, ekonomik kod maskesine göre noktalayıp döndüren yordam
        /// </summary>
        /// <param name="ekokod">Noktalanacak ekonomik kod</param>
        /// <returns>Noktalanmış ekonomik kod döndürülür.</returns>
        public static string EkoKodNoktala(string ekokod)
        {
            return OrtakFonksiyonlar.KodNoktaKoy(EkoKodYapiDondur(), ekokod);
        }

        /// <summary>
        /// Parametre olarak verilen peb kodu, peb kod maskesine göre noktalayıp döndüren yordam
        /// </summary>
        /// <param name="pebKod">Noktalanacak peb kod</param>
        /// <returns>Noktalanmış peb kod döndürülür.</returns>
        public static string PebKodNoktala(string pebKod)
        {
            return OrtakFonksiyonlar.KodNoktaKoy(PebKodYapiDondur(), pebKod);
        }

        /// <summary>
        /// Bütçe hazırlık yapılan yılı bulup döndüren yordam
        /// </summary>
        /// <returns>Bütçe hazırlık yılı döndürülür.</returns>
        public static int ButceHazirlikYilBul()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "/BUTCEHAZIRLIK"), 0);
        }

        /// <summary>
        /// Kurumsal kod maskesini Degisken.xml dosyasından bulup döndüren yordam
        /// </summary>
        /// <returns>Kurumsal kod maskesi döndürülür.</returns>
        public static string KurKodYapiDondur()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD1/KODYAPI");
        }

        /// <summary>
        /// Fonksiyonel kod maskesini Degisken.xml dosyasından bulup döndüren yordam
        /// </summary>
        /// <returns>Fonksiyonel kod maskesi döndürülür.</returns>
        public static string FonKodYapiDondur()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD2/KODYAPI");
        }

        /// <summary>
        /// Finansman kod maskesini Degisken.xml dosyasından bulup döndüren yordam
        /// </summary>
        /// <returns>Finansman kod maskesi döndürülür.</returns>
        public static string FinKodYapiDondur()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD3/KODYAPI");
        }

        /// <summary>
        /// Ekonomik kod maskesini Degisken.xml dosyasından bulup döndüren yordam
        /// </summary>
        /// <returns>Ekonomik kod maskesi döndürülür.</returns>
        public static string EkoKodYapiDondur()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD4/KODYAPI");
        }

        /// <summary>
        /// Bütçe alt faaliyet kodu maskesini Degisken.xml dosyasından bulup döndüren yordam
        /// </summary>
        /// <returns>Bütçe alt faaliyet kodu maskesi döndürülür.</returns>
        public static string PebKodYapiDondur()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD14/KODYAPI");
        }

        /// <summary>
        /// Verilen kurumsal kodun veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Kontrol edilecek kurumsal kod</param>
        /// <returns>Kurumsal kod kayıtlı ise true, değilse false döndürülür.</returns>
        public static bool KurKodVarMi(int yil, string kurKod)
        {
            string kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", yil.ToString() + "-" + kurKod, true, "");
            if (string.IsNullOrEmpty(kurAd))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Verilen fonksiyonel kodun veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="fonKod">Kontrol edilecek fonksiyonel kod</param>
        /// <returns>Fonksiyonel kod kayıtlı ise true, değilse false döndürülür.</returns>
        public static bool FonKodVarMi(int yil, string fonKod)
        {
            string fonAd = PEBServis.UzayYaziDegerGetir("ABSFON", yil.ToString() + "-" + fonKod, true, "");
            if (string.IsNullOrEmpty(fonAd))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Verilen bütçe alt faaliyet kodunun veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="fonKod">Kontrol edilecek bütçe alt faaliyet kodu</param>
        /// <returns>Bütçe alt faaliyet kodu kayıtlı ise true, değilse false döndürülür.</returns>
        public static bool PEBFonKodVarMi(int yil, string fonKod)
        {
            string fonAd = PEBServis.UzayYaziDegerGetir("PEBFON", yil.ToString() + "-" + fonKod, true, "");
            if (string.IsNullOrEmpty(fonAd))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Verilen finansman kodun veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="finKod">Kontrol edilecek finansman kod</param>
        /// <returns>Finansman kod kayıtlı ise true, değilse false döndürülür.</returns>
        public static bool FinKodVarMi(int yil, string finKod)
        {
            string finAd = PEBServis.UzayYaziDegerGetir("ABSFIN", yil.ToString() + "-" + finKod, true, "");
            if (string.IsNullOrEmpty(finAd))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Verilen ekonomik kodun veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="ekoKod">Kontrol edilecek ekonomik kod</param>
        /// <returns>Ekonomik kod kayıtlı ise true, değilse false döndürülür.</returns>
        public static bool EkoKodVarMi(int yil, string ekoKod)
        {
            string ekoAd = PEBServis.UzayYaziDegerGetir("ABSEKO", yil.ToString() + "-" + ekoKod, true, "");
            if (string.IsNullOrEmpty(ekoAd))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Verilen stratejik hedefin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="donemByil">Dönem başlangıç yılı</param>
        /// <param name="hedefKod">Stratejik hedef programatik kodu</param>
        /// <returns>Stratejik hedef kayıtlı ise true, değilse false döndürülür.</returns>
        public static bool StratejikHedefVarMi(int donemByil, int hedefKod)
        {
            string strHedef = PEBServis.UzayYaziDegerGetir("STRHEDEF", donemByil.ToString() + "-" + hedefKod.ToString(), true, "");
            if (string.IsNullOrEmpty(strHedef))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Verilen performans hedefin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="hedefKod">Performans hedefin programatik kodu</param>
        /// <returns>Performans hedef kayıtlı ise true, değilse false döndürülür.</returns>
        public static bool PerformansHedefiVarMi(VeriErisim ve, int yil, string kurKod, int hedefKod)
        {
            TABPerHedef tPHedef = new TABPerHedef(ve);
            return tPHedef.HedefVarMi(yil, kurKod, hedefKod);
        }

        /// <summary>
        /// Verilen performans göstergesinin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="gostergeKod">Performans göstergesi programatik kodu</param>
        /// <param name="kullanimSekli">ENUMGostergeKullanimSekli listesindeki seçeneklerden biri olmalıdır.</param>
        /// <returns>Performans göstergesi kayıtlı ise true, değilse false döndürülür.</returns>
        public static bool GostergeVarMi(VeriErisim ve, int gostergeKod, int kullanimSekli)
        {
            TABGosterge tG = new TABGosterge(ve);
            ve.hata = "";
            bool sonuc = tG.GostergeVarMi(gostergeKod, kullanimSekli);
            if (!string.IsNullOrEmpty(ve.hata)) throw new Exception(ve.hata + ve.baglantiAd);
            return sonuc;
        }

        /// <summary>
        /// Verilen faaliyet/projenin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="fpKod">Faaliyet/projenin programatik kodu</param>
        /// <returns>Faaliyet/proje kayıtlı ise true, değilse false döndürülür.</returns>
        public static bool FaaliyetProjeVarMi(VeriErisim ve, int yil, int fpKod)
        {
            TABFaaliyetProje tFP = new TABFaaliyetProje(ve);
            return tFP.FPVarMi(yil, fpKod);
        }

        /// <summary>
        /// Verilen sabit maliyetin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="smKod">Sabit maliyetin programatik kodu</param>
        /// <returns>Sabit maliyet kayıtlı ise true, değilse false döndürülür.</returns>
        public static bool SabitMaliyetVarMi(VeriErisim ve, int yil, int smKod)
        {
            TABSabitMaliyet tSM = new TABSabitMaliyet(ve);
            return tSM.SMVarMi(yil, smKod);
        }

        /// <summary>
        /// Verilen dairenin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="daire">Daire bilgilerini tutan nesne</param>
        /// <returns>Daire kayıtlı ise true, değilse false döndürülür.</returns>
        public static bool DaireVarMi(VeriErisim ve, Daire daire)
        {
            TABDaire tDaire = new TABDaire(ve);
            return tDaire.DaireVarMi(daire);
        }

        /// <summary>
        /// Verilen operasyonel plan alt faaliyetinin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="af">Operasyonel plan alt faaliyetinin bilgilerini tutan nesne</param>
        /// <returns>Operasyonel plan alt faaliyeti kayıtlı ise true, değilse false döndürülür.</returns>
        public static bool AltFaaliyetVarMi(VeriErisim ve, AltFaaliyet af)
        {
            TABAltFaaliyet tAF = new TABAltFaaliyet(ve);
            return tAF.AltFaaliyetVarMi(af);
        }


        //public static bool PersonelVarMi(VeriErisim ve, string tcKimlikNo)
        //{
        //    TABPersonel tPersonel = new TABPersonel(ve);
        //    return tPersonel.PersonelVarMi(tcKimlikNo);
        //}

        //public static bool GostergeVarMi(VeriErisim ve, int gostergeKod)
        //{
        //    TABGosterge tG = new TABGosterge(ve);
        //    return tG.GostergeVarMi(gostergeKod);
        //}

        //public static bool PerformansHedefiKullanilmisMi(VeriErisim ve, int yil, string kurKod, int hedefKod)
        //{
        //    TABPerHedef tPHedef = new TABPerHedef(ve);
        //    return tPHedef.HedefKullanilmisMi(yil, kurKod, hedefKod);
        //}

        //public static Sonuc TertipKontrol(int yil, string kurKod, Tertip tertip)
        //{
        //    string hata = string.Empty;
        //    if (string.IsNullOrEmpty(tertip.kodlar[0]))
        //        hata += PerformansButceResI.PEB048 + "<br>";
        //    else if (!KurKodVarMi(yil, tertip.kodlar[0]))
        //        hata += string.Format(PerformansButceResI.PEB049, tertip.kodlar[0]) + "<br>";
        //    else if (tertip.kodlar[0].Replace(".", "").Length != KurKodYapiDondur().Replace(".", "").Length)
        //        hata += string.Format(PerformansButceResI.PEB050, tertip.kodlar[0]) + "<br>";
        //    //Kural yanlış, örneğin Personel Genel Müdürlüğü 12.01.00.05 (Merkez)
        //    //ve 12.01.00.62 (Taşra) bu kurala takılıyor ve böyle bir tertip kullanılabilmeli
        //    //else if (!((kurKod.Replace(".", "").EndsWith("00") && tertip.kodlar[0].Replace(".", "").StartsWith(kurKod.Replace(".", "").Substring(0, kurKod.Replace(".", "").Length - 2)))
        //    //        || tertip.kodlar[0].Replace(".", "") == kurKod.Replace(".", "")))
        //    //    hata += tertip.kodlar[0] + " Kurumsal Kodu " + kurKod + " Kodlu Birime ait değil.<br>";

        //    if (string.IsNullOrEmpty(tertip.kodlar[1]))
        //        hata += PerformansButceResI.PEB051 + "<br>";
        //    else if (!FonKodVarMi(yil, tertip.kodlar[1]))
        //        hata += string.Format(PerformansButceResI.PEB052, tertip.kodlar[1]) + "<br>";
        //    else if (tertip.kodlar[1].Replace(".", "").Length != FonKodYapiDondur().Replace(".", "").Length)
        //        hata += string.Format(PerformansButceResI.PEB053, tertip.kodlar[1]) + "<br>";

        //    if (string.IsNullOrEmpty(tertip.kodlar[2]))
        //        hata += PerformansButceResI.PEB054 + "<br>";
        //    else if (!FinKodVarMi(yil, tertip.kodlar[2]))
        //        hata += string.Format(PerformansButceResI.PEB055, tertip.kodlar[2]) + "<br>";
        //    else if (tertip.kodlar[2].Replace(".", "").Length != FinKodYapiDondur().Replace(".", "").Length)
        //        hata += string.Format(PerformansButceResI.PEB056, tertip.kodlar[2]) + "<br>";

        //    if (string.IsNullOrEmpty(tertip.kodlar[3]))
        //        hata += PerformansButceResI.PEB057 + "<br>";
        //    else if (!EkoKodVarMi(yil, tertip.kodlar[3]))
        //        hata += string.Format(PerformansButceResI.PEB058, tertip.kodlar[3]) + "<br>";
        //    else if (tertip.kodlar[3].Replace(".", "").Length != EkoKodYapiDondur().Replace(".", "").Length)
        //        hata += string.Format(PerformansButceResI.PEB059, tertip.kodlar[3]) + "<br>";

        //    Sonuc sonuc = new Sonuc();
        //    if (!string.IsNullOrEmpty(hata))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = hata;
        //    }
        //    return sonuc;
        //}
    }
}