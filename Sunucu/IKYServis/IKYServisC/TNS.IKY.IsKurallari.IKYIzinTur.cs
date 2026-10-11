using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.IKY.IsKurallari
{
    class PERIzinTur
    {
        public ObjectArray Listele(IzinTur p)
        {
            ObjectArray pVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));
            TABIzinTur tab = new TABIzinTur(ve);
            IDataReader r = tab.Listele(p);

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = ve.hata;
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
                pVAN.objeler.Add(pp);
            }
            r.Close();
            ve.VeriErisimKapat();

            if (pVAN.objeler.Count == 0)
            {
                pVAN.objeler.Add(new IzinTur(0, "Yıllık İzin", 0, 1, 1));
                pVAN.objeler.Add(new IzinTur(0, "657-104/C", 10, 1, 2));
                pVAN.objeler.Add(new IzinTur(0, "Evlenme", 7, 1, 3));
                pVAN.objeler.Add(new IzinTur(0, "Babalık", 10, 1, 4));
                pVAN.objeler.Add(new IzinTur(0, "Ölüm", 7, 1, 5));
                pVAN.objeler.Add(new IzinTur(0, "Sıhhi", 0, 2, 6));
                pVAN.objeler.Add(new IzinTur(0, "Analık", 0, 2, 7));
                pVAN.objeler.Add(new IzinTur(0, "Ücretli", 0, 2, 8));
                pVAN.objeler.Add(new IzinTur(0, "Ücretsiz", 0, 2, 9));
                pVAN.objeler.Add(new IzinTur(0, "Mazeret İzni", 0, 1, 10));
                pVAN.objeler.Add(new IzinTur(0, "Geçici Görev (Yevmiyesiz- Yolluksuz)", 0, 1, 11));
                pVAN.objeler.Add(new IzinTur(0, "Geçici Görev (Yevmiyesiz- Yolluklu)", 0, 1, 12));
                pVAN.objeler.Add(new IzinTur(0, "Geçici Görev (Yevmiyeli- Yolluklu)", 0, 1, 13));
                pVAN.objeler.Add(new IzinTur(0, "Geçici Görev (Yevmiyeli- Yolluksuz)", 0, 1, 14));

                pVAN.sonuc = IlkKayitlariOlustur(pVAN);
                if (!pVAN.sonuc.islemSonuc)
                {
                    pVAN.objeler.Clear();
                }
            }
            return pVAN;
        }

        public Sonuc IlkKayitlariOlustur(ObjectArray pVAN)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));
            ve.BeginTransaction();
            TABIzinTur tab = new TABIzinTur(ve);

            foreach (IzinTur item in pVAN.objeler)
            {
                sonuc = tab.Kaydet(item);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        public Sonuc Kaydet(Kullanici kullanan, IzinTur p)
        {
            Sonuc sonuc = new Sonuc();

            if (p.kod == 1)
            {
                sonuc.DegerAta("Bu izin türü değiştirilemez.");
            }

            if (string.IsNullOrWhiteSpace(p.ad))
                sonuc.DegerAta(PersonelResI.FRMTIT001 + "<br>");
            if (p.sure < 0)
                sonuc.DegerAta(PersonelResI.FRMTIT002 + "<br>");
            if (p.gorulebilirlik < 0)
                sonuc.DegerAta(PersonelResI.FRMTIT003 + "<br>");
            if (p.sira <= 0)
                sonuc.DegerAta(PersonelResI.FRMTIT004 + "<br>");

            if (!sonuc.islemSonuc)
            {
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));
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

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

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