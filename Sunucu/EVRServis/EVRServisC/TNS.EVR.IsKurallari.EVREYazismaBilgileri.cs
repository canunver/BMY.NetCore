using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;
using System.Collections;

namespace TNS.EVR.IsKurallari
{
    public class EVREYazismaBilgi
    {

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EYazismaBilgi form)
        {
            if (form == null ||
                form.servisTur <= 0 || string.IsNullOrWhiteSpace(form.kullaniciAd) || string.IsNullOrWhiteSpace(form.parola))
                return new Sonuc(false, "Lütfen kayıt bilgilerini kontrol ediniz", "");

            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEYazismaBilgileri tab = new TABEYazismaBilgileri(ve);
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
                OrtakFonksiyonlar.HataStrYaz("{C7218985-CBEB-4C11-B8A1-D1D5CC47A766} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, int eYazismaServisTur)
        {
            if (eYazismaServisTur <= 0)
                return new Sonuc(false, "Silinecek veri bulunamadı.", "");

            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEYazismaBilgileri tab = new TABEYazismaBilgileri(ve);
            ve.BeginTransaction();
            try
            {
                sonuc = tab.Sil(eYazismaServisTur);
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
                OrtakFonksiyonlar.HataStrYaz("{3A2B46B1-21F7-40AE-87F4-C89C04F97C8B} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static EYazismaBilgi Oku(VeriErisim veriErisim, EYazismaBilgi form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEYazismaBilgileri tab = new TABEYazismaBilgileri(ve);

            IDataReader dr = tab.Oku(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new EYazismaBilgi();
                    form.servisTur = VeriErisim.DegerAl(dr, (int)TABEYazismaBilgileri.ENUMAlanlar.SERVISTUR, 0);
                    form.servisIdariKimlikKod = VeriErisim.DegerAl(dr, (int)TABEYazismaBilgileri.ENUMAlanlar.SERVISIDARIKIMLIKKOD, "");
                    form.servisKurumAd = VeriErisim.DegerAl(dr, (int)TABEYazismaBilgileri.ENUMAlanlar.SERVISKURUMAD, "");
                    form.kullaniciAd = VeriErisim.DegerAl(dr, (int)TABEYazismaBilgileri.ENUMAlanlar.KULLANICIAD, "");
                    form.parola = VeriErisim.DegerAl(dr, (int)TABEYazismaBilgileri.ENUMAlanlar.PAROLA, "");
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return form;
        }

        public static EYazismaBilgi[] Listele(VeriErisim veriErisim)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEYazismaBilgileri tab = new TABEYazismaBilgileri(ve);

            IDataReader dr = tab.Listele();
            if (dr != null)
            {
                while (dr.Read())
                {
                    EYazismaBilgi form = new EYazismaBilgi();
                    form.servisTur = VeriErisim.DegerAl(dr, (int)TABEYazismaBilgileri.ENUMAlanlar.SERVISTUR, 0);
                    form.servisIdariKimlikKod = VeriErisim.DegerAl(dr, (int)TABEYazismaBilgileri.ENUMAlanlar.SERVISIDARIKIMLIKKOD, "");
                    form.servisKurumAd = VeriErisim.DegerAl(dr, (int)TABEYazismaBilgileri.ENUMAlanlar.SERVISKURUMAD, "");
                    form.kullaniciAd = VeriErisim.DegerAl(dr, (int)TABEYazismaBilgileri.ENUMAlanlar.KULLANICIAD, "");
                    form.parola = VeriErisim.DegerAl(dr, (int)TABEYazismaBilgileri.ENUMAlanlar.PAROLA, "");
                  
                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (EYazismaBilgi[])liste.objeler.ToArray(typeof(EYazismaBilgi));
        }
    }
}
