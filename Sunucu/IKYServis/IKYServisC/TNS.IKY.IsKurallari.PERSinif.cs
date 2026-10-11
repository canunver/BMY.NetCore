using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.IKY.IsKurallari
{
    class PERSinif
    {
        //public ObjectArray Listele(Sinif p)
        //{
        //    ObjectArray pVAN = new ObjectArray();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));

        //    TABSinif tab = new TABSinif(ve);

        //    IDataReader r = null;

        //    try
        //    {
        //        r = tab.Listele(p);
        //    }
        //    catch (Exception e)
        //    {
        //        ve.VeriErisimKapat();
        //        pVAN.sonuc.islemSonuc = false;
        //        pVAN.sonuc.hataStr = e.Message;

        //        return pVAN;
        //    }

        //    while (r.Read())
        //    {
        //        Sinif pp = new Sinif();

        //        pp.kod = VeriErisim.DegerAl(r, 0, 0);
        //        pp.ad = VeriErisim.DegerAl(r, 1, "");

        //        pVAN.objeler.Add(pp);
        //    }
        //    r.Close();

        //    pVAN.sonuc.islemSonuc = true;

        //    if (pVAN.objeler.Count == 0)
        //        pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

        //    ve.VeriErisimKapat();

        //    return pVAN;
        //}

        //public Sonuc Kaydet(Kullanici kullanan, Sinif p)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    if (p.ad.Trim() == "")
        //    {
        //        sonuc.hataStr = "Unvan adı boş bırakılamaz";
        //        sonuc.islemSonuc = false;
        //        return sonuc;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    ve.BeginTransaction();

        //    TABSinif tab = new TABSinif(ve);

        //    sonuc = tab.Kaydet(p);

        //    if (sonuc.islemSonuc)
        //    {
        //        ve.Commit();
        //        sonuc.bilgiStr = PersonelResI.PER007;
        //    }
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        //public Sonuc Sil(Kullanici kullanan, Sinif p)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    if (p.kod == 0)
        //    {
        //        sonuc.hataStr = "Silmek istenilen <b>Bilginin kodu</b> belirtilmemiş.";
        //        sonuc.islemSonuc = false;
        //        return sonuc;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    ve.BeginTransaction();

        //    TABSinif tab = new TABSinif(ve);

        //    sonuc = tab.Sil(p);

        //    if (sonuc.islemSonuc)
        //    {
        //        ve.Commit();
        //        sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
        //    }
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        //public static ObjectArray SinifBazliPersonelSayilariListele(int muhasebeKod, string birimKod)
        //{
        //    ObjectArray donusDeger = new ObjectArray();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));
        //    TABSinif tab = new TABSinif(ve);

        //    IDataReader r = null;

        //    try
        //    {
        //        r = tab.SinifBazliPersonelSayilariListele(muhasebeKod, birimKod);
        //    }
        //    catch (Exception e)
        //    {
        //        ve.VeriErisimKapat();
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.Message;

        //        return donusDeger;
        //    }

        //    while (r.Read())
        //    {
        //        SinifBazliPersonelSayi gelen = new SinifBazliPersonelSayi();

        //        gelen.sinif = new Sinif();
        //        gelen.sinif.kod = VeriErisim.DegerAl(r, 0, 0);
        //        gelen.sinif.ad = VeriErisim.DegerAl(r, 1, string.Empty);
        //        gelen.personelSayi = VeriErisim.DegerAl(r, 2, 0);

        //        donusDeger.objeler.Add(gelen);
        //    }
        //    r.Close();

        //    if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
        //        donusDeger.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

        //    ve.VeriErisimKapat();
        //    return donusDeger;
        //}
    }
}
