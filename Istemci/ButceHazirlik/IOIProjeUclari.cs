using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;

namespace Istemci.ButceHazirlik
{
    /// <summary>Proje tanım penceresinin kaydı (eski wndTanim: btnTanimKaydet_Click).</summary>
    public class IOIProjeKayit
    {
        public int siraNo { get; set; }
        public int isTur { get; set; }
        public int muhasebeKod { get; set; }
        public string birimKod { get; set; }
        public string ioiProjeAdi { get; set; }
        public int baslaYil { get; set; }
        public int bitisYil { get; set; }
        public double projeTutari { get; set; }
        public int ilKod { get; set; }
        public string aciklama { get; set; }
    }

    public class IOIProjeAnahtar
    {
        public int siraNo { get; set; }
    }

    /// <summary>Birimin İÖİ proje onayı (eski btnOnay_Click / btnOnayKaldir_Click).</summary>
    public class IOIProjeOnayIstegi
    {
        public int muhasebeKod { get; set; }
        public string birimKod { get; set; }
        public bool onayla { get; set; }
    }

    /// <summary>Tahsis penceresinin satırı (eski stoAktarim alanları; tarih yyyy-MM-dd).</summary>
    public class IOIProjeTahsisSatir
    {
        public string turStr { get; set; }
        public int aktarmaNo { get; set; }
        public string aktarmaTarihiDT { get; set; }
        public double aktarilanTutar { get; set; }
        public string aciklama { get; set; }
    }

    /// <summary>Harcama penceresinin satırı (eski stoHarcama alanları; tarih yyyy-MM-dd).</summary>
    public class IOIProjeHarcamaSatir
    {
        public int harcamaNo { get; set; }
        public string harcamaTarihiDT { get; set; }
        public double harcamaTutari { get; set; }
        public string aciklama { get; set; }
    }

    public class IOIProjeTahsisKayit
    {
        public int siraNo { get; set; }
        public List<IOIProjeTahsisSatir> satirlar { get; set; }
    }

    public class IOIProjeHarcamaKayit
    {
        public int siraNo { get; set; }
        public List<IOIProjeHarcamaSatir> satirlar { get; set; }
    }

    /// <summary>
    /// İl Özel İdaresi Projeleri ekranının (eski ButceHazirlikv2/IOIProjeListeForm.aspx) JavaScript'ten çağırdığı uçlar.
    ///   GET  /api/ioiproje/tanim                  açılış: yıl, iller, yönetici mi, kullanıcının yetkili muhasebe / birimi
    ///   GET  /api/ioiproje/liste?...              proje listesi (eski btnOku_Click: ListeleIOIProjeGiris)
    ///   GET  /api/ioiproje/oku?siraNo=            tanım penceresi (eski wndTanim_BeforeShow: OkuIOIProjeGiris)
    ///   GET  /api/ioiproje/birim?muhasebe=&amp;birim=  birim adı ve İÖİ proje onay durumu (eski txtBirim_Changed / TuslariAyarla)
    ///   POST /api/ioiproje/kaydet, /sil           (eski btnTanimKaydet_Click, btnTanimSil_Click)
    ///   POST /api/ioiproje/onay                   birimin onayı (eski btnOnay_Click; Onay Kaldır yalnızca sistem yöneticisi)
    ///   GET  /api/ioiproje/tahsis?siraNo=, POST tahsis/kaydet     (eski wndAkt)
    ///   GET  /api/ioiproje/harcama?siraNo=, POST harcama/kaydet   (eski wndHarc)
    ///   GET  /api/ioiproje/onaydurum, POST onaydurum/hepsi        (eski TavanGenelDurumBirim.aspx?onayKod=21; sistem yöneticisi)
    ///   GET  /api/ioiproje/rapor?tur=1|2|3&amp;...    1 Yazdır, 2 Yazdır (Detaylı), 3 Yazdır (Tertip) (eski kodla, Excel)
    /// İş kuralları eskisi gibi TNS.BHMv2 (IBHMv2Servis) servisinden çağrılır.
    /// </summary>
    public static class IOIProjeUclari
    {
        public const string SayfaAdresi = "../ButceHazirlikv2/IOIProjeListeForm.aspx";
        private const string U = "/api/ioiproje/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(U + "tanim", Tanim);
            app.MapGet(U + "liste", Liste);
            app.MapGet(U + "oku", Oku);
            app.MapGet(U + "birim", Birim);
            app.MapPost(U + "kaydet", Kaydet);
            app.MapPost(U + "sil", Sil);
            app.MapPost(U + "onay", Onay);
            app.MapGet(U + "tahsis", Tahsis);
            app.MapPost(U + "tahsis/kaydet", TahsisKaydet);
            app.MapGet(U + "harcama", Harcama);
            app.MapPost(U + "harcama/kaydet", HarcamaKaydet);
            app.MapGet(U + "onaydurum", OnayDurum);
            app.MapPost(U + "onaydurum/hepsi", HepsiniOnayla);
            app.MapGet(U + "rapor", Rapor);
        }

