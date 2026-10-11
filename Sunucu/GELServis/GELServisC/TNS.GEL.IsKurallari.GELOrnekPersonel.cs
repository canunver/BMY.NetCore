using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.KYM;
using TNS.GEL.IsKurallari.Veri;

namespace TNS.GEL.IsKurallari
{
    public class GELOrnekPersonel
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, OrnekPersonel bilgi)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            if (string.IsNullOrEmpty(bilgi.ad))
                hatalar.Append("Personel adı boş olmamalıdır");
            if (string.IsNullOrEmpty(bilgi.soyad))
                hatalar.Append("Personel soyadı boş olmamalıdır");
            if (hatalar.ToString() != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
            }
            else
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
                ve.BeginTransaction();
                TABOrnekPersonel tabOrnPer = new TABOrnekPersonel(ve);
                if (bilgi.id==0)
                {
                    bilgi.id = tabOrnPer.MaxIdGetir();
                }
                sonuc = tabOrnPer.Kaydet(bilgi);
                if (sonuc.islemSonuc == true)
                {
                    ve.Commit();
                    sonuc.bilgiStr = "Veri Kaydedildi";
                }
                else
                {
                    ve.Rollback();
                }
                ve.VeriErisimKapat();

            }
            return sonuc;
        }
    }
}
