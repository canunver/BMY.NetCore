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
    /// Personel Görev Sınıfı
    /// </summary>
    class PERGorev
    {
        /// <summary>
        /// Kontrol durum olasılıkları
        /// </summary>
        private enum ENUMKontrolDurum
        {
            /// <summary>
            /// Kaydet
            /// </summary>
            KAYDET = 1,
            /// <summary>
            /// Sil
            /// </summary>
            SIL = 2,
            /// <summary>
            /// Durum Değiştir
            /// </summary>
            DURUMDEGISTIR = 3,
        }

        /// <summary>
        /// Görev bilgilerini sorgulayan fonksiyon
        /// </summary>
        /// <param name="p">Görev bilgisi, Görev tipinde sınıf</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        public ObjectArray Sorgula(Gorev p)
        {
            
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);

            TABGorev tab = new TABGorev(ve);

            IDataReader r = null;

            try
            {
                r = tab.Sorgula(p);
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
                Gorev gorev = new Gorev();

                gorev.kod = VeriErisim.DegerAl(r, 0, 0);
                gorev.muhasebeKod = VeriErisim.DegerAl(r, 1, 0);
                gorev.birimKod = VeriErisim.DegerAl(r, 2, "");
                gorev.amac = VeriErisim.DegerAl(r, 3, "");
                gorev.tur = VeriErisim.DegerAl(r, 4, 0);
                gorev.durum = VeriErisim.DegerAl(r, 5, 0);
                gorev.islemKod = VeriErisim.DegerAl(r, 6, 0);
                gorev.evrEvrakNo = VeriErisim.DegerAl(r, 7, "");
                gorev.prEvrakNo = VeriErisim.DegerAl(r, 8, "");

                gorev.muhasebeAd = VeriErisim.DegerAl(r, 9, "");
                gorev.birimAd = VeriErisim.DegerAl(r, 10, "");

                liste.objeler.Add(gorev);
            }
            r.Close();

            liste.sonuc.islemSonuc = true;

            if (liste.ObjeSayisi == 0)
                liste.sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER062;

            ve.VeriErisimKapat();

            return liste;
        }

        /// <summary>
        /// Görev bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="p">Görev bilgisi, Görev tipinde sınıf</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        public ObjectArray Listele(Gorev p)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);

            TABGorev tab = new TABGorev(ve);

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
                Gorev gorev = new Gorev();

                gorev.kod = VeriErisim.DegerAl(r, 0, 0);
                gorev.muhasebeKod = VeriErisim.DegerAl(r, 1, 0);
                gorev.birimKod = VeriErisim.DegerAl(r, 2, "");
                gorev.amac = VeriErisim.DegerAl(r, 3, "");
                gorev.tur = VeriErisim.DegerAl(r, 4, 0);
                gorev.durum = VeriErisim.DegerAl(r, 5, 0);
                gorev.islemKod = VeriErisim.DegerAl(r, 6, 0);
                gorev.evrEvrakNo = VeriErisim.DegerAl(r, 7, "");
                gorev.prEvrakNo = VeriErisim.DegerAl(r, 8, "");

                gorev.gorevYerleri = ListeleGorevYerleri(gorev);
                gorev.gorevliPersoneller = ListeleGorevliPersoneller(gorev);
                gorev.gorevTertipler = ListeleGorevTertipler(gorev);

                liste.objeler.Add(gorev);
            }
            r.Close();

            liste.sonuc.islemSonuc = true;

            if (liste.ObjeSayisi == 0)
                liste.sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER063;

            ve.VeriErisimKapat();

            return liste;
        }

        /// <summary>
        /// Görev Yerlerini listeleyen fonksiyon
        /// </summary>
        /// <param name="gorev">Görev bilgisi, Görev tipinde sınıf</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        private ObjectArray ListeleGorevYerleri(Gorev gorev)
        {
            GorevYerTarih yerTarih = new GorevYerTarih();
            yerTarih.gorevKod = gorev.kod;
            return new PERGorevYerTarih().Listele(yerTarih);
        }

        /// <summary>
        /// Görevli Personelleri listeleyen fonksiyon
        /// </summary>
        /// <param name="gorev">Görev bilgisi, Görev tipinde sınıf</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        private ObjectArray ListeleGorevliPersoneller(Gorev gorev)
        {
            GorevPersonel gorevPersonel = new GorevPersonel();
            gorevPersonel.gorevKod = gorev.kod;
            return new PERGorevPersonel().Listele(gorevPersonel);
        }

        /// <summary>
        /// Görev tertiplerini listeleyen fonksiyon
        /// </summary>
        /// <param name="gorev">Görev bilgisi, Görev tipinde sınıf</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        private ObjectArray ListeleGorevTertipler(Gorev gorev)
        {
            GorevTertip tertip = new GorevTertip();
            tertip.gorevKod = gorev.kod;
            return new PERGorevTertip().Listele(tertip);
        }

        /// <summary>
        /// Görev bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="p">Görev bilgisi, Görev tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc Kaydet(Kullanici kullanici, Gorev p)
        {
            Sonuc sonuc = new Sonuc();
            string hata = string.Empty;

            hata = KontrolEt(p, ENUMKontrolDurum.KAYDET);

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanici));

            //Belgeye numara alınıyor
            if (p.kod == 0)
            {
                p.kod = TABGenel.SayacDegerAl(ve, "PERGOREV", "KOD");
            }

            ve.BeginTransaction();

            TABGorev tab = new TABGorev(ve);
            sonuc = tab.Kaydet(p);

            if (sonuc.islemSonuc)
            {
                sonuc = new PERGorevYerTarih().Kaydet(kullanici, p.gorevYerleri, p.kod);
                if (sonuc.islemSonuc)
                {
                    sonuc = new PERGorevPersonel().Kaydet(kullanici, p.gorevliPersoneller, p.kod);
                    if (sonuc.islemSonuc)
                    {
                        sonuc = new PERGorevTertip().Kaydet(kullanici, p.gorevTertipler, p.kod);

                        if (sonuc.islemSonuc)
                        {
                            ve.Commit();
                            sonuc.anahtar = p.kod.ToString();
                            sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER064;
                        }
                        else
                            ve.Rollback();
                    }
                    else
                        ve.Rollback();
                }
                else
                    ve.Rollback();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Evrak bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="kod">Evrak kod bilgisi, Int tipinde sınıf</param>
        /// <param name="evrEvrakNo">Evrak numarası bilgisi, String tipinde sınıf</param>
        /// <param name="prEvrakNo">Program Evrak Numara bilgisi, String tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc KaydetEvrakBilgi(Kullanici kullanici, int kod, string evrEvrakNo, string prEvrakNo)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanici));

            ve.BeginTransaction();

            TABGorev tab = new TABGorev(ve);
            sonuc = tab.KaydetEvrakBilgi(kod, evrEvrakNo, prEvrakNo);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER065;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Görev durumu değiştiren fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="p">Görev bilgisi, Görev tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc DurumDegistir(Kullanici kullanici, Gorev p)
        {
            Sonuc sonuc = new Sonuc();
            string hata = string.Empty;

            hata = KontrolEt(p, ENUMKontrolDurum.DURUMDEGISTIR);

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanici));

            ve.BeginTransaction();

            TABGorev tab = new TABGorev(ve);

            sonuc = tab.Kaydet(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER066;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Görev bilgilerini silen fonksiyon
        /// </summary>
        /// <param name="kullanici">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="p">Görev bilgisi, Görev tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc Sil(Kullanici kullanici, Gorev p)
        {
            Sonuc sonuc = new Sonuc();

            if (p.kod == 0)
            {
                sonuc.hataStr = TNS.ORG.OrganizasyonSemaResI.PER067;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanici));

            ve.BeginTransaction();

            TABGorev tab = new TABGorev(ve);

            sonuc = tab.Sil(p);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.ORG.OrganizasyonSemaResI.PER068;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Görev bilgisini kontrol eden fonksiyon
        /// </summary>
        /// <param name="p">Görev Bilgisi, Görev tipinde sınıf</param>
        /// <param name="durum">Kontrol durum bilgisi, ENUM Kontrol Durum tipinde sınıf</param>
        /// <returns>String tipinde sınıf döndürür</returns>
        private string KontrolEt(Gorev p, ENUMKontrolDurum durum)
        {
            string hata = string.Empty;
            if (durum == ENUMKontrolDurum.KAYDET)
            {
                if (p.muhasebeKod == 0)
                    hata += TNS.ORG.OrganizasyonSemaResI.PER069 + "<br>";
                if (p.tur == 0)
                    hata += TNS.ORG.OrganizasyonSemaResI.PER070 + "<br>";
                if (p.birimKod == "")
                    hata += TNS.ORG.OrganizasyonSemaResI.PER071 + "<br>";
                if (p.gorevYerleri == null || p.gorevYerleri.ObjeSayisi == 0)
                    hata += TNS.ORG.OrganizasyonSemaResI.PER072+"<br>";
                if (p.gorevliPersoneller == null || p.gorevliPersoneller.ObjeSayisi == 0)
                    hata +=TNS.ORG.OrganizasyonSemaResI.PER073 +"<br>";
                if (p.gorevTertipler == null || p.gorevTertipler.ObjeSayisi == 0)
                    hata += TNS.ORG.OrganizasyonSemaResI.PER074+"<br>";
            }
            else if (durum == ENUMKontrolDurum.SIL)
            {
                if (p.kod == 0)
                    hata += TNS.ORG.OrganizasyonSemaResI.PER075+"<br>";
            }
            else if (durum == ENUMKontrolDurum.DURUMDEGISTIR)
            {
                if (p.kod == 0)
                    hata += TNS.ORG.OrganizasyonSemaResI.PER076+"<br>";
            }
            return hata;
        }
    }
}