using System;
using System.Collections.Generic;
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
using TNS.BKD;
using TNS.KYM;
using TNS.ORG;

namespace Istemci.Kullanicilar
{
    /// <summary>Kaydet / Dosya Yükle isteği: formdaki alanlar (eski EkrandanAl).</summary>
    public class YerleskeOdaIstegi
    {
        public int kod { get; set; }
        public int ustKod { get; set; }
        public string ustGorKod { get; set; }
        public string ustTarif { get; set; }
        public string gorKod { get; set; }
        public string ad { get; set; }
        public int saymanlikKod { get; set; }
        public string birimKod { get; set; }
        public int yerTurKod { get; set; }
        public int odaTurKod { get; set; }
        public string ilKod { get; set; }
        public int orgKod { get; set; }
        public int durum { get; set; }
        public int maliyetMerkezi { get; set; }
        public int siraNo { get; set; }
    }

    /// <summary>Taşı isteği (eski ağaçta sürükle-bırak, NodeTasimaGerceklestir).</summary>
    public class YerleskeOdaTasiIstegi
    {
        public int kod { get; set; }
        public int ustKod { get; set; }
        public string birimKod { get; set; }
    }

    /// <summary>
    /// Yerleşke Oda Tanımı (eski KYM/TanimOda.aspx; yeni sayfa KYM/TanimOda) uçları:
    ///   GET  /api/yerleskeoda/tanim                 Merkez Bankası mı, iller (eski IlYukle)
    ///   GET  /api/yerleskeoda/agac?ust=&amp;pasif=  yerleşke / oda ağacı (eski NodeLoad)
    ///   GET  /api/yerleskeoda/oku?kod=              (eski Goster)
    ///   GET  /api/yerleskeoda/yeni?ust=&amp;birimKod= seçili yerin altına yeni kayıt (eski Yeni)
    ///   GET  /api/yerleskeoda/birimler?ust=         odanın birimi için organizasyon ağacı (eski ddlBirim / ORGBirim NodeLoad)
    ///   GET  /api/yerleskeoda/rapor?birimKod=       Yazdır, Excel (eski btnExcelYaz_Click; şablon KYM\YerleskeListe(MB).xlt)
    ///   POST /api/yerleskeoda/kaydet                (eski btnKaydet_Click)
    ///   POST /api/yerleskeoda/tasi                  başka yerin altına taşı (eski NodeTasimaGerceklestir)
    ///   POST /api/yerleskeoda/yukle (multipart: form alanları + dosya)  seçili yerin altına Excel'den kayıt (eski fuDosya_FileSelected)
    /// İş kuralları eski sayfadaki gibi IKYMServis.YerleskeOdaListele / Oku / Kaydet / UstKodGuncelle; yazma yetkisini iş kuralları
    /// denetler (sistem yöneticisi ya da taşınır mal kaydı). Eski sayfadaki Pasif Yap tuşu gizliydi (Hidden="true"), taşınmadı;
    /// pasif yapmak için Durum alanı kullanılır. Oda Türü listesi eski sayfada doldurulmuyordu (boştu), taşınmadı.
    /// </summary>
    public static class YerleskeOdaUclari
    {
        public const string SayfaAdresi = "../KYM/TanimOda.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/yerleskeoda/tanim", Tanim);
            app.MapGet("/api/yerleskeoda/agac", Agac);
            app.MapGet("/api/yerleskeoda/oku", Oku);
            app.MapGet("/api/yerleskeoda/yeni", Yeni);
            app.MapGet("/api/yerleskeoda/birimler", Birimler);
            app.MapGet("/api/yerleskeoda/rapor", Rapor);
            app.MapPost("/api/yerleskeoda/kaydet", Kaydet);
            app.MapPost("/api/yerleskeoda/tasi", Tasi);
            app.MapPost("/api/yerleskeoda/yukle", Yukle);
        }

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IKYMServis Servis()
        {
            return TNS.KYM.Arac.Tanimla();
        }

        private static bool MerkezBankasi()
        {
            return TNS.TMM.Arac.MerkezBankasiKullaniyor();
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = hata ?? "";
            return hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at ") ? varsayilan : hata;
        }

        // ------------------------------------------------------------------ okuma

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return Results.Json(new { merkezBankasi = MerkezBankasi(), iller = IlYukle(kullanan) });
        }

        /// <summary>Eski GenelIslemlerIstemci.IlYukle: kod türü 20, seviye 3; kod noktasız.</summary>
        private static List<object> IlYukle(Kullanici kullanan)
        {
            List<object> liste = new List<object>();
            try
            {
                ABSKodlarVAN v = TNS.BKD.Arac.Tanimla().KodListeleSeviye(0, "20", "", 3, true);
                if (v == null || v.sonuc == null || !v.sonuc.islemSonuc || v.absKodlar == null) return liste;
                string kultur = string.IsNullOrEmpty(kullanan.aktifKultur) ? "tr" : kullanan.aktifKultur;
                v.absKodlar.Sort();
                foreach (ABSKod k in v.absKodlar)
                    liste.Add(new { kod = (k.kod ?? "").Replace(".", "").Trim(), ad = (k.Ad(kultur) ?? "").Trim() });
            }
            catch (Exception)
            {
            }
            return liste;
        }

        /// <summary>Eski NodeLoad (sayfanın ağacı): ust -1 kök. MB'de pasifler yalnızca "Pasifleri de Listele" ile gelir.</summary>
        private static IResult Agac(HttpContext baglam, IWebHostEnvironment ortam, int? ust, string pasif)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            bool mb = MerkezBankasi();
            YerleskeOda kriter = new YerleskeOda();
            kriter.ustKod = ust ?? -1;
            if (mb) kriter.durum = pasif == "1" ? 100 : 1;

            try
            {
                ObjectArray<YerleskeOda> oa = Servis().YerleskeOdaListele(kullanan, kriter);
                if (oa == null || !oa.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(oa?.sonuc?.hataStr, "Yerler okunamadı.") });
                List<object> satirlar = new List<object>();
                foreach (YerleskeOda d in oa.objeler)
                {
                    string ad = d.ad ?? "";
                    if (d.yerTurKod == 6 && mb) ad = d.gorKodKisa ?? "";
                    satirlar.Add(new { kod = d.kod, ad = d.durum == 3 ? ad + " PASİF" : ad, altVar = d.altDalVar, birimKod = d.birimKod ?? "", durum = d.durum });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Yerler okunamadı." });
            }
        }

        /// <summary>Eski EkranaYaz: forma gidecek alanlar.</summary>
        private static object Kayit(YerleskeOda oda)
        {
            return new
            {
                kod = oda.kod,
                ustKod = oda.ustKod,
                ustGorKod = oda.ustGorKod ?? "",
                ustTarif = oda.ustTarif ?? "",
                ustBilgi = oda.UstBilgiOl() ?? "",
                gorKod = oda.gorKodKisa ?? "",
                ad = oda.ad ?? "",
                tarif = oda.tarif ?? "",
                yerTurKod = oda.yerTurKod,
                odaTurKod = oda.odaTurKod,
                saymanlikKod = oda.saymanlikKod,
                birimKod = oda.birimKod ?? "",
                orgKod = oda.orgKod,
                orgAd = oda.orgAd ?? "",
                ilKod = oda.ilKod ?? "",
                // Yeni kayıtta durum Aktif gelir.
                durum = oda.kod == 0 && oda.durum == 0 ? 1 : oda.durum,
                maliyetMerkezi = oda.maliyetMerkezi,
                siraNo = oda.siraNo
            };
        }

        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                YerleskeOda oda = Servis().YerleskeOdaOku(kullanan, kod ?? 0);
                if (oda == null) return Results.Json(new { hata = "Kayıt bulunamadı." });
                return Results.Json(new { kayit = Kayit(oda) });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Kayıt okunamadı." });
            }
        }

        /// <summary>Eski Yeni(ustKod, birimKod): üst yerin bilgileriyle boş kayıt.</summary>
        private static IResult Yeni(HttpContext baglam, IWebHostEnvironment ortam, int? ust, string birimKod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                YerleskeOda oda = new YerleskeOda();
                string uyari = null;
                YerleskeOda ustOda = Servis().YerleskeOdaOku(kullanan, ust ?? -1);
                if (ustOda == null)
                {
                    uyari = "Bulunduğu yer bulunamadı!";
                    oda.ustKod = 0;
                }
                else
                {
                    oda.kod = 0;
                    oda.ustKod = ustOda.kod;
                    oda.ustAd = ustOda.ad;
                    oda.ustGorKod = ustOda.gorTamKod;
                    oda.ustTarif = ustOda.tarif;
                    oda.saymanlikKod = 200;
                    oda.birimKod = MerkezBankasi() ? (birimKod ?? "") : "965";
                }
                return Results.Json(new { kayit = Kayit(oda), uyari = uyari });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Kayıt hazırlanamadı." });
            }
        }

        /// <summary>Odanın birimi: eski ddlBirim ağacı (ORGBirim.aspx NodeLoad, özellik aramasız); cls = muhasebe birimi.</summary>
        private static IResult Birimler(HttpContext baglam, IWebHostEnvironment ortam, int? ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                OrganizasyonBirim kriter = new OrganizasyonBirim();
                kriter.anaBirimKod = Math.Max(0, ust ?? 0);
                if (kriter.anaBirimKod == 0) kriter.kopruKod = "999";
                kriter.sablonMu = false;
                ObjectArray<OrganizasyonBirim> oa = TNS.ORG.Arac.Tanimla().OrganizasyonBirimListele(kriter, false, false);
                if (oa == null || !oa.sonuc.islemSonuc) return Results.Json(new { hata = "Birimler okunamadı." });
                List<object> satirlar = new List<object>();
                foreach (OrganizasyonBirim b in oa.objeler)
                    satirlar.Add(new { kod = b.kod, ad = b.adi ?? "", altVar = b.altDalVar, muhasebeBirim = b.muhasebebirim ?? "" });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Birimler okunamadı." });
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>Eski EkrandanAl.</summary>
        private static YerleskeOda EkrandanAl(YerleskeOdaIstegi i)
        {
            bool mb = MerkezBankasi();
            YerleskeOda oda = new YerleskeOda();
            oda.kod = i.kod;
            oda.ustKod = i.ustKod;
            if (!mb) oda.ustGorKod = i.ustGorKod ?? "";
            oda.gorKodKisa = i.gorKod ?? "";
            oda.ad = i.ad ?? "";
            oda.ustTarif = i.ustTarif ?? "";
            oda.tarif = oda.ustTarif + (string.IsNullOrWhiteSpace(oda.ustTarif) ? "" : ", ") + oda.ad;
            oda.saymanlikKod = i.saymanlikKod;
            oda.birimKod = i.birimKod ?? "";
            oda.yerTurKod = i.yerTurKod;
            oda.odaTurKod = i.odaTurKod;
            oda.ilKod = i.ilKod ?? "";
            oda.orgKod = i.orgKod;
            oda.durum = i.durum;
            oda.maliyetMerkezi = i.maliyetMerkezi;
            oda.siraNo = i.siraNo;
            return oda;
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YerleskeOdaIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            try
            {
                YerleskeOda y = EkrandanAl(istek);
                Sonuc s = Servis().YerleskeOdaKaydet(kullanan, y);
                if (!s.islemSonuc) return Hata("Kayıt işlemi başarısız: " + HataMetni(s.hataStr, "beklenmeyen bir hata oluştu."));
                return Results.Json(new { basarili = true, kod = OrtakFonksiyonlar.ConvertToInt(s.anahtar, 0), bilgi = "Kayıt işlemi başarılı" });
            }
            catch (Exception)
            {
                return Hata("Kayıt işlemi başarısız.");
            }
        }

        private static async Task<IResult> Tasi(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YerleskeOdaTasiIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.kod <= 0) return Hata("Taşınacak yeri seçiniz.");
            if (istek.kod == istek.ustKod) return Hata("Bir yer kendi altına taşınamaz.");
            try
            {
                YerleskeOda yd = new YerleskeOda();
                yd.kod = istek.kod;
                yd.ustKod = istek.ustKod;
                yd.birimKod = istek.birimKod ?? "";
                Sonuc s = Servis().YerleskeOdaUstKodGuncelle(kullanan, yd);
                if (!s.islemSonuc) return Hata(HataMetni(s.hataStr, "Taşıma yapılamadı."));
                return Results.Json(new { basarili = true, bilgi = "Taşıma işlemi başarılı" });
            }
            catch (Exception)
            {
                return Hata("Taşıma yapılamadı.");
            }
        }

        /// <summary>
        /// Eski fuDosya_FileSelected: Excel'in 1. sütunu oda/yer kodu, 2. sütunu adı; boş kod satırında durur. Kayıtlar formdaki yerin
        /// altına, bir alt yer türüyle eklenir (yerleşke ise il 0103001, oda ise birim 1).
        /// </summary>
        private static async Task<IResult> Yukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            IFormCollection form = await baglam.Request.ReadFormAsync();
            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length == 0 || string.IsNullOrWhiteSpace(dosya.FileName)) return Hata("Lütfen Dosya Seçiniz");

            YerleskeOdaIstegi i = new YerleskeOdaIstegi();
            i.kod = OrtakFonksiyonlar.ConvertToInt(form["kod"].ToString(), 0);
            i.ustKod = OrtakFonksiyonlar.ConvertToInt(form["ustKod"].ToString(), 0);
            i.ustGorKod = form["ustGorKod"].ToString();
            i.ustTarif = form["ustTarif"].ToString();
            i.gorKod = form["gorKod"].ToString();
            i.ad = form["ad"].ToString();
            i.yerTurKod = OrtakFonksiyonlar.ConvertToInt(form["yerTurKod"].ToString(), 0);
            YerleskeOda ekrOda = EkrandanAl(i);

            int odaKod = 0;
            string hata = "";
            try
            {
                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                using (MemoryStream ms = new MemoryStream())
                {
                    await dosya.CopyToAsync(ms);
                    ms.Position = 0;
                    XLS.DosyaOkuAc(ms);
                    try
                    {
                        int satir = 0;
                        while (true)
                        {
                            YerleskeOda oda = new YerleskeOda();
                            oda.ustKod = ekrOda.kod;
                            if (oda.ustKod == 0) oda.ustKod = -1;
                            oda.ustGorKod = ekrOda.gorTamKod;
                            oda.kod = 0;
                            oda.gorKodKisa = XLS.HucreDegerAl(satir, 0);
                            if (string.IsNullOrWhiteSpace(oda.gorKodKisa)) break;
                            oda.ad = XLS.HucreDegerAl(satir, 1);
                            oda.tarif = ekrOda.tarif + (string.IsNullOrWhiteSpace(ekrOda.tarif) ? "" : ", ") + oda.ad;
                            oda.saymanlikKod = 200;
                            oda.birimKod = "965";
                            oda.yerTurKod = ekrOda.yerTurKod + 1;
                            if (oda.yerTurKod == 6)
                            {
                                oda.odaTurKod = odaKod;
                                oda.orgKod = 1;
                            }
                            else
                                oda.odaTurKod = 0;
                            if (oda.yerTurKod == 1)
                                oda.ilKod = "0103001";
                            Sonuc s = Servis().YerleskeOdaKaydet(kullanan, oda);
                            if (!s.islemSonuc)
                                hata += satir.ToString() + ". satır " + s.hataStr;
                            satir++;
                        }
                    }
                    finally
                    {
                        XLS.DosyaKapat();
                    }
                }
            }
            catch (Exception)
            {
                return Hata("Dosya okunamadı.");
            }

            if (string.IsNullOrWhiteSpace(hata))
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi tamamlandı." });
            return Results.Json(new { basarili = false, hata = "Kayıt işlemi tamamlandı, listedeki hatalar oluştu: " + hata });
        }

        // ------------------------------------------------------------------ rapor

        /// <summary>Eski btnExcelYaz_Click: tüm yerler (MB'de seçili yerin birimi verilirse yalnızca o birimin yerleri).</summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, string birimKod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                bool mb = MerkezBankasi();
                YerleskeOda form = new YerleskeOda() { kod = -100, durum = 1 };
                if (mb && !string.IsNullOrWhiteSpace(birimKod))
                {
                    form.kod = 0;
                    form.birimKod = birimKod;
                }
                ObjectArray<YerleskeOda> dler = Servis().YerleskeOdaListele(kullanan, form);
                if (dler == null || !dler.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(dler?.sonuc?.hataStr, "Yerler okunamadı.") });

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;
                int sutun = 0;
                int kaynakSatir = 0;
                string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
                string raporSablonYol = Ayar.Oku("RaporSablonYol");
                if (mb)
                {
                    XLS.DosyaAc(Path.Combine(raporSablonYol, "KYM", "YerleskeListeMB.xlt"), sonucDosyaAd);
                    XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
                    satir = kaynakSatir;
                    foreach (YerleskeOda oda in dler.objeler)
                    {
                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);
                        XLS.HucreDegerYaz(satir, sutun, oda.gorKodKisa);
                        XLS.HucreDegerYaz(satir, sutun + 1, oda.kod + " - " + oda.ad);
                        if (oda.ustKod == 0)
                            XLS.KoyuYap(satir, sutun, satir, sutun + 1, true);
                    }
                    XLS.SatirSil(kaynakSatir, kaynakSatir);
                }
                else
                {
                    XLS.DosyaAc(Path.Combine(raporSablonYol, "KYM", "YerleskeListe.xlt"), sonucDosyaAd);
                    XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
                    satir = kaynakSatir;
                    foreach (YerleskeOda oda in dler.objeler)
                    {
                        satir++;
                        XLS.SatirAc(satir, 1);
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);
                        XLS.HucreDegerYaz(satir, sutun, oda.ustGorKod);
                        XLS.HucreDegerYaz(satir, sutun + 1, oda.gorKodKisa);
                        XLS.HucreDegerYaz(satir, sutun + 2, oda.ad);
                    }
                    XLS.SatirSil(kaynakSatir, kaynakSatir);
                }
                XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), "Yerleşke Oda Listesi", true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }
    }
}
