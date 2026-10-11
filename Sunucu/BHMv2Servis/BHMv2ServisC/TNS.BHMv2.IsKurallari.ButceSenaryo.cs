using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.DEG;
using TNS.KYM;
using TNS.Veri;
using TNS.BHMv2.IsKurallari.Veri;
using System.Collections;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// Tüm bütçe rakamlarının farklı sürümlerini tutan senaryoların
    /// tanımlarıyla ilgili işlemlerin yapıldığı yordamlarını tutan sınıf.
    /// </summary>
    public class BHMv2ButceSenaryo
    {
        private static string kodYapisi = "99.99";

        /// <summary>
        /// Kullanıcının yetkisi ve alan değerlerinin geçerliliği kontrol edildikten sonra, 
        /// var olan senaryo tanım bilgisi değiştirilir veya senaryo kodu sadece aşamadan oluşuyor ise yeni senaryo tanımı oluşturulur.
        /// Senaryo kodları iki hane aşama ve iki hane aşama içi sıra numarasından oluşmaktadır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceSenaryo">Bütçe senaryo tanım bilgisi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ButceSenaryo butceSenaryo)
        {
            Sonuc donusDeger = new Sonuc();

            if (!YazmaYetkisiVarMi(kullanan))
            {
                donusDeger.DegerAta(ButceHazirlikResI.BHMBS001);
                return donusDeger;
            }

            string hata = string.Empty;
            if (butceSenaryo.kod == null) butceSenaryo.kod = "";
            else butceSenaryo.kod = butceSenaryo.kod.Trim();
            if (string.IsNullOrEmpty(butceSenaryo.kod))
                hata = string.Empty;
            else if (butceSenaryo.kod.Length != 2 && butceSenaryo.kod.Length != 5)
                hata = string.Format(ButceHazirlikResI.BHMBS003, kodYapisi) + "<br>";
            if (string.IsNullOrEmpty(butceSenaryo.ad.Trim()))
                hata += ButceHazirlikResI.BHMBS004 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.DegerAta(hata);
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            TABBHMBUTCESENARYO tabBS = new TABBHMBUTCESENARYO(ve);

            try
            {
                ve.BeginTransaction();
                if (butceSenaryo.kod.Length < 5)
                {
                    butceSenaryo.kod = tabBS.AnahtarBul(butceSenaryo.kod);
                    donusDeger = tabBS.Kaydet(butceSenaryo);
                }
                else
                    donusDeger = tabBS.Guncelle(butceSenaryo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.Message;
            }

            if (donusDeger.islemSonuc)
            {
                donusDeger.anahtar = OrtakFonksiyonlar.KodNoktaKoy("99.99", butceSenaryo.kod);
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Kullanıcının, Senaryo tanım bilgileri üzerinde değişiklik yapma hakkını bulup döndüren yordam.
        /// Kullanıcı sistem yöndetici ise değişiklik yapma hakkı vardır.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcıya ait bilgiler</param>
        /// <returns>Değişiklik yapma hakkı varsa doğru, yoksa yanlış döndürülür.</returns>
        private static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Kullanıcının yetkisi kontrol edildikten sonra var olan senaryo tanım bilgisi silinir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceSenaryo">Bütçe senaryo tanım bilgisi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ButceSenaryo butceSenaryo)
        {
            Sonuc donusDeger = new Sonuc();

            if (!YazmaYetkisiVarMi(kullanan))
            {
                donusDeger.DegerAta(ButceHazirlikResI.BHMBS001);
            }

            if (string.IsNullOrEmpty(butceSenaryo.kod.Trim()))
                donusDeger.DegerAta(ButceHazirlikResI.BHMBS005 + "<br>");


            if (!donusDeger.islemSonuc)
            {
                return donusDeger;
            }

            //Kod kullanılmış mı kontrolü olmalı

            VeriErisim veOdenek = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(0), kullanan.KimlikBul());
            TABBHMODENEKGIDER tab = new TABBHMODENEKGIDER(veOdenek);
            double senSay = tab.SenaryoKayitSayisi(butceSenaryo.kod);
            veOdenek.VeriErisimKapat();

            if (senSay > 0)
                donusDeger.DegerAta(ButceHazirlikResI.BHMBS006 + "<br>");
            if (!donusDeger.islemSonuc)
            {
                return donusDeger;
            }


            VeriErisim veSozluk = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            TABBHMBUTCESENARYO tabBS = new TABBHMBUTCESENARYO(veSozluk);
            veSozluk.BeginTransaction();

            try
            {
                donusDeger = tabBS.Sil(butceSenaryo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.Message;
            }

            if (donusDeger.islemSonuc)
                veSozluk.Commit();
            else
                veSozluk.Rollback();

            veSozluk.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Verilen senaryo tanım kıstasına uyan senaryo tanımlarını döndüren yordam.
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceSenaryo">Kıstas olarak kullanılacak bütçe senaryo tanım bilgisi</param>
        /// <returns>Verilen bütçe senaryo tanım kıstasına uyan senaryolar döndürülür.</returns>
        public static ButceSenaryo[] Listele(VeriErisim veriErisim, Kullanici kullanan, ButceSenaryo butceSenaryo)
        {
            ArrayList donusDeger = new ArrayList();
            string dilEk = TNS.BKD.Arac.AlanEkBul();
            IDataReader dr = null;
            donusDeger.Add(new ButceSenaryo("00.00", TNS.BHMv2.ButceHazirlikResI.BHMASAMA0000));
            donusDeger.Add(new ButceSenaryo("01.00", TNS.BHMv2.ButceHazirlikResI.BHMASAMA0100));
            donusDeger.Add(new ButceSenaryo("02.00", TNS.BHMv2.ButceHazirlikResI.BHMASAMA0200));
            donusDeger.Add(new ButceSenaryo("03.00", TNS.BHMv2.ButceHazirlikResI.BHMASAMA0300));
            donusDeger.Add(new ButceSenaryo("04.00", TNS.BHMv2.ButceHazirlikResI.BHMASAMA0400));
            donusDeger.Add(new ButceSenaryo("05.00", TNS.BHMv2.ButceHazirlikResI.BHMASAMA0500));
            donusDeger.Add(new ButceSenaryo("06.00", TNS.BHMv2.ButceHazirlikResI.BHMASAMA0600));

            bool devam = true;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), TNS.KYM.Arac.KimlikBul(kullanan));
            TABBHMBUTCESENARYO tabBS = new TABBHMBUTCESENARYO(ve);
            while (devam)
            {
                try
                {
                    dr = tabBS.Listele(butceSenaryo, dilEk);
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            ButceSenaryo gelen = new ButceSenaryo(OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, VeriErisim.DegerAl(dr, (int)TABBHMBUTCESENARYO.ENUMAlanlar.KOD, string.Empty)), VeriErisim.DegerAl(dr, (int)TABBHMBUTCESENARYO.ENUMAlanlar.AD, string.Empty));
                            donusDeger.Add(gelen);
                        }
                        dr.Close();
                        devam = false;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(dilEk))
                        {
                            ve.VeriErisimKapat();
                            throw new Exception(ve.hata);
                        }
                        ve.hata = "";
                        dilEk = "";
                        devam = true;
                    }
                }
                catch (Exception e)
                {
                    ve.VeriErisimKapat();
                    throw e;
                }
            }
            ve.VeriErisimKapat();
            donusDeger.Sort();
            return (ButceSenaryo[])donusDeger.ToArray(typeof(ButceSenaryo));
        }

        /// <summary>
        /// Verilen senaryo kodunun açıklamasını bulan ve döndüren yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="senaryoKod">Adı bulunacak senaryonun kodu</param>
        /// <returns>Verilen senaryo kodunun adı döndürülür.</returns>
        internal static string AdBul(VeriErisim veriErisim, string senaryoKod)
        {
            string donusDeger = string.Empty;
            string dilEk = TNS.BKD.Arac.AlanEkBul();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), null);
            TABBHMBUTCESENARYO tabBS = new TABBHMBUTCESENARYO(ve);
            IDataReader dr = null;
            bool devam = true;
            while (devam)
            {
                devam = false;
                Exception ex = null;
                try
                {
                    dr = tabBS.Oku(senaryoKod);
                }
                catch (Exception e)
                {
                    ex = e;
                }

                if (dr == null || ex != null)
                {
                    if (string.IsNullOrEmpty(dilEk))
                    {
                        ve.VeriErisimKapat();
                        if (ex != null) throw ex;
                        throw new Exception(ve.hata);
                    }
                    ve.hata = "";
                    dilEk = "";
                    devam = true;

                }
            }
            if (dr.Read())
            {
                donusDeger = VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.AD, string.Empty);
            }
            dr.Close();
            ve.VeriErisimKapat();
            if (string.IsNullOrEmpty(donusDeger))
            {
                donusDeger = TNS.BHMv2.ButceHazirlikResI.ResourceManager.GetString("BHMASAMA" + senaryoKod.Replace(".", ""));
                if (string.IsNullOrEmpty(donusDeger))
                {
                    donusDeger = "TANIMSIZ";
                }
            }

            return donusDeger;
        }

        internal static ButceSenaryo[] SenaryolariBul(VeriErisim veriErisim, int tasnifSiraNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), null);
            TABBHMBUTCETASNIF tab = new TABBHMBUTCETASNIF(ve);
            IDataReader dr = null;

            dr = tab.TasnifIliskileriListele(tasnifSiraNo);
            ArrayList al = new ArrayList();
            if (dr != null)
            {
                while (dr.Read())
                {
                    ButceSenaryo bs = new ButceSenaryo();
                    string tut = VeriErisim.DegerAl(dr, 1, string.Empty);
                    bs.kod = OrtakFonksiyonlar.KodNoktaKoy("99.99", tut);
                    bs.ad = VeriErisim.DegerAl(dr, 2, string.Empty);
                    if (string.IsNullOrEmpty(bs.ad))
                    {
                        bs.ad = TNS.BHMv2.ButceHazirlikResI.ResourceManager.GetString("BHMASAMA" + tut);
                        if (string.IsNullOrEmpty(bs.ad))
                        {
                            bs.ad = "TANIMSIZ";
                        }
                    }
                    al.Add(bs);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return (ButceSenaryo[])al.ToArray(typeof(ButceSenaryo));
        }
    }
}


