using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Değerleme bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMAmortismanDegerleme
    {
        /// <summary>
        /// Değerlemeyle ilgili genel hata bilgisini tutan değişken
        /// </summary>
        public static string degerlemeHata = TasinirMalResI.TMM068;

        /// <summary>
        /// Verilen kriterlere uygun olan herhangi bir demirbaşa değerleme uygulanmış mı kontrolünü yapan yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebeKod">Muhasebe birimi</param>
        /// <param name="harcamaKod">Harcama birimi</param>
        /// <param name="belgeNo">Taşınır işlem fişi belge numarası</param>
        /// <returns>Kontrol sonucu döndürülür.</returns>
        public static bool DegerlemeYapilmisMi(VeriErisim veriErisim, int yil, string muhasebeKod, string harcamaKod, string belgeNo)
        {
            return DegerlemeYapilmisMi(veriErisim, new AmortismanKriter(yil, muhasebeKod, harcamaKod, belgeNo));
        }
        /// <summary>
        /// Verilen kriterlere uygun olan herhangi bir demirbaşa değerleme uygulanmış mı kontrolünü yapan yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="ak">Değerleme listeleme kriterleri</param>
        /// <returns>Kontrol sonucu döndürülür.</returns>
        private static bool DegerlemeYapilmisMi(VeriErisim veriErisim, AmortismanKriter ak)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABAmortismanDegerleme tAd = new TABAmortismanDegerleme(ve);
            bool sonuc = tAd.DegerlemeYapilmisMi(ak);
            ve.VeriErisimKapat();
            return sonuc;
        }

        ///// <summary>
        ///// Taşınır demirbaşlarının değerlemelerini verilen kriterlere göre hesaplayıp kaydeden yordam
        ///// Verilen kriterlerle ilgili bir takım kontroller yapılmakta, sorunsuz bir bilgi kümesi
        ///// olduğuna karar verilirse değerleme hesaplama işlemleri yapılıp, bulunan değerler kayıt edilir.
        ///// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="ak">Değerleme yapılacak demirbaşlara ait bilgiler</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        //{
        //    Sonuc sonuc = new Sonuc();
        //    sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod);
        //    if (!sonuc.islemSonuc)
        //    {
        //        sonuc.hataStr = TasinirMalResI.TMM050;
        //        return sonuc;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

        //    int amortismanBaslamaYil = TABGenel.AmortismanBaslamaYilDondur(ve, ak);

        //    sonuc = TMMAmortisman.AmortismanKriterKontrol(kullanan, ak, TMMAmortisman.ENUMIslemTur.KAYDET, amortismanBaslamaYil);
        //    if (!sonuc.islemSonuc)
        //    {
        //        ve.VeriErisimKapat();
        //        return sonuc;
        //    }

        //    if (TMMAmortisman.AmortismanYapilmisMi(ve, ak))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = TMMAmortisman.amortismanHata;
        //        ve.VeriErisimKapat();
        //        return sonuc;
        //    }

        //    //Değerleme uygulanacak malzemeler bulunuyor
        //    ObjectArray degerlemeYapilacaklar = DegerlemeYapilacaklariListele(ve, kullanan, ak);
        //    if (!degerlemeYapilacaklar.sonuc.islemSonuc)
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = degerlemeYapilacaklar.sonuc.hataStr;
        //        ve.VeriErisimKapat();
        //        return sonuc;
        //    }

        //    try
        //    {
        //        TABAmortismanDegerleme tDegerleme = new TABAmortismanDegerleme(ve);
        //        ve.BeginTransaction();

        //        //Kriterlere uyan kayıtlı değerlemeler siliniyor, döngüde hesaplanıp tekrar kayıt edilecek
        //        sonuc = tDegerleme.Sil(ak);
        //        if (sonuc.islemSonuc)
        //        {
        //            foreach (SicilNoHareket dy in degerlemeYapilacaklar.objeler)
        //            {
        //                decimal degerlemeHesaplanan = dy.fiyat * (decimal)ak.degerlemeOran / 100;

        //                AmortismanDegerleme yeniDegerleme = new AmortismanDegerleme();
        //                if (degerlemeHesaplanan > 0)
        //                {
        //                    yeniDegerleme.yil = ak.yil;
        //                    yeniDegerleme.prSicilNo = dy.prSicilNo;
        //                    yeniDegerleme.degerlemeMiktar = degerlemeHesaplanan;
        //                    yeniDegerleme.degerlemeOran = ak.degerlemeOran;
        //                    yeniDegerleme.muhasebeKod = dy.muhasebeKod;
        //                    yeniDegerleme.harcamaKod = dy.harcamaBirimKod;
        //                    yeniDegerleme.ambarKod = dy.ambarKod;
        //                    yeniDegerleme.gorSicilNo = dy.sicilNo;
        //                    yeniDegerleme.hesapPlanKod = dy.hesapPlanKod;
        //                    yeniDegerleme.islemTarih.Yaz(DateTime.Now);
        //                    yeniDegerleme.islemYapan = kullanan.kullaniciKodu;

        //                    sonuc = tDegerleme.Kaydet(yeniDegerleme);
        //                    if (!sonuc.islemSonuc)
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = e.Message;
        //    }

        //    if (sonuc.islemSonuc)
        //        ve.Commit();
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();
        //    return sonuc;
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan değerleme işlemi yapılacak taşınır demirbaşlarını döndüren yordam
        ///// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="ak">Değerleme listeleme kriterleri</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır demirbaşları listesi döndürülür.</returns>
        //private static ObjectArray DegerlemeYapilacaklariListele(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        //{
        //    //Değerleme yapılacaklar ile amortisman yapılacaklar aynı kriterlere göre tek rutinden geliyor
        //    return TMMAmortisman.AmortismanYapilacaklariListele(veriErisim, kullanan, ak);
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan taşınır demirbaşlarının değerlemelerini silen yordam
        ///// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="ak">Değerlemesi silinecek demirbaşlara ait bilgiler</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, AmortismanKriter ak)
        //{
        //    Sonuc sonuc = new Sonuc();
        //    sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod);
        //    if (!sonuc.islemSonuc)
        //    {
        //        sonuc.hataStr = TasinirMalResI.TMM061;
        //        return sonuc;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

        //    int amortismanBaslamaYil = TABGenel.AmortismanBaslamaYilDondur(ve, ak);

        //    sonuc = TMMAmortisman.AmortismanKriterKontrol(kullanan, ak, TMMAmortisman.ENUMIslemTur.SIL, amortismanBaslamaYil);
        //    if (!sonuc.islemSonuc)
        //    {
        //        ve.VeriErisimKapat();
        //        return sonuc;
        //    }

        //    if (TMMAmortisman.AmortismanYapilmisMi(ve, ak))
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = TMMAmortisman.amortismanHata;
        //        ve.VeriErisimKapat();
        //        return sonuc;
        //    }

        //    TABAmortismanDegerleme tDegerleme = new TABAmortismanDegerleme(ve);
        //    ve.BeginTransaction();

        //    try
        //    {
        //        sonuc = tDegerleme.Sil(ak);
        //    }
        //    catch (Exception e)
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = e.Message;
        //    }

        //    if (sonuc.islemSonuc)
        //        ve.Commit();
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();
        //    return sonuc;
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan değerleme toplam tutarlarını demirbaş bazında döndüren yordam
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="ak">Değerleme listeleme kriterleri</param>
        ///// <param name="yilIci">Verilen yıl içindeki değerleme bilgileri isteniyorsa true, önceki yıllara ait bilgiler isteniyorsa false olmalı.</param>
        ///// <returns>Değerleme toplam tutarları döndürülür.</returns>
        //public static TNSCollection SicilDegerlemeMiktarListele(VeriErisim veriErisim, AmortismanKriter ak, bool yilIci)
        //{
        //    TNSCollection liste = new TNSCollection();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABAmortismanDegerleme tAmortismanDegerleme = new TABAmortismanDegerleme(ve);

        //    IDataReader dr = tAmortismanDegerleme.SicilDegerlemeMiktarListele(ak, yilIci);
        //    while (dr.Read())
        //    {
        //        AmortismanDegerleme gelen = new AmortismanDegerleme();
        //        gelen.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
        //        if (yilIci)
        //            gelen.yapilanToplamDegerlemeYilIci = VeriErisim.DegerAl(dr, 1, (decimal)0);
        //        else
        //            gelen.yapilanToplamDegerlemeEskiYillar = VeriErisim.DegerAl(dr, 1, (decimal)0);

        //        liste.Add(gelen);
        //    }
        //    dr.Close();

        //    ve.VeriErisimKapat();
        //    return liste;
        //}



    }
}