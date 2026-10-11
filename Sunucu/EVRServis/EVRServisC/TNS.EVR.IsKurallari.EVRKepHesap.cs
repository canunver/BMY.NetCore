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
    public class EVRKepHesap
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, KepHesap form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (form.hesapTur == 0)
                hata += "Kep hesap tür bilgisi boş bırakılamaz." + "<br>";
            if (string.IsNullOrEmpty(form.hesap))
                hata += "Kep hesap adı boş bırakılamaz." + "<br>";
            if (string.IsNullOrEmpty(form.hesapSahibi))
                hata += "Kep hesap sahibi bilgisi boş bırakılamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABKepHesap tab = new TABKepHesap(ve);

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
                OrtakFonksiyonlar.HataStrYaz("{239254A4-5471-4E26-B4E2-73557EF85DE5} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, KepHesap form)
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
            TABKepHesap tab = new TABKepHesap(ve);

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
                OrtakFonksiyonlar.HataStrYaz("{9737DC7D-10F6-4DD5-AB93-AABDF71D2AD0} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static KepHesap[] Listele(VeriErisim veriErisim, Kullanici kullanan, KepHesap form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABKepHesap tab = new TABKepHesap(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new KepHesap();
                    form.kod = VeriErisim.DegerAl(dr, (int)TABKepHesap.ENUMAlanlar.KOD, 0);
                    form.hesapTur = VeriErisim.DegerAl(dr, (int)TABKepHesap.ENUMAlanlar.HESAPTUR, 0);
                    form.hesap = VeriErisim.DegerAl(dr, (int)TABKepHesap.ENUMAlanlar.HESAP, "");
                    form.hesapSahibi = VeriErisim.DegerAl(dr, (int)TABKepHesap.ENUMAlanlar.HESAPSAHIBI, "");
                    form.varsayilan = VeriErisim.DegerAl(dr, (int)TABKepHesap.ENUMAlanlar.VARSAYILAN, 0);
                    form.islemYapan = VeriErisim.DegerAl(dr, (int)TABKepHesap.ENUMAlanlar.ISLEMYAPAN, "");
                    form.islemYapanAd = VeriErisim.DegerAl(dr, (int)TABKepHesap.ENUMAlanlar.ISLEMYAPANAD, "");
                    form.anahtar = VeriErisim.DegerAl(dr, (int)TABKepHesap.ENUMAlanlar.ANAHTAR, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            KepHesap[] arr = null;
            if (liste.objeler.Count > 0)
                arr = (KepHesap[])liste.objeler.ToArray(typeof(KepHesap));

            return arr;
        }

        public static KepHesap Oku(VeriErisim veriErisim, Kullanici kullanan, KepHesap form)
        {
            if (form.kod == 0) return null;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            KepHesap[] list = Listele(ve, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            ve.VeriErisimKapat();

            return form;
        }

        public static Sonuc VarsayilanYap(VeriErisim veriErisim, Kullanici kullanan, KepHesap form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABKepHesap tab = new TABKepHesap(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.VarsayilanYap(form);
                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.bilgiStr = "Hesap varsayılan olarak seçildi.";
                    ve.Commit();
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{A94FAE9D-8DC8-4D2D-A4D3-370BAD64890C} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}
