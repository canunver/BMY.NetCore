using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.KYM;
using TNS.EVR;

namespace TNS
{
    public class EVRYetkiTipiListe : YetkiTipiListesi, YetkiTipiAgaci
    {
        private ListeSatir[] IslemListesi(Kullanici kullanan, int yil, bool p)
        {
            ListeSatir[] lv = new ListeSatir[34];
            lv[0] = new ListeSatir(StrYap(ENUMYetkiler.GELENEVRAK), "Gelen Evrak");
            lv[1] = new ListeSatir(StrYap(ENUMYetkiler.GIDENEVRAK), "Giden Evrak");
            lv[2] = new ListeSatir(StrYap(ENUMYetkiler.EVRAKISLEMLERI), "Evrak İşlemleri");
            lv[3] = new ListeSatir(StrYap(ENUMYetkiler.ARAMA), "Evrak Arama");
            lv[4] = new ListeSatir(StrYap(ENUMYetkiler.DURUMSORGULAMA), "Durum Sorgulama");
            lv[5] = new ListeSatir(StrYap(ENUMYetkiler.TASIMA), "Evrak Taşıma");
            lv[6] = new ListeSatir(StrYap(ENUMYetkiler.POSTAISLEMLERI), "Posta İşlemleri");
            lv[7] = new ListeSatir(StrYap(ENUMYetkiler.EPOSTAISLEMLERI), "E-Posta İşlemleri");
            lv[8] = new ListeSatir(StrYap(ENUMYetkiler.FAKSISLEMLERI), "Faks İşlemleri");
            lv[9] = new ListeSatir(StrYap(ENUMYetkiler.HAVALEGRUP), "Havale Grup Tanımlama");
            lv[10] = new ListeSatir(StrYap(ENUMYetkiler.HAVALEKURAL), "Havale Kural Tanımlama");
            lv[11] = new ListeSatir(StrYap(ENUMYetkiler.DAGITIM), "Dağıtım Tanımlama");
            lv[12] = new ListeSatir(StrYap(ENUMYetkiler.GELENKONUTANIMLAMA), "Gelen Konu Tanımlama");
            lv[13] = new ListeSatir(StrYap(ENUMYetkiler.GELENYERTANIMLAMA), "Gelen Yer Tanımlama");
            lv[14] = new ListeSatir(StrYap(ENUMYetkiler.ANTET), "Antet Tanımlama");
            lv[15] = new ListeSatir(StrYap(ENUMYetkiler.BIRIM), "Birim Tanımlama");
            lv[16] = new ListeSatir(StrYap(ENUMYetkiler.DOSYATASNIFPLANI), "Dosya Tasnif Planı");
            lv[17] = new ListeSatir(StrYap(ENUMYetkiler.DOSYATASNIFPLANIGRUP), "Dosya Tasnif Planı (Grup)");
            lv[18] = new ListeSatir(StrYap(ENUMYetkiler.DOSYATASNIFPLANIBIRIM), "Dosya Tasnif Planı (Birim)");
            lv[19] = new ListeSatir(StrYap(ENUMYetkiler.GIZLILIK), "Gizlilik");
            lv[20] = new ListeSatir(StrYap(ENUMYetkiler.YAZIDURUMU), "Yazı Durumu");
            lv[21] = new ListeSatir(StrYap(ENUMYetkiler.GELISSEKLI), "Geliş Şekli");
            lv[22] = new ListeSatir(StrYap(ENUMYetkiler.GIDENTUR), "Giden Yazı Türü");
            lv[23] = new ListeSatir(StrYap(ENUMYetkiler.POSTATUR), "Posta Türü");
            lv[24] = new ListeSatir(StrYap(ENUMYetkiler.EVRAKCINSI), "Evrak Cinsi");
            lv[25] = new ListeSatir(StrYap(ENUMYetkiler.IMZAYOLU), "İmza Yolu Tanımlama");
            lv[26] = new ListeSatir(StrYap(ENUMYetkiler.AVSIVKLASORLERI), "Arşiv Klasörleri");
            lv[27] = new ListeSatir(StrYap(ENUMYetkiler.METAGRUP), "Meta Grup Tanımlama");
            lv[28] = new ListeSatir(StrYap(ENUMYetkiler.META), "Meta Tanımlama");
            lv[29] = new ListeSatir(StrYap(ENUMYetkiler.VEKALET), "Vekalet Tanımlama");
            lv[30] = new ListeSatir(StrYap(ENUMYetkiler.DINAMIKFORM), "Dinamik Form");
            lv[31] = new ListeSatir(StrYap(ENUMYetkiler.ETIKETKISISEL), "Etiket (Kişisel)");
            lv[32] = new ListeSatir(StrYap(ENUMYetkiler.ETIKETKURUMSAL), "Etiket (Kurumsal)");

            lv[33] = new ListeSatir(StrYap(ENUMYetkiler.RAPORLAR), "Raporlar");

            return lv;
        }

        private string StrYap(ENUMYetkiler yetkiIslem)
        {
            int a = (int)yetkiIslem;
            return a.ToString("00");
        }

        public ListeSatir[] YetkiTipiListeElemanlari(Kullanici kullanan, int yil, string ekParamCagri, string ekParamTanim)
        {
            if (ekParamTanim.Equals("1"))
                return IslemListesi(kullanan, yil, false);
            //else if (ekParamTanim.Equals("2"))
            //    return KullaniciTipiListesi(kullanan, yil, false);
            else
                return IslemListesi(kullanan, yil, false);
        }

        //private ListeSatir[] KullaniciTipiListesi(Kullanici kullanan, int yil, bool p)
        //{
        //    List<ListeSatir> l = new List<ListeSatir>();
        //    ObjectArray oa = TNS.EVR.IsKurallari.IKYPersonelTipForm.Listele(null, kullanan, new PersonelTipi());
        //    foreach (PersonelTipi item in oa.objeler)
        //    {
        //        if (item.kod < 10000)
        //            l.Add(new ListeSatir(item.kod.ToString(), item.ad));
        //    }
        //    return l.ToArray();
        //}

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
