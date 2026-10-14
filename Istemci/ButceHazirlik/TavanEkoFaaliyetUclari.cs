using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ButceHazirlikv2;
using Istemci.ButceKod;
using Istemci.Organizasyon;
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
    public class FaaliyetTavanSatiri
    {
        public string Kod1 { get; set; }
        public string Kod2 { get; set; }
        public string Kod3 { get; set; }
        public string Kod4 { get; set; }
        public string Kod5 { get; set; }
        public string Kod6 { get; set; }
        public double Yil0 { get; set; }
        public double Yil1 { get; set; }
        public double Yil2 { get; set; }
        public double EkOdenek { get; set; }
        public double EkOdenek1 { get; set; }
        public double EkOdenek2 { get; set; }
    }

    public class FaaliyetTavanKayitIstegi
    {
        public int Yil { get; set; }
        public string Eko { get; set; }
        public string Sektor { get; set; }
        public string Birim { get; set; }
        public List<FaaliyetTavanSatiri> Satirlar { get; set; }
    }

    public class FaaliyetTavanOnayIstegi
    {
        public int Yil { get; set; }
        public string Eko { get; set; }
        public string Sektor { get; set; }
        public string Birim { get; set; }
        /// <summary>onayla (5), kapat (6: faaliyet girişine kapat), kaldir (1), ac (5: faaliyet girişine aç, yalnızca sistem yöneticisi).</summary>
        public string Islem { get; set; }
    }

    public class FaaliyetTopluOnayIstegi
    {
        public int Yil { get; set; }
        /// <summary>"eko-sektör-birim"; boşsa listedeki hepsi (Hepsini Onayla / Hepsini Aç).</summary>
        public string Kod { get; set; }
        public int Durum { get; set; }
    }

    /// <summary>
    /// Koordinasyon Tavan Girişi ve Kontrolü sayfasının (eski ButceHazirlikv2/TavanEkoFaaliyet.aspx) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/tavanfaal/tanim, /birimler?yil=, /ekosektor?yil=&amp;birim=, /liste?yil=&amp;eko=&amp;sektor=&amp;birim=, /onaylar?yil=
    ///   POST /api/tavanfaal/kaydet, /onay, /onay-toplu
    ///   GET  /api/tavanfaal/rapor, /rapor-imzali (Excel; eski kodla, TavanRaporlari / KoordinasyonRaporu)
    /// Birim (PEB) ve onu tavanı girilmiş ekonomik kod + sektör seçilir; o tavanın faaliyet/projelere dağıtımı girilir
    /// (tavan türü tavanTurEkoFaaliyet, senaryo "0000"). Onay durumu: 0/1 açık, 5 onaylı (detay girişine açık), 6 onaylı (detay kapalı).
    /// </summary>
    public static class TavanEkoFaaliyetUclari
    {
        private const string Senaryo = "0000";
        private const string OnaySenaryo = "00.00"; // eski sayfa onay okurken / toplu onayda bunu kullanıyordu (noktalar silinir, aynı kayıt)

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/tavanfaal/tanim", Tanim);
            app.MapGet("/api/tavanfaal/birimler", Birimler);
            app.MapGet("/api/tavanfaal/ekosektor", EkoSektor);
            app.MapGet("/api/tavanfaal/liste", Liste);
            app.MapGet("/api/tavanfaal/onaylar", Onaylar);
            app.MapPost("/api/tavanfaal/kaydet", Kaydet);
            app.MapPost("/api/tavanfaal/onay", Onay);
            app.MapPost("/api/tavanfaal/onay-toplu", TopluOnay);
            app.MapGet("/api/tavanfaal/rapor", Rapor);
            app.MapGet("/api/tavanfaal/rapor-imzali", RaporImzali);
        }

        /// <summary>Kullanıcı bu sayfayı kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), "../ButceHazirlikv2/TavanEkoFaaliyet.aspx");
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

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static bool Bos(params string[] s)
        {
            foreach (string x in s)
                if (string.IsNullOrWhiteSpace(x)) return true;
            return false;
        }

        /// <summary>Faaliyet tavanlarının onay durumu (eski BHMTavanGenelOnayOku; -1: okunamadı, 0: kayıt yok).</summary>
        private static int DurumOku(Kullanici kullanan, int yil, string eko, string sektor, string birim)
        {
            TavanGenel tg = new TavanGenel();
            tg.kod1 = eko;
            tg.kod2 = sektor;
            tg.kod3 = birim;
            tg = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayOku(kullanan, yil, OnaySenaryo, ENUMTavanTur.tavanTurEkoFaaliyet, tg);
            return tg == null ? -1 : tg.durum;
        }

        public static string DurumAd(int durum)
        {
            if (durum == 6)
                return "Onaylı (Detay Kapalı)";
            else if (durum == 5)
                return "Onaylı (Detay Açık)";
            else
                return "Açık (Detay Kapalı)";
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Açılış: bütçe hazırlık yılı ve o yılın birimleri (eski Page_Load / YilDegisti).</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            int yil = KodTanimUclari.YilBul(null);
            try
            {
                return Results.Json(new { yil = yil, sistemYoneticisi = GenelGorevUclari.YetkiVar(kullanan), birimler = BirimleriOku(yil) });
            }
            catch (Exception)
            {
                return Results.Json(new { yil = yil, sistemYoneticisi = GenelGorevUclari.YetkiVar(kullanan), hata = "Birimler okunamadı." });
            }
        }

        /// <summary>PEB birinci düzey birimleri, hiyerarşik ad (eski ABSKodDoldur(yil, "14", "", 1, ...)).</summary>
        private static List<object> BirimleriOku(int yil)
        {
            List<object> birimler = new List<object>();
            ABSKodlarVAN van = TNS.BKD.Arac.Tanimla().KodListeleSeviye(yil, "14", "", 1, false, false, "", "", "", true);
            if (van != null && van.absKodlar != null)
                foreach (ABSKod k in van.absKodlar)
                    birimler.Add(new { kod = k.kod, ad = k.kodAd });
            return birimler;
        }

        private static IResult Birimler(HttpContext baglam, IWebHostEnvironment ortam, int yil)
        {
            IResult ret = Denetle(baglam, ortam, out _);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            try
            {
                return Results.Json(new { birimler = BirimleriOku(yil) });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Birimler okunamadı." });
            }
        }

        /// <summary>Birimin onaylı ekonomik kod + sektör tavanları (eski cbFaal1_Changed).</summary>
        private static IResult EkoSektor(HttpContext baglam, IWebHostEnvironment ortam, int yil, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            if (Bos(birim)) return Results.Json(new { kodlar = new object[0] });
            try
            {
                TavanGenel kistas = new TavanGenel();
                kistas.kod3 = birim.Trim();
                ObjectArray oa = TNS.BHMv2.Arac.Tanimla().ListeleBHMTavanGenel(kullanan, yil, Senaryo, ENUMTavanTur.tavanTurEkoSektorBirim, kistas, true, false);
                if (oa == null) return Results.Json(new { hata = "Ekonomik kod / sektör tavanları okunamadı." });
                if (!oa.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(oa.sonuc.hataStr, "Ekonomik kod / sektör tavanları okunamadı.") });

                List<object> kodlar = new List<object>();
                for (int i = 0; i < oa.ObjeSayisi; i++)
                {
                    TavanGenel t = (TavanGenel)oa.objeler[i];
                    if (Bos(t.kod1)) continue;
                    kodlar.Add(new { eko = t.kod1, sektor = t.kod2, birim = t.kod3, ad = t.ekoSektorAciklama });
                }
                return Results.Json(new { kodlar = kodlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Ekonomik kod / sektör tavanları okunamadı." });
            }
        }

        private static object Satir(TavanGenel t)
        {
            return new
            {
                id = t.kod4 + "@" + t.kod5 + "@" + t.kod6,
                kod1 = t.kod1, kod2 = t.kod2, kod3 = t.kod3, kod4 = t.kod4, kod5 = t.kod5, kod6 = t.kod6, ad4 = t.ad4,
                odenek_2 = t.odenek_2, harcama_2 = t.harcama_2, odenek_1 = t.odenek_1,
                yil0 = t.yil0, yil1 = t.yil1, yil2 = t.yil2,
                teklif0 = t.teklif0, teklif1 = t.teklif1, teklif2 = t.teklif2,
                ekOdenek = t.ekOdenek, ekOdenek1 = t.ekOdenek1, ekOdenek2 = t.ekOdenek2
            };
        }

        /// <summary>
        /// Seçilen tavan ve faaliyet/proje dağıtımı (eski Oku). TAVAN satırı ekonomik kod + sektör + birim tavanıdır;
        /// FARK ve TOPLAM satırlarını sayfa hesaplar.
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string eko, string sektor, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            if (Bos(eko, sektor, birim)) return Results.Json(new { hata = "Lütfen seçim yapınız!" });
            eko = eko.Trim(); sektor = sektor.Trim(); birim = birim.Trim();

            try
            {
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                ObjectArray liste1 = servis.ListeleEkoSektorTavan(kullanan, yil, Senaryo, eko, sektor, birim, "0600");
                if (liste1 == null) return Results.Json(new { hata = "Tavan okunamadı." });
                if (!liste1.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(liste1.sonuc.hataStr, "Tavan okunamadı.") });
                if (liste1.ObjeSayisi != 1) return Results.Json(new { hata = "Seçilen kod için tavan bilgisi bulunamadı!" });

                ObjectArray liste = servis.ListeleEkoSektorFaaliyetTavan(kullanan, yil, Senaryo, eko, sektor, birim, null);
                if (liste == null) return Results.Json(new { hata = "Faaliyet tavanları okunamadı." });
                if (!liste.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(liste.sonuc.hataStr, "Faaliyet tavanları okunamadı.") });

                List<object> satirlar = new List<object>();
                for (int i = 0; i < liste.ObjeSayisi; i++)
                    satirlar.Add(Satir((TavanGenel)liste.objeler[i]));

                return Results.Json(new
                {
                    tavan = Satir((TavanGenel)liste1.objeler[0]),
                    satirlar = satirlar,
                    durum = DurumOku(kullanan, yil, eko, sektor, birim)
                });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Faaliyet tavanları okunamadı." });
            }
        }

        /// <summary>Toplu Onay penceresi: bütün ekonomik kod + sektör + birim tavanları ve faaliyet onay durumları (eski OnayListeBul).</summary>
        private static List<object> OnayListeBul(Kullanici kullanan, int yil)
        {
            TavanGenel kistas = new TavanGenel();
            kistas.kod3 = "";
            ObjectArray oa = TNS.BHMv2.Arac.Tanimla().ListeleBHMTavanGenel(kullanan, yil, Senaryo, ENUMTavanTur.tavanTurEkoSektorBirim, kistas, false);
            if (oa == null || !oa.sonuc.islemSonuc) throw new InvalidOperationException(oa == null ? "" : oa.sonuc.hataStr);

            List<object> l = new List<object>();
            foreach (TavanGenel item in oa.objeler)
            {
                int durum = DurumOku(kullanan, yil, item.kod1, item.kod2, item.kod3);
                l.Add(new { kod = item.kod1 + "-" + item.kod2 + "-" + item.kod3, ad = $"{item.ad1}-{item.ad2} ({item.ad3})", durum = durum, durumAd = DurumAd(durum) });
            }
            return l;
        }

        private static IResult Onaylar(HttpContext baglam, IWebHostEnvironment ortam, int yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!GenelGorevUclari.YetkiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            try
            {
                return Results.Json(new { onaylar = OnayListeBul(kullanan, yil) });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Onay durumları okunamadı." });
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>
        /// Faaliyet/proje tavanlarını kaydeder (eski Kaydet). Onaylanmış tavanlar değiştirilemez (eski sayfa Kaydet'i gizliyordu,
        /// burada sunucu da denetler). Toplamın birim tavanını aşmaması ve birim tavanının onaylı olması iş kurallarındadır.
        /// Ek ödenekler de geri gönderilir: kayıt sorgusu onları da yazar.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, FaaliyetTavanKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000 || Bos(istek.Eko, istek.Sektor, istek.Birim))
                return Results.Json(new { basarili = false, hata = "Lütfen seçim yapınız!" });

            string eko = istek.Eko.Trim(), sektor = istek.Sektor.Trim(), birim = istek.Birim.Trim();
            try
            {
                int durum = DurumOku(kullanan, istek.Yil, eko, sektor, birim);
                if (durum < 0) return Results.Json(new { basarili = false, hata = "Onay durumu okunamadı." });
                if (durum > 1) return Results.Json(new { basarili = false, hata = "Faaliyet tavanları onaylanmış, değiştirilemez." });

                TNSCollection tavanListe = new TNSCollection();
                foreach (FaaliyetTavanSatiri s in istek.Satirlar ?? new List<FaaliyetTavanSatiri>())
                {
                    if (s == null || (s.Kod4 ?? "").Length <= 2) continue; // TAVAN / FARK / TOPLAM satırları
                    // Satırlar listelenen tavana aittir; başka tavana yazılamaz.
                    if (s.Kod1 != eko || s.Kod2 != sektor || s.Kod3 != birim) continue;
                    TavanGenel tg = new TavanGenel();
                    tg.kod1 = s.Kod1;
                    tg.kod2 = s.Kod2;
                    tg.kod3 = s.Kod3;
                    tg.kod4 = s.Kod4;
                    tg.kod5 = s.Kod5;
                    tg.kod6 = s.Kod6;
                    tg.yil0 = s.Yil0;
                    tg.yil1 = s.Yil1;
                    tg.yil2 = s.Yil2;
                    tg.ekOdenek = s.EkOdenek;
                    tg.ekOdenek1 = s.EkOdenek1;
                    tg.ekOdenek2 = s.EkOdenek2;
                    tavanListe.Add(tg);
                }

                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().KaydetBHMTavanGenel(kullanan, istek.Yil, Senaryo, ENUMTavanTur.tavanTurEkoFaaliyet, tavanListe);
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
        }

        /// <summary>
        /// Onayla (5), Faaliyet Girişine Kapat (6), Onay kaldır (1), Faaliyet Girişine Aç (5) (eski btnOnay_Click).
        /// Detay kapalı (6) tavanı yalnızca sistem yöneticisi açabilir (eski sayfada yalnızca ona görünen tuş).
        /// </summary>
        private static async Task<IResult> Onay(HttpContext baglam, IWebHostEnvironment ortam, FaaliyetTavanOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000 || Bos(istek.Eko, istek.Sektor, istek.Birim))
                return Results.Json(new { basarili = false, hata = "Lütfen onaylanacak faaliyeti seçiniz!" });

            int yeniDurum;
            switch (istek.Islem)
            {
                case "onayla": yeniDurum = 5; break;
                case "kapat": yeniDurum = 6; break;
                case "kaldir": yeniDurum = 1; break;
                case "ac": yeniDurum = 5; break;
                default: return Results.Json(new { basarili = false, hata = "İşlem hatalı." });
            }

            try
            {
                TavanGenel tg = new TavanGenel();
                tg.kod1 = istek.Eko.Trim();
                tg.kod2 = istek.Sektor.Trim();
                tg.kod3 = istek.Birim.Trim();

                int durum = DurumOku(kullanan, istek.Yil, tg.kod1, tg.kod2, tg.kod3);
                if (durum < 0) return Results.Json(new { basarili = false, hata = "Onay durumu okunamadı." });
                if ((durum == 6 || istek.Islem == "ac") && !GenelGorevUclari.YetkiVar(kullanan))
                    return Results.StatusCode(StatusCodes.Status403Forbidden);

                tg.durum = yeniDurum;
                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayKaydet(kullanan, istek.Yil, Senaryo, ENUMTavanTur.tavanTurEkoFaaliyet, tg);
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "İşlem başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "İşlem tamamlandı.", durum = yeniDurum });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "İşlem başarısız." });
            }
        }

        /// <summary>
        /// Toplu Onay penceresi (yalnızca sistem yöneticisi): tek satırın kilidi (eski DurumDegistirBirim) ya da Kod boşsa listedekilerin
        /// hepsi (eski btnHepsiniOnayla_Click; ilk hatada durur). Güncel liste döndürülür.
        /// </summary>
        private static async Task<IResult> TopluOnay(HttpContext baglam, IWebHostEnvironment ortam, FaaliyetTopluOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!GenelGorevUclari.YetkiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });
            if (istek.Durum != 1 && istek.Durum != 5 && istek.Durum != 6) return Results.Json(new { basarili = false, hata = "İşlem hatalı." });

            try
            {
                List<string> kodlar = new List<string>();
                if (!Bos(istek.Kod))
                    kodlar.Add(istek.Kod.Trim());
                else
                {
                    TavanGenel kistas = new TavanGenel();
                    kistas.kod3 = "";
                    ObjectArray oa = TNS.BHMv2.Arac.Tanimla().ListeleBHMTavanGenel(kullanan, istek.Yil, Senaryo, ENUMTavanTur.tavanTurEkoSektorBirim, kistas, false);
                    if (oa == null || !oa.sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = "Onay durumları okunamadı." });
                    foreach (TavanGenel item in oa.objeler)
                        kodlar.Add(item.kod1 + "-" + item.kod2 + "-" + item.kod3);
                }

                string hata = null;
                foreach (string kod in kodlar)
                {
                    string[] k = kod.Split('-');
                    TavanGenel tg = new TavanGenel();
                    tg.kod1 = OrtakFonksiyonlar.DiziElemanAl(k, 0, "");
                    tg.kod2 = OrtakFonksiyonlar.DiziElemanAl(k, 1, "");
                    tg.kod3 = OrtakFonksiyonlar.DiziElemanAl(k, 2, "");
                    tg.durum = istek.Durum;
                    Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayKaydet(kullanan, istek.Yil, OnaySenaryo, ENUMTavanTur.tavanTurEkoFaaliyet, tg);
                    if (!sonuc.islemSonuc) { hata = HataMetni(sonuc.hataStr, "İşlem başarısız."); break; }
                }

                List<object> onaylar = OnayListeBul(kullanan, istek.Yil);
                if (hata != null) return Results.Json(new { basarili = false, hata = hata, onaylar = onaylar });
                return Results.Json(new { basarili = true, bilgi = "İşlem tamamlandı.", onaylar = onaylar });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "İşlem başarısız." });
            }
        }

        // ------------------------------------------------------------------ raporlar (eski kodla)

        /// <summary>
        /// Yazdır (eski btnYazdirDetay_Click): seçili ekonomik kod + sektör + birim için faaliyet tavanları; ekonomik kod / sektör
        /// seçilmemişse birimin bütün tavanları (şablon BHMv2\TavanEkoFaaliyetDetay.xltx).
        /// </summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int yil, string birim, string eko, string sektor)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            try
            {
                AsposeLisans.Yukle();
                string dosya = TavanRaporlari.TavanEkoFaaliyetDetayYazdir(TNS.BHMv2.Arac.Tanimla(), kullanan, yil,
                    (eko ?? "").Trim(), (sektor ?? "").Trim(), (birim ?? "").Trim(), Ayar.Oku("RaporSablonYol"));
                return DosyaIslem.DosyaGonder(dosya, "TavanEkoFaaliyetDetay_" + DosyaIslem.DosyaAdUretSade(), true, "xlsx");
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }

        /// <summary>İmzalı Çıktı (eski btnImzaliCikti_Click → rapor 7500-6): seçili birimin koordinasyon raporu (şablon BHMv2\KoordinasyonRapor.xltx).</summary>
        private static IResult RaporImzali(HttpContext baglam, IWebHostEnvironment ortam, int yil, string eko, string sektor, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out _);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            if (Bos(eko, sektor, birim)) return Results.Json(new { hata = "Lütfen seçim yapınız!" });
            try
            {
                AsposeLisans.Yukle();
                string dosya = KoordinasyonRaporu.Hazirla(yil, birim.Trim(), Ayar.Oku("RaporSablonYol") + "/BHMv2/KoordinasyonRapor.xltx",
                    TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMAD"));
                return DosyaIslem.DosyaGonder(dosya, "KoordinasyonRaporu_" + yil, true, "xlsx");
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }
    }
}
