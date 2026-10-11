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
    public class EVREvrakMeta
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif evrak, EvrakMeta[] form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = "";
            if (string.IsNullOrWhiteSpace(evrak.evrEvrakNo))
                hata += "evrEvrakNo boş olamaz.<br>";
            if (string.IsNullOrWhiteSpace(evrak.prEvrakNo))
                hata += "prEvrakNo boş olamaz.<br>";
            if (form == null || evrak == null)
                sonuc.hataStr = "Kaydedilecek bilgi bulunamadı.<br>";
            if (hata != "")
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakMeta tab = new TABEvrakMeta(ve);
            TABGenel tabGenel = new TABGenel(ve);


            sonuc = EVREvrak.KontrolEt(ve, kullanan, evrak);
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }


            ve.BeginTransaction();
            try
            {
                sonuc = tab.SilMeta(evrak.evrEvrakNo);

                foreach (EvrakMeta meta in form)
                {
                    meta.evrEvrakNo = evrak.evrEvrakNo;
                    sonuc = tab.Kaydet(meta);
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
                OrtakFonksiyonlar.HataStrYaz("{F98D36C3-7DCE-46E4-B28B-B4F9E71A56AV} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static EvrakMeta[] Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakMeta form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakMeta tab = new TABEvrakMeta(ve);

            Meta[] metaListe = EVRMeta.Listele(ve, kullanan, form.meta, true);
            if (metaListe != null && metaListe.Length > 0)
            {
                IDataReader dr = tab.Listele(form);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        EvrakMeta evrakMeta = new EvrakMeta();
                        evrakMeta.evrEvrakNo = VeriErisim.DegerAl(dr, (int)TABEvrakMeta.ENUMAlanlar.EVREVRAKNO, "");
                        evrakMeta.meta.kod = VeriErisim.DegerAl(dr, (int)TABEvrakMeta.ENUMAlanlar.METAKOD, 0);
                        evrakMeta.metaDeger = VeriErisim.DegerAl(dr, (int)TABEvrakMeta.ENUMAlanlar.METADEGER, "");
                        evrakMeta.metaDeger2 = VeriErisim.DegerAl(dr, (int)TABEvrakMeta.ENUMAlanlar.METADEGER2, "");

                        liste.objeler.Add(evrakMeta);
                    }
                    dr.Close();
                }

                foreach (var meta in metaListe)
                {
                    bool metaVar = false;
                    foreach (EvrakMeta evrakMeta in liste.objeler)
                    {
                        if (meta.kod == evrakMeta.meta.kod)
                        {
                            evrakMeta.meta = meta;
                            metaVar = true;
                            break;
                        }
                    }

                    if (!metaVar)
                    {
                        EvrakMeta evrakMeta = new EvrakMeta();
                        evrakMeta.evrEvrakNo = form.evrEvrakNo;
                        evrakMeta.meta = meta;
                        evrakMeta.metaDeger = "";
                        evrakMeta.metaDeger2 = "";

                        liste.objeler.Add(evrakMeta);
                    }
                }

            }

            ve.VeriErisimKapat();

            return (EvrakMeta[])liste.objeler.ToArray(typeof(EvrakMeta));
        }

    }
}
