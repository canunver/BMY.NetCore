using System;
using System.Data;
using TNS.KYM;
using TNS.PEB.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary> 
    /// Dönem tanımlanması ile ilgili veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma, silme işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class STRDonem
    {
        /// <summary>
        /// Dönem varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen donem nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donem">Dönem bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Donem donem)
        {
            Sonuc sonuc = YetkiSTR.TanimlamaYazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (donem.baslamaYili <= 0)
                hata = PerformansButceResI.STR038 + "<br>";
            if (donem.bitisYili <= 0)
                hata += PerformansButceResI.STR039 + "<br>";
            if (donem.bitisYili < donem.baslamaYili)
                hata += PerformansButceResI.STR142 + "<br>";
            if (donem.pasif && string.IsNullOrEmpty(donem.pasifAciklama))
                hata += PerformansButceResI.STR052 + "<br>";
            if (string.IsNullOrEmpty(donem.donemAd))
                hata += PerformansButceResI.STR150 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABDonem tDonem = new TABDonem(ve);
            ve.BeginTransaction();

            if (donem.donemNo > 0 && tDonem.DonemVarMi(donem.donemNo) && !tDonem.DonemVarMi(donem, false))
                sonuc = tDonem.Guncelle(donem);
            else if (donem.donemNo <= 0 && !tDonem.DonemVarMi(donem, false))
            {
                donem.versiyon = tDonem.VersiyonNoAl(donem.baslamaYili, donem.bitisYili);
                sonuc = tDonem.Kaydet(donem);
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR092;
            }

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
        /// Dönemi silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen donem nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donem">Dönem bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Donem donem)
        {
            Sonuc sonuc = YetkiSTR.TanimlamaYazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (donem.donemNo <= 0)
                hata = PerformansButceResI.STR032;
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABDonem tDonem = new TABDonem(ve);
            ve.BeginTransaction();

            sonuc = tDonem.Sil(donem);
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
        /// Verilen kriterlere uygun olan Dönem ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donem">Dönem bilgisini tutan nesne</param>
        /// <returns>Bir Dönem için Dönem ayrıntılarını döndürür</returns>
        public static Donem[] Listele(VeriErisim veriErisim, Kullanici kullanan, Donem donem)
        {
            TNSCollection donemler = new TNSCollection();
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABDonem tDonem = new TABDonem(ve);

            IDataReader dr = tDonem.Listele(donem);

            if (dr != null)
            {
                while (dr.Read())
                {
                    Donem gelen = new Donem();
                    gelen.donemNo = VeriErisim.DegerAl(dr, (int)TABDonem.ENUMAlanlar.DONEMNO, 0);
                    gelen.baslamaYili = VeriErisim.DegerAl(dr, (int)TABDonem.ENUMAlanlar.BASLAMAYIL, 0);
                    gelen.bitisYili = VeriErisim.DegerAl(dr, (int)TABDonem.ENUMAlanlar.BITISYIL, 0);
                    gelen.pasif = VeriErisim.DegerAl(dr, (int)TABDonem.ENUMAlanlar.PASIF, 0) > 0 ? true : false;
                    gelen.pasifAciklama = VeriErisim.DegerAl(dr, (int)TABDonem.ENUMAlanlar.PASIFACIKLAMA, string.Empty);
                    gelen.versiyon = VeriErisim.DegerAl(dr, (int)TABDonem.ENUMAlanlar.VERSIYON, 0);
                    gelen.donemAd = VeriErisim.DegerAl(dr, (int)TABDonem.ENUMAlanlar.DONEMAD, string.Empty);

                    donemler.Add(gelen);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return (Donem[])donemler.ToArray(typeof(Donem));
        }

        public static int AktifDonemBul(VeriErisim veriErisim, int yil)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(null));
            TABDonem tDonem = new TABDonem(ve);
            return tDonem.AktifDonemBul(yil);
        }
    }
}