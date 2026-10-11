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
    public class GELIhaleTurMadde
    {
        public static ObjectArray Listele(VeriErisim veriErisim,Kullanici kullanan, int ihaleKod, int tur)
        {
            ObjectArray bilgi = new ObjectArray();
            VeriErisim ve = null;
            if (tur == (int)ENUMModulTuru.Gelir)
            {
                ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                TABGELIhaleTurMadde to = new TABGELIhaleTurMadde(ve);
                IDataReader dr = to.Listele(ihaleKod);
                if (dr!=null)
                {
                    while (dr.Read())
                    {
                        IhaleTurMadde itm = new IhaleTurMadde();
                        itm.kod = VeriErisim.DegerAl(dr, 0, 0);
                        itm.ihaleKod = VeriErisim.DegerAl(dr, 1, 0);
                        itm.adi = VeriErisim.DegerAl(dr, 2, "");
                        itm.aciklama = VeriErisim.DegerAl(dr, 3, "");
                        itm.ilanZorunlu = VeriErisim.DegerAl(dr, 4, 0);
                        bilgi.objeler.Add(itm);
                    }
                    dr.Close();
                }
            }
            else if (tur == (int)ENUMModulTuru.Harcama)
            {
                ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC");
                TABHRCIhaleTurMadde to = new TABHRCIhaleTurMadde(ve);
                IDataReader dr = to.Listele(ihaleKod);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        IhaleTurMadde itm = new IhaleTurMadde();
                        itm.kod = VeriErisim.DegerAl(dr, 0, 0);
                        itm.ihaleKod = VeriErisim.DegerAl(dr, 1, 0);
                        itm.adi = VeriErisim.DegerAl(dr, 2, "");
                        itm.aciklama = VeriErisim.DegerAl(dr, 3, "");
                        itm.ilanZorunlu = VeriErisim.DegerAl(dr, 4, 0);
                        bilgi.objeler.Add(itm);
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

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, IhaleTurMadde bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");

            if (bilgi.ihaleKod <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00034 + "<br/>");

            if (bilgi.adi.Trim() == "")
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00040 + "<br/>");

            if (bilgi.tur <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00048 + "<br/>");

            if (hatalar.ToString().Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString().Trim();
                return sonuc;
            }

            VeriErisim ve = null;
            if (bilgi.tur == (int)ENUMModulTuru.Gelir)
            {
                ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                TABGELIhaleTurMadde tabIHLTurMadde = new TABGELIhaleTurMadde(ve);
                ve.BeginTransaction();
                int kayitSayisi = tabIHLTurMadde.KayitVarmi(bilgi);
                if (kayitSayisi == 0)
                {
                    if (bilgi.kod <= 0)
                        bilgi.kod = tabIHLTurMadde.KayitSiraNoAl(bilgi);

                    sonuc = tabIHLTurMadde.Kaydet(bilgi);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00041;
                    }
                    else
                    {
                        ve.Rollback();
                    }
                }
                else
                {
                    if (bilgi.kod <= 0)
                    {
                        sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00038; ;
                        sonuc.islemSonuc = false;
                        return sonuc;
                    }
                    sonuc = tabIHLTurMadde.Guncelle(bilgi);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00041;
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
                TABHRCIhaleTurMadde tabIHLTurMadde = new TABHRCIhaleTurMadde(ve);
                ve.BeginTransaction();
                int kayitSayisi = tabIHLTurMadde.KayitVarmi(bilgi);
                if (kayitSayisi == 0)
                {
                    if (bilgi.kod <= 0)
                        bilgi.kod = tabIHLTurMadde.KayitSiraNoAl(bilgi);

                    sonuc = tabIHLTurMadde.Kaydet(bilgi);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00041;
                    }
                    else
                    {
                        ve.Rollback();
                    }
                }
                else
                {
                    if (bilgi.kod <= 0)
                    {
                        sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00038; ;
                        sonuc.islemSonuc = false;
                        return sonuc;
                    }
                    sonuc = tabIHLTurMadde.Guncelle(bilgi);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00041;
                    }
                    else
                    {
                        ve.Rollback();
                    }
                }
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, IhaleTurMadde bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))

                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");

            if (bilgi.ihaleKod <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00034 + "<br/>");

            if (bilgi.kod <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00038 + "<br/>");

            if (bilgi.tur <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00048 + "<br/>");

            if (hatalar.ToString().Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString().Trim();
                return sonuc;
            }
            VeriErisim ve = null;
            if (bilgi.tur == (int)ENUMModulTuru.Gelir)
            {
                ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                TABGELIhaleTurMadde tabIHLTurMadde = new TABGELIhaleTurMadde(ve);
                ve.BeginTransaction();
                tabIHLTurMadde.Sil(bilgi);
                if (ve.hata.Trim() == "")
                {
                    sonuc.islemSonuc = true;
                    sonuc.hataStr = "";
                    sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00042;
                    ve.Commit();
                }
                else
                {
                    ve.Rollback();
                    sonuc.islemSonuc = false;
                    if (ve.hata.Trim().IndexOf("(0x80131904): The DELETE statement conflicted with the REFERENCE constraint \"FK_GELIHALEONAYFORM_GELIHALETURMADDE\"") > 0)
                        sonuc.hataStr = "İhale Maddesine Ait Onay Bilgisi Kaydı Bulunmaktadır Silinemez...";
                    else
                        sonuc.hataStr = ve.hata;
                }
            }
            else if (bilgi.tur == (int)ENUMModulTuru.Harcama)
            {

                ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                TABHRCIhaleTurMadde tabIHLTurMadde = new TABHRCIhaleTurMadde(ve);
                ve.BeginTransaction();
                tabIHLTurMadde.Sil(bilgi);
                if (ve.hata.Trim() == "")
                {
                    sonuc.islemSonuc = true;
                    sonuc.hataStr = "";
                    sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00042;
                    ve.Commit();
                }
                else
                {
                    ve.Rollback();
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ve.hata;
                }
            }
            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}
