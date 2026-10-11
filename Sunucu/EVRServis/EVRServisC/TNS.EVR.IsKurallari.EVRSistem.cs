using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVRSistem
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, string digerUstVeriler)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABSistem tab = new TABSistem(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.Kaydet(digerUstVeriler);
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{1B644080-0C0F-4002-B666-6EF5B8B4760E} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static string Oku(VeriErisim veriErisim)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABSistem tab = new TABSistem(ve);

            string digerUstVeriler = tab.Oku();

            ve.VeriErisimKapat();

            return digerUstVeriler;
        }

    }
}
