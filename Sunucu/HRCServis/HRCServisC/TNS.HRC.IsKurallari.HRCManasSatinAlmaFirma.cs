using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    public class HRCManasSatinAlmaFirma
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ManasSatinAlmaFirma[] formList, int satinAlmaKod, int tur)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            string hata = string.Empty;
            if (satinAlmaKod <= 0)
                hata += "İş tanım bilgisi boş." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABManasSatinAlmaFirma tab = new TABManasSatinAlmaFirma(ve);
            ve.BeginTransaction();

            sonuc = tab.Sil(satinAlmaKod, tur);
            if (sonuc.islemSonuc)
            {
                foreach (var form in formList)
                {
                    if (string.IsNullOrWhiteSpace(form.refId)) form.refId = Guid.NewGuid().ToString();

                    sonuc = tab.Kaydet(form);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ManasSatinAlmaFirma[] Listele(VeriErisim veriErisim, Kullanici kullanan, ManasSatinAlmaFirma form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABManasSatinAlmaFirma tab = new TABManasSatinAlmaFirma(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new ManasSatinAlmaFirma();
                    form.satinAlmaKod = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaFirma.ENUMAlanlar.SATINALMAKOD, 0);
                    form.tur = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaFirma.ENUMAlanlar.TUR, 0);
                    form.siraNo = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaFirma.ENUMAlanlar.SIRANO, 0);
                    form.vergiNo = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaFirma.ENUMAlanlar.VERGINO, "");
                    form.unvan = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaFirma.ENUMAlanlar.UNVAN, "");
                    form.refId = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaFirma.ENUMAlanlar.REFID, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (ManasSatinAlmaFirma[])liste.objeler.ToArray(typeof(ManasSatinAlmaFirma));
        }

    }
}
