using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TNS.KYM;

namespace TNS.BHF
{
    /// <summary>
    /// Bütçe hazırlık ek formları paketiyle ilgili ortak statik yordamları tutan sınıf.
    /// </summary>
    [Serializable()]
    public static class Arac
    {
        /// <summary>
        /// Bütçe hazırlık ek formları sunucu nesnesi uyandıran yordam.
        /// </summary>
        /// <returns>Bütçe hazırlık ek formları sunucu nesnesi döndürülür.</returns>
        public static IBHFServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("BHFServisURL");
            IBHFServis servis = TNS.Api.ServisIstemci.Olustur<IBHFServis>(sUrl);
            return servis;
        }
    }

    /// <summary>
    /// AracKiralamaPlanDetay: 
    /// </summary>
    [System.Serializable()]
    public class AracKiralamaPlanDetay
    {
        public double buYilFiyat { get; set; }
        public double gelecekYilFiyat { get; set; }
        public double oncYilKiralanan { get; set; }
        public int oncYilKiralananSure { get; set; }
        public double oncYilFiyat { get; set; }

        /// <summary>
        /// Taşıt Tipi: Taşıt Tipi (Binek, Arazi)
        /// </summary>
        public int tasitTipi { get; set; }

        /// <summary>
        /// Taşıt Kodu: Taşıt T Kodu
        /// </summary>
        public string tasitKodu { get; set; }

        /// <summary>
        /// Bütçe Kaynağı: Bütçe Kaynağı (Bütçe, Döner Sermaye)
        /// </summary>
        public int butceKaynagi { get; set; }

        /// <summary>
        /// Bu Yıl Kiralanan: Bu yıla ait kiralanan araç sayısı
        /// </summary>
        public double buYilKiralanan { get; set; }

        /// <summary>
        /// Bu Yıl Kiralanan Süre: Bu yıla ait kiralanan araç süresi
        /// </summary>
        public int buYilKiralananSure { get; set; }

        /// <summary>
        /// Gelecek Yıl Kiralanacak: Gelecek yıl kiralanacak araç sayısı
        /// </summary>
        public double gelecekYilKiralanacak { get; set; }

        /// <summary>
        /// Gelecek Yıl Kiralanacak Süre: Gelecek yıl kiralanacak araç süresi
        /// </summary>
        public int gelecekYilKiralanacakSure { get; set; }
    }

    /// <summary>
    /// AracKiralamaPlanAna: 
    /// </summary>
    [System.Serializable()]
    public class AracKiralamaPlanAna
    {

        /// <summary>
        /// Yıl: İşlem yılı
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// Muhasebe Kod: Muhasebe kodunu giriniz.
        /// </summary>
        public int muhasebeKod { get; set; }

        /// <summary>
        /// Birim Kod: Birim kodu
        /// </summary>
        public string birimKod { get; set; }

        /// <summary>
        /// Durum: Verinin durumu (yeni, onaylı)
        /// </summary>
        public int durum { get; set; }

        /// <summary>
        /// Kaydeden: Birimde bilgiyi giren kişi.
        /// </summary>
        public string kaydeden { get; set; }

        /// <summary>
        /// Kayıt Tarihi: Birimde bilginin girildiği tarih.
        /// </summary>
        public TNSDateTime kayitTarihi { get; set; }

        /// <summary>
        /// Onaylayan: Birimde bilgiyi onaylayan kişi.
        /// </summary>
        public string onaylayan { get; set; }

        /// <summary>
        /// Onay Tarihi: Birimin bilgiyi onayladığı tarih
        /// </summary>
        public TNSDateTime onayTarihi { get; set; }

        /// <summary>
        /// AracKiralamaPlanDetay: 
        /// </summary>
        public TNSCollection aracKiralamaPlanDetay;

        public int siralaYontem = 0;

        public void RakamlarGecerliMi(Sonuc sonuc)
        {
            int i = 1;
            foreach (AracKiralamaPlanDetay item in aracKiralamaPlanDetay)
            {
                if (item.butceKaynagi < 1 || item.butceKaynagi > 2)
                    sonuc.DegerAta($"{i}. satırda bütçe kaynağı girilmeli!");
                if (string.IsNullOrWhiteSpace(item.tasitKodu))
                    sonuc.DegerAta($"{i}. satırda taşıt kodu boş olamaz!");
                string satirBilgi = $"Bütçe kaynak kodu {item.butceKaynagi} ve taşıt kodu {item.tasitKodu} olan {i}. satırda ";
                if (item.oncYilKiralanan < 0 || item.oncYilKiralananSure < 0 || item.oncYilFiyat < 0 ||
                    item.buYilKiralanan < 0 || item.buYilKiralananSure < 0 || item.buYilFiyat < 0 ||
                    item.gelecekYilKiralanacak < 0 || item.gelecekYilKiralanacakSure < 0)
                    sonuc.DegerAta(satirBilgi + "Lütfen negatif rakam girmeyiniz!");

                if (item.oncYilKiralananSure > 12 || item.buYilKiralananSure > 12 || item.gelecekYilKiralanacakSure > 12)
                    sonuc.DegerAta(satirBilgi + "Kiralama süreleri 12 ayı geçemez!");

                if ((item.oncYilKiralanan > 0 || item.oncYilKiralananSure > 0 || item.oncYilFiyat > 0) && (item.oncYilKiralanan == 0 || item.oncYilKiralananSure == 0 || item.oncYilFiyat == 0))
                    sonuc.DegerAta(satirBilgi + "Önceki yıl verilerinin ya hepsini giriniz ya hepsini boş bırakınız!");

                if ((item.buYilKiralanan > 0 || item.buYilKiralananSure > 0 || item.buYilFiyat > 0) && (item.buYilKiralanan == 0 || item.buYilKiralananSure == 0 || item.buYilFiyat == 0))
                    sonuc.DegerAta(satirBilgi + "Bu yıl verilerinin ya hepsini giriniz ya hepsini boş bırakınız!");

                if ((item.gelecekYilKiralanacak > 0 || item.gelecekYilKiralanacakSure > 0) && (item.gelecekYilKiralanacak == 0 || item.gelecekYilKiralanacakSure == 0))
                    sonuc.DegerAta(satirBilgi + "Gelecek yıl verilerinin ya hepsini giriniz ya hepsini boş bırakınız!");
                i++;
            }
        }
    }

    /// <summary>
    /// Bütçe cetvellerinde yer alan taşıt sözlüklerinin bir satırının bilgilerini tutan sınıf.
    /// </summary>
    [Serializable()]
    public class TasitSozluk
    {
        public string SiraNo { get { return siraNo; } set { siraNo = value; } }
        public string Cinsi { get { return cinsi; } set { cinsi = value; } }
        public string Diffarensiyel { get { return diffarensiyel; } set { diffarensiyel = value; } }
        public string Aciklama { get { return siraNo + ": " + cinsi + " (" + Diffarensiyel + ")"; } }

        /// <summary>
        /// Taşıt cetveli sıra numarası
        /// </summary>
        public string siraNo { get; set; } = string.Empty;

        /// <summary>
        /// Taşıtın cinsi
        /// </summary>
        public string cinsi { get; set; } = string.Empty;

        /// <summary>
        /// Taşıtın diffarensiyeli.
        /// </summary>
        public string diffarensiyel = string.Empty;
    }

    /// <summary>
    /// Sözlük bazında, kullanım yeri ve finans kaynağı bazında edinilecek miktarı tutan sınıf.
    /// </summary>
    [Serializable()]
    public class EdinilecekTasitSatiri
    {
        /// <summary>
        /// Edinilecek taşıtın türü.
        /// </summary>
        public TasitSozluk tasitSozluk = new TasitSozluk();
        public string siraNo { get { return tasitSozluk.siraNo; } set { tasitSozluk.siraNo = value; } }

        /// <summary>
        /// Edinilecek taşıtın miktarı.
        /// </summary>
        public int adet { get; set; } = 0;

        /// <summary>
        /// Edinilecek taşıtın kullanım yeri.
        /// </summary>
        public string kullanimYeri { get; set; } = string.Empty;

        /// <summary>
        /// Edinilecek taşıtın finans kaynağı.
        /// </summary>
        public string finkaynak { get; set; } = string.Empty;
    }

    [Serializable()]
    public class FaaliyetGerekce
    {

        public class Sirala : IComparer
        {
            private int alan;
            /// <summary>
            /// 1 ise Kurum Kod; 2 ise Faal Kod;
            /// </summary>
            /// <param name="alan"></param>
            public Sirala(int alan)
            {
                this.alan = alan;
            }

            public int Compare(object x, object y)
            {
                return CompareIc((FaaliyetGerekce)x, (FaaliyetGerekce)y);
            }

            public int CompareIc(FaaliyetGerekce x, FaaliyetGerekce y)
            {
                int tut;
                if (alan == 1)
                {
                    tut = x.kurKod.CompareTo(y.kurKod);
                    if (tut == 0)
                        tut = x.faalKod.CompareTo(y.faalKod);
                }
                else
                {
                    tut = x.faalKod.CompareTo(y.faalKod);
                    if (tut == 0)
                        tut = x.kurKod.CompareTo(y.kurKod);
                }
                return tut;
            }
        }

        public Sonuc sonuc = new Sonuc();
        public string kurKod { get; set; } = "";
        public int faalKod { get; set; }

        public string gerekce { get; set; } = "";
        public string dayanak { get; set; } = "";
        public string maliyet { get; set; } = "";
    }

    /// <summary>
    /// Uluslararası kuruluşlara üyelik ve üyeliğin maliyeti ile ilgili bilgileri tutan sınıf.
    /// </summary>
    [Serializable()]
    public class UluslarArasiKurulus
    {
        /// <summary>
        /// Üyelik sıra numarası
        /// </summary>
        public int siraNo = 0;

        /// <summary>
        /// Üye olunan kuruluşun adı
        /// </summary>
        public string kurulusAd = string.Empty;

        /// <summary>
        /// Üyeliğin yasal dayanağı
        /// </summary>
        public string yasalDayanak = string.Empty;

        /// <summary>
        /// Üyeli sonucunda bir sonraki yıl katınılması planlanan toplantı sayısı.
        /// </summary>
        public int toplantiSayisi = 0;

        /// <summary>
        /// Katınılması planlanan toplantılara katılması planlanan kişilerin sayısı.
        /// </summary>
        public int toplantiKatsayisi = 0;

        /// <summary>
        /// Yapılacak olan harcamaların döviz cinsi.
        /// </summary>
        public string dovizCinsi = string.Empty;

        /// <summary>
        /// Üyeliğin bir önceki yıldaki aidatı.
        /// </summary>
        public double oncekiyilAidat = 0;

        /// <summary>
        /// Üyeliğin bir önceki yıldaki aidatının döviz karşılığı.
        /// </summary>
        public double oncekiyilDoviz = 0;

        /// <summary>
        /// Üyeliğin bütçe hazırlığı yapılan yıldaki aidatı.
        /// </summary>
        public double hazirlikYilAidat = 0;

        /// <summary>
        /// Üyeliğin bütçe hazırlığı yapılan yıldaki aidatının döviz karşılığı.
        /// </summary>
        public double hazirlikYilDoviz = 0;

        /// <summary>
        /// Üyeliğin bütçe hazırlığı yapılan yıldan bir yıl sonraki yıldaki aidatı.
        /// </summary>
        public double ertesiYilAidat = 0;

        /// <summary>
        /// Üyeliğin bütçe hazırlığı yapılan yıldan bir yıl sonraki yıldaki aidatının döviz karşılığı.
        /// </summary>
        public double ertesiYilDoviz = 0;

        /// <summary>
        /// Üyeliğin bütçe hazırlığı yapılan yıldan iki yıl sonraki yıldaki aidatı.
        /// </summary>
        public double ertesiYil2Aidat = 0;

        /// <summary>
        /// Üyeliğin bütçe hazırlığı yapılan yıldan iki yıl sonraki yıldaki aidatının döviz karşılığı.
        /// </summary>
        public double ertesiYil2Doviz = 0;

        /// <summary>
        /// Planlanan üyelikle ilgili açıklama
        /// </summary>
        public string aciklama = string.Empty;
    }

    /// <summary>
    /// Biri birimin hangi ekonomik koddan ne kadar tavan üstü ödenek talebi olduğunu ve açıklamasını tutan sınıf.
    /// </summary>
    [Serializable()]
    public class TavanUstuOdenek
    {
        /// <summary>
        /// Tavan üstü ödeneği talep eden birimin kurumsal kodu.
        /// </summary>
        public string kurumsalKod;

        /// <summary>
        /// Tavan üstü ödenek talebinin gerekçe metni.
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Tavan üstü ödeneğin ekonomik kodu.
        /// </summary>
        public string ekokod;

        /// <summary>
        /// Talep edilen tavan üstü ödenek miktarı.
        /// </summary>
        public double odenek;
    }

    /// <summary>
    /// Yeni yapılacak yada tamamlanacak hizmetler için, ekonomik kod ve miktarları tutan dizinin bir elemanı.
    /// </summary>
    [Serializable()]
    public class HizmetBilgiEko
    {
        /// <summary>
        /// Yeni yapılacak yada tamamlanacak hizmetin maliyetinin bir ekonomik kodu.
        /// </summary>
        public string ekoKod;

        /// <summary>
        /// Yeni yapılacak yada tamamlanacak hizmetin maliyetinin bir ekonomik koduna ait tutarı.
        /// </summary>
        public double miktar;
    }


    /// <summary>
    /// Bir birimin yeni yapılacak yada tamamlanacak hizmeti ile ilgili bilgisini tutan sınıf.
    /// </summary>
    [Serializable()]
    public class HizmetBilgisiSatiri
    {
        /// <summary>
        /// Yeni yapılacak yada tamamlanacak hizmetin biriminin kurumsal kodu. 
        /// </summary>
        public string birimKod;

        /// <summary>
        /// Yeni yapılacak yada tamamlanacak hizmetin biriminin adı. 
        /// </summary>
        public string birimAd;

        /// <summary>
        /// Yeni yapılacak yada tamamlanacak hizmetin sıra numarası. 
        /// </summary>
        public int siraNo;

        /// <summary>
        /// Yeni yapılacak yada tamamlanacak hizmetin başlangıç tarihi.
        /// </summary>
        public TNSDateTime baslangicTarihi;

        /// <summary>
        /// Yeni yapılacak yada tamamlanacak hizmetin bitiş tarihi.
        /// </summary>
        public TNSDateTime bitisTarihi;

        /// <summary>
        /// Yeni yapılacak yada tamamlanacak hizmetin adı.
        /// </summary>
        public string hizmetAdi;

        /// <summary>
        /// Yeni yapılacak yada tamamlanacak hizmetin açıklaması.
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Yeni yapılacak yada tamamlanacak hizmetin ekonomik kod bazında maliyetlerini tutan dizi.
        /// </summary>
        public HizmetBilgiEko[] miktarlar;
    }

    /// <summary>
    /// Fiziksel değeler sözlüğü için, her bir bilginin verilen bir yıldaki miktarını tutan sınıf.
    /// </summary>
    [Serializable()]
    public class FizikselDegerYilBilgi
    {
        /// <summary>
        /// Fiziksel değeler sözlüğü için, her bir bilginin verilen miktarının yılı.
        /// </summary>
        public int yil;

        /// <summary>
        /// Fiziksel değeler sözlüğü için, her bir bilginin verilen yıldaki miktarı.
        /// </summary>
        public int deger;
    }

    /// <summary>
    /// Bütçe hazırlığı sırasında kullanılan bir fiziksel değerin; kodunu, açıklamasını ve yıl değerlerini tutan sınıf.
    /// </summary>
    [Serializable()]
    public class FizikselDeger
    {
        /// <summary>
        /// Bilgisi girilen fiziksel değerin kodu. Bu kod ebütçe tarafından verilmektedir.
        /// </summary>
        public string kod;

        /// <summary>
        /// Bilgisi girilen fiziksel değerin açıklaması. Bu ad ebütçe tarafından verilmektedir.
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Bu fiziksel değer bilgisine veri girilecek mi? Fiziksel değerler hiyerarşik olarak girildiğinden, hiyerarşide üst satırlarda olan bilgilere veri girilmez. Bu satırların değeri, alt değerlerin toplamı olarak bulunur.
        /// </summary>
        public bool degerGirilsinMi;

        /// <summary>
        /// Fiziksel değerlerin yıl bazında miktarını tutan dizi.
        /// </summary>
        public FizikselDegerYilBilgi[] degerler;
    }

    /// <summary>
    /// Döner sermaye ve fon girişlerinde her yıl için bütçe ödeneği yada gerçekleşmenin tutulduğu sınıf.
    /// </summary>
    [Serializable()]
    public class DonerSermayeYil
    {
        /// <summary>
        /// Ödenek yada gerçekleşme yılı.
        /// </summary>
        public int yil;

        /// <summary>
        /// Ödenek mi alanı doğru ise değer alanında ödenek, yoksa gerçekleşme rakamı vardır.
        /// </summary>
        public bool odenekMi;

        /// <summary>
        /// Döner sermaye yada fonun ilgili yıldaki ödenek ve gerçekleşme tutarı.
        /// </summary>
        public double deger;
    }

    /// <summary>
    /// Döner sermaye ve fon bilgilerinin gelir ve gider satırlarının her birini tutan sınıf.
    /// </summary>
    [Serializable()]
    public class DonerSermayeSatir
    {
        /// <summary>
        /// Döner sermaye ve fon bilgisi gelir gider kodu. eBütçeden alınmalıdır.
        /// </summary>
        public string kod;

        /// <summary>
        /// Döner sermaye ve fon bilgisi gelir gider adı. eBütçeden alınmalıdır.
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Bu satıra veri girilecek mi? Döner sermaye ve fon bilgisi gelir gider  kodları hiyerarşik olarak girildiğinden, hiyerarşide üst satırlarda olan bilgilere veri girilmez. Bu satırların değeri, alt değerlerin toplamı olarak bulunur.
        /// </summary>
        public bool degerGirilsinMi;

        /// <summary>
        /// Bu satıra ait her yıl için döner sermaye veya fon bilgisinin gerçekleşme ve ödeneklerini tutan dizi. 
        /// </summary>
        public DonerSermayeYil[] degerler;
    }

    /// <summary>
    /// Döner sermaye/fon bilgileri için döner sermaye/fon adını ve ödenek/gerçekleşme satırlarının dizisini tutan sınıf.
    /// </summary>
    [Serializable()]
    public class DonerSermaye
    {
        /// <summary>
        /// Döner sermaye/fon adı.
        /// </summary>
        public string donerSermayeAd = string.Empty;

        /// <summary>
        /// Döner sermaye/fon için ödenek/gerçekleşme satırlarının dizisi.
        /// </summary>
        public DonerSermayeSatir[] donerSermayeSatirlari;

        /// <summary>
        /// Sunucudan okuma yapıldığında, okuma işleminin başarılı olup olmadığını tutan Sonuc yapısı.
        /// </summary>
        public Sonuc sonuc = new Sonuc();
    }

    /// <summary>
    /// Hizmet maliyetinin tesbiti için gerekli parametrelerin alındığı formun her bir satırının tutulduğu sınıf.
    /// </summary>
    [Serializable()]
    public class HizmetMaliyetiSatir
    {
        /// <summary>
        /// Hizmet maliyetinin tesbiti satırının kodu. Bu kod eBütçeden alınmalıdır.
        /// </summary>
        public string kod = string.Empty;

        /// <summary>
        /// Hizmet maliyetinin tesbiti satırının açıklaması. Bu değer eBütçeden alınmalıdır.
        /// </summary>
        public string aciklama = string.Empty;

        /// <summary>
        /// Bu satıra veri girilecek mi? Hizmet maliyeti tesbiti satır kodları hiyerarşik olarak girildiğinden, hiyerarşide üst satırlarda olan bilgilere veri girilmez. Bu satırların değeri, alt değerlerin toplamı olarak bulunur.
        /// </summary>
        public bool degerGirilsinMi = true;

    }

    /// <summary>
    /// Hizmet maliyetinin tesbiti için gerekli parametrelerin alındığı formun her bir satırının tutulduğu sınıf.
    /// </summary>
    [Serializable()]
    public class HizmetMaliyetiTesbitiSatir
    {
        /// <summary>
        /// Hizmet maliyetinin tesbiti satırının kodu. Bu kod eBütçeden alınmalıdır.
        /// </summary>
        public string kod { get; set; } = string.Empty;

        /// <summary>
        /// Hizmet maliyetinin tesbiti satırının açıklaması. Bu değer eBütçeden alınmalıdır.
        /// </summary>
        public string aciklama { get; set; } = string.Empty;

        /// <summary>
        /// Bu satıra veri girilecek mi? Hizmet maliyeti tesbiti satır kodları hiyerarşik olarak girildiğinden, hiyerarşide üst satırlarda olan bilgilere veri girilmez. Bu satırların değeri, alt değerlerin toplamı olarak bulunur.
        /// </summary>
        public bool degerGirilsinMi { get; set; } = true;

        /// <summary>
        /// Hizmet maliyeti için girilen satırın bir önceki yıl gerçekleşmesi.
        /// </summary>
        public double gercek { get; set; } = 0;

        /// <summary>
        /// Hizmet maliyeti için girilen satırın içinde bulunulan yıl haziran ayı gerçekleşmesi.
        /// </summary>
        public double haziran { get; set; } = 0;

        /// <summary>
        /// Hizmet maliyeti için girilen satırın bütçesi yapılan yıl için ödeneği.
        /// </summary>
        public double tahmin { get; set; } = 0;

        /// <summary>
        /// Hizmet maliyeti için girilen satırın bütçesi yapılan yıldan sonraki yıl için ödeneği.
        /// </summary>
        public double tahmin1 { get; set; } = 0;

        /// <summary>
        /// Hizmet maliyeti için girilen satırın bütçesi yapılan yıldan iki sonraki yıl için ödeneği.
        /// </summary>
        public double tahmin2 { get; set; } = 0;
    }

    /// <summary>
    /// Özel güvenlik formundaki, maliyet unsurlarını içeren tablonun her bir satırını tutan sınıf.
    /// </summary>
    [Serializable()]
    public class OzelGuvenlikMaliyet
    {
        /// <summary>
        /// Özel güvenlik formundaki, maliyet unsurlarını içeren tablonun satırının kodu.
        /// </summary>
        public string kod;

        /// <summary>
        /// Özel güvenlik formundaki, maliyet unsurlarını içeren tablonun satırının açıklaması.
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Maliyet satırında, ilgili kalemin, harcaması.
        /// </summary>
        public double Harcama;

        /// <summary>
        /// Maliyet satırında, ilgili kalemin, yıl sonu harcama tahmini.
        /// </summary>
        public double Tahmin;

        /// <summary>
        /// Maliyet satırında, ilgili kalemin, talep edilen ödeneği.
        /// </summary>
        public double Teklif;
    }

    /// <summary>
    /// Özel güvenlik/temizlik bilgi formundaki ilgili ödeneği hangi tertiplerden karşılanacağını tutan tablonun her bir satırını tutan sınıf.
    /// </summary>
    [Serializable()]
    public class GuvenlikTemizlikIhaleTertip
    {
        /// <summary>
        /// Özel güvenlik/temizlik bilgi formundaki ilgili ödeneğin tertibinin, kurumsal kodu.
        /// </summary>
        public string kurKod;

        /// <summary>
        /// Özel güvenlik/temizlik bilgi formundaki ilgili ödeneğin tertibinin, fonksiyonel kodu.
        /// </summary>
        public string fonKod;

        /// <summary>
        /// Özel güvenlik/temizlik bilgi formundaki ilgili ödeneğin tertibinin, finansman kodu.
        /// </summary>
        public string finKod;

        /// <summary>
        /// Özel güvenlik/temizlik bilgi formundaki ilgili ödeneğin tertibinin, ekonomik kodu.
        /// </summary>
        public string ekoKod;
    }

    /// <summary>
    /// Özel güvenlik formundaki, yapılan ihale ile ilgili büyüklüklerin tutulduğu tablo bilgileri ile kimlik bilgilerini tutan sınıf.
    /// </summary>
    [Serializable()]
    public class OzelGuvenlikBilgi
    {
        /// <summary>
        /// İhaleye çıkacak birimin kurumsal kodu.
        /// </summary>
        public string kurKod;

        /// <summary>
        /// İhalenin sıra numarasının ve açıklamasının tutulduğu yapıl
        /// </summary>
        public OzelGuvenlikIhaleTanim ihaleTanim = new OzelGuvenlikIhaleTanim();

        /// <summary>
        /// İhale ile özel güvenlik tarafından korunacak alanın, içinde bulunulan yıla göre önceki yıldaki miktarı.
        /// </summary>
        public int hzmAlanOnceki;

        /// <summary>
        /// İhale ile özel güvenlik tarafından korunacak alanın, içinde bulunulan yıla göre bu yıldaki miktarı.
        /// </summary>
        public int hzmAlanBuYil;

        /// <summary>
        /// İhale ile özel güvenlik tarafından korunacak alanın, içinde bulunulan yıla göre sonraki yıldaki (bütçe hazırlığı yapılan yıl) miktarı.
        /// </summary>
        public int hzmAlanSonraki;

        /// <summary>
        /// İhale ile alınacak özel güvenlik elemanı sayısının, içinde bulunulan yıla göre önceki yıldaki miktarı.
        /// </summary>
        public int gorSayisiOnceki;

        /// <summary>
        /// İhale ile alınacak özel güvenlik elemanı sayısının, içinde bulunulan yıla göre bu yıldaki miktarı.
        /// </summary>
        public int gorSayisiBuYil;

        /// <summary>
        /// İhale ile alınacak özel güvenlik elemanı sayısının, içinde bulunulan yıla göre sonraki yıldaki (bütçe hazırlığı yapılan yıl) miktarı.
        /// </summary>
        public int gorSayisiSonraki;

        /// <summary>
        /// İhale ile alınacak özel güvenlik hizmetinin, içinde bulunulan yıla göre önceki yıldaki başlangıç tarihi.
        /// </summary>
        public string basOnceki;

        /// <summary>
        /// İhale ile alınacak özel güvenlik hizmetinin, içinde bulunulan yıla göre bu yıldaki başlangıç tarihi.
        /// </summary>
        public string basBuYil;

        /// <summary>
        /// İhale ile alınacak özel güvenlik hizmetinin, içinde bulunulan yıla göre sonraki yıldaki (bütçe hazırlığı yapılan yıl) başlangıç tarihi.
        /// </summary>
        public string basSonraki;

        /// <summary>
        /// İhale ile alınacak özel güvenlik hizmetinin, içinde bulunulan yıla göre önceki yıldaki bitiş tarihi.
        /// </summary>
        public string bitisOnceki;

        /// <summary>
        /// İhale ile alınacak özel güvenlik hizmetinin, içinde bulunulan yıla göre bu yıldaki bitiş tarihi.
        /// </summary>
        public string bitisBuYil;

        /// <summary>
        /// İhale ile alınacak özel güvenlik hizmetinin, içinde bulunulan yıla göre sonraki yıldaki (bütçe hazırlığı yapılan yıl) bitiş tarihi.
        /// </summary>
        public string bitisSonraki;

        /// <summary>
        /// İhalede kullanılacak tertiplerin listesini tutan dizi.
        /// </summary>
        public GuvenlikTemizlikIhaleTertip[] tertipler;

        /// <summary>
        /// İhalenin maliyet kalemlerini tutan dizi.
        /// </summary>
        public OzelGuvenlikMaliyet[] maliyetler;
    }

    /// <summary>
    /// İhalenin sıra numarasının ve açıklamasının tutulduğu sınıf.
    /// </summary>
    [Serializable()]
    public class OzelGuvenlikIhaleTanim
    {
        /// <summary>
        /// İhalenin birim içi sıra numarası.
        /// </summary>
        public int ihaleNo;

        /// <summary>
        /// İhale açıklaması.
        /// </summary>
        public string aciklama;
    }

    /// <summary>
    /// Temizlik hizmeti bilgi formundaki, maliyet unsurlarını içeren tablonun her bir satırını tutan sınıf.
    /// </summary>
    [Serializable()]
    public class TemizlikMaliyet
    {
        /// <summary>
        /// Temizlik hizmeti bilgi formundaki, maliyet unsurlarını içeren tablonun satırının kodu.
        /// </summary>
        public string kod;

        /// <summary>
        /// Temizlik hizmeti bilgi formundaki, maliyet unsurlarını içeren tablonun satırının açıklaması.
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Maliyet satırında, ilgili kalemin, harcaması.
        /// </summary>
        public double Harcama;

        /// <summary>
        /// Maliyet satırında, ilgili kalemin, yıl sonu harcama tahmini.
        /// </summary>
        public double Tahmin;

        /// <summary>
        /// Maliyet satırında, ilgili kalemin, talep edilen ödeneği.
        /// </summary>
        public double Teklif;
    }

    /// <summary>
    /// Temizlik hizmeti bilgi formundaki, yapılan ihale ile ilgili büyüklüklerin tutulduğu tablo bilgileri ile kimlik bilgilerini tutan sınıf.
    /// </summary>
    [Serializable()]
    public class TemizlikBilgi
    {
        /// <summary>
        /// İhaleye çıkacak birimin kurumsal kodu.
        /// </summary>
        public string kurKod;

        /// <summary>
        /// Yapılan ihalenin birim içi sıra numarası
        /// </summary>
        public int ihaleNo;

        /// <summary>
        /// Yapılan ihale ile ilgili açıklama
        /// </summary>
        public string aciklama;

        /// <summary>
        /// İhale ile Temizlik hizmeti ile ilgili alanın, içinde bulunulan yıla göre önceki yıldaki miktarı.
        /// </summary>
        public int hzmAlanOnceki;

        /// <summary>
        /// İhale ile Temizlik hizmeti ile ilgili alanın, içinde bulunulan yıla göre bu yıldaki miktarı.
        /// </summary>
        public int hzmAlanBuYil;

        /// <summary>
        /// İhale ile Temizlik hizmeti ile ilgili alanın, içinde bulunulan yıla göre sonraki yıldaki (bütçe hazırlığı yapılan yıl) miktarı.
        /// </summary>
        public int hzmAlanSonraki;

        /// <summary>
        /// İhale ile alınacak temizlik hizmetinde çalışacak elemanı sayısının, içinde bulunulan yıla göre önceki yıldaki miktarı.
        /// </summary>
        public int gorSayisiOnceki;

        /// <summary>
        /// İhale ile alınacak temizlik hizmetinde çalışacak elemanı sayısının, içinde bulunulan yıla göre bu yıldaki miktarı.
        /// </summary>
        public int gorSayisiBuYil;

        /// <summary>
        /// İhale ile alınacak temizlik hizmetinde çalışacak elemanı sayısının, içinde bulunulan yıla göre sonraki yıldaki (bütçe hazırlığı yapılan yıl) miktarı.
        /// </summary>
        public int gorSayisiSonraki;

        /// <summary>
        /// İhale ile alınacak temizlik hizmetinin, içinde bulunulan yıla göre önceki yıldaki başlangıç tarihi.
        /// </summary>
        public string basOnceki;

        /// <summary>
        /// İhale ile alınacak temizlik hizmetinin, içinde bulunulan yıla göre bu yıldaki başlangıç tarihi.
        /// </summary>
        public string basBuYil;

        /// <summary>
        /// İhale ile alınacak temizlik hizmetinin, içinde bulunulan yıla göre sonraki yıldaki (bütçe hazırlığı yapılan yıl) başlangıç tarihi.
        /// </summary>
        public string basSonraki;

        /// <summary>
        /// İhale ile alınacak temizlik hizmetinin, içinde bulunulan yıla göre önceki yıldaki bitiş tarihi.
        /// </summary>
        public string bitisOnceki;

        /// <summary>
        /// İhale ile alınacak temizlik hizmetinin, içinde bulunulan yıla göre bu yıldaki bitiş tarihi.
        /// </summary>
        public string bitisBuYil;

        /// <summary>
        /// İhale ile alınacak temizlik hizmetinin, içinde bulunulan yıla göre sonraki yıldaki (bütçe hazırlığı yapılan yıl) bitiş tarihi.
        /// </summary>
        public string bitisSonraki;


        /// <summary>
        /// İhalede kullanılacak tertiplerin listesini tutan dizi.
        /// </summary>
        public GuvenlikTemizlikIhaleTertip[] tertipler;

        /// <summary>
        /// İhalenin maliyet kalemlerini tutan dizi.
        /// </summary>
        public TemizlikMaliyet[] maliyetler;
    }

    /// <summary>
    /// Hizmet alımı yoluyla kullanılan görevlilere ilişkin bilgi formu satırlarını tutan sınıf.
    /// </summary>
    [Serializable()]
    public class HizmetAlimiGorevli
    {
        /// <summary>
        /// Formu satır numarası.
        /// </summary>
        public string SiraNo;

        /// <summary>
        /// İki yıl önceki temizlik görevlisi adedi
        /// </summary>
        public int ozelGorevliTemizlikAdet2;

        /// <summary>
        /// İki yıl önceki temizlik görevlisi süresi
        /// </summary>
        public int ozelGorevliTemizlikSure2;

        /// <summary>
        /// İki yıl önceki temizlik görevlilerinin maliyeti
        /// </summary>
        public double ozelGorevliTemizlikMaliyet2;

        /// <summary>
        /// İki yıl önceki güvenlik görevlisi adedi
        /// </summary>
        public int ozelGorevliGuvenlikAdet2;

        /// <summary>
        /// İki yıl önceki güvenlik görevlisi süresi
        /// </summary>
        public int ozelGorevliGuvenlikSure2;

        /// <summary>
        /// İki yıl önceki güvenlik görevlisi maliyeti
        /// </summary>
        public double ozelGorevliGuvenlikMaliyet2;

        /// <summary>
        /// İki yıl önceki şoför adedi
        /// </summary>
        public int ozelGorevliSoforAdet2;

        /// <summary>
        /// İki yıl önceki şoför süresi
        /// </summary>
        public int ozelGorevliSoforSure2;

        /// <summary>
        /// İki yıl önceki şoför maliyeti
        /// </summary>
        public double ozelGorevliSoforMaliyet2;

        /// <summary>
        /// İki yıl önceki diğer görevli adedi
        /// </summary>
        public int ozelGorevliDigerAdet2;

        /// <summary>
        /// İki yıl önceki diğer görevli süresi
        /// </summary>
        public int ozelGorevliDigerSure2;

        /// <summary>
        /// İki yıl önceki diğer görevli maliyeti
        /// </summary>
        public double ozelGorevliDigerMaliyet2;

        /// <summary>
        /// Bir yıl önceki temizlik görevlisi adedi
        /// </summary>
        public int ozelGorevliTemizlikAdet1;

        /// <summary>
        /// Bir yıl önceki temizlik görevlisi süresi
        /// </summary>
        public int ozelGorevliTemizlikSure1;

        /// <summary>
        /// Bir yıl önceki temizlik görevlilerinin maliyeti
        /// </summary>
        public double ozelGorevliTemizlikMaliyet1;

        /// <summary>
        /// Bir yıl önceki güvenlik görevlisi adedi
        /// </summary>
        public int ozelGorevliGuvenlikAdet1;

        /// <summary>
        /// Bir yıl önceki güvenlik görevlisi süresi
        /// </summary>
        public int ozelGorevliGuvenlikSure1;

        /// <summary>
        /// Bir yıl önceki güvenlik görevlisi maliyeti
        /// </summary>
        public double ozelGorevliGuvenlikMaliyet1;

        /// <summary>
        /// Bir yıl önceki şoför adedi
        /// </summary>
        public int ozelGorevliSoforAdet1;

        /// <summary>
        /// Bir yıl önceki şoför süresi
        /// </summary>
        public int ozelGorevliSoforSure1;

        /// <summary>
        /// Bir yıl önceki şoför maliyeti
        /// </summary>
        public double ozelGorevliSoforMaliyet1;

        /// <summary>
        /// Bir yıl önceki diğer görevli adedi
        /// </summary>
        public int ozelGorevliDigerAdet1;

        /// <summary>
        /// Bir yıl önceki diğer görevli süresi
        /// </summary>
        public int ozelGorevliDigerSure1;

        /// <summary>
        /// Bir yıl önceki diğer görevli maliyeti
        /// </summary>
        public double ozelGorevliDigerMaliyet1;

        /// <summary>
        /// Veri girilen satırın tüm alanlarına veri girilmiş mi kontrolü yapılır.
        /// </summary>
        /// <returns></returns>
        public bool DoluMu()
        {
            if (
        ozelGorevliTemizlikAdet2 > 0 ||
        ozelGorevliTemizlikSure2 > 0 ||
        ozelGorevliTemizlikMaliyet2 > 0 ||
        ozelGorevliGuvenlikAdet2 > 0 ||
        ozelGorevliGuvenlikSure2 > 0 ||
        ozelGorevliGuvenlikMaliyet2 > 0 ||
        ozelGorevliSoforAdet2 > 0 ||
        ozelGorevliSoforSure2 > 0 ||
        ozelGorevliSoforMaliyet2 > 0 ||
        ozelGorevliDigerAdet2 > 0 ||
        ozelGorevliDigerSure2 > 0 ||
        ozelGorevliDigerMaliyet2 > 0 ||

        ozelGorevliTemizlikAdet1 > 0 ||
        ozelGorevliTemizlikSure1 > 0 ||
        ozelGorevliTemizlikMaliyet1 > 0 ||
        ozelGorevliGuvenlikAdet1 > 0 ||
        ozelGorevliGuvenlikSure1 > 0 ||
        ozelGorevliGuvenlikMaliyet1 > 0 ||
        ozelGorevliSoforAdet1 > 0 ||
        ozelGorevliSoforSure1 > 0 ||
        ozelGorevliSoforMaliyet1 > 0 ||
        ozelGorevliDigerAdet1 > 0 ||
        ozelGorevliDigerSure1 > 0 ||
        ozelGorevliDigerMaliyet1 > 0)
                return true;
            return false;
        }
    }

    /// <summary>
    /// Hizmet alımı ile edinilecek taşıtların; adet, süre ve maliyet bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class HizmetAlimiTasitMaliyet
    {
        /// <summary>
        /// Hizmet alımı ile edinilecek taşıtların bağlı olduğu özelliğe göre adedi.
        /// </summary>
        public int adet;

        // / <summary>
        // / Hizmet alımı ile edinilecek taşıtların bağlı olduğu özelliğe göre süresi.
        // / </summary>
        public double sure;

        /// <summary>
        /// Hizmet alımı ile edinilecek taşıtların bağlı olduğu özelliğe göre maliyeti.
        /// </summary>
        public double maliyet;
    }

    /// <summary>
    /// Hizmet alımı ile edinilecek taşıtların şoförlü/şoförsüz, giderli/gidersiz, üç yıllık bilgilerinin (adet, süre maliyet) tutulduğu sınıf. 
    /// </summary>
    [Serializable()]
    public class HizmetAlimiTasitBilgisiDetay
    {
        /// <summary>
        /// Hizmet alımı ile edinilecek taşıtın türü.
        /// </summary>
        public TasitSozluk tasitSozluk = new TasitSozluk();

        /// <summary>
        /// Hizmet alımı ile edinilecek şoförlü ve yakıt gideri dahil maliyet bilgilerini 3 yıl için tutan yapı.
        /// </summary>
        public HizmetAlimiTasitMaliyet[] soforluYakitGiderDahil = new HizmetAlimiTasitMaliyet[2];

        /// <summary>
        /// Hizmet alımı ile edinilecek şoförlü ve yakıt gideri hariç maliyet bilgilerini 3 yıl için tutan yapı.
        /// </summary>
        public HizmetAlimiTasitMaliyet[] soforluYakitGiderHaric = new HizmetAlimiTasitMaliyet[2];

        /// <summary>
        /// Hizmet alımı ile edinilecek şoförsüz ve yakıt gideri dahil maliyet bilgilerini 3 yıl için tutan yapı.
        /// </summary>
        public HizmetAlimiTasitMaliyet[] soforsuzYakitGiderDahil = new HizmetAlimiTasitMaliyet[2];

        /// <summary>
        /// Hizmet alımı ile edinilecek şoförsüz ve yakıt gideri hariç maliyet bilgilerini 3 yıl için tutan yapı.
        /// </summary>
        public HizmetAlimiTasitMaliyet[] soforsuzYakitGiderHaric = new HizmetAlimiTasitMaliyet[2];

        /// <summary>
        /// Maliyet bilgilerini tutan dizilere maliyet nesneleri atanır.
        /// </summary>
        public HizmetAlimiTasitBilgisiDetay()
        {
            for (int i = 0; i < 2; i++)
            {
                soforluYakitGiderDahil[i] = new HizmetAlimiTasitMaliyet();
                soforluYakitGiderHaric[i] = new HizmetAlimiTasitMaliyet();
                soforsuzYakitGiderDahil[i] = new HizmetAlimiTasitMaliyet();
                soforsuzYakitGiderHaric[i] = new HizmetAlimiTasitMaliyet();
            }
        }

        /// <summary>
        /// Her yıl için ve her maliyet grubu için(şoförlü/şoförsüz, yakıt gideri dahil/hariç) tüm alanların dolu olduğunu kontrol eden yordam. Hepsi dolu ise doğru, değilse yanlış döndürülür.
        /// </summary>
        /// <returns>Tüm alanlar dolu ise doğru, değilse yanlış döndürülür.</returns>
        public bool DoluMu()
        {
            for (int i = 0; i < 2; i++)
            {
                if (
                    soforluYakitGiderDahil[i].adet > 0 ||
                    soforluYakitGiderDahil[i].maliyet > 0 ||

                    soforluYakitGiderHaric[i].adet > 0 ||
                    soforluYakitGiderHaric[i].maliyet > 0 ||

                    soforsuzYakitGiderDahil[i].adet > 0 ||
                    soforsuzYakitGiderDahil[i].maliyet > 0 ||

                    soforsuzYakitGiderHaric[i].adet > 0 ||
                    soforsuzYakitGiderHaric[i].maliyet > 0)
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Mevcut taşıtlar, tasfiye edilecek taşıtlar, hizmet alımı ile kullanılacak taşıt sayısı ve birim maliyetinin gibi taşıtlarla ilgili genel bilgileri tutan sınıf.
    /// </summary>
    [Serializable()]
    public class TasitGenelBilgi
    {
        public TasitGenelBilgi()
        {

        }

        public TasitGenelBilgi(string siraNo, int kaynakKod, int yasKod, int adet)
        {
            this.siraNo = siraNo;
            this.kaynakKod = kaynakKod;
            this.yasKod = yasKod;
            this.adet = adet;
        }

        public string siraNo { get { return tasitSozluk.siraNo; } set { tasitSozluk.siraNo = value; } }

        /// <summary>
        /// Genel bilgisi tutulan taşıt türünü gösteren taşıt sözlüğü yapısı.
        /// </summary>
        public TasitSozluk tasitSozluk = new TasitSozluk();


        public int kaynakKod { get; set; }

        public int yasKod { get; set; }

        public int adet { get; set; }


        ///// <summary>
        ///// Bütçe yılına göre iki yıl önce tasfiye edilen taşıt sayısı.
        ///// </summary>
        //public int tasfiye2;


        ///// <summary>
        ///// Bütçe yılına göre iki yıl önce hizmet alımı yoluyla kullanılan taşıtların birim maliyeti.
        ///// </summary>
        //public double birim2;

        ///// <summary>
        ///// Bütçe yılına göre bir yıl önce tasfiye edilen taşıt sayısı.
        ///// </summary>
        //public int tasfiye1;

        ///// <summary>
        ///// Bütçe yılına göre bir yıl önce hizmet alımı yoluyla kullanılan taşıtların birim maliyeti.
        ///// </summary>
        //public double birim1;

        ///// <summary>
        ///// Bütçe yılında tasfiye edilmesi planlanan taşıt sayısı.
        ///// </summary>
        //public int tasfiye0;

        ///// <summary>
        ///// Bütçe yılında hizmet alımı yoluyla kullanılması planlanan taşıtların birim maliyeti.
        ///// </summary>
        //public double birim0;

        /// <summary>
        /// Bütçe kaynağı ile sahip olunan taşıt sayısı
        /// </summary>
        //public int mevcutButce;

        ///// <summary>
        ///// Döner sermaye kaynağı ile sahip olunan taşıt sayısı
        ///// </summary>
        //public int mevcutDonersermaye;

        ///// <summary>
        ///// Fon kaynağı ile sahip olunan taşıt sayısı
        ///// </summary>
        //public int mevcutFon;

        ///// <summary>
        ///// Bütçe yılına göre iki yıl önce hizmet alımı yoluyla kullanılan taşıt sayısı.
        ///// </summary>
        //public int hizmetButce;

        ///// <summary>
        ///// Bütçe yılına göre bir yıl önce hizmet alımı yoluyla kullanılan taşıt sayısı.
        ///// </summary>
        //public int hizmetDonerSermaye;

        ///// <summary>
        ///// Bütçe yılında hizmet alımı yoluyla kullanılması planlanan taşıt sayısı.
        ///// </summary>
        //public int hizmetFon;


        /// <summary>
        /// Taşıt bilgi kaydının tutarlılığını kontrol eden yordam. Eğer tutarlı ise doğru, tutarlı değil ise sonuç yapısında hata mesajı ile birlikte yanlış döndürür.
        /// </summary>
        /// <param name="sonuc"></param>
        /// <returns>Taşıt bilgi kaydı tutarlı ise doğru, değilse yanlış döndürülür.</returns>
        public bool KontrolEt(Sonuc sonuc, int i)
        {
            if (string.IsNullOrEmpty(tasitSozluk.siraNo))
                sonuc.DegerAta($"{i}. satırda hatalı taşıt kodu seçimi");
            if (this.kaynakKod < 1 || this.kaynakKod > 5)
                sonuc.DegerAta($"{i}. satırda hatalı kaynak seçimi");
            if (this.yasKod < 0 || this.yasKod > 15)
                sonuc.DegerAta($"{i}. satırda hatalı yaş seçimi");
            return sonuc.islemSonuc;
        }

        /// <summary>
        /// Veri girilmesi zorunlu olan alanların dolu olup olmadığı kontrol edilen yordam.
        /// </summary>
        /// <returns>Zorunlu alanlar dolu ise doğru, değilse yanlış döndürülür.</returns>
        public bool DoluMu()
        {
            return (this.adet > 0);
        }

        public bool EsitMi(TasitGenelBilgi hatIc)
        {
            if (this.siraNo == hatIc.siraNo && this.kaynakKod == hatIc.kaynakKod && this.yasKod == hatIc.yasKod)
                return true;
            return false;
        }
    }

    /// <summary>
    /// TasitMarkaModel: Araç Tipleri
    /// </summary>
    [System.Serializable()]
    public class TasitCins
    {
        /// <summary>
        /// Kod: Model Kodu
        /// </summary>
        public int kod { get; set; }

        /// <summary>
        /// Model: Model Adı
        /// </summary>
        public string ad { get; set; }

        public int tipKodu { get; set; }

        /// <summary>
        /// Taşıt cetveli T Kodu
        /// </summary>
        public string tkodu { get; set; }


        public string tip { get; set; }

        public int motorsuz { get; set; }
    }

    /// <summary>
    /// TasitMarkaModel: Araç Tipleri
    /// </summary>
    [System.Serializable()]
    public class TasitTip
    {
        /// <summary>
        /// Kod: Model Kodu
        /// </summary>
        public int kod { get; set; }

        /// <summary>
        /// Model: Model Adı
        /// </summary>
        public string ad { get; set; }
    }


    /// <summary>
    /// TasitMarkaModel: Araç Modelleri
    /// </summary>
    [System.Serializable()]
    public class TasitMarkaModel
    {
        /// <summary>
        /// Kod: Model Kodu
        /// </summary>
        public int kod { get; set; }

        /// <summary>
        /// Marka: Marka Adı
        /// </summary>
        public string marka { get; set; }

        /// <summary>
        /// Model: Model Adı
        /// </summary>
        public string model { get; set; }

        /// <summary>
        /// Model: Model Adı
        /// </summary>
        public int tipKodu { get; set; }

        public string tipAdi { get; set; }

        public int kesin { get; set; }
    }

    public enum ENUMYakitTuru : int
    {
        Bos = 0,
        Benzinli = 1,
        Dizel = 2,
        Benzin_LPG = 3
    }

    public enum ENUMSureBirim : int
    {
        Bos = 0,
        Yil = 1,
        Ay = 2,
        TakvimGunu = 3,
        IsGunu = 4
    }

    /// <summary>
    /// Ait Olduğu Bütçe: 
    /// </summary>
    public enum ENUMAitOlduguButce : int
    {
        Bos = 0,
        GenelButce = 1,
        DonerSermaye = 2,
        SosyalTesis = 3
    }

    /// <summary>
    /// Kullanım Durumu: Faal-Faal Değil-Onarımı Ekonomik Değil
    /// </summary>
    public enum ENUMKullanimDurumu : int
    {
        Bos = 0,
        Faal = 1,
        FaalDegil = 2,
        OnarimiEkonomikDegil = 3,
        //Ayrildi = 4
    }

    public enum ENUMAracCikisSebebi : int
    {
        Bos = 0,
        KURUMICIDEVIR = 1,
        KURUMDISIDEVIR = 2,
        HURDA = 3,
        SATIS = 4,
        DIGER = 5,
        TERKIN = 6
    }

    /// <summary>
    /// TasitBilgi: 
    /// </summary>
    [System.Serializable()]
    public class TasitBilgi
    {
        /// <summary>
        /// Şase No: Şase No
        /// </summary>
        public string saseNo { get; set; }

        /// <summary>
        /// Model Kodu: Model Kodu
        /// </summary>
        public int modelKodu { get; set; }

        /// <summary>
        /// Markası: Markası
        /// </summary>
        public string markasi { get; set; }

        /// <summary>
        /// Modeli: Modeli
        /// </summary>
        public string modeli { get; set; }

        /// <summary>
        /// Model Yılı: Model Yılı
        /// </summary>
        public int modelYili { get; set; }

        /// <summary>
        /// Cins Kodu: Cins Kodu
        /// </summary>
        public int cinsKodu { get; set; }

        /// <summary>
        /// Cinsi: Cinsi
        /// </summary>
        public string cinsi { get; set; }

        /// <summary>
        /// Tip Kodu: Tip Kodu
        /// </summary>
        public int tipKodu { get; set; }

        /// <summary>
        /// Tipi: Tipi
        /// </summary>
        public string tipi { get; set; }

        /// <summary>
        /// Motor No: Motor No
        /// </summary>
        public string motorNo { get; set; }

        /// <summary>
        /// Koltuk Sayısı: Koltuk Sayısı
        /// </summary>
        public int koltukSayisi { get; set; }

        /// <summary>
        /// Yakıt Türü: Benzinli-Dizel-Benzin+LPG
        /// </summary>
        public ENUMYakitTuru yakitTuru { get; set; }

        /// <summary>
        /// Yakıt Türü: Benzinli-Dizel-Benzin+LPG
        /// </summary>
        public string yakitTuruAd
        {
            get
            {
                return YakitTuruAd(yakitTuru);
            }
        }

        /// <summary>
        /// Ait Olduğu Bütçe: Genel Bütçe-Döner Sermaye-Sosyal Tesis-Diğer
        /// </summary>
        public ENUMAitOlduguButce aitOlduguButce { get; set; }
        public string aitOlduguButceAd
        {
            get
            {
                return AitOlduguButceAd(aitOlduguButce);
            }
        }

        /// <summary>
        /// Kullanım Durumu: Faal-Faal Değil-Onarımı Ekonomik Değil
        /// </summary>
        public ENUMKullanimDurumu kullanimDurumu { get; set; }
        public string kullanimDurumuAd
        {
            get
            {
                return KullanimDurumuAd(kullanimDurumu);
            }
        }

        /// <summary>
        /// Onay Kodu: Onay Kodu
        /// </summary>
        public int onayKodu { get; set; }
        public string onayKoduAd
        {
            get
            {
                if (onayKodu == 5) return "Onaylı";
                if (onayKodu == 2) return "Değişiklik Yapılıyor";
                return "Yeni";
            }
        }

        public static TNSCollection YakitTurleri(bool bosEkle)
        {
            TNSCollection tc = new TNSCollection();
            if (bosEkle)
                tc.Add(new IntKodStrAd((int)ENUMYakitTuru.Bos, YakitTuruAd(ENUMYakitTuru.Bos)));
            tc.Add(new IntKodStrAd((int)ENUMYakitTuru.Benzinli, YakitTuruAd(ENUMYakitTuru.Benzinli)));
            tc.Add(new IntKodStrAd((int)ENUMYakitTuru.Dizel, YakitTuruAd(ENUMYakitTuru.Dizel)));
            tc.Add(new IntKodStrAd((int)ENUMYakitTuru.Benzin_LPG, YakitTuruAd(ENUMYakitTuru.Benzin_LPG)));
            return tc;
        }

        public static string YakitTuruAd(ENUMYakitTuru yakitTuru)
        {
            if (yakitTuru == ENUMYakitTuru.Benzinli) return "Benzinli";
            if (yakitTuru == ENUMYakitTuru.Dizel) return "Dizel";
            if (yakitTuru == ENUMYakitTuru.Benzin_LPG) return "LPG ve Benzinli";
            return "";
        }

        public static TNSCollection AitOlduguButceler(bool bosEkle, bool sosyalTesisOlsun = true)
        {
            TNSCollection tc = new TNSCollection();
            if (bosEkle)
                tc.Add(new IntKodStrAd((int)ENUMAitOlduguButce.Bos, AitOlduguButceAd(ENUMAitOlduguButce.Bos)));

            tc.Add(new IntKodStrAd((int)ENUMAitOlduguButce.GenelButce, AitOlduguButceAd(ENUMAitOlduguButce.GenelButce)));
            tc.Add(new IntKodStrAd((int)ENUMAitOlduguButce.DonerSermaye, AitOlduguButceAd(ENUMAitOlduguButce.DonerSermaye)));
            if (sosyalTesisOlsun)
                tc.Add(new IntKodStrAd((int)ENUMAitOlduguButce.SosyalTesis, AitOlduguButceAd(ENUMAitOlduguButce.SosyalTesis)));
            return tc;
        }
        private static string AitOlduguButceAd(ENUMAitOlduguButce aitOlduguButce)
        {
            if (aitOlduguButce == ENUMAitOlduguButce.GenelButce) return "Genel Bütçe";
            if (aitOlduguButce == ENUMAitOlduguButce.DonerSermaye) return "Döner Sermaye";
            if (aitOlduguButce == ENUMAitOlduguButce.SosyalTesis) return "Sosyal Tesis";
            return "";
        }

        public static TNSCollection KullanimDurumlari(bool bosEkle)
        {
            TNSCollection tc = new TNSCollection();
            if (bosEkle)
                tc.Add(new IntKodStrAd((int)ENUMKullanimDurumu.Bos, KullanimDurumuAd(ENUMKullanimDurumu.Bos)));

            tc.Add(new IntKodStrAd((int)ENUMKullanimDurumu.Faal, KullanimDurumuAd(ENUMKullanimDurumu.Faal)));
            tc.Add(new IntKodStrAd((int)ENUMKullanimDurumu.FaalDegil, KullanimDurumuAd(ENUMKullanimDurumu.FaalDegil)));
            tc.Add(new IntKodStrAd((int)ENUMKullanimDurumu.OnarimiEkonomikDegil, KullanimDurumuAd(ENUMKullanimDurumu.OnarimiEkonomikDegil)));
            //tc.Add(new IntKodStrAd((int)ENUMKullanimDurumu.Ayrildi, KullanimDurumuAd(ENUMKullanimDurumu.Ayrildi)));

            return tc;
        }

        private static string KullanimDurumuAd(ENUMKullanimDurumu kullanimDurumu)
        {
            if (kullanimDurumu == ENUMKullanimDurumu.Faal) return "Faal";
            if (kullanimDurumu == ENUMKullanimDurumu.FaalDegil) return "Faal Değil";
            if (kullanimDurumu == ENUMKullanimDurumu.OnarimiEkonomikDegil) return "Onarımı Ekonomik Değil";
            //if (kullanimDurumu == ENUMKullanimDurumu.Ayrildi) return "Çıkışı yapıldı";
            return "";
        }

        private static string AracCikisSebebiAd(ENUMAracCikisSebebi kullanimDurumu)
        {
            if (kullanimDurumu == ENUMAracCikisSebebi.DIGER) return "Diğer";
            if (kullanimDurumu == ENUMAracCikisSebebi.HURDA) return "Hurdaya Ayrıldı";
            if (kullanimDurumu == ENUMAracCikisSebebi.TERKIN) return "Terkin Edildi";
            if (kullanimDurumu == ENUMAracCikisSebebi.KURUMDISIDEVIR) return "Kurum Dışı Devir";
            if (kullanimDurumu == ENUMAracCikisSebebi.KURUMICIDEVIR) return "Kurum içi Devir";
            if (kullanimDurumu == ENUMAracCikisSebebi.SATIS) return "Satış";
            return "";
        }

        public TNSCollection hareketler = new TNSCollection();

        private TasitBilgiDetay _aktifDetay = null;
        public TasitBilgiDetay AktifDetay
        {
            get
            {
                if (_aktifDetay == null)
                    _aktifDetay = new TasitBilgiDetay();
                return _aktifDetay;
            }
        }

        public string plakaNo
        {
            get
            {
                return AktifDetay.plakaNo;
            }
            set
            {
                AktifDetay.plakaNo = value;
            }
        }


        public int bulunduguSaymanlik { get { return AktifDetay.bulunduguSaymanlik; } set { AktifDetay.bulunduguSaymanlik = value; } }
        public string bulunduguSaymanlikAd { get { return AktifDetay.bulunduguSaymanlikAd; } set { AktifDetay.bulunduguSaymanlikAd = value; } }

        /// <summary>
        /// Bulunduğu Birim: Bulunduğu Birim
        /// </summary>
        public string bulunduguBirim { get { return AktifDetay.bulunduguBirim; } set { AktifDetay.bulunduguBirim = value; } }
        public string bulunduguBirimAd { get { return AktifDetay.bulunduguBirimAd; } set { AktifDetay.bulunduguBirimAd = value; } }
        public string bulunduguYerAd { get { return TNS.MUH.Arac.BirimAdOl(bulunduguSaymanlikAd, bulunduguBirimAd); } }


        /// <summary>
        /// Ait Olduğu Saymanlık: Ait Olduğu Saymanlık
        /// </summary>
        public int aitOlduguSaymanlik { get { return AktifDetay.aitOlduguSaymanlik; } set { AktifDetay.aitOlduguSaymanlik = value; } }
        public string aitOlduguSaymanlikAd { get { return AktifDetay.aitOlduguSaymanlikAd; } set { AktifDetay.aitOlduguSaymanlikAd = value; } }

        /// <summary>
        /// Ait Olduğu Birim: Ait Olduğu Birim
        /// </summary>
        public string aitOlduguBirim { get { return AktifDetay.aitOlduguBirim; } set { AktifDetay.aitOlduguBirim = value; } }
        public string aitOlduguBirimAd { get { return AktifDetay.aitOlduguBirimAd; } set { AktifDetay.aitOlduguBirimAd = value; } }
        public string aitOlduguYerAd { get { return TNS.MUH.Arac.BirimAdOl(aitOlduguSaymanlikAd, aitOlduguBirimAd); } }

        public void HareketEkle(TasitBilgiDetay tbd)
        {
            if (tbd.siraNo == 0) _aktifDetay = tbd;
            hareketler.Add(tbd);
        }

        public TNSDateTime girisTarih { get { return AktifDetay.girisTarih; } set { AktifDetay.girisTarih = value; } }

        public DateTime girisTarihDT { get { return AktifDetay.girisTarihDT; } set { AktifDetay.girisTarihDT = value; } }

        public ENUMAracCikisSebebi cikisSebebi { get { return AktifDetay.cikisSebebi; } set { AktifDetay.cikisSebebi = value; } }

        public string cikisEvrakSayisi { get { return AktifDetay.cikisEvrakSayisi; } set { AktifDetay.cikisEvrakSayisi = value; } }

        public DateTime cikisTarihDT { get { return AktifDetay.cikisTarihDT; } set { AktifDetay.cikisTarihDT = value; } }

        public TNSDateTime cikisTarih { get { return AktifDetay.cikisTarih; } set { AktifDetay.cikisTarih = value; } }

        public string tasitCetveliKodu { get; set; }

        public byte[] ruhsatResim { get; set; }

        public string ruhsatResimAd { get; set; }

        public string aracSinifi { get; set; }

        public string modelGirilen { get; set; }

        public int kesin { get; set; }

        public bool resimVar { get; set; }

        public int motorsuz { get; set; }
    }

    /// <summary>
    /// TasitBilgiAna: 
    /// </summary>
    [System.Serializable()]
    public class TasitBilgiDetay
    {
        /// <summary>
        /// Bulunduğu Saymanlık: Bulunduğu Saymanlık
        /// </summary>
        public int bulunduguSaymanlik { get; set; }
        public string bulunduguSaymanlikAd { get; set; }

        /// <summary>
        /// Bulunduğu Birim: Bulunduğu Birim
        /// </summary>
        public string bulunduguBirim { get; set; }
        public string bulunduguBirimAd { get; set; }
        public string bulunduguYerAd { get { return TNS.MUH.Arac.BirimAdOl(bulunduguSaymanlikAd, bulunduguBirimAd); } }


        /// <summary>
        /// Ait Olduğu Saymanlık: Ait Olduğu Saymanlık
        /// </summary>
        public int aitOlduguSaymanlik { get; set; }
        public string aitOlduguSaymanlikAd { get; set; }

        /// <summary>
        /// Ait Olduğu Birim: Ait Olduğu Birim
        /// </summary>
        public string aitOlduguBirim { get; set; }
        public string aitOlduguBirimAd { get; set; }
        public string aitOlduguYerAd { get { return TNS.MUH.Arac.BirimAdOl(aitOlduguSaymanlikAd, aitOlduguBirimAd); } }

        /// <summary>
        /// Plaka No: Plaka No
        /// </summary>
        public string plakaNo { get; set; }

        public TNSDateTime girisTarih { get; set; }
        public DateTime girisTarihDT
        {
            get
            {
                if (girisTarih == null)
                    return DateTime.MinValue;
                else
                    return girisTarih.Oku();
            }
            set
            {
                if (girisTarih == null) girisTarih = new TNSDateTime(value);
                else girisTarih.Yaz(value);
            }
        }

        public TNSDateTime cikisTarih { get; set; }
        public DateTime cikisTarihDT
        {
            get
            {
                if (cikisTarih == null)
                    return DateTime.MinValue;
                else
                    return cikisTarih.Oku();
            }
            set
            {
                if (cikisTarih == null) cikisTarih = new TNSDateTime(value);
                else cikisTarih.Yaz(value);
            }
        }

        public ENUMAracCikisSebebi cikisSebebi { get; set; }

        public int siraNo { get; set; }

        public string cikisEvrakSayisi { get; set; }
    }

    public enum ENUMTasitIslemTur : int
    {
        KurumIciDevir = 1,
        KurumDisiDevir = 2,
        Terkin = 3,
        Hurda = 4,
        Bilinmiyor = 0
    }
    /// <summary>
    /// TasitIslemTalep: 
    /// </summary>
    [System.Serializable()]
    public class TasitIslemTalep
    {
        public string kod { get { return saseNo + "-" + talepNo; } }

        /// <summary>
        /// Şase No: Şase No
        /// </summary>
        public string saseNo { get; set; }

        /// <summary>
        /// Talep No: Talep Numarası
        /// </summary>
        public int talepNo { get; set; }

        /// <summary>
        /// Talep Tür: Devir/Terkin Talep
        /// </summary>
        public ENUMTasitIslemTur talepTur { get; set; }

        /// <summary>
        /// Talep Tür: Devir/Terkin Talep
        /// </summary>
        public string talepTurAd
        {
            get
            {
                if (talepTur == ENUMTasitIslemTur.Terkin)
                    return "Terkin";
                if (talepTur == ENUMTasitIslemTur.KurumDisiDevir)
                    return "Kurum Dışı Devir";
                if (talepTur == ENUMTasitIslemTur.KurumIciDevir)
                    return "Kurum İçi Devir";
                return "";
            }
        }

        /// <summary>
        /// Gideceği Birim: Gideceği Birim
        /// </summary>
        public string gidecegiBirim { get; set; }

        /// <summary>
        /// Gideceği Saymanlık: Gideceği Saymanlık
        /// </summary>
        public int gidecegiSaymanlik { get; set; }

        /// <summary>
        /// Sahiplik Değişmesin: Araç Sahipliği Değişmesin
        /// </summary>
        public int sahiplikDegismesin { get; set; }

        /// <summary>
        /// Ayrıldığı Tarih: Ayrıldığı Tarih
        /// </summary>
        public TNSDateTime ayrildigiTarih { get; set; }
        public DateTime ayrildigiTarihDT
        {
            get
            {
                if (ayrildigiTarih == null)
                    return DateTime.MinValue;
                else
                    return ayrildigiTarih.Oku();
            }
            set
            {
                if (ayrildigiTarih == null) ayrildigiTarih = new TNSDateTime(value);
                else ayrildigiTarih.Yaz(value);
            }
        }

        /// <summary>
        /// Ayrıldığı Evrak Sayı: Ayrıldığı Evrak Sayı
        /// </summary>
        public string ayrildigiEvrakSayi { get; set; }

        /// <summary>
        /// Onay Kodu: Onay Kodu
        /// </summary>
        public int onayKodu { get; set; }

        /// <summary>
        /// Talep Yapan: Talep Yapan
        /// </summary>
        public string talepYapan { get; set; }

        /// <summary>
        /// İşlem Yapan: İşlem Yapan
        /// </summary>
        public string islemYapan { get; set; }

        public string gidecegiSaymanlikAd { get; set; }

        public string gidecegiBirimAd { get; set; }

        public string gidecegiYerAd { get { return TNS.MUH.Arac.BirimAdOl(gidecegiSaymanlikAd, gidecegiBirimAd); } }

        public int talepSaymanlik { get; set; }

        public string talepBirim { get; set; }

        public string talepSaymanlikAd { get; set; }

        public string talepBirimAd { get; set; }

        public string talepYerAd { get { return TNS.MUH.Arac.BirimAdOl(talepSaymanlikAd, talepBirimAd); } }

        public string plakaNo { get; set; }
    }

    /// <summary>
    /// KiralikArac: Sözleşmede bulunan araç bilgileri
    /// </summary>
    [System.Serializable()]
    public class KiralikArac
    {
        public bool dsMi;
        public int tasitAy { get { return TasitAyBul(sure, sureBirimi, adet); } }
        public double birimFiyatAy { get { return BirimFiyatAyBul(sureBirimi, birimFiyat); } }

        public static double BirimFiyatAyBul(ENUMSureBirim SUREBIRIMI, double BIRIMFIYAT)
        {
            if (SUREBIRIMI == ENUMSureBirim.Yil)
            {
                BIRIMFIYAT = BIRIMFIYAT / 12;
            }

            else if (SUREBIRIMI == ENUMSureBirim.TakvimGunu)
            {
                BIRIMFIYAT = BIRIMFIYAT * 30;
            }
            else if (SUREBIRIMI == ENUMSureBirim.IsGunu)
            {
                BIRIMFIYAT = BIRIMFIYAT * 22;
            }

            return BIRIMFIYAT;
        }


        public static int TasitAyBul(int SURE, ENUMSureBirim SUREBIRIMI, int ADET)
        {
            if (SUREBIRIMI == ENUMSureBirim.Yil)
            {
                SURE = SURE * 12;
            }

            else if (SUREBIRIMI == ENUMSureBirim.TakvimGunu)
            {
                SURE = SURE / 30;
            }
            else if (SUREBIRIMI == ENUMSureBirim.IsGunu)
            {
                SURE = SURE / 22;
            }

            if (SURE > 12) SURE = 12;
            return ADET * SURE;
        }

        public string sureBirimiAd
        {
            get { return SureBirimAd(this.sureBirimi); }
        }

        public string tasitCinsiAd { get; set; }
        public string tasitCinsiAdTKOD { get { return tasitTKod + "- " + tasitCinsiAd; } }

        public string tasitTKod { get; set; }
        public int tasitTipKodu { get; set; }
        public string tasitTipAd { get; set; }

        /// <summary>
        /// Araç Sıra No: Sözleşmedeki aracın numarası (Sistem tarafından verilen otomatik numara)
        /// </summary>
        public int aracSiraNo { get; set; }

        /// <summary>
        /// Taşıt Cinsi: Dosya Parçası
        /// </summary>
        public int tasitCinsi { get; set; }

        /// <summary>
        /// En Büyük Yaş: Kiralanan aracın olabileceği en büyük yaş
        /// </summary>
        public int enBuyukYas { get; set; }

        /// <summary>
        /// Koltuk Sayısı: Kiralanan aracın koltuk sayısı
        /// </summary>
        public int koltukSayisi { get; set; }

        /// <summary>
        /// Yakıt Türü: Kiralanan aracın yakıt türü
        /// </summary>
        public ENUMYakitTuru yakitTuru { get; set; }

        public string yakitTuruAd
        {

            get
            {
                return TasitBilgi.YakitTuruAd(yakitTuru);
            }
        }
        /// <summary>
        /// Şoför Dahil: Şoför dahil olarak kiralanacaksa işaret konulmalı
        /// </summary>
        public int soforDahil { get; set; }

        public string soforDahilAd { get { return BoolAd(soforDahil); } }

        /// <summary>
        /// Yakıt Dahil: Yakıt dahil olarak kiralanacaksa işaret konulmalı
        /// </summary>
        public int yakitDahil { get; set; }
        public string yakitDahilAd { get { return BoolAd(yakitDahil); } }

        public static string BoolAd(int boolKod)
        {
            if (boolKod == 1) return "Evet";
            return "Hayır";
        }

        /// <summary>
        /// Adet: Bu araç cinsinden kiralanan araç sayısı
        /// </summary>
        public int adet { get; set; }

        /// <summary>
        /// Süre: Bu araç cinsinden kiralanan bir aracın ne kadar süre için kiralandığı
        /// </summary>
        public int sure { get; set; }

        /// <summary>
        /// Süre Birimi: Kiralama süresinin birimi (Yıl, Ay, Takvim Günü, İş Günü)
        /// </summary>
        public ENUMSureBirim sureBirimi { get; set; }

        public string sureAd { get { return sure.ToString() + " " + SureBirimAd(sureBirimi); } }
        /// <summary>
        /// Birim Fiyat: Bir aracın birim fiyatı
        /// </summary>
        public double birimFiyat { get; set; }
        public string pebKod { get; set; }
        public double Tutar
        {
            get
            {
                return birimFiyat * sure * adet;
            }
        }

        public string pebAd { get; set; }

        public static object SureBirimleri(bool bosEkle)
        {
            TNSCollection tc = new TNSCollection();
            if (bosEkle)
                tc.Add(new IntKodStrAd((int)ENUMSureBirim.Bos, SureBirimAd(ENUMSureBirim.Bos)));
            tc.Add(new IntKodStrAd((int)ENUMSureBirim.Yil, SureBirimAd(ENUMSureBirim.Yil)));
            tc.Add(new IntKodStrAd((int)ENUMSureBirim.Ay, SureBirimAd(ENUMSureBirim.Ay)));
            tc.Add(new IntKodStrAd((int)ENUMSureBirim.TakvimGunu, SureBirimAd(ENUMSureBirim.TakvimGunu)));
            tc.Add(new IntKodStrAd((int)ENUMSureBirim.IsGunu, SureBirimAd(ENUMSureBirim.IsGunu)));
            return tc;
        }

        private static string SureBirimAd(ENUMSureBirim sureBirim)
        {
            if (sureBirim == ENUMSureBirim.Yil) return "Yıl";
            if (sureBirim == ENUMSureBirim.Ay) return "Ay";
            if (sureBirim == ENUMSureBirim.TakvimGunu) return "Gün (Tatiller Dahil)";
            if (sureBirim == ENUMSureBirim.IsGunu) return "Gün (Tatiller Hariç)";
            return "";
        }

        public string FaaliyetAd
        {
            get
            {
                if (dsMi) return "Döner Sermaye";
                else return "Bütçe";
            }
        }
    }

    /// <summary>
    /// KiralikAracSozlesmesi: Araç kiralamak için yapılan sözleşmelerin girildiği ekran
    /// </summary>
    [System.Serializable()]
    public class KiralikAracSozlesmesi
    {
        /// <summary>
        /// Yıl: Sözleşmenin İmzalandığı Yıl
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// Sözleşme Kod: Sözleşme Kodu (Sistem tarafından verilen otomatik kod)
        /// </summary>
        public int sozlesmeKod { get; set; }

        /// <summary>
        /// Saymanlık No: Sözleşmeyi Yapan Birimin Saymanlık Kodu
        /// </summary>
        public int saymanlikNo { get; set; }

        /// <summary>
        /// Birim Kod: Sözleşmeyi Yapan Birimin Birim Kodu
        /// </summary>
        public string birimKod { get; set; }

        /// <summary>
        /// İmza Tarihi: Sözleşmenin İmzalandığı Tarih
        /// </summary>
        public TNSDateTime imzaTarihi { get; set; }
        public DateTime imzaTarihiDT
        {
            get
            {
                if (imzaTarihi == null)
                    return DateTime.MinValue;
                else
                    return imzaTarihi.Oku();
            }
            set
            {
                if (imzaTarihi == null) imzaTarihi = new TNSDateTime(value);
                else imzaTarihi.Yaz(value);
            }
        }

        /// <summary>
        /// Sözleşme No: Sözleşme No
        /// </summary>
        public string sozlesmeNo { get; set; }

        /// <summary>
        /// İş Başlangıç Tarihi: İşin Başlayacağı Tarih
        /// </summary>
        public TNSDateTime isBaslangicTarihi { get; set; }
        public DateTime isBaslangicTarihiDT
        {
            get
            {
                if (isBaslangicTarihi == null)
                    return DateTime.MinValue;
                else
                    return isBaslangicTarihi.Oku();
            }
            set
            {
                if (isBaslangicTarihi == null) isBaslangicTarihi = new TNSDateTime(value);
                else isBaslangicTarihi.Yaz(value);
            }
        }

        /// <summary>
        /// İş Bitiş Tarihi: İşin Biteceği Tarih
        /// </summary>
        public TNSDateTime isBitisTarihi { get; set; }
        public DateTime isBitisTarihiDT
        {
            get
            {
                if (isBitisTarihi == null)
                    return DateTime.MinValue;
                else
                    return isBitisTarihi.Oku();
            }
            set
            {
                if (isBitisTarihi == null) isBitisTarihi = new TNSDateTime(value);
                else isBitisTarihi.Yaz(value);
            }
        }

        /// <summary>
        /// Firma Kod: Sözleşmenin kiminle imzalandığı
        /// </summary>
        public int firmaKod { get; set; }

        /// <summary>
        /// Sözleşme Bedeli: Sözleşme Bedeli
        /// </summary>
        public double sozlesmeBedeliTop { get { return sozlesmeBedeliButce + sozlesmeBedeliDoner; } }

        public double sozlesmeBedeliButce { get { return SozlemeBedeli(true); } }
        public double sozlesmeBedeliDoner { get { return SozlemeBedeli(false); } }

        private double SozlemeBedeli(bool butce)
        {
            double toplam = 0;
            foreach (KiralikArac item in kiralikArac)
            {
                if (butce && !item.dsMi || !butce && item.dsMi)
                    toplam += item.Tutar;
            }
            return toplam;
        }

        /// <summary>
        /// Bütçe Türü: Bütçe Türü: Bütçe, Döner Sermaye Yatırım, Döner Sermaye Cari
        /// </summary>
        //public int butceTuru { get; set; }

        /// <summary>
        /// İlçeler Dahil: Eğer sadece il merkezinde değil de ilçelerde de kullanılacaksa işaret konulmalı
        /// </summary>
        public int ilcelerDahil { get; set; }

        /// <summary>
        /// Süreç Yıl: İlgili sürecin yılı
        /// </summary>
        public int surecYil { get; set; }

        /// <summary>
        /// Süreç No: İlgili sürecin süreç numarası
        /// </summary>
        public string surecNo { get; set; }

        /// <summary>
        /// KiralikArac: Sözleşmede bulunan araç bilgileri
        /// </summary>
        public TNSCollection kiralikArac;
        public string saymanlikNoBirim { get { return saymanlikNo + "-" + birimKod; } }

        public string firmaAd { get; set; }
        public string birimAd { get { return TNS.KYM.Arac.BirimAdOl(muhasebeAd, muhBirimAd); } }
        public string muhasebeAd { get; set; }
        public string muhBirimAd { get; set; }
        public double ToplamTutarBul
        {
            get
            {
                double toplam = 0;
                if (kiralikArac != null)
                {
                    foreach (KiralikArac item in kiralikArac)
                    {
                        toplam += item.Tutar;
                    }
                }
                return toplam;
            }
        }
    }


    /// <summary>
    /// DepremHarcamaDetay: 
    /// </summary>
    [System.Serializable()]
    public class DepremHarcamaDetay
    {
        /// <summary>
        /// EkoKod: Ekonomik Kodu Giriniz
        /// </summary>
        public string ekoKod { get; set; }

        /// <summary>
        /// Ad: Açıklama
        /// </summary>
        public string ad { get; set; }

        /// <summary>
        /// Bütçe İçi: Deprem için yapılan bütçe içi harcama tutarı.
        /// </summary>
        public double butceIci { get; set; }

        /// <summary>
        /// Bütçe İçi Açıklama: Deprem için yapılan bütçe içi harcama açıklaması (50 Harf).
        /// </summary>
        public string butceIciAciklama { get; set; }

        /// <summary>
        /// HT1: Deprem için yapılan Özel Hesap harcama tutarı
        /// </summary>
        public double hT1 { get; set; }

        /// <summary>
        /// HT2: Deprem için yapılan Döner Sermaye harcama tutarı
        /// </summary>
        public double hT2 { get; set; }

        /// <summary>
        /// HT3: Deprem için yapılan Fon harcama tutarı
        /// </summary>
        public double hT3 { get; set; }

        /// <summary>
        /// HT4: Deprem için yapılan Sosyal Tesis harcama tutarı
        /// </summary>
        public double hT4 { get; set; }

        /// <summary>
        /// HT5: Deprem için yapılan Diğer harcama tutarı
        /// </summary>
        public double hT5 { get; set; }

        /// <summary>
        /// Bütçe Dışı Açıklama: Deprem için yapılan bütçe dışı harcamalara ait açıklama (50 Harf).
        /// </summary>
        public string butceDisiAciklama { get; set; }

        public bool HataliRakamVarMi()
        {
            return (butceIci < 0 || hT1 < 0 || hT2 < 0 || hT3 < 0 || hT4 < 0 || hT5 < 0);
        }

        public bool RakamVarMi()
        {
            return (butceIci > 0 || hT1 > 0 || hT2 > 0 || hT3 > 0 || hT4 > 0 || hT5 > 0);
        }

        public bool aciklamaVarMi()
        {
            return (!string.IsNullOrWhiteSpace(butceIciAciklama) || !string.IsNullOrWhiteSpace(butceDisiAciklama));
        }
    }


    /// <summary>
    /// DepremHarcamaAna: 
    /// </summary>
    [System.Serializable()]
    public class DepremHarcamaAna
    {

        /// <summary>
        /// Yıl: Harcama yılını giriniz.
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// Ay: Harcama ayını giriniz.
        /// </summary>
        public int ay { get; set; }

        /// <summary>
        /// Muhasebe Kod: Muhasebe Kodunu giriniz.
        /// </summary>
        public int muhasebeKod { get; set; }

        /// <summary>
        /// Birim Kod: Birim kodunu giriniz.
        /// </summary>
        public string birimKod { get; set; }

        /// <summary>
        /// İl Kod: İl kodunu giriniz.
        /// </summary>
        public int ilKod { get; set; }
        public int durum { get; set; }
        public string kaydeden { get; set; }
        public TNSDateTime kayitTarihi { get; set; }
        public string onaylanan { get; set; }
        public TNSDateTime onayTarihi { get; set; }

        /// <summary>
        /// DepremHarcamaDetay: 
        /// </summary>
        public TNSCollection depremHarcamaDetay = new TNSCollection();

        public void KodlariYaz(List<StrKodStrAd> depKodlar)
        {
            foreach (var item in depKodlar)
            {
                DepremHarcamaDetay detay = new DepremHarcamaDetay();
                detay.ekoKod = item.kod;
                detay.ad = item.ad;
                depremHarcamaDetay.Add(detay);
            }
        }

        public void DepremHarcamaDetayBulEkle(DepremHarcamaDetay detay)
        {
            foreach (DepremHarcamaDetay item in depremHarcamaDetay)
            {
                if (item.ekoKod == "" || item.ekoKod == detay.ekoKod.Substring(0, 2) || item.ekoKod == detay.ekoKod)
                {
                    item.butceIci += detay.butceIci;
                    item.hT1 += detay.hT1;
                    item.hT2 += detay.hT2;
                    item.hT3 += detay.hT3;
                    item.hT4 += detay.hT4;
                    item.hT5 += detay.hT5;
                    if (item.ekoKod == detay.ekoKod)
                    {
                        item.butceIciAciklama = detay.butceIciAciklama;
                        item.butceDisiAciklama = detay.butceDisiAciklama;
                    }
                }
            }
        }

        public bool Equals(DepremHarcamaAna gelen)
        {
            if (gelen != null && this.yil == gelen.yil && this.ay == gelen.ay && this.muhasebeKod == gelen.muhasebeKod && this.birimKod == gelen.birimKod && this.ilKod == gelen.ilKod)
                return true;
            return false;
        }

        public void RakamlarGecerliMi(Sonuc sonuc)
        {
            foreach (DepremHarcamaDetay item in depremHarcamaDetay)
            {
                if (item.HataliRakamVarMi()) sonuc.DegerAta($"{item.ad} satırında hatalı rakam var.");
                if (!item.RakamVarMi() && item.aciklamaVarMi()) sonuc.DegerAta($"{item.ad} satırında rakam olmamasına rağmen açıklama girilmiş.");
            }
        }
    }

    /// <summary>
    /// Bütçe hazırlık formu sunucu yordamlarının tanımlarını tutan arayüz.
    /// </summary>
    public interface IBHFServis
    {
        /// <summary>
        /// Bir birimin satın almayı planladığı taşıtları listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Taşıtların listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Taşıtların listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Verilen yılda, birimin satın almayı planladığı taşıtların listesi, listeyi ve sonucu içeren yapıda döndürülür.</returns>
        EdinilecekTasitSatiri[] EdinilecekTasitlarListele(Kullanici kullanan, int yil, string kurumsalKod);

        /// <summary>
        /// Bir birimin satın almayı planladığı taşıtları kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Taşıtların kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Taşıtların kaydedilen birimin kurumsal kodu.</param>
        /// <param name="edinilecekTasitlar">Satın alınması planlanan taşıtların EdinilecekTasitSatiri nesnesi içeren listesi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc EdinilecekTasitlarKaydet(Kullanici kullanan, int yil, string kurumsalKod, TNSCollection edinilecekTasitlar);//edinilecekTasitSatir

        /// <summary>
        /// Taşıt işlemleri tarafından bümko tarafından yayınlanan taşıt sözlük bilgilerini listeleyen yordam. Taşıtlar için bu sözlük bazında veri girilecektir.
        /// </summary>
        /// <param name="yil">Taşıt sözlük bilgilerinin listeleneceği yıl bilgisi.</param>
        /// <returns>Verilen yıldaki taşıt sözlük bilgileri döndürülür.</returns>
        TasitSozluk[] TasitSozlukListele(int yil);

        /// <summary>
        /// Uluslararası kuruluş üyelik bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Uluslararası kuruluş üyelik bilgilerinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Uluslararası kuruluş üyelik bilgileri kaydedilen birimin kurumsal kodu.</param>
        /// <param name="uluslarArasiKuruluslar">Kaydedilecek üyelik bilgileri nesnelerini içeren dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc UluslarArasiKuruluslarKaydet(Kullanici kullanan, int yil, string kurumsalKod, TNSCollection uluslarArasiKuruluslar);

        /// <summary>
        /// Uluslararası kuruluş üyelik bilgilerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Uluslararası kuruluş üyelik bilgilerinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Uluslararası kuruluş üyelik bilgileri listelenen birimin kurumsal kodu.</param>
        /// <returns>Verilen yılda birimin üye olacağı uluslar arası kuruluşların listesi döndürülür.</returns>
        UluslarArasiKurulus[] UluslarArasiKuruluslarListele(Kullanici kullanan, int yil, string kurumsalKod);

        /// <summary>
        /// Birime ait gerekçe metnini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait hizmet gerekçesinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait hizmet gerekçesinin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="gerekce">Kaydedilecek gerekçe metni.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc FaaliyetGerekcesiKaydet(Kullanici kullanan, int yil, string kurumsalKod, int faaliyetKod, FaaliyetGerekce gerekce);

        /// <summary>
        /// Birime ait hizmet gerekçesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait hizmet gerekçesinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait hizmet gerekçesinin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Hizmet gerekçesi metni döndürülür.</returns>
        FaaliyetGerekce FaaliyetGerekceOku(Kullanici kullanan, int yil, string kurumsalKod, int faaliyetKod);
        ObjectArray FaaliyetGerekceListele(Kullanici kullanan, int yil, string kurumsalKod, int faaliyetKod);
        Sonuc FaaliyetGerekceBirlestir(Kullanici kullanan, int yil, string kurumsalKod);

        /// <summary>
        /// Hizmet gerekçesi girmiş olan birim sayısını bulup döndüren yordam.
        /// </summary>
        /// <param name="yil">Gerekçe sayısı bulunacak yıl.</param>
        /// <param name="kurumsalKod">Birime ait hizmet gerekçesi sayısının bulunacağı birimin kurumsal kodu.</param>
        /// <returns>Hizmet gerekçesi sayısı döndürülür.</returns>
        int HizmetGerekcesiSayisiBul(int yil, string kurumsalKod);

        /// <summary>
        /// Hizmet gerekçelerini bir önceki yıldan, alıp bu yıla kopyalayan yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait hizmet gerekçesinin kopyalanacağı hedef yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait hizmet gerekçesinin kopyalanacağı birimin kurumsal kodu.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc HizmetGerekcesiKopyalaOncYil(Kullanici kullanan, int yil, string kurumsalKod);

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
        Sonuc DonerSermayeKaydet(Kullanici kullanan, int yil, string kurumsalKod, int donerSermayeNo, string donerSermayeAd, TNSCollection donerSermaye);

        /// <summary>
        /// Döner sermaye ve fon bilgilerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait döner sermaye veya fon bilgilerinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait döner sermaye veya fon bilgilerinin listeleneceği birimin kurumsal kodu.</param>
        /// <param name="donerSermayeNo">Listelenecek döner sermaye veya fon bilgisinin sıra numarası. (0 ise fon, birden başlıyorsa döner sermayedir.)</param>
        /// <returns>Birimin verilen sıradaki döner sermaye veya fona ait tanım ve ödenekleri döndürülür.</returns>
        DonerSermaye DonerSermayeListele(Kullanici kullanan, int yil, string kurumsalKod, int donerSermayeNo);

        /// <summary>
        /// Bir birimin sahip olduğu fiziksel değerleri kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait fiziksel değerlerin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait fiziksel değerlerin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="fizikselDegerler">Kaydedilecek fiziksel değer satırları.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc FizikselDegerlerKaydet(Kullanici kullanan, int yil, string kurumsalKod, TNSCollection fizikselDegerler);

        /// <summary>
        /// Bir birimin sahip olduğu fiziksel değerlerin tutulduğu satırları listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait fiziksel değerlerin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait fiziksel değerlerin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>fiziksel değerlerin tutulduğu satırlar döndürülür.</returns>
        ObjectArray FizikselDegerlerListele(Kullanici kullanan, int yil, string kurumsalKod);

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
        Sonuc HizmetBilgisiKaydet(Kullanici kullanan, int yil, string kurumsalKod, int tamamMiIlkMi, int hizmetYili, TNSCollection hizmetBilgisiSatirlari);

        /// <summary>
        /// Yeni başlıyacak hizmetlerin ekonomik kod bazında maliyetleri ve bitirilecek hizmetlerin ekonomik kod bazında maliyetlerinin listelendiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birime ait tamamlanacak yada başlayacak hizmetlerin maliyetlerinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait tamamlanacak yada başlayacak hizmetlerin maliyetlerinin listeleneceği birimin kurumsal kodu.</param>
        /// <param name="tamamMiIlkMi">1 ise ilk defa yapılması planlanan hizmetler, 1 değil ise tamamlanması planlanan hizmetler</param>
        /// <param name="hizmetYili">3 yıllık süreçdeki hizmetlerle ilgili olduğundan hizmet yılı, yıl, yıl+1, yıl+2 değerleri almalıdır.</param>
        /// <returns>Verilen kriterlere uyan hizmetlerin maliyetlerinin tutulduğu satırlar döndürülür.</returns>
        ObjectArray HizmetBilgisiListele(Kullanici kullanan, int yil, string kurumsalKod, int tamamMiIlkMi, int hizmetYili);

        /// <summary>
        /// Birimlerin hizmetlerini gerçekleştirmek amacıyla yaptıkları işlemlerin detaylandırıldığı formun detaylarını satır satır alıp kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birim hizmetlerinin maliyetlerinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birim hizmetlerin maliyetlerinin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="hizmetMaliyeti">Hizmet maliyetinin tesbiti için kullanılan satırlar dizisi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc HizmetMaliyetiTesbitiKaydet(Kullanici kullanan, int yil, string kurumsalKod, TNSCollection hizmetMaliyeti);

        /// <summary>
        /// Birimlerin hizmetlerini gerçekleştirmek amacıyla yaptıkları işlemlerin detaylandırıldığı formun detaylarını satır satır listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Birim hizmetlerinin maliyetlerinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birim hizmetlerin maliyetlerinin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Hizmet maliyeti tesbitinde kullanılan satırlar döndürülür.</returns>
        ObjectArray HizmetMaliyetiTesbitiListele(Kullanici kullanan, int yil, string kurumsalKod);

        /// <summary>
        /// Tavan üstüne sığmayan ödeneklerin, açıklamalarıyla birlikte kaydedildiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Tavan üstüne sığmayan ödeneklerin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Tavan üstüne sığmayan ödeneklerin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="TavanUstuOdenekSatirlari">Kaydedilecek, açıklama ve ekonomik kodları tutan satır dizisi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc TavanUstuKaydet(Kullanici kullanan, int yil, string kurumsalKod, TNSCollection TavanUstuOdenekSatirlari);

        /// <summary>
        /// Tavan üstüne sığmayan ödeneklerin, açıklamalarıyla birlikte listelendiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Tavan üstüne sığmayan ödeneklerin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Tavan üstüne sığmayan ödeneklerin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Tavan üstüne sığmayan kullanılan ödeneklerin bilgileri satırlar halinde döndürülür.</returns>
        ObjectArray TavanUstuListele(Kullanici kullanan, int yil, string kurumsalKod);

        /// <summary>
        /// Tavan üstüne sığmayan ödeneklerin, detay bazda girilenlerinden hareketle üreten yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Tavan üstüne sığmayan ödeneklerin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Tavan üstüne sığmayan ödeneklerin listeleneceği birimin kurumsal kodu.</param>
        /// <param name="senaryoKod">Tavan üstüne sığmayan ödeneklerin listeleneceği senaryo kodu.</param>
        /// <returns>Tavan üstüne sığmayan kullanılan ödeneklerin bilgileri satırlar halinde döndürülür.</returns>
        ObjectArray TavanUstuListeleDetaydan(Kullanici kullanan, int yil, string kurumsalKod, string senaryoKod, bool YirmiBes2);

        /// <summary>
        /// Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin detaylandırıldığı bilgileri kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="TavanUstuOdenekSatirlari">Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin miktarlarını ve açıklamalarını tutan satırların dizisi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc TavanUstuKaydetDetay(Kullanici kullanan, int yil, string kurumsalKod, TNSCollection TavanUstuOdenekSatirlari);

        /// <summary>
        /// Tavan üstüne sığmayan ödeneklerin 03.2 ve 03.5 ekonomik kodlu olanlarının detaylandırıldığı bilgileri listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Tavan üstüne sığmayan 03.2 ve 03.5 ekonomik kodlu ödeneklerin bilgileri satırlar halinde döndürülür.</returns>
        ObjectArray TavanUstuListeleDetay(Kullanici kullanan, int yil, string kurumsalKod);

        /// <summary>
        /// Özel güvenlik ihaleleriyle ilgili bilgilerin kaydedildiği yordam. İhale bilgisi OzelGuvenlikBilgi nesnesi olarak verilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Özel güvenlik ihaleleriyle ilgili bilgilerin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Özel güvenlik ihaleleriyle ilgili bilgilerin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="ihaleNo">Özel güvenlik ihalelesinin birim içi sıra numarası.</param>
        /// <param name="ihale">Birimin verilen sıradaki ihalesinin bilgilerini tutan yapı.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc OzelGuvenlikKaydet(Kullanici kullanan, int yil, string kurumsalKod, int ihaleNo, OzelGuvenlikBilgi ihale);

        /// <summary>
        /// Özel güvenlik ihaleleriyle ilgili bilgilerin OzelGuvenlikBilgi nesnesi olarak döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Özel güvenlik ihalelesinin bilgisinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Özel güvenlik ihalelesinin bilgisinin listeleneceği birimin kurumsal kodu.</param>
        /// <param name="ihaleNo">Özel güvenlik ihalelesinin birim içi sıra numarası. 0 verilirse tüm ihaleler listelenir.</param>
        /// <returns>Birimin verilen sıradaki ihalesinin yada tüm ihalelerinin bilgileri döndürülür.</returns>
        OzelGuvenlikBilgi[] OzelGuvenlikMaliyetOku(Kullanici kullanan, int yil, string kurumsalKod, int ihaleNo);

        /// <summary>
        /// Özel güvenlik ihaleleriyle ilgili bilgilerin sadece sıra numarası ve ihale adı döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Özel güvenlik ihalelesinin kimlik bilgisinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Özel güvenlik ihalelesinin kimlik bilgisinin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Özel güvenlik ihaleleriyle ilgili bilgilerin sadece sıra numarası ve ihale adı OzelGuvenlikBilgi yapısında döndürülür.</returns>
        ObjectArray OzelGuvenlikListele(Kullanici kullanan, int yil, string kurumsalKod);

        /// <summary>
        /// Mevcut taşıtlar, tasfiye edilecek taşıtlar, hizmet alımı ile kullanılacak taşıt sayısı ve birim maliyetinin gibi taşıtlarla ilgili genel bilgilerin kaydedildiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Taşıt genel bilgilerinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Taşıt genel bilgilerinin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="tasitGenelBilgi">Taşıt genel bilgilerinin tutulduğu dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc KullanilanTasitlarKaydet(Kullanici kullanan, int yil, string kurumsalKod, int merkezTasra, TNSCollection tasitGenelBilgi);

        /// <summary>
        /// Mevcut taşıtlar, tasfiye edilecek taşıtlar, hizmet alımı ile kullanılacak taşıt sayısı ve birim maliyetinin gibi taşıtlarla ilgili genel bilgilerin listelendiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Taşıt genel bilgilerinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Taşıt genel bilgilerinin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Taşıt genel bilgileri dizisi döndürülür.</returns>
        ObjectArray KullanilanTasitlarListele(Kullanici kullanan, int yil, string kurumsalKod, int merkez1Tasra2Toplam3);

        /// <summary>
        /// Hizmet alımı ile edinilecek taşıtların şoförlü/şoförsüz, yakıtlı/yakıtsız miktar ve maliyetlerinin kaydedildiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Hizmet alımı ile edinilecek taşıtların bilgilerinin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Hizmet alımı ile edinilecek taşıtların bilgilerinin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="hizmetAlimiTasitlariDetay">Hizmet alımı ile edinilecek taşıtların bilgilerinin tutulduğu dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc HizmetAlimiTasitlariDetayKaydet(Kullanici kullanan, int yil, string kurumsalKod, TNSCollection hizmetAlimiTasitlariDetay, bool kullanilan);

        /// <summary>
        /// Hizmet alımı ile edinilecek taşıtların şoförlü/şoförsüz, yakıtlı/yakıtsız miktar ve maliyetlerinin listelendiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Hizmet alımı ile edinilecek taşıtların bilgilerinin listelendiği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Hizmet alımı ile edinilecek taşıtların bilgilerinin listelendiği birimin kurumsal kodu.</param>
        /// <returns>Hizmet alımı ile edinilecek taşıtların bilgilerinin tutulduğu dizi döndürülür.</returns>
        ObjectArray HizmetAlimiTasitlariDetayListele(Kullanici kullanan, int yil, string kurumsalKod, bool kullanilan);

        /// <summary>
        /// Hizmet alımı ile edinilecek görevlilerin hizmet türleri itibariyle miktar ve maliyetlerinin kaydedildiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Hizmet alımı ile edinilecek görevlilerin bilgilerinin kaydedildiği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Hizmet alımı ile edinilecek görevlilerin bilgilerinin kaydedildiği birimin kurumsal kodu.</param>
        /// <param name="hizmetAlimiGorevli">Hizmet alımı ile edinilecek görevlilerin bilgilerini tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc HizmetAlimiGorevliKaydet(Kullanici kullanan, int yil, string kurumsalKod, TNSCollection hizmetAlimiGorevli);

        /// <summary>
        /// Hizmet alımı ile edinilecek görevlilerin hizmet türleri itibariyle miktar ve maliyetlerinin listelendiği yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Hizmet alımı ile edinilecek görevlilerin bilgilerinin listelendiği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Hizmet alımı ile edinilecek görevlilerin bilgilerinin listelendiği birimin kurumsal kodu.</param>
        /// <returns>Hizmet alımı ile edinilecek görevlilerin bilgilerinin tutulduğu dizi döndürülür.</returns>
        ObjectArray HizmetAlimiGorevliListele(Kullanici kullanan, int yil, string kurumsalKod);

        /// <summary>
        /// Temizlik hizmeti ihaleleriyle ilgili bilgilerin kaydedildiği yordam. İhale bilgisi OzelGuvenlikBilgi nesnesi olarak verilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Temizlik hizmeti ihaleleriyle ilgili bilgilerin kaydedileceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Temizlik hizmeti ihaleleriyle ilgili bilgilerin kaydedileceği birimin kurumsal kodu.</param>
        /// <param name="ihaleNo">Temizlik hizmeti ihalelesinin birim içi sıra numarası.</param>
        /// <param name="ihale">Birimin verilen sıradaki ihalesinin bilgilerini tutan yapı.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc TemizlikKaydet(Kullanici kullanan, int yil, string kurumsalKod, int ihaleNo, TemizlikBilgi ihale);

        /// <summary>
        /// Temizlik hizmeti ihaleleriyle ilgili bilgilerin OzelGuvenlikBilgi nesnesi olarak döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Temizlik hizmeti ihalelesinin bilgisinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Temizlik hizmeti ihalelesinin bilgisinin listeleneceği birimin kurumsal kodu.</param>
        /// <param name="ihaleNo">Temizlik hizmeti ihalelesinin birim içi sıra numarası. 0 verilirse tüm ihaleler listelenir.</param>
        /// <returns>Birimin verilen sıradaki ihalesinin yada tüm ihalelerinin bilgileri döndürülür.</returns>
        TemizlikBilgi[] TemizlikMaliyetOku(Kullanici kullanan, int yil, string kurumsalKod, int ihaleNo);

        /// <summary>
        /// eBütçeden çekilen taşıt sözlük bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="yil">Sözlük bilgilerinin ait olduğu yıl.</param>
        /// <param name="tasitSozlukler">eBütçeden çekilen taşıt sözlük bilgilerini tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc TasitSozlukKaydet(Kullanici kullanan, int yil, TNSCollection tasitSozlukler);

        /// <summary>
        /// eBütçeden çekilen fiziksel değer sözlük bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="fizikselDegerlerSozlukler">eBütçeden çekilen fiziksel değer sözlük bilgilerini tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc FizikselDegerlerSozlukKaydet(Kullanici kullanan, TNSCollection fizikselDegerlerSozlukler);

        /// <summary>
        /// eBütçeden çekilen hizmet maliyetinin tesbiti sözlük bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="hizmetMaliyetiSozlukler">eBütçeden çekilen hizmet maliyetinin tesbiti sözlük bilgilerini tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc HizmetMaliyetiSozlukKaydet(Kullanici kullanan, TNSCollection hizmetMaliyetiSozlukler);

        /// <summary>
        /// Hizmet maliyeti ile ilgili sözlük bilgilerini listeleyen yordam.
        /// </summary>
        /// <returns>Bulunan sözlük bilgileri dizi olarak döndürülür.</returns>
        HizmetMaliyetiSatir[] HizmetMaliyetiSozlukListele();

        /// <summary>
        /// Döner sermaye ile ilgili sözlük bilgilerini listeleyen yordam.
        /// </summary>
        /// <returns>Bulunan sözlük bilgileri dizi olarak döndürülür.</returns>
        DonerSermayeSatir[] DonerSermayeSozlukListele();

        /// <summary>
        /// Fiziksel değerler ile ilgili sözlük bilgilerini listeleyen yordam.
        /// </summary>
        /// <returns>Bulunan sözlük bilgileri dizi olarak döndürülür.</returns>
        FizikselDeger[] FizikselDegerlerSozlukListele();

        /// <summary>
        /// Döner sermaye sözlük bilgilerini silen ve verilen dizideki sözlük bilgilerini ekleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="dssler">Kaydedilecek DonerSermayeSatir tipinde nesneleri tutan dizi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc DonerSermayeSozlukKaydet(Kullanici kullanan, TNSCollection dssler);


        Sonuc HizmetMaliyetiTasraKaydet(Kullanici kullanan, int p, int muhasebeKod, string birimKod, string kurumsalKod, TNSCollection hmc);

        ObjectArray HizmetMaliyetiTasraListele(Kullanici kullanan, int p, int muhasebeKod, string birimKod, string kurumsalKod, bool kurumsalToplam);

        /// <summary>
        /// TasitMarkaModel listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitMarkaModel yapısında TasitMarkaModel: İstatistik Kodu bilgilerini tutan değişken</param>
        ObjectArray ListeleTasitMarkaModel(Kullanici kullanan, TasitMarkaModel form);

        /// <summary>
        /// TasitMarkaModel nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitMarkaModel yapısında TasitMarkaModel: İstatistik Kodu bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetTasitMarkaModel(Kullanici kullanan, TasitMarkaModel form);

        ObjectArray ListeleMarkaModel(Kullanici kullanan, TasitMarkaModel form);

        ObjectArray ListeleTasitTip(Kullanici kullanan, TasitTip form);

        Sonuc KaydetTasitTip(Kullanici kullanan, TasitTip form);

        ObjectArray ListeleTasitCins(Kullanici kullanan, TasitCins form);

        Sonuc KaydetTasitCins(Kullanici kullanan, TasitCins form);

        /// <summary>
        /// TasitBilgi listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitBilgi yapısında TasitBilgi:  bilgilerini tutan değişken</param>
        ObjectArray ListeleTasitBilgi(Kullanici kullanan, TasitBilgi form);

        /// <summary>
        /// TasitBilgi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitBilgi yapısında TasitBilgi:  bilgilerini tutan değişken</param>
        ObjectVAN OkuTasitBilgi(Kullanici kullanan, TasitBilgi form);

        /// <summary>
        /// TasitBilgi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">TasitBilgi yapısında TasitBilgi:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetTasitBilgi(Kullanici kullanan, TasitBilgi form);

        Sonuc OnaylaTasitBilgi(Kullanici kullanan, TNSCollection formlar, int onayKod);

        Sonuc SilTasitBilgi(Kullanici kullanan, TasitBilgi form);

        ObjectVAN TasitIslemTalepOkuBekleyen(Kullanici kullanan, string saseNo);

        ObjectArray TasitIslemTalepListele(Kullanici kullanan, string saseNo, bool bekleyen);

        Sonuc TasitIslemTalepKaydet(Kullanici kullanan, TasitIslemTalep form);

        Sonuc TasitMarkaModelTipGuncelle(Kullanici kullanan, string tipAd, string marka, string modelBasla);

        Sonuc TasitIslemTalepOnayla(Kullanici kullanan, TNSCollection talepListesi, int onayKod);

        /// <summary>
        /// Ruhsat resminin kayıt edilmesi işlemini gerçekleştirir
        /// </summary>
        /// <param name="bilgi">Resme ait bilgileri saklar</param>
        /// <returns>Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.</returns>
        Sonuc TasitRuhsatResimKaydet(Kullanici kullanan, string saseNo, string resimAd, byte[] resim);

        /// <summary>
        /// KiralikAracSozlesmesi listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KiralikAracSozlesmesi yapısında KiralikAracSozlesmesi: Araç kiralamak için yapılan sözleşmelerin girildiği ekran bilgilerini tutan değişken</param>
        ObjectArray ListeleKiralikAracSozlesme(Kullanici kullanan, KiralikAracSozlesmesi form);

        /// <summary>
        /// KiralikAracSozlesmesi nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KiralikAracSozlesmesi yapısında KiralikAracSozlesmesi: Araç kiralamak için yapılan sözleşmelerin girildiği ekran bilgilerini tutan değişken</param>
        ObjectVAN OkuKiralikAracSozlesme(Kullanici kullanan, KiralikAracSozlesmesi form);

        /// <summary>
        /// KiralikAracSozlesmesi nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">KiralikAracSozlesmesi yapısında KiralikAracSozlesmesi: Araç kiralamak için yapılan sözleşmelerin girildiği ekran bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetKiralikAracSozlesme(Kullanici kullanan, KiralikAracSozlesmesi form);
        Sonuc KaydetKiralikArac(Kullanici kullanan, int yil, int sozlesmeKod, KiralikArac kiralikArac);
        ObjectVAN OkuKiralikArac(Kullanici kullanan, int yil, int sozlesmeKod, int aracSiraNo);
        Sonuc SilKiralikArac(Kullanici kullanan, int yil, int sozlesmeKod, int aracSiraNo);
        Sonuc SilKiralikAracSozlesme(Kullanici kullanan, KiralikAracSozlesmesi form);
        ObjectArray KiralikAracRaporla(Kullanici kullanan, int yil, int saymanlikNo, string birimKod);

        /// <summary>
        /// DepremHarcamaAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DepremHarcamaAna yapısında DepremHarcamaAna:  bilgilerini tutan değişken</param>
        ObjectArray ListeleDepremHarcamaForm(Kullanici kullanan, DepremHarcamaAna form);

        /// <summary>
        /// DepremHarcamaAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DepremHarcamaAna yapısında DepremHarcamaAna:  bilgilerini tutan değişken</param>
        ObjectVAN OkuDepremHarcamaForm(Kullanici kullanan, DepremHarcamaAna form);

        /// <summary>
        /// DepremHarcamaAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">DepremHarcamaAna yapısında DepremHarcamaAna:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetDepremHarcamaForm(Kullanici kullanan, DepremHarcamaAna form);
        Sonuc OnaylaDepremHarcamaForm(Kullanici kullanan, DepremHarcamaAna form);
        Sonuc OncekiAydanKopyalaDepremHarcamaForm(Kullanici kullanan, DepremHarcamaAna form);
        /// <summary>
        /// AracKiralamaPlanAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AracKiralamaPlanAna yapısında AracKiralamaPlanAna:  bilgilerini tutan değişken</param>
        ObjectArray ListeleAracKiralamaPlani(Kullanici kullanan, AracKiralamaPlanAna form);

        /// <summary>
        /// AracKiralamaPlanAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AracKiralamaPlanAna yapısında AracKiralamaPlanAna:  bilgilerini tutan değişken</param>
        ObjectVAN OkuAracKiralamaPlani(Kullanici kullanan, AracKiralamaPlanAna form);

        /// <summary>
        /// AracKiralamaPlanAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">AracKiralamaPlanAna yapısında AracKiralamaPlanAna:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetAracKiralamaPlani(Kullanici kullanan, AracKiralamaPlanAna form);
        Sonuc OnaylaAracKiralamaPlani(Kullanici kullanan, AracKiralamaPlanAna form, bool kayitYoksaEkle);
    }
}
