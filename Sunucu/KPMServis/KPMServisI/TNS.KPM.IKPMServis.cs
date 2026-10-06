using System;
using TNS.KYM;

namespace TNS.KPM
{
    [Serializable()]
    public static class Arac
    {
        /// <summary>
        /// Verilen TC kimlik numarasını yapısal olarak inceleyen yordam. TC Kimlik numarası içinde tutulan son iki karakter, TC kimlik numarasının diğer 9 karekterinden elde edilecek şekilde yapılandırılmıştır. 9 karakterden elde edilen iki karakterle verilen 11 haneli kodun son iki karakeri uyumlu ise doğru döndürülür.
        /// </summary>
        /// <param name="tcKimlikNo">İncelenecek TC kimlik numarası.</param>
        /// <returns>TC kimlik numarasını yapısal olarak doğru ise Doğru, yanlış ise Yanlış döndürülür.</returns>
        public static string TCNoDogruMu(string tcKimlikNo)
        {
            if (!string.IsNullOrWhiteSpace(tcKimlikNo) && tcKimlikNo.Length == 11)
            {
                int[] tckimlikdize = new int[11];

                for (int i = 0; i < 11; i++)
                    tckimlikdize[i] = OrtakFonksiyonlar.ConvertToInt(tcKimlikNo.Substring(i, 1), -1);

                int tekler = tckimlikdize[0] + tckimlikdize[2] + tckimlikdize[4] + tckimlikdize[6] + tckimlikdize[8];
                int ciftler = tckimlikdize[1] + tckimlikdize[3] + tckimlikdize[5] + tckimlikdize[7];

                int t1 = (tekler * 3) + ciftler;
                int c1 = (10 - (t1 % 10)) % 10;
                int t2 = c1 + ciftler;
                int t3 = (t2 * 3) + tekler;
                int c2 = (10 - (t3 % 10)) % 10;

                if ((c1.ToString() == tckimlikdize[9].ToString()) && (c2.ToString() == tckimlikdize[10].ToString()))
                    return "";
                else
                    return string.Format(KullaniciResI.KYMKUL033, tcKimlikNo);
            }
            else
                return KullaniciResI.KYMKUL034;
        }

        public static void TarihYaz(Mernis.KPSv2.Types.BilesikKutukSorgulaKimlikNoServis.TarihBilgisi dogumTarih, TNSDateTime dogumTarihi)
        {
            if (dogumTarih != null && dogumTarih.Yil.HasValue && dogumTarih.Ay.HasValue && dogumTarih.Gun.HasValue)
                dogumTarihi.Yaz(new DateTime(dogumTarih.Yil.Value, dogumTarih.Ay.Value, dogumTarih.Gun.Value));
        }


        public static IKPMServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("KPMServisURL");
            IKPMServis servis = TNS.Api.ServisIstemci.Olustur<IKPMServis>(sUrl);
            return servis;
        }
    }

    [Serializable()]
    public static class Yetki
    {
        public static bool SayfayaGirisYapabilirMi(TNS.KYM.Kullanici kullanan)
        {
            return OkumaYetkisiVarMi(kullanan) || YoneticiMi(kullanan);
        }

        public static bool YoneticiMi(TNS.KYM.Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        public static bool TeftisPersoneliMi(TNS.KYM.Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI) ||
                   kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI);//Tarım kullanıcı yaratıyor
        }

        private static bool OkumaYetkisiVarMi(TNS.KYM.Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KIMLIKPAYLASIMI) ||
                   TeftisPersoneliMi(kullanan);

            //kullanan.YetkiTipiDegeriIceriyorMu("Kimlik Paylaşım Yetkisi", "1", false) ||
            //kullanan.YetkiTipiDegeriIceriyorMu("Kimlik Paylaşım Yetkisi", "2", false) ||
            //kullanan.YetkiTipiDegeriIceriyorMu("Kimlik Paylaşım Yetkisi", "3", false) ||
            //kullanan.YetkiTipiDegeriIceriyorMu("Kimlik Paylaşım Yetkisi", "4", false);
        }

        private static bool YazmaYetkisiVarMi(TNS.KYM.Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KIMLIKPAYLASIMI) ||
                   TeftisPersoneliMi(kullanan);

            //kullanan.YetkiTipiDegeriIceriyorMu("Kimlik Paylaşım Yetkisi", "2", false) ||
            //kullanan.YetkiTipiDegeriIceriyorMu("Kimlik Paylaşım Yetkisi", "4", false);
        }

        public static bool OkumaHakkiVarMi(TNS.KYM.Kullanici kullanan)
        {
            if (YoneticiMi(kullanan) || OkumaYetkisiVarMi(kullanan))
                return true;
            else
                return false;
        }

        public static bool YazmaHakkiVarMi(TNS.KYM.Kullanici kullanan)
        {
            if (YoneticiMi(kullanan) || YazmaYetkisiVarMi(kullanan))
                return true;
            else
                return false;
        }
    }

    public interface IKPMServis
    {
        ObjectArray IlListele(TNS.KYM.Kullanici kullanan);

        ObjectArray IlceListele(TNS.KYM.Kullanici kullanan);
        ObjectArray IlceListele(TNS.KYM.Kullanici kullanan, int ilKod);

        ObjectArray KoyListele(TNS.KYM.Kullanici kullanan, int ilKod, int ilceKod);
        ObjectArray BucakListele(TNS.KYM.Kullanici kullanan, int ilKod, int ilceKod);

        //ObjectArray KimlikBilgisindenKisiBilgisiSorgula(TNS.KYM.Kullanici kullanan, Kisi kisi);
        //ObjectArray KayitYerindenKisiBilgisiSorgula(TNS.KYM.Kullanici kullanan, Kisi kisi);

        ObjectArray IstekListele(TNS.KYM.Kullanici kullanan, Istek istek);

        ObjectArray NufusKayitOrnekRaporla(TNS.KYM.Kullanici kullanan, Istek istek);
        ObjectArray IkametgahRaporla(TNS.KYM.Kullanici kullanan, Istek istek);
        ObjectArray VukuatliNufusKayitOrnekRaporla(TNS.KYM.Kullanici kullanan, Istek istek);
    }
}