using System;
using System.Data;
using TNS.PER;
using TNS.PER.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.PER.IsKurallari
{
    class PERIzinTur
    {
        public ObjectArray Listele(IzinTur p)
        {
            ObjectArray pVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));
            TABIzinTur tab = new TABIzinTur(ve);
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
                IzinTur pp = new IzinTur();

                pp.kod = VeriErisim.DegerAl(r, (int)TABIzinTur.ENUMAlanlar.KOD, 0);
                pp.ad = VeriErisim.DegerAl(r, (int)TABIzinTur.ENUMAlanlar.AD, "");
                pp.sure = VeriErisim.DegerAl(r, (int)TABIzinTur.ENUMAlanlar.SURE, 0);
                pp.gorulebilirlik = VeriErisim.DegerAl(r, (int)TABIzinTur.ENUMAlanlar.GORULEBILIRLIK, 0);
                pp.sira = VeriErisim.DegerAl(r, (int)TABIzinTur.ENUMAlanlar.SIRA, 0);
                pp.ataKod = VeriErisim.DegerAl(r, (int)TABIzinTur.ENUMAlanlar.ATAKOD, 0);
                pVAN.objeler.Add(pp);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            ve.VeriErisimKapat();

            //if (pVAN.objeler.Count == 0)
            //    return IlkKayitlariOlustur();

            return pVAN;
        }

        //public ObjectArray IlkKayitlariOlustur()
        //{
        //    Sonuc sonuc = new Sonuc();
        //    string hata = "";

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));
        //    ve.BeginTransaction();

        //    TABIzinTur tab = new TABIzinTur(ve);
        //    sonuc = tab.TumunuSil();
        //    if (!sonuc.islemSonuc)
        //        hata += sonuc.hataStr;
        //    if (sonuc.islemSonuc)
        //        ve.Commit();
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();
            
        //    IzinTur p;

        //    p = new IzinTur(0, "Yıllık İzin", 0, 1, 1);
        //    sonuc = Kaydet(null, p);
        //    if (!sonuc.islemSonuc)
        //        hata += sonuc.hataStr;
        //    p = new IzinTur(0, "657-104/C", 10, 1, 2);
        //    sonuc = Kaydet(null, p);
        //    if (!sonuc.islemSonuc)
        //        hata += sonuc.hataStr;
        //    p = new IzinTur(0, "Evlenme", 7, 1, 3);
        //    sonuc = Kaydet(null, p);
        //    if (!sonuc.islemSonuc)
        //        hata += sonuc.hataStr;
        //    p = new IzinTur(0, "Babalık", 10, 1, 4);
        //    sonuc = Kaydet(null, p);
        //    if (!sonuc.islemSonuc)
        //        hata += sonuc.hataStr;
        //    p = new IzinTur(0, "Ölüm", 7, 1, 5);
        //    sonuc = Kaydet(null, p);
        //    if (!sonuc.islemSonuc)
        //        hata += sonuc.hataStr;
        //    p = new IzinTur(0, "Sıhhi", 0, 2, 6);
        //    sonuc = Kaydet(null, p);
        //    if (!sonuc.islemSonuc)
        //        hata += sonuc.hataStr;
        //    p = new IzinTur(0, "Analık", 0, 2, 7);
        //    sonuc = Kaydet(null, p);
        //    if (!sonuc.islemSonuc)
        //        hata += sonuc.hataStr;
        //    p = new IzinTur(0, "Ücretli", 0, 2, 8);
        //    sonuc = Kaydet(null, p);
        //    if (!sonuc.islemSonuc)
        //        hata += sonuc.hataStr;
        //    p = new IzinTur(0, "Ücretsiz", 0, 2, 9);
        //    sonuc = Kaydet(null, p);
        //    if (!sonuc.islemSonuc)
        //        hata += sonuc.hataStr;

        //    if (string.IsNullOrEmpty(hata))
        //        return Listele(new IzinTur());
        //    else
        //        return null;
        //}

        public Sonuc Kaydet(Kullanici kullanan, IzinTur p)
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

            TABIzinTur tab = new TABIzinTur(ve);

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

        public Sonuc Sil(Kullanici kullanan, IzinTur p)
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

            TABIzinTur tab = new TABIzinTur(ve);

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