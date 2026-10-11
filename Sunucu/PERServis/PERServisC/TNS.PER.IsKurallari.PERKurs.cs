using System;
using System.Data;
using TNS.PER;
using TNS.PER.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.PER.IsKurallari
{
    class PERKurs
    {
        public ObjectArray Listele(Kurs p)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));

            TABKurs tab = new TABKurs(ve);

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
                Kurs pp = new Kurs();

                pp.kod = VeriErisim.DegerAl(r, 0, 0);
                pp.ad = VeriErisim.DegerAl(r, 1, "");

                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public Sonuc Kaydet(Kullanici kullanan, Kurs p)
        {
            Sonuc sonuc = new Sonuc();

            if (p.ad.Trim() == "")
            {
                sonuc.hataStr = "Kurs adı boş bırakılamaz";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABKurs tab = new TABKurs(ve);

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

        public Sonuc Sil(Kullanici kullanan, Kurs p)
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

            TABKurs tab = new TABKurs(ve);

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
    }
}
