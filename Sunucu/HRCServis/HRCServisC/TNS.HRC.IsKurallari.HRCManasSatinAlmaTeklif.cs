using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using System.Collections;

namespace TNS.HRC.IsKurallari
{
    public class HRCManasSatinAlmaTeklif
    {
        public static string kodTasinirHesapYapisiHesap = null;

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ManasSatinAlmaTeklif[] formList, ManasSatinAlmaTeklifForm tf)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            string hata = string.Empty;
            if (tf.satinAlmaKod <= 0)
                hata += "İş tanım bilgisi boş." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABManasSatinAlmaTeklif tab = new TABManasSatinAlmaTeklif(ve);
            ve.BeginTransaction();

            sonuc = tab.Sil(tf.satinAlmaKod, tf.tur);
            sonuc = tab.SilForm(tf.satinAlmaKod, tf.tur);

            Hashtable htYaklasikMaliyet = new Hashtable();
            if (sonuc.islemSonuc)
            {
                foreach (var form in formList)
                {
                    form.refId = Guid.NewGuid().ToString();
                    sonuc = tab.Kaydet(form);
                    if (!sonuc.islemSonuc)
                        break;

                    if (OrtakFonksiyonlar.ConvertToDouble(htYaklasikMaliyet[form.urunRefId]) > form.tutar || OrtakFonksiyonlar.ConvertToDouble(htYaklasikMaliyet[form.urunRefId]) == 0)
                    {
                        htYaklasikMaliyet[form.urunRefId] = form.tutar;
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                double ym = 0.0;
                foreach (DictionaryEntry entry in htYaklasikMaliyet)
                {
                    ym += OrtakFonksiyonlar.ConvertToDbl(entry.Value);
                }
                tf.tutar = ym;
                sonuc = tab.KaydetForm(tf);
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

        public static ManasSatinAlmaTeklifForm Listele(VeriErisim veriErisim, Kullanici kullanan, ManasSatinAlmaTeklif form)
        {
            ObjectArray liste = new ObjectArray();
            ManasSatinAlmaTeklifForm tf = new ManasSatinAlmaTeklifForm();

            if (kodTasinirHesapYapisiHesap == null)
                kodTasinirHesapYapisiHesap = TNS.DEG.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABManasSatinAlmaTeklif tab = new TABManasSatinAlmaTeklif(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                int sonKolon = (int)TABManasSatinAlmaTeklif.ENUMAlanlar.TUR;

                while (dr.Read())
                {
                    form = new ManasSatinAlmaTeklif();
                    form.satinAlmaKod = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaTeklif.ENUMAlanlar.SATINALMAKOD, 0);
                    form.refId = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaTeklif.ENUMAlanlar.REFID, "");
                    form.urunRefId = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaTeklif.ENUMAlanlar.URUNREFID, "");
                    form.firmaRefId = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaTeklif.ENUMAlanlar.FIRMAREFID, "");
                    form.tutar = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaTeklif.ENUMAlanlar.TUTAR, 0.0);
                    form.tur = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaTeklif.ENUMAlanlar.TUR, 0);
                    form.firma.unvan = VeriErisim.DegerAl(dr, sonKolon + 1, "");
                    form.urun.hesapPlanKodu = VeriErisim.DegerAl(dr, sonKolon + 2, "");
                    form.urun.hesapPlanAdi = VeriErisim.DegerAl(dr, sonKolon + 3, "");

                    form.urun.hesapPlanKodu = OrtakFonksiyonlar.KodNoktaKoy(kodTasinirHesapYapisiHesap, form.urun.hesapPlanKodu);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            dr = tab.ListeleForm(new ManasSatinAlmaTeklifForm { satinAlmaKod = form.satinAlmaKod, tur = form.tur });
            if (dr != null)
            {
                while (dr.Read())
                {
                    tf.satinAlmaKod = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaTeklif.ENUMAlanlar.SATINALMAKOD, 0);
                    tf.tur = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaTeklif.ENUMAlanlarForm.TUR, 0);
                    tf.tarih = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaTeklif.ENUMAlanlarForm.TARIH);
                    tf.yontem = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaTeklif.ENUMAlanlarForm.YONTEM, 0);
                    tf.tutar = VeriErisim.DegerAl(dr, (int)TABManasSatinAlmaTeklif.ENUMAlanlarForm.TUTAR, 0.0);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();

            tf.teklifler = (ManasSatinAlmaTeklif[])liste.objeler.ToArray(typeof(ManasSatinAlmaTeklif));

            return tf;
        }

    }
}
