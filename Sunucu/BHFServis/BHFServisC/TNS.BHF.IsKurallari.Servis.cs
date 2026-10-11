using System;
using TNS.KYM;
using TNS.BHF;
using System.Runtime;
using System.Data.SqlClient;
using TNS.BHF.IsKurallari;
using TNS.UZY;
using TNS.BHMv2;

namespace TNS.BHF.IsKurallari
{
    /// <summary>
    /// Bütçe hazırlık formu sunucu yordamlarının fabrika sınıfı. Gerçekleştirilen yordamlar, ilgili sınıfdaki işlem yapan yordamını çağırırlar.
    /// </summary>
    [Serializable()]
    public class Servis : MarshalByRefObject, TNS.BHF.IBHFServis
    {
        /// <summary>
        /// Bütçe hazırlık sunucu erişimi ve uzay erişimi nesneleri oluşturulur.
        /// </summary>
        static Servis()
        {
        }

        /// <summary>
        /// Uzay sunucu erişimi nesnesi kullanılarak, uzay sorgusununun sonucunu bulup döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="uzayAd">Erişim yapılacak uzayın adı.</param>
        /// <param name="uzaySorgu">Uzay sorgusu dizgisi.</param>
        /// <param name="esitle">Sorgu alanları eşit sorgusu ile mi, like sorgusu ile mi oluşturulacak bilgisi.</param>
        /// <returns>Uzay sorgusununun sonucu dizgi olarak döndürülür.</returns>
        public static string UzayDegeriStr(Kullanici kullanan, string uzayAd, string uzaySorgu, bool esitle)
        {
            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            return uzyServis.UzayDegeriStr(kullanan, uzayAd, uzaySorgu, esitle);
        }

        /// <summary>
        /// Veri tabanı bağlantısı adını bulup döndüren yordam.
        /// </summary>
        /// <returns>Veri tabanı bağlantısı döndürülür.</returns>
        public static string BaglantiSatiriBul()
        {
            return BaglantiSatiriBul(0);
        }

        /// <summary>
        /// Veri tabanı bağlantısı adını bulup döndüren yordam.
        /// </summary>
        /// <param name="yil">Çok yıllı olması için tasarlandı. Fakat bütçe hazırlık formları veri tabanı çok yıllı olmadığından kullanılmıyor.</param>
        /// <returns>Veri tabanı bağlantısı döndürülür.</returns>
        public static string BaglantiSatiriBul(int yil)
        {
            return "BaglantiSatiriBHF";
        }


        /// <summary>
        /// Parametre olarak verilen kullanıcının yönetici olup ollmadığını bulup döndüren yordam.
        /// </summary>
        /// <param name="kullanan">Yönetici olup olmadığı kontrol edilecek kullanıcı nesnesi.</param>
        /// <returns>Kullanıcı yönetici ise doğru, değilse yanlış döndürülür.</returns>
        public static bool Yoneticimi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;

            return false;
        }

        /// <summary>
        /// Kullanıcının, verilen kurumsal kodu görme yetkisinin olup olmadığını bulup döndüren yordam. Bütçe hazırlık modülüne sorularak cevap bulunur.
        /// </summary>
        /// <param name="kullanan">Kurumsal kodu görme yetkisinin  olup olmadığı kontrol edilecek kullanıcı nesnesi.</param>
        /// <param name="kurum">Kullanıcı tarafından görülüp görülmeyeceği sorgulanacak kurumsal kod.</param>
        /// <returns>Kullanıcının, verilen kurumsal kodu görme yetkisi varsa doğru, yoksa yanlış döndürülür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, string kurum)
        {
            //return bhmServis.GormeYetkisiVarMi(kullanan, 0, kurum).islemSonuc;
            return true;
        }

        /// <summary>
        /// Kullanıcının, verilen kurumsal koda yazma yetkisinin olup olmadığını bulup döndüren yordam. Bütçe hazırlık modülüne sorularak cevap bulunur.
        /// </summary>
        /// <param name="kullanan">Kurumsal koda yazma yetkisinin  olup olmadığı kontrol edilecek kullanıcı nesnesi.</param>
        /// <param name="kurum">Kullanıcı tarafından yazma yetkisi sorgulanacak kurumsal kod.</param>
        /// <returns>Kullanıcının, verilen kurumsal koda yazma yetkisi varsa doğru, yoksa yanlış döndürülür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, int yil, string kurum)
        {
            //TNS.BKD.Tertip t = new BKD.Tertip(yil, BKD.ENUMTabloKod.TABPEBOdenek, kurum);
            //return bhmServis.OGYYetkisiVarMi(kullanan, "00.00", t, ENUMOGY.Yazma);
            return true;
        }

        /// <summary>
        /// Bir birimin satın almayı planladığı taşıtları listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Taşıtların listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Taşıtların listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Verilen yılda, birimin satın almayı planladığı taşıtların listesi, listeyi ve sonucu içeren yapıda döndürülür.</returns>
        public EdinilecekTasitSatiri[] EdinilecekTasitlarListele(Kullanici kullanan, int yil, string kurumsalKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return EdinilecekTasitlarServis.EdinilecekTasitlarListele(null, kullanan, yil, kurumsalKod);
        }

