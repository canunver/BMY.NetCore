using System;
using System.Data;
using System.Text;
using TNS.BKD;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Taşınır mal modülü iş kuralları sınıflarının ortak kullandığı yordamları tutan sınıf
    /// </summary>
    public class Genel
    {
        /// <summary>
        /// Kullanılan veri tabanının ne olduğu bilgisini yapılandırma dosyasından okuyup tutan değişken
        /// </summary>
        private static string veriTabani = System.Configuration.ConfigurationManager.AppSettings.Get("BaglantiSatiriTMM.Data");

        /// <summary>
        /// RFID entegrasyonunun olup olmadığını tutan değişken
        /// </summary>
        public static bool rfIdVarMi = System.Configuration.ConfigurationManager.AppSettings.Get("RFID") == "1" ? true : false;

        /// <summary>
        /// RFID no ile SicilNo farklı ise
        /// </summary>
        public static bool tasinirSicilNoRFIDFarkli = System.Configuration.ConfigurationManager.AppSettings.Get("TasinirSicilNoRFIDFarkli") == "1" ? true : false;

        /// <summary>
        /// Verilen kriterlere karşılık gelen demirbaşın programatik sicil numarasını bulan yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebeKod">Muhasebe birimi</param>
        /// <param name="harcamaKod">Harcama birimi</param>
        /// <param name="gorSicilNo">Sicil numarası</param>
        /// <returns>Programatik sicil numarası döndürülür.</returns>
        public static int PrSicilNoGetir(int yil, string muhasebeKod, string harcamaKod, string gorSicilNo, decimal birimFiyat = 0)
        {
            //return (int)TMMServis.UzaySayiDegerGetir("TASPRSICILGETIR", muhasebeKod + "-" + harcamaKod + "-" + gorSicilNo, true, "");

            SicilNoHareket sc = new SicilNoHareket();
            sc.muhasebeKod = muhasebeKod;
            sc.harcamaBirimKod = harcamaKod;
            sc.sicilNo = gorSicilNo;
            sc.fiyat = birimFiyat;

            ObjectArray liste = TMMSicilNoHareket.AmbardakiSicilNolar(null, null, sc);

            foreach (SicilNoHareket ssc in liste.objeler)
                return ssc.prSicilNo;

            //Eğer sicil bulamaz ise
            return -1;
        }

        /// <summary>
        /// Verilen kriterlere karşılık gelen demirbaşın sicil numarasını bulan yordam
        /// </summary>
        /// <param name="muhasebeKod">Muhasebe birimi</param>
        /// <param name="harcamaKod">Harcama birimi</param>
        /// <param name="prSicilNo">Programatik sicil numarası</param>
        /// <returns>Sicil numarası döndürülür.</returns>
        public static string GorSicilNoGetir(string muhasebeKod, string harcamaKod, int prSicilNo)
        {
            return TMMServis.UzayYaziDegerGetir("TASSICILGETIR", muhasebeKod + "-" + harcamaKod.Replace(".", "") + "-" + prSicilNo.ToString(), true, "");
        }

        /// <summary>
        /// Parametre olarak verilen demirbaş ambarda mı kontrolü yapan yordam
        /// </summary>
        /// <param name="muhasebeKod">Muhasebe birimi</param>
        /// <param name="harcamaKod">Harcama birimi</param>
        /// <param name="ambarKod">Ambar kodu</param>
        /// <param name="prSicilNo">Programatik sicil numarası</param>
        /// <returns>Demirbaş, ambarda ise boşluk değilse hata mesajı döndürülür.</returns>
        public static ObjectArray SicilNoluMalzemeAmbardami(VeriErisim veriErisim, string muhasebeKod, string harcamaKod, string ambarKod, string prSicilNoListe)
        {
            if (muhasebeKod == "" || harcamaKod == "" || ambarKod == "")
                return new ObjectArray();

            SicilNoHareket sicilNo = new SicilNoHareket();
            sicilNo.muhasebeKod = muhasebeKod;
            sicilNo.harcamaBirimKod = harcamaKod;
            sicilNo.ambarKod = ambarKod;
            //sicilNo.prSicilNo = prSicilNo;
            sicilNo.sorguPrSicilNoListe = prSicilNoListe;

            return TMMSicilNoHareket.AmbardakiSicilNolar(veriErisim, new Kullanici(), sicilNo);

            //if (aSicilNolar.objeler.Count > 0)
            //    return "";
            //else
            //    return TasinirMalResI.TMM090 + "<br>";
        }

        /// <summary>
        /// Verilen demirbaşın sistemde olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcama">Harcama birimi</param>
        /// <param name="sicilNo">Sicil numarası</param>
        /// <returns>Demirbaş varsa boşluk, yoksa hata döndürülür.</returns>
        public static string KontrolSicilNo(int yil, string muhasebe, string harcama, string sicilNo)
        {
            string hata = string.Empty;

            if (!string.IsNullOrEmpty(sicilNo))
            {
                if (TMMServis.UzaySayiDegerGetir("TASPRSICILGETIR", muhasebe + "-" + harcama.Replace(".", "") + "-" + sicilNo.Replace(".", ""), true, "") <= 0)
                    hata += string.Format(TasinirMalResI.TMM091, sicilNo) + "<br>";
            }

            return hata;
        }

        /// <summary>
        /// RFID numarası verilen demirbaşın programatik sicil numarasını döndüren yordam
        /// </summary>
        /// <param name="rfid">Demirbaşın RFID numarası</param>
        /// <returns>Demirbaşın programatik sicil numarası döndürülür.</returns>
        public static int RFIDPrSicilNoDondur(int rfid)
        {
            if (Genel.rfIdVarMi)
            {
                if (rfid > 0)
                    return (int)TMMServis.UzaySayiDegerGetir("TASSICILNORFIDPRSICILNO", rfid.ToString(), true, "");
            }
            return 0;
        }

        /// <summary>
        /// Programatik sicil numarası verilen demirbaşın RFID numarasını döndüren yordam
        /// </summary>
        /// <param name="prSicilNo">Demirbaşın programatik sicil numarası</param>
        /// <returns>Demirbaşın RFID numarası döndürülür.</returns>
        public static int PrSicilNoRFIDDondur(int prSicilNo)
        {
            try
            {
                if (prSicilNo > 0)
                    return (int)TMMServis.UzaySayiDegerGetir("TASSICILNORFID", prSicilNo.ToString(), true, "");
            }
            catch
            {
                return 0;
            }

            return 0;
        }

        /// <summary>
        /// Parametre olarak verilen taşınır hesap planını kontrol eden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hesapPlanKod">Taşınır hesap planı kodu</param>
        /// <param name="satir">Taşınır hesap planının geçtiği satır numarası</param>
        /// <param name="altVarmiKontrol">Detay kodları var mı kontrolü yapılsın mı bilgisi</param>
        /// <param name="kapaliHesapKodBak">Kullanılmayan taşınır hesap planları da kontrol edilsin mi bilgisi</param>
        /// <returns>Hata varsa hata bilgisi, yoksa boşluk döndürülür.</returns>
        public static string KontrolHesapPlan(Kullanici kullanan, string hesapPlanKod, int satir, bool altVarmiKontrol, bool kapaliHesapKodBak)
        {
            string hata = string.Empty;

            //Hesap kodu detay olmak zorunda olmama zorunluluğu kaldırılanbilsin
            int detayHesapZorunlulugu = OrtakFonksiyonlar.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings.Get("TasinirDetayHesapKullanmasin") + "", 0);

            if (!string.IsNullOrEmpty(hesapPlanKod))
            {
                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.hesapKod = hesapPlanKod.Replace(".", "");
                if (kapaliHesapKodBak)
                    hs.kullanilmiyor = 2;//hesabın Açık,Kapali olmasına bakma

                Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABTASHesapPlani);

                TMMHesapPlani hp = new TMMHesapPlani();
                ObjectArray bilgiHp = hp.Listele(kullanan, hs, new Sayfalama());
                string hesapKodNoktali = hs.hesapKod;
                hesapKodNoktali = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), hesapKodNoktali);

                if (!hesapKodNoktali.StartsWith("630"))
                {
                    if (bilgiHp.ObjeSayisi < 1)
                        hata += string.Format(TasinirMalResI.TMM092, satir.ToString(), hesapKodNoktali) + "<br>";
                    else if (!((HesapPlaniSatir)bilgiHp[0]).detay && altVarmiKontrol && detayHesapZorunlulugu == 0)
                        hata += string.Format(TasinirMalResI.TMM093, satir.ToString(), hesapKodNoktali) + "<br>";
                    else if (((HesapPlaniSatir)bilgiHp[0]).seviye < 2)
                        hata += string.Format(TasinirMalResI.TMM094, satir.ToString(), hesapKodNoktali) + "<br>";
                }
            }
            else
                hata += string.Format(TasinirMalResI.TMM095, satir.ToString()) + "<br>";

            return hata;
        }

        /// <summary>
        /// Parametre olarak verilen muhasebe birimi, harcama birimi
        /// ve ambar sistemde kayıtlı mı kontrolünü yapan yordam
        /// </summary>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcama">Harcama birimi</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <returns>Hata varsa hata bilgisi, yoksa boşluk döndürülür.</returns>
        public static string KontrolMuhasebeHarcamaAmbar(string muhasebe, string harcama, string ambar)
        {
            string hata = string.Empty;

            if (!string.IsNullOrEmpty(muhasebe))
            {
                if (string.IsNullOrEmpty(TMMServis.UzayYaziDegerGetir("TASMUHASEBE", muhasebe, true, "")))
                    hata += string.Format(TasinirMalResI.TMM096, muhasebe) + "<br>";
            }

            if (!string.IsNullOrEmpty(muhasebe) && !string.IsNullOrEmpty(harcama))
            {
                if (string.IsNullOrEmpty(TMMServis.UzayYaziDegerGetir("TASHARCAMABIRIM", muhasebe + "-" + harcama, true, "")))
                    hata += string.Format(TasinirMalResI.TMM097, muhasebe, harcama) + "<br>";
            }

            if (!string.IsNullOrEmpty(muhasebe) && !string.IsNullOrEmpty(harcama) && !string.IsNullOrEmpty(ambar))
            {
                if (string.IsNullOrEmpty(TMMServis.UzayYaziDegerGetir("TASAMBAR", muhasebe + "-" + harcama + "-" + ambar, true, "")))
                    hata += string.Format(TasinirMalResI.TMM098, muhasebe, harcama, ambar) + "<br>";
            }

            return hata;
        }

        /// <summary>
        /// Kontrols the ambar.
        /// </summary>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="harcama">The harcama.</param>
        /// <param name="ambar">The ambar.</param>
        /// <returns></returns>
        public static string KontrolAmbar(string muhasebe, string harcama, string ambar)
        {
            string hata = string.Empty;

            if (!string.IsNullOrEmpty(muhasebe) && !string.IsNullOrEmpty(harcama) && !string.IsNullOrEmpty(ambar))
            {
                if (string.IsNullOrEmpty(TMMServis.UzayYaziDegerGetir("TASAMBAR", muhasebe + "-" + harcama + "-" + ambar, true, "")))
                    hata += string.Format(TasinirMalResI.TMM098, muhasebe, harcama, ambar) + "<br>";
            }

            return hata;
        }

        /// <summary>
        /// Verilen sayım formunun veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="sf">Sayım formunun bilgilerini tutan nesne</param>
        /// <returns>Sayım formu kayıtlı ise true, değilse false döndürülür.</returns>
        public static bool SayimFormVarMi(VeriErisim ve, SayimForm sf)
        {
            TABSayimForm tabSF = new TABSayimForm(ve);
            return tabSF.SayimFormVarMi(sf);
        }

        /// <summary>
        /// Verilen kayıttan düşme teklif ve onay tutanağının veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kf">Kayıttan düşme teklif ve onay tutanağının bilgilerini tutan nesne</param>
        /// <returns>Kayıttan düşme teklif ve onay tutanağı kayıtlı ise true, değilse false döndürülür.</returns>
        public static bool KayittanDusmeFormVarMi(VeriErisim ve, KayittanDusmeForm kf)
        {
            TABKayittanDusmeForm tabKF = new TABKayittanDusmeForm(ve);
            return tabKF.KayittanDusmeFormVarMi(kf);
        }

        /// <summary>
        /// Verilen personelin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        /// <param name="tcKimlikNo">Personelin T.C. kimlik numarası</param>
        /// <returns>Personel kayıtlı ise true, değilse false döndürülür.</returns>
        public static bool PersonelVarMi(VeriErisim ve, string tcKimlikNo)
        {
            TABPersonel tabPersonel = new TABPersonel(ve);
            return tabPersonel.PersonelVarMi(tcKimlikNo);
        }

        /// <summary>
        /// hesapPlani parametresi true ise taşınır hesap planı kodunun maskesini, false ise
        /// harcama birimi kodunun maskesini Degisken.xml dosyasından bulup döndüren yordam
        /// </summary>
        /// <param name="hesapPlani">Taşınır hesap planı kodunun maskesi mi bilgisi</param>
        /// <returns>İstenen kodun maskesi döndürülür.</returns>
        public static string KodYapiDondur(bool hesapPlani)
        {
            string kodYapisi = string.Empty;

            if (hesapPlani)
                kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");
            else
                kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI");

            return kodYapisi;
        }

        /// <summary>
        /// Parametre olarak verilen SQL cümlesini çalıştırıp sonucunu döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sql">Çalıştırılacak SQL cümlesi</param>
        /// <returns>Çalıştırılan SQL cümlesi sonucunda gelen bilgi formatlanarak döndürülür.</returns>
        public static string SQLCalistir(Kullanici kullanan, string baglanti, string sql)
        {
            StringBuilder bilgi = new StringBuilder();

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
                return TasinirMalResI.TMM099;

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                !(kullanan.kullaniciKodu == "stratek" ||
                kullanan.kullaniciKodu == "39228" ||
                kullanan.kullaniciKodu == "cunver" ||
                kullanan.kullaniciKodu == "29319"))
                return TasinirMalResI.TMM099;

            if (string.IsNullOrWhiteSpace(baglanti))
                baglanti = "BaglantiSatiriKYM";

            VeriErisim ve = VeriErisim.VeriErisimBul(null, baglanti, TNS.KYM.Arac.KimlikBul(kullanan));
            TABGenel to = new TABGenel(ve);

            IDataReader r = to.SQLCalistir(sql);
            if (r != null)
            {
                int satir = 1;
                bilgi.Append("<table border='1'>");
                bilgi.Append("<tr>");
                bilgi.Append("<td><b>" + TasinirMalResI.TMM100 + "</td>"); ;
                for (int i = 0; i < r.FieldCount; i++)
                    bilgi.Append("<td><b>" + r.GetName(i) + "</td>");
                bilgi.Append("</tr>");

                while (r.Read())
                {
                    bilgi.Append("<tr>");
                    bilgi.Append("<td>" + satir + "</td>");

                    for (int i = 0; i < r.FieldCount; i++)
                        bilgi.Append("<td>" + r.GetValue(i).ToString() + "</td>");

                    bilgi.Append("</tr>");
                    satir++;
                    if (satir > 999) break;
                }

                bilgi.Append("<br>");
                bilgi.Append("<br>");
                if (satir == 1)
                    bilgi.Append(TasinirMalResI.TMM029);
                else
                    bilgi.Append(string.Format(TasinirMalResI.TMM101, (satir - 1).ToString()));

                bilgi.Append("</table>");
                r.Close();
            }
            else
                bilgi.Append(to.Hata());

            ve.VeriErisimKapat();
            return bilgi.ToString();
        }

        /// <summary>
        /// Parametre olarak verilen veri tabanı tablosunun kolon adlarını bulup döndüren yordam
        /// </summary>
        /// <param name="tabloAd">Veri tabanı tablosunun adı</param>
        /// <returns>Tablonun kolon adları listesi döndürülür.</returns>
        public static string[] AlanAdlariDondur(string tabloAd)
        {
            if (string.IsNullOrEmpty(tabloAd))
                return null;

            string sql = string.Empty;
            if (veriTabani == "SQLCLIENT")
                sql = AlanAdlariSorgusuDondurSQL(tabloAd);
            else if (veriTabani == "ORACLE")
                sql = AlanAdlariSorgusuDondurOracle(tabloAd);
            else
                return null;

            return AlanAdlariAl(sql);
        }

        /// <summary>
        /// MS SQL veri tabanından verilen tablonun kolon adlarını çeken sorguyu döndüren yordam
        /// </summary>
        /// <param name="tabloAd">Veri tabanı tablosunun adı</param>
        /// <returns>Tablo kolon adlarını çeken sorgu döndürülür.</returns>
        private static string AlanAdlariSorgusuDondurSQL(string tabloAd)
        {
            return "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tabloAd + "'";
        }

        /// <summary>
        /// Oracle veri tabanından verilen tablonun kolon adlarını çeken sorguyu döndüren yordam
        /// </summary>
        /// <param name="tabloAd">Veri tabanı tablosunun adı</param>
        /// <returns>Tablo kolon adlarını çeken sorgu döndürülür.</returns>
        private static string AlanAdlariSorgusuDondurOracle(string tabloAd)
        {
            return "SELECT COLUMN_NAME FROM COLS WHERE TABLE_NAME = '" + tabloAd + "'";
        }

        /// <summary>
        /// Parametre olarak verilen SQL cümlesini çalıştırıp gelen kolon adlarını diziye ekleyip döndüren yordam
        /// </summary>
        /// <param name="sql">Tablo kolon adlarını çeken sorgu</param>
        /// <returns>Tablo kolon adları dizisi döndürülür.</returns>
        private static string[] AlanAdlariAl(string sql)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABGenel t = new TABGenel(ve);

            IDataReader r = t.SQLCalistir(sql);

            string adlar = string.Empty;
            while (r.Read())
                adlar += r.GetValue(0).ToString() + ";";
            r.Close();
            ve.VeriErisimKapat();

            adlar = adlar.Substring(0, adlar.Length - 1);
            return adlar.Split(';');
        }

        /// <summary>
        /// İstenilen bilgi isimler değişkeninde varsa oradan bulan, yoksa
        /// uzaylardan çeken ve isimler değişkenine ekleyen yordam
        /// </summary>
        /// <param name="isimler">Daha önce uzaylardan çekilmiş bilgileri tutan nesne</param>
        /// <param name="anahtar">Uzaylardan çekilecek bilginin koşulu</param>
        /// <param name="uzayAd">Bilgi çekilecek uzayın adı</param>
        /// <returns>İstenilen bilgi döndürülür.</returns>
        public static string AdGetir(System.Collections.Hashtable isimler, string anahtar, string uzayAd)
        {
            string ad = string.Empty;
            if (!string.IsNullOrEmpty(anahtar))
            {
                ad = (string)isimler[anahtar];
                if (ad == null)
                {
                    ad = TMMServis.UzayYaziDegerGetir(uzayAd, anahtar, true, "");
                    if (ad != null)
                        isimler.Add(anahtar, ad);
                }
            }
            return ad;
        }

        /// <summary>
        /// Parametre olarak verilen viewın olup olmadığını döndüren yordam
        /// </summary>
        /// <param name="ad">Aranan viewın adı</param>
        /// <returns>View var mı sonucu döndürülür.</returns>
        public static bool ViewMi(string ad)
        {
            if (string.IsNullOrEmpty(ad))
                return false;

            string sql = string.Empty;
            if (veriTabani == "SQLCLIENT")
                sql = ViewSorgusuDondurSQL(ad);
            else if (veriTabani == "ORACLE")
                sql = ViewSorgusuDondurOracle(ad);
            else
                return false;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            bool viewMi = ve.TekKolonSQLInt(sql) == 1 ? true : false;
            ve.VeriErisimKapat();
            return viewMi;
        }

        /// <summary>
        /// Parametre olarak verilen viewın MS SQL veri tabanında olup olmadığının sorgusunu döndüren yordam
        /// </summary>
        /// <param name="ad">Aranan viewın adı</param>
        /// <returns>View var mı sorgusu döndürülür.</returns>
        private static string ViewSorgusuDondurSQL(string ad)
        {
            return "SELECT 1 FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = '" + ad + "'";
        }

        /// <summary>
        /// Parametre olarak verilen viewın Oracle veri tabanında olup olmadığının sorgusunu döndüren yordam
        /// </summary>
        /// <param name="ad">Aranan viewın adı</param>
        /// <returns>View var mı sorgusu döndürülür.</returns>
        private static string ViewSorgusuDondurOracle(string ad)
        {
            return "SELECT 1 FROM USER_OBJECTS WHERE OBJECT_NAME = '" + ad + "' AND OBJECT_TYPE = 'VIEW'";
        }

        public static string KodAl()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}