using System;
using TNS;
using TNS.KYM;
using TNS.BHF;
using TNS.Veri;
using System.Data;
using TNS.BHF.IsKurallari.Veri;
using System.Collections;

namespace TNS.BHF.IsKurallari
{
    /// <summary>
    /// Bütçe hazırlığı sırasında, ödenek tahmini yapabilmek için kullanılan, bir birimin sahip olduğu fiziksel değerlerle ilgili işlemleri tutan sınıf.
    /// </summary>
    public class FizikselDegerlerServisi
    {
        /// <summary>
        /// Bir birimin sahip olduğu fiziksel değerlerin tutulduğu satırları listeleyen yordam.
        /// Yetki kontrolü yapıldıktan sonra, veri tabanından çekilen sözlük bilgileri listeye doldurulur. Her bir sözlük nesnesi için yıl bilgisi okunur ve böylece bir fiziksel değerin tüm yılları okunmuş olur. Okunan yıl bilgileri diziye çevrilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait fiziksel değerlerin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait fiziksel değerlerin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>fiziksel değerlerin tutulduğu satırlar döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod)
        {
            IDataReader r;
            FizikselDeger fd;
            ObjectArray donusDeger = new ObjectArray();
            donusDeger.sonuc.islemSonuc = false;

            if (Servis.GormeYetkisiVarMi(kullanan, kurumsalKod))
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                TABFizikselDegerler tfd = new TABFizikselDegerler(ve);
                r = tfd.KodlariAl();
                if (tfd.Hata() == "")
                {
                    while (r.Read())
                    {
                        fd = new FizikselDeger();
                        fd.kod = VeriErisim.DegerAl(r, 0, "");
                        fd.aciklama = VeriErisim.DegerAl(r, 1, "");
                        fd.degerGirilsinMi = VeriErisim.DegerAl(r, 2, 0) == 1;
                        donusDeger.Ekle(fd);
                    }
                    r.Close();
                    donusDeger.sonuc.islemSonuc = true;
                    for (int i = 0; i < donusDeger.ObjeSayisi; i++)
                    {
                        fd = (FizikselDeger)donusDeger[i];
                        r = tfd.YillariAl(yil, kurumsalKod, fd.kod);
                        fd.kod = TNS.OrtakFonksiyonlar.KodNoktaKoy("9.9.9.9", fd.kod);
                        if (tfd.Hata() == "")
                        {
                            ArrayList al = new ArrayList();
                            while (r.Read())
                            {
                                FizikselDegerYilBilgi y = new FizikselDegerYilBilgi();
                                y.yil = VeriErisim.DegerAl(r, 0, 0); ;
                                y.deger = VeriErisim.DegerAl(r, 1, 0);
                                if (fd.kod.StartsWith("1") && fd.kod.Length < 3)
                                    y.deger = 0;
                                al.Add(y);
                            }
                            fd.degerler = (FizikselDegerYilBilgi[])al.ToArray(typeof(FizikselDegerYilBilgi));
                            r.Close();
                        }
                        else
                        {
                            donusDeger.sonuc.hataStr = tfd.Hata();
                            donusDeger.sonuc.islemSonuc = false;
                            break;
                        }
                    }
                }
                else
                    donusDeger.sonuc.hataStr = tfd.Hata();
                ve.VeriErisimKapat();

            }
            else
                donusDeger.sonuc.hataStr = string.Format(ButceFormResI.BHFFDS001, kurumsalKod);

            return donusDeger;
        }

        /// <summary>
        /// Bir birimin sahip olduğu fiziksel değerleri kaydeden yordam.
        /// Yetki kontrolü yapıldıktan sonra, bu birimin eski kayıtları silinir, fizikselDegerler dizisindeki, yıl dizisinin her bir satırı kontrol edilir ve hata yoksa veri tabanına kaydedilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait fiziksel değerlerin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait fiziksel değerlerin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="fizikselDegerler">Kaydedilecek fiziksel değer satırları.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurumsalKod, TNSCollection fizikselDegerler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            TNS.BHF.IsKurallari.Servis.KurumsalKodBirimKontrol(kurumsalKod, sonuc);
            if (!Servis.YazmaYetkisiVarMi(kullanan, yil, kurumsalKod))
                sonuc.DegerAta(string.Format(ButceFormResI.BHFHBS002, kurumsalKod));

