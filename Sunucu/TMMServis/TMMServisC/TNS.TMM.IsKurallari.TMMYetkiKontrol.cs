using TNS.KYM;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Taşınır mal modülünün okuma, yazma, onaylama gibi yetki kontrollerini yapan fonksiyonları içeren sınıf
    /// </summary>
    public class TMMYetkiKontrol
    {
        /// <summary>
        /// Parametre olarak gönderilen kullanıcının yönetici olup olmadığının kontrolünü yapan yordam
        /// Kullanıcı yönetici ise true, değilse false döndürür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <returns>Yönetici olup olmadığının sonucu</returns>
        public static bool Yoneticimi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRMALYONETICISI))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının taşınır mal modülüne
        /// yetkisinin olup olmadığının kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <returns>Taşınır mal modülüne yetkili olup olmadığının sonucu</returns>
        public static bool TasinirMalYetkisiVarMi(Kullanici kullanan)
        {
            if (Yoneticimi(kullanan) ||
                (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRMALKAYDI)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının muhasebe-harcama-ambar üçlüsü üzerinde
        /// ogy parametresindeki yetkiye sahip olup olmadığının kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcama">Harcama birimi</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <param name="ogy">ENUMOGY listesindeki değerlerden biri olmalıdır.</param>
        /// <param name="yetkiKontrolu">if set to <c>true</c> [yetki kontrolu].</param>
        /// <returns>
        /// Yetki kontrolünün sonucu döndürülür.
        /// </returns>
        private static bool OGYYetkisiVarMi(Kullanici kullanan, string muhasebe, string harcama, string ambar, int ogy, bool yetkiKontrolu)
        {
            if (kullanan == null)
                return false;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                if (kullanan.calistigiKurum == muhasebe)
                    return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRMALYONETICISI))
                if (kullanan.calistigiKurum == muhasebe)
                    return true;

            string yetkiTipiAdi = "Taşınır Muhasebe Harcama Birim Yetkisi";
            string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

            yetkiTipiAdi = "Taşınır Kullanıcı Birimi Yetkisi";
            yetkiTipiDegeri += ";" + OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

            yetkiTipiDegeri = yetkiTipiDegeri.Replace(".", "");
            harcama = harcama.Replace(".", "");

            string[] birimler = yetkiTipiDegeri.Split(';');

            //Birim - > MuhasebeKod1-HarcamaKod1-AmbarKod1;MuhasebeKod2-HarcamaKod2-AmbarKod2....

            bool birimeYetkisiVar = false;

            if (birimler.Length == 0)
            {
                birimeYetkisiVar = true;
            }
            else
            {
                for (int i = 0; i < birimler.Length; i++)
                {
                    string[] mb = birimler[i].Split('-');

                    if (OrtakFonksiyonlar.ConvertToInt(mb[0], 0) == 0 && mb[0] != "*") continue;

                    if (OrtakFonksiyonlar.ConvertToInt(mb[0], 0) == OrtakFonksiyonlar.ConvertToInt(muhasebe, 0) || mb[0] == "*"
                        //|| (mb[0].Length > 1 && mb[0].Contains("*") && OrtakFonksiyonlar.ConvertToInt(mb[0].Substring(0, mb[0].Length - 1), 0) == OrtakFonksiyonlar.ConvertToInt(muhasebe.Substring(0, mb[0].Length - 1), 0)))
                        || (mb[0].Length > 1 && mb[0].Contains("*") && OrtakFonksiyonlar.ConvertToInt(mb[0].Substring(0, mb[0].Length), 0) == OrtakFonksiyonlar.ConvertToInt(muhasebe.Substring(0, mb[0].Length), 0)))
                    {
                        if (mb[1].Replace(".", "") == harcama.Replace(".", "") || (mb[1] == "*"))
                        {
                            if (mb[2] == ambar || mb[2] == "*" || ambar == string.Empty)
                            {
                                birimeYetkisiVar = true;
                                break;
                            }
                        }
                        //12.01.32.62.932-->12.01.32*
                        else if (harcama.Replace(".", "").StartsWith(mb[1].Replace(".", "").Replace("*", "")) && mb[1].Replace(".", "").Replace("*", "") != "")
                        {
                            if (mb[2] == ambar || mb[2] == "*" || ambar == string.Empty)
                            {
                                birimeYetkisiVar = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (birimeYetkisiVar)
            {
                if (yetkiKontrolu)
                    return TNS.TMM.Yetki.YetkisiVarMi(kullanan, ogy);
                else
                    return birimeYetkisiVar;
            }
            else
                return birimeYetkisiVar;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının muhasebe-harcama ikilisi üzerinde
        /// ogy parametresindeki yetkiye sahip olup olmadığının kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcama">Harcama birimi</param>
        /// <param name="ogy">ENUMOGY listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Yetki kontrolünün sonucu döndürülür.</returns>
        private static bool OGYYetkisiVarMiHarcamaBirimi(Kullanici kullanan, string muhasebe, string harcama, int ogy)
        {
            if (kullanan == null)
                return false;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                if (kullanan.calistigiKurum == muhasebe)
                    return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRMALYONETICISI))
                if (kullanan.calistigiKurum == muhasebe)
                    return true;

            string yetkiTipiAdi;
            yetkiTipiAdi = "Taşınır Muhasebe Harcama Birim Yetkisi";
            harcama.Replace(".", "");

            string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);
            string[] birimler = yetkiTipiDegeri.Split(';');

            //Birim - > MuhasebeKod1-HarcamaKod1;MuhasebeKod2-HarcamaKod2

            bool birimeYetkisiVar = false;

            if (birimler.Length == 0)
                birimeYetkisiVar = true;
            else
            {
                for (int i = 0; i < birimler.Length; i++)
                {
                    string[] mb = birimler[i].Split('-');

                    if (OrtakFonksiyonlar.ConvertToInt(mb[0], 0) == OrtakFonksiyonlar.ConvertToInt(muhasebe, 0) || mb[0] == "*"
                        //|| (mb[0].Length > 1 && mb[0].Contains("*") && OrtakFonksiyonlar.ConvertToInt(mb[0].Substring(0, mb[0].Length - 1), 0) == OrtakFonksiyonlar.ConvertToInt(muhasebe.Substring(0, mb[0].Length - 1), 0)))
                        || (mb[0].Length > 1 && mb[0].Contains("*") && OrtakFonksiyonlar.ConvertToInt(mb[0].Substring(0, mb[0].Length), 0) == OrtakFonksiyonlar.ConvertToInt(muhasebe.Substring(0, mb[0].Length), 0)))
                    {
                        if (mb[1].Replace(".", "") == harcama.Replace(".", ""))
                        {
                            birimeYetkisiVar = true;
                            break;
                        }
                        else if (mb[1] == "*")
                        {
                            birimeYetkisiVar = true;
                            break;
                        }
                    }
                }
            }
            if (birimeYetkisiVar)
                return TNS.TMM.Yetki.YetkisiVarMi(kullanan, ogy);
            else
                return false;
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının muhasebe-harcama-ambar üçlüsü
        /// üzerinde onay kaldırma yetkisine sahip olup olmadığının kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcama">Harcama birimi</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <returns>Yetki kontrolünün sonucu döndürülür.</returns>
        public static bool OnayKaldirmaYetkisiVarMi(Kullanici kullanan, string muhasebe, string harcama, string ambar)
        {
            return OGYYetkisiVarMi(kullanan, muhasebe, harcama, ambar, (int)ENUMOGY.OnayKaldir, true);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının muhasebe-harcama-ambar üçlüsü
        /// üzerinde onaylama yetkisine sahip olup olmadığının kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcama">Harcama birimi</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <returns>Yetki kontrolünün sonucu döndürülür.</returns>
        public static bool OnaylamaYetkisiVarMi(Kullanici kullanan, string muhasebe, string harcama, string ambar)
        {
            return OGYYetkisiVarMi(kullanan, muhasebe, harcama, ambar, (int)ENUMOGY.Onaylama, true);
        }

        /// <summary>
        /// Called when [yetkisi var mi kullanici birimi].
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="harcama">The harcama.</param>
        /// <param name="ambar">The ambar.</param>
        /// <returns></returns>
        public static bool OnaylamaYetkisiVarMiKullaniciBirimi(Kullanici kullanan, string muhasebe, string harcama, string ambar)
        {
            return OGYYetkisiVarMi(kullanan, muhasebe, harcama, ambar, (int)ENUMOGY.Onaylama, false);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının muhasebe-harcama-ambar üçlüsü
        /// üzerinde görme yetkisine sahip olup olmadığının kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcama">Harcama birimi</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <returns>Yetki kontrolünün sonucu döndürülür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, string muhasebe, string harcama, string ambar)
        {
            return OGYYetkisiVarMi(kullanan, muhasebe, harcama, ambar, (int)ENUMOGY.Gorme, true);
        }

        /// <summary>
        /// Gormes the yetkisi var mi kullanici birimi.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="harcama">The harcama.</param>
        /// <param name="ambar">The ambar.</param>
        /// <returns></returns>
        public static bool GormeYetkisiVarMiKullaniciBirimi(Kullanici kullanan, string muhasebe, string harcama, string ambar)
        {
            return OGYYetkisiVarMi(kullanan, muhasebe, harcama, ambar, (int)ENUMOGY.Gorme, false);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının taşınır mal modülü üzerinde
        /// görme yetkisine sahip olup olmadığının kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <returns>Yetki kontrolünün sonucu döndürülür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan)
        {
            return TNS.TMM.Yetki.YetkisiVarMi(kullanan, (int)ENUMOGY.Gorme);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının muhasebe-harcama-ambar üçlüsü
        /// üzerinde yazma yetkisine sahip olup olmadığının kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcama">Harcama birimi</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <returns>Yetki kontrolünün sonucu döndürülür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, string muhasebe, string harcama, string ambar)
        {
            return OGYYetkisiVarMi(kullanan, muhasebe, harcama, ambar, (int)ENUMOGY.Yazma, true);
        }

        /// <summary>
        /// Yazmas the yetkisi var mi kullanici birimi.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="harcama">The harcama.</param>
        /// <param name="ambar">The ambar.</param>
        /// <returns></returns>
        public static bool YazmaYetkisiVarMiKullaniciBirimi(Kullanici kullanan, string muhasebe, string harcama, string ambar)
        {
            return OGYYetkisiVarMi(kullanan, muhasebe, harcama, ambar, (int)ENUMOGY.Yazma, false);
        }

        public static bool YilDevriGirisCikisYetkisiVarmi(Kullanici kullanan)
        {
            if (kullanan == null)
                return false;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIR_MAL_YIL_DEVRI_MUDAHALE_YETKISI))
                return true;
            else
                return false;
        }


        /// <summary>
        /// Parametre olarak gönderilen kullanıcının muhasebe-harcama ikilisi
        /// üzerinde görme yetkisine sahip olup olmadığının kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcama">Harcama birimi</param>
        /// <returns>Yetki kontrolünün sonucu döndürülür.</returns>
        public static bool GormeYetkisiVarMiHarcamaBirimi(Kullanici kullanan, string muhasebe, string harcama)
        {
            return OGYYetkisiVarMiHarcamaBirimi(kullanan, muhasebe, harcama, (int)ENUMOGY.Gorme);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının muhasebe-harcama ikilisi
        /// üzerinde yazma yetkisine sahip olup olmadığının kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcama">Harcama birimi</param>
        /// <returns>Yetki kontrolünün sonucu döndürülür.</returns>
        public static bool YazmaYetkisiVarMiHarcamaBirimi(Kullanici kullanan, string muhasebe, string harcama)
        {
            return OGYYetkisiVarMiHarcamaBirimi(kullanan, muhasebe, harcama, (int)ENUMOGY.Yazma);
        }

        public static string YetkisiOlanBirimlerMB(Kullanici kullanan, string muhasebe)
        {
            if (kullanan == null)
                return "-";

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return "";

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                if (kullanan.calistigiKurum == muhasebe)
                    return "";

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.TASINIRMALYONETICISI))
                if (kullanan.calistigiKurum == muhasebe)
                    return "";

            string yetkiTipiAdi = "Taşınır Muhasebe Harcama Birim Yetkisi";
            string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

            yetkiTipiAdi = "Taşınır Kullanıcı Birimi Yetkisi";
            yetkiTipiDegeri += ";" + OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

            string[] birimler = yetkiTipiDegeri.Split(';');

            //Birim - > MuhasebeKod1-HarcamaKod1-AmbarKod1;MuhasebeKod2-HarcamaKod2-AmbarKod2....

            string yetkiliBirimler = "";
            for (int i = 0; i < birimler.Length; i++)
            {
                string[] mb = birimler[i].Split('-');

                if (mb.Length < 2)
                    continue;

                if (mb[0] == muhasebe && OrtakFonksiyonlar.ConvertToInt(mb[1], 0) > 0 && !yetkiliBirimler.Contains(mb[1]))
                {
                    if (yetkiliBirimler != "")
                        yetkiliBirimler += ",";
                    yetkiliBirimler += mb[1];
                }
            }
            return yetkiliBirimler;
        }


    }
}