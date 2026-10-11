using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
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
using TNS.BKD;
using TNS.KYM;
using TNS.PEB;

namespace Istemci.StratejikPlan
{
    public class DonemIstegi
    {
        public string KurumKod { get; set; }
        public int Yil { get; set; }
    }

    public class MisyonIstegi
    {
        public string KurumKod { get; set; }
        /// <summary>Plan yılı (planın sürümü; çoğunlukla dönemin ilk yılı).</summary>
        public int Yil { get; set; }
        public int BasYil { get; set; }
        public int SonYil { get; set; }
        public string Misyon { get; set; }
        public string Aciklama { get; set; }
    }

    public class DonemKopyalaIstegi
    {
        public int KaynakYil { get; set; }
        public int HedefYil { get; set; }
    }

    public class VizyonIstegi
    {
        public string Vizyon { get; set; }
        public string Aciklama { get; set; }
    }

    public class AmacIstegi
    {
        /// <summary>Amaç no; yeni amaçta 0.</summary>
        public int Kod { get; set; }
        /// <summary>Amaç kodu (A1 gibi kısa ad).</summary>
        public string Kisa { get; set; }
        public string Ad { get; set; }
        public string Aciklama { get; set; }
    }

    public class UstBelgeIstegi
    {
        /// <summary>"kod-ad" biçiminde üst politika belgesi.</summary>
        public string KodAd { get; set; }
        public string Referans { get; set; }
        public string Aciklama { get; set; }
    }

    public class HedefBirimIstegi
    {
        /// <summary>Birimin kurumsal kodu. Listeden yeni seçilen birimde boştur; kod "kod / ad" biçimindeki addan alınır.</summary>
        public string Kod { get; set; }
        public string Ad { get; set; }
    }

    public class HedefIstegi
    {
        /// <summary>Hedef no; yeni hedefte 0.</summary>
        public int Kod { get; set; }
        public int AmacKod { get; set; }
        public string Kisa { get; set; }
        public string Ad { get; set; }
        public string Aciklama { get; set; }
        /// <summary>Sorumlu (ilk sıradaki) ve işbirliği yapılacak birimler.</summary>
        public HedefBirimIstegi[] Birimler { get; set; }
        public string[] Riskler { get; set; }
        public string[] Stratejiler { get; set; }
        public string[] Tespitler { get; set; }
        public string[] Ihtiyaclar { get; set; }
        public string ProgramAd { get; set; }
        public string AltProgramHedef { get; set; }
        public UstBelgeIstegi[] UstBelgeler { get; set; }
    }

    public class HedefGostergeSatiri
    {
        public int GostergeKod { get; set; }
        public string GostergeAd { get; set; }
        public string GorKod { get; set; }
        public double HedefeEtkisi { get; set; }
        public double Gerceklesen { get; set; }
        public double Tahmin0 { get; set; }
        public double Tahmin1 { get; set; }
        public double Tahmin2 { get; set; }
        public double Tahmin3 { get; set; }
        public double Tahmin4 { get; set; }
        /// <summary>İzleme ve raporlama sıklığı: 1 aylık, 2 üç aylık, 3 altı aylık, 4 yıllık.</summary>
        public int IzlemeDonemi { get; set; }
        public int RaporlamaDonemi { get; set; }
    }

    public class HedefGostergeIstegi
    {
        public int Hedef { get; set; }
        public HedefGostergeSatiri[] Satirlar { get; set; }
    }

    public class HedefOdenekSatiri
    {
        public string EkoKod { get; set; }
        public string EkoAd { get; set; }
        public double D0 { get; set; }
        public double D1 { get; set; }
        public double D2 { get; set; }
        public double D3 { get; set; }
        public double D4 { get; set; }
    }

    public class HedefOdenekIstegi
    {
        public int Hedef { get; set; }
        public HedefOdenekSatiri[] Satirlar { get; set; }
    }

    /// <summary>
    /// Stratejik Plan sayfasının (eski PerformansButce/StratejikPlanAna.aspx ve açtığı DonemSec, Misyon, Vizyon, StratejikAmac,
    /// StratejikHedef sayfaları) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/strplan/durum, /donem-tanim, /donemler?kurum=, /misyon?kurum=&amp;yil=, /vizyon, /amac?kod=,
    ///        /hedef-tanim, /hedef?kod=, /gostergeler?hedef=, /odenekler?hedef=,
    ///        /rapor/hedef-karti?kod= ve /rapor/hedef-kartlari (Word), /rapor/hedef-birim ve /rapor/maliyet (Excel)
    ///   POST /api/strplan/donem-sec, /misyon-kaydet, /misyon-kopyala, /vizyon-kaydet, /amac-kaydet, /amac-sil,
    ///        /hedef-kaydet, /hedef-sil, /gosterge-kaydet, /odenek-kaydet
    /// Üzerinde çalışılan kurum ve plan dönemi eskisi gibi kullanıcının "StrPlan" değişkeninde ("kurumKod-yıl") saklanır;
    /// Dönem Seç ve Misyon kaydı bunu değiştirir, diğer bütün işlemler bu döneme yapılır.
    /// Sayfayı menüde Stratejik Plan seçeneğini görebilenler (sistem yöneticisi, stratejik plan yetkilisi) kullanır.
    /// </summary>
    public static class StratejikPlanUclari
    {
        private const string DonemDegiskeni = "StrPlan";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/strplan/durum", Durum);
            app.MapGet("/api/strplan/donem-tanim", DonemTanim);
            app.MapGet("/api/strplan/donemler", Donemler);
            app.MapGet("/api/strplan/misyon", MisyonOku);
            app.MapGet("/api/strplan/vizyon", VizyonOku);
            app.MapGet("/api/strplan/amac", AmacOku);
            app.MapGet("/api/strplan/hedef-tanim", HedefTanim);
            app.MapGet("/api/strplan/hedef", HedefOku);
            app.MapGet("/api/strplan/gostergeler", Gostergeler);
            app.MapGet("/api/strplan/odenekler", Odenekler);
            app.MapGet("/api/strplan/rapor/hedef-karti", HedefKarti);
            app.MapGet("/api/strplan/rapor/hedef-kartlari", HedefKartlari);
            app.MapGet("/api/strplan/rapor/hedef-birim", HedefBirimTablosu);
            app.MapGet("/api/strplan/rapor/maliyet", MaliyetTablosu);
            app.MapPost("/api/strplan/donem-sec", DonemSec);
            app.MapPost("/api/strplan/misyon-kaydet", MisyonKaydet);
            app.MapPost("/api/strplan/misyon-kopyala", MisyonKopyala);
            app.MapPost("/api/strplan/vizyon-kaydet", VizyonKaydet);
            app.MapPost("/api/strplan/amac-kaydet", AmacKaydet);
            app.MapPost("/api/strplan/amac-sil", AmacSil);
            app.MapPost("/api/strplan/hedef-kaydet", HedefKaydet);
            app.MapPost("/api/strplan/hedef-sil", HedefSil);
            app.MapPost("/api/strplan/gosterge-kaydet", GostergeKaydet);
            app.MapPost("/api/strplan/odenek-kaydet", OdenekKaydet);
        }

