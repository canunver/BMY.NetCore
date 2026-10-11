using System.Collections.Generic;

namespace Istemci.Organizasyon
{
    /// <summary>Yazılım yetkisi penceresindeki bir alanın türü.</summary>
    public enum YetkiAlanTur
    {
        /// <summary>Serbest metin.</summary>
        Yazi,
        /// <summary>Onay kutusu; damga doğru/yanlış olarak saklanır.</summary>
        Evet,
        /// <summary>Onay kutusu; işaretliyse damgaya belirli bir sayı (AcikDeger), değilse 0 yazılır.</summary>
        EvetSayi,
        /// <summary>Tek seçim (radyo); damga sayı olarak saklanır.</summary>
        Radyo,
        /// <summary>Açılır listeden tek seçim; damga metin olarak saklanır.</summary>
        Secim,
        /// <summary>Listeden çoklu seçim; seçilen kodlar virgülle ayrılarak saklanır.</summary>
        Coklu
    }

    /// <summary>Yazılım yetkisi penceresindeki bir alan: hangi damgaya yazıldığı ve nasıl gösterildiği.</summary>
    public class YetkiAlan
    {
        /// <summary>Yetkinin damgalarındaki anahtar (eski sayfalardaki DamgaEkle / DegerAl adı). Değiştirilmemeli.</summary>
        public string Damga;
        public YetkiAlanTur Tur;
        public string Etiket;
        /// <summary>Aynı gruptaki onay kutuları tek satırda, bu başlık altında gösterilir.</summary>
        public string Grup;
        /// <summary>Seçenekleri sağlayan yetki tipinin kodu (KYM YetkiTipiGirisListesi); 0 ise Secenekler kullanılır.</summary>
        public int YetkiTipi;
        public KeyValuePair<string, string>[] Secenekler;
        /// <summary>Radyo için varsayılan değer.</summary>
        public int Varsayilan;
        /// <summary>EvetSayi için işaretliyken yazılan değer.</summary>
        public int AcikDeger;
    }

    /// <summary>
    /// Yazılım yetkilerinin alan tanımları. Eski sistemde her yetki türünün ayrı bir sayfası vardı
    /// (Organizasyon/YETxxx.aspx; hangi sayfanın açılacağı BirimGorev.SayfaAd ile belirlenir). Bu sayfaların hepsi aynı
    /// kalıptaydı: açıklama + birkaç alan + Kaydet; alanlar yetkinin "damga"larına yazılır. Burada o sayfaların alanları
    /// tablo olarak tutulur ve tek bir ortak pencereyle gösterilir. Etiketler eski sayfalardaki gibidir.
    /// Yeni bir yetki türü eklendiğinde buraya SayfaAd'ıyla bir satır eklemek yeterlidir.
    /// </summary>
    public static class YetkiTanimlari
    {
        private static YetkiAlan Yazi(string damga, string etiket)
        {
            return new YetkiAlan { Damga = damga, Tur = YetkiAlanTur.Yazi, Etiket = etiket };
        }

        private static YetkiAlan Evet(string damga, string etiket, string grup)
        {
            return new YetkiAlan { Damga = damga, Tur = YetkiAlanTur.Evet, Etiket = etiket, Grup = grup };
        }

        private static YetkiAlan Coklu(string damga, string etiket, int yetkiTipi)
        {
            return new YetkiAlan { Damga = damga, Tur = YetkiAlanTur.Coklu, Etiket = etiket, YetkiTipi = yetkiTipi };
        }

        private static KeyValuePair<string, string> S(string kod, string ad)
        {
            return new KeyValuePair<string, string>(kod, ad);
        }

        private static YetkiAlan[] Birimler(string grup)
        {
            return new[] { Evet("Merkez", "Merkez", grup), Evet("Tasra", "Taşra", grup), Evet("Yurtdisi", "Yurtdışı", grup) };
        }

        private static YetkiAlan[] GorYazOnay()
        {
            return new[] { Evet("Oku", "Görür", "Yetki"), Evet("Yaz", "Yazar", "Yetki"), Evet("Onayla", "Onaylar", "Yetki") };
        }

