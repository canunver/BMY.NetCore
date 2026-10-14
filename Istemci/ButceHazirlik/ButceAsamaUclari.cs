using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Istemci.Organizasyon;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;

namespace Istemci.ButceHazirlik
{
    public class AsamaDurumSatiri
    {
        public string SenaryoKod { get; set; }
        public int OnayDurum { get; set; }
    }

    public class AsamaDurumIstegi
    {
        public int Yil { get; set; }
        public string Birim { get; set; }
        public List<AsamaDurumSatiri> Satirlar { get; set; }
    }

    public class AsamaKopyaIstegi
    {
        /// <summary>1 = gider ödenekleri, 2 = gelir ödenekleri.</summary>
        public int Tur { get; set; }
        public int KaynakYil { get; set; }
        public string KaynakAsama { get; set; }
        /// <summary>Yalnızca gider kopyasında: bu ekonomik koda (birinci düzey) ait ödenekler kopyalanır; boşsa hepsi.</summary>
        public string EkoKod { get; set; }
        public int HedefYil { get; set; }
        public string HedefAsama { get; set; }
    }

    /// <summary>
    /// Bütçe Aşama İşlemleri sayfasının (eski ButceHazirlikv2/ButceAsamaIslemleri.aspx) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/butceasama/tanim, /kodlar?yil=, /liste?yil=&amp;birim=
    ///   POST /api/butceasama/kaydet (aşamaların birim için onay durumları), /kopyala (bir aşamanın ödeneklerini başka aşamaya)
    /// İşlemler eskisi gibi TNS.BHMv2.Arac.Tanimla() üzerinden yapılır. Sayfayı menüde bu seçeneği görebilenler kullanır;
    /// durumları kaydetmeyi iş kuralları yalnızca sistem yöneticisine izin verir (ProgramOnayForm.YazmaYetkisiVarMi).
    /// </summary>
    public static class ButceAsamaUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/butceasama/tanim", Tanim);
            app.MapGet("/api/butceasama/kodlar", Kodlar);
            app.MapGet("/api/butceasama/liste", Liste);
            app.MapPost("/api/butceasama/kaydet", Kaydet);
            app.MapPost("/api/butceasama/kopyala", Kopyala);
        }

        /// <summary>Kullanıcı Bütçe Aşama İşlemleri sayfasını kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), "../ButceHazirlikv2/ButceAsamaIslemleri.aspx");
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

        /// <summary>Varsayılan kurum kodu (eski GenelIslemlerIstemci.VarsayilanKurumBul: /KURUMKOD).</summary>
        private static string KurumKod()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMKOD") ?? "";
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Sayfanın seçenekleri (eski Page_Load): yıllar, aşamalar (senaryolar), onay durumları, birimler ve ekonomik kodlar.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            int yil = DateTime.Now.Year;
            List<object> asamalar = new List<object>();
            try
            {
                ButceSenaryo[] senaryolar = TNS.BHMv2.Arac.Tanimla().ButceSenaryoListele(kullanan, null);
                if (senaryolar != null)
                    foreach (ButceSenaryo bs in senaryolar)
                        asamalar.Add(new { kod = bs.kod, ad = bs.kod + "- " + bs.ad });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Bütçe aşamaları okunamadı." });
            }

            List<object> durumlar = new List<object>();
            for (int i = 0; i < TNS.BHMv2.Arac.Durumlar.Length; i++)
                durumlar.Add(new { kod = i, ad = TNS.BHMv2.Arac.Durumlar[i] });

            return Results.Json(new
            {
                yil = yil,
                // Eski sayfada yıl alanları bu yılın bir sonrasına kadar artırılabiliyordu.
                enBuyukYil = yil + 1,
                asamalar = asamalar,
                durumlar = durumlar,
                // Durumları yalnızca sistem yöneticisi kaydedebilir (iş kurallarındaki ProgramOnayForm.YazmaYetkisiVarMi ile aynı kural);
                // diğerleri listeyi salt okunur görür.
                kaydedebilir = GenelGorevUclari.YetkiVar(kullanan),
                kodlar = KodlariOku(yil)
            });
        }

        /// <summary>Yıla bağlı listeler: birimler (kurumsal kodun 4. düzeyi, sonda kurumun kendisi) ve birinci düzey ekonomik kodlar.</summary>
        private static object KodlariOku(int yil)
        {
            IBKDServis servis = TNS.BKD.Arac.Tanimla();
            string kurumKod = KurumKod();

            // Eski BHMv2Ortak.EXTBirimDoldur(..., bosItemOlsun: false, kurumEkle: true)
            List<object> birimler = new List<object>();
            ABSKodlarVAN birimVan = servis.KodListeleSeviye(yil, "1", kurumKod, 4, true);
            if (birimVan != null && birimVan.absKodlar != null)
                foreach (ABSKod k in birimVan.absKodlar)
                    birimler.Add(new { kod = k.kod, ad = k.kodAd });
            if (kurumKod != "")
                birimler.Add(new { kod = kurumKod, ad = kurumKod + "-" + (TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMAD") ?? "") });

            // Eski BHMv2Ortak.ABSKodDoldur(yil, "4", "", 1, ..., bosBasaEkle: true, "", "Hepsi", sadeceDetay: false, hiyerarsik: false)
            List<object> ekoKodlar = new List<object> { new { kod = "", ad = "Hepsi" } };
            ABSKodlarVAN ekoVan = servis.KodListeleSeviye(yil, "4", "", 1, false, false, "", "", "", false);
            if (ekoVan != null && ekoVan.absKodlar != null)
                foreach (ABSKod k in ekoVan.absKodlar)
                    ekoKodlar.Add(new { kod = k.kod, ad = k.kodAd });

            return new { birimler = birimler, ekoKodlar = ekoKodlar };
        }

        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil)
        {
            IResult ret = Denetle(baglam, ortam, out _);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            try
            {
                return Results.Json(KodlariOku(yil));
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Kodlar okunamadı." });
            }
        }

        /// <summary>Birim için her aşamanın onay durumu (eski btnListele_Click).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            if (string.IsNullOrWhiteSpace(birim)) return Results.Json(new { hata = "Birim seçiniz." });

            try
            {
                OdenekSenaryoDurum[] durumlar = TNS.BHMv2.Arac.Tanimla().ButceOdenekDurumListele(kullanan, yil, birim.Trim());
                List<object> satirlar = new List<object>();
                if (durumlar != null)
                    foreach (OdenekSenaryoDurum d in durumlar)
                        satirlar.Add(new { senaryoKod = d.senaryoKod, senaryoAd = d.senaryoAd, onayDurum = d.onayDurum });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Aşama durumları okunamadı." });
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>
        /// Birimin aşama durumlarını kaydeder (eski btnKaydet_Click). İş kuralı birimin bütün durumlarını silip gelen listeyi yazar;
        /// bu yüzden listenin tamamı gönderilir.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, AsamaDurumIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000 || string.IsNullOrWhiteSpace(istek.Birim))
                return Results.Json(new { basarili = false, hata = "Yıl ve birim seçiniz." });

            List<OdenekSenaryoDurum> onaylar = new List<OdenekSenaryoDurum>();
            foreach (AsamaDurumSatiri s in istek.Satirlar ?? new List<AsamaDurumSatiri>())
            {
                if (s == null || string.IsNullOrWhiteSpace(s.SenaryoKod)) continue;
                if (s.OnayDurum < 0 || s.OnayDurum >= TNS.BHMv2.Arac.Durumlar.Length)
                    return Results.Json(new { basarili = false, hata = "Geçersiz durum: " + s.SenaryoKod });
                OdenekSenaryoDurum osd = new OdenekSenaryoDurum();
                osd.senaryoKod = s.SenaryoKod.Trim();
                osd.onayDurum = s.OnayDurum;
                onaylar.Add(osd);
            }

            Sonuc sonuc;
            try
            {
                sonuc = TNS.BHMv2.Arac.Tanimla().ButceOdenekDurumKaydet(kullanan, istek.Yil, istek.Birim.Trim(), onaylar.ToArray());
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız.") });
            return Results.Json(new { basarili = true, bilgi = "Durum bilgisi kaydedildi." });
        }

        /// <summary>
        /// Bir yılın bir aşamasındaki gider ya da gelir ödeneklerini başka bir yılın aşamasına kopyalar (eski KopyaIslem).
        /// Kopya varsayılan kurumun bütün birimleri içindir; gider kopyası istenirse bir ekonomik kodla sınırlanır.
        /// Eski sayfa denetimlerin sonucunu kullanmıyordu (hatalı alanla da kopyalıyordu); burada denetim geçmezse kopyalanmaz.
        /// </summary>
        private static async Task<IResult> Kopyala(HttpContext baglam, IWebHostEnvironment ortam, AsamaKopyaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            string kaynakAsama = (istek.KaynakAsama ?? "").Trim();
            string hedefAsama = (istek.HedefAsama ?? "").Trim();
            ENUMTabloKod tk = istek.Tur == 1 ? ENUMTabloKod.TABPEBOdenek : istek.Tur == 2 ? ENUMTabloKod.TABPEBGelir : ENUMTabloKod.TANIMSIZ;

            List<string> hatalar = new List<string>();
            if (istek.KaynakYil < 2000) hatalar.Add("Kaynak yıl hatalı!");
            if (istek.HedefYil < 2000) hatalar.Add("Hedef yıl hatalı!");
            if (kaynakAsama == "") hatalar.Add("Kaynak aşama hatalı!");
            if (hedefAsama == "") hatalar.Add("Hedef aşama hatalı!");
            if (tk == ENUMTabloKod.TANIMSIZ) hatalar.Add("Hatalı ödenek türü!");
            if (hatalar.Count == 0 && istek.KaynakYil == istek.HedefYil && kaynakAsama == hedefAsama)
                hatalar.Add("Kaynak ve hedef aynı olamaz!");
            if (hatalar.Count > 0) return Results.Json(new { basarili = false, hata = string.Join(" ", hatalar) });

            Sonuc sonuc;
            try
            {
                Tertip t = new Tertip(istek.HedefYil, tk, KurumKod());
                if (istek.Tur == 1) t.kodlar[3] = (istek.EkoKod ?? "").Trim();
                sonuc = TNS.BHMv2.Arac.Tanimla().ButceOdenekKopyala(kullanan, t, hedefAsama, istek.KaynakYil, kaynakAsama);
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kopyalama işlemi başarısız." });
            }

            // Gider kopyası her tablonun sonucunu art arda yazar ("... kopyalandı (5 Kayıt)Gider Ödenek ..."): satırlara ayrılır.
            string bilgi = Regex.Replace(sonuc.bilgiStr ?? "", @"\)(?=\S)", ")\n").Trim();
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Kopyalama işlemi başarısız."), bilgi = bilgi });
            if (bilgi == "") bilgi = (istek.Tur == 1 ? "Gider" : "Gelir") + " ödenekleri kopyalandı.";
            // Gider kopyasında tablolardan biri hata verse de işlem sonucu başarılıdır; hata metni bilgide yazar.
            return Results.Json(new { basarili = true, bilgi = bilgi, uyari = bilgi.Contains("hata oluştu") });
        }
    }
}
