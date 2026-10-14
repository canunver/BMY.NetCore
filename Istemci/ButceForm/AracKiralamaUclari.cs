using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.Organizasyon;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHF;
using TNS.KYM;

namespace Istemci.ButceForm
{
    public class AracKiralamaSatiri
    {
        public int TasitTipi { get; set; }
        public string TasitKodu { get; set; }
        public int ButceKaynagi { get; set; }
        public double OncYilKiralanan { get; set; }
        public int OncYilKiralananSure { get; set; }
        public double OncYilFiyat { get; set; }
        public double BuYilKiralanan { get; set; }
        public int BuYilKiralananSure { get; set; }
        public double BuYilFiyat { get; set; }
        public double GelecekYilKiralanacak { get; set; }
        public int GelecekYilKiralanacakSure { get; set; }
        public double GelecekYilFiyat { get; set; }
    }

    public class AracKiralamaKayitIstegi
    {
        public int Yil { get; set; }
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        /// <summary>Okunan formun durumu (0 yok, 1 yeni, 5 onaylı); eski gizli txtdurum alanı.</summary>
        public int Durum { get; set; }
        public List<AracKiralamaSatiri> Satirlar { get; set; }
    }

    public class AracKiralamaOnayIstegi
    {
        public int Yil { get; set; }
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        /// <summary>5: onayla, 1: onayı kaldır.</summary>
        public int Durum { get; set; }
    }

    public class AracKiralamaBirimSecimi
    {
        public int Saymanlik { get; set; }
        public string Birim { get; set; }
    }

    public class AracKiralamaSeciliOnayIstegi
    {
        public int Yil { get; set; }
        /// <summary>5: onayla, 1: onayı kaldır.</summary>
        public int Durum { get; set; }
        public List<AracKiralamaBirimSecimi> Birimler { get; set; }
    }

    /// <summary>
    /// Kiralık Taşıt Planlama Formu sayfasının (eski ButceForm/AracKiralamaPlaniForm.aspx) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/arackiralama/tanim                         açılış: yıl, bütçe kaynakları, taşıt kodları, kullanıcının muhasebe/birimi
    ///   (muhasebe / birim seçimi ve birim adı ortak bileşenle: /api/muhbirim/..., OrtakSayfa/MuhasebeBirimUclari.cs)
    ///   GET  /api/arackiralama/oku?yil=&amp;muhasebe=&amp;birim=
    ///   POST /api/arackiralama/kaydet, /onayla
    ///   GET  /api/arackiralama/rapor?yil=&amp;muhasebe=&amp;birim=&amp;tur=   1 birim, 2 kurum toplamı, 3 bütün birimlerin detayı (eski kodla, Excel)
    ///   GET  /api/arackiralama/durumlar?yil=, POST /seciliOnayla   Onay İşlemleri penceresi
    /// Okuma / kaydetme / onaylama eskisi gibi TNS.BHF.Arac.Tanimla() üzerinden yapılır; muhasebe-birim görme / yazma yetkisi
    /// iş kurallarında denetlenir. Kurum Yazdır, Detay Yazdır ve Onay İşlemleri eskiden yalnızca sistem yöneticisine
    /// görünüyordu; burada uçlar da yalnızca sistem yöneticisine açıktır.
    /// </summary>
    public static class AracKiralamaUclari
    {
        private const string SayfaAdresi = "../ButceForm/AracKiralamaPlaniForm.aspx";
        /// <summary>Eski spnYil.MinValue.</summary>
        private const int IlkYil = 2023;

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/arackiralama/tanim", Tanim);
            app.MapGet("/api/arackiralama/oku", Oku);
            app.MapPost("/api/arackiralama/kaydet", Kaydet);
            app.MapPost("/api/arackiralama/onayla", Onayla);
            app.MapGet("/api/arackiralama/rapor", Rapor);
            app.MapGet("/api/arackiralama/durumlar", Durumlar);
            app.MapPost("/api/arackiralama/seciliOnayla", SeciliOnayla);
        }

        /// <summary>Kullanıcı bu sayfayı kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), SayfaAdresi);
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

