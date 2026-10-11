using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;
using TNS.BKD;
using TNS;
using TNS.MUH;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Yetki kontrolü yapan işlemleri yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCYetkiKontrol
    {
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kullanıcının okuma görme yazma yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı değişkeni</param>
        /// <param name="muhasebe">Yetki kontrolü yapılacak muhasebe kodu bilgisi</param>
        /// <param name="birim">Yetki kontrolü yapılacak birim kodu bilgisi</param>
        /// <param name="ogy">Okuma Görme Yazma yetkisinden hangisinin kontrol edileceğini belirleyen değişken</param>
        /// <returns>Kullanıcının okuma görme yazma yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool OGYYetkisiVarMi(Kullanici kullanan, string muhasebe, string birim, int ogy)
        {
            if (kullanan == null)
                return false;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MUHASEBEYONETICISI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                if (kullanan.calistigiKurum == muhasebe)
                    return true;

            string yetkiTipiAdi = "Ödeme Emri/MİF Saymanlık-Birim Yetkisi";

            string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);
            if (string.IsNullOrWhiteSpace(yetkiTipiDegeri)) return false;
            string[] birimler = yetkiTipiDegeri.Split(';');
            int iMuh = OrtakFonksiyonlar.ConvertToInt(muhasebe, 0);
            bool birimeYetkisiVar = false;
            if (birimler.Length == 0)
                birimeYetkisiVar = false;
            else
            {
                //Birim - > MuhasebeKod1-HarcamaKod1;MuhasebeKod2-HarcamaKod2
                for (int i = 0; i < birimler.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(birimler[i])) continue;
                    string[] mb = birimler[i].Split('-');
                    string yetkiMuh = OrtakFonksiyonlar.DiziElemanAl(mb, 0, "");
                    if (string.IsNullOrWhiteSpace(yetkiMuh)) continue;
                    string yetkiBrm = OrtakFonksiyonlar.DiziElemanAl(mb, 1, "");

                    if (yetkiMuh == "*" || OrtakFonksiyonlar.ConvertToInt(yetkiMuh, -1) == iMuh 
                        || YildizYetkiVarMi(yetkiMuh, iMuh))
                    {
                        if (yetkiBrm.Replace(".", "") == birim.Replace(".", ""))
                        {
                            birimeYetkisiVar = true;
                            break;
                        }
                        else if (yetkiBrm == "*")
                        {
                            birimeYetkisiVar = true;
                            break;
                        }
                    }
                }
            }
            if (!birimeYetkisiVar)
                return TNS.TMM.Yetki.YetkisiVarMi(kullanan, ogy);
            else
                return true;
        }

        private static bool YildizYetkiVarMi(string yetkiMuh, int iMuh)
        {
            if (yetkiMuh.EndsWith("*"))
            {
                int ilKod = OrtakFonksiyonlar.ConvertToInt(yetkiMuh.Substring(0, yetkiMuh.Length - 1), 0);
                if (ilKod == iMuh / 1000) return true;
            }
            return false;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kullanıcının muhasebe ve birimde yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı değişkeni</param>
        /// <param name="muhasebe">Yetki kontrolü yapılacak muhasebe kodu bilgisi</param>
        /// <param name="birim">Yetki kontrolü yapılacak birim kodu bilgisi</param>
        /// <returns>Kullanıcının muhasebe ve birimde yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool MuhasebeBirimYetkiVarMi(Kullanici kullanan, string muhasebe, string birim, ENUMKullaniciYetkiTipi yetkiTipi = ENUMKullaniciYetkiTipi.SURECBASLAT)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MUHASEBEYONETICISI))
                return true;

            string yetkiTipiAdi = ""; 
            string yetkiTipiDegeri = "";

            yetkiTipiAdi = "Ödeme Emri/MİF Saymanlık-Birim Yetkisi";
            yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

            if (yetkiTipiDegeri.Trim() == "")
            {
                if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                {
                    if (OrtakFonksiyonlar.ConvertToInt(kullanan.calistigiKurum, 0) == OrtakFonksiyonlar.ConvertToInt(muhasebe, 0))
                        return true;
                    else
                        return false;
                }
            }

            string[] d = null;

            if (yetkiTipiDegeri.Trim() != "")
            {
                d = yetkiTipiDegeri.Split(';');
                for (int i = 0; i < d.Length; i++)
                {
                    string[] muhasebeBirim = d[i].Split('-');
                    if (muhasebeBirim.Length != 2)
                        return false;

                    if (OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[0].Trim(), 0) == OrtakFonksiyonlar.ConvertToInt(muhasebe, 0) ||
                        muhasebeBirim[0].Trim() == "*")
                    {
                        if (birim.Trim() == "")
                            return true;
                        else if (OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[1].Trim(), 0) == OrtakFonksiyonlar.ConvertToInt(birim, 0) ||
                            muhasebeBirim[1].Trim() == "*")
                            return true;
                    }
                }
            }
            else
            {
                //////
                yetkiTipiAdi = "Taşınır Muhasebe Harcama Birim Yetkisi";
                yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

                d = yetkiTipiDegeri.Split(';');
                for (int i = 0; i < d.Length; i++)
                {
                    string[] muhasebeBirim = d[i].Split('-');
                    if (muhasebeBirim.Length != 3)
                        return false;

                    if (OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[0].Trim(), 0) == OrtakFonksiyonlar.ConvertToInt(muhasebe, 0) ||
                        muhasebeBirim[0].Trim() == "*")
                    {
                        string birimDeger = muhasebeBirim[1].Split('.')[muhasebeBirim[1].Split('.').Length - 1];
                        if (birim.Trim() == "")
                            return true;
                        else if (OrtakFonksiyonlar.ConvertToInt(birimDeger.Trim(), 0) == OrtakFonksiyonlar.ConvertToInt(birim, 0) ||
                            muhasebeBirim[1].Trim() == "*")
                            return true;
                    }
                }
                //////
            }
            return false;
        }

        /// <summary>
        /// Kullanıcının görme yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı değişkeni</param>
        /// <returns>Kullanıcının görme yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, int muhasebe, string birim)
        {
            return OGYYetkisiVarMi(kullanan, StringMuhasebe(muhasebe), birim, (int)ENUMOGY.Gorme);
        }

        /// <summary>
        /// Kullanıcının yazma yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı değişkeni</param>
        /// <returns>Kullanıcının yazma yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, int muhasebe, string birim)
        {
            return OGYYetkisiVarMi(kullanan, StringMuhasebe(muhasebe), birim, (int)ENUMOGY.Yazma);
        }

        /// <summary>
        /// Kullanıcının görme yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı değişkeni</param>
        /// <returns>Kullanıcının görme yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan)
        {
            return true;
        }
        /// <summary>
        /// Kullanıcının yazma yetkisinin olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı değişkeni</param>
        /// <returns>Kullanıcının yazma yetkisinin olup olmadığını bool olarak döner</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            return true;
        }
        internal static Sonuc SurecYetkisiVarMi(VeriErisim veriErisim, Kullanici kullanan, Surec surec, ENUMKullaniciYetkiTipi yetkiTipi)
        {
            Sonuc sonuc = new Sonuc();
            if (surec == null)
            {
                sonuc.DegerAta(TNS.HRC.HarcamaSurecleriResI.HRC00013);
            }
            else
            {
                bool yetkiVarMi = MuhasebeBirimYetkiVarMi(kullanan, StringMuhasebe(surec.muhasebe), surec.birim, yetkiTipi);
                if(!yetkiVarMi)
                    sonuc.DegerAta(TNS.HRC.HarcamaSurecleriResI.HRC00013);
            }
            //    if (yetkiTipi == ENUMYetkiTipi.YAPABILIR)
            //{
            //    if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan, surec.muhasebe, surec.birim))
            //    {
            //        sonuc.DegerAta(TNS.HRC.HarcamaSurecleriResI.HRC00013);
            //    }
            //}
            //else if (yetkiTipi == ENUMYetkiTipi.GOREBILIR)
            //{
            //    if (!HRCYetkiKontrol.GormeYetkisiVarMi(kullanan))
            //    {
            //        sonuc.DegerAta(TNS.HRC.HarcamaSurecleriResI.HRC00013);
            //    }
            //}
            //else if (yetkiTipi == ENUMYetkiTipi.ONAYLAYABILIR)
            //{
            //    sonuc.DegerAta(TNS.HRC.HarcamaSurecleriResI.HRC00013);
            //}

            return sonuc;
        }

        private static string StringMuhasebe(int muhasebe)
        {
            return muhasebe.ToString().PadLeft(5, '0');
        }


        /// <summary>
        /// Kullanıcının yönetici olup olmadığını bool olarak döner
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı değişkeni</param>
        /// <returns>Kullanıcının yönetici olup olmadığını bool olarak döner</returns>
        public static bool YoneticiMi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else
                return false;
        }

        public static bool TeminatMektubuYetkisiVarmi(Kullanici kullanan, int ylsYetki)
        {
            //yetkiTipiDegeri
            //1 görme
            //2 görme ve yazma
            //3 görme yazma silme

            //ylsYetki= 1 yazma , 2 listeleme , 3 silme
            bool yetkiVar = false;
            string yetkiTipiAdi = "Teminat Mektubu Yetkisi";
            string yetkiTipiDegeri = "";

            if (kullanan == null)
                yetkiVar = false;

            yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);
            if (yetkiTipiDegeri.Trim() != "")
            {
                switch (ylsYetki)
                {
                    case 1://yazma
                        if (yetkiTipiDegeri.Contains("2") || yetkiTipiDegeri.Contains("3"))
                            yetkiVar = true;
                        break;

                    case 2: // listeleme 
                        if (yetkiTipiDegeri.Contains("1") || yetkiTipiDegeri.Contains("2") || yetkiTipiDegeri.Contains("3"))
                            yetkiVar = true;

                        break;
                    case 3: //silme
                        if (yetkiTipiDegeri.Contains("3"))
                            yetkiVar = true;
                        break;
                    default:
                        break;
                }
            }
            return yetkiVar;
        }

    }
}
