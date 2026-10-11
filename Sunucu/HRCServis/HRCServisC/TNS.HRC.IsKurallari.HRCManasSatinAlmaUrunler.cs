using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    public class HRCManasSatinAlmaUrun
    {
        public static string kodTasinirHesapYapisiHesap = null;

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ManasSatinAlmaUrun[] formList, int satinAlmaKod)
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
            TABManasSatinAlmaUrunler tab = new TABManasSatinAlmaUrunler(ve);
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

        public static ManasSatinAlmaUrun[] Listele(VeriErisim veriErisim, Kullanici kullanan, ManasSatinAlmaUrun form)
        {
            ObjectArray liste = new ObjectArray();

            if (kodTasinirHesapYapisiHesap == null)
                kodTasinirHesapYapisiHesap = TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABManasSatinAlmaUrunler tab = new TABManasSatinAlmaUrunler(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new ManasSatinAlmaUrun();
                    form.satinAlmaKod = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaUrunler.ENUMAlanlar.SATINALMAKOD, 0);
                    form.siraNo = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaUrunler.ENUMAlanlar.SIRANO, 0);
                    form.muhasebeBirim.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaUrunler.ENUMAlanlar.MUHASEBEKODU, 0);
                    form.muhasebeBirim.birimKod = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaUrunler.ENUMAlanlar.HARCAMABIRIMKODU, "");
                    form.muhasebeBirim.birimAd = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaUrunler.ENUMAlanlar.HARCAMABIRIMADI, "");
                    form.talepRefId = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaUrunler.ENUMAlanlar.TALEPREFID, "");
                    form.talepBelgeNo = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaUrunler.ENUMAlanlar.TALEPBELGENO, "");
                    form.talepTarihi = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaUrunler.ENUMAlanlar.TALEPTARIH);
                    form.hesapPlanKodu = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaUrunler.ENUMAlanlar.HESAPPLANKOD, "");
                    form.hesapPlanAdi = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaUrunler.ENUMAlanlar.HESAPPLANAD, "");
                    form.yansitmaHesapKod = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaUrunler.ENUMAlanlar.YANSITMAHESAPKOD, "");
                    form.aciklama = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaUrunler.ENUMAlanlar.ACIKLAMA, "");
                    form.miktar = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaUrunler.ENUMAlanlar.MIKTAR, (decimal)0);
                    form.olcuBirimi = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaUrunler.ENUMAlanlar.OLCUBIRIMI, "");

                    form.hesapPlanKodu = OrtakFonksiyonlar.KodNoktaKoy(kodTasinirHesapYapisiHesap, form.hesapPlanKodu);
                    form.yansitmaHesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodTasinirHesapYapisiHesap, form.yansitmaHesapKod);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (ManasSatinAlmaUrun[])liste.objeler.ToArray(typeof(ManasSatinAlmaUrun));
        }

    }
}
