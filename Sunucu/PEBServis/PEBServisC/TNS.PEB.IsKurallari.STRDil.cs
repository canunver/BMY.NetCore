using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Dil tanımlamalarıyla ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class STRDil
    {
        /// <summary>
        /// Dil varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Dil nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dil">Dil bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Dil dil)
        {
            Sonuc sonuc = YetkiSTR.TanimlamaYazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (string.IsNullOrEmpty(dil.ad))
                hata = PerformansButceResI.STR016 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABDil tDil = new TABDil(ve);
            ve.BeginTransaction();

            if (tDil.DilVarMi(dil.kod))
                sonuc = tDil.Guncelle(dil);
            else if (dil.kod > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR017 + "<br>";
            }
            else
            {
                sonuc = tDil.Kaydet(dil);
                dil.kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar.Trim(), 0);
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
        /// Dil, stratejik planda kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Dil nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dil">Dil bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Dil dil)
        {
            Sonuc sonuc = YetkiSTR.TanimlamaYazmaHakkiVarMi(kullanan);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (dil.kod <= 0)
                hata = PerformansButceResI.STR018 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABDil tDil = new TABDil(ve);
            ve.BeginTransaction();

            if (tDil.DilKullanilmisMi(dil.kod))
            {
                ve.VeriErisimKapat();
                sonuc.hataStr = PerformansButceResI.STR019;
                return sonuc;
            }

            sonuc = tDil.Sil(dil);
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
        /// Verilen kriterlere uygun olan dil bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dil">Dil bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında dil listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Dil dil)
        {
            ObjectArray dilVAN = new ObjectArray();
            //dilVAN.sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan);
            //if (!dilVAN.sonuc.islemSonuc)
            //    return dilVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABDil tDil = new TABDil(ve);

            IDataReader dr = tDil.Listele(dil);
            while (dr.Read())
            {
                Dil gelen = new Dil();
                gelen.kod = VeriErisim.DegerAl(dr, 0, 0);
                gelen.ad = VeriErisim.DegerAl(dr, 1, string.Empty);
                dilVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (dilVAN.objeler.Count == 0)
                dilVAN.sonuc.bilgiStr = PerformansButceResI.STR012;

            ve.VeriErisimKapat();
            return dilVAN;
        }
    }
}