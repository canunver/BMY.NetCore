using System;
using TNS.UZY;
using TNS.KYM;
using TNS.BHMv2;
using TNS.PEB;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// IPEBServis arayüzünden türeyen, istemciden gelen çağrıları
    /// ilgili sınıflardaki ilgili yordamlara yönlendiren fonksiyonları tutan sınıf
    /// </summary>
    [Serializable()]
    public class PEBServis : MarshalByRefObject, IPEBServis
    {
        /// <summary>
        /// Uzaylar servisine ulaşmak için kullanılan değişken
        /// </summary>
        private static IUZYServis servisUZY = null;

        public static bool tabloDegisimYap = true;

        /// <summary>
        /// Parametresiz Performans bütçe servisi nesnesi oluşturucu yordam
        /// </summary>
        public PEBServis()
        {
            lock ("OncekiIslemiBiteneKadarBekle")
            {
                if (tabloDegisimYap)
                {
                    tabloDegisimYap = false;
                    TNS.PEB.IsKurallari.PEBTabloDegisiklikleri.Kaydet();
                }
            }
        }

        /// <summary>
        /// Gönderilen uzaydan verilen kriterlere uygun olan uzay alan bilgisini çeken yordam
        /// uzayAlan parametresi boş ise varsayılan kullanılır.
        /// </summary>
        /// <param name="uzayAd">Uzay adı</param>
        /// <param name="kosul">Uzaya verilecek koşul bilgisi</param>
        /// <param name="esitle">esitle true ise koşullar koşul alanlarına eşitlenir, false ise like kullanılır.</param>
        /// <param name="uzayAlan">Değeri istenen uzay alanı</param>
        /// <returns>İstenen uzay alanı değeri döndürülür.</returns>
        public static string UzayYaziDegerGetir(string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            if (servisUZY == null)
                servisUZY = TNS.UZY.Arac.Tanimla();

            string deger = "";
            if (uzayAlan == "")
                deger = servisUZY.UzayDegeriStr(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle);
            else
                deger = servisUZY.UzayDegeriStr(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle, uzayAlan);

            return deger;
        }

        /// <summary>
        /// Gönderilen uzaydan verilen kriterlere uygun olan uzay alan bilgisini çeken yordam
        /// uzayAlan parametresi boş ise varsayılan kullanılır.
        /// </summary>
        /// <param name="uzayAd">Uzay adı</param>
        /// <param name="kosul">Uzaya verilecek koşul bilgisi</param>
        /// <param name="esitle">esitle true ise koşullar koşul alanlarına eşitlenir, false ise like kullanılır.</param>
        /// <param name="uzayAlan">Değeri istenen uzay alanı</param>
        /// <returns>İstenen uzay alanı değeri döndürülür.</returns>
        public static string UzayYaziDegerGetir(Kullanici kullanan, TNS.Veri.VeriErisim ve, string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            if (servisUZY == null)
                servisUZY = TNS.UZY.Arac.Tanimla();

            string deger = servisUZY.UzayKosulStr(kullanan, uzayAd, kosul, esitle, uzayAlan);
            return ve.TekKolonSQLStr(deger);
        }

        /// <summary>
        /// Gönderilen uzaydan verilen kriterlere uygun olan uzay alan bilgisini çeken yordam
        /// uzayAlan parametresi boş ise varsayılan kullanılır.
        /// </summary>
        /// <param name="uzayAd">Uzay adı</param>
        /// <param name="kosul">Uzaya verilecek koşul bilgisi</param>
        /// <param name="esitle">esitle true ise koşullar koşul alanlarına eşitlenir, false ise like kullanılır.</param>
        /// <param name="uzayAlan">Değeri istenen uzay alanı</param>
        /// <returns>İstenen uzay alanı değeri döndürülür.</returns>
        public static double UzaySayiDegerGetir(string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            if (servisUZY == null)
                servisUZY = TNS.UZY.Arac.Tanimla();

            double deger = 0;
            if (uzayAlan == "")
                deger = servisUZY.UzayDegeriDbl(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle);
            else
                deger = servisUZY.UzayDegeriDbl(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle, uzayAlan);

            return deger;
        }

        /// <summary>
        /// Dil kaydedebilmek için işlem yapan kullanıcının
        /// Dil işlemlerine yetkili olması ve yazma hakkının olması gerekir.
        /// Program kullanıcının yetkili olduğuna karar verirse Dil kayıt edilir.
        /// </summary>
        /// <param name="kullanan">
        /// Kullanıcı bilgilerinin tutulduğu nesne.
        /// Kullanıcının ilgili işleme yetkili olup olmadığı kontrolünde bu nesne kullanılır.
        /// </param>
        /// <param name="dil">
        /// Dil bilgilerinin tutulduğu nesne.
        /// </param>
        /// <returns>
        /// Bu fonksiyon Sonuc nesnesi döndürür.
        /// Sonuc nesnesi içinde işlemin başarılı olup olmadığı bilgisi yer alır.
        /// </returns>
        public Sonuc DilKaydet(Kullanici kullanan, Dil dil)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRDil.Kaydet(null, kullanan, dil);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan dil bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dil">Dil bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında dil listesi döndürülür.</returns>
        public ObjectArray DilListele(Kullanici kullanan, Dil dil)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRDil.Listele(null, kullanan, dil);
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
        /// Dil, stratejik planda kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Dil nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dil">Dil bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc DilSil(Kullanici kullanan, Dil dil)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRDil.Sil(null, kullanan, dil);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Misyon varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Misyon nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="misyon">Misyon bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc MisyonKaydet(Kullanici kullanan, Misyon misyon)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRMisyon.Kaydet(null, kullanan, misyon);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Verilen kriterlere uygun olan misyon bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="misyon">Misyon bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında misyon nesnesi döndürülür.</returns>
        public ObjectArray MisyonListele(Kullanici kullanan, Misyon misyon)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRMisyon.Listele(null, kullanan, misyon);
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
        /// Vizyon varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Vizyon nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="vizyon">Vizyon bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc VizyonKaydet(Kullanici kullanan, Vizyon vizyon)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRVizyon.Kaydet(null, kullanan, vizyon);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan vizyon bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="vizyon">Vizyon bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında vizyon nesnesi döndürülür.</returns>
        public ObjectArray VizyonListele(Kullanici kullanan, Vizyon vizyon)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRVizyon.Listele(null, kullanan, vizyon);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Tema varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Tema nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tema">Tema bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc TemaKaydet(Kullanici kullanan, Tema tema)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRTema.Kaydet(null, kullanan, tema);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan tema bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tema">Tema bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tema listesi döndürülür.</returns>
        public ObjectArray TemaListele(Kullanici kullanan, Tema tema)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRTema.Listele(null, kullanan, tema);
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
        /// Verilen dönem başlangıç yılına ait tema özet/kısa bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tema listesi döndürülür.</returns>
        public ObjectArray TemaKisaListele(Kullanici kullanan, string kurumKod, int donemBYil)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRTema.TemaKisaListele(null, kullanan, kurumKod, donemBYil);
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
        /// Tema, stratejik amaçta kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// Tema nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="temalar">Tema nesnelerini tutan liste</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc TemaSil(Kullanici kullanan, string kurumKod, TNSCollection temalar)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRTema.Sil(null, kullanan, kurumKod, temalar);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc BirimMisyonVizyonKaydet(Kullanici kullanan, BirimMisyonVizyon misyonVizyon, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRBirimMisyonVizyon.Kaydet(null, kullanan, misyonVizyon, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc BirimMisyonVizyonSil(Kullanici kullanan, BirimMisyonVizyon misyonVizyon, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRBirimMisyonVizyon.Sil(null, kullanan, misyonVizyon, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public BirimMisyonVizyon BirimMisyonVizyonListele(Kullanici kullanan, BirimMisyonVizyon misyonVizyon)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimMisyonVizyon.Listele(null, kullanan, misyonVizyon);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public BirimMisyonVizyon[] BirimMisyonVizyonSorgula(Kullanici kullanan, BirimMisyonVizyon misyonVizyon)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimMisyonVizyon.Sorgula(null, kullanan, misyonVizyon);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc KurumsalAnalizKaydet(Kullanici kullanan, KurumsalAnaliz[] kalar, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRKurumsalAnaliz.Kaydet(null, kullanan, kalar, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc KurumsalAnalizSil(Kullanici kullanan, KurumsalAnaliz ka, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRKurumsalAnaliz.Sil(null, kullanan, ka, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public KurumsalAnaliz[] KurumsalAnalizListele(Kullanici kullanan, KurumsalAnaliz ka)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRKurumsalAnaliz.Listele(null, kullanan, ka);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public KurumsalAnaliz[] KurumsalAnalizSorgula(Kullanici kullanan, KurumsalAnaliz ka)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRKurumsalAnaliz.Sorgula(null, kullanan, ka);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc KurumsalAnalizKaydetSablon(Kullanici kullanan, KurumsalAnalizSablon sablon)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRKurumsalAnaliz.KaydetSablon(null, kullanan, sablon);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc KurumsalAnalizSilSablon(Kullanici kullanan, KurumsalAnalizSablon sablon)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRKurumsalAnaliz.SilSablon(null, kullanan, sablon);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public KurumsalAnalizSablon[] KurumsalAnalizListeleSablon(Kullanici kullanan, KurumsalAnalizSablon sablon)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRKurumsalAnaliz.ListeleSablon(null, kullanan, sablon);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public KurumsalAnaliz[] KurumsalAnalizListeleSablonDetay(Kullanici kullanan, KurumsalAnaliz ka)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRKurumsalAnaliz.ListeleSablonDetay(null, kullanan, ka);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Stratejik amaç varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Stratejik Amaç nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="strAmac">Stratejik amaç bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc StratejikAmacKaydet(Kullanici kullanan, StratejikAmac strAmac)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRAmac.Kaydet(null, kullanan, strAmac);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Verilen kriterlere uygun olan stratejik amaç bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="strAmac">Stratejik amaç bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik amaç listesi döndürülür.</returns>
        public ObjectArray StratejikAmacListele(Kullanici kullanan, StratejikAmac strAmac)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRAmac.Listele(null, kullanan, strAmac);
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
        /// Verilen dönem başlangıç yılına ait stratejik amaç özet/kısa bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik amaç listesi döndürülür.</returns>
        public ObjectArray StratejikAmacKisaListele(Kullanici kullanan, string kurumKod, int donemBYil)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRAmac.AmacKisaListele(null, kullanan, kurumKod, donemBYil);
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
        /// Stratejik amaç, stratejik hedefte kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// Stratejik Amaç nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="amaclar">Stratejik Amaç nesnelerini tutan liste</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc StratejikAmacSil(Kullanici kullanan, TNSCollection amaclar)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRAmac.Sil(null, kullanan, amaclar);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Stratejik hedef varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Stratejik Hedef nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="strHedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc StratejikHedefKaydet(Kullanici kullanan, StratejikHedef strHedef)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRHedef.Kaydet(null, kullanan, strHedef);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Verilen kriterlere uygun olan stratejik hedef bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="strHedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik hedef listesi döndürülür.</returns>
        public ObjectArray StratejikHedefListele(Kullanici kullanan, StratejikHedef strHedef, bool hedefler)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRHedef.Listele(null, kullanan, strHedef, hedefler);
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
        /// Verilen dönem başlangıç yılına ait stratejik hedef özet/kısa bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik hedef listesi döndürülür.</returns>
        public ObjectArray StratejikHedefKisaListele(Kullanici kullanan, string kurumKod, int donemBYil)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRHedef.HedefKisaListele(null, kullanan, kurumKod, donemBYil);
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
        /// Stratejik hedef, performans hedefte kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// Stratejik Hedef nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hedefler">Stratejik Hedef nesnelerini tutan liste</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc StratejikHedefSil(Kullanici kullanan, TNSCollection hedefler)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRHedef.Sil(null, kullanan, hedefler);
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
        /// Ölçü birim varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen OlcuBirim nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="olcuBirim">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc OlcuBirimKaydet(Kullanici kullanan, OlcuBirim olcuBirim)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBOlcuBirim.Kaydet(null, kullanan, olcuBirim);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ölçü birim bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="olcuBirim">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ölçü birim listesi döndürülür.</returns>
        public ObjectArray OlcuBirimListele(Kullanici kullanan, OlcuBirim olcuBirim)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBOlcuBirim.Listele(null, kullanan, olcuBirim);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Ölçü birim, performans göstergede kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen OlcuBirim nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="olcuBirim">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc OlcuBirimSil(Kullanici kullanan, OlcuBirim olcuBirim)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBOlcuBirim.Sil(null, kullanan, olcuBirim);
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
        /// Performans gösterge varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGosterge nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gosterge">Performans gösterge bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PerformansGostergeKaydet(Kullanici kullanan, PerformansGosterge gosterge)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBGosterge.Kaydet(null, kullanan, gosterge);
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
        /// Verilen kriterlere uygun olan performans gösterge bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gosterge">Performans gösterge bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans gösterge listesi döndürülür.</returns>
        public ObjectArray PerformansGostergeListele(Kullanici kullanan, PerformansGosterge gosterge)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBGosterge.Listele(null, kullanan, gosterge);
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
        /// Performans gösterge, hedeflenen ve gerçekleşen değerlerde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGosterge nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gosterge">Performans gösterge bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PerformansGostergeSil(Kullanici kullanan, PerformansGosterge gosterge)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBGosterge.Sil(null, kullanan, gosterge);
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
        /// Performans göstergesi hedeflenen değerlerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGostergeHedeflenen nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedeflenen">Performans göstergesi hedeflenen değerlerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PerformansGostergeHedeflenenKaydet(Kullanici kullanan, PerformansGostergeHedeflenen pHedeflenen)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBGostergeHedef.Kaydet(null, kullanan, pHedeflenen);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Verilen kriterlere uygun olan performans göstergesi hedeflenen değerleri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGostergeHedeflenen nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedeflenen">Performans göstergesi hedeflenen değerlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans göstergesi hedeflenen değerler listesi döndürülür.</returns>
        public ObjectArray PerformansGostergeHedeflenenListele(Kullanici kullanan, PerformansGostergeHedeflenen pHedeflenen)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBGostergeHedef.Listele(null, kullanan, pHedeflenen);
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
        /// Performans göstergesi gerçekleşen değerlerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGostergeGerceklesen nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gerceklesenler">Performans göstergesi gerçekleşen değerlerini tutan PerformansGostergeGerceklesen yapısında nesneler tutan dizi.</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PerformansGostergeGerceklesenKaydet(Kullanici kullanan, TNSCollection gerceklesenler)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBGostergeGercek.Kaydet(null, kullanan, gerceklesenler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Verilen kriterlere uygun olan performans göstergesi gerçekleşen değerleri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansGostergeGerceklesen nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pGerceklesen">Performans göstergesi gerçekleşen değerlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans göstergesi gerçekleşen değerler listesi döndürülür.</returns>
        public ObjectArray PerformansGostergeGerceklesenListele(Kullanici kullanan, PerformansGostergeGerceklesen pGerceklesen)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBGostergeGercek.Listele(null, kullanan, pGerceklesen);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc PerformansGostergeGerceklesenOnayla(Kullanici kullanan, PerformansGostergeGerceklesen pGerceklesen)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBGostergeGercekOnay.DurumDegistir(null, kullanan, pGerceklesen, "onayla");
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public bool PerformansGostergeGerceklesenOnayliMi(Kullanici kullanan, PerformansGostergeGerceklesen pGerceklesen)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBGostergeGercekOnay.OnayliMi(null, kullanan, pGerceklesen);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc PerformansGostergeGerceklesenOnayKaldir(Kullanici kullanan, PerformansGostergeGerceklesen pGerceklesen)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBGostergeGercekOnay.DurumDegistir(null, kullanan, pGerceklesen, "onayKaldir");
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Performans hedef varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen PerformansHedef nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// Performans hedefin stratejik hedeflerle bağının ve ağırlık bilgilerinin de ilgili yordamı çağırarak kaydedilmesini sağlar.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedef">Performans hedef bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PerformansHedefKaydet(Kullanici kullanan, PerformansHedef pHedef)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBPerHedef.Kaydet(null, kullanan, pHedef);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        /// <summary>
        /// Verilen kriterlere uygun olan performans hedef bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedef">Performans hedef bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Performans hedefin stratejik hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="detayGosterge">Performans hedefe ait performans gösterge bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="kurumDahil">Kurumun performans hedeflerinin listeye dahil edilip edilmeyeceği bilgisi</param>
        /// <param name="asama">Performans göstergesi hedeflenen değerlerinin istendiği aşama</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        public ObjectArray PerformansHedefListele(Kullanici kullanan, PerformansHedef pHedef, bool detayAgirlik, bool detayGosterge, bool kurumDahil, int asama)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBPerHedef.Listele(null, kullanan, pHedef, detayAgirlik, detayGosterge, kurumDahil, asama);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan performans hedef bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedef">Performans hedef bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Performans hedefin stratejik hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedef programatik kodu</param>
        /// <param name="detayGosterge">Performans hedefe ait performans gösterge bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="kurumDahil">Kurumun performans hedeflerinin listeye dahil edilip edilmeyeceği bilgisi</param>
        /// <param name="asama">Performans göstergesi hedeflenen değerlerinin istendiği aşama</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        public ObjectArray PerformansHedefListele(Kullanici kullanan, PerformansHedef pHedef, bool detayAgirlik, int donemBYil, int strHedefKod, bool detayGosterge, bool kurumDahil, int asama)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBPerHedef.Listele(null, kullanan, pHedef, detayAgirlik, donemBYil, strHedefKod, detayGosterge, kurumDahil, asama);
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
        /// Verilen kriterlere uygun olan performans hedef bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedef">Performans hedef bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Performans hedefin stratejik hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedef programatik kodu</param>
        /// <param name="kurumDahil">Kurumun performans hedeflerinin listeye dahil edilip edilmeyeceği bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        public ObjectArray PerformansHedefListele(Kullanici kullanan, PerformansHedef pHedef, bool detayAgirlik, int donemBYil, int strHedefKod, bool kurumDahil)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBPerHedef.Listele(null, kullanan, pHedef, detayAgirlik, donemBYil, strHedefKod, kurumDahil);
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
        /// Performans hedefi, faaliyet/projelerde, performans göstergesi hedeflenen/gerçekleşen değerlerinde
        /// ve sabit maliyetlerde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedef">PerformansHedef nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PerformansHedefSil(Kullanici kullanan, TNSCollection pHedef)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBPerHedef.Sil(null, kullanan, pHedef);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Faaliyet/proje varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen FaaliyetProje nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// Faaliyet/projenin performans hedeflerle bağının ve ağırlık bilgilerinin de ilgili yordamı çağırarak kaydedilmesini sağlar.
        /// Aynı zamanda bütçe hazırlık alt faaliyetlerle bağının da ilgili yordamı çağırarak kaydedilmesini sağlar.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc FaaliyetProjeKaydet(Kullanici kullanan, FaaliyetProje fp)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBFaaliyetProje.Kaydet(null, kullanan, fp);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/proje bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Faaliyet/projenin performans hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="perHedefKod">Performans hedef programatik kodu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje listesi döndürülür.</returns>
        public ObjectArray FaaliyetProjeListele(Kullanici kullanan, FaaliyetProje fp, bool detayAgirlik, int perHedefKod)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBFaaliyetProje.Listele(null, kullanan, fp, detayAgirlik, perHedefKod);
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
        /// Verilen kriterlere uygun olan faaliyet/proje bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Faaliyet/projenin performans hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="aktarEbutce">e-butce sistemine aktarım için mi çağrılıyor yoksa sistem kullanımı için mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje listesi döndürülür.</returns>
        public ObjectArray FaaliyetProjeListele(Kullanici kullanan, FaaliyetProje fp, bool detayAgirlik, bool aktarEbutce)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBFaaliyetProje.Listele(null, kullanan, fp, detayAgirlik, aktarEbutce);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/proje bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgisini tutan nesne</param>
        /// <param name="detayAgirlik">Faaliyet/projenin performans hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        /// <param name="perHedefKod">Performans hedef programatik kodu</param>
        /// <param name="aktarEbutce">e-butce sistemine aktarım için mi çağrılıyor yoksa sistem kullanımı için mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje listesi döndürülür.</returns>
        public ObjectArray FaaliyetProjeListele(Kullanici kullanan, FaaliyetProje fp, bool detayAgirlik, int perHedefKod, bool aktarEbutce)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBFaaliyetProje.Listele(null, kullanan, fp, detayAgirlik, perHedefKod, aktarEbutce);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Faaliyet/proje, sabit maliyetlerde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fpler">FaaliyetProje nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc FaaliyetProjeSil(Kullanici kullanan, TNSCollection fpler)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBFaaliyetProje.Sil(null, kullanan, fpler);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Faaliyet/projenin ilgili birimlerini alt faaliyet bağı üzerinden bütçe hazırlıkta
        /// ödenek girişleri sırasında kullanılmış tertiplerden bulup döndüren yordam
        /// Faaliyet/projeler e-butce sistemine aktarılırken ilgili birimleri de istendiği için yazıldı.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgisini tutan nesne</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/projeye ait ilgili birimler string olarak liste şeklinde döndürülür.</returns>
        public ObjectArray FaaliyetProjeIlgiliBirimleriBul(Kullanici kullanan, FaaliyetProje fp, string senaryoKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBFaaliyetProje.IlgiliBirimleriBul(null, kullanan, fp, senaryoKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/proje ödenek bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fpOdenek">Faaliyet/proje ödenek bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje ödenek listesi döndürülür.</returns>
        public ObjectArray FaaliyetProjeOdenekListele(Kullanici kullanan, FaaliyetProjeOdenek fpOdenek)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBFaaliyetProjeOdenek.Listele(null, kullanan, fpOdenek);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/proje ödenek bilgilerini döndüren yordam
        /// Ödenekler e-butce sisteminin istediği yapıda yani ekonomik kodun ilk kırılımı bazında toplanıyor.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="tur">ENUMFaaliyetProjeTur listesindeki türlerden biri olmalıdır.</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje ödenek listesi döndürülür.</returns>
        public ObjectArray FaaliyetProjeOdenekleriEbutce(Kullanici kullanan, int yil, string senaryoKod, int tur)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBFaaliyetProjeOdenek.FaaliyetProjeOdenekleriEbutce(null, kullanan, yil, senaryoKod, tur);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Sabit maliyet varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen SabitMaliyet nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// Sabit maliyetin bütçe hazırlık alt faaliyetlerle bağının da ilgili yordamı çağırarak kaydedilmesini sağlar.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sm">Sabit maliyet bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc SabitMaliyetKaydet(Kullanici kullanan, SabitMaliyet sm)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                return TNS.PEB.IsKurallari.PEBSabitMaliyet.Kaydet(null, kullanan, sm);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Verilen kriterlere uygun olan sabit maliyet bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// Sabit maliyetin bütçe ile bağını kuran alt faaliyet bilgilerini de listeye ekler.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sm">Sabit maliyet bilgisini tutan nesne</param>
        /// <param name="detayOdenek">Sabit maliyetin ödenek bilgilerinin istenip istenmediği bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet listesi döndürülür.</returns>
        public ObjectArray SabitMaliyetListele(Kullanici kullanan, SabitMaliyet sm, bool detayOdenek)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBSabitMaliyet.Listele(null, kullanan, sm, detayOdenek);
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
        /// Sabit maliyeti ve detay bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="smler">SabitMaliyet nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc SabitMaliyetSil(Kullanici kullanan, TNSCollection smler)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBSabitMaliyet.Sil(null, kullanan, smler);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan sabit maliyet ödenek bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="smOdenek">Sabit maliyet ödenek bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet ödenek listesi döndürülür.</returns>
        public ObjectArray SabitMaliyetOdenekListele(Kullanici kullanan, SabitMaliyetOdenek smOdenek)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBSabitMaliyetOdenek.Listele(null, kullanan, smOdenek);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan sabit maliyet ödenek bilgilerini döndüren yordam
        /// Ödenekler e-butce sisteminin istediği yapıda yani ekonomik kodun ilk kırılımı bazında toplanıyor.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet ödenek listesi döndürülür.</returns>
        public ObjectArray SabitMaliyetOdenekleriEbutce(Kullanici kullanan, int yil, string senaryoKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBSabitMaliyetOdenek.SabitMaliyetOdenekleriEbutce(null, kullanan, yil, senaryoKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Sabit maliyet ağırlık bilgilerini alarak sabit maliyetin ödeneğini bu ağırlıkları
        /// kullanarak ilgili faaliyet/projelere yansıyan ödenekleri hesaplayıp kaydeden yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="smAgirlik">Sabit maliyet ağırlık bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc SabitMaliyetAgirlikKaydet(Kullanici kullanan, TNSCollection smAgirlik)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBSabitMaliyetAgirlik.Kaydet(null, kullanan, smAgirlik);
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
        /// Verilen kriterlere uygun olan sabit maliyet ağırlık ve yansıyan ödenek bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="smAgirlik">Sabit maliyet ağırlık bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet ağırlık ve yansıyan ödenek listesi döndürülür.</returns>
        public ObjectArray SabitMaliyetAgirlikListele(Kullanici kullanan, SabitMaliyetAgirlik smAgirlik)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBSabitMaliyetAgirlik.Listele(null, kullanan, smAgirlik);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/projeleri bulan ve ödeneklerinden ağırlıklarını hesaplıp döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="smAgirlik">Sabit maliyet ağırlık bilgilerini tutan nesne</param>
        /// <param name="tur">ENUMSabitMaliyetTur listesindeki türlerden biri olmalıdır.</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet ağırlık listesi döndürülür.</returns>
        public ObjectArray SabitMaliyetAgirlikHesapla(Kullanici kullanan, SabitMaliyetAgirlik smAgirlik, int tur)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBSabitMaliyetAgirlik.AgirlikHesapla(null, kullanan, smAgirlik, tur);
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
        /// Verilen kriterlere uygun olan birim bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Kurum kodu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında birim listesi döndürülür.</returns>
        public ObjectArray BirimListele(Kullanici kullanan, int yil, string kurKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBKurSinif.Listele(null, kullanan, yil, kurKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen stratejik hedefe ait performans hedefleri ve ödeneklerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedef programatik kodu</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        public ObjectArray StrHedefRaporla(Kullanici kullanan, int donemBYil, int strHedefKod, string senaryoKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.StrHedefRaporla(null, kullanan, donemBYil, strHedefKod, senaryoKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen stratejik amaca ait stratejik hedefleri ve ödeneklerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strAmacKod">Stratejik amaç programatik kodu</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında StrAmacRapor nesnesi döndürülür.</returns>
        public ObjectArray StrAmacRaporla(Kullanici kullanan, int donemBYil, int strAmacKod, string senaryoKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.StrAmacRaporla(null, kullanan, donemBYil, strAmacKod, senaryoKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet, proje ve sabit maliyet ödeneklerini
        /// ve yansıtılan ödenekleri tertip bazında ekonomik kodun 4. düzeyinde toplayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında OdenekDurumListesi nesnesi döndürülür.</returns>
        public ObjectArray OdenekDurumRaporla(Kullanici kullanan, int yil, string kurKod, string odenekSenaryo)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.OdenekDurumRaporla(null, kullanan, yil, kurKod, odenekSenaryo);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet, proje ve sabit maliyet ödeneklerini
        /// ve yansıtılan ödenekleri tertip bazında ekonomik kodun 2. düzeyinde toplayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında OdenekDurumListesi nesnesi döndürülür.</returns>
        public ObjectArray OdenekDurumRaporla2Duzey(Kullanici kullanan, int yil, string kurKod, string odenekSenaryo)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.OdenekDurumRaporla2Duzey(null, kullanan, yil, kurKod, odenekSenaryo);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/projeleri, ödeneklerini ve sabit maliyetlerden
        /// yansıyan ödenekleri toplayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ButceFaaliyetDagilim nesneleri listesi döndürülür.</returns>
        public ObjectArray ButceFaaliyetDagilimRaporla(Kullanici kullanan, int yil, string odenekSenaryo)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.ButceFaaliyetDagilimRaporla(null, kullanan, yil, odenekSenaryo);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uyan temaları, stratejik amaçları, stratejik hedefleri,
        /// performans hedeflerini, faaliyet/projeleri ve ödeneklerini toparlayıp döndüren yordam
        /// Ödeneklerin toplamı ve birim bazında dağılımı gösteriliyor ayrı ayrı.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında TemaOdenek nesneleri listesi döndürülür.</returns>
        public ObjectArray PerformansHedefDagilimRaporla(Kullanici kullanan, int donemBYil, int yil, string senaryoKod, string kurKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.PerformansHedefDagilimRaporla(null, kullanan, donemBYil, yil, senaryoKod, kurKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen performans hedefin bilgilerini, destekleyen faaliyet/proje ağırlıklarını
        /// ve ödeneklerini, bağlı olan performans göstergelerini ve değerlerini,
        /// bağlı olduğu stratejik hedefleri ve ağırlıklarını toparlayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hedef">Performans hedefi bilgilerini tutan nesne</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        public ObjectArray PerformansHedefKartiListele(Kullanici kullanan, PerformansHedef hedef, string odenekSenaryo, int gercekYil, int fpTur, int yansiyan)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBRaporlar.PerformansHedefKartiListele(null, kullanan, hedef, odenekSenaryo, gercekYil, fpTur, yansiyan);
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
        /// Verilen faaliyet/projenin bilgilerini, bağlı olduğu performans hedeflerini
        /// ve ağırlıklarını, ekonomik kod, fonksiyonel kod ve finansman kod bazında
        /// ödenek toplamlarını toparlayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fp">Faaliyet/proje bilgilerini tutan nesne</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje listesi döndürülür.</returns>
        public ObjectArray FaaliyetProjeKartiListele(Kullanici kullanan, FaaliyetProje fp, string odenekSenaryo)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.FaaliyetProjeKartiListele(null, kullanan, fp, odenekSenaryo);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen sabit maliyetin bilgilerini, ödeneklerinin yansıtıldığı faaliyet/projeleri
        /// ve ağırlıklarını, ekonomik kod, fonksiyonel kod ve finansman kod bazında
        /// ödenek toplamlarını toparlayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sm">Sabit maliyet bilgilerini tutan nesne</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet listesi döndürülür.</returns>
        public ObjectArray SabitMaliyetKartiListele(Kullanici kullanan, SabitMaliyet sm, string odenekSenaryo)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.SabitMaliyetKartiListele(null, kullanan, sm, odenekSenaryo);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uyan stratejik amacı veya amaçları ve ödeneklerini bulup döndüren yordam
        /// Performans bütçede yer almayan ödenekler de bulunur ve listeye eklenir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strAmacKod">Stratejik amaç programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="odenekSenaryo">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında PEBOzetRapor nesnesi döndürülür.</returns>
        public ObjectArray PEBOzetRaporla(Kullanici kullanan, int donemBYil, int strAmacKod, int yil, string odenekSenaryo)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.PEBOzetRaporla(null, kullanan, donemBYil, strAmacKod, yil, odenekSenaryo);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uyan stratejik hedefleri ve ekonomik kod bazında ödeneklerini bulup döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında StratejikHedefOdenekDagilim nesneleri listesi döndürülür.</returns>
        public ObjectArray StrHedefOdenekEkoDagilimRaporla(Kullanici kullanan, int yil, string senaryoKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.StrHedefOdenekEkoDagilimRaporla(null, kullanan, yil, senaryoKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uyan stratejik hedefleri ve fonksiyonel kod bazında ödeneklerini bulup döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında StratejikHedefOdenekDagilim nesneleri listesi döndürülür.</returns>
        public ObjectArray StrHedefOdenekFonDagilimRaporla(Kullanici kullanan, int yil, string senaryoKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.StrHedefOdenekFonDagilimRaporla(null, kullanan, yil, senaryoKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uyan stratejik amaçları ve ekonomik kod bazında ödeneklerini bulup döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında StratejikAmacOdenekDagilim nesneleri listesi döndürülür.</returns>
        public ObjectArray StrAmacOdenekEkoDagilimRaporla(Kullanici kullanan, int yil, string senaryoKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.StrAmacOdenekEkoDagilimRaporla(null, kullanan, yil, senaryoKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uyan stratejik amaçları ve fonksiyonel kod bazında ödeneklerini bulup döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında StratejikAmacOdenekDagilim nesneleri listesi döndürülür.</returns>
        public ObjectArray StrAmacOdenekFonDagilimRaporla(Kullanici kullanan, int yil, string senaryoKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.StrAmacOdenekFonDagilimRaporla(null, kullanan, yil, senaryoKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Tedbir varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Tedbir nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// Tedbirin detay bilgileri ve detayın da mevcut durum bilgileri de kayıt edilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tedbir">Tedbir bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc TedbirKaydet(Kullanici kullanan, Tedbir tedbir)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBTedbir.Kaydet(null, kullanan, tedbir);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan tedbir, detay ve mevcut durum bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tedbir">Tedbir bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tedbir nesnesi döndürülür.</returns>
        public ObjectArray TedbirListele(Kullanici kullanan, Tedbir tedbir)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBTedbir.Listele(null, kullanan, tedbir);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan tedbir mevcut tarihlerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında mevcut tarih listesi döndürülür.</returns>
        public ObjectArray TedbirMevcutTarihListele(Kullanici kullanan, int yil, int kurKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBTedbir.ListeleMevcutTarih(null, kullanan, yil, kurKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Tedbir birim listesini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tedbir birimleri listesi döndürülür.</returns>
        public ObjectArray TedbirBirimListele(Kullanici kullanan)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBTedbir.ListeleTedbirBirim(null, kullanan);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uyan performans göstergelerini ve ödeneklerini toplayıp döndüren yordam
        /// Bu raporda, performans göstergeleri hedeflenen değerler giriş ekranında sadece ilk sıraya girilen
        /// performans göstergeleri listeleniyor ve çıktıda performans hedefleri olarak görüntüleniyor.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Ödeneği istenen senaryo kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans gösterge listesi döndürülür.</returns>
        public ObjectArray PerformansGostergeOdenekRaporla(Kullanici kullanan, int yil, string senaryoKod, string kurKod)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBRaporlar.PerformansGostergeOdenekRaporla(null, kullanan, yil, senaryoKod, kurKod);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc = new Sonuc(false, e.ToString(), "");
                return donusDeger;
            }
        }

        /// <summary>
        /// Verilen kriterlere uyan performans göstergelerini, hedeflenen ve gerçekleşen değerlerini,
        /// bağlı olduğu performans hedeflerini ve stratejik hedefleri toparlayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında BirimGostergeleriRaporu nesneleri listesi döndürülür.</returns>
        public ObjectArray BirimGostergeleriRaporla(Kullanici kullanan, int yil, string kurKod, int[] gostergeSeviye)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.BirimGostergeleriRaporla(null, kullanan, yil, kurKod, gostergeSeviye);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen performans hedefi ve performans göstergesi kriterlerine uyan performans
        /// göstergesinin hedeflenen ve gerçekleşen değerlerini toparlayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pHedef">Performans hedef bilgilerini tutan nesne</param>
        /// <param name="pGosterge">Performans gösterge bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef nesnesi döndürülür.</returns>
        public ObjectArray PerformansGostergeDetayRaporla(Kullanici kullanan, PerformansHedef pHedef, PerformansGosterge pGosterge)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.PerformansGostergeDetayRaporla(null, kullanan, pHedef, pGosterge);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uyan performan göstergelerini, hedeflenen ve gerçekleşen değerlerini toplayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında PerformansGostergeGerceklesen nesneleri listesi döndürülür.</returns>
        public ObjectArray PerformansGostergeRaporla(Kullanici kullanan, int yil, string kurKod, int[] gostergeSeviye)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.PerformansGostergeRaporla(null, kullanan, yil, kurKod, gostergeSeviye);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uyan performan göstergelerini, hedeflenen ve gerçekleşen değerlerini toplayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="perHedef">Performans hedef bilgilerini tutan nesne</param>
        /// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında PerformansGostergeGerceklesen nesneleri listesi döndürülür.</returns>
        public ObjectArray PerformansGostergeRaporla(Kullanici kullanan, PerformansHedef perHedef, int[] gostergeSeviye)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.PerformansGostergeRaporla(null, kullanan, perHedef, gostergeSeviye);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uyan stratejik hedefleri ve bağlı olan I. ve II. seviye performans göstergelerini toparlayıp döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="asama">Aşama kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans gösterge listesi döndürülür.</returns>
        public ObjectArray StratejikGostergeRaporla(Kullanici kullanan, int donemBYil, int yil, int asama)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.StratejikGostergeRaporla(null, kullanan, donemBYil, yil, asama);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Daire varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Daire nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="daire">Daire bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc DaireKaydet(Kullanici kullanan, Daire daire)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBDaire.Kaydet(null, kullanan, daire);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan daire bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="daire">Daire bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında daire listesi döndürülür.</returns>
        public ObjectArray DaireListele(Kullanici kullanan, Daire daire)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBDaire.Listele(null, kullanan, daire);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Daire, operasyonel plan alt faaliyetlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Daire nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="daire">Daire bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc DaireSil(Kullanici kullanan, Daire daire)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBDaire.Sil(null, kullanan, daire);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Operasyonel plan alt faaliyeti varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AltFaaliyet nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="af">Alt faaliyet bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc AltFaaliyetKaydet(Kullanici kullanan, AltFaaliyet af)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBAltFaaliyet.Kaydet(null, kullanan, af);
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
        /// Verilen kriterlere uygun olan operasyonel plan alt faaliyeti bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="af">Alt faaliyet bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında operasyonel plan alt faaliyeti listesi döndürülür.</returns>
        public ObjectArray AltFaaliyetListele(Kullanici kullanan, AltFaaliyet af)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBAltFaaliyet.Listele(null, kullanan, af);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Operasyonel plan alt faaliyeti, iş/görevde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AltFaaliyet nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="af">Alt faaliyet bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc AltFaaliyetSil(Kullanici kullanan, AltFaaliyet af)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBAltFaaliyet.Sil(null, kullanan, af);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// İş/görev varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen IsGorev nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// İş/görevin zaman planı, sorumlu personelleri ve performans göstergeleri de ilgili yordamlar çağırılarak kayıt edilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc IsGorevKaydet(Kullanici kullanan, IsGorev ig)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBIsGorev.Kaydet(null, kullanan, ig);
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
        /// Verilen kriterlere uygun olan iş/görev bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// detayVer parametresi true ise iş/görevin zaman planı, sorumlu
        /// personelleri ve performans göstergeleri de listeye eklenir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <param name="detayVer">İş/göreve ait detay bilgiler listelensin mi listelenmesin mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında iş/görev listesi döndürülür.</returns>
        public ObjectArray IsGorevListele(Kullanici kullanan, IsGorev ig, bool detayVer)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBIsGorev.Listele(null, kullanan, ig, detayVer);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// İş/görev bilgilerini, zaman planını, sorumlu personellerini ve performans göstergelerini silen yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="igler">IsGorev nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc IsGorevSil(Kullanici kullanan, TNSCollection igler)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBIsGorev.Sil(null, kullanan, igler);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen personelin iş/görev bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// detayVer parametresi true ise iş/görevin zaman planı, sorumlu
        /// personelleri ve performans göstergeleri de listeye eklenir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="tcKimlikNo">İş/görev bilgileri istenen personelin T.C. kimlik numarası</param>
        /// <param name="detayVer">İş/göreve ait detay bilgiler listelensin mi listelenmesin mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında iş/görev listesi döndürülür.</returns>
        public ObjectArray PersonelIsGorevListele(Kullanici kullanan, int yil, string tcKimlikNo, bool detayVer)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBIsGorev.PersonelIsGorevListele(null, kullanan, yil, tcKimlikNo, detayVer);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan iş/görev bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// İş/görevin zaman planı, sorumlu personelleri ve performans göstergeleri de listeye eklenir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında iş/görev listesi döndürülür.</returns>
        public ObjectArray OperasyonelPlanYazdir(Kullanici kullanan, IsGorev ig)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBIsGorev.Yazdir(null, kullanan, ig);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SPGenelHavuzKaydet(Kullanici kullanan, SpGenelHavuz sp)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRGenelHavuz.Kaydet(null, kullanan, sp);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public SpGenelHavuz SPGenelHavuzListele(Kullanici kullanan, SpGenelHavuz sp)
        {
            SpGenelHavuz donusDeger = new SpGenelHavuz();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRGenelHavuz.Listele(null, kullanan, sp);
            }
            catch (Exception e)
            {
                throw e;
            }
            return donusDeger;
        }

        public SpGenelHavuz[] SPGenelHavuzListele(Kullanici kullanan, int donemNo, int orgBirimKod, string spGenelHavuzKod, string[] ustSpGenelHavuzKod, EnumSPGenelHavuzDurum durum)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRGenelHavuz.Listele(null, kullanan, donemNo, orgBirimKod, spGenelHavuzKod, ustSpGenelHavuzKod, durum);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public SpGenelHavuz[] SPGenelHavuzListele(Kullanici kullanan, string spGenelHavuzKod, EnumSPGenelHavuzDurum durum, bool altBagiOlanlar, params string[] birimTur)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRGenelHavuz.Listele(null, kullanan, spGenelHavuzKod, durum, altBagiOlanlar, birimTur);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public SpGenelHavuz[] SPGenelHavuzListele(Kullanici kullanan, string spGenelHavuzKod, EnumSPGenelHavuzDurum durum, bool bagVer, bool altBagiOlanlar, params string[] birimTur)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRGenelHavuz.Listele(null, kullanan, spGenelHavuzKod, durum, bagVer, altBagiOlanlar, birimTur);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public SpGenelHavuz[] SPGenelHavuzListele(Kullanici kullanan, string spGenelHavuzKod, string[] ustSpGenelHavuzKod, EnumSPGenelHavuzDurum durum, bool altBagiOlanlar, params string[] birimTur)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRGenelHavuz.Listele(null, kullanan, spGenelHavuzKod, ustSpGenelHavuzKod, durum, altBagiOlanlar, birimTur);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public SpGenelHavuzRapor[] SPGenelHavuzRaporListele(Kullanici kullanan, int donemNo, string birimTur, string ustKod)
        {
            SpGenelHavuzRapor[] donusDeger;
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRSpGenelHavuzRapor.Listele(null, kullanan, donemNo, birimTur, ustKod);
            }
            catch (Exception e)
            {
                throw e;
            }
            return donusDeger;
        }

        /// <summary>
        /// Dönem varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen donem nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donem">Dönem bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc DonemKaydet(Kullanici kullanan, Donem donem)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRDonem.Kaydet(null, kullanan, donem);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Dönemi silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen donem nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donem">Dönem bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc DonemSil(Kullanici kullanan, Donem donem)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRDonem.Sil(null, kullanan, donem);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Dönem ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <returns>Bir Dönem için Dönem ayrıntılarını döndürür</returns>
        public Donem[] DonemListele(Kullanici kullanan)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRDonem.Listele(null, kullanan, new Donem());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Verilen kriterlere uygun olan Dönem ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donem">Dönem bilgisini tutan nesne</param>
        /// <returns>Bir Dönem için Dönem ayrıntılarını döndürür</returns>
        public Donem[] DonemListele(Kullanici kullanan, Donem donem)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRDonem.Listele(null, kullanan, donem);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Personel-Görev ilişkisi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Gorevlendirme nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ekipler">Görevlendirme bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc GorevlendirmeKaydet(Kullanici kullanan, GorevlendirmeEkip[] ekipler, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRGorevlendirmeEkip.Kaydet(null, kullanan, ekipler, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Personel-Görev ilişkisini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Gorevlendirme nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ekip">Görevlendirme bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc GorevlendirmeSil(Kullanici kullanan, GorevlendirmeEkip ekip, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRGorevlendirmeEkip.Sil(null, kullanan, ekip, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan görevlendirme ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ekip">Görevlendirme bilgisini tutan nesne</param>
        /// <returns>Bir görevlendirme için görevlendirme ayrıntılarını döndürür</returns>
        public GorevlendirmeEkip[] GorevlendirmeListele(Kullanici kullanan, GorevlendirmeEkip ekip, bool detay)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRGorevlendirmeEkip.Listele(null, kullanan, ekip, detay);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public GorevlendirmeEkip[] GorevlendirmeSorgula(Kullanici kullanan, GorevlendirmeEkip ekip)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRGorevlendirmeEkip.Sorgula(null, kullanan, ekip);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc StratejiBelirlemeKaydet(Kullanici kullanan, StratejiBelirleme[] stratejiler, string amacKod, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRStratejiBelirleme.Kaydet(null, kullanan, stratejiler, amacKod, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc StratejiBelirlemeSil(Kullanici kullanan, StratejiBelirleme strateji, string amacKod, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRStratejiBelirleme.Sil(null, kullanan, strateji, amacKod, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public StratejiBelirleme[] StratejiBelirlemeListele(Kullanici kullanan, StratejiBelirleme strateji)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRStratejiBelirleme.Listele(null, kullanan, strateji);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public StratejiBelirleme[] StratejiBelirlemeSorgula(Kullanici kullanan, StratejiBelirleme strateji)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRStratejiBelirleme.Sorgula(null, kullanan, strateji);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc BirimStratejikHedefMaliyetKaydet(Kullanici kullanan, BirimStratejikHedefMaliyet[] maliyetler, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRBirimStratejikHedefMaliyet.Kaydet(null, kullanan, maliyetler, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc BirimStratejikHedefMaliyetSil(Kullanici kullanan, BirimStratejikHedefMaliyet maliyet, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRBirimStratejikHedefMaliyet.Sil(null, kullanan, maliyet, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public BirimStratejikHedefMaliyet[] BirimStratejikHedefMaliyetListele(Kullanici kullanan, BirimStratejikHedefMaliyet maliyet)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimStratejikHedefMaliyet.Listele(null, kullanan, maliyet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public BirimStratejikHedefMaliyet[] BirimStratejikHedefMaliyetSorgula(Kullanici kullanan, BirimStratejikHedefMaliyet maliyet)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimStratejikHedefMaliyet.Sorgula(null, kullanan, maliyet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public BirimStratejikHedefMaliyetKatman[] BirimStratejikHedefMaliyetRapor(Kullanici kullanan, BirimStratejikHedefMaliyet maliyet)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimStratejikHedefMaliyet.Raporla(null, kullanan, maliyet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Hizmet veya Üst Belge'nin seçili olma durumunu kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen birimler nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="birim_SpGenelHavuzlar">Hizmet veya Üst Belge bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc BirimGenelHavuzKaydet(Kullanici kullanan, Birim_SpGenelHavuz[] birim_SpGenelHavuzlar, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.BirimGenelHavuz.Kaydet(null, kullanan, birim_SpGenelHavuzlar, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Hizmet veya Üst Belge silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen birim nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="birim_SpGenelHavuz">Hizmet veya Üst Belge bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc BirimGenelHavuzSil(Kullanici kullanan, Birim_SpGenelHavuz birim_SpGenelHavuz, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.BirimGenelHavuz.Sil(null, kullanan, birim_SpGenelHavuz, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan Hizmet veya Üst Belge ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="birim_SpGenelHavuz">Hizmet veya Üst Belge bilgisini tutan nesne</param>
        /// <returns>Bir Hizmet veya Üst Belge için Hizmet veya Üst Belge ayrıntılarını döndürür</returns>
        public Birim_SpGenelHavuz[] BirimGenelHavuzListele(Kullanici kullanan, Birim_SpGenelHavuz birim_SpGenelHavuz)
        {
            try
            {
                return TNS.PEB.IsKurallari.BirimGenelHavuz.Listele(null, kullanan, birim_SpGenelHavuz);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Birim_SpGenelHavuz[] BirimGenelHavuzAgacGetir(Kullanici kullanan, int donemNo, int orgBirimKod, EnumSPGenelHavuzTur girisNoktasi, EnumSPGenelHavuzTur cikisNoktasi)
        {
            try
            {
                return TNS.PEB.IsKurallari.BirimGenelHavuz.AgacGetir(null, kullanan, donemNo, orgBirimKod, girisNoktasi, cikisNoktasi);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Birim_SpGenelHavuz[] BirimGenelHavuzSorgula(Kullanici kullanan, Birim_SpGenelHavuz birim_SpGenelHavuz)
        {
            try
            {
                return TNS.PEB.IsKurallari.BirimGenelHavuz.Sorgula(null, kullanan, birim_SpGenelHavuz);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen gzft nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gzft">Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc GzftKaydet(Kullanici kullanan, Gzft gzft, string formId, bool sadeceAciklama)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRGzft.Kaydet(null, kullanan, gzft, formId, sadeceAciklama);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen gzft nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gzft">Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc GzftSil(Kullanici kullanan, Gzft gzft, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRGzft.Sil(null, kullanan, gzft, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) ayrıntısını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gzft">Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) bilgisini tutan nesne</param>
        /// <returns>Bir Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) için Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) ayrıntılarını döndürür</returns>
        public Gzft GzftTekListele(Kullanici kullanan, Gzft gzft)
        {
            Gzft donusDeger = new Gzft();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRGzft.ListeleTek(null, kullanan, gzft);
            }
            catch (Exception e)
            {
                throw e;
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gzft">Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) bilgisini tutan nesne</param>
        /// <returns>Bir Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) için Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) ayrıntılarını döndürür</returns>
        public Gzft[] GzftListele(Kullanici kullanan, Gzft gzft, bool detay)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRGzft.Listele(null, kullanan, gzft, detay);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Gzft[] GzftSorgula(Kullanici kullanan, Gzft gzft)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRGzft.Sorgula(null, kullanan, gzft);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc AsamaKontrolKaydet(Kullanici kullanan, AsamaKontrol asamaKontrol, ENUMAsamaKontrolIslemTur islem)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRAsamaKontrol.Kaydet(null, kullanan, asamaKontrol, islem);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public AsamaKontrol[] AsamaKontrolListele(Kullanici kullanan, AsamaKontrol asamaKontrol)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRAsamaKontrol.Listele(null, kullanan, asamaKontrol);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public AsamaKontrol[] AsamaKontrolSorgula(Kullanici kullanan, AsamaKontrol asamaKontrol)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRAsamaKontrol.Sorgula(null, kullanan, asamaKontrol);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc AsamaKontrolKaydetGecmis(Kullanici kullanan, AsamaKontrolGecmis asamaKontrolGecmis, ENUMAsamaKontrolIslemTur islem)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRAsamaKontrol.KaydetGecmis(null, kullanan, asamaKontrolGecmis, islem);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public AsamaKontrolGecmis[] AsamaKontrolListeleGecmis(Kullanici kullanan, AsamaKontrolGecmis asamaKontrolGecmis)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRAsamaKontrol.ListeleGecmis(null, kullanan, asamaKontrolGecmis);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen pest nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pest">Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PestKaydet(Kullanici kullanan, Pest pest, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRPest.Kaydet(null, kullanan, pest, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen pest nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pest">Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PestSil(Kullanici kullanan, Pest pest, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRPest.Sil(null, kullanan, pest, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="pest">Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) bilgisini tutan nesne</param>
        /// <returns>Bir Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) için Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) ayrıntılarını döndürür</returns>
        public Pest[] PestListele(Kullanici kullanan, Pest pest, bool detay)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRPest.Listele(null, kullanan, pest, detay);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Pest[] PestSorgula(Kullanici kullanan, Pest pest)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRPest.Sorgula(null, kullanan, pest);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Paydaş varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen paydasAnaliz nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="paydasAnaliz">Paydaş bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PaydasAnalizKaydet(Kullanici kullanan, PaydasAnaliz paydasAnaliz, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRPaydasAnaliz.Kaydet(null, kullanan, paydasAnaliz, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Paydaş silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen paydasAnaliz nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="paydasAnaliz">Paydaş bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PaydasAnalizSil(Kullanici kullanan, PaydasAnaliz paydasAnaliz, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRPaydasAnaliz.Sil(null, kullanan, paydasAnaliz, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan Paydaş ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="paydasAnaliz">Paydaş bilgisini tutan nesne</param>
        /// <returns>Bir Paydaş için Paydaş ayrıntılarını döndürür</returns>
        public PaydasAnaliz[] PaydasAnalizListele(Kullanici kullanan, PaydasAnaliz paydasAnaliz, bool detay)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRPaydasAnaliz.Listele(null, kullanan, paydasAnaliz, detay);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public PaydasAnaliz[] PaydasAnalizSorgula(Kullanici kullanan, PaydasAnaliz paydasAnaliz)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRPaydasAnaliz.Sorgula(null, kullanan, paydasAnaliz);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Planlanan Çalışmalar varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen planlanan nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="planlanan">Planlanan Çalışmalar bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PlanlananKaydet(Kullanici kullanan, Planlanan planlanan, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRPlanlanan.Kaydet(null, kullanan, planlanan, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Planlanan Çalışmaları silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen planlanan nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="planlanan">Planlanan Çalışmalar bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PlanlananSil(Kullanici kullanan, Planlanan planlanan, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRPlanlanan.Sil(null, kullanan, planlanan, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan Planlanan Çalışmalar ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="planlanan">Planlanan Çalışmalar bilgisini tutan nesne</param>
        /// <returns>Bir Planlanan Çalışmalar için Planlanan Çalışmalar ayrıntılarını döndürür</returns>
        public Planlanan[] PlanlananListele(Kullanici kullanan, Planlanan planlanan)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRPlanlanan.Listele(null, kullanan, planlanan);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Planlanan[] PlanlananSorgula(Kullanici kullanan, Planlanan planlanan)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRPlanlanan.Sorgula(null, kullanan, planlanan);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Hizmet-Paydaş ilişkisi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen bhpiler nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="bhpiler">Hizmet-Paydaş ilişkisi bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc BirimHizmetPaydasIliskiKaydet(Kullanici kullanan, Birim_SpGenelHavuz_Paydas_Iliski[] bhpiler, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRBirimHizmetPaydasIliski.Kaydet(null, kullanan, bhpiler, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Hizmet-Paydaş ilişkisini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen bhpi nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="bhpi">Hizmet-Paydaş ilişkisi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc BirimHizmetPaydasIliskiSil(Kullanici kullanan, Birim_SpGenelHavuz_Paydas_Iliski bhpi, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRBirimHizmetPaydasIliski.Sil(null, kullanan, bhpi, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan Hizmet-Paydaş ilişkisi ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="bhpi">Hizmet-Paydaş ilişkisi bilgisini tutan nesne</param>
        /// <returns>Bir Hizmet-Paydaş ilişkisi için Hizmet-Paydaş ilişkisi ayrıntılarını döndürür</returns>
        public Birim_SpGenelHavuz_Paydas_Iliski[] BirimHizmetPaydasIliskiListele(Kullanici kullanan, Birim_SpGenelHavuz_Paydas_Iliski bhpi)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimHizmetPaydasIliski.Listele(null, kullanan, bhpi);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Birim_SpGenelHavuz_Paydas_Iliski[] BirimHizmetPaydasIliskiSorgula(Kullanici kullanan, Birim_SpGenelHavuz_Paydas_Iliski bhpi)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimHizmetPaydasIliski.Sorgula(null, kullanan, bhpi);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public BirimHedef[] BirimHedefListele(Kullanici kullanan, BirimHedef kriter)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimHedef.Listele(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public BirimHedef[] BirimHedefSorgula(Kullanici kullanan, BirimHedef kriter)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimHedef.Sorgula(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc BirimHedefKaydet(Kullanici kullanan, BirimHedef bilgi, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = STRBirimHedef.Kaydet(null, kullanan, bilgi, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public BirimHedefKoordineBirim[] BirimHedefKoordineBirimListele(Kullanici kullanan, BirimHedefKoordineBirim kriter)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimHedefKoordineBirim.Listele(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc BirimHedefKoordineBirimKaydet(Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, ObjectArray bilgi, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = STRBirimHedefKoordineBirim.Kaydet(null, kullanan, donemNo, orgBirimKod, hedefKod, bilgi, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public BirimHedefUstHedef[] BirimHedefUstHedefListele(Kullanici kullanan, BirimHedefUstHedef kriter)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimHedefUstHedef.Listele(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc BirimHedefUstHedefKaydet(Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, ObjectArray bilgi, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = STRBirimHedefUstHedef.Kaydet(null, kullanan, donemNo, orgBirimKod, hedefKod, bilgi, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public BirimHedefSorunAlan[] BirimHedefSorunAlanListele(Kullanici kullanan, BirimHedefSorunAlan kriter)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimHedefSorunAlan.Listele(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc BirimHedefSorunAlanKaydet(Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, ObjectArray bilgi, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = STRBirimHedefSorunAlan.Kaydet(null, kullanan, donemNo, orgBirimKod, hedefKod, bilgi, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public BirimHedefUstPolitika[] BirimHedefUstPolitikaListele(Kullanici kullanan, BirimHedefUstPolitika kriter)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimHedefUstPolitika.Listele(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Sonuc BirimHedefUstPolitikaKaydet(Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, ObjectArray bilgi, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = STRBirimHedefUstPolitika.Kaydet(null, kullanan, donemNo, orgBirimKod, hedefKod, bilgi, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public BirimHedefGzft[] BirimHedefGzftListele(Kullanici kullanan, BirimHedefGzft kriter)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimHedefGzft.Listele(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc BirimHedefGzftKaydet(Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, ObjectArray bilgi, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = STRBirimHedefGzft.Kaydet(null, kullanan, donemNo, orgBirimKod, hedefKod, bilgi, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc MevzuatAnalizKaydet(Kullanici kullanan, MevzuatAnaliz mevzuat, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRMevzuatAnaliz.Kaydet(null, kullanan, mevzuat, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc MevzuatAnalizSil(Kullanici kullanan, MevzuatAnaliz mevzuat, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRMevzuatAnaliz.Sil(null, kullanan, mevzuat, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public MevzuatAnaliz MevzuatAnalizListele(Kullanici kullanan, MevzuatAnaliz mevzuat)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRMevzuatAnaliz.Listele(null, kullanan, mevzuat);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public MevzuatAnaliz[] MevzuatAnalizSorgula(Kullanici kullanan, MevzuatAnaliz mevzuat)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRMevzuatAnaliz.Sorgula(null, kullanan, mevzuat);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc TarihselGelisimKaydet(Kullanici kullanan, TarihselGelisim tg, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRTarihselGelisim.Kaydet(null, kullanan, tg, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc TarihselGelisimSil(Kullanici kullanan, TarihselGelisim tg, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRTarihselGelisim.Sil(null, kullanan, tg, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public TarihselGelisim TarihselGelisimListele(Kullanici kullanan, TarihselGelisim tg)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRTarihselGelisim.Listele(null, kullanan, tg);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public TarihselGelisim[] TarihselGelisimSorgula(Kullanici kullanan, TarihselGelisim tg)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRTarihselGelisim.Sorgula(null, kullanan, tg);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc HarcamaHesaplaKaydet(Kullanici kullanan, Harcama hk)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.PEBHarcama.HarcamaHesaplaKaydet(null, kullanan, hk);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc PerformansHamVeriKaydet(Kullanici kullanan, PerformansHamVeri phVeri)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRPerformansHamVeri.Kaydet(null, kullanan, phVeri);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public PerformansHamVeri[] PerformansHamVeriListele(Kullanici kullanan, PerformansHamVeri phVeri)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRPerformansHamVeri.Listele(null, kullanan, phVeri);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Daire[] KullanilanDaireBul(bool sadeceEskiKayitlar)
        {
            try
            {
                return TNS.PEB.IsKurallari.PEBDaire.KullanilanDaireListele(null, sadeceEskiKayitlar);
            }
            catch
            {
                return null;
            }
        }

        public Sonuc DaireyiOrgaCevir(Kullanici kullanan, int yil, string kurKod, int daireKod, int orgKod)
        {
            try
            {
                return PEBDaire.DaireyiOrgaCevir(null, kullanan, yil, kurKod, daireKod, orgKod);
            }
            catch
            {
                return null;
            }
        }

        public string AyniNumaraliFPSMListele()
        {
            return PEBDaire.AyniNumaraliFPSMListele(null);
        }

        public void AyniNumaraliFPSMDuzelt(Kullanici kullanan, string faalSmKod)
        {
            PEBDaire.AyniNumaraliFPSMDuzelt(null, kullanan, faalSmKod);
        }

        //public ObjectArray HarcamaAgirlikHesapla(Kullanici kullanan, Harcama hk)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = TNS.PEB.IsKurallari.PEBHarcama.AgirlikHesapla(null, kullanan, hk);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        //public Sonuc HarcamaKaydet(Kullanici kullanan, Harcama hk, TNSCollection agirliklar)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = TNS.PEB.IsKurallari.PEBHarcama.Kaydet(null, kullanan, hk, agirliklar);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        //public ObjectArray HarcamaListele(Kullanici kullanan, Harcama hk)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = TNS.PEB.IsKurallari.PEBHarcama.Listele(null, kullanan, hk);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        //public Sonuc AsamaKopyala(Kullanici kullanan, AsamaKopyala bilgi)
        //{
        //    Sonuc donusDeger = new Sonuc();
        //    try
        //    {
        //        donusDeger = TNS.PEB.IsKurallari.PEBAsama.Kopyala(kullanan, bilgi);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.islemSonuc = false;
        //        donusDeger.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        //public ObjectArray FarkliTertipDuzenleriListele(Kullanici kullanan, FaaliyetProjeOdenek fpOdenek)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = TNS.PEB.IsKurallari.PEBFaaliyetProjeOdenek.FarkliTertipListele(null, kullanan, fpOdenek);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan ölçü birim bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="olcuBirim">Ölçü birim bilgisini tutan nesne</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ölçü birim listesi döndürülür.</returns>
        //public ObjectArray OlcuBirimListeleEskiDB(Kullanici kullanan, OlcuBirim olcuBirim)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = TNS.PEB.IsKurallari.PEBOlcuBirim.ListeleEskiDB(null, kullanan, olcuBirim);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan performans gösterge bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="gosterge">Performans gösterge bilgisini tutan nesne</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans gösterge listesi döndürülür.</returns>
        //public ObjectArray PerformansGostergeListeleEskiDB(Kullanici kullanan, PerformansGosterge gosterge)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = TNS.PEB.IsKurallari.PEBGosterge.ListeleEskiDB(null, kullanan, gosterge);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        ///// <summary>
        ///// Verilen kriterlere uygun olan performans hedef bilgilerini döndüren yordam
        ///// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="pHedef">Performans hedef bilgisini tutan nesne</param>
        ///// <param name="detayAgirlik">Performans hedefin stratejik hedeflerle bağının ve ağırlık bilgilerinin istenip istenmediği bilgisi</param>
        ///// <param name="donemBYil">Dönem başlangıç yılı</param>
        ///// <param name="strHedefKod">Stratejik hedef programatik kodu</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef listesi döndürülür.</returns>
        //public ObjectArray PerformansHedefListeleEskiDB(Kullanici kullanan, PerformansHedef pHedef, bool detayAgirlik, int donemBYil, int strHedefKod)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = TNS.PEB.IsKurallari.PEBPerHedef.ListeleEskiDB(null, kullanan, pHedef, detayAgirlik, donemBYil, strHedefKod);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        ///// <summary>
        ///// Verilen kriterlere uyan performans göstergelerini, hedeflenen ve gerçekleşen değerlerini,
        ///// bağlı olduğu performans hedeflerini ve stratejik hedefleri toparlayıp döndüren yordam
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="kurKod">Birim kodu</param>
        ///// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında BirimGostergeleriRaporu nesneleri listesi döndürülür.</returns>
        //public ObjectArray BirimGostergeleriRaporlaEskiDB(Kullanici kullanan, int yil, string kurKod, int[] gostergeSeviye)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.BirimGostergeleriRaporlaEskiDB(null, kullanan, yil, kurKod, gostergeSeviye);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        ///// <summary>
        ///// Verilen performans hedefi ve performans göstergesi kriterlerine uyan performans
        ///// göstergesinin hedeflenen ve gerçekleşen değerlerini toparlayıp döndüren yordam
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="pHedef">Performans hedef bilgilerini tutan nesne</param>
        ///// <param name="pGosterge">Performans gösterge bilgilerini tutan nesne</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında performans hedef nesnesi döndürülür.</returns>
        //public ObjectArray PerformansGostergeDetayRaporlaEskiDB(Kullanici kullanan, PerformansHedef pHedef, PerformansGosterge pGosterge)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.PerformansGostergeDetayRaporlaEskiDB(null, kullanan, pHedef, pGosterge);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        ///// <summary>
        ///// Verilen kriterlere uyan performan göstergelerini, hedeflenen ve gerçekleşen değerlerini toplayıp döndüren yordam
        ///// </summary>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="kurKod">Birim kodu</param>
        ///// <param name="gostergeSeviye">Listelenmek istenen gösterge seviyeleri dizisi</param>
        ///// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında PerformansGostergeGerceklesen nesneleri listesi döndürülür.</returns>
        //public ObjectArray PerformansGostergeRaporlaEskiDB(Kullanici kullanan, int yil, string kurKod, int[] gostergeSeviye)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    try
        //    {
        //        donusDeger = TNS.PEB.IsKurallari.PEBRaporlar.PerformansGostergeRaporlaEskiDB(null, kullanan, yil, kurKod, gostergeSeviye);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.ToString();
        //    }
        //    return donusDeger;
        //}

        public Sonuc HamVeriToplamAl(Kullanici kullanan, int yil, int IlceIlUlke, string ilceKodIlKod)
        {
            return TNS.PEB.IsKurallari.STRPerformansHamVeri.ToplamAl(null, kullanan, yil, IlceIlUlke, ilceKodIlKod);
        }

        ///// <summary>
        ///// LPUretim nesnesini veri tabanından okuyan fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">LPUretim yapısında LPUretim: bilgilerini tutan değişken</param>
        ///// <param name="detayOku">Okunan formların değişken bilgileri de okunacak mı?</param>
        //public LPUretim[] LPUretimListele(Kullanici kullanan, LPUretim form, bool detayOku)
        //{
        //    return PEBLPUret.Listele(null, kullanan, form, detayOku);
        //}

        ///// <summary>
        ///// LPUretim nesnesini veri tabanına yazan fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">LPUretim yapısında LPUretim: bilgilerini tutan değişken</param>
        ///// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        //public Sonuc LPUretimKaydet(Kullanici kullanan, LPUretim form)
        //{
        //    try
        //    {
        //        return PEBLPUret.Kaydet(null, kullanan, form, false);
        //    }
        //    catch (Exception e)
        //    {
        //        return new Sonuc(false, e.ToString(), "");
        //    }
        //}

        //public Sonuc LPUretimSil(Kullanici kullanan, LPUretim[] form)
        //{
        //    try
        //    {
        //        return PEBLPUret.Sil(null, kullanan, form);
        //    }
        //    catch (Exception e)
        //    {
        //        return new Sonuc(false, e.ToString(), "");
        //    }
        //}

        //public Sonuc LPUretimeAl(Kullanici kullanan, LPUretim[] form)
        //{
        //    try
        //    {
        //        return PEBLPUret.LPUretimeAl(null, kullanan, form);
        //    }
        //    catch (Exception e)
        //    {
        //        return new Sonuc(false, e.ToString(), "");
        //    }
        //}

        //public void LPCalistir(Kullanici kullanan, int yil)
        //{
        //    PEBLPUret.LPCalistir(null, kullanan, yil);
        //}

        //public Sonuc LPKontrolVerisiYukle(Kullanici kullanan, LPUretim form, string xmlVeri)
        //{
        //    return PEBLPUret.LPKontrolVerisiYukle(kullanan, form, xmlVeri);
        //}

        public Sonuc BirimAmacHedefOtomatikUret(Kullanici kullanan, int donemNo, int orgBirimKod, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = STRBirimAmacHedef.OtomatikUret(null, kullanan, donemNo, orgBirimKod, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc BirimAmacHedefKaydet(Kullanici kullanan, Birim_AmacHedef bah, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = STRBirimAmacHedef.Kaydet(null, kullanan, bah, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc BirimAmacHedefSil(Kullanici kullanan, Birim_AmacHedef bah, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = STRBirimAmacHedef.Sil(null, kullanan, bah, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Birim_AmacHedef[] BirimAmacHedefListele(Kullanici kullanan, Birim_AmacHedef bah, bool detay, bool amac)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimAmacHedef.Listele(null, kullanan, bah, detay, amac);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Birim_AmacHedef[] BirimAmacHedefSorgula(Kullanici kullanan, Birim_AmacHedef bah, bool amac)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimAmacHedef.Sorgula(null, kullanan, bah, amac);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc BirimFaaliyetMaliyetKaydet(Kullanici kullanan, BirimFaaliyetMaliyet[] maliyetler, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRBirimFaaliyetMaliyet.Kaydet(null, kullanan, maliyetler, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc BirimFaaliyetMaliyetSil(Kullanici kullanan, BirimFaaliyetMaliyet maliyet, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRBirimFaaliyetMaliyet.Sil(null, kullanan, maliyet, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public BirimFaaliyetMaliyet[] BirimFaaliyetMaliyetListele(Kullanici kullanan, BirimFaaliyetMaliyet maliyet)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimFaaliyetMaliyet.Listele(null, kullanan, maliyet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public BirimFaaliyetMaliyet[] BirimFaaliyetMaliyetSorgula(Kullanici kullanan, BirimFaaliyetMaliyet maliyet)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimFaaliyetMaliyet.Sorgula(null, kullanan, maliyet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc RaporSablonKaydet(Kullanici kullanan, int sablonNo, RTFOutline[] rtfOutlineSatirlar)
        {
            try
            {
                return TNS.PEB.IsKurallari.RTFRaporYonetim.Kaydet(null, kullanan, sablonNo, rtfOutlineSatirlar);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.Message, "");
            }
        }

        public RTFOutline[] RaporSablonListele(Kullanici kullanan, int sablonNo, int merkezTasra)
        {
            return TNS.PEB.IsKurallari.RTFRaporYonetim.Listele(null, kullanan, sablonNo, merkezTasra);
        }

        public Sonuc RaporSablonTurDegistir(Kullanici kullanan, int sablonNo, RTFOutline rtfOutline)
        {
            return TNS.PEB.IsKurallari.RTFRaporYonetim.TurDegistir(null, kullanan, sablonNo, rtfOutline);
        }


        public RTFOutline RaporSablonTurOku(Kullanici kullanan, int sablonNo, int kod)
        {
            return TNS.PEB.IsKurallari.RTFRaporYonetim.TurOku(null, kullanan, sablonNo, kod);
        }

        public string RaporDetayOku(Kullanici kullanan, int sablonNo, int kod, int donemNo, int birimKod, int yil)
        {
            return TNS.PEB.IsKurallari.RTFRaporYonetim.RaporDetayOku(null, kullanan, sablonNo, kod, donemNo, birimKod, yil, true);
        }

        public Sonuc RaporDetayYaz(Kullanici kullanan, int sablonNo, int kod, int donemNo, int birimKod, int yil, string htmlMetin)
        {
            return TNS.PEB.IsKurallari.RTFRaporYonetim.RaporDetayYaz(null, kullanan, sablonNo, kod, donemNo, birimKod, yil, htmlMetin);
        }

        public Sonuc BirimHedefFaaliyetKaydet(Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, TNSCollection faaliyetler, string formId)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return STRBirimHedefFaaliyet.Kaydet(null, kullanan, donemNo, orgBirimKod, hedefKod, faaliyetler, formId);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();

                return donusDeger;
            }
        }

        public BirimHedefFaaliyet[] BirimHedefFaaliyetListele(Kullanici kullanan, BirimHedefFaaliyet kriter)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimHedefFaaliyet.Listele(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc BirimFaaliyetAgirlikKaydet(Kullanici kullanan, BirimFaaliyetAgirlik[] agirliklar, string formId)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimFaaliyetAgirlik.Kaydet(null, kullanan, agirliklar, formId);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();

                return donusDeger;
            }
        }

        public Sonuc BirimFaaliyetAgirlikSil(Kullanici kullanan, BirimFaaliyetAgirlik agirlik, string formId)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimFaaliyetAgirlik.Sil(null, kullanan, agirlik, formId);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();

                return donusDeger;
            }
        }

        public BirimFaaliyetAgirlik[] BirimFaaliyetAgirlikListele(Kullanici kullanan, BirimFaaliyetAgirlik agirlik)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimFaaliyetAgirlik.Listele(null, kullanan, agirlik, false);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc STRGostergeKaydet(Kullanici kullanan, STRGosterge gosterge)
        {
            try
            {
                return STRSTRGosterge.Kaydet(null, kullanan, gosterge);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc STRGostergeSil(Kullanici kullanan, STRGosterge gosterge)
        {
            try
            {
                return STRSTRGosterge.Sil(null, kullanan, gosterge);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public STRGosterge[] STRGostergeListele(Kullanici kullanan, STRGosterge gosterge, bool detay, params string[] bagliKodlar)
        {
            try
            {
                return STRSTRGosterge.Listele(null, kullanan, gosterge, detay, bagliKodlar);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc STRGostergeFormulDerlenebilirMi(string formul)
        {
            try
            {
                return STRSTRGosterge.FormulDerlenebilirMi(formul);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }
        public Sonuc BirimHedefGostergeKaydet(Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, TNSCollection gostergeler, string formId)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return STRBirimHedefGosterge.Kaydet(null, kullanan, donemNo, orgBirimKod, hedefKod, gostergeler, formId);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();

                return donusDeger;
            }
        }

        public BirimHedefGosterge[] BirimHedefGostergeListele(Kullanici kullanan, BirimHedefGosterge kriter)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimHedefGosterge.Listele(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DegiskenSablon[] VZADegiskenSablonListele(Kullanici kullanan)
        {
            return PEBVeriZarflama.DegiskenSablonListele(null, kullanan);
        }

        public DegiskenSablon VZADegiskenSablonTanimEkle(Kullanici kullanan)
        {
            return PEBVeriZarflama.DegiskenSablonTanimEkle(null, kullanan);
        }

        public Sonuc VZADegiskenSablonAdDegistir(Kullanici kullanan, int kod, string metin)
        {
            return PEBVeriZarflama.DegiskenSablonAdDegistir(null, kullanan, kod, metin);
        }

        public Sonuc VZADegiskenSablonSil(Kullanici kullanan, int kod)
        {
            return PEBVeriZarflama.DegiskenSablonSil(null, kullanan, kod);
        }


        public DegiskenSablon VZADegiskenSablonDetayOku(Kullanici kullanan, int kod)
        {
            return PEBVeriZarflama.DegiskenSablonDetayOku(null, kullanan, kod);
        }

        public Sonuc VZADegiskenSablonDetayYaz(Kullanici kullanan, DegiskenSablon d)
        {
            return PEBVeriZarflama.DegiskenSablonDetayYaz(null, kullanan, d);
        }

        //public KarsilastirmaGrubu[] VZAKarsilastirmaGruplariListele(Kullanici kullanan)
        //{
        //    return PEBVeriZarflama.KarsilastirmaGruplariListele(null, kullanan);
        //}

        //public Sonuc VZAKarsilastirmaGrubuAdDegistir(Kullanici kullanan, int kod, string metin)
        //{
        //    return PEBVeriZarflama.KarsilastirmaGrubuAdDegistir(null, kullanan, kod, metin);
        //}

        //public Sonuc VZAKarsilastirmaGrubuSil(Kullanici kullanan, int kod)
        //{
        //    return PEBVeriZarflama.KarsilastirmaGrubuSil(null, kullanan, kod);
        //}

        //public KarsilastirmaGrubu VZAKarsilastirmaGrubuTanimEkle(Kullanici kullanan)
        //{
        //    return PEBVeriZarflama.KarsilastirmaGrubuTanimEkle(null, kullanan);
        //}

        //public KarsilastirmaGrubu VZAKarsilastirmaGrubuDetayOku(Kullanici kullanan, int kod)
        //{
        //    return PEBVeriZarflama.KarsilastirmaGrubuDetayOku(null, kullanan, kod);
        //}

        //public Sonuc VZAKarsilastirmaGrubuDetayYaz(Kullanici kullanan, KarsilastirmaGrubu d)
        //{
        //    return PEBVeriZarflama.KarsilastirmaGrubuDetayYaz(null, kullanan, d);
        //}

        public Sonuc BirimIsGorevMaliyetKaydet(Kullanici kullanan, BirimIsGorevMaliyet[] maliyetler, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRBirimIsGorevMaliyet.Kaydet(null, kullanan, maliyetler, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc BirimIsGorevMaliyetSil(Kullanici kullanan, BirimIsGorevMaliyet maliyet, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRBirimIsGorevMaliyet.Sil(null, kullanan, maliyet, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public BirimIsGorevMaliyet[] BirimIsGorevMaliyetListele(Kullanici kullanan, BirimIsGorevMaliyet maliyet)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimIsGorevMaliyet.Listele(null, kullanan, maliyet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public BirimIsGorevMaliyet[] BirimIsGorevMaliyetSorgula(Kullanici kullanan, BirimIsGorevMaliyet maliyet)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimIsGorevMaliyet.Sorgula(null, kullanan, maliyet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc BirimHedefGostergeYillikKaydet(Kullanici kullanan, BirimHedefGostergeYillik[] gostergeler, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRBirimHedefGostergeYillik.Kaydet(null, kullanan, gostergeler, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc BirimHedefGostergeYillikSil(Kullanici kullanan, BirimHedefGostergeYillik gosterge, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRBirimHedefGostergeYillik.Sil(null, kullanan, gosterge, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public BirimHedefGostergeYillik[] BirimHedefGostergeYillikListele(Kullanici kullanan, BirimHedefGostergeYillik gosterge)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimHedefGostergeYillik.Listele(null, kullanan, gosterge);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public BirimHedefGostergeYillik[] BirimHedefGostergeYillikSorgula(Kullanici kullanan, BirimHedefGostergeYillik gosterge)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimHedefGostergeYillik.Sorgula(null, kullanan, gosterge);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public VZAAnalizGrup[] VZAAnalizGrupDetayListele(Kullanici kullanan)
        {
            return PEBVeriZarflama.AnalizGrupDetayListele(null, kullanan);
        }

        public VZAAnalizGrup VZAAnalizGrupTanimEkle(Kullanici kullanan)
        {
            return PEBVeriZarflama.AnalizGrupTanimEkle(null, kullanan);
        }

        public Sonuc VZAAnalizGrupSil(Kullanici kullanan, int grupKod, int altKod)
        {
            return PEBVeriZarflama.AnalizGrupSil(null, kullanan, grupKod, altKod);
        }

        public Sonuc VZAAnalizGrupAdDegistir(Kullanici kullanan, int kod, int altKod, string metin)
        {
            return PEBVeriZarflama.VZAAnalizGrupAdDegistir(null, kullanan, kod, altKod, metin);
        }

        public Sonuc VZAAnalizDetayTanimEkle(Kullanici kullanan, int grupKod, VZAAnalizDetay d)
        {
            return PEBVeriZarflama.VZAAnalizDetayTanimEkle(null, kullanan, grupKod, d);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="islemYil"></param>
        /// <param name="grupKod"></param>
        /// <param name="altKod"></param>
        /// <param name="listelenecekler">0: hepsi, 1: bekleyenler, 2: işleme alınmışlar (Hepsi), 3: başarılılar, 4: başarısızlar</param>
        /// <returns></returns>
        public VZAAnalizDetay VZAIslemDurumListele(Kullanici kullanan, int islemYil, int grupKod, int altKod, int listelenecekler)
        {
            return PEBVeriZarflama.VZAIslemDurumListele(null, kullanan, islemYil, grupKod, altKod, listelenecekler);
        }

        public Sonuc VZAIslemeAl(Kullanici kullanan, int islemYil, int grupKod, int altKod)
        {
            return PEBVeriZarflama.VZAIslemeAl(null, kullanan, islemYil, grupKod, altKod);
        }

        public Sonuc VZALPIslem(Kullanici kullanan, int islemYil, int grupKod, int altKod)
        {
            return PEBVeriZarflama.VZALPIslem(null, kullanan, islemYil, grupKod, altKod);
        }

        public VZASonucBilgi VZASonucDondur(Kullanici kullanan, int islemYil, int grupKod, int altKod, string cogKod)
        {
            return PEBVeriZarflama.VZASonucDondur(null, kullanan, islemYil, grupKod, altKod, cogKod);
        }

        public VZASonucBirim VZASonucDondur(Kullanici kullanan, int islemYil, int orgKod)
        {
            return PEBVeriZarflama.VZASonucDondur(null, kullanan, islemYil, orgKod);
        }

        public Sonuc GostergeVeriKaynagindanCek(Kullanici kullanan, int orgBirimKod, int yil)
        {
            return STRSTRGosterge.GostergeVeriKaynagindanCek(null, kullanan, orgBirimKod, yil);
        }

        public Decimal VeriKaynagiDegerBul(Kullanici kullanan, int yil, int orgKod, STRGosterge gosterge)
        {
            return STRSTRGosterge.VeriKaynagiDegerBul(kullanan, null, TNS.UZY.Arac.Tanimla(), yil, orgKod, gosterge);
        }

        public Decimal VeriKaynagiDegerBulCografi(Kullanici kullanan, int yil, string cografiTur, int orgOrCogKod, STRGosterge gosterge)
        {
            return STRSTRGosterge.VeriKaynagiDegerBulCografi(kullanan, null, TNS.UZY.Arac.Tanimla(), yil, cografiTur, orgOrCogKod, gosterge);
        }

        public Sonuc GostergeHesapla(Kullanici kullanan, int orgBirimKod, int yil)
        {
            return STRSTRGosterge.GostergeHesapla(null, kullanan, orgBirimKod, yil);
        }

        public Sonuc BirimGostergeGerceklesenKaydet(Kullanici kullanan, BirimGostergeGerceklesen[] gostergeler, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRBirimGostergeGerceklesen.Kaydet(null, kullanan, gostergeler, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc BirimGostergeGerceklesenSil(Kullanici kullanan, BirimGostergeGerceklesen gosterge, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRBirimGostergeGerceklesen.Sil(null, kullanan, gosterge, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public BirimGostergeGerceklesen[] BirimGostergeGerceklesenListele(Kullanici kullanan, BirimGostergeGerceklesen gosterge)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimGostergeGerceklesen.Listele(null, kullanan, gosterge);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public BirimGostergeGerceklesmeDurum BirimGostergeGerceklesmeDurumBul(Kullanici kullanan, int orgBirimKod, int yil, int ay)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimGostergeGerceklesen.GerceklesmeDurumBul(null, kullanan, orgBirimKod, yil, ay);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc BirimPerHedefKaydet(Kullanici kullanan, BirimPerHedef perHedef, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRBirimPerHedef.Kaydet(null, kullanan, perHedef, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc BirimPerHedefSil(Kullanici kullanan, BirimPerHedef perHedef, string formId)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PEB.IsKurallari.STRBirimPerHedef.Sil(null, kullanan, perHedef, formId);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public BirimPerHedef[] BirimPerHedefListele(Kullanici kullanan, BirimPerHedef perHedef)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimPerHedef.Listele(null, kullanan, perHedef);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public BirimPerHedef[] BirimPerHedefSorgula(Kullanici kullanan, BirimPerHedef perHedef)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRBirimPerHedef.Sorgula(null, kullanan, perHedef);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public double VZANotBul(Kullanici kullanan, int yil, int orgKod)
        {
            return PEBVeriZarflama.VZANotBul(null, kullanan, yil, orgKod);
        }

        public Sonuc VZAAnalizGrupVarsayilanYap(Kullanici kullanan, int grupKod, int varsayilan)
        {
            return PEBVeriZarflama.VZAAnalizGrupVarsayilanYap(null, kullanan, grupKod, varsayilan);
        }

        public ObjectArray GostergeGerceklesenHedefGosterge(Kullanici kullanan, int yil, string kurKod, int ay)
        {
            return PEBRaporlar.GostergeGerceklesenHedefGosterge(null, kullanan, yil, kurKod, ay);
        }

        public Sonuc PerformansGostergeHedeflenenKopyala(Kullanici kullanan, int yil, string ghKurKod, int kaynakAsama, int hedefAsama)
        {
            return TNS.PEB.IsKurallari.PEBGostergeHedef.Kopyala(null, kullanan, yil, ghKurKod, kaynakAsama, hedefAsama);
        }

        public Sonuc PerformansKopyala(Kullanici kullanan, int kaynakYil, int hedefYil)
        {
            return TNS.PEB.IsKurallari.PEBPerHedef.PerformansKopyala(null, kullanan, kaynakYil, hedefYil);
        }

        public UcgenSatir[] OdenekListeleFGT(Kullanici kullanan, int yil, string senaryoKod, string kurKod, int ekoUz, bool sifirlarDahil)
        {
            return TNS.PEB.IsKurallari.PEBFaaliyetProjeOdenek.OdenekListeleFGT(null, kullanan, yil, senaryoKod, kurKod, ekoUz, sifirlarDahil);
        }

        public ObjectArray ListeleStrHedefGosterge(Kullanici kullanan, int donemBasYil, int hedefKod, string kurumKod)
        {
            return STRHedef.ListeleStrHedefGosterge(null, kullanan, donemBasYil, hedefKod, kurumKod);
        }

        public Sonuc KaydetStrHedefGosterge(Kullanici kullanan, int donemBasYil, int hedefKod, string kurumKod, TNSCollection gostergeler)
        {
            return STRHedef.KaydetStrHedefGosterge(null, kullanan, donemBasYil, hedefKod, kurumKod, gostergeler);
        }

        public ObjectArray ListeleStrHedefOdenek(Kullanici kullanan, int donemBasYil, int hedefKod, string kurumKod)
        {
            return STRHedef.ListeleStrHedefOdenek(null, kullanan, donemBasYil, hedefKod, kurumKod);
        }

        public Sonuc KaydetStrHedefOdenek(Kullanici kullanan, int donemBasYil, int hedefKod, string kurumKod, TNSCollection odenekler)
        {
            return STRHedef.KaydetStrHedefOdenek(null, kullanan, donemBasYil, hedefKod, kurumKod, odenekler);
        }

        public ObjectArray StratejikFaaliyetListele(Kullanici kullanan, STRFaaliyet form, bool detayli)
        {
            return PEBStratejikFaaliyet.Listele(null, kullanan, form, detayli);
        }

        public Sonuc KaydetStratejikFaaliyet(Kullanici kullanan, STRFaaliyet form)
        {
            return PEBStratejikFaaliyet.Kaydet(null, kullanan, form);
        }

        public ObjectArray ListeleStrHedefGostergeGerc(Kullanici kullanan, StratejikHedef hedef, int yil, int donem)
        {
            return STRHedef.ListeleStrHedefGostergeGerc(null, kullanan, hedef, yil, donem);
        }

        public ObjectArray ListeleStrHedefGercBilgi(Kullanici kullanan, StratejikHedef hedef, int yil, int donem, int bilgiTur)
        {
            return STRHedef.ListeleStrHedefGercBilgi(null, kullanan, hedef, yil, donem, bilgiTur);
        }

        public Sonuc KaydetStrHedefGostergeGerc(Kullanici kullanan, TNSCollection gerceklesmeler, TNSCollection[] gercBilgiler, bool aktarim)
        {
            return STRHedef.KaydetStrHedefGostergeGerc(null, kullanan, gerceklesmeler, gercBilgiler, aktarim);
        }

        public ObjectArray UstPolitikaBelgeleriListele(Kullanici kullanan, int yil)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRHedef.UstPolitikaBelgeleriListele(null, kullanan, yil);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray StratejikHedefRaporAl(Kullanici kullanan, StratejikHedef strHedef, int raporNo)
        {
            return TNS.PEB.IsKurallari.STRHedef.RaporAl(null, kullanan, strHedef, raporNo);
        }

        /// <summary>
        /// KurumGruplari nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KurumGruplari yapısında KurumGruplari: Kurum Grupları bilgilerini tutan değişken</param>
        public ObjectArray ListeleKurumGrup(Kullanici kullanan, KurumGruplari form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return PEBKurumGrup.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        /// <summary>
        /// KurumGruplari nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KurumGruplari yapısında KurumGruplari: Kurum Grupları bilgilerini tutan değişken</param>
        public ObjectVAN OkuKurumGrup(Kullanici kullanan, KurumGruplari form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return PEBKurumGrup.Oku(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// KurumGruplari nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KurumGruplari yapısında KurumGruplari: Kurum Grupları bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetKurumGrup(Kullanici kullanan, KurumGruplari form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return PEBKurumGrup.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc SilKurumGrup(Kullanici kullanan, KurumGruplari form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return PEBKurumGrup.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// EylemPlani nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EylemPlani yapısında EylemPlani: bilgilerini tutan değişken</param>
        /// <param name="yil">Gerçekleşme bilgisinin okunacağı yıl</param>
        /// <param name="ay">Gerçekleşme bilgisinin okunacağı ay</param>
        /// <param name="detayli">Eylemlere ait gerçekleşme bilgileri ve ilgili birimler de okunur</param>
        public ObjectArray ListeleEylemPlaniForm(Kullanici kullanan, EylemPlani form, int yil, int ay, int donemBasYil, bool detayli)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return STREylemPlaniForm.Listele(null, kullanan, form, yil, ay, donemBasYil, detayli);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }


        /// <summary>
        /// EylemPlani nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="eylemID">Gerçekleşmesi okunacak eylemin IDsi</param>
        /// <param name="yil">Gerçekleşmesi okunacak dönemim yılı</param>
        /// <param name="ay">Gerçekleşmesi okunacak dönemim ayı</param>
        public ObjectVAN OkuEylemPlaniGerceklesme(Kullanici kullanan, int eylemID, int yil, int ay)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return STREylemPlaniForm.OkuGerceklesme(null, kullanan, eylemID, yil, ay);
            }
            catch (Exception e)
            {
                ObjectVAN ov = new ObjectVAN();
                ov.sonuc.DegerAta(e.ToString());
                return ov;
            }
        }

        /// <summary>
        /// EylemPlani nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">EylemPlani yapısında EylemPlani:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetEylemPlaniForm(Kullanici kullanan, EylemPlani form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return STREylemPlaniForm.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc EylemPlaniAgirlikGuncelle(Kullanici kullanan, int yil, int eylemID, int agirlik)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return STREylemPlaniForm.AgirlikGuncelle(null, kullanan, yil, eylemID, agirlik);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc KaydetEylemPlaniYetki(Kullanici kullanan, int eylemID, TNSCollection kisiler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return STREylemPlaniForm.KaydetEylemPlaniYetki(null, kullanan, eylemID, kisiler);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray ListeleEylemPlaniYetki(Kullanici kullanan, int eylemID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return STREylemPlaniForm.ListeleEylemPlaniYetki(null, kullanan, eylemID);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public ObjectArray ListeleEylemPlaniDonem(Kullanici kullanan, int eylemTuru, int yil, int ay)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return STREylemPlaniForm.ListeleEylemPlaniDonem(null, kullanan, eylemTuru, yil, ay);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public Sonuc AyarlaEylemPlaniDonem(Kullanici kullanan, int yil, int ay, int eylemID, int donemDurum)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return STREylemPlaniForm.AyarlaEylemPlaniDonem(null, kullanan, yil, ay, eylemID, donemDurum);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc KaydetEylemPlaniGerceklesme(Kullanici kullanan, int eylemID, EylemGerceklesme form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return STREylemPlaniForm.KaydetEylemPlaniGerceklesme(null, kullanan, eylemID, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray ListeleEylemPlaniTarihce(Kullanici kullanan, int eylemID)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return STREylemPlaniForm.ListeleEylemPlaniTarihce(null, kullanan, eylemID);
            }
            catch (Exception e)
            {
                ObjectArray oa = new ObjectArray();
                oa.sonuc.DegerAta(e.ToString());
                return oa;
            }
        }

        public Sonuc SilEylemPlaniForm(Kullanici kullanan, EylemPlani form)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return STREylemPlaniForm.SilEylemPlaniForm(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc MisyonDonemKopyala(Kullanici kullanan, int kaynakYil, int hedefYil)
        {
            try
            {
                return TNS.PEB.IsKurallari.STRMisyon.Kopyala(null, kullanan, kaynakYil, hedefYil);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }
    }
}