        // ------------------------------------------------------------------ yetki ve yardımcılar

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi);
        }

        private static bool Yonetici(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static async Task<IResult> DenetleYaz(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, Func<Kullanici, IResult> is_)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return is_(kullanan);
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static IResult Basarisiz(string hata) { return Results.Json(new { basarili = false, hata = hata }); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string TarihIso(TNSDateTime t)
        {
            if (t == null || TNSDateTime.IsNull(t)) return "";
            DateTime d = t.Oku();
            if (d == DateTime.MinValue || d.Year < 1900) return "";
            return d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private static TNSDateTime TarihOku(string s)
        {
            s = Temiz(s);
            if (s == "") return new TNSDateTime();
            string[] bicimler = { "yyyy-MM-dd", "dd.MM.yyyy", "d.M.yyyy" };
            if (DateTime.TryParseExact(s, bicimler, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                return new TNSDateTime(t);
            return new TNSDateTime();
        }

        /// <summary>Eski MuhasebeKodIntYap: onay tablosunda muhasebe kodu baştaki sıfırlar olmadan tutulur.</summary>
        private static string MuhasebeKodIntYap(int muhKod) { return muhKod.ToString(); }

        /// <summary>Eski IOIProjeListeForm.OnayliMi: birimin İÖİ proje onayı (durum 5).</summary>
        private static bool OnayliMi(Kullanici kullanan, int muhasebeKod, string birimKod)
        {
            TavanGenel tg = new TavanGenel();
            tg.kod1 = MuhasebeKodIntYap(muhasebeKod);
            tg.kod2 = Temiz(birimKod);
            tg = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayOku(kullanan, 0, "00.00", ENUMTavanTur.tavanTurIOIProje, tg);
            return tg != null && tg.durum == 5;
        }

        /// <summary>Eski KodAdOlMuhBirim: birim adı (GenelIslemler.KodAd 12, "birim-muhasebe").</summary>
        private static string BirimAdBul(int muhasebeKod, string birimKod)
        {
            birimKod = Temiz(birimKod);
            if (muhasebeKod <= 0 || birimKod == "") return "";
            try
            {
                return GenelIslemler.KodAd(12, birimKod + "-" + muhasebeKod.ToString(), false) ?? "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static object BirimDurum(Kullanici kullanan, int muhasebeKod, string birimKod)
        {
            string ad = BirimAdBul(muhasebeKod, birimKod);
            bool onayli = ad != "" && OnayliMi(kullanan, muhasebeKod, birimKod);
            return new
            {
                birimAd = ad,
                onayli = onayli,
                durum = ad == "" ? "" : (onayli ? "Onaylı" : "Veri Girilebilir")
            };
        }

        // ------------------------------------------------------------------ açılış

        /// <summary>Eski Page_Load: yıl, iller (YAT IlListele kod 100), yönetici mi, kullanıcının yetkili muhasebe / birimi.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                List<object> iller = new List<object>();
                TNS.YAT.BilgiVAN bv = TNS.YAT.Arac.Tanimla().IlListele(kullanan, new TNS.YAT.Sozluk() { kod = 100 });
                if (bv != null && bv.liste != null)
                    foreach (TNS.YAT.Sozluk il in bv.liste)
                        iller.Add(new { kod = il.kod, ad = il.ad });

                string yetkiSaymanlik, yetkiBirim;
                TNS.MUH.Arac.YetkiliBirimBul(kullanan, out yetkiSaymanlik, out yetkiBirim);
                yetkiSaymanlik = Temiz(yetkiSaymanlik);
                yetkiBirim = Temiz(yetkiBirim);
                return Results.Json(new
                {
                    yil = DateTime.Now.Year,
                    iller = iller,
                    yonetici = Yonetici(kullanan),
                    muhasebe = yetkiSaymanlik,
                    birim = yetkiBirim,
                    birimAd = BirimAdBul(OrtakFonksiyonlar.ConvertToInt(yetkiSaymanlik, 0), yetkiBirim),
                    listele = yetkiSaymanlik != "" && yetkiBirim != ""
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        // ------------------------------------------------------------------ liste

        /// <summary>Eski KriterTopla (alt / üst ödenek eskisi gibi projeTutari / iOIHarc alanlarına tam sayı olarak konur).</summary>
        private static IOIProje Kriter(int? tur, string ad, int? basYil, int? bitYil, string altOdenek, string ustOdenek, int? il)
        {
            IOIProje form = new IOIProje();
            form.isTur = tur == 1 || tur == 2 ? tur.Value : 0;
            form.ilKod = il ?? 0;
            form.ioiProjeAdi = Temiz(ad);
            form.baslaYil = basYil ?? 0;
            form.bitisYil = bitYil ?? 0;
            form.projeTutari = OrtakFonksiyonlar.ConvertToInt(Temiz(altOdenek), 0);
            form.iOIHarc = OrtakFonksiyonlar.ConvertToInt(Temiz(ustOdenek), 0);
            return form;
        }

        private static object SatirJson(IOIProje p)
        {
            return new
            {
                siraNo = p.siraNo,
                isTur = p.isTur,
                muhasebeKod = p.muhasebeKod,
                birimKod = p.birimKod ?? "",
                muhasebeBirimAd = p.muhasebeBirimAd ?? "",
                DPTProjeNo = p.DPTProjeNo ?? "",
                projeAdi = p.projeAdi ?? "",
                altProjeAdi = p.altProjeAdi ?? "",
                ioiProjeAdi = p.ioiProjeAdi ?? "",
                baslaYil = p.baslaYil,
                bitisYil = p.bitisYil,
                projeTutari = p.projeTutari,
                aktarilanTutar = p.aktarilanTutar,
                islemleEklenen = p.islemleEklenen,
                toplamButce = p.toplamButce,
                iOIHarc = p.iOIHarc,
                gonderilen = p.gonderilen,
                toplamHarc = p.toplamHarc,
                ilKod = p.ilKod,
                ilAdi = p.ilAdi ?? "",
                aciklama = p.aciklama ?? ""
            };
        }

        /// <summary>Eski Listele: rapor yılı (spnYilRap) bütçe yılı olarak verilir.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? tur, string ad, int? basYil, int? bitYil,
            string altOdenek, string ustOdenek, int? il, int? yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            ObjectArray oa;
            try
            {
                oa = TNS.BHMv2.Arac.Tanimla().ListeleIOIProjeGiris(kullanan, Kriter(tur, ad, basYil, bitYil, altOdenek, ustOdenek, il), false, yil ?? DateTime.Now.Year);
            }
            catch (Exception)
            {
                return Hata("Projeler okunamadı.");
            }
            if (oa == null || !oa.sonuc.islemSonuc) return Hata(HataMetni(oa?.sonuc?.hataStr, "Projeler okunamadı."));
            List<object> satirlar = new List<object>();
            foreach (IOIProje p in oa.objeler)
                satirlar.Add(SatirJson(p));
            return Results.Json(new { satirlar = satirlar });
        }

        // ------------------------------------------------------------------ tanım penceresi

        /// <summary>Eski wndTanim_BeforeShow / Goster: proje bilgileri, bağlı alt faaliyetler ve birimin onay durumu.</summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int siraNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (siraNo <= 0) return Hata("Lütfen proje seçiniz!");
            ObjectVAN ov;
            try
            {
                ov = TNS.BHMv2.Arac.Tanimla().OkuIOIProjeGiris(kullanan, new IOIProje() { siraNo = siraNo });
            }
            catch (Exception)
            {
                return Hata("Proje okunamadı.");
            }
            if (ov == null || !ov.sonuc.islemSonuc) return Hata(HataMetni(ov?.sonuc?.hataStr, "Proje okunamadı."));
            if (ov.o == null) return Hata("Kayıt bulunamadı.");
            IOIProje p = (IOIProje)ov.o;
            List<object> pebKodlar = new List<object>();
            if (p.pebKodlar != null)
                foreach (IOIProjeBag b in p.pebKodlar)
                    pebKodlar.Add(new { yil = b.yil, pebKod = b.pebKod ?? "", pebAd = b.pebAd ?? "", projeAd = b.projeAd ?? "", altProjeAd = b.altProjeAd ?? "" });
            return Results.Json(new
            {
                proje = SatirJson(p),
                pebKodlar = pebKodlar,
                birim = BirimDurum(kullanan, p.muhasebeKod, p.birimKod)
            });
        }

        private static IResult Birim(HttpContext baglam, IWebHostEnvironment ortam, int? muhasebe, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                return Results.Json(BirimDurum(kullanan, muhasebe ?? 0, birim));
            }
            catch (Exception)
            {
                return Hata("Birim bilgisi okunamadı.");
            }
        }

        /// <summary>Eski btnTanimKaydet_Click: tür seçilmemişse İÖİ sayılır (eski radYIKOB değilse 1).</summary>
        private static Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IOIProjeKayit istek, IAntiforgery sahtecilik)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                if (istek == null) return Basarisiz("Kaydedilecek bilgi yok.");
                IOIProje form = new IOIProje();
                form.siraNo = istek.siraNo;
                form.muhasebeKod = istek.muhasebeKod;
                form.birimKod = Temiz(istek.birimKod);
                form.isTur = istek.isTur == 2 ? 2 : 1;
                form.ioiProjeAdi = Temiz(istek.ioiProjeAdi);
                form.baslaYil = istek.baslaYil;
                form.bitisYil = istek.bitisYil;
                form.projeTutari = istek.projeTutari;
                form.ilKod = istek.ilKod;
                form.aciklama = Temiz(istek.aciklama);
                Sonuc sonuc;
                try
                {
                    sonuc = TNS.BHMv2.Arac.Tanimla().KaydetIOIProjeGiris(kullanan, form);
                }
                catch (Exception)
                {
                    return Basarisiz("Kayıt işlemi başarısız.");
                }
                if (!sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız."));
                int sira = form.siraNo == 0 ? OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0) : form.siraNo;
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi tamamlandı.", siraNo = sira });
            });
        }

        /// <summary>Eski btnTanimSil_Click: silinen proje yeniden kaydedilirse yeni proje olur.</summary>
        private static Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, IOIProjeAnahtar istek, IAntiforgery sahtecilik)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                if (istek == null || istek.siraNo <= 0) return Basarisiz("Lütfen silinecek projeyi seçiniz!");
                Sonuc sonuc;
                try
                {
                    sonuc = TNS.BHMv2.Arac.Tanimla().SilIOIProjeGiris(kullanan, new IOIProje() { siraNo = istek.siraNo });
                }
                catch (Exception)
                {
                    return Basarisiz("Silme işlemi başarısız.");
                }
                if (!sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc.hataStr, "Silme işlemi başarısız."));
                return Results.Json(new { basarili = true, bilgi = "Kayıt silindi, tekrar kaydederseniz yeni proje olarak kaydedilecektir!" });
            });
        }

        /// <summary>Eski btnOnay_Click (durum 5) / btnOnayKaldir_Click (durum 1; yalnızca sistem yöneticisi).</summary>
        private static Task<IResult> Onay(HttpContext baglam, IWebHostEnvironment ortam, IOIProjeOnayIstegi istek, IAntiforgery sahtecilik)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                if (istek == null || istek.muhasebeKod <= 0 || Temiz(istek.birimKod) == "") return Basarisiz("Lütfen muhasebe ve birim seçiniz!");
                if (!istek.onayla && !Yonetici(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
                TavanGenel tg = new TavanGenel();
                tg.kod1 = MuhasebeKodIntYap(istek.muhasebeKod);
                tg.kod2 = Temiz(istek.birimKod);
                tg.durum = istek.onayla ? 5 : 1;
                string ne = istek.onayla ? "Onaylama" : "Onay kaldırma";
                Sonuc s;
                try
                {
                    s = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayKaydet(kullanan, 0, "00.00", ENUMTavanTur.tavanTurIOIProje, tg);
                }
                catch (Exception)
                {
                    return Basarisiz(ne + " işlemi başarısız.");
                }
                if (s == null || !s.islemSonuc) return Basarisiz(ne + " işlemi başarısız: " + HataMetni(s?.hataStr, ""));
                return Results.Json(new { basarili = true, bilgi = ne + " işlemi başarılı", birim = BirimDurum(kullanan, istek.muhasebeKod, istek.birimKod) });
            });
        }

        // ------------------------------------------------------------------ tahsis ve harcama pencereleri

        /// <summary>Eski wndAktarim_BeforeShow.</summary>
        private static IResult Tahsis(HttpContext baglam, IWebHostEnvironment ortam, int siraNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            ObjectArray oa;
            try
            {
                oa = TNS.BHMv2.Arac.Tanimla().IOIProjeAktarimListele(kullanan, siraNo);
            }
            catch (Exception)
            {
                return Hata("Tahsisler okunamadı.");
            }
            if (oa == null || !oa.sonuc.islemSonuc) return Hata(HataMetni(oa?.sonuc?.hataStr, "Tahsisler okunamadı."));
            List<object> satirlar = new List<object>();
            foreach (IOIProjeAktarilan a in oa.objeler)
                satirlar.Add(new { turStr = a.turStr, aktarmaNo = a.aktarmaNo, aktarmaTarihiDT = TarihIso(a.aktarmaTarihi), aktarilanTutar = a.aktarilanTutar, aciklama = a.aciklama ?? "" });
            return Results.Json(new { satirlar = satirlar });
        }

        /// <summary>Eski btnAktarimKaydet_Click: tutarı 0 ve açıklaması boş satır silinir (iş kuralı).</summary>
        private static Task<IResult> TahsisKaydet(HttpContext baglam, IWebHostEnvironment ortam, IOIProjeTahsisKayit istek, IAntiforgery sahtecilik)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                if (istek == null || istek.siraNo <= 0) return Basarisiz("Lütfen proje numarasını veriniz!");
                List<IOIProjeAktarilan> l = new List<IOIProjeAktarilan>();
                foreach (IOIProjeTahsisSatir s in istek.satirlar ?? new List<IOIProjeTahsisSatir>())
                {
                    IOIProjeAktarilan a = new IOIProjeAktarilan();
                    a.turStr = Temiz(s.turStr);
                    a.aktarmaNo = s.aktarmaNo;
                    a.aktarmaTarihi = TarihOku(s.aktarmaTarihiDT);
                    a.aktarilanTutar = s.aktarilanTutar;
                    a.aciklama = Temiz(s.aciklama);
                    l.Add(a);
                }
                Sonuc sonuc;
                try
                {
                    sonuc = TNS.BHMv2.Arac.Tanimla().IOIProjeAktarimKaydet(kullanan, istek.siraNo, l.ToArray());
                }
                catch (Exception)
                {
                    return Basarisiz("Kayıt işlemi başarısız.");
                }
                if (!sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız."));
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi tamamlandı." });
            });
        }

        /// <summary>Eski wndHarcama_BeforeShow.</summary>
        private static IResult Harcama(HttpContext baglam, IWebHostEnvironment ortam, int siraNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            ObjectArray oa;
            try
            {
                oa = TNS.BHMv2.Arac.Tanimla().IOIProjeHarcamaListele(kullanan, siraNo);
            }
            catch (Exception)
            {
                return Hata("Harcamalar okunamadı.");
            }
            if (oa == null || !oa.sonuc.islemSonuc) return Hata(HataMetni(oa?.sonuc?.hataStr, "Harcamalar okunamadı."));
            List<object> satirlar = new List<object>();
            foreach (IOIProjeHarcama h in oa.objeler)
                satirlar.Add(new { harcamaNo = h.harcamaNo, harcamaTarihiDT = TarihIso(h.harcamaTarihi), harcamaTutari = h.harcamaTutari, aciklama = h.aciklama ?? "" });
            return Results.Json(new { satirlar = satirlar });
        }

        /// <summary>Eski btnHarcamaKaydet_Click: tutarı 0 ve açıklaması boş satır silinir (iş kuralı).</summary>
        private static Task<IResult> HarcamaKaydet(HttpContext baglam, IWebHostEnvironment ortam, IOIProjeHarcamaKayit istek, IAntiforgery sahtecilik)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                if (istek == null || istek.siraNo <= 0) return Basarisiz("Lütfen proje numarasını veriniz!");
                List<IOIProjeHarcama> l = new List<IOIProjeHarcama>();
                foreach (IOIProjeHarcamaSatir s in istek.satirlar ?? new List<IOIProjeHarcamaSatir>())
                {
                    IOIProjeHarcama h = new IOIProjeHarcama();
                    h.harcamaNo = s.harcamaNo;
                    h.harcamaTarihi = TarihOku(s.harcamaTarihiDT);
                    h.harcamaTutari = s.harcamaTutari;
                    h.aciklama = Temiz(s.aciklama);
                    l.Add(h);
                }
                Sonuc sonuc;
                try
                {
                    sonuc = TNS.BHMv2.Arac.Tanimla().IOIProjeHarcamaKaydet(kullanan, istek.siraNo, l.ToArray());
                }
                catch (Exception)
                {
                    return Basarisiz("Kayıt işlemi başarısız.");
                }
                if (!sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız."));
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi tamamlandı." });
            });
        }

        // ------------------------------------------------------------------ onay durumu (eski TavanGenelDurumBirim.aspx?onayKod=21)

        private const int OnayKodIOI = 21;

        private class OnayDurumSatir
        {
            public int muhKod;
            public string birimKod;
            public string birimAd;
            public int projeSay;
            public int durumKod;
        }

        /// <summary>Eski TavanGenelDurumBirim.FormOl: açık muhasebe birimleri, onay durumları ve İÖİ proje sayıları.</summary>
        private static List<OnayDurumSatir> OnayDurumOku(bool projeSayVar)
        {
            List<OnayDurumSatir> l = new List<OnayDurumSatir>();
            int n = projeSayVar ? 3 : 2;
            string[] sorgular = new string[n];
            string[] baglar = new string[n];
            sorgular[0] = "select CONVERT(INT,KOD1), KOD2, ONAY from BHMTAVANGENELONAY WHERE TAVANTUR = " + OnayKodIOI.ToString() + " AND YIL=0 AND ASAMA='0000' order by 1,2";
            sorgular[1] = "select M.MUHASEBEKOD, B.BIRIMKOD, MUHASEBEAD, BIRIMAD from MUHMUHASEBE M INNER JOIN MUHMUHASEBEBIRIM B ON M.MUHASEBEKOD=B.MUHASEBEKOD where BIRIMKOD<>'000' AND 1 NOT IN (SELECT 1 FROM MUHMUHASEBEBIRIMKAPALI K where K.MUHASEBEKOD=B.MUHASEBEKOD AND K.BIRIMKOD=B.BIRIMKOD AND K.KURKOD=B.KURKOD) ORDER BY 1,2";
            baglar[0] = "BaglantiSatiriBHMv2";
            baglar[1] = "BaglantiSatiriBUM";
            if (projeSayVar)
            {
                sorgular[2] = "select MUHASEBEKOD, BIRIMKOD, COUNT(*) from BHMV2IOIPROJE GROUP BY MUHASEBEKOD, BIRIMKOD ORDER BY 1,2";
                baglar[2] = "BaglantiSatiriBHMv2";
            }
            SortedList lst = TNS.UZY.Arac.Tanimla().SorguBirlestir(sorgular, baglar, new int[] { 0, 1 });
            foreach (DictionaryEntry de in lst)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                OnayDurumSatir od = new OnayDurumSatir();
                od.muhKod = alAnahtar.IntAl(0);
                od.birimKod = alAnahtar.StrAl(1);
                od.durumKod = alVeri.IntAl(0);
                od.birimAd = TNS.MUH.Arac.BirimAdOl(alVeri.StrAl(1), alVeri.StrAl(2));
                if (projeSayVar)
                    od.projeSay = alVeri.IntAl(3);
                l.Add(od);
            }
            return l;
        }

        private static IResult OnayDurum(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!Yonetici(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            try
            {
                List<object> satirlar = new List<object>();
                foreach (OnayDurumSatir od in OnayDurumOku(true))
                    satirlar.Add(new { muhKod = od.muhKod, birimKod = od.birimKod, birimAd = od.birimAd, projeSay = od.projeSay, durum = od.durumKod > 2 ? "Onaylı" : "Onaysız" });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Onay durumu okunamadı.");
            }
        }

        /// <summary>Eski btnHepOnayla_Click: onaylı olmayan bütün birimler onaylanır.</summary>
        private static Task<IResult> HepsiniOnayla(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                if (!Yonetici(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
                int hataSay = 0;
                try
                {
                    IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                    foreach (OnayDurumSatir item in OnayDurumOku(false))
                    {
                        if (item.durumKod >= 5) continue;
                        TavanGenel tg = new TavanGenel();
                        tg.kod1 = item.muhKod.ToString();
                        tg.kod2 = item.birimKod;
                        tg.durum = 5;
                        Sonuc s = servis.BHMTavanGenelOnayKaydet(kullanan, 0, "00.00", ENUMTavanTur.tavanTurIOIProje, tg);
                        if (s == null || !s.islemSonuc) hataSay++;
                    }
                }
                catch (Exception)
                {
                    return Basarisiz("Onaylama işlemi başarısız.");
                }
                return Results.Json(new
                {
                    basarili = true,
                    bilgi = hataSay == 0 ? "Bütün birimler onaylandı." : hataSay.ToString() + " birim onaylanamadı."
                });
            });
        }

        // ------------------------------------------------------------------ raporlar (eski kodla, Excel)

        /// <summary>İÖİ şablonu: önce kurumun RaporSablonYol\BHMv2 klasörü, yoksa uygulamayla gelen SGB.Net\RaporSablon\BHMv2.</summary>
        private static string SablonBul(string uygulamaKlasoru, string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "BHMv2", sablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "BHMv2", sablonAd);
            if (File.Exists(u)) return u;
            throw new Exception("Rapor şablonu bulunamadı (BHMv2\\" + sablonAd + ").");
        }

        /// <summary>tur 1: Yazdır, 2: Yazdır (Detaylı) (eski Yazdir), 3: Yazdır (Tertip) (eski btnYazdirTertip_Click).</summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int? tur, int? isTur, string ad, int? basYil, int? bitYil,
            string altOdenek, string ustOdenek, int? il, int? yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int y = yil ?? DateTime.Now.Year;
            try
            {
                AsposeLisans.Yukle();
                if (tur == 3) return YazdirTertip(kullanan, y);
                return Yazdir(kullanan, ortam.ContentRootPath, Kriter(isTur, ad, basYil, bitYil, altOdenek, ustOdenek, il), y, tur == 2);
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        /// <summary>Eski Yazdir(detayli): IOIProjeListe.xltx şablonuna proje listesi; detaylıda tahsis / harcama satırları.</summary>
        private static IResult Yazdir(Kullanici kullanan, string uygulamaKlasoru, IOIProje form, int yil, bool detayli)
        {
            ObjectArray oa = TNS.BHMv2.Arac.Tanimla().ListeleIOIProjeGiris(kullanan, form, detayli, yil);
            if (!oa.sonuc.islemSonuc) return Hata(HataMetni(oa.sonuc.hataStr, "Projeler okunamadı."));

            string uzanti = GenelIslemler.ExcelTur();
            Tablo XLS = GenelIslemler.NewTablo();
            string sonucDosya = "IOIProje_" + DosyaIslem.DosyaAdUretSade() + "." + uzanti;
            string sonucDosyaYol = DosyaIslem.DosyaAdUret() + "." + uzanti;
            XLS.DosyaAc(SablonBul(uygulamaKlasoru, "IOIProjeListe.xltx"), sonucDosyaYol);
            int fsheetNo = 0;
            int fsatir1 = 0;
            int fsutun1 = 0;
            int fsatir2 = 0;
            int fsutun2 = 0;

            XLS.HucreAdAdresCoz("FormatSatir", ref fsheetNo, ref fsatir1, ref fsutun1, ref fsatir2, ref fsutun2);
            int satir = 13;
            int sutun = 2;
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            int baslaSatir = satir;
            int satirYukseklik = XLS.SatirYukseklikAl(baslaSatir);
            for (int i = 0; i < oa.ObjeSayisi; i++)
            {
                IOIProje p = (IOIProje)oa[i];
                XLS.HucreKopyala(fsatir1, fsutun1, fsatir2, fsutun2, satir, sutun);

                int icSutun = sutun;
                XLS.HucreDegerYaz(satir, icSutun++, p.siraNo);
                XLS.HucreDegerYaz(satir, icSutun++, p.muhasebeKod);
                XLS.HucreDegerYaz(satir, icSutun++, p.birimKod);
                XLS.HucreDegerYaz(satir, icSutun++, p.muhasebeBirimAd);
                XLS.HucreDegerYaz(satir, icSutun++, p.DPTProjeNo);
                XLS.HucreDegerYaz(satir, icSutun++, p.projeAdi);
                XLS.HucreDegerYaz(satir, icSutun++, p.altProjeAdi);
                XLS.HucreDegerYaz(satir, icSutun++, p.ioiProjeAdi);
                XLS.HucreDegerYaz(satir, icSutun++, p.baslaYil);
                XLS.HucreDegerYaz(satir, icSutun++, p.bitisYil);
                XLS.HucreDegerYaz(satir, icSutun++, p.projeTutari);
                XLS.HucreDegerYaz(satir, icSutun++, p.toplamButce);
                XLS.HucreDegerYaz(satir, icSutun++, p.toplamHarc);
                XLS.HucreDegerYaz(satir, icSutun++, p.aciklama);
                XLS.HucreDegerYaz(satir, icSutun++, p.isTur == 1 ? "İÖİ" : "YİKOB");

                satir++;
                if (detayli && p.detaylar != null)
                {
                    foreach (IOIDetayBilgi item in p.detaylar)
                    {
                        XLS.HucreKopyala(fsatir1 + 1, fsutun1, fsatir2 + 1, fsutun2, satir, sutun);

                        XLS.HucreDegerYaz(satir, sutun + 0, p.siraNo);
                        XLS.HucreDegerYaz(satir, sutun + 4, item.islemTarihi.Oku());
                        XLS.HucreDegerYaz(satir, sutun + 5, item.aciklama);
                        XLS.HucreDegerYaz(satir, sutun + 11, item.aktarilanTutar);
                        XLS.HucreDegerYaz(satir, sutun + 12, item.harcamaTutari);
                        satir++;
                    }
                }
            }
            XLS.SatirYukseklikAyarla(baslaSatir, satir, -1, 5, satirYukseklik);
            XLS.DosyaSaklaTamYol();
            return DosyaIslem.DosyaGonder(sonucDosyaYol, sonucDosya, true, uzanti);
        }

        private static void KYKodParcala(string kyKod, out int sayKod, out string birKod)
        {
            sayKod = 0;
            birKod = "";
            if (!string.IsNullOrWhiteSpace(kyKod) && kyKod.Length >= 10)
            {
                sayKod = OrtakFonksiyonlar.ConvertToInt(kyKod.Substring(1, 5), 0);
                birKod = kyKod.Substring(7, 3);
            }
        }

        /// <summary>Eski btnYazdirTertip_Click: masraf planı tahsisleri ve gönderilen ödenekler, iş ve tertip bazında (şablonsuz Excel).</summary>
        private static IResult YazdirTertip(Kullanici kullanan, int yil)
        {
            string[] sql = new string[2];
            string[] baglanti = new string[2];

            ObjectArray oa = TNS.BHMv2.Arac.Tanimla().ListeleIOIProjeGiris(kullanan, new IOIProje(), false, yil);
            if (!oa.sonuc.islemSonuc) return Hata(HataMetni(oa.sonuc.hataStr, "Projeler okunamadı."));

            Dictionary<int, IOIProje> isler = new Dictionary<int, IOIProje>();
            for (int i = 0; i < oa.ObjeSayisi; i++)
            {
                IOIProje p = (IOIProje)oa[i];
                isler[p.siraNo] = p;
            }

            string uzanti = GenelIslemler.ExcelTur();
            Tablo XLS = GenelIslemler.NewTablo();

            sql[0] = $@"SELECT IOIISKOD, MPGMASRAFPLANITERTIP.KURKOD, MPGMASRAFPLANITERTIP.FONKOD, MPGMASRAFPLANITERTIP.FINKOD, MPGMASRAFPLANITERTIP.EKOKOD, MPGMASRAFPLANITERTIP.PEBKOD, KURUMSALYAPIKOD, KY.AD, IOIPROJEADI, ISTUR, PEBKOD.PEBAD, SUM(ODENEK), MIN(MPGMASRAFPLANIREVIZEFORM.EVRAKTARIH)
FROM MPGMASRAFPLANITERTIP
INNER JOIN MPGMASRAFPLANIREVIZEFORM ON MPGMASRAFPLANIREVIZEFORM.YIL = MPGMASRAFPLANITERTIP.YIL AND MPGMASRAFPLANIREVIZEFORM.REVIZENO = MPGMASRAFPLANITERTIP.REVIZENO
INNER JOIN ButceHazirlik.dbo.ABSPEBKOD PEBKOD ON PEBKOD.YIL=MPGMASRAFPLANITERTIP.YIL AND PEBKOD.PEBKOD=LEFT(MPGMASRAFPLANITERTIP.PEBKOD,2)
INNER JOIN BHMKURUMSALYAPI KY ON KY.KOD=MPGMASRAFPLANITERTIP.KURUMSALYAPIKOD
INNER JOIN ButceHazirlikv2.dbo.BHMV2IOIPROJE P ON P.SIRANO=IOIISKOD
WHERE MPGMASRAFPLANITERTIP.YIL={yil} AND DURUM<=5 AND DURUM>1 AND IOIISKOD > 0
GROUP BY MPGMASRAFPLANITERTIP.KURKOD, MPGMASRAFPLANITERTIP.FONKOD, MPGMASRAFPLANITERTIP.FINKOD, MPGMASRAFPLANITERTIP.EKOKOD, MPGMASRAFPLANITERTIP.PEBKOD, KURUMSALYAPIKOD, IOIISKOD, KY.AD, IOIPROJEADI, ISTUR, PEBKOD.PEBAD order by 1,5";

            sql[1] = $@"SELECT F.IOIISKOD, D.KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD,  KY.KOD, KY.AD, IOIPROJEADI, ISTUR, SUM(D.MIKTAR)
FROM MUHGONDERMETENKISFORM F
INNER JOIN MUHGONDERMETENKIS D ON F.YIL = D.YIL AND F.BELGETUR = D.BELGETUR AND F.BELGENO = D.BELGENO
INNER JOIN BHMKURUMSALYAPI KY ON D.KURKOD = KY.KURKOD AND CONVERT(VARCHAR(20), F.MUHASEBEKOD) = KY.SAYMANLIKKOD AND F.BIRIMKOD = KY.BIRIMKOD
INNER JOIN ButceHazirlikv2.dbo.BHMV2IOIPROJE P ON P.SIRANO=F.IOIISKOD
WHERE D.YIL={yil} AND F.FORMDURUM<7 AND F.FORMDURUM>3 AND F.IOIISKOD > 0
GROUP BY D.KURKOD, D.FONKOD, D.FINKOD, D.EKOKOD, D.PEBKOD, KY.KOD, F.IOIISKOD, KY.AD, IOIPROJEADI, ISTUR order by 1,5";
            baglanti[0] = "BaglantiSatiriBUM";
            baglanti[1] = "BaglantiSatiriBUM";
            SortedList sl = TNS.UZY.Arac.Tanimla().SorguBirlestir(sql, baglanti, new int[] { 0, 1, 2, 3, 4, 5, 6 });

            string sonucDosya = "IOIProje_" + DosyaIslem.DosyaAdUretSade() + "." + uzanti;
            string sonucDosyaYol = DosyaIslem.DosyaAdUret() + "." + uzanti;
            XLS.BosDosyaAc(sonucDosyaYol);
            int satir = 0;
            int kol = 0;
            XLS.HucreDegerYaz(satir, kol, "#");
            XLS.SutunGenislikAyarla(kol, kol++, 4);

            XLS.HucreDegerYaz(satir, kol, "Muh.\rKod");
            XLS.SutunGenislikAyarla(kol, kol, 7);
            XLS.HucreMetniKaydir(satir, kol++, true);

            XLS.HucreDegerYaz(satir, kol, "Birim\rKod");
            XLS.SutunGenislikAyarla(kol, kol, 6);
            XLS.HucreMetniKaydir(satir, kol++, true);

            XLS.HucreDegerYaz(satir, kol, "Birim Adı");
            XLS.SutunGenislikAyarla(kol, kol, 40);
            XLS.HucreMetniKaydir(satir, kol++, true);

            XLS.HucreDegerYaz(satir, kol, "DPT\rProje\rNo");
            XLS.HucreMetniKaydir(satir, kol, true);
            XLS.SutunGenislikAyarla(kol, kol++, 12);

            XLS.HucreDegerYaz(satir, kol, "DPT Proje Adı");
            XLS.HucreMetniKaydir(satir, kol++, true);

            XLS.HucreDegerYaz(satir, kol, "DPT Alt Proje Adı");
            XLS.HucreMetniKaydir(satir, kol++, true);

            XLS.HucreDegerYaz(satir, kol, "İÖİ Proje Adı");
            XLS.HucreMetniKaydir(satir, kol++, true);
            XLS.SutunGenislikAyarla(kol - 3, kol - 1, 40);

            XLS.HucreDegerYaz(satir, kol, "İş\rTürü");
            XLS.HucreMetniKaydir(satir, kol, true);
            XLS.SutunGenislikAyarla(kol, kol++, 6);

            XLS.HucreDegerYaz(satir, kol, "Açıklama");
            XLS.SutunGenislikAyarla(kol, kol, 40);
            XLS.HucreMetniKaydir(satir, kol++, true);

            XLS.HucreDegerYaz(satir, kol, "Proje\rTutarı");
            XLS.HucreMetniKaydir(satir, kol, true);
            XLS.SutunGenislikAyarla(kol, kol, 15);
            XLS.HucreFormatla(satir + 1, kol, satir + 1, kol++, "#,##0.00");

            XLS.HucreDegerYaz(satir, kol, "Baş.\rYılı");
            XLS.HucreMetniKaydir(satir, kol, true);
            XLS.SutunGenislikAyarla(kol, kol++, 8);

            XLS.HucreDegerYaz(satir, kol, "Bitiş\rYılı");
            XLS.HucreMetniKaydir(satir, kol, true);
            XLS.SutunGenislikAyarla(kol, kol++, 8);

            XLS.HucreDegerYaz(satir, kol, "İlgili G.M.");
            XLS.SutunGenislikAyarla(kol, kol++, 40);

            XLS.HucreDegerYaz(satir, kol, "Tahsis\rTarihi");
            XLS.HucreMetniKaydir(satir, kol, true);
            XLS.SutunGenislikAyarla(kol, kol++, 10);

            XLS.HucreDegerYaz(satir, kol++, "Tertip (SGB)");
            XLS.HucreDegerYaz(satir, kol++, "Tertip (PRG)");
            XLS.SutunGenislikAyarla(kol - 2, kol - 1, 35);

            XLS.HucreDegerYaz(satir, kol++, "Tahsis");
            XLS.HucreDegerYaz(satir, kol++, "Gönderilen");
            XLS.SutunGenislikAyarla(kol - 2, kol - 1, 15);
            XLS.HucreFormatla(satir + 1, kol - 2, satir + 1, kol - 1, "#,##0.00");

            XLS.DuseyHizala(satir, 0, satir, kol, 2);

            satir++;

            XLS.YatayHizala(satir, 0, satir, kol, 1);
            satir++;
            int pOncioiIsKod = -1;

            foreach (DictionaryEntry de in sl)
            {
                int sayKod;
                string birKod;
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                string kurKod = alAnahtar.StrAl(1);
                string fonKod = alAnahtar.StrAl(2);
                string finKod = alAnahtar.StrAl(3);
                string ekoKod = alAnahtar.StrAl(4);
                string pebKod = alAnahtar.StrAl(5);
                string kyKod = alAnahtar.StrAl(6);
                int ioiIsKod = alAnahtar.IntAl(0);

                Tertip trtp = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kurKod, fonKod, finKod, ekoKod, pebKod);
                trtp.Noktala();

                string kyAd = alVeri.StrAl(0);
                string ioiIsAdAd = alVeri.StrAl(1);
                int ioiIsTur = alVeri.IntAl(2);
                string birimAd = alVeri.StrAl(3);
                double tahsis = alVeri.DblAl(4);
                TNSDateTime dt = alVeri.DTAl(5);

                if (string.IsNullOrWhiteSpace(kyAd))
                    kyAd = alVeri.StrAl(6);
                if (string.IsNullOrWhiteSpace(ioiIsAdAd))
                    ioiIsAdAd = alVeri.StrAl(7);
                if (ioiIsTur == 0)
                    ioiIsTur = alVeri.IntAl(8);

                double gonderilen = alVeri.DblAl(9);

                XLS.HucreKopyala(1, 0, 1, 19, satir, 0);

                XLS.HucreDegerYaz(satir, 0, ioiIsKod);

                IOIProje pSimd = null;
                if (!isler.TryGetValue(ioiIsKod, out pSimd))
                {
                    pSimd = new IOIProje();
                    pSimd.siraNo = ioiIsKod;
                }
                KYKodParcala(kyKod, out sayKod, out birKod);

                XLS.HucreDegerYaz(satir, 1, sayKod);
                XLS.HucreDegerYaz(satir, 2, birKod);
                XLS.HucreDegerYaz(satir, 3, kyAd);
                XLS.HucreDegerYaz(satir, 7, ioiIsAdAd);
                XLS.HucreDegerYaz(satir, 8, ioiIsTur == 1 ? "İÖİ" : "YİKOB");

                XLS.HucreDegerYaz(satir, 4, pSimd.DPTProjeNo);
                XLS.HucreDegerYaz(satir, 5, pSimd.projeAdi);
                XLS.HucreDegerYaz(satir, 6, pSimd.altProjeAdi);
                XLS.HucreDegerYaz(satir, 9, pSimd.aciklama);

                if (pOncioiIsKod != ioiIsKod)
                {
                    XLS.HucreDegerYaz(satir, 10, pSimd.projeTutari);
                }

                XLS.HucreDegerYaz(satir, 11, pSimd.baslaYil);
                XLS.HucreDegerYaz(satir, 12, pSimd.bitisYil);
                pOncioiIsKod = ioiIsKod;

                XLS.HucreDegerYaz(satir, 13, birimAd);
                if (!TNSDateTime.IsNull(dt))
                    XLS.HucreDegerYaz(satir, 14, dt.ToString());
                XLS.HucreDegerYaz(satir, 15, trtp.TertipStrOl());
                XLS.HucreDegerYaz(satir, 16, TNS.BHMv2.Arac.ProgramTertipOl(trtp, false));

                XLS.HucreDegerYaz(satir, 17, tahsis);
                XLS.HucreDegerYaz(satir, 18, gonderilen);
                satir++;
            }

            XLS.KoyuYap(0, 0, 0, 18, true);
            XLS.SatirSil(1, 1);
            XLS.YatayCizgiCiz(1, 0, 18, LineStyle.THIN, TabloRenk.BLACK);
            XLS.SatirYukseklikAyarla(0, 0, -1, 5, 50);
            XLS.SatirYukseklikAyarla(1, satir, -1, 1, 10);
            XLS.DosyaSaklaTamYol();
            return DosyaIslem.DosyaGonder(sonucDosyaYol, sonucDosya, true, uzanti);
        }
    }
}
