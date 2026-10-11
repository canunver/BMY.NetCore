using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVRHavaleFavori
    {

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, HavaleFavori form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHavaleFavori tab = new TABHavaleFavori(ve);
            TABGenel tabGenel = new TABGenel(ve);

            ve.BeginTransaction();
            try
            {
                if (form.kod == 0)
                {
                    string sayacKod = "EVRHAVALELISTE";
                    form.kod = OrtakFonksiyonlar.ConvertToInt(tabGenel.BelgeNoAl(sayacKod, "0000000"), 0);

                    sonuc = tab.KayitVarsaSil(form.islemYapan, form.havaleBirimKod, form.havalePersonelMernis);
                    sonuc = tab.Kaydet(form);
                    if (!sonuc.islemSonuc && sonuc.hataStr.IndexOf("duplicate key") > -1)
                    {
                        ve.hata = "";
                        form.kod = tab.KodAl();
                        tabGenel.BelgeNoYaz(sayacKod, form.kod.ToString());
                        sonuc = tab.Kaydet(form);
                    }
                }
                else
                    sonuc = tab.Guncelle(form);

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
                OrtakFonksiyonlar.HataStrYaz("{CE69DE03-FCDF-4068-B346-76EBB267AB56} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, HavaleFavori[] list)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHavaleFavori tab = new TABHavaleFavori(ve);

            ve.BeginTransaction();
            try
            {
                foreach (var item in list)
                {
                    sonuc = EVRHavaleFavori.Kaydet(veriErisim, kullanan, item);
                    if (!sonuc.islemSonuc)
                        break;
                }

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
                OrtakFonksiyonlar.HataStrYaz("{4A7DAD75-D3F2-4428-83C8-10CF4322218F} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, string[] kodList)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = string.Empty;
            if (kodList == null || kodList.Length == 0) { hata = "1" + "<br>"; }

            if (string.IsNullOrEmpty(hata))
                foreach (var kod in kodList)
                    if (OrtakFonksiyonlar.ConvertToInt(kod, 0) <= 0) { hata = "1"; break; }

            if (hata == "1")
                hata = "Silinecek veri bulunamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHavaleFavori tab = new TABHavaleFavori(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.Sil(kodList);
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
                OrtakFonksiyonlar.HataStrYaz("{2E95C289-30A4-4025-A6A7-01634B264D26} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static HavaleFavori[] Listele(VeriErisim veriErisim, Kullanici kullanan, HavaleFavori form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABHavaleFavori tab = new TABHavaleFavori(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new HavaleFavori();
                    form.kod = VeriErisim.DegerAl(dr, (int)TABHavaleFavori.ENUMAlanlar.KOD, 0);
                    form.islemYapan = VeriErisim.DegerAl(dr, (int)TABHavaleFavori.ENUMAlanlar.ISLEMYAPAN, "");
                    form.siraNo = VeriErisim.DegerAl(dr, (int)TABHavaleFavori.ENUMAlanlar.SIRANO, 0);
                    form.havaleBirimKod = VeriErisim.DegerAl(dr, (int)TABHavaleFavori.ENUMAlanlar.HAVALEBIRIMKOD, "");
                    form.havalePersonelMernis = VeriErisim.DegerAl(dr, (int)TABHavaleFavori.ENUMAlanlar.HAVALEPERMERNIS, "");
                    form.havalePersonelAd = VeriErisim.DegerAl(dr, (int)TABHavaleFavori.ENUMAlanlar.HAVALEPERAD, "");
                    form.havalePersonelSoyad = VeriErisim.DegerAl(dr, (int)TABHavaleFavori.ENUMAlanlar.HAVALEPERSOYAD, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            HavaleFavori[] arr = null;
            if (liste.objeler.Count > 0)
                arr = (HavaleFavori[])liste.objeler.ToArray(typeof(HavaleFavori));

            return arr;
        }

        public static HavaleFavori Oku(VeriErisim veriErisim, Kullanici kullanan, HavaleFavori form)
        {
            if (form.kod == 0) return null;

            VeriErisim ve1 = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            HavaleFavori[] list = EVRHavaleFavori.Listele(ve1, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            ve1.VeriErisimKapat();
            return form;
        }
    }
}
