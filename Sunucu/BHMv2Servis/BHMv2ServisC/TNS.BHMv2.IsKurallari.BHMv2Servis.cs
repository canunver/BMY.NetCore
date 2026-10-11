using System;
using TNS;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using TNS.BKD;
using TNS.HRC;
using System.Collections;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// Interface cağrılarını implement eden
    /// ilgili sınıftaki ilgili metodu çağıran fonksiyonları tutan sınıf
    /// </summary>
    [Serializable()]
    public class BHMv2Servis : MarshalByRefObject, IBHMv2Servis
    {
        public static bool tabloDegisimYap = true;

        static BHMv2Servis()
        {
            lock ("OncekiIslemiBiteneKadarBekle")
            {
                if (tabloDegisimYap)
                {
                    tabloDegisimYap = false;
                    TNS.BHMv2.IsKurallari.BHMv2TabloDegisiklikleri.Kaydet();
                }
            }
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ve hiyerarşideki bir alt kırılımı ve bütün alt kırılımları döndüren yordam.
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="kullanan">Yordamı çağıran kullanıcı</param>
        /// <param name="kurumsalYapi">Kurumsal yapı kriteri. Bu yapı içindeki değerlere göre kriter oluşturulur.</param>
        /// <param name="birAltSeviye">True ise Kurumsal yapı kriterinde verilen kodun bir alt seviyesindeki kodlar listelenir. False ise o kod ile başlayan tüm kodlar listelenir</param>
        /// <returns>Verilen kriterlere uyan kurumsal kodlar dizisi döndürülür</returns>
        public KurumsalYapi[] KurumsalYapiListele(Kullanici kullanan, KurumsalYapi kurumsalYapi, bool birAltSeviye, bool enAltSeviye)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2KurumsalYapi.Listele(null, kullanan, kurumsalYapi, false, enAltSeviye, birAltSeviye);
        }

        /// <summary>
        /// Verilen saymanlık, birim ve kurumsal kod kriterlerine uygun olan en alt kırılımdaki harcama birimlerini döndüren yordam.
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="saymanlik">Harcama birimi verilen saymanlık olan harcama birimleri listelenir.</param>
        /// <param name="birim">Harcama birimi verilen birim kodu olan harcama birimleri listelenir.</param>
        /// <param name="kurKod">Harcama birimi verilen kurumsal kod harcama birimleri listelenir.</param>
        /// <returns>Verilen kriterlere uyan kurumsal kodlar dizisi döndürülür.</returns>
        public string[] KurumsalYapiListele(string saymanlik, string birim, string kurKod)
        {
            return BHMv2KurumsalYapi.Listele(null, saymanlik, birim, kurKod);
        }

        /// <summary>
        /// Verilen kurumsal yapı koduna ait bilgiler döndürülür.
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="aktifKultur">İstemci tarafının aktif kültürü. Oluşan mesajlar bu dile göre döndürülecektir.</param>
        /// <param name="kod">Kurumsal yapı bilgisi döndürülecek kod</param>
        /// <returns>Verilen koda ait kurumsal yapı bilgisi</returns>
        public KurumsalYapi KurumsalYapiDondur(string aktifKultur, string kod)
        {
            TNS.KYM.Arac.KulturDegistir(aktifKultur);
            return BHMv2KurumsalYapi.KurumsalYapiDondur(null, kod);
        }

        public Sonuc KurumsalYapiKaydet(Kullanici kullanan, KurumsalYapi ky)
        {
            try
            {
                return BHMv2KurumsalYapi.Kaydet(null, kullanan, ky);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc KurumsalYapiSil(Kullanici kullanan, KurumsalYapi ky)
        {
            try
            {
                return BHMv2KurumsalYapi.Sil(null, kullanan, ky);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Kullanıcının yetkisi kontrol edildikten sonra, 
        /// var olan senaryo tanım bilgisi değiştirilir veya senaryo kodu boş ise yeni senaryo tanımı oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceSenaryo">Bütçe senaryo tanım bilgisi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceSenaryoKaydet(Kullanici kullanan, ButceSenaryo butceSenaryo)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ButceSenaryo.Kaydet(null, kullanan, butceSenaryo);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        /// <summary>
        /// Kullanıcının yetkisi kontrol edildikten sonra var olan senaryo tanım bilgisi silinir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceSenaryo">Bütçe senaryo tanım bilgisi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceSenaryoSil(Kullanici kullanan, ButceSenaryo butceSenaryo)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ButceSenaryo.Sil(null, kullanan, butceSenaryo);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        /// <summary>
        /// Verilen senaryo tanım kıstasına uyan senaryo tanımlarını döndüren yordam.
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceSenaryo">Kıstas olarak kullanılacak bütçe senaryo tanım bilgisi</param>
        /// <returns>Verilen bütçe senaryo tanım kıstasına uyan senaryolar döndürülür.</returns>
        public ButceSenaryo[] ButceSenaryoListele(Kullanici kullanan, ButceSenaryo butceSenaryo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2ButceSenaryo.Listele(null, kullanan, butceSenaryo);
        }

        /// <summary>
        /// Senaryo adını verilen senaryo koduna göre döndüren yordam
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="aktifKultur">İstemci tarafının aktif kültürü. Oluşan mesajlar bu dile göre döndürülecektir.</param>
        /// <param name="senaryoKod">Adı döndürülecek senaryo kodu</param>
        /// <returns>Verilen senaryo koduna göre senaryo adı döndürülür.</returns>
        public string ButceSenaryoAdBul(string aktifKultur, string senaryoKod)
        {
            TNS.KYM.Arac.KulturDegistir(aktifKultur);
            return TNS.BHMv2.IsKurallari.BHMv2ButceSenaryo.AdBul(null, senaryoKod);
        }

        /// <summary>
        /// Bütçe tasnif tanım bilgisini verilen sıra numarasına göre döndüren yordam
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasnifSiraNo">Tasnif tanım bilgisi döndürülecek tasnif sıra numarası</param>
        /// <returns>Verilen sıra numarasına ait tasnif tanımı döndürülür.</returns>
        public ButceTasnif ButceTasnifOku(Kullanici kullanan, int tasnifSiraNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return ButceTasnifIslemler.Oku(null, kullanan, tasnifSiraNo);
        }

        public Sonuc ButceTasnifSenaryolarAta(Kullanici kullanan, ButceTasnif form, ButceSenaryo[] bsler)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return ButceTasnifIslemler.SenaryolarAta(null, kullanan, form, bsler);
        }

        public ButceTasnif ButceTasnifSenaryoEkle(Kullanici kullanan, ButceTasnif form, ButceSenaryo bs)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return ButceTasnifIslemler.SenaryoEkle(null, kullanan, form, bs);
        }

        public ButceTasnif ButceTasnifSenaryoSil(Kullanici kullanan, ButceTasnif form, ButceSenaryo bs)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return ButceTasnifIslemler.SenaryoSil(null, kullanan, form, bs);
        }

        /// <summary>
        /// Numarası verilen tasnif, ve tasnife ait tüm bilgiler silinir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="tasnifSiraNo">Tasnif bilgisi silinecek tasnifin sıra numarası.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceTasnifHiyerarsiSil(Kullanici kullanan, int tasnifSiraNo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return ButceTasnifIslemler.Sil(null, kullanan, tasnifSiraNo);
        }


        /// <summary>
        /// Kullanıcının yetkisi kontrol edildikten sonra, 
        /// var olan tasnif tanım bilgisi değiştirilir veya tasnif kodu boş ise yeni tasnif tanımı oluşturulur.
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceTasnif">Bütçe tasnif tanım bilgisi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceTasnifKaydet(Kullanici kullanan, ButceTasnif butceTasnif)
        {
            try
            {
                return ButceTasnifIslemler.Kaydet(null, kullanan, butceTasnif);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }


        /// <summary>
        /// Bütçe tasnifi kimlik bilgilerini döndüren yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kurKod">Hangi birime ait tasniflerin listeleneceği bilgisi.</param>
        /// <param name="tur">Hangi türdeki tasnifler listelenecek.</param>
        /// <returns>Sistemde kayıtlı tüm tasnif kimlik bilgileri dizi içerisinde döndürülür.</returns>
        public ButceTasnif[] ButceTasnifListele(Kullanici kullanan, string kurKod, int tur)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return ButceTasnifIslemler.Listele(null, kullanan, kurKod, tur);
        }

        /// <summary>
        /// Bir bütçe tasnifine ait, tanımlı tüm satırları kaydeden yordam
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasnifSiraNo">Kaydedilecek hiyerarşinin ait olduğu tasnifin sıra numarası</param>
        /// <param name="hiyerarsi">Verilen tasnif numarasına ait hiyerarşik satırları içeren dizi</param>
        /// <param name="yil">Her bir satırdaki tertip şablonunun yıldan yıla değişebileceği tasarlanmıştır. Verilen satırlardaki tertip şablonlarının hangi yıla ait olduğu bilgisi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceTasnifHiyerarsiKaydet(Kullanici kullanan, int tasnifSiraNo, ButceTasnifHiyerarsikSatir[] hiyerarsi, int yil)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ButceTasnifHiyerarsi.Kaydet(null, kullanan, tasnifSiraNo, hiyerarsi, yil);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        /// <summary>
        /// Sıra numarası verilen tasnife ait, hiyerarşik satır bilgilerini ve her bir satırın verilen yıla ait tertip şablonlarını döndüren yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasnifSiraNo">Hiyerarşi satırları döndürülecek tasnifin sıra numarası</param>
        /// <param name="yil">Her bir satırdaki tertip şablonunun yıldan yıla değişebileceği tasarlanmıştır. Döndürülecek satırlardaki tertip şablonlarının hangi yıla ait olduğu bilgisi.</param>
        /// <param name="hiyerarsiKod">Sadece verilen hiyerarsik kodun bilgileri okunur.</param>
        /// <returns>Sıra numarası verilen tasnife ait hiyerarşik satır bilgileri dizi olarak döndürülür.</returns>
        public ButceTasnifHiyerarsikSatir[] ButceTasnifHiyerarsiListele(Kullanici kullanan, int tasnifSiraNo, int yil, string hiyerarsiKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2ButceTasnifHiyerarsi.Listele(null, kullanan, tasnifSiraNo, yil, hiyerarsiKod, null);
        }

        /// <summary>
        /// Verilen tasnif sıra numarası, yıl ve senaryo bilgisine göre, 
        /// kullanıcının girmiş olduğu tasnif değerlerini döndüren yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="form">Form parametresinin; tasnif sıra numarası, yıl ve senaryo alanları kullanılarak değerler bulunacaktır.</param>
        /// <param name="ekDegerParamlar">Okunan her bir satırla birlikte döndürülecek olan ek bilgilerin tanımlarını tutan dizi.</param>
        /// <param name="senaryoKod">Eğer dolu ise, tasnifle ilişkilendirilmiş senaryolar yerine verilen senaryo değerleri okunur.</param>
        /// <param name="kistasTertip">Okunan her bir satırdaki tertip şablonlarının uyması gereken kistas tertip.</param>
        /// <returns>Verilen tasnif sıra numarası, yıl ve senaryo bilgisine göre girilen değerler ve istenilen ek bilgiler form yapısı içindeki dizide döndürülür.</returns>
        public ButceTasnifOdenekleriFormu ButceTasnifOdenekOku(Kullanici kullanan, ButceTasnifOdenekleriFormu form, EkDegerParam[] ekDegerParamlar, string senaryoKod, string hiyerarsiKod, string kistasTertip)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return ButceTasnifOdenekGiris.Oku(null, kullanan, form, ekDegerParamlar, senaryoKod, hiyerarsiKod, kistasTertip);
        }

        /// <summary>
        /// Form içerisinde verilen tasnif ödenek değerlerini, tasnif sıra numarası, yıl ve senaryo bilgisini kullanarak kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasnifOdenekleriFormu">Form parametresinin; tasnif sıra numarası, yıl ve senaryo alanları ve verilen ödenekler kullanılarak kayıt yapılır.</param>
        /// <param name="onceSilme">Bu parametre doğru ise silme işlemi yapılmaz, sadece ekleme işlemi yapılır.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceTasnifOdenekKaydet(Kullanici kullanan, ButceTasnifOdenekleriFormu tasnifOdenekleriFormu, bool onceSilme)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ButceTasnifOdenekGiris.Kaydet(null, kullanan, tasnifOdenekleriFormu, onceSilme);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        /// <summary>
        /// Her senaryo bir tasnif ile ilişkilendirilmiş ve her tasnif satırına bir kaynak bilgisi tanımlanmıştır.
        /// Bu senaryoya girilen tasnif değerleri ile, bu senaryonun tertip ödenek değerlerini; her satır için girilen kaynak verisi ile oluşturan yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="hedefYil">Oluşturulacak detay tertip ödeneklerinin yılı</param>
        /// <param name="hedefSenaryoKod">Oluşturulacak detay tertip ödeneklerinin senaryo kodu. Senaryo kodu kullanılarak tasnif verilerine de ulaşılır.</param>
        /// <param name="tasnifNo">Oluşturma işleminde kullanılacak tertiplerin listesini bulmakta kullanılacak tasnifin numarası.</param>
        /// <param name="hiyerarsiKod">Oluşturma işleminde kullanılacak tertiplerin listesini bulmakta kullanılacak tasnifin satırını gösterem hiyerarşik satır kodu.</param>
        /// <param name="kaynakTur">Tertip listesi elde edildikten sonra, tertip için kullanılacak kaynak rakamın elde edileceği kaynak.</param>
        /// <param name="kaynakYil">Tertip listesi elde edildikten sonra, tertip için kullanılacak kaynak rakamın elde edileceği kaynağın yıl bilgisi.</param>
        /// <param name="kaynakSenaryoKodVeyaAy">Tertip listesi elde edildikten sonra, tertip için kullanılacak kaynak rakamın elde edileceği kaynağın senaryosu veya ayı.</param>
        /// <param name="sadeceKontrol">İşlemin yapılıp yapılmayacağı. True ise sadece kontrol yapılır işlem yapılmaz.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc TasnifdenGiderOdenekOlustur(Kullanici kullanan, int hedefYil, string hedefSenaryoKod, int tasnifNo, string hiyerarsiKod, ENUMUretimKaynakTur kaynakTur, int kaynakYil, string kaynakSenaryoKodVeyaAy, bool sadeceKontrol)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2ButceOdenek.TasnifdenGiderOdenekOlustur(kullanan, null, hedefYil, hedefSenaryoKod, tasnifNo, hiyerarsiKod, kaynakTur, kaynakYil, kaynakSenaryoKodVeyaAy, sadeceKontrol);
        }

        /// <summary>
        /// Kıstas olarak verilen AltFaaliyet nesnesindeki yıl ve kod alanı kullanılarak, alt faaliyet bilgileri okunur ve döndürülür.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="altFaaliyet">Kıstas olarak verilen AltFaaliyet nesnesi (yıl ve kod alanları kullanılır)</param>
        /// <returns>Kıstas olarak verilen AltFaaliyete ait bilgiler okunur ve döndürülür.</returns>
        public AltFaaliyet AltFaaliyetTanimOku(Kullanici kullanan, AltFaaliyet altFaaliyet)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2AltFaaliyet.Oku(null, kullanan, altFaaliyet);
        }

        /// <summary>
        /// Alt faaliyet tanım bilgisini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="altFaaliyet">Kaydedilecek bilgileri içeren AltFaaliyet tanım nesnesi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc AltFaaliyetTanimKaydet(Kullanici kullanan, AltFaaliyet altFaaliyet)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2AltFaaliyet.Kaydet(null, kullanan, altFaaliyet);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        /// <summary>
        /// Parametre olarak verilen dizideki tüm faaliyetleri silen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="altFaaliyetler">Silinecek faaliyetleri içeren dizi (yıl ve kod alanları dolu olmalıdır.)</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc AltFaaliyetTanimSil(Kullanici kullanan, AltFaaliyet[] altFaaliyetler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2AltFaaliyet.Sil(null, kullanan, altFaaliyetler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        /// <summary>
        /// Verilen alt faaliyet kıstasına (yıla ait kodla başlayan faaliyetler) uyan faaliyetlerin listesi döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="altFaaliyet">Listelenecek faaliyetlerin uyacağı kıstas bilgisi</param>
        /// <returns>Verilen kıstasa uyan faaliyetlerin dizisi döndürülür.</returns>
        public AltFaaliyet[] AltFaaliyetListele(Kullanici kullanan, AltFaaliyet altFaaliyet, string senaryoKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2AltFaaliyet.Listele(null, kullanan, altFaaliyet, senaryoKod);
        }


        public Sonuc AltFaaliyetIliskiKaydet(Kullanici kullanan, AltFaaliyet[] altFaaliyetler, int tur, string senaryoKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2AltFaaliyet.IliskiKaydet(null, kullanan, altFaaliyetler, tur, senaryoKod);
        }

        /// <summary>
        /// Alt faaliyet kıstası içinde tanımlı olan yıl ve DPT alt proje bilgileri kullanılarak,
        /// bu DPT alt projesi ile ilişkilendirilmiş olan alt faaliyetlerin listesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="altFaaliyet">Yıl ve DPT alt proje kodu bilgileri kullanılan alt faaliyet yapısı</param>
        /// <returns>Verilen yıl ve DPT alt proje kodu kıstasına uyan faaliyetlerin listesi</returns>
        public AltFaaliyet[] YatBagPebKodlariListele(Kullanici kullanan, AltFaaliyet altFaaliyet, string senaryoKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2AltFaaliyet.YatBagPebKodlariListele(null, kullanan, altFaaliyet, 0, senaryoKod);
        }

        /// <summary>
        /// Kıstas olarak verilen yıl ve DPT proje kodu bilgileri kullanılarak,
        /// bu DPT projesi ile ilişkilendirilmiş olan alt faaliyetlerin listesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="yil">Alt faaliyetlerin listeleneceği yıl bilgisi</param>
        /// <param name="projeKod">Alt faaliyetleri bulunacak DPT yatırım projesi kodu</param>
        /// <returns>Verilen yıl ve DPT proje kodu kıstasına uyan faaliyetlerin listesi</returns>
        public AltFaaliyet[] YatBagPebKodlariListele(Kullanici kullanan, int yil, int projeKod, string senaryoKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            AltFaaliyet altFaaliyet = new AltFaaliyet();
            altFaaliyet.yil = yil;
            return BHMv2AltFaaliyet.YatBagPebKodlariListele(null, kullanan, altFaaliyet, projeKod, senaryoKod);
        }

        /// <summary>
        /// Bütçe hazırlık işlemleri sırasında kullanılan gelir ve gider tertipleri en alt düzeyde sistemde tanımlanmış olmalıdır.
        /// Tanımlı tertiplerden verilen tertip kıstasına uygun olanlarının listesini dizi olarak döndüren yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <returns>Verilen tertip kıstasına uyan tertip bilgileri dizi olarak döndürülür.</returns>
        public Tertip[] TertipTanimlariListele(Kullanici kullanan, Tertip kistasTertip)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return TertipTanim.Listele(null, kullanan, kistasTertip, 2, null);
        }

        /// <summary>
        /// Bütçe hazırlık işlemleri sırasında kullanılan gelir ve gider tertipleri en alt düzeyde sistemde tanımlanmış olmalıdır.
        /// Tanımlı tertiplerden verilen tertip kıstasına uygun olanlarının listesini dizi olarak döndüren yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <returns>Verilen tertip kıstasına uyan tertip bilgileri dizi olarak döndürülür.</returns>
        public Tertip[] TertipTanimlariListeleFonkod(Kullanici kullanan, Tertip kistasTertip, bool fonkodUyumsuz)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return TertipTanim.Listele(null, kullanan, kistasTertip, 2, null, fonkodUyumsuz);
        }
        /// <summary>
        /// Bütçe hazırlık işlemleri sırasında kullanılan gelir ve gider tertipleri en alt düzeyde sistemde tanımlanmış olmalıdır.
        /// Tanımlı tertiplerden verilen tertip kıstasına uygun olanlarının listesini dizi olarak döndüren yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="tertipDurumlar">Listelecek tertiplerin uyacağı durumların listesi. Tertip bu durumlardan birinde ise döndürülür.</param>
        /// <returns>Verilen tertip kıstasına uyan tertip bilgileri dizi olarak döndürülür.</returns>
        public Tertip[] TertipTanimlariListele(Kullanici kullanan, Tertip kistasTertip, ENUMTertipDurum[] tertipDurumlar)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return TertipTanim.Listele(null, kullanan, kistasTertip, 2, tertipDurumlar);
        }

        /// <summary>
        /// Bütçe hazırlık işlemleri sırasında kullanılan gelir ve gider tertipleri en alt düzeyde sistemde tanımlanmış olmalıdır.
        /// Verilen tertip dizisindeki, tertip bilgileri sisteme kaydedilir.
        /// Parametre olarak verilen dizide, tertip adı boş olan tertipler, tertip havuzundan silinir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertipler">GiderTertipListe yapısında GiderTertipListe: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc TertipTanimlariKaydet(Kullanici kullanan, Tertip[] tertipler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TertipTanim.Kaydet(null, kullanan, tertipler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        public ABSKod[] BirimKodListele(int yil, string kodTur, string birimKurKod)
        {
            //TNS.KYM.Arac.KulturDegistir(kullanan);
            return TertipTanim.BirimKodListele(null, yil, kodTur, birimKurKod);
        }

        /// <summary>
        /// Kıstas olarak verilen tertipe uyan, tertipleri (gelir, gider ve finansman) 
        /// ve bu tertiple birlikte, tertip yılı ve senaryo kodu kullanılarak, yıl ve senaryoya ait çok yıllı ödeneklerle birlikte
        /// istenilen ek bilgileri TertipOdenek dizisi şeklinde döndüren yordam.
        /// TertipOdenek nesnesinde ödenek ise 3 + istenilen, tertip ise 1 + istenilen ek bilgi kadar değer oluşacaktır.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="senaryoKod">Çok yıllı ödenek değerleri döndürülecek senaryo kodu</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="ekDegerParamlar">Kullanıcının verdiği senaryoya ait ödeneklerle birlikte döndürülecek ek değerler dizisi</param>
        /// <param name="tahmin">1 ise tahmin bilgileri, değil ise ödenek bilgileri listelenir.</param>
        /// <returns>Verile kıstas tertibe uygun tertipler, bu tertiplerin verilen senaryoya ait bilgileri ile ek değerleri TertipOdenek dizisinde döndürülür.</returns>
        public TertipOdenek[] ButceOdenekListele(Kullanici kullanan, string senaryoKod, Tertip kistasTertip, EkDegerParam[] ekDegerParamlar, int tahmin)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2ButceOdenek.OdenekListele(null, kullanan, senaryoKod, kistasTertip, ekDegerParamlar, tahmin);
        }

        /// <summary>
        /// Kıstas olarak verilen tertipe uyan, tertipleri (gelir, gider ve finansman) 
        /// ve bu tertiple birlikte, tertip yılı ve senaryo kodu kullanılarak, yıl ve senaryoya ait çok yıllı ödeneklerle birlikte
        /// istenilen ek bilgileri TertipOdenek dizisi şeklinde döndüren yordam.
        /// TertipOdenek nesnesinde ödenek ise 3 + istenilen, tertip ise 1 + istenilen ek bilgi kadar değer oluşacaktır.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="senaryoKod">Çok yıllı ödenek değerleri döndürülecek senaryo kodu</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="ekDegerParamlar">Kullanıcının verdiği senaryoya ait ödeneklerle birlikte döndürülecek ek değerler dizisi</param>
        /// <param name="tahmin">1 ise tahmin bilgileri, değil ise ödenek bilgileri listelenir.</param>
        /// <returns>Verile kıstas tertibe uygun tertipler, bu tertiplerin verilen senaryoya ait bilgileri ile ek değerleri TertipOdenek dizisinde döndürülür.</returns>
        public TertipOdenek[] ButceOdenekListele(Kullanici kullanan, string senaryoKod, Tertip kistasTertip, EkDegerParam[] ekDegerParamlar, int tahmin, UcgenKodBilgileri kodBilgileriGelen)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2ButceOdenek.OdenekListele(null, kullanan, senaryoKod, kistasTertip, ekDegerParamlar, tahmin, kodBilgileriGelen);
        }

        public Sonuc ButceOdenekKopyala(Kullanici kullanan, Tertip hedefTertip, string hedefAsama, int kaynakYil, string kaynakAsama)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2ButceOdenek.ButceOdenekKopyala(null, kullanan, hedefTertip, hedefAsama, kaynakYil, kaynakAsama);
        }

        /// <summary>
        /// Kıstas olarak verilen tertipe uyan, tertipleri (gelir, gider ve finansman) 
        /// ve bu tertiple birlikte, tertip yılı ve senaryo kodu kullanılarak, yıl ve senaryoya ait çok yıllı ödeneklerle birlikte
        /// istenilen ek bilgileri TertipOdenek dizisi şeklinde döndüren yordam.
        /// TertipOdenek nesnesinde ödenek ise 3 + istenilen, tertip ise 1 + istenilen ek bilgi kadar değer oluşacaktır.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="senaryoKod">Çok yıllı ödenek değerleri döndürülecek senaryo kodu</param>
        /// <param name="kistasYapi">Listelecek tertiplerin uyacağı kıstasları birden fazla olarak ve dahil hariç tutan yapı</param>
        /// <param name="ekDegerParamlar">Kullanıcının verdiği senaryoya ait ödeneklerle birlikte döndürülecek ek değerler dizisi</param>
        /// <param name="tahmin">1 ise tahmin bilgileri, değil ise ödenek bilgileri listelenir.</param>
        /// <returns>Verile kıstas tertibe uygun tertipler, bu tertiplerin verilen senaryoya ait bilgileri ile ek değerleri TertipOdenek dizisinde döndürülür.</returns>
        public TertipOdenek[] ButceOdenekListele(Kullanici kullanan, string senaryoKod, KistasYapi kistasYapi, EkDegerParam[] ekDegerParamlar, int tahmin, int aciklamaKolon, bool eko2)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2ButceOdenek.OdenekListele(null, kullanan, senaryoKod, kistasYapi, ekDegerParamlar, tahmin, null, aciklamaKolon, eko2);
        }

        /// <summary>
        /// Verilen tertip listesindeki çok yıllı ödenekler veri tabanına kaydedilir. 
        /// Sıfır değeri olan ödenekler kaydedilmez. Ödenekler tertibin yılına ve verilen senaryoya kaydedilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="senaryoKod">Ödeneklerin kaydedileceği senaryo kodu</param>
        /// <param name="tertipOdenekListesi">Tertip ve çok yıllı ödenek değerlerinin bulunduğu dizi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceOdenekKaydet(Kullanici kullanan, string senaryoKod, TertipOdenek[] tertipOdenekListesi)
        {
            return ButceOdenekKaydet(kullanan, senaryoKod, tertipOdenekListesi, false);
        }

        public Sonuc ButceOdenekKaydet(Kullanici kullanan, string senaryoKod, TertipOdenek[] tertipOdenekListesi, bool aciklamali)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ButceOdenek.Kaydet(null, kullanan, senaryoKod, tertipOdenekListesi, aciklamali);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        public Sonuc ButceOdenekOnayla(Kullanici kullanan, string birim, int hedefYil, string hedefAsama, int onayDurum)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2ButceOdenek.ButceOdenekOnayla(null, kullanan, birim, hedefYil, hedefAsama, onayDurum);
        }

        /// <summary>
        /// Verilen tertip listesindeki yıl sonu ödenekleri veri tabanına kaydedilir. 
        /// Sıfır değeri olan ödenekler kaydedilmez. Ödenekler tertibin yılına ve verilen senaryoya kaydedilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="senaryoKod">Yıl sonu gerçekleşme tahmininin kaydedileceği senaryo kodu</param>
        /// <param name="tertipListesi">Tertip ve yıl sonu gerçekleşme tahmini değerlerinin bulunduğu dizi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceYSTahminKaydet(Kullanici kullanan, string senaryoKod, TertipOdenek[] tertipListesi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ButceYSTahmin.Kaydet(null, kullanan, senaryoKod, tertipListesi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        public ObjectVAN OdenekGondermeKontrolMiktarlariniBul(Kullanici kullanan, int yil, TertipKodlari kodlar)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2ButceOdenek.OdenekGondermeKontrolMiktarlariniBul(null, yil, kodlar);
        }

        /// <summary>
        /// Verilen tertip listesindeki yıl sonu ödenekleri veri tabanına kaydedilir. 
        /// Sıfır değeri olan ödenekler kaydedilmez. Ödenekler tertibin yılına ve verilen senaryoya kaydedilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="senaryoKod">Yıl sonu gerçekleşme tahmininin kaydedileceği senaryo kodu</param>
        /// <param name="kistas">Seçilecek tertiplerin kıstas değerlerini tutat değişken</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public ObjectArray ButceYSTahminListele(Kullanici kullanan, string senaryoKod, Tertip kistas)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ButceYSTahmin.Listele(null, kullanan, senaryoKod, kistas);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }
        /// <summary>
        /// Verilen kıstas tertibe uyan tertipler bulunur. Bu tertipler verilen kod bilgilerine göre sıralanır 
        /// ve verilen kırılım sayısı kadar olan kısmı alınır. Oluşan tertip bölümüne ait ek bilgiler hesaplanır ve döndürülür. kırılımlardan bölünürek sonuç dizisi oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="kodBilgileri">Tertip sonuç listesi üretilecekken, sonuç listesinde oluşacak boyutların kodları, sırası ve kırılımlarını tutan dizi.</param>
        /// <param name="ekBilgiler">Sonuc listesinde yer alan her bir tertiple birlikte, döndürülecek ödeneklerin hangileri olduğunu tutan değişken</param>
        /// <param name="sifirlarHaric">Parametre true ise döndürülecek listede sıfır değeri olan satırlar bulunmaz.</param>
        /// <returns>Verilen kıstasa uyan tertipler, verilen kod şeklinde ve ek bilgilerle OdenekBilgi dizisi şeklinde döndürülür.</returns>
        public OdenekBilgi[] ListeTertipOl(Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric)
        {
            return ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, sifirlarHaric, false, true, -1);
        }

        /// <summary>
        /// Verilen kıstas tertibe uyan tertipler bulunur. Bu tertipler verilen kod bilgilerine göre sıralanır 
        /// ve verilen kırılım sayısı kadar olan kısmı alınır. Oluşan tertip bölümüne ait ek bilgiler hesaplanır ve döndürülür. kırılımlardan bölünürek sonuç dizisi oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="kodBilgileri">Tertip sonuç listesi üretilecekken, sonuç listesinde oluşacak boyutların kodları, sırası ve kırılımlarını tutan dizi.</param>
        /// <param name="ekBilgiler">Sonuc listesinde yer alan her bir tertiple birlikte, döndürülecek ödeneklerin hangileri olduğunu tutan değişken</param>
        /// <param name="sifirlarHaric">Parametre true ise döndürülecek listede sıfır değeri olan satırlar bulunmaz.</param>
        /// <param name="tanimliTertipler">Kayıtlı tüm tertipler getirilir.</param>
        /// <returns>Verilen kıstasa uyan tertipler, verilen kod şeklinde ve ek bilgilerle OdenekBilgi dizisi şeklinde döndürülür.</returns>
        public OdenekBilgi[] ListeTertipOl(Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric, bool tanimliTertipler)
        {
            return ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, sifirlarHaric, tanimliTertipler, true, -1);
        }

        /// <summary>
        /// Verilen kıstas tertibe uyan tertipler bulunur. Bu tertipler verilen kod bilgilerine göre sıralanır 
        /// ve verilen kırılım sayısı kadar olan kısmı alınır. Oluşan tertip bölümüne ait ek bilgiler hesaplanır ve döndürülür. kırılımlardan bölünürek sonuç dizisi oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="kodBilgileri">Tertip sonuç listesi üretilecekken, sonuç listesinde oluşacak boyutların kodları, sırası ve kırılımlarını tutan dizi.</param>
        /// <param name="ekBilgiler">Sonuc listesinde yer alan her bir tertiple birlikte, döndürülecek ödeneklerin hangileri olduğunu tutan değişken</param>
        /// <param name="sifirlarHaric">Parametre true ise döndürülecek listede sıfır değeri olan satırlar bulunmaz.</param>
        /// <param name="tanimliTertipler">Kayıtlı tüm tertipler getirilir.</param>
        /// <returns>Verilen kıstasa uyan tertipler, verilen kod şeklinde ve ek bilgilerle OdenekBilgi dizisi şeklinde döndürülür.</returns>
        public OdenekBilgi[] ListeTertipOl(Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric, bool tanimliTertipler, bool adlariBul)
        {
            return ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, sifirlarHaric, tanimliTertipler, adlariBul, -1);
        }

        /// <summary>
        /// Verilen kıstas tertibe uyan tertipler bulunur. Bu tertipler verilen kod bilgilerine göre sıralanır 
        /// ve verilen kırılım sayısı kadar olan kısmı alınır. Oluşan tertip bölümüne ait ek bilgiler hesaplanır ve döndürülür. kırılımlardan bölünürek sonuç dizisi oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="kodBilgileri">Tertip sonuç listesi üretilecekken, sonuç listesinde oluşacak boyutların kodları, sırası ve kırılımlarını tutan dizi.</param>
        /// <param name="ekBilgiler">Sonuc listesinde yer alan her bir tertiple birlikte, döndürülecek ödeneklerin hangileri olduğunu tutan değişken</param>
        /// <param name="sifirlarHaric">Parametre true ise döndürülecek listede sıfır değeri olan satırlar bulunmaz.</param>
        /// <param name="tanimliTertipler">Kayıtlı tüm tertipler getirilir.</param>
        /// <returns>Verilen kıstasa uyan tertipler, verilen kod şeklinde ve ek bilgilerle OdenekBilgi dizisi şeklinde döndürülür.</returns>
        public OdenekBilgi[] ListeTertipOl(Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric, bool tanimliTertipler, bool adlariBul, int aciklamaKolon)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2.IsKurallari.UcgenServisi.ListeTertipOl(null, kullanan, kistasTertip, kodBilgileri, ekBilgiler, sifirlarHaric, tanimliTertipler, adlariBul, aciklamaKolon);
        }

        public OdenekBilgi[] ListeTertipOl(Kullanici kullanan, KistasYapi kistasYapi, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric, bool tanimliTertipler, bool adlariBul, int aciklamaKolon)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2.IsKurallari.UcgenServisi.ListeTertipOl(null, kullanan, kistasYapi, kodBilgileri, ekBilgiler, sifirlarHaric, tanimliTertipler, adlariBul, aciklamaKolon);
        }


        /// <summary>
        /// Verilen kıstas tertibe uyan tertipler bulunur. Bu tertipler verilen kod bilgilerine göre sıralanır 
        /// ve verilen kırılım sayısı kadar olan kısmı alınır. Daha sonra bu kısmın 1., 2. 3. ve n. kırılımları alınarak üçgen format elde edilir.
        /// Her bir üçgen satır için ek bilgiler hesapllanır ve sonuç dizisi oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="kodBilgileri">Tertip sonuç listesi üretilecekken, sonuç listesinde oluşacak boyutların kodları, sırası ve kırılımlarını tutan dizi.</param>
        /// <param name="ekBilgiler">Sonuc listesinde yer alan her bir tertiple birlikte, döndürülecek ödeneklerin hangileri olduğunu tutan değişken</param>
        /// <param name="sifirlarHaric">Değişken değeri true ise ek bilgilerinin hepsi sıfır olan tertipler sonuç kümesine alınmaz.</param>
        /// <returns>Verilen kıstasa uyan tertipler, verilen kod şeklinde üçgen olarak ve ek bilgilerle OdenekBilgi dizisi şeklinde döndürülür.</returns>
        public UcgenSatir[] UcgenTertipOl(Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2.IsKurallari.UcgenServisi.UcgenTertipOl(null, kullanan, kistasTertip, kodBilgileri, ekBilgiler, sifirlarHaric, -1);
        }

        /// <summary>
        /// Verilen kıstas tertibe uyan tertipler bulunur. Bu tertipler verilen kod bilgilerine göre sıralanır 
        /// ve verilen kırılım sayısı kadar olan kısmı alınır. Daha sonra bu kısmın 1., 2. 3. ve n. kırılımları alınarak üçgen format elde edilir.
        /// Her bir üçgen satır için ek bilgiler hesapllanır ve sonuç dizisi oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="kodBilgileri">Tertip sonuç listesi üretilecekken, sonuç listesinde oluşacak boyutların kodları, sırası ve kırılımlarını tutan dizi.</param>
        /// <param name="ekBilgiler">Sonuc listesinde yer alan her bir tertiple birlikte, döndürülecek ödeneklerin hangileri olduğunu tutan değişken</param>
        /// <param name="sifirlarHaric">Değişken değeri true ise ek bilgilerinin hepsi sıfır olan tertipler sonuç kümesine alınmaz.</param>
        /// <returns>Verilen kıstasa uyan tertipler, verilen kod şeklinde üçgen olarak ve ek bilgilerle OdenekBilgi dizisi şeklinde döndürülür.</returns>
        public UcgenSatir[] UcgenTertipOl(Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric, int aciklamaKolon)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2.IsKurallari.UcgenServisi.UcgenTertipOl(null, kullanan, kistasTertip, kodBilgileri, ekBilgiler, sifirlarHaric, aciklamaKolon);
        }

        /// <summary>
        /// Bütçe fişleri girilirken her bir tertip için girilen gerekçe yazısını veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Bütçe fişinin tertibi</param>
        /// <param name="yazi">Bütçe tertibi için istenen ödeneğin gerekçesi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceFisiGerekceKaydet(Kullanici kullanan, Tertip tertip, string[] yazi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ButceFisi.KaydetGerekce(kullanan, tertip, yazi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Verilen yıl ve tertip için istenilen ödenek için girilmiş olan gerekçe döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Bütçe fişinin tertibi</param>
        /// <returns>Bütçe tertibi için istenen ödeneğin gerekçesi döndürülür</returns>
        public string[] ButceFisiGerekceYaziGetir(Kullanici kullanan, Tertip tertip)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ButceFisi.ListeleGerekce(kullanan, tertip);
            }
            catch
            {
            }
            return null;
        }

        /// <summary>
        /// Bütçe tertibi için istenen ödeneğin gerekçesi girilirken, satın alınması planlanan malzemeler
        /// kullanıcı tarafından girilir. Bu malzemeleri veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Bütçe fişinin tertibi</param>
        /// <param name="tertipMalzemeleri">Bu tertip ile alınaca malzemelerin bilgilerinin tutulduğu dizi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceFisiMalzemeKaydet(Kullanici kullanan, Tertip tertip, ButceFisiMalzeme[] tertipMalzemeleri)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ButceFisi.KaydetMalzeme(kullanan, tertip, tertipMalzemeleri);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Parametre olarak verilen tertip için istenen ödenek gerekçelendirilirken, girilen malzeme bilgilerini veri tabanından okuyup döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Kendisi ile ilişkilendirilen malzemelerin döndürüleceği tertip</param>
        /// <returns>Parametre olarak verilen tertip için istenen ödenek gerekçelendirilirken girilen malzeme bilgileri döndürülür.</returns>
        public ButceFisiMalzeme[] ButceFisiMalzemeListele(Kullanici kullanan, Tertip tertip)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2ButceFisi.ListeleMalzeme(kullanan, tertip);
        }

        /// <summary>
        /// Bütçe fişleri girilirken her bir gelir tertibi için, gelir tahmini için girilen gerekçe yazısını veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Gelir fişinin tertibi</param>
        /// <param name="yazi">Gelir tertibi için istenen ödeneğin gerekçesi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceGelirFisiGerekceKaydet(Kullanici kullanan, Tertip tertip, string yazi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ButceGelirFisi.KaydetGerekce(kullanan, tertip, yazi);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Verilen yıl ve gelir tertibi için tahmin edilen gelir için girilmiş olan gerekçe döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Gelir fişinin tertibi</param>
        /// <returns>Gelir tertibi için tahmin edilen gelirin gerekçesi döndürülür</returns>
        public string ButceGelirFisiGerekceYaziGetir(Kullanici kullanan, Tertip tertip)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ButceGelirFisi.ListeleGerekce(kullanan, tertip);
            }
            catch
            {
                return string.Empty;
            }
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
        public OdenekBilgi[] HarcamaDagitimYuzdeListele(Kullanici kullanan, int butceYili, int yil, int ay, Tertip tertip)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return HarcamaDagitimAltFaaliyet.Listele(kullanan, butceYili, yil, ay, tertip);
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
        public Sonuc HarcamaDagitimKaydetTekTertip(Kullanici kullanan, int butceYili, int yil, int ay, OdenekBilgi[] oranlar)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HarcamaDagitimAltFaaliyet.KaydetTekTertip(null, butceYili, yil, ay, oranlar);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
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
        public Sonuc HarcamaDagitimKaydetTumTertipler(Kullanici kullanan, int butceYili, int yil, int ay, OdenekBilgi[] oranlar)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HarcamaDagitimAltFaaliyet.KaydetTumTertipler(null, butceYili, yil, ay, oranlar);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Muhasebat harcamalarından gelen tertiplerden, alt faaliyetler ilişkisi hiç kurulmamış 
        /// olanların listesi dizi olarak döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceYili">Her bütçe yılı için ayrı veri tabanları tutulduğundan, her bütçe yılı için ayrı parametreler kullanılabilmektedir.</param>
        /// <param name="yil">Tertip alt faaliyet eşleşmesi olmayan tertiplerin bulunacağı yıl</param>
        /// <param name="ay">Tertip alt faaliyet eşleşmesi olmayan tertiplerin bulunacağı ay</param>
        /// <returns>Tertip alt faaliyet eşleşmesi olmayan tertiplerin listesi</returns>
        public OdenekBilgi[] HarcamaDagitimPebListeleYuzdesiOlmayanlar(Kullanici kullanan, int butceYili, int yil, int ay)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return HarcamaDagitimAltFaaliyet.ListeleYuzdesiOlmayanlar(kullanan, butceYili, yil, ay, true);
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
        public Sonuc HarcamalaraAltFaalEkle(Kullanici kullanan, int butceYili, int yil, int ay)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return HarcamaDagitimAltFaaliyet.HarcamalaraAltFaalEkle(null, butceYili, yil, ay);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Verilen kıstas tertibe uyan ve harcama birimi bazında girilen ödenekleri listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Listelenecek ödenekler için verilen kıstas.
        /// Tertip tasra ödenek türünde olmalı ve son kırılımı birimi göstermelidir.</param>
        /// <param name="asama">Listelenecek verilerin aşaması. Harcama birimi bazında ödenekler çeşitli aşamalarda girilebilemekte ve listelenmektedir.</param>
        /// <param name="ekDegerParamlar">Tertipler listelenirken, her bir tertiple birlikte listelenecek ek bilgiler dizisi</param>
        /// <returns>Verilen kıstas tertiple uyumlu harcama birimi ödenekleri, istenilen ek bilgilerle birlikte döndürülür.</returns>
        public TertipOdenek[] BirimOdenekGiderListele(Kullanici kullanan, Tertip tertip, ENUMBHBirimAsama asama, EkDegerParam[] ekDegerParamlar)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BirimOdenekGider.Listele(null, kullanan, tertip, asama, ekDegerParamlar);
        }

        /// <summary>
        /// Verilen ödenekler dizisini, verilen asama için veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="asama">Listelenecek verilerin aşaması. Harcama birimi bazında ödenekler çeşitli aşamalarda girilebilemekte ve listelenmektedir.</param>
        /// <param name="tertipOdenekler">Veri tabanına kaydedilecek ödenek bilgilerini tutan dizi.
        /// Dizideki tertipler tasra ödenek türünde olmalı ve son kırılımı birimi göstermelidir.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc BirimOdenekKaydet(Kullanici kullanan, ENUMBHBirimAsama asama, TertipOdenek[] tertipOdenekler)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BirimOdenekGider.Kaydet(null, kullanan, asama, tertipOdenekler);
        }

        /// <summary>
        /// Harcama birimi ödenek talebi yaparken, ekonomik kod bazında gerekçe girilirken,
        /// ekonomik kod taşınır ile ilişkili ise, taşınır bilgileri girilir.
        /// Girilen taşınır bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasinirTalep">Kaydedilecek taşınır bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ParTalepTasinirKaydet(Kullanici kullanan, ParTalepTasinir tasinirTalep)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ParTalepTasinir.Kaydet(kullanan, tasinirTalep);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Bir harcama biriminin talep ettiği taşınırları verilen kriterlere göre bulup döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasinirTalep">Listelenecek taşınır bilgilerinin kıstasını içeren nesne</param>
        /// <returns>Verilen taşınır kıstasına uyan, taşınır bilgilerinin listesi ParTalepTasinir nesneleri içeren dizide döndürülür.</returns>
        public ObjectArray ParTalepTasinirListele(Kullanici kullanan, ParTalepTasinir tasinirTalep)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ParTalepTasinir.Listele(tasinirTalep);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Bir harcama biriminin talep ettiği taşınırlardan, verilen kriterlere uyanları, silen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasinirTalep">Silinecek taşınır taleplerinden silinecekleri gösteren kıstas nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ParTalepTasinirSil(Kullanici kullanan, ParTalepTasinir tasinirTalep)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ParTalepTasinir.Sil(kullanan, tasinirTalep);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Taşınır talepleri girilirken, kullanılacak ortalama fiyat bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Kaydedilecek ortalama fiyat bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ParTasinirOrtalamaFiyatKaydet(Kullanici kullanan, ParTasinirOrtalamaFiyat ortalamaFiyat)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ParTasinirOrtalamaFiyat.Kaydet(kullanan, ortalamaFiyat);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Taşınır talepleri girilirken, kullanılacak ortalama fiyat bilgilerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Listelenecek ortalama fiyat bilgilerinin kıstasını içeren nesne</param>
        /// <returns>Verilen ortalama fiyat kıstasına uyan ortalama fiyat bilgilerinin listesi ParTasinirOrtalamaFiyat nesneleri içeren dizide döndürülür.</returns>
        public ObjectArray ParTasinirOrtalamaFiyatListele(Kullanici kullanan, ParTasinirOrtalamaFiyat ortalamaFiyat)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ParTasinirOrtalamaFiyat.Listele(ortalamaFiyat);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Kaydedilen taşınır ortalama fiyatlarından verilen kritere uyanları silen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Silinecek ortalama fiyat bilgileri için kriter bilgilerini tutan nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ParTasinirOrtalamaFiyatSil(Kullanici kullanan, ParTasinirOrtalamaFiyat ortalamaFiyat)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ParTasinirOrtalamaFiyat.Sil(kullanan, ortalamaFiyat);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Harcama birimi ödenek talebi yaparken, ekonomik kod bazında gerekçe girilirken,
        /// ekonomik kod tüketim ile ilişkili ise, tüketim bilgileri girilir.
        /// Girilen tüketim bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tuketim">Kaydedilecek tüketim bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ParTuketimKaydet(Kullanici kullanan, ParTuketim tuketim)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ParTuketim.Kaydet(kullanan, tuketim);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Bir harcama biriminin talep ettiği tüketim bilgilerini verilen kriterlere göre bulup döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tuketim">Listelenecek tüketim bilgilerinin kıstasını içeren nesne</param>
        /// <returns>Verilen tüketim kıstasına uyan, tüketim bilgilerinin listesi ParTuketim nesneleri içeren dizide döndürülür.</returns>
        public ObjectArray ParTuketimListele(Kullanici kullanan, ParTuketim tuketim)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ParTuketim.Listele(tuketim);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Bir harcama biriminin girdiği tüketim bilgilerinden, verilen kriterlere uyanları, silen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tuketim">Silinecek tüketim bilgilerinden, silinecekleri gösteren kıstas nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ParTuketimSil(Kullanici kullanan, ParTuketim tuketim)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ParTuketim.Sil(kullanan, tuketim);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Tüketim bilgileri girilirken, kullanılacak ortalama fiyat bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Kaydedilecek ortalama fiyat bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ParTuketimOrtalamaFiyatKaydet(Kullanici kullanan, ParTuketimOrtalamaFiyat ortalamaFiyat)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ParTuketimOrtalamaFiyat.Kaydet(kullanan, ortalamaFiyat);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Tüketim bilgileri girilirken, kullanılacak ortalama fiyat bilgilerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Listelenecek ortalama fiyat bilgilerinin kıstasını içeren nesne</param>
        /// <returns>Verilen ortalama fiyat kıstasına uyan ortalama fiyat bilgilerinin listesi ParTuketimOrtalamaFiyat nesneleri içeren dizide döndürülür.</returns>
        public ObjectArray ParTuketimOrtalamaFiyatListele(Kullanici kullanan, ParTuketimOrtalamaFiyat ortalamaFiyat)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ParTuketimOrtalamaFiyat.Listele(ortalamaFiyat);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Kaydedilen tüketim bilgileri ortalama fiyatlarından verilen kritere uyanları silen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Silinecek ortalama fiyat bilgileri için kriter bilgilerini tutan nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ParTuketimOrtalamaFiyatSil(Kullanici kullanan, ParTuketimOrtalamaFiyat ortalamaFiyat)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ParTuketimOrtalamaFiyat.Sil(kullanan, ortalamaFiyat);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Harcama birimi ödenek talebi yaparken, ekonomik kod bazında gerekçe girilirken,
        /// ekonomik kod yolluk ile ilişkili ise, yolluk bilgileri girilir.
        /// Girilen yolluk bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="yolluklar">Kaydedilecek yolluk bilgilerini içeren dizi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ParYolluklarKaydet(Kullanici kullanan, ParYolluk[] yolluklar)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ParYolluklar.Kaydet(kullanan, yolluklar);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Bir harcama biriminin talep ettiği yolluk bilgilerini verilen kriterlere göre bulup döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="yolluk">Listelenecek yolluk bilgilerinin kıstasını içeren nesne</param>
        /// <returns>Verilen yolluk kıstasına uyan, tüketim bilgilerinin listesi ParYolluk nesneleri içeren dizide döndürülür.</returns>
        public ObjectArray ParYolluklarListele(Kullanici kullanan, ParYolluk yolluk)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ParYolluklar.Listele(kullanan, yolluk);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Yolluk bilgileri girilirken, kullanılacak ortalama fiyat bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Kaydedilecek ortalama fiyat bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ParYolluklarOrtalamaFiyatKaydet(Kullanici kullanan, ParYollukOrtalamaFiyat ortalamaFiyat)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ParYolluklarOrtalamaFiyat.Kaydet(kullanan, ortalamaFiyat);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Yolluk bilgileri girilirken, kullanılacak ortalama fiyat bilgilerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Listelenecek ortalama fiyat bilgilerinin kıstasını içeren nesne</param>
        /// <returns>Verilen ortalama fiyat kıstasına uyan ortalama fiyat bilgilerinin listesi ParYollukOrtalamaFiyat nesneleri içeren dizide döndürülür.</returns>
        public ObjectArray ParYolluklarOrtalamaFiyatListele(Kullanici kullanan, ParYollukOrtalamaFiyat ortalamaFiyat)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ParYolluklarOrtalamaFiyat.Listele(kullanan, ortalamaFiyat);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Kaydedilen yolluk bilgileri ortalama fiyatlarından verilen kritere uyanları silen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Silinecek ortalama fiyat bilgileri için kriter bilgilerini tutan nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ParYolluklarOrtalamaFiyatSil(Kullanici kullanan, ParYollukOrtalamaFiyat ortalamaFiyat)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ParYolluklarOrtalamaFiyat.Sil(kullanan, ortalamaFiyat);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Verilen veri türleri listesi için yapılan en son değişiklik zamanını bulup döndüren yordam.
        /// </summary>
        /// <param name="degZaman">En son değişim zamanı bulunacak veri türlerinin listesini tutan dizi. Yıl, veri türü, kurkod, senaryo yada ay bilgileri kullanılır.</param>
        /// <returns>Verilen kısta bilgiler kullanılarak, değişim zamanı en yeni olan verinin, değişim zamanı döndürülür.</returns>
        public TNSDateTime IslemKayitZaman(DegisimZaman[] degZaman)
        {
            TNS.Veri.VeriErisim ve = TNS.Veri.VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(degZaman[0].yil), null);
            TABDEGISIMZAMAN zaman = new TABDEGISIMZAMAN(ve);
            TNSDateTime dt = zaman.OdenekKayitZamani(degZaman);
            ve.VeriErisimKapat();
            return dt;
        }

        public BirimMasrafPlaniGenel MasrafPlaniBulTavan(Kullanici kullanan, ENUMMasrafPlanAsama asama, Tertip kistasTertip, UcgenKodBilgileri ukb, string saymanlikBirim, bool yeni = false)
        {
            if (yeni)
                return MasrafPlani.MasrafPlaniBulTavanYeni(null, kullanan, asama, kistasTertip, ukb, saymanlikBirim);
            return MasrafPlani.MasrafPlaniBulTavan(null, kullanan, asama, kistasTertip, ukb, saymanlikBirim);
        }

        public TertipOdenek[] BirimMasrafPlaniOzetListele(Kullanici kullanan, int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama asama, string kurKod, string ekoKod, int ekBilgi)
        {
            return MasrafPlani.BirimMasrafPlaniOzetListele(null, kullanan, yil, kurumsalYapiKod, asama, kurKod, ekoKod, ekBilgi);
        }

        public Sonuc MasrafPlaniGenelOnayla(Kullanici kullanan, int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama mpAsama, int ay)
        {
            return MasrafPlani.MasrafPlaniGenelOnayla(null, kullanan, yil, kurumsalYapiKod, mpAsama, ay);
        }

        public Sonuc MasrafPlaniGenelOnayKaldir(Kullanici kullanan, int yil, string kurumsalYapiKod, string[] onayKalkacakBirimler, ENUMMasrafPlanAsama mpAsama)
        {
            return MasrafPlani.MasrafPlaniGenelOnayKaldir(null, kullanan, yil, kurumsalYapiKod, onayKalkacakBirimler, mpAsama);
        }

        public int MasrafPlaniGenelOnayDurumBul(Kullanici kullanan, int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama mpAsama)
        {
            return MasrafPlani.MasrafPlaniGenelOnayDurumBul(null, yil, kurumsalYapiKod, mpAsama);
        }

        public Sonuc MasrafPlaniGenelTertipKaydet(Kullanici kullanan, ENUMMasrafPlanAsama asama, TertipOdenek[] tertipOdenekler)
        {
            return MasrafPlani.MasrafPlaniGenelTertipKaydet(null, kullanan, asama, tertipOdenekler);
        }

        public TertipOdenek[] MPBirimTertipListele(Tertip kistasTertip, int saymanlik, string birim)
        {
            return MasrafPlani.MPBirimTertipListele(null, kistasTertip, saymanlik, birim);
        }

        public TertipOdenek[] MPBirimTertipListele(Tertip kistasTertip, string[] kurumsalYapiKodlar)
        {
            return MasrafPlani.MPBirimTertipListele(null, kistasTertip, kurumsalYapiKodlar);
        }

        public double[] MPMasrafPlaniOku(Kullanici kullanan, ENUMMasrafPlanAsama asama, Tertip kistasTertip, int islemNo, int ustIslemNo, bool onaysiz)
        {
            return MasrafPlani.MPMasrafPlaniOku(null, kullanan, asama, kistasTertip, islemNo, ustIslemNo, onaysiz);
        }

        public bool OGYYetkisiVarMi(Kullanici kullanan, string senaryoKod, Tertip tertip, ENUMOGY ogy)
        {
            return YetkiServis.OGYYetkisiVarMi(kullanan, senaryoKod, tertip, ogy);
        }

        public MPTavanTertip[] MPTavanListele(Kullanici kullanan, string senaryoKod, Tertip tertip, string kyKod, bool detayliMi, string saymanlikBirim)
        {
            return MasrafPlaniTavan.TavanListele(null, kullanan, senaryoKod, tertip, kyKod, detayliMi, saymanlikBirim);
        }

        public TertipOdenek[] MPTavanIlListele(Kullanici kullanan, Tertip tertip, string saymanlikBirim)
        {
            return MasrafPlaniTavan.TavanIlListele(null, kullanan, tertip, saymanlikBirim);
        }

        public Sonuc MPTavanKaydet(Kullanici kullanan, string odenekKontrolSenaryo, TertipOdenek[] tertipOdenek, bool detayliMi, string silKYKod, bool toplamYaz, string saymanlikBirim)
        {
            try
            {
                return MasrafPlaniTavan.MPTavanKaydet(null, kullanan, odenekKontrolSenaryo, tertipOdenek, detayliMi, silKYKod, toplamYaz, saymanlikBirim);
            }
            catch (Exception ex)
            {
                return new Sonuc(false, ex.Message, "");
            }
        }

        public bool MPTavanIlIzniBul(Kullanici kullanan, Tertip tertip)
        {
            return MasrafPlaniTavan.MPTavanIlIzniBul(null, kullanan, tertip);
        }

        public Sonuc MPIlIzniDegistir(Kullanici kullanan, Tertip tertip)
        {
            try
            {
                return MasrafPlaniTavan.MPIlIzniDegistir(null, kullanan, tertip);
            }
            catch (Exception ex)
            {
                return new Sonuc(false, ex.Message, "");
            }
        }

        public TertipOdenek[] MPListeleRapor(Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri ukb, bool ekBilgili, string saymanlikBirim)
        {
            return MasrafPlaniTavan.MPListeleRapor((TNS.Veri.VeriErisim)null, kullanan, kistasTertip, ukb, ekBilgili, saymanlikBirim);
        }

        public Sonuc MPTavanOnay(Kullanici kullanan, int yil, int detayMi, string kurKod, int onayKod)
        {
            return MasrafPlaniTavan.Onayla(null, kullanan, yil, detayMi, kurKod, onayKod);
        }

        public ObjectArray MPFazlaGonderilen(Kullanici kullanan, int detayMi, Tertip kistasTertip)
        {
            return MasrafPlaniTavan.MPFazlaGonderilen(null, kullanan, detayMi, kistasTertip);
        }

        public OdenekBilgi[] MPTavanGenelListele(Kullanici kullanan, string senaryoKod, Tertip kistasTertip)
        {
            return MasrafPlaniTavan.TavanGenelListele(null, kullanan, senaryoKod, kistasTertip);
        }

        public TertipOdenek[] MPTertipListeleAy(Kullanici kullanan, Tertip kistas, string birim)
        {
            return MasrafPlani.MPTertipListeleAy(null, kullanan, kistas, birim, true, true);
        }

        public Tertip[] TertipOdenekAlacaklarListele(Kullanici kullanan, Tertip tertip, bool kayitli)
        {
            return TertipTanim.TertipOdenekAlacaklarListele(null, kullanan, tertip, kayitli);
        }

        public Sonuc TertipOdenekAlacaklarKaydet(Kullanici kullanan, Tertip tertip, Tertip[] odenekAlacagiTertipler)
        {
            return TertipTanim.TertipOdenekAlacaklarKaydet(null, kullanan, tertip, odenekAlacagiTertipler);
        }

        public MPBilgi[] BirimMasrafPlaniBul(Kullanici kullanan, int yil, ENUMMasrafPlanAsama asama, string kurumsalYapiKod, string ekoKod)
        {
            return MasrafPlani.BirimMasrafPlaniBul(null, kullanan, yil, asama, kurumsalYapiKod, ekoKod);
        }

        public TertipOdenek[] MasrafPlaniTertipToplamGonderilen(Kullanici kullanan, ENUMMasrafPlanAsama eNUMMasrafPlanAsama, int ay, Tertip kistasTertip)
        {
            return MasrafPlani.MasrafPlaniTertipToplamGonderilen(null, kullanan, eNUMMasrafPlanAsama, ay, kistasTertip);
        }

        public Sonuc MasrafPlaniGenelOtomatikHesapla(Kullanici kullanan, int yil, string kurumsalYapiKod, string ekoKod2, double artisYuzde, bool ustuneYaz, bool bosIseYaz)
        {
            throw new Exception("yazılmadı");
        }

        public Sonuc MasrafPlaniGenelUretAHP(Kullanici kullanan, int yil, ENUMMasrafPlanAsama asama, string kurumsalYapiKod, bool ustuneYaz, int ay, Tertip tertip)
        {
            return MasrafPlani.MasrafPlaniGenelUretAHP(null, kullanan, yil, asama, kurumsalYapiKod, ustuneYaz, ay, tertip);
        }

        public BirimMasrafPlaniGenel MasrafPlaniBul(Kullanici kullanan, ENUMMasrafPlanAsama asama, Tertip kistasTertip, UcgenKodBilgileri ukb, EkDegerParam[] ekBilgiler)
        {
            return MasrafPlani.MasrafPlaniBul(null, kullanan, asama, kistasTertip, ukb, ekBilgiler);
        }

        public Sonuc MPRevizeFormKaydetEklDus(Kullanici kullanan, MPRevizeForm form, bool dortDuzeyDegisimUret, bool eklDus)
        {
            return MasrafPlaniRevize.MPRevizeFormKaydet(null, kullanan, form, dortDuzeyDegisimUret, eklDus);
        }

        public Sonuc MPRevizeFormKaydet(Kullanici kullanan, MPRevizeForm form)
        {
            return MPRevizeFormKaydet(kullanan, form, true);
        }

        public Sonuc MPRevizeFormKaydet(Kullanici kullanan, MPRevizeForm form, bool dortDuzeyDegisimUret)
        {
            return MPRevizeFormKaydetEklDus(kullanan, form, dortDuzeyDegisimUret, false);
        }

        public MPRevizeForm MPRevizeFormOku(Kullanici kullanan, int yil, int revizeNo)
        {
            return MasrafPlaniRevize.MPRevizeFormOku(null, kullanan, yil, revizeNo);
        }

        public Sonuc MPRevizeFormDurumAta(Kullanici kullanan, int yil, int revizeNo, BKD.ENUMRevizeFormDurum formDurum)
        {
            return MasrafPlaniRevize.MPRevizeFormDurumAta(null, kullanan, yil, revizeNo, formDurum);
        }

        public Sonuc MPRevizeFormDurumAta(Kullanici kullanan, TNSCollection formlar)
        {
            Sonuc s = MasrafPlaniRevize.MPRevizeFormDurumAta(null, kullanan, formlar);
            return s;
        }

        public TertipOdenek[] MPBirimListele(Kullanici kullanan, Tertip kistas)
        {
            return MasrafPlani.MPBirimListele(null, kullanan, kistas);
        }

        public TertipOdenek[] MPTertipListele(Kullanici kullanan, string kurumsalYapi, Tertip kistas)
        {
            return MasrafPlani.MPTertipListele(null, kullanan, kurumsalYapi, kistas);
        }

        public string FormKosulOl(Kullanici kullanan, MPRevizeFormSorgu form, string tabloAd, bool detayaBak, string JoinToDisTabloAd)
        {
            return MasrafPlaniRevize.FormKosulOl(form, tabloAd, detayaBak, JoinToDisTabloAd);
        }

        public MPRevizeForm[] MPRevizeFormListele(Kullanici kullanan, MPRevizeFormSorgu form)
        {
            return MasrafPlaniRevize.MPRevizeFormListele(null, kullanan, form);
        }

        public MPRevizeTalep[] MasrafPlaniRevizeTalepListele(Kullanici kullanan, MPRevizeTalep kistasForm)
        {
            return MasrafPlaniRevizeTalep.Listele(null, kullanan, kistasForm);
        }

        public Sonuc MPRevizeTelepKaydet(Kullanici kullanan, MPRevizeTalep form)
        {
            return MasrafPlaniRevizeTalep.Kaydet(null, kullanan, form, true);
        }

        public Sonuc MPRevizeTalepDurumAta(Kullanici kullanan, int yil, string kurumsalYapiKod, int revizeTalepNo, BKD.ENUMRevizeFormDurum eNUMRevizeFormDurum)
        {
            return MasrafPlaniRevizeTalep.DurumAta(null, kullanan, yil, kurumsalYapiKod, revizeTalepNo, eNUMRevizeFormDurum);
        }

        public MPRevizeTalep MPRevizeTalepOku(Kullanici kullanan, int yil, string kurumsalKod, int revizeTalepNo)
        {
            return MasrafPlaniRevizeTalep.Oku(null, kullanan, yil, kurumsalKod, revizeTalepNo);
        }

        public Sonuc[] MPRevizeVeyaTalepYap(Kullanici kullanan, MPRevizeTalep form, int eskiTalepNo, int eskiRevizeNo)
        {
            return MasrafPlaniRevizeTalep.MPRevizeVeyaTalepYap(null, kullanan, form, eskiTalepNo, eskiRevizeNo);
        }

        public bool TertipDegisimIzniVarMi(Kullanici kullanan, Tertip alanTertip, Tertip verenTertip)
        {
            return IsKurallari.TertipTanim.TertipDegisimIzniVarMi(null, kullanan, alanTertip, verenTertip);
        }

        public Sonuc MasrafPlaniKarsilananlariYaz(Kullanici kullanan, MPRevizeTalep[] revizeTalepler, int revizeNo, bool bosSatirKontroluYap)
        {
            return MasrafPlaniRevizeTalep.MasrafPlaniKarsilananlariYaz(null, kullanan, revizeTalepler, revizeNo, bosSatirKontroluYap);
        }

        public double[] MasrafPlaniOdenekBilgiBul(Tertip tertip)
        {
            return MasrafPlani.MasrafPlaniOdenekBilgiBul(null, tertip, false, 0);
        }

        public double[] MasrafPlaniOdenekBilgiBul(Tertip tertip, bool onaysiz, bool harcamaVar, bool blokeVar)
        {
            return MasrafPlani.MasrafPlaniOdenekBilgiBul(null, tertip, onaysiz, 0);
        }

        public double[] MasrafPlaniOdenekBilgiBul(Tertip tertip, bool onaysiz, int haricRevizeNo)
        {
            return MasrafPlani.MasrafPlaniOdenekBilgiBul(null, tertip, onaysiz, haricRevizeNo);
        }

        public Sonuc MasrafPlaniTalepleriIptalEt(Kullanici kullanan, MPRevizeTalep[] revizeTalepler)
        {
            return MasrafPlaniRevizeTalep.TalepleriIptalEt(null, kullanan, revizeTalepler);
        }

        public Sonuc MPRevize4DuzeyDegisimYap(Kullanici kullanan, int yil, int revizeNo)
        {
            return MasrafPlaniRevize.MPRevize4DuzeyDegisimYap(null, kullanan, yil, revizeNo);
        }

        public int TertipSayisiBul(Tertip tertip)
        {
            return TertipTanim.TertipSayisiBul(null, tertip);
        }

        public Sonuc TertipTanimKopyalaOncYil(Kullanici kullanan, Tertip tertip)
        {
            return TertipTanim.TertipTanimKopyalaOncYil(null, kullanan, tertip);
        }

        public Sonuc KaydetHarcamaSurecOnay(int yil, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ButceHarcamaSurecDeger().KaydetHarcamaSurecOnay(null, yil, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc KaydetHarcamaSurecAlim(int yil, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ButceHarcamaSurecDeger().KaydetHarcamaSurecAlim(null, yil, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc KaydetHarcamaSurecFatura(int yil, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ButceHarcamaSurecDeger().KaydetHarcamaSurecFatura(null, yil, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc KaydetHarcamaSurecTeslim(int yil, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ButceHarcamaSurecDeger().KaydetHarcamaSurecTeslim(null, yil, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc KaydetHarcamaSurecOdeme(int yil, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ButceHarcamaSurecDeger().KaydetHarcamaSurecOdeme(null, yil, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc SilSurecBilgi(int hedefYil, int yil, string kur, string fon, string fin, string eko)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new ButceHarcamaSurecDeger().SurecBilgiSil(null, hedefYil, yil, kur, fon, fin, eko);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray SurecBilgiListele(Kullanici kullanan, BHMHarcamaSorgu kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = ButceHarcamaSurecDeger.ListeleSurecBilgi(kullanan, kriter);
                return donusDeger;
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        /// <summary>
        /// Verilen yıl ve tertip şablonu için, girilmiş bütçe fişi sayısını döndüren yordam.
        /// </summary>
        /// <param name="tertip">Bütçe fişleri sayısı bulunacak tertip bilgisi.</param>
        /// <returns>Bütçe tertibi için kayıtlı bütçe fişi sayısı döndürülür.</returns>
        public int ButceFisiSayisiBul(Tertip tertip)
        {
            return BHMv2ButceFisi.ButceFisiSayisiBul(tertip);
        }

        /// <summary>
        /// Tertip yapısında verilen şablona uyan tertiplerin, bütçe fişleri bir önceki yıldan alınır ve kaydedilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Bütçe fişleri kopyalanacak tertip bilgisi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceFisiKopyalaOncYil(Kullanici kullanan, Tertip tertip)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ButceFisi.ButceFisiKopyalaOncYil(kullanan, tertip);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc ButceTasnifOnayla(Kullanici kullanan, int tasnifSiraNo, int yil, string kurKod, string asamaKod, int kaldir)
        {
            return IsKurallari.ButceTasnifIslemler.Onayla(null, kullanan, tasnifSiraNo, yil, kurKod, asamaKod, kaldir);
        }

        public int ButceTasnifOnayBul(int tasnifSiraNo, int yil, string kurKod, string asamaKod)
        {
            return IsKurallari.ButceTasnifIslemler.OnayBul(null, tasnifSiraNo, yil, kurKod, asamaKod);
        }

        /// <summary>
        /// Tavan üstü bilgilerini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="senaryoKod">Ödenek üstünün ait olduğu senaryo.</param>
        /// <param name="tertip"> Tavan üstü bilgisi okunacak tertiplerin kıstası</param>
        public TavanUstuTertipOdenek[] TavanUstuOdenekListele(Kullanici kullanan, string[] senaryoKod, KistasYapi tertip, bool butceFormdan)
        {
            return TavanUstuTertipOdenekHizmet.Listele(null, kullanan, senaryoKod, tertip, true, butceFormdan);
        }

        /// <summary>
        /// Tavan üstü ödenek bilgilerini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="senaryoKod">Ödenek üstünün ait olduğu senaryo.</param>
        /// <param name="odenekler">Kaydedilecek tavan üstü ödenek bilgileri</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc TavanUstuOdenekKaydet(Kullanici kullanan, string senaryoKod, TavanUstuTertipOdenek[] odenekler)
        {
            try
            {
                return TavanUstuTertipOdenekHizmet.Kaydet(null, kullanan, senaryoKod, odenekler);
            }
            catch (Exception ex)
            {
                return new Sonuc(false, ex.Message, "");
            }
        }


        public Sonuc TorbaTertipBirimOdenekKaydet(Kullanici kullanan, TorbaTertipBirimOdenek torbatertipbirim)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TorbaTertipBirimOdenekIs.Kaydet(null, kullanan, torbatertipbirim);
            }
            catch (Exception ex)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ex.ToString();
            }
            return sonuc;
        }

        public ObjectArray TorbaTertipBirimOdenekListele(Kullanici kullanan, TorbaTertipBirimOdenek torbatertipbirim)
        {
            ObjectArray sonuclar = new ObjectArray();
            try
            {
                sonuclar = TorbaTertipBirimOdenekIs.Listele(null, kullanan, torbatertipbirim);
            }
            catch (Exception ex)
            {
                sonuclar.sonuc.islemSonuc = false;
                sonuclar.sonuc.hataStr = ex.ToString();
            }
            return sonuclar;
        }


        public ObjectArray BirimTorbaOdenekTalepListele(Kullanici kullanan, BirimTorbaOdenekTalep talep)
        {
            ObjectArray sonuclar = new ObjectArray();
            try
            {
                sonuclar = BirimTorbaOdenekTalepIs.Listele(null, kullanan, talep);
            }
            catch (Exception ex)
            {
                sonuclar.sonuc.islemSonuc = false;
                sonuclar.sonuc.hataStr = ex.ToString();
            }
            return sonuclar;
        }

        public Sonuc BirimTorbaOdenekTalepKaydet(Kullanici kullanan, BirimTorbaOdenekTalep talep)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = BirimTorbaOdenekTalepIs.Kaydet(null, kullanan, talep);
            }
            catch (Exception ex)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ex.ToString();
            }
            return sonuc;
        }

        public Sonuc BirimTorbaOdenekTalepDurumGuncelle(Kullanici kullanan, BirimTorbaOdenekTalep talep)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = BirimTorbaOdenekTalepIs.DurumGuncelle(null, kullanan, talep);
            }
            catch (Exception ex)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ex.ToString();
            }
            return sonuc;
        }

        /// <summary>
        /// ProgramOnayAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ProgramOnayAna yapısında ProgramOnayAna:  bilgilerini tutan değişken</param>
        public ProgramOnay[] ListeleProgramOnayForm(Kullanici kullanan, ProgramOnay form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return ProgramOnayForm.Listele(null, kullanan, form);
        }

        /// <summary>
        /// ProgramOnayAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ProgramOnayAna yapısında ProgramOnayAna:  bilgilerini tutan değişken</param>
        public ProgramOnay OkuProgramOnayForm(Kullanici kullanan, ProgramOnay form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return ProgramOnayForm.Oku(null, kullanan, form);
        }

        /// <summary>
        /// ProgramOnayAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ProgramOnayAna yapısında ProgramOnayAna:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetProgramOnayForm(Kullanici kullanan, ProgramOnay form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return ProgramOnayForm.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc OnaylaProgramOdenek(Kullanici kullanan, int yil, string senaryoKod, string programKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return ProgramOnayForm.Onayla(null, kullanan, yil, senaryoKod, programKod);
        }

        public OdenekSenaryoDurum[] ButceOdenekDurumListele(Kullanici kullanan, int yil, string birim)
        {
            return ProgramOnayForm.ButceOdenekDurumListele(null, kullanan, yil, birim);
        }

        public Sonuc ButceOdenekDurumKaydet(Kullanici kullanan, int yil, string birim, OdenekSenaryoDurum[] onaylar)
        {
            return ProgramOnayForm.ButceOdenekDurumKaydet(null, kullanan, yil, birim, onaylar);
        }

        public BirimDegisken[] ListeleBirimDegisken(Kullanici kullanan, int yil, int saymanlik, string birim, string kurumKodu)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2BirimDegisken.Listele(null, kullanan, yil, saymanlik, birim, kurumKodu);
        }

        public Sonuc KaydetBirimDegisken(Kullanici kullanan, int yil, int saymanlik, string birim, string kurumKodu, BirimDegisken[] degiskenler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2BirimDegisken.Kaydet(null, kullanan, yil, saymanlik, birim, kurumKodu, degiskenler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public BirimDegiskenYapi BirimDegiskenYapiOku(Kullanici kullanan, string kurKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2BirimDegisken.BirimDegiskenYapiOku(null, kullanan, kurKod);
        }

        public Sonuc BirimDegiskenYapiSakla(Kullanici kullanan, string kurKod, BirimDegiskenYapi by)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2BirimDegisken.BirimDegiskenYapiSakla(null, kullanan, kurKod, by);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Tertip OncekiSonrakiTertip(Kullanici kullanan, int yil, Tertip simdikiTertip, Tertip kistasTertip, int yon)
        {
            return BHMv2ButceFisi.OncekiSonrakiTertip(null, kullanan, yil, simdikiTertip, kistasTertip, yon);
        }

        public string OnayliAsamaBul(int secYil)
        {
            return ProgramOnayForm.OnayliAsamaBul(null, secYil);
        }

        public Sonuc MPAHPdenMPye(Kullanici kullanan, int yil, ENUMMasrafPlanAsama asama, TNSCollection tertipler)
        {
            return MasrafPlani.MPAHPdenMPye(null, kullanan, yil, asama, tertipler);
        }

        public Sonuc MPdenMPye(Kullanici kullanan, int yil, ENUMMasrafPlanAsama kaynakAsama, ENUMMasrafPlanAsama hedefAsama)
        {
            return MasrafPlani.MPdenMPye(null, kullanan, yil, kaynakAsama, hedefAsama);
        }

        ///// <summary>
        ///// FaaliyetinYapildigiYer nesnesini veri tabanından okuyan fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">FaaliyetinYapildigiYer yapısında FaaliyetinYapildigiYer: Faaliyetin Yapıldığı Yer bilgilerini tutan değişken</param>
        //public FaaliyetinYapildigiYer[] ListFaaliyetinYapildigiYer(Kullanici kullanan, FaaliyetinYapildigiYer form)
        //{
        //    TNS.KYM.Arac.KulturDegistir(kullanan);
        //    return BKDFaaliyetinYapildigiYer.List(null, kullanan, form);
        //}

        ///// <summary>
        ///// FaaliyetinYapildigiYer nesnesini veri tabanından okuyan fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">FaaliyetinYapildigiYer yapısında FaaliyetinYapildigiYer: Faaliyetin Yapıldığı Yer bilgilerini tutan değişken</param>
        //public FaaliyetinYapildigiYer GetFaaliyetinYapildigiYer(Kullanici kullanan, FaaliyetinYapildigiYer form)
        //{
        //    TNS.KYM.Arac.KulturDegistir(kullanan);
        //    return BKDFaaliyetinYapildigiYer.Get(null, kullanan, form);
        //}

        //public FaaliyetinYapildigiYer[] ListeleTumunu(Kullanici kullanan, FaaliyetinYapildigiYer form)
        //{
        //    TNS.KYM.Arac.KulturDegistir(kullanan);
        //    return BKDFaaliyetinYapildigiYer.ListeleTumunu(null, kullanan, form);
        //}

        //public bool IllereGoreFaaliyetGetir(Kullanici kullanan, FaaliyetinYapildigiYer form, int ilkod)
        //{
        //    TNS.KYM.Arac.KulturDegistir(kullanan);
        //    return BKDFaaliyetinYapildigiYer.IllereGoreFaaliyetGetir(null, kullanan, form, ilkod);
        //}

        /// <summary>
        /// BirimButceHazirlik nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BirimButceHazirlik yapısında BirimButceHazirlik: Birim Bütçe Hazırlık bilgilerini tutan değişken</param>
        public ObjectArray ListeleBirimButceHazirlik(Kullanici kullanan, Tertip tertip, int saymanlik, string birim, string senaryo, bool birimGrupla, bool eko2)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BKDBirimButceHazirlik.Listele(null, kullanan, tertip, saymanlik, birim, senaryo, birimGrupla, eko2);
        }

        /// <summary>
        /// BirimButceHazirlik nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BirimButceHazirlik yapısında BirimButceHazirlik: Birim Bütçe Hazırlık bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SaklaBirimButceHazirlik(Kullanici kullanan, int yil, int saymanlik, string birim, string senaryoKod, TertipOdenek[] tertipler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BKDBirimButceHazirlik.Sakla(null, kullanan, yil, saymanlik, birim, senaryoKod, tertipler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray ListeleBirimButceKarsilama(Kullanici kullanan, Tertip tertip, int il, int saymanlik, string birim, bool sifirlarGizli, int tur)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BKDBirimButceHazirlik.ListeleBirimButceKarsilama(null, kullanan, tertip, il, saymanlik, birim, sifirlarGizli, tur);
        }

        public Sonuc BirimButceDetayOranla(Kullanici kullanan, Tertip tertip, int il, int saymanlik, string birim, double karsilanan, int dagitimYontem)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BKDBirimButceHazirlik.BirimButceDetayOranla(null, kullanan, tertip, il, saymanlik, birim, karsilanan, dagitimYontem);
        }

        public Sonuc OnayKodAtaTasra(Kullanici kullanan, int yil, int saymanlik, string birim, string asama, ENUMBirimOnayTur onayTur, ENUMTasraHazirlikOnayDurum onayDurum)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BKDBirimButceHazirlik.OnayKodAtaTasra(null, kullanan, yil, saymanlik, birim, asama, onayTur, onayDurum);
        }

        public Sonuc OnayKodAtaToplu(Kullanici kullanan, int yil, int saymanlik, string birim, string asama, ENUMBirimOnayTur onayTur, int onayKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BKDBirimButceHazirlik.OnayKodAtaToplu(null, kullanan, yil, saymanlik, birim, asama, onayTur, onayKod);
        }

        public ObjectArray OnayKodListeleTasra(Kullanici kullanan, int yil, int saymanlik, string birim, string senaryoKod, ENUMBirimOnayTur onayTur)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BKDBirimButceHazirlik.OnayKodListeleTasra(null, kullanan, yil, saymanlik, birim, senaryoKod, onayTur);
        }

        public int OnayKodOkuTasra(Kullanici kullanan, int yil, int saymanlik, string birim, string senaryoKod, ENUMBirimOnayTur onayTur)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BKDBirimButceHazirlik.OnayKodOkuTasra(null, kullanan, yil, saymanlik, birim, senaryoKod, onayTur);
        }

        public ObjectArray BirimButceHazirlikOnayDurum(Kullanici kullanan, int yil, string senaryoKod, int saymanlik, string birim)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BKDBirimButceHazirlik.BirimButceHazirlikOnayDurum(null, kullanan, yil, saymanlik, birim, senaryoKod);
        }

        public bool IlKullanicisiMi(Kullanici kullanan, int ilKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BKDBirimButceHazirlik.IlKullanicisiMi(kullanan, ilKod);
        }

        public Sonuc KaydetBHMTavanGenel(Kullanici kullanan, int yil, string senaryo, ENUMTavanTur tavanTur, TNSCollection tavanListe)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TavanGenelIslemler.Kaydet(null, kullanan, yil, senaryo, tavanTur, tavanListe);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        public ObjectArray ListeleBHMTavanGenel(Kullanici kullanan, int yil, string senaryo, ENUMTavanTur tavanTur, TavanGenel kistas, bool sadeceOnayli, bool bosEkle = false)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TavanGenelIslemler.Listele(null, kullanan, yil, senaryo, tavanTur, kistas, sadeceOnayli, bosEkle);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// Tavan listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl: Listelenecek tavan yılını gösterir</param>
        /// <param name="ekoKod">ekoKod: Listelenecek ekonomik kodu gösterir</param>
        /// <param name="sektorKod">sektorKod: Listelenecek sektör kodunu gösterir</param>
        public ObjectArray ListeleEkoSektorTavan(Kullanici kullanan, int yil, string senaryoKod, string ekoKod, string sektorKod, string pebBirimKod, string oncekiYilSenaryo)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return TavanGenelIslemler.ListeleEkoSektorTavan(null, kullanan, yil, senaryoKod, ekoKod, sektorKod, pebBirimKod, oncekiYilSenaryo);
        }

        public ObjectArray ListeleEkoSektorFaaliyetTavan(Kullanici kullanan, int yil, string senaryoKod, string ekoKod, string sektorKod, string pebBirimKod, string pebFaalKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return TavanGenelIslemler.ListeleEkoSektorFaaliyetTavan(null, kullanan, yil, senaryoKod, ekoKod, sektorKod, pebBirimKod, pebFaalKod, false);
        }

        public ObjectArray ListeleYatirimProjeTavan(Kullanici kullanan, int yil, string senaryoKod, string ekoKod, string sektorKod, string pebBirimKod, string pebFaalKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return TavanGenelIslemler.ListeleYatirimProjeTavan(null, kullanan, yil, senaryoKod, ekoKod, sektorKod, pebBirimKod, pebFaalKod);
        }

        public ObjectArray ListeleYatirimVizeTavan(Kullanici kullanan, int yil, string senaryoKod, string pebBirimKod, string ekoKod, string finKod, string pebFaalKod, int projeKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return TavanGenelIslemler.ListeleYatirimVizeTavan(null, kullanan, yil, senaryoKod, pebBirimKod, ekoKod, finKod, pebFaalKod, projeKod);
        }

        public ObjectArray ListeleYatirimVizeTavanRapor(Kullanici kullanan, int yil, string senaryoKod, bool detay, string pebKod, string ekoKod, string finKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return TavanGenelIslemler.ListeleYatirimVizeTavanRapor(null, kullanan, yil, senaryoKod, detay, pebKod, ekoKod, finKod);
        }

        public ObjectArray ListeleGorusmeCari(Kullanici kullanan, Tertip tertip, string senaryoKod, bool eko2)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return TavanGenelIslemler.ListeleGorusmeCari(null, kullanan, tertip, senaryoKod, eko2);
        }

        public ObjectArray ListeleTertipTavan(Kullanici kullanan, int yil, string senaryoKod, string[] kodlar)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return TavanGenelIslemler.ListeleTertipTavan(null, kullanan, yil, senaryoKod, kodlar);
        }

        public ObjectArray ListeleTertipVize(Kullanici kullanan, int yil, string senaryoKod, string finkod, string ekoKod, string pebFaalKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return TavanGenelIslemler.ListeleTertipVize(null, kullanan, yil, senaryoKod, finkod, ekoKod, pebFaalKod);
        }

        public TavanGenel BHMTavanGenelOnayOku(Kullanici kullanan, int yil, string senaryo, ENUMTavanTur tavanTur, TavanGenel form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TavanGenelIslemler.BHMTavanGenelOnayOku(null, kullanan, yil, senaryo, tavanTur, form);
            }
            catch
            {
                return null;
            }
        }

        public ObjectArray BHMTavanGenelOnayListele(Kullanici kullanan, int yil, string senaryo, ENUMTavanTur tavanTur, TavanGenel form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TavanGenelIslemler.BHMTavanGenelOnayListele(null, kullanan, yil, senaryo, tavanTur, form);
            }
            catch (Exception ex)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(ex.Message);
                return oa;
            }
        }

        public Sonuc BHMTavanGenelOnayKaydet(Kullanici kullanan, int yil, string senaryo, ENUMTavanTur tavanTur, TavanGenel form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TavanGenelIslemler.BHMTavanGenelOnayKaydet(null, kullanan, yil, senaryo, tavanTur, form);
            }
            catch (Exception ex)
            {
                return new Sonuc(false, ex.Message, "");
            }
        }

        /// <summary>
        /// MaliDurumBeklenti nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MaliDurumBeklenti yapısında MaliDurumBeklenti:  bilgilerini tutan değişken</param>
        public MaliDurumBeklenti[] ListeleBHMMaliDurumBeklenti(Kullanici kullanan, MaliDurumBeklenti form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2BHMMaliDurumBeklenti.Listele(null, kullanan, form);
        }

        /// <summary>
        /// MaliDurumBeklenti nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MaliDurumBeklenti yapısında MaliDurumBeklenti:  bilgilerini tutan değişken</param>
        public MaliDurumBeklenti OkuBHMMaliDurumBeklenti(Kullanici kullanan, MaliDurumBeklenti form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2BHMMaliDurumBeklenti.Oku(null, kullanan, form);
        }

        /// <summary>
        /// MaliDurumBeklenti nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MaliDurumBeklenti yapısında MaliDurumBeklenti:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetBHMMaliDurumBeklenti(Kullanici kullanan, MaliDurumBeklenti form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2BHMMaliDurumBeklenti.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// IOIProje nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IOIProje yapısında IOIProje:  bilgilerini tutan değişken</param>
        public ObjectArray ListeleIOIProjeGiris(Kullanici kullanan, IOIProje form, bool detaylarOlsun, int butceYil)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2IOIProjeGiris.Listele(null, kullanan, form, detaylarOlsun, null, butceYil);
        }

        public ObjectVAN ListeleIOIProjePEBBilgi(Kullanici kullanan, int yil, string pebKod)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2IOIProjeGiris.PEBBilgi(null, kullanan, yil, pebKod);
        }


        /// <summary>
        /// IOIProje nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IOIProje yapısında IOIProje:  bilgilerini tutan değişken</param>
        public ObjectVAN OkuIOIProjeGiris(Kullanici kullanan, IOIProje form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return BHMv2IOIProjeGiris.Oku(null, kullanan, form);
        }

        /// <summary>
        /// IOIProje nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IOIProje yapısında IOIProje:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetIOIProjeGiris(Kullanici kullanan, IOIProje form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2IOIProjeGiris.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc SilIOIProjeGiris(Kullanici kullanan, IOIProje form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2IOIProjeGiris.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray ListeleYIKOBProjeGiris(Kullanici kullanan, YIKOBProje form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return YIKOBProjeGiris.Listele(null, kullanan, form);
        }

        public Sonuc KaydetYIKOBProjeGiris(Kullanici kullanan, YIKOBProje form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return YIKOBProjeGiris.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectVAN OkuYIKOBProjeGiris(Kullanici kullanan, YIKOBProje form)
        {
            TNS.KYM.Arac.KulturDegistir(kullanan);
            return YIKOBProjeGiris.Oku(null, kullanan, form);
        }

        public Sonuc SilYIKOBProjeGiris(Kullanici kullanan, YIKOBProje form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return YIKOBProjeGiris.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray SektorKullanilmisMi(Kullanici kullanan, int yil, string senaryoKod, string sektorKod)
        {
            return TavanGenelIslemler.SektorKullanilmisMi(null, kullanan, yil, senaryoKod, sektorKod);
        }

        public Sonuc TavanUretEkoSektorBirim(Kullanici kullanan, int yil, string senaryoKod, string kaynakSenaryoKod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return TavanGenelIslemler.TavanUretEkoSektorBirim(null, kullanan, yil, senaryoKod, kaynakSenaryoKod);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        /// <summary>
        /// ButceCalismasiPersonelAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ButceCalismasiPersonelAna yapısında ButceCalismasiPersonelAna: Bütçe Çalışmalarında Görevli Personel Listesi bilgilerini tutan değişken</param>
        public ObjectArray ListeleButceCalismasiPersoneli(Kullanici kullanan, ButceCalismasiPersoneli form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ButceCalismasiPersoneli.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// ButceCalismasiPersonelAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ButceCalismasiPersonelAna yapısında ButceCalismasiPersonelAna: Bütçe Çalışmalarında Görevli Personel Listesi bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetButceCalismasiPersoneli(Kullanici kullanan, TNSCollection formlar)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ButceCalismasiPersoneli.Kaydet(null, kullanan, formlar);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc KopyalaButceCalismasiPersoneli(Kullanici kullanan, int yil)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return BHMv2ButceCalismasiPersoneli.Kopyala(null, kullanan, yil);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public PEBProjeBilgi PEBBilgiBul(Kullanici kullanan, int yil, string peb)
        {
            return MasrafPlaniRevize.PEBBilgiBul(null, kullanan, yil, peb);
        }

        public ObjectArray HizmetMaliyetiTasraListele(Kullanici kullanan, int hizMalKontrolYil, int saymanlik, string birim, string kurKod, bool kurumsalToplam)
        {
            return TNS.BHF.Arac.Tanimla().HizmetMaliyetiTasraListele(kullanan, hizMalKontrolYil, saymanlik, birim, kurKod, kurumsalToplam);
        }

        public TertipOdenek[] DagitilmayanlariHesapla(Kullanici kullanan, int yil)
        {
            Sonuc s = new Sonuc();
            return TNS.BHMv2.IsKurallari.MasrafPlani.DagitilmayanlariHesapla(null, kullanan, yil, ENUMMasrafPlanAsama.AHP, s);
        }

        public Hashtable ProgramButceCevrimOku(int yil)
        {
            return TNS.BHMv2.IsKurallari.BHMv2ButceOdenek.ProgramButceCevrimOku(null, yil);
        }

        public Sonuc ProjeKaydet(Kullanici kullanan, ProjeTeklif form)
        {
            try
            {
                return BHMv2Proje.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray ProjeListele(Kullanici kullanan, ProjeTeklif form)
        {
            try
            {
                return BHMv2Proje.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }
        public Sonuc ProjeSil(Kullanici kullanan, ProjeTeklif form)
        {
            try
            {
                return BHMv2Proje.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public bool ProjeKayitVarMi(Kullanici kullanan, string projeKod)
        {
            try
            {
                return BHMv2Proje.KayitVarMi(null, kullanan, projeKod);
            }
            catch
            {
                return false;
            }
        }


        public Sonuc ProjeBirimKaydet(Kullanici kullanan, ProjeBirim form)
        {
            try
            {
                return BHMv2ProjeBirim.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray ProjeBirimListele(Kullanici kullanan, ProjeBirim form)
        {
            try
            {
                return BHMv2ProjeBirim.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }
        public Sonuc ProjeBirimSil(Kullanici kullanan, ProjeBirim form)
        {
            try
            {
                return BHMv2ProjeBirim.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }


        public Sonuc ProjeYurutucuKaydet(Kullanici kullanan, ProjeYurutucu form)
        {
            try
            {
                return BHMv2ProjeYurutucu.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray ProjeYurutucuListele(Kullanici kullanan, ProjeYurutucu form)
        {
            try
            {
                return BHMv2ProjeYurutucu.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }
        public Sonuc ProjeYurutucuSil(Kullanici kullanan, ProjeYurutucu form)
        {
            try
            {
                return BHMv2ProjeYurutucu.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }


        public Sonuc ProjeFaaliyetKaydet(Kullanici kullanan, ProjeFaaliyet form)
        {
            try
            {
                return BHMv2ProjeFaaliyet.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        public ObjectArray ProjeFaaliyetListele(Kullanici kullanan, ProjeFaaliyet form)
        {
            try
            {
                return BHMv2ProjeFaaliyet.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }
        public Sonuc ProjeFaaliyetSil(Kullanici kullanan, ProjeFaaliyet form)
        {
            try
            {
                return BHMv2ProjeFaaliyet.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }


        public Sonuc ProjeUrunKaydet(Kullanici kullanan, ProjeUrun form)
        {
            try
            {
                return BHMv2ProjeUrun.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray ProjeUrunListele(Kullanici kullanan, ProjeUrun form)
        {
            try
            {
                return BHMv2ProjeUrun.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }
        public Sonuc ProjeUrunSil(Kullanici kullanan, ProjeUrun form)
        {
            try
            {
                return BHMv2ProjeUrun.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }


        public Sonuc ProjeFinansmanKaydet(Kullanici kullanan, ProjeFinansman form)
        {
            try
            {
                return BHMv2ProjeFinansman.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray ProjeFinansmanListele(Kullanici kullanan, ProjeFinansman form)
        {
            try
            {
                return BHMv2ProjeFinansman.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }
        public Sonuc ProjeFinansmanSil(Kullanici kullanan, ProjeFinansman form)
        {
            try
            {
                return BHMv2ProjeFinansman.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }


        public Sonuc ProjeGirdiKaydet(Kullanici kullanan, ProjeGirdi form)
        {
            try
            {
                return BHMv2ProjeGirdi.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray ProjeGirdiListele(Kullanici kullanan, ProjeGirdi form)
        {
            try
            {
                return BHMv2ProjeGirdi.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }
        public Sonuc ProjeGirdiSil(Kullanici kullanan, ProjeGirdi form)
        {
            try
            {
                return BHMv2ProjeGirdi.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        /*
        public Sonuc ProjeBirimIlgiliKaydet(Kullanici kullanan, ProjeBirimIlgili form)
        {
            try
            {
                return BHMv2ProjeBirimIlgili.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray ProjeBirimIlgiliListele(Kullanici kullanan, ProjeBirimIlgili form)
        {
            try
            {
                return BHMv2ProjeBirimIlgili.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }
        public Sonuc ProjeBirimIlgiliSil(Kullanici kullanan, ProjeBirimIlgili form)
        {
            try
            {
                return BHMv2ProjeBirimIlgili.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
*/

        public Sonuc ProjeSorunKaydet(Kullanici kullanan, ProjeSorun form)
        {
            try
            {
                return BHMv2ProjeSorun.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray ProjeSorunListele(Kullanici kullanan, ProjeSorun form)
        {
            try
            {
                return BHMv2ProjeSorun.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }
        public Sonuc ProjeSorunSil(Kullanici kullanan, ProjeSorun form)
        {
            try
            {
                return BHMv2ProjeSorun.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }


        public Sonuc FaaliyetKaydet(Kullanici kullanan, Faaliyet form)
        {
            try
            {
                return BHMv2Faaliyet.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray FaaliyetListele(Kullanici kullanan, Faaliyet form)
        {
            try
            {
                return BHMv2Faaliyet.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }
        public Sonuc FaaliyetSil(Kullanici kullanan, Faaliyet form)
        {
            try
            {
                return BHMv2Faaliyet.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }


        public Sonuc FinansmanKaynakKaydet(Kullanici kullanan, FinansmanKaynak form)
        {
            try
            {
                return BHMv2FinansmanKaynak.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray FinansmanKaynakListele(Kullanici kullanan, FinansmanKaynak form)
        {
            try
            {
                return BHMv2FinansmanKaynak.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }
        public Sonuc FinansmanKaynakSil(Kullanici kullanan, FinansmanKaynak form)
        {
            try
            {
                return BHMv2FinansmanKaynak.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        public Sonuc UrunKaydet(Kullanici kullanan, Urun form)
        {
            try
            {
                return BHMv2Urun.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray UrunListele(Kullanici kullanan, Urun form)
        {
            try
            {
                return BHMv2Urun.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }
        public Sonuc UrunSil(Kullanici kullanan, Urun form)
        {
            try
            {
                return BHMv2Urun.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        public Sonuc ProjeFaaliyetGostergeKaydet(Kullanici kullanan, ProjeFaaliyetGosterge form)
        {
            try
            {
                return BHMv2ProjeFaaliyetGosterge.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray ProjeFaaliyetGostergeListele(Kullanici kullanan, ProjeFaaliyetGosterge form)
        {
            try
            {
                return BHMv2ProjeFaaliyetGosterge.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }
        public Sonuc ProjeFaaliyetGostergeSil(Kullanici kullanan, ProjeFaaliyetGosterge form)
        {
            try
            {
                return BHMv2ProjeFaaliyetGosterge.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        public Sonuc ProjeAciklamaTurKaydet(Kullanici kullanan, ProjeAciklamaTur form)
        {
            try
            {
                return BHMv2ProjeAciklamaTur.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray ProjeAciklamaTurListele(Kullanici kullanan, ProjeAciklamaTur form)
        {
            try
            {
                return BHMv2ProjeAciklamaTur.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }
        public Sonuc ProjeAciklamaTurSil(Kullanici kullanan, ProjeAciklamaTur form)
        {
            try
            {
                return BHMv2ProjeAciklamaTur.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        public Sonuc ProjeAciklamaKaydet(Kullanici kullanan, ProjeAciklama form)
        {
            try
            {
                return BHMv2ProjeAciklama.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }
        public ObjectArray ProjeAciklamaListele(Kullanici kullanan, ProjeAciklama form)
        {
            try
            {
                return BHMv2ProjeAciklama.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

        }
        public Sonuc ProjeAciklamaSil(Kullanici kullanan, ProjeAciklama form)
        {
            try
            {
                return BHMv2ProjeAciklama.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        public ObjectArray SPAmacHedefListele(Kullanici kullanan)
        {
            try
            {
                return BHMv2ProjeAciklama.SPAmacHedefListele(null, kullanan);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc = new Sonuc(e.Message);
                return oa;
            }
        }

        public ObjectVAN ProjeFaaliyetGerceklesmeListele(Kullanici kullanan, int yil, int ay, string faaliyetId)
        {
            try
            {
                return BHMv2ProjeFaaliyet.ProjeFaaliyetGerceklesmeListele(null, kullanan, yil, ay, faaliyetId, true);
            }
            catch (Exception e)
            {
                ObjectVAN oa = new ObjectVAN();
                oa.sonuc = new Sonuc(e.Message);
                return oa;
            }
        }

        public Sonuc ProjeFaaliyetGerceklesmeKaydet(Kullanici kullanan, ProjeFaaliyetGerceklesme gerc)
        {
            try
            {
                return BHMv2ProjeFaaliyet.ProjeFaaliyetGerceklesmeKaydet(null, kullanan, gerc);
            }
            catch (Exception e)
            {
                return new Sonuc(e.Message);
            }
        }

        public ObjectArray IOIProjeHarcamaListele(Kullanici kullanan, int ioiProjeNo)
        {
            try
            {
                return BHMv2IOIProjeGiris.IOIProjeHarcamaListele(null, kullanan, ioiProjeNo, false);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc = new Sonuc(e.Message);
                return oa;
            }
        }

        public ObjectArray IOIProjeAktarimListele(Kullanici kullanan, int ioiProjeNo)
        {
            try
            {
                return BHMv2IOIProjeGiris.IOIProjeAktarimListele(null, kullanan, ioiProjeNo, false);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc = new Sonuc(e.Message);
                return oa;
            }
        }

        public Sonuc IOIProjeHarcamaKaydet(Kullanici kullanan, int ioiProjeNo, IOIProjeHarcama[] liste)
        {
            try
            {
                return BHMv2IOIProjeGiris.IOIProjeHarcamaKaydet(null, kullanan, ioiProjeNo, liste);
            }
            catch (Exception e)
            {
                return new Sonuc(e.Message);
            }
        }

        public Sonuc IOIProjeAktarimKaydet(Kullanici kullanan, int ioiProjeNo, IOIProjeAktarilan[] liste)
        {
            try
            {
                return BHMv2IOIProjeGiris.IOIProjeAktarimKaydet(null, kullanan, ioiProjeNo, liste);
            }
            catch (Exception e)
            {
                return new Sonuc(e.Message);
            }
        }

        public Sonuc ButceIslemUret(Kullanici kullanan, int yil, MPRevizeSatir islemSatir, int yontem)
        {
            return MasrafPlaniRevize.ButceIslemUret(null, kullanan, yil, islemSatir, yontem);
        }

        public Sonuc KaydetMPDagitim(Kullanici kullanan, MPDagitimAna form)
        {
            try
            {
                return MPDagitimServis.Kaydet(null, kullanan, form);
            }
            catch (Exception e) { return new Sonuc(e.Message); }
        }

        public ObjectArray ListeleMPDagitim(Kullanici kullanan, MPDagitimAna form)
        {
            try
            {
                return MPDagitimServis.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.Message);
                return oa;
            }
        }

        public ObjectVAN OkuMPDagitim(Kullanici kullanan, int id)
        {
            try
            {
                return MPDagitimServis.Oku(null, kullanan, id);
            }
            catch (Exception e)
            {
                ObjectVAN oa = new ObjectVAN();
                oa.sonuc.DegerAta(e.Message);
                return oa;
            }
        }

        public ObjectVAN MPDagitimBirimEkle(Kullanici kullanan, int id, string birimKod, double tutar)
        {
            try
            {
                return MPDagitimServis.BirimEkle(null, kullanan, id, birimKod, tutar);
            }
            catch (Exception e)
            {
                ObjectVAN oa = new ObjectVAN();
                oa.sonuc.DegerAta(e.Message);
                return oa;
            }
        }

        public Sonuc MPDagitimRevizeUret(Kullanici kullanan, int id, int ay)
        {
            try
            {
                return MPDagitimServis.RevizeUret(null, kullanan, id, ay);
            }
            catch (Exception e)
            {
                return new Sonuc(e.Message);
            }
        }
    }
}
