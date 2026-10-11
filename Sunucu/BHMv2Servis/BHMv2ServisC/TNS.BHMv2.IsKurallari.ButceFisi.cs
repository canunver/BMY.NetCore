using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BKD;
using TNS.UZY;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// Bütçe girilirken bütçe fişine girilen açıklamalar ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BHMv2ButceFisi
    {
        /// <summary>
        /// Bütçe fişleri girilirken her bir tertip için girilen gerekçe yazısını veri tabanına kaydeden yordam.
        /// Yetki kontrolü yapılır. Veri tabanı bağlantısı açılır. Kayıtlı gerekçe silinir. Gelen gerekçe 2500 karakterlik parçalar haline getirilerek veri tabanına kaydedilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Bütçe fişinin tertibi</param>
        /// <param name="gerekce">Bütçe tertibi için istenen ödeneğin gerekçesi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc KaydetGerekce(Kullanici kullanan, Tertip tertip, string[] gerekce)
        {
            int yil = tertip.yil;

            Sonuc sonuc = new Sonuc(false, string.Empty, string.Empty);

            string kurkod = tertip.kodlar[0];

            bool yetkikontrol = BHMv2Genel.YazmaYetkisiVarMi(kullanan, kurkod, "00.00");

            if (!yetkikontrol)
            {
                sonuc.DegerAta(string.Format(ButceHazirlikResI.BHMBF001, tertip.TertipStrOl()));
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            TABBHMBUTCEFISI tabButceFisi = new TABBHMBUTCEFISI(ve);
            ve.BeginTransaction();
            sonuc = tabButceFisi.SilGerekce(tertip);
            if (sonuc.islemSonuc && !string.IsNullOrEmpty(gerekce[1]))
                sonuc = tabButceFisi.KaydetGerekce(tertip, -1, gerekce[1]);
            if (sonuc.islemSonuc && !string.IsNullOrEmpty(gerekce[2]))
                sonuc = tabButceFisi.KaydetGerekce(tertip, -2, gerekce[2]);

            if (sonuc.islemSonuc)
            {
                int yazilan = 0;
                string parca = string.Empty;
                int satir = 0;
                int kalan = gerekce[0].Length;
                int parcaBoy;
                while (kalan > 0)
                {
                    if (kalan > 2500)
                        parcaBoy = 2500;
                    else
                        parcaBoy = kalan;

                    parca = gerekce[0].Substring(yazilan, parcaBoy);
                    yazilan = yazilan + parcaBoy;
                    kalan = kalan - parcaBoy;

                    sonuc = tabButceFisi.KaydetGerekce(tertip, ++satir, parca);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            return sonuc;
        }

        /// <summary>
        /// Verilen yıl ve tertip için istenilen ödenek için girilmiş olan gerekçe döndürülür.
        /// Yetki kontrolü yapılır. Veri tabanı bağlantısı açılır. Kayıtlı gerekçe okunur parçalar birleştirilir ve döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Bütçe fişinin tertibi</param>
        /// <returns>Bütçe tertibi için istenen ödeneğin gerekçesi döndürülür</returns>
        public static string[] ListeleGerekce(Kullanici kullanan, Tertip tertip)
        {
            int yil = tertip.yil;
            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            TABBHMBUTCEFISI tabButceFisi = new TABBHMBUTCEFISI(ve);
            string[] donusDeger = new string[3];

            string yazi = string.Empty;

            IDataReader r = tabButceFisi.ListeleGerekce(tertip);
            if (tabButceFisi.Hata() == string.Empty)
            {
                while (r.Read())
                {
                    int parcaNo = VeriErisim.DegerAl(r, 0, 0);
                    string parca = VeriErisim.DegerAl(r, 1, "");
                    if (parcaNo == -1) donusDeger[1] = parca;
                    else if (parcaNo == -2) donusDeger[2] = parca;
                    else yazi += parca;
                }
            }
            r.Close();
            ve.VeriErisimKapat();
            donusDeger[0] = yazi;
            return donusDeger;
        }

        /// <summary>
        /// Bütçe tertibi için istenen ödeneğin gerekçesi girilirken, satın alınması planlanan malzemeler
        /// kullanıcı tarafından girilir. Bu malzemeleri veri tabanına kaydeden yordam.
        /// Yetki kontrolü yapılır. Veri tabanı bağlantısı açılır. Kayıtlı malzemeler silinir. Gelen malzemeler, eğer miktar sıfırdan büyükse veri tabanına kaydedilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Bütçe fişinin tertibi</param>
        /// <param name="tertipMalzemeleri">Bu tertip ile alınaca malzemelerin bilgilerinin tutulduğu dizi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc KaydetMalzeme(Kullanici kullanan, Tertip tertip, ButceFisiMalzeme[] tertipMalzemeleri)
        {
            Sonuc sonuc = new Sonuc(false, string.Empty, string.Empty);

            //if (tertipMalzemeleri == null || tertipMalzemeleri.Length == 0)
            //{
            //    sonuc.DegerAta(ButceHazirlikResI.BHMBF002);
            //    return sonuc;
            //}

            if (!BHMv2Genel.YazmaYetkisiVarMi(kullanan, tertip.kodlar[0], "00.00"))
            {
                sonuc.DegerAta(string.Format(ButceHazirlikResI.BHMBF001, tertip.TertipStrOl()));
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(tertip.yil), kullanan.KimlikBul());
            TABBHMBUTCEFISI to = new TABBHMBUTCEFISI(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(tertip);
            foreach (ButceFisiMalzeme malzeme in tertipMalzemeleri)
            {
                if (malzeme.miktar > 0)
                    sonuc = to.KaydetMalzeme(tertip, malzeme);
                if (!sonuc.islemSonuc)
                {
                    break;
                }
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen tertip için istenen ödenek gerekçelendirilirken, girilen malzeme bilgilerini veri tabanından okuyup döndüren yordam.
        /// Veri tabanından alınan malzemeler, diziye çevrilir ve döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Kendisi ile ilişkilendirilen malzemelerin döndürüleceği tertip</param>
        /// <returns>Parametre olarak verilen tertip için istenen ödenek gerekçelendirilirken girilen malzeme bilgileri döndürülür.</returns>
        public static ButceFisiMalzeme[] ListeleMalzeme(Kullanici kullanan, Tertip tertip)
        {
            ArrayList al = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(tertip.yil), kullanan.KimlikBul());

            TABBHMBUTCEFISI to = new TABBHMBUTCEFISI(ve);
            IDataReader r = to.ListeleMalzeme(tertip);

            if (to.Hata() == string.Empty)
            {
                while (r.Read())
                {
                    ButceFisiMalzeme m = new ButceFisiMalzeme();

                    m.hesapKod = VeriErisim.DegerAl(r, 6, string.Empty);
                    m.hesapAd = VeriErisim.DegerAl(r, 7, string.Empty);
                    m.miktar = VeriErisim.DegerAl(r, 8, (decimal)0);
                    m.birimFiyat = VeriErisim.DegerAl(r, 9, (decimal)0);
                    m.toplamTutar = VeriErisim.DegerAl(r, 10, (decimal)0);
                    m.olcuBirimKod = VeriErisim.DegerAl(r, 11, 0);
                    m.olcuBirimAd = VeriErisim.DegerAl(r, 12, string.Empty);

                    al.Add(m);
                }
                r.Close();
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(to.Hata());
            }

            ve.VeriErisimKapat();

            return (ButceFisiMalzeme[])al.ToArray(typeof(ButceFisiMalzeme));
        }

        /// <summary>
        /// Tertip yapısında verilen şablona uyan tertiplerin, bütçe fişleri bir önceki yıldan alınır ve kaydedilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Bütçe fişleri kopyalanacak tertip bilgisi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        internal static Sonuc ButceFisiKopyalaOncYil(Kullanici kullanan, Tertip tertip)
        {
            int yil = tertip.yil;

            Sonuc sonuc = new Sonuc(true, string.Empty, string.Empty);

            string kurkod = tertip.kodlar[0];

            bool yetkikontrol = BHMv2Genel.YazmaYetkisiVarMi(kullanan, kurkod, "00.00");

            if (!yetkikontrol)
            {
                sonuc.DegerAta(string.Format(ButceHazirlikResI.BHMBF001, tertip.TertipStrOl()));
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            ve.BeginTransaction();
            if (tertip.tabloKod == ENUMTabloKod.TABPEBGelir)
            {
                TABBHMBUTCEGELIRFISI tabButceFisi = new TABBHMBUTCEGELIRFISI(ve);
                sonuc = tabButceFisi.ButceFisiKopyalaOncYil(tertip);
            }
            else
            {
                TABBHMBUTCEFISI tabButceFisi = new TABBHMBUTCEFISI(ve);
                sonuc = tabButceFisi.ButceFisiKopyalaOncYil(tertip);
            }
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            return sonuc;
        }

        /// <summary>
        /// Verilen yıl ve tertip şablonu için, girilmiş bütçe fişi sayısını döndüren yordam.
        /// </summary>
        /// <param name="tertip">Bütçe fişleri sayısı bulunacak tertip bilgisi.</param>
        /// <returns>Bütçe tertibi için kayıtlı bütçe fişi sayısı döndürülür.</returns>
        internal static int ButceFisiSayisiBul(Tertip tertip)
        {
            int yil = tertip.yil;

            int sonuc = 0;


            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(yil), null);
            if (tertip.tabloKod == ENUMTabloKod.TABPEBGelir)
            {
                TABBHMBUTCEGELIRFISI tabButceFisi = new TABBHMBUTCEGELIRFISI(ve);
                sonuc = tabButceFisi.ButceFisiSayisiBul(tertip);
            }
            else
            {
                TABBHMBUTCEFISI tabButceFisi = new TABBHMBUTCEFISI(ve);
                sonuc = tabButceFisi.ButceFisiSayisiBul(tertip);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        internal static Tertip OncekiSonrakiTertip(VeriErisim veriErisim, Kullanici kullanan, int yil, Tertip simdikiTertip, Tertip kistasTertip, int yon)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            Tertip donenTertip = null;
            kistasTertip.NoktalariAt();
            kistasTertip.DiziKontrol();
            TABBHMTERTIPLISTESIGIDER to = new TABBHMTERTIPLISTESIGIDER(ve);
            IDataReader r = to.IlkTertip(yil, kistasTertip, simdikiTertip, yon);
            if (to.Hata() == "")
            {
                if (r.Read())
                {
                    string tertipStr = VeriErisim.DegerAl(r, 0, "");
                    if (!string.IsNullOrEmpty(tertipStr))
                    {
                        donenTertip = new Tertip(kistasTertip.yil, kistasTertip.tabloKod);
                        donenTertip.TertipAta(tertipStr);
                    }
                }
                r.Close();
            }
            if (donenTertip != null)
            {
                IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
                donenTertip.Noktala();
                donenTertip.AdlariBul(servisUZY);
            }
            ve.VeriErisimKapat();
            return donenTertip;
        }
    }
}

/*
        //public Sonuc KaydetGerekceAktar(Kullanici kullanan, int yil, string tertip, string giris)
        //{

        //    Sonuc sonuc = new Sonuc(false, string.Empty, string.Empty);

        //    bool yetkikontrol = false;

        //    string kurkod = string.Empty;

        //    if (tertip != string.Empty)
        //    {
        //        string[] tertipdegerler = tertip.Split('-');
        //        kurkod = tertipdegerler[0];
        //    }
        //    else
        //    {
        //        sonuc.DegerAta("Hatalı tertip geldi");
        //    }

        //    yetkikontrol = BHMv2Genel.YazmaYetkisiVarMi(kullanan, kurkod, "00.00");


        //    if (!yetkikontrol)
        //    {
        //        sonuc.DegerAta("Bu tertipe yazma yetkiniz yoktur. (" + tertip + ")");
        //        return sonuc;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
        //    TABBHMBUTCEFISI tabButceFisi = new TABBHMBUTCEFISI(ve);
        //    ve.BeginTransaction();
        //    tertip = tertip.Replace(".", string.Empty);
        //    sonuc = tabButceFisi.SilGerekceAktar(yil, tertip);
        //    if (sonuc.islemSonuc)
        //    {
        //        int yazilan = 0;
        //        string parca = string.Empty;
        //        int satir = 0;
        //        int kalan = giris.Length;
        //        int parcaBoy;
        //        while (kalan > 0)
        //        {
        //            if (kalan > 2500)
        //                parcaBoy = 2500;
        //            else
        //                parcaBoy = kalan;

        //            parca = giris.Substring(yazilan, parcaBoy);
        //            yazilan = yazilan + parcaBoy;
        //            kalan = kalan - parcaBoy;

        //            sonuc = tabButceFisi.KaydetGerekceAktar(yil, tertip.Replace(".", string.Empty), ++satir, parca);
        //            if (!sonuc.islemSonuc)
        //                break;
        //        }
        //    }
        //    if (sonuc.islemSonuc)
        //        ve.Commit();
        //    else
        //        ve.Rollback();

        //    return sonuc;
        //}

        //public string ListeleGerekceAktar(Kullanici kullanan, int yil, string tertip)
        //{
        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
        //    TABBHMBUTCEFISI tabButceFisi = new TABBHMBUTCEFISI(ve);

        //    int satir;
        //    string yazi = string.Empty;
        //    tertip = tertip.Replace(".", string.Empty);

        //    IDataReader r = tabButceFisi.ListeleGerekceAktar(yil, tertip);
        //    if (tabButceFisi.Hata() == string.Empty)
        //    {
        //        while (r.Read())
        //        {
        //            satir = r.GetInt32(0);
        //            yazi += r.GetString(1);
        //        }
        //    }
        //    r.Close();
        //    ve.VeriErisimKapat();
        //    return yazi;
        //}
*/