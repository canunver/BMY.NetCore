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
    /// Personel Görev Yer Tarih Sınıfı
    /// </summary>
    class PERGorevYerTarih
    {
        /// <summary>
        /// Görev Yer Tarih bilgisini listeleyen fonksiyon
        /// </summary>
        /// <param name="p">Görev Yer Tarih bilgisi, Görev Yer Tarih tipinde sınıf</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        public ObjectArray Listele(GorevYerTarih p)
        {
            
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);

            TABGorevYerTarih tab = new TABGorevYerTarih(ve);

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
                TNS.ORG.GorevYerTarih yerTarih = new TNS.ORG.GorevYerTarih();
                yerTarih.gorevKod = VeriErisim.DegerAl(r, 0, 0);
                yerTarih.siraNo = VeriErisim.DegerAl(r, 1, 0);
                yerTarih.bulunduguUlkeKod = VeriErisim.DegerAl(r, 2, "");
                yerTarih.bulunduguIlKod = VeriErisim.DegerAl(r, 3, "");
                yerTarih.bulunduguIlceKod = VeriErisim.DegerAl(r, 4, "");
                yerTarih.gidecegiUlkeKod = VeriErisim.DegerAl(r, 5, "");
                yerTarih.gidecegiIlKod = VeriErisim.DegerAl(r, 6, "");
                yerTarih.gidecegiIlceKod = VeriErisim.DegerAl(r, 7, "");
                yerTarih.baslangicTarihi = VeriErisim.DegerAl(r, 8);
                yerTarih.bitisTarihi = VeriErisim.DegerAl(r, 9);
                yerTarih.vasita = VeriErisim.DegerAl(r, 10, 0);
                yerTarih.yolDahilMi = VeriErisim.DegerAl(r, 11, 0);
                yerTarih.bulunduguYer = VeriErisim.DegerAl(r, 12, "") + "-" + VeriErisim.DegerAl(r, 13, "") + "-" + VeriErisim.DegerAl(r, 14, "");
                yerTarih.gidecegiYer = VeriErisim.DegerAl(r, 15, "") + "-" + VeriErisim.DegerAl(r, 16, "") + "-" + VeriErisim.DegerAl(r, 17, "");

                liste.objeler.Add(yerTarih);
            }
            r.Close();

            liste.sonuc.islemSonuc = true;

            if (liste.objeler.Count == 0)
                liste.sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER090;

            ve.VeriErisimKapat();

            return liste;
        }

        /// <summary>
        /// Görev Yer Tarih bilgisi kaydeden fonksiyon
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
                hata = KontrolEt((GorevYerTarih)liste.objeler[i], gorevKod);
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanici));
            ve.BeginTransaction();
            TABGorevYerTarih tab = new TABGorevYerTarih(ve);

            sonuc = tab.Sil(gorevKod);
            if (sonuc.islemSonuc)
                sonuc = tab.Kaydet(liste, gorevKod);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER091;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Görev Yer Tarih bilgisi silen fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="gorevKod">Görev Kodu, Int tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc Sil(Kullanici kullanici, int gorevKod)
        {
            Sonuc sonuc = new Sonuc();

            if (gorevKod == 0)
            {
                sonuc.hataStr = TNS.ORG.OrganizasyonSemaResI.PER092;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanici));

            ve.BeginTransaction();

            TABGorevYerTarih tab = new TABGorevYerTarih(ve);

            sonuc = tab.Sil(gorevKod);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER093;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Görev Yer Tarih bilgisini kontrol eden fonksiyon
        /// </summary>
        /// <param name="yerTarih">Görev Yer Tarih bilgisi, Görev Yer Tarih tipinde sınıf</param>
        /// <param name="gorevKod">Görev kodu, Int tipinde sınıf</param>
        /// <returns>String tipinde sınıf döndürür</returns>
        private string KontrolEt(GorevYerTarih yerTarih, int gorevKod)
        {
            string hata = string.Empty;
            if (gorevKod == 0)
                hata += TNS.ORG.OrganizasyonSemaResI.PER094 + "<br>";
            if (yerTarih.bulunduguIlKod == string.Empty || yerTarih.bulunduguIlceKod == string.Empty)
                hata += TNS.ORG.OrganizasyonSemaResI.PER095 + "<br>";
            if (yerTarih.gidecegiIlKod == string.Empty || yerTarih.gidecegiIlceKod == string.Empty)
                hata += TNS.ORG.OrganizasyonSemaResI.PER096 + "<br>";
            if (yerTarih.baslangicTarihi.isNull)
                hata += TNS.ORG.OrganizasyonSemaResI.PER097 + "<br>";
            if (yerTarih.bitisTarihi.isNull)
                hata += TNS.ORG.OrganizasyonSemaResI.PER098 + "<br>";
            if (yerTarih.vasita == 0)
                hata += TNS.ORG.OrganizasyonSemaResI.PER099 + "<br>";

            return hata;
        }
    }
}