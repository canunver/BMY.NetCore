using System;
using TNS.KYM;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Modülde işlem yapan kullanıcının işlemlere yetkisinin kontrolünü yapan sınıf 
    /// </summary>
    public class MUHYetkiServis
    {
        /// <summary>
        /// Makbuz işlemi yapmak isteyen kullanıcının bu işleme yetkisinin olup olmadığını kontrol eden metod
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Harcama birim kodu</param>
        /// <returns></returns>
        public static bool MakbuzMuhasebeBirimYetkiVarMi(Kullanici kullanan, int muhasebe, string birim)
        {
            if (kullanan.ad == "aktarim")//Sistem dışı aktarımların yapıldığı kullanıcı
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MUHASEBEYONETICISI))
                return true;

            string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Makbuz Saymanlık-Birim Yetkisi");
            if (string.IsNullOrEmpty(yetkiTipiDegeri))
                return false;

            string[] d = yetkiTipiDegeri.Split(';');
            for (int i = 0; i < d.Length; i++)
            {
                string[] muhasebeBirim = d[i].Split('-');
                if (muhasebeBirim.Length != 2)
                    return false;

                if (OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[0], 0) == muhasebe || muhasebeBirim[0] == "*")
                {
                    if (OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[1], 0) == OrtakFonksiyonlar.ConvertToInt(birim, 0) || muhasebeBirim[1] == "*")
                        return true;
                }
            }
            return false;
        }

        static bool TertipKiyasla(string sablonTertip, string tertip)
        {
            sablonTertip = sablonTertip.Replace(".", "");
            tertip = tertip.Replace(".", "");

            ObjectArray st = new ObjectArray();
            ObjectArray t = new ObjectArray();
            int gSay;
            int stSay = OrtakFonksiyonlar.StringiParcala(sablonTertip, "-", st);
            int tSay = OrtakFonksiyonlar.StringiParcala(tertip, "-", t);
            if (stSay > tSay) gSay = stSay;
            else gSay = tSay;
            string stp, tp;
            for (int i = 0; i < gSay; i++)
            {
                if (i < stSay) stp = (string)st.objeler[i];
                else stp = "";

                if (i < tSay) tp = (string)t.objeler[i];
                else tp = "";

                if (stp == "*") stp = "";
                if (stp.Length != 0)
                {
                    if (tp.Length < stp.Length)
                        return false;
                    if (tp.Substring(0, stp.Length) != stp)
                        return false;
                }
            }
            return true;
        }

        static bool OGYYetkisiVarMi(Kullanici kullanan, int belgeTur, String tertip, int ogy, bool kurumMu)
        {
            int parca;
            if (kullanan == null)
                return false;

            if (kullanan.ad == "aktarim")//Sistem dışı aktarımların yapıldığı kullanıcı
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MUHASEBEYONETICISI))//22.08.2014 Melih ekledi
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI) && ogy != (int)ENUMOGY.MuhasebatGonder)
                return true;

            if (ogy == (int)ENUMOGY.Gorme)
                return true;

            string kurum = "";
            ObjectArray d = new ObjectArray();
            parca = OrtakFonksiyonlar.StringiParcala(tertip, "-", d);

            if (parca > 0)
                kurum = ((string)d.objeler[0]).Replace(".", "");
            else
                kurum = "";

            if (kurumMu)
            {
                if (parca > 0)
                    tertip = OrtakFonksiyonlar.KurumsaldanKurumaNoktali((string)d.objeler[0]).Replace(".", "");
                else
                    tertip = "";
            }

            if (kurum == "" && ogy == (int)ENUMOGY.Onaylama)
                return false;

            if (kurumMu)
            {
                return TNS.MUH.Yetki.YetkisiVarMi(kullanan, ogy, belgeTur);
            }

            string yetkiTipiAdi;

            if (belgeTur == (int)ENUMBelgeTuru.ODEMEEMRI || belgeTur == (int)ENUMBelgeTuru.MIF)
                yetkiTipiAdi = "Ödeme Emri/MİF Hazırlayacağı Birimler";
            else if (belgeTur == (int)ENUMBelgeTuru.ODENEKGONDERME || belgeTur == (int)ENUMBelgeTuru.TENKIS)
                yetkiTipiAdi = "Ödenek Gönderme Hazırlayacağı Birimler";
            else
                yetkiTipiAdi = "";

            if (yetkiTipiAdi != "")
            {
                string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);
                d.objeler.Clear();
                parca = OrtakFonksiyonlar.StringiParcala(yetkiTipiDegeri, ";", d);

                foreach (string yetkiTipiDegeriParca in d.objeler)
                {
                    if (!string.IsNullOrWhiteSpace(yetkiTipiDegeriParca))
                        if (TertipKiyasla(yetkiTipiDegeriParca, tertip))
                            return TNS.MUH.Yetki.YetkisiVarMi(kullanan, ogy, belgeTur);
                }
                return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Kullanıcının, işlem yapmak istediği ekonomik koda yetkisinin olup olmadığını kontrol eden metod
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belgeTur">Belge türü</param>
        /// <param name="ekoKod">Ekonomik kod</param>
        /// <returns></returns>
        public static bool EkonomikYetkiVarMi(Kullanici kullanan, int belgeTur, string ekoKod)
        {
            if (string.IsNullOrWhiteSpace(ekoKod)) return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            string yetkiTipiAdi = "";

            if (belgeTur == (int)ENUMBelgeTuru.ODEMEEMRI || belgeTur == (int)ENUMBelgeTuru.MIF)
                yetkiTipiAdi = "Ödeme Emri/MİF Ekonomik Kod";
            else if (belgeTur == (int)ENUMBelgeTuru.ODENEKGONDERME || belgeTur == (int)ENUMBelgeTuru.TENKIS)
                yetkiTipiAdi = "Ödenek Gönderme Ekonomik Kod";

            string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

            if (string.IsNullOrWhiteSpace(yetkiTipiDegeri) || yetkiTipiDegeri == "*")
                return true;

            foreach (string yetkiTipiDegeriParca in yetkiTipiDegeri.Split(';', ','))
            {
                if (yetkiTipiDegeriParca == "*") return true;
                if (!string.IsNullOrWhiteSpace(yetkiTipiDegeriParca))
                    if (ekoKod.Replace(".", "").StartsWith(yetkiTipiDegeriParca.Replace(".", "")))
                        return true;
            }
            return false;
        }

        internal static bool AltFaaliyetYetkiVarMi(Kullanici kullanan, int belgeTur, string pebKod)
        {
            //if (string.IsNullOrWhiteSpace(pebKod)) return true;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            string yetkiTipiAdi = "";

            if (belgeTur == (int)ENUMBelgeTuru.ODEMEEMRI || belgeTur == (int)ENUMBelgeTuru.MIF)
                yetkiTipiAdi = "Ödeme Emri/MİF Faaliyet Kod";
            else if (belgeTur == (int)ENUMBelgeTuru.ODENEKGONDERME || belgeTur == (int)ENUMBelgeTuru.TENKIS)
                yetkiTipiAdi = "Ödenek Gönderme Faaliyet Kod";

            string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);
            if (string.IsNullOrWhiteSpace(yetkiTipiDegeri) || yetkiTipiDegeri == "*")
                return true;

            foreach (string yetkiTipiDegeriParca in yetkiTipiDegeri.Split(';', ','))
            {
                if (yetkiTipiDegeriParca == "*") return true;
                if (!string.IsNullOrWhiteSpace(yetkiTipiDegeriParca))
                    if (pebKod.Replace(".", "").StartsWith(yetkiTipiDegeriParca.Replace(".", "")))
                        return true;
            }
            return false;
        }

        //static bool OnaylamaYetkisiVarMi(Kullanici kullanan, int belgeTur, string kurum)
        //{
        //    return OGYYetkisiVarMi(kullanan, belgeTur, kurum, (int)ENUMOGY.Onaylama, false);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belgeTur">Belge türü</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Harcama birim kodu</param>
        /// <returns></returns>
        public static bool OnaylamaYetkisiVarMiOdemeMIF(Kullanici kullanan, int belgeTur, string muhasebe, string birim, int formDurum)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
            {
                if (OrtakFonksiyonlar.ConvertToInt(kullanan.calistigiKurum, 0) == OrtakFonksiyonlar.ConvertToInt(muhasebe, 0))
                    return true;
                else
                    return false;
            }

            bool yetki = TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, muhasebe, birim);
            if (yetki)
            {
                string yetkiTipiAdi = "Ödeme Emri/MİF Yetkisi";
                string yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);
                yetkiTipiYetkiDegeri = yetkiTipiYetkiDegeri.Replace(";", "");
                int yetkiTipiYetkiDegeriInt = OrtakFonksiyonlar.ConvertToInt(yetkiTipiYetkiDegeri, -1);
                if (yetkiTipiYetkiDegeriInt > 2)//Onaylama
                {
                    if (formDurum == (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER ||
                        formDurum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER ||
                        formDurum == (int)ENUMBelgeDurumu.CEKNOVER)
                    {
                        string yetkisiz = kullanan.YetkiTipiDegeriBul(90);//Yevmiye Veremez yetki tip
                        return string.IsNullOrEmpty(yetkisiz);
                    }
                    else if (formDurum == (int)ENUMBelgeDurumu.YEVMIYEKALDIR)
                    {
                        return YevmiyeKaldirYetkisiVarMi(kullanan, belgeTur, muhasebe + "-" + birim);
                    }
                    else
                        return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Ödenek gönderme belgesini muhasebata gönderme yetkisini kontrol eder
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belgeTur">Belge türü</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <returns></returns>
        public static bool MuhasebatOnaylamaYetkisiVarMi(Kullanici kullanan, int belgeTur, string kurum)
        {
            return OGYYetkisiVarMi(kullanan, belgeTur, kurum, (int)ENUMOGY.MuhasebatGonder, false);
        }

        //static bool OnayKaldirmaYetkisiVarMi(Kullanici kullanan, int belgeTur, string kurum)
        //{
        //    return OGYYetkisiVarMi(kullanan, belgeTur, kurum, (int)ENUMOGY.OnayKaldir, false);
        //}

        static bool YevmiyeKaldirYetkisiVarMi(Kullanici kullanan, int belgeTur, string kurum)
        {
            return TNS.MUH.Yetki.YetkisiVarMi(kullanan, (int)ENUMOGY.YevmiyeKaldir, belgeTur);
        }

        //static bool GormeYetkisiVarMi(Kullanici kullanan, int belgeTur, string tertip)
        //{
        //    return OGYYetkisiVarMi(kullanan, belgeTur, tertip, (int)ENUMOGY.Gorme, false);
        //}

        /// <summary>
        /// Kurum bazında kayıtları görme yetkisini kontrol eder
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belgeTur">Belge türü</param>
        /// <param name="tertip">Tertip bilgisi</param>
        /// <returns></returns>
        public static bool GormeYetkisiVarMiKurum(Kullanici kullanan, int belgeTur, string tertip)
        {
            return OGYYetkisiVarMi(kullanan, belgeTur, tertip, (int)ENUMOGY.Gorme, true);
        }

        /// <summary>
        /// Yazma yetkisini kontrol eder
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belgeTur">Belge türü</param>
        /// <param name="tertip">Tertip bilgisi</param>
        /// <returns></returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, int belgeTur, string tertip)
        {
            return OGYYetkisiVarMi(kullanan, belgeTur, tertip, (int)ENUMOGY.Yazma, false);
        }

        /// <summary>
        /// Kurum bazında yazma yetkisini kontrol eder
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="belgeTur">Belge türü</param>
        /// <param name="tertip">Tertip bilgisi</param>
        /// <returns></returns>
        public static bool YazmaYetkisiVarMiKurum(Kullanici kullanan, int belgeTur, string tertip)
        {
            return OGYYetkisiVarMi(kullanan, belgeTur, tertip, (int)ENUMOGY.Yazma, true);
        }

        static bool HesapPlaniYetki(Kullanici kullanan, int ogy)
        {
            if (kullanan == null)
                return false;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI, (int)ENUMKullaniciTipi.BUTCEYONETICISI, (int)ENUMKullaniciTipi.MUHASEBEYONETICISI))
                return true;

            if (ogy == (int)ENUMOGY.Gorme)
                return true;

            string yetkiTipiAdi;

            yetkiTipiAdi = "Hesap Planı İşlemleri";

            string yetkiTipiYetkiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);
            yetkiTipiYetkiDegeri = yetkiTipiYetkiDegeri.Replace(";", "");
            int yetkiTipiYetkiDegeriInt = OrtakFonksiyonlar.ConvertToInt(yetkiTipiYetkiDegeri, -1);
            if (ogy == (int)ENUMHesapPlaniYetki.Gorme && yetkiTipiYetkiDegeriInt > 0)
                return true;
            else if (ogy == (int)ENUMOGY.Yazma && (yetkiTipiYetkiDegeriInt == 2 || yetkiTipiYetkiDegeriInt == 4))
                return true;
            else if (ogy == (int)ENUMOGY.Onaylama && yetkiTipiYetkiDegeriInt > 2)
                return true;
            else
                return false;
        }

        //static bool HesapPlaniGormeYetkisiVarMi(Kullanici kullanan)
        //{
        //    return HesapPlaniYetki(kullanan, (int)ENUMHesapPlaniYetki.Gorme);
        //}

        /// <summary>
        /// Hesap Planı yazma yetkisini kontrol eder
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns></returns>
        public static bool HesapPlaniYazmaYetkisiVarMi(Kullanici kullanan)
        {
            return HesapPlaniYetki(kullanan, (int)ENUMHesapPlaniYetki.Yazma);
        }

        /// <summary>
        /// Hesap planı silme yetkisini kontrol eder
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns></returns>
        public static bool HesapPlaniSilmeYetkisiVarMi(Kullanici kullanan)
        {
            return HesapPlaniYetki(kullanan, (int)ENUMHesapPlaniYetki.Silme);
        }

        /// <summary>
        /// Ödenek gönderme / Tenkis yetkili olduğu birimlşerin listesini verir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns></returns>
        public static string GondermeBelgesiKurumlariVer(Kullanici kullanan)
        {
            string yetkiTipiAdi = "";

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                return TNS.MUH.Yetki.VarsayilanKurumBul();
            else
            {
                yetkiTipiAdi = "Ödenek Gönderme Hazırlayacağı Birimler";
                return OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);
            }
        }

        /// <summary>
        /// Yetkili olduğu alacak takip birimlerinin listesini verir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns></returns>
        public static string AlacakTakipKurumlariVer(Kullanici kullanan)
        {
            string yetkiTipiAdi = "";

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                return kullanan.calistigiKurum;
            else
            {
                yetkiTipiAdi = "Alacak Takibi Yapacağı Birimler";
                return OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);
            }
        }

        /// <summary>
        /// Yetkili olduğu personel birimlerinin listesini verir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns></returns>
        public static string PersonelKurumlariVer(Kullanici kullanan)
        {
            string yetkiTipiAdi = "";

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                return kullanan.calistigiKurum;
            else
            {
                yetkiTipiAdi = "Personel Saymanlık-Birim Yetkisi";
                return OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);
            }
        }
    }
}
