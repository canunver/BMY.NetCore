using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.BKD;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    public class HRCButceBildirim
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ButceBildirim form)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            string hata = "";
            if (form.orgKod <= 0)
                hata += "Birim bilgisi boş bırakılamaz." + "<br>";
            if (form.yil <= 0)
                hata += "Yıl bilgisi boş bırakılamaz." + "<br>";
            if (form.ay <= 0)
                hata += "Ay bilgisi boş bırakılamaz." + "<br>";
            if (form.islemTur <= 0)
                hata += "İşlem türü boş bırakılamaz." + "<br>";
            if (form.tutar <= 0)
                hata += "Tutar değeri boş veya hatalı." + "<br>";
            if (form.tarih.isNull)
                hata += "Tutar alanı boş bırakılamaz." + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABButceBildirim tab = new TABButceBildirim(ve);

            ve.BeginTransaction();
            sonuc = tab.Kaydet(form);

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

        public static ButceBildirim[] Listele(VeriErisim veriErisim, Kullanici kullanan, ButceBildirim form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABButceBildirim tab = new TABButceBildirim(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new ButceBildirim();
                    form.orgKod = VeriErisim.DegerAl(dr, (int)TABButceBildirim.ENUMAlanlar.ORGKOD, 0);
                    form.yil = VeriErisim.DegerAl(dr, (int)TABButceBildirim.ENUMAlanlar.YIL, 0);
                    form.ay = VeriErisim.DegerAl(dr, (int)TABButceBildirim.ENUMAlanlar.AY, 0);
                    form.islemTur = VeriErisim.DegerAl(dr, (int)TABButceBildirim.ENUMAlanlar.ISLEMTUR, 0);
                    form.tutar = VeriErisim.DegerAl(dr, (int)TABButceBildirim.ENUMAlanlar.TUTAR, 0.0);
                    form.tarih = VeriErisim.DegerAl(dr, (int)TABButceBildirim.ENUMAlanlar.TARIH);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            return (ButceBildirim[])liste.objeler.ToArray(typeof(ButceBildirim));
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ButceBildirim form)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABButceBildirim tab = new TABButceBildirim(ve);

            ve.BeginTransaction();

            sonuc = tab.Sil(form);
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
    }
}
