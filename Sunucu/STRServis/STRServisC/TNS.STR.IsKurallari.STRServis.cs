using System;
using TNS.UZY;
using TNS.KYM;
using TNS.STR;

namespace TNS.STR.IsKurallari
{
    /// <summary>
    /// ISTRServis arayüzünden türeyen, istemciden gelen çağrıları
    /// ilgili sınıflardaki ilgili yordamlara yönlendiren fonksiyonları tutan sınıf
    /// </summary>
    [Serializable()]
    public class STRServis : MarshalByRefObject, ISTRServis
    {
        /// <summary>
        /// Uzaylar servisine ulaşmak için kullanılan değişken
        /// </summary>
        private static IUZYServis servisUZY = null;

        /// <summary>
        /// Parametresiz Stratejik plan servisi nesnesi oluşturucu yordam
        /// </summary>
        public STRServis()
        {
            if (servisUZY == null)
                servisUZY = TNS.UZY.Arac.Tanimla();
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
        public static double UzaySayiDegerGetir(string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
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
                donusDeger = TNS.STR.IsKurallari.STRDil.Kaydet(null, kullanan, dil);
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
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.STR.IsKurallari.STRDil.Listele(null, kullanan, dil);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
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
                donusDeger = TNS.STR.IsKurallari.STRDil.Sil(null, kullanan, dil);
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
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.STR.IsKurallari.STRMisyon.Kaydet(null, kullanan, misyon);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
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
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.STR.IsKurallari.STRMisyon.Listele(null, kullanan, misyon);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
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
                donusDeger = TNS.STR.IsKurallari.STRVizyon.Kaydet(null, kullanan, vizyon);
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
                donusDeger = TNS.STR.IsKurallari.STRVizyon.Listele(null, kullanan, vizyon);
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
                donusDeger = TNS.STR.IsKurallari.STRTema.Kaydet(null, kullanan, tema);
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
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.STR.IsKurallari.STRTema.Listele(null, kullanan, tema);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen dönem başlangıç yılına ait tema özet/kısa bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tema listesi döndürülür.</returns>
        public ObjectArray TemaKisaListele(Kullanici kullanan, int donemBYil)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.STR.IsKurallari.STRTema.TemaKisaListele(null, kullanan, donemBYil);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Tema, stratejik amaçta kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// Tema nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="temalar">Tema nesnelerini tutan liste</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc TemaSil(Kullanici kullanan, TNSCollection temalar)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.STR.IsKurallari.STRTema.Sil(null, kullanan, temalar);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
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
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.STR.IsKurallari.STRAmac.Kaydet(null, kullanan, strAmac);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
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
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.STR.IsKurallari.STRAmac.Listele(null, kullanan, strAmac);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen dönem başlangıç yılına ait stratejik amaç özet/kısa bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik amaç listesi döndürülür.</returns>
        public ObjectArray StratejikAmacKisaListele(Kullanici kullanan, int donemBYil)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.STR.IsKurallari.STRAmac.AmacKisaListele(null, kullanan, donemBYil);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
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
                donusDeger = TNS.STR.IsKurallari.STRAmac.Sil(null, kullanan, amaclar);
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
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.STR.IsKurallari.STRHedef.Kaydet(null, kullanan, strHedef);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan stratejik hedef bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="strHedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik hedef listesi döndürülür.</returns>
        public ObjectArray StratejikHedefListele(Kullanici kullanan, StratejikHedef strHedef)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.STR.IsKurallari.STRHedef.Listele(null, kullanan, strHedef);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen dönem başlangıç yılına ait stratejik hedef özet/kısa bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında stratejik hedef listesi döndürülür.</returns>
        public ObjectArray StratejikHedefKisaListele(Kullanici kullanan, int donemBYil)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.STR.IsKurallari.STRHedef.HedefKisaListele(null, kullanan, donemBYil);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
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
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.STR.IsKurallari.STRHedef.Sil(null, kullanan, hedefler);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
    }
}