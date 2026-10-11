using System;
using System.Collections.Generic;
using System.Text;
using TNS.BKD;
using TNS.KYM;
using System.Data;
using TNS.DEG;
using TNS.BKD.IsKurallari;

namespace TNS.BKD.IsKurallari
{
    /// <summary>
    /// Bütçe kodları sunucu servisi tarafından sağlanan hizmetleri gerçekleştiren sınıf. Bu sınıfdaki yordamlar ilgili işlem sınıfındaki yordamı çağırır.
    /// </summary>
    [Serializable()]
    public class BKDServis : MarshalByRefObject, IBKDServis
    {
        /// <summary>
        /// Verilen arama kıstası kullanılarak, kod ve addan arama yaparak, uyan kodları listeleyen yordam.
        /// </summary>
        /// <param name="dilKod">Adların aranacağı ve listeleneceği dil.</param>
        /// <param name="yil">Kodların listeleneceği yıl.</param>
        /// <param name="kodTuru">Listelenecek kodun tür kodu (Değişken.xml'den kodların adları ve numaraları bulunabilir.)</param>
        /// <param name="aramaKistas">Listelenecek kodların uyacağı arama dizgisi. Arama dizgisi boşluklardan bölünür. İlk parça hem kod için hem de ad için kullanılır. Diğer parçalar sadece ad için arama da kullanılır.</param>
        /// <returns>Verilen kriterlere uyan kodların listesi döndürülür.</returns>
        public ABSKod[] KodAra(string dilKod, int yil, string kodTuru, string aramaKistas)
        {
            KYM.Arac.KulturDegistir(dilKod);
            return KodServisi.KodAra(null, dilKod, yil, kodTuru, aramaKistas);
        }

        public ABSKod[] KodAra(string dilKod, int yil, string kodTuru, string aramaKistas, bool adaGoreSirala)
        {
            KYM.Arac.KulturDegistir(dilKod);
            return KodServisi.KodAra(null, dilKod, yil, kodTuru, aramaKistas, adaGoreSirala);
        }

        /// <summary>
        /// Kendisine parametre olarak gecirilen kodTuru'ne ve yil'a ait, ve verilen kod'un bir alt seviyesindeki kodları 
        /// listeleyen yordam. ustKod parametresi boş ise ilk seviye kodlar listelenir.
        /// </summary>
        /// <param name="yil">Kod değerlerinin listeleneceği yıl.</param>
        /// <param name="kodTuru">Listelenecek kodun tür kodu (Değişken.xml'den kodların adları ve numaraları bulunabilir.)</param>
        /// <param name="ustKod">Listelenecek kodun başladığı karakterlerin uyacağı kod parametresi.</param>
        /// <returns>Verilen kriterlere uyan kodların listesi ve sonuc bilgisi döndürülür.</returns>
        public ABSKodlarVAN KodListele(int yil, string kodTuru, string ustKod)
        {
            return KodListeleSeviye(yil, kodTuru, ustKod, 0, false);
        }

        /// <summary>
        /// Kendisine parametre olarak gecirilen kodTuru'ne ve yil'a ait, ve verilen kod'un bir alt seviyesinden verilen seviyeye kadar olan kodları 
        /// listeleyen yordam. ustKod parametresi boş ise listeleme ilk seviyeden başlar.
        /// </summary>
        /// <param name="yil">Kod değerlerinin listeleneceği yıl.</param>
        /// <param name="kodTuru">Listelenecek kodun tür kodu (Değişken.xml'den kodların adları ve numaraları bulunabilir.)</param>
        /// <param name="ustKod">Listelenecek kodun başladığı karakterlerin uyacağı kod parametresi.</param>
        /// <param name="seviye">Listelenecek kodun seviyesini gösteren parametre.</param>
        /// <returns>Verilen kriterlere uyan kodların listesi ve sonuc bilgisi döndürülür.</returns>
        public ABSKodlarVAN KodListeleSeviye(int yil, string kodTuru, string ustKod, int seviye)
        {
            return KodListeleSeviye(yil, kodTuru, ustKod, seviye, false);
        }

