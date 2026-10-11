using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.KYM;
using TNS.Veri;
using TNS.YAT.IsKurallari.Veri;
using TNS.BKD;
using TNS.BHMv2;

namespace TNS.YAT.IsKurallari
{
    /// <summary>
    /// Bütçe kaynağın ödenek teklif bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class ButceKaynakOdenekClass
    {
        ///// <summary>
        ///// Bütçe kaynağın türüne göre ödenek teklif bilgilerinin yatırım modülünden
        ///// girilip girilemeyeceğini kontrol eden yordam
        ///// Eğer bütçe kaynağın türü bütçede gösterilen cinsten ise Sonuc yapısında false döndürür.
        ///// Çünkü, bu durumda ödenek teklif verileri bütçe modüllerinden aktarılacak demektir.
        ///// Eğer bütçe kaynağın türü bütçede yer almayan cinsten ise Sonuc yapısında true döndürür ve
        ///// ödenek teklif bilgileri yatırım modülünden girilecek demektir.
        ///// Bu kural, sadece yatırım ve bütçe hazırlık modüllerinin entegrasyonundan sonrası için çalışır.
        ///// </summary>
        ///// <param name="odenek">ButceKaynakOdenek nesnelerinden oluşan listeyi tutan nesne</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //private static Sonuc KontrolEt(TNSCollection odenek)
        //{
        //    Sonuc s = new Sonuc();

        //    //int kaynakTur;
        //    //foreach (ButceKaynakOdenek o in odenek)
        //    //{
        //    //    kaynakTur = OrtakFonksiyonlar.ConvertToInt(YATServis.UzayYaziDegerGetir("YATBUTCEKAYNAK", o.kaynakNo.ToString(), true, "STR(KAYNAKTUR)"), 0);
        //    //    if (!Genel.KaynakButceDisiMi(o.yil, kaynakTur))
        //    //    {
        //    //        foreach (double d in o.odenek)
        //    //        {
        //    //            if (d > 0)
        //    //            {
        //    //                s.islemSonuc = false;
        //    //                s.hataStr = string.Format(YatirimResI.YAT031, o.yil.ToString());
        //    //                return s;
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //    return s;
        //}

        ///// <summary>
        ///// Bütçe kaynağın odenek teklif bilgilerini kaydeden yordam
        ///// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        ///// ButceKaynakOdenek nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="odenek">ButceKaynakOdenek nesnelerinden oluşan listeyi tutan nesne</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection odenek)
        //{
        //    return Kaydet(veriErisim, kullanan, odenek, false);
        //}

        ///// <summary>
        ///// Bütçe kaynağın odenek teklif bilgilerini kaydeden yordam
        ///// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        ///// ButceKaynakOdenek nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="odenek">ButceKaynakOdenek nesnelerinden oluşan listeyi tutan nesne</param>
        ///// <param name="aktarim">Kayıt isteğinin aktarımdan gelip gelmediğini belirten parametre</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //private static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection odenek, bool aktarim)
        //{
        //    Sonuc donusDeger = null;
        //    //if (!aktarim)
        //    //{
        //    //    donusDeger = Yetki.YazmaHakkiVarMi(kullanan);
        //    //    if (!donusDeger.islemSonuc)
        //    //        return donusDeger;

        //    //    donusDeger = KontrolEt(odenek);
        //    //    if (!donusDeger.islemSonuc)
        //    //        return donusDeger;
        //    //}
        //    //else
        //    //{
        //    //    donusDeger = Yetki.YazmaHakkiVarMi(kullanan);
        //    //    if (!(donusDeger.islemSonuc || kullanan.ad == "aktarim"))
        //    //        return donusDeger;
        //    //    else
        //    //        donusDeger = new Sonuc();
        //    //}

        //    //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
        //    //TABYATButceKaynakOdenek tabKO = new TABYATButceKaynakOdenek(ve);
        //    //ve.BeginTransaction();

        //    //try
        //    //{
        //    //    foreach (ButceKaynakOdenek o in odenek)
        //    //    {
        //    //        if (o.kaynakNo <= 0)
        //    //        {
        //    //            donusDeger.islemSonuc = false;
        //    //            donusDeger.hataStr = YatirimResI.YAT023;
        //    //            break;
        //    //        }
        //    //        if (!Genel.ButceKaynakVarMi(ve, o.kaynakNo))
        //    //        {
        //    //            donusDeger.islemSonuc = false;
        //    //            donusDeger.hataStr = YatirimResI.YAT026;
        //    //            break;
        //    //        }

        //    //        tabKO.Sil(o);
        //    //        for (int i = 0; i < o.odenek.Length; i++)
        //    //        {
        //    //            if (o.odenek[i] > 0)
        //    //            {
        //    //                donusDeger = tabKO.Kaydet(o, i);
        //    //                if (!donusDeger.islemSonuc)
        //    //                    break;
        //    //            }
        //    //        }
        //    //        if (!donusDeger.islemSonuc)
        //    //            break;
        //    //    }
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    donusDeger.islemSonuc = false;
        //    //    donusDeger.hataStr = e.Message;
        //    //}

        //    //if (donusDeger.islemSonuc)
        //    //    ve.Commit();
        //    //else
        //    //    ve.Rollback();

        //    //ve.VeriErisimKapat();
        //    return donusDeger;
        //}

        ///// <summary>
        ///// Verilen bütçe kaynağa ve senaryoya uygun olan ödenek teklif bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="kaynakNo">Ödenek teklif bilgileri istenen bütçe kaynağın programatik kodu</param>
        ///// <param name="senaryoKod">Senaryo kriteri</param>
        ///// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ButceKaynakOdenek nesneleri döndürülür.</returns>
        //public static BilgiVAN Listele(VeriErisim veriErisim, Kullanici kullanan, int kaynakNo, string senaryoKod)
        //{
        //    return Listele(veriErisim, kullanan, kaynakNo, 0, senaryoKod);
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan ödenek teklif bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="kaynakNo">Ödenek teklif bilgileri istenen bütçe kaynağın programatik kodu</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="senaryoKod">Senaryo kriteri</param>
        ///// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ButceKaynakOdenek nesneleri döndürülür.</returns>
        //public static BilgiVAN Listele(VeriErisim veriErisim, Kullanici kullanan, int kaynakNo, int yil, string senaryoKod)
        //{
        //    return Listele(veriErisim, kullanan, kaynakNo, yil, senaryoKod, 0);
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan ödenek teklif bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="kaynakNo">Ödenek teklif bilgileri istenen bütçe kaynağın programatik kodu</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="senaryoKod">Senaryo kriteri</param>
        ///// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        ///// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ButceKaynakOdenek nesneleri döndürülür.</returns>
        //public static BilgiVAN Listele(VeriErisim veriErisim, Kullanici kullanan, int kaynakNo, int yil, string senaryoKod, int projeKodu)
        //{
        //    BilgiVAN donusDeger = new BilgiVAN();

        //    //donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
        //    //if (!donusDeger.sonuc.islemSonuc)
        //    //    return donusDeger;

        //    //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
        //    //TABYATButceKaynakOdenek tabKO = new TABYATButceKaynakOdenek(ve);
        //    //IDataReader dr = null;

        //    //try
        //    //{
        //    //    dr = tabKO.Listele(kaynakNo, yil, senaryoKod, projeKodu, Yetki.BirimKodlariDondur(kullanan));
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    donusDeger.sonuc.islemSonuc = false;
        //    //    donusDeger.sonuc.hataStr = e.Message;
        //    //    ve.VeriErisimKapat();
        //    //    return donusDeger;
        //    //}

        //    //while (dr.Read())
        //    //{
        //    //    ButceKaynakOdenek gelen = null;
        //    //    foreach (ButceKaynakOdenek o in donusDeger.liste)
        //    //    {
        //    //        if (o.kaynakNo == VeriErisim.DegerAl(dr, 0, -1) &&
        //    //            o.yil == VeriErisim.DegerAl(dr, 1, -1) &&
        //    //            o.senaryoKod == VeriErisim.DegerAl(dr, 2, string.Empty))
        //    //        {
        //    //            gelen = o;
        //    //            break;
        //    //        }
        //    //    }

        //    //    if (gelen == null)
        //    //    {
        //    //        gelen = new ButceKaynakOdenek();
        //    //        donusDeger.liste.Add(gelen);
        //    //    }

        //    //    gelen.kaynakNo = VeriErisim.DegerAl(dr, 0, -1);
        //    //    gelen.yil = VeriErisim.DegerAl(dr, 1, 0);
        //    //    gelen.senaryoKod = VeriErisim.DegerAl(dr, 2, string.Empty);

        //    //    int indeks = VeriErisim.DegerAl(dr, 3, 0);
        //    //    gelen.odenek[indeks] = VeriErisim.DegerAl(dr, 4, (double)0);
        //    //}

        //    //if (donusDeger.liste.Count == 0)
        //    //    donusDeger.sonuc.bilgiStr = YatirimResI.YAT017;

        //    //donusDeger.sonuc.islemSonuc = true;

        //    //dr.Close();
        //    //ve.VeriErisimKapat();
        //    return donusDeger;
        //}

        ///// <summary>
        ///// Bütçe kaynağın ödenek teklif bilgilerini silen yordam
        ///// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        ///// ButceKaynakOdenek nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="odenek">ButceKaynakOdenek nesnelerinden oluşan listeyi tutan nesne</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TNSCollection odenek)
        //{
        //    Sonuc donusDeger = Yetki.YazmaHakkiVarMi(kullanan);
        //    //if (!donusDeger.islemSonuc)
        //    //    return donusDeger;

        //    //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
        //    //TABYATButceKaynakOdenek tabOdenek = new TABYATButceKaynakOdenek(ve);
        //    //ve.BeginTransaction();

        //    //try
        //    //{
        //    //    foreach (ButceKaynakOdenek o in odenek)
        //    //    {
        //    //        if (o.kaynakNo <= 0)
        //    //        {
        //    //            donusDeger.islemSonuc = false;
        //    //            donusDeger.hataStr = YatirimResI.YAT023;
        //    //            break;
        //    //        }
        //    //        if (!Genel.ButceKaynakVarMi(ve, o.kaynakNo))
        //    //        {
        //    //            donusDeger.islemSonuc = false;
        //    //            donusDeger.hataStr = YatirimResI.YAT024;
        //    //            break;
        //    //        }

        //    //        tabOdenek.Sil(o);
        //    //        if (!string.IsNullOrEmpty(ve.hata))
        //    //        {
        //    //            donusDeger.islemSonuc = false;
        //    //            donusDeger.hataStr = ve.hata;
        //    //            break;
        //    //        }
        //    //    }
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    donusDeger.islemSonuc = false;
        //    //    donusDeger.hataStr = e.Message;
        //    //}

        //    //if (donusDeger.islemSonuc)
        //    //{
        //    //    ve.Commit();
        //    //    donusDeger.bilgiStr = YatirimResI.YAT020;
        //    //}
        //    //else
        //    //    ve.Rollback();

        //    //ve.VeriErisimKapat();
        //    return donusDeger;
        //}

        ///// <summary>
        ///// Bütçe kaynağın ödenek teklif bilgilerini silen yordam
        ///// Kullanıcı işleme yetkili değilse veya yıl bilgisi sıfırdan küçükse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="senaryoKod">Senaryo kriteri</param>
        ///// <param name="tur">Silinmek istenen bütçe kaynak türleri</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //private static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, ENUMButceKaynakTur[] tur)
        //{
        //    //Sonuc donusDeger = Yetki.YazmaHakkiVarMi(kullanan);
        //    //if (!(donusDeger.islemSonuc || kullanan.ad == "aktarim"))
        //    //    return donusDeger;
        //    //else
        //    //    donusDeger = new Sonuc();

        //    //if (yil <= 0)
        //    //    donusDeger.hataStr = YatirimResI.YAT032;
        //    //if (!string.IsNullOrEmpty(donusDeger.hataStr))
        //    //{
        //    //    donusDeger.islemSonuc = false;
        //    //    return donusDeger;
        //    //}

        //    //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));

        //    //try
        //    //{
        //    //    TABYATButceKaynakOdenek tabOdenek = new TABYATButceKaynakOdenek(ve);
        //    //    ve.BeginTransaction();
        //    //    donusDeger = tabOdenek.Sil(yil, senaryoKod, tur);
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    donusDeger.islemSonuc = false;
        //    //    donusDeger.hataStr = e.Message;
        //    //}

        //    //if (donusDeger.islemSonuc)
        //    //{
        //    //    ve.Commit();
        //    //    donusDeger.bilgiStr = YatirimResI.YAT020;
        //    //}
        //    //else
        //    //    ve.Rollback();

        //    //ve.VeriErisimKapat();
        //    //return donusDeger;
        //    return null;
        //}
    }
}