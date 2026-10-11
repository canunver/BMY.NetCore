using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.HRC.IsKurallari.Veri;
using System.Collections;
using System.Data;

namespace TNS.HRC.IsKurallari
{
    public class HRCOnMaliKontrol
    {
        internal static Sonuc Kaydet(VeriErisim veriErisim, KYM.Kullanici kullanan, OnMaliKontrolAna omk)
        {
            if (omk == null || omk.dizi == null || omk.dizi.Length == 0)
            {
                return new Sonuc(false, "Lütfen kaydedilecek veri seçiniz!", "");
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABHRCOMK tab = new TABHRCOMK(ve);

            Sonuc s = tab.SilDetay(omk.surecYil, omk.surecNo, omk.onMaliKontrolTur, omk.odemeEmriNo);
            if (s.islemSonuc)
                s = tab.SilAna(omk.surecYil, omk.surecNo, omk.onMaliKontrolTur, omk.odemeEmriNo);
            if (s.islemSonuc)
                s = tab.EkleAna(omk.surecYil, omk.surecNo, omk.onMaliKontrolTur, omk.odemeEmriNo, omk.aciklama, kullanan.ad + " " + kullanan.soyad);
            for (int i = 0; i < omk.dizi.Length && s.islemSonuc; i++)
            {
                tab.EkleDetay(omk.surecYil, omk.surecNo, omk.onMaliKontrolTur, omk.odemeEmriNo, omk.dizi[i].id, omk.dizi[i].deger);
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static OnMaliKontrolAna Listele(VeriErisim veriErisim, KYM.Kullanici kullanan, int surecYil, string surecNo, ENUMOnMaliKontrolTur onMaliKontrolTur, string odemeEmriNo)
        {
            System.Collections.Generic.List<OnMaliKontrolDetay> al = new List<OnMaliKontrolDetay>();
            int say = 0;
            string resourceStr;
            if (onMaliKontrolTur == ENUMOnMaliKontrolTur.SOZLESMEONCESI)
            {
                say = 17;
                resourceStr = "SO";
            }
            else
            {
                say = 13;
                resourceStr = "OO";
            }

            for (int i = 1; i <= say; i++)
            {
                OnMaliKontrolDetay omkd = new OnMaliKontrolDetay(i, HarcamaSurecleriResI.ResourceManager.GetString("HRCOMK" + resourceStr + i.ToString()));
                omkd.deger = 3;
                al.Add(omkd);
            }

            OnMaliKontrolAna omk = new OnMaliKontrolAna();
            omk.dizi = al.ToArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABHRCOMK tab = new TABHRCOMK(ve);

            IDataReader dr = tab.AciklamaBul(surecYil, surecNo, onMaliKontrolTur, odemeEmriNo);
            if (dr != null)
            {
                if (dr.Read())
                {
                    omk.aciklama = VeriErisim.DegerAl(dr, 0, "");
                    omk.tarih = VeriErisim.DegerAl(dr, 1);
                    omk.islemiYapan = VeriErisim.DegerAl(dr, 2, "");
                }
                dr.Close();
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            dr = tab.ListeleDetay(surecYil, surecNo, onMaliKontrolTur, odemeEmriNo);

            if (dr != null)
            {
                while (dr.Read())
                {
                    int kod = VeriErisim.DegerAl(dr, 0, (int)0);
                    OnMaliKontrolDetay det = omk.DetayBul(kod);
                    if (det != null)
                        det.deger = VeriErisim.DegerAl(dr, 1, (int)0);
                }
                dr.Close();
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            ve.VeriErisimKapat();
            return omk;
        }
    }
}
