using System;
using TNS.KYM;
using TNS.Veri;
using TNS.BUM.IsKurallari.Veri;
using TNS.BKD;

namespace TNS.BUM.IsKurallari
{
    /// <summary>
    /// Bütçe Uygulama Yetki Servisi Sınıfı
    /// </summary>
    public class BUMYetkiServisi
    {
        /// <summary>
        /// Bütçe Uygulama Görme, yetki değişken tanımı
        /// </summary>
        public static string YT_ButceUygulamaGorme = "Bütçe Uygulama Görme Yetki Şablonu";
        /// <summary>
        /// Bütçe Uygulama Göreceği Aşamalar, yetki değişken tanımı
        /// </summary>
        public static string YT_ButceUygulamaGormeAsama = "Bütçe Uygulama Göreceği Aşamalar";
        /// <summary>
        /// Bütçe Uygulama Yazma, yetki değişken tanımı
        /// </summary>
        public static string YT_ButceUygulamaYazma = "Bütçe Uygulama Kayıt Yetki Şablonu";
        /// <summary>
        /// Bütçe Uygulama Yazma Aşama, yetki değişken tanımı
        /// </summary>
        public static string YT_ButceUygulamaYazmaAsama = "Bütçe Uygulama Kaydedeceği Aşamalar";
        /// <summary>
        /// Bütçe Uygulama Onay, yetki değişken tanımı
        /// </summary>
        public static string YT_ButceUygulamaOnay = "Bütçe Uygulama Onay Yetki Şablonu";
        /// <summary>
        /// Bütçe Uygulama Onay Aşama, yetki değişken tanımı
        /// </summary>
        public static string YT_ButceUygulamaOnayAsama = "Bütçe Uygulama Onaylayacağı Aşamalar";
        /// <summary>
        /// Bütçe İşlem Yetki Şablonu, yetki değişken tanımı
        /// </summary>
        public static string ButceIslemYT = "Bütçe İşlem Yetki Şablonu";
        /// <summary>
        /// Bütçe İşlem Yetkisi, yetki değişken tanımı
        /// </summary>
        public static string ButceIslemYetkiYT = "Bütçe İşlem Yetkisi";

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum ve Aşama bilgilerine göre,
        /// Kullanıcının Görme Yetkisi olup olmadığı bilgisini dönen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer kullanıcının görme yetkisi var ise true yok ise false değerini tutan Bool türünde nesne döndürür</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, int yil, string kurum, int asama)
        {
            if (kullanan == null) return false;

            //melih açtı
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            //else
            //    return kullanan.YetkiTipiDegeriIceriyorMu(ButceIslemYT, kurum, kurum, true) && kullanan.YetkiTipiDegeriIceriyorMu(ButceIslemYetkiYT, "1;2;3;4", false);
            bool kurumHakki = kullanan_YetkiTipiDegeriIceriyorMu(kullanan, YT_ButceUygulamaGorme, kurum);
            if (asama < 0)
                return kurumHakki;
            if (kurumHakki)
                return kullanan_YetkiTipiDegeriIceriyorMu(kullanan, YT_ButceUygulamaGormeAsama, asama.ToString("00"));
            else
                return false;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum ve Aşama bilgilerine göre,
        /// Kullanıcının Yazma Yetkisi olup olmadığı bilgisini dönen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer kullanıcının yazma yetkisi var ise true yok ise false değerini tutan Bool türünde nesne döndürür</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, int yil, string kurum, int asama)
        {
            if (kullanan == null) return false;
            //melih açtı
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            //else
            //    return kullanan.YetkiTipiDegeriIceriyorMu(ButceIslemYT, kurum, kurum, true) && kullanan.YetkiTipiDegeriIceriyorMu(ButceIslemYetkiYT, "2;4", false);
            bool kurumHakki = kullanan_YetkiTipiDegeriIceriyorMu(kullanan, YT_ButceUygulamaYazma, kurum);
            if (asama < 0)
                return kurumHakki;
            if (kurumHakki)
                return kullanan_YetkiTipiDegeriIceriyorMu(kullanan, YT_ButceUygulamaYazmaAsama, asama.ToString("00"));
            else
                return false;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum ve Aşama bilgilerine göre,
        /// Kullanıcının Onaylama Yetkisi olup olmadığı bilgisini dönen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer kullanıcının onaylama yetkisi var ise true yok ise false değerini tutan Bool türünde nesne döndürür</returns>
        public static bool OnaylamaYetkisiVarMi(Kullanici kullanan, int yil, string kurum, int asama)
        {
            if (kullanan == null) return false;
            //melih açtı
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            //else
            //    return kullanan.YetkiTipiDegeriIceriyorMu(ButceIslemYT, kurum, true) && kullanan.YetkiTipiDegeriIceriyorMu(ButceIslemYetkiYT, "3;4", false);
            bool kurumHakki = kullanan_YetkiTipiDegeriIceriyorMu(kullanan, YT_ButceUygulamaOnay, kurum);
            if (asama < 0)
                return kurumHakki;
            if (kurumHakki)
                return kullanan_YetkiTipiDegeriIceriyorMu(kullanan, YT_ButceUygulamaOnayAsama, asama.ToString("00"));
            else
                return false;
        }

        private static bool kullanan_YetkiTipiDegeriIceriyorMu(Kullanici kullanan, string yetkiTipi, string asamaStr)
        {
            string degerler = kullanan.YetkiTipiDegeriBul(yetkiTipi);
            string[] parcalar = degerler.Split(',', ';');
            for (int i = 0; i < parcalar.Length; i++)
            {
                if (asamaStr.StartsWith(parcalar[i]))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Durum Kodu, Kullanıcı Tip kodu bilgilerine göre,
        /// Kullanıcının işlemi görme yetkisi olup olmadığı bilgisini dönen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="durumKod">Durum kodu bilgisini tutan, ENUMFormDurum türünde nesne</param>
        /// <param name="kullaniciTipKod">Kullanıcı tip kodunu tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer kullanıcının işlemi görme yetkisi var ise true yok ise false değerini tutan Bool türünde nesne döndürür</returns>
        public static bool IslemiGormeYetkisiVarMi(Kullanici kullanan, ENUMFormDurum durumKod, int kullaniciTipKod)
        {
            if (kullanan == null) return false;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            if (durumKod == ENUMFormDurum.ONAYLI)
                return true;
            else
                return true;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanici ve Kullanıcı kodu bilgisine göre,
        /// Kullanıcının işleme yazma yetkisi olup olmadığı bilgisini dönen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="kullaniciKodu">Kullanıcı kodu bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer kullanıcının işleme yazma yetkisi var ise true yok ise false değerini tutan Bool türünde nesne döndürür</returns>
        public static bool IslemeYazmaYetkisiVarMi(Kullanici kullanan, string kullaniciKodu)
        {
            if (kullanan == null) return false;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else
                return kullanan.kullaniciKodu == kullaniciKodu;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanici ve Kullanıcı kodu ve BUMKO Onayı bilgisine göre,
        /// Kullanıcının işlemi onaylama yetkisi olup olmadığı bilgisini dönen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="kullaniciKodu">Kullanıcı kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="bumkoOnayiMi">Bümko onayı ile ilgili olarak yetki kontrolünün yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer kullanıcının işlemi onaylama yetkisi var ise true yok ise false değerini tutan Bool türünde nesne döndürür</returns>
        public static bool IslemiOnaylamaYetkisiVarMi(Kullanici kullanan, string kullaniciKodu, bool bumkoOnayiMi)
        {
            if (bumkoOnayiMi) return true;
            if (kullanan == null) return false;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else
                return kullanan.kullaniciKodu == kullaniciKodu;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanici ve Kullanıcı kodu bilgisine göre,
        /// Kullanıcının işlemi onaylama yetkisi olup olmadığı bilgisini dönen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="kullaniciKodu">Kullanıcı kodu bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer kullanıcının işlemi onaylama yetkisi var ise true yok ise false değerini tutan Bool türünde nesne döndürür</returns>
        public static bool IslemiOnaylamaYetkisiVarMi(Kullanici kullanan, string kullaniciKodu)
        {
            if (kullanan == null) return false;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else
                return kullanan.kullaniciKodu == kullaniciKodu;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum ve Aşama bilgilerine göre,
        /// Kullanıcının Ayrıntılı Harcama Programını Görme Yetkisi olup olmadığı bilgisini dönen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer kullanıcının ayrıntılı harcama programını görme yetkisi var ise true yok ise false değerini tutan Bool türünde nesne döndürür</returns>
        public static bool AHPGormeYetkisiVarMi(Kullanici kullanan, int yil, string kurum, int asama)
        {
            return GormeYetkisiVarMi(kullanan, yil, kurum, asama);
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum ve Aşama bilgilerine göre,
        /// Kullanıcının Ayrıntılı Harcama Programı Yazma Yetkisi olup olmadığı bilgisini dönen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer kullanıcının ayrıntılı harcama programı yazma yetkisi var ise true yok ise false değerini tutan Bool türünde nesne döndürür</returns>
        public static bool AHPYazmaYetkisiVarMi(Kullanici kullanan, int yil, string kurum, int asama)
        {
            return YazmaYetkisiVarMi(kullanan, yil, kurum, asama);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, Aşama ve İşlem bilgilerine göre,
        /// Bütçe İşleminin onaylı olup olmadığı bilgisini dönen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim nesnesini tutan, VeriErisim türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer bütçe işlemi onaylı ise true değilse false değerini tutan Bool türünde nesne döndürür</returns>
        public static bool OnayliMi(VeriErisim veriErisim, int yil, string kurum, int asama, string islem)
        {
            bool donusDeger;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BUMServis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(null));
            donusDeger = ButceIslemServisi.ButceIslemiOnayliMi(ve, yil, kurum, islem, asama) == ENUMFormDurumGrup.ONAYLI;
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum ve Aşama bilgilerine göre,
        /// Kullanıcının İcmal Harcama Programını Görme Yetkisi olup olmadığı bilgisini dönen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer kullanıcının icmal harcama programını görme yetkisi var ise true yok ise false değerini tutan Bool türünde nesne döndürür</returns>
        public static bool IHPKurumGormeYetkisiVarMi(Kullanici kullanan, int yil, string kurum, int asama)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else
                return GormeYetkisiVarMi(kullanan, yil, kurum, asama);
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum ve Aşama bilgilerine göre,
        /// Kullanıcının İcmal Harcama Programına Yazma Yetkisi olup olmadığı bilgisini dönen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer kullanıcının icmal harcama programına yazma yetkisi var ise true yok ise false değerini tutan Bool türünde nesne döndürür</returns>
        public static bool IHPYazmaYetkisiVarMi(Kullanici kullanan, int yil, string kurum, int asama)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else
                return YazmaYetkisiVarMi(kullanan, yil, kurum, asama);
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum ve Aşama bilgilerine göre,
        /// Kullanıcının İcmal Harcama Programını Onaylama Yetkisi olup olmadığı bilgisini dönen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer kullanıcının icmal harcama programını onaylama yetkisi var ise true yok ise false değerini tutan Bool türünde nesne döndürür</returns>
        public static bool IHPOnaylamaYetkisiVarMi(Kullanici kullanan, int yil, string kurum, int asama)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else
                return OnaylamaYetkisiVarMi(kullanan, yil, kurum, asama);
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama bilgilerine göre,
        /// Kullanıcının Harcama Kurum Görme yetkisi olup olmadığı bilgisini dönen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer kullanıcının harcama kurum görme yetkisi var ise true yok ise false değerini tutan Bool türünde nesne döndürür</returns>
        public static bool HarcamaKurumGormeYetkisiVarMi(Kullanici kullanan, int yil, string kurum)
        {
            return true;
        }

        internal static bool AHPYazmaYetkisiVarMi(Kullanici kullanan, int yil, Tertip tertip, int asama)
        {
            TNS.BHMv2.IBHMv2Servis bhmServis = TNS.BHMv2.Arac.Tanimla();
            return bhmServis.OGYYetkisiVarMi(kullanan, (asama-1).ToString(), tertip, BHMv2.ENUMOGY.Yazma);
        }

        internal static bool AHPGormeYetkisiVarMi(Kullanici kullanan, int yil, Tertip kistasTertip, int asama)
        {
            TNS.BHMv2.IBHMv2Servis bhmServis = TNS.BHMv2.Arac.Tanimla();
            return bhmServis.OGYYetkisiVarMi(kullanan, (asama - 1).ToString(), kistasTertip, BHMv2.ENUMOGY.Gorme);
        }

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum ve Aşama bilgilerine göre,
        /// Kullanıcının Ayrıntılı Harcama Programı Onaylama Yetkisi olup olmadığı bilgisini dönen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer kullanıcının ayrıntılı harcama programı onaylama yetkisi var ise true yok ise false değerini tutan Bool türünde nesne döndürür</returns>
        public static bool AHPOnaylamaYetkisiVarMi(Kullanici kullanan, int yil, string kurum, int asama)
        {
            TNS.BHMv2.IBHMv2Servis bhmServis = TNS.BHMv2.Arac.Tanimla();
            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            kistasTertip.kodlar[0] = kurum;
            kistasTertip.kodlar[4] = "*";
            bool tut = bhmServis.OGYYetkisiVarMi(kullanan, (asama - 1).ToString(), kistasTertip, BHMv2.ENUMOGY.Onaylama);
            return tut;
        }
    }
}
