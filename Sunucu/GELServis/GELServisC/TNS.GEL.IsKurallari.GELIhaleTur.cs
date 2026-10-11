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
    public class GELIhaleTur
    {
        public static ObjectArray Listele(Kullanici kullanan, TNS.GEL.IhaleTur kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            VeriErisim ve = null;


            if (kriter.tur == (int)ENUMModulTuru.Harcama)
            {
                ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC");
                TABHRCIhaleTur itur = new TABHRCIhaleTur(ve);
                IDataReader dr = itur.Listele(kriter);
                if (itur.Hata() == "")
                {
                    while (dr.Read())
                    {
                        IhaleTur it = new IhaleTur();
                        it.kod = VeriErisim.DegerAl(dr, 0, 0);
                        it.adi = VeriErisim.DegerAl(dr, 1, "");
                        bilgi.objeler.Add(it);
                    }
                }
                dr.Close();
            }
            else if (kriter.tur == (int)ENUMModulTuru.Gelir)
            {
                ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriGEL");
                TABIGELhaleTur itur = new TABIGELhaleTur(ve);
                IDataReader dr = itur.Listele(kriter);
                if (itur.Hata() == "")
                {
                    while (dr.Read())
                    {
                        IhaleTur it = new IhaleTur();
                        it.kod = VeriErisim.DegerAl(dr, 0, 0);
                        it.adi = VeriErisim.DegerAl(dr, 1, "");
                        bilgi.objeler.Add(it);
                    }
                }
                dr.Close();
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

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, IhaleTur bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");

            if (bilgi.adi.Trim() == "")
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00035 + "<br/>");

            if (bilgi.tur <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00048 + "<br/>");

            if (hatalar.ToString().Trim() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }

            VeriErisim ve = null;
            if (bilgi.tur == (int)ENUMModulTuru.Gelir)
            {
                ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                TABIGELhaleTur tabITur = new TABIGELhaleTur(ve);
                ve.BeginTransaction();
                int kayitVar = tabITur.KayitVarmi(bilgi);

                if (kayitVar == 0)
                {
                    if (bilgi.kod <= 0)
                        bilgi.kod = tabITur.KayitSiraNoAl();

                    sonuc = tabITur.Kaydet(bilgi);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00036;
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
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00034;
                        return sonuc;
                    }
                    sonuc = tabITur.Guncelle(bilgi);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00036;
                    }
                    else
                    {
                        ve.Rollback();
                    }
                }
                ve.VeriErisimKapat();
            }
            else if (bilgi.tur == (int)ENUMModulTuru.Harcama)
            {
                ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC");
                TABHRCIhaleTur tabITur = new TABHRCIhaleTur(ve);
                ve.BeginTransaction();
                int kayitVar = tabITur.KayitVarmi(bilgi);
                if (kayitVar == 0)
                {
                    if (bilgi.kod <= 0)
                        bilgi.kod = tabITur.KayitSiraNoAl();

                    sonuc = tabITur.Kaydet(bilgi);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00036;
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
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00034;
                        return sonuc;
                    }
                    sonuc = tabITur.Guncelle(bilgi);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00036;
                    }
                    else
                    {
                        ve.Rollback();
                    }
                }
                ve.VeriErisimKapat();
            }
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, IhaleTur bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");

            if (bilgi.kod <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00034 + "<br/>");

            if (bilgi.adi.Trim() == "")
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00035 + "<br/>");

            if (bilgi.tur <= 0)
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00048 + "<br/>");

            if (hatalar.ToString().Trim() != "")
            {

                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }
            VeriErisim ve = null;
            if (bilgi.tur == (int)ENUMModulTuru.Harcama)
            {
                ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC");
                TABHRCIhaleTur tabITur = new TABHRCIhaleTur(ve);
                ve.BeginTransaction();
                tabITur.Sil(bilgi);
                if (ve.hata.Trim() == "")
                {
                    sonuc.islemSonuc = true;
                    sonuc.hataStr = "";
                    sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00037;
                    ve.Commit();
                }
                else
                {
                    ve.Rollback();
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ve.hata;
                }
            }
            else if (bilgi.tur == (int)ENUMModulTuru.Gelir)
            {
                ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                TABIGELhaleTur tabITur = new TABIGELhaleTur(ve);
                ve.BeginTransaction();
                tabITur.Sil(bilgi);
                if (ve.hata.Trim() == "")
                {
                    sonuc.islemSonuc = true;
                    sonuc.hataStr = "";
                    sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00037;
                    ve.Commit();
                }
                else
                {
                    ve.Rollback();
                    sonuc.islemSonuc = false;
                    if (ve.hata.Trim().IndexOf("(0x80131904): The DELETE statement conflicted with the REFERENCE constraint \"FK_GELIHALETURMADDE_GELIHALETUR\"") > 0)
                        sonuc.hataStr = "İhale Türüne Ait Madde Kaydı Bulunmaktadır Silinemez...";
                    else if (ve.hata.Trim().IndexOf("(0x80131904): The DELETE statement conflicted with the REFERENCE constraint \"FK_GELIHALEILANTANIM_GELIHALETUR\"") > 0)
                        sonuc.hataStr = "İhale Türüne Ait İlan Şekli Kaydı Bulunmaktadır Silinemez...";
                    else if (ve.hata.Trim().IndexOf("(0x80131904): The DELETE statement conflicted with the REFERENCE constraint \"FK_GELIHALEONAYFORM_GELIHALETUR\"") > 0)
                        sonuc.hataStr = "İhale Türüne Ait Onay Bilgisi Kaydı Bulunmaktadır Silinemez...";
                    else
                        sonuc.hataStr = ve.hata;
                }
            }
            ve.VeriErisimKapat();
            return sonuc;

        }
    }
}
