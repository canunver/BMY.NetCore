using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    public class HRCManasSatinAlmaGorevli
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ManasSatinAlmaGorevli[] formList, int satinAlmaKod)
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
            TABManasSatinAlmaGorevli tab = new TABManasSatinAlmaGorevli(ve);
            ve.BeginTransaction();

            sonuc = tab.Sil(satinAlmaKod);
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

        public static ManasSatinAlmaGorevli[] Listele(VeriErisim veriErisim, Kullanici kullanan, ManasSatinAlmaGorevli form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABManasSatinAlmaGorevli tab = new TABManasSatinAlmaGorevli(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new ManasSatinAlmaGorevli();
                    form.satinAlmaKod = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaGorevli.ENUMAlanlar.SATINALMAKOD, 0);
                    form.siraNo = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaGorevli.ENUMAlanlar.SIRANO, 0);
                    form.yetkili = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaGorevli.ENUMAlanlar.YETKILI, "");
                    form.unvan = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaGorevli.ENUMAlanlar.UNVAN, "");

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (ManasSatinAlmaGorevli[])liste.objeler.ToArray(typeof(ManasSatinAlmaGorevli));
        }

    }
}