        /// <summary>
        /// <summary>
        /// Kendisine parametre olarak gecirilen kodTuru'ne ve yil'a ait, ve verilen kod'un 
        ///     - seviye 0 ise kodun bir alt kırılımındaki kodları
        ///     - seviye 0 değilse, 
        ///         - sadeceDetay ise verilen seviyedeki kodları
        ///         - sadeceDetay değil ise, kod ile seviye arasındaki tüm kodları
        /// listeleyen yordam. ustKod parametresi boş ise listeleme ilk seviyeden başlar.
        /// Veri tabanından verilen kodların listesi alınır ve kodla birlikte gelen her dil için kod yapısına ad eklenir.
        /// Daha sonra her bir dil için açıklama bulunur ve kod yapısına eklenir.
        /// </summary>
        /// </summary>
        /// <param name="yil">Kod değerlerinin listeleneceği yıl.</param>
        /// <param name="kodTuru">Listelenecek kodun tür kodu (Değişken.xml'den kodların adları ve numaraları bulunabilir.)</param>
        /// <param name="ustKod">Listelenecek kodun başladığı karakterlerin uyacağı kod parametresi.</param>
        /// <param name="seviye">Listelenecek kodun seviyesini gösteren parametre.</param>
        /// <param name="sadeceDetay">Bu parametre doğru ise verilen seviye kodları döndürülür. Değilse ustkodla seviye arasındaki tüm kodlar getirilir.</param>
        /// <returns>Verilen kriterlere uyan kodların listesi ve sonuc bilgisi döndürülür.</returns>
        public ABSKodlarVAN KodListeleSeviye(int yil, string kodTuru, string ustKod, int seviye, bool sadeceDetay)
        {
            return KodListeleSeviye(yil, kodTuru, ustKod, seviye, sadeceDetay, false);
        }

        public ABSKodlarVAN KodListeleSeviye(int yil, string kodTuru, string ustKod, int seviye, bool sadeceDetay, bool adaGoreSirala)
        {
            return KodListeleSeviye(yil, kodTuru, ustKod, seviye, sadeceDetay, adaGoreSirala, "", "", "", true);
        }

        public ABSKodlarVAN KodListeleSeviye(int yil, string kodTuru, string ustKod, int seviye, bool sadeceDetay, bool adaGoreSirala, string filtreAd, string filtreKod, string dilKod)
        {
            return KodListeleSeviye(yil, kodTuru, ustKod, seviye, sadeceDetay, adaGoreSirala, filtreAd, filtreKod, dilKod, true);
        }

        public ABSKodlarVAN KodListeleSeviye(int yil, string kodTuru, string ustKod, int seviye, bool sadeceDetay, bool adaGoreSirala, string filtreAd, string filtreKod, string dilKod, bool hiyerarsik)
        {
            return KodListeleSeviyeIkiYil(yil, kodTuru, ustKod, seviye, sadeceDetay, adaGoreSirala, filtreAd, filtreKod, dilKod, hiyerarsik, false, "", true);
        }

