using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;
using TNS.ORG;

namespace Istemci.Organizasyon
{
    public class GenelGorevKayitIstegi
    {
        /// <summary>Görevin anahtarı: "birimKodu_sıraNo" (genel görevlerde birim kodu 0).</summary>
        public string Kod { get; set; }
        public string Ad { get; set; }
        public string Aciklama { get; set; }
        public int MaxGorev { get; set; }
        public bool Zorunlu { get; set; }
        /// <summary>İlişkili birim türleri (BirimTip değerleri); yalnızca genel görevlerde.</summary>
        public int[] Birimler { get; set; }
    }

    public class GenelGorevYetkiIstegi
    {
        public string Kod { get; set; }
        /// <summary>Yazılım yetkisinin türü: SabitGorevlerArac.birimGorevler içindeki sırası.</summary>
        public int Indis { get; set; }
        /// <summary>Görevdeki yetkinin sıra numarası; yeni yetkide 0.</summary>
        public int SiraNo { get; set; }
        public string Aciklama { get; set; }
        /// <summary>Damga adı -> değer. Onay kutuları "true"/"false", çoklu seçimler virgülle ayrılmış kodlardır.</summary>
        public Dictionary<string, string> Degerler { get; set; }
    }

    /// <summary>
    /// Genel Görev Tanımlama sayfasının (eski Organizasyon/GenelGorevAna.aspx + GenelGorevDetay.aspx + YETxxx.aspx sayfaları)
    /// JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/genelgorev/liste, /oku?kod=, /yetki?kod=&amp;indis=&amp;siraNo=
    ///   POST /api/genelgorev/yeni, /kaydet, /yetki-kaydet, /yetki-sil
    /// Yalnızca sistem yöneticisi kullanabilir (menüdeki Yetki="1").
    /// </summary>
    public static class GenelGorevUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/genelgorev/liste", Liste);
            app.MapGet("/api/genelgorev/oku", Oku);
            app.MapGet("/api/genelgorev/yetki", YetkiOku);
            app.MapPost("/api/genelgorev/yeni", Yeni);
            app.MapPost("/api/genelgorev/kaydet", Kaydet);
            app.MapPost("/api/genelgorev/yetki-kaydet", YetkiKaydet);
            app.MapPost("/api/genelgorev/yetki-sil", YetkiSil);
        }

        public static bool YetkiVar(Kullanici kullanan)
        {
            return kullanan != null && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        /// <summary>İstek için kullanıcıyı ve yetkisini denetler; uygun değilse döndürülecek yanıtı verir.</summary>
        private static IResult Denetle(HttpContext baglam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        /// <summary>"birimKodu_sıraNo" anahtarını ayırır (eski GenelGorevDetay: alt çizgi yoksa değer sıra numarasıdır).</summary>
        private static void AnahtarAyir(string kod, out int birimKod, out int siraNo)
        {
            string birimKodStr = "", siraNoStr = "";
            OrtakFonksiyonlar.StringiIkiyeBol(kod ?? "", "_", ref birimKodStr, ref siraNoStr);
            if (string.IsNullOrEmpty(siraNoStr))
            {
                birimKod = -1;
                siraNo = OrtakFonksiyonlar.ConvertToInt(birimKodStr, -1);
            }
            else
            {
                birimKod = OrtakFonksiyonlar.ConvertToInt(birimKodStr, -1);
                siraNo = OrtakFonksiyonlar.ConvertToInt(siraNoStr, -1);
            }
        }

        // ------------------------------------------------------------------ görevler

        private static List<object> GorevSatirlari(BirimGenelGorev[] gorevler)
        {
            List<object> satirlar = new List<object>();
            if (gorevler != null)
                foreach (BirimGenelGorev g in gorevler)
                    satirlar.Add(new { kod = g.Anahtar, ad = g.ad });
            return satirlar;
        }

        private static IResult Liste(HttpContext baglam)
        {
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;

            IORGServis servis = TNS.ORG.Arac.Tanimla();
            BirimGenelGorev[] gorevler = servis.GenelGorevListele(kullanan, null, false);
            // Hiç görev yoksa eski sayfadaki gibi boş bir görev açılır.
            if (gorevler == null || gorevler.Length == 0)
            {
                servis.YeniGorevEkle(kullanan, null);
                gorevler = servis.GenelGorevListele(kullanan, null, false);
            }
            return Results.Json(new { satirlar = GorevSatirlari(gorevler) });
        }

        private static async Task<IResult> Yeni(HttpContext baglam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;

            try
            {
                IORGServis servis = TNS.ORG.Arac.Tanimla();
                BirimGenelGorev yeni = servis.YeniGorevEkle(kullanan, null);
                return Results.Json(new { basarili = true, kod = yeni.Anahtar, satirlar = GorevSatirlari(servis.GenelGorevListele(kullanan, null, false)) });
            }
            catch (Exception)
            {
                return Hata("Yeni görev eklenemedi.");
            }
        }

        private static IResult Oku(HttpContext baglam, string kod)
        {
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;

            AnahtarAyir(kod, out int birimKod, out int siraNo);
            OrganizasyonBirim ob = birimKod > 0 ? new OrganizasyonBirim { kod = birimKod } : null;
            BirimGenelGorev gorev = TNS.ORG.Arac.Tanimla().GenelGorevOku(kullanan, ob, siraNo);
            if (gorev == null) return Results.NotFound();
            ob = gorev.orgBirim;

            // Genel görevde ilişkili birim türleri seçilir; bir birime bağlı görevde birimin kendi türleri geçerlidir.
            List<object> birimTurleri = null;
            BirimTip[] birimTipler = null;
            if (ob == null)
            {
                birimTurleri = new List<object>();
                foreach (KeyValuePair<BirimTip, string> bt in BirimTurleri())
                    birimTurleri.Add(new { kod = (int)bt.Key, ad = bt.Value, secili = gorev.iliskiliBirimler != null && Array.IndexOf(gorev.iliskiliBirimler, bt.Key) >= 0 });
            }
            else
                birimTipler = ob.YazilimYetkiBirimTipBul();

            List<object> islemler = new List<object>();
            for (int i = 0; i < SabitGorevlerArac.birimGorevler.Length; i++)
            {
                BirimGorev bg = SabitGorevlerArac.birimGorevler[i];
                if (bg != null && bg.BirimTipVarMi(birimTipler))
                    islemler.Add(new { indis = i, ad = bg.GorevAd });
            }

            return Results.Json(new
            {
                kod = gorev.Anahtar,
                ad = gorev.ad,
                aciklama = gorev.aciklama,
                maxGorev = gorev.maxGorev,
                zorunlu = gorev.zorunlu,
                birimTurleri = birimTurleri,
                islemler = islemler,
                yetkiler = YetkiSatirlari(gorev)
            });
        }

        private static List<object> YetkiSatirlari(BirimGenelGorev gorev)
        {
            List<object> satirlar = new List<object>();
            if (gorev.yazilimYetkiler != null)
                foreach (YazilimYetki y in gorev.yazilimYetkiler)
                {
                    string ad = null;
                    if (y.birimGorevlerIndis >= 0 && y.birimGorevlerIndis < SabitGorevlerArac.birimGorevler.Length && SabitGorevlerArac.birimGorevler[y.birimGorevlerIndis] != null)
                        ad = SabitGorevlerArac.birimGorevler[y.birimGorevlerIndis].GorevAd;
                    satirlar.Add(new { siraNo = y.siraNo, indis = y.birimGorevlerIndis, ad = ad, aciklama = y.gorevAciklama });
                }
            return satirlar;
        }

        private static KeyValuePair<BirimTip, string>[] BirimTurleri()
        {
            return new[]
            {
                new KeyValuePair<BirimTip, string>(BirimTip.ButceBirimi, OrganizasyonSemaResI.PERBRM001),
                new KeyValuePair<BirimTip, string>(BirimTip.ButceKurumu, OrganizasyonSemaResI.PERBRM002),
                new KeyValuePair<BirimTip, string>(BirimTip.EvrakBirimi, OrganizasyonSemaResI.PERBRM003),
                new KeyValuePair<BirimTip, string>(BirimTip.HarcamaBirimi, OrganizasyonSemaResI.PERBRM004),
                new KeyValuePair<BirimTip, string>(BirimTip.PersonelBirimi, OrganizasyonSemaResI.PERBRM005),
                new KeyValuePair<BirimTip, string>(BirimTip.TasinirBirimi, OrganizasyonSemaResI.PERBRM006),
                new KeyValuePair<BirimTip, string>(BirimTip.Ambar, OrganizasyonSemaResI.PERBRM007)
            };
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, GenelGorevKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            AnahtarAyir(istek.Kod, out int birimKod, out int siraNo);
            IORGServis servis = TNS.ORG.Arac.Tanimla();

            BirimGenelGorev gorev = new BirimGenelGorev(siraNo, istek.Ad ?? "");
            gorev.orgBirim = null;
            if (birimKod > 0)
            {
                // Birime bağlı görev: birim okunamazsa eski sayfadaki gibi genel görev sayılır.
                try
                {
                    OrganizasyonBirim ob = new OrganizasyonBirim { kod = birimKod };
                    ObjectArray oa = servis.OrganizasyonBirimListele(ob, false, false);
                    if (oa.ObjeSayisi == 1) gorev.orgBirim = (OrganizasyonBirim)oa[0];
                }
                catch (Exception)
                {
                    gorev.orgBirim = null;
                }
            }
            gorev.aciklama = istek.Aciklama ?? "";
            gorev.maxGorev = Math.Max(-1, Math.Min(100, istek.MaxGorev));
            gorev.zorunlu = istek.Zorunlu;
            gorev.iliskiliBirimler = null;
            if (gorev.orgBirim == null && istek.Birimler != null && istek.Birimler.Length > 0)
            {
                List<BirimTip> turler = new List<BirimTip>();
                foreach (int b in istek.Birimler)
                    turler.Add(Enum.IsDefined(typeof(BirimTip), b) ? (BirimTip)b : BirimTip.TANIMSIZ);
                gorev.iliskiliBirimler = turler.ToArray();
            }

            Sonuc sonuc = servis.GenelGorevKaydet(kullanan, gorev);
            if (!sonuc.islemSonuc) return Hata("Kayıt işlemi yapılamadı. " + HataMetni(sonuc));
            return Results.Json(new { basarili = true, kod = gorev.OrgKod + "_" + gorev.siraNo, ad = gorev.ad, bilgi = "Kayıt işlemi tamamlandı." });
        }

        // ------------------------------------------------------------------ yazılım yetkileri

        /// <summary>Bir yazılım yetkisinin penceresi: alan tanımları, seçenek listeleri ve (varsa) kayıtlı değerler.</summary>
        private static IResult YetkiOku(HttpContext baglam, string kod, int indis, int siraNo)
        {
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (indis < 0 || indis >= SabitGorevlerArac.birimGorevler.Length || SabitGorevlerArac.birimGorevler[indis] == null) return Results.NotFound();

            BirimGorev bg = SabitGorevlerArac.birimGorevler[indis];
            YetkiAlan[] tanim = YetkiTanimlari.Bul(bg.SayfaAd);
            if (tanim == null) return Results.Json(new { hata = bg.GorevAd + ": bu yetki türünün alan tanımı yeni sisteme eklenmemiş (" + bg.SayfaAd + ")." });

            GenelGorevYetki ggy = null;
            if (siraNo > 0)
            {
                AnahtarAyir(kod, out int birimKod, out int gorevSiraNo);
                IORGServis servis = TNS.ORG.Arac.Tanimla();
                BirimGenelGorev bgg = servis.GenelGorevOku(kullanan, BirimGorev.OrganizasyonelBirimBul(servis, birimKod), gorevSiraNo);
                if (bgg != null) ggy = servis.GenelGorevYetkiOku(kullanan, bgg, siraNo);
            }

            IKYMServis servisKym = null;
            bool listeOkunamadi = false;
            List<object> alanlar = new List<object>();
            foreach (YetkiAlan a in tanim)
            {
                List<object> secenekler = null;
                if (a.YetkiTipi > 0)
                {
                    secenekler = new List<object>();
                    if (servisKym == null) servisKym = TNS.KYM.Arac.Tanimla();
                    ListeSatir[] liste = null;
                    try
                    {
                        liste = servisKym.YetkiTipiGirisListesi(kullanan, TNS.KYM.Arac.YetkiTipiBul(a.YetkiTipi), DateTime.Now.Year, "");
                    }
                    catch (Exception)
                    {
                        // Listeyi sağlayan modül çalışmıyorsa (ör. henüz taşınmamış Evrak) pencere yine açılır;
                        // kayıtlı değerler korunur, yeni seçim yapılamaz.
                        listeOkunamadi = true;
                    }
                    if (liste != null)
                        foreach (ListeSatir li in liste)
                            if (li != null && !string.IsNullOrEmpty(li.kod)) secenekler.Add(new { kod = li.kod, ad = li.ad });
                }
                else if (a.Secenekler != null)
                {
                    secenekler = new List<object>();
                    foreach (KeyValuePair<string, string> s in a.Secenekler) secenekler.Add(new { kod = s.Key, ad = s.Value });
                }

                string deger;
                switch (a.Tur)
                {
                    case YetkiAlanTur.Evet: deger = ggy != null && ggy.DegerAlBool(a.Damga) ? "true" : "false"; break;
                    case YetkiAlanTur.EvetSayi: deger = ggy != null && ggy.DegerAl(a.Damga) == a.AcikDeger.ToString() ? "true" : "false"; break;
                    case YetkiAlanTur.Radyo: deger = (ggy != null ? ggy.DegerAl(a.Damga, a.Varsayilan) : a.Varsayilan).ToString(); break;
                    default: deger = ggy != null ? (ggy.DegerAl(a.Damga) ?? "") : ""; break;
                }

                alanlar.Add(new { damga = a.Damga, tur = a.Tur.ToString(), etiket = a.Etiket, grup = a.Grup, secenekler = secenekler, deger = deger });
            }

            return Results.Json(new { indis = indis, siraNo = siraNo, baslik = bg.GorevAd, aciklama = ggy != null ? ggy.yetkiAd : "", alanlar = alanlar,
                uyari = listeOkunamadi ? "Bazı seçenek listeleri okunamadı (listeyi sağlayan modül henüz çalışmıyor). Kayıtlı değerler korunur." : null });
        }

        private static async Task<IResult> YetkiKaydet(HttpContext baglam, GenelGorevYetkiIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Indis < 0 || istek.Indis >= SabitGorevlerArac.birimGorevler.Length || SabitGorevlerArac.birimGorevler[istek.Indis] == null)
                return Results.BadRequest();

            YetkiAlan[] tanim = YetkiTanimlari.Bul(SabitGorevlerArac.birimGorevler[istek.Indis].SayfaAd);
            if (tanim == null) return Results.BadRequest();

            AnahtarAyir(istek.Kod, out int birimKod, out int gorevSiraNo);
            GenelGorevYetki yetki = new GenelGorevYetki();
            yetki.birimGenelGorev = new BirimGenelGorev();
            yetki.birimGenelGorev.orgBirim = BirimGorev.OrganizasyonelBirimOl(birimKod);
            yetki.birimGenelGorev.siraNo = gorevSiraNo;
            yetki.yetkiAd = istek.Aciklama ?? "";
            yetki.yazYetkiIndis = istek.Indis;
            yetki.yazYetkiSiraNo = istek.SiraNo;
            yetki.yetkiDamgalar = new Hashtable();

            foreach (YetkiAlan a in tanim)
            {
                string deger = null;
                if (istek.Degerler != null) istek.Degerler.TryGetValue(a.Damga, out deger);
                deger = deger ?? "";
                switch (a.Tur)
                {
                    case YetkiAlanTur.Evet: yetki.DamgaEkle(a.Damga, deger == "true"); break;
                    case YetkiAlanTur.EvetSayi: yetki.DamgaEkle(a.Damga, deger == "true" ? a.AcikDeger : 0); break;
                    case YetkiAlanTur.Radyo: yetki.DamgaEkle(a.Damga, OrtakFonksiyonlar.ConvertToInt(deger, 0)); break;
                    default: yetki.DamgaEkle(a.Damga, deger); break;
                }
            }

            Sonuc sonuc = TNS.ORG.Arac.Tanimla().GenelGorevYetkiKaydet(kullanan, yetki);
            if (!sonuc.islemSonuc) return Hata("Kayıt yapılamadı! " + HataMetni(sonuc));
            return Results.Json(new { basarili = true, siraNo = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, istek.SiraNo), bilgi = "Kayıt işlemi başarılı." });
        }

        private static async Task<IResult> YetkiSil(HttpContext baglam, GenelGorevYetkiIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            AnahtarAyir(istek.Kod, out int birimKod, out int gorevSiraNo);
            Sonuc sonuc = TNS.ORG.Arac.Tanimla().GenelGorevYetkiSil(kullanan, birimKod, gorevSiraNo, istek.SiraNo);
            if (!sonuc.islemSonuc) return Hata("Silme yapılamadı! " + HataMetni(sonuc));
            return Results.Json(new { basarili = true, bilgi = "Silme işlemi başarılı." });
        }

        // ------------------------------------------------------------------ yardımcılar

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin.Trim() });
        }

        /// <summary>Servisin hata metni; yakalanmış bir istisnanın dökümüyse boş.</summary>
        private static string HataMetni(Sonuc sonuc)
        {
            string hata = sonuc.hataStr ?? "";
            return hata.Contains("Exception") || hata.Contains("   at ") ? "" : hata;
        }
    }
}
