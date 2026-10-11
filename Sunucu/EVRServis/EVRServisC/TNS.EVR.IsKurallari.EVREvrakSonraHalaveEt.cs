using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;
using EVRServisC;
using System.Collections;

namespace TNS.EVR.IsKurallari
{
    public class EVREvrakSonraHavaleEt
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakSonraHavaleEt form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakSonraHavaleEt tab = new TABEvrakSonraHavaleEt(ve);

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
                OrtakFonksiyonlar.HataStrYaz("{9598655A-E475-4A69-B920-CF1EE6BF766B} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc GonderildiOlarakIsaretle(VeriErisim veriErisim, Kullanici kullanan, string prEvrakNo)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            if (string.IsNullOrEmpty(prEvrakNo))
            {
                sonuc.hataStr = "prEvrakNo boş bırakılamaz.";
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakSonraHavaleEt tab = new TABEvrakSonraHavaleEt(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.GonderildiOlarakIsaretle(prEvrakNo);

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
                OrtakFonksiyonlar.HataStrYaz("{A2E2CFD4-0233-45B8-A112-695B1EDB9B03} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static EvrakSonraHavaleEt[] Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakSonraHavaleEt form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakSonraHavaleEt tab = new TABEvrakSonraHavaleEt(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new EvrakSonraHavaleEt();
                    form.prEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakSonraHavaleEt.ENUMAlanlar.PREVRAKNO, "");
                    form.siraNo = VeriErisim.DegerAl(dr, (int)TABEvrakSonraHavaleEt.ENUMAlanlar.SIRANO, 0);
                    form.evrakBirimKod = VeriErisim.DegerAl(dr, (int)TABEvrakSonraHavaleEt.ENUMAlanlar.EVRAKBIRIMKOD, "");
                    form.tcKimlikNo = VeriErisim.DegerAl(dr, (int)TABEvrakSonraHavaleEt.ENUMAlanlar.TCKIMLIKNO, "");
                    form.havaleNot = VeriErisim.DegerAl(dr, (int)TABEvrakSonraHavaleEt.ENUMAlanlar.HAVALENOT, "");
                    form.gonderildi = VeriErisim.DegerAl(dr, (int)TABEvrakSonraHavaleEt.ENUMAlanlar.GONDERILDI, 0);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (EvrakSonraHavaleEt[])liste.objeler.ToArray(typeof(EvrakSonraHavaleEt));
        }
    }
}
