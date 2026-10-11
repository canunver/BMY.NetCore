using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.GEL.IsKurallari.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari
{
    public class GELIhaleTeklif
    {


        protected static string FirmaKontrol(TNS.HRC.TeklifFirma bilgi, ENUMIslemTuru islemturu, int islemkodu=0, int parcakodu=0)
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
            if (bilgi.firmaNo <= 0 && islemturu != ENUMIslemTuru.LISTELEME)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00057 + "<br/>");
            return hatalar.ToString();
        }
        protected static string TasinmazKontrol(TeklifTasinmaz bilgi, ENUMIslemTuru islemturu, int islemkodu=0, int parcakodu=0)
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
            if (string.IsNullOrEmpty(bilgi.tasinmazBilgisi.tasinmazSistemKodu) && islemturu != ENUMIslemTuru.LISTELEME)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00058 + "<br/>");
            if (bilgi.kiralanmaSuresi <= 0 && islemturu == ENUMIslemTuru.KAYIT)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00059 + "<br/>");
            if (bilgi.kiralanmaTipi <= 0 && islemturu == ENUMIslemTuru.KAYIT)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00060 + "<br/>");
            return hatalar.ToString();
        }
        protected static string TeklifKontrol(Teklif bilgi, ENUMIslemTuru islemturu, int islemkodu = 0, int parcakodu = 0, bool tasinmazKodKontrol = false)
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
            if (islemturu != ENUMIslemTuru.LISTELEME && islemturu != ENUMIslemTuru.KAYITKONTROL)
            {
                if (bilgi.teklifno <= 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00062 + "<br/>");
                if (string.IsNullOrEmpty(bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu)  && tasinmazKodKontrol == true)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00058 + "<br/>");
                if (bilgi.firma.firmaNo <= 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00057 + "<br/>");
                if (bilgi.tip <= 0)
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00063 + "<br/>");
            }

            return hatalar.ToString();
        }

        public static Sonuc TeklifKaydiVarmi(VeriErisim veriErisim, KYM.Kullanici kullanan, TNS.GEL.Teklif bilgi)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            string hata = "";
            string satirNo = "";

            hata = TeklifKontrol(bilgi, ENUMIslemTuru.KAYITKONTROL);
            if (hata != "")
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00061 + satirNo + "<br/>" + hata + "<br/>");

            if (hatalar.ToString().Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABIhaleTeklif tabTeklif = new TABIhaleTeklif(ve);
            ve.BeginTransaction();
            IDataReader dr = tabTeklif.TeklifKayitVarmi(bilgi.yil, bilgi.muhasebe, bilgi.birim, bilgi.surecNo);
            int teklifTipi = 0;
            while (dr.Read())
            {
                teklifTipi = VeriErisim.DegerAl(dr, 0, 0);
            }
            dr.Close();
            if (teklifTipi != bilgi.tip && teklifTipi != (int)ENUMIHALETeklifTuru.TANIMSIZ)
            {
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00099;
                if (teklifTipi == (int)ENUMIHALETeklifTuru.BIRIMFIYAT)
                    sonuc.bilgiStr += TNS.GEL.GelirSurecleriResI.GEL00102;
                else if (teklifTipi == (int)ENUMIHALETeklifTuru.KISMITEKLIF)
                    sonuc.bilgiStr += TNS.GEL.GelirSurecleriResI.GEL00103;
                else if (teklifTipi == (int)ENUMIHALETeklifTuru.GOTURUBEDEL)
                    sonuc.bilgiStr += TNS.GEL.GelirSurecleriResI.GEL00101;
                else if (teklifTipi == (int)ENUMIHALETeklifTuru.PROTOKOL)
                    sonuc.bilgiStr += TNS.GEL.GelirSurecleriResI.GEL00104;
                sonuc.bilgiStr += TNS.GEL.GelirSurecleriResI.GEL00100;
            }
            else
                sonuc.islemSonuc = false;
            ve.VeriErisimKapat();
            return sonuc;
        }
        public static Sonuc TeklifFirmaKaydet(VeriErisim veriErisim, KYM.Kullanici kullanan, TNS.GEL.TeklifFirma[] bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc();
            int yil = 0;
            string surecno = string.Empty;
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            string hata = "";
            if (bilgi.Length == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00108;
                return sonuc;
            }
            for (int i = 0; i < bilgi.Length; i++)
            {
                TNS.GEL.TeklifFirma firma = (TNS.GEL.TeklifFirma)bilgi[i];
                hata = FirmaKontrol(firma, ENUMIslemTuru.KAYIT, islemkod, parcakod);
                if (hata.Trim() != "")
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00061 + (i + 1).ToString() + "<br/>" + hata);
            }

            if (hatalar.ToString().Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }

            //*****Sürece ait parça durum kontrolü
            bool parcatamamlandi = GELSurec.ParcaTamamlandiMi(kullanan, bilgi[0].yil, bilgi[0].surecNo, islemkod, parcakod);
            if (parcatamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00107;
                sonuc.anahtar = parcatamamlandi.ToString();
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABIhaleTeklif tabTeklif = new TABIhaleTeklif(ve);
            ve.BeginTransaction();
            if (bilgi.Length > 0)
            {
                sonuc.islemSonuc = true;
                for (int i = 0; i < bilgi.Length; i++)
                {
                    TNS.GEL.TeklifFirma firma = (TNS.GEL.TeklifFirma)bilgi[i];
                    yil = firma.yil;
                    surecno = firma.surecNo;
                    bool kayitVar = tabTeklif.TeklifFirmaVarmi(firma);
                    if (!kayitVar) //kayıt yoksa ekle varsada update e gerek yok
                    {
                        sonuc = tabTeklif.TeklifFirmaKaydet(firma);
                        if (!sonuc.islemSonuc) break;
                    }
                }
                if (sonuc.islemSonuc)
                {
                    //***********Süreç Durumu Devam Ediyor Olarak Güncelleniyor****
                    Surec s = new Surec();
                    s.yil = yil;
                    s.surecNo = surecno;
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
            ve.VeriErisimKapat();
            return sonuc;
        }
        public static ObjectArray TeklifFirmaListele(VeriErisim veriErisim, KYM.Kullanici kullanan, TNS.HRC.TeklifFirma bilgi)
        {
            ObjectArray donusdeger = new ObjectArray();
            string hata = FirmaKontrol(bilgi, ENUMIslemTuru.LISTELEME);
            if (hata.Trim() != "")
            {
                donusdeger.sonuc.islemSonuc = false;
                donusdeger.sonuc.hataStr = hata;
                return donusdeger;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABIhaleTeklif tabTeklif = new TABIhaleTeklif(ve);
            ve.BeginTransaction();
            IDataReader dr = tabTeklif.TeklifFirmaListele(bilgi);
            if (dr != null)
            {
                while (dr.Read())
                {
                    TeklifFirma firma = new TeklifFirma();
                    firma.yil = VeriErisim.DegerAl(dr, 0, 0);
                    firma.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                    firma.birim = VeriErisim.DegerAl(dr, 2, "");
                    firma.surecNo = VeriErisim.DegerAl(dr, 3, "");
                    firma.firmaNo = VeriErisim.DegerAl(dr, 4, 0);
                    firma.ad = GELServis.servisUZY.UzayDegeriStr(kullanan, "HRCFIRMABILGISI", "-" + firma.muhasebe + "-" + firma.firmaNo, false);
                    firma.firmaBilgisi.adres = GELServis.servisUZY.UzayDegeriStr(kullanan, "HRCFIRMABILGISI", "-" + firma.muhasebe + "-" + firma.firmaNo, false, "ADRES");
                    donusdeger.objeler.Add(firma);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return donusdeger;
        }
        public static Sonuc TeklifFirmaSil(VeriErisim veriErisim, KYM.Kullanici kullanan, TeklifFirma bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            hatalar.Append(FirmaKontrol(bilgi, ENUMIslemTuru.SILME, islemkod, parcakod));
            if (hatalar.ToString().Trim() != "")
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
            TABIhaleTeklif tabTeklif = new TABIhaleTeklif(ve);
            ve.BeginTransaction();
            if (!tabTeklif.FirmayaAitTeklifVarmi(bilgi))
            {
                tabTeklif.TeklifFirmaSil(bilgi);
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
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00066;
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc TeklifTasinmazKaydet(VeriErisim veriErisim, KYM.Kullanici kullanan, TeklifTasinmaz[] bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc();
            int yil = 0;
            string surecno = string.Empty;
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            string satirNo = "";
            string hata = "";
            if (bilgi.Length == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00108;
                return sonuc;
            }
            for (int i = 0; i < bilgi.Length; i++)
            {
                satirNo = (i + 1).ToString();
                TeklifTasinmaz tasinmaz = (TeklifTasinmaz)bilgi[i];
                hata = TasinmazKontrol(tasinmaz, ENUMIslemTuru.KAYIT, islemkod, parcakod);
                if (hata.Trim() != "")
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00061 + satirNo + "<br/>" + hata);
            }
            if (hatalar.ToString().Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }

            //*****Sürece ait parça durum kontrolü
            bool parcatamamlandi = GELSurec.ParcaTamamlandiMi(kullanan, bilgi[0].yil, bilgi[0].surecNo, islemkod, parcakod);
            if (parcatamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00107;
                sonuc.anahtar = parcatamamlandi.ToString();
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABIhaleTeklif tabTeklif = new TABIhaleTeklif(ve);
            ve.BeginTransaction();
            bool kayitVar = false;
            sonuc.islemSonuc = true;
            for (int i = 0; i < bilgi.Length; i++)
            {
                TeklifTasinmaz tasinmaz = (TeklifTasinmaz)bilgi[i];
                yil = tasinmaz.yil;
                surecno = tasinmaz.surecNo;
                kayitVar = tabTeklif.TeklifTasinmazVarmi(tasinmaz);
                if (!sonuc.islemSonuc) break;
                if (kayitVar)
                {
                    sonuc = tabTeklif.TeklifTasinmazGuncelle(tasinmaz);
                }
                else
                {
                    sonuc = tabTeklif.TeklifTasinmazKaydet(tasinmaz);
                }
            }
            if (sonuc.islemSonuc)
            {
                //***********Süreç Durumu Devam Ediyor Olarak Güncelleniyor****
                Surec s = new Surec();
                s.yil = yil;
                s.surecNo = surecno;
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
            ve.VeriErisimKapat();
            return sonuc;
        }
        public static Sonuc TeklifTasinmazSil(VeriErisim veriErisim, KYM.Kullanici kullanan, TeklifTasinmaz bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hata = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hata.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");

            hata.Append(TasinmazKontrol(bilgi, ENUMIslemTuru.SILME, islemkod, parcakod));
            if (hata.ToString().Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata.ToString();
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
            TABIhaleTeklif tabTeklif = new TABIhaleTeklif(ve);
            ve.BeginTransaction();
            if (!tabTeklif.TasinmazaAitTeklifVarmi(bilgi))
            {
                tabTeklif.TeklifTasinmazSil(bilgi);
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
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00067;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
        public static ObjectArray TeklifTasinmazListele(VeriErisim veriErisim, KYM.Kullanici kullanan, TeklifTasinmaz bilgi)
        {
            ObjectArray donusdeger = new ObjectArray();
            string hata = TasinmazKontrol(bilgi, ENUMIslemTuru.LISTELEME);
            if (hata.Trim() != "")
            {
                donusdeger.sonuc.islemSonuc = false;
                donusdeger.sonuc.hataStr = hata;
                return donusdeger;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABIhaleTeklif tabTeklif = new TABIhaleTeklif(ve);
            ve.BeginTransaction();
            IDataReader dr = tabTeklif.TeklifTasinmazListele(bilgi);
            if (dr != null)
            {
                while (dr.Read())
                {
                    TeklifTasinmaz tasinmaz = new TeklifTasinmaz();
                    tasinmaz.yil = VeriErisim.DegerAl(dr, 0, 0);
                    tasinmaz.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                    tasinmaz.birim = VeriErisim.DegerAl(dr, 2, "");
                    tasinmaz.surecNo = VeriErisim.DegerAl(dr, 3, "");
                    tasinmaz.tasinmazBilgisi.tasinmazSistemKodu = VeriErisim.DegerAl(dr, 4, "");
                    tasinmaz.kiralanmaSuresi = VeriErisim.DegerAl(dr, 5, 0);
                    tasinmaz.kiralanmaTipi = VeriErisim.DegerAl(dr, 6, 0);
                    donusdeger.objeler.Add(tasinmaz);
                }
                dr.Close();
            }
            TABTasinmazTanim tabTTanim = new TABTasinmazTanim(ve);
            foreach (TeklifTasinmaz item in donusdeger.objeler)
            {
                item.tasinmazBilgisi.tasinmazAd = tabTTanim.TasinmazAdiGetir(item.tasinmazBilgisi.tasinmazSistemKodu,item.tasinmazBilgisi.yil);
            }
            ve.VeriErisimKapat();
            return donusdeger;
        }

        public static Sonuc TeklifKaydet(VeriErisim veriErisim, KYM.Kullanici kullanan, Teklif[] bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc();
            int yil = 0;
            string surecno = string.Empty;
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            string hata = "";
            string satirNo = "";
            if (bilgi.Length == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00108;
                return sonuc;
            }
            for (int i = 0; i < bilgi.Length; i++)
            {
                hata = TeklifKontrol(bilgi[i], ENUMIslemTuru.KAYIT, islemkod, parcakod);
                satirNo = (i + 1).ToString();
                if (hata != "")
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00061 + satirNo + "<br/>" + hata + "<br/>");
            }
            if (hatalar.ToString().Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }
            //*****Sürece ait parça durum kontrolü
            bool parcatamamlandi = GELSurec.ParcaTamamlandiMi(kullanan, bilgi[0].yil, bilgi[0].surecNo, islemkod, parcakod);
            if (parcatamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00107;
                sonuc.anahtar = parcatamamlandi.ToString();
                return sonuc;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABIhaleTeklif tabTeklif = new TABIhaleTeklif(ve);
            ve.BeginTransaction();
            sonuc.islemSonuc = true;
            Teklif teklif = null;
            for (int i = 0; i < bilgi.Length; i++)
            {
                if (!sonuc.islemSonuc) break;
                teklif = (Teklif)bilgi[i];
                yil = teklif.yil;
                surecno = teklif.surecNo;
                //tabTeklif.TeklifSil(teklif);
                if (tabTeklif.TeklifKayitVarmi(teklif))
                    sonuc = tabTeklif.TeklifKayitGuncelle(teklif);
                else
                    sonuc = tabTeklif.TeklifKaydet(teklif);

                //if (ve.hata.Trim() == "")
                //{
                //    sonuc = tabTeklif.TeklifKaydet(teklif);
                //}
                //else
                //{
                //    sonuc.islemSonuc = false;
                //    sonuc.hataStr = ve.hata;
                //    break;
                //}
            }
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                VeriErisim veSurec = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                TABSurec tabSurec = new TABSurec(veSurec);
                veSurec.BeginTransaction();
                //sonuc = tabSurec.SurecTeklifTuruGuncelle(teklif.yil, teklif.surecNo, teklif.tip);
                //if (sonuc.islemSonuc)
                //{
                    //***********Süreç Durumu Devam Ediyor Olarak Güncelleniyor****
                    Surec s = new Surec();
                    s.yil = yil;
                    s.surecNo = surecno;
                    s.surecDurum = (int)TNS.HRC.ENUMSurecDurum.DEVAMEDIYOR;
                    sonuc = GELSurec.DurumGuncelle(veSurec, kullanan, s);
                    if (sonuc.islemSonuc)
                    {
                        veSurec.Commit();
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00056;
                    }
                    else
                    {
                        veSurec.Rollback();
                    }
                //}
                //else
                //{
                //    veSurec.Rollback();
                //}
                veSurec.VeriErisimKapat();
                sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00025;
            }
            else
            {
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
        public static Sonuc TeklifSil(VeriErisim veriErisim, KYM.Kullanici kullanan, Teklif[] bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            string hata = "";
            if (bilgi.Length == 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00068 + "<br/>");

            int satirNo = 1;
            if (bilgi.Length == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00108;
                return sonuc;
            }
            foreach (Teklif item in bilgi)
            {
                hata = TeklifKontrol(item, ENUMIslemTuru.SILME, islemkod, parcakod);
                if (hata != "")
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00061 + satirNo.ToString() + "<br/>" + hata + "<br/>");
            }

            if (hatalar.ToString().Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString(); ;
                return sonuc;
            }
            //*****Sürece ait parça durum kontrolü
            bool parcatamamlandi = GELSurec.ParcaTamamlandiMi(kullanan, bilgi[0].yil, bilgi[0].surecNo, islemkod, parcakod);
            if (parcatamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00107;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABIhaleTeklif tabTeklif = new TABIhaleTeklif(ve);
            ve.BeginTransaction();
            sonuc.islemSonuc = true;
            ve.hata = "";

            TABIhaleKomisyonKarar tabKarar = new TABIhaleKomisyonKarar(ve);
            bool kararVar = false;

            foreach (Teklif item in bilgi)
            {
                if (item.tip == (int)ENUMIHALETeklifTuru.KISMITEKLIF)
                    kararVar = tabKarar.KayitVarmi(item.yil, item.muhasebe, item.birim, item.surecNo, item.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu, item.tip);
                else
                    //*karar kaydedilirken toplam bedel üzerinden kaydedildiği için
                    //*taşınmaz kodu 0 dır
                    kararVar = tabKarar.KayitVarmi(item.yil, item.muhasebe, item.birim, item.surecNo, "", item.tip);
                if (kararVar)
                {
                    sonuc.islemSonuc = false;
                    ve.hata = TNS.GEL.GelirSurecleriResI.GEL00095;
                    break;
                }
                if (ve.hata.Trim() != "") break;
                tabTeklif.TeklifSil(item);

            }
            if (ve.hata.Trim() == "")
            {
                //***********Süreç Durumu Devam Ediyor Olarak Güncelleniyor****
                Surec s = new Surec();
                s.yil = bilgi[0].yil;
                s.surecNo = bilgi[0].surecNo;
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
        public static ObjectArray TeklifListele(VeriErisim veriErisim, KYM.Kullanici kullanan, Teklif bilgi)
        {
            ObjectArray teklifler = new ObjectArray();
            string hata = TeklifKontrol(bilgi, ENUMIslemTuru.LISTELEME);
            if (hata.Trim() != "")
            {
                teklifler.sonuc.islemSonuc = false;
                teklifler.sonuc.hataStr = hata;
                return teklifler;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABIhaleTeklif tabTeklif = new TABIhaleTeklif(ve);
            ve.BeginTransaction();
            IDataReader dr = tabTeklif.TeklifListele(bilgi);
            if (((System.Data.SqlClient.SqlDataReader)(dr)).HasRows)
            {
                while (dr.Read())
                {
                    Teklif t = new Teklif();
                    t.yil = bilgi.yil;
                    t.muhasebe = bilgi.muhasebe;
                    t.birim = bilgi.birim;
                    t.surecNo = bilgi.surecNo;
                    t.firma.firmaNo = VeriErisim.DegerAl(dr, 4, 0);
                    t.firma.ad = GELServis.servisUZY.UzayDegeriStr(kullanan, "HRCFIRMABILGISI", "-" + t.muhasebe + "-" + t.firma.firmaNo, false);
                    t.firma.firmaBilgisi.adres = GELServis.servisUZY.UzayDegeriStr(kullanan, "HRCFIRMABILGISI", "-" + t.muhasebe + "-" + t.firma.firmaNo, false, "ADRES");
                    t.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu = VeriErisim.DegerAl(dr, 5, "");
                    t.teklifno = VeriErisim.DegerAl(dr, 6, 0);
                    t.tutari = VeriErisim.DegerAl(dr, 7, 0.0);
                    t.tip = VeriErisim.DegerAl(dr, 8, 0);
                    t.tarihi = VeriErisim.DegerAl(dr, 9);
                    teklifler.objeler.Add(t);
                }
                dr.Close();
                foreach (Teklif teklif in teklifler.objeler)
                {
                    dr = tabTeklif.TasinmazKiralamaSuresiTipiGetir(teklif.yil, teklif.muhasebe, teklif.birim, teklif.surecNo, teklif.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu);
                    while (dr.Read())
                    {
                        teklif.tasinmaz.kiralanmaSuresi = VeriErisim.DegerAl(dr, 0, 0.0);
                        teklif.tasinmaz.kiralanmaTipi = VeriErisim.DegerAl(dr, 1, 0);
                    }
                    dr.Close();
                }

                TABTasinmazTanim tabTasTanim = new TABTasinmazTanim(ve);
                Tasinmaz tas = new Tasinmaz();
                foreach (Teklif teklif in teklifler.objeler)
                {
                    tas.tasinmazSistemKodu = teklif.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu;
                    dr = tabTasTanim.Listele(tas);
                    while (dr.Read())
                    {
                        teklif.tasinmaz.tasinmazBilgisi.tasinmazAd = VeriErisim.DegerAl(dr, 8, "");
                    }
                    dr.Close();
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    Teklif t = new Teklif();
                    t.yil = bilgi.yil;
                    t.muhasebe = bilgi.muhasebe;
                    t.birim = bilgi.birim;
                    t.surecNo = bilgi.surecNo;
                    t.firma.firmaNo = bilgi.firma.firmaNo;
                    t.firma.ad = GELServis.servisUZY.UzayDegeriStr(kullanan, "HRCFIRMABILGISI", "-" + t.muhasebe + "-" + t.firma.firmaNo, false);
                    t.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu = "";
                    t.teklifno = 0;
                    t.tutari = 0.0;
                    t.tip = 0;
                    t.tasinmaz.kiralanmaSuresi = 0.0;
                    t.tasinmaz.kiralanmaTipi = 0;
                    t.tarihi = new TNSDateTime(DateTime.Now);
                    teklifler.objeler.Add(t);
                }
            }

            ve.VeriErisimKapat();
            return teklifler;
        }

        public static ObjectArray SonTeklif(VeriErisim veriErisim, KYM.Kullanici kullanan, Teklif bilgi)
        {
            ObjectArray donusdeger = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABIhaleTeklif tabTeklif = new TABIhaleTeklif(ve);
            ve.BeginTransaction();

            IDataReader dr = tabTeklif.SonTeklifGetir(bilgi);
            Teklif teklif = new Teklif();
            while (dr.Read())
            {
                teklif.teklifno = VeriErisim.DegerAl(dr, 0, 0);
                teklif.tutari = VeriErisim.DegerAl(dr, 1, 0.0);
                donusdeger.objeler.Add(teklif);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return donusdeger;

        }
    }
}
