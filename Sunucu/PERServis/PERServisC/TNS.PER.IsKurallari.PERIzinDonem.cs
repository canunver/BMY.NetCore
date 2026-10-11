using System;
using System.Data;
using TNS.PER;
using TNS.PER.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.PER.IsKurallari
{
    class PERIzinDonem
    {
        public ObjectArray Listele(IzinDonem p)
        {
            ObjectArray pVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));
            TABIzinDonem tab = new TABIzinDonem(ve);
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
                IzinDonem pp = new IzinDonem();

                pp.kod = VeriErisim.DegerAl(r, (int)TABIzinDonem.ENUMAlanlar.DONEMID, 0);
                pp.yil = VeriErisim.DegerAl(r, (int)TABIzinDonem.ENUMAlanlar.DONEMYIL, 0);
                pp.donemBaslangicTarihi = VeriErisim.DegerAl(r, (int)TABIzinDonem.ENUMAlanlar.DONEMBASLANGICTARIHI);
                pp.donemBitisTarihi = VeriErisim.DegerAl(r, (int)TABIzinDonem.ENUMAlanlar.DONEMBITISTARIHI);
                pp.donemAciklama = VeriErisim.DegerAl(r, (int)TABIzinDonem.ENUMAlanlar.DONEMACIKLAMA, "");
                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            ve.VeriErisimKapat();

            if (pVAN.objeler.Count == 0)
                return IlkKayitlariOlustur();

            return pVAN;
        }

        public ObjectArray IlkKayitlariOlustur()
        {
            Sonuc sonuc = new Sonuc();
            string hata = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));
            ve.BeginTransaction();

            TABIzinDonem tab = new TABIzinDonem(ve);
            sonuc = tab.TumunuSil();
            if (!sonuc.islemSonuc)
                hata += sonuc.hataStr;
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            
            IzinDonem p;

            p = new IzinDonem(0, 2014, null, null, "1.Dönem");
            sonuc = Kaydet(null, p);
            if (!sonuc.islemSonuc)
                hata += sonuc.hataStr;
            p = new IzinDonem(0, 2014, null, null, "1.Dönem");
            sonuc = Kaydet(null, p);

            if (string.IsNullOrEmpty(hata))
                return Listele(new IzinDonem());
            else
                return null;
        }

        public Sonuc Kaydet(Kullanici kullanan, IzinDonem p)
        {
            Sonuc sonuc = new Sonuc();

            if (p.kod == 1)
            {
                sonuc.hataStr = "Bu izin türü değiştirilemez.";
                sonuc.islemSonuc = false;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABIzinDonem tab = new TABIzinDonem(ve);

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

        public Sonuc Sil(Kullanici kullanan, IzinDonem p)
        {
            Sonuc sonuc = new Sonuc();

            if (p.kod == 0)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (p.kod == 1)
            {
                sonuc.hataStr = "Bu izin türü silinemez.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABIzinDonem tab = new TABIzinDonem(ve);

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