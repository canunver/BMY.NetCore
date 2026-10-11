using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.KYM;
using TNS.GEL.IsKurallari.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari
{
    public class GELOnayForm
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, OnayForm bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00003;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            StringBuilder hatalar = new StringBuilder();

            if (islemkod <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00109 + "<br/>");
            if (parcakod <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00110 + "<br/>");
            if (bilgi.yil <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00022 + "<br/>");
            if (bilgi.muhasebe <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00023 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.birim))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00024 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.surecNo.Trim()))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00026 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.isTanimi.Trim()))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00027 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.isNiteligi.Trim()))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00028 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.isMiktari.Trim()))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00029 + "<br/>");
            if (bilgi.muhammenBedel <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00030 + "<br/>");
            if (bilgi.ihaleUsulu <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00031 + "<br/>");
            if (bilgi.ihaleMaddesi <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00032 + "<br/>");
            if (bilgi.ilanSekli.Trim() == "")
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00033 + "<br/>");

            if (hatalar.ToString().Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString().Trim();
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
            TABOnayForm tabOnayForm = new TABOnayForm(ve);
            ve.BeginTransaction();

            //***********önce onay formuna ait tertip bilgileri kaydediliyor
            if (bilgi.tertip.objeler.Count == 0) sonuc.islemSonuc = true;
            OnayFormTertip kriterTertip = new OnayFormTertip();
            kriterTertip.yil = bilgi.yil;
            kriterTertip.surecNo = bilgi.surecNo;
            TABOnayFormTertip tabTertip = new TABOnayFormTertip(ve);
            tabTertip.TertipSil(kriterTertip);//***onay formuna ait tüm tertipleri sil(yil ve süreç no kriter)
            if (ve.hata.Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ve.hata.Trim();
            }
            else
            {
                foreach (OnayFormTertip tertip in bilgi.tertip.objeler)
                {
                    sonuc = tabTertip.TertipKaydet(tertip);
                    if (!sonuc.islemSonuc) break;
                }
            }
            //**********onayformu bilgilerini de kaydet
            if (sonuc.islemSonuc)//**tertip bilgileri kaydetme işleminde sorun çıkmadı
            {
                tabOnayForm.Sil(bilgi);
                if (ve.hata.Trim() == "")
                {
                    sonuc = tabOnayForm.Kaydet(bilgi);
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
                else //*****onay formu bilgisi silerken sorun çıktı
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ve.hata.Trim();
                    ve.Rollback();
                }
            }
            else//*****onay formu tertibi silerken sorun çıktı
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, OnayForm bilgi)
        {
            ObjectArray onayFormBilgileri = new ObjectArray();
            StringBuilder hatalar = new StringBuilder();
            if (bilgi.yil <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00022 + "<br/>");
            if (bilgi.muhasebe <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00023 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.birim))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00024 + "<br/>");
            if (string.IsNullOrEmpty(bilgi.surecNo))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00026 + "<br/>");
            if (hatalar.ToString().Trim() != "")
            {
                onayFormBilgileri.sonuc.islemSonuc = false;
                onayFormBilgileri.sonuc.hataStr = hatalar.ToString().Trim();
                return onayFormBilgileri;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABOnayForm tabOnayForm = new TABOnayForm(ve);
            ve.BeginTransaction();
            IDataReader dr = tabOnayForm.Listele(bilgi);
            OnayForm oform = new OnayForm();
            if (dr != null)
            {
                oform.yil = bilgi.yil;
                oform.muhasebe = bilgi.muhasebe;
                oform.birim = bilgi.birim;
                oform.surecNo = bilgi.surecNo;
                while (dr.Read())
                {
                    //oform.yil = VeriErisim.DegerAl(dr, 0, 0);
                    //oform.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                    //oform.birim = VeriErisim.DegerAl(dr, 2, "");
                    //oform.surecNo = VeriErisim.DegerAl(dr, 3, "");
                    oform.isTanimi = VeriErisim.DegerAl(dr, 4, "");
                    oform.isNiteligi = VeriErisim.DegerAl(dr, 5, "");
                    oform.isMiktari = VeriErisim.DegerAl(dr, 6, "");
                    oform.muhammenBedel = VeriErisim.DegerAl(dr, 7, 0);
                    oform.muhammenBedelPBKod = VeriErisim.DegerAl(dr, 8, 0);
                    oform.geciciTeminatMiktari = VeriErisim.DegerAl(dr, 9, "");
                    oform.avansVarmi = VeriErisim.DegerAl(dr, 10, 0);
                    oform.avansSartlari = VeriErisim.DegerAl(dr, 11, "");
                    oform.onIlanVarMi = VeriErisim.DegerAl(dr, 12, 0);
                    oform.ihaleUsulu = VeriErisim.DegerAl(dr, 13, 0);
                    oform.ihaleMaddesi = VeriErisim.DegerAl(dr, 14, 0);
                    oform.ilanVerilecekmi = VeriErisim.DegerAl(dr, 15, 0);
                    oform.ilanSekli = VeriErisim.DegerAl(dr, 16, "");
                    oform.aciklama = VeriErisim.DegerAl(dr, 17, "");
                    oform.projeNo = VeriErisim.DegerAl(dr, 18, "");
                    oform.dokumanBedeli = VeriErisim.DegerAl(dr, 19, "");
                    oform.fiyatFarkiVarmi = VeriErisim.DegerAl(dr, 20, 0);
                    oform.komisyonVarmi = VeriErisim.DegerAl(dr, 21, 0);
                    oform.dokumanPostaSatisVarMi = VeriErisim.DegerAl(dr, 22, 0);
                    oform.ihaleMakami = VeriErisim.DegerAl(dr, 23, "");
                    oform.evrakSayi = VeriErisim.DegerAl(dr, 24, "");
                    oform.belgeTarihi = VeriErisim.DegerAl(dr, 25);
                    oform.onIlanGercekGorevli = VeriErisim.DegerAl(dr, 26, "");
                    oform.onIlanGercekGorevliUnvan = VeriErisim.DegerAl(dr, 27, "");
                    oform.onIlanOnayaSunumTarihi = VeriErisim.DegerAl(dr, 28);
                    oform.onIlanIhaleYetkili = VeriErisim.DegerAl(dr, 29, "");
                    oform.onIlanIhaleYetkiliUnvan = VeriErisim.DegerAl(dr, 30, "");
                    oform.onIlanOnaylamaTarihi = VeriErisim.DegerAl(dr, 31);
                    oform.sozlesmeYapilacak = VeriErisim.DegerAl(dr, 32, 0);
                }
                dr.Close();
                if (oform.muhammenBedel == 0)
                {
                    oform.muhammenBedel = tabOnayForm.MuhammenBedel(oform.yil, oform.muhasebe, oform.birim, oform.surecNo);
                }
                onayFormBilgileri.objeler.Add(oform);
                //onaya ait bütçe tertipleri alınıyor
                foreach (OnayForm of in onayFormBilgileri.objeler)
                {
                    TABOnayFormTertip tabTertip = new TABOnayFormTertip(ve);
                    ObjectArray tertipler = new ObjectArray();
                    OnayFormTertip tertipKriter = new OnayFormTertip();
                    tertipKriter.yil = of.yil;
                    tertipKriter.surecNo = of.surecNo;
                    dr = tabTertip.TertipListele(tertipKriter);
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            OnayFormTertip ttip = new OnayFormTertip();
                            ttip.yil = VeriErisim.DegerAl(dr, 0, 0);
                            ttip.surecNo = VeriErisim.DegerAl(dr, 1, "");
                            ttip.siraNo = VeriErisim.DegerAl(dr, 2, 0);
                            ttip.kurKod = VeriErisim.DegerAl(dr, 3, "");
                            ttip.ekoKod = VeriErisim.DegerAl(dr, 4, "");
                            ttip.pebKod = VeriErisim.DegerAl(dr, 5, "");
                            ttip.odenekTutari = VeriErisim.DegerAl(dr, 6, 0.0);
                            ttip.blokeTutari = VeriErisim.DegerAl(dr, 7, 0.0);
                            ttip.kullanilacakTutar = VeriErisim.DegerAl(dr, 8, 0.0);
                            ttip.kdvTutar = VeriErisim.DegerAl(dr, 9, 0.0);
                            of.tertip.objeler.Add(ttip);
                        }
                        dr.Close();
                    }



                }
            }
            ve.VeriErisimKapat();
            return onayFormBilgileri;
        }


    }
}
