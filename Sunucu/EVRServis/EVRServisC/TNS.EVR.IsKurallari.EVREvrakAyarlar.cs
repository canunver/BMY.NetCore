using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVREvrakAyarlar
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, EvrakAyarlar form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.anahtarSahibi))
                hata += "Anahtar sahibi boş bırakılamaz." + "<br>";
            if (form.anahtar <= 0)
                hata += "Anahtar boş bırakılamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakAyarlar tab = new TABEvrakAyarlar(ve);
            ve.BeginTransaction();
            try
            {
                sonuc = tab.Kaydet(form);

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{D36D4087-4B53-4020-A673-80FDF2E6584D} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();

            return sonuc;
        }
        public static EvrakAyarlar Oku(VeriErisim veriErisim, string anahtarSahibi, ENUMEvrakAyarlarAnahtar anahtar)
        {
            EvrakAyarlar returnForm = new EvrakAyarlar();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakAyarlar tab = new TABEvrakAyarlar(ve);
            IDataReader dr = tab.Oku(anahtarSahibi, (int)anahtar);
            if (dr != null)
            {
                while (dr.Read())
                {
                    returnForm.anahtarSahibi = VeriErisim.DegerAl(dr, (int)TABEvrakAyarlar.ENUMAlanlar.ANAHTARSAHIBI, "");
                    returnForm.anahtar = VeriErisim.DegerAl(dr, (int)TABEvrakAyarlar.ENUMAlanlar.ANAHTAR, 0);
                    returnForm.deger = VeriErisim.DegerAl(dr, (int)TABEvrakAyarlar.ENUMAlanlar.DEGER, "");
                    returnForm.deger2 = VeriErisim.DegerAl(dr, (int)TABEvrakAyarlar.ENUMAlanlar.DEGER2, "");
                    break;
                }
                dr.Close();
            }
            ve.VeriErisimKapat();

            return returnForm;
        }

        public static EvrakAyarlar[] Listele(VeriErisim veriErisim, params EvrakAyarlar[] kriter)
        {
            ObjectArray liste = new ObjectArray();

            if (kriter != null)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
                TABEvrakAyarlar tab = new TABEvrakAyarlar(ve);
                IDataReader dr = tab.Listele(kriter);

                if (dr != null)
                {
                    while (dr.Read())
                    {
                        EvrakAyarlar form = new EvrakAyarlar();
                        form.anahtarSahibi = VeriErisim.DegerAl(dr, (int)TABEvrakAyarlar.ENUMAlanlar.ANAHTARSAHIBI, "");
                        form.anahtar = VeriErisim.DegerAl(dr, (int)TABEvrakAyarlar.ENUMAlanlar.ANAHTAR, 0);
                        form.deger = VeriErisim.DegerAl(dr, (int)TABEvrakAyarlar.ENUMAlanlar.DEGER, "");
                        form.deger2 = VeriErisim.DegerAl(dr, (int)TABEvrakAyarlar.ENUMAlanlar.DEGER2, "");

                        liste.objeler.Add(form);
                    }
                    dr.Close();
                }
                ve.VeriErisimKapat();
            }
            
            return (EvrakAyarlar[])liste.objeler.ToArray(typeof(EvrakAyarlar));
        }
    }
}
