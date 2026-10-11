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
    public class EVRBirimDagitimYetki
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, BirimDagitimYetki[] formList, string evrakBirimKod)
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
            TABBirimDagitimYetki tab = new TABBirimDagitimYetki(ve);
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

        public static BirimDagitimYetki[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimDagitimYetki form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABBirimDagitimYetki tab = new TABBirimDagitimYetki(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                int index = (int)TABBirimDagitimYetki.ENUMAlanlar.SIRANO;
                while (dr.Read())
                {
                    form = new BirimDagitimYetki();
                    form.evrakBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimDagitimYetki.ENUMAlanlar.EVRAKBIRIMKOD, "");
                    form.dagitimBirimi.kod = VeriErisim.DegerAl(dr, (int)TABBirimDagitimYetki.ENUMAlanlar.DAGITIMBIRIMKOD, "");
                    form.siraNo = VeriErisim.DegerAl(dr, (int)TABBirimDagitimYetki.ENUMAlanlar.SIRANO, 0);

                    form.dagitimBirimi.ad = VeriErisim.DegerAl(dr, index + 1, "");
                    form.dagitimBirimi.idareKimlikKodu = VeriErisim.DegerAl(dr, index + 2, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (BirimDagitimYetki[])liste.objeler.ToArray(typeof(BirimDagitimYetki));
        }
    }
}