        public ABSKodlarVAN KodListeleSeviyeIkiYil(int yil, string kodTuru, string ustKod, int seviye, bool sadeceDetay, bool adaGoreSirala, string filtreAd, string filtreKod, string dilKod, bool hiyerarsik, bool ikiYil, string baslangicFiltreKodAd, bool aciklamaEkle)
        {
            try
            {
                KYM.Arac.KulturDegistir(dilKod);
                return KodServisi.KodListeleSeviye(null, yil, kodTuru, ustKod, seviye, sadeceDetay, adaGoreSirala, filtreAd, filtreKod, hiyerarsik, ikiYil, baslangicFiltreKodAd, aciklamaEkle);
            }
            catch (Exception e)
            {
                ABSKodlarVAN donusDeger = new ABSKodlarVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Verilen kod türü için verilen yıldaki kayıt sayısını bulup döndüren yordam.
        /// </summary>
        /// <param name="yil">Kod sayısının bulunacağı yıl.</param>
        /// <param name="kodTuru">Kodların ait olduğu kod türü. Bu bilgi kullanılarak veri tabanı bilgileri bulunur.</param>
        /// <returns>İşlem sonucunda bulunan sayı döndürülür.</returns>
        public int ButceKodSatirSayisiBul(int yil, string kodTuru)
        {
            try
            {
                return KodServisi.ButceKodSatirSayisiBul(null, yil, kodTuru);
            }
            catch
            {
                return -1;
            }
        }


        /// <summary>
        /// Verilen bir kuruma ait birimleri listeleyen yordam. Bu yordam verilen kullanıcının görebileceği birimleri listeler.
        /// </summary>
        /// <param name="kullanan">Görebileceği birimleri listelenecek kullanıcı bilgilerini tutan sınıf.</param>
        /// <param name="yil">Birimlerin listeleneceği yıl.</param>
        /// <param name="kurum">Birimlerin listeleneceği kurum.</param>
        /// <param name="altDuzey">En alt düzeydeki birimler listelenir.</param>
        /// <returns>Verilen kriterlere uyan kodların listesi ve sonuc bilgisi döndürülür.</returns>
        public ABSKodlarVAN BirimListele(TNS.KYM.Kullanici kullanan, int yil, string kurum, bool altDuzey)
        {
            try
            {
                return KodServisi.BirimListele(null, kullanan, yil, kurum, altDuzey, false);
            }
            catch (Exception e)
            {
                ABSKodlarVAN donusDeger = new ABSKodlarVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ABSKodlarVAN BirimListele(TNS.KYM.Kullanici kullanan, int yil, string kurum, bool altDuzey, bool adaGoreSirala)
        {
            try
            {
                return KodServisi.BirimListele(null, kullanan, yil, kurum, altDuzey, false, adaGoreSirala);
            }
            catch (Exception e)
            {
                ABSKodlarVAN donusDeger = new ABSKodlarVAN();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Verilen listedeki (TNSCollection) kodları veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Kodların kaydedileceği yıl.</param>
        /// <param name="kodTuru">Kodların ait olduğu kod türü. Bu bilgi kullanılarak veri tabanı bilgileri bulunur.</param>
        /// <param name="kodlar">Kaydedilecek kod değerlerini tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public TNS.Sonuc KodKaydet(TNS.KYM.Kullanici kullanan, int yil, string kodTuru, TNS.TNSCollection kodlar)
        {
            try
            {
                return KodServisi.KodKaydet(null, kullanan, yil, kodTuru, kodlar);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Verilen kod türü bir önceki yıldaki değerleri bir sonraki yıla kopyalar.
        /// İşlem sonucu Sonuc yapısında döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Kodların kaydedileceği yıl.</param>
        /// <param name="kodTuru">Kodların ait olduğu kod türü. Bu bilgi kullanılarak veri tabanı bilgileri bulunur.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc OncekiYildanKopyala(Kullanici kullanan, int yil, string kodTuru)
        {
            return KodServisi.OncekiYildanKopyala(null, kullanan, yil, kodTuru);
        }

        /// <summary>
        /// Verilen kod numarasının veya kod adının, verilen yıldaki yapısı ve veri tabanı erişim bilgilerini döndüren yordam.
        /// </summary>
        /// <param name="yil">Verilen kodun bu yıla ait bilgileri döndürülür.</param>
        /// <param name="kodNo">Döndürülecek kodun numarası. (Degisken.xml'deki kod no yada kd adı verilmelidir.)</param>
        /// <param name="kodAd">Döndürülecek kodun adı. (Degisken.xml'deki kod no yada kd adı verilmelidir.)</param>
        /// <returns>Verilen kod numarasının veya kod adının, verilen yıldaki yapısı ve veri tabanı erişim bilgileri ButceKodYapi nesnesi olarak döndürülür.</returns>
        public ButceKodYapi ButceKodBilgiBul(int yil, int kodNo, string kodAd)
        {
            return ButceKodlariListesi.ButceKodlariAl(yil, kodNo, kodAd);
        }

        /// <summary>
        /// Verilen para birimi tanım bilgisini veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="paraBirimi">Kaydedilecek para birimi tanım bilgilerini tutan nesne.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ParaBirimiKaydet(Kullanici kullanan, ParaBirimi paraBirimi)
        {
            try
            {
                return BKDParaBirimi.Kaydet(kullanan, paraBirimi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Verilen kritere uyan, para birimlerinin listesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="kriterParaBirimi">Döndürülecek para birimlerinin uyacağı kriter bilgilerini tutan yapı.</param>
        /// <returns>Verilen kritere uyan, para birimlerinin dizisi döndürülür.</returns>
        public ParaBirimi[] ParaBirimiListele(Kullanici kullanan, ParaBirimi kriterParaBirimi)
        {
            return BKDParaBirimi.Listele(kullanan, kriterParaBirimi);
        }

        /// <summary>
        /// Verilen kritere uyan, para birimleri veri tabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="kriterParaBirimi">Silinecek para birimlerinin uyacağı kriter bilgilerini tutan yapı.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ParaBirimiSil(Kullanici kullanan, ParaBirimi kriterParaBirimi)
        {
            try
            {
                return BKDParaBirimi.Sil(kullanan, kriterParaBirimi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public System.Collections.ArrayList TertipPerformans()
        {
            DateTime t = DateTime.Now;
            OrtakFonksiyonlar.HataStrYaz("Metoda girdi:" + t.ToLongTimeString() + "." + t.Millisecond.ToString("000"));
            System.Collections.ArrayList tc = new System.Collections.ArrayList();
            for (int i = 0; i < 100000; i++)
            {
                TertipKisa tertip = new TertipKisa(5);
                for (int j = 0; j < tertip.kodlar.Length; j++)
                {
                    tertip.kodlar[j] = StringUret(5);
                    //tertip.adlar[j] = StringUret(20);
                }
                tc.Add(tertip);
                //for (int j = 0; j < 5; j++)
                //{
                //    tc.Add(StringUret(5));
                //    tc.Add(StringUret(20));
                //}
            }
            t = DateTime.Now;
            OrtakFonksiyonlar.HataStrYaz("Döngüden çıktı:" + t.ToLongTimeString() + "." + t.Millisecond.ToString("000"));
            return tc;
        }

        private string StringUret(int uz)
        {
            string don = "";
            for (int i = 0; i < uz; i++)
            {
                don += "A";
            }
            return don;
        }

        public string KodAdBul(int yil, string kodTuru, string kod)
        {
            return TNS.BKD.IsKurallari.KodServisi.KodAdBul(null, yil, kodTuru, kod);
        }

        public string KodAdBul(Kullanici kullanan, int yil, string kodTuru, string kod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return TNS.BKD.IsKurallari.KodServisi.KodAdBul(null, yil, kodTuru, kod);
        }

        public ABSKodlarVAN KurumListesiKullanici(Kullanici kullanan, int yil)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return TNS.BKD.IsKurallari.KodServisi.KurumListesiKullanici(null, kullanan, yil);
        }

        public Sonuc PrgPebIliskiKaydet(Kullanici kullanan, PrgPebIliski bilgi)
        {
            try
            {
                return BHMv2APrgKodYapi.KaydetPEBIliski(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray PrgPebIliskiListele(Kullanici kullanan, PrgPebIliski bilgi)
        {
            try
            {
                return BHMv2APrgKodYapi.ListelePEBIliski(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        public Sonuc PrgPebIliskiSil(Kullanici kullanan, PrgPebIliski bilgi)
        {
            try
            {
                return BHMv2APrgKodYapi.SilPEBIliski(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc PrgPebIliskiKopyala(Kullanici kullanan, int yil)
        {
            try
            {
                return BHMv2APrgKodYapi.IliskiKopyala(null, kullanan, yil);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }



        public Sonuc PrgKodYapiKaydet(Kullanici kullanan, PrgKodYapi bilgi)
        {
            try
            {
                return BHMv2APrgKodYapi.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc PrgKodYapiAktar(Kullanici kullanan, PrgKodYapi bilgi)
        {
            try
            {
                return BHMv2APrgKodYapi.Aktar(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray PrgKodYapiListele(Kullanici kullanan, PrgKodYapi bilgi)
        {
            try
            {
                return BHMv2APrgKodYapi.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc PrgKodYapiSil(Kullanici kullanan, PrgKodYapi bilgi)
        {
            try
            {
                return BHMv2APrgKodYapi.Sil(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        //public DataTable ParaBirimiListele(Kullanici kullanan)
        //{
        //    DataTable donusDeger = new DataTable();
        //    try
        //    {
        //        donusDeger = new BKDParaBirimi().Listele(kullanan);
        //    }
        //    catch
        //    {
        //    }
        //    return donusDeger;
        //}
        //public ABSKodlarVAN BirimListele(TNS.KYM.Kullanici kullanan, int yil)
        //{
        //    try
        //    {
        //        return KodServisi.BirimListele(null, kullanan, yil);
        //    }
        //    catch (Exception e)
        //    {
        //        ABSKodlarVAN donusDeger = new ABSKodlarVAN();
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //        return donusDeger;
        //    }
        //}

        //public ABSKodlarVAN KurumListesiKullanici(TNS.KYM.Kullanici kullanici, int yil)
        //{
        //    return KurumListesiKullanici(kullanici, yil, false);
        //}

        //public ABSKodlarVAN KurumListesiKullanici(TNS.KYM.Kullanici kullanici, int yil, bool gelirlilerMi)
        //{
        //    try
        //    {
        //        return KodServisi.KurumListesiKullanici(null, kullanici, yil, gelirlilerMi);
        //    }
        //    catch (Exception e)
        //    {
        //        ABSKodlarVAN absk = new ABSKodlarVAN();
        //        absk.sonuc.islemSonuc = false;
        //        absk.sonuc.hataStr = e.ToString();
        //        return absk;
        //    }
        //}

        //public ABSKodlarVAN KurumListesi(int yil, ENUMButceTur butceTur)
        //{
        //    try
        //    {
        //        return KodServisi.KurumListesi(null, yil, butceTur);
        //    }
        //    catch (Exception e)
        //    {
        //        ABSKodlarVAN absk = new ABSKodlarVAN();
        //        absk.sonuc.islemSonuc = false;
        //        absk.sonuc.hataStr = e.ToString();
        //        return absk;
        //    }
        //}

        //public TNS.Sonuc SonKirilimMi(int yil, string kodTuru, string kod)
        //{
        //    Sonuc sonuc = new Sonuc(false, "", "");
        //    try
        //    {
        //        sonuc.islemSonuc = true; // BHMaButceKodlariFabrikasi.ButceKodlariAlKodYadaAd(yil, kodTuru).SonKirilimMi(kod);
        //    }
        //    catch (Exception e)
        //    {
        //        sonuc.hataStr = e.ToString();
        //    }
        //    return sonuc;
        //}

        //public TNS.Sonuc KurumMu(int yil, string kod)
        //{
        //    Sonuc sonuc = new Sonuc(false, "", "");
        //    try
        //    {
        //        sonuc.islemSonuc = TNS.BKD.Arac.KurumMu(kod);
        //    }
        //    catch (Exception e)
        //    {
        //        sonuc.hataStr = e.ToString();
        //    }
        //    return sonuc;
        //}

        //public string KurumsaldanKuruma(string kurumsalKod)
        //{
        //    return Tertip.KurumsaldanKuruma(kurumsalKod);
        //}

        //public ABSKodlarVAN OdenekAsamalariBul()
        //{
        //    return Asamalar.OdenekAsamalariBul(false);
        //}
    }
}

namespace TNS
{
    /// <summary>
    /// Kullanıcı modülünde kullanılan yetki tipi listelerini döndüren yordamları tutan sınıf.
    /// </summary>
    public class KYMListe : YetkiTipiListesi, YetkiTipiAgaci
    {
        private static ListeSatir[] BirimListesi(Kullanici kullanan, int yil, bool sadeceAltBirim)
        {
            string kurum = TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMKOD");
            ListeSatir[] donusDeger = null;
            try
            {
                ABSKodlarVAN lv = TNS.BKD.IsKurallari.KodServisi.BirimListele(null, kullanan, yil, kurum, sadeceAltBirim, true);
                if (lv.absKodlar.Count > 0)
                {
                    donusDeger = new ListeSatir[lv.absKodlar.Count];
                    int ilkLen = ((ABSKod)lv.absKodlar[0]).kod.Length;
                    int i = 0;
                    foreach (ABSKod k in lv.absKodlar)
                    {
                        donusDeger[i++] = new ListeSatir(k.kod, k.kod.PadLeft(2 * k.kod.Length - ilkLen) + " " + k.Ad());
                    }
                }
            }
            catch { }
            return donusDeger;
        }

        /// <summary>
        /// Bütçe hazırlık aşamaları listesini döndüren yordam.
        /// </summary>
        /// <returns>Bütçe hazırlık aşamaları listesi döndürülür.</returns>
        public static ListeSatir[] AsamaListesi()
        {
            ListeSatir[] lv = new ListeSatir[7];
            lv[0] = new ListeSatir("00", ButceKodResI.BKDASA000);
            lv[1] = new ListeSatir("01", ButceKodResI.BKDASA001);
            lv[2] = new ListeSatir("02", ButceKodResI.BKDASA002);
            lv[3] = new ListeSatir("03", ButceKodResI.BKDASA003);
            lv[4] = new ListeSatir("04", ButceKodResI.BKDASA004);
            lv[5] = new ListeSatir("05", ButceKodResI.BKDASA005);
            lv[6] = new ListeSatir("06", ButceKodResI.BKDASA006);

            return lv;
        }

        /// <summary>
        /// Kullanıcı sunucusu tarafından döndürülen yetki listelerinin giriş yordamı. Bu yordam 3 ve 7 kodlarını tanır. Kod:
        ///     3 ise Kurumsal Kod listesi
        ///     7 ise Aşama listesi
        /// döndürülür.
        /// </summary>
        /// <param name="kullanan">Listeyi isteyen kullanıcının bilgisi.</param>
        /// <param name="yil">Sonuc listedeki elemanların kıstas yılı.</param>
        /// <param name="ekParamCagri">Sonuc listedeki elemanlar için verilen ek kıstas bilgisi. Bu alanın biçimi ve anlamı yordamdan yordama değişir.</param>
        /// <param name="ekParamTanim">Bir yordam, birden fazla liste döndürebilir. Bu alan döndürülecek listenin bilgisini tutar. Yetki tipi tanımlanırken, statik olarak verilir.</param>
        /// <returns>Verilen kıstaslara uyan yetki listesi döndürülür.</returns>
        public ListeSatir[] YetkiTipiListeElemanlari(Kullanici kullanan, int yil, string ekParamCagri, string ekParamTanim)
        {
            //KOD1,KURUMSAL
            //KOD2,FONKSİYONEL
            //KOD3,FİNANS
            //KOD4,EKONOMİK
            //KOD5,GELİR
            //KOD6,FİNANSMAN
            //KOD7,SEKTÖR

            if (ekParamTanim.Equals("3"))
                return BirimListesi(kullanan, yil, false);
            else if (ekParamTanim.Equals("7"))
                return AsamaListesi();
            else if (ekParamTanim.Equals("SEK"))
                return KodListesi(yil, "7", "", false);
            else
                return BirimListesi(kullanan, yil, true);
        }

        public ListeSatir[] YetkiTipiAgacElemanlari(Kullanici kullanan, int yil, string ustKod, string ekParamCagri, string ekParamTanim)
        {
            if (ekParamTanim.Equals("3"))
                return KodListesi(yil, "1", ustKod, true);
            else if (ekParamTanim.Equals("4"))
                return KodListesi(yil, "2", ustKod, true);
            else if (ekParamTanim.Equals("5"))
                return KodListesi(yil, "3", ustKod, true);
            else if (ekParamTanim.Equals("6"))
                return KodListesi(yil, "4", ustKod, true);
            else
                return KodListesi(yil, ekParamTanim, ustKod, true);
        }

        public ListeSatir[] KodListesi(int yil, string kodTuru, string ustKod, bool agac)
        {
            string kodYapi = TNS.DEG.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD" + kodTuru + "/KODYAPI");
            ListeSatir[] donusDeger = null;
            try
            {
                if (!string.IsNullOrEmpty(ustKod))
                {
                    if (!ustKod.EndsWith(".")) ustKod += ".";
                }
                else ustKod = "";

                string[] parca = ustKod.Split('.');
                ABSKodlarVAN lv = TNS.BKD.IsKurallari.KodServisi.KodListeleSeviye(null, yil, kodTuru, ustKod, agac ? parca.Length : -1, agac, false, "", "", agac, false, "", true);
                if (lv.absKodlar.Count > 0)
                {
                    donusDeger = new ListeSatir[lv.absKodlar.Count];
                    int ilkLen = ((ABSKod)lv.absKodlar[0]).kod.Length;
                    int i = 0;
                    foreach (ABSKod k in lv.absKodlar)
                    {
                        donusDeger[i] = new ListeSatir(k.kod, k.kod + " " + k.Ad());
                        donusDeger[i].cocuguVar = k.kod.Length < kodYapi.Length;
                        i++;
                    }
                }
            }
            catch { }
            return donusDeger;
        }

        public string YetkiTipiKodAd(Kullanici kullanan, int yil, string kod, string ekParamCagri, string ekParamTanim)
        {
            if (ekParamTanim.Equals("3"))
                return KodListesiAd(yil, "1", kod);
            else if (ekParamTanim.Equals("4"))
                return KodListesiAd(yil, "2", kod);
            else if (ekParamTanim.Equals("5"))
                return KodListesiAd(yil, "3", kod);
            else if (ekParamTanim.Equals("6"))
                return KodListesiAd(yil, "4", kod);
            else
                return null;
        }

        private string KodListesiAd(int yil, string kodTuru, string kod)
        {
            return kod + " " + TNS.BKD.IsKurallari.KodServisi.KodAdBul(null, yil, kodTuru, kod);
        }
    }
}
