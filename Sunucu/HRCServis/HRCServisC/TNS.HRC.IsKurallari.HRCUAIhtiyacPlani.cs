using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.BKD;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Para birimi bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCUAIhtiyacPlani
    {

        public Sonuc Kaydet(Kullanici kullanan, ObjectArray hesapPlani)
        {
            Sonuc sonuc = new Sonuc(false, "Kayıt başarısız", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABUAIhtiyacPlani to = new TABUAIhtiyacPlani(ve);
            sonuc = to.HesapKodKaydet(hesapPlani);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kayıt başarıyla yapılmıştır.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }
        public Sonuc Kaydet(Kullanici kullanan, IhtiyacPlaniSatir hesapPlani)
        {
            Sonuc sonuc = new Sonuc(false, "Kayıt başarısız", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABUAIhtiyacPlani to = new TABUAIhtiyacPlani(ve);
            sonuc = to.HesapKodKaydet(hesapPlani);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kayıt başarıyla yapılmıştır.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc Sil(Kullanici kullanan, ObjectArray hesapPlani)
        {
            Sonuc sonuc = new Sonuc(false, "Silme işlemi başarısız", "");

            //sonuc = KontrolEt(kullanan, hesapPlani, "Silme");

            //if (sonuc.islemSonuc)
            //{
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABUAIhtiyacPlani to = new TABUAIhtiyacPlani(ve);
            sonuc = to.HesapKodSil(hesapPlani);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla tamamlandı.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            //}

            return sonuc;
        }
        public Sonuc Sil(Kullanici kullanan, IhtiyacPlaniSatir hesapPlani)
        {
            Sonuc sonuc = new Sonuc(false, "Silme işlemi başarısız", "");

            //sonuc = KontrolEt(kullanan, hesapPlani, "Silme");

            //if (sonuc.islemSonuc)
            //{
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABUAIhtiyacPlani to = new TABUAIhtiyacPlani(ve);
            sonuc = to.HesapKodSil(hesapPlani);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla tamamlandı.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            //}

            return sonuc;
        }


        public ObjectArray Listele(Kullanici kullanan, IhtiyacPlaniSatir hesKod)
        {
            ObjectArray hVAN = new ObjectArray();
            //string hata = "";

            //if (hesKod.yil <= 0)
            //    hata += ButceMuhasebeResI.MUH070;

            //if (hata != "")
            //{
            //    hVAN.sonuc.hataStr = hata;
            //    hVAN.sonuc.islemSonuc = false;
            //    return hVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABUAIhtiyacPlani to = new TABUAIhtiyacPlani(ve);
            IDataReader r = to.HesapKodListele(hesKod);

            if (to.Hata() == "")
            {
                Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);

                while (r.Read())
                {
                    IhtiyacPlaniSatir hKod = new IhtiyacPlaniSatir();

                    hKod.kod = VeriErisim.DegerAl(r, 0, "");
                    hKod.aciklama = VeriErisim.DegerAl(r, 1, "");
                    hKod.durum = VeriErisim.DegerAl(r, 2, 0);

                    hVAN.objeler.Add(hKod);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

        Sonuc KontrolEt(Kullanici kullanan, ObjectArray hesapPlani, string yetkiTipi)
        {
            Sonuc sonuc = new Sonuc(false, "hata", "");

            sonuc.islemSonuc = true;

            //Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);

            //bool yetki = false;

            //VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            //if (yetkiTipi == "Yazma")
            //    yetki = MUH.IsKurallari.MUHYetkiServis.HesapPlaniYazmaYetkisiVarMi(kullanan);
            //else if (yetkiTipi == "Silme")
            //    yetki = MUH.IsKurallari.MUHYetkiServis.HesapPlaniSilmeYetkisiVarMi(kullanan);

            //if (hesapPlani != null && yetkiTipi == "Yazma")
            //{
            //    foreach (IhtiyacPlaniSatir hs in hesapPlani.objeler)
            //    {
            //        if (hs.yil <= 0)
            //            hata += ButceMuhasebeResI.MUH072;
            //        if (hs.hesapKod == "")
            //            hata += ButceMuhasebeResI.MUH073 + "<br>";
            //        if (hs.aciklama == "")
            //            hata += hs.hesapKod + " - " + ButceMuhasebeResI.MUH074 + "<br>";

            //        //Excel den liste halinde kayıt yapıldığında anasıcocugundan dolayı kayıt yapılamıyor
            //        //Kayıt esnasında bakmak lazım

            //        if (hs.hesapKod.Length > 2)
            //        {
            //            if (!OrtakFonksiyonlar.KodYapiKontrol(t.Kirilimlar(0), hs.hesapKod.Replace(".", ""), false).islemSonuc)
            //                hata += string.Format(ButceMuhasebeResI.MUH075, hs.hesapKod, t.Kirilimlar(0)) + "<br>";
            //        }
            //    }
            //}
            //else if (hesapPlani != null && yetkiTipi == "Silme")
            //{
            //    TABUAIhtiyacPlani th = new TABUAIhtiyacPlani(ve);
            //    foreach (IhtiyacPlaniSatir hs in hesapPlani.objeler)
            //    {
            //        if (th.HesapKodAnasiCocuguVarMi(hs.yil, hs.hesapKod, true))
            //            hata += "<br>" + string.Format(ButceMuhasebeResI.MUH076, hs.hesapKod);
            //        else
            //        {
            //            MUHOdemeEmriMIFServis ode = new MUHOdemeEmriMIFServis();
            //            OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            //            oEmri.yil = hs.yil;
            //            OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(hs.yil);
            //            oEmriDetay.hesapKod = hs.hesapKod;
            //            oEmriDetay.kur = "";
            //            oEmriDetay.fon = "";
            //            oEmriDetay.fin = "";
            //            oEmriDetay.eko = "";
            //            oEmri.detay.objeler.Add(oEmriDetay);

            //            ObjectArray olar = ode.CokBelgeGetir(kullanan, oEmri, false, false);
            //            if (olar.objeler.Count > 0)
            //            {
            //                hata += "<br>" + string.Format(ButceMuhasebeResI.MUH077, hs.hesapKod);
            //                break;
            //            }
            //        }
            //    }
            //}

            //if (hata == "")
            //    sonuc.islemSonuc = true;

            //sonuc.hataStr = hata;

            //ve.VeriErisimKapat();

            return sonuc;
        }

    }
}
