using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary> 
    /// Stratejik plan çalışmalarında gerçekleştirilmesi planlanan çalışmaların belirlenmesi ve yönetilmesi
    /// ile ilgili veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma, silme işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class STRPlanlanan
    {
        /// <summary>
        /// Planlanan Çalışmalar varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen planlanan nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="planlanan">Planlanan Çalışmalar bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Planlanan planlanan, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (planlanan.donem == null || planlanan.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (planlanan.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (planlanan.planlananDetayListe == null || planlanan.planlananDetayListe.Length <= 0)
                hata += PerformansButceResI.STR049 + "<br>";
            else
            {
                string detayHata = string.Empty;
                foreach (PlanlananDetay pd in planlanan.planlananDetayListe)
                {
                    if (string.IsNullOrEmpty(pd.calismaAdimi))
                        detayHata += PerformansButceResI.STR065 + "<br>";
                    if (pd.baslangicTarihi == null)
                        detayHata += PerformansButceResI.STR061 + "<br>";
                    if (pd.bitisTarihi == null)
                        detayHata += PerformansButceResI.STR062 + "<br>";
                    //if (pd.gerBaslangicTarihi.Ticks == tdt.Ticks)
                    //    pd.gerBaslangicTarihi = null; //detayHata += PerformansButceResI.STR063 + "<br>";
                    //if (pd.gerBitisTarihi.Ticks == tdt.Ticks)
                    //    pd.gerBitisTarihi = null; //detayHata += PerformansButceResI.STR064 + "<br>";
                    if (!string.IsNullOrEmpty(detayHata))
                        break;
                }
                if (!string.IsNullOrEmpty(detayHata))
                    hata += detayHata;
            }
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, planlanan.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, planlanan.donem.donemNo, planlanan.orgBirimKod, ENUMSurec.SpHazirlik, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            ve.BeginTransaction();

            Sil(ve, planlanan.donem.donemNo, planlanan.orgBirimKod);
            sonuc = Kaydet(ve, planlanan);

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, planlanan.donem.donemNo, planlanan.orgBirimKod, ENUMSurec.SpHazirlik);

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

        private static Sonuc Kaydet(VeriErisim ve, Planlanan planlanan)
        {
            TABPlanlanan to = new TABPlanlanan(ve);
            Sonuc sonuc = new Sonuc();
            //to.Kaydet(planlanan);

            //if (sonuc.islemSonuc)
            //{
            foreach (PlanlananDetay gd in planlanan.planlananDetayListe)
            {
                sonuc = to.KaydetDetay(planlanan.donem.donemNo, planlanan.orgBirimKod, gd);
                if (!sonuc.islemSonuc)
                    break;
            }
            //}

            return sonuc;
        }

        private static Sonuc Sil(VeriErisim ve, int donemNo, int orgBirimKod)
        {
            TABPlanlanan to = new TABPlanlanan(ve);
            Sonuc sonuc = to.SilDetay(donemNo, orgBirimKod);
            //if (sonuc.islemSonuc)
            //    sonuc = to.Sil(donemNo, orgBirimKod);

            return sonuc;
        }

        /// <summary>
        /// Planlanan Çalışmaları silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen planlanan nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="planlanan">Planlanan Çalışmalar bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Planlanan planlanan, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (planlanan.donem == null || planlanan.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (planlanan.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, planlanan.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, planlanan.donem.donemNo, planlanan.orgBirimKod, ENUMSurec.SpHazirlik, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            ve.BeginTransaction();

            sonuc = Sil(ve, planlanan.donem.donemNo, planlanan.orgBirimKod);
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
        /// Verilen kriterlere uygun olan Planlanan Çalışmalar ayrıntısını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="planlanan">Planlanan Çalışmalar bilgisini tutan nesne</param>
        /// <returns>Bir Planlanan Çalışmalar için Planlanan Çalışmalar ayrıntılarını döndürür</returns>
        public static Planlanan ListeleTek(VeriErisim veriErisim, Kullanici kullanan, Planlanan planlanan)
        {
            Planlanan[] liste = Listele(veriErisim, kullanan, planlanan);
            if (liste.Length > 0)
                return liste[0];
            return null;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan Planlanan Çalışmalar ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="planlanan">Planlanan Çalışmalar bilgisini tutan nesne</param>
        /// <returns>Bir Planlanan Çalışmalar için Planlanan Çalışmalar ayrıntılarını döndürür</returns>
        public static Planlanan[] Listele(VeriErisim veriErisim, Kullanici kullanan, Planlanan planlanan)
        {
            return Listele(veriErisim, kullanan, planlanan, false);
        }

        public static Planlanan[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, Planlanan planlanan)
        {
            string hata = string.Empty;
            if (planlanan.donem == null || planlanan.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (planlanan.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033;
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            if (planlanan.planlananDetayListe != null && planlanan.planlananDetayListe.Length == 1)
            {
                planlanan.planlananDetayListe[0].donem = planlanan.donem;
                planlanan.planlananDetayListe[0].orgBirimKod = planlanan.orgBirimKod;
            }

            return Listele(veriErisim, kullanan, planlanan, true);
        }

        private static Planlanan[] Listele(VeriErisim veriErisim, Kullanici kullanan, Planlanan planlanan, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, planlanan.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPlanlanan to = new TABPlanlanan(ve);

            //IDataReader dr = null;
            //try
            //{
            //    dr = to.Listele(planlanan);
            //}
            //catch (Exception e)
            //{
            //    ve.VeriErisimKapat();
            //    throw e;
            //}

            //VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            TNSCollection planlananListe = new TNSCollection();

            //while (dr.Read())
            //{
            Planlanan gelen = new Planlanan();

            PlanlananDetay pd;
            if (planlanan.planlananDetayListe != null && planlanan.planlananDetayListe.Length == 1)
                pd = planlanan.planlananDetayListe[0];
            else
            {
                pd = new PlanlananDetay();
                pd.donem = new Donem();
                pd.donem.donemNo = planlanan.donem.donemNo;
                pd.orgBirimKod = planlanan.orgBirimKod;
            }
            gelen.planlananDetayListe = ListeleDetay(ve, pd, sorgu);

            gelen.donem = pd.donem;
            gelen.orgBirimKod = pd.orgBirimKod;
            planlananListe.Add(gelen);
            //}
            //dr.Close();

            ve.VeriErisimKapat();
            //ve2.VeriErisimKapat();
            return (Planlanan[])planlananListe.ToArray(typeof(Planlanan));
        }

        private static PlanlananDetay[] ListeleDetay(VeriErisim ve, PlanlananDetay planlananDetay, bool sorgu)
        {
            TNSCollection liste = new TNSCollection();
            TABPlanlanan to = new TABPlanlanan(ve);
            IDataReader dr = to.ListeleDetay(planlananDetay, sorgu);
            while (dr.Read())
            {
                PlanlananDetay detay = new PlanlananDetay();
                detay.donem = new Donem();
                detay.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABPlanlanan.ENUMAlanlarDetay.DONEMNO, 0);
                detay.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABPlanlanan.ENUMAlanlarDetay.ORGBIRIMKOD, 0);
                detay.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", detay.orgBirimKod.ToString(), true, string.Empty);
                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABPlanlanan.ENUMAlanlarDetay.SIRANO, 0);
                detay.calismaAdimi = VeriErisim.DegerAl(dr, (int)TABPlanlanan.ENUMAlanlarDetay.CALISMAADIMI, string.Empty);
                detay.baslangicTarihi = VeriErisim.DegerAl(dr, (int)TABPlanlanan.ENUMAlanlarDetay.BASLANGICTARIH);
                detay.bitisTarihi = VeriErisim.DegerAl(dr, (int)TABPlanlanan.ENUMAlanlarDetay.BITISTARIH);
                detay.gerBaslangicTarihi = VeriErisim.DegerAl(dr, (int)TABPlanlanan.ENUMAlanlarDetay.GERBASLANGICTARIH);
                detay.gerBitisTarihi = VeriErisim.DegerAl(dr, (int)TABPlanlanan.ENUMAlanlarDetay.GERBITISTARIH);

                liste.Add(detay);
            }
            dr.Close();

            return (PlanlananDetay[])liste.ToArray(typeof(PlanlananDetay));
        }
    }
}