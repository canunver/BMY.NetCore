using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.BKD;

namespace TNS.HRC.IsKurallari
{
    public class HRCMPRevizeNoTalepNo
    {
        public Sonuc Kaydet(Kullanici kullanan, MPRevizeNoTalepNo bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMPRevizeNoTalepNo to = new TABMPRevizeNoTalepNo(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc Sil(Kullanici kullanan, MPRevizeNoTalepNo bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (bilgi.yil == 0)
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00211;

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMPRevizeNoTalepNo to = new TABMPRevizeNoTalepNo(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla tamamlandı.<br>";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00019 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public MPRevizeNoTalepNo Listele(Kullanici kullanan, MPRevizeNoTalepNo kriter)
        {
            MPRevizeNoTalepNo f = new MPRevizeNoTalepNo();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABMPRevizeNoTalepNo to = new TABMPRevizeNoTalepNo(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                if (r.Read())
                {
                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.surecYil = VeriErisim.DegerAl(r, 1, 0);
                    f.surecNo = VeriErisim.DegerAl(r, 2, "");
                    f.revizeNo = VeriErisim.DegerAl(r, 3, 0);
                    f.talepNo = VeriErisim.DegerAl(r, 4, 0);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return f;
        }
    }
}
