using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.KYM;
using TNS.GEL.IsKurallari.Veri;

namespace TNS.GEL.IsKurallari
{
    public class GELOnayFormTertip
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, OnayFormTertip bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00003;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABOnayFormTertip tabOFTertip = new TABOnayFormTertip(ve);
            ve.BeginTransaction();
            tabOFTertip.TertipSil(bilgi);
            if (ve.hata.Trim() == "")
            {
                sonuc = tabOFTertip.TertipKaydet(bilgi);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.anahtar = bilgi.surecNo.Trim();
                    sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00025;
                }
                else
                {
                    ve.Rollback();
                }
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ve.hata.Trim();
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;         
        }
    }
}
