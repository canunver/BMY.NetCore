using TNS.KYM;

namespace TNS.KHM.IsKurallari
{
    /// <summary>
    /// Yetki ile ilgili fonksiyonları yöneten sınıf
    /// </summary>
    public class KHMYetkiServisi
    {

        static string KesinHesapYT = "Kesin Hesap Yapacağı Birimler";
        static string KesinHesapYetkiYT = "Kesin Hesap Yetkisi";

        /// <summary>
        /// Arguman olarak verilen kullanıcının görme yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <param name="yil">Yetki kontrolü yapılacak yil bilgisi.</param>
        /// <param name="kurum">Yetki kontrolü yapılacak kurum/birim bilgisi.</param>
        /// <returns>Kullanıcının görme yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool KesinHesapOlusturmaYetkisiVarMi(Kullanici kullanan, int yil, string kurum)
        {
            if (kullanan == null) return false;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else
                return kullanan.YetkiTipiDegeriIceriyorMu(KesinHesapYT, kurum, true) && kullanan.YetkiTipiDegeriIceriyorMu(KesinHesapYetkiYT, "2;4", false);
        }

        /// <summary>
        /// Arguman olarak verilen kullanıcının görme yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <param name="kurum">Yetki kontrolü yapılacak kurum/birim bilgisi.</param>
        /// <returns>Kullanıcının görme yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, string kurum)
        {
            return KHM.Yetki.GormeYetkisiVarMi(kullanan, kurum);
            //if (kullanan == null) return false;
            //if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            //    return true;
            //else
            //    return kullanan.YetkiTipiDegeriIceriyorMu(KesinHesapYT, kurum, true) && kullanan.YetkiTipiDegeriIceriyorMu(KesinHesapYetkiYT, "1;2;3;4", false);
        }

        /// <summary>
        /// Arguman olarak verilen kullanıcının görme yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <param name="kurum">Yetki kontrolü yapılacak kurum/birim bilgisi.</param>
        /// <returns>Kullanıcının görme yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, string kurum, string pebkod)
        {
            return KHM.Yetki.GormeYetkisiVarMi(kullanan, kurum);
            //if (kullanan == null) return false;
            //if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            //    return true;
            //else
            //    return kullanan.YetkiTipiDegeriIceriyorMu(KesinHesapYT, kurum, true) && kullanan.YetkiTipiDegeriIceriyorMu(KesinHesapYetkiYT, "1;2;3;4", false);
        }

        /// <summary>
        /// Arguman olarak verilen kullanıcının yazma yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <param name="kurum">Yetki kontrolü yapılacak kurum/birim bilgisi.</param>
        /// <returns>Kullanıcının görme yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, string kurum)
        {
            if (kullanan == null) return false;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else
                return kullanan.YetkiTipiDegeriIceriyorMu(KesinHesapYT, kurum, true) && kullanan.YetkiTipiDegeriIceriyorMu(KesinHesapYetkiYT, "2;4", false);
        }

        /// <summary>
        /// Arguman olarak verilen kullanıcının yazma yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı bilgisi.</param>
        /// <param name="kurum">Yetki kontrolü yapılacak kurum/birim bilgisi.</param>
        /// <returns>Kullanıcının görme yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, string kurum, string pebkod)
        {
            if (kullanan == null) return false;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else
                return kullanan.YetkiTipiDegeriIceriyorMu(KesinHesapYT, kurum, true) && kullanan.YetkiTipiDegeriIceriyorMu(KesinHesapYetkiYT, "2;4", false);
        }
    }
}
