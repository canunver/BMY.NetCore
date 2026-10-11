using System;
using System.Data;
using TNS.PER;
using TNS.PER.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.PER.IsKurallari
{
    class PERGorevPersonel
    {
        public ObjectArray Listele(GorevPersonel p)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));

            TABGorevPersonel tab = new TABGorevPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.Listele(p);
            
                while (r.Read())
                {
                    GorevPersonel gp = new GorevPersonel();

                    gp.gorevKod = VeriErisim.DegerAl(r, 0, 0);
                    gp.tckimlikno = VeriErisim.DegerAl(r, 1, "");
                    gp.ad = VeriErisim.DegerAl(r, 2, "");
                    gp.soyad = VeriErisim.DegerAl(r, 3, "");
                    gp.unvan = VeriErisim.DegerAl(r, 4, "");
                    gp.unvanKod = VeriErisim.DegerAl(r, 5, 0);
                    gp.kadroDerece = VeriErisim.DegerAl(r, 6, 0);
                    gp.ekGosterge = VeriErisim.DegerAl(r, 7, 0);

                    liste.objeler.Add(gp);
                }
                r.Close();
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;

                return liste;
            }

            liste.sonuc.islemSonuc = true;

            if (liste.objeler.Count == 0)
                liste.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return liste;
        }

        public Sonuc Kaydet(Kullanici kullanan, ObjectArray liste, int gorevYil, int gorevKod)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            string hata = string.Empty;

            if (liste.ObjeSayisi == 0)
                return sonuc;

            for (int i = 0; i < liste.ObjeSayisi; i++)
            {
                hata = KontrolEt((GorevPersonel)liste.objeler[i], gorevKod);
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABGorevPersonel tab = new TABGorevPersonel(ve);

            sonuc = tab.Sil(gorevYil, gorevKod);
            if (sonuc.islemSonuc)
                sonuc = tab.Kaydet(liste, gorevYil, gorevKod);

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

        public Sonuc Sil(Kullanici kullanan, int gorevYil, int gorevKod)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABGorevPersonel tab = new TABGorevPersonel(ve);

            sonuc = tab.Sil(gorevYil, gorevKod);

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

        private string KontrolEt(GorevPersonel gorevPersonel, int gorevKod)
        {
            string hata = string.Empty;
            if (gorevKod == 0)
                hata += "Görev No bilgisi boş bırakılamaz.<br>";
            if (gorevPersonel.tckimlikno == "")
                hata += "TC Kimlik No bilgisi boş bırakılamaz.<br>";
            return hata;
        }
    }
}
