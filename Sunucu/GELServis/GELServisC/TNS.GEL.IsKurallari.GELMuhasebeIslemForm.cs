using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.GEL.IsKurallari.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari
{
    public class GELMuhasebeIslemForm
    {
        public static string MuhasebeIslemFormKontrol(MuhasebeIslemForm bilgi, ENUMIslemTuru islemturu, int islemkodu = 0, int parcakodu = 0)
        {
            StringBuilder hatalar = new StringBuilder();
            if (islemturu == ENUMIslemTuru.KAYIT || islemturu == ENUMIslemTuru.SILME)
            {
                if (islemkodu <= 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00109 + "<br/>");
                if (parcakodu <= 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00110 + "<br/>");
            }
            if (bilgi.yil <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00022 + "<br/>");
            if (bilgi.muhasebe <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00023 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.birim))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00024 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.surecNo))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00026 + "<br/>");
            if (bilgi.firma.kod <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00057 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.tasinmaz.tasinmazSistemKodu))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00005 + "<br/>");
            if (bilgi.taksitno <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00070 + "<br/>");

            if (islemturu == ENUMIslemTuru.KAYIT)
            {
                //if (bilgi.odemeEmriTarih == null || bilgi.odemeEmriTarih.Oku().ToString() == "01.01.0001 00:00:00")
                //    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00088 + "<br/>");
                //if (string.IsNullOrEmpty(bilgi.odemeEmriNo))
                //    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00089 + "<br/>");
                //if(bilgi.odemeEmriYil <=0)
                //    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00090 + "<br/>");
                if (bilgi.prmakbuzno <= 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00081 + "<br/>");
            }
            return hatalar.ToString();
        }

        public static string MuhasebeIslemFormDetayKontrol(MuhasebeIslemFormDetay bilgi, ENUMIslemTuru islemturu)
        {

            StringBuilder hatalar = new StringBuilder();
            if (bilgi.yil <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00022 + "<br/>");
            if (bilgi.muhasebe <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00023 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.birim))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00024 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.surecNo))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00026 + "<br/>");
            if (bilgi.firma.kod <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00057 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.tasinmaz.tasinmazSistemKodu))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00005 + "<br/>");
            if (bilgi.taksitno <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00070 + "<br/>");
            if (islemturu == ENUMIslemTuru.KAYIT)
            {
                if (string.IsNullOrEmpty(bilgi.hesapPlanKod))
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00091 + "<br/>");
                if (string.IsNullOrEmpty(bilgi.yansitma))
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00092 + "<br/>");
                if (string.IsNullOrEmpty(bilgi.tertip))
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00093 + "<br/>");

            }
            return hatalar.ToString();
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, KYM.Kullanici kullanan, MuhasebeIslemForm bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");

            string hata = MuhasebeIslemFormKontrol(bilgi, ENUMIslemTuru.KAYIT, islemkod, parcakod);
            if (hata.Trim().Length > 0) hatalar.Append(hata);

            if (hatalar.ToString().Length > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }
            //*****Sürece ait parça durum kontrolü
            bool parcatamamlandi = GELSurec.ParcaTamamlandiMi(kullanan, bilgi.yil, bilgi.surecNo, islemkod, parcakod);
            if (parcatamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00107;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABMuhasebeIslemForm tabMuhasebeForm = new TABMuhasebeIslemForm(ve);
            ve.BeginTransaction();
            if (tabMuhasebeForm.BilgiKaydiVarmi(bilgi))
                tabMuhasebeForm.BilgiGuncelle(bilgi);
            else
            {
                TABGenel tg = new TABGenel(ve);
                string sayacKod = bilgi.yil.ToString();
                sayacKod = "GELMUHODEMEEMRINO" + sayacKod;
                bilgi.prOdemeEmriNo = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl(sayacKod, "000000"), 0);
                if (bilgi.prmakbuzno > 0)
                    sonuc = tabMuhasebeForm.BilgiKaydet(bilgi);
                else
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00081;
                    return sonuc;
                }
            }
            tabMuhasebeForm.DetaySil(bilgi.yil, bilgi.muhasebe, bilgi.birim, bilgi.surecNo, bilgi.firma.kod, bilgi.tasinmaz.tasinmazSistemKodu, bilgi.taksitno, 0);
            if (sonuc.islemSonuc && ve.hata.Trim() == "")
            {
                foreach (MuhasebeIslemFormDetay item in bilgi.detay.objeler)
                {
                    if (!sonuc.islemSonuc) break;
                    sonuc = tabMuhasebeForm.DetayKaydet(item);
                }
                if (sonuc.islemSonuc)
                {
                    //***********Süreç Durumu Devam Ediyor Olarak Güncelleniyor****
                    Surec s = new Surec();
                    s.yil = bilgi.yil;
                    s.surecNo = bilgi.surecNo;
                    s.surecDurum = (int)TNS.HRC.ENUMSurecDurum.DEVAMEDIYOR;
                    sonuc = GELSurec.DurumGuncelle(ve, kullanan, s);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00025;
                    }
                    else
                    {
                        ve.Rollback();
                    }
                }
                else
                {
                    ve.Rollback();
                }
            }
            else
            {
                ve.Rollback();
                sonuc.islemSonuc = false;
                sonuc.hataStr = ve.hata;
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, KYM.Kullanici kullanan, MuhasebeIslemForm bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc();
            string hata = MuhasebeIslemFormKontrol(bilgi, ENUMIslemTuru.SILME, islemkod, parcakod);
            if (hata.Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }
            //*****Sürece ait parça durum kontrolü
            bool parcatamamlandi = GELSurec.ParcaTamamlandiMi(kullanan, bilgi.yil, bilgi.surecNo, islemkod, parcakod);
            if (parcatamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00107;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABMuhasebeIslemForm tabMuhasebeForm = new TABMuhasebeIslemForm(ve);
            ve.BeginTransaction();
            tabMuhasebeForm.DetaySil(bilgi.yil, bilgi.muhasebe, bilgi.birim, bilgi.surecNo, bilgi.firma.kod, bilgi.tasinmaz.tasinmazSistemKodu, bilgi.taksitno, 0);
            if (ve.hata.Trim() == "")
            {
                tabMuhasebeForm.BilgiSil(bilgi);
                if (ve.hata.Trim() == "")
                {
                    //***********Süreç Durumu Devam Ediyor Olarak Güncelleniyor****
                    Surec s = new Surec();
                    s.yil = bilgi.yil;
                    s.surecNo = bilgi.surecNo;
                    s.surecDurum = (int)TNS.HRC.ENUMSurecDurum.DEVAMEDIYOR;
                    sonuc = GELSurec.DurumGuncelle(ve, kullanan, s);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00056;
                    }
                    else
                    {
                        ve.Rollback();
                    }
                }
                else
                {
                    ve.Rollback();
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ve.hata;
                }
            }
            else
            {
                ve.Rollback();
                sonuc.islemSonuc = false;
                sonuc.hataStr = ve.hata;
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, KYM.Kullanici kullanan, MuhasebeIslemForm bilgi)
        {
            ObjectArray muhasebeformlar = new ObjectArray();
            string hata = MuhasebeIslemFormKontrol(bilgi, ENUMIslemTuru.LISTELEME);
            if (hata.Trim() != "")
            {
                muhasebeformlar.sonuc.islemSonuc = false;
                muhasebeformlar.sonuc.hataStr = hata;
                return muhasebeformlar;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABMuhasebeIslemForm tabMuhasebeIsleForm = new TABMuhasebeIslemForm(ve);
            ve.BeginTransaction();
            IDataReader dr = tabMuhasebeIsleForm.BilgiListele(bilgi);
            while (dr.Read())
            {
                MuhasebeIslemForm muhform = new MuhasebeIslemForm();
                muhform.yil = VeriErisim.DegerAl(dr, 0, 0);
                muhform.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                muhform.birim = VeriErisim.DegerAl(dr, 2, "");
                muhform.surecNo = VeriErisim.DegerAl(dr, 3, "");
                muhform.firma.kod = VeriErisim.DegerAl(dr, 4, 0);
                muhform.firma.ad = GELServis.servisUZY.UzayDegeriStr(kullanan, "HRCFIRMABILGISI", "-" + muhform.muhasebe + "-" + muhform.firma.kod, false);
                muhform.tasinmaz.tasinmazSistemKodu = VeriErisim.DegerAl(dr, 5, "");
                muhform.taksitno = VeriErisim.DegerAl(dr, 6, 0);
                muhform.prmakbuzno = VeriErisim.DegerAl(dr, 7, 0);
                muhform.prOdemeEmriNo = VeriErisim.DegerAl(dr, 8, 0);
                muhform.odemeEmriNo = VeriErisim.DegerAl(dr, 9, "");
                muhform.odemeEmriTarih = VeriErisim.DegerAl(dr, 10);
                muhform.odemeEmriYil = VeriErisim.DegerAl(dr, 11, 0);
                muhform.aciklama = VeriErisim.DegerAl(dr, 12, "");
                muhasebeformlar.objeler.Add(muhform);
            }
            dr.Close();
            foreach (MuhasebeIslemForm item in muhasebeformlar.objeler)
            {
                dr = tabMuhasebeIsleForm.DetayListele(item.yil, item.muhasebe, item.birim, item.surecNo, item.firma.kod, item.tasinmaz.tasinmazSistemKodu, 0, item.taksitno);
                while (dr.Read())
                {
                    MuhasebeIslemFormDetay mufformdetay = new MuhasebeIslemFormDetay();
                    mufformdetay.yil = VeriErisim.DegerAl(dr, 0, 0);
                    mufformdetay.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                    mufformdetay.birim = VeriErisim.DegerAl(dr, 2, "");
                    mufformdetay.surecNo = VeriErisim.DegerAl(dr, 3, "");
                    mufformdetay.firma.kod = VeriErisim.DegerAl(dr, 4, 0);
                    mufformdetay.tasinmaz.tasinmazSistemKodu = VeriErisim.DegerAl(dr, 5, "");
                    mufformdetay.taksitno = VeriErisim.DegerAl(dr, 6, 0);
                    mufformdetay.sirano = VeriErisim.DegerAl(dr, 7, 0);
                    mufformdetay.hesapPlanKod = VeriErisim.DegerAl(dr, 8, "");
                    mufformdetay.hesapPlanAd = GELServis.UzayYaziDegerGetir("MUHHESAPPLANI", mufformdetay.yil.ToString() + "-" + mufformdetay.hesapPlanKod, true, "ACIKLAMA");
                    mufformdetay.yansitma = VeriErisim.DegerAl(dr, 9, "");
                    mufformdetay.tertip = VeriErisim.DegerAl(dr, 10, "");
                    mufformdetay.borc = VeriErisim.DegerAl(dr, 11, 0.0);
                    mufformdetay.alacak = VeriErisim.DegerAl(dr, 12, 0.0);
                    item.detay.objeler.Add(mufformdetay);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return muhasebeformlar;

        }
    }
}
