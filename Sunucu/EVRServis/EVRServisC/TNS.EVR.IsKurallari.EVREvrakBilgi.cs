using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVREvrakBilgi
    {
        public static EvrakBilgi[] Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakBilgi form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakBilgi tab = new TABEvrakBilgi(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new EvrakBilgi();
                    form.prEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakBilgi.ENUMAlanlar.PREVRAKNO, "");
                    form.kullaniciId = VeriErisim.DegerAl(dr, (int)TABEvrakBilgi.ENUMAlanlar.KULLANICIID, "");
                    form.aciklama = VeriErisim.DegerAl(dr, (int)TABEvrakBilgi.ENUMAlanlar.ACIKLAMA, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (EvrakBilgi[])liste.objeler.ToArray(typeof(EvrakBilgi));
        }

        public static EvrakBilgi[] Listele(VeriErisim veriErisim, Kullanici kullanan, string[] prEvrakNoListe)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakBilgi tab = new TABEvrakBilgi(ve);

            IDataReader dr = tab.Listele(prEvrakNoListe);
            if (dr != null)
            {
                while (dr.Read())
                {
                    EvrakBilgi form = new EvrakBilgi();
                    form.prEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakBilgi.ENUMAlanlar.PREVRAKNO, "");
                    form.kullaniciId = VeriErisim.DegerAl(dr, (int)TABEvrakBilgi.ENUMAlanlar.KULLANICIID, "");
                    form.aciklama = VeriErisim.DegerAl(dr, (int)TABEvrakBilgi.ENUMAlanlar.ACIKLAMA, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (EvrakBilgi[])liste.objeler.ToArray(typeof(EvrakBilgi));
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakBilgi form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakBilgi tab = new TABEvrakBilgi(ve);

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
                OrtakFonksiyonlar.HataStrYaz("{987B546E-6048-42A8-9684-10E24C6DD47A} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, EvrakBilgi form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(form.prEvrakNo))
                hata = "Silinecek veri bulunamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakBilgi tab = new TABEvrakBilgi(ve);

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
                OrtakFonksiyonlar.HataStrYaz("{EE4FD3B3-4070-4F31-90EF-BAED0603BAF0} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static EvrakBilgi Oku(VeriErisim veriErisim, Kullanici kullanan, EvrakBilgi form)
        {
            if (string.IsNullOrEmpty(form.prEvrakNo)) return new EvrakBilgi();

            EvrakBilgi[] list = EVREvrakBilgi.Listele(veriErisim, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            return form;
        }

    }
}
