using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary> 
    /// Stratejik plan mevcut durum analizinde belirlenecek olan Paydaşların yönetilmesi
    /// ile ilgili veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma, silme işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class STRPaydasAnaliz
    {
        /// <summary>
        /// Paydaş varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen paydasAnaliz nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="paydasAnaliz">Paydaş bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, PaydasAnaliz paydasAnaliz, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (paydasAnaliz.donem == null || paydasAnaliz.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (paydasAnaliz.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (string.IsNullOrEmpty(paydasAnaliz.analizYontemi))
                hata += PerformansButceResI.STR042 + "<br>";
            if (paydasAnaliz.paydasAnalizDetayListe == null || paydasAnaliz.paydasAnalizDetayListe.Length <= 0)
                hata += PerformansButceResI.STR050;
            else
            {
                foreach (PaydasAnalizDetay padetay in paydasAnaliz.paydasAnalizDetayListe)
                {
                    if (string.IsNullOrEmpty(padetay.ad))
                        hata += PerformansButceResI.STR053 + "<br>";
                    //if (string.IsNullOrEmpty(padetay.aciklama))
                    //    hata += PerformansButceResI.STR054 + "<br>";
                    if (string.IsNullOrEmpty(padetay.kategoriKod))
                        hata += PerformansButceResI.STR055 + "<br>";
                    if (padetay.oncelikKod < 1)
                        hata += PerformansButceResI.STR056 + "<br>";
                    if (padetay.etkiKod < 1)
                        hata += PerformansButceResI.STR057 + "<br>";
                    if (padetay.stratejiKod < 1)
                        hata += PerformansButceResI.STR058;
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

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, paydasAnaliz.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, paydasAnaliz.donem.donemNo, paydasAnaliz.orgBirimKod, ENUMSurec.MevcutDurumAnalizi, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            ve.BeginTransaction();

            sonuc = Sil(ve, paydasAnaliz.donem.donemNo, paydasAnaliz.orgBirimKod);
            if (sonuc.islemSonuc)
                sonuc = Kaydet(ve, paydasAnaliz);

            if (sonuc.islemSonuc)
            {
                TABBirimHizmetPaydasIliski tabBHPI = new TABBirimHizmetPaydasIliski(ve);
                sonuc = tabBHPI.PaydasiOlmayanlariSil(paydasAnaliz.donem.donemNo, paydasAnaliz.orgBirimKod);
            }

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, paydasAnaliz.donem.donemNo, paydasAnaliz.orgBirimKod, ENUMSurec.MevcutDurumAnalizi);

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

        private static Sonuc Kaydet(VeriErisim ve, PaydasAnaliz paydasAnaliz)
        {
            TABPaydasAnaliz to = new TABPaydasAnaliz(ve);
            Sonuc sonuc = to.Kaydet(paydasAnaliz);
            if (sonuc.islemSonuc)
                sonuc = to.KaydetDetay(paydasAnaliz.donem.donemNo, paydasAnaliz.orgBirimKod, paydasAnaliz.paydasAnalizDetayListe);

            return sonuc;
        }

        private static Sonuc Sil(VeriErisim ve, int donemNo, int orgBirimKod)
        {
            TABPaydasAnaliz to = new TABPaydasAnaliz(ve);
            Sonuc sonuc = to.SilDetay(donemNo, orgBirimKod);
            if (sonuc.islemSonuc)
                sonuc = to.Sil(donemNo, orgBirimKod);
            else
                return sonuc;

            return sonuc;
        }

        /// <summary>
        /// Paydaş silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen paydasAnaliz nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="paydasAnaliz">Paydaş bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, PaydasAnaliz paydasAnaliz, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (paydasAnaliz.donem == null || paydasAnaliz.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (paydasAnaliz.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, paydasAnaliz.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, paydasAnaliz.donem.donemNo, paydasAnaliz.orgBirimKod, ENUMSurec.MevcutDurumAnalizi, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            ve.BeginTransaction();

            sonuc = Sil(ve, paydasAnaliz.donem.donemNo, paydasAnaliz.orgBirimKod);
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
        /// Verilen kriterlere uygun olan Paydaş ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="paydasAnaliz">Paydaş bilgisini tutan nesne</param>
        /// <returns>Bir Paydaş için Paydaş ayrıntısını döndürür</returns>
        public static PaydasAnaliz ListeleTek(VeriErisim veriErisim, Kullanici kullanan, PaydasAnaliz paydasAnaliz)
        {
            PaydasAnaliz[] liste = Listele(veriErisim, kullanan, paydasAnaliz, true);
            if (liste.Length > 0)
                return liste[0];
            return null;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan Paydaş ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="paydasAnaliz">Paydaş bilgisini tutan nesne</param>
        /// <returns>Bir Paydaş için Paydaş ayrıntılarını döndürür</returns>
        public static PaydasAnaliz[] Listele(VeriErisim veriErisim, Kullanici kullanan, PaydasAnaliz paydasAnaliz, bool detay)
        {
            return Listele(veriErisim, kullanan, paydasAnaliz, detay, false);
        }

        public static PaydasAnaliz[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, PaydasAnaliz paydasAnaliz)
        {
            string hata = string.Empty;
            if (paydasAnaliz.donem == null || paydasAnaliz.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (paydasAnaliz.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033;
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            if (paydasAnaliz.paydasAnalizDetayListe != null && paydasAnaliz.paydasAnalizDetayListe.Length == 1)
            {
                paydasAnaliz.paydasAnalizDetayListe[0].donem = paydasAnaliz.donem;
                paydasAnaliz.paydasAnalizDetayListe[0].orgBirimKod = paydasAnaliz.orgBirimKod;
            }

            return Listele(veriErisim, kullanan, paydasAnaliz, false, true);
        }

        private static PaydasAnaliz[] Listele(VeriErisim veriErisim, Kullanici kullanan, PaydasAnaliz paydasAnaliz, bool detay, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, paydasAnaliz.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPaydasAnaliz to = new TABPaydasAnaliz(ve);

            IDataReader dr = null;
            try
            {
                dr = to.Listele(paydasAnaliz, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            VeriErisim ve2 = null;
            if (detay || (sorgu && paydasAnaliz.paydasAnalizDetayListe == null))
                ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            TNSCollection paydasAnalizListe = new TNSCollection();
            while (dr.Read())
            {
                PaydasAnaliz gelen = new PaydasAnaliz();
                gelen.donem = new Donem();
                gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABPaydasAnaliz.ENUMAlanlar.DONEMNO, 0);
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABPaydasAnaliz.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                gelen.analizYontemi = VeriErisim.DegerAl(dr, (int)TABPaydasAnaliz.ENUMAlanlar.ANALIZYONTEMI, string.Empty);

                if (detay || (sorgu && paydasAnaliz.paydasAnalizDetayListe == null))
                {
                    try
                    {
                        PaydasAnalizDetay d = new PaydasAnalizDetay();
                        d.donem = gelen.donem;
                        d.orgBirimKod = gelen.orgBirimKod;

                        gelen.paydasAnalizDetayListe = ListeleDetay(ve2, d, sorgu);
                    }
                    catch (Exception)
                    {
                    }
                }

                paydasAnalizListe.Add(gelen);
            }
            dr.Close();

            if (sorgu && paydasAnaliz.paydasAnalizDetayListe != null && paydasAnaliz.paydasAnalizDetayListe.Length == 1)
            {
                PaydasAnalizDetay[] pd = ListeleDetay(ve, paydasAnaliz.paydasAnalizDetayListe[0], sorgu);
                paydasAnalizListe = SorguFormVeDetayBirlestir(paydasAnalizListe, pd);
            }

            ve.VeriErisimKapat();
            if (ve2 != null)
                ve2.VeriErisimKapat();
            return (PaydasAnaliz[])paydasAnalizListe.ToArray(typeof(PaydasAnaliz));
        }

        private static PaydasAnalizDetay[] ListeleDetay(VeriErisim ve, PaydasAnalizDetay paydasAnalizDetay, bool sorgu)
        {
            TNSCollection liste = new TNSCollection();

            TABPaydasAnaliz to = new TABPaydasAnaliz(ve);

            IDataReader dr = to.ListeleDetay(paydasAnalizDetay, sorgu);
            while (dr.Read())
            {
                PaydasAnalizDetay detay = new PaydasAnalizDetay();
                detay.donem = new Donem();
                detay.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABPaydasAnaliz.ENUMAlanlarDetay.DONEMNO, 0);
                detay.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABPaydasAnaliz.ENUMAlanlarDetay.ORGBIRIMKOD, 0);
                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABPaydasAnaliz.ENUMAlanlarDetay.SIRANO, 0);
                detay.sinif = VeriErisim.DegerAl(dr, (int)TABPaydasAnaliz.ENUMAlanlarDetay.SINIF, 0);
                detay.ad = VeriErisim.DegerAl(dr, (int)TABPaydasAnaliz.ENUMAlanlarDetay.AD, string.Empty);
                detay.aciklama = VeriErisim.DegerAl(dr, (int)TABPaydasAnaliz.ENUMAlanlarDetay.ACIKLAMA, string.Empty);
                detay.kategoriKod = VeriErisim.DegerAl(dr, (int)TABPaydasAnaliz.ENUMAlanlarDetay.KATEGORI, string.Empty);
                detay.oncelikKod = VeriErisim.DegerAl(dr, (int)TABPaydasAnaliz.ENUMAlanlarDetay.ONCELIK, 0);
                detay.etkiKod = VeriErisim.DegerAl(dr, (int)TABPaydasAnaliz.ENUMAlanlarDetay.ETKI, 0);
                detay.stratejiKod = VeriErisim.DegerAl(dr, (int)TABPaydasAnaliz.ENUMAlanlarDetay.STRATEJI, 0);

                liste.Add(detay);
            }
            dr.Close();

            return (PaydasAnalizDetay[])liste.ToArray(typeof(PaydasAnalizDetay));
        }

        private static TNSCollection SorguFormVeDetayBirlestir(TNSCollection formlar, PaydasAnalizDetay[] detaylar)
        {
            TNSCollection bulunanFormlar = new TNSCollection();

            foreach (PaydasAnaliz form in formlar)
            {
                bool bulundu = false;
                TNSCollection bulunanDetaylar = new TNSCollection();

                foreach (PaydasAnalizDetay detay in detaylar)
                {
                    if (form.donem.donemNo == detay.donem.donemNo && form.orgBirimKod == detay.orgBirimKod)
                    {
                        bulundu = true;
                        bulunanDetaylar.Add(detay);
                    }
                }

                if (bulundu)
                {
                    form.paydasAnalizDetayListe = (PaydasAnalizDetay[])bulunanDetaylar.ToArray(typeof(PaydasAnalizDetay));
                    bulunanFormlar.Add(form);
                }
            }

            return bulunanFormlar;
        }
    }
}