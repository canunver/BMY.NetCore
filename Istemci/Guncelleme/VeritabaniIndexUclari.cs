using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;
using TNS.UZY;

namespace Istemci.Guncelleme
{
    public class IndexSatiri
    {
        public string TabloAd { get; set; }
        public string IndexAd { get; set; }
    }

    public class IndexIslemIstegi
    {
        /// <summary>appSettings'teki bağlantı satırı adı (ör. BaglantiSatiriEVR).</summary>
        public string Baglanti { get; set; }
        /// <summary>rebuild | reorganize | drop</summary>
        public string Islem { get; set; }
        public List<IndexSatiri> Indexler { get; set; }
    }

    public class IndexOlusturIstegi
    {
        public string Baglanti { get; set; }
        public string IndexAd { get; set; }
    }

    /// <summary>
    /// Veritabanı Index İşlemleri sayfasının (eski Evrak/EvrakVeritabaniIndex.aspx) uçları. Yalnızca sistem yöneticisi.
    ///   GET  /api/vtindex/baglantilar           : appSettings'teki bağlantı satırları (veri tabanı adı ve ayar adı)
    ///   GET  /api/vtindex/liste?baglanti=&amp;tip=  : indexler; Nonclustered ise Evrak için tanımlı indexler ve var olup olmadıkları
    ///   POST /api/vtindex/islem                 : seçilen indexleri rebuild / reorganize / drop eder
    ///   POST /api/vtindex/olustur               : tanımlı bir indexi oluşturur (sayfa seçilenleri teker teker gönderip ilerlemeyi gösterir)
    /// İşlemler UZY servisindedir (VeritabaniIndexListele, VeritabaniIndexIslem, VeritabaniBaglantiSatiriListele).
    /// Eskisinden farklı olarak işlem yapılacak indexler tarayıcıdan gelen bilgilerle değil, veri tabanından yeniden okunan
    /// listeden alınır; oluşturulacak indexin sütunları da sunucudaki tanımdan gelir.
    /// </summary>
    public static class VeritabaniIndexUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/vtindex/baglantilar", Baglantilar);
            app.MapGet("/api/vtindex/liste", Liste);
            app.MapPost("/api/vtindex/islem", Islem);
            app.MapPost("/api/vtindex/olustur", Olustur);
        }

        private static IResult Denetle(HttpContext baglam, out Kullanici kullanan)
        {
            return GuncellemeOrtak.Denetle(baglam, GuncellemeOrtak.Yonetici, out kullanan);
        }

        /// <summary>Bağlantı satırı adına karşılık gelen veri tabanı adı; ad listede yoksa null.</summary>
        private static string VeritabaniBul(Kullanici kullanan, string baglanti)
        {
            if (string.IsNullOrWhiteSpace(baglanti)) return null;
            Hashtable tablo = TNS.UZY.Arac.Tanimla().VeritabaniBaglantiSatiriListele(kullanan);
            foreach (DictionaryEntry e in tablo)
                if (string.Equals(e.Value + "", baglanti, StringComparison.Ordinal) && (e.Key + "") != "*") return e.Key + "";
            return null;
        }

        /// <summary>Index Oluştur sekmesi yalnızca evrak veri tabanında açıktır (eski TabStrip1.Items[1].Disabled).</summary>
        private static bool OlusturAcik(string veritabani)
        {
            return (veritabani ?? "").ToLowerInvariant() == "evrak";
        }

        private static IResult Baglantilar(HttpContext baglam)
        {
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            Hashtable tablo = TNS.UZY.Arac.Tanimla().VeritabaniBaglantiSatiriListele(kullanan);
            List<KeyValuePair<string, string>> liste = new List<KeyValuePair<string, string>>();
            string hata = null;
            foreach (DictionaryEntry e in tablo)
            {
                if ((e.Key + "") == "*") { hata = e.Value + ""; continue; }
                liste.Add(new KeyValuePair<string, string>(e.Key + "", e.Value + ""));
            }
            var satirlar = liste.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase).Select(k => new { veritabani = k.Key, baglanti = k.Value });
            return Results.Json(new { satirlar = satirlar, hata = hata });
        }

        private static string TipDuzelt(string tip)
        {
            return tip == "Clustered" || tip == "Hepsi" ? tip : "Nonclustered";
        }

        private static IResult Liste(HttpContext baglam, string baglanti, string tip)
        {
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            string veritabani = VeritabaniBul(kullanan, baglanti);
            if (veritabani == null) return GuncellemeOrtak.Hata("Bağlantı satırını seçiniz.");
            tip = TipDuzelt(tip);

            VeritabaniIndex[] liste = TNS.UZY.Arac.Tanimla().VeritabaniIndexListele(kullanan, new VeritabaniIndex { indexTip = tip }, baglanti);
            if (liste != null && liste.Length == 1 && liste[0].objectId == -2)
                return GuncellemeOrtak.Hata(liste[0].tabloAd);
            liste = liste ?? new VeritabaniIndex[0];

            List<object> satirlar = liste.Select(i => (object)new
            {
                tabloAd = i.tabloAd,
                indexAd = i.indexAd,
                indexTip = i.indexTip,
                indexSize = i.indexSize,
                fragmentation = i.fragmentation,
                fillFactor = i.fillFactor
            }).ToList();

            // Eski Listele: Index Oluştur listesi yalnızca Nonclustered listelenince yenilenir.
            List<object> yeniler = null;
            if (tip == "Nonclustered")
                yeniler = YeniIndexListesi().Select(y => (object)new
                {
                    tabloAd = y.tabloAd,
                    indexAd = y.indexAd,
                    indexTip = y.indexTip,
                    kolon = y.kolon,
                    include = y.include,
                    durum = liste.Any(i => i.indexAd == y.indexAd) ? 1 : 0
                }).ToList();

            return Results.Json(new
            {
                basarili = true,
                veritabani = veritabani,
                olusturAcik = OlusturAcik(veritabani),
                satirlar = satirlar,
                yeniler = yeniler
            });
        }

        private static async Task<IResult> Islem(HttpContext baglam, IndexIslemIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Indexler == null || istek.Indexler.Count == 0)
                return GuncellemeOrtak.Hata("Lütfen işlem yapmak istediğiniz indexleri seçiniz.");
            if (VeritabaniBul(kullanan, istek.Baglanti) == null) return GuncellemeOrtak.Hata("Bağlantı satırını seçiniz.");

            ENUMVeritabaniIndexIslem islem = ENUMVeritabaniIndexIslem.TANIMSIZ;
            if (istek.Islem == "rebuild") islem = ENUMVeritabaniIndexIslem.REBUILD;
            else if (istek.Islem == "reorganize") islem = ENUMVeritabaniIndexIslem.REORGANIZE;
            else if (istek.Islem == "drop") islem = ENUMVeritabaniIndexIslem.DROP;
            if (islem == ENUMVeritabaniIndexIslem.TANIMSIZ) return GuncellemeOrtak.Hata("İşlem tanımsız.");

            IUZYServis servis = TNS.UZY.Arac.Tanimla();
            VeritabaniIndex[] mevcut = servis.VeritabaniIndexListele(kullanan, new VeritabaniIndex { indexTip = "Hepsi" }, istek.Baglanti);
            if (mevcut != null && mevcut.Length == 1 && mevcut[0].objectId == -2) return GuncellemeOrtak.Hata(mevcut[0].tabloAd);
            VeritabaniIndex[] secilen = (mevcut ?? new VeritabaniIndex[0])
                .Where(m => istek.Indexler.Any(s => s != null && s.TabloAd == m.tabloAd && s.IndexAd == m.indexAd)).ToArray();
            if (secilen.Length == 0) return GuncellemeOrtak.Hata("Seçilen indexler veri tabanında bulunamadı; listeyi yenileyiniz.");

            Sonuc sonuc = servis.VeritabaniIndexIslem(kullanan, secilen, islem, istek.Baglanti);
            if (!sonuc.islemSonuc) return GuncellemeOrtak.Hata(sonuc.hataStr);
            return Results.Json(new { basarili = true, bilgi = sonuc.bilgiStr });
        }

        private static async Task<IResult> Olustur(HttpContext baglam, IndexOlusturIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            string veritabani = VeritabaniBul(kullanan, istek?.Baglanti);
            if (veritabani == null) return GuncellemeOrtak.Hata("Bağlantı satırını seçiniz.");
            if (!OlusturAcik(veritabani)) return GuncellemeOrtak.Hata("Index oluşturma yalnızca evrak veri tabanında yapılabilir.");
            VeritabaniIndex tanim = YeniIndexListesi().FirstOrDefault(y => y.indexAd == istek.IndexAd);
            if (tanim == null) return GuncellemeOrtak.Hata("Index tanımı bulunamadı.");

            Sonuc sonuc = TNS.UZY.Arac.Tanimla().VeritabaniIndexIslem(kullanan, new[] { tanim }, ENUMVeritabaniIndexIslem.CREATE, istek.Baglanti);
            if (!sonuc.islemSonuc) return GuncellemeOrtak.Hata(sonuc.hataStr);
            return Results.Json(new { basarili = true, bilgi = sonuc.bilgiStr });
        }

        /// <summary>Evrak veri tabanı için tanımlı indexler (eski EvrakVeritabaniIndex.aspx.cs YeniIndexListesi; aynen).</summary>
        private static List<VeritabaniIndex> YeniIndexListesi()
        {
            List<VeritabaniIndex> liste = new List<VeritabaniIndex>();

            liste.Add(new VeritabaniIndex() { tabloAd = "EVREVRAKHAREKETSONDURUM", indexAd = "IX_EVREVRAKHAREKETSONDURUM_BIRIMKOD1_KISI1_ISLEM", indexTip = "Nonclustered", fillFactor = 80, kolon = "[BIRIMKOD1],[KISI1],[ISLEM]", include = "" });
            liste.Add(new VeritabaniIndex() { tabloAd = "EVREVRAKHAREKETSONDURUM", indexAd = "IX_EVREVRAKHAREKETSONDURUM_BIRIMKOD2_KISI2_ISLEM", indexTip = "Nonclustered", fillFactor = 80, kolon = "[BIRIMKOD2],[KISI2],[ISLEM]", include = "" });
            liste.Add(new VeritabaniIndex() { tabloAd = "EVREVRAKDOSYAPLANI", indexAd = "IX_EVREVRAKDOSYAPLANI_DOSYAPLANKODU", indexTip = "Nonclustered", fillFactor = 80, kolon = "[DOSYAPLANKODU]", include = "[PREVRAKNO]" });
            liste.Add(new VeritabaniIndex() { tabloAd = "EVREVRAKILISKI", indexAd = "IX_EVREVRAKILISKI_PREVRAKNOILISKI", indexTip = "Nonclustered", fillFactor = 80, kolon = "[PREVRAKNOILISKI]", include = "" });
            liste.Add(new VeritabaniIndex() { tabloAd = "EVREVRAK", indexAd = "IX_EVREVRAK_YIL_EVRAKTIPI_EVRAKBIRIMKODU", indexTip = "Nonclustered", fillFactor = 80, kolon = "[YIL],[EVRAKTIPI],[EVRAKBIRIMKODU]", include = "[EVREVRAKNO]" });
            liste.Add(new VeritabaniIndex() { tabloAd = "EVREVRAK", indexAd = "IX_EVREVRAK_YIL_EVRAKBIRIMKODU_EVRAKTARIH", indexTip = "Nonclustered", fillFactor = 80, kolon = "[YIL],[EVRAKBIRIMKODU],[EVRAKTARIH]", include = "[EVREVRAKNO]" });
            liste.Add(new VeritabaniIndex() { tabloAd = "EVREVRAKSICIL", indexAd = "IX_EVREVRAKSICIL", indexTip = "Nonclustered", fillFactor = 80, kolon = "[PREVRAKNO],[EVREVRAKNO],[ASIL]", include = "" });
            liste.Add(new VeritabaniIndex() { tabloAd = "EVREVRAKHAREKET", indexAd = "IX_EVREVRAKHAREKET_ISLEM_BIRIMKOD1_KISI1", indexTip = "Nonclustered", fillFactor = 80, kolon = "[ISLEM],[BIRIMKOD1],[KISI1]", include = "[PREVRAKNO],[SIRANO],[BIRIMKOD2],[KISI2],[ISLEMYAPAN],[ISLEMTARIH],[HAVALENOTU],[OKUNDU],[KISI1AD],[KISI2AD],[ISLEMYAPANAD]" });
            liste.Add(new VeritabaniIndex() { tabloAd = "EVREVRAKHAREKET", indexAd = "IX_EVREVRAKHAREKET_BIRIMKOD1", indexTip = "Nonclustered", fillFactor = 80, kolon = "[BIRIMKOD1]", include = "[PREVRAKNO],[SIRANO]" });

            liste.Add(new VeritabaniIndex() { tabloAd = "EVRGELENYER", indexAd = "IXN_EVRGELENYER_EVRAKBIRIMKODU_YERTURU", indexTip = "Nonclustered", fillFactor = 80, kolon = "[EVRAKBIRIMKODU], [YERTURU]", include = "[YERADI], [YERBIRIMKODU], [KULLANIMSAYISI], [KALICI]" });
            liste.Add(new VeritabaniIndex() { tabloAd = "EVRHAVALELISTE", indexAd = "IXN_EVRHAVALELISTE_ISLEMYAPAN", indexTip = "Nonclustered", fillFactor = 80, kolon = "[ISLEMYAPAN]", include = "" });
            liste.Add(new VeritabaniIndex() { tabloAd = "EVREVRAKHAREKETSONDURUM", indexAd = "IXN_EVREVRAKHAREKETSONDURUM_KISI1_BIRIMKOD1_ISLEM_1", indexTip = "Nonclustered", fillFactor = 80, kolon = "[KISI1],[BIRIMKOD1], [ISLEM]", include = "[PREVRAKNO]" });
            liste.Add(new VeritabaniIndex() { tabloAd = "EVREVRAKHAREKETSONDURUM", indexAd = "IXN_EVREVRAKHAREKETSONDURUM_KISI1_BIRIMKOD1_ISLEM_2", indexTip = "Nonclustered", fillFactor = 80, kolon = "[KISI1],[BIRIMKOD1], [ISLEM]", include = "[PREVRAKNO], [SIRANO], [BIRIMKOD2], [KISI2], [ISLEMYAPAN], [ISLEMTARIH], [HAVALENOTU], [OKUNDU], [KISI1AD], [KISI2AD], [ISLEMYAPANAD]" });
            liste.Add(new VeritabaniIndex() { tabloAd = "EVREVRAKHAREKETSONDURUM", indexAd = "IXN_EVREVRAKHAREKETSONDURUM_BIRIMKOD1_KISI1_ISLEM_OKUNDU", indexTip = "Nonclustered", fillFactor = 80, kolon = "[BIRIMKOD1], [KISI1], [ISLEM], [OKUNDU]", include = "[PREVRAKNO]" });
            liste.Add(new VeritabaniIndex() { tabloAd = "EVREVRAKHAREKETSONDURUM", indexAd = "IXN_EVREVRAKHAREKETSONDURUM_KISI1_ISLEM_BIRIMKOD1", indexTip = "Nonclustered", fillFactor = 80, kolon = "[KISI1], [ISLEM],[BIRIMKOD1]", include = "[PREVRAKNO]" });
            liste.Add(new VeritabaniIndex() { tabloAd = "EVREVRAK", indexAd = "IXN_EVREVRAK_EVRAKBIRIMKODU_KONU", indexTip = "Nonclustered", fillFactor = 80, kolon = "[EVRAKBIRIMKODU], [KONU]", include = "" });
            liste.Add(new VeritabaniIndex() { tabloAd = "EVREVRAKHAREKET", indexAd = "IXN_EVREVRAKHAREKET_BIRIMKOD2_KISI2_ISLEM_1", indexTip = "Nonclustered", fillFactor = 80, kolon = "[BIRIMKOD2], [KISI2], [ISLEM]", include = "[PREVRAKNO]" });
            liste.Add(new VeritabaniIndex() { tabloAd = "EVREVRAKHAREKET", indexAd = "IXN_EVREVRAKHAREKET_BIRIMKOD2_KISI2_ISLEM_2", indexTip = "Nonclustered", fillFactor = 80, kolon = "[BIRIMKOD2], [KISI2], [ISLEM]", include = "[PREVRAKNO], [SIRANO], [BIRIMKOD1], [KISI1], [ISLEMYAPAN], [ISLEMTARIH], [HAVALENOTU], [OKUNDU], [KISI1AD], [KISI2AD], [ISLEMYAPANAD]" });

            return liste;
        }
    }
}