        /// <summary>
        /// Bir birimin satın almayı planladığı taşıtları kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Taşıtların kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Taşıtların kaydedilen birimin kurumsal kodu.</param>
        /// <param name="edinilecekTasitlar">Satın alınması planlanan taşıtların EdinilecekTasitSatiri nesnesi içeren listesi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc EdinilecekTasitlarKaydet(Kullanici kullanan, int yil, string kurumsalKod, TNSCollection edinilecekTasitlar)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return EdinilecekTasitlarServis.EdinilecekTasitlarKaydet(null, kullanan, yil, kurumsalKod, edinilecekTasitlar);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Taşıt işlemleri tarafından bümko tarafından yayınlanan taşıt sözlük bilgilerini listeleyen yordam. Taşıtlar için bu sözlük bazında veri girilecektir.
        /// </summary>
        /// <param name="yil">Taşıt sözlük bilgilerinin listeleneceği yıl bilgisi.</param>
        /// <returns>Verilen yıldaki taşıt sözlük bilgileri döndürülür.</returns>
        public TasitSozluk[] TasitSozlukListele(int yil)
        {
            return EdinilecekTasitlarServis.TasitSozlukListele(null, yil);
        }

        /// <summary>
        /// Uluslararası kuruluş üyelik bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Uluslararası kuruluş üyelik bilgilerinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Uluslararası kuruluş üyelik bilgileri kaydedilen birimin kurumsal kodu.</param>
        /// <param name="uluslarArasiKuruluslar">Kaydedilecek üyelik bilgileri nesnelerini içeren dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc UluslarArasiKuruluslarKaydet(Kullanici kullanan, int yil, string kurumsalKod, TNSCollection uluslarArasiKuruluslar)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return UluslarArasiKuruluslarServis.UluslarArasiKuruluslarKaydet(null, kullanan, yil, kurumsalKod, uluslarArasiKuruluslar);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Uluslararası kuruluş üyelik bilgilerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Uluslararası kuruluş üyelik bilgilerinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Uluslararası kuruluş üyelik bilgileri listelenen birimin kurumsal kodu.</param>
        /// <returns>Verilen yılda birimin üye olacağı uluslar arası kuruluşların listesi döndürülür.</returns>
        public UluslarArasiKurulus[] UluslarArasiKuruluslarListele(Kullanici kullanan, int yil, string kurumsalKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return UluslarArasiKuruluslarServis.UluslarArasiKuruluslarListele(null, kullanan, yil, kurumsalKod);
        }

        /// <summary>
        /// Birime ait gerekçe metnini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait hizmet gerekçesinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait hizmet gerekçesinin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="gerekce">Kaydedilecek hizmet gerekçesi metni.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc FaaliyetGerekcesiKaydet(Kullanici kullanan, int yil, string kurumsalKod, int faaliyetKod, FaaliyetGerekce gerekce)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HizmetGerekcesiServis.FaaliyetGerekcesiKaydet(null, kullanan, yil, kurumsalKod, faaliyetKod, gerekce);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Birime ait hizmet gerekçesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait hizmet gerekçesinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait hizmet gerekçesinin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Faaliyet gerekçesi metni ve sonuç yapısı döndürülür.</returns>
        public FaaliyetGerekce FaaliyetGerekceOku(Kullanici kullanan, int yil, string kurumsalKod, int faaliyetKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return HizmetGerekcesiServis.FaaliyetGerekceOku(null, kullanan, yil, kurumsalKod, faaliyetKod);
        }

        /// <summary>
        /// Birime ait hizmet gerekçesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait hizmet gerekçesinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait hizmet gerekçesinin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Faaliyet gerekçesi metni ve sonuç yapısı döndürülür.</returns>
        public ObjectArray FaaliyetGerekceListele(Kullanici kullanan, int yil, string kurumsalKod, int faaliyetKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return HizmetGerekcesiServis.FaaliyetGerekceListele(null, kullanan, yil, kurumsalKod, faaliyetKod, false);
        }

