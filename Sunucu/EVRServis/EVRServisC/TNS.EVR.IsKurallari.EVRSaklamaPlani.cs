using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System.Collections;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVRSaklamaPlani
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, SaklamaPlani form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.ad))
                hata += "Saklama plan adı boş bırakılamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABSaklamaPlani tab = new TABSaklamaPlani(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = (form.kod == 0) ? tab.Kaydet(form) : tab.Guncelle(form);

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.anahtar = form.kod.ToString();
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{5C5E245C-D1C5-4BC8-B096-84D2519F0939} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, SaklamaPlani form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = string.Empty;
            if (form.kod <= 0)
                hata = "Silinecek veri bulunamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABSaklamaPlani tab = new TABSaklamaPlani(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.Sil(form);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = EvrakI.EVRI097;
                }
                else
                {
                    ve.Rollback();
                    sonuc.hataStr = EvrakI.EVRI098 + "<br>" + sonuc.hataStr;
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{8360ED6B-D4DC-40E2-9F07-0DBCD8551FE5} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
        public static SaklamaPlani[] Listele(VeriErisim veriErisim, Kullanici kullanan, SaklamaPlani form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABSaklamaPlani tab = new TABSaklamaPlani(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new SaklamaPlani();
                    form.kod = VeriErisim.DegerAl(dr, (int)TABSaklamaPlani.ENUMAlanlar.KOD, 0);
                    form.ad = VeriErisim.DegerAl(dr, (int)TABSaklamaPlani.ENUMAlanlar.AD, "");
                    form.sure = VeriErisim.DegerAl(dr, (int)TABSaklamaPlani.ENUMAlanlar.SURE, 0);
                    form.kriterKod = VeriErisim.DegerAl(dr, (int)TABSaklamaPlani.ENUMAlanlar.KRITERKOD, 0);
                    form.baslangicKod = VeriErisim.DegerAl(dr, (int)TABSaklamaPlani.ENUMAlanlar.BASLANGICKOD, 0);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            SaklamaPlani[] arr = (SaklamaPlani[])liste.objeler.ToArray(typeof(SaklamaPlani));

            return arr;
        }

        public static SaklamaPlani Oku(VeriErisim veriErisim, Kullanici kullanan, SaklamaPlani form)
        {
            if (form.kod == 0) return null;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            SaklamaPlani[] list = Listele(ve, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            ve.VeriErisimKapat();

            return form;
        }
    }
}
