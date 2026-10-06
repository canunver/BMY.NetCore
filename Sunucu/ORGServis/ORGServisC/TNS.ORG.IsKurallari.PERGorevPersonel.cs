using System;
using System.Collections.Generic;
using System.Text;
using TNS.KYM;
using TNS.Veri;
using TNS.ORG.IsKurallari.Veri;
using System.Data;

namespace TNS.ORG.IsKurallari
{
    /// <summary>
    /// Personel Görev Personel Sınıfı
    /// </summary>
    class PERGorevPersonel
    {
        /// <summary>
        /// Görevli Personelleri listeleyen fonksiyon
        /// </summary>
        /// <param name="p">Görev Personel bilgisi, Görev Personel tipinde sınıf</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        public ObjectArray Listele(GorevPersonel p)
        {
            
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);

            TABGorevPersonel tab = new TABGorevPersonel(ve);

            IDataReader r = null;

            try
            {
                r = tab.Listele(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;

                return liste;
            }

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
                gp.avansTutari = VeriErisim.DegerAl(r, 8, 0.0);
                gp.avansParaBirimKod = VeriErisim.DegerAl(r, 9, 0);
                gp.surecYil = VeriErisim.DegerAl(r, 10, 0);
                gp.surecNo = VeriErisim.DegerAl(r, 11, "");

                liste.objeler.Add(gp);
            }
            r.Close();

            liste.sonuc.islemSonuc = true;

            if (liste.objeler.Count == 0)
                liste.sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER077;

            ve.VeriErisimKapat();

            return liste;
        }

        /// <summary>
        /// Görev personel bilgisi kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="liste">Liste bilgisi, Object Array tipinde sınıf</param>
        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc Kaydet(Kullanici kullanici, ObjectArray liste, int gorevKod)
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

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanici));
            ve.BeginTransaction();
            TABGorevPersonel tab = new TABGorevPersonel(ve);

            sonuc = tab.Sil(gorevKod);
            if (sonuc.islemSonuc)
                sonuc = tab.Kaydet(liste, gorevKod);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER078;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Görev Personel bilgisini silen fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc Sil(Kullanici kullanici, int gorevKod)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanici));

            ve.BeginTransaction();

            TABGorevPersonel tab = new TABGorevPersonel(ve);

            sonuc = tab.Sil(gorevKod);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER079;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Görevli Personeli kontrol eden fonksiyon
        /// </summary>
        /// <param name="gorevPersonel">Görev Personel bilgisi, Görev Personel tipinde sınıf</param>
        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>
        /// <returns>String tipinde sınıf döndürür</returns>
        private string KontrolEt(GorevPersonel gorevPersonel, int gorevKod)
        {
            string hata = string.Empty;
            if (gorevKod == 0)
                hata += TNS.ORG.OrganizasyonSemaResI.PER080 + "<br>";
            if (gorevPersonel.tckimlikno == "")
                hata += TNS.ORG.OrganizasyonSemaResI.PER081 + "<br>";
            if (gorevPersonel.avansTutari > 0 && gorevPersonel.avansParaBirimKod == 0)
                hata += TNS.ORG.OrganizasyonSemaResI.PER082 + "<br>";
            return hata;
        }
    }
}