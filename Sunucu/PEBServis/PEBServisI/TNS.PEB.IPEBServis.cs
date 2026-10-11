using System;
using TNS.KYM;
using TNS.BKD;
using TNS.ORG;

namespace TNS.PEB
{
    /// <summary>
    /// Remoting yöntemi ile PEBServis nesnesi oluşturmak için kullanılan sınıf
    /// Ayrıca, istemcide ve sunucuda ihtiyaç duyulan ortak kullanılacak statik fonksiyonlar da bu sınıfta yer alabilir.
    /// </summary>
    [Serializable()]
    public static class Arac
    {
        /// <summary>
        /// PEBServis nesnesini, konfigürasyon dosyasından okuduğu remoting dizgisini kullanarak
        /// .net remoting ile oluşturup döndüren statik yordam
        /// </summary>
        /// <returns>.net remoting ile oluşturulan IPEBServis arayüzünden türemiş nesne</returns>
        public static IPEBServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("PEBServisURL");
            IPEBServis servis = TNS.Api.ServisIstemci.Olustur<IPEBServis>(sUrl);
            return servis;
        }

        public static string HamVeriAdBul(string degiskenKod)
        {
            string[] degiskenParca = degiskenKod.Split('-');
            int vt = OrtakFonksiyonlar.ConvertToInt(degiskenParca[0], 1);
            string anahtar1 = "";
            string anahtar2 = "";
            string anahtar3 = "";
            string anahtar4 = "";
            if (degiskenParca.Length >= 2) anahtar1 = degiskenParca[1];
            if (degiskenParca.Length >= 3) anahtar1 = degiskenParca[2];
            if (degiskenParca.Length >= 4) anahtar1 = degiskenParca[3];
            if (degiskenParca.Length >= 5) anahtar1 = degiskenParca[4];
            return HamVeriAdBul(vt, anahtar1, anahtar2, anahtar3, anahtar4);
        }

        public static string HamVeriAdBul(int hamVeri, string anahtar1, string anahtar2, string anahtar3, string anahtar4)
        {
            if (hamVeri == 1)
            {
                if (anahtar1 == "01") return "Ögrenci Sayısı";
                if (anahtar1 == "02") return "Mezuniyet Oranı";
                if (anahtar1 == "03") return "Terk Oranı";
                if (anahtar1 == "04") return "Devam Oranı";
                if (anahtar1 == "05") return "Sınıf Geçme Oranı";
                if (anahtar1 == "06") return "On günden fazla devamsız öğrenci sayısı";
                if (anahtar1 == "07") return "Disiplin cezası alan öğrenci sayısı";

                if (anahtar1 == "08") return "Lisanslı sporcu sayısı";
                if (anahtar1 == "09") return "Katılınan beyin olimpiyatlari";
                if (anahtar1 == "10") return "Ulusal etkinlik sayısı";
                if (anahtar1 == "11") return "Uluslararası etkinlik sayısı";
                if (anahtar1 == "12") return "Ulusal ödül sayısı";
                if (anahtar1 == "13") return "Uluslararası ödül sayısı";

                if (anahtar1 == "14") return "AlanDalSayisi";
                if (anahtar1 == "15") return "IsletmeUygulamaOgrenciSayisi";
                if (anahtar1 == "16") return "MezunCalisanSayisi";
                if (anahtar1 == "17") return "AkrediteDalSayisi";
                if (anahtar1 == "18") return "AcilanKursSayisi";
                if (anahtar1 == "19") return "KursCesidiSayisi";
                if (anahtar1 == "20") return "KursaKatilimciSayisi";
                if (anahtar1 == "21") return "RehberOgretmenSayisi";
                if (anahtar1 == "22") return "YapilanRehberlikSayisi";
                if (anahtar1 == "23") return "GuvenlikGorevlisiSayisi";
                if (anahtar1 == "24") return "OgretmenSayisi";
                if (anahtar1 == "25") return "BransDersineGirme";
                if (anahtar1 == "26") return "LisansUstuDereceliOgretmenSayisi";

            }
            else if (hamVeri == 2)
            {
                return "Öğrenci sınav başarı";
            }
            else if (hamVeri == 3)
            {
                return "Oğrenci yıl sonu başarı";
            }
            else if (hamVeri == 4)
            {
                return "Üst eğitim kurumuna geçiş";
            }
            else
            {
                if (anahtar1 == "01") return "Şube sayısı";
                if (anahtar1 == "02") return "Kız öğrenci sayısı";
                if (anahtar1 == "03") return "Erkek öğrenci sayısı";
                if (anahtar1 == "04") return "Dezavantajlı  öğrenci sayısı";
                if (anahtar1 == "05") return "Özel öğrenci sayısı";
                if (anahtar1 == "06") return "Yatılı öğrenci sayısı";
            }
            return "Değişken Adı Bulunamadı!";
        }

