using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.GEL.IsKurallari.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari
{
    public class GELMakbuzForm
    {
        protected static string MakbuzFormKontrol(Makbuz bilgi, ENUMIslemTuru islemturu,int islemkodu=0,int parcakodu=0)
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
                if (bilgi.makbuztarihi == null || bilgi.makbuztarihi.Oku().ToString() == "01.01.0001 00:00:00")
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00082 + "<br/>");
                if (string.IsNullOrEmpty(bilgi.makbuzno))
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00081 + "<br/>");
            }
            return hatalar.ToString();
        }

        public static string MakbuzFormDetayKontrol(MakbuzDetay bilgi, ENUMIslemTuru islemturu)
        {
            StringBuilder hatalar = new StringBuilder();
            if (bilgi.yil <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00022 + "<br/>");
            if (bilgi.muhasebe <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00023 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.birim))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00024 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.surecno))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00026 + "<br/>");
            if (bilgi.firma.kod <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00057 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.tasinmaz.tasinmazSistemKodu))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00005 + "<br/>");
            if (bilgi.taksitno <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00070 + "<br/>");
            if (islemturu == ENUMIslemTuru.KAYIT)
            {
                if (bilgi.prmakbuzno<=0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00081 + "<br/>");
                if (bilgi.miktar <= 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00083 + "<br/>");
                if (bilgi.tutar <= 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00084 + "<br/>");
            }
            return hatalar.ToString();

        }

        public static Sonuc Kaydet(VeriErisim veriErisim, KYM.Kullanici kullanan, Makbuz bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");

            string hata = MakbuzFormKontrol(bilgi, ENUMIslemTuru.KAYIT,islemkod,parcakod);
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
            TABMakbuzForm tabMakbuzForm = new TABMakbuzForm(ve);
            ve.BeginTransaction();
            if (tabMakbuzForm.BilgiKaydiVarmi(bilgi))
                tabMakbuzForm.BilgiGuncelle(bilgi);
            else
            {
                TABGenel tg = new TABGenel(ve);
                string sayacKod = bilgi.yil.ToString();
                sayacKod = "GELMAKBUZNO" + sayacKod;
                bilgi.prmakbuzno =OrtakFonksiyonlar.ConvertToInt( tg.BelgeNoAl(sayacKod, "000000"),0);
                if (bilgi.prmakbuzno > 0)
                    sonuc = tabMakbuzForm.BilgiKaydet(bilgi);
                else
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr =  TNS.GEL.GelirSurecleriResI.GEL00081  ;
                    return sonuc;
                }
            }
            tabMakbuzForm.DetaySil(bilgi.yil, bilgi.muhasebe, bilgi.birim, bilgi.surecNo, bilgi.firma.kod,bilgi.tasinmaz.tasinmazSistemKodu,bilgi.taksitno,0);
            if (sonuc.islemSonuc && ve.hata.Trim() == "")
            {
                foreach (MakbuzDetay item in bilgi.detay.objeler)
                {
                    if (!sonuc.islemSonuc) break;
                    item.prmakbuzno = bilgi.prmakbuzno;
                    sonuc = tabMakbuzForm.DetayKaydet(item);
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

      

        public static ObjectArray Listele(VeriErisim veriErisim, KYM.Kullanici kullanan, Makbuz bilgi)
        {
            ObjectArray makbuzlar = new ObjectArray();
         
         
            string hata = MakbuzFormKontrol(bilgi, ENUMIslemTuru.LISTELEME);
            if (hata.Trim() != "")
            {
                makbuzlar.sonuc.islemSonuc = false;
                makbuzlar.sonuc.hataStr = hata;
                return makbuzlar;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABMakbuzForm tabMakbuzForm = new TABMakbuzForm(ve);
            ve.BeginTransaction();
            IDataReader dr = tabMakbuzForm.BilgiListele(bilgi);
            while (dr.Read())
            {
                Makbuz makbuz = new Makbuz();
                makbuz.yil = VeriErisim.DegerAl(dr, 0, 0);
                makbuz.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                makbuz.birim = VeriErisim.DegerAl(dr, 2, "");
                makbuz.surecNo = VeriErisim.DegerAl(dr, 3, "");
                makbuz.firma.kod = VeriErisim.DegerAl(dr, 4, 0);
                makbuz.firma.ad = GELServis.servisUZY.UzayDegeriStr(kullanan, "HRCFIRMABILGISI", "-" + makbuz.muhasebe + "-" + makbuz.firma.kod, false);
                makbuz.tasinmaz.tasinmazSistemKodu = VeriErisim.DegerAl(dr, 5, "");
                makbuz.taksitno = VeriErisim.DegerAl(dr, 6, 0);
                makbuz.prmakbuzno = VeriErisim.DegerAl(dr, 7, 0);
                makbuz.makbuzno = VeriErisim.DegerAl(dr, 8, "");
                makbuz.makbuztarihi = VeriErisim.DegerAl(dr,9);
                makbuz.kdvTevkifati = VeriErisim.DegerAl(dr, 10, (decimal)0);
                makbuz.gelirtevkifati = VeriErisim.DegerAl(dr, 11, (decimal)0);
                makbuz.aciklama = VeriErisim.DegerAl(dr, 12, "");
                makbuzlar.objeler.Add(makbuz);
            }
            dr.Close();
            foreach (Makbuz item in makbuzlar.objeler)
            {
                dr = tabMakbuzForm.DetayListele(item.yil, item.muhasebe, item.birim, item.surecNo, item.firma.kod, item.tasinmaz.tasinmazSistemKodu, 0,item.taksitno);
                while (dr.Read())
                {
                    MakbuzDetay mkbzdetay = new MakbuzDetay();
                    mkbzdetay.yil = VeriErisim.DegerAl(dr, 0, 0);
                    mkbzdetay.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                    mkbzdetay.birim = VeriErisim.DegerAl(dr, 2, "");
                    mkbzdetay.surecno = VeriErisim.DegerAl(dr, 3, "");
                    mkbzdetay.firma.kod = VeriErisim.DegerAl(dr, 4, 0);
                    mkbzdetay.tasinmaz.tasinmazSistemKodu = VeriErisim.DegerAl(dr, 5, "");            
                    mkbzdetay.taksitno = VeriErisim.DegerAl(dr, 6, 0);
                    mkbzdetay.sirano = VeriErisim.DegerAl(dr, 7, 0);
                    mkbzdetay.prmakbuzno = VeriErisim.DegerAl(dr, 8, 0);
                    mkbzdetay.miktar = VeriErisim.DegerAl(dr, 9, (decimal)0);
                    mkbzdetay.kdv = VeriErisim.DegerAl(dr, 10, (decimal)0);
                    mkbzdetay.tutar = VeriErisim.DegerAl(dr, 11, (decimal)0);
                    item.detay.objeler.Add(mkbzdetay);
                }
                dr.Close();
                TABTasinmazTanim tabTTanim = new TABTasinmazTanim(ve);
                foreach (MakbuzDetay makdet in item.detay.objeler)
                {
                    makdet.tasinmaz.tasinmazAd = tabTTanim.TasinmazAdiGetir(makdet.tasinmaz.tasinmazSistemKodu, makdet.tasinmaz.yil);
                }
            }
            ve.VeriErisimKapat();        
            return makbuzlar;
        }

        public static Sonuc MakbuzSil(VeriErisim veriErisim, KYM.Kullanici kullanan, Makbuz bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc();
            string hata = MakbuzFormKontrol(bilgi, ENUMIslemTuru.SILME,islemkod,parcakod);
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
            TABMakbuzForm tabMakbuzForm = new TABMakbuzForm(ve);
            ve.BeginTransaction();
            tabMakbuzForm.DetaySil(bilgi.yil, bilgi.muhasebe, bilgi.birim, bilgi.surecNo, bilgi.firma.kod,  bilgi.tasinmaz.tasinmazSistemKodu,bilgi.taksitno, 0);
            if (ve.hata.Trim() == "")
            {
                tabMakbuzForm.BilgiSil(bilgi);
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

        public static Sonuc MakbuzDetaySil(VeriErisim veriErisim, KYM.Kullanici kullanan, MakbuzDetay bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc();
            string hata = MakbuzFormDetayKontrol(bilgi, ENUMIslemTuru.SILME);
            if (hata.Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }
            //*****Sürece ait parça durum kontrolü
            bool parcatamamlandi = GELSurec.ParcaTamamlandiMi(kullanan, bilgi.yil, bilgi.surecno, islemkod, parcakod);
            if (parcatamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00107;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABMakbuzForm tabMakbuzForm = new TABMakbuzForm(ve);
            ve.BeginTransaction();
            tabMakbuzForm.DetaySil(bilgi.yil, bilgi.muhasebe, bilgi.birim, bilgi.surecno, bilgi.firma.kod, bilgi.tasinmaz.tasinmazSistemKodu,bilgi.taksitno, bilgi.sirano);
            if (ve.hata.Trim() == "")
            {
               
                //***********Süreç Durumu Devam Ediyor Olarak Güncelleniyor****
                Surec s = new Surec();
                s.yil = bilgi.yil;
                s.surecNo = bilgi.surecno;
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
            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}
