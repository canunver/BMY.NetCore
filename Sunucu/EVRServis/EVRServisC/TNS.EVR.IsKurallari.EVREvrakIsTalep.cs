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
    public class EVREvrakIsTalep
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakIsTalep form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (form.kod == 0)
            {
                if (form.prEvrakNo == "")
                    hata += "Evrak bilgisi boş." + "<br>";
                if (string.IsNullOrEmpty(form.islemYapan))
                    hata += "İşlem yapan kişi bilgisi boş." + "<br>";
                if (string.IsNullOrEmpty(form.atananKisiAd))
                    hata += "Atama yapılan kişi bilgisi boş." + "<br>";
            }
            if (form.islem == ENUMEvrakIsTalepIslem.TANIMSIZ)
                hata += "Kayıt işlem durumu tanımsız." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakIsTalep tab = new TABEvrakIsTalep(ve);

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
                Arac.HataStrYaz("{4F2AA8BF-FAC7-4CDD-9D14-72F8C9F52F13} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, EvrakIsTalep form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = string.Empty;
            if (form.kod <= 0 || form.servisKayitKod == "")
                hata = "Silinecek veri bulunamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakIsTalep tab = new TABEvrakIsTalep(ve);

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
                Arac.HataStrYaz("{3707B3CF-DE7C-4805-AB94-1894B6397674} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static EvrakIsTalep[] Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakIsTalep form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakIsTalep tab = new TABEvrakIsTalep(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new EvrakIsTalep();
                    form.kod = VeriErisim.DegerAl(dr, (int)TABEvrakIsTalep.ENUMAlanlar.KOD, 0);
                    form.prEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakIsTalep.ENUMAlanlar.PREVRAKNO, "");
                    form.servisKayitKod = VeriErisim.DegerAl(dr, (int)TABEvrakIsTalep.ENUMAlanlar.SERVISKAYITKOD, "");
                    form.atananKisi = VeriErisim.DegerAl(dr, (int)TABEvrakIsTalep.ENUMAlanlar.ATANANKISI, "");
                    form.atananKisiAd = VeriErisim.DegerAl(dr, (int)TABEvrakIsTalep.ENUMAlanlar.ATANANKISIAD, "");
                    form.islemYapan = VeriErisim.DegerAl(dr, (int)TABEvrakIsTalep.ENUMAlanlar.ISLEMYAPAN, "");
                    form.islemYapanAd = VeriErisim.DegerAl(dr, (int)TABEvrakIsTalep.ENUMAlanlar.ISLEMYAPANAD, "");
                    form.islemTarihi = VeriErisim.DegerAl(dr, (int)TABEvrakIsTalep.ENUMAlanlar.ISLEMTARIH);
                    form.islem = (ENUMEvrakIsTalepIslem)VeriErisim.DegerAl(dr, (int)TABEvrakIsTalep.ENUMAlanlar.ISLEM, 0);
                    form.islemAciklama = VeriErisim.DegerAl(dr, (int)TABEvrakIsTalep.ENUMAlanlar.ISLEMACIKLAMA, "");
                    form.detay = VeriErisim.DegerAl(dr, (int)TABEvrakIsTalep.ENUMAlanlar.DETAY, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            EvrakIsTalep[] arr = null;
            if (liste.objeler.Count > 0)
                arr = (EvrakIsTalep[])liste.objeler.ToArray(typeof(EvrakIsTalep));

            return arr;
        }

        public static EvrakIsTalep Oku(VeriErisim veriErisim, Kullanici kullanan, EvrakIsTalep form)
        {
            if (form.kod == 0) return null;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            EvrakIsTalep[] list = Listele(ve, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            ve.VeriErisimKapat();

            return form;
        }

    }
}