        // ------------------------------------------------------------------ yetki ve dönem

        /// <summary>Kullanıcı Stratejik Plan sayfasını kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), "../PerformansButce/StratejikPlanAna.aspx");
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

        /// <summary>Üzerinde çalışılan kurum ve plan dönemi.</summary>
        private class Donem
        {
            public string KurumKod = "";
            public int Yil;
            public int BasYil;
            public int SonYil;
            public string PlanAd = "";
            public bool MisyonVar;
            public string Misyon = "";
            public bool Secili { get { return Yil > 0 && !string.IsNullOrWhiteSpace(KurumKod); } }
        }

        /// <summary>Kullanıcının seçtiği dönem (eski StratejikPlanAna.KurumDonemBul): "StrPlan" değişkeni + o dönemin misyonu.</summary>
        private static Donem DonemBul(Kullanici kullanan)
        {
            Donem d = new Donem();
            string kayitli = "";
            try
            {
                TNSCollection degerler = TNS.KYM.Arac.Tanimla().DegiskenleriListele(kullanan.kullaniciKodu, DonemDegiskeni);
                if (degerler != null && degerler.Count > 0) kayitli = Convert.ToString(degerler[0]) ?? "";
            }
            catch (Exception)
            {
                kayitli = "";
            }

            string p1 = "", p2 = "";
            OrtakFonksiyonlar.StringiIkiyeBol(kayitli, "-", ref p1, ref p2);
            d.KurumKod = p1 ?? "";
            d.Yil = OrtakFonksiyonlar.ConvertToInt(p2, -1);
            if (!d.Secili)
            {
                d.Yil = 0;
                return d;
            }
            d.BasYil = d.Yil;
            d.SonYil = d.Yil + 4;

            Misyon kriter = new Misyon();
            kriter.donemBYil = d.Yil;
            kriter.kurumKod = d.KurumKod;
            ObjectArray bilgiler = TNS.PEB.Arac.Tanimla().MisyonListele(kullanan, kriter);
            if (bilgiler.sonuc.islemSonuc && bilgiler.objeler.Count > 0)
            {
                Misyon m = (Misyon)bilgiler.objeler[0];
                d.BasYil = m.aralikBasiYil;
                d.SonYil = m.aralikSonuYil;
                d.PlanAd = m.Ad;
                d.MisyonVar = true;
                d.Misyon = m.misyon ?? "";
            }
            return d;
        }

        private static void DonemYaz(Kullanici kullanan, string kurumKod, int yil)
        {
            TNSCollection degerler = new TNSCollection();
            degerler.Add(kurumKod + "-" + yil);
            kullanan.DegiskenleriSakla(TNS.KYM.Arac.Tanimla(), DonemDegiskeni, degerler);
        }