        /// <summary>Eski sayfada Onay Kaldır, Kurum / Detay Yazdır ve Onay İşlemleri tuşları yalnızca sistem yöneticisine görünürdü.</summary>
        private static bool SistemYoneticisi(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        private static IResult Hata(string hata)
        {
            return Results.Json(new { hata = hata });
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        /// <summary>Varsayılan kurum kodu (eski GenelIslemlerIstemci.VarsayilanKurumBul: /KURUMKOD).</summary>
        private static string KurumKod()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMKOD") ?? "";
        }

        /// <summary>Eski hdnKurumsalYaz: kurumsal kod noktalı yazılır.</summary>
        private static string KurumsalYaz(int yil, string kurum)
        {
            kurum = (kurum ?? "").Replace(".", "").Trim();
            if (kurum == "") return "";
            return OrtakFonksiyonlar.KodNoktaKoy(TNS.BKD.TertipYapi.KodYapiBul(yil, 1, false), kurum);
        }

        private static AracKiralamaPlanAna Form(int yil, int muhasebe, string birim)
        {
            AracKiralamaPlanAna form = new AracKiralamaPlanAna();
            form.yil = yil;
            form.muhasebeKod = muhasebe;
            form.birimKod = (birim ?? "").Trim();
            return form;
        }

        /// <summary>
        /// Eski MuhBirimAdBul / SaymanlikBirimBul: saymanlık ve birim adı (bulunamazsa uyarı metni) ve birimin kurum kodu.
        /// </summary>
        private static string MuhBirimAdBul(int saymanlikNo, string birimKod, out string kurum)
        {
            kurum = "";
            if (saymanlikNo <= 0) return "";
            string birimAd = "";
            string sayAd = TNS.UZY.Arac.Tanimla().UzayDegeriStr(new Kullanici(), "MUHMUHASEBE", saymanlikNo.ToString(), true, "MUHASEBEAD");
            if (string.IsNullOrWhiteSpace(sayAd)) sayAd = "Saymanlık Bulunamadı!";
            else if (!string.IsNullOrWhiteSpace(birimKod))
            {
                TNS.MUH.MuhasebeBirim kriter = new TNS.MUH.MuhasebeBirim();
                kriter.muhasebe = saymanlikNo;
                kriter.birimKod = birimKod.Trim();
                kriter.birimAd = "";
                kriter.kurum = "";
                ObjectArray oa = TNS.MUH.Arac.Tanimla().MuhasebeBirimListele(kriter);
                if (oa != null && oa.sonuc.islemSonuc && oa.ObjeSayisi >= 1)
                {
                    TNS.MUH.MuhasebeBirim bir = (TNS.MUH.MuhasebeBirim)oa.objeler[0];
                    birimAd = bir.birimAd;
                    kurum = bir.kurum ?? "";
                }
                if (string.IsNullOrWhiteSpace(birimAd)) birimAd = "Birim Bulunamadı!";
            }
            return TNS.MUH.Arac.BirimAdOl(sayAd, birimAd);
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>
        /// Eski Page_Load: yıl (içinde bulunulan yıl, 2023 ile sonraki yıl arası), bütçe kaynakları (genel bütçe / döner sermaye),
        /// taşıt kodları (o yılın taşıt sözlüğü) ve kullanıcı değişkenlerindeki muhasebe / birim / kurumsal kod.
        /// </summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int yil = DateTime.Now.Year;
            try
            {
                List<object> kaynaklar = new List<object>();
                foreach (IntKodStrAd k in TasitBilgi.AitOlduguButceler(false, false))
                    kaynaklar.Add(new { kod = k.kod, ad = k.ad });

                List<object> tasitlar = new List<object>();
                foreach (TasitSozluk t in TNS.BHF.Arac.Tanimla().TasitSozlukListele(yil) ?? new TasitSozluk[0])
                    tasitlar.Add(new { kod = t.SiraNo, ad = t.Aciklama });

                string muhasebe = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBE");
                string birim = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEBIRIM");
                string kurumsal = KurumsalYaz(yil, GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEKURUMSAL"));
                string birimAd = MuhBirimAdBul(OrtakFonksiyonlar.ConvertToInt(muhasebe, 0), birim, out _);

                return Results.Json(new
                {
                    yil = yil,
                    ilkYil = IlkYil,
                    sonYil = yil + 1,
                    kaynaklar = kaynaklar,
                    tasitlar = tasitlar,
                    muhasebe = muhasebe,
                    birim = birim,
                    kurumsal = kurumsal,
                    birimAd = birimAd,
                    sistemYoneticisi = SistemYoneticisi(kullanan)
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        private static object SatirJson(AracKiralamaPlanDetay d)
        {
            return new
            {
                tasitTipi = d.tasitTipi,
                tasitKodu = d.tasitKodu ?? "",
                butceKaynagi = d.butceKaynagi,
                oncYilKiralanan = d.oncYilKiralanan,
                oncYilKiralananSure = d.oncYilKiralananSure,
                oncYilFiyat = d.oncYilFiyat,
                buYilKiralanan = d.buYilKiralanan,
                buYilKiralananSure = d.buYilKiralananSure,
                buYilFiyat = d.buYilFiyat,
                gelecekYilKiralanacak = d.gelecekYilKiralanacak,
                gelecekYilKiralanacakSure = d.gelecekYilKiralanacakSure,
                gelecekYilFiyat = d.gelecekYilFiyat
            };
        }

        /// <summary>Eski Oku (Oku tuşu): formun ana bilgileri ve satırları; kayıt yoksa bulundu = false.</summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int yil, int? muhasebe, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            ObjectVAN ov;
            try
            {
                ov = TNS.BHF.Arac.Tanimla().OkuAracKiralamaPlani(kullanan, Form(yil, muhasebe ?? 0, birim));
            }
            catch (Exception)
            {
                return Hata("Form okunamadı.");
            }
            if (ov == null) return Hata("Form okunamadı.");
            if (!ov.sonuc.islemSonuc) return Hata(HataMetni(ov.sonuc.hataStr, "Form okunamadı."));
            if (ov.o == null) return Results.Json(new { bulundu = false, durum = 0, satirlar = new object[0] });

            AracKiralamaPlanAna form = (AracKiralamaPlanAna)ov.o;
            List<object> satirlar = new List<object>();
            if (form.aracKiralamaPlanDetay != null)
                foreach (AracKiralamaPlanDetay d in form.aracKiralamaPlanDetay)
                    satirlar.Add(SatirJson(d));
            return Results.Json(new
            {
                bulundu = true,
                durum = form.durum,
                kaydeden = form.kaydeden ?? "",
                kayitTarihi = EkFormUclari.TarihStr(form.kayitTarihi),
                onaylayan = form.onaylayan ?? "",
                onayTarihi = EkFormUclari.TarihStr(form.onayTarihi),
                satirlar = satirlar
            });
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>Eski Kaydet (Kaydet tuşu): satırların hepsi yeniden yazılır; denetimler (yetki, onaylı form, rakamlar) iş kurallarında.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, AracKiralamaKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.Json(new { basarili = false, hata = "Kaydedilecek bilgi yok." });

            AracKiralamaPlanAna form = Form(istek.Yil, istek.Muhasebe, istek.Birim);
            form.durum = istek.Durum;
            form.aracKiralamaPlanDetay = new TNSCollection();
            foreach (AracKiralamaSatiri s in istek.Satirlar ?? new List<AracKiralamaSatiri>())
            {
                if (s == null) continue;
                AracKiralamaPlanDetay d = new AracKiralamaPlanDetay();
                d.tasitTipi = s.TasitTipi;
                d.tasitKodu = (s.TasitKodu ?? "").Trim();
                d.butceKaynagi = s.ButceKaynagi;
                d.oncYilKiralanan = s.OncYilKiralanan;
                d.oncYilKiralananSure = s.OncYilKiralananSure;
                d.oncYilFiyat = s.OncYilFiyat;
                d.buYilKiralanan = s.BuYilKiralanan;
                d.buYilKiralananSure = s.BuYilKiralananSure;
                d.buYilFiyat = s.BuYilFiyat;
                d.gelecekYilKiralanacak = s.GelecekYilKiralanacak;
                d.gelecekYilKiralanacakSure = s.GelecekYilKiralanacakSure;
                d.gelecekYilFiyat = s.GelecekYilFiyat;
                form.aracKiralamaPlanDetay.Add(d);
            }

            Sonuc sonuc;
            try
            {
                sonuc = TNS.BHF.Arac.Tanimla().KaydetAracKiralamaPlani(kullanan, form);
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız.") });
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
        }

        /// <summary>Eski btnOnayla_Click / btnOnayKaldir_Click (durum 5 / 1). Onay kaldırmayı iş kuralı yalnızca sistem yöneticisine bırakır.</summary>
        private static async Task<IResult> Onayla(HttpContext baglam, IWebHostEnvironment ortam, AracKiralamaOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || (istek.Durum != 5 && istek.Durum != 1)) return Results.Json(new { basarili = false, hata = "İşlem hatalı." });

            AracKiralamaPlanAna form = Form(istek.Yil, istek.Muhasebe, istek.Birim);
            form.durum = istek.Durum;
            Sonuc sonuc;
            try
            {
                sonuc = TNS.BHF.Arac.Tanimla().OnaylaAracKiralamaPlani(kullanan, form, false);
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Onay işlemi yapılamadı." });
            }
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Onay işlemi yapılamadı.") });
            return Results.Json(new { basarili = true, bilgi = istek.Durum == 5 ? "Onaylama işlemi tamamlandı" : "Onay kaldırma işlemi başarıyla tamamlandı" });
        }

        // ------------------------------------------------------------------ rapor (eski kodla)

        /// <summary>Eski btnYazdir_Click: tur 1 Yazdır (seçili birim), 2 Kurum Yazdır, 3 Detay Yazdır (2 ve 3 yalnızca sistem yöneticisi).</summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int yil, int? muhasebe, string birim, int? tur)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int raporTur = tur ?? 1;
            if ((raporTur == 2 || raporTur == 3) && !SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (yil < IlkYil) return Hata("Yıl hatalı!");

            AracKiralamaPlanAna form = Form(yil, muhasebe ?? 0, birim);
            try
            {
                AsposeLisans.Yukle();
                global::ButceForm.AracKiralamaPlanRapor r = new global::ButceForm.AracKiralamaPlanRapor(TNS.BHF.Arac.Tanimla(), kullanan, Ayar.Oku("RaporSablonYol"), form);
                string dosya;
                if (raporTur == 2)
                    dosya = r.YazdirKurum();
                else if (raporTur == 3)
                    dosya = r.YazdirDetay();
                else
                    dosya = r.YazdirBirim(MuhBirimAdBul(form.muhasebeKod, form.birimKod, out _));
                if (string.IsNullOrEmpty(dosya) || !System.IO.File.Exists(dosya)) return Hata("Rapor hazırlanamadı.");
                return DosyaIslem.DosyaGonder(dosya, System.IO.Path.GetFileName(dosya), true, GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        // ------------------------------------------------------------------ onay işlemleri (yalnızca sistem yöneticisi)

        /// <summary>Eski DurumlariYukle: bütün birimlerin form durumu ve araç*ay / maliyet toplamları.</summary>
        private static IResult Durumlar(HttpContext baglam, IWebHostEnvironment ortam, int yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (yil < IlkYil) return Hata("Yıl hatalı!");
            try
            {
                AracKiralamaPlanAna frm = new AracKiralamaPlanAna();
                frm.yil = yil;
                string[] sql = new string[] { global::ButceForm.AracKiralamaPlanRapor.BirimSorgu(frm) };
                string[] baglanti = new string[] { "BaglantiSatiriBHM" };
                SortedList sl = TNS.UZY.Arac.Tanimla().SorguBirlestir(sql, baglanti, new int[] { 0, 1, 2, 3 });
                List<object> liste = new List<object>();
                foreach (DictionaryEntry de in sl)
                {
                    TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                    TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                    liste.Add(new
                    {
                        saymanlik = alAnahtar.IntAl(0),
                        birim = alAnahtar.StrAl(1),
                        birimAd = TNS.MUH.Arac.BirimAdOl(alAnahtar.StrAl(2), alAnahtar.StrAl(3)),
                        durum = alVeri.IntAl(0),
                        buYilMiktar = alVeri.IntAl(1),
                        buYilTutar = alVeri.DblAl(2),
                        gelecekYilMiktar = alVeri.IntAl(3),
                        oncYilMiktar = alVeri.IntAl(4),
                        oncYilTutar = alVeri.DblAl(5)
                    });
                }
                return Results.Json(new { satirlar = liste });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Birimler okunamadı."));
            }
        }

        /// <summary>Eski btnSeciliOnayla_Click: seçilen birimlerin formları onaylanır / onayı kaldırılır (kayıt yoksa eklenir).</summary>
        private static async Task<IResult> SeciliOnayla(HttpContext baglam, IWebHostEnvironment ortam, AracKiralamaSeciliOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || (istek.Durum != 5 && istek.Durum != 1)) return Results.Json(new { basarili = false, hata = "İşlem hatalı." });

            IBHFServis servis = TNS.BHF.Arac.Tanimla();
            int hatali = 0;
            int basarili = 0;
            foreach (AracKiralamaBirimSecimi b in istek.Birimler ?? new List<AracKiralamaBirimSecimi>())
            {
                if (b == null) continue;
                AracKiralamaPlanAna form = Form(istek.Yil, b.Saymanlik, b.Birim);
                form.durum = istek.Durum;
                Sonuc sonuc;
                try
                {
                    sonuc = servis.OnaylaAracKiralamaPlani(kullanan, form, true);
                }
                catch (Exception)
                {
                    sonuc = new Sonuc("Onay işlemi yapılamadı.");
                }
                if (!sonuc.islemSonuc)
                    hatali++;
                else
                    basarili++;
            }
            if (hatali == 0 && basarili == 0)
                return Results.Json(new { basarili = false, hata = "Seçim yapmanız gerekmektedir!" });
            string metin = istek.Durum == 5 ? "onaylandı" : "onayı kaldırıldı";
            string metinHata = istek.Durum == 5 ? "onaylanamadı" : "onayı kaldırılamadı";
            return Results.Json(new { basarili = hatali == 0, bilgi = $"{basarili} adet birim {metin} ve {hatali} adet birim {metinHata}!", hata = hatali > 0 ? $"{basarili} adet birim {metin} ve {hatali} adet birim {metinHata}!" : null });
        }
    }
}
