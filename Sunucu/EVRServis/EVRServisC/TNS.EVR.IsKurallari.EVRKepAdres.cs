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
    public class EVRKepAdres
    {

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, KepAdres form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (form.adres == "")
                hata += "Kep adres bilgisi boş." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABKepAdres tab = new TABKepAdres(ve);

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
                OrtakFonksiyonlar.HataStrYaz("{DE1FB555-1363-4CEE-B5EC-97EA0FE6F300} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, KepAdres form)
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
            TABKepAdres tab = new TABKepAdres(ve);

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
                OrtakFonksiyonlar.HataStrYaz("{7D581DCB-826D-4AD4-B140-FAC4204E8F1B} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static KepAdres[] Listele(VeriErisim veriErisim, Kullanici kullanan, KepAdres form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABKepAdres tab = new TABKepAdres(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new KepAdres();
                    form.kod = VeriErisim.DegerAl(dr, (int)TABKepAdres.ENUMAlanlar.KOD, 0);
                    form.evrakBirimKod = VeriErisim.DegerAl(dr, (int)TABKepAdres.ENUMAlanlar.EVRAKBIRIMKODU, "");
                    form.adres = VeriErisim.DegerAl(dr, (int)TABKepAdres.ENUMAlanlar.ADRES, "");
                    form.tip = (ENUMKepAdresTip)VeriErisim.DegerAl(dr, (int)TABKepAdres.ENUMAlanlar.TIP, 0);
                    form.kullanimSayisi = VeriErisim.DegerAl(dr, (int)TABKepAdres.ENUMAlanlar.KULLANIMSAYISI, 0);
                    form.kalici = VeriErisim.DegerAl(dr, (int)TABKepAdres.ENUMAlanlar.KALICI, 0);
                    form.islemTarihi = VeriErisim.DegerAl(dr, (int)TABKepAdres.ENUMAlanlar.ISLEMTARIHI);
                    form.islemYapan = VeriErisim.DegerAl(dr, (int)TABKepAdres.ENUMAlanlar.ISLEMYAPAN, "");
                    form.islemYapanAd = VeriErisim.DegerAl(dr, (int)TABKepAdres.ENUMAlanlar.ISLEMYAPANAD, "");
                    form.iptal = VeriErisim.DegerAl(dr, (int)TABKepAdres.ENUMAlanlar.IPTAL, 0);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            KepAdres[] arr = null;
            if (liste.objeler.Count > 0)
                arr = (KepAdres[])liste.objeler.ToArray(typeof(KepAdres));

            return arr;
        }

        public static KepAdres Oku(VeriErisim veriErisim, Kullanici kullanan, KepAdres form)
        {
            if (form.kod == 0) return null;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            KepAdres[] list = Listele(ve, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            ve.VeriErisimKapat();

            return form;
        }

    }
}
