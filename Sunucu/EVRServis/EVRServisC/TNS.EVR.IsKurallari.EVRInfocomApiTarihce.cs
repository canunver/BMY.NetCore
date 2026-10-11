using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVRInfocomApiTarihce
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, InfocomApiTarihce form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABInfocomApiTarihce tab = new TABInfocomApiTarihce(ve);

            ve.BeginTransaction();
            try
            {
                if (string.IsNullOrWhiteSpace(form.kod))
                {
                    form.kod = Guid.NewGuid().ToString("N");
                    sonuc = tab.Kaydet(form);
                }
                else if (!string.IsNullOrWhiteSpace(form.cevap))
                    sonuc = tab.CevapKaydet(form);

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
                OrtakFonksiyonlar.HataStrYaz("{1C1A063F-B573-468A-B01B-865B008BA845} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static InfocomApiTarihce[] Listele(VeriErisim veriErisim, Kullanici kullanan, InfocomApiTarihce form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABInfocomApiTarihce tab = new TABInfocomApiTarihce(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    InfocomApiTarihce o = new InfocomApiTarihce();
                    o.kod = VeriErisim.DegerAl(dr, (int)TABInfocomApiTarihce.ENUMAlanlar.KOD, "");
                    o.islemYapan = VeriErisim.DegerAl(dr, (int)TABInfocomApiTarihce.ENUMAlanlar.ISLEMYAPAN, "");
                    o.islemYapanAd = VeriErisim.DegerAl(dr, (int)TABInfocomApiTarihce.ENUMAlanlar.ISLEMYAPANAD, "");
                    o.islemTarih = VeriErisim.DegerAl(dr, (int)TABInfocomApiTarihce.ENUMAlanlar.ISLEMTARIH);
                    o.personIdnp = VeriErisim.DegerAl(dr, (int)TABInfocomApiTarihce.ENUMAlanlar.PERSONIDNP, "");
                    o.organizationInn = VeriErisim.DegerAl(dr, (int)TABInfocomApiTarihce.ENUMAlanlar.ORGANIZATIONINN, "");
                    o.servis = VeriErisim.DegerAl(dr, (int)TABInfocomApiTarihce.ENUMAlanlar.SERVIS, "");
                    o.talep = VeriErisim.DegerAl(dr, (int)TABInfocomApiTarihce.ENUMAlanlar.TALEP, "");
                    o.cevap = VeriErisim.DegerAl(dr, (int)TABInfocomApiTarihce.ENUMAlanlar.CEVAP, "");

                    liste.objeler.Add(o);
                }
                dr.Close();
            }


            ve.VeriErisimKapat();

            InfocomApiTarihce[] arr = null;
            if (liste.objeler.Count > 0)
                arr = (InfocomApiTarihce[])liste.objeler.ToArray(typeof(InfocomApiTarihce));

            return arr;
        }

    }
}
