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
    /// Personel Görev Tertip Sınıfı
    /// </summary>
    class PERGorevTertip
    {
        /// <summary>
        /// Görev Tertip bilgisini listeleyen fonksiyon
        /// </summary>
        /// <param name="p">Görev Tertip bilgisi, Görev Tertip tipinde sınıf</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        public ObjectArray Listele(GorevTertip p)
        {
            
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);

            TABGorevTertip tab = new TABGorevTertip(ve);

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
                TNS.ORG.GorevTertip tertip = new TNS.ORG.GorevTertip();
                tertip.gorevKod = VeriErisim.DegerAl(r, 0, 0);
                tertip.siraNo = VeriErisim.DegerAl(r, 1, 0);
                tertip.tertip = VeriErisim.DegerAl(r, 2, "");
                tertip.kullanilacakTutar = VeriErisim.DegerAl(r, 3, 0.0);

                liste.objeler.Add(tertip);
            }
            r.Close();

            liste.sonuc.islemSonuc = true;

            if (liste.objeler.Count == 0)
                liste.sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER083;

            ve.VeriErisimKapat();

            return liste;
        }

        /// <summary>
        /// Görev Tertip bilgisini kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="liste">Liste bilgisi, Object Array tipinde sınıf</param>
        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc Kaydet(Kullanici kullanici, ObjectArray liste, int gorevKod)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;

            for (int i = 0; i < liste.ObjeSayisi; i++)
            {
                hata = KontrolEt((GorevTertip)liste.objeler[i], gorevKod);
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanici));
            ve.BeginTransaction();
            TABGorevTertip tab = new TABGorevTertip(ve);

            sonuc = tab.Sil(gorevKod);
            if (sonuc.islemSonuc)
                sonuc = tab.Kaydet(liste, gorevKod);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER084;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Görev Tertip bilgisini silen fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc Sil(Kullanici kullanici, int gorevKod)
        {
            Sonuc sonuc = new Sonuc();

            if (gorevKod == 0)
            {
                sonuc.hataStr = TNS.ORG.OrganizasyonSemaResI.PER085;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanici));

            ve.BeginTransaction();

            TABGorevTertip tab = new TABGorevTertip(ve);

            sonuc = tab.Sil(gorevKod);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER086;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Görev Tertip bilgisini kontrol eden fonksiyon
        /// </summary>
        /// <param name="tertip">Görev Tertip bilgisi, Görev Tertip tipinde sınıf</param>
        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>
        /// <returns>String tipinde sınıf döndürür</returns>
        private string KontrolEt(GorevTertip tertip, int gorevKod)
        {
            string hata = string.Empty;
            if (gorevKod == 0)
                hata += TNS.ORG.OrganizasyonSemaResI.PER087 + "<br>";
            if (tertip.tertip == string.Empty)
                hata += TNS.ORG.OrganizasyonSemaResI.PER088+"<br>";
            if (tertip.kullanilacakTutar == 0.0)
                hata += TNS.ORG.OrganizasyonSemaResI.PER089+"<br>";

            return hata;
        }
    }
}