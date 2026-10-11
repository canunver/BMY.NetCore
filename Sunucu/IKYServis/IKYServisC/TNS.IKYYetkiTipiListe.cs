using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.KYM;
using TNS.IKY;

namespace TNS
{
    public class IKYYetkiTipiListe : YetkiTipiListesi, YetkiTipiAgaci
    {
        private ListeSatir[] IslemListesi(Kullanici kullanan, int yil, bool p)
        {
            ListeSatir[] lv = new ListeSatir[26];
            lv[0] = new ListeSatir(StrYap(ENUMYetkiIslem.OzlukOzet), "Ozluk Ozet");
            lv[1] = new ListeSatir(StrYap(ENUMYetkiIslem.OzlukDetay), "Ozluk Detay");
            lv[2] = new ListeSatir(StrYap(ENUMYetkiIslem.Nufus), "Nufus");
            lv[3] = new ListeSatir(StrYap(ENUMYetkiIslem.Iletisim), "Iletisim");
            lv[4] = new ListeSatir(StrYap(ENUMYetkiIslem.Askerlik), "Askerlik");
            lv[5] = new ListeSatir(StrYap(ENUMYetkiIslem.Aile), "Aile");
            lv[6] = new ListeSatir(StrYap(ENUMYetkiIslem.Ogrenim), "Oğrenim");
            lv[7] = new ListeSatir(StrYap(ENUMYetkiIslem.YabanciDil), "Yabanci Dil");
            lv[8] = new ListeSatir(StrYap(ENUMYetkiIslem.HizmetIciEgitim), "Hizmet Ici Eğitim");
            lv[9] = new ListeSatir(StrYap(ENUMYetkiIslem.Izin), "Izin");
            lv[10] = new ListeSatir(StrYap(ENUMYetkiIslem.Hizmetler), "Hizmetler");
            lv[11] = new ListeSatir(StrYap(ENUMYetkiIslem.CezaOdul), "Ceza/Odul");
            lv[12] = new ListeSatir(StrYap(ENUMYetkiIslem.MaasParametreleri), "Maas Parametreleri");
            lv[13] = new ListeSatir(StrYap(ENUMYetkiIslem.Kartlar), "Kartlar");
            lv[14] = new ListeSatir(StrYap(ENUMYetkiIslem.IzinDevir), "İzin Devir");
            lv[15] = new ListeSatir(StrYap(ENUMYetkiIslem.MalBildirimi), "Mal Bildirimi");
            lv[16] = new ListeSatir(StrYap(ENUMYetkiIslem.KadroKontenjan), "Kadro Kontenjan");
            lv[17] = new ListeSatir(StrYap(ENUMYetkiIslem.Lojman), "Lojman");
            lv[18] = new ListeSatir(StrYap(ENUMYetkiIslem.Nitelik), "Nitelik");
            lv[19] = new ListeSatir(StrYap(ENUMYetkiIslem.Tarihce), "Tarihce");
            lv[20] = new ListeSatir(StrYap(ENUMYetkiIslem.KullaniciIslemleri), "KullaniciIslemleri");
            lv[21] = new ListeSatir(StrYap(ENUMYetkiIslem.SiteYonetimi), "SiteYonetimi");
            lv[22] = new ListeSatir(StrYap(ENUMYetkiIslem.LojmanTanim), "LojmanTanim");
            lv[23] = new ListeSatir(StrYap(ENUMYetkiIslem.PersonelIslemleri), "PersonelIslemleri");
            lv[24] = new ListeSatir(StrYap(ENUMYetkiIslem.HizliBilgi), "HizliBilgi");
            lv[25] = new ListeSatir(StrYap(ENUMYetkiIslem.Gider), "Yemek");

            return lv;
        }

        private string StrYap(ENUMYetkiIslem yetkiIslem)
        {
            int a = (int)yetkiIslem;
            return a.ToString("00");
        }

        public ListeSatir[] YetkiTipiListeElemanlari(Kullanici kullanan, int yil, string ekParamCagri, string ekParamTanim)
        {
            if (ekParamTanim.Equals("1"))
                return IslemListesi(kullanan, yil, false);
            else if (ekParamTanim.Equals("2"))
                return KullaniciTipiListesi(kullanan, yil, false);
            else
                return IslemListesi(kullanan, yil, false);
        }

        private ListeSatir[] KullaniciTipiListesi(Kullanici kullanan, int yil, bool p)
        {
            List<ListeSatir> l = new List<ListeSatir>();
            ObjectArray oa = TNS.IKY.IsKurallari.IKYPersonelTipForm.Listele(null, kullanan, new PersonelTipi());
            foreach (PersonelTipi item in oa.objeler)
            {
                if (item.kod < 10000)
                    l.Add(new ListeSatir(item.kod.ToString(), item.ad));
            }
            return l.ToArray();
        }

        public ListeSatir[] YetkiTipiAgacElemanlari(Kullanici kullanan, int yil, string ustKod, string ekParamCagri, string ekParamTanim)
        {
            throw new NotImplementedException();
        }

        public string YetkiTipiKodAd(Kullanici kullanan, int yil, string kod, string ekParamCagri, string ekParamTanim)
        {
            throw new NotImplementedException();
        }
    }
}