        private static YetkiAlan[] GorYazSil()
        {
            return new[] { Evet("Oku", "Sadece Görür", "Yetki"), Evet("Yaz", "Sadece Yazar", "Yetki"), Evet("Siler", "Görür, Yazar, Siler", "Yetki") };
        }

        private static YetkiAlan[] YoneticiGorevli()
        {
            return new[] { Evet("Yonetici", "Yönetici", "Görev"), Evet("Gorevli", "Görevli", "Görev") };
        }

        private static YetkiAlan[] Birlestir(params YetkiAlan[][] parcalar)
        {
            List<YetkiAlan> hepsi = new List<YetkiAlan>();
            foreach (YetkiAlan[] p in parcalar) hepsi.AddRange(p);
            return hepsi.ToArray();
        }

        private static YetkiAlan[] Tek(YetkiAlan alan)
        {
            return new[] { alan };
        }

        private static readonly Dictionary<string, YetkiAlan[]> tanimlar = new Dictionary<string, YetkiAlan[]>(System.StringComparer.OrdinalIgnoreCase)
        {
            { "YETODENEKGONDERME", Birlestir(Tek(Yazi("Ekonomik", "Öd. Göndereceği Ekonomik Kodlar")), Birimler("Öd. Göndereceği Birimler"), GorYazOnay()) },
            { "YETBUTCEHAZIRLIK", Birlestir(Tek(Yazi("Faaliyet", "İşlem Yapacağı Faaliyetler")), Birimler("Birimler"), GorYazOnay(),
                                             Tek(Evet("Proje", "İşlemleri Yapar", "Yat. Projesi"))) },
            { "YETBUTCEUYGULAMA", Birlestir(Birimler("Bütçe İşlem Birimleri"), GorYazOnay()) },
            { "YETYATIRIMPROJELERI", Birlestir(Birimler("Yatırım Projesi Birimleri"), GorYazOnay()) },
            { "YETKESINHESAP", Birlestir(Birimler("Kesin Hesap Birimleri"), GorYazOnay()) },
            { "YETSTRATEJIKPLAN", Birlestir(Birimler("Str. Plan Birimleri"), GorYazOnay()) },
            { "YETPERFORMANSBUTCE", Birlestir(Birimler("Perf. Bütçe Birimleri"), GorYazOnay()) },
            { "YETEVRAK", new[]
                {
                    new YetkiAlan { Damga = "Gorev", Tur = YetkiAlanTur.Radyo, Etiket = "Evrak Görevi", Varsayilan = 1,
                                    Secenekler = new[] { S("1", "Evrak Yetkilisi"), S("2", "Evrak Kullanıcısı"), S("3", "Evrak Memuru") } },
                    Evet("Imza", "İmza yetkisi var", "İmza"),
                    // Belge yöneticisi, damgaya kullanıcı tipi kodu (53) olarak yazılır.
                    new YetkiAlan { Damga = "BelgeYoneticisi", Tur = YetkiAlanTur.EvetSayi, Etiket = "Belge Yöneticisi", Grup = "Belge Yöneticisi", AcikDeger = 53 },
                    Coklu("OkuIs", "Okuma İşlemleri", 121),
                    Coklu("YazIs", "Yazma İşlemleri", 122),
                    Coklu("SilIs", "Silme İşlemleri", 123)
                } },
            { "YETODEMEEMRI_MIF", Birlestir(Tek(Yazi("Ekonomik", "Öd. Keseceği Ekonomik Kodlar")), GorYazOnay(),
                                             new[] { Evet("OnayKaldir", "Onay kaldır", "Diğer"), Evet("YevmiyeIptal", "Yevmiye İptal", "Diğer") }) },
            { "YETALACAKTAKIP", GorYazOnay() },
            { "YETTEMINATMEKTUBU", GorYazSil() },
            { "YETTAAHHUTKARTI", GorYazSil() },
            { "YETKURUMPROJELERI", GorYazOnay() },
            { "YETDUNYABANKASIPROJELERI", Birlestir(YoneticiGorevli(), GorYazOnay()) },
            { "YETSAYISTAYBULGUYONETIMI", Birlestir(YoneticiGorevli(), GorYazOnay()) },
            { "YETPERSONEL", Birlestir(
                new[]
                {
                    Coklu("OkuIs", "Gör. İşlemler", 114), Coklu("YazIs", "Yap. İşlemler", 115), Coklu("OnayIs", "Onay. İşlemler", 116),
                    Coklu("GORPERTIP", "Gör. Per.Tipleri", 118), Coklu("YAZPERTIP", "İşl.Yap. Per. Tipleri", 119), Coklu("ONAPERTIP", "Onay. Per. Tipleri", 120)
                },
                GorYazOnay(),
                new[]
                {
                    Evet("KY", "Yapar", "Kullanıcı işlemleri"),
                    Yazi("KY_Liste", "Kullanıcı işlemleri görev listesi"),
                    Evet("AltBirim", "İşlemlerini Yapar", "Alt Birim"),
                    Yazi("AcilisMasa", "Açılış Sayfa (Masa Üstü)"),
                    Yazi("AcilisTab", "Açılış Sayfa (Tablet)")
                }) },
            { "YETKADRO", GorYazOnay() },
            { "YETKULLANICIYONETICISI", new YetkiAlan[0] },
            { "YETTASINIRMALKAYDI", Birlestir(GorYazOnay(), new[]
                {
                    Evet("AmortismanYapar", "Amortisman Yapar", "Yetki"),
                    Evet("MalKaydi", "Mal Kaydı", "Görev"), Evet("Konsolide", "Konsolide Yet.", "Görev"), Evet("MalYoneticisi", "Taşınır Yönetici", "Görev"),
                    Evet("KullaniciBirim", "Kullanıcı Birim", "Görev"), Evet("TuketimYilDevri", "Tüketim Yıl Devri", "Görev"),
                    new YetkiAlan { Damga = "TasinirIslemTuru", Tur = YetkiAlanTur.Secim, Etiket = "İşlem Türü",
                                    Secenekler = new[] { S("1", "Demirbaş"), S("2", "Gayrimenkul"), S("3", "Yazılım"), S("4", "Personel") } },
                    Yazi("MuhasebeHarcamaYetki", "Birim Yetkisi"),
                    Yazi("TasinirAmbarKodu", "Ambar Kodu")
                }) },
            { "YETSISTEMYONETICISI", new YetkiAlan[0] },
            { "YETKUTUPHANE", Birlestir(Tek(Yazi("Kutuphaneler", "Kütüphane kodları")), GorYazOnay()) },
            { "YETKIMLIKPAYLASIMI", new YetkiAlan[0] },
            { "YETGOREVYONETIMSISTEMI", Tek(new YetkiAlan { Damga = "Rol", Tur = YetkiAlanTur.Secim, Etiket = "Yetki", YetkiTipi = 112 }) },
            { "YETSATINALMA", new YetkiAlan[0] },
            { "YETHARCAMA", new[]
                {
                    Coklu("Görebileceği Adımlar", "Gör. İşlemler", 127),
                    Coklu("Yapabileceği Adımlar", "Yap. İşlemler", 126),
                    Coklu("Onaylayabileceği Adımlar", "Onay. İşlemler", 128)
                } }
        };

        /// <summary>
        /// Sayfa adından (ör. "YETEVRAK.aspx") alan tanımları; tanımı olmayan (henüz eklenmemiş) bir yetki türü için null.
        /// </summary>
        public static YetkiAlan[] Bul(string sayfaAd)
        {
            if (string.IsNullOrEmpty(sayfaAd)) return null;
            int nokta = sayfaAd.IndexOf('.');
            string ad = nokta >= 0 ? sayfaAd.Substring(0, nokta) : sayfaAd;
            YetkiAlan[] alanlar;
            return tanimlar.TryGetValue(ad, out alanlar) ? alanlar : null;
        }
    }
}
