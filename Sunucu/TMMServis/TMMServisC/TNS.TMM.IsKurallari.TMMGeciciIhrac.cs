using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    public class TMMGeciciIhrac
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciIhrac to = new TABGeciciIhrac(ve);

            sonuc = Kontrol(ve, kullanan, tf);
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();

            foreach (TasinirIslemDetay detay in tf.detay.objeler)
            {
                GeciciIhrac form = new GeciciIhrac
                {
                    prSicilNo = detay.sicilNo,
                    yil = detay.yil,
                    muhasebeKod = detay.muhasebeKod,
                    harcamaKod = detay.harcamaKod.Replace(".", ""),
                    fisNo = detay.fisNo
                };

                sonuc = to.Kaydet(form);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TasinirMalResI.TMM118;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        private static Sonuc Kontrol(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGeciciIhrac to = new TABGeciciIhrac(ve);

            foreach (TasinirIslemDetay detay in tf.detay.objeler)
            {
                bool kayitVarMi = to.GeciciIhracKaydiVarMi(detay.sicilNo);
                if (kayitVarMi)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr += detay.sicilNo + ", ";
                }
            }
            if (!sonuc.islemSonuc)
                sonuc.hataStr += " nolu sicil için daha önce \"Geçici İhraç\" işlemi yapılmış.<br>";

            ve.VeriErisimKapat();

            return sonuc;
        }

    }
}
