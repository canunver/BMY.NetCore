using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Istemci.ButceHazirlik;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>
    /// Çapraz Tablo (eski ButceMuhasebe/CaprazTablo.aspx) uçları. Satır hesap kodları ile sütun türüne (hesap kodu / ekonomik kod /
    /// kurumsal kod) göre ödeme emri / MİF tutarları Excel'de çapraz tablo olarak dökülür; satır, sütun ve sorgu alanları adıyla
    /// saklanabilir (MUHCAPRAZTABLO). İş kuralları MUH servisinde (CaprazTabloKaydet / Sil / Listele / RaporListele /
    /// RaporHesapKodListele2); bu sınıf eski code-behind'ın yerini alır.
    ///   GET  /api/capraztablo/tanim     : yıllar, durumlar, sütun türleri (eski Page_Load)
    ///   GET  /api/capraztablo/liste     : kayıtlı sorgular (eski btnListele_Click)
    ///   GET  /api/capraztablo/getir     : kayıtlı sorgunun satırları, sütunları ve sorgu alanları (eski GetirDetay)
    ///   POST /api/capraztablo/kaydet    : kaydet / farklı kaydet (eski Kaydet; json biçimi eskisiyle aynı)
    ///   POST /api/capraztablo/sil       : kayıtlı sorguyu siler (eski btnSil_Click)
    ///   GET  /api/capraztablo/yazdir    : Tablo Göster, CaprazTablo.xltx (eski btnYazdir_Click, YazdirEkoKur, YazdirHesapKod)
    ///   GET  /api/capraztablo/kodlar    : kurumsal / fonksiyonel / finans / ekonomik / faaliyet kod ağacı (eski nodeLoadPar)
    ///   GET  /api/capraztablo/hesap/agac, hesap/ara, hesap/ad : hesap kodu alanı (Sgb.hesapKodu)
    /// Sorgu alanı adları eski PropertyGrid adlarıdır (yil, durum, muhasebe, birim, yevmiyeTarihi1/2, belgeTarihi1/2, ilgiliKod,
    /// althesapNo, kur, fon, fin, eko, peb); tarihler istemciyle yyyy-MM-dd.
    /// </summary>
    public static class CaprazTabloUclari
    {
        public const string SayfaAdres = "../ButceMuhasebe/CaprazTablo.aspx";
        public const string Sablon = "CaprazTablo.xltx";
        private const string Uc = "/api/capraztablo/";

        /// <summary>Saklanan sorgu alanları (eski pgFiltre.getSource() anahtarları, aynı sırayla).</summary>
        private static readonly string[] FiltreAlanlari =
        {
            "yil", "durum", "muhasebe", "birim", "yevmiyeTarihi1", "yevmiyeTarihi2", "belgeTarihi1", "belgeTarihi2",
            "ilgiliKod", "althesapNo", "kur", "fon", "fin", "eko", "peb"
        };

        private static readonly HashSet<string> TarihAlanlari = new HashSet<string>
        {
            "yevmiyeTarihi1", "yevmiyeTarihi2", "belgeTarihi1", "belgeTarihi2"
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "liste", Liste);
            app.MapGet(Uc + "getir", Getir);
            app.MapPost(Uc + "kaydet", Kaydet);
            app.MapPost(Uc + "sil", Sil);
            app.MapGet(Uc + "yazdir", Yazdir);
            app.MapGet(Uc + "kodlar", Kodlar);
            app.MapGet(Uc + "hesap/agac", HesapAgac);
            app.MapGet(Uc + "hesap/ara", HesapAra);
            app.MapGet(Uc + "hesap/ad", HesapAd);
        }

        // ------------------------------------------------------------------ yetki ve ortak

        /// <summary>Menü yetkisi ve eski Page_Load girişi (ödeme emri görme yetkisi).</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (!SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdres)) return false;
            try
            {
                return TNS.MUH.Yetki.YetkisiVarMi(kullanan, (int)ENUMOGY.Gorme, (int)ENUMBelgeTuru.ODEMEEMRI);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = Temiz(hata);
            return hata == "" ? varsayilan : hata;
        }

        private static int HesapYil(int? yil)
        {
            int y = yil ?? 0;
            return y <= 2000 ? DateTime.Now.Year : y;
        }

        private static readonly string[] TarihBicimleri =
        {
            "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ss.fff", "yyyy-MM-ddTHH:mm:ssK", "yyyy-MM-ddTHH:mm:ss.fffK",
            "dd.MM.yyyy", "d.M.yyyy", "dd.MM.yyyy HH:mm:ss", "d.M.yyyy HH:mm:ss", "dd/MM/yyyy", "MM/dd/yyyy", "M/d/yyyy"
        };

        /// <summary>Tarih metni (yyyy-MM-dd, eski Ext tarih kodlaması ya da gg.aa.yyyy) -> tarih; boş ya da çözülemezse null.</summary>
        private static DateTime? TarihCoz(string s)
        {
            s = Temiz(s);
            if (s == "") return null;
            if (DateTime.TryParseExact(s, TarihBicimleri, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t)) return t.Date;
            if (DateTime.TryParse(s, new CultureInfo("tr-TR"), DateTimeStyles.None, out t)) return t.Date;
            return null;
        }

        private static TNSDateTime TnsTarih(string s)
        {
            DateTime? t = TarihCoz(s);
            return t.HasValue ? new TNSDateTime(t.Value) : new TNSDateTime();
        }

        // ------------------------------------------------------------------ açılış (eski Page_Load)

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            try
            {
                List<int> yillar = new List<int>();
                for (int i = 2006; i <= DateTime.Now.Year; i++)
                    yillar.Add(i);

                // Eski durumStore (Resources.ButceMuhasebe FRMOES002-007)
                List<object> durumlar = new List<object>
                {
                    new { kod = 1, ad = "Yeni Belgeler" },
                    new { kod = 5, ad = "Onaylı" },
                    new { kod = 23, ad = "Yevmiyesiz" },
                    new { kod = 6, ad = "Yevmiyeli" },
                    new { kod = 9, ad = "İptal Edilenler" },
                    new { kod = 22, ad = "Bütün Belgeler (İptaller Hariç)" }
                };
                List<object> sutunTurleri = new List<object>
                {
                    new { kod = 1, ad = "Hesap Kod" },
                    new { kod = 2, ad = "Ekonomik Kod" },
                    new { kod = 3, ad = "Kurumsal Kod" }
                };

                return Results.Json(new
                {
                    yillar = yillar,
                    yil = DateTime.Now.Year,
                    durumlar = durumlar,
                    varsayilanDurum = 22,
                    sutunTurleri = sutunTurleri
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Sayfa bilgileri okunamadı."));
            }
        }

        // ------------------------------------------------------------------ kayıtlı sorgular

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray bilgiler = TNS.MUH.Arac.Tanimla().CaprazTabloListele(kullanan, new TNS.MUH.CaprazTablo());
                if (bilgiler == null || !bilgiler.sonuc.islemSonuc)
                    return Hata(HataMetni(bilgiler?.sonuc.hataStr, "Kayıtlı sorgular okunamadı."));

                List<object> satirlar = new List<object>();
                foreach (TNS.MUH.CaprazTablo item in bilgiler.objeler)
                    satirlar.Add(new { kod = item.kod, adi = item.adi ?? "", kayitEden = item.kayitEden ?? "" });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kayıtlı sorgular okunamadı."));
            }
        }

        /// <summary>Saklanan json parçasındaki satır / sütun kayıtları ({KOD, HESAPKOD} dizisi).</summary>
        private static List<CaprazTabloKod> KodlariOku(string json)
        {
            List<CaprazTabloKod> liste = new List<CaprazTabloKod>();
            json = Temiz(json);
            if (json == "") return liste;
            using (JsonDocument d = JsonDocument.Parse(json))
            {
                IEnumerable<JsonElement> ogeler = d.RootElement.ValueKind == JsonValueKind.Array
                    ? d.RootElement.EnumerateArray()
                    : new[] { d.RootElement };
                foreach (JsonElement e in ogeler)
                {
                    if (e.ValueKind != JsonValueKind.Object) continue;
                    liste.Add(new CaprazTabloKod { KOD = AlanMetni(e, "KOD"), HESAPKOD = AlanMetni(e, "HESAPKOD") });
                }
            }
            return liste;
        }

        private static string AlanMetni(JsonElement e, string ad)
        {
            if (!e.TryGetProperty(ad, out JsonElement v)) return "";
            switch (v.ValueKind)
            {
                case JsonValueKind.String: return v.GetString() ?? "";
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False: return v.GetRawText();
                default: return "";
            }
        }

        /// <summary>Saklanan sorgu alanları (eski pgFiltre.getSource()); tarihler yyyy-MM-dd'ye çevrilir.</summary>
        private static Dictionary<string, string> FiltreOku(string json)
        {
            Dictionary<string, string> filtre = new Dictionary<string, string>();
            json = Temiz(json);
            if (json == "") return filtre;
            using (JsonDocument d = JsonDocument.Parse(json))
            {
                JsonElement kok = d.RootElement;
                if (kok.ValueKind == JsonValueKind.Array)
                {
                    if (kok.GetArrayLength() == 0) return filtre;
                    kok = kok[0];
                }
                if (kok.ValueKind != JsonValueKind.Object) return filtre;
                foreach (string ad in FiltreAlanlari)
                {
                    string v = AlanMetni(kok, ad);
                    if (TarihAlanlari.Contains(ad))
                    {
                        DateTime? t = TarihCoz(v);
                        v = t.HasValue ? t.Value.ToString("yyyy-MM-dd") : "";
                    }
                    filtre[ad] = v;
                }
            }
            return filtre;
        }

        /// <summary>Eski GetirDetay: json = satırlar | sütunlar (yalnız sütun türü 1) | sorgu alanları.</summary>
        private static IResult Getir(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((kod ?? 0) <= 0) return Hata("Kayıt seçilmedi.");
            try
            {
                TNS.MUH.CaprazTablo kriter = new TNS.MUH.CaprazTablo { kod = kod.Value };
                ObjectArray bilgiler = TNS.MUH.Arac.Tanimla().CaprazTabloListele(kullanan, kriter);
                if (bilgiler == null || !bilgiler.sonuc.islemSonuc)
                    return Hata(HataMetni(bilgiler?.sonuc.hataStr, "Kayıt okunamadı."));
                if (bilgiler.objeler.Count == 0) return Hata("Kayıt bulunamadı.");
                kriter = (TNS.MUH.CaprazTablo)bilgiler.objeler[0];

                string[] json = (kriter.json ?? "").Split('|');
                List<CaprazTabloKod> satirlar = KodlariOku(json[0]);
                List<CaprazTabloKod> sutunlar = new List<CaprazTabloKod>();
                Dictionary<string, string> filtre = new Dictionary<string, string>();
                if (json.Length > 1)
                {
                    if (json[1] != "" && kriter.sutunTur == 1)
                    {
                        sutunlar = KodlariOku(json[1]);
                        if (json.Length > 2) filtre = FiltreOku(json[2]);
                    }
                    else
                        filtre = FiltreOku(json[1]);
                }

                return Results.Json(new
                {
                    kod = kriter.kod,
                    adi = kriter.adi ?? "",
                    sutunTur = kriter.sutunTur,
                    satirlar = satirlar,
                    sutunlar = sutunlar,
                    filtre = filtre
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kayıt okunamadı."));
            }
        }

        /// <summary>Eski Kaydet: saklanan json biçimi eskisiyle aynıdır (eski sayfa da aynı kayıtları açabilsin).</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, CaprazTabloIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                List<CaprazTabloKod> satirlar = (istek.satirlar ?? new List<CaprazTabloKod>()).Where(s => s != null && Temiz(s.HESAPKOD) != "").ToList();
                List<CaprazTabloKod> sutunlar = (istek.sutunlar ?? new List<CaprazTabloKod>()).Where(s => s != null && Temiz(s.HESAPKOD) != "").ToList();
                if (satirlar.Count == 0 && sutunlar.Count == 0) return Hata("Satır ve Sütun bilgileri boş bırakılamaz.");

                int sutunTur = istek.sutunTur;
                TNS.MUH.CaprazTablo bilgi = new TNS.MUH.CaprazTablo();
                if (!istek.farkliKaydet)
                    bilgi.kod = istek.kod;
                bilgi.adi = Temiz(istek.adi);
                if (bilgi.kod <= 0 && bilgi.adi == "") return Hata("Sorgu adı boş bırakılamaz.");

                JsonSerializerOptions js = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
                bilgi.json = satirlar.Count > 0 ? JsonSerializer.Serialize(satirlar, js) : "";
                if (sutunTur == 1)
                {
                    if (sutunlar.Count == 0) return Hata("Sütun bilgisi boş bırakılamaz.");
                    bilgi.json += "|" + JsonSerializer.Serialize(sutunlar, js);
                }

                // Sorgu alanları eski pgFiltre.getSource() gibi; tarihler eski Ext kodlamasıyla (yyyy-MM-ddT00:00:00).
                Dictionary<string, string> filtre = new Dictionary<string, string>();
                foreach (string ad in FiltreAlanlari)
                {
                    string v = istek.filtre != null && istek.filtre.TryGetValue(ad, out string d) ? Temiz(d) : "";
                    if (TarihAlanlari.Contains(ad))
                    {
                        DateTime? t = TarihCoz(v);
                        v = t.HasValue ? t.Value.ToString("yyyy-MM-dd'T'HH:mm:ss") : "";
                    }
                    filtre[ad] = v;
                }
                bilgi.json += "|" + JsonSerializer.Serialize(filtre, js);
                bilgi.sutunTur = sutunTur;

                Sonuc sonuc = TNS.MUH.Arac.Tanimla().CaprazTabloKaydet(kullanan, bilgi);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Kayıt yapılamadı."));
                return Results.Json(new { basarili = true, kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0), adi = bilgi.adi, bilgi = HataMetni(sonuc.bilgiStr, "Kayıt yapıldı.") });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, CaprazTabloIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek.kod <= 0) return Hata("Silinecek kaydı seçiniz.");
            try
            {
                Sonuc sonuc = TNS.MUH.Arac.Tanimla().CaprazTabloSil(kullanan, new TNS.MUH.CaprazTablo { kod = istek.kod });
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Kayıt silinemedi."));
                return Results.Json(new { basarili = true, bilgi = HataMetni(sonuc.bilgiStr, "Kayıt silindi.") });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kayıt silinemedi."));
            }
        }

        // ------------------------------------------------------------------ Tablo Göster (eski btnYazdir_Click)

        /// <summary>
        /// Eski btnYazdir_Click. satir / sutun: virgülle ayrılmış hesap kodları (istemci "kod - ad" değerinden kodu ayırır, eski
        /// kayitliHesapKod.Substring); noktalar burada atılır. Sütun türü 1 hesap kodu (RaporHesapKodListele2), 2 ekonomik kod,
        /// 3 kurumsal kod (RaporListele).
        /// </summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                IQueryCollection q = baglam.Request.Query;
                string Al(string ad) { return Temiz(q[ad]); }

                int sutunTur = OrtakFonksiyonlar.ConvertToInt(Al("sutunTur"), 0);
                List<string> satirKodlari = KodListesi(Al("satir"));
                List<string> sutunKodlari = KodListesi(Al("sutun"));

                if (sutunTur == 0) return Hata("Sütun Tür bilgisi boş bırakılamaz.");
                if (sutunTur == 1 && sutunKodlari.Count == 0) return Hata("Sütun bilgisi boş bırakılamaz.");
                if (satirKodlari.Count == 0) return Hata("Satır bilgisi boş bırakılamaz.");

                TNS.MUH.CaprazTablo kriter = new TNS.MUH.CaprazTablo();
                kriter.sutunTur = sutunTur;
                kriter.sorguSatir = string.Join(", ", satirKodlari.Select(k => "'" + k + "'"));
                if (sutunTur == 1)
                    kriter.sorguSutun = string.Join(", ", sutunKodlari.Select(k => "'" + k + "'"));

                kriter.sorguYil = OrtakFonksiyonlar.ConvertToInt(Al("yil"), 0);
                kriter.sorguDurum = OrtakFonksiyonlar.ConvertToInt(Al("durum"), 0);
                kriter.sorguMuhasebeKod = Al("muhasebe");
                kriter.sorguBirimKod = Al("birim");
                kriter.sorguTarih1 = TnsTarih(Al("belgeTarihi1"));
                kriter.sorguTarih2 = TnsTarih(Al("belgeTarihi2"));
                kriter.sorguYevmiyeTarih1 = TnsTarih(Al("yevmiyeTarihi1"));
                kriter.sorguYevmiyeTarih2 = TnsTarih(Al("yevmiyeTarihi2"));
                kriter.sorguAltHesapKod = Al("althesapNo");
                kriter.sorguIlgiliKod = Al("ilgiliKod");
                kriter.sorguKurKod = Al("kur");
                kriter.sorguFonkKod = Al("fon");
                kriter.sorguFinansKod = Al("fin");
                kriter.sorguEkoKod = Al("eko");
                kriter.sorguFaaliyetKod = Al("peb");

                string sablon = MuavinDefteriIslem.SablonBul(ortam.ContentRootPath, Sablon);
                if (sablon == null) return Hata("Rapor şablonu bulunamadı: MUH/" + Sablon);

                IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
                string dosya;
                if (sutunTur == 2 || sutunTur == 3)
                {
                    ObjectArray liste = servisMUH.CaprazTabloRaporListele(kullanan, kriter);
                    if (liste == null || !liste.sonuc.islemSonuc) return Hata(HataMetni(liste?.sonuc.hataStr, "Sorgu yapılamadı."));
                    if (liste.objeler.Count == 0) return Hata("Verilen kriterlere uygun kayıt bulunamadı.");
                    AsposeLisans.Yukle();
                    dosya = YazdirEkoKur(sablon, liste, kriter);
                }
                else if (sutunTur == 1)
                {
                    ObjectArray liste = servisMUH.CaprazTabloRaporHesapKodListele2(kullanan, kriter);
                    if (liste == null || !liste.sonuc.islemSonuc) return Hata(HataMetni(liste?.sonuc.hataStr, "Sorgu yapılamadı."));
                    if (liste.objeler.Count < 2 || !(liste.objeler[0] is Hashtable htSatir) || htSatir.Count == 0)
                        return Hata("Verilen kriterlere uygun kayıt bulunamadı.");
                    AsposeLisans.Yukle();
                    dosya = YazdirHesapKod(sablon, liste);
                }
                else
                    return Hata("Sütun Tür bilgisi hatalı.");

                return DosyaIslem.DosyaGonder(dosya, "Çapraz Tablo", true, "XLSX");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        /// <summary>"150.03.01,150.03.02" -> noktasız, tekil kodlar (eski hesapKod.Replace(".", "")).</summary>
        private static List<string> KodListesi(string s)
        {
            List<string> liste = new List<string>();
            foreach (string parca in (s ?? "").Split(','))
            {
                string kod = parca.Trim().Replace(".", "").Replace("'", "");
                if (kod != "" && !liste.Contains(kod)) liste.Add(kod);
            }
            return liste;
        }

        private static string SonucDosyaAd()
        {
            return DosyaIslem.DosyaAdUret("CaprazTablo_", "") + ".xlsx";
        }

        /// <summary>
        /// Eski YazdirEkoKur: sütunlar ekonomik (2) ya da kurumsal (3) kodlar, satırlar hesap kodları, hücre borç - alacak.
        /// Eski kod satır değerlerini "kolon|tutar-kolon|tutar" metnine yazıp '-' ile bölüyordu; eksi tutarlarda (alacak bakiyesi)
        /// bu bozuluyordu. Aynı çıktı, değerler listede tutularak üretilir.
        /// </summary>
        private static string YazdirEkoKur(string sablon, ObjectArray liste, TNS.MUH.CaprazTablo kriter)
        {
            Tablo XLS = GenelIslemler.NewTablo();

            int kolonSatir = 0;
            int kolonSutun = 0;

            int kaynakSatir = 0;
            int sutun = 0;
            int yazSatir = 0;

            string sonucDosyaAd = SonucDosyaAd();
            XLS.DosyaAc(sablon, sonucDosyaAd);

            XLS.HucreAdAdresCoz("baslaSutun", ref kolonSatir, ref kolonSutun);
            XLS.HucreAdAdresCoz("baslaSatir", ref yazSatir, ref sutun);

            // Sütun yaz
            Hashtable htSutun = new Hashtable();
            string keySutun = "";
            foreach (OdemeEmriMIFDetay item in liste.objeler)
            {
                if (kriter.sutunTur == 2)//EKOKOD
                    keySutun = item.eko ?? "";
                else if (kriter.sutunTur == 3)//KURKOD
                    keySutun = item.kur ?? "";

                if (htSutun.ContainsKey(keySutun)) continue;

                htSutun.Add(keySutun, "");
            }

            var sutunlar = htSutun.Cast<DictionaryEntry>().OrderBy(entry => entry.Key).ToList();

            foreach (DictionaryEntry item in sutunlar)
            {
                XLS.HucreKopyala(1, kolonSutun, 1, kolonSutun, kolonSatir, kolonSutun);

                XLS.HucreDegerYaz(kolonSatir, kolonSutun, item.Key.ToString());
                htSutun[item.Key] = kolonSutun;
                kolonSutun++;
            }

            XLS.HucreBirlestir(2, 0, 2, kolonSutun - 1);

            // Satır yaz
            Hashtable htSatir = new Hashtable();
            foreach (OdemeEmriMIFDetay item in liste.objeler)
            {
                int icSutun = 0;
                if (kriter.sutunTur == 2)//EKOKOD
                    icSutun = OrtakFonksiyonlar.ConvertToInt(htSutun[item.eko ?? ""], 0);
                else if (kriter.sutunTur == 3)//KURKOD
                    icSutun = OrtakFonksiyonlar.ConvertToInt(htSutun[item.kur ?? ""], 0);

                string keySatir = item.hesapKod + "|" + icSutun;
                double valSatir = (item.borc - item.alacak);

                if (htSatir.ContainsKey(keySatir))
                    htSatir[keySatir] = OrtakFonksiyonlar.ConvertToDbl(htSatir[keySatir]) + valSatir;
                else
                    htSatir.Add(keySatir, valSatir);
            }

            var satirlar = htSatir.Cast<DictionaryEntry>().OrderBy(entry => entry.Key).ToList();

            // Hesap kodu -> (kolon, tutar) listesi; sıra eskisi gibi anahtar sırasıdır.
            List<string> hesapSira = new List<string>();
            Dictionary<string, List<KeyValuePair<int, double>>> dic = new Dictionary<string, List<KeyValuePair<int, double>>>();
            foreach (DictionaryEntry item in satirlar)
            {
                double val = OrtakFonksiyonlar.ConvertToDbl(item.Value);
                string anahtar = item.Key.ToString();
                int ayrac = anahtar.LastIndexOf('|');
                string icHesapKod = anahtar.Substring(0, ayrac);
                int kolon = OrtakFonksiyonlar.ConvertToInt(anahtar.Substring(ayrac + 1), 0);

                if (!dic.TryGetValue(icHesapKod, out List<KeyValuePair<int, double>> degerler))
                {
                    degerler = new List<KeyValuePair<int, double>>();
                    dic.Add(icHesapKod, degerler);
                    hesapSira.Add(icHesapKod);
                }
                degerler.Add(new KeyValuePair<int, double>(kolon, val));
            }

            foreach (string hesapKod in hesapSira)
            {
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, kolonSutun - 1, yazSatir, sutun);
                XLS.HucreDegerYaz(yazSatir, 0, hesapKod);

                foreach (KeyValuePair<int, double> d in dic[hesapKod])
                    XLS.HucreDegerYaz(yazSatir, d.Key, d.Value);

                yazSatir++;
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        /// <summary>Hashtable değerini CaprazTabloIcerik'e çevirir (API'den $type ile gelir; gelmezse alanlardan kurulur).</summary>
        private static CaprazTabloIcerik Icerik(object o)
        {
            if (o is CaprazTabloIcerik c) return c;
            if (o is Hashtable h)
            {
                CaprazTabloIcerik i = new CaprazTabloIcerik();
                i.belgeNo = Convert.ToString(h["belgeNo"]) ?? "";
                i.hesapKodu = Convert.ToString(h["hesapKodu"]) ?? "";
                i.tur = Convert.ToString(h["tur"]) ?? "";
                i.yevmiyeNo = OrtakFonksiyonlar.ConvertToInt(h["yevmiyeNo"] + "", 0);
                i.bakiye = OrtakFonksiyonlar.ConvertToDbl(h["bakiye"]);
                i.ilgiliAdi = Convert.ToString(h["ilgiliAdi"]) ?? "";
                i.ilgiliNo = Convert.ToString(h["ilgiliNo"]) ?? "";
                i.belgeTarih = h["belgeTarih"] as TNSDateTime ?? new TNSDateTime(h["belgeTarih"] + "");
                i.yevmiyeTarih = h["yevmiyeTarih"] as TNSDateTime ?? new TNSDateTime(h["yevmiyeTarih"] + "");
                return i;
            }
            return new CaprazTabloIcerik();
        }

        /// <summary>
        /// Eski YazdirHesapKod: satırlar belge + satır hesap kodu (belge no, tarih, yevmiye no, ilgili, hesap kodu, bakiye), sütunlar
        /// sütun hesap kodları; hücre aynı belgedeki sütun hesap kodunun bakiyesi. Satırlar belge no / hesap koduna göre sıralı
        /// (eski kodda Hashtable sırasıyla geliyordu).
        /// </summary>
        private static string YazdirHesapKod(string sablon, ObjectArray liste)
        {
            string hesYapi = TNS.MUH.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD8/KODYAPI");

            Hashtable htSorguSatir = (Hashtable)liste.objeler[0];
            Hashtable htSorguSutun = (Hashtable)liste.objeler[1] ?? new Hashtable();
            Hashtable htKolonSutun = new Hashtable();

            Tablo XLS = GenelIslemler.NewTablo();

            int kolonSatir = 0;
            int kolonSutun = 6;

            int kaynakSatir = 0;
            int sutun = 0;
            int yazSatir = 0;

            string sonucDosyaAd = SonucDosyaAd();
            XLS.DosyaAc(sablon, sonucDosyaAd);

            XLS.HucreAdAdresCoz("baslaSutun", ref kolonSatir, ref kolonSutun);
            XLS.HucreAdAdresCoz("baslaSatir", ref yazSatir, ref sutun);

            // Sütun başlık yaz: sütunlar için tekil hesap kodları
            List<string> sutunHesapKodlari = new List<string>();
            foreach (DictionaryEntry item in htSorguSutun)
            {
                string[] parca = Convert.ToString(item.Key).Split('_');
                if (parca.Length == 2 && !sutunHesapKodlari.Contains(parca[1]))
                    sutunHesapKodlari.Add(parca[1]);
            }

            sutunHesapKodlari.Sort();

            foreach (string hs in sutunHesapKodlari)
            {
                XLS.HucreKopyala(kaynakSatir, 6, kaynakSatir, 6, kolonSatir, kolonSutun);
                XLS.HucreDegerYaz(kolonSatir, kolonSutun, OrtakFonksiyonlar.KodNoktaKoy(hesYapi, hs));
                htKolonSutun[hs] = kolonSutun;
                kolonSutun++;
            }

            XLS.HucreBirlestir(2, 0, 2, kolonSutun - 1);

            // Satır yaz
            List<DictionaryEntry> satirlar = htSorguSatir.Cast<DictionaryEntry>()
                .OrderBy(e => Convert.ToString(e.Key), StringComparer.Ordinal).ToList();
            List<DictionaryEntry> sutunDegerleri = htSorguSutun.Cast<DictionaryEntry>().ToList();

            foreach (DictionaryEntry item in satirlar)
            {
                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 5, yazSatir, 0);

                XLS.CerceveCizgiCiz(yazSatir, yazSatir, sutun, kolonSutun - 1, LineStyle.THIN, TabloRenk.BLACK);

                CaprazTabloIcerik cp = Icerik(item.Value);

                XLS.HucreDegerYaz(yazSatir, 0, cp.belgeNo);

                if (cp.yevmiyeTarih != null && !cp.yevmiyeTarih.isNull)
                    XLS.HucreDegerYaz(yazSatir, 1, cp.yevmiyeTarih.Oku());
                else if (cp.belgeTarih != null && !cp.belgeTarih.isNull)
                    XLS.HucreDegerYaz(yazSatir, 1, cp.belgeTarih.Oku());

                XLS.HucreDegerYaz(yazSatir, 2, cp.yevmiyeNo);
                XLS.HucreDegerYaz(yazSatir, 3, cp.ilgiliAdi);
                XLS.HucreDegerYaz(yazSatir, 4, OrtakFonksiyonlar.KodNoktaKoy(hesYapi, cp.hesapKodu));
                XLS.HucreDegerYaz(yazSatir, 5, cp.bakiye);

                foreach (DictionaryEntry ssi in sutunDegerleri)
                {
                    string[] parca = Convert.ToString(ssi.Key).Split('_');
                    if (parca.Length == 2 && parca[0] == cp.belgeNo && htKolonSutun.ContainsKey(parca[1]))//belge no satır ile aynı olan sütunlar
                    {
                        CaprazTabloIcerik cps = Icerik(ssi.Value);
                        int kolon = (int)htKolonSutun[parca[1]];
                        XLS.HucreKopyala(kaynakSatir, 7, kaynakSatir, 7, yazSatir, kolon);
                        XLS.HucreDegerYaz(yazSatir, kolon, cps.bakiye);
                    }
                }

                yazSatir++;
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        // ------------------------------------------------------------------ kod ağaçları (eski nodeLoadPar: 1, 2, 3, 4, 14)

        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string tur, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            tur = Temiz(tur);
            if (tur != "1" && tur != "2" && tur != "3" && tur != "4" && tur != "14") return Hata("Kod türü hatalı.");
            return GorusmeCariUclari.KodDuzeyi(kullanan, HesapYil(yil), tur, ust, false);
        }

        // ------------------------------------------------------------------ hesap kodu (eski StoreHesapPlan / HesapListesiDoldur)
        // Sgb.hesapKodu (js/hesapkodu.js) uçları; Muavin Defteri uçlarının aynısı, bu sayfanın yetkisiyle.

        private static object HesapDugum(HesapPlaniSatir h)
        {
            return new { kod = h.hesapKod, ad = h.hesapKod + " - " + h.aciklama, hesapAd = h.aciklama ?? "", altVar = !h.detay, detay = h.detay };
        }

        private static IResult HesapAgac(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            try
            {
                int y = HesapYil(yil);
                ust = Temiz(ust);
                List<object> dugumler = new List<object>();
                if (ust == "")
                {
                    TNS.UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
                    for (int i = 1; i < 10; i++)
                    {
                        string ad = servisUZY.UzayDegeriStr(new Kullanici(), "MUHHESAPPLANI", y.ToString() + "-" + i.ToString(), true);
                        dugumler.Add(new { kod = i.ToString(), ad = i.ToString() + "-" + ad, hesapAd = ad ?? "", altVar = true, detay = false });
                    }
                    return Results.Json(new { dugumler = dugumler });
                }

                string[] kirilimlar = ust.Split('.');
                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.yil = y;
                hs.hesapKod = ust;
                hs.seviye = ust.Length < 3 ? 1 : kirilimlar.Length + 1;

                IMUHServis servis = TNS.MUH.Arac.Tanimla();
                ObjectArray hv = new ObjectArray();
                for (int i = 0; i < 10; i++)
                {
                    hv = servis.HesapPlaniListele(new Kullanici(), hs);
                    if (hv.objeler.Count == 0) hs.seviye++;
                    else break;
                }
                foreach (HesapPlaniSatir h in hv.objeler) dugumler.Add(HesapDugum(h));
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Hata("Hesap planı okunamadı.");
            }
        }

        private static IResult HesapAra(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string metin, string sonKirilim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                metin = Temiz(metin);
                if (metin.Length < 2) return Hata("En az iki karakter yazınız.");
                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = HesapYil(yil);
                h.hesapKodHesapAd = metin;
                h.detay = sonKirilim == "1";
                ObjectArray hesap = TNS.MUH.Arac.Tanimla().HesapPlaniListele(kullanan, h);
                if (hesap == null || !hesap.sonuc.islemSonuc) return Hata(HataMetni(hesap?.sonuc.hataStr, "Arama yapılamadı."));
                List<object> satirlar = new List<object>();
                foreach (HesapPlaniSatir d in hesap.objeler)
                {
                    satirlar.Add(HesapDugum(d));
                    if (satirlar.Count >= 500) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Arama yapılamadı.");
            }
        }

        private static IResult HesapAd(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                kod = Temiz(kod);
                if (kod == "") return Results.Json(new { kod = "", ad = "", detay = false });
                int y = HesapYil(yil);
                string hesYapi = TNS.MUH.Arac.DegiskenDegerBul(y, "/KODYAPI/KOD8/KODYAPI");
                if (!string.IsNullOrEmpty(hesYapi)) kod = OrtakFonksiyonlar.KodNoktaKoy(hesYapi, kod.Replace(".", ""));
                ObjectArray hv = TNS.MUH.Arac.Tanimla().HesapPlaniListele(kullanan, new HesapPlaniSatir { yil = y, hesapKod = kod });
                if (hv != null && hv.objeler != null)
                    foreach (HesapPlaniSatir d in hv.objeler)
                        if (d.hesapKod == kod) return Results.Json(new { kod = d.hesapKod, ad = d.aciklama ?? "", detay = d.detay });
                return Results.Json(new { kod = kod, ad = "", detay = false });
            }
            catch (Exception)
            {
                return Hata("Hesap kodu okunamadı.");
            }
        }
    }

    /// <summary>Satır / sütun kaydı (eski StoreSatir / StoreSutun alanları; json'a bu adlarla yazılır).</summary>
    public class CaprazTabloKod
    {
        public string KOD { get; set; } = "";
        public string HESAPKOD { get; set; } = "";
    }

    /// <summary>Kaydet / Sil isteği.</summary>
    public class CaprazTabloIstegi
    {
        public int kod { get; set; }
        public string adi { get; set; }
        public bool farkliKaydet { get; set; }
        public int sutunTur { get; set; }
        public List<CaprazTabloKod> satirlar { get; set; }
        public List<CaprazTabloKod> sutunlar { get; set; }
        public Dictionary<string, string> filtre { get; set; }
    }
}