            if (sonuc.islemSonuc)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil), TNS.KYM.Arac.KimlikBul(kullanan));
                TABFizikselDegerler tfd = new TABFizikselDegerler(ve);
                ve.BeginTransaction();
                sonuc = tfd.Sil(yil, kurumsalKod);
                if (sonuc.islemSonuc)
                {
                    foreach (FizikselDeger fd in fizikselDegerler)
                    {
                        if (fd.degerGirilsinMi)
                        {
                            string kod = fd.kod.Replace(".", "");
                            foreach (FizikselDegerYilBilgi yb in fd.degerler)
                            {
                                sonuc = tfd.Ekle(yil, kurumsalKod, kod, yb.yil, yb.deger);
                                if (!sonuc.islemSonuc)
                                    break;
                            }
                        }
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
                ve.VeriErisimKapat();
            }
            return sonuc;
        }

        /// <summary>
        /// eBütçeden çekilen fiziksel değer sözlük bilgilerini kaydeden yordam.
        /// Kullanıcı sistem yöneticisi ise bu yordam çalışır. Önce tüm kayıtlar kontrol edilir. Yetki varsa ve kayıtlar kontrolden geçmiş ise, eski kayıtlar silinir, fizikselDegerlerSozlukler dizisindeki her bir satır veri tabanına kaydedilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="fizikselDegerlerSozlukler">eBütçeden çekilen fiziksel değer sözlük bilgilerini tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc SozlukKaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection fizikselDegerlerSozlukler)
        {
            Sonuc sonuc = new Sonuc();

            if (!Servis.Yoneticimi(kullanan))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ButceFormResI.BHFFDS007;
                return sonuc;
            }

            foreach (FizikselDeger fd in fizikselDegerlerSozlukler)
            {
                if (string.IsNullOrEmpty(fd.kod))
                    sonuc.hataStr = ButceFormResI.BHFFDS003 + "<br>";
                if (string.IsNullOrEmpty(fd.aciklama))
                    sonuc.hataStr += ButceFormResI.BHFFDS004 + "<br>";

                if (!string.IsNullOrEmpty(sonuc.hataStr))
                {
                    sonuc.islemSonuc = false;
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(), TNS.KYM.Arac.KimlikBul(kullanan));
            TABFizikselDegerler tfd = new TABFizikselDegerler(ve);
            ve.BeginTransaction();

            tfd.KodSil();
            foreach (FizikselDeger fd in fizikselDegerlerSozlukler)
            {
                sonuc = tfd.KodKaydet(fd);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = ButceFormResI.BHFFDS005;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = ButceFormResI.BHFFDS006 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Fiziksel değerler ile ilgili sözlük bilgilerini listeleyen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <returns>Bulunan sözlük bilgileri dizi olarak döndürülür.</returns>
        public static FizikselDeger[] SozlukListele(VeriErisim veriErisim)
        {
            ArrayList donusDeger = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(), TNS.KYM.Arac.KimlikBul(null));
            TABFizikselDegerler tfd = new TABFizikselDegerler(ve);

            IDataReader r = tfd.KodlariAl();
            if (!string.IsNullOrEmpty(tfd.Hata()))
            {
                ve.VeriErisimKapat();
                throw new Exception(tfd.Hata());
            }

            while (r.Read())
            {
                FizikselDeger fd = new FizikselDeger();
                fd.kod = VeriErisim.DegerAl(r, 0, string.Empty);
                fd.aciklama = VeriErisim.DegerAl(r, 1, string.Empty);
                fd.degerGirilsinMi = VeriErisim.DegerAl(r, 2, 0) == 1;

                donusDeger.Add(fd);
            }
            r.Close();

            if (donusDeger.Count <= 0)
            {
                ve.VeriErisimKapat();
                throw new Exception(ButceFormResI.BHFFDS008);
            }
            ve.VeriErisimKapat();
            return (FizikselDeger[])donusDeger.ToArray(typeof(FizikselDeger));
        }
    }
}
/*

    //public class FizikselDegerlerServisiTasra
    //{
    //    public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, int muhasebeKod, string birimKod, string kurumBirim)
    //    {
    //        IDataReader r;
    //        FizikselDeger fd;
    //        ObjectArray donusDeger = new ObjectArray();
    //        donusDeger.sonuc.islemSonuc = false;

    //        if (Servis.GormeYetkisiVarMi(kullanan, kurumBirim))
    //        {
    //            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil));
    //            TABFizikselDegerlerTasra tfd = new TABFizikselDegerlerTasra(ve);
    //            r = tfd.KodlariAl();
    //            if (tfd.Hata() == "")
    //            {
    //                while (r.Read())
    //                {
    //                    fd = new FizikselDeger();
    //                    fd.kod = VeriErisim.DegerAl(r, 0, "");
    //                    fd.aciklama = VeriErisim.DegerAl(r, 1, "");
    //                    fd.degerGirilsinMi = VeriErisim.DegerAl(r, 2, 0) == 1;
    //                    donusDeger.Ekle(fd);
    //                }
    //                r.Close();
    //                donusDeger.sonuc.islemSonuc = true;
    //                for (int i = 0; i < donusDeger.ObjeSayisi; i++)
    //                {
    //                    fd = (FizikselDeger)donusDeger[i];
    //                    r = tfd.YillariAl(yil, muhasebeKod, birimKod, kurumBirim, fd.kod);
    //                    fd.kod = TNS.OrtakFonksiyonlar.KodNoktaKoy("9.9.9.9", fd.kod);
    //                    if (tfd.Hata() == "")
    //                    {
    //                        while (r.Read())
    //                        {
    //                            YilBilgi y = new YilBilgi();
    //                            y.yil = VeriErisim.DegerAl(r, 0, 0); ;
    //                            y.deger = VeriErisim.DegerAl(r, 1, 0);
    //                            if (fd.kod.StartsWith("1") && fd.kod.Length < 3)
    //                                y.deger = 0;
    //                            fd.degerler.Add(y);
    //                        }
    //                        r.Close();
    //                    }
    //                    else
    //                    {
    //                        donusDeger.sonuc.hataStr = tfd.Hata();
    //                        donusDeger.sonuc.islemSonuc = false;
    //                        break;
    //                    }
    //                }
    //            }
    //            else
    //                donusDeger.sonuc.hataStr = tfd.Hata();
    //            ve.VeriErisimKapat();

    //        }
    //        else
    //            donusDeger.sonuc.hataStr = "Bu kurumla (" + kurumBirim + ") ilgili verileri görme yetkiniz bulunmamaktadır.";

    //        return donusDeger;
    //    }

    //    public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, int muhasebeKod, string birimKod, string kurumBirim, TNSCollection fizikselDegerler)
    //    {
    //        Sonuc sonuc = new Sonuc(false, "", "");
    //        if (Servis.YazmaYetkisiVarMi(kullanan, kurumBirim))
    //        {
    //            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, Servis.BaglantiSatiriBul(yil));
    //            TABFizikselDegerlerTasra tfd = new TABFizikselDegerlerTasra(ve);
    //            ve.BeginTransaction();
    //            sonuc = tfd.Sil(yil, muhasebeKod, birimKod, kurumBirim);
    //            if (sonuc.islemSonuc)
    //            {
    //                foreach (FizikselDeger fd in fizikselDegerler)
    //                {
    //                    if (fd.degerGirilsinMi)
    //                    {
    //                        string kod = fd.kod.Replace(".", "");
    //                        foreach (YilBilgi yb in fd.degerler)
    //                        {
    //                            sonuc = tfd.Ekle(yil, muhasebeKod, birimKod, kurumBirim, kod, yb.yil, yb.deger);
    //                            if (!sonuc.islemSonuc)
    //                                break;
    //                        }
    //                    }
    //                    if (!sonuc.islemSonuc)
    //                        break;
    //                }
    //            }

    //            if (sonuc.islemSonuc)
    //                ve.Commit();
    //            else
    //                ve.Rollback();
    //            ve.VeriErisimKapat();
    //        }
    //        else
    //            sonuc.hataStr = "Bu kurumla (" + kurumBirim + ") ilgili veri kaydetme yetkiniz bulunmamaktadır.";
    //        return sonuc;
    //    }
    //}

*/