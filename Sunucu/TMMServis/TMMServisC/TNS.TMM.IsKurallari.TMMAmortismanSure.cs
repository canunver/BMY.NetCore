using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    ///// <summary>
    ///// Taşınır malzemelerinin amortisman süre bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    ///// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    ///// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    ///// </summary>
    //public class TMMAmortismanSure
    //{
    //    /// <summary>
    //    /// Taşınır malzemelerinin amortisman sürelerini varsa güncelleyen, yoksa kaydeden yordam
    //    /// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanSure nesnesi eksikse işlemi yapmaz ve hata döndürür.
    //    /// </summary>
    //    /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
    //    /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
    //    /// <param name="sure">Amortisman süre bilgisini tutan nesne</param>
    //    /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
    //    public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, AmortismanSure sure)
    //    {
    //        Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

    //        if (!TMMYetkiKontrol.Yoneticimi(kullanan))
    //        {
    //            sonuc.hataStr = TasinirMalResI.TMM030;
    //            sonuc.islemSonuc = false;
    //            return sonuc;
    //        }

    //        string hata = string.Empty;
    //        if (sure.yil <= 0)
    //            hata = TasinirMalResI.TMM036 + "<br>";
    //        if (string.IsNullOrEmpty(sure.hesapPlanKod))
    //            hata += TasinirMalResI.TMM027 + "<br>";
    //        else if (sure.hesapPlanKod.Replace(".", "").Length != 7)
    //            hata += TasinirMalResI.TMM072 + "<br>";
    //        if (sure.sure <= 0)
    //            hata += TasinirMalResI.TMM073;
    //        if (!string.IsNullOrEmpty(hata))
    //        {
    //            sonuc.islemSonuc = false;
    //            sonuc.hataStr = hata;
    //            return sonuc;
    //        }

    //        VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

    //        if (TMMAmortisman.AmortismanYapilmisMi(ve, sure.yil, sure.hesapPlanKod))
    //        {
    //            sonuc.islemSonuc = false;
    //            sonuc.hataStr = TMMAmortisman.amortismanHata;
    //            ve.VeriErisimKapat();
    //            return sonuc;
    //        }

    //        TABAmortismanSure tSure = new TABAmortismanSure(ve);
    //        ve.BeginTransaction();

    //        try
    //        {
    //            sonuc = tSure.Guncelle(sure);
    //            if (!sonuc.islemSonuc)
    //                sonuc = tSure.Kaydet(sure);
    //        }
    //        catch (Exception e)
    //        {
    //            sonuc.islemSonuc = false;
    //            sonuc.hataStr = e.Message;
    //        }

    //        if (sonuc.islemSonuc)
    //            ve.Commit();
    //        else
    //            ve.Rollback();

    //        ve.VeriErisimKapat();
    //        return sonuc;
    //    }

    //    /// <summary>
    //    /// Taşınır malzemesi, verilen yılda amortisman işlemlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
    //    /// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanSure nesnesi eksikse işlemi yapmaz ve hata döndürür.
    //    /// </summary>
    //    /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
    //    /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
    //    /// <param name="sure">Amortisman süre bilgisini tutan nesne</param>
    //    /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
    //    public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, AmortismanSure sure)
    //    {
    //        Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM071, "");

    //        if (!TMMYetkiKontrol.Yoneticimi(kullanan))
    //        {
    //            sonuc.hataStr = TasinirMalResI.TMM030;
    //            sonuc.islemSonuc = false;
    //            return sonuc;
    //        }

    //        string hata = string.Empty;
    //        if (sure.yil <= 0)
    //            hata = TasinirMalResI.TMM036 + "<br>";
    //        if (string.IsNullOrEmpty(sure.hesapPlanKod))
    //            hata += TasinirMalResI.TMM027;
    //        else if (sure.hesapPlanKod.Replace(".", "").Length != 7)
    //            hata += TasinirMalResI.TMM072 + "<br>";
    //        if (!string.IsNullOrEmpty(hata))
    //        {
    //            sonuc.islemSonuc = false;
    //            sonuc.hataStr = hata;
    //            return sonuc;
    //        }

    //        VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

    //        if (TMMAmortisman.AmortismanYapilmisMi(ve, sure.yil, sure.hesapPlanKod))
    //        {
    //            sonuc.islemSonuc = false;
    //            sonuc.hataStr = TMMAmortisman.amortismanHata;
    //            ve.VeriErisimKapat();
    //            return sonuc;
    //        }

    //        TABAmortismanSure tSure = new TABAmortismanSure(ve);
    //        ve.BeginTransaction();

    //        try
    //        {
    //            sonuc = tSure.Sil(sure);
    //        }
    //        catch (Exception e)
    //        {
    //            sonuc.islemSonuc = false;
    //            sonuc.hataStr = e.Message;
    //        }

    //        if (sonuc.islemSonuc)
    //            ve.Commit();
    //        else
    //            ve.Rollback();

    //        ve.VeriErisimKapat();
    //        return sonuc;
    //    }

    //    /// <summary>
    //    /// Verilen kriterlere uygun olan taşınır malzemelerinin amortisman süre bilgilerini döndüren yordam
    //    /// </summary>
    //    /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
    //    /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
    //    /// <param name="sure">Amortisman süre bilgisini tutan nesne</param>
    //    /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında amortisman süre bilgileri listesi döndürülür.</returns>
    //    public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, AmortismanSure sure)
    //    {
    //        ObjectArray listeVAN = new ObjectArray();

    //        VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
    //        TABAmortismanSure tSure = new TABAmortismanSure(ve);
    //        IDataReader dr = null;

    //        try
    //        {
    //            dr = tSure.Listele(sure);
    //        }
    //        catch (Exception e)
    //        {
    //            listeVAN.sonuc.islemSonuc = false;
    //            listeVAN.sonuc.hataStr = e.Message;
    //            dr.Close();
    //            ve.VeriErisimKapat();
    //            return listeVAN;
    //        }

    //        System.Collections.Hashtable isimler = new System.Collections.Hashtable();
    //        while (dr.Read())
    //        {
    //            AmortismanSure gelen = new AmortismanSure();
    //            gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
    //            gelen.hesapPlanKod = VeriErisim.DegerAl(dr, 1, string.Empty);
    //            gelen.sure = VeriErisim.DegerAl(dr, 2, 0);

    //            gelen.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), gelen.hesapPlanKod);
    //            gelen.hesapPlanAd = Genel.AdGetir(isimler, gelen.hesapPlanKod, "TASHESAPPLAN");

    //            listeVAN.objeler.Add(gelen);
    //        }
    //        dr.Close();

    //        if (listeVAN.objeler.Count == 0)
    //            listeVAN.sonuc.bilgiStr = TasinirMalResI.TMM029;

    //        listeVAN.sonuc.islemSonuc = true;

    //        ve.VeriErisimKapat();
    //        return listeVAN;
    //    }
    //}
}