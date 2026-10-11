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
    public class GELMuhammenBedel
    {
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, MuhammenBedel yaklasikMaliyet)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriGEL");
            ObjectArray bilgi = new ObjectArray();

            TABMuhammenBedel tabMb = new TABMuhammenBedel(ve);
            TABTasinmazTanim tabTasTanim = new TABTasinmazTanim(ve);
            ve.BeginTransaction();
            IDataReader dr = tabMb.Listele(yaklasikMaliyet);
            if (dr != null)
            {
                MuhammenBedel mb = new MuhammenBedel();

                while (dr.Read())
                {
                    mb.yil = VeriErisim.DegerAl(dr, 0, 0);
                    mb.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                    mb.birim = VeriErisim.DegerAl(dr, 2, "");
                    mb.surecNo = VeriErisim.DegerAl(dr, 3, "");

                    MuhammenBedelSatir mbs = new MuhammenBedelSatir();
                    mbs.tasinmazKodu = VeriErisim.DegerAl(dr, 4, "");
                    mbs.islem.kod = VeriErisim.DegerAl(dr, 5, 0);
                    mbs.islem.ad = VeriErisim.DegerAl(dr, 6, "");
                    mbs.kiralamaSuresi = VeriErisim.DegerAl(dr, 7, 0);
                    mbs.kiralamaTipi = VeriErisim.DegerAl(dr, 8, 0);
                    mbs.birimFiyat = VeriErisim.DegerAl(dr, 9, 0.0);
                    mbs.tutar = VeriErisim.DegerAl(dr, 10, 0.0);
                    mbs.aciklama = VeriErisim.DegerAl(dr, 11, "");

                    mb.satirlar.objeler.Add(mbs);

                }
                bilgi.objeler.Add(mb);
                dr.Close();
                foreach (MuhammenBedelSatir item in mb.satirlar.objeler)
                {
                    item.tasinmazAdi = tabTasTanim.TasinmazAdiGetir(item.tasinmazKodu,item.tasinmazyili);
                }
            }
            ve.VeriErisimKapat();
            return bilgi;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, MuhammenBedel bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00003;
                sonuc.islemSonuc = false;
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

            string hatalar = VeriKontrol(bilgi, ENUMIslemTuru.KAYIT, islemkod, parcakod);
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            ve.BeginTransaction();
            if (hatalar.Trim() == "")
            {

                TABMuhammenBedel tabMb = new TABMuhammenBedel(ve);
                tabMb.Sil(bilgi);
                if (ve.hata.Trim() == "")
                {
                    sonuc.islemSonuc = true;
                    foreach (MuhammenBedelSatir mbs in bilgi.satirlar.objeler)
                    {
                        if (!sonuc.islemSonuc) break;
                        sonuc = tabMb.Kaydet(bilgi, mbs);
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
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ve.hata;
                }

            }
            else
            {
                sonuc.hataStr = hatalar;
                sonuc.islemSonuc = false;
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, MuhammenBedel bilgi, int islemkod, int parcakod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            string hatalar = VeriKontrol(bilgi, ENUMIslemTuru.SILME, islemkod, parcakod);
            //*****Sürece ait parça durum kontrolü
            bool parcatamamlandi = GELSurec.ParcaTamamlandiMi(kullanan, bilgi.yil, bilgi.surecNo, islemkod, parcakod);
            if (parcatamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00107;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            ve.BeginTransaction();
            if (hatalar.Trim() == "")
            {
                TABMuhammenBedel tabMb = new TABMuhammenBedel(ve);
                tabMb.Sil(bilgi);
                if (ve.hata.Trim() == "")
                    ve.Commit();
                else
                    ve.Rollback();

            }
            else
            {
                sonuc.hataStr = hatalar;
                sonuc.islemSonuc = false;
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static string VeriKontrol(MuhammenBedel bilgi, ENUMIslemTuru islemturu, int islemkodu, int parcakodu)
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
            foreach (MuhammenBedelSatir item in bilgi.satirlar.objeler)
            {
                if (string.IsNullOrEmpty(item.tasinmazKodu))
                {
                    hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00005 + "<br/>");
                    break;
                }
            }
            return hatalar.ToString();
        }
    }
}
