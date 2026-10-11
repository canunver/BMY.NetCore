using System;
using System.Data;
using TNS.PER;
using TNS.PER.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.PER.IsKurallari
{
    class PEROgrenim
    {
        public ObjectArray Listele(Ogrenim p)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));

            TABOgrenim tab = new TABOgrenim(ve);

            IDataReader r = null;

            try
            {
                r = tab.Listele(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = e.Message;

                return pVAN;
            }

            while (r.Read())
            {
                Ogrenim pp = new Ogrenim();

                pp.kod = VeriErisim.DegerAl(r, (int)TABOgrenim.ENUMAlanlar.KOD, 0);
                pp.ad = VeriErisim.DegerAl(r, (int)TABOgrenim.ENUMAlanlar.AD, "");
                pp.siraNo = VeriErisim.DegerAl(r, (int)TABOgrenim.ENUMAlanlar.SIRANO, 0);
                pp.tip = VeriErisim.DegerAl(r, (int)TABOgrenim.ENUMAlanlar.TIP, 0);

                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public Sonuc Kaydet(Kullanici kullanan, Ogrenim p)
        {
            Sonuc sonuc = new Sonuc();

            if (p.ad.Trim() == "")
            {
                sonuc.hataStr = "Öğrenim adı boş bırakılamaz";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABOgrenim tab = new TABOgrenim(ve);

            if (p.kod > 0)
                sonuc = tab.Guncelle(p);
            else
                sonuc = tab.Kaydet(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc Sil(Kullanici kullanan, Ogrenim p)
        {
            Sonuc sonuc = new Sonuc();

            if (p.kod == 0)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABOgrenim tab = new TABOgrenim(ve);

            sonuc = tab.Sil(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray OgrenimBazliPersonelSayilariListele(int muhasebeKod, string birimKod)
        {
            ObjectArray donusDeger = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));
            TABOgrenim tab = new TABOgrenim(ve);

            IDataReader r = null;

            try
            {
                r = tab.OgrenimBazliPersonelSayilariListele(muhasebeKod, birimKod);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;

                return donusDeger;
            }

            while (r.Read())
            {
                OgrenimBazliPersonelSayi gelen = new OgrenimBazliPersonelSayi();

                gelen.ogrenim = new Ogrenim();
                gelen.ogrenim.kod = VeriErisim.DegerAl(r, 0, 0);
                gelen.ogrenim.ad = VeriErisim.DegerAl(r, 1, string.Empty);
                gelen.personelSayi = VeriErisim.DegerAl(r, 2, 0);

                donusDeger.objeler.Add(gelen);
            }
            r.Close();

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();
            return donusDeger;
        }
    }
}