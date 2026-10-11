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
    public class GELIhaleIlanTanim
    {
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int ihaleKod, int ihaleMaddeSira, int tur)
        {
            ObjectArray bilgi = new ObjectArray();
            VeriErisim ve = null;
            if (tur == (int)ENUMModulTuru.Gelir)
            {
                ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                TABGELIhaleIlanTanim to = new TABGELIhaleIlanTanim(ve);
                IDataReader dr = to.Listele(ihaleKod, ihaleMaddeSira);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        IhaleIlanTanim iit = new IhaleIlanTanim();
                        iit.ihaleKod = VeriErisim.DegerAl(dr, 0, 0);
                        iit.ihaleMaddeSira = VeriErisim.DegerAl(dr, 1, 0);
                        iit.aciklama = VeriErisim.DegerAl(dr, 2, "");
                        iit.yaklasikMaliyetMalHizmet = VeriErisim.DegerAl(dr, 3, 0.0);
                        iit.yaklasikMaliyetMalHizmetUst = VeriErisim.DegerAl(dr, 4, 0.0);
                        iit.yaklasikMaliyetYapim = VeriErisim.DegerAl(dr, 5, 0.0);
                        iit.yaklasikMaliyetYapimUst = VeriErisim.DegerAl(dr, 6, 0.0);
                        iit.zaman = VeriErisim.DegerAl(dr, 7, 0);
                        iit.zamanBaslamaTur = VeriErisim.DegerAl(dr, 8, 0);
                        bilgi.objeler.Add(iit);
                    }
                    dr.Close();
                }
            }
            else if (tur == (int)ENUMModulTuru.Harcama)
            {
                ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC");
                TABHRCIhaleIlanTanim to = new TABHRCIhaleIlanTanim(ve);
                IDataReader dr = to.Listele(ihaleKod, ihaleMaddeSira);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        IhaleIlanTanim iit = new IhaleIlanTanim();
                        iit.ihaleKod = VeriErisim.DegerAl(dr, 0, 0);
                        iit.ihaleMaddeSira = VeriErisim.DegerAl(dr, 1, 0);
                        iit.aciklama = VeriErisim.DegerAl(dr, 2, "");
                        iit.yaklasikMaliyetMalHizmet = VeriErisim.DegerAl(dr, 3, 0.0);
                        iit.yaklasikMaliyetMalHizmetUst = VeriErisim.DegerAl(dr, 4, 0.0);
                        iit.yaklasikMaliyetYapim = VeriErisim.DegerAl(dr, 5, 0.0);
                        iit.yaklasikMaliyetYapimUst = VeriErisim.DegerAl(dr, 6, 0.0);
                        iit.zaman = VeriErisim.DegerAl(dr, 7, 0);
                        iit.zamanBaslamaTur = VeriErisim.DegerAl(dr, 8, 0);
                        bilgi.objeler.Add(iit);
                    }
                    dr.Close();
                }
            }
            else
            {

                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00048;
                return bilgi;

            }
            ve.VeriErisimKapat();
            return bilgi;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, IhaleIlanTanim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            StringBuilder hatalar = new StringBuilder();
            VeriErisim ve = null;
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");

            if (bilgi.ihaleKod <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00034 + "<br/>");

            if (bilgi.yaklasikMaliyetMalHizmet <= 0 || bilgi.yaklasikMaliyetMalHizmetUst <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00046);

            if (bilgi.yaklasikMaliyetYapim <= 0 || bilgi.yaklasikMaliyetYapimUst <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00047 + "<br/>");

            if (hatalar.ToString().Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }

            if (bilgi.tur == (int)ENUMModulTuru.Gelir)
            {
                ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                TABGELIhaleIlanTanim tabIHLITanim = new TABGELIhaleIlanTanim(ve);
                ve.BeginTransaction();
                int kayitSayisi = tabIHLITanim.KayitVarmi(bilgi);
                if (kayitSayisi == 0)
                {
                    if (bilgi.ihaleMaddeSira <= 0)
                        bilgi.ihaleMaddeSira = tabIHLITanim.KayitSiraNoAl(bilgi);

                    sonuc = tabIHLITanim.Kaydet(bilgi);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00044;
                    }
                    else
                    {
                        ve.Rollback();
                    }
                }
                else//güncelle
                {
                    if (bilgi.ihaleMaddeSira <= 0)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00049;
                        return sonuc;
                    }
                    sonuc = tabIHLITanim.Guncelle(bilgi);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00044;
                    }
                    else
                    {
                        ve.Rollback();
                    }
                }
            }
            else if (bilgi.tur == (int)ENUMModulTuru.Harcama)
            {
                ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC");
                TABHRCIhaleIlanTanim tabIHLITanim = new TABHRCIhaleIlanTanim(ve);
                ve.BeginTransaction();
                int kayitSayisi = tabIHLITanim.KayitVarmi(bilgi);
                if (kayitSayisi == 0)//kaydet
                {
                    if (bilgi.ihaleMaddeSira <= 0)
                        bilgi.ihaleMaddeSira = tabIHLITanim.KayitSiraNoAl(bilgi);

                    sonuc = tabIHLITanim.Kaydet(bilgi);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00044;
                    }
                    else
                    {
                        ve.Rollback();
                    }
                }
                else//güncelle
                {
                    if (bilgi.ihaleMaddeSira <= 0)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00049;
                        return sonuc;
                    }
                    sonuc = tabIHLITanim.Guncelle(bilgi);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00044;
                    }
                    else
                    {
                        ve.Rollback();
                    }
                }
            }
            else
            {

                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00048;
                return sonuc;

            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, IhaleIlanTanim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");

            if (bilgi.ihaleKod <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00034 + "<br/>");

            if (bilgi.ihaleMaddeSira <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00049 + "<br/>");

           if (hatalar.ToString().Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }
           VeriErisim ve = null;
           if (bilgi.tur==(int)ENUMModulTuru.Gelir)
           {
               ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
               TABGELIhaleIlanTanim tabIHLITanim = new TABGELIhaleIlanTanim(ve);
               ve.BeginTransaction();
               tabIHLITanim.Sil(bilgi);
               if (ve.hata.Trim() == "")
               {
                   sonuc.islemSonuc = true;
                   sonuc.hataStr = "";
                   sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00045;
                   ve.Commit();
               }
               else
               {
                   ve.Rollback();
                   sonuc.islemSonuc = false;
                   sonuc.hataStr = ve.hata;
               }

           }
           else if (bilgi.tur == (int)ENUMModulTuru.Harcama)
           {
               ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC");
               TABHRCIhaleIlanTanim tabIHLITanim = new TABHRCIhaleIlanTanim(ve);
               ve.BeginTransaction();
               tabIHLITanim.Sil(bilgi);
               if (ve.hata.Trim() == "")
               {
                   sonuc.islemSonuc = true;
                   sonuc.hataStr = "";
                   sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00045;
                   ve.Commit();
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
               sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00048;
               return sonuc;

           }
                
             
            ve.VeriErisimKapat();
            return sonuc;
        }

    }
}