        /// <summary>
        /// Birime ait hizmet gerekçesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait hizmet gerekçesinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait hizmet gerekçesinin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Faaliyet gerekçesi metni ve sonuç yapısı döndürülür.</returns>
        public Sonuc FaaliyetGerekceBirlestir(Kullanici kullanan, int yil, string kurumsalKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return HizmetGerekcesiServis.FaaliyetGerekceBirlestir(null, kullanan, yil, kurumsalKod);
        }
        /// <summary>
        /// Döner sermaye ve fon bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait döner sermaye veya fon bilgilerinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait döner sermaye veya fon bilgilerinin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="donerSermayeNo">Kaydedilecek döner sermaye veya fon bilgisinin sıra numarası. (0 ise fon, birden başlıyorsa döner sermayedir.)</param>
        /// <param name="donerSermayeAd">Kaydedilecek döner sermaye veya fon bilgisinin adı.</param>
        /// <param name="donerSermaye">Kaydedilecek döner sermaye veya fonun ödenek bilgisi satırları.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc DonerSermayeKaydet(Kullanici kullanan, int yil, string kurumsalKod, int donerSermayeNo, string donerSermayeAd, TNSCollection donerSermaye)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DonerSermayeServisi.Kaydet(null, kullanan, yil, kurumsalKod, donerSermayeNo, donerSermayeAd, donerSermaye);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Döner sermaye ve fon bilgilerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait döner sermaye veya fon bilgilerinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait döner sermaye veya fon bilgilerinin listeleneceği birimin kurumsal kodu.</param>
        /// <param name="donerSermayeNo">Listelenecek döner sermaye veya fon bilgisinin sıra numarası. (0 ise fon, birden başlıyorsa döner sermayedir.)</param>
        /// <returns>Birimin verilen sıradaki döner sermaye veya fona ait tanım ve ödenekleri döndürülür.</returns>
        public DonerSermaye DonerSermayeListele(Kullanici kullanan, int yil, string kurumsalKod, int donerSermayeNo)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return DonerSermayeServisi.Listele(null, kullanan, yil, kurumsalKod, donerSermayeNo);
            }
            catch (Exception e)
            {
                DonerSermaye donusDeger = new DonerSermaye();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Bir birimin sahip olduğu fiziksel değerleri kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait fiziksel değerlerin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait fiziksel değerlerin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="fizikselDegerler">Kaydedilecek fiziksel değer satırları.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc FizikselDegerlerKaydet(Kullanici kullanan, int yil, string kurumsalKod, TNSCollection fizikselDegerler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return FizikselDegerlerServisi.Kaydet(null, kullanan, yil, kurumsalKod, fizikselDegerler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Bir birimin sahip olduğu fiziksel değerlerin tutulduğu satırları listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait fiziksel değerlerin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait fiziksel değerlerin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>fiziksel değerlerin tutulduğu satırlar döndürülür.</returns>
        public ObjectArray FizikselDegerlerListele(Kullanici kullanan, int yil, string kurumsalKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return FizikselDegerlerServisi.Listele(null, kullanan, yil, kurumsalKod);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Yeni başlıyacak hizmetlerin ekonomik kod bazında maliyetleri ve bitirilecek hizmetlerin ekonomik kod bazında maliyetlerinin kaydedildiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait tamamlanacak yada başlayacak hizmetlerin maliyetlerinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait tamamlanacak yada başlayacak hizmetlerin maliyetlerinin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="tamamMiIlkMi">1 ise ilk defa yapılması planlanan hizmetler, 1 değil ise tamamlanması planlanan hizmetler</param>
        /// <param name="hizmetYili">3 yıllık süreçdeki hizmetlerle ilgili olduğundan hizmet yılı, yıl, yıl+1, yıl+2 değerleri almalıdır.</param>
        /// <param name="hizmetBilgisiSatirlari">Ekonomik kod bazında, hizmet maliyetlerinin tutulduğu dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc HizmetBilgisiKaydet(Kullanici kullanan, int yil, string kurumsalKod, int tamamMiIlkMi, int hizmetYili, TNSCollection hizmetBilgisiSatirlari)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HizmetBilgisiServisi.Kaydet(null, kullanan, yil, kurumsalKod, tamamMiIlkMi, hizmetYili, hizmetBilgisiSatirlari);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Yeni başlıyacak hizmetlerin ekonomik kod bazında maliyetleri ve bitirilecek hizmetlerin ekonomik kod bazında maliyetlerinin listelendiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait tamamlanacak yada başlayacak hizmetlerin maliyetlerinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait tamamlanacak yada başlayacak hizmetlerin maliyetlerinin listeleneceği birimin kurumsal kodu.</param>
        /// <param name="tamamMiIlkMi">1 ise ilk defa yapılması planlanan hizmetler, 1 değil ise tamamlanması planlanan hizmetler</param>
        /// <param name="hizmetYili">3 yıllık süreçdeki hizmetlerle ilgili olduğundan hizmet yılı, yıl, yıl+1, yıl+2 değerleri almalıdır.</param>
        /// <returns>Verilen kriterlere uyan hizmetlerin maliyetlerinin tutulduğu satırlar döndürülür.</returns>
        public ObjectArray HizmetBilgisiListele(Kullanici kullanan, int yil, string kurumsalKod, int tamamMiIlkMi, int hizmetYili)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HizmetBilgisiServisi.Listele(null, kullanan, yil, kurumsalKod, tamamMiIlkMi, hizmetYili);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Birimlerin hizmetlerini gerçekleştirmek amacıyla yaptıkları işlemlerin detaylandırıldığı formun detaylarını satır satır alıp kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birim hizmetlerinin maliyetlerinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birim hizmetlerin maliyetlerinin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="hizmetMaliyeti">Hizmet maliyetinin tesbiti için kullanılan satırlar dizisi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc HizmetMaliyetiTesbitiKaydet(Kullanici kullanan, int yil, string kurumsalKod, TNSCollection hizmetMaliyeti)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HizmetMaliyetiServisi.Kaydet(null, kullanan, yil, kurumsalKod, hizmetMaliyeti);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Birimlerin hizmetlerini gerçekleştirmek amacıyla yaptıkları işlemlerin detaylandırıldığı formun detaylarını satır satır listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birim hizmetlerinin maliyetlerinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birim hizmetlerin maliyetlerinin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Hizmet maliyeti tesbitinde kullanılan satırlar döndürülür.</returns>
        public ObjectArray HizmetMaliyetiTesbitiListele(Kullanici kullanan, int yil, string kurumsalKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HizmetMaliyetiServisi.Listele(null, kullanan, yil, kurumsalKod);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Tavan üstüne sığmayan ödeneklerin, açıklamalarıyla birlikte kaydedildiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Tavan üstüne sığmayan ödeneklerin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Tavan üstüne sığmayan ödeneklerin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="TavanUstuOdenekSatirlari">Kaydedilecek, açıklama ve ekonomik kodları tutan satır dizisi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc TavanUstuKaydet(Kullanici kullanan, int yil, string kurumsalKod, TNSCollection TavanUstuOdenekSatirlari)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TavanUstuOdenekServisi.Kaydet(null, kullanan, yil, kurumsalKod, TavanUstuOdenekSatirlari);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Tavan üstüne sığmayan ödeneklerin, detay bazda girilenlerinden hareketle üreten yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Tavan üstüne sığmayan ödeneklerin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Tavan üstüne sığmayan ödeneklerin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Tavan üstüne sığmayan kullanılan ödeneklerin bilgileri satırlar halinde döndürülür.</returns>
        public ObjectArray TavanUstuListeleDetaydan(Kullanici kullanan, int yil, string kurumsalKod, string senaryoKod, bool YirmiBes2)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TavanUstuOdenekServisi.ListeleDetaydan(null, kullanan, yil, kurumsalKod, senaryoKod, YirmiBes2);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Tavan üstüne sığmayan ödeneklerin, açıklamalarıyla birlikte listelendiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Tavan üstüne sığmayan ödeneklerin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Tavan üstüne sığmayan ödeneklerin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Tavan üstüne sığmayan kullanılan ödeneklerin bilgileri satırlar halinde döndürülür.</returns>
        public ObjectArray TavanUstuListele(Kullanici kullanan, int yil, string kurumsalKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TavanUstuOdenekServisi.Listele(null, kullanan, yil, kurumsalKod);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin detaylandırıldığı bilgileri kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="TavanUstuOdenekSatirlari">Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin miktarlarını ve açıklamalarını tutan satırların dizisi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc TavanUstuKaydetDetay(Kullanici kullanan, int yil, string kurumsalKod, TNSCollection TavanUstuOdenekSatirlari)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TavanUstuOdenekDetayServisi.Kaydet(null, kullanan, yil, kurumsalKod, TavanUstuOdenekSatirlari);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Tavan üstüne sığmayan ödeneklerin 03.2 ve 03.5 ekonomik kodlu olanlarının detaylandırıldığı bilgileri listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin bilgileri satırlar halinde döndürülür.</returns>
        public ObjectArray TavanUstuListeleDetay(Kullanici kullanan, int yil, string kurumsalKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TavanUstuOdenekDetayServisi.Listele(null, kullanan, yil, kurumsalKod);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Özel güvenlik ihaleleriyle ilgili bilgilerin kaydedildiği yordam. İhale bilgisi OzelGuvenlikBilgi nesnesi olarak verilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Özel güvenlik ihaleleriyle ilgili bilgilerin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Özel güvenlik ihaleleriyle ilgili bilgilerin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="ihaleNo">Özel güvenlik ihalelesinin birim içi sıra numarası.</param>
        /// <param name="ihale">Birimin verilen sıradaki ihalesinin bilgilerini tutan yapı.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc OzelGuvenlikKaydet(Kullanici kullanan, int yil, string kurumsalKod, int ihaleNo, OzelGuvenlikBilgi ihale)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return OzelGuvenlikServisi.Kaydet(null, kullanan, yil, kurumsalKod, ihaleNo, ihale);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Özel güvenlik ihaleleriyle ilgili bilgilerin OzelGuvenlikBilgi nesnesi olarak döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Özel güvenlik ihalelesinin bilgisinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Özel güvenlik ihalelesinin bilgisinin listeleneceği birimin kurumsal kodu.</param>
        /// <param name="ihaleNo">Özel güvenlik ihalelesinin birim içi sıra numarası. 0 verilirse tüm ihaleler listelenir.</param>
        /// <returns>Birimin verilen sıradaki ihalesinin bilgileri döndürülür.</returns>
        public OzelGuvenlikBilgi[] OzelGuvenlikMaliyetOku(Kullanici kullanan, int yil, string kurumsalKod, int ihaleNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return OzelGuvenlikServisi.OzelGuvenlikOku(null, kullanan, yil, kurumsalKod, ihaleNo);
        }

        /// <summary>
        /// Özel güvenlik ihaleleriyle ilgili bilgilerin sadece sıra numarası ve ihale adı döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Özel güvenlik ihalelesinin kimlik bilgisinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Özel güvenlik ihalelesinin kimlik bilgisinin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Özel güvenlik ihaleleriyle ilgili bilgilerin sadece sıra numarası ve ihale adı OzelGuvenlikBilgi yapısında döndürülür.</returns>
        public ObjectArray OzelGuvenlikListele(Kullanici kullanan, int yil, string kurumsalKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return OzelGuvenlikServisi.Listele(null, kullanan, yil, kurumsalKod);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Mevcut taşıtlar, tasfiye edilecek taşıtlar, hizmet alımı ile kullanılacak taşıt sayısı ve birim maliyetinin gibi taşıtlarla ilgili genel bilgilerin kaydedildiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Taşıt genel bilgilerinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Taşıt genel bilgilerinin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="tasitGenelBilgi">Taşıt genel bilgilerinin tutulduğu dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KullanilanTasitlarKaydet(Kullanici kullanan, int yil, string kurumsalKod, int merkezTasra, TNSCollection tasitGenelBilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return EdinilecekTasitlarServis.KullanilanTasitlarKaydet(null, kullanan, yil, kurumsalKod, merkezTasra, tasitGenelBilgi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Mevcut taşıtlar, tasfiye edilecek taşıtlar, hizmet alımı ile kullanılacak taşıt sayısı ve birim maliyetinin gibi taşıtlarla ilgili genel bilgilerin listelendiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Taşıt genel bilgilerinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Taşıt genel bilgilerinin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Taşıt genel bilgileri dizisi döndürülür.</returns>
        public ObjectArray KullanilanTasitlarListele(Kullanici kullanan, int yil, string kurumsalKod, int merkez1Tasra2Toplam3)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return EdinilecekTasitlarServis.KullanilanTasitlarListele(null, kullanan, yil, kurumsalKod, merkez1Tasra2Toplam3);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Hizmet alımı ile edinilecek taşıtların şoförlü/şoförsüz, yakıtlı/yakıtsız miktar ve maliyetlerinin kaydedildiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Hizmet alımı ile edinilecek taşıtların bilgilerinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Hizmet alımı ile edinilecek taşıtların bilgilerinin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="hatDetaylar">Hizmet alımı ile edinilecek taşıtların bilgilerinin tutulduğu dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc HizmetAlimiTasitlariDetayKaydet(Kullanici kullanan, int yil, string kurumsalKod, TNSCollection hatDetaylar, bool kullanilan)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return EdinilecekTasitlarServis.HizmetAlimiTasitlariDetayKaydet(null, kullanan, yil, kurumsalKod, hatDetaylar, kullanilan);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Hizmet alımı ile edinilecek taşıtların şoförlü/şoförsüz, yakıtlı/yakıtsız miktar ve maliyetlerinin listelendiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Hizmet alımı ile edinilecek taşıtların bilgilerinin listelendiği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Hizmet alımı ile edinilecek taşıtların bilgilerinin listelendiği birimin kurumsal kodu.</param>
        /// <returns>Hizmet alımı ile edinilecek taşıtların bilgilerinin tutulduğu dizi döndürülür.</returns>
        public ObjectArray HizmetAlimiTasitlariDetayListele(Kullanici kullanan, int yil, string kurumsalKod, bool kullanilan)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return EdinilecekTasitlarServis.HizmetAlimiTasitlariDetayListele(null, kullanan, yil, kurumsalKod, kullanilan);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Hizmet alımı ile edinilecek görevlilerin hizmet türleri itibariyle miktar ve maliyetlerinin kaydedildiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Hizmet alımı ile edinilecek görevlilerin bilgilerinin kaydedildiği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Hizmet alımı ile edinilecek görevlilerin bilgilerinin kaydedildiği birimin kurumsal kodu.</param>
        /// <param name="hizmetAlimiGorevliler">Hizmet alımı ile edinilecek görevlilerin bilgilerini tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc HizmetAlimiGorevliKaydet(Kullanici kullanan, int yil, string kurumsalKod, TNSCollection hizmetAlimiGorevliler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return EdinilecekTasitlarServis.HizmetAlimiGorevliKaydet(null, kullanan, yil, kurumsalKod, hizmetAlimiGorevliler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Hizmet alımı ile edinilecek görevlilerin hizmet türleri itibariyle miktar ve maliyetlerinin listelendiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Hizmet alımı ile edinilecek görevlilerin bilgilerinin listelendiği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Hizmet alımı ile edinilecek görevlilerin bilgilerinin listelendiği birimin kurumsal kodu.</param>
        /// <returns>Hizmet alımı ile edinilecek görevlilerin bilgilerinin tutulduğu dizi döndürülür.</returns>
        public ObjectArray HizmetAlimiGorevliListele(Kullanici kullanan, int yil, string kurumsalKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return EdinilecekTasitlarServis.HizmetAlimiGorevliListele(null, kullanan, yil, kurumsalKod);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Temizlik hizmeti ihaleleriyle ilgili bilgilerin kaydedildiği yordam. İhale bilgisi OzelGuvenlikBilgi nesnesi olarak verilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Temizlik hizmeti ihaleleriyle ilgili bilgilerin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Temizlik hizmeti ihaleleriyle ilgili bilgilerin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="ihaleNo">Temizlik hizmeti ihalelesinin birim içi sıra numarası.</param>
        /// <param name="ihale">Birimin verilen sıradaki ihalesinin bilgilerini tutan yapı.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc TemizlikKaydet(Kullanici kullanan, int yil, string kurumsalKod, int ihaleNo, TemizlikBilgi ihale)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TemizlikServisi.Kaydet(null, kullanan, yil, kurumsalKod, ihaleNo, ihale);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Temizlik hizmeti ihaleleriyle ilgili bilgilerin OzelGuvenlikBilgi nesnesi olarak döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Temizlik hizmeti ihalelesinin bilgisinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Temizlik hizmeti ihalelesinin bilgisinin listeleneceği birimin kurumsal kodu.</param>
        /// <param name="ihaleNo">Temizlik hizmeti ihalelesinin birim içi sıra numarası. 0 verilirse tüm ihaleler listelenir.</param>
        /// <returns>Birimin verilen sıradaki ihalesinin yada tüm ihalelerinin bilgileri döndürülür.</returns>
        public TemizlikBilgi[] TemizlikMaliyetOku(Kullanici kullanan, int yil, string kurumsalKod, int ihaleNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return TemizlikServisi.TemizlikOku(null, kullanan, yil, kurumsalKod, ihaleNo);
        }

        /// <summary>
        /// eBütçeden çekilen taşıt sözlük bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Sözlük bilgilerinin ait olduğu yıl.</param>
        /// <param name="tasitSozlukler">eBütçeden çekilen taşıt sözlük bilgilerini tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc TasitSozlukKaydet(Kullanici kullanan, int yil, TNSCollection tasitSozlukler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return EdinilecekTasitlarServis.TasitSozlukKaydet(null, kullanan, yil, tasitSozlukler);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// eBütçeden çekilen fiziksel değer sözlük bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="fizikselDegerlerSozlukler">eBütçeden çekilen fiziksel değer sözlük bilgilerini tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc FizikselDegerlerSozlukKaydet(Kullanici kullanan, TNSCollection fizikselDegerlerSozlukler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return FizikselDegerlerServisi.SozlukKaydet(null, kullanan, fizikselDegerlerSozlukler);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// eBütçeden çekilen hizmet maliyetinin tesbiti sözlük bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="hizmetMaliyetiSozlukler">eBütçeden çekilen hizmet maliyetinin tesbiti sözlük bilgilerini tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc HizmetMaliyetiSozlukKaydet(Kullanici kullanan, TNSCollection hizmetMaliyetiSozlukler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HizmetMaliyetiServisi.SozlukKaydet(null, kullanan, hizmetMaliyetiSozlukler);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        internal static void KurumsalKodBirimKontrol(string kurumsalKod, Sonuc sonuc)
        {
            if (kurumsalKod.Replace(".", "").Length != 8)
                sonuc.DegerAta(ButceFormResI.BHF009);
        }

        /// <summary>
        /// Hizmet maliyeti ile ilgili sözlük bilgilerini listeleyen yordam.
        /// </summary>
        /// <returns>Bulunan sözlük bilgileri dizi olarak döndürülür.</returns>
        public HizmetMaliyetiSatir[] HizmetMaliyetiSozlukListele()
        {
            return HizmetMaliyetiServisi.SozlukListele(null);
        }
        /// <summary>
        /// Döner sermaye ile ilgili sözlük bilgilerini listeleyen yordam.
        /// </summary>
        /// <returns>Bulunan sözlük bilgileri dizi olarak döndürülür.</returns>
        public DonerSermayeSatir[] DonerSermayeSozlukListele()
        {
            return DonerSermayeServisi.SozlukListele(null);
        }

        /// <summary>
        /// Fiziksel değerler ile ilgili sözlük bilgilerini listeleyen yordam.
        /// </summary>
        /// <returns>Bulunan sözlük bilgileri dizi olarak döndürülür.</returns>
        public FizikselDeger[] FizikselDegerlerSozlukListele()
        {
            return FizikselDegerlerServisi.SozlukListele(null);
        }

        /// <summary>
        /// Döner sermaye sözlük bilgilerini silen ve verilen dizideki sözlük bilgilerini ekleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="dssler">Kaydedilecek DonerSermayeSatir tipinde nesneleri tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc DonerSermayeSozlukKaydet(Kullanici kullanan, TNSCollection dssler)
        {
            try
            {
                return DonerSermayeServisi.SozlukKaydet(null, kullanan, dssler);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Hizmet gerekçesi girmiş olan birim sayısını bulup döndüren yordam.
        /// </summary>
        /// <param name="yil">Gerekçe sayısı bulunacak yıl.</param>
        /// <param name="kurumsalKod">Birime ait hizmet gerekçesi sayısının bulunacağı birimin kurumsal kodu.</param>
        /// <returns>Hizmet gerekçesi sayısı döndürülür.</returns>
        public int HizmetGerekcesiSayisiBul(int yil, string kurumsalKod)
        {
            return HizmetGerekcesiServis.HizmetGerekcesiSayisiBul(null, yil, kurumsalKod);
        }

        /// <summary>
        /// Hizmet gerekçelerini bir önceki yıldan, alıp bu yıla kopyalayan yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait hizmet gerekçesinin kopyalanacağı hedef yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait hizmet gerekçesinin kopyalanacağı birimin kurumsal kodu.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc HizmetGerekcesiKopyalaOncYil(Kullanici kullanan, int yil, string kurumsalKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HizmetGerekcesiServis.HizmetGerekcesiKopyalaOncYil(null, kullanan, yil, kurumsalKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray HizmetMaliyetiTasraListele(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, string kurumsalKod, bool kurumsalToplam)
        {
            try
            {
                return HizmetMaliyetiServisiTasra.Listele(null, kullanan, yil, muhasebeKod, birimKod, kurumsalKod, kurumsalToplam);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc HizmetMaliyetiTasraKaydet(Kullanici kullanan, int yil, int muhasebeKod, string birimKod, string kurumsalKod, TNSCollection hizmetMaliyeti)
        {
            try
            {
                return HizmetMaliyetiServisiTasra.Kaydet(null, kullanan, yil, muhasebeKod, birimKod, kurumsalKod, hizmetMaliyeti);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// TasitMarkaModel nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitMarkaModel yapısında TasitMarkaModel: İstatistik Kodu bilgilerini tutan değişken</param>
        public ObjectArray ListeleTasitMarkaModel(Kullanici kullanan, TasitMarkaModel form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitMarkaModel.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// TasitMarkaModel nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitMarkaModel yapısında TasitMarkaModel: İstatistik Kodu bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetTasitMarkaModel(Kullanici kullanan, TasitMarkaModel form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitMarkaModel.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray ListeleMarkaModel(Kullanici kullanan, TasitMarkaModel form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitMarkaModel.ListeleMarkaModel(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public ObjectArray ListeleTasitTip(Kullanici kullanan, TasitTip form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitTip.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public Sonuc KaydetTasitTip(Kullanici kullanan, TasitTip form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitTip.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray ListeleTasitCins(Kullanici kullanan, TasitCins form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitCins.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public Sonuc KaydetTasitCins(Kullanici kullanan, TasitCins form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitCins.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// TasitBilgi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitBilgi yapısında TasitBilgi:  bilgilerini tutan değişken</param>
        public ObjectArray ListeleTasitBilgi(Kullanici kullanan, TasitBilgi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitBilgi.Listele(null, kullanan, form, false, null, false);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// TasitBilgi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitBilgi yapısında TasitBilgi:  bilgilerini tutan değişken</param>
        public ObjectVAN OkuTasitBilgi(Kullanici kullanan, TasitBilgi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitBilgi.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// TasitBilgi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitBilgi yapısında TasitBilgi:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetTasitBilgi(Kullanici kullanan, TasitBilgi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitBilgi.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc OnaylaTasitBilgi(Kullanici kullanan, TNSCollection formlar, int onayKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitBilgi.Onayla(null, kullanan, formlar, onayKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc SilTasitBilgi(Kullanici kullanan, TasitBilgi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitBilgi.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectVAN TasitIslemTalepOkuBekleyen(Kullanici kullanan, string saseNo)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitBilgi.TasitIslemTalepOkuBekleyen(null, kullanan, saseNo);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        public ObjectArray TasitIslemTalepListele(Kullanici kullanan, string saseNo, bool bekleyen)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitBilgi.TasitIslemTalepListele(null, kullanan, saseNo, 0, bekleyen);
            }
            catch (Exception e)
            {
                ObjectArray ov = new ObjectArray();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        public Sonuc TasitIslemTalepKaydet(Kullanici kullanan, TasitIslemTalep form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitBilgi.TasitIslemTalepKaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc TasitIslemTalepOnayla(Kullanici kullanan, TNSCollection talepListesi, int onayKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitBilgi.TasitIslemTalepOnayla(null, kullanan, talepListesi, onayKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc TasitMarkaModelTipGuncelle(Kullanici kullanan, string tipAd, string marka, string modelBasla)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitMarkaModel.TipGuncelle(null, kullanan, tipAd, marka, modelBasla);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }


        public Sonuc TasitRuhsatResimKaydet(Kullanici kullanan, string saseNo, string resimAd, byte[] resim)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFTasitBilgi.TasitRuhsatResimKaydet(null, kullanan, saseNo, resimAd, resim);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// KiralikAracSozlesmesi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KiralikAracSozlesmesi yapısında KiralikAracSozlesmesi: Araç kiralamak için yapılan sözleşmelerin girildiği ekran bilgilerini tutan değişken</param>
        public ObjectArray ListeleKiralikAracSozlesme(Kullanici kullanan, KiralikAracSozlesmesi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFKiralikAraclar.ListeleSozlesme(null, kullanan, form, false);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }


        /// <summary>
        /// KiralikAracSozlesmesi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KiralikAracSozlesmesi yapısında KiralikAracSozlesmesi: Araç kiralamak için yapılan sözleşmelerin girildiği ekran bilgilerini tutan değişken</param>
        public ObjectVAN OkuKiralikAracSozlesme(Kullanici kullanan, KiralikAracSozlesmesi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFKiralikAraclar.OkuSozlesme(null, kullanan, form, true);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }


        /// <summary>
        /// KiralikAracSozlesmesi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KiralikAracSozlesmesi yapısında KiralikAracSozlesmesi: Araç kiralamak için yapılan sözleşmelerin girildiği ekran bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetKiralikAracSozlesme(Kullanici kullanan, KiralikAracSozlesmesi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFKiralikAraclar.KaydetSozlesme(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc KaydetKiralikArac(Kullanici kullanan, int yil, int sozlesmeKod, KiralikArac kiralikArac)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFKiralikAraclar.KaydetArac(null, kullanan, yil, sozlesmeKod, kiralikArac);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectVAN OkuKiralikArac(Kullanici kullanan, int yil, int sozlesmeKod, int aracSiraNo)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFKiralikAraclar.OkuArac(null, kullanan, yil, sozlesmeKod, aracSiraNo);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        public Sonuc SilKiralikArac(Kullanici kullanan, int yil, int sozlesmeKod, int aracSiraNo)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFKiralikAraclar.SilArac(null, kullanan, yil, sozlesmeKod, aracSiraNo);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc SilKiralikAracSozlesme(Kullanici kullanan, KiralikAracSozlesmesi form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFKiralikAraclar.SilSozlesme(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray KiralikAracRaporla(Kullanici kullanan, int yil, int saymanlikNo, string birimKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFKiralikAraclar.KiralikAracRaporla(null, kullanan, yil, saymanlikNo, birimKod);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// DepremHarcamaAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DepremHarcamaAna yapısında DepremHarcamaAna:  bilgilerini tutan değişken</param>
        public ObjectArray ListeleDepremHarcamaForm(Kullanici kullanan, DepremHarcamaAna form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFDepremHarcamaForm.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }


        /// <summary>
        /// DepremHarcamaAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DepremHarcamaAna yapısında DepremHarcamaAna:  bilgilerini tutan değişken</param>
        public ObjectVAN OkuDepremHarcamaForm(Kullanici kullanan, DepremHarcamaAna form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFDepremHarcamaForm.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }


        /// <summary>
        /// DepremHarcamaAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DepremHarcamaAna yapısında DepremHarcamaAna:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetDepremHarcamaForm(Kullanici kullanan, DepremHarcamaAna form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFDepremHarcamaForm.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc OnaylaDepremHarcamaForm(Kullanici kullanan, DepremHarcamaAna form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFDepremHarcamaForm.Onayla(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc OncekiAydanKopyalaDepremHarcamaForm(Kullanici kullanan, DepremHarcamaAna form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFDepremHarcamaForm.OncekiAydanKopyala(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        /// <summary>
        /// AracKiralamaPlanAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AracKiralamaPlanAna yapısında AracKiralamaPlanAna:  bilgilerini tutan değişken</param>
        public ObjectArray ListeleAracKiralamaPlani(Kullanici kullanan, AracKiralamaPlanAna form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFAracKiralamaPlani.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }


        /// <summary>
        /// AracKiralamaPlanAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AracKiralamaPlanAna yapısında AracKiralamaPlanAna:  bilgilerini tutan değişken</param>
        public ObjectVAN OkuAracKiralamaPlani(Kullanici kullanan, AracKiralamaPlanAna form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFAracKiralamaPlani.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }


        /// <summary>
        /// AracKiralamaPlanAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AracKiralamaPlanAna yapısında AracKiralamaPlanAna:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetAracKiralamaPlani(Kullanici kullanan, AracKiralamaPlanAna form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFAracKiralamaPlani.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc OnaylaAracKiralamaPlani(Kullanici kullanan, AracKiralamaPlanAna form, bool kayitYoksaEkle)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHFAracKiralamaPlani.Onayla(null, kullanan, form, kayitYoksaEkle);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
    }
}
