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
    /// Muhasebattan gelen harcamalara, Alt faaliyet kodu eklemek için kullanılan yordamları tutan sınıf
    /// </summary>
    public class HarcamaDagitimAltFaaliyet
    {
        /// <summary>
        /// Tertip, alt faaliyet yüzdeleri kullanılarak, verilen yıl ve aydaki harcama gerçekleşmeleri 
        /// alt faaliyet bazında hesaplayıp ve veri tabanına yazan yordamı, eğer yüzdesi olmayan tertip yoksa çağıran yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceYili">Her bütçe yılı için ayrı veri tabanları tutulduğundan, her bütçe yılı için ayrı parametreler kullanılabilmektedir.</param>
        /// <param name="yil">Harcama gerçekleşmeleri girilen oranlarla alt faaliyetlere dağıtılacağı yıl</param>
        /// <param name="ay">Harcama gerçekleşmeleri girilen oranlarla alt faaliyetlere dağıtılacağı ay</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc HarcamalaraAltFaalEkle(Kullanici kullanan, int butceYili, int yil, int ay)
        {
            Sonuc sonuc = new Sonuc(true, string.Empty, string.Empty);

            OdenekBilgi[] yuzdesiOlmayanlar = ListeleYuzdesiOlmayanlar(kullanan, butceYili, yil, ay, false);
            if (yuzdesiOlmayanlar.Length == 0)
            {
                sonuc = KaydetYuzde(kullanan, butceYili, yil, ay);
            }
            else
            {
                sonuc.DegerAta(ButceHazirlikResI.BHMHDA001);
            }
            return sonuc;
        }

        /// <summary>
        /// Tertip, alt faaliyet yüzdeleri kullanılarak, verilen yıl ve aydaki harcama gerçekleşmeleri 
        /// alt faaliyet bazında hesaplanır ve veri tabanına yazılır.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceYili">Her bütçe yılı için ayrı veri tabanları tutulduğundan, her bütçe yılı için ayrı parametreler kullanılabilmektedir.</param>
        /// <param name="yil">Harcama gerçekleşmeleri girilen oranlarla alt faaliyetlere dağıtılacağı yıl</param>
        /// <param name="ay">Harcama gerçekleşmeleri girilen oranlarla alt faaliyetlere dağıtılacağı ay</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc KaydetYuzde(Kullanici kullanan, int butceYili, int yil, int ay)
        {
            Sonuc sonuc = new Sonuc(true, string.Empty, string.Empty);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(butceYili), kullanan.KimlikBul());
            TABBHMHARCAMADAGITIMPEB to = new TABBHMHARCAMADAGITIMPEB(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(yil, ay);
            if (sonuc.islemSonuc)
            {
                sonuc = to.Kaydet(yil, ay);
            }
            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = ButceHazirlikResI.BHMHDA002;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = string.Format(ButceHazirlikResI.BHMHDA003, sonuc.hataStr);
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Muhasebat harcamalarından gelen tertiplerden, alt faaliyetler ilişkisi hiç kurulmamış 
        /// olanların listesi dizi olarak döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceYili">Her bütçe yılı için ayrı veri tabanları tutulduğundan, her bütçe yılı için ayrı parametreler kullanılabilmektedir.</param>
        /// <param name="yil">Tertip alt faaliyet eşleşmesi olmayan tertiplerin bulunacağı yıl</param>
        /// <param name="ay">Tertip alt faaliyet eşleşmesi olmayan tertiplerin bulunacağı ay</param>
        /// <param name="hepsi">false ise ilk yüzdesi olmayan bulunduğunda çıkılsın, true ise hepsi diziye doldurulsun</param>
        /// <returns>Tertip alt faaliyet eşleşmesi olmayan tertiplerin listesi</returns>
        public static OdenekBilgi[] ListeleYuzdesiOlmayanlar(Kullanici kullanan, int butceYili, int yil, int ay, bool hepsi)
        {
            ArrayList al = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(butceYili), kullanan.KimlikBul());
            TABBHMHARCAMADAGITIMPEB to = new TABBHMHARCAMADAGITIMPEB(ve);

            IDataReader r = to.ListeleYuzdesiOlmayanlar(yil, ay);
            if (r != null)
            {
                while (r.Read())
                {
                    OdenekBilgi ob = new OdenekBilgi(yil, ENUMTabloKod.TABPEBOdenek);
                    ob.tertip.kodlar[0] = VeriErisim.DegerAl(r, 0, string.Empty);     //KURKOD
                    ob.tertip.kodlar[1] = VeriErisim.DegerAl(r, 1, string.Empty);     //FONKOD
                    ob.tertip.kodlar[2] = VeriErisim.DegerAl(r, 2, string.Empty);     //FINKOD
                    ob.tertip.kodlar[3] = VeriErisim.DegerAl(r, 3, string.Empty);     //EKOKOD
                    al.Add(ob);

                    if (!hepsi)
                        break;
                }
                r.Close();
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(string.Format(ButceHazirlikResI.BHMHDA004, ve.hata));
            }

            ve.VeriErisimKapat();

            return (OdenekBilgi[])al.ToArray(typeof(OdenekBilgi));
        }

        /// <summary>
        /// Muhasebattan gelen harcamaları, alt faaliyet kodlarının eklenebilmesi için,
        /// muhasebat harcamaları tertiplerine alt faaliyet eklenebilmesi için, tüm tertiplerin alt faaliyet oranlarını
        /// veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceYili">Her bütçe yılı için ayrı veri tabanları tutulduğundan, her bütçe yılı için ayrı parametreler kullanılabilmektedir.</param>
        /// <param name="yil">Dağılım yüzdesi kaydedilecek yıl bilgisi</param>
        /// <param name="ay">Dağılım yüzdesi kaydedilecek ay bilgisi</param>
        /// <param name="oranlar">tüm tertipler için, pebleri farklı olan ve oranları tutan dizi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc KaydetTumTertipler(Kullanici kullanan, int butceYili, int yil, int ay, OdenekBilgi[] oranlar)
        {
            return KaydetUret(kullanan, butceYili, yil, ay, oranlar, true);
        }

        /// <summary>
        /// Muhasebattan gelen harcamaları, alt faaliyet kodlarının eklenebilmesi için,
        /// muhasebat harcamaları tertiplerine alt faaliyet eklenebilmesi için, verilen tertibin alt faaliyet oranlarını
        /// veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceYili">Her bütçe yılı için ayrı veri tabanları tutulduğundan, her bütçe yılı için ayrı parametreler kullanılabilmektedir.</param>
        /// <param name="yil">Dağılım yüzdesi kaydedilecek yıl bilgisi</param>
        /// <param name="ay">Dağılım yüzdesi kaydedilecek ay bilgisi</param>
        /// <param name="oranlar">kur, fon, fin ve eko kısmı aynı, pebleri farklı olan ve oranları tutan dizi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc KaydetTekTertip(Kullanici kullanan, int butceYili, int yil, int ay, OdenekBilgi[] oranlar)
        {
            return KaydetUret(kullanan, butceYili, yil, ay, oranlar, false);
        }

        /// <summary>
        /// Muhasebattan gelen harcamaları, alt faaliyet kodlarının eklenebilmesi için,
        /// muhasebat harcamaları tertiplerine alt faaliyet eklenebilmesi için, verilen tertibin alt faaliyet oranlarını
        /// veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceYili">Her bütçe yılı için ayrı veri tabanları tutulduğundan, her bütçe yılı için ayrı parametreler kullanılabilmektedir.</param>
        /// <param name="yil">Dağılım yüzdesi kaydedilecek yıl bilgisi</param>
        /// <param name="ay">Dağılım yüzdesi kaydedilecek ay bilgisi</param>
        /// <param name="oranlar">kur, fon, fin ve eko kısmı aynı, pebleri farklı veya tüm tertipleri içeren ve oranları tutan dizi</param>
        /// <param name="tumTertipler">true ise oranlar dizisi tüm tertipleri tutar, false ise kur, fon, fin ve eko kısmı aynı, pebleri farklı olan tertipleri tutar.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc KaydetUret(Kullanici kullanan, int butceYili, int yil, int ay, OdenekBilgi[] oranlar, bool tumTertipler)
        {
            Sonuc sonuc = new Sonuc(false, string.Empty, string.Empty);

            if (oranlar.Length > 0)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(butceYili), kullanan.KimlikBul());
                TABBHMHARCAMADAGITIMORAN to = new TABBHMHARCAMADAGITIMORAN(ve);

                ve.BeginTransaction();

                OdenekBilgi ob = (OdenekBilgi)oranlar[0];
                if (tumTertipler)
                {
                    sonuc = to.Sil(yil, ay, null);
                }
                else
                {
                    sonuc = to.Sil(yil, ay, ob.tertip);
                }

                if (sonuc.islemSonuc)
                {
                    foreach (OdenekBilgi obIc in oranlar)
                    {
                        if (sonuc.islemSonuc)
                        {
                            sonuc = to.Kaydet(yil, ay, obIc);
                        }
                    }
                }
                if (sonuc.islemSonuc)
                {
                    sonuc.bilgiStr = ButceHazirlikResI.BHMHDA005;
                    ve.Commit();
                }
                else
                {
                    sonuc.hataStr = string.Format(ButceHazirlikResI.BHMHDA006, sonuc.hataStr);
                    ve.Rollback();
                }

                ve.VeriErisimKapat();
            }
            return sonuc;
        }

        /// <summary>
        /// Kurumsal, fonksiyonel, finans kodu ve ekonomik (4 düzey) kodu verilen dizgi olarak verilen tertibin
        /// alt faaliyet kodu bazında verilen tertibin harcamalarının verilen yıl ve ay için dağıtım yüzdesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceYili">Her bütçe yılı için ayrı veri tabanları tutulduğundan, her bütçe yılı için ayrı parametreler kullanılabilmektedir.</param>
        /// <param name="yil">Dağılım yüzdesi girilecek yıl bilgisi</param>
        /// <param name="ay">Dağılım yüzdesi girilecek ay bilgisi</param>
        /// <param name="tertip">Kurumsal, fonksiyonel, finans kodu ve ekonomik (4 düzey) koddan oluşan tertip</param>
        /// <returns>Verilen tertibe ait alt faaliyet kodları ve alt faaliyetin oranı dizide döndürülür.</returns>
        public static OdenekBilgi[] Listele(Kullanici kullanan, int butceYili, int yil, int ay, Tertip tertip)
        {
            ArrayList al = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(butceYili), kullanan.KimlikBul());
            TABBHMHARCAMADAGITIMORAN tabDagitimOran = new TABBHMHARCAMADAGITIMORAN(ve);

            IDataReader r = tabDagitimOran.Listele(yil, ay, tertip);
            if (r != null)
            {
                while (r.Read())
                {
                    OdenekBilgi ob = new OdenekBilgi(yil, ENUMTabloKod.TABPEBOdenek);
                    string pebKod = VeriErisim.DegerAl(r, 0, string.Empty);           //PEB
                    double oran = VeriErisim.DegerAl(r, 1, (double)0.0);    //ORAN                   
                    ob.tertip.kodlar[4] = pebKod.Replace(".", string.Empty);
                    ob.ekRakamlar.Add(oran);

                    al.Add(ob);
                }
                r.Close();
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(string.Format(ButceHazirlikResI.BHMHDA007, ve.hata));
            }

            ve.VeriErisimKapat();

            return (OdenekBilgi[])al.ToArray(typeof(OdenekBilgi));
        }
    }
}