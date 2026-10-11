using System;
using System.Collections;
using System.Data;
//using MUHServisC.ebutce.Muhasebe;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Yansıtma işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHYansitmaServis
    {
        /// <summary>
        /// Yansıtma bilgileri kaydet
        /// </summary>
        /// <param name="yansitmalar">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc Kaydet(ObjectArray yansitmalar)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            ve.BeginTransaction();

            TABYansitma to = new TABYansitma(ve);
            TABGenel tg = new TABGenel(ve);

            foreach (HesapPlaniYansitma yansitma in yansitmalar.objeler)
            {
                if (yansitma.hesapKod != "")
                    sonuc = to.Kaydet(yansitma);

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH002;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Yansitma listesi aktar
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kaynakYil">Kaynak yil</param>
        /// <param name="hedefYil">Hedef yil</param>
        /// <returns></returns>
        public Sonuc Aktar(Kullanici kullanan, int kaynakYil, int hedefYil)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABYansitma to = new TABYansitma(ve);
            sonuc = to.YansitmaAktar(kaynakYil, hedefYil);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = string.Format(ButceMuhasebeResI.MUH003, hedefYil);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Yansıtma bilgileri sil
        /// </summary>
        /// <param name="yansitmalar">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc Sil(ObjectArray yansitmalar)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            ve.BeginTransaction();
            TABYansitma to = new TABYansitma(ve);

            foreach (HesapPlaniYansitma yansitma in yansitmalar.objeler)
            {
                if (yansitma.hesapKod != "")
                    sonuc = to.Sil(yansitma);

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH004;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Yansitma bilgilerini listele
        /// </summary>
        /// <param name="pYansitma">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public static ObjectArray Listele(VeriErisim veriErisim, HesapPlaniYansitma pYansitma, bool sadeceEsitler)
        {
            ObjectArray iVAN = new ObjectArray();

            Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            TABYansitma to = new TABYansitma(ve);
            IDataReader r = to.Listele(pYansitma, sadeceEsitler);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    HesapPlaniYansitma yansitma = new HesapPlaniYansitma();

                    yansitma.yil = VeriErisim.DegerAl(r, 0, 0);
                    yansitma.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), VeriErisim.DegerAl(r, 1, ""));
                    yansitma.yansitmaHesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), VeriErisim.DegerAl(r, 2, ""));
                    yansitma.satir = VeriErisim.DegerAl(r, 3, 0);

                    iVAN.objeler.Add(yansitma);
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            return iVAN;
        }

        /// <summary>
        /// Ekonomik koddan hesap planı kodlarını getir
        /// </summary>
        /// <param name="ekoKod">eko kod</param>
        /// <returns></returns>
        public HesapPlaniYansitma[] EkonomikKoddanHesapKodlariGetir(string ekoKod)
        {
            ArrayList donusDeger = new ArrayList();
            if (ekoKod == "")
                return (HesapPlaniYansitma[])donusDeger.ToArray(typeof(HesapPlaniYansitma));

            ekoKod = ekoKod.Replace(".", "");
            if (ekoKod.Length != 6)
                return (HesapPlaniYansitma[])donusDeger.ToArray(typeof(HesapPlaniYansitma));

            string yansitmaHesapKod = "830" + ekoKod.Substring(0, 2) + "0" + ekoKod.Substring(2, 1) + "0" + ekoKod.Substring(3, 1) + ekoKod.Substring(4);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            TABYansitma to = new TABYansitma(ve);

            Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);
            HesapPlaniYansitma pYansitma = new HesapPlaniYansitma();
            pYansitma.yil = DateTime.Now.Year;
            pYansitma.yansitmaHesapKod = yansitmaHesapKod;
            pYansitma.hesapKod = "15,25";

            IDataReader r = to.Listele(pYansitma, false);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    HesapPlaniYansitma yansitma = new HesapPlaniYansitma();

                    yansitma.yil = VeriErisim.DegerAl(r, 0, 0);
                    yansitma.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), VeriErisim.DegerAl(r, 1, ""));
                    yansitma.yansitmaHesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), VeriErisim.DegerAl(r, 2, ""));
                    yansitma.satir = VeriErisim.DegerAl(r, 3, 0);

                    donusDeger.Add(yansitma);
                }
                r.Close();
            }


            ve.VeriErisimKapat();

            return (HesapPlaniYansitma[])donusDeger.ToArray(typeof(HesapPlaniYansitma));
        }

        /// <summary>
        /// Yansitma listesini ebutceden aktar
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="hesapKod">Hesap kodu</param>
        /// <param name="yansitmaHesapKod">Yansitma hesap kodu</param>
        /// <returns></returns>
        //public Sonuc EbutcedenAktar(string yil, string kurum, string hesapKod, string yansitmaHesapKod)
        //{
        //    Sonuc sonuc = new Sonuc(false, "", "");

        //    //System.Net.ServicePointManager.CertificatePolicy = new CertificatePolicy();
        //    MUHServisC.ebutce.Muhasebe.Muhasebe mEbutce = new MUHServisC.ebutce.Muhasebe.Muhasebe();
        //    mEbutce.Proxy = OrtakFonksiyonlar.ProxyAyarla();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
        //    TABYansitma to = new TABYansitma(ve);
        //    ve.BeginTransaction();

        //    if (hesapKod == "" && yansitmaHesapKod == "")
        //        sonuc = to.SilHepsini(OrtakFonksiyonlar.ConvertToInt(yil, 0));
        //    else
        //        sonuc.islemSonuc = true;

        //    for (int satir = 1; satir < 20; satir++)
        //    {
        //        ObjectArray yanlar = new ObjectArray();
        //        mEbutce.Timeout = 20 * 60 * 1000;

        //        YansitmalarW mW = mEbutce.YansitmaListele("12", hesapKod, yansitmaHesapKod, satir);//kurum=12 bütün kurumlar için

        //        for (int i = 0; i < mW.yansitma.Length; i++)
        //        {
        //            HesapPlaniYansitma yansitma = new HesapPlaniYansitma();
        //            yansitma.hesapKod = mW.yansitma[i].hesapKod;
        //            yansitma.hKodAciklama = mW.yansitma[i].hKodAciklama;
        //            yansitma.satir = mW.yansitma[i].satir;
        //            yansitma.yansitmaHesapKod = mW.yansitma[i].yansitmaHesapKod;
        //            yansitma.yil = mW.yansitma[i].yil;
        //            yansitma.yKodAciklama = mW.yansitma[i].yKodAciklama;

        //            yanlar.objeler.Add(yansitma);
        //        }

        //        if (sonuc.islemSonuc && yanlar.objeler.Count > 0)
        //            sonuc = Kaydet(yanlar);

        //        if (!sonuc.islemSonuc || yanlar.objeler.Count == 0)
        //            break;
        //    }

        //    if (sonuc.islemSonuc)
        //    {
        //        sonuc.bilgiStr = ButceMuhasebeResI.MUH005;
        //        ve.Commit();
        //    }
        //    else
        //    {
        //        ve.Rollback();
        //    }

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        /// <summary>
        /// Yansitma bilgilerini listele
        /// </summary>
        /// <param name="pYansitma">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public static ObjectArray ListeleEkoKod(VeriErisim veriErisim, HesapPlaniYansitma pYansitma)
        {
            ObjectArray iVAN = new ObjectArray();

            Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            TABYansitma to = new TABYansitma(ve);
            IDataReader r = to.ListeleTasinirEkoKod(pYansitma);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    HesapPlaniYansitma yansitma = new HesapPlaniYansitma();

                    yansitma.yil = VeriErisim.DegerAl(r, 0, 0);
                    yansitma.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), VeriErisim.DegerAl(r, 1, ""));
                    yansitma.hKodAciklama = VeriErisim.DegerAl(r, 2, "");
                    iVAN.objeler.Add(yansitma);
                }
                r.Close();
            }


            ve.VeriErisimKapat();

            return iVAN;
        }
    }
}
