using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.KYM;
using TNS.Veri;
using TNS.YAT.IsKurallari.Veri;

namespace TNS.YAT.IsKurallari
{
    /// <summary>
    /// Sektör bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class Sektor
    {
        ///// <summary>
        ///// Sektör varsa güncelleyen, yoksa kaydeden yordam
        ///// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="sektor">Sektör sözlük bilgisini tutan nesne</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //public static Sonuc Kaydet(Kullanici kullanan, SektorSozluk sektor)
        //{
        //    return IslemYap(null, (int)ENUMSozlukIslemTur.KAYDET, kullanan, sektor);
        //}

        ///// <summary>
        ///// Sektör, yatırım projelerinde kullanılmamışsa silen,
        ///// kullanılmışsa uyarı döndüren yordam
        ///// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="sektor">Sektör sözlük bilgisini tutan nesne</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //public static Sonuc Sil(Kullanici kullanan, SektorSozluk sektor)
        //{
        //    return IslemYap(null, (int)ENUMSozlukIslemTur.SIL, kullanan, sektor);
        //}

        ///// <summary>
        ///// Sektör sözlüğün silme işlemi sırasında çağırılan,
        ///// gönderilen sözlük kodunun doğruluğunu kontrol eden yordam
        ///// </summary>
        ///// <param name="kod">Sözlük kodu</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //private static Sonuc SektorSozlukSilKontrol(string kod)
        //{
        //    Sonuc donusDeger = new Sonuc(true, "", "");
        //    SektorSozluk sekSoz = new SektorSozluk();
        //    if (kod == "" || kod == sekSoz.kod)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = YatirimResI.YAT038;
        //    }

        //    return donusDeger;
        //}

        ///// <summary>
        ///// Sektör sözlüğün kaydet işlemi sırasında çağırılan,
        ///// gönderilen sektör sözlük nesnesinin doğruluğunu kontrol eden yordam
        ///// </summary>
        ///// <param name="sektor">Sektör sözlük bilgilerini tutan nesne</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //private static Sonuc SektorSozlukKaydetKontrol(SektorSozluk sektor)
        //{
        //    Sonuc donusDeger = SektorSozlukSilKontrol(sektor.kod);

        //    if (!donusDeger.islemSonuc)
        //        return donusDeger;

        //    if (sektor.ad.Trim() == "")
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = YatirimResI.YAT010;
        //    }

        //    return donusDeger;
        //}

        ///// <summary>
        ///// Verilen sektör kodunun üst sektörünü döndüren yordam
        ///// </summary>
        ///// <param name="sektorKodu">Sektör sözlüğün kodu</param>
        ///// <returns>Üst sektör kodu döndürülür.</returns>
        //private static string UstSektorKoduBul(string sektorKodu)
        //{
        //    string donusDeger = "";

        //    if (sektorKodu.Length <= 2)
        //        return donusDeger;
            
        //    string[] parcaliKod = sektorKodu.Split('.');
        //    for (int i = 0; i < parcaliKod.Length; i++)
        //    {
        //        if (i != parcaliKod.Length - 1)
        //            donusDeger += parcaliKod[i] + ".";
        //    }
            
        //    donusDeger = donusDeger.Substring(0, donusDeger.Length - 1);
            
        //    return donusDeger;
        //}

        ///// <summary>
        ///// Kodu gönderilen sektör sözlük bilgisinin sistemde tanımlı olup olmadığını kontrol eden yordam
        ///// </summary>
        ///// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="sektorKodu">Sektör sözlüğünün kodu</param>
        ///// <returns>Sektör varsa Sonuc yapısında true, yoksa false ve hata mesajı döndürülür.</returns>
        //private static Sonuc SektorVarMi(VeriErisim ve, string sektorKodu)
        //{
        //    Sonuc donusDeger = new Sonuc(true, "", "");
        //    TABYATSektor tabSektor = new TABYATSektor(ve);

        //    if (!tabSektor.SektorVarMi(sektorKodu))
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = YatirimResI.YAT060;
        //    }

        //    return donusDeger;
        //}

        ///// <summary>
        ///// Gönderilen sektör sözlük bilgisinin alt sektörünün olup olmadığını kontrol eden yordam
        ///// </summary>
        ///// <param name="ve">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="sektor">Sektör sözlük bilgisini tutan nesne</param>
        ///// <returns>Sektörün, alt sektörü varsa Sonuc yapısında true, yoksa false ve hata mesajı döndürülür.</returns>
        //private static Sonuc AltSektorVarMi(VeriErisim ve, SektorSozluk sektor)
        //{
        //    Sonuc donusDeger = new Sonuc(true, "", "");
        //    TABYATSektor tabSektor = new TABYATSektor(ve);

        //    if (tabSektor.AltSektorVarMi(sektor))
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = YatirimResI.YAT061;
        //    }

        //    return donusDeger;
        //}

        ///// <summary>
        ///// Gelen işlem türüne göre sektör bilgisini kaydeden veya silen yordam
        ///// islemTur parametresinin değeri 0 ise kayıt, 1 ise silme işlemi yapar.
        ///// Kullanıcı işleme yetkili değilse veya gönderilen sözlük nesnesi eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="islemTur">İşlem kayıt mı, silme mi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="sektor">Sektör sözlük bilgisini tutan nesne</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //private static Sonuc IslemYap(VeriErisim veriErisim, int islemTur, Kullanici kullanan, SektorSozluk sektor)
        //{
        //    Sonuc donusDeger = Yetki.SozlukYazmaHakkiVarMi(kullanan);
        //    if (!donusDeger.islemSonuc)
        //        return donusDeger;

        //    if (islemTur == (int)ENUMSozlukIslemTur.KAYDET)
        //        donusDeger = SektorSozlukKaydetKontrol(sektor);
        //    else
        //        donusDeger = SektorSozlukSilKontrol(sektor.kod);
        //    if (!donusDeger.islemSonuc)
        //        return donusDeger;

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABYATSektor tabSektor = new TABYATSektor(ve);
        //    ve.BeginTransaction();

        //    if (islemTur == (int)ENUMSozlukIslemTur.KAYDET)
        //    {
        //        string ustKod = UstSektorKoduBul(sektor.kod);

        //        if (ustKod != "")
        //        {
        //            donusDeger = SektorVarMi(ve, ustKod);
        //            if (!donusDeger.islemSonuc)
        //            {
        //                ve.VeriErisimKapat();
        //                return donusDeger;
        //            }
                    
        //            donusDeger = tabSektor.GuncelleAltSektorVarMi(new SektorSozluk(ustKod, "", "E"));
        //            if (!donusDeger.islemSonuc)
        //            {
        //                ve.VeriErisimKapat();
        //                return donusDeger;
        //            }
        //        }
        //    }

        //    if (islemTur == (int)ENUMSozlukIslemTur.SIL)
        //    {
        //        donusDeger = AltSektorVarMi(ve, sektor);
        //        if (!donusDeger.islemSonuc)
        //        {
        //            ve.VeriErisimKapat();
        //            return donusDeger;
        //        }
        //    }

        //    try
        //    {
        //        if (islemTur == (int)ENUMSozlukIslemTur.KAYDET)
        //        {
        //            donusDeger = tabSektor.GuncelleAd(sektor);
        //            if (!donusDeger.islemSonuc)
        //            {
        //                sektor.altSektorVarMi = "H";
        //                donusDeger = tabSektor.Kaydet(sektor);
        //            }
        //        }
        //        else
        //        {
        //            if (Genel.SektorKullanilmisMi(ve, sektor.kod))
        //            {
        //                donusDeger.islemSonuc = false;
        //                donusDeger.hataStr = YatirimResI.YAT062;
        //            }
        //            else
        //                donusDeger = tabSektor.Sil(sektor);

        //            if (donusDeger.islemSonuc)
        //            {
        //                string ustKod = UstSektorKoduBul(sektor.kod);

        //                if (ustKod != "")
        //                {
        //                    Sonuc sonuc = AltSektorVarMi(ve, new SektorSozluk(ustKod, "", ""));
        //                    if (sonuc.islemSonuc)
        //                    {
        //                        donusDeger = tabSektor.GuncelleAltSektorVarMi(new SektorSozluk(ustKod, "", "H"));
        //                        if (!donusDeger.islemSonuc)
        //                        {
        //                            ve.VeriErisimKapat();
        //                            return donusDeger;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.Message;
        //    }

        //    if (donusDeger.islemSonuc)
        //        ve.Commit();
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();
        //    return donusDeger;
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan sektör bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="sektor">Sektör kriteri</param>
        ///// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sektör listesi döndürülür.</returns>
        //public static BilgiVAN Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, SektorSozluk sektor)
        //{
        //    BilgiVAN donusDeger = new BilgiVAN();

        //    donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
        //    if (!donusDeger.sonuc.islemSonuc)
        //        return donusDeger;

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABYATSektor tabsektor = new TABYATSektor(ve);
        //    IDataReader dr = tabsektor.Listele(yil, sektor);

        //    if(dr == null)
        //    {
        //        donusDeger.sonuc.DegerAta(ve.hata);
        //        ve.VeriErisimKapat();
        //        return donusDeger;
        //    }

        //    while (dr.Read())
        //    {
        //        SektorSozluk gelen = new SektorSozluk();
        //        string kodYapi = gelen.kod;
        //        gelen.kod = VeriErisim.DegerAl(dr, 0, "");
        //        gelen.kod = OrtakFonksiyonlar.KodNoktaKoy(kodYapi, gelen.kod);
        //        gelen.ad = VeriErisim.DegerAl(dr, 1, "");
        //        gelen.altSektorVarMi = VeriErisim.DegerAl(dr, 2, "");
        //        donusDeger.liste.Add(gelen);
        //    }
        //    dr.Close();

        //    if (donusDeger.liste.Count == 0)
        //        donusDeger.sonuc.bilgiStr = YatirimResI.YAT017;

        //    donusDeger.sonuc.islemSonuc = true;

        //    ve.VeriErisimKapat();
        //    return donusDeger;
        //}
    }
}