        public static bool YetkinMi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) return true;
            if (kullanan.YetkiTipiDegeriBul(42).IndexOf("4;") >= 0) return true;
            return false;

        }

        internal static bool OranMi(int bilgiTuru, string anahtar1, string anahtar2, string anahtar3, string anahtar4)
        {
            bool sayiMi = false;
            if (bilgiTuru == 1)
            {
                if (anahtar1 == "01" || anahtar1 == "09" || anahtar1 == "10" || anahtar1 == "11" || anahtar1 == "12" || anahtar1 == "13") sayiMi = true;
            }
            if (bilgiTuru == 5)
            {
                sayiMi = true;
            }
            return !sayiMi;
        }

        public static string HedefAciklamaAdBul(ENUMSTRHEDEFAciklama aciklamaTur)
        {
            switch (aciklamaTur)
            {
                case ENUMSTRHEDEFAciklama.Riskler: return "Riskler";
                case ENUMSTRHEDEFAciklama.Stratejiler: return "Stratejiler";
                case ENUMSTRHEDEFAciklama.Tespitler: return "Tespitler";
                case ENUMSTRHEDEFAciklama.Ihtiyaclar: return "İhtiyaçlar";
                case ENUMSTRHEDEFAciklama.ProgramBagi: return "Program Bağı";
                default: return "";
            }
        }

        public static string DonemStr(ENUMDonemler donem, bool kodEkle)
        {
            string on = "";
            if (kodEkle)
                on = ((int)donem).ToString() + "- ";
            switch (donem)
            {
                case ENUMDonemler.Ay: return on + "Aylık";
                case ENUMDonemler.UcAy: return on + "3 Aylık";
                case ENUMDonemler.AltiAy: return on + "6 Aylık";
                case ENUMDonemler.Yil: return on + "Yıllık";
                default: return on + "";
            }
        }

        internal static ENUMDonemler DonemBul(string strDonem)
        {
            if (string.IsNullOrWhiteSpace(strDonem)) return ENUMDonemler.Ay;
            return (ENUMDonemler)OrtakFonksiyonlar.ConvertToInt(strDonem.Split('-')[0], (int)ENUMDonemler.Ay);
        }
    }

    /// <summary>
    /// Stratejik plan modülünün okuma, yazma gibi yetki kontrollerini yapan statik fonksiyonları içeren sınıf
    /// </summary>
    [Serializable()]
    public static class YetkiSTR
    {
        /// <summary>
        /// Parametre olarak gönderilen kullanıcının stratejik plan
        /// bilgilerini görme yetkisinin olup olmadığını kontrol eden yordam
        /// Kullanıcının stratejik plan bilgilerini görme yetkisi varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">İstekte bulunan kullanıcı</param>
        /// <returns>Görme yetkisi olup olmadığının sonucu</returns>
        private static bool GormeYetkisiVarMi(Kullanici kullanan, int sira)
        {
            int yetki = YetkiDegerBul(kullanan.yetkiTipiDegerleri["Stratejik Plan Yetkisi", sira], 0);
            if (yetki >= 1 && yetki <= 7)
                return true;
            return false;
        }

        private static int YetkiDegerBul(string yetkiDeger, int defaultDeger)
        {
            if (string.IsNullOrWhiteSpace(yetkiDeger)) return defaultDeger;
            string[] degerler = yetkiDeger.Split(',', ';');
            return OrtakFonksiyonlar.ConvertToInt(degerler[0], defaultDeger);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının stratejik plan
        /// bilgilerini değiştirme yetkisinin olup olmadığını kontrol eden yordam
        /// Kullanıcının stratejik plan bilgilerini değiştirme yetkisi varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">İstekte bulunan kullanıcı</param>
        /// <returns>Yazma yetkisi olup olmadığının sonucu</returns>
        private static bool OGYYetkisiVarMi(Kullanici kullanan, int sira, TNS.BHMv2.ENUMOGY ogy)
        {
            int yetki = YetkiDegerBul(kullanan.yetkiTipiDegerleri["Stratejik Plan Yetkisi", sira], 0);
            if (ogy == BHMv2.ENUMOGY.Yazma && (yetki == 2 || yetki == 4))
                return true;
            if (ogy == BHMv2.ENUMOGY.Gorme && (yetki > 1))
                return true;
            return false;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının stratejik plan
        /// bilgilerini onaylama yetkisinin olup olmadığını kontrol eden yordam
        /// Kullanıcının stratejik plan bilgilerini onaylama yetkisi varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">İstekte bulunan kullanıcı</param>
        /// <returns>Onaylama yetkisi olup olmadığının sonucu</returns>
        private static bool OnaylamaYetkisiVarMi(Kullanici kullanan, int sira)
        {
            int yetki = YetkiDegerBul(kullanan.yetkiTipiDegerleri["Stratejik Plan Yetkisi", sira], 0);
            if (yetki == 3 || yetki == 4)
                return true;
            return false;
        }

        private static bool BirimeYetkisiVarMi(Kullanici kullanan, int orgBirimKod, int sira, bool ustBirimlerDahil)
        {
            return BirimeYetkisiVarMi(kullanan, orgBirimKod, sira, ustBirimlerDahil, false);
        }

        private static bool UstBirimineYetkisiVarMi(Kullanici kullanan, int orgBirimKod, int sira)
        {
            return BirimeYetkisiVarMi(kullanan, orgBirimKod, sira, true, true);
        }

        private static bool BirimeYetkisiVarMi(Kullanici kullanan, int orgBirimKod, int sira, bool ustBirimlerDahil, bool kendisiHaric)
        {
            TNS.UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
            string kopruKod = servisUZY.UzayDegeriStr(kullanan, "ORGBIRIMKOPRUKOD", orgBirimKod.ToString(), true);
            if (string.IsNullOrEmpty(kopruKod))
                return false;

            int altdal_Yetki = OrtakFonksiyonlar.ConvertToInt(kullanan.yetkiTipiDegerleri["Altdal Yetki", sira], 0);
            string yetkiKopruKod = kullanan.yetkiTipiDegerleri["Stratejik Plan Kopru Kod", sira];
            if (yetkiKopruKod == kopruKod) return true;
            if (altdal_Yetki == 1)
            {
                return kopruKod.StartsWith(yetkiKopruKod);
            }

            return false;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının istemci tarafındaki stratejik plan
        /// sayfalarını görmeye yetkili olup olmadığının kontrolünü yapan yordam
        /// Kullanıcının stratejik plan yetkisi varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">İstekte bulunan kullanıcı</param>
        /// <returns>Yetkili olup olmadığının sonucu</returns>
        public static bool SayfayaGirisYapabilirMi(Kullanici kullanan)
        {
            if (YoneticiMi(kullanan))
                return true;

            for (int i = kullanan.yetkiTipiDegerleri.MinIndex(); i <= kullanan.yetkiTipiDegerleri.MaxIndex(); i++)
                if (GormeYetkisiVarMi(kullanan, i))
                    return true;

            return false;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının yönetici olup olmadığının kontrolünü yapan yordam
        /// Kullanıcı yönetici ise true, değilse false döndürür.
        /// </summary>
        /// <param name="kullanan">İstekte bulunan kullanıcı</param>
        /// <returns>Yönetici olup olmadığının sonucu</returns>
        public static bool YoneticiMi(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI, (int)ENUMKullaniciTipi.PERFORMANSBUTCEYONETICISI);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının stratejik plan sözlüklerini
        /// tanımlamaya hakkı var mı yok mu kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Sözlük kaydedecek kullanıcı</param>
        /// <returns>Yetkili olup olmadığının sonucu</returns>
        public static Sonuc TanimlamaYazmaHakkiVarMi(Kullanici kullanan)
        {
            if (YoneticiMi(kullanan))
                return new Sonuc();
            return new Sonuc(false, PerformansButceResI.STR001, string.Empty);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının stratejik plan
        /// bilgilerini okuma hakkının olup olmadığını kontrol eden yordam
        /// Kullanıcının stratejik plan bilgilerini okuma hakkı varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">İstekte bulunan kullanıcı</param>
        /// <returns>Okuma hakkı olup olmadığının sonucu</returns>
        public static Sonuc OkumaHakkiVarMi(Kullanici kullanan)
        {
            return new Sonuc();
            //if (YoneticiMi(kullanan))
            //    return new Sonuc();

            //for (int i = kullanan.yetkiTipiDegerleri.MinIndex(); i <= kullanan.yetkiTipiDegerleri.MaxIndex(); i++)
            //{
            //    if (GormeYetkisiVarMi(kullanan, i))
            //        return new Sonuc();
            //}

            //return new Sonuc(false, PerformansButceResI.STR002, string.Empty);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının stratejik plan
        /// bilgilerini değiştirme hakkının olup olmadığını kontrol eden yordam
        /// Kullanıcının stratejik plan bilgilerini değiştirme hakkı varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <returns>Yazma hakkı olup olmadığının sonucu</returns>
        public static Sonuc OGYHakkiVarMi(Kullanici kullanan, string kurumKod, BHMv2.ENUMOGY ogy)
        {
            Sonuc s = new Sonuc();
            if (string.IsNullOrWhiteSpace(kurumKod))
                s.DegerAta("Yetki kontrolü için kurumsal kod girilmelidir!");
            else
            {
                if (YoneticiMi(kullanan))
                    return s;

                for (int i = kullanan.yetkiTipiDegerleri.MinIndex(); i <= kullanan.yetkiTipiDegerleri.MaxIndex(); i++)
                {
                    if (OGYYetkisiVarMi(kullanan, i, ogy))
                        return s;
                }
                s.DegerAta(PerformansButceResI.STR001);
            }
            return s;
        }

        public static Sonuc OnaylamaHakkiVarMi(Kullanici kullanan)
        {
            if (YoneticiMi(kullanan))
                return new Sonuc();

            for (int i = kullanan.yetkiTipiDegerleri.MinIndex(); i <= kullanan.yetkiTipiDegerleri.MaxIndex(); i++)
            {
                if (OnaylamaYetkisiVarMi(kullanan, i))
                    return new Sonuc();
            }

            return new Sonuc(false, PerformansButceResI.STR100, string.Empty);
        }

        public static Sonuc OkumaHakkiVarMi(Kullanici kullanan, int orgBirimKod)
        {
            if (YoneticiMi(kullanan))
                return new Sonuc();

            if (orgBirimKod > 0)
            {
                for (int i = kullanan.yetkiTipiDegerleri.MinIndex(); i <= kullanan.yetkiTipiDegerleri.MaxIndex(); i++)
                {
                    if (GormeYetkisiVarMi(kullanan, i) && BirimeYetkisiVarMi(kullanan, orgBirimKod, i, true))
                        return new Sonuc();
                }
            }

            return new Sonuc(false, PerformansButceResI.STR002, string.Empty);
        }

        public static Sonuc YazmaHakkiVarMi(Kullanici kullanan, int orgBirimKod)
        {
            if (YoneticiMi(kullanan))
                return new Sonuc();

            if (orgBirimKod > 0)
            {
                for (int i = kullanan.yetkiTipiDegerleri.MinIndex(); i <= kullanan.yetkiTipiDegerleri.MaxIndex(); i++)
                {
                    if (OGYYetkisiVarMi(kullanan, i, BHMv2.ENUMOGY.Yazma) && BirimeYetkisiVarMi(kullanan, orgBirimKod, i, false))
                        return new Sonuc();
                }
            }

            return new Sonuc(false, PerformansButceResI.STR001, string.Empty);
        }

        public static Sonuc OnaylamaHakkiVarMi(Kullanici kullanan, int orgBirimKod)
        {
            return OnaylamaHakkiVarMi(kullanan, orgBirimKod, true);
        }

        public static Sonuc OnaylamaHakkiVarMi(Kullanici kullanan, int orgBirimKod, bool ustBirimlerDahil)
        {
            if (YoneticiMi(kullanan))
                return new Sonuc();

            if (orgBirimKod > 0)
            {
                for (int i = kullanan.yetkiTipiDegerleri.MinIndex(); i <= kullanan.yetkiTipiDegerleri.MaxIndex(); i++)
                {
                    if (OnaylamaYetkisiVarMi(kullanan, i) && BirimeYetkisiVarMi(kullanan, orgBirimKod, i, ustBirimlerDahil))
                        return new Sonuc();
                }
            }

            return new Sonuc(false, PerformansButceResI.STR100, string.Empty);
        }

        public static Sonuc UstOnayaHakkiVarMi(Kullanici kullanan, int orgBirimKod)
        {
            if (YoneticiMi(kullanan))
                return new Sonuc();

            if (orgBirimKod > 0)
            {
                for (int i = kullanan.yetkiTipiDegerleri.MinIndex(); i <= kullanan.yetkiTipiDegerleri.MaxIndex(); i++)
                {
                    if (OnaylamaYetkisiVarMi(kullanan, i) && UstBirimineYetkisiVarMi(kullanan, orgBirimKod, i))
                        return new Sonuc();
                }
            }

            return new Sonuc(false, PerformansButceResI.STR100, string.Empty);
        }
    }

    /// <summary>
    /// Performans bütçe modülünün okuma, yazma gibi yetki kontrollerini yapan statik fonksiyonları içeren sınıf
    /// </summary>
    [Serializable()]
    public static class Yetki
    {
        /// <summary>
        /// Parametre olarak gönderilen kullanıcının istemci tarafındaki performans bütçe
        /// sayfalarını görmeye yetkili olup olmadığının kontrolünü yapan yordam
        /// Kullanıcının performans bütçe yetkisi varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <returns>Yetkili olup olmadığının sonucu</returns>
        public static bool SayfayaGirisYapabilirMi(Kullanici kullanan)
        {
            return GormeYetkisiVarMi(kullanan) || YoneticiMi(kullanan);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının yönetici olup olmadığının kontrolünü yapan yordam
        /// Kullanıcı yönetici ise true, değilse false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <returns>Yönetici olup olmadığının sonucu</returns>
        public static bool YoneticiMi(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI, (int)ENUMKullaniciTipi.PERFORMANSBUTCEYONETICISI);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının performans bütçe
        /// bilgilerini görme yetkisinin olup olmadığını kontrol eden yordam
        /// Kullanıcının performans bütçe bilgilerini görme yetkisi varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <returns>Görme yetkisi olup olmadığının sonucu</returns>
        private static bool GormeYetkisiVarMi(Kullanici kullanan)
        {
            return
                kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "1", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "2", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "3", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Personel Yetkisi", "4", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Stratejik Plan Yetkisi", "1", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Stratejik Plan Yetkisi", "2", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Stratejik Plan Yetkisi", "3", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Stratejik Plan Yetkisi", "4", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Performans Bütçe Yetkisi", "1", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Performans Bütçe Yetkisi", "2", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Performans Bütçe Yetkisi", "3", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Performans Bütçe Yetkisi", "4", false);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının performans bütçe
        /// bilgilerini değiştirme yetkisinin olup olmadığını kontrol eden yordam
        /// Kullanıcının performans bütçe bilgilerini değiştirme yetkisi varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <returns>Yazma yetkisi olup olmadığının sonucu</returns>
        private static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            return
                kullanan.YetkiTipiDegeriIceriyorMu("Performans Bütçe Yetkisi", "2", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Performans Bütçe Yetkisi", "4", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Stratejik Plan Yetkisi", "2", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Stratejik Plan Yetkisi", "4", false);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının performans bütçe
        /// bilgilerini onaylama yetkisinin olup olmadığını kontrol eden yordam
        /// Kullanıcının performans bütçe bilgilerini onaylama yetkisi varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <returns>Onaylama yetkisi olup olmadığının sonucu</returns>
        private static bool OnaylamaYetkisiVarMi(Kullanici kullanan)
        {
            return
                kullanan.YetkiTipiDegeriIceriyorMu("Performans Bütçe Yetkisi", "3", false) ||
                kullanan.YetkiTipiDegeriIceriyorMu("Performans Bütçe Yetkisi", "4", false);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının performans bütçe
        /// yapabileceği birimleri döndüren yordam
        /// </summary>
        /// <param name="kullanan">Birimleri istenen kullanıcı</param>
        /// <returns>Performans bütçe yapabileceği birim listesi</returns>
        private static string[] BirimKodlariDondur(Kullanici kullanan)
        {
            string birimKodlari = (string)kullanan.yetkiTipiDegerleri["Performans Bütçe Yapacağı Birimler"];

            if (birimKodlari == null)
                birimKodlari = "";

            return birimKodlari.Split(';');
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının
        /// gönderilen birime yetkisi var mı yok mu kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Birim kontrolü yapılacak kullanıcı</param>
        /// <param name="birimKodu">Kullanıcının birimlerinde aranacak birim</param>
        /// <returns>Birime yetkili olup olmadığının sonucu</returns>
        private static bool BirimBirimiMi(Kullanici kullanan, string birimKodu)
        {
            if (birimKodu == "")
            {
                string[] birimler = BirimKodlariDondur(kullanan);
                for (int i = 0; i < birimler.Length; i++)
                    if (TNS.BKD.Arac.KurumMu(birimler[i]))
                        return true;

                return false;
            }

            return kullanan.YetkiTipiDegeriIceriyorMu("Performans Bütçe Yapacağı Birimler", birimKodu, true) || kullanan.YetkiTipiDegeriIceriyorMu("Stratejik Plan Yapacağı Birimler", birimKodu, true);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının gönderilen
        /// tedbir birimine yetkisi var mı yok mu kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Birim kontrolü yapılacak kullanıcı</param>
        /// <param name="birimKodu">Kullanıcının birimlerinde aranacak tedbir birimi</param>
        /// <returns>Birime yetkili olup olmadığının sonucu</returns>
        private static bool BirimBirimiMi(Kullanici kullanan, int birimKodu)
        {
            if (kullanan.YetkiTipiDegeriIceriyorMu("Performans Bütçe Tedbir Yapacağı Birimler", birimKodu.ToString(), false)
                || kullanan.YetkiTipiDegeriIceriyorMu("Performans Bütçe Tedbir Yapacağı Birimler", ((int)ENUMTedbirBirim.YETKILIBIRIM).ToString(), false))
                return true;

            return false;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının ENUMTedbirBirim listesindeki
        /// YETKILIBIRIM tedbir birimine yetkisi var mı yok mu kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Birim kontrolü yapılacak kullanıcı</param>
        /// <returns>Birime yetkili olup olmadığının sonucu</returns>
        public static bool YetkiliBirimMi(Kullanici kullanan)
        {
            if (BirimBirimiMi(kullanan, (int)ENUMTedbirBirim.YETKILIBIRIM) || YoneticiMi(kullanan))
                return true;

            return false;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının performans bütçe
        /// bilgilerini değiştirme hakkının olup olmadığını kontrol eden yordam
        /// Kullanıcının performans bütçe bilgilerini değiştirme hakkı varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <returns>Yazma hakkı olup olmadığının sonucu</returns>
        public static Sonuc YazmaHakkiVarMi(Kullanici kullanan)
        {
            Sonuc donusDeger = new Sonuc(false, PerformansButceResI.PEB001, "");

            if (YoneticiMi(kullanan) || YazmaYetkisiVarMi(kullanan))
            {
                donusDeger.islemSonuc = true;
                donusDeger.hataStr = "";
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının gönderilen birim için performans bütçe
        /// bilgilerini değiştirme hakkının olup olmadığını kontrol eden yordam
        /// Kullanıcının gönderilen birim için performans bütçe bilgilerini değiştirme hakkı varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <param name="birimKodu">Kullanıcının birimlerinde aranacak birim</param>
        /// <returns>Yazma hakkı olup olmadığının sonucu</returns>
        public static Sonuc YazmaHakkiVarMi(Kullanici kullanan, string birimKodu)
        {
            Sonuc donusDeger = new Sonuc(false, PerformansButceResI.PEB002, "");

            if (YoneticiMi(kullanan) || (YazmaYetkisiVarMi(kullanan) && BirimBirimiMi(kullanan, birimKodu)))
            {
                donusDeger.islemSonuc = true;
                donusDeger.hataStr = "";
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının gönderilen tedbir birimi için tedbir
        /// bilgilerini değiştirme hakkının olup olmadığını kontrol eden yordam
        /// Kullanıcının gönderilen tedbir birimi için tedbir bilgilerini değiştirme hakkı varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <param name="birimKodu">Kullanıcının tedbir birimlerinde aranacak tedbir birimi</param>
        /// <returns>Yazma hakkı olup olmadığının sonucu</returns>
        public static Sonuc YazmaHakkiVarMi(Kullanici kullanan, int birimKodu)
        {
            Sonuc donusDeger = new Sonuc(true, "", "");

            if (YoneticiMi(kullanan) || (YazmaYetkisiVarMi(kullanan) && BirimBirimiMi(kullanan, birimKodu)))
            {
                return donusDeger;
            }
            if (kullanan.YetkiTipiDegeriIceriyorMu("Personel Organizasyon Yetkisi", birimKodu.ToString(), false))
            {
                return donusDeger;
            }
            donusDeger.DegerAta(PerformansButceResI.PEB002);
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının performans bütçe sözlüklerini
        /// tanımlamaya hakkı var mı yok mu kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Sözlük kaydedecek kullanıcı</param>
        /// <returns>Yetkili olup olmadığının sonucu</returns>
        public static Sonuc TanimlamaYazmaHakkiVarMi(Kullanici kullanan)
        {
            Sonuc donusDeger = new Sonuc(false, PerformansButceResI.PEB001, "");

            if (YoneticiMi(kullanan))
            {
                donusDeger.islemSonuc = true;
                donusDeger.hataStr = "";
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının performans bütçe
        /// bilgilerini okuma hakkının olup olmadığını kontrol eden yordam
        /// Kullanıcının performans bütçe bilgilerini okuma hakkı varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <returns>Okuma hakkı olup olmadığının sonucu</returns>
        public static Sonuc OkumaHakkiVarMi(Kullanici kullanan)
        {
            Sonuc donusDeger = new Sonuc(false, PerformansButceResI.PEB003, "");

            if (YoneticiMi(kullanan) || GormeYetkisiVarMi(kullanan))
            {
                donusDeger.islemSonuc = true;
                donusDeger.hataStr = "";
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının gönderilen birimin performans bütçe
        /// bilgilerini okuma hakkı var mı yok mu kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <param name="birimKodu">Kontrol edilecek birim</param>
        /// <returns>Yetkili olup olmadığının sonucu</returns>
        public static Sonuc OkumaHakkiVarMi(Kullanici kullanan, string birimKodu)
        {
            Sonuc donusDeger = new Sonuc(false, PerformansButceResI.PEB004, "");

            if (YoneticiMi(kullanan) || (GormeYetkisiVarMi(kullanan) && BirimBirimiMi(kullanan, birimKodu)))
            {
                donusDeger.islemSonuc = true;
                donusDeger.hataStr = "";
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının gönderilen tedbir biriminin
        /// tedbir bilgilerini okuma hakkı var mı yok mu kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <param name="birimKodu">Kontrol edilecek tedbir birimi</param>
        /// <returns>Yetkili olup olmadığının sonucu</returns>
        public static Sonuc OkumaHakkiVarMi(Kullanici kullanan, int birimKodu)
        {
            Sonuc donusDeger = new Sonuc(true, "", "");

            if (YoneticiMi(kullanan) || (GormeYetkisiVarMi(kullanan) && BirimBirimiMi(kullanan, birimKodu)))
            {
                return donusDeger;
            }
            if (kullanan.YetkiTipiDegeriIceriyorMu("Personel Organizasyon Yetkisi", birimKodu.ToString(), false))
            {
                return donusDeger;
            }
            donusDeger.DegerAta(PerformansButceResI.PEB004);
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının performans bütçe
        /// bilgilerini onaylama hakkının olup olmadığını kontrol eden yordam
        /// Kullanıcının performans bütçe bilgilerini onaylama hakkı varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <returns>Onaylama hakkı olup olmadığının sonucu</returns>
        public static Sonuc OnaylamaHakkiVarMi(Kullanici kullanan)
        {
            Sonuc donusDeger = new Sonuc(false, PerformansButceResI.PEB110, "");

            if (YoneticiMi(kullanan) || OnaylamaYetkisiVarMi(kullanan))
            {
                donusDeger.islemSonuc = true;
                donusDeger.hataStr = "";
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının gönderilen birim için performans bütçe
        /// bilgilerini onaylama hakkının olup olmadığını kontrol eden yordam
        /// Kullanıcının gönderilen birim için performans bütçe bilgilerini onaylama hakkı varsa true, yoksa false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <param name="birimKodu">Kullanıcının birimlerinde aranacak birim</param>
        /// <returns>Onaylama hakkı olup olmadığının sonucu</returns>
        public static Sonuc OnaylamaHakkiVarMi(Kullanici kullanan, string birimKodu)
        {
            Sonuc donusDeger = new Sonuc(false, PerformansButceResI.PEB111, "");

            if (YoneticiMi(kullanan) || (OnaylamaYetkisiVarMi(kullanan) && BirimBirimiMi(kullanan, birimKodu)))
            {
                donusDeger.islemSonuc = true;
                donusDeger.hataStr = "";
            }

            return donusDeger;
        }

        public static string OrgKodKurumsalBul(int orgKod)
        {
            IORGServis servisORG = TNS.ORG.Arac.Tanimla();

            OrganizasyonBirim kriter = new OrganizasyonBirim();
            kriter.kod = orgKod;
            kriter.sablonMu = false;
            ObjectArray orgler = servisORG.OrganizasyonBirimListele(kriter, false, 1, false);
            if (!orgler.sonuc.islemSonuc)
                throw new Exception(orgler.sonuc.hataStr);
            if (orgler.objeler.Count <= 0)
                throw new Exception(orgler.sonuc.bilgiStr);

            return OrgKodKurumsalBul(orgler.objeler, orgKod);
        }

        private static string OrgKodKurumsalBul(TNSCollection orgler, int orgKod)
        {
            if (orgler == null || orgler.Count <= 0 || orgKod <= 0)
                return string.Empty;

            for (int i = 0; i < orgler.Count; i++)
            {
                OrganizasyonBirim orgBirim = (OrganizasyonBirim)orgler[i];
                if (orgBirim.kod == orgKod)
                {
                    if (!string.IsNullOrEmpty(orgBirim.kurumsalkod))
                        return orgBirim.kurumsalkod;
                    else
                    {
                        if (orgBirim.anaBirimKod <= 0)
                            return string.Empty;
                        orgKod = orgBirim.anaBirimKod;
                        i = -1;
                    }
                }
            }

            return string.Empty;
        }

        public static Sonuc YazmaHakkiVarMiOrgKod(Kullanici kullanan, int orgKod)
        {
            Sonuc s = YazmaHakkiVarMi(kullanan, orgKod);
            if (s.islemSonuc) return s;

            return YazmaHakkiVarMi(kullanan, OrgKodKurumsalBul(orgKod));
        }

        public static Sonuc OkumaHakkiVarMiOrgKod(Kullanici kullanan, int orgKod)
        {
            Sonuc s = OkumaHakkiVarMi(kullanan, orgKod);
            if (s.islemSonuc) return s;

            return OkumaHakkiVarMi(kullanan, OrgKodKurumsalBul(orgKod));
        }
    }

    /// <summary>
    /// KurumGrupDetay: Kurum Listesi
    /// </summary>
    [System.Serializable()]
    public class KurumGrupDetay
    {
        /// <summary>
        /// Kurum Kod: Kurum Kodu
        /// </summary>
        public string kurumKod { get; set; }

        public string kurumAd { get; set; }

        public KurumGrupDetay()
        {
        }

        public KurumGrupDetay(string kurumKod)
        {
            this.kurumKod = kurumKod;
        }
    }


    /// <summary>
    /// KurumGruplari: Kurum Grupları
    /// </summary>
    [System.Serializable()]
    public class KurumGruplari
    {

        /// <summary>
        /// Kullanıcı Kodu: Kullanıcı Kodu
        /// </summary>
        public string kullaniciKodu { get; set; }

        /// <summary>
        /// Sıra No: Sıra No
        /// </summary>
        public int siraNo { get; set; }

        /// <summary>
        /// Ad: Ad
        /// </summary>
        public string ad { get; set; }

        /// <summary>
        /// KurumGrupDetay: Kurum Listesi
        /// </summary>
        public TNSCollection kurumGrupDetay;

        public bool detayOku = false;

        public int yil = 0;
    }

    /// <summary>
    /// HEDEFOdenek: Ödenekler
    /// </summary>
    [System.Serializable()]
    public class StrHedefOdenek
    {
        public StrHedefOdenek()
        {
            this.ekoKod = "";
            this.ekoAd = "";
        }

        public StrHedefOdenek(string ekoKod, string ekoAd)
        {
            this.ekoKod = ekoKod;
            this.ekoAd = ekoAd;
        }

        /// <summary>
        /// Eko Kod: Eko Kod
        /// </summary>
        public string ekoKod { get; set; }

        /// <summary>
        /// Eko Kod: Eko Kod
        /// </summary>
        public string ekoAd { get; set; }

        /// <summary>
        /// ODENEK0: Dönem 0
        /// </summary>
        public double d0 { get; set; }

        /// <summary>
        /// ODENEK1: Dönem 1
        /// </summary>
        public double d1 { get; set; }

        /// <summary>
        /// ODENEK2: Dönem 2
        /// </summary>
        public double d2 { get; set; }

        /// <summary>
        /// ODENEK3: Dönem 3
        /// </summary>
        public double d3 { get; set; }

        /// <summary>
        /// ODENEK4: Dönem 4
        /// </summary>
        public double d4 { get; set; }
    }

    /// <summary>
    /// HEDEFGosterge: Göstergeler
    /// </summary>
    [System.Serializable()]
    public class StrHedefGosterge
    {
        /// <summary>
        /// Gösterge Kod: Gösterge Kod
        /// </summary>
        public int gostergeKod { get; set; }

        public ENUMGostergeTur gostergeTur { get; set; }
        public ENUMGostergeIlerlemeYonu gostergeIlerlemeYon { get; set; }
        public int gostergeOndalik { get; set; }
        /// <summary>
        /// Gösterge Kod: Gösterge Kod
        /// </summary>
        public string gostergeAd { get; set; }
        public string gostergeKodAd { get { return gorKod + "- " + gostergeAd; } }

        /// <summary>
        /// GERCEKLESEN: GERCEKLESEN
        /// </summary>
        public double gerceklesen { get; set; }

        /// <summary>
        /// TAHMIN0: Tahmin 0
        /// </summary>
        public double tahmin0 { get; set; }

        /// <summary>
        /// TAHMIN1: Tahmin 1
        /// </summary>
        public double tahmin1 { get; set; }

        /// <summary>
        /// TAHMIN2: Tahmin 2
        /// </summary>
        public double tahmin2 { get; set; }

        /// <summary>
        /// TAHMIN3: Tahmin 3
        /// </summary>
        public double tahmin3 { get; set; }

        /// <summary>
        /// TAHMIN4: Tahmin 4
        /// </summary>
        public double tahmin4 { get; set; }

        public string gostergeBirimAd { get; set; }

        public string gorKod { get; set; }
        public double hedefeEtkisi { get; set; }
        public ENUMDonemler izlemeDonemi { get; set; }
        public ENUMDonemler raporlamaDonemi { get; set; }

        public string izlemeDonemiStr { get { return TNS.PEB.Arac.DonemStr(izlemeDonemi, true); } set { izlemeDonemi = TNS.PEB.Arac.DonemBul(value); } }
        public string raporlamaDonemiStr { get { return TNS.PEB.Arac.DonemStr(raporlamaDonemi, true); } set { raporlamaDonemi = TNS.PEB.Arac.DonemBul(value); } }


        public int donemBYil { get; set; }
        public int hedefKod { get; set; }
        public string kurumKod { get; set; }
        public int gercYil { get; set; }
        public int donem { get; set; }
        public double donemBaslangic { get { return gerceklesen; } set { gerceklesen = value; } }

        public double oncYilGerceklesme { get; set; }
        public double hedeflenenBirikimliDeger
        {
            get
            {
                if (gercYil == aralikBasYil + 0) return tahmin0;
                if (gercYil == aralikBasYil + 1) return tahmin1;
                if (gercYil == aralikBasYil + 2) return tahmin2;
                if (gercYil == aralikBasYil + 3) return tahmin3;
                if (gercYil == aralikBasYil + 4) return tahmin4;
                return 0;
            }

            set
            {
                if (gercYil == aralikBasYil + 0) tahmin0 = value;
                if (gercYil == aralikBasYil + 1) tahmin1 = value;
                if (gercYil == aralikBasYil + 2) tahmin2 = value;
                if (gercYil == aralikBasYil + 3) tahmin3 = value;
                if (gercYil == aralikBasYil + 4) tahmin4 = value;
            }
        }

        public double birikimliGerceklesme { get; set; }
        public double birikimliPerformans { get { return Oranla(donemBaslangic, hedeflenenBirikimliDeger, birikimliGerceklesme, this.gostergeTur, gostergeIlerlemeYon); } }
        public double birikimliPerformansSinirli
        {
            get
            {
                double tut = Oranla(donemBaslangic, hedeflenenBirikimliDeger, birikimliGerceklesme, this.gostergeTur, gostergeIlerlemeYon);
                if (tut < 0) return 0;
                if (tut > 100) return 100;
                return tut;
            }
        }

        public string aciklama { get; set; }

        public string ilgililik { get; set; }
        public string etkililik { get; set; }
        public string etkinlik { get; set; }
        public string surdurulebilirlik { get; set; }

        public static double Oranla(double baslangicDegeri, double hedeflenenDeger, double gerceklesenDeger, ENUMGostergeTur oranlamaYontem, ENUMGostergeIlerlemeYonu ilerlemeYonu)
        {
            double bolen, bolunen;

            if (oranlamaYontem == ENUMGostergeTur.OranABCB)
            {
                bolen = hedeflenenDeger - baslangicDegeri;
                bolunen = gerceklesenDeger - baslangicDegeri;
            }
            else
            {
                bolen = hedeflenenDeger;
                bolunen = gerceklesenDeger;
            }
            if (bolen != 0)
            {
                return bolunen * 100 / bolen;
            }
            return 0;
        }

        public string AciklamaOl(int tur)
        {
            if (tur == 4)
            {
                string tut = Formatla("İlgililik", ilgililik) + Formatla("Etkililik", etkililik) + Formatla("Etkinlik", etkinlik) + Formatla("Sürdürülebilirlik", surdurulebilirlik);
                if (!string.IsNullOrWhiteSpace(tut))
                    tut = tut.Substring(0, tut.Length - 2);
                return tut;
            }
            else
                return aciklama;
        }

        private string Formatla(string baslik, string yazi)
        {
            if (string.IsNullOrWhiteSpace(yazi)) return "";
            return baslik + ": " + yazi + "\r\n";
        }

        public int aralikBasYil { get; set; }

        public int aralikSonYil { get; set; }

        public int aktifOranNo { get; set; }

        public double gerceklesen0 { get; set; }
        public double gerceklesen1 { get; set; }
        public double gerceklesen2 { get; set; }
        public double gerceklesen3 { get; set; }
        public double gerceklesen4 { get; set; }

        public double GostergeOranBul
        {
            get
            {
                if (aktifOranNo == 0) return gostOran0;
                if (aktifOranNo == 1) return gostOran1;
                if (aktifOranNo == 2) return gostOran2;
                if (aktifOranNo == 3) return gostOran3;
                return gostOran4;
            }
        }
        public double gostOran0 { get { return Oranla(donemBaslangic, tahmin0, gerceklesen0, this.gostergeTur, gostergeIlerlemeYon); } }
        public double gostOran1 { get { return Oranla(donemBaslangic, tahmin1, gerceklesen1, this.gostergeTur, gostergeIlerlemeYon); } }
        public double gostOran2 { get { return Oranla(donemBaslangic, tahmin2, gerceklesen2, this.gostergeTur, gostergeIlerlemeYon); } }
        public double gostOran3 { get { return Oranla(donemBaslangic, tahmin3, gerceklesen3, this.gostergeTur, gostergeIlerlemeYon); } }
        public double gostOran4 { get { return Oranla(donemBaslangic, tahmin4, gerceklesen4, this.gostergeTur, gostergeIlerlemeYon); } }

        /// <summary>
        /// Gösterge yazılırken kaç noktadan sonra kaç basamak gösterileceği
        /// </summary>
        public int ondalikSayisi { get; set; }
    }

    /// <summary>
    /// Performans bütçe modülü sunucu sınıfı tarafından sunulan fonksiyonları belirten arayüz sınıfı
    /// </summary>
    public interface IPEBServis
    {
        /// <summary>
        /// Dil varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Dil nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dil">Dil bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc DilKaydet(Kullanici kullanan, Dil dil);

        /// <summary>
        /// Verilen kriterlere uygun olan dil bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dil">Dil bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında dil listesi döndürülür.</returns>
        ObjectArray DilListele(Kullanici kullanan, Dil dil);

        /// <summary>
        /// Dil, stratejik planda kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Dil nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dil">Dil bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc DilSil(Kullanici kullanan, Dil dil);

        /// <summary>
        /// Misyon varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Misyon nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="misyon">Misyon bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc MisyonKaydet(Kullanici kullanan, Misyon misyon);

        /// <summary>
        /// Verilen kriterlere uygun olan misyon bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="misyon">Misyon bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında misyon nesnesi döndürülür.</returns>
        ObjectArray MisyonListele(Kullanici kullanan, Misyon misyon);

        /// <summary>
        /// Vizyon varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Vizyon nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="vizyon">Vizyon bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc VizyonKaydet(Kullanici kullanan, Vizyon vizyon);

        /// <summary>
        /// Verilen kriterlere uygun olan vizyon bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="vizyon">Vizyon bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında vizyon nesnesi döndürülür.</returns>
        ObjectArray VizyonListele(Kullanici kullanan, Vizyon vizyon);

        Sonuc BirimMisyonVizyonKaydet(Kullanici kullanan, BirimMisyonVizyon misyonVizyon, string formId);
        Sonuc BirimMisyonVizyonSil(Kullanici kullanan, BirimMisyonVizyon misyonVizyon, string formId);
        BirimMisyonVizyon BirimMisyonVizyonListele(Kullanici kullanan, BirimMisyonVizyon misyonVizyon);
        BirimMisyonVizyon[] BirimMisyonVizyonSorgula(Kullanici kullanan, BirimMisyonVizyon misyonVizyon);

        Sonuc KurumsalAnalizKaydet(Kullanici kullanan, KurumsalAnaliz[] ka, string formId);
        Sonuc KurumsalAnalizSil(Kullanici kullanan, KurumsalAnaliz ka, string formId);
        KurumsalAnaliz[] KurumsalAnalizListele(Kullanici kullanan, KurumsalAnaliz ka);
        KurumsalAnaliz[] KurumsalAnalizSorgula(Kullanici kullanan, KurumsalAnaliz ka);

        Sonuc KurumsalAnalizKaydetSablon(Kullanici kullanan, KurumsalAnalizSablon sablon);
        Sonuc KurumsalAnalizSilSablon(Kullanici kullanan, KurumsalAnalizSablon sablon);
        KurumsalAnalizSablon[] KurumsalAnalizListeleSablon(Kullanici kullanan, KurumsalAnalizSablon sablon);
        KurumsalAnaliz[] KurumsalAnalizListeleSablonDetay(Kullanici kullanan, KurumsalAnaliz ka);

        /// <summary>
        /// Tema varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Tema nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tema">Tema bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc TemaKaydet(Kullanici kullanan, Tema tema);

        /// <summary>
        /// Verilen kriterlere uygun olan tema bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tema">Tema bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tema listesi döndürülür.</returns>
        ObjectArray TemaListele(Kullanici kullanan, Tema tema);

        /// <summary>
        /// Verilen dönem başlangıç yılına ait tema özet/kısa bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tema listesi döndürülür.</returns>
        ObjectArray TemaKisaListele(Kullanici kullanan, string kurumKod, int donemBYil);

        /// <summary>
        /// Tema, stratejik amaçta kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// Tema nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="temalar">Tema nesnelerini tutan liste</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc TemaSil(Kullanici kullanan, string kurumKod, TNSCollection temalar);

        /// <summary>
        /// Stratejik amaç varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Stratejik Amaç nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="strAmac">Stratejik amaç bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc StratejikAmacKaydet(Kullanici kullanan, StratejikAmac strAmac);

        /// <summary>
        /// Verilen kriterlere uygun olan stratejik amaç bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="strAmac">Stratejik amaç bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik amaç listesi döndürülür.</returns>
        ObjectArray StratejikAmacListele(Kullanici kullanan, StratejikAmac strAmac);

        /// <summary>
        /// Verilen dönem başlangıç yılına ait stratejik amaç özet/kısa bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik amaç listesi döndürülür.</returns>
        ObjectArray StratejikAmacKisaListele(Kullanici kullanan, string kurumKod, int donemBYil);

        /// <summary>
        /// Stratejik amaç, stratejik hedefte kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// Stratejik Amaç nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="amaclar">Stratejik Amaç nesnelerini tutan liste</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc StratejikAmacSil(Kullanici kullanan, TNSCollection amaclar);

        /// <summary>
        /// Stratejik hedef varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Stratejik Hedef nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="strHedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc StratejikHedefKaydet(Kullanici kullanan, StratejikHedef strHedef);

        /// <summary>
        /// Verilen kriterlere uygun olan stratejik hedef bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="strHedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik hedef listesi döndürülür.</returns>
        ObjectArray StratejikHedefListele(Kullanici kullanan, StratejikHedef strHedef, bool hedefler);

        /// <summary>
        /// Verilen dönem başlangıç yılına ait stratejik hedef özet/kısa bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik hedef listesi döndürülür.</returns>
        ObjectArray StratejikHedefKisaListele(Kullanici kullanan, string kurumKod, int donemBYil);

        /// <summary>
        /// Stratejik hedef, performans hedefte kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// Stratejik Hedef nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hedefler">Stratejik Hedef nesnelerini tutan liste</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc StratejikHedefSil(Kullanici kullanan, TNSCollection hedefler);

        /// <summary>
        /// Ölçü birim varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen OlcuBirim nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="olcuBirim">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc OlcuBirimKaydet(Kullanici kullanan, OlcuBirim olcuBirim);

        /// <summary>
        /// Verilen kriterlere uygun olan ölçü birim bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="olcuBirim">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ölçü birim listesi döndürülür.</returns>
        ObjectArray OlcuBirimListele(Kullanici kullanan, OlcuBirim olcuBirim);

        /// <summary>
        /// Ölçü birim, performans göstergede kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen OlcuBirim nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="olcuBirim">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc OlcuBirimSil(Kullanici kullanan, OlcuBirim olcuBirim);

        /// <summary>
        /// Performans gösterge varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGosterge nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gosterge">Performans gösterge bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc PerformansGostergeKaydet(Kullanici kullanan, PerformansGosterge gosterge);

        /// <summary>
        /// Verilen kriterlere uygun olan performans gösterge bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gosterge">Performans gösterge bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans gösterge listesi döndürülür.</returns>
        ObjectArray PerformansGostergeListele(Kullanici kullanan, PerformansGosterge gosterge);

        /// <summary>
        /// Performans gösterge, hedeflenen ve gerçekleşen değerlerde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGosterge nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gosterge">Performans gösterge bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc PerformansGostergeSil(Kullanici kullanan, PerformansGosterge gosterge);

        /// <summary>
        /// Performans göstergesi hedeflenen değerlerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGostergeHedeflenen nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedeflenen">Performans göstergesi hedeflenen değerlerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc PerformansGostergeHedeflenenKaydet(Kullanici kullanan, PerformansGostergeHedeflenen pHedeflenen);

        /// <summary>
        /// Verilen kriterlere uygun olan performans göstergesi hedeflenen değerleri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGostergeHedeflenen nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedeflenen">Performans göstergesi hedeflenen değerlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans göstergesi hedeflenen değerler listesi döndürülür.</returns>
        ObjectArray PerformansGostergeHedeflenenListele(Kullanici kullanan, PerformansGostergeHedeflenen pHedeflenen);

        /// <summary>
        /// Performans göstergesi gerçekleşen değerlerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGostergeGerceklesen nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gerceklesenler">Performans göstergesi gerçekleşen değerlerini tutan PerformansGostergeGerceklesen yapısında nesneler tutan dizi.</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc PerformansGostergeGerceklesenKaydet(Kullanici kullanan, TNSCollection gerceklesenler);

        /// <summary>
        /// Verilen kriterlere uygun olan performans göstergesi gerçekleşen değerleri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGostergeGerceklesen nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pGerceklesen">Performans göstergesi gerçekleşen değerlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans göstergesi gerçekleşen değerler listesi döndürülür.</returns>
        ObjectArray PerformansGostergeGerceklesenListele(Kullanici kullanan, PerformansGostergeGerceklesen pGerceklesen);

        Sonuc PerformansGostergeGerceklesenOnayla(Kullanici kullanan, PerformansGostergeGerceklesen pGerceklesen);

        Sonuc PerformansGostergeGerceklesenOnayKaldir(Kullanici kullanan, PerformansGostergeGerceklesen pGerceklesen);

        bool PerformansGostergeGerceklesenOnayliMi(Kullanici kullanan, PerformansGostergeGerceklesen pGerceklesen);

        /// <summary>
        /// Performans hedef varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansHedef nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// Performans hedefin stratejik hedeflerle bağının ve ağırlık bilgilerinin de ilgili yordamı çağırarak kaydedilmesini sağlar.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedef">Performans hedef bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc PerformansHedefKaydet(Kullanici kullanan, PerformansHedef pHedef);

        /// <summary>
        /// Verilen kriterlere uygun olan performans hedef bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedef">Performans hedef bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Performans hedefin stratejik hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedef programatik kodu</param>
        /// <param name="kurumDahil">Kurumun performans hedeflerinin listeye dahil edilip edilmeyeceği bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        ObjectArray PerformansHedefListele(Kullanici kullanan, PerformansHedef pHedef, bool detayAgirlik, int donemBYil, int strHedefKod, bool kurumDahil);

        /// <summary>
        /// Verilen kriterlere uygun olan performans hedef bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedef">Performans hedef bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Performans hedefin stratejik hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="detayGosterge">Performans hedefe ait performans gösterge bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="kurumDahil">Kurumun performans hedeflerinin listeye dahil edilip edilmeyeceği bilgisi</param>
        /// <param name="asama">Performans göstergesi hedeflenen değerlerinin istendiği aşama</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        ObjectArray PerformansHedefListele(Kullanici kullanan, PerformansHedef pHedef, bool detayAgirlik, bool detayGosterge, bool kurumDahil, int asama);

        /// <summary>
        /// Verilen kriterlere uygun olan performans hedef bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedef">Performans hedef bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Performans hedefin stratejik hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedef programatik kodu</param>
        /// <param name="detayGosterge">Performans hedefe ait performans gösterge bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="kurumDahil">Kurumun performans hedeflerinin listeye dahil edilip edilmeyeceği bilgisi</param>
        /// <param name="asama">Performans göstergesi hedeflenen değerlerinin istendiği aşama</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        ObjectArray PerformansHedefListele(Kullanici kullanan, PerformansHedef pHedef, bool detayAgirlik, int donemBYil, int strHedefKod, bool detayGosterge, bool kurumDahil, int asama);

        /// <summary>
        /// Performans hedefi, faaliyet/projelerde, performans göstergesi hedeflenen/gerçekleşen değerlerinde
        /// ve sabit maliyetlerde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedef">PerformansHedef nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc PerformansHedefSil(Kullanici kullanan, TNSCollection pHedef);

        /// <summary>
        /// Faaliyet/proje varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen FaaliyetProje nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// Faaliyet/projenin performans hedeflerle bağının ve ağırlık bilgilerinin de ilgili yordamı çağırarak kaydedilmesini sağlar.
        /// Aynı zamanda bütçe hazırlık alt faaliyetlerle bağının da ilgili yordamı çağırarak kaydedilmesini sağlar.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc FaaliyetProjeKaydet(Kullanici kullanan, FaaliyetProje fp);

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/proje bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Faaliyet/projenin performans hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="aktarEbutce">e-butce sistemine aktarım için mi çağrılıyor yoksa sistem kullanımı için mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje listesi döndürülür.</returns>
        ObjectArray FaaliyetProjeListele(Kullanici kullanan, FaaliyetProje fp, bool detayAgirlik, bool aktarEbutce);

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/proje bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Faaliyet/projenin performans hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="perHedefKod">Performans hedef programatik kodu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje listesi döndürülür.</returns>
        ObjectArray FaaliyetProjeListele(Kullanici kullanan, FaaliyetProje fp, bool detayAgirlik, int perHedefKod);

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/proje bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Faaliyet/projenin performans hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="perHedefKod">Performans hedef programatik kodu</param>
        /// <param name="aktarEbutce">e-butce sistemine aktarım için mi çağrılıyor yoksa sistem kullanımı için mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje listesi döndürülür.</returns>
        ObjectArray FaaliyetProjeListele(Kullanici kullanan, FaaliyetProje fp, bool detayAgirlik, int perHedefKod, bool aktarEbutce);

        /// <summary>
        /// Faaliyet/proje, sabit maliyetlerde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fpler">FaaliyetProje nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc FaaliyetProjeSil(Kullanici kullanan, TNSCollection fpler);

        /// <summary>
        /// Faaliyet/projenin ilgili birimlerini alt faaliyet bağı üzerinden bütçe hazırlıkta
        /// ödenek girişleri sırasında kullanılmış tertiplerden bulup döndüren yordam
        /// Faaliyet/projeler e-butce sistemine aktarılırken ilgili birimleri de istendiği için yazıldı.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgisini tutan nesne</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/projeye ait ilgili birimler string olarak liste şeklinde döndürülür.</returns>
        ObjectArray FaaliyetProjeIlgiliBirimleriBul(Kullanici kullanan, FaaliyetProje fp, string senaryoKod);

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/proje ödenek bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fpOdenek">Faaliyet/proje ödenek bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje ödenek listesi döndürülür.</returns>
        ObjectArray FaaliyetProjeOdenekListele(Kullanici kullanan, FaaliyetProjeOdenek fpOdenek);

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/proje ödenek bilgilerini döndüren yordam
        /// Ödenekler e-butce sisteminin istediği yapıda yani ekonomik kodun ilk kırılımı bazında toplanıyor.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="tur">ENUMFaaliyetProjeTur listesindeki türlerden biri olmalıdır.</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje ödenek listesi döndürülür.</returns>
        ObjectArray FaaliyetProjeOdenekleriEbutce(Kullanici kullanan, int yil, string senaryoKod, int tur);

        /// <summary>
        /// Sabit maliyet varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen SabitMaliyet nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// Sabit maliyetin bütçe hazırlık alt faaliyetlerle bağının da ilgili yordamı çağırarak kaydedilmesini sağlar.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sm">Sabit maliyet bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc SabitMaliyetKaydet(Kullanici kullanan, SabitMaliyet sm);

        /// <summary>
        /// Verilen kriterlere uygun olan sabit maliyet bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// Sabit maliyetin bütçe ile bağını kuran alt faaliyet bilgilerini de listeye ekler.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sm">Sabit maliyet bilgisini tutan nesne</param>
        /// <param name="detayOdenek">Sabit maliyetin ödenek bilgilerinin istenip istenmediği bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet listesi döndürülür.</returns>
        ObjectArray SabitMaliyetListele(Kullanici kullanan, SabitMaliyet sm, bool detayOdenek);

        /// <summary>
        /// Sabit maliyeti ve detay bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="smler">SabitMaliyet nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc SabitMaliyetSil(Kullanici kullanan, TNSCollection smler);

        /// <summary>
        /// Verilen kriterlere uygun olan sabit maliyet ödenek bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="smOdenek">Sabit maliyet ödenek bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet ödenek listesi döndürülür.</returns>
        ObjectArray SabitMaliyetOdenekListele(Kullanici kullanan, SabitMaliyetOdenek smOdenek);

        /// <summary>
        /// Verilen kriterlere uygun olan sabit maliyet ödenek bilgilerini döndüren yordam
        /// Ödenekler e-butce sisteminin istediği yapıda yani ekonomik kodun ilk kırılımı bazında toplanıyor.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet ödenek listesi döndürülür.</returns>
        ObjectArray SabitMaliyetOdenekleriEbutce(Kullanici kullanan, int yil, string senaryoKod);

        /// <summary>
        /// Sabit maliyet ağırlık bilgilerini alarak sabit maliyetin ödeneğini bu ağırlıkları
        /// kullanarak ilgili faaliyet/projelere yansıyan ödenekleri hesaplayıp kaydeden yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="smAgirlik">Sabit maliyet ağırlık bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc SabitMaliyetAgirlikKaydet(Kullanici kullanan, TNSCollection smAgirlik);

        /// <summary>
        /// Verilen kriterlere uygun olan sabit maliyet ağırlık ve yansıyan ödenek bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="smAgirlik">Sabit maliyet ağırlık bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet ağırlık ve yansıyan ödenek listesi döndürülür.</returns>
        ObjectArray SabitMaliyetAgirlikListele(Kullanici kullanan, SabitMaliyetAgirlik smAgirlik);

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/projeleri bulan ve ödeneklerinden ağırlıklarını hesaplıp döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="smAgirlik">Sabit maliyet ağırlık bilgilerini tutan nesne</param>
        /// <param name="tur">ENUMSabitMaliyetTur listesindeki türlerden biri olmalıdır.</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet ağırlık listesi döndürülür.</returns>
        ObjectArray SabitMaliyetAgirlikHesapla(Kullanici kullanan, SabitMaliyetAgirlik smAgirlik, int tur);

        /// <summary>
        /// Verilen kriterlere uygun olan birim bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Kurum kodu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında birim listesi döndürülür.</returns>
        ObjectArray BirimListele(Kullanici kullanan, int yil, string kurKod);

        /// <summary>
        /// Verilen stratejik hedefe ait performans hedefleri ve ödeneklerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedef programatik kodu</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        ObjectArray StrHedefRaporla(Kullanici kullanan, int donemBYil, int strHedefKod, string senaryoKod);

        /// <summary>
        /// Verilen stratejik amaca ait stratejik hedefleri ve ödeneklerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strAmacKod">Stratejik amaç programatik kodu</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında StrAmacRapor nesnesi döndürülür.</returns>
        ObjectArray StrAmacRaporla(Kullanici kullanan, int donemBYil, int strAmacKod, string senaryoKod);

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet, proje ve sabit maliyet ödeneklerini
        /// ve yansıtılan ödenekleri tertip bazında ekonomik kodun 4. düzeyinde toplayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında OdenekDurumListesi nesnesi döndürülür.</returns>
        ObjectArray OdenekDurumRaporla(Kullanici kullanan, int yil, string kurKod, string odenekSenaryo);

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet, proje ve sabit maliyet ödeneklerini
        /// ve yansıtılan ödenekleri tertip bazında ekonomik kodun 2. düzeyinde toplayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında OdenekDurumListesi nesnesi döndürülür.</returns>
        ObjectArray OdenekDurumRaporla2Duzey(Kullanici kullanan, int yil, string kurKod, string odenekSenaryo);

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/projeleri, ödeneklerini ve sabit maliyetlerden
        /// yansıyan ödenekleri toplayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ButceFaaliyetDagilim nesneleri listesi döndürülür.</returns>
        ObjectArray ButceFaaliyetDagilimRaporla(Kullanici kullanan, int yil, string odenekSenaryo);

        /// <summary>
        /// Verilen kriterlere uyan temaları, stratejik amaçları, stratejik hedefleri,
        /// performans hedeflerini, faaliyet/projeleri ve ödeneklerini toparlayıp döndüren yordam
        /// Ödeneklerin toplamı ve birim bazında dağılımı gösteriliyor ayrı ayrı.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında TemaOdenek nesneleri listesi döndürülür.</returns>
        ObjectArray PerformansHedefDagilimRaporla(Kullanici kullanan, int donemBYil, int yil, string senaryoKod, string kurKod);

        /// <summary>
        /// Verilen performans hedefin bilgilerini, destekleyen faaliyet/proje ağırlıklarını
        /// ve ödeneklerini, bağlı olan performans göstergelerini ve değerlerini,
        /// bağlı olduğu stratejik hedefleri ve ağırlıklarını toparlayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hedef">Performans hedefi bilgilerini tutan nesne</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        ObjectArray PerformansHedefKartiListele(Kullanici kullanan, PerformansHedef hedef, string odenekSenaryo, int gercekYil, int fpTur, int yansiyan);

        /// <summary>
        /// Verilen faaliyet/projenin bilgilerini, bağlı olduğu performans hedeflerini
        /// ve ağırlıklarını, ekonomik kod, fonksiyonel kod ve finansman kod bazında
        /// ödenek toplamlarını toparlayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgilerini tutan nesne</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje listesi döndürülür.</returns>
        ObjectArray FaaliyetProjeKartiListele(Kullanici kullanan, FaaliyetProje fp, string odenekSenaryo);

        /// <summary>
        /// Verilen sabit maliyetin bilgilerini, ödeneklerinin yansıtıldığı faaliyet/projeleri
        /// ve ağırlıklarını, ekonomik kod, fonksiyonel kod ve finansman kod bazında
        /// ödenek toplamlarını toparlayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sm">Sabit maliyet bilgilerini tutan nesne</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet listesi döndürülür.</returns>
        ObjectArray SabitMaliyetKartiListele(Kullanici kullanan, SabitMaliyet sm, string odenekSenaryo);

        /// <summary>
        /// Verilen kriterlere uyan stratejik amacı veya amaçları ve ödeneklerini bulup döndüren yordam
        /// Performans bütçede yer almayan ödenekler de bulunur ve listeye eklenir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strAmacKod">Stratejik amaç programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında PEBOzetRapor nesnesi döndürülür.</returns>
        ObjectArray PEBOzetRaporla(Kullanici kullanan, int donemBYil, int strAmacKod, int yil, string odenekSenaryo);

        /// <summary>
        /// Verilen kriterlere uyan stratejik hedefleri ve ekonomik kod bazında ödeneklerini bulup döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında StratejikHedefOdenekDagilim nesneleri listesi döndürülür.</returns>
        ObjectArray StrHedefOdenekEkoDagilimRaporla(Kullanici kullanan, int yil, string senaryoKod);

        /// <summary>
        /// Verilen kriterlere uyan stratejik hedefleri ve fonksiyonel kod bazında ödeneklerini bulup döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında StratejikHedefOdenekDagilim nesneleri listesi döndürülür.</returns>
        ObjectArray StrHedefOdenekFonDagilimRaporla(Kullanici kullanan, int yil, string senaryoKod);

        /// <summary>
        /// Verilen kriterlere uyan stratejik amaçları ve ekonomik kod bazında ödeneklerini bulup döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında StratejikAmacOdenekDagilim nesneleri listesi döndürülür.</returns>
        ObjectArray StrAmacOdenekEkoDagilimRaporla(Kullanici kullanan, int yil, string senaryoKod);

        /// <summary>
        /// Verilen kriterlere uyan stratejik amaçları ve fonksiyonel kod bazında ödeneklerini bulup döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında StratejikAmacOdenekDagilim nesneleri listesi döndürülür.</returns>
        ObjectArray StrAmacOdenekFonDagilimRaporla(Kullanici kullanan, int yil, string senaryoKod);

        /// <summary>
        /// Verilen kriterlere uyan performans göstergelerini ve ödeneklerini toplayıp döndüren yordam
        /// Bu raporda, performans göstergeleri hedeflenen değerler giriş ekranında sadece ilk sıraya girilen
        /// performans göstergeleri listeleniyor ve çıktıda performans hedefleri olarak görüntüleniyor.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans gösterge listesi döndürülür.</returns>
        ObjectArray PerformansGostergeOdenekRaporla(Kullanici kullanan, int yil, string senaryoKod, string kurKod);

        /// <summary>
        /// Verilen kriterlere uyan performans göstergelerini, hedeflenen ve gerçekleşen değerlerini,
        /// bağlı olduğu performans hedeflerini ve stratejik hedefleri toparlayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında BirimGostergeleriRaporu nesneleri listesi döndürülür.</returns>
        ObjectArray BirimGostergeleriRaporla(Kullanici kullanan, int yil, string kurKod, int[] gostergeSeviye);

        /// <summary>
        /// Verilen performans hedefi ve performans göstergesi kriterlerine uyan performans
        /// göstergesinin hedeflenen ve gerçekleşen değerlerini toparlayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedef">Performans hedef bilgilerini tutan nesne</param>
        /// <param name="pGosterge">Performans gösterge bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef nesnesi döndürülür.</returns>
        ObjectArray PerformansGostergeDetayRaporla(Kullanici kullanan, PerformansHedef pHedef, PerformansGosterge pGosterge);

        /// <summary>
        /// Verilen kriterlere uyan performan göstergelerini, hedeflenen ve gerçekleşen değerlerini toplayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında PerformansGostergeGerceklesen nesneleri listesi döndürülür.</returns>
        ObjectArray PerformansGostergeRaporla(Kullanici kullanan, int yil, string kurKod, int[] gostergeSeviye);

        /// <summary>
        /// Verilen kriterlere uyan performan göstergelerini, hedeflenen ve gerçekleşen değerlerini toplayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="perHedef">Performans hedef bilgilerini tutan nesne</param>
        /// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında PerformansGostergeGerceklesen nesneleri listesi döndürülür.</returns>
        ObjectArray PerformansGostergeRaporla(Kullanici kullanan, PerformansHedef perHedef, int[] gostergeSeviye);

        /// <summary>
        /// Verilen kriterlere uyan stratejik hedefleri ve bağlı olan I. ve II. seviye performans göstergelerini toparlayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="asama">Aşama kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans gösterge listesi döndürülür.</returns>
        ObjectArray StratejikGostergeRaporla(Kullanici kullanan, int donemBYil, int yil, int asama);

        /// <summary>
        /// Tedbir varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Tedbir nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// Tedbirin detay bilgileri ve detayın da mevcut durum bilgileri de kayıt edilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tedbir">Tedbir bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc TedbirKaydet(Kullanici kullanan, Tedbir tedbir);

        /// <summary>
        /// Verilen kriterlere uygun olan tedbir, detay ve mevcut durum bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tedbir">Tedbir bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tedbir nesnesi döndürülür.</returns>
        ObjectArray TedbirListele(Kullanici kullanan, Tedbir tedbir);

        /// <summary>
        /// Verilen kriterlere uygun olan tedbir mevcut tarihlerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında mevcut tarih listesi döndürülür.</returns>
        ObjectArray TedbirMevcutTarihListele(Kullanici kullanan, int yil, int kurKod);

        /// <summary>
        /// Tedbir birim listesini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tedbir birimleri listesi döndürülür.</returns>
        ObjectArray TedbirBirimListele(Kullanici kullanan);

        /// <summary>
        /// Daire varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Daire nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="daire">Daire bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc DaireKaydet(Kullanici kullanan, Daire daire);

        /// <summary>
        /// Verilen kriterlere uygun olan daire bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="daire">Daire bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında daire listesi döndürülür.</returns>
        ObjectArray DaireListele(Kullanici kullanan, Daire daire);

        /// <summary>
        /// Daire, operasyonel plan alt faaliyetlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Daire nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="daire">Daire bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc DaireSil(Kullanici kullanan, Daire daire);

        /// <summary>
        /// Operasyonel plan alt faaliyeti varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AltFaaliyet nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="af">Alt faaliyet bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc AltFaaliyetKaydet(Kullanici kullanan, AltFaaliyet af);

        /// <summary>
        /// Verilen kriterlere uygun olan operasyonel plan alt faaliyeti bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="af">Alt faaliyet bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında operasyonel plan alt faaliyeti listesi döndürülür.</returns>
        ObjectArray AltFaaliyetListele(Kullanici kullanan, AltFaaliyet af);

        /// <summary>
        /// Operasyonel plan alt faaliyeti, iş/görevde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AltFaaliyet nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="af">Alt faaliyet bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc AltFaaliyetSil(Kullanici kullanan, AltFaaliyet af);


        /// <summary>
        /// İş/görev varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen IsGorev nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// İş/görevin zaman planı, sorumlu personelleri ve performans göstergeleri de ilgili yordamlar çağırılarak kayıt edilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc IsGorevKaydet(Kullanici kullanan, IsGorev ig);

        /// <summary>
        /// Verilen kriterlere uygun olan iş/görev bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// detayVer parametresi true ise iş/görevin zaman planı, sorumlu
        /// personelleri ve performans göstergeleri de listeye eklenir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <param name="detayVer">İş/göreve ait detay bilgiler listelensin mi listelenmesin mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında iş/görev listesi döndürülür.</returns>
        ObjectArray IsGorevListele(Kullanici kullanan, IsGorev ig, bool detayVer);

        /// <summary>
        /// İş/görev bilgilerini, zaman planını, sorumlu personellerini ve performans göstergelerini silen yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="igler">IsGorev nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc IsGorevSil(Kullanici kullanan, TNSCollection igler);

        /// <summary>
        /// Verilen kriterlere uygun olan iş/görev bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// İş/görevin zaman planı, sorumlu personelleri ve performans göstergeleri de listeye eklenir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında iş/görev listesi döndürülür.</returns>
        ObjectArray OperasyonelPlanYazdir(Kullanici kullanan, IsGorev ig);

        /// <summary>
        /// Parametre olarak verilen personelin iş/görev bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// detayVer parametresi true ise iş/görevin zaman planı, sorumlu
        /// personelleri ve performans göstergeleri de listeye eklenir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="tcKimlikNo">İş/görev bilgileri istenen personelin T.C. kimlik numarası</param>
        /// <param name="detayVer">İş/göreve ait detay bilgiler listelensin mi listelenmesin mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında iş/görev listesi döndürülür.</returns>
        ObjectArray PersonelIsGorevListele(Kullanici kullanan, int yil, string tcKimlikNo, bool detayVer);

        Sonuc SPGenelHavuzKaydet(Kullanici kullanan, SpGenelHavuz sp);
        SpGenelHavuz SPGenelHavuzListele(Kullanici kullanan, SpGenelHavuz sp);
        SpGenelHavuz[] SPGenelHavuzListele(Kullanici kullanan, int donemNo, int orgBirimKod, string spGenelHavuzKod, string[] ustSpGenelHavuzKod, EnumSPGenelHavuzDurum durum);
        SpGenelHavuz[] SPGenelHavuzListele(Kullanici kullanan, string spGenelHavuzKod, EnumSPGenelHavuzDurum durum, bool altBagiOlanlar, params string[] birimTur);
        SpGenelHavuz[] SPGenelHavuzListele(Kullanici kullanan, string spGenelHavuzKod, EnumSPGenelHavuzDurum durum, bool bagVer, bool altBagiOlanlar, params string[] birimTur);
        SpGenelHavuz[] SPGenelHavuzListele(Kullanici kullanan, string spGenelHavuzKod, string[] ustSpGenelHavuzKod, EnumSPGenelHavuzDurum durum, bool altBagiOlanlar, params string[] birimTur);

        SpGenelHavuzRapor[] SPGenelHavuzRaporListele(Kullanici kullanan, int donemNo, string birimTur, string ustKod);

        Sonuc DonemKaydet(Kullanici kullanan, Donem donem);
        Sonuc DonemSil(Kullanici kullanan, Donem donem);
        Donem[] DonemListele(Kullanici kullanan);
        Donem[] DonemListele(Kullanici kullanan, Donem donem);

        Sonuc GorevlendirmeKaydet(Kullanici kullanan, GorevlendirmeEkip[] ekipler, string formId);
        Sonuc GorevlendirmeSil(Kullanici kullanan, GorevlendirmeEkip ekip, string formId);
        GorevlendirmeEkip[] GorevlendirmeListele(Kullanici kullanan, GorevlendirmeEkip ekip, bool detay);
        GorevlendirmeEkip[] GorevlendirmeSorgula(Kullanici kullanan, GorevlendirmeEkip ekip);

        Sonuc StratejiBelirlemeKaydet(Kullanici kullanan, StratejiBelirleme[] stratejiler, string amacKod, string formId);
        Sonuc StratejiBelirlemeSil(Kullanici kullanan, StratejiBelirleme strateji, string amacKod, string formId);
        StratejiBelirleme[] StratejiBelirlemeListele(Kullanici kullanan, StratejiBelirleme strateji);
        StratejiBelirleme[] StratejiBelirlemeSorgula(Kullanici kullanan, StratejiBelirleme strateji);

        Sonuc BirimStratejikHedefMaliyetKaydet(Kullanici kullanan, BirimStratejikHedefMaliyet[] maliyetler, string formId);
        Sonuc BirimStratejikHedefMaliyetSil(Kullanici kullanan, BirimStratejikHedefMaliyet maliyet, string formId);
        BirimStratejikHedefMaliyet[] BirimStratejikHedefMaliyetListele(Kullanici kullanan, BirimStratejikHedefMaliyet maliyet);
        BirimStratejikHedefMaliyet[] BirimStratejikHedefMaliyetSorgula(Kullanici kullanan, BirimStratejikHedefMaliyet maliyet);
        BirimStratejikHedefMaliyetKatman[] BirimStratejikHedefMaliyetRapor(Kullanici kullanan, BirimStratejikHedefMaliyet maliyet);

        Sonuc BirimGenelHavuzKaydet(Kullanici kullanan, Birim_SpGenelHavuz[] birim_SpGenelHavuzlar, string formId);
        Sonuc BirimGenelHavuzSil(Kullanici kullanan, Birim_SpGenelHavuz birim_SpGenelHavuz, string formId);
        Birim_SpGenelHavuz[] BirimGenelHavuzListele(Kullanici kullanan, Birim_SpGenelHavuz birim_SpGenelHavuz);
        Birim_SpGenelHavuz[] BirimGenelHavuzSorgula(Kullanici kullanan, Birim_SpGenelHavuz birim_SpGenelHavuz);
        Birim_SpGenelHavuz[] BirimGenelHavuzAgacGetir(Kullanici kullanan, int donemNo, int orgBirimKod, EnumSPGenelHavuzTur girisNoktasi, EnumSPGenelHavuzTur cikisNoktasi);

        Sonuc GzftKaydet(Kullanici kullanan, Gzft gzft, string formId, bool sadeceAciklama);
        Sonuc GzftSil(Kullanici kullanan, Gzft gzft, string formId);
        Gzft GzftTekListele(Kullanici kullanan, Gzft gzft);
        Gzft[] GzftListele(Kullanici kullanan, Gzft gzft, bool detay);
        Gzft[] GzftSorgula(Kullanici kullanan, Gzft gzft);

        Sonuc AsamaKontrolKaydet(Kullanici kullanan, AsamaKontrol asamaKontrol, ENUMAsamaKontrolIslemTur islem);
        AsamaKontrol[] AsamaKontrolListele(Kullanici kullanan, AsamaKontrol asamaKontrol);
        AsamaKontrol[] AsamaKontrolSorgula(Kullanici kullanan, AsamaKontrol asamaKontrol);

        Sonuc AsamaKontrolKaydetGecmis(Kullanici kullanan, AsamaKontrolGecmis asamaKontrolGecmis, ENUMAsamaKontrolIslemTur islem);
        AsamaKontrolGecmis[] AsamaKontrolListeleGecmis(Kullanici kullanan, AsamaKontrolGecmis asamaKontrolGecmis);

        Sonuc PestKaydet(Kullanici kullanan, Pest pest, string formId);
        Sonuc PestSil(Kullanici kullanan, Pest pest, string formId);
        Pest[] PestListele(Kullanici kullanan, Pest pest, bool detay);
        Pest[] PestSorgula(Kullanici kullanan, Pest pest);

        Sonuc PaydasAnalizKaydet(Kullanici kullanan, PaydasAnaliz paydasAnaliz, string formId);
        Sonuc PaydasAnalizSil(Kullanici kullanan, PaydasAnaliz paydasAnaliz, string formId);
        PaydasAnaliz[] PaydasAnalizListele(Kullanici kullanan, PaydasAnaliz paydasAnaliz, bool detay);
        PaydasAnaliz[] PaydasAnalizSorgula(Kullanici kullanan, PaydasAnaliz paydasAnaliz);

        Sonuc PlanlananKaydet(Kullanici kullanan, Planlanan planlanan, string formId);
        Sonuc PlanlananSil(Kullanici kullanan, Planlanan planlanan, string formId);
        Planlanan[] PlanlananListele(Kullanici kullanan, Planlanan planlanan);
        Planlanan[] PlanlananSorgula(Kullanici kullanan, Planlanan planlanan);

        Sonuc BirimHizmetPaydasIliskiKaydet(Kullanici kullanan, Birim_SpGenelHavuz_Paydas_Iliski[] bhpiler, string formId);
        Sonuc BirimHizmetPaydasIliskiSil(Kullanici kullanan, Birim_SpGenelHavuz_Paydas_Iliski bhpi, string formId);
        Birim_SpGenelHavuz_Paydas_Iliski[] BirimHizmetPaydasIliskiListele(Kullanici kullanan, Birim_SpGenelHavuz_Paydas_Iliski bhpi);
        Birim_SpGenelHavuz_Paydas_Iliski[] BirimHizmetPaydasIliskiSorgula(Kullanici kullanan, Birim_SpGenelHavuz_Paydas_Iliski bhpi);

        BirimHedef[] BirimHedefListele(Kullanici kullanan, BirimHedef kriter);
        BirimHedef[] BirimHedefSorgula(Kullanici kullanan, BirimHedef kriter);
        Sonuc BirimHedefKaydet(Kullanici kullanan, BirimHedef bilgi, string formId);

        BirimHedefKoordineBirim[] BirimHedefKoordineBirimListele(Kullanici kullanan, BirimHedefKoordineBirim kriter);
        Sonuc BirimHedefKoordineBirimKaydet(Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, ObjectArray bilgi, string formId);

        BirimHedefUstHedef[] BirimHedefUstHedefListele(Kullanici kullanan, BirimHedefUstHedef kriter);
        Sonuc BirimHedefUstHedefKaydet(Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, ObjectArray bilgi, string formId);

        BirimHedefSorunAlan[] BirimHedefSorunAlanListele(Kullanici kullanan, BirimHedefSorunAlan kriter);
        Sonuc BirimHedefSorunAlanKaydet(Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, ObjectArray bilgi, string formId);

        BirimHedefUstPolitika[] BirimHedefUstPolitikaListele(Kullanici kullanan, BirimHedefUstPolitika kriter);
        Sonuc BirimHedefUstPolitikaKaydet(Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, ObjectArray bilgi, string formId);

        BirimHedefGzft[] BirimHedefGzftListele(Kullanici kullanan, BirimHedefGzft kriter);
        Sonuc BirimHedefGzftKaydet(Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, ObjectArray bilgi, string formId);

        Sonuc MevzuatAnalizKaydet(Kullanici kullanan, MevzuatAnaliz mevzuat, string formId);
        Sonuc MevzuatAnalizSil(Kullanici kullanan, MevzuatAnaliz mevzuat, string formId);
        MevzuatAnaliz MevzuatAnalizListele(Kullanici kullanan, MevzuatAnaliz mevzuat);
        MevzuatAnaliz[] MevzuatAnalizSorgula(Kullanici kullanan, MevzuatAnaliz mevzuat);

        Sonuc TarihselGelisimKaydet(Kullanici kullanan, TarihselGelisim tg, string formId);
        Sonuc TarihselGelisimSil(Kullanici kullanan, TarihselGelisim tg, string formId);
        TarihselGelisim TarihselGelisimListele(Kullanici kullanan, TarihselGelisim tg);
        TarihselGelisim[] TarihselGelisimSorgula(Kullanici kullanan, TarihselGelisim tg);

        Sonuc HarcamaHesaplaKaydet(Kullanici kullanan, Harcama hk);
        //ObjectArray HarcamaAgirlikHesapla(Kullanici kullanan, Harcama hk);
        //Sonuc HarcamaKaydet(Kullanici kullanan, Harcama hk, TNSCollection agirliklar);
        //ObjectArray HarcamaListele(Kullanici kullanan, Harcama hk);
        //ObjectArray FarkliTertipDuzenleriListele(Kullanici kullanan, FaaliyetProjeOdenek fpOdenek);
        //Sonuc AsamaKopyala(Kullanici kullanan, AsamaKopyala bilgi);

        ///// <summary>
        ///// Verilen kriterlere uyan performans göstergelerini, hedeflenen ve gerçekleşen değerlerini,
        ///// bağlı olduğu performans hedeflerini ve stratejik hedefleri toparlayıp döndüren yordam
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="kurKod">Birim kodu</param>
        ///// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında BirimGostergeleriRaporu nesneleri listesi döndürülür.</returns>
        //ObjectArray BirimGostergeleriRaporlaEskiDB(Kullanici kullanan, int yil, string kurKod, int[] gostergeSeviye);

        ///// <summary>
        ///// Verilen performans hedefi ve performans göstergesi kriterlerine uyan performans
        ///// göstergesinin hedeflenen ve gerçekleşen değerlerini toparlayıp döndüren yordam
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="pHedef">Performans hedef bilgilerini tutan nesne</param>
        ///// <param name="pGosterge">Performans gösterge bilgilerini tutan nesne</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef nesnesi döndürülür.</returns>
        //ObjectArray PerformansGostergeDetayRaporlaEskiDB(Kullanici kullanan, PerformansHedef pHedef, PerformansGosterge pGosterge);

        ///// <summary>
        ///// Verilen kriterlere uyan performan göstergelerini, hedeflenen ve gerçekleşen değerlerini toplayıp döndüren yordam
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="kurKod">Birim kodu</param>
        ///// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında PerformansGostergeGerceklesen nesneleri listesi döndürülür.</returns>
        //ObjectArray PerformansGostergeRaporlaEskiDB(Kullanici kullanan, int yil, string kurKod, int[] gostergeSeviye);

        ///// <summary>
        ///// Verilen kriterlere uygun olan ölçü birim bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="olcuBirim">Ölçü birim bilgisini tutan nesne</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ölçü birim listesi döndürülür.</returns>
        //ObjectArray OlcuBirimListeleEskiDB(Kullanici kullanan, OlcuBirim olcuBirim);

        ///// <summary>
        ///// Verilen kriterlere uygun olan performans gösterge bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="gosterge">Performans gösterge bilgisini tutan nesne</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans gösterge listesi döndürülür.</returns>
        //ObjectArray PerformansGostergeListeleEskiDB(Kullanici kullanan, PerformansGosterge gosterge);

        ///// <summary>
        ///// Verilen kriterlere uygun olan performans hedef bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="pHedef">Performans hedef bilgisini tutan nesne</param>
        ///// <param name="detayAgirlik">Performans hedefin stratejik hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        ///// <param name="donemBYil">Dönem başlangıç yılı</param>
        ///// <param name="strHedefKod">Stratejik hedef programatik kodu</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        //ObjectArray PerformansHedefListeleEskiDB(Kullanici kullanan, PerformansHedef pHedef, bool detayAgirlik, int donemBYil, int strHedefKod);


        Sonuc PerformansHamVeriKaydet(Kullanici kullanan, PerformansHamVeri phVeri);
        PerformansHamVeri[] PerformansHamVeriListele(Kullanici kullanan, PerformansHamVeri phVeri);
        Daire[] KullanilanDaireBul(bool sadeceEskiKayitlar);
        Sonuc DaireyiOrgaCevir(Kullanici kullanan, int yil, string kurKod, int daireKod, int orgKod);


        string AyniNumaraliFPSMListele();

        void AyniNumaraliFPSMDuzelt(Kullanici kullanan, string faalSmKod);

        Sonuc HamVeriToplamAl(Kullanici kullanan, int yil, int IlceIlUlke, string ilceKodIlKod);


        ///// <summary>
        ///// LPUretim nesnesini veri tabanından okuyan fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">LPUretim yapısında LPUretim: bilgilerini tutan değişken</param>
        ///// <param name="detayOku">Okunan formların değişken bilgileri de okunacak mı?</param>
        //LPUretim[] LPUretimListele(Kullanici kullanan, LPUretim form, bool detayOku);

        ///// <summary>
        ///// LPUretim nesnesini veri tabanına yazan fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">LPUretim yapısında LPUretim: bilgilerini tutan değişken</param>
        ///// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        //Sonuc LPUretimKaydet(Kullanici kullanan, LPUretim form);

        ///// <summary>
        ///// Verilen dizideki formların durumu 1 yapılır.
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">LPUretim yapısında LPUretim: bilgilerini tutan değişken</param>
        ///// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        //Sonuc LPUretimeAl(Kullanici kullanan, LPUretim[] form);

        /// <summary>
        /// LPUretim dizisi ile verilen bilgileri veri tabanınından silen fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">LPUretim yapısında LPUretim: bilgilerini tutan değişken</param>
        ///// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        //Sonuc LPUretimSil(Kullanici kullanan, LPUretim[] form);

        //void LPCalistir(Kullanici kullanan, int yil);

        //Sonuc LPKontrolVerisiYukle(Kullanici kullanan, LPUretim form, string xmlVeri);

        Sonuc BirimAmacHedefOtomatikUret(Kullanici kullanan, int donemNo, int orgBirimKod, string formId);
        Sonuc BirimAmacHedefKaydet(Kullanici kullanan, Birim_AmacHedef bah, string formId);
        Sonuc BirimAmacHedefSil(Kullanici kullanan, Birim_AmacHedef bah, string formId);
        Birim_AmacHedef[] BirimAmacHedefListele(Kullanici kullanan, Birim_AmacHedef bah, bool detay, bool amac);
        Birim_AmacHedef[] BirimAmacHedefSorgula(Kullanici kullanan, Birim_AmacHedef bah, bool amac);

        Sonuc BirimFaaliyetMaliyetKaydet(Kullanici kullanan, BirimFaaliyetMaliyet[] maliyetler, string formId);
        Sonuc BirimFaaliyetMaliyetSil(Kullanici kullanan, BirimFaaliyetMaliyet maliyet, string formId);
        BirimFaaliyetMaliyet[] BirimFaaliyetMaliyetListele(Kullanici kullanan, BirimFaaliyetMaliyet maliyet);
        BirimFaaliyetMaliyet[] BirimFaaliyetMaliyetSorgula(Kullanici kullanan, BirimFaaliyetMaliyet maliyet);

        Sonuc RaporSablonKaydet(Kullanici kullanan, int sablonNo, RTFOutline[] rtfOutlineSatirlar);
        RTFOutline[] RaporSablonListele(Kullanici kullanan, int sablonNo, int merkezTasra);

        Sonuc RaporSablonTurDegistir(Kullanici kullanan, int sablonNo, RTFOutline ro);

        RTFOutline RaporSablonTurOku(Kullanici kullanan, int sablonNo, int kod);
        string RaporDetayOku(Kullanici kullanan, int sablonNo, int kod, int donemNo, int birimKod, int yil);

        Sonuc RaporDetayYaz(Kullanici kullanan, int sablonNo, int kod, int donemNo, int birimKod, int yil, string htmlMetin);

        BirimHedefFaaliyet[] BirimHedefFaaliyetListele(Kullanici kullanan, BirimHedefFaaliyet kriter);
        Sonuc BirimHedefFaaliyetKaydet(Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, TNSCollection faaliyetler, string formId);

        Sonuc BirimFaaliyetAgirlikKaydet(Kullanici kullanan, BirimFaaliyetAgirlik[] agirliklar, string formId);
        Sonuc BirimFaaliyetAgirlikSil(Kullanici kullanan, BirimFaaliyetAgirlik agirlik, string formId);
        BirimFaaliyetAgirlik[] BirimFaaliyetAgirlikListele(Kullanici kullanan, BirimFaaliyetAgirlik agirlik);

        Sonuc STRGostergeKaydet(Kullanici kullanan, STRGosterge gosterge);
        Sonuc STRGostergeSil(Kullanici kullanan, STRGosterge gosterge);
        STRGosterge[] STRGostergeListele(Kullanici kullanan, STRGosterge gosterge, bool detay, params string[] bagliKodlar);
        Sonuc STRGostergeFormulDerlenebilirMi(string formul);

        BirimHedefGosterge[] BirimHedefGostergeListele(Kullanici kullanan, BirimHedefGosterge kriter);
        Sonuc BirimHedefGostergeKaydet(Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, TNSCollection gostergeler, string formId);

        DegiskenSablon[] VZADegiskenSablonListele(Kullanici kullanan);

        DegiskenSablon VZADegiskenSablonTanimEkle(Kullanici kullanan);

        Sonuc VZADegiskenSablonAdDegistir(Kullanici kullanan, int kod, string metin);

        Sonuc VZADegiskenSablonSil(Kullanici kullanan, int kod);

        DegiskenSablon VZADegiskenSablonDetayOku(Kullanici kullanan, int kod);

        Sonuc VZADegiskenSablonDetayYaz(Kullanici kullanan, DegiskenSablon d);

        //KarsilastirmaGrubu[] VZAKarsilastirmaGruplariListele(Kullanici kullanan);

        //Sonuc VZAKarsilastirmaGrubuAdDegistir(Kullanici kullanan, int kod, string metin);

        //Sonuc VZAKarsilastirmaGrubuSil(Kullanici kullanan, int kod);

        //KarsilastirmaGrubu VZAKarsilastirmaGrubuTanimEkle(Kullanici kullanan);

        //KarsilastirmaGrubu VZAKarsilastirmaGrubuDetayOku(Kullanici kullanan, int kod);

        //Sonuc VZAKarsilastirmaGrubuDetayYaz(Kullanici kullanan, KarsilastirmaGrubu d);

        Sonuc BirimIsGorevMaliyetKaydet(Kullanici kullanan, BirimIsGorevMaliyet[] maliyetler, string formId);
        Sonuc BirimIsGorevMaliyetSil(Kullanici kullanan, BirimIsGorevMaliyet maliyet, string formId);
        BirimIsGorevMaliyet[] BirimIsGorevMaliyetListele(Kullanici kullanan, BirimIsGorevMaliyet maliyet);
        BirimIsGorevMaliyet[] BirimIsGorevMaliyetSorgula(Kullanici kullanan, BirimIsGorevMaliyet maliyet);

        Sonuc BirimHedefGostergeYillikKaydet(Kullanici kullanan, BirimHedefGostergeYillik[] gostergeler, string formId);
        Sonuc BirimHedefGostergeYillikSil(Kullanici kullanan, BirimHedefGostergeYillik gosterge, string formId);
        BirimHedefGostergeYillik[] BirimHedefGostergeYillikListele(Kullanici kullanan, BirimHedefGostergeYillik gosterge);
        BirimHedefGostergeYillik[] BirimHedefGostergeYillikSorgula(Kullanici kullanan, BirimHedefGostergeYillik gosterge);

        VZAAnalizGrup[] VZAAnalizGrupDetayListele(Kullanici kullanan);

        VZAAnalizGrup VZAAnalizGrupTanimEkle(Kullanici kullanan);

        Sonuc VZAAnalizGrupSil(Kullanici kullanan, int grupKod, int altKod);

        Sonuc VZAAnalizDetayTanimEkle(Kullanici kullanan, int grupKod, VZAAnalizDetay d);

        Sonuc VZAAnalizGrupAdDegistir(Kullanici kullanan, int kod, int altKod, string metin);

        Sonuc VZAAnalizGrupVarsayilanYap(Kullanici kullanan, int grupKod, int varsayilan);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="islemYil"></param>
        /// <param name="grupKod"></param>
        /// <param name="altKod"></param>
        /// <param name="listelenecekler">0: hepsi, 1: bekleyenler, 2: işleme alınmışlar (Hepsi), 3: başarılılar, 4: başarısızlar</param>
        /// <returns></returns>
        VZAAnalizDetay VZAIslemDurumListele(Kullanici kullanan, int islemYil, int grupKod, int altKod, int listelenecekler);

        Sonuc VZAIslemeAl(Kullanici kullanan, int islemYil, int grupKod, int altKod);

        Sonuc VZALPIslem(Kullanici kullanan, int islemYil, int grupKod, int altKod);
        VZASonucBilgi VZASonucDondur(Kullanici kullanan, int islemYil, int grupKod, int altKod, string cogKod);
        VZASonucBirim VZASonucDondur(Kullanici kullanan, int islemYil, int orgKod);
        Sonuc GostergeVeriKaynagindanCek(Kullanici kullanan, int orgBirimKod, int yil);
        Sonuc GostergeHesapla(Kullanici kullanan, int orgBirimKod, int yil);
        Decimal VeriKaynagiDegerBul(Kullanici kullanan, int yil, int orgKod, STRGosterge gosterge);
        Decimal VeriKaynagiDegerBulCografi(Kullanici kullanan, int yil, string cografiTur, int orgOrCogKod, STRGosterge gosterge);

        Sonuc BirimGostergeGerceklesenKaydet(Kullanici kullanan, BirimGostergeGerceklesen[] gostergeler, string formId);
        Sonuc BirimGostergeGerceklesenSil(Kullanici kullanan, BirimGostergeGerceklesen gosterge, string formId);
        BirimGostergeGerceklesen[] BirimGostergeGerceklesenListele(Kullanici kullanan, BirimGostergeGerceklesen gosterge);

        BirimGostergeGerceklesmeDurum BirimGostergeGerceklesmeDurumBul(Kullanici kullanan, int orgBirimKod, int yil, int ay);

        Sonuc BirimPerHedefKaydet(Kullanici kullanan, BirimPerHedef perHedef, string formId);
        Sonuc BirimPerHedefSil(Kullanici kullanan, BirimPerHedef perHedef, string formId);
        BirimPerHedef[] BirimPerHedefListele(Kullanici kullanan, BirimPerHedef perHedef);
        BirimPerHedef[] BirimPerHedefSorgula(Kullanici kullanan, BirimPerHedef perHedef);

        double VZANotBul(Kullanici kullanan, int yil, int orgKod);

        ObjectArray GostergeGerceklesenHedefGosterge(Kullanici kullanan, int yil, string kurKod, int ay);


        Sonuc PerformansGostergeHedeflenenKopyala(Kullanici kullanan, int yil, string ghKurKod, int kaynakAsama, int hedefAsama);

        Sonuc PerformansKopyala(Kullanici kullanan, int kaynakYil, int hedefYil);

        TNS.BHMv2.UcgenSatir[] OdenekListeleFGT(Kullanici kullanan, int yil, string senaryoKod, string kurKod, int ekoUz, bool sifirlarDahil);

        /// <summary>
        /// HEDEF listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">HEDEF yapısında HEDEF: Stratejik Hedef bilgilerini tutan değişken</param>
        ObjectArray ListeleStrHedefGosterge(Kullanici kullanan, int donemBasYil, int hedefKod, string kurumKod);

        /// <summary>
        /// HEDEF nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">HEDEF yapısında HEDEF: Stratejik Hedef bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetStrHedefGosterge(Kullanici kullanan, int donemBasYil, int hedefKod, string kurumKod, TNSCollection gostergeler);

        /// <summary>
        /// HEDEF listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">HEDEF yapısında HEDEF: Stratejik Hedef bilgilerini tutan değişken</param>
        ObjectArray ListeleStrHedefOdenek(Kullanici kullanan, int donemBasYil, int hedefKod, string kurumKod);

        /// <summary>
        /// HEDEF nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">HEDEF yapısında HEDEF: Stratejik Hedef bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetStrHedefOdenek(Kullanici kullanan, int donemBasYil, int hedefKod, string kurumKod, TNSCollection odenekler);

        ObjectArray StratejikFaaliyetListele(Kullanici kullanan, STRFaaliyet pHedef, bool detayli);

        Sonuc KaydetStratejikFaaliyet(Kullanici kullanan, STRFaaliyet form);

        ObjectArray ListeleStrHedefGostergeGerc(Kullanici kullanan, StratejikHedef hedef, int yil, int donem);

        Sonuc KaydetStrHedefGostergeGerc(Kullanici kullanan, TNSCollection gerceklesmeler, TNSCollection[] gercBilgiler, bool aktarim);

        ObjectArray ListeleStrHedefGercBilgi(Kullanici kullanan, StratejikHedef hedef, int yil, int donem, int bilgiTur);

        ObjectArray UstPolitikaBelgeleriListele(Kullanici kullanan, int yil);

        ObjectArray StratejikHedefRaporAl(Kullanici kullanan, StratejikHedef hedef, int raporNo);
        /// <summary>
        /// KurumGruplari listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KurumGruplari yapısında KurumGruplari: Kurum Grupları bilgilerini tutan değişken</param>
        ObjectArray ListeleKurumGrup(Kullanici kullanan, KurumGruplari form);

        /// <summary>
        /// KurumGruplari nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KurumGruplari yapısında KurumGruplari: Kurum Grupları bilgilerini tutan değişken</param>
        ObjectVAN OkuKurumGrup(Kullanici kullanan, KurumGruplari form);

        /// <summary>
        /// KurumGruplari nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KurumGruplari yapısında KurumGruplari: Kurum Grupları bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetKurumGrup(Kullanici kullanan, KurumGruplari form);

        Sonuc SilKurumGrup(Kullanici kullanan, KurumGruplari form);

        /// <summary>
        /// EylemPlani nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EylemPlani yapısında EylemPlani: bilgilerini tutan değişken</param>
        /// <param name="yil">Gerçekleşme bilgisinin okunacağı yıl</param>
        /// <param name="ay">Gerçekleşme bilgisinin okunacağı ay</param>
        /// <param name="detayli">Eylemlere ait gerçekleşme bilgileri ve ilgili birimler de okunur</param>
        ObjectArray ListeleEylemPlaniForm(Kullanici kullanan, EylemPlani form, int yil, int ay, int donemBasYil, bool detayli);

        /// <summary>
        /// EylemPlani nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="eylemID">Gerçekleşmesi okunacak eylemin IDsi</param>
        /// <param name="yil">Gerçekleşmesi okunacak dönemim yılı</param>
        /// <param name="ay">Gerçekleşmesi okunacak dönemim ayı</param>
        ObjectVAN OkuEylemPlaniGerceklesme(Kullanici kullanan, int eylemID, int yil, int ay);

        /// <summary>
        /// EylemPlani nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EylemPlani yapısında EylemPlani:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetEylemPlaniForm(Kullanici kullanan, EylemPlani form);
        /// <summary>
        /// EylemPlani nesnesini veri tabanından silen fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EylemPlani yapısında EylemPlani:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SilEylemPlaniForm(Kullanici kullanan, EylemPlani form);

        Sonuc KaydetEylemPlaniGerceklesme(Kullanici kullanan, int eylemID, EylemGerceklesme form);
        Sonuc EylemPlaniAgirlikGuncelle(Kullanici kullanan, int yil, int eylemID, int agirlik);
        Sonuc KaydetEylemPlaniYetki(Kullanici kullanan, int eylemId, TNSCollection kisiler);
        ObjectArray ListeleEylemPlaniYetki(Kullanici kullanan, int eylemID);
        ObjectArray ListeleEylemPlaniTarihce(Kullanici kullanan, int eylemID);
        ObjectArray ListeleEylemPlaniDonem(Kullanici kullanan, int eylemTuru, int yil, int ay);
        Sonuc AyarlaEylemPlaniDonem(Kullanici kullanan, int yil, int ay, int eylemID, int donemDurum);
        Sonuc MisyonDonemKopyala(Kullanici kullanan, int v1, int v2);
    }
}