        private static int HazirlikYili()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBulCache(0, "/BUTCEHAZIRLIK"), DateTime.Now.Year + 1);
        }

        private static IResult DonemYok()
        {
            return Hata("Önce 'Dönem Seç' ile çalışacağınız kurumu ve stratejik planı seçiniz.");
        }

        // ------------------------------------------------------------------ ana sayfa

        /// <summary>Ana sayfanın bütün içeriği: seçili dönem, misyon, vizyon, amaçlar ve hedefleri.</summary>
        private static IResult Durum(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            Donem d = DonemBul(kullanan);
            if (!d.Secili) return Results.Json(new { secili = false });

            IPEBServis servis = TNS.PEB.Arac.Tanimla();
            string kurumAd = "";
            try
            {
                kurumAd = TNS.UZY.Arac.Tanimla().UzayDegeriStr(kullanan, "ABSKUR", d.Yil + "-" + d.KurumKod, true, null) ?? "";
            }
            catch (Exception)
            {
                kurumAd = d.KurumKod;
            }

            string vizyon = "";
            bool vizyonVar = false;
            List<object> amaclar = new List<object>();
            if (d.MisyonVar)
            {
                Vizyon vk = new Vizyon();
                vk.donemBYil = d.Yil;
                vk.dil.kod = 1;
                vk.kurumKod = d.KurumKod;
                ObjectArray vizyonlar = servis.VizyonListele(kullanan, vk);
                if (vizyonlar.sonuc.islemSonuc && vizyonlar.objeler.Count > 0)
                {
                    vizyon = ((Vizyon)vizyonlar.objeler[0]).vizyon ?? "";
                    vizyonVar = true;
                }

                // Hedefler amaçlarına göre gruplanır; hedefi olmayan amaçlar da listelenir.
                Dictionary<int, List<object>> hedefler = new Dictionary<int, List<object>>();
                TNS.PEB.StratejikHedef hk = new TNS.PEB.StratejikHedef();
                hk.donemBYil = d.Yil;
                hk.dil.kod = 1;
                hk.kurumKod = d.KurumKod;
                ObjectArray hedefListe = servis.StratejikHedefListele(kullanan, hk, false);
                if (hedefListe.sonuc.islemSonuc)
                    foreach (TNS.PEB.StratejikHedef h in hedefListe.objeler)
                    {
                        if (h.strHedefKod <= 0 || h.strAmac == null) continue;
                        if (!hedefler.TryGetValue(h.strAmac.strAmacKod, out List<object> liste))
                            hedefler[h.strAmac.strAmacKod] = liste = new List<object>();
                        liste.Add(new { kod = h.strHedefKod, kisa = h.strHedefKisa, ad = h.strHedef });
                    }

                TNS.PEB.StratejikAmac ak = new TNS.PEB.StratejikAmac();
                ak.donemBYil = d.Yil;
                ak.dil.kod = 1;
                ak.kurumKod = d.KurumKod;
                ObjectArray amacListe = servis.StratejikAmacListele(kullanan, ak);
                if (amacListe.sonuc.islemSonuc)
                    foreach (TNS.PEB.StratejikAmac a in amacListe.objeler)
                    {
                        hedefler.TryGetValue(a.strAmacKod, out List<object> liste);
                        amaclar.Add(new { kod = a.strAmacKod, kisa = a.strAmacKisa, ad = a.strAmac, hedefler = liste ?? new List<object>() });
                    }
            }

            return Results.Json(new
            {
                secili = true,
                kurumKod = d.KurumKod,
                yil = d.Yil,
                baslik = (kurumAd + " " + (d.MisyonVar ? d.PlanAd : "")).Trim(),
                misyonVar = d.MisyonVar,
                misyon = d.Misyon,
                vizyonVar = vizyonVar,
                vizyon = vizyon,
                amaclar = amaclar
            });
        }

        // ------------------------------------------------------------------ dönem seçimi

        private static List<object> KurumListesi(Kullanici kullanan, int yil)
        {
            List<object> kurumlar = new List<object>();
            ABSKodlarVAN liste = TNS.BKD.Arac.Tanimla().KurumListesiKullanici(kullanan, yil);
            if (liste != null && liste.absKodlar != null)
                foreach (ABSKod k in liste.absKodlar)
                    kurumlar.Add(new { kod = k.kod, ad = k.kodAd });
            return kurumlar;
        }

        /// <summary>Dönem Seç ve Misyon pencerelerinin kurum listesi ile şimdiki seçim.</summary>
        private static IResult DonemTanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            Donem d = DonemBul(kullanan);
            int yil = d.Yil > 2000 ? d.Yil : HazirlikYili();
            return Results.Json(new { kurumlar = KurumListesi(kullanan, yil), kurumKod = d.KurumKod, yil = d.Secili ? d.Yil : 0, hazirlikYili = HazirlikYili() });
        }

        /// <summary>Bir kurumun kayıtlı stratejik planları (misyonu girilmiş dönemler).</summary>
        private static IResult Donemler(HttpContext baglam, IWebHostEnvironment ortam, string kurum)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            List<object> satirlar = new List<object>();
            if (!string.IsNullOrWhiteSpace(kurum))
            {
                Misyon kriter = new Misyon();
                kriter.dil = new TNS.PEB.Dil();
                kriter.dil.kod = 1;
                kriter.kurumKod = kurum.Trim();
                ObjectArray yillar = TNS.PEB.Arac.Tanimla().MisyonListele(kullanan, kriter);
                if (yillar.sonuc.islemSonuc)
                    foreach (Misyon m in yillar.objeler)
                        satirlar.Add(new { yil = m.donemBYil, ad = m.Ad });
            }
            return Results.Json(new { satirlar = satirlar });
        }

        private static async Task<IResult> DonemSec(HttpContext baglam, IWebHostEnvironment ortam, DonemIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || string.IsNullOrWhiteSpace(istek.KurumKod) || istek.Yil <= 0) return Hata("Kurum ve dönem seçiniz.");

            DonemYaz(kullanan, istek.KurumKod.Trim(), istek.Yil);
            return Results.Json(new { basarili = true });
        }

        // ------------------------------------------------------------------ misyon

        /// <summary>Kurumun verilen plan yılındaki misyonu; kurum ve yıl verilmezse seçili dönemin misyonu.</summary>
        private static IResult MisyonOku(HttpContext baglam, IWebHostEnvironment ortam, string kurum, int? yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            Donem d = DonemBul(kullanan);
            string kurumKod = string.IsNullOrWhiteSpace(kurum) ? d.KurumKod : kurum.Trim();
            int planYil = yil.HasValue && yil.Value > 0 ? yil.Value : (d.Yil > 0 ? d.Yil : HazirlikYili());

            Misyon kriter = new Misyon();
            kriter.donemBYil = planYil;
            kriter.dil.kod = 1;
            kriter.kurumKod = kurumKod;
            Misyon m = null;
            if (!string.IsNullOrWhiteSpace(kurumKod))
            {
                ObjectArray bilgiler = TNS.PEB.Arac.Tanimla().MisyonListele(kullanan, kriter);
                if (bilgiler.sonuc.islemSonuc && bilgiler.objeler.Count > 0) m = (Misyon)bilgiler.objeler[0];
            }

            return Results.Json(new
            {
                bulundu = m != null,
                kurumKod = kurumKod,
                yil = m != null ? m.donemBYil : planYil,
                basYil = m != null ? m.aralikBasiYil : planYil,
                sonYil = m != null ? m.aralikSonuYil : planYil + 4,
                misyon = m != null ? m.misyon : "",
                aciklama = m != null ? m.aciklama : ""
            });
        }

        private static async Task<IResult> MisyonKaydet(HttpContext baglam, IWebHostEnvironment ortam, MisyonIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            if (string.IsNullOrWhiteSpace(istek.KurumKod)) return Hata("Kurum seçiniz.");
            if (istek.Yil <= 0) return Hata("Plan yılı boş bırakılamaz.");
            if (istek.BasYil <= 0 || istek.SonYil < istek.BasYil) return Hata("Dönemin başlangıç ve bitiş yıllarını kontrol ediniz.");
            if (string.IsNullOrWhiteSpace(istek.Misyon)) return Hata("Misyonu yazınız.");

            Misyon misyon = new Misyon();
            misyon.donemBYil = istek.Yil;
            misyon.aciklama = (istek.Aciklama ?? "").Trim();
            misyon.dil.kod = 1;
            misyon.kurumKod = istek.KurumKod.Trim();
            misyon.misyon = istek.Misyon.Trim();
            misyon.aralikBasiYil = istek.BasYil;
            misyon.aralikSonuYil = istek.SonYil;

            Sonuc sonuc = TNS.PEB.Arac.Tanimla().MisyonKaydet(kullanan, misyon);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Misyon kaydedilemedi."));
            // Misyonu kaydedilen plan, üzerinde çalışılan dönem olur.
            DonemYaz(kullanan, misyon.kurumKod, misyon.donemBYil);
            return Results.Json(new { basarili = true, bilgi = "Misyon kaydedildi." });
        }

        /// <summary>Bir plan yılındaki bütün stratejik plan bilgilerini başka bir yıla kopyalar (eski Misyon sayfasındaki Kopyala).</summary>
        private static async Task<IResult> MisyonKopyala(HttpContext baglam, IWebHostEnvironment ortam, DonemKopyalaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.KaynakYil <= 0 || istek.HedefYil <= 0 || istek.KaynakYil == istek.HedefYil)
                return Hata("Kaynak ve hedef yılı yazınız.");

            Sonuc sonuc = TNS.PEB.Arac.Tanimla().MisyonDonemKopyala(kullanan, istek.KaynakYil, istek.HedefYil);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kopyalama yapılamadı."));
            return Results.Json(new { basarili = true, bilgi = ("Kopyalama tamamlandı. " + (sonuc.bilgiStr ?? "")).Trim() });
        }

        // ------------------------------------------------------------------ vizyon

        private static IResult VizyonOku(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            Donem d = DonemBul(kullanan);
            if (!d.Secili) return Results.Json(new { hata = "Önce dönem seçiniz." });

            Vizyon kriter = new Vizyon();
            kriter.donemBYil = d.Yil;
            kriter.dil.kod = 1;
            kriter.kurumKod = d.KurumKod;
            ObjectArray bilgiler = TNS.PEB.Arac.Tanimla().VizyonListele(kullanan, kriter);
            Vizyon v = bilgiler.sonuc.islemSonuc && bilgiler.objeler.Count > 0 ? (Vizyon)bilgiler.objeler[0] : null;
            return Results.Json(new { vizyon = v != null ? v.vizyon : "", aciklama = v != null ? v.aciklama : "" });
        }

        private static async Task<IResult> VizyonKaydet(HttpContext baglam, IWebHostEnvironment ortam, VizyonIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            Donem d = DonemBul(kullanan);
            if (!d.Secili) return DonemYok();
            if (string.IsNullOrWhiteSpace(istek.Vizyon)) return Hata("Vizyonu yazınız.");

            Vizyon vizyon = new Vizyon();
            vizyon.aciklama = (istek.Aciklama ?? "").Trim();
            vizyon.dil.kod = 1;
            vizyon.donemBYil = d.Yil;
            vizyon.kurumKod = d.KurumKod;
            vizyon.vizyon = istek.Vizyon.Trim();

            Sonuc sonuc = TNS.PEB.Arac.Tanimla().VizyonKaydet(kullanan, vizyon);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Vizyon kaydedilemedi."));
            return Results.Json(new { basarili = true, bilgi = "Vizyon kaydedildi." });
        }

        // ------------------------------------------------------------------ amaç

        private static IResult AmacOku(HttpContext baglam, IWebHostEnvironment ortam, int kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            Donem d = DonemBul(kullanan);
            if (!d.Secili || kod <= 0) return Results.Json(new { hata = "Amaç okunamadı." });

            TNS.PEB.StratejikAmac kriter = new TNS.PEB.StratejikAmac();
            kriter.donemBYil = d.Yil;
            kriter.dil.kod = 1;
            kriter.strAmacKod = kod;
            kriter.kurumKod = d.KurumKod;
            ObjectArray bilgiler = TNS.PEB.Arac.Tanimla().StratejikAmacListele(kullanan, kriter);
            if (!bilgiler.sonuc.islemSonuc || bilgiler.objeler.Count == 0) return Results.Json(new { hata = "Amaç okunamadı." });

            TNS.PEB.StratejikAmac a = (TNS.PEB.StratejikAmac)bilgiler.objeler[0];
            return Results.Json(new { kod = a.strAmacKod, kisa = a.strAmacKisa, ad = a.strAmac, aciklama = a.aciklama });
        }

        private static async Task<IResult> AmacKaydet(HttpContext baglam, IWebHostEnvironment ortam, AmacIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            Donem d = DonemBul(kullanan);
            if (!d.Secili) return DonemYok();
            if (string.IsNullOrWhiteSpace(istek.Ad)) return Hata("Amacı yazınız.");

            TNS.PEB.StratejikAmac amac = new TNS.PEB.StratejikAmac();
            amac.aciklama = (istek.Aciklama ?? "").Trim();
            amac.dil.kod = 1;
            amac.donemBYil = d.Yil;
            amac.kurumKod = d.KurumKod;
            amac.tema.temaKod = 1;
            amac.strAmacKisa = (istek.Kisa ?? "").Trim();
            amac.strAmac = istek.Ad.Trim();
            amac.strAmacKod = Math.Max(0, istek.Kod);

            Sonuc sonuc = TNS.PEB.Arac.Tanimla().StratejikAmacKaydet(kullanan, amac);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Amaç kaydedilemedi."));
            return Results.Json(new { basarili = true, bilgi = (amac.strAmacKod == 0 ? "Yeni amaç" : "Amaç") + " kaydedildi.", kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, amac.strAmacKod) });
        }

        private static async Task<IResult> AmacSil(HttpContext baglam, IWebHostEnvironment ortam, AmacIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Kod <= 0) return Results.BadRequest();
            Donem d = DonemBul(kullanan);
            if (!d.Secili) return DonemYok();

            TNS.PEB.StratejikAmac amac = new TNS.PEB.StratejikAmac();
            amac.donemBYil = d.Yil;
            amac.kurumKod = d.KurumKod;
            amac.strAmacKod = istek.Kod;
            Sonuc sonuc = TNS.PEB.Arac.Tanimla().StratejikAmacSil(kullanan, new TNSCollection(amac));
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Amaç silinemedi."));
            return Results.Json(new { basarili = true, bilgi = "Amaç silindi." });
        }

        // ------------------------------------------------------------------ hedef

        private static List<object> DonemSecenekleri()
        {
            List<object> liste = new List<object>();
            foreach (ENUMDonemler donem in new[] { ENUMDonemler.Ay, ENUMDonemler.UcAy, ENUMDonemler.AltiAy, ENUMDonemler.Yil })
                liste.Add(new { kod = (int)donem, ad = TNS.PEB.Arac.DonemStr(donem, false) });
            return liste;
        }

        /// <summary>Hedef penceresinin seçim listeleri: amaçlar, birimler, üst politika belgeleri, göstergeler, sıklıklar.</summary>
        private static IResult HedefTanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            Donem d = DonemBul(kullanan);
            if (!d.Secili) return Results.Json(new { hata = "Önce dönem seçiniz." });

            IPEBServis servis = TNS.PEB.Arac.Tanimla();
            List<string> uyarilar = new List<string>();

            List<object> amaclar = new List<object>();
            TNS.PEB.StratejikAmac ak = new TNS.PEB.StratejikAmac();
            ak.donemBYil = d.Yil;
            ak.kurumKod = d.KurumKod;
            ObjectArray amacListe = servis.StratejikAmacListele(kullanan, ak);
            if (amacListe.sonuc.islemSonuc)
                foreach (TNS.PEB.StratejikAmac a in amacListe.objeler)
                    amaclar.Add(new { kod = a.strAmacKod, ad = (a.strAmacKisa + " " + a.strAmac).Trim() });

            List<string> birimler = new List<string>();
            try
            {
                ObjectArray birimListe = servis.BirimListele(kullanan, d.Yil, d.KurumKod);
                if (birimListe.sonuc.islemSonuc)
                    foreach (ABSKod k in birimListe.objeler)
                        birimler.Add(k.kod + " / " + k.Ad());
            }
            catch (Exception)
            {
                uyarilar.Add("Birimler okunamadı.");
            }

            List<string> ustBelgeler = new List<string>();
            try
            {
                ObjectArray belgeler = servis.UstPolitikaBelgeleriListele(kullanan, d.Yil);
                if (belgeler.sonuc.islemSonuc)
                    foreach (object o in belgeler.objeler)
                    {
                        StrUstPolitikaBelgesi b = o as StrUstPolitikaBelgesi;
                        if (b != null) ustBelgeler.Add(b.kodAd);
                    }
            }
            catch (Exception)
            {
                uyarilar.Add("Üst politika belgeleri okunamadı.");
            }

            List<object> gostergeler = new List<object>();
            try
            {
                PerformansGosterge gk = new PerformansGosterge();
                gk.adaGoreSirala = true;
                gk.kurumKod = d.KurumKod;
                ObjectArray gostergeListe = servis.PerformansGostergeListele(kullanan, gk);
                if (gostergeListe.sonuc.islemSonuc)
                    foreach (PerformansGosterge g in gostergeListe.objeler)
                        gostergeler.Add(new { kod = g.gostergeKod, ad = g.ad });
            }
            catch (Exception)
            {
                uyarilar.Add("Göstergeler okunamadı.");
            }

            return Results.Json(new
            {
                basYil = d.BasYil,
                amaclar = amaclar,
                birimler = birimler,
                ustBelgeler = ustBelgeler,
                gostergeler = gostergeler,
                donemler = DonemSecenekleri(),
                uyari = uyarilar.Count > 0 ? string.Join(" ", uyarilar) : null
            });
        }

        private static List<string> Metinler(TNSCollection liste)
        {
            List<string> sonuc = new List<string>();
            if (liste != null)
                foreach (object o in liste)
                    sonuc.Add(Convert.ToString(o) ?? "");
            return sonuc;
        }

        private static IResult HedefOku(HttpContext baglam, IWebHostEnvironment ortam, int kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            Donem d = DonemBul(kullanan);
            if (!d.Secili || kod <= 0) return Results.Json(new { hata = "Hedef okunamadı." });

            TNS.PEB.StratejikHedef kriter = new TNS.PEB.StratejikHedef();
            kriter.donemBYil = d.Yil;
            kriter.strHedefKod = kod;
            kriter.dil.kod = 1;
            kriter.kurumKod = d.KurumKod;
            ObjectArray bilgiler = TNS.PEB.Arac.Tanimla().StratejikHedefListele(kullanan, kriter, true);
            if (!bilgiler.sonuc.islemSonuc || bilgiler.objeler.Count == 0) return Results.Json(new { hata = "Hedef okunamadı." });
            TNS.PEB.StratejikHedef h = (TNS.PEB.StratejikHedef)bilgiler.objeler[0];

            List<object> birimler = new List<object>();
            if (h.birimler != null)
                foreach (object o in h.birimler)
                {
                    StrKodStrAd b = o as StrKodStrAd;
                    if (b != null) birimler.Add(new { kod = b.kod ?? "", ad = b.ad ?? "" });
                }

            List<object> ustBelgeler = new List<object>();
            if (h.ustPolitikaBaglari != null)
                foreach (object o in h.ustPolitikaBaglari)
                {
                    StrUstPolitikaBelgesi b = o as StrUstPolitikaBelgesi;
                    if (b != null) ustBelgeler.Add(new { kodAd = b.kodAd, referans = b.referans ?? "", aciklama = b.aciklama ?? "" });
                }

            List<string> programBagi = Metinler(h.programBagi);
            return Results.Json(new
            {
                kod = h.strHedefKod,
                amacKod = h.strAmac != null ? h.strAmac.strAmacKod : 0,
                kisa = h.strHedefKisa,
                ad = h.strHedef,
                aciklama = h.aciklama,
                birimler = birimler,
                riskler = Metinler(h.riskler),
                stratejiler = Metinler(h.stratejiler),
                tespitler = Metinler(h.tespitler),
                ihtiyaclar = Metinler(h.ihtiyaclar),
                programAd = programBagi.Count > 0 ? programBagi[0] : "",
                altProgramHedef = programBagi.Count > 1 ? programBagi[1] : "",
                ustBelgeler = ustBelgeler
            });
        }

        private static void MetinEkle(TNSCollection hedef, string[] kaynak)
        {
            foreach (string s in kaynak ?? new string[0])
                if (!string.IsNullOrWhiteSpace(s)) hedef.Add(s.Trim());
        }

        private static async Task<IResult> HedefKaydet(HttpContext baglam, IWebHostEnvironment ortam, HedefIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            Donem d = DonemBul(kullanan);
            if (!d.Secili) return DonemYok();
            if (istek.AmacKod <= 0) return Hata("Hedefin amacını seçiniz.");
            if (string.IsNullOrWhiteSpace(istek.Ad)) return Hata("Hedefi yazınız.");

            TNS.PEB.StratejikHedef hedef = new TNS.PEB.StratejikHedef();
            hedef.aciklama = (istek.Aciklama ?? "").Trim();
            hedef.dil.kod = 1;
            hedef.donemBYil = d.Yil;
            hedef.kurumKod = d.KurumKod;
            hedef.strAmac.strAmacKod = istek.AmacKod;
            hedef.strHedefKisa = (istek.Kisa ?? "").Trim();
            hedef.strHedef = istek.Ad.Trim();
            hedef.strHedefKod = Math.Max(0, istek.Kod);

            // Listeden yeni seçilen birim "kod / ad" biçiminde gelir (kod baştaki parçadır); kayıtlı birim kendi koduyla gelir.
            foreach (HedefBirimIstegi b in istek.Birimler ?? new HedefBirimIstegi[0])
            {
                if (b == null || string.IsNullOrWhiteSpace(b.Ad)) continue;
                StrKodStrAd birim = new StrKodStrAd((b.Kod ?? "").Trim(), b.Ad.Trim());
                if (birim.ad.Contains("/")) birim.kod = birim.ad.Split('/')[0].Trim();
                hedef.birimler.Add(birim);
            }
            MetinEkle(hedef.riskler, istek.Riskler);
            MetinEkle(hedef.stratejiler, istek.Stratejiler);
            MetinEkle(hedef.tespitler, istek.Tespitler);
            MetinEkle(hedef.ihtiyaclar, istek.Ihtiyaclar);
            hedef.programBagi.Add((istek.ProgramAd ?? "").Trim());
            hedef.programBagi.Add((istek.AltProgramHedef ?? "").Trim());
            foreach (UstBelgeIstegi u in istek.UstBelgeler ?? new UstBelgeIstegi[0])
            {
                if (u == null || string.IsNullOrWhiteSpace(u.KodAd)) continue;
                StrUstPolitikaBelgesi belge = new StrUstPolitikaBelgesi();
                belge.kodAd = u.KodAd.Trim();
                belge.referans = (u.Referans ?? "").Trim();
                belge.aciklama = (u.Aciklama ?? "").Trim();
                hedef.ustPolitikaBaglari.Add(belge);
            }

            Sonuc sonuc = TNS.PEB.Arac.Tanimla().StratejikHedefKaydet(kullanan, hedef);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Hedef kaydedilemedi."));
            return Results.Json(new { basarili = true, bilgi = (hedef.strHedefKod == 0 ? "Yeni hedef" : "Hedef") + " kaydedildi.", kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, hedef.strHedefKod) });
        }

        private static async Task<IResult> HedefSil(HttpContext baglam, IWebHostEnvironment ortam, HedefIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Kod <= 0) return Results.BadRequest();
            Donem d = DonemBul(kullanan);
            if (!d.Secili) return DonemYok();

            TNS.PEB.StratejikHedef hedef = new TNS.PEB.StratejikHedef();
            hedef.donemBYil = d.Yil;
            hedef.kurumKod = d.KurumKod;
            hedef.strHedefKod = istek.Kod;
            Sonuc sonuc = TNS.PEB.Arac.Tanimla().StratejikHedefSil(kullanan, new TNSCollection(hedef));
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Hedef silinemedi."));
            return Results.Json(new { basarili = true, bilgi = "Hedef silindi." });
        }

        // ------------------------------------------------------------------ hedefin göstergeleri ve ödenekleri

        private static IResult Gostergeler(HttpContext baglam, IWebHostEnvironment ortam, int hedef)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            Donem d = DonemBul(kullanan);
            if (!d.Secili || hedef <= 0) return Results.Json(new { hata = "Göstergeler okunamadı." });

            ObjectArray liste = TNS.PEB.Arac.Tanimla().ListeleStrHedefGosterge(kullanan, d.Yil, hedef, d.KurumKod);
            if (!liste.sonuc.islemSonuc) return Results.Json(new { hata = "Göstergeler okunamadı." });

            List<object> satirlar = new List<object>();
            foreach (StrHedefGosterge g in liste.objeler)
                satirlar.Add(new
                {
                    gostergeKod = g.gostergeKod,
                    gostergeAd = g.gostergeAd,
                    gorKod = g.gorKod,
                    hedefeEtkisi = g.hedefeEtkisi,
                    gerceklesen = g.gerceklesen,
                    tahmin0 = g.tahmin0,
                    tahmin1 = g.tahmin1,
                    tahmin2 = g.tahmin2,
                    tahmin3 = g.tahmin3,
                    tahmin4 = g.tahmin4,
                    izlemeDonemi = (int)g.izlemeDonemi,
                    raporlamaDonemi = (int)g.raporlamaDonemi,
                    ondalik = g.gostergeOndalik
                });
            return Results.Json(new { basYil = d.BasYil, satirlar = satirlar });
        }

        private static ENUMDonemler DonemCevir(int kod)
        {
            // 0: sıklık girilmemiş (kayıtlı göstergelerde böyle olabilir; olduğu gibi korunur).
            return kod >= 0 && kod <= (int)ENUMDonemler.Yil ? (ENUMDonemler)kod : ENUMDonemler.Ay;
        }

        private static async Task<IResult> GostergeKaydet(HttpContext baglam, IWebHostEnvironment ortam, HedefGostergeIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Hedef <= 0) return Results.BadRequest();
            Donem d = DonemBul(kullanan);
            if (!d.Secili) return DonemYok();

            TNSCollection gostergeler = new TNSCollection();
            foreach (HedefGostergeSatiri s in istek.Satirlar ?? new HedefGostergeSatiri[0])
            {
                if (s == null || s.GostergeKod <= 0) continue;
                StrHedefGosterge g = new StrHedefGosterge();
                g.gostergeKod = s.GostergeKod;
                g.gostergeAd = s.GostergeAd ?? "";
                g.gorKod = (s.GorKod ?? "").Trim();
                g.hedefeEtkisi = s.HedefeEtkisi;
                g.gerceklesen = s.Gerceklesen;
                g.tahmin0 = s.Tahmin0;
                g.tahmin1 = s.Tahmin1;
                g.tahmin2 = s.Tahmin2;
                g.tahmin3 = s.Tahmin3;
                g.tahmin4 = s.Tahmin4;
                g.izlemeDonemi = DonemCevir(s.IzlemeDonemi);
                g.raporlamaDonemi = DonemCevir(s.RaporlamaDonemi);
                gostergeler.Add(g);
            }

            Sonuc sonuc = TNS.PEB.Arac.Tanimla().KaydetStrHedefGosterge(kullanan, d.Yil, istek.Hedef, d.KurumKod, gostergeler);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Göstergeler kaydedilemedi."));
            return Results.Json(new { basarili = true, bilgi = "Göstergeler kaydedildi." });
        }

        private static IResult Odenekler(HttpContext baglam, IWebHostEnvironment ortam, int hedef)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            Donem d = DonemBul(kullanan);
            if (!d.Secili || hedef <= 0) return Results.Json(new { hata = "Ödenekler okunamadı." });

            ObjectArray liste = TNS.PEB.Arac.Tanimla().ListeleStrHedefOdenek(kullanan, d.Yil, hedef, d.KurumKod);
            if (!liste.sonuc.islemSonuc) return Results.Json(new { hata = "Ödenekler okunamadı." });

            List<object> satirlar = new List<object>();
            foreach (StrHedefOdenek o in liste.objeler)
                satirlar.Add(new { ekoKod = o.ekoKod, ekoAd = o.ekoAd, d0 = o.d0, d1 = o.d1, d2 = o.d2, d3 = o.d3, d4 = o.d4 });
            return Results.Json(new { basYil = d.BasYil, satirlar = satirlar });
        }

        private static async Task<IResult> OdenekKaydet(HttpContext baglam, IWebHostEnvironment ortam, HedefOdenekIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Hedef <= 0) return Results.BadRequest();
            Donem d = DonemBul(kullanan);
            if (!d.Secili) return DonemYok();

            TNSCollection odenekler = new TNSCollection();
            foreach (HedefOdenekSatiri s in istek.Satirlar ?? new HedefOdenekSatiri[0])
            {
                if (s == null || string.IsNullOrWhiteSpace(s.EkoKod)) continue;
                StrHedefOdenek o = new StrHedefOdenek(s.EkoKod.Trim(), s.EkoAd ?? "");
                o.d0 = s.D0;
                o.d1 = s.D1;
                o.d2 = s.D2;
                o.d3 = s.D3;
                o.d4 = s.D4;
                odenekler.Add(o);
            }

            Sonuc sonuc = TNS.PEB.Arac.Tanimla().KaydetStrHedefOdenek(kullanan, d.Yil, istek.Hedef, d.KurumKod, odenekler);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Ödenekler kaydedilemedi."));
            return Results.Json(new { basarili = true, bilgi = "Ödenekler kaydedildi." });
        }

        // ------------------------------------------------------------------ raporlar
        // Eski sayfalardaki kodun aynısıdır (StratejikPlanAna.btnRaporAl_Click / HedefleriYazdir, StratejikHedefYeni.HedefYazdir):
        // hedef kartı Aspose.Words ile RaporSablon\PEB\StrHedefKarti.dotx şablonundan, tablolar OrtakClass.IkiBoyutluTablo
        // (Aspose.Cells) ile üretilir. Tek fark dosyanın tarayıcıya gönderilme biçimidir (Results.File).

        private static string StrYap(double p, int ondalik = 0)
        {
            string fEk = "";
            if (ondalik > 0)
                fEk = "." + "0".PadRight(ondalik, '0');
            return p.ToString("#,##0" + fEk);
        }

        private static string StringYap(TNSCollection metinler)
        {
            string donDeger = "";
            foreach (string item in metinler)
            {
                if (donDeger != "") donDeger += "\r\n";
                donDeger += item;
            }
            return donDeger;
        }

        private static string MetinBul(TNSCollection liste, int ind)
        {
            if (ind < liste.Count)
                return (string)liste[ind];
            return "";
        }

        private static string BirimBul(TNSCollection birimler, int basla, int say)
        {
            string birimAd = "";
            for (int i = basla; i < basla + say && i < birimler.Count; i++)
            {
                StrKodStrAd birim = (StrKodStrAd)birimler[i];
                if (birim != null)
                {
                    if (birimAd != "") birimAd += ", ";
                    birimAd += birim.ad;
                }
            }
            return birimAd;
        }

        private static double ToplamAl(ObjectArray odenekler)
        {
            double toplam = 0;
            foreach (StrHedefOdenek item in odenekler.objeler)
                toplam += (item.d0 + item.d1 + item.d2 + item.d3 + item.d4);
            return toplam;
        }

        private static string RaporSablonYol()
        {
            return Ayar.Oku("RaporSablonYol");
        }

        /// <summary>Bir hedefin kartını şablondan üretir ve geçici dosyanın yolunu döndürür (eski StratejikHedefYeni.HedefYazdir).</summary>
        private static string HedefYazdir(IPEBServis servisPEB, Kullanici kullanan, int yil, int hedefKod, int dilKod, string kurumKod, string raporSablonYol)
        {
            TNS.PEB.StratejikHedef hedef = new TNS.PEB.StratejikHedef();
            hedef.donemBYil = yil;
            hedef.strHedefKod = hedefKod;
            hedef.dil.kod = dilKod;
            hedef.kurumKod = kurumKod;
            if (hedef.strHedefKod < 1)
                throw new Exception("Lütfen hedef seçiniz!");

            ObjectArray bilgiler = servisPEB.StratejikHedefListele(kullanan, hedef, true);
            if (!bilgiler.sonuc.islemSonuc || bilgiler.objeler.Count == 0)
                throw new Exception("Lütfen hedef seçiniz!");

            TNS.PEB.StratejikHedef item = (TNS.PEB.StratejikHedef)bilgiler.objeler[0];
            ObjectArray gostergeler = servisPEB.ListeleStrHedefGosterge(kullanan, hedef.donemBYil, hedef.strHedefKod, hedef.kurumKod);
            ObjectArray odenekler = servisPEB.ListeleStrHedefOdenek(kullanan, hedef.donemBYil, hedef.strHedefKod, hedef.kurumKod);

            AsposeLisans.Yukle();
            Aspose.Words.Document doc = new Aspose.Words.Document(Path.Combine(raporSablonYol, "PEB", "StrHedefKarti.dotx"));
            Aspose.Words.DocumentBuilder db = new Aspose.Words.DocumentBuilder(doc);

            GenelIslemlerIstemci.AsposeAlanaYaz(db, "aK", item.strAmac.strAmacKisa);
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "amac", item.strAmac.strAmac);
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "hK", item.strHedefKisa);
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "hedef", item.strHedef);
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "ilgPrg", MetinBul(item.programBagi, 0));
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "iliskiliAprgHedef", MetinBul(item.programBagi, 1));
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "sbirim", BirimBul(item.birimler, 0, 1));
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "ibirim", BirimBul(item.birimler, 1, item.birimler.Count - 1));
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "yil_1", (item.aralikBasYil - 1).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "yil", (item.aralikBasYil).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "yil1", (item.aralikBasYil + 1).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "yil2", (item.aralikBasYil + 2).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "yil3", (item.aralikBasYil + 3).ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "yil4", (item.aralikBasYil + 4).ToString());

            DataTable dt = new DataTable();
            dt.TableName = "veri";
            dt.Columns.Add("pg");
            dt.Columns.Add("hetki");
            dt.Columns.Add("deg_1");
            dt.Columns.Add("deg0");
            dt.Columns.Add("deg1");
            dt.Columns.Add("deg2");
            dt.Columns.Add("deg3");
            dt.Columns.Add("deg4");
            dt.Columns.Add("izs");
            dt.Columns.Add("raps");
            foreach (StrHedefGosterge gos in gostergeler.objeler)
            {
                int ondalik = gos.ondalikSayisi;
                dt.Rows.Add(gos.gorKod + " " + gos.gostergeAd, "%" + StrYap(gos.hedefeEtkisi, 0), StrYap(gos.gerceklesen, ondalik), StrYap(gos.tahmin0, ondalik), StrYap(gos.tahmin1, ondalik), StrYap(gos.tahmin2, ondalik), StrYap(gos.tahmin3, ondalik), StrYap(gos.tahmin4, ondalik), TNS.PEB.Arac.DonemStr(gos.izlemeDonemi, false), TNS.PEB.Arac.DonemStr(gos.raporlamaDonemi, false));
            }
            if (dt.Rows.Count == 0)
                dt.Rows.Add("", "", "", "", "", "", "", "", "", "");
            doc.MailMerge.ExecuteWithRegions(dt);

            GenelIslemlerIstemci.AsposeAlanaYaz(db, "riskler", StringYap(item.riskler));
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "stratejiler", StringYap(item.stratejiler));
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "tespitler", StringYap(item.tespitler));
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "ihtiyaclar", StringYap(item.ihtiyaclar));
            GenelIslemlerIstemci.AsposeAlanaYaz(db, "maliyet", StrYap(ToplamAl(odenekler)) + " TL");

            string sonucDosyaYol = DosyaIslem.DosyaAdUret() + ".docx";
            doc.Save(sonucDosyaYol);
            return sonucDosyaYol;
        }

        /// <summary>Geçici dosyayı okuyup siler ve indirilecek dosya olarak döndürür (eski DosyaIslem.DosyaGonderX).</summary>
        private static IResult DosyaGonder(string dosyaYol, string gidenAd, string icerikTuru)
        {
            byte[] icerik = File.ReadAllBytes(dosyaYol);
            try { File.Delete(dosyaYol); } catch { }
            return Results.File(icerik, icerikTuru, gidenAd);
        }

        private const string DocxTuru = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        private const string XlsxTuru = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        /// <summary>Hedef penceresindeki Yazdır: açık hedefin kartı (eski StratejikHedef.btnYazdir_Click).</summary>
        private static IResult HedefKarti(HttpContext baglam, IWebHostEnvironment ortam, int kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            Donem d = DonemBul(kullanan);
            if (!d.Secili) return Results.Json(new { hata = "Önce dönem seçiniz." });

            try
            {
                string sonucDosyaYol = HedefYazdir(TNS.PEB.Arac.Tanimla(), kullanan, d.Yil, kod, 1, d.KurumKod, RaporSablonYol());
                return DosyaGonder(sonucDosyaYol, "StrHedefKarti_" + DosyaIslem.DosyaAdUretSade() + ".docx", DocxTuru);
            }
            catch (Exception ex)
            {
                return Results.Json(new { hata = HataMetniDuz(ex, "Hedef kartı hazırlanamadı.") });
            }
        }

        /// <summary>Raporlar > Hedef kartları: dönemin bütün hedeflerinin kartları tek belgede (eski StratejikPlanAna.HedefleriYazdir).</summary>
        private static IResult HedefKartlari(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            Donem d = DonemBul(kullanan);
            if (!d.Secili) return Results.Json(new { hata = "Önce dönem seçiniz." });

            IPEBServis servisPEB = TNS.PEB.Arac.Tanimla();
            TNS.PEB.StratejikHedef hedef = new TNS.PEB.StratejikHedef();
            hedef.donemBYil = d.Yil;
            hedef.dil.kod = 1;
            hedef.kurumKod = d.KurumKod;

            ObjectArray bilgiler = servisPEB.StratejikHedefListele(kullanan, hedef, true);
            if (!bilgiler.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(bilgiler.sonuc, "Hedefler okunamadı.") });
            if (bilgiler.ObjeSayisi == 0) return Results.Json(new { hata = "Hedef bulunamadı!" });

            try
            {
                int ilk = 1;
                Aspose.Words.Document docIc = null;
                Aspose.Words.DocumentBuilder builder = null;
                foreach (TNS.PEB.StratejikHedef okunanHedef in bilgiler.objeler)
                {
                    string dosyaAd = HedefYazdir(servisPEB, kullanan, d.Yil, okunanHedef.strHedefKod, 1, d.KurumKod, RaporSablonYol());
                    if (ilk == 1)
                    {
                        docIc = new Aspose.Words.Document(dosyaAd);
                        builder = new Aspose.Words.DocumentBuilder(docIc);
                    }
                    else
                    {
                        builder.MoveToDocumentEnd();
                        builder.InsertBreak(Aspose.Words.BreakType.PageBreak);
                        GenelIslemlerIstemci.AsposeBuilderaDocEkle(builder, dosyaAd);
                    }
                    try
                    {
                        File.Delete(dosyaAd);
                    }
                    catch { }

                    ilk++;
                }
                if (docIc == null) return Results.Json(new { hata = "Hedef bulunamadı!" });

                string enIc = DosyaIslem.DosyaAdUret("HedefKartlari", "") + ".docx";
                docIc.Save(enIc);
                return DosyaGonder(enIc, "HedefKartlari_" + DosyaIslem.DosyaAdUretSade() + ".docx", DocxTuru);
            }
            catch (Exception ex)
            {
                return Results.Json(new { hata = HataMetniDuz(ex, "Hedef kartları hazırlanamadı.") });
            }
        }

        /// <summary>Raporlar > Hedef - Birim Tablosu (raporNo 1) ve Tahmini Maliyet Tablosu (raporNo 2): eski btnRaporAl_Click.</summary>
        private static IResult TabloRaporu(HttpContext baglam, IWebHostEnvironment ortam, int raporNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            Donem d = DonemBul(kullanan);
            if (!d.Secili) return Results.Json(new { hata = "Önce dönem seçiniz." });

            TNS.PEB.StratejikHedef hedef = new TNS.PEB.StratejikHedef();
            hedef.donemBYil = d.Yil;
            hedef.kurumKod = d.KurumKod;
            ObjectArray bilgiler = TNS.PEB.Arac.Tanimla().StratejikHedefRaporAl(kullanan, hedef, raporNo);
            if (!bilgiler.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(bilgiler.sonuc, "Rapor verisi okunamadı.") });

            try
            {
                IkiBoyutluTablo ib = new IkiBoyutluTablo();
                if (raporNo == 2)
                {
                    ib.ustBaslik1 = "Tahmini Maliyet Tablosu";
                    ib.ustBaslik2 = "";
                    ib.sutunlarBaslik = "Plan Yılları";
                    ib.kosegenYazi = "Hedefler";
                    ib.hucreFormat = "#,###";
                    ib.sutunBaslikRenk = System.Drawing.Color.FromArgb(247, 150, 70);
                    ib.sonSutunSigdir = false;
                    ib.soldaAlanSayisi = 2;
                    ib.ustteAlanSayisi = 1;
                }
                else
                {
                    ib.ustBaslik1 = "Hedeflerden Sorumlu ve İşbirliği Yapılacak Birimler Tablosu";
                    ib.ustBaslik2 = "";
                    ib.sutunlarBaslik = "Harcama Birimleri";
                    ib.kosegenYazi = "Hedefler";
                    ib.hucreFormat = "";
                    ib.sutunBaslikRenk = System.Drawing.Color.FromArgb(247, 150, 70);
                    ib.veriYanasik = 2;
                    ib.toplamGizle = true;
                    ib.sonSutunSigdir = false;
                    ib.dipNot = "S: Sorumlu birim, İ: İşbirliği yapılacak birim";
                    ib.soldaAlanSayisi = 1;
                    ib.ustteAlanSayisi = 1;
                }
                ib.veriler = bilgiler.objeler;

                AsposeLisans.Yukle();
                string uz = "xlsx";
                string sonucDosyaAd = DosyaIslem.DosyaAdUret("HB", "") + "." + uz;
                string tut = IkiBoyutluTablo.TabloUret(ib, uz, sonucDosyaAd);
                return DosyaGonder(tut, (raporNo == 2 ? "TahminiMaliyetTablosu_" : "HedefBirimTablosu_") + DosyaIslem.DosyaAdUretSade() + "." + uz, XlsxTuru);
            }
            catch (Exception ex)
            {
                return Results.Json(new { hata = HataMetniDuz(ex, "Rapor hazırlanamadı.") });
            }
        }

        private static IResult HedefBirimTablosu(HttpContext baglam, IWebHostEnvironment ortam)
        {
            return TabloRaporu(baglam, ortam, 1);
        }

        private static IResult MaliyetTablosu(HttpContext baglam, IWebHostEnvironment ortam)
        {
            return TabloRaporu(baglam, ortam, 2);
        }

        /// <summary>Kullanıcıya gösterilecek hata: kendi attığımız kısa ileti gösterilir, iç ayrıntı (yığın izi vb.) gösterilmez.</summary>
        private static string HataMetniDuz(Exception ex, string varsayilan)
        {
            return ex.Message != null && ex.Message.StartsWith("Lütfen", StringComparison.Ordinal) ? ex.Message : varsayilan;
        }

        // ------------------------------------------------------------------ yardımcılar

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static string HataMetni(Sonuc sonuc, string varsayilan)
        {
            string hata = sonuc.hataStr ?? "";
            return hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at ") ? varsayilan : hata;
        }
    }
}
