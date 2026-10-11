using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVREvrakVersiyon2
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, EvrakVersiyon2 form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrakVersiyon2 tab = new TABEvrakVersiyon2(ve);

                sonuc = tab.Kaydet(form);
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{F02905FB-A611-4D44-883D-8E3C6B4969F2} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static EvrakVersiyon2[] Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakVersiyon2 form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakVersiyon2 tab = new TABEvrakVersiyon2(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new EvrakVersiyon2();
                    form.evrEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon2.ENUMAlanlar.EVREVRAKNO, "");
                    form.siraNo = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon2.ENUMAlanlar.SIRANO, 0);
                    form.guid = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon2.ENUMAlanlar.GUID, "");
                    form.islemTarih = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon2.ENUMAlanlar.ISLEMTARIH);
                    form.islemYapan = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon2.ENUMAlanlar.ISLEMYAPAN, "");
                    form.islemYapanAd = VeriErisim.DegerAl(dr, (int)TABEvrakVersiyon2.ENUMAlanlar.ISLEMYAPANAD, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (EvrakVersiyon2[])liste.objeler.ToArray(typeof(EvrakVersiyon2)); ;
        }

        public static EvrakVersiyon2 Oku(VeriErisim veriErisim, Kullanici kullanan, EvrakVersiyon2 form)
        {
            EvrakVersiyon2[] list = EVREvrakVersiyon2.Listele(veriErisim, kullanan, form);
            if (list != null && list.Length == 1)
                form = list[0];
            return form;
        }

    }
}
