using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary> 
    /// Stratejik plan mevcut durum analizinde belirlenmiş hizmetler ve paydaşlar arasında ilişki kurulması ve yönetilmesi 
    /// ile ilgili veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma, silme işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class STRBirimHizmetPaydasIliski
    {
        /// <summary>
        /// Hizmet-Paydaş ilişkisi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen bhpiler nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="bhpiler">Hizmet-Paydaş ilişkisi bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Birim_SpGenelHavuz_Paydas_Iliski[] bhpiler, string formId)
        {
            Sonuc sonuc = new Sonuc();

            if (bhpiler == null || bhpiler.Length <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR046;
                return sonuc;
            }

            foreach (Birim_SpGenelHavuz_Paydas_Iliski bhpi in bhpiler)
            {
                string hata = string.Empty;
                if (bhpi.donem == null || bhpi.donem.donemNo <= 0)
                    hata = PerformansButceResI.STR032 + "<br>";
                if (bhpi.orgBirimKod <= 0)
                    hata += PerformansButceResI.STR033 + "<br>";
                if (bhpi.spGenelHavuz == null || string.IsNullOrEmpty(bhpi.spGenelHavuz.kod))
                    hata += PerformansButceResI.STR048 + "<br>";
                if (bhpi.paydas == null || bhpi.paydas.siraNo <= 0 || bhpi.paydas.sinif <= 0)
                    hata += PerformansButceResI.STR047 + "<br>";
                if (bhpi.yararlanma < 1)
                    hata += PerformansButceResI.STR060 + "<br>";
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }
            }

            for (int i = 0; i < bhpiler.Length; i++)
            {
                for (int j = i + 1; j < bhpiler.Length; j++)
                {
                    if (bhpiler[i].paydas.siraNo == bhpiler[j].paydas.siraNo &&
                        bhpiler[i].paydas.sinif == bhpiler[j].paydas.sinif &&
                        bhpiler[i].spGenelHavuz.kod.Equals(bhpiler[j].spGenelHavuz.kod))
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = string.Format(PerformansButceResI.STR066, i + 1, j + 1);
                        return sonuc;
                    }
                }
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, bhpiler[0].orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, bhpiler[0].donem.donemNo, bhpiler[0].orgBirimKod, ENUMSurec.MevcutDurumAnalizi, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimHizmetPaydasIliski tBHPI = new TABBirimHizmetPaydasIliski(ve);
            ve.BeginTransaction();

            tBHPI.Sil(bhpiler[0]);
            foreach (Birim_SpGenelHavuz_Paydas_Iliski bhpi in bhpiler)
            {
                sonuc = tBHPI.Kaydet(bhpi);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, bhpiler[0].donem.donemNo, bhpiler[0].orgBirimKod, ENUMSurec.MevcutDurumAnalizi);

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

        /// <summary>
        /// Hizmet-Paydaş ilişkisini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen bhpi nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="bhpi">Hizmet-Paydaş ilişkisi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Birim_SpGenelHavuz_Paydas_Iliski bhpi, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (bhpi.donem == null || bhpi.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (bhpi.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, bhpi.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, bhpi.donem.donemNo, bhpi.orgBirimKod, ENUMSurec.MevcutDurumAnalizi, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimHizmetPaydasIliski tBHPI = new TABBirimHizmetPaydasIliski(ve);
            ve.BeginTransaction();

            sonuc = tBHPI.Sil(bhpi);
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
        /// Verilen kriterlere uygun olan Hizmet-Paydaş ilişkisi ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="bhpi">Hizmet-Paydaş ilişkisi bilgisini tutan nesne</param>
        /// <returns>Bir Hizmet-Paydaş ilişkisi için Hizmet-Paydaş ilişkisi ayrıntılarını döndürür</returns>
        public static Birim_SpGenelHavuz_Paydas_Iliski[] Listele(VeriErisim veriErisim, Kullanici kullanan, Birim_SpGenelHavuz_Paydas_Iliski bhpi)
        {
            return Listele(veriErisim, kullanan, bhpi, false);
        }

        public static Birim_SpGenelHavuz_Paydas_Iliski[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, Birim_SpGenelHavuz_Paydas_Iliski bhpi)
        {
            string hata = string.Empty;
            if (bhpi.donem == null || bhpi.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (bhpi.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033;
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            return Listele(veriErisim, kullanan, bhpi, true);
        }

        private static Birim_SpGenelHavuz_Paydas_Iliski[] Listele(VeriErisim veriErisim, Kullanici kullanan, Birim_SpGenelHavuz_Paydas_Iliski bhpi, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, bhpi.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimHizmetPaydasIliski tBHPI = new TABBirimHizmetPaydasIliski(ve);

            IDataReader dr = null;
            try
            {
                dr = tBHPI.Listele(bhpi, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection bhpiler = new TNSCollection();
            while (dr.Read())
            {
                Birim_SpGenelHavuz_Paydas_Iliski gelen = new Birim_SpGenelHavuz_Paydas_Iliski();
                gelen.donem = new Donem();
                gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABBirimHizmetPaydasIliski.ENUMAlanlar.DONEMNO, 0);
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimHizmetPaydasIliski.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                gelen.paydas = new PaydasAnalizDetay();
                gelen.paydas.siraNo = VeriErisim.DegerAl(dr, (int)TABBirimHizmetPaydasIliski.ENUMAlanlar.PAYDASSIRANO, 0);
                gelen.paydas.sinif = VeriErisim.DegerAl(dr, (int)TABBirimHizmetPaydasIliski.ENUMAlanlar.PAYDASSINIF, 0);
                gelen.paydas.ad = PEBServis.UzayYaziDegerGetir("STRPAYDASANALIZDETAY", gelen.donem.donemNo.ToString() + "-" + gelen.orgBirimKod.ToString() + "-" + gelen.paydas.siraNo.ToString() + "-" + gelen.paydas.sinif.ToString(), true, string.Empty);
                gelen.spGenelHavuz = new SpGenelHavuz();
                gelen.spGenelHavuz.kod = VeriErisim.DegerAl(dr, (int)TABBirimHizmetPaydasIliski.ENUMAlanlar.HIZMETKOD, string.Empty);
                gelen.spGenelHavuz.absKod = new BKD.ABSKod(gelen.spGenelHavuz.kod);
                gelen.spGenelHavuz.absKod.AdAta(PEBServis.UzayYaziDegerGetir("STRGENELHAVUZ", gelen.spGenelHavuz.absKod.kod, true, string.Empty));
                gelen.yararlanma = VeriErisim.DegerAl(dr, (int)TABBirimHizmetPaydasIliski.ENUMAlanlar.YARARLANMA, 0);
                gelen.aciklama = VeriErisim.DegerAl(dr, (int)TABBirimHizmetPaydasIliski.ENUMAlanlar.ACIKLAMA, string.Empty);

                bhpiler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (Birim_SpGenelHavuz_Paydas_Iliski[])bhpiler.ToArray(typeof(Birim_SpGenelHavuz_Paydas_Iliski));
        }
    }
}