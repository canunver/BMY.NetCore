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
    public class EVRBirimHavaleYetki
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, BirimHavaleYetki[] formList, string evrakBirimKod)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (string.IsNullOrWhiteSpace(evrakBirimKod))
                hata += "Evrak birim bilgisi boş." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABBirimHavaleYetki tab = new TABBirimHavaleYetki(ve);
            ve.BeginTransaction();

            sonuc = tab.Sil(evrakBirimKod);
            if (sonuc.islemSonuc)
            {
                foreach (var form in formList)
                {
                    sonuc = tab.Kaydet(form);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = EvrakI.EVRI094;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static BirimHavaleYetki[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHavaleYetki form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABBirimHavaleYetki tab = new TABBirimHavaleYetki(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                int index = (int)TABBirimHavaleYetki.ENUMAlanlar.SIRANO;
                while (dr.Read())
                {
                    form = new BirimHavaleYetki();
                    form.evrakBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimHavaleYetki.ENUMAlanlar.EVRAKBIRIMKOD, "");
                    form.havaleBirimi.kod = VeriErisim.DegerAl(dr, (int)TABBirimHavaleYetki.ENUMAlanlar.HAVALEBIRIMKOD, "");
                    form.siraNo = VeriErisim.DegerAl(dr, (int)TABBirimHavaleYetki.ENUMAlanlar.SIRANO, 0);

                    form.havaleBirimi.ad = VeriErisim.DegerAl(dr, index + 1, "");
                    form.havaleBirimi.idareKimlikKodu = VeriErisim.DegerAl(dr, index + 2, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (BirimHavaleYetki[])liste.objeler.ToArray(typeof(BirimHavaleYetki));
        }
    }
}
