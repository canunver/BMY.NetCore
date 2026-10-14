using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public class DepremSatiri
    {
        public string EkoKod { get; set; }
        public string Ad { get; set; }
        public double ButceIci { get; set; }
        public string ButceIciAciklama { get; set; }
        public double HT1 { get; set; }
        public double HT2 { get; set; }
        public double HT3 { get; set; }
        public double HT4 { get; set; }
        public double HT5 { get; set; }
        public string ButceDisiAciklama { get; set; }
    }

    public class DepremKayitIstegi
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        public int Il { get; set; }
        public List<DepremSatiri> Satirlar { get; set; }
    }

    public class DepremOnayIstegi
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        /// <summary>5: onayla, 1: onay kaldır.</summary>
        public int Durum { get; set; }
    }

    public class DepremSeciliBirim
    {
        public int SaymanlikKod { get; set; }
        public string BirimKod { get; set; }
    }

    public class DepremSeciliOnayIstegi
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        /// <summary>5: onayla, 1: onay kaldır.</summary>
        public int Durum { get; set; }
        public List<DepremSeciliBirim> Birimler { get; set; }
    }

    public class DepremAyIstegi
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        /// <summary>Yalnızca Aktar: harcamanın yapıldığı il (0: bütün iller).</summary>
        public int Il { get; set; }
    }

    /// <summary>
    /// Deprem Harcamaları Bilgi Formu sayfasının (eski ButceForm/DepremHarcamaForm.aspx) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/depremharcama/tanim (muhasebe / birim seçimi ortak bileşenle: /api/muhbirim/..., OrtakSayfa/MuhasebeBirimUclari.cs)
    ///   GET  /api/depremharcama/oku?yil=&amp;ay=&amp;muhasebe=&amp;birim=&amp;il=
    ///   POST /api/depremharcama/kaydet
    ///   GET  /api/depremharcama/rapor?tur=&amp;yil=&amp;ay=&amp;muhasebe=&amp;birim=&amp;il=&amp;kod= (Excel; eski kodla, DepremRapor)
    ///        tur 1: İl-Eko Yazdır, 2: Eko Yazdır, 3: Muh. Birim Yazdır, 4: Liste Yazdır (kod: ekonomik kod başı)
    ///   Yalnızca sistem yöneticisi (eski sayfada bu tuşlar yalnızca ona görünürdü):
    ///   POST /api/depremharcama/onayla (durum 5: Onayla, 1: Onay Kaldır), /aktar (e-bütçeye aktarım)
    ///   GET  /api/depremharcama/durumlar?yil=&amp;ay=;  POST /secili-onayla, /onceki-aydan-kopyala (Onay İşlemleri penceresi)
    /// İşlemler eskisi gibi TNS.BHF.Arac.Tanimla() üzerinden yapılır; muhasebe birimi görme / yazma yetkisi iş kurallarında denetlenir.
    /// </summary>
    public static class DepremHarcamaUclari
    {
        private const string SayfaAdresi = "../ButceForm/DepremHarcamaForm.aspx";
        private const int OnayliDurum = 5;
        private const int IlkYil = 2023;

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/depremharcama/tanim", Tanim);
            app.MapGet("/api/depremharcama/oku", Oku);
            app.MapPost("/api/depremharcama/kaydet", Kaydet);
            app.MapGet("/api/depremharcama/rapor", Rapor);
            app.MapPost("/api/depremharcama/onayla", Onayla);
            app.MapPost("/api/depremharcama/aktar", Aktar);
            app.MapGet("/api/depremharcama/durumlar", Durumlar);
            app.MapPost("/api/depremharcama/secili-onayla", SeciliOnayla);
            app.MapPost("/api/depremharcama/onceki-aydan-kopyala", OncekiAydanKopyala);
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

        /// <summary>Onayla / Onay Kaldır / Onay İşlemleri / Aktar eski sayfada yalnızca sistem yöneticisine görünürdü.</summary>
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
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static DepremHarcamaAna Form(int yil, int ay, int muhasebe, string birim, int il)
        {
            DepremHarcamaAna form = new DepremHarcamaAna();
            form.yil = yil;
            form.ay = ay;
            form.muhasebeKod = muhasebe;
            form.birimKod = (birim ?? "").Trim();
            form.ilKod = il;
            return form;
        }

        /// <summary>Varsayılan kurum kodu (eski GenelIslemlerIstemci.VarsayilanKurumBul: /KURUMKOD).</summary>
        private static string KurumKod()
        {
            return TNS.DEG.Arac.VarsayilanKurumBul() ?? "";
        }

        /// <summary>Eski hdnKurumsalYaz: kurumsal kod, yılın kurumsal kod yapısına göre noktalı.</summary>
        private static string KurumsalYaz(int yil, string kurum)
        {
            if (string.IsNullOrWhiteSpace(kurum)) return "";
            return OrtakFonksiyonlar.KodNoktaKoy(TNS.BKD.TertipYapi.KodYapiBul(yil, 1, false), kurum.Replace(".", ""));
        }

        /// <summary>Eski SaymanlikBirimBul + MuhBirimAdBul: saymanlık / birim adı ve birimin kurumsal kodu.</summary>
        private static string BirimAdBul(int saymanlikNo, string birimKod, out string kurum)
        {
            kurum = "";
            if (saymanlikNo <= 0) return "";
            string sayAd = TNS.UZY.Arac.Tanimla().UzayDegeriStr(new Kullanici(), "MUHMUHASEBE", saymanlikNo.ToString(), true, "MUHASEBEAD");
            string birimAd = "";
            if (string.IsNullOrWhiteSpace(sayAd)) sayAd = "Saymanlık Bulunamadı!";
            else if (!string.IsNullOrWhiteSpace(birimKod))
            {
                TNS.MUH.MuhasebeBirim bir = new TNS.MUH.MuhasebeBirim();
                bir.muhasebe = saymanlikNo;
                bir.birimKod = birimKod.Trim();
                ObjectArray oa = TNS.MUH.Arac.Tanimla().MuhasebeBirimListele(bir);
                if (oa != null && oa.ObjeSayisi >= 1)
                {
                    bir = (TNS.MUH.MuhasebeBirim)oa.objeler[0];
                    birimAd = bir.birimAd;
                    kurum = bir.kurum ?? "";
                }
                if (string.IsNullOrWhiteSpace(birimAd)) birimAd = "Birim Bulunamadı!";
            }
            return TNS.MUH.Arac.BirimAdOl(sayAd, birimAd);
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Açılış (eski Page_Load): yıl / ay, iller, kullanıcının muhasebe birimi (kullanıcı değişkenleri), yönetici tuşları.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int yil = DateTime.Now.Year;
                int ay = DateTime.Now.Month - 1;
                if (ay < 1) ay = 1;

                List<object> aylar = new List<object>();
                for (int i = 1; i <= 12; i++) aylar.Add(new { kod = i, ad = OrtakFonksiyonlar.AyAdiGetir(i) });

                List<object> iller = new List<object>();
                TNS.YAT.BilgiVAN bv = TNS.YAT.Arac.Tanimla().IlListele(kullanan, new TNS.YAT.Sozluk() { kod = 100 });
                if (bv != null && bv.liste != null)
                    foreach (TNS.YAT.Sozluk il in bv.liste)
                        iller.Add(new { kod = il.kod, ad = il.ad });

                string muhasebe = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBE");
                string birim = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEBIRIM");
                string kurum;
                string birimAd = BirimAdBul(OrtakFonksiyonlar.ConvertToInt(muhasebe, 0), birim, out kurum);
                string kurumsal = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEKURUMSAL");
                if (string.IsNullOrWhiteSpace(kurumsal)) kurumsal = kurum;

                return Results.Json(new
                {
                    yil = yil, ilkYil = IlkYil, sonYil = DateTime.Now.Year, ay = ay, aylar = aylar, iller = iller,
                    muhasebe = muhasebe, birim = birim, birimAd = birimAd, kurumsal = KurumsalYaz(yil, kurumsal),
                    sistemYoneticisi = SistemYoneticisi(kullanan)
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>Oku (eski btnOku_Click): ilk satır (genel toplam) "T" kodunu alır; durum 5 ise form onaylıdır.</summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int yil, int ay, int? muhasebe, string birim, int? il)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                DepremHarcamaAna form = Form(yil, ay, muhasebe ?? 0, birim, il ?? 0);
                ObjectVAN ov = TNS.BHF.Arac.Tanimla().OkuDepremHarcamaForm(kullanan, form);
                if (ov == null || !ov.sonuc.islemSonuc) return Hata(HataMetni(ov?.sonuc.hataStr, "Form okunamadı."));
                if (ov.o == null) return Results.Json(new { satirlar = new object[0], durum = 0, bilgi = "Verilen kriterlere uygun kayıt bulunamadı." });

                form = (DepremHarcamaAna)ov.o;
                List<object> satirlar = new List<object>();
                for (int i = 0; i < form.depremHarcamaDetay.Count; i++)
                {
                    DepremHarcamaDetay d = (DepremHarcamaDetay)form.depremHarcamaDetay[i];
                    satirlar.Add(new
                    {
                        ekoKod = i == 0 ? "T" : (d.ekoKod ?? ""), ad = d.ad ?? "",
                        butceIci = d.butceIci, butceIciAciklama = d.butceIciAciklama ?? "",
                        hT1 = d.hT1, hT2 = d.hT2, hT3 = d.hT3, hT4 = d.hT4, hT5 = d.hT5,
                        butceDisiAciklama = d.butceDisiAciklama ?? ""
                    });
                }
                return Results.Json(new { satirlar = satirlar, durum = form.durum });
            }
            catch (Exception)
            {
                return Hata("Form okunamadı.");
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>Kaydet (eski btnKaydet_Click): tablodaki bütün satırlar gönderilir; iş kuralı yalnızca rakamı olan eko 4 satırlarını yazar.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, DepremKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });

            try
            {
                DepremHarcamaAna form = Form(istek.Yil, istek.Ay, istek.Muhasebe, istek.Birim, istek.Il);
                foreach (DepremSatiri s in istek.Satirlar ?? new List<DepremSatiri>())
                {
                    if (s == null || string.IsNullOrEmpty(s.EkoKod)) continue;
                    DepremHarcamaDetay d = new DepremHarcamaDetay();
                    d.ekoKod = s.EkoKod.Trim();
                    d.ad = s.Ad ?? "";
                    d.butceIci = s.ButceIci;
                    d.butceIciAciklama = s.ButceIciAciklama ?? "";
                    d.hT1 = s.HT1;
                    d.hT2 = s.HT2;
                    d.hT3 = s.HT3;
                    d.hT4 = s.HT4;
                    d.hT5 = s.HT5;
                    d.butceDisiAciklama = s.ButceDisiAciklama ?? "";
                    form.depremHarcamaDetay.Add(d);
                }
                Sonuc sonuc = TNS.BHF.Arac.Tanimla().KaydetDepremHarcamaForm(kullanan, form);
                if (sonuc == null || !sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc?.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
        }

        /// <summary>Onayla (durum 5) / Onay Kaldır (durum 1) (eski btnOnayla_Click / btnOnayKaldir_Click).</summary>
        private static async Task<IResult> Onayla(HttpContext baglam, IWebHostEnvironment ortam, DepremOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null) return Results.Json(new { basarili = false, hata = "İşlem yapılamadı." });

            int durum = istek.Durum == OnayliDurum ? OnayliDurum : 1;
            try
            {
                DepremHarcamaAna form = Form(istek.Yil, istek.Ay, istek.Muhasebe, istek.Birim, 0);
                form.durum = durum;
                Sonuc sonuc = TNS.BHF.Arac.Tanimla().OnaylaDepremHarcamaForm(kullanan, form);
                if (sonuc == null || !sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc?.hataStr, "İşlem yapılamadı.") });
                return Results.Json(new { basarili = true, durum = durum, bilgi = durum == OnayliDurum ? "Onaylama işlemi tamamlandı" : "Onay kaldırma işlemi başarıyla tamamlandı" });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "İşlem yapılamadı." });
            }
        }

        /// <summary>Onay İşlemleri penceresinin listesi (eski DurumlariYukle): ayın bütün birimleri, toplam tutarları ve onay durumları.</summary>
        private static IResult Durumlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, int ay)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            try
            {
                DepremHarcamaAna frm = Form(yil, ay, 0, null, 0);
                string[] sql = new string[] { global::ButceForm.DepremRapor.BirimSorgu(frm) };
                string[] baglanti = new string[] { "BaglantiSatiriBHM" };
                SortedList sl = TNS.UZY.Arac.Tanimla().SorguBirlestir(sql, baglanti, new int[] { 0, 1, 2, 3 });
                List<object> liste = new List<object>();
                if (sl != null)
                {
                    foreach (DictionaryEntry de in sl)
                    {
                        TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                        TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                        liste.Add(new
                        {
                            saymanlikKod = alAnahtar.IntAl(0), birimKod = alAnahtar.StrAl(1),
                            birimAd = TNS.MUH.Arac.BirimAdOl(alAnahtar.StrAl(2), alAnahtar.StrAl(3)),
                            tutar = alVeri.DblAl(0) + alVeri.DblAl(1) + alVeri.DblAl(2) + alVeri.DblAl(3) + alVeri.DblAl(4) + alVeri.DblAl(5),
                            durum = alVeri.IntAl(6)
                        });
                    }
                }
                return Results.Json(new { satirlar = liste });
            }
            catch (Exception e)
            {
                return Hata(e.GetType().Name + ": " + e.Message);
            }
        }

        /// <summary>Onay İşlemleri: seçili birimleri onaylar (5) ya da onaylarını kaldırır (1) (eski btnSeciliOnayla_Click).</summary>
        private static async Task<IResult> SeciliOnayla(HttpContext baglam, IWebHostEnvironment ortam, DepremSeciliOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || istek.Birimler == null || istek.Birimler.Count == 0)
                return Results.Json(new { basarili = false, hata = "Seçim yapmanız gerekmektedir!" });

            int durum = istek.Durum == OnayliDurum ? OnayliDurum : 1;
            int hatali = 0;
            int basarili = 0;
            IBHFServis servis = TNS.BHF.Arac.Tanimla();
            foreach (DepremSeciliBirim item in istek.Birimler)
            {
                if (item == null) continue;
                try
                {
                    DepremHarcamaAna form = Form(istek.Yil, istek.Ay, item.SaymanlikKod, item.BirimKod, 0);
                    form.durum = durum;
                    Sonuc sonuc = servis.OnaylaDepremHarcamaForm(kullanan, form);
                    if (sonuc == null || !sonuc.islemSonuc) hatali++;
                    else basarili++;
                }
                catch (Exception)
                {
                    hatali++;
                }
            }
            if (hatali == 0 && basarili == 0)
                return Results.Json(new { basarili = false, hata = "Seçim yapmanız gerekmektedir!" });
            return Results.Json(new { basarili = hatali == 0, bilgi = $"{basarili} adet birim onaylandı ve {hatali} adet birim onaylanamadı!" });
        }

        /// <summary>Onay İşlemleri: Önceki Aydan Kopyala (eski btnOncekiAydanKopyala_Click).</summary>
        private static async Task<IResult> OncekiAydanKopyala(HttpContext baglam, IWebHostEnvironment ortam, DepremAyIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null) return Results.Json(new { basarili = false, hata = "Kopyalama işlemi tamamlanamadı." });
            try
            {
                Sonuc sonuc = TNS.BHF.Arac.Tanimla().OncekiAydanKopyalaDepremHarcamaForm(kullanan, Form(istek.Yil, istek.Ay, 0, null, 0));
                if (sonuc == null || !sonuc.islemSonuc)
                    return Results.Json(new { basarili = false, hata = "Kopyalama işlemi tamamlanamadı:" + HataMetni(sonuc?.hataStr, "") });
                return Results.Json(new { basarili = true, bilgi = "Kopyalama işlemi tamamlandı!" });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kopyalama işlemi tamamlanamadı." });
            }
        }

        /// <summary>Aktar (eski btnAktar_Click): ayın il / eko 4 toplamları SBB e-bütçe "DepremHarcamaKaydet" servisine gönderilir.</summary>
        private static async Task<IResult> Aktar(HttpContext baglam, IWebHostEnvironment ortam, DepremAyIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || istek.Yil < IlkYil || istek.Ay < 1 || istek.Ay > 12) return Results.Json(new { basarili = false, hata = "Yıl / ay hatalı!" });
            try
            {
                string hata = await DepremHarcamaAktarimi.Aktar(kullanan, Form(istek.Yil, istek.Ay, 0, null, istek.Il), Ayar.Oku("RaporSablonYol"));
                if (hata != null) return Results.Json(new { basarili = false, hata = hata });
                return Results.Json(new { basarili = true, bilgi = "Aktarım tamamlandı" });
            }
            catch (Exception e)
            {
                return Results.Json(new { basarili = false, hata = "Aktarım tamamlanamadı: " + e.Message });
            }
        }

        // ------------------------------------------------------------------ raporlar (eski kodla)

        /// <summary>Yazdır tuşları (eski btnYazdir_Click): tur 1 İl-Eko, 2 Eko, 3 Muh. Birim, 4 Liste (kod: ekonomik kod başı).</summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int tur, int yil, int ay, int? muhasebe, string birim, int? il, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000 || ay < 1 || ay > 12) return Hata("Yıl / ay hatalı!");
            try
            {
                AsposeLisans.Yukle();
                global::ButceForm.DepremRapor r = new global::ButceForm.DepremRapor(kullanan, Ayar.Oku("RaporSablonYol"), Form(yil, ay, muhasebe ?? 0, birim, il ?? 0));
                string dosya;
                if (tur == 4)
                    dosya = r.YazdirListe(kod ?? "");
                else if (tur == 3)
                    dosya = r.YazdirBirim();
                else
                    dosya = r.YazdirIlEko(tur == 2 ? 2 : 1);
                return DosyaIslem.DosyaGonder(dosya, Path.GetFileName(dosya), true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }
    }
}