/*

        //public static ButceSenaryo[] ListeleSeviye(VeriErisim veriErisim, Kullanici kullanan, ButceSenaryo bs, int seviye)
        //{
        //    ArrayList donusDeger = new ArrayList();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
        //    TABBHMBUTCESENARYO tabBS = new TABBHMBUTCESENARYO(ve);
        //    IDataReader dr = null;

        //    try
        //    {
        //        dr = tabBS.ListeleSeviye(bs, seviye);
        //    }
        //    catch (Exception e)
        //    {
        //        ve.VeriErisimKapat();
        //        throw e;
        //    }

        //    while (dr.Read())
        //    {
        //        ButceSenaryo gelen = new ButceSenaryo();
        //        gelen.kod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.KOD, string.Empty));
        //        gelen.ad = VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.AD, string.Empty);

        //        donusDeger.Add(gelen);
        //    }

        //    if (donusDeger.Count == 0)
        //        return null;

        //    dr.Close();
        //    ve.VeriErisimKapat();
        //    return (ButceSenaryo[])donusDeger.ToArray(typeof(ButceSenaryo));
        //}

        //public static ObjectArray ButceSenaryoDondur(VeriErisim veriErisim, Kullanici kullanan, string kod)
        //{
        //    ObjectArray donusDeger = new ObjectArray();

        //    //Yetki kontrolü eklenecek

        //    if (string.IsNullOrEmpty(kod))
        //        throw new Exception("Kod alanı boş bırakılamaz.<br>");

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
        //    TABBHMKURUMSALYAPI tabKY = new TABBHMKURUMSALYAPI(ve);
        //    IDataReader dr = null;

        //    try
        //    {
        //        dr = tabKY.ButceSenaryoDondur(kod);

        //        if (dr.Read())
        //        {
        //            ButceSenaryo gelen = new ButceSenaryo();
        //            gelen.kod = VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.KOD, string.Empty);
        //            gelen.kod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, gelen.kod);
        //            gelen.ad = VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.AD, string.Empty);
        //            gelen.saymanlikKod = VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.SAYMANLIKKOD, -1);
        //            gelen.birimKod = VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.BIRIMKOD, string.Empty);
        //            gelen.kurKod = VeriErisim.DegerAl(dr, (int)TABBHMKURUMSALYAPI.ENUMAlanlar.KURKOD, string.Empty);
        //            gelen.kurKod = OrtakFonksiyonlar.KodNoktaKoy(kurKodYapisi, gelen.kurKod);

        //            donusDeger.objeler.Add(gelen);
        //        }

        //        dr.Close();
        //        ve.VeriErisimKapat();
        //        return donusDeger;
        //    }
        //    catch (Exception e)
        //    {
        //        ve.VeriErisimKapat();
        //        throw new Exception(e.Message);
        //    }
        //}



*/