using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BKD;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// BKDFaaliyetinYapildigiYer formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BKDFaaliyetinYapildigiYer
    {
    //    /// <summary>
    //    /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
    //    /// </summary>
    //    public static string VERIERISIMAD = "BaglantiSatiriBKD";

    //    /// <summary>
    //    /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
    //    /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
    //    /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
    //    /// </summary>
    //    /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
    //    public static bool HasWriteAccess(Kullanici kullanan, FaaliyetinYapildigiYer form)
    //    {
    //        return true;
    //    }

    //    /// <summary>
    //    /// Verilen formdaki alanların tutarlılığını kontrol eden fonksiyon
    //    /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
    //    /// <param name="form">Alan tutarlılığı kontrol edilecek nesne</param>
    //    /// </summary>
    //    /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
    //    public static Sonuc IsFieldsOK(Kullanici kullanan, FaaliyetinYapildigiYer form)
    //    {
    //        return new Sonuc(true, "", "");
    //    }


    //    /// <summary>
    //    /// FaaliyetinYapildigiYer nesnesini veri tabanına yazan fonksiyon
    //    /// Hata olursa Exception oluşturur
    //    /// </summary>
    //    /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
    //    /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
    //    /// <param name="form">FaaliyetinYapildigiYer yapısında FaaliyetinYapildigiYer: Faaliyetin Yapıldığı Yer bilgilerini tutan değişken</param>
    //    /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
    //    public static Sonuc Save(VeriErisim veriErisim, Kullanici kullanan, FaaliyetinYapildigiYer form)
    //    {
    //        Sonuc s = new Sonuc(true, "", "");
    //        if (!HasWriteAccess(kullanan, form))
    //        {
    //            s.DegerAta(ButceKodResI.FaaliyetinYapildigiYer001);
    //            return s;
    //        }

    //        s = IsFieldsOK(kullanan, form);
    //        if (!s.islemSonuc)
    //        {
    //            return s;
    //        }

    //        VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
    //        TABBKDFAALIYETINYAPILDIGIYER tabBKDFAALIYETINYAPILDIGIYER = new TABBKDFAALIYETINYAPILDIGIYER(ve);
    //        TABBKDFAALIYETINYAPILDIGIYERDETAY tabBKDFAALIYETINYAPILDIGIYERDETAY = new TABBKDFAALIYETINYAPILDIGIYERDETAY(ve);
    //        TABBKDFAALIYETINYAPILDIGIYERBIRIM tabBKDFAALIYETINYAPILDIGIYERBIRIM = new TABBKDFAALIYETINYAPILDIGIYERBIRIM(ve);

    //        ve.BeginTransaction();

    //        if (s.islemSonuc)
    //            s = tabBKDFAALIYETINYAPILDIGIYERDETAY.Delete(form);
    //        if (s.islemSonuc)
    //            s = tabBKDFAALIYETINYAPILDIGIYERBIRIM.Delete(form);
    //        if (s.islemSonuc)
    //            s = tabBKDFAALIYETINYAPILDIGIYER.Delete(form);

    //        if (s.islemSonuc)
    //            s = tabBKDFAALIYETINYAPILDIGIYER.Insert(form);

    //        for (int i = 0; i < form.faaliyetinYapildigiYerDetay.Length && s.islemSonuc; i++)
    //        {
    //            s = tabBKDFAALIYETINYAPILDIGIYERDETAY.Insert(form, i);
    //        }

    //        for (int i = 0; i < form.faaliyetinYapildigiYerBirim.Length && s.islemSonuc; i++)
    //        {
    //            s = tabBKDFAALIYETINYAPILDIGIYERBIRIM.Insert(form, i);
    //        }

    //        if (s.islemSonuc)
    //            ve.Commit();
    //        else
    //            ve.Rollback();

    //        ve.VeriErisimKapat();
    //        return s;
    //    }


    //    /// <summary>
    //    /// FaaliyetinYapildigiYer nesnesini veri tabanından okuyan fonksiyon
    //    /// Hata olursa Exception oluşturur
    //    /// </summary>
    //    /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
    //    /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
    //    /// <param name="form">FaaliyetinYapildigiYer yapısında FaaliyetinYapildigiYer: Faaliyetin Yapıldığı Yer bilgilerini tutan değişken</param>
    //    public static FaaliyetinYapildigiYer Get(VeriErisim veriErisim, Kullanici kullanan, FaaliyetinYapildigiYer form)
    //    {
    //        FaaliyetinYapildigiYer[] s = List(veriErisim, kullanan, form);
    //        if (s == null || s.Length == 0) return null;
    //        return s[0];
    //    }

    //    /// <summary>
    //    /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
    //    /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
    //    /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
    //    /// </summary>
    //    /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
    //    public static bool HasReadAccess(Kullanici kullanan, FaaliyetinYapildigiYer form)
    //    {
    //        return true;
    //    }

    //    /// <summary>
    //    /// Anahtar alanların tutarlılığını kontrol eden fonksiyon
    //    /// <param name="kullanan">Anahtar alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
    //    /// <param name="form">Anahtar alan tutarlılığı kontrol edilecek nesne</param>
    //    /// </summary>
    //    /// <returns>Anahtar alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
    //    public static Sonuc AreKeyFieldsOK(Kullanici kullanan, FaaliyetinYapildigiYer form)
    //    {
    //        return new Sonuc(true, "", "");
    //    }


    //    /// <summary>
    //    /// FaaliyetinYapildigiYer listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
    //    /// Hata olursa Exception oluşturur
    //    /// </summary>
    //    /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
    //    /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
    //    /// <param name="form">FaaliyetinYapildigiYer yapısında FaaliyetinYapildigiYer: Faaliyetin Yapıldığı Yer bilgilerini tutan değişken</param>
    //    public static FaaliyetinYapildigiYer[] List(VeriErisim veriErisim, Kullanici kullanan, FaaliyetinYapildigiYer form)
    //    {
    //        if (!HasReadAccess(kullanan, form))
    //        {
    //            throw new Exception(ButceKodResI.FaaliyetinYapildigiYer002);
    //        }

    //        Sonuc s = AreKeyFieldsOK(kullanan, form);
    //        if (!s.islemSonuc)
    //        {
    //            throw new Exception(s.hataStr);
    //        }

    //        VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
    //        TABBKDFAALIYETINYAPILDIGIYER tabBKDFAALIYETINYAPILDIGIYER = new TABBKDFAALIYETINYAPILDIGIYER(ve);
    //        TABBKDFAALIYETINYAPILDIGIYERDETAY tabBKDFAALIYETINYAPILDIGIYERDETAY = new TABBKDFAALIYETINYAPILDIGIYERDETAY(ve);
    //        TABBKDFAALIYETINYAPILDIGIYERBIRIM tabBKDFAALIYETINYAPILDIGIYERBIRIM = new TABBKDFAALIYETINYAPILDIGIYERBIRIM(ve);


    //        ArrayList alAna;
    //        IDataReader dr;
    //        dr = tabBKDFAALIYETINYAPILDIGIYER.Get(form);
    //        if (dr == null)
    //        {
    //            ve.VeriErisimKapat();
    //            throw new Exception(ve.hata);
    //        }
    //        alAna = new ArrayList();
    //        while (dr.Read())
    //        {
    //            FaaliyetinYapildigiYer detay = new FaaliyetinYapildigiYer();
    //            detay.yil = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYER.ENUMAlanlar.YIL, 0);
    //            detay.faaliyetKod = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYER.ENUMAlanlar.FAALIYETKOD, string.Empty);
    //            detay.merkezDahil = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYER.ENUMAlanlar.MERKEZDAHIL, 0);
    //            detay.tumilveilceler = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYER.ENUMAlanlar.TUMILVEILCELER, 0);
    //            alAna.Add(detay);
    //        }
    //        dr.Close();
    //        ArrayList al;
    //        foreach (FaaliyetinYapildigiYer formIc in alAna)
    //        {
    //            dr = tabBKDFAALIYETINYAPILDIGIYERDETAY.Get(formIc);
    //            if (dr == null)
    //            {
    //                ve.VeriErisimKapat();
    //                throw new Exception(ve.hata);
    //            }
    //            al = new ArrayList();
    //            while (dr.Read())
    //            {
    //                FaaliyetinYapildigiYerDetay detay = new FaaliyetinYapildigiYerDetay();
    //                detay.ilKod = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYERDETAY.ENUMFields.ILKOD, string.Empty);
    //                //detay.ilAdi = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYERDETAY.ENUMFields.ILADI, "");
    //                al.Add(detay);
    //            }
    //            formIc.faaliyetinYapildigiYerDetay = (FaaliyetinYapildigiYerDetay[])al.ToArray(typeof(FaaliyetinYapildigiYerDetay));
    //            dr.Close();
    //        }

    //        foreach (FaaliyetinYapildigiYer formIc in alAna)
    //        {
    //            dr = tabBKDFAALIYETINYAPILDIGIYERBIRIM.Get(formIc);
    //            if (dr == null)
    //            {
    //                ve.VeriErisimKapat();
    //                throw new Exception(ve.hata);
    //            }
    //            al = new ArrayList();
    //            while (dr.Read())
    //            {
    //                FaaliyetinYapildigiYerBirim birim = new FaaliyetinYapildigiYerBirim();
    //                birim.birimKod = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYERBIRIM.ENUMFields.BIRIMKOD, 0);
    //                al.Add(birim);
    //            }
    //            formIc.faaliyetinYapildigiYerBirim = (FaaliyetinYapildigiYerBirim[])al.ToArray(typeof(FaaliyetinYapildigiYerBirim));
    //            dr.Close();
    //        }
    //        ve.VeriErisimKapat();
    //        return (FaaliyetinYapildigiYer[])alAna.ToArray(typeof(FaaliyetinYapildigiYer));
    //    }

    //    public static FaaliyetinYapildigiYer[] ListeleTumunu(VeriErisim veriErisim, Kullanici kullanan, FaaliyetinYapildigiYer form)
    //    {
    //        if (!HasReadAccess(kullanan, form))
    //        {
    //            throw new Exception(ButceKodResI.FaaliyetinYapildigiYer002);
    //        }

    //        Sonuc s = AreKeyFieldsOK(kullanan, form);
    //        if (!s.islemSonuc)
    //        {
    //            throw new Exception(s.hataStr);
    //        }
    //        VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
    //        TABBKDFAALIYETINYAPILDIGIYER tabBKDFAALIYETINYAPILDIGIYER = new TABBKDFAALIYETINYAPILDIGIYER(ve);
    //        TABBKDFAALIYETINYAPILDIGIYERDETAY tabBKDFAALIYETINYAPILDIGIYERDETAY = new TABBKDFAALIYETINYAPILDIGIYERDETAY(ve);
    //        TABBKDFAALIYETINYAPILDIGIYERBIRIM tabBKDFAALIYETINYAPILDIGIYERBIRIM = new TABBKDFAALIYETINYAPILDIGIYERBIRIM(ve);


    //        ArrayList alAna;
    //        IDataReader dr;
    //        dr = tabBKDFAALIYETINYAPILDIGIYER.TumunuGetir(form);
    //        if (dr == null)
    //        {
    //            ve.VeriErisimKapat();
    //            throw new Exception(ve.hata);
    //        }
    //        alAna = new ArrayList();
    //        while (dr.Read())
    //        {
    //            FaaliyetinYapildigiYer detay = new FaaliyetinYapildigiYer();
    //            detay.yil = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYER.ENUMAlanlar.YIL, 0);
    //            detay.faaliyetKod = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYER.ENUMAlanlar.FAALIYETKOD, string.Empty);
    //            detay.merkezDahil = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYER.ENUMAlanlar.MERKEZDAHIL, 0);
    //            detay.tumilveilceler = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYER.ENUMAlanlar.TUMILVEILCELER, 0);
    //            alAna.Add(detay);
    //        }
    //        dr.Close();
    //        ArrayList al;
    //        foreach (FaaliyetinYapildigiYer formIc in alAna)
    //        {
    //            dr = tabBKDFAALIYETINYAPILDIGIYERDETAY.Get(formIc);
    //            if (dr == null)
    //            {
    //                ve.VeriErisimKapat();
    //                throw new Exception(ve.hata);
    //            }
    //            al = new ArrayList();
    //            while (dr.Read())
    //            {
    //                FaaliyetinYapildigiYerDetay detay = new FaaliyetinYapildigiYerDetay();
    //                detay.ilKod = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYERDETAY.ENUMFields.ILKOD, string.Empty);
    //                //detay.ilAdi = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYERDETAY.ENUMFields.ILADI, "");
    //                al.Add(detay);
    //            }
    //            formIc.faaliyetinYapildigiYerDetay = (FaaliyetinYapildigiYerDetay[])al.ToArray(typeof(FaaliyetinYapildigiYerDetay));
    //            dr.Close();
    //        }

    //        foreach (FaaliyetinYapildigiYer formIc in alAna)
    //        {
    //            dr = tabBKDFAALIYETINYAPILDIGIYERBIRIM.Get(formIc);
    //            if (dr == null)
    //            {
    //                ve.VeriErisimKapat();
    //                throw new Exception(ve.hata);
    //            }
    //            al = new ArrayList();
    //            while (dr.Read())
    //            {
    //                FaaliyetinYapildigiYerBirim birim = new FaaliyetinYapildigiYerBirim();
    //                birim.birimKod = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYERBIRIM.ENUMFields.BIRIMKOD, 0);
    //                al.Add(birim);
    //            }
    //            formIc.faaliyetinYapildigiYerBirim = (FaaliyetinYapildigiYerBirim[])al.ToArray(typeof(FaaliyetinYapildigiYerBirim));
    //            dr.Close();
    //        }
    //        ve.VeriErisimKapat();
    //        return (FaaliyetinYapildigiYer[])alAna.ToArray(typeof(FaaliyetinYapildigiYer));

    //    }

    //    public static bool IllereGoreFaaliyetGetir(VeriErisim veriErisim, Kullanici kullanan, FaaliyetinYapildigiYer form, int ilkod)
    //    {
    //        bool faaliyetVar;

    //        VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
    //        TABBKDFAALIYETINYAPILDIGIYER tabBKDFAALIYETINYAPILDIGIYER = new TABBKDFAALIYETINYAPILDIGIYER(ve);

    //        ArrayList alAna;
    //        IDataReader dr;
    //        dr = tabBKDFAALIYETINYAPILDIGIYER.FaaliyetBul(form, ilkod);
    //        if (dr == null)
    //        {
    //            ve.VeriErisimKapat();
    //            throw new Exception(ve.hata);
    //        }
    //        alAna = new ArrayList();
    //        //while (dr.Read())
    //        //{
    //        //    FaaliyetinYapildigiYer detay = new FaaliyetinYapildigiYer();
    //        //    detay.yil = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYER.ENUMAlanlar.YIL, 0);
    //        //    detay.faaliyetKod = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYER.ENUMAlanlar.FAALIYETKOD, 0);
    //        //    alAna.Add(detay);
    //        //}

    //        if (dr.Read() == true)
    //            faaliyetVar = true;
    //        else faaliyetVar = false;

    //        dr.Close();
    //        return (faaliyetVar);

    //        //return (FaaliyetinYapildigiYer[])alAna.ToArray(typeof(FaaliyetinYapildigiYer));
    //    }
    //    public static bool BirimeGoreFaaliyetGetir(VeriErisim veriErisim, Kullanici kullanan, FaaliyetinYapildigiYer form, int birimKod)
    //    {
    //        bool birimVar;
    //        VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
    //        TABBKDFAALIYETINYAPILDIGIYER tabBKDFAALIYETINYAPILDIGIYER = new TABBKDFAALIYETINYAPILDIGIYER(ve);

    //        ArrayList alAna;
    //        IDataReader dr;
    //        dr = tabBKDFAALIYETINYAPILDIGIYER.BirimBul(form, birimKod);
    //        if (dr == null)
    //        {
    //            ve.VeriErisimKapat();
    //            throw new Exception(ve.hata);
    //        }
    //        alAna = new ArrayList();
    //        //while (dr.Read())
    //        //{
    //        //    FaaliyetinYapildigiYer detay = new FaaliyetinYapildigiYer();
    //        //    detay.yil = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYER.ENUMAlanlar.YIL, 0);
    //        //    detay.faaliyetKod = VeriErisim.DegerAl(dr, (int)TABBKDFAALIYETINYAPILDIGIYER.ENUMAlanlar.FAALIYETKOD, 0);
    //        //    alAna.Add(detay);
    //        //}

    //        if (dr.Read() == true)
    //            birimVar = true;
    //        else birimVar = false;
    //        dr.Close();
    //        return (birimVar);
    //    }
    }
}
