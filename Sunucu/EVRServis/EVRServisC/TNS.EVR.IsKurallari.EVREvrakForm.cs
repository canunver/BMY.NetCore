using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVREvrakForm
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, string sayfaNo, EvrakForm[] formList)
        {
            if (formList == null || formList.Length == 0)
                return new Sonuc(false, "formList boş veya null", "");

            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakForm tab = new TABEvrakForm(ve);

            ve.BeginTransaction();
            try
            {
                string sayac = "";

                sonuc = tab.Sil(formList[0].evrEvrakNo);
                if (sonuc.islemSonuc)
                {
                    foreach (var form in formList)
                    {
                        if (form.alanAdi.StartsWith("syc"))
                        {
                            if (string.IsNullOrEmpty(form.deger))
                                form.deger = EVREvrakForm.EvrakFormSayacNoAl(ve, sayfaNo);
                            sayac = form.deger;
                        }

                        sonuc = tab.Kaydet(form);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    sonuc.anahtar = sayac;
                    ve.Commit();
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{4C553085-9904-450E-A538-87CC3FFC1ED7} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static EvrakForm[] Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakForm form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakForm tab = new TABEvrakForm(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new EvrakForm();
                    form.evrEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakForm.ENUMAlanlar.EVREVRAKNO, "");
                    form.alanAdi = VeriErisim.DegerAl(dr, (int)TABEvrakForm.ENUMAlanlar.ALANADI, "");
                    form.deger = VeriErisim.DegerAl(dr, (int)TABEvrakForm.ENUMAlanlar.DEGER, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (EvrakForm[])liste.objeler.ToArray(typeof(EvrakForm));
        }

        public static string EvrakFormSayacNoAl(VeriErisim ve, string sayfaKod)
        {
            TABGenel tabGenel = new TABGenel(ve);
            string sayacKod = "EVREVRAKFORM" + DateTime.Now.Year + sayfaKod;
            return tabGenel.BelgeNoAl(sayacKod, "0000000");
        }
    }
}
