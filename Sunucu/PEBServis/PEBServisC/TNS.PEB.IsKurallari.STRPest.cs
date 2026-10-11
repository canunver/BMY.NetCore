using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary> 
    /// Stratejik plan mevcut durum analizinde yapılacak olan Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) analizlerinin yönetilmesi 
    /// ile ilgili veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma, silme işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class STRPest
    {
        /// <summary>
        /// Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen pest nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pest">Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Pest pest, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (pest.donem == null || pest.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (pest.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (string.IsNullOrEmpty(pest.analizYontemi))
                hata += PerformansButceResI.STR042 + "<br>";
            if (pest.pestKonuListe == null || pest.pestKonuListe.Length <= 0)
                hata += PerformansButceResI.STR043;
            else
            {
                foreach (PestDetay pdetay in pest.pestKonuListe)
                {
                    if (string.IsNullOrEmpty(pdetay.temelEgilimler))
                        hata += PerformansButceResI.STR051;
                    if (!string.IsNullOrEmpty(hata))
                        break;
                }
            }
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, pest.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, pest.donem.donemNo, pest.orgBirimKod, ENUMSurec.MevcutDurumAnalizi, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            ve.BeginTransaction();

            Sil(ve, pest.donem.donemNo, pest.orgBirimKod);
            sonuc = Kaydet(ve, pest);

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, pest.donem.donemNo, pest.orgBirimKod, ENUMSurec.MevcutDurumAnalizi);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = PerformansButceResI.STR010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PerformansButceResI.STR011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc Kaydet(VeriErisim ve, Pest pest)
        {
            TABPest to = new TABPest(ve);
            Sonuc sonuc = to.Kaydet(pest);

            if (sonuc.islemSonuc)
            {
                foreach (PestDetay pd in pest.pestKonuListe)
                {
                    sonuc = to.KaydetDetay(pest.donem.donemNo, pest.orgBirimKod, pd);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            return sonuc;
        }

        private static Sonuc Sil(VeriErisim ve, int donemNo, int orgBirimKod)
        {
            TABPest to = new TABPest(ve);
            Sonuc sonuc = to.SilDetay(donemNo, orgBirimKod);

            if (sonuc.islemSonuc)
                sonuc = to.Sil(donemNo, orgBirimKod);

            return sonuc;
        }

        /// <summary>
        /// Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen pest nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pest">Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Pest pest, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (pest.donem == null || pest.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (pest.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, pest.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, pest.donem.donemNo, pest.orgBirimKod, ENUMSurec.MevcutDurumAnalizi, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            ve.BeginTransaction();

            sonuc = Sil(ve, pest.donem.donemNo, pest.orgBirimKod);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PerformansButceResI.STR014;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = PerformansButceResI.STR015 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pest">Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) bilgisini tutan nesne</param>
        /// <returns>Bir Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) için Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) ayrıntılarını döndürür</returns>
        public static Pest[] Listele(VeriErisim veriErisim, Kullanici kullanan, Pest pest, bool detay)
        {
            return Listele(veriErisim, kullanan, pest, detay, false);
        }

        public static Pest[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, Pest pest)
        {
            string hata = string.Empty;
            if (pest.donem == null || pest.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (pest.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033;
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            if (pest.pestKonuListe != null && pest.pestKonuListe.Length == 1)
            {
                pest.pestKonuListe[0].donem = pest.donem;
                pest.pestKonuListe[0].orgBirimKod = pest.orgBirimKod;
            }

            return Listele(veriErisim, kullanan, pest, false, true);
        }

        private static Pest[] Listele(VeriErisim veriErisim, Kullanici kullanan, Pest pest, bool detay, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, pest.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPest to = new TABPest(ve);

            IDataReader dr = null;
            try
            {
                dr = to.Listele(pest, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            VeriErisim ve2 = null;
            if (detay || (sorgu && pest.pestKonuListe == null))
                ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            TNSCollection pestListe = new TNSCollection();
            while (dr.Read())
            {
                Pest gelen = new Pest();
                gelen.donem = new Donem();
                gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABPest.ENUMAlanlar.DONEMNO, 0);
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABPest.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                gelen.analizYontemi = VeriErisim.DegerAl(dr, (int)TABPest.ENUMAlanlar.ANALIZYONTEMI, string.Empty);

                if (detay || (sorgu && pest.pestKonuListe == null))
                {
                    try
                    {
                        PestDetay d = new PestDetay();
                        d.donem = gelen.donem;
                        d.orgBirimKod = gelen.orgBirimKod;

                        gelen.pestKonuListe = ListeleDetay(ve2, d, sorgu);
                    }
                    catch (Exception)
                    {
                    }
                }

                pestListe.Add(gelen);
            }
            dr.Close();

            if (sorgu && pest.pestKonuListe != null && pest.pestKonuListe.Length == 1)
            {
                PestDetay[] pd = ListeleDetay(ve, pest.pestKonuListe[0], sorgu);
                pestListe = SorguFormVeDetayBirlestir(pestListe, pd);
            }

            ve.VeriErisimKapat();
            if (ve2 != null)
                ve2.VeriErisimKapat();
            return (Pest[])pestListe.ToArray(typeof(Pest));
        }

        private static PestDetay[] ListeleDetay(VeriErisim ve, PestDetay pestDetay, bool sorgu)
        {
            TABPest to = new TABPest(ve);

            IDataReader dr = null;
            try
            {
                dr = to.ListeleDetay(pestDetay, sorgu);
            }
            catch (Exception e)
            {
                throw e;
            }

            TNSCollection liste = new TNSCollection();
            while (dr.Read())
            {
                PestDetay detay = new PestDetay();
                detay.donem = new Donem();
                detay.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABPest.ENUMAlanlarDetay.DONEMNO, 0);
                detay.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABPest.ENUMAlanlarDetay.ORGBIRIMKOD, 0);
                detay.konuNo = VeriErisim.DegerAl(dr, (int)TABPest.ENUMAlanlarDetay.KONUNO, 0);
                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABPest.ENUMAlanlarDetay.SIRANO, 0);
                detay.temelEgilimler = VeriErisim.DegerAl(dr, (int)TABPest.ENUMAlanlarDetay.TEMELEGILIMLER, string.Empty);
                detay.aciklama = VeriErisim.DegerAl(dr, (int)TABPest.ENUMAlanlarDetay.ACIKLAMA, string.Empty);
                detay.iliski = VeriErisim.DegerAl(dr, (int)TABPest.ENUMAlanlarDetay.ILISKI, string.Empty);

                liste.Add(detay);
            }
            dr.Close();

            return (PestDetay[])liste.ToArray(typeof(PestDetay));
        }

        private static TNSCollection SorguFormVeDetayBirlestir(TNSCollection formlar, PestDetay[] detaylar)
        {
            TNSCollection bulunanFormlar = new TNSCollection();

            foreach (Pest form in formlar)
            {
                bool bulundu = false;
                TNSCollection bulunanDetaylar = new TNSCollection();

                foreach (PestDetay detay in detaylar)
                {
                    if (form.donem.donemNo == detay.donem.donemNo && form.orgBirimKod == detay.orgBirimKod)
                    {
                        bulundu = true;
                        bulunanDetaylar.Add(detay);
                    }
                }

                if (bulundu)
                {
                    form.pestKonuListe = (PestDetay[])bulunanDetaylar.ToArray(typeof(PestDetay));
                    bulunanFormlar.Add(form);
                }
            }

            return bulunanFormlar;
        }
    }
}